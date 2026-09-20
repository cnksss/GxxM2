# 并行报告 — 车道 `p7-client-virtual`

**任务**：恢复客户端 actor 继承链的**虚分派**（一次精确的机械改动）
**分支**：`par/p7-client-virtual`
**工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p7-client-virtual`

---

## 1. 全部 commit hash

| hash | 说明 |
|---|---|
| `28868762` | 11 处机械改动 + 多态验证测试（7 例）+ `CustomActorTests` 三处调用点收敛；build 0 error、`GXX.Client.Tests` 4319→4326 全绿 |
| 本报告所在提交 | 仅新增 `GXX.CSharp/docs/并行报告-p7-client-virtual.md`（HEAD，无 WIP 标记） |

> 本车道**没有** `WIP-不可合并` 提交：第一笔提交前已实跑
> `dotnet build GXX.slnx -c Debug`（0 error）与
> `dotnet test tests\GXX.Client.Tests`（Failed 0 / Passed 4326），故两笔均已验证。

---

## 2. 11 处改动逐一确认

行号为**改动后**的行号（改前行号取自 `HEAD~1`，与任务书给的 ±2 漂移一致）。

### A. 基类侧（4 处）

| # | 文件:行 | 改前 | 改后 |
|---|---|---|---|
| 1 | `src/GXX.Client/Scenes/ActorCore.cs:250`（改前 :247） | `public void CalcActorFrame()` | `public virtual void CalcActorFrame()` |
| 2 | `src/GXX.Client/Scenes/ActorCore.cs:344`（改前 :338） | `public void Run(uint now)` | `public virtual void Run(uint now)` |
| 3 | `src/GXX.Client/Scenes/ActorMotion.cs:192`（改前 :191） | `public int GetDefaultFrame(bool wmode)` | `public virtual int GetDefaultFrame(bool wmode)` |
| 4 | `src/GXX.Client/Scenes/PlaySceneNewActor.cs:475/478/481/484`（`TActor` 类体在 :472，改前 :466 是空类体） | `public class TActor : TActorCore { }` | 新增 4 个虚成员：`public virtual void LoadSurface()` / `public virtual void DrawChr(int dx, int dy, bool blend, bool boFlag)` / `public virtual void RunSound()` / `public virtual void RunActSound(int frame)`，均空实现，注释注明「对应原文 TActor 的虚方法，供子类 override，**本轮新增**」 |

### B. 子类侧 `src/GXX.Client/Scenes/CustomActor.cs`（7 处）

| # | 行（改前） | 改前 | 改后 |
|---|---|---|---|
| 5 | :1698（1691） | `public new void CalcActorFrame()` | `public override void CalcActorFrame()` |
| 6 | :1751（1740） | `public void LoadSurface(object? sender)` | `public override void LoadSurface()`（**去掉 sender 形参**；方法体未引用 `sender`） |
| 7 | :1812（1798） | `public new int GetDefaultFrame(bool wmode)` | `public override int GetDefaultFrame(bool wmode)` |
| 8 | :1831（1814） | `public void DrawChr(int dx, int dy, bool blend, bool boFlag)` | `public override void DrawChr(int dx, int dy, bool blend, bool boFlag)` |
| 9 | :1938（1917） | `public new void Run(uint now)` | `public override void Run(uint now)` |
| 10 | :1981（1957） | `public void RunSound()` | `public override void RunSound()` |
| 11 | :2000（1973） | `public void RunActSound(int frame)` | `public override void RunActSound(int frame)` |

**11 处全部按表完成，无一项跳过。** 方法体除签名外**零改动**（未新增任何 `inherited`
调用；基类新成员是空实现，发出调用也等价，留给后续批次按需接入）。

### 附带的注释同步（非行为改动，逐条列出以免被当成隐藏重构）

| 位置 | 说明 |
|---|---|
| `CustomActor.cs` 文件头「虚分派」段（:29 起） | 原文写「基类**非虚**／**无此成员**，故用 `new`」，已改为「本轮已补 `virtual`／新增空虚成员，7 处全部 `override`」 |
| `CustomActor.cs` `TCustomActor` 前置说明块（:1564 起） | 同上，并把逐方法处置表更新为 `override` |
| 7 个方法各自的 XML 注释 + 4 处方法体内「基类无此成员，不发出调用」注释 | 改为「基类空虚成员，不发出调用（行为不变）」 |
| `ActorCore.cs` / `ActorMotion.cs` / `PlaySceneNewActor.cs` 三处 XML 注释 | 注明「车道 p7-client-virtual 补 virtual／本轮新增虚成员」 |

---

## 3. 多态验证测试（本任务核心证据）

文件：`tests/GXX.Client.Tests/VirtualDispatchCustomActorTests.cs`（**新增**，7 例）
手法：`private static TActor AsBaseType(TCustomActor a) => a;` —— 用**基类静态类型 `TActor`**
持有 `TCustomActor` 实例（与 `PlaySceneMessages.cs:729`、`ActorMessages.cs:277/287`、
`ActorMotion.cs:290` 的调用形状一致），再逐一调用 7 个方法。

判据全部是**结构性**的（基类实现不可能写出这些状态）：

| 测试 | 判据 |
|---|---|
| `BaseTypedCalcActorFrame_LandsOnCustomActor` | `m_nStartFrame=112 / m_nEndFrame=115 / m_dwStartTime=4242`（接缝时钟）+ `ClientAction=matStand`（子类独有 `FClientActionIndex`） |
| `BaseTypedLoadSurface_LandsOnCustomActor` | `m_boLoadSurface` 由 true→false、`m_BodySurface="s"`、取图接缝被调用 |
| `BaseTypedGetDefaultFrame_LandsOnCustomActor` | 返回 `120`（`100+3*(4+2)+2`）+ `ClientAction=matStand` |
| `BaseTypedDrawChr_LandsOnCustomActor` | `DrawSelfMagicEffect` 664-672 的**时钟接缝被调用**（基类空实现不会调用） |
| `BaseTypedRun_LandsOnCustomActor` | 1033-1035 分支把 `m_boCreateEffect` 由 true 落到 **false** |
| `BaseTypedRunSound_LandsOnCustomActor` | 播放 `s_digup`（走 `PlaySoundFn` 接缝）+ `m_boRunSound=true` |
| `BaseTypedRunActSound_LandsOnCustomActor` | 播放 `s_attack` + `m_boRunSound=false` |

### 改动前（实跑，`HEAD~1` + 测试文件）

**阶段 A —— 3 个方法（改动前即可编译）**：
`dotnet test --filter FullyQualifiedName~VirtualDispatchCustomActorTests` →
**Failed: 3, Passed: 0, Total: 3**（`[exit code 1]`）

```
Failed BaseTypedCalcActorFrame_LandsOnCustomActor
  Expected: 112   Actual: 20      ← 通用动作表 MA19 接管（race 156 落 MA19）
Failed BaseTypedGetDefaultFrame_LandsOnCustomActor
  Expected: 120   Actual: 32      ← 同上
Failed BaseTypedRun_LandsOnCustomActor
  Assert.False() Failure  Expected: False  Actual: True   ← m_boCreateEffect 未被回落
```

**阶段 B —— 4 个新成员（改动前无失败态可构造，只有编译失败态）**：
用一次性探针（`TActor b = a; b.LoadSurface(); b.DrawChr(...); b.RunSound(); b.RunActSound(3);`）
实跑 `dotnet build tests\GXX.Client.Tests` → **4 Error(s)**，逐个为：

```
error CS1061: 'TActor' does not contain a definition for 'LoadSurface'
error CS1061: 'TActor' does not contain a definition for 'DrawChr'
error CS1061: 'TActor' does not contain a definition for 'RunSound'
error CS1061: 'TActor' does not contain a definition for 'RunActSound'
```

即：改动前这 4 个名字**在基类静态类型上根本不存在**，`TCustomActor` 的实现只能被子类静态类型
（或 `TCustomActor` 变量）调到 —— 这正是"多态路径被静默旁路"的编译期形态。探针文件已删除。

### 改动后（实跑）

```
Passed BaseTypedLoadSurface_LandsOnCustomActor        [12 ms]
Passed BaseTypedRunActSound_LandsOnCustomActor        [11 ms]
Passed BaseTypedCalcActorFrame_LandsOnCustomActor     [14 ms]
Passed BaseTypedRun_LandsOnCustomActor                [ 2 ms]
Passed BaseTypedGetDefaultFrame_LandsOnCustomActor    [ 1 ms]
Passed BaseTypedDrawChr_LandsOnCustomActor            [ 2 ms]
Passed BaseTypedRunSound_LandsOnCustomActor           [ 1 ms]
Total tests: 7        （全绿）
```

---

## 4. 警告

- **CS0114 清单：空**。全仓**没有**其它 `TActor`/`TActorCore` 子类声明同签名成员
  （逐名 grep 全仓 `.cs`：`CalcActorFrame` / `GetDefaultFrame(bool)` / `Run(uint)` /
  `LoadSurface()` / `DrawChr(int,int,bool,bool)` / `RunSound()` / `RunActSound(int)`
  的定义只有 `CustomActor.cs` 一处；`ClEvent.cs` 的 `LoadSurface/Run` 属**另一个类**，
  与 `TActor` 链无关；`MagicEffects*.cs` 的 `Run()` 是 `bool Run()`，签名不同）。
  故本次 virtual 化**未产生任何 CS0114**。
- **其它警告：0 新增**。`dotnet build --no-incremental` 后与基线警告清单**逐条比对完全一致**
  （150 Warning(s) / 300 条日志行，`Compare-Object` 双向差集为空），全部是既有的
  xUnit 分析器警告（xUnit2000/2004/2013/2031 等），与本次改动无关。
- **Error：0**。

---

## 5. build / test 结果（本工作树实跑）

| 门禁 | 基线 | 改动后 |
|---|---|---|
| `dotnet build GXX.slnx -c Debug --nologo` | 0 Error / 150 Warning | **0 Error / 150 Warning**（清单一致） |
| `dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug` | Passed **4319** / Failed 0 | Passed **4326** / Failed **0** / Skipped 0 |

`4326 = 4319（基线）+ 7（新增虚分派测试）`，**无新增失败**。

---

## 6. 未按任务表完成的项

**无。** 11 处全部按表落地。两处需要说明的偏差：

1. **`CustomActor.cs:1740` 的方法体未引用 `sender`**，故按表安全去掉形参改为无参
   `override`，未触发"停下并报告"的条件。
2. **任务书的前置侦察结论「全仓没有任何地方以带参形式调用 `LoadSurface(...)`」不成立**：
   `tests/GXX.Client.Tests/CustomActorTests.cs` 有 **3 处** `a.LoadSurface(null)`
   （原 :1634 / :1648 / :1809），走的是 `TCustomActor` 静态类型，签名收敛后必须同步。
   这 3 行属"其余只读"区，但**不改成无参就无法编译**，故按最小机械改动同步为
   `a.LoadSurface();`（3 行，无其它改动），特此声明为计划外的必要适配。
   （教训：`grep 'LoadSurface\s*\('` 会命中带实参的调用，上一轮的结论只核对了定义处与
   `RequestLoadSurface`，漏了实参调用点。）

---

## 7. 遗留风险（诚实说明）

1. **基类 4 个新成员是空实现**：本轮只补"虚分派槽位"，原文 `TActor.LoadSurface`
   （5480-5593）/ `DrawChr`（6067-6129）/ `RunSound`（6788-6901）/ `RunActSound`（6903-7200）
   的**本体仍未移植**。因此对非自定义怪 actor 经基类静态类型调 `LoadSurface()` 等，
   现在会正确地分派到 `TActor` 的**空实现**（此前是编译不过或调不到）——行为与改动前
   等价（改动前那 4 个成员在基类上不存在，调用点无从到达），但**槽位已就位、本体待补**。
2. **`TCustomActor` 的 7 处 `inherited` 转调仍未发出**：原文各方法的"前置门为真 → inherited"
   分支当前只 `return`。因基类新成员为空实现，发出调用与不发出**行为等价**；
   待基类 4 个本体移植后再接入为宜（届时是行为改动，需单独测试）。
3. **还有子类"应当 override 但仍是 `new`"吗**：本次范围内**没有**其它子类隐藏这 7 个名字
   （故 0 个 CS0114）。真正待办是**原文里存在、托管侧尚未写成类 override** 的那批：
   `Tail/HerbActor.cs` 记录的 `TKillingHerb` / `TBeeQueen` / `TMineMon` / `TCentipedeKingMon` /
   `TCastleDoor` / `TWallStructure` / `TDragonBody` 等子类的 `CalcActorFrame` / `GetDefaultFrame` /
   `LoadSurface` / `DrawChr` / `Run` ——它们在 `HerbActor.cs` 里目前只是**静态纯函数**，
   基类虚化后已**具备**落成真 `override` 的条件。本车道不动它们（只读区）。
4. **只读区里已过期的注释**（本车道无权改，留给相应车道）：
   - `src/GXX.Client/Tail/HerbActor.cs:11`：`TActorCore 的这两个方法是 **非虚的**` —— 已过期。
   - `src/GXX.Client/Tail/HerbActor.cs:603`：`("Actor 基类虚方法", 28, "**未落地（受阻）**：… 非虚且签名不同；要 1:1 落成 override 必须改别人的文件（本车道无权）")` —— 阻塞已解除。
5. **生效范围**：具体调用点 `PlaySceneMessages.cs:729`（`G.MySelf.CalcActorFrame()`，
   `G.MySelf` 静态类型为 `TActorCore`）、`ActorMessages.cs:277/287`、`ActorMotion.cs:290`
   （`DefaultMotion` 内 `GetDefaultFrame(m_boWarMode)`）现在会落到 `TCustomActor`
   （当实例是自定义怪时）。这些路径**没有**既有单测覆盖（本车道新增的 7 例即为第一批
   回归证据），若前端集成侧另有自定义怪多态场景，属未覆盖面。

---

## 8. 复现命令

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p7-client-virtual\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo
# 只看本车道证据：
dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo `
  --filter "FullyQualifiedName~VirtualDispatchCustomActorTests" --logger "console;verbosity=detailed"
```
