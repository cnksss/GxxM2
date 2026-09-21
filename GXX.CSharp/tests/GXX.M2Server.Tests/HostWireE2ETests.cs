// ============================================================================
// 车道 p17-m2-hostwire：**真宿主端到端**用例（台账 §58.5）
//
// 本文件解决的缺口：§58.5 裁定「没有任何接缝真正接上宿主 ⇒ 无端到端行为被验证」。
// 这里用 **M2Server 自己的宿主入口**（`GXX.M2Server.Program` 的 `M2EngineService`：
// `MainLoop` 独立线程 + `TUserEngine.Process()` + `GateManager` 真 TCP 监听）
// 把已移植的真实实现跑起来，并断言**宿主循环确实推进了状态**，
// 而不是只断言「服务起来了」。
//
// 走通的链路（每一步都是产品代码，测试不造替身，§14.2）：
//   [测试 Socket] ──GM_* 帧──▶ GateManager.AcceptLoop / ProcessGateData（Engine/RunSock.cs）
//        ──OnClientData 事件──▶ M2EngineService.OnGateClientData（Program.cs）
//        ──CM_WALK──▶ TPlayObject.WalkTo（Engine/ObjBase.cs）
//        ──GateManager.SendToClient──▶ [测试 Socket] 收到 SM_WALK 回执
//   主循环侧：
//   M2EngineService.MainLoop（独立线程）──▶ TUserEngine.Process()（Engine/UsrEngn.cs）
//        ├─▶ TPlayObject.Run() ⇒ TCreature.Run() ⇒ Operate() 抽干 RM_* 消息队列
//        └─▶ TMonster.Run()（60ms 节拍）⇒ 追踪 m_Target 走位
//
// ★★ 本文件是**全工程第一段真正跑起 `M2EngineService` 的代码**。
//    取证（可复跑）：
//      git grep -n "M2EngineService" -- GXX.CSharp/tests   ⇒ 修本车道前 **0 命中**
//      git grep -n "M2EngineService" -- GXX.CSharp/src     ⇒ 仅 Program.cs 自身（Main 里 new）
//    ⇒ 在这条车道之前，宿主的 `OnGateClientData` / `MainLoop` **从未被执行过一次**，
//      这正是 §58.5「无端到端行为被验证」的字面含义。它也因此藏着一个真 bug（见下）。
//
// ★★ 第一次运行就抓到宿主的**真缺陷 D-P17-01**（本车道已修）：
//    原 `OnGateClientData` 把 `msg.Recog` 当移动方向读。`Recog` 是**对象标识**
//    （原文 `ObjPlayer.pas:20196 MakeDefaultMsg(SM_USERNAME, NativeInt(Target), ...)`；
//    本文件自己的**出站**帧 `Make(SM_TURN, player.m_nRecogId, dir, x, y)` 也这么用）
//    ⇒ 自我矛盾。而且 `TCreature.WalkTo` 用 `s_DirX[Math.Min(dir, 7)]` 取增量，
//    **任何越界方向都被静默夹到 7（DR_UPLEFT）** ⇒ 玩家朝错误方向走一格，
//    **不抛异常、不打日志**（"静默错误"，本工程最危险的一类）。
//    实测：`Recog` 低位为 238(=0xEE) 时玩家从 x=12 走到 x=11（DR_UPLEFT），
//    而请求的 `DR_RIGHT` 是 x=13。方向在原文取 `ProcessMsg.wParam`
//    （`ObjPlayer.pas:17337 ProcessMsg.wParam { dir }`）⇒ 线上对应 `TDefaultMessage.Param`。
//    回归守卫见 `HostWire_DirectionCarrier_IsParamNotRecog_D_P17_01`。
//
// ⚠ 版本说明（必须写清用的是哪一版）：
//   * `p16-m2-tmonster-run` **尚未并入 main**（分支 `par/p16-m2-tmonster-run` 在飞）
//     ⇒ 本链路用的是**当前可用**的 `TMonster.Run`（Engine/ObjBase.cs:229，18 行近似物）。
//   * `TPlayObject.Run` **未** override（`TPlayObject.PlayerSurface.Core2.cs:380` 只有
//     `RunNotPortedMarker()`；原文 `ObjPlayer.pas:3772` 的真 `Run` 尚未落地）
//     ⇒ 解析到继承来的 `TCreature.Run`（Engine/ObjBase.cs:179）。
//   这两条都在报告里如实登记，不当作"已接线"。
//
// ⚠ 测试纪律：
//   * **不硬编码端口**：`FreeLoopbackPort()` 先向 OS 要一个临时端口再释放（复用即重试）；
//   * **不依赖真实网络**：全部走 127.0.0.1；
//   * **不按值传递巨型结构体**（§52.3）：本文件不构造 `THumData`/`THeroData`；
//   * **否定性断言必须计数取证**（§37.3）：见 `DispatchHook_ReturningTrue_...` 里的
//     计数 + 随后打开通道的对照实验。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GXX.Core.Protocol;
using GXX.GatewayKit;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 真宿主端到端用例：用 `M2EngineService` 的**真主循环 + 真网关监听**跑通 CM_WALK 全链路。
/// </summary>
public class HostWireE2ETests
{
    // ------------------------------------------------------------------ 夹具

    /// <summary>宿主夹具：临时地图目录 + 临时端口 + 真 `M2EngineService`。</summary>
    private sealed class HostFixture : IDisposable
    {
        internal const string MapName = "hostwire";

        internal M2EngineService Engine = null!;
        internal string MapDir = "";
        internal int Port;
        internal readonly List<(string Msg, int Level)> Logs = new();

        internal static HostFixture Start(int width = 40, int height = 40)
        {
            var fixture = new HostFixture();
            fixture.MapDir = Path.Combine(Path.GetTempPath(), "p17host_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(fixture.MapDir);
            WriteClassicMap(Path.Combine(fixture.MapDir, MapName + ".map"), width, height);

            // 端口用「向 OS 要一个临时端口再释放」得到（不硬编码）；绑定失败则换端口重试。
            for (int attempt = 0; attempt < 8; attempt++)
            {
                int port = FreeLoopbackPort();
                var engine = new M2EngineService { MapDir = fixture.MapDir, GatePort = port };
                engine.OnLogMsg += (m, l) =>
                {
                    lock (fixture.Logs) fixture.Logs.Add((m, l));
                };
                if (engine.StartService())
                {
                    fixture.Engine = engine;
                    fixture.Port = port;
                    return fixture;
                }
                engine.Dispose();
            }

            fixture.Dispose();
            throw new InvalidOperationException("宿主无法在 8 个临时端口上启动（GateManager.Start 全部失败）");
        }

        internal bool WaitForGateConnected(int timeoutMs)
            => WaitUntil(() => Engine.GateMgr.GateCount >= 1, timeoutMs);

        internal bool HasMainLoopFailure()
        {
            lock (Logs) return Logs.Any(l => l.Msg.Contains("主循环异常"));
        }

        internal string LogDump()
        {
            lock (Logs) return string.Join(" / ", Logs.Select(l => l.Msg));
        }

        public void Dispose()
        {
            try { Engine?.Dispose(); } catch { }
            try { if (Directory.Exists(MapDir)) Directory.Delete(MapDir, true); } catch { }
        }
    }

    /// <summary>网关侧客户端：真 TCP + `TcpLink` 收包重组 + `TSvrCmdPack` 解帧（全部复用产品设施）。</summary>
    private sealed class GateClient : IDisposable
    {
        private readonly TcpLink _link;
        private readonly List<byte[]> _payloads = new();
        private readonly object _lock = new();

        internal GateClient(int port)
        {
            _link = new TcpLink("127.0.0.1", port);
            _link.OnReceive += OnReceive;
        }

        internal bool Connect() => _link.Connect();

        /// <summary>按 GM_* 帧封装并发送（与网关↔服务器链路同一装配函数）。</summary>
        internal void SendFrame(uint sockId, ushort cmd, byte[]? payload)
        {
            payload ??= Array.Empty<byte>();
            _link.Send(GatewayProtocol.BuildServerPacket(sockId, cmd, 0, payload, payload.Length));
        }

        /// <summary>发一条客户端消息（`TDefaultMessage` 经 `EDcode` 编码后作为 GM_DATA 载荷）。</summary>
        internal void SendClientMessage(uint sockId, in TDefaultMessage msg)
            => SendFrame(sockId, (ushort)GatewayProtocol.GM_DATA, EDcode.EncodeMessage(msg));

        private void OnReceive(byte[] buf, int off, int len)
        {
            _link.Accumulate(buf, off, len);
            Drain();
        }

        /// <summary>解帧：与 `GateManager.ProcessGateData` 同一判定口径。</summary>
        private void Drain()
        {
            while (_link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
            {
                var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(_link.AccumBuffer, 0);
                if (header.Flag != GatewayProtocol.RUNGATECODE)
                {
                    _link.ConsumeAccum(_link.AccumLength);
                    return;
                }
                int dataLen = header.DataLen;
                if (dataLen < 0 || dataLen > 1024 * 1024)
                {
                    _link.ConsumeAccum(_link.AccumLength);
                    return;
                }
                if (_link.AccumLength < GatewayProtocol.SizeOfTSvrCmdPack + dataLen) return;

                byte[] payload = new byte[dataLen];
                if (dataLen > 0)
                    Array.Copy(_link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, payload, 0, dataLen);
                _link.ConsumeAccum(GatewayProtocol.SizeOfTSvrCmdPack + dataLen);
                lock (_lock) _payloads.Add(payload);
            }
        }

        internal int Count(Func<TDefaultMessage, bool> predicate)
        {
            lock (_lock)
                return _payloads.Count(p => p.Length >= 22 && predicate(EDcode.DecodeMessage(p)));
        }

        internal bool WaitFor(Func<TDefaultMessage, bool> predicate, int timeoutMs)
            => WaitUntil(() => Count(predicate) > 0, timeoutMs);

        /// <summary>取第一条命中的已解码消息；必须先用 <see cref="WaitFor"/> 确认存在。</summary>
        internal TDefaultMessage First(Func<TDefaultMessage, bool> predicate)
        {
            lock (_lock)
            {
                foreach (byte[] p in _payloads)
                {
                    if (p.Length < 22) continue;
                    var msg = EDcode.DecodeMessage(p);
                    if (predicate(msg)) return msg;
                }
            }
            throw new InvalidOperationException("GateClient.First：没有命中的帧");
        }

        public void Dispose() => _link.Dispose();
    }

    // ------------------------------------------------------------------ 工具

    /// <summary>向 OS 要一个空闲的环回端口（探测后立即释放）。</summary>
    private static int FreeLoopbackPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    /// <summary>轮询等待；不使用固定 sleep 作为"已发生"的判据。</summary>
    private static bool WaitUntil(Func<bool> condition, int timeoutMs)
    {
        int waited = 0;
        while (waited < timeoutMs)
        {
            if (condition()) return true;
            Thread.Sleep(10);
            waited += 10;
        }
        return condition();
    }

    /// <summary>
    /// 合成经典（非 EN 加密）`.map`：`TMapHeader`(52B, Pack=1) + 列主序 `w*h*12B` 格子。
    /// 全 0 单元格 ⇒ `chFlag == 0` ⇒ 整图可走。
    /// <para>
    /// ⚠ 与 `M2ServerTests.CreateTestMap`（private，EN 加密格式）**不共用**：本文件刻意走
    /// **经典**分支（`Envir.cs:122-128`）以覆盖另一条加载路径；产物格式由 `TMapHeader` 常量决定。
    /// </para>
    /// </summary>
    private static void WriteClassicMap(string path, int width, int height)
    {
        byte[] buf = new byte[52 + width * height * 12];
        BitConverter.GetBytes((ushort)width).CopyTo(buf, 0);      // TMapHeader.wWidth  (:17)
        BitConverter.GetBytes((ushort)height).CopyTo(buf, 2);     // TMapHeader.wHeight (:18)
        // :19 sTitle0 = 0 ⇒ enTitle != "Map 2010 Ver 1.0" ⇒ 走非 EN 分支
        // 其余字节保持 0：UpdateDate/btVersion/Reserved + 全部格子（BkImg=0/FrImg=0 ⇒ 可走）
        File.WriteAllBytes(path, buf);
    }

    /// <summary>
    /// 造一条 CM_WALK：方向放 **`Param`**（= 原文 `TProcessMessage.wParam`，
    /// 见 `ObjPlayer.pas:17337` 对 CM_TURN 的注释 `ProcessMsg.wParam { dir }`），
    /// `Recog` 留 0。
    /// ⚠ 剩余不确定项（已登记，见报告 D-P17-02）：原文 `ClientWalk`（`ObjPlayer.pas:17637`）
    /// 用的是 `nParam1{x}`/`nParam2{y}` 并由其**反解**方向；托管宿主尚未移植该段，
    /// 故这里只驱动"方向"这一格。X/Y 的正确字位（`Tag`/`Series`？）**待 wire 打包映射取证**。
    /// </summary>
    private static TDefaultMessage CmWalk(long recog, int direction, ushort tag = 0, ushort series = 0)
        => TDefaultMessage.Make((ushort)Grobal2Const.CM_WALK, recog, (ushort)direction, tag, series);

    // ------------------------------------------------- 1. 端到端：玩家 CM_WALK 全链路

    /// <summary>
    /// 端到端正例：真 socket → 真网关解帧 → 真宿主分派 → 真 `TPlayObject.WalkTo` → 真回执回 socket。
    /// 断言的是**状态推进**（坐标 + 回执逐字段），不是"服务起来了"。
    /// </summary>
    [Fact]
    public void HostWire_E2E_GatewayCmWalkMovesPlayerAndEchoesSmWalk()
    {
        using var host = HostFixture.Start();

        // 宿主真的通过 LoadMaps 加载了地图（不是注入进来的）
        Assert.Equal(1, host.Engine.UserEngine.MapCount);
        Assert.NotNull(host.Engine.UserEngine.FindMap(HostFixture.MapName));

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect(), "无法连接宿主网关端口 " + host.Port);
        Assert.True(host.WaitForGateConnected(5000), "网关未在 5s 内接受连接；日志=" + host.LogDump());

        // 会话 77：宿主侧先有玩家（原文由 LoginSrv/DBServer 上线流程建立，本用例只造会话）
        var player = new TPlayObject { m_sCharName = "接线测试", m_sUserID = "p17", m_nSocket = 77 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 10, 10));
        Assert.Equal(10, player.m_nCurrX);
        Assert.Equal(10, player.m_nCurrY);

        client.SendClientMessage(77, CmWalk(0, Grobal2Const.DR_DOWN));

        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_WALK, 5000),
            "5s 内未收到 SM_WALK 回执；日志=" + host.LogDump());

        // 回执逐字段断言：Make(SM_WALK, Recog=对象标识, Param=方向, Tag=x, Series=y)
        TDefaultMessage ack = client.First(m => m.Ident == Grobal2Const.SM_WALK);
        Assert.Equal(player.m_nRecogId, ack.Recog);
        Assert.Equal((ushort)Grobal2Const.DR_DOWN, ack.Param);
        Assert.Equal((ushort)10, ack.Tag);
        Assert.Equal((ushort)11, ack.Series);

        // 状态真的推进了：DR_DOWN ⇒ y 10 → 11
        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_nCurrY) == 11, 2000),
            "玩家未按 CM_WALK(DR_DOWN) 前进；实际 y=" + Volatile.Read(ref player.m_nCurrY));
        Assert.Equal(10, Volatile.Read(ref player.m_nCurrX));

        Assert.False(host.HasMainLoopFailure(), "主循环抛异常：" + host.LogDump());
    }

    // ------------------------------------------------- 2. 主循环：节拍 + 怪物 AI + 消息队列

    /// <summary>
    /// 断言**主循环线程确实在跑已移植的 Run/Operate**：
    /// (a) `MainLoopTickHook` 被调用（节拍观测点）；(b) 玩家的 `RM_WALK` 消息队列被
    /// `TCreature.Operate()` 抽干并落实为坐标；(c) `TMonster.Run` 的追踪走位发生。
    /// <para>(a) 顺带就是**宿主接线接缝的 opt-in 正例**。</para>
    /// </summary>
    [Fact]
    public void HostWire_MainLoopAdvancesMonsterAiAndDrainsPlayerMessageQueue()
    {
        using var host = HostFixture.Start();

        int ticks = 0;
        host.Engine.MainLoopTickHook = _ => { Interlocked.Increment(ref ticks); };

        // (b) 目标玩家：怪物追踪的对象
        var target = new TPlayObject { m_sCharName = "靶子" };
        Assert.True(host.Engine.UserEngine.AddPlayObject(target, HostFixture.MapName, 10, 6));

        // (b) 排队玩家：只有主循环里的 Operate 才会消费这条消息
        var walker = new TPlayObject { m_sCharName = "排队玩家" };
        Assert.True(host.Engine.UserEngine.AddPlayObject(walker, HostFixture.MapName, 20, 20));
        walker.SendMsg(Grobal2Const.RM_WALK, Grobal2Const.DR_UP, 0, 0, 0, "");
        Assert.Equal(20, Volatile.Read(ref walker.m_nCurrY)); // 入队后、主循环前：原地不动

        // (c) 怪物：Run 里按目标走位（每 800ms 一步）
        // ★ 集成方修正（台账 §62.3）：本行原为 `monster.m_Target`，那是**旧的 18 行近似物**的字段；
        //   车道 p16-m2-tmonster-run 把 `TMonster.Run` 改成原文 259 行 1:1 后，该近似字段**已删除**
        //   （原文用的是 `m_TargetCret`）。这是**语义合并冲突**：git 看不出，编译器看得出 ——
        //   两个车道各自都在自己的 worktree 里验证过"全树 0 引用"，但**合起来**才暴露。
        var monster = host.Engine.UserEngine.SpawnMonster(HostFixture.MapName, 10, 10, "鸡");
        Assert.NotNull(monster);
        monster!.m_TargetCret = target;
        Assert.Equal(10, Volatile.Read(ref monster.m_nCurrY));

        Assert.True(WaitUntil(() => Volatile.Read(ref ticks) > 0, 5000),
            "MainLoopTickHook 从未被调用 ⇒ 主循环线程没跑；日志=" + host.LogDump());

        Assert.True(WaitUntil(() => Volatile.Read(ref walker.m_nCurrY) == 19, 5000),
            "玩家消息队列未被主循环消费（RM_WALK 未生效）；实际 y="
            + Volatile.Read(ref walker.m_nCurrY) + "；日志=" + host.LogDump());

        // ★ 集成方移除了这里原有的"怪物 AI 会在主循环里推进（y < 10）"断言 —— 见下方 372 行处的说明：
        //   那条断言只对**旧的 18 行近似 Run** 成立；原文 1:1 的 `Run` 需要宿主先装配
        //   `IMonsterRunWorld`/`IMonsterRunEnvirView`（NotPorted 接缝，台账 §61.2），当前宿主没装。
        // 节拍必须持续（不是只跑了一轮）
        int first = Volatile.Read(ref ticks);
        Assert.True(WaitUntil(() => Volatile.Read(ref ticks) > first + 5, 5000),
            "主循环节拍没有持续推进：first=" + first + " now=" + Volatile.Read(ref ticks));

        // ★ 集成方修正（台账 §62.3）：本用例原先断言"怪物会在主循环里沿目标走位（y < 10）"。
        //   那条断言成立的前提是**旧的 18 行近似 `TMonster.Run`**（它自带简化追踪循环）。
        //   车道 p16-m2-tmonster-run 把 Run 改成**原文 259 行 1:1** 后，追踪/攻击/拾取都要经
        //   `IMonsterRunWorld`（`Think`/`AttackTarget`/`GotoTargetXY` …）与 `IMonsterRunEnvirView`
        //   —— 而**宿主目前没有装配它们**（该车道把它们列为 NotPorted 接缝，见台账 §61.2）。
        //   ⇒ 正确行为就是"**AI 的追踪路径当前是惰性的**"：这里改成断言这一点，
        //   并把它登记为宿主装配的下一步（"宿主缺失"类，D-P17-03）。**不是**把断言放宽了事。
        Assert.Equal(10, Volatile.Read(ref monster.m_nCurrY));
        Assert.Equal(10, Volatile.Read(ref monster.m_nCurrX));
        Assert.False(host.HasMainLoopFailure(), "主循环抛异常：" + host.LogDump());
        Assert.True(host.Engine.ServiceStarted);
    }

    // ------------------------------------------------- 3. 默认路径不变（§59.7 的核心判据）

    /// <summary>
    /// **默认路径不变** 的正例：`GateMessageDispatchHook == null`（默认）时，
    /// 默认 `CM_*` 分派照常生效 —— 与接线前完全同一可观测结果。
    /// <para>
    /// 与 <see cref="DispatchHook_ReturningTrueSuppressesDefaultSwitch"/> 合成一对：
    /// 只有返回 <c>true</c> 才能压制默认分派 ⇒ 证明"默认行为没被搬空"（§59.7）。
    /// </para>
    /// </summary>
    [Fact]
    public void DispatchHook_WhenOff_DefaultCmWalkDispatchUnchanged()
    {
        using var host = HostFixture.Start();
        host.Engine.GateMessageDispatchHook = null; // 显式：关闭（默认值）

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect());
        Assert.True(host.WaitForGateConnected(5000));

        var player = new TPlayObject { m_sCharName = "默认路径", m_nSocket = 91 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 12, 12));

        client.SendClientMessage(91, CmWalk(0, Grobal2Const.DR_RIGHT));

        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_WALK, 5000),
            "关闭接缝时默认分派失效；日志=" + host.LogDump());
        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_nCurrX) == 13, 2000),
            "关闭接缝时玩家未前进；实际 x=" + Volatile.Read(ref player.m_nCurrX));
        Assert.Equal(12, Volatile.Read(ref player.m_nCurrY));
    }

    /// <summary>
    /// 防漏断言（"其它全开只关它"的镜像）：接缝装了、但**返回 false** ⇒ 必须**落回默认分派**。
    /// 若把默认分支的语句搬进 opt-in 分支（§59.7 的真实回归形态），本用例会红。
    /// </summary>
    [Fact]
    public void DispatchHook_ReturningFalseFallsThroughToDefaultDispatch()
    {
        using var host = HostFixture.Start();

        var seen = new List<(int SockId, ushort Ident)>();
        host.Engine.GateMessageDispatchHook = (sockId, msg) =>
        {
            lock (seen) seen.Add((sockId, msg.Ident));
            return false; // 明确不消费 ⇒ 默认分派必须继续执行
        };

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect());
        Assert.True(host.WaitForGateConnected(5000));

        var player = new TPlayObject { m_sCharName = "落回默认", m_nSocket = 92 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 12, 12));

        client.SendClientMessage(92, CmWalk(0, Grobal2Const.DR_RIGHT));

        // 接缝确实收到了**已解码**的消息（证明它接在解码之后、分派之前）
        Assert.True(WaitUntil(() =>
        {
            lock (seen) return seen.Any(s => s.SockId == 92 && s.Ident == Grobal2Const.CM_WALK);
        }, 5000), "接缝未收到解码后的 CM_WALK；日志=" + host.LogDump());

        // 且默认分派照常完成
        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_WALK, 5000),
            "返回 false 时默认分派被跳过；日志=" + host.LogDump());
        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_nCurrX) == 13, 2000),
            "返回 false 时玩家未前进；实际 x=" + Volatile.Read(ref player.m_nCurrX));
    }

    /// <summary>
    /// opt-in 生效 + **否定性断言计数取证** + 对照实验：
    /// <list type="number">
    ///   <item>接缝返回 <c>true</c> ⇒ 默认分派被跳过（玩家不动、无 SM_WALK 回执）；</item>
    ///   <item>同一帧在**同一宿主**里把接缝置回 <c>null</c> 再发一次 ⇒ 玩家前进、
    ///         收到 SM_WALK —— 证明第 1 步的"什么都没有"是**被压制**造成的，
    ///         而不是链路坏了（否则第 1 步是假绿）。</item>
    /// </list>
    /// </summary>
    [Fact]
    public void DispatchHook_ReturningTrueSuppressesDefaultSwitch()
    {
        using var host = HostFixture.Start();

        int hookCalls = 0;
        host.Engine.GateMessageDispatchHook = (sockId, msg) =>
        {
            if (msg.Ident != Grobal2Const.CM_WALK) return false;
            Interlocked.Increment(ref hookCalls);
            return true; // 消费掉 ⇒ 默认 switch 不得执行
        };

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect());
        Assert.True(host.WaitForGateConnected(5000));

        var player = new TPlayObject { m_sCharName = "被压制", m_nSocket = 93 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 12, 12));

        client.SendClientMessage(93, CmWalk(0, Grobal2Const.DR_RIGHT));

        Assert.True(WaitUntil(() => Volatile.Read(ref hookCalls) == 1, 5000),
            "接缝未被调用；日志=" + host.LogDump());

        // 否定性断言：给足时间后**计数**为 0（不是"没看到"）
        Thread.Sleep(300);
        Assert.Equal(0, client.Count(m => m.Ident == Grobal2Const.SM_WALK));
        Assert.Equal(12, Volatile.Read(ref player.m_nCurrX));
        Assert.Equal(1, Volatile.Read(ref hookCalls)); // 只被调用一次，没有重发

        // ---- 对照实验：同一宿主、接缝置回 null、同一帧再发一次 ----
        host.Engine.GateMessageDispatchHook = null;
        client.SendClientMessage(93, CmWalk(0, Grobal2Const.DR_RIGHT));

        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_WALK, 5000),
            "对照实验失败：接缝置 null 后默认分派仍未生效 ⇒ 链路本身不可用；日志=" + host.LogDump());
        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_nCurrX) == 13, 2000),
            "对照实验失败：玩家未前进；实际 x=" + Volatile.Read(ref player.m_nCurrX));
    }

    // ------------------------------------------------- 4. D-P17-01 回归守卫（方向字段）

    /// <summary>
    /// **D-P17-01 回归守卫（差异断言）**：方向载体必须是 `Param`，**不是** `Recog`。
    ///
    /// <para>
    /// 做法：同一帧里故意让两个字段**给出相反的方向** ——
    /// `Recog = 7`（低位 = `DR_UPLEFT`，若被当方向用会走**左上**：x−1, y−1）
    /// `Param = DR_RIGHT`（正确方向：x+1, y±0）。
    /// 然后断言玩家**向右**走了。
    /// </para>
    /// <para>
    /// 同时用回执帧把**约定**锁死：出站 `Make(SM_WALK, Recog, Param, Tag, Series)` 里
    /// `Recog = player.m_nRecogId`（对象标识）、`Param = 方向` —— 与自己的入站读法一致。
    /// </para>
    /// <para>
    /// 若有人把入站改回 `(byte)msg.Recog`，本用例立刻变红（且方向会变成左上）。
    /// </para>
    /// </summary>
    [Fact]
    public void HostWire_DirectionCarrier_IsParamNotRecog_D_P17_01()
    {
        using var host = HostFixture.Start();

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect());
        Assert.True(host.WaitForGateConnected(5000));

        var player = new TPlayObject { m_sCharName = "方向载体", m_nSocket = 95 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 12, 12));

        const long recogTrap = (long)Grobal2Const.DR_UPLEFT; // 7：若被当方向 ⇒ 走左上
        client.SendClientMessage(95, CmWalk(recogTrap, Grobal2Const.DR_RIGHT));

        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_WALK, 5000),
            "未收到 SM_WALK；日志=" + host.LogDump());
        TDefaultMessage ack = client.First(m => m.Ident == Grobal2Const.SM_WALK);
        Assert.Equal(player.m_nRecogId, ack.Recog);                       // 出站：Recog = 对象标识
        Assert.Equal((ushort)Grobal2Const.DR_RIGHT, ack.Param);           // 出站：Param = 方向

        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_nCurrX) == 13, 2000),
            "x 未按 DR_RIGHT 前进 ⇒ 方向可能仍取自 Recog；实际 x=" + Volatile.Read(ref player.m_nCurrX));
        Assert.Equal(12, Volatile.Read(ref player.m_nCurrY)); // 左下/左上都会改 y ⇒ 必须不变
    }

    /// <summary>
    /// CM_TURN：**原文缺陷照抄 + 差异断言**。
    /// 原文 `TPlayObject.ClientChangeDir`（`ObjPlayer.pas:17269-17273`）：
    /// <code>
    /// if not(nDir in [DR_UP .. DR_UPLEFT]) then
    /// begin
    ///   Result := True;   // ← 越界方向算"已处理"
    ///   Exit;             // ← 且**不发任何回执**、不改变朝向
    /// end;
    /// </code>
    /// 断言：越界方向 ⇒ `m_btDirection` 不变 **且** 无 `SM_TURN` 回执；
    /// 随后合法方向 ⇒ 朝向改变 **且** 收到回执（对照，证明通道可用）。
    /// </summary>
    [Fact]
    public void HostWire_CmTurn_OutOfRangeDirectionAcceptedWithoutAck_OriginalDefect()
    {
        using var host = HostFixture.Start();

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect());
        Assert.True(host.WaitForGateConnected(5000));

        var player = new TPlayObject { m_sCharName = "越界转向", m_nSocket = 96 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 12, 12));
        byte before = player.m_btDirection;

        // 越界方向 200（> DR_UPLEFT=7）
        client.SendClientMessage(96, TDefaultMessage.Make((ushort)Grobal2Const.CM_TURN, 0, 200, 0, 0));
        Thread.Sleep(300);
        Assert.Equal(before, Volatile.Read(ref player.m_btDirection));      // 朝向未变
        Assert.Equal(0, client.Count(m => m.Ident == Grobal2Const.SM_TURN)); // 且无回执（计数取证）

        // 对照：合法方向 ⇒ 朝向改变 + 回执
        client.SendClientMessage(96, TDefaultMessage.Make((ushort)Grobal2Const.CM_TURN, 0, Grobal2Const.DR_LEFT, 0, 0));
        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_TURN, 5000),
            "对照实验失败：合法方向未收到 SM_TURN；日志=" + host.LogDump());
        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_btDirection) == Grobal2Const.DR_LEFT, 2000),
            "对照实验失败：朝向未改变；实际 dir=" + Volatile.Read(ref player.m_btDirection));
    }

    // ------------------------------------------------- 5. 畸形帧不得终止宿主

    /// <summary>
    /// 畸形输入（短载荷 + 错 Flag 帧）之后宿主必须仍然可用：主循环继续推进，
    /// 且随后一条合法 CM_WALK 照常生效。这也顺带证明 `data.Length &lt; 22` 的门控
    /// **留在默认路径上**（没有因为接缝而挪位）。
    /// </summary>
    [Fact]
    public void MalformedGatewayFramesDoNotStopHostOrMainLoop()
    {
        using var host = HostFixture.Start();

        int ticks = 0;
        host.Engine.MainLoopTickHook = _ => { Interlocked.Increment(ref ticks); };

        using var client = new GateClient(host.Port);
        Assert.True(client.Connect());
        Assert.True(host.WaitForGateConnected(5000));

        var player = new TPlayObject { m_sCharName = "畸形帧后", m_nSocket = 94 };
        Assert.True(host.Engine.UserEngine.AddPlayObject(player, HostFixture.MapName, 12, 12));

        // (1) 合法 GM_DATA 但载荷只有 3 字节 ⇒ 命中 `data.Length < 22` 提前返回
        client.SendFrame(94, (ushort)GatewayProtocol.GM_DATA, new byte[] { 1, 2, 3 });
        Thread.Sleep(120);

        // (2) Flag 不对的帧 ⇒ ProcessGateData 丢弃整段累计并返回
        byte[] bogus = GatewayProtocol.BuildServerPacket(94, (ushort)GatewayProtocol.GM_DATA, 0,
            EDcode.EncodeMessage(CmWalk(0, Grobal2Const.DR_RIGHT)), 22);
        bogus[0] ^= 0xFF; // 破坏 Flag
        client.SendFrame(94, (ushort)GatewayProtocol.GM_DATA, bogus);
        Thread.Sleep(120);

        Assert.True(WaitUntil(() => Volatile.Read(ref ticks) > 0, 5000),
            "主循环未跑；日志=" + host.LogDump());
        Assert.Equal(12, Volatile.Read(ref player.m_nCurrX)); // 畸形帧不得造成位移
        Assert.False(host.HasMainLoopFailure(), "主循环抛异常：" + host.LogDump());

        // (3) 随后一条合法帧必须照常生效 ⇒ 宿主与链路都还活着
        client.SendClientMessage(94, CmWalk(0, Grobal2Const.DR_RIGHT));
        Assert.True(client.WaitFor(m => m.Ident == Grobal2Const.SM_WALK, 5000),
            "畸形帧之后合法帧失效；日志=" + host.LogDump());
        Assert.True(WaitUntil(() => Volatile.Read(ref player.m_nCurrX) == 13, 2000),
            "畸形帧之后玩家未前进；实际 x=" + Volatile.Read(ref player.m_nCurrX));
    }

    // ------------------------------------------------- 6. 宿主生命周期契约

    /// <summary>
    /// 宿主生命周期：`StartService` 幂等；停服后主循环节拍必须**停止增长**
    /// （对应 `svMain.pas` 的"停止引擎"语义），且 `ServiceStarted` 归 false。
    /// </summary>
    [Fact]
    public void HostServiceLifecycle_StopHaltsMainLoopTicks()
    {
        using var host = HostFixture.Start();

        int ticks = 0;
        host.Engine.MainLoopTickHook = _ => { Interlocked.Increment(ref ticks); };
        Assert.True(host.Engine.ServiceStarted);
        Assert.True(host.Engine.StartService(), "重复 StartService 必须幂等返回 true");
        Assert.True(WaitUntil(() => Volatile.Read(ref ticks) > 0, 5000));

        host.Engine.StopService();
        Assert.False(host.Engine.ServiceStarted);

        int afterStop = Volatile.Read(ref ticks);
        Thread.Sleep(200);
        int later = Volatile.Read(ref ticks);
        Assert.True(later <= afterStop,
            "停服后主循环仍在推进节拍：afterStop=" + afterStop + " later=" + later);
    }
}
