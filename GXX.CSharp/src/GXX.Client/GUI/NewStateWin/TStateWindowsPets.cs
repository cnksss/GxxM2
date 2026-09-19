using GXX.Core.Protocol;

namespace GXX.Client.GUI.NewStateWin;

/// <summary>
/// StateWindows.pas 宠物（GamePet / 宠物包裹）相关的**纯逻辑**移植。
///
/// 对应用户端侧原文（Source\Client-HGE\GUI\NewStateWin\StateWindows.pas）：
///   - DSWPetsGridDblClick        13692-13738  宠物包裹网格双击
///   - DSWPetsGridGridPaint       13740-13753  网格单元绘制定位
///   - DSWPetsGridGridSelect      13755-13826  网格单元选择/拖放
///   - DSWPetsGridGridMouseMove   13828-13864  网格单元悬浮提示定位
///   - UpdateSelectPetsInfo       13208-13243  选中宠物信息
///   - RefreshGamePetList         13254-13297  宠物列表刷新
///
/// 接缝：g_PetItemArr / g_PetItemArrEffect / g_MovingItem / g_WaitingUseItem（MShare.pas）未移植，
/// 本文件只移植**下标换算 / 边界判定 / 分支选择**三类可单测逻辑，物品数据由调用方以委托提供。
/// 接缝：frmMain.PetUseItem / SendBagItemToPetBag / SendPetOverLapItem / ArrangePetItembag（FState.pas 的 TFrmDlg）。
/// </summary>
public static class TStateWindowsPets
{
    /// <summary>宠物包裹格数（Grobal2.pas MAX_GAMEPET_BAG_COUNT = 30）。</summary>
    public const int MaxGamePetBagCount = Grobal2Const.MAX_GAMEPET_BAG_COUNT;

    /// <summary>
    /// 原文 13703 / 13748 / 13765 / 13842（四处逐字相同）：
    /// `nIdx := ACol + ARow * DItemGrid.ColCount;` —— 行优先（ColCount 是每行格数）。
    /// </summary>
    public static int GridIndex(int aCol, int aRow, int colCount)
        => aCol + aRow * colCount;   // 原文如此（StateWindows.pas:13703）

    /// <summary>
    /// 原文 13704 / 13749 / 13766 / 13844（四处逐字相同）：
    /// `if nIdx in [0..MAX_GAMEPET_BAG_COUNT - 1] then ...`（闭区间，上界 = 29）。
    /// </summary>
    public static bool IsValidPetBagIndex(int nIdx)
        => nIdx >= 0 && nIdx <= MaxGamePetBagCount - 1;

    /// <summary>
    /// 原文 13750：`R := Bounds(Rect.Left, Rect.Top, DItemGrid.ColWidth, DItemGrid.RowHeight);`
    /// 即单元矩形左/上取传入 Rect，宽高取网格的 ColWidth / RowHeight（忽略传入 Rect 的宽高）。
    /// </summary>
    public static (int Left, int Top, int Right, int Bottom) GridCellBounds(
        int rectLeft, int rectTop, int colWidth, int rowHeight)
        => (rectLeft, rectTop, rectLeft + colWidth, rectTop + rowHeight);

    /// <summary>
    /// 原文 13849-13851 的悬浮提示定位：
    /// `FrmDlg.ShowMouseItemInfo(g_MySelf, @g_MouseItem,
    ///     vtRect.Left + ACol * DItemGrid.ColWidth,
    ///     vtRect.Top + +(ARow + 1) * DItemGrid.RowHeight, ...)`
    /// 注意 y 用的是 `+(ARow + 1)`（多一行），x 用的是 `ACol`（不加一）——原文如此。
    /// </summary>
    public static (int X, int Y) MouseItemInfoPoint(
        int virtualRectLeft, int virtualRectTop, int aCol, int aRow, int colWidth, int rowHeight)
        => (virtualRectLeft + aCol * colWidth,
            virtualRectTop + (aRow + 1) * rowHeight);   // 原文如此（StateWindows.pas:13850）

    /// <summary>宠物包裹单元选择的分支结果（对应 DSWPetsGridGridSelect 的各分支）。</summary>
    public enum SelectAction
    {
        /// <summary>下标越界 / 右键 / 移动物品类型不被接受：什么都不做。</summary>
        None,
        /// <summary>抓起：g_boItemMoving := True; g_MovingItem.Index := nIdx（原文 13774-13781）。</summary>
        PickUp,
        /// <summary>从普通背包放进宠物包裹：SendBagItemToPetBag（原文 13787-13796）。</summary>
        BagToPetBag,
        /// <summary>与目标格重叠：SendPetOverLapItem（原文 13800-13808）。</summary>
        Overlap,
        /// <summary>与目标格交换（原文 13810-13814）。</summary>
        Swap,
        /// <summary>放进空格（原文 13817-13821）。</summary>
        Place,
    }

    /// <summary>
    /// <see cref="SelectAction.Overlap"/> 的细分（**仅为可观测性引入**，原文没有这个区分）。
    /// 原文 13802 的 `if g_WaitingUseItem.Item.s.Name = ''` 只控制 SendPetOverLapItem 是否发出，
    /// 分类结果同样是「重叠」；细分后可以断言该 gating 确实生效。
    /// </summary>
    public enum OverlapKind
    {
        /// <summary>不是重叠分支。</summary>
        NotOverlap,
        /// <summary>重叠且 g_WaitingUseItem 为空 → 会发 SendPetOverLapItem。</summary>
        SendOverlap,
        /// <summary>重叠但 g_WaitingUseItem 非空 → 原文什么都不发（也不交换、不放置）。</summary>
        SuppressedByWaitingItem,
    }

    /// <summary>
    /// 原文 13800-13808 的细分判定（保留给测试与后续真类实现使用）。
    /// </summary>
    public static OverlapKind ClassifyOverlap(bool targetHasItem, MovingItemType movingItemType,
        bool isOverLap, bool waitingUseItemEmpty)
    {
        if (movingItemType != MovingItemType.GamePetBagItem) return OverlapKind.NotOverlap;
        if (!targetHasItem) return OverlapKind.NotOverlap;
        if (!isOverLap) return OverlapKind.NotOverlap;
        return waitingUseItemEmpty ? OverlapKind.SendOverlap : OverlapKind.SuppressedByWaitingItem;
    }

    /// <summary>g_MovingItem.ItemType（MShare.pas）用到的两个取值。</summary>
    public enum MovingItemType
    {
        /// <summary>mtBagItem</summary>
        BagItem,
        /// <summary>mtGamePetBagItem</summary>
        GamePetBagItem,
        /// <summary>其它类型</summary>
        Other,
    }

    /// <summary>
    /// 原文 13762-13825 的分支骨架（只保留可判定的分支选择，不含物品搬运副作用）。
    /// </summary>
    /// <param name="aCol">ACol</param>
    /// <param name="aRow">ARow</param>
    /// <param name="colCount">DItemGrid.ColCount</param>
    /// <param name="rightButton">Button = mbRight</param>
    /// <param name="itemMoving">g_boItemMoving</param>
    /// <param name="targetHasItem">g_PetItemArr[nIdx].S.Name &lt;&gt; ''</param>
    /// <param name="movingItemType">g_MovingItem.ItemType</param>
    /// <param name="movingIndexEqualsNIdx">g_MovingItem.Index = nIdx</param>
    /// <param name="isOverLap">IsOverLapItem(@g_PetItemArr[nIdx], @g_MovingItem.Item)</param>
    /// <param name="waitingUseItemEmpty">g_WaitingUseItem.Item.s.Name = ''</param>
    public static SelectAction ClassifyGridSelect(
        int aCol, int aRow, int colCount,
        bool rightButton,
        bool itemMoving,
        bool targetHasItem,
        MovingItemType movingItemType,
        bool movingIndexEqualsNIdx,
        bool isOverLap,
        bool waitingUseItemEmpty)
    {
        int nIdx = GridIndex(aCol, aRow, colCount);
        if (!IsValidPetBagIndex(nIdx))
            return SelectAction.None;

        if (rightButton)
            return SelectAction.None;   // 原文 13767-13770：右键直接 Exit

        if (!itemMoving)
        {
            if (targetHasItem)
                return SelectAction.PickUp;   // 原文只判是否为空，不看 movingIndexEqualsNIdx
            return SelectAction.None;
        }

        // 原文 13785：移动中的物品类型既不是背包也不是宠物包裹 → Exit
        if (movingItemType == MovingItemType.Other)
            return SelectAction.None;

        if (movingItemType == MovingItemType.BagItem)
        {
            // 原文 13788：正在等待使用的物品不为空则什么都不做
            return waitingUseItemEmpty ? SelectAction.BagToPetBag : SelectAction.None;
        }

        // mtGamePetBagItem
        if (targetHasItem)
            return isOverLap ? SelectAction.Overlap : SelectAction.Swap;

        return SelectAction.Place;
    }

    /// <summary>
    /// 原文 13728-13732（双击时的「用物品」判据）：
    ///   (StdMode in [0, 31, 93]) or ((StdMode = 49) and (Dura &gt;= DuraMax))
    /// 原文的括号写法使 `or` / `and` 的结合为 `(A) or ((B) and (C))`。
    /// </summary>
    public static bool IsUsablePetItem(int stdMode, int dura, int duraMax)
        => (stdMode == 0 || stdMode == 31 || stdMode == 93)
           || (stdMode == 49 && dura >= duraMax);

    /// <summary>
    /// 原文 13708 / 13722：`if keyvalue[VK_CONTROL] = $80 then` —— Ctrl 键按下时走「取出到背包」分支。
    /// VK_CONTROL = $11，GetKeyboardState 高位 = $80。
    /// </summary>
    public static bool IsControlDown(byte vkControlState) => vkControlState == 0x80;

    /// <summary>
    /// 原文 13836-13839：DSWPetsGridGridMouseMove 开头 —— 右键拖动时改以 **mbLeft** 调用
    /// DSWPetsGridGridSelect(Self, ACol, ARow, mbLeft, Shift)。
    /// </summary>
    public static bool ShouldRedirectToSelectOnMouseMove(bool shiftHasRight, bool itemMoving)
        => shiftHasRight && itemMoving;

    /// <summary>
    /// 原文 13844-13855 悬浮提示的两条路径：
    ///   (btSuspensionShowItem &gt; 0) or (g_ClientVersion &gt; cvSerial) → g_boShowBagInfo := False + 走 ShowMouseItemInfo；
    ///   否则 g_boShowBagInfo := True（不显示悬浮信息，改由背包信息面板显示）。
    /// g_ClientVersion &gt; cvSerial 是**严格大于**：cvSerial 本身走 else 分支。
    /// </summary>
    public static bool ShouldShowMouseItemInfo(int btSuspensionShowItem, int clientVersion)
        => btSuspensionShowItem > 0
           || clientVersion > TStateWindowsTitle.TStateWindowsClientVersion.cvSerial;
}
