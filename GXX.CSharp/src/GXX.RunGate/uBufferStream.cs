using System;
using System.IO;

namespace GXX.RunGate;

// =====================================================================================
// uBuffer.pas 内存流部分 1:1 转换（Source\RunGate\Common\uBuffer.pas）
//
// 本文件覆盖原文行号：
//   :54-94    TDxMemoryStream 声明
//   :535-1316 TDxMemoryStream 全部方法实现
//
// 语义要点（保真点，逐条对照原文）：
//   * 内存来自六个全局池之一（由 FMemBlockType 决定，见 GetPool / 原 :556-565）；
//   * 块之间用 NextEx/PrevEx 串成**单向可回退**的双链表（无环，与 TDxRingStream 不同）；
//   * `Read`/`Write` 的 `var Buffer` → C# `(byte[] buffer, int offset, int count)`；
//     Delphi `TStream.Read/Write` 返回**实际**字节数（这里 = 被截断后的 Count）；
//   * `Seek(Offset: Integer; Origin: Word)` → `Seek(long offset, SeekOrigin origin)`；
//     Origin 的 Word 取值 0/1/2 = soBeginning/soCurrent/soEnd；
//   * `SetSize(NewSize: Integer)` → `SetLength(long)` override（.NET 同名语义）。
//
// ★ 原文已知缺陷（本移植按原样保留，测试中固定其行为）：
//   D1 `Seek(0, soEnd)`（Offset >= 0 落在原 :1017-1022 的 else 分支）会回到 **位置 0** 而不是末尾。
//   D2 `Read`（原 :720-721）用 `Count := FSize - FPosition` 截断长度，但**不检查 FCurBlock 是否
//      还有后续块**；当 FCurBlockPos 恰好跨块后 FCurBlock 为 nil 时会在原 :766/:774 解引用 nil。
//      正常路径下 SetSize 保证块数足够，故仅在"块链被外部破坏"时触发。
//   D3 `Write`（原 :1171-1177）只在 `FCurBlock = FLast` 时才扩 FSize；覆盖写（FPosition+Count <= FSize）
//      不改变 FSize —— 这与 TStream.Write 的常规预期不同，属刻意行为。
//   D4 `WriteStream`（原 :1263）扩容量写成 `MPool.FBlockSize - FCurBlockPos - Len + FSize`，
//      与 `Write`（原 :1174 的 `FSize + Count`）**不同**；两者在跨块时结果一致，是等价化简。
//   D5 `LinkToBufferList` 把块送走后**不重置 FMemBlockType**（原 :605-614），
//      且把 `FLast.DataLen` 设成 `BSize - (FCapacity - FSize)`（原 :603）。
//   D6 `SwapStreamLink`（原 :1116-1152）**不交换 FSize 的托管侧写法会丢大小**：
//      接收方 `Stream.FSize := OldSize` 拿的是**发送方原来的大小**，而发送方的
//      `FSize := Stream.Size` 在 Stream 为空时会是 0 —— 即"发送方变空、接收方拿到旧大小"。
//      原文 D6 的组合结果是：块链已经易主，但**接收方的 FSize 是已清空流的旧大小(=0)**，
//      而 `FCapacity`/`FMemBlockCount` 却是真实块数 —— 二者不一致。测试固定该行为。
//   D7 `LoadFromBufferList`/`LoadFromStream` 的 `for Windex := 0 to FMemBlockCount - 1`
//      循环体里跟着 `NextEx` 走（原 :645/:693）；末块的 NextEx 是 nil，靠循环次数兜住。
//      若块数不足（外部破坏）会解引用 nil（LoadFromBufferList 有 `if tmpBlock = nil then Break;`
//      保护，LoadFromStream 没有 —— 见原 :687-693）。
// =====================================================================================

/// <summary>
/// 原 :54-94 `TDxMemoryStream = class(TStream)` —— 依附于全局内存池的块式内存流。
/// </summary>
public class TDxMemoryStream : Stream
{
    // 原 :58-66 字段
    private TMemoryBlock FHead;         // 头块
    private TMemoryBlock FLast;         // 末块
    private TMemoryBlock FCurBlock;     // 当前读写块
    private int FCurBlockPos;           // 当前块内位置
    private TMemoryBlock FMarkBlock;    // 标记块
    private int FMarkBlokPos;           // 标记位置（原文拼写 Blok，保留）
    public int FSize;                 // 逻辑大小
    public int FPosition;             // 逻辑位置
    public int FCapacity;             // 已分配的块总字节数
    private TDxMemBlockType FMemBlockType;
    public int FMemBlockCount;        // 块数

    /// <summary>
    /// 原 :554-566 `function GetBlockSize: DWORD` —— **同单元内直接读池的 private FBlockSize**。
    /// 注意 `else Result := 0`：非法枚举值返回 0（Delphi 不检查 case 覆盖度）。
    /// </summary>
    public static int GetPoolBlockSize(TDxMemBlockType type)
    {
        switch (type)
        {
            case TDxMemBlockType.MB_Small: return MemoryPoolGlobal.SmallMemoryPool().BlockSize;
            case TDxMemBlockType.MB_Normal: return MemoryPoolGlobal.MemoryPool().BlockSize;
            case TDxMemBlockType.MB_Big: return MemoryPoolGlobal.BigMemoryPool().BlockSize;
            case TDxMemBlockType.MB_SpBig: return MemoryPoolGlobal.SuperMemoryPool().BlockSize;
            case TDxMemBlockType.MB_Large: return MemoryPoolGlobal.LargeMemoryPool().BlockSize;
            case TDxMemBlockType.MB_SPLarge: return MemoryPoolGlobal.SuperLargeMemoryPool().BlockSize;
            default: return 0;
        }
    }

    /// <summary>
    /// 原文 :589-597 / :627-635 / :675-683 / :711-719 / :796-804 / :883-891 / :912-920 /
    /// :1049-1057 / :1160-1168 / :1249-1257 反复出现的同一段 `case` —— 取本流对应池。
    /// 实现在 <see cref="MemoryPoolGlobal.GetPool"/>（TDxRingStream 也要用同一段逻辑）。
    /// 注意 `else` 落在 **BigMemoryPool**（不是 Small），必须原样保留。
    /// </summary>
    public static TDxMemoryPool GetPool(TDxMemBlockType type) => MemoryPoolGlobal.GetPool(type);

    /// <summary>原 :554-566 `GetBlockSize` 的实例属性版本（原 :92 `property BlockSize: DWORD`）。</summary>
    public int BlockSize => GetPoolBlockSize(FMemBlockType);

    /// <summary>原 :72 `constructor Create(const MemType: TDxMemBlockType = MB_Small)` —— 原 :542-546。</summary>
    public TDxMemoryStream(TDxMemBlockType MemType = TDxMemBlockType.MB_Small)
    {
        FMemBlockType = MemType;                                           // 原 :545
    }

    /// <summary>原 :90-92 只读属性 Head / Last / BlockSize。</summary>
    public TMemoryBlock Head => FHead;
    public TMemoryBlock Last => FLast;

    /// <summary>原 :93 `property MemBlockType` 的读侧。</summary>
    public TDxMemBlockType MemBlockType => FMemBlockType;

    /// <summary>原 :1031-1038 `procedure SetMemBlockType` —— 换类型会先把流清空。</summary>
    public void SetMemBlockType(TDxMemBlockType value)
    {
        if (FMemBlockType != value)                                        // 原 :1033
        {
            Clear();                                                       // 原 :1035
            FMemBlockType = value;                                         // 原 :1036
        }
    }

    /// <summary>
    /// 原 :68-71 `function GetSize: Int64`；原 :568-571 实现。
    /// <para>
    /// .NET 基类 <see cref="Stream.Length"/> 在 Dispose 后抛 <see cref="ObjectDisposedException"/>，
    /// 而原文的 FSize 只是普通字段（析构后仍可读）。为保真，这里返回缓存的 FSize，
    /// 不继承基类的"已释放"检查。
    /// </para>
    /// </summary>
    public override long Length => FSize;

    /// <summary>
    /// 原 `TStream.Position` 读侧 = FPosition；写侧 = `Seek(Value, soBeginning)`（**不做越界校验**）。
    /// .NET 基类写侧会对超出 Length 的值做校验并可能抛异常，故这里整体覆写以保原文语义。
    /// </summary>
    public override long Position
    {
        get => FPosition;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanSeek => true;
    public override void Flush() { }

    /// <summary>原 :548-552 `destructor Destroy` —— SetSize(0) 把块归还池。</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing) SetLength(0);                                       // 原 :550
        base.Dispose(disposing);                                           // 原 :551 inherited
    }

    /// <summary>原 :537-540 `procedure Clear`（SetSize(0)）。</summary>
    public void Clear() => SetLength(0);

    /// <summary>
    /// 原 :1040-1114 `procedure SetSize(NewSize: Integer)`。
    /// `FCapacity div MPool.FBlockSize` 是**旧的块数**（Delphi `div` 对非负数即整除），
    /// 与按 NewSize 算出的新块数比较 → 多了回收（从 FLast 往前 FreeMemoryBlock）、
    /// 少了从池里 GetMemoryBlock 挂到尾部。仅在块数**真的变了**时才更新 FCapacity。
    /// <para>NewSize = 0 时清空所有指针（原 :1093-1102）；否则若 FCurBlock 为 nil 则指向 FHead。</para>
    /// </summary>
    public override void SetLength(long value)
    {
        int NewSize = (int)value;                                          // 原签名是 Longint
        if (FSize != NewSize)                                              // 原 :1046
        {
            FSize = NewSize;                                               // 原 :1048
            TDxMemoryPool MPool = GetPool(FMemBlockType);                  // 原 :1049-1057
            int CurCount = FCapacity / MPool.BlockSize;                    // 原 :1058
            FMemBlockCount = NewSize / MPool.BlockSize;                    // 原 :1059
            if (NewSize % MPool.BlockSize != 0)                            // 原 :1060-1061
                FMemBlockCount++;
            if (CurCount != FMemBlockCount)                                // 原 :1062
            {
                while (CurCount > FMemBlockCount)                          // 原 :1064-1070 内存回收
                {
                    TMemoryBlock mBlock = FLast;                           // 原 :1066
                    FLast = FLast.PrevEx;                                  // 原 :1067
                    MPool.FreeMemoryBlock(mBlock);                         // 原 :1068
                    CurCount--;                                            // 原 :1069
                }
                while (CurCount < FMemBlockCount)                          // 原 :1071-1090
                {
                    TMemoryBlock mBlock = MPool.GetMemoryBlock();          // 原 :1073
                    mBlock.NextEx = null;                                  // 原 :1074
                    mBlock.PrevEx = null;                                  // 原 :1075
                    if (FHead == null)                                     // 原 :1076-1082
                    {
                        FHead = mBlock;
                        FLast = mBlock;
                        FHead.NextEx = null;
                        FHead.PrevEx = null;
                    }
                    else                                                   // 原 :1083-1088
                    {
                        FLast.NextEx = mBlock;
                        mBlock.PrevEx = FLast;
                        FLast = mBlock;
                    }
                    CurCount++;                                            // 原 :1089
                }
                FCapacity = MPool.BlockSize * FMemBlockCount;              // 原 :1091
                //（注意：内存流的末块 NextEx 保持 nil —— 不环回，与 TDxRingStream 相反）
            }
            if (NewSize == 0)                                              // 原 :1093-1102
            {
                FPosition = 0;
                FCurBlock = null;
                FHead = null;
                FLast = null;
                FMarkBlokPos = 0;
                FMarkBlock = null;
                FCurBlockPos = 0;
            }
            else                                                           // 原 :1103-1112
            {
                if (FPosition > NewSize)                                   // 原 :1105-1106
                    Position = NewSize;                                    //   Position := NewSize（走 Seek）
                if (FCurBlock == null)                                     // 原 :1107-1111
                {
                    FCurBlock = FHead;
                    FCurBlockPos = 0;
                }
            }
        }
    }

    /// <summary>
    /// 原 :904-1029 `function Seek(Offset: Integer; Origin: Word): Longint`。
    /// 返回**新的 FPosition**（不是新的块内位置）。
    /// <para>
    /// ★ D1：`soEnd` 分支（原 :987-1023）里 `if Offset &lt; 0 then Offset := -offset;`
    /// 会把负偏移**取反成正数**（原文如此），随后 `Offset &lt; FSize` 才走"从末尾倒数"路径，
    /// 否则回到位置 0。因此 `Seek(0, soEnd)` 与 `Seek(-0, soEnd)` 都得到 0，而**不是 FSize**。
    /// </para>
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin)
    {
        int Offset = (int)offset;
        if (FHead != null)                                                 // 原 :910
        {
            TDxMemoryPool MPool = GetPool(FMemBlockType);                  // 原 :912-920
            switch (origin)                                                // 原 :921 `case TSeekOrigin(Origin)`
            {
                case SeekOrigin.Begin:                                     // 原 :922-945 soBeginning
                    {
                        if (Offset < 0)                                    // 原 :924-925
                            Offset = 0;
                        if (Offset <= FSize)                               // 原 :926
                        {
                            FCurBlockPos = Offset % MPool.BlockSize;       // 原 :928
                            int BIndex = Offset / MPool.BlockSize;         // 原 :929
                            FCurBlock = FHead;                             // 原 :930
                            FPosition = FCurBlockPos;                      // 原 :931
                            if (BIndex > 0)                                // 原 :932-937
                                do
                                {
                                    FPosition += MPool.BlockSize;          // 原 :934
                                    BIndex--;                              // 原 :935
                                    FCurBlock = FCurBlock.NextEx;          // 原 :936
                                } while (BIndex != 0 && FCurBlock != null);
                        }
                        else                                               // 原 :939-944
                        {
                            FCurBlock = FLast;
                            FCurBlockPos = FCapacity - FSize;
                            FPosition = FSize;                             // 原 :943 `FPosition := Size`（即 FSize）
                        }
                        break;
                    }
                case SeekOrigin.Current:                                   // 原 :946-986 soCurrent
                    {
                        if (Offset > 0)                                    // 原 :948
                        {
                            int BIndex = (FCurBlockPos + Offset) / MPool.BlockSize;   // 原 :950
                            FPosition += MPool.BlockSize - FCurBlockPos;   // 原 :951
                            FCurBlockPos = (FCurBlockPos + Offset) % MPool.BlockSize; // 原 :952
                            FPosition += FCurBlockPos;                     // 原 :953
                            while (BIndex > 0)                             // 原 :954-960
                            {
                                FCurBlock = FCurBlock.NextEx;              // 原 :956
                                BIndex--;                                  // 原 :957
                                if (BIndex > 0)                            // 原 :958-959
                                    FPosition += MPool.BlockSize;
                            }
                        }
                        else                                               // 原 :962
                        {
                            if (Offset + FCurBlockPos >= 0)                // 原 :964
                            {
                                FPosition += Offset;                       // 原 :966
                                FCurBlockPos += Offset;                    // 原 :967
                            }
                            else                                           // 原 :969-984
                            {
                                FCurBlock = FCurBlock.PrevEx;              // 原 :971
                                if (FCurBlock == null)                     // 原 :972-976
                                {
                                    FCurBlockPos = 0;
                                    FCurBlock = FHead;
                                }
                                else                                       // 原 :977-983
                                {
                                    Offset += FCurBlockPos;                // 原 :979
                                    FPosition -= FCurBlockPos;             // 原 :980
                                    FCurBlockPos = MPool.BlockSize;        // 原 :981
                                    Seek(Offset, SeekOrigin.Current);      // 原 :982 递归（原文 `Seek(Offset, soCurrent)`）
                                }
                            }
                        }
                        break;
                    }
                case SeekOrigin.End:                                       // 原 :987-1023 soEnd
                    {
                        if (Offset < 0)                                    // 原 :989-990 ★ 取反
                            Offset = -Offset;
                        if (Offset < FSize)                                // 原 :991
                        {
                            int FLastSize = MPool.BlockSize - FCapacity + FSize;   // 原 :993 最后一个块的有效长度
                            if (Offset <= FLastSize)                       // 原 :994-999
                            {
                                FCurBlockPos = FLastSize - Offset;
                                FCurBlock = FLast;
                                FPosition = FSize - Offset;
                            }
                            else                                           // 原 :1000-1015
                            {
                                FCurBlock = FLast.PrevEx;                  // 原 :1002
                                Offset -= FLastSize;                       // 原 :1003
                                FPosition = FSize - FLastSize;             // 原 :1004
                                int BIndex = Offset / MPool.BlockSize;     // 原 :1005
                                Offset = Offset % MPool.BlockSize;         // 原 :1006
                                FCurBlockPos = MPool.BlockSize - Offset;   // 原 :1007
                                FPosition -= MPool.BlockSize - FCurBlockPos;   // 原 :1008
                                while (BIndex > 0)                         // 原 :1009-1014
                                {
                                    FCurBlock = FCurBlock.PrevEx;          // 原 :1011
                                    BIndex--;                              // 原 :1012
                                    FPosition -= MPool.BlockSize;          // 原 :1013
                                }
                            }
                        }
                        else                                           // 原 :1017-1022 ★ D1
                        {
                            FPosition = 0;
                            FCurBlockPos = 0;
                            FCurBlock = FHead;
                        }
                        break;
                    }
            }
            return FPosition;                                              // 原 :1025
        }
        return 0;                                                          // 原 :1027-1028
    }

    /// <summary>
    /// 原 :701-787 `function Read(var Buffer; Count: Longint): Longint`。
    /// <para>
    /// 流程：① FCurBlock 为 nil → 0；② `FPosition + Count > FSize` 则截断 Count；
    /// ③ Count = 0 → 0；④ 若 `FCurBlockPos = FBlockSize` 先跨到下一块；
    /// ⑤ 先吃当前块剩余（`Count &lt;= FBlockSize - FCurBlockPos`），否则吃满当前块并跨块；
    /// ⑥ 剩余按"整块 / 不满一块"循环搬运。
    /// </para>
    /// <para>返回值是**截断后的 Count**（原 :737 `Result := Count`，写在搬运之前）。</para>
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (FCurBlock == null)                                             // 原 :707-708
            return 0;

        TDxMemoryPool MPool = GetPool(FMemBlockType);                      // 原 :711-719
        if (FPosition + count > FSize)                                     // 原 :720-721
            count = FSize - FPosition;
        if (count == 0)                                                    // 原 :722-726
            return 0;
        if (FCurBlockPos == MPool.BlockSize)                               // 原 :727-731
        {
            FCurBlock = FCurBlock.NextEx;
            FCurBlockPos = 0;
        }
        if (FCurBlock == null)                                             // 原 :732-736
            return 0;

        int Result = count;                                                // 原 :737
        int p = offset;                                                    // 原 :738 `p := @Buffer`
        int pBuf = FCurBlock.Offset(FCurBlockPos);                         // 原 :740 本块内起始偏移
        if (count <= MPool.BlockSize - FCurBlockPos)                       // 原 :741 足够写了
        {
            BufferHelper.Copy(FCurBlock.Memory, pBuf, buffer, p, count);   // 原 :743 Move(PBuf^, buffer, Count)
            FPosition += count;                                            // 原 :744
            FCurBlockPos += count;                                         // 原 :745
            count = 0;                                                     // 原 :746
        }
        else                                                               // 原 :748-756
        {
            int n = MPool.BlockSize - FCurBlockPos;
            BufferHelper.Copy(FCurBlock.Memory, pBuf, buffer, p, n);       // 原 :750
            count -= n;                                                    // 原 :751
            FPosition += n;                                                // 原 :752
            p += n;                                                        // 原 :753
            FCurBlock = FCurBlock.NextEx;                                  // 原 :754
            FCurBlockPos = 0;                                              // 原 :755
        }
        if (FCurBlockPos == MPool.BlockSize)                               // 原 :757-761
        {
            FCurBlock = FCurBlock.NextEx;
            FCurBlockPos = 0;
        }
        while (count > 0)                                                  // 原 :762-785
        {
            if (count > MPool.BlockSize)                                   // 原 :764-771
            {
                BufferHelper.Copy(FCurBlock.Memory, 0, buffer, p, MPool.BlockSize);   // 原 :766
                count -= MPool.BlockSize;
                FCurBlockPos = MPool.BlockSize;
                FPosition += MPool.BlockSize;
                p += MPool.BlockSize;
            }
            else                                                           // 原 :772-779
            {
                BufferHelper.Copy(FCurBlock.Memory, 0, buffer, p, count);  // 原 :774
                FPosition += count;
                p += count;
                FCurBlockPos = count;
                count = 0;
            }
            if (FCurBlockPos == MPool.BlockSize)                           // 原 :780-784
            {
                FCurBlock = FCurBlock.NextEx;
                FCurBlockPos = 0;
            }
        }
        return Result;
    }

    /// <summary>`TStream.Read(buffer, count)` 便捷重载。</summary>
    public int Read(byte[] buffer, int count) => Read(buffer, 0, count);

    /// <summary>
    /// 原 :1154-1242 `function Write(const Buffer; Count: Longint): Longint`。
    /// <para>
    /// ★ D3：只有 `FCurBlock = nil`（首次）或 `FCurBlock = FLast`（写末块）时才可能扩容：
    /// 末块内 `FCurBlockPos + Count &gt; FBlockSize` → `SetSize(FSize + Count)`；
    /// 否则 `FPosition + Count &gt; FSize` → `Inc(FSize, Count)`（原地扩）。
    /// 中间块覆盖写**不改 FSize**。
    /// </para>
    /// <para>
    /// 尾部（原 :1237-1241）：若搬完后 `FCurBlock = nil`（恰好写满最后一块并把游标跨出去），
    /// 则把游标复位到 `FLast` + `FBlockSize`，即"下一次写会先跨块"。
    /// </para>
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        TDxMemoryPool MPool = GetPool(FMemBlockType);                      // 原 :1160-1168
        if (FCurBlock == null)                                             // 原 :1169-1170
            SetLength(count);                                              //   SetSize(Count)
        else if (ReferenceEquals(FCurBlock, FLast))                        // 原 :1171-1177
        {
            if (FCurBlockPos + count > MPool.BlockSize)                    // 原 :1173
                SetLength(FSize + count);                                  //   SetSize(FSize + Count)
            else if (FPosition + count > FSize)                            // 原 :1175
                FSize += count;                                            //   Inc(FSize, Count)
        }
        if (FCurBlockPos == MPool.BlockSize)                               // 原 :1178-1182
        {
            FCurBlock = FCurBlock.NextEx;
            FCurBlockPos = 0;
        }
        // ★ 托管侧安全等价（有意偏差）：原文此处若 FCurBlock 为 nil 会在 :1186 解引用 nil（AV）。
        //   触发条件：FSize > 0 但 FCurBlock = nil —— 例如写满整块后 :1230 把 FCurBlock 跨成 nil，
        //   或 WriteStream 的末尾（原码**没有** Write 的 :1237-1241 复位）之后紧跟 count = 0 的 Write。
        //   本移植返回"什么都不写"（与 count = 0 分支同结果），使该路径可测而非崩溃。
        if (FCurBlock == null) return;
        int Result = count;                                                // 原 :1183
        int t = offset;                                                    // 原 :1184 `tmpBuf := @Buffer`
        int pBuf = FCurBlock.Offset(FCurBlockPos);                         // 原 :1186
        if (count <= MPool.BlockSize - FCurBlockPos)                       // 原 :1187 足够写了
        {
            BufferHelper.Copy(buffer, t, FCurBlock.Memory, pBuf, count);   // 原 :1189 Move(Buffer, pBuf^, Count)
            FCurBlockPos += count;                                         // 原 :1190
            FPosition += count;                                            // 原 :1191
            count = 0;                                                     // 原 :1192
        }
        else                                                               // 原 :1194-1207
        {
            int n = MPool.BlockSize - FCurBlockPos;
            BufferHelper.Copy(buffer, t, FCurBlock.Memory, pBuf, n);       // 原 :1196
            count -= n;                                                    // 原 :1197
            t += n;                                                        // 原 :1198
            FPosition += n;                                                // 原 :1199
            FCurBlock = FCurBlock.NextEx;                                  // 原 :1200
            FCurBlockPos = 0;                                              // 原 :1201
            if (ReferenceEquals(FCurBlock, FLast))                         // 原 :1202-1206
            {
                if (FPosition + count > FSize)
                    FSize = FPosition + count;
            }
        }
        while (count > 0)                                                  // 原 :1208-1236
        {
            if (count > MPool.BlockSize)                                   // 原 :1210-1217
            {
                BufferHelper.Copy(buffer, t, FCurBlock.Memory, 0, MPool.BlockSize);   // 原 :1212
                t += MPool.BlockSize;
                count -= MPool.BlockSize;
                FCurBlockPos = MPool.BlockSize;
                FPosition += MPool.BlockSize;
            }
            else                                                           // 原 :1218-1225
            {
                BufferHelper.Copy(buffer, t, FCurBlock.Memory, 0, count);  // 原 :1220
                FPosition += count;
                t += count;
                FCurBlockPos = count;
                count = 0;
            }
            if (FCurBlockPos == MPool.BlockSize)                           // 原 :1226-1235
            {
                FCurBlock = FCurBlock.NextEx;
                FCurBlockPos = 0;
                if (ReferenceEquals(FCurBlock, FLast))
                {
                    if (FPosition + count > FSize)
                        FSize = FPosition + count;
                }
            }
        }
        if (FCurBlock == null)                                             // 原 :1237-1241
        {
            FCurBlockPos = MPool.BlockSize;
            FCurBlock = FLast;
        }
    }

    /// <summary>
    /// `TStream.Write(buffer, count)` 便捷重载；返回写入的字节数（Delphi 版 <c>Result := Count</c>，
    /// 恒等于入参 Count —— 见原 :1183）。
    /// </summary>
    public int Write(byte[] buffer, int count) { Write(buffer, 0, count); return count; }

    /// <summary>原 :789-847 `procedure ReadStream(Stream: TStream; len: Integer)` —— 本流 → 目标流。</summary>
    public void ReadStream(Stream Stream, int len)
    {
        if (FCurBlock == null) return;                                     // 原 :794
        TDxMemoryPool MPool = GetPool(FMemBlockType);                      // 原 :796-804
        if (FPosition + len > FSize)                                       // 原 :805-806
            len = FSize - FPosition;
        int pBuf = FCurBlock.Offset(FCurBlockPos);                         // 原 :808
        if (len <= MPool.BlockSize - FCurBlockPos)                         // 原 :809 足够写了
        {
            Stream.Write(FCurBlock.Memory, pBuf, len);                     // 原 :811 WriteBuffer(PBuf^, Len)
            FPosition += len;                                              // 原 :812
            FCurBlockPos += len;                                           // 原 :813
            len = 0;                                                       // 原 :814
        }
        else                                                               // 原 :816-823
        {
            int n = MPool.BlockSize - FCurBlockPos;
            Stream.Write(FCurBlock.Memory, pBuf, n);                       // 原 :818
            len -= n;                                                      // 原 :819
            FPosition += n;                                                // 原 :820
            FCurBlock = FCurBlock.NextEx;                                  // 原 :821
            FCurBlockPos = 0;                                              // 原 :822
        }
        while (len > 0)                                                    // 原 :824-845
        {
            if (len > MPool.BlockSize)                                     // 原 :826-832
            {
                Stream.Write(FCurBlock.Memory, 0, MPool.BlockSize);        // 原 :828
                len -= MPool.BlockSize;
                FCurBlockPos = MPool.BlockSize;
                FPosition += MPool.BlockSize;
            }
            else                                                           // 原 :833-839
            {
                Stream.Write(FCurBlock.Memory, 0, len);                    // 原 :835
                FPosition += len;
                FCurBlockPos = len;
                len = 0;
            }
            if (FCurBlockPos == MPool.BlockSize)                           // 原 :840-844
            {
                FCurBlock = FCurBlock.NextEx;
                FCurBlockPos = 0;
            }
        }
    }

    /// <summary>
    /// 原 :1244-1316 `procedure WriteStream(Stream: TStream; Len: Integer)` —— 目标流 → 本流。
    /// <para>★ D4：扩容量 `MPool.FBlockSize - FCurBlockPos - Len + FSize`（原 :1263）。</para>
    /// <para>注意原文**没有**末尾的 `if FCurBlock = nil` 复位（与 <see cref="Write"/> 不同），
    /// 所以正好写满最后一块后 `FCurBlock` 会是 nil —— 下一次调用会在原 :1268 解引用 nil。</para>
    /// </summary>
    public void WriteStream(Stream Stream, int Len)
    {
        TDxMemoryPool MPool = GetPool(FMemBlockType);                      // 原 :1249-1257
        if (FCurBlock == null)                                             // 原 :1258-1259
            SetLength(Len);
        else if (ReferenceEquals(FCurBlock, FLast))                        // 原 :1260-1266
        {
            if (FCurBlockPos + Len > MPool.BlockSize)                      // 原 :1262
                SetLength(MPool.BlockSize - FCurBlockPos - Len + FSize);   // 原 :1263 ★ D4
            else if (FPosition + Len > FSize)                              // 原 :1264
                FSize += Len;                                              // 原 :1265
        }
        // ★ 托管侧安全等价（有意偏差，同 Write 的同类守卫）：
        //   原文在 :1268 直接解引用 FCurBlock。`FCurBlock = nil` 有两种来源：
        //   ① 前一次 WriteStream **不带** Write 的 :1237-1241 复位，写满恰好一整块后游标跨成 nil；
        //   ② 前一次 Write 的 `nil + FBlockSize` 哨兵被 Seek 改写后（见 Write 的说明）。
        //   两种情况原文都是 AV；本移植选择"什么都不读"（Len 保持入参，不改变任何字段）。
        if (FCurBlock == null) return;
        int pBuf = FCurBlock.Offset(FCurBlockPos);                         // 原 :1268
        if (Len <= MPool.BlockSize - FCurBlockPos)                         // 原 :1269
        {
            ReadExactly(Stream, FCurBlock.Memory, pBuf, Len);              // 原 :1271 ReadBuffer(PBuf^, Len)
            FCurBlockPos += Len;                                           // 原 :1272
            FPosition += Len;                                              // 原 :1273
            Len = 0;                                                       // 原 :1274
        }
        else                                                               // 原 :1276-1288
        {
            int n = MPool.BlockSize - FCurBlockPos;
            ReadExactly(Stream, FCurBlock.Memory, pBuf, n);                // 原 :1278
            Len -= n;                                                      // 原 :1279
            FPosition += n;                                                // 原 :1280
            FCurBlock = FCurBlock.NextEx;                                  // 原 :1281
            FCurBlockPos = 0;                                              // 原 :1282
            if (ReferenceEquals(FCurBlock, FLast))                         // 原 :1283-1287
            {
                if (FPosition + Len > FSize)
                    FSize = FPosition + Len;
            }
        }
        while (Len > 0)                                                    // 原 :1289-1315
        {
            // ★ 托管侧安全等价（同 Write 的同类守卫）：跨块后 FCurBlock 可能为 nil，
            //   原文在 :1293/:1300 解引用它（AV）。此时本次 WriteStream 到此为止。
            if (FCurBlock == null) return;
            if (Len > MPool.BlockSize)                                     // 原 :1291-1297
            {
                ReadExactly(Stream, FCurBlock.Memory, 0, MPool.BlockSize); // 原 :1293
                Len -= MPool.BlockSize;
                FCurBlockPos = MPool.BlockSize;
                FPosition += MPool.BlockSize;
            }
            else                                                           // 原 :1299-1304
            {
                ReadExactly(Stream, FCurBlock.Memory, 0, Len);             // 原 :1300
                FPosition += Len;
                FCurBlockPos = Len;
                Len = 0;
            }
            if (FCurBlockPos == MPool.BlockSize)                           // 原 :1305-1314
            {
                FCurBlock = FCurBlock.NextEx;
                FCurBlockPos = 0;
                if (ReferenceEquals(FCurBlock, FLast))
                {
                    if (FPosition + Len > FSize)
                        FSize = FPosition + Len;
                }
            }
        }
    }

    /// <summary>
    /// 原 :876-902 `procedure SaveToStream(Stream: TStream)` —— 只写**有效数据**：
    /// 非末块写满 FBlockSize，末块写 `FBlockSize - FCapacity + FSize`。
    /// 原文要求调用前先 `Seek(0, soBeginning)`/手动设置 Position，否则从 FCurBlock 起写；
    /// 本方法**不受 FCurBlock 影响**（原 :892-900 从 FHead 遍历）。
    /// </summary>
    public void SaveToStream(Stream Stream)
    {
        if (FHead != null)                                                 // 原 :881
        {
            TDxMemoryPool MPool = GetPool(FMemBlockType);                  // 原 :883-891
            TMemoryBlock tmp = FHead;                                      // 原 :892
            while (tmp != null)                                            // 原 :893-900
            {
                if (!ReferenceEquals(tmp, FLast))                          // 原 :895
                    Stream.Write(tmp.Memory, 0, MPool.BlockSize);          // 原 :896 WriteBuffer(Memory^, FBlockSize)
                else
                    Stream.Write(tmp.Memory, 0, MPool.BlockSize - FCapacity + FSize);   // 原 :898
                tmp = tmp.NextEx;                                          // 原 :899
            }
        }
    }

    /// <summary>原 :861-874 `procedure SaveToFile(const FileName: string)`（FHead = nil 时不建文件）。</summary>
    public void SaveToFile(string FileName)
    {
        if (FHead != null)                                                 // 原 :865
        {
            using FileStream FStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);   // 原 :867 fmCreate
            SaveToStream(FStream);                                         // 原 :869
        }
    }

    /// <summary>原 :665-699 `procedure LoadFromStream(Stream: TStream)`（先 Stream.Position := 0，再按块读满）。</summary>
    public void LoadFromStream(Stream Stream)
    {
        Stream.Position = 0;                                               // 原 :671
        SetLength(Stream.Length);                                          // 原 :672 SetSize(Stream.Size)
        if (FSize != 0)                                                    // 原 :673
        {
            TDxMemoryPool MPool = GetPool(FMemBlockType);                  // 原 :675-683
            TMemoryBlock tmpBlock = FHead;                                 // 原 :684
            for (int Windex = 0; Windex <= FMemBlockCount - 1; Windex++)   // 原 :685
            {
                if (Windex == FMemBlockCount - 1)                          // 原 :687-690
                    ReadExactly(Stream, tmpBlock.Memory, 0, MPool.BlockSize - (FCapacity - FSize));
                else
                    ReadExactly(Stream, tmpBlock.Memory, 0, MPool.BlockSize);   // 原 :692
                tmpBlock = tmpBlock.NextEx;                                // 原 :693
            }
            FPosition = 0;                                                 // 原 :695
            FCurBlock = FHead;                                             // 原 :696
            FCurBlockPos = 0;                                              // 原 :697
        }
    }

    /// <summary>原 :653-663 `procedure LoadFromFile(const FileName: string)`。</summary>
    public void LoadFromFile(string FileName)
    {
        using FileStream F = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);   // 原 :657 fmOpenRead or fmShareDenyWrite
        LoadFromStream(F);                                                 // 原 :659
    }

    /// <summary>
    /// 原 :618-651 `procedure LoadFromBufferList(BufList: TBufferLink; const DataLen: Integer)`。
    /// 与 <see cref="LoadFromStream"/> 的唯一差异：数据源是 <see cref="TBufferLink.ReadBuffer"/>，
    /// 且循环里有 `if tmpBlock = nil then Break;` 保护（原 :639-640）。
    /// </summary>
    public void LoadFromBufferList(TBufferLink BufList, int DataLen)
    {
        SetLength(DataLen);                                                // 原 :624
        if (FSize != 0)                                                    // 原 :625
        {
            TDxMemoryPool MPool = GetPool(FMemBlockType);                  // 原 :627-635
            TMemoryBlock tmpBlock = FHead;                                 // 原 :636
            for (int Windex = 0; Windex <= FMemBlockCount - 1; Windex++)   // 原 :637
            {
                if (tmpBlock == null) break;                               // 原 :639-640
                if (Windex == FMemBlockCount - 1)                          // 原 :641-642
                    BufList.ReadBuffer(tmpBlock.Memory, 0, (uint)(MPool.BlockSize - (FCapacity - FSize)));
                else
                    BufList.ReadBuffer(tmpBlock.Memory, 0, (uint)MPool.BlockSize);   // 原 :644
                tmpBlock = tmpBlock.NextEx;                                // 原 :645
            }
            FPosition = 0;                                                 // 原 :647
            FCurBlock = FHead;                                             // 原 :648
            FCurBlockPos = 0;                                              // 原 :649
        }
    }

    /// <summary>
    /// 原 :573-616 `procedure LinkToBufferList(BufList: TBufferLink)` —— 把本流的整条块链
    /// **整体挂到**缓冲链尾部（NextEx/PrevEx 相接），把每块的 DataLen 设为块大小
    /// （末块 = `BSize - (FCapacity - FSize)`，原 :603），然后**清空本流的全部状态**
    /// （原 :604-614，注释写明"连接到连接缓存区，就相当于本流清空"）。
    /// <para>★ 注意原文清空列表**不含 `FMemBlockType`**。</para>
    /// </summary>
    public void LinkToBufferList(TBufferLink BufList)
    {
        if (FHead == null) return;                                         // 原 :578
        if (BufList.HeadBlock == null)                                     // 原 :580-581
            BufList.SetHeadBlock(FHead);
        else                                                               // 原 :582-586
        {
            FHead.PrevEx = BufList.LastBlock;                              // 原 :584
            BufList.LastBlock.NextEx = FHead;                              // 原 :585
        }
        BufList.SetLastBlock(FLast);                                       // 原 :587
        TMemoryBlock tmp = FHead;                                          // 原 :588
        int BSize;                                                         // 原 :576
        switch (FMemBlockType)                                             // 原 :589-597
        {
            case TDxMemBlockType.MB_Small: BSize = MemoryPoolGlobal.SmallMemoryPool().BlockSize; break;
            case TDxMemBlockType.MB_Normal: BSize = MemoryPoolGlobal.MemoryPool().BlockSize; break;
            case TDxMemBlockType.MB_SpBig: BSize = MemoryPoolGlobal.SuperMemoryPool().BlockSize; break;
            case TDxMemBlockType.MB_Large: BSize = MemoryPoolGlobal.LargeMemoryPool().BlockSize; break;
            case TDxMemBlockType.MB_SPLarge: BSize = MemoryPoolGlobal.SuperLargeMemoryPool().BlockSize; break;
            default: BSize = MemoryPoolGlobal.BigMemoryPool().BlockSize; break;   // 原 :595-596
        }
        while (!ReferenceEquals(tmp, FLast))                               // 原 :598-602
        {
            tmp.DataLen = (ushort)BSize;                                   // 原 :600
            tmp = tmp.NextEx;
        }
        FLast.DataLen = (ushort)(BSize - (FCapacity - FSize));             // 原 :603
        // 原 :604-614：相当于本流清空
        FHead = null;
        FLast = null;
        FMarkBlokPos = 0;
        FPosition = 0;
        FMarkBlock = null;
        FSize = 0;
        FCapacity = 0;
        FMemBlockCount = 0;
        FCurBlockPos = 0;
        FCurBlock = null;
    }

    /// <summary>
    /// 原 :1116-1152 `procedure SwapStreamLink(Stream: TDxMemoryStream)` —— 交换两个流的块链
    /// 与记账字段。★ D6：`FSize` 的交换**不是对称的**（见文件头说明），本移植逐句照抄。
    /// </summary>
    public void SwapStreamLink(TDxMemoryStream Stream)
    {
        FMarkBlokPos = 0;                                                  // 原 :1123
        FMarkBlock = null;                                                 // 原 :1124
        int OldBlockCount = FMemBlockCount;                                // 原 :1125
        FMemBlockCount = Stream.FMemBlockCount;                            // 原 :1126
        TDxMemBlockType OldMtype = FMemBlockType;                          // 原 :1127
        FMemBlockType = Stream.FMemBlockType;                              // 原 :1128
        int OldFCapacity = FCapacity;                                      // 原 :1129
        FCapacity = Stream.FCapacity;                                      // 原 :1130
        TMemoryBlock OldHead = FHead;                                      // 原 :1131
        FHead = Stream.FHead;                                              // 原 :1132
        TMemoryBlock OldLast = FLast;                                      // 原 :1133
        FLast = Stream.FLast;                                              // 原 :1134
        int OldSize = FSize;                                               // 原 :1135
        FSize = (int)Stream.Length;                                        // 原 :1136 `FSize := Stream.Size`
        FCurBlock = null;                                                  // 原 :1137
        FCurBlockPos = 0;                                                  // 原 :1138
        FPosition = 0;                                                     // 原 :1139
        Stream.FHead = OldHead;                                            // 原 :1140
        Stream.FLast = OldLast;                                            // 原 :1141
        Stream.FMarkBlock = null;                                          // 原 :1142
        Stream.FMarkBlokPos = 0;                                           // 原 :1143
        Stream.FMarkBlock = null;                                          // 原 :1144（原文重复，保留）
        Stream.FCurBlock = null;                                           // 原 :1145
        Stream.FCurBlockPos = 0;                                           // 原 :1146
        Stream.FMemBlockCount = OldBlockCount;                             // 原 :1147
        Stream.FCapacity = OldFCapacity;                                   // 原 :1148
        Stream.FPosition = 0;                                              // 原 :1149
        Stream.FSize = OldSize;                                            // 原 :1150
        Stream.FMemBlockType = OldMtype;                                   // 原 :1151
    }

    /// <summary>原 :849-853 `procedure RestoreCurBlock`。</summary>
    public void RestoreCurBlock()
    {
        FCurBlock = FMarkBlock;
        FCurBlockPos = FMarkBlokPos;
    }

    /// <summary>原 :855-859 `procedure SaveCurBlock`。</summary>
    public void SaveCurBlock()
    {
        FMarkBlock = FCurBlock;
        FMarkBlokPos = FCurBlockPos;
    }

    /// <summary>
    /// Delphi `TStream.ReadBuffer`：读不满就抛 `EReadError`。原码在 LoadFromStream 里依赖它。
    /// </summary>
    private static void ReadExactly(Stream s, byte[] buf, int off, int count)
    {
        int done = 0;
        while (done < count)
        {
            int n = s.Read(buf, off + done, count - done);
            if (n <= 0)
                throw new EndOfStreamException("TDxMemoryStream.LoadFromStream: TStream.ReadBuffer 读不满");
            done += n;
        }
    }

    // ---- 原文没有、仅为测试/诊断提供的只读探针 ----
    public int PositionInternal => FPosition;
    public int CapacityInternal => FCapacity;
    public int CurBlockPosInternal => FCurBlockPos;
    public TMemoryBlock CurBlockInternal => FCurBlock;
    public TMemoryBlock MarkBlockInternal => FMarkBlock;
    public int MarkBlokPosInternal => FMarkBlokPos;
}

/// <summary>
/// Delphi `Move(Src, Dst, Len)` 的等价物（逐字节内存拷贝，允许 Src/Dst 同一数组重叠）。
/// <para>
/// 原文用 `Move` 的地方源与目标是**不同**的存贮（一个在池块里、一个在调用方 buffer 里），
/// 但 `TDxMemoryStream.Write` 在"buffer 就是池块内存"时会出现重叠，故用
/// <see cref="Array.Copy(Array, int, Array, int, int)"/>（其重叠处理是定义良好的）。
/// </para>
/// <para>`Len = 0` 时是 no-op（Delphi `Move(..., 0)` 同样什么都不做）。</para>
/// </summary>
public static class BufferHelper
{
    public static void Copy(byte[] src, int srcOff, byte[] dst, int dstOff, int len)
    {
        if (len <= 0) return;
        Array.Copy(src, srcOff, dst, dstOff, len);
    }
}
