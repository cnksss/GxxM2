using System;
using System.Collections.Generic;
using System.Text;

namespace GXX.M2Server;

/// <summary>
/// 怪物刷新配置（`MonGen.txt`）**文本解析层** 1:1 移植（批次J108），
/// 主源 `LocalDB.pas` 3528-3598，依赖 `HUtil32.pas` 的分隔串工具：
/// `GetValidStr3`(1243-1341)、`GetValidStrCap`(1344-1361)、`CompareLStr`(1981-1995)。
///
/// J106 移植了候选表的**消费**（`ProcessMonsters` 遍历 `m_MonGenList`）、
/// J107 移植了 `ProcessMonsters` 的外层骨架，本批次补上 `MonGenInfo` **从何而来**——
/// 即 `m_MonGenList` 的元素由 `MonGen.txt` 逐行解析而成。
///
/// **每行的字段顺序（3528-3598，共 16 个字段）**：
/// `地图名 X Y "怪物名" 范围 数量 刷新秒数 集中刷新率 名称颜色 内功怪 国家ID
///  可攻同国玩家 异国怪物PK 可被同国玩家攻 TriggerScript G变量序号 比较符 G变量值`
/// 注意 3528 的 `for I := 0 to LoadList.Count - 1` 循环体以
/// `if (sLineText &lt;&gt; '') and (sLineText[1] &lt;&gt; ';')` 为唯一门槛——
/// **空行与以 `;` 开头的行被跳过**，但**行首空格后的 `;` 不算注释**
/// （因只查 `sLineText[1]`）。
///
/// **三处易错的解析细节（已逐条固化）**：
///
/// ① **`sMonName` 用 `GetValidStrCap` 而非 `GetValidStr3`**（3541）——
/// 因此怪物名支持双引号包裹（名字含空格时必需）；若已用双引号
/// 则再调 `ArrestStringEx(sData, '"', '"', sData)` 剥掉引号本身（3542-3543）。
/// **其余字段一律用 `GetValidStr3`**，故字段间只能用空格/制表符分隔。
///
/// ② **`dwZenTime` 是"分钟数 × 60 × 1000"**（3550）：
/// `StrToIntDef(sData, -1) * 60 * 1000`——**默认值是 `-1` 而非 0**，
/// 故缺失该字段时得到 **-60000**（负数毫秒）。这与 3572 的校验
/// `dwZenTime = 0 → DisPose` 配合：**-60000 不等于 0，故不会被丢弃**，
/// 而是留下一个负刷怪间隔（在后续 `tick_diff` 语境下等价于"立即刷新"）。
///
/// ③ **`nMissionGenRate` 先经 `IsStringNumber` 过滤**（3552-3553）：
/// 非数字则强制置 `'0'` 再 `StrToIntDef`；而 `btNameColor` 的默认值是 **255**（3556），
/// `nRange`/`nCount`/`nX`/`nY` 默认 0，`GVarIndex` 默认 **-1**（3580）、
/// `GVarValue` 默认 **0**（3598）——**默认值各不相同，不可统一为 0**。
///
/// **布尔字段的统一写法（3558/3562/3564/3566）**：
/// `(Length(sData) &lt;&gt; 0) and (sData &lt;&gt; '0')`——即**空字符串与字面 `"0"` 都为假**，
/// 其余任何非空值（含 `"false"`、`"no"`）**都为真**。
///
/// **`TriggerScript` 需以 `@` 开头**（3568-3571），否则整字段置空。
///
/// **G 变量比较符（3582-3595）映射为 7 个分支**，其中**含 `<=` / `>=` / `<>` 三个双字符比较符**，
/// 判断顺序为 `<`、`=`、`>`、`<=`、`>=`、`<>`；由于先判单字符，
/// **双字符比较符只能落在后面的分支**，故顺序不可重排。无法识别则 `ctFail`。
///
/// **`CompareLStr(Src, targ, compn)`（1981-1995）是大小写不敏感的"前缀比较"**：
/// 先要求 `compn &gt; 0` 且两串长度都 `&gt;= compn`，再逐个字符 `UpCase` 比较。
/// 3517 用它判断行首是否为 `loadgen`（长度 7），**不区分大小写**。
/// </summary>
public static class MonGenParseCore
{
    /// <summary>3550：`dwZenTime` 的默认输入值（分钟）——**-1，非 0**。</summary>
    public const int DefaultZenTimeMinutes = -1;

    /// <summary>3550：分钟 → 毫秒的换算系数（原文写为 `* 60 * 1000`）。</summary>
    public const int ZenTimeMinutesToMs = 60 * 1000;

    /// <summary>3556：`btNameColor` 默认 255。</summary>
    public const int DefaultNameColor = 255;

    /// <summary>3580：`GVarIndex` 默认 -1。</summary>
    public const int DefaultGVarIndex = -1;

    /// <summary>`TCompareType`（原文枚举顺序）。</summary>
    public enum CompareType
    {
        ctLess,
        ctEqual,
        ctGreater,
        ctLessEqual,
        ctGreaterEqual,
        ctNotEqual,
        ctFail,
    }

    /// <summary>
    /// `MonGen.txt` 一行的解析结果（对应 `TMonGenInfo` 的可从文本得到的字段）。
    /// </summary>
    public sealed class MonGenLine
    {
        public string MapName = "";
        public int X;
        public int Y;
        public string MonName = "";
        public int Range;
        public int Count;

        /// <summary>3550：已换算为毫秒，且**默认可为负**（-60000）。</summary>
        public int ZenTimeMs;

        public int MissionGenRate;
        public int NameColor;
        public bool IsNGMon;
        public string NationaID = "";
        public bool CanAttackSameNationPlayer;
        public bool NoSameNationMonPK;
        public bool AllowSameNationPlayerAttack;
        public string TriggerScript = "";
        public int GVarIndex;
        public CompareType GVarCompareType;
        public int GVarValue;
    }

    // ===================== 基础工具（HUtil32.pas） =====================

    /// <summary>
    /// `CompareLStr`(1981-1995)：**大小写不敏感的前缀比较**。
    /// 需 `compn > 0` 且两串长度都 `>= compn`。
    /// </summary>
    public static bool CompareLStr(string src, string targ, int compn)
    {
        if (compn <= 0 || src.Length < compn || targ.Length < compn)
            return false;

        for (int i = 0; i < compn; i++)
        {
            if (char.ToUpperInvariant(src[i]) != char.ToUpperInvariant(targ[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// `GetValidStr3`(1243-1341)：取**第一个非分隔符段**作为 `dest`，
    /// 返回其**之后**的剩余串（已跳过紧随的一个分隔符）。
    /// 前导分隔符会被全部跳过；无分隔符时 `dest` 为整串、返回空。
    /// </summary>
    public static string GetValidStr3(string str, out string dest, params char[] dividers)
    {
        dest = str;
        if (str.Length == 0 || dividers.Length == 0)
            return "";

        bool isStart = false;
        int startIndex = 0;   // 原文 1-based，此处 0-based

        for (int i = 0; i < str.Length; i++)
        {
            bool isFound = false;
            foreach (char d in dividers)
            {
                if (str[i] == d)
                {
                    isFound = true;
                    break;
                }
            }

            if (isFound)
            {
                if (isStart)
                {
                    // 1283-1284
                    dest = str.Substring(startIndex, i - startIndex);
                    return str.Substring(i + 1);
                }
            }
            else if (!isStart)
            {
                isStart = true;
                startIndex = i;
            }
        }

        // 1296-1299：只有前导分隔符
        if (startIndex > 0)
            dest = str.Substring(startIndex);

        return "";
    }

    /// <summary>
    /// `GetValidStrCap`(1344-1361)：先 `TrimLeft`；若首字符为 `"` 则走 `CaptureString`
    /// （取引号内内容并返回剩余），否则退化为 `GetValidStr3`。
    /// </summary>
    public static string GetValidStrCap(string str, out string dest, params char[] dividers)
    {
        str = str.TrimStart();

        if (str.Length == 0)
        {
            dest = "";
            return "";
        }

        if (str[0] == '"')
            return CaptureString(str, out dest);

        return GetValidStr3(str, out dest, dividers);
    }

    /// <summary>
    /// `CaptureString`：从首字符 `"` 开始，取到**下一个** `"` 为止的内容，
    /// 返回其后的剩余串。未闭合时（无第二个引号）`dest` 为引号后全文、返回空。
    /// </summary>
    public static string CaptureString(string str, out string dest)
    {
        dest = "";
        if (str.Length == 0 || str[0] != '"')
            return str;

        int close = str.IndexOf('"', 1);
        if (close < 0)
        {
            // 未闭合：引号后全部作为内容（并跳过引号后可能的分隔符）
            string rest = str.Substring(1);
            return GetValidStr3(rest, out dest, ' ', '\t');
        }

        dest = str.Substring(1, close - 1);
        return str.Substring(close + 1);
    }

    /// <summary>
    /// `IsStringNumber`：全部字符为数字则为真；空串为假
    /// （3552 用 `not IsStringNumber` 决定是否强制置 `'0'`）。
    /// </summary>
    public static bool IsStringNumber(string s)
    {
        if (string.IsNullOrEmpty(s))
            return false;

        foreach (char c in s)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    /// <summary>
    /// `StrToIntDef`：解析十进制；失败或有**前导非数字**时返回默认值。
    /// Delphi 允许前后空白，此处按同语义处理。
    /// </summary>
    public static int StrToIntDef(string s, int def)
        => int.TryParse(s.Trim(), out int v) ? v : def;

    /// <summary>
    /// 3558/3562/3564/3566：布尔字段统一写法
    /// `(Length(sData) &lt;&gt; 0) and (sData &lt;&gt; '0')`。
    /// </summary>
    public static bool ParseBoolField(string sData)
        => sData.Length != 0 && sData != "0";

    /// <summary>3582-3595：G 变量比较符解析。**顺序不可重排**（先单字符后双字符）。</summary>
    public static CompareType ParseCompareType(string sData)
    {
        if (sData == "<") return CompareType.ctLess;
        if (sData == "=") return CompareType.ctEqual;
        if (sData == ">") return CompareType.ctGreater;
        if (sData == "<=") return CompareType.ctLessEqual;
        if (sData == ">=") return CompareType.ctGreaterEqual;
        if (sData == "<>") return CompareType.ctNotEqual;
        return CompareType.ctFail;
    }

    /// <summary>
    /// 3531：是否处理该行——**非空且首字符不是 `;`**。
    /// **只查首字符**，故行首空格后跟 `;` **不会**被当作注释。
    /// </summary>
    public static bool ShouldProcessLine(string line)
        => line.Length != 0 && line[0] != ';';

    /// <summary>3517：`loadgen` 指令判定（长度 7，大小写不敏感）。</summary>
    public static bool IsLoadGenLine(string line)
        => CompareLStr("loadgen", line, "loadgen".Length);

    // ===================== 逐行解析（3528-3598） =====================

    /// <summary>
    /// 解析 `MonGen.txt` 的一行（3528-3598）。
    /// 调用方应先以 `ShouldProcessLine` 过滤。
    /// </summary>
    public static MonGenLine ParseLine(string line)
    {
        var r = new MonGenLine();
        string sLineText = line;
        string sData;

        // 3535-3536
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.MapName = sData;

        // 3537-3538
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.X = StrToIntDef(sData, 0);

        // 3539-3540
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.Y = StrToIntDef(sData, 0);

        // 3541-3544：**用 GetValidStrCap**，支持双引号
        sLineText = GetValidStrCap(sLineText, out sData, ' ', '\t');
        if (sData.Length != 0 && sData[0] == '"')
        {
            // 3543：ArrestStringEx(sData, '"', '"', sData)
            string inner = sData;
            if (inner.Length >= 2 && inner[0] == '"')
            {
                int close = inner.IndexOf('"', 1);
                if (close > 0)
                    inner = inner.Substring(1, close - 1);
            }
            sData = inner;
        }
        r.MonName = sData;

        // 3545-3546
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.Range = StrToIntDef(sData, 0);

        // 3547-3548
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.Count = StrToIntDef(sData, 0);

        // 3549-3550：**默认 -1，且乘 60*1000**
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.ZenTimeMs = StrToIntDef(sData, DefaultZenTimeMinutes) * ZenTimeMinutesToMs;

        // 3551-3554：非数字先置 '0'
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        if (!IsStringNumber(sData))
            sData = "0";
        r.MissionGenRate = StrToIntDef(sData, 0);

        // 3555-3556：默认 255
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.NameColor = StrToIntDef(sData, DefaultNameColor);

        // 3557-3558
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.IsNGMon = ParseBoolField(sData);

        // 3559-3560
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.NationaID = sData;

        // 3561-3562
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.CanAttackSameNationPlayer = ParseBoolField(sData);

        // 3563-3564
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.NoSameNationMonPK = ParseBoolField(sData);

        // 3565-3566
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.AllowSameNationPlayerAttack = ParseBoolField(sData);

        // 3567-3571：需以 @ 开头
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        if (sData.Length > 0 && sData[0] == '@')
            r.TriggerScript = sData;
        else
            r.TriggerScript = "";

        // 3577-3580：G 变量序号，可带 'G' 前缀
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        if (sData != "" && sData[0] == 'G')
            sData = sData.Substring(1);
        r.GVarIndex = StrToIntDef(sData, DefaultGVarIndex);

        // 3581-3596：G 变量比较符
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.GVarCompareType = ParseCompareType(sData);

        // 3597-3598
        sLineText = GetValidStr3(sLineText, out sData, ' ', '\t');
        r.GVarValue = StrToIntDef(sData, 0);

        return r;
    }

    /// <summary>
    /// 3572：字段校验——`sMapName`/`sMonName` 为空或 `dwZenTime = 0` 时丢弃该行。
    /// **注意 `dwZenTime` 判断的是"等于 0"**，故负值（-60000）能通过。
    /// </summary>
    public static bool ShouldDiscardLine(MonGenLine r)
        => r.MapName == "" || r.MonName == "" || r.ZenTimeMs == 0;

    /// <summary>
    /// 3528-3674：解析整份列表，返回保留的行。
    /// 跳过不满足 `ShouldProcessLine` 的行，并在 3572 处丢弃字段不合格者。
    /// </summary>
    public static List<MonGenLine> ParseAll(IEnumerable<string> lines)
    {
        var result = new List<MonGenLine>();

        foreach (string line in lines)
        {
            if (!ShouldProcessLine(line))
                continue;

            var r = ParseLine(line);

            if (ShouldDiscardLine(r))
                continue;

            result.Add(r);
        }

        return result;
    }
}
