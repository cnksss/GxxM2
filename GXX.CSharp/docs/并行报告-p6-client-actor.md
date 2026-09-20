# 并行报告 — 车道 `p6-client-actor`

**源单元**：`Source\Client-HGE\CustomActor.pas`（1,130 行，GBK）
**分支**：`par/p6-client-actor`
**工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p6-client-actor`

---

## 1. 全部 commit hash

| hash | 说明 |
|---|---|
| `2ddb4cc4` | `WIP-不可合并` — 纯逻辑层 + 实体骨架（编译通过、无测试） |
| `f6ef8e1d` | CustomActor.pas 1:1 移植 — 纯决策层 + `TCustomActor` + 148 项测试 |
| `64d8fa20` | **HEAD** — 补 5 项「退回基类落点」测试（→153 项）+ 本报告 |

> 中间那笔 `WIP-不可合并` 是**已登记的存档提交**（按纪律要求标注）。它**不是** HEAD，
> `tools/Check-LaneReady.ps1` 只检 HEAD subject，实测输出
> `par/p6-client-actor  3  0  p6: 补 153 项测试…` → `OK: 3 lane(s) ready to merge, none marked WIP.`
> 三笔**均编译通过**，无损坏风险；若需线性历史可在集成侧 `rebase -i` 压平。

---

## 2. 逐方法族判定表

| 方法（原文行号） | 判定 | 说明 |
|---|---|---|
| `Create` 61-70 | ✅ 已完成 | 全部 6 行赋值逐字保留；含 68 行 `m_nChrLight := 1`（**不是** `m_nOldChrLight`） |
| `CalcActorFrame` 72-401 | ✅ 已完成（决策层） | 10 个动作分支 + 6 个攻击槽 + 默认落空；330 行的分支顺序、条件、写入字段全量 |
| └ 350-377 环绕特效生成 | ⚙️ 接缝 | `SelfCenterEffect`/`SelfCenterEffectFrame` 已把参数算尽；`TMagicEff.Create`+`AddEffectList` 由集成方落地 |
| ├ 380-390 `Self_PlayDelayAction` 分支 | ⛔ 不移植 | 整段被原文 `{...}` 注释 |
| └ 174-205 `SM_LIGHTINGEX` | ⛔ 不移植（空实现） | 整段被原文注释，本体为 `begin end`；仅 93-98 重置生效（已固化） |
| `LoadSurface` 402-584 | ✅ 已完成（决策层） | 幽灵门、三段图库选择、三段偏移、取图分派全量；真实取图走接缝 |
| └ 425-484 按动作选 `ClientAction` | ⛔ 不移植 | 整段被原文 `{...}` 注释 |
| `GetDefaultFrame` 585-639 | ✅ 已完成 | 死亡/骨架/石化/站立四路 + `cf` 三重钳制 + `FClientAction` 落值 |
| `DrawChr` 640-787 | ✅ 已完成（顺序与门） | 三种 `DrawOrder` 的绘制序列、`DrawSelfMagicEffect` 双相门、帧推进、图号公式全量；绘制本身走接缝 |
| ├ 655-661 / 675-715 | ✅ 已完成 | 攻击槽映射、帧范围门、`mdctNone`/`mdctNormal` 图号、`mdctCenter` 抑制 |
| └ 220-228 / 245-250 / 380-390 等注释块 | ⛔ 不移植 | 原文整段注释 |
| `Run` 788-1038 | ✅ 已完成（决策层） | 效果帧推进、魔法帧门、`m_boCreateEffect` 三态、844/941 两段特效门全量 |
| └ 899-908 / 923-931 / 980-1028 特效实例化 | ⚙️ 接缝 | `SpawnEffectFn` 接缝 + 计划记录已给全参数 |
| `RunActSound` 1039-1107 | ✅ 已完成 | 9 个动作分支、帧号（1/2/3）、`Random(8)` 门、`m_boRunSound` 关闭 |
| `RunSound` 1108-1130 | ✅ 已完成 | `SM_STRUCK`（含武器音）+ `SM_DIGUP`；1117/1118 副作用 |

**未覆盖**：仅上表标 ⛔ 的「原文已注释」段落（按规程不移植）与标 ⚙️ 的渲染/音频/入列副作用。

---

## 3. 新增文件与方法行号覆盖

### 新增/改动文件

| 文件 | 动作 | 内容 |
|---|---|---|
| `GXX.CSharp/src/GXX.Client/Scenes/CustomActor.cs` | **新建**（1,989 行） | 纯决策层 + `TCustomActor` 本体 |
| `GXX.CSharp/src/GXX.Client/Scenes/PlaySceneNewActor.cs` | 改 | **删除 6 行 `TCustomActor` 桩**；`CustomMonsterConfigResolver` 由 `Func<int,object?>` 收敛为 `Func<int,TClientCustomMonsterConfig?>` |
| `GXX.CSharp/tests/GXX.Client.Tests/CustomActorTests.cs` | **新建** | 153 项测试 |
| `GXX.CSharp/tests/GXX.Client.Tests/FormJ72Tests.cs` | 改 1 行 | `_ => new object()` → `_ => new TClientCustomMonsterConfig()`（随 resolver 类型收敛） |

### `CustomActor.cs` 内各类型的对应原文行号

| 类型 | 对应原文 | 纯逻辑覆盖 |
|---|---|---|
| `CustomActorEnv` | 全单元注入点 | — |
| `CustomActorGate` | 6 处前置门（+ RunActSound 1043 例外） | 全部 |
| `CustomActorAttackActions` | Grobal2.pas 1938-1943 | 常量核对 |
| `CustomActorAttackSlot` | 279-330（攻击槽）、655-660、832-839 | 全部 |
| `CustomActorLogic` | **72-401** | 分支全量；接缝：350-377 |
| `CustomActorDefaultFrame` | **585-639** | 全量 |
| `CustomActorSound` | **1039-1107 / 1108-1130** | 全量 |
| `CustomActorSurface` | **402-584** | 决策全量；接缝：548-579 取图 |
| `CustomActorDraw` | **640-787**（含 648-717 内嵌过程） | 顺序/门/图号全量；接缝：绘制调用 |
| `CustomActorRun` | **788-1038** | 决策全量；接缝：899-1028 实例化 |
| `TCustomActor` | 18-48 / 61-1130 | 状态搬运；接缝：渲染/音频/入列 |

### 已覆盖 / 未覆盖行号区间（按原文）

| 原文区间 | 状态 |
|---|---|
| 1-60（uses / 声明） | — 无逻辑 |
| 61-70 | ✅ 全量 |
| 72-173 | ✅ 全量 |
| 174-205 `SM_LIGHTINGEX` | ⛔ 原文整段注释 → 实现为空（已固化） |
| 206-278 | ✅ 全量（220-228 / 245-250 注释块保留说明） |
| 279-346 | ✅ 全量 |
| 347-391 | ✅ 决策全量；380-390 原文注释 |
| 392-401 | ✅ 全量 |
| 402-424 | ✅ 全量 |
| 425-484 | ⛔ 原文整段注释 |
| 485-547 | ✅ 全量 |
| 548-579 | ✅ 分派决策；⚙️ 实际取图接缝 |
| 580-584 | ✅ 全量 |
| 585-639 | ✅ 全量 |
| 640-717 | ✅ 顺序/门/图号；⚙️ 绘制接缝 |
| 718-787 | ✅ 全量 |
| 788-843 | ✅ 全量 |
| 844-939 | ✅ 决策全量；⚙️ 特效实例化接缝 |
| 940-1030 | ✅ 决策全量；⚙️ 特效实例化接缝 |
| 1031-1038 | ✅ 全量 |
| 1039-1130 | ✅ 全量 |

---

## 4. ★ 虚分派处置（本车道最高风险项）

原文 7 个方法**全部带 `override`**（声明 37-44）。逐方法对照托管基类 `TActorCore`：

| 原文方法 | 基类成员 | 基类是否 `virtual` | 本车道处置 | 结论 |
|---|---|---|---|---|
| `CalcActorFrame` 37 | `ActorCore.cs:247` `public void CalcActorFrame()` | ❌ **非虚** | `new` 隐藏 | ⚠️ **缺虚成员** |
| `LoadSurface(Sender)` 38 | **无同名成员**（`ClEvent.LoadSurface()` 属其它类） | — | 新增重载（`LoadSurface(object?)`） | ⚠️ **缺虚成员** |
| `GetDefaultFrame` 39 | `ActorMotion.cs:191` `public int GetDefaultFrame(bool)` | ❌ **非虚** | `new` 隐藏 | ⚠️ **缺虚成员** |
| `DrawChr` 40 | **无同名成员** | — | 新增重载 | ⚠️ **缺虚成员** |
| `Run` 41 | `ActorCore.cs:338` `public void Run(uint)` | ❌ **非虚** | `new` 隐藏 | ⚠️ **缺虚成员** |
| `RunActSound` 44 | **无同名成员**（基类侧只有静态 `ActorSoundDispatch.RunActSoundWar`/`Other`，签名不同） | — | 新增重载 | ⚠️ **缺虚成员** |
| `RunSound` 43 | **无同名成员**（基类侧只有静态 `ActorSoundDispatch.RunSound`，签名不同） | — | 新增重载 | ⚠️ **缺虚成员** |

**当前后果（诚实说明）**：`CalcActorFrame` / `GetDefaultFrame` / `Run` 三处在多态调用下
**不会**落到 `TCustomActor` —— 因为基类成员非虚、且我用 `new` 隐藏。`LoadSurface` / `DrawChr` /
`RunSound` / `RunActSound` 四处因基类无同名成员，多态调用**无从谈起**（编译期就没有可覆写的目标）。

> 我**没有**用「非虚/静态委托」去伪装覆写（台帐 §18.8 的两次事故形态），也**没有**为了
> 拿到 `override` 而去改基类文件（`ActorCore.cs` / `ActorMotion.cs` **不在本车道独占区**）。

### 精确的基类补虚成员请求（交给集成方 / 基类归属车道）

请把下列成员改为 `virtual`（并**保持现有实现体不变**，仅加修饰符）：

```csharp
// GXX.CSharp/src/GXX.Client/Scenes/ActorCore.cs
// 第 247 行，现为：public void CalcActorFrame()
public virtual void CalcActorFrame()          // ← 原 Actor.pas:3482 是 virtual，TCustomActor:37 override

// 第 338 行，现为：public void Run(uint now)
public virtual void Run(uint now)             // ← 原 Actor.pas:7391 是 virtual，TCustomActor:41 override

// GXX.CSharp/src/GXX.Client/Scenes/ActorMotion.cs
// 第 191 行，现为：public int GetDefaultFrame(bool wmode)
public virtual int GetDefaultFrame(bool wmode) // ← 原 Actor.pas:6136 是 virtual，TCustomActor:39 override
```

**为什么必须是 `virtual`**：`TActorCore` 是全部角色类的**唯一** headless 承载基类，
而其被调用点均为**基类静态类型**（`PlaySceneMessages.cs:729` `G.MySelf.CalcActorFrame()`；
`ActorMessages.cs:277,287` `CalcActorFrame()`；`ActorMotion.cs:290` `GetDefaultFrame(m_boWarMode)`）。
若基类成员非虚，则 `TCustomActor` 的同名方法**永远不会**被这些调用点选中 ——
表现为「自定义怪的帧计算/默认帧/推进被通用动作表接管」，在联机时只在自定义怪身上出错，
**编译通过、单测通过**，正是 §18.8 描述的隐蔽缺陷形态。

另需在基类**新增**下列 4 个虚成员（原文均为 `TActor` 的虚方法，子类覆写）：

```csharp
// Actor.pas TActor 的 headless 对应（签名对齐 CustomActor.pas 声明，去掉 Pascal 的 Sender 形状）
public virtual void LoadSurface()                          // CustomActor.pas:38  override
public virtual void DrawChr(int dx, int dy, bool blend, bool boFlag)  // :40  override
public virtual void RunSound()                             // :43  override
public virtual void RunActSound(int frame)                 // :44  override
```

**补齐后的必要改动**（集成方，一次性）：
1. `TCustomActor` 的 `CalcActorFrame` / `GetDefaultFrame` / `Run` 把 `new` 改成 `override`；
2. `TCustomActor` 的 `LoadSurface(object? sender)` 改为无参 `LoadSurface()` 并加 `override`
   （默认 `object? sender` 形参在原文中并未被使用 —— 402-584 全文不含 `Sender`）；
3. `DrawChr` / `RunSound` / `RunActSound` 加 `override`；
4. 各调用点改用基类静态类型的虚分派，即**无需**改调用点（虚分派自动生效）。

我已把这条请求写进 `CustomActor.cs` 头部注释（第 27-36 行）与每个方法上方注释，便于定位。

---

## 5. 测试用例数与 build / test 结果

### 新增测试：`CustomActorTests.cs` — **153 项**，0 失败

覆盖分布：前置门 1 · `CalcActorFrame` 26 · `SelfCenterEffect` 4 · `GetDefaultFrame` 12 ·
`LoadSurface` 15 · `DrawChr` 18 · `Run` 15 · `m_Saying` 4 · `RunSound`/`RunActSound` 16 ·
`TCustomActor` 本体 + 退回基类落点 10 · 常量核对 3（部分为 Theory，实际断言数更多）。

**8 组差异断言**（"看起来一样实则不同"）：

| 测试 | 夹住的差异 |
|---|---|
| `CalcActorFrame_SkeletonBranchIsEmptyButLightStillWritten` | 空实现动作**仍**写 `m_nChrLight` |
| `CalcActorFrame_LightingAndSkeletonBothLeaveClientActionNil` | 两个空实现的输出**完全一致**（防误"补全"） |
| `CalcActorFrame_DeathUsesLastFrameWhileNowDeathUsesFullRange` | `SM_DEATH` 单帧定格 vs `SM_NOWDEATH` 整段 |
| `CalcActorFrame_StruckFrameTimeDiffersFromActionPlayTime` | `SM_STRUCK` 用 `m_dwStruckFrameTime`，其余用 `PlayTime` |
| `CalcActorFrame_WalkAndBackstepShiftDifferOnSameDir` | 前进 `m_btDir` vs 后退 `GetBack(m_btDir)` |
| `SurfaceFetchKind_Effect2DoesNotHonourGrayScale2` | 三段取图分派的**不对称**（Effect2 段不认 `ceGrayScale2`） |
| `DrawSequence_Eff1Eff2SelfAndUnknownShareTheElseBranch` | 第三个枚举值与**任何未知值**落同一 `else` |
| `Run_NoFlyConditionIsNotTheComplementOfFlyCondition` | 844 与 941 的「或」关系**不互补**，844 优先 |
| `ActorCtor_ChrLightIsOneNotOldChrLight` | 68 行写 `m_nChrLight` 而非 `m_nOldChrLight` |
| `RunSound_ReturnsNullOnlyForBypassNotEmptyResult` | 门内「无声」返回**空列表**而非 null |

### 门禁结果（在本工作树实跑）

```
dotnet build GXX.slnx -c Debug --nologo          →  0 Error(s)（146 pre-existing warnings，无新增）
dotnet test tests\GXX.Client.Tests\...            →  Passed! Failed: 0, Passed: 4319, Total: 4319
```

**基线对照**：`main` 基线 **4166 例**（实跑确认）→ 现 **4319 例 = 4166 + 153**，
**零新增失败**。`GXX.Client.csproj` 亦单独 0 error、0 warning（本车道文件）。

---

## 6. 发现的原文缺陷 / 易错点（带 `文件:行`）

1. **`CustomActor.pas:844` 与 `:941` 的条件不是互补关系**（最值得记录）
   - 844 分支门：`(Fly_StartIndex < 0) or (Fly_PlayCount <= 0)`
   - 941 分支门：`(Fly_StartIndex >= 0) or (Fly_PlayCount > 0)`
   两者**都含 `or`**。当 `Fly_StartIndex >= 0 且 Fly_PlayCount > 0` 时 844 为假、941 为真；
   当 `Fly_StartIndex < 0` 时 844 为真（走目标分支），**即便 `Fly_PlayCount > 0`**。
   结果：`Fly_StartIndex < 0 且 Fly_PlayCount > 0` 这类**配置错乱**的怪物会静默走「只有目标特效」，
   而 `Fly_StartIndex >= 0 且 Fly_PlayCount <= 0` 时 844 也为真 → 同样走目标分支。
   已用 `Run_NoFlyBranchWinsWhenStartIndexIsNegative` + `Run_NoFlyConditionIsNotTheComplementOfFlyCondition` 双向固化。
   **不可**改成 `if/else if` 的直觉形态。

2. **`CustomActor.pas:551 / 562 / 573` 三段取图分派不对称**
   Body 段与 Effect1 段都列了 `ceGrayScale, ceGrayScale2, ceBright`，
   而 **Effect2 段（573）只列了 `ceGrayScale, ceBright`，缺 `ceGrayScale2`**。
   故 `ceGrayScale2` 下前两段取灰度图、第三段取原图。已用
   `SurfaceFetchKind_Effect2DoesNotHonourGrayScale2` 固化，**不可**"统一"三段。

3. **`CustomActor.pas:68` 写的是 `m_nChrLight := 1`，不是 `m_nOldChrLight`**
   `m_nOldChrLight` 在构造后保持默认 0，直到 98/596 行把它读进 `m_nChrLight`。
   初次移植极易写成 `m_nOldChrLight := 1`。已用 `ActorCtor_ChrLightIsOneNotOldChrLight` 固化。

4. **`CustomActor.pas:1043` 的 `m_boRunSound` 门位于前置门之前**（全单元唯一门序例外）
   `RunActSound` 先判 `if not m_boRunSound then Exit;`，**再**判 `(m_nChangeAppr >= 0) and (m_btRace <> 156)`。
   其余 6 处前置门都在最前。门序颠倒会让「已播过音的自定义怪」在退回基类时行为不同。
   已用 `RunActSound_ExitsWhenRunSoundFlagFalse` 与 `ActorRunSound_BypassGateDoesNotPlayAnything` 两侧夹住。

5. **`CustomActor.pas:667` 与 `:668` 比较的是两个不同字段**
   667 比 `Self_PlayTime`（毫秒间隔），668 比 `Self_PlayCount`（帧数上限）。
   写成同一个字段会导致自身特效帧不推进或无限推进。已用
   `AdvanceSelfFrame_IncrementBoundaryIsSelfPlayCountInclusive`（`6 <= 4` 为假时不动）固化，
   并断言 `?` 边界（`4 <= 4` 为真 → 仍 `Inc`）为**闭区间**。

6. **`CustomActor.pas:842` 的 `Exit` 会跳出整个 `Run`**，因此它也跳过了
   1033-1035 的 `else m_boCreateEffect := False`。而 829 行已先把 `m_boCreateEffect := True`，
   故该路径下标志**保持真**。已用 `Run_UnknownEffectNumberExitsEarly`（断言 `CreateEffect == true`）固化。

7. **`CustomActor.pas:228` 的 `else` 需要缩进判读**
   `else` 属于 `if (ClientConfig.Fly_StartIndex < 0) or (ClientConfig.Fly_PlayCount <= 0)`（844），
   而非内层 `if`。原文缩进（`end` 后接 `else if`）容易误读。已用 `Run_FlyBranchBuildsFlyPlan` 固化为飞行分支。

8. **`CustomActor.pas:259` 的 `+ ClientAction.PlayCount - 1` 后紧跟 `m_nEndFrame := m_nStartFrame`**
   `SM_DEATH` 是**单帧定格**且 `EndFrame` 取 `StartFrame`（而非 `StartFrame + PlayCount - 1`）。
   已用 `CalcActorFrame_DeathUsesLastFrameWhileNowDeathUsesFullRange` 与
   `CalcActorFrame_StandIsNotSingleFrameUnlikeStoneRevive` 双固化。

---

## 7. 接缝清单 + 集成方精确签名要求

全部接缝集中在 `CustomActorEnv`（本文件），**默认值均为"无头惰性"**，不影响既有测试。

| 接缝 | 签名 | 对应原文 | 集成方需提供 |
|---|---|---|---|
| 时钟 | `Func<uint> MyGetTickCountFn` / `Func<uint> TimeGetTimeFn` | 115/129/415/667… | 已默认 `SceneTime.TickNow` |
| 随机 | `Func<int,int> RandomFn` | 1052 `Random(8)` | 已默认 `_ => 0` |
| 图库计数 | `Func<int> EffectImageListCountFn` | 369/497/505/527/682 | `g_EffectImageList.Count` |
| 幽灵开关 | `bool ClientConfigBoHideGhost` / `ConfigDlgCkHideGhost` / `PlugInEnabled` | 421 | `g_ClientConfig.boHideGhost` / `g_ConfigDlg.ConfigCheckeds[ckHideGhost]` / `PlugInEnabled` |
| 画布就绪 | `bool GameCanvasActive` / `GameCanvasInitialized` | 414 | `GameCanvas.Active` / `.Initialized` |
| 取图 | `Func<int,int,int,int,string,object?> FetchSurfaceFn` | 551-576 | 见下表 |
| 取图偏移 | `Func<int,int,(int Px,int Py)> GetCachedImageOffsetFn` | 690/699 | `GetCachedImage(idx, out px, out py)` |
| 表面存在性 | `Func<int,int,bool> SurfaceExistsFn` | 706 | `SelfEffectSurface <> nil` |
| 绘制 | `Action<int,int,object?,string> DrawSurfaceFn` | 708-711 / 729-782 | `GameCanvas.Draw` / `DrawBlend` |
| 音效 | `Action<string> PlaySoundFn` / `Action<int> PlaySoundByIdFn` | 1053/1121/1122 | Bass 层 |
| 坐标 | `Action<int,int,RefInt,RefInt> ScreenXYfromMCXYFn` | 351/876/945/946 | `PlayScene.ScreenXYfromMCXY` |
| 方向 | `Func<int,int,int,int,int> GetNextDirectionFn` / `GetFlyDirection16Fn` / `GetFlyDirectionFn` | 695/697/951/953 | `MapPath.GetNextDirection` 等 |
| Finalize | `Action<TCustomActor>? OnFinalizeRequested` | 423 | 原 `Finalize` |
| 特效入列 | `Action<TCustomActor, CustomActorRunPlan>? SpawnEffectFn` | 899-908 / 923-931 / 980-1028 | 见下 |
| 特效查重 | `CustomActorEffectSink.FindActorFn` / `EffectListFn` / `ContainsEquivalentTargetEffect(...)` | 853/858/878 | `PlayScene.FindActor` / `m_EffectList` |
| 多目标 | `CustomActorRunContext.SayingTargets(string)` | 912-938 | `m_Saying` |

### `FetchSurfaceFn` 的 `kind` 约定

返回 `"image"` / `"gray"` / `"bright"` / `"none"`，集成方按此选：

```
"gray"   → giBody.GetCachedGrayImage(idx, px, py)
"bright" → giBody.GetCachedBrightImage(idx, px, py)
"image"  → giBody.GetCachedImage(idx, px, py)
"none"   → 不取图（m_boReverseFrame 为真）
```

### `SpawnEffectFn` 的落地签名要求（原文 899-1028 的 3 条建特效路径）

```csharp
// 路径 A：899 —— Target_LockDraw 真；用 IMagicTarget 版构造
//   TCustomMonTargetEffect.Create(Target_StartIndex, Target_StartIndex2, Target_PlayCount, Actor)
// 路径 B：901 —— Target_LockDraw 假；用坐标版构造
//   TCustomMonTargetEffect.Create(Target_StartIndex, Target_StartIndex2, Target_PlayCount,
//                                 Actor.m_nCurrX, Actor.m_nCurrY)
// 三者随后统一写：DrawMode / DrawMode2 / ImgLib / Light / NextFrameTime / MagOwner := Self
// 路径 C：980 —— 飞行
//   TCustomMonFlyEffect.Create(Fly_StartIndex + FlyDir * (Fly_PlayCount + Fly_EmptyCount),
//     IntCurrentX, IntCurrentY, IntTargetX, IntTargetY, Actor,
//     Fly_PlayCount, ExplosionImgLib, Explosion_LockDraw, False)
```

上述构造函数**均已存在**：
`MagicEffectsCustomMon.cs:271`（坐标版）、`:294`（目标版）、`:57`（飞行版）。
但 `TActorCore` **未实现 `IMagicTarget`**（`Rx`/`Ry`/`ShiftX`/`ShiftY` 四个只读属性缺失），
故集成方需二选一：
- **推荐**：给 `TActorCore` 加 `: IMagicTarget` 并补齐 4 个属性（`m_nRx`/`m_nRy`/`m_nShiftX`/`m_nShiftY` 已在基类存在）；
- 或加一个薄适配器 `TActorMagicTargetAdapter : IMagicTarget`。

**另需集成方接入**（本车道未做，因超出独占区）：
- `m_CustomMagicStatusEffect`（243 行）：我用标量 `m_CustomMagicStatusEffect_Struck` 承载；
  真正的聚合应在 `TActorCore` 侧提供。
- `SetSound`（1118 行）：基类未登记，`RunSound` 中留了空占位 `SetSound_()`。
- `ActionChanged`（582 行）：已接到基类已有的 `ComputeActionChanged()`（`ActorMotion.cs:303`，`virtual`）。

### 集成方还需注意的两处行为变更

1. `PlaySceneNewActor.CustomMonsterConfigResolver` 的类型由 `Func<int, object?>`
   **收敛为** `Func<int, TClientCustomMonsterConfig?>`（与 `TActorCore.CustomMonsterConfigResolver`
   在 `ActorMotion.cs:180` 的定义同型）。若集成侧另有实现该接缝的代码，需同步改类型。
2. `TCustomActor.Config` 的类型由 `object?` **变为** `TClientCustomMonsterConfig`（值类型属性）。
   `CreateActorForCustomMon` 已改为 `new TCustomActor(cfg.Value)` 走原 61 行构造。

---

## 8. 诚实说明：未完成部分与剩余量

**已完成且全绿**：本单元 8 个方法的**全部决策/状态/帧计算逻辑**（可无缝单测的部分），
外加 153 项测试、0 build error、0 test failure。

**未完成（性质分类，非偷工）**：

| 项 | 性质 | 剩余量估计 |
|---|---|---|
| 7 个方法的虚分派落地 | **被基类阻塞** — 需改 `ActorCore.cs` / `ActorMotion.cs`（不在我的独占区） | 集成方按 §4 清单改 3 个修饰符 + 加 4 个虚成员 + 7 处 `new`→`override`；约 30 分钟 |
| 特效实例化 + `AddEffectList` | 接缝 — 需 `IMagicTarget` 适配 | 约 1-2 小时（含 `TActorCore : IMagicTarget`） |
| 纹理真实取图 / 绘制调用 | 接缝 — 无头环境无法截图，按规程走接缝 | 约 1 小时接线 |
| 音频播放 | 接缝 — Bass 层，属独立批次 | 不属本单元 |
| `m_CustomMagicStatusEffect` 聚合 | 依赖基类 | 随基类批次 |
| `SetSound` | 依赖基类 | 随基类批次 |

**风险自评**：
- 本车道最大风险是 §4 的虚分派缺口 —— 它**不会**在 build/test 中暴露（当前 4319 例全绿），
  只在多态路径上出错，与台帐 §18.8 的两次事故同型。已尽我所能用「纯静态层 + 153 项测试」
  把**逻辑**锁死，并给出可直接套用的补虚清单；但**多态正确性本身必须由集成方补齐后才能验证**。
- 次要风险：本单元大量使用 `in TClientCustomMonsterConfig`（`unsafe struct` + `InlineArray`）。
  已确认 `Actions[(int)type]` / `AttackConfigs[idx]` 按值读取、`Sounds[i].Value` 取值均正确
  （`MakeSoundCfg` 直写后能读回，见测试）。

**工具链备注（供后续车道避坑）**：
- 用 `pwsh` 的 `ReadAllText`/`WriteAllText` 编辑此文件时，**混用 CRLF 的 here-string 锚点会静默失配**，
  导致 `-replace` 无效或误删闭括号。本车道为此浪费了一轮排障（Roslyn 只报 `CS1513: } expected`
  于文件末行，真实缺失处是 `CustomActorRunContext` 的闭括号）。
  **建议：对 `.cs` 一律用 `edit`/`write` 工具，不要用 pwsh 字符串替换。**
- `Get-Content` 与 `[IO.File]::ReadAllLines` 对本文件的**行数报告不一致**（1820 vs 1981），
  定位问题时以 `ReadAllLines` 与工具 `read` 为准。
