using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// `ClientSession.pas:157-401 ProcessCltData` 的包头门控 + `:462-687` 的 `CM_*` 分派
/// → `Rest11LoginGatePacketGate.cs`
///
/// <para>
/// ★ 车道 **p14-logingate-wire** 新增。p11 只移植了 `ProcessCltData` 的**状态面**
/// （`m_dwProtocolPassword` / `m_IsCanSetL2Password` / `m_IsCanCheckL2Password` / `DelayClose`），
/// 因此那批设施"可接线"但**没接线**。本类把门控与分派**逐行落地并接上**
/// （宿主 = `GXX.LoginGate.LoginGateService`）。
/// </para>
///
/// <para>
/// ⚠ 与既有设施的关系：
/// <list type="bullet">
///   <item>落在 `GXX.GatewayKit`（而**不是** `GXX.LoginGate`）是为了能被
///         `tests/GXX.GatewayKit.Tests/Rest14*.cs` **直接单测**（本工程不引用 `GXX.LoginGate`）；
///         LoginGate 侧只提供 <see cref="IRest11PacketGateHost"/> 接缝（`SendRawToClient` /
///         `CloseClientSession` / `ForwardFrameToServer` / `AddRest11Log` 四个既有出口）。</item>
///   <item>踢线/封禁**不重写**：一律走
///         <see cref="Rest11LoginGateMisc.KickUser(IRest11SessionObj, Rest11LoginGateConfig, IRest11EnforcementChannel)"/>。</item>
///   <item>装帧**不重写**：走 <see cref="Rest11LoginGateSession.BuildDefMessageFrame"/>。</item>
///   <item>`CltCmd` 解码**不重写**：走 <see cref="EDcode.DecodeMessage(byte[])"/>。</item>
/// </list>
/// </para>
///
/// <para>
/// 原文缺陷**照抄**（逐条断言见 `tests/GXX.GatewayKit.Tests/Rest14LoginGateWireTests.cs`）：
/// <list type="number">
///   <item>`:189-194` `m_fKickFlag` 早退时把标志**反转回 False**（而不是保留）；</item>
///   <item>`:196-203` 超长判据是 `Len > m_nNomClientPacketSize`（**严格大于**）；</item>
///   <item>`:207-217` `'HTTP/'` 只在 `m_fDefenceCCPacket` 打开时查，且 `Len >= 5` 才查；</item>
///   <item>`:219-229` `'$'` **无条件**查（`Len >= 1`），与 `m_fDefenceCCPacket` 无关；</item>
///   <item>`:242-247` `Len < DEF_BLOCK_SIZE(22)` 直接踢线；</item>
///   <item>`:462` `m_fHandleLogin := 2` **无条件**赋值；</item>
///   <item>`:489-520` L2 门控是 `if/else if` 链 ⇒ 两个许可**互不消费**；</item>
///   <item>`:582-678` 原文该大段被 `(* *)` 注释包住 ⇒ `CM_ADDNEWUSER` 等落到 `else` 被踢线；</item>
///   <item>`:679-685` `else` 对未列入白名单的 `CM_*` 一律踢线（并记 `错误的数据包索引`）。</item>
/// </list>
/// </para>
/// </summary>
public sealed class Rest11LoginGatePacketGate
{
    private readonly Rest11LoginGateConfig _config;
    private readonly IRest11EnforcementChannel _channel;
    private readonly Func<IReadOnlyList<IRest11SessionObj?>> _userList;

    /// <summary>宿主接缝（`SendDefMessage` / `FreeSocket` / 转发 / 日志四个既有出口）。</summary>
    public IRest11PacketGateHost? Host { get; set; }

    /// <summary>
    /// 宿主日志接缝（`g_pLogMgr`）。未提供时只走 <see cref="IRest11EnforcementChannel"/> 的
    /// `AddLog`/`CheckLevel`（p11 既有出入口）。
    /// </summary>
    public IRest11GateLogger? Logger { get; set; }

    /// <summary>`:259-401` 协议密码校验块**未移植**而放行的包数（显式留痕，见 §"未完成"）。</summary>
    public long ProtocolPasswordGateSkipped;

    /// <summary>`:538 DelayClose(1000)` 施加次数（`CM_MACHINEID` 版本不符）。</summary>
    public long DelayClose1000Applied;

    /// <summary>尚未 1:1 移入方法体的**显式留痕**（§48.1 / §25.2：绝不静默返回默认值）。</summary>
    public readonly List<string> NotPortedMethods = new();

    public Rest11LoginGatePacketGate(Rest11LoginGateConfig config,
                                     IRest11EnforcementChannel channel,
                                     Func<IReadOnlyList<IRest11SessionObj?>> userList)
    {
        _config = config;
        _channel = channel;
        _userList = userList;
    }

    /// <summary>`ProcessCltData` 在**进入 `:462` 的 `CM_*` 分派之前**的三种去向。</summary>
    public readonly struct ProcessResult
    {
        /// <summary>`ContinueToDispatch` / `Kick` / `KickFlagReversed`。</summary>
        public readonly Rest11ProcessCltResult Result;

        /// <summary>`CM_*` 分派结果（仅当 <see cref="Result"/> 为 `ContinueToDispatch` 时有意义）。</summary>
        public readonly Rest11CommandDispatchResult Dispatch;

        /// <summary>`:538` 是否已施加 `DelayClose(1000)`。</summary>
        public readonly bool DelayClose1000;

        public ProcessResult(Rest11ProcessCltResult result,
                             Rest11CommandDispatchResult dispatch = Rest11CommandDispatchResult.ForwardToGameSvr,
                             bool delayClose1000 = false)
        {
            Result = result;
            Dispatch = dispatch;
            DelayClose1000 = delayClose1000;
        }

        /// <summary>宿主是否需要关闭连接（`Kick` / `KickFlagReversed` 都要求关）。</summary>
        public bool NeedsClose => Result != Rest11ProcessCltResult.ContinueToDispatch;
    }

    /// <summary>
    /// `ClientSession.pas:157-401` 的包头门控 + `:462-687` 的 `CM_*` 分派。
    ///
    /// <paramref name="frame"/> 是**一个完整帧的载荷**（对应原文 `AppMain.pas:1096-1106`：
    /// `pTRBuffer` 找到 `'!'` 后 `Inc(pTRBuf, 2)` 跳过 `'#'` + 1 字节，
    /// `iLen := pTRBuffer - pTRBuf` ⇒ 即 `#` 与 `!` **之间**的字节），
    /// <paramref name="frameLen"/> 即原文 `ProcessCltData(const Addr, Len)` 的 `Len`。
    /// </summary>
    public ProcessResult ProcessClientFrame(IRest11SessionObj session, byte[] frame, int frameLen)
    {
        int len = frameLen;

        // ---- :189-194 m_fKickFlag 早退（原文"直接反转 m_fKickFlag 标志，退出"）----
        if (session.KickFlag)
        {
            session.KickFlag = false;                              // :191
            return new ProcessResult(Rest11ProcessCltResult.KickFlagReversed);
        }

        // ---- :196-203 数据包超长（严格大于）----
        if (len > _config.m_nNomClientPacketSize)
        {
            Log(4, "数据包超长: " + DelphiRTL.IntToStr(len));      // :198-199
            Kick(session);                                         // :200
            return new ProcessResult(Rest11ProcessCltResult.Kick);
        }

        // ---- :205 PByte(Addr + Len)^ := 0（Delphi 靠缓冲尾部 #0 终止；托管以长度表达）----

        // ---- :207-217 CC 攻击（只在 m_fDefenceCCPacket 打开时查 'HTTP/'，且 Len >= 5）----
        if (len >= 5 && _config.m_fDefenceCCPacket)
        {
            if (IndexOfAscii(frame, len, "HTTP/") >= 0)            // :209 StrPos(PChar(Addr), 'HTTP/')
            {
                Log(6, "CC Attack, Kick: " + session.IPText);      // :211-212
                Kick(session);                                     // :213
                return new ProcessResult(Rest11ProcessCltResult.Kick);
            }
        }

        // ---- :219-229 '$' 攻击（原文**无条件**，与 m_fDefenceCCPacket 无关）----
        if (len >= 1)
        {
            if (IndexOfAscii(frame, len, "$") >= 0)                // :221 StrPos(PChar(Addr), '$')
            {
                Log(6, "$ Attack, Kick: " + session.IPText);       // :223-224
                Kick(session);                                     // :225
                return new ProcessResult(Rest11ProcessCltResult.Kick);
            }
        }

        // ---- :231-236 gDeny（ClientSession.pas:53 的模块级全局）----
        if (Rest11LoginGateSession.gDeny)
        {
            Kick(session);                                         // :233
            return new ProcessResult(Rest11ProcessCltResult.Kick);
        }

        // ---- :242-247 首包长度下限（DEF_BLOCK_SIZE = 22）----
        if (len < Grobal2Const.DEF_BLOCK_SIZE)
        {
            Kick(session);                                         // :244
            return new ProcessResult(Rest11ProcessCltResult.Kick);
        }

        // ---- :249-254 解出包头 + 包体 ----
        byte[] head = new byte[Grobal2Const.DEF_BLOCK_SIZE];
        Array.Copy(frame, 0, head, 0, Grobal2Const.DEF_BLOCK_SIZE);
        TDefaultMessage cltCmd = EDcode.DecodeMessage(head);        // :249 DecodeMessage(PAnsiChar(Addr), DEF_BLOCK_SIZE)
        int bodyLen = len - Grobal2Const.DEF_BLOCK_SIZE;            // :250 SetLength(sRecv, Len - DEF_BLOCK_SIZE)
        byte[] body = new byte[bodyLen < 0 ? 0 : bodyLen];
        if (bodyLen > 0) Array.Copy(frame, Grobal2Const.DEF_BLOCK_SIZE, body, 0, bodyLen); // :253

        // ---- :259-401 协议密码校验块：**未移植** ----
        //   逐字节依赖 `m_pOverlapRecv.ABuffer` 的原地内存布局（`PAnsiChar(Addr)` 指针算术、
        //   `DecryptDes(sRecv[1], ...)` 原地解密）与 `DecodeString` 的**未解码**入参；
        //   前者属 `AcceptExWorkedThread.pas`（本仓 0 托管声明）。缺它时：
        //     · **不**踢线（原文只有 `IsPackError` 才踢，`:394-401`）⇒ 不引入新的误杀；
        //     · `IsPackError` 分支**当前不可达**；
        //     · 留痕计数见 ProtocolPasswordGateSkipped / NotPortedMethods（非静默）。
        ProtocolPasswordGateSkipped++;
        NotPorted("ProcessCltData:259-401 协议密码校验块", 259);

        // ==== :462-687 CM_* 分派 ====
        var d = DispatchCommand(session, cltCmd, body);
        return new ProcessResult(Rest11ProcessCltResult.ContinueToDispatch, d.Result, d.DelayClose1000);
    }

    /// <summary>
    /// 一次接收里切出的**全部**帧（原文 `AppMain.pas:1086-1124` 的 `LOOP:` 循环：
    /// 逐个 `#`…`!` 调 `ProcessCltData`，踢线即 `Break`）。
    /// </summary>
    /// <returns>true 表示需要宿主关闭连接（原文 `Succeed := False`）。</returns>
    public bool ProcessClientFrames(IRest11SessionObj session, IReadOnlyList<byte[]> frames)
    {
        for (int i = 0; i < frames.Count; i++)
        {
            byte[] f = frames[i];
            ProcessResult r = ProcessClientFrame(session, f, f.Length);

            if (r.Result == Rest11ProcessCltResult.ContinueToDispatch)
            {
                if (r.Dispatch == Rest11CommandDispatchResult.ForwardToGameSvr)
                {
                    Host?.ForwardFrameToServer(session, f);        // 原文 `:569-579`
                    continue;
                }
                if (r.Dispatch == Rest11CommandDispatchResult.HandledExit)
                    continue;                                      // :566 Exit：只处理不转发
                // Kick ⇒ 落到下面统一关连接
            }

            if (r.DelayClose1000) DelayClose1000Applied++;          // :538 已由 DispatchCommand 置位
            Host?.CloseClientSession(session);                      // 原文 `Succeed := False`
            return true;
        }
        return false;
    }

    /// <summary>`DispatchCommand` 的返回。</summary>
    public readonly struct DispatchResult
    {
        /// <summary>`CM_*` 分派结果。</summary>
        public readonly Rest11CommandDispatchResult Result;

        /// <summary>`SM_CHECKCLIENTVERSION_FAIL` 分支要求 `DelayClose(1000)`（`:538`）。</summary>
        public readonly bool DelayClose1000;

        public DispatchResult(Rest11CommandDispatchResult result, bool delayClose1000 = false)
        {
            Result = result;
            DelayClose1000 = delayClose1000;
        }
    }

    /// <summary>
    /// `ClientSession.pas:462-687` 的 `CM_*` 分派。
    ///
    /// <para>
    /// 原文该块开头是**无条件**的 `m_fHandleLogin := 2;`（`:462`，不在 `if` 内），
    /// 紧接着 `if m_fHandleLogin = 2 then`（`:464`）—— 托管照抄这两步。
    /// </para>
    /// </summary>
    public DispatchResult DispatchCommand(IRest11SessionObj session, TDefaultMessage cltCmd, byte[] body)
    {
        session.HandleLogin = 2;                                        // :462 ★ 无条件

        if (session.HandleLogin != 2)                                   // :464（原文恒真）
            return new DispatchResult(Rest11CommandDispatchResult.ForwardToGameSvr);

        switch ((int)cltCmd.Ident)                                      // :466 case CltCmd.Ident of
        {
            case Grobal2Const.CM_GETVERIFICATIONCODE:                   // :467
            case Grobal2Const.CM_PHONELOGIN:                            // :468
            case Grobal2Const.CM_REALNAME:                              // :469
            case Grobal2Const.CM_GETPASSWORDBACK_PHONE:                 // :470
            case Grobal2Const.CM_GETPASSWORDBACK:                       // :471
            case Grobal2Const.CM_CHANGEPHONE:                           // :472
            case Grobal2Const.CM_CHANGEPASSWORD_NEW:                    // :473
            case Grobal2Const.CM_CREATEACCOUNT:                         // :474
            case Grobal2Const.CM_BINDPHONE:                             // :475
            case Grobal2Const.CM_QUICKLOGIN:                            // :476
            case Grobal2Const.CM_MACHINEID:                             // :477
            case Grobal2Const.CM_IDPASSWORD:                            // :478
            case Grobal2Const.CM_ADDNEWUSER:                            // :479
            case Grobal2Const.CM_UPDATEUSER:                            // :480
            case Grobal2Const.CM_SELECTSERVER:                          // :481
            case Grobal2Const.CM_CHANGEPASSWORD:                        // :482
            case Grobal2Const.CM_CHANGERANDOMCODE:                      // :483
            case Grobal2Const.CM_CHECKRANDOMCODE:                       // :484
            case Grobal2Const.CM_SETL2PASSWORD:                         // :485
            case Grobal2Const.CM_GETBACKPASSWORD:                       // :486
            case Grobal2Const.CM_CHECKL2PASSWORD:                       // :487
            {
                if (cltCmd.Ident == Grobal2Const.CM_SETL2PASSWORD)      // :489
                {
                    if (!session.m_IsCanSetL2Password)                  // :491
                    {
                        Log(1, "非法设置二级密码: " + session.IPText);  // :493-494
                        Kick(session);                                  // :496
                        return new DispatchResult(Rest11CommandDispatchResult.Kick);   // :497-498
                    }
                    session.m_IsCanSetL2Password = false;               // :502
                }
                else if (cltCmd.Ident == Grobal2Const.CM_CHECKL2PASSWORD)   // :505
                {
                    if (!session.m_IsCanCheckL2Password)                // :507
                    {
                        Log(1, "非法检测二级密码:  " + session.IPText); // :509-510 ★ 原文双空格
                        Kick(session);                                  // :512
                        return new DispatchResult(Rest11CommandDispatchResult.Kick);   // :513-514
                    }
                    session.m_IsCanCheckL2Password = false;             // :518
                }
                else if (cltCmd.Ident == Grobal2Const.CM_MACHINEID)     // :521
                {
                    // :523-540 的版本号判定：`sRecv := DecodeString(sRecv); sSoftVersion := GetValidStr3(...)`
                    //   依赖**已解码**的包体（属未移植块），故只在包体可解出 '/' 前缀时比较；
                    //   原文 `if not SameText(m_sClientSoftVer, sSoftVersion)` —— 配置为空串且包体非空时
                    //   原文会踢，托管同样会（`'' != "x"`）。
                    if (_config.m_boCheckVersion)                       // :525
                    {
                        string softVersion = ExtractSoftVersion(body);  // :523-524
                        if (!SameText(_config.m_sClientSoftVer, softVersion))  // :527 not SameText
                        {
                            byte[] fail = Rest11LoginGateSession.BuildDefMessageFrame(
                                Grobal2Const.SM_CHECKCLIENTVERSION_FAIL, 0, 0, 0, 0, "");  // :529
                            Host?.SendRawToClient(session.Socket, fail);               // :533
                            session.DelayClose(1000);                                  // :538
                            return new DispatchResult(Rest11CommandDispatchResult.HandledExit, true);
                        }
                    }
                    return new DispatchResult(Rest11CommandDispatchResult.HandledExit); // :566 Exit（**不转发**）
                }

                // :569-579 把包转发给 m_tLastGameSvr（`A%d/#1%s!$`）
                return new DispatchResult(Rest11CommandDispatchResult.ForwardToGameSvr);
            }

            // :582-678 原文该大段被 `(* ... *)` 注释包住（CM_IDPASSWORD / CM_ADDNEWUSER 等）
            //          ⇒ 落到下面的 else 分支。**照抄**（不"顺手恢复"）。

            default:                                                    // :679 else
                Log(4, "错误的数据包索引: " + DelphiRTL.IntToStr(cltCmd.Ident));  // :681-682
                Kick(session);                                              // :683
                return new DispatchResult(Rest11CommandDispatchResult.Kick);
        }
    }

    /// <summary>
    /// `AppMain.pas:747-752`：**服务器 → 网关**方向给出的 `SM_*` 置位二级密码许可 /
    /// 触发 `DelayClose(8000)`。这是 L2 门控的**上游**（`ClientThread.pas` 里没有这段）。
    ///
    /// <para>
    /// 不接线这一侧 ⇒ `m_IsCanSetL2Password` 恒 false ⇒ `CM_SETL2PASSWORD` 恒被踢线
    /// （原文 `:491`）⇒ 登录流程会被本设施自己打断。因此本方法是"L2 门控真正可用"的必要条件。
    /// </para>
    /// </summary>
    public void OnServerMessage(IRest11SessionObj session, TDefaultMessage cmd)
    {
        if (cmd.Ident == Grobal2Const.SM_SETL2PASSWORD)                 // :747
            session.m_IsCanSetL2Password = true;                        // :748
        else if (cmd.Ident == Grobal2Const.SM_CHECKL2PASSWORD)          // :749
            session.m_IsCanCheckL2Password = true;                      // :750
        else if (cmd.Ident == Grobal2Const.SM_SELECTSERVER_OK)          // :751
            session.DelayClose(8000);                                   // :752
    }

    // =====================================================================================
    // 内部工具
    // =====================================================================================

    /// <summary>`Misc.KickUser(const UserObj)` 的既有一份实现（不重写）。</summary>
    private void Kick(IRest11SessionObj session)
        => Rest11LoginGateMisc.KickUser(session, _config, _channel);

    /// <summary>
    /// `if g_pLogMgr.CheckLevel(n) then g_pLogMgr.Add(msg)` 的两步门。
    ///
    /// <para>
    /// 只走 <see cref="Logger"/>（宿主日志接缝）；**不**走
    /// <see cref="IRest11EnforcementChannel.AddLog"/>——那是 p11 执法面（`Misc`/`FuncForComm`）
    /// 的出入口，两个接缝同时写会造成重复日志与门控失配（录见 D-P14-09）。
    /// </para>
    /// </summary>
    private void Log(int level, string msg)
    {
        if (Logger?.CheckLevel(level) == true) Logger.AddLog(msg);
    }

    /// <summary>`StrPos(PChar(Addr), Sub)` 的等价：在 `[0, len)` 内查 ASCII 子串，返回下标或 -1。</summary>
    internal static int IndexOfAscii(byte[] buf, int len, string sub)
    {
        int n = Math.Min(len, buf.Length);
        if (sub.Length == 0 || n < sub.Length) return -1;
        for (int i = 0; i + sub.Length <= n; i++)
        {
            bool hit = true;
            for (int j = 0; j < sub.Length; j++)
            {
                if (buf[i + j] != (byte)sub[j]) { hit = false; break; }
            }
            if (hit) return i;
        }
        return -1;
    }

    /// <summary>`DelphiSystem.SameText`（大小写不敏感）。</summary>
    internal static bool SameText(string a, string b)
        => string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// `AppMain.pas:524 GetValidStr3(sRecv, sMachineID, ['/'])` 的前半段（软件版本号）。
    ///
    /// <para>
    /// ★ **偏离登记 D-P14-10**：原文 `:523` 先 `sRecv := DecodeString(sRecv)`，而 `DecodeString`
    /// 的**入参口径由 :259-401 那段未移植的协议密码块决定**（该块把 `sRecv` 原地解密后
    /// 再交给这里）。本车道因此无法确定 `sRecv` 到底是"已解码明文"还是"待解码密文"：
    /// 直接 `DecodeString` 会在明文输入上产出不可读结果，跳过它则在密文输入上同样不可读。
    /// </para>
    /// <para>
    /// 采用**双路探测 + 只认可打印 ASCII** 的保守策略：两条路都不像软件版本号时返回 `""`
    /// ⇒ 上层 `DispatchCommand` 会因为 `m_sClientSoftVer` 非空而判定"版本不符"并走
    /// `SM_CHECKCLIENTVERSION_FAIL`（原文 `:527-539` 的分支），**不静默放行**。
    /// 版本号门控的完整 1:1 依赖 `:259-401`，登记为未完成项（见报告 §未完成）。
    /// </para>
    /// </summary>
    public static string ExtractSoftVersion(byte[] body)
    {
        if (body == null || body.Length == 0) return "";

        // 首选原文路径：`:523 sRecv := DecodeString(sRecv)` 后再取 '/' 之前
        string decodedText = TrimToPrintable(EncodingInit.GBK.GetString(EDcode.DecodeString(body)));
        if (decodedText.IndexOf('/') >= 0) return PrefixBeforeSlash(decodedText);

        // 回退：若包体本身就是明文（未再编码），原文的 DecodeString 只是个恒等包装
        string rawText = TrimToPrintable(EncodingInit.GBK.GetString(body));
        if (rawText.IndexOf('/') >= 0) return PrefixBeforeSlash(rawText);

        // 两条路都取不到版本号 ⇒ 返回空串（上层按"版本不符"处理，**不静默放行**）
        return decodedText.Length > 0 ? PrefixBeforeSlash(decodedText) : PrefixBeforeSlash(rawText);
    }

    /// <summary>`GetValidStr3(sRecv, sMachineID, ['/'])` 的"取第一个 '/' 之前"。</summary>
    private static string PrefixBeforeSlash(string s)
    {
        int slash = s.IndexOf('/');
        return slash >= 0 ? s.Substring(0, slash) : s;
    }

    /// <summary>截到第一个不可打印字符（<c>0x20..0x7E</c> 以外）为止；用于判别"像不像版本号"。</summary>
    private static string TrimToPrintable(string s)
    {
        int n = 0;
        while (n < s.Length && s[n] >= ' ' && s[n] <= '~') n++;
        return s.Substring(0, n);
    }

    /// <summary>`_userList` 的当前快照（`CloseIPConnect` 遍历用）。</summary>
    internal IReadOnlyList<IRest11SessionObj?> UserList => _userList();

    private void NotPorted(string method, int line)
        => NotPortedMethods.Add(method + " (ClientSession.pas:" + line + ")");
}

/// <summary>
/// `ProcessCltData` 的 `CM_*` 分派需要的**宿主四出口**（`GXX.LoginGate.LoginGateService` 实现它）。
///
/// <para>
/// 这样 `GXX.GatewayKit.Rest11` 能承载 1:1 的分派逻辑，而**不引用** `GXX.LoginGate`
/// （依赖方向 GatewayKit ← LoginGate），同时让 `tests/GXX.GatewayKit.Tests` 可直接单测。
/// 四个出口在 LoginGate 侧全部落到**既有**设施：
/// `SendRawToClient → IocpManager.Send`、`CloseClientSession → IocpManager.CloseSession`、
/// `ForwardFrameToServer → GateService.SendToServer(GM_DATA)`、
/// `AddRest11Log → GateService.SendLog`。
/// </para>
/// </summary>
public interface IRest11PacketGateHost
{
    /// <summary>`m_tIOCPSender.SendData(m_pOverlapSend, ...)`（`:443`/`:533`）。</summary>
    void SendRawToClient(int socket, byte[] frame);

    /// <summary>关闭连接（原文 `Succeed := False` 后由宿主 `SHSocket.FreeSocket`）。</summary>
    void CloseClientSession(IRest11SessionObj session);

    /// <summary>`m_tLastGameSvr.SendBuffer('A%d/#1%s!$')`（`:578-579`）。</summary>
    void ForwardFrameToServer(IRest11SessionObj session, byte[] frame);

    /// <summary>`g_pLogMgr.Add(sMsg)`（`:199` 等）。</summary>
    void AddRest11Log(string msg);
}

/// <summary>
/// `LogManager.pas` 的 `g_pLogMgr` 接缝（`CheckLevel` + `Add`）。
///
/// <para>
/// 与 <see cref="IRest11EnforcementChannel"/> 上的同名两个成员并存：
/// 前者由 p11 的执法面（`Misc`/`FuncForComm`）使用，后者供 `ProcessCltData` 的包头门控
/// （`ClientSession.pas:198/:211/:223/:681/:493/:509`）使用。`GXX.LoginGate` 侧两个都指向
/// 同一个既有日志出口（`GateService.SendLog` + `m_nShowLogLevel` 门）。
/// </para>
/// </summary>
public interface IRest11GateLogger
{
    /// <summary>`g_pLogMgr.CheckLevel(nLevel)`。</summary>
    bool CheckLevel(int level);

    /// <summary>`g_pLogMgr.Add(sMsg)`。</summary>
    void AddLog(string sMsg);
}
