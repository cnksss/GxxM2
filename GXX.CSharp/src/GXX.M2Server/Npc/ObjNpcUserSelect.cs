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
    /// </summary>
    public bool UserSelectRepairCommands(TPlayObject PlayObject, int nNF)
    {
        switch (nNF)
        {
            case ObjNpcConst.nNF_SuperRepair:   // 原文 2702-2706
                if (m_boS_repair)
                    SuperRepairItem(PlayObject);
                return true;
            case ObjNpcConst.nNF_Repair:        // 原文 2737-2741
                if (m_boRepair)
                    RepairItem(PlayObject);
                return true;
            default:
                return false;
        }
    }
}
