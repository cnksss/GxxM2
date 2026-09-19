using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GXX.Client.ReadResources;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次P1 / 车道 lane-resources：Wzl.pas（1475 行）1:1 移植的资源格式测试。
/// <para>全部用**合成字节**构造文件头/索引/ImageInfo（不依赖仓库外真实资源），逐字段断言偏移与宽度，
/// 并覆盖异常路径（索引越界、条目数为 0、文件被截断、非法 zlib 流）。</para>
/// <para><b>有效几何的原文怪癖</b>（本文件所有期望值都据此推算）：
/// Wzl.pas 的 TWzlImageInfo 声明为**非 packed** record，而磁盘上是 packed 15 字节块，于是原文 366-372 行
/// 实际读到的是 <c>nWidth = 块[0..1] = PixelFormat | (bt2 shl 8)</c>、
/// <c>nHeight = 块[4..5] = 文件里的 nWidth 字段</c>、
/// <c>px = 块[0..3] 拼成 32 位</c>、<c>py = 块[8..11] 拼成 32 位</c>。
/// 测试用 <c>Dim</c> 直接以“有效尺寸”书写，再由 <c>MakeInfo</c> 反推块内字段。</para>
/// </summary>
public sealed class ResourceWzlTests : IDisposable
{
    private readonly string _dir;

    public ResourceWzlTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_p1_wzl_" + Guid.NewGuid().ToString("N"));
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
        TGameImages.g_UpdateRetryTime = 30000;
        TGameImages.MyGetTickCountFn = () => 0x0001_0000u;
    }

    // ===================================================================================
    // 1. 大段表（调色板）校验 —— 由脚本从 GameImages.pas 190-255 抽取，此处断言校验和与首尾取样
    // ===================================================================================

    [Fact]
    public void GDefColorTable_Checksum_And_Samples()
    {
        // GameImages.pas 190-255：ColorArray:array[0..1023] of byte（256 × B,G,R,Reserved）
        Assert.Equal(1024, GDefColorTable.ColorArray.Length);

        long sum = 0;
        foreach (byte b in GDefColorTable.ColorArray) sum += b;
        Assert.Equal(71641L, sum); // 抽取脚本计算的校验和

        // 首 8 项（Windows 16 基本色前 8 个）
        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00 }, GDefColorTable.ColorArray[0..4]);    // idx0 黑
        Assert.Equal(new byte[] { 0x00, 0x00, 0x80, 0x00 }, GDefColorTable.ColorArray[4..8]);    // idx1 B=0 G=0 R=128
        Assert.Equal(new byte[] { 0x00, 0x80, 0x00, 0x00 }, GDefColorTable.ColorArray[8..12]);   // idx2 绿
        Assert.Equal(new byte[] { 0x00, 0x80, 0x80, 0x00 }, GDefColorTable.ColorArray[12..16]);  // idx3 青
        Assert.Equal(new byte[] { 0x80, 0x00, 0x00, 0x00 }, GDefColorTable.ColorArray[16..20]);  // idx4 红
        Assert.Equal(new byte[] { 0x80, 0x00, 0x80, 0x00 }, GDefColorTable.ColorArray[20..24]);  // idx5 品红
        Assert.Equal(new byte[] { 0x80, 0x80, 0x00, 0x00 }, GDefColorTable.ColorArray[24..28]);  // idx6 黄
        Assert.Equal(new byte[] { 0xC0, 0xC0, 0xC0, 0x00 }, GDefColorTable.ColorArray[28..32]);  // idx7 $C0C0C0

        // 末 8 项（GameImages.pas 253-254 行）
        Assert.Equal(new byte[] { 0x80, 0x80, 0x80, 0x00 }, GDefColorTable.ColorArray[992..996]);   // idx248 灰
        Assert.Equal(new byte[] { 0x00, 0x00, 0xFF, 0x00 }, GDefColorTable.ColorArray[996..1000]);  // idx249 蓝
        Assert.Equal(new byte[] { 0x00, 0xFF, 0x00, 0x00 }, GDefColorTable.ColorArray[1000..1004]); // idx250 绿
        Assert.Equal(new byte[] { 0x00, 0xFF, 0xFF, 0x00 }, GDefColorTable.ColorArray[1004..1008]); // idx251 青
        Assert.Equal(new byte[] { 0xFF, 0x00, 0x00, 0x00 }, GDefColorTable.ColorArray[1008..1012]); // idx252 红
        Assert.Equal(new byte[] { 0xFF, 0x00, 0xFF, 0x00 }, GDefColorTable.ColorArray[1012..1016]); // idx253 品红
        Assert.Equal(new byte[] { 0xFF, 0xFF, 0x00, 0x00 }, GDefColorTable.ColorArray[1016..1020]); // idx254 黄
        Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0x00 }, GDefColorTable.ColorArray[1020..1024]); // idx255 白

        // GameImages.pas 1795：Cardinal(pal1) or $FF000000（小端 → ARGB int）
        Assert.Equal(unchecked((int)0xFF000000), GDefColorTable.GetARGB32(0));   // idx0 黑
        Assert.Equal(unchecked((int)0xFFFFFFFF), GDefColorTable.GetARGB32(255)); // idx255 白
        Assert.Equal(unchecked((int)0xFFC0C0C0), GDefColorTable.GetARGB32(7));   // idx7 $C0C0C0
        Assert.Equal(unchecked((int)0xFF800000), GDefColorTable.GetARGB32(1));   // idx1 B=0 G=0 R=128
        Assert.Equal(unchecked((int)0xFF800080), GDefColorTable.GetARGB32(5));   // idx5 品红

        // 原文 1794：Integer(pal1) <> 0 判定
        Assert.False(GDefColorTable.IsNonZero(0)); // 0,0,0,0
        Assert.True(GDefColorTable.IsNonZero(1));
    }

    [Fact]
    public void GameImagesConsts_MatchOriginal()
    {
        // GameImages.pas 13-20
        Assert.Equal(0, GameImagesConsts.LOADIMAGEMODE);
        Assert.Equal(0, GameImagesConsts.USEMAPSTREAM);
        Assert.Equal(0x800000, GameImagesConsts.MAX_IMAGE_SIZE); // 8 shl 10 shl 10（注释「4M」有误）
        Assert.Equal(3200, GameImagesConsts.MAX_IMAGE_WIDTH);
        Assert.Equal(3200, GameImagesConsts.MAX_IMAGE_HEIGHT);
    }

    // ===================================================================================
    // 2. 结构尺寸与字段偏移（逐字节）
    // ===================================================================================

    [Fact]
    public void TWzlImageHeader_Layout_64Bytes()
    {
        // 非 packed：string[40] = 41 字节 → Integer 对齐到 44
        Assert.Equal(64, TWzlImageHeader.SizeOf);

        var h = TWzlImageHeader.CreateEmpty();
        h.TitleText = "WEMADE Entertainment inc."; // 25 字符
        h.ImageCount = 0x11223344;
        h.ColorCount = 0x22334455;
        h.PaletteSize = 0x33445566;
        h.VerFlag = 0x44556677;
        h.Flag = 0x55667788;

        var buf = new byte[64];
        h.ToBytes(buf, 0);

        Assert.Equal(25, buf[0]);                       // ShortString 长度字节
        Assert.Equal((byte)'W', buf[1]);
        Assert.Equal((byte)'.', buf[25]);               // 第 25 个字符
        Assert.Equal(0, buf[26]);                       // 26..40 补 0
        Assert.Equal(0, buf[40]);
        Assert.Equal(0, buf[41]);
        Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, buf[44..48]); // ImageCount @44
        Assert.Equal(new byte[] { 0x55, 0x44, 0x33, 0x22 }, buf[48..52]); // ColorCount @48
        Assert.Equal(new byte[] { 0x66, 0x55, 0x44, 0x33 }, buf[52..56]); // PaletteSize @52
        Assert.Equal(new byte[] { 0x77, 0x66, 0x55, 0x44 }, buf[56..60]); // VerFlag @56
        Assert.Equal(new byte[] { 0x88, 0x77, 0x66, 0x55 }, buf[60..64]); // Flag @60

        var back = TWzlImageHeader.FromBytes(buf, 0);
        Assert.Equal("WEMADE Entertainment inc.", back.TitleText);
        Assert.Equal(0x11223344, back.ImageCount);
        Assert.Equal(0x22334455, back.ColorCount);
        Assert.Equal(0x33445566, back.PaletteSize);
        Assert.Equal(0x44556677, back.VerFlag);
        Assert.Equal(0x55667788, back.Flag);

        // GBK 往返（中文标题）
        var h2 = TWzlImageHeader.CreateEmpty();
        h2.TitleText = "传奇资源库";
        var buf2 = new byte[64];
        h2.ToBytes(buf2, 0);
        Assert.Equal(10, buf2[0]); // GBK 每汉字 2 字节
        Assert.Equal("传奇资源库", TWzlImageHeader.FromBytes(buf2, 0).TitleText);
    }

    [Fact]
    public void TWzlIndexHeader_Layout_48Bytes()
    {
        Assert.Equal(48, TWzlIndexHeader.SizeOf);

        var h = TWzlIndexHeader.CreateEmpty();
        h.TitleText = "WEMADE Entertainment inc.";
        h.IndexCount = 0x0A0B0C0D;

        var buf = new byte[48];
        h.ToBytes(buf, 0);

        Assert.Equal(25, buf[0]);
        Assert.Equal(0, buf[40]);
        Assert.Equal(0, buf[41]);
        Assert.Equal(new byte[] { 0x0D, 0x0C, 0x0B, 0x0A }, buf[44..48]); // IndexCount @44

        var back = TWzlIndexHeader.FromBytes(buf, 0);
        Assert.Equal(0x0A0B0C0D, back.IndexCount);
        Assert.Equal("WEMADE Entertainment inc.", back.TitleText);
    }

    [Fact]
    public void TWzlImageInfo_Layout_15Bytes_Packed()
    {
        // Wzl.pas 366-372 的读取宽度自洽：磁盘上是 15 字节 packed 块
        Assert.Equal(15, TWzlImageInfo.SizeOf);

        var info = default(TWzlImageInfo);
        info.PixelFormat = 3;   // 块偏移 0（原文注释里的 bt1）
        info.bt2 = 1;           // 块偏移 1（是否压缩）
        info.bt3 = 2;           // 块偏移 2
        info.bt4 = 9;           // 块偏移 3（ZIP 压缩等级）
        info.nWidth = 17;       // 块偏移 4
        info.nHeight = -2;      // 块偏移 6
        info.px = -300;         // 块偏移 8
        info.py = 400;          // 块偏移 10
        info.Length = 0x01020304; // 块偏移 12

        var buf = new byte[16]; // 15 + 1：Length 末字节跨界
        info.ToBytes(buf, 0);

        Assert.Equal(3, buf[0]);
        Assert.Equal(1, buf[1]);
        Assert.Equal(2, buf[2]);
        Assert.Equal(9, buf[3]);
        Assert.Equal(new byte[] { 0x11, 0x00 }, buf[4..6]);   // nWidth = 17
        Assert.Equal(new byte[] { 0xFE, 0xFF }, buf[6..8]);   // nHeight = -2
        Assert.Equal(new byte[] { 0xD4, 0xFE }, buf[8..10]);  // px = -300
        Assert.Equal(new byte[] { 0x90, 0x01 }, buf[10..12]); // py = 400
        Assert.Equal(new byte[] { 0x04, 0x03, 0x02, 0x01 }, buf[12..16]);

        var back = TWzlImageInfo.FromBytes(buf, 0);
        Assert.Equal(3, back.PixelFormat);
        Assert.Equal(1, back.bt2);
        Assert.Equal(2, back.bt3);
        Assert.Equal(9, back.bt4);
        Assert.Equal(17, back.nWidth);
        Assert.Equal(-2, back.nHeight);
        Assert.Equal(-300, back.px);
        Assert.Equal(400, back.py);
        Assert.Equal(0x01020304, back.Length);
    }

    [Fact]
    public void TWzlImageInfo_LegacyView_MirrorsNonPackedRecord()
    {
        // 原文的 Length := PInteger(@rec[12])^ 读 4 字节 @12..15 —— 第 4 字节已越出 SizeOf=15 的块，
        // 故缓冲给到 16 字节（块外那 1 字节正是被跳过的 PixelFormat 占位）。
        var buf = new byte[16];
        buf[0] = 0x03; // 块偏移 0（原文注释的 bt1 / PixelFormat）
        buf[1] = 0x01; // 块偏移 1（原文注释的 bt2）
        buf[2] = 0x02; // 块偏移 2（原文注释的 bt3）
        buf[3] = 0x09; // 块偏移 3（原文注释的 bt4）
        buf[4] = 0x11; buf[5] = 0x00;              // 块偏移 4 的 nWidth 字段 = 17
        buf[6] = 0x22; buf[7] = 0x00;              // 块偏移 6 的 nHeight 字段 = 34
        buf[8] = 0xD4; buf[9] = 0xFE;              // 块偏移 8 的 px 字段 = -300
        buf[10] = 0x90; buf[11] = 0x01;            // 块偏移 10 的 py 字段 = 400
        buf[12] = 0x04; buf[13] = 0x03; buf[14] = 0x02; buf[15] = 0x01; // Length 4 字节 = 0x01020304

        var v = TWzlImageInfo.ReadLegacyView(buf, 0);

        // 原文 nWidth := PSmallInt(@rec[0])^ —— 低 16 位 = PixelFormat or (bt2 shl 8) = 3 | 256 = 259
        Assert.Equal(259, v.nWidth);
        // 原文 nHeight := PSmallInt(@rec[4])^ —— 即块偏移 4 的 nWidth 字段 = 17
        Assert.Equal(17, v.nHeight);
        // 原文 px := PSmallInt(@rec[0])^ + PSmallInt(@rec[2])^ shl 16 = 259 + (0x0902 shl 16)
        Assert.Equal(259 + (0x0902 << 16), v.px);
        // 原文 py := PSmallInt(@rec[8])^ + PSmallInt(@rec[10])^ shl 16 = -300 + (400 shl 16)
        Assert.Equal(-300 + (400 << 16), v.py);
        // 原文 Length := PInteger(@rec[12])^ = 0x01020304
        Assert.Equal(0x01020304, v.Length);
    }

    [Fact]
    public void TTextureRef_ThreeStateDistinction()
    {
        // nil（未尝试）/ NULLTexture（哨兵）/ 真实句柄 —— 原文 GetCached* 的判空依据
        Assert.False(TTextureRef.Nil.Assigned);
        Assert.True(TTextureRef.NullTextureSentinel.Assigned);
        Assert.True(TTextureRef.NullTextureSentinel.IsNullTexture);

        var real = TTextureRef.FromHandle(new IntPtr(0x1234));
        Assert.True(real.Assigned);
        Assert.False(real.IsNullTexture);
        Assert.NotEqual(TTextureRef.NullTextureSentinel, real);

        // FromHandle(0) 视同 nil（原文 Texture_Create 失败返回 nil 的场景）
        Assert.False(TTextureRef.FromHandle(IntPtr.Zero).Assigned);
    }

    // ===================================================================================
    // 3. 文件头解析（顺序读）
    // ===================================================================================

    [Fact]
    public void ReadHeaderFromStream_Sequential()
    {
        var h = TWzlImageHeader.CreateEmpty();
        h.TitleText = "WEMADE Entertainment inc.";
        h.ImageCount = 1024;
        h.ColorCount = 256;
        h.PaletteSize = 1024;
        h.VerFlag = 1;
        h.Flag = 0;
        var buf = new byte[64];
        h.ToBytes(buf, 0);

        using var ms = new MemoryStream(buf);
        var got = TWzlImages.ReadHeaderFromStream(ms);

        Assert.Equal(64, ms.Position); // 顺序读推进 64 字节
        Assert.Equal("WEMADE Entertainment inc.", got.TitleText);
        Assert.Equal(1024, got.ImageCount);
        Assert.Equal(256, got.ColorCount);
        Assert.Equal(1024, got.PaletteSize);
        Assert.Equal(1, got.VerFlag);
        Assert.Equal(0, got.Flag);
    }

    [Fact]
    public void ReadHeaderFromStream_TruncatedFile_GivesZeroFields()
    {
        // 截断文件（仅 4 字节）→ 余下字段补 0（原文 Read 短读后结构体为栈残留，此处确定性补 0）
        var buf = new byte[4];
        buf[0] = 3;
        buf[1] = (byte)'A';
        buf[2] = (byte)'B';
        buf[3] = (byte)'C';
        using var ms = new MemoryStream(buf);
        var got = TWzlImages.ReadHeaderFromStream(ms);

        Assert.Equal("ABC", got.TitleText);
        Assert.Equal(0, got.ImageCount);
        Assert.Equal(0, got.ColorCount);
        Assert.Equal(0, got.PaletteSize);
        Assert.Equal(0, got.VerFlag);
        Assert.Equal(0, got.Flag);
    }

    // ===================================================================================
    // 4. LoadIndex / 索引缓存（合成 .wzx 索引）
    // ===================================================================================

    [Fact]
    public void LoadIndex_ReadsOffsets_InOrder()
    {
        var (wzl, wzx) = WriteWzl("t1", BuildWzlHeader(0), new[] { 0x0000_0064, 0x0000_0100, 0x0000_0200 });

        var images = new TWzlImages();
        images.FileName = wzl;

        // GameImages.pas 1115-1117：TWzlImages → .wzx
        Assert.Equal(wzx, images.IndexFileName);

        images.LoadIndex(images.IndexFileName);

        Assert.True(images.IndexLoaded);
        Assert.Equal(3, images.IndexCount);
        Assert.Equal(0x64, images.GetIndexPosition(0));
        Assert.Equal(0x100, images.GetIndexPosition(1));
        Assert.Equal(0x200, images.GetIndexPosition(2));
    }

    [Fact]
    public void LoadIndex_ZeroCount_LeavesListEmpty()
    {
        var wzx = Path.Combine(_dir, "t2.wzx");
        WriteIndex(wzx, Array.Empty<int>());

        var images = new TWzlImages();
        images.LoadIndex(wzx);

        Assert.False(images.IndexLoaded);
        Assert.Equal(0, images.IndexCount);
    }

    [Fact]
    public void LoadIndex_IndexCountLargerThanFile_TruncatedEntriesAreZero()
    {
        // 索引越界/截断：头里声明 4 条，文件只给了 2 条 → 后 2 条取 0（原文会读越界内存，此处确定性补 0）
        var wzx = Path.Combine(_dir, "t3.wzx");
        var buf = BuildIndexBytes(new[] { 0x11, 0x22 });
        BitConverter.GetBytes(4).CopyTo(buf, 44);
        File.WriteAllBytes(wzx, buf);

        var images = new TWzlImages();
        images.LoadIndex(wzx);

        Assert.True(images.IndexLoaded);
        Assert.Equal(4, images.IndexCount);
        Assert.Equal(0x11, images.GetIndexPosition(0));
        Assert.Equal(0x22, images.GetIndexPosition(1));
        Assert.Equal(0, images.GetIndexPosition(2));
        Assert.Equal(0, images.GetIndexPosition(3));
    }

    [Fact]
    public void LoadIndex_TruncatedHeader_ReturnsWithoutList()
    {
        var wzx = Path.Combine(_dir, "t4.wzx");
        File.WriteAllBytes(wzx, new byte[20]); // 头都不够 48 字节

        var images = new TWzlImages();
        images.LoadIndex(wzx);

        Assert.False(images.IndexLoaded);
        Assert.Equal(0, images.IndexCount);
    }

    [Fact]
    public void LoadIndex_MissingFile_TriggersUpdateEngineSeam()
    {
        var (wzl, wzx) = WriteWzl("t5", BuildWzlHeader(0), Array.Empty<int>());
        File.Delete(wzx); // 制造“索引文件缺失”场景（WriteWzl 会顺手把 .wzx 也写出来）

        var images = new TWzlImages();
        images.FileName = wzl;
        images.m_boUpdateIndex = true;
        images.m_boUpdateIndexing = false;
        images.m_dwUpdateIndexingTick = 0;

        var calls = new List<(string, int, int)>();
        TGameImages.g_UpdateRetryTime = 0; // 使“重试窗口”立即成立（MyGetTickCount() - 0 >= 0）
        TGameImages.g_boAutoUpdate = true;
        TGameImages.UpdateEngineAddFn = (file, kind, index, _, _) =>
        {
            calls.Add((file, kind, index));
            return true;
        };

        images.LoadIndex(); // IndexFileName 指向的 .wzx 不存在

        Assert.True(calls.Count == 1, "idxFile=" + images.IndexFileName + " exists=" + File.Exists(images.IndexFileName)
            + " bo=" + images.m_boUpdateIndex + " ing=" + images.m_boUpdateIndexing + " tick=" + images.m_dwUpdateIndexingTick
            + " retry=" + TGameImages.g_UpdateRetryTime + " auto=" + TGameImages.g_boAutoUpdate
            + " now=" + TGameImages.MyGetTickCount() + " fn=" + (TGameImages.UpdateEngineAddFn != null));
        Assert.Equal(wzl, calls[0].Item1);
        Assert.Equal(0, calls[0].Item2); // udtIndexWzl
        Assert.Equal(-1, calls[0].Item3);
        Assert.True(images.m_boUpdateIndexing);
    }

    // ===================================================================================
    // 5. LoadDxImage：未压缩 / 解压 / 全部守卫分支
    // ===================================================================================

    [Fact]
    public void Initialize_And_LoadDxImage_Uncompressed_RawScanlines()
    {
        // 有效几何 (3, 8) → DIB nSize = WidthBytes(3)=4 * 8 = 32；Length = 0 → 原文 971-979 顺序读 nSize 字节
        var file = BuildWzlFile(new[] { MakeInfo(Dim(3, 8), 3, 10, 0) }, new[] { MakeFill(32, 5) });
        var (wzl, _) = WriteWzl("u1", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        Assert.True(images.Initialized);
        Assert.Equal(1, images.ImageCount);
        Assert.Equal(1, images.IndexCount);
        Assert.Equal("WEMADE Entertainment inc.", images.FHeader.TitleText);

        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);
        // 原文 nWidth := PSmallInt(@rec[0])^ —— 低 16 位 = PixelFormat | (bt2 shl 8) = 3
        Assert.Equal(3, slot.nWidth);
        // 原文 nHeight := PSmallInt(@rec[4])^ —— 即文件块偏移 4 的 nWidth 字段 = 8
        Assert.Equal(8, slot.nHeight);
        // 原文 px := PSmallInt(@rec[0])^ + PSmallInt(@rec[2])^ shl 16 —— 高 16 位来自 bt3/bt4（此处为 0）
        Assert.Equal(3, slot.nPx);
        // 原文 py := PSmallInt(@rec[8])^ + PSmallInt(@rec[10])^ shl 16 —— 低 16 位 = 块偏移 8 的 px 字段 = 10
        Assert.Equal(10, slot.nPy);
        Assert.Equal(0x0001_0000u, slot.dwLatestTime);

        // 像素解码：DIB (3, 8)，调色板 idx5 = B=$80 G=$00 R=$80 → 品红
        var bmp = TextureSeams.GetImage(slot.Surface.Handle);
        Assert.NotNull(bmp);
        Assert.Equal(3, bmp!.Width);
        Assert.Equal(8, bmp.Height);
        Assert.Equal(GDefColorTable.GetARGB32(5), bmp.GetPixel(0, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(5), bmp.GetPixel(2, 7).ToArgb());
    }

    [Fact]
    public void LoadDxImage_ZlibCompressed_Decompresses()
    {
        // 有效几何 (259, 8) → DIB nSize = WidthBytes(259)=260 * 8 = 2080
        var px = MakeFill(2080, 7);
        var compressed = GXX.Core.Compress.ZlibEx.CompressBuf(px, px.Length)!;
        var file = BuildWzlFile(new[] { MakeInfo(Dim(259, 8), 0, 0, compressed.Length) }, new[] { compressed });
        var (wzl, _) = WriteWzl("u2", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);
        Assert.Equal(259, slot.nWidth); // = 3 | (1 shl 8) = 259（bt2=1 走压缩分支）
        Assert.Equal(8, slot.nHeight);

        // DIB (259, 8)：2080 字节恰好铺满；idx7 = $C0C0C0
        var bmp = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(259, bmp.Width);
        Assert.Equal(8, bmp.Height);
        Assert.Equal(GDefColorTable.GetARGB32(7), bmp.GetPixel(0, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(7), bmp.GetPixel(258, 7).ToArgb());
    }

    [Fact]
    public void LoadDxImage_ZlibCompressed_ShorterThanDib_LeavesBlackTail()
    {
        // 有效几何 (259, 8) → nSize = 2080；只给 40 字节压缩数据 → 前 40 字节铺上，尾部保持索引 0（黑）
        var compressed = GXX.Core.Compress.ZlibEx.CompressBuf(MakeFill(40, 7), 40)!;
        var file = BuildWzlFile(new[] { MakeInfo(Dim(259, 8), 0, 0, compressed.Length) }, new[] { compressed });
        var (wzl, _) = WriteWzl("u3", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        var bmp = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(GDefColorTable.GetARGB32(7), bmp.GetPixel(0, 0).ToArgb());   // 前 40 字节内
        Assert.Equal(GDefColorTable.GetARGB32(7), bmp.GetPixel(39, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(0), bmp.GetPixel(0, 1).ToArgb());   // 尾部未被覆盖 → idx0 黑
    }

    [Fact]
    public void LoadDxImage_ErrorGuards_AllProduceNullTextureSentinel()
    {
        // 各越界档均 OutMessage 后置 NULLTexture 并保留 px/py。
        // 有效几何取非 packed 视图：Dim(w,h) → 块偏移 0/1 = w 低/高字节；块偏移 4/5 = h 低/高字节。
        var cases = new (string Name, byte[] Info)[]
        {
            ("px越界", MakeInfo(Dim(3, 8), 4000, 0, 0)),       // 有效 px = 4000 > MAX_IMAGE_WIDTH(3200)
            ("nHeight越界", MakeInfo(Dim(3, 4000), 0, 0, 0)),  // 有效 nHeight = 4000 > MAX_IMAGE_HEIGHT(3200)
            ("Length越界", MakeLengthOverflowInfo()),          // Length = 0xFFFFFF >= MAX_IMAGE_SIZE(0x800000)
            ("nWidth越界", MakeInfo(Dim(3200, 8), 0, 0, 0)),   // 有效 nWidth = 3200 >= MAX_IMAGE_WIDTH
            // 注意：空图片档**不** OutMessage（原文 856-862 直接置 NULLTexture 并 Exit）→ 单列一个用例
        };

        int caseIdx = 0;
        foreach (var (caseName, info) in cases)
        {
            var file = BuildWzlFile(new[] { info }, new[] { new byte[64] });
            var (wzl, _) = WriteWzl("gcase" + caseIdx++, file, new[] { 64 });

            var messages = new List<string>();
            TGameImages.g_DebugTextOut = (m, _) => messages.Add(m);

            var images = new TWzlImages();
            images.FileName = wzl;
            images.Initialize();
            var slot = images.m_ImgArr![0];
            images.LoadDxImage(64, slot, 0);

            Assert.True(slot.Surface.Assigned, caseName + "/assigned/init=" + images.Initialized);
            Assert.True(slot.Surface.IsNullTexture, caseName + "/nulltexture");
            Assert.True(messages.Count == 1, caseName + "/msgs=" + messages.Count + "/" + string.Join("|", messages));
            Assert.StartsWith("WZL ", messages[0]);
            Assert.Equal(0x0001_0000u, slot.dwLatestTime);
        }

        // 空图片档：原文 856-862 直接置 NULLTexture 并 Exit（**不** OutMessage）
        {
            var file = BuildWzlFile(new[] { MakeInfo(Dim(3, 0), 0, 0, 0) }, new[] { new byte[8] });
            var (wzl, _) = WriteWzl("gquiet", file, new[] { 64 });

            var messages = new List<string>();
            TGameImages.g_DebugTextOut = (m, _) => messages.Add(m);

            var images = new TWzlImages();
            images.FileName = wzl;
            images.Initialize();
            var slot = images.m_ImgArr![0];
            images.LoadDxImage(64, slot, 0);

            Assert.True(slot.Surface.IsNullTexture);
            Assert.Empty(messages);
            Assert.Equal(0x0001_0000u, slot.dwLatestTime);
        }
    }

    [Fact]
    public void LoadDxImage_PositionZeroOrPastEnd_NoTextureNoMessage()
    {
        var file = BuildWzlFile(new[] { MakeInfo(Dim(3, 8), 3, 2, 0) }, new[] { new byte[32] });
        var (wzl, _) = WriteWzl("p0", file, new[] { 64 });

        var messages = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => messages.Add(m);

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        // Position = 0 → 直接 Exit（打点 + 清 px/py，不改动 Surface）
        var slot0 = images.m_ImgArr![0];
        images.LoadDxImage(0, slot0, 0);
        Assert.False(slot0.Surface.Assigned);
        Assert.Equal(0, slot0.nPx);
        Assert.Equal(0, slot0.nPy);
        Assert.Equal(0x0001_0000u, slot0.dwLatestTime);

        // Position > Size - 15 → 同样 Exit
        var slot1 = new TDxImage();
        images.LoadDxImage((int)images.m_FileStream!.Length, slot1, 0);
        Assert.False(slot1.Surface.Assigned);
        Assert.Empty(messages);
    }

    [Fact]
    public void LoadDxImage_BadPixelFormatByte_MakeDibReturnsNull()
    {
        // 原文 MakeDibByPixelFormat 的 else 分支（pfDevice/pf1bit/pf4bit/pfCustom 等）→ Source = nil →
        // 整个装载体被跳过，Surface 保持 nil（**不是** NULLTexture）。
        // 取块偏移 0 = 1（pf1bit）：有效 nWidth = 1，有效 nHeight = 8，nWidth*nHeight > 0，能走到 MakeDibByPixelFormat。
        var file = BuildWzlFile(new[] { MakeInfo(Dim(1, 8), 0, 0, 0) }, new[] { new byte[64] });
        var (wzl, _) = WriteWzl("pf1bit", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        Assert.False(slot.Surface.Assigned); // Source = nil → 未赋任何纹理
        Assert.Equal(1, slot.nWidth);        // 打点/尺寸赋值已发生（在 Source 之前）
        Assert.Equal(8, slot.nHeight);
    }

    [Fact]
    public void LoadDxImage_TruncatedData_DecompressFails_ProducesNullTexture()
    {
        // Length 声明 8 字节，数据是**截断的 zlib 流**（合法头 0x78 0x9C + 不完整 deflate）→
        // DecompressBuf 抛异常 → boDecompressError → NULLTexture
        var payload = new byte[] { 0x78, 0x9C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        // 先用原生库确认该载荷确实解压失败（否则本用例失去意义）
        Assert.ThrowsAny<Exception>(() =>
        {
            using var msIn = new MemoryStream(payload);
            using var zs = new System.IO.Compression.ZLibStream(msIn, System.IO.Compression.CompressionMode.Decompress);
            zs.CopyTo(Stream.Null);
        });

        var file = new List<byte>();
        file.AddRange(BuildWzlHeader(1));
        file.AddRange(MakeInfo(Dim(3, 8), 0, 0, payload.Length));
        file.AddRange(payload);

        var (wzl, _) = WriteWzl("trunc", file.ToArray(), new[] { 64 });

        var messages = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => messages.Add(m);

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        Assert.True(slot.Surface.IsNullTexture);
        Assert.Single(messages); // 只报解压失败（Length 8 < MAX_IMAGE_SIZE 且尺寸合法）
        Assert.Contains("解压失败", messages[0]);
    }

    [Fact]
    public void LoadDxImage_EmptyDecompressOutput_DoesNotSetErrorFlag()
    {
        // 原文缺陷复现：DecompressBuf 返回**空数组**（OutBytes = 0）时不置 boDecompressError，
        // 于是不填像素却仍造出纹理（黑图）。
        var payload = new byte[] { 0x78, 0x9C, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01 }; // 合法的“空 deflate”
        var file = new List<byte>();
        file.AddRange(BuildWzlHeader(1));
        file.AddRange(MakeInfo(Dim(3, 8), 0, 0, payload.Length));
        file.AddRange(payload);

        var (wzl, _) = WriteWzl("emptyz", file.ToArray(), new[] { 64 });

        var messages = new List<string>();
        TGameImages.g_DebugTextOut = (m, _) => messages.Add(m);

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);
        Assert.Empty(messages);
    }

    [Fact]
    public void LoadDxImage_ResetWZLAlpha_BuildsAlphaPlane()
    {
        // 有效几何 (259, 8) → DIB nSize = 260 * 8 = 2080；
        // alpha 尾长度须等于 nWidth*nHeight div 2 = 259*8/2 = 1036 → 解压输出须为 2080 + 1036 = 3116 字节
        var alphaTail = new byte[1036];
        for (int i = 0; i < 1036; i++) alphaTail[i] = (byte)(0x10 + i);
        var payload = new byte[2080 + 1036];
        Array.Copy(MakeFill(2080, 3), 0, payload, 0, 2080);
        Array.Copy(alphaTail, 0, payload, 2080, 1036);

        var compressed = GXX.Core.Compress.ZlibEx.CompressBuf(payload, payload.Length)!;
        var file = BuildWzlFile(new[] { MakeInfo(Dim(259, 8), 0, 0, compressed.Length) }, new[] { compressed });
        var (wzl, _) = WriteWzl("alpha", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.ResetWZLAlpha = true;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);

        var bmp = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(259, bmp.Width);
        Assert.Equal(8, bmp.Height);

        // 原文 alpha 展开（Wzl.pas 950-964）：
        //   pData := @SourceAlphaBuf[nY * lsAlpha.Width div 2] → 行基址 = nY * (259 div 2) = nY * 129
        //   nX_2 := nX div 2; nX 偶 → pData[nX_2]；nX 奇 → (pData[nX_2] + pData[nX_2+1]) div 2
        // 第 0 行：nX=0→pData[0]=0x10；nX=1→(0x10+0x11)/2=0x10；nX=2→pData[1]=0x11
        Assert.Equal(0x10, bmp.GetPixel(0, 0).A);
        Assert.Equal(0x10, bmp.GetPixel(1, 0).A);
        Assert.Equal(0x11, bmp.GetPixel(2, 0).A);
        // 第 1 行：行基址 = 129 → nX=0→pData[129]=0x91；nX=1→(0x91+0x92)/2=0x91；nX=2→pData[130]=0x92
        Assert.Equal(0x91, bmp.GetPixel(0, 1).A);
        Assert.Equal(0x91, bmp.GetPixel(1, 1).A);
        Assert.Equal(0x92, bmp.GetPixel(2, 1).A);
    }

    [Fact]
    public void LoadDxImage_ResetWZLAlpha_SizeMismatch_NoAlpha()
    {
        // 解压输出尾部长度 != nWidth*nHeight div 2 → 不构造 alpha 平面（原文 944 行的 if 守卫）
        var payload = new byte[2080 + 10]; // 尾部只有 10 字节，而期望 1036
        Array.Copy(MakeFill(2080, 3), 0, payload, 0, 2080);
        for (int i = 0; i < 10; i++) payload[2080 + i] = 0xAA;

        var compressed = GXX.Core.Compress.ZlibEx.CompressBuf(payload, payload.Length)!;
        var file = BuildWzlFile(new[] { MakeInfo(Dim(259, 8), 0, 0, compressed.Length) }, new[] { compressed });
        var (wzl, _) = WriteWzl("alpha2", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.ResetWZLAlpha = true;
        images.Initialize();
        var slot = images.m_ImgArr![0];
        images.LoadDxImage(64, slot, 0);

        var bmp = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(255, bmp.GetPixel(0, 0).A); // 无 alpha 平面 → 不透明
    }

    // ===================================================================================
    // 6. 缓存入口（GetCachedImage / GetCachedImageSize / GetCachedGray / GetCachedBright）
    // ===================================================================================

    [Fact]
    public void GetCachedImage_LazyLoads_And_MarksIndexList()
    {
        var file = BuildWzlFile(new[] { MakeInfo(Dim(4, 4), 4, -7, 0) }, new[] { MakeFill(4 * 4, 255) });
        var (wzl, _) = WriteWzl("c1", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        Assert.Empty(images.IndexList);

        var tex = images.GetCachedImage(0, out int px0, out int py0);
        Assert.True(tex.Assigned);
        Assert.False(tex.IsNullTexture);
        Assert.Equal(4, px0); // 有效 px = 块偏移 0..3 低 16 位 = 4
        Assert.Equal(-7, py0);
        Assert.Single(images.IndexList);
        Assert.Equal(0, images.IndexList[0]);
        Assert.Equal(0x0001_0000u, images.m_ImgArr![0].dwLatestTime);

        // 二次访问：命中缓存（不重复 Add）
        var tex2 = images.GetCachedImage(0, out _, out _);
        Assert.Equal(tex, tex2);
        Assert.Single(images.IndexList);
    }

    [Fact]
    public void GetCachedImage_NegativeIndex_ReturnsNil()
    {
        var images = new TWzlImages();
        var tex = images.GetCachedImage(-1, out int px, out int py);
        Assert.False(tex.Assigned);
        Assert.Equal(0, px);
        Assert.Equal(0, py);
    }

    [Fact]
    public void GetCachedImage_OutOfRange_FallsToUpdateEngine_Branch()
    {
        var file = BuildWzlFile(new[] { MakeInfo(Dim(3, 8), 0, 0, 0) }, new[] { new byte[32] });
        var (wzl, _) = WriteWzl("c2", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        images.m_boUpdateIndex = true;
        images.m_boNeedUpdate = true;
        images.m_dwUpdateIndexingTick = 0;

        var calls = new List<int>();
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (_, kind, index, _, _) => { calls.Add(index); return true; };

        // Index >= ImageCount → else 分支
        var tex = images.GetCachedImage(99, out _, out _);
        Assert.False(tex.Assigned);
        Assert.Single(calls);
        Assert.Equal(-1, calls[0]);
    }

    [Fact]
    public void GetCachedImage_FailureFallsBackToNullImage()
    {
        // 空图片 → LoadDxImage 置 NULLTexture（Assigned=true）→ 不再回退 g_NullImage
        var file = BuildWzlFile(new[] { MakeInfo(Dim(3, 0), 0, 0, 0) }, new[] { new byte[8] });
        var (wzl, _) = WriteWzl("c3", file, new[] { 64 });

        var nullImage = TTextureRef.FromHandle(new IntPtr(0x7777));
        TGameImages.g_NullImage = nullImage;

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        var tex = images.GetCachedImage(0, out _, out _);
        Assert.True(tex.IsNullTexture);
        Assert.NotEqual(nullImage, tex);
    }

    [Fact]
    public void GetCachedImageSize_ReadsInfoAtPosition()
    {
        // 有效几何 (12, 9)，APoint = (px 字段, py 字段) = (3, 4)
        var file = BuildWzlFile(new[] { MakeInfo(Dim(12, 9), 12, 9, 0) }, new[] { new byte[64] });
        var (wzl, _) = WriteWzl("s1", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        Assert.True(images.GetCachedImageSize(0, out int cx, out int cy, out int ptX, out int ptY));
        Assert.Equal(12, cx);
        Assert.Equal(9, cy);
        Assert.Equal(12, ptX); // 有效 px 低 16 位 = 块偏移 0..1 = 12（原文这一档读的是 px，不是 nWidth）
        Assert.Equal(9, ptY);  // 有效 py 低 16 位 = 块偏移 8..9 = 9

        // 槽位已被填充 → 二次调用走缓存分支
        Assert.True(images.GetCachedImageSize(0, out cx, out cy, out ptX, out ptY));
        Assert.Equal(12, cx);
        Assert.Equal(9, cy);

        // 越界索引
        Assert.False(images.GetCachedImageSize(5, out _, out _, out _, out _));
        Assert.False(images.GetCachedImageSize(-1, out _, out _, out _, out _));
    }

    [Fact]
    public void GetCachedGray_And_Bright_UseTheirOwnSlots()
    {
        var file = BuildWzlFile(new[] { MakeInfo(Dim(4, 4), 4, 2, 0) }, new[] { MakeFill(4 * 4, 200) });
        var (wzl, _) = WriteWzl("gb", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        var gray = images.GetCachedGray(0);
        Assert.True(gray.Assigned);
        Assert.Single(images.GrayIndexList);
        Assert.False(images.m_ImgArr![0].Gray.IsNullTexture);

        var bright = images.GetCachedBright(0);
        Assert.True(bright.Assigned);
        Assert.Single(images.BrightIndexList);
        Assert.False(images.m_ImgArr[0].Bright.IsNullTexture);

        // 两槽位是不同的纹理对象
        Assert.NotEqual(gray.Handle, bright.Handle);

        // 原文 1095/1150 的 Index < 0 早退
        Assert.False(images.GetCachedGray(-1).Assigned);
        Assert.False(images.GetCachedBright(-1).Assigned);

        // GetCachedGrayImage 的 var 输出走同名槽位（有效 px = 块偏移 0..1 = 4）
        var gray2 = images.GetCachedGrayImage(0, out int gpx, out int gpy);
        Assert.Equal(gray, gray2);
        Assert.Equal(4, gpx);
        Assert.Equal(2, gpy);
    }

    [Fact]
    public void GetCachedBitmap_AlwaysNil_BecauseLoadDxBitmapIsEmpty()
    {
        var file = BuildWzlFile(new[] { MakeInfo(Dim(3, 8), 0, 0, 0) }, new[] { new byte[32] });
        var (wzl, _) = WriteWzl("bm", file, new[] { 64 });

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        // 原文 LoadDxBitmap（1004-1007）方法体为空 → Bitmap 槽位恒 nil，IndexList 不被写入
        Assert.False(images.GetCachedBitmap(0).Assigned);
        Assert.False(images.GetBitmap(0, out _, out _).Assigned);
        Assert.Empty(images.IndexList);
    }

    [Fact]
    public void Finalize_ReleasesTexturesAndResetsState()
    {
        var file = BuildWzlFile(new[] { MakeInfo(Dim(4, 4), 0, 0, 0) }, new[] { MakeFill(4 * 4, 9) });
        var (wzl, _) = WriteWzl("fin", file, new[] { 64 });

        var freed = new List<IntPtr>();
        TextureSeams.TextureFreeFn = h => freed.Add(h);

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();
        var tex = images.GetCachedImage(0, out _, out _);
        Assert.True(tex.Assigned);

        images.Finalize_();

        Assert.False(images.Initialized);
        Assert.Equal(0, images.ImageCount);
        Assert.Null(images.m_ImgArr);
        Assert.Null(images.m_FileStream);
        Assert.Single(freed);
        Assert.Equal(tex.Handle, freed[0]);
    }

    [Fact]
    public void Initialize_MissingFile_StaysUninitialized()
    {
        var images = new TWzlImages();
        images.FileName = Path.Combine(_dir, "nope.wzl");
        images.Initialize();

        Assert.False(images.Initialized);
        Assert.Null(images.m_FileStream);
        Assert.Equal(0, images.ImageCount);
    }

    [Fact]
    public void Initialize_ZeroImageCount_NoHeaderFailure()
    {
        // 条目数为 0：m_ImgArr 为空数组，LoadIndex 仍可读到 0 条索引
        var (wzl, _) = WriteWzl("zero", BuildWzlHeader(0), Array.Empty<int>());

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        Assert.True(images.Initialized);
        Assert.Equal(0, images.ImageCount);
        Assert.Empty(images.m_ImgArr!);
        Assert.Equal(0, images.IndexCount);
    }

    [Fact]
    public void Initialize_TruncatedFile_ImageCountZero()
    {
        // 文件头不足 64 字节：ReadFully 短读 → 余下补 0 → ImageCount = 0（不抛）
        var wzl = Path.Combine(_dir, "short.wzl");
        File.WriteAllBytes(wzl, new byte[20]);

        var images = new TWzlImages();
        images.FileName = wzl;
        images.Initialize();

        Assert.True(images.Initialized);
        Assert.Equal(0, images.ImageCount);
    }

    // ===================================================================================
    // 7. FileName → IndexFileName 推导（GameImages.pas 1109-1121 + 257-278）
    // ===================================================================================

    [Fact]
    public void SetFileName_DerivesWzxPath()
    {
        var images = new TWzlImages();
        images.FileName = @"D:\Mir\Data\ChrSel.Wzl";
        Assert.Equal(@"D:\Mir\Data\ChrSel.wzx", images.IndexFileName);

        images.FileName = @"Data\Prguse.WZL";
        Assert.Equal(@"Data\Prguse.wzx", images.IndexFileName);

        images.FileName = "noext";
        Assert.Equal("noext.wzx", images.IndexFileName);
    }

    [Fact]
    public void ExtractFilePath_And_FileNameOnly_MatchDelphiSemantics()
    {
        Assert.Equal(@"D:\Mir\Data\", TGameImages.ExtractFilePath(@"D:\Mir\Data\a.wzl"));
        Assert.Equal("", TGameImages.ExtractFilePath("a.wzl"));
        Assert.Equal("a", TGameImages.ExtractFileNameOnly("a.wzl"));
        Assert.Equal("ChrSel", TGameImages.ExtractFileNameOnly(@"D:\Mir\Data\ChrSel.Wzl"));
        Assert.Equal("noext", TGameImages.ExtractFileNameOnly("noext"));
    }

    // ===================================================================================
    // 8. 合成字节工具
    // ===================================================================================

    /// <summary>
    /// 有效几何 → (块偏移0 低/高字节 = 有效 nWidth, 块偏移4 低/高字节 = 有效 nHeight)。
    /// </summary>
    private static (byte B0, byte B1, byte B4, byte B5) Dim(int w, int h)
        => ((byte)(w & 0xFF), (byte)((w >> 8) & 0xFF), (byte)(h & 0xFF), (byte)((h >> 8) & 0xFF));

    private static byte[] MakeFill(int count, byte value)
    {
        var buf = new byte[count];
        for (int i = 0; i < count; i++) buf[i] = value;
        return buf;
    }

    /// <summary>
    /// 按 packed 15 字节块布局构造 ImageInfo。
    /// <para><b>原文怪癖</b>：原文的非 packed 视图实际读出的是
    /// <c>nWidth = 块[0..1]</c>、<c>nHeight = 块[4..5]</c>、<c>px = 块[0..3]</c>、<c>py = 块[8..11]</c>。</para>
    /// <para>故 <paramref name="dim"/> 直接给出**有效**的 (nWidth, nHeight)；
    /// <paramref name="effPx"/> / <paramref name="effPy"/> 给出**有效**的 px / py（分别落到块偏移 0..3 / 8..11 的低 16 位）。</para>
    /// </summary>
    private static byte[] MakeInfo((byte B0, byte B1, byte B4, byte B5) dim, short effPx, short effPy, int length)
    {
        var info = default(TWzlImageInfo);
        info.PixelFormat = dim.B0; // 块偏移 0 = 有效 nWidth 低字节
        info.bt2 = dim.B1;         // 块偏移 1 = 有效 nWidth 高字节
        info.bt3 = (byte)((effPx >> 8) & 0xFF);  // 块偏移 2 = 有效 px 高 16 位低半
        info.bt4 = (byte)((effPx >> 16) & 0xFF); // 块偏移 3 = 有效 px 高 16 位高半
        info.nWidth = (short)(dim.B4 | (dim.B5 << 8)); // 块偏移 4 = 有效 nHeight
        info.nHeight = 0;          // 块偏移 6（原文未使用）
        info.px = effPy;           // 块偏移 8 = 有效 py 低 16 位
        info.py = 0;               // 块偏移 10 = 有效 py 高 16 位
        info.Length = length;      // 块偏移 12

        // 真实 .wzl 里 ImageInfo 占 15 字节（原文 Position + SizeOf(TWzlImageInfo) = +15），
        // 数据块紧跟在块偏移 15 —— 即 Length 的第 4 字节与数据块首字节在同一位置。
        // ToBytes 需要 16 字节缓冲（Length 4 字节跨到偏移 15），故先给 16 再裁到 15。
        var buf = new byte[TWzlImageInfo.SizeOf + 1];
        info.ToBytes(buf, 0);
        Array.Resize(ref buf, TWzlImageInfo.SizeOf);
        return buf;
    }

    /// <summary>Length 越界用例：15 字节块里 Length 只有 3 字节（@12..14）可表达，写满 0xFF = 16777215 ≥ MAX_IMAGE_SIZE。</summary>
    private static byte[] MakeLengthOverflowInfo()
    {
        var buf = MakeInfo(Dim(3, 8), 0, 0, 0);
        buf[12] = 0xFF;
        buf[13] = 0xFF;
        buf[14] = 0xFF;
        return buf;
    }

    /// <summary>Wzl.pas 20-27 头（64 字节）。</summary>
    private static byte[] BuildWzlHeader(int imageCount)
    {
        var h = TWzlImageHeader.CreateEmpty();
        h.TitleText = "WEMADE Entertainment inc.";
        h.ImageCount = imageCount;
        h.ColorCount = 256;
        h.PaletteSize = 1024;
        h.VerFlag = 1;
        h.Flag = 0;
        var buf = new byte[TWzlImageHeader.SizeOf];
        h.ToBytes(buf, 0);
        return buf;
    }

    /// <summary>合成 .wzl：头 + 每图（15 字节 ImageInfo + 数据块）。</summary>
    private static byte[] BuildWzlFile(byte[][] infos, byte[][] payloads)
    {
        var list = new List<byte>();
        list.AddRange(BuildWzlHeader(infos.Length));
        for (int i = 0; i < infos.Length; i++)
        {
            list.AddRange(infos[i]);
            if (i < payloads.Length) list.AddRange(payloads[i]);
        }
        return list.ToArray();
    }

    /// <summary>合成 .wzx：ShortString 头（48 字节）+ IndexCount 个 Integer。</summary>
    private static byte[] BuildIndexBytes(IReadOnlyList<int> offsets)
    {
        var h = TWzlIndexHeader.CreateEmpty();
        h.TitleText = "WEMADE Entertainment inc.";
        h.IndexCount = offsets.Count;
        var head = new byte[TWzlIndexHeader.SizeOf];
        h.ToBytes(head, 0);

        var buf = new byte[TWzlIndexHeader.SizeOf + (offsets.Count * 4)];
        Array.Copy(head, buf, TWzlIndexHeader.SizeOf);
        for (int i = 0; i < offsets.Count; i++)
            BitConverter.GetBytes(offsets[i]).CopyTo(buf, TWzlIndexHeader.SizeOf + (i * 4));
        return buf;
    }

    private void WriteIndex(string path, IReadOnlyList<int> offsets)
        => File.WriteAllBytes(path, BuildIndexBytes(offsets));

    /// <summary>写 .wzl/.wzx 并返回路径（offsets 为图块在文件中的字节偏移）。</summary>
    private (string Wzl, string Wzx) WriteWzl(string name, byte[] wzlBytes, IReadOnlyList<int> offsets)
    {
        var wzl = Path.Combine(_dir, name + ".wzl");
        var wzx = Path.Combine(_dir, name + ".wzx");
        File.WriteAllBytes(wzl, wzlBytes);
        WriteIndex(wzx, offsets);
        return (wzl, wzx);
    }
}
