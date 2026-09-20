// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：`TNormNpc` 的**对话/会话下发族** 1:1（`Engine/PlayerSurface/**` 落地后解锁）。
//   · TNormNpc.SendMsgToUser   9789-9798
//   · TNormNpc.MessageBox      9800-9805
//   · TNormNpc.SendCustemMsg   9837-9862  ← 由切片 7 的"虚方法外壳"升级为**真实现**
//   · TBoxMonster.Initialize   10521-10525
//   · TGuildOfficial.Create    10374-10379
//
// 解锁依据（调度方 2026 第三轮）：`TCreature.Initialize` 已为 `virtual`、
// `m_wAppr`/`m_btRaceImg`/`m_nRecogId` 已在 Engine 落地。
//
// 命名裁定：原文 `PlayObject.SendMsg(Self, ...)`（**网络下发**版，ObjBase.pas:30339）
//   托管侧一律写作 `PlayObject.SendTo(this, ...)` —— 见 `ObjNpcSendToExtensions` 的说明
//   （不能叫 SendMsg：`Engine.TCreature.SendMsg` 是语义不同的入队版）。
// ============================================================================

using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TNormNpc
{
    /// <summary>
    /// 原文 `procedure SendMsgToUser(PlayObject: TPlayObject; sMsg: string; boShowNPCName: Boolean = True);`
    /// （ObjNpc.pas:9789-9798）。
    /// <para><b>照抄的原文细节</b>：`g_OnlineMsgControl.boDisableUseNpc` 为真时**直接退出**（不发任何包）；
    /// `boShowNPCName` 为真时消息头拼 `m_sCharName + '/'`（**斜杠，不是冒号**）。
    /// 两条分支的 `wIdent` 都是 `RM_MERCHANTSAY`(20077)，`wParam`/`nParam1`/`nParam2`/`nParam3` 全 0。</para>
    /// </summary>
    public void SendMsgToUser(TPlayObject PlayObject, string sMsg, bool boShowNPCName = true)
    {
        if (OnlineMsgControl.g_OnlineMsgControl.boDisableUseNpc)
            return;

        if (boShowNPCName)
            PlayObject.SendTo(this, Grobal2Const.RM_MERCHANTSAY, 0, 0, 0, 0, m_sCharName + "/" + sMsg);
        else
            PlayObject.SendTo(this, Grobal2Const.RM_MERCHANTSAY, 0, 0, 0, 0, sMsg);
    }

    /// <summary>
    /// 原文 `procedure MessageBox(PlayObject: TPlayObject; sMsg: string);`（ObjNpc.pas:9800-9805）。
    /// <para><b>照抄的原文细节</b>：消息体先过一遍 `GetLineVariableText`（变量替换），
    /// `wIdent = RM_MENU_OK`(20118)，**`nParam1` 传的是 `NativeInt(Self)`**（对象指针 = 托管侧的 `m_nRecogId`），
    /// 其余参数全 0。`IsBreakParseVar` 是本地变量、结果被丢弃（原文如此，保留）。</para>
    /// </summary>
    public void MessageBox(TPlayObject PlayObject, string sMsg)
    {
        bool IsBreakParseVar = false;
        PlayObject.SendTo(this, Grobal2Const.RM_MENU_OK, 0, m_nRecogId, 0, 0,
            GetLineVariableText(PlayObject, sMsg, ref IsBreakParseVar));
    }

    /// <summary>
    /// 原文 `procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); virtual;`（ObjNpc.pas:9837-9862）。
    /// <para><b>★ 切片 15 起为真实现</b>（切片 7 只落了虚方法外壳 + 接缝）：</para>
    /// <list type="number">
    /// <item>9841：`g_Config.boSendCustemMsg` 为假 → `SysMsg(g_sSendCustMsgCanNotUseNowMsg, c_Red, t_Hint)` 后 `Exit`。</item>
    /// <item>9847：**仅当** `g_FilterTexts &lt;&gt; nil` **且** `sMsg &lt;&gt; ''` 时才过滤；
    ///   `Filter` 返回真表示**命中非法字符**，用过滤后的串替换；替换后为空则 `Exit`。</item>
    /// <item>9857：`m_boSendMsgFlag` 为真才广播 —— 且**先置假再广播**（单次放行语义）；
    ///   消息体是 `m_sCharName + ': ' + sMsg`（**冒号**，与 <see cref="SendMsgToUser"/> 的斜杠不同）；
    ///   类型是 `t_Cust`（注意 `<see cref="TCastleOfficial.SendCustemMsg"/>` 用的是 `t_Castle`）。</item>
    /// </list>
    /// <para>三个子类覆写（`TMerchant`/`TGuildOfficial` 走 `inherited`；`TCastleOfficial` **不调**基类）
    /// 依赖本虚方法。</para>
    /// </summary>
    public virtual void SendCustemMsg(TPlayObject PlayObject, string sMsg)
    {
        if (!NpcSeams.boSendCustemMsg)
        {
            NpcSeams.SysMsg(PlayObject, NpcSeams.g_sSendCustMsgCanNotUseNowMsg, TMsgColor.c_Red, TMsgType.t_Hint);
            return;
        }

        TFilterTexts filter = NpcSeams.GetFilterTexts();
        if ((filter != null) && (sMsg != ""))
        {
            // 检测用户输入是否有非法字符
            if (filter.Filter(sMsg, out string sNewMsg))
            {
                sMsg = sNewMsg;
                if (sMsg == "")
                    return;
            }
        }

        if (NpcSeams.GetSendMsgFlag(PlayObject))
        {
            NpcSeams.ClearSendMsgFlag(PlayObject);
            NpcSeams.SendBroadCastMsg(PlayObject.m_sCharName + ": " + sMsg, TMsgType.t_Cust);
        }
    }
}

public partial class TBoxMonster
{
    /// <summary>
    /// 原文 `procedure Initialize; override;`（ObjNpc.pas:10521-10525）。
    /// <para><b>顺序照抄</b>：**先** `m_btDirection := Random(3)`（0..2，即只取上/右上/右三个方向），
    /// **再** `inherited`。`TCreature.Initialize` 已是 `virtual`（`PlayerSurface.Base.cs:271`），故可直接 `base.Initialize()`。</para>
    /// </summary>
    public override void Initialize()
    {
        m_btDirection = (byte)NpcSeams.Random(3);
        base.Initialize();
    }
}

public partial class TGuildOfficial
{
    /// <summary>
    /// 原文 `constructor Create; override;`（ObjNpc.pas:10374-10379）：
    /// `inherited;` + `m_btRaceImg := RC_MERCHANT`（50）+ `m_wAppr := 8`。
    /// <para><b>与 <see cref="TCastleOfficial"/> 的 Create 差异（照抄）</b>：后者只 `inherited`，
    /// **不置**种族图与外观值 —— 同族两个 Create 行为不一致（原文如此）。</para>
    /// </summary>
    public TGuildOfficial()
    {
        m_btRaceImg = (byte)Grobal2Const.RC_MERCHANT;
        m_wAppr = 8;
    }
}
