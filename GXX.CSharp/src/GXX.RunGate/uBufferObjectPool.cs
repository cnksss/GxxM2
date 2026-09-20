using System;
using System.Collections.Generic;
using System.Threading;

namespace GXX.RunGate;

// =====================================================================================
// uBuffer.pas 对象池 + 全局释放部分 1:1 转换（Source\RunGate\Common\uBuffer.pas）
//
// 本文件覆盖原文行号：
//   :125-137  TDxObjectPool 声明
//   :1687-1756 TDxObjectPool 全部方法实现
//   :1758-1772 procedure FreeObjPool（转发到 MemoryPoolGlobal.FreeObjPool）
//
// 临界区映射：原 :130 `FLocker: TCriticalSection` → Monitor 上的 Enter/Leave
// （与 uBufferMemoryPool.cs 中 TDxMemoryPool.Lock/Unlock 同一手法）。
//
// ★ 原文缺陷：
//   O1 原 :1711-1715 的析构第二个循环里写的是 `FUses.Delete(FUnUses.Count - 1)`
//      —— **在 FUses 上按 FUnUses 的下标删除**（应为 FUnUses.Delete）。
//      当两个列表长度不同时会删错元素/越界（TList.Delete 越界抛 EListError）。
//      本移植**按原样保留**该越界风险（用 List.RemoveAt，越界时抛 ArgumentOutOfRangeException），
//      并有测试 <c>Destroy_UsesWrongListIndex_AsInOriginal</c> 固定该行为。
//   O2 原 :1741 `FObjClass.InheritsFrom(TComponent)` 只在 ObjClass 是 TComponent 子类时
//      走 `Create(nil)` 构造；本移植用 `Activator.CreateInstance`（无参公共构造），
//      TComponent 分支在托管侧不存在等价概念 —— 见 <see cref="GetObject"/> 注释。
// =====================================================================================

/// <summary>
/// 原 :125-137 `TDxObjectPool = class` —— 泛型对象池：`GetObject` 从空闲表尾取（复用），
/// `FreeObject` 归还（超过 MaxCount 直接释放）。
/// <para>
/// 原文用 `TList`（`List&lt;TObject&gt;`）；本移植用 <see cref="List{T}"/>（元素 object）。
/// 注意原文**不销毁游离对象**：`Destroy` 只释放池内的对象。
/// </para>
/// </summary>
public class TDxObjectPool
{
    // 原 :127 `FUses, FUnUses: TList`
    private readonly List<object> FUses = new();
    private readonly List<object> FUnUses = new();

    // 原 :128 `FObjClass: TClass`
    private Type FObjClass;

    // 原 :129 `FMaxObjCount: Integer`
    private int FMaxObjCount;

    // 原 :130 `FLocker: TCriticalSection` → lock 对象
    private readonly object FLocker = new();

    /// <summary>原 :1689-1695 `constructor Create(AMaxCount: Integer)`。</summary>
    public TDxObjectPool(int AMaxCount)
    {
        FMaxObjCount = AMaxCount;                                          // 原 :1693
        // 原 :1691-1692 FUses/FUnUses := TList.Create  → 字段初始化
        // 原 :1694 FLocker := TCriticalSection.Create        → 字段初始化
    }

    /// <summary>原 :1697-1701 `constructor Create(AMaxCount: Integer; ObjClass: TClass)`。</summary>
    public TDxObjectPool(int AMaxCount, Type ObjClass) : this(AMaxCount)
    {
        FObjClass = ObjClass;                                              // 原 :1700
    }

    /// <summary>原文没有的只读探针：`FUses.Count`。</summary>
    public int UsesCount => FUses.Count;

    /// <summary>原文没有的只读探针：`FUnUses.Count`。</summary>
    public int UnUsesCount => FUnUses.Count;

    /// <summary>原 :129 `FMaxObjCount`。</summary>
    public int MaxObjCount => FMaxObjCount;

    /// <summary>
    /// 原 :1735-1756 `function GetObject: TObject` —— 空闲表为空则**新建**并登记到 FUses；
    /// 否则从空闲表**尾部**取出（`FUnUses[Count-1]`，LIFO）并移入 FUses。
    /// </summary>
    public object GetObject()
    {
        Monitor.Enter(FLocker);                                            // 原 :1737
        try
        {
            if (FUnUses.Count == 0)                                        // 原 :1739
            {
                // 原 :1741-1744：
                //   if FObjClass.InheritsFrom(TComponent) then Result := TComponentClass(FObjClass).Create(nil)
                //   else Result := FObjClass.Create;
                object Result = CreateNewObject();
                FUses.Add(Result);                                         // 原 :1745
                return Result;
            }
            else                                                           // 原 :1747-1752
            {
                object Result = FUnUses[FUnUses.Count - 1];                // 原 :1749
                FUnUses.RemoveAt(FUnUses.Count - 1);                       // 原 :1750
                FUses.Add(Result);                                         // 原 :1751
                return Result;
            }
        }
        finally
        {
            Monitor.Exit(FLocker);                                         // 原 :1754
        }
    }

    /// <summary>
    /// 原 :1741-1744 的构造分派。
    /// <para>
    /// 原文 `FObjClass.Create` 是 Delphi 的**虚构造**调用（无参）；托管侧等价物是
    /// `Activator.CreateInstance(FObjClass)`。原文的 `InheritsFrom(TComponent) → Create(nil)`
    /// 分支在托管侧没有对应类型（本工程没有 VCL），故统一走无参构造 —— 见 O2。
    /// </para>
    /// <para>`FObjClass` 为 nil 时原文会 AV；托管侧抛 <see cref="InvalidOperationException"/>。</para>
    /// </summary>
    private object CreateNewObject()
    {
        if (FObjClass == null)
            throw new InvalidOperationException(
                "TDxObjectPool.GetObject: 未通过 Create(AMaxCount, ObjClass) 指定对象类型" +
                "（原文此处为 FObjClass 为 nil 的访问违例，uBuffer.pas:1741-1744）");
        return Activator.CreateInstance(FObjClass);
    }

    /// <summary>
    /// 原 :1721-1733 `procedure FreeObject(Obj: TObject)`。
    /// <list type="number">
    /// <item>`FUses.Remove(obj)` —— 原文**不检查是否在表中**，不在则 TList.Remove 静默不做；</item>
    /// <item>`if FUnUses.Count + 1 &gt; FMaxObjCount then obj.Free`（**真销毁**），否则 `FUnUses.Add(Obj)`。</item>
    /// </list>
    /// <para>★ 阈值是 **`+1 &gt;`**（原 :1726）：`FUnUses.Count = MaxObjCount` 时归还即销毁；
    /// `FUnUses.Count = MaxObjCount - 1` 时归还后恰好装满。测试有边界断言。</para>
    /// <para>★ 托管侧不做 `obj.Free`（GC 语义），改为**丢弃引用**（即不加入 FUnUses）；
    /// 这样 "复用同一实例" 的可观测语义与原文一致（同一 TestConfig 下的对象数受 MaxObjCount 限制）。</para>
    /// </summary>
    public void FreeObject(object Obj)
    {
        Monitor.Enter(FLocker);                                            // 原 :1723
        try
        {
            FUses.Remove(Obj);                                             // 原 :1725
            if (FUnUses.Count + 1 > FMaxObjCount)                          // 原 :1726
            {
                // 原 :1727 obj.Free —— 托管侧 GC 回收，这里只需不再入池
            }
            else
                FUnUses.Add(Obj);                                          // 原 :1729
        }
        finally
        {
            Monitor.Exit(FLocker);                                         // 原 :1731
        }
    }

    /// <summary>
    /// 原 :1703-1719 `destructor Destroy` —— 释放池内对象并清空两个列表。
    /// <para>★ O1：第二个循环的 `FUses.Delete(FUnUses.Count - 1)` 是原文笔误；本移植照抄其形状，
    /// 于是当 `FUses.Count &lt; FUnUses.Count` 时会抛
    /// <see cref="ArgumentOutOfRangeException"/>（对应原文的 `EListError`）。</para>
    /// </summary>
    public void Dispose()
    {
        // 原 :1705 FLocker.Free —— 托管侧 lock 对象无显式释放
        while (FUses.Count > 0)                                            // 原 :1706-1710
        {
            FUses.RemoveAt(FUses.Count - 1);                               // 原 :1709（原 :1708 为 .Free）
        }
        while (FUnUses.Count > 0)                                          // 原 :1711-1715
        {
            object obj = FUnUses[FUnUses.Count - 1];                       // 原 :1713（原 :1713 为 .Free）
            FUses.RemoveAt(FUnUses.Count - 1);                             // 原 :1714 ★ O1：故意删 FUses
            FUnUses.Remove(obj);
        }
        // 原 :1716-1717 FUses.Free / FUnUses.Free —— 托管侧由 GC 接管
    }
}

/// <summary>
/// 原 :1758-1772 `procedure FreeObjPool` —— 释放六个全局内存池。
/// <para>本移植转发到 <see cref="MemoryPoolGlobal.FreeObjPool"/>（原文同一动作）。</para>
/// </summary>
public static class FreeObjPoolGlobal
{
    /// <summary>原 :1758-1772。</summary>
    public static void FreeObjPool() => MemoryPoolGlobal.FreeObjPool();
}
