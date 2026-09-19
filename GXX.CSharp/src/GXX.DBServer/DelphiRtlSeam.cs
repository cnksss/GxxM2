using System;
using GXX.Core.Rtl;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：Delphi RTL（System.pas / SysUtils.pas）中 GXX.Core.Rtl.DelphiRTL 尚未提供的成员。
//   · Random(Range) / RandSeed
//   · DecodeDate / EncodeDate / DecodeTime / Date / Now(TDateTime)
//   · GetTickCount 可注入包装（单测替换时钟）
// 待 GXX.Core.Rtl.DelphiRTL 补齐后并入并删除本文件。
// ============================================================================================

/// <summary>Delphi SysUtils.EConvertError 的最小对应（EncodeDate/DecodeDate 非法值时抛）。</summary>
public class EConvertError : Exception
{
    public EConvertError(string message) : base(message) { }
}

/// <summary>
/// Delphi System.Random 语义。
/// 注意：Delphi 的 RandSeed 是 32 位 LCG（RandSeed := RandSeed * 134775813 + 1），
/// 本移植保留 "Random(0) = 0" 这一原文依赖的边界语义，但底层取数改用 .NET Random；
/// 原代码仅把 Random 用于网关/备用网关的负载选择（DBShare.pas:710/778/820），不参与协议字节，
/// 因此序列不同不影响功能等价。测试可通过 Next 注入确定性序列。
/// </summary>
public static class DelphiRandom
{
    private static readonly Random _rng = new Random();

    /// <summary>注入点：给定 Range(&gt;0) 返回 [0, Range) 的整数。测试可替换。</summary>
    public static Func<int, int> Next = range => _rng.Next(range);

    /// <summary>Delphi `Random(Range: Integer): Integer`：Range = 0 时返回 0。</summary>
    public static int Random(int Range)
        => Range == 0 ? 0 : Next(Range);
}

/// <summary>Delphi TDateTime（Double，天数；0 = 1899-12-30）。</summary>
public static class DelphiDate
{
    /// <summary>Delphi TDateTime 的零点（1899-12-30 00:00:00）。</summary>
    public static readonly DateTime Base = new DateTime(1899, 12, 30, 0, 0, 0, DateTimeKind.Unspecified);

    public static double FromDateTime(DateTime dt) => (dt - Base).TotalDays;

    public static DateTime ToDateTime(double tdt) => Base.AddDays(tdt);

    /// <summary>Delphi SysUtils.Now（TDateTime）。</summary>
    public static double Now() => FromDateTime(DateTime.Now);

    /// <summary>Delphi SysUtils.Date（TDateTime 的整数天，#1899-12-30 起）。</summary>
    public static double Date() => Math.Floor(FromDateTime(DateTime.Now));

    /// <summary>Delphi SysUtils.DecodeDate（仅覆盖 Date &gt;= 0 的正区间；原文只对 Now/Date 使用）。</summary>
    public static void DecodeDate(double date, out ushort year, out ushort month, out ushort day)
    {
        DateTime dt = ToDateTime(Math.Floor(date));
        year = (ushort)dt.Year;
        month = (ushort)dt.Month;
        day = (ushort)dt.Day;
    }

    /// <summary>Delphi SysUtils.DecodeTime（取 TDateTime 的小数部分）。</summary>
    public static void DecodeTime(double time, out ushort hour, out ushort min, out ushort sec, out ushort msec)
    {
        double frac = time - Math.Floor(time);
        double totalMs = Math.Round(frac * 86400000.0, MidpointRounding.AwayFromZero);
        int ms = (int)totalMs % 86400000;
        hour = (ushort)(ms / 3600000);
        ms %= 3600000;
        min = (ushort)(ms / 60000);
        ms %= 60000;
        sec = (ushort)(ms / 1000);
        msec = (ushort)(ms % 1000);
    }

    /// <summary>Delphi SysUtils.EncodeDate：非法日期抛 EConvertError（原文 FDBexpl.GetDateTime 依赖此校验）。</summary>
    public static double EncodeDate(ushort year, ushort month, ushort day)
    {
        if (year < 1 || year > 9999) throw new EConvertError("Invalid date: year out of range");
        if (month < 1 || month > 12) throw new EConvertError("Invalid date: month out of range");
        if (day < 1 || day > DaysInMonth(year, month)) throw new EConvertError("Invalid date: day out of range");
        return FromDateTime(new DateTime(year, month, day));
    }

    private static int DaysInMonth(int year, int month)
        => DateTime.DaysInMonth(year, month);
}

/// <summary>
/// Delphi Windows.GetTickCount 的可注入包装（uint 回绕语义取自 DelphiRTL.GetTickCount）。
/// 原文多处用 "GetTickCount - X &lt;= 3000" 判活跃，测试需要固定时钟。
/// </summary>
public static class DelphiTick
{
    public static Func<uint> GetTickCount = DelphiRTL.GetTickCount;
}

/// <summary>Delphi Boolean(1 字节) ↔ C# bool 的边界转换助手。</summary>
public static class TBool
{
    public static byte ToByte(bool v) => v ? (byte)1 : (byte)0;
    public static bool ToBool(byte v) => v != 0;
    public static bool ToBool(int v) => v != 0;
}

/// <summary>
/// 接缝：Delphi `StrUtils`（GXX.Core.Rtl.DelphiRTL 尚未提供）。
/// 原文使用处：RouteEdit.pas:217/510、DBShare.pas:742 等。
/// Delphi 的 SameText/CompareText 走 ANSI 区域比较，此处用 OrdinalIgnoreCase 等价（ASCII IP/端口串场景一致）。
/// </summary>
public static class DelphiStrUtils
{
    /// <summary>Delphi `StrUtils.SameText`：忽略大小写比较。</summary>
    public static bool SameText(string a, string b)
        => string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>Delphi `SysUtils.CompareText`：忽略大小写比较，返回 -1/0/1（原文用其 == 0 判等）。</summary>
    public static int CompareText(string a, string b)
        => Math.Sign(string.Compare(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase));
}

/// <summary>Dialogs 消息框常量（Windows.pas）。</summary>
public static class TMsgBox
{
    public const uint MB_OK = 0x00000000;
    public const uint MB_OKCANCEL = 0x00000001;
    public const uint MB_YESNO = 0x00000004;
    public const uint MB_ICONHAND = 0x00000010;
    public const uint MB_ICONERROR = 0x00000010;    // 同 MB_ICONHAND
    public const uint MB_ICONQUESTION = 0x00000020;
    public const uint MB_ICONEXCLAMATION = 0x00000030;
    public const uint MB_ICONINFORMATION = 0x00000040;
    public const int IDOK = 1;
    public const int IDYES = 6;
}

/// <summary>Delphi Forms 的 ModalResult 常量。</summary>
public static class TModalResult
{
    public const int mrNone = 0;
    public const int mrOK = 1;
    public const int mrCancel = 2;
    public const int mrYes = 6;
    public const int mrNo = 7;
}
