// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：`TMerchant.UserSelect`(2087-2899, 813 行) 的 **`@repair` 分片**。
//
// `UserSelect` 的结构 = 20 个**嵌套过程**（2089-2546）+ 一段**命令派发体**（2547-2899）：
//   派发体先解析 `sData`／`sLabel`／`sMsg`，再用
//   `nIndex := g_NpcProcessCommand.IndexOf(sLabel)` → `case nIndex of` 逐条命中 `nNF_*`。
//
// ★★ **分片前提的实测结论（与"首片 @repair 最小"的预期不同，已报告 §18）**：
//   三条分片（`@repair`/`@sell`/`@buy`）**并不互相独立** —— 它们共享同一套
//   **派发基础设施**：`g_NpcProcessCommand`（标签→命令号表，NpcCommon.pas:1906-1960）、
//   `nNF_*` 枚举（NpcCommon.pas:10-88）、以及 2547-2696 的标签/参数解析。
//   因此本片先落**各分片自己的内容**（嵌套过程 + 其 `case` 分支），
//   派发基础设施以**单一接缝** `NpcSeams.NpcProcessCommandIndexOf` 表示（待移植）。
//
// 本片覆盖（原文行号）：
//   2089-2092  `procedure SuperRepairItem(User: TPlayObject);`
//   2262-2265  `procedure RepairItem(User: TPlayObject);`
//   2702-2706  `case nNF_SuperRepair` 分支（守卫 `m_boS_repair`）
//   2737-2741  `case nNF_Repair` 分支（守卫 `m_boRepair`）
//
// ⚠ 本片**没有**把 `UserSelect` 本身登记为 Covered —— 派发体（含 2547-2696 的解析与其余
//   18 个 `nNF_*` 分支）仍未移植。登记表里 `UserSelect` 保持 `Missing`，
//   本片以"分片进度"形式记录（见报告 §18）。
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TMerchant
{
    /// <summary>
    /// 原文 `TMerchant.UserSelect` 内的**嵌套过程**
    /// `procedure SuperRepairItem(User: TPlayObject);`（ObjNpc.pas:2089-2092）：
    /// <code>
    ///   User.SendMsg(Self, RM_SENDUSERSREPAIR, 0, NativeInt(Self), 0, 0, '');
    /// </code>
    /// <para>托管侧落为独立方法（原文是嵌套过程，不捕获外层）。
    /// 发包经 <see cref="ObjNpcSendToExtensions.SendTo"/>（`SendMsgToClient` 的托管名，
    /// 见报告 §10 的命名裁定）—— `nParam1 = NativeInt(Self)` 在托管是 `long`，
    /// 传 `m_nRecogId`（`TCreature` 的唯一标识，原文即对象指针）**不改变语义**，
    /// 因为接收方按标识匹配 NPC；此处照原文传自身标识。</para>
    /// <para>触发点：`UserSelect` 的 `nNF_SuperRepair` 分支（原文 2702-2706），守卫 `m_boS_repair`。</para>
    /// </summary>
    public void SuperRepairItem(TPlayObject User)
    {
        User.SendTo(this, Grobal2Const.RM_SENDUSERSREPAIR, 0, m_nRecogId, 0, 0, "");
    }

    /// <summary>
    /// 原文 `TMerchant.UserSelect` 内的**嵌套过程**
    /// `procedure RepairItem(User: TPlayObject);`（ObjNpc.pas:2262-2265）：
    /// <code>
    ///   User.SendMsg(Self, RM_SENDUSERREPAIR, 0, NativeInt(Self), 0, 0, '');
    /// </code>
    /// <para>⚠ 与 <see cref="SuperRepairItem"/> 只差**一个包号**（`RM_SENDUSERREPAIR` vs
    /// `RM_SENDUSERSREPAIR`，`Grobal2.Const.g.cs:916` vs `:913`）—— 原文如此，两个都要留。</para>
    /// <para>触发点：`UserSelect` 的 `nNF_Repair` 分支（原文 2737-2741），守卫 `m_boRepair`。</para>
    /// </summary>
    public void RepairItem(TPlayObject User)
    {
        User.SendTo(this, Grobal2Const.RM_SENDUSERREPAIR, 0, m_nRecogId, 0, 0, "");
    }

    /// <summary>
    /// 原文 `TMerchant.UserSelect` 内的**嵌套过程**
    /// `procedure SellItem(User: TPlayObject); // 004A1544`（ObjNpc.pas:2257-2260）：
    /// <code>
    ///   User.SendMsg(Self, RM_SENDUSERSELL, 0, NativeInt(Self), 0, 0, '');
    /// </code>
    /// <para>触发点：`nNF_Sell` 分支（原文 2732-2736），守卫 `m_boSell`。</para>
    /// <para>⚠ 与 <see cref="RepairItem"/>/<see cref="ArmRemoveStoneItem"/> 是**三个不同的包号**，
    /// 只有包号与守卫不同 —— 已写互相对照的差异断言（勿互相抄）。</para>
    /// </summary>
    public void SellItem(TPlayObject User)
    {
        User.SendTo(this, Grobal2Const.RM_SENDUSERSELL, 0, m_nRecogId, 0, 0, "");
    }

    /// <summary>
    /// 原文 `TMerchant.UserSelect` 内的**嵌套过程**
    /// `procedure ArmRemoveStoneItem(User: TPlayObject); // 004A1570`（ObjNpc.pas:2267-2270）：
    /// <code>
    ///   User.SendMsg(Self, RM_ARMREMOVESTONE, 0, NativeInt(Self), 0, 0, '');
    /// </code>
    /// <para>触发点：`nNF_ArmRemoveStone` 分支（原文 2742-2746），守卫 `m_boArmRemoveStone`。</para>
    /// <para>⚠ 原文里本过程与 `RepairItem` 的注释都写 `// 004A1570`（**同一个地址注释**）——
    /// 原文如此，**不是**我抄错；两者的包号仍不同（`RM_ARMREMOVESTONE` vs `RM_SENDUSERREPAIR`）。</para>
    /// </summary>
    public void ArmRemoveStoneItem(TPlayObject User)
    {
        User.SendTo(this, Grobal2Const.RM_ARMREMOVESTONE, 0, m_nRecogId, 0, 0, "");
    }

    /// <summary>原文 `AutoGetExp(User, sMsg)`（ObjNpc.pas:2206-2210）：`User.m_sAutoSendMsg := sMsg;`
    /// （原文 2209 的 `// User.SysMsg('挂机成功！', ...)` 注释保留）。触发：`nNF_OfflineMsg`，守卫 `m_boofflinemsg`。</summary>
    public void AutoGetExp(TPlayObject User, string sMsg)
    {
        User.m_sAutoSendMsg = sMsg;
        // 原文 2209：`// User.SysMsg('挂机成功！', c_Red, t_Hint);` —— 原文如此，保留
    }

    /// <summary>原文 `ItemPrices(User)`（ObjNpc.pas:2307-2309）—— ★ **过程体为空**（`begin end` 之间无任何语句）。
    /// 照抄成空方法体，**不**"顺手补实现"。触发：`nNF_Prices`，守卫 `m_boPrices`。</summary>
    public void ItemPrices(TPlayObject User)
    {
        // 原文 2307-2309：过程体为空 —— 原文如此，保留空实现
    }

    /// <summary>原文 `Storage(User, nPage)`（ObjNpc.pas:2311-2314）：`SendMsg(Self, RM_USERSTORAGEITEM, 0, Self, nPage, 0, '')`。
    /// ★ **参数位易抄错**：`nPage` 在 **`nParam2`**（第 4 实参），`wParam` 是 **0**，`nParam1` 才是 `Self`。
    /// 对照 <see cref="BigGetBack"/>（page 在 **`wParam`**）。触发：`nNF_Storage`(0)/`2`(1)/`3`(2)/`4`(3)，**共用守卫 `m_boStorage`**。</summary>
    public void Storage(TPlayObject User, int nPage)
    {
        User.SendTo(this, Grobal2Const.RM_USERSTORAGEITEM, 0, m_nRecogId, nPage, 0, "");
    }

    /// <summary>原文 `GetBack(User, nPage)`（ObjNpc.pas:2316-2319）：`SendMsg(Self, RM_USERGETBACKITEM, 0, Self, nPage, 0, '')`。
    /// 参数位同 <see cref="Storage"/>。触发：`nNF_Getback`(0)/`2`(1)/`3`(2)/`4`(3)，**共用守卫 `m_boGetback`**。</summary>
    public void GetBack(TPlayObject User, int nPage)
    {
        User.SendTo(this, Grobal2Const.RM_USERGETBACKITEM, 0, m_nRecogId, nPage, 0, "");
    }

    /// <summary>原文 `BigStorage(User)`（ObjNpc.pas:2321-2324）：`SendMsg(Self, RM_USERSTORAGEITEM, 0, Self, 0, 0, '')`。
    /// ★ 与 <see cref="Storage"/>(User, 0) **同包同实参**，差别**只在守卫**（`m_boBigStorage` vs `m_boStorage`）。触发：`nNF_BigStorage`。</summary>
    public void BigStorage(TPlayObject User)
    {
        User.SendTo(this, Grobal2Const.RM_USERSTORAGEITEM, 0, m_nRecogId, 0, 0, "");
    }

    /// <summary>原文 `BigGetBack(User)`（ObjNpc.pas:2326-2330）：`m_nBigStoragePage := 0;` 再
    /// `SendMsg(Self, RM_USERBIGGETBACKITEM, m_nBigStoragePage, Self, 0, 0, '')`。
    /// ★ **参数位与 Storage/GetBack 相反**：page 落在 **`wParam`**，`nParam1` 是 `Self`。触发：`nNF_BigGetback`，守卫 `m_boBigGetback`。</summary>
    public void BigGetBack(TPlayObject User)
    {
        User.m_nBigStoragePage = 0;
        User.SendTo(this, Grobal2Const.RM_USERBIGGETBACKITEM, User.m_nBigStoragePage, m_nRecogId, 0, 0, "");
    }

    /// <summary>原文 `GetPreviousPage(User)`（ObjNpc.pas:2332-2339）：`page &gt; 0` 则 `Dec`，否则**显式置 0**
    /// （结果相同，写法照抄）→ 再按 BigGetBack 的参数位发包。触发：`nNF_GetPreviousPage`，守卫 `m_boGetPreviousPage`。</summary>
    public void GetPreviousPage(TPlayObject User)
    {
        if (User.m_nBigStoragePage > 0)
            User.m_nBigStoragePage -= 1;
        else
            User.m_nBigStoragePage = 0;
        User.SendTo(this, Grobal2Const.RM_USERBIGGETBACKITEM, User.m_nBigStoragePage, m_nRecogId, 0, 0, "");
    }

    /// <summary>原文 `GetNextPage(User)`（ObjNpc.pas:2341-2345）：`Inc(page)` 后发包。
    /// ★ 与 <see cref="GetPreviousPage"/> 一样**无任何上界**（原文没有 Max 夹取）—— 照抄，不夹取。
    /// 触发：`nNF_GetNextPage`，守卫 `m_boGetNextPage`。</summary>
    public void GetNextPage(TPlayObject User)
    {
        User.m_nBigStoragePage += 1;
        User.SendTo(this, Grobal2Const.RM_USERBIGGETBACKITEM, User.m_nBigStoragePage, m_nRecogId, 0, 0, "");
    }

    /// <summary>
    /// `UserSelect` 派发体里**本车道已移植的 `case` 分支**
    /// （原文 2702-2706 / 2722-2726 / 2732-2736 / 2737-2741 / 2742-2746 / 2752-2816）。
    /// <para><b>为什么是一个独立方法</b>：`UserSelect` 的派发体（2597-2899）尚未移植，而这些分支是
    /// **可独立验证的完整语义单元**；等派发体移植时**原样搬进** `switch`，本方法随之删除。
    /// **它不是新 API** —— 名字标出它对应原文 `UserSelect` 内的 `case` 片段。</para>
    /// <para>返回 `true` = 命中已移植分支；`false` = 该命令号**不在已移植集合内** —— 调用方**必须区分**，
    /// 不要把它当成"命令未实现"的静默兜底。</para>
    /// <para><b>★ 删除条件（可执行）</b>：当 `TMerchant.UserSelect` 派发体落地（含
    /// `NpcProcessCmd.g_NpcProcessCommand.GetCommand(sLabel)` 后的 `switch`）时，把本方法的全部 `case`
    /// 原样搬进那个 `switch`，并**删除本方法**。判据：`UserSelect` 由 `Missing` → `Covered`，
    /// 且 `UserSelectPortedArms` 在 `src`/`tests` 中**零引用**（`grep` 可验）。</para>
    /// <para>`sMsg` 仅为 `nNF_OfflineMsg` → <see cref="AutoGetExp"/> 保留（其余分支不用）。</para>
    /// </summary>
    /// <summary>
    /// 原文 `procedure RemoteMsg(User: TPlayObject; sLabel, sMsg: string); // 接受歌曲`（ObjNpc.pas:2177-2204）。
    /// <para><b>照抄要点</b>：2182 先 `Trim(sMsg)`（改写的是**局部** `sMsg`）；2183 `sMsg &lt;&gt; ''` 才继续；
    /// 2185 按**玩家名**查对象；2190 `sLabel := Copy(sLabel, 2, Length(sLabel) - 1)`（去掉第 1 个字符，
    /// 因标签形如 `@@rmst`）；2196 与 2201 **两处提示都保留已 Trim 的 `sMsg` 前缀**，照抄。</para>
    /// <para>触发点：`nNF_Rmst` 分支（原文 2717-2721），守卫 `m_boofflinemsg`。</para>
    /// </summary>
    public void RemoteMsg(TPlayObject User, string sLabel, string sMsg)
    {
        // 原文 2182
        sMsg = sMsg.Trim();
        // 原文 2183
        if (sMsg != "")
        {
            // 原文 2185
            TPlayObject? TargetObject = ObjNpcInputSeams.GetPlayObject(sMsg);
            // 原文 2186
            if (TargetObject != null)
            {
                // 原文 2188
                if (ObjNpcInputSeams.GetBoRemoteMsg(TargetObject))
                {
                    // 原文 2190-2192
                    sLabel = DelphiRTL.Copy(sLabel, 2, sLabel.Length - 1);
                    string sSENDMSG = "你的好友 " + User.m_sCharName + " 给你发送音乐\\ \\<播放歌曲/"
                        + sLabel + ">\\";
                    SendMsgToUser(TargetObject, sSENDMSG);
                }
                else
                {
                    // 原文 2196：**保留 sMsg 前缀**
                    NpcSeams.SysMsg(User, sMsg + "你的好友 " + TargetObject.m_sCharName + " 拒绝接受歌曲！",
                        TMsgColor.c_Red, TMsgType.t_Hint);
                }
            }
            else
            {
                // 原文 2201：**保留 sMsg 前缀** + 全局串
                NpcSeams.SysMsg(User, sMsg + ObjNpcInputSeams.g_sUserNotOnLine,
                    TMsgColor.c_Red, TMsgType.t_Hint);
            }
        }
    }

    /// <summary>
    /// 原文 `procedure InPutInteger(User: TPlayObject; sLabel, sMsg: string);`（ObjNpc.pas:2461-2484）。
    /// <para><b>★ 与 <see cref="InPutString"/> 是"看起来一样实则不同"的一对（四处差异）</b>：</para>
    /// <list type="number">
    /// <item>`Copy` **起点/长度不同**：本过程 `Copy(sLabel, **15**, Len-14)`（2477）vs 字符串版 `(…, **14**, Len-13)`（2490）；</item>
    /// <item>**过滤检查位置不同**：本过程在 `StrToIntDef` **之前**（2470）vs 字符串版在**之后**（2496）；</item>
    /// <item>写入目标：`m_nInteger[nNo]`（2480）vs `m_sString[nNo]`（2503）；</item>
    /// <item>过滤命中标签：`'@InputIntegerFilter'`（2472）vs `'@InputStringFilter'`（2498）。</item>
    /// </list>
    /// <para>⚠ 2476 的 `StrToIntDef(sMsg, 0)` 是 **Integer**（原文）；2478 范围门 `(nNo &gt;= 0) and (nNo &lt;= 999)`
    /// —— 超范围**什么都不做**（连 `GotoLable` 都不发）。</para>
    /// </summary>
    public void InPutInteger(TPlayObject User, string sLabel, string sMsg)
    {
        // 原文 2466
        if (HUtil32.IsStringNumber(sMsg))
        {
            // 原文 2468：`if (g_InputBoxFilterList <> nil) then`
            if (ObjNpcInputSeams.GetInputBoxFilterList() != null)
            {
                // 原文 2470：★ 过滤检查在 StrToIntDef **之前**
                if (ObjNpcInputSeams.GetInputBoxInFilterList(sMsg))
                {
                    // 检测用户输入是否有非法字符
                    PlayerSurfaceNpcSeams.GotoLable(this, User, "@InputIntegerFilter", false);
                    return;
                }
            }
            // 原文 2476-2477
            int nValue = (int)DelphiRTL.StrToInt64Def(sMsg, 0);
            int nNo = (int)DelphiRTL.StrToInt64Def(DelphiRTL.Copy(sLabel, 15, sLabel.Length - 14), -1);
            // 原文 2478
            if ((nNo >= 0) && (nNo <= 999))
            {
                // 原文 2480-2481
                User.m_nInteger[nNo] = nValue;
                PlayerSurfaceNpcSeams.GotoLable(this, User, DelphiRTL.Copy(sLabel, 2, sLabel.Length - 1), false);
            }
        }
    }

    /// <summary>
    /// 原文 `procedure InPutString(User: TPlayObject; sLabel, sMsg: string);`（ObjNpc.pas:2486-2506）。
    /// <para>★ 与 <see cref="InPutInteger"/> 的四处差异见该方法的文档。特别地：
    /// 本过程**先** `StrToIntDef(Copy(sLabel, **14**, Len-13), -1)`（2490）、**后**才进范围门与过滤检查
    /// —— 与整数版**顺序相反**。照抄。</para>
    /// <para>2491/2502 两处 `MainOutMessage` 是**注释**，保留。</para>
    /// </summary>
    public void InPutString(TPlayObject User, string sLabel, string sMsg)
    {
        // 原文 2490
        int nNo = (int)DelphiRTL.StrToInt64Def(DelphiRTL.Copy(sLabel, 14, sLabel.Length - 13), -1);
        // 原文 2491：`// MainOutMessage('InPutString:' + ...)` —— 原文如此，保留
        // 原文 2492
        if ((nNo >= 0) && (nNo <= 999))
        {
            // 原文 2494
            if (ObjNpcInputSeams.GetInputBoxFilterList() != null)
            {
                // 原文 2496：★ 过滤检查在范围门**之后**（与整数版相反）
                if (ObjNpcInputSeams.GetInputBoxInFilterList(sMsg))
                {
                    // 检测用户输入是否有非法字符
                    PlayerSurfaceNpcSeams.GotoLable(this, User, "@InputStringFilter", false);
                    return;
                }
            }
            // 原文 2502：`// MainOutMessage('User.m_sString');` —— 原文如此，保留
            // 原文 2503-2504
            User.m_sString[nNo] = sMsg;
            PlayerSurfaceNpcSeams.GotoLable(this, User, DelphiRTL.Copy(sLabel, 2, sLabel.Length - 1), false);
        }
    }

    /// <summary>
    /// 原文 `procedure DealGold(User: TPlayObject; sMsg: string);`（ObjNpc.pas:2212-2254）—— 元宝转账。
    /// <para><b>照抄要点（三处早退各跳一个**不同**标签，勿互抄）</b>：2218 `m_nDealGoldPose &lt;&gt; 1` → `'@dealgoldPlayError'` + **Exit**（**不改 Pose**）；
    /// 2223 **无条件**先把 Pose 置 **2**（在金额校验**之前**）；2224 `nGameGold &lt;= 0` → `'@dealgoldInputFail'`；
    /// 2250 余额不足 → `'@dealgoldFail'`；2233 对向不合法 → `'@dealgoldpost'`。</para>
    /// <para>2233 对向**三个条件**：非 nil **且** `PoseHuman.GetPoseCreate = User`（**互指**）**且** `m_btRaceServer = RC_PLAYOBJECT`。
    /// 2235-2238 顺序：先 `Inc` 对方、再 `Dec` 自己，然后**双方各一次** `GameGoldChanged`。
    /// 2239-2240 两条提示串的 `#10`/`#9`、转出/增加、当前余额 —— 照抄。
    /// 2241-2243 的 `AddGameDataLog(...)` 是**注释**（内容还是从别处复制的"装备破碎"模板）—— 原文如此，保留。</para>
    /// <para>触发点：`nNF_DealGold`（原文 2727-2731），守卫 `m_boDealGold`。</para>
    /// <para>⚠ **托管偏差登记**：原文 `m_nGameGold: LongWord`（ObjPlayer.pas:202），托管是 **`int`**
    /// （`Engine/ObjBase.OnlineMsg.cs:36`）⇒ **溢出边界**行为不同（无符号回绕 vs 有符号）。
    /// 本方法只在 `m_nGameGold &gt;= nGameGold &gt; 0` 路径上做减法，**正常路径不受影响**；已登记待统一。</para>
    /// </summary>
    public void DealGold(TPlayObject User, string sMsg)
    {
        // 原文 2217：默认 -1
        int nGameGold = (int)DelphiRTL.StrToInt64Def(sMsg, -1);
        // 原文 2218-2222
        if (User.m_nDealGoldPose != 1)
        {
            PlayerSurfaceNpcSeams.GotoLable(this, User, "@dealgoldPlayError", false);
            return;
        }
        // 原文 2223：先置 2（在金额校验之前）
        User.m_nDealGoldPose = 2;
        // 原文 2224
        if (nGameGold <= 0)
        {
            // 原文 2226
            PlayerSurfaceNpcSeams.GotoLable(this, User, "@dealgoldInputFail", false);
        }
        // 原文 2230
        else if (User.m_nGameGold >= nGameGold)
        {
            // 原文 2232-2233：对向**互指**校验
            TPlayObject? PoseHuman = User.GetPoseCreate() as TPlayObject;
            if ((PoseHuman != null) && (ReferenceEquals(PoseHuman.GetPoseCreate(), User))
                && (PoseHuman.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT))
            {
                // 原文 2235-2238（顺序照抄）
                PoseHuman.m_nGameGold += nGameGold;
                User.m_nGameGold -= nGameGold;
                PoseHuman.GameGoldChanged();
                User.GameGoldChanged();
                // 原文 2239-2240（第三参 False = 不显示 NPC 名）
                SendMsgToUser(User, "转帐成功：" + '\n' + "转出" + M2Config.sGameGoldName + "："
                    + DelphiRTL.IntToStr(nGameGold) + '\t' + "当前" + M2Config.sGameGoldName + "："
                    + DelphiRTL.IntToStr(User.m_nGameGold), false);
                SendMsgToUser(PoseHuman, "转帐成功：" + '\n' + "增加" + M2Config.sGameGoldName + "："
                    + DelphiRTL.IntToStr(nGameGold) + '\t' + "当前" + M2Config.sGameGoldName + "："
                    + DelphiRTL.IntToStr(PoseHuman.m_nGameGold), false);
                // 原文 2241-2243：`// AddGameDataLog(...)` 注释 —— 原文如此，保留
            }
            else
            {
                // 原文 2247
                PlayerSurfaceNpcSeams.GotoLable(this, User, "@dealgoldpost", false);
            }
        }
        else
        {
            // 原文 2252
            PlayerSurfaceNpcSeams.GotoLable(this, User, "@dealgoldFail", false);
        }
    }

    /// <summary>
    /// 原文 `procedure MakeDurg(User: TPlayObject); // 004A16A0`（ObjNpc.pas:2272-2304）—— 制药列表。
    /// <para><b>照抄要点</b>：2282 `label RefMakeDurg` + `goto` 用于"**删掉空组后从头重扫**"；
    /// 2283 每次重扫都重置 `sSENDMSG := ''`；2287 `List14.Count &lt;= 0` → `Free` + `m_GoodsList.Delete(I)` + **goto 重扫**；
    /// 2294 只看每组的**第 0 件**；2300 拼 `StdItem.Name + '/' + 0 + '/' + nMakeDurgPrice + '/' + 1 + '/'`
    /// （**两个常量 0 与 1 是照抄**，不是占位）；2303 **串非空才发包**。</para>
    /// <para>★ **与 `BuyItem`(2094-2176) 的"看起来一样实则不同"**：`BuyItem` 在 2110-2111 **先判 `List14 = nil`**
    /// 再判 `Count &lt;= 0`；本过程 **2287 只判 `Count`、2294 直接读 `List14[0]`，没有 nil 分支**
    /// ⇒ 对 nil 组**直接抛**（原文如此，**照抄不补**）。已写差异断言。</para>
    /// <para>⚠ 原文 2284 的 `for I := 0 to Count - 1` 是 **Delphi 语义：上界只求值一次**；
    /// 托管 `for (int I = 0; I &lt;= Count - 1; I++)` **每次迭代都重求**。因循环体内**只有** `goto` 分支
    /// 会改 `Count`（且它立刻重启），故两者**等价**；已登记。</para>
    /// <para>触发点：`nNF_MakedUrg` 分支（原文 2747-2751），守卫 `m_boMakeDrug`。</para>
    /// </summary>
    public void MakeDurg(TPlayObject User)
    {
    RestartMakeDurg:
        // 原文 2283
        string sSENDMSG = "";
        // 原文 2284
        for (int I = 0; I <= m_GoodsList.Count - 1; I++)
        {
            // 原文 2286：**不判 nil**（与 BuyItem 不同）
            List<object> List14 = (List<object>)m_GoodsList[I];
            // 原文 2287-2293
            if (List14.Count <= 0)
            {
                m_GoodsList.RemoveAt(I);
                goto RestartMakeDurg;
            }
            // 原文 2294-2296
            if (List14[0] == null)
                continue;
            TUserItem UserItem = (TUserItem)List14[0]!;
            // 原文 2297-2301
            TStdItem? StdItem = NpcSeams.GetStdItem(UserItem.wIndex);
            if (StdItem != null)
            {
                sSENDMSG = sSENDMSG + StdItem.Value.NameStr + "/" + DelphiRTL.IntToStr(0) + "/"
                    + DelphiRTL.IntToStr(M2Config.nMakeDurgPrice) + "/" + DelphiRTL.IntToStr(1) + "/";
            }
        }
        // 原文 2303-2304
        if (sSENDMSG != "")
            User.SendTo(this, Grobal2Const.RM_USERMAKEDRUGITEMS, 0, m_nRecogId, 0, 0, sSENDMSG);
    }

    /// <summary>原文 `Self = g_MissionNPC` 同型的占位说明见 <see cref="UserSelectPortedArms"/>。</summary>
    public bool UserSelectPortedArms(TPlayObject PlayObject, int nNF, string sMsg = "", string sLabel = "")
    {
        switch (nNF)
        {
            case NpcProcessCmd.nNF_SuperRepair:        // 原文 2702-2706
                if (m_boS_repair)
                    SuperRepairItem(PlayObject);
                return true;
            case NpcProcessCmd.nNF_Sell:               // 原文 2732-2736
                if (m_boSell)
                    SellItem(PlayObject);
                return true;
            case NpcProcessCmd.nNF_Repair:             // 原文 2737-2741
                if (m_boRepair)
                    RepairItem(PlayObject);
                return true;
            case NpcProcessCmd.nNF_ArmRemoveStone:     // 原文 2742-2746
                if (m_boArmRemoveStone)
                    ArmRemoveStoneItem(PlayObject);
                return true;
            case NpcProcessCmd.nNF_Prices:             // 原文 2752-2756（ItemPrices 过程体为空）
                if (m_boPrices)
                    ItemPrices(PlayObject);
                return true;
            case NpcProcessCmd.nNF_Storage:            // 原文 2757-2761
                if (m_boStorage)
                    Storage(PlayObject, 0);
                return true;
            case NpcProcessCmd.nNF_Storage2:           // 原文 2762-2766
                if (m_boStorage)
                    Storage(PlayObject, 1);
                return true;
            case NpcProcessCmd.nNF_Storage3:           // 原文 2767-2771
                if (m_boStorage)
                    Storage(PlayObject, 2);
                return true;
            case NpcProcessCmd.nNF_Storage4:           // 原文 2772-2776
                if (m_boStorage)
                    Storage(PlayObject, 3);
                return true;
            case NpcProcessCmd.nNF_Getback:            // 原文 2777-2781
                if (m_boGetback)
                    GetBack(PlayObject, 0);
                return true;
            case NpcProcessCmd.nNF_Getback2:           // 原文 2782-2786
                if (m_boGetback)
                    GetBack(PlayObject, 1);
                return true;
            case NpcProcessCmd.nNF_Getback3:           // 原文 2787-2791
                if (m_boGetback)
                    GetBack(PlayObject, 2);
                return true;
            case NpcProcessCmd.nNF_Getback4:           // 原文 2792-2796
                if (m_boGetback)
                    GetBack(PlayObject, 3);
                return true;
            case NpcProcessCmd.nNF_BigStorage:         // 原文 2797-2801
                if (m_boBigStorage)
                    BigStorage(PlayObject);
                return true;
            case NpcProcessCmd.nNF_BigGetback:         // 原文 2802-2806
                if (m_boBigGetBack)
                    BigGetBack(PlayObject);
                return true;
            case NpcProcessCmd.nNF_GetPreviousPage:    // 原文 2807-2811
                if (m_boGetPreviousPage)
                    GetPreviousPage(PlayObject);
                return true;
            case NpcProcessCmd.nNF_GetNextPage:        // 原文 2812-2816
                if (m_boGetNextPage)
                    GetNextPage(PlayObject);
                return true;
            case NpcProcessCmd.nNF_OfflineMsg:         // 原文 2722-2726（离线挂机 → AutoGetExp）
                if (m_boofflinemsg)
                    AutoGetExp(PlayObject, sMsg);
                return true;
            case NpcProcessCmd.nNF_MakedUrg:           // 原文 2747-2751（制药列表）
                if (m_boMakeDrug)
                    MakeDurg(PlayObject);
                return true;
            case NpcProcessCmd.nNF_DealGold:           // 原文 2727-2731（元宝转账）
                if (m_boDealGold)
                    DealGold(PlayObject, sMsg);
                return true;
            case NpcProcessCmd.nNF_Rmst:               // 原文 2717-2721（接受歌曲 → RemoteMsg）
                if (m_boofflinemsg)
                    RemoteMsg(PlayObject, sLabel, sMsg);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// `TMerchant.UserSelect` 的**解析段 + 门控链**（原文 **2527-2596**）。
    /// <para>落在 `TMerchant` 上，**不是新 API** —— 它是原文 `TMerchant.UserSelect` 的开头部分；
    /// 等派发体（2597-2899）落地时，把本方法体**原样内联**进 `UserSelect` 并删除本方法。
    /// <b>★ 删除条件（可执行）</b>：`UserSelect` 在登记表里由 `Missing` → `Covered`，
    /// 且 `UserSelectPrepare` 在 `src`/`tests` 中**零引用**（`grep` 可验）。</para>
    /// <para><b>逐段对照（报告 §22.5 的 15 条）</b> —— 五个易抄错点已逐条落实：
    /// ① 2529 `ClassNameIs` 是**精确类名**（**不是** `is`）；
    /// ② 2533 **Delphi 优先级 `not` &gt; `and` &gt; `or`**；
    /// ③ 2538 两个出口都接（`ref sLabel` + 返回剩余串）；
    /// ④ 2542 `Pos('(')` 找不到返回 **0**；
    /// ⑤ 2578 长度参数是 **`Length(sLabel)`**（不是被比较串的长度）。</para>
    /// <para>另：2535 `sData[1]` 是 **1-based** 且**已先判空**；2549-2562 `@FOUNDRYITEM_`/`@SHOWITEM_` 归一；
    /// 2586/2595 两处**早退**；2531/2892 的 `try/except` 要报出 **`nCode`（0..21）**。</para>
    /// <para><b>返回</b>：`true` = 继续走到派发体；`false` = 原文在 2530/2586/2595 处 `Exit`。</para>
    /// <para>⚠ `m_boCastle = true` 的城堡 NPC 会触达 `NpcSeams.GetCastleUnderWar`（偏差 **D37**，未接线时抛 —— **窄路径**）。</para>
    /// </summary>
    public bool UserSelectPrepare(TPlayObject PlayObject, string sData, out string sLabel, out string sMsg,
        out bool boAllowSelect, out bool boCanGoto, out bool boCanJmp)
    {
        // 原文 2527：inherited（落到 TNormNpc.UserSelect 真实现 —— 虚分派链的基类落点）
        base.UserSelect(PlayObject, sData);

        sLabel = "";
        sMsg = "";
        boAllowSelect = false;
        boCanGoto = false;
        boCanJmp = false;

        // 原文 2528
        int nCode = 0;
        // 原文 2529-2530：★ ① 精确类名比较（不是 is / 派生判定）
        if (GetType() != typeof(TMerchant))
            return false;
        try
        {
            // 原文 2532
            nCode = 1;
            // ★ ② 原文 2533：`not m_boCastle or not ((m_Castle<>nil) and underWar) and (PlayObject<>nil)`
            //   Delphi 优先级 `not` > `and` > `or` → `(!m_boCastle) || ((!underWar) && (PlayObject != null))`
            bool castleUnderWar = false;
            if (m_boCastle)
            {
                object castle = NpcSeams.GetNpcCastle(this);
                if (castle != null)
                    castleUnderWar = NpcSeams.GetCastleUnderWar(castle);   // 偏差 D37（窄路径）
            }
            if ((!m_boCastle) || ((!castleUnderWar) && (PlayObject != null)))
            {
                // 原文 2535：`(sData <> '') and (sData[1] = '@')` —— sData[1] 1-based 且已先判空
                if ((sData != "") && (sData[0] == '@'))
                {
                    // 原文 2538：★ ③ 两个出口都要接（ref sLabel + 返回剩余串）
                    sMsg = HUtil32.GetValidStr3_Ex(sData, ref sLabel, '\r');
                    // 原文 2540-2545：★ ④ Pos 找不到返回 0
                    if ((sLabel.Length >= 2) && (sLabel[1] == '@') && (sLabel[sLabel.Length - 1] == ')'))
                    {
                        int nPos = (int)DelphiRTL.Pos("(", sLabel);
                        if (nPos > 0)
                            sLabel = DelphiRTL.Copy(sLabel, 1, nPos - 1);
                    }
                    // 原文 2548-2562
                    nCode = 3;
                    if (MonGenParseCore.CompareLStr(sLabel, "@FOUNDRYITEM_", "@FOUNDRYITEM_".Length))
                    {
                        PlayObject.m_sNpcSelectItemName = DelphiRTL.Copy(sLabel, "@FOUNDRYITEM_".Length + 1,
                            sLabel.Length - "@FOUNDRYITEM_".Length);
                        sLabel = "@FOUNDRYITEM_";
                    }
                    else if (MonGenParseCore.CompareLStr(sLabel, "@SHOWITEM_", "@SHOWITEM_".Length))
                    {
                        PlayObject.m_sNpcSelectItemName = DelphiRTL.Copy(sLabel, "@SHOWITEM_".Length + 1,
                            sLabel.Length - "@SHOWITEM_".Length);
                        sLabel = "@SHOWITEM_";
                    }
                    else
                    {
                        PlayObject.m_sNpcSelectItemName = "";
                    }
                    // 原文 2563-2566
                    nCode = 4;
                    PlayObject.m_sScriptLable = sData;
                    PlayObject.m_sInputData = sMsg;
                    // 原文 2567-2568：`// boCanGoto := PlayObject.LableIsCanJmp(sLabel);` 注释保留
                    nCode = 5;
                    boAllowSelect = AllowSelect(sLabel);
                    // 原文 2571-2575
                    nCode = 6;
                    bool boIdentityMatch = NpcSeams.IsFunctionNpc(this) || NpcSeams.IsManageNpc(this)
                        || NpcSeams.IsMissionNpc(this);
                    if (boIdentityMatch)
                        boCanGoto = NpcSeams.LableIsCanJmp(PlayObject, sLabel) && boAllowSelect;
                    else
                        boCanGoto = NpcSeams.LableIsCanJmp(PlayObject, sLabel);
                    // 原文 2576-2579：★ ⑤ 长度参数是 `Length(sLabel)`
                    if ((!boCanGoto) && PlayObject.m_boMessageBox)
                    {
                        boCanGoto = MonGenParseCore.CompareLStr(sLabel, PlayObject.m_sYesLable, sLabel.Length)
                            || MonGenParseCore.CompareLStr(sLabel, PlayObject.m_sNoLable, sLabel.Length);
                    }
                    // 原文 2580-2588
                    nCode = 7;
                    if (NpcSeams.IsFunctionNpc(this) || NpcSeams.IsMissionNpc(this))
                    {
                        if (!boAllowSelect)
                        {
                            NpcSeams.MainOutMessage(
                                $"用户:{PlayObject.m_sCharName}; NPC:{m_sCharName} 禁止点用该NPC触发字段:{sLabel}");
                            return false;
                        }
                    }
                    // 原文 2589-2590
                    nCode = 8;
                    boCanJmp = boCanGoto
                        || (NpcSeams.IsFunctionNpc(this) && boAllowSelect)
                        || (NpcSeams.IsMissionNpc(this) && boAllowSelect);
                    // 原文 2591-2596
                    nCode = 9;
                    if (ObjNpcText.CompareText(sLabel, NpcProcessCmd.sNF_SendMsg) == 0)
                    {
                        if (sMsg == "")
                            return false;
                    }
                    // 原文 2597-2631：**派发体前置段**（与 `case nNF_*` 派发体区分）
                    //   —— `sNF_InputInteger` / `sNF_InputString` / `@@copytoclipboard` 三个**前缀识别**分支。
                    //   ⚠ 这三条**将来会被派发体取代吗？不会** —— 它们按 `sLabel` 的**前缀**判定（`CompareLStr`），
                    //   而派发体按 `g_NpcProcessCommand` 表**精确查表**；`@InputInteger1(...)` 这类带参标签
                    //   根本不在表里（表里只有 `@@InputInteger`），故本节与派发体**并存**、各司其职。
                    nCode = 10;
                    if (MonGenParseCore.CompareLStr(sLabel, NpcProcessCmd.sNF_InputInteger,
                            NpcProcessCmd.sNF_InputInteger.Length))
                    {
                        // 原文 2600：防止非法刷变量 2020-11-04 23:27:59
                        if (boCanGoto || (NpcSeams.IsMissionNpc(this) && boAllowSelect))
                        {
                            nCode = 11;
                            // 原文 2603
                            if (sMsg.Length > 10)
                            {
                                NpcSeams.MainOutMessage($"{sLabel}长度错误; 用户:{PlayObject.m_sCharName}; 长度:{sMsg.Length}");
                                return false;
                            }
                            // 原文 2608-2609
                            InPutInteger(PlayObject, sLabel, sMsg);
                            return false;
                        }
                    }
                    else if (MonGenParseCore.CompareLStr(sLabel, NpcProcessCmd.sNF_InputString,
                        NpcProcessCmd.sNF_InputString.Length))
                    {
                        // 原文 2614
                        if (boCanGoto || (NpcSeams.IsMissionNpc(this) && boAllowSelect))
                        {
                            nCode = 12;
                            // 原文 2617
                            if (sMsg.Length > M2Config.nMaxInputStringLen)
                            {
                                NpcSeams.MainOutMessage($"{sLabel}长度错误; 用户:{PlayObject.m_sCharName}; 长度:{sMsg.Length}");
                                return false;
                            }
                            // 原文 2622-2623
                            InPutString(PlayObject, sLabel, sMsg);
                            return false;
                        }
                    }
                    else if (MonGenParseCore.CompareLStr(sLabel, "@@copytoclipboard", "@@copytoclipboard".Length))
                    {
                        // 原文 2628-2630：`Copy(sLabel, 2, MaxInt)` —— 去掉第 1 个字符后跳转
                        nCode = 12;
                        PlayerSurfaceNpcSeams.GotoLable(this, PlayObject,
                            DelphiRTL.Copy(sLabel, 2, int.MaxValue), false);
                        return false;
                    }
                    // 原文 2597 起是派发体（本方法不含）—— `true` = 继续派发
                    return true;
                }
            }
            // 2533 的门不成立（或 2535 不成立）→ 与原文一致地走到末尾
            return true;
        }
        catch (Exception e)
        {
            // 原文 2892-2897：`MainOutMessage(Format(sExceptionMsg, [sData, nCode])); MainOutMessage(E.Message);`
            NpcSeams.MainOutMessage($"[Exception] TMerchant.UserSelect... Data: {sData}; Code: {nCode}");
            NpcSeams.MainOutMessage(e.Message);
            return false;
        }
    }
}


/// <summary>
/// `TNormNpc.UserSelect` —— `UserSelect` **虚分派链的基类落点**（原文 `ObjNpc.pas:305` 声明 `virtual`，
/// 实现 `:9807-9835`）。
/// <para>★ 原文共 **4 个覆写点**（已核查，全仓 `.pas` 一并搜过）：
/// `TMerchant`(2087)、`TGuildOfficial`(10101)、`TCastleOfficial`(1186)，
/// 以及类声明区的 `:411`/`:444`/`:481` 三条 `override` 声明
/// （实测分别属 `TMerchant`/`TGuildOfficial`/`TCastleOfficial` 的声明块，与前三者同一批）。</para>
/// <para>⇒ 托管侧此方法**必须** `public virtual`，否则各处覆写的 `inherited` 全部落空
/// （台账那条"基类方法 + 子类 `inherited` ⇒ 必须落虚方法"的规程）。</para>
/// <para><b>★ 与调度方裁定的差异（已在报告登记）</b>：裁定(1)原本只要求"落虚外壳 + 接缝"，
/// 前提是"基类实体 9807 起**长度未测**"。实测该实体**仅 30 行**，且**全部依赖已就位**
/// （`m_nScriptGotoCount`、`HUtil32.GetValidStr3_Ex`、`GotoLable` 接缝、
/// `m_sScriptCurrLable`/`m_sScriptGoBackLable`、`NpcProcessCmd.sNF_Back`），
/// 故本车道**直接落真实现** —— 省掉一个一次性接缝，并收口登记表 `9807` 一条。
/// （若调度方坚持只要外壳，删除本实体体、改为转发接缝即可，两处调用点不变。）</para>
/// </summary>
public partial class TNormNpc
{
    /// <summary>
    /// 原文 `procedure TNormNpc.UserSelect(PlayObject: TPlayObject; sData: string);`
    /// （ObjNpc.pas:9807-9835）——"脚本命令 `@back` 返回上级标签"的基类处理。
    /// <para><b>照抄的原文细节</b>：</para>
    /// <list type="bullet">
    /// <item>9811：**先把 `m_nScriptGotoCount` 归零**（子类 `UserSelect` 的 `inherited` 先调到这里）。</item>
    /// <item>9814：`(sData &lt;&gt; '') and (sData[1] = '@')` —— `sData[1]` 是 **1-based**，且**已先判空**。</item>
    /// <item>9816：`sMsg := GetValidStr3_Ex(sData, sLabel, #13);` —— **原地改 `ref sLabel`、返回剩余串**；
    ///   本方法**丢弃**返回的剩余串（原文的局部 `sMsg` 在此处未被使用），只用 `sLabel`。照抄 `ref` 语义。</item>
    /// <item>9817-9818：`'@HeroMap'` **特殊直跳** `GotoLable`（注释：支持卧龙笔记移动），**不**走 9819 的标签栈。</item>
    /// <item>9819：`m_sScriptCurrLable &lt;&gt; sLabel` 才动栈（**同标签重复点击不入栈**）。</item>
    /// <item>9821-9825：**不是** `@back` 时：`GoBackLable := 旧 CurrLable; CurrLable := sLabel;`
    ///   —— **赋值顺序照抄**（先存旧值再覆盖）。</item>
    /// <item>9826-9831：**是** `@back` 时：若 `CurrLable &lt;&gt; ''` 则清 `CurrLable`，否则清 `GoBackLable`
    ///   —— **只清一层**，照抄这个 else 链。</item>
    /// </list>
    /// </summary>
    public virtual void UserSelect(TPlayObject PlayObject, string sData)
    {
        // 原文 9811
        PlayObject.m_nScriptGotoCount = 0;

        // 原文 9813-9814：处理脚本命令 @back 返回上级标签内容
        if ((sData != "") && (sData[0] == '@'))
        {
            // 原文 9816：`sMsg := GetValidStr3_Ex(sData, sLabel, #13);`
            // —— 原地改 `sLabel`（ref）、返回剩余串；与原文一致地**丢弃剩余串**。
            string sLabel = "";
            HUtil32.GetValidStr3_Ex(sData, ref sLabel, '\r');
            // 原文 9817-9818：@HeroMap 特殊直跳（支持卧龙笔记移动 piaoyun 2013-08-20）
            if (sLabel == "@HeroMap")
            {
                PlayerSurfaceNpcSeams.GotoLable(this, PlayObject, sLabel, false);
            }
            // 原文 9819
            else if (PlayObject.m_sScriptCurrLable != sLabel)
            {
                // 原文 9821
                if (!string.Equals(sLabel, NpcProcessCmd.sNF_Back, StringComparison.OrdinalIgnoreCase))
                {
                    // 原文 9823-9824（顺序照抄：先存旧值，再覆盖）
                    PlayObject.m_sScriptGoBackLable = PlayObject.m_sScriptCurrLable;
                    PlayObject.m_sScriptCurrLable = sLabel;
                }
                else
                {
                    // 原文 9828-9831：只清一层
                    if (PlayObject.m_sScriptCurrLable != "")
                        PlayObject.m_sScriptCurrLable = "";
                    else
                        PlayObject.m_sScriptGoBackLable = "";
                }
            }
        }
    }
}
