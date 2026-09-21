// ============================================================================
// 车道 p9-m2-pathfind —— 切片 1
// 源单元：Source/M2Engine/ClientPickItemsCfg.pas（192 行，镜像 _analysis/utf8_mirror/M2Engine/ClientPickItemsCfg.pas）
// 1:1 托管移植。原文头注释（逐字保留其语义）：
//
//   客户端内挂上传的捡取物品
//   这样搞的原因是减少内存占用 2020-03-08 01:36:31
//   X X 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
//   前面02bit是标记：特殊物品，可捡物品
//   后面14bit是物品数据库的Idx + 1，金币特殊处理Idx = 0
//
// ---------------------------------------------------------------------------
// 【位域语义（从原文 :75 / :87 / :99 / :126-127 / :176 反推，全部经用例锁定）】
//   bit15 ($8000) = 优先捡取（CheckPriorityPickup）
//   bit14 ($4000) = 可捡取    （CheckEnablePickup）
//   低 14 位 ($3FFF) = 物品数据库 Idx + 1（排序键 / 查找键）
//   ⇒ 可捡判定是 **或**（$C000 掩码），而**不是**"两个位都不为 0"。
//
// 【原文缺陷（照抄 + 差异断言锁定）】
//   ① `Sort` 是**不稳定**的原地快排：`CompareItem` 只看低 14 位，故低 14 位相同的两条
//      在分区里会被**无条件互换**（`if I <> J then` 交换，不看 CompareItem 结果）。
//      后果：**同 Idx 的两条记录，谁在前取决于快排的分区轨迹，与输入顺序无关**。
//      用例 `Sort_SwapsEqualKeyEntries_QuicksortPartition` 实测该顺序并锁死。
//   ② `SearchItem` 在命中时写 `H := I - 1; L := I; Result := I;` ⇒ 继续向左收缩，
//      返回的是**重复键中最左**的那一个（首次匹配）。
//   ③ `SearchItem` 的 `I := L + (H - L) shr 1` 依赖 Delphi 的运算符优先级
//      （`shr` 与 `*` 同级、**高于** `+`），即 `L + ((H - L) shr 1)`。
//      C# 的 `>>` **低于** `+`，写成 `L + (H - L) >> 1` 会变成 `(L + (H - L)) >> 1`
//      —— 台账 §35.3 记录过同一坑；本文件显式加括号并加用例锁死。
//   ④ `SetData` 只判 `BufLen mod 2 <> 0`（奇数长度直接丢弃），**不校验 BufLen 是否
//      真的小于缓冲区长度**；原文 `Move(Buf^, FItems[0], BufLen)` 会越界读（AV），
//      托管侧等价地抛 IndexOutOfRangeException。
//   ⑤ 原文 `destructor Destroy` 里的 `FItems := nil` 与 `Clear` 的 `SetLength(FItems,0)`
//      语义相同（都清空），托管侧 `Dispose()` 与 `Clear()` 都落成空数组。
// ============================================================================

using System;

namespace GXX.M2Server.Sweep9.PathFind.Items;

/// <summary>
/// 原文 <c>ClientPickItemsCfg.pas:19-40 TClientPickItems = class(TObject)</c>。
/// <para>Buff 布局：每个物品一个 <see cref="ushort"/>，bit15 = 优先捡取、bit14 = 可捡取、
/// 低 14 位 = 物品 Idx + 1（金币 Idx = 0 特殊处理）。</para>
/// </summary>
public class TClientPickItems : IDisposable
{
    /// <summary>原文 <c>:21 FItems: array of Word</c>（private）。</summary>
    private ushort[] FItems = Array.Empty<ushort>();

    /// <summary>原文 <c>:46-49 constructor TClientPickItems.Create</c>（空体）。</summary>
    public TClientPickItems()
    {
    }

    /// <summary>
    /// 原文 <c>:51-55 destructor TClientPickItems.Destroy</c>（<c>FItems := nil</c>）。
    /// 托管侧无析构时机，故落为显式释放（与原 <c>Clear</c> 同效，见原文缺陷 ⑤）。
    /// </summary>
    public void Dispose()
    {
        FItems = Array.Empty<ushort>();
    }

    /// <summary>原文 <c>:57-60 procedure TClientPickItems.Clear</c>（<c>SetLength(FItems, 0)</c>）。</summary>
    public void Clear()
    {
        FItems = Array.Empty<ushort>();
    }

    /// <summary>原文 <c>:31 property Count: Integer read GetCount</c>（= <c>Length(FItems)</c>）。</summary>
    public int Count => GetCount();

    /// <summary>原文 <c>:62-65 function TClientPickItems.GetCount</c>。</summary>
    private int GetCount()
    {
        return FItems.Length;
    }

    /// <summary>
    /// 原文 <c>:67-77 function TClientPickItems.CheckPriorityPickup(ItemIdx)</c>。
    /// <c>Result := FItems[Index] and $8000 &lt;&gt; 0</c>（Delphi 里 <c>and</c> 是乘法级优先级、
    /// 高于关系运算 <c>&lt;&gt;</c>，故等于 <c>(FItems[Index] and $8000) &lt;&gt; 0</c>）。
    /// </summary>
    public bool CheckPriorityPickup(int ItemIdx)
    {
        bool Result = false;
        int Index = SearchItem(ItemIdx);
        if (Index >= 0)
        {
            Result = (FItems[Index] & 0x8000) != 0;
        }

        return Result;
    }

    /// <summary>原文 <c>:79-89 function TClientPickItems.CheckEnablePickup(ItemIdx)</c>（掩码 <c>$4000</c>）。</summary>
    public bool CheckEnablePickup(int ItemIdx)
    {
        bool Result = false;
        int Index = SearchItem(ItemIdx);
        if (Index >= 0)
        {
            Result = (FItems[Index] & 0x4000) != 0;
        }

        return Result;
    }

    /// <summary>
    /// 原文 <c>:91-101 function TClientPickItems.CheckCanPickItem(ItemIdx)</c>。
    /// 掩码 <c>$C000</c> ⇒ **优先捡 或 可捡**（注释「允许捡或优先捡」）。
    /// </summary>
    public bool CheckCanPickItem(int ItemIdx)
    {
        bool Result = false;
        int Index = SearchItem(ItemIdx);
        if (Index >= 0)
        {
            Result = (FItems[Index] & 0xC000) != 0;
        }

        return Result;
    }

    /// <summary>
    /// 原文 <c>:103-120 procedure TClientPickItems.SetData(Buf: PByte; BufLen: Integer)</c>。
    /// <para>奇数长度直接丢弃（不清空既有数据）；偶数时按 **小端 Word** 逐条搬入并排序。</para>
    /// <para>托管侧 <c>PByte</c> → <c>byte[]</c>；<c>Move(Buf^, FItems[0], BufLen)</c> →
    /// 逐字节小端还原（等价于原文的整块内存拷贝）。</para>
    /// </summary>
    public void SetData(byte[] Buf, int BufLen)
    {
        if (BufLen % 2 != 0)
        {
            return;
        }

        int Count = BufLen / 2;
        // 原文 SetLength(FItems, Count)：负数会抛范围错误，托管侧同样抛（未做额外守卫）。
        FItems = new ushort[Count];
        if (Count > 0)
        {
            for (int i = 0; i < Count; i++)
            {
                // 原文 Move 的逐字节等价：低字节在前（Windows/Delphi 小端）。
                FItems[i] = (ushort)(Buf[i * 2] | (Buf[i * 2 + 1] << 8));
            }

            Sort(0, Count - 1);
        }
    }

    /// <summary>
    /// 原文 <c>:122-129 function TClientPickItems.CompareItem(Index1, Index2)</c>。
    /// 只比低 14 位 ⇒ **忽略两个标志位**。
    /// </summary>
    private int CompareItem(int Index1, int Index2)
    {
        int V1 = FItems[Index1] & 0x3FFF;
        int V2 = FItems[Index2] & 0x3FFF;
        return V1 - V2;
    }

    /// <summary>
    /// 原文 <c>:131-163 procedure TClientPickItems.Sort(L, R: Integer)</c>。
    /// <para>原地快排 + 显式枢轴下标跟踪（<c>P</c> 会随后续交换改写）。
    /// 外层是 <c>repeat ... until I &gt;= R</c>，左侧递归、右侧改 <c>L</c> 继续循环
    /// （尾递归手工消除）——逐行保留。</para>
    /// </summary>
    public void Sort(int L, int R)
    {
        int I, J, P;
        ushort TempValue;
        do
        {
            I = L;
            J = R;
            // 原文 `(L + R) shr 1`：Delphi 的 shr 高于 +，C# 的 >> 低于 +，故必须整体加括号。
            P = (L + R) >> 1;
            do
            {
                while (CompareItem(I, P) < 0)
                {
                    I++;
                }

                while (CompareItem(J, P) > 0)
                {
                    J--;
                }

                if (I <= J)
                {
                    if (I != J)
                    {
                        TempValue = FItems[I];
                        FItems[I] = FItems[J];
                        FItems[J] = TempValue;
                    }

                    // 原文如此：枢轴下标跟着换位走（相等键也会被交换，A 处不稳性的来源）。
                    if (P == I)
                    {
                        P = J;
                    }
                    else if (P == J)
                    {
                        P = I;
                    }

                    I++;
                    J--;
                }
            }
            while (I <= J);

            if (L < J)
            {
                Sort(L, J);
            }

            L = I;
        }
        while (I < R);
    }

    /// <summary>
    /// 原文 <c>:165-190 function TClientPickItems.SearchItem(ItemIdx): Integer</c>。
    /// <para>二分查找（要求已排序）；未命中返回 <c>-1</c>。</para>
    /// <para><b>命中即终止</b>：命中分支先 <c>H := I - 1</c>、后 <c>L := I</c>，
    /// 于是循环条件 <c>L &lt;= H</c> 立刻变成 <c>I &lt;= I - 1</c>（假）⇒ 直接退出，
    /// <b>不会继续向左找重复键</b>。因此低 14 位相同的多条记录里，
    /// 返回的是"标准对半收敛所落到的那个下标"，而不是最左或最右 —— 这是原文行为，
    /// 由用例 <c>SearchItem_ReturnsBinarySearchLandingIndex_ForDuplicateKeys</c> 锁死。</para>
    /// </summary>
    private int SearchItem(int ItemIdx)
    {
        int Result = -1;
        int L = 0;
        int H = FItems.Length - 1;
        while (L <= H)
        {
            // 原文 `L + (H - L) shr 1` = L + ((H - L) shr 1)（同 Sort 的优先级说明）。
            int I = L + ((H - L) >> 1);
            int CurItem = FItems[I] & 0x3FFF;
            int C = CurItem - ItemIdx;
            if (C < 0)
            {
                L = I + 1;
            }
            else
            {
                H = I - 1;
                if (C == 0)
                {
                    L = I;
                    Result = I;
                }
            }
        }

        return Result;
    }
}
