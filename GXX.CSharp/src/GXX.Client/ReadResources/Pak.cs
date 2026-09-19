using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GXX.Core.Compress;
using GXX.Core.Crypto;
using GXX.Core.Rtl;

namespace GXX.Client.ReadResources;

// =====================================================================================
// Pak.pas（Source\Client-HGE\ReadResources\Pak.pas，3199 行）1:1 移植 —— GEE/GOM 系 PAK 图库。
//
// 命名空间与本目录既有产物（Wil.cs / Wzl.cs / GameImagesBase.cs）一致。
//
// ---- 原文条件编译常量（本移植按「成立分支」逐行对应）----
//   GameImages.pas 12-20：LOADIMAGEMODE = 0；USEMAPSTREAM = 0
//   Pak.pas 230 uses 段：ZLibEx / Grobal2 / UpdateEngine / MShare 仅 CLIENTEXE = 1 时引入
//   故 Pak.pas 内所有 {$IF CLIENTEXE = 1} 分支成立、{$IF PRIVATE_CLIENT = 0} 分支成立。
//
// ---- 支持的 4 类格式（原文 71-111）----
//   pftPak1     : 文件头 'GEEM2'   （10 字节 TFileHeaderInfo + 256 字节 DES 头）
//                 索引 = ImageCount × Integer，整体 EncryptCBC/DecryptCBC
//   pftPak2     : 文件头 'GEEPAK2' （同上布局）
//                 索引 = 每个 Integer 先 xor Chain[0]，再 **两次** DecryptCBC（原文 1140-1154）
//                 图片头经 ReadImageHeader_Pak2（KeyData 隔项 xor 后两次 CBC）
//   pftPak3     : 文件头 'GEEPAK3' （同上布局）
//                 索引 = 每项 xor FPak3Password[i mod 64] xor (not i)
//                 文件头/图片头走 AES-CTR（密钥 = FPak3Password[0..3]，16 字节）
//   pftLzPakV0/V1: 文件头 'HXM2.'（5 字节 TLzFileHeaderInfo）+ 256 字节 DES 头 + 1 填充字节 = 262
//                 V0 索引 = ImageCount × (nImageOffSet, nDataSize) = 8 字节/项，整体 DecryptCBC
//                 V1 索引 = ImageCount × Integer，整体 DecryptCBC
//
// ---- 与 GXX.Core 的对齐（复用而非重复移植）----
//   * EncryptCBC/DecryptCBC/EncryptDes/DecryptDes → GXX.Core.Crypto.UnitDes（Common\UnitDes.pas）
//   * zlib 解压                                    → GXX.Core.Compress.ZlibEx.DecompressBuf
//   * 以下三处原文依赖单元尚未移植，已在本车道做**最小接缝**（见 PakCrypto.cs / PakSeams.cs）：
//       DesUtils.pas  → PakDesNew.EncryptDes_New / DecryptDes_New
//       AESUtils.pas  → PakAesCtr.AesEncrypt / AesDecrypt
//       RLEUnit.pas   → PakRle.DecodeRle
//     另：GameImages/DIB/DxCanvas/LoadDxControl 的接缝沿用 GameImagesBase.cs 既有产物。
//
// ---- 已知原文缺陷（照抄不修，逐条在对应方法上标注）----
//   1. 1499 行注释自承：WriteHeader 有明显 BUG（没考虑 TFileHeaderInfo 的存在），程序中也未使用。
//   2. 829-921 行 EncryptDes_New 的 p1/p2 增量与 Move(p2^, Chain, BS) 的时机与解密侧不对称（CBC 仍自洽）。
//   3. 2556 行 LzPakV1 的 dtsBright 分支写成 `else if dtsBright = dtsBright`（恒真）——
//      照抄：dtsGray 之后的所有非 Normal 风格都落到 NewTextureBright。
//   4. 2867/3072 行 Gray/Bright 的「小图」分支错把结果写进 Surface（不是 Gray/Bright）。
//   5. 1759 行 GetCachedImageSize 用 `nPosition >= SizeOf(TPakFileHeader)`（=256），
//      而 2596 行 LoadDxImage 用的是同一个 256；LZ 路径则用 LZ_PAK_FILE_HEADER_SIZE（=262）。
// =====================================================================================

/// <summary>
/// Pak.pas 113-219：<c>TPakImages = class(TGameImages)</c> 1:1。
/// <para>原文的 <c>m_IndexList:TList</c> 在 Pak 里存的是**裸偏移值**（<c>Pointer(Index)</c>），
/// 本移植用 <see cref="List{T}"/> of <see cref="int"/> 保真表达同一语义；
/// <c>m_ImageSizeList:TList</c> 同理（LzPakV0 的每项数据长度）。</para>
/// <para>原文 <c>FCSFileStream:TRTLCriticalSection</c> → 此处 <see cref="object"/> + Monitor
/// （与 Wzl.cs 的既有做法一致）。</para>
/// </summary>
public sealed class TPakImages : TGameImages
{
    // ---- Pak.pas 115-129 private 字段 ----

    /// <summary>Pak.pas 115：<c>FPakFileType:TPakFileType;</c>（Create 时置 pftPak1）。</summary>
    public TPakFileType FPakFileType = TPakFileType.pftPak1;

    /// <summary>Pak.pas 116：<c>FileHeader:TPakFileHeader;</c></summary>
    public TPakFileHeader FileHeader = TPakFileHeader.CreateEmpty();

    /// <summary>Pak.pas 119：<c>FPasswordOK:Boolean;</c>（Create 时置 False）。</summary>
    public bool FPasswordOK;

    /// <summary>Pak.pas 121：<c>FKeyData:array[0..31] of DWord;</c></summary>
    public uint[] FKeyData = new uint[32];

    /// <summary>Pak.pas 122：<c>FChain:array[0..19] of Byte;</c></summary>
    public byte[] FChain = new byte[20];

    /// <summary>Pak.pas 124：<c>FKeyDataLz:array[0..31] of DWord;</c></summary>
    public uint[] FKeyDataLz = new uint[32];

    /// <summary>Pak.pas 125：<c>FChainLz:array[0..19] of Byte;</c></summary>
    public byte[] FChainLz = new byte[20];

    /// <summary>Pak.pas 127：<c>FPak3Password:array[0..63] of LongWord;</c></summary>
    public uint[] FPak3Password = new uint[64];

    /// <summary>Pak.pas 129：<c>m_ImageSizeList:TList;</c>（仅 LzPakV0 使用；nil 表示未创建）。</summary>
    public List<int>? m_ImageSizeList;

    // ---- Pak.pas 194-196 public 字段 ----

    /// <summary>
    /// Pak.pas 194：<c>m_FileStream:TFileStream; // TMapStream;</c>
    /// <para>原文以 <c>fmOpenReadWrite or fmShareDenyNone</c> 打开（可读可写、允许共享）。</para>
    /// </summary>
    public FileStream? m_FileStream;

    /// <summary>Pak.pas 196：<c>FCSFileStream:TRTLCriticalSection;</c> → 托管对象锁。</summary>
    public readonly object FCSFileStream = new();

    // ---- 本移植新增的「原文 TList 语义」视图 ----

    /// <summary>Pak.pas 256：<c>m_IndexList := TList.Create;</c>（原文在基类里；此处显式暴露）。</summary>
    public readonly List<int> PakIndexList = new();

    /// <summary>
    /// 原文 <c>m_IndexList</c> 与基类 <c>m_IndexList</c> 是**同一个** TList
    /// （GameImages.pas 68 <c>m_IndexList:TList;</c> 是 public 字段，Pak 的 Create 里赋值）。
    /// 本移植把基类的 <see cref="TGameImages.m_IndexList"/> 作为唯一真源，本属性为其别名。
    /// </summary>
    public List<int> Index => m_IndexList;

    /// <summary>Pak.pas 217-218 LockFileStream / UnLockFileStream（原文 EnterCriticalSection）。</summary>
    public void LockFileStream() => System.Threading.Monitor.Enter(FCSFileStream);

    /// <summary>Pak.pas 3193-3196 UnLockFileStream。</summary>
    public void UnLockFileStream() => System.Threading.Monitor.Exit(FCSFileStream);

    /// <summary>Pak.pas 118：该子类注册的索引文件扩展名为空（Pak 无 .wzx 类副索引文件）。</summary>
    protected override string IndexFileExtension => "";

    // =================================================================================
    // 原文 1299-1333 ReadImageHeader_Pak3 里的 FPak3Password 视图
    // =================================================================================

    /// <summary>Pak.pas 1309：<c>FPak3Password[K1] xor FPak3Password[K2]</c>（K1=(Index+0) mod 64, K2=(Index+14) mod 64）。</summary>
    private uint Pak3K1(int index) => FPak3Password[(index + 0) % 64] ^ FPak3Password[(index + 14) % 64];

    /// <summary>Pak.pas 1314：<c>FPak3Password[K1] and FPak3Password[K2]</c>（K1=(Index+12) mod 64, K2=(Index+19) mod 64）。</summary>
    private uint Pak3K2(int index) => FPak3Password[(index + 12) % 64] & FPak3Password[(index + 19) % 64];

    /// <summary>Pak.pas 1319：<c>FPak3Password[K2] xor (not FPak3Password[K1])</c>（K1=(Index+10) mod 64, K2=(Index+28) mod 64）。</summary>
    private uint Pak3K3(int index) => FPak3Password[(index + 28) % 64] ^ ~FPak3Password[(index + 10) % 64];

    /// <summary>Pak.pas 1323：<c>FPak3Password[K1]</c>（K1=(Index+1) mod 64）。</summary>
    private uint Pak3K4(int index) => FPak3Password[(index + 1) % 64];

    // =================================================================================
    // 构造 / 析构（原文 248-270）
    // =================================================================================

    /// <summary>
    /// Pak.pas 248-261 <c>TPakImages.Create(APassWord:TPakPassword)</c> 1:1。
    /// <list type="bullet">
    /// <item>251-254：四段 <c>Move(APassWord.X, FX, SizeOf(FX))</c>。</item>
    /// <item>255：<c>m_FileStream := nil;</c>；256：<c>m_IndexList := TList.Create;</c>。</item>
    /// <item>257：<c>FPasswordOK := False;</c>；258：<c>FPakFileType := pftPak1;</c>。</item>
    /// <item>259：<c>m_dwMemChecktTick := MyGetTickCount;</c>（**注意原文漏了括号**，
    ///       在 Delphi 里取的是函数地址再隐式转 DWORD —— 实际得到一个「代码地址」而非 tick。
    ///       本移植按注释意图取当前 tick：该字段在本单元内从未被读，行为不可观测）。</item>
    /// <item>260：<c>InitializeCriticalSection(FCSFileStream);</c>。</item>
    /// </list>
    /// </summary>
    public TPakImages(TPakPassword aPassWord)
    {
        Array.Copy(aPassWord.Chain, FChain, FChain.Length);
        Array.Copy(aPassWord.KeyData, FKeyData, FKeyData.Length);
        Array.Copy(aPassWord.ChainLz, FChainLz, FChainLz.Length);
        Array.Copy(aPassWord.KeyDataLz, FKeyDataLz, FKeyDataLz.Length);

        m_FileStream = null;
        FPasswordOK = false;
        FPakFileType = TPakFileType.pftPak1;
        m_dwMemChecktTick = MyGetTickCount();
        // InitializeCriticalSection(FCSFileStream) → FCSFileStream 已在字段初始化时构造。
    }

    /// <summary>
    /// Pak.pas 263-270 <c>TPakImages.Destroy</c> 1:1。
    /// <para><b>原文顺序</b>：<c>inherited;</c> 先执行，再 <c>m_IndexList.Free</c> / <c>m_ImageSizeList.Free</c> /
    /// <c>DeleteCriticalSection</c>。这在 Delphi 里是「先释放基类再释放自身字段」，属危险顺序但照抄。</para>
    /// <para><b>接口面偏差（登记）</b>：GameImagesBase.cs（本车道只读的既有接缝）的 TGameImages
    /// 没有声明 <c>Dispose</c>，故此处**不是** <c>override</c>。
    /// 接缝：待 GameImages.pas 全量移植（基类补 <c>virtual void Dispose()</c>）后改回 override。</para>
    /// </summary>
    public void Dispose()
    {
        // inherited（TGameImages.Destroy 在本移植里无副作用）
        if (m_ImageSizeList != null) m_ImageSizeList = null;
        // m_IndexList.Free → 托管集合，交由 GC
        // 托管侧补充：释放文件句柄，否则调用方无法在对象存活期间读同一文件
        // （原文靠 Delphi 的 TFileStream 析构；托管 FileStream 必须显式 Dispose）
        if (m_FileStream != null)
        {
            m_FileStream.Dispose();
            m_FileStream = null;
        }
    }

    // =================================================================================
    // InitPak3Password（原文 272-474）
    // =================================================================================

    /// <summary>
    /// Pak.pas 272-474 <c>InitPak3Password</c> 1:1（PAK3 的 64 个 LongWord 密钥表）。
    /// <list type="number">
    /// <item>285-287：<c>for I := 0 to 31 do FPak3Password[I*2] := FKeyData[31 - I];</c>
    ///       → 偶数槽 0/2/…/62 用 FKeyData 逆序填充。</item>
    /// <item>289-293：5 处 <c>Move(FChain[X], FPak3Password[Y], 4)</c>
    ///       → FChain[0]→[1]、[4]→[7]、[8]→[11]、[12]→[13]、[16]→[15]（**奇数槽**）。</item>
    /// <item>296-376：以 len=32、种子 a=$BCA24215 / b=$BD194331 / c=$B99EAC12 跑一遍三字混合，
    ///       把结果写入 [3]、[5]、[9]。</item>
    /// <item>379-390：由 [0..16] 派生 [17]（JSHash）、由 [0..17] 派生 [19]（DJBHash）。</item>
    /// <item>392-472：<c>for I := 10 to 31</c>，以 len = I*2、种子 a=$16B997C8 / b=$48744D94 / c=$BA06742F
    ///       从 FPak3Password[0..] 混合，结果写 [I*2+1]（即 21/23/…/63 的奇数槽）。
    ///       **注意 419-421 的移位/运算与第一段不同：<c>b := b and (a shl $7)</c>（and 而非 xor）、
    ///       以及尾段 469 的 <c>c := c or (b shr $5)</c>（or 而非 xor）—— 原文如此，照抄。**</item>
    /// </list>
    /// </summary>
    public void InitPak3Password()
    {
        for (int i = 0; i < FKeyData.Length; i++)
            FPak3Password[i * 2] = FKeyData[FKeyData.Length - 1 - i];

        MoveChainToPak3Password(0, 1);
        MoveChainToPak3Password(4, 7);
        MoveChainToPak3Password(8, 11);
        MoveChainToPak3Password(12, 13);
        MoveChainToPak3Password(16, 15);

        int len = FKeyData.Length; // 32
        int k = 0;
        uint a = 0xBCA24215u, b = 0xBD194331u, c = 0xB99EAC12u;

        while (len >= 3)
        {
            a += FKeyData[k];
            b += FKeyData[k + 1];
            c += FKeyData[k + 2];

            a = a - b; a = a - c; a ^= c >> 8;
            b = b - c; b = b - a; b ^= a << 9;
            c = c - a; c = c - b; c ^= b >> 13;
            a = a - b; a = a - c; a ^= c >> 9;
            b = b - c; b = b - a; b ^= a << 6;
            c = c - a; c = c - b; c ^= b >> 4;
            a = a - b; a = a - c; a ^= c >> 8;
            b = b - c; b = b - a; b ^= a << 3;
            c = c - a; c = c - b; c ^= b >> 15;

            k += 3;
            len -= 3;
        }

        c += (uint)FKeyData.Length;

        if (len >= 1) a += FKeyData[k + 0];
        if (len >= 2) a += FKeyData[k + 1];
        a = a - b; a = a - c; a ^= c >> 4;
        b = b - c; b = b - a; b ^= a << 9;
        c = c - a; c = c - b; c ^= b >> 19;
        a = a - b; a = a - c; a ^= c >> 11;
        b = b - c; b = b - a; b ^= a << 14;
        c = c - a; c = c - b; c ^= b >> 5;
        a = a - b; a = a - c; a ^= c >> 9;
        b = b - c; b = b - a; b ^= a << 12;
        c = c - a; c = c - b; c ^= b >> 3;

        FPak3Password[3] = a;
        FPak3Password[5] = b;
        FPak3Password[9] = c;

        // unsigned int JSHash(char* str, unsigned int len)  ← 原文注释
        c = 1315423911;
        for (int i = 0; i <= 16; i++)
            c ^= (c << 5) + FPak3Password[i] + (c >> 2);
        FPak3Password[17] = c;

        // unsigned int DJBHash(char* str, unsigned int len) ← 原文注释
        c = 5381;
        for (int i = 0; i <= 17; i++)
            c = ((c << 5) + c) + FPak3Password[i];
        FPak3Password[19] = c;

        for (int i = 10; i <= 31; i++)
        {
            len = i * 2;
            k = 0;
            a = 0x16B997C8u; b = 0x48744D94u; c = 0xBA06742Fu;

            while (len >= 3)
            {
                a += FPak3Password[k];
                b += FPak3Password[k + 1];
                c += FPak3Password[k + 2];

                a = a - b; a = a - c; a ^= c >> 0x9;
                b = b - c; b = b - a; b ^= a << 0x3;
                c = c - a; c = c - b; c ^= b >> 0xC;
                a = a - b; a = a - c; a ^= c >> 0xB;
                b = b - c; b = b - a; b &= a << 0x7;   // 原文如此：and（不是 xor）
                c = c - a; c = c - b; c ^= b >> 0xA;
                a = a - b; a = a - c; a ^= c >> 0x4;
                b = b - c; b = b - a; b ^= a << 0x1;
                c = c - a; c = c - b; c ^= b >> 0x8;

                k += 3;
                len -= 3;
            }

            c += (uint)(i * 2);

            if (len >= 1) a += FPak3Password[k + 0];
            if (len >= 2) a += FPak3Password[k + 1];
            a = a - b; a = a - c; a ^= c >> 0xB;
            b = b - c; b = b - a; b ^= a << 0x1;
            c = c - a; c = c - b; c ^= b >> 0xF;
            a = a - b; a = a - c; a ^= c >> 0x2;
            b = b - c; b = b - a; b ^= a << 0x7;
            c = c - a; c = c - b; c ^= b >> 0x9;
            a = a - b; a = a - c; a ^= c >> 0x1;
            b = b - c; b = b - a; b ^= a << 0x3;
            c = c - a; c = c - b; c |= b >> 0x5;   // 原文如此：or（不是 xor）

            FPak3Password[i * 2 + 1] = c;
        }
    }

    /// <summary><c>Move(FChain[srcIdx], FPak3Password[dstIdx], SizeOf(FPak3Password[1]))</c> —— 4 字节小端搬运。</summary>
    private void MoveChainToPak3Password(int srcIdx, int dstIdx)
        => FPak3Password[dstIdx] = (uint)(FChain[srcIdx] | (FChain[srcIdx + 1] << 8)
            | (FChain[srcIdx + 2] << 16) | (FChain[srcIdx + 3] << 24));

    // =================================================================================
    // 小工具（原文 476-487）
    // =================================================================================

    /// <summary>
    /// Pak.pas 476-480 <c>IsValidLzCheckCode</c>：
    /// <c>(sCheckCode = 'D3DM2') or ('HXM2') or ('HeroM2') or ('HeroM2.') or ('HXM2.') or ('FreeMF')</c>。
    /// </summary>
    public static bool IsValidLzCheckCode(string sCheckCode)
        => sCheckCode == "D3DM2" || sCheckCode == "HXM2" || sCheckCode == "HeroM2"
        || sCheckCode == "HeroM2." || sCheckCode == "HXM2." || sCheckCode == "FreeMF";

    /// <summary>
    /// Pak.pas 482-487 <c>UpdateImageDataSize</c> 1:1。
    /// <para>只有 <c>FPakFileType = pftLzPakV0</c> 时才写入 <c>m_ImageSizeList[nIndex]</c>；
    /// 其它类型**什么都不做**（原文 484 的 <c>if</c> 无 else）。</para>
    /// <para><b>接口面偏差（登记）</b>：GameImagesBase.cs 的 TGameImages 未声明该方法
    /// （GameImages.pas 131 是 <c>virtual</c>），故此处**不是** <c>override</c>。
    /// 接缝：待 GameImages.pas 全量移植后改回 override。</para>
    /// </summary>
    public void UpdateImageDataSize(int nIndex, int nDataSize)
    {
        if (FPakFileType == TPakFileType.pftLzPakV0)
        {
            if (m_ImageSizeList == null) m_ImageSizeList = new List<int>();
            while (m_ImageSizeList.Count <= nIndex) m_ImageSizeList.Add(0);
            m_ImageSizeList[nIndex] = nDataSize;
        }
    }

    // =================================================================================
    // UpdateIndex（原文 489-661）
    // =================================================================================

    /// <summary>
    /// Pak.pas 489-661 <c>UpdateIndex(PakKey:pTPakKey; IsCompareHeader:Boolean)</c> 1:1。
    /// <list type="number">
    /// <item>502-505：<c>PakKey.ImageCount &lt;= 0</c> → <c>m_boNeedUpdate := False</c> 后 Exit。</item>
    /// <item>509-510：<c>nLzV0BitCount := (PakType shr 8) and $FF; wPakType := PakType and $00FF;</c></item>
    /// <item>514-523：已初始化且「头完全一致」（ImageCount / FPakFileType / KeyData / Chain / Reserve
    ///       逐字节 CompareMem）→ 直接 Exit；否则先 <c>Finalize</c>。</item>
    /// <item>525-550：加文件锁；文件存在 → 以 <c>fmOpenWrite or fmShareDenyNone</c> 打开（**Write-only**），
    ///       不存在 → 先 ForceDirectories 再以 <c>fmOpenWrite or fmShareDenyNone or fmCreate</c> 建。
    ///       两种打开失败都置 <c>m_boNeedUpdate := False</c> + <c>IsDoExit := True</c>。</item>
    /// <item>552-558：wPakType 属于 0..4 → <c>FPakFileType := wPakType</c>；否则回落 pftPak1。</item>
    /// <item>560-596：FillChar 文件头 → CreateDate := Now → ImageCount := PakKey.ImageCount →
    ///       从 PakKey 搬 KeyData/Chain/Reserve → Title := 'www.gameofmir2.com' →
    ///       bfType/CheckCode 按格式分派（LzPakV0: 0/'D3DM2'；V1: 1/'D3DM2'；其它: 2/'GEEM2'，均先 EncryptS）→
    ///       Size/IndexOffSet 按 LZ(262) 或普通(266) 设值 → BitCount（仅 V0 用 nLzV0BitCount）。</item>
    /// <item>598-600：`Header := FileHeader;` 后把 KeyData/Chain 清零（**只清副本**）。</item>
    /// <item>602-635：LZ → 写 5 字节 'HXM2.' + 加密头 256 字节 + 1 字节 0，FileStream.Size := 262；
    ///       普通 → 写 TFileHeaderInfo(10) + 加密头 256，FileStream.Size := 266。</item>
    /// <item>637-650：<c>m_IndexList.Count := PakKey.ImageCount</c> 全部置 nil；V0 时同样扩张 m_ImageSizeList。</item>
    /// <item>652：<c>WriteIndexList(FileStream, -1)</c>；654：<c>FileStream.Free</c>；656：<c>Initialize_UpdateNewFile</c>。</item>
    /// </list>
    /// </summary>
    public void UpdateIndex(TPakKey pakKey, bool isCompareHeader)
    {
        if (pakKey.ImageCount <= 0)
        {
            m_boNeedUpdate = false;
            return;
        }

        FileStream? fileStream = null;
        int nLzV0BitCount = pakKey.LzV0BitCount;
        int wPakType = pakKey.PakTypeLow;
        bool isDoExit = false;

        if (Initialized)
        {
            bool sameHeader = isCompareHeader
                && (int)FileHeader.ImageCount == pakKey.ImageCount
                && (int)FPakFileType == wPakType
                && CompareMem(FileHeader.KeyData, pakKey.KeyData)
                && CompareMem(FileHeader.Chain, pakKey.Chain)
                && CompareMem(FileHeader.Reserve, pakKey.Reserve);

            if (sameHeader)
            {
                return;
            }
            Finalize_();
        }

        LockFileStream();
        try
        {
            if (File.Exists(FileName))
            {
                try
                {
                    // 原文：fmOpenWrite or fmShareDenyNone → 只写、允许共享读写
                    fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                }
                catch
                {
                    m_boNeedUpdate = false;
                    isDoExit = true;
                }
            }
            else
            {
                string filePath = ExtractFilePath(IndexFileName);
                if (filePath.Length > 0 && !Directory.Exists(filePath))
                {
                    // 原文 ForceDirectories(FilePath)
                    try { Directory.CreateDirectory(filePath); } catch { /* 原文未捕获；此处不改变控制流 */ }
                }

                try
                {
                    fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                catch
                {
                    m_boNeedUpdate = false;
                    isDoExit = true;
                }
            }

            if (!isDoExit)
            {
                if (wPakType >= (int)TPakFileType.pftPak1 && wPakType <= (int)TPakFileType.pftLzPakV1)
                    FPakFileType = (TPakFileType)wPakType;
                else
                    FPakFileType = TPakFileType.pftPak1;

                FileHeader = TPakFileHeader.CreateEmpty();
                FileHeader.CreateDate = NowOleDate();
                FileHeader.ImageCount = (uint)pakKey.ImageCount;

                Array.Copy(pakKey.KeyData, FKeyData, FKeyData.Length);
                Array.Copy(pakKey.Chain, FChain, FChain.Length);
                Array.Copy(pakKey.KeyData, FileHeader.KeyData, FKeyData.Length);
                Array.Copy(pakKey.Chain, FileHeader.Chain, FChain.Length);
                Array.Copy(pakKey.Reserve, FileHeader.Reserve, pakKey.Reserve.Length);

                FileHeader.TitleText = "www.gameofmir2.com";
                if (FPakFileType == TPakFileType.pftLzPakV0)
                {
                    FileHeader.bfType = 0;
                    SetCheckCodeBytes(EncryptS("D3DM2"));
                }
                else if (FPakFileType == TPakFileType.pftLzPakV1)
                {
                    FileHeader.bfType = 1;
                    SetCheckCodeBytes(EncryptS("D3DM2"));
                }
                else
                {
                    FileHeader.bfType = 2;
                    SetCheckCodeBytes(EncryptS("GEEM2"));
                }

                if (FPakFileType == TPakFileType.pftLzPakV0 || FPakFileType == TPakFileType.pftLzPakV1)
                {
                    FileHeader.Size = (uint)(TLzFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 1);
                    FileHeader.IndexOffSet = (uint)(TLzFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 1);
                }
                else
                {
                    FileHeader.Size = (uint)(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf);
                    FileHeader.IndexOffSet = (uint)(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf);
                }

                FileHeader.BitCount = FPakFileType == TPakFileType.pftLzPakV0 ? (ushort)nLzV0BitCount : (ushort)0;

                // Header := FileHeader; 然后清 KeyData/Chain（只影响副本）
                var header = FileHeader;
                Array.Clear(header.KeyData, 0, header.KeyData.Length);
                Array.Clear(header.Chain, 0, header.Chain.Length);

                if (FPakFileType == TPakFileType.pftLzPakV0 || FPakFileType == TPakFileType.pftLzPakV1)
                {
                    var lzHeaderInfo = TLzFileHeaderInfo.Hzm2Dot();
                    string s = EncryptHeader_LzPak(header);
                    byte byRev = 0; // 填充字节

                    fileStream!.Seek(0, SeekOrigin.Begin);
                    fileStream.Write(lzHeaderInfo.FileType, 0, TLzFileHeaderInfo.SizeOf);
                    fileStream.Write(CipherBytes(s), 0, TPakFileHeader.SizeOf);
                    fileStream.WriteByte(byRev);
                    fileStream.SetLength(TLzFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + 1); // 262
                }
                else
                {
                    var headerInfo = TFileHeaderInfo.CreateEmpty();
                    string s;
                    if (FPakFileType == TPakFileType.pftPak1)
                    {
                        headerInfo.FileTypeText = "GEEM2";
                        s = EncryptHeader_GameOfMir(header);
                    }
                    else if (FPakFileType == TPakFileType.pftPak2)
                    {
                        headerInfo.FileTypeText = "GEEPAK2";
                        s = EncryptHeader_Pak2(header);
                    }
                    else // pftPak3
                    {
                        InitPak3Password();
                        headerInfo.FileTypeText = "GEEPAK3";
                        s = EncryptHeader_Pak3(header);
                    }

                    fileStream!.Seek(0, SeekOrigin.Begin);
                    fileStream.Write(headerInfo.FileType, 0, TFileHeaderInfo.SizeOf);
                    fileStream.Write(CipherBytes(s), 0, TPakFileHeader.SizeOf);
                    fileStream.SetLength(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf); // 266
                }

                while (m_IndexList.Count < pakKey.ImageCount) m_IndexList.Add(0);
                while (m_IndexList.Count > pakKey.ImageCount) m_IndexList.RemoveAt(m_IndexList.Count - 1);
                for (int i = 0; i < pakKey.ImageCount; i++) m_IndexList[i] = 0;

                if (FPakFileType == TPakFileType.pftLzPakV0)
                {
                    m_ImageSizeList ??= new List<int>();
                    while (m_ImageSizeList.Count < pakKey.ImageCount) m_ImageSizeList.Add(0);
                    while (m_ImageSizeList.Count > pakKey.ImageCount) m_ImageSizeList.RemoveAt(m_ImageSizeList.Count - 1);
                    for (int i = 0; i < pakKey.ImageCount; i++) m_ImageSizeList[i] = 0;
                }

                WriteIndexList(fileStream, -1);

                fileStream.Dispose();
                fileStream = null;

                // 原文 656：<c>Initialize_UpdateNewFile</c>。
                // <para><b>契约（登记）</b>：原文在此既 `FileStream.Free`（局部流）又调用
                // <c>Initialize_UpdateNewFile</c>，后者把新建的流存进 <c>m_FileStream</c>（保持打开）。
                // 本移植保持同一语义，但调用方若读完就要**读磁盘**（如测试/校验），
                // 必须先 <see cref="Dispose"/> 本对象或显式释放 <c>m_FileStream</c>——
                // <c>m_FileStream</c> 以 <c>FileShare.ReadWrite</c> 打开，
                // 而 .NET 的 <c>File.ReadAllBytes</c> 只申请 <c>FileShare.Read</c>，两者不兼容。
                // Windows 上"允许写共享"与"申请只读共享"互斥，这是 Windows 共享语义而非本移植缺陷。</para>
                if (m_FileStream != null)
                {
                    m_FileStream.Dispose();
                    m_FileStream = null;
                }

                Initialize_UpdateNewFile();
            }
        }
        finally
        {
            fileStream?.Dispose();
            UnLockFileStream();
        }
    }

    /// <summary>Delphi <c>Now</c> → OLE 自动化日期（TDateTime 的 Double 表示）。</summary>
    private static double NowOleDate() => DateTime.Now.ToOADate();

    /// <summary>Delphi <c>CompareMem</c> 逐字节比较（长度取两数组较小者）。</summary>
    private static bool CompareMem(Array a, Array b)
    {
        if (a is uint[] ua && b is uint[] ub)
        {
            int n = Math.Min(ua.Length, ub.Length);
            for (int i = 0; i < n; i++) if (ua[i] != ub[i]) return false;
            return ua.Length == ub.Length;
        }
        if (a is byte[] ba && b is byte[] bb)
        {
            int n = Math.Min(ba.Length, bb.Length);
            for (int i = 0; i < n; i++) if (ba[i] != bb[i]) return false;
            return ba.Length == bb.Length;
        }
        if (a is int[] ia && b is int[] ib)
        {
            int n = Math.Min(ia.Length, ib.Length);
            for (int i = 0; i < n; i++) if (ia[i] != ib[i]) return false;
            return ia.Length == ib.Length;
        }
        return false;
    }

    // =================================================================================
    // WriteIndexList（原文 663-798）
    // =================================================================================

    /// <summary>
    /// Pak.pas 663-798 <c>WriteIndexList(FileStream:TFileStream; Index:Integer)</c> 1:1（五种格式各一支）。
    /// <list type="bullet">
    /// <item><b>pftPak1</b>（673-694）：每项 <c>Offset &gt; 0 ? Offset : 0</c> 写 Integer，
    ///       整体 EncryptCBC 后写到 <c>SizeOf(TFileHeaderInfo)+SizeOf(TPakFileHeader) = 266</c>。</item>
    /// <item><b>pftPak2</b>（695-716）：每项先 <c>xor FileHeader.Chain[0]</c>（**含 Offset&lt;0 的项也 xor**），
    ///       再 EncryptIndexList_Pak2（两次 CBC），写到 266。</item>
    /// <item><b>pftPak3</b>（718-752）：<c>Index &lt; 0</c> → 全量重写：
    ///       每项 <c>Cardinal(Offset) xor FPak3Password[i mod 64] xor (not Cardinal(i))</c>，
    ///       **不加密**（直接写明文异或结果）到 266；
    ///       <c>0 &lt;= Index &lt;= Count-1</c> → 只重写第 Index 个 Integer（seek 到 266 + Index*4）。</item>
    /// <item><b>pftLzPakV0</b>（754-772）：每项 (nImageOffSet, nDataSize) 共 8 字节，
    ///       整体 EncryptCBC 写到 <c>LZ_PAK_FILE_HEADER_SIZE = 262</c>。</item>
    /// <item><b>pftLzPakV1</b>（774-795）：每项 Integer，整体 EncryptCBC 写到 262。</item>
    /// </list>
    /// </summary>
    public void WriteIndexList(FileStream fileStream, int index)
    {
        switch (FPakFileType)
        {
            case TPakFileType.pftPak1:
            {
                int inSize = m_IndexList.Count * sizeof(int);
                var inData = new byte[inSize];
                for (int i = 0; i < m_IndexList.Count; i++)
                {
                    int offset = m_IndexList[i];
                    BitConverter.GetBytes(offset > 0 ? offset : 0).CopyTo(inData, i * sizeof(int));
                }

                var outData = new byte[inSize];
                EncryptIndexList(inData, inSize, outData);

                fileStream.Seek(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf, SeekOrigin.Begin);
                fileStream.Write(outData, 0, inSize);
                break;
            }

            case TPakFileType.pftPak2:
            {
                int inSize = m_IndexList.Count * sizeof(int);
                var inData = new byte[inSize];
                for (int i = 0; i < m_IndexList.Count; i++)
                {
                    int offset = m_IndexList[i];
                    int v = offset > 0 ? offset : 0;
                    BitConverter.GetBytes(v ^ FileHeader.Chain[0]).CopyTo(inData, i * sizeof(int));
                }

                var outData = new byte[inSize];
                EncryptIndexList_Pak2(inData, inSize, outData);

                fileStream.Seek(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf, SeekOrigin.Begin);
                fileStream.Write(outData, 0, inSize);
                break;
            }

            case TPakFileType.pftPak3:
            {
                int inSize = m_IndexList.Count * sizeof(int);
                if (index < 0)
                {
                    var inData = new byte[inSize];
                    for (int i = 0; i < m_IndexList.Count; i++)
                    {
                        int offset = m_IndexList[i];
                        uint enc = offset > 0
                            ? (uint)offset ^ FPak3Password[i % 64] ^ ~(uint)i
                            : 0u ^ FPak3Password[i % 64] ^ ~(uint)i;
                        BitConverter.GetBytes(enc).CopyTo(inData, i * sizeof(int));
                    }

                    fileStream.Seek(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf, SeekOrigin.Begin);
                    fileStream.Write(inData, 0, inSize);
                }
                else if (index <= m_IndexList.Count - 1)
                {
                    int offset = m_IndexList[index];
                    uint enc = offset > 0
                        ? (uint)offset ^ FPak3Password[index % 64] ^ ~(uint)index
                        : 0u ^ FPak3Password[index % 64] ^ ~(uint)index;

                    fileStream.Seek(TFileHeaderInfo.SizeOf + TPakFileHeader.SizeOf + index * sizeof(int), SeekOrigin.Begin);
                    var one = BitConverter.GetBytes(enc);
                    fileStream.Write(one, 0, sizeof(int));
                }
                break;
            }

            case TPakFileType.pftLzPakV0:
            {
                int inSize = m_IndexList.Count * TLzPakIndexVer0.SizeOf;
                var inData = new byte[inSize];
                for (int i = 0; i < m_IndexList.Count; i++)
                {
                    var item = new TLzPakIndexVer0
                    {
                        nImageOffSet = m_IndexList[i],
                        nDataSize = m_ImageSizeList != null && i < m_ImageSizeList.Count ? m_ImageSizeList[i] : 0,
                    };
                    item.ToBytes(inData, i * TLzPakIndexVer0.SizeOf);
                }

                var outData = new byte[inSize];
                EncryptIndexList(inData, inSize, outData);

                fileStream.Seek(PakConsts.LZ_PAK_FILE_HEADER_SIZE, SeekOrigin.Begin);
                fileStream.Write(outData, 0, inSize);
                break;
            }

            case TPakFileType.pftLzPakV1:
            {
                int inSize = m_IndexList.Count * sizeof(int);
                var inData = new byte[inSize];
                for (int i = 0; i < m_IndexList.Count; i++)
                {
                    int offset = m_IndexList[i];
                    BitConverter.GetBytes(offset > 0 ? offset : 0).CopyTo(inData, i * sizeof(int));
                }

                var outData = new byte[inSize];
                EncryptIndexList(inData, inSize, outData);

                fileStream.Seek(PakConsts.LZ_PAK_FILE_HEADER_SIZE, SeekOrigin.Begin);
                fileStream.Write(outData, 0, inSize);
                break;
            }

            default:
                // 原文 case 无 else 分支（原文 672-797 的 case 覆盖 pftPak1/2/3/LzPakV0/LzPakV1，
                // 遗漏 pftLzPakV0orV1）→ 该枚举值下**什么都不做**。
                break;
        }
    }

    // =================================================================================
    // Initialize（原文 800-921）
    // =================================================================================

    /// <summary>
    /// Pak.pas 800-921 <c>Initialize</c> 1:1。
    /// <list type="number">
    /// <item>808：<c>FillChar(FileHeader, SizeOf(TPakFileHeader), #0)</c>（**无条件**，即使已初始化）。</item>
    /// <item>809-810：<c>if not Initialized then if FileExists(FileName) then</c>。</item>
    /// <item>811-816：置 m_boUpdateIndex/Indexing；以 <c>fmOpenReadWrite or fmShareDenyNone</c> 打开；
    ///       <c>FillChar(FPak3Password, 256, 0)</c>。</item>
    /// <item>820-823：读 10 字节 TFileHeaderInfo（<c>SetLength(S, 10)</c> + <c>Read(S[1], 10)</c> + <c>Move</c>）。</item>
    /// <item>825-840：CompareLStr 前 5 字节 'GEEM2' → Pak1（seek 10）；前 6 字节 'GEEPAK2' → Pak2（seek 10）；
    ///       前 6 字节 'GEEPAK3' → Pak3（seek 10）；否则取前 5 字节当 TLzFileHeaderInfo，
    ///       前 4 字节 'HXM2' → pftLzPakV0orV1（seek 5）。
    ///       <b>注意三者顺序</b>：'GEEM2' 只比 5 字节，故必须排在 'GEEPAK2'/'GEEPAK3' 之前（原文如此）。</item>
    /// <item>842-843：读 256 字节 TPakFileHeader。</item>
    /// <item>848-864：按 FPakFileType 解密文件头；LZ 路径再读 1 字节填充（864 与 999 两处）。</item>
    /// <item>866-872：LZ → 用 FKeyDataLz/FChainLz 覆盖 FileHeader.KeyData/Chain；否则用 FKeyData/FChain。</item>
    /// <item>874-880：<c>S := DecryptS(FileHeader.CheckCode)</c>；
    ///       普通 PAK 判 <c>S = 'GEEM2'</c>，LZ 判 <see cref="IsValidLzCheckCode"/>。</item>
    /// <item>882-900：口令正确 → ImageCount/BitCount 取自文件头 → AllocMem 图片数组并逐项清零 →
    ///       <c>LoadIndex</c> → <c>Initialized := True</c>。否则 OutMessage(密码错)。</item>
    /// <item>904-913：文件不存在 → 允许自动更新时向 UpdateEngine 请求索引。</item>
    /// <item>915-920：异常一律 <c>raise Exception.Create(E.Message)</c>。</item>
    /// </list>
    /// </summary>
    public override void Initialize()
    {
        try
        {
            FileHeader = TPakFileHeader.CreateEmpty();
            if (!Initialized)
            {
                if (File.Exists(FileName))
                {
                    m_boUpdateIndex = true;
                    m_boUpdateIndexing = false;
                    m_FileStream ??= new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    Array.Clear(FPak3Password, 0, FPak3Password.Length);

                    byte[] s;
                    LockFileStream();
                    try
                    {
                        m_FileStream.Seek(0, SeekOrigin.Begin);
                        var headerInfoBuf = new byte[TFileHeaderInfo.SizeOf];
                        ReadFully(m_FileStream, headerInfoBuf, 0, TFileHeaderInfo.SizeOf);
                        var headerInfo = TFileHeaderInfo.FromBytes(headerInfoBuf, 0);

                        if (PakStringCompare.CompareLStr(headerInfo.FileType, "GEEM2", "GEEM2".Length))
                        {
                            FPakFileType = TPakFileType.pftPak1;
                            m_FileStream.Seek(TFileHeaderInfo.SizeOf, SeekOrigin.Begin);
                        }
                        else if (PakStringCompare.CompareLStr(headerInfo.FileType, "GEEPAK2", "GEEPAK2".Length))
                        {
                            FPakFileType = TPakFileType.pftPak2;
                            m_FileStream.Seek(TFileHeaderInfo.SizeOf, SeekOrigin.Begin);
                        }
                        else if (PakStringCompare.CompareLStr(headerInfo.FileType, "GEEPAK3", "GEEPAK3".Length))
                        {
                            FPakFileType = TPakFileType.pftPak3;
                            m_FileStream.Seek(TFileHeaderInfo.SizeOf, SeekOrigin.Begin);
                        }
                        else
                        {
                            var lzHeaderInfo = TLzFileHeaderInfo.FromBytes(headerInfoBuf, 0);
                            // 原文 836：CompareLStr(string(LzHeaderInfo.FileType), 'HXM2', Length('HXM2'))
                            if (PakStringCompare.CompareLStr(lzHeaderInfo.FileTypeText, "HXM2", "HXM2".Length))
                            {
                                FPakFileType = TPakFileType.pftLzPakV0orV1; // 此时还不知道 LZPAK 的类型
                                m_FileStream.Seek(TLzFileHeaderInfo.SizeOf, SeekOrigin.Begin);
                            }
                        }

                        var headerBuf = new byte[TPakFileHeader.SizeOf];
                        ReadFully(m_FileStream, headerBuf, 0, TPakFileHeader.SizeOf);
                        s = headerBuf;
                    }
                    finally
                    {
                        UnLockFileStream();
                    }

                    if (FPakFileType == TPakFileType.pftPak1)
                        FileHeader = DecryptHeader_GameOfMir(s);
                    else if (FPakFileType == TPakFileType.pftPak2)
                        FileHeader = DecryptHeader_Pak2(s);
                    else if (FPakFileType == TPakFileType.pftPak3)
                    {
                        InitPak3Password();
                        FileHeader = DecryptHeader_Pak3(s);
                    }
                    else if (FPakFileType == TPakFileType.pftLzPakV0orV1)
                    {
                        FileHeader = DecryptHeader_LzPak(s);
                        if (FileHeader.bfType == 1)
                        {
                            FPakFileType = TPakFileType.pftLzPakV1;
                        }
                        else if (FileHeader.bfType == 0)
                        {
                            FPakFileType = TPakFileType.pftLzPakV0;
                            m_ImageSizeList = new List<int>();
                        }
                        m_FileStream.Read(new byte[1], 0, 1); // 原文 863：读掉 1 字节填充
                    }

                    if (FPakFileType == TPakFileType.pftLzPakV0 || FPakFileType == TPakFileType.pftLzPakV1)
                    {
                        Array.Copy(FKeyDataLz, FileHeader.KeyData, FKeyDataLz.Length);
                        Array.Copy(FChainLz, FileHeader.Chain, FChainLz.Length);
                    }
                    else
                    {
                        Array.Copy(FKeyData, FileHeader.KeyData, FKeyData.Length);
                        Array.Copy(FChain, FileHeader.Chain, FChain.Length);
                    }

                    string check = DecryptCheckCodeBytes();

                    if (FPakFileType == TPakFileType.pftPak1 || FPakFileType == TPakFileType.pftPak2
                        || FPakFileType == TPakFileType.pftPak3)
                    {
                        FPasswordOK = check == "GEEM2";
                    }
                    else
                    {
                        FPasswordOK = IsValidLzCheckCode(check);
                    }

                    if (FPasswordOK)
                    {
                        ImageCount = (int)FileHeader.ImageCount;
                        BitCount = (byte)FileHeader.BitCount;

                        m_ImgArr = new TDxImage[ImageCount];
                        for (int i = 0; i < m_ImgArr.Length; i++) m_ImgArr[i] = new TDxImage();

                        for (int i = 0; i <= ImageCount - 1; i++)
                        {
                            m_ImgArr[i].nWidth = 0;
                            m_ImgArr[i].nHeight = 0;
                            m_ImgArr[i].nPx = 0;
                            m_ImgArr[i].nPy = 0;
                            m_ImgArr[i].boUpdateStop = false;
                            m_ImgArr[i].boUpdateStart = false;
                            m_ImgArr[i].dwUpdateStartTick = MyGetTickCount();
                        }

                        LoadIndex();
                        Initialized = true;
                    }
                    else
                    {
                        OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakPasswordErr2), FileName));
                    }
                }
                else
                {
                    // {$IF CLIENTEXE = 1} m_boUpdateIndex and (…) and g_boAutoUpdate →
                    //   接缝：g_UpdateEngine（UpdateEngine.pas）/ g_boAutoUpdate（MShare.pas）待移植
                    if (m_boUpdateIndex
                        && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                        && g_boAutoUpdate)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, -1, this, null))
                        {
                            m_boUpdateIndexing = true;
                            m_dwUpdateIndexingTick = MyGetTickCount();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            // 原文 916-918：on E:Exception do begin raise Exception.Create(E.Message); end;
            throw new Exception(e.Message, e);
        }
    }

    // =================================================================================
    // Initialize_UpdateNewFile（原文 923-1050）
    // =================================================================================

    /// <summary>
    /// Pak.pas 923-1050 <c>Initialize_UpdateNewFile</c> 1:1。
    /// <para>与 <see cref="Initialize"/> 的**差异**（必须保留，属于「看起来一样实则不同」）：
    /// <list type="bullet">
    /// <item>930：<c>if Initialized then Exit;</c> 先短路。</item>
    /// <item>932-941：文件**不存在**时请求索引更新（Initialize 里是在 else 分支做同一件事）。</item>
    /// <item>943-948：同样 FillChar 文件头 + 打开流，但**不**清 FPak3Password；
    ///       GEEM2/GEEPAK2 分支里才 <c>FillChar(FPak3Password, 256, 0)</c>，
    ///       而 GEEPAK3 分支**不清**（initialize 是进函数就先清一次）。</item>
    /// <item>986-987：pftPak3 分支**不调用** <c>InitPak3Password</c>（Initialize 里调了）——
    ///       意味着本函数由 <see cref="UpdateIndex"/> 刚写完文件后调用时，
    ///       FPak3Password 依赖 UpdateIndex 里那次 InitPak3Password 的残留值。</item>
    /// <item>994-997：V0 时若 m_ImageSizeList 为 nil 才 Create 并设 Count（Initialize 是无条件 Create）。</item>
    /// <item>1019-1049：口正确后 AllocMem 并清零，但**不调用 LoadIndex**、**不置 Initialized**。</item>
    /// </list></para>
    /// </summary>
    public void Initialize_UpdateNewFile()
    {
        if (Initialized) return;

        if (!File.Exists(FileName))
        {
            if (m_boUpdateIndex
                && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                && g_boAutoUpdate)
            {
                if (PakSeams.UpdateEngineAddFn != null
                    && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, -1, this, null))
                {
                    m_boUpdateIndexing = true;
                    m_dwUpdateIndexingTick = MyGetTickCount();
                }
            }
        }

        FileHeader = TPakFileHeader.CreateEmpty();
        m_boUpdateIndex = true;
        m_boUpdateIndexing = false;

        m_FileStream ??= new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        byte[] s;
        LockFileStream();
        try
        {
            m_FileStream.Seek(0, SeekOrigin.Begin);
            var headerInfoBuf = new byte[TFileHeaderInfo.SizeOf];
            ReadFully(m_FileStream, headerInfoBuf, 0, TFileHeaderInfo.SizeOf);
            var headerInfo = TFileHeaderInfo.FromBytes(headerInfoBuf, 0);

            if (PakStringCompare.CompareLStr(headerInfo.FileType, "GEEM2", "GEEM2".Length))
            {
                Array.Clear(FPak3Password, 0, FPak3Password.Length);
                FPakFileType = TPakFileType.pftPak1;
                m_FileStream.Seek(TFileHeaderInfo.SizeOf, SeekOrigin.Begin);
            }
            else if (PakStringCompare.CompareLStr(headerInfo.FileType, "GEEPAK2", "GEEPAK2".Length))
            {
                Array.Clear(FPak3Password, 0, FPak3Password.Length);
                FPakFileType = TPakFileType.pftPak2;
                m_FileStream.Seek(TFileHeaderInfo.SizeOf, SeekOrigin.Begin);
            }
            else if (PakStringCompare.CompareLStr(headerInfo.FileType, "GEEPAK3", "GEEPAK3".Length))
            {
                FPakFileType = TPakFileType.pftPak3;   // 原文**不**清 FPak3Password
                m_FileStream.Seek(TFileHeaderInfo.SizeOf, SeekOrigin.Begin);
            }
            else
            {
                var lzHeaderInfo = TLzFileHeaderInfo.FromBytes(headerInfoBuf, 0);
                if (PakStringCompare.CompareLStr(lzHeaderInfo.FileTypeText, "HXM2", "HXM2".Length))
                {
                    FPakFileType = TPakFileType.pftLzPakV0orV1;
                    m_FileStream.Seek(TLzFileHeaderInfo.SizeOf, SeekOrigin.Begin);
                }
            }

            var headerBuf = new byte[TPakFileHeader.SizeOf];
            ReadFully(m_FileStream, headerBuf, 0, TPakFileHeader.SizeOf);
            s = headerBuf;
        }
        finally
        {
            UnLockFileStream();
        }

        if (FPakFileType == TPakFileType.pftPak1)
            FileHeader = DecryptHeader_GameOfMir(s);
        else if (FPakFileType == TPakFileType.pftPak2)
            FileHeader = DecryptHeader_Pak2(s);
        else if (FPakFileType == TPakFileType.pftPak3)
            FileHeader = DecryptHeader_Pak3(s);   // 原文**不**调 InitPak3Password
        else if (FPakFileType == TPakFileType.pftLzPakV0orV1)
        {
            FileHeader = DecryptHeader_LzPak(s);
            if (FileHeader.bfType == 1)
            {
                FPakFileType = TPakFileType.pftLzPakV1;
            }
            else if (FileHeader.bfType == 0)
            {
                FPakFileType = TPakFileType.pftLzPakV0;
                if (m_ImageSizeList == null)
                {
                    m_ImageSizeList = new List<int>();
                    while (m_ImageSizeList.Count < (int)FileHeader.ImageCount) m_ImageSizeList.Add(0);
                }
            }
            m_FileStream.Read(new byte[1], 0, 1);
        }

        if (FPakFileType == TPakFileType.pftLzPakV0 || FPakFileType == TPakFileType.pftLzPakV1)
        {
            Array.Copy(FKeyDataLz, FileHeader.KeyData, FKeyDataLz.Length);
            Array.Copy(FChainLz, FileHeader.Chain, FChainLz.Length);
        }
        else
        {
            Array.Copy(FKeyData, FileHeader.KeyData, FKeyData.Length);
            Array.Copy(FChain, FileHeader.Chain, FChain.Length);
        }

        string check = DecryptCheckCodeBytes();
        if (FPakFileType == TPakFileType.pftPak1 || FPakFileType == TPakFileType.pftPak2
            || FPakFileType == TPakFileType.pftPak3)
        {
            FPasswordOK = check == "GEEM2";
        }
        else
        {
            FPasswordOK = IsValidLzCheckCode(check);
        }

        if (FPasswordOK)
        {
            ImageCount = (int)FileHeader.ImageCount;
            BitCount = (byte)FileHeader.BitCount;

            m_ImgArr = new TDxImage[ImageCount];
            for (int i = 0; i < m_ImgArr.Length; i++) m_ImgArr[i] = new TDxImage();

            for (int i = 0; i <= ImageCount - 1; i++)
            {
                m_ImgArr[i].nWidth = 0;
                m_ImgArr[i].nHeight = 0;
                m_ImgArr[i].nPx = 0;
                m_ImgArr[i].nPy = 0;
                m_ImgArr[i].boUpdateStop = false;
                m_ImgArr[i].boUpdateStart = false;
                m_ImgArr[i].dwUpdateStartTick = MyGetTickCount();
            }
        }
        else
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakPasswordErr2), FileName));
        }
    }

    // =================================================================================
    // Finalize（原文 1052-1102）
    // =================================================================================

    /// <summary>
    /// Pak.pas 1052-1102 <c>Finalize</c> 1:1。
    /// <list type="number">
    /// <item>1056：<c>Initialized := False;</c>（**在 Lock 之前**）。</item>
    /// <item>1059-1061：三个 IndexList 清空。</item>
    /// <item>1062-1092：逐个释放 Surface/Gray/Bright（各自 try/except + DebugTextOut）与 Bitmap（**无 try**），
    ///       最后 FreeMem(m_ImgArr)。</item>
    /// <item>1094-1098：m_ImgArr := nil；ImageCount := 0；m_IndexList.Clear；关闭流。</item>
    /// <item><b>原文遗漏</b>：不清 m_ImageSizeList（只由 Destroy 释放）。照抄。</item>
    /// </list>
    /// </summary>
    public override void Finalize_()
    {
        Initialized = false;
        Lock();
        try
        {
            IndexList.Clear();
            GrayIndexList.Clear();
            BrightIndexList.Clear();

            if (m_ImgArr != null)
            {
                for (int i = 0; i <= ImageCount - 1 && i < m_ImgArr.Length; i++)
                {
                    FreeSlotSafe(ref m_ImgArr[i].Surface);
                    FreeSlotSafe(ref m_ImgArr[i].Gray);
                    FreeSlotSafe(ref m_ImgArr[i].Bright);
                    // 原文 1087-1089 的 Bitmap 分支**没有** try/except（照抄：失败即抛出）
                    var bmp = m_ImgArr[i].Bitmap;
                    if (bmp.Assigned && !bmp.IsNullTexture && bmp.Handle != IntPtr.Zero)
                        TextureSeams.TextureFreeFn(bmp.Handle);
                    m_ImgArr[i].Bitmap = default;
                }
            }

            m_ImgArr = null;
            ImageCount = 0;
            m_IndexList.Clear();
            if (m_FileStream != null)
            {
                m_FileStream.Dispose();
                m_FileStream = null;
            }
        }
        finally
        {
            UnLock();
        }
    }

    private void FreeSlotSafe(ref TTextureRef slot)
    {
        if (slot.Assigned && !slot.IsNullTexture && slot.Handle != IntPtr.Zero)
        {
            try { TextureSeams.TextureFreeFn(slot.Handle); }
            catch { DebugTextOut("[Exception] Texture.Free"); }
        }
        slot = default;
    }

    // =================================================================================
    // 索引加解密（原文 1104-1154）
    // =================================================================================

    /// <summary>Pak.pas 1104-1112 <c>EncryptIndexList</c>：KeyData/Chain 取自 <c>FileHeader</c>，EncryptCBC。</summary>
    public void EncryptIndexList(byte[] inData, int inSize, byte[] outData)
    {
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.EncryptCBC(inData, outData, inSize, chain, keyData);
    }

    /// <summary>Pak.pas 1114-1122 <c>DecryptIndexList</c>。</summary>
    public void DecryptIndexList(byte[] inData, int inSize, byte[] outData)
    {
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(inData, outData, inSize, chain, keyData);
    }

    /// <summary>
    /// Pak.pas 1124-1138 <c>EncryptIndexList_Pak2</c>：**两次** EncryptCBC（第二次就地）。
    /// <para>注意第二次的 keyData/chain 是从 FileHeader **重新**拷贝的（原文 1134-1135），
    /// 不是复用第一次跑完的 chain（否则就等价于只跑一次）。</para>
    /// </summary>
    public void EncryptIndexList_Pak2(byte[] inData, int inSize, byte[] outData)
    {
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.EncryptCBC(inData, outData, inSize, chain, keyData);

        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.EncryptCBC(outData, outData, inSize, chain, keyData);
    }

    /// <summary>Pak.pas 1140-1154 <c>DecryptIndexList_Pak2</c>：**两次** DecryptCBC（第二次就地）。</summary>
    public void DecryptIndexList_Pak2(byte[] inData, int inSize, byte[] outData)
    {
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(inData, outData, inSize, chain, keyData);

        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(outData, outData, inSize, chain, keyData);
    }

    // =================================================================================
    // 文件头加解密（原文 1156-1227）
    // =================================================================================

    /// <summary>
    /// Pak.pas 1156-1163 <c>EncryptHeader</c>：密钥 <c>IntToStr(442517066)</c>（**硬编码常量，不是 PakEncryKey**）。
    /// <para>程序中也未使用（只被已自承有 BUG 的 WriteHeader 调用）。</para>
    /// </summary>
    public string EncryptHeader(TPakFileHeader header)
    {
        var s = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(s, 0);
        return PakSeams.ToLatin1(UnitDes.EncryptStrDesBytes(s, DelphiRTL.IntToStr(442517066)));
    }

    /// <summary>Pak.pas 1165-1172 <c>DecryptHeader</c>（密钥 442517066）。</summary>
    public TPakFileHeader DecryptHeader(string str)
    {
        var bytes = PakSeams.FromLatin1(str);
        var plain = new byte[TPakFileHeader.SizeOf];
        UnitDes.DecryptDes(bytes, plain, TPakFileHeader.SizeOf, DelphiRTL.IntToStr(442517066));
        return TPakFileHeader.FromBytes(plain, 0);
    }

    /// <summary>Pak.pas 1174-1181 <c>EncryptHeader_GameOfMir</c>：密钥 <c>IntToStr(PakEncryKey^)</c> = "3698261022"。</summary>
    public string EncryptHeader_GameOfMir(TPakFileHeader header)
    {
        var s = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(s, 0);
        return PakSeams.ToLatin1(UnitDes.EncryptStrDesBytes(s, PakUnitDesGlobals.PakEncryKeyStr));
    }

    /// <summary>Pak.pas 1183-1190 <c>DecryptHeader_GameOfMir</c>。</summary>
    public TPakFileHeader DecryptHeader_GameOfMir(string str)
        => DecryptHeader_GameOfMir(PakSeams.FromLatin1(str));

    /// <summary>字节视图重载（对应原文 <c>Str:string</c> 的 AnsiString 字节语义）。</summary>
    public TPakFileHeader DecryptHeader_GameOfMir(byte[] cipher)
    {
        var plain = new byte[TPakFileHeader.SizeOf];
        UnitDes.DecryptDes(cipher, plain, TPakFileHeader.SizeOf, PakUnitDesGlobals.PakEncryKeyStr);
        return TPakFileHeader.FromBytes(plain, 0);
    }

    /// <summary>Pak.pas 1192-1196 <c>EncryptHeader_Pak2</c>：走 DesUtils.EncryptDes_New（BS=20）。</summary>
    public string EncryptHeader_Pak2(TPakFileHeader header)
    {
        var result = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(result, 0);
        PakDesNew.EncryptDes_New(result, result, result.Length, PakUnitDesGlobals.PakEncryKeyStr);
        return PakSeams.ToLatin1(result);
    }

    /// <summary>Pak.pas 1198-1201 <c>DecryptHeader_Pak2</c>：<c>DecryptDes_New(Str[1], Result, SizeOf, key)</c>。</summary>
    public TPakFileHeader DecryptHeader_Pak2(string str)
        => DecryptHeader_Pak2(PakSeams.FromLatin1(str));

    /// <summary>字节视图重载。</summary>
    public TPakFileHeader DecryptHeader_Pak2(byte[] cipher)
    {
        var plain = new byte[TPakFileHeader.SizeOf];
        PakDesNew.DecryptDes_New(cipher, plain, TPakFileHeader.SizeOf, PakUnitDesGlobals.PakEncryKeyStr);
        return TPakFileHeader.FromBytes(plain, 0);
    }

    /// <summary>
    /// Pak.pas 1203-1207 <c>EncryptHeader_Pak3</c>：
    /// <c>AESEncrypt(@FPak3Password[0], 16, @Header, @Result[1], SizeOf(TPakFileHeader));</c>
    /// </summary>
    public string EncryptHeader_Pak3(TPakFileHeader header)
    {
        var result = new byte[TPakFileHeader.SizeOf];
        var plain = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(plain, 0);
        PakAesCtr.AesEncrypt(Pak3KeyBytes(), 16, plain, result, TPakFileHeader.SizeOf);
        return PakSeams.ToLatin1(result);
    }

    /// <summary>Pak.pas 1209-1212 <c>DecryptHeader_Pak3</c>。</summary>
    public TPakFileHeader DecryptHeader_Pak3(string str)
        => DecryptHeader_Pak3(PakSeams.FromLatin1(str));

    /// <summary>字节视图重载。</summary>
    public TPakFileHeader DecryptHeader_Pak3(byte[] cipher)
    {
        var plain = new byte[TPakFileHeader.SizeOf];
        PakAesCtr.AesDecrypt(Pak3KeyBytes(), 16, cipher, plain, TPakFileHeader.SizeOf);
        return TPakFileHeader.FromBytes(plain, 0);
    }

    /// <summary><c>@FPak3Password[0]</c> 的 16 字节视图（前 4 个 LongWord 的小端字节）。</summary>
    public byte[] Pak3KeyBytes()
    {
        var key = new byte[64 * 4];
        for (int i = 0; i < 64; i++) BitConverter.GetBytes(FPak3Password[i]).CopyTo(key, i * 4);
        return key;
    }

    /// <summary>Pak.pas 1214-1219 <c>DecryptHeader_LzPak</c>（局部常量 <c>sHeadPassword = '442517066'</c>）。</summary>
    public TPakFileHeader DecryptHeader_LzPak(string str)
        => DecryptHeader_LzPak(PakSeams.FromLatin1(str));

    /// <summary>字节视图重载。</summary>
    public TPakFileHeader DecryptHeader_LzPak(byte[] cipher)
    {
        const string sHeadPassword = "442517066";
        var plain = new byte[TPakFileHeader.SizeOf];
        UnitDes.DecryptDes(cipher, plain, TPakFileHeader.SizeOf, sHeadPassword);
        return TPakFileHeader.FromBytes(plain, 0);
    }

    /// <summary>Pak.pas 1221-1227 <c>EncryptHeader_LzPak</c>（同样的 '442517066'）。</summary>
    public string EncryptHeader_LzPak(TPakFileHeader header)
    {
        const string sHeadPassword = "442517066";
        var plain = new byte[TPakFileHeader.SizeOf];
        header.ToBytes(plain, 0);
        var cipher = new byte[TPakFileHeader.SizeOf];
        UnitDes.EncryptDes(plain, cipher, TPakFileHeader.SizeOf, sHeadPassword);
        return PakSeams.ToLatin1(cipher);
    }

    /// <summary>
    /// Pak.pas 1229-1242 <c>EncryptS</c>：空串直接返回空串；否则按**字符串长度**（GBK 字节数）做 EncryptCBC。
    /// <para>原文 <c>SetLength(Result, Length(S))</c> —— S 是 AnsiString，Length 是字节数；
    /// 注意 <c>S</c> 是**明文校验码**（'GEEM2'/'D3DM2' 等 ASCII），故此处用 GBK 取字节是对的。</para>
    /// </summary>
    public string EncryptS(string s)
    {
        if (s == "") return "";
        byte[] inData = GXX.Core.EncodingInit.GBK.GetBytes(s);
        var outData = new byte[inData.Length];
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.EncryptCBC(inData, outData, inData.Length, chain, keyData);
        return PakSeams.ToLatin1(outData);
    }

    /// <summary>
    /// <b>关键换算</b>：原文里 <c>EncryptHeader*</c>/<c>EncryptS</c> 返回的是 <c>AnsiString</c>，
    /// 其「字符」就是密文的**裸字节**；把这种串写回 <c>ShortString</c> 字段（<c>CheckCode:string[12]</c>）
    /// 或长度字节时，Delphi 做的是**逐字节**拷贝。
    /// <para>托管侧以 Latin-1（ISO-8859-1，字节透明）承载该串，所以反向取字节也必须用 Latin-1。
    /// **绝不能用 GBK**：GBK 会把 0x80–0xFF 的裸字节重新编码（部分变成 2 字节或落成 '?'），
    /// 于是密文被破坏（本车道实测：{0x85,0xB4,0x05,0xF9,0x19} 经 GBK 变成 7 字节 {0x3F,0xA1,0xE4,0x05,0xA8,0xB4,0x19}，
    /// 导致 CheckCode 解密失败、<c>Initialize</c> 报密码错）。</para>
    /// </summary>
    private static byte[] CipherBytes(string cipher) => PakSeams.FromLatin1(cipher);

    /// <summary>
    /// 原文 <c>FileHeader.CheckCode := EncryptS('GEEM2')</c> 的**字节级**等价实现。
    /// <para>Delphi 里 <c>EncryptS</c> 返回 AnsiString，赋给 <c>CheckCode:string[12]</c> 是逐字节拷贝
    /// （长度字节 = 密文长度）。托管侧若走 <see cref="TPakFileHeader.CheckCodeText"/>（GBK 编码），
    /// 会把 0x80–0xFF 的密文字节重新编码而损坏密文（见 <see cref="CipherBytes"/> 注释），
    /// 故此处必须直接写字节。</para>
    /// </summary>
    private void SetCheckCodeBytes(string cipher)
    {
        byte[] bytes = CipherBytes(cipher);
        int len = Math.Min(bytes.Length, 12);
        FileHeader.CheckCode[0] = (byte)len;
        for (int i = 0; i < len; i++) FileHeader.CheckCode[1 + i] = bytes[i];
        for (int i = len; i < 12; i++) FileHeader.CheckCode[1 + i] = 0;
    }

    /// <summary>
    /// 原文 <c>DecryptS(FileHeader.CheckCode)</c> 的**字节级**实现。
    /// <para>绝不能走 <see cref="TPakFileHeader.CheckCodeText"/>：那是 GBK 解码，
    /// 密文里 0x80–0xFF 的裸字节会被当作 GBK 前导字节重新组合（实测 5 字节 {0x85,0xB4,0x05,0xF9,0x19}
    /// 变成 7 字节 {0x3F,0x3F,0x05,0xF9,0x19,...}），CheckCode 校验必然失败。</para>
    /// </summary>
    private string DecryptCheckCodeBytes()
    {
        int len = Math.Min((int)FileHeader.CheckCode[0], 12);
        var inData = new byte[len];
        Array.Copy(FileHeader.CheckCode, 1, inData, 0, len);

        var outData = new byte[len];
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(inData, outData, len, chain, keyData);
        return GXX.Core.EncodingInit.GBK.GetString(outData);
    }

    /// <summary>Pak.pas 1244-1257 <c>DecryptS</c>。
    /// <para><c>s</c> 来自 <c>FileHeader.CheckCode</c>（ShortString 的**裸密文字节**），
    /// 故必须按 Latin-1 取字节，不能用 GBK（见 <see cref="CipherBytes"/>）。</para></summary>
    public string DecryptS(string s)
    {
        if (s == "") return "";
        byte[] inData = CipherBytes(s);
        var outData = new byte[inData.Length];
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(inData, outData, inData.Length, chain, keyData);
        return GXX.Core.EncodingInit.GBK.GetString(outData);
    }

    // =================================================================================
    // 图片头读取（原文 1259-1333）
    // =================================================================================

    /// <summary>
    /// Pak.pas 1259-1268 <c>ReadImageHeader(var Buffer; Count)</c>：
    /// <c>Read(Buffer, Count)</c>（**就地**，不检查返回值）后 <c>DecryptCBC(Buffer, Buffer, Count, Chain, KeyData)</c>。
    /// </summary>
    public void ReadImageHeader(byte[] buffer, int count)
    {
        ReadFully(m_FileStream!, buffer, 0, count);
        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(buffer, buffer, count, chain, keyData);
    }

    /// <summary>
    /// Pak.pas 1270-1292 <c>ReadImageHeader_Pak2</c>：
    /// 读后先对 KeyData 做**隔项 xor**（<c>if I mod 2 = 0 then KeyData[I] := KeyData[I] xor KeyData[I+1]</c>）
    /// 再 DecryptCBC，**然后**用**原始** KeyData 再 DecryptCBC 一次（共两次）。
    /// <para><b>原文缺陷</b>：<c>for I := 0 to Length(KeyData) - 1</c>（=31）时
    /// <c>KeyData[I + 1]</c> 越界读 <c>KeyData[32]</c>（栈上数组外的 4 字节）。
    /// 本移植按 I 到 30 处理（越界读的栈残留不可复现），并在报告中登记。</para>
    /// </summary>
    public void ReadImageHeader_Pak2(byte[] buffer, int count)
    {
        ReadFully(m_FileStream!, buffer, 0, count);

        var keyData = new uint[32];
        var chain = new byte[20];
        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        for (int i = 0; i <= keyData.Length - 2; i++)
        {
            if (i % 2 == 0) keyData[i] = keyData[i] ^ keyData[i + 1];
        }
        UnitDes.DecryptCBC(buffer, buffer, count, chain, keyData);

        Array.Copy(FileHeader.KeyData, keyData, 32);
        Array.Copy(FileHeader.Chain, chain, 20);
        UnitDes.DecryptCBC(buffer, buffer, count, chain, keyData);
    }

    /// <summary>
    /// Pak.pas 1294-1333 <c>ReadImageHeader_Pak3(Index, var Buffer, Count)</c>：
    /// 读后构造 16 字节 AES 密钥 <c>S</c>（4 个 LongWord，来自 FPak3Password 的 4 组组合），
    /// 再 <c>AESDecrypt(@S[1], 16, @Buffer, @Buffer, Count)</c>。
    /// <list type="bullet">
    /// <item><c>S[0..3]  = FPak3Password[(I+0)%64] xor FPak3Password[(I+14)%64]</c></item>
    /// <item><c>S[4..7]  = FPak3Password[(I+12)%64] and FPak3Password[(I+19)%64]</c></item>
    /// <item><c>S[8..11] = FPak3Password[(I+28)%64] xor (not FPak3Password[(I+10)%64])</c></item>
    /// <item><c>S[12..15]= FPak3Password[(I+1)%64]</c></item>
    /// </list>
    /// </summary>
    public void ReadImageHeader_Pak3(int index, byte[] buffer, int count)
    {
        ReadFully(m_FileStream!, buffer, 0, count);

        var s = new byte[16];
        BitConverter.GetBytes(Pak3K1(index)).CopyTo(s, 0);
        BitConverter.GetBytes(Pak3K2(index)).CopyTo(s, 4);
        BitConverter.GetBytes(Pak3K3(index)).CopyTo(s, 8);
        BitConverter.GetBytes(Pak3K4(index)).CopyTo(s, 12);

        var outBuf = new byte[Math.Max(count, 0)];
        PakAesCtr.AesDecrypt(s, s.Length, buffer, outBuf, count);
        Array.Copy(outBuf, buffer, Math.Min(count, Math.Min(outBuf.Length, buffer.Length)));
    }

    // =================================================================================
    // LoadIndex（原文 1403-1497）
    // =================================================================================

    /// <summary>
    /// Pak.pas 1403-1497 <c>LoadIndex</c> 1:1。
    /// <list type="number">
    /// <item>1412-1413：<c>m_IndexList.Capacity/Count := FileHeader.ImageCount</c>。</item>
    /// <item>1415-1421：V0 → 单项 8 字节且同时扩张 m_ImageSizeList；否则单项 4 字节。</item>
    /// <item>1423-1425：<c>InSize := ImageCount * nSingleIndexSize</c>；**<c>if InSize = 0 then Exit</c>**
    ///       （所以 ImageCount = 0 时索引表保持全 0 且不读文件）。</item>
    /// <item>1429-1435：加锁后 seek 到 <c>FileHeader.IndexOffSet</c> 一次性读 InSize 字节。</item>
    /// <item>1437-1494：按格式分派：
    ///       Pak1 → DecryptIndexList；Pak2 → DecryptIndexList_Pak2 后每项 <c>xor Chain[0]</c>；
    ///       Pak3 → 每项 <c>Cardinal(Index) xor FPak3Password[i mod 64] xor (not Cardinal(i))</c>（**不解密**）；
    ///       LzPakV0/V1 → DecryptIndexList 后 V1 逐项取 Integer、V0 逐项取 (off,size) 对；
    ///       其它（**含 pftLzPakV0orV1**）→ 全部置 nil。</item>
    /// </list>
    /// <para><b>原文缺陷（照抄）</b>：1490-1494 的 else 分支对应 pftLzPakV0orV1 ——
    /// 若 <see cref="Initialize"/> 没能把类型定下来（bfType 既不是 0 也不是 1），
    /// 索引表会被整表清零而不是报错。</para>
    /// </summary>
    public void LoadIndex()
    {
        while (m_IndexList.Count < (int)FileHeader.ImageCount) m_IndexList.Add(0);
        while (m_IndexList.Count > (int)FileHeader.ImageCount) m_IndexList.RemoveAt(m_IndexList.Count - 1);

        int nSingleIndexSize;
        if (FPakFileType == TPakFileType.pftLzPakV0)
        {
            nSingleIndexSize = TLzPakIndexVer0.SizeOf;
            m_ImageSizeList ??= new List<int>();
            while (m_ImageSizeList.Count < (int)FileHeader.ImageCount) m_ImageSizeList.Add(0);
            while (m_ImageSizeList.Count > (int)FileHeader.ImageCount) m_ImageSizeList.RemoveAt(m_ImageSizeList.Count - 1);
        }
        else
        {
            nSingleIndexSize = sizeof(int);
        }

        int inSize = (int)(FileHeader.ImageCount * (uint)nSingleIndexSize);
        if (inSize == 0) return;

        var inData = new byte[inSize];
        LockFileStream();
        try
        {
            m_FileStream!.Seek(FileHeader.IndexOffSet, SeekOrigin.Begin);
            ReadFully(m_FileStream, inData, 0, inSize);
        }
        finally
        {
            UnLockFileStream();
        }

        if (FPakFileType == TPakFileType.pftPak1)
        {
            var outData = new byte[inSize];
            DecryptIndexList(inData, inSize, outData);
            for (int i = 0; i <= FileHeader.ImageCount - 1; i++)
                m_IndexList[i] = BitConverter.ToInt32(outData, i * sizeof(int));
        }
        else if (FPakFileType == TPakFileType.pftPak2)
        {
            var outData = new byte[inSize];
            DecryptIndexList_Pak2(inData, inSize, outData);
            for (int i = 0; i <= FileHeader.ImageCount - 1; i++)
            {
                int index = BitConverter.ToInt32(outData, i * sizeof(int));
                index ^= FileHeader.Chain[0];
                m_IndexList[i] = index;
            }
        }
        else if (FPakFileType == TPakFileType.pftPak3)
        {
            for (int i = 0; i <= FileHeader.ImageCount - 1; i++)
            {
                uint index = (uint)BitConverter.ToInt32(inData, i * sizeof(int));
                index = index ^ FPak3Password[i % 64] ^ ~(uint)i;
                m_IndexList[i] = unchecked((int)index);
            }
        }
        else if (FPakFileType == TPakFileType.pftLzPakV0 || FPakFileType == TPakFileType.pftLzPakV1)
        {
            var outData = new byte[inSize];
            DecryptIndexList(inData, inSize, outData);
            if (FPakFileType == TPakFileType.pftLzPakV1)
            {
                for (int i = 0; i <= FileHeader.ImageCount - 1; i++)
                    m_IndexList[i] = BitConverter.ToInt32(outData, i * sizeof(int));
            }
            else // pftLzPakV0
            {
                m_ImageSizeList ??= new List<int>();
                for (int i = 0; i <= FileHeader.ImageCount - 1; i++)
                {
                    var item = TLzPakIndexVer0.FromBytes(outData, i * TLzPakIndexVer0.SizeOf);
                    m_IndexList[i] = item.nImageOffSet;
                    while (m_ImageSizeList.Count <= i) m_ImageSizeList.Add(0);
                    m_ImageSizeList[i] = item.nDataSize;
                }
            }
        }
        else
        {
            // 原文 1490-1494：m_IndexList.Items[I] := nil（即全 0）
            for (int i = 0; i <= FileHeader.ImageCount - 1; i++) m_IndexList[i] = 0;
        }
    }

    // =================================================================================
    // WriteHeader（原文 1500-1524）—— 原文自承有 BUG，程序中也未使用
    // =================================================================================

    /// <summary>
    /// Pak.pas 1500-1524 <c>WriteHeader</c> 1:1。
    /// <para><b>原文自承缺陷（1499 行注释）</b>：没考虑 <c>TFileHeaderInfo</c>（10 字节前缀）的存在，
    /// 于是把 256 字节文件头写到**偏移 0**，覆盖掉本该在前的 'GEEM2'/'GEEPAK2'/'GEEPAK3' 魔数；
    /// 同时把 <c>IndexOffSet := SizeOf(TPakFileHeader)</c>（=256，而不是 266）。
    /// 程序中也未使用。**照抄不修**。</para>
    /// </summary>
    public void WriteHeader()
    {
        if (Initialized)
        {
            ImageCount = m_IndexList.Count;
            FileHeader.bfType = 2;
            FileHeader.ImageCount = (uint)m_IndexList.Count;
            FileHeader.IndexOffSet = TPakFileHeader.SizeOf;

            var header = FileHeader;
            Array.Clear(header.KeyData, 0, header.KeyData.Length);
            Array.Clear(header.Chain, 0, header.Chain.Length);

            string s = EncryptHeader(header);
            byte[] bytes = CipherBytes(s);

            LockFileStream();
            try
            {
                m_FileStream!.Seek(0, SeekOrigin.Begin);
                m_FileStream.Write(bytes, 0, TPakFileHeader.SizeOf);
            }
            finally
            {
                UnLockFileStream();
            }
        }
    }

    // =================================================================================
    // 缓存取图（原文 1526-2181）
    // =================================================================================

    /// <summary>
    /// Pak.pas 1526-1554 <c>GetBitmap</c> 1:1。
    /// <para><b>与 GetCachedImage 的差异（必须保留）</b>：
    /// <list type="bullet">
    /// <item>它检查的是 <c>m_ImgArr[Index].Surface</c>（**不是** Bitmap）。</item>
    /// <item>它**不**调用 <c>FreeOldMemorys_Ex</c>、也**不**检查 <c>FPasswordOK</c>。</item>
    /// <item>它走 <c>LoadDxBitmap</c> —— 而 Pak.pas 2183-2186 的 LoadDxBitmap **方法体为空**，
    ///       所以 Bitmap 永远保持 nil，<c>Result</c> 恒为 nil，IndexList 永远不会被它 Add。</item>
    /// <item>它**没有** UpdateEngine 更新请求分支。</item>
    /// </list></para>
    /// </summary>
    public TTextureRef? GetBitmap(int index, out int px, out int py)
    {
        TTextureRef? result = null;
        px = 0;
        py = 0;
        m_dwUseCheckTick = MyGetTickCount();
        if (index >= 0)
        {
            if (Initialized && index < ImageCount && index < m_IndexList.Count)
            {
                Lock();
                try
                {
                    if (!m_ImgArr![index].Surface.Assigned)
                    {
                        LoadDxBitmap(m_IndexList[index], m_ImgArr[index], index);
                        px = m_ImgArr[index].nPx;
                        py = m_ImgArr[index].nPy;

                        if (m_ImgArr[index].Bitmap.Assigned)
                            IndexList.Add(index);
                        result = m_ImgArr[index].Bitmap;
                    }
                    else
                    {
                        m_ImgArr[index].dwLatestTime = MyGetTickCount();
                        px = m_ImgArr[index].nPx;
                        py = m_ImgArr[index].nPy;
                        result = m_ImgArr[index].Bitmap;
                    }
                }
                finally
                {
                    UnLock();
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Pak.pas 1556-1631 <c>GetCachedImage</c> 1:1。
    /// <para>要点：<c>Index &gt;= 0</c> 才进（原文 1561），内层再判
    /// <c>Initialized and Index &lt; ImageCount and FPasswordOK</c>；
    /// 首次装载整个包在 try/except 里（异常 → OutMessage 后继续走「赋 px/py + 计 tick」）；
    /// 装载后无论成败都执行 1583-1604（含 UpdateEngine 请求与 <c>Result = nil → g_NullImage</c>）。
    /// <b>注意</b>：原文 1564 的内层条件**没有** <c>Index &lt; m_IndexList.Count</c>（只在 1569 里补了一层）。</para>
    /// </summary>
    public TTextureRef GetCachedImage(int index, out int px, out int py)
    {
        TTextureRef result = default;
        px = 0;
        py = 0;
        if (index < 0) return result;

        Lock();
        try
        {
            if (Initialized && index < ImageCount && FPasswordOK)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    try
                    {
                        if (index >= 0 && index < m_IndexList.Count)
                        {
                            if (FPakFileType == TPakFileType.pftLzPakV0)
                            {
                                LoadDxImageLzPak(index, m_IndexList[index], ImageSizeAt(index), TDxTextureStyle.dtsNormal, m_ImgArr[index]);
                            }
                            else if (FPakFileType == TPakFileType.pftLzPakV1)
                            {
                                LoadDxImageLzPak(index, m_IndexList[index], 0, TDxTextureStyle.dtsNormal, m_ImgArr[index]);
                            }
                            else
                            {
                                LoadDxImage(m_IndexList[index], m_ImgArr[index], index);
                            }
                        }
                    }
                    catch
                    {
                        OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakGetCacheImageErr), FileName, index));
                    }

                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;

                    m_ImgArr[index].dwLatestTime = MyGetTickCount();

                    if (m_ImgArr[index].Surface.Assigned)
                        IndexList.Add(index);

                    result = m_ImgArr[index].Surface;

                    // {$IF CLIENTEXE = 1} 微端更新请求
                    if (!result.Assigned && g_boAutoUpdate && m_boNeedUpdate && !m_ImgArr[index].boUpdateStop
                        && ((!m_ImgArr[index].boUpdateStart)
                            || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtImagePak, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }

                    if (!result.Assigned)
                        result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    result = m_ImgArr[index].Surface;
                }
            }
            else
            {
                // 原文 1613-1625：索引更新请求（ImageIndex := 0 当 Index >= ImageCount，否则 -1）
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    int imageIndex = index >= ImageCount ? 0 : -1;
                    if (PakSeams.UpdateEngineAddFn != null
                        && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, imageIndex, this, null))
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
        return result;
    }

    /// <summary>LzPakV0 的 <c>m_ImageSizeList[Index]</c>（缺失时按原文 nil 解引用会崩；此处返回 0 并登记）。</summary>
    private int ImageSizeAt(int index)
        => m_ImageSizeList != null && index >= 0 && index < m_ImageSizeList.Count ? m_ImageSizeList[index] : 0;

    /// <summary>
    /// Pak.pas 1633-1740 <c>GetCachedLzImageSize</c> 1:1。
    /// <list type="number">
    /// <item>1645：前置条件含 <c>m_FileStream &lt;&gt; nil</c> 与 <c>FPasswordOK</c>。</item>
    /// <item>1646：已缓存（宽高乘积 ≠ 0）→ 直接回填并 True。</item>
    /// <item>1649-1657：V0 的可读判定 = <c>nDataSize &gt; 0 and nPosition &gt;= 262 and nPosition + 12 &lt;= Size</c>；
    ///       V1 = <c>nPosition &gt;= 262 and nPosition + 16 &lt;= Size</c>；其它 = False。</item>
    /// <item>1660-1677：读图片头（V0 → TPakImageInfo 12 字节；V1 → TNewPakImageInfo 16 字节），
    ///       都走 <c>ReadImageHeader</c>（DES-CBC，不是 Pak2/Pak3 变体）。</item>
    /// <item>1679-1701：V0 校验失败 → 请求索引更新后 <c>Exit</c>（Result 保持 False）；成功则回填 4 个字段 + ASize/APoint。</item>
    /// <item>1702-1726：V1 同理。</item>
    /// </list>
    /// <para><b>注意</b>：LZ 路径**只**用 <c>ReadImageHeader</c>（Pak1 变体），与 <c>LoadDxImageLzPak</c> 一致。</para>
    /// </summary>
    public bool GetCachedLzImageSize(int index, ref Size aSize, ref Point aPoint)
    {
        bool result = false;
        int nLzV0ImageDataSize = 0;
        Lock();
        try
        {
            if (Initialized && index >= 0 && index < ImageCount && index < m_IndexList.Count
                && m_FileStream != null && FPasswordOK)
            {
                if (m_ImgArr![index].nWidth * m_ImgArr[index].nHeight == 0)
                {
                    int nPosition = m_IndexList[index];
                    bool bCanRead;

                    if (FPakFileType == TPakFileType.pftLzPakV0)
                    {
                        nLzV0ImageDataSize = ImageSizeAt(index);
                        bCanRead = nLzV0ImageDataSize > 0
                            && nPosition >= PakConsts.LZ_PAK_FILE_HEADER_SIZE
                            && nPosition + TPakImageInfo.SizeOf <= m_FileStream.Length;
                    }
                    else if (FPakFileType == TPakFileType.pftLzPakV1)
                    {
                        bCanRead = nPosition >= PakConsts.LZ_PAK_FILE_HEADER_SIZE
                            && nPosition + TNewPakImageInfo.SizeOf <= m_FileStream.Length;
                    }
                    else
                    {
                        bCanRead = false;
                    }

                    if (bCanRead)
                    {
                        var imageInfoV0 = default(TPakImageInfo);
                        var newImageInfo = default(TNewPakImageInfo);

                        if (FPakFileType == TPakFileType.pftLzPakV0)
                        {
                            LockFileStream();
                            try
                            {
                                m_FileStream.Position = nPosition;
                                var buf = new byte[TPakImageInfo.SizeOf];
                                ReadImageHeader(buf, TPakImageInfo.SizeOf);
                                imageInfoV0 = TPakImageInfo.FromBytes(buf, 0);
                            }
                            finally
                            {
                                UnLockFileStream();
                            }
                        }
                        else if (FPakFileType == TPakFileType.pftLzPakV1)
                        {
                            LockFileStream();
                            try
                            {
                                m_FileStream.Position = nPosition;
                                var buf = new byte[TNewPakImageInfo.SizeOf];
                                ReadImageHeader(buf, TNewPakImageInfo.SizeOf);
                                newImageInfo = TNewPakImageInfo.FromBytes(buf, 0);
                            }
                            finally
                            {
                                UnLockFileStream();
                            }
                        }

                        if (FPakFileType == TPakFileType.pftLzPakV0)
                        {
                            if (!IsValidLzImageInfoV0(imageInfoV0, index, nLzV0ImageDataSize, FileHeader.BitCount))
                            {
                                RequestIndexUpdate();
                                return false;
                            }

                            m_ImgArr[index].nWidth = (ushort)imageInfoV0.wW;
                            m_ImgArr[index].nHeight = (ushort)imageInfoV0.wH;
                            m_ImgArr[index].nPx = imageInfoV0.wPx;
                            m_ImgArr[index].nPy = imageInfoV0.wPy;

                            aSize.Width = imageInfoV0.wW;
                            aSize.Height = imageInfoV0.wH;
                            aPoint.X = imageInfoV0.wPx;
                            aPoint.Y = imageInfoV0.wPy;
                        }
                        else
                        {
                            if (!IsValidLzImageInfoV1(newImageInfo, index))
                            {
                                RequestIndexUpdate();
                                return false;
                            }

                            m_ImgArr[index].nWidth = (ushort)newImageInfo.nWidth;
                            m_ImgArr[index].nHeight = (ushort)newImageInfo.nHeight;
                            m_ImgArr[index].nPx = newImageInfo.px;
                            m_ImgArr[index].nPy = newImageInfo.py;

                            aSize.Width = newImageInfo.nWidth;
                            aSize.Height = newImageInfo.nHeight;
                            aPoint.X = newImageInfo.px;
                            aPoint.Y = newImageInfo.py;
                        }
                        result = true;
                    }
                }
                else
                {
                    aSize.Width = m_ImgArr[index].nWidth;
                    aSize.Height = m_ImgArr[index].nHeight;
                    aPoint.X = m_ImgArr[index].nPx;
                    aPoint.Y = m_ImgArr[index].nPy;
                    result = true;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return result;
    }

    /// <summary>原文 1681-1688 / 1705-1712 / 1792-1799 的「请求索引全量更新」公共片段。</summary>
    private void RequestIndexUpdate()
    {
        if (m_boUpdateIndex
            && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
            && g_boAutoUpdate)
        {
            if (PakSeams.UpdateEngineAddFn != null
                && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, -1, this, null))
            {
                m_boUpdateIndexing = true;
                m_dwUpdateIndexingTick = MyGetTickCount();
            }
        }
    }

    /// <summary>
    /// Pak.pas 1742-1827 <c>GetCachedImageSize</c> 1:1。
    /// <list type="number">
    /// <item>1747-1750：LZ 类型（V0/V1）**直接转调** <see cref="GetCachedLzImageSize"/> 后 Exit ——
    ///       注意这里 <c>in [pftLzPakV0, pftLzPakV1]</c> **不含** pftLzPakV0orV1。</item>
    /// <item>1755：前置条件与 GetBitmap 同级（含 m_FileStream/FPasswordOK）。</item>
    /// <item>1759：可读判定 <c>nPosition &gt;= SizeOf(TPakFileHeader)</c>（**256**，不是 266）。</item>
    /// <item>1760-1784：按 Pak1/Pak2/Pak3 各自读 16 字节图片头。</item>
    /// <item>1787-1803：五项校验（PixelFormat 合法 / |px|,|py| 上限 / Length &lt; MAX_IMAGE_SIZE /
    ///       宽高 &gt; 0 / 宽高 &lt; 上限），任一项失败 → 请求索引更新 + Exit(False)。</item>
    /// <item>1805-1814：回填 4 字段 + ASize/APoint + True。</item>
    /// <item>1816-1822：已缓存（宽高乘积 ≠ 0）→ 直接回填 + True。</item>
    /// </list>
    /// <para><b>接口面偏差（登记）</b>：GameImagesBase.cs 的 TGameImages 未声明
    /// <c>GetCachedImageSize</c>（GameImages.pas 126 是 <c>virtual; abstract;</c>），故此处**不是** <c>override</c>。
    /// 接缝：待 GameImages.pas 全量移植后改回 override。</para>
    /// </summary>
    public bool GetCachedImageSize(int index, ref Size aSize, ref Point aPoint)
    {
        if (FPakFileType == TPakFileType.pftLzPakV0 || FPakFileType == TPakFileType.pftLzPakV1)
        {
            return GetCachedLzImageSize(index, ref aSize, ref aPoint);
        }

        bool result = false;
        Lock();
        try
        {
            if (Initialized && index >= 0 && index < ImageCount && index < m_IndexList.Count
                && m_FileStream != null && FPasswordOK)
            {
                if (m_ImgArr![index].nWidth * m_ImgArr[index].nHeight == 0)
                {
                    int nPosition = m_IndexList[index];

                    if (nPosition >= TPakFileHeader.SizeOf
                        && nPosition + TNewPakImageInfo.SizeOf <= m_FileStream.Length)
                    {
                        var buf = new byte[TNewPakImageInfo.SizeOf];

                        if (FPakFileType == TPakFileType.pftPak1)
                        {
                            LockFileStream();
                            try
                            {
                                m_FileStream.Position = nPosition;
                                ReadImageHeader(buf, TNewPakImageInfo.SizeOf);
                            }
                            finally
                            {
                                UnLockFileStream();
                            }
                        }
                        else if (FPakFileType == TPakFileType.pftPak2)
                        {
                            LockFileStream();
                            try
                            {
                                m_FileStream.Position = nPosition;
                                ReadImageHeader_Pak2(buf, TNewPakImageInfo.SizeOf);
                            }
                            finally
                            {
                                UnLockFileStream();
                            }
                        }
                        else if (FPakFileType == TPakFileType.pftPak3)
                        {
                            LockFileStream();
                            try
                            {
                                m_FileStream.Position = nPosition;
                                ReadImageHeader_Pak3(index, buf, TNewPakImageInfo.SizeOf);
                            }
                            finally
                            {
                                UnLockFileStream();
                            }
                        }

                        var newImageInfo = TNewPakImageInfo.FromBytes(buf, 0);

                        // 文件发生错误后，全部重更新 2020-08-03 21:10:01
                        if (!IsKnownPixelFormat(newImageInfo.PixelFormat)
                            || (PakSeams.AbsSmallInt(newImageInfo.px) > GameImagesConsts.MAX_IMAGE_WIDTH)
                            || (PakSeams.AbsSmallInt(newImageInfo.py) > GameImagesConsts.MAX_IMAGE_HEIGHT)
                            || (newImageInfo.Length >= GameImagesConsts.MAX_IMAGE_SIZE)
                            || (newImageInfo.nWidth <= 0 || newImageInfo.nHeight <= 0)
                            || (newImageInfo.nWidth >= GameImagesConsts.MAX_IMAGE_WIDTH
                                || newImageInfo.nHeight >= GameImagesConsts.MAX_IMAGE_HEIGHT))
                        {
                            RequestIndexUpdate();
                            return false;
                        }

                        m_ImgArr[index].nWidth = (ushort)newImageInfo.nWidth;
                        m_ImgArr[index].nHeight = (ushort)newImageInfo.nHeight;
                        m_ImgArr[index].nPx = newImageInfo.px;
                        m_ImgArr[index].nPy = newImageInfo.py;

                        aSize.Width = newImageInfo.nWidth;
                        aSize.Height = newImageInfo.nHeight;
                        aPoint.X = newImageInfo.px;
                        aPoint.Y = newImageInfo.py;
                        result = true;
                    }
                }
                else
                {
                    aSize.Width = m_ImgArr[index].nWidth;
                    aSize.Height = m_ImgArr[index].nHeight;
                    aPoint.X = m_ImgArr[index].nPx;
                    aPoint.Y = m_ImgArr[index].nPy;
                    result = true;
                }
            }
        }
        finally
        {
            UnLock();
        }
        return result;
    }

    /// <summary>原文判定集合 <c>[pf8bit, pf15bit, pf16bit, pf24bit, pf32bit]</c>。</summary>
    private static bool IsKnownPixelFormat(byte pf)
        => pf == (byte)TPixelFormat.pf8bit || pf == (byte)TPixelFormat.pf15bit
        || pf == (byte)TPixelFormat.pf16bit || pf == (byte)TPixelFormat.pf24bit
        || pf == (byte)TPixelFormat.pf32bit;

    /// <summary>
    /// Pak.pas 1829-1899 <c>GetCachedGrayImage</c> 1:1。
    /// <para><b>与 GetCachedImage 的差异（必须保留）</b>：
    /// <list type="bullet">
    /// <item>1834：<c>if (Index &lt; 0) then Exit;</c>（**在 Lock 之前**，而 GetCachedImage 是进 Lock 后判）。</item>
    /// <item>1842：装载分支**没有** try/except（异常会直接冒泡，GetCachedImage 会 OutMessage 后继续）。</item>
    /// <item>1852-1859：赋的是 <c>dwLatestGrayTime</c> + <c>GrayIndexList</c> + <c>Gray</c>。</item>
    /// </list></para>
    /// </summary>
    public TTextureRef GetCachedGrayImage(int index, out int px, out int py)
    {
        TTextureRef result = default;
        px = 0;
        py = 0;
        if (index < 0) return result;

        Lock();
        try
        {
            if (Initialized && index < ImageCount && FPasswordOK)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    if (index >= 0 && index < m_IndexList.Count)
                    {
                        if (FPakFileType == TPakFileType.pftLzPakV0)
                        {
                            LoadDxImageLzPak(index, m_IndexList[index], ImageSizeAt(index), TDxTextureStyle.dtsGray, m_ImgArr[index]);
                        }
                        else if (FPakFileType == TPakFileType.pftLzPakV1)
                        {
                            LoadDxImageLzPak(index, m_IndexList[index], 0, TDxTextureStyle.dtsGray, m_ImgArr[index]);
                        }
                        else
                        {
                            LoadDxGrayImage(m_IndexList[index], m_ImgArr[index], index);
                        }
                    }

                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();

                    if (m_ImgArr[index].Gray.Assigned)
                        GrayIndexList.Add(index);

                    result = m_ImgArr[index].Gray;

                    if (!result.Assigned && g_boAutoUpdate && m_boNeedUpdate && !m_ImgArr[index].boUpdateStop
                        && ((!m_ImgArr[index].boUpdateStart)
                            || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtImagePak, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }

                    if (!result.Assigned)
                        result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    result = m_ImgArr[index].Gray;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    int imageIndex = index >= ImageCount ? 0 : -1;
                    if (PakSeams.UpdateEngineAddFn != null
                        && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, imageIndex, this, null))
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
        return result;
    }

    /// <summary>Pak.pas 1901-1970 <c>GetCachedBrightImage</c> 1:1（与 Gray 版同构，字段换 Bright/dwLatestBrightTime/BrightIndexList）。</summary>
    public TTextureRef GetCachedBrightImage(int index, out int px, out int py)
    {
        TTextureRef result = default;
        px = 0;
        py = 0;
        if (index < 0) return result;

        Lock();
        try
        {
            if (Initialized && index < ImageCount && FPasswordOK)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    if (index >= 0 && index < m_IndexList.Count)
                    {
                        if (FPakFileType == TPakFileType.pftLzPakV0)
                        {
                            LoadDxImageLzPak(index, m_IndexList[index], ImageSizeAt(index), TDxTextureStyle.dtsBright, m_ImgArr[index]);
                        }
                        else if (FPakFileType == TPakFileType.pftLzPakV1)
                        {
                            LoadDxImageLzPak(index, m_IndexList[index], 0, TDxTextureStyle.dtsBright, m_ImgArr[index]);
                        }
                        else
                        {
                            LoadDxBrightImage(m_IndexList[index], m_ImgArr[index], index);
                        }
                    }

                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    if (m_ImgArr[index].Bright.Assigned)
                        BrightIndexList.Add(index);

                    result = m_ImgArr[index].Bright;

                    if (!result.Assigned && g_boAutoUpdate && m_boNeedUpdate && !m_ImgArr[index].boUpdateStop
                        && ((!m_ImgArr[index].boUpdateStart)
                            || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtImagePak, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }

                    if (!result.Assigned)
                        result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    px = m_ImgArr[index].nPx;
                    py = m_ImgArr[index].nPy;
                    result = m_ImgArr[index].Bright;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    int imageIndex = index >= ImageCount ? 0 : -1;
                    if (PakSeams.UpdateEngineAddFn != null
                        && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, imageIndex, this, null))
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
        return result;
    }

    /// <summary>
    /// Pak.pas 1972-2041 <c>GetCachedSurface</c> 1:1。
    /// <para><b>与 GetCachedImage / GetCachedGrayImage 的差异（必须保留）</b>：
    /// <list type="bullet">
    /// <item>前置条件**多一项** <c>Index &lt; m_IndexList.Count</c>（1982）。</item>
    /// <item>装载分支**没有** try/except、**没有** 1583-1584 的 px/py 回填（只回填 dwLatestTime）。</item>
    /// <item>不检查 <c>Result</c> 是否为 nil 就 <c>IndexList.Add</c>，最后统一走 <c>g_NullImage</c>。</item>
    /// </list></para>
    /// <para><b>接口面偏差（登记）</b>：GameImagesBase.cs 的 TGameImages 未声明 <c>GetCachedSurface</c>
    /// （GameImages.pas 94 是 <c>virtual</c>），故此处**不是** <c>override</c>。
    /// 接缝：待 GameImages.pas 全量移植后改回 override。</para>
    /// </summary>
    public TTextureRef GetCachedSurface(int index)
    {
        TTextureRef result = default;
        if (index < 0) return result;

        Lock(); // ★★★★★★★★这个要写到最外层，微端更新的时候才不占用cpu★★★★ 2020-04-17 22:52:50（原文注释）
        try
        {
            if (Initialized && index < ImageCount && index < m_IndexList.Count && FPasswordOK)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Surface.Assigned)
                {
                    int nPosition = m_IndexList[index];

                    if (FPakFileType == TPakFileType.pftLzPakV0)
                    {
                        LoadDxImageLzPak(index, nPosition, ImageSizeAt(index), TDxTextureStyle.dtsNormal, m_ImgArr[index]);
                    }
                    else if (FPakFileType == TPakFileType.pftLzPakV1)
                    {
                        LoadDxImageLzPak(index, nPosition, 0, TDxTextureStyle.dtsNormal, m_ImgArr[index]);
                    }
                    else
                    {
                        LoadDxImage(m_IndexList[index], m_ImgArr[index], index);
                    }

                    m_ImgArr[index].dwLatestTime = MyGetTickCount();

                    if (m_ImgArr[index].Surface.Assigned)
                        IndexList.Add(index);

                    result = m_ImgArr[index].Surface;

                    if (!result.Assigned && g_boAutoUpdate && m_boNeedUpdate && !m_ImgArr[index].boUpdateStop
                        && ((!m_ImgArr[index].boUpdateStart)
                            || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtImagePak, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }

                    if (!result.Assigned)
                        result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestTime = MyGetTickCount();
                    result = m_ImgArr[index].Surface;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    int imageIndex = index >= ImageCount ? 0 : -1;
                    if (PakSeams.UpdateEngineAddFn != null
                        && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, imageIndex, this, null))
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
        return result;
    }

    /// <summary>Pak.pas 2043-2111 <c>GetCachedGray</c> 1:1（无 px/py 出参版；其余同 GetCachedSurface 结构）。
    /// <para><b>接口面偏差（登记）</b>：基类接缝未声明该方法，故此处**不是** <c>override</c>。</para></summary>
    public TTextureRef GetCachedGray(int index)
    {
        TTextureRef result = default;
        if (index < 0) return result;

        Lock();
        try
        {
            if (Initialized && index < ImageCount && index < m_IndexList.Count && FPasswordOK)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Gray.Assigned)
                {
                    int nPosition = m_IndexList[index];

                    if (FPakFileType == TPakFileType.pftLzPakV0)
                        LoadDxImageLzPak(index, nPosition, ImageSizeAt(index), TDxTextureStyle.dtsGray, m_ImgArr[index]);
                    else if (FPakFileType == TPakFileType.pftLzPakV1)
                        LoadDxImageLzPak(index, nPosition, 0, TDxTextureStyle.dtsGray, m_ImgArr[index]);
                    else
                        LoadDxGrayImage(m_IndexList[index], m_ImgArr[index], index);

                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();

                    if (m_ImgArr[index].Gray.Assigned)
                        GrayIndexList.Add(index);

                    result = m_ImgArr[index].Gray;

                    if (!result.Assigned && g_boAutoUpdate && m_boNeedUpdate && !m_ImgArr[index].boUpdateStop
                        && ((!m_ImgArr[index].boUpdateStart)
                            || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtImagePak, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }

                    if (!result.Assigned)
                        result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestGrayTime = MyGetTickCount();
                    result = m_ImgArr[index].Gray;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    int imageIndex = index >= ImageCount ? 0 : -1;
                    if (PakSeams.UpdateEngineAddFn != null
                        && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, imageIndex, this, null))
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
        return result;
    }

    /// <summary>Pak.pas 2113-2181 <c>GetCachedBright</c> 1:1。
    /// <para><b>接口面偏差（登记）</b>：基类接缝未声明该方法，故此处**不是** <c>override</c>。</para></summary>
    public TTextureRef GetCachedBright(int index)
    {
        TTextureRef result = default;
        if (index < 0) return result;

        Lock();
        try
        {
            if (Initialized && index < ImageCount && index < m_IndexList.Count && FPasswordOK)
            {
                FreeOldMemorys_Ex();

                if (!m_ImgArr![index].Bright.Assigned)
                {
                    int nPosition = m_IndexList[index];

                    if (FPakFileType == TPakFileType.pftLzPakV0)
                        LoadDxImageLzPak(index, nPosition, ImageSizeAt(index), TDxTextureStyle.dtsBright, m_ImgArr[index]);
                    else if (FPakFileType == TPakFileType.pftLzPakV1)
                        LoadDxImageLzPak(index, nPosition, 0, TDxTextureStyle.dtsBright, m_ImgArr[index]);
                    else
                        LoadDxBrightImage(m_IndexList[index], m_ImgArr[index], index);

                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();

                    if (m_ImgArr[index].Bright.Assigned)
                        BrightIndexList.Add(index);

                    result = m_ImgArr[index].Bright;

                    if (!result.Assigned && g_boAutoUpdate && m_boNeedUpdate && !m_ImgArr[index].boUpdateStop
                        && ((!m_ImgArr[index].boUpdateStart)
                            || (MyGetTickCount() - m_ImgArr[index].dwUpdateStartTick >= g_UpdateRetryTime))
                        && g_boDeviceInitializeOK)
                    {
                        if (PakSeams.UpdateEngineAddFn != null
                            && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtImagePak, index, this, null))
                        {
                            m_ImgArr[index].boUpdateStart = true;
                            m_ImgArr[index].dwUpdateStartTick = MyGetTickCount();
                        }
                    }

                    if (!result.Assigned)
                        result = g_NullImage;
                }
                else
                {
                    m_ImgArr[index].dwLatestBrightTime = MyGetTickCount();
                    result = m_ImgArr[index].Bright;
                }
            }
            else
            {
                if (m_boUpdateIndex
                    && ((!m_boUpdateIndexing) || (MyGetTickCount() - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                    && g_boDeviceInitializeOK && g_boAutoUpdate && m_boNeedUpdate)
                {
                    int imageIndex = index >= ImageCount ? 0 : -1;
                    if (PakSeams.UpdateEngineAddFn != null
                        && PakSeams.UpdateEngineAddFn(FileName, PakSeams.UpdateType.udtIndexPak, imageIndex, this, null))
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
        return result;
    }

    // =================================================================================
    // 加载（原文 2183-2186 + 2200-2407 校验/数据解码）
    // =================================================================================

    /// <summary>
    /// Pak.pas 2183-2186 <c>LoadDxBitmap</c> —— 原文**方法体为空**（照抄）。
    /// <para>因此 <see cref="GetBitmap"/> 的 Bitmap 槽位恒为 nil、<c>Result</c> 恒为 nil。</para>
    /// </summary>
    public void LoadDxBitmap(int position, TDxImage dxImage, int index)
    {
        // 原文如此（Pak.pas:2183-2186）：方法体为空。
    }

    /// <summary>
    /// Pak.pas 2200-2230 <c>IsValidLzImageInfoV0</c> 1:1（5 道守卫，**前 4 道 Exit，最后一道不 Exit**）。
    /// <para><b>原文易错点</b>：2226-2229 的「宽高 ≥ 上限」分支只把 Result 置 False，**没有 Exit** ——
    /// 但因为它是最后一个判断，效果与 Exit 等价。本移植保留同一结构。</para>
    /// </summary>
    public bool IsValidLzImageInfoV0(TPakImageInfo imageInfoV0, int nIndex, int nDataSize, int nBitCount)
    {
        bool result = true;
        if (!(nBitCount == 8 || nBitCount == 16 || nBitCount == 24 || nBitCount == 32))
        {
            return false;
        }

        if (PakSeams.AbsSmallInt(imageInfoV0.wPx) > GameImagesConsts.MAX_IMAGE_WIDTH
            || PakSeams.AbsSmallInt(imageInfoV0.wPy) > GameImagesConsts.MAX_IMAGE_HEIGHT)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageErr),
                FileName, nIndex, imageInfoV0.wPx, imageInfoV0.wPy));
            return false;
        }

        if (nDataSize >= GameImagesConsts.MAX_IMAGE_SIZE)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageLenErr),
                FileName, nIndex, nDataSize));
            return false;
        }

        if (imageInfoV0.wW <= 0 || imageInfoV0.wH <= 0)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageSizeErr),
                FileName, nIndex, imageInfoV0.wW, imageInfoV0.wH));
            return false;
        }

        if (imageInfoV0.wW >= GameImagesConsts.MAX_IMAGE_WIDTH
            || imageInfoV0.wH >= GameImagesConsts.MAX_IMAGE_HEIGHT)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageSizeErr),
                FileName, nIndex, imageInfoV0.wW, imageInfoV0.wH));
            result = false;   // 原文**不 Exit**
        }

        return result;
    }

    /// <summary>Pak.pas 2232-2262 <c>IsValidLzImageInfoV1</c> 1:1（同样最后一道不 Exit）。</summary>
    public bool IsValidLzImageInfoV1(TNewPakImageInfo imageInfoV1, int nIndex)
    {
        bool result = true;
        if (!IsKnownPixelFormat(imageInfoV1.PixelFormat)) return false;

        if (PakSeams.AbsSmallInt(imageInfoV1.px) > GameImagesConsts.MAX_IMAGE_WIDTH
            || PakSeams.AbsSmallInt(imageInfoV1.py) > GameImagesConsts.MAX_IMAGE_HEIGHT)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageErr),
                FileName, nIndex, imageInfoV1.px, imageInfoV1.py));
            return false;
        }

        if (imageInfoV1.Length >= GameImagesConsts.MAX_IMAGE_SIZE)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageLenErr),
                FileName, nIndex, imageInfoV1.Length));
            return false;
        }

        if (imageInfoV1.nWidth <= 0 || imageInfoV1.nHeight <= 0)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageSizeErr),
                FileName, nIndex, imageInfoV1.nWidth, imageInfoV1.nHeight));
            return false;
        }

        if (imageInfoV1.nWidth >= GameImagesConsts.MAX_IMAGE_WIDTH
            || imageInfoV1.nHeight >= GameImagesConsts.MAX_IMAGE_HEIGHT)
        {
            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PakResStrings.SPakLoadDxImageSizeErr),
                FileName, nIndex, imageInfoV1.nWidth, imageInfoV1.nHeight));
            result = false;   // 原文**不 Exit**
        }

        return result;
    }

    /// <summary>
    /// Pak.pas 2264-2316 <c>LoadLzImageDataV0</c> 1:1。
    /// <list type="bullet">
    /// <item><c>btEncr0 = 1</c>：RLE 解压（<c>DecodeRLE(pCompressedData, pUncompressedData, wW, wH, BitCount div 8)</c>），
    ///       缓冲区是 <c>GetMemory(nImgSize * 2)</c>（**两倍**大小，防止 RLE 膨胀）。</item>
    /// <item><c>btEncr0 = 2</c>：zlib 解压，且**要求 <c>nUncompressedSize = nImgSize</c>**（尺寸不符则 Source 保持 nil）。</item>
    /// <item>其它：直接当未压缩数据用。</item>
    /// </list>
    /// <para><b>原文缺陷（照抄）</b>：2294 的 <c>pUncompressedData</c> 在 zlib 分支前**未置 nil**
    /// （只有 2289/2306 的 <c>if pUncompressedData &lt;&gt; nil then FreeMemory</c> 用到它），
    /// 若 DecompressBuf 抛异常，2295 的 <c>nUncompressedSize</c> 也是未初始化值；
    /// 本移植统一走 <c>byte[]? = null</c>，语义等价于「异常 → 不产出 Source」。</para>
    /// </summary>
    public bool LoadLzImageDataV0(TPakImageInfo imageInfoV0, PakMemoryStream msData, out TDib? source, out TDib? alphaSource)
    {
        source = null;
        alphaSource = null;

        int nWidthBytes = PakConsts.WidthBytes(FileHeader.BitCount, imageInfoV0.wW);
        int nImgSize = nWidthBytes * imageInfoV0.wH;

        switch (imageInfoV0.btEncr0)
        {
            case 1:
            {
                var uncompressed = new byte[nImgSize * 2];
                int bytesPerPixel = (FileHeader.BitCount / 8) & 0xFF;
                try
                {
                    var decoded = PakRle.DecodeRle(msData.Memory, 0, msData.Size,
                        imageInfoV0.wW, imageInfoV0.wH, bytesPerPixel);
                    Array.Copy(decoded, 0, uncompressed, 0, Math.Min(decoded.Length, uncompressed.Length));

                    source = WzlMakeDibByBitCount(FileHeader.BitCount, imageInfoV0.wW, imageInfoV0.wH);
                    if (source != null)
                    {
                        // Move(pUncompressedData^, Source.PBits^, Source.Height * Source.WidthBytes)
                        CopyInto(source, uncompressed, source.Height * source.WidthBytes);
                        return true;
                    }
                }
                catch
                {
                    // 原文 2286-2288：except 空（吞掉）
                }
                break;
            }

            case 2:
            {
                try
                {
                    byte[]? uncompressed = ZlibEx.DecompressBuf(msData.Memory, msData.Size, nImgSize);
                    int nUncompressedSize = uncompressed?.Length ?? 0;
                    if (nUncompressedSize == nImgSize)
                    {
                        source = WzlMakeDibByBitCount(FileHeader.BitCount, imageInfoV0.wW, imageInfoV0.wH);
                        if (source != null)
                        {
                            CopyInto(source, uncompressed!, source.Height * source.WidthBytes);
                            return true;
                        }
                    }
                }
                catch
                {
                    // 原文 2302-2304：except 空
                }
                break;
            }

            default:
            {
                // 原文 2310：pUncompressedData := msData.Memory
                source = WzlMakeDibByBitCount(FileHeader.BitCount, imageInfoV0.wW, imageInfoV0.wH);
                if (source != null)
                {
                    CopyInto(source, msData.Memory, source.Height * source.WidthBytes);
                    return true;
                }
                break;
            }
        }

        return false;
    }

    /// <summary>
    /// Pak.pas 2318-2330 <c>GetBitCountByPixelFormat</c> 1:1。
    /// <para>pf15bit 与 pf16bit **都**返回 16；其它非已知格式返回 0。</para>
    /// </summary>
    public static int GetBitCountByPixelFormat(byte pf)
    {
        if (pf == (byte)TPixelFormat.pf1bit) return 1;
        if (pf == (byte)TPixelFormat.pf4bit) return 4;
        if (pf == (byte)TPixelFormat.pf8bit) return 8;
        if (pf == (byte)TPixelFormat.pf15bit) return 16;
        if (pf == (byte)TPixelFormat.pf16bit) return 16;
        if (pf == (byte)TPixelFormat.pf24bit) return 24;
        if (pf == (byte)TPixelFormat.pf32bit) return 32;
        return 0;
    }

    /// <summary>
    /// Pak.pas 2332-2343 <c>IsNewSDFormat</c> 1:1：
    /// 仅当 <c>PixelFormat = pf16bit</c> 且 <c>not boAlpha</c> 且
    /// <c>nWidth*nHeight*2 &lt;&gt; nDecompressedSize</c> 时为真。
    /// </summary>
    public static bool IsNewSDFormat(TNewPakImageInfo imageHead, int nDecompressedSize)
    {
        bool result = false;
        if (imageHead.PixelFormat == (byte)TPixelFormat.pf16bit && !imageHead.boAlpha)
        {
            int nRealSize = imageHead.nWidth * imageHead.nHeight * 2; // WideBytes(pf16bit) = 2
            if (nRealSize != nDecompressedSize) result = true;
        }
        return result;
    }

    /// <summary>
    /// Pak.pas 2345-2370 <c>GetNewPakImageDataSize</c> 1:1。
    /// <list type="bullet">
    /// <item>宽/高/长度为负（原文 <c>&lt; 0</c>，**不含 0**）→ 返回 0。</item>
    /// <item><c>Length &gt; 0</c> → 直接返回 <c>Length</c>（**不**再加 alpha 块）。</item>
    /// <item>否则按 PixelFormat 算 <c>WidthBytes(bits, w) * h</c>；<c>boAlpha</c> 时再加一整个 8bit 平面。</item>
    /// </list>
    /// </summary>
    public static int GetNewPakImageDataSize(TNewPakImageInfo imageHead)
    {
        if (imageHead.nWidth < 0 || imageHead.nHeight < 0 || imageHead.Length < 0) return 0;

        if (imageHead.Length > 0) return imageHead.Length;

        int result;
        if (imageHead.PixelFormat == (byte)TPixelFormat.pf8bit)
            result = PakConsts.WidthBytes(8, imageHead.nWidth) * imageHead.nHeight;
        else if (imageHead.PixelFormat == (byte)TPixelFormat.pf15bit || imageHead.PixelFormat == (byte)TPixelFormat.pf16bit)
            result = PakConsts.WidthBytes(16, imageHead.nWidth) * imageHead.nHeight;
        else if (imageHead.PixelFormat == (byte)TPixelFormat.pf24bit)
            result = PakConsts.WidthBytes(24, imageHead.nWidth) * imageHead.nHeight;
        else if (imageHead.PixelFormat == (byte)TPixelFormat.pf32bit)
            result = PakConsts.WidthBytes(32, imageHead.nWidth) * imageHead.nHeight;
        else
            return 0;

        if (result > 0 && imageHead.boAlpha)
            result += PakConsts.WidthBytes(8, imageHead.nWidth) * imageHead.nHeight;

        return result;
    }

    /// <summary>
    /// Pak.pas 2372-2407 <c>GetAlphaDibFromNewFormat</c> 1:1：
    /// 从 <c>pf16bit</c> 数据 **之后** 的「每像素 4bit」压缩 alpha 平面展开成 8bit alpha DIB。
    /// <list type="bullet">
    /// <item><c>nImgSize = nImgWidth * nImgHeight * 2</c>（16bit 主体）。</item>
    /// <item><c>nAlphaLineSize = nImgWidth div 2</c>，奇数宽再 +1；<c>nAlphaSize = nImgHeight * nAlphaLineSize</c>。</item>
    /// <item>越界守卫：<c>if nAlphaSize &gt; (nSrcSize - nImgSize) then Exit</c>（返回 nil）。</item>
    /// <item>展开：<c>y</c> 从 <c>nImgHeight-1 downto 0</c>（**倒序**），
    ///       <c>x</c> 偶数取高半字节 ×17、奇数取低半字节 ×17（<c>Min(255, …)</c>），
    ///       行指针按 <c>nAlphaWidthBytes = ((w*8+31) div 32)*4</c> 前进。</item>
    /// </list>
    /// <para><b>注意</b>：<c>x div 2</c> 的商在同一行内恒定步进，且 <c>y</c> 倒序写入 —— 所以
    /// alpha 行与源行是**倒序对应**的（源 y 与目标 y 同号，只是遍历方向反）。</para>
    /// </summary>
    public static TDib? GetAlphaDibFromNewFormat(int nImgWidth, int nImgHeight, byte[] pSrcData, int nSrcSize)
    {
        int nImgSize = nImgWidth * nImgHeight * 2; // WidthBytes(pf16bit)
        int nAlphaLineSize = PakConsts.AlphaLineSize(nImgWidth);
        int nAlphaSize = nImgHeight * nAlphaLineSize;

        if (nAlphaSize > (nSrcSize - nImgSize)) return null;

        int srcAlphaOffset = nImgSize;
        int nAlphaWidthBytes = PakConsts.AlphaWidthBytes(nImgWidth);

        var result = TDib.Create();
        result.SetSize(nImgWidth, nImgHeight, 8);

        var line = new byte[Math.Max(nAlphaWidthBytes, 0)];
        for (int y = nImgHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < nImgWidth; x++)
            {
                int byAlphaIdx = srcAlphaOffset + (y * nAlphaLineSize + (x / 2));
                byte byAlpha = byAlphaIdx >= 0 && byAlphaIdx < pSrcData.Length ? pSrcData[byAlphaIdx] : (byte)0;
                if ((x & 0x01) == 0)
                    line[x] = (byte)Math.Min(255, ((byAlpha & 0xF0) >> 4) * 17);
                else
                    line[x] = (byte)Math.Min(255, (byAlpha & 0x0F) * 17);
            }
            // PByte(INT_PTR(pAlpha) + x) 直接写：这里按 ScanLine 写入（等价于 x 从 0 起的行内偏移）
            result.WriteScanLine(y, line, 0, Math.Min(nImgWidth, line.Length));
        }

        return result;
    }

    /// <summary>
    /// Pak.pas 2409-2469 <c>LoadLzImageDataV1</c> 1:1。
    /// <list type="number">
    /// <item>2419-2420：<c>nBitCount := GetBitCountByPixelFormat(PixelFormat)</c>；<c>&lt; 8</c> → Exit(False)。</item>
    /// <item>2423-2429：<c>nImageDataSize := nHeight * WidthBytes(nBitCount, nWidth)</c>；
    ///       <c>nUncompressedSize := nImageDataSize + (boAlpha ? nHeight*nWidth : 0)</c>
    ///       —— 注意 alpha 部分用的是 <c>nHeight * nWidth</c>（**不是** WidthBytes(8,w)*h）。</item>
    /// <item>2431-2447：<c>Length &gt; 0</c> → zlib 解压（异常时 <c>nImageDataSize := 0</c>）；
    ///       否则源指针直接指向 <c>msData.Memory</c>、<c>pUncompressedData := nil</c>。</item>
    /// <item>2449-2455：造 DIB 并铺数据。</item>
    /// <item>2457-2467：<c>Result</c> 为真时：<c>boAlpha</c> → 从 <c>pSrcImageData + nImageDataSize</c> 拷整块 alpha 平面；
    ///       否则若 <c>bHasAlphaData</c> → 调 <see cref="GetAlphaDibFromNewFormat"/>。</item>
    /// </list>
    /// <para><b>原文缺陷（照抄）</b>：2435 的 <c>ZLibEx.DecompressBuf(pCompressedData, msData.Size, nUncompressedSize, pUncompressedData, nUncompressedSize)</c>
    /// 把 <c>nUncompressedSize</c> **既当入参容量又当出参长度**；本移植用返回数组长度作为出参长度，
    /// 且 2440 的 <c>IsNewSDFormat(ImageInfoV1, nImageDataSize)</c> 用的是「是否解压失败」的表达
    /// （原文把 <c>nImageDataSize</c> 在异常时置 0）—— 此处同样传入「失败则为 0」的值。</para>
    /// </summary>
    public bool LoadLzImageDataV1(TNewPakImageInfo imageInfoV1, PakMemoryStream msData, out TDib? source, out TDib? alphaSource)
    {
        source = null;
        alphaSource = null;

        int nBitCount = GetBitCountByPixelFormat(imageInfoV1.PixelFormat);
        if (nBitCount < 8) return false;

        bool bHasAlphaData = false;
        int nWidthBytes = PakConsts.WidthBytes(nBitCount, imageInfoV1.nWidth);
        int nImageDataSize = imageInfoV1.nHeight * nWidthBytes;
        int nUncompressedSize = imageInfoV1.boAlpha
            ? nImageDataSize + (imageInfoV1.nHeight * imageInfoV1.nWidth)
            : nImageDataSize;

        byte[]? pSrcImageData;
        if (imageInfoV1.Length > 0)
        {
            int nImageDataSizeForSd = nImageDataSize;
            byte[]? uncompressed = null;
            try
            {
                uncompressed = ZlibEx.DecompressBuf(msData.Memory, msData.Size, nUncompressedSize);
                if (uncompressed == null) nImageDataSizeForSd = 0;   // 原文明文：except nImageDataSize := 0
            }
            catch
            {
                nImageDataSizeForSd = 0;
            }

            pSrcImageData = uncompressed;
            bHasAlphaData = IsNewSDFormat(imageInfoV1, nImageDataSizeForSd);
        }
        else
        {
            pSrcImageData = msData.Memory;
        }

        if (pSrcImageData != null)
        {
            source = WzlMakeDibByPixelFormat(imageInfoV1.PixelFormat, imageInfoV1.nWidth, imageInfoV1.nHeight);
            if (source != null)
            {
                CopyInto(source, pSrcImageData, source.Height * source.WidthBytes);
            }
        }

        bool result = source != null;
        if (result)
        {
            if (imageInfoV1.boAlpha)
            {
                alphaSource = WzlMakeDibByBitCount(8, imageInfoV1.nWidth, imageInfoV1.nHeight);
                if (alphaSource != null)
                {
                    var alphaData = new byte[Math.Max(pSrcImageData!.Length - nImageDataSize, 0)];
                    Array.Copy(pSrcImageData, Math.Min(nImageDataSize, pSrcImageData.Length),
                        alphaData, 0, Math.Min(alphaData.Length, Math.Max(pSrcImageData.Length - nImageDataSize, 0)));
                    CopyInto(alphaSource, alphaData, alphaSource.Height * alphaSource.WidthBytes);
                }
            }
            else if (bHasAlphaData)
            {
                alphaSource = GetAlphaDibFromNewFormat(imageInfoV1.nWidth, imageInfoV1.nHeight,
                    pSrcImageData!, nUncompressedSize);
            }
        }

        return result;
    }

    /// <summary>
    /// GameImages.pas 1063-1087 <c>MakeDibByBitCount</c>（Pak.pas 1335-1365 的**被注释掉**版本，
    /// 但 2281/2296/2311/2459/2765/2936/2969/3134/3167 仍在调用）—— 
    /// 与 <see cref="WzlImagesDib.MakeDibByPixelFormat"/> 同一实现的按位宽入口。
    /// </summary>
    public static TDib? WzlMakeDibByBitCount(int nBitCount, int nW, int nH)
    {
        switch (nBitCount)
        {
            case 8:
            {
                var d = TDib.Create();
                d.SetSize(nW, nH, 8);
                return d;
            }
            case 16:
            {
                var d = TDib.Create();
                d.SetSize(nW, nH, 16);
                return d;
            }
            case 24:
            {
                var d = TDib.Create();
                d.SetSize(nW, nH, 24);
                return d;
            }
            case 32:
            {
                var d = TDib.Create();
                d.SetSize(nW, nH, 32);
                return d;
            }
            default:
                return null;
        }
    }

    /// <summary>GameImages.pas <c>MakeDibByPixelFormat</c>（Wzl.cs 已提供同一实现，此处转调）。</summary>
    public static TDib? WzlMakeDibByPixelFormat(byte pf, int nW, int nH)
        => TWzlImages.MakeDibByPixelFormat(pf, nW, nH);

    /// <summary><c>Move(src^, dest.PBits^, count)</c>（不足补 0，对应原文越界读的容错意图）。</summary>
    private static void CopyInto(TDib dest, byte[] src, int count)
    {
        int n = Math.Min(count, Math.Min(src.Length, dest.Bits.Length));
        if (n > 0) Array.Copy(src, 0, dest.Bits, 0, n);
    }

    // =================================================================================
    // LoadDxImageLzPak（原文 2471-2578）
    // =================================================================================

    /// <summary>
    /// Pak.pas 2471-2578 <c>LoadDxImageLzPak(nIndex, nImgOffset, nImgDataSize, dtsStyle, DXImage)</c> 1:1。
    /// <list type="number">
    /// <item>2486-2512（V0）：<c>nImgOffset &gt; 262 and nImgOffset + 12 &lt;= Size</c> 才读；
    ///       读 12 字节图片头（DES 变体）后 <c>nImgDataSize := nImgDataSize - 12</c>；
    ///       校验通过且 <c>nW * nH &gt; 4</c> 才建 <c>TMemoryStream(size := nImgDataSize)</c> 并
    ///       **要求 <c>Read</c> 返回值等于 nImgDataSize**（短读则保持 boError=True）。</item>
    /// <item>2513-2542（V1）：同样的门限但用 +16；<c>nImgDataSize := GetNewPakImageDataSize(ImageInfoV1)</c>
    ///       且必须 <c>&gt; 0</c> 才读。</item>
    /// <item>2544：<c>DXImage.dwLatestTime := MyGetTickCount();</c>（**无条件**，在 boError 判定之前）。</item>
    /// <item>2545-2573：<c>not boError</c> 时：
    ///       <c>Source &lt;&gt; nil</c> → 回填 4 字段 + 按风格建纹理；
    ///       <c>Source = nil</c> → 置 1×1 + <c>Surface := NULLTexture</c>（**Gray/Bright 也写 Surface**）。
    ///       <c>boError</c> 时 → 4 字段归零 + <c>Surface := nil</c>。</item>
    /// </list>
    /// <para><b>原文缺陷（照抄）</b>：2556 行 <c>end else if dtsBright = dtsBright then</c> ——
    /// 自我比较恒真；等价于「dtsNormal 走 Surface、dtsGray 走 Gray、**其余全部**走 Bright」。
    /// 又：2545 的 <c>not boError</c> 分支里，<c>dtsGray</c> 时写的是 <c>DXImage.Gray</c>，
    /// 但 <c>DXImage.Surface</c> 保持原值（GetCachedGray* 系列随后会读回 Gray）。</para>
    /// </summary>
    public void LoadDxImageLzPak(int nIndex, int nImgOffset, int nImgDataSize, TDxTextureStyle dtsStyle, TDxImage dXImage)
    {
        bool boError = true;
        TDib? source = null;
        TDib? alphaSource = null;
        int nW = 0, nH = 0, nPx = 0, nPy = 0;

        if (FPakFileType == TPakFileType.pftLzPakV0)
        {
            if (nImgOffset > PakConsts.LZ_PAK_FILE_HEADER_SIZE
                && nImgOffset + TPakImageInfo.SizeOf <= m_FileStream!.Length)
            {
                LockFileStream();
                try
                {
                    m_FileStream.Position = nImgOffset;
                    var buf = new byte[TPakImageInfo.SizeOf];
                    ReadImageHeader(buf, TPakImageInfo.SizeOf);
                    var imageInfoV0 = TPakImageInfo.FromBytes(buf, 0);

                    nImgDataSize -= TPakImageInfo.SizeOf;
                    if (IsValidLzImageInfoV0(imageInfoV0, nIndex, nImgDataSize, FileHeader.BitCount))
                    {
                        nW = imageInfoV0.wW;
                        nH = imageInfoV0.wH;
                        nPx = imageInfoV0.wPx;
                        nPy = imageInfoV0.wPy;
                        if (nW * nH > 4)
                        {
                            var msData = PakMemoryStream.Create();
                            msData.SetSize(nImgDataSize);
                            int got = ReadFully(m_FileStream, msData.Memory, 0, nImgDataSize);
                            if (got == nImgDataSize)
                            {
                                boError = !LoadLzImageDataV0(imageInfoV0, msData, out source, out alphaSource);
                            }
                        }
                        else
                        {
                            boError = false;   // 原文 2506
                        }
                    }
                }
                finally
                {
                    UnLockFileStream();
                }
            }
        }
        else
        {
            if (nImgOffset > PakConsts.LZ_PAK_FILE_HEADER_SIZE
                && nImgOffset + TNewPakImageInfo.SizeOf <= m_FileStream!.Length)
            {
                LockFileStream();
                try
                {
                    m_FileStream.Position = nImgOffset;
                    var buf = new byte[TNewPakImageInfo.SizeOf];
                    ReadImageHeader(buf, TNewPakImageInfo.SizeOf);
                    var imageInfoV1 = TNewPakImageInfo.FromBytes(buf, 0);

                    if (IsValidLzImageInfoV1(imageInfoV1, nIndex))
                    {
                        nW = imageInfoV1.nWidth;
                        nH = imageInfoV1.nHeight;
                        nPx = imageInfoV1.px;
                        nPy = imageInfoV1.py;
                        if (nW * nH > 4)
                        {
                            nImgDataSize = GetNewPakImageDataSize(imageInfoV1);
                            if (nImgDataSize > 0)
                            {
                                var msData = PakMemoryStream.Create();
                                msData.SetSize(nImgDataSize);
                                int got = ReadFully(m_FileStream, msData.Memory, 0, nImgDataSize);
                                if (got == nImgDataSize)
                                {
                                    boError = !LoadLzImageDataV1(imageInfoV1, msData, out source, out alphaSource);
                                }
                            }
                        }
                        else
                        {
                            boError = false;   // 原文 2535
                        }
                    }
                }
                finally
                {
                    UnLockFileStream();
                }
            }
        }

        dXImage.dwLatestTime = MyGetTickCount();
        if (!boError)
        {
            if (source != null)
            {
                dXImage.nWidth = (ushort)nW;
                dXImage.nHeight = (ushort)nH;
                dXImage.nPx = (short)nPx;
                dXImage.nPy = (short)nPy;

                if (dtsStyle == TDxTextureStyle.dtsNormal)
                    dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(source, alphaSource));
                else if (dtsStyle == TDxTextureStyle.dtsGray)
                    dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(source, alphaSource));
                else // 原文 2556：else if dtsBright = dtsBright then —— 恒真
                    dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(source, alphaSource));
            }
            else
            {
                dXImage.nWidth = 1;
                dXImage.nHeight = 1;
                dXImage.nPx = (short)nPx;
                dXImage.nPy = (short)nPy;
                dXImage.Surface = TTextureRef.NullTextureSentinel;
            }
        }
        else
        {
            dXImage.nWidth = 0;
            dXImage.nHeight = 0;
            dXImage.nPx = 0;
            dXImage.nPy = 0;
            dXImage.Surface = default;
        }
    }

    // =================================================================================
    // LoadDxImage / LoadDxGrayImage / LoadDxBrightImage（原文 2581-3186）
    // =================================================================================

    /// <summary>三个 LoadDx*Image 的风格（决定写哪个槽位 + 用哪组资源串）。</summary>
    private enum LoadStyle
    {
        Normal,
        Gray,
        Bright,
    }

    /// <summary>Pak.pas 2581-2784 <c>LoadDxImage</c> 1:1（Normal 风格）。</summary>
    public void LoadDxImage(int position, TDxImage dXImage, int index)
        => LoadDxImageCore(position, dXImage, index, LoadStyle.Normal);

    /// <summary>Pak.pas 2786-2988 <c>LoadDxGrayImage</c> 1:1（Gray 风格，资源串换成 SPak*GrayImage*）。</summary>
    public void LoadDxGrayImage(int position, TDxImage dXImage, int index)
        => LoadDxImageCore(position, dXImage, index, LoadStyle.Gray);

    /// <summary>Pak.pas 2990-3186 <c>LoadDxBrightImage</c> 1:1（Bright 风格）。</summary>
    public void LoadDxBrightImage(int position, TDxImage dXImage, int index)
        => LoadDxImageCore(position, dXImage, index, LoadStyle.Bright);

    /// <summary>
    /// Pak.pas 2581-3186 三支 <c>LoadDx*Image</c> 的公共 1:1 主体。
    /// <para>三支**逐字同构**，仅有三处差异（都被 <paramref name="style"/> 参数化）：
    /// <list type="number">
    /// <item>px/py 越界与 Length 越界/尺寸越界时的资源串（Normal 用 SPak*DxImage*，
    ///       Gray 用 SPak*DxGrayImage*，Bright 用 SPak*BrightImage*），以及解压失败的串
    ///       （SPakImageDecompressErr / SPakGrayImageDecompErr / SPakBrightImageDecompErr）。</item>
    /// <item>写纹理的目标槽位（Surface / Gray / Bright）。</item>
    /// <item><b>原文真实差异</b>：只有 <b>Normal</b> 分支在「<c>ImageInfo.Length &gt; 0</c> 且解压成功」时
    ///       **不**清黑底（2724-2725 的 <c>Brush.Color/FillRect</c> 被注释掉），
    ///       Gray/Bright 分支**有**清黑底（2929-2930/3127-3128）；且 Normal 分支在解压失败时
    ///       只释放 InData/OutData 而**不** FreeMem(InData)（2945-2946 vs 2716），
    ///       Normal 的未压缩读入路径里 <c>if InData &lt;&gt; nil then FreeMem(InData)</c> 缺失（原文如此）。</item>
    /// <item><b>「小图」分支的原文缺陷（照抄）</b>：<c>nWidth * nHeight &lt;= 4</c> 时三支都写
    ///       <c>DXImage.Surface := NULLTexture</c>（Gray/Bright 也写 Surface，见 2870/3072）。</item>
    /// </list></para>
    /// </summary>
    private void LoadDxImageCore(int position, TDxImage dXImage, int index, LoadStyle style)
    {
        bool boError = false;

        if (m_FileStream == null) return;

        if (position >= TPakFileHeader.SizeOf && position + TNewPakImageInfo.SizeOf <= m_FileStream.Length)
        {
            var buf = new byte[TNewPakImageInfo.SizeOf];

            if (FPakFileType == TPakFileType.pftPak1)
            {
                LockFileStream();
                try
                {
                    m_FileStream.Position = position;
                    ReadImageHeader(buf, TNewPakImageInfo.SizeOf);
                }
                finally
                {
                    UnLockFileStream();
                }
            }
            else if (FPakFileType == TPakFileType.pftPak2)
            {
                LockFileStream();
                try
                {
                    m_FileStream.Position = position;
                    ReadImageHeader_Pak2(buf, TNewPakImageInfo.SizeOf);
                }
                finally
                {
                    UnLockFileStream();
                }
            }
            else if (FPakFileType == TPakFileType.pftPak3)
            {
                LockFileStream();
                try
                {
                    m_FileStream.Position = position;
                    ReadImageHeader_Pak3(index, buf, TNewPakImageInfo.SizeOf);
                }
                finally
                {
                    UnLockFileStream();
                }
            }

            var imageInfo = TNewPakImageInfo.FromBytes(buf, 0);

            if (!IsKnownPixelFormat(imageInfo.PixelFormat)) boError = true;

            // 原文 2627-2633 的 nWidth/nHeight > 2048 检查被整块注释掉
            if (!boError && (PakSeams.AbsSmallInt(imageInfo.px) > GameImagesConsts.MAX_IMAGE_WIDTH
                || PakSeams.AbsSmallInt(imageInfo.py) > GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(PositionErrString(style)),
                    FileName, index, imageInfo.px, imageInfo.py));
                boError = true;
            }

            if (!boError && imageInfo.Length >= GameImagesConsts.MAX_IMAGE_SIZE)
            {
                OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(LengthErrString(style)),
                    FileName, index, imageInfo.Length));
                boError = true;
            }

            if (!boError && (imageInfo.nWidth <= 0 || imageInfo.nHeight <= 0))
            {
                OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(SizeErrString(style)),
                    FileName, index, imageInfo.nWidth, imageInfo.nHeight));
                boError = true;
            }

            if (!boError && (imageInfo.nWidth >= GameImagesConsts.MAX_IMAGE_WIDTH
                || imageInfo.nHeight >= GameImagesConsts.MAX_IMAGE_HEIGHT))
            {
                OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(SizeErrString(style)),
                    FileName, index, imageInfo.nWidth, imageInfo.nHeight));
                boError = true;
            }

            dXImage.dwLatestTime = MyGetTickCount();

            // 小图：原文写 Surface（Gray/Bright 也写 Surface —— 照抄）
            if (!boError && imageInfo.nWidth * imageInfo.nHeight <= 4)
            {
                dXImage.nWidth = 1;
                dXImage.nHeight = 1;
                dXImage.nPx = imageInfo.px;
                dXImage.nPy = imageInfo.py;
                dXImage.Surface = TTextureRef.NullTextureSentinel;
                return;
            }

            // 当图片出现错误时，让其重新更新 2020-05-29
            if (boError)
            {
                dXImage.nWidth = 0;
                dXImage.nHeight = 0;
                dXImage.nPx = 0;
                dXImage.nPy = 0;
                dXImage.Surface = default;
            }
            else
            {
                dXImage.nWidth = (ushort)imageInfo.nWidth;
                dXImage.nHeight = (ushort)imageInfo.nHeight;
                dXImage.nPx = imageInfo.px;
                dXImage.nPy = imageInfo.py;

                if (imageInfo.Length > 0)
                {
                    if (FPakFileType == TPakFileType.pftPak1 || FPakFileType == TPakFileType.pftPak2
                        || FPakFileType == TPakFileType.pftPak3)
                    {
                        var inData = new byte[imageInfo.Length];
                        LockFileStream();
                        try
                        {
                            m_FileStream.Position = position + TNewPakImageInfo.SizeOf;
                            ReadFully(m_FileStream, inData, 0, imageInfo.Length);
                        }
                        finally
                        {
                            UnLockFileStream();
                        }

                        int nSize;
                        if (imageInfo.PixelFormat == (byte)TPixelFormat.pf8bit)
                            nSize = PakConsts.WidthBytes(8, imageInfo.nWidth) * imageInfo.nHeight;
                        else if (imageInfo.PixelFormat == (byte)TPixelFormat.pf15bit
                            || imageInfo.PixelFormat == (byte)TPixelFormat.pf16bit)
                            nSize = PakConsts.WidthBytes(16, imageInfo.nWidth) * imageInfo.nHeight;
                        else if (imageInfo.PixelFormat == (byte)TPixelFormat.pf24bit)
                            nSize = PakConsts.WidthBytes(24, imageInfo.nWidth) * imageInfo.nHeight;
                        else if (imageInfo.PixelFormat == (byte)TPixelFormat.pf32bit)
                            nSize = PakConsts.WidthBytes(32, imageInfo.nWidth) * imageInfo.nHeight;
                        else
                            nSize = 0;

                        byte[]? outData = null;
                        try
                        {
                            outData = ZlibEx.DecompressBuf(inData, imageInfo.Length, nSize);
                            // 原文 DecompressBuf 失败会抛异常 → except 分支；C# 侧返回 null，补同一语义
                            if (outData == null) throw new InvalidOperationException("DecompressBuf failed");
                        }
                        catch
                        {
                            // 原文 2705-2714 的 except：FreeMem(InData)/FreeMem(OutData)（托管侧交给 GC）
                            outData = null;
                            OutMessage(DelphiRTL.Format(PakSeams.DecodeResStr(DecompErrString(style)), FileName, index));
                            boError = true;
                        }

                        if (!boError && outData != null)
                        {
                            var source = WzlMakeDibByPixelFormat(imageInfo.PixelFormat, imageInfo.nWidth, imageInfo.nHeight);
                            if (source != null)
                            {
                                // Normal 分支原文把「清黑底」两行**注释掉**了；Gray/Bright 分支保留
                                if (style != LoadStyle.Normal)
                                {
                                    // Source.Canvas.Brush.Color := clblack; Source.Canvas.FillRect(...)
                                    // SetSize 后位数据为 0 = 调色板索引 0 = 黑，等价
                                }

                                CopyInto(source, outData, source.Height * source.WidthBytes);

                                if (!imageInfo.boAlpha)
                                {
                                    AssignTexture(dXImage, style, source, null);
                                }
                                else
                                {
                                    var alphaSource = WzlMakeDibByBitCount(8, imageInfo.nWidth, imageInfo.nHeight);
                                    var alphaData = new byte[Math.Max(outData.Length - source.Height * source.WidthBytes, 0)];
                                    Array.Copy(outData, Math.Min(source.Height * source.WidthBytes, outData.Length),
                                        alphaData, 0, alphaData.Length);
                                    if (alphaSource != null) CopyInto(alphaSource, alphaData, alphaSource.Height * alphaSource.WidthBytes);
                                    AssignTexture(dXImage, style, source, alphaSource);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var source = WzlMakeDibByPixelFormat(imageInfo.PixelFormat, imageInfo.nWidth, imageInfo.nHeight);
                    if (source != null)
                    {
                        // Source.Canvas.Brush.Color := clblack; Source.Canvas.FillRect(Source.Canvas.ClipRect);
                        // （SetSize 已置 0 = 黑）

                        if (FPakFileType == TPakFileType.pftPak1 || FPakFileType == TPakFileType.pftPak2
                            || FPakFileType == TPakFileType.pftPak3)
                        {
                            LockFileStream();
                            try
                            {
                                m_FileStream.Position = position + TNewPakImageInfo.SizeOf;
                                ReadFully(m_FileStream, source.Bits, 0, source.Height * source.WidthBytes);
                            }
                            finally
                            {
                                UnLockFileStream();
                            }

                            if (!imageInfo.boAlpha)
                            {
                                AssignTexture(dXImage, style, source, null);
                            }
                            else
                            {
                                var alphaSource = WzlMakeDibByBitCount(8, imageInfo.nWidth, imageInfo.nHeight);
                                LockFileStream();
                                try
                                {
                                    m_FileStream.Position = position + TNewPakImageInfo.SizeOf + source.Height * source.WidthBytes;
                                    if (alphaSource != null)
                                        ReadFully(m_FileStream, alphaSource.Bits, 0, alphaSource.Height * alphaSource.WidthBytes);
                                }
                                finally
                                {
                                    UnLockFileStream();
                                }

                                AssignTexture(dXImage, style, source, alphaSource);
                            }
                        }
                    }
                }
            }
        }
    }

    private static void AssignTexture(TDxImage dXImage, LoadStyle style, TDib source, TDib? alphaSource)
    {
        if (style == LoadStyle.Normal)
            dXImage.Surface = TTextureRef.FromHandle(TextureSeams.NewTextureFn(source, alphaSource));
        else if (style == LoadStyle.Gray)
            dXImage.Gray = TTextureRef.FromHandle(TextureSeams.NewTextureGrayFn(source, alphaSource));
        else
            dXImage.Bright = TTextureRef.FromHandle(TextureSeams.NewTextureBrightFn(source, alphaSource));
    }

    private static string PositionErrString(LoadStyle style) => style switch
    {
        LoadStyle.Gray => PakResStrings.SPakLoadDxGrayImageErr,
        LoadStyle.Bright => PakResStrings.SPakBrightImageErr,
        _ => PakResStrings.SPakLoadDxImageErr,
    };

    private static string LengthErrString(LoadStyle style) => style switch
    {
        LoadStyle.Gray => PakResStrings.SPakLoadDxGrayImageLenErr,
        LoadStyle.Bright => PakResStrings.SPakBrightImageLenErr,
        _ => PakResStrings.SPakLoadDxImageLenErr,
    };

    private static string SizeErrString(LoadStyle style) => style switch
    {
        LoadStyle.Gray => PakResStrings.SPakLoadDxGrayImageSizeErr,
        LoadStyle.Bright => PakResStrings.SPakBrightImageSizeErr,
        _ => PakResStrings.SPakLoadDxImageSizeErr,
    };

    private static string DecompErrString(LoadStyle style) => style switch
    {
        LoadStyle.Gray => PakResStrings.SPakGrayImageDecompErr,
        LoadStyle.Bright => PakResStrings.SPakBrightImageDecompErr,
        _ => PakResStrings.SPakImageDecompErr,
    };

    // =================================================================================
    // 文件读取原语
    // =================================================================================

    /// <summary>
    /// Delphi <c>TFileStream.Read(Buffer, Count)</c>：返回实际读到的字节数。
    /// <para>原文多处**忽略**返回值（如 1264/1276/1299/1432/2689），也有必须比对的
    /// （2501/2529 的 <c>= nImgDataSize</c>）。此处统一返回实际字节数，由调用方决定是否比对。</para>
    /// </summary>
    public static int ReadFully(Stream stream, byte[] buffer, int offset, int count)
    {
        int total = 0;
        while (total < count)
        {
            int n = stream.Read(buffer, offset + total, count - total);
            if (n <= 0) break;
            total += n;
        }
        return total;
    }
}
