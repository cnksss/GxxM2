// 源单元：Source/Client-HGE/uDropItemEffectList.pas（原文 160 行 / CRLF 计入 182 行）
// 原文 uses：Windows, SysUtils, Classes, Grobal2
// 原文无同名 .dfm（纯集合类，无窗体）。
//
// 移植策略：
//   · 原文是"按 ItemEffectIndex 去重 + 可排序 + 二分查找"的 TList 包装
//     （TList 存 PDropItemEffect 指针；Add 命中已存在项时**覆盖**其内容）；
//   · C# 侧用 List<TDropItemEffect>（结构体值语义）承载，指针语义由"取引用后整体赋值"表达；
//   · 原文 QuickSort 是**教科书 Hoare 分区 + 尾递归消除**（StringHashMap 那套之外的独立实现），
//     逐行保留（含 P 轴元素随交换而移动的细节）。
using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Tail;

/// <summary>
/// uDropItemEffectList.pas 的 <c>TDropItemEffectList</c> 1:1 移植。
///
/// <para><b>行为要点</b>：</para>
/// <list type="bullet">
/// <item><c>Add</c>：按 <c>ItemEffectIndex</c> 查重；**不存在**则追加新项并把入参整体拷进去，
/// **已存在**则把入参整体覆盖到已有项上（返回的始终是该索引对应的那一项）。</item>
/// <item>任何结构性改动（Add 新项 / Clear）都会把 <c>FIsSorted</c> 置回 False，
/// 于是 <c>IndexOf</c> 退回**线性**查找；只有显式 <c>Sort</c> 之后才走二分。</item>
/// <item><c>Items[Index]</c> 越界返回 nil（原文如此），而 <c>FList.Items[Index]</c> 在 C# 侧越界会抛
/// —— 本移植按原文语义返回 <c>null</c>（差异已登记，见报告）。</item>
/// </list>
/// </summary>
public class TDropItemEffectList
{
    // 原文 :14-17
    private readonly List<TDropItemEffect> FList = new List<TDropItemEffect>();
    private bool FIsSorted = false;

    /// <summary>原文 :93-96 <c>GetCount</c></summary>
    public int Count => FList.Count;

    /// <summary>
    /// 原文 :98-104 <c>GetItems(Index)</c>：越界返回 nil（托管侧以 <c>null</c> 表达，
    /// 因为 <c>TDropItemEffect</c> 是值类型，用 <c>HasValue</c> 的空结构承载指针语义）。
    /// </summary>
    public TDropItemEffect? this[int Index]
    {
        get
        {
            if (Index < 0 || Index >= FList.Count) return null;
            return FList[Index];
        }
    }

    /// <summary>原文 :25 — <c>property Count:Integer read GetCount;</c></summary>
    public bool IsSorted => FIsSorted;

    /// <summary>
    /// 原文 :53-67 <c>function Add(DropItemEffect: PDropItemEffect): PDropItemEffect;</c>
    /// </summary>
    /// <returns>该 <c>ItemEffectIndex</c> 对应的列表项（原文返回的是列表内的指针）。</returns>
    public TDropItemEffect Add(TDropItemEffect DropItemEffect)
    {
        int Index = IndexOf(DropItemEffect.ItemEffectIndex);
        if (Index < 0)
        {
            // 原文 :59-61：New(Result); FList.Add(Result); FIsSorted := False;
            FList.Add(DropItemEffect);
            FIsSorted = false;
            Index = FList.Count - 1;
        }
        else
        {
            // 原文 :64：Result := FList.Items[Index];（随后 Result^ := DropItemEffect^ 覆盖内容）
        }

        // 原文如此（uDropItemEffectList.pas:66）：Result^ := DropItemEffect^;
        // —— 命中已存在项时这里会把入参**整体覆盖**到已有项上。
        FList[Index] = DropItemEffect;
        return FList[Index];
    }

    /// <summary>原文 :69-80 <c>Clear</c>：释放全部项（托管侧由 GC 承担）并把 FIsSorted 置 False。</summary>
    public void Clear()
    {
        FList.Clear();
        FIsSorted = false;
    }

    /// <summary>原文 :82-91 <c>Get(ItemEffectIndex)</c>：未命中返回 nil。</summary>
    public TDropItemEffect? Get(int ItemEffectIndex)
    {
        int Index = IndexOf(ItemEffectIndex);
        if (Index < 0) return null;
        return FList[Index];
    }

    /// <summary>
    /// 原文 :106-141 <c>IndexOf(ItemEffectIndex)</c>。
    /// <para><c>FIsSorted</c> 为真走二分（<c>(L + H) shr 1</c>，命中即 Break），
    /// 否则线性扫描。注意原文二分里 <c>C := Item.ItemEffectIndex - ItemEffectIndex</c>
    /// 是 **Word 相减后按 Integer 比较**（Delphi 会把两操作数提升为 Integer），
    /// 所以不会出现无符号回绕导致的比较反转。</para>
    /// </summary>
    public int IndexOf(int ItemEffectIndex)
    {
        int Result = -1;
        if (FList.Count == 0) return Result;

        if (FIsSorted)
        {
            int L = 0;
            int H = FList.Count - 1;
            while (L <= H)
            {
                int I = (L + H) >> 1;
                var Item = FList[I];
                int C = Item.ItemEffectIndex - ItemEffectIndex;
                if (C < 0)
                {
                    L = I + 1;
                }
                else
                {
                    H = I - 1;
                    if (C == 0)
                    {
                        Result = I;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int I = 0; I < FList.Count; I++)
            {
                if (FList[I].ItemEffectIndex == ItemEffectIndex)
                {
                    Result = I;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 :143-149 <c>Sort</c>：仅在非空时排序并置 FIsSorted := True。</summary>
    public void Sort()
    {
        if (FList.Count > 0)
        {
            QuickSort(0, FList.Count - 1);
            FIsSorted = true;
        }
    }

    /// <summary>
    /// 原文 :151-175 <c>QuickSort(L, R)</c>（Hoare 分区 + 尾递归消除）。
    /// <para>注意原文用 <c>P := (L + R) shr 1</c> 作为**值轴**而非下标轴，
    /// 交换后要把 P 跟着搬（<c>if P = I then P := J else if P = J then P := I</c>），
    /// 否则轴值会被换丢。逐行保留。</para>
    /// </summary>
    private void QuickSort(int L, int R)
    {
        int I, J, P;
        do
        {
            I = L;
            J = R;
            P = (L + R) >> 1;
            do
            {
                while (CompareItem(I, P) < 0) I++;
                while (CompareItem(J, P) > 0) J--;
                if (I <= J)
                {
                    Exchange(I, J);
                    if (P == I) P = J;
                    else if (P == J) P = I;
                    I++;
                    J--;
                }
            } while (I <= J);
            if (L < J) QuickSort(L, J);
            L = I;
        } while (I < R);
    }

    /// <summary>原文 :177-180 <c>CompareItem</c>：按 <c>ItemEffectIndex</c> 作差。</summary>
    private int CompareItem(int Index1, int Index2)
        => FList[Index1].ItemEffectIndex - FList[Index2].ItemEffectIndex;

    /// <summary>原文 <c>FList.Exchange(I, J)</c>（TList 的经典 swap）。</summary>
    private void Exchange(int I, int J)
    {
        var t = FList[I];
        FList[I] = FList[J];
        FList[J] = t;
    }

    /// <summary>测试接缝：导出当前顺序（原文只能通过 Items[i] 逐个读）。</summary>
    public IReadOnlyList<TDropItemEffect> ToList() => FList;
}
