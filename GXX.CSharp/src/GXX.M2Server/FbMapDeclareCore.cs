using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// `FB(...)` 副本地图声明解析、`g_FBMapManager` 注册与 `FindMonster`/`GetMonRace` 查表
/// 1:1 移植（批次J111）。主源：
/// - `LocalDB.pas` 1719-1760（`FB(...)` 参数解析与三条校验）
/// - `LocalDB.pas` 2225-2251（副本地图创建与 `g_FBMapManager.AddObject` 注册）
/// - `UsrEngn.pas` 11142-11177（`FindMonster` 二分查找）+ 714-745（`GetMonRace*` 三个包装）
///
/// J110 移植了副本地图**模板的消费**（`$` 前缀行的副本构造），本批次补上**这些模板从何而来**：
/// ① 地图配置行里的 `FB(...)` 声明（1723-1760）解析出副本数量/名称/进入限制等；
/// ② 地图初始化时按声明创建 `nFBCount` 个副本地图并注册进 `g_FBMapManager`（2225-2251）；
/// ③ `FindMonster`/`GetMonRace` 供 J110 的 3610 判定"怪物名是否存在"。
/// 至此"**地图声明 → 副本创建与注册 → 模板登记 → `$` 行副本构造 → 刷新**"整链贯通。
///
/// **`FB(...)` 的四个参数（1725-1738），原文注释 1720-1722 给出了完整语义**：
/// ```
/// FB(40,祖玛副本,0,1) 创建40个祖玛副本地图
/// 第3个参数：0:限制队友必须有三职业; 1:不限制职业，队友都可进; 2:只允许自己进入; 3:允许行会进入
/// 第4个参数：副本创建1分钟后(未收回时)，允许延时进入副本时间(分)
/// ```
/// **但注释的数值描述与枚举的名字并不一一对应**：`Envir.pas` 155 的原始声明是
/// `TFBEnterLimit = (fbel_JOB3, fbel_Group, fbel_OnlyCreater, fbel_Guild)`——
/// **1 号的名字是 `fbel_Group`（队伍）而非"不限职业"**，2 号才是 `fbel_OnlyCreater`。
/// 移植时**必须沿用原文枚举名**，否则 J112 里 `fbel_JOB3`/`fbel_Group` 的成组判断会失去依据。
/// 解析顺序：`nFBCount`（1727）→ `sFBName`（1728）→ `s44`（1729，第 3 参数）→ `s4C`（1730，第 4 参数）
/// → `s38`（剩余，第 5 参数 `NoHumClearFBMin`）。
/// **注意 1726-1730 的 `GetValidStr3_Ex` 都是"把返回值写回 `s38`"的链式写法**，
/// 即每步消费一个逗号分隔段；而 1738 直接对 `s38` 取整——故**第 5 参数就是调用后的剩余串**。
///
/// **`EnterLimit` 的越界回退（1731-1735）**：`IntTemp := StrToIntDef(s44, -1)`——
/// **缺省 -1**，再用 `[Low(TFBEnterLimit) .. High(TFBEnterLimit)]` 区间校验，
/// 越界则回退到 `fbel_OnlyCreater`。故"参数缺失"与"参数非法"都落到同一默认值，
/// **不是**按第 3 参数注释里的 "0" 默认。已用 `EnterLimitOutOfRangeFallsBackToOnlyCreater` 固化。
///
/// **`NoHumClearFBMin` 的默认值与下限（1738-1740）**：`StrToIntDef(s38, 10)`
/// **缺省 10**，且 `<= 0` 时**强制重置为 10**——即"显式写 0 或负数"也会变成 10，
/// 而不是 0。这与 J108 的 `dwZenTime`（缺省 -1 且负值通过校验）形成对照：
/// **同类字段的默认值策略在本项目里并不统一**，必须逐处按原文，不可类推。
///
/// **三条校验及其错误码（1741-1758），顺序不可重排**：
/// ① 副本名为空 → `Result := -12`，消息 `'<地图名> 副本名称不能为空.'`；
/// ② `nFBCount` 不在 `[2 .. 99]` → `Result := -13`，消息 `'<地图名> 副本数量为2~99.'`；
/// ③ 副本名已存在于 `g_FBMapManager` → `Result := -14`，消息 `'<地图名> 副本名称[<名>]已经存在.'`。
/// 三者都 `Exit`（中止整个地图加载），而非跳过该行。**注意 ① 的判定晚于解析**——
/// 名字是从 1728 解析出来的，空名要到 1741 才检查。
/// **`boFB := True` 只在三条校验全部通过后设置**（1759）。
///
/// **副本地图的命名规则（2235）**：`'$FB_' + sMainMapName + '_' + IntToStr(k)`，
/// `k` 从 **1** 到 `nFBCount`（2233 `for k := 1 to nFBCount`）——**从 1 开始而非 0**，
/// 故生成的第一个副本名以 `_1` 结尾。
/// 各副本地图字段（2239-2243）：`m_boFB := True`、`m_sFBName`、`m_FBEnterLimit`、
/// `m_dwFBEnterDelayMin := EnterDelayMin * 60000`（**分转毫秒**）、
/// `m_dwFBNoHumClearMin := NoHumClearFBMin * 1000`（**秒转毫秒**）——
/// **两个字段的单位不同（分/秒），换算系数因此不同（60000/1000）**，不可统一。
/// 创建失败（`Envir = nil`）时输出 `'副本地图创建失败，地图名已经存在：' + sMapName`（2247），
/// **但不中止**。
///
/// **`sMainMapName` 的补写（2228-2229）**：副本路径下若 `sMainMapName = ''` 则**取 `sMapName` 自身**。
/// **`FindMonster`（11142-11177）与 J109 的 `FindFirstSortMapMonGenListIndex` 结构几乎相同，
/// 但有一处关键差异**：`Index := L` 在 11170 **无条件赋值**（在 `while` 之外），
/// 故**未命中时 `Index` 也是有效下标（插入位置）而非 -1**——
/// 调用方若误把 `Index` 当"命中下标"，会在未命中时读到**相邻的另一个怪**。
/// `GetMonRace`（714-721）正是靠 `if FindMonster(...)` 的**布尔返回值**而非 `Index` 取值来防护这一点。
/// </summary>
public static class FbMapDeclareCore
{
    // ===================== FB(...) 参数解析（1723-1740） =====================

    /// <summary>1723：声明前缀。</summary>
    public const string FbDeclPrefix = "FB(";

    /// <summary>1722 注释：`nFBCount` 的合法区间下限。</summary>
    public const int FbCountMin = 2;

    /// <summary>1747：`nFBCount` 的合法区间上限。</summary>
    public const int FbCountMax = 99;

    /// <summary>1731：进入限制参数的解析缺省值。</summary>
    public const int EnterLimitDefaultRaw = -1;

    /// <summary>1738：`NoHumClearFBMin` 的解析缺省值（分钟）。</summary>
    public const int NoHumClearFbMinDefault = 10;

    /// <summary>1740：`NoHumClearFBMin` 的下限——`<= 0` 时重置。</summary>
    public const int NoHumClearFbMinFloor = 10;

    /// <summary>2233：副本序号**从 1 开始**。</summary>
    public const int FbIndexStart = 1;

    /// <summary>2235：副本名前缀。</summary>
    public const string FbMapNamePrefix = "$FB_";

    /// <summary>2242：`EnterDelayMin` 分→毫秒。</summary>
    public const int MinuteToMs = 60_000;

    /// <summary>2243：`NoHumClearFBMin` 秒→毫秒。</summary>
    public const int SecondToMs = 1_000;

    /// <summary>三条校验的错误码（1743/1749/1755）。</summary>
    public const int ErrEmptyFbName = -12;
    public const int ErrFbCountOutOfRange = -13;
    public const int ErrFbNameDuplicated = -14;

    /// <summary>
    /// 1722：进入限制枚举（`TFBEnterLimit`，Envir.pas 155 原始声明）。
    /// **原文次序为 `(fbel_JOB3, fbel_Group, fbel_OnlyCreater, fbel_Guild)`，即 0..3**——
    /// **注意 1 是 `Group` 而非"不限职业"，2 才是 `OnlyCreater`**。
    /// 1721 的注释按数值解释了 0/1/2/3 的**用途**，但 1 号的名字是 `fbel_Group`（队伍），
    /// 故"不限制职业，队友都可进"与"队伍"是同一项的两种描述。
    /// 名称必须与原文一致，否则 J112 的 `fbel_JOB3`/`fbel_Group` 组合判断会失去依据。
    /// </summary>
    public enum FbEnterLimit
    {
        /// <summary>0：`fbel_JOB3` —— 限制队友必须有三职业。</summary>
        Job3 = 0,

        /// <summary>1：`fbel_Group` —— 队伍可进（注释的"不限制职业，队友都可进"）。</summary>
        Group = 1,

        /// <summary>2：`fbel_OnlyCreater` —— 只允许自己进入。</summary>
        OnlyCreater = 2,

        /// <summary>3：`fbel_Guild` —— 允许行会进入。</summary>
        Guild = 3,
    }

    /// <summary>1735：越界回退值 `fbel_OnlyCreater`。</summary>
    public const FbEnterLimit FbEnterLimitFallback = FbEnterLimit.OnlyCreater;

    /// <summary>1722：`Low(TFBEnterLimit)`。</summary>
    public const int FbEnterLimitLow = (int)FbEnterLimit.Job3;

    /// <summary>1722：`High(TFBEnterLimit)`。</summary>
    public const int FbEnterLimitHigh = (int)FbEnterLimit.Guild;

    /// <summary>1723：`CompareLStr(s34, 'FB(', 3)` —— 大小写不敏感前缀。</summary>
    public static bool IsFbDecl(string text)
        => MonGenParseCore.CompareLStr(FbDeclPrefix, text, FbDeclPrefix.Length);

    /// <summary>
    /// 1725：`ArrestStringEx(s34, '(', ')', s38)` 的 1:1 语义
    /// （`Common\HUtil32.pas` 1713-1759）。
    /// **注意它不是"只取括号内"**：
    /// - `ArrestStr`（对应 1725 的 `s38`）得到**括号内**的内容（1751-1752）；
    /// - **返回值**得到 **`)` 之后的剩余部分**（1753），而非整串、也非括号内。
    /// 故 1726 的 `s38 := GetValidStr3_Ex(s38, s44, ',')` 消费的是**括号内的第一个段**。
    /// 括号不配对时（无 `(` 或无 `)`）`ArrestStr` 保持 **`''`**（1720 无条件置空），
    /// 而返回值保持 **原串**（1719 `Result := Source`）。
    /// **本方法返回一个元组以同时表达两者**——只返回"括号内"会丢掉 1753 的剩余串语义。
    /// </summary>
    public static (string Arrested, string Remainder) ArrestStringEx(string source)
    {
        // 1719-1720：无条件初始化
        string result = source;
        string arrested = "";

        if (source.Length == 0)
            return ("", "");   // 1722-1726

        int open = source.IndexOf('(');
        if (open < 0)
            return (arrested, result);   // 未找到 '(' → P2 保持 nil

        int close = source.IndexOf(')', open + 1);
        if (close < 0)
            return (arrested, result);   // 未找到 ')' → 内层循环未 Break

        // 1751-1752：括号内
        arrested = source.Substring(open + 1, close - open - 1);

        // 1753：')' 之后的剩余部分
        result = source.Substring(close + 1);

        return (arrested, result);
    }

    /// <summary>1725 的简化入口：只要括号内内容（`s38` 的初值）。</summary>
    public static string? ExtractParenContent(string text)
    {
        var (arrested, _) = ArrestStringEx(text);
        return arrested;
    }

    /// <summary>1731-1735：进入限制的区间校验与回退。</summary>
    public static FbEnterLimit SelectEnterLimit(int rawValue)
    {
        if (rawValue >= FbEnterLimitLow && rawValue <= FbEnterLimitHigh)
            return (FbEnterLimit)rawValue;

        return FbEnterLimitFallback;
    }

    /// <summary>
    /// 1726-1738：`FB(...)` 的参数解析结果。
    /// </summary>
    public sealed class FbDecl
    {
        /// <summary>1727：`nFBCount`。缺省 **0**（`StrToIntDef(s44, 0)`）。</summary>
        public int FbCount;

        /// <summary>1728：`sFBName`（副本名）。</summary>
        public string FbName = "";

        /// <summary>1733/1735：进入限制。</summary>
        public FbEnterLimit EnterLimit;

        /// <summary>1736：`EnterDelayMin`，缺省 **0**。</summary>
        public int EnterDelayMin;

        /// <summary>1738-1740：`NoHumClearFBMin`，缺省 10 且 `<= 0` 重置为 10。</summary>
        public int NoHumClearFbMin;

        /// <summary>1729：第 3 参数的原始文本（校验前）。</summary>
        public string RawEnterLimit = "";

        /// <summary>1730：第 4 参数的原始文本。</summary>
        public string RawEnterDelay = "";
    }

    /// <summary>
    /// 1725-1740：解析 `FB(...)` 的括号参数。
    /// 五个逗号分隔段：数量, 名称, 进入限制, 延时进入(分), 无人清除(分)。
    /// **每段都用 `StrToIntDef` 的各自缺省值**：数量 0、进入限制 -1、延时 0、无人清除 10。
    /// </summary>
    public static FbDecl ParseFbDecl(string text)
    {
        var decl = new FbDecl();

        // 1725：ArrestStringEx 取括号内内容；**返回值是 ')' 之后的剩余串**。
        // 若括号不配对，s38 保持 ''（1720 无条件置空）。
        var (inner, _afterParen) = ArrestStringEx(text);

        // 1726-1730：链式消费逗号分隔段（从括号内容开始）
        string rest = inner;

        string seg1 = TakeSegment(ref rest);
        decl.FbCount = MonGenParseCore.StrToIntDef(seg1, 0);          // 1727 缺省 0

        string seg2 = TakeSegment(ref rest);
        decl.FbName = seg2;                                          // 1728

        string seg3 = TakeSegment(ref rest);
        decl.RawEnterLimit = seg3;
        decl.EnterLimit = SelectEnterLimit(MonGenParseCore.StrToIntDef(seg3, EnterLimitDefaultRaw)); // 1731-1735

        string seg4 = TakeSegment(ref rest);
        decl.RawEnterDelay = seg4;
        decl.EnterDelayMin = MonGenParseCore.StrToIntDef(seg4, 0);    // 1736 缺省 0

        // 1738：直接对"剩余串"取整（第 5 参数）
        decl.NoHumClearFbMin = MonGenParseCore.StrToIntDef(rest, NoHumClearFbMinDefault);

        // 1739-1740：<= 0 强制重置
        if (decl.NoHumClearFbMin <= 0)
            decl.NoHumClearFbMin = NoHumClearFbMinFloor;

        return decl;
    }

    /// <summary>取下一个逗号分隔段；无逗号时取整串并把 `rest` 置空。</summary>
    private static string TakeSegment(ref string rest)
    {
        int idx = rest.IndexOf(',');
        if (idx < 0)
        {
            string all = rest;
            rest = "";
            return all;
        }

        string seg = rest.Substring(0, idx);
        rest = rest.Substring(idx + 1);
        return seg;
    }

    // ===================== 三条校验（1741-1758） =====================

    /// <summary>1741-1758：校验结果。</summary>
    public enum FbValidateResult
    {
        /// <summary>三条全过。</summary>
        Ok,

        /// <summary>1743：副本名为空。</summary>
        EmptyFbName,

        /// <summary>1749：副本数量不在 [2..99]。</summary>
        CountOutOfRange,

        /// <summary>1755：副本名已存在。</summary>
        NameDuplicated,
    }

    /// <summary>
    /// 1741-1758：**按原文顺序**依次校验，返回首个失败项。
    /// 顺序不可重排：空名 → 数量 → 重名。
    /// </summary>
    public static FbValidateResult ValidateFbDecl(FbDecl decl, bool nameAlreadyExists)
    {
        if (decl.FbName == "")
            return FbValidateResult.EmptyFbName;

        if (decl.FbCount < FbCountMin || decl.FbCount > FbCountMax)
            return FbValidateResult.CountOutOfRange;

        if (nameAlreadyExists)
            return FbValidateResult.NameDuplicated;

        return FbValidateResult.Ok;
    }

    /// <summary>1743/1749/1755：校验结果 → 返回码。</summary>
    public static int ValidateResultToCode(FbValidateResult r) => r switch
    {
        FbValidateResult.Ok => 0,
        FbValidateResult.EmptyFbName => ErrEmptyFbName,
        FbValidateResult.CountOutOfRange => ErrFbCountOutOfRange,
        FbValidateResult.NameDuplicated => ErrFbNameDuplicated,
        _ => 0,
    };

    /// <summary>1744/1750/1756：校验失败消息。`Ok` 返回空串。</summary>
    public static string ValidateMessage(FbValidateResult r, string mapName, string fbName) => r switch
    {
        FbValidateResult.Ok => "",
        FbValidateResult.EmptyFbName => mapName + " 副本名称不能为空.",
        FbValidateResult.CountOutOfRange => mapName + " 副本数量为2~99.",
        FbValidateResult.NameDuplicated => mapName + " 副本名称[" + fbName + "]已经存在.",
        _ => "",
    };

    /// <summary>1747：`nFBCount in [2 .. 99]` —— 闭区间。</summary>
    public static bool IsFbCountValid(int count) => count >= FbCountMin && count <= FbCountMax;

    /// <summary>1759：三条校验全过后才置 `boFB := True`。</summary>
    public static bool ShouldSetBoFB(FbValidateResult r) => r == FbValidateResult.Ok;

    // ===================== 副本地图创建与注册（2225-2251） =====================

    /// <summary>2235：生成第 k 个副本地图名（k 从 1 开始）。</summary>
    public static string MakeFbMapName(string mainMapName, int k)
        => FbMapNamePrefix + mainMapName + "_" + k.ToString();

    /// <summary>
    /// 2233-2235：为 `nFBCount` 个副本生成地图名，**序号从 1 到 nFBCount**。
    /// </summary>
    public static List<string> MakeAllFbMapNames(string mainMapName, int fbCount)
    {
        var names = new List<string>(fbCount);

        for (int k = FbIndexStart; k <= fbCount; k++)
            names.Add(MakeFbMapName(mainMapName, k));

        return names;
    }

    /// <summary>2228-2229：副本路径下 `sMainMapName` 为空则取 `sMapName`。</summary>
    public static string ResolveMainMapName(string mainMapName, string mapName)
        => mainMapName == "" ? mapName : mainMapName;

    /// <summary>2242：`EnterDelayMin * 60000`。</summary>
    public static int EnterDelayMinToMs(int minutes) => minutes * MinuteToMs;

    /// <summary>2243：`NoHumClearFBMin * 1000`。</summary>
    public static int NoHumClearMinToMs(int minutes) => minutes * SecondToMs;

    /// <summary>2247：创建失败消息。</summary>
    public static string FbCreateFailMessage(string mapName)
        => "副本地图创建失败，地图名已经存在：" + mapName;

    /// <summary>2237：`Envir <> nil` 才算创建成功（并加入 `FBList` 与设置各字段）。</summary>
    public static bool IsFbMapCreated(object? envir) => envir is not null;

    /// <summary>2244：成功时加入 `FBList`。</summary>
    public static bool ShouldAddToFbList(bool created) => created;

    /// <summary>
    /// 2225-2251：初始化单个副本地图条目。
    /// </summary>
    public sealed class FbMapEntry
    {
        public string MapName = "";
        public bool BoFB;
        public string FbName = "";
        public FbEnterLimit EnterLimit;
        public int EnterDelayMs;
        public int NoHumClearMs;
    }

    /// <summary>
    /// 2239-2243：把一个成功创建的 `Envir` 填成副本地图条目。
    /// </summary>
    public static FbMapEntry MakeFbMapEntry(string mapName, FbDecl decl)
    {
        return new FbMapEntry
        {
            MapName = mapName,
            BoFB = true,                                        // 2239
            FbName = decl.FbName,                               // 2240
            EnterLimit = decl.EnterLimit,                       // 2241
            EnterDelayMs = EnterDelayMinToMs(decl.EnterDelayMin),      // 2242：分→毫秒
            NoHumClearMs = NoHumClearMinToMs(decl.NoHumClearFbMin),    // 2243：秒→毫秒
        };
    }

    // ===================== FindMonster / GetMonRace（UsrEngn.pas） =====================

    /// <summary>`FindMonster`（11142-11177）的返回：**`Index` 无条件赋值**。</summary>
    public readonly struct FindMonsterResult
    {
        public readonly bool Found;

        /// <summary>
        /// 11170：`Index := L` —— **未命中时也是有效下标（插入位置），不是 -1**。
        /// </summary>
        public readonly int Index;

        public FindMonsterResult(bool found, int index)
        {
            Found = found;
            Index = index;
        }
    }

    /// <summary>
    /// 11142-11177：在**按名称升序**的怪物表中二分查找。
    /// 与 J109 的 `FindFirstSortMapMonGenListIndex` 差异：
    /// **返回 `Index` 无条件赋值**（11170 在 `while` 之外），故未命中时 `Index` = 插入位置。
    /// 命中时 `Index` = **第一个**匹配位置（因命中后仍 `H := I - 1` 继续左收缩）。
    /// </summary>
    public static FindMonsterResult FindMonster<T>(IList<T> sortedList, Func<T, string> getName, string monName)
    {
        bool found = false;
        int l = 0;
        int h = sortedList.Count - 1;

        while (l <= h)
        {
            int i = (l + h) >> 1;
            int c = MonGenLoadCore.AnsiCompareText(getName(sortedList[i]), monName);

            if (c < 0)
            {
                l = i + 1;
            }
            else
            {
                h = i - 1;

                if (c == 0)
                    found = true;
            }
        }

        // 11170：无条件赋值
        return new FindMonsterResult(found, l);
    }

    /// <summary>
    /// 714-721：`GetMonRace` —— 命中则返回 `btRace`，**未命中返回 -1**。
    /// **取值靠布尔返回值而非 `Index`**，这正是 11170 无条件赋值的防护点。
    /// </summary>
    public static int GetMonRace<T>(IList<T> sortedList, Func<T, string> getName, Func<T, byte> getRace, string monName)
    {
        var r = FindMonster(sortedList, getName, monName);
        return r.Found ? getRace(sortedList[r.Index]) : -1;
    }

    /// <summary>723-730：`GetMonRaceImg` —— 同样未命中返回 -1。</summary>
    public static int GetMonRaceImg<T>(IList<T> sortedList, Func<T, string> getName, Func<T, byte> getRaceImg, string monName)
    {
        var r = FindMonster(sortedList, getName, monName);
        return r.Found ? getRaceImg(sortedList[r.Index]) : -1;
    }

    /// <summary>732-746：`GetMonRaceImgAndAppr` —— 未命中返回 false 且不写出参。</summary>
    public static bool GetMonRaceImgAndAppr<T>(
        IList<T> sortedList, Func<T, string> getName, Func<T, byte> getRaceImg, Func<T, ushort> getAppr,
        Func<T, byte> getRace, string monName,
        out byte monRaceImg, out ushort monAppr, out byte race)
    {
        var r = FindMonster(sortedList, getName, monName);

        if (r.Found)
        {
            monRaceImg = getRaceImg(sortedList[r.Index]);
            monAppr = getAppr(sortedList[r.Index]);
            race = getRace(sortedList[r.Index]);
            return true;
        }

        monRaceImg = 0;
        monAppr = 0;
        race = 0;
        return false;
    }
}
