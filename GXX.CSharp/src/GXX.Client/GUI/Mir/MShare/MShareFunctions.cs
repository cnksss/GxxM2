using System;
using System.Globalization;
using GXX.Core.Protocol;
using TShiftState = GXX.Client.GUI.Share.TShiftState;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【P17 切片1】MShare.pas —— 单元级函数真身（第一批：无外部资源依赖的纯函数）
//
// 命名 100% 保留 Delphi 原名与参数顺序；控制流、早退顺序、边界逐条照抄原文。
// 行号 = `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号。
// 计数对账（本文件）：真实体 21 / NotPorted 0 / 原文如此 2 = 23。
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
}
