// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：`TBoxMonster`（箱子怪物）1:1 移植（原文 461-468 声明、10508-10543 实现）。
//   · TBoxMonster.Create      10510-10514
//   · TBoxMonster.Operate     10527-10532
//   · TBoxMonster.Run         10534-10543
//
// **未覆盖（阻塞，见报告 §6.1-C）**：
//   · TBoxMonster.Initialize（10521-10525）需要 `TCreature.Initialize`（原文 `inherited`），
//     托管侧 `Engine.TCreature` **没有** `Initialize` 这个虚方法 —— 与本车道已上报的 6 个
//     Engine 成员同性质。**不自行声明替身**（那会再造一次重名/重复定义）。
//   · TBoxMonster.Destroy（10516-10519）原文函数体只有 `inherited;`，托管侧无析构器覆写语义，
//     同样登记为未覆盖而非伪造实现。
//
// 继承链：原文 `TBoxMonster = class(TAnimalObject)`（ObjNpc.pas:461），
// 与 TNormNpc 同源 → 托管侧同样直接以既有 `GXX.M2Server.Engine.TCreature` 为祖先。
// ============================================================================

using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TBoxMonster
{
    /// <summary>
    /// 原文 `constructor Create; override;`（ObjNpc.pas:10510-10514）：
    /// `inherited;` + `m_btRaceServer := RC_BOX`（Grobal2.pas，值 30）。
    /// 托管侧以构造函数表达（`TCreature()` 是 protected 无参构造，此处再置种族值）。
    /// </summary>
    public TBoxMonster()
    {
        m_btRaceServer = Grobal2Const.RC_BOX;
    }

    /// <summary>
    /// 原文 `function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC`（ObjNpc.pas:10527-10532）。
    /// <para><b>照抄的原文细节</b>：函数体只有 `Result := False;` —— 唯一一行有效逻辑
    /// （`if ProcessMsg.wIdent = RM_MAGSTRUCK then Result := inherited Operate(ProcessMsg);`）
    /// 被 `{ }` 注释掉，故**恒返回 False**（与 TNormNpc.Operate 的同样写法一致）。
    /// 参数名 `ProcessMsg` 保留；托管侧 `TProcessMsg` 取 <c>GXX.M2Server.TProcessMessage</c>
    /// （`MsgQueueConsumeCore.cs:8`，原文 `pTProcessMessage` 的消费端引用）。</para>
    /// </summary>
    public bool Operate(TProcessMessage ProcessMsg)
    {
        bool Result = false;
        // 原文 10530-10531：`{ if ProcessMsg.wIdent = RM_MAGSTRUCK then Result := inherited Operate(ProcessMsg); }`
        // —— 原文如此，整段被 `{ }` 注释，保留。
        return Result;
    }

    /// <summary>
    /// 原文 `procedure Run; override;`（ObjNpc.pas:10534-10543）。
    /// <para><b>照抄的原文细节</b>：存活时把 HP 顶满、死亡时置 0（注释说明用于 `CHECKHPPER` 判定），
    /// 且 `m_Master &lt;&gt; nil` 时把 `m_Master := nil`（**注意不是"若为 nil 则跳过"，
    /// 而是"非 nil 才清"—— 与"无条件清空"行为等价但写法照抄**）。</para>
    /// <para>末尾 `inherited Run` → `base.Run()`（`TCreature.Run` 会跑一次消息队列）。</para>
    /// </summary>
    public override void Run()
    {
        // 采集怪物死亡生命值归零 用于CHECKHPPER判断是否死亡 By 一支笔 at:2021-07-03 11:07:34
        if (!m_boDeath)
            m_wAbil.HP = m_wAbil.MaxHP;
        else
            m_wAbil.HP = 0;
        if (m_Master != null)
            m_Master = null;
        base.Run();
    }
}
