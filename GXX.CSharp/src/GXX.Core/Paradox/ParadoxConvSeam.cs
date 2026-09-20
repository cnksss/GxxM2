// ============================================================================
// 源单元：Source\RunGate\ParadoxDataSet.pas（GBK，1362 行）
// 本文件 = **ParadoxConv 接缝**（不是移植单元的一部分）。
//
// 原文 uses 子句（ParadoxDataSet.pas:55）：`DB, Classes, SysUtils, Forms, ParadoxConv;`
// TParadoxDataSet 只用到 ParadoxConv 的**三个入口**：
//   :1171  FCodepage := GetCodepage;                                  → 接缝 GetCodepage
//   :1349  Encoding(CP1251, UTF8, S)      （TEncoding.CP1251/UTF8）
//   :1350  Encoding(CP1251, KOI8R, S)
//   :1354  Encoding(CP866,  UTF8, S)      （TEncoding.CP866/UTF8）
//   :1355  Encoding(CP866,  KOI8R, S)
//   :1356  Encoding(CP866,  CP1251, S)
// 即：`TEncodingKind` 5 个取值（UCS4/UTF8/KOI8R/ISO88595/CP1251/CP866 中的 5 个）
// 与 `Encoding(Source, Dest, S: AnsiString): AnsiString`。
//
// ⚠ 引用方向（本车道的关键裁定，详见 docs/并行报告-p6-core-paradox.md）：
//   ParadoxConv 的**已完成托管实现**在 `GXX.RunGate/ParadoxConv.cs`
//   （namespace GXX.RunGate，public static partial class ParadoxConv，
//   含 TEncodingKind / GetCodepage / Encoding(string) / Encoding(byte[])）。
//   而 GXX.RunGate.csproj:9 引用 GXX.Core ⇒ **GXX.Core 不能反向引用 GXX.RunGate**，
//   否则形成循环工程引用。
//   故此处只声明**入口形状 + 可注入钩子**：RunGate/GameCenter 侧在启动时把
//   `ParadoxConv.Encoding` / `ParadoxConv.GetCodepage` 挂到下面的钩子上即可，
//   不需要在 GXX.Core 里复制一份转换器（台账 §15：已有真实现就不要另造接缝本体）。
//
//   接缝：待集成方在 GXX.RunGate / GXX.GameCenter 内接入（一行）：
//     GXX.Core.Paradox.ParadoxConvSeam.EncodingHook =
//         (src, dst, s) => GXX.RunGate.ParadoxConv.Encoding((GXX.RunGate.TEncodingKind)(int)src,
//                                                           (GXX.RunGate.TEncodingKind)(int)dst, s);
//     GXX.Core.Paradox.ParadoxConvSeam.GetCodepageHook = GXX.RunGate.ParadoxConv.GetCodepage;
//   （两个 TEncodingKind 的**整数值一致**——见下方枚举，与 ParadoxConv.pas:49 同序。）
//
// 原文缺陷（托管侧照抄上游实现，不在本接缝修正）：ParadoxConv 的 `Encoding` **没有
// CP866 作为 Source 的分支**（ParadoxConv.pas:155-164 被整段注释掉），故原文
// ParadoxDataSet.pas:1354-1356 这三行的 Encoding(CP866, ...) **恒返回空串**。
// ============================================================================

using System;

namespace GXX.Core.Paradox;

/// <summary>
/// ParadoxConv.pas:49 <c>TEncoding = (UCS4, UTF8, KOI8R, ISO88595, CP1251, CP866)</c>。
/// 顺序照抄 ⇒ 整数值与 GXX.RunGate.TEncodingKind 完全一致，接缝可直接转型。
/// </summary>
public enum TEncodingKind
{
    /// <summary>UCS4（4 字节大端）。</summary>
    UCS4 = 0,

    /// <summary>UTF-8。</summary>
    UTF8 = 1,

    /// <summary>KOI8-R。</summary>
    KOI8R = 2,

    /// <summary>ISO 8859-5。</summary>
    ISO88595 = 3,

    /// <summary>CP1251。</summary>
    CP1251 = 4,

    /// <summary>CP866。</summary>
    CP866 = 5,
}

/// <summary>
/// 接缝：ParadoxConv 的两个入口（<c>GetCodepage: string</c>、<c>Encoding(Source, Dest, S: AnsiString): AnsiString</c>）。
/// </summary>
public static class ParadoxConvSeam
{
    /// <summary>
    /// 接缝：待 `GXX.RunGate.ParadoxConv.GetCodepage` 接入（原文 ParadoxConv.pas:82-99，
    /// WINDOWS 分支为 <c>'CP' + IntToStr(GetACP)</c>）。
    /// 默认实现按同一 Windows 语义直算，使 GXX.Core 单独可运行/可测；
    /// 注意因此默认值是 `'CP&lt;当前 ANSI 代码页&gt;'`，而 EncodingString 比较的常量是
    /// `'UTF-8'`/`'KOI8R'`/`'CP1251'`（原文 :1349-1356）——**两者永不相等**，
    /// 于是原文在 Windows 下 EncodingString 恒等于入参（原文缺陷，见报告）。
    /// </summary>
    public static Func<string> GetCodepageHook = DefaultGetCodepage;

    /// <summary>
    /// 接缝：待 `GXX.RunGate.ParadoxConv.Encoding` 接入
    /// （签名 <c>Encoding(Source, Dest: TEncodingKind; S: AnsiString): AnsiString</c>）。
    /// 未接入时返回入参 S（恒等），与"无转换器"语义一致。
    /// </summary>
    public static Func<TEncodingKind, TEncodingKind, string, string> EncodingHook = DefaultEncoding;

    /// <summary>接缝默认实现：ParadoxConv.pas:82-99 的 WINDOWS 分支。</summary>
    public static string DefaultGetCodepage()
    {
        // 原文 {$IFDEF WINDOWS} Result := 'CP' + IntToStr(GetACP); {$ENDIF}
        try
        {
            return "CP" + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
        }
        catch
        {
            return "CP1252";
        }
    }

    /// <summary>接缝默认实现：恒等（未接入真转换器时不改字节）。</summary>
    public static string DefaultEncoding(TEncodingKind source, TEncodingKind dest, string s) => s;

    /// <summary>原文 :1171 <c>GetCodepage</c> 的调用入口。</summary>
    public static string GetCodepage() => GetCodepageHook();

    /// <summary>原文 :1349-1356 <c>Encoding(Source, Dest, S)</c> 的调用入口。</summary>
    public static string Encoding(TEncodingKind source, TEncodingKind dest, string s)
        => EncodingHook(source, dest, s);
}
