using System;
using System.Globalization;
using GXX.Core.Protocol;
using TShiftState = GXX.Client.GUI.Share.TShiftState;
// TFontStyles 已由 GUI/Share 车道定义（FStateSeams.cs:440，Delphi Graphics.TFontStyles set of）；
// 用别名引用以免与同时 using 两个命名空间的文件产生 CS0104（与 TShiftState 同样的处理）。
using TFontStyles = GXX.Client.GUI.Share.TFontStyles;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【P17 切片1】MShare.pas —— 单元级函数真身（第一批：无外部资源依赖的纯函数）
//
// 命名 100% 保留 Delphi 原名与参数顺序；控制流、早退顺序、边界逐条照抄原文。
// 行号 = `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号。
// 计数对账（本文件）：真实体 21 / NotPorted 0 / 原文如此 3 = 24。
//   其中 3 条「原文如此」＝ ActorXYToMapXY·MapXYToActorXY 的 Y 轴恒等式（P17-ASIS-01）、
//   IntToHexN 的 Digits 当进制 + `>10` 早退（P17-DEF-02）、
//   GetInputBoxInFilterList 对 nil 过滤表无 Assigned 判断（P17-DEF-03）。
// ============================================================================================

/// <summary>
/// MShare.pas 的单元级函数（自 3062 implementation 段起）。
/// 本文件只承载**不依赖图像/资源加载**的纯函数，便于以真实断言锁死行为。
/// </summary>
public static unsafe class MShareFunctions
{
    // ================================================================================
    // 物品叠加判定（原文 4084-4100）
    //
    // 原文形参是 `PTClientItem`（指针），托管侧直接沿用 `TClientItem*`：
    // `TStdItem.Name` / `Item.NewValue` 等都是 C# 定长缓冲区，走 `ref`/`in` 参数会被
    // 编译器拒绝（CS1666「不能使用非固定表达式中包含的固定大小缓冲区」）。
    // ================================================================================

    /// <summary>
    /// MShare.pas:4084 `function IsOverLapItem(Item:PTClientItem):Boolean`。
    /// 叠加物品加入 31 chongchong 2014-04-08。
    /// </summary>
    public static bool IsOverLapItem(TClientItem* Item)
        => (Item->s.NameStr != "")
           && StdModeInOverLapSet(Item->s.StdMode)
           && (Item->s.OverLap > 0);

    /// <summary>
    /// MShare.pas:4090 `function IsOverLapItem(Item1, Item2:PTClientItem):Boolean`（原文 overload）。
    /// 物品名比较用 `=`（原文 AnsiString 逐字节相等，此处按序号相等）。
    /// </summary>
    public static bool IsOverLapItem(TClientItem* Item1, TClientItem* Item2)
        => (Item1->s.NameStr != "")
           && StdModeInOverLapSet(Item1->s.StdMode)
           && (Item1->s.OverLap > 0)
           && (Item2->s.NameStr != "")
           && StdModeInOverLapSet(Item2->s.StdMode)
           && (Item2->s.OverLap > 0)
           && (Item1->s.NameStr == Item2->s.NameStr)
           && (Item1->s.StdMode == Item2->s.StdMode);

    /// <summary>
    /// MShare.pas:4096 `function IsUnOverLapItem(Item:PTClientItem):Boolean`。
    /// 与 `IsOverLapItem(Item)` 的差别只在**第 3 项判据**：此处是 `Item.Dura &gt; 0`。
    /// </summary>
    public static bool IsUnOverLapItem(TClientItem* Item)
        => (Item->s.NameStr != "")
           && StdModeInOverLapSet(Item->s.StdMode)
           && (Item->Dura > 0)
           && (Item->s.OverLap > 0);

    /// <summary>原文三处共用的 `Item.s.StdMode in [0, 2, 3, 31, 40, 41, 42, 46, 47]`。</summary>
    private static bool StdModeInOverLapSet(byte stdMode)
        => stdMode == 0 || stdMode == 2 || stdMode == 3 || stdMode == 31
           || stdMode == 40 || stdMode == 41 || stdMode == 42 || stdMode == 46 || stdMode == 47;

    // ================================================================================
    // 特性长度 / 调色板（原文 4102-4122）
    // ================================================================================

    /// <summary>
    /// MShare.pas:4102 `function GetFeatureLen(nLen:Integer):Integer`。
    /// 用启动期算得的 `SizeOf(THumFeature)` / `SizeOf(TMonFeature)` 与入参比对后替换。
    /// </summary>
    public static int GetFeatureLen(int nLen)
    {
        int Result = nLen;
        if (nLen == MShareGlobals.g_nHumFeature)
            Result = MShareGlobals.g_nHumFeature;
        else if (nLen == MShareGlobals.g_nMonFeature)
            Result = MShareGlobals.g_nMonFeature;
        return Result;
    }

    /// <summary>
    /// MShare.pas:4111 `function GetRGB(c256:byte):Integer`：
    /// `RGB(g_DefColorTable[c256].rgbRed, g_DefColorTable[c256].rgbGreen, g_DefColorTable[c256].rgbBlue)`。
    /// </summary>
    public static int GetRGB(byte c256)
    {
        var e = MShareGlobals.g_DefColorTable[c256];
        return RGB(e.rgbRed, e.rgbGreen, e.rgbBlue);
    }

    /// <summary>Windows `RGB(r,g,b)` 宏 `:= r or (g shl 8) or (b shl 16)`。</summary>
    private static int RGB(byte r, byte g, byte b) => r | (g << 8) | (b << 16);

    /// <summary>
    /// MShare.pas:4116 `function RGB32(C:LongInt; BitCount:Byte):TColor`。
    /// BitCount = 16 时按 `$F8`/`$FC`/`$F8` 掩码右移/左移做 5-6-5 还原，否则原样返回。
    /// 返回 `Integer`（Delphi `TColor` 就是 Integer）；对应托管 `TColor.Value`。
    /// </summary>
    public static int RGB32(int C, byte BitCount)
    {
        if (BitCount == 16)
            return RGB((byte)(((C & 0xF8) >> 8) & 0xFF), (byte)((C & 0xFC) >> 3), (byte)((C & 0xF8) << 3));
        return C;
    }

    // ================================================================================
    // 职业 / 性别名（原文 8957-8983）
    // ================================================================================

    /// <summary>MShare.pas:8957 `function GetJobName(nJob:Integer):string`。0/1/2 → 战士/法师/道士，else 未知。</summary>
    public static string GetJobName(int nJob)
    {
        switch (nJob)
        {
            case 0: return MShareGlobals.g_sWarriorName;
            case 1: return MShareGlobals.g_sWizardName;
            case 2: return MShareGlobals.g_sTaoistName;
            default: return MShareGlobals.g_sUnKnowName;
        }
    }

    /// <summary>MShare.pas:8973 `function GetSexName(nSex:Integer):string`。0 → '男'，1 → '女'，其余 → ''。</summary>
    public static string GetSexName(int nSex)
        => nSex switch
        {
            0 => "男",
            1 => "女",
            _ => "",
        };

    // ================================================================================
    // 座标换算（原文 4181-4201）
    // ================================================================================

    /// <summary>
    /// MShare.pas:4181 `procedure ActorXYToMapXY(nCurrX, nCurrY:Integer; var nX, nY:Integer)`。
    /// `nX := nCurrX * 48 div 32; nY := nCurrY * 32 div 32;`（原文 Y 轴的 `* 32 div 32` 是恒等，**照抄**）。
    /// </summary>
    public static void ActorXYToMapXY(int nCurrX, int nCurrY, out int nX, out int nY)
    {
        nX = nCurrX * 48 / 32;
        nY = nCurrY * 32 / 32;
    }

    /// <summary>MShare.pas:4187 `procedure MapXYToActorXY(nMapX, nMapY:Integer; var nX, nY:Integer)`。</summary>
    public static void MapXYToActorXY(int nMapX, int nMapY, out int nX, out int nY)
    {
        nX = nMapX * 32 / 48;
        nY = nMapY * 32 / 32;
    }

    /// <summary>
    /// MShare.pas:4193 `procedure MapToScreen(nMapWH, nScreenWH, nMapXY:Integer; var nXY:Integer)`。
    /// 原文 `Round(nScreenWH * nMapXY / nMapWH)`；托管侧用 `MidpointRounding.ToEven`
    /// 对齐 Delphi `Round` 的**银行家舍入**。
    /// </summary>
    public static void MapToScreen(int nMapWH, int nScreenWH, int nMapXY, out int nXY)
        => nXY = (int)Math.Round(nScreenWH * nMapXY / (double)nMapWH, MidpointRounding.ToEven);

    /// <summary>MShare.pas:4198 `procedure ScreenToMap(nMapWH, nScreenWH, nScreenXY:Integer; var nXY:Integer)`。</summary>
    public static void ScreenToMap(int nMapWH, int nScreenWH, int nScreenXY, out int nXY)
        => nXY = (int)Math.Round(nMapWH * nScreenXY / (double)nScreenWH, MidpointRounding.ToEven);

    // ================================================================================
    // 字符串 / 数字格式（原文 8536-8570、11484-11519、11659-11680）
    // ================================================================================

    /// <summary>
    /// MShare.pas:8536 `function DeleteNumber(Src:string; var nNum:Integer):string`（原文是
    /// `LoadEffectImageList` 的内部函数；8619 处另有一份**逐字重复**的副本）。
    /// 从**尾部**数连续数字位；`nNum := StrToIntDef(数位串, 0)`（前导零被丢弃）；
    /// `nC = 0` 时整体原样返回。当 Src 全为数字时 `Len - nC = 0`，`Copy(Src,1,0) = ''`。
    /// </summary>
    public static string DeleteNumber(string Src, out int nNum)
    {
        int Len = Src?.Length ?? 0;
        int nC = 0;
        string sNum = "";
        for (int I = Len; I >= 1; I--)
        {
            char ch = Src[I - 1];
            if (ch >= '0' && ch <= '9')
            {
                nC++;
                sNum = ch + sNum;
            }
        }

        nNum = int.TryParse(sNum, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int parsed)
            ? parsed
            : 0;
        if (nC == 0)
            return Src;
        return Src.Substring(0, Len - nC);
    }

    /// <summary>
    /// MShare.pas:8557 `function DeleteFileExt(Src:string):string`（8640 处另有一份逐字重复的副本）。
    /// **注意**：原文用 `Pos('.', Src)`，即**第一个**点，不是最后一个 ⇒ `'a.b.c'` → `'a'`。
    /// </summary>
    public static string DeleteFileExt(string Src)
    {
        int nPos = Src?.IndexOf('.') ?? -1;
        if (nPos >= 0)
            return Src.Substring(0, nPos);
        return Src;
    }

    /// <summary>
    /// MShare.pas:11659 `function tick_diff(tick_start, tick_end:Cardinal):Cardinal`。
    /// `tick_end &gt;= tick_start` 时相减，否则按 `High(Cardinal)` 回绕。
    /// Cardinal 是无符号语义，托管侧用 `uint`（**不用 long**，否则负入参行为不一致）。
    /// </summary>
    public static uint tick_diff(uint tick_start, uint tick_end)
    {
        if (tick_end >= tick_start)
            return tick_end - tick_start;
        return uint.MaxValue - tick_start + tick_end;
    }

    /// <summary>MShare.pas:11667 `function GetTickCount_Ex():LongWord`：`Result := TimeGetTime()`。</summary>
    public static uint GetTickCount_Ex() => (uint)Environment.TickCount;

    /// <summary>
    /// MShare.pas:11672 `function HpAddUnit(V:LongWord):string`。
    /// ≥1e8 → `%.2fE`（**原文除的是 1e8，不是 1e9**，照抄）；≥1e5 → `V div 10000` + 'W'；否则十进制。
    /// 分隔符用 InvariantCulture 固定为 '.'（Delphi `Format` 不带千分位、不带本地化）。
    /// </summary>
    public static string HpAddUnit(uint V)
    {
        if (V >= 100000000)
            return (V / 100000000.0).ToString("F2", CultureInfo.InvariantCulture) + "E";
        if (V >= 100000)
            return (V / 10000).ToString(CultureInfo.InvariantCulture) + "W";
        return V.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// MShare.pas:11484 `function IntToHexN(const V, Digits:Integer):string`。
    /// ★ **名不副实（原文缺陷，照抄）**：原文用 `I mod Digits` / `I div Digits`，把 `Digits` 当
    /// **进制**用，而不是十六进制的位数；且 `Digits &gt; 10` 或 `Digits &lt; 2` 直接返回空串。
    /// ⇒ **不存在 `Digits = 16` 的十六进制形态**，此时恒返回 `''`。已用真实断言锁死（含 3/4/5/8/9 进制）。
    /// </summary>
    public static string IntToHexN(int V, int Digits)
    {
        // 原文 const CSTR = '00000000000000000000000000000000'（32 个 '0'）
        int capacity = 32;
        // 原文 const Convert2:array[0..9] of Char = ('0'..'9')
        string Convert2 = "0123456789";

        string Result = "";
        if (Digits > 10)
            return Result;
        if (Digits < 2)
            return Result;

        var P = new char[capacity];
        for (int i = 0; i < capacity; i++) P[i] = '0';

        int p1Idx = capacity - 1;
        long I = V;
        while (true)
        {
            P[p1Idx] = Convert2[(int)(I % Digits)];
            I = I / Digits;
            if (I == 0)
                break;
            // 原文顺序：先比较 `if P1 - P <= 0 then Break`，**通过后**才 `Dec(P1)`。
            // 所以 P1 最终停在「最后一个有效数字」所在的格，`NewLen := 32 - (P1 - P)`。
            // 若把 Dec 提前到比较之前，IntToHexN(0,16) 会多出前导 '0'。
            if (p1Idx <= 0)
                break;
            p1Idx--;
        }

        int NewLen = capacity - p1Idx;
        Result = new string(P, p1Idx, NewLen);
        return Result;
    }

    // ================================================================================
    // 输入过滤 / 攻击判定（原文 11781-11784、11959-11983）
    // ================================================================================

    /// <summary>
    /// MShare.pas:11959 `function GetInputBoxInFilterList(sInputBox:string):Boolean`。
    /// 先逐**字符**查 `@ &lt; &gt; $`（原文用 WideString 逐字符，托管 string 是 UTF-16，等价）；
    /// 再把整串转小写，逐个过滤词做 `Pos(词, 串) &gt; 0` 子串包含判定。
    /// </summary>
    public static bool GetInputBoxInFilterList(string sInputBox)
    {
        string text = sInputBox ?? "";
        for (int I = 0; I < text.Length; I++)
        {
            char WChr = text[I];
            if (WChr == '@' || WChr == '<' || WChr == '>' || WChr == '$')
                return true;
        }

        string lowered = text.ToLowerInvariant();
        var list = MShareGlobals.g_InputBoxFilterList;
        if (list == null)
            return false;
        for (int I = 0; I < list.Count; I++)
        {
            if (list[I] != null && list[I].Length > 0 && lowered.Contains(list[I]))
                return true;
        }
        return false;
    }

    /// <summary>
    /// MShare.pas:11781 `function IsAttackAction(Action:Integer):Boolean`。
    /// 18 个具名攻击动作码 + `SM_CUSTOM_HIT001 .. SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1` 区间。
    /// </summary>
    public static bool IsAttackAction(int Action)
        => Action == Grobal2Const.SM_HIT
           || Action == Grobal2Const.SM_HEAVYHIT
           || Action == Grobal2Const.SM_BIGHIT
           || Action == Grobal2Const.SM_POWERHIT
           || Action == Grobal2Const.SM_LONGHIT
           || Action == Grobal2Const.SM_WIDEHIT
           || Action == Grobal2Const.SM_FIREHIT
           || Action == Grobal2Const.SM_CRSHIT
           || Action == Grobal2Const.SM_TWNHIT
           || Action == Grobal2Const.SM_SWORDHIT
           || Action == Grobal2Const.SM_43HIT
           || Action == Grobal2Const.SM_66HIT
           || Action == Grobal2Const.SM_66HIT1
           || Action == Grobal2Const.SM_101HIT
           || Action == Grobal2Const.SM_102HIT
           || Action == Grobal2Const.SM_103HIT
           || Action == Grobal2Const.SM_113HIT
           || Action == Grobal2Const.SM_115HIT
           || (Action >= Grobal2Const.SM_CUSTOM_HIT001
               && Action < Grobal2Const.SM_CUSTOM_HIT001 + Grobal2Const.CUSTOM_MAGIC_COUNT);

    // ================================================================================
    // 包裹格位上限（原文 11762-11765）
    // ================================================================================

    /// <summary>MShare.pas:11762 `function GetMaxBagCount:Integer`：`DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount`。</summary>
    public static int GetMaxBagCount() => Grobal2Const.DEF_MAX_BAG_ITEM + MShareGlobals.g_ExtBagOpenItemCount;

    // ================================================================================
    // 【P17 切片3 · 优先③ 平台族】只需 BCL / 既有托管类型
    // ================================================================================

    /// <summary>
    /// MShare.pas:3267 `function IsInContinuous:Boolean`。
    /// 原文 `Result := g_boContinuous;`（后面那句 `InterlockedCompareExchange` 是**注释**，照抄不启用）。
    /// </summary>
    public static bool IsInContinuous() => MShareGlobals.g_boContinuous;

    /// <summary>
    /// MShare.pas:11565 `function ProcessFileNameSpecialChar(S:string):string`。
    /// 把 9 个 Windows 文件名非法字符逐字符替换（原文用 WideString 逐字符，托管 string 是 UTF-16，等价）。
    /// </summary>
    public static string ProcessFileNameSpecialChar(string S)
    {
        if (string.IsNullOrEmpty(S))
            return S;

        var WS = S.ToCharArray();
        for (int I = 0; I < WS.Length; I++)
        {
            switch (WS[I])
            {
                case '/': WS[I] = '{'; break;
                case '\\': WS[I] = '}'; break;
                case ':': WS[I] = ';'; break;
                case '*': WS[I] = '@'; break;
                case '?': WS[I] = '!'; break;
                case '"': WS[I] = '~'; break;
                case '<': WS[I] = '('; break;
                case '>': WS[I] = ')'; break;
                case '|': WS[I] = '-'; break;
            }
        }
        return new string(WS);
    }

    /// <summary>
    /// MShare.pas:11682 `function GetTempDir:string`：
    /// `GetTempPath(SizeOf(Buf) div SizeOf(Buf[0]), Buf); Result := StrPas(Buf);`
    /// Win32 `GetTempPath` 的返回串**带结尾反斜杠**；.NET `Path.GetTempPath()` 语义相同。
    /// （原文全文**无调用点**，见报告 §4.5。）
    /// </summary>
    public static string GetTempDir() => System.IO.Path.GetTempPath();

    /// <summary>
    /// MShare.pas:11690 `function MakeTempFileName(const FileExt:string):string`。
    /// `QueryPerformanceCounter(N)` 成功 ⇒ `Format('%x', [N])`（**小写十六进制、无前导零**）；
    /// 失败 ⇒ `Format('%.8x%.4x', [MyGetTickCount, Random(MAXINT)])`（**各带前导零**）。
    /// 最后 `if Length(FileExt) > 0 then Result := Result + '.' + FileExt;`
    ///
    /// 托管侧 `Stopwatch.GetTimestamp()` 即 `QueryPerformanceCounter`，故默认走第一分支。
    /// 第二分支按原文的 `%.8x`/`%.4x`（含前导零）保留，供测试注入。
    /// 原文**没有**拼目录（`GetTempDir` 无调用点）⇒ 返回值是**裸文件名**。
    /// </summary>
    public static string MakeTempFileName(string FileExt)
    {
        string Result;
        if (PerformanceCounterProvider != null)
        {
            Result = PerformanceCounterProvider().ToString("x", CultureInfo.InvariantCulture);
        }
        else if (QueryPerformanceCounterFailsForTests)
        {
            uint tick = MShareGlobals.MyGetTickCount;
            int rnd = RandomProvider?.Invoke() ?? 0;
            Result = tick.ToString("x8", CultureInfo.InvariantCulture)
                     + ((uint)rnd).ToString("x4", CultureInfo.InvariantCulture);
        }
        else
        {
            Result = System.Diagnostics.Stopwatch.GetTimestamp()
                .ToString("x", CultureInfo.InvariantCulture);
        }

        if (FileExt != null && FileExt.Length > 0)
            Result = Result + "." + FileExt;
        return Result;
    }

    /// <summary>测试注入点：`QueryPerformanceCounter` 的返回（原文 `N:Int64`）。</summary>
    public static Func<long> PerformanceCounterProvider;

    /// <summary>测试注入点：强制走原文的 `QueryPerformanceCounter` **失败**分支。</summary>
    public static bool QueryPerformanceCounterFailsForTests;

    /// <summary>测试注入点：原文失败分支里的 `Random(MAXINT)`。</summary>
    public static Func<int> RandomProvider;

    /// <summary>
    /// MShare.pas:8984 `function _FileSize(const fname:string):LongWord`。
    /// 原文用 `FindFirst(ExpandFileName(fname), faAnyFile, SearchRec)`：**找到才返回
    /// `SearchRec.Size`，否则返回 0**。托管侧只支持文件（目录按"找不到"处理 ⇒ 返回 0），
    /// 并在读取异常时返回 0（对应原文"找不到"语义）。
    /// </summary>
    public static uint FileSize(string fname)
    {
        try
        {
            string full = System.IO.Path.GetFullPath(fname);
            if (!System.IO.File.Exists(full))
                return 0;
            var len = new System.IO.FileInfo(full).Length;
            if (len < 0)
                return 0;
            if (len > uint.MaxValue)
                return uint.MaxValue;   // 原文 LongWord 截断语义的保守上界
            return (uint)len;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// MShare.pas:6542 `function GetAbsolutePathEx(BasePath, RelativePath:string):string`。
    /// 原文 `FillChar(Dest, MAX_PATH+1, 0); PathCombine(Dest, BasePath, RelativePath)`；
    /// 托管侧 `Path.Combine` 语义一致（相对段直接拼接，不解析 `..`）。异常时回退为原文的
    /// "只填了 BasePath"形态（原文若 `PathCombine` 失败则 Dest 保持全 0 ⇒ 空串）。
    /// </summary>
    public static string GetAbsolutePathEx(string BasePath, string RelativePath)
    {
        try
        {
            return System.IO.Path.Combine(BasePath, RelativePath);
        }
        catch (ArgumentException)
        {
            // 原文 PathCombine 失败 ⇒ Dest 未被写入（全 0）⇒ Result = ''
            return "";
        }
    }

    // ================================================================================
    // 【P17 切片3 · 优先③】EnterGate(Plug) 的 ShiftState 位映射（原文 11786-11809）
    // ================================================================================

    /// <summary>MShare.pas:2582 `ShiftState_Shift = 1`（可组合）。</summary>
    public const int ShiftState_Shift = 1;

    /// <summary>MShare.pas:2583 `ShiftState_Alt = 2`。</summary>
    public const int ShiftState_Alt = 2;

    /// <summary>MShare.pas:2584 `ShiftState_Ctrl = 4`。</summary>
    public const int ShiftState_Ctrl = 4;

    /// <summary>MShare.pas:2585 `ShiftState_Left = 8`。</summary>
    public const int ShiftState_Left = 8;

    /// <summary>MShare.pas:2586 `ShiftState_Right = 16`。</summary>
    public const int ShiftState_Right = 16;

    /// <summary>MShare.pas:2587 `ShiftState_Middle = 32`。</summary>
    public const int ShiftState_Middle = 32;

    /// <summary>MShare.pas:2588 `ShiftState_Double = 64`。</summary>
    public const int ShiftState_Double = 64;

    /// <summary>
    /// MShare.pas:11786 `function ShiftStateToPlugShiftState(Shift:TShiftState):Integer`。
    /// 七个独立 `if`（**不是 else-if**）依次累加位；顺序 Shift→Alt→Ctrl→Left→Right→Middle→Double。
    /// </summary>
    public static int ShiftStateToPlugShiftState(TShiftState Shift)
    {
        int Result = 0;

        if ((Shift & TShiftState.ssShift) != 0)
            Result = Result + ShiftState_Shift;

        if ((Shift & TShiftState.ssAlt) != 0)
            Result = Result + ShiftState_Alt;

        if ((Shift & TShiftState.ssCtrl) != 0)
            Result = Result + ShiftState_Ctrl;

        if ((Shift & TShiftState.ssLeft) != 0)
            Result = Result + ShiftState_Left;

        if ((Shift & TShiftState.ssRight) != 0)
            Result = Result + ShiftState_Right;

        if ((Shift & TShiftState.ssMiddle) != 0)
            Result = Result + ShiftState_Middle;

        if ((Shift & TShiftState.ssDouble) != 0)
            Result = Result + ShiftState_Double;

        return Result;
    }

    // ================================================================================
    // 【P17 切片3 · 优先③】黑名单系统消息过滤（原文 11454-11482）
    // ================================================================================

    /// <summary>
    /// MShare.pas:11454 `function CheckBlockListSys(Ident:Integer; sMsg:string):Boolean`。
    ///
    /// 逐条照抄的控制流：
    ///  1. `try` 体内 `Result := True`；
    ///  2. `case Ident`：`SM_HEAR/SM_GROUPMESSAGE/SM_GUILDMESSAGE` → 以 `':'` 取第一段为用户名；
    ///     `SM_CRY` → 以 `':'` 取第一段后再 `RightStr(用户名, Length-3)`（**砍掉前 3 个字符**）；
    ///     `SM_WHISPER` → 以 `'='` 取第一段；**其它 Ident 不取**（用户名保持 `''`）；
    ///  3. 用户名非空时：先以 `' '` 再取一次第一段（"私聊显示等级时，黑名单不过滤"），
    ///     再查黑名单，命中 ⇒ `Result := False`；
    ///  4. **任何异常 ⇒ `Result := False`**（原文 11478-11481 的 `except`，HZQ 20230524 加）。
    /// </summary>
    public static bool CheckBlockListSys(int Ident, string sMsg)
    {
        try
        {
            bool Result = true;
            string sUserName = "";

            switch (Ident)
            {
                case Grobal2Const.SM_HEAR:
                case Grobal2Const.SM_GROUPMESSAGE:
                case Grobal2Const.SM_GUILDMESSAGE:
                    GetValidStr3_Ex(sMsg, ref sUserName, ':');
                    break;

                case Grobal2Const.SM_CRY:
                    GetValidStr3_Ex(sMsg, ref sUserName, ':');
                    // 原文 `RightStr(sUserName, Length(sUserName) - 3)`：取**后** Length-3 个字符。
                    // Length < 3 时 Delphi RightStr 的负长度行为未定义；托管侧按 `Max(0, ...)` 保守处理。
                    if (sUserName != null)
                    {
                        int keep = sUserName.Length - 3;
                        sUserName = keep > 0 ? sUserName.Substring(sUserName.Length - keep) : "";
                    }
                    break;

                case Grobal2Const.SM_WHISPER:
                    GetValidStr3_Ex(sMsg, ref sUserName, '=');
                    break;
            }

            if (!string.IsNullOrEmpty(sUserName))
            {
                // 私聊显示等级时，黑名单不过滤
                GetValidStr3_Ex(sUserName, ref sUserName, ' ');
                int I = BlacklistIndexOf(sUserName);
                if (I > -1)
                    Result = false;
            }

            return Result;
        }
        catch (Exception)
        {
            CheckBlockListSysExceptionHandler?.Invoke();
            return false; //HZQ 20230524添加异常后的返回值
        }
    }

    /// <summary>测试/接线注入点：原文 `DebugOutStr('[Exception] MShare.CheckBlockListSys')` 的留痕。</summary>
    public static Action CheckBlockListSysExceptionHandler;

    /// <summary>
    /// 原文 11475 `g_MyBlacklist.IndexOf(sUserName)` 的等价物。
    /// `THashedStringList` 继承 `TStringList`，其 `IndexOf` 走 `AnsiCompareText` ⇒ **大小写不敏感**；
    /// `AddObject` 保序 ⇒ 返回**第一个**命中的下标。
    /// </summary>
    private static int BlacklistIndexOf(string sUserName)
    {
        var list = MShareGlobals.g_MyBlacklist;
        if (list == null)
            return -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Equals(list[i], sUserName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    /// <summary>HUtil32.pas `GetValidStr3_Ex`：按分隔符切出第一段到 <paramref name="dest"/>，返回剩余串。</summary>
    private static string GetValidStr3_Ex(string str, ref string dest, char divider)
        => GXX.Core.Util.HUtil32.GetValidStr3_Ex(str, ref dest, divider);

    // ================================================================================
    // 【P17 切片C · hint 字体族 7 条】原文 11703-11760
    //
    // 全部只读 `g_ClientConfig` 的 7 个字段（切片A 已补进 MirForms.TConfigClient）。
    // ⚠ 原文写的是 `g_ClientConfig`，而 `MShare.pas:2180` 真正的声明是 `g_ConfigClient`
    //   （`g_ClientConfig` 全文无声明）⇒ 语义上就是 `MShareGlobals.g_ConfigClient`。见 P17-DEF-04。
    //
    // 【与既有 seam 的关系】`GUI/Share/FStateSeams.cs::MShareHintFont`（**不在本车道分区**）
    //   目前以「可注入 + 默认值兜底」承载了其中 3 条（`GetHintFontSize`/`GetHintFontStyle`/`GetHintFontStroke`，
    //   兜底值 9 / 原样 / 原样）。**真身在此落地**；seam 的退役与改指由集成方执行，
    //   退役清单与**语义差异**见报告 §5.3。
    // ================================================================================

    /// <summary>
    /// MShare.pas:11703 `function GetHintNameFontName:string`：
    /// `Result := g_ClientConfig.sShowHintFontName;`
    /// </summary>
    public static string GetHintNameFontName() => MShareGlobals.g_ConfigClient.ShowHintFontName;

    /// <summary>
    /// MShare.pas:11708 `function GetHintNameFontSize:Integer`：
    /// `Result := g_ClientConfig.btShowHintNameFontSize;`
    /// </summary>
    public static int GetHintNameFontSize() => MShareGlobals.g_ConfigClient.btShowHintNameFontSize;

    /// <summary>
    /// MShare.pas:11713 `function GetHintNameFontStyle(FontStyles:TFontStyles):TFontStyles`。
    /// `case` **无 `else`** ⇒ 值不在 0/1/2 时 Result 保持 Delphi 的**默认空集** `[]`
    /// （托管侧即 `TFontStyles.fsNone`，**不是**入参 `FontStyles`）—— 这点必须照抄，已用用例锁死。
    /// </summary>
    public static TFontStyles GetHintNameFontStyle(TFontStyles FontStyles)
    {
        TFontStyles Result = TFontStyles.fsNone;
        switch (MShareGlobals.g_ConfigClient.btShowHintNameFontBold)
        {
            case 0:
                Result = FontStyles;
                break;
            case 1:
                Result = TFontStyles.fsNone;
                break;
            case 2:
                Result = TFontStyles.fsBold;
                break;
        }
        return Result;
    }

    /// <summary>
    /// MShare.pas:11725 `function GetHintNameFontStroke(IsStroke:Boolean = False):Boolean`。
    /// 三个具名分支 + `else Result := False`（原文 11731，**HZQ 20230524 加的**）。
    /// </summary>
    public static bool GetHintNameFontStroke(bool IsStroke = false)
    {
        switch (MShareGlobals.g_ConfigClient.btShowHintNameFontStroke)
        {
            case 0: return IsStroke;
            case 1: return false;
            case 2: return true;
            default: return false; //HZQ 20230524
        }
    }

    /// <summary>
    /// MShare.pas:11735 `function GetHintFontSize:Integer`：
    /// `Result := g_ClientConfig.btShowHintOtherFontSize;`
    /// </summary>
    public static int GetHintFontSize() => MShareGlobals.g_ConfigClient.btShowHintOtherFontSize;

    /// <summary>
    /// MShare.pas:11740 `function GetHintFontStyle(FontStyles:TFontStyles):TFontStyles`。
    /// 与 `GetHintNameFontStyle` 同构（只是读 `btShowHintOtherFontBold`），同样**无 `else`**。
    /// </summary>
    public static TFontStyles GetHintFontStyle(TFontStyles FontStyles)
    {
        TFontStyles Result = TFontStyles.fsNone;
        switch (MShareGlobals.g_ConfigClient.btShowHintOtherFontBold)
        {
            case 0:
                Result = FontStyles;
                break;
            case 1:
                Result = TFontStyles.fsNone;
                break;
            case 2:
                Result = TFontStyles.fsBold;
                break;
        }
        return Result;
    }

    /// <summary>
    /// MShare.pas:11752 `function GetHintFontStroke(IsStroke:Boolean = False):Boolean`。
    /// 与 `GetHintNameFontStroke` 同构（只是读 `btShowHintOtherFontStroke`）。
    /// </summary>
    public static bool GetHintFontStroke(bool IsStroke = false)
    {
        switch (MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke)
        {
            case 0: return IsStroke;
            case 1: return false;
            case 2: return true;
            default: return false; //HZQ 20230524
        }
    }
}
