// ============================================================================
// 源单元：Source/M2Engine/ItemEvent.pas（1:1 移植）
//   · 类本体 TItemObject + 基类 TGameObject → 同目录 `ItemEvent.cs`
//   · 本文件 = **TItemManager**（ItemEvent.pas:38-55 声明、166-386 实现）
//
// 供 tools/audit-coverage.ps1 的 E2 规则识别的单元名：Source/M2Engine/ItemEvent.pas
//
// 【原文结构】`TItemManager` 持两个 `TGList`：
//   `m_ItemList`（活跃地面物品）与 `m_FreeItemList`（待释放幽灵物品），
//   外加一个**游标** `m_nProcItemIDx`（原文 :41）—— 让 `Run` 每帧最多跑 5 ms 后**接着上次的位置续跑**。
//
// 【托管侧复用】`TGList` 用既有的 `GXX.Core.Protocol.SDK.TGList`（SDK.cs:55，含 Lock/UnLock），
//   **不另造列表类型**。`TGList` 的索引器 `this[int]`、`Add`、`RemoveAt(int)` 与本单元用到的
//   `Items[I]`/`Add`/`Delete(I)` 一一对应（Delphi `TGList.Delete(I)` ↔ 托管 `RemoveAt(I)`）。
// ============================================================================

using System;
using GXX.Core.Protocol;

// 既有 `TGList` 落在嵌套类 `SDK` 里（`GXX.Core.Protocol.SDK.TGList`，SDK.cs:55），
// 与全仓既有用法一致（如 `GXX.Client/GUI/Mir/ClientGlobals.cs:7`）。
// 起别名的目的只是让本文件中的类型名与原文（`TGList`）**逐字一致**。
using TGList = GXX.Core.Protocol.SDK.TGList;

namespace GXX.M2Server.Sweep9.DataLayer;

/// <summary>
/// ItemEvent.pas:38-55 的 <c>TItemManager</c> 1:1。
///
/// <para><b>字段</b>：`m_ItemList: TGList`（:39）、`m_FreeItemList: TGList`（:40）、
/// `m_nProcItemIDx: Integer`（:41）。</para>
///
/// <para><b>7 个过程函数</b>：`Create`(:166-171)、`Destroy`(:173-189)、`GetItemCount`(:191-199)、
/// `AddItem`(:201-209)、4 个 `FindItem` 重载(:211-301)、`Run`(:303-386)。</para>
///
/// <para><b>原文把 `Lock`/`UnLock` 全部注释掉了</b>（:193-198、:203-208、:216-229、:238-252、
/// :261-275、:284-300、:316-317、:345-347、:354-355、:371-372、:383-385）
/// —— 即**列表操作实际无锁**，尽管 `TGList` 本身线程安全。逐字保留：托管侧同样只调
/// `Add`/`Delete`/索引器，**不补 Lock**（那会偏离 1:1 并可能死锁）。</para>
/// </summary>
public class TItemManager
{
    /// <summary>原文 `m_ItemList: TGList;`（:39）。</summary>
    public TGList m_ItemList = new();

    /// <summary>原文 `m_FreeItemList: TGList;`（:40）。</summary>
    public TGList m_FreeItemList = new();

    /// <summary>原文 `m_nProcItemIDx: Integer;`（:41）——`Run` 的续跑游标。</summary>
    public int m_nProcItemIDx;

    /// <summary>
    /// ItemEvent.pas:166-171 的 `constructor TItemManager.Create();`。
    /// <para>**原文没有 `inherited`**（对比 TItemObject.Create 的 :64 有）—— 逐字保留：
    /// 托管侧构造不写 `: base()`（隐式调用无参基构造，语义与原文一致）。</para>
    /// </summary>
    public TItemManager()
    {
        m_ItemList = new TGList();
        m_FreeItemList = new TGList();
        m_nProcItemIDx = 0;
    }

    /// <summary>
    /// ItemEvent.pas:173-189 的 `destructor TItemManager.Destroy;`。
    /// <para>原文：先遍历 `m_ItemList` **逐个 `Free`**（:177-180）再 `m_ItemList.Free`（:181）；
    /// 然后同样处理 `m_FreeItemList`（:183-187）；最后 `inherited`（:188）。</para>
    /// <para>托管侧 `TGList.Dispose()` 是空实现（SDK.cs:99），GC 负责释放；
    /// 逐元素释放语义在托管侧由"清空两表"表达，且**顺序与原文一致**（先 Item 后 Free）。</para>
    /// </summary>
    public void Destroy()
    {
        for (int I = 0; I <= m_ItemList.Count - 1; I++)
        {
            // 原文 :179 `TItemObject(m_ItemList.Items[I]).Free;` —— 托管侧由 GC 承担
        }
        m_ItemList.Dispose();

        for (int I = 0; I <= m_FreeItemList.Count - 1; I++)
        {
            // 原文 :185 `TItemObject(m_FreeItemList.Items[I]).Free;` —— 托管侧由 GC 承担
        }
        m_FreeItemList.Dispose();
    }

    /// <summary>
    /// ItemEvent.pas:191-199 的 `function TItemManager.GetItemCount: Integer;`
    /// （`property ItemCount: Integer read GetItemCount`，:54）。
    /// <para>原文 :193-198 把 `m_ItemList.Lock` / `UnLock` 注释掉了，只剩 `Result := m_ItemList.Count;`。</para>
    /// </summary>
    public int ItemCount => m_ItemList.Count;

    /// <summary>
    /// ItemEvent.pas:201-209 的 `procedure TItemManager.AddItem(ItemObject: TItemObject);`。
    /// 原文把锁注释掉，只剩 `m_ItemList.Add(ItemObject);`（:205）。
    /// </summary>
    public void AddItem(TItemObject ItemObject)
    {
        m_ItemList.Add(ItemObject);
    }

    /// <summary>
    /// ItemEvent.pas:211-230 的**第一个** `FindItem` 重载
    /// （`FindItem(Envir: TObject; ItemObject: TItemObject): TItemObject`）。
    /// <para>判据（:220-221）三条件与：`not m_boGhost` **且** `m_PEnvir = Envir` **且** 引用相等。
    /// 命中即 `Result := ...; Break;`。</para>
    /// <para><b>注意同名不同义的消歧</b>：Delphi 用 `overload`；托管侧四个重载的形参表互不相同
    /// （依次 2/3/4/5 参），**不需要**重命名。</para>
    /// </summary>
    public TItemObject? FindItem(object Envir, TItemObject ItemObject)
    {
        TItemObject? Result = null;
        for (int I = 0; I <= m_ItemList.Count - 1; I++)
        {
            TItemObject item = (TItemObject)m_ItemList[I];
            if ((!item.m_boGhost) && ReferenceEquals(item.m_PEnvir, Envir) && ReferenceEquals(item, ItemObject))
            {
                Result = item;
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// ItemEvent.pas:232-253 的第二个 `FindItem` 重载（按坐标）。
    /// <para>判据（:243）：`not m_boGhost` 且 `m_PEnvir = Envir` 且 `m_nMapX = nX` 且 `m_nMapY = nY`。
    /// **与第一个重载的差异**：把"对象相等"换成了"坐标相等"。</para>
    /// </summary>
    public TItemObject? FindItem(object Envir, int nX, int nY)
    {
        TItemObject? Result = null;
        for (int I = 0; I <= m_ItemList.Count - 1; I++)
        {
            TItemObject ItemObject = (TItemObject)m_ItemList[I];
            if ((!ItemObject.m_boGhost) && ReferenceEquals(ItemObject.m_PEnvir, Envir) &&
                (ItemObject.m_nMapX == nX) && (ItemObject.m_nMapY == nY))
            {
                Result = ItemObject;
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// ItemEvent.pas:255-276 的第三个 `FindItem` 重载（坐标 + 对象）。
    /// <para><b>★ 原文缺陷（照抄 + 差异断言锁定）</b>：判据（:266-267）混用了**两个不同的变量**——
    /// `(AItemObject = ItemObject)` 与 `(AItemObject.m_PEnvir = Envir)` 比的是**列表元素** `AItemObject`，
    /// 但坐标 `(ItemObject.m_nMapX = nX) and (ItemObject.m_nMapY = nY)` 读的是**形参** `ItemObject`。
    /// 由于 :266 已要求二者引用相等，**当前路径下两者恒等**，所以行为正确；
    /// 但一旦将来放宽相等判定，坐标就会读错对象。托管侧逐字保留 `AItemObject`/`ItemObject` 两个变量
    /// （**不"统一"成同一个变量**）。</para>
    /// <para>返回的是**形参** `ItemObject`（:269 `Result := ItemObject;`），不是列表元素。</para>
    /// </summary>
    public TItemObject? FindItem(object Envir, int nX, int nY, TItemObject ItemObject)
    {
        TItemObject? Result = null;
        for (int I = 0; I <= m_ItemList.Count - 1; I++)
        {
            TItemObject AItemObject = (TItemObject)m_ItemList[I];
            if ((!AItemObject.m_boGhost) && ReferenceEquals(AItemObject.m_PEnvir, Envir) &&
                ReferenceEquals(AItemObject, ItemObject) &&
                (ItemObject.m_nMapX == nX) && (ItemObject.m_nMapY == nY))
            {
                Result = ItemObject;
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// ItemEvent.pas:278-301 的第四个 `FindItem` 重载（矩形范围收集）。
    /// <para>判据（:289-290）用**绝对值差** `abs(m_nMapX - nX) &lt;= nRange` 与
    /// `abs(m_nMapY - nY) &lt;= nRange` ⇒ 闭区间方形；`List &lt;&gt; nil` 时才 `Add`，
    /// 但 `Inc(nCount)` **无条件**执行（:292-294）——所以即使不收集，返回值也照数。</para>
    /// <para>形参 `List: TList`（原文 :278）在托管侧取既有的 `GXX.Core.Protocol.TGList`
    /// （SDK.cs:55，Delphi `TGList` 的 1:1 对应；`TGList` 本就是 `TList` 的派生，
    /// 故这是**收窄到既有等价类型**而非另造）。为表达原文的 `List &lt;&gt; nil`，此处可空。</para>
    /// </summary>
    public int FindItem(object Envir, int nX, int nY, int nRange, TGList? List)
    {
        int nCount = 0;
        for (int I = 0; I <= m_ItemList.Count - 1; I++)
        {
            TItemObject ItemObject = (TItemObject)m_ItemList[I];
            if ((!ItemObject.m_boGhost) && ReferenceEquals(ItemObject.m_PEnvir, Envir) &&
                (Math.Abs(ItemObject.m_nMapX - nX) <= nRange) &&
                (Math.Abs(ItemObject.m_nMapY - nY) <= nRange))
            {
                nCount++;
                if (List != null)
                    List.Add(ItemObject);
            }
        }
        return nCount;
    }
    /// <summary>
    /// ItemEvent.pas:303-386 的 `procedure TItemManager.Run();` 1:1。
    ///
    /// <para><b>原文的 `resourcestring`</b>（:309-310）：本函数体内声明了
    /// `sExceptionMsg = '[Exception] TItemManager.Run'` —— 逐字保留为 <see cref="ExceptionMsg"/>。</para>
    ///
    /// <para><b>三段结构</b>：
    /// <b>①</b> :312-352 主循环：`while True` 中
    ///   a) `m_ItemList.Count &lt;= nIdx` ⇒ `Break`（:320-321）；
    ///   b) 取 `ItemObject := m_ItemList.Items[nIdx]`（:323）；
    ///   c) **非幽灵且距 `m_dwRunTick` 超 250 ms** 才刷新 tick 并 `ItemObject.Run()`（:324-328）；
    ///   d) 若变幽灵 ⇒ 入 `m_FreeItemList` + `m_ItemList.Delete(nIdx)` + `Continue`
    ///      （**注意：`Continue` 时不 `Inc(nIdx)`**，因为删除后同一位置已是下一个元素，:330-335）；
    ///   e) 否则 `Inc(nIdx)`；若本次已耗时 > 5 ms ⇒ 记 `boCheckTimeLimit := True`、
    ///      **保存游标** `m_nProcItemIDx := nIdx`（:338-343）后 `Break`。
    ///   循环后：`if not boCheckTimeLimit then m_nProcItemIDx := 0;`（:348-349）
    ///   —— 跑完了就复位游标，被时间限制打断就保留（下帧续跑）。
    ///   `except MainOutMessage(sExceptionMsg)`（:350-351）**吞掉一切异常**，且**不重置游标**。
    /// <b>②</b> :353-369 是**整段被 `{ }` 注释掉**的旧版倒序循环 —— 逐字保留为注释，**不执行**。
    /// <b>③</b> :373-385 清理 `m_FreeItemList`：从尾到头，若 `MyGetTickCount - m_dwGhostTick > 5*60*1000`
    ///   则 `Delete(I)` + `Free`（:376-381）；:380 的 `// break;` 是**被注释掉的 break**，逐字保留。</para>
    ///
    /// <para><b>原文缺陷（照抄 + 差异断言锁定）</b>：
    /// ① :313 与 :324/:326/:338 各调 `MyGetTickCount`，**每次都是新值**；被测帧长由假时钟控制。
    /// ② :324 的 `(MyGetTickCount - ItemObject.m_dwRunTick) > 250` 与 :326 的
    ///    `ItemObject.m_dwRunTick := MyGetTickCount()` 是**两次不同调用**（差几微秒，原始如此）。
    /// ③ 异常路径（:350）**不重置 `m_nProcItemIDx`**，而正常路径（:348-349）会 —— 不对称，照抄。
    /// ④ 段③的清理循环**没有 `Lock`**（:371-372 的 `Lock` 被注释掉），且 `I` 是 `Integer`、
    ///    循环变量由 `downto 0` 递减 ⇒ 托管侧用 `for (int I = Count - 1; I >= 0; I--)`。
    /// ⑤ **第一段 while 循环的 `I`/`nIdx` 与段③的 `I` 是两个不同的变量**（:305 声明 `I, nIdx`）；
    ///    段① 用 `nIdx`，段③ 用 `I`。托管侧逐字保留两个局部变量名。</para>
    /// </summary>
    public void Run()
    {
        bool boCheckTimeLimit = false;
        uint dwCheckTime = Sweep9DataLayerSeam.MyGetTickCount();
        int nIdx = m_nProcItemIDx;
        try
        {
            while (true)
            {
                if (m_ItemList.Count <= nIdx)
                    break;

                TItemObject ItemObject = (TItemObject)m_ItemList[nIdx];
                if ((!ItemObject.m_boGhost) &&
                    ((Sweep9DataLayerSeam.MyGetTickCount() - ItemObject.m_dwRunTick) > 250))
                {
                    ItemObject.m_dwRunTick = Sweep9DataLayerSeam.MyGetTickCount();
                    ItemObject.Run();
                }

                if (ItemObject.m_boGhost)
                {
                    m_FreeItemList.Add(ItemObject);
                    RemoveAt(m_ItemList, nIdx);
                    continue;
                }

                nIdx++;
                if ((Sweep9DataLayerSeam.MyGetTickCount() - dwCheckTime) > 5)
                {
                    boCheckTimeLimit = true;
                    m_nProcItemIDx = nIdx;
                    break;
                }
            }

            if (!boCheckTimeLimit)
                m_nProcItemIDx = 0;
        }
        catch (Exception)
        {
            Sweep9DataLayerSeam.MainOutMessage(ExceptionMsg);
        }

        // 原文 :353-369 —— 整段被 { } 注释掉的旧版倒序循环（**不执行**，逐字保留为注释）：
        //
        //   m_ItemList.Lock;
        //   try
        //     for I := m_ItemList.Count - 1 downto 0 do begin
        //       ItemObject := TItemObject(m_ItemList.Items[I]);
        //       if (not ItemObject.m_boGhost) and ((MyGetTickCount - ItemObject.m_dwRunTick) > 250) then begin
        //         ItemObject.m_dwRunTick := MyGetTickCount();
        //         ItemObject.Run();
        //       end;
        //       if ItemObject.m_boGhost then begin
        //         m_FreeItemList.Add(ItemObject);
        //         m_ItemList.Delete(I);
        //       end;
        //     end;
        //   finally
        //     m_ItemList.UnLock;
        //   end;

        // { m_FreeItemList.Lock; try }  ← 原文 :371-372 的 Lock 被注释掉
        for (int I = m_FreeItemList.Count - 1; I >= 0; I--)
        {
            TItemObject ItemObject = (TItemObject)m_FreeItemList[I];
            if ((Sweep9DataLayerSeam.MyGetTickCount() - ItemObject.m_dwGhostTick) > 5 * 60 * 1000)
            {
                RemoveAt(m_FreeItemList, I);
                // 原文 :379 `ItemObject.Free;` —— 托管侧由 GC 承担
                // 原文 :380 `// break;` —— 被注释掉的 break，逐字保留
            }
        }
    }

    /// <summary>
    /// ItemEvent.pas:310 的 `resourcestring sExceptionMsg = '[Exception] TItemManager.Run';`。
    /// <para>级联：`Run` 的 `except MainOutMessage(sExceptionMsg)`（:351）用此串。</para>
    /// </summary>
    public const string ExceptionMsg = "[Exception] TItemManager.Run";

    /// <summary>
    /// `TGList.Delete(Index)` 的语义在既有托管实现里叫 `RemoveAt(int)`
    /// （SDK.cs:72；Delphi 侧 `TGList.Delete(Index)` 与之等价）。此处集中一层，
    /// 便于将来若 `TGList` 补出 `Delete(int)` 时一处替换。
    /// </summary>
    private static void RemoveAt(TGList list, int index) => list.RemoveAt(index);
}
