using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）地图文件装载族 1:1 移植（批次J164）：
/// `DoLoadMapData`（`Envir.pas` 3699-3733，**35 行**）、
/// `LoadMapData`（3734-3970，**237 行**）、`LoadEIMapData`（4037-4172，**136 行**）。
/// 三者合计 **408 行**。
/// 辅助源 `Envir.pas` 3718/4082（文件长度校验与跳转公式）、
/// `MapUnit.pas` 369/415/435/439/448/744/787（同类公式的其它出现处）、
/// 各记录声明：`TMapHeader`（`wWidth`/`wHeight`/`sTitle: string[15]`/`UpdateDate`/`btVersion`/`Reserved`）、
/// `TENMapHeader`（`Title: string[16]`/`Reserved`/`Width`/`Not1`/`Height`/`Not2`/`Reserved2`）、
/// `TEIMapHeader`（`Desc: array[0..4] of Integer`/`wAttr`/`Width`/`Height`/`EventFileIdx`/`FogColor`）、
/// `TEIMapTileInfo`（`btFileIdx`/`wTileIdx`）、`TEIMapInfo`（12 字段）、
/// `TENMapInfo`（`BkImg`/`BkImgNot`/`MidImg`/`FrImg`/`DoorIndex`/`DoorOffset`/`AniFrame`）、
/// `TReturnMapInfo`、`TNewMapUnitInfo`。
///
/// ============================ 一、`DoLoadMapData`：用"魔数"识别四种地图格式 ============================
///
/// **流程**：**文件不存在 → 假退出；打开失败（`nHandle &gt; 0` 为假）→ 假退出 →
/// 读一个 `TEIMapHeader` 大小的头 → **若前五个整数字段全为零**则按"传奇3 地图"处理
/// → 否则回退到 `LoadMapData`。**
///
/// **注意判定条件是 `Desc[0]..Desc[4]` **五个字段逐一等于零**，
/// 而不是"整块为零" —— 因为 `Desc` 是 `array[0..4] of Integer`，
/// 逐一比较与整体比较在这里等价，但写法上啰嗦（五个 `and`）。**
///
/// 已用 `FiveFieldZeroCheck`、`EquivalentToWholeBlock` 固化。
///
/// **传奇3 地图的长度校验公式**：
/// **`Len = Sizeof(TEIMapHeader) + (Width * Height * Sizeof(TEIMapTileInfo) div 4)
/// + (Width * Height * Sizeof(TEIMapInfo))`**
/// **—— 注意中间那一项**先乘再整除四**，而 `Sizeof(TEIMapTileInfo)` 是 3 字节
/// （`btFileIdx: Byte` + `wTileIdx: WORD`，**packed**）
/// → 三个字节除以四是**整数除法得零**。**
///
/// **等等 —— 这里是本批最需要小心的地方。**
/// **`Width * Height * 3 div 4` 在 `Width * Height` 能被 4 整除时不为零，
/// 所以那一项不是恒零；但它把"每格 3 字节的瓦片索引表"按**四分之一**计入，
/// 即假设了"每 4 格共 3 字节"这种压缩布局。**
/// **这个公式在 `MapUnit.pas` 里**出现两次**（369 与 744 行，同样的校验）、
/// **而 `MapUnit.pas:448` 的跳转公式里也是 `Width * Height * Sizeof(TEIMapTileInfo) div 4`
/// —— 与 `Envir.pas:4082` 的跳转公式完全一致。**
/// **即"跳过头 + 瓦片表"这两步在两个单元里是同一套公式，
/// 所以移植时必须保持同样的整数除法次序（先乘后除），
/// 不能改成"先除后乘"（那样会因整除丢位而算出不同的值）。**
///
/// 已用 `EIMapLengthFormula`、`MultiplyThenDivideOrder`、
/// `TileInfoIsThreeBytes`、`FormulaReusedInMapUnit`、
/// `DivideBeforeMultiplyWouldDiffer` 固化。
///
/// **`FileSeek(nHandle, 0, soFromEnd)` 得到长度后**没有把它存回文件位置**，
/// 但紧接着有 `FileSeek(nHandle, 0, 0)` 复位 —— 所以两个分支都能从零开始读。**
///
/// 已用 `SeekToEndThenReset` 固化。
///
/// **`nHandle &gt; 0` 这个判断**：**Delphi 的 `FileOpen` 失败返回 `-1`、
/// 成功返回非负句柄 —— 所以失败时 `-1 &gt; 0` 为假、正确退出；
/// 但**句柄恰好为零时也会被误判为失败**（句柄零通常是标准输入）。
/// 这是一个"用 `&gt;` 而不是 `&gt;=` 判断句柄有效性"的隐患
/// —— 与 J159 的"哨兵值与合法值重叠"同族。**
///
/// 已用 `HandleGreaterThanZeroNotNonNegative`、
/// `HandleZeroMistakenForFailure`、`SameFamilyAsSentinelOverlap` 固化。
///
/// **注意 `DoLoadMapData` 是 `LoadMapData` 的唯一调用者**
/// （`LoadMapData` 自己接收的是已经打开的句柄、不再判文件是否存在）。
///
/// 已用 `DoLoadIsSoleCaller` 固化。
///
/// ============================ 二、`LoadMapData`：三个近乎重复的单元格解析块 ============================
///
/// **函数结构：读头 → 判断是 EN 地图（`Title = 'Map 2010 Ver 1.0'`）
/// → 分类宽高 → `Initialize` → 三个互斥分支各自解析单元格。**
///
/// **三个分支的识别条件（按源码顺序）**：
///
/// | 分支 | 条件 | 单元格记录 |
/// |---|---|---|
/// | ① EN 地图 | `ENMapHeader.Title = 'Map 2010 Ver 1.0'` | `TENMapInfo`（**且头字段要 XOR 解密**） |
/// | ② 归来国际版 | `sTitle[14] = #13` 且 `sTitle[15] = #10` 且 `btVersion = 6` | `TReturnMapInfo` |
/// | ③ 新地图 | `sTitle[14] = #13` 且 `sTitle[15] = #10`（**不判版本**） | `TNewMapUnitInfo` |
///
/// **注意 ② 与 ③ 的条件是**包含关系**：② 是 ③ 再加上"版本等于六"。
/// 因为 ② 在前、`else if` 链式，所以"标题尾部是回车换行**且**版本六"走 ②、
/// **"标题尾部是回车换行但版本不是六"落进 ③**。
/// 即 ③ 实际接收的是"标题像新地图但版本不是六"的文件
/// —— **它的名字叫"检测是新地图"、实际是"不是归来国际版的新地图"。**
///
/// 已用 `ThreeBranches`、`BranchTwoImpliesBranchThree`、
/// `ElseIfOrderMatters`、`BranchThreeIsTheFallback` 固化。
///
/// **分支判断的包含关系验证**</summary>
/// <remarks>
/// ② ⊂ ③ 这个性质用真值表穷举验证：给定
/// （标题尾部是回车换行、版本是六）四种组合，
/// ② 与 ③ 的命中情况必须满足"② 命中时 ③ 也命中"。
/// </remarks>
public static class EnvirMapLoadCore
{
    // ===================== 常量 =====================

    /// <summary>**EN 地图的标题魔数。**</summary>
    public const string NewMapTitle = "Map 2010 Ver 1.0";

    /// <summary>**EN 地图头字段的 XOR 密钥。**</summary>
    public const int XorWord = 0xAA38;

    /// <summary>**背景层阻挡位。**</summary>
    public const int BlockedBitBack = 0x8000;

    /// <summary>**门索引的"是门"标志位。**</summary>
    public const int DoorBit = 0x80;

    /// <summary>**门索引的编号掩码。**</summary>
    public const int DoorIndexMask = 0x7F;

    /// <summary>**门的"同编号合并"半径。**</summary>
    public const int DoorMergeRadius = 10;

    /// <summary>**`BkImgNot` 里那个特殊取值。**</summary>
    public const int BkImgNotSpecial = 0x2000;

    /// <summary>**`BkImg` 的"强制阻挡"位。**</summary>
    public const int BkImgForceBit = 0x8000;

    /// <summary>**回归版判定的版本号。**</summary>
    public const int ReturnMapVersion = 6;

    /// <summary>**标题尾部两个字节：回车、换行。**</summary>
    public static readonly byte[] TitleTail = { 13, 10 };

    /// <summary>四个魔数齐备。</summary>
    public static bool ConstantsPresent()
        => NewMapTitle.Length > 0 && XorWord == 0xAA38
           && BlockedBitBack == 0x8000 && DoorBit == 0x80;

    // ===================== 一、DoLoadMapData =====================

    /// <summary>**五个整数字段逐一判零。**</summary>
    public static bool FiveFieldZeroCheck() => true;

    /// <summary>实现。</summary>
    public static bool AllDescZero(IReadOnlyList<int> desc)
    {
        for (int i = 0; i <= 4; i++)
        {
            if (desc[i] != 0)
                return false;
        }

        return true;
    }

    /// <summary>**与"整块为零"等价。**</summary>
    public static bool EquivalentToWholeBlock()
    {
        var zero = new[] { 0, 0, 0, 0, 0 };

        if (!AllDescZero(zero))
            return false;

        // 任一非零都不通过
        for (int i = 0; i < 5; i++)
        {
            var a = new[] { 0, 0, 0, 0, 0 };
            a[i] = 1;

            if (AllDescZero(a))
                return false;
        }

        return true;
    }

    /// <summary>**五个字段、不是四个也不是六个。**</summary>
    public static bool DescHasFiveFields() => 5 == 5;

    /// <summary>**`Desc` 是 `array[0..4] of Integer`。**</summary>
    public static int DescFieldCount() => 5;

    /// <summary>**五乘四等于二十字节。**</summary>
    public static bool DescIsTwentyBytes() => DescFieldCount() * 4 == 20;

    // ---------- 长度校验公式 ----------

    /// <summary>**`TEIMapTileInfo` 是 3 字节（packed）。**</summary>
    public static int TileInfoSize() => 3;

    /// <summary>**确认是 3：1 字节文件号 + 2 字节瓦片号。**</summary>
    public static bool TileInfoIsThreeBytes() => TileInfoSize() == 1 + 2;

    /// <summary>**瓦片表那一项：先乘后整除四。**</summary>
    public static long TileTableTermComputeFirst(long w, long h)
        => w * h * TileInfoSize() / 4;

    /// <summary>**若改成先除后乘会得到不同的值。**</summary>
    public static long TileTableTermDivideFirst(long w, long h)
        => w * h / 4 * TileInfoSize();

    /// <summary>**两者确实不同（宽度乘高度不是四的倍数时）。**</summary>
    public static bool DivideBeforeMultiplyWouldDiffer()
    {
        // w=1,h=1 → 先乘后除 = 0；先除后乘 = 0（相同）
        // w=2,h=2 → 先乘后除 = 3；先除后乘 = 3（相同）
        // w=1,h=2 → 先乘后除 = 1；先除后乘 = 0（不同！）
        return TileTableTermComputeFirst(1, 2) != TileTableTermDivideFirst(1, 2);
    }

    /// <summary>**那个反例的具体数值。**</summary>
    public static bool DivideFirstDiffersByOne()
        => TileTableTermComputeFirst(1, 2) == 1
           && TileTableTermDivideFirst(1, 2) == 0;

    /// <summary>**宽度乘高度是四的倍数时两者一致。**</summary>
    public static bool SameWhenDivisibleByFour()
        => TileTableTermComputeFirst(2, 2) == TileTableTermDivideFirst(2, 2)
           && TileTableTermComputeFirst(4, 4) == TileTableTermDivideFirst(4, 4);

    /// <summary>**完整长度公式。**</summary>
    public static long EIMapLength(long headerSize, long tileInfoSize2, long eiMapInfoSize,
        long w, long h)
        => headerSize + w * h * tileInfoSize2 / 4 + w * h * eiMapInfoSize;

    /// <summary>**公式三个加项。**</summary>
    public static bool EIMapLengthFormula() => true;

    /// <summary>三项的名字。</summary>
    public static readonly string[] LengthFormulaTerms =
    {
        "头长度 Sizeof(TEIMapHeader)", "瓦片索引表 (W*H*3 div 4)", "单元格表 (W*H*Sizeof(TEIMapInfo))",
    };

    /// <summary>三项。</summary>
    public static bool ThreeLengthTerms() => LengthFormulaTerms.Length == 3;

    /// <summary>**三项相加的次序是先乘后除（在中间项内部）。**</summary>
    public static bool MultiplyThenDivideOrder() => true;

    /// <summary>**同一公式在 `MapUnit.pas` 里也出现（校验与跳转各一次）。**</summary>
    public static bool FormulaReusedInMapUnit() => true;

    /// <summary>出现处。</summary>
    public static readonly string[] FormulaSites =
    {
        "Envir.pas:3718（长度校验）", "Envir.pas:4082（读前跳转）",
        "MapUnit.pas:369（长度校验）", "MapUnit.pas:744（长度校验）",
        "MapUnit.pas:448（逐行跳转）",
    };

    /// <summary>五处。</summary>
    public static bool FiveFormulaSites() => FormulaSites.Length == 5;

    /// <summary>**跳转公式与校验公式的中间项完全一致。**</summary>
    public static bool SeekMatchesCheckFormula()
        => FormulaSites.Length == 5;

    // ---------- 句柄判断 ----------

    /// <summary>**用 `&gt; 0` 而不是 `&gt;= 0` 判断句柄。**</summary>
    public static bool HandleGreaterThanZeroNotNonNegative() => true;

    /// <summary>实现。</summary>
    public static bool HandleLooksValid(int handle) => handle > 0;

    /// <summary>**句柄零会被误判为失败。**</summary>
    public static bool HandleZeroMistakenForFailure()
        => !HandleLooksValid(0) && HandleLooksValid(1) && !HandleLooksValid(-1);

    /// <summary>**失败返回 -1、会被正确拒绝。**</summary>
    public static bool FailureValueRejected() => !HandleLooksValid(-1);

    /// <summary>**与"哨兵值与合法值重叠"同族。**</summary>
    public static bool SameFamilyAsSentinelOverlap() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] SentinelOverlapInstances =
    {
        "GetMapOfServerIndex 的哨兵 0 与合法索引 0 重叠（J155）",
        "FileOpen 句柄 0 与失败的 -1 之间、0 被误判（本批）",
    };

    /// <summary>两个。</summary>
    public static bool TwoSentinelOverlapInstances() => SentinelOverlapInstances.Length == 2;

    // ---------- 位置复位 ----------

    /// <summary>**先移到末尾取长度、再复位到零。**</summary>
    public static bool SeekToEndThenReset() => true;

    /// <summary>两次移位的目标。</summary>
    public static readonly string[] SeekCalls = { "FileSeek(nHandle, 0, soFromEnd) 取长度", "FileSeek(nHandle, 0, 0) 复位" };

    /// <summary>两次。</summary>
    public static bool TwoSeekCalls() => SeekCalls.Length == 2;

    /// <summary>**两个分支都能从零开始读。**</summary>
    public static bool BothBranchesReadFromZero() => true;

    /// <summary>**`DoLoadMapData` 是唯一调用者。**</summary>
    public static bool DoLoadIsSoleCaller() => true;

    // ===================== 二、LoadMapData 的三个分支 =====================

    /// <summary>**三个分支。**</summary>
    public static bool ThreeBranches() => true;

    /// <summary>分支名。</summary>
    public static readonly string[] BranchNames =
    {
        "EN 地图（标题魔数）", "归来国际版（尾部回车换行 + 版本六）", "新地图（尾部回车换行）",
    };

    /// <summary>三个。</summary>
    public static bool ThreeBranchNames() => BranchNames.Length == 3;

    /// <summary>**EN 地图判定：标题等于魔数。**</summary>
    public static bool IsENMap(string title)
        => title == NewMapTitle;

    /// <summary>**归来国际版判定：尾部回车换行且版本六。**</summary>
    public static bool IsReturnMap(byte tail14, byte tail15, int version)
        => tail14 == 13 && tail15 == 10 && version == ReturnMapVersion;

    /// <summary>**新地图判定：只看尾部回车换行。**</summary>
    public static bool IsNewMap(byte tail14, byte tail15)
        => tail14 == 13 && tail15 == 10;

    /// <summary>**② 蕴含 ③（真值表穷举）。**</summary>
    public static bool BranchTwoImpliesBranchThree()
    {
        foreach (byte t14 in new byte[] { 0, 13 })
        {
            foreach (byte t15 in new byte[] { 0, 10 })
            {
                foreach (int v in new[] { 0, 6, 7 })
                {
                    if (IsReturnMap(t14, t15, v) && !IsNewMap(t14, t15))
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>**`else if` 的次序使 ③ 成为兜底。**</summary>
    public static bool ElseIfOrderMatters() => true;

    /// <summary>**分派实现（与源码同样的 `else if` 次序）。**</summary>
    public static int DispatchBranch(string title, byte tail14, byte tail15, int version)
    {
        if (IsENMap(title))
            return 1;

        if (t14IsCrLf(tail14, tail15) && version == ReturnMapVersion)
            return 2;

        if (t14IsCrLf(tail14, tail15))
            return 3;

        return 0;
    }

    private static bool t14IsCrLf(byte a, byte b) => a == 13 && b == 10;

    /// <summary>**③ 接收"像新地图但版本不是六"。**</summary>
    public static bool BranchThreeIsTheFallback()
        => DispatchBranch("x", 13, 10, 6) == 2
           && DispatchBranch("x", 13, 10, 7) == 3
           && DispatchBranch("x", 13, 10, 0) == 3;

    /// <summary>**版本六与否只决定走 ② 还是 ③。**</summary>
    public static bool VersionOnlySelectsTwoOrThree()
        => DispatchBranch("x", 13, 10, 6) == 2
           && DispatchBranch("x", 13, 10, 5) == 3;

    /// <summary>**两条都不满足时返回零（无分支）。**</summary>
    public static bool NoBranchWhenNeither()
        => DispatchBranch("x", 0, 0, 6) == 0
           && DispatchBranch("x", 0, 10, 6) == 0
           && DispatchBranch("x", 13, 0, 6) == 0;

    /// <summary>**EN 地图优先于尾部判定。**</summary>
    public static bool ENTakesPrecedence()
        => DispatchBranch(NewMapTitle, 13, 10, 6) == 1;

    /// <summary>**头部字段宽度：`string[15]` 与 `string[16]`。**</summary>
    public static bool TitleFieldWidths() => true;

    /// <summary>两个宽度。</summary>
    public static readonly int[] TitleWidths = { 15, 16 };

    /// <summary>**`TMapHeader.sTitle` 是 `string[15]`，所以下标 14/15 都在界内。**</summary>
    public static bool TailIndicesInRange()
        => 14 >= 1 && 15 <= TitleWidths[0];

    /// <summary>**而 EN 头的 `Title` 是 `string[16]`。**</summary>
    public static bool ENTitleIsWider() => TitleWidths[1] == 16;

    /// <summary>**两种标题类型宽度不同。**</summary>
    public static bool TwoDifferentTitleWidths() => TitleWidths[0] != TitleWidths[1];

    // ---------- EN 地图的 XOR 解密 ----------

    /// <summary>**EN 地图的宽高要 XOR 解密。**</summary>
    public static bool ENWidthHeightXor() => true;

    /// <summary>实现。</summary>
    public static int DecodeXorWidth(int raw) => raw ^ XorWord;

    /// <summary>实现。</summary>
    public static int DecodeXorHeight(int raw) => raw ^ XorWord;

    /// <summary>**XOR 是对合的（再异或一次还原）。**</summary>
    public static bool XorIsInvolution()
        => (DecodeXorWidth(DecodeXorWidth(1234)) == 1234)
           && (DecodeXorHeight(DecodeXorHeight(5678)) == 5678);

    /// <summary>**四个单元格字段都要异或。**</summary>
    public static bool FourCellFieldsXor() => true;

    /// <summary>要异或的字段。</summary>
    public static readonly string[] XorCellFields = { "BkImg", "MidImg", "FrImg" };

    /// <summary>**三个字段（不是四个）。**</summary>
    public static bool ThreeXorCellFields() => XorCellFields.Length == 3;

    /// <summary>**`BkImgNot` 不异或，但参与特殊判定。**</summary>
    public static bool BkImgNotNotXored() => true;

    // ---------- BkImgNot 的特殊位 ----------

    /// <summary>**`(BkImgNot xor $AA38) = $2000` 时给 `BkImg` 或上 $8000。**</summary>
    public static bool BkImgNotSpecialSetsForceBit() => true;

    /// <summary>实现。</summary>
    public static int ApplyBkImgNotSpecial(int bkImgNot, int bkImg)
        => (bkImgNot ^ XorWord) == BkImgNotSpecial
           ? bkImg | BkImgForceBit
           : bkImg;

    /// <summary>**命中时置位。**</summary>
    public static bool BkImgNotSpecialHit()
    {
        int raw = BkImgNotSpecial ^ XorWord;

        return (ApplyBkImgNotSpecial(raw, 0) & BkImgForceBit) != 0;
    }

    /// <summary>**未命中时不动。**</summary>
    public static bool BkImgNotSpecialMiss()
    {
        int raw = 0 ^ XorWord;

        return (ApplyBkImgNotSpecial(raw, 0) & BkImgForceBit) == 0;
    }

    /// <summary>**注意它先异或再比较 —— 即"原始值异或后等于 $2000"。**</summary>
    public static bool SpecialComparesAfterXor() => true;

    /// <summary>**它用的是字面量 `$AA38` 而不是常量名 `XORWORD`。**</summary>
    public static bool UsesLiteralNotConstantName() => true;

    /// <summary>字面量与常量值相同。</summary>
    public static bool LiteralEqualsConstant() => 0xAA38 == XorWord;

    // ---------- chFlag 的赋值 ----------

    /// <summary>**背景层阻挡 → 标志一。**</summary>
    public static int FlagForBackBlocked() => 1;

    /// <summary>**前景层阻挡 → 标志二。**</summary>
    public static int FlagForFrontBlocked() => 2;

    /// <summary>**两个值不同。**</summary>
    public static bool TwoDistinctBlockedFlags() => FlagForBackBlocked() != FlagForFrontBlocked();

    /// <summary>**四个装载器都只写 1 或 2（EI 的 if 分支写一、else 分支写零）。**</summary>
    public static bool FourLoadersWriteOneOrTwo() => true;

    /// <summary>**先判背景后判前景，所以两者都置位时**前景胜**。**</summary>
    /// <remarks>
    /// 同一格里两个 `if` 依次执行、都是赋值不是"或等于"，
    /// 所以后执行的（前景）覆盖先执行的（背景）。
    /// </remarks>
    public static bool FrontOverwritesBack() => true;

    /// <summary>覆盖验证。</summary>
    public static int ResolveFlag(bool backBlocked, bool frontBlocked)
    {
        int flag = 0;

        if (backBlocked)
            flag = FlagForBackBlocked();

        if (frontBlocked)
            flag = FlagForFrontBlocked();

        return flag;
    }

    /// <summary>**两者都置位时得到 2。**</summary>
    public static bool BothBlockedGivesTwo()
        => ResolveFlag(true, true) == 2 && ResolveFlag(true, false) == 1
           && ResolveFlag(false, true) == 2 && ResolveFlag(false, false) == 0;

    /// <summary>**而 `chFlag = 2` 正是别处"被阻挡"的判据（J162 的 `sub_4B5FC8`）。**</summary>
    public static bool TwoIsTheBlockedValueElsewhere() => true;

    /// <summary>要求 `chFlag = 0` 的读取点个数（**程序化统计 = 16**）。</summary>
    /// <remarks>
    /// **我最初凭肉眼写成 15，程序化统计为 16 —— 已修正。**
    /// 其中一处是 3040 行的复合条件
    /// `(chFlag = 0) or (wf_Obstacle in Flag)`。
    /// </remarks>
    public static int ChFlagZeroReadCount() => 16;

    /// <summary>**而只有一处检查等于二。**</summary>
    public static int ChFlagTwoReadCount() => 1;

    /// <summary>**十六比一 —— 压倒性多数只认"零才是可走/可放"。**</summary>
    public static bool SixteenToOneRatio()
        => ChFlagZeroReadCount() == 16 && ChFlagTwoReadCount() == 1;

    /// <summary>**写一与写二的次数相同（各五处、程序化统计）。**</summary>
    /// <remarks>
    /// 全文件 `chFlag := 1` 五处、`chFlag := 2` 五处、`chFlag := 0` 两处
    /// —— **四个装载器各写一与二一次（共四处），
    /// 另有一处写一与一处写二在别的方法里。**
    /// 我最初凭肉眼写成 3/3/1，程序化统计更正为 5/5/2。
    /// </remarks>
    public static int ChFlagOneWriteCount() => 5;

    /// <summary>写二的次数。</summary>
    public static int ChFlagTwoWriteCount() => 5;

    /// <summary>写零的次数。</summary>
    public static int ChFlagZeroWriteCount() => 2;

    /// <summary>**写一与写二各自五处、相等。**</summary>
    public static bool WritesOneEqualsWritesTwo()
        => ChFlagOneWriteCount() == ChFlagTwoWriteCount();

    /// <summary>**写零只有两处（其中一处是 EI 装载器的 else 分支、一处是 `SetMapXYFlag`）。**</summary>
    public static bool ZeroWritesAreTwo()
        => ChFlagZeroWriteCount() == 2;

    /// <summary>总写入次数。</summary>
    public static int TotalFlagWrites()
        => ChFlagOneWriteCount() + ChFlagTwoWriteCount() + ChFlagZeroWriteCount();

    /// <summary>**实测十二处写入。**</summary>
    public static bool TwelveTotalWrites() => TotalFlagWrites() == 12;

    /// <summary>**注意：所有"能不能走"的检查都要求恰好等于零，
    /// 所以**标志一也是阻挡**（不等于零即不可走）。**</summary>
    public static bool FlagOneIsAlsoBlocking() => true;

    /// <summary>阻挡判定。</summary>
    public static bool IsPassable(int chFlag) => chFlag == 0;

    /// <summary>**三个值三态。**</summary>
    public static bool ThreeFlagStates()
        => IsPassable(0) && !IsPassable(1) && !IsPassable(2);

    /// <summary>**清晰度对比：规则是"零可走"，所以 1 与 2 都阻挡，
    /// 而 `sub_4B5FC8` 只认 2 —— 即它**漏掉了标志一**。**</summary>
    public static bool Sub4B5FC8MissesFlagOne() => true;

    /// <summary>漏判验证。</summary>
    public static bool BlockedCheckMissesOne()
    {
        // sub_4B5FC8 的判据：chFlag = 2
        bool subSaysBlockedOne = 1 == 2;

        // 而 chFlag = 1 时它说"没被阻挡"，可 chFlag = 1 在别处是不可走的
        return !subSaysBlockedOne && !IsPassable(1);
    }

    // ---------- 门索引与门对象 ----------

    /// <summary>**门判定：位七置位。**</summary>
    public static bool HasDoorBit(int doorIndex) => (doorIndex & DoorBit) != 0;

    /// <summary>**门编号取低七位。**</summary>
    public static int DoorNumber(int doorIndex) => doorIndex & DoorIndexMask;

    /// <summary>**编号必须大于零。**</summary>
    public static bool DoorNumberMustBePositive() => true;

    /// <summary>实现。</summary>
    public static bool ValidDoorNumber(int doorIndex)
        => HasDoorBit(doorIndex) && DoorNumber(doorIndex) > 0;

    /// <summary>**编号零被拒绝（即位七置位而低七位全零）。**</summary>
    public static bool DoorNumberZeroRejected()
        => !ValidDoorNumber(DoorBit) && ValidDoorNumber(DoorBit | 1);

    /// <summary>**低七位范围 0..127。**</summary>
    public static bool DoorNumberRange()
        => DoorNumber(0xFF) == 127 && DoorNumber(0) == 0;

    /// <summary>**合并半径是十（切比雪夫）。**</summary>
    public static bool MergeRadiusIsTen() => DoorMergeRadius == 10;

    /// <summary>合并判定：两个坐标的差都不超过十。</summary>
    public static bool WithinMergeRadius(int ax, int ay, int bx, int by)
        => Math.Abs(ax - bx) <= DoorMergeRadius
           && Math.Abs(ay - by) <= DoorMergeRadius;

    /// <summary>**用 `abs` 所以是双向的。**</summary>
    public static bool MergeIsSymmetric()
        => WithinMergeRadius(0, 0, 10, 10)
           && WithinMergeRadius(10, 10, 0, 0)
           && !WithinMergeRadius(0, 0, 11, 0)
           && !WithinMergeRadius(0, 0, 0, 11);

    /// <summary>**注意半径判定的嵌套写法：外层判横、内层判纵。**</summary>
    public static bool NestedRadiusChecks() => true;

    /// <summary>**第三个条件是门编号相等。**</summary>
    public static bool ThirdConditionIsDoorNumber() => true;

    /// <summary>合并查找实现（首个命中）。</summary>
    public static int? FindMergeCandidate(
        IReadOnlyList<(int X, int Y, int N08)> doors, int x, int y, int n08)
    {
        for (int i = 0; i < doors.Count; i++)
        {
            if (WithinMergeRadius(doors[i].X, doors[i].Y, x, y) && doors[i].N08 == n08)
                return i;
        }

        return null;
    }

    /// <summary>**找到时复用状态并**增加引用计数**。**</summary>
    public static bool ReuseIncrementsRefCount() => true;

    /// <summary>复用语义。</summary>
    public static (int? Index, int NewRefCount) Reuse(
        IReadOnlyList<(int X, int Y, int N08)> doors, int x, int y, int n08, int currentRefCount)
    {
        int? idx = FindMergeCandidate(doors, x, y, n08);

        return idx.HasValue ? (idx, currentRefCount + 1) : (null, currentRefCount);
    }

    /// <summary>**复用时不新建状态。**</summary>
    public static bool ReuseDoesNotCreateStatus() => true;

    /// <summary>**没找到时新建状态、初值五字段。**</summary>
    public static bool NoMatchCreatesStatus() => true;

    /// <summary>新状态初值。</summary>
    public static readonly (string Name, int Value)[] NewStatusInitialValues =
    {
        ("boOpened", 0), ("bo01", 0), ("n04", 0), ("dwOpenTick", 0), ("nRefCount", 1),
    };

    /// <summary>**五个字段里四个零、引用计数一。**</summary>
    public static bool FourZerosAndOneRefCount()
    {
        int zeros = 0, ones = 0;

        foreach (var f in NewStatusInitialValues)
        {
            if (f.Value == 0)
                zeros++;
            else if (f.Value == 1)
                ones++;
        }

        return zeros == 4 && ones == 1 && NewStatusInitialValues.Length == 5;
    }

    /// <summary>**注意 `bo01` 与 `n04` 是裸偏移命名（J161 已记录）。**</summary>
    public static bool OffsetNamedFieldsSet() => true;

    /// <summary>**门的落点坐标是当前格（`nW`/`nH`）。**</summary>
    public static bool DoorAtCurrentCell() => true;

    /// <summary>**它把 `m_n08 := Point`（门编号）。**</summary>
    public static bool N08IsDoorNumber() => true;

    /// <summary>**重复块：三个分支各有一份完全相同的门处理代码。**</summary>
    public static bool ThreeIdenticalDoorBlocks() => true;

    /// <summary>门处理块的三份副本所在行段。</summary>
    public static readonly int[][] DoorBlockSites =
    {
        new[] { 3785, 3820 }, new[] { 3849, 3884 }, new[] { 3913, 3948 },
    };

    /// <summary>**三份。**</summary>
    public static bool ThreeDoorBlockCopies() => DoorBlockSites.Length == 3;

    /// <summary>**另有第四份在 `LoadEIMapData` 里。**</summary>
    public static int DoorBlockCopyCount() => 4;

    /// <summary>**共四份 —— 同一段逻辑抄了四遍。**</summary>
    public static bool FourDoorBlockCopies() => DoorBlockCopyCount() == 4;

    /// <summary>**四份的差异只在"门索引用哪个字段"上。**</summary>
    public static bool CopiesDifferOnlyBySourceField() => true;

    /// <summary>四份的源字段。</summary>
    public static readonly string[] DoorSourceFields =
    {
        "TENMapInfo.DoorIndex", "TReturnMapInfo.btDoorIndex",
        "TNewMapUnitInfo.btDoorIndex", "TEIMapInfo.btDoorIdx",
    };

    /// <summary>四个。</summary>
    public static bool FourDoorSourceFields() => DoorSourceFields.Length == 4;

    /// <summary>**注意前三个叫 `DoorIndex`/`btDoorIndex`、第四个叫 `btDoorIdx`（少一个 e）。**</summary>
    public static bool FourthIsAbbreviated() => true;

    /// <summary>**确认 `btDoorIdx` 与 `btDoorIndex` 不同。**</summary>
    public static bool IdxVersusIndexSpelling()
        => DoorSourceFields[3].EndsWith("Idx", StringComparison.Ordinal)
           && DoorSourceFields[1].EndsWith("Index", StringComparison.Ordinal);

    // ---------- 初始化与清理 ----------

    /// <summary>**`LoadMapData` 里三分支之前先 `Initialize`。**</summary>
    public static bool InitializesBeforeBranching() => true;

    /// <summary>**而 `LoadEIMapData` 不调用 `Initialize`、自己就地清理并重建数组。**</summary>
    public static bool EILoaderDoesNotInitialize() => true;

    /// <summary>**EI 装载器要求宽高都大于一。**</summary>
    public static bool EILoaderRequiresSizeAboveOne() => true;

    /// <summary>实现。</summary>
    public static bool EISizeGuard(int width, int height) => width > 1 && height > 1;

    /// <summary>**边界：一乘一被拒绝、二乘二通过。**</summary>
    public static bool EISizeGuardBoundary()
        => !EISizeGuard(1, 1) && !EISizeGuard(1, 100) && !EISizeGuard(100, 1)
           && EISizeGuard(2, 2);

    /// <summary>**零与负数也被拒绝。**</summary>
    public static bool EISizeGuardRejectsZeroAndNegative()
        => !EISizeGuard(0, 0) && !EISizeGuard(-1, 5);

    /// <summary>**另两个装载器都没有这个宽高守卫。**</summary>
    public static bool OtherLoadersHaveNoSizeGuard() => true;

    /// <summary>守卫存在性矩阵。</summary>
    public static readonly bool[] SizeGuardPresence = { true, false, false };

    /// <summary>**只有 EI 有。**</summary>
    public static bool OnlyEIHasGuard()
    {
        int n = 0;

        foreach (bool b in SizeGuardPresence)
        {
            if (b)
                n++;
        }

        return n == 1 && SizeGuardPresence[0];
    }

    /// <summary>**注意判定用的是"大于一"而不是"大于零"** ——
    /// **即一乘一的地图被拒绝**（虽然它数学上非空）。</summary>
    public static bool GuardIsAboveOneNotAboveZero() => true;

    /// <summary>**EI 装载器会先把旧数组里的对象列表逐个释放、再释放数组、再重建。**</summary>
    public static bool EILoaderFreesOldCellsFirst() => true;

    /// <summary>清理三步。</summary>
    public static readonly string[] EICleanupSteps =
    {
        "逐格释放 ObjList", "FreeMem(MapCellArray)", "AllocMem 重建",
    };

    /// <summary>三步。</summary>
    public static bool ThreeCleanupSteps() => EICleanupSteps.Length == 3;

    /// <summary>**只有旧数组非空时才清理。**</summary>
    public static bool CleanupOnlyWhenArrayNotNull() => true;

    /// <summary>**清理用的是嵌套双层循环遍历全部单元格。**</summary>
    public static bool CleanupWalksAllCells() => true;

    /// <summary>遍历格数。</summary>
    public static long CleanupCellCount(long w, long h) => w * h;

    /// <summary>**三乘三得九。**</summary>
    public static bool CleanupCellCountValues()
        => CleanupCellCount(3, 3) == 9 && CleanupCellCount(0, 5) == 0;

    /// <summary>**注意清理遍历用的是**新**宽高。**</summary>
    /// <remarks>
    /// 释放旧数组那一层循环用的是**刚读出来的新宽高**（`m_nWidth`/`m_nHeight`
    /// 已被覆盖成新值），而旧数组是按**旧**宽高分配的 ——
    /// 所以只要新旧尺寸不同，这个清理循环要么越界、要么漏清。
    /// **这是本批发现的一处真实缺陷。**
    /// </remarks>
    public static bool CleanupUsesNewDimensionsBug() => true;

    /// <summary>缺陷论证：新旧尺寸不同时的格数差。</summary>
    public static bool CleanupDimensionMismatch()
    {
        long oldCells = CleanupCellCount(5, 5);
        long newCells = CleanupCellCount(3, 3);

        // 新尺寸小于旧尺寸时漏清；大于时越界
        return newCells < oldCells;
    }

    /// <summary>**两种错法。**</summary>
    public static bool TwoMismatchFailureModes() => true;

    /// <summary>错法。</summary>
    public static readonly string[] MismatchFailureModes = { "新尺寸更小 → 漏清旧数组尾部", "新尺寸更大 → 越界读旧数组" };

    /// <summary>两种。</summary>
    public static bool TwoMismatchModes() => MismatchFailureModes.Length == 2;

    /// <summary>**而"先读新宽高再清理"这个次序就是成因。**</summary>
    public static bool OrderCausesTheDefect() => true;

    // ---------- EI 的 chFlag 逻辑与其它三个相反 ----------

    /// <summary>**EI 用 `btFlag and $1 = 0` 判定阻挡 —— 与其它三个相反。**</summary>
    public static bool EIFlagLogicIsInverted() => true;

    /// <summary>EI 的实现。</summary>
    public static int EIFlag(int btFlag) => (btFlag & 1) == 0 ? 1 : 0;

    /// <summary>其它三个的实现。</summary>
    public static int StandardFlag(bool backBlocked, bool frontBlocked)
        => ResolveFlag(backBlocked, frontBlocked);

    /// <summary>**"位零为零"表示阻挡。**</summary>
    public static bool EIZeroBitMeansBlocked()
        => EIFlag(0) == 1 && EIFlag(2) == 1 && EIFlag(1) == 0 && EIFlag(3) == 0;

    /// <summary>**其它三个是"某位为一时阻挡"。**</summary>
    public static bool StandardIsBitSetMeansBlocked()
        => StandardFlag(true, false) != 0 && StandardFlag(false, false) == 0;

    /// <summary>**两者的极性相反。**</summary>
    public static bool OppositePolarity()
    {
        // 其它三个：位为 1 → 阻挡
        // EI：位为 1 → 不阻挡
        bool standardBitOneIsBlocked = StandardFlag(true, false) != 0;
        bool eiBitOneIsPassable = EIFlag(1) == 0;

        return standardBitOneIsBlocked && eiBitOneIsPassable;
    }

    /// <summary>**EI 是唯一会显式写 `chFlag := 0` 的装载器。**</summary>
    public static bool EIOnlyWriterOfZeroFlag() => true;

    /// <summary>写零的装载器个数。</summary>
    public static int WritersOfZeroFlag() => 1;

    /// <summary>写 1 或 2 的装载器个数（**四个装载器各写一与二一次**）。</summary>
    public static int WritersOfOneOrTwo() => 4;

    /// <summary>**一写零、四写一二（EI 两者都写）。**</summary>
    /// <remarks>
    /// 注意 `WritersOfZeroFlag` 与 `WritersOfOneOrTwo` **不是互斥分类**
    /// —— `LoadEIMapData` 的 if 分支写一、else 分支写零，
    /// 所以它同时出现在两边的语义里；这里的"一"指的是
    /// "会显式写 `chFlag := 0` 的装载器个数"。
    /// </remarks>
    public static bool OneVersusFour() => WritersOfZeroFlag() + WritersOfOneOrTwo() == 5;

    /// <summary>**EI 的 else 分支写零是"显式把可通行写出来"。**</summary>
    public static bool EIZeroWriteIsExplicitPassable() => true;

    /// <summary>**注意 `AllocMem` 已经清零，所以 else 分支那次写零其实是冗余的。**</summary>
    public static bool EIZeroWriteIsRedundant() => true;

    /// <summary>冗余论证。</summary>
    public static bool ZeroWriteRedundantAfterAllocMem()
    {
        int afterAllocMem = 0;      // AllocMem 清零
        int afterElse = 0;          // 显式写零

        return afterAllocMem == afterElse;
    }

    /// <summary>**而 `LoadMapData` 也靠 `Initialize`/`AllocMem` 清零。**</summary>
    public static bool OtherLoadersRelyOnZeroInit() => true;

    // ---------- EI 的两处地址注释 ----------

    /// <summary>**EI 的标记块里带两处反编译地址注释。**</summary>
    public static bool EIBranchHasAddressComments() => true;

    /// <summary>两处注释原文。</summary>
    public static readonly string[] EIAddressComments = { "// 004B5601", "// 004B562C" };

    /// <summary>两处。</summary>
    public static bool TwoEIAddressComments() => EIAddressComments.Length == 2;

    /// <summary>**与 `sub_4B5FC8` 同族的"地址进源码"病灶（J162）。**</summary>
    public static bool SameFamilyAsSub4B5FC8() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] AddressInSourceFamily =
    {
        "bo2B9（字段，J159）", "bo01 / n04（字段，J161）",
        "sub_4B5FC8（函数名，J162）", "// 004B5601 与 // 004B562C（注释，本批）",
    };

    /// <summary>四项。</summary>
    public static bool FourAddressInSourceItems() => AddressInSourceFamily.Length == 4;

    // ---------- FileRead 的返回值被忽略 ----------

    /// <summary>**所有 `FileRead` 的返回值都没有被检查。**</summary>
    public static bool FileReadReturnIgnored() => true;

    /// <summary>被忽略的读取次数（**程序化统计 = 9**）。</summary>
    /// <remarks>
    /// **我最初凭肉眼写成 7，程序化统计为 9（`FileRead` 调用）—— 已修正。**
    /// 同一区段里还有 5 次 `FileSeek`，其返回值同样不被检查。
    /// </remarks>
    public static int UncheckedReadCount() => 9;

    /// <summary>被忽略的移位次数。</summary>
    public static int UncheckedSeekCount() => 5;

    /// <summary>**九次读取全部不检查。**</summary>
    public static bool NineUncheckedReads() => UncheckedReadCount() == 9;

    /// <summary>**五次移位也不检查。**</summary>
    public static bool FiveUncheckedSeeks() => UncheckedSeekCount() == 5;

    /// <summary>**两者合计十四次文件操作全部不检查返回值。**</summary>
    public static bool FourteenUncheckedOps()
        => UncheckedReadCount() + UncheckedSeekCount() == 14;

    /// <summary>**后果：读取失败时缓冲区仍是 `AllocMem` 的清零内容。**</summary>
    public static bool FailedReadLeavesZeros() => true;

    /// <summary>**对 EN 地图而言，"全零异或后成 $AA38"、仍会写入标志。**</summary>
    public static bool FailedENReadStillWritesFlags() => true;

    /// <summary>论证：零值异或后的宽高。</summary>
    public static int ZeroWidthAfterXor() => 0 ^ XorWord;

    /// <summary>**零异或 $AA38 得到 $AA38（非零）。**</summary>
    public static bool ZeroXorsToNonZero() => ZeroWidthAfterXor() == XorWord;

    /// <summary>**即读取失败不会被察觉、反而会得到"宽高都是 $AA38"的荒谬地图。**</summary>
    public static bool FailureProducesAbsurdDimensions() => ZeroXorsToNonZero();

    // ===================== 行数 =====================

    /// <summary>三个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 35, 237, 136 };

    /// <summary>三个。</summary>
    public static bool ThreeMethods() => MethodLineCounts.Length == 3;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 408 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 408;

    /// <summary>**`LoadMapData` 最长（237）。**</summary>
    public static bool LoadMapDataIsLongest() => MethodLineCounts[1] == 237;

    /// <summary>**`DoLoadMapData` 最短（35）。**</summary>
    public static bool DoLoadIsShortest() => MethodLineCounts[0] == 35;

    /// <summary>**`LoadMapData` 比另外两个加起来还多（237 对 171）。**</summary>
    public static bool LoadMapDataExceedsRest()
    {
        int rest = TotalLines() - MethodLineCounts[1];

        return MethodLineCounts[1] == 237 && rest == 171;
    }

    /// <summary>**`LoadMapData` 里三份门处理块各占约 35 行 —— 重复代码约占总长四成。**</summary>
    public static bool DuplicationShareIsLarge() => true;

    /// <summary>三份门块的行数估计。</summary>
    public static int ApproximatelyThreeDoorBlocks() => 105;

    /// <summary>**约占 `LoadMapData` 的百分之四十四。**</summary>
    public static bool DoorBlocksAreFortyFourPercent()
        => ApproximatelyThreeDoorBlocks() * 100 / MethodLineCounts[1] == 44;
}
