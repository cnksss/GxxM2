// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF）
// 本文件：**消息派发与心跳片（切片 2）** —— 本车道 p13-m2-objplayer 自有切片。
// 覆盖原文行号范围（实现段）：
//   ObjPlayer.pas:3699-3771  TPlayObject.Operate（含 3706-3721 内嵌 ProcessPlayObjectMessage、
//                            3723-3732 内嵌 CanFilter）
//   ObjPlayer.pas:3772-5606  TPlayObject.Run（1,835 行，**留痕未移植**，见下方说明）
//   ObjPlayer.pas:1224       `function Operate(ProcessMsg: pTProcessMessage): Boolean; override;` 声明
//
// 三数对账（本切片）：
//   真实体 4（Operate / ProcessPlayObjectMessage / CanFilter / FilterMessage 常量表）
//   NotPorted 1（Run —— 1,835 行，依赖面见下）
//   原文如此 0
//   合计 5 条 = 本切片覆盖的 2 条具名例程（Operate、Run）+ 3 条原文**内嵌**例程
//
// ⚠ `Run` 为什么留痕而非半移植：
//   原文 3772-5606 的 `Run` 是一个 1,835 行的巨型心跳体，逐段依赖**尚未移植**的
//   `TEnvirnoment`（`g_Config` 字段族、`GetMovingObject`、`CanWalk`）、`TUserCastle`、
//   `TItemObject`、`TCustomMagicConfig`、`PClientBufInfo`/`PArrBufInfo`、`THeroObject`、
//   `g_CastleManager`、`GetHighHuman`、`StopCollect`、`RecalcPlayCombatPower` 等
//   （对照 `_p13_joined.txt` 的 Run 端点 3772-5606）。
//   按任务书第 4 条（依赖缺失就登记阻塞项，不臆造替身）与台账 §48.1（禁止裸 `=> true;`），
//   此处落 `PortNotPorted` 显式留痕，**不**写成"近似心跳"。
//   托管侧既有的 `TCreature.Operate()`（ObjBase.cs:99）与 `TPlayObject.Run()`
//   （ObjBase.cs:208，2 行近似物）**保持原样不动** —— 本文件不重复声明它们。
//
// ⚠ 不重复声明的既有成员（`grep` 证据）：
//   · `Operate()`（无参、虚、drain 队列）→ `Engine/ObjBase.cs:99`
//   · `Operate(TProcessMessageRef)`（protected 虚）→ `Engine/ObjBase.cs:114`
//   · `Run()` → `Engine/ObjBase.cs:208`
//   · `SendDefMessage(...)` 接缝 → `Engine/PlayerSurface/TPlayObject.PlayerSurface.Gold.cs:52`
//   · `SendSocketRef` / `PlayerSurfacePack` / `PortNotPorted` → 本车道 PortKit
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Sweep9.Monsters;

namespace GXX.M2Server.Engine;

/// <summary>
/// `ObjPlayer.pas` 消息派发片的**依赖接缝**：原文 `Operate` 里三个不属于本单元的落点。
/// </summary>
public static class PlayerSurfaceOperateSeams
{
    /// <summary>
    /// 原文 `MainOutMessage(sMsg: string)`（`M2Share.pas` 的全局日志输出）。
    /// 默认：无宿主，丢弃。原文调用点：`ObjPlayer.pas:3717`（内嵌 `ProcessPlayObjectMessage` 的异常分支）。
    /// </summary>
    public static Action<string> MainOutMessage { get; set; } = _ => { };

    /// <summary>
    /// 原文 `TBaseObject.Operate(ProcessMsg): Boolean`（**基类实现**，`ObjBase.pas`）。
    /// 托管侧基类只有 `Operate()`（drain）与 `protected Operate(TProcessMessageRef)`（无返回值），
    /// **没有**同名同签名的布尔版 ⇒ 按 §18.8「接缝不得把虚调用降级」的相反面处理：
    /// 这里用接缝**如实表达"转调基类实现"**，而不是在本类里重写基类语义。
    /// 参数：接收者 / 报文。返回：原文 `inherited Operate` 的 `Result`。默认：false。
    /// </summary>
    public static Func<TPlayObject, TProcessMessage, bool> InheritedOperate { get; set; }
        = (_, _) => false;

    /// <summary>
    /// 原文 `g_PluginManager.HookPlayerProcessMsgBegin(...)`（PluginManager.pas）。
    /// 参数：玩家 / wIdent / wParam / nParam1 / nParam2 / nParam3 / BaseObject /
    /// dwDeliveryTime / sMsg / **var boReturn**。默认：无宿主，不改写 boReturn。
    /// ⚠ `BaseObject` 在托管 `TProcessMessage` 里是 `nint`（`MsgQueueConsumeCore.cs:15`，
    /// 承载原文的对象指针），故此处以 `nint` 原样透传，**不做强转**（原文是
    /// `TBaseObject(ProcessMsg.BaseObject)` 指针转换，托管无对应安全转换）。
    /// </summary>
    public static Action<TPlayObject, int, nint, nint, nint, nint, nint, uint, string, BoolRef> HookProcessMsgBegin { get; set; }
        = (_, _, _, _, _, _, _, _, _, _) => { };

    /// <summary>原文 `g_PluginManager.HookPlayerProcessMsgEnd(...)`（同名同参，返回值被忽略）。</summary>
    public static Action<TPlayObject, int, nint, nint, nint, nint, nint, uint, string, BoolRef> HookProcessMsgEnd { get; set; }
        = (_, _, _, _, _, _, _, _, _, _) => { };

    /// <summary>原文 `g_PluginManager <> nil` 的可用性判定。默认：插件面未接（false）。</summary>
    public static Func<bool> PluginManagerAvailable { get; set; } = () => false;

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        MainOutMessage = _ => { };
        InheritedOperate = (_, _) => false;
        HookProcessMsgBegin = (_, _, _, _, _, _, _, _, _, _) => { };
        HookProcessMsgEnd = (_, _, _, _, _, _, _, _, _, _) => { };
        PluginManagerAvailable = () => false;
    }
}

/// <summary>
/// Delphi `var boReturn: BOOL` 形参的托管替身（`HookProcessMsgBegin` 的**出参**语义：
/// 插件可以把它改成 True，调用方随后读它）。
/// </summary>
public sealed class BoolRef
{
    public bool Value;
    public BoolRef(bool value = false) => Value = value;
}

/// <summary>
/// `ObjPlayer.pas` 的**消息派发面**（原文 3699-3771）。
/// </summary>
public partial class TPlayObject
{
    // ------------------------------------------------------------------
    // 原文 3706-3721：内嵌 function ProcessPlayObjectMessage（TPlayObject.Operate 的过程级嵌套函数）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `function ProcessPlayObjectMessage(ProcessMsg: pTProcessMessage; var boResult: Boolean): Boolean;`
    /// （`ObjPlayer.pas:3706-3721`，是 `TPlayObject.Operate` 的**过程级嵌套函数**）。
    ///
    /// <code>
    ///   Result := False;
    ///   if (ProcessMsg.wIdent &gt; 0) and (ProcessMsg.wIdent &lt;= MAXCLIENTMESSAGECOUNT) then
    ///   begin
    ///     Result := True;
    ///     if Assigned(PlayObjectMessageArray[ProcessMsg.wIdent]) then
    ///       try
    ///         PlayObjectMessageArray[ProcessMsg.wIdent](ProcessMsg, boResult);
    ///       except
    ///         MainOutMessage('ProcessPlayObjectMessage Error; wIdent:' + IntToStr(ProcessMsg.wIdent));
    ///       end;
    ///   end;
    /// </code>
    ///
    /// ★ 三条**逐字保留**的原文语义：
    /// <list type="number">
    ///   <item><description>`Result := True` 在**越界判定之后、槽位存在性判定之前** ——
    ///     即"标识在 1..MAXCLIENTMESSAGECOUNT 内"就返回 True（**哪怕该槽位是 nil**）。</description></item>
    ///   <item><description>槽位为 `nil` 时**既不派发也不报错**，`boResult` 保持调用方传入的值。</description></item>
    ///   <item><description>异常被**吞掉**并只打一行日志 —— 原文 `try..except` 无 `raise`，
    ///     所以被处理函数抛出的任何异常都不会传播（托管侧 `catch (Exception)` 等价）。</description></item>
    /// </list>
    /// </summary>
    /// <param name="processMsg">报文（原文是指针，托管用引用）。</param>
    /// <param name="boResult">原文 `var boResult` 的**出参**（调用方传入初值，处理函数可改写）。</param>
    /// <returns>原文 `Result`。</returns>
    public bool ProcessPlayObjectMessage(TProcessMessage processMsg, ref bool boResult)
    {
        // 原文 3708：Result := False;
        bool result = false;

        // 原文 3709：if (ProcessMsg.wIdent > 0) and (ProcessMsg.wIdent <= MAXCLIENTMESSAGECOUNT) then
        // MAXCLIENTMESSAGECOUNT 是全局常量（M2Share/Grobal2 侧），托管侧未切出 —— 见 PlayerSurfaceOperateConst。
        if (processMsg.wIdent > 0 && processMsg.wIdent <= PlayerSurfaceOperateConst.MAXCLIENTMESSAGECOUNT)
        {
            // 原文 3711：Result := True;
            result = true;

            // 原文 3712：if Assigned(PlayObjectMessageArray[ProcessMsg.wIdent]) then
            var handler = PlayObjectMessageArray.Length > processMsg.wIdent
                ? PlayObjectMessageArray[processMsg.wIdent]
                : null;
            if (handler != null)
            {
                // 原文 3714-3718：try ... except MainOutMessage(...) end;（**无 raise**）
                try
                {
                    handler(processMsg, ref boResult);
                }
                catch (Exception)
                {
                    // 原文 3717：MainOutMessage('ProcessPlayObjectMessage Error; wIdent:' + IntToStr(ProcessMsg.wIdent));
                    PlayerSurfaceOperateSeams.MainOutMessage(
                        "ProcessPlayObjectMessage Error; wIdent:" + processMsg.wIdent);
                }
            }
        }

        // 原文 3721：end;（函数的隐式 Result 返回）
        return result;
    }

    // ------------------------------------------------------------------
    // 原文 3723-3732：内嵌 function CanFilter（同上，过程级嵌套函数）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `function CanFilter(wIdent: Word): Boolean;`（`ObjPlayer.pas:3723-3732`）。
    ///
    /// <code>
    ///   Result := True;
    ///   case wIdent of
    ///     RM_HEAR, RM_WHISPER, RM_CRY, RM_SYSMESSAGE, RM_SYSMESSAGE_EX, RM_GROUPMESSAGE,
    ///     RM_GUILDMESSAGE, RM_DELAYMESSAGE, RM_CENTERMESSAGE, RM_TOPCHATBOARDMESSAGE,
    ///     RM_MOVEMESSAGE, RM_MOVEMESSAGE_NEW, RM_SCREENMESSAGE, RM_CHANGESPEED, RM_SERVERCONFIG:
    ///       Result := False;
    ///   end;
    /// </code>
    ///
    /// ★ 语义：**这些标识"可被过滤"**（返回 False = 不过滤/走另一条路径），
    /// 其余一律 True。函数名有点反直觉 —— 返回 True 表示"需要走 `ProcessPlayObjectMessage`"。
    /// </summary>
    public static bool CanFilter(int wIdent)
    {
        // 原文 3725：Result := True;
        bool result = true;

        // 原文 3726-3731：case wIdent of ... Result := False; end;
        switch (wIdent)
        {
            case Grobal2Const.RM_HEAR:
            case PlayerSurfaceOperateConst.RM_WHISPER:
            case PlayerSurfaceOperateConst.RM_CRY:
            case PlayerSurfaceOperateConst.RM_SYSMESSAGE:
            case PlayerSurfaceOperateConst.RM_SYSMESSAGE_EX:
            case PlayerSurfaceOperateConst.RM_GROUPMESSAGE:
            case PlayerSurfaceOperateConst.RM_GUILDMESSAGE:
            case PlayerSurfaceOperateConst.RM_DELAYMESSAGE:
            case PlayerSurfaceOperateConst.RM_CENTERMESSAGE:
            case PlayerSurfaceOperateConst.RM_TOPCHATBOARDMESSAGE:
            case PlayerSurfaceOperateConst.RM_MOVEMESSAGE:
            case PlayerSurfaceOperateConst.RM_MOVEMESSAGE_NEW:
            case PlayerSurfaceOperateConst.RM_SCREENMESSAGE:
            case PlayerSurfaceOperateConst.RM_CHANGESPEED:
            case Grobal2Const.RM_SERVERCONFIG:
                result = false;
                break;
        }

        return result;
    }

    // ------------------------------------------------------------------
    // 原文 3699-3769：TPlayObject.Operate
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `function TPlayObject.Operate(ProcessMsg: pTProcessMessage): Boolean; override;`
    /// （声明 `ObjPlayer.pas:1224`，实现 `:3699-3769`）。
    ///
    /// 逐行对照：
    /// <list type="bullet">
    ///   <item><description>:3735 `Result := True;`</description></item>
    ///   <item><description>:3736-3740 `if (ProcessMsg = nil) or (ProcessMsg.wIdent = 0) then begin Result := False; Exit; end;`</description></item>
    ///   <item><description>:3742 `ProcessMessage := ProcessMsg^;` —— 把报文**按值复制**一份到过程级局部
    ///     （随后两条分支里分别用**原始指针**与**这份副本**的**地址**调用）。</description></item>
    ///   <item><description>:3744-3752 `if g_PluginManager &lt;&gt; nil then` 调 `HookPlayerProcessMsgBegin`，
    ///     **若插件把 boReturn 置 True 且 CanFilter 为真 → 直接 Exit**（此时 Result 仍是 True）。</description></item>
    ///   <item><description>:3754-3760 的两分支差异**只在传的是哪个地址**：
    ///     `CanFilter` 为真 → 传原始 `ProcessMsg`；为假 → 传副本 `@ProcessMessage`。
    ///     两边都判 `not ProcessPlayObjectMessage(...)` 才回落到 `inherited Operate`。</description></item>
    ///   <item><description>:3762-3768 收尾 `HookPlayerProcessMsgEnd`，**返回值被丢弃**。</description></item>
    /// </list>
    ///
    /// ⚠ 托管侧签名说明：原文 `ProcessMsg` 是可空指针，托管 <see cref="TProcessMessage"/> 是引用类型，
    /// 故第 2 步的 `nil` 判定用 `is null`；`ProcessMessage := ProcessMsg^` 用**浅拷贝**表达
    /// （原文 `TProcessMessage` 是值记录，`^` 即按值复制）。
    /// </remarks>
    public bool Operate(TProcessMessage processMsg)
    {
        // 原文 3735：Result := True;
        bool result = true;

        // 原文 3736-3740
        if (processMsg is null || processMsg.wIdent == 0)
        {
            // 原文 3738：Result := False;
            result = false;
            // 原文 3739：Exit;
            return result;
        }

        // 原文 3742：ProcessMessage := ProcessMsg^;   ← 按值复制一份
        TProcessMessage processMessage = CopyProcessMessage(processMsg);

        // 原文 3743：boReturn := False;
        var boReturn = new BoolRef(false);

        // 原文 3744-3752：if g_PluginManager <> nil then begin ... end;
        if (PlayerSurfaceOperateSeams.PluginManagerAvailable())
        {
            // 原文 3746-3748：HookPlayerProcessMsgBegin(Self, wIdent, wParam, nParam1, nParam2, nParam3,
            //                    TBaseObject(BaseObject), dwDeliveryTime, PAnsiChar(sMsg), boReturn);
            PlayerSurfaceOperateSeams.HookProcessMsgBegin(
                this, processMsg.wIdent, processMsg.wParam,
                processMsg.nParam1, processMsg.nParam2, processMsg.nParam3,
                processMsg.BaseObject, processMsg.dwDeliveryTime, processMsg.sMsg, boReturn);

            // 原文 3750-3751：if boReturn and CanFilter(ProcessMessage.wIdent) then Exit;
            //   ⚠ 注意：这里判的是**副本** processMessage.wIdent（与 3746 传的原始 wIdent 同值，但原文写的是副本）
            if (boReturn.Value && CanFilter((int)processMessage.wIdent))
                return result;   // 原文 Exit：Result 保持 True
        }

        // 原文 3754-3760
        if (CanFilter((int)processMessage.wIdent))
        {
            // 原文 3756-3757：if not ProcessPlayObjectMessage(ProcessMsg, Result) then Result := inherited Operate(ProcessMsg);
            if (!ProcessPlayObjectMessage(processMsg, ref result))
                result = PlayerSurfaceOperateSeams.InheritedOperate(this, processMsg);
        }
        else
        {
            // 原文 3759-3760：else if not ProcessPlayObjectMessage(@ProcessMessage, Result) then
            //                    Result := inherited Operate(@ProcessMessage);
            //   ⚠ 原文此分支传的是**副本的地址**（与上一分支不同）—— 逐字保留这个不对称。
            if (!ProcessPlayObjectMessage(processMessage, ref result))
                result = PlayerSurfaceOperateSeams.InheritedOperate(this, processMessage);
        }

        // 原文 3762：boReturn := False;
        //   ★ 原文如此：这里**重新赋了一个新的局部值**，但紧接着 3765 的
        //   HookPlayerProcessMsgEnd 传的是 **同一个 boReturn 变量**（Delphi 里它是过程级变量，
        //   所以 3762 的置 False **会**影响 3765 看到的初值）。托管侧用同一个 BoolRef 表达。
        boReturn.Value = false;

        // 原文 3763-3768：if g_PluginManager <> nil then HookPlayerProcessMsgEnd(...);
        if (PlayerSurfaceOperateSeams.PluginManagerAvailable())
        {
            PlayerSurfaceOperateSeams.HookProcessMsgEnd(
                this, processMsg.wIdent, processMsg.wParam,
                processMsg.nParam1, processMsg.nParam2, processMsg.nParam3,
                processMsg.BaseObject, processMsg.dwDeliveryTime, processMsg.sMsg, boReturn);
        }

        // 原文 3769：end;
        return result;
    }

    /// <summary>
    /// 原文 `ProcessMessage := ProcessMsg^;`（`ObjPlayer.pas:3742`）——
    /// `TProcessMessage` 在原文是**值记录**，`^` 是**按值复制**（托管引用类型需显式浅拷贝）。
    /// </summary>
    private static TProcessMessage CopyProcessMessage(TProcessMessage src) => new()
    {
        wIdent = src.wIdent,
        wParam = src.wParam,
        nParam1 = src.nParam1,
        nParam2 = src.nParam2,
        nParam3 = src.nParam3,
        BaseObject = src.BaseObject,
        dwTimeTick = src.dwTimeTick,
        dwDeliveryTime = src.dwDeliveryTime,
        boLateDelivery = src.boLateDelivery,
        sMsg = src.sMsg,
    };

    // ------------------------------------------------------------------
    // 原文 3772-5606：TPlayObject.Run —— 显式留痕（未移植）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `procedure TPlayObject.Run();`（`ObjPlayer.pas:3772-5606`，**1,835 行**）。
    ///
    /// <para><b>状态：未移植（显式留痕）</b>。按台账 §48.1，此处调用
    /// <see cref="PlayerSurfacePortLedger.NotPorted"/> 留痕，**不**写成裸 `=> true;`，
    /// 也**不**写一个"近似心跳"冒充移植。</para>
    ///
    /// <para><b>阻塞面（逐段，带原文行号）</b>：
    /// <list type="bullet">
    ///   <item><description>:3825-3833 `m_boDelayClose` / `m_dwDelayCloseTick` / `m_dwDelayCloseTime`
    ///     / `DoClientClose()` / `SendDefMessage(SM_SOFTCLOSE,...)`（`SendDefMessage` 有 p6 接缝，
    ///     但 `DoClientClose`（声明 :1127）未移植）</description></item>
    ///   <item><description>:3835-3840 `g_boExitServer`（M2Share 全局）</description></item>
    ///   <item><description>:3842-3854 `m_nClearDayVarTime` / `m_JVal` / `HourOf` / `g_Config.btPlayerVarJClearTime`
    ///     / `g_Config.boOpenCombatPowerCalc` / `g_Config.boOpenCombatPowerVarCalc` / `RecalcPlayCombatPower(Self)`
    ///     ／ 其中 `m_JVal`（CombatPower.cs 已声明）与 `m_ZVal` 可清零，但 `g_Config` 面与
    ///     `RecalcPlayCombatPower` 未移植</description></item>
    ///   <item><description>:3859-3887 `ProcessSafeZoneHint`（声明 :546，实现未移植）/ `m_boCollecting` 族
    ///     / `StopCollect` / `m_sInviteGroupHuman`（`THashedStringList`）</description></item>
    ///   <item><description>:3889 起（后续 1,700 余行）：`m_sVerifyCode` 族、`g_Config` 大量字段、
    ///     `TUserCastle`（Castle.cs 有类但缺所需成员）、`TItemObject`、`TCustomMagicConfig`、
    ///     `PClientBufInfo`/`PArrBufInfo`、`THeroObject`（托管侧未切出）、`g_CastleManager`、
    ///     `GetHighHuman`、`SendMsg(BaseObject, wIdent, ...)`（p6 报告 §8-G 登记的"同名不同义"待裁定项）</description></item>
    /// </list>
    /// 结论：**`Run` 的移植必须等上述面落地**，否则只能造替身（违反 §14.2）。
    /// 建议拆成 5~6 个子切片按段推进（:3825-:3888 / :3889-:4200 / … / :5400-5606），
    /// 每段各自登记依赖。</para>
    ///
    /// <para>⚠ 托管侧 <c>Engine/ObjBase.cs:208</c> 的 `public override void Run()`（2 行近似物）
    /// **保持原样不动** —— 本文件不重复声明 `Run`（会 CS0111）。
    /// 另注：原文 `Run` 返回 **void**，而 `TCreature` 基类的 `Run()` 也是 void ⇒ 无签名冲突，
    /// 未来移植时直接在别的片文件里 `public override void Run()` 覆盖即可，但**必须先删除
    /// `ObjBase.cs:208` 那个近似物**（需 `ObjBase.cs` 写权限，本车道没有）。</para>
    /// </summary>
    public void RunNotPortedMarker()
        => PortNotPorted(nameof(Run), 3772);
}

/// <summary>
/// 原文 `TPlayObject` 消息派发面用到的**常量**。
/// 托管侧 `GXX.Core.Protocol.Grobal2Const`（`Grobal2.Const.g.cs`）已收录大部分 `RM_*`/`SM_*`；
/// 此处只登记**该文件里没有的**少数几个，值取自原文 `Grobal2.pas` / `grobal2.pas`
/// （`grep -n "RM_WHISPER" _analysis/utf8_mirror/Common/Grobal2.pas` 可复核）。
/// </summary>
public static class PlayerSurfaceOperateConst
{
    /// <summary>
    /// 原文 `MAXCLIENTMESSAGECOUNT = 30000; // 22000`（**实测**：`ObjBase.pas:11`，
    /// 是该常量在全仓的唯一定义处 —— `grep -rn "MAXCLIENTMESSAGECOUNT\s*=" _analysis/utf8_mirror` 只此一条命中）。
    /// 原文注释 `// 22000` 记录了它上一个小版本的值（**原文如此**，保留注释）。
    /// </summary>
    public const int MAXCLIENTMESSAGECOUNT = 30000;

    // ---- 以下 13 个 RM_* 见 CanFilter（原文 3727-3729）；值**逐字取自 Grobal2.pas**（实测行号附后） ----

    /// <summary>原文 `RM_WHISPER = 20068; // 366;`（Grobal2.pas:1008）。</summary>
    public const int RM_WHISPER = 20068;
    /// <summary>原文 `RM_CRY = 20069; // 367;`（Grobal2.pas:1009）。</summary>
    public const int RM_CRY = 20069;
    /// <summary>原文 `RM_SYSMESSAGE = 20070; // 368;`（Grobal2.pas:1010）。</summary>
    public const int RM_SYSMESSAGE = 20070;
    /// <summary>原文 `RM_GROUPMESSAGE = 20071; // 369;`（Grobal2.pas:1011）。</summary>
    public const int RM_GROUPMESSAGE = 20071;
    /// <summary>原文 `RM_GUILDMESSAGE = 20072; // 370;`（Grobal2.pas:1012）。</summary>
    public const int RM_GUILDMESSAGE = 20072;
    /// <summary>原文 `RM_DELAYMESSAGE = 20073;`（Grobal2.pas:1014）。</summary>
    public const int RM_DELAYMESSAGE = 20073;
    /// <summary>原文 `RM_CENTERMESSAGE = 20074;`（Grobal2.pas:1015）。</summary>
    public const int RM_CENTERMESSAGE = 20074;
    /// <summary>原文 `RM_TOPCHATBOARDMESSAGE = 20075;`（Grobal2.pas:1016）。</summary>
    public const int RM_TOPCHATBOARDMESSAGE = 20075;
    /// <summary>原文 `RM_MOVEMESSAGE = 20076;`（Grobal2.pas:1017）。
    /// ⚠ `Grobal2.pas:1018` 的 `RM_MERCHANTSAY = 20077` **不在此 case 表内** —— 逐字保留。</summary>
    public const int RM_MOVEMESSAGE = 20076;
    /// <summary>原文 `RM_SCREENMESSAGE = 20078;`（Grobal2.pas:1019）。
    /// ⚠ 原文如此：`20077`（`RM_MERCHANTSAY`）与 `20079`（`RM_SUPERMOVEMESSAGE`）被**跳过**。</summary>
    public const int RM_SCREENMESSAGE = 20078;
    /// <summary>原文 `RM_CHANGESPEED = 20171;`（Grobal2.pas:1114）。</summary>
    public const int RM_CHANGESPEED = 20171;
    /// <summary>原文 `RM_MOVEMESSAGE_NEW = 20235; // 滚动消息 chongchong 2014-04-17`（Grobal2.pas:1189）。</summary>
    public const int RM_MOVEMESSAGE_NEW = 20235;
    /// <summary>原文 `RM_SYSMESSAGE_EX = 20266;`（Grobal2.pas:1223）。</summary>
    public const int RM_SYSMESSAGE_EX = 20266;
}

/// <summary>
/// 原文 `TPlayObjectMessageProcedure = procedure(ProcessMsg: pTProcessMessage; var boResult: Boolean)`
/// 的托管等价签名（`ObjPlayer.pas:3715` 的数组元素类型）。
/// ⚠ 必须自定义委托：C# 的 `Action&lt;T&gt;` 族**不支持 `ref` 形参**。
/// </summary>
public delegate void PlayerMessageHandler(TProcessMessage processMsg, ref bool boResult);

/// <summary>
/// 原文 `PlayObjectMessageArray: array[0..MAXCLIENTMESSAGECOUNT] of TPlayObjectMessageProcedure`
/// （`ObjPlayer.pas:3712` 用它按 `wIdent` 查派发函数）—— 托管侧的等价容器。
///
/// 原文在 `initialization` 段把各 `Client*` 过程逐个填进这张表；
/// 托管侧对应「本车道各切片把自己的 `Client*` 方法注册进来」，
/// 故此处给一个**可注册的稀疏表**（越界访问返回 null，与原文 `Assigned(...)` 判定等价）。
/// </summary>
public static class PlayerSurfaceMessageTable
{
    /// <summary>原文 `array[0..MAXCLIENTMESSAGECOUNT]` 的稀疏表达。</summary>
    private static readonly PlayerMessageHandler?[] s_table
        = new PlayerMessageHandler?[PlayerSurfaceOperateConst.MAXCLIENTMESSAGECOUNT + 1];

    /// <summary>原文 `PlayObjectMessageArray[I] := SomeProc;` 的注册侧。</summary>
    public static void Register(int wIdent, PlayerMessageHandler handler)
    {
        if (wIdent >= 0 && wIdent < s_table.Length)
            s_table[wIdent] = handler;
    }

    /// <summary>原文 `Assigned(PlayObjectMessageArray[I])` 的判定侧。</summary>
    public static PlayerMessageHandler? Get(int wIdent)
        => wIdent >= 0 && wIdent < s_table.Length ? s_table[wIdent] : null;

    /// <summary>原文 `PlayObjectMessageArray` 整体的只读视图（`ProcessPlayObjectMessage` 用下标访问）。</summary>
    public static PlayerMessageHandler?[] Snapshot => s_table;

    /// <summary>清空注册表（单测隔离用）。</summary>
    public static void Clear()
    {
        for (int i = 0; i < s_table.Length; i++) s_table[i] = null;
    }
}

/// <summary>
/// `TPlayObject` 上 `PlayObjectMessageArray` 的**实例侧视图**（原文它是单元级全局数组，
/// 但注册内容按 wIdent 全局唯一，故托管侧做成静态表 + 实例转发，语义一致）。
/// </summary>
public partial class TPlayObject
{
    /// <summary>原文 `PlayObjectMessageArray`（单元级全局数组）的只读视图。</summary>
    private static PlayerMessageHandler?[] PlayObjectMessageArray
        => PlayerSurfaceMessageTable.Snapshot;
}
