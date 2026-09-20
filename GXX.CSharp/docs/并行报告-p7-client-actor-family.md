# 并行报告 — 车道 `p7-client-actor-family`

**任务**：补齐客户端 actor 基类本体（`TActor` 4 个虚方法） + 移植 7 个原文子类
**分支**：`par/p7-client-actor-family`
**工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p7-client-actor-family`

| 轮次 | 基线（实跑） | 结果 | commit |
|---|---|---|---|
| 第一轮（受限） | build 0 Error / 157 W；`GXX.Client.Tests` **4326** | 4326 → **4386**（+60）；任务1 落成静态核心、任务2 未做（越区阻塞） | `72da453a` `ea380457` `16515c30` |
| rebase 到 main（`5538f7d0`，已并入 §32 批次） | 基线 **4437** | 4437 → 4534 → **4603** | 见 §1.2 |
| **第二轮（越区请求全部获批）** | 续上 | **两项任务全部完成**；`GXX.Client.Tests` **4603** 全绿 | 见 §1.2 |

> ★ **本报告 §0 的"未完成"结论已被第二轮推翻**：父 agent 批准了三项越区请求
> （`PlaySceneNewActor.cs` / `CustomActor.cs` / `ActorSoundDispatch.cs` 加入本车道分区），
> 故 **`TActor` 4 个本体已接上虚槽位、7 个子类已全部落成真类 `override`**。
> §0 保留为**第一轮的历史记录**（并已标注其结论的失效），第二轮的完整内容见 **§12**。

---

## 0. 第一轮结论（★ 已被第二轮推翻，保留为历史记录）

**第一轮这两件事都没能真正完成，原因是同一个、且是当时任务书设下的边界**：

> `TActor` 的 4 个空虚成员声明在 `Scenes/PlaySceneNewActor.cs:474-484`，
> 而 `:472` 的 `public class TActor : TActorCore` **没有 `partial`**；
> 7 个目标子类（`TKillingHerb`/`TBeeQueen`/`TMineMon`/`TCentipedeKingMon`/`TCastleDoor`/
> `TWallStructure`/`TDragonBody`）的**桩类体也全部在同一个文件**（`:504-551`）。
> 按当时任务书「⚠ `PlaySceneNewActor.cs` 不在你的分区 —— 若需要动它，请在报告里提出，**不要自行修改**」，
> 第一轮没有动它 ⇒ **落 `override` 与落 `partial` 均不可能**（`CS0260` + `CS0111`，已实跑确认）。

第一轮的实际交付是：
① 把 4 个本体的 1:1 代码**写成静态核心**（接上虚槽位时**方法体零改动**）；
② 60 例回归证据；
③ 更新两处过期注释；
④ **精确到行**的越区请求清单（§8）——**该清单在第二轮被全部批准并执行**。

---

## 1. 全部 commit hash

### 1.1 第一轮（受限轮）

| hash | 说明 |
|---|---|
| `72da453a` | 切片1：`ActorFamilyEnv.cs` + `ActorFamilyImpl.cs`（4 个本体 1:1 + 接缝 + 缺失字段）。build 0 Error |
| `ea380457` | 切片2：`ActorFamilyBaseTests.cs`（60 例）+ `Tail/HerbActor.cs` 两处过期注释与两张登记表。4326 → 4386 全绿 |
| `16515c30` | docs 报告 + 越区请求清单（**该清单在第二轮被全部批准**） |

> 这三笔已随 main 的 `4a18dc15 integrate par/p7-client-actor-family` **并入 main**。

### 1.2 第二轮（越区请求获批后）

| hash | 说明 |
|---|---|
| `802e87ca` | 切片3：4 本体接上虚槽位 + `CustomActor` 7 处 `inherited` 转调 + `ActorSoundDispatch` 源头补齐 |
| `215775b7` | 切片4：7 个原文子类的 1:1 override 落地 |
| `ac2e7cd5` | 切片5：子类 override 测试 + 覆盖登记表更新 |
| （本报告提交） | `docs/并行报告-p7-client-actor-family.md` 第二轮内容 |

> rebase 说明：第二轮开始前把分支 rebase 到 main 的 `5538f7d0`（含 §32 批次），
> 以便门禁基线取得 main 的真实测试数（**4437**）。

> **本车道没有 `WIP-不可合并` 提交**：两笔提交前都实跑了
> `dotnet build GXX.slnx -c Debug`（0 Error）与
> `dotnet test tests\GXX.Client.Tests`（Failed 0）。
> 最后一笔提交后 `git status --porcelain` **为空**（台账 §29.4 第 3 条）。

---

## 2. 逐子类判定表

原文行号取自 `Source\Client-HGE\HerbActor.pas`（GBK，1341 行；方法体已脚本抽取后回读核对）。

| 类名（原文行） | 覆写的方法（原文行号） | 基类虚成员 | **状态** |
|---|---|---|---|
| `TKillingHerb`（:28） | `Create`（:140-143）· `Destroy`（:145-148）· **`CalcActorFrame`（:157-268）** · **`GetDefaultFrame`（:270-299）** | `CalcActorFrame` ✓ / `GetDefaultFrame` ✓ | **未覆盖**：本体是纯决策表、已在 `HerbActor.cs` 落为 `PlanKillingHerb`/`GetDefaultFrameKillingHerb`；**类 override 未写**（桩类在 `PlaySceneNewActor.cs:505`） |
| `TMineMon`（:37） | `Create`（:1172-1175）· **`CalcActorFrame`（:1166-1170）** · **`GetDefaultFrame`（:1218-1226）** | 同上 | **未覆盖**（桩类 `:544`）。★ 全文最短（`calc`=5 行空转调、`getframe`=9 行未变身恒 0），**最适合作为第一个接通的类** |
| `TBeeQueen`（:45） | **`CalcActorFrame`（:303-397）** · **`GetDefaultFrame`（:399-425）** | 同上 | **未覆盖**：决策表已落为 `PlanBeeQueen`/`GetDefaultFrameBeeQueen`（桩类 `:524`） |
| `TCentipedeKingMon`（:52） | **`CalcActorFrame`（:430-514）** · **`LoadSurface`（:1208-1216）** · **`Run`（:1244-1284）** · `Finalize`（:1202-1206） · `DrawEff`（:1177-1182） · `LoadEffect`（:1184-1200，private） | 5 个**全部** ✓ | **接缝**：`CalcActorFrame` 决策表已落（`PlanCentipedeKing`）；`Run` 依赖 `CheckLoadSurface` + `LoadEffect` + `g_WMonImages.Indexs[15]`；其余依赖 `TTexture` |
| `TCastleDoor`（:76） | `Create`（:519-525）· `Finalize`（:527-531）· `ApplyDoorState`（:533-560，private）· **`LoadSurface`（:562-581）** · **`CalcActorFrame`（:583-671）** · **`GetDefaultFrame`（:673-699）** · `ActionEnded`（:701-709）· **`Run`（:711-724）** · **`DrawChr`（:726-741）** | 5 个**全部** ✓ | **接缝**：`ApplyDoorState` 的 12 次 `MarkCanWalk` 表已落；其余依赖 `TTexture`/`Map`/`BoDoorOpen`/`m_boHoldPlace`（桩类 `:551`） |
| `TWallStructure`（:94） | `Create`（:746-754）· `Finalize`（:756-761）· **`CalcActorFrame`（:763-830）** · **`LoadSurface`（:832-912）** · **`GetDefaultFrame`（:914-927）** · **`DrawChr`（:929-950）** · **`Run`（:952-968）** | 5 个**全部** ✓ | **接缝**：`CalcActorFrame`/`GetDefaultFrame`/`Run` 只依赖数值与 `Map` ⇒ **可落但需桩类可改**；`LoadSurface`/`DrawChr` 依赖 `TTexture`（桩类 `:550`） |
| `TDragonBody`（:132） | **`CalcActorFrame`（:1288-1308）** · `DrawEff`（:1310-1315） · **`LoadSurface`（:1317-1339）** | `CalcActorFrame` ✓ / `LoadSurface(object?)` ✗（见下） | **接缝**：`CalcActorFrame` 只用**数值常量**（`StartFrame=0`/`EndFrame=1`/`FrameTime=400`）⇒ **可落**；`LoadSurface` 依赖 `g_WDragonImg`（桩类 `:548`） |
| *（同族、任务书未列，一并侦察）* | | | |
| `TNewWallStructure`（:111） | `Create`（:973-981）· `Finalize`（:983-988）· `CalcActorFrame`（:990-1048）· `LoadSurface`（:1050-1106）· `GetDefaultFrame`（:1108-1121）· `DrawChr`（:1123-1144）· `Run`（:1146-1162） | 5 个 ✓ | **未覆盖**（**类名在 main 上不存在**，可新增；但落 override 仍受同一 `partial` 阻塞） |
| `TBigHeartMon`（:66） | `CalcActorFrame`（:1230-1234） | ✓ | 已落为 `PlanDirZeroThenInherited`（决策表） |
| `TSpiderHouseMon`（:71） | `CalcActorFrame`（:1238-1242） | ✓ | 同上（**与 `TBigHeartMon` 逐字相同**） |
| `TSoccerBall`（:128） | 无（原文空类） | — | 无待办 |

### 2.1 关键签名发现（本车道的核心侦察结论）

| 原文方法 | 原文签名 | 托管侧虚槽位（`p7-client-virtual` 落的） | 能否直接 `override` |
|---|---|---|---|
| `CalcActorFrame` | `procedure CalcActorFrame; override;` | `public virtual void CalcActorFrame()` | ✅ **可以** |
| `GetDefaultFrame` | `function GetDefaultFrame(wmode:Boolean):Integer; override;` | `public virtual int GetDefaultFrame(bool wmode)` | ✅ **可以** |
| `Run` | `procedure Run; override;` | `public virtual void Run(uint now)` | ✅ **可以**（原文无参 → 托管多一个 `now`，属既定承载方式） |
| `DrawChr` | `procedure DrawChr(dx,dy:Integer; blend,boFlag:Boolean); override;` | `public virtual void DrawChr(int,int,bool,bool)` | ✅ **可以** |
| **`LoadSurface`** | **`procedure LoadSurface(Sender:TObject); override;`** | **`public virtual void LoadSurface()`（无参）** | ❌ **不可以 —— 签名不同** |

★ 即：**任务书"这 5 个方法已具备落 1:1 `override` 的条件"对 4 个成立、对 `LoadSurface` 不成立**。
原文 `LoadSurface(Sender:TObject)` 需要一个 **带参重载**（我在 `ActorFamilyImpl` 里已按原文签名
备好 `LoadSurface(TActorCore self, object? sender)`，见 §3）。

---

## 3. 基类 4 个本体方法的落地情况

**落地文件**：`GXX.Client/src/GXX.Client/Scenes/ActorFamilyImpl.cs`（新增，全部 1:1 静态核心）

| 原文方法 | 原文行范围 | 落地位置（`ActorFamilyImpl.cs`） | 状态 |
|---|---|---|---|
| `TActor.LoadSurface(Sender:TObject)` | 5480-5593（**115 行**） | `LoadSurface(TActorCore, object?)` — 第 **101-199** 行 | ✅ 1:1 |
| `TActor.DrawChr(dx,dy,blend,boFlag)` | 6067-6129（**63 行**） | `DrawChr(TActorCore, int,int,bool,bool)` — 第 **228-262** 行 | ✅ 1:1 |
| `TActor.DrawStateEffSurface`（本体依赖） | 5654-5702（**49 行**） | `DrawStateEffSurface(TActorCore, int,int)` — 第 **281-320** 行 | ✅ 1:1（三层游标回写） |
| `TActor.RunSound` | 6788-6901（**114 行**） | `RunSound(TActorCore)` — 第 **347-370** 行 | ✅ 1:1（经既有 `ActorSoundDispatch.RunSound` 派发） |
| `TActor.RunActSound(frame)` | 6903-7095（**193 行**） | `RunActSound(TActorCore, int)` — 第 **403-497** 行 | ✅ 1:1 |
| `TActor.SetSound`（本体依赖） | 6454-6786（**333 行**） | `SetSound(TActorCore)` — 第 **522-528** 行 | ⚠ **空实现（= 原文语义）**，见 §7.5 |

**接缝与字段**：`GXX.Client/src/GXX.Client/Scenes/ActorFamilyEnv.cs`（新增）

| 区块 | 行 | 内容 |
|---|---|---|
| `ActorFamilyEnv`（接缝） | 19-208 | 22 个接缝字段 + `CustomMagicSoundView` + `Reset()` |
| `TActorCore`（partial 字段扩展） | 210-286 | 补 26 个字段（`m_BodySurface` / `m_boLoadSurface` / 三层状态游标 / 音效槽族 / `m_btSex` 以外的施法层字段…） |

★ **为什么不是一个真实的 `virtual override`**：见 §0/§8。
★ **接通成本**：在 `PlaySceneNewActor.cs:472` 加 `partial` 后，新建一个
`public partial class TActor` 文件写 5 行 `=> ActorFamilyImpl.X(this, …)` 即可 ——
`ActorFamilyImpl` 的**方法体一行都不用改**（它只读写 `TActorCore` 字段与接缝）。

---

## 4. 新增文件 + 已覆盖/未覆盖行号范围

| 文件 | 行数 | 说明 |
|---|---|---|
| `src/GXX.Client/Scenes/ActorFamilyEnv.cs` | 286 | 接缝 + `TActorCore` 字段扩展（**全绿**） |
| `src/GXX.Client/Scenes/ActorFamilyImpl.cs` | 约 590 | 4 个本体 + 2 个依赖本体（**全绿**） |
| `tests/GXX.Client.Tests/ActorFamilyBaseTests.cs` | 1129 | 60 例（**全绿**） |
| `src/GXX.Client/Tail/HerbActor.cs` | 615 → 约 680 | **只改注释与两张登记表**，零行为改动 |
| `docs/并行报告-p7-client-actor-family.md` | 本文件 | — |

### `Actor.pas` 覆盖行号

| 区间 | 状态 |
|---|---|
| 5480-5593 `LoadSurface` | ✅ 已覆盖 |
| 5654-5702 `DrawStateEffSurface` | ✅ 已覆盖 |
| 6067-6129 `DrawChr` | ✅ 已覆盖（6086-6097 / 6117-6128 的 `{$IF Enabled_PlugEngine}` 插件钩子**未覆盖**，见 §8.3） |
| 6454-6786 `SetSound` | ⚠ **未覆盖**（§7.5；但原文对怪物族本就无副作用） |
| 6788-6901 `RunSound` | ✅ 已覆盖 |
| 6903-7095 `RunActSound` | ✅ 已覆盖 |

### `HerbActor.pas` 覆盖行号

| 区间 | 状态 |
|---|---|
| 19-23 / 26 / 28-134（常量·枚举·类声明） | ✅ 已在 `HerbActor.cs` |
| 157-299 / 303-425 / 430-514（3 类 `CalcActorFrame`/`GetDefaultFrame`） | ✅ **决策表已覆盖**（纯函数）/ ❌ **类 override 未覆盖** |
| 519-560 / 673-699（`TCastleDoor` Create/Finalize/ApplyDoorState/GetDefaultFrame） | 部分 |
| 562-671 / 701-741（`TCastleDoor` LoadSurface/CalcActorFrame/ActionEnded/Run/DrawChr） | ❌ 未覆盖 |
| 746-968 / 973-1162（墙系两类的 6 方法） | ❌ 未覆盖 |
| 1166-1175 / 1218-1226 / 1230-1242（`TMineMon`·`TBigHeartMon`·`TSpiderHouseMon`） | ✅ 决策表已覆盖 |
| 1177-1216（`TCentipedeKingMon` DrawEff/LoadEffect/Finalize/LoadSurface） | ❌ 未覆盖 |
| 1244-1284（`TCentipedeKingMon.Run`） | ❌ 未覆盖 |
| 1288-1339（`TDragonBody` 三方法） | ❌ 未覆盖 |

---

## 5. 虚分派确认（★ 逐覆写对应关系）

### 5.1 基类那一侧（`p7-client-virtual` 已落，本车道**复核确认**）

| 基类虚成员 | 声明位置 | 与原文对应 |
|---|---|---|
| `TActorCore.CalcActorFrame()` | `ActorCore.cs:250` | `Actor.pas:1766 procedure CalcActorFrame; virtual;` |
| `TActorCore.GetDefaultFrame(bool)` | `ActorMotion.cs:192` | `Actor.pas:1751 function GetDefaultFrame(wmode:Boolean):Integer; virtual;` |
| `TActorCore.Run(uint now)` | `ActorCore.cs:344` | `Actor.pas:1818 procedure Run; virtual;` |
| `TActor.LoadSurface()` | `PlaySceneNewActor.cs:475` | `Actor.pas:1842 procedure LoadSurface(Sender:TObject); virtual;`（**签名不同**） |
| `TActor.DrawChr(int,int,bool,bool)` | `PlaySceneNewActor.cs:478` | `Actor.pas:1832` |
| `TActor.RunSound()` | `PlaySceneNewActor.cs:481` | `Actor.pas:1819` |
| `TActor.RunActSound(int)` | `PlaySceneNewActor.cs:484` | `Actor.pas:1820` |

### 5.2 子类那一侧

| 子类 | 覆写的基类虚成员 | 本车道是否落下 |
|---|---|---|
| `TCustomActor`（`CustomActor.cs`，**非本分区**） | 7 个全部（`:1698/1751/1812/1831/1938/1981/2000`） | 本次**未动**（它的 7 个 `override` 已在上条车道完成） |
| `TKillingHerb` 等 7 类（`PlaySceneNewActor.cs` 桩） | 0 个 | ❌ **未落下**（§8.1 阻塞） |

### 5.3 ★ 本车道**新增**的非虚接入点（供 §8.1 接通后使用）

| 新增成员 | 位置 | 原文对应 |
|---|---|---|
| `ActorFamilyImpl.LoadSurface(TActorCore, object?)` | `ActorFamilyImpl.cs:101` | `Actor.pas:5480`（**带参重载**，原文签名 1:1） |
| `ActorFamilyImpl.DrawChr / DrawStateEffSurface / RunSound / RunActSound / SetSound` | 同上 | 见 §3 |
| `TActorCore.ActorColorEffectEffective`（属性，非虚镜像） | `ActorFamilyEnv.cs:220` | 读基类那份 `m_ColorEffect` |
| `TActorCore.StateStoneMode`（属性） | `ActorFamilyEnv.cs:222` | 原文 5512 的 `STATE_STONE_MODE` 位 |

---

## 6. 测试用例数 + build / test 结果

| 门禁 | 基线（改动前实跑） | 改动后实跑 |
|---|---|---|
| `dotnet build GXX.slnx -c Debug --nologo` | **0 Error / 157 Warning** | **0 Error / 157 Warning**（逐条一致；`ActorFamily*` 相关警告 **0 条**） |
| `dotnet test tests\GXX.Client.Tests\... -c Debug` | Passed **4326** / Failed 0 | Passed **4386** / Failed **0** / Skipped 0 |

`4386 = 4326 + 60`（新增 `ActorFamilyBaseTests`），**无新增失败**。

### 60 例分布（每公开方法 ≥3 例）

| 覆盖对象 | 例数 | 备注 |
|---|---|---|
| `LoadSurface` | 20 | 含 `[Theory]` 6 行（取图种类三分支） |
| `DrawChr` + `DrawStateEffSurface` | 12 | |
| `RunSound` | 9 | |
| `RunActSound` | 13 | |
| `SetSound` | 1 | |
| null 守卫 | 1 | 6 个方法**显式抛** `ArgumentNullException`（不静默跳过） |

**差异断言**（原文最易"看起来一样实则不同"的三处，均已锁定）：

1. **自定义怪偏移 vs 全局反向帧**：`LoadSurface_CustomMonster_CustomPathOffsetIsRawCurrentFrame`
   （偏移 = 37，**不减** `m_nStartFrame=30`）vs `LoadSurface_GlobalPath_ReverseIndexUsesEndFrameAndStartFrame`
   （`GetOffset + EndFrame − (CurrentFrame − StartFrame)`），另有 `...ReverseAndForwardIndexesDiffer` 直接断言两者不等；
2. **`SM_STRUCK` 三音各自判 `>=0`**（`RunSound_StruckPlaysThreeSoundsIndependently`）
   vs **`SM_ALIVE`/`SM_DIGUP` 不判**（`RunSound_AppearActionPlaysEvenWhenSoundIdIsNegative`，音号 −1 仍会被播）；
3. **race 50 空实现**（`RunActSound_Race50IsEmptyImplementation`：**连随机数都不取**，用计数器锁定）
   vs **race 202..209 硬编码音且不判 frame**（`RunActSound_Mon36FamilyPlaysHardCodedSoundsWithoutFrameGate`：frame=0/7/99 都出声）。

另有：`ceGrayScale` 与 `ceGrayScale2` 同支（`...GrayScaleAndGrayScale2ShareOneBranch`）；
`m_boDuanJin` 只压制**蛛网**而不压制毒烟/冰冻（`...DuanJinSuppressesCobwebOnly`）；
冰冻 0..3 回卷 vs 毒烟 0..9 回卷（`...FrozenWrapsAfter3ButToxicAfter9`）；
外观 900..906 排除区间**边界四点**（899/900/906/907）。

---

## 7. 发现的原文缺陷 / 易错点（带 `文件:行`）

### 7.1 ★★ **既有派发层缺口**：`RunActSound` 的 else 分支只落了一半

`ActorSoundDispatch.RunActSoundOther`（`src/GXX.Client/Scenes/ActorSoundDispatch.cs:260-297`）
只落了原文 **7051-7062** 两段，**漏掉**：

| 原文行 | 内容 | 后果 |
|---|---|---|
| **7063-7072** | `case m_wAppearance of 80:` — `SM_NOWDEATH` + `frame=2` 播 `m_nDie2Sound` | 该外观的怪物死亡音**静默丢失** |
| **7076-7092** | `m_btRace in [202..209]` — `SM_TURN→542` / `SM_STRUCK→495` / `SM_NOWDEATH→496`（**不判 frame**） | Mon36_X 族三种音**全部静默丢失** |

**本车道的处置**：这两支已在 `ActorFamilyImpl.RunActSound` 内**逐字补齐**（第 455-493 行），
并有 4 例测试锁定。**未去改 `ActorSoundDispatch.cs`**（不在我的独占区，且改了会与既有
`FormJ94Tests` 的期望产生分歧）。**建议**：由该文件的所属方把这两支补进 `RunActSoundOther`，
或在接通 §8.1 后统一走 `ActorFamilyImpl`。

### 7.2 `LoadSurface` 的"被校验字段 ≠ 被使用字段"（原文 5544-5550）

判据校验 `ClientAction.StartIndex >= 0` 与 `PlayCount > 0`，而真正取图用的是
**`ActionFile`** 与 **`m_nCurrentFrame`** —— `StartIndex`/`PlayCount` **一个都没用到**，
`ActionFile` 反而**没被校验**。逐字保留并有 `LoadSurface_CustomMonster_TripleGuardRejectsBadSlots` 锁定。
（与 `ClientLoadSurfaceCore`（M2Server，J179）"核心发现十一"同一处，**此处是它在 `GXX.Client` 侧的对应物**。）

### 7.3 九标签 `case` 的两个**空分支** + **无 `else`**（原文 5523-5525 / 5538-5540 / 5541）

`SM_LIGHTINGEX` 与 `SM_SKELETON` 被显式列出却**什么都不做**，结果与"未列出"**完全相同**
（都让 `ClientAction` 保持 nil）⇒ 走 5551 的 `else`：`giBody := nil`、`nBodyOffset := 0`，
最终**不画身体、不报错、不落日志**。有 `..._EmptyCaseBranchesMapToNull` 与
`..._SkillBranchesSplitIdentically` 锁定。

### 7.4 `m_BodySurface := nil` 相邻几行写了**两次**（原文 5494 与 5498）

第二次在自定义怪分支入口，**冗余**。逐字保留（`ActorFamilyImpl.cs` 已注释
`// 5498：原文如此（Actor.pas:5498）`）。

### 7.5 ★ `SetSound` 的"空实现 = 原文语义"（必须说清，否则会被误判为 §25.2 的伪装层）

原文 `SetSound`（6454-6786，333 行）**整段被 `6459 if m_btRace in [0, 1] then begin` 包住且无 `else`**；
6766 的 `end;` 之后 6769-6785 的受击武器音段还要求 `PlayScene.FindActor(m_nHiterCode)` 命中。
⇒ **对怪物族（`m_btRace` ∉ {0,1}），原文的 `SetSound` 逐字就是"什么都不做"**。
故基类空实现**不是**"静默返回中性值"，而是**原文语义**（台账 §25.2 的禁止对象是前者）。
`RunSound` 的 `case` 因此在本族路径上**不会命中任何标签**（音号恒 −1），
而 `RunActSound` 的非武器 else 分支（`m_btRace ∉ {0,1}` 且 `≠ 50`）**是活的** —— 这两点已在
`SetSound_IsNoOpForMonsterRaces` 与 4 例 `RunActSound` 用例中分别锁定。

### 7.6 其它易错点

| 原文行 | 内容 |
|---|---|
| 6803 | `SM_NOWDEATH` 的死亡音**额外要求 `m_boDeath`**（不只判音号 ≥ 0） |
| 6814-6815 | `SM_ALIVE`/`SM_DIGUP` 的 appear 音**没有 `>=0` 判定**（与 6798-6800 三音各自判形成对照） |
| 6915 / 7054 / 7060 / 7068 / 7082 / 7086 / 7090 | 播过之后**几乎都置 `m_boRunSound := False`**，但 `SM_CUSTOM_HIT001` 范围支路**只有真取到配置音才置 False**（原文 6920-6945） |
| 7011 | `SM_102HIT` 屏幕震动要求 `g_ConfigDlg.ConfigCheckeds[ckSceneShake]`（**唯一一处读配置勾选**） |
| 7051-7062 | 两段是**两个独立 `if`**（不是 `else if`）⇒ 理论上可同时命中 |
| 6909 | `RunActSound` 整段被 `if m_boRunSound` 包住（`RunSound` 刚把它全置真） |
| 5660/5675/5690 | 三层状态节的**节流值不同**：蛛网 **100**ms，毒烟/冰冻 **80**ms；回卷区间也不同（0..9 / 0..9 / 0..3） |
| 5669 | 三层里**唯独蛛网**的 y 多减 **20** |
| 5491 vs 5492 | 画布守卫在**打点之前** ⇒ 画布不可用时**不消耗节流窗口**（与 `ClientLoadSurfaceCore`（J179）"核心发现一"逐字一致） |

---

## 8. 接缝清单 + 精确签名 + ★ **需要你处理的越区请求**

### 8.1 ★★ 请求 1（**最高优先，阻塞两条任务的全部产出**）：`PlaySceneNewActor.cs` 的两处改动

**文件**：`GXX.CSharp/src/GXX.Client/Scenes/PlaySceneNewActor.cs`（**不在我的分区**）

```diff
-:472   public class TActor : TActorCore
+:472   public partial class TActor : TActorCore
```

```diff
 :474-484  // 删除这 4 个空虚成员（本体已由 ActorFamilyImpl 提供）
-    public virtual void LoadSurface() { }
-    public virtual void DrawChr(int dx, int dy, bool blend, bool boFlag) { }
-    public virtual void RunSound() { }
-    public virtual void RunActSound(int frame) { }
```

**实测证据**（都不做时）：

```
error CS0260: Missing partial modifier on declaration of type 'TActor';
              another partial declaration of this type exists
error CS0111: Type 'TActor' already defines a member called 'DrawChr' with the same parameter types
error CS0111: Type 'TActor' already defines a member called 'RunSound' with the same parameter types
error CS0111: Type 'TActor' already defines a member called 'RunActSound' with the same parameter types
```

**改完之后**（我可在 1 个切片内完成，方法体零改动）：

```csharp
// 新文件 Scenes/ActorFamilyVirtualWiring.cs
public partial class TActor
{
    public virtual void LoadSurface()                  => ActorFamilyImpl.LoadSurface(this, null);
    public virtual void LoadSurface(object? sender)    => ActorFamilyImpl.LoadSurface(this, sender);  // ★ 原文签名 1:1
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
                                                       => ActorFamilyImpl.DrawChr(this, dx, dy, blend, boFlag);
    public virtual void RunSound()                     => ActorFamilyImpl.RunSound(this);
    public virtual void RunActSound(int frame)         => ActorFamilyImpl.RunActSound(this, frame);
}
```

**连带（请求 1 生效后同时解锁）**：`PlaySceneNewActor.cs:505/516/517/518/524/544/548/550/551`
这 9 行桩类改为 `partial`，7 个目标子类的 `override` 就能落在我自有的
`Scenes/ActorFamilyHerb*.cs` 里（那时 `CalcActorFrame`/`GetDefaultFrame`/`DrawChr`/`Run` 四个
可直接 `override`；`LoadSurface` 走上面那个带参重载）。

> **如果你选择不授权**：请把这条改动转给该文件的所有者，并把 §0 的结论作为本车道的最终状态。

### 8.2 请求 2：`CustomActor.cs` 的 7 处 `inherited` 转调（**行为改动，需单独测试**）

`CustomActor.cs:1758-1763 / 1840-1845 / 1854` 等 7 处"前置门为真 → `inherited`"的分支
目前只 `return`（上条车道因基类为空而**有意**这么写）。**基类现在有本体了**，
这些分支必须改为真调 `base.*`，否则"前置门为真时**什么也不会发生**" ——
正是台账 §24.2「只改基类不改子类 = 完全无效果」的**镜像形态**。
另需在 `TCustomActor` 上覆写 `ActorColorEffect` 与 `ActorSex`，让它把自己的
`m_ColorEffect`（`CustomActor.cs:1628`）与 `m_btSex` 暴露给基类本体。

### 8.3 其它接缝（我这一侧的，已全部显式登记，无静默中性值）

| 接缝 | 精确签名 | 未接线时的语义（**已对应到原文分支**） |
|---|---|---|
| `ActorFamilyEnv.CanvasReadyFn` | `Func<bool>` | `false` = 原文 5491 的 `Exit` |
| `ActorFamilyEnv.MyGetTickCountFn` | `Func<uint>` | `SceneTime.TickNow()` |
| `ActorFamilyEnv.CustomMonsterConfigLookupFn` | `Func<int, TClientCustomMonsterConfig?>?` | `null` = 原文 5500-5506 查找未命中 |
| `ActorFamilyEnv.EffectImageListCountFn` | `Func<int>` | `0` = 任何 `ActionFile >= 0` 都判越界（原文 5545 的回退分支） |
| `ActorFamilyEnv.FetchBodySurfaceFn` | `Func<ActorBodyImage, string, object?>` | `null` = 原文 `m_BodySurface` 保持 nil ⇒ 6075 的门为假 |
| `ActorFamilyEnv.GhostHideEnabledFn` | `Func<bool>` | `false` = 原文 5567 三重与为假 |
| `ActorFamilyEnv.FinalizeRequestedFn` | `Action<TActorCore>` | **接缝：待 `TActor.Finalize`（Actor.pas:1848 声明）移植后接入** |
| `ActorFamilyEnv.ActionChangedRequestedFn` | `Action<TActorCore>` | **接缝：待 `TActor.ActionChanged`（Actor.pas:7101-7104，空函数体）接入** |
| `ActorFamilyEnv.GetEffectBaseFn` | `Func<int,int,int, ActorDrawDispatch.EffectBaseRef?>` | `null` = 原文 `wimg = nil`（6104 的门） |
| `ActorFamilyEnv.CanvasDrawBlendFn` | `Action<SurfaceDrawOp>` | 无操作 = 原文 `d = nil` 不调 DrawBlend |
| `ActorFamilyEnv.CanvasDrawOpFn` | `Action<SurfaceDrawOp?>` | 同上（三层状态） |
| `ActorFamilyEnv.DrawEffSurfaceOpFn` | `Action<SurfaceDrawOp>` | 同上 |
| `ActorFamilyEnv.StateFxImageExistsFn` | `Func<string,int,bool>` | `false` = `d = nil`（**注意游标照样推进**，已锁定） |
| `ActorFamilyEnv.PlaySoundByIdFn` / `PlaySoundByNameFn` | `Action<int>` / `Action<string>` | 无操作 = Bass 层未接线 |
| `ActorFamilyEnv.SoundIdFn` | `Func<string,int>` | `-1` = `g_SoundList` 命名音未接线（**60 余个 `s_*` 常量的统一出口**） |
| `ActorFamilyEnv.ViewerDeadFn` | `Func<bool>` | `false` = 原文 6105 未走灰度 |
| `ActorFamilyEnv.RandomFn` | `Func<int,int>` | `0` = 原文 7052 `Random(8)` 未命中（`== 1` 才播） |
| `ActorFamilyEnv.CustomMagicConfigLookupFn` | `Func<int, CustomMagicSoundView?>?` | `null` = 原文 6820/6919 `CustomMagicConfig = nil` |
| `ActorFamilyEnv.SendGameOverBgmDelayFn` / `SceneShakeFn` | `Action?` | `null` = 原文 6807 / 7012 未接线 |

**接缝：待插件引擎（`Enabled_PlugEngine=1` 的 `HookTActor_DrawChr1/2`，Actor.pas:6086-6097 / 6117-6128）移植后接入**
—— 原文被 `{$IF Enabled_PlugEngine = 1}` 条件编译包住，托管侧无对应物。

**接缝：待 `TActor.SetSound`（Actor.pas:6454-6786）与 `THumActor.SetSound` 移植后接入**（§7.5）。

---

## 9. ★ 诚实说明：未完成部分与剩余量

### 9.1 完全没做的（**2 项任务中的 1 项，以及第 1 项的一半**）

| 任务 | 状态 | 原因 |
|---|---|---|
| **任务2：7 个子类的 `override`** | **一行都没写** | 桩类体在 `PlaySceneNewActor.cs:504-551`（§8.1） |
| **任务1：4 个本体的"落地"** | **写成静态核心，未接上虚槽位** | `TActor` 未 `partial`（§8.1） |
| **任务3：两处过期注释** | ✅ **完成** | — |

### 9.2 剩余量（按"接通后还要做的事"估）

| 剩余项 | 规模 | 说明 |
|---|---|---|
| §8.1 的两处机械改动 | 2 行 + 删 4 行 | **由你/所有者执行** |
| `ActorFamilyVirtualWiring.cs`（5 行转调） | 30 行 | 我可在授权后 1 个切片完成 |
| 9 行桩类加 `partial` | 9 行 | 同上 |
| 7 子类的 `override` 本体 | `CalcActorFrame` 4 个 + `GetDefaultFrame` 3 个 + `DrawChr` 3 个 + `Run` 4 个 + `LoadSurface` 6 个 ≈ **600 行** | 其中 `CalcActorFrame`/`GetDefaultFrame` 的**决策表已全部落地**（`HerbActor.cs`），接通时只是转调；`LoadSurface`/`DrawChr`/`Run` 依赖 `TTexture`/`g_WMonImages.Indexs[15]`/`g_WDragonImg`/`Map.MarkCanWalk`/`BoDoorOpen` 等**另一批接缝** |
| `SetSound` 本体 | 333 行 / 60+ 命名音常量 | §7.5 |
| `THumActor.LoadSurface`（Actor.pas:14532-14751）与 `THumActor.DrawChr`（:16501-17420，**900 行**） | 极大 | 本车道**未碰**；注意：这两个类在托管侧**继承我现在给 `TActor` 的本体**，在它们自己的覆写落地前会走**怪物图集路径**（已登记为已知缺口） |
| `TNpcActor.LoadSurface`（Actor.pas:10482）与 `DrawChr`（:10297） | 中 | 同上 |

### 9.3 我做过的、**看起来像越界但其实不是**的动作

1. 用 `write`/`edit` 工具改 `Tail/HerbActor.cs`（任务书明确要求，且是它的独占区）；
2. 以 `partial class TActorCore` 在**自有新文件**里补 26 个字段（`TActorCore` 自 `ActorCore.cs:11`
   起就是 `partial`；**零改既有文件**，符合本工程"消除两个写者"的既定手法）；
3. **没有**给 `TActor` 写 `partial`、**没有**动 `PlaySceneNewActor.cs`、**没有**动 `CustomActor.cs`、
   **没有**动 `csproj`。

### 9.4 一次过程失误（如实记录）

切片1 期间我曾用 `pwsh` 的 `Get-Content -Raw | -replace | Set-Content -NoNewline` 改
`ActorFamilyBase.cs`，**把文件写坏了**（`-replace` 的 `\r?\n` 未跨行匹配 + `-NoNewline` 丢尾换行），
`read` 报 `invalid UTF-8 text`。我随即删除并用 `write` 工具重建。
**教训**：正是台账 §24.5 / 纪律第 10 条（"对 `.cs` 一律用编辑工具，不要用脚本字符串替换"）。
此后**所有** `.cs` 改动都走 `write`/`edit`，并用字节级行尾检查确认过
（`CRLF=0 LF-only=461`、尾部 `}LF`）。

---

## 10. 复现命令

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p7-client-actor-family\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo
# 只看本车道证据：
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo `
  --filter "FullyQualifiedName~ActorFamily"
```

---

# ★ 第二轮（越区请求全部获批后）—— 两项任务全部完成

## 11.1 全部 commit hash（第二轮）

| hash | 说明 |
|---|---|
| `9c26fd88` | rebase 到 main（`5538f7d0`）后的报告提交（第一轮 3 笔已被 main 并入 `4a18dc15`） |
| `802e87ca` | 切片3：4 本体**接上虚槽位** + 越区批准的两处机械改动 + `CustomActor` 的 7 处 `inherited` 转调 + `ActorSoundDispatch` 源头补齐 |
| `215775b7` | 切片4：**7 个原文子类的 1:1 override 落地** |
| `ac2e7cd5` | 切片5：69 例子类 override 测试 + 覆盖登记表更新 |

> 三笔提交前都实跑了 build + test；最后一笔后 `git status --porcelain` **为空**。

## 11.2 三项越区请求的执行结果

| 请求 | 批准 | 执行 |
|---|---|---|
| ① `PlaySceneNewActor.cs` 加 `partial` + 删 4 个空虚成员 | ✅ | ✅ 已做（`:472` 加 `partial`；`:474-484` 的 4 个空虚成员删除，实体移入 `ActorFamilyBase.cs`） |
| ② `CustomActor.cs` 的 7 处 `inherited` 转调 + `ActorColorEffect`/`ActorSex` | ✅ | ✅ 已做（见 §11.4） |
| ③ `ActorSoundDispatch.RunActSoundOther` 缺口 **在源头修** + 删掉我的重复补偿 | ✅ | ✅ 已做（见 §11.5） |

## 11.3 基类 4 本体的最终形态

| 原文方法 | 虚成员（`ActorFamilyBase.cs`） | 1:1 实体 |
|---|---|---|
| `TActor.LoadSurface(Sender:TObject)` 5480-5593 | `virtual void LoadSurface(object? sender)` —— **原文签名** | `ActorFamilyImpl.LoadSurface(TActor, object?)` |
| （同上，`CustomActor.pas` 族的无参形态） | `virtual void LoadSurface()` —— **派生槽位**，如实转调带参重载（传 `null`；原文全文不引用 `Sender`） | 同上 |
| `TActor.DrawChr` 6067-6129 | `virtual void DrawChr(int,int,bool,bool)` | `ActorFamilyImpl.DrawChr` |
| `TActor.RunSound` 6788-6901 | `virtual void RunSound()` | `ActorFamilyImpl.RunSound` |
| `TActor.RunActSound` 6903-7095 | `virtual void RunActSound(int)` | `ActorFamilyImpl.RunActSound` |
| （依赖）`DrawStateEffSurface` 5654-5702 | `virtual void DrawStateEffSurface(int,int)` | `ActorFamilyImpl.DrawStateEffSurface` |
| （依赖）`SetSound` 6454-6786 | `virtual void SetSound()` | `ActorFamilyImpl.SetSound`（**空 = 原文对怪物族的语义**，§7.5 已论证） |

**接通成本实测**：`ActorFamilyImpl` 的方法体**一行未改**（只把形参由 `TActorCore` 收紧为 `TActor`，
以便读到虚接入点）；`PlaySceneNewActor.cs` 只改了 2 处（+1 `partial`、删 4 行）。

## 11.4 `CustomActor.cs` 的改动（请求②）

1. **7 处"前置门为真 → `inherited`"改为真发 `base.*`**：
   `LoadSurface`（`base.LoadSurface(null)`）/ `DrawChr`（precondition + `"body"` 序位两处）/
   `RunSound` / `RunActSound(frame)`。
   > 改动前它们只 `return` —— 基类空实现时"等价"，基类**有本体**后就是
   > "**门为真时什么都不发生**"，即台账 §24.2 的镜像形态。
2. **「继承字段消重」（★ 本轮发现的隐藏缺陷）**：`TCustomActor` 曾自建 **11 个**与基类同名的字段
   （`m_nSpellFrame` / `m_nCurEffFrame` / `m_nEffectFrame` / `m_nEffectEnd` /
   `m_nStruckWeaponSound` / `m_BodySurface` / `m_dwLoadSurfaceTime` / `m_boLoadSurface` /
   `m_boRunSound` / `m_dwEffectFrameTime` / `m_dwEffectStartTime`），形成字段隐藏（CS0108）。
   基类本体读写的是**继承的那一份**，两处永不相等 —— 属"**同一状态两份存储**"的静默缺陷
   （台账 §25.2 同族）。已全部删除，一律使用继承字段。
3. `m_ColorEffect` 改为**覆写虚属性** `ActorColorEffect`（基类存储名为 `m_BaseColorEffect`）；
   3 处读取点随之改用属性。
4. `SetSound_()` 由**恒空替身**改为如实转调 `base.SetSound()`
   （后者是"原文对怪物族无副作用"的正当承载，留个恒空替身会把基类语义伪装成本类占位）。
5. **测试期望更正**：`CustomActorTests.ActorRunSound_BypassGateDoesNotPlayAnything` 原先断言
   `Assert.False(a.m_boRunSound)` —— 那是"基类尚为空实现"时的产物。原文依据
   `CustomActor.pas:1112-1114`（门为真即 `inherited RunSound`）+ `Actor.pas:6794`
   （置真在**基类里**，故子类门之后仍会执行）⇒ 更正为 `Assert.True`，
   并补上"基类确实被转调"的证据（`SM_DIGUP` 的 appear 音）。已改名为
   `ActorRunSound_BypassGateDelegatesToBaseRunSound`。

## 11.5 `ActorSoundDispatch.RunActSoundOther` 的源头修复（请求③）

按父 agent 裁定「**不得留两份同义但内容不同的派发实现**」执行：

1. **在源头补齐两支**（逐字，注释带原文行号）：
   - `7063-7072`：`case m_wAppearance of 80:` → 仅 `SM_NOWDEATH` 且 `frame = 2` 播 `m_nDie2Sound`；
   - `7076-7092`：`m_btRace in [202..209]` → `SM_TURN→542` / `SM_STRUCK→495` / `SM_NOWDEATH→496`
     （**三分支都不判 `frame`**）。
2. **删除我在 `ActorFamilyImpl` 里的重复补偿**，改回转调该权威函数。
3. **新入参位置**：`m_wAppearance` / `m_nDie2Sound` 追加在 `rand8` **之后**（而非之前），
   以不破坏既有 8 处调用点的实参顺序；且该顺序在逻辑上也忠于原文流程（7063 在 7051 之后）。
4. **更正该函数原先"7045-7110ish 的决策部分 1:1"这一与实现不符的注释**
   （它当时只落了 7051-7062 两段，属"注释自称 1:1、实现不完整"，是台账 §25.2/§31.4 同族形态）。
5. **`FormJ94Tests` 的处置（父 agent 要求给出结论与依据）**：
   **结论：原来那 8 处断言没有"锁死错误行为"**，它们只覆盖 7051-7062 段，
   且传入的 `m_wAppearance` 语义位置当时还不存在 ⇒ 只需补两个中性实参（`0, -1`）即可编译，
   **不需要改任何期望值**。另**新增 4 例**锁定补齐的两支（含边界 `201/202/209/210`）。
   > 之所以不需要改期望：原文确有两支（已回读 `Actor.pas:7063-7072` 与 `7076-7092`），
   > 是**实现缺失**而不是**测试期望错**（两者区别见父 agent 的判定规则）。

## 11.6 7 个原文子类的落地（★ 任务2，本轮完成）

| 类（原文行） | 落地文件 | 覆写的方法（原文行号） |
|---|---|---|
| `TKillingHerb`（:28） | `Scenes/ActorFamilyHerb.cs` | `CalcActorFrame` 157-268 · `GetDefaultFrame` 270-299 |
| `TMineMon`（:37） | 同上 | `CalcActorFrame` 1166-1170 · `GetDefaultFrame` 1218-1226 |
| `TBeeQueen`（:45） | 同上 | `CalcActorFrame` 303-397 · `GetDefaultFrame` 399-425 |
| `TCentipedeKingMon`（:52） | 同上 | `CalcActorFrame` 430-514 · `DrawEff` 1177-1182 · `LoadEffect` 1184-1200 · `Finalize` 1202-1206 · `LoadSurface` 1208-1216 · `Run` 1244-1284 |
| `TBigHeartMon`（:66） | 同上 | `CalcActorFrame` 1230-1234 |
| `TSpiderHouseMon`（:71） | 同上 | `CalcActorFrame` 1238-1242 |
| `TCastleDoor`（:76） | `Scenes/ActorFamilyStructures.cs` | `Create` 519-525 · `Finalize` 527-531 · `ApplyDoorState` 533-560 · `LoadSurface` 562-581 · `CalcActorFrame` 583-671 · `GetDefaultFrame` 673-699 · `ActionEnded` 701-709 · `Run` 711-724 · `DrawChr` 726-741 |
| `TWallStructure`（:94） | 同上 | `Create` 746-754 · `Finalize` 756-761 · `CalcActorFrame` 763-830 · `LoadSurface` 832-912 · `GetDefaultFrame` 914-927 · `DrawChr` 929-950 · `Run` 952-968 |
| `TNewWallStructure`（:111，**托管侧原先完全缺失**） | 同上 | `Create` 973-981 · `Finalize` 983-988 · `CalcActorFrame` 990-1048 · `LoadSurface` 1050-1106 · `GetDefaultFrame` 1108-1121 · `DrawChr` 1123-1144 · `Run` 1146-1162 |
| `TDragonBody`（:132） | `Scenes/ActorFamilyDragonBody.cs` | `CalcActorFrame` 1288-1308 · `DrawEff` 1310-1315 · `LoadSurface` 1317-1339 |

### ★ 一处**必须更正的结构事实**（本轮发现）

托管侧原先把这些子类**平铺**为 `: TActor`；而原文层级是：

```
TKillingHerb : TActor
TMineMon / TCentipedeKingMon / TBigHeartMon / TSpiderHouseMon / TDragonBody : TKillingHerb
TBeeQueen : TActor            ← ★ 不是 TKillingHerb
TCastleDoor / TWallStructure / TNewWallStructure : TActor
```

若继续平铺，`inherited` 会**整段绕过 `TKillingHerb` 的覆写** ——
属"看起来一样实则不同"的典型（例如 `TMineMon` 的 `inherited` 本应落到
`TKillingHerb` 的变身前置段 162-173，平铺后却直接落到 `TActorCore`）。
已在 `PlaySceneNewActor.cs` 的桩类头**按原文更正基类**，并有
`SubclassHierarchyMatchesOriginal` 锁定。

### 本轮新增的字面 / 逐字保留项（原文如此）

- **原文本体里被注释掉的写法**逐字保留并注明行号，例如
  `182/190/329/337/384/391`（方向乘法）、`651/814`（`+ frame - 1`）、
  `654/817/1043`（`m_nDefFrameCount` 的赋值被注释）、
  `822`（`m_boHoldPlace := False` 被注释）、`1023/1036`（`m_boUseEffect := True` 被注释）、
  `869-877 / 894-898`（外观 `>= 904` 的整段被 `{ }` 注释 ⇒ **空分支**）；
- `605/785/1012`：`m_sUserName := ' '`（**一个空格**，非空串）；
- `1299`：`TDragonBody` 的 `m_nMaxTick := pm.ActWalk.**ftime**`（不是 `usetick`，原文如此）；
- `1248-1254`：`TCentipedeKingMon.Run` 的移动族 Exit 列表（`SM_WALK/SM_BACKSTEP/SM_HORSERUN/SM_RUN`）
  **与基类 `IsMoveAction` 不同** —— 逐字照抄，未合并。

## 11.7 测试与门禁（第二轮）

| 门禁 | 结果 |
|---|---|
| `dotnet build GXX.slnx -c Debug --nologo` | **0 Error** |
| `dotnet test tests\GXX.Client.Tests` | **Passed 4603 / Failed 0 / Skipped 0** |

进度：`4437（rebase 后基线）→ 4534（切片3 +97）→ 4603（切片5 +69）`。

新增两个测试文件：
- `ActorFamilyBaseTests.cs`（60 例，第一轮）：4 个本体 + 接缝语义；
- `ActorFamilySubclassTests.cs`（69 例，第二轮）：**7 个子类 override 的行为**，
  全部以**基类静态类型** `TActor` 持有实例（与场景工厂调用点同形）。

**差异断言（本轮重点，均双向）**：

| # | A 侧 | B 侧 |
|---|---|---|
| 1 | `TKillingHerb.SM_HIT` 的 `start` **含** `Dir*(frame+skip)`（= 80） | `TBeeQueen` 同条件 = 30（**不含**） |
| 2 | `TKillingHerb` 死亡帧**有** `m_boSkeleton` 分支 | `TBeeQueen` **无**该分支（且无方向乘法） |
| 3 | `TKillingHerb` **有** `SM_DIGUP`/`SM_DIGDOWN` 分支 | `TBeeQueen` **无**（越界动作不改写字段） |
| 4 | `TCentipedeKingMon.SM_DIGDOWN` **不**清 `m_btDir` | 其 `SM_TURN`/`else` **清** |
| 5 | `TWallStructure` 的 `else` 有 `SM_TURN → deathframe` 特判 | `TNewWallStructure` **无** |
| 6 | `TWallStructure.SM_DIGUP` 置 `m_boUseEffect` | `TNewWallStructure` **不置**（1036 被注释） |
| 7 | `TWallStructure.LoadSurface` 用 `m_wAppearance` 且 `>= 904` 为空分支 | `TNewWallStructure` 用 `m_wAppearance - 904`（无空分支） |
| 8 | `TDragonBody` **无**变身前置段 | 其余 6 类**都有** |
| 9 | `TCastleDoor.DrawChr` 主体绘制 **强制** `boFlag = FALSE` | `TWallStructure` **原样传** `boFlag` |
| 10 | `TCastleDoor.CalcActorFrame` 清 `m_boUseEffect` | 怪物族清 `m_boUseMagic` |
| 11 | `ApplyDoorState(dsOpen)` = 15 次 MarkCanWalk（末 3 格改回不可走） | `dsClose` = 12 次（前 3 格**无条件可走**） |
| 12 | `ChangeAppr >= 0`（**0 也算变身**）走基类分支 | `-1` 走自有分支（哨兵值可区分，见测试注释） |

## 11.8 覆盖登记表（第二轮的更正）

`Tail/HerbActor.cs` 的三张表按事实更正：

1. **`Units`**：7 个目标子类由"纯函数抽取"升级为"**类 override（含原文行号）+ 纯函数抽取**"；
   `TNewWallStructure` 补登记；`Actor 基类虚方法` 一条由"未落地"改为"已落地"。
2. **`UncoveredRanges`**：原四段（`583-745` / `763-1165` / `1177-1216` / `1244-1341`）**全部覆盖**，
   改为 `(303,302)` 占位 + 新增 `AllRangesCovered = true` 显式声明。
   > ★ **不要用 `(0,0)` 作占位**：它满足 `From <= To`，会被既有审计断言
   > `UncoveredRanges_AreOrderedAndInBounds` 当成**真实区间**而因越界（原文 1..1341）报错
   > —— 本轮踩过一次，已记入 §11.10。
3. **`SubclassOverrideBlockers`**：语义由"阻塞原因"变为"落地位置"（表名保留以免动既有断言）。
4. **`BaseActorBodies`**：`RunActSound` 一条更正为"已在源头补齐 + 删除重复补偿"。
5. 文件头两处"受阻"说明改为"已落地（第二轮）"。

**既有断言的更正（均注明依据）**：
- `TailHerbActorTests.CoverageTable_IsSelfConsistent`：由「必须存在一条含"未落地"+"Scenes"的登记」
  改为「**不再有**任何未落地条目」（依据：4 本体 + 7 子类的落地位置）；
- `TailHerbActorTests.UncoveredRanges_AreOrderedAndInBounds`：由「必须非空」改为
  「为空时须由 `AllRangesCovered` 显式声明（**不得静默为空**）」。

## 11.9 剩余量（第二轮结束时）

| 剩余项 | 说明 |
|---|---|
| `TActor.SetSound` 本体（333 行 / 60+ 命名音常量） | **未移植**；但对怪物族本就无副作用（§7.5），人类族覆写在 `THumActor` |
| `THumActor.LoadSurface`（14532-14751）/ `DrawChr`（16501-17420，~900 行） | **未碰**。★ 它们**继承**本轮给 `TActor` 的本体，在自身覆写落地前会走**怪物图集路径** —— 已登记为已知缺口；本轮**未**为掩盖它给基类加特判（父 agent 明确认可这一处理） |
| `TNpcActor.LoadSurface`（10482）/ `DrawChr`（10297） | 同上 |
| `TActor.SetSound` 之外的 `Actor.pas` 未移植项 | `LoadNameSurface` / `LoadActorIcons` / `CheckLoadSurface` / `DrawEffSurface` 等（本轮只落了 4 个本体 + 2 个依赖） |
| **取图/绘制/特效的接缝** | `ActorFamilyHerbEnv` 的 11 个接缝（6 取图 + 5 场景/地图）与 `ActorFamilyEnv` 的 22 个接缝仍未接线到真实图库/地图 —— 这是**接缝层**，不是本体的缺失 |
| `ActorFamilySubclassTests` 的覆盖面 | 69 例覆盖 7 类的**帧决策 / 地图标记 / 门状态 / 特效游标**；`DrawChr` 的绘制参数只做了"确实画了"级别的断言（`SurfaceDrawOp` 的完整字段比对可再加厚） |

## 11.10 本轮的两处过程失误（如实记录）

1. **警告计数口径**：`--no-incremental` 全量构建的 `Warning(s)` 计数在 157/163 间波动，
   原因是同一警告会因多项目构建而被**重复计入**（`Sort-Object -Unique` 去重后两次都是 163 行，
   但 `Warning(s)` 摘要行取的是另一口径）。已核实：**`ActorFamily*` / `CustomActorTests` /
   `FormJ94Tests` / `TailHerbActorTests` 四个文件贡献的警告为 0 条**，
   故本轮改动**未引入新警告**；但我**没能**给出"与基线逐条比对完全一致"那种强度的证据
   （第一轮那条 150 条逐条比对的结论本轮未复现）。**如需要该强度证据，请指示我去 main 上跑一次同口径基线。**
2. **`(0,0)` 占位踩坑**：我一度把"无未覆盖区间"表达为 `(0,0)`，它满足审计的
   `From <= To` 过滤条件 ⇒ 被当成真实区间 ⇒ `Assert.InRange(0, 1, 1341)` 失败。
   已改为既有的 `(303,302)` 约定并加注释警示（见 §11.8-2）。

## 11.11 复现命令（第二轮）

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p7-client-actor-family\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo
# 只看本车道证据（9 个测试类共 174 例）：
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo `
  --filter "FullyQualifiedName~ActorFamily"
```

