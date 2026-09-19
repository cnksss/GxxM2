using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Uib.pas（503 行）1:1 移植 —— UI 位图库（按**文件名列表**组织的磁盘位图集合，非容器文件）。
//
// 原文没有任何容器格式解析：TUibImages.Initialize（58-67）只把 m_FileList.Count 当作 ImageCount，
// 每张图按 m_FileList.Strings[Index] 给出的路径从磁盘单独 LoadFromFile（TDIB.LoadFromFile）。
// 因此本单元的保真点是：
//   1) m_FileList 的**顺序即索引**、GetIndexByName 的“未命中则追加”语义（215-230）；
//   2) Initialize / Finalize 对 m_ImgArr 与三个 IndexList 的处置；
//   3) 缓存访问（Surface/Gray/Bright/Image/ImageSize）里的守卫顺序与打点时机；
//   4) StreamSaveToFile 的落盘与 boUpdateStart / boUpdateStop 状态翻转。
// TDIB.LoadFromFile / NewTexture* / UpdateEngine 均按车道约定改为接缝
// （见 BitmapFileSeams / TextureSeams / TGameImages.UpdateEngineAddFn），不顺手移植。
// =====================================================================================

/// <summary>
/// TDIB.LoadFromFile（DIB.pas）在本车道的**最小接缝**：
/// 输入磁盘位图路径，输出位图（即原文 <c>Source:TDIB</c> 中本车道用到的那部分）。
/// <para>接缝：待 DIB.pas 移植后接入真实实现（当前默认实现解析最小 BMP 头 + 像素）。</para>
/// </summary>
public static class BitmapFileSeams
{
    /// <summary>
    /// 对应原文 <c>Source := TDIB.Create; Source.LoadFromFile(AFileName);</c> 这一对调用。
    /// 返回 null 表示加载失败（原文 <c>except Source.Free; Exit;</c>）。
    /// </summary>
    public static Func<string, TDib?> LoadFromFileFn = DefaultLoadFromFile;

    /// <summary>
    /// 默认实现：解析 BMP（BITMAPFILEHEADER 14 + BITMAPINFOHEADER 40）得到宽高与位深，
    /// 构造 TDib 并逐行读入像素。非 BMP、头不足或行数据越界返回 null
    /// （对应原文 LoadFromFile 抛异常 → except 分支）。
    /// </summary>
    public static TDib? DefaultLoadFromFile(string fileName)
    {
        try
        {
            if (!File.Exists(fileName)) return null;
            byte[] bytes = File.ReadAllBytes(fileName);
            if (bytes.Length < 54) return null;
            if (bytes[0] != (byte)'B' || bytes[1] != (byte)'M') return null; // 'BM'

            int dataOffset = BitConverter.ToInt32(bytes, 10);
            int width = BitConverter.ToInt32(bytes, 18);
            int rawHeight = BitConverter.ToInt32(bytes, 22); // 负值 = 自顶向下
            short bitCount = BitConverter.ToInt16(bytes, 28);
            bool topDown = rawHeight < 0;
            int absHeight = Math.Abs(rawHeight);
            if (width <= 0 || absHeight <= 0) return null;
            if (bitCount != 8 && bitCount != 16 && bitCount != 24 && bitCount != 32) return null;

            var dib = TDib.Create();
            dib.SetSize(width, absHeight, bitCount);

            int rowBytes = ((width * bitCount) + 31) / 32 * 4;
            int copyBytes = Math.Min(rowBytes, dib.WidthBytes);
            for (int y = 0; y < absHeight; y++)
            {
                int srcRow = topDown ? y : (absHeight - 1 - y); // BMP 默认自下而上
                int srcOff = dataOffset + (srcRow * rowBytes);
                if (srcOff < 0 || srcOff + copyBytes > bytes.Length) break;
                var line = new byte[copyBytes];
                Array.Copy(bytes, srcOff, line, 0, copyBytes);
                dib.WriteScanLine(y, line, 0, copyBytes);
            }
            // 8bit 的调色板由 TextureSeams / g_DefColorTable 承担（与原文 Move(g_DefColorTable, ...) 一致）
            return dib;
        }
        catch
        {
            return null; // 原文 LoadFromFile 失败 → except → Source.Free; Exit
        }
    }

    /// <summary>测试用复位。</summary>
    public static void ResetForTest() => LoadFromFileFn = DefaultLoadFromFile;
}

/// <summary>
/// Uib.pas 14-38 <c>TUibImages = class(TGameImages)</c> 1:1 移植。
/// </summary>
public sealed class TUibImages : TGameImages
{
    /// <summary>Uib.pas 15：<c>m_FileList:TStringList;</c>（顺序即索引）。</summary>
    public TStringList m_FileList = new();

    public TUibImages()
    {
        m_FileList = new TStringList();
    }

    /// <summary>GameImages.pas 1118-1120：Uib 不属于 TWMImages / TWzlImages → IndexFileName = FileName。</summary>
    protected override string IndexFileExtension => "";

    // =============================== Initialize / Finalize（58-116） ===============================

    /// <summary>
    /// Uib.pas 58-67 Initialize 1:1：
    /// <c>if not Initialized then begin BitCount := 8; ImageCount := m_FileList.Count;
    /// m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount); Initialized := True; end;</c>
    /// <para>注意：不检查 FileName、不检查文件存在性（与 Wzl/Wis 不同）。</para>
    /// </summary>
    public override void Initialize()
    {
        if (!Initialized)
        {
            BitCount = 8;
            ImageCount = m_FileList.Count;
            // FileName := 'Data\';   ← 原文 63 行注释
            m_ImgArr = new TDxImage[ImageCount];
            // AllocMem 的等价物：Delphi 里 TDXImage 是值类型数组（AllocMem 已清零），
            // C# 里 TDXImage 是引用类型 → 必须逐项实例化。
            for (int I = 0; I < m_ImgArr.Length; I++) m_ImgArr[I] = new TDxImage();
            Initialized = true;
        }
    }

    /// <summary>Uib.pas 69-116 Finalize 1:1。</summary>
    public override void Finalize_()
    {
        Initialized = false;
        Lock();
        try
        {
            IndexList.Clear();
            GrayIndexList.Clear();
            BrightIndexList.Clear();
            FreeImageArray();
            ImageCount = 0;
        }
        finally
        {
            UnLock();
        }
    }

    // =============================== 文件名 → 索引（215-230） ===============================

    /// <summary>
    /// Uib.pas 215-230 GetIndexByName 1:1：
    /// 先按 <c>LowerCase</c> 全表线性查找；未命中则 <c>m_FileList.Add(Name)</c> 并返回新下标；
    /// 最后**无条件** <c>ImageCount := m_FileList.Count</c>（含命中分支）。
    /// </summary>
    public int GetIndexByName(string name)
    {
        int Result = -1;
        for (int I = 0; I <= m_FileList.Count - 1; I++)
        {
            if (DelphiRTL.LowerCase(name) == DelphiRTL.LowerCase(m_FileList[I]))
            {
                Result = I; // Integer(m_FileList.Objects[I]);   ← 原文 222 行注释
                break;
            }
        }
        if (Result < 0)
        {
            m_FileList.Add(name);
            Result = m_FileList.Count - 1;
        }
        ImageCount = m_FileList.Count;
        return Result;
    }

    /// <summary>Uib.pas 37：<c>property Names[Name:string]:TTexture read GetSurfaceByName;</c>。</summary>
    public TTextureRef GetSurfaceByName(string name)
    {
        TTextureRef Result = default;
        if ((m_FileList == null) || (!Initialized)) return Result;
        int Index = GetIndexByName(name);
        if (Index < 0) return Result;
        // 安全边界：GetIndexByName 可能把新名字追加进 m_FileList，使其长度超过 Initialize 时分配的 m_ImgArr。
        // 原文会越界读 m_ImgArr[Index]（未定义行为）；此处以返回 nil 表达「该槽位不存在」。
        if (m_ImgArr == null || Index >= m_ImgArr.Length) return Result;

        Lock();
        try
        {
            if (!m_ImgArr[Index].Surface.Assigned)
            {
                // IndexList.Add(Pointer(Index));   ← 原文 496 行注释（本分支刻意不加）
                LoadDxImage(name, m_ImgArr[Index]);
                m_ImgArr[Index].dwLatestTime = MyGetTickCount();
                Result = m_ImgArr[Index].Surface;
            }
            else
            {
                m_ImgArr[Index].dwLatestTime = MyGetTickCount();
                Result = m_ImgArr[Index].Surface;
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    // =============================== LoadDx*（118-213） ===============================

    /// <summary>
    /// Uib.pas 118-148 LoadDxBrightImage 1:1。
    /// <para>顺序：FileExists → TDIB.Create + LoadFromFile（失败则 Source.Free + Exit）→
    /// <c>(Source &lt;&gt; nil) and (Source.Width * Source.Height &gt; 4)</c> → 打点 dwLatestBrightTime →
    /// D3D 大图路径 / NewTextureBright。</para>
    /// </summary>
    public void LoadDxBrightImage(string aFileName, TDxImage dXImage)
    {
        if (File.Exists(aFileName))
        {
            TDib? Source = BitmapFileSeams.LoadFromFileFn(aFileName);
            if (Source == null) return; // 原文 except Source.Free; Exit;

            if ((Source != null) && (Source.Width * Source.Height > 4))
            {
                dXImage.dwLatestBrightTime = MyGetTickCount();

                if (D3DFormat && (Source.Width >= 400) && (Source.Height >= 400))
                {
                    // 接缝：FileDataBright32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(Source, null));
                }
                else
                {
                    dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(Source, null));
                }
            }
            // Source.Free;  ← 托管对象，无显式释放
        }
    }

    /// <summary>Uib.pas 150-181 LoadDxGrayImage 1:1。</summary>
    public void LoadDxGrayImage(string aFileName, TDxImage dXImage)
    {
        if (File.Exists(aFileName))
        {
            TDib? Source = BitmapFileSeams.LoadFromFileFn(aFileName);
            if (Source == null) return;

            if ((Source != null) && (Source.Width * Source.Height > 4))
            {
                dXImage.dwLatestGrayTime = MyGetTickCount();

                if (D3DFormat && (Source.Width >= 400) && (Source.Height >= 400))
                {
                    // 接缝：FileDataGray32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(Source, null));
                }
                else
                {
                    dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(Source, null));
                }
            }
        }
    }

    /// <summary>Uib.pas 183-213 LoadDxImage 1:1。</summary>
    public void LoadDxImage(string aFileName, TDxImage dXImage)
    {
        if (File.Exists(aFileName))
        {
            TDib? Source = BitmapFileSeams.LoadFromFileFn(aFileName);
            if (Source == null) return;

            if ((Source != null) && (Source.Width * Source.Height > 4))
            {
                dXImage.dwLatestTime = MyGetTickCount();

                if (D3DFormat && (Source.Width >= 400) && (Source.Height >= 400))
                {
                    // 接缝：FileData32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(Source, null));
                }
                else
                {
                    dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(Source, null));
                }
            }
        }
    }

    // =============================== 缓存访问（232-482） ===============================

    /// <summary>Uib.pas 232-263 GetCachedSurface 1:1（含 UpdateEngine(udtFileOther) 接缝）。</summary>
    public TTextureRef GetCachedSurface(int index)
    {
        TTextureRef Result = default;
        Lock();
        try
        {
            if ((index >= 0) && (index < ImageCount) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    IndexList.Add(index);
                    LoadDxImage(m_FileList[index], m_ImgArr[index]);
                    Result = m_ImgArr[index].Surface;
                    if (g_boAutoUpdate && (!Result.Assigned) && (!m_ImgArr[index].boUpdateStop) && (!m_ImgArr[index].boUpdateStart)
                        && g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[index];
                        if (sFileName != "")
                        {
                            // 接缝：g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile)
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, index, null, null))
                                m_ImgArr[index].boUpdateStart = true;
                        }
                    }
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    Result = m_ImgArr[index].Surface;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Uib.pas 265-296 GetCachedGray 1:1。</summary>
    public TTextureRef GetCachedGray(int index)
    {
        TTextureRef Result = default;
        Lock();
        try
        {
            if ((index >= 0) && (index < ImageCount) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    GrayIndexList.Add(index);
                    LoadDxGrayImage(m_FileList[index], m_ImgArr[index]);
                    Result = m_ImgArr[index].Gray;
                    if (g_boAutoUpdate && (!Result.Assigned) && (!m_ImgArr[index].boUpdateStop) && (!m_ImgArr[index].boUpdateStart)
                        && g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[index];
                        if (sFileName != "")
                        {
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, index, null, null))
                                m_ImgArr[index].boUpdateStart = true;
                        }
                    }
                }
                else
                {
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    Result = m_ImgArr[index].Gray;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Uib.pas 298-329 GetCachedBright 1:1（**守卫在 Lock 之前** —— 照抄结构）。</summary>
    public TTextureRef GetCachedBright(int index)
    {
        TTextureRef Result = default;
        if ((index >= 0) && (index < ImageCount) && Initialized)
        {
            Lock();
            try
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    BrightIndexList.Add(index);
                    LoadDxBrightImage(m_FileList[index], m_ImgArr[index]);
                    Result = m_ImgArr[index].Bright;
                    if (g_boAutoUpdate && (!Result.Assigned) && (!m_ImgArr[index].boUpdateStop) && (!m_ImgArr[index].boUpdateStart)
                        && g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[index];
                        if (sFileName != "")
                        {
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, index, null, null))
                                m_ImgArr[index].boUpdateStart = true;
                        }
                    }
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    Result = m_ImgArr[index].Bright;
                }
            }
            finally
            {
                UnLock();
            }
        }
        return Result;
    }

    /// <summary>Uib.pas 331-366 GetCachedImage 1:1（带 var PX, PY）。</summary>
    public TTextureRef GetCachedImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        Lock();
        try
        {
            if ((index >= 0) && (index < ImageCount) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    IndexList.Add(index);
                    LoadDxImage(m_FileList[index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Surface;
                    if (g_boAutoUpdate && (!Result.Assigned) && (!m_ImgArr[index].boUpdateStop)
                        && (!m_ImgArr[index].boUpdateStart) && g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[index];
                        if (sFileName != "")
                        {
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, index, null, null))
                            {
                                m_ImgArr[index].boUpdateStart = true;
                            }
                        }
                    }
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Surface;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>
    /// Uib.pas 368-407 GetCachedImageSize 1:1。
    /// <para>原文行内注释：<c>//HZQ 20230524尝试添加了这个函数，未经过验证，程序中也未使用</c> —— 照抄。
    /// 它把尺寸写进 <c>ASize</c> / <c>APoint</c>，但来源是 <c>m_ImgArr[AIndex]</c> 的 nWidth/nHeight/nPx/nPy ——
    /// 而 LoadDxImage 并不会回填这四个字段，故这两项恒为 0（原文如此）。</para>
    /// </summary>
    public bool GetCachedImageSize(int aIndex, out int cx, out int cy, out int pointX, out int pointY)
    {
        bool Result = false;
        cx = 0;
        cy = 0;
        pointX = 0;
        pointY = 0;
        Lock();
        try
        {
            if ((aIndex >= 0) && (aIndex < ImageCount) && Initialized)
            {
                FreeOldMemorys_Ex();
                if (!m_ImgArr![aIndex].Surface.Assigned)
                {
                    IndexList.Add(aIndex);
                    LoadDxImage(m_FileList[aIndex], m_ImgArr[aIndex]);
                    pointX = m_ImgArr[aIndex].nPx;
                    pointY = m_ImgArr[aIndex].nPy;
                    cx = m_ImgArr[aIndex].nWidth;
                    cy = m_ImgArr[aIndex].nHeight;
                    Result = true;
                    if (g_boAutoUpdate && (m_ImgArr[aIndex].Surface == TTextureRef.Nil)
                        && (!m_ImgArr[aIndex].boUpdateStop) && (!m_ImgArr[aIndex].boUpdateStart) && g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[aIndex];
                        if (sFileName != "")
                        {
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, aIndex, null, null))
                            {
                                m_ImgArr[aIndex].boUpdateStart = true;
                            }
                        }
                    }
                }
                else
                {
                    m_ImgArr[aIndex].dwLatestTime = MyGetTickCount();
                    pointX = m_ImgArr[aIndex].nPx;
                    pointY = m_ImgArr[aIndex].nPy;
                    cx = m_ImgArr[aIndex].nWidth;
                    cy = m_ImgArr[aIndex].nHeight;
                    Result = true;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Uib.pas 409-444 GetCachedGrayImage 1:1（带 var PX, PY）。</summary>
    public TTextureRef GetCachedGrayImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        Lock();
        try
        {
            if ((index >= 0) && (index < ImageCount) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    GrayIndexList.Add(index);
                    LoadDxGrayImage(m_FileList[index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Gray;
                    if (g_boAutoUpdate && (!Result.Assigned) && (!m_ImgArr[index].boUpdateStop) && (!m_ImgArr[index].boUpdateStart) &&
                        g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[index];
                        if (sFileName != "")
                        {
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, index, null, null))
                                m_ImgArr[index].boUpdateStart = true;
                        }
                    }
                }
                else
                {
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Gray;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Uib.pas 446-482 GetCachedBrightImage 1:1（带 var PX, PY）。</summary>
    public TTextureRef GetCachedBrightImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        Lock();
        try
        {
            if ((index >= 0) && (index < ImageCount) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    BrightIndexList.Add(index);
                    LoadDxBrightImage(m_FileList[index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Bright;

                    if (g_boAutoUpdate && (!Result.Assigned) && (!m_ImgArr[index].boUpdateStop) && (!m_ImgArr[index].boUpdateStart) &&
                        g_boDeviceInitializeOK)
                    {
                        string sFileName = m_FileList[index];
                        if (sFileName != "")
                        {
                            if (UpdateEngineAddFn != null && UpdateEngineAddFn(sFileName, 3 /* udtFileOther */, index, null, null))
                                m_ImgArr[index].boUpdateStart = true;
                        }
                    }
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Bright;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    // =============================== StreamSaveToFile（510-543） ===============================

    /// <summary>
    /// Uib.pas 510-543 StreamSaveToFile 1:1（UpdateEngine 下载完成回调）：
    /// Stream 非空 → 建目录 + SaveToFile（异常吞掉）→ Index 合法则 boUpdateStart := False → Stream.Free；
    /// Stream 为空 → Index 合法则 boUpdateStart := False **且** boUpdateStop := True。
    /// </summary>
    public void StreamSaveToFile(object? sender, MemoryStream? stream, int index, string fileName)
    {
        Lock();
        try
        {
            if (stream != null)
            {
                string filePath = ExtractFilePath(fileName);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                try
                {
                    using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                    stream.Position = 0;
                    stream.CopyTo(fs);
                }
                catch
                {
                    // 原文 523-527：try Stream.SaveToFile(FileName) except end;（吞异常）
                }

                if ((index >= 0) && (index < ImageCount))
                    m_ImgArr![index].boUpdateStart = false;

                // Stream.Free;  ← 托管对象，无显式释放
            }
            else
            {
                if ((index >= 0) && (index < ImageCount))
                {
                    m_ImgArr![index].boUpdateStart = false;
                    m_ImgArr![index].boUpdateStop = true;
                }
            }
        }
        finally
        {
            UnLock();
        }
    }
}
