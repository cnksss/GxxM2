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

    // ★ 2026 第十六轮：`UserSelectPrepare`（原文 2527-2596 解析段 + 门控链）**暂缓落地**。
    //   原因（本轮普查实测）：门控链 2573/2575 依赖 `PlayObject.LableIsCanJmp(sLabel)`，
    //   而它在 `src` **没有任何代码声明**（此前误判为已存在 —— 命中的其实是
    //   `TPlayObject.PlayerSurface.NpcSession.cs:346` 的**注释行**）。
    //   ⇒ 需先裁定 1 个新接缝 `LableIsCanJmp`（签名 `Func<TPlayObject, string, bool>`，
    //     原文 `ObjPlayer.pas` `function TPlayerObject.LableIsCanJmp(sLabel: string): Boolean`），
    //     落地后再补 `UserSelectPrepare`。详见报告 §22。
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
