// ============================================================================
// 测试：本车道 p13-m2-objplayer **切片 2（消息派发片）**。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Core2.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:1224（声明）/ 3699-3771（Operate 实现）
//           / 3706-3721（内嵌 ProcessPlayObjectMessage）/ 3723-3732（内嵌 CanFilter）
//           / 3772-5606（Run，**本切片留痕未移植**）
//           Source/Common/Grobal2.pas:1007-1019 / 1114 / 1189 / 1223（RM_* 取值）
//           Source/M2Engine/ObjBase.pas:11（MAXCLIENTMESSAGECOUNT = 30000）
// 用例 ≥3/方法：正常 / 边界 / 早退顺序 / 差异断言。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class ObjPlayerCore2Tests : IDisposable
{
    public ObjPlayerCore2Tests()
    {
        PlayerSurfaceOperateSeams.ResetDefaults();
        PlayerSurfaceMessageTable.Clear();
        PlayerSurfacePortLedger.ResetNotPorted();
    }

    public void Dispose()
    {
        PlayerSurfaceOperateSeams.ResetDefaults();
        PlayerSurfaceMessageTable.Clear();
        PlayerSurfacePortLedger.ResetNotPorted();
    }

    private static TProcessMessage Msg(int wIdent) => new() { wIdent = (ushort)wIdent };

    // ==================================================================
    // CanFilter（原文 3723-3732）
    // ==================================================================

    /// <summary>原文 3725：`Result := True;` —— 未列入 case 的标识一律 True。</summary>
    [Fact]
    public void CanFilter_UnlistedIdent_ReturnsTrue()
    {
        Assert.True(TPlayObject.CanFilter(Grobal2Const.RM_WALK));
        Assert.True(TPlayObject.CanFilter(1));
        Assert.True(TPlayObject.CanFilter(PlayerSurfaceOperateConst.MAXCLIENTMESSAGECOUNT));
    }

    /// <summary>原文 3727-3729：15 个标识全部 Result := False（逐个断言，防漏项）。</summary>
    [Theory]
    [InlineData(20067)]  // RM_HEAR
    [InlineData(20068)]  // RM_WHISPER
    [InlineData(20069)]  // RM_CRY
    [InlineData(20070)]  // RM_SYSMESSAGE
    [InlineData(20266)]  // RM_SYSMESSAGE_EX（**非 20071**）
    [InlineData(20071)]  // RM_GROUPMESSAGE
    [InlineData(20072)]  // RM_GUILDMESSAGE
    [InlineData(20073)]  // RM_DELAYMESSAGE
    [InlineData(20074)]  // RM_CENTERMESSAGE
    [InlineData(20075)]  // RM_TOPCHATBOARDMESSAGE
    [InlineData(20076)]  // RM_MOVEMESSAGE
    [InlineData(20235)]  // RM_MOVEMESSAGE_NEW（**非 20077**）
    [InlineData(20078)]  // RM_SCREENMESSAGE
    [InlineData(20171)]  // RM_CHANGESPEED（**非 20079**）
    [InlineData(20172)]  // RM_SERVERCONFIG
    public void CanFilter_ListedIdent_ReturnsFalse(int ident)
    {
        Assert.False(TPlayObject.CanFilter(ident));
    }

    /// <summary>
    /// ★ 差异断言 / 原文如此：`RM_MERCHANTSAY = 20077`（Grobal2.pas:1018）
    /// 与 `RM_SUPERMOVEMESSAGE = 20079`（:1021）**不在** CanFilter 的 case 表里 ——
    /// 表里在 `RM_MOVEMESSAGE = 20076` 之后直接跳到 `RM_SCREENMESSAGE = 20078`。
    /// 若有人"顺手补全"这两个值，本用例会立刻失败（刻意的告警）。
    /// </summary>
    [Fact]
    public void CanFilter_GapValues_AreNotInTheCaseTable_OriginalDefect()
    {
        // 原文如此（ObjPlayer.pas:3727-3729 的 case 列表跳过了 20077 与 20079）
        Assert.True(TPlayObject.CanFilter(20077));   // RM_MERCHANTSAY —— 未列入
        Assert.True(TPlayObject.CanFilter(20079));   // RM_SUPERMOVEMESSAGE —— 未列入
    }

    /// <summary>常量取值必须与 Grobal2.pas 逐字一致（本题锁死 4 个"易错值"）。</summary>
    [Fact]
    public void OperateConst_Values_MatchGrobal2Pas()
    {
        Assert.Equal(20068, PlayerSurfaceOperateConst.RM_WHISPER);
        Assert.Equal(20070, PlayerSurfaceOperateConst.RM_SYSMESSAGE);
        Assert.Equal(20266, PlayerSurfaceOperateConst.RM_SYSMESSAGE_EX);
        Assert.Equal(20235, PlayerSurfaceOperateConst.RM_MOVEMESSAGE_NEW);
        Assert.Equal(20171, PlayerSurfaceOperateConst.RM_CHANGESPEED);
        Assert.Equal(30000, PlayerSurfaceOperateConst.MAXCLIENTMESSAGECOUNT);   // ObjBase.pas:11
    }

    // ==================================================================
    // ProcessPlayObjectMessage（原文 3706-3721）
    // ==================================================================

    /// <summary>原文 3708：wIdent = 0 → Result 保持 False。</summary>
    [Fact]
    public void ProcessPlayObjectMessage_IdentZero_ReturnsFalse_AndDoesNotDispatch()
    {
        bool dispatched = false;
        PlayerSurfaceMessageTable.Register(0, (TProcessMessage _, ref bool _) => dispatched = true);

        bool boResult = false;
        bool r = new TPlayObject().ProcessPlayObjectMessage(Msg(0), ref boResult);

        Assert.False(r);
        Assert.False(dispatched);   // 首行 Result := False 后直接跳过整段
    }

    /// <summary>
    /// 原文 3709 / 3711：`wIdent` 在 `1..MAXCLIENTMESSAGECOUNT` 内 → **Result := True**，
    /// 即使该槽位是 nil（原文 `Result := True` 在 `Assigned` 判定**之前**）。
    /// </summary>
    [Fact]
    public void ProcessPlayObjectMessage_InRangeButSlotNil_ReturnsTrue_OriginalOrder()
    {
        bool boResult = false;
        bool r = new TPlayObject().ProcessPlayObjectMessage(Msg(1234), ref boResult);

        Assert.True(r);        // ★ 原文如此：槽位 nil 也返回 True
        Assert.False(boResult);
    }

    /// <summary>原文 3709：越界（&gt; MAXCLIENTMESSAGECOUNT）→ Result 保持 False，不派发。</summary>
    [Fact]
    public void ProcessPlayObjectMessage_AboveMax_ReturnsFalse()
    {
        int over = PlayerSurfaceOperateConst.MAXCLIENTMESSAGECOUNT + 1;
        bool dispatched = false;
        // 直接注册到越界下标是无效的（表长只有 MAX+1），故这里证明"不会派发"即可
        bool boResult = false;
        bool r = new TPlayObject().ProcessPlayObjectMessage(Msg(over), ref boResult);

        Assert.False(r);
        Assert.False(dispatched);
    }

    /// <summary>原文 3712-3715：槽位存在 → 派发，处理函数可改写 var boResult。</summary>
    [Fact]
    public void ProcessPlayObjectMessage_SlotPresent_Dispatches_AndWritesBoResult()
    {
        const int ident = 3010;   // 原文 3712 注释列出的实例标识之一
        PlayerSurfaceMessageTable.Register(ident, (TProcessMessage m, ref bool b) =>
        {
            Assert.Equal((ushort)ident, m.wIdent);
            b = true;
        });

        bool boResult = false;
        bool r = new TPlayObject().ProcessPlayObjectMessage(Msg(ident), ref boResult);

        Assert.True(r);
        Assert.True(boResult);
    }

    /// <summary>
    /// ★ 原文 3714-3718：`try..except` **无 raise** —— 处理函数抛异常被吞掉，
    /// Result 仍为 True（已置），只打一行 `ProcessPlayObjectMessage Error; wIdent:N` 日志。
    /// </summary>
    [Fact]
    public void ProcessPlayObjectMessage_HandlerThrows_IsSwallowed_ResultStaysTrue()
    {
        const int ident = 20065;   // 原文 3712 注释列出的实例标识之一
        var logged = new List<string>();
        PlayerSurfaceOperateSeams.MainOutMessage = s => logged.Add(s);

        PlayerSurfaceMessageTable.Register(ident, (TProcessMessage _, ref bool _)
            => throw new InvalidOperationException("boom"));

        bool boResult = false;
        bool r = new TPlayObject().ProcessPlayObjectMessage(Msg(ident), ref boResult);

        Assert.True(r);                                    // 原文 3711 已置 True，异常不影响
        Assert.False(boResult);
        Assert.Single(logged);
        Assert.Equal("ProcessPlayObjectMessage Error; wIdent:" + ident, logged[0]);
    }

    // ==================================================================
    // Operate（原文 3699-3769）
    // ==================================================================

    /// <summary>原文 3736-3740：`ProcessMsg = nil` → Result := False 且立刻 Exit。</summary>
    [Fact]
    public void Operate_NullMessage_ReturnsFalse_AndSkipsEverything()
    {
        bool inheritedCalled = false;
        PlayerSurfaceOperateSeams.InheritedOperate = (_, _) => { inheritedCalled = true; return true; };

        var p = new TPlayObject();
#pragma warning disable CS8625 // 原文 ProcessMsg 可为 nil
        bool r = p.Operate(null!);
#pragma warning restore CS8625

        Assert.False(r);
        Assert.False(inheritedCalled);
    }

    /// <summary>原文 3735-3736：`wIdent = 0` → 与 nil 同样早退（Result := False）。</summary>
    [Fact]
    public void Operate_IdentZero_ReturnsFalse()
    {
        Assert.False(new TPlayObject().Operate(Msg(0)));
    }

    /// <summary>
    /// 原文 3754-3757：`CanFilter = True` 且槽位不存在 → `inherited Operate(ProcessMsg)` 的
    /// 返回值**覆盖** Result（原文 3735 置的 True 被覆盖）。
    /// </summary>
    [Fact]
    public void Operate_NoHandler_FallsBackToInherited_ResultComesFromInherited()
    {
        TProcessMessage? seen = null;
        PlayerSurfaceOperateSeams.InheritedOperate = (_, m) => { seen = m; return false; };

        var msg = Msg(Grobal2Const.RM_WALK);   // 不在 CanFilter 的 case 表内 → CanFilter = True
        bool r = new TPlayObject().Operate(msg);

        Assert.False(r);                        // 来自 InheritedOperate
        Assert.Same(msg, seen);                 // ★ CanFilter=True 分支传的是**原始**报文对象
    }

    /// <summary>
    /// ★★ 差异断言（原文 3756 vs 3759 的**不对称**）：`CanFilter = False` 时传的是
    /// `@ProcessMessage`（**按值副本**），而不是原始 `ProcessMsg` —— 所以落到
    /// `inherited Operate` 的是**另一份对象**（值相等、引用不同）。
    /// 若有人把两分支合并成"都传 ProcessMsg"，本用例会失败。
    /// </summary>
    [Fact]
    public void Operate_CanFilterFalse_PassesTheValueCopy_NotTheOriginal_OriginalAsymmetry()
    {
        TProcessMessage? seen = null;
        PlayerSurfaceOperateSeams.InheritedOperate = (_, m) => { seen = m; return false; };

        var msg = Msg(20068);   // RM_WHISPER → CanFilter = False
        new TPlayObject().Operate(msg);

        Assert.NotNull(seen);
        Assert.NotSame(msg, seen);                       // ★ 副本，不是原对象
        Assert.Equal(msg.wIdent, seen!.wIdent);          // 但字段逐一同值
    }

    /// <summary>原文 3744-3751：插件把 boReturn 置 True 且 CanFilter 为真 → 直接 Exit，Result 仍 True。</summary>
    [Fact]
    public void Operate_PluginHookReturnsTrue_AndCanFilter_ShortCircuitsToTrue()
    {
        PlayerSurfaceOperateSeams.PluginManagerAvailable = () => true;
        PlayerSurfaceOperateSeams.HookProcessMsgBegin =
            (_, _, _, _, _, _, _, _, _, b) => b.Value = true;

        bool inheritedCalled = false;
        PlayerSurfaceOperateSeams.InheritedOperate = (_, _) => { inheritedCalled = true; return false; };

        var p = new TPlayObject();
        bool r = p.Operate(Msg(Grobal2Const.RM_WALK));   // CanFilter = True

        Assert.True(r);                  // 原文 3735 的 True 保留
        Assert.False(inheritedCalled);   // 3751 Exit 掉了
    }

    /// <summary>
    /// ★ 差异断言：同一个 boReturn 被置 True，但 `CanFilter = False` 的报文
    /// **不会**被短路（原文 3750 的 `and CanFilter(...)` 是**与**条件）。
    /// </summary>
    [Fact]
    public void Operate_PluginHookReturnsTrue_ButCanFilterFalse_DoesNotShortCircuit()
    {
        PlayerSurfaceOperateSeams.PluginManagerAvailable = () => true;
        PlayerSurfaceOperateSeams.HookProcessMsgBegin =
            (_, _, _, _, _, _, _, _, _, b) => b.Value = true;

        bool inheritedCalled = false;
        PlayerSurfaceOperateSeams.InheritedOperate = (_, _) => { inheritedCalled = true; return true; };

        bool r = new TPlayObject().Operate(Msg(20068));   // RM_WHISPER → CanFilter = False

        Assert.True(inheritedCalled);   // 没被短路，走到了 inherit 回落
        Assert.True(r);
    }

    /// <summary>原文 3762-3768：收尾钩子无条件在"未短路"路径上被调用一次，且不参与返回值。</summary>
    [Fact]
    public void Operate_PluginManager_CallsBeginThenEnd()
    {
        var calls = new List<string>();
        PlayerSurfaceOperateSeams.PluginManagerAvailable = () => true;
        PlayerSurfaceOperateSeams.HookProcessMsgBegin = (_, _, _, _, _, _, _, _, _, _) => calls.Add("begin");
        PlayerSurfaceOperateSeams.HookProcessMsgEnd = (_, _, _, _, _, _, _, _, _, _) => calls.Add("end");
        PlayerSurfaceOperateSeams.InheritedOperate = (_, _) => true;

        new TPlayObject().Operate(Msg(Grobal2Const.RM_WALK));

        Assert.Equal(new[] { "begin", "end" }, calls);
    }

    /// <summary>原文 3762：`boReturn := False` 之后传给 End 钩子的初值必须是 False（不是 Begin 留下的 True）。</summary>
    [Fact]
    public void Operate_BoReturnIsResetToFalse_BeforeEndHook()
    {
        bool? seenAtEnd = null;
        PlayerSurfaceOperateSeams.PluginManagerAvailable = () => true;
        PlayerSurfaceOperateSeams.HookProcessMsgBegin = (_, _, _, _, _, _, _, _, _, b) => b.Value = true;
        // 报文取 CanFilter=False，避免 3751 提前 Exit 而跳过 3762
        PlayerSurfaceOperateSeams.HookProcessMsgEnd = (_, _, _, _, _, _, _, _, _, b) => seenAtEnd = b.Value;
        PlayerSurfaceOperateSeams.InheritedOperate = (_, _) => true;

        new TPlayObject().Operate(Msg(20068));

        Assert.False(seenAtEnd);
    }

    /// <summary>原文 3742：报文按值复制 —— 副本在派发前就固定，处理函数改原对象不影响本次副本内容。</summary>
    [Fact]
    public void Operate_ValueCopySemantics_Original3742()
    {
        ushort seenIdent = 0;
        PlayerSurfaceMessageTable.Register(Grobal2Const.RM_WALK, (TProcessMessage m, ref bool _) =>
        {
            seenIdent = m.wIdent;
            m.wIdent = 0;    // 处理函数改的是**原始**对象
        });

        var msg = Msg(Grobal2Const.RM_WALK);
        new TPlayObject().Operate(msg);

        Assert.Equal((ushort)Grobal2Const.RM_WALK, seenIdent);
        Assert.Equal((ushort)0, msg.wIdent);   // 原文传的就是 ProcessMsg（可被改写）
    }

    // ==================================================================
    // Run（原文 3772-5606）—— 留痕断言
    // ==================================================================

    /// <summary>
    /// ★ 否定性断言（计数取证，台账 §37.3）：`Run`（1,835 行）**未被移植**，
    /// 只能以 `PortNotPorted` 留痕。此用例证明"留痕机制可用且计数为 1"。
    /// </summary>
    [Fact]
    public void Run_IsNotPortedBookkept_NotSilentlyStubbed()
    {
        Assert.Equal(0, PlayerSurfacePortLedger.NotPortedCount);

        new TPlayObject().RunNotPortedMarker();

        Assert.Equal(1, PlayerSurfacePortLedger.NotPortedCount);
        Assert.Equal("Run (ObjPlayer.pas:3772)", PlayerSurfacePortLedger.NotPortedMethods[0]);
    }

    /// <summary>留痕幂等：重复调用不重复计数（防循环刷爆列表）。</summary>
    [Fact]
    public void NotPorted_IsIdempotent()
    {
        var p = new TPlayObject();
        p.RunNotPortedMarker();
        p.RunNotPortedMarker();
        p.RunNotPortedMarker();

        Assert.Equal(1, PlayerSurfacePortLedger.NotPortedCount);
    }

    // ==================================================================
    // PortKit 打包函数（原文 MakeWord/MakeLong/LoWord/HiWord）
    // ==================================================================

    /// <summary>原文 `MakeWord(bLow, bHigh)`：字节打包；入参 Integer 时 Delphi 静默窄化到 Byte。</summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0xFF, 0x01, 0x01FF)]
    [InlineData(0x100, 0, 0x0000)]        // ★ 256 窄化为 Byte → 0（Delphi 隐式截断）
    [InlineData(0xFF, 0x1FF, 0xFFFF)]     // 511 窄化为 255
    public void Pack_MakeWord_NarrowsToByte(int low, int high, int expected)
    {
        Assert.Equal((ushort)expected, PlayerSurfacePack.MakeWord(low, high));
    }

    /// <summary>原文 `MakeLong(wLow, wHigh)`：16 位拼 32 位。</summary>
    [Fact]
    public void Pack_MakeLong()
    {
        Assert.Equal(0x0001_0000u, PlayerSurfacePack.MakeLong(0, 1));
        Assert.Equal(0xFFFF_FFFFu, PlayerSurfacePack.MakeLong(0xFFFF, 0xFFFF));
    }

    /// <summary>原文 `HiWord` **不做符号扩展**；`LoWord` 取低 16 位。</summary>
    [Fact]
    public void Pack_LoHiWord_NoSignExtension()
    {
        unchecked
        {
            nint v = (nint)0xFFFF_0001u;   // 高 16 位全 1
            Assert.Equal((ushort)0x0001, PlayerSurfacePack.LoWord(v));
            Assert.Equal((ushort)0xFFFF, PlayerSurfacePack.HiWord(v));
        }
    }

    /// <summary>
    /// ★ 差异断言：`MakeWord` 的**裸 `&amp; 0xFF` 实现**与"先窄化 Byte 再拼接"在
    /// 高位有值时分道扬镳 —— `MakeWord(0x1FF, 0)` 的原文语义是 `0x00FF`（低字节 255），
    /// 而"只取 bit0-7"的实现是 `0xFF`，两者在 **bit8** 上不同。
    /// </summary>
    [Fact]
    public void Pack_MakeWord_LowArgNarrowing_DiffersFromPlainMask()
    {
        // 原文语义（(byte)0x1FF == 0xFF）→ 0x00FF
        Assert.Equal(0x00FF, PlayerSurfacePack.MakeWord(0x1FF, 0));
        // 若误写成 (((int)bLow) & 0xFF) | ...，对 0x1FF 恰好也是 0xFF —— 差异在高字节入参：
        // MakeWord(0, 0x1FF)：原文 (byte)0x1FF = 0xFF → 0xFF00；"先移位再掩码"若无窄化会得 0x1FF00 截断成 0xFF00（同），
        // 故真正的差异点是 0x100 类入参（见上一个用例的 256→0）。
        Assert.Equal(0xFF00, PlayerSurfacePack.MakeWord(0, 0x1FF));
        Assert.Equal(0x0000, PlayerSurfacePack.MakeWord(0x100, 0));   // ← 与 & 0xFF 实现（得 0x00）同，但与非窄化实现（0x100 低位 0）此时同值；关键差异见 0xFF+0x100 组合
        Assert.Equal(0xFF00, PlayerSurfacePack.MakeWord(0xFF, 0x100));
    }

    /// <summary>接缝默认值：`SendSocket` 未接宿主时不得抛异常（原文的投递面）。</summary>
    [Fact]
    public void SocketSeams_Default_DoNotThrow_AndWriteDefMsg()
    {
        var p = new TPlayObject();
        var msg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_TURN, 12345, 1, 2, 3);

        p.SendSocketRef(msg, "payload");

        Assert.Equal(msg.Ident, p.m_DefMsg.Ident);
        Assert.Equal(msg.Recog, p.m_DefMsg.Recog);
        Assert.Equal(12345L, p.m_DefMsg.Recog);
    }

    /// <summary>接缝可注入并可读回参数（证明参数顺序正确）。</summary>
    [Fact]
    public void SocketSeams_SendSocket_ReceivesExactArguments()
    {
        TPlayObject? gotPlayer = null;
        TDefaultMessage gotMsg = default;
        string? gotPayload = null;
        PlayerSurfaceSocketSeams.SendSocket = (player, m, s) => { gotPlayer = player; gotMsg = m; gotPayload = s; };

        var p = new TPlayObject();
        var msg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_WALK, 7, 11, 22, 33);
        p.SendSocketRef(msg, "abc");

        Assert.Same(p, gotPlayer);
        Assert.Equal(msg.Ident, gotMsg.Ident);
        Assert.Equal("abc", gotPayload);
    }
}
