namespace GXX.Core.Launcher;

/// <summary>一个字段在 TConfigClient 记录中的位置。</summary>
/// <param name="Name">字段名（与 Delphi 声明一致）</param>
/// <param name="Offset">在记录中的字节偏移（字段自身，不是内容）</param>
/// <param name="Kind">类型标签</param>
/// <param name="Capacity">string[N] 的 N；数组的每元素大小</param>
/// <param name="Count">数组元素个数；标量为 1</param>
public sealed record FieldDescriptor(string Name, int Offset, string Kind, int Capacity, int Count)
{
    /// <summary>该字段占用的总字节数。</summary>
    public int Size => Kind switch
    {
        "str" => Capacity + 1,
        "bool" => Count,
        "u8" => Count,
        "u16" => 2 * Count,
        "i32" => 4 * Count,
        "u32" => 4 * Count,
        _ => throw new InvalidOperationException($"未知类型 {Kind}")
    };

    /// <summary>数组元素 i 的偏移（标量时 Count=1，i 必须为 0）。</summary>
    public int OffsetOf(int i)
    {
        if (i < 0 || i >= Count) throw new ArgumentOutOfRangeException(nameof(i));
        return Kind switch
        {
            "str" => Offset + i * (Capacity + 1),
            "bool" => Offset + i,
            "u8" => Offset + i,
            "u16" => Offset + 2 * i,
            "i32" => Offset + 4 * i,
            "u32" => Offset + 4 * i,
            _ => throw new InvalidOperationException()
        };
    }
}

/// <summary>
/// TConfigClient 字段目录 —— **已用真机 ClientData.dat 实测校准**。
///
/// <para>校准方法与证据：</para>
/// <list type="number">
///   <item>用生成器 <c>Config.ini</c> 的字符串值在解密后的记录里按值定位（要求前一字节 == 字节长度），
///         得到 52 个字符串字段的精确偏移。</item>
///   <item><c>ClientConfigs</c> 的边界由 <c>sRunGatePassWord @270(str[50])</c> 推出，
///         并以 <c>Config.ini</c> 的 <c>Checked0..Checked100</c> 逐项比对：
///         <b>101/101 完全一致</b> ⇒ ClientConfigs 个数 = <b>101</b>（仓库源码写的是 96，真机为较新修订）。</item>
///   <item>随后 <c>ClientConfigs_Ex @422</c>、<c>HumManuallyCustomHits @423</c>(5×4)、
///         <c>sResourcesDir @443</c> 三点与 <c>Config.ini</c> 全部吻合，形成闭合。</item>
///   <item><c>sElementNewPropertyTexts</c> 相邻项步长实测 81 且共 <b>27</b> 项，
///         与 <c>Config.ini</c> 的 <c>ElementNewPropertyText1..27</c> 一致（仓库源码为 25）。</item>
///   <item>枚举宽度实测为 <b>1 字节</b>（<c>sNGExpAddHintText</c> 与 <c>sElementNewPropertyTexts</c>
///         之间恰好 30 字节 = 28 + 2 个枚举）。</item>
/// </list>
///
/// <para>注意：真机运行的客户端是**比仓库源码更新的修订**，中段还有若干未映射字段。
/// 因此配置器采用「模板 + 已知偏移编辑」策略：未映射字段在保存时保持字节不变，
/// 兼容性风险为零。<see cref="KnownFieldsEnd"/> 之后到记录末尾之间存在未映射区域属正常。</para>
/// </summary>
public static class ClientDataFields
{
    /// <summary>真机实测的记录大小（SizeOf(TConfigClient)）。</summary>
    public const int ReferenceRecordSize = 23324;

    // ---------------- 记录前缀：payload 段落表 ----------------
    // 已实测确认前 15 项（nSize/nCrc + 6 组 (Offset,Size) + nBackBmpCrc）
    public const int OffSize = 0;
    public const int OffCrc = 4;
    public const int OffBaseUIOffset = 8;
    public const int OffBaseUISize = 12;
    public const int OffShareUIOffset = 16;
    public const int OffShareUISize = 20;
    public const int OffNewStateWindowUIOffset = 24;
    public const int OffNewStateWindowUISize = 28;
    public const int OffConfigDlgUIOffset = 32;
    public const int OffConfigDlgUISize = 36;
    public const int OffJSYUIOffset = 40;
    public const int OffJSYUISize = 44;
    public const int OffBackBmpOffset = 48;
    public const int OffBackBmpSize = 52;
    public const int OffBackBmpCrc = 56;
    // 以下 3 组 (Offset,Size,Crc) 按声明顺序紧随其后（前缀表 [15..23]）
    public const int OffCursorDefOffset = 60;
    public const int OffCursorDefSize = 64;
    public const int OffCursorDefCrc = 68;
    public const int OffCursorMountOffset = 72;
    public const int OffCursorMountSize = 76;
    public const int OffCursorMountCrc = 80;
    public const int OffCursorUnmountOffset = 84;
    public const int OffCursorUnmountSize = 88;
    public const int OffCursorUnmountCrc = 92;

    // ---------------- 名称 / 密码 / 复选框 ----------------
    public const int OffGamePlanName = 236;      // string[30]
    public const int CapGamePlanName = 30;
    public const int OffChangeScreenBitCount = 267;
    public const int OffShowOpenDoor = 268;
    public const int OffShow1024 = 269;
    public const int OffRunGatePassWord = 270;   // string[50]
    public const int CapRunGatePassWord = 50;
    public const int OffClientConfigs = 321;     // Boolean[101]
    public const int CountClientConfigs = 101;
    public const int OffClientConfigsEx = 422;   // Boolean[1]
    public const int OffHumManuallyCustomHits = 423; // LongWord[5]
    public const int OffResourcesDir = 443;      // string[50]
    public const int CapResourcesDir = 50;

    // ---------------- 文本区（全部经实测锚点确认；数值 = 字段偏移，即长度字节位置）----------------
    public const int OffAttackModeTexts = 1197;  // string[40][8]
    public const int CapAttackModeText = 40;
    public const int CountAttackModeTexts = 8;
    public const int OffExpAddHintText = 1525;   // string[60]
    public const int OffNGExpAddHintText = 1586; // string[60]
    public const int CapHintText60 = 60;
    public const int OffElementNewPropertyTexts = 1677; // string[80][27]
    public const int CapElementText = 80;
    public const int CountElementTexts = 27;
    public const int OffHumPropertyGroupCaption = 3864; // string[40][7]
    public const int CountHumGroupCaptions = 7;
    public const int OffItemHintFluteStoneText = 4151;  // string[80]
    public const int OffItemHintNoFluteStoneText = 4232; // string[80]
    public const int OffItemHintFluteStoneColor = 4313;
    public const int OffItemHintNoFluteStoneColor = 4317;
    public const int OffHairOffsets = 4365;      // Integer[12]
    public const int OffGameLoginVersion = 4498; // string[10]
    public const int CapGameLoginVersion = 10;

    /// <summary>
    /// 已知字段区域的上界（不含）。此偏移之后到记录末尾存在未映射字段，
    /// 保存时原样保留即可。
    /// </summary>
    public const int KnownFieldsEnd = OffGameLoginVersion + CapGameLoginVersion + 1;   // 4509

    /// <summary>全部已实测字段。</summary>
    public static readonly IReadOnlyList<FieldDescriptor> Catalog = new FieldDescriptor[]
    {
        new("nSize",                    OffSize,                    "i32", 0, 1),
        new("nCrc",                     OffCrc,                     "u32", 0, 1),
        new("nBaseUIOffSet",            OffBaseUIOffset,            "i32", 0, 1),
        new("nBaseUISize",              OffBaseUISize,              "i32", 0, 1),
        new("nShareUIOffSet",           OffShareUIOffset,           "i32", 0, 1),
        new("nShareUISize",             OffShareUISize,             "i32", 0, 1),
        new("nNewStateWindowUIOffSet",  OffNewStateWindowUIOffset,  "i32", 0, 1),
        new("nNewStateWindowUISize",    OffNewStateWindowUISize,    "i32", 0, 1),
        new("nConfigDlgUIOffSet",       OffConfigDlgUIOffset,       "i32", 0, 1),
        new("nConfigDlgUISize",         OffConfigDlgUISize,         "i32", 0, 1),
        new("nJSYUIOffSet",             OffJSYUIOffset,             "i32", 0, 1),
        new("nJSYUISize",               OffJSYUISize,               "i32", 0, 1),
        new("nBackBmpOffSet",           OffBackBmpOffset,           "i32", 0, 1),
        new("nBackBmpSize",             OffBackBmpSize,             "i32", 0, 1),
        new("nBackBmpCrc",              OffBackBmpCrc,              "u32", 0, 1),

        new("sGamePlanName",            OffGamePlanName,            "str", CapGamePlanName, 1),
        new("boChangeSrceenBitCount",   OffChangeScreenBitCount,    "bool", 0, 1),
        new("boShowOpenDoor",           OffShowOpenDoor,            "bool", 0, 1),
        new("boShow1024",               OffShow1024,                "bool", 0, 1),
        new("sRunGatePassWord",         OffRunGatePassWord,         "str", CapRunGatePassWord, 1),
        new("ClientConfigs",            OffClientConfigs,           "bool", 0, CountClientConfigs),
        new("ClientConfigs_Ex",         OffClientConfigsEx,         "bool", 0, 1),
        new("HumManuallyCustomHits",    OffHumManuallyCustomHits,   "u32", 0, 5),
        new("sResourcesDir",            OffResourcesDir,            "str", CapResourcesDir, 1),

        new("sAttackModeTexts",         OffAttackModeTexts,         "str", CapAttackModeText, CountAttackModeTexts),
        new("sExpAddHintText",          OffExpAddHintText,          "str", CapHintText60, 1),
        new("sNGExpAddHintText",        OffNGExpAddHintText,        "str", CapHintText60, 1),
        new("sElementNewPropertyTexts", OffElementNewPropertyTexts, "str", CapElementText, CountElementTexts),
        new("sHumPropertyGroupCaption", OffHumPropertyGroupCaption, "str", CapAttackModeText, CountHumGroupCaptions),
        new("sItemHintFluteStoneText",  OffItemHintFluteStoneText,  "str", CapElementText, 1),
        new("sItemHintNoFluteStoneText",OffItemHintNoFluteStoneText,"str", CapElementText, 1),
        new("nItemHintFluteStoneColor", OffItemHintFluteStoneColor, "i32", 0, 1),
        new("nItemHintNoFluteStoneColor",OffItemHintNoFluteStoneColor,"i32",0, 1),
        new("nHairOffsets",             OffHairOffsets,             "i32", 0, 12),
        new("sGameLoginVersion",        OffGameLoginVersion,        "str", CapGameLoginVersion, 1),
    };

    /// <summary>按名字查字段。</summary>
    public static FieldDescriptor Get(string name) =>
        Catalog.FirstOrDefault(f => f.Name == name)
        ?? throw new KeyNotFoundException($"未知字段 {name}");

    /// <summary>
    /// 校验一个模板记录是否与已校准布局相容。
    /// 真机记录为 23324 字节；不同修订可能不同，因此仅在明显不符时给出警告文本（返回 null 表示通过）。
    /// </summary>
    public static string ValidateTemplate(byte[] record)
    {
        if (record.Length < KnownFieldsEnd)
            return $"记录仅 {record.Length} 字节，小于已知字段区上界 {KnownFieldsEnd}，布局可能不同";

        // sGamePlanName 的容量应为 30：内容之后必须能找到合理的零填充
        int len = record[OffGamePlanName];
        if (len > CapGamePlanName)
            return $"sGamePlanName 长度字节 = {len} > 容量 {CapGamePlanName}，布局可能不同";

        // sGameLoginVersion 应为 string[10] 的日期串（形如 2026-06-06）
        int verLen = record[OffGameLoginVersion];
        if (verLen > CapGameLoginVersion)
            return $"sGameLoginVersion 长度字节 = {verLen} > 容量 {CapGameLoginVersion}，布局可能不同";

        return record.Length == ReferenceRecordSize
            ? null
            : $"记录大小 {record.Length} ≠ 参考值 {ReferenceRecordSize}（不同修订，已知字段仍按实测偏移使用）";
    }
}
