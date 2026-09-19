using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Microsoft.Data.Sqlite;

namespace GXX.M2Server.GameCenter;

/// <summary>BDE 数据集字段类型（ftString/ftInteger/TFloatField 等效）。</summary>
public enum GBDEFieldType
{
    ftString,
    ftInteger,
    ftFloat,
}

/// <summary>TField 等效（字段名/类型/字符串长度 + 行取值）。</summary>
public class GBDEField
{
    public string FieldName = "";
    public GBDEFieldType DataType;
    public int Size; // TStringField.Size
    public string AsString = "";
    public int AsInteger;
    public double AsFloat;
}

/// <summary>
/// TDataSet/TADODataSet/TParadoxDataSet 读取接缝（BDE 与 Jet 驱动为宿主侧能力，
/// 托管版以行迭代器注入；FieldCount/First/Next/Eof 与 Delphi 遍历语义一致）。
/// Fields 每行刷新取值。
/// </summary>
public interface IGBDEDataSet
{
    int FieldCount { get; }
    GBDEField[] Fields { get; }
    bool Eof { get; }
    void First();
    void Next();
}

/// <summary>
/// GameCenter GBDEtoSqlite.pas TFrmBDEToSqlite 1:1（批次J45 收尾）：
/// DateSetToSqlite 建表（TEXT(尺寸)/INTEGER，TFloatField 亦建 INTEGER）+ DROP IF EXISTS + 逐行
/// OrderBindText/OrderBindInt/OrderBindInt64(Round) 迁移；FBtnbtn1Click 校验链与 HeroDB.MDB
/// (boACC→ADO) / Paradox 双路径；Open 预填源目录与 BmM2.db；GetFieldNameInGomArray 原文直返。
/// </summary>
public sealed class TFrmBDEToSqlite : System.Windows.Forms.Form
{
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.Label lbl2 = null!;
    public System.Windows.Forms.Label lbl3 = null!;
    public System.Windows.Forms.TextBox btnSrc = null!;
    public System.Windows.Forms.TextBox btnDest = null!;
    public System.Windows.Forms.Button FBtnbtn1 = null!;
    public System.Windows.Forms.CheckBox CheckBox1 = null!;

    /// <summary>Delphi ModalResult 等效（mrNone/mrOk）。</summary>
    public int ModalResult;
    public const int mrNone = 0;
    public const int mrOk = 1;

    public string LastShowMessage;
    public string LastFocus = "";

    /// <summary>Paradox 数据集接缝（按 FileDir+TableName 打开 .DB；托管版注入行迭代器）。</summary>
    public Func<string, string, IGBDEDataSet?>? ParadoxDataSetOpener;

    /// <summary>ADO 数据集接缝（boACC 路径：连接串 + CommandText）。</summary>
    public Func<string, string, IGBDEDataSet?>? AdoDataSetOpener;

    /// <summary>选择目录/保存文件对话框接缝。</summary>
    public Func<string, string?>? SelectDirectoryHandler;
    public Func<string, string?>? SaveFileDialogHandler;

    public TFrmBDEToSqlite()
    {
        Text = "BDE数据库转Sqlite";
        Width = 560;
        Height = 220;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        lbl1 = new System.Windows.Forms.Label { Text = "源数据库目录:", Left = 12, Top = 16, AutoSize = true };
        Controls.Add(lbl1);
        btnSrc = new System.Windows.Forms.TextBox { Left = 120, Top = 12, Width = 400 };
        Controls.Add(btnSrc);
        lbl2 = new System.Windows.Forms.Label { Text = "目标数据库:", Left = 12, Top = 46, AutoSize = true };
        Controls.Add(lbl2);
        btnDest = new System.Windows.Forms.TextBox { Left = 120, Top = 42, Width = 400 };
        Controls.Add(btnDest);
        lbl3 = new System.Windows.Forms.Label { Text = "转换 StdItems/Magic/Monster 三表到 Sqlite", Left = 12, Top = 76, AutoSize = true };
        Controls.Add(lbl3);
        CheckBox1 = new System.Windows.Forms.CheckBox { Text = "完成后自动关闭", Left = 120, Top = 70, AutoSize = true };
        Controls.Add(CheckBox1);
        FBtnbtn1 = new System.Windows.Forms.Button { Text = "开始转换", Left = 120, Top = 110, Width = 120 };
        FBtnbtn1.Click += (_, _) => FBtnbtn1Click();
        Controls.Add(FBtnbtn1);

        btnSrc.DoubleClick += (_, _) => BtnSrcButtonClick();
        btnDest.DoubleClick += (_, _) => BtnDestButtonClick();
    }

    // ================= Delphi 1:1 =================

    /// <summary>btnSrcButtonClick（SelectDirectory('请选择BDE数据库目录')）。</summary>
    public void BtnSrcButtonClick()
    {
        string dir = btnSrc.Text;
        var chosen = SelectDirectoryHandler?.Invoke(dir);
        if (chosen != null)
            btnSrc.Text = chosen;
    }

    /// <summary>btnDestButtonClick（SaveDialog 'Sqlite数据库文件(*.db)|*.db'）。</summary>
    public void BtnDestButtonClick()
    {
        var chosen = SaveFileDialogHandler?.Invoke(btnDest.Text);
        if (chosen != null)
            btnDest.Text = chosen;
    }

    /// <summary>GetFieldNameInGomArray 1:1（原文直返字段名，映射逻辑保留为恒等）。</summary>
    public static string GetFieldNameInGomArray(string fieldName, string tableName) => fieldName;

    /// <summary>
    /// DateSetToSqlite 1:1（93-183）：先建表（字符串 TEXT(尺寸)、整数与浮点均 INTEGER——浮点入库
    /// 经 Round 转 Int64），Exec DROP TABLE IF EXISTS + CREATE，再 prepare insert 逐行绑定。
    /// </summary>
    public static void DateSetToSqlite(string tableName, IGBDEDataSet dataSet, SqliteConnection sqliteDB)
    {
        if (dataSet.FieldCount == 0)
            return;
        string s1 = "";
        string s2 = "";
        string s3 = "";
        var fields = dataSet.Fields;
        for (int i = 0; i < dataSet.FieldCount; i++)
        {
            var field = fields[i];
            string sFieldName = GetFieldNameInGomArray(field.FieldName, tableName);
            switch (field.DataType)
            {
                case GBDEFieldType.ftString:
                    s1 += $"\"{sFieldName}\" TEXT({field.Size}),";
                    s2 += sFieldName + ",";
                    s3 += "@p" + i + ",";
                    break;
                case GBDEFieldType.ftInteger:
                    s1 += $"\"{sFieldName}\" INTEGER,";
                    s2 += sFieldName + ",";
                    s3 += "@p" + i + ",";
                    break;
                case GBDEFieldType.ftFloat:
                    s1 += $"\"{sFieldName}\" INTEGER,";
                    s2 += sFieldName + ",";
                    s3 += "@p" + i + ",";
                    break;
                default:
                    throw new Exception("不识别的字段类型(" + field.GetType().Name + "): " + field.FieldName);
            }
        }
        // 创建表
        s1 = s1.Remove(s1.Length - 1);
        s1 = "CREATE TABLE \"" + tableName + "\" ( " + s1 + ");";
        Exec(sqliteDB, "DROP TABLE IF EXISTS \"" + tableName + "\";" + Environment.NewLine + s1);

        s2 = s2.Remove(s2.Length - 1);
        s3 = s3.Remove(s3.Length - 1);

        // 处理记录
        using var sm = sqliteDB.CreateCommand();
        sm.CommandText = "insert into " + tableName + "(" + s2 + ") values(" + s3 + ");";
        sm.Prepare();
        for (int p = 0; p < dataSet.FieldCount; p++)
        {
            var parameter = sm.CreateParameter();
            parameter.ParameterName = "@p" + p;
            sm.Parameters.Add(parameter);
        }
        dataSet.First();
        while (!dataSet.Eof)
        {
            for (int i = 0; i < dataSet.FieldCount; i++)
            {
                var field = fields[i];
                switch (field.DataType)
                {
                    case GBDEFieldType.ftString:
                        sm.Parameters["@p" + i].Value = field.AsString;
                        break;
                    case GBDEFieldType.ftInteger:
                        sm.Parameters["@p" + i].Value = field.AsInteger;
                        break;
                    case GBDEFieldType.ftFloat:
                        sm.Parameters["@p" + i].Value = (long)Math.Round(field.AsFloat);
                        break;
                }
            }
            sm.ExecuteNonQuery();
            dataSet.Next();
        }
    }

    private static void Exec(SqliteConnection db, string sql)
    {
        using var cmd = db.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    /// <summary>FBtnbtn1Click 1:1（四段校验 → boACC 判定 → 建库关同步 → ADO/Paradox 双路径三表迁移）。</summary>
    public void FBtnbtn1Click()
    {
        if (btnSrc.Text.Length == 0)
        {
            LastShowMessage = "源数据库目录不能为空";
            LastFocus = "btnSrc";
            return;
        }
        if (!Directory.Exists(btnSrc.Text))
        {
            LastShowMessage = "源数据库目录不存在";
            LastFocus = "btnSrc";
            return;
        }
        if (btnDest.Text.Length == 0)
        {
            LastShowMessage = "目标数据库不能为空";
            LastFocus = "btnDest";
            return;
        }

        bool boACC = false;
        if (File.Exists(btnSrc.Text + "\\HeroDB.MDB"))
            boACC = true;

        // FillChar 复位三张核对表
        Array.Clear(GBDEtoSqlite.StdCheckFieldName);
        Array.Clear(GBDEtoSqlite.MagCheckFieldName);
        Array.Clear(GBDEtoSqlite.MonCheckFieldName);

        string fileDir = IncludeTrailingPathDelimiter(btnSrc.Text);
        CheckBox1.Enabled = false;
        FBtnbtn1.Enabled = false;
        try
        {
            using var sqlite3DB = new SqliteConnection("Data Source=" + btnDest.Text);
            sqlite3DB.Open();
            Exec(sqlite3DB, "PRAGMA synchronous = OFF;");

            if (boACC)
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + btnSrc.Text + "\\HeroDB.MDB" + ";Persist Security Info=False";
                MigrateTable("StdItems", AdoDataSetOpener, connectionString, "SELECT * FROM StdItems", sqlite3DB);
                MigrateTable("Magic", AdoDataSetOpener, connectionString, "SELECT * FROM Magic", sqlite3DB);
                MigrateTable("Monster", AdoDataSetOpener, connectionString, "SELECT * FROM Monster", sqlite3DB);
            }
            else
            {
                MigrateTable("StdItems", ParadoxDataSetOpener, fileDir + "StdItems.DB", null, sqlite3DB);
                MigrateTable("Magic", ParadoxDataSetOpener, fileDir + "Magic.DB", null, sqlite3DB);
                MigrateTable("Monster", ParadoxDataSetOpener, fileDir + "Monster.DB", null, sqlite3DB);
            }
        }
        finally
        {
            CheckBox1.Enabled = true;
            FBtnbtn1.Enabled = true;
        }
        ModalResult = mrOk;
        LastShowMessage = "数据库转换完成";
    }

    private static void MigrateTable(string tableName, Func<string, string, IGBDEDataSet?>? opener,
        string openKey, string? commandText, SqliteConnection db)
    {
        var dataSet = commandText == null ? opener?.Invoke(openKey, tableName) : opener?.Invoke(openKey, commandText);
        if (dataSet == null)
            return;
        DateSetToSqlite(tableName, dataSet, db);
    }

    private static string IncludeTrailingPathDelimiter(string path)
        => path.EndsWith("\\") ? path : path + "\\";

    /// <summary>Open 1:1（预填 Mud2\DB 源目录与 BmM2.db 目标）。</summary>
    public void Open(string fileDir, bool showModal = true)
    {
        string sDir = IncludeTrailingPathDelimiter(fileDir);
        btnSrc.Text = sDir + "Mud2\\DB\\";
        btnDest.Text = sDir + "Mud2\\DB\\BmM2.db";
        ModalResult = mrNone;
        if (showModal)
            ShowDialog();
    }
}
