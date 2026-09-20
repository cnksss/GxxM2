# 并行报告：车道 `p6-m2-playersurface`（`TCreature`/`TPlayObject` 面拼接）

> 分支 `par/p6-m2-playersurface` ｜ 工作树 `.worktrees/p6-m2-playersurface`
> 独占区：`GXX.CSharp/src/GXX.M2Server/Engine/PlayerSurface/**`（7 个新 .cs）、
> `GXX.CSharp/tests/GXX.M2Server.Tests/PlayerSurface*.cs`（5 个新 .cs）
> **未改任何既有文件**（`Engine/ObjBase.cs`、`Engine/CombatPower.cs`、`Engine/RecalcChain.cs`、
> `Engine/Castle.cs`、`Engine/Envir.cs` 等全部只读 —— `git show --stat HEAD` 只有 12 个新增文件）
> 门禁：`dotnet build GXX.slnx -c Debug` **0 error**；`GXX.M2Server.Tests` **7,367 passed / 0 failed**
> （基线实跑 **7,214**，本车道 **+153**，无新增失败）

---

## 0. 给调度方的一段话（结论先行）

`TCreature`/`TPlayObject` 的**四片成员面已按原文 1:1 补齐**（95 个成员/方法，见 §2），
**ObjNpc 报告 §8.6 四条优先清单的硬阻塞全部解除**（逐条打勾见 §6）。
全部产出通过 `partial` 落在**本车道自己的新文件**里，**没有碰任何既有 Engine 文件**。

**三件需要调度方决策/执行的事**：

1. ★ **`m_ItemList` 的整合**（§8-A）：托管侧有两个近义容器 —— `Engine/ObjBase.cs:166` 的
   `List<TUserItem> m_ItemList`（`git grep` 证明**当前零调用方**）与本车道的
   `TCreature.BagItems`（`List<TUserItemView>`）。建议把 `ObjBase.cs:166` 的类型改为
   `List<TUserItemView>` 并删除本车道的私有后备字段。**本车道未动它**（`Engine/**` 只读）。
2. ★ **`TCastleManager` 不是 `partial`**（§8-B）：`Engine/Castle.cs:406`
   `public class TCastleManager`（**无 `partial`**），因此车道**无法**按 partial 手法加
   `GetCastleNameList`。需要**您**在 `Castle.cs` 里加（精确签名见 §8-B），或把它改成 `partial`。
3. ⚠ **`RecalcAbilitys` 是方法名碰撞的第二处实例**（§4）：托管侧原先只有
   `Engine/RecalcChain.cs:32` 的**扩展方法** `RecalcAbilitys(this TPlayObject)`；
   本车道补的 `TCreature.RecalcAbilitys()` **虚方法**在 C# 重载决议里**优先于扩展方法**，
   因此**所有 `player.RecalcAbilitys()` 调用点现在走虚方法**（其默认实现转调那个扩展方法，
   行为不变，`RecalcChainTests` 6 例全绿）。但这是一个**静默的调用目标切换**，请知悉。

---

## 1. 全部 commit hash

| # | commit | 内容 |
|---|---|---|
| 1 | `61d3ab20` | 切片 1-4：7 个源文件 + 5 个测试文件（4,157 行新增）；变量容器 / 金额物品 / NPC 会话与脚本标签 / 英雄副将 / `TCreature` 基类成员 + 153 用例 |

> 基线对照：本车道开工时工作树 = `170cd9cb`；交付时 main 已推进到 `7824ede2`（顺序会话持续提交）。
> 本车道**未 rebase**；已用 `git grep ... main` 对**当前** main 复查全部新成员，结果见 §3。

---

## 2. 成员添加表

「虚」列：★ = 原文 `virtual`/`override`，托管侧已保留虚分派（台账 §18.8 硬要求）。

### 2.1 片 1 —— 变量容器（`TPlayObject.PlayerSurface.Vars.cs` / `.VarDefaults.cs`）

| 成员 | 原文 `文件:行` | 托管落点 | 虚 | 备注 |
|---|---|---|---|---|
| `m_nVal` | `ObjPlayer.pas:119` | Vars.cs | — | `int[1000]`，P 变量 |
| `m_TVal` | `ObjPlayer.pas:123` | Vars.cs | — | `string[500]`；`SetTVal/GetTVal` 带 `string[100]` 截断 |
| `m_sString` | `ObjPlayer.pas:127` | Vars.cs | — | `string[1000]`；**无长度上限**（与 `m_TVal` 不同） |
| `m_ArrayList` | `ObjPlayer.pas:260` | Vars.cs | — | **复用** `CombatPower.cs:567` 的 `TValueListStub` |
| `ClearNValues` | `ObjNpc.pas:9394/9417` | Vars.cs | — | `FillChar(m_nVal[0], SizeOf(m_nVal), 0)` 等价 |
| `ClearTValues` | `ObjPlayer.pas:1833` | Vars.cs | — | |
| `ClearZValues` | `ObjPlayer.pas:1835/3849` | Vars.cs | — | 两处调用点 |
| `ClearSStrings` | `ObjPlayer.pas:1627` | Vars.cs | — | |
| `TruncShortString100` / `SetTVal` / `SetZVal` / `GetTVal` / `GetZVal` | `ObjPlayer.pas:123/125` + 写入点 `ObjBase.pas:26290`、`ObjNpc.pas:5132/5166` | Vars.cs | — | `string[100]` 截断语义 |
| `HasStringVar` / `HasIntegerVar` | `CombatPower.cs` 既有 `m_StringList`/`m_IntegerList` 的 `GetIndex >= 0` | Vars.cs | — | 便利包装，非原文成员名 |
| `PlayerVarSurfaceConst` | `ObjPlayer.pas:119-127` | Vars.cs | — | 数组长度常量 |
| `PlayerSurfaceVarDefaults`（`MigrateStringVarDefaults` / `ReadVar`） | `ObjPlayer.pas:119-127` | VarDefaults.cs | — | 见 §7-A（`null` vs `''` 缺陷的**接缝式**修正） |

**未重复声明**（既有）：`m_nMval` / `m_DyVal` / `m_nInteger` / `m_UVal` / `m_JVal` / `m_ZVal` /
`m_StringList` / `m_IntegerList` → `Engine/CombatPower.cs:541-563`。

### 2.2 片 2 —— 金额 / 物品容器

**`TPlayObject.PlayerSurface.Gold.cs`**

| 成员 | 原文 `文件:行` | 虚 | 备注 |
|---|---|---|---|
| `m_nGameGoldEx` | `ObjPlayer.pas:201` | — | |
| `m_nDealGoldPose` | `ObjPlayer.pas:250` | — | |
| `m_nBigStoragePage` | `ObjPlayer.pas:48` | — | |
| `m_nGoldMax` | `ObjBase.pas:106`（初始化 `:11311`） | — | 取 `M2Config.nHumanMaxGold` |
| `m_dwRecordBeadExp` | `ObjPlayer.pas:3388/3391` | — | 聚灵珠 |
| `m_nCurrentItemMakeIndex` / `m_sCurrentItemName` | `ObjPlayer.pas:3376-3380` | — | `@AddBag` 期间生效 |
| `m_nGateIdx` | `ObjPlayer.pas:246` | — | |
| `IncGold(uint)` | `ObjPlayer.pas:1181/3231-3249` | — | ★ **Cardinal 回绕**（`unchecked`）已逐字复刻 |
| `DecGold(uint)` | `ObjPlayer.pas:1212/3284-3293` | — | 不够时**原地不动** |
| `IncGameGold(uint)` | `ObjPlayer.pas:3251-3260` | — | 只夹 `High(LongWord)`，**不夹 `m_nGoldMax`** |
| `DecGameGold(uint)` | `ObjPlayer.pas:3295-3303` | — | 不够时**夹到 0**（与 `DecGold` 不同） |
| `GoldChanged()` | `ObjPlayer.pas:1168/2529-2532` | 非虚（原文无） | |
| `GameGoldChanged()` | `ObjPlayer.pas:1169/2534-2537` | 非虚（原文无） | |
| `NewGamePointChanged()` | `ObjPlayer.pas:2539-2542` | — | |
| `GameGloryChanged()` | `ObjPlayer.pas:2544-2547` | — | |
| `PlayerSurfaceMsgSeams` | — | — | `SendUpdateMsg`/`SendDefMessage`/`SendSocketEx`/`GetStdItem(Name)` |

**未重复声明**（既有）：`m_nGold` → `ObjBase.OnlineMsg.cs:44`（`uint`，与原文 `LongWord` 一致）；
`m_nGameGold` → `ObjBase.OnlineMsg.cs:36`（**类型偏差**：托管 `int`、原文 `LongWord`，见 §8-C）。

**`TCreature.PlayerSurface.Items.cs`**

| 成员 | 原文 `文件:行` | 虚 | 备注 |
|---|---|---|---|
| `GetMaxBagCount()` | `ObjBase.pas:630/26738-26741` + `ObjPlayer.pas:1264` | ★ `virtual`（`TPlayObject` `override`） | 原文 `TPlayObject` 是 `override` |
| `IsEnoughBag()` | `ObjBase.pas:630/13691-13696` | 非虚（原文无） | `<` 严格小于 |
| `IsEnoughBagEx(int)` | `ObjBase.pas:13698-13703` | — | `<=`（**可等于**，与上者不同） |
| `AddItemToBag(TUserItemView)` | `ObjBase.pas:708/26743-26752` | ★ `virtual` | 原文 `:708` 带 `virtual` |
| `CheckItems(string, out TUserItemView?)` | `ObjBase.pas:752/41668-41684` | — | 返回下标（`-1` = 原文 `nil`） |
| `CheckItemsIndex(...)` | 同上 | — | 便利入口（**非原文成员名**） |
| `IsAddWeightAvailable(int)` | `ObjBase.pas:570/41968-41974` | — | 不做负数守卫 |
| `WeightChanged()` | `ObjBase.pas:35605` | — | 最小接缝 |
| `Bag` / `AddToBag` / `BagItems` | `ObjBase.pas:322` | — | **接缝式容器**，见 §8-A |
| `SendAddItem(TUserItemView)` | `ObjPlayer.pas:1196/3360-3394` | — | 逐行含聚灵珠三段阈值 |
| `SendDelItem(TUserItemView)` | `ObjPlayer.pas:1197/12640-12657` | — | 含 `btValue[13]` 魔法下标 |
| `CheckItemsNeed(ref TStdItem)` | `ObjPlayer.pas:16298-` | — | `Need` = 6/60/7/70/8/81 六个分支 |

**未重复声明**（既有）：`m_UseItems` → `Engine/RecalcChain.cs:101`（`TUserItemView?[21]`，
**类型/槽位数偏差**见 §7-B）；`m_ItemList` → `Engine/ObjBase.cs:166`。

### 2.3 片 3 —— NPC 会话与脚本标签（`TPlayObject.PlayerSurface.NpcSession.cs`）

| 成员 | 原文 `文件:行` | 虚 | 备注 |
|---|---|---|---|
| `m_Script` | `ObjPlayer.pas:116` | — | 复用 `GXX.M2Server.Npc.TScript`（`ObjNpcSeams.cs:81`） |
| `m_NPC` / `m_ItemBoxNpc` | `ObjPlayer.pas:117/118` | — | `TCreature?`（`TBaseObject` 未切出） |
| `m_boBreakLoopGoto` | `ObjPlayer.pas:148` | — | |
| `m_sScriptCurrLable` / `m_sScriptGoBackLable` | `ObjPlayer.pas:150/151` | — | |
| `m_sLastGotoLabel` / `m_dwLastGotoLabelTick` / `m_nOneLabelGotoCount` | `ObjPlayer.pas:152/153/154` | — | tick 初值 = `MyGetTickCount`（`:1651`，**非 0**） |
| `m_CanJmpScriptLableList` / `m_CanRequestStdItemList` | `ObjPlayer.pas:146/147` | — | `List<string>` / `List<int>` |
| `m_sScriptLable` / `m_sInputData` | `ObjPlayer.pas:269/270` | — | |
| `m_sYesLable` / `m_sNoLable` / `m_boMessageBox` | `ObjPlayer.pas:272/273/274` | — | |
| `m_QuestFlag` | `ObjPlayer.pas:196`（`Grobal2.pas:4116`） | — | `byte[128]` |
| `GetQuestFlagStatus(int)` | `ObjPlayer.pas:1155/6202-6220` | — | **`SizeOf` 原样照抄**，见 §7-C |
| `SetQuestFlagStatus(int,int)` | `ObjPlayer.pas:1154/6222-6241` | — | `not(128 shr n14)` 按 8 位截断 |
| `SetScriptLabel(string)` | `ObjPlayer.pas:15172-15176` | — | |
| `GetScriptLabel(string)` | `ObjPlayer.pas:1253/15179-15232` | — | 含原文缺陷 D-P6-1，见 §7-D |
| `Initialize()` | `ObjBase.pas:768/32881-32906` | ★ `override` | 覆写以承接原文 `:32890-32895` 的魔术段 |
| `PlayerSurfaceNpcSeams` | — | — | `GotoLable` / `MyGetTickCount` / `SendFirstMsgToClient` |

**未重复声明**（既有）：`m_boOffLine` / `m_boDummyObject` → `ObjBase.OnlineMsg.cs:10/13`；
`m_nScriptGotoCount` / `m_sRandomString` → `NpcScriptState.cs:31/32`（**另一个类** `TNpcScriptState`，非 `TPlayObject`）。

### 2.4 片 4 —— 英雄 / 副将（`TPlayObject.PlayerSurface.Hero.cs`）

| 成员 | 原文 `文件:行` | 备注 |
|---|---|---|
| `m_MyHero` | `ObjPlayer.pas:372` | `TCreature?`；原文类型是 **`TBaseObject`**（非 `THeroObject`） |
| `m_sHeroName` | `ObjPlayer.pas:374` | |
| `m_sTempHeroName` | `ObjPlayer.pas:375` | |
| `m_sDeputyHeroName` | `ObjPlayer.pas:380` | |
| `m_boWaitHeroDate` | `ObjPlayer.pas:275` | |

### 2.5 `TCreature` 基类成员片（`TCreature.PlayerSurface.Base.cs`）

| 成员 | 原文 `文件:行` | 虚 | 备注 |
|---|---|---|---|
| `m_wAppr` | `ObjBase.pas:162` | — | |
| `m_LastHiter` / `m_CurrTarget` | `ObjBase.pas:293/357` | — | `TCreature?` |
| `m_ActorIcons` | `ObjBase.pas:362`（`Grobal2.pas:3284`/`:50`） | — | `TActorIcon[10]`，初值 `nFileIndex=-1 / nIconCount=1`（`:11473-11478`） |
| `m_boAddtoMapFail` / `m_nCharStatus` | `ObjBase.pas:32898/32901` | — | |
| `NewDefaultActorIcons()` | `ObjBase.pas:11473-11478` | — | `ClearObject` 等价 |
| `Initialize()` | `ObjBase.pas:768/32881-32906` | ★ **`virtual`** | `TBoxMonster`/`TNormNpc`/`TCastleOfficial`/`TGuildOfficial` 的 `inherited` 依赖 |
| `AbilCopyToWAbil()` | `ObjBase.pas:32876-32879` | ★ `virtual` | 接缝（`m_Abil` 未切出） |
| `RecalcAbilitys()` | `ObjBase.pas:778/18178-` | ★ **`virtual`** | 默认转调 `RecalcChain` 既有扩展方法 |
| `FeatureChanged()` | `ObjBase.pas:681/32929-32932` | **非虚**（原文 :681 无 `virtual`） | 照抄不升级 |
| `GetPoseCreate()` ×3 | `ObjBase.pas:645-647/26888-26912` | 非虚 | 无参 / `TCreature?` / `uint` 三个重载 |
| `GetFrontPosition(out,out)` | `ObjBase.pas` 内 `TBaseObject` | — | 用既有方向表 `ObjBase.cs:122-127` |
| `TurnTo(int)` / `TurnToEx(int)` | `ObjBase.pas:679-680/33382-33392` | 非虚 | 含 `Integer`→`Byte` 静默窄化 |
| `SendRefMsg(int,long,long,long,long,string,uint=0)` | `ObjBase.pas:586/30980-` | 非虚 | **签名按原文**；实现体接缝 |
| `PlayerSurfaceBaseSeams` / `PlayerSurfaceConst` / `TActorIconArrayConst` | — | — | 接缝与常量 |

**未重复声明**（既有）：`m_btDirection` → `ObjBase.cs:19`；`m_btRaceImg` → `ObjBase.cs:21`；
`m_PEnvir` → `ObjBase.cs:24`；`m_btRaceServer` → `MagicModel.cs:52`（经 `TSpellCaster` 继承面）。

---

## 3. 已在 main 上存在而**未**重复添加的成员清单

**开工前**（工作树 = `170cd9cb`）已核对并复用：

```powershell
git grep -n "\bm_ZVal\b" main -- 'GXX.CSharp/src/GXX.M2Server/Engine/*.cs'
# main:.../CombatPower.cs:556  /// <summary>m_ZVal: array[0..499] of string[100]（Z 私有字符串型，1 天 1 清）。</summary>
# main:.../CombatPower.cs:557  public readonly string[] m_ZVal = new string[500];
git grep -n "\bm_nGold\b" main -- '...'      # ObjBase.OnlineMsg.cs:44  public uint m_nGold;
git grep -n "\bm_nGameGold\b" main -- '...'  # ObjBase.OnlineMsg.cs:36  public int m_nGameGold;
git grep -n "\bm_UseItems\b" main -- '...'   # RecalcChain.cs:101  public TUserItemView?[] m_UseItems = ...;
git grep -n "\bm_ItemList\b" main -- '...'   # ObjBase.cs:166  public List<TUserItem> m_ItemList = new();
git grep -n "\bRecalcAbilitys\b" main -- '...' # RecalcChain.cs:32 扩展方法
```

**交付前对当前 main（`7824ede2`）复核 71 个新成员名**，只有 3 个命中既有（均为上述已复用项，无 CS0102）：

| 命中 | 含义 | 处置 |
|---|---|---|
| `m_ZVal` (2) | `CombatPower.cs:556/557` | **未声明**，只加 `ClearZValues/SetZVal/GetZVal` |
| `m_UseItems` (6) | `RecalcChain.cs:101` 等 | **未声明**，直接复用（`UpgradeWapon` 需要的读写已可用） |
| `RecalcAbilitys` (5) | `RecalcChain.cs:32` 扩展方法 | **不冲突**（扩展方法 ≠ 实例方法），但见 §0-3 的调用目标切换提示 |

其余 68 个成员名在 main 的 `Engine/*.cs` 内**均为空**（无重复）。

---

## 4. 新增文件 + 每片的覆盖行号范围

| 文件 | 行数 | 覆盖的原文行号范围 |
|---|---|---|
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.Vars.cs` | 208 | ObjPlayer.pas **119 / 123 / 125 / 127 / 260 / 1627 / 1833 / 1835 / 3849**；ObjNpc.pas **9394 / 9417** |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.VarDefaults.cs` | 76 | ObjPlayer.pas **119-127**（默认值语义） |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.Gold.cs` | 250 | ObjPlayer.pas **48 / 201 / 246 / 250 / 1168-1171 / 2529-2547 / 3231-3260 / 3284-3303 / 3376-3380 / 3388-3392**；ObjBase.pas **105-106 / 11311** |
| `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs` | 545 | ObjBase.pas **322 / 570 / 630 / 708 / 752 / 882 / 13691-13703 / 26738-26752 / 35605 / 41668-41684 / 41968-41974**；ObjPlayer.pas **1196-1197 / 1264 / 3360-3394 / 12640-12657 / 16298-16340**；Grobal2.pas **51 / 102 / 4169** |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.NpcSession.cs` | 389 | ObjPlayer.pas **116-118 / 146-154 / 196 / 269-274 / 1154-1155 / 1253 / 1646-1652 / 6202-6241 / 15172-15232**；Grobal2.pas **4116**；ObjBase.pas **32881-32906** |
| `Engine/PlayerSurface/TPlayObject.PlayerSurface.Hero.cs` | 47 | ObjPlayer.pas **275 / 372 / 374 / 375 / 380 / 1698-1702** |
| `Engine/PlayerSurface/TCreature.PlayerSurface.Base.cs` | 481 | ObjBase.pas **101 / 162 / 293 / 357 / 362 / 586 / 645-647 / 679-681 / 768 / 778 / 11473-11478 / 26888-26912 / 30980 / 32876-32932 / 33382-33392**；Grobal2.pas **50 / 3284** |
| `tests/.../PlayerSurfaceVarsTests.cs` | 218 | 片 1 |
| `tests/.../PlayerSurfaceGoldTests.cs` | 295 | 片 2（金额） |
| `tests/.../PlayerSurfaceItemsTests.cs` | 767 | 片 2（物品） |
| `tests/.../PlayerSurfaceNpcSessionTests.cs` | 358 | 片 3 |
| `tests/.../PlayerSurfaceHeroTests.cs` | 523 | 片 4 + `TCreature` 基类片 |

**每个源文件的头 40 行内**均写明源单元路径、LF 实测行数与**逐条原文行号**（覆盖率审计要求）。

---

## 5. 测试用例数 + build/test 结果

| 项 | 数值 |
|---|---|
| 基线（本工作树实跑，`170cd9cb`） | `GXX.M2Server.Tests` **7,214 passed / 0 failed** |
| 交付（`61d3ab20`） | **7,367 passed / 0 failed**（+153） |
| 本车道 `PlayerSurface*` 子集 | **153**（[Fact] 146 + [Theory] 7 行 `InlineData`） |
| `dotnet build GXX.slnx -c Debug` | **Build succeeded，0 Error(s)** |

差异断言（"看起来一样实则不同"）清单 —— 每条都有独立用例：

1. `m_TVal`（`string[100]`，截断）vs 直接写数组（不截断）：`TVal_ShortStringTruncation_DiffersFromPlainString`
2. `m_ZVal` 元素 `null`（托管）vs `''`（原文）：`ZVal_ManagedElementDefaultIsNull_DiffersFromDelphiEmptyString`
3. `m_sString`（无上限）vs `m_TVal`（100 上限）：`SString_IsNotShortString_HasNoTruncation`
4. `IncGold` Cardinal **回绕** vs Int64 加法：`IncGold_CardinalWraparound_DiffersFromInt64Addition`
5. `DecGold`（不够时**原地不动**）vs `DecGameGold`（不够时**夹到 0**）：`DecGameGold_NotEnough_ClampsToZero_DiffersFromDecGold`
6. `IncGameGold` 只夹 `High(LongWord)`、**不受 `m_nGoldMax` 约束**
7. `IsEnoughBag`（`<`）vs `IsEnoughBagEx`（`<=`）：`IsEnoughBagEx_CountEqualsCapacity_True_DiffersFromIsEnoughBag`
8. `CheckItems` 用 Delphi `CompareText`（**大小写不敏感**）vs 序数比较
9. `SendDelItem` 的 `btValue[13] = 1` **且** `Name <> ''` 双条件
10. `SendAddItem` 聚灵珠三段阈值（`> 0` / `= 49` / `<`）的每一段
11. `TurnTo` 的 `Integer`→`Byte` **静默截断**（256→0、-1→255）
12. `GetPoseCreate(0)` —— Delphi `for I := 0 to -1` 零次迭代 vs 直觉的"至少探测一次"
13. `GetQuestFlagStatus(1025)` —— `SizeOf` 守卫边界（**返回 0 且不越界**）
14. `CheckItemsNeed(Need=81, NeedLevel=0)` —— 默认通过（见 §7-E）
15. `AddItemToBag` 的**引用语义**（托管 `TUserItemView`）与原文 `TList(pTUserItem)` 一致
16. `m_UseItems` 新槽为 `null` 而非"全零 TUserItem"

---

## 6. 哪些下游阻塞已解除（对照 ObjNpc 报告 §8.6 逐条打勾）

### (1) `TMerchant.UserSelect`（2087-2900，814 行）

| 需要成员 | 状态 |
|---|---|
| `m_sCharName` | ✅ 既有 |
| `m_nInteger` | ✅ 既有 |
| `m_sString` | ✅ **本车道补齐** |
| `m_nGameGold` | ✅ 既有（`ObjBase.OnlineMsg.cs:36`） |
| `m_nBigStoragePage` / `m_nDealGoldPose` | ✅ **本车道补齐** |
| `m_boWaitHeroDate` | ✅ **本车道补齐** |
| `m_sHeroName` / `m_sDeputyHeroName` / `m_sTempHeroName` | ✅ **本车道补齐** |
| `m_sAutoSendMsg` | ✅ 既有（`ObjBase.OnlineMsg.cs:41`） |
| `GameGoldChanged()` | ✅ **本车道补齐** |
| `SendMsg(...)` | ⚠ **未解** —— 原文 `TBaseObject.SendMsg(BaseObject,wIdent,wParam..,sMsg)`（`ObjBase.pas:30339`）含 `m_boOffLine/m_boDummyObject` 白名单与视野下发，托管侧 `TCreature.SendMsg` 是**入队版**且无 `BaseObject` 形参。已落 `PlayerSurfaceMsgSeams.SendDefMessage` 等接缝与 `SendRefMsg`，但**没有**同名同签名的 `SendMsg`（避免与 `ObjBase.cs:48` 的既有 `SendMsg` 重载混淆）。**需集成方裁定命名** |
| `SysMsg(...)` | ✅ 既有（`ObjBase.OnlineMsg.cs:25`，`TCreature.SysMsg(string,TMsgColor,TMsgType)`） |
| `GetPoseCreate()` | ✅ **本车道补齐** |

**结论：解除 11/12**（只剩 `SendMsg` 的签名/命名裁定）。

### (2) `TMerchant.ClientBuyItem`（3367-3689，323 行）

| 需要成员 | 状态 |
|---|---|
| `m_sCharName` | ✅ |
| `m_nGold` | ✅ 既有 |
| `AddItemToBag(...)` | ✅ **本车道补齐**（`TUserItemView` 形参，见 §8-A） |
| `IsEnoughBag()` | ✅ **本车道补齐** |
| `IsAddWeightAvailable(...)` | ✅ **本车道补齐** |
| `SendAddItem(...)` | ✅ **本车道补齐** |
| `SendMsg(...)` | ⚠ 同 (1) |

**结论：解除 6/7**。**报告特别指出的 `ClientSellItem`（3798-3868）现在只差 `SendMsg`** —— 它要的
`IncGold`/`m_nGold`/`m_sCharName` 全部就绪。

### (3) `TMerchant.UpgradeWapon` 外层体（1830-1901，72 行）

| 需要成员 | 状态 |
|---|---|
| `m_sCharName` / `m_ItemList` | ✅ 既有 |
| `m_nGold` | ✅ 既有 |
| `m_UseItems[U_WEAPON]`（**读写**） | ✅ **可用**（复用 `RecalcChain.cs:101`；`U_WEAPON = 1` 在 21 槽内） |
| `CheckItems(name)` | ✅ **本车道补齐** |
| `DecGold(n)` / `GoldChanged()` | ✅ **本车道补齐** |
| `SendDelItem(@item)` | ✅ **本车道补齐** |
| `RecalcAbilitys()` / `FeatureChanged()` | ✅ **本车道补齐** |
| `SendMsg(...)` / `SysMsg(...)` | ⚠ / ✅ |
| `GotoLable(...)` | ❌ ObjNpc 自身 9263-9574 未移植（**不属本车道**） |
| `g_ItemRules.Get` / `g_CastleManager.IncRateGold` / `g_boGameLogGold` / `g_sCannotUpgradeWeapon` | ❌ M2Share/ItemRules 侧（**不属本车道**） |

**结论：本车道该给的 8 项全部解除**；余下 2 项属 ObjNpc/宿主侧。

### (4) `TNormNpc.GotoLable`（9263-9574，312 行）

| 需要成员 | 状态 |
|---|---|
| `m_sCharName` | ✅ |
| `m_nVal`（含 `FillChar` 整块清零） | ✅ **本车道补齐**（`m_nVal` + `ClearNValues()`） |
| `m_NPC` / `m_Script` | ✅ **本车道补齐** |
| `m_ItemBoxNpc` | ✅ **本车道补齐** |
| `m_boOffLine` / `m_boDummyObject` | ✅ 既有（`ObjBase.OnlineMsg.cs:10/13`） |
| `m_dwLastGotoLabelTick` / `m_nOneLabelGotoCount` / `m_sLastGotoLabel` | ✅ **本车道补齐** |
| `m_sScriptParams` | ⚠ **原文不存在该字段** —— 见 §7-F（原文是 GotoLable 内的**过程级局部** `array[0..8] of string`），本车道**未伪造字段** |
| `GetQuestFlagStatus(flag)` | ✅ **本车道补齐** |
| `GetScriptLabel(sMsg)` | ✅ **本车道补齐**（含原文缺陷 D-P6-1，见 §7-D） |
| `SendFirstMsg(...)` | ❌ **原文不存在 `SendFirstMsg`**（全仓 `git grep` 0 命中）；原文 `GotoLable` 用的是 `SendMsg`/`SysMsg` |
| `SendMsg(...)` | ⚠ 同 (1) |

**结论：解除 9/10**（`SendFirstMsg` 系任务书列举错误，非真实缺口）。

### §8.8 / §8.10 的 Engine 缺口

| 缺口 | 状态 |
|---|---|
| `TCreature.Initialize`（**虚方法**） | ✅ **本车道补齐**（`public virtual`，`TPlayObject` `override`） |
| `TCreature.m_ActorIcons`（`@m_ActorIcons`） | ✅ **本车道补齐**（`TActorIcon[10]`，`nFileIndex=-1`/`nIconCount=1`） |
| `TCreature.m_CurrTarget` / `m_LastHiter` / `GetPoseCreate()` | ✅ **本车道补齐** |
| `TCreature.TurnTo(Integer)` / `SendRefMsg(...)` | ✅ **本车道补齐**（签名按原文） |
| `TBaseObject.m_wAppr` | ✅ **本车道补齐**（落在 `TCreature`） |
| `TPlayObject.m_MyHero` | ✅ **本车道补齐** |
| `g_CastleManager.GetCastleNameList` | ❌ **未能补**（`TCastleManager` 非 `partial`，见 §8-B） |
| `TPlayObject.m_btPermission` | ✅ 既有（`ObjBase.OnlineMsg.cs:35`） |
| `TPlayObject.LableIsCanJmp` / `m_sScriptGoBackLable` / 6 个 `sNF_*` 常量 | `m_sScriptGoBackLable` ✅ **本车道补齐**；`LableIsCanJmp`（`ObjPlayer.pas:15234`）与 `sNF_*` 属 ObjNpc/M2Share 侧，**未做** |

---

## 7. 发现的原文缺陷 / 易错点（带 `文件:行`）

> 续 ObjNpc 报告的 D 系列，编号 **D-P6-n**。

| # | 位置 | 问题 |
|---|---|---|
| **D-P6-1** | `ObjPlayer.pas:15216` | ★★ **`GetScriptLabel` 因该行而恒不生效**。`sLabel := GetValidStr3_Ex(sCmdStr, sCmdStr, '/')` —— Delphi 该函数把**分隔符之前**的部分写进 `Dest`（第二实参 `sCmdStr`），**返回值**是分隔符**之后**的剩余串（`HUtil32.pas:1467/1486-1487/1503`）。于是 `sLabel` 拿到的是**剩下一半**，真正的标签留在 `sCmdStr`。紧接着 `:15226` 判 `sLabel[1] = '@'` → 对任何输入都不成立 → **`m_CanJmpScriptLableList` 恒空**，`LableIsCanJmp`（`:15234`）只可能命中 `@main`/`@HeroMap`/Yes/No 等硬编码项。实测：输入 `<@main/x>` → `sLabel = "x"`、`sCmdStr = "@main"`。**已逐字照抄并单测锁死**（`GetScriptLabel_AtLabels_AreNeverCollected_DueToOriginalDefectAt15216`）—— 若集成方"顺手修好"，该用例会立刻失败（这是刻意的告警）。连带使 `:15219-15224` 的去括号分支与 `:15227` 的 `Trim` 分支**只在特殊输入下可达**（如 `<输入/@@InputInteger1(...)>`，已单测）。 |
| **D-P6-2** | `ObjPlayer.pas:15197` vs `:15199` | 内层 `while (Pos('<',...) > 0) and (Pos('>',...) > 0)`：段必须**同时**含 `<` 与 `>` 才进入循环，而 `:15199` 又只处理「第一个字符不是 `<`」的情况。结果 `<@main>`（外层去掉一对尖括号后无尖括号）**整段被丢弃**。这与 D-P6-1 叠加，使 `GetScriptLabel` 实际上几乎什么都收集不到。 |
| **D-P6-3** | `ObjPlayer.pas:6213` / `:6233` | `if (n10 - SizeOf(TQuestFlag)) < 0 then` —— 写的是 **`SizeOf`（字节数 128）**，本意显然是**元素个数**（`Length`）。`TQuestFlag = array[0..127] of Byte`（`Grobal2.pas:4116`）恰好让二者都等于 128，**侥幸不越界**。若该类型日后改成 `array[0..N] of Word`（字节数 = 2N），`n10` 的合法上界会立刻超标 → **越界写**。已逐字照抄 `128` 字面量（**不**写成 `m_QuestFlag.Length`），并加 `GetQuestFlagStatus(1025)`/`SetQuestFlagStatus(1025,...)` 边界用例。 |
| **D-P6-4** | `ObjPlayer.pas:3241` | `tmpValue := Min(m_nGold + tGold, MAXDWORD - 1)` —— 加法是 **Cardinal（32 位无符号）加法**，`Min` 之前**已经回绕**。`m_nGold=4e9 + tGold=1e9` → `705_032_704`，与 Int64 语义的 `4_294_967_294` 相差极大。托管侧必须 `unchecked`；已用两例锁死（回绕、回绕到 0）。另注：上界是 **`MAXDWORD - 1`**（`4294967294`）**不是** `MAXDWORD`。 |
| **D-P6-5** | `ObjPlayer.pas:3284-3293` vs `:3295-3303` | 同族两个「扣钱」语义**不一致**：`DecGold` 不够时**原地不动**；`DecGameGold` 不够时**夹到 0**。已加对照差异断言。 |
| **D-P6-6** | `ObjPlayer.pas:16340` | `CheckItemsNeed` 的 `Need = 81` 分支：`StdItem.NeedLevel = 0`（DB 未设置）时 `LoWord=0`、`HiWord=0` → 与默认 `m_nMemberType=0`/`m_nMemberLevel=0` 比较**两条件皆假** → **默认通过**（会员限制形同虚设）。已单测锁死。 |
| **D-P6-7** | `ObjPlayer.pas:12651` | `if (UserItem.btValue[13] = 1) and (UserItem.Name <> '')` —— 用 `btValue[13]` 这个**魔法下标**决定用自定义名还是标准名，无具名常量（与 ObjNpc 报告 **D23** 同型）。 |
| **D-P6-8** | `ObjPlayer.pas:15187` | `// m_CanRequestStdItemList.Clear;` 是**注释掉的**，而上一行 `m_CanJmpScriptLableList.Clear` 是**活的**。结果两条列表的清理策略不对称：`m_CanRequestStdItemList` 会**跨调用累加**。已加差异断言（同一实例两次调用 = 2 项）。 |
| **D-P6-9** | `ObjBase.pas:41968-41974` | `IsAddWeightAvailable` **不做负数守卫**：`nWeight` 为大负数时 `Weight + nWeight <= MaxWeight` 更易成立 → 返回 **True**（"减重"能让超重角色通过校验）。已锁死。 |
| **D-P6-10** | `ObjBase.pas:11473-11478` | `m_ActorIcons` 先 `FillChar(...,0)` 再逐元素设 `nFileIndex := -1` —— 前者的清零被后者**部分覆盖**，最终有效默认值是 `-1`（`$FFFF`）而**不是** `FillChar` 的 0。若有人只照抄 `FillChar` 会得到 `nFileIndex = 0`（合法资源号）→ 顶戴花翎会**误显示第 0 号图标**。本车道按最终生效值实现。 |
| **D-P6-11** | `ObjBase.pas:679-680` / `:33382-33392` | `TurnTo(nDir: Integer)` 内 `m_btDirection := nDir`（`Byte` ← `Integer`）是 **Delphi 隐式窄化**：`256→0`、`-1→255`，且 `SendRefMsg` 里传的仍是**未截断的 `nDir`** → 广播出去的方向值与本机记录的方向值**可能不同**。已用三个用例锁死。 |
| **D-P6-12** | `ObjPlayer.pas:1651` | `m_dwLastGotoLabelTick := MyGetTickCount`（**不是 0**，且**没有括号**）—— 与相邻的 `m_sLastGotoLabel := ''` 形成"字符串清零、tick 取当前时间"的不对称初始化。若照抄成 0，跳转防环的第一次判定会**立刻超时**。 |
| **D-P6-13** | `ObjBase.pas:32899` | `Initialize` 里 `if m_PEnvir.CanWalk(...) and AddToMap() then` **不判 `m_PEnvir = nil`**（原文靠调用约定保证非空）。托管 `m_PEnvir` 是可空引用；本车道在 `null` 时只置 `m_boAddtoMapFail = True` 并跳过后续，**不**让它变成 NRE 打断整个 `Initialize`（否则下游 NPC 的 `inherited Initialize` 会直接崩）。该偏差已登记。 |
| **D-P6-14** | `ObjPlayer.pas:3373/3384` | `SendAddItem` 里 `UserItemToClientItem(...)` 与紧随其后的 `ClientItem.btValue[10] := 0` 是**一对**：`btValue[10]` 被清零是**故意**的（注释：防止武器升级后通过查找内存判断是否成功）。若把这两行的顺序颠倒或省略清零，会**泄露武器升级结果**（客户端可探测）。 |
| **D-P6-15** | `ObjPlayer.pas:15234-` | `LableIsCanJmp` 把 `'@main'`/`'@HeroMap'`/`m_sYesLable`/`m_sNoLable` 四个值**硬编码**在代码里（不在脚本/配置中）—— 与 D-P6-1 叠加后，这四个是**唯一**可能通过白名单的标签。 |

---

## 8. 接缝清单 + 需要集成方处理的**精确签名要求**

### (A) ★ `m_ItemList` 容器整合（**最高优先**）

**现状**：托管侧有两个近义容器 ——

| 位置 | 类型 | 调用方 |
|---|---|---|
| `Engine/ObjBase.cs:166` | `List<TUserItem> m_ItemList` | **零**（`git grep -n "m_ItemList" main -- 'GXX.CSharp/src/**/*.cs'` 只命中声明本身） |
| 本车道 `TCreature.PlayerSurface.Items.cs` | `protected virtual List<TUserItemView> BagItems => m_BagItems;` | `AddItemToBag`/`IsEnoughBag`/`CheckItems` |

**为什么没有直接把 `BagItems` 接到 `m_ItemList`**：`Engine/ObjBase.cs` 对本车道**只读**，
而两者元素类型不同（`TUserItem` 值类型 vs `TUserItemView` 引用类型），
无法用 `override` 桥接（C# 属性重写要求类型一致）。

**要求（二选一）**：

```csharp
// 方案 1（推荐）：改 ObjBase.cs:166 一行，让 TPlayObject.m_ItemList 成为唯一背包容器
public List<TUserItemView> m_ItemList = new();      // 原：List<TUserItem>
// 然后删除 TCreature.PlayerSurface.Items.cs 里的 m_BagItems / Bag / AddToBag，
// 并把 BagItems 改为：protected override List<TUserItemView> BagItems => m_ItemList;
```

```csharp
// 方案 2：保留两份容器，但必须在文档/契约里明确「背包 = BagItems」，
//         并把 m_ItemList 标注为历史遗留（不推荐 —— 同一概念两份数据迟早出错）
```

> 在方案 1 落地前，**ObjNpc 的 `ClientBuyItem` 请使用本车道的 `AddToBag(view)` 或
> `AddItemToBag(view)`**，不要写 `m_ItemList`。

### (B) ★ `TCastleManager.GetCastleNameList` —— **`TCastleManager` 不是 `partial`**

**实测**：`GXX.CSharp/src/GXX.M2Server/Engine/Castle.cs:406` = `public class TCastleManager`
（**没有 `partial`**）。因此车道**无法**按 partial 手法在自己的文件里补该方法。
按您的要求：**车道未改 `Castle.cs`**，在此给出精确签名要求。

**要求**：把 `Castle.cs:406` 改为 `public partial class TCastleManager`，**或**直接在
`Castle.cs` 内追加下述方法（1:1 对应原文 `Castle.pas` 的 `g_CastleManager.GetCastleNameList`）：

```csharp
/// <summary>原文 Castle.pas 的 TCastleManager.GetCastleNameList —— 把城堡名列表写入 list。</summary>
public void GetCastleNameList(TStringList list)
{
    // 逐项添加 m_CastleList 中各城堡的 m_sName（顺序 = 列表插入顺序）
}
```

**消费方**：`GXX.M2Server.Npc.NpcSeams.GetCastleNameList`（ObjNpc 车道）目前是占位，
补上后可从占位改为**转调真实现**。
**当前状态下的可用替代**：本车道**未**提供 `GetCastleNameList`（无 partial 落点），
ObjNpc 的 `$REQUESTCASTLELIST`（`ObjNpc.pas:10055-10089`）在接入前仍需其自有接缝。

### (C) `m_nGameGold` 的类型不一致（登记，非阻塞）

| 位置 | 类型 | 原文 |
|---|---|---|
| `Engine/ObjBase.OnlineMsg.cs:36` | `public int m_nGameGold;` | `ObjPlayer.pas:202` `m_nGameGold: LongWord;` |

**要求**：建议改成 `public uint m_nGameGold;` 并把 `DbLayer` 的 `m_nGameGold`/`GetGameGold` 系列
（`DbLayer/DbSeam.cs:118` 一带）一并核对。**本车道的方法（`IncGameGold`/`DecGameGold`）
已按 `uint` 语义实现并显式做 `(uint)`/`unchecked((int))` 转换**，所以改成 `uint` 后
只需删掉本车道那两个方法里的转换——**行为不变**。

### (D) `m_UseItems` 的槽位数与元素类型偏差（登记，非阻塞）

| 位置 | 现状 | 原文 |
|---|---|---|
| `Engine/RecalcChain.cs:101` | `TUserItemView?[UseSlots.SlotCount]`，`SlotCount = 21` | `ObjBase.pas:882` `THumanUseItems = array[0..29] of TUserItem`（30 个**值**元素） |

- **槽位数**：21 vs 30。原文 21..29 三档在**已移植路径里没有读写点**（`grobal2.pas:4169`
  注释"加盾牌 原为0..15"），`UpgradeWapon` 用的 `U_WEAPON = 1` 在两者范围内。
  **建议**：把 `UseSlots.SlotCount` 改成 30 以贴原文（**本车道未改**，`RecalcChain.cs` 只读）。
- **元素类型**：`TUserItemView?` 新槽默认 **`null`**，原文值数组默认是**全零 `TUserItem`**。
  调用方（如 NPC 写 `User.m_UseItems[U_WEAPON].wIndex := 0`）必须先
  `m_UseItems[UseSlots.U_WEAPON] ??= new TUserItemView()`。
  **建议**：在 `RecalcChain.cs` 或构造路径里把 30 个槽预填成 `new TUserItemView()`
  （同样需要 `RecalcChain.cs` 的写权限）。

### (E) 接缝默认实现一览（可直接注入，**请复用勿另造**）

| 接缝类 | 成员 | 精确签名 | 原文出处 |
|---|---|---|---|
| `PlayerSurfaceMsgSeams` | `SendUpdateMsg` | `Action<TCreature,int>` | `ObjPlayer.pas:2531` 等 |
| | `SendDefMessage` | `Action<TPlayObject,ushort,long,ushort,ushort,ushort,string>` | `:12655` 等 |
| | `SendSocketEx` | `Action<TPlayObject,object>` | `:3386` |
| | `GetStdItemName` | `Func<int,string>` | `ObjBase.pas:41678` |
| | `GetStdItem` | `Func<int,TStdItem?>` | `ObjPlayer.pas:3369/12648` |
| `PlayerSurfaceBaseSeams` | `SendRefMsg` | `Action<TCreature,int,long,long,long,long,string,uint>` | `ObjBase.pas:30980` |
| | `GetMovingObject` | `Func<TEnvirnoment,int,int,TCreature?,bool,TCreature?>` | `:26893/26901` |
| | `AddToMap` | `Func<TEnvirnoment,int,int,TCreature,bool>` | `:32899` |
| | `GetCharStatus` | `Func<TCreature,int>` | `:32901` |
| | `AddBodyLuck` | `Action<TCreature,int>` | `:32902` |
| | `LoadSayMsg` / `MonsterSayMsg` | `Action<TCreature>` | `:32903/32905` |
| | `AbilCopyToWAbil` | `Action<TCreature>` | `:32876` |
| | `RecalcAbilitys` | `Action<TCreature>`（默认转调 `RecalcAbilitysChain`） | `:18178` |
| | `InitializeMagicLevelClamp` | `Action<TCreature>` | `:32890-32895` |
| | `WeightChanged` | `Action<TCreature>` | `:35605` |
| | `GetMaxBagCount` | `Func<TCreature,int>`（默认 `DEF_MAX_BAG_ITEM = 48`） | `:26740` |
| | `IsCopyMon` | `Func<TCreature,bool>` | `:32914` |
| `PlayerSurfaceItemSeams` | `UserItemToClientItem` | `Func<TUserItemView,TStdItem,object?>` | `ObjPlayer.pas:3373` |
| | `ItemMakeIndex` / `ItemName` / `ItemDura` / `ItemDuraMax` | `Func<TUserItemView,int/string/ushort/ushort>` | `:3376/12651/3388` |
| | `FunctionNPC` / `FunctionNpcGotoLable` | `object?` / `Action<object,TPlayObject,string,bool>` | `:3374/3378` |
| | `IncBeadExp` | `Action<TPlayObject,uint>` | `:3392` |
| | `BlankClientItemBtValue10` | `Action<object>?` | `:3384` |
| | `IsCastleMember` / `MyGuild` / `GuildRankNo` / `MemberType` / `MemberLevel` | 见源码 | `:16303-16340` |
| `PlayerSurfaceNpcSeams` | `GotoLable` | `Func<object,TPlayObject,string,bool,bool>` | `ObjNpc.pas:9263` |
| | `MyGetTickCount` | `Func<uint>`（默认 `DelphiRTL.GetTickCount`） | `m_dwLastGotoLabelTick` |
| | `SendFirstMsgToClient` | `Action<TPlayObject,string>` | 预留 |
| `PlayerSurfaceVarDefaults` | `MigrateStringVarDefaults` | `void (TPlayObject)`（幂等，**需构造末尾调用**） | `ObjPlayer.pas:119-127` |

调用方均可用 `ResetDefaults()` 隔离（全部 5 个测试文件已示范）。

### (F) 需集成方在**构造路径**加的一行（`null` → `''` 默认值修正）

**问题**：`ObjPlayer.pas:123/125/127` 的 `m_TVal`/`m_ZVal`/`m_sString` 是 Delphi
`string[100]` / `string`，**元素默认 `''`**；托管 `string[]` **元素默认 `null`**（ObjNpc 报告 D18 已登记）。
`null` 与 `''` 在 `Length`/`CompareText` 类调用上**行为不同**（前者抛 NRE）。

**为什么本车道没能自动修好**：字段声明在 `Engine/CombatPower.cs:557`（只读），
而 `Engine/ObjBase.cs:169` 已有 `public TPlayObject()`；本工程是多 partial 合并，
再声明无参构造会 **CS0111**，声明 `TPlayObject(bool = true)` 又会让部分构造调用**递归**。

**要求（一行）**：在 `Engine/ObjBase.cs:169` 的 `public TPlayObject()` 体**末尾**追加：

```csharp
PlayerSurfaceVarDefaults.MigrateStringVarDefaults(this);   // null → ''（原文 ShortString/AnsiString 默认值）
```

或在 `Engine/CombatPower.cs:557/…` 旁为每个 `string[]` 字段加初始化器。
**在落地前**：读取请用本车道的 `GetTVal(i)` / `GetZVal(i)`（已归一为 `''`），
或调用一次 `PlayerSurfaceVarDefaults.MigrateStringVarDefaults(player)`。

### (G) 未做的部分（诚实）

| 项 | 原因 |
|---|---|
| `TBaseObject.SendMsg(BaseObject, wIdent, wParam.., sMsg)` | 与既有 `TCreature.SendMsg(ushort,long,long,long,long,string)`（`ObjBase.cs:48`，**入队版**）**同名不同义**。按"不重复/不混淆"原则未覆盖，改由 `SendRefMsg` + `SendDefMessage` 接缝表达。**需集成方裁定命名**（建议 `SendTo(BaseObject, ...)` 或 `SendViewMsg(...)`）。 |
| `g_CastleManager.GetCastleNameList` | `TCastleManager` 非 `partial`（§8-B）。 |
| `LableIsCanJmp`（`ObjPlayer.pas:15234`） | 属 ObjNpc 侧（依赖 `m_sYesLable`/`m_sNoLable`，本车道已提供这两个字段，可直接做）。 |
| `TPlayObject.CheckItemsNeed` 的 `Need` 其余分支（`:16341-16389`） | 依赖未移植的 `m_btNation`/`m_nCreditPoint`/`m_boMarried` 等 20+ 字段，超出本片范围。 |
| `m_ItemBoxItems` / `m_ItemBoxAddIndex` / 交易/挑战/仓库/宠物/师徒/夫妻等其余 `TPlayObject` 字段 | 不在四片清单内（ObjPlayer.pas 共 ~49,000 行，本车道覆盖约 3,000 行）。 |

---

## 9. 诚实说明：未完成部分与剩余量

### 9.1 量化

| 口径 | 数值 |
|---|---|
| 覆盖的原文单元 | `ObjPlayer.pas`（49,232 LF）**部分** + `ObjBase.pas`（43,480 LF）**部分** + `Grobal2.pas` 常量 |
| 本车道覆盖的原文行区间（**逐行 1:1**） | **约 1,150 行**（详见 §4 的逐文件行号表；不含被引用的既有实现） |
| `ObjPlayer.pas` 总覆盖率 | **≈ 2.4%**（`ObjPlayer.pas` 的接口段字段 14-495 已覆盖约 60 行、实现段约 1,000 行） |
| `ObjBase.pas` 相关面 | `TCreature`/`TBaseObject` 的**被 ObjNpc 四优先方法需要的那一小片**（字段 + 7 个方法），**不是**整个 `TBaseObject`（该单元 43,480 行，绝大部分未移植） |
| 新增源码 | 7 文件 / **2,196 行** |
| 新增测试 | 5 文件 / **2,161 行** / **153 用例** |

### 9.2 明确**未做**（不是"差不多做完"）

1. **`TBaseObject` 未切出**：原文的 `TBaseObject`/`TSmartObject` 两层在托管侧**不存在**；
   本车道把它们的成员**落在 `TCreature` 上**（最薄代表），并在每个成员注释里标注了原文归属。
   → 这会让 `TMonster`/`TNpc` 之类的非玩家对象也"拥有"背包/装备等玩家专属概念
   （`BagItems` 默认返回各自私有空表，行为无害，但与原文的类层次**不等价**）。
2. **没有一行接缝被真正接到宿主上**：全部默认实现是「无宿主」。
   唯一例外是 `RecalcAbilitys`（默认转调 `Engine/RecalcChain.cs:32` 既有实现）
   与 `MyGetTickCount`（默认 `DelphiRTL.GetTickCount`）。
3. **`m_UseItems` 的 21 vs 30、`TUserItemView?` vs `TUserItem` 两处偏差未修**（需 `RecalcChain.cs` 写权限，§8-D）。
4. **`m_ItemList` 的双容器问题未修**（需 `ObjBase.cs` 写权限，§8-A）。
5. **`GetScriptLabel` 按原文照抄了缺陷 D-P6-1**，因此**该函数在托管侧同样不生效**。
   本车道**故意不修**（任务书第 1 条"原文笔误保留并注释"），但这是一个**功能级**缺陷
   ——**如果 ObjNpc 的 `GotoLable`/`LableIsCanJmp` 依赖 `m_CanJmpScriptLableList` 的填充，
   那么这条链在原文里本身也是坏的**。请调度方与 ObjNpc 车道确认。
6. **`SendAddItem`/`SendDelItem`/`SendRefMsg` 的实现体是接缝**（未真正下发到客户端），
   `CheckItemsNeed` 只做了 6/60/7/70/8/81 六个分支。
7. **未做条件编译分支核查**：`ObjPlayer.pas` 内有 `{$IFDEF CPUX64}` 等条件块，
   本车道只按 Delphi 7（ANSI）分支实现。

### 9.3 可信度边界

- **可信**：§2 表中所有成员的**声明形状（名/类型/可见性/虚修饰）逐条对照原文**；
  所有**差异断言**都用两个可观测值证明"两条路径不同"；原文缺陷条目均带精确行号且**实测复现过**
  （D-P6-1 用临时探针实测 `sLabel="x"`/`sCmdStr="@main"`，探针文件已删除）。
- **不完全可信**：接缝的**行为取决于未来注入的实现**；本车道只验证了「默认实现下不抛异常」
  与「转发参数正确」。
- **未验证**：任何真实宿主下的端到端 NPC/玩家行为（无 `TEnvirnoment` 完整面、
  无 `TBaseObject`、无客户端管道，无法跑起来）。
- **基线的诚实说明**：本车道**未 rebase**，工作树停在 `170cd9cb`；main 已推进到
  `7824ede2`。门禁用的是**本工作树的工程文件**（`GXX.slnx` 未变），
  `PlayerSurface*` 文件全部是新增，**无基线漂移导致的问题**。
  但 §3 的复核证明：对照**当前** main，本车道的 71 个新成员名里只有 3 个命中既有（且均为已复用项）。

---

## 10. 清理

- 临时探针文件 `tests/GXX.M2Server.Tests/TempProbe.cs`、`TempProbe2.cs` 与
  `TCreature.PlayerSurface.Items.cs.bak` **已删除**（`git status` 干净，`git show --stat HEAD`
  只有 12 个预期文件）。
- 未创建任何 `.recon*`/`.tmp*` 目录。

---

*报告完。本车道全部产出在 `.worktrees/p6-m2-playersurface`，分支 `par/p6-m2-playersurface`，
HEAD = `61d3ab20`；未触碰主工作树、未触碰任何兄弟 `.worktrees\*`。*
