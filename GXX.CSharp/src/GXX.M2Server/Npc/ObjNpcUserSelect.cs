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
    /// `UserSelect` 派发体里**本分片负责的两个 `case` 分支**（原文 2702-2706 / 2737-2741）：
    /// <code>
    ///   nNF_SuperRepair: begin if m_boS_repair then SuperRepairItem(PlayObject); end;
    ///   nNF_Repair:      begin if m_boRepair   then RepairItem(PlayObject);      end;
    /// </code>
    /// <para><b>为什么是一个独立方法</b>：`UserSelect` 的派发体（2547-2899）尚未移植，
    /// 而本分片的两条分支是**可独立验证的完整语义单元**。等派发体移植时，
    /// 这两条分支原样搬进 `case` 即可（本方法随之删除）。
    /// **它不是新 API** —— 名字直接标出它对应原文 `UserSelect` 内 `@repair` 的那两条 `case`。</para>
    /// <para>返回 `true` 表示 `nNF` 命中本分片的两条分支之一（已处理）；`false` 表示该命令号
    /// 不属 `@repair` 族 —— **调用方必须区分**，不要把它当成"命令未实现"的静默兜底。</para>
    /// <para><b>★ 删除条件（可执行，勿留给后人猜）</b>：当 `TMerchant.UserSelect` 的派发体
    /// （原文 2547-2899，含 `nIndex := NpcProcessCmd.g_NpcProcessCommand.GetCommand(sLabel)`
    /// 后的 `switch (nIndex)`）落地时，把本方法 `switch` 里的两个 `case` 分支**原样搬进**那个
    /// `switch`，然后**删除本方法**。判据：`UserSelect` 在登记表里由 `Missing` 变为
    /// `Covered`，且 `UserSelectRepairCommands` 在 `src` 与 `tests` 中**零引用**（`grep` 可验）。</para>
    /// </summary>
    public bool UserSelectRepairCommands(TPlayObject PlayObject, int nNF)
    {
        switch (nNF)
        {
            case NpcProcessCmd.nNF_SuperRepair:   // 原文 2702-2706
                if (m_boS_repair)
                    SuperRepairItem(PlayObject);
                return true;
            case NpcProcessCmd.nNF_Repair:        // 原文 2737-2741
                if (m_boRepair)
                    RepairItem(PlayObject);
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
