using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.Core;

/// <summary>Share.pas 1:1 转换（IP 转换 + 系统版本；GetWindowsVersion 用 Environment.OSVersion 等效实现）。</summary>
public static class Share
{
    public struct TIPaddr
    {
        public byte A;
        public byte B;
        public byte C;
        public byte D;
        public int Port;
    }

    public static string MakeIPToStr(in TIPaddr iPAddr)
        => $"{iPAddr.A}.{iPAddr.B}.{iPAddr.C}.{iPAddr.D}";

    public static string MakeIntToIP(int nIPaddr)
    {
        TIPaddr ip = default;
        ip.A = DelphiRTL.LoByte(DelphiRTL.LoWord(nIPaddr));
        ip.B = DelphiRTL.HiByte(DelphiRTL.LoWord(nIPaddr));
        ip.C = DelphiRTL.LoByte(DelphiRTL.HiWord(nIPaddr));
        ip.D = DelphiRTL.HiByte(DelphiRTL.HiWord(nIPaddr));
        return MakeIPToStr(ip);
    }

    public static int MakeIPToInt(string sIPaddr)
    {
        int result = -1;
        string sA = "", sB = "", sC = "", sD = "";
        sIPaddr = DelphiRTL.Trim(HUtil32.GetValidStr3(sIPaddr, ref sA, new[] { '.' }));
        sIPaddr = DelphiRTL.Trim(HUtil32.GetValidStr3(sIPaddr, ref sB, new[] { '.' }));
        sD = DelphiRTL.Trim(HUtil32.GetValidStr3(sIPaddr, ref sC, new[] { '.' }));
        if (sA != "" && sB != "" && sC != "" && sD != "")
        {
            byte a = (byte)HUtil32.Str_ToInt(sA, 0);
            byte b = (byte)HUtil32.Str_ToInt(sB, 0);
            byte c = (byte)HUtil32.Str_ToInt(sC, 0);
            byte d = (byte)HUtil32.Str_ToInt(sD, 0);
            result = DelphiRTL.MakeLong(DelphiRTL.MakeWord(a, b), DelphiRTL.MakeWord(c, d));
        }
        return result;
    }

    /// <summary>取系统版本号（字符串形式）。.NET 下由 Environment.OSVersion 等效提供。</summary>
    public static string GetWindowsVersion()
    {
        var v = Environment.OSVersion;
        string result = "Microsoft Windows";
        switch (v.Platform)
        {
            case PlatformID.Win32Windows:
                if (v.Version.Major == 4 && v.Version.Minor == 0) result += " 95";
                else if (v.Version.Major == 4 && v.Version.Minor == 10) result += " 98";
                else if (v.Version.Major == 4 && v.Version.Minor == 90) result += " Millenium Edition";
                break;
            case PlatformID.Win32NT:
                result += $" NT {v.Version.Major}.{v.Version.Minor}";
                result += $" (Build {v.Version.Build & 0xFFFF})";
                break;
            default:
                result += $" {v.Version.Major}.{v.Version.Minor}";
                break;
        }
        return result;
    }
}
