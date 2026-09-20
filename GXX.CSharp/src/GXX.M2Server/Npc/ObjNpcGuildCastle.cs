// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：`TGuildOfficial`（公会官员）与 `TCastleOfficial`（攻城官员）的**可落地方法** 1:1。
//   · TCastleOfficial.Click          1107-1116
//   · TCastleOfficial.Create         10364-10367
//   · TCastleOfficial.SendCustemMsg  10391-10404
//   · TGuildOfficial.Click           10049-10053
//   · TGuildOfficial.GetVariableText 10055-10089
//   · TGuildOfficial.SendCustemMsg   10386-10389
//   · TMerchant.Click                3228-3232
//
// **未覆盖（阻塞，见报告 §8.6）**：
//   · TGuildOfficial.Create（10374-10379）需要 `TBaseObject.m_wAppr`（Engine 缺口）
//     —— 与 `TNormNpc.Initialize`(9864) 同源。第 10377 行 `m_btRaceImg := RC_MERCHANT` 本身可用。
//   · TGuildOfficial.Run（10091-10099）需要 `TurnTo(Integer)` 与 `SendRefMsg(...)`（Engine 缺口）。
//   · TGuildOfficial.UserSelect（10101-10151）需要 `PlayObject.LableIsCanJmp`、`GotoLable`、
//     `m_sScriptGoBackLable` 与 6 个 `sNF_*` 常量。
//   · TGuildOfficial.ReQuestBuildGuild / ReQuestGuildWar / DoNate / ReQuestCastleWar、
//     TCastleOfficial.UserSelect / GetVariableText / HireGuard / HireArcher / RepairDoor / RepairWallNow
//     —— 依赖 Guild.pas / Castle.pas 与 ObjPlayer 面。
//
// 复用（未复制）：`HUtil32.sub_49ADB8`（已 1:1）、`DelphiRTL.Format`（Delphi `%s`/`%d` 格式串）、
//   `GXX.Core.Util.TStringList`、`Engine.TMsgColor` / `Engine.TMsgType`、
//   `NpcSeams.IsMasterGuild`（`TUserCastle.IsMasterGuild` 接缝，与 `GetUserPrice` 同一个）。
// ============================================================================

using System;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TMerchant
{
    /// <summary>
    /// 原文 `procedure Click(PlayObject: TPlayObject); override; // 0049FF24`（ObjNpc.pas:3228-3232）。
    /// 函数体只有一行被注释掉的 `GotoLable(PlayObject,'@main');`（**原文如此，保留**）与 `inherited;`。
    /// </summary>
    public override void Click(TPlayObject PlayObject)
    {
        // 原文 3230：`// GotoLable(PlayObject,'@main');` —— 原文如此，保留。
        base.Click(PlayObject);
    }
}

public partial class TGuildOfficial
{
    /// <summary>
    /// 原文 `procedure Click(PlayObject: TPlayObject); override; // 004A30F4`（ObjNpc.pas:10049-10053）。
    /// 同 <see cref="TMerchant.Click"/>：被注释掉的 `GotoLable` + `inherited;`。
    /// </summary>
    public override void Click(TPlayObject PlayObject)
    {
        // 原文 10051：`// GotoLable(PlayObject,'@main');` —— 原文如此，保留。
        base.Click(PlayObject);
    }

    /// <summary>
    /// 原文 `procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); override;`（ObjNpc.pas:10386-10389）——
    /// 函数体只有 `inherited;`。
    /// </summary>
    public override void SendCustemMsg(TPlayObject PlayObject, string sMsg)
    {
        base.SendCustemMsg(PlayObject, sMsg);
    }

    /// <summary>
    /// 原文 `function GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string;
    /// var IsBreakParseVar: Boolean; nPos: Integer): Boolean;`（ObjNpc.pas:10055-10089）。
    /// <para>与 <see cref="TMerchant.GetVariableText"/> 同型：先走 `inherited`，**仅当基类返回 False** 才处理
    /// 唯一一个公会变量 `$REQUESTCASTLELIST`；落到函数尾时 `Result` 已被改回 `False`（原文 10087）。</para>
    /// <para><b>照抄的原文细节</b>：列表项前/后的 `\` 由 `((II div 2) * 2 = II)` 判定 ——
    /// 即 **II 为偶数**（原文 `I` 从 0 起，`II = I + 1`）时插 `\`，作用是**每两项一行**；
    /// 末尾固定追加 `'\ \'`（原文 10082，两个反斜杠夹一个空格）。</para>
    /// <para>`g_CastleManager.GetCastleNameList` 经 <see cref="NpcSeams.GetCastleNameList"/> 接缝。</para>
    /// </summary>
    public override bool GetVariableText(TPlayObject PlayObject, ref string sMsg, string sVariable,
        ref bool IsBreakParseVar, int nPos)
    {
        bool Result = base.GetVariableText(PlayObject, ref sMsg, sVariable, ref IsBreakParseVar, nPos);
        if (!Result)
        {
            Result = true;
            sVariable = DelphiRTL.UpperCase(sVariable);
            if (sVariable == "$REQUESTCASTLELIST")
            {
                string sText = "";
                TStringList List = new();
                NpcSeams.GetCastleNameList(List);
                for (int I = 0; I <= List.Count - 1; I++)
                {
                    int II = I + 1;
                    string sStr;
                    if ((II / 2) * 2 == II)
                        sStr = "\\";
                    else
                        sStr = "";
                    sText = sText + DelphiRTL.Format("<%s/@requestcastlewarnow%d> %s", List[I], I, sStr);
                }

                sText = sText + "\\ \\";
                sMsg = HUtil32.sub_49ADB8(nPos, sMsg, "<$REQUESTCASTLELIST>", sText);
                return Result;
            }
            Result = false;
        }
        return Result;
    }
}

public partial class TCastleOfficial
{
    /// <summary>
    /// 原文 `constructor Create; override;`（ObjNpc.pas:10364-10367）—— 函数体只有 `inherited;`。
    /// 托管侧以无参构造函数表达（`TMerchant()` 链回 `TNormNpc()` → `TCreature()`）。
    /// </summary>
    public TCastleOfficial()
    {
        // 原文 10366：inherited;
    }

    /// <summary>
    /// 原文 `procedure Click(PlayObject: TPlayObject); override; // FFEB`（ObjNpc.pas:1107-1116）。
    /// <para>`m_Castle = nil` → 提示 `'NPC不属于城堡！'`（`c_Red` / `t_Hint`）并 `Exit`；
    /// 否则**只有**"城主行会成员 **或** `m_btPermission &gt;= 3`"才走 `inherited`（= 打开对话框），
    /// 非成员是**静默无反应**（既无提示也不 `inherited`）—— 照抄。</para>
    /// </summary>
    public override void Click(TPlayObject PlayObject)
    {
        object castle = NpcSeams.GetNpcCastle(this);
        if (castle == null)
        {
            NpcSeams.SysMsg(PlayObject, "NPC不属于城堡！", TMsgColor.c_Red, TMsgType.t_Hint);
            return;
        }
        if (NpcSeams.IsMasterGuild(castle, PlayObject) || (NpcSeams.GetPlayerPermission(PlayObject) >= 3))
            base.Click(PlayObject);
    }

    /// <summary>
    /// 原文 `procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); override;`（ObjNpc.pas:10391-10404）。
    /// <para><b>与 TNormNpc/TGuildOfficial 版的差异（照抄）</b>：本覆写**不调用 `inherited`**，
    /// 而是自己判 `g_Config.boSubkMasterSendMsg` 开关；关闭时提示 `g_sSubkMasterMsgCanNotUseNowMsg`
    /// （`c_Red` / `t_Hint`）后 `Exit`；开启时按 `m_boSendMsgFlag` 单次放行并
    /// `UserEngine.SendBroadCastMsg(角色名 + ': ' + sMsg, t_Castle)`（**不是** `t_Cust`）。</para>
    /// </summary>
    public override void SendCustemMsg(TPlayObject PlayObject, string sMsg)
    {
        if (!NpcSeams.boSubkMasterSendMsg)
        {
            NpcSeams.SysMsg(PlayObject, NpcSeams.g_sSubkMasterMsgCanNotUseNowMsg, TMsgColor.c_Red, TMsgType.t_Hint);
            return;
        }

        if (NpcSeams.GetSendMsgFlag(PlayObject))
        {
            NpcSeams.ClearSendMsgFlag(PlayObject);
            NpcSeams.SendBroadCastMsg(PlayObject.m_sCharName + ": " + sMsg, TMsgType.t_Castle);
        }
    }
}
