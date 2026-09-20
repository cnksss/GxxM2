using System;

namespace GXX.RunGate;

// =====================================================================================
// uBuffer.pas 缓冲链部分 1:1 转换（Source\RunGate\Common\uBuffer.pas）
//
// 本文件覆盖原文行号：
//   :96-124    TBufferLink 声明
//   :1317-1686 TBufferLink 全部方法实现
//
// 指针/缓冲区映射：
//   `PAnsiChar` + `len` → C# `byte[] buf` + `int offset` + `int len`
//   （原文 buf 只做 `buf^` / `NativeUInt(buf) + lvReadCount` 的字节访问，等价）。
//   `buf = nil` 是一个有意义的分支（ReadBufferWhileFindChar 原 :1618 用它做"只找不读"），
//   在 C# 里用 `buf == null` 保留同一分支。
//
// 两个"看起来一样实则不同"的方法（测试有差异断言）：
//   * ClearBuffer        （原 :1432-1457）释放**整条链**，四个游标全部清零；
//   * ClearHaveReadBuffer（原 :1459-1482）只释放**已读过**的前缀块（严格从 FRead.PrevEx 往前），
//                        把 FHead 移到 FRead；**FReadPosition 保持不动**，
//                        FMark/FMarkPosition 也**不动**（可能因此悬空 —— 见 D8）。
// =====================================================================================

/// <summary>
/// 原 :96-124 `TBufferLink = class` —— 内部分配用全局内存池的块链缓冲，
/// 提供顺序读游标（FRead/FReadPosition）与标记回滚（FMark/FMarkPosition）。
/// </summary>
public class TBufferLink
{
    // 原 :101-106 字段
    private TMemoryBlock FHead;      // 头部
    private TMemoryBlock FLast;      // 最后一个可用的内存块
    private TMemoryBlock FRead;      // 当前读到的 Buffer
    private uint FReadPosition;      // 当前读到的 Buffer 位置
    private TMemoryBlock FMark;      // 标记的内存块
    private uint FMarkPosition;      // 标记的内存块位置

    /// <summary>原文没有的只读探针（供测试与诊断）：当前读块 FRead。</summary>
    public TMemoryBlock ReadBlock => FRead;

    /// <summary>原文没有的只读探针：当前读位置 FReadPosition。</summary>
    public uint ReadPositionInternal => FReadPosition;

    /// <summary>原文没有的只读探针：标记块 FMark。</summary>
    public TMemoryBlock MarkBlock => FMark;

    /// <summary>原文没有的只读探针：标记位置 FMarkPosition。</summary>
    public uint MarkPositionInternal => FMarkPosition;

    /// <summary>原文没有的只读探针：头块 FHead。</summary>
    public TMemoryBlock HeadBlock => FHead;

    /// <summary>原文没有的只读探针：尾块 FLast。</summary>
    public TMemoryBlock LastBlock => FLast;

    /// <summary>供 <see cref="TDxMemoryStream.LinkToBufferList"/> 使用（同单元内直接写私有字段）。</summary>
    public void SetHeadBlock(TMemoryBlock b) => FHead = b;

    /// <summary>供 <see cref="TDxMemoryStream.LinkToBufferList"/> 使用。</summary>
    public void SetLastBlock(TMemoryBlock b) => FLast = b;

    /// <summary>原 :1484-1488 `constructor Create`（只把 FReadPosition 置 0）。</summary>
    public TBufferLink()
    {
        FReadPosition = 0;                                                 // 原 :1487
    }

    /// <summary>
    /// 原 :1490-1494 `destructor Destroy`（ClearBuffer + inherited）。
    /// <para>
    /// 托管侧刻意**不实现终结器**：原文的析构是显式调用（`BufList.Free`），而终结器会在线程池
    /// 线程上触碰全局内存池，进程退出阶段可能与 <c>FreeObjPool</c> 竞争。因此这里只提供
    /// <see cref="Dispose"/>，由调用方显式释放（等价于原文的 `Free`）。
    /// </para>
    /// </summary>
    public void Dispose()
    {
        ClearBuffer();                                                     // 原 :1492
    }

    /// <summary>
    /// 原 :1319-1413 `procedure AddBuffer(buf: PAnsiChar; len: Cardinal)`。
    /// <para>
    /// ① 若尾块未满（`FLast.DataLen &lt;&gt; BlockSize`），先把它填满（原 :1326-1357）；
    /// ② 仍未写完，则**按剩余长度选池**（`&lt;=` 逐级比较，原 :1360-1373），
    ///    取一个块并设 `DataLen := len`、`Move` 数据、挂到尾部（原 :1374-1391）；
    /// ③ 若剩余长度**超过最大池的块大小**（`MPool = nil`），取 SuperLarge 块、
    ///    `DataLen := SuperMemoryPool.FBlockSize`（**2048，不是 SuperLarge 的 16384** —— 原 :1395），
    ///    搬运后**递归**处理剩余（原 :1392-1412）。
    /// </para>
    /// <para>★ D9：`MemBlock^.DataLen := len`（原 :1377）把 Cardinal 赋给 `Word` 字段，
    /// len ≥ 65536 时**静默截断到低 16 位**；原 :1395 的同类赋值也截断（2048 → 2048，无影响）。
    /// 本移植用显式 `(ushort)` 保留截断，并有测试固定 `len = 65536 → DataLen = 0`。</para>
    /// </summary>
    public void AddBuffer(byte[] buf, uint len)
    {
        if (FLast != null)                                                 // 原 :1326
        {
            // 原 :1329-1337：按**尾块自己的** BlockType 选池算块大小
            uint BlockSize;
            switch (FLast.BlockType)
            {
                case TDxMemBlockType.MB_Small: BlockSize = (uint)MemoryPoolGlobal.SmallMemoryPool().BlockSize; break;
                case TDxMemBlockType.MB_Normal: BlockSize = (uint)MemoryPoolGlobal.MemoryPool().BlockSize; break;
                case TDxMemBlockType.MB_SpBig: BlockSize = (uint)MemoryPoolGlobal.SuperMemoryPool().BlockSize; break;
                case TDxMemBlockType.MB_Large: BlockSize = (uint)MemoryPoolGlobal.LargeMemoryPool().BlockSize; break;
                case TDxMemBlockType.MB_SPLarge: BlockSize = (uint)MemoryPoolGlobal.SuperLargeMemoryPool().BlockSize; break;
                default: BlockSize = (uint)MemoryPoolGlobal.BigMemoryPool().BlockSize; break;
            }
            if (FLast.DataLen != BlockSize)                                // 原 :1338
            {
                uint WSize = BlockSize - FLast.DataLen;                    // 原 :1340
                int bufOff = 0;                                            // 原 :1341-1342 pBuf := Memory; Inc(PBuf, DataLen)
                int pBuf = FLast.Offset(FLast.DataLen);
                if (WSize >= len)                                          // 原 :1343-1348
                {
                    BufferHelper.Copy(buf, bufOff, FLast.Memory, pBuf, (int)len);   // 原 :1345
                    FLast.DataLen = (ushort)(FLast.DataLen + (ushort)len);  // 原 :1346 Inc(FLast^.DataLen, len)
                    len = 0;                                               // 原 :1347
                }
                else                                                       // 原 :1349-1355
                {
                    BufferHelper.Copy(buf, bufOff, FLast.Memory, pBuf, (int)WSize);  // 原 :1351
                    len -= WSize;                                          // 原 :1352
                    bufOff += (int)WSize;                                  // 原 :1353 Inc(buf, WSize)
                    FLast.DataLen = (ushort)BlockSize;                     // 原 :1354
                }
            }
        }
        if (len == 0) return;                                              // 原 :1358-1359

        // 原 :1360-1373：按剩余长度选池（阈值是各池**对齐后**的 FBlockSize）
        TDxMemoryPool MPool;
        if (len <= (uint)MemoryPoolGlobal.SmallMemoryPool().BlockSize)
            MPool = MemoryPoolGlobal.SmallMemoryPool();
        else if (len <= (uint)MemoryPoolGlobal.MemoryPool().BlockSize)
            MPool = MemoryPoolGlobal.MemoryPool();
        else if (len <= (uint)MemoryPoolGlobal.BigMemoryPool().BlockSize)
            MPool = MemoryPoolGlobal.BigMemoryPool();
        else if (len <= (uint)MemoryPoolGlobal.SuperMemoryPool().BlockSize)
            MPool = MemoryPoolGlobal.SuperMemoryPool();
        else if (len <= (uint)MemoryPoolGlobal.LargeMemoryPool().BlockSize)
            MPool = MemoryPoolGlobal.LargeMemoryPool();
        else if (len <= (uint)MemoryPoolGlobal.SuperLargeMemoryPool().BlockSize)
            MPool = MemoryPoolGlobal.SuperLargeMemoryPool();
        else
            MPool = null;                                                  // 原 :1373

        // 注意原码两处重复的"挂尾"块（:1381-1390 与 :1399-1408）里，
        // 第一处用的是 `if FLast = nil then FLast := MemBlock`（不是 FHead），本移植照抄。
        if (MPool != null)                                                 // 原 :1374
        {
            TMemoryBlock MemBlock = MPool.GetMemoryBlock();                // 原 :1376
            MemBlock.DataLen = unchecked((ushort)len);                     // 原 :1377 ★ D9 截断
            MemBlock.NextEx = null;                                        // 原 :1378
            MemBlock.PrevEx = null;                                        // 原 :1379
            BufferHelper.Copy(buf, 0, MemBlock.Memory, 0, (int)len);       // 原 :1380
            if (FHead == null) FHead = MemBlock;                           // 原 :1381-1382
            if (FLast == null) FLast = MemBlock;                           // 原 :1383-1384
            else                                                           // 原 :1385-1390
            {
                FLast.NextEx = MemBlock;
                MemBlock.PrevEx = FLast;
                FLast = MemBlock;
            }
        }
        else                                                               // 原 :1392-1412
        {
            TMemoryBlock MemBlock = MemoryPoolGlobal.SuperLargeMemoryPool().GetMemoryBlock();   // 原 :1394
            MemBlock.DataLen = (ushort)MemoryPoolGlobal.SuperMemoryPool().BlockSize;            // 原 :1395（2048，非本池块大小）
            MemBlock.NextEx = null;                                        // 原 :1396
            MemBlock.PrevEx = null;                                        // 原 :1397
            BufferHelper.Copy(buf, 0, MemBlock.Memory, 0, MemBlock.DataLen);   // 原 :1398
            if (FHead == null) FHead = MemBlock;                           // 原 :1399-1400
            if (FLast == null) FLast = MemBlock;                           // 原 :1401-1402
            else                                                           // 原 :1403-1408
            {
                FLast.NextEx = MemBlock;
                MemBlock.PrevEx = FLast;
                FLast = MemBlock;
            }
            len -= MemBlock.DataLen;                                       // 原 :1409 Dec(len, MemBlock^.DataLen)
            byte[] rest = new byte[len];                                   // 原 :1410 Inc(buf, MemBlock^.DataLen)
            BufferHelper.Copy(buf, MemBlock.DataLen, rest, 0, (int)len);
            AddBuffer(rest, len);                                          // 原 :1411 递归
        }
    }

    /// <summary>
    /// 原 :1415-1430 `procedure AddMemBlockLink(MemBlockLink: PMemoryBlock)`。
    /// <para>
    /// ★ D10：入参 `MemBlockLink` 在第一行就被**立刻覆盖**（原 :1417
    /// `MemBlockLink := SmallMemoryPool.GetMemoryBlock;`），所以调用方传进来的块**被丢弃**，
    /// 函数行为等价于"追加一个空的 Small 池块"。这明显是原文笔误（应为 `MemBlockLink^.NextEx := nil`
    /// 之类），但本移植按原样保留（含该参数虽无意义的语义）。
    /// </para>
    /// </summary>
    public void AddMemBlockLink(TMemoryBlock MemBlockLink)
    {
        MemBlockLink = MemoryPoolGlobal.SmallMemoryPool().GetMemoryBlock();     // 原 :1417
        MemBlockLink.NextEx = null;                                        // 原 :1418
        MemBlockLink.PrevEx = null;                                        // 原 :1419
        if (FHead == null) FHead = MemBlockLink;                           // 原 :1420-1421
        if (FLast == null) FLast = MemBlockLink;                           // 原 :1422-1423
        else                                                               // 原 :1424-1429
        {
            FLast.NextEx = MemBlockLink;
            MemBlockLink.PrevEx = FLast;
            FLast = MemBlockLink;
        }
    }

    /// <summary>
    /// 原 :1432-1457 `procedure ClearBuffer` —— 从 FLast 沿 `PrevEx` 往回，
    /// 把**整条链**逐块 `FreeMemoryBlock`（按块自己的 BlockType 选池，原 :1441-1449），
    /// 然后把 FHead/FLast/FRead/FReadPosition/FMark/FMarkPosition **全部清零**。
    /// </summary>
    public void ClearBuffer()
    {
        TMemoryBlock lvBuf = FLast;                                        // 原 :1436
        while (lvBuf != null)                                              // 原 :1437-1450
        {
            TMemoryBlock lvFreeBuf = lvBuf;                                // 原 :1439
            lvBuf = lvBuf.PrevEx;                                          // 原 :1440
            FreeBlockByType(lvFreeBuf);                                    // 原 :1441-1449
        }
        FHead = null;                                                      // 原 :1451
        FLast = null;                                                      // 原 :1452
        FRead = null;                                                      // 原 :1453
        FReadPosition = 0;                                                 // 原 :1454
        FMark = null;                                                      // 原 :1455
        FMarkPosition = 0;                                                 // 原 :1456
    }

    /// <summary>
    /// 原 :1459-1482 `procedure ClearHaveReadBuffer` —— 只回收**已读过**的块：
    /// 从 `FRead.PrevEx` 起沿 `PrevEx` 往前逐块释放（原 :1464-1478），
    /// 再把 `FRead.PrevEx := nil`、`FHead := FRead`（原 :1480-1481）。
    /// <para>
    /// 与 <see cref="ClearBuffer"/> 的三处关键差异：
    /// ① FRead 为 nil 时**立即返回**（原 :1463，什么都不做）；
    /// ② FReadPosition / FMark / FMarkPosition **不动**（FRead 块连同其游标位置保留）；
    /// ③ 只释放 FRead 之前的块，FRead 及其后的块保留在链上。
    /// </para>
    /// <para>★ D8：`FMark` 若指向被释放的前缀块，会变成**悬空引用**（原文不修正）。</para>
    /// </summary>
    public void ClearHaveReadBuffer()
    {
        if (FRead == null) return;                                         // 原 :1463
        TMemoryBlock lvBuf = FRead.PrevEx;                                 // 原 :1464
        while (lvBuf != null)                                              // 原 :1465-1478
        {
            TMemoryBlock lvFreeBuf = lvBuf;                                // 原 :1467
            lvBuf = lvBuf.PrevEx;                                          // 原 :1468
            FreeBlockByType(lvFreeBuf);                                    // 原 :1469-1477
        }
        FRead.PrevEx = null;                                               // 原 :1480
        FHead = FRead;                                                     // 原 :1481
    }

    /// <summary>原 :1441-1449 / :1469-1477 的同一段 `case lvFreeBuf^.BlockType of` 分派。</summary>
    private static void FreeBlockByType(TMemoryBlock b)
    {
        switch (b.BlockType)
        {
            case TDxMemBlockType.MB_Small: MemoryPoolGlobal.SmallMemoryPool().FreeMemoryBlock(b); break;
            case TDxMemBlockType.MB_Normal: MemoryPoolGlobal.MemoryPool().FreeMemoryBlock(b); break;
            case TDxMemBlockType.MB_SpBig: MemoryPoolGlobal.SuperMemoryPool().FreeMemoryBlock(b); break;
            case TDxMemBlockType.MB_Large: MemoryPoolGlobal.LargeMemoryPool().FreeMemoryBlock(b); break;
            case TDxMemBlockType.MB_SPLarge: MemoryPoolGlobal.SuperLargeMemoryPool().FreeMemoryBlock(b); break;
            default: MemoryPoolGlobal.BigMemoryPool().FreeMemoryBlock(b); break;   // 原 :1447-1448
        }
    }

    /// <summary>
    /// 原 :1496-1521 `function InnerReadBuf(pvBufRecord, pvStartPostion, buf, len): Cardinal`。
    /// 从**单块**的 `pvStartPostion` 处最多拷 `len` 字节，返回实际拷出的字节数；
    /// 块内有效数据不足时**只拷块内剩余**并返回该剩余量（调用方负责跨块）。
    /// <para>★ 有效量按 Cardinal 无符号算：`DataLen - pvStartPostion`，
    /// 当 `pvStartPostion &gt; DataLen` 时会**下溢成巨大值**（原 :1504），
    /// 随后 `len &lt;= lvValidCount` 成立并拷贝 `len` 字节 —— 即**越界读块内存**。
    /// 正常路径不会出现（DataLen 由 AddBuffer 保证），异常路径见 D9 截断。</para>
    /// </summary>
    public static uint InnerReadBuf(TMemoryBlock pvBufRecord, uint pvStartPostion,
                                    byte[] buf, int offset, uint len)
    {
        uint Result = 0;                                                   // 原 :1501
        if (pvBufRecord != null)                                           // 原 :1502
        {
            uint lvValidCount = unchecked(pvBufRecord.DataLen - pvStartPostion);   // 原 :1504 本块剩余
            if (lvValidCount <= 0)                                         // 原 :1505（Cardinal 下 lvValidCount <= 0 仅当 == 0）
                Result = 0;                                                // 原 :1506
            else
            {
                if (len <= lvValidCount)                                   // 原 :1509 数据全在本块
                {
                    BufferHelper.Copy(pvBufRecord.Memory, (int)pvStartPostion, buf, offset, (int)len);   // 原 :1511
                    Result = len;                                          // 原 :1512
                }
                else                                                       // 原 :1514-1518
                {
                    BufferHelper.Copy(pvBufRecord.Memory, (int)pvStartPostion, buf, offset, (int)lvValidCount);   // 原 :1516
                    Result = lvValidCount;                                  // 原 :1517
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原 :1523-1572 `function ReadBuffer(buf: PAnsiChar; len: Cardinal): Cardinal` ——
    /// 从当前读游标顺序读最多 `len` 字节，返回实际读到的字节数（可能 &lt; len，链尾即止）。
    /// <para>游标推进：每读一块后 `FReadPosition += l`、`FRead := lvBuf`；
    /// 跨块时再 `FRead := lvBuf.NextEx; FReadPosition := 0`（原 :1559-1565）。</para>
    /// <para>★ FRead 为 nil 时从 FHead / 位置 0 开始（原 :1532-1536）。</para>
    /// </summary>
    public uint ReadBuffer(byte[] buf, int offset, uint len)
    {
        uint lvReadCount = 0;                                              // 原 :1529
        TMemoryBlock lvBuf = FRead;                                        // 原 :1530
        uint lvPosition = FReadPosition;                                   // 原 :1531
        if (lvBuf == null)                                                 // 原 :1532-1536
        {
            lvBuf = FHead;
            lvPosition = 0;
        }
        if (lvBuf == null) return 0;                                       // 原 :1570-1571
        uint lvRemain = len;                                               // 原 :1539
        while (lvBuf != null)                                              // 原 :1540
        {
            uint l = InnerReadBuf(lvBuf, lvPosition, buf, offset + (int)lvReadCount, lvRemain);   // 原 :1542
            if (l == lvRemain)                                             // 原 :1543 读完
            {
                lvReadCount += l;                                          // 原 :1546
                lvPosition += l;                                           // 原 :1547
                FReadPosition = lvPosition;                                // 原 :1548
                FRead = lvBuf;                                             // 原 :1549
                break;                                                     // 原 :1550
            }
            else if (l < lvRemain)                                         // 原 :1552 读的比需要的少
            {
                lvRemain -= l;                                             // 原 :1554
                lvReadCount += l;                                          // 原 :1555
                lvPosition += l;                                           // 原 :1556
                FReadPosition = lvPosition;                                // 原 :1557
                FRead = lvBuf;                                             // 原 :1558
                lvBuf = lvBuf.NextEx;                                      // 原 :1559
                if (lvBuf != null)                                         // 原 :1560-1565 读下一个
                {
                    FRead = lvBuf;
                    FReadPosition = 0;
                    lvPosition = 0;
                }
            }
            else
            {
                // ★ 原文缺陷：`l > lvRemain` 时**两个分支都不命中**，while 体会原地空转
                //   （lvBuf/lvRemain 都不变）→ 死循环。InnerReadBuf 在正常数据下不会返回
                //   l > lvRemain（lvRemain 从不大于本块有效量），仅在 DataLen 被 D9 截断
                //   等异常数据下可能发生。本移植原样保留（不注入 break），见报告"已知风险"。
                //   为可测性，此处不改写语义，但测试不构造该状态。
            }
        }
        return lvReadCount;                                                // 原 :1568
    }

    /// <summary>`ReadBuffer(buf, len)` 便捷重载。</summary>
    public uint ReadBuffer(byte[] buf, uint len) => ReadBuffer(buf, 0, len);

    /// <summary>
    /// 原 :1574-1655 `function ReadBufferWhileFindChar(buf: PAnsiChar; C: AnsiChar): Cardinal`
    /// —— 专为 mir2 网关写的"一直读到发现字符"（chongchong 2014-06-12）。
    /// <para>
    /// 语义：从当前读游标起逐块扫描，找到 `C` 时**把包括 C 在内的整段**拷到 `buf`
    /// （`lvRemain := lvIndex - lvPosition + 1`，原 :1611）并返回拷贝总量；
    /// 未找到的块整块拷走并推进到下一块；链尾无 C 时返回**已拷贝总量**（原 :1635 不执行，
    /// 循环自然结束，`Result` 保持初始 0 —— 注意：此时 `lvReadCount` 已被累加，
    /// 但**返回值仍是 0**！见 D11）。
    /// </para>
    /// <para>
    /// ★ D11（易错点，测试固定）：未找到 C 时函数返回 **0**，尽管中间块的字节已经被拷进
    /// `buf` 且读游标已经推进到最后一块的末尾。调用方若只看返回值会误判"没读到东西"。
    /// </para>
    /// <para>
    /// ★ D12：`buf = nil` 时只推进游标不拷贝（原 :1618-1625 的 else 分支 `l := lvRemain`），
    /// 用来"找字符并把游标移过去"。本移植用 `buf == null` 保留该分支。
    /// </para>
    /// <para>
    /// ★ D13：空块（`DataLen = 0`）且 `lvPosition = 0` 时，`lvRemain := lvBuf.DataLen - lvPosition`
    /// 在 Cardinal 下下溢为 0xFFFFFFFF；查找循环因 `DataLen - 1` 同样下溢而空转；
    /// 随后 `InnerReadBuf` 因有效量为 0 返回 0，`l &lt;&gt; lvRemain` →
    /// **抛 <see cref="InvalidOperationException"/>（原 `raise Exception.Create('TBufferLink.ReadBufferWhileFindChar Error')`，原 :1622）**。
    /// 只有 `FRead` **自身**为空块时才会 Break（原 :1615）；若 FRead 为 nil → FHead 是空块，则
    /// 第 ① 块在 `lvRemain = 0` 且 `boIsFoundChar = False` 时 `Break`（原 :1615）→ 返回 0。
    /// 链中间出现空块则会抛错。测试分别覆盖这两条路径。
    /// </para>
    /// </summary>
    public uint ReadBufferWhileFindChar(byte[] buf, byte C)
    {
        uint Result = 0;                                                   // 原 :1582
        uint lvReadCount = 0;                                              // 原 :1583
        TMemoryBlock lvBuf = FRead;                                        // 原 :1584
        uint lvPosition = FReadPosition;                                   // 原 :1585
        if (lvBuf == null)                                                 // 原 :1586-1590
        {
            lvBuf = FHead;
            lvPosition = 0;
        }
        if (lvBuf == null) return 0;                                       // 原 :1591

        uint lvRemain = unchecked((uint)ValidCount);                       // 原 :1593 `lvRemain := ValidCount`
        while (lvBuf != null)                                              // 原 :1594
        {
            bool boIsFoundChar = false;                                    // 原 :1596
            uint lvIndex = 0;                                              // 原 :1597

            // 原 :1599 `for I := lvPosition to lvBuf.DataLen - 1 do`
            // （Cardinal 无符号：DataLen = 0 时上界下溢成 0xFFFFFFFF；lvPosition = 0 时仍会进循环体
            //   并对 Memory[0] 做越界访问，原码在 Win32 上是读非法页 → AV。本移植用 checked 边界
            //   把这一路径显式化成 IndexOutOfRangeException，见 D13 注释。）
            for (uint I = lvPosition; I <= unchecked((uint)(lvBuf.DataLen - 1)); I++)
            {
                if (I >= lvBuf.Memory.Length)
                    throw new IndexOutOfRangeException(
                        "TBufferLink.ReadBufferWhileFindChar: 空块(DataLen=0)导致原码越界读（原 uBuffer.pas:1599-1608）");
                byte Chr = lvBuf.Memory[(int)I];                           // 原 :1601
                if (Chr == C)                                              // 原 :1602
                {
                    boIsFoundChar = true;                                  // 原 :1604
                    lvIndex = I;                                           // 原 :1605
                    break;                                                 // 原 :1606
                }
            }

            if (boIsFoundChar)
                lvRemain = unchecked(lvIndex - lvPosition + 1);            // 原 :1611（含分隔符本身）
            else
            {
                lvRemain = unchecked((uint)(lvBuf.DataLen - lvPosition));  // 原 :1614
                if (lvRemain == 0) break;                                  // 原 :1615（仅本块为空时退出）
            }

            uint l;
            if (buf != null)                                               // 原 :1618
            {
                l = InnerReadBuf(lvBuf, lvPosition, buf, (int)lvReadCount, lvRemain);   // 原 :1620
                if (l != lvRemain)                                         // 原 :1621
                    throw new InvalidOperationException("TBufferLink.ReadBufferWhileFindChar Error");   // 原 :1622
            }
            else
                l = lvRemain;                                              // 原 :1625

            if (boIsFoundChar)                                             // 原 :1627-1637 找到字符
            {
                lvReadCount += l;                                          // 原 :1631
                lvPosition += l;                                           // 原 :1632
                FReadPosition = lvPosition;                                // 原 :1633
                FRead = lvBuf;                                             // 原 :1634
                Result = lvReadCount;                                      // 原 :1635
                break;                                                     // 原 :1636
            }
            else                                                           // 原 :1638-1652
            {
                lvRemain -= l;                                             // 原 :1640
                lvReadCount += l;                                          // 原 :1641
                lvPosition += l;                                           // 原 :1642
                FReadPosition = lvPosition;                                // 原 :1643
                FRead = lvBuf;                                             // 原 :1644
                lvBuf = lvBuf.NextEx;                                      // 原 :1645
                if (lvBuf != null)                                         // 原 :1646-1651 读下一个
                {
                    FRead = lvBuf;
                    FReadPosition = 0;
                    lvPosition = 0;
                }
            }
        }
        return Result;                                                     // ★ D11：未找到时为 0
    }

    /// <summary>原 :1657-1661 `procedure RestoreReaderIndex`（把 FRead/FReadPosition 回滚到标记）。</summary>
    public void RestoreReaderIndex()
    {
        FRead = FMark;                                                     // 原 :1659
        FReadPosition = FMarkPosition;                                     // 原 :1660
    }

    /// <summary>原 :1663-1667 `procedure MarkReaderIndex`。</summary>
    public void MarkReaderIndex()
    {
        FMark = FRead;                                                     // 原 :1665
        FMarkPosition = FReadPosition;                                     // 原 :1666
    }

    /// <summary>
    /// 原 :1669-1686 `function ValidCount: Integer` —— 当前**尚可读**的字节数：
    /// `FRead` 为 nil → 从 FHead 起累加全部块的 DataLen；
    /// 否则 = `FRead.DataLen - FReadPosition` 加上后续所有块的 DataLen（沿 NextEx）。
    /// </summary>
    public int ValidCount
    {
        get
        {
            int Result = 0;                                                // 原 :1673
            TMemoryBlock lvNext;
            if (FRead == null)                                             // 原 :1674-1675
                lvNext = FHead;
            else                                                           // 原 :1676-1680
            {
                Result = FRead.DataLen - (int)FReadPosition;               // 原 :1678
                lvNext = FRead.NextEx;                                     // 原 :1679
            }
            while (lvNext != null)                                         // 原 :1681-1685
            {
                Result += lvNext.DataLen;                                  // 原 :1683
                lvNext = lvNext.NextEx;                                    // 原 :1684
            }
            return Result;
        }
    }
}
