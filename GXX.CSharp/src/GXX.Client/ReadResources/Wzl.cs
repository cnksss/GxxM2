using System;
using System.IO;
using System.Runtime.InteropServices;
using GXX.Core.Compress;
using GXX.Core.Rtl;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Wzl.pas（1475 行）1:1 移植 —— WZL 图库（.wzl + .wzx 索引）。
//
// 原文条件编译常量（GameImages.pas 12-20）：LOADIMAGEMODE = 0，USEMAPSTREAM = 0，
// 故 Wzl.pas 中全部 {$IF USEMAPSTREAM = 0} 分支成立，走局部 record + TFileStream 顺序读
// （每次 Read 都推进文件位置）；{$ELSE} 的 TMapStream 指针对齐分支不参与编译，仅以注释保留。
// =====================================================================================

/// <summary>
/// Wzl.pas 20-27 <c>TWzlImageHeader = record Title:string[40]; ImageCount:Integer;
/// ColorCount:Integer; PaletteSize:Integer; VerFlag:Integer; Flag:Integer; end;</c>
/// <para>**非** packed：Delphi 7 中 string[N] 占 N+1 = 41 字节，其后 5 个 Integer 自然对齐到 4 字节边界
/// （41 → 44），故 SizeOf = 44 + 20 = <b>64</b> 字节。</para>
/// </summary>
public struct TWzlImageHeader
{
    /// <summary>Title:string[40]，第 0 字节为长度（41 字节）。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
    public byte[] Title;

    public int ImageCount;
    public int ColorCount;
    public int PaletteSize;
    public int VerFlag;
    public int Flag;

    /// <summary>Delphi <c>SizeOf(TWzlImageHeader)</c> = 64。</summary>
    public const int SizeOf = 64;

    /// <summary>Title 的字符串视图（Delphi ShortString 语义）。</summary>
    public string TitleText
    {
        get => ShortStringLayout.GetString(Title, 0, 40);
        set => ShortStringLayout.SetBytes(Title, 0, 40, value);
    }

    public static TWzlImageHeader CreateEmpty()
    {
        var h = default(TWzlImageHeader);
        h.Title = new byte[41];
        return h;
    }

    /// <summary>从字节流读取（字段偏移与 SizeOf 一致，供合成字节测试使用）。</summary>
    public static TWzlImageHeader FromBytes(byte[] buf, int offset)
    {
        var h = CreateEmpty();
        Buffer.BlockCopy(buf, offset, h.Title, 0, 41);
        h.ImageCount = BitConverter.ToInt32(buf, offset + 44);
        h.ColorCount = BitConverter.ToInt32(buf, offset + 48);
        h.PaletteSize = BitConverter.ToInt32(buf, offset + 52);
        h.VerFlag = BitConverter.ToInt32(buf, offset + 56);
        h.Flag = BitConverter.ToInt32(buf, offset + 60);
        return h;
    }

    /// <summary>把结构写进缓冲（与 FromBytes 对称，供合成字节构造）。</summary>
    public readonly void ToBytes(byte[] buf, int offset)
    {
        Buffer.BlockCopy(Title, 0, buf, offset, 41);
        BitConverter.GetBytes(ImageCount).CopyTo(buf, offset + 44);
        BitConverter.GetBytes(ColorCount).CopyTo(buf, offset + 48);
        BitConverter.GetBytes(PaletteSize).CopyTo(buf, offset + 52);
        BitConverter.GetBytes(VerFlag).CopyTo(buf, offset + 56);
        BitConverter.GetBytes(Flag).CopyTo(buf, offset + 60);
    }
}

/// <summary>
/// Wzl.pas 30-42 <c>TWzlImageInfo = record</c>（原文未写 packed）。
/// <para>原文以**非 packed record** 声明，却按 <b>packed</b> 的 15 字节布局读写，这是原文的既有缺陷：
/// Delphi 7 对该 record 的实际字段偏移为 0 / 4 / 8 / 12 / 16 / 20 / 22 / 24 / 28（SizeOf = 32），
/// 与磁盘上真实存在的 packed 15 字节布局不一致。</para>
/// <para>Delphi 7 <c>TPixelFormat</c> 的 SizeOf 为 1，故 packed 布局为：
/// 0:PixelFormat(1) 1:bt2(1) 2:bt3(1) 3:bt4(1) 4:nWidth(2) 6:nHeight(2) 8:px(2) 10:py(2) 12:Length(4) = 15 字节。
/// 与 Wzl.pas 366-372 对 <c>nWidth $FF</c>/<c>nHeight * $100</c> 的取值、以及 458/920 行
/// <c>Position + SizeOf(TWzlImageInfo)</c> 的跳过量完全自洽 —— 这就是文件里的真实布局。</para>
/// <para>本类型以 Pack=1 序列化，并通过 <see cref="ReadLegacyView"/> 暴露原文实际用到的整数视图，
/// 字节偏移与宽度逐字节一致。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TWzlImageInfo
{
    /// <summary>偏移 0：原文为 TPixelFormat（1 字节枚举），本移植按字节保留。</summary>
    public byte PixelFormat;

    /// <summary>偏移 1：<c>bt2:Byte; // bt2=1 是否压缩</c>。</summary>
    public byte bt2;

    /// <summary>偏移 2。</summary>
    public byte bt3;

    /// <summary>偏移 3：<c>bt4:Byte; // ZIP 压缩等级</c>。</summary>
    public byte bt4;

    /// <summary>偏移 4：<c>nWidth:SmallInt;</c></summary>
    public short nWidth;

    /// <summary>偏移 6：<c>nHeight:SmallInt;</c></summary>
    public short nHeight;

    /// <summary>偏移 8：<c>px:SmallInt;</c></summary>
    public short px;

    /// <summary>偏移 10：<c>py:SmallInt;</c></summary>
    public short py;

    /// <summary>偏移 12：<c>Length:Integer;</c>（压缩数据字节数；0 表示未压缩）。</summary>
    public int Length;

    /// <summary>Delphi <c>SizeOf(TWzlImageInfo)</c>（packed 版，即磁盘上的真实宽度）= 15。</summary>
    public const int SizeOf = 15;

    /// <summary>把任意 15 字节缓冲按 packed 布局取出字段（越界/不足时补 0）。</summary>
    public static TWzlImageInfo FromBytes(byte[] buf, int offset)
    {
        var info = default(TWzlImageInfo);
        info.PixelFormat = ReadByte(buf, offset + 0);
        info.bt2 = ReadByte(buf, offset + 1);
        info.bt3 = ReadByte(buf, offset + 2);
        info.bt4 = ReadByte(buf, offset + 3);
        info.nWidth = (short)ReadInt(buf, offset + 4, 2);
        info.nHeight = (short)ReadInt(buf, offset + 6, 2);
        info.px = (short)ReadInt(buf, offset + 8, 2);
        info.py = (short)ReadInt(buf, offset + 10, 2);
        info.Length = ReadInt(buf, offset + 12, 4);
        return info;
    }

    /// <summary>
    /// 把结构按 packed 15 字节布局写进缓冲（供合成字节构造）。缓冲必须至少 16 字节——
    /// 偏移 12 的 Length 是 4 字节字段，其末字节（偏移 15）已超出 SizeOf=15，与被跳过 1 字节的 PixelFormat 抵消。
    /// </summary>
    public readonly void ToBytes(byte[] buf, int offset)
    {
        buf[offset + 0] = PixelFormat;
        buf[offset + 1] = bt2;
        buf[offset + 2] = bt3;
        buf[offset + 3] = bt4;
        WriteInt(buf, offset + 4, nWidth, 2);
        WriteInt(buf, offset + 6, nHeight, 2);
        WriteInt(buf, offset + 8, px, 2);
        WriteInt(buf, offset + 10, py, 2);
        WriteInt(buf, offset + 12, Length, 4);
    }

    /// <summary>原文 <c>m_FileStream.Read(ImageInfo, SizeOf(TWzlImageInfo))</c> 的等价读取（顺序读，推进位置）。</summary>
    public static TWzlImageInfo ReadFrom(Stream stream)
    {
        var buf = new byte[SizeOf];
        int read = ReadFully(stream, buf, 0, SizeOf);
        if (read < SizeOf)
            throw new EndOfStreamException($"TWzlImageInfo 读取不足：{read}/{SizeOf}");
        return FromBytes(buf, 0);
    }

    /// <summary>
    /// 原文 LoadDxImage / LoadDxGrayImage / LoadDxBrightImage 在**非 packed record** 下的整数视图：
    /// <code>
    /// nWidth  := PSmallInt(@rec[0])^;                               // 低 16 位 = PixelFormat or (bt2 shl 8)
    /// nHeight := PSmallInt(@rec[4])^;                               // 即 nWidth 字段本身
    /// px      := PSmallInt(@rec[0])^ + PSmallInt(@rec[2])^ shl 16;  // 低 16 位来自 rec[0..1]
    /// py      := PSmallInt(@rec[8])^ + PSmallInt(@rec[10])^ shl 16; // 低 16 位来自 rec[8..9]
    /// Length  := PInteger(@rec[12])^;
    /// </code>
    /// 注意 <c>px</c> 的**高 16 位来自 rec[2..3]（bt3/bt4）**、<c>py</c> 的**高 16 位来自 rec[10..11]（py 字段）** ——
    /// 这是原文把 SmallInt 提升为 Integer 后再拼接造成的，**照抄不修**。
    /// </summary>
    public static WzlImageInfoView ReadLegacyView(byte[] buf, int offset)
    {
        var v = default(WzlImageInfoView);
        v.nWidth = (short)ReadInt(buf, offset + 0, 2);
        v.nHeight = (short)ReadInt(buf, offset + 4, 2);
        v.px = ReadInt(buf, offset + 0, 2) + (ReadInt(buf, offset + 2, 2) << 16);
        v.py = ReadInt(buf, offset + 8, 2) + (ReadInt(buf, offset + 10, 2) << 16);
        v.Length = ReadInt(buf, offset + 12, 4);
        return v;
    }

    /// <summary>原文 LoadDx* 的整数视图（见 <see cref="ReadLegacyView"/>）。</summary>
    public struct WzlImageInfoView
    {
        public short nWidth;
        public short nHeight;
        public int px;
        public int py;
        public int Length;
    }


    internal static byte ReadByte(byte[] buf, int offset)
        => offset >= 0 && offset < buf.Length ? buf[offset] : (byte)0;

    /// <summary>按 (偏移,宽度) 读取有符号小端整数（不足补 0，对齐原文越界读的容错意图）。</summary>
    internal static int ReadInt(byte[] buf, int offset, int width)
    {
        // 小端：偏移 +0 是最低有效字节
        long v = ReadByte(buf, offset);
        for (int i = 1; i < width; i++)
            v |= (long)ReadByte(buf, offset + i) << (8 * i);
        if (width < 4)
        {
            long signBit = 1L << ((width * 8) - 1);
            if ((v & signBit) != 0) v -= 1L << (width * 8);
        }
        else
        {
            v = (int)v;
        }
        return (int)v;
    }

    internal static void WriteInt(byte[] buf, int offset, int value, int width)
    {
        for (int i = 0; i < width; i++)
            buf[offset + i] = (byte)((value >> (8 * i)) & 0xFF);
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
/// Wzl.pas 44-48 <c>TWzlIndexHeader = record Title:string[40]; IndexCount:Integer; end;</c>
/// <para>非 packed：41 字节 Title 后 Integer 对齐到 44，故 SizeOf = <b>48</b>。</para>
/// </summary>
public struct TWzlIndexHeader
{
    /// <summary>Title:string[40]（41 字节）。</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
    public byte[] Title;

    public int IndexCount;

    /// <summary>Delphi <c>SizeOf(TWZLIndexHeader)</c> = 48。</summary>
    public const int SizeOf = 48;

    public string TitleText
    {
        get => ShortStringLayout.GetString(Title, 0, 40);
        set => ShortStringLayout.SetBytes(Title, 0, 40, value);
    }

    public static TWzlIndexHeader CreateEmpty()
    {
        var h = default(TWzlIndexHeader);
        h.Title = new byte[41];
        return h;
    }

    public static TWzlIndexHeader FromBytes(byte[] buf, int offset)
    {
        var h = CreateEmpty();
        Buffer.BlockCopy(buf, offset, h.Title, 0, 41);
        h.IndexCount = BitConverter.ToInt32(buf, offset + 44);
        return h;
    }

    public readonly void ToBytes(byte[] buf, int offset)
    {
        Buffer.BlockCopy(Title, 0, buf, offset, 41);
        BitConverter.GetBytes(IndexCount).CopyTo(buf, offset + 44);
    }
}

/// <summary>
/// Wzl.pas 50-91 <c>TWzlImages = class(TGameImages)</c> 1:1 移植。
/// <para><b>本类只移植“资源格式解析 + 索引/缓存装载”</b>；原文中与 UpdateEngine / GlobalString(DecodeResStr) /
/// HGE 渲染耦合的部分以接缝承载（见类内 <c>// 接缝</c> 注释与 <see cref="TextureSeams"/>）。</para>
/// </summary>
public sealed class TWzlImages : TGameImages
{
    /// <summary>Wzl.pas 68/70：<c>m_FileStream:TFileStream</c>（USEMAPSTREAM=0 分支）。</summary>
    public FileStream? m_FileStream;

    /// <summary>Wzl.pas 73（原文为 TRTLCriticalSection，此处以对象锁表达同一串行化）。</summary>
    public readonly object FCSFileStream = new();

    /// <summary>Wzl.pas 259-298：LoadIndex 读取到的索引偏移表（原文 m_IndexList）。</summary>
    public readonly System.Collections.Generic.List<int> m_IndexList = new();

    /// <summary>Wzl.pas 266：<c>m_IndexList.Clear;</c> 后若索引文件存在且 IndexCount &gt; 0 才置真。</summary>
    public bool IndexLoaded;

    /// <summary>Wzl.pas 165：<c>m_FileStream.Read(FHeader, SizeOf(TWzlImageHeader))</c> 的结果。</summary>
    public TWzlImageHeader FHeader;

    /// <summary>Wzl.pas 75：<c>MainPalette:TRGBQuads;</c>（本移植的调色板统一走 g_DefColorTable，见 GDefColorTable）。</summary>
    public byte[] MainPalette = GDefColorTable.ColorArray;

    public TWzlImages()
    {
        m_FileStream = null;
        ResetWZLAlpha = false;
    }

    /// <summary>GameImages.pas 1115-1117：TWzlImages 的索引文件扩展名为 .wzx。</summary>
    protected override string IndexFileExtension => ".wzx";

    /// <summary>Wzl.pas 89-90 LockFileStream + 1465-1468 EnterCriticalSection(FCSFileStream)。</summary>
    public void LockFileStream() => System.Threading.Monitor.Enter(FCSFileStream);

    /// <summary>Wzl.pas 1470-1472 UnLockFileStream。</summary>
    public void UnLockFileStream() => System.Threading.Monitor.Exit(FCSFileStream);

    // =============================== Initialize（135-203） ===============================

    /// <summary>
    /// Wzl.pas 135-203 Initialize 1:1。
    /// <list type="bullet">
    /// <item>141：<c>if not Initialized then</c>；142：<c>if FileExists(FileName) then</c>。</item>
    /// <item>151：以 fmOpenReadWrite or fmShareDenyNone 打开（FileMode.Open + ReadWrite + 共享读写）。</item>
    /// <item>152-154：打开失败 → m_FileStream := nil。</item>
    /// <item>159-163：nil → OutMessage + Exit（Initialized 保持 False）。</item>
    /// <item>165：Read(FHeader, SizeOf(TWzlImageHeader))；167：ImageCount := FHeader.ImageCount。</item>
    /// <item>169-175：AllocMem(SizeOf(TDXImage) * ImageCount)；nil → 关流 + Exit。</item>
    /// <item>177-185：逐项清零并把 dwUpdateStartTick 置为 MyGetTickCount。</item>
    /// <item>189-190：LoadIndex(IndexFileName) 后 Initialized := True。</item>
    /// <item>197-201：异常时 <c>raise Exception.Create(E.Message)</c> —— 此处以原样重抛等价表达。</item>
    /// </list>
    /// </summary>
    public override void Initialize()
    {
        try
        {
            Finalize_();
            if (!Initialized)
            {
                if (File.Exists(FileName))
                {
                    Lock();
                    try
                    {
                        m_boUpdateIndex = true;
                        m_boUpdateIndexing = false;
                        m_boNeedUpdate = true;

                        try
                        {
                            // 原文 fmOpenReadWrite or fmShareDenyNone
                            m_FileStream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        }
                        catch
                        {
                            m_FileStream = null;
                        }

                        if (m_FileStream == null)
                        {
                            // OutMessage('[Exception] TWzlImages::Initialize');   ← 原文 160 行注释掉的写法
                            // 接缝：DecodeResStr(SWzlInitErr) / GlobalString 资源串待 GlobalString.pas 移植后接入
                            OutMessage(DelphiRTL.Format("文件打开失败：{0}", FileName));
                            return;
                        }

                        FHeader = ReadHeaderFromStream(m_FileStream);

                        ImageCount = FHeader.ImageCount;

                        m_ImgArr = new TDxImage[ImageCount];
                        // AllocMem 的等价物：Delphi 里 TDXImage 是值类型数组（AllocMem 已清零），
                        // C# 里 TDxImage 是引用类型 → 必须逐项实例化，否则数组元素为 null。
                        for (int i = 0; i < m_ImgArr.Length; i++) m_ImgArr[i] = new TDxImage();

                        if (m_ImgArr == null)
                        {
                            if (m_FileStream != null)
                            {
                                m_FileStream.Dispose();
                                m_FileStream = null;
                            }
                            return;
                        }

                        for (int I = 0; I <= ImageCount - 1; I++)
                        {
                            m_ImgArr[I].nWidth = 0;
                            m_ImgArr[I].nHeight = 0;
                            m_ImgArr[I].nPx = 0;
                            m_ImgArr[I].nPy = 0;
                            m_ImgArr[I].boUpdateStop = false;
                            m_ImgArr[I].boUpdateStart = false;
                            m_ImgArr[I].dwUpdateStartTick = MyGetTickCount();
                        }

                        // idxfile := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.WZX';   ← 原文 187 行注释

                        LoadIndex(IndexFileName);
                        Initialized = true;
                    }
                    finally
                    {
                        UnLock();
                    }
                }
            }
        }
        catch (Exception E)
        {
            // 原文 197-201：on E:Exception do begin raise Exception.Create(E.Message); end;
            // （原文只带走 Message；此处把内层异常挂在 InnerException 上以便定位，Message 语义不变）
            throw new Exception(E.Message, E);
        }
    }

    /// <summary>Wzl.pas 205-257 Finalize 1:1。</summary>
    public override void Finalize_()
    {
        Lock();
        try
        {
            if (Initialized)
            {
                Initialized = false;
                IndexList.Clear();
                GrayIndexList.Clear();
                BrightIndexList.Clear();
                FreeImageArray();
                ImageCount = 0;
                if (m_FileStream != null)
                {
                    m_FileStream.Dispose();
                    m_FileStream = null;
                }
            }
        }
        finally
        {
            UnLock();
        }
    }

    /// <summary>Wzl.pas 165：<c>m_FileStream.Read(FHeader, SizeOf(TWzlImageHeader))</c>（SizeOf = 64）。</summary>
    public static TWzlImageHeader ReadHeaderFromStream(Stream stream)
    {
        var buf = new byte[TWzlImageHeader.SizeOf];
        TWzlImageInfo.ReadFully(stream, buf, 0, TWzlImageHeader.SizeOf);
        return TWzlImageHeader.FromBytes(buf, 0);
    }

    // =============================== LoadIndex（259-298） ===============================

    /// <summary>
    /// Wzl.pas 259-298 LoadIndex 1:1。
    /// <list type="bullet">
    /// <item>266：<c>m_IndexList.Clear;</c>（无条件）。</item>
    /// <item>267-287：索引文件存在 → 读 TWzlIndexHeader（SizeOf = 48）→ IndexCount &gt; 0 时
    /// 一次性读 IndexCount 个 Integer 并按序 Add 进 m_IndexList。</item>
    /// <item>288-297：不存在 → CLIENTEXE=1 时尝试 UpdateEngine 请求索引更新（接缝）。</item>
    /// </list>
    /// </summary>
    public void LoadIndex(string sIdxFile)
    {
        m_IndexList.Clear();
        IndexLoaded = false;
        if (File.Exists(sIdxFile))
        {
            FileStream? indexStream;
            try
            {
                indexStream = new FileStream(sIdxFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch
            {
                indexStream = null;
            }

            if (indexStream == null)
            {
                // OutMessage('[Exception] TWzlImages::LoadIndex');   ← 原文 270 行注释
                OutMessage(DelphiRTL.Format("索引文件加载失败：{0}", sIdxFile));
                return;
            }

            var headerBuf = new byte[TWzlIndexHeader.SizeOf];
            if (TWzlImageInfo.ReadFully(indexStream, headerBuf, 0, TWzlIndexHeader.SizeOf) < TWzlIndexHeader.SizeOf)
            {
                indexStream.Dispose();
                return;
            }
            var header = TWzlIndexHeader.FromBytes(headerBuf, 0);

            if (header.IndexCount > 0)
            {
                var pValue = new byte[sizeof(int) * header.IndexCount];
                int got = TWzlImageInfo.ReadFully(indexStream, pValue, 0, pValue.Length);
                for (int I = 0; I <= header.IndexCount - 1; I++)
                {
                    int value = (I * sizeof(int)) + sizeof(int) <= got
                        ? BitConverter.ToInt32(pValue, I * sizeof(int))
                        : 0;
                    m_IndexList.Add(value);
                }
                IndexLoaded = true;
            }
            indexStream.Dispose();
        }
        else
        {
            // 原文 289-296（{$IF CLIENTEXE = 1}）：索引文件缺失且允许自动更新时向 UpdateEngine 请求。
            // 接缝：待 UpdateEngine.pas / MShare.pas(g_boAutoUpdate, g_UpdateRetryTime) 移植后接入。
            if (m_boUpdateIndex
                && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                && g_boAutoUpdate)
            {
                if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                {
                    m_boUpdateIndexing = true;
                    m_dwUpdateIndexingTick = MyGetTickCount();
                }
            }
        }
    }

    /// <summary>Wzl.pas 189：<c>LoadIndex(IndexFileName)</c>。</summary>
    public void LoadIndex() => LoadIndex(IndexFileName);

    /// <summary>Wzl.pas 260 的 <c>m_IndexList.Count</c> 视图。</summary>
    public int IndexCount => m_IndexList.Count;

    /// <summary>
    /// Wzl.pas 1018/1049/1104/1214/1276 的 <c>Integer(m_IndexList[Index])</c> 视图
    /// （原文把 TList 里的 Pointer 直接当地址值用）。
    /// </summary>
    public int GetIndexPosition(int index) => m_IndexList[index];

    // =============================== LoadDxBitmap（1004-1007） ===============================

    /// <summary>
    /// Wzl.pas 1004-1007 LoadDxBitmap —— 原文**方法体为空**（照抄）。
    /// 因此 <see cref="GetCachedBitmap"/> 与 <see cref="GetBitmap"/> 的 Bitmap 槽位恒为 nil。
    /// </summary>
    public void LoadDxBitmap(int position, TDxImage dxImage, int index)
    {
        // 原文如此（Wzl.pas:1004-1007）：方法体为空。
    }

    // =============================== 文件读取原语 ===============================

    /// <summary>
    /// Wzl.pas 341-345 / 584-588 / 812-816：<c>m_FileStream.Position := Position; Read(ImageInfo, SizeOf)</c>。
    /// 返回 15 字节 ImageInfo 的原始缓冲（整数视图见 <see cref="TWzlImageInfo.ReadLegacyView"/>）。
    /// </summary>
    public byte[] ReadImageInfoAt(int position)
    {
        var buf = new byte[TWzlImageInfo.SizeOf];
        if (m_FileStream == null) return buf;
        LockFileStream();
        try
        {
            m_FileStream.Position = position;
            TWzlImageInfo.ReadFully(m_FileStream, buf, 0, TWzlImageInfo.SizeOf);
        }
        finally
        {
            UnLockFileStream();
        }
        return buf;
    }

    /// <summary>Wzl.pas 456-462 / 691-697 / 918-924：<c>Position := Position + SizeOf(TWzlImageInfo); Read(InBuf^, ImageInfo.Length)</c>。</summary>
    public byte[] ReadImageDataAt(int position, int length)
    {
        var buf = new byte[Math.Max(length, 0)];
        if (m_FileStream == null || buf.Length == 0) return buf;
        LockFileStream();
        try
        {
            m_FileStream.Position = (long)position + TWzlImageInfo.SizeOf;
            TWzlImageInfo.ReadFully(m_FileStream, buf, 0, buf.Length);
        }
        finally
        {
            UnLockFileStream();
        }
        return buf;
    }

    /// <summary>Wzl.pas 511-517 / 744-750 / 972-978：未压缩分支 <c>Read(Source.PBits^, nSize)</c>（紧接 ImageInfo 之后）。</summary>
    public byte[] ReadRawImageDataAt(int position, int nSize)
    {
        var buf = new byte[Math.Max(nSize, 0)];
        if (m_FileStream == null || buf.Length == 0) return buf;
        LockFileStream();
        try
        {
            m_FileStream.Position = (long)position + TWzlImageInfo.SizeOf;
            TWzlImageInfo.ReadFully(m_FileStream, buf, 0, buf.Length);
        }
        finally
        {
            UnLockFileStream();
        }
        return buf;
    }

    // =============================== LoadDxImage（776-1002） ===============================

    /// <summary>
    /// Wzl.pas 776-1002 LoadDxImage 1:1。
    /// <list type="bullet">
    /// <item>801：<c>if (DXImage.Surface = nil) then</c>。</item>
    /// <item>802-807：Position = 0 或 Position &gt; Size - SizeOf(TWzlImageInfo) → 打点 + 清 px/py + Exit。</item>
    /// <item>823-862：四处 OutMessage+Exit 守卫（px/nHeight 越界、Length 越界、nWidth/nHeight 越界、空图片）。</item>
    /// <item>906/908-912：MakeDibByPixelFormat(ImageInfo.PixelFormat, ...) + 黑底。</item>
    /// <item>915-979：Length &gt; 0 走 DecompressBuf（失败 → boDecompressError + NULLTexture），
    /// 否则直接顺序读 nSize 字节。</item>
    /// <item>942-966：ResetWZLAlpha 时把 OutBuf 尾部 (nWidth*nHeight div 2) 字节展开为 alpha 平面。</item>
    /// <item>981-994：D3D 大图路径 / 普通 NewTexture；解压失败一律 NULLTexture。</item>
    /// </list>
    /// </summary>
    public void LoadDxImage(int position, TDxImage dXImage, int index)
    {
        if (!dXImage.Surface.Assigned)
        {
            if (m_FileStream == null) return;

            if ((position == 0) || (position > m_FileStream.Length - TWzlImageInfo.SizeOf)) // 可能需要更新资源，可能是空图片
            {
                dXImage.dwLatestTime = MyGetTickCount();
                dXImage.nPx = 0; // ImageInfo.px;
                dXImage.nPy = 0; // ImageInfo.py;
                return;
            }

            var infoBuf = ReadImageInfoAt(position);
            var ImageInfo = TWzlImageInfo.ReadLegacyView(infoBuf, 0);

            if ((Math.Abs(ImageInfo.px) > GameImagesConsts.MAX_IMAGE_WIDTH) || (Math.Abs(ImageInfo.nHeight) > GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                // 接缝：DecodeResStr(SWzlImageErr) 待 GlobalString.pas 移植后接入
                OutMessage(DelphiRTL.Format("WZL 图片信息错误：{0},{1},{2},{3}", FileName, index, ImageInfo.px, ImageInfo.py));

                dXImage.dwLatestTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Surface = TTextureRef.NullTextureSentinel;

                return;
            }

            if (ImageInfo.Length >= GameImagesConsts.MAX_IMAGE_SIZE)
            {
                OutMessage(DelphiRTL.Format("WZL 图片长度错误：{0},{1},{2}", FileName, index, ImageInfo.Length));

                dXImage.dwLatestTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Surface = TTextureRef.NullTextureSentinel;

                return;
            }

            if ((ImageInfo.nWidth >= GameImagesConsts.MAX_IMAGE_WIDTH) || (ImageInfo.nHeight >= GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                OutMessage(DelphiRTL.Format("WZL 图片尺寸错误：{0},{1},{2},{3}", FileName, index, ImageInfo.nWidth, ImageInfo.nHeight));

                dXImage.dwLatestTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Surface = TTextureRef.NullTextureSentinel;

                return;
            }

            if ((ImageInfo.nWidth * ImageInfo.nHeight <= 0) || (ImageInfo.nWidth <= 0)) // 空图片
            {
                dXImage.dwLatestTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Surface = TTextureRef.NullTextureSentinel;
                return;
            }

            dXImage.dwLatestTime = MyGetTickCount();
            dXImage.nWidth = (ushort)ImageInfo.nWidth;
            dXImage.nHeight = (ushort)ImageInfo.nHeight;
            dXImage.nPx = (short)ImageInfo.px;
            dXImage.nPy = (short)ImageInfo.py;

            // 原文 870-906：先注释掉旧 case 分支（pf8bit/pf15bit/.../pf32bit 逐 WidthBytes 估算 nSize），
            // 再统一调用 Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);
            // 原文如此（Wzl.pas:872-906）：ImageInfo.PixelFormat 是**非 packed record 的偏移 0 字段**，
            // 即 ImageInfo 结构体的第 0 字节 = 文件里 15 字节块的第 0 字节（原文注释里的 bt1）。
            TDib? Source = MakeDibByPixelFormat(infoBuf[0], ImageInfo.nWidth, ImageInfo.nHeight);

            bool boDecompressError = false;
            TDib? lsAlpha = null;

            if (Source != null)
            {
                int nSize = Source.WidthBytes * Source.Height;

                // 原文 911-912：Source.Canvas.Brush.Color := clBlack; Source.Canvas.FillRect(Source.Canvas.ClipRect);
                // SetSize 已按 0 初始化位数据，等价于黑底（调色板索引 0 = 黑）。
                if (ImageInfo.Length > 0) // 需要解压
                {
                    byte[] InBuf = ReadImageDataAt(position, ImageInfo.Length);

                    byte[]? OutBuf = null;
                    int OutBytes = 0;
                    try
                    {
                        OutBuf = ZlibEx.DecompressBuf(InBuf, ImageInfo.Length, nSize);
                        OutBytes = OutBuf?.Length ?? 0;
                        // 原文 DecompressBuf 失败时抛异常 → 外层 except 置 boDecompressError 并报错；
                        // C# 侧 ZlibEx.DecompressBuf 内部吞异常并返回 null，故此处补上同一「置标志 + 报错」，
                        // 否则会与原文行为分叉（原文失败 → NULLTexture，此处会误造出黑图纹理）。
                        if (OutBuf == null)
                        {
                            boDecompressError = true;
                            // OutMessage('[Exception] TWzlImages::LoadDxImage DecompressBuf');   ← 原文 935 行注释
                            OutMessage(DelphiRTL.Format("WZL 图片解压失败：{0},{1}", FileName, index));
                        }
                    }
                    catch
                    {
                        boDecompressError = true;
                        // OutMessage('[Exception] TWzlImages::LoadDxImage DecompressBuf');   ← 原文 935 行注释
                        OutMessage(DelphiRTL.Format("WZL 图片解压失败：{0},{1}", FileName, index));
                    }

                    if ((OutBuf != null) && (OutBytes > 0))
                    {
                        // Move(OutBuf^, Source.PBits^, nSize)：把 nSize 字节铺到 8bit 扫描线平面
                        FillDibFromBuffer(Source, OutBuf, nSize);

                        if (ResetWZLAlpha)
                        {
                            // 原文 943-944：SourceAlphaSize := OutBytes - Source.Size;
                            int SourceAlphaSize = OutBytes - Source.Size;
                            if (SourceAlphaSize == ((int)ImageInfo.nWidth * (int)ImageInfo.nHeight / 2))
                            {
                                int alphaOffset = Source.Size; // PByte(Integer(OutBuf) + Source.Size)

                                lsAlpha = TDib.Create();
                                lsAlpha.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);

                                // 原文 950-964：
                                //   LineData := lsAlpha.ScanLine[nY];
                                //   pData := @SourceAlphaBuf[nY * lsAlpha.Width div 2];
                                //   nX_2 := nX div 2;
                                //   if nX mod 2 = 0 then LineData^ := pData[nX_2]
                                //                   else LineData^ := (pData[nX_2] + pData[nX_2 + 1]) div 2;
                                BuildAlphaPlane(lsAlpha, OutBuf, alphaOffset);
                            }
                        }
                        // FreeMem(OutBuf)
                    }
                    // FreeMem(InBuf)
                }
                else
                {
                    // 原文 971-979：没有压缩，顺序读 nSize 字节
                    byte[] raw = ReadRawImageDataAt(position, nSize);
                    FillDibFromBuffer(Source, raw, nSize);
                }

                if (D3DFormat && (Source.Width >= 400) && (Source.Height >= 400))
                {
                    // 原文 982-986：FileData32(Source, FileData, FileSize) → NewTexture(FileData, FileSize, ...)
                    // 接缝：FileData32/FileDataBright32/FileDataGray32（GameImages.pas）与 D3D 纹理路径待移植。
                    if (!boDecompressError)
                        dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(Source, lsAlpha));
                    else
                        dXImage.Surface = TTextureRef.NullTextureSentinel;
                }
                else
                {
                    // 修改chongchong 2015-07-30 14:05:59（原文 988-993）
                    if (!boDecompressError)
                    {
                        dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(Source, lsAlpha));
                    }
                    else
                    {
                        dXImage.Surface = TTextureRef.NullTextureSentinel;
                    }
                }

                // if lsAlpha <> nil then lsAlpha.Free;   ← 托管对象，无显式释放
                // Source.Free;                            ← 托管对象，无显式释放
            }
        }
    }

    /// <summary>Wzl.pas 544-774 LoadDxGrayImage 1:1（与 LoadDxImage 同构，差异逐条保留）。</summary>
    public void LoadDxGrayImage(int position, TDxImage dXImage, int index)
    {
        if (!dXImage.Gray.Assigned)
        {
            if (m_FileStream == null) return;

            if ((position == 0) || (position > m_FileStream.Length - TWzlImageInfo.SizeOf)) // 需要更新资源
            {
                dXImage.dwLatestGrayTime = MyGetTickCount();
                dXImage.nPx = 0; // ImageInfo.px;
                dXImage.nPy = 0; // ImageInfo.py;
                // DXImage.Gray := NULLTexture;   ← 原文 577 行注释
                return;
            }

            var infoBuf = ReadImageInfoAt(position);
            var ImageInfo = TWzlImageInfo.ReadLegacyView(infoBuf, 0);

            if ((Math.Abs(ImageInfo.px) > GameImagesConsts.MAX_IMAGE_WIDTH) || (Math.Abs(ImageInfo.nHeight) > GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                OutMessage(DelphiRTL.Format("WZL 灰度图片信息错误：{0},{1},{2},{3}", FileName, index, ImageInfo.px, ImageInfo.py));

                dXImage.dwLatestGrayTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Gray = TTextureRef.NullTextureSentinel;

                return;
            }

            if (ImageInfo.Length >= GameImagesConsts.MAX_IMAGE_SIZE)
            {
                OutMessage(DelphiRTL.Format("WZL 灰度图片长度错误：{0},{1},{2}", FileName, index, ImageInfo.Length));

                dXImage.dwLatestGrayTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Gray = TTextureRef.NullTextureSentinel;

                return;
            }

            // 原文 617 行守卫两个上限都写 MAX_IMAGE_WIDTH（Bright 版是 MAX_IMAGE_HEIGHT —— 照抄不修）
            if ((ImageInfo.nWidth >= GameImagesConsts.MAX_IMAGE_WIDTH) || (ImageInfo.nHeight >= GameImagesConsts.MAX_IMAGE_WIDTH))
            {
                OutMessage(DelphiRTL.Format("WZL 灰度图片尺寸错误：{0},{1},{2},{3}", FileName, index, ImageInfo.nWidth, ImageInfo.nHeight));

                dXImage.dwLatestGrayTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Gray = TTextureRef.NullTextureSentinel;

                return;
            }

            // 原文 628-634：本版**没有** Bright 版那一档 `nWidth*nHeight <= 4`
            if ((ImageInfo.nWidth * ImageInfo.nHeight <= 0) || (ImageInfo.nWidth <= 0)) // 空图片
            {
                dXImage.dwLatestGrayTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Gray = TTextureRef.NullTextureSentinel;
                return;
            }

            dXImage.dwLatestGrayTime = MyGetTickCount();
            dXImage.nWidth = (ushort)ImageInfo.nWidth;
            dXImage.nHeight = (ushort)ImageInfo.nHeight;
            dXImage.nPx = (short)ImageInfo.px;
            dXImage.nPy = (short)ImageInfo.py;

            TDib? Source = MakeDibByPixelFormat(infoBuf[0], ImageInfo.nWidth, ImageInfo.nHeight);

            bool boDecompressError = false;
            TDib? lsAlpha = null;

            if (Source != null)
            {
                int nSize = Source.WidthBytes * Source.Height;

                if (ImageInfo.Length > 0)
                {
                    byte[] InBuf = ReadImageDataAt(position, ImageInfo.Length);

                    byte[]? OutBuf = null;
                    int OutBytes = 0;
                    try
                    {
                        OutBuf = ZlibEx.DecompressBuf(InBuf, ImageInfo.Length, nSize);
                        OutBytes = OutBuf?.Length ?? 0;
                        // 原文 DecompressBuf 失败时抛异常 → 外层 except 置 boDecompressError 并报错；
                        // C# 侧 ZlibEx.DecompressBuf 内部吞异常并返回 null，故此处补上同一「置标志 + 报错」。
                        if (OutBuf == null)
                        {
                            boDecompressError = true;
                            // OutMessage('[Exception] TWzlImages::LoadDxGrayImage DecompressBuf');   ← 原文 706 行注释
                            OutMessage(DelphiRTL.Format("WZL 灰度图片解压失败：{0},{1}", FileName, index));
                        }
                    }
                    catch
                    {
                        boDecompressError = true;
                        // OutMessage('[Exception] TWzlImages::LoadDxGrayImage DecompressBuf');   ← 原文 706 行注释
                        OutMessage(DelphiRTL.Format("WZL 灰度图片解压失败：{0},{1}", FileName, index));
                    }

                    if ((OutBuf != null) && (OutBytes > 0))
                    {
                        FillDibFromBuffer(Source, OutBuf, nSize);

                        if (ResetWZLAlpha)
                        {
                            int SourceAlphaSize = OutBytes - Source.Size;
                            if (SourceAlphaSize == ((int)ImageInfo.nWidth * (int)ImageInfo.nHeight / 2))
                            {
                                int alphaOffset = Source.Size;

                                lsAlpha = TDib.Create();
                                lsAlpha.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);

                                BuildAlphaPlane(lsAlpha, OutBuf, alphaOffset);
                            }
                        }
                    }
                }
                else
                {
                    byte[] raw = ReadRawImageDataAt(position, nSize);
                    FillDibFromBuffer(Source, raw, nSize);
                }

                if (D3DFormat && (Source.Width >= 400) && (Source.Height >= 400))
                {
                    // 接缝：FileDataGray32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    if (!boDecompressError)
                        dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(Source, lsAlpha));
                    else
                        dXImage.Gray = TTextureRef.NullTextureSentinel;
                }
                else
                {
                    if (!boDecompressError)
                        dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(Source, lsAlpha));
                    else
                        dXImage.Gray = TTextureRef.NullTextureSentinel;
                }
            }
        }
    }

    /// <summary>Wzl.pas 302-542 LoadDxBrightImage 1:1（与 Gray 版差异逐条保留）。</summary>
    public void LoadDxBrightImage(int position, TDxImage dXImage, int index)
    {
        if (!dXImage.Bright.Assigned)
        {
            if (m_FileStream == null) return;

            if ((position == 0) || (position > m_FileStream.Length - TWzlImageInfo.SizeOf)) // 需要更新资源
            {
                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = 0; // ImageInfo.px;
                dXImage.nPy = 0; // ImageInfo.py;
                // DXImage.Bright := NULLTexture;   ← 原文 334 行注释
                return;
            }

            var infoBuf = ReadImageInfoAt(position);
            var ImageInfo = TWzlImageInfo.ReadLegacyView(infoBuf, 0);

            if ((Math.Abs(ImageInfo.px) > GameImagesConsts.MAX_IMAGE_WIDTH) || (Math.Abs(ImageInfo.nHeight) > GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                OutMessage(DelphiRTL.Format("WZL 加亮图片信息错误：{0},{1},{2},{3}", FileName, index, ImageInfo.px, ImageInfo.py));

                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Bright = TTextureRef.NullTextureSentinel;

                return;
            }

            if (ImageInfo.Length >= GameImagesConsts.MAX_IMAGE_SIZE)
            {
                OutMessage(DelphiRTL.Format("WZL 加亮图片长度错误：{0},{1},{2}", FileName, index, ImageInfo.Length));

                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Bright = TTextureRef.NullTextureSentinel;

                return;
            }

            if ((ImageInfo.nWidth >= GameImagesConsts.MAX_IMAGE_WIDTH) || (ImageInfo.nHeight >= GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                OutMessage(DelphiRTL.Format("WZL 加亮图片尺寸错误：{0},{1},{2},{3}", FileName, index, ImageInfo.nWidth, ImageInfo.nHeight));

                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Bright = TTextureRef.NullTextureSentinel;

                return;
            }

            // 原文 384-393：Bright 版独有的 `nWidth*nHeight <= 4` 档（先置 NULLTexture，再打点再置一次 —— 照抄两处赋值）
            if (ImageInfo.nWidth * ImageInfo.nHeight <= 4)
            {
                dXImage.Bright = TTextureRef.NullTextureSentinel;

                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Bright = TTextureRef.NullTextureSentinel;

                return;
            }

            if ((ImageInfo.nWidth * ImageInfo.nHeight <= 0) || (ImageInfo.nWidth <= 0)) // 空图片
            {
                dXImage.dwLatestBrightTime = MyGetTickCount();
                dXImage.nPx = (short)ImageInfo.px;
                dXImage.nPy = (short)ImageInfo.py;
                dXImage.Bright = TTextureRef.NullTextureSentinel;
                return;
            }

            dXImage.dwLatestBrightTime = MyGetTickCount();
            dXImage.nWidth = (ushort)ImageInfo.nWidth;
            dXImage.nHeight = (ushort)ImageInfo.nHeight;
            dXImage.nPx = (short)ImageInfo.px;
            dXImage.nPy = (short)ImageInfo.py;

            TDib? Source = MakeDibByPixelFormat(infoBuf[0], ImageInfo.nWidth, ImageInfo.nHeight);

            bool boDecompressError = false;
            TDib? lsAlpha = null;

            if (Source != null)
            {
                int nSize = Source.WidthBytes * Source.Height;

                if (ImageInfo.Length > 0)
                {
                    byte[] InBuf = ReadImageDataAt(position, ImageInfo.Length);

                    byte[]? OutBuf = null;
                    int OutBytes = 0;
                    try
                    {
                        OutBuf = ZlibEx.DecompressBuf(InBuf, ImageInfo.Length, nSize);
                        OutBytes = OutBuf?.Length ?? 0;
                        // 原文 DecompressBuf 失败时抛异常 → 外层 except 置 boDecompressError 并报错；
                        // C# 侧 ZlibEx.DecompressBuf 内部吞异常并返回 null，故此处补上同一「置标志 + 报错」。
                        if (OutBuf == null)
                        {
                            boDecompressError = true;
                            // OutMessage('[Exception] TWzlImages::LoadDxBrightImage DecompressBuf');   ← 原文 472 行注释
                            OutMessage(DelphiRTL.Format("WZL 加亮图片解压失败：{0},{1}", FileName, index));
                        }
                    }
                    catch
                    {
                        boDecompressError = true;
                        // OutMessage('[Exception] TWzlImages::LoadDxBrightImage DecompressBuf');   ← 原文 472 行注释
                        OutMessage(DelphiRTL.Format("WZL 加亮图片解压失败：{0},{1}", FileName, index));
                    }

                    if ((OutBuf != null) && (OutBytes > 0))
                    {
                        FillDibFromBuffer(Source, OutBuf, nSize);

                        if (ResetWZLAlpha)
                        {
                            int SourceAlphaSize = OutBytes - Source.Size;
                            if (SourceAlphaSize == ((int)ImageInfo.nWidth * (int)ImageInfo.nHeight / 2))
                            {
                                int alphaOffset = Source.Size;

                                lsAlpha = TDib.Create();
                                lsAlpha.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);

                                BuildAlphaPlane(lsAlpha, OutBuf, alphaOffset);
                            }
                        }
                    }
                }
                else
                {
                    byte[] raw = ReadRawImageDataAt(position, nSize);
                    FillDibFromBuffer(Source, raw, nSize);
                }

                if (D3DFormat && (Source.Width >= 400) && (Source.Height >= 400))
                {
                    // 接缝：FileDataBright32(Source, FileData, FileSize) 待 GameImages.pas 移植后接入
                    if (!boDecompressError)
                        dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(Source, lsAlpha));
                    else
                        dXImage.Bright = TTextureRef.NullTextureSentinel;
                }
                else
                {
                    // 修改chongchong 2015-07-30 14:05:59（原文 988-993）
                    if (!boDecompressError)
                        dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(Source, lsAlpha));
                    else
                        dXImage.Bright = TTextureRef.NullTextureSentinel;
                }
            }
        }
    }

    // =============================== 缓存访问（1009-1463） ===============================

    /// <summary>Wzl.pas 1009-1038 GetCachedBitmap 1:1（LoadDxBitmap 为空 → Bitmap 槽位恒 nil）。</summary>
    public TTextureRef GetCachedBitmap(int index)
    {
        TTextureRef Result = default;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && (m_FileStream != null) && Initialized)
            {
                if (!m_ImgArr![index].Bitmap.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxBitmap(nPosition, m_ImgArr[index], index);
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();

                    if (m_ImgArr[index].Bitmap.Assigned)
                        IndexList.Add(index);

                    Result = m_ImgArr[index].Bitmap;
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    Result = m_ImgArr[index].Bitmap;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1040-1093 GetCachedSurface 1:1。</summary>
    public TTextureRef GetCachedSurface(int index)
    {
        TTextureRef Result = default;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxImage(nPosition, m_ImgArr[index], index);
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();

                    if (m_ImgArr[index].Surface.Assigned)
                        IndexList.Add(index);

                    Result = m_ImgArr[index].Surface;

                    if (!Result.Assigned && g_boAutoUpdate && m_boNeedUpdate && (!m_ImgArr[index].boUpdateStop)
                        && ((!m_ImgArr[index].boUpdateStart) || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 1 /* udtImageWzl */, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }
                    if (!Result.Assigned)
                        Result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    Result = m_ImgArr[index].Surface;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                    {
                        m_boUpdateIndexing = true;
                        m_dwUpdateIndexingTick = MyGetTickCount();
                    }
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1095-1148 GetCachedGray 1:1。</summary>
    public TTextureRef GetCachedGray(int index)
    {
        TTextureRef Result = default;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxGrayImage(nPosition, m_ImgArr[index], index);
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();

                    if (m_ImgArr[index].Gray.Assigned)
                        GrayIndexList.Add(index);

                    Result = m_ImgArr[index].Gray;

                    if (!Result.Assigned && g_boAutoUpdate && m_boNeedUpdate && (!m_ImgArr[index].boUpdateStop)
                        && ((!m_ImgArr[index].boUpdateStart) || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 1 /* udtImageWzl */, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }
                    if (!Result.Assigned)
                        Result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    Result = m_ImgArr[index].Gray;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                    {
                        m_boUpdateIndexing = true;
                        m_dwUpdateIndexingTick = MyGetTickCount();
                    }
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1150-1203 GetCachedBright 1:1。</summary>
    public TTextureRef GetCachedBright(int index)
    {
        TTextureRef Result = default;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxBrightImage(nPosition, m_ImgArr[index], index);
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();

                    if (m_ImgArr[index].Bright.Assigned)
                        BrightIndexList.Add(index);

                    Result = m_ImgArr[index].Bright;

                    if (!Result.Assigned && g_boAutoUpdate && m_boNeedUpdate && (!m_ImgArr[index].boUpdateStop)
                        && ((!m_ImgArr[index].boUpdateStart) || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 1 /* udtImageWzl */, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }
                    if (!Result.Assigned)
                        Result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    Result = m_ImgArr[index].Bright;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                    {
                        m_boUpdateIndexing = true;
                        m_dwUpdateIndexingTick = MyGetTickCount();
                    }
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1205-1262 GetCachedImage 1:1（带 var PX, PY 输出）。</summary>
    public TTextureRef GetCachedImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxImage(nPosition, m_ImgArr[index], index);

                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;

                    if (m_ImgArr[index].Surface.Assigned)
                        IndexList.Add(index);

                    Result = m_ImgArr[index].Surface;
                    if (!Result.Assigned && g_boAutoUpdate && m_boNeedUpdate && (!m_ImgArr[index].boUpdateStop)
                        && ((!m_ImgArr[index].boUpdateStart) || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 1 /* udtImageWzl */, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }
                    if (!Result.Assigned)
                        Result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Surface;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                    {
                        m_boUpdateIndexing = true;
                        m_dwUpdateIndexingTick = MyGetTickCount();
                    }
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1264-1315 GetCachedImageSize 1:1。</summary>
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
            if ((aIndex >= 0) && (aIndex < ImageCount) && (aIndex < m_IndexList.Count) && (m_FileStream != null) && Initialized)
            {
                if (m_ImgArr![aIndex].nWidth * m_ImgArr[aIndex].nHeight == 0)
                {
                    int nPosition = m_IndexList[aIndex];

                    if ((nPosition > 0) && (nPosition < m_FileStream.Length))
                    {
                        // 原文 1282-1288：LockFileStream → Seek(nPosition, soBeginning) → Read(ImageInfo, SizeOf) → UnLockFileStream
                        var infoBuf = new byte[TWzlImageInfo.SizeOf];
                        LockFileStream();
                        try
                        {
                            m_FileStream.Seek(nPosition, SeekOrigin.Begin);
                            TWzlImageInfo.ReadFully(m_FileStream, infoBuf, 0, TWzlImageInfo.SizeOf);
                        }
                        finally
                        {
                            UnLockFileStream();
                        }
                        var ImageInfo = TWzlImageInfo.ReadLegacyView(infoBuf, 0);

                        m_ImgArr[aIndex].nWidth = (ushort)ImageInfo.nWidth;
                        m_ImgArr[aIndex].nHeight = (ushort)ImageInfo.nHeight;
                        m_ImgArr[aIndex].nPx = (short)ImageInfo.px;
                        m_ImgArr[aIndex].nPy = (short)ImageInfo.py;

                        cx = ImageInfo.nWidth;
                        cy = ImageInfo.nHeight;
                        pointX = ImageInfo.px;
                        pointY = ImageInfo.py;
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
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1317-1373 GetCachedGrayImage 1:1。</summary>
    public TTextureRef GetCachedGrayImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxGrayImage(nPosition, m_ImgArr[index], index);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();

                    if (m_ImgArr[index].Gray.Assigned)
                        GrayIndexList.Add(index);

                    Result = m_ImgArr[index].Gray;
                    if (!Result.Assigned && g_boAutoUpdate && m_boNeedUpdate && (!m_ImgArr[index].boUpdateStop)
                        && ((!m_ImgArr[index].boUpdateStart) || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 1 /* udtImageWzl */, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }
                    if (!Result.Assigned)
                        Result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Gray;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                    {
                        m_boUpdateIndexing = true;
                        m_dwUpdateIndexingTick = MyGetTickCount();
                    }
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1375-1431 GetCachedBrightImage 1:1。</summary>
    public TTextureRef GetCachedBrightImage(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        if (index < 0) return Result;

        Lock();
        try
        {
            if ((index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxBrightImage(nPosition, m_ImgArr[index], index);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();

                    if (m_ImgArr[index].Bright.Assigned)
                        BrightIndexList.Add(index);

                    Result = m_ImgArr[index].Bright;
                    if (!Result.Assigned && g_boAutoUpdate && m_boNeedUpdate && (!m_ImgArr[index].boUpdateStop)
                        && ((!m_ImgArr[index].boUpdateStart) || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 1 /* udtImageWzl */, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }
                    if (!Result.Assigned)
                        Result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Bright;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    if (UpdateEngineAddFn != null && UpdateEngineAddFn(FileName, 0 /* udtIndexWzl */, -1, this, null))
                    {
                        m_boUpdateIndexing = true;
                        m_dwUpdateIndexingTick = MyGetTickCount();
                    }
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    /// <summary>Wzl.pas 1433-1463 GetBitmap 1:1。</summary>
    public TTextureRef GetBitmap(int index, out int px, out int py)
    {
        TTextureRef Result = default;
        px = 0;
        py = 0;
        if (index < 0) return Result;

        Lock();
        try
        {
            // 原文 1442：Initialized and (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized（重复判断照抄）
            if (Initialized && (index < ImageCount) && (index < m_IndexList.Count) && Initialized)
            {
                if (!m_ImgArr![index].Bitmap.Assigned)
                {
                    int nPosition = m_IndexList[index];
                    LoadDxBitmap(nPosition, m_ImgArr[index], index);
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    if (m_ImgArr[index].Bitmap.Assigned)
                        IndexList.Add(index);

                    Result = m_ImgArr[index].Bitmap;
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    Result = m_ImgArr[index].Bitmap;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return Result;
    }

    // =============================== 内部助手 ===============================

    /// <summary>
    /// GameImages.pas 1055-1087 MakeDibByPixelFormat 1:1。
    /// <para>pf8bit → TDIB.Create + ColorTable := g_DefColorTable + UpdatePalette + SetSize(nW, nH, 8)；
    /// pf15bit/pf16bit → MakeDIBPixelFormat(5, 6, 5) + SetSize(..., 16)；pf24bit → (8, 8, 8) + 24；
    /// pf32bit → (8, 8, 8) + 32；其它 → nil。</para>
    /// </summary>
    public static TDib? MakeDibByPixelFormat(byte pf, int nW, int nH)
    {
        switch (pf)
        {
            case (byte)TPixelFormat.pf8bit:
            {
                var result = TDib.Create();
                result.SetSize(nW, nH, 8);
                return result;
            }

            case (byte)TPixelFormat.pf15bit:
            case (byte)TPixelFormat.pf16bit:
            {
                var result = TDib.Create();
                result.SetSize(nW, nH, 16);
                return result;
            }

            case (byte)TPixelFormat.pf24bit:
            {
                var result = TDib.Create();
                result.SetSize(nW, nH, 24);
                return result;
            }

            case (byte)TPixelFormat.pf32bit:
            {
                var result = TDib.Create();
                result.SetSize(nW, nH, 32);
                return result;
            }

            default:
                return null;
        }
    }

    /// <summary><c>Move(OutBuf^, Source.PBits^, nSize)</c>：把解压结果铺满 8bit 扫描线平面（不足部分保持 0）。</summary>
    private static void FillDibFromBuffer(TDib source, byte[] buf, int nSize)
    {
        int count = Math.Min(nSize, Math.Min(buf.Length, source.Bits.Length));
        if (count > 0)
            Array.Copy(buf, 0, source.Bits, 0, count);
    }

    /// <summary>
    /// 原文 950-964 / 721-734 / 487-500 的 alpha 平面展开（三处逐字相同）：
    /// <code>
    /// LineData := lsAlpha.ScanLine[nY];
    /// pData := @SourceAlphaBuf[nY * lsAlpha.Width div 2];
    /// nX_2 := nX div 2;
    /// if nX mod 2 = 0 then LineData^ := pData[nX_2]
    ///                 else LineData^ := (pData[nX_2] + pData[nX_2 + 1]) div 2;
    /// </code>
    /// </summary>
    private static void BuildAlphaPlane(TDib lsAlpha, byte[] outBuf, int alphaOffset)
    {
        for (int nY = 0; nY <= lsAlpha.Height - 1; nY++)
        {
            int rowBase = alphaOffset + (nY * lsAlpha.Width / 2);
            var lineData = new byte[lsAlpha.Width];
            for (int nX = 0; nX <= lsAlpha.Width - 1; nX++)
            {
                int nX_2 = nX / 2;
                byte b0 = GetBufByte(outBuf, rowBase + nX_2);
                byte b1 = GetBufByte(outBuf, rowBase + nX_2 + 1);
                lineData[nX] = nX % 2 == 0 ? b0 : (byte)((b0 + b1) / 2);
            }
            lsAlpha.WriteScanLine(nY, lineData, 0, lsAlpha.Width);
        }
    }

    /// <summary>越界读返回 0（对齐原文越界读的容错意图；原文会读到相邻内存）。</summary>
    private static byte GetBufByte(byte[] buf, int index)
        => index >= 0 && index < buf.Length ? buf[index] : (byte)0;
}

/// <summary>
/// Graphics.pas <c>TPixelFormat</c>（Delphi 7，SizeOf = 1）。MakeDibByPixelFormat 的 case 分派依据。
/// </summary>
public enum TPixelFormat : byte
{
    pfDevice = 0,
    pf1bit = 1,
    pf4bit = 2,
    pf8bit = 3,
    pf15bit = 4,
    pf16bit = 5,
    pf24bit = 6,
    pf32bit = 7,
    pfCustom = 8,
}
