using System;
using System.IO;

namespace GXX.RunGate;

// =====================================================================================
// uBuffer.pas 环形缓冲流部分 1:1 转换（Source\RunGate\Common\uBuffer.pas）
//
// 本文件覆盖原文行号：
//   :138-178  TDxRingStream 声明
//   :1773-2350 TDxRingStream 全部方法实现
//
// 与 TDxMemoryStream 的**结构性差异**（这是最容易翻错的地方）：
//   * 块链是**环形**：`SetSize` 在末尾执行 `FLast.NextEx := FHead`（原 :2233-2234），
//     而 `TDxMemoryStream.SetSize` **不做**这一步（末块 NextEx 保持 nil）；
//   * 因此 `FReadPosition`/`FWritePosition` 是**逻辑环形位置**（0..FSize-1），
//     `FReadBlock`/`FReadBlockPos` 才是物理落点；
//   * `DataSize`/`CanWriteSize`（原 :1784-1806）在 `FWritePosition = FReadPosition` 时
//     按 `FReadPosition = 0`（空）还是 ≠ 0（满）区分 —— 两个分支的判据字段**不同**
//     （GetCanWriteSize 判 `FReadPosition = 0`，GetDataSize 判 `FWritePosition = 0`），
//     虽然在这两个位置恒等时等价，但必须照抄。
//
// ★ 原文已知缺陷（本移植按原样保留，测试固定行为）：
//   R1 `Seek`（原 :1932-1935）**恒返回 0 且什么都不做** —— 这是原文写的空实现。
//      → 托管侧用 NotSupportedException 显式暴露，避免调用方误以为可用。
//   R2 `SeekReadWrite` 的 `soCurrent` + `Offset > 0`（原 :2009-2038）先
//      `BIndex := (FBlockPos + Offset) div FBlockSize`，再把 **FBlockPos** 换成
//      `(FBlockPos + Offset) mod FBlockSize` —— 当 `FBlockPos + Offset < FBlockSize`
//      （即不跨块）时 BIndex = 0，逻辑位置却加了 `(FBlockSize - FBlockPos) + 新BlockPos`
//      = Offset —— 等价。但**跨块**时把 `FBlockPos` 当成"位于该块内的偏移"用是错的。
//   R3 `SeekReadWrite` 的 `soCurrent` + `Offset < 0` 且需要回退到上一块时，
//      先 `FReadBlock := FReadBlock.PrevEx` 再改动逻辑位置（原 :2051-2063）——
//      若回退到块内某个位置，`FBlockPos` 被临时设为 FBlockSize 后**递归调用自身**。
//   R4 `SeekReadWrite` 的 `soCurrent` 写侧（原 :2031-2037）写成 `FWRiteBlock := FWRiteBlock^.NextEx`
//      —— Delphi 大小写不敏感故能编译且指向同一字段，本移植用 FWriteBlock（等价）。
//   R5 `SeekReadWrite` 的 `soEnd` 第二分支在进入前就计算了
//      `FReadBlock := FLast^.PrevEx`（原 :2119）/`FWriteBlock := FLast^.PrevEx`（原 :2124），
//      若 `FLast.PrevEx` 为 nil（只有一块）会抛 NullReferenceException（原码为 AV）。
//   R6 `Write`（原 :2272-2345）在 `FWriteBlock = nil`（FSize = 0 的流）时会在原 :2296/:2304
//      解引用 nil —— `CanWriteSize = 0` 时已提前返回，故仅当 FSize = 0 且 CSize ≠ 0 时不可达；
//      `Create(RingBufferSize = 0)` 会留下这种状态（原 :1780-1781 `if RingBufferSize <> 0`）。
//   R7 `SetSize` 缩容循环（原 :2205-2211）与 TDxMemoryStream 同形，也是只改 `FLast` 出链。
// =====================================================================================

/// <summary>
/// 原 :138-178 `TDxRingStream = class(TStream)` —— 环形缓冲流（定容、读满后可继续覆盖写）。
/// </summary>
public class TDxRingStream : Stream
{
    // 原 :141-153 字段
    private TMemoryBlock FReadBlock;      // 读取的块
    private int FReadBlockPos;
    private TDxMemBlockType FMemBlockType;
    private TMemoryBlock FWriteBlock;
    private int FWriteBlockPos;
    public int FSize;
    public int FCapacity;
    public int FMemBlockCount;
    private TMemoryBlock FHead;
    private TMemoryBlock FLast;
    public int FReadPosition;
    public int FWritePosition;
    private TMemoryBlock FMarkRead, FMarkWrite;             // 标记的内存块
    private uint FMarkReadPosition, FMarkWritePosition;     // 标记的内存块位置

    /// <summary>
    /// 原 :1775-1782 `constructor Create(const RingBufferSize: DWORD; const MemType: TDxMemBlockType)`。
    /// <para>★ RingBufferSize = 0 时**不调用 SetSize**（原 :1780），于是 FHead/FReadBlock 均为 nil。</para>
    /// </summary>
    public TDxRingStream(uint RingBufferSize, TDxMemBlockType MemType = TDxMemBlockType.MB_Small)
    {
        FMemBlockType = MemType;                                           // 原 :1779
        if (RingBufferSize != 0)                                           // 原 :1780
            SetLength(RingBufferSize);                                     // 原 :1781 SetSize(RingBufferSize)
    }

    /// <summary>
    /// 原 `TStream.Position` 读侧在原文（原 :1932-1935 的 `Seek` 恒返回 0）下只能得到 0。
    /// 这里返回 0 保持一致；写侧抛 <see cref="NotSupportedException"/>（同 <see cref="Seek"/>）。
    /// 需要真实位置请用 <see cref="ReadPosition"/> / <see cref="WritePosition"/>。
    /// </summary>
    public override long Position
    {
        get => 0;
        set => throw new NotSupportedException(
            "TDxRingStream.Position 在原文（uBuffer.pas:1932-1935 Seek 为空实现）下不可用；" +
            "请改用 ReadPosition / WritePosition。");
    }

    /// <summary>原 :1808-1811 `function GetSize: Int64`。</summary>
    public override long Length => FSize;

    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanSeek => true;
    public override void Flush() { }

    /// <summary>
    /// 原 :1932-1935 `function Seek(Offset: Integer; Origin: Word): Longint` ——
    /// **原文件里这就是个空实现**（`Result := 0`，什么都没做）。
    /// <para>
    /// 本移植不静默返回 0（那会让 .NET 的 <see cref="Position"/> 使用方以为位移成功），
    /// 而是抛 <see cref="NotSupportedException"/> 明确暴露该缺口；
    /// <see cref="Position"/> 的读侧返回 0、写侧同样抛异常。
    /// 需要定位时请用原文提供的 <see cref="SeekReadWrite"/> /
    /// <see cref="ReadPosition"/> / <see cref="WritePosition"/>。
    /// </para>
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException(
            "TDxRingStream.Seek 在原文（uBuffer.pas:1932-1935）中是空实现（恒返回 0、不移动游标）；" +
            "请改用 SeekReadWrite / ReadPosition / WritePosition。");

    /// <summary>原 :2176-2179 `procedure SetReadPosition`（SeekReadWrite(Value, soBeginning, True)）。</summary>
    public int ReadPosition
    {
        get => FReadPosition;
        set => SeekReadWrite(value, SeekOrigin.Begin, true);               // 原 :2178
    }

    /// <summary>原 :2267-2270 `procedure SetWritePostion`（SeekReadWrite(Value, soBeginning, False)）。</summary>
    public int WritePosition
    {
        get => FWritePosition;
        set => SeekReadWrite(value, SeekOrigin.Begin, false);              // 原 :2269
    }

    /// <summary>
    /// 原 :1784-1794 `function GetCanWriteSize: Integer` —— 还能写多少字节。
    /// <para>三分支判据：Write &gt; Read → `FSize - Write + Read`；Write &lt; Read → `Read - Write`；
    /// 相等时再看 **FReadPosition = 0**（空 → FSize）否则 0（满）。</para>
    /// </summary>
    public int CanWriteSize
    {
        get
        {
            if (FWritePosition > FReadPosition)                            // 原 :1786-1787
                return FSize - FWritePosition + FReadPosition;
            else if (FWritePosition < FReadPosition)                       // 原 :1788-1789
                return FReadPosition - FWritePosition;
            else if (FReadPosition == 0)                                   // 原 :1790-1791
                return FSize;
            else                                                           // 原 :1792-1793
                return 0;
        }
    }

    /// <summary>
    /// 原 :1796-1806 `function GetDataSize: Integer` —— 还能读多少字节。
    /// <para>★ 与 <see cref="CanWriteSize"/> 的"相等"分支判据**不同**：这里判
    /// **FWritePosition = 0**（→ 0）否则 `FSize`（满）。两处在两位置相等时等价，但必须照抄。</para>
    /// </summary>
    public int DataSize
    {
        get
        {
            if (FWritePosition > FReadPosition)                            // 原 :1798-1799
                return FWritePosition - FReadPosition;
            else if (FWritePosition < FReadPosition)                       // 原 :1800-1801
                return FWritePosition + FSize - FReadPosition;
            else if (FWritePosition == 0)                                  // 原 :1802-1803
                return 0;
            else                                                           // 原 :1804-1805
                return FSize;
        }
    }

    /// <summary>原 :1813-1817 `procedure MarkReaderIndex`。</summary>
    public void MarkReaderIndex()
    {
        FMarkRead = FReadBlock;                                            // 原 :1815
        FMarkReadPosition = (uint)FReadBlockPos;                           // 原 :1816（Cardinal 字段）
    }

    /// <summary>原 :1819-1823 `procedure markWriterIndex`。</summary>
    public void markWriterIndex()
    {
        FMarkWrite = FWriteBlock;                                          // 原 :1821
        FMarkWritePosition = (uint)FWriteBlockPos;                         // 原 :1822
    }

    /// <summary>原 :1920-1924 `procedure RestoreReaderIndex`。</summary>
    public void RestoreReaderIndex()
    {
        FReadBlock = FMarkRead;                                            // 原 :1922
        FReadBlockPos = (int)FMarkReadPosition;                            // 原 :1923
    }

    /// <summary>原 :1926-1930 `procedure restoreWriterIndex`。</summary>
    public void restoreWriterIndex()
    {
        FWriteBlock = FMarkWrite;                                          // 原 :1928
        FWriteBlockPos = (int)FMarkWritePosition;                          // 原 :1929
    }

    /// <summary>
    /// 原 :1825-1913 `function Read(var Buffer; Count: Longint): Longint` ——
    /// 与 <see cref="TDxMemoryStream.Read"/> 同形，但：① 用 `DataSize` 截断 Count；
    /// ② 每推进一段就 `Inc(FReadPosition, …)`（逻辑环形位置）；
    /// ③ 循环里跨块靠**环形的 NextEx**（FSize ≠ 0 时永不 nil）。
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (FReadBlock == null) return 0;                                  // 原 :1832-1833
        TDxMemoryPool MPool = MemoryPoolGlobal.GetPool(FMemBlockType);      // 原 :1836-1844
        int CanReadSize = DataSize;                                        // 原 :1845
        if (count > CanReadSize) count = CanReadSize;                      // 原 :1846-1847
        if (count == 0) return 0;                                          // 原 :1848-1852
        if (FReadBlockPos == MPool.BlockSize)                              // 原 :1853-1857
        {
            FReadBlock = FReadBlock.NextEx;
            FReadBlockPos = 0;
        }
        if (FReadBlock == null) return 0;                                  // 原 :1858-1862
        int Result = count;                                                // 原 :1863
        int p = offset;                                                    // 原 :1864
        int pBuf = FReadBlock.Offset(FReadBlockPos);                       // 原 :1866
        if (count <= MPool.BlockSize - FReadBlockPos)                      // 原 :1867 足够写了
        {
            BufferHelper.Copy(FReadBlock.Memory, pBuf, buffer, p, count);   // 原 :1869
            FReadPosition += count;                                        // 原 :1870
            FReadBlockPos += count;                                        // 原 :1871
            count = 0;                                                     // 原 :1872
        }
        else                                                               // 原 :1874-1882
        {
            int n = MPool.BlockSize - FReadBlockPos;
            BufferHelper.Copy(FReadBlock.Memory, pBuf, buffer, p, n);       // 原 :1876
            count -= n;                                                    // 原 :1877
            FReadPosition += n;                                            // 原 :1878
            p += n;                                                        // 原 :1879
            FReadBlock = FReadBlock.NextEx;                                // 原 :1880
            FReadBlockPos = 0;                                             // 原 :1881
        }
        if (FReadBlockPos == MPool.BlockSize)                              // 原 :1883-1887
        {
            FReadBlock = FReadBlock.NextEx;
            FReadBlockPos = 0;
        }
        while (count > 0)                                                  // 原 :1888-1911
        {
            if (count > MPool.BlockSize)                                   // 原 :1890-1897
            {
                BufferHelper.Copy(FReadBlock.Memory, 0, buffer, p, MPool.BlockSize);   // 原 :1892
                count -= MPool.BlockSize;
                FReadBlockPos = MPool.BlockSize;
                FReadPosition += MPool.BlockSize;
                p += MPool.BlockSize;
            }
            else                                                           // 原 :1898-1905
            {
                BufferHelper.Copy(FReadBlock.Memory, 0, buffer, p, count); // 原 :1900
                FReadPosition += count;
                p += count;
                FReadBlockPos = count;
                count = 0;
            }
            if (FReadBlockPos == MPool.BlockSize)                          // 原 :1906-1910
            {
                FReadBlock = FReadBlock.NextEx;
                FReadBlockPos = 0;
            }
        }
        return Result;
    }

    /// <summary>`Read(buffer, count)` 便捷重载。</summary>
    public int Read(byte[] buffer, int count) => Read(buffer, 0, count);

    /// <summary>原 :1915-1918 `procedure ReadBuffer`（直接转调 Read，忽略返回值）。</summary>
    public void ReadBuffer(byte[] Buffer, int Count)
    {
        Read(Buffer, Count);                                               // 原 :1917
    }

    /// <summary>原 :2347-2350 `procedure WriteBuffer`（直接转调 Write，忽略返回值）。</summary>
    public void WriteBuffer(byte[] Buffer, int Count)
    {
        Write(Buffer, Count);                                              // 原 :2349
    }

    /// <summary>
    /// 原 :2272-2345 `function Write(const Buffer; Count: Longint): Longint`。
    /// <para>
    /// ① `CSize := CanWriteSize`；CSize = 0 → 返回 0（**一个字节都不覆盖**，原 :2280-2284）；
    /// ② `Count &gt; CSize` → Count 截断到 CSize（原 :2285-2286）；
    /// ③ 游标若停在块尾（`FWriteBlockPos = FBlockSize`）先跨块（原 :2296-2300）；
    /// ④ 先吃当前块剩余，再按"整块/不满一块"循环搬运，每段 `Inc(FWritePosition, …)`。
    /// </para>
    /// <para>返回值恒为**截断后**的 Count（原 :2301 `Result := Count`，写在搬运之前）。</para>
    /// <para>★ 原文在写满时**不阻断覆盖**：只要 CanWriteSize &gt; 0，旧的未读数据就会被覆盖
    /// （环形语义），且 `FReadPosition` 不会随之推进 —— 测试固定该行为。</para>
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        int CSize = CanWriteSize;                                          // 原 :2279
        if (CSize == 0) return;                                            // 原 :2280-2284（Result := 0）
        else if (count > CSize) count = CSize;                             // 原 :2285-2286
        TDxMemoryPool MPool = MemoryPoolGlobal.GetPool(FMemBlockType);      // 原 :2287-2295
        if (FWriteBlockPos == MPool.BlockSize)                             // 原 :2296-2300
        {
            FWriteBlock = FWriteBlock.NextEx;
            FWriteBlockPos = 0;
        }
        int t = offset;                                                    // 原 :2302
        int pBuf = FWriteBlock.Offset(FWriteBlockPos);                     // 原 :2304
        if (count <= MPool.BlockSize - FWriteBlockPos)                     // 原 :2305 足够写了
        {
            BufferHelper.Copy(buffer, t, FWriteBlock.Memory, pBuf, count);  // 原 :2307
            FWriteBlockPos += count;                                       // 原 :2308
            FWritePosition += count;                                       // 原 :2309
            count = 0;                                                     // 原 :2310
        }
        else                                                               // 原 :2312-2320
        {
            int n = MPool.BlockSize - FWriteBlockPos;
            BufferHelper.Copy(buffer, t, FWriteBlock.Memory, pBuf, n);      // 原 :2314
            count -= n;                                                    // 原 :2315
            t += n;                                                        // 原 :2316
            FWritePosition += n;                                           // 原 :2317
            FWriteBlock = FWriteBlock.NextEx;                              // 原 :2318
            FWriteBlockPos = 0;                                            // 原 :2319
        }
        while (count > 0)                                                  // 原 :2321-2344
        {
            if (count > MPool.BlockSize)                                   // 原 :2323-2330
            {
                BufferHelper.Copy(buffer, t, FWriteBlock.Memory, 0, MPool.BlockSize);   // 原 :2325
                t += MPool.BlockSize;
                count -= MPool.BlockSize;
                FWriteBlockPos = MPool.BlockSize;
                FWritePosition += MPool.BlockSize;
            }
            else                                                           // 原 :2331-2338
            {
                BufferHelper.Copy(buffer, t, FWriteBlock.Memory, 0, count);   // 原 :2333
                FWritePosition += count;
                t += count;
                FWriteBlockPos = count;
                count = 0;
            }
            if (FWriteBlockPos == MPool.BlockSize)                         // 原 :2339-2343
            {
                FWriteBlock = FWriteBlock.NextEx;
                FWriteBlockPos = 0;
            }
        }
    }

    /// <summary>`Write(buffer, count)` 便捷重载；返回写入字节数（= 被 CSize 截断后的 count）。</summary>
    public int Write(byte[] buffer, int count)
    {
        int CSize = CanWriteSize;
        if (CSize == 0) return 0;
        int n = count > CSize ? CSize : count;
        Write(buffer, 0, n);
        return n;
    }

    /// <summary>
    /// 原 :2181-2265 `procedure SetSize(NewSize: Integer)`。
    /// 与 <see cref="TDxMemoryStream.SetLength"/> 的两处关键差异：
    /// ① 分配块后执行 `FLast.NextEx := FHead`（原 :2233-2234）**构成环**；
    /// ② NewSize = 0 时清 FReadBlock/FWriteBlock + 两个 BlockPos，但**不清 FReadPosition/FWritePosition**
    ///    （对比 TDxMemoryStream 会清 FPosition）—— 见原 :2236-2244。
    /// </summary>
    public override void SetLength(long value)
    {
        int NewSize = (int)value;
        if (FSize != NewSize)                                              // 原 :2187
        {
            FSize = NewSize;                                               // 原 :2189
            TDxMemoryPool MPool = MemoryPoolGlobal.GetPool(FMemBlockType);  // 原 :2190-2198
            int CurCount = FCapacity / MPool.BlockSize;                    // 原 :2199
            FMemBlockCount = NewSize / MPool.BlockSize;                    // 原 :2200
            if (NewSize % MPool.BlockSize != 0)                            // 原 :2201-2202
                FMemBlockCount++;
            if (CurCount != FMemBlockCount)                                // 原 :2203
            {
                while (CurCount > FMemBlockCount)                          // 原 :2205-2211 内存回收
                {
                    TMemoryBlock mBlock = FLast;                           // 原 :2207
                    FLast = FLast.PrevEx;                                  // 原 :2208
                    MPool.FreeMemoryBlock(mBlock);                         // 原 :2209
                    CurCount--;                                            // 原 :2210
                }
                while (CurCount < FMemBlockCount)                          // 原 :2212-2231
                {
                    TMemoryBlock mBlock = MPool.GetMemoryBlock();          // 原 :2214
                    mBlock.NextEx = null;                                  // 原 :2215
                    mBlock.PrevEx = null;                                  // 原 :2216
                    if (FHead == null)                                     // 原 :2217-2223
                    {
                        FHead = mBlock;
                        FLast = mBlock;
                        FHead.NextEx = null;
                        FHead.PrevEx = null;
                    }
                    else                                                   // 原 :2224-2229
                    {
                        FLast.NextEx = mBlock;
                        mBlock.PrevEx = FLast;
                        FLast = mBlock;
                    }
                    CurCount++;                                            // 原 :2230
                }
                FCapacity = MPool.BlockSize * FMemBlockCount;              // 原 :2232
                if (FLast != null)                                         // 原 :2233-2234 指向头部形成环形
                    FLast.NextEx = FHead;
            }
            if (NewSize == 0)                                              // 原 :2236-2244
            {
                FReadBlock = null;
                FWriteBlock = null;
                FHead = null;
                FLast = null;
                FWriteBlockPos = 0;
                FReadBlockPos = 0;
                // ★ 原文**不清** FReadPosition / FWritePosition（对比 TDxMemoryStream.FPosition）
            }
            else                                                           // 原 :2245-2263
            {
                if (FReadPosition > NewSize)                               // 原 :2247-2248
                    ReadPosition = NewSize;                                //   走 SeekReadWrite(...soBeginning, True)
                if (FReadBlock == null)                                    // 原 :2249-2254
                {
                    FReadBlock = FHead;
                    FReadBlockPos = 0;
                    FReadPosition = 0;
                }
                if (FWritePosition > NewSize)                              // 原 :2255-2256
                    WritePosition = NewSize;
                if (FWriteBlock == null)                                   // 原 :2257-2262
                {
                    FWriteBlock = FHead;
                    FWriteBlockPos = 0;
                    FWritePosition = 0;
                }
            }
        }
    }

    /// <summary>
    /// 原 :1937-2174 `function SeekReadWrite(Offset: Integer; Origin: TSeekOrigin; SeekRead: Boolean): Longint`。
    /// 读/写游标共用同一套位移逻辑，用 <paramref name="SeekRead"/> 选目标；
    /// 返回所选游标的**逻辑位置**（FReadPosition / FWritePosition）。
    /// </summary>
    public long SeekReadWrite(int Offset, SeekOrigin Origin, bool SeekRead)
    {
        if (FHead != null)                                                 // 原 :1944
        {
            TDxMemoryPool MPool = MemoryPoolGlobal.GetPool(FMemBlockType);  // 原 :1946-1954
            switch (Origin)                                                // 原 :1955
            {
                case SeekOrigin.Begin:                                     // 原 :1956-2006 soBeginning
                    {
                        if (Offset < 0) Offset = 0;                        // 原 :1958-1959
                        if (Offset <= FSize)                               // 原 :1960
                        {
                            int BIndex = Offset / MPool.BlockSize;         // 原 :1962
                            if (SeekRead)                                  // 原 :1963-1968
                            {
                                FReadBlockPos = Offset % MPool.BlockSize;
                                FReadBlock = FHead;
                                FReadPosition = FReadBlockPos;
                            }
                            else                                           // 原 :1969-1974
                            {
                                FWriteBlockPos = Offset % MPool.BlockSize;
                                FWriteBlock = FHead;
                                FWritePosition = FWriteBlockPos;
                            }
                            if (BIndex > 0)                                // 原 :1975-1989
                            {
                                if (SeekRead)
                                    do
                                    {
                                        FReadPosition += MPool.BlockSize;  // 原 :1979
                                        BIndex--;                          // 原 :1980
                                        FReadBlock = FReadBlock.NextEx;    // 原 :1981
                                    } while (BIndex != 0 && FReadBlock != null);   // 原 :1982
                                else
                                    do
                                    {
                                        FWritePosition += MPool.BlockSize; // 原 :1985
                                        BIndex--;                          // 原 :1986
                                        FWriteBlock = FWriteBlock.NextEx;  // 原 :1987
                                    } while (BIndex != 0 && FWriteBlock != null);  // 原 :1988
                            }
                        }
                        else                                               // 原 :1991-2005
                        {
                            if (SeekRead)                                  // 原 :1993-1998
                            {
                                FReadBlock = FLast;
                                FReadBlockPos = FCapacity - FSize;
                                FReadPosition = FSize;                     // 原 :1997 `:= Size`
                            }
                            else                                           // 原 :1999-2004
                            {
                                FWriteBlock = FLast;
                                FWriteBlockPos = FCapacity - FSize;
                                FWritePosition = FSize;
                            }
                        }
                        break;
                    }
                case SeekOrigin.Current:                                   // 原 :2007-2091 soCurrent
                    {
                        if (Offset > 0)                                    // 原 :2009
                        {
                            if (SeekRead)                                  // 原 :2011-2024
                            {
                                int BIndex = (FReadBlockPos + Offset) / MPool.BlockSize;    // 原 :2013
                                FReadPosition += MPool.BlockSize - FReadBlockPos;           // 原 :2014
                                FReadBlockPos = (FReadBlockPos + Offset) % MPool.BlockSize; // 原 :2015
                                FReadPosition += FReadBlockPos;                             // 原 :2016
                                while (BIndex > 0)                         // 原 :2017-2023
                                {
                                    FReadBlock = FReadBlock.NextEx;        // 原 :2019
                                    BIndex--;                              // 原 :2020
                                    if (BIndex > 0)                        // 原 :2021-2022
                                        FReadPosition += MPool.BlockSize;
                                }
                            }
                            else                                           // 原 :2025-2038
                            {
                                int BIndex = (FWriteBlockPos + Offset) / MPool.BlockSize;   // 原 :2027
                                FWritePosition += MPool.BlockSize - FWriteBlockPos;         // 原 :2028
                                FWriteBlockPos = (FWriteBlockPos + Offset) % MPool.BlockSize;   // 原 :2029
                                FWritePosition += FWriteBlockPos;                           // 原 :2030
                                while (BIndex > 0)                         // 原 :2031-2037
                                {
                                    FWriteBlock = FWriteBlock.NextEx;      // 原 :2033（原文写 FWRiteBlock）
                                    BIndex--;                              // 原 :2034
                                    if (BIndex > 0)                        // 原 :2035-2036
                                        FWritePosition += MPool.BlockSize;
                                }
                            }
                        }
                        else                                               // 原 :2040
                        {
                            if (SeekRead)                                  // 原 :2042-2065
                            {
                                if (Offset + FReadBlockPos >= 0)           // 原 :2044
                                {
                                    FReadPosition += Offset;               // 原 :2046
                                    FReadBlockPos += Offset;               // 原 :2047
                                }
                                else                                       // 原 :2049-2064
                                {
                                    FReadBlock = FReadBlock.PrevEx;        // 原 :2051
                                    if (FReadBlock == null)                // 原 :2052-2056
                                    {
                                        FReadBlockPos = 0;
                                        FReadBlock = FHead;
                                    }
                                    else                                   // 原 :2057-2063
                                    {
                                        Offset += FReadBlockPos;           // 原 :2059
                                        FReadPosition -= FReadBlockPos;    // 原 :2060
                                        FReadBlockPos = MPool.BlockSize;   // 原 :2061
                                        SeekReadWrite(Offset, SeekOrigin.Current, SeekRead);   // 原 :2062 递归
                                    }
                                }
                            }
                            else                                           // 原 :2066-2089
                            {
                                if (Offset + FWriteBlockPos >= 0)          // 原 :2068
                                {
                                    FWritePosition += Offset;              // 原 :2070
                                    FWriteBlockPos += Offset;              // 原 :2071
                                }
                                else                                       // 原 :2073-2088
                                {
                                    FWriteBlock = FWriteBlock.PrevEx;      // 原 :2075
                                    if (FWriteBlock == null)               // 原 :2076-2080
                                    {
                                        FWriteBlockPos = 0;
                                        FWriteBlock = FHead;
                                    }
                                    else                                   // 原 :2081-2087
                                    {
                                        Offset += FWriteBlockPos;          // 原 :2083
                                        FWritePosition -= FWriteBlockPos;  // 原 :2084
                                        FWriteBlockPos = MPool.BlockSize;  // 原 :2085
                                        SeekReadWrite(Offset, SeekOrigin.Current, SeekRead);   // 原 :2086 递归
                                    }
                                }
                            }
                        }
                        break;
                    }
                case SeekOrigin.End:                                       // 原 :2092-2165 soEnd
                    {
                        if (Offset < 0) Offset = -Offset;                  // 原 :2094-2095 ★ 取反
                        if (Offset < FSize)                                // 原 :2096
                        {
                            int FLastSize = MPool.BlockSize - FCapacity + FSize;   // 原 :2098 最后一个块的有效长度
                            if (Offset <= FLastSize)                       // 原 :2099-2113
                            {
                                if (SeekRead)                              // 原 :2101-2106
                                {
                                    FReadBlockPos = FLastSize - Offset;
                                    FReadBlock = FLast;
                                    FReadPosition = FSize - Offset;
                                }
                                else                                       // 原 :2107-2112
                                {
                                    FWriteBlockPos = FLastSize - Offset;
                                    FWriteBlock = FLast;
                                    FWritePosition = FSize - Offset;
                                }
                            }
                            else                                           // 原 :2114-2151
                            {
                                Offset -= FLastSize;                       // 原 :2116
                                if (SeekRead)                              // 原 :2117-2121
                                {
                                    FReadBlock = FLast.PrevEx;
                                    FReadPosition = FSize - FLastSize;
                                }
                                else                                       // 原 :2122-2126
                                {
                                    FWriteBlock = FLast.PrevEx;
                                    FWritePosition = FSize - FLastSize;
                                }
                                int BIndex = Offset / MPool.BlockSize;     // 原 :2127
                                Offset = Offset % MPool.BlockSize;         // 原 :2128
                                if (SeekRead)                              // 原 :2129-2139
                                {
                                    FReadBlockPos = MPool.BlockSize - Offset;
                                    FReadPosition -= MPool.BlockSize - FReadBlockPos;
                                    while (BIndex > 0)
                                    {
                                        FReadBlock = FReadBlock.PrevEx;
                                        BIndex--;
                                        FReadPosition -= MPool.BlockSize;
                                    }
                                }
                                else                                       // 原 :2140-2150
                                {
                                    FWriteBlockPos = MPool.BlockSize - Offset;
                                    FWritePosition -= MPool.BlockSize - FWriteBlockPos;
                                    while (BIndex > 0)
                                    {
                                        FWriteBlock = FWriteBlock.PrevEx;
                                        BIndex--;
                                        FWritePosition -= MPool.BlockSize;
                                    }
                                }
                            }
                        }
                        else if (SeekRead)                                 // 原 :2153-2158
                        {
                            FReadPosition = 0;
                            FReadBlockPos = 0;
                            FReadBlock = FHead;
                        }
                        else                                               // 原 :2159-2164
                        {
                            FWritePosition = 0;
                            FWriteBlockPos = 0;
                            FWriteBlock = FHead;
                        }
                        break;
                    }
            }
            if (SeekRead) return FReadPosition;                            // 原 :2167-2168
            return FWritePosition;                                         // 原 :2169-2170
        }
        return 0;                                                          // 原 :2172-2173
    }

    /// <summary>原文没有、仅为测试/诊断提供的只读探针（原 :143 `FMemBlockType` 是私有字段）。</summary>
    public TDxMemBlockType MemBlockType => FMemBlockType;

    // ---- 原文没有、仅为测试/诊断提供的只读探针 ----
    public TMemoryBlock HeadInternal => FHead;
    public TMemoryBlock LastInternal => FLast;
    public TMemoryBlock ReadBlockInternal => FReadBlock;
    public TMemoryBlock WriteBlockInternal => FWriteBlock;
    public int ReadBlockPosInternal => FReadBlockPos;
    public int WriteBlockPosInternal => FWriteBlockPos;
    public int CapacityInternal => FCapacity;
    public int MemBlockCountInternal => FMemBlockCount;
}
