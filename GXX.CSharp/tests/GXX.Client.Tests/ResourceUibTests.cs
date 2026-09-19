using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GXX.Client.ReadResources;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次P1 / 车道 lane-resources：Uib.pas（503 行）1:1 移植的资源测试。
/// <para>UIB 不是容器格式：<c>TUibImages</c> 只维护一份**文件名列表**（顺序即索引），
/// 每张图按路径从磁盘单独加载。因此本文件的重点是列表/索引语义、缓存槽位与更新状态机。</para>
/// <para>位图加载经 <see cref="BitmapFileSeams"/> 接缝（默认实现解析最小 BMP），
/// 测试用**合成 BMP 字节**构造，不依赖仓库外资源。</para>
/// </summary>
public sealed class ResourceUibTests : IDisposable
{
    private readonly string _dir;

    public ResourceUibTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_p1_uib_" + Guid.NewGuid().ToString("N"));
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
        TGameImages.MyGetTickCountFn = () => 0x0003_0000u;
    }

    // ===================================================================================
    // 1. Initialize / Finalize（58-116）
    // ===================================================================================

    [Fact]
    public void Initialize_TakesImageCountFromFileList()
    {
        var images = new TUibImages();
        images.Initialize();
        Assert.True(images.Initialized);
        Assert.Equal(0, images.ImageCount);
        Assert.Equal(8, images.BitCount); // 原文 61：BitCount := 8
        Assert.Empty(images.m_ImgArr!);

        // 再调用不会重复初始化（原文 60：if not Initialized then）
        images.m_FileList.Add("a.bmp");
        images.Initialize();
        Assert.Equal(0, images.ImageCount);

        // Finalize 后可重新 Initialize，此时按列表长度分配
        images.Finalize_();
        Assert.False(images.Initialized);
        Assert.Null(images.m_ImgArr);
        Assert.Equal(0, images.ImageCount);

        images.Initialize();
        Assert.Equal(1, images.ImageCount);
        Assert.Single(images.m_ImgArr!);
    }

    [Fact]
    public void Finalize_ClearsIndexLists()
    {
        var bmp = WriteBmp("f1.bmp", 4, 2, 5);
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();
        images.GetCachedImage(0, out _, out _);
        Assert.Single(images.IndexList);

        images.Finalize_();

        Assert.Empty(images.IndexList);
        Assert.Empty(images.GrayIndexList);
        Assert.Empty(images.BrightIndexList);
        Assert.Null(images.m_ImgArr);
        Assert.Equal(0, images.ImageCount);
    }

    // ===================================================================================
    // 2. GetIndexByName（215-230）：顺序即索引 + 未命中追加
    // ===================================================================================

    [Fact]
    public void GetIndexByName_CaseInsensitive_And_AppendsOnMiss()
    {
        var images = new TUibImages();

        Assert.Equal(0, images.GetIndexByName("Data\\Prguse.bmp"));
        Assert.Equal(1, images.m_FileList.Count); // 追加 → 列表长度 1
        Assert.Equal(1, images.ImageCount);       // ImageCount := m_FileList.Count

        // 命中（大小写不敏感）→ 返回原下标，不再追加
        Assert.Equal(0, images.GetIndexByName("data\\PRGUSE.BMP"));
        Assert.Equal(1, images.m_FileList.Count);
        Assert.Equal(1, images.ImageCount); // ImageCount 被无条件覆写为 Count

        // 未命中 → 追加到末尾
        Assert.Equal(1, images.GetIndexByName("Data\\Title.bmp"));
        Assert.Equal(2, images.m_FileList.Count);
        Assert.Equal(2, images.ImageCount);
        Assert.Equal("Data\\Title.bmp", images.m_FileList[1]);

        // 顺序即索引：先来先得
        Assert.Equal(0, images.GetIndexByName("DATA\\prguse.BMP"));
        Assert.Equal(1, images.GetIndexByName("DATA\\TITLE.BMP"));
    }

    [Fact]
    public void GetIndexByName_EmptyListAddsFirst()
    {
        var images = new TUibImages();
        Assert.Equal(0, images.GetIndexByName("a"));
        Assert.Equal(1, images.ImageCount);
        Assert.Equal("a", images.m_FileList[0]);
    }

    // ===================================================================================
    // 3. LoadDx*（118-213）：文件不存在 / 加载失败 / 面积 <= 4 三条早退
    // ===================================================================================

    [Fact]
    public void LoadDxImage_MissingFile_LeavesSlotNil()
    {
        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxImage(Path.Combine(_dir, "nope.bmp"), slot);

        Assert.False(slot.Surface.Assigned);
        Assert.Equal(0u, slot.dwLatestTime);
    }

    [Fact]
    public void LoadDxImage_LoadFailure_LeavesSlotNil()
    {
        // BitmapFileSeams 返回 null = 原文 LoadFromFile 抛异常被 except 吞掉
        BitmapFileSeams.LoadFromFileFn = _ => null;
        var path = Path.Combine(_dir, "broken.bmp");
        File.WriteAllBytes(path, new byte[] { 1, 2, 3 });

        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxImage(path, slot);

        Assert.False(slot.Surface.Assigned);
        Assert.Equal(0u, slot.dwLatestTime); // 打点在面积判断之后，故未被更新
    }

    [Fact]
    public void LoadDxImage_AreaNotGreaterThan4_LeavesSlotNil()
    {
        // 原文 198：Source.Width * Source.Height > 4 —— 2x2 = 4 不满足
        var bmp = WriteBmp("tiny.bmp", 2, 2, 9);
        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxImage(bmp, slot);

        Assert.False(slot.Surface.Assigned);
        Assert.Equal(0u, slot.dwLatestTime);
    }

    [Fact]
    public void LoadDxImage_Valid_AssignsTextureAndTimestamp()
    {
        var bmp = WriteBmp("ok.bmp", 4, 2, 5);
        var images = new TUibImages();
        var slot = new TDxImage();
        images.LoadDxImage(bmp, slot);

        Assert.True(slot.Surface.Assigned);
        Assert.False(slot.Surface.IsNullTexture);
        Assert.Equal(0x0003_0000u, slot.dwLatestTime);

        var tex = TextureSeams.GetImage(slot.Surface.Handle)!;
        Assert.Equal(4, tex.Width);
        Assert.Equal(2, tex.Height);
    }

    [Fact]
    public void LoadDxGrayImage_And_BrightImage_AreIndependent()
    {
        var bmp = WriteBmp("gb.bmp", 4, 2, 6);
        var images = new TUibImages();
        var slot = new TDxImage();

        images.LoadDxGrayImage(bmp, slot);
        Assert.True(slot.Gray.Assigned);
        Assert.Equal(0x0003_0000u, slot.dwLatestGrayTime);
        Assert.Equal(0u, slot.dwLatestTime);
        Assert.Equal(0u, slot.dwLatestBrightTime);

        images.LoadDxBrightImage(bmp, slot);
        Assert.True(slot.Bright.Assigned);
        Assert.Equal(0x0003_0000u, slot.dwLatestBrightTime);
        Assert.NotEqual(slot.Gray.Handle, slot.Bright.Handle);
    }

    // ===================================================================================
    // 4. GetSurfaceByName（484-508）
    // ===================================================================================

    [Fact]
    public void GetSurfaceByName_NotInitialized_ReturnsNil()
    {
        var images = new TUibImages();
        Assert.False(images.GetSurfaceByName("x.bmp").Assigned);
    }

    [Fact]
    public void GetSurfaceByName_AddsToFileList_And_Loads()
    {
        var bmp = WriteBmp("named.bmp", 4, 2, 3);
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();

        var tex = images.GetSurfaceByName(bmp);
        Assert.True(tex.Assigned);
        Assert.Equal(0x0003_0000u, images.m_ImgArr![0].dwLatestTime);
        Assert.Empty(images.IndexList); // 原文 496 行把 IndexList.Add 注释掉了

        // 二次调用命中缓存
        var tex2 = images.GetSurfaceByName(bmp);
        Assert.Equal(tex, tex2);

        // 未登记的名字会被 GetIndexByName 追加进列表（m_FileList 变长），
        // 但 m_ImgArr 仍是 Initialize 时按旧长度分配的 → 无对应槽位 → nil（原文本处为越界读）
        var missing = images.GetSurfaceByName(Path.Combine(_dir, "missing.bmp"));
        Assert.False(missing.Assigned);
        Assert.Equal(2, images.m_FileList.Count);
        Assert.Equal(2, images.ImageCount);
        Assert.Single(images.m_ImgArr!); // 槽位数组未增长
    }

    // ===================================================================================
    // 5. 缓存访问（232-482）
    // ===================================================================================

    [Fact]
    public void GetCachedImage_LazyLoads_And_AlwaysAddsIndex()
    {
        var bmp = WriteBmp("c1.bmp", 4, 2, 11);
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();

        var tex = images.GetCachedImage(0, out int px, out int py);
        Assert.True(tex.Assigned);
        Assert.Equal(0, px);
        Assert.Equal(0, py);
        Assert.Single(images.IndexList); // 原文 342 行**无条件** IndexList.Add（在装载之前）
        Assert.Equal(0x0003_0000u, images.m_ImgArr![0].dwLatestTime);

        // 二次访问：命中缓存分支
        var tex2 = images.GetCachedImage(0, out _, out _);
        Assert.Equal(tex, tex2);
        Assert.Single(images.IndexList);

        // 越界与未初始化
        Assert.False(images.GetCachedImage(5, out _, out _).Assigned);
        Assert.False(images.GetCachedImage(-1, out _, out _).Assigned);
    }

    [Fact]
    public void GetCachedSurface_Gray_Bright_UseOwnSlots()
    {
        var bmp = WriteBmp("c2.bmp", 4, 2, 12);
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();

        var surface = images.GetCachedSurface(0);
        Assert.True(surface.Assigned);
        Assert.Single(images.IndexList);

        var gray = images.GetCachedGray(0);
        Assert.True(gray.Assigned);
        Assert.Single(images.GrayIndexList);

        var bright = images.GetCachedBright(0);
        Assert.True(bright.Assigned);
        Assert.Single(images.BrightIndexList);

        Assert.NotEqual(surface.Handle, gray.Handle);
        Assert.NotEqual(surface.Handle, bright.Handle);

        // 带 var 输出的变体走各自槽位
        var gray2 = images.GetCachedGrayImage(0, out int gpx, out int gpy);
        Assert.Equal(gray, gray2);
        Assert.Equal(0, gpx);
        Assert.Equal(0, gpy);
        var bright2 = images.GetCachedBrightImage(0, out int bpx, out int bpy);
        Assert.Equal(bright, bright2);
        Assert.Equal(0, bpx);
        Assert.Equal(0, bpy);
    }

    [Fact]
    public void GetCachedBright_WhenNotInitialized_ReturnsNil()
    {
        // 原文 298-329：GetCachedBright 的守卫在 Lock **之前**，未初始化时直接返回 nil
        var images = new TUibImages();
        Assert.False(images.GetCachedBright(0).Assigned);
    }

    [Fact]
    public void GetCachedImageSize_ReturnsTrueForValidIndex()
    {
        var bmp = WriteBmp("c3.bmp", 4, 2, 13);
        var images = new TUibImages();
        images.m_FileList.Add(bmp);
        images.Initialize();

        // 原文只把 Source.Width/Height 写进 TDIB，**不**回填 m_ImgArr[].nWidth/nHeight
        // → 该 API 恒返回 cx=0/cy=0（原文如此：HZQ 20230524 未验证实现）
        Assert.True(images.GetCachedImageSize(0, out int cx, out int cy, out int ptX, out int ptY));
        Assert.Equal(0, cx);
        Assert.Equal(0, cy);
        Assert.Equal(0, ptX);
        Assert.Equal(0, ptY);
        Assert.Single(images.IndexList);

        Assert.False(images.GetCachedImageSize(9, out _, out _, out _, out _));
        Assert.False(images.GetCachedImageSize(-1, out _, out _, out _, out _));
    }

    // ===================================================================================
    // 6. UpdateEngine 接缝（udtFileOther）与 StreamSaveToFile
    // ===================================================================================

    [Fact]
    public void GetCachedImage_FailedLoad_TriggersUpdateEngine()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent.bmp"));
        images.Initialize();

        var calls = new List<(string, int, int)>();
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (file, kind, index, _, _) =>
        {
            calls.Add((file, kind, index));
            return true;
        };

        images.GetCachedImage(0, out _, out _);

        Assert.Single(calls);
        Assert.Equal(3, calls[0].Item2); // udtFileOther
        Assert.Equal(0, calls[0].Item3);
        Assert.True(images.m_ImgArr![0].boUpdateStart);
    }

    [Fact]
    public void GetCachedImage_EmptyFileName_SkipsUpdateEngine()
    {
        var images = new TUibImages();
        images.m_FileList.Add("");
        images.Initialize();

        var calls = new List<string>();
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (file, _, _, _, _) => { calls.Add(file); return true; };

        images.GetCachedImage(0, out _, out _);

        Assert.Empty(calls); // 原文 350：if sFileName <> '' then
        Assert.False(images.m_ImgArr![0].boUpdateStart);
    }

    [Fact]
    public void GetCachedImage_UpdateAlreadyStarted_DoesNotRetrigger()
    {
        var images = new TUibImages();
        images.m_FileList.Add(Path.Combine(_dir, "absent2.bmp"));
        images.Initialize();
        images.m_ImgArr![0].boUpdateStart = true;

        int callCount = 0;
        TGameImages.g_boAutoUpdate = true;
        TGameImages.g_boDeviceInitializeOK = true;
        TGameImages.UpdateEngineAddFn = (_, _, _, _, _) => { callCount++; return true; };

        images.GetCachedImage(0, out _, out _);

        Assert.Equal(0, callCount); // 原文 347-348：(not boUpdateStop) and (not boUpdateStart)
    }

    [Fact]
    public void StreamSaveToFile_WritesFile_CreatesDirectory_And_ClearsFlag()
    {
        var images = new TUibImages();
        images.m_FileList.Add("x");
        images.Initialize();
        images.m_ImgArr![0].boUpdateStart = true;

        string target = Path.Combine(_dir, "sub", "deep", "uib_out.bmp");
        using var ms = new MemoryStream(new byte[] { 0x42, 0x4D, 1, 2, 3 });

        images.StreamSaveToFile(null, ms, 0, target);

        Assert.True(File.Exists(target));
        Assert.Equal(new byte[] { 0x42, 0x4D, 1, 2, 3 }, File.ReadAllBytes(target));
        Assert.False(images.m_ImgArr[0].boUpdateStart);
        Assert.False(images.m_ImgArr[0].boUpdateStop);
    }

    [Fact]
    public void StreamSaveToFile_NullStream_MarksUpdatesStopped()
    {
        var images = new TUibImages();
        images.m_FileList.Add("x");
        images.Initialize();
        images.m_ImgArr![0].boUpdateStart = true;
        images.m_ImgArr[0].boUpdateStop = false;

        images.StreamSaveToFile(null, null, 0, "");

        Assert.False(images.m_ImgArr[0].boUpdateStart);
        Assert.True(images.m_ImgArr[0].boUpdateStop);
    }

    [Fact]
    public void StreamSaveToFile_NullStream_InvalidIndex_NoThrow()
    {
        var images = new TUibImages();
        images.Initialize();

        images.StreamSaveToFile(null, null, 5, "");  // 越界：无操作
        images.StreamSaveToFile(null, null, -1, ""); // 负数：无操作
    }

    [Fact]
    public void StreamSaveToFile_UnwritableTarget_SwallowsException_AndClearsFlag()
    {
        var images = new TUibImages();
        images.m_FileList.Add("y");
        images.Initialize();
        images.m_ImgArr![0].boUpdateStart = true;

        // 目标是一个已存在的目录 → FileStream 创建失败 → 原文 except 吞掉
        string target = Path.Combine(_dir, "adir");
        Directory.CreateDirectory(target);

        using var ms = new MemoryStream(new byte[] { 1 });
        images.StreamSaveToFile(null, ms, 0, target);

        Assert.False(images.m_ImgArr[0].boUpdateStart); // 标志仍被清掉
    }

    // ===================================================================================
    // 7. 合成 BMP 工具
    // ===================================================================================

    /// <summary>
    /// 写一个最小 24bpp BMP（BITMAPFILEHEADER 14 + BITMAPINFOHEADER 40 + 像素，行按 4 字节对齐）。
    /// 每行填充同一调色板索引 <paramref name="value"/>，便于断言取值。
    /// </summary>
    private string WriteBmp(string name, int width, int height, byte value)
    {
        int rowBytes = ((width * 24) + 31) / 32 * 4;
        int dataSize = rowBytes * height;
        int fileSize = 54 + dataSize;
        var buf = new byte[fileSize];

        buf[0] = (byte)'B';
        buf[1] = (byte)'M';
        BitConverter.GetBytes(fileSize).CopyTo(buf, 2);
        BitConverter.GetBytes(54).CopyTo(buf, 10);          // bfOffBits

        BitConverter.GetBytes(40).CopyTo(buf, 14);          // biSize
        BitConverter.GetBytes(width).CopyTo(buf, 18);       // biWidth
        BitConverter.GetBytes(height).CopyTo(buf, 22);      // biHeight（正 = 自下而上）
        BitConverter.GetBytes((short)1).CopyTo(buf, 26);    // biPlanes
        BitConverter.GetBytes((short)24).CopyTo(buf, 28);   // biBitCount
        BitConverter.GetBytes(dataSize).CopyTo(buf, 34);    // biSizeImage

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int off = 54 + (y * rowBytes) + (x * 3);
                buf[off + 0] = value;
                buf[off + 1] = value;
                buf[off + 2] = value;
            }

        string path = Path.Combine(_dir, name);
        File.WriteAllBytes(path, buf);
        return path;
    }
}
