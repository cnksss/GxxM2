using GXX.M2Server.Engine;

namespace GXX.M2Server.GameCenter;

/// <summary>
/// GameCenter GBDEtoSqlite.pas 核心 1:1（批次J21）：BDE 数据库 → Sqlite 迁移字段映射与转换。
/// STD_FIELDCOUNT=33；GOM/GEE 字段名表；DateSetToSqlite 数据行转换接缝随 Sqlite 批次接入。
/// </summary>
public static class GBDEtoSqlite
{
    public const int STD_FIELDCOUNT = 33;
    public const int MAG_FIELDCOUNT = 1;
    public const int MON_FIELDCOUNT = 1;

    public static readonly string[] GomFieldNameStd =
    {
        "Expand1", "Expand2", "Expand3", "Expand4", "Expand5", "Value1", "Value2", "Value3", "Value4", "Value5",
        "Value6", "Value7", "Value8", "Value9", "Value10", "Value11", "Value12", "Value13", "Value14", "Value15",
        "Value16", "Value17", "Value18", "Value19", "Value20", "Value21", "Value22", "Value23", "Value24", "Value25",
        "Job", "Horse", "InsuranceCurrency", "InsuranceGold"
    };

    public static readonly string[] GeeFieldNameStd =
    {
        "Expand1", "Expand2", "Expand3", "Expand4", "Expand5", "element", "element1", "element2", "element3", "element4",
        "element5", "element6", "element7", "element8", "element9", "element10", "element11", "element12", "element13",
        "element14", "element15", "element16", "element17", "element18", "element19", "element20", "element21",
        "element22", "element23", "element24", "Light", "Horse", "InsuranceCurrency", "InsuranceGold"
    };

    public static readonly string[] GomFieldNameMag = { "CanUpgrade", "MaxUpgradeLv" };
    public static readonly string[] GeeFieldNameMag = { "CanUpgrade", "MaxUpgradeLv" };

    public static readonly string[] GomFieldNameMon = { "ExploreItem", "DisableSimpleActor" };
    public static readonly string[] GeeFieldNameMon = { "ExploreItem", "DisableSimpleActor" };

    public static readonly bool[] StdCheckFieldName = new bool[STD_FIELDCOUNT + 1];
    public static readonly bool[] MagCheckFieldName = new bool[MAG_FIELDCOUNT + 1];
    public static readonly bool[] MonCheckFieldName = new bool[MON_FIELDCOUNT + 1];

    /// <summary>GOM → GEE 字段名映射（同位置重命名）。</summary>
    public static string? MapFieldName(string engineKind, string fieldName)
    {
        return engineKind switch
        {
            "STD" => Array.IndexOf(GomFieldNameStd, fieldName) >= 0
                ? GeeFieldNameStd[Array.IndexOf(GomFieldNameStd, fieldName)]
                : null,
            "MAG" => Array.IndexOf(GomFieldNameMag, fieldName) >= 0
                ? GeeFieldNameMag[Array.IndexOf(GomFieldNameMag, fieldName)]
                : null,
            "MON" => Array.IndexOf(GomFieldNameMon, fieldName) >= 0
                ? GeeFieldNameMon[Array.IndexOf(GomFieldNameMon, fieldName)]
                : null,
            _ => null
        };
    }
}

/// <summary>GameCenter GHeroDBConfig.pas 核心（英雄库配置：BDE/Sqlite 双源 + 库文件路径）。</summary>
public static class GHeroDBConfig
{
    public static bool UseSqlite = true;
    public static string BdeDir = @".\FDB\";
    public static string SqliteFile = @".\GHeroDB.db";

    public static void Reset()
    {
        UseSqlite = true;
        BdeDir = @".\FDB\";
        SqliteFile = @".\GHeroDB.db";
    }
}
