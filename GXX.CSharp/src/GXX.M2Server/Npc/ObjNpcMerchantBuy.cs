// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：`TMerchant.ClientBuyItem`（原文 **3367-3688，323 行**）1:1。
//
// 这是 NPC 商店的**购买主流程**：倒序遍历 `m_GoodsList`（商品组表），按"物品名 + MakeIndex"
// 命中后走两条分支 —— 背包里已有可叠加同类（`OverLapItems` 命中）或需要新占一格。
//
// 依赖（本轮全部就绪）：
//   · `PlayObject.m_nGold` / `IsAddWeightAvailable` / `IsEnoughBag` / `AddItemToBag` / `SendAddItem`
//     （`Engine/PlayerSurface/**`；方案 A 后 `AddItemToBag` 收 `TUserItem?`）
//   · `TMerchant.GetUserPrice` / `GetUserItemPrice(ref TUserItem, bool)` / `GetItemPrice`
//     （本车道已覆盖的价格族）
//   · `VisibleItemLifecycleCore.CheckOverLapItem`（原文 M2Share.pas:11028-11032，已有 1:1 实现）
//
// ★ D35（值语义 vs 指针语义）在本方法里的具体处理：
//   原文 `UserItem := List20.Items[II]` 取到的是**指针**，`UserItem.Dura := x` 会就地改写商品表里那件；
//   托管 `m_GoodsList` 的组元素是 `List<object>`（装箱 `TUserItem`），**改完必须写回**
//   `List20[II] = UserItem;`（同 `Engine` 侧背包的 `SetBagItem` 契约）。见报告 §13.3 的 D35 条目。
//
// 原文在此方法内**两处逐字重复**了同一段"增加显示下一个物品"代码（:3486-3531 与 :3621-3666）——
// 本文件抽为私有方法 `BuildNextGoodsDisplay`（与 `ChargeCastleTax` 同一处理方式），
// 分支顺序与字面串**逐行一致**，仅消除字面重复。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TMerchant
{
    /// <summary>
    /// 原文 `procedure TMerchant.ClientBuyItem(PlayObject: TPlayObject; sItemName: string;
    /// nCount, nInt: Integer; IsFromTradingDlg: Boolean);`（ObjNpc.pas:3367-3688）。
    /// <para><b>照抄的原文细节 / 缺陷</b>：</para>
    /// <list type="bullet">
    /// <item>3386：`g_OnlineMsgControl.boDisableBuy` 为真**直接 Exit**（不发包）。</item>
    /// <item>3394-3395：`nCount &lt;= 0` → 强制为 1（**是 `&lt;=`，0 与负数同待遇**）。</item>
    /// <item>3396：`for I := m_GoodsList.Count - 1 downto 0` —— 商品组**倒序**遍历；
    ///   3398 每轮先判 `if bo29 or bo574 then Break`（`bo29` 是"已成交"标志，`bo574` 是封禁标志）。</item>
    /// <item>3421/3431/3538：`CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) }`
    ///   —— `{ }` 里的是**原文注释**（已注释掉的条件），照抄保留。</item>
    /// <item>3423-3426：叠加物品重量用 `StdItem.Weight * nCount div 10`（**整数除法**），
    ///   非叠加用 `StdItem.Weight * nCount`；两者都 `Min(..., High(Byte))`（= 255）。</item>
    /// <item>3433-3437：`nCount &lt; UserItem.Dura + 1` → 按比例重算价格（**实数除法**）；
    ///   `nCount &gt;` 时**把 nCount 夹到 `UserItem.Dura + 1`**（原文注释：防刷物品的修正）。</item>
    /// <item>3536：`if PlayObject.IsEnoughBag then` —— **无括号调用**（函数式），非布尔字段。</item>
    /// <item>3552-3558：新建叠加件时 **先记住 `CopyToUserItemFromName` 分配的 `MakeIndex`**，
    ///   再整条 `Move(UserItem^, OverLapItem^, SizeOf(TUserItem))` 覆盖，最后**把 MakeIndex 还原** ——
    ///   即"复制前者全部字段，但保留新分配的 MakeIndex"。照抄（托管结构赋值一步完成）。</item>
    /// <item>3603-3608：**只有 `ifUnknow` 一个分支**（`ifShopBuy`/其它来源**没有**分支处理）——
    ///   置 `ItemForm := ifShopBuy`、`sMakerName := PlayObject.m_sCharName`、`DateTime := Now()`。</item>
    /// <item>3614：`// List20.Delete(II);` 注释保留（此处**不**删商品，物品是"卖出去的那件仍留在商品表"语义）。</item>
    /// <item>3623：`UserItem := List20.Items[0]` —— **不判 nil**（原文此处空槽即 AV，照抄）。</item>
    /// <item>3684-3687：`n1C = 0` 才发 `RM_BUYITEM_SUCCESS`（`wParam = Integer(IsFromTradingDlg)`，False=0/True=1），
    ///   否则发 `RM_BUYITEM_FAIL`（`nParam1 = n1C`，取值 **2 或 3**）。</item>
    /// </list>
    /// </summary>
    public void ClientBuyItem(TPlayObject PlayObject, string sItemName, int nCount, int nInt,
        bool IsFromTradingDlg)
    {
        // 原文 3386-3387
        if (OnlineMsgControl.g_OnlineMsgControl.boDisableBuy)
            return;

        bool bo29 = false;
        int n1C = 1;
        // 原文 3390：`// I := 0;` —— 原文如此，保留
        string sSendText = "";
        int nItemCount = 0;
        bool boDelete = false;
        // 原文 3394-3395
        if (nCount <= 0)
            nCount = 1;
        // 原文 3396：for I := m_GoodsList.Count - 1 downto 0 do
        for (int I = m_GoodsList.Count - 1; I >= 0; I += -1)
        {
            // 原文 3398-3399
            if (bo29 || bo574)
                break;
            // 原文 3400
            List<object> List20 = (List<object>)m_GoodsList[I];
            // 原文 3401-3402
            if (List20 == null)
                continue;
            // 原文 3403-3404
            if (List20.Count <= 0)
                continue;
            // 原文 3405：for II := List20.Count - 1 downto 0 do
            for (int II = List20.Count - 1; II >= 0; II += -1)
            {
                // 原文 3407：UserItem := List20.Items[II];
                // ★ D35：组元素是装箱的 `TUserItem`（值语义）—— 取出来后凡有改动都要写回 `List20[II]`。
                if (List20[II] == null)
                    continue;
                TUserItem UserItem = (TUserItem)List20[II];
                // 原文 3408
                TStdItem? StdItem = NpcSeams.GetStdItem(UserItem.wIndex);
                // 原文 3409-3410
                if (StdItem == null)
                    continue;
                // 原文 3411-3415：取自定义物品名称
                string sUserItemName;
                if ((UserItem.GetBtValue(13) == 1) && (UserItem.NameStr != ""))
                    sUserItemName = UserItem.NameStr;                       // 原文 3413
                else
                    sUserItemName = NpcSeams.GetStdItemName(UserItem.wIndex); // 原文 3415
                // 原文 3416：if SameText(sUserItemName, sItemName) then
                if (string.Equals(sUserItemName, sItemName, StringComparison.OrdinalIgnoreCase))
                {
                    // 原文 3418
                    if (UserItem.MakeIndex == nInt)
                    {
                        // 原文 3420
                        int nWeight = StdItem.Value.Weight;
                        // 原文 3421：`if CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) } then`
                        if (CheckOverLap(StdItem.Value))
                        {
                            // 计算购买叠加物品
                            if (StdItem.Value.OverLap == 1)
                                nWeight = Math.Min(Math.Max(StdItem.Value.Weight * nCount / 10, StdItem.Value.Weight), 255);
                            else
                                nWeight = Math.Min(StdItem.Value.Weight * nCount, 255);
                        }
                        // 原文 3428
                        if (PlayObject.IsAddWeightAvailable(nWeight))
                        {
                            // 原文 3430
                            int nPrice = GetUserPrice(PlayObject, GetUserItemPrice(ref UserItem, false));
                            // 原文 3431：`if CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) } then`
                            if (CheckOverLap(StdItem.Value))
                            {
                                // 叠加物品 计算购买数量的价格
                                // 原文 3433-3434
                                if (nCount < UserItem.Dura + 1)
                                    nPrice = EnvirWalkDoorCore.DelphiRound(nCount * (nPrice / (double)(UserItem.Dura + 1)));
                                // 修正购买叠加物品，数量大于可买数量时，可以刷物品 2019-11-26 00:44:40
                                // 原文 3436-3437
                                else if (nCount > UserItem.Dura + 1)
                                    nCount = UserItem.Dura + 1;
                            }
                            // 原文 3439
                            if ((PlayObject.m_nGold >= nPrice) && (nPrice > 0))
                            {
                                // 原文 3441
                                TUserItem? OverLapItem = NpcSeams.OverLapItems(
                                    PlayObject, StdItem.Value, (ushort)(nCount - 1));
                                // 原文 3442
                                if (OverLapItem != null)
                                {
                                    // 包裹里有可以叠加的物品
                                    // 原文 3444
                                    PlayObject.m_nGold -= (uint)nPrice;
                                    // 原文 3445-3455（与 :3588-3598 逐字重复的税收块）
                                    ChargeCastleTax(nPrice);
                                    // 原文 3456-3457
                                    int nDura = UserItem.Dura + 1;
                                    nDura = nDura - nCount;
                                    // 原文 3458
                                    if (nDura <= 0)
                                    {
                                        // 原文 3460-3463
                                        nCount = 0;
                                        List20.RemoveAt(II);
                                        // 原文 3462：Dispose(UserItem) —— 托管侧由 GC 负责
                                        boDelete = true;
                                    }
                                    else
                                    {
                                        // 原文 3467-3469
                                        UserItem.Dura = (ushort)(nDura - 1);
                                        nCount = nDura;
                                        nItemCount = UserItem.Dura + 1;
                                        // 原文 3470-3473 的注释块（旧价格算法）—— 原文如此，保留
                                        // ★ D35 写回：原文此处通过指针就地改写商品表里那件
                                        List20[II] = UserItem;
                                    }
                                    // 原文 3475
                                    UserItem = OverLapItem.Value;
                                    // 原文 3476-3480
                                    if (List20.Count <= 0)
                                    {
                                        // 原文 3478-3479：FreeAndNil(List20); m_GoodsList.Delete(I);
                                        m_GoodsList.RemoveAt(I);
                                    }
                                    // 原文 3481-3484
                                    if (StdItem.Value.NeedIdentify == 1)
                                    {
                                        NpcSeams.AddGameDataLog(ObjNpcConst.LOG_ItemBuy, ObjNpcConst.LOG_GoldChange,
                                            PlayObject, StdItem.Value.NameStr, UserItem.MakeIndex, m_sCharName,
                                            (int)PlayObject.m_nGold, -nPrice,
                                            "NPC购买 [" + ObjNpcConst.sSTRING_GOLDNAME + "]");
                                    }
                                    // 增加显示下一个物品（原文 3486-3531）
                                    if (boDelete && (List20 != null) && (List20.Count > 0))
                                    {
                                        // 原文 3488 的 MainOutMessage 调试行（注释）—— 原文如此，保留
                                        BuildNextGoodsDisplay(PlayObject, List20, sItemName, nPrice, ref nCount,
                                            out sSendText);
                                    }
                                    // 原文 3532-3533
                                    n1C = 0;
                                    break;
                                }
                                else
                                {
                                    // 原文 3535：`{ // if OverLapItem <> nil then begin 包裹里没有可以叠加的物品 }`
                                    // 原文 3536：`if PlayObject.IsEnoughBag then`（无括号调用）
                                    if (PlayObject.IsEnoughBag())
                                    {
                                        // 原文 3538
                                        if (CheckOverLap(StdItem.Value))
                                        {
                                            // 原文 3540-3541
                                            int nDura = UserItem.Dura + 1;
                                            nDura = nDura - nCount;
                                            // 原文 3542
                                            if (nDura <= 0)
                                            {
                                                // 购买全部叠加物品
                                                // 原文 3544：`// nCount := nCount - nDura;` 注释保留
                                                nCount = 0;
                                                // 原文 3546-3548
                                                PlayObject.AddItemToBag(UserItem);
                                                List20.RemoveAt(II);
                                                boDelete = true;
                                            }
                                            else
                                            {
                                                // 购买部分叠加物品
                                                // 原文 3552：New(OverLapItem)
                                                TUserItem OverLapItem2 = default;
                                                // 原文 3553
                                                if (NpcSeams.CopyToUserItemFromName(StdItem.Value.NameStr, ref OverLapItem2))
                                                {
                                                    // 原文 3555：nMakeIndex := OverLapItem.MakeIndex;
                                                    int nMakeIndex = OverLapItem2.MakeIndex;
                                                    // 原文 3556：Move(UserItem^, OverLapItem^, SizeOf(TUserItem));
                                                    // 托管结构赋值即 1:1（整条记录逐字段复制）
                                                    OverLapItem2 = UserItem;
                                                    // 原文 3557：`// OverLapItem^ := UserItem^;` 注释保留
                                                    // 原文 3558-3562
                                                    OverLapItem2.MakeIndex = nMakeIndex;
                                                    OverLapItem2.Dura = (ushort)(nCount - 1);
                                                    UserItem.Dura = (ushort)(nDura - 1);
                                                    nCount = nDura;
                                                    nItemCount = UserItem.Dura + 1;
                                                    // 原文 3563-3567 的注释块（旧价格算法）—— 原文如此，保留
                                                    // 原文 3568
                                                    PlayObject.AddItemToBag(OverLapItem2);
                                                    // 原文 3569 的 MainOutMessage 调试行（注释）—— 原文如此，保留
                                                    // 原文 3570
                                                    UserItem = OverLapItem2;
                                                    // ★ D35 写回（原文 :3560 通过指针就地改写商品表里那件）
                                                    List20[II] = UserItem;
                                                }
                                                else
                                                {
                                                    // 原文 3574：Dispose(OverLapItem) —— 托管侧由 GC 负责
                                                    // 原文 3575
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // 原文 3581-3584
                                            nCount = 0;
                                            PlayObject.AddItemToBag(UserItem);
                                            List20.RemoveAt(II);
                                            boDelete = true;
                                        }
                                        // 原文 3586：`// if PlayObject.AddItemToBag(UserItem) then begin` 注释保留
                                        // 原文 3587
                                        PlayObject.m_nGold -= (uint)nPrice;
                                        // 原文 3588-3598（税收块）
                                        ChargeCastleTax(nPrice);
                                        // 原文 3599-3602：旧版税收块（整段 `{ }` 注释）—— 原文如此，保留
                                        // 原文 3603-3608：**只有 ifUnknow 一个分支**
                                        if (UserItem.ItemFrom.ItemForm == TItemFormType.ifUnknow)
                                        {
                                            UserItem.ItemFrom.ItemForm = TItemFormType.ifShopBuy;
                                            UserItem.ItemFrom.MakerName = PlayObject.m_sCharName;
                                            UserItem.ItemFrom.DateTime = DelphiRTL.Now().ToOADate();
                                        }
                                        // ★★ D36（D35 的**具体后果**，本方法特有，已在报告登记）：
                                        //   原文 3546/3568/3582 的 `AddItemToBag(UserItem)` 加的是**指针**，
                                        //   所以上面 3603-3608 对 `UserItem.ItemFrom` 的修改**自动**反映到背包那件；
                                        //   托管是**值复制** → 入包发生在 3603 之前，改动**不会**进背包。
                                        //   故必须显式把改动同步回**刚追加的那一格**（`AddItemToBag` 是 Append 语义
                                        //   → 下标恒为 `Bag.Count - 1`，三条子分支都成立）。
                                        PlayObject.SetBagItem(PlayObject.Bag.Count - 1, UserItem);
                                        // 原文 3609
                                        PlayObject.SendAddItem(UserItem);
                                        // 原文 3610-3613
                                        if (StdItem.Value.NeedIdentify == 1)
                                        {
                                            NpcSeams.AddGameDataLog(ObjNpcConst.LOG_ItemBuy, ObjNpcConst.LOG_GoldChange,
                                                PlayObject, StdItem.Value.NameStr, UserItem.MakeIndex, m_sCharName,
                                                (int)PlayObject.m_nGold, -nPrice,
                                                "NPC购买 [" + ObjNpcConst.sSTRING_GOLDNAME + "]");
                                        }
                                        // 原文 3614：`// List20.Delete(II);` 注释保留
                                        // ★ D35 写回：`ItemFrom` 与 `Dura` 的改动须落回商品表
                                        if (II < List20.Count)
                                            List20[II] = UserItem;
                                        // 原文 3615-3619
                                        if ((List20 != null) && (List20.Count <= 0))
                                        {
                                            // 原文 3617-3618：FreeAndNil(List20); m_GoodsList.Delete(I);
                                            m_GoodsList.RemoveAt(I);
                                        }
                                        // 增加显示下一个物品（原文 3621-3666）
                                        if (boDelete && (List20 != null) && (List20.Count > 0))
                                        {
                                            BuildNextGoodsDisplay(PlayObject, List20, sItemName, nPrice, ref nCount,
                                                out sSendText);
                                        }
                                        // 原文 3667-3668
                                        n1C = 0;
                                        break;
                                    }
                                    else
                                        n1C = 2; // 原文 3671：`// if PlayObject.IsEnoughBag then begin`
                                }
                            }
                            else
                                n1C = 3;   // 原文 3674
                        }
                        else
                            n1C = 2;       // 原文 3677：`// 004A2639`
                        // 原文 3678
                        bo29 = true;
                    }
                }
            }
        }
        // for（原文 3683）
        // 原文 3684-3687
        if (n1C == 0)
            PlayObject.SendTo(this, Grobal2Const.RM_BUYITEM_SUCCESS, IsFromTradingDlg ? 1 : 0, PlayObject.m_nGold,
                nInt, nItemCount, sSendText);
        else
            PlayObject.SendTo(this, Grobal2Const.RM_BUYITEM_FAIL, 0, n1C, 0, 0, "");
    }

    /// <summary>
    /// 原文在 `ClientBuyItem` 内**两处逐字重复**的"增加显示下一个物品"块
    /// （ObjNpc.pas:3486-3531 与 :3621-3666，两段完全相同）。
    /// <para>抽为私有方法只为消除字面重复；分支顺序与字面串**逐行一致**。</para>
    /// <para><b>照抄的原文细节</b>：3659/3524 的 `nSubMenu = 0` 分支用**外层**的 `nPrice`
    /// （不是本次重算的 `nNewPrice`）—— 照抄；3662/3527 的 `nSubMenu = 1` 分支用 `nNewPrice`。
    /// 3662 的 `UserItem.Dura` 是**显示用的当前持久值**。</para>
    /// <para>⚠ 3623/3489：`UserItem := List20.Items[0]` **不判 nil**（原文空槽即 AV，照抄）。</para>
    /// </summary>
    private void BuildNextGoodsDisplay(TPlayObject PlayObject, List<object> List20, string sItemName,
        int nPrice, ref int nCount, out string sSendText)
    {
        sSendText = "";
        // 原文 3489/3623
        TUserItem UserItem = (TUserItem)List20[0]!;
        // 原文 3490/3625
        TStdItem? StdItem = NpcSeams.GetStdItem(UserItem.wIndex);
        // 原文 3491/3626
        if (StdItem != null)
        {
            // 原文 3493/3628
            string sUserItemName = "";
            // 原文 3494-3495 / 3629-3630
            if (UserItem.GetBtValue(13) == 1)
                sUserItemName = UserItem.NameStr;
            // 原文 3496-3497 / 3631-3632
            if (sUserItemName == "")
                sUserItemName = StdItem.Value.NameStr;
            // 原文 3498/3633
            if (ObjNpcText.CompareText(sUserItemName, sItemName) == 0)
            {
                // 原文 3500/3635
                int nNewPrice = GetItemPrice(UserItem.wIndex);
                // 原文 3501/3636
                if (CheckOverLap(StdItem.Value))
                {
                    // 原文 3503-3504 / 3638-3639
                    nCount = UserItem.Dura + 1;
                    nNewPrice = nNewPrice * nCount;
                }
                else
                    nCount = 1;   // 原文 3507/3642
                // 原文 3508/3643
                nNewPrice = GetUserPrice(PlayObject, nNewPrice);
                // 原文 3509 的旧写法（注释）—— 原文如此，保留
                // 原文 3510/3644
                int nStock = List20.Count;
                int nSubMenu;
                // 原文 3511-3519 / 3645-3653
                if ((StdItem.Value.StdMode <= 4) || (StdItem.Value.StdMode == 42) || (StdItem.Value.StdMode == 31))
                {
                    nSubMenu = 0;
                    // 原文 3514/3648：这一类"库存"字段被复用成 MakeIndex（原文如此）
                    nStock = UserItem.MakeIndex;
                }
                else
                {
                    nSubMenu = 1;
                }
                // 原文 3654：`// MainOutMessage(IntToStr(nSubMenu));` 注释保留
                // 原文 3520/3655
                if (CheckOverLap(StdItem.Value) || (nSubMenu == 0))
                {
                    if (nSubMenu == 0)
                    {
                        // 原文 3524/3659
                        sSendText = sUserItemName + "/" + DelphiRTL.IntToStr(nSubMenu) + "/"
                            + DelphiRTL.IntToStr(nPrice) + "/" + DelphiRTL.IntToStr(nStock) + "/"
                            + DelphiRTL.IntToStr(nCount);
                    }
                    else
                    {
                        // 原文 3527/3662
                        sSendText = "+" + sUserItemName + "/" + DelphiRTL.IntToStr(nNewPrice) + "/"
                            + DelphiRTL.IntToStr(UserItem.MakeIndex) + "/"
                            + DelphiRTL.IntToStr(UserItem.Dura) + "/" + DelphiRTL.IntToStr(nCount);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 原文 `function CheckOverLapItem(StdItem: pTStdItem): Boolean;`
    /// （M2Share.pas:11028-11032）：`(OverLap &gt; 0) and (StdMode in [0,2,3,31,40,41,42,46,47]) and (DuraMax &gt; 1)`。
    /// 复用既有 1:1 实现 `VisibleItemLifecycleCore.CheckOverLapItem(overlap, stdMode, duraMax)`。
    /// </summary>
    private static bool CheckOverLap(TStdItem stdItem)
        => VisibleItemLifecycleCore.CheckOverLapItem(stdItem.OverLap, stdItem.StdMode, stdItem.DuraMax);

    /// <summary>
    /// 原文在购买/卖出成功后重复出现的**税收块**（本方法内 :3445-3455 与 :3588-3598 逐字相同；
    /// `ClientSellItem` 内亦同型）：
    /// ```
    ///   if m_boCastle or g_Config.boGetAllNpcTax then
    ///   begin
    ///     if m_Castle &lt;&gt; nil then TUserCastle(m_Castle).IncRateGold(nPrice)
    ///     else if g_Config.boGetAllNpcTax then g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice);
    ///   end;
    /// ```
    /// <para>⚠ **D33 同型**：管理器分支传的是 `nUpgradeWeaponPrice` 而**不是**本次成交价 `nPrice`
    /// （照抄，不"修正"）。</para>
    /// </summary>
    private void ChargeCastleTax(int nPrice)
    {
        if (m_boCastle || M2Config.boGetAllNpcTax)
        {
            object castle = NpcSeams.GetNpcCastle(this);
            if (castle != null)
            {
                NpcSeams.IncRateGoldOnCastle(castle, nPrice);
            }
            else if (M2Config.boGetAllNpcTax)
            {
                // 原文 3453/3596：传的是 **nUpgradeWeaponPrice**（D33 同型，照抄）
                NpcSeams.IncRateGoldOnCastleManager(M2Config.nUpgradeWeaponPrice);
            }
        }
    }
}
