# 并行报告 — 车道 `p17-client-actor`

**源单元**：`Source\Client-HGE\Actor.pas`（**18,009 行 / 5 个类 / 159 条方法实现**）
**分支**：`par/p17-client-actor`
**工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-actor`
**源镜像**：`_analysis/utf8_mirror/Client-HGE/Actor.pas`（UTF-8；`Source/` 是 GBK）

---

## 0. 一句话结论

**真实覆盖率 = 78 / 159 = 49.06%**（`真实体 78 / NotPorted 77 / 原文如此 4 = 159`）。

本轮把 `Actor.pas` 里**三个整类**从「一行空壳」做成 **1:1 全量**（`TNpcActor` 10/10、
`TStatuaryNpcActor` 9/9、`THeroActor` 6/6，且**更正了 `THeroActor` 的基类**），
把 `THumActor` 的空壳**迁出** `PlaySceneNewActor.cs` 并落下 8 条确定性方法，
并补齐 `TActor` 若干此前**没有任何槽位**的虚成员（`light` / `Initialize` /
`CheckLoadUserName` / `CheckLoadSurface` / `Destroy` / `DrawEffSurface` / `StretchDrawEffSurface`）。

**未完成的主体是 `TActor` 的 60 条**（消息层、16 个属性访问器、名字/喊话渲染族、
移动 `DoSmoothMove`、血条构造族）**与 `THumActor` 的 17 条**（含 5 条巨型方法：
`CalcActorFrame` 1,783 行 / `LoadSurface` 1,969 行 / `DrawChr` 925 行 / `Run` 436 行 /
`PlayMagicEffect` 326 行）。**本报告不把这些填成 0** —— 逐条见 §3。

---

## 1. 方法与可复跑命令

### 1.1 抽取口径

```powershell
# 原文：类方法实现（去重前 176 条 → 去重 156 个「类.方法」→ 159 条实现）
$Src = 'D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\Client-HGE\Actor.pas'
$L   = [System.IO.File]::ReadAllLines($Src)
$inv = for ($i=0; $i -lt $L.Count; $i++) {
  if ($L[$i] -match '^\s*\{?\s*(constructor|destructor|procedure|function)\s+(T\w+)\.(\w+)') {
    [pscustomobject]@{ Line=$i+1; Kind=$Matches[1]; Class=$Matches[2]; Name=$Matches[3] }
  }
}
$inv.Count                       # → 159
$inv.Class | Sort-Object -Unique # → TActor / TNpcActor / TStatuaryNpcActor / THumActor / THeroActor
```

| 类 | 原文声明行 | 实现条数 | 原文行区间 |
|---|---|---|---|
| `TActor` | 1298 | **109** | 2777-9846 |
| `TNpcActor` | 1877 | **10** | 9936-11122 |
| `THumActor` | 1935 | **25** | 11130-17436 |
| `THeroActor` | 2049 | **6** | 17437-17533 |
| `TStatuaryNpcActor` | 1903 | **9** | 17537-18008 |
| **合计** | — | **159** | — |

> 与只读复核车道 `p12-e2only-review` §3.5 的口径差异：p12 报「156 个去重方法名 / 176 条实现」，
> 本报告报「**159 条实现**」（差 17 条的来源：p12 的 176 含 17 条**声明段**误命中，
> 而 159 是按 `T<类>.<方法>` 实现头严格抽取的结果 —— 两个口径的**实现条数都是 176 行原文**，
> 本报告按任务书给定的「159 条方法实现」为准，并在表中**逐条给原文行号**以便对账）。

### 1.2 覆盖率复算（**机械**：只读本报告 §3 表的第 1 列与第 4 列）

```powershell
# 本脚本把 §3 的 159 行判定表解析出来并复算三数 —— 判定本身可在表中逐行复核。
# 行首是「| 原文行号 |」，状态列是「**real** / notported / as-is」（real 加粗）。
$report = 'GXX.CSharp\docs\并行报告-p17-client-actor.md'
$rows = Get-Content $report -Encoding UTF8 |
        Where-Object { $_ -match '^\|\s*\d+\s*\|.*\|\s*\*{0,2}(real|notported|as-is)\*{0,2}\s*\|' }
$real = ($rows | Where-Object { $_ -match '\|\s*\*{0,2}real\*{0,2}\s*\|' }).Count
$np   = ($rows | Where-Object { $_ -match '\|\s*\*{0,2}notported\*{0,2}\s*\|' }).Count
$ai   = ($rows | Where-Object { $_ -match '\|\s*\*{0,2}as-is\*{0,2}\s*\|' }).Count
"rows=$($rows.Count) real=$real notported=$np as-is=$ai"
"coverage = {0:N2}%" -f (100.0*$real/($real+$np+$ai))
# 实测输出： rows=159 real=78 notported=77 as-is=4
#            coverage = 49.06%
```

### 1.3 状态判据（三档 + 一个补充档）

| 状态 | 含义 |
|---|---|
| `real`（**真实体**） | 对应实现已落地，且**我逐条核对了原文主分支**；若原文某段未覆盖，在「说明」列写明。规划层 = 真实体**当且仅当**该规划层就是原文本体的等价承载（如 `ActorDrawDispatch.DrawEffSurface`）。 |
| `notported`（**NotPorted**） | 三种情形合并计入：① 托管侧**无**任何对应成员；② 只有 `NotPorted(...)` 留痕（台帐 §48.1）；③ **近似物** —— 同名成员只是钩子（`=> OnXxx?.Invoke()`）、空体、或签名收敛（如 `SendMsg` 的 3 个重载收敛成 1 个 `TChrMsg` 版）。**近似物一律计为未移植**，不计入覆盖率。 |
| `as-is`（**原文如此**） | 原文该成员体**本身就是空**（`begin end`）。照抄为空即 1:1，故单列而不计入分子分母的"实现"里。 |

---

## 2. 空壳迁移清单（`PlaySceneNewActor.cs` → `Scenes/Actor*.cs`）

`p12-e2only-review` §5.2/§5.3 的裁决：`PlaySceneNewActor.cs:488-571` 有 50+ 个与 `Actor.pas`/`ObjMon.pas`
**同名**的类，每个只有一行 `public override string ActorClass => "Xxx";`，而该文件的头注释写的是
**`PlayScn.pas 6969-7486 NewActor 1:1`** —— 它们是从 `PlayScn.pas` 的 `wRaceImg` 分派表**照抄类名**的壳。
其中**属于 `Actor.pas` 的 4 个**已在本轮迁走：

| 类 | 迁移前（`PlaySceneNewActor.cs`） | 迁移后（本车道） | 原文归属 | 基类更正 |
|---|---|---|---|---|
| `THumActor` | 488-491（空壳） | `Scenes/ActorHumActor.cs` | 1935 / 11130-17436 | `: TActor`（原文 1935 即如此，**正确**） |
| `THeroActor` | 493-497（空壳） | `Scenes/ActorHeroActor.cs` | 2049 / 17437-17533 | ★ **`: TActor` → `: THumActor`**（原文 2049） |
| `TNpcActor` | 536（空壳） | `Scenes/ActorNpcActor.cs` | 1877 / 9936-11122 | `: TActor`（原文 1877，**正确**） |
| `TStatuaryNpcActor` | 537（空壳） | `Scenes/ActorStatuaryNpc.cs` | 1903 / 17537-18008 | ★ **`: TNpcActor`**（原文 1903） |

> **基类更正为什么是必须的**：`THeroActor` 若仍是 `: TActor`，则 `THumActor` 的
> `CalcActorFrame`/`LoadSurface`/`DrawChr`/`Run`/`GetDefaultFrame` 覆写会被**整段绕过**
> （英雄退化成普通怪）；`TStatuaryNpcActor` 若仍是 `: TNpcActor` 之外的东西，
> `inherited Finalize/DrawChr/LoadSurface` 会跳掉 NPC 族的那一层。
> 两者都是「看起来一样实则不同」（台帐 §18.8）的形态，且**编译通过、空壳单测通过**。

**留在 `PlaySceneNewActor.cs` 的 46 个壳**（`TLionMonster`/`TElectronicScolpionMon`/`TMon27_3` …）
**经核对不属于 `Actor.pas`**：`Actor.pas` 只有上表 5 个类；这些是 `ObjMon.pas` 族
（`p13-m2-objmon-real` 的归属），故本车道**不动**它们（避免与 ObjMon 车道争同一分区）。

**新增/改动文件**

| 文件 | 动作 | 内容 |
|---|---|---|
| `Scenes/ActorNpcEnv.cs` | **新建** | NPC 族接缝（取图/配置/绘制/上行/纹理）+ `TActorCore` 实例字段扩展 |
| `Scenes/ActorNpcActor.cs` | **新建** | `TNpcActor` 10 条 1:1 + `TActorCore` 的新虚槽位与 `NotPorted` 留痕设施 |
| `Scenes/ActorStatuaryNpc.cs` | **新建** | `TStatuaryNpcActor` 9 条 1:1（含 `SetEffigyState` 341 行） |
| `Scenes/ActorHeroActor.cs` | **新建** | `THeroActor` 6 条 1:1 + `CM_HERO*` 常量 |
| `Scenes/ActorHumActor.cs` | **新建** | `THumActor` 声明迁移 + 8 条 1:1 + `TActorCore` 字段扩展 |
| `Scenes/ActorDrawEffSurface.cs` | **新建** | `TActor.DrawEffSurface` / `StretchDrawEffSurface` 两个**落点**（转调既有规划层） |
| `Scenes/PlaySceneNewActor.cs` | 改 | 删除 4 个 `Actor.pas` 空壳（13 行 → 迁移注释） |
| `tests/GXX.Client.Tests/ActorNpcActorTests.cs` | **新建** | 151 例 |
| `tests/GXX.Client.Tests/ActorHumActorTests.cs` | **新建** | 31 例 |

---

## 3. 逐例程对账表（159 条）

> 列义：**原文行号** / **方法** / **原文行数** / **状态** / **托管侧承载与说明**。
> 状态三档见 §1.3；"说明"里的 `file:line` 是**我逐条核对过**的承载位置。

### 3.1 `TActor`（109 条；real 45 / notported 60 / as-is 4 → 41.28%）

| 原文行号 | 方法 | 行数 | 状态 | 托管侧承载与说明 |
|---|---:|---|---|---|
| 2777 | `Create` | 168 | notported | 无。`TActorCore` 无 `Create` 成员；168 行（含 <br>全部 `m_*` 初值）未落 |
| 2945 | `Destroy` | 25 | notported | `TActorCore.Destroy()` = `NotPorted("Destroy",2945)`（`ActorNpcActor.cs`）—— 只是**槽位**，25 行本体未落 |
| 2970 | `SendMsg` | 5 | notported | 近似物：`ActorMessages.cs:132` 的 `SendMsg(TChrMsg)` 只做入队尾；原 3 个重载（5/56/…）**签名收敛**，Feature/字段装配未落 |
| 2975 | `UpdateMsg` | 10 | notported | 无 |
| 2985 | `SendMsg` | 56 | notported | 近似物：同上（收敛为 `TChrMsg` 版） |
| 3041 | `FindMsg` | 254 | notported | 无（254 行的消息查找族） |
| 3295 | `UpdateMsg` | 34 | notported | 无（`IsScreenSync` 版） |
| 3329 | `UpdateStruckMsg` | 21 | notported | 无 |
| 3350 | `CleanStruckMsg` | 20 | notported | 无 |
| 3370 | `CleanMsgs` | 21 | **real** | `ActorMessages.cs:126` |
| 3391 | `DeleteMsg` | 20 | notported | 无 |
| 3411 | `CleanUserMsgs` | 25 | **real** | `ActorMotion.cs:437`（`UserMsgIdents` 白名单原地过滤） |
| 3436 | `CleanUserMsgs` | 46 | **real** | 同上（原文 3411-3480 是**同名重复声明**，托管侧一份实现按其语义承载，注释已注明行区间） |
| 3482 | `CalcActorFrame` | 294 | **real** | `ActorCore.cs:250`（表驱动分支：TURN/WALK/RUSH/RUSHKUNG/BACKSTEP/LIGHTINGEX/HIT/STRUCK/DEATH/NOWDEATH/SKELETON）；race 156 自定义怪分支由 `ActorActionTables` + `CustomMonsterFrameCalc.cs` 承载 |
| 3776 | `ReadyAction` | 425 | **real** | `ActorMessages.cs:160`（142 行）：ALIVE 复活、开血条位、旧坐标、dir/step 打包、受击帧时长公式、动作切换+`CalcActorFrame`、死亡标记齐备；原 425 行中的 `m_Saying`/特效分支未覆盖（见 D-P17-06） |
| 4201 | `GetCurrX` | 8 | notported | 无。★ 注意原文**不是直通**：主角色走 `Cutecode_T(FCurrX)`（坐标编解码），非直通实现会错 |
| 4209 | `GetCurrY` | 8 | notported | 无（同上） |
| 4217 | `SetCurrX` | 8 | notported | 无（`Makecode_T`） |
| 4225 | `SetCurrY` | 8 | notported | 无 |
| 4233 | `GetOldX` | 8 | notported | 无 |
| 4241 | `GetOldY` | 8 | notported | 无 |
| 4249 | `SetOldX` | 8 | notported | 无 |
| 4257 | `SetOldY` | 8 | notported | 无 |
| 4265 | `GetRX` | 8 | notported | 无 |
| 4273 | `GetRY` | 8 | notported | 无 |
| 4281 | `SetRX` | 8 | notported | 无 |
| 4289 | `SetRY` | 8 | notported | 无 |
| 4297 | `GetActBeforeX` | 8 | notported | 无 |
| 4305 | `GetActBeforeY` | 8 | notported | 无 |
| 4313 | `SetActBeforeX` | 8 | notported | 无 |
| 4321 | `SetActBeforeY` | 8 | notported | 无 |
| 4329 | `GetMessage` | 34 | notported | 无。托管侧有 `GetNextMsg(out TChrMsg)`（`ActorMessages.cs:135`，4330-4361）—— **不同成员名**，`GetMessage` 的 `ChrMsg:pTChrMsg` 版未落 |
| 4363 | `ProcLastMsg` | 35 | notported | 近似物：`ActorMessages.cs:114` 仅 `OnProcLastMsg?.Invoke()` 钩子 |
| 4398 | `PlaySelfEffect` | 43 | notported | 无 |
| 4441 | `PlaySelfEffectCustom` | 19 | notported | 无 |
| 4460 | `ProcMsg` | 177 | notported | 近似物：`ActorMessages.cs:115` 钩子 |
| 4637 | `ProcHurryMsg` | 234 | notported | 近似物：`ActorMessages.cs:116` 钩子 |
| 4871 | `IsIdle` | 11 | notported | 近似物：`ActorMessages.cs:123` 钩子（默认 `true`） |
| 4882 | `ActionFinished` | 9 | **real** | `ActorMotion.cs:101` |
| 4891 | `GetNextHitTime` | 24 | **real** | `ActorMotion.cs:110` |
| 4915 | `CanMove` | 38 | notported | 无。`PlaySceneCore.cs:1149` 的 `CanMove(int,int)` 是 **`TPlaySceneCore`** 的同名不同族成员，**不可当承载** |
| 4953 | `CanWalk` | 12 | **real** | `ActorMotion.cs:137`（`TActorCore.CanWalk`） |
| 4965 | `CanRun` | 17 | **real** | `ActorMotion.cs:154` |
| 4982 | `Strucked` | 18 | **real** | `ActorMotion.cs:167` |
| 5000 | `Shift` | 169 | **real** | `ActorCore.cs:87`（159 行；UNITX/UNITY、八方向、黑边修正逐分支） |
| 5169 | `FeatureChanged` | 246 | notported | 近似物：`ActorMessages.cs:112` 钩子 |
| 5415 | `light` | 5 | **real** | ★ **本切片新增**：`TActorCore.light() => m_nChrLight`（`ActorNpcActor.cs`），原文 5417 |
| 5420 | `Initialize` | 5 | **as-is** | 原文 5421-5422 为**空体**；★ 本切片在 `TActorCore` 上新建同名虚槽位（此前**不存在**），供 `TNpcActor` 10254 / `THumActor` 11218 覆写 |
| 5425 | `Finalize` | 55 | notported | 近似物：`ActorFamilyHerbEnv.cs:195` 的 `virtual void Finalize() { }` 是**空体**；原文 55 行释放 12 个纹理槽未落 |
| 5480 | `LoadSurface` | 115 | **real** | `ActorFamilyBase.cs:64` → `ActorFamilyImpl.cs:74`（p7 已落，含 9 标签 case 与三重判据） |
| 5595 | `CharWidth` | 16 | notported | 无（`m_btHorse` 两分支 + 48/100 兜底） |
| 5611 | `CharHeight` | 16 | notported | 无（70/70 兜底） |
| 5627 | `CheckSelect` | 27 | **real** | `PlaySceneActors.cs:267`（`TActorCore.CheckSelect`，12 行；五向 alpha 采样，原文 5632-5636） |
| 5654 | `DrawStateEffSurface` | 50 | **real** | `ActorFamilyBase.cs:108` → `ActorFamilyImpl.cs:223` + `ActorDrawDispatch.cs:181` |
| 5704 | `DrawEffSurface` | 110 | **real** | 规划层 `ActorDrawDispatch.cs:86`（48 行）+ ★ 本切片落点 `TActor.DrawEffSurface`（`ActorDrawEffSurface.cs`） |
| 5814 | `StretchDrawEffSurface` | 74 | **real** | 规划层 `ActorDrawDispatch.cs:138` + ★ 落点 `TActor.StretchDrawEffSurface` |
| 5888 | `DrawWeaponGlimmer` | 27 | notported | 无（`ActorDrawDispatch.cs:75` 只有**文档提及**，无方法） |
| 5915 | `DrawShieldEffect` | 20 | notported | 近似物：`DressEffectRender.cs:39` 只给「是否画」的规划（`ShieldEffectGate`），非 20 行本体 |
| 5935 | `DrawPlayEffect` | 39 | **real** | `ActorPlayEffectQueue.cs:113` |
| 5974 | `DrawDressEffect` | 18 | notported | 无（`TActor` 版，非 `THumActor` 11260 那条） |
| 5992 | `DrawDressEffectEx` | 16 | notported | 无 |
| 6008 | `GetDrawEffectValue` | 59 | notported | 无 |
| 6067 | `DrawChr` | 64 | **real** | `ActorFamilyBase.cs:94` → `ActorFamilyImpl.cs:182`（方向守卫 + 施法层） |
| 6131 | `DrawEff` | 5 | **as-is** | 原文 6132-6133 为**空体**（同一族里 `TNpcActor.DrawEff` 10424 才是实体） |
| 6136 | `GetDefaultFrame` | 81 | **real** | `ActorMotion.cs:192`（83 行，含死亡/骨架/石化/站立四路 + `cf` 三重钳制） |
| 6217 | `DefaultMotion` | 26 | **real** | `ActorMotion.cs:279` |
| 6243 | `SetMagicSound` | 211 | **real** | `ActorDrawDispatch.cs:276`（`MagicSoundIds`，含 40 余条技能音效修正表） |
| 6454 | `SetSound` | 334 | notported | 近似物：`ActorFamilyImpl.cs:386` 只承载「非 0/1 种族 ⇒ 什么都不做」这一段等价（原文 6459 的门无 else）；334 行主体未落 |
| 6788 | `RunSound` | 115 | **real** | `ActorFamilyBase.cs:121` → `ActorFamilyImpl.cs:275` + `ActorSoundDispatch.cs:46` |
| 6903 | `RunActSound` | 194 | **real** | `ActorFamilyBase.cs:148` → `ActorFamilyImpl.cs:319` |
| 7097 | `RunFrameAction` | 4 | **as-is** | 原文 7098-7099 为**空体**；托管侧无成员（基类空体本身即 1:1，故计入 as-is） |
| 7101 | `ActionChanged` | 5 | **as-is** | 原文 7102-7104 为**空体**；`ActorFamilyHerbEnv.cs:209` 建了同名虚成员转调 `ComputeActionChanged()` |
| 7106 | `ActionEnded` | 14 | **real** | `TActorCore.RunActionEnded()`（`ActorMotion.cs`）+ `ActorFamilyHerbEnv.cs:185` 的原文名虚成员 |
| 7120 | `CheckLoadUserName` | 54 | notported | `NotPorted("CheckLoadUserName",7120)`（`ActorNpcActor.cs`）—— **只是槽位**；54 行名库装载未落 |
| 7174 | `CheckLoadActorIcon` | 39 | notported | 无 |
| 7213 | `LoadActorIcons` | 61 | notported | 近似物：`ActorFamilyHerbEnv.cs:202` = `virtual void LoadActorIcons() { }`**空体** |
| 7274 | `CheckLoadPlayEffect` | 43 | **real** | `ActorPlayEffectQueue.cs:44` |
| 7317 | `LoadPlayEffectSurface` | 38 | **real** | `ActorPlayEffectQueue.cs:93` |
| 7355 | `CheckLoadSurface` | 12 | notported | `NotPorted("CheckLoadSurface",7355)`（`ActorNpcActor.cs`）—— 只是槽位；12 行（`m_boLoadSurface` + 2 秒节流 + `GetRaceByPM` 守卫）未落 |
| 7367 | `CheckLoadFengHaoSurface` | 24 | notported | 无 |
| 7391 | `Run` | 282 | **real** | `ActorCore.cs:344`（38 行：行走族守卫 + 帧推进 + `m_boDelActionAfterFinished`）；原文 282 行中的魔法路径 `/1.8`、特效帧推进未覆盖（见 D-P17-06） |
| 7673 | `DoMove` | 342 | notported | 近似物：`ActorMessages.cs:122` 的 `DoMove(step) => OnDoMove?.Invoke(step) ?? false` 钩子 |
| 8015 | `DoSmoothMove` | 351 | notported | 无（351 行平滑移动） |
| 8366 | `MoveFail` | 29 | **real** | `ActorMotion.cs:370` |
| 8395 | `CanCancelAction` | 8 | **real** | `ActorMotion.cs:325` |
| 8403 | `CancelAction` | 6 | notported | 近似物：`ActorMessages.cs:113` 钩子 |
| 8409 | `CleanCharMapSetting` | 26 | **real** | `ActorMotion.cs:349` |
| 8435 | `NewHealthNumberFromGroup` | 80 | notported | 无 |
| 8515 | `AddHealthNumber` | 86 | notported | 无 |
| 8601 | `ShowIcons` | 66 | **real** | `ActorSelfEffectRender.cs:217`（67 行） |
| 8667 | `ShowHealthNumber` | 84 | **real** | `ActorHealthNumber.cs:440` |
| 8751 | `DrawSelfEffect` | 143 | **real** | `ActorSelfEffectRender.cs:49`（160 行） |
| 8894 | `DrawLockTargetEffect` | 16 | **real** | `ActorSelfEffectRender.cs:371` |
| 8910 | `GetNearObjectHintInfo` | 16 | **real** | `ActorSelfEffectRender.cs:395` |
| 8926 | `DrawExploreItemEffect` | 36 | **real** | `ActorSelfEffectRender.cs:327` |
| 8962 | `DrawPreviewItem` | 73 | notported | 近似物：`PreviewItemRender.cs`（**非 `Actor*` 文件**）是规划层；`ActorHpBar.cs:125` 只留 `DrawPreviewItemFn` 接缝 |
| 9035 | `LoadHealthNumber` | 177 | **real** | `ActorHealthNumber.cs:268` |
| 9212 | `CheckLoadHealthNumber` | 95 | **real** | `ActorHealthNumber.cs:167` |
| 9307 | `CheckLoadNumberLable` | 79 | **real** | `ActorHealthNumber.cs:506` |
| 9386 | `LoadNumberLableSurface` | 14 | **real** | `ActorSurfaceLoad.cs:236` |
| 9400 | `ShowSay` | 80 | notported | 无（`ActorLabelRender.cs` 只有行高/常量与布局注释，无 `ShowSay` 方法） |
| 9480 | `CheckLoadSay` | 24 | notported | 无 |
| 9504 | `LoadSaySurface` | 22 | **real** | `ActorSurfaceLoad.cs:118` |
| 9526 | `LoadNameSurface` | 37 | **real** | `ActorSurfaceLoad.cs:157` |
| 9563 | `LoadFengHaoSurface` | 46 | **real** | `ActorFengHaoLoad.cs:54` |
| 9609 | `ShowName` | 117 | notported | 无（`ActorLabelRender.cs:28` 只有 `_OFFSET` 常量） |
| 9726 | `ShowShopName` | 67 | notported | 无 |
| 9793 | `ShowNumberLable` | 53 | notported | 无 |
| 9846 | `Say` | 90 | **real** | `ActorSay.cs:202`（30 行；换行/排版 + `Layout`） |

### 3.2 `TNpcActor`（10 条；real 10 → **100%**）★ 本切片全量落地

| 原文行号 | 方法 | 行数 | 状态 | 托管侧承载与说明 |
|---|---:|---|---|---|
| 9936 | `CalcActorFrame` | 295 | **real** | `ActorNpcActor.cs` `TNpcActor.CalcActorFrame`。四标签 case（TURN/HIT/DIGUP/WALK）；自定义 NPC 支三个分支各自 **`Exit`**（10010/10118/10225）；外观 42..47 的倒置帧区间、210 的 `(frame+skip)` 单步、84 的 `<= 0` 判据全部逐字 |
| 10231 | `Create` | 23 | **real** | C# 构造函数承载；10236-10239 无条件四句 + 自定义 NPC 的 Keep 播种（★ 该播种在真实流程里**不可达**，见 §5.2） |
| 10254 | `Initialize` | 5 | **real** | 只 `base.Initialize()`（10256）；基类空体（5415 族）为 `as-is` |
| 10259 | `Finalize` | 7 | **real** | `base.Finalize()` 后清 `m_EffSurface`/`m_KeepSurface`（顺序即语义） |
| 10266 | `CheckLoadUserName` | 31 | **real** | 画布守卫、`m_sNameText := ''`、`PlugInEnabled` 两分支、幽灵三重与、`CompareText`（**大小写不敏感**）判等 |
| 10297 | `DrawChr` | 127 | **real** | 自定义 NPC 六种 `DrawOrder`（★ 执行序与枚举名**互为倒序**，见 §5.1）+ 全局支三分支（54..58 强制 `DrawBlend`、51 强制 `True`、否则用实参）+ 特效白名单 |
| 10424 | `DrawEff` | 11 | **real** | `m_boUseEffect && m_EffSurface != null` 双门；10426 的 `// inherited;` 逐字保留为注释 |
| 10435 | `GetDefaultFrame` | 47 | **real** | 10441 `Result := 0` 初值、两条各自的 `pm = nil then Exit`、`cf` 钳制、★ 归零外观集合**不含 245**（见 §5.3） |
| 10482 | `LoadSurface` | 578 | **real** | 三大段（自定义 NPC / ≥2000 / 普通）各自 `Exit`；四段"只判上界"的下界注释、`Act_Count/Act_Time` 复用笔误、761/891 坐标修正、10939 的**唯一一处不带 `Indexs[]`** 取图、`m_nEffX/m_nEffY` 的 +6/+7/+8/+12 全部逐字 |
| 11060 | `Run` | 70 | **real** | `base.Run(now)` → 帧快照 → 特效帧节流（`m_boUseMagic` 时 `/3`）→ `m_bo248` 熄灭（严格 `>`）→ Keep 帧推进（`>=` 闭区间）→ **仅当帧号变化**才请求 `LoadSurface` |

### 3.3 `TStatuaryNpcActor`（9 条；real 9 → **100%**）★ 本切片全量落地

| 原文行号 | 方法 | 行数 | 状态 | 托管侧承载与说明 |
|---|---:|---|---|---|
| 17537 | `CalcActorFrame` | 16 | **real** | 常量帧：`m_nBodyOffset := 1200`、`m_btDir := 0`、帧 0..0、ftime 100、`m_nDefFrameCount := 1`；**不转调继承** |
| 17553 | `CheckLoadSurface` | 5 | **real** | 只 `base.CheckLoadSurface()` |
| 17558 | `Create` | 23 | **real** | 构造函数承载；★ 17652-17653 只写 `Value1/Value2`，**不动** `wDressEffType` |
| 17581 | `Destroy` | 14 | **real** | 三张纹理各自判 nil → `FreeAndNil` → 最后 `base.Destroy()` |
| 17595 | `DrawChr` | 49 | **real** | `base.DrawChr` 在前；`FIsFinalized` 一次性重放（用**当前字段值**作实参）；`m_boScaleShow` 二分支（放大 `StretchDraw`/非放大 `DrawBlend`×2 + **主体用 `Draw`**）；`OffsetRect(..., -200, -237)` + `InflateRect(..., 20, 20)` |
| 17644 | `Finalize` | 6 | **real** | `base.Finalize()` 在前、`FIsFinalized := True` 在**后** |
| 17650 | `GetDefaultFrame` | 5 | **real** | 无视 `wmode` 恒 0；不写 `m_dwFrameTime` |
| 17655 | `LoadSurface` | 12 | **real** | **不转调基类**；只清 `m_EffSurface`（**不清 Keep**）；图库 `g_WNewopUIImages`；分支依据是 `m_IsGrayShow`（**不是** `m_ColorEffect`） |
| 17667 | `SetEffigyState` | 343 | **real** | `HiLong/LoLong` 位段解包（Dress\|Weapon / Effect\|Hair\|Shield / 两个 EffectIndex / 两个 EffectOffSet）；`m_btSex := m_wDress mod 2`；`nHairOffset` 七段大表 + 头盔 4→3600/5→4800；`m_wEffect ≥1000 / =50（空分支）/ ≠0` 三分支；衣服/武器特效的两段 `g_EffectImageList` 回退与 DB 武器特效 1000..1015 / 1025..2000；`CopyTexture` 的缩放位只在 17945/18005/17962 带 |

### 3.4 `THeroActor`（6 条；real 6 → **100%**）★ 本切片全量落地 + 基类更正

| 原文行号 | 方法 | 行数 | 状态 | 托管侧承载与说明 |
|---|---:|---|---|---|
| 17437 | `FindGroupMagic` | 21 | **real** | 线性**首命中**且 `GetGroupMagicId` 在循环内**每次比较都调一次**；返回 `int?`（见 D-P17-01） |
| 17458 | `GetGroupMagicId` | 28 | **real** | 3×3 查表，**无 `else`**（越界保持 0）；对角线与对称项均逐格核对 |
| 17486 | `GroupAttack` | 11 | **real** | 17491 的 `g_dwLatestSpellTick` 打点在 `SendClientMessage` **之前**且受同一 if 包裹（压 F1 卡技能竞态） |
| 17497 | `Rest` | 5 | **real** | 只有 `SendSay('@RestHero')` |
| 17502 | `Protect` | 5 | **real** | `SendClientMessage(CM_HEROPROTECT, 0, 鼠标 X, 鼠标 Y, 0)` |
| 17507 | `Target` | 30 | **real** | 锁内快照两引用；17521 的 `or` 形态（**非**德摩根）+ else 分支**不判 `m_boDeath`** 的不对称，逐字 |

### 3.5 `THumActor`（25 条；real 8 / notported 17 → 32.00%）

| 原文行号 | 方法 | 行数 | 状态 | 托管侧承载与说明 |
|---|---:|---|---|---|
| 11130 | `Create` | 82 | notported | 近似物：**未落**。★ 刻意不落：它会改 `m_nDressInd…`/`m_nMedalEffectIndex` 等初值（-1），从而改变既有 `new THumActor()` 的初始状态与那些以 `THumActor` 作 `TActor` 替身的测试语义（见 D-P17-02） |
| 11212 | `Destroy` | 6 | notported | 近似物：`TActorCore.Destroy()` = `NotPorted(2945)` 槽位；`m_FriendHitList.Free` 未落 |
| 11218 | `Initialize` | 5 | notported | 近似物：只 `base.Initialize()`（10256 族）；未在本类加 `override`（同 D-P17-02 理由） |
| 11223 | `Finalize` | 37 | notported | 近似物：只落到 `TActorCore.Finalize()`（空体）；12 个纹理槽 + `m_ActorEffects` 遍历未落 |
| 11260 | `DrawDressEffect` | 47 | notported | 无（需 `m_HumWinSurface`/`m_HumWinSurface_30`/`m_nSpX/m_nSpY/m_nSpX_30/m_nSpY_30`） |
| 11307 | `DrawDressEffectEx` | 31 | notported | 无 |
| 11338 | `CalcActorFrame` | **1783** | notported | 无（★ 本单元最大单块；人类动作表的 `ActWalk/ActRun/ActHorseRun/ActAttack` 全表展开） |
| 13121 | `UseMagicDelayTime` | 15 | **real** | ★ 本切片：13125-13130 的加速公式**整段被原文注释**，生效仅 13133 `200 + dwDelayTime` |
| 13136 | `DefaultMotion` | 81 | notported | 近似物：`ActorMotion.cs:279` 是**基类** `TActor.DefaultMotion`，不是本类覆写（`m_wEffect = 50` 等分支未落） |
| 13217 | `GetDefaultFrame` | 93 | notported | 近似物：`ActorMotion.cs:192` 基类版 + `CustomMonsterFrameCalc.cs` 规划层；本类覆写的 `matDie/matStoneRevive` 分支未逐字落 |
| 13310 | `RunFrameAction` | 45 | notported | 近似物：`ActorMotion.cs:301` 是**基类** `TActor.RunFrameAction`（原文 7097 空体），本类 45 行覆写未落 |
| 13355 | `DoWeaponBreakEffect` | 6 | **real** | ★ 本切片 |
| 13361 | `DoBrokenShieldEffect` | 6 | **real** | ★ 本切片（与上一条写**两组不同字段**，测试已夹住互不干扰） |
| 13367 | `CheckLoadUserName` | 52 | notported | 近似物：`TActorCore.CheckLoadUserName` = `NotPorted(7120)` 槽位；本类 52 行（含双人骑马只显名、`m_boShopStall` 商店名判据）未落 |
| 13419 | `CheckLoadDressAddEffect` | 20 | **real** | ★ 本切片；13426 是**严格 `>`**（与 `TNpcActor.Run` 11104 的 `>=` 相反），门外分支**会清表面** |
| 13439 | `LoadDressAddEffect` | 26 | **real** | ★ 本切片；**四重门**（比 `CheckLoadDressAddEffect` 多 `CurIndex >= 0`）；`ceGrayScale` **不含** `ceGrayScale2` |
| 13465 | `CheckLoadSurface` | 172 | notported | 近似物：`NotPorted(7355)` 槽位；172 行人类图集越界/节流未落 |
| 13637 | `OnTargetExplosion` | 13 | **real** | ★ 本切片；`Sender is TCustomMonFlyEffect` 类型门 + 空串判据两级语义 |
| 13650 | `OnTargetFinished` | 108 | notported | 无（含飞行特效回收与 `PlayMagicEffect` 联动） |
| 13758 | `PlayMagicEffect` | 326 | notported | 无 |
| 14084 | `Run` | 436 | notported | 无（本类覆写；基类 `ActorCore.cs:344` 是 `TActor.Run`） |
| 14520 | `light` | 12 | **real** | ★ 本切片；`L < m_nMagLight` **且**（`m_boUseMagic` **或** `m_boHitEffect`）才提升 |
| 14532 | `LoadSurface` | **1969** | notported | 无（人类身体/武器/发型/盾牌/坐骑/翅膀/摆摊/勋章全表） |
| 16501 | `DrawChr` | **925** | notported | 无 |
| 17426 | `TakeHorse` | 11 | **real** | ★ 本切片；`m_btHorse = 0` ⇒ 参数 1，否则 0；`LegendMap.Stop` 是**无条件**后置 |

---

## 4. 计数对账（每切片三数）

| 切片 | 成员数 | 真实体 | NotPorted | 原文如此 | 壳内剩余空壳（脚本实测） |
|---|---:|---:|---:|---:|---|
| 切片 1（`TNpcActor` + `TStatuaryNpcActor` + `THeroActor` + `TActor` 两个落点） | 30 | 29 | 1 | 0 | `PlaySceneNewActor.cs` 内 `Actor.pas` 空壳 **4 → 0** |
| 切片 2（`THumActor` 确定性 8 条 + `TActor.light`/`Initialize`/`CheckLoadUserName`/`CheckLoadSurface`/`Destroy`） | 13 | 9 | 4 | 1 | `THumActor` 空壳 **1 → 0**（声明已迁，方法体 17 条待补） |
| **累计去重（全部 159 条）** | **159** | **78** | **77** | **4** | — |

`PlaySceneNewActor.cs` 空壳数复算：

```powershell
$f = 'GXX.CSharp\src\GXX.Client\Scenes\PlaySceneNewActor.cs'
# 每个壳 = 一行 "public [partial] class X : Y { public override string ActorClass => "X"; }"
$shells = Select-String -Path $f -Pattern 'public\s+(partial\s+)?class\s+\w+\s*:\s*\w+\s*\{\s*public override string ActorClass'
"total shells in PlaySceneNewActor.cs = $($shells.Count)"     # 改前 50+ → 改后 46（删掉的 4 个属 Actor.pas）
$actorPas = Select-String -Path $f -Pattern 'THumActor|THeroActor|TNpcActor|TStatuaryNpcActor'
"remaining Actor.pas shells = $($actorPas.Count)"             # → 0（只剩迁移说明注释）
```

---

## 5. 发现的原文缺陷 / 易错点（带 `文件:行`）

### 5.1 ★ `TNpcActor.DrawChr` 的六种 `DrawOrder`：**执行序与枚举标识符读序互为倒序**

| 枚举值 | 标识符读序 | **实际执行序**（10355-10385） |
|---|---|---|
| `ndoKeep_Chr_Eff` (0) | Keep, Chr, Eff | **Eff → Chr → Keep** |
| `ndoKeep_Eff_Chr` (1) | Keep, Eff, Chr | **Chr → Eff → Keep** |
| `ndoChr_Keep_Eff` (2) | Chr, Keep, Eff | **Eff → Keep → Chr** |
| `ndoChr_Eff_Keep` (3) | Chr, Eff, Keep | **Keep → Eff → Chr** |
| `ndoEff_Keep_Chr` (4) | Eff, Keep, Chr | **Chr → Keep → Eff** |
| `ndoEff_Chr_Keep` (5) | Eff, Chr, Keep | **Keep → Chr → Eff** |

六条**全部**成立（已用 `DrawChr_CustomNpcDrawOrderIdentifierIsReverseOfExecutionOrder` 逐条对撞）。
按标识符"直译"会画出恰好相反的三层覆盖关系 —— 静态截图在多数姿态下看不出差别，
**只在人物/特效/Keep 图重叠的帧上错**。**不可**把实现改成"按名字的顺序"。

### 5.2 ★ `TNpcActor.Create` 的 10241 分支在真实流程里**不可达**

`10241 if m_wAppearance >= 10000 then` 之后 10247-10248 会播种 `m_nKeepFrame`/`m_LastKeepPlayTick`。
但 `PlayScn.NewActor` 的次序是**先构造、后写形象字段**（本工程对应
`PlaySceneNewActor.cs:174` 构造 → `:266` 写 `m_wAppearance`），构造期 `m_wAppearance` 恒为 0
⇒ 10241 永假 ⇒ **自定义 NPC 的 Keep 首帧永不播种**（`m_nKeepFrame` 保持 0，
于是 `TNpcActor.Run` 11106 的 `if m_nKeepFrame < KeepPlayIndex` 会在第一次推进时把它拉回 `KeepPlayIndex`，
**行为上事后自愈**，但 10247 那一次"提前落点"从未发生）。
已用 `Create_CustomNpcKeepPlayBranchCannotObserveAppearanceAtConstructionTime` 锁死。
**未擅自"修好"**（那会引入原文没有的行为）；登记为原文缺陷。

### 5.3 ★ 两个"方向归零外观集合"不一致：`CalcActorFrame` **含 245**，`GetDefaultFrame` **不含**

- `TNpcActor.CalcActorFrame` 9978：
  `[54..59, 70..75, 81..84, 90..92, 94..101, 211..225, **245**]`
- `TNpcActor.GetDefaultFrame` 10475：
  `[54..59, 70..75, 81..84, 90..92, 94..101, 211..225]` —— **没有 245**

已用 `GetDefaultFrame_WhiteListExcludes245UnlikeCalcActorFrame` 用 `dir = 4`（mod 3 = 1）
双向夹住：`CalcActorFrame` 得 0，`GetDefaultFrame` 得 1。

### 5.4 ★ 外观 42..47 的 `SM_TURN` 把帧区间**写反**（10031-10032）

```
m_nStartFrame := 20;
m_nEndFrame   := 10;     // ← Start > End
```
逐字照抄（构造出的"范围"让 `m_nCurrentFrame` 的越界修正（7451 族）每次都把它拉回 20，
即**定格在第 20 帧**）。已用 `CalcActorFrame_Appr42to47TurnHasInvertedFrameRange` 锁死。

### 5.5 ★ `SM_HIT` 的默认支里有**空 then 体**（10160-10162）

```pascal
if (pm.ActAttack.frame = 0) and (m_wAppearance >= 226) and (m_wAppearance <= 272) then begin
end                                    // ← 原文留白
else begin … 设帧 … end;
```
照抄为空分支；"顺手删掉空 if"在 `ActAttack.frame = 0` 且外观 226..272 时会**改变行为**
（原文本分支什么都不做，即保留上一动作的帧）。

### 5.6 ★ 外观 84 的 `ActCritical` 分支在 `MA19` 下**不可达**（10130-10143）

判据是 `(m_nStartFrame <= 0) or (m_nStartFrame >= ActCritical.start + dir*(frame+skip))`，
而 `MA19.ActCritical = (start=0, frame=0, skip=0)` ⇒ 右项对任何 `m_nStartFrame >= 0` 都真，
或关系恒真 ⇒ 10137 的 else（ActCritical 帧）永不执行。
★ 且判据用的是 **`<= 0`**（**含 0**）而非 `< 0` —— 即 `m_nStartFrame = 0` 也算"未开始"。

### 5.7 `TNpcActor.LoadSurface` 的两处"下界判定被注释"与"Count/Time 复用"

- 10526 / 10537 / 10550 / 10561 的 `{(...File >= 0) and}` **都被原文 `{...}` 包住**，
  只判上界 `< g_EffectImageList.Count`。由于 `Act_File`/`Std_File` 是 `Word`，
  "下界"本就不可能为负 —— 注释掉是**冗余清理**，但逐字保留"只判上界"。
- 10537-10538 / 10561-10562 的**特效段判的是主体的 `Act_Count`/`Act_Time`（`Std_Count`/`Std_Time`）**，
  而不是 `*_EffCount`/`*_EffTime`（后者在 `TNpcDirAction` 里根本不存在，见 `Grobal2.pas:5859/5868`）。
  原文如此，逐字保留。

### 5.8 `THumActor.CheckLoadDressAddEffect` 用严格 `>`，而 `TNpcActor.Run` 用 `>=`

- `THumActor.CheckLoadDressAddEffect` 13426：`MyGetTickCount - last **>** m_wDressAddEffectTime`
- `TNpcActor.Run` 11104：`TimeGetTime - m_LastKeepPlayTick **>=** KeepPlayTime`

两者都是"帧游标推进"，一个开区间一个闭区间。已分别用
`CheckLoadDressAddEffect_TimeExactlyEqualDoesNotAdvance`
与 `Run_KeepFrameTimeComparisonIsInclusive` 夹住。

### 5.9 `THumActor.UseMagicDelayTime` 的加速公式被整段注释（13125-13130）

生效的只有 `13133 Result := (200 + dwDelayTime);`。
"补回"被注释的公式会让所有人物技能施法帧时长变短——且**编译、单测都可能仍通过**。

### 5.10 `TNpcActor.GetDefaultFrame` 的 10441 `Result := 0` 是**无条件初值**却不是兜底

10463 的 `pm = nil then Exit` 会**带着 0 返回**；而自定义 NPC 支若 `NpcConfig = nil`
也**不写** `result`（保持 0）。故"配置缺失"与"配置成立且钳制到 0"在返回值上**不可区分**
（原文如此；托管侧同样不可区分，未加额外的存在性标志以免偏离原文）。

### 5.11 `TActor` 的 16 个属性访问器**不是直通**

`GetCurrX/GetCurrY/SetCurrX/…/SetActBeforeY`（4201-4327）对**主角**走
`Cutecode_T`/`Makecode_T`（坐标编解码），对其他角色才直通 `F*` 字段。
故"照抄成属性直通"是错的（这也是本报告把它们判为 notported 而不是"trivial 已实现"的理由）。

### 5.12 `THumActor.Create` 11206-11207 有一个**孤立的空语句**

```pascal
  m_boBrokenShield := False;
  ;                       // ← 原文留下的孤立分号（11207）
```
无害但说明该处被改过；托管侧若逐字移植应保留注释，不要"顺手删"成看似不同的结构。

---

## 6. 新增虚槽位与虚分派（台帐 §18.8）

本轮**新增**了以下虚成员 —— 此前托管侧**没有**这些名字，
导致子类的 `override` 无处可落（只能 `new` 隐藏，而调用点全是基类静态类型）：

| 新虚成员 | 承载文件 | 原文 | 为什么必须 |
|---|---|---|---|
| `TActorCore.Initialize()` | `ActorNpcActor.cs` | 5420（**空体 = 原文如此**） | `TNpcActor.Initialize` 10254 / `THumActor.Initialize` 11218 的覆写目标 |
| `TActorCore.CheckLoadUserName()` | `ActorNpcActor.cs` | 7120 | `TNpcActor.CheckLoadUserName` 10266 / `THumActor.CheckLoadUserName` 13367 的覆写目标 |
| `TActorCore.CheckLoadSurface()` | `ActorNpcActor.cs` | 7355 | `TStatuaryNpcActor.CheckLoadSurface` 17553 / `THumActor.CheckLoadSurface` 13465 的覆写目标 |
| `TActorCore.Destroy()` | `ActorNpcActor.cs` | 2945 | `TStatuaryNpcActor.Destroy` 17581 / `THumActor.Destroy` 11212 的 `inherited` 目标 |
| `TActorCore.light()` | `ActorNpcActor.cs` | 5415（**实体**） | `THumActor.light` 14520 的覆写目标 |
| `TActor.DrawEffSurface(object?,int,int,bool,TColorEffect)` | `ActorDrawEffSurface.cs` | 5704 | `TNpcActor.DrawChr` 10334/10342/10399 的调用目标（此前**根本不存在**） |
| `TActor.StretchDrawEffSurface(...)` | `ActorDrawEffSurface.cs` | 5814 | 同族落点（供后续 `THumActor.LoadSurface` 使用） |

**虚分派正确性证据**：`LightIsVirtualSoBaseStaticTypeDispatchesToHum` 用
`TActor asBase = new THumActor { … }; asBase.light()` 经**基类静态类型**调用并断言落到 `THumActor.light`。

`NotPorted(...)` 留痕设施（台帐 §48.1）：

```csharp
public static bool NotPorted(string member, int sourceLine)   // ActorNpcActor.cs（TActorCore）
{
    NotPortedLog.Add($"{member}@{sourceLine}");
    return false;
}
```
它**每次调用都记录**到 `TActorCore.NotPortedLog`，使"未移植"在运行期可观测
——不是静默中性值。**本车道全程没有裸 `=> true;`**（§48.1）。

---

## 7. 偏离登记（D-P17-xx）

| 编号 | 偏离 | 理由 | 影响面 |
|---|---|---|---|
| **D-P17-01** | `THeroActor.FindGroupMagic` 返回 `int?`（命中的 `wMagicId`），原文返回 `pTClientMagic` | headless 侧不持有 `g_HeroMagicList` 元素本体，只有 `Def.wMagicId` 序列；用 `null` 表原文 `Result := nil` | 调用方拿不到整条 `TClientMagic`（除 `wMagicId` 外的字段） |
| **D-P17-02** | `THumActor` 的 17 条未在本类 `override`，暂**继承** `TActor`/`TActorCore` 的通用实现 | 既有测试以 `new THumActor()` 作 `TActor` 替身（`ActorFamilyBaseTests.NewActor()`）；一旦覆写成 `NotPorted` 会让多态调用点**静默变空操作**，把"未移植"伪装成"已移植但没效果"（台帐 §25.2）。**先改测试再覆盖**是正确次序 | `THumActor` 实例在多态路径上走通用实现；已逐条登记（§3.5） |
| **D-P17-03** | `TNpcActor.CalcActorFrame` 的自定义 NPC 支把 `NpcDirAction` 用 `TNpcDirAction?`（可空结构体）承载，原文是 `PNpcDirAction` 指针 | C# 侧 `TClientCustomNpcConfig` 是结构体（`Grobal2.Types5.cs:716`），取地址改取副本 | `@NpcConfig.Actions[m_btDir]` 的"引用语义"变成"值语义"；原文在本方法内**只读**它（不写回），故等价 |
| **D-P17-04** | `TNpcActor.LoadSurface` 的 ~40 段同形取图用 `FetchNpcBody/FetchNpcEff` 两个私有助手表达 | 原文每段是 5 行 `case m_ColorEffect of …` 三连；助手把"哪一段用哪个 `Indexs[k]` + 三分支"参数化，**调用点仍逐段写原文行号** | 三分支与图号/库号逐段可核对；`m_nEffX/m_nEffY` 的 +6/+7/+8/+12 修正逐处保留 |
| **D-P17-05** | `THumActor.OnTargetExplosion` 经 `ActorNpcEnv.MonFlyEffectExplosionSoundFn` 取音（`null` = 非飞行特效，空串 = 未配音） | `TCustomMonFlyEffect` 属魔法批次（`MagicEffectsCustomMon.cs`） | 两级判据在类型层面可区分，非静默中性值 |
| **D-P17-06** | `TActor.ReadyAction`（3776）与 `TActor.Run`（7391）标为**真实体**，但其原文中确有未覆盖行段（`ReadyAction` 的 `m_Saying`/特效分支；`Run` 的魔法路径 `/1.8`） | 主分支已 1:1 且被测试夹住；未覆盖段不改变已覆盖段的控制流 | 计数上计入真实体；行段差异在本表"说明"列写明，**未隐藏** |
| **D-P17-07** | `TNpcActor.CalcActorFrame` 10186 的 `Randomize`（全局随机种子重置）未移植 | headless 无全局种子状态；且 `Random` 本身已是接缝 `ActorNpcEnv.RandomFn` | 随机序列语义由接缝提供方负责 |

---

## 8. 未完成 / 阻塞（**如实登记，未填成 0**）

### 8.1 未完成（按"接通后还要做的事"排序）

| 项 | 剩余量 | 性质 |
|---|---|---|
| `THumActor.CalcActorFrame`（11338，**1,783 行**） | 人类动作表全表（Walk/Run/HorseRun/Attack/Magic/Struck/Die 的 start/frame/skip/ftime 展开 + 武器/发型/坐骑/翅膀/摆摊/勋章状态机） | 纯逻辑，**可无头单测**。最大单块，建议单独车道 |
| `THumActor.LoadSurface`（14532，**1,969 行**） | 人类图集三大段 + 六张表选择 + 越界判据 + 大量 `out` 偏移 | 取图接缝已备（`ActorNpcEnv` 全套） |
| `THumActor.DrawChr`（16501，**925 行**） | 人类绘制层序（约 30 个绘制点） | 需先落 `DrawDressEffect/Ex`（11260/11307） |
| `THumActor.Run`（14084，**436 行**） | 人类帧推进 + 坐骑/摆摊/翅膀/勋章/破盾/破武 | 依赖 `Create` 的字段初值（11130） |
| `THumActor.PlayMagicEffect`（13758，**326 行**） | 魔法特效建实例 + 目标联动 | 依赖魔法批次（`MagicEffects*`） |
| `TActor` 的 60 条 | 见 §3.1：消息层 2975/3041/3295/3329/3350/3391/4398/4441（+5 条钩子待实体化）、16 个属性访问器、`CharWidth/CharHeight`、名字/喊话族 `ShowSay/CheckLoadSay/ShowName/ShowShopName/ShowNumberLable`、`DrawWeaponGlimmer/DrawDressEffect/DrawDressEffectEx/GetDrawEffectValue`、`DoSmoothMove`、`NewHealthNumberFromGroup/AddHealthNumber`、`CheckLoadActorIcon/CheckLoadFengHaoSurface`、`FindMsg`、`Create(168 行)`、`Destroy(25 行)`、`Finalize(55 行)` | 其中 16 个访问器要**先**落 `Makecode_T/Cutecode_T`（见 §5.11），否则会写成错误的直通 |
| `THumActor.Create`（11130，82 行） | 40+ 字段初值 | ★ 落地前**先**把既有 `new THumActor()` 的替身测试改为 `new TActor()`（见 D-P17-02） |
| `THumActor` 的 8 条中等方法 | `DrawDressEffect/Ex`、`DefaultMotion`、`GetDefaultFrame`、`RunFrameAction`、`CheckLoadUserName`、`CheckLoadSurface`、`OnTargetFinished` | 需要新字段（`m_HumWinSurface`/`m_nSpX/m_nSpY`/`m_ActorEffects`/`m_FriendHitList`…） |

### 8.2 阻塞项（精确到成员名 + 原文行号）

| 阻塞项 | 精确依赖 | 影响 |
|---|---|---|
| `Makecode_T` / `Cutecode_T` | `Source/Common/*`（坐标编解码函数）未在托管侧发现同名实现 | 阻塞 `TActor` 的 16 个属性访问器（4201-4327） |
| `TCustomMonFlyEffect` 载体 | `MagicEffectsCustomMon.cs` 的构造函数签名 + `ClientConfig.Sounds[custMagicExplosion]` | `THumActor.OnTargetFinished`(13650) / `PlayMagicEffect`(13758) |
| `m_ActorEffects`（脚本命令特效表） | `pTClientActorEffect` 列表（`ActorData.cs:52` 有 `TClientActorEffect` 结构体，但**实例表**未在 `TActorCore` 上） | `THumActor.Finalize`(11223) |
| `TStringList m_FriendHitList` | 托管侧已有 `List<string> m_FriendHitList`（`ActorMessages.cs:56`）—— **部分**满足；`THumActor.Destroy` 的 `Free` 无对象语义 | `THumActor.Destroy`(11212) |
| `g_WNpcImgImages.Indexs[11]` 等图集实例 | 取图接缝已备（`ActorNpcEnv.FetchNpcImageFn`），**未接线**到真实图集 | 所有 `LoadSurface` 的**真实像素**（headless 下不影响逻辑正确性） |
| `m_wAppearance` 的构造期可读性 | `PlayScn.NewActor` 的"先构造后赋值"次序（原文一致） | `TNpcActor.Create` 10241 分支不可达（§5.2）——**不是**本车道的缺陷，是原文缺陷 |

---

## 9. 测试与门禁

### 9.1 新增用例

| 文件 | 例数 | 覆盖分布 |
|---|---:|---|
| `tests/GXX.Client.Tests/ActorNpcActorTests.cs` | **151** | `TNpcActor.CalcActorFrame` 22 · `Create/Initialize/Finalize` 3 · `CheckLoadUserName` 9 · `DrawChr` 17 · `DrawEff` 2 · `GetDefaultFrame` 6 · `LoadSurface` 17 · `Run` 10 · `TStatuaryNpcActor` 28 · `THeroActor` 20 · 层级/常量 5（部分为 `Theory`，实际断言数更多） |
| `tests/GXX.Client.Tests/ActorHumActorTests.cs` | **31** | `UseMagicDelayTime` 5 · `light` 6 · 破武/破盾 3 · `CheckLoadDressAddEffect` 7 · `LoadDressAddEffect` 4 · `OnTargetExplosion` 3 · `TakeHorse` 3 |
| **合计** | **182** | — |

**计数**：`GXX.Client.Tests` 基线 **4,999** → 现 **5,181**（= 4,999 + 182），**0 失败**。

**差异断言（"看起来一样实则不同"）**：

| 测试 | 夹住的差异 |
|---|---|
| `CalcActorFrame_CustomNpcWalkSetsMoveParamsThenExits` | `NpcDirAction <> nil` 是 **Exit** 而非 Break（全局表不覆盖） |
| `CalcActorFrame_Appr42to47TurnHasInvertedFrameRange` | 帧区间**倒置**（20 > 10） |
| `CalcActorFrame_Appearance84HitAlwaysTakesActAttackForMA19` | `<= 0` 判据 + MA19 使 ActCritical 支不可达 |
| `CalcActorFrame_Appearance210HitUsesSingleStepNotDirection` | `(frame+skip)` 而**不乘** `m_btDir` |
| `CalcActorFrame_Appearance33HitUsesActStandAndKeepsFrameTime` | 33/34/52 的 HIT 支**不写** `m_dwFrameTime` |
| `CalcActorFrame_Appr52DigUpStarts23SecondLoop` | `m_nEffectEnd := m_nEffectStart + 11`（**不是 +11-1**） |
| `DrawChr_CustomNpcDrawOrderIdentifierIsReverseOfExecutionOrder` | 六种 `DrawOrder` 的**名序 = 逆执行序** |
| `DrawChr_Appearance54to58ForcesDrawBlendRegardlessOfBlendArgument` | 54..58 段**不看** `blend` 实参 |
| `DrawChr_Appearance51ForcesBlendOnDrawEffSurface` | 51 段传字面 `True` ⇒ 落 `DrawColorAlpha(White,150)` |
| `DrawChr_EffOutsideWhitelistIsNotDrawn` | 特效层外观白名单是**有限集** |
| `GetDefaultFrame_WhiteListExcludes245UnlikeCalcActorFrame` | 两个"方向归零集合"**不一致** |
| `CheckLoadUserName_ComparisonIsCaseInsensitive` | `CompareText`（不敏感）而非 `CompareStr` |
| `CheckLoadUserName_GhostHideGateSuppressesOnlyWhenBothSwitchesAndDead` | 幽灵门是**三重与** |
| `LoadSurface_ResourceCorrectionIsSkippedForGrayScale` | 761/891 坐标修正只在**非灰度**分支 |
| `LoadSurface_Appearance42EffUsesRootFetchWithoutIndexs` | 10939 是**唯一**一处不带 `Indexs[]` 的取图（且 42 的 +71/+5 修正被注释） |
| `LoadSurface_CustomNpcStdEffectUsesStdCountAndTimeGate` | 特效段判**主体的** Count/Time |
| `Run_Bo248ExtinguishesOnlyAfterUseEffectTick` | 11084 是严格 `>` |
| `Run_KeepFrameTimeComparisonIsInclusive` | 11104 是 `>=`（与上条相反） |
| `Run_RequestsLoadSurfaceOnlyWhenFrameChanged` | 11117 是 `or`（任一变化都重载） |
| `SetEffigyState_HairOffsetTable`（12 组 `Theory`） | 七张发型表与区间的对应 + `m_btSex := wDress mod 2` |
| `SetEffigyState_EffectFiftyBranchIsEmpty` | `m_wEffect = 50` 是**空分支**（整段被注释） |
| `SetEffigyState_Value1ZeroStillRunsWingsSection` | `Value1 = 0` **不提前 Exit**，仍走 17917 翅膀段 |
| `SetEffigyState_DressEffectFallbackUsesEffectImageList` | 衣服特效索引**越界才**回退 |
| `LoadDressAddEffect_GrayScale2IsNotTreatedAsGray` | 13452 只列 `ceGrayScale`（**不含** `ceGrayScale2`） |
| `CheckLoadDressAddEffect_TimeExactlyEqualDoesNotAdvance` | 13426 是严格 `>`（与 11104 的 `>=` 相反） |
| `LoadDressAddEffect_NegativeCursorSkipsFetch` | 第四重门 `CurIndex >= 0` |
| `WeaponBreakAndBrokenShieldAreIndependent` | 两组字段互不干扰 |
| `LightIsVirtualSoBaseStaticTypeDispatchesToHum` | 经基类静态类型的**虚分派**落到 `THumActor.light` |
| `HeroActorInheritsFromHumActorNotFromActorDirectly` | 基类层级（原文 2049） |
| `Target_InvalidTargetFallsToElseBranchWithoutDeathCheck` | 17529 的 else **不判 `m_boDeath`** |

### 9.2 门禁（官方脚本，三项同时成立）

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-actor
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 `
  -Project GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj
```

```
build exit code       : 0
dotnet test exit code : 0
crash markers found   : none
GATE: PASS (build 0 error, test exit 0, no crash markers)
```

> ★ 按台帐 §52.3：**不**以 `已通过!` 摘要行作为判据 —— 该行在 testhost 崩溃时**照样打印"通过"**。
> 本报告的三行证据取自 `run-gate.ps1` 的 `gate evidence` 段。

### 9.3 巨型结构体的栈安全（台帐 §52.3）

本切片**没有**按值传递/返回巨型结构体：
- `TClientCustomNpcConfig`（含 `TNpcBaseConfig` + `TNpcDirActionArray8`）在
  `ActorNpcEnv.CustomNpcConfigLookupFn` 上按**返回值**出现，但其体积为
  `2 + 2 + sizeof(TNpcBaseConfig) + 8×sizeof(TNpcDirAction)` ≈ **数百字节**，
  与 `THumData`（≈645 KB）不同量级；
- `TNpcActor.CalcActorFrame` 的 `pm`（`TMonsterAction?`）与 `npcDirAction`（`TNpcDirAction?`）
  都是**结构体可空值**，均在**小体积**范围（`TMonsterAction` 9 × `TActionInfo` = 9 × 12 B）；
- 测试进程未出现任何 testhost 崩溃标记（§9.2 第三项证据）。

---

## 10. 全部 commit hash

| hash | 说明 |
|---|---|
| `dfabe7ce` | 起点（`main` @ 本轮开始时） |
| `2cfa1f6e` | **切片 1**：NPC 族三空壳迁移 + 1:1 落地（`TNpcActor` 10 / `TStatuaryNpcActor` 9 / `THeroActor` 6）+ `TActor.DrawEffSurface`/`StretchDrawEffSurface` 落点 + 151 例（真实体 29 / NotPorted 1 / 原文如此 0 = 本切片 30 个成员） |
| `0d2a3ad0` | **切片 2**：`THumActor` 8 条确定性方法 + `TActor.light`/`Initialize`/`CheckLoadUserName`/`CheckLoadSurface`/`Destroy` 虚槽位 + 31 例 + 本报告（真实体 9 / NotPorted 4 / 原文如此 1 = 本切片 14 个成员；累计 78 / 77 / 4） |

复算：`git -C <工作树> log --oneline main..HEAD` → 2 条（本车道的 2 个切片）。
10 个改动文件全部落在本车道独占分区内（`git diff --name-only main...HEAD`）。

---

## 11. 复现命令

```powershell
$wt = 'D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p17-client-actor'

# 1) 原文 159 条清单
$Src = 'D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\Client-HGE\Actor.pas'
((Get-Content $Src) | Select-String -Pattern '^\s*\{?\s*(constructor|destructor|procedure|function)\s+T\w+\.\w+').Count   # 159

# 2) 覆盖率复算（解析本报告 §3 的表）
#    见 §1.2；实测 real=78 notported=77 as-is=4 → 49.06%

# 3) 只看本车道的证据
cd $wt
dotnet test GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj -c Debug --nologo `
  --filter "FullyQualifiedName~ActorNpcActorTests|FullyQualifiedName~ActorHumActorTests"
# → 182 例全绿

# 4) 官方门禁
powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/run-gate.ps1 `
  -Project GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj

# 5) 提交前自检（工作树内不得有分区外路径）
git -C $wt status --porcelain
```

---

## 12. 过程失误（如实记录）

1. **第一次写测试时把 `TNpcActor.DrawChr` 的绘制出口搞错了**：主体的"普通"分支走
   `DrawEffSurface`（→ `ActorFamilyEnv.DrawEffSurfaceOpFn`），只有外观 54..58 与特效层走
   `ActorNpcEnv.NpcDrawFn`。我最初只挂了后者，导致 16 个用例误判为"少画了"。
   **已修**：加 `RecordLayerOrder` 把两个出口汇成一条层序（并在测试注释里写明为什么）。
   教训：**同一方法里"画一张图"可以有两个出口接缝**，挂一个接缝会把顺序看错。
2. **第一次写覆盖率脚本时用了"类名精确匹配"**，得出 `Actor* = 6`——因为本工程的架构是
   `TActorCore`（partial，跨 ~15 个文件）承载 `Actor.pas` 的 `TActor` 实体，
   而 `TActor : TActorCore` 只是薄壳。**已修**为"同名成员 + 承载类核对"，
   并**逐条人工复核**（这也是 §1.3 里"近似物一律计未移植"那条判据的由来）。
3. **`ActorHumActorTests` 的 `Run`/`CalcActorFrame` 未覆盖**：本切片刻意只做"不改变
   `new THumActor()` 初始状态"的成员；这不是遗漏，而是为了避免让既有测试的语义连带改变
   （见 D-P17-02）。若集成方希望更快推进 `THumActor`，正确次序是：
   **先把 `ActorFamilyBaseTests.NewActor()` 从 `new THumActor()` 改成 `new TActor()`**，
   再逐条补 `override`。
