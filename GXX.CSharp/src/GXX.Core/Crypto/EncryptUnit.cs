using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.Core.Crypto;

/// <summary>
/// EncryptUnit.pas 1:1 转换：DES + Base64 + 6Bit 组合加解密入口。
/// </summary>
public static class EncryptUnit
{
    public const int EncryKey = 20120101;

    private static readonly byte[] B64Table =
    {
        65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
        81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108,
        109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 60, 61, 62, 64, 91, 92, 93
    };

    private static readonly byte[] Random55 =
    {
        50, 51, 52, 53, 54, 55, 56, 57,
        65, 66, 67, 68, 69, 70, 71, 72, 74, 75, 76, 77, 78, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90,
        97, 98, 99, 100, 101, 102, 103, 104, 106, 107, 109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122
    };

    private static readonly Random Rnd = new();

    public static string GetRandomValue(int value)
    {
        var chars = new char[4];
        chars[0] = (char)Random55[DelphiRTL.LoByte(DelphiRTL.LoWord(value))];
        chars[1] = (char)Random55[DelphiRTL.HiByte(DelphiRTL.LoWord(value))];
        chars[2] = (char)Random55[DelphiRTL.LoByte(DelphiRTL.HiWord(value))];
        chars[3] = (char)Random55[DelphiRTL.HiByte(DelphiRTL.HiWord(value))];
        return new string(chars);
    }

    public static int GetRandomValue()
        => DelphiRTL.MakeLong(DelphiRTL.MakeWord(Rnd.Next(55), Rnd.Next(55)), DelphiRTL.MakeWord(Rnd.Next(55), Rnd.Next(55)));

    public static int GetKeyValue()
        => DelphiRTL.MakeLong(DelphiRTL.MakeWord(Rnd.Next(58), Rnd.Next(58)), DelphiRTL.MakeWord(Rnd.Next(58), Rnd.Next(58)));

    public static string GetKeyValue(int value)
    {
        var chars = new char[4];
        chars[0] = (char)B64Table[DelphiRTL.LoByte(DelphiRTL.LoWord(value))];
        chars[1] = (char)B64Table[DelphiRTL.HiByte(DelphiRTL.LoWord(value))];
        chars[2] = (char)B64Table[DelphiRTL.LoByte(DelphiRTL.HiWord(value))];
        chars[3] = (char)B64Table[DelphiRTL.HiByte(DelphiRTL.HiWord(value))];
        return new string(chars);
    }

    public static string DecryScript(string str)
        => UnitDes.DecryptStrDesText(str, DelphiRTL.IntToStr(EncryKey));

    public static string EncryScript(string str)
        => UnitDes.EncryptStrDesText(str, DelphiRTL.IntToStr(EncryKey));

    /// <summary>EncryStrHex：字符串→HEX（GBK 字节）。</summary>
    public static string EncryStrHex(string str)
    {
        byte[] data = EncodingInit.GBK.GetBytes(str ?? "");
        var sb = new System.Text.StringBuilder(data.Length * 2);
        foreach (byte b in data)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static string DecryStrHex(string strHex)
    {
        byte[] data = new byte[strHex.Length / 2];
        for (int i = 0; i < data.Length; i++)
        {
            string temp = DelphiRTL.Copy(strHex, i * 2 + 1, 2);
            data[i] = (byte)HUtil32Of(temp);
        }
        return EncodingInit.GBK.GetString(data);
    }

    private static int HUtil32Of(string hex)
    {
        int res = 0;
        foreach (char ch in hex)
        {
            if (ch >= '0' && ch <= '9') res = res * 16 + ch - '0';
            else if (ch >= 'A' && ch <= 'F') res = res * 16 + ch - 'A' + 10;
            else if (ch >= 'a' && ch <= 'f') res = res * 16 + ch - 'a' + 10;
            else throw new Exception("Error: not a Hex String");
        }
        return res;
    }

    /// <summary>EncryString：随机 4 字符 Key 前缀 + DES 加密 + Base64 + 6Bit。</summary>
    public static string EncryString(string str)
    {
        if (str == "") return "";
        byte[] data = EncodingInit.GBK.GetBytes(str);
        return EncryBuffer(data, data.Length);
    }

    public static string DecryString(string str)
    {
        if (str.Length <= 4) return "";
        string key = str.Substring(0, 2) + str.Substring(str.Length - 2, 2);
        string body = DelphiRTL.Copy(str, 3, str.Length - 4);
        byte[] decoded = EDcode.DecodeString(EncodingInit.GBK.GetBytes(body));
        byte[] b64 = Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded));
        byte[] plain = UnitDes.DecryptStrDes(b64, key);
        return EncodingInit.GBK.GetString(plain);
    }

    public static string EncryBuffer(byte[] buf, int bufsize)
    {
        if (bufsize <= 0) return "";
        var keyChars = new char[4];
        for (int i = 0; i < 4; i++)
            keyChars[i] = (char)B64Table[Rnd.Next(58)];
        string key = new string(keyChars);
        byte[] sText = new byte[bufsize];
        UnitDes.EncryptDes(buf, sText, bufsize, key);
        string b64 = Base64Util.Base64EncodeStr(sText);
        string enc = EncodingInit.GBK.GetString(EDcode.EncodeString(b64));
        return keyChars[0].ToString() + keyChars[1] + enc + keyChars[2] + keyChars[3];
    }

    public static void DecryBuffer(string src, byte[] buf, int bufsize)
    {
        if (src.Length <= 4) return;
        string key = src.Substring(0, 2) + src.Substring(src.Length - 2, 2);
        string body = DelphiRTL.Copy(src, 3, src.Length - 4);
        byte[] decoded = EDcode.DecodeString(EncodingInit.GBK.GetBytes(body));
        byte[] sText = Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded));
        UnitDes.DecryptDes(sText, buf, Math.Min(bufsize, sText.Length), key);
    }

    // ---- K 系列（固定 Key）----

    public static string EncryBufferK(byte[] buf, int bufsize, string key)
    {
        if (bufsize <= 0) return "";
        byte[] sText = new byte[bufsize];
        UnitDes.EncryptDes(buf, sText, bufsize, key);
        string b64 = Base64Util.Base64EncodeStr(sText);
        return EncodingInit.GBK.GetString(EDcode.EncodeString(b64));
    }

    public static void DecryBufferK(string src, byte[] buf, int bufsize, string key)
    {
        if (src.Length == 0) return;
        byte[] decoded = EDcode.DecodeStringK(EncodingInit.GBK.GetBytes(src));
        string sText = EncodingInit.GBK.GetString(Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded)));
        byte[] cipher = EncodingInit.GBK.GetBytes(sText);
        UnitDes.DecryptDes(cipher, buf, Math.Min(bufsize, cipher.Length), key);
    }

    public static string EncryStringK(string src, string key)
    {
        if (src == "") return "";
        byte[] enc = UnitDes.EncryptStrDesBytes(EncodingInit.GBK.GetBytes(src), key);
        string b64 = Base64Util.Base64EncodeStr(enc);
        return EncodingInit.GBK.GetString(EDcode.EncodeString(b64));
    }

    public static string DecryStringK(string src, string key)
    {
        if (src == "") return "";
        byte[] decoded = EDcode.DecodeStringK(EncodingInit.GBK.GetBytes(src));
        byte[] b64 = Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded));
        byte[] plain = UnitDes.DecryptStrDesBytes(b64, key);
        return EncodingInit.GBK.GetString(plain);
    }

    public static string zEncryStringK(string src, string key)
    {
        if (src == "") return "";
        byte[] enc = UnitDes.EncryptStrDesBytes(EncodingInit.GBK.GetBytes(src), key);
        string b64 = Base64Util.Base64EncodeStr(enc);
        return EncodingInit.GBK.GetString(EDcode.zEncodeString(b64));
    }

    public static string zDecryStringK(string src, string key)
    {
        if (src == "") return "";
        byte[] decoded = EDcode.zDecodeString(EncodingInit.GBK.GetBytes(src));
        byte[] b64 = Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded));
        byte[] plain = UnitDes.DecryptStrDes(b64, key);
        return EncodingInit.GBK.GetString(plain);
    }

    public static string zEncryBufferK(byte[] buf, int bufsize, string key)
    {
        if (bufsize <= 0) return "";
        byte[] compressed = Compress.ZlibEx.CompressBuf(buf, bufsize);
        if (compressed == null || compressed.Length == 0) return "";
        byte[] sText = new byte[compressed.Length];
        UnitDes.EncryptDes(compressed, sText, compressed.Length, key);
        string b64 = Base64Util.Base64EncodeStr(sText);
        return EncodingInit.GBK.GetString(EDcode.EncodeStringK(b64));
    }

    public static void zDecryBufferK(string src, byte[] buf, int bufsize, string key)
    {
        if (src.Length == 0) return;
        byte[] decoded = EDcode.DecodeString(EncodingInit.GBK.GetBytes(src));
        string sText = EncodingInit.GBK.GetString(Base64Util.Base64DecodeStr(EncodingInit.GBK.GetString(decoded)));
        byte[] cipher = EncodingInit.GBK.GetBytes(sText);
        byte[] sTemp = new byte[cipher.Length];
        UnitDes.DecryptDes(cipher, sTemp, cipher.Length, key);
        byte[] plain = Compress.ZlibEx.DecompressBuf(sTemp, sTemp.Length);
        if (plain != null && plain.Length > 0)
        {
            int n = Math.Min(plain.Length, bufsize);
            Array.Copy(plain, 0, buf, 0, n);
        }
    }

    // ---- A 系列（默认 EncryKey）----

    public static byte[] EncryBufferA(byte[] buf, int bufsize)
    {
        byte[] result = new byte[bufsize];
        UnitDes.EncryptDes(buf, result, bufsize, DelphiRTL.IntToStr(EncryKey));
        return result;
    }

    public static void EncryBufferA(byte[] buf, int bufsize, byte[] outBuf)
        => UnitDes.EncryptDes(buf, outBuf, bufsize, DelphiRTL.IntToStr(EncryKey));

    public static void DecryBufferA(string src, byte[] buf, int bufsize)
    {
        byte[] cipher = EncodingInit.GBK.GetBytes(src);
        UnitDes.DecryptDes(cipher, buf, Math.Min(bufsize, cipher.Length), DelphiRTL.IntToStr(EncryKey));
    }

    public static byte[] DecryBufferA(byte[] buf, int bufsize)
    {
        byte[] result = new byte[bufsize];
        UnitDes.DecryptDes(buf, result, bufsize, DelphiRTL.IntToStr(EncryKey));
        return result;
    }

    public static string DecryBufferA(string src)
        => UnitDes.DecryptStrDesText(src, DelphiRTL.IntToStr(EncryKey));

    public static void DecryBufferA(byte[] source, int sourceSize, byte[] outdata)
        => UnitDes.DecryptDes(source, outdata, sourceSize, DelphiRTL.IntToStr(EncryKey));

    /// <summary>客户端参数解密（Client.dpr 用）：zDecryBufferK 的结构体便捷版。</summary>
    public static byte[] zDecryStruct(string src, int structSize, string key)
    {
        byte[] buf = new byte[structSize];
        zDecryBufferK(src, buf, structSize, key);
        return buf;
    }
}
