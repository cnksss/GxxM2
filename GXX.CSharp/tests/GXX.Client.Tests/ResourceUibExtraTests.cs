using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GXX.Client.ReadResources;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次P2 / 车道 p2-resources-uib：Uib.pas（503 行）1:1 移植的补充测试。
/// <para>与 ResourceUibTests 互补，补齐：像素级解码、LoadDx* 的正向分支、D3DFormat 路径、
/// Gray/Bright/ImageSize 的 UpdateEngine 接缝、以及 GetCachedSurface/Gray/Bright 的越界与未初始化早退。</para>
/// </summary>
public sealed class ResourceUibExtraTests : IDisposable
{
    private readonly string _dir;

    public ResourceUibExtraTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_p2_uib_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        ResetGlobals();
    }

    public void Dispose()
    {
        TextureSeams.ResetForTest();
        BitmapFileSeams.ResetForTest();
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
        TGameImages.MyGetTickCountFn = () => 0x0004_0000u;
    }

    // ===================================================================================
    // 1. 位图装载：像素级断言（8bpp 索引经 g_DefColorTable 展开）
    // ===================================================================================

    [Fact]
    public void LoadDxImage_DecodesPaletteIndexesRowMajor()
    {
        // 3x2 8bpp：像素 1,2,3 / 4,5,6；BMP 行按 4 字节对齐（3→4），最后 1 字节为对齐填充
        var bmp = WriteBmp8("px.bmp", 3, 2, new byte[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
        });

        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxImage(bmp, slot);

        Assert.True(slot.Surface.Assigned);
        var tex = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(3, tex.Width);
        Assert.Equal(2, tex.Height);
        Assert.Equal(GDefColorTable.GetARGB32(1), tex.GetPixel(0, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(3), tex.GetPixel(2, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(4), tex.GetPixel(0, 1).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(6), tex.GetPixel(2, 1).ToArgb());
    }

    [Fact]
    public void LoadFromFile_BottomUpBmp_IsFlippedToTopDown()
    {
        // BMP 默认自下而上：文件里第 0 行数据是图像的**最后一行**。
        // 用 3x2（面积 6 > 4，满足原文 198 行守卫）便于观察行序。
        var bmp = WriteBmp8("flip.bmp", 3, 2, new byte[,]
        {
            { 100, 100, 100 }, // 图像第 0 行（在文件里排第 2 行）
            { 200, 200, 200 }, // 图像第 1 行（在文件里排第 1 行）
        });

        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxImage(bmp, slot);

        Assert.True(slot.Surface.Assigned);
        var tex = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(GDefColorTable.GetARGB32(100), tex.GetPixel(0, 0).ToArgb());
        Assert.Equal(GDefColorTable.GetARGB32(200), tex.GetPixel(0, 1).ToArgb());
    }

    [Fact]
    public void LoadFromFile_NonBmpSignature_ReturnsNull()
    {
        // BitmapFileSeams 默认实现对非 BM 头返回 null（原文 LoadFromFile 抛异常 → except）
        string path = Path.Combine(_dir, "notbmp.bmp");
        File.WriteAllBytes(path, new byte[64]); // 全 0，无 'BM'

        Assert.Null(BitmapFileSeams.LoadFromFileFn(path));
    }

    [Fact]
    public void LoadFromFile_TooShort_ReturnsNull()
    {
        string path = Path.Combine(_dir, "short.bmp");
        File.WriteAllBytes(path, new byte[] { (byte)'B', (byte)'M', 0, 0 });

        Assert.Null(BitmapFileSeams.LoadFromFileFn(path));
    }

    // ===================================================================================
    // 2. LoadDx* 正向分支 + D3DFormat
    // ===================================================================================

    [Fact]
    public void LoadDxGrayImage_And_BrightImage_AssignOwnTimestamps()
    {
        var bmp = WriteBmp8("gb.bmp", 3, 2, new byte[,]
        {
            { 7, 7, 7 },
            { 7, 7, 7 },
        });

        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxGrayImage(bmp, slot);

        Assert.True(slot.Gray.Assigned);
        Assert.False(slot.Gray.IsNullTexture);
        Assert.Equal(0x0004_0000u, slot.dwLatestGrayTime);
        Assert.Equal(0u, slot.dwLatestTime);
        Assert.Equal(0u, slot.dwLatestBrightTime);

        images.LoadDxBrightImage(bmp, slot);
        Assert.True(slot.Bright.Assigned);
        Assert.Equal(0x0004_0000u, slot.dwLatestBrightTime);
        // 灰/亮/常三种是**不同**纹理对象
        Assert.NotEqual(slot.Gray.Handle, slot.Bright.Handle);
    }

    [Fact]
    public void LoadDxImage_D3DFormatLargeImage_UsesSameSeam()
    {
        // 原文 201：D3DFormat and Width >= 400 and Height >= 400 → FileData32 + NewTexture(FileData, ...)
        // 本车道两条分支都走 TextureSeams.NewTextureFn（FileData32 为接缝），故 D3DFormat 只改路径不改结果形状
        var bmp = WriteBmp8("big.bmp", 400, 400, FillGrid(400, 400, 9));
        var images = new TUibImages { D3DFormat = true };
        var slot = new TDxImage();
        images.LoadDxImage(bmp, slot);

        Assert.True(slot.Surface.Assigned);
        var tex = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(400, tex.Width);
        Assert.Equal(400, tex.Height);
    }

    // ===================================================================================
    // 3. 缓存入口的越界 / 未初始化早退
    // ===================================================================================

    [Fact]
    public void CacheAccessors_EarlyExitWhenNotInitialized()
    {
        var images = new TUibImages();

        Assert.False(images.GetCachedSurface(0).Assigned);
        Assert.False(images.GetCachedGray(0).Assigned);
        Assert.False(images.GetCachedBright(0).Assigned);
        Assert.False(images.GetCachedImage(0, out _, out _).Assigned);
        Assert.False(images.GetCachedGrayImage(0, out _, out _).Assigned);
        Assert.False(images.GetCachedBrightImage(0, out _, out _).Assigned);
        Assert.False(images.GetCachedImageSize(0, out _, out _, out _, out _));
    }

    [Fact]
    public void CacheAccessors_EarlyExitOnOutOfRangeIndex()
    {
        var bmp = WriteBmp8("one.bmp", 3, 2, FillGrid(3, 2, 1));
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();

        Assert.False(images.GetCachedSurface(1).Assigned);
        Assert.False(images.GetCachedGray(-1).Assigned);
        Assert.False(images.GetCachedBright(9).Assigned);
        Assert.False(images.GetCachedImage(-1, out _, out _).Assigned);
        Assert.False(images.GetCachedGrayImage(9, out _, out _).Assigned);
        Assert.False(images.GetCachedBrightImage(-1, out _, out _).Assigned);
        Assert.False(images.GetCachedImageSize(9, out _, out _, out _, out _));
        Assert.Empty(images.IndexList);
        Assert.Empty(images.GrayIndexList);
        Assert.Empty(images.BrightIndexList);
    }

    [Fact]
    public void CachedBranches_SecondCall_HitsCacheInsteadOfReloading()
    {
        var bmp = WriteBmp8("two.bmp", 3, 2, FillGrid(3, 2, 2));
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();

        var s1 = images.GetCachedSurface(0);
        var s2 = images.GetCachedSurface(0);
        Assert.Equal(s1, s2);
        Assert.Single(images.IndexList);

        var g1 = images.GetCachedGray(0);
        var g2 = images.GetCachedGray(0);
        Assert.Equal(g1, g2);
        Assert.Single(images.GrayIndexList);

        var b1 = images.GetCachedBright(0);
        var b2 = images.GetCachedBright(0);
        Assert.Equal(b1, b2);
        Assert.Single(images.BrightIndexList);
    }

    // ===================================================================================
    // 4. UpdateEngine 接缝：Gray / Bright / ImageSize 三条路径
    // ===================================================================================

    [Fact]
    public void GetCachedGray_FailedLoad_TriggersUpdateEngine()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent_gray.bmp"));
        images.Initialize();

        var calls = new List<(string, int, int)>();
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (file, kind, index, _, _) =>
        {
            calls.Add((file, kind, index));
            return true;
        };

        images.GetCachedGray(0);

        Assert.Single(calls);
        Assert.Equal(3, calls[0].Item2); // udtFileOther
        Assert.True(images.m_ImgArr![0].boUpdateStart);
    }

    [Fact]
    public void GetCachedBright_FailedLoad_TriggersUpdateEngine()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent_bright.bmp"));
        images.Initialize();

        var calls = new List<int>();
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (_, kind, index, _, _) => { calls.Add(kind); return true; };

        images.GetCachedBright(0);

        Assert.Single(calls);
        Assert.Equal(3, calls[0]);
        Assert.True(images.m_ImgArr![0].boUpdateStart);
    }

    [Fact]
    public void GetCachedImageSize_FailedLoad_TriggersUpdateEngine()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent_size.bmp"));
        images.Initialize();

        var calls = new List<int>();
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (_, kind, index, _, _) => { calls.Add(index); return true; };

        Assert.True(images.GetCachedImageSize(0, out int cx, out int cy, out int ptX, out int ptY));
        Assert.Equal(0, cx);
        Assert.Equal(0, cy);
        Assert.Single(calls);
        Assert.Equal(0, calls[0]);
        Assert.True(images.m_ImgArr![0].boUpdateStart);
    }

    [Fact]
    public void UpdateEngine_NotAccepted_LeavesFlagFalse()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent_rej.bmp"));
        images.Initialize();

        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (_, _, _, _, _) => false; // 引擎拒绝受理

        images.GetCachedImage(0, out _, out _);

        Assert.False(images.m_ImgArr![0].boUpdateStart);
    }

    [Fact]
    public void UpdateEngine_DeviceNotReady_Skipped()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent_dev.bmp"));
        images.Initialize();

        int callCount = 0;
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = false; // 设备未就绪
        TGameImages.UpdateEngineAddFn = (_, _, _, _, _) => { callCount++; return true; };

        images.GetCachedImage(0, out _, out _);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void UpdateEngine_UpdateStopSet_Skipped()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent_stop.bmp"));
        images.Initialize();
        images.m_ImgArr![0].boUpdateStop = true;

        int callCount = 0;
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (_, _, _, _, _) => { callCount++; return true; };

        images.GetCachedImage(0, out _, out _);

        Assert.Equal(0, callCount);
    }

    // ===================================================================================
    // 5. 合成 BMP 工具
    // ===================================================================================

    private static byte[,] FillGrid(int width, int height, byte value)
    {
        var g = new byte[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                g[y, x] = value;
        return g;
    }

    /// <summary>
    /// 写 8bpp BMP：14 + 40 头 + 1024 字节调色板 + 逐行像素（行按 4 字节对齐）。
    /// <paramref name="rows"/>[y, x] = 图像第 y 行第 x 列的调色板索引；文件里按 BMP 约定自下而上存放。
    /// </summary>
    private string WriteBmp8(string name, int width, int height, byte[,] rows)
    {
        int rowBytes = ((width * 8) + 31) / 32 * 4;
        int dataSize = rowBytes * height;
        int paletteSize = 256 * 4;
        int offBits = 14 + 40 + paletteSize;
        int fileSize = offBits + dataSize;
        var buf = new byte[fileSize];

        buf[0] = (byte)'B';
        buf[1] = (byte)'M';
        BitConverter.GetBytes(fileSize).CopyTo(buf, 2);
        BitConverter.GetBytes(offBits).CopyTo(buf, 10);

        BitConverter.GetBytes(40).CopyTo(buf, 14);        // biSize
        BitConverter.GetBytes(width).CopyTo(buf, 18);     // biWidth
        BitConverter.GetBytes(height).CopyTo(buf, 22);    // biHeight（正 = 自下而上）
        BitConverter.GetBytes((short)1).CopyTo(buf, 26);  // biPlanes
        BitConverter.GetBytes((short)8).CopyTo(buf, 28);  // biBitCount
        BitConverter.GetBytes(dataSize).CopyTo(buf, 34);  // biSizeImage
        BitConverter.GetBytes(256).CopyTo(buf, 46);       // biClrUsed

        // 调色板：B,G,R,0 × 256（与 g_DefColorTable 同布局）
        for (int i = 0; i < 256; i++)
        {
            buf[54 + (i * 4) + 0] = GDefColorTable.ColorArray[(i * 4) + 0];
            buf[54 + (i * 4) + 1] = GDefColorTable.ColorArray[(i * 4) + 1];
            buf[54 + (i * 4) + 2] = GDefColorTable.ColorArray[(i * 4) + 2];
            buf[54 + (i * 4) + 3] = 0;
        }

        for (int y = 0; y < height; y++)
        {
            int srcRow = height - 1 - y; // 自下而上
            for (int x = 0; x < width; x++)
                buf[offBits + (y * rowBytes) + x] = rows[srcRow, x];
        }

        string path = Path.Combine(_dir, name);
        File.WriteAllBytes(path, buf);
        return path;
    }
}
