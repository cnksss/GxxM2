using System;
using GXX.Core.Rtl;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Pak.pas 1:1 移植 —— 接缝层（未移植单元的**最小**替代）。
//
// 本文件只放「Pak.pas 依赖、但归属其他单元且尚未移植」的符号；每个接缝都标注待接入的单元名。
// 一律**不做**超出 Pak.pas 用到的功能的扩张实现（避免顺手移植别的单元）。
//
// 已就位、直接复用（不算接缝）：
//   * GXX.Core.Crypto.UnitDes        ← Common\UnitDes.pas（EncryptCBC/DecryptCBC/EncryptDes/DecryptDes/DoInit/des_sptrans）
//   * GXX.Core.Compress.ZlibEx       ← Common\ZlibEx.pas（DecompressBuf）
//   * GXX.Core.Rtl.DelphiRTL         ← Delphi RTL 垫片（Format / IntToStr / Copy / Abs 语义）
//   * GXX.Core.Protocol.SDK          ← Common\SDK.pas（Pak.pas implementation uses 里有，但实现段未用）
//   * GameImagesBase.TTextureRef / TGameImages / TDxImage / TDib ← ReadResources 既有产物
// =====================================================================================

/// <summary>
/// GlobalString.pas 的资源字符串（原文 117-138 行，逐字抽取）。
/// <para><b>接缝：待 GlobalString.pas（含 DecodeResStr 的解密/查表）移植后接入</b>——
/// 此处直接内联明文格式串（与原文 <c>DecodeResStr(SPakXxx)</c> 的结果等价，
/// 因为原文 <c>DecodeResStr</c> 只是在运行时解密这些常量，最终 Format 的模板就是这些字节）。</para>
/// <para>编号与 Pak.pas 的调用点一一对应，便于后续替换为真正的资源层调用。</para>
/// </summary>
public static class PakResStrings
{
    /// <summary>GlobalString.pas 117：<c>SPakPasswordErr2 = '[Password] TPakImages::Initialize password error: %s';</c>
    /// <para>Pak.pas 902、1048。</para></summary>
    public const string SPakPasswordErr2 = "[Password] TPakImages::Initialize password error: %s";

    /// <summary>GlobalString.pas 118：Pak.pas 1580。</summary>
    public const string SPakGetCacheImageErr = "[Exception] TPakImages::GetCachedImage file error: %s; index: %d";

    /// <summary>GlobalString.pas 119：Pak.pas 2209/2241/2637。</summary>
    public const string SPakLoadDxImageErr = "[Exception] TPakImages::LoadDxImage position error: %s; Index: %d; X: %d; Y: %d";

    /// <summary>GlobalString.pas 120：Pak.pas 2215/2247/2642。</summary>
    public const string SPakLoadDxImageLenErr = "[Exception] TPakImages::LoadDxImage Length error: %s; Index: %d; Length: %d";

    /// <summary>GlobalString.pas 121：Pak.pas 2221/2227/2253/2259/2647/2652。</summary>
    public const string SPakLoadDxImageSizeErr = "[Exception] TPakImages::LoadDxImage Size error: %s; Index: %d; Width: %d; Height: %d";

    /// <summary>GlobalString.pas 123：Pak.pas 2712。</summary>
    public const string SPakImageDecompressErr = "[Exception] TPakImages::LoadDxImage decompressBuf: %s; Index: %d";

    /// <summary>GlobalString.pas 129：Pak.pas 2844。</summary>
    public const string SPakLoadDxGrayImageErr = "[Exception] TPakImages::LoadDxGrayImage position error: %s; Index: %d; X: %d; Y: %d";

    /// <summary>GlobalString.pas 130：Pak.pas 2849。</summary>
    public const string SPakLoadDxGrayImageLenErr = "[Exception] TPakImages::LoadDxGrayImage Length error: %s; Index: %d; Length: %d";

    /// <summary>GlobalString.pas 131：Pak.pas 2854/2859。</summary>
    public const string SPakLoadDxGrayImageSizeErr = "[Exception] TPakImages::LoadDxGrayImage Size error: %s; Index: %d; Width: %d; Height: %d";

    /// <summary>GlobalString.pas 132：Pak.pas 2921。</summary>
    public const string SPakGrayImageDecompErr = "[Exception] TPakImages::LoadDxGrayImage decompressBuf error: %s; Index: %d";

    /// <summary>GlobalString.pas 123：Pak.pas 2712。</summary>
    public const string SPakImageDecompErr = "[Exception] TPakImages::LoadDxImage decompressBuf: %s; Index: %d";

    /// <summary>GlobalString.pas 134：Pak.pas 3047。</summary>
    public const string SPakBrightImageErr = "[Exception] TPakImages::LoadDxBrightImage position error: %s; Index: %d; X: %d; Y: %d";

    /// <summary>GlobalString.pas 135：Pak.pas 3052。</summary>
    public const string SPakBrightImageLenErr = "[Exception] TPakImages::LoadDxBrightImage Length error: %s; Index: %d; Length: %d";

    /// <summary>GlobalString.pas 136：Pak.pas 3057/3062。</summary>
    public const string SPakBrightImageSizeErr = "[Exception] TPakImages::LoadDxBrightImage Size error: %s; Index: %d; Width: %d; Height: %d";

    /// <summary>GlobalString.pas 137：Pak.pas 3120。</summary>
    public const string SPakBrightImageDecompErr = "[Exception] TPakImages::LoadDxBrightImage decompressBuf error: %s; Index: %d";
}

/// <summary>
/// Pak.pas 依赖的「未移植单元」符号接缝。
/// <para>全部为静态可替换委托，默认实现与原文字节/语义一致；正式单元移植后把委托指过去即可。</para>
/// </summary>
public static class PakSeams
{
    /// <summary>
    /// 对应 <c>DecodeResStr(S)</c>（GlobalString.pas）：原文是运行时解密资源串。
    /// <para><b>接缝：待 GlobalString.pas 移植后接入</b>；当前为恒等（明文即最终模板）。</para>
    /// </summary>
    public static Func<string, string> DecodeResStrFn = s => s;

    /// <summary><c>DecodeResStr</c> 包装。</summary>
    public static string DecodeResStr(string s) => DecodeResStrFn(s);

    /// <summary>
    /// UpdateEngine 接缝（UpdateEngine.pas）：
    /// <c>g_UpdateEngine.Add(FileName, udtIndexPak|udtImagePak, Index, Self, nil): Boolean</c>。
    /// <para><b>接缝：待 UpdateEngine.pas / MShare.pas 移植后接入</b>。</para>
    /// <para>udt 常量（SDK.pas）：udtIndexPak / udtImagePak；Pak.pas 传 -1 / 0 / 具体索引。</para>
    /// </summary>
    public static Func<string, int, int, object?, object?, bool>? UpdateEngineAddFn;

    /// <summary>SDK.pas 的更新类型常量（Pak.pas 用到的两个）。</summary>
    public static class UpdateType
    {
        public const int udtIndexPak = 4;
        public const int udtImagePak = 5;
    }

    /// <summary>
    /// 对应文本侧 <c>Abs(SmallInt)</c>（System.Abs 的 16 位重载）。
    /// <para>原文多处 <c>Abs(ImageInfo.px) &gt; MAX_IMAGE_WIDTH</c>，其中 px/py 是 SmallInt。
    /// Delphi 对 <c>SmallInt(-32768)</c> 取 Abs 会**按 16 位回绕**成 -32768（仍为负），
    /// 于是该守卫对 -32768 失效；本方法复刻该回绕（C# 的 <c>Math.Abs(short)</c> 会抛异常或提升为 int）。</para>
    /// </summary>
    public static int AbsSmallInt(short v)
    {
        if (v == short.MinValue) return short.MinValue; // -32768：Delphi 16 位回绕
        return v < 0 ? -v : v;
    }

    /// <summary>
    /// <c>Move(Source.PBits^, ...)</c> 的最小承载物：把裸字节缓冲当成某图层的位数据。
    /// <para>原文用 <c>TDIB.PBits</c> 未类型化指针，本移植统一用 byte[]（见 <see cref="TDib.Bits"/>）。</para>
    /// </summary>
    public static void MoveBits(byte[] src, byte[] dest, int count)
    {
        int n = Math.Min(count, Math.Min(src.Length, dest.Length));
        if (n > 0) Array.Copy(src, 0, dest, 0, n);
    }

    /// <summary>
    /// Pak.pas 1188 等处的 <c>SetLength(S, SizeOf(TPakFileHeader))</c> + <c>Str[1]</c> 字节视图：
    /// Delphi AnsiString 的字符即字节，故本移植统一以 byte[] 承载密文串。
    /// 本方法把字节数组包成 Latin-1 字符串（用于需要 string 形参的 UnitDes 文本接口），反向见 FromLatin1。
    /// </summary>
    public static string ToLatin1(byte[] bytes)
        => System.Text.Encoding.GetEncoding(28591).GetString(bytes);

    /// <summary>Latin-1 字符串 → 字节（与 <see cref="ToLatin1"/> 对称）。</summary>
    public static byte[] FromLatin1(string s)
        => System.Text.Encoding.GetEncoding(28591).GetBytes(s ?? "");
}

/// <summary>
/// Delphi <c>TMemoryStream</c> 在 Pak.pas 里的用法（2499-2532/2272/2294/2432/2445）：
/// <c>Create</c> → <c>SetSize(n)</c> → <c>Read(Memory^, n)</c> → <c>Size</c> / <c>Memory</c>。
/// <para>本接缝只保留这三件事（不移植 TMemoryStream 全量）。</para>
/// </summary>
public sealed class PakMemoryStream
{
    private byte[] _data = Array.Empty<byte>();

    /// <summary>原文 <c>msData.SetSize(nImgDataSize)</c>（新缓冲，内容为 0）。</summary>
    public void SetSize(int size) => _data = new byte[Math.Max(size, 0)];

    /// <summary>原文 <c>msData.Memory</c>（缓冲首地址）。</summary>
    public byte[] Memory => _data;

    /// <summary>原文 <c>msData.Size</c>。</summary>
    public int Size => _data.Length;

    public static PakMemoryStream Create() => new();
}
