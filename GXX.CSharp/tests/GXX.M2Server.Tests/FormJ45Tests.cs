using GXX.M2Server.GameCenter;
using GXX.M2Server.Forms;
using Microsoft.Data.Sqlite;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J45：GBDEtoSqlite.pas 收尾（DateSetToSqlite 数据行迁移 + TFrmBDEToSqlite 工具窗体）1:1 测试。</summary>
public sealed class BDEToSqliteTests : IDisposable
{
    private readonly string _dir;

    public BDEToSqliteTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j45_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    /// <summary>内存行数据集（TDataSet 遍历语义等效）。</summary>
    private sealed class MemDataSet : IGBDEDataSet
    {
        public GBDEField[] Fields { get; }
        private readonly List<object?[]> _rows;
        private int _index;

        public MemDataSet(GBDEField[] fields, List<object?[]> rows)
        {
            Fields = fields;
            _rows = rows;
            _index = -1;
        }

        public int FieldCount => Fields.Length;
        public bool Eof => _index >= _rows.Count;

        public void First()
        {
            _index = 0;
            LoadRow();
        }

        public void Next()
        {
            _index++;
            LoadRow();
        }

        private void LoadRow()
        {
            if (Eof)
                return;
            for (int i = 0; i < Fields.Length; i++)
            {
                var v = _rows[_index][i];
                switch (Fields[i].DataType)
                {
                    case GBDEFieldType.ftString: Fields[i].AsString = (string?)v ?? ""; break;
                    case GBDEFieldType.ftInteger: Fields[i].AsInteger = (int?)v ?? 0; break;
                    case GBDEFieldType.ftFloat: Fields[i].AsFloat = (double?)v ?? 0; break;
                }
            }
        }
    }

    private static GBDEField StrField(string name, int size) => new() { FieldName = name, DataType = GBDEFieldType.ftString, Size = size };
    private static GBDEField IntField(string name) => new() { FieldName = name, DataType = GBDEFieldType.ftInteger };
    private static GBDEField FloatField(string name) => new() { FieldName = name, DataType = GBDEFieldType.ftFloat };

    [Fact]
    public void DateSetToSqlite_CreatesTable_And_MigratesRows()
    {
        string dbPath = Path.Combine(_dir, "BmM2.db");
        using (var db = new SqliteConnection("Data Source=" + dbPath))
        {
            db.Open();
            var fields = new[] { StrField("Name", 14), IntField("Level"), FloatField("Weight") };
            var dataSet = new MemDataSet(fields, new List<object?[]>
            {
                new object?[] { "木剑", 3, 12.4 },
                new object?[] { "金创药", 10, 7.5 },
            });
            TFrmBDEToSqlite.DateSetToSqlite("StdItems", dataSet, db);
        }

        using (var check = new SqliteConnection("Data Source=" + dbPath))
        {
            check.Open();
            // 表结构：字符串 TEXT(14)、整数与浮点均 INTEGER（TFloatField 建表原文形态）
            var schema = new List<(string Name, string Type)>();
            using (var cmd = check.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info('StdItems')";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    schema.Add((reader.GetString(1), reader.GetString(2)));
            }
            Assert.Equal(3, schema.Count);
            Assert.Equal(("Name", "TEXT(14)"), schema[0]); // Delphi TEXT(Size) 声明原样入库
            Assert.Equal(("Level", "INTEGER"), schema[1]);
            Assert.Equal(("Weight", "INTEGER"), schema[2]);

            // 行数据：浮点 Round → Int64
            var rows = new List<(string Name, long Level, long Weight)>();
            using (var cmd = check.CreateCommand())
            {
                cmd.CommandText = "SELECT Name, Level, Weight FROM StdItems ORDER BY rowid";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    rows.Add((reader.GetString(0), reader.GetInt64(1), reader.GetInt64(2)));
            }
            Assert.Equal(2, rows.Count);
            Assert.Equal(("木剑", 3, 12), rows[0]);
            Assert.Equal(("金创药", 10, 8), rows[1]); // Round(7.5)=8（银行家舍入→8）
        }
    }

    [Fact]
    public void DateSetToSqlite_DropAndRecreate()
    {
        string dbPath = Path.Combine(_dir, "BmM2.db");
        using (var db = new SqliteConnection("Data Source=" + dbPath))
        {
            db.Open();
            using (var cmd = db.CreateCommand())
                cmd.ExecuteNonQuery(); // open enough

            var fields1 = new[] { StrField("Name", 8) };
            TFrmBDEToSqlite.DateSetToSqlite("Magic", new MemDataSet(fields1, new List<object?[]> { new object?[] { "火球术" } }), db);
            // 二次迁移：DROP IF EXISTS 重建后旧行不残留
            TFrmBDEToSqlite.DateSetToSqlite("Magic", new MemDataSet(fields1, new List<object?[]> { new object?[] { "治愈术" } }), db);

            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Magic";
                Assert.Equal(1L, (long)(cmd.ExecuteScalar() ?? 0));
            }
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM Magic";
                Assert.Equal("治愈术", cmd.ExecuteScalar()?.ToString());
            }
        }
    }

    [Fact]
    public void DateSetToSqlite_EmptyFields_Exits()
    {
        string dbPath = Path.Combine(_dir, "BmM2.db");
        using var db = new SqliteConnection("Data Source=" + dbPath);
        db.Open();
        TFrmBDEToSqlite.DateSetToSqlite("Empty", new MemDataSet(System.Array.Empty<GBDEField>(), new List<object?[]>()), db);
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Empty'";
        Assert.Equal(0L, (long)(cmd.ExecuteScalar() ?? 1));
    }

    [Fact]
    public void FBtnbtn1Click_ValidationChain()
    {
        var form = StaRunner.New(() => new TFrmBDEToSqlite());
        StaRunner.New(() =>
        {
            // 1：源目录为空
            form.FBtnbtn1Click();
            Assert.Equal("源数据库目录不能为空", form.LastShowMessage);
            Assert.Equal("btnSrc", form.LastFocus);
            Assert.Equal(TFrmBDEToSqlite.mrNone, form.ModalResult);

            // 2：源目录不存在
            form.btnSrc.Text = Path.Combine(_dir, "不存在目录");
            form.FBtnbtn1Click();
            Assert.Equal("源数据库目录不存在", form.LastShowMessage);
            Assert.Equal("btnSrc", form.LastFocus);

            // 3：目标为空
            Directory.CreateDirectory(Path.Combine(_dir, "src"));
            form.btnSrc.Text = Path.Combine(_dir, "src");
            form.FBtnbtn1Click();
            Assert.Equal("目标数据库不能为空", form.LastShowMessage);
            Assert.Equal("btnDest", form.LastFocus);
        });
    }

    [Fact]
    public void FBtnbtn1Click_ParadoxPath_MigratesThreeTables()
    {
        string srcDir = Path.Combine(_dir, "src");
        Directory.CreateDirectory(srcDir);
        string destDb = Path.Combine(_dir, "BmM2.db");
        foreach (var dbFile in new[] { "StdItems.DB", "Magic.DB", "Monster.DB" })
            File.WriteAllText(Path.Combine(srcDir, dbFile), "stub"); // Paradox 源文件存在性

        var form = StaRunner.New(() => new TFrmBDEToSqlite());
        StaRunner.New(() =>
        {
            form.btnSrc.Text = srcDir;
            form.btnDest.Text = destDb;
            form.ParadoxDataSetOpener = (fileName, tableName) =>
            {
                var fields = new[] { StrField("Name", 14), IntField("Level") };
                var ds = new MemDataSet(fields, new List<object?[]> { new object?[] { tableName + "行", 1 } });
                Assert.True(File.Exists(fileName)); // .DB 路径形态 FileDir + 'StdItems.DB' 等
                return ds;
            };

            form.FBtnbtn1Click();
        });

        Assert.Equal("数据库转换完成", form.LastShowMessage);
        Assert.Equal(TFrmBDEToSqlite.mrOk, form.ModalResult);
        Assert.True(form.CheckBox1.Enabled); // finally 恢复
        Assert.True(form.FBtnbtn1.Enabled);

        using var check = new SqliteConnection("Data Source=" + destDb);
        check.Open();
        foreach (var table in new[] { "StdItems", "Magic", "Monster" })
        {
            using var cmd = check.CreateCommand();
            cmd.CommandText = $"SELECT Name, Level FROM {table}";
            using var reader = cmd.ExecuteReader();
            Assert.True(reader.Read());
            Assert.Equal(table + "行", reader.GetString(0));
            Assert.Equal(1, reader.GetInt64(1));
        }
    }

    [Fact]
    public void FBtnbtn1Click_BoAcc_AdoPath()
    {
        string srcDir = Path.Combine(_dir, "acc");
        Directory.CreateDirectory(srcDir);
        File.WriteAllText(Path.Combine(srcDir, "HeroDB.MDB"), "stub"); // boACC 判定文件
        string destDb = Path.Combine(_dir, "BmM2.db");

        var form = StaRunner.New(() => new TFrmBDEToSqlite());
        StaRunner.New(() =>
        {
            form.btnSrc.Text = srcDir;
            form.btnDest.Text = destDb;
            var seenCommands = new List<string>();
            form.AdoDataSetOpener = (connectionString, commandText) =>
            {
                seenCommands.Add(commandText);
                Assert.Contains("HeroDB.MDB", connectionString);
                var fields = new[] { StrField("Name", 20) };
                return new MemDataSet(fields, new List<object?[]> { new object?[] { commandText } });
            };

            form.FBtnbtn1Click();
            // ADO 路径三表命令：SELECT * FROM StdItems/Magic/Monster
            Assert.Contains("SELECT * FROM StdItems", seenCommands);
            Assert.Contains("SELECT * FROM Magic", seenCommands);
            Assert.Contains("SELECT * FROM Monster", seenCommands);
        });

        Assert.Equal("数据库转换完成", form.LastShowMessage);
        Assert.Equal(TFrmBDEToSqlite.mrOk, form.ModalResult);
    }

    [Fact]
    public void Open_Prefills_Source_And_Dest()
    {
        var form = StaRunner.New(() => new TFrmBDEToSqlite());
        StaRunner.New(() =>
        {
            form.Open("D:\\GameDir", showModal: false);
            Assert.Equal("D:\\GameDir\\Mud2\\DB\\", form.btnSrc.Text);
            Assert.Equal("D:\\GameDir\\Mud2\\DB\\BmM2.db", form.btnDest.Text);
            Assert.Equal(TFrmBDEToSqlite.mrNone, form.ModalResult);
            // 结尾带分隔符的目录不再重复追加
            form.Open("D:\\GameDir\\", showModal: false);
            Assert.Equal("D:\\GameDir\\Mud2\\DB\\", form.btnSrc.Text);
        });
    }

    [Fact]
    public void GetFieldNameInGomArray_Identity_And_CheckArrays_Reset()
    {
        var form = StaRunner.New(() => new TFrmBDEToSqlite());
        StaRunner.New(() =>
        {
            // GetFieldNameInGomArray 原文恒等返回
            Assert.Equal("Value1", TFrmBDEToSqlite.GetFieldNameInGomArray("Value1", "StdItems"));
            Assert.Equal("element", TFrmBDEToSqlite.GetFieldNameInGomArray("element", "StdItems"));

            // 核对表污染后重置
            GBDEtoSqlite.StdCheckFieldName[0] = true;
            GBDEtoSqlite.MagCheckFieldName[0] = true;
            GBDEtoSqlite.MonCheckFieldName[1] = true;
            Directory.CreateDirectory(Path.Combine(_dir, "src")); // 源目录须存在
            form.btnSrc.Text = Path.Combine(_dir, "src");
            form.btnDest.Text = Path.Combine(_dir, "reset.db");
            form.ParadoxDataSetOpener = (_, tableName) =>
            {
                var fields = new[] { StrField("Name", 10) };
                return new MemDataSet(fields, new List<object?[]> { new object?[] { tableName } });
            };
            form.FBtnbtn1Click();
            Assert.False(GBDEtoSqlite.StdCheckFieldName[0]);
            Assert.False(GBDEtoSqlite.MagCheckFieldName[0]);
            Assert.False(GBDEtoSqlite.MonCheckFieldName[1]);
        });
    }

    [Fact]
    public void DialogSeams_FillTargetTexts()
    {
        var form = StaRunner.New(() => new TFrmBDEToSqlite());
        StaRunner.New(() =>
        {
            form.btnSrc.Text = "D:\\old";
            form.SelectDirectoryHandler = old => old == "D:\\old" ? "D:\\chosen" : null;
            form.BtnSrcButtonClick();
            Assert.Equal("D:\\chosen", form.btnSrc.Text);

            form.btnDest.Text = "";
            form.SaveFileDialogHandler = _ => "D:\\new\\BmM2.db";
            form.BtnDestButtonClick();
            Assert.Equal("D:\\new\\BmM2.db", form.btnDest.Text);
        });
    }
}
