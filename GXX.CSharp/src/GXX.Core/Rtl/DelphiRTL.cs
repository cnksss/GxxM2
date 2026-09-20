using System;
using System.Diagnostics;
using System.Text;

namespace GXX.Core.Rtl;

/// <summary>
/// Delphi RTL 语义垫片：1:1 对应 Delphi7 System/SysUtils 中的常用函数语义。
/// 命名保持 Delphi 原名，便于与原代码逐行对照。
/// </summary>
public static unsafe class DelphiRTL
{
    public const int MaxInt = 0x7FFFFFFF;

    // ---------------- 位组装（Windows.pas） ----------------
    public static int MakeLong(int lo, int hi) => (hi << 16) | (lo & 0xFFFF);
    public static uint MakeLong(uint lo, uint hi) => (hi << 16) | (lo & 0xFFFF);
    public static ushort MakeWord(byte lo, byte hi) => (ushort)((hi << 8) | lo);
    public static int MakeWord(int lo, int hi) => (hi << 8) | (lo & 0xFF);
    public static byte LoByte(int w) => (byte)(w & 0xFF);
    public static byte HiByte(int w) => (byte)((w >> 8) & 0xFF);
    public static ushort LoWord(int l) => (ushort)(l & 0xFFFF);
    public static ushort HiWord(int l) => (ushort)((l >> 16) & 0xFFFF);

    // ---------------- 字符串（System.pas 语义，1-based 边界在调用处换算） ----------------

    /// <summary>Delphi Copy(S, Index, Count)：Index 1-based；越界返回空串；Count 超长截断。</summary>
    public static string Copy(string s, int index, int count)
    {
        if (s == null || index > s.Length) return "";
        if (index < 1) { count += index - 1; index = 1; }
        if (count <= 0) return "";
        int avail = s.Length - (index - 1);
        if (count > avail) count = avail;
        return s.Substring(index - 1, count);
    }

    /// <summary>Delphi Pos(Substr, S)：1-based；未找到 0。</summary>
    public static int Pos(string substr, string s)
    {
        if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(substr)) return 0;
        int i = s.IndexOf(substr, StringComparison.Ordinal);
        return i + 1;
    }

    /// <summary>Delphi Pos 重载：从指定 1-based 位置开始。</summary>
    public static int PosEx(string substr, string s, int offset)
    {
        if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(substr) || offset < 1) return 0;
        if (offset > s.Length) return 0;
        int i = s.IndexOf(substr, offset - 1, StringComparison.Ordinal);
        return i + 1;
    }

    // 原文 SysUtils.Trim/TrimLeft/TrimRight：
    //   while (I <= L) and (S[I] <= ' ') do Inc(I);      // 左
    //   while (L >= I) and (S[L] <= ' ') do Dec(L);      // 右
    // 判据是 `S[I] <= ' '` —— 即**所有 #0..#32**（含 NUL、BEL、ESC 等控制字符）；
    // 旧实现只去 6 个字符（#9 #10 #11 #12 #13 #32），**窄了**（车道 p4-m2-objnpc 复核发现，已修）。
    // 可达后果：原文 `"HERO\u0014.CHECKITEM"` 经 Trim 得 `"HERO"`，托管旧实现得 `"HERO\u0014"`，
    // 使 NPC 脚本的目标级别解析落到**不同分支**（CMD_RACE_1 vs CMD_RACE_5）。
    public static string Trim(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        int i = 0, l = s.Length;
        while (i < l && s[i] <= ' ') i++;
        while (l > i && s[l - 1] <= ' ') l--;
        return s.Substring(i, l - i);
    }

    public static string TrimLeft(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        int i = 0;
        while (i < s.Length && s[i] <= ' ') i++;
        return s.Substring(i);
    }

    public static string TrimRight(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        int l = s.Length;
        while (l > 0 && s[l - 1] <= ' ') l--;
        return s.Substring(0, l);
    }

    public static string UpperCase(string s) => s?.ToUpperInvariant() ?? "";
    public static string LowerCase(string s) => s?.ToLowerInvariant() ?? "";

    public static string IntToStr(int v) => v.ToString();
    public static string IntToStr(long v) => v.ToString();
    public static string IntToStr(uint v) => v.ToString();
    public static int StrToInt(string s)
    {
        s = s?.Trim() ?? "";
        long v = long.Parse(s, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowLeadingSign);
        return (int)v;
    }
    public static int StrToIntDef(string s, int def)
    {
        s = s?.Trim() ?? "";
        return long.TryParse(s, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowLeadingSign,
            System.Globalization.CultureInfo.InvariantCulture, out long v) && v is >= int.MinValue and <= int.MaxValue ? (int)v : def;
    }
    public static long StrToInt64Def(string s, long def)
    {
        s = s?.Trim() ?? "";
        return long.TryParse(s, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowLeadingSign,
            System.Globalization.CultureInfo.InvariantCulture, out long v) ? v : def;
    }
    public static bool TryStrToInt(string s, out int value)
    {
        value = 0;
        s = s?.Trim() ?? "";
        if (!long.TryParse(s, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowLeadingSign,
            System.Globalization.CultureInfo.InvariantCulture, out long v)) return false;
        if (v is < int.MinValue or > int.MaxValue) return false;
        value = (int)v;
        return true;
    }
    public static double StrToFloatDef(string s, double def)
    {
        s = s?.Trim() ?? "";
        return double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : def;
    }
    public static string FloatToStr(double v) => v.ToString("R", System.Globalization.CultureInfo.InvariantCulture);

    // ---------------- 布尔 → 字符串（SysUtils 语义；**只有这一套是 -1/0**） ----------------

    /// <summary>
    /// SysUtils.BoolToStr(B: Boolean; UseBoolStrs: Boolean = False)。
    ///
    /// Delphi 7 的**默认**重载（UseBoolStrs = False）返回 <c>'-1'</c>/<c>'0'</c> —— 既不是
    /// <c>'1'/'0'</c>（那是 HUtil32.BoolToStr2 与 TCustomIniFile.WriteBool），也不是
    /// <c>'True'/'False'</c>（那是 HUtil32.BoolToStr 的大写形态 'TRUE'/'FALSE' 之外的另一套）。
    /// UseBoolStrs = True 时返回 <c>'True'</c>/<c>'False'</c>（STrue/SFalse 资源串）。
    ///
    /// 依据：本仓库不含 Delphi RTL 源码（Source/ 下无 SysUtils.pas），无法给出 RTL 行号；
    /// 项目内三处按该语义独立落地的桩可互证：
    ///   * GXX.GameCenter/GShareTypes.cs:137-149（DelphiSystem.BoolToStr）
    ///   * GXX.M2Server/Forms/GeneralConfigForm.cs:474-475（DelphiBoolToStr）
    ///   * GXX.SelGate/SelGateConfig.cs:363
    /// 典型调用点：GameCenter/GMain.pas:1991-1993、M2Engine/Forms/GameConfig.pas:2146-2147
    /// （这两个单元的 uses 里**没有** HUtil32，故 BoolToStr 解析到 SysUtils 版 → 落盘 '-1'/'0'）。
    /// </summary>
    public static string BoolToStr(bool value, bool useBoolStrs = false)
        => useBoolStrs ? (value ? "True" : "False") : (value ? "-1" : "0");

    public static string Format(string fmt, params object[] args)
        => DelphiFormat.Format(fmt, args);

    // ---------------- 时间 ----------------

    /// <summary>对应 Delphi Now()（本地时间）。</summary>
    public static DateTime Now() => DateTime.Now;

    /// <summary>对应 Windows GetTickCount()：返回系统启动后毫秒数（uint 回绕）。</summary>
    public static uint GetTickCount() => (uint)Environment.TickCount;

    public static ulong GetTickCount64() => (ulong)Stopwatch.GetTimestamp() * 1000UL / (ulong)Stopwatch.Frequency;

    public static void Sleep(int ms) => System.Threading.Thread.Sleep(ms);

    // ---------------- 内存 ----------------

    /// <summary>Delphi FillChar。</summary>
    public static void FillChar(byte[] buf, int count, byte value)
        => Array.Fill(buf, value, 0, Math.Min(count, buf.Length));

    /// <summary>Delphi Move。</summary>
    public static void Move(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        => Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);

    public static void Move<T>(T[] src, int srcIdx, T[] dst, int dstIdx, int count)
        => Array.Copy(src, srcIdx, dst, dstIdx, count);

    /// <summary>Delphi CompareMem。</summary>
    public static bool CompareMem(byte[] a, byte[] b, int len)
    {
        if (a.Length < len || b.Length < len) return false;
        for (int i = 0; i < len; i++) if (a[i] != b[i]) return false;
        return true;
    }

    /// <summary>Delphi AllocMem/GetMem 等价的零初始化数组。</summary>
    public static byte[] AllocMem(int size) => new byte[size];

    // ---------------- MIN/MAX ----------------
    public static int _MIN(int a, int b) => a < b ? a : b;
    public static int _MAX(int a, int b) => a > b ? a : b;
    public static uint _MinLong(uint a, uint b) => a < b ? a : b;
    public static uint _MaxLong(uint a, uint b) => a > b ? a : b;
    public static long _MIN64(long a, long b) => a < b ? a : b;
    public static long _MAX64(long a, long b) => a > b ? a : b;

    // ---------------- 字节与字符串互转（GBK/AnsiString 语义） ----------------

    /// <summary>AnsiString → byte[]（GBK）。</summary>
    public static byte[] AnsiBytes(string s) => EncodingInit.GBK.GetBytes(s ?? "");

    /// <summary>byte[] → AnsiString（GBK）。</summary>
    public static string AnsiString(byte[] buf, int index = 0, int count = -1)
    {
        if (buf == null) return "";
        if (count < 0) count = buf.Length - index;
        if (count <= 0) return "";
        // 截断到第一个 \0（AnsiString 允许内嵌 0，但网关协议按 C 字符串处理处由调用方控制）
        return EncodingInit.GBK.GetString(buf, index, count);
    }

    /// <summary>模拟 Delphi PChar → string（取到 \0 为止）。</summary>
    public static string StrPas(byte[] buf, int index = 0)
    {
        if (buf == null) return "";
        int end = index;
        while (end < buf.Length && buf[end] != 0) end++;
        return EncodingInit.GBK.GetString(buf, index, end - index);
    }
}

/// <summary>Delphi Format 的格式串兼容层（%d %s %x %f %% 等）。</summary>
public static class DelphiFormat
{
    public static string Format(string fmt, params object[] args)
    {
        if (fmt == null) return "";
        var sb = new System.Text.StringBuilder();
        int argIdx = 0;
        for (int i = 0; i < fmt.Length; i++)
        {
            char c = fmt[i];
            if (c != '%') { sb.Append(c); continue; }
            if (i + 1 < fmt.Length && fmt[i + 1] == '%') { sb.Append('%'); i++; continue; }
            // 解析 %[-][width][.prec]type 或 %(index:)[-][width][.prec]type
            int j = i + 1;
            int index = -1;
            if (j < fmt.Length && fmt[j] == '*') { index = argIdx++; j++; if (j < fmt.Length && fmt[j] == ':') j++; }
            else
            {
                int k = j;
                while (k < fmt.Length && char.IsDigit(fmt[k])) k++;
                if (k < fmt.Length && fmt[k] == ':' && k > j) { index = int.Parse(fmt.Substring(j, k - j)) - 1; j = k + 1; }
            }
            bool left = false;
            if (j < fmt.Length && fmt[j] == '-') { left = true; j++; }
            int width = 0;
            while (j < fmt.Length && char.IsDigit(fmt[j])) { width = width * 10 + (fmt[j] - '0'); j++; }
            int prec = -1;
            if (j < fmt.Length && fmt[j] == '.')
            {
                j++; prec = 0;
                while (j < fmt.Length && char.IsDigit(fmt[j])) { prec = prec * 10 + (fmt[j] - '0'); j++; }
            }
            if (j >= fmt.Length) { sb.Append('%'); break; }
            char type = fmt[j];
            if (index < 0) index = argIdx++;
            object arg = index >= 0 && index < args.Length ? args[index] : null;
            sb.Append(FormatOne(arg, type, prec, width, left));
            i = j;
        }
        return sb.ToString();
    }

    private static string FormatOne(object arg, char type, int prec, int width, bool left)
    {
        string s;
        switch (char.ToLowerInvariant(type))
        {
            case 'd':
            {
                // Delphi 7 SysUtils.FormatBuf 的整数分支：实参是 Boolean（TVarRec.VType = vtBoolean）时
                // **不按 0/1 渲染**，而是与 SysUtils.BoolToStr 默认重载一致 → True = '-1'、False = '0'
                // （Delphi 的既有语义，不是笔误：整数分支里 Boolean 走的是 -1/0 的 Ordinal 约定）。
                // 依据：本仓库不含 Delphi RTL 源码（Source/ 下无 SysUtils.pas），无行号可引；见
                // GXX.Core.Rtl.DelphiRTL.BoolToStr 的 XML 注释列出的三处互证桩与典型调用点。
                // 另：%u / %x / %p 未跟随（Boolean 仍按 0/1 渲染）——RTL 里它们是否与 %d 同分支无法
                // 在本仓库取证，按"不凭直觉"原则保持现状，已登记为报告中的存疑项。
                long v = arg is bool bo ? (bo ? -1L : 0L) : Convert.ToInt64(arg ?? 0L);
                s = System.Math.Abs(v).ToString(new string('0', System.Math.Max(prec, 1)));
                if (v < 0) s = "-" + s;
                break;
            }
            case 'u': s = Convert.ToUInt64(arg ?? 0UL).ToString(); break;
            case 'x': s = Convert.ToInt64(arg ?? 0L).ToString(char.IsUpper(type) ? "X" : "x"); break;
            case 'e': s = Convert.ToDouble(arg ?? 0.0).ToString("E" + System.Math.Max(prec, 15), System.Globalization.CultureInfo.InvariantCulture); break;
            case 'f': s = Convert.ToDouble(arg ?? 0.0).ToString(prec >= 0 ? "F" + prec : "F2", System.Globalization.CultureInfo.InvariantCulture); break;
            case 'g': s = Convert.ToDouble(arg ?? 0.0).ToString("G15", System.Globalization.CultureInfo.InvariantCulture); break;
            case 'n': s = Convert.ToDouble(arg ?? 0.0).ToString("N" + System.Math.Max(prec, 2), System.Globalization.CultureInfo.InvariantCulture); break;
            case 'm': s = Convert.ToDouble(arg ?? 0.0).ToString("N2", System.Globalization.CultureInfo.InvariantCulture); break;
            case 's': s = arg?.ToString() ?? ""; if (prec >= 0 && s.Length > prec) s = s.Substring(0, prec); break;
            case 'p': s = "0x" + Convert.ToInt64(arg ?? 0L).ToString("x"); break;
            default: s = arg?.ToString() ?? ""; break;
        }
        if (width > s.Length) s = left ? s.PadRight(width) : s.PadLeft(width);
        return s;
    }
}
