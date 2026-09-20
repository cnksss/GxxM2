// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（Delphi 7，GBK）—— **地址/IP 工具族**
//   实测 LF = 3595 行。本文件覆盖：
//     :29-35   TSockaddr / pTSockaddr 声明（类型落在 GateShareContainers.cs）
//     :78-88   TAddressInfo 声明（同上）
//     :211-215 TIPSection 声明（同上）
//     :1805-1821 IsHexString
//     :1823-1837 StrToHexEx
//     :1839-1864 HexToStrEx
//     :2971-2977 ReverseBytes（implementation 段，非接口声明）
//     :2979-2985 IP2Long
//     :2987-2990 Long2IP
//
// 抽取/回读：全文用
//   $b=[IO.File]::ReadAllBytes('Source\RunGate\GateShare.pas')
//   $t=[Text.Encoding]::GetEncoding(936).GetString($b) -replace "`r`n","`n"
//   归一化行尾后按物理 LF 行号定位（原文混用 CRLF 与裸 LF，未归一时行号会漂移）。
//
// ── INET 语义（关键，容易移植错）────────────────────────────────────────────────────
//   原文用 Winsock 的 `inet_addr`/`inet_ntoa`（ws2_32）。
//   * `inet_addr('a.b.c.d')` 的**返回数值**是  a | b<<8 | c<<16 | d<<24（小端打包）。
//     原文自己给了真值（:2981-2982）：inet_addr('202.103.100.1') = 23357386 = $016467CA；
//       202 | 103<<8 | 100<<16 | 1<<24 = 202 + 26368 + 6553600 + 16777216 = 23357386 ✓
//   * `IP2Long` = `ReverseBytes(inet_addr(...))` → 得到**大端数值**（1.2.3.4 → $01020304），
//     这样数值大小顺序才与 IP 段比较一致（原文 :2983 的注释就是在解释这件事）。
//   * `Long2IP` = `inet_ntoa(ReverseBytes(IP))` → 与 IP2Long 互逆。
//   * 托管侧不 P/Invoke ws2_32（RunGate 的 socket 层已由 GatewayKit 的 SAEA/IOCP 覆盖），
//     用等价的纯托管实现；**已知差异**：Windows `inet_addr` 还接受 1/2/3 段简写
//     （'1.2.3'、'0x01020304'、'0177.1' 等），本实现只支持标准 4 段十进制。
//     原文的全部调用点传入的都是 `IsIpaddr`/Trim 后的完整 IP 或完整 IP 段，故不触发差异；
//     差异已在 GateShareAddressTests.InetAddr_ShorthandForms_DifferFromWinsock 登记。
// =====================================================================================

using System;
using System.Globalization;
using System.Text;

namespace GXX.RunGate;

/// <summary>GateShare.pas 的 inet_addr/inet_ntoa 等价实现 + IP 数值转换。</summary>
public static class GateShareInet
{
    /// <summary>WinSock `INADDR_NONE`（$FFFFFFFF）。原文用**无类型常量**，Delphi 定型为 Cardinal。</summary>
    public const uint INADDR_NONE = 0xFFFFFFFFu;

    /// <summary>WinSock `INADDR_ANY`（$00000000）——原文未直接用，登记备查。</summary>
    public const uint INADDR_ANY = 0x00000000u;

    /// <summary>
    /// `inet_addr(PChar(s))` 的托管等价物：返回**小端打包**值（a | b&lt;&lt;8 | c&lt;&lt;16 | d&lt;&lt;24）。
    /// 失败返回 <see cref="INADDR_NONE"/>（与 Winsock 一致）。
    /// </summary>
    public static uint InetAddr(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return INADDR_NONE;

        // Windows inet_addr 允许首尾空白；原文调用前一般都 Trim 过，这里保持一致地宽容处理。
        string s = ip.Trim();
        if (s.Length == 0) return INADDR_NONE;

        string[] parts = s.Split('.');
        if (parts.Length != 4) return INADDR_NONE;   // 见文件头「已知差异」：不支持 1/2/3 段简写

        uint[] octets = new uint[4];
        for (int i = 0; i < 4; i++)
        {
            string p = parts[i];
            if (p.Length == 0) return INADDR_NONE;
            // 只接受十进制数字（Winsock 还接受 0x / 0 前缀，本实现不收，见文件头）
            foreach (char c in p)
                if (c < '0' || c > '9') return INADDR_NONE;

            if (!uint.TryParse(p, NumberStyles.None, CultureInfo.InvariantCulture, out uint v))
                return INADDR_NONE;
            if (v > 255) return INADDR_NONE;
            octets[i] = v;
        }

        return octets[0] | (octets[1] << 8) | (octets[2] << 16) | (octets[3] << 24);
    }

    /// <summary>`inet_ntoa(TInAddr(v))` 的托管等价物：把**小端打包**值还原成 'a.b.c.d'。</summary>
    public static string InetNtoa(uint v)
    {
        var sb = new StringBuilder(15);
        sb.Append(v & 0xFF).Append('.');
        sb.Append((v >> 8) & 0xFF).Append('.');
        sb.Append((v >> 16) & 0xFF).Append('.');
        sb.Append((v >> 24) & 0xFF);
        return sb.ToString();
    }

    /// <summary>原文 :2971-2977 `function ReverseBytes(Value: LongWord): LongWord;`（实现段，非接口声明）。
    /// 注释里说的 `htonl == ReverseBytes` 成立。</summary>
    public static uint ReverseBytes(uint Value)
        => ((Value & 0x000000FFu) << 24)
         | ((Value >> 8 & 0x000000FFu) << 16)
         | ((Value >> 16 & 0x000000FFu) << 8)
         | (Value >> 24 & 0x000000FFu);

    /// <summary>原文 :2979-2985 `function IP2Long(sIPaddr: string): LongWord;`
    /// = `ReverseBytes(inet_addr(...))` → 大端数值（便于做 IP 段区间比较）。</summary>
    public static uint IP2Long(string sIPaddr) => ReverseBytes(InetAddr(sIPaddr));

    /// <summary>原文 :2987-2990 `function Long2IP(IP: LongWord): string;`
    /// = `StrPas(inet_ntoa(TInAddr(ReverseBytes(IP))))`。</summary>
    public static string Long2IP(uint IP) => InetNtoa(ReverseBytes(IP));

    /// <summary>原文 :1805-1821 `function IsHexString(S: string): Boolean;`
    /// 空串 → False；全部字符 ∈ [0-9A-Fa-f] → True。</summary>
    public static bool IsHexString(string S)
    {
        bool Result = false;                                  // 原 :1809
        if (S == null || S.Length == 0) return false;         // 原 :1810 `if Length(S) = 0 then Exit;`

        for (int I = 0; I < S.Length; I++)                    // 原 :1812（Delphi 1-based）
        {
            char c = S[I];                                    // 原 :1814 `S[I] in ['0'..'9', 'A'..'F', 'a'..'f']`
            bool ok = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
            if (!ok) return false;                            // 原 :1816 `Exit`
        }
        return true;                                          // 原 :1820
    }

    /// <summary>原文 :1823-1837 `function StrToHexEx(S: AnsiString): AnsiString;`
    /// 逐**字节**输出 2 位大写十六进制（Digits 表 '0'..'F'）。
    ///
    /// ★ 原文缺陷（照抄语义但不复制 hang）：原文 `I: Byte`（:1825）配 `for I := 1 to Length(S)`（:1832）——
    ///   当 `Length(S) &gt; 255` 时 Byte 计数器回绕，Delphi 侧会**死循环**。托管侧用 int 计数器，
    ///   `StrToHexEx_LengthAbove255_DoesNotHang` 用差异断言固定这一点。
    ///   （原文 [1] 起索引；托管侧按字节处理，语义等价于对 GBK 编码字节做 hex。）</summary>
    public static string StrToHexEx(string S)
    {
        const string Digits = "0123456789ABCDEF";             // 原 :1828-1829
        var Result = new StringBuilder();
        if (S == null) return "";                             // 原 :1832 空串 → 空结果
        foreach (char ch in S)
        {
            // 原 :1834 `C := Ord(S[I]);` —— AnsiString 下标取的是**字节**。
            // 托管侧 string 是 UTF-16；调用方（RunGate）只传 GBK 单字节域字符串，
            // 故对 >0xFF 的字符按低 8 位截断，与 AnsiString 的字节语义对齐并登记差异。
            byte C = ch <= 0xFF ? (byte)ch : (byte)(ch & 0xFF);
            Result.Append(Digits[(C >> 4) & 0x0F]).Append(Digits[C & 0x0F]);   // 原 :1835
        }
        return Result.ToString();
    }

    /// <summary>原文 :1839-1864 `function HexToStrEx(const StrHex: AnsiString; var OutStr: AnsiString): Boolean;`
    /// 奇数长度 → False；含非 hex 字符 → False（**OutStr 保持原值**，因为 `OutStr := TempStr` 只在末尾 :1862）。
    /// 成功时每个字节 = StrToInt('$'+C1+C2)。</summary>
    public static bool HexToStrEx(string StrHex, ref string OutStr)
    {
        bool Result = false;                                                   // 原 :1845
        if (StrHex == null || (StrHex.Length % 2) != 0) return false;           // 原 :1846

        int Len = StrHex.Length / 2;                                           // 原 :1848
        var TempStr = new byte[Len];
        for (int I = 1; I <= Len; I++)                                         // 原 :1850
        {
            char C1 = StrHex[I * 2 - 2];                                       // 原 :1852 `StrHex[I*2-1]`
            char C2 = StrHex[I * 2 - 1];                                       // 原 :1853 `StrHex[I*2]`
            if (!IsHexDigit(C1) || !IsHexDigit(C2)) return false;              // 原 :1854-1859 `Exit`
            TempStr[I - 1] = (byte)Convert.ToInt32(new string(new[] { C1, C2 }), 16);   // 原 :1856 `StrToInt('$'+C1+C2)`
        }

        // 原 :1862-1863（AnsiString → 托管 string 时按 Latin-1 还原字节）
        var sb = new StringBuilder(Len);
        foreach (byte b in TempStr) sb.Append((char)b);
        OutStr = sb.ToString();
        return true;
    }

    private static bool IsHexDigit(char c)
        => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
}
