using System;
using GXX.Core.Crypto;
using GXX.Core.Protocol;

namespace GXX.RunGate;

/// <summary>
/// EncryptUnit_LF.pas 1:1 转换（Source\RunGate\EncryptUnit_LF.pas，156 行 / ReadAllLines 186 行）。
///
/// 与 Source\Common\EncryptUnit_LF.pas 的差异（原文逐字比对，以 RunGate 版为准）：
///   RunGate 版 **没有** 使用 DesNew2 的 DecryptDes_New/EncryptDes_New 变体
///   （Common 版在 DecryScript_LF/EncryScript_LF 里用了），其余函数逐字相同。
///
/// 保真要点：
///  1) 密钥统一为 `IntToStr(NewEncryKey^)`；NewEncryKey 在 UnitDes.pas:1109 的
///     initialization 里赋值 `$0C08BE531` —— 注意该字面量有 9 位十六进制，
///     Delphi 7 对超出 32 位的整数字面量按 LongWord 截断，实际值是 **$C08BE531 = 3230393649**。
///     因此密钥字符串是十进制 "3230393649"（不是 "20120101"）。GXX.Core.EncryptUnit.EncryKey
///     是另一套 A 系列常量，切勿混用。
///  2) `EncryString_LF` 直接调用 EncryBuffer_LF（**不** 预处理 6Bit 的 EncodeString）；
///     而 `DecryString_LF` 会 DecodeString + Base64DecodeStr —— 与 EncryString_LF 不对称，
///     这是原文现状（照抄，不"修正"）。
///  3) `DecryBuffer_LF` 无长度校验：直接把 Base64 解出的全量缓冲按 Bufsize 解密，
///     由 UnitDes.DecryptDes 内部的 size 参数控制写入量。
///  4) `EncryStringHex_LF` 的 `Format('%x',[Ord(...)])` 对 &lt; 16 的值补前导 '0'，
///     因此输出恒为每字节 2 位小写十六进制。
///  5) UB: `SetLength(Result, Bufsize)` 后 `Result[1]` 取址 —— Bufsize = 0 时 Result 为空串，
///     C# 侧对应返回空数组。
/// 未移植依赖：无（UnitDes / Base64 / EDcode 均已在 GXX.Core 就位）。
/// </summary>
public static class EncryptUnitLf
{
    /// <summary>
    /// UnitDes.pas:1109 `NewEncryKey^ := $0C08BE531` 截断到 LongWord 后的值。
    /// 用于 `IntToStr(NewEncryKey^)` 生成 DES 密钥字符串。
    /// </summary>
    public const uint NewEncryKey = 0xC08BE531u;

    /// <summary>密钥字符串：Delphi `IntToStr(NewEncryKey^)`（无符号十进制）。</summary>
    public static string NewEncryKeyStr => NewEncryKey.ToString();

    // ---------------- Script 系列（原 33-41） ----------------

    /// <summary>原 EncryptUnit_LF.pas:33-36 DecryScript_LF。</summary>
    public static string DecryScript_LF(string str)
        => UnitDes.DecryptStrDesText(str, NewEncryKeyStr);

    /// <summary>原 EncryptUnit_LF.pas:38-41 EncryScript_LF。</summary>
    public static string EncryScript_LF(string str)
        => UnitDes.EncryptStrDesText(str, NewEncryKeyStr);

    // ---------------- String 系列（原 43-59） ----------------

    /// <summary>原 EncryptUnit_LF.pas:43-51 DecryString_LF（DecodeString → Base64DecodeStr → DES）。</summary>
    public static string DecryString_LF(string str)
    {
        if (str != "")
        {
            byte[] decoded = EDcode.DecodeString(GBK(str));
            byte[] cipher = Base64Util.Base64DecodeStr(GBKString(decoded));
            return GBKString(UnitDes.DecryptStrDesBytes(cipher, NewEncryKeyStr));
        }
        return "";
    }

    /// <summary>原 EncryptUnit_LF.pas:53-59 EncryString_LF（**不**做 6Bit 预处理，与 DecryString_LF 不对称）。</summary>
    public static string EncryString_LF(string str)
    {
        if (str != "")
            return EncryBuffer_LF(GBK(str), GBK(str).Length);
        return "";
    }

    // ---------------- Hex 系列（原 61-124） ----------------

    /// <summary>原 EncryptUnit_LF.pas:61-99 DecryStringHex_LF（HEX 文本 → 字节 → Base64 解码 → DES）。</summary>
    public static string DecryStringHex_LF(string src)
    {
        if (src != "")
        {
            // 原文内嵌 HexToInt：逐字符 x16，非法字符被忽略（注释掉了 raise）
            int hexToInt(string hex)
            {
                int res = 0;
                for (int i = 0; i < hex.Length; i++)
                {
                    char ch = hex[i];
                    if (ch >= '0' && ch <= '9') res = res * 16 + ch - '0';
                    else if (ch >= 'A' && ch <= 'F') res = res * 16 + ch - 'A' + 10;
                    else if (ch >= 'a' && ch <= 'f') res = res * 16 + ch - 'a' + 10;
                    // else 原文注释掉 raise，静默按 0 计（不改变 res）
                }
                return res;
            }

            var bytes = new System.Collections.Generic.List<byte>();
            for (int i = 0; i <= src.Length / 2 - 1; i++)
                bytes.Add((byte)hexToInt(DelphiCopy(src, i * 2 + 1, 2)));

            byte[] cipher = Base64Util.Base64DecodeStr(ParadoxConv.BytesToLatin1(bytes.ToArray()));
            return GBKString(UnitDes.DecryptStrDesBytes(cipher, NewEncryKeyStr));
        }
        return "";
    }

    /// <summary>原 EncryptUnit_LF.pas:101-124 EncryStringHex_LF（DES → Base64 → 每字节 2 位小写 HEX）。</summary>
    public static string EncryStringHex_LF(string src)
    {
        int bufsize = GBK(src).Length;
        if (bufsize > 0)
        {
            byte[] sText = new byte[bufsize];
            UnitDes.EncryptDes(GBK(src), sText, bufsize, NewEncryKeyStr);
            string tempResult = Base64Util.Base64EncodeStr(sText);
            var sb = new System.Text.StringBuilder(tempResult.Length * 2);
            foreach (char ch in tempResult)
            {
                // 原文 Format('%x',[Ord(ch)])：Base64 字符集全为 ASCII(<128)，%x 不补零
                // 但代码里 `if Length(temp) = 1 then temp := '0' + temp;` 会补成两位
                sb.Append(((int)ch).ToString("x2"));
            }
            return sb.ToString();
        }
        return "";
    }

    // ---------------- Buffer 系列（原 126-149） ----------------

    /// <summary>原 EncryptUnit_LF.pas:126-138 EncryBuffer_LF（DES → Base64 → 6Bit EncodeString）。</summary>
    public static string EncryBuffer_LF(byte[] buf, int bufsize)
    {
        if (bufsize > 0)
        {
            byte[] sText = new byte[bufsize];
            UnitDes.EncryptDes(buf, sText, bufsize, NewEncryKeyStr);
            string b64 = Base64Util.Base64EncodeStr(sText);
            return GBKString(EDcode.EncodeString(b64));
        }
        return "";
    }

    /// <summary>原 EncryptUnit_LF.pas:126-138 EncryBuffer_LF（string 重载，按 GBK 取字节）。</summary>
    public static string EncryBuffer_LF(string buf, int bufsize)
        => EncryBuffer_LF(GBK(buf), bufsize);

    /// <summary>原 EncryptUnit_LF.pas:140-149 DecryBuffer_LF（DecodeString → Base64DecodeStr → DES → Buf）。</summary>
    public static void DecryBuffer_LF(string src, byte[] buf, int bufsize)
    {
        if (src != "")
        {
            // 原注释：// DecodeString    DecryStrHex
            byte[] decoded = EDcode.DecodeString(GBK(src));
            byte[] sText = Base64Util.Base64DecodeStr(GBKString(decoded));
            // 原文 DecryptDes(sText[1], Buf^, Bufsize) —— 无长度上限检查，
            // 由 DecryptDes 内部 size 控制；此处同样以 bufsize 为 size，并做托管侧越界保护。
            int size = Math.Min(bufsize, sText.Length);
            if (size <= 0) return;
            var tmp = new byte[sText.Length];
            UnitDes.DecryptDes(sText, tmp, sText.Length, NewEncryKeyStr);
            Array.Copy(tmp, 0, buf, 0, size);
        }
    }

    // ---------------- BufferA 系列（原 152-182） ----------------

    /// <summary>原 EncryptUnit_LF.pas:152-156 EncryBufferA_LF（返回 Bufsize 字节密文）。</summary>
    public static byte[] EncryBufferA_LF(byte[] buf, int bufsize)
    {
        var result = new byte[bufsize];
        UnitDes.EncryptDes(buf, result, bufsize, NewEncryKeyStr);
        return result;
    }

    /// <summary>原 EncryptUnit_LF.pas:158-161 EncryBufferA_LF（写入 OutBuf）。</summary>
    public static void EncryBufferA_LF(byte[] buf, int bufsize, byte[] outBuf)
        => UnitDes.EncryptDes(buf, outBuf, bufsize, NewEncryKeyStr);

    /// <summary>原 EncryptUnit_LF.pas:163-166 DecryBufferA_LF（Src 字符串按 AnsiString 取字节）。</summary>
    public static void DecryBufferA_LF(string src, byte[] buf, int bufsize)
    {
        byte[] cipher = GBK(src);
        int size = Math.Min(bufsize, cipher.Length);
        if (size <= 0) return;
        var tmp = new byte[cipher.Length];
        UnitDes.DecryptDes(cipher, tmp, cipher.Length, NewEncryKeyStr);
        Array.Copy(tmp, 0, buf, 0, size);
    }

    /// <summary>原 EncryptUnit_LF.pas:168-172 DecryBufferA_LF（Buf 字节版）。</summary>
    public static byte[] DecryBufferA_LF(byte[] buf, int bufsize)
    {
        var result = new byte[bufsize];
        UnitDes.DecryptDes(buf, result, bufsize, NewEncryKeyStr);
        return result;
    }

    /// <summary>原 EncryptUnit_LF.pas:174-177 DecryBufferA_LF（Src 字符串 → DecryptStrDes）。</summary>
    public static string DecryBufferA_LF(string src)
        => UnitDes.DecryptStrDesText(src, NewEncryKeyStr);

    /// <summary>原 EncryptUnit_LF.pas:179-182 DecryBufferA_LF（Source → Outdata）。</summary>
    public static void DecryBufferA_LF(byte[] source, int sourceSize, byte[] outdata)
        => UnitDes.DecryptDes(source, outdata, sourceSize, NewEncryKeyStr);

    // ---------------- 助手 ----------------

    private static byte[] GBK(string s) => GXX.Core.EncodingInit.GBK.GetBytes(s ?? "");

    private static string GBKString(byte[] b) => b == null ? "" : GXX.Core.EncodingInit.GBK.GetString(b);

    /// <summary>Delphi Copy(S, Index, Count)（1-based；越界返回空）。</summary>
    private static string DelphiCopy(string s, int index, int count)
    {
        if (string.IsNullOrEmpty(s) || index > s.Length || index < 1 || count <= 0) return "";
        int avail = s.Length - (index - 1);
        if (count > avail) count = avail;
        return s.Substring(index - 1, count);
    }
}
