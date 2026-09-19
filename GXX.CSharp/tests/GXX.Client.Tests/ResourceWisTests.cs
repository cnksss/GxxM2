using System;
using System.Collections.Generic;
using System.IO;
using GXX.Client.ReadResources;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次P1 / 车道 lane-resources：Wis.pas（996 行）1:1 移植的资源格式测试。
/// <para>WIS 格式：固定 512 字节文件头 + 图块区 + **文件尾倒序**索引（每项 12 字节 TWisHeader）。
/// 图块 = 12 字节 TImgInfo + 像素数据（btEncr0=1 时为 DecodeWis 压缩流）。</para>
/// <para>全部用合成字节构造，逐字段断言偏移与宽度，并覆盖异常路径。</para>
/// </summary>
public sealed class ResourceWisTests : IDisposable
{
    private readonly string _dir;

    public ResourceWisTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_p1_wis_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        ResetGlobals();
    }

    public void Dispose()
    {
        TextureSeams.ResetForTest();
        ResetGlobals();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private static void ResetGlobals()
    {
        TGameImages.g_boAutoUpdate = false;
        TGameImages.g_boDeviceInitializeOK = false;
        TGameImages.g_NullImage = default;
        TGameImages.g_DebugTextOut = null;
        TGameImages.UpdateEngineAddFn = null;
        TGameImages.MyGetTickCountFn = () => 0x0002_0000u;
    }

    // ===================================================================================
    // 1. 结构尺寸与字段偏移（逐字节）
    // ===================================================================================

    [Fact]
    public void TWisFileHeaderInfo_Is512Bytes_WithDocumentedFieldOffsets()
    {
        // Wis.pas 15-31：packed record，总计 $200 = 512 字节
        Assert.Equal(512, TWisFileHeaderInfo.SizeOf);

        // 逐字段偏移（与原文行内注释 0x04/0x9C/0xA0/0xA4/0xA8/0xAC 对应）
        Assert.Equal(0, OffsetOf(nameof(TWisFileHeaderInfo.nTitle)));
        Assert.Equal(4, OffsetOf(nameof(TWisFileHeaderInfo.VerFlag)));
        Assert.Equal(8, OffsetOf(nameof(TWisFileHeaderInfo.Reserve1)));
        Assert.Equal(12, OffsetOf(nameof(TWisFileHeaderInfo.DateTime)));
        Assert.Equal(20, OffsetOf(nameof(TWisFileHeaderInfo.Reserve2)));
        Assert.Equal(24, OffsetOf(nameof(TWisFileHeaderInfo.Reserve3)));
        Assert.Equal(28, OffsetOf(nameof(TWisFileHeaderInfo.CopyRight)));
        Assert.Equal(49, OffsetOf(nameof(TWisFileHeaderInfo.aTemp1)));
        Assert.Equal(156, OffsetOf(nameof(TWisFileHeaderInfo.nHeaderEncrypt)));
        Assert.Equal(160, OffsetOf(nameof(TWisFileHeaderInfo.nHeaderLen)));
        Assert.Equal(164, OffsetOf(nameof(TWisFileHeaderInfo.nImageCount)));
        Assert.Equal(168, OffsetOf(nameof(TWisFileHeaderInfo.nHeaderData)));
        Assert.Equal(172, OffsetOf(nameof(TWisFileHeaderInfo.aTemp2)));

        var h = TWisFileHeaderInfo.CreateEmpty();
        Assert.Equal(21, h.CopyRight.Length);
        Assert.Equal(107, h.aTemp1.Length);
        Assert.Equal(341, h.aTemp2.Length); // 172 + 341 = 513 > 512 —— 见类型注释的原文缺陷说明
    }

    private static int OffsetOf(string fieldName)
        => (int)System.Runtime.InteropServices.Marshal.OffsetOf<TWisFileHeaderInfo>(fieldName);

    [Fact]
    public void TWisHeader_Layout_12Bytes()
    {
        // Wis.pas 33-37：OffSet / Length / temp3
        Assert.Equal(12, TWisHeader.SizeOf);

        var h = new TWisHeader { OffSet = 0x11223344, Length = 0x55667788, temp3 = unchecked((int)0x99AABBCC) };
        var buf = new byte[12];
        h.ToBytes(buf, 0);

        Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, buf[0..4]);
        Assert.Equal(new byte[] { 0x88, 0x77, 0x66, 0x55 }, buf[4..8]);
        Assert.Equal(new byte[] { 0xCC, 0xBB, 0xAA, 0x99 }, buf[8..12]);

        var back = TWisHeader.FromBytes(buf, 0);
        Assert.Equal(0x11223344, back.OffSet);
        Assert.Equal(0x55667788, back.Length);
        Assert.Equal(unchecked((int)0x99AABBCC), back.temp3);
    }

    [Fact]
    public void TImgInfo_Layout_12Bytes()
    {
        // Wis.pas 41-50：btEncr0/btEncr1/bt2/bt3 @0..3，wW@4, wH@6, wPx@8, wPy@10
        Assert.Equal(12, TImgInfo.SizeOf);

        var i = default(TImgInfo);
        i.btEncr0 = 1;
        i.btEncr1 = 2;
        i.bt2 = 3;
        i.bt3 = 4;
        i.wW = 0x1112;
        i.wH = unchecked((short)0xFFFE); // -2
        i.wPx = -300;
        i.wPy = 400;

        var buf = new byte[12];
        i.ToBytes(buf, 0);

        Assert.Equal(new byte[] { 1, 2, 3, 4 }, buf[0..4]);
        Assert.Equal(new byte[] { 0x12, 0x11 }, buf[4..6]);
        Assert.Equal(new byte[] { 0xFE, 0xFF }, buf[6..8]);
        Assert.Equal(new byte[] { 0xD4, 0xFE }, buf[8..10]);
        Assert.Equal(new byte[] { 0x90, 0x01 }, buf[10..12]);

        var back = TImgInfo.FromBytes(buf, 0);
        Assert.Equal(1, back.btEncr0);
        Assert.Equal(2, back.btEncr1);
        Assert.Equal(3, back.bt2);
        Assert.Equal(4, back.bt3);
        Assert.Equal(0x1112, back.wW);
        Assert.Equal(-2, back.wH);
        Assert.Equal(-300, back.wPx);
        Assert.Equal(400, back.wPy);
    }

    // ===================================================================================
    // 2. DecodeWis 解码器（Wis.pas 218-266 扁平版 / 268-360 按行版）
    // ===================================================================================

    [Fact]
    public void DecodeWis_Flat_LiteralAndRun()
    {
        // 编码串解读（对应 Wis.pas 218-266 的状态机）：
        //   V=0, Len=0 → 读下一字节作 Len 与 boSkip
        //   [0, 3, 'A','B','C'] → 字面量段 3 字节
        //   [0, 2, 'D','E']     → 字面量段 2 字节
        //   [2, 'Z']            → 运行段：'Z' 重复 2 次
        var src = new byte[] { 0x00, 0x03, (byte)'A', (byte)'B', (byte)'C', 0x00, 0x02, (byte)'D', (byte)'E', 0x02, (byte)'Z' };
        var dst = new byte[16];

        Assert.True(TWisImages.DecodeWis(src, 0, dst, 0, src.Length, dst.Length));

        Assert.Equal((byte)'A', dst[0]);
        Assert.Equal((byte)'B', dst[1]);
        Assert.Equal((byte)'C', dst[2]);
        Assert.Equal((byte)'D', dst[3]);
        Assert.Equal((byte)'E', dst[4]);
        Assert.Equal((byte)'Z', dst[5]); // 运行段的起始下标 = 5
    }

    [Fact]
    public void DecodeWis_Flat_StopsAtDstSize()
    {
        // 目标容量极小：循环条件 (ADstSize > 0) 决定停止，多出的字节被丢弃
        var src = new byte[] { 0x00, 0x08, 1, 2, 3, 4, 5, 6, 7, 8 };
        var dst = new byte[3];

        Assert.True(TWisImages.DecodeWis(src, 0, dst, 0, src.Length, dst.Length));

        Assert.Equal(1, dst[0]);
        Assert.Equal(2, dst[1]);
        Assert.Equal(3, dst[2]);
    }

    [Fact]
    public void DecodeWis_Flat_ZeroLenLiteralTerminates()
    {
        // [0, 0] → Len = 0 → boSkip 被复位（原文的“空字面量”终止语义）
        var src = new byte[] { 0x00, 0x00 };
        var dst = new byte[8];

        Assert.True(TWisImages.DecodeWis(src, 0, dst, 0, src.Length, dst.Length));

        Assert.Equal(new byte[8], dst);
    }

    [Fact]
    public void DecodeWis_RowWrapped_HandlesWidthAndHeight()
    {
        // 按行版（Wis.pas 268-360）：ADst 写到行尾自动换到 ScanLine[nHeight]。
        // 4x2 图，按行给两次字面量段（每段 4 字节 = 恰好一行）：
        //   [0,4,10,11,12,13] [0,4,14,15,16,17]
        var src = new byte[] { 0x00, 0x04, 10, 11, 12, 13, 0x00, 0x04, 14, 15, 16, 17 };
        var dib = TWisImages.MakeDib(4, 2);

        Assert.True(TWisImages.DecodeWis(src, 0, src.Length, 4, 2, dib));

        Assert.Equal(10, dib.Pixel(0, 0));
        Assert.Equal(13, dib.Pixel(3, 0));
        Assert.Equal(14, dib.Pixel(0, 1));
        Assert.Equal(17, dib.Pixel(3, 1));
    }

    [Fact]
    public void DecodeWis_RowWrapped_RefusesRunLongerThanWidth()
    {
        // 原文 279：<c>if nWidth + WriteLen &lt;= AWidth then Move(...)</c> —— 字面量段**不跨行**；
        // 段长超过行宽时整段被丢弃（既不写也不推进 nWidth）—— 原文如此（Wis.pas:279-283）。
        var src = new byte[] { 0x00, 0x08, 10, 11, 12, 13, 14, 15, 16, 17 };
        var dib = TWisImages.MakeDib(4, 2);

        Assert.True(TWisImages.DecodeWis(src, 0, src.Length, 4, 2, dib));

        Assert.Equal(new byte[8], dib.Bits); // 整段被丢弃
    }

    [Fact]
    public void DecodeWis_RowWrapped_DiscardsBeyondHeight()
    {
        // 超出 AHeight 的写被静默丢弃（WriteData/WritePixel 的 if nHeight < AHeight 守卫）：
        // 4x2 图给 4 段各 4 字节（= 4 行），第 3/4 行越界被丢。
        var rows = new byte[4 * 6];
        for (int r = 0; r < 4; r++)
        {
            rows[r * 6 + 0] = 0x00;
            rows[r * 6 + 1] = 0x04;
            rows[r * 6 + 2] = (byte)(1 + r * 4);
            rows[r * 6 + 3] = (byte)(2 + r * 4);
            rows[r * 6 + 4] = (byte)(3 + r * 4);
            rows[r * 6 + 5] = (byte)(4 + r * 4);
        }
        var dib = TWisImages.MakeDib(4, 2);

        Assert.True(TWisImages.DecodeWis(rows, 0, rows.Length, 4, 2, dib));

        Assert.Equal(new byte[] { 1, 2, 3, 4 }, dib.ReadScanLine(0, 4));
        Assert.Equal(new byte[] { 5, 6, 7, 8 }, dib.ReadScanLine(1, 4));
    }

    [Fact]
    public void SGL_RLE8_Decode_LiteralAndRunBranches()
    {
        // 原文定义后从未被调用（Wis.pas:95-127），此处仅锁语义：
        //   0x03 'A' → 字面量分支读 1 字节（原文如此：Dec(ASrcSize, 2) 但只读 L=3 次同一字节）
        //   0x82 'B' → 游程分支：L = 0x82 and $7F = 2 → 读 2 个字节
        var src = new byte[] { 0x03, (byte)'A', 0x82, (byte)'B', (byte)'C' };
        var dst = new byte[16];

        Assert.True(TWisImages.SGL_RLE8_Decode(src, 0, src.Length, dst, 0, dst.Length));

        Assert.Equal((byte)'A', dst[0]);
        Assert.Equal((byte)'A', dst[1]);
        Assert.Equal((byte)'A', dst[2]);
        Assert.Equal((byte)'B', dst[3]);
        Assert.Equal((byte)'C', dst[4]);
    }

    [Fact]
    public void MakeDib_UsesWidthBytesPaddedTo4()
    {
        var dib = TWisImages.MakeDib(3, 2);
        Assert.Equal(3, dib.Width);
        Assert.Equal(2, dib.Height);
        Assert.Equal(8, dib.BitCount);
        Assert.Equal(4, dib.WidthBytes);   // ((3*8)+31) shr 5 * 4 = 4
        Assert.Equal(8, dib.Size);         // 4 * 2
        Assert.Equal(0, dib.ScanLineOffset(0));
        Assert.Equal(4, dib.ScanLineOffset(1));
    }

    // ===================================================================================
    // 3. LoadIndex：文件尾倒序索引
    // ===================================================================================

    [Fact]
    public void LoadIndex_SingleImage_FirstEntryPointsAtFirstImage()
    {
        // WIS 倒扫的终止条件（Wis.pas:472-475）：收集到 OffSet <= 512 的项就记下 FIndexOffset 并停。
        // 因此**第一项（文件里最靠前的那条）必须指向第一张图（OffSet = 512）**——这是格式约定。
        var img = BuildImageRecord(3, 8, 1, 2, compressed: false, payload: MakeFill(3 * 8, 7));
        var file = BuildWisFile(new[] { (OffSet: 512, Record: img) });
        var (wzl, _) = WriteWis("idx1", file);

        var images = new TWisImages();
        images.FileName = wzl;
        Assert.True(images.LoadIndex());

        Assert.Equal(1, images.ImageCount);
        Assert.NotNull(images.IndexArray);
        Assert.Equal(512, images.IndexArray![0].OffSet);
        Assert.Equal(img.Length, images.IndexArray[0].Length); // Length = 图块总字节数
        Assert.Equal(0, images.IndexArray[0].temp3);
        Assert.Equal(512 + img.Length, images.FIndexOffset); // 倒扫落在唯一的索引项上
    }

    [Fact]
    public void LoadIndex_SingleImage_PicksUpOneEntry()
    {
        var rec = BuildImageRecord(3, 3, 2, 2, false, MakeFill(9, 20));
        int off = 512;
        var file = BuildWisFileRaw(512, new[] { rec }, new[] { (OffSet: off, Length: rec.Length, temp3: 0) });
        var (wzl, _) = WriteWis("idx2", file);

        var images = new TWisImages();
        images.FileName = wzl;
        Assert.True(images.LoadIndex());

        Assert.Equal(1, images.ImageCount);
        Assert.Equal(off, images.IndexArray![0].OffSet);
        Assert.Equal(rec.Length, images.IndexArray[0].Length);
    }

    [Fact]
    public void LoadIndex_StaleIndexShapedImageData_IsAlsoCollected()
    {
        // 原文缺陷复现：倒扫不区分“索引区”与“图块区”，只要往前 12 字节读出来的 OffSet/Length 合法就继续收集。
        // 布局：头512 + 图块 + 伪索引#1 + 真索引 + 伪索引#2。
        //   · 倒扫首步落在**伪索引#2**（OffSet = 900 > 512，Legal）→ 收集但 OffSet 未 <= 512，继续；
        //   · 再往前一步是**真索引**（OffSet = 512 <= 512）→ 收集后 break，FIndexOffset 指向真索引；
        //   · 伪索引#1 因此**不会被收集**（它比终止项更靠前）。
        var img = BuildImageRecord(3, 3, 2, 2, false, MakeFill(9, 20));

        byte[] MakeFake(int off, int len)
        {
            var b = new byte[12];
            BitConverter.GetBytes(off).CopyTo(b, 0);
            BitConverter.GetBytes(len).CopyTo(b, 4);
            return b;
        }

        var list = new System.Collections.Generic.List<byte>(new byte[512]);
        list.AddRange(img);
        list.AddRange(MakeFake(700, 12));  // 伪索引#1（不会被收集）
        var realIdxPos = list.Count;
        list.AddRange(MakeFake(512, img.Length)); // 真索引
        list.AddRange(MakeFake(900, 12));  // 伪索引#2（会被收集）
        var file = list.ToArray();
        var (wzl, _) = WriteWis("idx2b", file);

        var images = new TWisImages();
        images.FileName = wzl;
        Assert.True(images.LoadIndex());

        // 收集顺序（越靠文件尾越先）为 [伪索引#2, 真索引]，再整体反转 → [真索引, 伪索引#2]
        Assert.Equal(2, images.ImageCount);
        Assert.Equal(512, images.IndexArray![0].OffSet);   // 真索引（文件里最靠前）
        Assert.Equal(21, images.IndexArray[0].Length);     // = 12（TImgInfo）+ 9（3x3 像素）
        Assert.Equal(900, images.IndexArray[1].OffSet);    // 伪索引#2（来自图块之后的填充数据）
        Assert.Equal(12, images.IndexArray[1].Length);
        Assert.Equal(realIdxPos, images.FIndexOffset);     // 倒扫终止于真索引
        Assert.Equal(21, img.Length);
    }

    [Fact]
    public void LoadIndex_InvalidEntry_StopsScan()
    {
        // 最靠文件尾的索引项非法（Length = 0）→ 立即 break，不收集任何项
        var file = BuildWisFileRaw(512, Array.Empty<byte[]>(), new[] { (OffSet: 600, Length: 0, temp3: 0) });
        var (wzl, _) = WriteWis("idx3", file);

        var images = new TWisImages();
        images.FileName = wzl;
        Assert.True(images.LoadIndex());
        Assert.Equal(0, images.ImageCount);
    }

    [Fact]
    public void LoadIndex_NoIndexArea_EmptyResult()
    {
        // 文件恰为 512 字节头：nIndexOffset(=512) 不 > iFileOffset(=512) → 走 else 分支
        var file = new byte[512];
        var (wzl, _) = WriteWis("idx4", file);

        var images = new TWisImages();
        images.FileName = wzl;
        Assert.True(images.LoadIndex());
        Assert.Equal(0, images.ImageCount);
        Assert.Equal(512, images.FIndexOffset);
    }

    [Fact]
    public void LoadIndex_MissingFile_ReturnsFalse()
    {
        var images = new TWisImages();
        images.FileName = Path.Combine(_dir, "nope.wis");
        Assert.False(images.LoadIndex());
        Assert.Equal(0, images.ImageCount);
    }

    [Fact]
    public void Initialize_MissingFile_StaysUninitialized()
    {
        var images = new TWisImages();
        images.FileName = Path.Combine(_dir, "nope2.wis");
        images.Initialize();

        Assert.False(images.Initialized);
        Assert.Null(images.FileStream);
        Assert.Equal(0, images.ImageCount);
    }

    [Fact]
    public void SetFileName_IndexFileName_EqualsFileName()
    {
        // GameImages.pas 1118-1120：Wis 不属于 TWMImages/TWzlImages → IndexFileName = FileName
        var images = new TWisImages();
        images.FileName = @"D:\Mir\Data\ChrSel.wis";
        Assert.Equal(@"D:\Mir\Data\ChrSel.wis", images.IndexFileName);
    }

    // ===================================================================================
    // 4. LoadDxImage / LoadDxGrayImage / LoadDxBrightImage
    // ===================================================================================

    [Fact]
    public void LoadDxImage_Uncompressed_ReadsRowsInOrder()
    {
        // 3x2 未压缩：逐行读 nWidth = 3 字节（WidthBytes 按 4 补齐，第 4 字节保持 0）
        var payload = new byte[] { 11, 12, 13, 21, 22, 23 };
        var rec = BuildImageRecord(3, 2, 5, 6, compressed: false, payload: payload);
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("u1", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        Assert.True(images.Initialized);

        var slot = images.m_ImgArr![0];
        images.LoadDxImage(images.IndexArray![0], slot);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);
        Assert.Equal(3, slot.nWidth);
        Assert.Equal(2, slot.nHeight);
        Assert.Equal(5, slot.nPx);
        Assert.Equal(6, slot.nPy);

        var bmp = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(3, bmp.Width);
        Assert.Equal(2, bmp.Height);
        Assert.Equal(GDefColorTable.GetARGB32(11), bmp.GetPixel(0, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(13), bmp.GetPixel(2, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(23), bmp.GetPixel(2, 1).ToArgb());
    }

    [Fact]
    public void LoadDxImage_Compressed_DecodeWisRowWrapped()
    {
        // 4x2 压缩图：两段字面量各 4 字节（每段不超过行宽，否则会被 WriteData 整段丢弃）
        var payload = new byte[] { 0x00, 0x04, 1, 2, 3, 4, 0x00, 0x04, 5, 6, 7, 8 };
        var rec = BuildImageRecord(4, 2, 0, 0, compressed: true, payload: payload);
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("u2", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(images.IndexArray![0], slot);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);
        var bmp = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(4, bmp.Width);
        Assert.Equal(2, bmp.Height);
        Assert.Equal(GDefColorTable.GetARGB32(1), bmp.GetPixel(0, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(5), bmp.GetPixel(0, 1).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(8), bmp.GetPixel(3, 1).ToArgb());
    }

    [Fact]
    public void LoadDxImage_TinyImage_LeavesSurfaceNilThenNullTexture()
    {
        // nSize = wW * wH <= 4 → 跳过整个装载块 → 末尾的 if DXImage.Surface = nil 把它置 NULLTexture
        var rec = BuildImageRecord(2, 2, 0, 0, compressed: false, payload: MakeFill(4, 9)); // 2*2 = 4
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("tiny", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(images.IndexArray![0], slot);

        Assert.True(slot.Surface.IsNullTexture);
        Assert.Equal(2, slot.nWidth); // 尺寸仍被赋值
        Assert.Equal(2, slot.nHeight);
    }

    [Fact]
    public void LoadDxImage_TooBigSize_Skipped()
    {
        // nSize >= 999999 → 跳过装载块
        var rec = BuildImageRecord(1000, 1000, 0, 0, compressed: false, payload: Array.Empty<byte>());
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("big", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(images.IndexArray![0], slot);

        Assert.True(slot.Surface.IsNullTexture);
        Assert.Equal(1000, slot.nWidth);
    }

    [Fact]
    public void LoadDxImage_OffsetOutOfRange_NoChange()
    {
        // IndexArray[0].OffSet 超出文件长 → 整段不做，Surface 保持 nil（末尾守卫也不进，因为它在 if 内）
        var rec = BuildImageRecord(3, 3, 0, 0, compressed: false, payload: MakeFill(9, 1));
        var header = new byte[512];
        var file = new byte[512 + rec.Length + 12];
        Array.Copy(header, file, 512);
        Array.Copy(rec, 0, file, 512, rec.Length);
        // 索引项指向文件之外
        var idx = new byte[12];
        BitConverter.GetBytes(99999).CopyTo(idx, 0);
        BitConverter.GetBytes(rec.Length).CopyTo(idx, 4);
        Array.Copy(idx, 0, file, 512 + rec.Length, 12);
        var (wzl, _) = WriteWis("oor", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        var before = slot.Surface;
        images.LoadDxImage(images.IndexArray![0], slot);

        Assert.False(slot.Surface.Assigned);
        Assert.Equal(before, slot.Surface);
        Assert.Equal(0, slot.nWidth);
    }

    [Fact]
    public void LoadDxGrayImage_And_BrightImage_UseOwnSlots()
    {
        var payload = new byte[] { 7, 8, 9, 7, 8, 9 }; // 3x2 → nSize = 6 > 4
        var rec = BuildImageRecord(3, 2, 1, 1, compressed: false, payload: payload);
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("gb", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();

        var gray = images.GetCachedGray(0);
        Assert.True(gray.Assigned);
        Assert.Single(images.GrayIndexList);

        var bright = images.GetCachedBright(0);
        Assert.True(bright.Assigned);
        Assert.Single(images.BrightIndexList);

        Assert.NotEqual(gray.Handle, bright.Handle);

        var bmpGray = TextureSeams.GetImage(gray.Handle)!;
        Assert.Equal(GDefColorTable.GetARGB32(7), bmpGray.GetPixel(0, 0).ToArgb());
        var bmpBright = TextureSeams.GetImage(bright.Handle)!;
        Assert.Equal(GDefColorTable.GetARGB32(7), bmpBright.GetPixel(0, 0).ToArgb());
    }

    [Fact]
    public void LoadDxBitmap_DoesNotProduceBitmap()
    {
        // 原文 803-806 走 TBitmap（本车道接缝未接入）→ Bitmap 槽位保持 nil
        var rec = BuildImageRecord(3, 3, 0, 0, compressed: false, payload: MakeFill(9, 4));
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("bmp", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();

        images.GetCachedBitmap(0);
        Assert.False(images.m_ImgArr![0].Bitmap.Assigned);
        Assert.Empty(images.IndexList);

        images.GetBitmap(0, out int px, out int py);
        Assert.Equal(0, px);
        Assert.Equal(0, py);
    }

    // ===================================================================================
    // 5. 缓存入口
    // ===================================================================================

    [Fact]
    public void GetCachedImage_LazyLoads_And_MarksIndexList()
    {
        var rec = BuildImageRecord(3, 3, 4, 5, compressed: false, payload: MakeFill(9, 77));
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("c1", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        Assert.Empty(images.IndexList);

        var tex = images.GetCachedImage(0, out int px, out int py);
        Assert.True(tex.Assigned);
        Assert.False(tex.IsNullTexture);
        Assert.Equal(4, px);
        Assert.Equal(5, py);
        Assert.Single(images.IndexList);
        Assert.Equal(0x0002_0000u, images.m_ImgArr![0].dwLatestTime);

        // 二次访问命中缓存
        var tex2 = images.GetCachedImage(0, out _, out _);
        Assert.Equal(tex, tex2);
        Assert.Single(images.IndexList);

        // GetCachedSurface 走同一槽位
        Assert.Equal(tex, images.GetCachedSurface(0));

        // 越界索引
        Assert.False(images.GetCachedImage(9, out _, out _).Assigned);
        Assert.False(images.GetCachedImage(-1, out _, out _).Assigned);
    }

    [Fact]
    public void GetCachedImageSize_AlwaysFalse_BecauseIndexListIsNeverFilled()
    {
        // 原文缺陷复现：条件里含 (AIndex < m_IndexList.Count)，而 TWisImages 从未向 m_IndexList 里 Add 过 →
        // 该函数**恒返回 False**（Wis.pas:537）。
        var rec = BuildImageRecord(6, 7, 1, 2, compressed: false, payload: MakeFill(6 * 7, 3));
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("s1", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();

        Assert.False(images.GetCachedImageSize(0, out int cx, out int cy, out int ptX, out int ptY));
        Assert.Equal(0, cx);
        Assert.Equal(0, cy);
        Assert.Equal(0, ptX);
        Assert.Equal(0, ptY);

        // 原文在 else 分支才会写入 ASize/APoint；本移植保持同样早退
        Assert.False(images.GetCachedImageSize(5, out _, out _, out _, out _));
    }

    [Fact]
    public void GetCachedGrayImage_And_BrightImage_OutputCoordinates()
    {
        var rec = BuildImageRecord(3, 3, 8, 9, compressed: false, payload: MakeFill(9, 2));
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("c2", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();

        images.GetCachedGrayImage(0, out int gpx, out int gpy);
        Assert.Equal(8, gpx);
        Assert.Equal(9, gpy);

        images.GetCachedBrightImage(0, out int bpx, out int bpy);
        Assert.Equal(8, bpx);
        Assert.Equal(9, bpy);
    }

    [Fact]
    public void Finalize_ResetsState()
    {
        var rec = BuildImageRecord(3, 3, 0, 0, compressed: false, payload: MakeFill(9, 1));
        var file = BuildWisFile(new[] { (OffSet: 512, Record: rec) });
        var (wzl, _) = WriteWis("fin", file);

        var images = new TWisImages();
        images.FileName = wzl;
        images.Initialize();
        Assert.True(images.Initialized);
        Assert.NotNull(images.FileStream);

        images.Finalize_();

        Assert.False(images.Initialized);
        Assert.Null(images.FileStream);
        Assert.Null(images.IndexArray);
        Assert.Null(images.m_ImgArr);
        Assert.Equal(0, images.ImageCount);
    }

    // ===================================================================================
    // 6. 合成字节工具
    // ===================================================================================

    private static byte[] MakeFill(int count, byte value)
    {
        var buf = new byte[count];
        for (int i = 0; i < count; i++) buf[i] = value;
        return buf;
    }

    /// <summary>合成一个图块 = 12 字节 TImgInfo + 像素数据。</summary>
    private static byte[] BuildImageRecord(short wW, short wH, short wPx, short wPy, bool compressed, byte[] payload)
    {
        var info = default(TImgInfo);
        info.btEncr0 = compressed ? (byte)1 : (byte)0;
        info.btEncr1 = 0;
        info.bt2 = 0;
        info.bt3 = 0;
        info.wW = wW;
        info.wH = wH;
        info.wPx = wPx;
        info.wPy = wPy;

        var buf = new byte[TImgInfo.SizeOf + payload.Length];
        info.ToBytes(buf, 0);
        Array.Copy(payload, 0, buf, TImgInfo.SizeOf, payload.Length);
        return buf;
    }

    /// <summary>
    /// 合成 .wis：512 字节头 + 顺序排列的图块 + **倒序**索引项。
    /// <para>索引项必须按「文件尾在前」的顺序写（原文 LoadIndex 用 Dec 倒着扫）。</para>
    /// </summary>
    private static byte[] BuildWisFileRaw(int headerSize, IReadOnlyList<byte[]> records, IReadOnlyList<(int OffSet, int Length, int temp3)> indexInFileOrder)
    {
        var list = new List<byte>(new byte[headerSize]);
        foreach (var r in records) list.AddRange(r);
        foreach (var e in indexInFileOrder)
        {
            var idx = new byte[TWisHeader.SizeOf];
            BitConverter.GetBytes(e.OffSet).CopyTo(idx, 0);
            BitConverter.GetBytes(e.Length).CopyTo(idx, 4);
            BitConverter.GetBytes(e.temp3).CopyTo(idx, 8);
            list.AddRange(idx);
        }
        return list.ToArray();
    }

    /// <summary>单/多图快捷构造：图块从 512 起顺序排列，索引按倒序写入文件尾。</summary>
    private static byte[] BuildWisFile(IReadOnlyList<(int OffSet, byte[] Record)> images)
    {
        var records = new List<byte[]>();
        foreach (var (_, rec) in images) records.Add(rec);

        // 索引项在文件里的书写顺序 = 图块在同一列表里的逆序
        var indexInFileOrder = new List<(int, int, int)>();
        for (int i = images.Count - 1; i >= 0; i--)
            indexInFileOrder.Add((images[i].OffSet, images[i].Record.Length, 0));

        return BuildWisFileRaw(512, records, indexInFileOrder);
    }

    private (string Wzl, string Wzx) WriteWis(string name, byte[] bytes)
    {
        var path = Path.Combine(_dir, name + ".wis");
        File.WriteAllBytes(path, bytes);
        return (path, path);
    }
}
