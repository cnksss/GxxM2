# 并行报告 p12-e2only-review —— 13 个巨型 E2-only 单元的逐单元裁决

- 车道：`par/p12-e2only-review`（只读复核）
- 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p12-e2only-review`
- 基线：`main` = `9204b73f`（调度第 55 轮）
- 权限：本车道**未修改任何** `src/**`、`tests/**`、`tools/**`、`GXX.slnx`、`*.csproj`、其它 `docs/*.md`；唯一产物是本文件。**未运行 `dotnet build` / `dotnet test`**。
- 全部结论来自**只读静态分析**（原文抽取 + C# 声明/实现检索 + 抽样逐行比对）。

---

## 0. 一句话结论

**本切片 13 个单元、270,731 行里：A（真移植）= 0 个；B（部分移植）= 2 个（26,347 行）；C（未移植 / 仅借名）= 11 个（244,384 行）。**

这 13 个单元全部**没有任何一个**能通过严口径的 A：它们的 E2 提及分别来自
**跨单元证据文件（`*Core.cs` 的恒真断言）**、**生成的声明壳（`*.g.cs` 全部 `throw`）**、
**成员容器片（`PlayerSurface/*`）**、或**另一个独立框架（`NpcScriptCommands.cs` 的命令派发器）**——
**没有一处是本单元的代码翻译**。

> **对 `mapped` 数字的直接影响（建议）**：本切片 11 个 C 单元合计 **244,384 行**应从 `MAPPED` 移出；
> 其中 `ObjPlayer`/`NpcActionCmd`/`NpcConditionCmd`/`HandleCommands`/`ObjHero`/`ObjMon`/`StateWindows`
> 7 个单元（**174,113 行**）按口径边界（"被共享框架/运行时等价取代"归 not-ported）应移入 **not-ported**；
> `M2Share`/`FState`/`Actor`/`MShare` 4 个单元（**70,271 行**）"部分移植"，
> 应移出 MAPPED 并**开（或并入）实现车道**，不得留在 MAPPED。
> 详见 §3。

---

## 1. 方法与可复跑命令

### 1.1 原文抽取口径（严口径）

```powershell
# 13 个单元的源文件（注意：镜像里 FState/StateWindows/FunctionConfig 在子目录）
$M = 'D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror'
#   M2Engine/ObjPlayer.pas        -> $M\M2Engine\ObjPlayer.pas
#   M2Engine/NpcActionCmd.pas     -> $M\M2Engine\NpcActionCmd.pas
#   M2Engine/M2Share.pas          -> $M\M2Engine\M2Share.pas
#   Client-HGE/FState.pas         -> $M\Client-HGE\GUI\Share\FState.pas
#   Client-HGE/Actor.pas          -> $M\Client-HGE\Actor.pas
#   M2Engine/FunctionConfig.pas   -> $M\M2Engine\Forms\FunctionConfig.pas
#   M2Engine/ObjHero.pas          -> $M\M2Engine\ObjHero.pas
#   Client-HGE/StateWindows.pas   -> $M\Client-HGE\GUI\NewStateWin\StateWindows.pas
#   Client-HGE/MShare.pas         -> $M\Client-HGE\MShare.pas
#   M2Engine/ObjNpc.pas           -> $M\M2Engine\ObjNpc.pas
#   M2Engine/NpcConditionCmd.pas  -> $M\M2Engine\NpcConditionCmd.pas
#   M2Engine/ObjMon.pas           -> $M\M2Engine\ObjMon.pas
#   M2Engine/HandleCommands.pas   -> $M\M2Engine\HandleCommands.pas

$lines   = [System.IO.File]::ReadAllLines($pasPath, [System.Text.Encoding]::UTF8)
$implIdx = 0
for ($i=0; $i -lt $lines.Count; $i++) { if ($lines[$i] -match '^\s*implementation\s*$') { $implIdx = $i; break } }

# (a) 类声明数
#     ^\s*(T\w+)\s*=\s*class
# (b) 类方法（去重后按 "类.方法" 计）
#     ^\s*(function|procedure|constructor|destructor)\s+(T\w+)\.(\w+)
# (c) 单元级裸例程（仅 implementation 之后，避免把 interface 段声明当实现）
#     ^\s*(function|procedure)\s+(\w+)\s*[\(;:]
```

**两个必须写下来的抽取修正（我第一版踩过）**：

1. **类方法必须按 `类.方法` 去重，不能按"方法名"去重**。
   按方法名去重会把 `TMonster.Run` / `TChickenDeer.Run` / `TActor.Run` … 全部并成 1 个 `Run`，
   于是 `ObjMon.pas` 的 **204** 条实现被压缩成 **54** 个名字，覆盖率被凭空放大。
2. **裸例程只在 `implementation` 之后抽**。`HandleCommands.pas`/`NpcActionCmd.pas`
   在 `interface` 段有大量 `procedure` 前置声明，把它们当实现是重复计数。

### 1.2 托管侧判据（三档，缺一不可）

```powershell
$W   = 'D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p12-e2only-review\GXX.CSharp'
$all = Get-ChildItem "$W\src","$W\tests" -Recurse -Filter *.cs |
       Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }        # 1,316 个 .cs

# (E1) 同名 .cs —— 本切片 13 个单元【全部不成立】（见 §5.4）
# (E2) 任一 .cs 的【头 40 行】内出现 '<unit>.pas'
#      => 注意：E2 只证明"某文件声称自己有这个源"，【不证明本单元的实体被移植】
# (D)  声明匹配：\b(class|record|struct|interface|enum)\s+<类名>\b   —— 不用子串
# (R)  例程反向覆盖：在 E2 文件集合里找与原文例程【同名】的方法【声明】（非注释提及）
#      ^\s*(public|private|protected|internal|static|virtual|override|sealed|async|partial|new|\s)*
#        [\w\<\>\[\]\?\.\,]+\s+<例程名>\s*\(
```

**三档的意义**（本报告的核心方法论）：

| 档 | 含义 | 能不能算"已移植" |
|---|---|---|
| 名字出现在**注释/文档**里 | 只是被引用作佐证 | **不能** |
| 名字出现在**恒真断言**（`=> true;`）里 | 该文件的用途是"证据/可测试断言"，不是实现 | **不能** |
| 名字是**方法声明**（有签名、有体） | 才是代码翻译 | 能 |

实测：`M2Server` 的 **146 个 `*Core.cs` 中 143 个含 `=> true;`**，全仓 `src` 共 **4,726 处** `=> true;`。
所以"某单元名出现在 `*Core.cs` 里"这一条**单独不可用**。

### 1.3 本报告用到的可复跑脚本

分析脚本我落在 `%TEMP%\p12\` 下（**未写进仓库**，因为车道只允许写一个报告文件）：
`tables.ps1`（类/例程抽取）、`e2.ps1`（E2 提及点）、`revcov3.ps1`（反向覆盖）、
`verify.ps1`（把"carried"逐个回验成**方法声明**而非注释）。
`revcov3.ps1` 的输入输出摘要已逐字抄进 §5，任何一条都能用 §1.1/§1.2 的命令重跑复核。

---

## 2. 13 行总表

口径：
- **例程总数** = (a) 类方法按 `类.方法` 去重 **+** (b) `implementation` 段单元级裸例程按名去重。
- **名面命中** = 在 E2 文件集合里能找到同名方法**声明**的例程数（**含通用名假阳性**，故是上界）。
- **已移植例程数** = **人工复核后**认定的真移植数；`—` 表示该单元判 C，不主张任何例程级移植（并非"恰好为 0"）。

| 单元 | 行数 | 类声明数 | 例程总数 | 裁决 | 已移植例程数 | 名面命中（上界） | 承载文件 | 一句话证据 |
|---|---|---|---|---|---|---|---|---|
| `M2Engine/ObjPlayer.pas` | 49,232 | **2**<br>`TPlayObject`(21)<br>`TWarrContinueHitManager`(1398) | **856**<br>(807 类 + 51 裸) | **C** | —（19） | 19 (2.2%) | 仅成员容器：`Engine/PlayerSurface/TPlayObject.*.cs`、`Engine/RecalcBonus.cs`、`Engine/ObjBase.cs` | 无 `TWarrContinueHitManager` 声明；19 处命中全是**孤立存取器/金额通知**，`TPlayObject` 的 807 条实现（`Operate`/`Run`/`ClientXxx` 整族）**无一条** |
| `M2Engine/NpcActionCmd.pas` | 47,018 | **0** | **737**<br>(全裸例程) | **C** | —（2） | 2 (0.27%) | 无（E2 来自 `FbEnterCore.cs` 两个**嵌套函数**） | 737 个 `ActionOf*` **无一**在托管侧；E2 那处只是 `ActionOfCreateEctype`/`ActionOfMoveEctype` 的**内嵌** `CanEnterToMap`/`DoEnterToMap` |
| `M2Engine/M2Share.pas` | 33,605 | **2**<br>`TNotifyThread`(19)<br>`TSortItemList`(175) | **314**<br>(13 类 + 302 裸，1 同名) | **C** | —（约 22） | 25 (8.0%) | `Engine/M2ShareFuncs.cs`、`Engine/ClientModuleList.cs`、`Forms/DummySetting/DummySettingGlobals.cs`、`Forms/GamePets/GamePetsState.cs`、`Engine/MissionPageState.cs`、`Engine/SkillPowerItemList.cs` | 全单元是**单元级函数库 + 全局配置**；命中 25 条里 3 条是 `Add`/`Clear` 通用名；**本单元真正的函数面（数百条）无声明**；E2 的 89 个提及文件绝大多数是"用了某个全局量"的佐证注释 |
| `Client-HGE/FState.pas` | 25,165 | **18**<br>`TFrmDlg`(311) 等 | **367**<br>(365 类 + 2 裸) | **C** | —（约 31） | 330 名面 / **43 真** | `GUI/Share/TFrmDlg.Core.cs`（**20 条**）、`FStateTypes.cs`(17)、`FStatePure.cs`(4)、`GUI/Mir/MirForms.cs`(4) | `TFrmDlg.Decl.g.cs` 的 **515 条全部是 `throw` 桩**（`gen=True`），291 条"命中"是**生成壳的假阳性**；真实现只有 `TFrmDlg.Core.cs` 那 20 条（`FindMagicButton`/`ShowMDlg`/`IsInputChatEdit`/`ShowChatEdit`/`ClearScreenMagicButtons`/`SaveMagicButtons` …） |
| `Client-HGE/Actor.pas` | 18,009 | **5**<br>`TActor`(1298)<br>`TNpcActor`(1877)<br>`TStatuaryNpcActor`(1903)<br>`THumActor`(1935)<br>`THeroActor`(2049) | **136**<br>(156 类去重名 + 14 裸 = 170 条实现) | **C** | —（约 43） | 50 (36.8%) | `Scenes/ActorMotion.cs`(12)、`ActorMessages.cs`(10)、`ActorFamilyBase/Impl.cs`、`ActorCore.cs`、`Actor*Render*.cs` | `TActor` 的动画/动作判定族（`CalcActorFrame`/`CanWalk`/`CanRun`/`GetDefaultFrame`/`DrawChr`/`LoadSurface`…）确有 1:1 本体，但 `THumActor`/`THeroActor`/`TNpcActor` 在 `Scenes/PlaySceneNewActor.cs:488-571` 只是 **`ActorClass` 字符串壳**，且该文件的头部源注释写的是 **`PlayScn.pas`**（见 §5.3） |
| `M2Engine/FunctionConfig.pas` | 15,801 | **1**<br>`TfrmFunctionConfig`(17) | **926**<br>(925 类 + 1 裸) | **B** | **142 / 926 (15.3%)** | 142 | `Forms/FunctionConfigForm.cs`（3,034 行） | **唯一有逐行比对确证的 1:1 真移植**：`RefGeneral`(5149-5160) 与托管实现逐行一致；142 个 `*Click`/`*Change` 事件处理器在册。**其余 784 条（Skill/UpgradeWeapon/Master/MonUpgrade/HeroOption/OffLine/MyShop… 各族）未落地** |
| `M2Engine/ObjHero.pas` | 14,664 | **1**<br>`THeroObject`(13) | **153**<br>(134 类 + 19 裸) | **C** | —（0） | **0 (0%)** | 无 | `THeroObject` **无声明**；`AllowHeroMagicRate`/`HeroTaosAttackTarget`/`HeroThink`/`MakeSaveRcd`/`DoMotaebo100`/`ContinueousAttack`/`HeroAvoidTarget`/`HeroAttackTarget`/`GroupAttackProcess`/`GetUserItemWeitht`/`EatUseItems` **全部 0 命中**。托管侧 `PlayerSurface.Hero.cs` 只有 5 个字段，注释自承"`THeroObject` 未切出" |
| `Client-HGE/StateWindows.pas` | 14,027 | **1**<br>`TStateWindows`(48) | **188** | **C** | —（约 5） | 6 (3.2%) | 弱：`GUI/NewStateWin/TStateWindows{Text,Meridians,Magic,...}.cs` 的**子函数**、`GUI/Mir/ClientGlobals.cs:107` | 182 条方法只存在于 `TStateWindowsSurface.g.cs`（**声明清单，`ms=0`**）；真实现 2 条（`GetGodBlessItemCaption`、`GetMeridianStateInfo`）；`ClientGlobals.cs:107` 的 `TStateWindows` 是**最小接缝**，其注释自承"未移植" |
| `Client-HGE/MShare.pas` | 13,522 | **22**<br>`TImageList`(779) 族、`TMapDesc`(1057)、`TWarrContinueHitManager`(1067) … | **244**<br>(342 条实现去重名) | **C** | —（约 10） | 15 (6.2%) | `GUI/Share/FStateSeams.cs`(7)、`GUI/Mir/ClientGlobals.cs`(3)、`Scenes/PlaySceneCore.cs`(3) | 22 个类**无一**有声明；命中 15 条中 3 条通用名；真命中是 `GetHintFontSize`/`GetHintFontStyle`/`GetHintFontStroke`（**原文在 `MShare.pas:2897` 附近**）、`GetRGB`、`DrawItemHintOldStyle` 等**零散辅助函数**；`GetRGB` 的托管实体还是接缝（`DxImageButtonEx.cs` 注释"默认恒等"） |
| `M2Engine/ObjNpc.pas` | 10,546 | **7**<br>`TConditionList`(219)、`TNormNpc`(262)、`TMerchant`(334)、`TGuildOfficial`(432)、`TTrainer`(449)、`TBoxMonster`(461)、`TCastleOfficial`(471) | **105**<br>(104 类 + 35 裸 = 139 条实现) | **B** | **67 / 112**<br>(顶层例程) | 72 | `Npc/ObjNpcMerchant.cs`(21)、`ObjNpcUserSelect.cs`(21)、`ObjNpcVars.cs`(9)、`ObjNpcLabels.cs`(9)、`ObjNpcUnitFuncs.cs`(5)、`ObjNpcClasses/Types/Conversation/GuildCastle/Persistence/BoxMonster/MerchantBuy/MerchantUpgrade.cs` | 车道的**自建登记表** `Npc/ObjNpcRoutineRegistry.cs` 逐条给出 **112 条顶层例程中 Covered=67 / Seam=4 / Missing=41**；`TNormNpc.Operate/Run/Initialize/GotoLable/GetShowName`、`TMerchant.UserSelect/Run/Operate`、`TGuildOfficial.UserSelect/DoNate/ReQuestBuildGuild`、`TCastleOfficial.UserSelect/HireGuard/HireArcher` 等 **41 条全在 Missing** |
| `M2Engine/NpcConditionCmd.pas` | 10,489 | **0** | **283** | **C** | —（0） | **0 (0%)** | 无（E2 来自 `Engine/NpcScriptCommands.cs`） | 283 条条件函数（首条 `CheckOrCreateTxtFile`，其余 `Check*` 族）**全无声明**；`NpcScriptCommands.cs` 用**自己的** `NpcCmdCodes` + lambda 重写了条件判定（同一文件头注释写明"对应 NpcConditionCmd.pas / NpcActionCmd.pas initialization 段"——是**替代**，不是翻译） |
| `M2Engine/ObjMon.pas` | 9,502 | **55**<br>`TMonster`(9)、`TChickenDeer`(25) … 全表见 `docs/ObjMon-manifest.tsv` | **204**<br>(204 类 + 16 裸 = 220 条实现) | **C** | —（0） | **0 (0%)** | 无（`Engine/ObjBase.cs:216` 的 `TMonster` 是**简化近似**） | 46 个 `ObjMon*Core.cs` 合计约 **6,300 个方法**，其中 **约 1,900 条是 `=> true;`**（`ObjMonSpitSpiderCore.cs` 单文件 70 条）；`Engine/ObjBase.cs:228` 的 `TMonster.Run()` 是 18 行简化 AI，原文 `TMonster.Run`(1121-1391) 是 **270+ 行**、含宠物拴物/镜像地图/`WalkWaitLock`/`SmartObject` 四条原文分支 |
| `M2Engine/HandleCommands.pas` | 9,151 | **0** | **168** | **C** | —（0） | **0 (0%)** | 无（E2 来自 `EnvirRangeQueryCore.cs` / `MapQueryCore.cs` 两个证据文件） | 168 个例程（`CmdChangeAdminMode`/`CmdDeleteItem`/`CmdMakeItem`/`ProcessUserLineMsg`…）**全部 0 命中**；两个 E2 文件是**恒真断言证据文件**（`taut` 45 / 32） |

**合计**：A = **0** 个；B = **2** 个（15,801 + 10,546 = **26,347 行**）；C = **11** 个（**244,384 行**）；总计 **270,731 行**。

---

## 3. 每单元一节

### 3.1 `M2Engine/ObjPlayer.pas`（49,232 行）→ **C**

**实体**：`TPlayObject`(L21, `class(TSmartObject)`)、`TWarrContinueHitManager`(L1398, `class(TObject)`)。**816 条**类/裸例程（`TPlayObject` 807 条）。

**托管侧有什么**

| 文件 | 性质 | 与 ObjPlayer 的关系 |
|---|---|---|
| `Engine/ObjBase.cs:186` | `partial class TPlayObject` | **仅 7 个字段 + 空构造 + 2 行 `Run()`**（注释："心跳：超时踢线由 UserEngine 统一处理"） |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.{Vars,VarDefaults,ScriptFields,Hero}.cs` | **成员容器片**（切片 1/4…4/4） | `m_nVal`/`m_TVal`/`m_sString`/`m_ArrayList`/`m_MyHero`/`m_sHeroName`…。文件头自注"`m_MyHero: TBaseObject`——**托管侧 `THeroObject` 与 `TBaseObject` 都未切出**" |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.Gold.cs` | **真实现（8 条）** | `IncGold`/`DecGold`/`IncGameGold`/`DecGameGold`/`GoldChanged`/`GameGoldChanged`/`GameGloryChanged`/`NewGamePointChanged` |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.NpcSession.cs` | **真实现（4 条）** | `GetScriptLabel`/`SetScriptLabel`/`GetQuestFlagStatus`/`SetQuestFlagStatus` |
| `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs` | 真实现（4 条） | `CheckItemsNeed`/`GetMaxBagCount`/`SendAddItem`/`SendDelItem`（**注意：这些其实是 `TCreature`/`TBaseObject` 面，不是 `TPlayObject` 的**） |
| `Engine/RecalcBonus.cs` | 真实现（2 条） | `RecalcAdjusBonus`(L183)、`AdjustAb2`(L161) |
| `EnvirGuardianQuestCore.cs`、`StoneMineCore.cs`、`ViewRangeMaintainCore.cs`、`Npc/ObjNpcInputSeams.cs`、`Sweep/Nations.cs` | **证据文件 / 接缝 / 别的单元** | `EnvirGuardianQuestCore.cs` 207 方法中 **59 条 `=> true;`**、命中 **0** |

**抽样逐行核对（3 条正例 + 3 条反例）**

| 例程 | 原文 | 托管 | 结论 |
|---|---|---|---|
| `RecalcAdjusBonus` | `ObjPlayer.pas:10947` | `Engine/RecalcBonus.cs:183`（注释逐字引用原文行号） | ✅ 真实现 |
| `GoldChanged` | `ObjPlayer.pas`（金额刷新） | `PlayerSurface.Gold.cs:183`、`DbLayer/M2DataDbSupport.cs:210` | ✅ 真实现 |
| `GetScriptLabel` | `ObjPlayer.pas:15216` | `PlayerSurface.NpcSession.cs:281` | ✅ 真实现 |
| `Operate` | `TPlayObject.Operate` | **无** | ❌ |
| `Run`（`TPlayObject.Run`） | 原文含完整会话/心跳/超时逻辑 | `ObjBase.cs:208-212` 仅 `base.Run()` + 一行注释 | ❌ **近似物，非翻译** |
| `TWarrContinueHitManager.*` | L1398 整个类 | **无任何声明** | ❌ |

**未移植部分**：`TPlayObject` 的 **807** 条实现里的 **约 790 条**——包括整族 `Client*` 客户端消息处理
（`ClientBagUseItem`/`ClientDealAddItem`/`ClientBuyShopItem`/`ClientGetMyShopItem`/… 约 400 条 `Client*`）、
`Operate`/`Run`/`ClearObject`/`RecalcAbilitys` 生命周期、`m_UseItems` 读写面、英雄/副将除 5 个字段外的全部逻辑。

**E2 那处提及来自哪里、是否属于本单元**：17 个文件头写 `ObjPlayer.pas`（`revcov3` 收紧到 src 后 14 个）。
其中 **9 个是本单元的真实载体（成员容器 + 金额/会话存取器）**，**5 个不是**：
`EnvirGuardianQuestCore.cs`/`StoneMineCore.cs`/`ViewRangeMaintainCore.cs`（证据文件）、
`Npc/ObjNpcInputSeams.cs`（别的单元的接缝）、`Sweep/Nations.cs`（别的单元）、
`Sweep9/Monsters/ObjRobotHostSeams.cs`（`p9-m2-monsters` 车道的临时接缝）。
**即 E2 成立，但它证明的是"某些成员被切出来了"，不是"本单元被移植了"。**

---

### 3.2 `M2Engine/NpcActionCmd.pas`（47,018 行）→ **C**

**实体**：**0 个类**，`implementation` 段 **737 个单元级 `ActionOf*` 例程**（`ActionOfCreateNpc`/`ActionOfCreateHero`/… 全表 737 条）。

**托管侧**：`Engine/NpcScriptCommands.cs`（493 行）的 6 个方法，**没有一个是 `ActionOf*`**。
该文件头注释自承："批次 I：NPC 脚本余下命令全量注册（**对应 NpcActionCmd.pas / NpcConditionCmd.pas initialization 段**）"，
即它是**按命令码重写的另一个派发器**（`CReg(NpcCmdCodes.nNC_DAYTIME, (n,u,c) => …)`），不是原文的翻译。

**抽样**（9 条，全部 0 命中）：`ActionOfCreateNpc`、`ActionOfGiveGamePetEx`、`ActionOfGotoLabel`、
`ActionOfOffLine`、`ActionOfSaveHero`、`ActionOfChangeDamageValue`、`ActionOfAddMirrorMap`、
`JmpToLable`、`GetBoxLockUpdateIndex`。

**唯一 2 条名面命中**：`CanEnterToMap`/`DoEnterToMap`（`FbEnterCore.cs:7-8`），
但它们**是 `ActionOfCreateEctype`/`ActionOfMoveEctype` 内部的嵌套函数**（原文 23044-23057 / 23217-23237），
不是 `NpcActionCmd` 的顶层例程——**属"另一个单元的等价物被登记到了这个单元名下"**。

**未移植部分**：737/737 顶层例程。**功能上脚本动作可用**（`NpcScriptCommands.cs` 注册了命令码），
但按本工程口径（§39.5/§44.5）"被共享框架/运行时等价取代"**归 not-ported，不归 MAPPED**。

---

### 3.3 `M2Engine/M2Share.pas`（33,605 行）→ **C**

**实体**：`TNotifyThread`(L19, `class(TThread)`)、`TSortItemList`(L175, `class(TObject)`) + **302 个单元级函数/过程**（`CanFilterMsg`/`GetDummyNameList`/`LoadClientModules`/…）。

**托管侧**：`revcov3` 在 81 个 E2 文件（src 内）里数到 **4,000+ 个方法**，但**名面命中只有 25 条**，
其中 `Add`/`Clear`/`UnLock` 是通用名 → 真约 **22 条**，分布在 6 个文件：
`Engine/ClientModuleList.cs`（`LoadClientModules`/`SaveClientModules`/`LoadClientBlackModules`/`SaveClientBlackModules`）、
`Forms/DummySetting/DummySettingGlobals.cs`（`GetDummyNameList`/`SaveDummyNameList`/`SaveDummyDisableMoveMap`/`SaveDummyNoActiveAttackMonList`）、
`Forms/GamePets/GamePetsState.cs`（`LoadGamePetsConfig`/`SaveGamePetsConfig`/`GetGamePetConfig`/`ClearGamePetsConfig`）、
`Engine/MissionPageState.cs`、`Engine/SkillPowerItemList.cs`、`Engine/M2ShareFuncs.cs`（`CanFilterMsg`）、
`Forms/CustomMagic/CustomMagicSeams.cs`、`Sweep/M2Locker.cs`、`Engine/M2Config.ServerValue.cs`（`LoadExp`）。

**抽样逐行核对**

| 例程 | 原文 | 托管 | 结论 |
|---|---|---|---|
| `GetDummyNameList` | `M2Share.pas:16640-16658` | `DummySetting/Globals.cs:126` | ✅ 真实现（文件头逐行列了 7 条 M2Share 例程的来源行号，**并主动标注 3 条"未移植"**） |
| `CanFilterMsg` | `M2Share.pas:3426/15517` | `Engine/M2ShareFuncs.cs:36` | ✅ 真实现 |
| `LoadClientModules` | `M2Share.pas:3430/8665` | `Engine/ClientModuleList.cs:84` | ✅ 真实现（原文有**两份**同名实现，托管只落一份——需后续裁决取哪份） |
| `SendRefMsg` | `TBaseObject.SendRefMsg`（M2Share 侧被大量调用） | **无**（49 个文件里全是**注释引用**） | ❌ 注释假阳性 |
| `TNotifyThread.*` | L19 整类 | **无声明** | ❌ |
| `TSortItemList.*` | L175 整类 | **无声明** | ❌ |

**未移植部分**：`TNotifyThread`/`TSortItemList` 两个类 + **约 290 个单元级函数/过程**（含全部配置加载
`Load*Config`、`g_Config` 家族的读写、`SendRefMsg` 族、地图/怪物列表构建等）。
`Engine/M2Config.*.cs` 那 20+ 个文件是**配置字段容器**（每文件 1-7 个方法），**不含** `M2Share.pas` 的例程体。

**E2 那处提及**：89 个文件（src 内 81 个）头写 `M2Share.pas`。**这是本切片最普遍的信号稀释**：
车道们写"用了 `M2Share.g_Config.xxx`"时都会在头注释里写 `M2Share.pas`，
于是 E2 被 89 个文件满足，而**本单元的实体只被移植了约 7%**。

---

### 3.4 `Client-HGE/FState.pas`（25,165 行）→ **C**

**实体**：**18 个类**，最大的是 `TFrmDlg`(L311)（`TFrmDlg.Decl.g.cs` 记 201 字段 + 538 方法），
其余 17 个是 `TGuildGroup`(78)/`TNpcButton`(146)/`TNpcLabel`(225)/`TImgCountDownButton`(253)/`TMagicButton`(305) 等控件族。本轮抽到 **367** 条实现（`TFrmDlg` 365 条）。

**托管侧（这是本切片最危险的一处）**

`GUI/Share/TFrmDlg.Decl.g.cs`（1,979 行，`<auto-generated>`）里 **515 条方法全部 `throw`**：

```csharp
/// FState.pas implements part of these in its implementation section; every member not yet
/// ported throws instead of silently returning a default, so an accidental call is loud.
```

——**这 515 条在"名面命中"里贡献了 291 条**（79% 的假阳性来源）。真正有体的只有：

| 文件 | 真实现条数 | 例 |
|---|---|---|
| `GUI/Share/TFrmDlg.Core.cs` | **20** | `FindMagicButton`(L455)、`ShowMDlg`、`IsInputChatEdit`(L317)、`ShowChatEdit`(L327)、`ClearScreenMagicButtons`(L359)、`AddScreenMagicButton`、`SaveMagicButtons`、`FindMagicButton`、`RefrshDStorageViewDlgText`、`UpdateGuildJoinCondition`、`OpenGuildViewMemeberInfo`、`CloseDHeroGodBlessDlg`、`GetLastHistroySendSay`… |
| `GUI/Mir/MirForms.cs` | 4 | `AddNpcMemo`(L13167)、`DLoginNewClickSound`(L576)、`ShowMDlg` 相关 |
| `GUI/Share/FStatePure.cs` | 4 | 纯函数族 |
| `GUI/Share/FStateTypes.cs` | 17 | 记录/枚举类型方法（`TGuildGroupList.Add/FindGroup/DeleteGroupByIndex` 等） |
| `GUI/NewStateWin/TStateWindowsText.cs` | 1 | 跨单元借用（`FState.pas` 的函数被落到 StateWindows 文件里） |

**抽样逐行核对（正例 3）**

| 例程 | 原文 | 托管 | 结论 |
|---|---|---|---|
| `FindMagicButton` | `FState.pas:24591` | `TFrmDlg.Core.cs:452-475`（注释写 `FState.pas:24591 function TFrmDlg.FindMagicButton(...)`） | ✅ 1:1 |
| `AddNpcMemo` | `FState.pas:13167` | `GUI/Mir/MirForms.cs`（注释写 `FState.pas:13167 AddNpcMemo(...)`） | ✅ 1:1 |
| `RefrshDStorageViewDlgText` | `23844-23850` | `TFrmDlg.Core.cs:247` | ✅ 1:1 |

**抽样反例（3）**：`DNPC_PLAYIMG_PAINT` 整族（`FState.pas` 数个 `*DirectPaint` 方法）、
`TFrmDlg.Create`（1417-1604，188 行，`TFrmDlg.Core.cs` 头部注释自承"本波次**未覆盖**"）、
`TFrmDlg.Destroy`（1644-1712，同样自承未覆盖）。
`TFrmDlg.Decl.g.cs` 里那 515 条**全部**是反例。

**未移植部分**：约 **324/367**（`FState.pas` 的整个 `TFrmDlg` 消息处理/绘制面，以及 17 个控件类）。

**E2 那处提及**：15 个文件（src 内 12 个）。**核心来源是 `TFrmDlg.Decl.g.cs` + `FStateDeclManifest.g.cs`——
两个自动生成文件，前者是 `throw` 桩、后者是名字清单（`ms=0`）。** 它们满足 E2，却**不承载一行实现**。

---

### 3.5 `Client-HGE/Actor.pas`（18,009 行）→ **C**

**实体**：`TActor`(L1298)、`TNpcActor`(1877)、`TStatuaryNpcActor`(1903)、`THumActor`(1935)、`THeroActor`(2049)。
本轮抽到 **156** 个去重方法名（176 条实现）+ 14 条裸例程。

**托管侧（真实、且是本切片质量最好的一块）**：35 个 E2 文件（src 内）。

| 文件 | 名面命中 | 抽样确认的真实现 |
|---|---|---|
| `Scenes/ActorMotion.cs` | 12 | `ActionFinished`(L101)、`CanWalk`(L137)、`CanRun`(L154)、`GetNextHitTime`(L110)、`Strucked`、`DefaultMotion`(L279)、`RunFrameAction`(L301)、`CanCancelAction`(L325)、`CleanCharMapSetting`(L349)、`MoveFail`(L370)、`CleanUserMsgs`(L437) |
| `Scenes/ActorCore.cs` | 3 | `CalcActorFrame`（注释写 `Actor.pas 3482-3774`）、`Run`(`7391-7509`) |
| `Scenes/ActorMessages.cs` | 10 | `CancelAction`(L113)、`ProcMsg`/`ProcLastMsg`/`ProcHurryMsg`、`DoMove`、`ReadyAction`(L160) |
| `Scenes/ActorFamilyBase.cs` | 6 | `LoadSurface`(L64, 5480-5593)、`DrawChr`(L94, 6067-6129)、`DrawStateEffSurface`(L108, 5654-5702)、`RunSound`、`RunActSound`(L148, 6903-7095) |
| `Scenes/ActorHealthNumber.cs` | 5 | `CheckLoadHealthNumber`(L167)、`ClearHealthNumber`(L149)、`LoadHealthNumber`(L268) |
| `Scenes/ActorSelfEffectRender.cs` | 5 | `DrawSelfEffect`(L49)、`DrawExploreItemEffect`(L327)、`DrawLockTargetEffect`(L371)、`GetNearObjectHintInfo`(L395) |
| `Scenes/ActorOffsets.cs` | 2 | `GetOffset`(L13)、`GetNpcOffset`(L396) |
| 其余 10 个（`ActorPlayEffectQueue`/`ActorSay`/`DressEffectRender`/`ActorSoundDispatch`/…） | 各 1-5 | 真实现 |

**抽样反例（★ 本报告最重要的形态发现）**：
`Scenes/PlaySceneNewActor.cs`（576 行）里 **50+ 个 `TActor` 子类声明是纯空壳**：

```csharp
public class THumActor : TActor { public override string ActorClass => "THumActor"; }
public class THeroActor : TActor { public override string ActorClass => "THeroActor"; }
public class TNpcActor : TActor { public override string ActorClass => "TNpcActor"; }
public class TLionMonster : TActor { public override string ActorClass => "TLionMonster"; }
// … 共 50+ 行同型
```

且该文件的头部源注释写的是 **`PlayScn.pas 6969-7486 NewActor 1:1`**——**不是 `Actor.pas`**。
即：**这些类名与 `Actor.pas` 同名，但它们是从 `PlayScn.pas` 的 `NewActor` 分派表里照抄类名的"壳"。**
`TStatuaryNpcActor`/`TSpiderHouseMon`/`TDragonBody` 等同理。

**未移植部分**：约 **93/136**。已移植的是"无渲染句柄依赖的判定/偏移/状态层"；
未移植的是 `THumActor`/`THeroActor`/`TNpcActor` 三族的**实际行为差**（`THumActor` 的 `DrawChr`/`GetDefaultFrame` 差异化分支、
`THeroActor` 全部、`TActor.CalcActorFrame` 的渲染相关分支）。

**E2 那处提及**：56 个文件；src 内 35 个。**但真正的实现在 `Scenes/Actor*` 这 20 个文件里**，
而它们**各自只是在头注释里写了 `Actor.pas`**；同一目录下 `PlaySceneNewActor.cs` 因为写的是 `PlayScn.pas`，
**反而没被算进 E2**——于是 E2 的"文件数"与"实现量"完全脱钩。

---

### 3.6 `M2Engine/FunctionConfig.pas`（15,801 行）→ **B（142 / 926 = 15.3%）**

**实体**：`TfrmFunctionConfig`(L17, `class(TForm)`)，`implementation` 段 **925 条类方法 + 1 条裸例程（`RefBagcount`）= 926**。

**承载文件**：`Forms/FunctionConfigForm.cs`（3,034 行，`sealed class FunctionConfigForm : System.Windows.Forms.Form`）。
文件头自注：

> `FunctionConfig.pas TfrmFunctionConfig 巨片拆分（批次J22 第一片，15801 行）：密码保护页全部处理器 … 与常规页第一组（RefGeneral 九名称颜色 + 饥饿系统 + 装备刻名前缀过滤 + 保存全键）1:1。其余页（Skill/UpgradeWeapon/Master/MonUpgrade/HeroOption/OffLine/MyShop 等）随后续巨片接入。`

**抽样逐行核对（1 条完整对照 + 3 条签名比对）**

原文 `FunctionConfig.pas:5149-5160`：

```pascal
procedure TfrmFunctionConfig.RefGeneral();
begin
  EditPKFlagNameColor.Value := g_Config.btPKFlagNameColor;
  EditPKLevel1NameColor.Value := g_Config.btPKLevel1NameColor;
  ...（共 9 行，逐字段赋值）
end;
```

托管 `FunctionConfigForm.cs:1252-1263`：

```csharp
public void RefGeneral()
{
    EditPKFlagNameColor.Value = M2Config.btPKFlagNameColor;
    EditPKLevel1NameColor.Value = M2Config.btPKLevel1NameColor;
    ...（共 9 行，逐字段赋值）
}
```

→ **逐行 1:1，仅 `g_Config` → `M2Config` 的名称改写**。✅

| 抽样例程 | 原文行 | 托管行 | 结论 |
|---|---|---|---|
| `RefGeneral` | 5149-5160 | 1252-1263 | ✅ 1:1（上面） |
| `CheckBoxEnablePasswordLockClick` | —（密码锁页） | 1267-1286 | ✅ 结构/分支/`if (!boOpened) return;` 位置一致 |
| `CheckBoxLockGetBackItemClick` | — | 1288-1294 | ✅ |
| `CheckBoxHungerSystemClick` | 5162-5181 | 见 `CheckBoxHungerSystemClick` | ✅（启用/禁用级联 + `boOpened` 守卫 + `ModValue()`） |

**已移植 142 条的构成**（逐条核验为**方法声明**，非注释）：`Button*SaveClick` 族 17 条、
`CheckBox*Click`/`chk*Click` 族 28 条、`Edit*Change` 族 60 条、`ScrollBar*Change` 族 19 条、
`se*Change` 族 5 条、`Ref*` 族 4 条（`RefGeneral`/`RefMagicSkill`/`RefUpgradeWeapon`）、`ModValue`/`uModValue`/`GridBoneFammSetEditText`、
`FunctionConfigControlChanging`、`Open`。

**未移植部分（784 条）**：
- **页级 `Ref*` 刷新器**：`RefFinalPages`/`RefOffLineMyShop`/`RefMutinyMsgLuck`/`RefReNewMonUpgrade`/`RefMasterMineLottery` 等（部分在 `FunctionConfigForm.cs` 内但**不在 926 计数里**——它们是**新增的重构函数**，不是同名例程；`RefFinalPages`/`RefOffLineMyShop`/`RefMutinyMsgLuck`/`RefReNewMonUpgrade`/`RefMasterMineLottery` 在原文里的名字不同，需下批对齐）
- **Skill 页 / UpgradeWeapon 页 / Master 页 / MonUpgrade 页 / HeroOption 页 / OffLine 页 / MyShop 页 / ReNewLevel 页 / SpiritMutiny 页 / WinLottery 页 / Stone 页 / BoneFamm 页 / MakeMine 页 / QueryBagItems 页 / MakeDrug 页 / BBMon 页 / FireCross 页 / SwordLong 页**的**全部**控件初始化（`FormCreate` 里数千行控件创建）与剩余 handler

**E2 那处提及**：8 个 src 文件。其中 `Engine/M2Config.Function*.cs` 6 个是**配置字段容器**（每文件 1-2 个方法），
只有 `Forms/FunctionConfigForm.cs` 是真实载体。**E2 成立且名副其实**——这是本切片唯一"E2 指向的就是真实现"的单元之一。

---

### 3.7 `M2Engine/ObjHero.pas`（14,664 行）→ **C**

**实体**：`THeroObject`(L13, `class(TSmartObject)`)，**134 条类方法 + 19 条裸例程 = 153**。

**托管侧**：E2 只有 2 个文件，**两家都不是载体**：

| 文件 | 性质 | 名面命中 |
|---|---|---|
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.Hero.cs` | **5 个字段**（`m_MyHero`/`m_sHeroName`/`m_sTempHeroName`/`m_sDeputyHeroName`/`m_boWaitHeroDate`），`ms=0` | 0 |
| `EnvirGuardianQuestCore.cs` | 证据文件（207 方法，59 条 `=> true;`） | 0 |

**抽样（11 条 hero 专属例程，全部 0 命中）**：
`AllowHeroMagicRate`、`AllowHeroMagicRate2`、`CheckHeroMagicUseCondition`、`HeroThink`、
`HeroTaosAttackTarget`、`HeroAttackTarget`、`GroupAttackProcess`、`HeroAvoidTarget`、`HeroAvoidTargetNext`、
`ContinueousAttack`、`DoMotaebo100`、`MakeSaveRcd`、`IncBeadExp`、`GetUserItemWeitht`、`EatUseItems`、
`CheckInMasterRange`、`CheckLastHiterRange`、`GetMoveTime`、`GetShowName`、`HeroWizardAttackTarget`。

**注意一处必须排除的假阳性**：全仓搜到同名 `RecalcAbilitys`（`Engine/RecalcAbilitys.cs` 等）与
`RecalcLevelAbilitys`（`Engine/AbilRecalc.cs`），但它们是**给玩家/怪物用的通用重算**，
**不是 `THeroObject` 的实现**——原文里 `THeroObject.RecalcAbilitys`(13950-14016) 与
`THeroObject.RecalcLevelAbilitys`(5723-5973) 是**独立的两段**。**同名 ≠ 同单元，这条实测成立。**

**未移植部分**：**153/153**——整个英雄/副将系统（召唤、英雄 AI 三职业分支、英雄装备/背包/修理、
`m_MyHero` 生命周期、英雄存档 `MakeSaveRcd`）。
`PlayerSurface.Hero.cs` 的注释已自承："托管侧 `THeroObject`（`ObjHero.pas`）与 `TBaseObject` 都未切出，用最薄的 `TCreature` 代表"。

**E2 那处提及**：2 个文件，**均为跨单元佐证**（一个字段容器、一个证据文件）。判 **C** 无疑。

---

### 3.8 `Client-HGE/StateWindows.pas`（14,027 行）→ **C**

**实体**：`TStateWindows`(L48)，**182 条方法**（`TStateWindowsSurface.g.cs` 逐条给出可见性/签名/SHA-256），本轮抽到 **188** 条实现条目。

**托管侧**：`GUI/NewStateWin/` 共 9 个文件，其中 3 个是**生成物**：

| 文件 | 性质 | 名面命中 |
|---|---|---|
| `TStateWindowsSurface.g.cs` | **182 条方法声明清单**（`DeclCount = 182`，`ms=0`，无实现） | 0 |
| `TStateWindowsFieldTable.g.cs` | 字段表（`ms=0`） | 0 |
| `TStateWindowsControlNameTable.g.cs` | 控件名表（`ms=0`） | 0 |
| `TStateWindowsText.cs` | 真实现（`GetGodBlessItemCaption` 10776-10804） | 2 |
| `TStateWindowsMeridians.cs` | 真实现（`GetMeridianStateInfo` 4814-4827） | 1 |
| `TStateWindowsMagic.cs` / `TStateWindowsPets.cs` / `TStateWindowsTitle.cs` / `TStateWindowsControlAddressList.cs` | 真实现（各 1-2 条） | 1-2 |

真实现合计 **6 条名面命中（约 5 条非通用名）**。

**抽样核对**

| 项 | 原文 | 托管 | 结论 |
|---|---|---|---|
| `GetGodBlessItemCaption` | `StateWindows.pas:10776-10804` | `TStateWindowsText.cs:6`（注释写行号） | ✅ 真实现 |
| `GetMeridianStateInfo` | `StateWindows.pas:4814-4827`（原文**顶层函数**，非 `TStateWindows` 方法） | `TStateWindowsMeridians.cs:10` | ✅ 真实现（**跨类借用**：顶层函数被落进 `TStateWindows*` 文件） |
| `TStateWindows.Create` / `InitSelf` / `InitHero` / `InitUserState1` | 48-1238 段 | **无** | ❌ |
| `DStateWinDirectPaint` 等 `D*Paint`/`D*Click` 族（约 120 条） | — | **无** | ❌ |

**E2 那处提及**：10 个 src 文件。**核心来源是 3 个生成文件（声明清单，0 实现）+ 6 个"子函数"文件**。

**★ 一处必须记录的"自承未移植"**：`GUI/Mir/ClientGlobals.cs:106-107`：

```csharp
/// <summary>FState.pas TStateWindows（连击/205 版人物状态窗口，未移植）的最小接缝。</summary>
public class TStateWindows
```

——**注释写的源单元是 `FState.pas`**（不是 `StateWindows.pas`），且**自己声明"未移植"**。
`GUI/Mir/MirForms.cs` 的头注释同样自承："`FState.pas TStateWindows`（连击版人物状态窗口，1041KB 单元，**未移植**）"。
**即：托管侧唯一一个叫 `TStateWindows` 的类，是接缝，不是移植，而且它挂名的源单元还是错的。**

---

### 3.9 `Client-HGE/MShare.pas`（13,522 行）→ **C**

**实体**：**22 个类**——`THttpThread`(761)、`TImageList`(779) 及其 12 个子类、
`TStateItemImages`(938)、`TDnItemImages`(960)、`TBagItemImages`(986)、`TNpcImageList`(1008)、
`TImageEvent`(1029)、`TMapDesc`(1057)、`TWarrContinueHitManager`(1067)、`THttpClient`(1081)、`TUserCenterManager`(1125) + 158 条裸例程。

**托管侧**：12 个 E2 文件，名面命中 **15 条**（`Add`/`Clear`/`IndexOf` 3 条通用名 → 真约 10-12 条）：

| 例程 | 托管位置 | 结论 |
|---|---|---|
| `GetHintFontSize` / `GetHintFontStyle` / `GetHintFontStroke` | `GUI/Share/FStateSeams.cs`（注释写 `MShare.pas:2897` 附近） | ✅ 真实现（**但注意：这是 `FState.pas` 车道为了用而落的接缝，源在 MShare**） |
| `DrawItemHintOldStyle` | `GUI/Share/FStateSeams.cs` | ✅ |
| `GetItemDesc` | `GUI/Share/FStateSeams.cs` | ✅ |
| `ProcessFileNameSpecialChar` | `Scenes/PlaySceneCore.cs` | ✅ |
| `GetRGB` | `DxComponent/DxImageButtonEx.cs` | ⚠️ **仅接缝**（注释："默认恒等"） |
| `UseMagic` | `GUI/Share/FStateSeams.cs` | ⚠️ 接缝（注释写源是 `ClMain.pas frmMain.UseMagic` → **不是本单元**） |
| `Initialize`/`Finalize`/`AddFreeActorList` | `Scenes/PlaySceneCore.cs` | ⚠️ 通用名/属于 `TPlayScene` |
| `TImageList` 族 12 个类、`TMapDesc`、`THttpClient`、`TUserCenterManager`、`TWarrContinueHitManager`（客户端那份） | **全无声明** | ❌ |

**抽样反例（4）**：`TImageList`/`TTilesList`/`TSmTilesList`/`TMonImageList`/`THumImageList`/`TWeaponImageList`
（整个图库族 `/GetImageIndexForUseMagic` 面）、`TMapDesc`、`THttpClient`、`TUserCenterManager`。

**未移植部分**：**约 232/244**——整个客户端图库/资源索引族、地图描述表、HTTP 线程与 HTTP 客户端、用户中心管理器。

**E2 那处提及**：13 个文件。**核心是 `FStateSeams.cs` 的 7 条与 `ClientGlobals.cs` 的 3 条**——
即"`FState.pas`/`ClMain.pas` 车道为了能编译而落的 MShare 接缝"。
`ClientGlobals.cs`/`MirForms.cs` 的注释明确写着"未移植的依赖一律只留最小接缝，**绝不顺手移植**"。

---

### 3.10 `M2Engine/ObjNpc.pas`（10,546 行）→ **B（67 / 112 顶层例程）**

**实体**：**7 个类**：`TConditionList`(219, `class(TList)`)、`TNormNpc`(262, `class(TAnimalObject)`)、
`TMerchant`(334)、`TGuildOfficial`(432)、`TTrainer`(449)、`TBoxMonster`(461)、`TCastleOfficial`(471)。
本轮抽到 **104 个去重方法名（139 条实现）+ 35 条裸例程**。

**★ 本单元有全工程最可靠的一份覆盖率台账**：`Npc/ObjNpcRoutineRegistry.cs`（车道 p4-m2-objnpc 自建），
逐条登记 **112 条顶层例程**（原文 `implementation` 段 496-10544），状态三档：

| 状态 | 条数 | 含义（文件头自述） |
|---|---|---|
| **Covered** | **67** | 已在本车道 `Npc/` 内 1:1 实现 |
| **Seam** | **4** | 已声明最小接缝/虚外壳，未逐行移植；或**部分**落地 |
| **Missing** | **41** | 未覆盖 |

**这与我的名字匹配结果 72/105 不矛盾**（72 是**名面命中**，含 `Add`/`Clear`/`DoSort`/`Initialize` 等通用名；
67 是车道的**逐条归属**）。**本单元取 67/112。**

**承载文件**（按名面命中排序，全部为真实方法声明）：

| 文件 | 命中 | 主要内容 |
|---|---|---|
| `Npc/ObjNpcMerchant.cs` | **21** | `AddItemPrice`(1446-1455)、`GetSellItemPrice`(3793-3796)、`GetItemPrice`/`GetUserPrice`/`GetUserItemPrice`、`GetBack`/`BigGetBack`/`BigStorage`、`DealGold`、`MakeDurg`、`RepairItem`、`RefreshGoods`… + **嵌套过程 `sub_4A0218`(1686-1828)** |
| `Npc/ObjNpcUserSelect.cs` | **21** | `SuperRepairItem`(2089-2092)、`ArmRemoveStoneItem`、`AutoGetExp`、`MakeHeroName`/`MakeDeputyHeroName`、`GetNextPage`/`GetPreviousPage`、`GetRefillList`、`SendCustemMsg`… |
| `Npc/ObjNpcVars.cs` | 9 | `GetVariableText`/`SetVarValue`/`GetVarValue`/`GetDynamicValue`/`SetDynamicValue`/`CheckStrIsVar`… |
| `Npc/ObjNpcLabels.cs` | 9 | `GetLineVariableText`、`ScriptActionError`/`ScriptConditionError`、`AddSelectLable`/`DeleteSelectLable`/`AllowSelect` |
| `Npc/ObjNpcUnitFuncs.cs` | 5 | `LoadLevelScriptAction`(509-595)、`LoadLevelScriptCondition`(596-681)、`GetLevelBaseObjectCondition`(682-856)、`GetLevelBaseObjectAction`(857-1106) |
| `Npc/ObjNpcTypes.cs` | 1 | `TConditionList` 整类（`ObjNpc.pas:219-227` 逐字段注释） |
| `Npc/ObjNpcConversation.cs`/`GuildCastle.cs`/`Persistence.cs`/`BoxMonster.cs`/`MerchantBuy.cs`/`MerchantUpgrade.cs` | 各 1-4 | `Click`、`LoadNPCData`/`SaveNPCData`、`ClientBuyItem`(3367-3688，323 行 1:1)、`UpgradeWapon` |

**抽样逐行核对**

| 例程 | 原文 | 托管 | 结论 |
|---|---|---|---|
| `TConditionList`（整个类） | 219-227 + 504-507 | `ObjNpcTypes.cs:405-434`（逐字段注释原文行号） | ✅ 真实现 |
| `AddItemPrice` | 1446-1455 | `ObjNpcMerchant.cs`（登记表 `StartLine=1446`） | ✅ |
| `ClientBuyItem` | 3367-3688（323 行） | `ObjNpcMerchantBuy.cs:3`（注释"1:1"） | ✅ |
| `sub_4A0218`（嵌套） | 1686-1828 | `ObjNpcMerchant.cs:306` | ✅（嵌套过程被提升为方法） |
| `TNormNpc.GotoLable` | 原文 312 行 | **无**（`ObjNpcSeams.cs:266` 只讨论"原文里它就是坏的"） | ❌ |
| `TMerchant.UserSelect` | 原文 814 行 | **无**（`ObjNpcRoutineRegistry.cs` 标 `Missing`） | ❌ |

**未移植部分（41 条，逐条列在 `ObjNpcRoutineRegistry.cs`）**，按族归并：
- **`TNormNpc`**：`Create`/`Destroy`/`Operate`/`Run`/`Initialize`/`LoadAddData`/`SaveAddData`/`GotoLable`/`GetShowName`（9 条）
- **`TMerchant`**：`Create`/`Destroy`/`Operate`/`Run`/`RefillGoods`/`GetBackupgWeapon`/`UserSelect`/`ClientGetDetailGoodsList`/`ClientQuerySellPrice`/`ClientMakeDrugItem`/`ClientQueryRepairCost`/`ClientRepairItem`/`ChangeUseItemName`（13 条）
- **`TGuildOfficial`**：`Run`/`UserSelect`/`ReQuestBuildGuild`/`ReQuestGuildWar`/`DoNate`/`ReQuestCastleWar`/`Destroy`（7 条）
- **`TCastleOfficial`**：`GetVariableText`/`UserSelect`/`HireGuard`/`HireArcher`/`RepairDoor`/`RepairWallNow`/`Destroy`（7 条）
- **`TTrainer`**：`Create`/`Destroy`/`Operate`/`Run`（4 条）
- **`TBoxMonster`**：`Destroy`（1 条）

**E2 那处提及**：39 个文件（src 内 18 个）。载体是 `Npc/**` 那 13 个文件 + `Engine/PlayerSurface/NpcSession.cs`。
**E2 成立且名副其实**（这是本切片第二个"E2 指向真实现"的单元）。

---

### 3.11 `M2Engine/NpcConditionCmd.pas`（10,489 行）→ **C**

**实体**：**0 个类**，`implementation` 段 **283 个单元级函数**（首条 `CheckOrCreateTxtFile`，
其余为 `Check*`/`Is*`/`Have*` 条件族）。

**托管侧**：E2 只有 2 个文件——`Engine/NpcScriptCommands.cs`（0 命中）、`FbInstancePoolCore.cs`（0 命中）。
**283 条全无声明。** 名面命中 **0（0.0%）**。

**抽样（6 条，全部 0 命中）**：`CheckOrCreateTxtFile`、`nNC_DAYTIME` 对应的条件函数、
`CheckLevelEx`、`CheckPKPoint`、`CheckGuildList`、`CheckContainsText`。

**与 `NpcScriptCommands.cs` 的关系**：该文件 **用命令码重写**了这些判定，例如

```csharp
CReg(NpcCmdCodes.nNC_DAYTIME, (n, u, c) => {
    int hour = DateTime.Now.Hour;
    return hour switch { >= 5 and < 7 => c.GetInt(0) == 0, ... }; });
```

—— **语义等价、代码非翻译**。按 §39.5/§44.5 口径（"被共享框架/运行时等价取代"归 **not-ported**），
**不得留在 MAPPED**。

---

### 3.12 `M2Engine/ObjMon.pas`（9,502 行）→ **C**

**实体**：**55 个类**（`docs/ObjMon-manifest.tsv` 全表；`TMonster`(9) → `TWealthAnimalMon`(531) 共 56 条声明，扣 `(* *)` 里 1 条）。
**204 条类方法 + 16 条裸例程 = 220 条实现**（与 `docs/ObjMon-对账.md` 的"去重后 206"一致量级）。

**托管侧**：**46 个 `ObjMon*Core.cs`**（src 内）+ 44 个测试文件。

| 指标 | 实测 |
|---|---|
| 46 个 `ObjMon*Core.cs` 的方法声明总数 | 约 **6,300** |
| 其中 `=> true;` / `=> false;` | 约 **1,900**（`ObjMonSpitSpiderCore.cs` 单文件 **70**，`ObjMonFireCrossCore.cs` 57，`ObjMonTruckCore.cs` 56） |
| 与 `ObjMon.pas` **代码例程**同名的 | **0** |
| 类声明匹配到 `ObjMon.pas` 类的 | 仅 `Engine/ObjBase.cs:216` 的 `TMonster` |

**`ObjMonSpitSpiderCore.cs` 的实测形态**（本单元的代表）：

```csharp
public static class ObjMonSpitSpiderCore {
    public static bool SlowCreateIsShell() => true;
    public static bool SlowDestroyIsShell() => true;
    public static bool BothEquivalentToNoOverride() => true;
    public static bool ScorpionSetsAnimalTrue() => true;
    ...
}
```

—— 这是**"把原文事实写成可断言的恒真谓词"**，用途是当**证据/回归哨兵**，
**不是 `TSpitSpider.SpitAttack`（1539-1651，113 行）的翻译**。文件名 `*Core.cs` 容易被误读成"核心实现"，**必须按内容判定**。

**★ 关键反例：`Engine/ObjBase.cs:216-245` 的 `TMonster`**

```csharp
public class TMonster : TCreature {
    public uint m_dwWalkTick; public uint m_dwAttackTick;
    public int m_nViewRange = 8; public TCreature? m_Target;
    public override void Run() {
        base.Run();
        if (m_boDeath || m_boGhost) return;
        uint now = DelphiRTL.GetTickCount();
        if (m_Target != null && !m_Target.m_boDeath && m_PEnvir != null) {
            if (now > m_dwWalkTick) { m_dwWalkTick = now + 800;
                int dx = Math.Sign(...); int dy = Math.Sign(...);
                WalkTo(DirFromDelta(dx, dy)); } } }
```

原文 `TMonster.Run`（`ObjMon.pas:1121-1391`，**270+ 行**）含**四条**该近似物完全没有的分支：
`m_Master` 天关宝宝 `MakeGhost` / 镜像地图 `SpaceMove` / `m_boWalkWaitLocked` 走步等待锁 /
`m_boGamePet && g_Config.boPetQuickPickup` 宠物拴物。→ **近似物，非翻译。判 C 成立。**

**抽样（6 条，全部 0 命中）**：`TMonster.Run`(1121)、`TMonster.Think`(845)、`TMonster.Operate`(840)、
`TMonster.AttackTarget`(888)、`TMonster.MakeClone`(806)、`TChickenDeer.Run`(1393)。

**与既有文档的关系（已复核）**：
- `docs/ObjMon-覆盖率表.md` 自报"37/55 已移植"，但其 §1 的判据是"取每个 C# 文件**头 4 行 `///`** 注释里出现的 Delphi 类名"，
  该文件 §6 与 §8 自承**三种自动判据全部失败**、`TScultureMonster`/`TScultureKingMonster` 等**注释命中即假阳性**；
- `docs/ObjMon-对账.md` §3 判"未移植 14 个类 / 62 条方法"，但同样声明"是**上界估计**"。
- **本轮独立复核结论：55 个类全部无托管声明（唯一同名 `TMonster` 是简化近似物），204 条实现 0 条移植。**

**E2 那处提及**：92 个文件（src 内 46 个）——**全是 `ObjMon*Core.cs` 证据文件**。
这是"E2 最响、实现最少"的极端例：**92 个文件头写 `ObjMon.pas`，204 条例程 0 条落地**。

---

### 3.13 `M2Engine/HandleCommands.pas`（9,151 行）→ **C**

**实体**：**0 个类**，`implementation` 段 **168 个单元级过程**（`CmdChangeAdminMode`/`CmdChangeAttackMode`/
`CmdChangeSuperManMode`/`CmdChangeObMode`/`CmdMakeItem`/`CmdDeleteItem`/`CmdTrainingMagic`(37)/
`ProcessUserLineMsg`(16)… 全是 `@` 开头的 GM 命令处理器）。

**托管侧**：E2 只有 2 个文件——`EnvirRangeQueryCore.cs`（111 方法，**45 条 `=> true;`**）、
`MapQueryCore.cs`（102 方法，**32 条 `=> true;`**）。**两家都是证据文件，0 命中。**

**抽样（5 条，全部 0 命中）**：`ProcessUserLineMsg`、`CmdChangeAdminMode`、`CmdChangeAttackMode`、
`CmdMakeItem`、`CmdDeleteItem`、`CmdTrainingMagic`。

**未移植部分**：**168/168**。GM 命令的"功能"由 `NpcScriptCommands.cs` / 其它命令注册表部分覆盖，
但**本单元没有一行代码被翻译**。

**E2 那处提及**：2 个文件，**均为跨单元证据文件**（它们的主源是别的单元）。
**这两个文件头写 `HandleCommands.pas` 是"顺带引用"，不构成本单元的任何移植证据。**

---

## 4. 汇总与 `mapped` 修正建议

### 4.1 计数

| 裁决 | 个数 | 单元 | 行数合计 |
|---|---|---|---|
| **A（真移植）** | **0** | — | **0** |
| **B（部分移植）** | **2** | `FunctionConfig`(15,801，142/926)、`ObjNpc`(10,546，67/112) | **26,347** |
| **C（未移植 / 仅借名）** | **11** | `ObjPlayer`(49,232)、`NpcActionCmd`(47,018)、`M2Share`(33,605)、`FState`(25,165)、`Actor`(18,009)、`ObjHero`(14,664)、`StateWindows`(14,027)、`MShare`(13,522)、`NpcConditionCmd`(10,489)、`ObjMon`(9,502)、`HandleCommands`(9,151) | **244,384** |
| **合计** | **13** | | **270,731** |

> 行数与任务表**逐一相符**（已复核）；实例化的"类声明数 / 例程总数"按 §1.1 口径重算，
> 与任务表的提示数可能在"例程"上有差异（如 `ObjMon` 提示 204、我按实现条数得 220；`Actor` 提示 156、我按实现条数得 170）——
> 差异来自**是否把 `implementation` 段裸例程与同名去重方法计入**，本报告已写明口径。

### 4.2 对 `mapped` 数字的修正建议（本切片 13 条）

| 单元 | 现状 | **建议去处** | 行数 | 理由（口径） |
|---|---|---|---|---|
| `M2Engine/ObjPlayer.pas` | MAPPED(E2) | **not-ported 注册表** | 49,232 | `TPlayObject` 807 条实现 0 条翻译；仅成员容器 + 19 条孤立存取器。**这是全工程最大单条缺口** |
| `M2Engine/NpcActionCmd.pas` | MAPPED(E2) | **not-ported 注册表** | 47,018 | 737/737 无翻译；功能被 `NpcScriptCommands.cs` **等价取代**（§39.5 口径即归 not-ported） |
| `M2Engine/M2Share.pas` | MAPPED(E2) | **开实现车道**（若不允许新车道：not-ported） | 33,605 | 约 22/314 真移植，**约 7%**——已过 not-ported 的"按设计不移植"门槛吗？否，它有明确的待移植面；但**不得留在 MAPPED** |
| `Client-HGE/FState.pas` | MAPPED(E2) | **开实现车道**（`FState` 余部 300+ 例程早已在台账 §12.7 登记为待办） | 25,165 | 约 31/367 真移植；515 条 `throw` 桩被误当命中 |
| `Client-HGE/Actor.pas` | MAPPED(E2) | **开实现车道** | 18,009 | 约 43/136 真移植；`THumActor`/`THeroActor`/`TNpcActor` 三族是空壳 |
| `M2Engine/FunctionConfig.pas` | MAPPED(E2) | **保留 MAPPED，但必须标注"部分"并开续片车道** | 15,801 | 142/926 是**真 1:1**，属"在做的移植"，不该移出；但报表的 MAPPED 二值语义掩盖了 784 条缺口 |
| `M2Engine/ObjHero.pas` | MAPPED(E2) | **not-ported 注册表 → 或开实现车道** | 14,664 | 153/153 无翻译、`THeroObject` 无声明。**但英雄是产品必需功能**，建议**开实现车道**而非 not-ported |
| `Client-HGE/StateWindows.pas` | MAPPED(E2) | **开实现车道** | 14,027 | 182 条方法只有声明清单；真实现约 5 条 |
| `Client-HGE/MShare.pas` | MAPPED(E2) | **开实现车道** | 13,522 | 约 10/244；22 个类 0 声明 |
| `M2Engine/ObjNpc.pas` | MAPPED(E2) | **保留 MAPPED，标注"67/112 + 41 Missing"并续片** | 10,546 | 67 条是逐行 1:1 的真移植，且有**登记表**可查 |
| `M2Engine/NpcConditionCmd.pas` | MAPPED(E2) | **not-ported 注册表** | 10,489 | 283/283 无翻译；功能被命令码派发器等价取代 |
| `M2Engine/ObjMon.pas` | MAPPED(E2) | **开实现车道**（`docs/ObjMon-*` 已把它当"已完成"，是**最需更正的既有结论**） | 9,502 | 204/204 无翻译；46 个 `*Core.cs` 是恒真断言证据文件 |
| `M2Engine/HandleCommands.pas` | MAPPED(E2) | **not-ported 注册表** | 9,151 | 168/168 无翻译；GM 命令由别的注册表覆盖 |

**建议的操作（给集成方，本车道无 `tools/` 写权）**：
1. **第一步（零风险，只改口径）**：把上表 5 个纯 not-ported 单元
   （`ObjPlayer`/`NpcActionCmd`/`NpcConditionCmd`/`HandleCommands`，以及 `ObjMon` 若要按"等价取代"口径）
   加进 `tools/audit-coverage.ps1` 的 `$VENDOR_UNITS`。**预计 `mapped` 由 349 → 344（或 343），
   `not-ported` 由 196 → 200（或 202）**——**`mapped` 只降不虚增**，符合 §39.5 的"只改口径、不虚增完成度"。
   > ⚠️ **但 `ObjMon`/`ObjHero` 是否归 not-ported 需先裁定**：它们是**产品必需**且**已有大量证据文件**，
   > 按"被共享框架等价取代"归 not-ported 会**把真缺口藏进 not-ported 桶**（正是 §39.5 警告的反向风险）。
   > 我的建议：**这 4 个纯声明缺失 + 无替代框架的（`ObjPlayer`/`ObjHero`/`StateWindows`/`MShare`）
   > 应开实现车道；只有"确有替代框架"的（`NpcActionCmd`/`NpcConditionCmd`/`HandleCommands`）才进 not-ported。**
2. **第二步**：给 `FunctionConfig`/`ObjNpc` 两个 B 单元在报表里加"部分"标记（需要 `audit-coverage.ps1` 支持三值），
   或在台账里引用 `FunctionConfigForm.cs` / `ObjNpcRoutineRegistry.cs` 的计数。
3. **第三步**：**不要把本切片任何单元加进"补 E2 头注释"清单**——它们的 E2 已经成立，
   问题是**实现不存在**（详见 §5.1）。

### 4.3 数字影响的一句话

**本切片 11 个 C 单元 = 244,384 行应从 `MAPPED` 移出。** 若只做最保守的处置
（4 个"有替代框架"的单元 → not-ported，7 个"无替代框架"的 → 移出并开车道），
`mapped` 至少应 **349 − 11 = 338**，`not-ported` **196 + 4 = 200**，
另外 7 个单元（**189,308 行**）需要一个**新的"真缺口"类别或 ASSIGNED 车道**去承载——
因为当前的 MAPPED/WEAK/ASSIGNED/UNMAPPED/not-ported 五桶里，
"**E2 成立但实现不存在**"**没有家**。

---

## 5. 附带发现（对审计工具的直接含义）

### 5.1 ★★ `E2` 是"某文件的自我申报"，与"实现是否存在"无关——本切片给了 4 种反例

| 反例类型 | 实例 | 后果 |
|---|---|---|
| **生成壳** | `TFrmDlg.Decl.g.cs`（515 条全 `throw`，头注释写 `FState.pas`） | `FState.pas` 的名面命中 **330** 中 **291 条是假阳性（79%）** |
| **恒真断言证据文件** | `ObjMon*Core.cs`（46 个，约 1,900 条 `=> true;`） | `ObjMon.pas` 有 **92 个文件**满足 E2，**204 条例程 0 条移植** |
| **成员容器片** | `PlayerSurface/TPlayObject.*.cs` | `ObjPlayer.pas` 满足 E2，但 `TPlayObject` 807 条实现 0 条 |
| **替代框架** | `Engine/NpcScriptCommands.cs`（头注释写 `NpcActionCmd.pas / NpcConditionCmd.pas`） | 两单元 1,020 条例程 0 条翻译；脚本功能却"能用" |

> **工具建议**：`audit-coverage.ps1` 若要把 E2 保留为 MAPPED 判据，**必须加一道过滤**：
> 排除 `// <auto-generated>` 文件、排除"命中仅出现在 `throw`/`=> true` 桩里"的文件。
> 否则 **E2 会把"证据"读成"实现"**——这正是 §38.6 那句"名字匹配≠移植"的第三次复发，
> 只是这次伪装成了"头 40 行提到源单元"。

### 5.2 ★ "同名类声明"也会骗人：`PlaySceneNewActor.cs` 的 50+ 空壳

`Scenes/PlaySceneNewActor.cs:488-571` 有 **50+ 个**与 `Actor.pas` / `ObjMon.pas` **同名的类声明**，
每个只有一行 `public override string ActorClass => "Xxx";`：

```csharp
public class THumActor : TActor { public override string ActorClass => "THumActor"; }
public class TLionMonster : TActor { public override string ActorClass => "TLionMonster"; }
public class TWhiteSkeleton : TActor { public override string ActorClass => "TWhiteSkeleton"; }
public class TElectronicScolpionMon : TActor { public override string ActorClass => "TElectronicScolpionMon"; }
```

而**该文件的头注释写的是 `PlayScn.pas 6969-7486 NewActor 1:1`**——它们是从 `PlayScn.pas` 的
`wRaceImg` 分派表里**照抄类名**得到的壳。**用"类声明匹配"判定会全部判为"已移植"。**
（注：`TElectronicScolpionMon` 同时是 `ObjMon.pas` 里"57 个未移植类"之一——
**客户端一个空壳名字，会让服务端那条真缺口在类声明判据下也变绿。**）

### 5.3 E2 是**按 basename** 匹配的：`Actor.pas` 的反例证明它两个方向都会错

- `Scenes/Actor*.cs`（真实现）**写对了** `Actor.pas` → 进 E2 ✅
- `Scenes/PlaySceneNewActor.cs`（50+ 个 `Actor.pas` 空壳类）**写的是 `PlayScn.pas`** → **没进 E2**
- 而 `M2Server/Client*Core.cs`（9 个，约 1,000 个恒真断言，与 `Actor.pas` 无关）**头注释里写了 `Actor.pas`** → **进了 E2** ❌

**净效果**：`Actor.pas` 的 E2 文件数（56）里，**约 10 个是无关的恒真断言文件**，
而**真正装着 50+ 个同名类的文件反而不在列**。`E2` 的"文件数"完全不能当作覆盖度代理。

### 5.4 E1 在本切片 13 个单元上**全部不成立**（已逐个确认）

```powershell
# 13 个 basename 在 src/tests/ 下都没有同名 .cs
foreach ($n in 'ObjPlayer','NpcActionCmd','M2Share','FState','Actor','FunctionConfig',
               'ObjHero','StateWindows','MShare','ObjNpc','NpcConditionCmd','ObjMon','HandleCommands') {
  Get-ChildItem "$W\src","$W\tests" -Recurse -Filter "$n.cs" -ErrorAction SilentlyContinue
}   # => 0 条
```

⇒ 这 13 条**全部符合任务描述的"E2-only"**：只有 E2、没有 E1。

### 5.5 虚分派与"接缝自承"是好信号，建议纳入审计

本切片里凡**判 C 但托管侧确有同名类**的地方，代码注释**都自己写明了"未移植"**：

| 位置 | 注释 |
|---|---|
| `GUI/Mir/ClientGlobals.cs:106` | `/// <summary>FState.pas TStateWindows（连击/205 版人物状态窗口，未移植）的最小接缝。</summary>` |
| `GUI/Mir/MirForms.cs:14` | `//   FState.pas  TStateWindows（… 1041KB 单元，未移植）` |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.Hero.cs:26` | `托管侧 THeroObject（ObjHero.pas）与 TBaseObject 都未切出，用最薄的 TCreature 代表` |
| `Forms/DummySetting/DummySettingGlobals.cs:11-13` | `LoadDummyDisableMoveMap（**未移植**）` / `LoadDummyNoActiveAttackMonList（**未移植**）` / `LoadDummyNameList（**未移植**）` |
| `Npc/ObjNpcRoutineRegistry.cs` | 112 条逐条 `Covered/Seam/Missing` |

⇒ **建议**：`audit-coverage.ps1` 增加一条**负向判据**——若 `.cs` 头 40 行内同时出现
`<unit>.pas` 与 `未移植`／`Missing`／`throw new NotSupportedException`，
则**不得计为 MAPPED**。本切片有 **5 处**可直接被这条规则救回。

---

## 6. 注册表提案（本车道无权改 `tools/`，仅提案）

### 6.1 basename 唯一性核查（**裸键 vs 逐副本键**的前提）

```powershell
$hits = Get-ChildItem 'D:\chuanqi\daima\GXX原版_Delphi7\Source' -Recurse -Filter '<name>.pas'
```

| 单元 basename | `Source/` 下副本数 | 路径 | 是否可用**裸键** |
|---|---|---|---|
| `ObjPlayer` | **1** | `Source\M2Engine\ObjPlayer.pas` | ✅ 可 |
| `NpcActionCmd` | **1** | `Source\M2Engine\NpcActionCmd.pas` | ✅ 可 |
| `M2Share` | **1** | `Source\M2Engine\M2Share.pas` | ✅ 可 |
| `FState` | **1** | `Source\Client-HGE\GUI\Share\FState.pas` | ✅ 可 |
| `Actor` | **1** | `Source\Client-HGE\Actor.pas` | ✅ 可 |
| `FunctionConfig` | **1** | `Source\M2Engine\Forms\FunctionConfig.pas` | ✅ 可（但它是 B，**不进** not-ported） |
| `ObjHero` | **1** | `Source\M2Engine\ObjHero.pas` | ✅ 可 |
| `StateWindows` | **1** | `Source\Client-HGE\GUI\NewStateWin\StateWindows.pas` | ✅ 可 |
| `MShare` | **1** | `Source\Client-HGE\MShare.pas` | ✅ 可 |
| `ObjNpc` | **1** | `Source\M2Engine\ObjNpc.pas` | ✅ 可（但它是 B，**不进** not-ported） |
| `NpcConditionCmd` | **1** | `Source\M2Engine\NpcConditionCmd.pas` | ✅ 可 |
| `ObjMon` | **1** | `Source\M2Engine\ObjMon.pas` | ✅ 可 |
| `HandleCommands` | **1** | `Source\M2Engine\HandleCommands.pas` | ✅ 可 |

**结论：本切片 13 个 basename 在 `Source/` 树里都是唯一副本**（已在镜像 `_analysis/utf8_mirror` 与真实 `Source/` 两处各验一次），
因此 **§39.3 的"重复 basename 禁止写裸键"约束在本切片不触发**，`$VENDOR_UNITS` 里**可以安全使用裸键**。

**⚠️ 但有一个必须写进提案的注意点**：这些 basename 唯一，**不代表它们与其它单元的"实体名"不冲突**——
`FState`/`StateWindows`/`MShare` 三个单元**互相共享实体**（`TFrmDlg` 在 FState，`TStateWindows` 被
`FState` 的注释挂名，`GetHintFontSize`/`GetRGB` 在 `MShare` 却被 `FState` 车道落进 `FStateSeams.cs`）。
**且 `TWarrContinueHitManager` 这个类名同时出现在 `M2Engine/ObjPlayer.pas:1398` 与
`Client-HGE/MShare.pas:1067`（两份不同的实现）** —— 若将来要用**实体名**做注册键，这两个必须逐副本。

### 6.2 具体提案

| 单元 | 是否进 not-ported 注册表 | 键 | 理由 |
|---|---|---|---|
| `NpcActionCmd` | **建议进** | 裸键 `NpcActionCmd` | 737 例程 0 翻译；命令派发器是等价取代；无同名副本 |
| `NpcConditionCmd` | **建议进** | 裸键 `NpcConditionCmd` | 283 例程 0 翻译；同上 |
| `HandleCommands` | **建议进** | 裸键 `HandleCommands` | 168 例程 0 翻译；GM 命令由注册表取代 |
| `ObjPlayer` | **建议不进** | — | **不是"按设计不移植"，而是巨型真缺口**。进 not-ported 会把 49,232 行藏起来。建议开实现车道（台账 §18.7 已把它列为"最主要系统性瓶颈"） |
| `ObjHero` | **建议不进** | — | 同上（14,664 行，产品必需） |
| `ObjMon` | **建议不进** | — | 204 例程 0 翻译，但 `docs/ObjMon-*` 已把它当"37/55 已完成"——**它是真缺口，进 not-ported 会把结论错误地固化**。建议改为**更正既有文档 + 开实现车道** |
| `StateWindows` | **建议不进** | — | 同上（182 条方法） |
| `MShare` | **建议不进** | — | 同上（22 个类） |
| `Actor` | **建议不进** | — | 已移植约 1/3，属在做的移植 |
| `FState` | **建议不进** | — | 已移植约 31 例程，台账 §12.7 早列为"FState 余部 300 例程"待办 |
| `FunctionConfig` | **不建议进**，建议**新增"部分"标记** | — | 142 条 1:1 真移植，进 not-ported 是错的 |
| `ObjNpc` | **不建议进**，建议**新增"部分"标记** | — | 67/112，有自建登记表 |

**并附一条流程建议**：`$VENDOR_UNITS` 目前是**二值**的（在内 = 不移植，不在 = 其余四桶）。
本切片证明**需要第三态**："**E2 成立、部分是真移植、缺口已量化的实现中单元**"。
在没有第三态之前，我建议**宁可让这 7 个单元继续显示为"缺口"**（报表不准但能推进），
也**不要把它们的 basename 加进 not-ported**（那会让 189,308 行的真缺口从报表上消失）。

---

## 7. 无法判定的部分（如实登记，**未猜**）

1. **`mapped` 的其它 153 个 E2-only 单元未复核**。本轮只裁决了指定的 13 个巨型单元；
   剩余 **153 个单元 / 213,804 行**（484,535 − 270,731 = 213,804）的 `MAPPED(E2)` 状态**本轮未验**。
   本报告 §5.1 的四种反例形态**很可能在其中复现**，但我**没有证据**，不作断言。
2. **`M2Share.pas` 的精确例程分母**。原文里同一函数名有**多份实现**
   （实测 `LoadClientModules` 在 3430 与 8665 两处、`CanFilterMsg` 在 3426 与 15517 两处）——
   "314"是按名去重后的数。**要给出精确的"已移植/总"必须先把重复实现逐对分流**
   （可能涉及 `{$IFDEF}` 分支），本轮**未做**，故 3.3 节我只给"约 22/314（约 7%）"的**量级**，不主张精确值。
3. **`FunctionConfig.pas` 的"未移植部分"里哪些新写的 `Ref*` 是重构而非对应例程**。
   `FunctionConfigForm.cs` 里有 `RefFinalPages`/`RefOffLineMyShop`/`RefMutinyMsgLuck`/`RefReNewMonUpgrade`/
   `RefMasterMineLottery` 五个 `Ref*` 方法，**在 926 条原文名字里找不到**（因此没算进 142），
   但它们显然是**若干原文 handler 的重构合并**。**要判"这五个方法覆盖了哪些原文例程"必须先读原文 3692-15801 段**，
   本轮未做，故 3.6 节把"未移植 784 条"作为**上界**（真缺口可能小于 784）。
4. **`Client-HGE/Actor.pas` 的 `THumActor`/`THeroActor` 三族是否有除 `PlaySceneNewActor.cs` 外的实现**。
   我已搜遍 `src` 的 `class THumActor`/`class THeroActor`/`class TNpcActor` 声明（只有 `PlaySceneNewActor.cs:488/494/536`），
   但**没有逐个方法搜**（例如某个 `ThumActor*` 前缀的静态类里可能藏着部分逻辑）。
   **本轮以"类声明即空壳"为据判 C 的"三族未移植"，这个判断的方法级证据不足，故登记为不确定项。**
5. **`MShare.pas` 的 `TWarrContinueHitManager` 与 `ObjPlayer.pas` 的同名类的关系**。
   `Client-HGE/MShare.pas:1067` 与 `M2Engine/ObjPlayer.pas:1398` 各有一个 `TWarrContinueHitManager`，
   两份均无托管声明。**是否两份 SHA256 相同（同一份源码被复制）本轮未算**——
   §1.1 的方法（`Get-FileHash`）本轮只用于确认 basename 唯一性，**未做类级源码比对**。
6. **`ObjMon.pas` 的 55 个类里，`Engine/ObjBase.cs` 的 `TMonster` 是否覆盖了 `TAnimal`/`TAnimalObject` 的行为**。
   `TAnimal`/`TAnimalObject` 在 `ObjMon.pas` 里是**基类**（`TMonster = class(TAnimalObject)`），
   但它们**不在本单元的 55 个类声明里**（来自别的单元）；本轮**未追**它们的归属，
   故 3.12 节的"204 条"只针对 `ObjMon.pas` 自己的实现，**不含继承来的行为**。
7. **未做任何编译/测试验证**（车道明令禁止 `dotnet build`/`dotnet test`）。
   本报告全部"真移植/未移植"结论都是**静态**的；若某处托管代码能编译但对原文语义有偏差，
   本轮**不会**发现。

---

## 8. 交付物

- 本文件：`GXX.CSharp/docs/并行报告-p12-e2only-review.md`（唯一写入）
- 未创建/修改任何其它文件；**未运行** `dotnet build` / `dotnet test`。
- 分析脚本位于 `%TEMP%\p12\`（`tables.ps1` / `e2.ps1` / `revcov3.ps1` / `verify.ps1`），
  **刻意不写进仓库**（车道只允许一个产物），其口径已完整抄进 §1，可原样重建。
