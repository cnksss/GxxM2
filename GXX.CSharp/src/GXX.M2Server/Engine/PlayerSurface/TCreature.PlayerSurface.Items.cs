// ============================================================================
// 源单元：Source\M2Engine\ObjBase.pas（43,480 LF）／Source\M2Engine\ObjPlayer.pas（49,232 LF）
// 本文件：**物品容器片**（切片 2 / 4 的另一半）。
// 覆盖原文行号范围：
//   ObjBase.pas:882     m_UseItems: THumanUseItems;  // 0x420  + D8 -> 4F8
//   ObjBase.pas:322     m_ItemList: TList;         // 0x40C 人物背包  ← 托管已存在，不重复声明
//   ObjBase.pas:630     function IsEnoughBag(): Boolean; virtual;
//   ObjBase.pas:752     function CheckItems(sItemName: string): pTUserItem;
//   ObjBase.pas:708     function AddItemToBag(UserItem: pTUserItem): Boolean; virtual;
//   ObjBase.pas:570     function IsAddWeightAvailable(nWeight: Integer): Boolean;
//   ObjBase.pas:13691-13696  TBaseObject.IsEnoughBag
//   ObjBase.pas:26738-26741  TBaseObject.GetMaxBagCount
//   ObjBase.pas:26743-26752  TBaseObject.AddItemToBag
//   ObjBase.pas:41668-41684  TBaseObject.CheckItems
//   ObjBase.pas:41968-41974  TBaseObject.IsAddWeightAvailable
//   ObjBase.pas:32898-32900  Initialize 内的 AddToMap 落地
//   ObjBase.pas:35605        TBaseObject.WeightChanged
//   ObjPlayer.pas:1264       TPlayObject.GetMaxBagCount 覆写
//   ObjPlayer.pas:1196-1197  procedure SendAddItem / SendDelItem 声明
//   ObjPlayer.pas:3360-3394  TPlayObject.SendAddItem   // 004D0824
//   ObjPlayer.pas:12640-12657 TPlayObject.SendDelItem
//   ObjPlayer.pas:16298-...  TPlayObject.CheckItemsNeed
//   Grobal2.pas:51      MAX_USE_ITEM_COUNT = 30;
//   Grobal2.pas:102     U_WEAPON = 1;
//   Grobal2.pas:4169    THumanUseItems = array [0 .. MAX_USE_ITEM_COUNT - 1] of TUserItem;
//
// ⚠ 不重复声明的既有成员（git grep 证据见交付报告）：
//   · m_ItemList → Engine/ObjBase.cs:166（`List<TUserItem>`，挂在 TPlayObject 上）
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>
/// 物品容器片的**只写方**接缝（`SendAddItem` / `SendDelItem` / `CheckItemsNeed` 的宿主能力）。
/// </summary>
public static class PlayerSurfaceItemSeams
{
    /// <summary>
    /// `UserItemToClientItem(UserItem, StdItem, @ClientItem, True, True)`
    /// （ObjPlayer.pas:3373）——依赖 `ClientItem` 编码与 `TStdItem` 全字段；未移植。
    /// 默认：无宿主，返回 false 表示「无法编码」。
    /// </summary>
    public static Func<TUserItemView, TStdItem, object?> UserItemToClientItem { get; set; } = (_, _) => null;

    /// <summary>`UserEngine.GetStdItem(wIndex)`（ObjPlayer.pas:3369/12648）——见 PlayerSurfaceMsgSeams。</summary>
    public static Func<int, TStdItem?> GetStdItem
    {
        get => PlayerSurfaceMsgSeams.GetStdItem;
        set => PlayerSurfaceMsgSeams.GetStdItem = value;
    }

    /// <summary>`UserEngine.GetStdItemName(wIndex)`（ObjBase.pas:41678）——见 PlayerSurfaceMsgSeams。</summary>
    public static Func<int, string> GetStdItemName
    {
        get => PlayerSurfaceMsgSeams.GetStdItemName;
        set => PlayerSurfaceMsgSeams.GetStdItemName = value;
    }

    /// <summary>`g_FunctionNPC`（ObjPlayer.pas:3374/3378）——功能 NPC 单例；默认 null。</summary>
    public static object? FunctionNPC { get; set; }

    /// <summary>`g_FunctionNPC.GotoLable(Self, '@AddBag', False)`（ObjPlayer.pas:3378）。</summary>
    public static Action<object, TPlayObject, string, bool> FunctionNpcGotoLable { get; set; } = (_, _, _, _) => { };

    /// <summary>`m_dwRecordBeadExp` / `IncBeadExp`（ObjPlayer.pas:3388-3392）——未移植。</summary>
    public static Action<TPlayObject, uint> IncBeadExp { get; set; } = (_, _) => { };

    /// <summary>
    /// `ClientItem.btValue[10] := 0;`（ObjPlayer.pas:3384）——原文对**编码后的客户端物品包**
    /// 就地改写第 10 个附加值（注释：「防止武器升级后，通过查找内存可以知道是否成功」）。
    /// `TClientItem` 尚未在 M2Server 侧落类型（`Engine/Boxs.cs:381` 仅有注释），
    /// 故用 `object` 接缝；默认空操作（等同于「未编码即无需擦除」）。
    /// </summary>
    public static Action<object>? BlankClientItemBtValue10 { get; set; }

    // ★ 2026 第六轮（方案 A 第②步）：此处原有 4 个只读委托
    //   `ItemMakeIndex` / `ItemName` / `ItemDura` / `ItemDuraMax`（各自的 `Func<TUserItemView, ...>`），
    //   **已全部删除**。
    //   删除理由（台账 §25.2 立的规矩）：它们是"默认静默返回中性值"的委托
    //   （`_ => 0` / `_ => ""`），一旦忘记接线就把**字段语义错**伪装成**分支没命中**；
    //   而且 `Dura`/`DuraMax` 是**可写**的（`GetUserItemPrice` 在 `StdMode = 43` 时写回），
    //   只读 `Func` 从类型上就承载不了。现在物品元素是权威记录 `TUserItem`，字段直读即可。

    /// <summary>`g_CastleManager.IsCastleMember(Self)`（ObjPlayer.pas:16303）——`Castle.cs` 无该方法。</summary>
    public static Func<TPlayObject, object?> IsCastleMember { get; set; } = _ => null;

    /// <summary>`m_MyGuild`（ObjPlayer.pas:16307/16314）——`Guild` 未切到 TPlayObject 面。</summary>
    public static Func<TPlayObject, object?> MyGuild { get; set; } = _ => null;

    /// <summary>`m_nGuildRankNo`（ObjPlayer.pas:16314/16328）——未移植。</summary>
    public static Func<TPlayObject, int> GuildRankNo { get; set; } = _ => 0;

    /// <summary>`m_nMemberType` / `m_nMemberLevel`（ObjPlayer.pas:16335/16340）——未移植到 TPlayObject 面。</summary>
    public static Func<TPlayObject, int> MemberType { get; set; } = _ => 0;

    /// <summary>见 <see cref="MemberType"/>。</summary>
    public static Func<TPlayObject, int> MemberLevel { get; set; } = _ => 0;

    /// <summary>恢复默认（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        UserItemToClientItem = (_, _) => null;
        GetStdItem = _ => null;
        FunctionNPC = null;
        FunctionNpcGotoLable = (_, _, _, _) => { };
        IncBeadExp = (_, _) => { };
        BlankClientItemBtValue10 = null;
        // ★ 4 个 ItemXxx 委托已删除（见上），此处不再复位。
        IsCastleMember = _ => null;
        MyGuild = _ => null;
        GuildRankNo = _ => 0;
        MemberType = _ => 0;
        MemberLevel = _ => 0;
    }
}

/// <summary>
/// `TBaseObject` 的**物品容器面**。原文这些成员在 `TBaseObject` / `TSmartObject` 上，
/// 托管侧只有 `TCreature` 这一层（同 `TCreature.PlayerSurface.Base.cs` 的口径）。
/// </summary>
public abstract partial class TCreature
{
    /// <summary>
    /// 原文 `m_UseItems: THumanUseItems; // 0x420`（ObjBase.pas:882），
    /// 其中 `THumanUseItems = array [0 .. MAX_USE_ITEM_COUNT - 1] of TUserItem`
    /// （Grobal2.pas:4169，`MAX_USE_ITEM_COUNT = 30`，Grobal2.pas:51）。
    ///
    /// ★★ **本字段不在此声明** —— 托管侧**已存在**同名成员：
    /// `Engine/RecalcChain.cs:101` `public TUserItemView?[] m_UseItems = new TUserItemView?[UseSlots.SlotCount];`
    /// （§14.2「不造第三份」/ 防 CS0102）。本车道**复用它**，并登记两处偏差：
    /// <list type="number">
    ///   <item><description>**槽位数 21 vs 30**：`UseSlots.SlotCount = 21`（RecalcChain.cs:21），
    ///     原文是 30（Grobal2.pas:51/:4169「加盾牌 原为0..15」后为 30）。
    ///     原文 30 个槽位的**后半段（21..29）在源码里几乎没有读写点**，故 21 对当前已移植路径无影响；
    ///     但 `UpgradeWapon` 用的 `U_WEAPON = 1` 在两者范围内 —— **可读写**，已确认。</description></item>
    ///   <item><description>**元素类型 `TUserItemView?`（引用/可空）vs 原文值数组 `TUserItem`**：
    ///     原文 `m_UseItems[U_WEAPON].wIndex := 0;`（ObjNpc.pas:1886）在托管侧写作
    ///     `m_UseItems[UseSlots.U_WEAPON]!.wIndex = 0;` —— 语义一致（都是就地改写），
    ///     但**新槽默认是 `null` 而不是「全零的 TUserItem」**：调用方必须先 `new TUserItemView()`。
    ///     差异断言见 <c>PlayerSurfaceItemsTests.UseItems_ExistingSlotArrayIsReused_NotRedeclared</c>。</description></item>
    /// </list>
    /// </summary>
    /// <remarks>本段是**登记注释**，没有对应的字段声明（复用了 `RecalcChain.cs:101` 的既有成员）。</remarks>

    /// <summary>
    /// 原文 `m_ItemList: TList; // 0x40C 人物背包(Dword)数量`（**ObjBase.pas:322，在 TBaseObject 上**）。
    /// 本访问器为原文 **`TBaseObject` 的四个背包方法**（`AddItemToBag`/`IsEnoughBag`/
    /// `IsEnoughBagEx`/`CheckItems`，ObjBase.pas:708/630/13698/752）提供基类落点：
    /// 这些方法在原文里属于 `TBaseObject`，托管侧提到 `TCreature` 这一层才符合原文归属。
    /// </summary>
    /// <remarks>
    /// ★★ 集成方裁定（台账 §26，**方案 A**）：`GXX.Core.Protocol.TUserItem` 为**唯一存储与权威**，
    /// `TUserItemView` 退化为"能力聚合用的轻量视图"。
    /// <list type="number">
    ///   <item><description>**已完成**：`m_ItemList` 已改为 **`List&lt;TUserItem?&gt;`**（可空是为了保留原文
    ///     `pTUserItem` 的"空槽"语义 —— 原文多处 `if UserItem = nil then Continue`），
    ///     且**已从 `TPlayObject` 上移到 `TCreature`**（原文 `ObjBase.pas:322` 在 `TBaseObject` 上；
    ///     上移前 `TCreature.BagItems` 根本够不到它 —— 这正是**双容器产生的根因**）。
    ///     本行即 `BagItems => m_ItemList`，私有后备字段 `m_BagItems` **已删除**。</description></item>
    ///   <item><description>4 个只读委托 `ItemMakeIndex/ItemName/ItemDura/ItemDuraMax` **已删除**，
    ///     改为直读 `TUserItem` 字段；受影响的 7 个公开成员签名已收敛，
    ///     `PlayerSurfaceItemsTests.cs` 的约 40 处调用点已同步适配（该文件已获调度方加入本车道分区）。</description></item>
    /// </list>
    /// </remarks>
    /// <remarks>
    /// ⚠⚠ **正式偏差 D35：值语义 vs 指针语义**（已在交付报告登记为偏差条目，
    /// **不是**实现细节，**请勿当 bug 去"修"**）：
    /// <list type="bullet">
    ///   <item><description><b>偏离点</b>：元素是可空**值类型** `TUserItem?`，`BagItems.Add(x)` / `SetBagItem` 都是
    ///     **值复制**。</description></item>
    ///   <item><description><b>原文行为</b>：`m_ItemList` 存 `pTUserItem` **指针** ——
    ///     `BagItems.Items[I]^.X := v` 与"调用方手上那件"是**同一对象**，改一处两处都变（**别名共享**）。</description></item>
    ///   <item><description><b>托管行为</b>：改本地副本**不影响**背包。</description></item>
    ///   <item><description><b>为什么必须偏离</b>：`TUserItem` 是 `struct`（`Grobal2.Types6.cs:13`，
    ///     1:1 的 wire/DB 权威布局），值类型无法表达"共享同一实例"。</description></item>
    ///   <item><description><b>调用方契约（强制）</b>：**改动物品后必须写回槽位** ——
    ///     `var t = BagItems[i]!.Value; ...改 t...; SetBagItem(i, t);`
    ///     （原地 `BagItems[i]!.Value.Dura = x` 在 C# 中**不可编译**，值属性不可变）；
    ///     `GetUserItemPrice(ref TUserItem, ...)` 这类"按引用就地改写"的调用**不能**直接传 `BagItems[i]`
    ///     （`List&lt;T&gt;` 索引器不可 `ref`），同样要先取出、改完写回。</description></item>
    ///   <item><description><b>若将来要恢复别名语义</b>：唯一途径是回到**方案 B** —— 让
    ///     `TUserItemView` **持有 `TUserItem` 的引用**，使 `BagItems` 的元素变成引用类型。</description></item>
    ///   <item><description><b>为什么必须登记而不是只写注释</b>：这类差别**编译过、多数单测过**，
    ///     只在"改了一处、另一处没变"时暴露 —— 与台账 §25.2/§26.2/§29 那些"假完成/沉默中性值"同族。</description></item>
    /// </list>
    /// </remarks>
    protected virtual List<TUserItem?> BagItems => m_ItemList;

    /// <summary>
    /// 背包容器的**只读视图**（供跨程序集/NPC 车道读取，不暴露可变接口）。
    /// <para>写入路径见 <see cref="SetBagItem"/>（单向只读视图 + 单点写入，避免暴露整个可变 `List`）。</para>
    /// </summary>
    public IReadOnlyList<TUserItem?> Bag => m_ItemList;

    /// <summary>
    /// **方案 A 的必要补充入口**（已获调度方批准）：按下标**写回**背包里的一件。
    /// <para><b>原文依据</b>：Delphi `TList.Items[Index]` 是**可写属性**
    /// （`property Items[Index: Integer]: Pointer read Get write Put`），故原文完全允许
    /// `BagItems.Items[I] := UserItem;`；托管侧 `Bag` 是 `IReadOnlyList` 只读、`AddToBag` 只能追加，
    /// 因此**必须**补一个等价写入口，否则"取出 → 改 → 写回"这条原文模式无法表达
    /// （见偏差 **D35** 的调用方契约）。</para>
    /// <para>返回 `false` 表示下标越界（不做静默忽略，便于调用方发现真 bug）。</para>
    /// </summary>
    public bool SetBagItem(int index, TUserItem? item)
    {
        if (index < 0 || index >= m_ItemList.Count)
            return false;
        m_ItemList[index] = item;
        return true;
    }

    /// <summary>
    /// 向背包追加一件（等价原文 `m_ItemList.Add(UserItem)`；ObjNpc 侧 `ClientBuyItem` 等可直接用）。
    /// <para>⚠ 元素是**可空值类型**（对应原文 `pTUserItem`）：`Add(SomeItem)` 会把**值复制**进背包，
    /// 与原文"加入指针、共享同一对象"的**别名语义不同** —— 即正式偏差 **D35**，详见 <see cref="BagItems"/>。</para>
    /// </summary>
    public void AddToBag(TUserItem? item) => m_ItemList.Add(item);

    /// <summary>
    /// 原文 `function TBaseObject.GetMaxBagCount: Integer;`（ObjBase.pas:26738-26741）
    /// —— 基类返回 `DEF_MAX_BAG_ITEM`，`TPlayObject` 覆写（ObjPlayer.pas:1264）。
    /// ★ **原文 `TPlayObject.GetMaxBagCount` 是 `override`**（ObjPlayer.pas:1264），
    /// 故基类此处**必须** `virtual`，否则派生覆写无法参与多态。
    /// </summary>
    public virtual int GetMaxBagCount()
    {
        // 原文 26740：Result := DEF_MAX_BAG_ITEM;
        return PlayerSurfaceBaseSeams.GetMaxBagCount(this);
    }

    /// <summary>
    /// 原文 `function TBaseObject.IsEnoughBag: Boolean;`（ObjBase.pas:630 声明，:13691-13696 实现）。
    /// <code>
    ///   Result := False;
    ///   if BagItems.Count &lt; GetMaxBagCount then Result := True;
    /// </code>
    /// ⚠ 原文**不是** `virtual`（:630 声明**无** `virtual` 关键字）。
    /// </summary>
    public bool IsEnoughBag()
    {
        // 原文 13693：Result := False;
        bool result = false;
        // 原文 13694：if m_ItemList.Count < GetMaxBagCount then
        if (BagItems.Count < GetMaxBagCount())
            result = true;   // 原文 13695
        return result;
    }

    /// <summary>
    /// 原文 `function TBaseObject.IsEnoughBagEx(Count: Integer): Boolean;`（ObjBase.pas:13698-13703）。
    /// ⚠ 与 `IsEnoughBag` 的边界**不同**：这里是 `Count + Count &lt;= Max`（**可等于**），
    /// 而 `IsEnoughBag` 是 `&lt;`（不可等于）。原文如此，照抄。
    /// </summary>
    public bool IsEnoughBagEx(int count)
    {
        // 原文 13700：Result := False;
        bool result = false;
        // 原文 13701：if BagItems.Count + Count <= GetMaxBagCount then
        if (BagItems.Count + count <= GetMaxBagCount())
            result = true;   // 原文 13702
        return result;
    }

    /// <summary>
    /// 原文 `function TBaseObject.AddItemToBag(UserItem: pTUserItem): Boolean;`
    /// （ObjBase.pas:708 声明 `virtual`，:26743-26752 实现）。
    /// <code>
    ///   Result := False;
    ///   if BagItems.Count &lt; GetMaxBagCount then
    ///   begin
    ///     BagItems.Add(UserItem);
    ///     WeightChanged();
    ///     Result := True;
    ///   end;
    /// </code>
    /// ★ 原文 :708 声明是 **`virtual`** → 托管侧保留 `virtual`。
    /// ⚠ 托管 `m_ItemList` 是 `List&lt;TUserItemView&gt;`（**引用语义**，`AddAbility.cs:64`），
    /// 原文 `TList` 存 `pTUserItem` **指针** —— 两者在这一点上**语义一致**（加入的是引用）。
    /// </summary>
    public virtual bool AddItemToBag(TUserItem? userItem)
    {
        // 原文 26745：Result := False;
        bool result = false;

        // 原文 26746：if m_ItemList.Count < GetMaxBagCount then
        if (BagItems.Count < GetMaxBagCount())
        {
            // 原文 26748：m_ItemList.Add(UserItem);
            BagItems.Add(userItem);
            // 原文 26749：WeightChanged();
            WeightChanged();
            // 原文 26750：Result := True;
            result = true;
        }

        return result;   // 原文 26751
    }

    /// <summary>
    /// 原文 `procedure TBaseObject.WeightChanged();`（ObjBase.pas:35605）——**未移植**
    /// （依赖 `g_Config` 的重量倍率与全部 `m_UseItems` 重量求和）→ 最小接缝。
    /// </summary>
    public void WeightChanged()
    {
        PlayerSurfaceBaseSeams.WeightChanged(this);
    }

    /// <summary>
    /// 原文 `function TBaseObject.CheckItems(sItemName: string): pTUserItem;`
    /// （ObjBase.pas:752 声明，:41668-41684 实现）：
    /// <code>
    ///   Result := nil;
    ///   for I := 0 to BagItems.Count - 1 do
    ///   begin
    ///     UserItem := BagItems.Items[I];
    ///     if CompareText(UserEngine.GetStdItemName(UserItem.wIndex), sItemName) = 0 then
    ///     begin Result := UserItem; Break; end;
    ///   end;
    /// </code>
    /// 托管签名：返回**下标**（`-1` = 原文 `nil`）+ `out` 出参，
    /// 因为 C# 不能返回指向 `List&lt;T&gt;` 元素的指针；调用方用下标取/改元素。
    /// ⚠ 用 `-1` 作「未找到」哨兵时**不要**与 `wIndex` 混用（参见任务书「不用 != -1 当哨兵」的告警）；
    /// 本方法返回的是**背包下标**，取值范围 `[0, Count)`，与 `wIndex` 不共域，安全。
    /// </summary>
    /// <returns>命中的 `BagItems` 下标；未命中返回 `-1`（对应原文 `Result := nil`）。</returns>
    public int CheckItems(string sItemName, out TUserItem? userItem)
    {
        // 原文 41673：Result := nil;
        userItem = null;

        // 原文 41675：for I := 0 to BagItems.Count - 1 do
        for (int i = 0; i < BagItems.Count; i++)
        {
            // 原文 41677：UserItem := BagItems.Items[I];  （原文不判 nil 就读 wIndex → 空槽即 AV，照抄）
            var item = BagItems[i];
            // 原文 41678：if CompareText(UserEngine.GetStdItemName(UserItem.wIndex), sItemName) = 0 then
            // Delphi `CompareText` 不区分大小写 → 托管 `StringComparison.OrdinalIgnoreCase`。
            if (string.Equals(PlayerSurfaceItemSeams.GetStdItemName(item!.Value.wIndex), sItemName,
                    StringComparison.OrdinalIgnoreCase))
            {
                // 原文 41680-41681：Result := UserItem; Break;
                userItem = item;
                return i;
            }
        }

        return -1;   // 原文 41673 的 Result := nil
    }

    /// <summary>
    /// <see cref="CheckItems"/> 的**便利入口**（只问「在第几格」，不要元素本身）。
    /// 名字不同于原文（原文只有返回 `pTUserItem` 的那一个版本），故不占用任何原文成员名。
    /// </summary>
    /// <returns>命中的背包下标；未命中返回 `-1`（对应原文 `nil`）。</returns>
    public int CheckItemsIndex(string sItemName, out TUserItem? userItem)
        => CheckItems(sItemName, out userItem);

    /// <summary>
    /// 原文 `function TBaseObject.IsAddWeightAvailable(nWeight: Integer): Boolean; // 004C4A78`
    /// （ObjBase.pas:570 声明，:41968-41974 实现）：
    /// <code>
    ///   Result := False;
    ///   if (m_WAbil.Weight + nWeight) &lt;= m_WAbil.MaxWeight then Result := True;
    /// </code>
    /// ⚠ 原文的加法是 **Integer（32 位有符号）加法**，`nWeight` 为负且幅度大于 `Weight` 时
    /// 结果是负值 —— 仍然 `&lt;= MaxWeight` → **返回 True**。照抄（不夹 0）。
    /// </summary>
    public bool IsAddWeightAvailable(int nWeight)
    {
        // 原文 41970：Result := False;
        bool result = false;
        // 原文 41972：if (m_WAbil.Weight + nWeight) <= m_WAbil.MaxWeight then
        if (m_wAbil.Weight + nWeight <= m_wAbil.MaxWeight)
            result = true;   // 原文 41973
        return result;
    }
}

/// <summary>
/// `TPlayObject` 侧的物品容器方法（`SendAddItem` / `SendDelItem` / `CheckItemsNeed` /
/// `GetMaxBagCount` 覆写）。
/// </summary>
public partial class TPlayObject
{
    // 背包容器由 TCreature.BagItems（本文件同族）提供；Engine/ObjBase.cs:166 的
    // `List<TUserItem> m_ItemList` 未被接入（Engine/** 对本车道只读），整合建议见 BagItems 的注释。

    /// <summary>
    /// 原文 `procedure TPlayObject.GetMaxBagCount: Integer; override;`（ObjPlayer.pas:1264 `override`，
    /// 实现体在 ObjPlayer.pas 内以 `function TPlayObject.GetMaxBagCount: Integer;` 覆写默认背包格数）。
    /// ★ **原文是 `override`** → 托管侧必须是 `override`（基类已 `virtual`）。
    /// 实现体依赖 `g_Config` 的背包扩展（`nBagCount` 一路），**未移植** → 转发接缝。
    /// </summary>
    public override int GetMaxBagCount()
    {
        // 原文实现体：按 g_Config 的会员/扩展背包格数返回 → 接缝
        return PlayerSurfaceBaseSeams.GetMaxBagCount(this);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendAddItem(UserItem: pTUserItem); // 004D0824`
    /// （ObjPlayer.pas:1196 声明，:3360-3394 实现）。
    /// </summary>
    /// <remarks>
    /// 逐行对照：
    /// <list type="number">
    ///   <item><description>:3366-3367 `if m_boOffLine or m_boDummyObject then Exit;`</description></item>
    ///   <item><description>:3369-3371 `StdItem := UserEngine.GetStdItem(UserItem.wIndex); if StdItem = nil then Exit;`</description></item>
    ///   <item><description>:3373 `UserItemToClientItem(...)` → 接缝</description></item>
    ///   <item><description>:3374-3381 `(m_btRaceServer = RC_PLAYOBJECT) and (g_FunctionNPC &lt;&gt; nil)`
    ///     → `g_FunctionNPC.GotoLable(Self, '@AddBag', False)` 夹在
    ///     `m_nCurrentItemMakeIndex`/`m_sCurrentItemName` 的置位与复位之间 → 接缝</description></item>
    ///   <item><description>:3384 `ClientItem.btValue[10] := 0;` ——**防武器升级内存探测**，必须在编码之后</description></item>
    ///   <item><description>:3385-3386 `MakeDefaultMsg(SM_ADDITEM, ..., 1)` + `SendSocketEx` → 接缝</description></item>
    ///   <item><description>:3388-3393 聚灵珠经验：`(m_dwRecordBeadExp &gt; 0) and (StdItem.StdMode = 49) and (UserItem.Dura &lt; UserItem.DuraMax)`
    ///     → 三段阈值全为**严格大于/小于**；命中则 `m_dwRecordBeadExp := 0` 再 `IncBeadExp(旧值, False)`</description></item>
    /// </list>
    /// ⚠ `m_boOffLine`/`m_boDummyObject` 已在 `ObjBase.OnlineMsg.cs:10/13` 存在，**未重复声明**。
    /// ⚠ **签名偏差（托管侧）**：原文形参是 `pTUserItem`（指向值类型 `TUserItem`）。
    /// 托管侧 `m_UseItems` / `m_ItemList` 用的是 `TUserItemView`（`AddAbility.cs:64`，引用类型），
    /// 故形参取 `TUserItemView`；`MakeIndex`/`Dura`/`DuraMax`/`Name` 四个字段
    /// 不在 `TUserItemView` 上（它只有 `wIndex`/`BtValue`/`CustomProperties`），
    /// 经 <see cref="PlayerSurfaceItemSeams"/> 取值。差异见交付报告「接缝清单」。
    /// </remarks>
    public void SendAddItem(TUserItem userItem)
    {
        // 原文 3366-3367
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 3369-3371
        var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
        if (stdItem == null) return;

        // 原文 3373：UserItemToClientItem(UserItem, StdItem, @ClientItem, True, True);
        // 方案 A：`TUserItemView` 只是"能力聚合用的轻量视图"，调用处按需从权威记录现造。
        // 接缝：无法编码时按「无宿主」处理（返回 null 表示未编码）。
        var clientItem = PlayerSurfaceItemSeams.UserItemToClientItem(ToItemView(userItem), stdItem.Value);

        // 原文 3374-3381
        if (m_btRaceServer == Grobal2Const.RC_PLAYOBJECT && PlayerSurfaceItemSeams.FunctionNPC != null)
        {
            // 原文 3376-3377：m_nCurrentItemMakeIndex := UserItem.MakeIndex; m_sCurrentItemName := StdItem.Name;
            m_nCurrentItemMakeIndex = userItem.MakeIndex;
            m_sCurrentItemName = stdItem.Value.NameStr;
            // 原文 3378：g_FunctionNPC.GotoLable(Self, '@AddBag', False);
            PlayerSurfaceItemSeams.FunctionNpcGotoLable(
                PlayerSurfaceItemSeams.FunctionNPC, this, "@AddBag", false);
            // 原文 3379-3380：复位
            m_nCurrentItemMakeIndex = 0;
            m_sCurrentItemName = "";
        }

        // 原文 3384：ClientItem.btValue[10] := 0;  （防武器升级后靠内存探测是否成功）
        if (clientItem != null)
            PlayerSurfaceItemSeams.BlankClientItemBtValue10?.Invoke(clientItem);

        // 原文 3385-3386：MakeDefaultMsg(SM_ADDITEM, NativeInt(Self), 0, 0, 1) + SendSocketEx
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_ADDITEM,
            m_nRecogId, 0, 0, 1, "");
        if (clientItem != null)
            PlayerSurfaceMsgSeams.SendSocketEx(this, clientItem);

        // 原文 3388：if (m_dwRecordBeadExp > 0) and (StdItem.StdMode = 49) and (UserItem.Dura < UserItem.DuraMax) then
        if (m_dwRecordBeadExp > 0 && stdItem.Value.StdMode == 49
            && userItem.Dura < userItem.DuraMax)
        {
            // 原文 3390-3391：_dwRecordBeadExp := m_dwRecordBeadExp; m_dwRecordBeadExp := 0;
            uint old = m_dwRecordBeadExp;
            m_dwRecordBeadExp = 0;
            // 原文 3392：IncBeadExp(_dwRecordBeadExp, False);
            PlayerSurfaceItemSeams.IncBeadExp(this, old);
        }
    }

    /// <summary>
    /// 方案 A 的**视图现造点**：把权威记录 `TUserItem`（唯一存储）转成能力聚合用的
    /// `TUserItemView`（`AddAbility.cs:64`，只含 `wIndex` + `BtValue[14]` + `CustomProperties`）。
    /// <para>⚠ **有损**：`TUserItem.btValue` 原文是 `array[0..13] of Integer`（托管 `fixed int btValue[14]`），
    /// 而 `TUserItemView.BtValue` 是 `byte[14]` —— 大于 255 的附加值会被截断（按 `(byte)` 窄化，与原文
    /// 消费方 `GetAccessory` 读 byte 的口径一致）。已登记，见报告 §12.5 同族的"视图 ≠ 存储"说明。</para>
    /// </summary>
    internal static TUserItemView ToItemView(TUserItem item)
    {
        var view = new TUserItemView { wIndex = item.wIndex };
        for (int i = 0; i < view.BtValue.Length; i++)
            view.BtValue[i] = (byte)item.GetBtValue(i);
        return view;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendDelItem(UserItem: pTUserItem);`
    /// （ObjPlayer.pas:1197 声明，:12640-12657 实现）。
    /// <code>
    ///   if m_boOffLine or m_boDummyObject then Exit;
    ///   StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    ///   if StdItem &lt;&gt; nil then
    ///   begin
    ///     if (UserItem.btValue[13] = 1) and (UserItem.Name &lt;&gt; '') then sItemName := UserItem.Name
    ///     else sItemName := StdItem.Name;
    ///     SendDefMessage(SM_DELITEM, UserItem.MakeIndex, 0, 0, 0, sItemName);
    ///   end;
    /// </code>
    /// ⚠ `btValue[13]` 是**魔法下标**（ObjNpc 车道报告 D23 同型）：用 `btValue[13] = 1`
    /// 决定日志/消息里用自定义名还是标准名，无具名常量。
    /// </remarks>
    public void SendDelItem(TUserItem userItem)
    {
        // 原文 12645-12646
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 12648-12649
        var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
        if (stdItem != null)
        {
            string sItemName;
            // 原文 12651：if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
            if (userItem.GetBtValue(13) == 1 && userItem.NameStr != "")
                sItemName = userItem.NameStr;       // 原文 12652
            else
                sItemName = stdItem.Value.NameStr;  // 原文 12654
            // 原文 12655：SendDefMessage(SM_DELITEM, UserItem.MakeIndex, 0, 0, 0, sItemName);
            PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_DELITEM,
                userItem.MakeIndex, 0, 0, 0, sItemName);
        }
    }

    /// <summary>
    /// 原文 `function TPlayObject.CheckItemsNeed(StdItem: pTStdItem): Boolean;`
    /// （ObjPlayer.pas:16298-...）。
    /// <code>
    ///   Result := True;
    ///   Castle := g_CastleManager.IsCastleMember(Self);
    ///   case StdItem.Need of
    ///     6:  if (m_MyGuild = nil) then Result := False;
    ///     60: if (m_MyGuild = nil) or (m_nGuildRankNo &lt;&gt; 1) then Result := False;
    ///     7:  if Castle = nil then Result := False;
    ///     70: if (Castle = nil) or (m_nGuildRankNo &lt;&gt; 1) then Result := False;
    ///     8:  if m_nMemberType = 0 then Result := False;
    ///     81: if (m_nMemberType &lt;&gt; LoWord(StdItem.NeedLevel)) or
    ///              (m_nMemberLevel &lt; HiWord(StdItem.NeedLevel)) then Result := False;
    ///     ...
    ///   end;
    /// </code>
    /// 依赖 `m_MyGuild` / `m_nGuildRankNo` / `m_nMemberType` / `m_nMemberLevel` /
    /// `g_CastleManager.IsCastleMember` —— 全部**未移植**到 `TPlayObject` 面 → 最小接缝。
    /// </summary>
    public bool CheckItemsNeed(ref TStdItem stdItem)
    {
        // 原文 16302：Result := True;
        bool result = true;

        // 原文 16303：Castle := g_CastleManager.IsCastleMember(Self);
        object? castle = PlayerSurfaceItemSeams.IsCastleMember(this);

        // 原文 16304：case StdItem.Need of
        switch (stdItem.Need)
        {
            case 6:   // 原文 16305-16311
                if (PlayerSurfaceItemSeams.MyGuild(this) == null) result = false;
                break;
            case 60:  // 原文 16312-16318
                if (PlayerSurfaceItemSeams.MyGuild(this) == null
                    || PlayerSurfaceItemSeams.GuildRankNo(this) != 1) result = false;
                break;
            case 7:   // 原文 16319-16325
                if (castle == null) result = false;
                break;
            case 70:  // 原文 16326-16332
                if (castle == null || PlayerSurfaceItemSeams.GuildRankNo(this) != 1) result = false;
                break;
            case 8:   // 原文 16333-16337
                if (PlayerSurfaceItemSeams.MemberType(this) == 0) result = false;
                break;
            case 81:  // 原文 16338-...
                if (PlayerSurfaceItemSeams.MemberType(this) != (stdItem.NeedLevel & 0xFFFF)
                    || PlayerSurfaceItemSeams.MemberLevel(this) < (stdItem.NeedLevel >> 16)) result = false;
                break;
            // 原文余下分支（Need = 82/83/... 及 else）依赖未移植面，登记未覆盖，见报告。
        }

        return result;
    }
}
