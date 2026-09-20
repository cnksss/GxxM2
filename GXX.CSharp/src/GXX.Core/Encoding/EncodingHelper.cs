// ============================================================================
// EncodingHelper.pas（Source\Common\EncodingHelper.pas，206 行，GBK）1:1 移植
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.Core.EncodingHelper
//
// 单元内容：
//   * TUTF8NoBomEncoding = class(TUTF8Encoding)（:9-12，实现 :31-34）—— 唯一的重写是
//     GetPreamble 返回**空**（原文 `SetLength(Result, 0)`）；
//   * TEncodingHelper = class helper for TEncoding（:14-25，实现 :133-204）——
//     GetBufferEncoding ×2（overload）、NoBomUTF8 属性、私有 GetNoBomUTF8 单例；
//   * 单元级函数 IsBufferUTF8（:48-131）与 GetBufferEncoding 内的嵌套函数 ContainsPreamble（:142-158）。
//
// 【为什么命名空间不是 GXX.Core.Encoding】
//   派发规定的目录是 src/GXX.Core/Encoding/**，但若命名空间取名 `GXX.Core.Encoding`，
//   则 `GXX.Core` 下的**既有文件**（GXX.Core/EncodingInit.cs:22/28/31/36、Launcher/*.cs、
//   Paradox/ParadoxDataSet.cs 等）里所有 `Encoding.Xxx` 的**简单名解析会命中命名空间**而
//   不是 System.Text.Encoding → 一片 CS0118（"命名空间被当作类型使用"）。
//   那些文件都在**他方常驻区**（不可修改），故本车道把命名空间定为 `GXX.Core.EncodingHelper`
//   （目录名与命名空间名不必一致）。已在报告登记。
//
// 【1:1 保真要点 —— 本单元"看起来一样实则不同"的分支最多】
//   1. `GetPreamble` 的 BOM/UTF-16 嗅探顺序**逐分支照抄**：UTF8(EF BB BF) → Unicode(FF FE) →
//      BigEndianUnicode(FE FF) → IsBufferUTF8 → 默认编码（**提前 Exit，不读默认编码的 BOM**）。
//   2. `IsBufferUTF8` 对**纯 ASCII 返回 False**（:68-69 注释写的是 "If all character is US-ASCII,
//      done."，但 Result 仍保持 False）→ 纯 ASCII 内容**不会**被判为 UTF-8，而是回落到默认编码。
//   3. `ContainsPreamble` 在签名长度为 0 时对**任何**缓冲（含空缓冲）返回 True（`for I := 1 to 0`
//      不执行）—— 这是个真实边界，见 :147-157。
//   4. IsBufferUTF8 接受**已废弃的 5/6 字节形态**（$F8..$FB、$FC..$FD）与**过长编码**（$C0/$C1）。
//   5. 默认编码是 Delphi 的 `TEncoding.Default` = **系统 ANSI 代码页**（本工程为 936/GBK），
//      不是 .NET 的 Encoding.Default（.NET Core 里那是 UTF-8）。
//   6. `AtomicCmpExchange` 单例：竞争失败的一方释放自己那份（:197-198）。
// ============================================================================

using System.Threading;
using GXX.Core.Protocol;

// 别名：让代码与原文逐行对照（原类型名 TEncoding/TUTF8Encoding）。
using TEncoding = System.Text.Encoding;
using TUTF8Encoding = System.Text.UTF8Encoding;

namespace GXX.Core.EncodingHelper;

// ---------------------------------------------------------------------------
// TUTF8NoBomEncoding（原文 :9-12、:31-34）
// ---------------------------------------------------------------------------

/// <summary>
/// EncodingHelper.pas:9-12 <c>TUTF8NoBomEncoding = class(TUTF8Encoding)</c> 1:1。
/// <para>唯一的重写：<c>GetPreamble</c> 恒返回空数组（原文 <c>SetLength(Result, 0)</c>），
/// 即"UTF-8 但写文件时不带 BOM"。</para>
/// <para>
/// 构造：原文用继承来的无参 <c>TUTF8Encoding.Create</c>；托管侧显式传
/// <c>encoderShouldEmitUTF8Identifier: false</c>（该标志在 .NET 里只影响
/// <c>UTF8Encoding.GetPreamble</c>，而后者已被本类重写为恒空，故两者等价）。
/// </para>
/// </summary>
public class TUTF8NoBomEncoding : TUTF8Encoding
{
    /// <summary>原文继承的无参构造（<c>TUTF8Encoding.Create</c>）。</summary>
    public TUTF8NoBomEncoding() : base(false)
    {
    }

    /// <summary>原文 <c>function TUTF8NoBomEncoding.GetPreamble: TBytes</c>（:31-34）：SetLength(Result, 0)。</summary>
    public override byte[] GetPreamble() => Array.Empty<byte>();
}

// ---------------------------------------------------------------------------
// TEncodingHelper（原文 :14-25、:133-204）
// ---------------------------------------------------------------------------

/// <summary>
/// EncodingHelper.pas:14-25 <c>TEncodingHelper = class helper for TEncoding</c> 1:1。
/// <para>
/// C#（LangVersion 12）无 "class helper"，故落为**静态类**：成员名、重载顺序、语义不变；
/// 调用点由 <c>TEncoding.GetBufferEncoding(...)</c> 变为
/// <c>TEncodingHelper.GetBufferEncoding(...)</c>。该形式偏差已在报告登记。
/// </para>
/// </summary>
public static class TEncodingHelper
{
    /// <summary>原文 <c>FNoBomEncoding: TUTF8NoBomEncoding</c>（:17，class var）。</summary>
    private static TUTF8NoBomEncoding? FNoBomEncoding;

    /// <summary>
    /// 原文 <c>Default</c>（:135 传给三参重载）＝ <c>TEncoding.Default</c>＝**系统 ANSI 代码页**
    /// （中文 Windows = 936/GBK；工程内既有实现 <c>EncodingInit.GBK</c>）。
    /// <para>★ 不是 .NET 的 <c>Encoding.Default</c>（.NET Core 下那是 UTF-8，语义完全不同）。</para>
    /// </summary>
    public static TEncoding Default => EncodingInit.GBK;

    /// <summary>
    /// 原文 <c>class function TEncodingHelper.GetBufferEncoding(const Buffer: TBytes;
    /// var AEncoding: TEncoding): Integer</c>（:133-136）1:1。
    /// <para>原文注释 "Must call property getter to create Encoding" 保留：此处取的是
    /// <see cref="Default"/> 属性。</para>
    /// </summary>
    public static int GetBufferEncoding(byte[] Buffer, ref TEncoding? AEncoding)
    {
        return GetBufferEncoding(Buffer, ref AEncoding, Default); // Must call property getter to create Encoding
    }

    /// <summary>
    /// 原文 <c>class function TEncodingHelper.GetBufferEncoding(const Buffer: TBytes;
    /// var AEncoding: TEncoding; ADefaultEncoding: TEncoding): Integer</c>（:139-188）1:1。
    ///
    /// <para>返回值为**该编码的 BOM 长度**（调用方据此跳过前 N 字节）。分支顺序逐字照抄：</para>
    /// <list type="number">
    /// <item><c>AEncoding = nil</c> 时按 BOM 嗅探，命中即 <c>Result := Length(AEncoding.GetPreamble)</c>；</item>
    /// <item>四条嗅探全不中且 <c>IsBufferUTF8</c> 为真 → <c>NoBomUTF8</c>（其 preamble 为空 → Result = 0）；</item>
    /// <item>否则 <c>AEncoding := ADefaultEncoding; Exit;</c> —— <b>提前退出，结果保持 0</b>，
    ///   即使 ADefaultEncoding 自己有 BOM（原文 :178 注释 <c>Don't proceed just in case
    ///   ADefaultEncoding has a Preamble</c>）；</item>
    /// <item><c>AEncoding &lt;&gt; nil</c> 时只看"传入编码的 preamble"是否匹配缓冲前缀，
    ///   匹配 → 其长度，否则 0（<b>不改变</b> <c>AEncoding</c>）。</item>
    /// </list>
    /// </summary>
    public static int GetBufferEncoding(byte[] Buffer, ref TEncoding? AEncoding, TEncoding ADefaultEncoding)
    {
        // 原文嵌套函数 ContainsPreamble（:142-158）
        static bool ContainsPreamble(byte[] Buffer, byte[] Signature)
        {
            bool Result = true;                                              // :146
            if (Buffer.Length >= Signature.Length)                           // :147
            {
                for (int I = 1; I <= Signature.Length; I++)                  // :149
                {
                    if (Buffer[I - 1] != Signature[I - 1])                   // :150
                    {
                        Result = false;                                      // :152
                        break;                                               // :153
                    }
                }
            }
            else
                Result = false;                                              // :157
            // ★ 签名长度为 0 时：Length(Buffer) >= 0 恒真、循环不执行 → Result 保持 True
            //   （对任何缓冲都"包含空前缀"，原文如此）
            return Result;
        }

        int Result = 0;                                                      // :163
        if (AEncoding == null)                                               // :164
        {
            // Find the appropraite encoding                                // :166（原文拼写如此）
            if (ContainsPreamble(Buffer, TEncoding.UTF8.GetPreamble()))      // :167
                AEncoding = TEncoding.UTF8;                                  // :168
            else if (ContainsPreamble(Buffer, TEncoding.Unicode.GetPreamble()))          // :169
                AEncoding = TEncoding.Unicode;                               // :170
            else if (ContainsPreamble(Buffer, TEncoding.BigEndianUnicode.GetPreamble())) // :171
                AEncoding = TEncoding.BigEndianUnicode;                      // :172
            else if (IsBufferUTF8(Buffer))                                   // :173
                AEncoding = TEncodingHelper.NoBomUTF8;                       // :174
            else
            {
                AEncoding = ADefaultEncoding;                                // :177
                return Result; // Don't proceed just in case ADefaultEncoding has a Preamble  // :178
            }
            Result = AEncoding.GetPreamble().Length;                         // :180
        }
        else
        {
            byte[] Preamble = AEncoding.GetPreamble();                       // :184
            if (ContainsPreamble(Buffer, Preamble))                          // :185
                Result = Preamble.Length;                                    // :186
        }
        return Result;
    }

    /// <summary>
    /// 原文 <c>class property NoBomUTF8: TEncoding read GetNoBomUTF8</c>（:24）。
    /// </summary>
    public static TEncoding NoBomUTF8 => GetNoBomUTF8();

    /// <summary>
    /// 原文 <c>class function TEncodingHelper.GetNoBomUTF8: TEncoding</c>（:190-204）1:1：
    /// 懒建 <see cref="TUTF8NoBomEncoding"/> 单例，用 <c>AtomicCmpExchange</c> 竞争，失败方释放自己那份
    /// （<c>LEncoding.Free</c>；托管侧丢弃引用即可）。
    /// <para><c>{$IFDEF AUTOREFCOUNT} FNoBomEncoding.__ObjAddRef; {$ENDIF}</c>（:199-201）
    /// 仅移动编译器（ARC）需要，托管侧不适用。</para>
    /// </summary>
    private static TEncoding GetNoBomUTF8()
    {
        if (FNoBomEncoding == null)                                          // :194
        {
            var LEncoding = new TUTF8NoBomEncoding();                        // :196
            if (Interlocked.CompareExchange(ref FNoBomEncoding, LEncoding, null) != null)  // :197
            {
                LEncoding = null!;                                           // :198 LEncoding.Free
            }
        }
        return FNoBomEncoding!;                                              // :203
    }

    // -----------------------------------------------------------------------
    // 单元级函数（原文 :48-131，不在 TEncodingHelper 内，属实现段）
    // -----------------------------------------------------------------------

    /// <summary>
    /// EncodingHelper.pas:48-131 <c>function IsBufferUTF8(Buffer: TBytes): Boolean</c> 1:1。
    ///
    /// <para>逐分支照抄（含注释里的字节形态表）：</para>
    /// <list type="bullet">
    /// <item>空缓冲 → <c>False</c>（:54）；</item>
    /// <item>先跳过前导 US-ASCII 段（:60-66）；</item>
    /// <item><b>★ 若全是 ASCII（P = EndPtr）→ 直接 Exit，Result 仍是 False</b>（:68-69，
    ///   注释写 "If all character is US-ASCII, done." 但并未置 True）；</item>
    /// <item>主循环：<c>$00..$7F</c> 前进 1；<c>$C0..$DF</c> + 1 续字节 → 2；
    ///   <c>$E0..$EF</c> + 2 → 3；<c>$F0..$F7</c> + 3 → 4；<c>$F8..$FB</c> + 4 → 5（已废弃形态）；
    ///   <c>$FC..$FD</c> + 5 → 6（已废弃形态）；其余（含 <c>$FE</c>/<c>$FF</c>）→ <c>Break</c>；</item>
    /// <item>续字节区间一律 <c>$80..$BF</c>，且长度判据是 <c>P+n &lt; EndPtr</c>（不是 <c>&lt;=</c>）；</item>
    /// <item>无"最短编码"校验：<c>$C0/$C1</c> 这类过长编码照样算合法（原文如此）。</item>
    /// </list>
    /// </summary>
    public static bool IsBufferUTF8(byte[] Buffer)
    {
        bool Result = false;                                                 // :53
        if (Buffer.Length == 0) return Result;                               // :54

        int P = 0;                                                           // :56 P := PByte(Buffer)
        int EndPtr = Buffer.Length;                                          // :57 EndPtr := P + Length(Buffer)

        // skip leading US-ASCII part.                                      // :59
        while (P < EndPtr)                                                   // :60
        {
            if (Buffer[P] <= 0x7F)                                           // :62
                P++;                                                         // :63
            else
                break;                                                       // :65
        }

        // If all character is US-ASCII, done.                              // :68
        if (P == EndPtr) return Result;                                      // :69  ★ Result 仍为 False

        while (P < EndPtr)                                                   // :71
        {
            byte C = Buffer[P];                                              // :73
            if (C <= 0x7F)                                                   // :75  $00..$7F: 1字节 0xxxxxxx
            {
                P++;                                                         // :76
            }
            else if (C >= 0xC0 && C <= 0xDF)                                 // :78  $C0..$DF: 2字节 110xxxxx 10xxxxxx
            {
                if (P + 1 < EndPtr && (Buffer[P + 1] >= 0x80 && Buffer[P + 1] <= 0xBF))  // :79-80
                    P += 2;                                                  // :81
                else
                    break;                                                   // :83
            }
            else if (C >= 0xE0 && C <= 0xEF)                                 // :85  3字节 1110xxxx
            {
                if (P + 2 < EndPtr
                    && (Buffer[P + 1] >= 0x80 && Buffer[P + 1] <= 0xBF)
                    && (Buffer[P + 2] >= 0x80 && Buffer[P + 2] <= 0xBF))     // :86-88
                    P += 3;                                                  // :89
                else
                    break;                                                   // :91
            }
            else if (C >= 0xF0 && C <= 0xF7)                                 // :93  4字节 11110xxx
            {
                if (P + 3 < EndPtr
                    && (Buffer[P + 1] >= 0x80 && Buffer[P + 1] <= 0xBF)
                    && (Buffer[P + 2] >= 0x80 && Buffer[P + 2] <= 0xBF)
                    && (Buffer[P + 3] >= 0x80 && Buffer[P + 3] <= 0xBF))     // :94-97
                    P += 4;                                                  // :98
                else
                    break;                                                   // :100
            }
            else if (C >= 0xF8 && C <= 0xFB)                                 // :102 5字节 111110xx（已废弃形态）
            {
                if (P + 4 < EndPtr
                    && (Buffer[P + 1] >= 0x80 && Buffer[P + 1] <= 0xBF)
                    && (Buffer[P + 2] >= 0x80 && Buffer[P + 2] <= 0xBF)
                    && (Buffer[P + 3] >= 0x80 && Buffer[P + 3] <= 0xBF)
                    && (Buffer[P + 4] >= 0x80 && Buffer[P + 4] <= 0xBF))     // :103-107
                    P += 5;                                                  // :108
                else
                    break;                                                   // :110
            }
            else if (C >= 0xFC && C <= 0xFD)                                 // :112 6字节 1111110x（已废弃形态）
            {
                if (P + 5 < EndPtr
                    && (Buffer[P + 1] >= 0x80 && Buffer[P + 1] <= 0xBF)
                    && (Buffer[P + 2] >= 0x80 && Buffer[P + 2] <= 0xBF)
                    && (Buffer[P + 3] >= 0x80 && Buffer[P + 3] <= 0xBF)
                    && (Buffer[P + 4] >= 0x80 && Buffer[P + 4] <= 0xBF)
                    && (Buffer[P + 5] >= 0x80 && Buffer[P + 5] <= 0xBF))     // :113-118
                    P += 6;                                                  // :119
                else
                    break;                                                   // :121
            }
            else
                break;                                                       // :123  （$FE/$FF 落此处）
        }

        if (P == EndPtr)                                                     // :127
            Result = true;                                                   // :128
        else
            Result = false;                                                  // :130
        return Result;
    }
}
