using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Util;

namespace GXX.RunGate;

/// <summary>ParadoxConv.pas:43-47 eConvException（转换失败异常）。</summary>
public class EConvException : Exception
{
    public string SrcEncoding;
    public string DstEncoding;
    public string Symbol;

    public EConvException(string message) : base(message) { }

    public static EConvException Create(string source, string dest, string sym)
    {
        // 原 ParadoxConv.pas:71-80
        return new EConvException(
            DelphiFormatString("Can't convert from %S to %S symbol %S", source, dest, sym))
        {
            SrcEncoding = source,
            DstEncoding = dest,
            Symbol = sym
        };
    }

    private static string DelphiFormatString(string fmt, params string[] args)
        => string.Format(fmt.Replace("%S", "{0}"), args);
}

/// <summary>ParadoxConv.pas:49 TEncoding（Delphi 枚举顺序/PascalCase 保留）。</summary>
public enum TEncodingKind
{
    UCS4 = 0,
    UTF8 = 1,
    KOI8R = 2,
    ISO88595 = 3,
    CP1251 = 4,
    CP866 = 5
}

/// <summary>
/// ParadoxConv.pas 1:1 转换（Source\RunGate\ParadoxConv.pas，1049 行）。
/// 该单元是"UCS4(4 字节大端) ↔ 各 8 位代码页"的转换器，供 GameCenter 的
/// Paradox/GBDE → SQLite 迁移工具使用（见转换开发文档 §4.6）。
///
/// 保真要点（全部照抄原文，含原文缺陷）：
///  1) 所有 UCS4 形态都是 **4 字节大端** 的 AnsiChar 序列，字节运算逐字节进行；
///     托管侧以 `byte[]` 为原始载体（避免 GBK 解码破坏任意字节），
///     另提供 Latin-1 透明映射的 string 重载（Delphi AnsiString 语义）。
///  2) 长度校验只有 `(Length(S) &lt; 4) or ((Length(S) mod 4) &lt;&gt; 0)` 时抛异常；
///     **不做 4 对齐以外的合法性检查**。
///  3) UCS4ToCp1251/UCS4ToKoi8r/UCS4ToISO88595/UCS4ToCp866 只接受 `S[I] = #0 and S[I+1] = #0`
///     （即 BMP 且高两字节为 0），否则抛 eConvException。
///  4) Cp1251ToUCS4 与 Koi8rToUCS4 的 `case` **无 else 分支**：落到 case 外的字节
///     两个字符都不追加，静默吞掉（原文缺陷，照抄）。
///     Cp1251ToUCS4 的 else 分支只覆盖 `#$85..#$FF` 之外 —— 由于 `#$C0..#$FF`
///     在后面显式列出，实际只有字节 $98 会落空；ISO88595ToUCS4 全字节覆盖且无 else。
///  5) `Encoding()` 的 `case Source of` **没有 CP866 分支**（原文 155-164 被注释掉），
///     因此 Source=CP866 时一律返回空串（原文缺陷，照抄）。
///  6) 逐字节映射表由脚本从原文抽取生成，见 ParadoxConv.Tables.g.cs。
/// 未移植依赖：无。
/// </summary>
public static partial class ParadoxConv
{
    /// <summary>恒等标记：原文 `Result := Result + S[I + 3]`（原样输出低字节）。</summary>
    private const byte Identity = 0;

    /// <summary>ParadoxConv.pas:82-99 GetCodepage（WINDOWS 分支：'CP' + GetACP）。</summary>
    public static string GetCodepage()
    {
        // 原文 {$IFDEF WINDOWS} Result := 'CP' + IntToStr(GetACP); {$ENDIF}
        return "CP" + GetACP();
    }

    /// <summary>Windows GetACP()（当前系统 ANSI 代码页）。</summary>
    public static int GetACP()
    {
        try
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
        }
        catch
        {
            return 1252;
        }
    }

    /// <summary>ParadoxConv.pas:101-166 Encoding（字节版）。</summary>
    public static byte[] Encoding(TEncodingKind source, TEncodingKind dest, byte[] s)
    {
        byte[] result = Array.Empty<byte>();
        switch (source)
        {
            case TEncodingKind.UCS4:
                switch (dest)
                {
                    case TEncodingKind.UTF8: result = UCS4ToUtf8(s); break;
                    case TEncodingKind.KOI8R: result = UCS4ToKoi8r(s); break;
                    case TEncodingKind.ISO88595: result = UCS4ToISO88595(s); break;
                    case TEncodingKind.CP1251: result = UCS4ToCp1251(s); break;
                    case TEncodingKind.CP866: result = UCS4ToCp866(s); break;
                }
                break;
            case TEncodingKind.UTF8:
                switch (dest)
                {
                    case TEncodingKind.UCS4: result = Utf8ToUCS4(s); break;
                    case TEncodingKind.KOI8R: result = UCS4ToKoi8r(Utf8ToUCS4(s)); break;
                    case TEncodingKind.ISO88595: result = UCS4ToISO88595(Utf8ToUCS4(s)); break;
                    case TEncodingKind.CP1251: result = UCS4ToCp1251(Utf8ToUCS4(s)); break;
                    case TEncodingKind.CP866: result = UCS4ToCp866(Utf8ToUCS4(s)); break;
                }
                break;
            case TEncodingKind.KOI8R:
                switch (dest)
                {
                    case TEncodingKind.UTF8: result = UCS4ToUtf8(Koi8rToUCS4(s)); break;
                    case TEncodingKind.UCS4: result = Koi8rToUCS4(s); break;
                    case TEncodingKind.ISO88595: result = UCS4ToISO88595(Koi8rToUCS4(s)); break;
                    case TEncodingKind.CP1251: result = UCS4ToCp1251(Koi8rToUCS4(s)); break;
                    case TEncodingKind.CP866: result = UCS4ToCp866(Koi8rToUCS4(s)); break;
                }
                break;
            case TEncodingKind.ISO88595:
                switch (dest)
                {
                    case TEncodingKind.UTF8: result = UCS4ToUtf8(ISO88595ToUCS4(s)); break;
                    case TEncodingKind.KOI8R: result = UCS4ToKoi8r(ISO88595ToUCS4(s)); break;
                    case TEncodingKind.UCS4: result = ISO88595ToUCS4(s); break;
                    case TEncodingKind.CP1251: result = UCS4ToCp1251(ISO88595ToUCS4(s)); break;
                    case TEncodingKind.CP866: result = UCS4ToCp866(ISO88595ToUCS4(s)); break;
                }
                break;
            case TEncodingKind.CP1251:
                switch (dest)
                {
                    case TEncodingKind.UTF8: result = UCS4ToUtf8(Cp1251ToUCS4(s)); break;
                    case TEncodingKind.KOI8R: result = UCS4ToKoi8r(Cp1251ToUCS4(s)); break;
                    case TEncodingKind.ISO88595: result = UCS4ToISO88595(Cp1251ToUCS4(s)); break;
                    case TEncodingKind.UCS4: result = Cp1251ToUCS4(s); break;
                    case TEncodingKind.CP866: result = UCS4ToCp866(Cp1251ToUCS4(s)); break;
                }
                break;
            // 原文缺陷：CP866 作为 Source 的分支被整段注释掉（ParadoxConv.pas:155-164），
            // 因此 Source=CP866 时 Result 恒为 ''。此处照抄（不补分支）。
            case TEncodingKind.CP866:
                break;
        }
        return result;
    }

    /// <summary>ParadoxConv.pas:101-166 Encoding（Latin-1 透明 string 版，对应 Delphi AnsiString）。</summary>
    public static string Encoding(TEncodingKind source, TEncodingKind dest, string s)
        => BytesToLatin1(Encoding(source, dest, Latin1ToBytes(s)));

    // ---------------- UTF-8 ↔ UCS4 ----------------

    /// <summary>ParadoxConv.pas:168-213 Utf8ToUCS4。</summary>
    public static byte[] Utf8ToUCS4(byte[] s)
    {
        var result = new List<byte>();
        int i = 1;
        while (i <= s.Length)
        {
            byte b = s[i - 1];
            int count;
            if ((b & 0x80) == 0)
                count = 1;
            else
                count = 0;

            while ((b & 0x80) != 0)
            {
                count = count + 1;
                b = (byte)(b << 1);
            }

            if (count > 1)
                b = (byte)(b >> count);

            int value = 0;
            value = value | b;

            if ((s.Length - i) < (count - 1))
                throw EConvException.Create("Utf8", "UCS4", "");

            for (int ii = 1; ii <= count - 1; ii++)
            {
                if ((s[i + ii - 1] & 0xC0) != 0x80)
                    throw EConvException.Create("Utf8", "UCS4", "");
                value = (value << 6) | (s[i + ii - 1] & 0x3F);
            }

            result.Add((byte)((value >> 24) & 0xFF));
            result.Add((byte)((value >> 16) & 0xFF));
            result.Add((byte)((value >> 8) & 0xFF));
            result.Add((byte)(value & 0xFF));

            i = i + count;
        }
        return result.ToArray();
    }

    /// <summary>ParadoxConv.pas:215-295 UCS4ToUtf8。</summary>
    public static byte[] UCS4ToUtf8(byte[] s)
    {
        var result = new List<byte>();
        if (s.Length == 0) return result.ToArray();
        if (s.Length < 4 || (s.Length % 4) != 0)
            throw EConvException.Create("UCS4", "Utf8", "");

        int i = 1;
        while (i <= s.Length)
        {
            int value = 0;
            value = value | s[i - 1]; i++;
            value = (value << 8) | s[i - 1]; i++;
            value = (value << 8) | s[i - 1]; i++;
            value = (value << 8) | s[i - 1]; i++;

            int count;
            if (value < 0x80) count = 1;
            else if (value < 0x800) count = 2;
            else if (value < 0x10000) count = 3;
            else if (value < 0x200000) count = 4;
            else if (value < 0x4000000) count = 5;
            else count = 6;

            var strTemp = new List<byte>();
            if (count >= 6)
            {
                strTemp.Insert(0, (byte)(0x80 | (value & 0x3F)));
                value = (int)((uint)value >> 6);
                value = value | 0x4000000;
            }
            if (count >= 5)
            {
                strTemp.Insert(0, (byte)(0x80 | (value & 0x3F)));
                value = (int)((uint)value >> 6);
                value = value | 0x200000;
            }
            if (count >= 4)
            {
                strTemp.Insert(0, (byte)(0x80 | (value & 0x3F)));
                value = (int)((uint)value >> 6);
                value = value | 0x10000;
            }
            if (count >= 3)
            {
                strTemp.Insert(0, (byte)(0x80 | (value & 0x3F)));
                value = (int)((uint)value >> 6);
                value = value | 0x800;
            }
            if (count >= 2)
            {
                strTemp.Insert(0, (byte)(0x80 | (value & 0x3F)));
                value = (int)((uint)value >> 6);
                value = value | 0xC0;
            }
            strTemp.Insert(0, (byte)value);
            result.AddRange(strTemp);
        }
        return result.ToArray();
    }

    // ---------------- CP1251 ↔ UCS4 ----------------

    /// <summary>ParadoxConv.pas:297-376 Cp1251ToUCS4。
    /// 原文缺陷：`case S[I] of` **无 else 分支**，落到 case 外的字节（实测只有 $98）
    /// 会保留已追加的 `#0#0` 两个字节后什么都不做 —— 即静默输出 `#$00#$00`。此处照抄。</summary>
    public static byte[] Cp1251ToUCS4(byte[] s)
    {
        var result = new List<byte>();
        for (int i = 1; i <= s.Length; i++)
        {
            result.Add(0);
            result.Add(0);
            ushort v;
            if (Cp1251ToUcs4Table.TryGetValue(s[i - 1], out v))
            {
                result.Add((byte)((v >> 8) & 0xFF));
                result.Add((byte)(v & 0xFF));
            }
        }
        return result.ToArray();
    }

    /// <summary>ParadoxConv.pas:378-491 UCS4ToCp1251。</summary>
    public static byte[] UCS4ToCp1251(byte[] s)
    {
        var result = new List<byte>();
        if (s.Length == 0) return result.ToArray();
        if (s.Length < 4 || (s.Length % 4) != 0)
            throw EConvException.Create("UCS4", "Cp1251", "");

        int i = 1;
        while (i <= s.Length)
        {
            if (s[i - 1 + 0] != 0 || s[i - 1 + 1] != 0)
                throw EConvException.Create("UCS4", "Cp1251",
                    "$" + s[i - 1].ToString("X2") + s[i - 1 + 1].ToString("X2") + s[i - 1 + 2].ToString("X2") + s[i - 1 + 3].ToString("X2"));
            if (!TryLookup(Ucs4ToCp1251Table, s[i - 1 + 2], s[i - 1 + 3], s[i - 1 + 3], out byte outByte))
                throw EConvException.Create("UCS4", "Cp1251", "$" + s[i - 1].ToString("X2"));
            result.Add(outByte);
            i = i + 4;
        }
        return result.ToArray();
    }

    // ---------------- CP866 -> UCS4 的 Cp866ToUCS4 在原文被注释掉（ParadoxConv.pas:492-574），
    // 故此处不提供；UCS4ToCp866 保留。 ----------------

    /// <summary>ParadoxConv.pas:576-698 UCS4ToCp866。</summary>
    public static byte[] UCS4ToCp866(byte[] s)
    {
        var result = new List<byte>();
        if (s.Length == 0) return result.ToArray();
        if (s.Length < 4 || (s.Length % 4) != 0)
            throw EConvException.Create("UCS4", "Cp866", "");

        int i = 1;
        while (i <= s.Length)
        {
            if (s[i - 1 + 0] != 0 || s[i - 1 + 1] != 0)
                throw EConvException.Create("UCS4", "Cp866",
                    "$" + s[i - 1].ToString("X2") + s[i - 1 + 1].ToString("X2") + s[i - 1 + 2].ToString("X2") + s[i - 1 + 3].ToString("X2"));
            if (!TryLookup(Ucs4ToCp866Table, s[i - 1 + 2], s[i - 1 + 3], s[i - 1 + 3], out byte outByte))
                throw EConvException.Create("UCS4", "Cp866", "$" + s[i - 1].ToString("X2"));
            result.Add(outByte);
            i = i + 4;
        }
        return result.ToArray();
    }

    // ---------------- KOI8-R ↔ UCS4 ----------------

    /// <summary>ParadoxConv.pas:700-842 Koi8rToUCS4。</summary>
    public static byte[] Koi8rToUCS4(byte[] s)
    {
        var result = new List<byte>();
        for (int i = 1; i <= s.Length; i++)
        {
            result.Add(0);
            result.Add(0);
            ushort v;
            if (Koi8rToUcs4Table.TryGetValue(s[i - 1], out v))
            {
                result.Add((byte)((v >> 8) & 0xFF));
                result.Add((byte)(v & 0xFF));
            }
            // 原文 case 无 else 分支（256 字节全覆盖，未命中时静默输出 #0#0，照抄）
        }
        return result.ToArray();
    }

    /// <summary>ParadoxConv.pas:844-1028 UCS4ToKoi8r。</summary>
    public static byte[] UCS4ToKoi8r(byte[] s)
    {
        var result = new List<byte>();
        if (s.Length == 0) return result.ToArray();
        if (s.Length < 4 || (s.Length % 4) != 0)
            throw EConvException.Create("UCS4", "Koi8r", "");

        int i = 1;
        while (i <= s.Length)
        {
            if (s[i - 1 + 0] != 0 || s[i - 1 + 1] != 0)
                throw EConvException.Create("UCS4", "Koi8r",
                    "$" + s[i - 1].ToString("X2") + s[i - 1 + 1].ToString("X2") + s[i - 1 + 2].ToString("X2") + s[i - 1 + 3].ToString("X2"));
            if (!TryLookup(Ucs4ToKoi8rTable, s[i - 1 + 2], s[i - 1 + 3], s[i - 1 + 3], out byte outByte))
                throw EConvException.Create("UCS4", "Koi8r", "$" + s[i - 1].ToString("X2"));
            result.Add(outByte);
            i = i + 4;
        }
        return result.ToArray();
    }

    // ---------------- ISO 8859-5 ↔ UCS4 ----------------

    /// <summary>ParadoxConv.pas:1030-1050 ISO88595ToUCS4。</summary>
    public static byte[] ISO88595ToUCS4(byte[] s)
    {
        var result = new List<byte>();
        for (int i = 1; i <= s.Length; i++)
        {
            result.Add(0);
            result.Add(0);
            ushort v;
            // 原文 case 覆盖 #$00..#$FF 且无 else（全字节）
            Iso88595ToUcs4Table.TryGetValue(s[i - 1], out v);
            result.Add((byte)((v >> 8) & 0xFF));
            result.Add((byte)(v & 0xFF));
        }
        return result.ToArray();
    }

    /// <summary>ParadoxConv.pas:1052-1102 UCS4ToISO88595。</summary>
    public static byte[] UCS4ToISO88595(byte[] s)
    {
        var result = new List<byte>();
        if (s.Length == 0) return result.ToArray();
        if (s.Length < 4 || (s.Length % 4) != 0)
            throw EConvException.Create("UCS4", "ISO 8859-5", "");

        int i = 1;
        while (i <= s.Length)
        {
            if (s[i - 1 + 0] != 0 || s[i - 1 + 1] != 0)
                throw EConvException.Create("UCS4", "ISO 8859-5",
                    "$" + s[i - 1].ToString("X2") + s[i - 1 + 1].ToString("X2") + s[i - 1 + 2].ToString("X2") + s[i - 1 + 3].ToString("X2"));
            if (!TryLookup(Ucs4ToIso88595Table, s[i - 1 + 2], s[i - 1 + 3], s[i - 1 + 3], out byte outByte))
                throw EConvException.Create("UCS4", "ISO 8859-5", "$" + s[i - 1].ToString("X2"));
            result.Add(outByte);
            i = i + 4;
        }
        return result.ToArray();
    }

    // ---------------- 表查找/组装助手 ----------------

    /// <summary>UCS4(S, Offset, Sign, Const) := #hi#lo 组装（<c>#$hi</c> 为高字节）。</summary>
    internal static ushort Ucs4(byte b, int offset, int sign, byte konst)
    {
        int v = sign >= 0 ? b + konst : b - konst;
        return (ushort)(v & 0xFF);
    }

    /// <summary>按 (S[I+2], S[I+3]) 两级 case 查表（原文 UCS4→单字节）。
    /// 表内 <see cref="Identity"/> 表示原文该区间是 `Result := Result + S[I + 3]`
    /// （原样输出低字节）；由于 0 也是合法输出值，只有 0 需要按低字节还原。
    /// 这里统一按"值 = 0 即恒等"处理：生成的表中真实的 0 输出只可能来自
    /// `S[I+3]` 恒等区间（原文没有写出任何"映射到 0"的常量条目）。</summary>
    private static bool TryLookup(Dictionary<int, Dictionary<int, byte>> table, byte hi, byte lo, byte sourceLo, out byte value)
    {
        value = 0;
        Dictionary<int, byte> sub;
        if (!table.TryGetValue(hi, out sub)) return false;
        byte raw;
        if (!sub.TryGetValue(lo, out raw)) return false;
        value = raw == Identity ? sourceLo : raw;
        return true;
    }

    /// <summary>Latin-1（ISO-8859-1）透明映射：Delphi AnsiString ↔ 字节（不丢字节）。</summary>
    public static byte[] Latin1ToBytes(string s)
    {
        if (string.IsNullOrEmpty(s)) return Array.Empty<byte>();
        var bytes = new byte[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] > 0xFF) throw new EConvException("Latin1ToBytes: 字符超出 8 位范围 (U+" + ((int)s[i]).ToString("X4") + ")");
            bytes[i] = (byte)s[i];
        }
        return bytes;
    }

    /// <summary>Latin-1（ISO-8859-1）透明映射：字节 → Delphi AnsiString 等值 string。</summary>
    public static string BytesToLatin1(byte[] b)
    {
        if (b == null || b.Length == 0) return "";
        var chars = new char[b.Length];
        for (int i = 0; i < b.Length; i++) chars[i] = (char)b[i];
        return new string(chars);
    }
}
