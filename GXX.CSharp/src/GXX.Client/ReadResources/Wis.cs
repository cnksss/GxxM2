using System;
using System.IO;
using System.Runtime.InteropServices;
using GXX.Core.Rtl;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Wis.pas（996 行）1:1 移植 —— WIS 图库（固定 512 字节文件头 + 文件尾倒序索引）。
//
// 原文 USEMAPSTREAM = 0（GameImages.pas 14），故 LoadDx* 走 TFileStream 顺序读分支；
// 但 TWisImages.LoadIndex（445-495）**无条件**使用 TMapStream（内存映射），这是原文的实际写法，
// 本移植以「一次性读入 byte[] 并按下标寻址」等价表达（TMapStream.Memory + Size 的语义）。
// =====================================================================================

/// <summary>
/// Wis.pas 15-31 <c>TWisFileHeaderInfo</c>（**packed**，512 字节）。
/// <para>字节偏移（与原文行内注释 <c>// 04 / 0XA0 / 0XA4 / 0XA8 / 0XAC</c> 自洽）：
/// nTitle@0, VerFlag@4, Reserve1@8, DateTime@12(8 字节 double), Reserve2@20, Reserve3@24,
/// CopyRight@28(string[20] = 21 字节), aTemp1@49(107 字节),
/// nHeaderEncrypt@156(0x9C，原文注释写 0XA0), nHeaderLen@160(0xA0), nImageCount@164(0xA4),
/// nHeaderData@168(0xA8), aTemp2@172(0xAC, 341 字节) → 共 512（= $200）。</para>
/// <para><b>原文缺陷</b>：行内注释的十六进制偏移整体比真实偏移大 4（0XA0 应为 0X9C … ），
/// 且 <c>aTemp2:array[1..$200 - $AC]</c> 按常量表达式是 512-172 = 340 字节，
/// 加上它总长会是 513；要凑满 $200 应为 341 字节。此处取 341 以保证 SizeOf = 512。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TWisFileHeaderInfo
{
    /// <summary>WISA = $41534957（小端读作 0x57495341）。</summary>
    public int nTitle;

    public int VerFlag;
    public int Reserve1;

    /// <summary>Delphi TDateTime（自 1899-12-30 起的双精度天数）—— 以原始 double 保存，字节一致。</summary>
    public double DateTime;

    public int Reserve2;
    public int Reserve3;

    /// <summary>CopyRight:string[20]（长度字节 + 20 字节 GBK）。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
    public byte[] CopyRight;

    /// <summary>aTemp1:array[1..107] of Char。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 107)]
    public byte[] aTemp1;

    /// <summary>nHeaderEncrypt（真实偏移 156 = 0X9C）。</summary>
    public int nHeaderEncrypt;

    /// <summary>nHeaderLen（真实偏移 160 = 0XA0）。</summary>
    public int nHeaderLen;

    /// <summary>nImageCount（真实偏移 164 = 0XA4）。</summary>
    public int nImageCount;

    /// <summary>nHeaderData（真实偏移 168 = 0XA8）。</summary>
    public int nHeaderData;

    /// <summary>aTemp2:array[1..341] of Char（真实偏移 172 = 0XAC；见类型注释的原文缺陷说明）。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 341)]
    public byte[] aTemp2;

    /// <summary>Delphi <c>SizeOf(TWisFileHeaderInfo)</c> = 512（= $200）。</summary>
    public const int SizeOf = 512;

    public string CopyRightText
    {
        get => ShortStringLayout.GetString(CopyRight, 0, 20);
        set => ShortStringLayout.SetBytes(CopyRight, 0, 20, value);
    }

    public static TWisFileHeaderInfo CreateEmpty()
    {
        var h = default(TWisFileHeaderInfo);
        h.CopyRight = new byte[21];
        h.aTemp1 = new byte[107];
        h.aTemp2 = new byte[341];
        return h;
    }
}

/// <summary>
/// Wis.pas 33-39 <c>TWisHeader = packed record OffSet:Integer; Length:Integer; temp3:Integer; end;</c>
/// <para>12 字节，位于文件尾部的倒序索引区。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TWisHeader
{
    public int OffSet;
    public int Length;
    public int temp3;

    public const int SizeOf = 12;

    public static TWisHeader FromBytes(byte[] buf, int offset)
    {
        var h = default(TWisHeader);
        h.OffSet = BitConverter.ToInt32(buf, offset);
        h.Length = BitConverter.ToInt32(buf, offset + 4);
        h.temp3 = BitConverter.ToInt32(buf, offset + 8);
        return h;
    }

    public readonly void ToBytes(byte[] buf, int offset)
    {
        BitConverter.GetBytes(OffSet).CopyTo(buf, offset);
        BitConverter.GetBytes(Length).CopyTo(buf, offset + 4);
        BitConverter.GetBytes(temp3).CopyTo(buf, offset + 8);
    }
}

/// <summary>
/// Wis.pas 41-51 <c>TImgInfo = packed record</c>（12 字节）：
/// btEncr0@0, btEncr1@1, bt2@2, bt3@3, wW@4, wH@6, wPx@8, wPy@10。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TImgInfo
{
    /// <summary>0X00：<c>btEncr0</c>（= 1 表示数据经 DecodeWis 压缩）。</summary>
    public byte btEncr0;

    /// <summary>0X01。</summary>
    public byte btEncr1;

    /// <summary>0X02。</summary>
    public byte bt2;

    /// <summary>0X03。</summary>
    public byte bt3;

    /// <summary>0X04：<c>wW:Smallint</c>。</summary>
    public short wW;

    /// <summary>0X06：<c>wH:Smallint</c>。</summary>
    public short wH;

    /// <summary>0X08：<c>wPx:Smallint</c>。</summary>
    public short wPx;

    /// <summary>0X0A：<c>wPy:Smallint</c>。</summary>
    public short wPy;

    public const int SizeOf = 12;

    public static TImgInfo FromBytes(byte[] buf, int offset)
    {
        var i = default(TImgInfo);
        i.btEncr0 = buf[offset + 0];
        i.btEncr1 = buf[offset + 1];
        i.bt2 = buf[offset + 2];
        i.bt3 = buf[offset + 3];
        i.wW = BitConverter.ToInt16(buf, offset + 4);
        i.wH = BitConverter.ToInt16(buf, offset + 6);
        i.wPx = BitConverter.ToInt16(buf, offset + 8);
        i.wPy = BitConverter.ToInt16(buf, offset + 10);
        return i;
    }

    public readonly void ToBytes(byte[] buf, int offset)
    {
        buf[offset + 0] = btEncr0;
        buf[offset + 1] = btEncr1;
        buf[offset + 2] = bt2;
        buf[offset + 3] = bt3;
        BitConverter.GetBytes(wW).CopyTo(buf, offset + 4);
        BitConverter.GetBytes(wH).CopyTo(buf, offset + 6);
        BitConverter.GetBytes(wPx).CopyTo(buf, offset + 8);
        BitConverter.GetBytes(wPy).CopyTo(buf, offset + 10);
    }

    /// <summary>原文 <c>FileStream.Read(ImgInfo, SizeOf(ImgInfo))</c> 的等价顺序读。</summary>
    public static TImgInfo ReadFrom(Stream stream)
    {
        var buf = new byte[SizeOf];
        int read = ReadFully(stream, buf, 0, SizeOf);
        if (read < SizeOf)
            throw new EndOfStreamException($"TImgInfo 读取不足：{read}/{SizeOf}");
        return FromBytes(buf, 0);
    }

    internal static int ReadFully(Stream stream, byte[] buf, int offset, int count)
    {
        int total = 0;
        while (total < count)
        {
            int n = stream.Read(buf, offset + total, count - total);
            if (n <= 0) break;
            total += n;
        }
        return total;
    }
}

/// <summary>
/// Wis.pas 53-90 <c>TWisImages = class(TGameImages)</c> 1:1 移植。
/// </summary>
public sealed class TWisImages : TGameImages
{
    /// <summary>Wis.pas 55：<c>FIndexOffset:Integer;</c>（倒序索引区起始偏移）。</summary>
    public int FIndexOffset;

    /// <summary>Wis.pas 57/59：<c>FileStream:TFileStream</c>（USEMAPSTREAM=0 分支）。</summary>
    public FileStream? FileStream;

    /// <summary>Wis.pas 61：<c>IndexArray:TWisFileHeaderArray;</c>（Delphi 动态数组）。</summary>
    public TWisHeader[]? IndexArray;

    /// <summary>Wis.pas 456：LoadIndex 内部 TMapStream.Memory + Size 的等价（整文件镜像）。</summary>
    public byte[]? MapMemory;

    /// <summary>Wis.pas 15-31 的 512 字节头（原文 LoadIndex 未读它，仅由格式定义；此处保留以便核对/生成）。</summary>
    public TWisFileHeaderInfo FileHeader;

    /// <summary>原文 512：<c>iFileOffset := 512;</c>（索引倒序扫描的下界）。</summary>
    public const int FILE_HEADER_SIZE = 512;

    public TWisImages()
    {
        FileStream = null;
        IndexArray = null;
    }

    /// <summary>GameImages.pas 1112-1114：TWMImages → .wix；Wis 不属于该判定 → IndexFileName = FileName。</summary>
    protected override string IndexFileExtension => "";

    // =============================== Initialize / Finalize（374-443） ===============================

    /// <summary>
    /// Wis.pas 374-390 Initialize 1:1：
    /// <c>if not Initialized then if FileExists(FileName) then if LoadIndex then begin
    /// FileStream := TFileStream.Create(...); m_ImgArr := AllocMem(...); Initialized := True; end;</c>
    /// <para>注意：原文**先** LoadIndex（内部自建 TMapStream 读整文件），成功后才开 FileStream。</para>
    /// </summary>
    public override void Initialize()
    {
        if (!Initialized)
        {
            if (File.Exists(FileName))
            {
                if (LoadIndex())
                {
                    FileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    m_ImgArr = new TDxImage[ImageCount];
                    for (int i = 0; i < m_ImgArr.Length; i++) m_ImgArr[i] = new TDxImage();
                    Initialized = true;
                }
            }
        }
    }

    /// <summary>Wis.pas 392-443 Finalize 1:1。</summary>
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
            IndexArray = null;
            ImageCount = 0;
            if (FileStream != null)
            {
                FileStream.Dispose();
                FileStream = null;
            }
        }
        finally
        {
            UnLock();
        }
    }

    // =============================== LoadIndex（445-495） ===============================

    /// <summary>
    /// Wis.pas 445-495 LoadIndex 1:1（含内嵌 <c>DecPointer</c>）。
    /// <para>算法：把整文件映射进内存，从 Size 往 512 方向**倒序**逐个读 12 字节 TWisHeader：
    /// 合法（OffSet &gt;= 512 且 Length &gt;= 1）就收集，遇到 <c>OffSet &lt;= 512</c> 时记下 FIndexOffset 并停；
    /// 非法则立即停。最后把收集到的顺序反转成 IndexArray（使 index 0 = 文件里最靠前的图）。</para>
    /// <para>原文的 <c>WisHeader := Pointer(Integer(MapStream.Memory) + MapStream.Size)</c> 起始于文件末尾之后的 1 字节，
    /// 首次循环先 Dec(SizeOf) 再取，故第一个读的是文件最后 12 字节 —— 本移植以 <c>nIndexOffset</c> 下标等价表达。</para>
    /// </summary>
    public bool LoadIndex()
    {
        bool Result = false;

        MapMemory = File.Exists(FileName) ? File.ReadAllBytes(FileName) : null;
        if (MapMemory != null)
        {
            int iFileOffset = FILE_HEADER_SIZE;
            int nIndexOffset = MapMemory.Length;
            var wisIndexArray = new System.Collections.Generic.List<TWisHeader>();

            while (true)
            {
                if (nIndexOffset > iFileOffset)
                {
                    nIndexOffset -= TWisHeader.SizeOf; // Dec(nIndexOffset, SizeOf(TWisHeader))
                    var wisHeader = TWisHeader.FromBytes(MapMemory, nIndexOffset);
                    if ((wisHeader.OffSet >= iFileOffset) && (wisHeader.Length >= 1))
                    {
                        wisIndexArray.Add(wisHeader);
                        if (wisHeader.OffSet <= iFileOffset)
                        {
                            FIndexOffset = nIndexOffset;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    FIndexOffset = nIndexOffset;
                    break;
                }
            }

            // 反转：IndexArray[nIndex] := WisIndexArray[Length - nIndex - 1]
            IndexArray = new TWisHeader[wisIndexArray.Count];
            for (int nIndex = 0; nIndex <= wisIndexArray.Count - 1; nIndex++)
                IndexArray[nIndex] = wisIndexArray[wisIndexArray.Count - nIndex - 1];

            ImageCount = IndexArray.Length;

            Result = true;
        }
        return Result;
    }

    /// <summary>Wis.pas 490：<c>ImageCount := Length(IndexArray);</c>。</summary>
    public int IndexCount => IndexArray?.Length ?? 0;

    // =============================== RLE 解码器 ===============================

    /// <summary>
    /// Wis.pas 95-127 <c>SGL_RLE8_Decode</c> 1:1（**原文定义后从未调用**，照抄保留）。
    /// <para>语义：<c>ASrc^ and $80 = 0</c> → 字面量段（L := ASrc^ 字节数，然后 L 个字节）；
    /// 否则 → 游程段（L := ASrc^ and $7F 个重复字节）。原文的 ASrcSize 递减量在两种分支下并不严格
    /// 对应实际消耗字节（字面量段 Dec(2) 却读了 L+1 字节）——照抄不修。</para>
    /// </summary>
    public static bool SGL_RLE8_Decode(byte[] aSrc, int srcOffset, int aSrcSize, byte[] aDst, int dstOffset, int aDstSize)
    {
        int src = srcOffset;
        int dst = dstOffset;
        bool result = true;
        while ((aSrcSize > 0) && (aDstSize > 0))
        {
            if ((ReadByte(aSrc, src) & 0x80) == 0) // 0..127
            {
                byte L = ReadByte(aSrc, src);
                src++;
                aSrcSize -= 2;
                if (L > aDstSize) L = (byte)aDstSize; // Delphi: if L > ADstSize then L := ADstSize（隐式截断）
                aDstSize -= L;
                for (int I = 1; I <= L; I++)
                {
                    WriteByte(aDst, dst, ReadByte(aSrc, src));
                    dst++;
                }
                src++;
            }
            else
            {
                byte L = (byte)(ReadByte(aSrc, src) & 0x7F);
                src++;
                aSrcSize -= L + 1;
                if (L > aDstSize) L = (byte)aDstSize; // Delphi: if L > ADstSize then L := ADstSize（隐式截断）
                aDstSize -= L;
                for (int I = 1; I <= L; I++)
                {
                    WriteByte(aDst, dst, ReadByte(aSrc, src));
                    dst++;
                    src++;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Wis.pas 218-266 <c>DecodeWis(ASrc, ADst, ASrcSize, ADstSize)</c> 1:1（扁平版）。
    /// <para>编码：<c>0x00 &lt;Len&gt; &lt;Len 个字节&gt;</c> 为字面量段；<c>&lt;L&gt; &lt;V&gt;</c> 为把 V 重复 L 次。
    /// 原文的 <c>V</c> 在循环外被复用，boSkip/Len 状态跨迭代保留 —— 逐步照抄。</para>
    /// </summary>
    public static bool DecodeWis(byte[] aSrc, int srcOffset, byte[] aDst, int dstOffset, int aSrcSize, int aDstSize)
    {
        byte V = 0, Len = 0, L;
        bool boSkip = false;
        int src = srcOffset;
        int dst = dstOffset;
        while ((aSrcSize > 0) && (aDstSize > 0))
        {
            if (!boSkip)
            {
                V = ReadByte(aSrc, src);
                aSrcSize--;
                src++;
            }
            if ((V == 0) && (Len <= 0))
            {
                V = ReadByte(aSrc, src);
                Len = V;
                aSrcSize--;
                src++;
                boSkip = true;
            }
            if (boSkip)
            {
                if (Len != 0)
                {
                    aDstSize -= Len;
                    aSrcSize -= Len;
                    // Move(ASrc^, ADst^, Len)
                    for (int i = 0; i < Len; i++) WriteByte(aDst, dst + i, ReadByte(aSrc, src + i));
                    dst += Len;
                    src += Len;
                }
                else
                {
                    boSkip = false;
                }
                Len = 0;
            }
            else
            {
                L = V;
                aSrcSize--;
                aDstSize -= L;
                V = ReadByte(aSrc, src);
                src++;
                for (int I = 1; I <= L; I++)
                {
                    WriteByte(aDst, dst, V);
                    dst++;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Wis.pas 268-360 <c>DecodeWis(ASrc, ASrcSize, AWidth, AHeight, Source)</c> 1:1（按行回绕版）。
    /// <para>内嵌 <c>WriteData</c> / <c>WritePixel</c> 的边界行为逐条照抄：
    /// 写满一行就把 ADst 移到下一行 ScanLine[nHeight]；越界（nHeight &gt;= AHeight）后**静默丢弃**。</para>
    /// </summary>
    public static bool DecodeWis(byte[] aSrc, int srcOffset, int aSrcSize, int aWidth, int aHeight, TDib source)
    {
        byte V = 0, Len = 0, L;
        bool boSkip = false;
        int src = srcOffset;
        int nWidth = 0;
        int nHeight = 0;
        int aDstSize = aWidth * aHeight;
        int dst = source.ScanLineOffset(0);

        void WriteData(int bitsOffset, int writeLen)
        {
            if (nHeight < aHeight)
            {
                if (nWidth + writeLen <= aWidth)
                {
                    for (int i = 0; i < writeLen; i++) WriteByte(source.Bits, dst + i, ReadByte(aSrc, bitsOffset + i));
                    dst += writeLen;
                    nWidth += writeLen;
                }
                if (nWidth >= aWidth)
                {
                    nHeight++;
                    nWidth = 0;
                    if (nHeight < aHeight)
                        dst = source.ScanLineOffset(nHeight);
                }
            }
        }

        void WritePixel(byte color)
        {
            if (nHeight < aHeight)
            {
                if (nWidth + 1 <= aWidth)
                {
                    WriteByte(source.Bits, dst, color);
                    dst++;
                    nWidth++;
                }
                if (nWidth >= aWidth)
                {
                    nHeight++;
                    nWidth = 0;
                    if (nHeight < aHeight)
                        dst = source.ScanLineOffset(nHeight);
                }
            }
        }

        while ((aSrcSize > 0) && (aDstSize > 0))
        {
            if (!boSkip)
            {
                V = ReadByte(aSrc, src);
                aSrcSize--;
                src++;
            }
            if ((V == 0) && (Len <= 0))
            {
                V = ReadByte(aSrc, src);
                Len = V;
                aSrcSize--;
                src++;
                boSkip = true;
            }
            if (boSkip)
            {
                if (Len != 0)
                {
                    aDstSize -= Len;
                    aSrcSize -= Len;
                    WriteData(src, Len);
                    src += Len;
                }
                else
                {
                    boSkip = false;
                }
                Len = 0;
            }
            else
            {
                L = V;
                aSrcSize--;
                aDstSize -= L;
                V = ReadByte(aSrc, src);
                src++;
                for (int I = 1; I <= L; I++)
                {
                    WritePixel(V);
                }
            }
        }

        return true;
    }

    // =============================== LoadDx*（761-1091） ===============================

    /// <summary>
    /// Wis.pas 761-811 LoadDxBitmap 1:1。
    /// <para>原文此方法**没有** btEncr0 压缩分支：非压缩时逐行 <c>FileStream.Read(Source.ScanLine[I]^, nWidth)</c>。</para>
    /// </summary>
    public void LoadDxBitmap(TWisHeader wisHeader, TDxImage dXImage)
    {
        if (FileStream == null) return;
        if ((wisHeader.OffSet > 0) && (wisHeader.OffSet < FileStream.Length))
        {
            FileStream.Position = wisHeader.OffSet;
            var ImgInfo = TImgInfo.ReadFrom(FileStream);
            dXImage.nWidth = (ushort)ImgInfo.wW;
            dXImage.nHeight = (ushort)ImgInfo.wH;
            dXImage.nPx = ImgInfo.wPx;
            dXImage.nPy = ImgInfo.wPy;
            dXImage.dwLatestTime = MyGetTickCount();
            int nSize = ImgInfo.wW * ImgInfo.wH;

            int nWidth = ImgInfo.wW;
            int nHeight = ImgInfo.wH;
            if ((nSize > 4) && (nSize < 999999))
            {
                var source = MakeDib(nWidth, nHeight);
                if (ImgInfo.btEncr0 == 1)
                {
                    var s = new byte[Math.Max(wisHeader.Length, 0)];
                    TImgInfo.ReadFully(FileStream, s, 0, s.Length);
                    DecodeWis(s, 0, wisHeader.Length, nWidth, nHeight, source);
                }
                else
                {
                    for (int I = 0; I <= source.Height - 1; I++)
                    {
                        var line = new byte[Math.Max(nWidth, 0)];
                        TImgInfo.ReadFully(FileStream, line, 0, line.Length);
                        source.WriteScanLine(I, line, 0, line.Length);
                    }
                }

                // 原文 803-806：DXImage.Bitmap := TBitmap.Create + Canvas.Draw(Source)
                // 接缝：TBitmap 待 Graphics 层移植后接入（本移植把 8bit 平面保留在 nWidth/nHeight 里）。
                // 原文如此（Wis.pas:803-806）：Bitmap 走 TBitmap，本车道只做资源格式解析。
            }
        }
    }

    /// <summary>Wis.pas 813-902 LoadDxImage 1:1。</summary>
    public void LoadDxImage(TWisHeader wisHeader, TDxImage dXImage)
    {
        if (FileStream == null) return;
        if ((wisHeader.OffSet > 0) && (wisHeader.OffSet < FileStream.Length))
        {
            FileStream.Position = wisHeader.OffSet;
            var ImgInfo = TImgInfo.ReadFrom(FileStream);

            dXImage.nWidth = (ushort)ImgInfo.wW;
            dXImage.nHeight = (ushort)ImgInfo.wH;
            dXImage.nPx = ImgInfo.wPx;
            dXImage.nPy = ImgInfo.wPy;
            dXImage.dwLatestTime = MyGetTickCount();
            int nSize = ImgInfo.wW * ImgInfo.wH;

            int nWidth = ImgInfo.wW;
            int nHeight = ImgInfo.wH;
            if ((nSize > 4) && (nSize < 999999))
            {
                var source = MakeDib(nWidth, nHeight);
                if (ImgInfo.btEncr0 == 1)
                {
                    var s = new byte[Math.Max(wisHeader.Length, 0)];
                    TImgInfo.ReadFully(FileStream, s, 0, s.Length);
                    DecodeWis(s, 0, wisHeader.Length, nWidth, nHeight, source);
                }
                else
                {
                    // 原文 869-872（USEMAPSTREAM = 0 分支）：
                    //   for I := 0 to Source.Height - 1 do begin
                    //     Move(SrcP^, Source.ScanLine[I]^, nWidth);      ← SrcP 此分支**未初始化**（原文缺陷）
                    //     FileStream.Read(Source.ScanLine[I]^, nWidth);  ← 真正生效的是这一句
                    //   end;
                    // 原文如此（Wis.pas:869-872）：Move 读的是未初始化的 SrcP，本移植略去该无效 Move（不改字节结果）。
                    for (int I = 0; I <= source.Height - 1; I++)
                    {
                        var line = new byte[Math.Max(nWidth, 0)];
                        TImgInfo.ReadFully(FileStream, line, 0, line.Length);
                        source.WriteScanLine(I, line, 0, line.Length);
                    }
                }

                if (D3DFormat && (source.Width >= 400) && (source.Height >= 400))
                {
                    // 接缝：FileData32(Source, FileData, FileSize) 与 D3D 纹理路径待 GameImages.pas 移植后接入
                    dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(source, null));
                }
                else
                {
                    dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(source, null));
                }

                // Source.Free;  ← 托管对象，无显式释放
            }

            if (!dXImage.Surface.Assigned)
            {
                dXImage.dwLatestTime = MyGetTickCount();
                dXImage.nPx = ImgInfo.wPx;
                dXImage.nPy = ImgInfo.wPy; // 原文如此（Wis.pas:898）：<c>DXImage.nPy := ImgInfo.wpy;</c> 小写字段名
                dXImage.Surface = TTextureRef.NullTextureSentinel;
            }
        }
    }

    /// <summary>Wis.pas 904-996 LoadDxGrayImage 1:1。</summary>
    public void LoadDxGrayImage(TWisHeader wisHeader, TDxImage dXImage)
    {
        if (FileStream == null) return;
        if ((wisHeader.OffSet > 0) && (wisHeader.OffSet < FileStream.Length))
        {
            FileStream.Position = wisHeader.OffSet;
            var ImgInfo = TImgInfo.ReadFrom(FileStream);

            dXImage.nWidth = (ushort)ImgInfo.wW;
            dXImage.nHeight = (ushort)ImgInfo.wH;
            dXImage.nPx = ImgInfo.wPx;
            dXImage.nPy = ImgInfo.wPy;
            dXImage.dwLatestGrayTime = MyGetTickCount();
            int nSize = ImgInfo.wW * ImgInfo.wH;

            int nWidth = ImgInfo.wW;
            int nHeight = ImgInfo.wH;
            if ((nSize > 4) && (nSize < 999999))
            {
                var source = MakeDib(nWidth, nHeight);
                if (ImgInfo.btEncr0 == 1)
                {
                    var s = new byte[Math.Max(wisHeader.Length, 0)];
                    TImgInfo.ReadFully(FileStream, s, 0, s.Length);
                    DecodeWis(s, 0, wisHeader.Length, nWidth, nHeight, source);
                }
                else
                {
                    for (int I = 0; I <= source.Height - 1; I++)
                    {
                        var line = new byte[Math.Max(nWidth, 0)];
                        TImgInfo.ReadFully(FileStream, line, 0, line.Length);
                        source.WriteScanLine(I, line, 0, line.Length);
                    }
                }

                if (D3DFormat && (source.Width >= 400) && (source.Height >= 400))
                {
                    // 接缝：FileDataGray32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(source, null));
                }
                else
                {
                    dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(source, null));
                }

                // Source.Free;  ← 托管对象，无显式释放
            }

            if (!dXImage.Gray.Assigned)
            {
                dXImage.dwLatestGrayTime = MyGetTickCount();
                dXImage.nPx = ImgInfo.wPx;
                dXImage.nPy = ImgInfo.wPy; // 原文如此（Wis.pas:992）：<c>DXImage.nPy := ImgInfo.wpy;</c>
                dXImage.Gray = TTextureRef.NullTextureSentinel;
            }
        }
    }

    /// <summary>Wis.pas 998-1091 LoadDxBrightImage 1:1。</summary>
    public void LoadDxBrightImage(TWisHeader wisHeader, TDxImage dXImage)
    {
        if (FileStream == null) return;
        if ((wisHeader.OffSet > 0) && (wisHeader.OffSet < FileStream.Length))
        {
            FileStream.Position = wisHeader.OffSet;
            var ImgInfo = TImgInfo.ReadFrom(FileStream);

            dXImage.nWidth = (ushort)ImgInfo.wW;
            dXImage.nHeight = (ushort)ImgInfo.wH;
            dXImage.nPx = ImgInfo.wPx;
            dXImage.nPy = ImgInfo.wPy;
            dXImage.dwLatestBrightTime = MyGetTickCount();
            int nSize = ImgInfo.wW * ImgInfo.wH;

            int nWidth = ImgInfo.wW;
            int nHeight = ImgInfo.wH;
            if ((nSize > 4) && (nSize < 999999))
            {
                var source = MakeDib(nWidth, nHeight);
                if (ImgInfo.btEncr0 == 1)
                {
                    var s = new byte[Math.Max(wisHeader.Length, 0)];
                    TImgInfo.ReadFully(FileStream, s, 0, s.Length);
                    DecodeWis(s, 0, wisHeader.Length, nWidth, nHeight, source);
                }
                else
                {
                    for (int I = 0; I <= source.Height - 1; I++)
                    {
                        var line = new byte[Math.Max(nWidth, 0)];
                        TImgInfo.ReadFully(FileStream, line, 0, line.Length);
                        source.WriteScanLine(I, line, 0, line.Length);
                    }
                }

                if (D3DFormat && (source.Width >= 400) && (source.Height >= 400))
                {
                    // 接缝：FileDataBright32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(source, null));
                }
                else
                {
                    dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(source, null));
                }

                // Source.Free;  ← 托管对象，无显式释放
            }

            if (!dXImage.Bright.Assigned)
            {
                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = ImgInfo.wPx;
                dXImage.nPy = ImgInfo.wPy; // 原文如此（Wis.pas:1086）：<c>DXImage.nPy := ImgInfo.wpy;</c>
                dXImage.Bright = TTextureRef.NullTextureSentinel;
            }
        }
    }

    // =============================== 缓存访问（497-759） ===============================

    /// <summary>Wis.pas 497-526 GetCachedImage 1:1（带 var PX, PY）。</summary>
    public TTextureRef GetCachedImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    LoadDxImage(IndexArray![index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();

                    if (m_ImgArr[index].Surface.Assigned)
                        IndexList.Add(index);

                    Result = m_ImgArr[index].Surface;
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

    /// <summary>Wis.pas 659-683 GetCachedSurface 1:1。</summary>
    public TTextureRef GetCachedSurface(int index)
    {
        TTextureRef Result = default;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    LoadDxImage(IndexArray![index], m_ImgArr[index]);
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    if (m_ImgArr[index].Surface.Assigned)
                        IndexList.Add(index);

                    Result = m_ImgArr[index].Surface;
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

    /// <summary>Wis.pas 685-707 GetCachedBitmap 1:1。</summary>
    public void GetCachedBitmap(int index)
    {
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                if (!m_ImgArr![index].Bitmap.Assigned)
                {
                    LoadDxBitmap(IndexArray![index], m_ImgArr[index]);

                    if (m_ImgArr[index].Bitmap.Assigned)
                        IndexList.Add(index);
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                }
            }
        }
        finally
        {
            UnLock();
        }
    }

    /// <summary>Wis.pas 709-734 GetCachedBright 1:1。</summary>
    public TTextureRef GetCachedBright(int index)
    {
        TTextureRef Result = default;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    LoadDxBrightImage(IndexArray![index], m_ImgArr[index]);
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();

                    if (m_ImgArr[index].Bright.Assigned)
                        BrightIndexList.Add(index);

                    Result = m_ImgArr[index].Bright;
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
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

    /// <summary>Wis.pas 736-759 GetCachedGray 1:1。</summary>
    public TTextureRef GetCachedGray(int index)
    {
        TTextureRef Result = default;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    LoadDxGrayImage(IndexArray![index], m_ImgArr[index]);
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    if (m_ImgArr[index].Gray.Assigned)
                        GrayIndexList.Add(index);
                    Result = m_ImgArr[index].Gray;
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

    /// <summary>Wis.pas 631-657 GetBitmap 1:1。</summary>
    public void GetBitmap(int index, out int px, out int py)
    {
        px = 0;
        py = 0;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                if (!m_ImgArr![index].Bitmap.Assigned)
                {
                    LoadDxBitmap(IndexArray![index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;

                    if (m_ImgArr[index].Bitmap.Assigned)
                        IndexList.Add(index);
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                }
            }
        }
        finally
        {
            UnLock();
        }
    }

    /// <summary>Wis.pas 569-598 GetCachedBrightImage 1:1。</summary>
    public TTextureRef GetCachedBrightImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    LoadDxBrightImage(IndexArray![index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();

                    if (m_ImgArr[index].Bright.Assigned)
                        BrightIndexList.Add(index);

                    Result = m_ImgArr[index].Bright;
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

    /// <summary>Wis.pas 600-629 GetCachedGrayImage 1:1。</summary>
    public TTextureRef GetCachedGrayImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        Lock();
        try
        {
            if (Initialized && (index >= 0) && (index < ImageCount))
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    LoadDxGrayImage(IndexArray![index], m_ImgArr[index]);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();

                    if (m_ImgArr[index].Gray.Assigned)
                        GrayIndexList.Add(index);

                    Result = m_ImgArr[index].Gray;
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

    /// <summary>Wis.pas 528-567 GetCachedImageSize 1:1。</summary>
    public bool GetCachedImageSize(int aIndex, out int cx, out int cy, out int pointX, out int pointY)
    {
        bool Result = false;
        cx = 0;
        cy = 0;
        pointX = 0;
        pointY = 0;
        if ((aIndex >= 0) && (aIndex < ImageCount) && (aIndex < m_IndexList.Count) && (FileStream != null) && Initialized)
        {
            if (m_ImgArr![aIndex].nWidth * m_ImgArr[aIndex].nHeight == 0)
            {
                if ((IndexArray![aIndex].OffSet > 0) && (IndexArray[aIndex].OffSet < FileStream.Length))
                {
                    FileStream.Position = IndexArray[aIndex].OffSet;
                    var ImgInfo = TImgInfo.ReadFrom(FileStream);

                    m_ImgArr[aIndex].nWidth = (ushort)ImgInfo.wW;
                    m_ImgArr[aIndex].nHeight = (ushort)ImgInfo.wH;
                    m_ImgArr[aIndex].nPx = ImgInfo.wPx;
                    m_ImgArr[aIndex].nPy = ImgInfo.wPy;

                    cx = ImgInfo.wW;
                    cy = ImgInfo.wH;
                    pointX = ImgInfo.wPx;
                    pointY = ImgInfo.wPy;
                    Result = true;
                }
            }
            else
            {
                cx = m_ImgArr[aIndex].nWidth;
                cy = m_ImgArr[aIndex].nHeight;
                pointX = m_ImgArr[aIndex].nPx;
                pointY = m_ImgArr[aIndex].nPy;
                Result = true;
            }
        }
        return Result;
    }

    // =============================== 内部助手 ===============================

    /// <summary>
    /// GameImages.pas 1055-1087 MakeDibByPixelFormat 的 pf8bit 分支
    /// （Wis 全部走 8bit：原文 784/850/942/1035 的 <c>Move(g_DefColorTable, Source.ColorTable, ...)</c>）。
    /// </summary>
    public static TDib MakeDib(int nW, int nH)
    {
        var result = TDib.Create();
        result.SetSize(nW, nH, 8);
        return result;
    }

    private static byte ReadByte(byte[] buf, int index)
        => index >= 0 && index < buf.Length ? buf[index] : (byte)0;

    private static void WriteByte(byte[] buf, int index, byte value)
    {
        if (index >= 0 && index < buf.Length) buf[index] = value;
    }
}
