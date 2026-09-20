// ============================================================================
// StringListHelper.pas（Source\M2Engine\StringListHelper.pas，53 行，GBK）1:1 移植
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.M2Server.Misc
//
// 单元原文（全文只有两个方法，:1-3 是注释）：
//   {
//     // 修复StringList读取无bom表的Utf8文件时乱码
//   }
//   unit StringListHelper;
//   interface
//   uses System.SysUtils, System.Classes, EncodingHelper;
//   type
//     TStringListHelper = class helper for TStringList
//       procedure LoadFromFile(const FileName: string); overload; virtual;
//       procedure LoadFromStream(Stream: TStream; Encoding: TEncoding); overload; virtual;
//     end;
//
// 【本单元存在的意义 = EncodingHelper 的**真实生产消费者**】
//   Delphi 侧调用链：`TStringListHelper.LoadFromFile` → `LoadFromStream(Stream, nil)`
//   → `TEncoding.GetBufferEncoding(Buffer, Encoding, DefaultEncoding)`
//   → `SetEncoding(Encoding)` + `SetTextStr(Encoding.GetString(Buffer, Size, Len - Size))`。
//   即：**无 BOM 的 UTF-8 文件**之所以不会乱码，全靠 EncodingHelper.pas 的嗅探。
//   （车道 p8-m2-itemprop-misc 的发现：EncodingHelper 移植后一度"零 C# 调用点"——
//    已移植但无生产调用方等于没接上；本文件就是把它接上的那一段。）
//
// 【保真要点】
//   * `class helper for TStringList` 在 C#（LangVersion 12）无对应物 → 落为**静态类**，
//     被扩展的实例作为**第一个显式参数** `TStringList strings`；方法名与重载顺序不变。
//   * 原文两个方法都标了 `virtual`（class helper 的 virtual 允许"后代 helper"覆盖）。
//     托管静态方法无虚分派语义，故该关键字**不适用**（登记为形式偏差 D-P8-14）。
//   * `TFileStream.Create(FileName, fmOpenRead or fmShareDenyWrite)` →
//     `FileMode.Open + FileAccess.Read + FileShare.Read`（"拒绝写、允许读"）。
//   * `BeginUpdate/EndUpdate` 在原文只是"批量修改通知"的抑制（TStrings 会触发 OnChange）；
//     托管 TStringList 没有 OnChange，故等价于空操作，此处照抄结构、注释说明。
//   * `DefaultEncoding` / `SetEncoding` 是 TStrings 的成员（托管侧见 GXX.Core.Util.TStringList，
//     由本车道请求 #3 补齐），**不是**本单元自定义的。
// ============================================================================

using GXX.Core.Util;

// 与 Delphi 逐字对照的别名（原文形参类型 TStream / TEncoding）
using TStream = System.IO.Stream;
using TEncoding = System.Text.Encoding;

namespace GXX.M2Server.Misc;

/// <summary>
/// StringListHelper.pas:13-16 <c>TStringListHelper = class helper for TStringList</c> 1:1
/// （托管侧为静态类，见文件头"保真要点"）。
/// </summary>
public static class TStringListHelper
{
    /// <summary>
    /// 原文 <c>procedure TStringListHelper.LoadFromFile(const FileName: string); overload; virtual;</c>
    /// （<c>:22-32</c>）1:1：
    /// <c>TFileStream.Create(FileName, fmOpenRead or fmShareDenyWrite)</c> →
    /// <c>try LoadFromStream(Stream, nil) finally Stream.Free end</c>。
    /// <para>
    /// 注意（原文如此）：此处把 <c>Encoding</c> 传 <c>nil</c>，即"完全交给嗅探"；
    /// 文件不存在时 <c>TFileStream.Create</c> 会抛 <c>EFOpenError</c>
    /// （与 <c>TStringList.LoadFromFile</c> 的托管既有"缺失即空表"行为不同，见 D-P8-12）。
    /// </para>
    /// </summary>
    public static void LoadFromFile(TStringList strings, string FileName)
    {
        var Stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read); // fmOpenRead or fmShareDenyWrite
        try
        {
            LoadFromStream(strings, Stream, null!);
        }
        finally
        {
            Stream.Dispose();                                   // :30 Stream.Free
        }
    }

    /// <summary>
    /// 原文 <c>procedure TStringListHelper.LoadFromStream(Stream: TStream; Encoding: TEncoding); overload; virtual;</c>
    /// （<c>:34-51</c>）1:1。
    ///
    /// <para>逐步：</para>
    /// <list type="number">
    /// <item><c>BeginUpdate</c>（托管侧无 OnChange，等价空操作）；</item>
    /// <item><c>Size := Stream.Size - Stream.Position</c>；<c>SetLength(Buffer, Size)</c>；
    ///   <c>Stream.Read(Buffer, 0, Size)</c>；</item>
    /// <item><c>Size := TEncoding.GetBufferEncoding(Buffer, Encoding, DefaultEncoding)</c>
    ///   —— ★ 本次接线的关键：<b>默认编码取的是被扩展实例的 <c>DefaultEncoding</c></b>
    ///   （Delphi <c>TStrings.DefaultEncoding</c>，托管 <c>TStringList.DefaultEncoding</c> = GBK/936），
    ///   而 <c>Encoding</c> 实参若是 <c>nil</c> 就完全由嗅探决定；</item>
    /// <item><c>SetEncoding(Encoding)</c>（原文注释 <c>// Keep Encoding in case the stream is saved</c>）
    ///   —— 让后续 <c>SaveToStream</c> 用同一编码；</item>
    /// <item><c>SetTextStr(Encoding.GetString(Buffer, Size, Length(Buffer) - Size))</c>
    ///   —— 跳过 BOM 后解码，再按 <c>TStrings.SetTextStr</c> 切行。</item>
    /// </list>
    /// </summary>
    public static void LoadFromStream(TStringList strings, TStream Stream, TEncoding? Encoding)
    {
        // :40 BeginUpdate —— 托管 TStringList 无 OnChange 通知，等价空操作
        try
        {
            int Size = (int)(Stream.Length - Stream.Position);   // :42
            byte[] Buffer = new byte[Size];                      // :43 SetLength(Buffer, Size)
            Stream.Read(Buffer, 0, Size);                        // :44

            TEncoding? encoding = Encoding;                      // 原文形参按值传递
            Size = GXX.Core.EncodingHelper.TEncodingHelper.GetBufferEncoding(   // :45
                Buffer, ref encoding, strings.DefaultEncoding);
            strings.SetEncoding(encoding!);                      // :46 Keep Encoding in case the stream is saved
            strings.SetTextStr(encoding!.GetString(Buffer, Size, Buffer.Length - Size));   // :47
        }
        finally
        {
            // :49 EndUpdate —— 同上，等价空操作
        }
    }
}
