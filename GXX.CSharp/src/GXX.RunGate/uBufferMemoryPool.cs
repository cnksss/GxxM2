using System;
using System.Threading;

namespace GXX.RunGate;

// =====================================================================================
// uBuffer.pas 内存池部分 1:1 转换（Source\RunGate\Common\uBuffer.pas，2,365 物理行）
//
// 本文件覆盖原文行号：
//   :14-25    TDxMemBlockType / PMemoryBlock / TMemoryBlock 类型声明
//   :27-52    TDxMemoryPool 声明
//   :180      GetTotalMemBytes 声明
//   :184-193  六个全局池变量 + TotalMemBytes / TotalMemLocker
//   :195-253  GetTotalMemBytes / AddMemory / DelMemory / 六个池工厂
//   :255-534  TDxMemoryPool 全部方法
//   :2352-2362 initialization / finalization（GetTotalMemBytes 部分）
//
// -------------------------------------------------------------------------------------
// 指针语义映射（重要）
//   Delphi `PMemoryBlock = ^TMemoryBlock`（record，2,365 行内全局 GetMem/FreeMem 手工管理）
//   在 C# 里用 `sealed class TMemoryBlock` 承载：
//     * `class` 是引用语义 → `Next/Prev/NextEx/PrevEx` 的赋值/比较天然等价于指针
//       （`p := p^.Next` → `p = p.Next`；`p <> nil` → `p != null`）；
//     * `Memory: Pointer` → `byte[]`（原码全程只做 `NativeUInt(Memory) + 偏移` 的字节访问，
//       没有任何类型化指针运算，所以 byte[] + 下标是完全等价的映射）；
//     * `DataLen: Word` → `ushort`（**保留 16 位截断**，见 :24 与 AddBuffer :1377 的注释）。
//   `SizeOf(TMemoryBlock)`（Delphi 32 位 = 32 字节）→ `TMemoryBlock.RecordSize`，
//   仅用于 `AddMemory/DelMemory` 的账目（测试断言用相对值而非硬编码绝对值）。
//
// -------------------------------------------------------------------------------------
// 六个全局内存池的 BlockSize —— **由脚本从原文抽取，非手工转录**
//
//   抽取命令（PowerShell，GBK 原文；`-Encoding Default` 是 GBK 页）：
//     $src = "D:\chuanqi\daima\GXX原版_Delphi7\Source\RunGate\Common\uBuffer.pas"
//     $lines = Get-Content -LiteralPath $src -Encoding Default
//     for ($i=0; $i -lt $lines.Count; $i++) {
//       if ($lines[$i] -match 'TDxMemoryPool\.Create\(') { "{0}: {1}" -f ($i+1), $lines[$i].Trim() }
//     }
//   回读结果（行号:1 文本，与 UTF-8 副本 C:\...\Temp\p2rg\uBuffer.pas.txt 行号一一对应）：
//     217: SuperBigMPool := TDxMemoryPool.Create(2048, 30, MB_SpBig, 30);
//     224: LargeMPool    := TDxMemoryPool.Create(4096, 30, MB_Large, 30);
//     231: SPLargeMPool  := TDxMemoryPool.Create(4096 * 4, 30, MB_SpLarge, 30);   //16KB
//     238: NormalMPool   := TDxMemoryPool.Create(640, 30, MB_Normal, 30);
//     245: SmallMPool    := TDxMemoryPool.Create(128, 30, MB_Small, 30);          //小内存块
//     252: BigMPool      := TDxMemoryPool.Create(1024, 30, MB_Big, 30);           //大内存块
//   → 请求值/InitCount/MaxFreeBlocks/枚举 = 128/30/30/MB_Small、640/…/MB_Normal、
//     1024/MB_Big、2048/MB_SpBig、4096/MB_Large、16384(4096*4)/30/30/MB_SpLarge。
//   → 对齐后 FBlockSize（原 :296-299 `(BlockSize div 64) * 64 + 64`，全部已是 64 的倍数）
//     = 128 / 640 / 1024 / 2048 / 4096 / 16384，即请求值本身。
//   测试 BufferMemoryPoolTests.GlobalPools_BlockSize_MatchExtractionFromSource 直接断言这六个数。
// =====================================================================================

/// <summary>
/// 原 :15 `TDxMemBlockType = (MB_Small, MB_Normal, MB_Big, MB_SpBig, MB_Large, MB_SPLarge)`。
/// **枚举顺序即序号，必须原样保留**（`case FMemBlockType of` 的分派与
/// `TMemoryBlock.BlockType` 的取值都依赖它）。
/// </summary>
public enum TDxMemBlockType
{
    MB_Small = 0,
    MB_Normal = 1,
    MB_Big = 2,
    MB_SpBig = 3,
    MB_Large = 4,
    MB_SPLarge = 5
}

/// <summary>
/// 原 :16-25 `PMemoryBlock = ^TMemoryBlock; TMemoryBlock = record ... end;`。
/// 用 sealed class 承载指针语义（见文件头说明）。
/// <para>`Memory: Pointer` → `byte[]`；`DataLen: Word` → `ushort`（保 16 位截断）。</para>
/// </summary>
public sealed class TMemoryBlock
{
    /// <summary>
    /// 原 `SizeOf(TMemoryBlock)`（Delphi 7 / Win32）= 4(Memory)+4+4+4+4+1(BlockType)+2(DataLen)
    /// = 23 → 按 4 字节对齐补齐到 **32**。仅用于 AddMemory/DelMemory 的账目。
    /// </summary>
    public const int RecordSize = 32;

    /// <summary>原 :18 `Memory: Pointer`。本移植用 byte[] 承载同一块 FBlockSize 字节的内存。</summary>
    public byte[] Memory;

    /// <summary>原 :19 `Next: PMemoryBlock`（内存池 use/unuse 双链用）。</summary>
    public TMemoryBlock Next;

    /// <summary>原 :20 `Prev: PMemoryBlock`。</summary>
    public TMemoryBlock Prev;

    /// <summary>原 :21 `NextEx: PMemoryBlock`（内存流/缓冲链的块链用）。</summary>
    public TMemoryBlock NextEx;

    /// <summary>原 :22 `PrevEx: PMemoryBlock`。</summary>
    public TMemoryBlock PrevEx;

    /// <summary>原 :23 `BlockType: TDxMemBlockType`（决定归还时落到哪个池）。</summary>
    public TDxMemBlockType BlockType;

    /// <summary>
    /// 原 :24 `DataLen: Word`。**必须是 ushort**：原文多处把 Cardinal/Integer 直接赋给它
    /// （如 :1377 `MemBlock^.DataLen := len`、:1395 `MemBlock^.DataLen := SuperMemoryPool.FBlockSize`），
    /// Delphi 在此处静默截断到 16 位；用 int 会改变行为。C# 的 `(ushort)` 转换同样静默截断
    /// （显式写在赋值处，见 TBufferLink.AddBuffer）。
    /// </summary>
    public ushort DataLen;

    /// <summary>原 :449-450 `GetMem(p, SizeOf(TMemoryBlock)); GetMem(p^.Memory, FBlockSize);`。</summary>
    public TMemoryBlock(int blockSize)
    {
        Memory = new byte[blockSize];
        DataLen = 0;
    }

    /// <summary>原 :476 `ZeroMemory(Result, FBlockSize)`（清零整块，不区分已写长度）。</summary>
    public void ZeroMemory()
    {
        Array.Clear(Memory, 0, Memory.Length);
    }

    /// <summary>
    /// 等价于 `NativeUInt(FCurBlock^.Memory) + 偏移`（原文到处出现的 pBuf 计算）。
    /// 原文该表达式是裸指针运算，本移植保持"基址 + 字节偏移"的语义。
    /// </summary>
    public int Offset(int pos) => pos;
}

/// <summary>
/// 原 :27-52 `TDxMemoryPool = class`。固定块大小的内存池：
/// <list type="bullet">
/// <item>两块链表：`FUseHead/FUseLast`（已分配）与 `FUnUseHead/FUnUseLast`（空闲），都走 `Next/Prev`；</item>
/// <item>另有 `NextEx/PrevEx` 四链，由使用方（内存流 / 缓冲链）自行组织；</item>
/// <item>原 :30 `FCs: TCriticalSection` → <see cref="Monitor"/> 上的 <see cref="Lock"/>/<see cref="Unlock"/>。</item>
/// </list>
/// </summary>
public class TDxMemoryPool : IDisposable
{
    // 原 :30 `FCs: TCriticalSection` → 托管侧 lock 对象（原 :526-534 Lock/Unlock = Enter/Leave）
    private readonly object FCs = new();

    // 原 :31 `FBlockSize: Integer` —— 64 字节对齐后的实际块大小（原 :296-299）
    private int FBlockSize;

    // 原 :32 `FMaxFreeBlocks: Integer`
    private int FMaxFreeBlocks;

    // 原 :33 `FUseHead: PMemoryBlock` —— 已使用链头
    private TMemoryBlock FUseHead;

    // 原 :34 `FUseLast: PMemoryBlock` —— 已使用链尾
    private TMemoryBlock FUseLast;

    // 原 :35 `FUnUseHead: PMemoryBlock` —— 空闲链头
    private TMemoryBlock FUnUseHead;

    // 原 :36 `FUnUseLast: PMemoryBlock` —— 空闲链尾
    private TMemoryBlock FUnUseLast;

    // 原 :37 `FUseCount: Integer`（原文只写不读，保留以便审计与测试）
    public int FUseCount;

    // 原 :38 `FFreeCount: Integer`（原文只写不读；:358/:415 的阈值判断用的是它）
    public int FFreeCount;

    // 原 :39 `FBlockType: TDxMemBlockType`
    private TDxMemBlockType FBlockType;

    /// <summary>
    /// 原 :43 `constructor Create(BlockSize, InitCount: Integer; BlockType: TDxMemBlockType;
    /// MaxFreeBlocks: Integer = 50)` —— 原 :290-327。
    /// 预分配 InitCount 个块挂到空闲链；块大小按 64 字节向上对齐（原 :295-299）。
    /// </summary>
    public TDxMemoryPool(int BlockSize, int InitCount, TDxMemBlockType BlockType, int MaxFreeBlocks = 50)
    {
        FBlockType = BlockType;                                            // 原 :294

        // 原 :295-299：块大小以 64 字节对齐
        //   if (BlockSize mod 64 = 0) then FBlockSize := BlockSize
        //   else FBlockSize := (BlockSize div 64) * 64 + 64;
        if (BlockSize % 64 == 0)
            FBlockSize = BlockSize;
        else
            FBlockSize = (BlockSize / 64) * 64 + 64;

        FMaxFreeBlocks = MaxFreeBlocks;                                    // 原 :300
        // 原 :301 FCS := TCriticalSection.Create;  → 托管侧 lock 对象已在字段初始化，无操作
        FMaxFreeBlocks = MaxFreeBlocks;                                    // 原 :302（原文重复赋值，保留）
        FUseCount = 0;                                                     // 原 :303
        FFreeCount = InitCount;                                            // 原 :304
        while (InitCount > 0)                                              // 原 :305-326
        {
            TMemoryBlock p = CreateRawBlock();                             // 原 :307-310
            if (FUnUseHead == null)                                        // 原 :311-317
            {
                FUnUseHead = p;
                FUnUseHead.Next = null;
                FUnUseHead.Prev = null;
                FUnUseLast = FUnUseHead;
            }
            else                                                           // 原 :318-324
            {
                p.Prev = FUnUseLast;
                p.Next = null;
                FUnUseLast.Next = p;
                FUnUseLast = p;
            }
            InitCount--;                                                   // 原 :325 Dec(InitCount)
        }
    }

    /// <summary>对齐后的块大小（原文为 private 字段，同单元内的 TDxMemoryStream 直接读它）。</summary>
    public int BlockSize => FBlockSize;

    /// <summary>原 :32 `FMaxFreeBlocks`。</summary>
    public int MaxFreeBlocks => FMaxFreeBlocks;

    /// <summary>原文没有的只读探针：`SizeOf(TMemoryBlock) + FBlockSize`，即每个块对全局账目的贡献。</summary>
    public int MemoryBlockBytes => TMemoryBlock.RecordSize + FBlockSize;

    /// <summary>原文没有的只读探针：已使用链长度（= 原 FUseCount）。</summary>
    public int UseCount => FUseCount;

    /// <summary>原文没有的只读探针：空闲链长度（= 原 FFreeCount）。</summary>
    public int FreeCount => FFreeCount;

    /// <summary>
    /// 原 :200-205 `procedure AddMemory(nSize: Integer)` —— 原文是 unit 级过程，
    /// 池内部各处（原 :309/:451/:490/:522）直接调用它。托管侧集中在
    /// <see cref="MemoryPoolGlobal.AddMemory"/>，这里保留同名的池内转发以便逐句对照。
    /// </summary>
    private static void AddMemory(int nSize) => MemoryPoolGlobal.AddMemory(nSize);

    /// <summary>原 :207-212 `procedure DelMemory(nSize: Integer)` 的池内转发。</summary>
    private static void DelMemory(int nSize) => MemoryPoolGlobal.DelMemory(nSize);

    /// <summary>
    /// 原 :449-451 / :488-491 的抽块动作（`GetMem` + `AddMemory` + `BlockType := FBlockType`）。
    /// 原文在 <see cref="GetMemory"/> / <see cref="GetMemoryBlock"/> 里各写了一遍，
    /// 另有从未被调用的 <see cref="InnerCreateBlock"/>（原 :518-524）是**完全相同的第三份死代码**。
    /// </summary>
    private TMemoryBlock CreateRawBlock()
    {
        // 原 :449-452
        TMemoryBlock p = new TMemoryBlock(FBlockSize);
        AddMemory(TMemoryBlock.RecordSize + FBlockSize);
        p.BlockType = FBlockType;
        return p;
    }

    /// <summary>
    /// 原 :518-524 `function InnerCreateBlock: PMemoryBlock`（protected）。
    /// **原文从未调用它**（死代码），此处按 1:1 保留以便审计。
    /// </summary>
    protected TMemoryBlock InnerCreateBlock()
    {
        return CreateRawBlock();
    }

    /// <summary>原 :526-529 `procedure Lock`（FCs.Enter）。原文为 public。</summary>
    public void Lock() => Monitor.Enter(FCs);

    // ---- 原文没有、仅为测试/诊断提供的探针 ----

    /// <summary>测试辅助：在**已使用链**（FUseHead 起，沿 Next）中查找 Memory 指针等于 p 的块。</summary>
    public TMemoryBlock FindUseBlock(byte[] p)
    {
        for (TMemoryBlock b = FUseHead; b != null; b = b.Next)
            if (ReferenceEquals(b.Memory, p)) return b;
        return null;
    }

    /// <summary>测试辅助：在**空闲链**（FUnUseHead 起，沿 Next）中查找 Memory 指针等于 p 的块。</summary>
    public TMemoryBlock FindFreeBlock(byte[] p)
    {
        for (TMemoryBlock b = FUnUseHead; b != null; b = b.Next)
            if (ReferenceEquals(b.Memory, p)) return b;
        return null;
    }

    /// <summary>测试辅助：在**已使用链**上与给定块记录**同一实例**的块（用于块记录级断言）。</summary>
    public TMemoryBlock FindUseBlock(TMemoryBlock block)
    {
        for (TMemoryBlock b = FUseHead; b != null; b = b.Next)
            if (ReferenceEquals(b, block)) return b;
        return null;
    }

    /// <summary>测试辅助：在**空闲链**上与给定块记录**同一实例**的块。</summary>
    public TMemoryBlock FindFreeBlock(TMemoryBlock block)
    {
        for (TMemoryBlock b = FUnUseHead; b != null; b = b.Next)
            if (ReferenceEquals(b, block)) return b;
        return null;
    }

    /// <summary>测试辅助：已使用链的实际长度（用于与 FUseCount 交叉校验）。</summary>
    public int CountUseChain()
    {
        int n = 0;
        for (TMemoryBlock b = FUseHead; b != null; b = b.Next) n++;
        return n;
    }

    /// <summary>测试辅助：空闲链的实际长度（用于与 FFreeCount 交叉校验）。</summary>
    public int CountFreeChain()
    {
        int n = 0;
        for (TMemoryBlock b = FUnUseHead; b != null; b = b.Next) n++;
        return n;
    }

    /// <summary>原 :531-534 `procedure Unlock`（FCs.Leave）。原文为 public。</summary>
    public void Unlock() => Monitor.Exit(FCs);

    /// <summary>
    /// 原 :441-480 `function GetMemory(Zero: Boolean): Pointer` —— 从空闲链取一个块
    /// （LIFO：取 FUnUseHead 并把它从空闲链摘下），挂到已使用链尾，返回 Memory。
    /// <paramref name="Zero"/> 为真时对**整块** FBlockSize 清零（原 :475-476）。
    /// </summary>
    public byte[] GetMemory(bool Zero)
    {
        Lock();                                                            // 原 :445
        try
        {
            TMemoryBlock p;
            if (FUnUseHead == null)                                        // 原 :447-453 新建
            {
                // 原 :449-452
                p = new TMemoryBlock(FBlockSize);
                AddMemory(TMemoryBlock.RecordSize + FBlockSize);
                p.BlockType = FBlockType;
            }
            else                                                           // 原 :454-459 直接从链表中取
            {
                p = FUnUseHead;
                FUnUseHead = FUnUseHead.Next;
                FFreeCount--;                                              // 原 :458 Dec(FFreeCount)
            }
            FUseCount++;                                                   // 原 :460 Inc(FUseCount)
            p.Next = null;                                                 // 原 :461
            if (FUseHead == null)                                          // 原 :462-467
            {
                FUseHead = p;
                p.Prev = null;
                FUseLast = p;
            }
            else                                                           // 原 :468-473
            {
                p.Prev = FUseLast;
                FUseLast.Next = p;
                FUseLast = p;
            }
            byte[] result = p.Memory;                                      // 原 :474
            if (Zero)                                                      // 原 :475-476
                p.ZeroMemory();                                            //   ZeroMemory(Result, FBlockSize)
            return result;
        }
        finally
        {
            Unlock();                                                      // 原 :478
        }
    }

    /// <summary>
    /// 原 :482-516 `function GetMemoryBlock: PMemoryBlock` —— 与 <see cref="GetMemory"/>
    /// 唯一差异：返回块记录本身（+ 取块时不 Zero）。注意 `Result` 在原文里是从未初始化的
    /// 函数返回值开始用（`GetMem(Result, …)`），本移植用局部变量等价。
    /// </summary>
    public TMemoryBlock GetMemoryBlock()
    {
        Lock();                                                            // 原 :484
        try
        {
            TMemoryBlock Result;
            if (FUnUseHead == null)                                        // 原 :486-492
            {
                Result = new TMemoryBlock(FBlockSize);
                AddMemory(TMemoryBlock.RecordSize + FBlockSize);
                Result.BlockType = FBlockType;
            }
            else                                                           // 原 :493-498
            {
                Result = FUnUseHead;
                FUnUseHead = FUnUseHead.Next;
                FFreeCount--;                                              // 原 :497
            }
            FUseCount++;                                                   // 原 :499
            Result.Next = null;                                            // 原 :500
            if (FUseHead == null)                                          // 原 :501-506
            {
                FUseHead = Result;
                Result.Prev = null;
                FUseLast = FUseHead;                                       //（原文是 FUseLast := FUseHead，与 GetMemory 的 FUseLast := p 等价）
            }
            else                                                           // 原 :507-512
            {
                Result.Prev = FUseLast;
                FUseLast.Next = Result;
                FUseLast = Result;
            }
            return Result;
        }
        finally
        {
            Unlock();                                                      // 原 :514
        }
    }

    /// <summary>
    /// 原 :336-388 `procedure FreeMemory(P: Pointer)` —— 按 Memory 指针在**已使用链**里
    /// 线性查找；找到后：
    /// <list type="number">
    /// <item>把它从 `Next/Prev` 链里摘掉（注意：**只改 Prev.Next，不改 Next.Prev**，原 :348-349）；</item>
    /// <item>若它是链尾则 `FUseLast := FUseLast.Prev`（原 :350-351）；</item>
    /// <item>空闲链为空 → 直接挂头（原 :352-357）；否则 `FFreeCount + 1 &lt; FMaxFreeBlocks` → 挂尾
    ///       （原 :358-363），**否则真释放**（原 :364-370）。</item>
    /// </list>
    /// <para>
    /// ★ 易错点：这里的阈值是 **严格小于** `&lt;`（`FFreeCount + 1 &lt; FMaxFreeBlocks`），
    /// 而 <see cref="FreeMemoryBlock"/> 是 **`&lt;=`**（原 :415）。同样的 FMaxFreeBlocks=2、
    /// 空闲 1 时，FreeMemory 会真释放、FreeMemoryBlock 会挂回空闲链 —— 见测试的差异断言。
    /// </para>
    /// <para>
    /// ★ 原文缺陷：`P` 不在本池（或已释放）时函数什么都不做、静默返回（原 :343-384 的循环
    /// 走到 `pBlock = nil` 就 break）；重复释放同一指针同样是静默 no-op，不会二次计入。
    /// 若 `P = nil` 则原 :340 直接 Exit。
    /// </para>
    /// </summary>
    public void FreeMemory(byte[] P)
    {
        if (P == null) return;                                             // 原 :340
        Lock();                                                            // 原 :341
        try
        {
            TMemoryBlock pBlock = FUseHead;                                // 原 :343
            // ★ 原文缺陷（本移植的安全等价）：原 :343 直接 `pBlock := FUseHead;` 后进入
            //   `while true do` 并在 :346 解引用 `PBlock^.Memory`。当已使用链为空
            //   （FUseHead = nil：P 不在本池、或 **重复释放** 已把最后一块摘走）时，
            //   原文是**访问违例（AV）**而非"静默返回"。这里提前返回，使
            //   "非本池指针/重复释放 = no-op" 的语义得以成立（并有测试固定）。
            if (pBlock == null) return;
            while (true)                                                   // 原 :344
            {
                if (ReferenceEquals(pBlock.Memory, P))                     // 原 :346 `if PBlock^.Memory = p then`
                {
                    // 原 :348-349：只改 Prev.Next —— 原文**没有**改 pBlock.Next.Prev
                    if (pBlock.Prev != null)
                        pBlock.Prev.Next = pBlock.Next;
                    // 原 :350-351
                    if (ReferenceEquals(FUseLast, pBlock))
                        FUseLast = FUseLast.Prev;
                    if (FUnUseHead == null)                                // 原 :352-357
                    {
                        FUnUseHead = pBlock;
                        pBlock.Prev = null;
                        FUnUseLast = pBlock;
                        //（原文此处未把 pBlock.Next 置 nil，由 :374 统一置 nil）
                    }
                    else if (FFreeCount + 1 < FMaxFreeBlocks)               // 原 :358 ★ 严格小于
                    {
                        pBlock.Prev = FUnUseLast;                          // 原 :360
                        FUnUseLast.Next = pBlock;                          // 原 :361
                        FUnUseLast = pBlock;                               // 原 :362
                    }
                    else                                                   // 原 :364-370 直接释放
                    {
                        FreeMemBlock(pBlock);                              // 原 :366-368 FreeMem(Memory)+FreeMem(block)+DelMemory
                        pBlock = null;                                     // 原 :369
                    }
                    if (pBlock != null)                                    // 原 :371-375
                    {
                        FFreeCount++;                                      // 原 :373 Inc(FFreeCount)
                        pBlock.Next = null;                                // 原 :374
                    }
                    FUseCount--;                                           // 原 :376 Dec(FUseCount)
                    if (FUseLast == null)                                  // 原 :377-378
                        FUseHead = null;
                    break;                                                 // 原 :379
                }
                pBlock = pBlock.Next;                                      // 原 :381
                if (pBlock == null) break;                                 // 原 :382-383
            }
            // ★ 原 :380 之前若 FUseHead 为 nil，原文会在此处直接 `pBlock^.Memory` 解引用 → AV。
            //   本移植保留同一形状，但 C# 抛 NullReferenceException（不可测，见报告"不覆盖项"）。
        }
        finally
        {
            Unlock();                                                      // 原 :386
        }
    }

    /// <summary>原 :366-368 `FreeMem(PBlock^.Memory); FreeMem(pBlock); DelMemory(SizeOf(TMemoryBlock) + FBlockSize);`。</summary>
    private void FreeMemBlock(TMemoryBlock p)
    {
        DelMemory(TMemoryBlock.RecordSize + FBlockSize);
        p.Memory = null;   // GetMem/FreeMem 后原指针悬空
    }

    /// <summary>
    /// 原 :390-439 `procedure FreeMemoryBlock(p: PMemoryBlock)` —— 把块记录本身归还。
    /// 先把 `p` 从 **NextEx/PrevEx** 链（原 :396-400）与 **Next/Prev** 链（原 :401-404）中摘除，
    /// 再按与 <see cref="FreeMemory"/> 相同的三种去向处理。
    /// <para>
    /// ★ 阈值是 **`FFreeCount + 1 &lt;= FMaxFreeBlocks`**（原 :415）—— 与 FreeMemory 的
    /// `<` 不同，这是原文刻意的/无意的 off-by-one 差异，必须原样保留。
    /// </para>
    /// </summary>
    public void FreeMemoryBlock(TMemoryBlock p)
    {
        if (p == null) return;                                             // 原 :392-393
        Lock();                                                            // 原 :394
        try
        {
            if (p.PrevEx != null)                                          // 原 :396-397
                p.PrevEx.NextEx = p.NextEx;
            if (p.NextEx != null)                                          // 原 :398-399
                p.NextEx.PrevEx = p.PrevEx;
            // 原 :400-404：从 Use 列表断开
            if (p.Prev != null)
                p.Prev.Next = p.Next;
            if (p.Next != null)
                p.Next.Prev = p.Prev;
            p.NextEx = null;                                               // 原 :405
            p.PrevEx = null;                                               // 原 :406
            if (ReferenceEquals(FUseLast, p))                              // 原 :407-408
                FUseLast = FUseLast.Prev;
            if (FUnUseHead == null)                                        // 原 :409-414
            {
                FUnUseHead = p;
                FUnUseHead.Prev = null;
                FUnUseLast = p;
            }
            else if (FFreeCount + 1 <= FMaxFreeBlocks)                     // 原 :415 ★ 小于等于
            {
                p.Prev = FUnUseLast;                                       // 原 :417
                FUnUseLast.Next = p;                                       // 原 :418
                FUnUseLast = p;                                            // 原 :419
            }
            else                                                           // 原 :421-427 直接释放
            {
                FreeMemBlock(p);                                           // 原 :423-425
                p = null;                                                  // 原 :426
            }
            FUseCount--;                                                   // 原 :428
            if (p != null)                                                 // 原 :429-433
            {
                FFreeCount++;                                              // 原 :431
                p.Next = null;                                             // 原 :432
            }
            if (FUseLast == null)                                          // 原 :434-435
                FUseHead = null;
        }
        finally
        {
            Unlock();                                                      // 原 :437
        }
    }

    /// <summary>
    /// 原 :257-288 `procedure TDxMemoryPool.Clear` —— 释放**两条链上的所有块**并把
    /// 四个头尾指针置 nil。
    /// <para>
    /// ★ 原文缺陷：**不清零 `FUseCount` / `FFreeCount`**（原 :281-284 只置 nil）。
    /// Clear 之后若继续用同一个池，`FUnUseHead` 为 nil → GetMemory 走新建路径，
    /// 但 `FFreeCount` 仍是旧值，`FreeMemory` 的 `<` 阈值判断会因此偏移。
    /// 测试 <c>Clear_DoesNotResetCounters</c> 固定这一行为。
    /// </para>
    /// </summary>
    public void Clear()
    {
        Lock();                                                            // 原 :261
        try
        {
            TMemoryBlock p = FUseHead;                                     // 原 :263
            while (p != null)                                              // 原 :264-271
            {
                TMemoryBlock tmp = p.Next;                                 // 原 :266
                DelMemory(TMemoryBlock.RecordSize + FBlockSize);           // 原 :269
                p.Memory = null;
                p = tmp;
            }
            p = FUnUseHead;                                               // 原 :272
            while (p != null)                                             // 原 :273-280
            {
                TMemoryBlock tmp = p.Next;                                 // 原 :275
                DelMemory(TMemoryBlock.RecordSize + FBlockSize);           // 原 :278
                p.Memory = null;
                p = tmp;
            }
            FUnUseHead = null;                                            // 原 :281
            FUnUseLast = null;                                            // 原 :282
            FUseHead = null;                                              // 原 :283
            FUseLast = null;                                              // 原 :284
            // ★ 原文不重置 FUseCount / FFreeCount（原 :281-284 无对应语句）—— 故意保留。
        }
        finally
        {
            Unlock();                                                      // 原 :286
        }
    }

    /// <summary>原 :329-334 `destructor Destroy`（Clear + FCS.Free + inherited）。</summary>
    public void Dispose()
    {
        Clear();                                                           // 原 :331
        // 原 :332 FCS.Free —— 托管侧 lock 对象无显式释放
    }
}

/// <summary>
/// 原文 `implementation` 段的全局变量与工厂（原 :184-253）+ initialization/finalization（原 :2352-2362）。
/// <para>
/// 托管侧映射：unit-level `var` → 静态字段；`initialization` → <see cref="ModuleInitializer"/>；
/// `finalization` → 进程退出钩子（<c>AppDomain.ProcessExit</c>）。
/// </para>
/// </summary>
public static class MemoryPoolGlobal
{
    // 原 :185-190
    private static TDxMemoryPool NormalMPool = null;
    private static TDxMemoryPool SmallMPool = null;        // 小内存块内存池
    private static TDxMemoryPool BigMPool = null;          // 大内存块内存池
    private static TDxMemoryPool SuperBigMPool = null;     // 超级大内存块内存池
    private static TDxMemoryPool LargeMPool = null;
    private static TDxMemoryPool SPLargeMPool = null;

    // 原 :192 `TotalMemBytes: Int64 = 0`
    private static long TotalMemBytes = 0;

    // 原 :193 `TotalMemLocker: TCriticalSection` → lock 对象
    private static readonly object TotalMemLocker = new();

    static MemoryPoolGlobal()
    {
        // 原 :2352-2356 initialization: TotalMemBytes := 0; TotalMemLocker := TCriticalSection.Create;
        TotalMemBytes = 0;
        // 原 :2358-2362 finalization: FreeObjPool; TotalMemLocker.Free;
        AppDomain.CurrentDomain.ProcessExit += (_, __) => FreeObjPool();
    }

    /// <summary>原 :195-198 `function GetTotalMemBytes: Int64`。</summary>
    public static long GetTotalMemBytes()
    {
        return TotalMemBytes;                                              // 原 :197
    }

    /// <summary>原 :200-205 `procedure AddMemory(nSize: Integer)`（加锁累加）。</summary>
    public static void AddMemory(int nSize)
    {
        lock (TotalMemLocker)                                              // 原 :202-204
        {
            TotalMemBytes = TotalMemBytes + nSize;                         // 原 :203
        }
    }

    /// <summary>原 :207-212 `procedure DelMemory(nSize: Integer)`（加锁累减）。</summary>
    public static void DelMemory(int nSize)
    {
        lock (TotalMemLocker)                                              // 原 :209-211
        {
            TotalMemBytes = TotalMemBytes - nSize;                         // 原 :210
        }
    }

    /// <summary>原 :214-219 `function SuperMemoryPool` —— BlockSize=2048, InitCount=30, MB_SpBig, MaxFree=30。</summary>
    public static TDxMemoryPool SuperMemoryPool()
    {
        if (SuperBigMPool == null)                                         // 原 :216
            SuperBigMPool = new TDxMemoryPool(2048, 30, TDxMemBlockType.MB_SpBig, 30);   // 原 :217
        return SuperBigMPool;                                              // 原 :218
    }

    /// <summary>原 :221-226 `function LargeMemoryPool` —— BlockSize=4096, 30, MB_Large, 30。</summary>
    public static TDxMemoryPool LargeMemoryPool()
    {
        if (LargeMPool == null)                                            // 原 :223
            LargeMPool = new TDxMemoryPool(4096, 30, TDxMemBlockType.MB_Large, 30);      // 原 :224
        return LargeMPool;                                                 // 原 :225
    }

    /// <summary>原 :228-233 `function SuperLargeMemoryPool` —— BlockSize=4096*4(=16384), 30, MB_SpLarge, 30。</summary>
    public static TDxMemoryPool SuperLargeMemoryPool()
    {
        if (SPLargeMPool == null)                                          // 原 :230
            SPLargeMPool = new TDxMemoryPool(4096 * 4, 30, TDxMemBlockType.MB_SPLarge, 30);  // 原 :231
        return SPLargeMPool;                                               // 原 :232
    }

    /// <summary>原 :235-240 `function MemoryPool` —— BlockSize=640, 30, MB_Normal, 30。</summary>
    public static TDxMemoryPool MemoryPool()
    {
        if (NormalMPool == null)                                           // 原 :237
            NormalMPool = new TDxMemoryPool(640, 30, TDxMemBlockType.MB_Normal, 30);     // 原 :238
        return NormalMPool;                                                // 原 :239
    }

    /// <summary>原 :242-247 `function SmallMemoryPool` —— BlockSize=128, 30, MB_Small, 30。</summary>
    public static TDxMemoryPool SmallMemoryPool()
    {
        if (SmallMPool == null)                                            // 原 :244
            SmallMPool = new TDxMemoryPool(128, 30, TDxMemBlockType.MB_Small, 30);       // 原 :245
        return SmallMPool;                                                 // 原 :246
    }

    /// <summary>原 :249-254 `function BigMemoryPool` —— BlockSize=1024, 30, MB_Big, 30。</summary>
    public static TDxMemoryPool BigMemoryPool()
    {
        if (BigMPool == null)                                              // 原 :251
            BigMPool = new TDxMemoryPool(1024, 30, TDxMemBlockType.MB_Big, 30);          // 原 :252
        return BigMPool;                                                   // 原 :253
    }

    /// <summary>
    /// 原文 :589-597 / :627-635 / :675-683 / :711-719 / :796-804 / :883-891 / :912-920 /
    /// :1049-1057 / :1160-1168 / :1249-1257 / :1330-1336 / :1836-1844 / :1946-1954 /
    /// :2190-2198 / :2287-2295 反复出现的同一段 `case ... of` —— 按块类型取池。
    /// <para>`MB_Big` **没有**显式分支，落在 `else`（原码如此）；因此 MB_Big 走 BigMemoryPool，
    /// 非法枚举值也走 BigMemoryPool。必须原样保留。</para>
    /// </summary>
    public static TDxMemoryPool GetPool(TDxMemBlockType type)
    {
        switch (type)
        {
            case TDxMemBlockType.MB_Small: return SmallMemoryPool();
            case TDxMemBlockType.MB_Normal: return MemoryPool();
            case TDxMemBlockType.MB_SpBig: return SuperMemoryPool();
            case TDxMemBlockType.MB_Large: return LargeMemoryPool();
            case TDxMemBlockType.MB_SPLarge: return SuperLargeMemoryPool();
            default: return BigMemoryPool();
        }
    }

    /// <summary>
    /// 原 :1758-1772 `procedure FreeObjPool` —— 释放六个全局池。
    /// <para>
    /// ★ 语义偏差（有意，见报告）：原文把六个变量**置空前先 Free**，但**不把变量置 nil**；
    /// 于是 finalization 之后再访问任一池工厂就是"使用已释放对象"→ 访问违例。
    /// 托管侧改为 Free + 置 nil（等价于原文 initialization 之前的初始状态），
    /// 之后再次访问会**重新创建**池而不是崩溃 —— 这对测试进程与进程退出都更安全，
    /// 且不改变任何正常路径的语义。
    /// </para>
    /// </summary>
    public static void FreeObjPool()
    {
        // 原 :1760-1771
        if (NormalMPool != null) { NormalMPool.Dispose(); NormalMPool = null; }
        if (SmallMPool != null) { SmallMPool.Dispose(); SmallMPool = null; }
        if (BigMPool != null) { BigMPool.Dispose(); BigMPool = null; }
        if (SuperBigMPool != null) { SuperBigMPool.Dispose(); SuperBigMPool = null; }
        if (LargeMPool != null) { LargeMPool.Dispose(); LargeMPool = null; }
        if (SPLargeMPool != null) { SPLargeMPool.Dispose(); SPLargeMPool = null; }
    }

    /// <summary>
    /// 原文没有的测试/诊断探针：当前六个池是否都还没创建。
    /// </summary>
    public static bool AllPoolsFreed =>
        NormalMPool == null && SmallMPool == null && BigMPool == null &&
        SuperBigMPool == null && LargeMPool == null && SPLargeMPool == null;

    /// <summary>
    /// 原文没有的测试辅助：把 TotalMemBytes 强制写回 0（原 :2354 initialization 的等价动作）。
    /// 仅用于在池已释放后让账目断言可以独立复核。
    /// </summary>
    public static void ResetTotalMemBytesForTest()
    {
        lock (TotalMemLocker) TotalMemBytes = 0;
    }
}
