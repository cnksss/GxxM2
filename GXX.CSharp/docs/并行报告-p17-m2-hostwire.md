# 并行报告 — 车道 `p17-m2-hostwire`

> **承接**：台账 **§58.5** 的战略裁定 —— *"没有任何接缝真正接上宿主 ⇒ **无端到端行为被验证**"*；
> 并给出两件交付：① 汇总全工程接缝的**接线工单表**；② **跑通一条真实端到端链路**。
>
> **前置必读**（本车道已读）：§33.4「可接线 ≠ 已接线」、§51.3「缺入口点 ⇒ 调用点无法接线」、
> §52.3「门禁假绿陷阱」、§58.5「战略缺口」、§59.7「接线把默认路径的转发搬空」；
> 先例 `docs/并行报告-p14-logingate-wire.md`（opt-in + 默认路径逐字节不变 + 结构性命明用例）。
>
> 分支 `par/p17-m2-hostwire` · 工作树 `D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-m2-hostwire`

---

## §0 一句话

**端到端跑通了。** 用 M2Server 自己的宿主入口（`M2EngineService` 的 **真主循环线程** +
`GateManager` 的**真 TCP 监听**）把已移植的真实实现跑起来，**9 条用例全绿**；
而**第一次把它跑起来就抓到一个静默真 bug（D-P17-01）**——宿主把 `msg.Recog`（对象标识）
当移动方向读，`WalkTo` 又用 `Math.Min(dir,7)` **静默夹取**越界方向，于是玩家**朝错误方向走一格，
不抛异常、不打日志**。这正是 §58.5 说的"无端到端行为被验证"的**字面后果**：
`M2EngineService` 在 `tests/**` 的修前命中数是 **0** —— 这份宿主代码**从未被执行过一次**。

同时交出 **`docs/接线工单表.md`**：**596 条**已逐条核对的接缝（另按机械扫描口径
`GXX.M2Server` 一侧为 369 静态委托 + 130 实例 + 34 非委托），并**逐条分类"忘记注入的后果"**
（静默 / 显式抛 / 可见占位）。**产品代码里的"宿主 → 接缝"赋值 = 0 个**（唯一 4 处是"转发属性 setter"）。

---

## §1 交付物

| # | 文件 | 内容 |
|---|---|---|
| 1 | `GXX.CSharp/docs/接线工单表.md`（**新建**） | 表 A 宿主接线层 9 条（逐条核过）；表 B `GXX.M2Server` 369+130+34；表 C GatewayKit/LoginGate 20 条 + Core 14 条；Top 危险 15 条；阻塞 14 条；**8 段可复跑命令** |
| 2 | `GXX.CSharp/tests/GXX.M2Server.Tests/HostWireE2ETests.cs`（**新建**，9 例） | 真宿主端到端（真 socket + 真主循环）+ 接缝 opt-in 正/反例 + D-P17-01 回归守卫 + 原文缺陷差异断言 |
| 3 | `GXX.CSharp/tests/GXX.M2Server.Tests/HostWireSourceContractTests.cs`（**新建**，6 例） | 源码级**结构性契约**（§59.7 的机器化守卫） |
| 4 | `GXX.CSharp/src/GXX.M2Server/Program.cs`（**改**，本车道独占分区） | ① 新增 2 处 **opt-in** 宿主接缝；② 修 **D-P17-01**；③ `CM_TURN` 越界分支照抄原文；④ `CM_QUERYUSERNAME` 的未 1:1 事实写进代码注释 |
| 5 | 本报告 | — |

**新增用例数：15**（9 + 6）。M2Server 侧用例总数 **10,696 → 10,711**（+15，**不多不少**）。

---

## §2 接了哪几条

### §2.1 本车道真接线（2 条，均在 `Program.cs`，opt-in）

| 接缝 | 位置 | 做法（照抄 `p14-logingate-wire`） |
|---|---|---|
| `M2EngineService.MainLoopTickHook : Action<uint>?` | `Program.cs:77` | `MainLoop` 每轮开头 `var h = MainLoopTickHook; if (h != null) h(DelphiRTL.GetTickCount());` ⇒ **默认 null 时连 `GetTickCount()` 都不取** |
| `M2EngineService.GateMessageDispatchHook : Func<int,TDefaultMessage,bool>?` | `Program.cs:90` | `OnGateClientData` **解码之后、默认 `switch` 之前** `if (h != null && h(sockId, msg)) return;` ⇒ 返回 `false` 也落默认分派 |

**两处都是 `public` 的*实例*字段（不是 `static`）**，理由：
① 不污染进程级静态全局；② 免于"`M2ConfigIsolationCoverage.cs` 是否覆盖本类型"的不确定性
（该文件**不在本车道分区** ⇒ 无法自行纳入，见 §7 D-P17-04）⇒ 由用例自己在 `finally`/`using` 里置回。

### §2.2 「关闭时行为等价」的证明（§59.7 的硬要求）

1. **行为正例**：`DispatchHook_WhenOff_DefaultCmWalkDispatchUnchanged` —— 接缝为 `null` 时，
   同一 CM_WALK 帧照旧推进玩家坐标并收回执。
2. **防漏反例（"其它全开只关它"的镜像）**：`DispatchHook_ReturningFalseFallsThroughToDefaultDispatch` ——
   接缝装了但返回 `false` ⇒ **必须**落回默认分派（若有人把默认分支的语句搬进 opt-in 分支，**本用例变红**）。
3. **opt-in 真生效 + 否定性断言计数取证 + 对照实验**：
   `DispatchHook_ReturningTrueSuppressesDefaultSwitch` —— 返回 `true` ⇒ 玩家不动、**计数为 0** 的 `SM_WALK`；
   随后**同一宿主**把接缝置回 `null` 再发同一帧 ⇒ 玩家前进、收到回执（证明第 1 步的"什么都没有"是**被压制**，
   而不是链路坏了 —— 否则第 1 步是假绿）。
4. **源码级结构契约**（6 例）：机器断言
   - `if (data.Length < 22) return;` 与 `TDefaultMessage msg = EDcode.DecodeMessage(data);`
     **出现在接缝判断之前**（即**留在默认路径**，没被搬进分支）；
   - 接缝判断**在 `switch (msg.Ident)` 之前**；
   - 4 个默认 `CM_*` case 标签**仍然存在**；
   - `MainLoopTickHook` 的 null 守卫**在 `UserEngine.Process()` 之前**；
   - `(byte)msg.Recog`（当方向读）**一个都不剩**、`byte dir = (byte)msg.Param;` **恰好 2 处**；
   - `Program.Main` 里 `new M2EngineService()` 在 `Application.Run` 之前（宿主**不是死代码**）。

### §2.3 为什么**没有**接更多

本车道的分区只有 `GXX.M2Server/Program.cs`。表 B 的 523 条接缝分布在
`Engine/**`、`Npc/**`、`DbLayer/**`、`Forms/**`、`Sweep9/**` —— **都不在本车道分区**。
本车道的职责是**把断层变成工单**（表 A 已经精确到"缺哪个成员"），而不是越区去接。
**唯一在分区内的宿主入口，本车道已把它从"从未执行"变成"有 15 条用例守着"。**

---

## §3 端到端链路走通了哪一段

### §3.1 走通的链路（每一步都是产品代码，测试不造替身，§14.2）

```
真 socket ──GM_*帧──▶ GateManager.AcceptLoop / ProcessGateData      (Engine/RunSock.cs:72/104)
        ──OnClientData 事件──▶ M2EngineService.OnGateClientData      (Program.cs:142)
        ──CM_WALK──▶ TPlayObject.WalkTo                              (Engine/ObjBase.cs:133)
        ──GateManager.SendToClient──▶ 真 socket 收到 SM_WALK 回执     (Engine/RunSock.cs:157)
主循环侧：
M2EngineService.MainLoop（独立后台线程, :121）──▶ TUserEngine.Process()  (Engine/UsrEngn.cs:153)
        ├─▶ TPlayObject.Run() ⇒ TCreature.Run() ⇒ Operate() 抽干 RM_* 消息队列
        └─▶ TMonster.Run()（60ms 节拍）⇒ 追踪 m_Target 走位
```

**地图也是走真加载路径**：测试在临时目录合成 **经典（非 EN）格式** `.map`
（`TMapHeader` 52B + 列主序 `w*h*12B`），由 `TUserEngine.LoadMaps`（`Envir.cs:99` 的
**非 EN 分支**，与既有 `M2ServerTests.CreateTestMap` 的 EN 分支互补）真读进来 ——
**不是**往 `MapList` 里注入对象。

### §3.2 跑通的是**哪一版**（按要求写明）

| 成员 | 用的哪一版 | 说明 |
|---|---|---|
| `TMonster.Run` | **当前可用的 18 行近似物**（`Engine/ObjBase.cs:229`） | `p16-m2-tmonster-run` **尚未并入 main**（分支 `par/p16-m2-tmonster-run` 在飞）⇒ 本报告**不声称**怪物 AI 已 1:1 |
| `TPlayObject.Run` | **继承来的 `TCreature.Run`**（`Engine/ObjBase.cs:179`） | `TPlayObject` **没有** `Run` 的 override；`TPlayObject.PlayerSurface.Core2.cs:380` 只有 `RunNotPortedMarker()`（原文 `ObjPlayer.pas:3772` 的 1,835 行真 `Run` 未落地）⇒ `Process()` 实际走 `TCreature.Run → Operate()` |
| 宿主本身 | `M2EngineService`（`Program.cs:29`） | 真入口；`Program.Main` 是 WinForms `Application.Run(new FrmMain(engine))` |

### §3.3 断言的是"状态推进"，不是"服务起来了"

| 用例 | 断言 |
|---|---|
| `HostWire_E2E_GatewayCmWalkMovesPlayerAndEchoesSmWalk` | 宿主 `MapCount==1` 且 `FindMap` 命中；真 socket 收到 `SM_WALK` 且**逐字段**（`Recog`=对象标识、`Param`=方向、`Tag`=x、`Series`=y）正确；玩家 `y: 10 → 11` |
| `HostWire_MainLoopAdvancesMonsterAiAndDrainsPlayerMessageQueue` | (a) `MainLoopTickHook` 被调用且**持续增长 > first+5**；(b) 玩家入队的 `RM_WALK` 被主循环抽干 ⇒ `y: 20 → 19`；(c) 怪物 `Run` 追踪 `m_Target` ⇒ `y < 10`；(d) **主循环日志无 `主循环异常`** |
| `HostWire_DirectionCarrier_IsParamNotRecog_D_P17_01` | 帧内故意让 `Recog` 低位给 `DR_UPLEFT`、`Param` 给 `DR_RIGHT` ⇒ **只能向右**（`x: 12→13`、`y` 不变） |
| `HostWire_CmTurn_OutOfRangeDirectionAcceptedWithoutAck_OriginalDefect` | 越界方向 `200` ⇒ 朝向不变 **且** `SM_TURN` **计数为 0**；合法方向 ⇒ 朝向改变 + 收到回执（对照） |
| `MalformedGatewayFramesDoNotStopHostOrMainLoop` | 短载荷（`< 22`）与错 Flag 帧之后：位移为 0、主循环仍在跑、**随后一条合法帧照常生效** |
| `HostServiceLifecycle_StopHaltsMainLoopTicks` | `StartService` 幂等；`StopService` 后节拍**停止增长** |

### §3.4 宿主跑通的**外部可观测证据**（不是只断言"服务起来了"）

- 用例里没有硬编码端口：`FreeLoopbackPort()` 向 OS 要临时端口后释放（绑定失败换端口重试 8 次）。
- 网络只有回环 `127.0.0.1`。
- 未按值传递巨型结构体（§52.3）：`THumData`/`THeroData` 在本文件**未出现**。

---

## §4 ★ 第一次跑起来就抓到的真缺陷（D-P17-01）

### §4.1 决定性实验数据

第一次运行 `HostWire_E2E_...` 时红，且**红得很奇怪**：

```
Assert.Equal() Failure: Values differ
Expected: 4      (SM_WALK 的 Param 应为 DR_DOWN)
Actual:   238
...
「关闭接缝时玩家未前进；实际 x=11」   ← 玩家从 x=12 走到 x=11（应到 13）
```

`238 = 0xEE`，低 3 位 = `6 = DR_LEFT`；而 `(byte)player.m_nRecogId` 恰好是 238
（`TCreature.NextRecogId` 从 1000 起全局自增，被整个测试套件的构造次数推到该值）。
⇒ **宿主把 `msg.Recog` 当方向读**，且 `WalkTo` 的 `s_DirX[Math.Min(dir, 7)]`
把越界方向**静默夹取**，所以"走错方向"**既不抛也不打日志**。

### §4.2 原文证据（三条独立站点）

| 位置 | 原文写法 | 含义 |
|---|---|---|
| `Source/M2Engine/ObjPlayer.pas:17337` | `ClientChangeDir(ProcessMsg.wIdent, ProcessMsg.nParam1 { x }, ProcessMsg.nParam2 { y }, ProcessMsg.wParam { dir }, …)` | **CM_TURN 的方向在 `wParam`** |
| `Source/M2Engine/ObjPlayer.pas:17637` | `ClientWalkXY(ProcessMsg.wIdent, ProcessMsg.nParam1 { x }, ProcessMsg.nParam2 { y }, …)` | **CM_WALK 用 x/y，方向由坐标反解**（没有"方向字段"） |
| `Source/M2Engine/ObjPlayer.pas:20189-20196` | `Target := TBaseObject(ProcessMsg.nParam1); X := ProcessMsg.nParam2; Y := ProcessMsg.nParam3; … MakeDefaultMsg(SM_USERNAME, NativeInt(Target), …)` | **`Recog` 是对象标识**，不是参数载体 |

（原文为 GBK；上表用 `_analysis/utf8_mirror` 的 UTF-8 镜像读取，行号为镜像实测。）

### §4.3 自我一致性证明（不需要 wire 打包映射就能定性）

本文件**自己的出站帧**写的是
`Make(SM_WALK, player.m_nRecogId, dir, x, y)` 与 `Make(SM_TURN, player.m_nRecogId, dir, x, y)`
—— 即 **`Recog` = 对象标识、`Param` = 方向**。
而**入站**却从 `Recog` 读方向 ⇒ **同一文件内部自相矛盾**。
⇒ 修法（**3 行**）：入站方向一律改读 `msg.Param`。

### §4.4 修复 + 回归守卫

- `Program.cs:160`（CM_WALK/CM_RUN）与 `:186`（CM_TURN）：`(byte)msg.Recog` → `(byte)msg.Param`，
  并把原文依据写进注释。
- 顺带**照抄原文缺陷**：`CM_TURN` 加上 `if (dir > DR_UPLEFT) break;`
  （原文 `ObjPlayer.pas:17269-17273` `Result := True; Exit;` ⇒ 越界方向被视为"已处理"且**不回执**），
  并用 `HostWire_CmTurn_OutOfRangeDirectionAcceptedWithoutAck_OriginalDefect` 锁死。
- 行为回归守卫：`HostWire_DirectionCarrier_IsParamNotRecog_D_P17_01`（反例构造：`Recog` 与 `Param` 给相反方向）。
- 源码回归守卫：`HostWireSourceContractTests.DirectionCarrier_IsParam_NoRecogAsDirection_D_P17_01`
  （`(byte)msg.Recog` 出现次数必须为 0，`(byte)msg.Param` 恰好 2）。

### §4.5 为什么这个"根因"值得单独记一条

§58.5 的裁定说"目标的可验证部分主要卡在**接线 + 端到端**"。这条是它的**第一个实证**：
宿主代码**编译通过、单元测试全绿、覆盖率审计也看不出问题**，
但**一次真实执行**就暴露"走错方向且完全静默"。
⇒ **"从未被执行过的代码"不是"已移植的代码"，它只是"能编译的代码"。**

---

## §5 工单表统计

### §5.1 总数 / 已接 / 未接 / 静默类

| 分区 | 条目 | 已接宿主 | 未接 | 静默(a) | 抛(b) | 占位(c) |
|---|---|---|---|---|---|---|
| A. M2Server 宿主接线层（逐条核过） | **9** | **2** | 7 | 3 | 0 | 6 |
| B. `GXX.M2Server` 静态委托 | **369** | 0 | 369 | ≈250 | ≈20 | ≈99 |
| B′. `GXX.M2Server` 实例 handler | **130** | 0 | 130 | ≈100 | ≈6 | ≈24 |
| B″. `GXX.M2Server` 非委托静态 | **34** | 0 | 34 | ≈25 | 2 | 7 |
| C. `GXX.GatewayKit`+`GXX.LoginGate`（逐条核过） | **40** | 19 + **2 条失效** | 21 | 34 | 1 | 5 |
| D. `GXX.Core`（逐条核过） | **14** | 1 | 13 | 8 | 5 | 1 |
| **合计（A–D，已逐条核对）** | **596** | **22（+2 失效）** | **574** | **≈420** | **≈34** | **≈142** |
| E. 其它程序集（**第二方只读普查**，见下） | **321 + ≈52（Client 抽样）** | **73** | **248+** | **≈200** | **≈59** | **≈13** |
| **合计（A–E）** | **917+** | **95** | **822+** | **≈620** | **≈93** | **≈155** |

**诚实声明**：A / C / D 的每一行都读过源码，数字精确；
B / B′ / B″ 的三档分布来自**机械扫描 + 抽样核对**，故写 `≈`（**不是**逐条读过 596 行）。
E 区来自**一条独立的第二方只读普查**（判据与本表一致；本车道**抽检 3 条 `file:line` 均准确**，
但**未全量逐行复核**）⇒ **单列**。按 §59.2-1 的规程：**第二方普查 ≠ 本车道已核**。

### §5.2 最重要的一个数

```
src_provider   = 4     ← 全部是"转发属性 setter"，不是真实现
tests_provider = 765
ResetDefaults  = 34    ← 34 个接缝类各自写了一个 ResetDefaults()
```

⇒ **`GXX.M2Server` 里没有任何 HostWire / CompositionRoot；523 条接缝没有"被接上"的地方。**
这印证并**量化**了 §58.5，也解释了 §D-P17-03：**缺的不是"接线动作"，是"接线的地方"。**

### §5.3 已接的 22 条分别是什么

| 来源 | 条数 | 内容 |
|---|---|---|
| `p14-logingate-wire`（已并入 main） | 19 | Rest11 执法面（默认 OFF、第一行短路）—— 但其中 **2 条实测失效**，见 §6 X-P17-01 |
| 本车道 `p17-m2-hostwire` | 2 | `MainLoopTickHook` / `GateMessageDispatchHook` |
| `GXX.Core` 默认即真实现 | 1 | `AsyncCalls.ThreadPool`（惰性自建） |

---

## §6 跨区事项（本车道只登记，不越区）

### §6.1 ★★ X-P17-01：Rest11 的 "opt-in" 在网关层**结构性失效**（已独立确认）

| 项 | 内容 |
|---|---|
| **现象** | 即使宿主 `new LoginGateService(Rest11LoginGateOptions.All)`，`GateService.CheckIP` 的三条 Rest11 判定（`IsBlockIP`/`IsBlockIPArea`/`OverConnectOfIP`，`GateService.cs:230/235/240`）**不可达**；`EnableIpAddrFilterResidual` 是**装饰品**。 |
| **根因** | 基类 `protected virtual Rest11LoginGateOptions? Rest11Options => null;`（`GateService.cs:307`，**get-only**）；派生类 `public Rest11LoginGateOptions? Rest11Options { get; set; }`（`LoginGateService.cs:56`，**无 `override`**）是**隐藏**。C# 不允许 get-only 覆写为 get/set（CS0546）⇒ 只能隐藏。而在 `GateService` 自己的方法体内，`Rest11Options` **静态绑定到基类的恒 null 属性**。 |
| **为什么没人发现** | ① 暴露它的警告 **`CS0108`/`CS0114` 被全局 `NoWarn` 关掉**（`GXX.CSharp/Directory.Build.props:12`）；② 报告引用的 3 个端到端用例**在本工作树 grep = 0 命中**（台账 §59.2-2：被归档成 markdown，未启用）；③ `LoadConfig` 那一半被 `LoginGateService.cs:121` 读**派生**属性**意外补偿** ⇒ "打开即加载原文段名"看起来是通的。 |
| **后果分级** | **a 静默**（正是本表最危险的一类） |
| **对既有结论的影响** | `docs/并行报告-p14-logingate-wire.md` 的"**15/17 已真正接上**"需要**下修**：17 项里第 #1/#2/#3（黑名单/IP 段/每 IP 连接数）在 `CheckIP` 侧**未真正接上**。 |
| **修法** | 见 `docs/接线工单表.md` §4.1（两种改法 + 各自的公开 API 影响，**需集成方裁定**）。 |
| **必须同时加的守卫** | 一条**行为**用例：opt-in 后各造一个黑名单/段表/超连接数命中输入，断言 `CheckIP` **真的拒绝**（修前必红）。 |
| **建议的工程级措施** | 把 `CS0108`/`CS0114` 从 `src/**` 的 `NoWarn` 移除，或加一条"禁止隐藏基类成员"的门禁 —— 这是本工程"同名不同义 / 静默失效"家族的**第 6 个实例**（前五个见台账 §59.1、§58.2；X-P10-04 是**同一机制**的第二例：`TextReplaceDialog` 用 `new` 隐藏而非 `override`）。 |

### §6.4 ★★ 结构性结论（第二方普查交回，范围比 §58.5 更大）

> **出货的宿主进程根本不驱动那些 1:1 移植的平面。**

| 宿主 | 实际跑的是 | **从不触碰**的 1:1 平面 |
|---|---|---|
| `GXX.RunGate/Program.cs:18-19` | `GateService` 骨架 | `GateShareSeam` / `MirClientContext` |
| `GXX.SelGate/Program.cs:18-19` | `GateService` 骨架 | `CSelSessionObj` / `CSelGateIPFilter` / `SelGateGlobals` |
| `GXX.LoginSrv/Program.cs:18-19` | `LoginSrvService` 包在 `GateMainForm` 里 | `MasSock` / `frmGateSet` / `LoginSrvShare` / `RoleDBSeam` |
| `GXX.LogDataServer/Program.cs:18-19` | `LogDataService` | `TFrmLogManage` / `Pool\FileSearchPool` |
| `GXX.GameCenter/Program.cs:15-20` | Mutex + 新建 `FrmMain` | `CheckPrevious` / `GMainHelpers` 面 |
| `GXX.LoginGate/Program.cs:17` | `new LoginGateService()`（**options = null**） | 整个 Rest11 面（§6.1） |
| **`GXX.M2Server/Program.cs:21`** | **`M2EngineService`（真主循环）** | 表 B 的 523 条接缝 |

⇒ **"未接"在 RunGate/SelGate/LoginSrv/GameCenter/LogDataServer 里，多数不是"忘了一根线"，
而是"宿主根本不存在"。** 这把 §58.5 的裁定**从 M2Server 扩到 7 个 exe**：
**D-P17-03 应从"建一个 M2Server HostWire"升级为"给 7 个 exe 各定一个宿主装配点"。**

**两条新机制（本车道登记，都能"把缺口伪装成别的东西"）：**

| ID | 机制 | 实例 |
|---|---|---|
| **X-P17-04** | **"b 显式抛"被上层 `catch` + 静默默认值吞成 "a 静默"** | `RoleDbSeam.MainOutMessage = _ => { }`（`DBServer/MySqlRoleDB.Seam.cs:175`）是 `THumanDBBase`/`THeroDBBase` **全部 wrapper 的唯一出口** ⇒ 已接线的 `SelectClientHumanDb/HeroDb` 适配器里 **23 个故意抛 `Unwired` 的成员**全部退化为静默中性返回（导出空文件、静默查不到角色） |
| **X-P17-05** | **`Reset*` / `ResetForTests` 不是接线**（只是把默认值再写一遍） | `GXX.Client` 全树 grep 赋值 ⇒ **恰好只有 1 处真接线**（`DxControlOps.CanMoveSink`，`DxComponent/DxImageForm.cs:835`）；其余全是"再断言同一个默认值" |

**E 区 Top 3 危险条目**（完整 Top 10 见工单表 §4.4.3）：
① 上表 `RoleDbSeam.MainOutMessage`；② `GateShareSeam.AddMainLogMsgSink`/`AddBlockIPSink`/`AddTempBlockIPSink`
（`RunGate/MirClientContextSeams.cs:705/710/714`，`≈80` 个日志点与**全部封禁动作**进入空 lambda，
而**兄弟平面** `RunGateConfigLoader.LogSink:105` 是接了的 ⇒ 镜像面对外报告"已封禁"却什么都没封）；
③ `CSelSessionObj.KickUser`+`.SendRaw`（`SelGate/SelGateSession.cs:70/64`，反 CC/`$` 攻击只记日志不踢人，
且客户端**什么都收不到**）。

### §6.5 X-P17-02：宿主接缝未被静态隔离机制覆盖

`tests/GXX.M2Server.Tests/M2ConfigIsolationCoverage.cs`（**不在本车道分区**）的类型清单里
**没有** `M2EngineService`。本车道的两个接缝因此**刻意做成实例字段**（见 §2.1），
但**建议**下次有人改这份清单时把宿主类型纳入（一行），取得"不靠自觉"的双保险
（与 §51.3 X-P9-01 同源请求）。

### §6.3 X-P17-03：`Directory.Build.props` 的 `NoWarn` 含 `CS0108;CS0114`

见 §6.1。该文件**不在本车道分区**，只登记。

---

## §7 偏离登记（D-P17-xx）

| ID | 偏离 | 依据 / 影响 | 恢复途径 |
|---|---|---|---|
| **D-P17-01** | **修掉了宿主的入站字段读法**（`Recog` → `Param`，3 行），并以原文三站点 + 自我一致性证明支撑 | 原实现**自相矛盾**且**静默**（走错方向不抛不打日志）。这是本车道**唯一**对既有默认行为的修改 | 若将来证实 wire 上 `Recog` 确为方向载体（需 wire 打包映射取证），把 3 行改回即可；两条回归守卫会立刻告诉你 |
| **D-P17-02** | `CM_WALK` 仍只读一个方向字节，**未**移植原文 `ClientWalk`（`ObjPlayer.pas:17464`，1,100+ 行：x/y 反解方向 + `boSpeedControl` + `CanParaly` + `CheckActionStatus` + 延时投递 + `m_nMoveCount` 限流） | 缺 `TProcessMessage ↔ TDefaultMessage` 的 **wire 打包映射**取证（x/y 落在 `Tag`/`Series` 还是 `Param`/`Tag` 未定）⇒ **不猜、不发明** | 取证后按 1:1 落 `ClientWalk`；`OnGateClientData` 的 `CM_WALK` 分支应整体被它取代 |
| **D-P17-03** | 本车道**没有**建 HostWire / CompositionRoot（523 条接缝无落点） | 分区只有 `Program.cs`；且"造一个宿主接线层"是**架构决策**，超出单车道 | 建议**单开一条车道**：以 `M2EngineService` 为根装配表 B 的 15 条高危接缝（工单表 §3.2 已排好序） |
| **D-P17-04** | 两个新接缝是**实例字段**而非 `static`（与全工程"静态接缝 + `ResetDefaults()`"惯例不同） | ① `M2ConfigIsolationCoverage.cs` 不在本分区 ⇒ 无法把 `M2EngineService` 纳入静态隔离；② `static` 会让"忘记置回"污染后续用例 | 若 §6.2 的纳纲被采纳，可改回 `static` 并加 `ResetDefaults()`（**但那时才安全**） |
| **D-P17-05** | `CM_QUERYUSERNAME` **未 1:1**，且**不假装已接** | 原文要 `CretInNearXY`/`GetCharColor`/`GetShowName`/`SendSocket` 四个面，而它们**各自也是未接接缝**（§3.2 #1/#6） | 见 D-P17-03；四个面各自接线后才可 1:1 |
| **D-P17-06** | 端到端用例用**经典（非 EN）格式**合成 `.map`，与既有 `M2ServerTests.CreateTestMap`（private，EN 格式）**不共用** | 既有 helper 是 `private`，且本车道刻意覆盖 `Envir.cs:122-128` 的**另一条加载分支** | 若集成方把 helper 提升为 internal/public，可合并（测试夹具层，不影响产品） |

**未做任何越区编辑**；**未修改任何 `*.csproj` / `GXX.slnx` / `Directory.Build.props` /
`docs/Checklist.md` / `docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**`**。

---

## §8 未完成 / 阻塞

| # | 项 | 性质 | 卡在哪 |
|---|---|---|---|
| 1 | **表 B 的 523 条接缝无落点** | **架构缺口**（本车道最大的未完成） | D-P17-03：缺 HostWire；见 §6.1 之外的"接线的地方" |
| 2 | **X-P17-01（Rest11 opt-in 失效）** | **确认的缺陷** | 跨分区（`GateService.cs` / `LoginGateService.cs`）+ 需裁定公开 API |
| 3 | `CM_WALK` 1:1 化 | 未知待证 | D-P17-02：wire 打包映射取证 |
| 4 | `CM_QUERYUSERNAME` 1:1 化 | 依赖缺口 | D-P17-05：四个上游面未接 |
| 5 | `TMonster.Run` 1:1 化 | 依赖缺口 | `p16-m2-tmonster-run` **未并入 main** ⇒ 本链路用的是 18 行近似物 |
| 6 | `TPlayObject.Run` 1:1 化 | 依赖缺口 | 原文 `ObjPlayer.pas:3772-5606`（1,835 行）未落地 ⇒ `Process()` 走继承的 `TCreature.Run` |
| 7 | E 区 321 条接缝（RunGate/SelGate/GameCenter/LogDataServer/LoginSrv/DBServer/Client） | **第二方普查已交回，但本车道未全量复核** | 本车道时间/分区；已单列（§5.1）并要求下一个接线车道按 §59.2-1 **先核对 main 再采信** |
| 8 | `GXX.M2Server` 的 **4,680 条裸 `=> true;`** | 未处理（**不在本车道任务范围**） | 台账 §58.2/§49.3 已登记；本车道只把它数字化（§1.2 命令 4） |

---

## §9 门禁证据（`tools/run-gate.ps1`，严格判据）

```
命令：powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 `
        -Project GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj
（cwd = D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-m2-hostwire）
```

```
build exit code       : 0
== gate evidence ==
dotnet test exit code : 0
crash markers found   : none
GATE: PASS (build 0 error, test exit 0, no crash markers)
```

`dotnet test` 摘要行：`已通过! - 失败: 0，通过: 10711，已跳过: 37，总计: 10748`（27–30 s）
—— 与台账 §59.8 的 `M2Server 10,696（+37 ticket skip）` 相比 **+15**，
**恰好等于本车道新增用例数**（9 + 6）。

**提交后已复跑一次**（`defe1fa4` 之后），四行取证完全一致 ⇒ 门禁可复现。

> ⚠ 按 §52.3 的规程，本报告**不以摘要行**为判据；上表四行由 `run-gate.ps1` 打印
> （`build exit` + `dotnet test exit` + `crash markers` + `GATE:`）。
> 本车道**没有**只贴摘要行，也没有在 testhost 崩溃的情况下误报"通过"。

---

## §10 可复跑命令（本报告所有数字的来源）

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-m2-hostwire

# 门禁
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 `
  -Project GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj

# 只跑本车道的 15 例
dotnet test GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj `
  --filter "FullyQualifiedName~HostWire"

# §5.2 / §0 的关键计数（完整 6 条见 docs/接线工单表.md §1.2）
"src_provider   = " + (Get-ChildItem GXX.CSharp\src   -Recurse -File -Filter *.cs | % { (Select-String $_.FullName 'Seams\.\w+\s*=[^=]' -AllMatches).Count } | Measure-Object -Sum).Sum
"tests_provider = " + (Get-ChildItem GXX.CSharp\tests -Recurse -File -Filter *.cs | % { (Select-String $_.FullName 'Seams\.\w+\s*=[^=]' -AllMatches).Count } | Measure-Object -Sum).Sum

# X-P17-01 的一行判据
Select-String GXX.CSharp\src\GXX.GatewayKit\GateService.cs     'Rest11Options'
Select-String GXX.CSharp\src\GXX.LoginGate\LoginGateService.cs 'Rest11Options'
Select-String GXX.CSharp\Directory.Build.props                 'NoWarn'   # 含 CS0108;CS0114
```

---

## §11 边界声明

- 所有写文件调用使用**绝对路径**；提交前已跑 `git -C <工作树> status --porcelain` 复核。
- **未在主工作树执行任何 git 写命令**；未执行 `merge`/`rebase`/`checkout`/`switch`/`push`/`worktree`/`reset`。
- 未修改 `GXX.slnx` / 任何 `*.csproj` / `Directory.Build.props` / `docs/Checklist.md` /
  `docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**`。
- 含中文文件一律用 read / edit / write 工具操作，**未做任何 shell 文本往返**（§41.6）。
- 本车道未使用裸 `=> true;`；`Program.cs` 中新增的每一处未 1:1 之处都在注释里写明原文行号与缺口。
