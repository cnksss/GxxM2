// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：TMerchant（NPC 商人）**价格/货物查询**方法族 1:1 移植。
//   · AddItemPrice                 1446-1455
//   · CheckItemPrice               1457-1486
//   · GetRefillList                1488-1510
//   · CheckItemType                1630-1643
//   · GetItemPrice                 1645-1672
//   · GetUserPrice                 2052-2085
//   · ClearExpreUpgradeListData    3160-3178
//   · GetUserItemPrice             3272-3365
//   · GetSellItemPrice             3793-3796
//
// 复用的既有实现（不复制第二份）：
//   · `Round`        → `GXX.M2Server.EnvirWalkDoorCore.DelphiRound`（EnvirWalkDoorCore.cs:1064，
//                      `(int)Math.Round(v, MidpointRounding.ToEven)` = Delphi 银行家舍入）
//   · `CheckOverLapItem(StdItem)` → `GXX.M2Server.VisibleItemLifecycleCore.CheckOverLapItem(
//                      OverLap, StdMode, DuraMax)`（VisibleItemLifecycleCore.cs:71，原文 M2Share.pas:11028-11032）
//   · `g_Config.nCastleMemberPriceRate` / `boSellItemToNpcShopNoCalcAddProperty` /
//     `nClearExpireUpgradeWeaponDays` → `GXX.M2Server.Engine.M2Config` 同名字段
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
    /// 原文 `procedure AddItemPrice(nIndex: Integer; nPrice: Integer);`（ObjNpc.pas:1446-1455）。
    /// 追加一条价格记录（**不去重**），随后落库。
    /// </summary>
    public void AddItemPrice(int nIndex, int nPrice)
    {
        TItemPrice ItemPrice = new();
        ItemPrice.wIndex = (ushort)nIndex;
        ItemPrice.nPrice = nPrice;
        m_ItemPriceList.Add(ItemPrice);
        NpcSeams.SaveGoodPriceRecord(this, m_sScript + '-' + m_sMapName);
    }

    /// <summary>
    /// 原文 `procedure CheckItemPrice(nIndex: Integer);`（ObjNpc.pas:1457-1486）。
    /// <para>已存在则**直接 Exit（不改价）**；否则查 `UserEngine.GetStdItem`，非 nil 时按
    /// `Round(StdItem.Price * 1.1)` 追加。原文 1470-1477 的旧涨价算法整段注释保留。</para>
    /// </summary>
    public void CheckItemPrice(int nIndex)
    {
        for (int I = 0; I <= m_ItemPriceList.Count - 1; I++)
        {
            TItemPrice ItemPrice = (TItemPrice)m_ItemPriceList[I];
            if (ItemPrice == null)
                continue;
            if (ItemPrice.wIndex == nIndex)
            {
                // 原文 1470-1477：旧算法（Round(n10 * 1.1) 或 Inc(n10)）整段被 `{ }` 注释 —— 原文如此，保留。
                return;
            }
        }
        TStdItem? StdItem = NpcSeams.GetStdItem(nIndex);
        if (StdItem != null)
        {
            AddItemPrice(nIndex, EnvirWalkDoorCore.DelphiRound(StdItem.Value.Price * 1.1));
        }
    }

    /// <summary>
    /// 原文 `function GetRefillList(nIndex: Integer): TList;`（ObjNpc.pas:1488-1510）。
    /// <para>`nIndex &lt;= 0` 直接返回 nil；在 `m_GoodsList` 中按"每组第 0 个物品的 wIndex"匹配，
    /// 命中即返回该组（**只看第 0 个元素**，不看整组）。</para>
    /// </summary>
    public List<object> GetRefillList(int nIndex)
    {
        List<object> Result = null;
        if (nIndex <= 0)
            return Result;
        for (int I = 0; I <= m_GoodsList.Count - 1; I++)
        {
            List<object> List = (List<object>)m_GoodsList[I];
            if (List == null)
                continue;
            if (List.Count > 0)
            {
                if (((TUserItem)List[0]).wIndex == nIndex)
                {
                    Result = List;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function CheckItemType(nStdMode: Integer): Boolean;`（ObjNpc.pas:1630-1643）。
    /// `m_ItemTypeList` 存的是**装箱的 Integer**（原文 1637 显式 `Integer(...)` 转换）。
    /// </summary>
    public bool CheckItemType(int nStdMode)
    {
        bool Result = false;
        for (int I = 0; I <= m_ItemTypeList.Count - 1; I++)
        {
            if (Convert.ToInt32(m_ItemTypeList[I]) == nStdMode)
            {
                Result = true;
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function GetItemPrice(nIndex: Integer): Integer;`（ObjNpc.pas:1645-1672）。
    /// <para>先查价格表；未命中（`Result &lt; 0`）再查标准物品，
    /// **且只有 `CheckItemType(StdItem.StdMode)` 为真才取 `StdItem.Price`**，
    /// 否则保持 -1（哨兵值，与"价格为 0 的物品"不同 —— 差异断言）。</para>
    /// </summary>
    public int GetItemPrice(int nIndex)
    {
        int Result = -1;
        for (int I = 0; I <= m_ItemPriceList.Count - 1; I++)
        {
            TItemPrice ItemPrice = (TItemPrice)m_ItemPriceList[I];
            if (ItemPrice == null)
                continue;
            if (ItemPrice.wIndex == nIndex)
            {
                Result = ItemPrice.nPrice;
                break;
            }
        }
        if (Result < 0)
        {
            TStdItem? StdItem = NpcSeams.GetStdItem(nIndex);
            if (StdItem != null)
            {
                if (CheckItemType(StdItem.Value.StdMode))
                    Result = StdItem.Value.Price;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function GetUserPrice(PlayObject: TPlayObject; nPrice: Integer): Integer; // 0049F6E0`
    /// （ObjNpc.pas:2052-2085）。
    /// <para><b>照抄的原文细节</b>：原文 2056-2067 的旧版整段注释保留；
    /// 生效版里 `n14 := Max(60, Round(m_nPriceRate * (g_Config.nCastleMemberPriceRate / 100)))`
    /// —— 括号内是**整数除法**（两个 Integer），故 `80/100 = 0`，`n14` 恒为 `Max(60, 0) = 60`；
    /// 而 `Result := Round(nPrice / 100 * n14)` 中 `nPrice / 100` 是**实数除法**（Delphi `/` 语义）。</para>
    /// </summary>
    public int GetUserPrice(TPlayObject PlayObject, int nPrice)
    {
        int Result;
        // 原文 2056-2067：旧版（UserCastle.IsMasterGuild + 浮点字面量 8.0e-1）整段被 `{ }` 注释 —— 原文如此，保留。
        if (m_boCastle)
        {
            // 原文 2070：`// if UserCastle.IsMasterGuild(TGuild(PlayObject.m_MyGuild)) then begin` 注释保留
            object castle = NpcSeams.GetNpcCastle(this);
            if ((castle != null) && NpcSeams.IsMasterGuild(castle, PlayObject))
            {
                int n14 = Math.Max(60, EnvirWalkDoorCore.DelphiRound(m_nPriceRate * (M2Config.nCastleMemberPriceRate / 100))); // 80%
                Result = EnvirWalkDoorCore.DelphiRound(nPrice / 100.0 * n14); // 100
            }
            else
            {
                Result = EnvirWalkDoorCore.DelphiRound(nPrice / 100.0 * m_nPriceRate);
            }
        }
        else
        {
            Result = EnvirWalkDoorCore.DelphiRound(nPrice / 100.0 * m_nPriceRate);
        }
        return Result;
    }

    /// <summary>
    /// 原文 `procedure ClearExpreUpgradeListData; // 004A01A0`（ObjNpc.pas:3160-3178）。
    /// <para><b>照抄的原文细节</b>：3167-3168 的 `if Count &lt;= 0 then Break` 在倒序循环里
    /// **永不为真**（冗余守卫，保留）；天数比较用 `Round(Now - dtTime)` 取整（Delphi TDateTime 差 = 天）。</para>
    /// </summary>
    public void ClearExpreUpgradeListData()
    {
        for (int I = m_UpgradeWeaponList.Count - 1; I >= 0; I += -1)
        {
            if (m_UpgradeWeaponList.Count <= 0)
                break;
            TUpgradeInfo UpgradeInfo = (TUpgradeInfo)m_UpgradeWeaponList[I];
            if (UpgradeInfo == null)
                continue;
            if (EnvirWalkDoorCore.DelphiRound((DelphiRTL.Now() - UpgradeInfo.dtTime).TotalDays)
                >= M2Config.nClearExpireUpgradeWeaponDays)
            {
                // 原文 3174：Dispose(UpgradeInfo) —— 托管侧由 GC 负责
                m_UpgradeWeaponList.RemoveAt(I);
            }
        }
    }

    /// <summary>
    /// 原文 `function GetUserItemPrice(UserItem: pTUserItem; IsSellToNpc: Boolean): Integer;`
    /// （ObjNpc.pas:3272-3365）。
    /// <para><b>托管侧签名偏差（必需）</b>：原文第一参是 `pTUserItem`（指针），
    /// 3302-3303 会**就地改写** `UserItem.DuraMax`；托管侧 `TUserItem` 是值类型结构，
    /// 故签名改为 `ref TUserItem` 以保住该就地写回语义。</para>
    /// <para><b>照抄的原文缺陷</b>：3323 `if (nC &lt;&gt; 4) or (nC &lt;&gt; 9)` 是**恒真**条件
    /// （同一变量不可能同时等于又不等），按原文保留 —— 改写成 `and` 会改变行为。</para>
    /// <para>3321 的 `StdMode in [5, 6, 68, 69]` 组内对第 6 槽（`nC = 6`）有独立的
    /// `- 10) * 2` 加成；其余 `StdMode` 走 3340 的 `Inc(n14, btValue[nC])`；循环固定跑 8 轮。</para>
    /// </summary>
    public int GetUserItemPrice(ref TUserItem UserItem, bool IsSellToNpc)
    {
        int n10;
        TStdItem? StdItem = null;
        double n20;
        int nC;
        int n14;
        n10 = GetItemPrice(UserItem.wIndex);
        if (n10 > 0)
        {
            StdItem = NpcSeams.GetStdItem(UserItem.wIndex);
            if ((StdItem != null) && (StdItem.Value.StdMode > 4) && (StdItem.Value.DuraMax > 0)
                && (UserItem.DuraMax > 0))
            {
                if (StdItem.Value.StdMode == 40)
                {
                    // 肉
                    // 原文 3289：`// n10 := Max(2, Round(n10 * UserItem.Dura / UserItem.DuraMax * 100));` 注释保留
                    // 原文 3290-3295：旧算法整段被 `{ }` 注释 —— 原文如此，保留。
                    n20 = (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
                    n10 = Math.Max(2, EnvirWalkDoorCore.DelphiRound(n10 - n20));
                }
                if (StdItem.Value.StdMode == 43)
                {
                    // 原文 3301：`// n10 := Max(2, Round(n10 * UserItem.Dura / UserItem.DuraMax * 100));` 注释保留
                    if (UserItem.DuraMax < 10000)
                        UserItem.DuraMax = 10000;
                    // 原文 3304-3309：旧算法整段被 `{ }` 注释 —— 原文如此，保留。
                    n20 = (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
                    n10 = Math.Max(2, EnvirWalkDoorCore.DelphiRound(n10 - n20));
                }
                if (StdItem.Value.StdMode > 4)
                {
                    if ((!IsSellToNpc) || (!M2Config.boSellItemToNpcShopNoCalcAddProperty))
                    {
                        n14 = 0;
                        nC = 0;
                        while (true)
                        {
                            // 原文 3323：`if (nC <> 4) or (nC <> 9)` —— 恒真（原文缺陷，照抄）
                            if (StdItem.Value.StdMode is 5 or 6 or 68 or 69)
                            {
                                if ((nC != 4) || (nC != 9))
                                {
                                    if (nC == 6)
                                    {
                                        if (UserItem.GetBtValue(nC) > 10)
                                        {
                                            n14 = n14 + (UserItem.GetBtValue(nC) - 10) * 2;
                                        }
                                    }
                                    else
                                    {
                                        n14 = n14 + UserItem.GetBtValue(nC);
                                    }
                                }
                            }
                            else
                            {
                                n14 += UserItem.GetBtValue(nC);
                            }
                            nC++;
                            if (nC >= 8)
                                break;
                        }
                        if (n14 > 0)
                        {
                            n10 = n10 + EnvirWalkDoorCore.DelphiRound(n10 * n14 / 200.0); // 修复极品装备价格币普通装备低的问题
                            // 原文 3349：`// n10 := n10 div 5 * n14;` 注释保留
                        }
                    }
                    // 叠加物品的装备价格计算错误 chongchong 2014-05-22
                    if (!VisibleItemLifecycleCore.CheckOverLapItem(
                            StdItem.Value.OverLap, StdItem.Value.StdMode, StdItem.Value.DuraMax))
                    {
                        n10 = EnvirWalkDoorCore.DelphiRound(n10 / (double)StdItem.Value.DuraMax * UserItem.DuraMax);
                        n20 = (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
                        n10 = Math.Max(2, EnvirWalkDoorCore.DelphiRound(n10 - n20));
                    }
                }
            }
        }
        // 叠加物品
        if ((StdItem != null) && VisibleItemLifecycleCore.CheckOverLapItem(
                StdItem.Value.OverLap, StdItem.Value.StdMode, StdItem.Value.DuraMax))
            n10 = n10 * (UserItem.Dura + 1);
        return n10;
    }

    /// <summary>
    /// 原文 `function GetSellItemPrice(nPrice: Integer): Integer;`（ObjNpc.pas:3793-3796）。
    /// 半价（`Round` 银行家舍入）。
    /// </summary>
    public int GetSellItemPrice(int nPrice)
    {
        return EnvirWalkDoorCore.DelphiRound(nPrice / 2.0);
    }
}
