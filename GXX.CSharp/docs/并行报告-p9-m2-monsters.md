# 并行报告 —— 车道 `p9-m2-monsters`

> 分支：`par/p9-m2-monsters` ｜ 基于 `main @ b48e4c43` ｜ 工作树：`.worktrees/p9-m2-monsters`
> 独占分区：`GXX.CSharp/src/GXX.M2Server/Sweep9/Monsters/**`、`GXX.CSharp/tests/GXX.M2Server.Tests/Sweep9Monsters*.cs`、本文件
> 源：`_analysis/utf8_mirror/M2Engine/{ObjDummy,ObjRobot,ObjFireDragon}.pas`（**只读镜像**，未读 GBK 原文）

---

## 0. 一句话结论

| 单元 | 行数 | 例程 | 已移植 | 形态 |
|---|---|---|---|---|
| `ObjRobot.pas` | 500 | **20** | **20/20** | **完整类移植**（真继承 `TPlayObject`，`Run` 真 `override`） |
| `ObjFireDragon.pas` | 531 | **14** | **14/14** | 逐条判定逻辑移植（类外壳受 **B-P9-01** 阻塞） |
| `ObjDummy.pas` | 1245 | **19** | **19/19** | 逐条判定逻辑移植（类外壳受 **B-P9-02** 阻塞） |
| **合计** | **2276** | **53** | **53/53** | —— |

**每一个过程 / 函数都已逐个登记（名称 + 起止行 + 计数对账），并把它的判定逻辑逐支实现为可执行、可断言的托管代码。**

---

## 1. 交付物

| 文件 | 内容 |
|---|---|
| `src/GXX.M2Server/Sweep9/Monsters/ObjRobotCore.cs` | `TRobotObject` / `TRobotManage` **完整类移植**（20 方法真实现）+ `ObjRobotCore`（清单/常量/差异断言） |
| `src/GXX.M2Server/Sweep9/Monsters/ObjRobotHostSeams.cs` | `partial class TCreature` 的**补成员** `m_boSuperMan`（ObjBase.pas:134；已 `git grep` 确认 main 缺失） |
| `src/GXX.M2Server/Sweep9/Monsters/ObjFireDragonCore.cs` | `TFireDragon` / `TFireDragonGuard` 的 14 例程判定逻辑 + 接缝 + 差异断言 |
| `src/GXX.M2Server/Sweep9/Monsters/ObjDummyCore.cs` | `TDummyObject` 的 19 例程判定逻辑 + 接缝 + 差异断言 |
| `tests/GXX.M2Server.Tests/Sweep9MonstersTestKit.cs` | 测试环境（内存文件系统 + 可控时钟 + 记录型接缝） |
| `tests/GXX.M2Server.Tests/Sweep9MonstersObjRobotTests.cs` | 84 例 |
| `tests/GXX.M2Server.Tests/Sweep9MonstersObjFireDragonTests.cs` | 64 例 |
| `tests/GXX.M2Server.Tests/Sweep9MonstersObjDummyTests.cs` | 123 例 |

**新增用例：271 例**（全部落在 `Sweep9Monsters*.cs` 三个文件内）。

---

## 2. 逐单元「已移植 / 总方法数」

### 2.1 `ObjRobot.pas`（**20/20 完整实现**）

| # | 方法 | 行段 | 状态 |
|---|---|---|---|
| 1 | `TRobotObject.AutoRun` | 94-163 | ✅ 真实现 |
| 2 | `TRobotObject.AutoRunOfOnDay` | 165-199 | ✅ 真实现 |
| 3 | `TRobotObject.AutoRunOfOnHour` | 201-204 | ✅ 真实现（空体，原文如此） |
| 4 | `TRobotObject.AutoRunOfOnMin` | 206-209 | ✅ 真实现（空体） |
| 5 | `TRobotObject.AutoRunOfOnSec` | 211-214 | ✅ 真实现（空体） |
| 6 | `TRobotObject.AutoRunOfOnWeek` | 216-253 | ✅ 真实现 |
| 7 | `TRobotObject.ClearScript` | 255-264 | ✅ 真实现 |
| 8 | `TRobotObject.Create` | 266-272 | ✅ 真实现 |
| 9 | `TRobotObject.Destroy` | 274-279 | ✅ 真实现（+ `Dispose` 等价物） |
| 10 | `TRobotObject.LoadScript` | 281-354 | ✅ 真实现 |
| 11 | `TRobotObject.ProcessAutoRun` | 356-366 | ✅ 真实现 |
| 12 | `TRobotObject.ReloadScript` | 368-372 | ✅ 真实现 |
| 13 | `TRobotObject.Run` | 374-378 | ✅ 真实现（**真 `override`**） |
| 14 | `TRobotObject.SendSocket` | 380-383 | ✅ 真实现（空体）+ **D-P9-02** |
| 15 | `TRobotManage.Create` | 387-392 | ✅ 真实现 |
| 16 | `TRobotManage.Destroy` | 394-399 | ✅ 真实现 |
| 17 | `TRobotManage.LoadRobot` | 401-446 | ✅ 真实现 |
| 18 | `TRobotManage.RELOADROBOT` | 448-452 | ✅ 真实现 |
| 19 | `TRobotManage.Run` | 454-479 | ✅ 真实现 |
| 20 | `TRobotManage.UnLoadRobot` | 481-497 | ✅ 真实现 |

**计数对账（§37.3）**：原文 `^\s*(procedure|function|constructor|destructor)` 命中 **40** = 20 条接口声明 + 20 条实现体 ⇒ **20 个例程**。
实现侧 `ObjRobotCore.DeclCount = ImplCount = Methods.Length = 20`，且 `Methods` 里每个名字都能在 `TRobotObject`/`TRobotManage` 上**反射找到**（`Create` 映射到构造器）—— 由 `MethodInventory_AllNamesExistInImplementation` 断言。

**保留的虚分派**：`TRobotObject : Engine.TPlayObject`（原文 `class(TPlayObject)`），`Run` 是真 `override`（§18.8）。
唯一形式偏差见 **D-P9-02**。

### 2.2 `ObjFireDragon.pas`（**14/14**）

| # | 例程 | 行段 | 类 |
|---|---|---|---|
| 1 | `Create` | 51-62 | `TFireDragon` |
| 2 | `Destroy` | 64-67 | `TFireDragon` |
| 3 | `CheckAttackTarget` | 69-103 | `TFireDragon` |
| 4 | `MagBigExplosion` | **139-173** | `TFireDragon` |
| — | *（旧版 `MagBigExplosion`，`{ }` 注释）* | *106-137* | *不移植（F12）* |
| 5 | `RecalcAbilitys` | 175-182 | `TFireDragon` |
| 6 | `AttackTarget` | 184-325 | `TFireDragon` |
| 7 | `Run` | 327-377 | `TFireDragon` |
| 8 | `Create` | 381-393 | `TFireDragonGuard` |
| 9 | `Destroy` | 395-398 | `TFireDragonGuard` |
| 10 | `MagBigExplosion` | 400-435 | `TFireDragonGuard` |
| 11 | `RecalcAbilitys` | 437-445 | `TFireDragonGuard` |
| 12 | `AttackTarget` | 447-495 | `TFireDragonGuard` |
| 13 | `IsChar`（**嵌套**） | 449-459 | `TFireDragonGuard.AttackTarget` |
| 14 | `Run` | 497-528 | `TFireDragonGuard` |

**计数对账**：原文关键字命中 **27** = 13 条接口声明 + 13 条实现体 + 1 条嵌套 `IsChar` ⇒ **14 个例程**。
`ObjFireDragonCore.Methods.Length = 13`（接口层），`RoutineCount = 14`（含嵌套）。

**类关系逐层核实**（三处易错点，均已用断言固化）

1. `TFireDragon` 的 `AttackTarget` 是**三级 override 链**：
   `TStickMonster.AttackTarget`（**虚根**，`ObjMon2.pas:16 virtual`）→ `TCentipedeKingMonster`（`ObjMon2.pas:42 override`）→ `TFireDragon`（`:27 override`）。
2. `TFireDragonGuard.AttackTarget`（`:43`）**没有 `override`** —— 因为 `TAnimalObject`（`ObjBase.pas:817`）分支上**根本没有** `AttackTarget`（该分支成员表里出现 **0** 次；`ObjBase.pas:1301` 的那个是**另一个签名**）。所以它是**普通新方法**，**不隐藏**任何基类虚方法 ⇒ 托管侧**不得**加 `override`。
3. `m_dwAttickTick` 的归属是 **`TCentipedeKingMonster`**（`ObjMon2.pas:36`，`0x560`），不是 `TFireDragon`；`TFireDragon` 自己只声明了 `m_dwLightTick`。

### 2.3 `ObjDummy.pas`（**19/19**）

| # | 例程 | 行段 | 层级 |
|---|---|---|---|
| 1 | `TDummyObject.Create` | 60-88 | 方法 |
| 2 | `TDummyObject.Destroy` | 90-93 | 方法 |
| 3 | `TDummyObject.OnSpaceMove` | 95-105 | 方法 |
| 4 | `TDummyObject.StartPickUpItem` | 107-200 | 方法 |
| 5 | `TDummyObject.Start` | 202-216 | 方法 |
| 6 | `TDummyObject.Stop` | 218-237 | 方法 |
| 7 | `TDummyObject.Ask` | 239-249 | 方法 |
| 8 | `TDummyObject.Initialize` | 251-277 | 方法 |
| 9 | `TDummyObject.IsProperTarget` | 279-302 | 方法 |
| 10 | `NextDirClockwise` | 304-325 | **单元级函数** |
| 11 | `NextDirAntiClockwise` | 327-348 | **单元级函数** |
| 12 | `TDummyObject.GotoPath` | 350-404 | 方法 |
| 13 | `TDummyObject.WalkToNext` | 406-410 | 方法 |
| 14 | `TDummyObject.RunToNext` | 412-419 | 方法 |
| 15 | `TDummyObject.Wondering` | 421-560 | 方法 |
| 16 | `GetPoint`（**嵌套于 `Wondering`**） | 423-479 | 嵌套函数 |
| 17 | `TDummyObject.Run` | 562-1080 | 方法 |
| 18 | `TDummyObject.Walk` | 1083-1104 | 方法 |
| 19 | `TDummyObject.CanAutoUseMagic` | 1106-1242 | 方法（private） |

**计数对账**：原文关键字命中 **35** = 16 条接口声明 + 16 条实现体 + 2 条单元级函数 + 1 条嵌套函数 ⇒ **19 个例程**。
`ObjDummyCore.Methods.Length = 19`，且已断言 `GetPoint` 的行段**包含在** `Wondering` 内、两个方向函数**没有** `TDummyObject.` 前缀（单元级而非类成员）。

---

## 3. 原文缺陷清单（逐条：因何成立 + 计数取证 + 差异断言）

> 处置口径一律为**照抄 + `// 原文如此` + 差异断言锁死**，不顺手修（任务书第 2 条）。

### 3.1 `ObjRobot.pas`（F1–F10）

| 编号 | 缺陷 | 计数取证 | 断言 |
|---|---|---|---|
| **F1** | `AutoRun` 的外层节流是**死条件**：`dwRunTimeLen` 唯一写入点 `:319` 写的是 `0`，全文再无其它写入 | 出现 **3** 次 = `:36` 声明 + `:100` 读 + `:319` 写 | `RunTimeLenIsAlwaysZero`、`RunTimeLenCountAddsUp`、`PassesOuterThrottle(1001,1000,0)==True` |
| **F2** | `case nRunCmd` 的 `1:`/`2:`/`3:` 是**空语句**且无 `else` | `nRunCmd` 赋值点 **1**（`:321`）、case 标签 **4**、空标签 **3** | `RunCmdHasUnreachableLabels`、`AutoRun_EmptyRunCmdLabels_DoNothing`（Theory 3 例） |
| **F3** | `New(AutoRunInfo)` **不置零** + 9 个 `CompareText` 是**并列 if**（无 else）⇒ `sMoethod` 全不匹配时 `nMoethod` 留**堆残留值**，随后 `case` 无 `else` ⇒ **静默失效**。另 `nParam2/3/4` **只声明、0 读 0 写** | `nMoethod` 赋值 **9** / case 标签 **9**；`nParam2..4` 各 **1**（仅声明） | `MoethodNoDefault`、`Flaw3_UnusedParamFields`、`LoadScript_UnknownMethod_LeavesMoethodAtHostDefault` |
| **F4** | `sLabel` **赋值后从不使用**（两处死存储） | 出现 **4** = 2 声明 + 2 赋值，**读取 0** | `SLabelIsDeadStore` |
| **F5** | 同一单元内**两套切分 API**：`OnDay` 用 `GetValidStr3_Ex`（单字符）、`OnWeek` 用 `GetValidStr3`（集合） | 调用点各 **1** | 记录在案（语义等价，不改） |
| **F6** | 时分区间上界是**越界值** `[0..24]` / `[0..60]` | `HourUpperBound=24`、`MinuteUpperBound=60`、`WeekUpperBound=7` | `TimeUpperBoundsAreOffByOne`、`AutoRunOfOnDay_Hour24_PassesRangeGuard`（24 点**可达**：用 `DecodeTime` 接缝造出 24 并观察到真的触发） |
| **F7** | `nRUNONHOUR/OnMin/OnSec` 三个标签**接了线但三个方法体完全为空** | 空过程体 **3** ↔ 未实现标签 **3** | `EmptyBodiesMatchTags`、`AutoRun_UnimplementedTagMethods_DoNothing`（Theory 3 例） |
| **F8** | `TRobotObject.Run` 的 `// inherited;` 被注释 ⇒ **不调基类** `TPlayObject.Run` | `inherited` 在本方法内 **0** | `ProcessAutoRun_And_Run_DriveEachEntry`（只走 `ProcessAutoRun`） |
| **F9** | `SendSocket` 是**空 override** ⇒ 机器人吞掉一切发包 | 参数 **2**、正文 **0** 行 | `SendSocket_IsNoOp` + **D-P9-02** |
| **F10** | `LoadRobot` 的 `if RobotHuman.m_sCharName = '' then m_sCharName := 'TRobotObject'` 是**死分支**（上一行刚赋成非空 `sRobotName`，且 `:426` 已要求非空） | 非空守卫 **1** + 判空 **1**，同一嵌套层内 | `DeadBranchIsProvable`、`LoadRobot_EmptyNameLine_RejectedBeforeDeadBranch` |

### 3.2 `ObjFireDragon.pas`（F1–F12）

| 编号 | 缺陷 | 计数取证 | 断言 |
|---|---|---|---|
| **F1** | **多线程读锁整段被编译掉**：`{$IF MULTI_THREAD = 1}` 的判定源是 `M2Share.pas:75 MULTI_THREAD = 0`（const 段）⇒ `:198-202`/`:239-244` 两块（含 `try`/`finally`/`LockR(3)`/`UnLockR`）**不参与编译** ⇒ 点灯循环访问 `UserEngine.m_MonObjectList` **完全没有读锁** | `{$IF}` **2** / `{$IFEND}` **2**（一一配对）、`MULTI_THREAD` 常量声明 **1** | `Flaw1_MultiThreadLockBlockIsCompiledOut`（沿用 `EnvirObjectQueryCore.cs:171-173` 的同一裁定：「单线程编译时整段消失」，不另立标准） |
| **F2** | 点灯循环对候选对象做**零类型判别的硬转换** `TFireDragonGuard(BaseObject)`，并读其 `m_boLight`/`m_boAttick`；判据**只有"同地图"一条** ⇒ **火龙自己**（同地图）也会被当成守护兽读 | 循环体内类型/种族判别 **0**、入口判据 **1** | `Flaw2_GuardLoopHasNoTypeCheck` |
| **F3** | `:205 Randomize;` **在 `AttackTarget` 内每次点灯重新播种** ⇒ 紧随的 `K := Random(6)` 退化为按系统时钟取值 | `Randomize` 全单元 **1**（`:205`） | `Flaw3_RandomizeInsideAttackTarget` |
| **F4** | `MagBigExplosion` **无 nil 保护**解引用 `m_TargetCret` | —— | `MagBigExplosion_AssumesTargetNotNull`（靠调用方 `:309` 保证） |
| **F5** | `AttackTarget` 的 `Result := True` **无条件成立** ⇒ 调用方 `:346` 的 `else`（清 `m_VisibleActors`）**只在返回 False 时**走到 ⇒ 命中时**不清**视野列表、不刷新 `m_dwAttickTick` | —— | `Flaw5_PurgeOnlyWhenAttackFails` |
| **F6** | `for I := 0 to IsChar(s_AttickXY)` —— `IsChar` 数 `'\|'` 个数 ⇒ 循环次数 = 竖线数+1 = **段数**（**看似 off-by-one、实为正确**） | 4 组样例（空/单段/2 段/3 段） | `Flaw6_LoopCountEqualsSegmentCount` |
| **F7** | ★★ **判据不一致**：火圈入口守卫是 `Pos('|', s_AttickXY) > 0`（**要求含竖线**），而 `0..IsChar` 循环**单段也能正确解析** ⇒ **单坐标配置**（`"100,200"`）**一次火圈都不放** | `GuardExplosionGated("100,200")==False` 但 `GuardSegments==1` | `SingleCoordinateConfigNeverFires` |
| **F8** | `TFireDragonGuard.AttackTarget` 也**无条件 `Result := True`**（只要节流放行）⇒ 与 F7 叠加：**单坐标下照样清掉 `m_boAttick`** ⇒ 该守护兽**永久失去攻击机会** | —— | `SingleCoordinateStillClearsAttick` |
| **F9** | `TFireDragonGuard.m_dwLightTick`（`:33`）是**死字段**：Guard 的方法体里 **0 读 0 写** | `m_dwLightTick` 全单元 **5** —— `:19`(龙声明) `:61`(龙写) `:195`(龙读) `:197`(龙写) `:33`(**守护兽声明**) | `Flaw9_GuardLightTickIsDeadField` |
| **F10** | 清视野列表路径**逐个 `Dispose(VisibleBaseObject)`**（共享的 `pTVisibleBaseObject` 记录）后再 `Clear` ⇒ 其它持有者拿到野指针 | `Dispose(` 全单元 **1**（`:360`） | `Flaw10_SingleDisposeOnSharedRecord` |
| **F11** | 两处 `Run` 的**异常覆盖面不对称**：`TFireDragon.Run` 的收尾 `inherited` 在 `except` **之外**（`:376`），`TFireDragonGuard.Run` 的在 `try` **之内**（`:524`）⇒ 守护兽会**吞掉基类 `Run` 的异常** | `inherited` 全单元 **9**；外 **1** / 内 **1** | `Flaw11_InheritedAsymmetry`（含两条行号常量） |
| **F12** | `TFireDragon.MagBigExplosion` **出现两次**（`:106-137` 是 `{ }` 注释的旧版、`:139-173` 才是生效版）⇒ **按"第一次出现"做纯文本检索会移植错**（与 `ObjMonCore.cs` 记的 `TMonster.Run` 双份结构同类，区别是本处用 `{ }`） | 注释起/终点各 **1**；`CommentedOldExplosionEnd < live.Start` | `Flaw12_OldExplosionIsCommentedOut` |

### 3.3 `ObjDummy.pas`（F1–F15）

| 编号 | 缺陷 | 计数取证 | 断言 |
|---|---|---|---|
| **F1** | ★ `GotoPath` 的**整个函数体被 `(* *)` 注释掉**（`(*` `:355`、`*)` `:403`）⇒ 生效代码只有 `:354 Result := False;` **一行**，**永远返回 False** | `GotoPath` 内 `(*`/`*)` 各 **1**；活语句 **1**；`Result := True` **2**（都在注释内） | `GotoPathIsEntirelyCommentedOut` |
| **F1b** | 与 F1 互为印证：`Wondering` 里**唯一**的 `GotoPath()` 调用点（`:513`）也在 `{ }` 注释内；`Run` 里的旧版 `(* *)` 块 `:520-541` 亦为废弃实现 | 调用点 **1**（注释内）；旧块 `:520`/`:541` | `Wondering` 判定里**没有** `GotoPath` 分支（`WonderingOutcome` 无对应枚举值） |
| **F2** | 三个**死字段**：`m_NotCanPickItemList`（`:14`）、`m_dwStartPickItemTick`（`:15`）各**只出现 1 次**（仅声明）；`m_dwAskTick`（`:20`/`:73`）**写后从不读** | 1 / 1 / 2，`AskTickReadSites = 0` | `Flaw2_DeadAndWriteOnlyFields` |
| **F3** | `NextDirClockwise`/`NextDirAntiClockwise` 是**单元级函数**（纯、无实例状态），且 `case` **无 `else`** ⇒ 方向 &gt; 7 一律返回 `DR_UP` | case 标签 **8** / `else` **0**；0..255 全枚举 | `DirectionFunctionsAreUnitLevel`、`OutOfRangeDirectionBecomeUp`、`ClockwiseAntiClockwiseAreInverses` |
| **F4** | `GetPoint` 的 `nIndex` **只夹下限**（越界含上界越界一律退化为 0）；外层旧 `for` 骨架被注释 | 注释 `for` **1** | `CheckIndexClampsToZero`（`-1`/`3`/`99` → 0）、`ClampCheckStepIndex_OnlyLowerBoundIsHonored` |
| **F5** | `GetPoint` 每次调用**随机**选顺时针/逆时针（用函数变量 `GetNextDir` 做动态分派） | 赋值点 **2** | `ChooseNextDirStrategy_FollowsRandomTwo`（两策略在 `DR_UP` 上给出 1 vs 7） |
| **F6** | ★★ **战士假人的 `btDir` 可以变成 -1**：`btDir := (btDir - 1) mod 8`（`:800`）而 `btDir` 是 **`Integer`**（`:566` var 段），Delphi `mod` 取**被除数符号** ⇒ `btDir = 0` 时得 **-1**，随后原样传给 `GetNextPosition(..., btDir, ...)` | `-1` 侧 **1** / `+1` 侧 **1** / `+nCount` 侧 **1** | `MinusOneDirectionIsReachable`（并排除 7 与 255 两种"另一种读法"的值）、`PlusOneDirectionIsAlwaysValid`、`PlusCountDirectionIsAlwaysValid` |
| **F7** | ★★ `AttackTime := Max(0, AttackTime - (g_Config.dwIncSpeedDecInterval * m_nHitSpeed))` 是**混合符号类型 + 重载不确定**表达式：`Integer - (LongWord * Integer)`，而 `Math.Max` **没有无符号重载** ⇒ "夹 0" 还是 "回绕成 ~4.29e9（⇒ 永不攻击）" **从源码字面无法判定**。托管侧取**有符号 + 夹 0**（与作者注释"防止负数出错"一致、且是唯一不破坏玩法的读法） | 出现 **3**（`:826/:872/:972`） | `AttackTimeIsClampedNotWrapped`、`UnsignedReadingWouldNeverAttack`（对照读法回绕 = `4294965496`）→ **D-P9-03** |
| **F8** | ★★ **自动练功代码块出现两次**（`:1035-1052` 与 `:1054-1071`，函数体**逐字相同**），且第二处外层守卫**丢掉了 `m_boStart` 与 `CanMove`** ⇒ ① 假人**已 Stop** 时自动练功**照样跑**；② `CanMove = False` 时也照样跑 | 四合一条件串 **2** 处；第二处缺 **2** 个判据 | `AutoMagicRunsEvenWhenStoppedOrCannotMove`、`SecondGuardOmitsStartAndCanMove`、`SecondGuardMissingConditions==2` |
| **F9** | ★★ **英雄合击的 `m_MyHero.m_TargetCret` 无 nil 保护**：`boHeroJointAttackFly` 为真时直接解引用英雄的目标 | 解引用 **6** 次（3 职业分支 × 2 行）、nil 守卫 **0** | `HeroJointAttackDerefsTargetWithoutNilCheck` |
| **F10** | **`THeroObject(m_MyHero)` 无类型校验的硬转换**（与 `ObjFireDragon` 的 F2 同类） | 硬转换 **21**、类型判别 **0** | `HeroObjectCastHasNoTypeCheck` |
| **F11** | 三个职业分支的**收尾结构不对齐**：法师用 `if … else if`（走位 / 随机找**自己的火墙**站上去），道士**没有火墙分支**且把两个距离判据用 `and` 并成一段 ⇒ **法师会去找火墙、道士不会** | 火墙分支 **1**；走位判据写法 **2 种** | `FireWallHuntOnlyForWizard`、`MovementConditionStylesDiffer`、`TaoistMovePhase_HasNoFireWallBranch` |
| **F12** | 道士"没事找事跑"的抑制条件是 `… and (Random(4) <> 0)` ⇒ **1/4 的概率仍然会跑**（不是"完全不跑"） | —— | `TaoistStillRunsOneInFour`（`Random(4)=0` 不抑制） |
| **F13** | `IsProperTarget` 里同一"主人"判据**用了两种读法**：`(BaseObject.m_Master = Self) or (BaseObject.Master = Self)`（字段 + 属性 `ObjBase.pas:806 read GetMaster`） | 并存读法 **2** | `MasterVetoedByEitherReadForm` ⚠ **待确认项 C-P9-01**：`GetMaster` 实现是否等价于 `m_Master`（其实现体不属本单元，**未做断定**） |
| **F14** | 第二处自动练功块把 `m_TargetCret` 临时置 `Self` 再置 `nil`，**结束后不恢复原目标** | —— | `AutoUseMagicDue_Boundary` + `AutoMagicBlockIsDuplicated` |
| **F15** | 写 `m_SkillUseTick` 前**有**上界守卫（`:1045/:1064`），而 `CanAutoUseMagic` 里对它的**读取全无守卫** | 读 **8**、守卫 **0** | `SkillUseTickReadsHaveNoGuard`、`SkillUseTickWriteAllowed(1000)==True / (1001)==False` |

**另记（`CanAutoUseMagic` 的 14 分支结构性发现）**

* `SKILL_75`（护体神盾）分支**永远返回 False**（唯一赋值 `:1121` 就是 False）⇒ 自动练功对 75 号技能从不触发攻击。
* 比较运算符**不统一**：`>=` 6 个（75/56/26/42/66/113）、`>` 3 个（115/114/39）⇒ 在"恰好等于 CD"时结果不同（已用 `ExactCooldownDiffersBetweenBranches` 给出可执行证据）。
* `SKILL_MOOTEBO` 用**字面量** `1000 * 10` 而非 `GetMagicCD`（共 **9** 个 `GetMagicCD` 调用点，独缺它）。
* **4 个分支完全不做时间判定**（40 / 25 / 自定义区间 / `else` 兜底）⇒ `else Result := True` 意味着**任意未列举技能 ID 一律"允许使用"**。
* `m_SkillUseTick` 的下标**常量与字面量混用**（常量：`SKILL_56`/`SKILL_MOOTEBO`/`SKILL_114`；字面量：26/42/66/113/115/39）。

---

## 4. 语义偏离登记（D-P9-xx）

| 编号 | 位置 | 偏离点 | 原文行为 | 托管行为 | 为什么必须偏离 / 恢复途径 |
|---|---|---|---|---|---|
| **D-P9-01** | `ObjRobot.LoadScript`（`:317 New(AutoRunInfo)`） | **记录初始化的确定性** | `New` **不置零** ⇒ `nMoethod`/`nParam2..4` 是堆上**残留值** | `new TAutoRunInfo()` **零初始化** ⇒ `nMoethod = 0` | 托管侧无法表达"未初始化内存"（这是 .NET 的安全保证，不是可选项）。**可观测行为一致**：0 与任何残留值都匹配不到 `AutoRun` 的 9 个 `case` 标签（F3）⇒ 同样静默失效。已用 `LoadScript_UnknownMethod_LeavesMoethodAtHostDefault` 双向外加 1 例 `AutoRun_UnknownMoethod_SilentlyDoesNothing` 锁死。 |
| **D-P9-02** | `ObjRobot.SendSocket`（`:380`） | **`override` 形式不可表达** | `procedure SendSocket(...); override;`（基类 `ObjPlayer.pas:1191 virtual`） | `public virtual void SendSocket(TDefaultMessage, string)`（**无 `override`**） | 基类 `TPlayObject.SendSocket` **尚未移植**（`git grep SendSocket -- src/GXX.M2Server/Engine` = 0 命中）。为不制造"能 override 的空虚方法"（会与正式移植撞 CS0111），本批次只做**形式暂缓**（台账 §14.5 同类先例）。**恢复途径**：`ObjPlayer.pas:3526` 落地后把 `override` 加回，并把 `Flaw9_SendSocketIsEmptyOverride` 里那两行**绊线断言**改为 `Assert.Equal(typeof(TPlayObject), m.GetBaseDefinition().DeclaringType)`。 |
| **D-P9-03** | `ObjDummy` 的 `:826/:872/:972` | **混合符号类型表达式的读法** | `Max(0, Integer - (LongWord * Integer))` —— 重载不确定 | **有符号 + 夹 0**（`DummyAttackTimeSigned`） | 见 F7。注释"防止负数出错"只在**有符号**读法下才有意义；无符号读法会让 `AttackTime` 回绕成 `4294965496` ⇒ 假人**永不攻击**（明显非作者本意，却是源码在另一重载下的事实）。两种读法都作为**可执行对照**留下（`DummyAttackTimeIfUnsigned`），**未删任一**。**恢复途径**：若将来能确定 Delphi 7 在 `M2Share.pas` 的确切重载解析（或用原编译产物实测），把结论写死到 `AttackTimeIsClampedNotWrapped` 即可。 |
| **D-P9-04** | `partial class TCreature`（`ObjRobotHostSeams.cs`） | **跨区补成员** | `ObjBase.pas:134 m_boSuperMan: Boolean;` | 在**本车道新文件**里以 `partial` 补 `public bool m_boSuperMan;` | main 全树 0 命中（已 `git grep`）。本单元 `:270` 必须写它。**恢复途径**：`ObjBase.pas` 全量批次落地时**整块删除本 partial**（否则 CS0102）。 |
| **D-P9-05** | `ObjRobot.LoadScript` / `LoadRobot` 的取文件动作 | **文件读取接缝化** | `LoadList := TStringList.Create; LoadList.LoadFromFile(sFileName);` | 保留 `Create` 两步形态，只把 `LoadFromFile` 抽成 `ObjRobotSeam.LoadFromFile`（默认真读盘） | 本工程既定单测原则是**不碰磁盘**（`SweepTestKit.cs` 头注：与"数据库访问走接缝、单测不连真库"同一原则）。默认实现与原文等价，只有在测试注入时不同。 |
| **D-P9-06** | `TDummyObject.m_SayList.CustomSort(ObjLongWordSort_2)`（`:243`） | **排序比较器接缝** | 调用 `SDK.pas:179/275` 的 `ObjLongWordSort_2` | `ObjDummySortSeam.CustomSort` 委托（默认**不排序**） | 该函数**不属于本单元**，按"不顺手移植依赖"只做最小接缝。⚠ **默认是保守空实现**，未装配时 `Ask` 会返回未排序的首项 —— **已登记为待接线项 T-P9-01**。 |

---

## 5. 未完成 / 阻塞项（**如实登记**）

### B-P9-01（阻塞，非本车道可解）：`ObjFireDragon` 的类外壳
`TFireDragon : TCentipedeKingMonster`、`TFireDragonGuard : TAnimalObject` ——
托管侧 `TCentipedeKingMonster` / `TStickMonster` / `TAnimalObject` **全部未移植**
（`git grep "class TAnimalObject" / "class TCentipedeKingMonster" -- src/` = **0 命中**），
连带 `TFireBurnEvent` / `TGameEvent`（`GameEvent.pas:59`）、`UserEngine.m_MonObjectList` 的 `LockR/UnLockR`、
`GetMapBaseObjects` / `SendRefMsg` / `SendDelayMsg` / `SendAttackMsg` / `m_VisibleActors` 的真实宿主（`TCreature` 上的 `pTVisibleBaseObject`）也都不在。
**本车道未造任何替身基类**（台账 §14.2 / §18.7），只交付 14 例程的**判定逻辑 + 差异断言**。
**解锁条件**：`ObjBase.pas` / `ObjMon.pas` / `ObjMon2.pas` / `GameEvent.pas` 的 `TAnimalObject` 分支落地。

### B-P9-02（阻塞，非本车道可解）：`ObjDummy` 的类外壳
`TDummyObject : TPlayObject` —— 需要 `TPlayObject` 面约 **40+ 个成员**
（`m_SelItemObject` / `m_PickUpItemFailList` / `m_TargetCret` / `m_MovePath` / `m_nMoveIndex` /
`m_dwMoveTimeTick` / `m_dwHitTick` / `m_nHitSpeed` / `m_SkillUseTick` / `m_boFixedHideMode` /
`m_boStoneMode` / `m_boShopStall` / `m_MyHero` / `m_SayList` / `m_btAttatckMode` / `m_LastHiter` /
`StartAttack` / `ActThink` / `SelectMagic` / `GotoNext` / `DoPickUpItem` / `FindPriorityPickUpItem` /
`FindPickUpItem` / `CheckItemExists` / `GotoNextOne` / `GetMovingObjectEx` / `GotoNearGotoXY` /
`GotoNearRuntoXY` / `GetDifferenceDirection` / `AllowUseMagic` / `GetMagicCD` / `OpenSuperShiled` /
`AllowSWordHitSkill` / `AllowFireHitSkill` / `Allow42HitSkill` / `Allow66HitSkill` / `Allow113HitSkill` /
`Allow115HitSkill` / `SkillCrsOnOff` / `HalfMoonOnOff` / `HookDummyObjectRunBegin/End` …）。
其中 `m_NotCanPickItemList` 等**已确认是死字段**（F2），但其余**都是活的**。
台账 §18.7 已把 `TPlayObject` 面定性为**当前最主要的系统性瓶颈**；
本车道按该节既定判断**未自行声明任何 `TPlayObject` 替身成员**，只交付 19 例程的**判定逻辑 + 差异断言**。
**解锁条件**：`ObjPlayer.pas` 的 `TPlayObject` 面拼接批次。

### T-P9-01（待接线）：`ObjDummySortSeam.CustomSort` 的默认实现是空操作
`Ask`（`:239-249`）依赖 `SDK.pas` 的 `ObjLongWordSort_2`。本车道按"不顺手移植依赖"只做接缝，
**默认实现不排序** ⇒ 未装配时 `Ask` 返回的是"列表原顺序首项"而非"排序后首项"。
**处置建议**：`SDK.pas` 的 `ObjLongWordSort_2` 落地后，把默认实现改为转调它（一行）。

### T-P9-02（待确认，**未做断定**）：`ObjBase.GetMaster` 是否等价于 `m_Master`
`ObjDummy.IsProperTarget`（`:290`）把**字段**与**属性**两种读法并列在同一个 `or` 里。
`GetMaster` 的**实现体不在本单元**，故本文件只固化"两种读法并存"这一形态（F13），
**不声称它们等价、也不声称它们必然不同**。请在 `ObjBase.pas` 移植时核对。

### X-P9-01（跨区事项，**未改**，请集成方处置）
`tests/GXX.M2Server.Tests/M2ConfigIsolationCoverage.cs` 的静态全局清单里**没有**本车道新增的静态接缝
`ObjRobotSeam` / `ObjFireDragonSeam` / `ObjDummySeam` / `ObjDummySortSeam`。
本车道的每个用例都自己 `using var env = new MonstersTestEnv();` 安装/还原（**靠自觉**），
建议按该机制的设计初衷把它们一并纳纲（**一行**），取得"不靠自觉"的双保险。
（同类先例：该清单已把 `NpcSeams` / `AuctionDbRunSeam` / `DbLayerRunSeam` 纳入。）

### X-P9-02（跨区事项，**未改**）
`ObjRobotHostSeams.cs` 在 `partial class TCreature` 里补了 `m_boSuperMan`。
`ObjBase.pas` 全量批次落地时**必须删除该 partial 块**，否则 **CS0102**。

---

## 6. 门禁输出摘要

```
dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo
  → 0 个错误（165 个警告，均为既有文件的既有警告）

dotnet test GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj -c Debug --nologo
  → 失败: 0，通过: 9753，已跳过: 0，总计: 9753

dotnet test ... --filter "FullyQualifiedName~Sweep9Monsters"
  → 失败: 0，通过: 271，已跳过: 0，总计: 271
```

> **计数对账**：`M2Server.Tests` 在本车道开工时（main @ `b48e4c43`，含切片 1 前的基线）为 **9482**；
> 本车道结束后为 **9753** ⇒ **+271**，与 `--filter "~Sweep9Monsters"` 的 **271** 逐例相符
> （84 + 64 + 123 = 271，见第 1 节的文件表）。

---

## 7. 接缝清单（复用优先，未造第三份实现）

| 接缝 | 来源 | 说明 |
|---|---|---|
| `SweepSeam.MyGetTickCount` / `MainOutMessage` / `FileExists` / `ResetDefaults` | **复用**（`src/GXX.M2Server/Sweep/SweepSeams.cs`，车道 `p3-m2-sweep` 已并入 main） | 计时 / 日志 / 文件存在性 |
| `M2Config.dwDummy*WalkTime` / `dwDummy*AttackTime` / `dwIncSpeedDecInterval` | **直接引用**（`Engine/M2Config.ClientConf.cs`、`Forms/DummySetting/DummySettingConfig.cs`） | 未新建任何配置替身 |
| `IRobotNpcSeam`（`g_RobotNPC.GotoLable`） | 新增最小接缝 | 待 `ObjNpc.pas` / `M2Share.pas` 落位 |
| `ObjRobotSeam.FindMap`（`g_MapManager.FindMap`） | 新增最小接缝 | 待 `UsrEngn.pas` / `M2Share.pas` 落位 |
| `ObjRobotSeam.DecodeTime` / `Time` / `Now` / `DayOfTheWeek` | 新增最小接缝 | `git grep DecodeTime\|DayOfTheWeek -- src/GXX.Core` = 0 命中；**DayOfTheWeek 已对齐 Delphi 语义（1=周日）** 并有专测 |
| `ObjRobotSeam.LoadFromFile` | 新增最小接缝（**D-P9-05**） | 单测不碰磁盘 |
| `ObjFireDragonSeam.AddEvent`（`g_EventManager.AddEvent`） | 新增最小接缝 | 待 `GameEvent.pas` / `M2Share.pas` 落位 |
| `ObjDummySeam.g_FunctionNPC` / `g_PluginManager` / `Runtime` | 新增最小接缝 | 待 `ObjNpc.pas` / `PluginManager.pas` / `M2Share.pas` 落位 |
| `ObjDummySortSeam.CustomSort`（`ObjLongWordSort_2`） | 新增最小接缝（**T-P9-01**） | 待 `SDK.pas` 落位 |

---

## 8. 方法论遵循自检

| 任务书条款 | 落实 |
|---|---|
| 1. 1:1 忠实（逐方法、保留控制流/早退顺序/边界/字段名/常量名/类名） | 每个例程都有 `// :行号` 行内溯源；字段名 `m_xxx`、常量名 `sRO*`/`nRO*`/`DR_*`/`SKILL_*` 原样保留；类名保留 `T` 前缀 |
| 2. 原文缺陷照抄 + `// 原文如此` + 差异断言 | **39 条**缺陷逐条登记（Robot F1–F10、FireDragon F1–F12、Dummy F1–F15 + 分支结构性发现），每条至少一条差异断言 |
| 3. 虚分派必须保留（§18.8） | `TRobotObject.Run` 真 `override`（已用 `GetBaseDefinition` 断言）；`TFireDragonGuard.AttackTarget` **刻意不加 `override`**（原文无）；唯一形式暂缓是 `SendSocket`（**D-P9-02**，有绊线断言） |
| 4. 不造第三份实现（§14.2） | 新增接缝 **9 类**全部是 `interface` / `Func` 委托 / `Action`，**零个替身基类、零个 `TPlayObject` 替身成员**；唯一 partial 补成员是 `m_boSuperMan`（**D-P9-04**，已 `git grep` 确认缺失 + 留删除指引） |
| 5. 测试伴随（xUnit，`Sweep9MonstersXxxTests.cs`，不改 csproj） | 3 个测试文件 + 1 个测试工具文件，**271 例**；无 `csproj` 改动 |
| 6. 否定性断言必须计数取证（§37.3） | 所有"原文没有这一支 / 只出现 N 次 / 从不读取"类断言都写成 **常量 + 对账断言**（如 `SLabelReadSites == 0 && 声明+赋值 == 出现次数`、`GuardLoopTypeChecks == 0 && 入口判据 == 1`、`{$IF}` 2 ↔ `{$IFEND}` 2 ↔ `MULTI_THREAD` 声明 1），未靠肉眼 |
| 7. 先读文档 + 参考同类车道 | 已读 `docs/转换开发文档.md` 全文与台账 §3 / §14.2 / §18.7 / §18.8 / §34 / §37；风格对齐 `Sweep/Nations.cs`（接口接缝）、`ObjMonCore.cs` / `IcicleMonsterCore.cs`（怪物单元 Core + 差异断言）、`SweepTestKit.cs`（内存 FS + 接缝复用） |

---

## 9. 提交记录

| 批次 | 内容 |
|---|---|
| P1 | `ObjRobot.pas` 20/20 完整移植 + 84 例 + `m_boSuperMan` partial 补成员 |
| P2 | `ObjFireDragon.pas` 14/14 逻辑移植 + 64 例 |
| P3 | `ObjDummy.pas` 19/19 逻辑移植 + 123 例 |
| P4 | 本报告 + 最终门禁复核 |
