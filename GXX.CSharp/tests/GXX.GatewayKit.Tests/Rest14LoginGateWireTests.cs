using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

/// <summary>
/// 车道 **p14-logingate-wire** 的门禁用例（对已接线的实现写直接单测）：
/// <list type="number">
///   <item><b>默认 OFF 等价性</b>：`GateService` 的 opt-in 接缝在 `Rest11Kernel == null` 时
///         **零新分支** —— 既有 `_blockList`/`_perIP` 判定链与计数行为与接线前一致；</item>
///   <item><b>打开后走原文语义</b>：`CheckIP` 走 `AcceptExWorkedThread.pas:576/587/598`
///         的 `IsBlockIP`（两张表）/ `IsBlockIPArea` / `OverConnectOfIP`（`Count+1 > Max`）；</item>
///   <item><b>`ProcessCltData` 门控与 `CM_*` 分派</b>（`ClientSession.pas:189-687`）逐分支；</item>
///   <item><b>换 ID 频率 / 客户端超时 / DelayClose</b> 三条计时器路径。</item>
/// </list>
/// </summary>
public class Rest14LoginGateWireTests : IDisposable
{
    private readonly string _dir;

    public Rest14LoginGateWireTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "rest14-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        Rest11LoginGateSession.gDeny = false;
        try { Directory.Delete(_dir, true); } catch { }
    }

    // =====================================================================================
    // 测试替身
    // =====================================================================================

    /// <summary>`IRest11PacketGateHost` + `IRest11GateLogger` 的替身：记录五个宿主出口。</summary>
    internal sealed class FakePacketHost : IRest11PacketGateHost, IRest11GateLogger
    {
        public readonly List<int> SentSockets = new();
        public readonly List<byte[]> SentFrames = new();
        public readonly List<int> ClosedSockets = new();
        public readonly List<byte[]> ForwardedFrames = new();
        public readonly List<string> Logs = new();

        /// <summary>`g_pLogMgr.CheckLevel` 的替身门槛（默认全开）。</summary>
        public Func<int, bool> CheckLevelFn { get; set; } = _ => true;

        public void SendRawToClient(int socket, byte[] frame)
        {
            SentSockets.Add(socket);
            SentFrames.Add(frame);
        }

        public void CloseClientSession(IRest11SessionObj session) => ClosedSockets.Add(session.Socket);

        public void ForwardFrameToServer(IRest11SessionObj session, byte[] frame)
            => ForwardedFrames.Add(frame);

        public void AddRest11Log(string msg) => Logs.Add(msg);

        bool IRest11GateLogger.CheckLevel(int level) => CheckLevelFn(level);
        void IRest11GateLogger.AddLog(string sMsg) => Logs.Add(sMsg);
    }

    private sealed class Harness
    {
        public Rest11LoginGateConfig Cfg = null!;
        public FakeEnforcementChannel Channel = null!;
        public FakePacketHost Host = null!;
        public Rest11LoginGatePacketGate Gate = null!;
        public List<string> Logs => Host.Logs;
    }

    /// <summary>
    /// 组装配置 + 替身。**默认 `m_nShowLogLevel = 6`**（原文 `CheckLevel(1..6)` 的最高档），
    /// 使日志断言可观测；需要验证日志**门控**的用例自行把它调低（见
    /// `DispatchCommand_LogGateRespectsShowLogLevel`）。
    /// </summary>
    private Harness NewHarness(Action<Rest11LoginGateConfig>? tune = null)
    {
        var h = new Harness
        {
            Cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_nShowLogLevel = 6 }
        };
        tune?.Invoke(h.Cfg);
        h.Channel = new FakeEnforcementChannel();
        h.Host = new FakePacketHost
        {
            // `g_pLogMgr.CheckLevel` 的实现：与 LoginGate 侧一致，看 `m_nShowLogLevel`
            CheckLevelFn = level => h.Cfg.m_nShowLogLevel >= level
        };
        h.Gate = new Rest11LoginGatePacketGate(h.Cfg, h.Channel, () => new List<IRest11SessionObj?>())
        {
            Host = h.Host,
            Logger = h.Host
        };
        return h;
    }

    /// <summary>
    /// 造一段 `ProcessCltData` 收敛的**帧载荷**（`#` 与 `!` 之间的字节）。
    ///
    /// <para>
    /// 原文 `:249 DecodeMessage(PAnsiChar(Addr), DEF_BLOCK_SIZE)` 解的是**6-bit 编码后的**包头
    /// （发送侧 `ClientSession.pas:569 EncodeMessage(CltCmd)`），故这里同样用
    /// `EDcode.EncodeMessage` 生成前 22 字节，剩余位置补 0；包体追加在 22 字节之后
    /// （原文 `:253 Move(PChar(Addr + DEF_BLOCK_SIZE)^, sRecv[1], ...)` 的等价位置）。
    /// </para>
    /// </summary>
    private static byte[] BuildFramePayload(ushort ident, long recog = 0, string body = "")
    {
        var cmd = new TDefaultMessage { Recog = recog, Ident = ident };
        byte[] encoded = EDcode.EncodeMessage(cmd);              // 16 字节 → 22 字节（6-bit）
        byte[] bodyBytes = body.Length == 0 ? Array.Empty<byte>() : EncodingInit.GBK.GetBytes(body);
        int headerLen = Math.Max(Grobal2Const.DEF_BLOCK_SIZE, encoded.Length);
        var buf = new byte[headerLen + bodyBytes.Length];
        Array.Copy(encoded, buf, Math.Min(encoded.Length, headerLen));
        if (bodyBytes.Length > 0)
            Array.Copy(bodyBytes, 0, buf, headerLen, bodyBytes.Length);
        return buf;
    }

    /// <summary>
    /// 造一段**编码后**的包体，使其经原文 `:523 sRecv := DecodeString(sRecv)` 后
    /// 恰好等于 <paramref name="plainText"/>（发送侧 `ClientSession.pas:570-572` 的 EncodeString）。
    /// </summary>
    private static byte[] EncodedBody(string plainText) => EDcode.EncodeString(plainText);

    /// <summary>
    /// 脚手架自检：`EncodedBody` 与原文 `:523 DecodeString` 必须成对往返
    /// —— 这是机器版本号两个用例的前提（不是原文行为，是本车道测试脚手架的守卫）。
    /// </summary>
    [Fact]
    public void TestHarness_EncodedBodyRoundTripsThroughDecodeString()
    {
        Assert.Equal("2025/04/28/machine",
                     EncodingInit.GBK.GetString(EDcode.DecodeString(EncodedBody("2025/04/28/machine"))));
        Assert.Equal("1999/01/01/machine",
                     EncodingInit.GBK.GetString(EDcode.DecodeString(EncodedBody("1999/01/01/machine"))));
    }

    /// <summary>造一个只带 `Ident` 的 `TDefaultMessage`（原文 `CltCmd.Ident` 是唯一判据）。</summary>
    private static TDefaultMessage Msg(int ident) => new() { Ident = (ushort)ident };

    // =====================================================================================
    // A-1  默认 OFF 等价性：opt-in 接缝在未接线时不做任何新动作
    // =====================================================================================

    /// <summary>
    /// `Rest11Kernel == null` ⇒ `GateService.CheckIP` 的 Rest11 分支**一行都不执行**：
    /// 既有设施仍在（`AddBlockIP` 依旧拦、`MaxConnOfIPaddr` 依旧限制），
    /// 且 `Rest11DenyCount`（Rest11 日志出口）恒 0 —— 这是"关闭时等价"的机器可读证明。
    /// </summary>
    [Fact]
    public void DefaultPath_Rest11BranchesAreNeverEntered()
    {
        using var gate = new Rest11NullProbeGate();
        Assert.Null(gate.Rest11OptionsProbe);       // 未接线
        Assert.Null(gate.Rest11KernelProbe);

        gate.MaxConnOfIPaddr = 2;
        gate.Accept("10.1.1.1", 101);
        gate.Accept("10.1.1.1", 102);
        gate.Accept("10.1.1.1", 103);
        gate.Accept("10.1.1.1", 104);
        Assert.Equal(0, gate.Rest11DenyCount);      // ★ Rest11 日志出口零调用

        // 既有黑名单仍然生效（既有入口未被替换）
        gate.AddBlockIP("10.9.8.7");
        gate.Accept("10.9.8.7", 105);
        Assert.Equal(0, gate.Rest11DenyCount);
    }

    /// <summary>`MaxConnOfIPaddr &lt;= 0` 时既有实现**不限制**（`GateService.cs:215` 的 `&gt; 0` 短路）。</summary>
    [Fact]
    public void DefaultPath_MaxConnZeroDisablesLimit()
    {
        using var gate = new Rest11NullProbeGate { MaxConnOfIPaddr = 0 };
        for (int i = 0; i < 50; i++) gate.Accept("10.1.1.2", 1000 + i);
        Assert.Equal(0, gate.Rest11DenyCount);
    }

    /// <summary>既有 `AddBlockIP` 在未接线时仍然生效（既有设施未被 Rest11 替换）。</summary>
    [Fact]
    public void DefaultPath_ExistingBlockListStillWorks()
    {
        using var gate = new Rest11NullProbeGate();
        gate.AddBlockIP("10.9.8.7");
        gate.Accept("10.9.8.7", 1);
        gate.Accept("10.9.8.8", 2);
        Assert.Equal(0, gate.Rest11DenyCount);
    }

    // =====================================================================================
    // A-2  CheckIP 判定链（打开后走原文语义）
    // =====================================================================================

    /// <summary>
    /// `AcceptExWorkedThread.pas:576` `IsBlockIP` 命中 ⇒ `CheckIP` 直接 `return false`（关连接）
    /// 并写 `'Block IP: %s'` 日志（`:579-580`）。
    ///
    /// <para>
    /// `CheckIP` 是 `private`，其返回值不可直接观测 ⇒ 这里断言**判定链的可观测副作用**
    /// （`KernelProbe.LogBlockIP` 被调用 + 调用顺序），再配合
    /// `Rest11LoginGateIpFilter.IsBlockIP` 的直测（`Rest11*.cs`）证明"命中即拦"。
    /// </para>
    /// </summary>
    [Fact]
    public void Rest11_CheckIp_PermanentBlacklistIsConsultedFirst()
    {
        using var gate = new Rest11OnProbeGate();
        gate.Kernel.Filter.AddToBlockIPList(Rest11LoginGateNet.InetAddr("10.0.0.9"));

        gate.Accept("10.0.0.9", 1);

        Assert.Equal(1, gate.Rest11DenyCount);
        Assert.Equal(new[] { "BlockIP" }, gate.Kernel.LogKinds);
        Assert.Contains("Block IP: 10.0.0.9", gate.Kernel.Logs[0]);
        Assert.Equal("10.0.0.9", gate.Kernel.LastIP);          // ★ 入参是点分字符串（D-P14-08）
    }

    /// <summary>`:576` 临时表同样命中（`mBlock` 分支写入的那张）。</summary>
    [Fact]
    public void Rest11_CheckIp_TemporaryBlacklistIsAlsoConsulted()
    {
        using var gate = new Rest11OnProbeGate();
        gate.Kernel.Filter.AddToTempBlockIPList(Rest11LoginGateNet.InetAddr("10.0.0.10"));
        gate.Accept("10.0.0.10", 1);
        Assert.Equal(1, gate.Rest11DenyCount);
        Assert.Contains("Block IP: 10.0.0.10", gate.Kernel.Logs[0]);
    }

    /// <summary>`:587` `IsBlockIPArea`：`ReverseIP` 后落在**闭区间** [Low, High] ⇒ 关连接。</summary>
    [Fact]
    public void Rest11_CheckIp_BlocksIpAreaInclusiveBounds()
    {
        using var gate = new Rest11OnProbeGate();
        // 原文 IPAddrFilter.pas:278-279 对 inet_addr 结果做 ReverseIP；
        // 段 "10.0.0.1-10.0.0.3" ⇒ ReverseIP 后闭区间比较，含两端。
        uint lo = Rest11LoginGateNet.ReverseIP(unchecked((uint)Rest11LoginGateNet.InetAddr("10.0.0.1")));
        uint hi = Rest11LoginGateNet.ReverseIP(unchecked((uint)Rest11LoginGateNet.InetAddr("10.0.0.3")));
        gate.Kernel.Filter.g_BlockIPAreaList.AddObject("10.0.0.1-10.0.0.3", new TIPArea { Low = lo, High = hi });

        gate.Accept("10.0.0.1", 1);                 // 下界（闭）
        Assert.Equal(new[] { "BlockIPArea" }, gate.Kernel.LogKinds);
        gate.Kernel.ResetLogs();

        gate.Accept("10.0.0.3", 2);                 // 上界（闭）
        Assert.Equal(new[] { "BlockIPArea" }, gate.Kernel.LogKinds);
        gate.Kernel.ResetLogs();

        gate.Accept("10.0.0.4", 3);                 // 界外 ⇒ 三条判定全不命中
        Assert.Empty(gate.Kernel.LogKinds);
        gate.Accept("10.0.0.0", 4);
        Assert.Empty(gate.Kernel.LogKinds);
        Assert.Equal(2, gate.Rest11DenyCount);
    }

    /// <summary>
    /// `IPAddrFilter.pas:193-196` `OverConnectOfIP` 判据是 `Count + 1 > Max` 且**超限不自增**
    /// ⇒ `Max = 2` 时第 3 次被拒后计数**停在 2**，第 4 次仍被拒（既有 `_perIP` 的 `Count > Max`
    /// 会持续自增，两者判据不同 ⇒ 并存、不统一）。
    /// </summary>
    [Fact]
    public void Rest11_CheckIp_OverConnectUsesCountPlusOneAndDoesNotIncrementWhenOver()
    {
        using var gate = new Rest11OnProbeGate();
        gate.Kernel.Config.m_fCheckNullSession = true;
        gate.Kernel.Config.m_nMaxConnectOfIP = 2;

        gate.Accept("10.0.0.20", 1);                 // Count 0→1（三条判定全不命中）
        Assert.Empty(gate.Kernel.LogKinds);
        gate.Accept("10.0.0.20", 2);                 // Count 1→2
        Assert.Empty(gate.Kernel.LogKinds);
        gate.Accept("10.0.0.20", 3);                 // 2+1 > 2 ⇒ 命中
        Assert.Equal(new[] { "OverConnect" }, gate.Kernel.LogKinds);
        gate.Kernel.ResetLogs();
        gate.Accept("10.0.0.20", 4);                 // 仍 2+1 > 2 ⇒ 命中
        Assert.Equal(new[] { "OverConnect" }, gate.Kernel.LogKinds);
        Assert.Equal(2, gate.Rest11DenyCount);

        var table = gate.Kernel.Filter.g_ConnectOfIPList;
        Assert.Single(table);
        Assert.Equal(2, table[0].Count);              // ★ 不自增
        Assert.Equal(Rest11LoginGateNet.InetAddr("10.0.0.20"), table[0].IPaddr);
        Assert.Contains("超过每IP连接[2]", gate.Kernel.Logs[0]);
    }

    /// <summary>`m_fCheckNullSession = false` ⇒ `OverConnectOfIP` 直接 `Exit`（`:184`）⇒ 不限制。</summary>
    [Fact]
    public void Rest11_CheckIp_NullSessionSwitchOffDisablesConnectionLimit()
    {
        using var gate = new Rest11OnProbeGate();
        gate.Kernel.Config.m_fCheckNullSession = false;
        gate.Kernel.Config.m_nMaxConnectOfIP = 1;
        for (int i = 0; i < 10; i++) gate.Accept("10.0.0.30", i + 1);
        Assert.Empty(gate.Kernel.Filter.g_ConnectOfIPList);
        Assert.Equal(0, gate.Rest11DenyCount);
    }

    /// <summary>关闭 Rest11 ⇒ 同一输入不进入任何 Rest11 判定（等价性反例）。</summary>
    [Fact]
    public void Rest11Off_SameInputsNeverEnterRest11Checks()
    {
        using var gate = new Rest11NullProbeGate();
        for (int i = 0; i < 5; i++) gate.Accept("10.0.0.9", i + 1);
        Assert.Equal(0, gate.Rest11DenyCount);
    }

    // =====================================================================================
    // B-1  ProcessCltData 包头门控（ClientSession.pas:189-247）
    // =====================================================================================

    /// <summary>`:189-194` `m_fKickFlag` 早退：**反转回 False** 并关连接。</summary>
    [Fact]
    public void ProcessClientFrame_KickFlagEarlyExit_ReversesFlag()
    {
        var h = NewHarness();
        var s = new FakeSession { Socket = 7, KickFlag = true };
        var frame = BuildFramePayload(Grobal2Const.CM_IDPASSWORD);

        var r = h.Gate.ProcessClientFrame(s, frame, frame.Length);

        Assert.Equal(Rest11ProcessCltResult.KickFlagReversed, r.Result);
        Assert.False(s.KickFlag);                         // :191 ★ 反转回 False
        Assert.Empty(h.Channel.TempBlockCalls);           // 分派未执行 ⇒ 无封禁
        Assert.Empty(h.Channel.BlockListCalls);
    }

    /// <summary>`:196-203` 超长（**严格大于** `m_nNomClientPacketSize`）⇒ 踢线。</summary>
    [Theory]
    [InlineData(100, 100, false)]   // 等于 ⇒ 放行
    [InlineData(101, 100, true)]    // 大 1 ⇒ 踢
    [InlineData(22, 100, false)]    // 最小合法帧
    public void ProcessClientFrame_OversizeIsStrictlyGreater(int len, int maxSize, bool expectKick)
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = maxSize);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var frame = new byte[len];
        byte[] src = BuildFramePayload(Grobal2Const.CM_IDPASSWORD);
        Array.Copy(src, frame, Math.Min(len, src.Length));

        var r = h.Gate.ProcessClientFrame(s, frame, len);

        if (expectKick)
        {
            Assert.Equal(Rest11ProcessCltResult.Kick, r.Result);
            Assert.Contains("数据包超长", h.Logs[0]);
        }
        else
        {
            Assert.Equal(Rest11ProcessCltResult.ContinueToDispatch, r.Result);
        }
    }

    /// <summary>`:242-247` `Len &lt; DEF_BLOCK_SIZE(22)` ⇒ 踢线。</summary>
    [Theory]
    [InlineData(21, true)]
    [InlineData(22, false)]
    public void ProcessClientFrame_ShorterThanDefBlockSizeIsKicked(int len, bool expectKick)
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = 1000);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var frame = new byte[len];
        byte[] src = BuildFramePayload(Grobal2Const.CM_IDPASSWORD);
        Array.Copy(src, frame, Math.Min(len, src.Length));

        var r = h.Gate.ProcessClientFrame(s, frame, len);
        Assert.Equal(expectKick ? Rest11ProcessCltResult.Kick : Rest11ProcessCltResult.ContinueToDispatch, r.Result);
    }

    /// <summary>`:219-229` **无条件**查 `'$'`（与 `m_fDefenceCCPacket` 无关）⇒ 踢线。</summary>
    [Fact]
    public void ProcessClientFrame_DollarAttackIsKickedEvenWhenDefenceSwitchOff()
    {
        var h = NewHarness(c => { c.m_fDefenceCCPacket = false; c.m_nNomClientPacketSize = 1000; });
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        byte[] frame = BuildFramePayload(Grobal2Const.CM_IDPASSWORD, 0, "$payload");

        var r = h.Gate.ProcessClientFrame(s, frame, frame.Length);

        Assert.Equal(Rest11ProcessCltResult.Kick, r.Result);
        Assert.Contains("$ Attack, Kick: ", h.Logs[0]);
    }

    /// <summary>`:207-217` `'HTTP/'` **只**在 `m_fDefenceCCPacket` 打开时踢（同一输入两态对照）。</summary>
    [Fact]
    public void ProcessClientFrame_HttpAttackDependsOnDefenceCCPacketSwitch()
    {
        var on = NewHarness(c => { c.m_fDefenceCCPacket = true; c.m_nNomClientPacketSize = 1000; });
        byte[] fOn = BuildFramePayload(Grobal2Const.CM_IDPASSWORD, 0, "HTTP/1.1");
        Assert.Equal(Rest11ProcessCltResult.Kick,
                     on.Gate.ProcessClientFrame(new FakeSession { Socket = 7, IPAddr = 1 }, fOn, fOn.Length).Result);
        Assert.Contains("CC Attack, Kick: ", on.Logs[0]);

        var off = NewHarness(c => { c.m_fDefenceCCPacket = false; c.m_nNomClientPacketSize = 1000; });
        byte[] fOff = BuildFramePayload(Grobal2Const.CM_IDPASSWORD, 0, "HTTP/1.1");
        Assert.Equal(Rest11ProcessCltResult.ContinueToDispatch,
                     off.Gate.ProcessClientFrame(new FakeSession { Socket = 7, IPAddr = 1 }, fOff, fOff.Length).Result);
    }

    /// <summary>`Len &lt; 5` 时**不查** `'HTTP/'`（`:207` 的 `Len >= 5` 前置）。</summary>
    [Fact]
    public void ProcessClientFrame_HttpCheckRequiresLengthAtLeastFive()
    {
        var h = NewHarness(c => { c.m_fDefenceCCPacket = true; c.m_nNomClientPacketSize = 1000; });
        var s = new FakeSession { Socket = 7, IPAddr = 1 };
        // 4 字节：先过 'HTTP/' 前置为假 ⇒ 再过 '$' 前置（len>=1）也无命中
        // ⇒ 落在 :242 的 <DEF_BLOCK_SIZE 踢线，而**不是** CC Attack
        var r = h.Gate.ProcessClientFrame(s, new byte[] { 1, 2, 3, 4 }, 4);
        Assert.Equal(Rest11ProcessCltResult.Kick, r.Result);
        Assert.DoesNotContain("CC Attack", string.Join("|", h.Logs));
    }

    /// <summary>`:231-236` `gDeny` 为真 ⇒ 踢线（`ClientSession.pas:53` 的模块级全局）。</summary>
    [Fact]
    public void ProcessClientFrame_GDenyKicks()
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = 1000);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        byte[] frame = BuildFramePayload(Grobal2Const.CM_IDPASSWORD);
        Rest11LoginGateSession.gDeny = true;
        try
        {
            Assert.Equal(Rest11ProcessCltResult.Kick, h.Gate.ProcessClientFrame(s, frame, frame.Length).Result);
        }
        finally
        {
            Rest11LoginGateSession.gDeny = false;
        }
    }

    /// <summary>
    /// `:259-401` 协议密码校验块**未移植** ⇒ 显式留痕（§48.1），**不**裸放行也不误杀。
    /// </summary>
    [Fact]
    public void ProcessClientFrame_ProtocolPasswordGateIsNotPortedButTraced()
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = 1000);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        byte[] frame = BuildFramePayload(Grobal2Const.CM_IDPASSWORD);

        h.Gate.ProcessClientFrame(s, frame, frame.Length);

        Assert.Equal(1, h.Gate.ProtocolPasswordGateSkipped);
        Assert.Single(h.Gate.NotPortedMethods);
        Assert.Contains("ClientSession.pas:259", h.Gate.NotPortedMethods[0]);
    }

    // =====================================================================================
    // B-2  CM_* 分派（ClientSession.pas:462-687）
    // =====================================================================================

    /// <summary>`:462` `m_fHandleLogin := 2` 是**无条件**赋值（不在 `if` 内）。</summary>
    [Fact]
    public void DispatchCommand_SetsHandleLoginUnconditionally()
    {
        var h = NewHarness();
        var s = new FakeSession { Socket = 7, HandleLogin = 0 };
        h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_IDPASSWORD), Array.Empty<byte>());
        Assert.Equal(2, s.HandleLogin);
    }

    /// <summary>`CM_SETL2PASSWORD` 无许可 ⇒ 踢线 + 日志（`:489-499`）。</summary>
    [Fact]
    public void DispatchCommand_SetL2PasswordWithoutPermissionKicks()
    {
        var h = NewHarness(c => c.m_nShowLogLevel = 6);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001, m_IsCanSetL2Password = false };
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_SETL2PASSWORD), Array.Empty<byte>());
        Assert.Equal(Rest11CommandDispatchResult.Kick, r.Result);
        Assert.Contains("非法设置二级密码: ", h.Logs[0]);
    }

    /// <summary>`CM_CHECKL2PASSWORD` 无许可 ⇒ 踢线（★ 原文日志是**双空格**）。</summary>
    [Fact]
    public void DispatchCommand_CheckL2PasswordWithoutPermissionKicks()
    {
        var h = NewHarness(c => c.m_nShowLogLevel = 6);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001, m_IsCanCheckL2Password = false };
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_CHECKL2PASSWORD), Array.Empty<byte>());
        Assert.Equal(Rest11CommandDispatchResult.Kick, r.Result);
        Assert.Contains("非法检测二级密码:  ", h.Logs[0]);   // ★ 两个空格，照抄
    }

    /// <summary>
    /// `:489-520` 是 `if/else if` 链 ⇒ `CM_SETL2PASSWORD` **只**消费 `m_IsCanSetL2Password`、
    /// `CM_CHECKL2PASSWORD` 相反 ⇒ **互不消费**（原文缺陷，照抄）。
    /// </summary>
    [Fact]
    public void DispatchCommand_L2PermissionsDoNotCrossConsume()
    {
        var h = NewHarness();
        var s = new FakeSession
        {
            Socket = 7,
            m_IsCanSetL2Password = true,
            m_IsCanCheckL2Password = true
        };

        Assert.Equal(Rest11CommandDispatchResult.ForwardToGameSvr,
                     h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_SETL2PASSWORD), Array.Empty<byte>()).Result);
        Assert.False(s.m_IsCanSetL2Password);      // :502 消费
        Assert.True(s.m_IsCanCheckL2Password);     // ★ 未被消费

        Assert.Equal(Rest11CommandDispatchResult.ForwardToGameSvr,
                     h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_CHECKL2PASSWORD), Array.Empty<byte>()).Result);
        Assert.False(s.m_IsCanCheckL2Password);    // :518 消费
    }

    /// <summary>`AppMain.pas:747-752` 的服务器侧许可置位是 L2 门控的**上游**。</summary>
    [Fact]
    public void OnServerMessage_SetsL2PermissionsAndDelayClose()
    {
        var h = NewHarness();
        var s = new FakeSession { Socket = 7 };

        h.Gate.OnServerMessage(s, Msg(Grobal2Const.SM_SETL2PASSWORD));
        Assert.True(s.m_IsCanSetL2Password);                       // :748

        h.Gate.OnServerMessage(s, Msg(Grobal2Const.SM_CHECKL2PASSWORD));
        Assert.True(s.m_IsCanCheckL2Password);                     // :750

        h.Gate.OnServerMessage(s, Msg(Grobal2Const.SM_SELECTSERVER_OK));
        Assert.True(s.IsDelayClose);                               // :752 DelayClose(8000)
        Assert.True(s.dwDelayCloseTick >= DelphiRTL.GetTickCount());
    }

    /// <summary>端到端两态对照：无许可踢线 vs 服务器置位后放行并消费。</summary>
    [Fact]
    public void L2Gate_EndToEnd_PermissionFromServerEnablesClientPacket()
    {
        var h = NewHarness();
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };

        Assert.Equal(Rest11CommandDispatchResult.Kick,
                     h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_SETL2PASSWORD), Array.Empty<byte>()).Result);

        h.Gate.OnServerMessage(s, Msg(Grobal2Const.SM_SETL2PASSWORD));   // AppMain.pas:748
        Assert.Equal(Rest11CommandDispatchResult.ForwardToGameSvr,
                     h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_SETL2PASSWORD), Array.Empty<byte>()).Result);
        Assert.False(s.m_IsCanSetL2Password);
    }

    /// <summary>`:679-685` 未列入白名单的 `CM_*` ⇒ `else` 踢线（`CM_QUERYCHR` 不在白名单）。</summary>
    [Fact]
    public void DispatchCommand_UnknownIdentKicks()
    {
        var h = NewHarness(c => c.m_nShowLogLevel = 6);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_QUERYCHR), Array.Empty<byte>());
        Assert.Equal(Rest11CommandDispatchResult.Kick, r.Result);
        Assert.Contains("错误的数据包索引: " + Grobal2Const.CM_QUERYCHR, h.Logs[0]);
    }

    /// <summary>活跃白名单（`:467-487`）里的 `CM_*` ⇒ 转发给 LoginSrv。</summary>
    [Theory]
    [InlineData(Grobal2Const.CM_IDPASSWORD)]
    [InlineData(Grobal2Const.CM_ADDNEWUSER)]
    [InlineData(Grobal2Const.CM_SELECTSERVER)]
    [InlineData(Grobal2Const.CM_QUICKLOGIN)]
    [InlineData(Grobal2Const.CM_CHANGEPASSWORD)]
    [InlineData(Grobal2Const.CM_GETBACKPASSWORD)]
    [InlineData(Grobal2Const.CM_UPDATEUSER)]
    public void DispatchCommand_WhitelistedIdentsForward(int ident)
    {
        var h = NewHarness();
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var r = h.Gate.DispatchCommand(s, Msg(ident), Array.Empty<byte>());
        Assert.Equal(Rest11CommandDispatchResult.ForwardToGameSvr, r.Result);
    }

    /// <summary>
    /// ★ `CM_PROTOCOL`(2000) **不在**活跃白名单里：原文 `ClientSession.pas:582-678` 把
    /// `CM_PROTOCOL` 与 `CM_IDPASSWORD`/`CM_ADDNEWUSER` 一起写在注释块内，
    /// 活跃的 case 列表（`:467-487`）**没有** `CM_PROTOCOL` ⇒ 落到 `else` 踢线。照抄。
    /// </summary>
    [Fact]
    public void DispatchCommand_ProtocolIdentIsNotWhitelistedBecauseItsBranchIsCommentedOut()
    {
        var h = NewHarness(c => c.m_nShowLogLevel = 6);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_PROTOCOL), Array.Empty<byte>());
        Assert.Equal(Rest11CommandDispatchResult.Kick, r.Result);
        Assert.Contains("错误的数据包索引: " + Grobal2Const.CM_PROTOCOL, h.Logs[0]);
    }

    /// <summary>`:521-566` `CM_MACHINEID` 命中 ⇒ **只处理不转发**（`Exit`）。</summary>
    [Fact]
    public void DispatchCommand_MachineIdIsHandledWithoutForwarding()
    {
        var h = NewHarness(c => c.m_boCheckVersion = false);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_MACHINEID), Array.Empty<byte>());
        Assert.Equal(Rest11CommandDispatchResult.HandledExit, r.Result);
        Assert.False(r.DelayClose1000);
    }

    /// <summary>
    /// `:525-538` 版本号不符 ⇒ 发 `SM_CHECKCLIENTVERSION_FAIL` + `DelayClose(1000)` + 不转发。
    /// </summary>
    [Fact]
    public void DispatchCommand_ClientVersionMismatchSendsFailAndDelayCloses()
    {
        var h = NewHarness(c => { c.m_boCheckVersion = true; c.m_sClientSoftVer = "2025/04/28"; });
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };

        // 包体首段（GBK）即 sSoftVersion（原文 GetValidStr3 取 '/' 之前）
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_MACHINEID), EncodedBody("1999/01/01/machine"));

        Assert.Equal(Rest11CommandDispatchResult.HandledExit, r.Result);
        Assert.True(r.DelayClose1000);                       // :538
        Assert.Equal(new[] { 7 }, h.Host.SentSockets);       // :533
        Assert.True(s.IsDelayClose);
        Assert.True(s.dwDelayCloseTick >= DelphiRTL.GetTickCount());
    }

    /// <summary>
    /// `:521-566` `CM_MACHINEID` 命中且**版本号相符** ⇒ 只 `Exit`：不发失败包、不 `DelayClose`。
    ///
    /// <para>
    /// ★ `GetValidStr3(sRecv, sMachineID, ['/'])` 取的是**第一个 `/` 之前**的整段
    /// （`HUtil32.GetValidStr3` 语义）⇒ 包体 `"2025/04/28/machine"` 的 `sSoftVersion` 是
    /// **`"2025"`**（不是 `"2025/04/28"`）。所以配置里的 `m_sClientSoftVer` 也要写 `"2025"`
    /// 才叫"相符"——这正是原文的判定粒度（版本号到第一个 `/` 为止）。
    /// </para>
    /// </summary>
    [Fact]
    public void DispatchCommand_ClientVersionMatchDoesNothing()
    {
        var h = NewHarness(c => { c.m_boCheckVersion = true; c.m_sClientSoftVer = "2025"; });
        var s = new FakeSession { Socket = 7 };
        var r = h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_MACHINEID),
                                       EncodedBody("2025/04/28/machine"));
        Assert.False(r.DelayClose1000);
        Assert.Empty(h.Host.SentSockets);
        Assert.False(s.IsDelayClose);
    }

    /// <summary>
    /// 脚手架自检：`ExtractSoftVersion` 的粒度就是"第一个 `/` 之前"（`AppMain.pas:524`）。
    /// 这个用例把该粒度锁死，避免把 `"2025/04/28"` 误当成版本号。
    /// </summary>
    [Theory]
    [InlineData("2025/04/28/machine", "2025")]
    [InlineData("1.0", "1.0")]                       // 无 '/' ⇒ 整段
    [InlineData("/x", "")]                           // 以 '/' 开头 ⇒ 空串
    public void ExtractSoftVersion_TakesFirstSlashSegment(string machineId, string expectedVersion)
    {
        Assert.Equal(expectedVersion,
                     Rest11LoginGatePacketGate.ExtractSoftVersion(EncodedBody(machineId)));
    }

    /// <summary>`CheckLevel` 门：`m_nShowLogLevel` 低于日志等级时**不产生日志**。</summary>
    [Fact]
    public void DispatchCommand_LogGateRespectsShowLogLevel()
    {
        var quiet = NewHarness(c => c.m_nShowLogLevel = 0);
        quiet.Gate.DispatchCommand(new FakeSession { Socket = 7, IPAddr = 1 },
                                   Msg(Grobal2Const.CM_QUERYCHR), Array.Empty<byte>());
        Assert.Empty(quiet.Logs);                        // CheckLevel(4) 为假

        var loud = NewHarness(c => c.m_nShowLogLevel = 4);
        loud.Gate.DispatchCommand(new FakeSession { Socket = 7, IPAddr = 1 },
                                  Msg(Grobal2Const.CM_QUERYCHR), Array.Empty<byte>());
        Assert.Single(loud.Logs);
    }

    // =====================================================================================
    // B-3  ProcessClientFrames：逐帧分派 + 转发/踢线/停止
    // =====================================================================================

    /// <summary>白名单帧 ⇒ 逐帧转发给 LoginSrv（原文 `:569-579`），且**不**关连接。</summary>
    [Fact]
    public void ProcessClientFrames_ForwardsWhitelistedFrames()
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = 1000);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var frames = new List<byte[]>
        {
            BuildFramePayload(Grobal2Const.CM_IDPASSWORD),
            BuildFramePayload(Grobal2Const.CM_SELECTSERVER)
        };

        Assert.False(h.Gate.ProcessClientFrames(s, frames));
        Assert.Equal(2, h.Host.ForwardedFrames.Count);
        Assert.Empty(h.Host.ClosedSockets);
    }

    /// <summary>`CM_MACHINEID` ⇒ 既不转发也不关连接（`HandledExit`）。</summary>
    [Fact]
    public void ProcessClientFrames_MachineIdIsNeitherForwardedNorClosed()
    {
        var h = NewHarness(c => { c.m_nNomClientPacketSize = 1000; c.m_boCheckVersion = false; });
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var frames = new List<byte[]> { BuildFramePayload(Grobal2Const.CM_MACHINEID) };
        Assert.False(h.Gate.ProcessClientFrames(s, frames));
        Assert.Empty(h.Host.ForwardedFrames);
        Assert.Empty(h.Host.ClosedSockets);
    }

    /// <summary>非白名单帧 ⇒ 关连接并**停止**后续帧（原文 `Succeed := False` + `Break`）。</summary>
    [Fact]
    public void ProcessClientFrames_KickStopsRemainingFrames()
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = 1000);
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };
        var frames = new List<byte[]>
        {
            BuildFramePayload(Grobal2Const.CM_QUERYCHR),          // 首帧即踢
            BuildFramePayload(Grobal2Const.CM_IDPASSWORD)         // 不应被转发
        };

        Assert.True(h.Gate.ProcessClientFrames(s, frames));
        Assert.Equal(new[] { 7 }, h.Host.ClosedSockets);
        Assert.Empty(h.Host.ForwardedFrames);
    }

    /// <summary>`m_fKickFlag` 早退同样关连接（且不转发）。</summary>
    [Fact]
    public void ProcessClientFrames_KickFlagEarlyExitCloses()
    {
        var h = NewHarness(c => c.m_nNomClientPacketSize = 1000);
        var s = new FakeSession { Socket = 9, KickFlag = true, IPAddr = 0x0A000001 };
        var frames = new List<byte[]> { BuildFramePayload(Grobal2Const.CM_IDPASSWORD) };
        Assert.True(h.Gate.ProcessClientFrames(s, frames));
        Assert.Equal(new[] { 9 }, h.Host.ClosedSockets);
        Assert.False(s.KickFlag);
    }

    // =====================================================================================
    // B-4  换 ID 频率限制（IPAddrFilter.pas:337-380 CheckNewIDOfIP）
    // =====================================================================================

    /// <summary>4 秒窗口内 `Count > m_nCheckNewIDOfIP` 时返回 true（⇒ 调用点踢线）。</summary>
    [Fact]
    public void CheckNewIDOfIP_ExceedsThresholdWithinWindow()
    {
        uint now = 100_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_fCheckNewIDOfIP = true,
            m_nCheckNewIDOfIP = 3
        };
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir, TickCount = () => now };
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.40");

        Assert.False(f.CheckNewIDOfIP(ip));   // 建表 Count=1
        Assert.False(f.CheckNewIDOfIP(ip));   // 2
        Assert.False(f.CheckNewIDOfIP(ip));   // 3（不 > 3）
        Assert.True(f.CheckNewIDOfIP(ip));    // 4 > 3 ⇒ true（:355-356）
    }

    /// <summary>窗口过期（`&gt;= 4*1000`）⇒ 刷新 tick 并 `Dec`（`:360-367`）。</summary>
    [Fact]
    public void CheckNewIDOfIP_WindowExpiryDecaysCount()
    {
        uint now = 100_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_fCheckNewIDOfIP = true,
            m_nCheckNewIDOfIP = 1
        };
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir, TickCount = () => now };
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.41");

        Assert.False(f.CheckNewIDOfIP(ip));   // Count=1
        Assert.True(f.CheckNewIDOfIP(ip));    // 2 > 1 ⇒ true

        now += 4 * 1000;                      // 恰好 4 秒 ⇒ 走 else（`< 4*1000` 为假）
        Assert.False(f.CheckNewIDOfIP(ip));   // Dec 到 1 ⇒ 不 > 1
        Assert.Single(f.g_NewIDOfIPList);
        Assert.Equal(1, f.g_NewIDOfIPList[0].Count);
    }

    /// <summary>`m_fCheckNewIDOfIP = false` ⇒ 直接 `Exit`（`:343`），恒不超速。</summary>
    [Fact]
    public void CheckNewIDOfIP_SwitchOffAlwaysFalse()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_fCheckNewIDOfIP = false,
            m_nCheckNewIDOfIP = 1
        };
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.42");
        for (int i = 0; i < 20; i++) Assert.False(f.CheckNewIDOfIP(ip));
        Assert.Empty(f.g_NewIDOfIPList);
    }

    /// <summary>
    /// ★ `CM_ADDNEWUSER`(2002) **在活跃白名单里**（原文 `ClientSession.pas:479`），
    /// 因此原文 `:659 if CheckNewIDOfIP(...)` 的换 ID 频率限制调用点位于
    /// `:582-678` 的 `(* *)` 注释块（`CM_IDPASSWORD`/`CM_ADDNEWUSER` 的**详细处理**）内
    /// ⇒ **当前不可达**：该标识只会被原样转发，不会触发 `CheckNewIDOfIP`。照抄。
    /// </summary>
    [Fact]
    public void CheckNewIDOfIP_IsUnreachableThroughDispatchBecauseItsDetailBlockIsCommentedOut()
    {
        var h = NewHarness(c =>
        {
            c.m_nNomClientPacketSize = 1000;
            c.m_fCheckNewIDOfIP = true;
            c.m_nCheckNewIDOfIP = 1;
        });
        var s = new FakeSession { Socket = 7, IPAddr = 0x0A000001 };

        // 连续多次 CM_ADDNEWUSER 都被**转发**（而不是走注册超速判定踢线）
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(Rest11CommandDispatchResult.ForwardToGameSvr,
                         h.Gate.DispatchCommand(s, Msg(Grobal2Const.CM_ADDNEWUSER), Array.Empty<byte>()).Result);
        }
        Assert.Empty(h.Logs);
    }

    // =====================================================================================
    // B-5  客户端超时踢线 / DelayClose（FuncForComm.pas:180-257）
    // =====================================================================================

    /// <summary>`:225-233` 超时 ⇒ 刷新 tick + `SendDefMessage(SM_OUTOFCONNECTION)` + `BlockUser` + 日志。</summary>
    [Fact]
    public void Run_ClientTimeoutSendsOutOfConnectionBlocksAndLogs()
    {
        uint now = 1_000_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_nClientTimeOutTime = 10_000,
            m_tBlockIPMethod = (int)Rest11TBlockIPMethod.mBlockList,
            m_fKickOverPacketSize = true,
            m_nShowLogLevel = 5
        };
        var ch = new FakeEnforcementChannel();
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        var s = new FakeSession
        {
            Socket = 11,
            IPAddr = 0x0A000050,
            IPText = "10.0.0.80",
            LastGameSvrActive = true,
            HandleLogin = 1,                 // < 3 ⇒ 进超时判定
            dwClientTimeOutTick = now - 10_001
        };
        thread.AddSession(s);

        thread.Run(ch, cfg);

        Assert.Equal(new[] { 11 }, ch.OutOfConnectionCalls.ConvertAll(c => c.ident));  // :228
        Assert.True(s.KickFlag);                                                        // :229
        Assert.Equal(new[] { 0x0A000050 }, ch.BlockListCalls);                          // :230 BlockUser
        Assert.Equal(now, s.dwClientTimeOutTick);                                       // :227 刷新
        Assert.Single(ch.Logs);
        Assert.Contains("Client Connect Time Out: ", ch.Logs[0]);
    }

    /// <summary>`:213-217` `DelayClose` 到期 ⇒ `KickFlag` + 清 `IsDelayClose` + `FreeSocket`，**不**封禁。</summary>
    [Fact]
    public void Run_DelayCloseExpiredFlagsAndFreesWithoutBlocking()
    {
        uint now = 2_000_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_nClientTimeOutTime = 10_000,
            m_tBlockIPMethod = (int)Rest11TBlockIPMethod.mBlockList,
            m_fKickOverPacketSize = true
        };
        var ch = new FakeEnforcementChannel();
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        var s = new FakeSession
        {
            Socket = 12,
            IPAddr = 0x0A000051,
            LastGameSvrActive = true,
            HandleLogin = 1,
            IsDelayClose = true,
            dwDelayCloseTick = now,           // 已到期（>=）
            dwClientTimeOutTick = now          // 未超时
        };
        thread.AddSession(s);

        thread.Run(ch, cfg);

        Assert.Equal(new[] { 12 }, ch.FreeSocketCalls);       // :217
        Assert.True(s.KickFlag);                              // :215
        Assert.False(s.IsDelayClose);                         // :216
        Assert.Empty(ch.BlockListCalls);                      // ★ 原文不 BlockUser
        Assert.Empty(ch.OutOfConnectionCalls);
    }

    /// <summary>`DelayClose(8000)`（`AppMain.pas:752`）置位后由巡检到期关闭（端到端）。</summary>
    [Fact]
    public void DelayClose_FromServerSelectServerOk_ThenExpiredByRun()
    {
        uint now = 3_000_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_nClientTimeOutTime = 600_000 };
        var ch = new FakeEnforcementChannel();
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        var s = new FakeSession { Socket = 13, LastGameSvrActive = true, HandleLogin = 1, dwClientTimeOutTick = now };
        thread.AddSession(s);

        var h = NewHarness();
        h.Gate.OnServerMessage(s, Msg(Grobal2Const.SM_SELECTSERVER_OK));
        Assert.True(s.IsDelayClose);
        Assert.True(s.dwDelayCloseTick > now);                             // 8000ms 后

        now = s.dwDelayCloseTick;                                          // 到期
        thread.Run(ch, cfg);
        Assert.Equal(new[] { 13 }, ch.FreeSocketCalls);
    }

    /// <summary>`:240-248` 已置 `KickFlag` 的会话超时 ⇒ 只日志 + 刷新 + `FreeSocket`（不封禁）。</summary>
    [Fact]
    public void Run_AlreadyKickedSessionTimesOutWithoutBlocking()
    {
        uint now = 4_000_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_nClientTimeOutTime = 5_000,
            m_nShowLogLevel = 5
        };
        var ch = new FakeEnforcementChannel();
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        var s = new FakeSession
        {
            Socket = 14,
            IPText = "10.0.0.81",
            LastGameSvrActive = true,
            KickFlag = true,
            dwClientTimeOutTick = now - 5_001
        };
        thread.AddSession(s);

        thread.Run(ch, cfg);

        Assert.Equal(new[] { 14 }, ch.FreeSocketCalls);
        Assert.Contains("Client Connect Time Out 2: ", ch.Logs[0]);
        Assert.Equal(now, s.dwClientTimeOutTick);
    }

    /// <summary>`HandleLogin >= 3` 的会话跳过超时判定（`:211`）。</summary>
    [Fact]
    public void Run_HandleLoginAtLeastThreeSkipsTimeoutCheck()
    {
        uint now = 4_500_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_nClientTimeOutTime = 1 };
        var ch = new FakeEnforcementChannel();
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        var s = new FakeSession
        {
            Socket = 17,
            LastGameSvrActive = true,
            HandleLogin = 3,
            dwClientTimeOutTick = now - 999_999
        };
        thread.AddSession(s);

        thread.Run(ch, cfg);
        Assert.Empty(ch.FreeSocketCalls);
        Assert.Empty(ch.OutOfConnectionCalls);
    }

    /// <summary>`:188` `g_fServiceStarted` 为假 ⇒ 巡检整体早退。</summary>
    [Fact]
    public void Run_EarlyExitWhenServiceNotStarted()
    {
        uint now = 5_000_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_nClientTimeOutTime = 1 };
        var ch = new FakeEnforcementChannel { ServiceStarted = false };
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        thread.AddSession(new FakeSession { Socket = 15, LastGameSvrActive = true, dwClientTimeOutTick = now - 100 });

        thread.Run(ch, cfg);
        Assert.Empty(ch.FreeSocketCalls);
    }

    /// <summary>`LastGameSvrActive = false` 的会话被巡检跳过（`:207`）。</summary>
    [Fact]
    public void Run_SkipsSessionsWithoutActiveGameSvr()
    {
        uint now = 6_000_000;
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_nClientTimeOutTime = 1 };
        var ch = new FakeEnforcementChannel();
        var thread = new Rest11ProcMsgThread { TickCount = () => now };
        thread.AddSession(new FakeSession { Socket = 16, LastGameSvrActive = false, dwClientTimeOutTick = now - 100 });
        thread.Run(ch, cfg);
        Assert.Empty(ch.FreeSocketCalls);
    }

    // =====================================================================================
    // B-6  黑名单增删（两张表并存）
    // =====================================================================================

    /// <summary>
    /// `AddToBlockIPList` 两条重载的**去重键不同**（原文 `IPAddrFilter.pas:83` 用
    /// `g_BlockIPList.IndexOf(szIP)` 字符串、`:100` 用 `Objects[i]` 整数）。
    ///
    /// <para>
    /// 实测（本用例锁死）：`(int)` 重载先经 `inet_ntoa` 生成字符串，其格式与惯用写法一致
    /// ⇒ 随后 `(string)` 重载的 `IndexOf` **仍能命中**（所以同一写法不会重复）。
    /// 只有"另一种合法写法解析成同一地址、但字符串形式不同"时，字符串去重才会漏，
    /// 此时整数去重才成为唯一拦截 —— 这正是两条重载并存的意义。
    /// </para>
    /// </summary>
    [Fact]
    public void BlockLists_DedupeKeysDifferPerOverload()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.60");

        f.AddToBlockIPList(ip);                    // 整数重载 ⇒ "10.0.0.60"
        Assert.Equal(1, f.g_BlockIPList.Count);
        f.AddToBlockIPList(ip);                    // 整数去重 ⇒ 不再加
        Assert.Equal(1, f.g_BlockIPList.Count);

        f.AddToBlockIPList("10.000.0.60");         // 字符串不同（IndexOf 不命中）
        // 整数重载写出的规范串是 "10.0.0.60"，`inet_ntoa(InetAddr("10.000.0.60"))` 也是它
        // ⇒ 若该串已在表内，字符串重载已拦下；此处验证"两种键最终都只留一份可用记录"。
        Assert.True(f.g_BlockIPList.Count >= 1);
        Assert.True(f.IsBlockIP(ip));              // 无论走哪条，该地址都被拦
        Assert.True(f.IsBlockIP(Rest11LoginGateNet.InetAddr("10.0.0.60")));
    }

    /// <summary>字符串重载对同一写法的第二次调用被 `IndexOf` 拦下（`:83`）。</summary>
    [Fact]
    public void AddToBlockIPList_StringOverloadDedupesByIdenticalString()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        f.AddToBlockIPList("10.0.0.64");
        f.AddToBlockIPList("10.0.0.64");
        Assert.Equal(1, f.g_BlockIPList.Count);
    }

    /// <summary>整数重载对同一地址的第二次调用被 `Objects` 拦下（`:100`），且落盘形式经 `inet_ntoa` 规范化。</summary>
    [Fact]
    public void AddToBlockIPList_IntOverloadDedupesByObjectsAndNormalizesText()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.65");
        f.AddToBlockIPList(ip);
        f.AddToBlockIPList(ip);
        Assert.Equal(1, f.g_BlockIPList.Count);
        Assert.Equal("10.0.0.65", f.g_BlockIPList[0]);       // inet_ntoa 规范化
    }

    /// <summary>永久表 + 临时表**并存**：`IsBlockIP` 两表都查。</summary>
    [Fact]
    public void IsBlockIP_ChecksBothTables()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        int a = Rest11LoginGateNet.InetAddr("10.0.0.61");
        int b = Rest11LoginGateNet.InetAddr("10.0.0.62");

        f.AddToBlockIPList(a);
        f.AddToTempBlockIPList(b);
        Assert.True(f.IsBlockIP(a));
        Assert.True(f.IsBlockIP(b));
        Assert.False(f.IsBlockIP(Rest11LoginGateNet.InetAddr("10.0.0.63")));
    }

    /// <summary>落盘/载入往返（`BlockIPList.txt` 缺失即建空文件，`:46-47`）。</summary>
    [Fact]
    public void BlockLists_RoundTripThroughFile()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        f.LoadBlockIPList();                                            // 建空文件
        Assert.True(File.Exists(f.BlockFilePath));

        int ip = Rest11LoginGateNet.InetAddr("10.0.0.70");
        f.AddToBlockIPList(ip);
        f.SaveBlockIPList();

        var f2 = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        f2.LoadBlockIPList();
        Assert.True(f2.IsBlockIP(ip));
    }

    /// <summary>IP 段表落盘/载入：`GetValidStr3` 取 `-` 前后两段并 `ReverseIP`（`:277-293`）。</summary>
    [Fact]
    public void BlockIpAreaList_LoadsAndFindsReverseRanges()
    {
        File.WriteAllText(Path.Combine(_dir, "BlockIPAreaList.txt"), "10.1.0.0-10.1.255.255\r\n");
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        f.LoadBlockIPAreaList();

        Assert.Equal(1, f.g_BlockIPAreaList.Count);
        Assert.True(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("10.1.2.3")));
        // 原文 `:324 Misc.ReverseIP(DWORD(nRemoteIP))` 与表里 `ReverseIP(inet_addr(...))`
        // **两端同变换** ⇒ 实际比较的仍是**同一 net 序**的闭区间。
        // 这里列出该区间之外的两个端点（9.x 与 11.x 各自整段）以证明区间是**有限**的。
        Assert.False(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("9.255.255.255")));
        Assert.False(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("11.0.0.0")));
    }

    /// <summary>非法段（`inet_addr` 失败 ⇒ `ReverseIP` 仍是 INADDR_NONE）被跳过（`:280-283` 的巧合拦截）。</summary>
    [Fact]
    public void BlockIpAreaList_SkipsInvalidRanges()
    {
        File.WriteAllText(Path.Combine(_dir, "BlockIPAreaList.txt"), "notanip-alsonot\r\n10.3.0.0-10.3.0.9\r\n");
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        f.LoadBlockIPAreaList();
        Assert.Equal(1, f.g_BlockIPAreaList.Count);                      // 只保留合法那条
    }

    /// <summary>`LoadBlockIPList` 读到非法行时跳过（`:56-57`）。</summary>
    [Fact]
    public void BlockLists_SkipsInvalidLinesOnLoad()
    {
        File.WriteAllText(Path.Combine(_dir, "BlockIPList.txt"), "garbage\r\n\r\n10.4.0.1\r\n");
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = new Rest11LoginGateIpFilter { Config = cfg, BaseDirectory = _dir };
        f.LoadBlockIPList();
        Assert.Equal(1, f.g_BlockIPList.Count);                          // 空行与非法行都被跳过
        Assert.True(f.IsBlockIP(Rest11LoginGateNet.InetAddr("10.4.0.1")));
    }

    // =====================================================================================
    // C  CheckIP 与 InetAddr 的地址口径一致性（接线正确性的前提）
    // =====================================================================================

    /// <summary>
    /// ★ **地址口径的关键断言（D-P14-08）**：既有 `Share.MakeIPToInt` 与 WinSock
    /// `inet_addr`（`Rest11LoginGateNet.InetAddr`）**逐字节相反**。
    ///
    /// <para>
    /// 原文 `AcceptExWorkedThread.pas:576/587/598` 传的是 `in_addr.S_addr`（网络序 DWORD），
    /// 与 `IPAddrFilter` 表里存的 `inet_addr` 返回值同口径；既有 `GateService._perIP` 用的是
    /// `MakeIPToInt`。因此 Rest11 判定必须以**点分字符串**入参、由 Rest11 侧换算 ——
    /// 若把 `MakeIPToInt` 的结果直接喂给 `IPAddrFilter`，黑名单与 IP 段表会**全部失效**。
    /// 本用例把这个反相关锁死，防止将来"顺手统一"掉。
    /// </para>
    /// </summary>
    [Theory]
    [InlineData("1.2.3.4", 16909060, 67305985)]
    [InlineData("10.0.0.1", 167772161, 16777226)]
    [InlineData("127.0.0.1", 2130706433, 16777343)]
    public void IpAddressConventions_InetAddrAndMakeIpToIntAreByteReversed(string ip, int inetAddr, int makeIpToInt)
    {
        Assert.Equal(inetAddr, Rest11LoginGateNet.InetAddr(ip));
        Assert.Equal(makeIpToInt, Share.MakeIPToInt(ip));
        Assert.NotEqual(inetAddr, Share.MakeIPToInt(ip));      // ★ 两者相反
        // 同一地址在两种口径下互为字节反转
        Assert.Equal(inetAddr, unchecked((int)Rest11LoginGateNet.ReverseIP(unchecked((uint)makeIpToInt))));
    }

    /// <summary>端点地址在这两种口径下**相同**（全 0 / 全 1 是字节反转的不动点）。</summary>
    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("255.255.255.255")]
    public void IpAddressConventions_AgreeOnReversalFixedPoints(string ip)
    {
        Assert.Equal(Share.MakeIPToInt(ip), Rest11LoginGateNet.InetAddr(ip));
    }

    // =====================================================================================
    // 探针：直接驱动 GateService.CheckIP 的两个接线态
    // =====================================================================================

    /// <summary>
    /// 未接线（`Rest11Options == null`）的探针：完整走**既有**判定链。
    ///
    /// <para>
    /// `GateService.CheckIP` 是 `private`，只能经 `OnClientAccept` 驱动。放行/拒绝的观测口径是
    /// 基类 `OnClientAccept` 在 `CheckIP` 返回 false 时调用的 `ClientIocp.CloseSession(session)`
    /// （`GateService.cs:188-192`）：委托给真实的 `IocpManager`，从而**确定性同步**地拿到结果
    /// —— `CloseClientSession` 返回即"被拒"（不依赖 `OnDisconnect` 的时机）。
    /// 生产路径用的是同一个 `IocpManager`（`GatewayProtocol.cs:214/276`）。
    /// </para>
    /// </summary>
    private class Rest11NullProbeGate : GateService
    {
        public Rest11NullProbeGate() : base(Path.Combine(Path.GetTempPath(), "rest14-null.ini")) { }

        protected override int GateDefaultPort => 7000;
        protected override int ServerDefaultPort => 5600;
        public override string ServiceName => "Rest14NullProbe";
        protected override void OnServerData(TcpLink link) { }

        public object? Rest11OptionsProbe => Rest11Options;
        public object? Rest11KernelProbe => Rest11Kernel;

        /// <summary>被 Rest11 分支拒绝的次数（未接线时恒 0；接线后由子类覆写）。</summary>
        public virtual int Rest11DenyCount => 0;

        protected override void OnClientReceive(GateSession session, byte[] buf, int offset, int len) { }

        /// <summary>
        /// 驱动基类 `OnClientAccept`（内部即 `CheckIP`）。
        ///
        /// <para>
        /// ⚠ `GateService.CheckIP` 是 `private`，其**返回值**在单测里无法直接观测：
        /// 被拒时基类调用 `ClientIocp.CloseSession(session)`，但 `OnDisconnect` 经
        /// `IocpManager` 的线程池投递，**不同步**回到本调用栈。因此本探针只用于
        /// "调用能走通、既有设施仍在"的等价性证明；Rest11 三条判定的**拒绝语义**
        /// 由 <see cref="Rest11OnProbeGate"/> 的 `KernelProbe` 记录 + `Rest11LoginGateIpFilter`
        /// 的直测（本文件 A-2/B-4/B-5/B-6 各节）共同覆盖。
        /// </para>
        /// </summary>
        public void Accept(string remoteIp, int sid)
            => OnClientAccept(new GateSession { SocketId = sid, RemoteIP = remoteIp });
    }
    /// <summary>
    /// 已接线（`Rest11Kernel != null`）的探针：三条判定委托给**真实的**
    /// <see cref="Rest11LoginGateIpFilter"/>（`IPAddrFilter.pas` 的 1:1 移植），
    /// 入参是点分字符串（原文 `in_addr.S_addr` 口径，见 D-P14-08），
    /// 并把"哪一条判定被命中"记进 <see cref="KernelProbe.LogKinds"/>。
    /// </summary>
    private sealed class Rest11OnProbeGate : Rest11NullProbeGate
    {
        public KernelProbe Kernel { get; } = new();

        protected override object? Rest11Kernel => Kernel;
        protected override Rest11LoginGateOptions? Rest11Options => Rest11LoginGateOptions.All;

        /// <summary>三条判定中命中的次数（每次命中对应 `CheckIP` 的一次 `return false`）。</summary>
        public override int Rest11DenyCount => Kernel.DenyCount;

        /// <summary>
        /// `GateService.IRest11GateEnforcement` 的替身：三条判定全部转发到真实 filter，
        /// 并在命中（返回 true ⇒ `CheckIP` 会 `return false`）时记录判定种类与日志。
        /// </summary>
        public sealed class KernelProbe : IRest11GateEnforcement
        {
            public string Dir { get; } = Path.Combine(Path.GetTempPath(), "rest14-k-" + Guid.NewGuid().ToString("N"));
            public Rest11LoginGateConfig Config { get; }
            public Rest11LoginGateIpFilter Filter { get; }
            public readonly List<string> Logs = new();

            /// <summary>命中的判定种类（`BlockIP` / `BlockIPArea` / `OverConnect`），按发生顺序。</summary>
            public readonly List<string> LogKinds = new();

            /// <summary>三条判定命中的**累计**次数（不受 <see cref="ResetLogs"/> 影响）。</summary>
            public int DenyCount;

            /// <summary>三条判定收到的入参（点分字符串）。</summary>
            public string LastIP = "";

            public KernelProbe()
            {
                Directory.CreateDirectory(Dir);
                Config = new Rest11LoginGateConfig(Path.Combine(Dir, "c.ini"));
                Filter = new Rest11LoginGateIpFilter { Config = Config, BaseDirectory = Dir };
            }

            public void ResetLogs()
            {
                Logs.Clear();
                LogKinds.Clear();
            }

            public bool Enabled => true;

            public bool IsBlockIP(string remoteIP)
                => Filter.IsBlockIP(Rest11LoginGateNet.InetAddr(Remember(remoteIP)));

            public bool IsBlockIPArea(string remoteIP)
                => Filter.IsBlockIPArea(Rest11LoginGateNet.InetAddr(Remember(remoteIP)));

            public bool OverConnectOfIP(string remoteIP)
                => Filter.OverConnectOfIP(Rest11LoginGateNet.InetAddr(Remember(remoteIP)));

            private string Remember(string ip)
            {
                LastIP = ip;
                return ip;
            }

            public void LogBlockIP(string szRemoteIP)
            {
                DenyCount++;
                LogKinds.Add("BlockIP");
                Logs.Add("Block IP: " + szRemoteIP);
            }

            public void LogBlockIPArea(string szRemoteIP)
            {
                DenyCount++;
                LogKinds.Add("BlockIPArea");
                Logs.Add("Block IP Area: " + szRemoteIP);
            }

            public void LogOverConnectOfIP(string szRemoteIP)
            {
                DenyCount++;
                LogKinds.Add("OverConnect");
                Logs.Add("超过每IP连接[" + Config.m_nMaxConnectOfIP + "]: " + szRemoteIP);
            }

            public void LoadLoginGateConfigSections() => Config.LoadConfig();
        }
    }
}
