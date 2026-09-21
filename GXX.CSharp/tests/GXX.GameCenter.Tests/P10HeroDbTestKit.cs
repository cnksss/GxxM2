// ============================================================================
// 车道 p10-db-login-forms —— GHeroDBConfig 单测替身
//   · FakeHeroDB      ：IHeroDB 接缝的内存替身（GHeroDB.pas 未移植）
//   · FieldSets       ：原文 :218-372 / :400-480 逐字字段名清单（供"字段总数"断言）
//   · FolderPicker    ：SelectDirectory 的注入式目录选择替身（绝不弹真实对话框）
//   · FormSta         ：窗体测试的 STA 线程封装（WinForms 控件必须 STA）
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.GameCenter.Forms;

namespace GXX.GameCenter.Forms.Tests;

/// <summary>
/// <see cref="IHeroDB"/> 的内存替身。<c>CreateField</c> 会**真的**把字段登记进
/// <see cref="Fields"/>，因此紧随其后的 <c>CheckHeroDB</c> 能看到"字段已补齐"。
/// </summary>
public sealed class FakeHeroDB : IHeroDB
{
    /// <summary>HeroDBName 别名配置文件的落盘记录（原文 SaveHeroDBConfigFile）。</summary>
    public List<(string Name, string Path)> SavedConfigFiles { get; } = new();

    /// <summary>调用 <c>CreateField</c> 的记录（默认值 + 长度，用于断言取值规则/长度规则）。</summary>
    public List<(string Table, string Field, object Default, object Len)> CreatedFields { get; } = new();

    /// <summary>
    /// <c>Dispose</c>（原文 <c>Free</c>）当时的 <see cref="CreatedFields"/> 快照。
    /// 用它可以只看"某个 THeroDB 实例生命周期内"的建字段调用
    /// （<c>ButtonMagicFieldClick</c>/<c>ButtonMonsterFieldClick</c> 末尾的 CheckHeroDB 不会建字段，故此快照 = 按钮那一段）。
    /// </summary>
    public List<(string Table, string Field, object Default, object Len)> CreatedFieldsAtDispose { get; } = new();

    /// <summary>已存在的表名。</summary>
    public HashSet<string> Tables { get; } = new(StringComparer.Ordinal);

    /// <summary>已存在的字段（键 = <c>Table|Field</c>）。</summary>
    public HashSet<string> Fields { get; } = new(StringComparer.Ordinal);

    /// <summary>数据库别名是否存在。</summary>
    public bool AliasExists { get; set; } = true;

    /// <summary><c>CreateField</c> 的返回值（默认 False，与原文"改表失败/异常"分支一致）。</summary>
    public bool CreateFieldResult { get; set; }

    /// <summary><c>Dispose</c>（原文 <c>Free</c>）被调用次数。</summary>
    public int DisposeCount { get; private set; }

    public static string Key(string table, string field) => table + "|" + field;

    /// <summary>预置一张表及其字段。</summary>
    public FakeHeroDB SeedTable(string table, IEnumerable<string> fields)
    {
        Tables.Add(table);
        Fields.Add(Key(table, "<table>"));
        foreach (var f in fields) Fields.Add(Key(table, f));
        return this;
    }

    /// <summary>删掉某字段（模拟"数据库里缺这个字段"）。</summary>
    public FakeHeroDB RemoveField(string table, string field)
    {
        Fields.Remove(Key(table, field));
        return this;
    }

    public bool HasField(string table, string field) => Fields.Contains(Key(table, field));

    public bool HeroDBExist(string HeroDBName) => AliasExists;

    public bool TableExist(string HeroDBName, string TableName) => Tables.Contains(TableName);

    public void SaveHeroDBConfigFile(string Name, string Path) => SavedConfigFiles.Add((Name, Path));

    public bool FieldExist(string HeroDBName, string TableName, string FieldName)
        => Fields.Contains(Key(TableName, FieldName));

    public bool CreateField(string HeroDBName, string TableName, string FieldName, string Default, int Len)
    {
        CreatedFields.Add((TableName, FieldName, Default, Len));
        if (!CreateFieldResult) return false;
        Fields.Add(Key(TableName, FieldName));
        return true;
    }

    public bool CreateField(string HeroDBName, string TableName, string FieldName, int Default, byte Len)
    {
        CreatedFields.Add((TableName, FieldName, Default, Len));
        if (!CreateFieldResult) return false;
        Fields.Add(Key(TableName, FieldName));
        return true;
    }

    public void Dispose()
    {
        DisposeCount++;
        CreatedFieldsAtDispose.Clear();
        CreatedFieldsAtDispose.AddRange(CreatedFields);
    }
}

/// <summary>原文逐字字段名清单（GHeroDBConfig.pas:218-372 / :400-480）。</summary>
public static class FieldSets
{
    /// <summary>原文 :218-309 的 14 个具名 StdItems 字段（按原文顺序；含 :299/:305 的 InsuranceCurrency/InsuranceGold）。</summary>
    public static readonly string[] StdItemsNamed =
    {
        "Color", "OverLap", "HP", "MP", "Light", "Horse", "Element",
        "Expand1", "Expand2", "Expand3", "Expand4", "Expand5",
        "InsuranceCurrency", "InsuranceGold",
    };

    /// <summary>原文 :290-297 的 <c>Element1..Element24</c>（上界 24）。</summary>
    public static List<string> StdItemsElements()
    {
        var list = new List<string>();
        for (int i = 1; i <= 24; i++) list.Add("Element" + i);
        return list;
    }

    /// <summary>
    /// <c>CheckHeroDB</c>（:165-376）里 StdItems 的检查顺序（= 上框 ListBoxStdItems 的列表顺序）：
    /// :218-288 的 12 个具名（Color..Expand5）→ :290 的 <c>for I := 1 to 24</c> 得 Element1..24
    /// → :299/:305 的 InsuranceCurrency/InsuranceGold。共 38 项。
    /// </summary>
    public static List<string> StdItemsCheckOrder()
    {
        var list = new List<string>(StdItemsNamed);
        list.Remove("InsuranceCurrency");
        list.Remove("InsuranceGold");
        list.AddRange(StdItemsElements());
        list.Add("InsuranceCurrency");
        list.Add("InsuranceGold");
        return list;
    }

    /// <summary>
    /// <c>ButtonCreateStdItemsFieldClick</c>（:391-494）里 CreateField 的调用顺序：
    /// :400-:472 的 14 个具名（含 InsuranceCurrency/InsuranceGold）→ :474 的 <c>for I := 1 to 24</c>。
    /// 共 38 次调用。
    /// </summary>
    public static List<string> StdItemsCreateOrder()
    {
        var list = new List<string>(StdItemsNamed);
        list.AddRange(StdItemsElements());
        return list;
    }

    /// <summary>原文 :314-336 的 4 个 Monster 字段。</summary>
    public static readonly string[] Monster =
        { "AttackState", "ExploreItem", "AttackSource", "DisableSimpleActor" };

    /// <summary>原文 :340-370 的 33 个 Magic 字段（NeedL1..15 / L1Train..15Train / 3 具名）。</summary>
    public static List<string> Magic()
    {
        var list = new List<string>();
        for (int i = 1; i <= 15; i++)
        {
            list.Add("NeedL" + i);
            list.Add("L" + i + "Train");
        }
        list.Add("MaxTrainLv");
        list.Add("CanUpgrade");
        list.Add("MaxUpgradeLv");
        return list;
    }

    /// <summary>"完全就绪"的替身：三张表齐、全部字段齐。</summary>
    public static FakeHeroDB CompleteFake()
    {
        var fake = new FakeHeroDB();
        fake.SeedTable("StdItems", Union(StdItemsCheckOrder(), StdItemsCreateOrder()));
        fake.SeedTable("Monster", Monster);
        fake.SeedTable("Magic", Magic());
        return fake;
    }

    private static List<string> Union(List<string> a, List<string> b)
    {
        var list = new List<string>(a);
        foreach (string s in b)
            if (!list.Contains(s)) list.Add(s);
        return list;
    }
}

/// <summary>
/// <see cref="TFrmHeroDB.SelectDirectory"/> 的注入式目录选择替身（绝不弹真实对话框）。
/// </summary>
public static class FolderPicker
{
    private static Func<string, string?>? _saved;

    /// <summary>装上替身：返回一个固定目录（等价 ShBrowseForFolder 选中该目录）。</summary>
    public static void Returns(string path)
    {
        _saved = TFrmHeroDB.FolderPickerProvider;
        TFrmHeroDB.FolderPickerProvider = _ => path;
    }

    /// <summary>装上替身：模拟用户取消（等价 ShBrowseForFolder 返回 nil）。</summary>
    public static void Cancels()
    {
        _saved = TFrmHeroDB.FolderPickerProvider;
        TFrmHeroDB.FolderPickerProvider = _ => null;
    }

    /// <summary>装上替身：捕获预选目录并返回指定结果。</summary>
    public static void Capture(out Func<string> initialDirectory, string? result)
    {
        _saved = TFrmHeroDB.FolderPickerProvider;
        string captured = "";
        TFrmHeroDB.FolderPickerProvider = d => { captured = d; return result; };
        initialDirectory = () => captured;
    }

    /// <summary>还原上一状态。</summary>
    public static void Restore()
    {
        TFrmHeroDB.FolderPickerProvider = _saved;
        _saved = null;
    }
}

/// <summary>WinForms 控件必须在 STA 线程上创建/操作。</summary>
public static class FormSta
{
    public static void Run(Action action)
    {
        Exception? caught = null;
        var t = new System.Threading.Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        t.SetApartmentState(System.Threading.ApartmentState.STA);
        t.Start();
        t.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }
}
