// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：`TMerchant.UpgradeWapon` 的**外层体**（原文 **1830-1901，72 行**）1:1。
//   嵌套过程 `sub_4A0218`(1686-1828) 已在切片 8 覆盖（`ObjNpcMerchant.cs`）；
//   本文件补齐外层：查重 → 扣费 → 摘除武器 → 生成/入列升级记录 → 跳转提示标签。
//
// 前置核实（本轮已完成）：
//   ① `m_UseItems[U_WEAPON]` 是**读写**：`Engine/RecalcChain.cs:101` 目前是
//      **视图类型** `TUserItemView?[]`，而原文 `ObjBase.pas:882 m_UseItems: THumanUseItems`
//      是 `array[..] of TUserItem`（**权威值类型数组**）—— 与 `m_ItemList` 同型缺陷。
//      本车道**不改他人文件**，故读写各走一个按权威侧定名的接缝：
//      `NpcSeams.GetUseItemsWeapon`（读，已存在）/ `NpcSeams.SetUseItemsWeapon`（写回，本轮新增）。
//      按偏差 **D35** 的调用方契约："取出 → 改 → 写回"（原文 :1886 是就地改 `wIndex := 0`）。
//   ② `GotoLable`(9263-9574) 未移植 → 走 Engine 既有接缝 `PlayerSurfaceNpcSeams.GotoLable`。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TMerchant
{
    /// <summary>
    /// 原文 `procedure TMerchant.UpgradeWapon(User: TPlayObject); // 004A0920`（ObjNpc.pas:1830-1901）。
    /// <para><b>照抄的原文细节 / 缺陷</b>：</para>
    /// <list type="bullet">
    /// <item>1839-1849：先线性扫 `m_UpgradeWeaponList`，**同一角色已在升级中**就用
    ///   `sNF_Upgradeing` 标签 `GotoLable` 并 **`Exit`**（不扣费、不进第二步）。</item>
    /// <item>1850 的三合一门：`武器格 wIndex &lt;&gt; 0` **且** `m_nGold &gt;= nUpgradeWeaponPrice`
    ///   **且** `CheckItems(g_Config.sBlackStone) &lt;&gt; nil`。</item>
    /// <item>1854：**禁升级规则**用 `g_ItemRules.Get(wIndex, **17**)`（不是 4/8）——
    ///   且前两个条件 `StdItem &lt;&gt; nil`、`Length(StdItem.Name) &gt; 0` 与之**与**在一起；
    ///   命中时用 `AnsiReplaceStr(g_sCannotUpgradeWeapon, '%Item', StdItem.Name)` 拼提示
    ///   （`%Item` **带百分号**，不是 `%s`），并**用第二个 SysMsg 重载**（FColor/BColor 两个整型色值），随后 `Exit`。</item>
    /// <item>1860-1861：`OldGold := User.m_nGold;` **先存旧值**再 `DecGold`（1864 的日志用 `(当前, 旧值)` 这一对）。</item>
    /// <item>1862：`g_boGameLogGold` 为真才写"扣费"日志（`Data1 = 扣费后余额`、`Data2 = OldGold`）；
    ///   描述串是 `'扣费:' + IntToStr(nUpgradeWeaponPrice)`。</item>
    /// <item>1866-1876：税收块 —— **注意与 `ClientBuyItem`/`ClientSellItem` 的 D33 同型**：
    ///   两个分支传的都是 `g_Config.nUpgradeWeaponPrice`（**本来就该是这个**，此处**不是**缺陷）。</item>
    /// <item>1880：`UpgradeInfo.UserItem := User.m_UseItems[U_WEAPON];` —— **在清空前**先把整件武器存进升级记录。</item>
    /// <item>1885-1886：`SendDelItem(武器)` 后 **`wIndex := 0` 清空武器格**（D35：托管须写回）。</item>
    /// <item>1887-1889：`RecalcAbilitys()` → `FeatureChanged()` → `SendMsg(RM_ABILITY)`（**这个顺序**）。</item>
    /// <item>1890：`sub_4A0218(User.m_ItemList, ...)` —— 4 个 `var Byte` 出参写回 `UpgradeInfo` 的 btDc/btSc/btMc/btDura。</item>
    /// <item>1891-1892：`dtTime := Now()` / `dwGetBackTick := MyGetTickCount()`。</item>
    /// <item>1897-1900：按 `bo0D` 跳 `sNF_UpgradeOK` / `sNF_UpgradeFail`（**这是本过程的出口**，无早退）。</item>
    /// </list>
    /// </summary>
    public void UpgradeWapon(TPlayObject User)
    {
        bool bo0D = false;
        // 原文 1839-1849：同一角色已在升级中 → 提示并退出
        for (int I = 0; I <= m_UpgradeWeaponList.Count - 1; I++)
        {
            TUpgradeInfo UpgradeInfo = (TUpgradeInfo)m_UpgradeWeaponList[I];
            if (UpgradeInfo == null)
                continue;
            if (UpgradeInfo.sUserName == User.m_sCharName)
            {
                // 原文 1846：GotoLable(User, sNF_Upgradeing, False)
                PlayerSurfaceNpcSeams.GotoLable(this, User, ObjNpcConst.sNF_Upgradeing, false);
                return;
            }
        }
        // 原文 1850：三合一门
        // ★ D35：先"取出"武器（原文此处是解引用指针；托管经接缝取权威记录）
        TUserItem Weapon = NpcSeams.GetUseItemsWeapon(User);
        if ((Weapon.wIndex != 0) && (User.m_nGold >= (uint)M2Config.nUpgradeWeaponPrice)
            && (User.CheckItems(NpcSeams.sBlackStone, out _) != -1))
        {
            // 原文 1852
            TStdItem? StdItem = NpcSeams.GetStdItem(Weapon.wIndex);
            // TODO -ochongchong -c新增 : 物品规则 - 禁止升级【2013-07-27】
            // 原文 1854：`g_ItemRules.Get(User.m_UseItems[U_WEAPON].wIndex, 17)`
            if ((StdItem != null) && (StdItem.Value.NameStr.Length > 0)
                && NpcSeams.GetItemRule(Weapon.wIndex, 17))
            {
                // 原文 1856：`AnsiReplaceStr(g_sCannotUpgradeWeapon, '%Item', StdItem.Name)`
                string Msg = HUtil32.ReplaceStr(NpcSeams.g_sCannotUpgradeWeapon, "%Item", StdItem.Value.NameStr);
                // 原文 1857：**第二个 SysMsg 重载**（FColor / BColor 两个整型色值）
                NpcSeams.SysMsgFB(User, Msg, M2Config.btRedMsgFColor, M2Config.btRedMsgBColor, TMsgType.t_Hint);
                return;
            }
            // 原文 1860-1861
            uint OldGold = User.m_nGold;
            User.DecGold((uint)M2Config.nUpgradeWeaponPrice);
            // 原文 1862-1865
            if (NpcSeams.g_boGameLogGold)
            {
                NpcSeams.AddGameDataLog(ObjNpcConst.LOG_ItemUpgrade, ObjNpcConst.LOG_GoldChange, User,
                    ObjNpcConst.sSTRING_GOLDNAME, 0, m_sCharName, (int)User.m_nGold, (int)OldGold,
                    "扣费:" + DelphiRTL.IntToStr(M2Config.nUpgradeWeaponPrice));
            }
            // 原文 1866-1876：税收块（两个分支都传 nUpgradeWeaponPrice —— 此处**不是** D33）
            if (m_boCastle || M2Config.boGetAllNpcTax)
            {
                object castle = NpcSeams.GetNpcCastle(this);
                if (castle != null)
                {
                    NpcSeams.IncRateGoldOnCastle(castle, M2Config.nUpgradeWeaponPrice);
                }
                else if (M2Config.boGetAllNpcTax)
                {
                    NpcSeams.IncRateGoldOnCastleManager(M2Config.nUpgradeWeaponPrice);
                }
            }
            // 原文 1877
            User.GoldChanged();
            // 原文 1878-1880：New(UpgradeInfo) + 填 sUserName / UserItem（**在清空武器格之前**）
            TUpgradeInfo NewUpgradeInfo = new()
            {
                sUserName = User.m_sCharName,
                UserItem = Weapon,
            };
            // 原文 1881-1884
            if ((StdItem != null) && (StdItem.Value.NeedIdentify == 1))
            {
                NpcSeams.AddGameDataLog(ObjNpcConst.LOG_ItemUpgrade, ObjNpcConst.LOG_ActionNone, User,
                    StdItem.Value.NameStr, Weapon.MakeIndex, m_sCharName, 0, 0, "使用升级武器");
            }
            // 原文 1885
            User.SendDelItem(Weapon);
            // 原文 1886：`User.m_UseItems[U_WEAPON].wIndex := 0;`（就地改 → 托管写回）
            Weapon.wIndex = 0;
            NpcSeams.SetUseItemsWeapon(User, Weapon);
            // 原文 1887-1889（顺序照抄）
            User.RecalcAbilitys();
            User.FeatureChanged();
            User.SendTo(this, Grobal2Const.RM_ABILITY, 0, 0, 0, 0, "");
            // 原文 1890：`sub_4A0218(User.m_ItemList, UpgradeInfo.btDc, UpgradeInfo.btSc, UpgradeInfo.btMc, UpgradeInfo.btDura);`
            sub_4A0218(User, User.m_ItemList, out byte btDc, out byte btSc, out byte btMc, out byte btDura);
            NewUpgradeInfo.btDc = btDc;
            NewUpgradeInfo.btSc = btSc;
            NewUpgradeInfo.btMc = btMc;
            NewUpgradeInfo.btDura = btDura;
            // 原文 1891-1892
            NewUpgradeInfo.dtTime = DelphiRTL.Now();
            NewUpgradeInfo.dwGetBackTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            // 原文 1893-1895
            m_UpgradeWeaponList.Add(NewUpgradeInfo);
            SaveUpgradingList();
            bo0D = true;
        }
        // 原文 1897-1900
        if (bo0D)
            PlayerSurfaceNpcSeams.GotoLable(this, User, ObjNpcConst.sNF_UpgradeOK, false);
        else
            PlayerSurfaceNpcSeams.GotoLable(this, User, ObjNpcConst.sNF_UpgradeFail, false);
    }
}
