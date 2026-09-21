# 并行报告：车道 `p4-m2-objnpc`（`Source/M2Engine/ObjNpc.pas` → `src/GXX.M2Server/Npc/**`）

> 分支 `par/p4-m2-objnpc` ｜ 工作树 `.worktrees/p4-m2-objnpc`
> 源单元实测 **10,546 LF**（GBK，混用 CRLF/裸 LF，已归一化后计数）
> 产出目录：`GXX.CSharp/src/GXX.M2Server/Npc/**`（8 个 .cs）+ `GXX.CSharp/tests/GXX.M2Server.Tests/Npc*.cs`（4 个）
> 门禁：`dotnet build GXX.slnx -c Debug` **0 error**；`GXX.M2Server.Tests` **5786 passed / 0 failed**（基线 5591 + 本车道 195）

---

## 0. 给调度方的一段话（结论先行）

`ObjNpc.pas` **远未移植完**：本次落地 **34/112 条例程（1,649 行原文）**，
另有 **4 条（4,297 行）** 只落了最小接缝，
剩余 **74 条（4,095 行）未覆盖** —— 其中最大的三块是
`TNormNpc.GetVariableText`(6011-9262，**3,252 行**)、
`TMerchant.UserSelect`(2087-2900，814 行)、
`TNormNpc.GotoLable`(9263-9574，312 行)。
**"纯逻辑优先"的判定是对的**：本次选中的四个方法族（脚本目标级解析、变量系统、标签/排序、商人价格）
都做到了逐行照抄 + 可独立验证，共 195 个用例。

**两件需要调度方决策的事**（详见 §6）：
1. `GXX.Core/Util/HUtil32.cs:581` 的 `CompareLStr` 是**大小写敏感**且**缺 `compn <= 0` 守卫**，
   与原文 `HUtil32.pas:1981-1995` 不符 —— 本车道未改他人文件，改为转调 Engine 里已有的 1:1 实现
   `MonGenParseCore.CompareLStr`，**未造第三份**。
2. `GXX.M2Server.Engine.TNormNpc`（`Engine/NpcScriptEngine.cs:10`，会话 A 的前置最小模型）
   与本车道 `GXX.M2Server.Npc.TNormNpc` **同名不同命名空间** —— 这是 §12.8 那类碰撞的**第 5 次复发**。
   只要某个文件同时 `using` 两个命名空间就 CS0104（本车道的测试文件已实际踩到，用文件级别名消歧）。

---

## 1. 全部 commit hash

| # | commit | 内容 |
|---|---|---|
| 1 | `a83978a7` | 切片1：骨架 —— 接缝层 `ObjNpcSeams.cs` + interface 段记录/类字段 1:1（`ObjNpcTypes.cs`、`ObjNpcClasses.cs`） |
| 2 | `2deff4f3` | 切片2：单元级 4 函数（`LoadLevelScriptAction/Condition`、`GetLevelBaseObjectCondition/Action`、`CheckStrIsVar`）+ 变量族（`GetVarValue`×4、`SetVarValue`、`Get/SetDynamicValue`、`GetValNameValue`、`GetLineVariableText`、`GetDynamicVarList`）+ 105 用例 |
| 3 | `0d0a740c` | 切片3：标签管理/脚本记录快排与二分/脚本错误上报 + 30 用例 |
| 4 | `32ea9330` | 切片4：`TMerchant` 价格/货物族 9 个方法 + 42 用例 |
| 5 | `51ba3534` | 切片5：112 条例程覆盖登记表（脚本抽取 + 回读守卫）+ 18 用例 |

工作树当前 `git status --porcelain` **干净**，临时目录 `.tmp-probe/` 已删除。

---

## 2. 逐方法族判定表（已完成 / 接缝 / 未覆盖）

口径：**Covered** = 在 `Npc/` 内逐行 1:1；**Seam** = 只落最小接缝（委托），未逐行移植；**Missing** = 未覆盖。
完整 112 条机器可读登记见 `src/GXX.M2Server/Npc/ObjNpcRoutineRegistry.cs`（脚本从原文抽取，非手抄）。

### 2.1 Covered —— 34 条 / 1,649 行

| 原文行号 | 例程 | 归属文件 |
|---|---|---|
| 504-508 | `TConditionList.Create` | `ObjNpcTypes.cs` |
| 509-595 | `LoadLevelScriptAction` | `ObjNpcUnitFuncs.cs` |
| 596-681 | `LoadLevelScriptCondition` | `ObjNpcUnitFuncs.cs` |
| 682-856 | `GetLevelBaseObjectCondition` | `ObjNpcUnitFuncs.cs` |
| 857-1106 | `GetLevelBaseObjectAction` | `ObjNpcUnitFuncs.cs` |
| 10406-10509 | `CheckStrIsVar` | `ObjNpcUnitFuncs.cs` |
| 1446-1456 | `TMerchant.AddItemPrice` | `ObjNpcMerchant.cs` |
| 1457-1487 | `TMerchant.CheckItemPrice` | `ObjNpcMerchant.cs` |
| 1488-1511 | `TMerchant.GetRefillList` | `ObjNpcMerchant.cs` |
| 1630-1644 | `TMerchant.CheckItemType` | `ObjNpcMerchant.cs` |
| 1645-1673 | `TMerchant.GetItemPrice` | `ObjNpcMerchant.cs` |
| 2052-2086 | `TMerchant.GetUserPrice` | `ObjNpcMerchant.cs` |
| 3160-3179 | `TMerchant.ClearExpreUpgradeListData` | `ObjNpcMerchant.cs` |
| 3272-3366 | `TMerchant.GetUserItemPrice` | `ObjNpcMerchant.cs` |
| 3793-3797 | `TMerchant.GetSellItemPrice` | `ObjNpcMerchant.cs` |
| 4383-4430 | `TNormNpc.ClearScript` | `ObjNpcLabels.cs` |
| 4443-4456 | `TNormNpc.GetVarValue(var nValue)` | `ObjNpcVars.cs` |
| 4457-4466 | `TNormNpc.GetVarValue(var sValue)` | `ObjNpcVars.cs` |
| 4467-4481 | `TNormNpc.GetVarValue(var sValue,var nValue,var IsBreakParseVar)` | `ObjNpcVars.cs` |
| 4482-4494 | `TNormNpc.GetVarValue(var sVar,var sValue,var nValue)` | `ObjNpcVars.cs` |
| 4495-4511 | `TNormNpc.SetVarValue` | `ObjNpcVars.cs` |
| 4512-4575 | `TNormNpc.GetDynamicValue` | `ObjNpcVars.cs` |
| 4576-4644 | `TNormNpc.SetDynamicValue` | `ObjNpcVars.cs` |
| 5690-5878 | `TNormNpc.GetValNameValue` | `ObjNpcVars.cs` |
| 5934-5952 | `TNormNpc.AllowSelect` | `ObjNpcLabels.cs` |
| 5953-5966 | `TNormNpc.AddSelectLable` | `ObjNpcLabels.cs` |
| 5967-5980 | `TNormNpc.DeleteSelectLable` | `ObjNpcLabels.cs` |
| 5981-6010 | `TNormNpc.GetLineVariableText` | `ObjNpcVars.cs` |
| 9745-9766 | `TNormNpc.ScriptActionError` | `ObjNpcLabels.cs` |
| 9767-9788 | `TNormNpc.ScriptConditionError` | `ObjNpcLabels.cs` |
| 9877-9899 | `TNormNpc.GetDynamicVarList` | `ObjNpcVars.cs` |
| 9955-9995 | `TNormNpc.QuickSortRecordList` | `ObjNpcLabels.cs` |
| 9996-10018 | `TNormNpc.DoSort` | `ObjNpcLabels.cs` |
| 10019-10048 | `TNormNpc.GetSayingRecordFromRecordList` | `ObjNpcLabels.cs` |

**grep 证据（示例）**：

```powershell
Select-String -Path GXX.CSharp\src\GXX.M2Server\Npc\*.cs -Pattern '原文 509-594|原文 9955-9994|原文 3272-3365'
```

### 2.2 Seam —— 4 条 / 4,297 行（只落委托，未逐行移植）

| 原文行号 | 例程 | 行数 | 接缝 |
|---|---|---|---|
| 6011-9262 | `TNormNpc.GetVariableText` | **3,252** | `NpcSeams.GetVariableText`（`Func<TNormNpc,TPlayObject,string,string,int,(bool Result,string SMsg,bool IsBreakParseVar)>`） |
| 4935-5325 | `TNormNpc.SetValNameValue` | 391 | `NpcSeams.SetValNameValue` |
| 5326-5689 | `GetBoxItemValue`（单元级） | 364 | `NpcSeams.GetBoxItemValue` |
| 4645-4934 | `SetBoxItemValue`（单元级） | 290 | `NpcSeams.SetBoxItemValue` |

> 这 4 条**不是"顺手跳过"**：`GetVariableText` 是一张 3,252 行的巨型 `case` 表，
> 依赖 `TEnvirnoment`/`TGuild`/`TGameGoldDeal`/`TFoundryItem`/`g_Config` 等尚未移植的宿主面；
> `Get/SetBoxItemValue` 依赖 `TPlayObject` 的 8 类变量容器中**尚有 4 类在托管侧不存在**（见 §6.3）。
> 已按任务书要求落成"最小接缝 + 单测可注入"，`GetLineVariableText`（已 Covered）即通过该接缝转发。

### 2.3 Missing —— 74 条 / 4,095 行（按规模排序，前 12）

| 原文行号 | 例程 | 行数 | 阻塞原因 |
|---|---|---|---|
| 2087-2900 | `TMerchant.UserSelect` | 814 | NPC 商店/仓库/修理/升级武器全流程，依赖 `TPlayObject` 与 `FrmDB` |
| 3367-3689 | `TMerchant.ClientBuyItem` | 323 | 背包/交易/金币校验，依赖 `TPlayObject` 物品容器 |
| 1684-1902 | `TMerchant.UpgradeWapon` | 219 | 武器升级表 + `FrmDB` |
| 9263-9574 | `TNormNpc.GotoLable` | 312 | 脚本跳转核心，依赖 `m_ScriptList` 的完整装载（`LoadNpcScript` 未做） |
| 1512-1629 | `TMerchant.RefillGoods` | 118 | 嵌套过程 `RefillItems` + 定时刷新 |
| 9627-9744 | `TNormNpc.Run` | 118 | 心跳/动态名/自动变色，依赖 `TEnvirnoment` 与 `g_Config` |
| 1903-2051 | `TMerchant.GetBackupgWeapon` | 149 | 取回升级武器 |
| 1186-1334 | `TCastleOfficial.UserSelect` | 149 | 攻城官员：招募弓箭手/守卫/修门 |
| 3894-4029 | `TMerchant.ClientMakeDrugItem` | 136 | 制药 |
| 1107-1117 / 1118-1185 / 1335-1445 | `TCastleOfficial.*`（Click/GetVariableText/HireGuard/HireArcher） | 部分 | 依赖 Castle |
| 4164-4195 / 4196-4234 / 4241-4281 | `TMerchant.ClearScript/LoadUpgradeList/ClearData` | 小 | 与 `FrmDB` 存取耦合 |
| 5879-5932 | `TNormNpc.Create/Destroy` | 54 | 依赖基类字段 `m_nLight`/`m_btNameColor`/`m_nWalkSpeed`/`m_nInitWalkSpeed`/`m_wAppr`/`m_Castle`（托管侧 `Engine.TCreature` 尚无） |

其余 62 条见 `ObjNpcRoutineRegistry.cs`（`Status == "Missing"`）。

---

## 3. 新增文件 + 每个方法的已覆盖/未覆盖行号范围

### 3.1 源码（`GXX.CSharp/src/GXX.M2Server/Npc/`）

| 文件 | 行数 | 内容 | 覆盖原文行号 |
|---|---|---|---|
| `ObjNpcSeams.cs` | 314 | 常量 `CMD_RACE_0..12`（10-22）；接缝记录 `TQuestInfo`/`TScript`（M2Definition 215-230）、`TVarType`/`TVarAttr`/`TVarInfo`/`TDynamicVar`（M2Definition 60-77）；`NpcSeams` 宿主委托 | interface 段 10-22、M2Definition 摘录；**不含 ObjNpc 实现行** |
| `ObjNpcTypes.cs` | 473 | interface 段全部记录/类（25-258）+ `TConditionList.Create` | **25-258、504-508** |
| `ObjNpcClasses.cs` | 225 | 类**字段面**：`TNormNpc` 262-292、`TMerchant` 335-389、`TGuildOfficial` 432-437、`TTrainer` 449-453、`TBoxMonster` 461-468、`TCastleOfficial` 470-484 | **262-292、335-389、432-484** |
| `ObjNpcUnitFuncs.cs` | 778 | 单元级 5 函数 | **509-1106、10406-10509** |
| `ObjNpcVars.cs` | 520 | 变量族 10 方法 | **4443-4644、5690-5878、5981-6010、9877-9899** |
| `ObjNpcLabels.cs` | 273 | 标签/排序/错误上报 9 方法 + `ObjNpcText.CompareText` | **4383-4430、5934-5980、9745-9788、9955-10048** |
| `ObjNpcMerchant.cs` | 283 | 价格/货物族 9 方法 | **1446-1511、1630-1673、2052-2086、3160-3179、3272-3366、3793-3797** |
| `ObjNpcRoutineRegistry.cs` | 145 | 112 条例程登记（脚本抽取） | 全体（元数据） |

**未覆盖行号范围（本车道未落地）**：
`1107-1445`、`1512-1629`、`1674-2051`、`2087-3159`、`3180-3271`、`3367-3792`、`3798-4382`、
`4431-4442`、`4645-5689`、`5879-5933`、`6011-9262`、`9263-9574`、`9575-9744`、`9789-9876`、`9900-9954`。
（合计 8,897 行；其中 4,297 行有 Seam、4,600 行完全未触及。）

### 3.2 测试（`GXX.CSharp/tests/GXX.M2Server.Tests/`）

| 文件 | 用例数 | 覆盖对象 |
|---|---|---|
| `NpcObjNpcTests.cs` | 105 | `LoadLevelScript{Action,Condition}`、`GetLevelBaseObject{Condition,Action}`、`CheckStrIsVar`、`GetValNameValue`、`GetVarValue`×4、`SetVarValue`、`Get/SetDynamicValue`、`GetLineVariableText`、`GetDynamicVarList`、`TConditionList` |
| `NpcObjNpcLabelsTests.cs` | 30 | `AllowSelect`、`Add/DeleteSelectLable`、`ClearScript`、`QuickSortRecordList`、`DoSort`、`GetSayingRecordFromRecordList`、`ScriptActionError`、`ScriptConditionError` |
| `NpcObjNpcMerchantTests.cs` | 42 | `AddItemPrice`、`CheckItemPrice`、`GetRefillList`、`CheckItemType`、`GetItemPrice`、`GetUserPrice`、`ClearExpreUpgradeListData`、`GetUserItemPrice`、`GetSellItemPrice` |
| `NpcObjNpcRegistryTests.cs` | 18 | 登记表审计守卫（条数/行号连续性/状态取值域/Covered 计数） |
| **合计** | **195** | 每个公开方法 ≥3 用例（含 空/0/负/超界/异常）；**12 组差异断言**见下 |

**差异断言清单**（"看起来一样实则不同"的分支）：

| 用例 | 断言内容 |
|---|---|
| `LoadLevelScriptCondition_Diff1_TrailingDot_ThrowsWhileActionDoesNot` | 原文 605 无尾点保护 → Condition 抛 `ArgumentOutOfRangeException`，Action 正常返回 |
| `LoadLevelScriptCondition_Diff1_SingleDot_Throws` | `"."` → 空列表取 `Strings[-1]` |
| `LoadLevelScriptCondition_Diff2And3_BbFsBbrDegradeToVarName` | `BB/FS/BBR` 在 Condition 侧落 `CMD_RACE_5`（Action 侧 10/11/12） |
| `GetLevelBaseObject_Diff_Race5NullPlayer_BreaksInActionButNotInCondition` | 原文 779 无 nil 检查 vs 960-961 有 → 同一输入返回不同对象 |
| `GetLevelBaseObject_Race10_ReturnsFirstLivingSlave` | Condition 无 `CMD_RACE_10` 分支 → 保留 PlayObject |
| `GetLevelBaseObject_HeroExtOff_KeepsBaseObjectUntouched` | `g_nKey_HeroExt=0` 时 `CMD_RACE_7/8` 既不赋值也不 Break |
| `LoadLevelScriptAction_HeroExtOff_HmHlStayZeroWhileOtherPrefixesStillMap` | `HM/HL` 未赋值 → 零初始化 **0 == CMD_RACE_0(self)** |
| `GetDynamicValue_VNoneType_MatchesButYieldsFalseAndStopsSearch` | `vNone` 命中后 `Break` 仍执行，Result 为 False |
| `SetDynamicValue_VNoneType_StillReturnsTrue` | Set 版 `Result := True` 在 `case` 之后 → `vNone` 也 True |
| `GetItemPrice_ZeroPriceStdItem_IsDistinctFromSentinel` / `_NegativePriceInList_TriggersStdItemFallback` | `-1` 哨兵 vs 真 0；`Result < 0` 不看来源 |
| `GetUserPrice_CastleMasterGuild_UsesIntegerDivisionQuirk` | 原文 2073 括号内是整数除法 → `n14` 恒 60（不是 80%×rate） |
| `GetUserItemPrice_SellToNpcWithNoCalcFlag_SkipsAddPropertyBonus` | 3315 的 `or` 组合门 |
| `GetSellItemPrice_BankerRounding` | `Round` 银行家舍入：`2.5→2`、`1.5→2`、`0.5→0` |
| `GetValNameValue_EmptyVar_LeavesOutParamsUntouched` | 原文 5697 提前 Exit → 出参**保持调用方原值**（故托管用 `ref` 非 `out`） |
| `CheckStrIsVar_ShortString_ReturnsFalse` | 门槛是 `Length > 3`、且**不是** `>= 4` 的另一种表述 |

---

## 4. 测试用例数 + build/test 结果

```
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p4-m2-objnpc\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
  → Build succeeded. 0 Error(s)

dotnet test tests\GXX.M2Server.Tests\GXX.M2Server.Tests.csproj -c Debug --nologo
  → Passed! - Failed: 0, Passed: 5786, Skipped: 0, Total: 5786, Duration: 18 s
```

- 基线 **5591** → 现 **5786**（**+195**，全部为本车道新增；**0 新增失败**）。
- 期间未出现 `MSB4166`；`git status` 全程干净，未发现基线漂移。
- 本车道**未修改任何 `Npc/` 之外的文件**（`git show --stat` 可核）。

---

## 5. 发现的原文缺陷 / 易错点（带 `文件:行`）

> 全部**照抄保留**并在代码里加 `// 原文如此（ObjNpc.pas:<行>）` 或等价注释，未"顺手修正"。

### 5.1 崩溃 / 越界类

| # | 位置 | 问题 |
|---|---|---|
| D1 | `ObjNpc.pas:605`（对照 518） | `LoadLevelScriptCondition` **缺尾点保护**：`"A."` 时 `ExtractStrings` 得 `['A']` → `Delete(0)` 后取 `Strings[0]` → Delphi 抛 `EStringListError`。Action 侧 518 正是为此加了 `(sCmd[Length(sCmd)] <> '.')`。已用 `Assert.Throws` 锁死。 |
| D2 | `ObjNpc.pas:1082-1091` | `CMD_RACE_12` 内 `Player.m_SlaveList.Items[Random(Player.m_SlaveList.Count)]` —— `Count = 0` 时 `Random(0)=0` 仍取 `Items[0]` **越界**；外层只判了 `m_SlaveList <> nil`，没判 `Count > 0`。 |
| D3 | `ObjNpc.pas:9955-9994` | `QuickSortRecordList` **无 `R < L` 守卫**：`(0,-1)` 时 `P := (L+R) shr 1 = -1` → `List.Items[-1]`。正常路径被 `DoSort`(10005) 的 `Count > 0` 挡住。已用 `Assert.Throws<ArgumentOutOfRangeException>` 锁死。 |
| D4 | `ObjNpc.pas:9773-9787` | `ScriptConditionError` 拼出 `sMsg` 却**从不输出** —— 唯一含 `MainOutMessage` 的旧实现（9774-9786）被 `{ }` 注释掉了。**该过程在原文里是空操作**。 |

### 5.2 逻辑恒真/冗余类

| # | 位置 | 问题 |
|---|---|---|
| D5 | `ObjNpc.pas:3323` | `if (nC <> 4) or (nC <> 9)` —— 同一变量不可能同时等于又不等 → **恒真**。若改成 `and` 会改变行为（4/9 槽位会被跳过）。 |
| D6 | `ObjNpc.pas:3167-3168` | `for I := Count-1 downto 0` 里的 `if m_UpgradeWeaponList.Count <= 0 then Break` —— **永不为真**的冗余守卫。 |
| D7 | `ObjNpc.pas:10416-10419` | `$HUMAN(` 分支**没有 `Exit`**（`$GUILD(`/`$GLOBAL(`/`$STR(` 都有）。行为等价（Result 已置 True），但会多跑几次 `CompareLStr`。 |
| D8 | `ObjNpc.pas:4528 / 4531 / 4590 / 4593 / 5705` | `sData := ArrestStringEx(...)` 的赋值结果**之后再未被使用**（`sData` 是局部变量）。 |
| D9 | `ObjNpc.pas:2073` | `Max(60, Round(m_nPriceRate * (g_Config.nCastleMemberPriceRate / 100)))` —— 括号内是**整数除法**，`80/100 = 0` → `n14` 恒为 `60`（作者本意应是 80%）。 |
| D10 | `ObjNpc.pas:1663` | `if Result < 0` —— 不看 `Result` 来自价目表还是哨兵；价目表里存了负数会**再次**去查标准物品。 |

### 5.3 变量系统不一致类（最易误判）

| # | 位置 | 问题 |
|---|---|---|
| D11 | `ObjNpc.pas:4443-4455` vs `4467-4480` | 两个 `GetVarValue` 重载对 `nValue` 的默认值处理**不同**：4443 版第二参是**入参当前值**（`StrToInt64Def(sValue, nValue)`），4467 版是**字面量 0**（原文 4473 注释 `// chongchong 2016-08-31`）。 |
| D12 | `ObjNpc.pas:5700-5701` vs `5697-5698` | `GetValNameValue`：`sVar = ''` 时提前 `Exit`，**出参保持调用方原值**；否则先无条件 `sValue := sVar; nValue := 0;`。故托管签名必须用 `ref` 不能 `out`。 |
| D13 | `ObjNpc.pas:5829 vs 5837` | `L变量` 分支位于 `n01 >= 0` 的 `else if` 之后 —— 而 `GetValNameNo('L123')` 返回 `10123 >= 0`，落进 `if` 后**没有任何区间命中** → `Result = False` 且 `sValue` 已被改成 `sVar`。即 `L123`（纯数字 L 变量）**取不到值**。 |
| D14 | `ObjNpc.pas:5868-5874` | `N$` 分支：`GetIndex` 未命中时 `nValue := 0`；而 `S$` 分支（5842-5845）是 `sValue := ''`。两者不对称，且 `N$` 分支不看 `sValue`。 |
| D15 | `ObjNpc.pas:5941 / 5959 / 5973` | `CompareLStr(sLabel, 条目, **条目长度**)` —— 比较长度取黑名单条目长度，故"条目比 `sLabel` 长"时**不匹配**（`AllowSelect` 放行、`AddSelectLable` 会重复添加）。这不是笔误而是可观测行为，已单测锁死。 |

### 5.4 依赖的底层语义陷阱

| # | 位置 | 陷阱 |
|---|---|---|
| D16 | `HUtil32.pas:1807` `ArrestVariable` | 取出的子串**含 `$`**（`nLen` 从 `nPos+1` 起算，而 `nPos+1` 正是 `$` 所在位）。故 `GetLineVariableText` 传给 `GetVariableText` 的 `sVariable` 形如 `"$V"`，也正是后者内部直接比较 `'$HUMAN('` 的原因。已单测锁死（期望 `"$V"` 而非 `"V"`）。 |
| D17 | **托管侧基线缺陷** `GXX.Core/Util/HUtil32.cs:581` | `CompareLStr` 用 `string.CompareOrdinal` → **大小写敏感**、且**缺 `compn <= 0` 守卫**；原文 `HUtil32.pas:1981-1995` 是 `UpCase` 逐字符 + `compn <= 0` 直接 False。本车道的 `GetValNameValue`/`GetDynamicValue`/`AllowSelect` 等都依赖"大小写不敏感前缀比较"。**处置**：转调 Engine 里已有的 1:1 实现 `MonGenParseCore.CompareLStr`（`MonGenParseCore.cs:116`），**未复制第三份**。 |
| D18 | **托管侧基线缺陷** `Engine/CombatPower.cs:557` | `TPlayObject.m_ZVal` 是 `string[]`，元素默认 **`null`**；原文是 `string[100]`（ShortString，默认 `''`）。本车道取 `?? ""` 以贴合原文，已单测锁死；这属于 Engine 侧应修的数据初始化问题（不属本车道白名单）。 |
| D19 | 命名差异 `Engine/ObjBase.OnlineMsg.cs:50` | 原文 `m_boSuperMan`（大写 M）在托管侧写作 `m_boSuperman`。本车道未越区改名，也未在 `TNormNpc` 重复声明。 |

---

## 6. 接缝清单 + 需要调度方改白名单外文件的**精确签名要求**

### 6.1 必须由调度方处置的既有文件（本车道无写权限，未改动）

**(A) `GXX.CSharp/src/GXX.Core/Util/HUtil32.cs:581`** —— 与原文不符，影响面**跨车道**

现状：
```csharp
public static bool CompareLStr(string src, string targ, int compn)
{
    if (src.Length < compn || targ.Length < compn) return false;
    return string.CompareOrdinal(src, 0, targ, 0, compn) == 0;
}
```
要求改为（原文 `HUtil32.pas:1981-1995` 1:1）：
```csharp
public static bool CompareLStr(string src, string targ, int compn)
{
    if (compn <= 0 || src.Length < compn || targ.Length < compn) return false;
    for (int i = 0; i < compn; i++)
    {
        if (char.ToUpperInvariant(src[i]) != char.ToUpperInvariant(targ[i])) return false;
    }
    return true;
}
```
> 与 `GXX.M2Server.Engine.MonGenParseCore.CompareLStr`（MonGenParseCore.cs:116）**逐字相同** ——
> 修好后应把两处（含本车道 `ObjNpcUnitFuncs.cs`/`ObjNpcVars.cs`/`ObjNpcLabels.cs` 的转调）统一到 `HUtil32` 一份。
> **风险提示**：大小写敏感→不敏感是**行为放宽**，某些现有测试若断言"不匹配"可能翻转，需连同回归一起改。

**(B) 跨车道同名类型碰撞（第 5 次复发）**

| 命名空间 A | 命名空间 B | 后果 |
|---|---|---|
| `GXX.M2Server.Engine.TNormNpc`（`Engine/NpcScriptEngine.cs:10`，会话 A 的 ObjNpc 前置最小模型：仅 `m_sCharName` + 构造函数） | `GXX.M2Server.Npc.TNormNpc`（本车道，ObjNpc.pas:262 全量） | 任何同时 `using` 两命名空间的文件 **CS0104**（本车道测试文件已实际触发） |

**建议裁定（按 §12.8「以原文属性名为准、正式归属侧定名」）**：
ObjNpc.pas 的 `TNormNpc` 的**正式归属应是 ObjNpc 车道**，因此
1. 删除 `Engine/NpcScriptEngine.cs:10-15` 的最小 `TNormNpc`，把 `Engine` 内引用（`NpcScriptCommands.cs`/`NpcScriptState.cs`/`NpcScriptEngine.cs` 自身）
   改为 `using TNormNpc = GXX.M2Server.Npc.TNormNpc;` 或整体 `using GXX.M2Server.Npc;`；
2. 若调度方希望反向（把 ObjNpc 版并入 `Engine`），需要**本车道获得 `!` ALLOW-MODIFY 授权**把 `Npc/*.cs` 的 `namespace GXX.M2Server.Npc` 改成 `GXX.M2Server.Engine`，并删除 Engine 侧最小模型 —— 本车道**不会自行改名**（遵任务书第 6 条）。

**(C) 建议为后续批次开 `!` 授权的最小文件集**（本车道本轮**未申请**，仅登记需求）：
- `src/GXX.M2Server/Engine/ObjBase.cs`（补 `TCreature.m_CurrTarget`/`m_LastHiter`/`GetPoseCreate()`）
- `src/GXX.M2Server/Engine/RecalcBonus.cs` 或 `ObjBase.cs`（补 `TPlayObject.m_MyHero`）
- `src/GXX.M2Server/Engine/CombatPower.cs`（补 `TPlayObject.m_nVal`/`m_sString`/`m_TVal`/`m_ArrayList`，并修 `m_ZVal` 的 `null` → `""`）

> 补齐后 §2.2 的 4 条接缝与 §2.3 的 `Create`/`Initialize`/`Run` 即可解除阻塞。

### 6.2 本车道已落地的接缝（后续车道**请复用、勿另造**）

全部集中在 `src/GXX.M2Server/Npc/ObjNpcSeams.cs` 的 `NpcSeams` 静态类，默认实现 = "无宿主"，单测用 `ResetDefaults()` 隔离。

| 接缝 | 精确签名 | 原文出处 |
|---|---|---|
| `g_nKey_HeroExt` | `int { get; set; }` | M2Share 全局；ObjNpc.pas:558/565/645/652/803/820/985/1002 |
| `GetPlayObject` | `Func<string, TPlayObject?>` | `UserEngine.GetPlayObject`；779/959 |
| `GetMyHero` | `Func<TPlayObject, TCreature?>` | `TPlayObject.m_MyHero`；708/805/807/822/824/987/989/1004/1006 |
| `GetCurrTarget` | `Func<TCreature, TCreature?>` | `TBaseObject.m_CurrTarget`；736/807/916/989 |
| `GetLastHiter` | `Func<TCreature, TCreature?>` | `TBaseObject.m_LastHiter`；791/824/973/1006 |
| `GetPoseCreate` | `Func<TCreature, TCreature?>` | `TBaseObject.GetPoseCreate`；750/930 |
| `IsCopyMon` | `Func<TCreature, bool>` | `TempObject is TCopyMon`；1062 |
| `IsFunctionOrMissionNpc` | `Func<TNormNpc, bool>` | `g_FunctionNPC`/`g_MissionNPC`；5943 |
| `MainOutMessage` | `Action<string>` | M2Share；5945/9764 |
| `GetValNameNo` | `Func<string, int>` | 默认转调 `Engine.CombatPowerUtils.GetValNameNo`（已有 1:1 实现，**未复制**） |
| `GetVariableText` | `Func<TNormNpc,TPlayObject,string,string,int,(bool Result,string SMsg,bool IsBreakParseVar)>` | `TNormNpc.GetVariableText`(6011-9262) |
| `GetValNameValue`(存储面) | `GetPlayerPVal`/`GetPlayerSString`/`GetPlayerTVal`/`GetPlayerArrayListValue`/`GetGlobaDyMval`/`GetGlobalVal`/`GetGlobalAVal` | `m_nVal`/`m_sString`/`m_TVal`/`m_ArrayList`、`g_Config.GlobaDyMval`/`GlobalVal`/`GlobalAVal` |
| `SetValNameValue` | `Func<TNormNpc,TPlayObject,string,string,int,bool>` | `TNormNpc.SetValNameValue`(4935-5325) |
| `GetBoxItemValue` | `Func<string,TPlayObject,(bool Result,string Ret)>` | 单元级(5326-5689) |
| `SetBoxItemValue` | `Func<string,TPlayObject,string,int,bool>` | 单元级(4645-4934) |
| `GetPlayerDynamicVarList`/`GetPlayerGuildName`/`GetGuildDynamicVarList`/`GetGlobalDynamicVarList` | `Func<TPlayObject,List<TDynamicVar>>` / `Func<TPlayObject,string?>` / 同左 / `Func<List<TDynamicVar>>` | `GetDynamicVarList` 的三条分支（9882/9890/9895） |
| `Random` | `Func<int,int>` | Delphi `Random(n)`；1084（默认实现保证 `Random(0)=0`） |
| `GetStdItem` | `Func<int, TStdItem?>` | `UserEngine.GetStdItem`；1481/1665/3284 |
| `SaveGoodPriceRecord` | `Action<TMerchant, string>` | `FrmDB.SaveGoodPriceRecord`；1454 |
| `GetNpcCastle` | `Func<TNormNpc, object?>` | `m_Castle`；2071 |
| `IsMasterGuild` | `Func<object, TPlayObject, bool>` | `TUserCastle.IsMasterGuild`；2071 |

### 6.3 本车道**自定义**的接缝类型（等 M2Definition.pas 移植后应删除并改引用）

| 本车道类型 | 原文出处 | 说明 |
|---|---|---|
| `TVarType` / `TVarAttr` / `TVarInfo` / `TDynamicVar` | M2Definition.pas:60-77 | `GXX.M2Server.Plugins.PluginInterfaceSeams.cs:298` 另有一份**不同用途**的 `TDynamicVar` 接缝，命名空间不同、不冲突；`PluginInterfaceSeams.cs:274` 也有一份 `TUserMagic`/`TMagic` |
| `TQuestInfo` / `TScript` | M2Definition.pas:215-230 | `TDynamicVar` 用 `class` 而非 `struct`：`Get/SetDynamicValue` 按引用就地改写 |
| `ObjNpcText.CompareText` | SysUtils.CompareText | `internal static`，无同名冲突 |
| `ObjNpcConst.CMD_RACE_0..12` | ObjNpc.pas:10-22 | 单元级 const → 静态常量类（§3.3 命名规则） |

**托管侧签名偏差（必须在审计里认账，已写在文件头）**：

| 原文签名 | 托管签名 | 原因 |
|---|---|---|
| `function GetLevelBaseObjectCondition(...): TBaseObject` | `... : TCreature` | `TBaseObject`/`TAnimalObject` 尚未移植；复用既有 `Engine.TCreature` 作最薄代表，未复制第二份 |
| `procedure TNormNpc.ClearScript` | `public virtual void ClearScript()` | 原文即 `virtual`；Delphi `Dispose`/`Free` 无托管对应动作，**只保留结构性遍历**（遍历顺序 1:1） |
| `function GetUserItemPrice(UserItem: pTUserItem; ...)` | `GetUserItemPrice(ref TUserItem UserItem, ...)` | 原文 3302-3303 **就地改写** `UserItem.DuraMax`；`TUserItem` 是值类型结构，必须 `ref` 才保住语义 |
| `function GetDynamicVarList(...): TList` | `public List<TDynamicVar> GetDynamicVarList(...)` | `TList` → `List<T>`（§3.1） |
| `var sLabel: string` 等 `var` 出参 | C# `ref`（不是 `out`） | 原文多个方法在提前 `Exit` 时**不写出参**（D12/D11），`out` 会强制写入 |
| `TNormNpc.Operate(ProcessMsg: pTProcessMessage): Boolean` | 未声明 | `TCreature.Operate()`（无参）已被 Engine 占用同名；接入时需与 `Engine` 侧一并定名（见 §6.1-B） |

---

### 6.4 ⚠ 独占区与顺序会话既有文件的重叠（请调度方注意）

任务书给本车道的测试独占区写作 `tests/GXX.M2Server.Tests/Npc*.cs`。实测该 glob **已经命中会话 A 的两个既有文件**：

| 既有文件 | 行数 | 归属 |
|---|---|---|
| `tests/GXX.M2Server.Tests/NpcScriptTests.cs` | 258 | 会话 A（NpcCommon / NpcConditionCmd / NpcActionCmd / HandleNpcCmds 批次的测试） |
| `tests/GXX.M2Server.Tests/NpcScriptBatchITests.cs` | 242 | 会话 A |

本车道**未创建、未修改、未删除**这两个文件（`git diff --name-only 767ab38d..HEAD` 已核，只有 12 个新增文件，全部在本车道名下）。
后续若再派 `Npc*` 命名的车道，建议把独占区收窄为显式前缀（如 `NpcObjNpc*.cs`）以免误判越区。
## 7. 诚实说明：未完成部分与剩余量

### 7.1 量化

| 口径 | 行数 | 占比 |
|---|---|---|
| `ObjNpc.pas` 总行数 | 10,546 | 100% |
| **逐行 1:1 落地（34 条）** | **1,649** | **15.6%** |
| 只落最小接缝（4 条） | 4,297 | 40.7% |
| 完全未触及（74 条，含 70 条例程 + 4 条接缝的包裹） | 4,600 | 43.6% |
| 另：interface 段声明（10-495，已 1:1 落地为类型/字段面） | 486 | （计入上方骨架，不重复计数） |

**按调度方可直接采信的"例程口径"：Covered 34/112 = 30.4%。**

### 7.2 明确未做（不是"差不多做完"）

1. **`TNormNpc.GetVariableText`（3,252 行）完全未做** —— 这是 ObjNpc 的"变量文本引擎"，
   支撑 `<$$HUMAN(..)>`/`<$GUILD(..)>`/`<$STR(..)>`/`<$DATE>`… 数百种变量。**没有它，NPC 对话文本无法正确渲染。**
2. **`TNormNpc.GotoLable`(312) + `LoadNpcScript`(18) + `LoadNpcIconFile`(11) + `ClearScript` 的装载侧** ——
   `ClearScript` 已做（释放侧），但**装载侧完全没有**，故 `DoSort`/`GetSayingRecordFromRecordList` 目前只能靠人工构造 `TScript` 测试。
3. **`TMerchant.UserSelect`(814) 与其分发的全部商店行为**（买卖/修理/仓库存取/升级武器/请酒/酿酒/卧龙英雄/回收/副本）**未做**；
   本次只做了"价格计算"这一半。
4. **`TNormNpc.Click`/`UserSelect`/`SendMsgToUser`/`MessageBox`/`SendCustemMsg`/`GetShowName`/`Run`/`Initialize`** 未做 ——
   依赖 `TPlayObject` 的脚本标签字段（`m_nScriptGotoCount`/`m_sScriptGoBackLable`/`m_sScriptCurrLable`/`m_sRandomString`/`m_sInputData`/`m_sNpcSelectItemName`/`m_boSendMsgFlag`）与 `TIniFile`，托管侧目前**一个都没有**。
5. **`TNormNpc.Create`/`Destroy`** 未做 —— 依赖基类字段缺失（§6.1-C），不是"忘了"。
6. **`TGuildOfficial`(432-446)、`TTrainer`(449-459)、`TBoxMonster`(461-468)、`TCastleOfficial`(471-484)** 全部方法未做（只有字段面）。
7. **未做任何"接入"工作**：所有接缝默认实现都是"无宿主"。**没有一行接缝被真正接到 Engine 上** ——
   接入需要 §6.1 的文件授权。

### 7.3 本次交付的可信度边界

- **可信**：34 条 Covered 例程的**分支顺序、边界行为、原文笔误**均逐行对照原文；
  `ObjNpcRoutineRegistry.cs` 的 112 条行号区间由脚本从原文抽取（非手抄），并有 18 条守卫用例锁定。
- **不完全可信**：所有 Seam 转发路径（`GetVariableText`、`Get/SetValNameValue`、`Get/SetBoxItemValue`）
  的行为**取决于未来注入的实现**；本次只验证了"失败分支"与"转发参数正确"。
- **未验证**：任何真实宿主下的端到端 NPC 行为（无 `TEnvirnoment`/`TPlayObject` 完整面，无法跑起来）。
- **假设**：`ExtractStrings(['.'], [], ...)` 用 `TGroupItems.ExtractStrings(char,string)`（`.Split` + 跳空串）
  等效。该等效沿用仓库既有约定（`GroupItems.cs:287` 注释），**未独立验证 Delphi 4095 字符上限与 `#0` 截断分支**
  （原文调用点 `Pos('.') > 0` 已保证非空且 NPC 命令名远短于 4095）。

### 7.4 建议的下一批（按性价比排序）

| 批次 | 内容 | 前置 |
|---|---|---|
| N1 | `TNormNpc.Create/Destroy/Initialize/GetShowName/Operate` | 需 §6.1-C 的基类字段授权 |
| N2 | `GotoLable`(312) + `LoadNpcScript`(18) + `LoadNpcIconFile`(11) + `LoadAddData/SaveAddData`(53) | 需 `TIniFile`/`m_sScript` 宿主面 |
| N3 | `TNormNpc.Click/UserSelect/SendMsgToUser/MessageBox/SendCustemMsg`（约 110 行） | 需 `TPlayObject` 6 个脚本标签字段 |
| N4 | `TMerchant.UserSelect`(814) 分片：先做 `@buy`/`@sell`/`@repair` 三个分片 | 需 `FrmDB` 与背包宿主面 |
| N5 | `GetVariableText`(3,252) 分片：先做无宿主依赖的 `<$DATE>`/`<$TIME>`/`<$USERNAME>` 族 | 独立可做 |
| N6 | `GetBoxItemValue`/`SetBoxItemValue`(654) + `SetValNameValue`(391) | 需 `m_nVal`/`m_sString`/`m_TVal`/`m_ArrayList` |

---

*报告完。本车道全部产出在 `.worktrees/p4-m2-objnpc`，分支 `par/p4-m2-objnpc`，未触碰主工作树与任何兄弟工作树。*

---

# 8. 第二轮（切片 7 / 8，2026 波次续作）★ 本节数字**优先于** §2/§3/§7

> 前置：本分支已 `git rebase main`（基点由 `767ab38d` → **`5f34e9aa`**，main 上已有本车道切片 1-3 的集成提交 `7b851725`）。
> 后基线上门禁基线由 5,591 变为 **5,871**（main 的 29 个新提交带来的测试增量），本车道新增用例叠加于其上。

## 8.1 新增 commit

| # | commit | 内容 |
|---|---|---|
| 7 | `6c9e4d92` | 切片7：商人存取/装载族 + `TBoxMonster` + `GetVariableText` 虚分派链 1:1（16 例程上收，+61 用例） |
| 8 | `db3c3537` | 切片8：`UpgradeWapon` 嵌套过程 `sub_4A0218`（143 行）1:1 + 6 接缝（+16 用例） |

## 8.2 覆盖口径（★ 以本节为准）

| 口径 | 切片6 | **切片8** |
|---|---|---|
| Covered 例程数 / 112 | 34 | **50** |
| Seam 例程数 / 112 | 4 | **6** |
| Missing 例程数 / 112 | 74 | **56** |
| Covered 例程原文行数 | 1,649 | **1,960** |
| Seam 例程原文行数 | 4,297 | **4,543** |
| Missing 例程原文行数 | 4,600 | **3,538** |
| **逐行 1:1 落地（含嵌套过程 sub_4A0218 的 143 行）** | 1,649 | **2,103 / 10,546 = 19.9%** |

`GXX.M2Server.Tests`：**5,948 passed / 0 failed**（切片6 时 5,786；main 基线漂移 +85，本车道第二轮新增 +77）。

## 8.3 切片 7 / 8 逐例程判定

**切片7（16 条上收 Missing → Covered）**

| 原文行号 | 例程 | 归属 |
|---|---|---|
| 1674-1682 | `TMerchant.SaveUpgradingList` | `ObjNpcMerchant.cs` |
| 3052-3060 | `TMerchant.LoadNPCData` | `ObjNpcMerchant.cs` |
| 3062-3069 | `TMerchant.SaveNPCData` | `ObjNpcMerchant.cs` |
| 3180-3204 | `TMerchant.LoadNpcScript` | `ObjNpcPersistence.cs` |
| 3206-3226 | `TMerchant.LoadNpcIconFile` | `ObjNpcPersistence.cs` |
| 3234-3270 | `TMerchant.GetVariableText`（覆写） | `ObjNpcMerchant.cs` |
| 3869-3892 | `TMerchant.AddItemToGoodsList` | `ObjNpcMerchant.cs` |
| 4164-4194 | `TMerchant.ClearScript`（覆写） | `ObjNpcMerchant.cs` |
| 4196-4211 | `TMerchant.LoadUpgradeList` | `ObjNpcMerchant.cs` |
| 4235-4238 | `TMerchant.SendCustemMsg`（覆写） | `ObjNpcMerchant.cs` |
| 4241-4280 | `TMerchant.ClearData` | `ObjNpcMerchant.cs` |
| 9575-9591 | `TNormNpc.LoadNpcScript` | `ObjNpcPersistence.cs` |
| 9593-9602 | `TNormNpc.LoadNpcIconFile` | `ObjNpcPersistence.cs` |
| 10510-10514 | `TBoxMonster.Create` | `ObjNpcBoxMonster.cs` |
| 10527-10532 | `TBoxMonster.Operate` | `ObjNpcBoxMonster.cs` |
| 10534-10543 | `TBoxMonster.Run` | `ObjNpcBoxMonster.cs` |

**切片8（1 条 Missing → Seam：嵌套过程已 1:1，外层体阻塞）**

| 原文行号 | 例程 | 状态 |
|---|---|---|
| 1684-1902 | `TMerchant.UpgradeWapon` | **Seam** —— 嵌套过程 `sub_4A0218`(1686-1828，143 行)**已 1:1**；外层体(1830-1901，72 行)**阻塞未做** |

**新增 Seam（1 条）**：`TNormNpc.SendCustemMsg`(9837-9862) —— 由 `TMerchant.SendCustemMsg` 的 `inherited` 需要，
已落为 `public virtual` 外壳 + `NpcSeams.SendCustemMsg` 转发。

## 8.4 关键结构修正：`GetVariableText` / `SendCustemMsg` 由"纯委托"改为**真虚方法**

切片 2 把 `TNormNpc.GetVariableText`(6011-9262) 做成纯委托 —— 这是**错的**：原文的
`TMerchant.GetVariableText`(3234)、`TCastleOfficial.GetVariableText`(1118)、`TGuildOfficial.GetVariableText`(10055)
三个覆写都用 `inherited GetVariableText(...)`，且 `GetLineVariableText`(6000) 对它是**虚调用**。
切片 7 已改正为：

```
public virtual bool GetVariableText(TPlayObject, ref string sMsg, string sVariable, ref bool IsBreakParseVar, int nPos)  // 外壳→接缝
public virtual void SendCustemMsg(TPlayObject, string)                                                                    // 外壳→接缝
GetLineVariableText(...) → GetVariableText(...)   // 虚调用，不再是接缝直调
```

并加了 **虚分派回归用例**：`GetLineVariableText_DispatchesVirtuallyToMerchantOverride`（`"P=<$PRICERATE>"` → `"P=100"`）。
—— 这是"接缝做过头会**破坏虚分派链**"的一个真实例子，建议记入台账。

## 8.5 新增原文缺陷（续 D 系列）

| # | 位置 | 问题 |
|---|---|---|
| D20 | `ObjNpc.pas:3196-3200` / `3220-3224` | `TMerchant.LoadNpcScript`/`LoadNpcIconFile` 的 `IsAddMapName = False` 分支里，`if m_boFB then SC := m_sScript else SC := m_sScript;` —— **两个分支体完全相同**（原文冗余，照抄）。 |
| D21 | `ObjNpc.pas:1820` | `btDura := Round(Min(5, nItemCount) + Min(5, nItemCount) * ((nDura / nItemCount) / 5.0))` —— `nItemCount = 0`（一条材料都没剔到）时 `nDura / nItemCount` 是**实数除法** → Delphi 抛 `EZeroDivide`。实测托管侧 0/0 得 NaN、`(int)Math.Round(NaN)` 截断为 **0 且不抛** —— **两侧行为不同**（一边崩一边给 0），已单测锁死托管侧行为。 |
| D22 | `ObjNpc.pas:1803-1812` | `DuraList` 排序是**逐轮冒泡**（最坏 O(n²)），且外层 `for I := 0 to Count - 1` 内的 `if DuraList.Count <= 0 then Break` 永不为真（冗余守卫，照抄）。 |
| D23 | `ObjNpc.pas:1787` | `if (UserItem.btValue[13] = 1) and (UserItem.Name <> '')` —— 用 `btValue[13]` 这个**魔法下标**决定日志里用自定义名还是标准名，无具名常量。 |
| D24 | `M2Share.pas:11664-11665` | `IsUseItem` **不判 `StdItem = nil`** 就读 `StdItem.StdMode` —— 原文空指针 AV；已按原文照抄（托管侧读 `Nullable.Value` 抛 `InvalidOperationException`），并加用例 `Sub4A0218_IsUseItem_NoStdItemThrowsLikeOriginalNullDeref` 锁死。 |
| D25 | `ObjNpc.pas:3167-3168`（切片3 已记 D6） | 同型冗余守卫在 `sub_4A0218` 的 1805-1806 再次出现 —— 说明这是该作者的习惯写法，**不是孤例**。 |
| D26 | `ObjNpc.pas:3240-3268` | `TMerchant.GetVariableText` 在 `Result := inherited ...` 为假时把 `Result := True`，三个变量分支各 `Exit`，末尾再 `Result := False` —— 用"先置真再置假"表达"只有命中才成功"，功能正确但极易误读（照抄）。 |

## 8.6 ★ 四个优先方法（`UserSelect` / `ClientBuyItem` / `UpgradeWapon` 外层 / `GotoLable`）的**精确阻塞清单**

> 按任务要求"就地停下并列出哪个方法需要哪个成员"。用脚本从原文对应行区间抽取 `Player.`/`User.`/`PlayObject.` 成员访问，
> 再逐个核对托管侧是否存在。**结论：四个方法全部阻塞**，且阻塞面**不是**已上报的那 6 个 Engine 成员，
> 而是**整片未移植的 `ObjPlayer.pas` `TPlayObject` 面**（每人 7~14 个成员）。
> 表中 ✅ = 托管侧已有；❌ = 缺失。

### (1) `TMerchant.UserSelect`（2087-2900，814 行）

| 需要成员 | 托管侧 | 说明 |
|---|---|---|
| `m_sCharName` | ✅ | `Engine.TCreature.m_sCharName` |
| `m_nInteger`（N 变量） | ✅ | `Engine.TPlayObject.m_nInteger` |
| `m_sString`（S 变量） | ❌ | 属已上报的 6 项之一（`CombatPower.cs` 需补） |
| `m_nGameGold`（元宝） | ❌ | ObjPlayer.pas |
| `m_nBigStoragePage` / `m_nDealGoldPose` | ❌ | ObjPlayer.pas |
| `m_boWaitHeroDate` | ❌ | ObjPlayer.pas |
| `m_sHeroName` / `m_sDeputyHeroName` / `m_sTempHeroName` | ❌ | ObjPlayer.pas（卧龙英雄/副将） |
| `m_sAutoSendMsg` | ❌ | ObjPlayer.pas |
| `GameGoldChanged()` | ❌ | ObjPlayer.pas |
| `SendMsg(...)` | ⚠ | `TCreature.SendMsg` 存在但**签名不同**（无 sender 参数，且是入队而非下发），需 `NpcSeams.SendMsgToClient` 那类接缝 |
| `SysMsg(...)` | ❌ | ObjPlayer.pas |
| `GetPoseCreate` | ❌ | 基类成员（已上报 6 项之一） |

### (2) `TMerchant.ClientBuyItem`（3367-3689，323 行）

| 需要成员 | 托管侧 | 说明 |
|---|---|---|
| `m_sCharName` | ✅ | — |
| `m_nGold` | ❌ | ObjPlayer.pas |
| `AddItemToBag(...)` | ❌ | ObjPlayer.pas |
| `IsEnoughBag()` | ❌ | ObjPlayer.pas |
| `IsAddWeightAvailable(...)` | ❌ | ObjPlayer.pas |
| `SendAddItem(...)` | ❌ | ObjPlayer.pas |
| `SendMsg(...)` | ⚠ | 同 (1) |

> 另：同族的 `ClientSellItem`(3798-3868) 只要 `IncGold` / `m_nGold` / `SendMsg` —— **是这四个里阻塞最浅的**，
> 若下轮补齐 `m_nGold` + `IncGold`，它可立即上收。

### (3) `TMerchant.UpgradeWapon` 外层体（1830-1901，72 行）

| 需要成员 | 托管侧 | 说明 |
|---|---|---|
| `m_sCharName` | ✅ | — |
| `m_ItemList` | ✅ | `Engine.TPlayObject.m_ItemList`（`List<TUserItem>`）→ `sub_4A0218` 因此可独立落地 |
| `m_nGold` | ❌ | ObjPlayer.pas |
| `m_UseItems[U_WEAPON]`（**读写**） | ❌ | ObjPlayer.pas（接缝只能读，1866 行还要写回 `wIndex := 0`） |
| `CheckItems(name)` | ❌ | ObjPlayer.pas |
| `DecGold(n)` / `GoldChanged()` | ❌ | ObjPlayer.pas |
| `SendDelItem(@item)` | ❌ | ObjPlayer.pas |
| `RecalcAbilitys()` / `FeatureChanged()` | ❌ | ObjBase/ObjPlayer |
| `SendMsg(...)` / `SysMsg(...)` | ⚠ / ❌ | 同上 |
| `GotoLable(...)` | ❌ | **ObjNpc.pas 自身未移植的 9263-9574**（见 (4)） |
| `g_ItemRules.Get(idx, 17)` / `g_CastleManager.IncRateGold` / `g_boGameLogGold` / `g_sCannotUpgradeWeapon` | ❌ | 均为 M2Share/ItemRules 侧 |

### (4) `TNormNpc.GotoLable`（9263-9574，312 行）

| 需要成员 | 托管侧 | 说明 |
|---|---|---|
| `m_sCharName` | ✅ | — |
| `m_nVal`（P 变量，**含 `FillChar` 整块清零**） | ❌ | **属已上报的 6 项之一** → 本方法**硬阻塞** |
| `m_NPC` / `m_Script` | ❌ | ObjPlayer.pas（NPC 会话绑定） |
| `m_ItemBoxNpc` | ❌ | ObjPlayer.pas |
| `m_boOffLine` / `m_boDummyObject` | ❌ | ObjPlayer.pas |
| `m_dwLastGotoLabelTick` / `m_nOneLabelGotoCount` / `m_sLastGotoLabel` / `m_sScriptParams` | ❌ | ObjPlayer.pas（跳转防环/参数） |
| `GetQuestFlagStatus(flag)` | ❌ | ObjPlayer.pas（任务标志） |
| `GetScriptLabel(sMsg)` | ❌ | ObjPlayer.pas |
| `SendFirstMsg(...)` / `SendMsg(...)` | ❌ / ⚠ | ObjPlayer.pas |

**给调度方的建议**：`ObjPlayer.pas` 的 `TPlayObject` 面是这四条（以及 `TMerchant.UserSelect` 分片）
的共同硬前置。若把它排成一条**专门的 `TPlayObject` 面拼接批次**（按"变量容器 → 金额/物品容器 → NPC 会话/脚本标签 → 英雄/副将"分四片上收），
ObjNpc 的剩余 56 条里有 **20 条以上**会同时解锁 —— 比逐条开接缝划算得多。
本车道**不自行声明任何 `TPlayObject` 替身成员**。

## 8.7 本车道第二轮新增接缝（14 个，全部集中在 `ObjNpcSeams.cs`，后续车道请复用）

| 接缝 | 精确签名 | 原文出处 |
|---|---|---|
| `LoadGoodRecord` | `Action<TMerchant, string>` | FrmDB；3057 |
| `SaveGoodRecord` | `Action<TMerchant, string>` | FrmDB；3067 |
| `LoadGoodPriceRecord` | `Action<TMerchant, string>` | FrmDB；3058 |
| `LoadUpgradeWeaponRecord` | `Action<string, List<object>>` | FrmDB；4207 |
| `SaveUpgradeWeaponRecord` | `Action<string, List<object>>` | FrmDB；1678 |
| `LoadNpcScriptFile` | `Action<TNormNpc, string, string>` | `FrmDB.LoadNpcScript`；9583/9589 |
| `LoadScriptFile` | `Action<TMerchant, string, string>` | `FrmDB.LoadScriptFile`；3201 |
| `LoadIconFile` | `Action<TNormNpc, string, string>` | `FrmDB.LoadIconFile`；**吞掉 `@m_ActorIcons`**（该基类字段托管侧尚无） |
| `sMarket_Def` / `sNpc_def` / `sNpcIcons` | `string { get; set; }` | M2Share.pas:378/379/381，默认值即原文 |
| `GetStdItemName` | `Func<int, string>` | `UserEngine.GetStdItemName`；1712/3259 |
| `GetUseItemsWeapon` | `Func<TPlayObject, TUserItem>` | `m_UseItems[U_WEAPON]`；3257/3259 |
| `GetItemAddValue` | `delegate void (ref TUserItem, ref TStdItem)` | `ItemUnit.GetItemAddValue`；1733 |
| `AddGameDataLog` | `Action<byte,byte,TCreature,string,int,string,int,int,string>` | M2Share.pas:11686；1719/1794 |
| `sBlackStone` | `string`（默认 `"黑铁矿"`） | M2Share.pas:918/4142 |
| `IsUseItem` | `Func<int,bool>`（**默认实现即原文 1:1 逻辑**） | M2Share.pas:11660-11669 |
| `SendMsgToClient` | `Action<TCreature,TCreature,ushort,long,long,long,long,string>` | `TBaseObject.SendMsg`（网络下发版）；1825 |
| `SendCustemMsg` | `Action<TNormNpc,TPlayObject,string>` | 9837（虚外壳转发） |

新增常量：`ObjNpcConst.LOG_ActionNone = 0`、`ObjNpcConst.LOG_ItemDisappear = 9`（M2Share.pas:87/96，同一工程内 `LogDataServer` 有同值副本但不跨工程引用）。

## 8.8 `TBoxMonster` 的两个**未覆盖**子项（阻塞登记）

| 原文行号 | 例程 | 阻塞原因 |
|---|---|---|
| 10516-10519 | `TBoxMonster.Destroy` | 原文函数体**只有 `inherited;`**；托管侧 `TCreature` 无 `Destroy` 覆写语义 → 登记未覆盖，不伪造 |
| 10521-10525 | `TBoxMonster.Initialize` | 需要 `TCreature.Initialize`（原文 `inherited`）—— 托管侧 **`Engine.TCreature` 没有 `Initialize` 这个虚方法**。这**不在**已上报的 6 项里，属同类缺口，请一并纳入下一轮 Engine 成员补齐。`TNormNpc.Initialize`(9864) 与 `TCastleOfficial`/`TGuildOfficial` 的 `Initialize` 同此缺口。 |

## 8.9 第二轮测试增量

| 文件 | 用例数 | 覆盖 |
|---|---|---|
| `NpcObjNpcMerchant2Tests.cs` | 45 | 商人价格外的方法族：`GetVariableText`(覆写+虚分派)、`AddItemToGoodsList`、`ClearScript`、`ClearData`、`SendCustemMsg`、`LoadNPCData`/`SaveNPCData`、`LoadUpgradeList`、`SaveUpgradingList`、`LoadNpcScript`/`LoadNpcIconFile`(×2 类)、`TBoxMonster` |
| `NpcObjNpcSub4A0218Tests.cs` | 16 | `sub_4A0218`：黑铁矿剔除/耐久前 5 求和/属性最大与次大/StdMode 三档/`btValue[13]` 自定义名/`NeedIdentify` 日志/空表差异断言/`IsUseItem` 原文空指针 |

`Npc*` 子集：195（切片6）→ **272**；全工程 5,786 → **5,948**。
---

# 8.10 第三轮（切片 10）★ 本节数字**优先于** §8.2

| # | commit | 内容 |
|---|---|---|
| 10 | `001026d7` | 切片10：公会/攻城官员 7 例程 1:1 + `Click` 虚分派链（+20 用例） |

> 前置：本分支已再次 `rebase main`（基点 `97870cd0`；main 已含本车道切片 1-3 **与切片 7** 的两次集成：`7b851725` / `c242bf2c`）。

## 覆盖口径（★ 以本节为准）

| 口径 | 切片6 | 切片8 | **切片10** |
|---|---|---|---|
| Covered 例程 / 112 | 34 | 50 | **57** |
| Seam 例程 / 112 | 4 | 6 | **7** |
| Missing 例程 / 112 | 74 | 56 | **48** |
| Covered 例程原文行数 | 1,649 | 1,960 | **2,044** |
| Seam 例程原文行数 | 4,297 | 4,543 | **4,555** |
| Missing 例程原文行数 | 4,600 | 3,538 | **3,442** |
| **逐行 1:1（含嵌套过程 `sub_4A0218` 143 行）** | 1,649 | 2,103 | **2,187 / 10,546 = 20.7%** |

门禁：`GXX.M2Server.Tests` **6,043 passed / 0 failed**；`Npc*` 子集 **302** 例。

## 切片10 上收的 7 条例程（Missing → Covered）

| 原文行号 | 例程 | 归属 |
|---|---|---|
| 1107-1116 | `TCastleOfficial.Click` | `ObjNpcGuildCastle.cs` |
| 3228-3232 | `TMerchant.Click`（覆写） | `ObjNpcGuildCastle.cs` |
| 10049-10053 | `TGuildOfficial.Click`（覆写） | `ObjNpcGuildCastle.cs` |
| 10055-10089 | `TGuildOfficial.GetVariableText`（覆写，`$REQUESTCASTLELIST`） | `ObjNpcGuildCastle.cs` |
| 10364-10367 | `TCastleOfficial.Create` | `ObjNpcGuildCastle.cs` |
| 10386-10389 | `TGuildOfficial.SendCustemMsg`（覆写） | `ObjNpcGuildCastle.cs` |
| 10391-10404 | `TCastleOfficial.SendCustemMsg`（覆写） | `ObjNpcGuildCastle.cs` |

**新增 Seam（1 条）**：`TNormNpc.Click`(4431-4442) —— **第二个"虚分派链修正"**（同 §8.4 的 `GetVariableText`/`SendCustemMsg`）。
原文三个覆写（`TMerchant`/`TGuildOfficial`/`TCastleOfficial`）都用 `inherited`，故必须落为
`public virtual void Click(TPlayObject)` 外壳 + `NpcSeams.Click` 转发，否则三个覆写无从落地。

## 新增原文缺陷（续 D 系列）

| # | 位置 | 问题 |
|---|---|---|
| D27 | `ObjNpc.pas:1114-1115` | `TCastleOfficial.Click`：**非城主行会成员且权限 &lt; 3 时既不提示也不 `inherited`** —— 玩家点 NPC **静默无反应**（无任何反馈）。这是可观测行为差异，已单测锁死（`CastleOfficialClick_NonMemberLowPermission_SilentlyDoesNothing`）。 |
| D28 | `ObjNpc.pas:10391-10404` | `TCastleOfficial.SendCustemMsg` **不调用 `inherited`**，自己重写全部门槛 —— 与 `TNormNpc.SendCustemMsg`(9837)、`TMerchant.SendCustemMsg`(4235)、`TGuildOfficial.SendCustemMsg`(10386) 三个"只 inherited"的覆写**语义不同**。照抄，并加用例断言"不调基类接缝"。 |
| D29 | `ObjNpc.pas:10075` | `if ((II div 2) * 2 = II) then sStr := '\'` —— 用整除再乘回的方式判"偶数"，可读性极差；效果是攻城列表**每两项一行**。已用**字面量期望串**锁死输出格式（含 `'\'` 单反斜杠与末尾 `'\ \'`）。 |
| D30 | `ObjNpc.pas:10364-10367` vs `:10374-10379` | `TCastleOfficial.Create` **只 `inherited`**（不置种族值），而 `TGuildOfficial.Create` 置 `m_btRaceImg := RC_MERCHANT; m_wAppr := 8;` —— 同族两个 `Create` 行为不一致（照抄；已加用例断言 `TCastleOfficial` 的 `m_btRaceServer` 保持 0）。 |
| D31 | `ObjNpc.pas:1109` | `TCastleOfficial.Click` 用 `m_Castle = nil` 判"不属于城堡"，但 `TCastleOfficial` 自身**没有重写 `Initialize`** —— `m_Castle` 的赋值依赖基类 `TNormNpc.Initialize`(9867) 的 `g_CastleManager.InCastleWarArea(Self)`，而该 `Initialize` 目前**阻塞未移植**（需 `TCreature.Initialize`）。即：本方法的空城堡分支在当前托管状态下**恒真**，已登记。 |

## 新增 Engine / 宿主缺口（与 §8.6 合并看）

| 缺口 | 阻塞的例程 |
|---|---|
| `TBaseObject.m_wAppr` | `TGuildOfficial.Create`(10374-10379)、`TNormNpc.Initialize`(9864-9875) |
| `TCreature.Initialize`（虚方法） | `TNormNpc.Initialize`、`TBoxMonster.Initialize`、`TCastleOfficial`/`TGuildOfficial` 的 `Initialize` |
| `TCreature.TurnTo(Integer)` / `SendRefMsg(...)` | `TGuildOfficial.Run`(10091-10099) |
| `TCreature.m_ActorIcons` | `FrmDB.LoadIconFile` 那一路（接缝已吞掉该参数） |
| `TPlayObject.m_btPermission` | **已用 `NpcSeams.GetPlayerPermission` 接缝绕过**（`TCastleOfficial.Click` 因此得以上收） |
| `TPlayObject.LableIsCanJmp` / `m_sScriptGoBackLable` / 6 个 `sNF_*` 常量 | `TGuildOfficial.UserSelect`(10101-10151) |
| `g_CastleManager.GetCastleNameList` | **已用 `NpcSeams.GetCastleNameList` 接缝绕过**（`TCastleOfficial`/`TGuildOfficial` 的 `$REQUESTCASTLELIST` 因此得以上收）。注：`Engine.TCastleManager` 类**已存在**（`Castle.cs:406`），只差这一个方法 —— **这是最便宜的一个补齐点**。 |

**诚实登记**：`NpcSeams.g_sSubkMasterMsgCanNotUseNowMsg` 的默认值 `"当前无法使用城主喊话功能"` 是**语义占位、不是原文文案**
（原文该字符串由 M2Share 的 `LoadString` 从资源载入，源码里只有键名）。接入时必须以原文资源串为准。

## 剩余 48 条未覆盖的最大块（排序）

| 原文行号 | 例程 | 行数 | 主要阻塞 |
|---|---|---|---|
| 6011-9262 | `TNormNpc.GetVariableText` | 3,252 | Seam（宿主面广） |
| 2087-2900 | `TMerchant.UserSelect` | 814 | ObjPlayer 面（§8.6(1)） |
| 4935-5325 | `TNormNpc.SetValNameValue` | 391 | Seam（4 类变量容器缺失） |
| 5326-5689 | `GetBoxItemValue` | 364 | Seam |
| 3367-3689 | `TMerchant.ClientBuyItem` | 323 | ObjPlayer 面（§8.6(2)） |
| 9263-9574 | `TNormNpc.GotoLable` | 312 | **`m_nVal`（6 项之一）→ 硬阻塞** |
| 4645-4934 | `SetBoxItemValue` | 290 | Seam |
| 1684-1902 | `TMerchant.UpgradeWapon` 外层体 | 76 | §8.6(3) |
| 1186-1334 | `TCastleOfficial.UserSelect` | 149 | Castle + GotoLable |
| 1335-1445 | `TCastleOfficial.HireGuard`/`HireArcher` | 111 | Castle |
---

# 9. 复核轮（切片 12）：已覆盖 57 条的逐条回读复核

> 按调度方裁定：**暂停新增接缝**（guild/castle 剩余例程接缝密度约 1 接缝/7 行，不划算；
> `TPlayObject`/`TCreature` 面已另开车道 `p6-m2-playersurface` 补齐），转为"已覆盖例程的语义等价复核"。
> 复核方法：**机械化交叉核对**（分支顺序 / Break 归属 / 数值区间序列 / 常量值）+ **逐族人工回读原文**。

## 9.1 复核结果表

| 方法族 | 例程（原文行号） | 结论 | 依据 |
|---|---|---|---|
| 脚本目标级解析 | `LoadLevelScriptAction`(509) `LoadLevelScriptCondition`(596) `GetLevelBaseObjectCondition`(682) `GetLevelBaseObjectAction`(857) | **等价** | ① case 标签序列：Condition 10/10、Action 13/13 **逐项一致**（首轮脚本误报 7/8 缺失，根因是原文 801/818/983/1000 的标签**带行尾注释** `// Hero.mon`，正则漏配 —— 已修正正则复跑）；② `Break` 归属：原文 Action 段共 25 个 `Break`，其中 3 个（1043/1065/1088）缩进 18 空格、属**内层 `for II` / `while`**，剩余 22 个属外层 for —— 与 C# 的 22 处 `boBreak = true` **完全一致**；Condition 段 18/18 一致；③ 23 条分支逐条人工回读（含 `m_MyGamePet` / `m_SlaveList` / `GetPoseCreate` 的逐支 nil 判定） |
| 变量系统 | `GetValNameValue`(5690) `GetVarValue`×4(4443-4493) `SetVarValue`(4495) `GetDynamicValue`(4512) `SetDynamicValue`(4576) `GetDynamicVarList`(9877) `GetLineVariableText`(5981) | **等价（1 处已修）** | ① 12 段数值区间序列**逐项一致**；② `sData[Length(sData)-1]`（1-based 倒数第二）→ 托管 `sData[Length-2]` 换算核对；③ `ref` vs `out`：原文空串提前 `Exit` **不写出参**，已用 `GetValNameValue_EmptyVar_LeavesOutParamsUntouched` 锁死（若误用 `out` 此例必红）；④ **已修**：`GetDynamicValue`/`SetDynamicValue` 的变量名比较原用 `StringComparison.OrdinalIgnoreCase`，原文 4554/4616 是 `SysUtils.CompareText` → 改为 `ObjNpcText.CompareText(...) == 0`（`CompareText` 是本车道落地的原文 1:1 版）。保留 `OrdinalIgnoreCase` 的一处是原文 5709 `SameText`（Delphi 为 locale 敏感的 `AnsiCompareText`，仓库无对应垫片，托管取 `OrdinalIgnoreCase` 为最接近等效） |
| 标签 / 排序 | `AllowSelect`(5934) `AddSelectLable`(5953) `DeleteSelectLable`(5967) `QuickSortRecordList`(9955) `DoSort`(9996) `GetSayingRecordFromRecordList`(10019) `ClearScript`(4383) `ScriptActionError`(9745) `ScriptConditionError`(9767) | **等价** | ① 200 元素随机标签排序结果与 `List.Sort(OrdinalIgnoreCase)` **全等**（覆盖 Hoare 划分 + 枢纽元素跟随交换 + 尾递归消除）；② 二分命中项 `Assert.Same`（引用相等）；③ `CompareLStr(sLabel, 条目, **条目长度**)` 的"长度取条目侧"怪癖已单测；④ 错误上报格式串**逐字**对照（含 `%s`/`%d` 与 16 个占位符） |
| 商人价格 | `AddItemPrice`(1446) `CheckItemPrice`(1457) `GetRefillList`(1488) `CheckItemType`(1630) `GetItemPrice`(1645) `GetUserPrice`(2052) `GetUserItemPrice`(3272) `GetSellItemPrice`(3793) `ClearExpreUpgradeListData`(3160) | **等价** | 42 用例：`GetUserItemPrice` 的 8 步公式链（肉/43/属性加成/叠加/耐久折算）逐步锁定；银行家舍入 6 个边界（±0.5/±1.5/±2.5）；`GetUserPrice` 成员价的**整数除法缺陷**已用 3 个不同 `m_nPriceRate`（100/1/999）证明结果恒 60 |
| 商人存取 / 装载 | `LoadNPCData`(3052) `SaveNPCData`(3062) `LoadNpcScript`(3180) `LoadNpcIconFile`(3206) `LoadUpgradeList`(4196) `SaveUpgradingList`(1674) `ClearData`(4241) `AddItemToGoodsList`(3869) `SendCustemMsg`(4235) | **等价** | seam 调用的**顺序与文件名**逐条断言（`m_sScript + '-' + m_sMapName`）；`IsAddMapName × m_boFB` **四组合**全覆盖（含原文"两分支体相同"的冗余）；异常被吞 + 输出两条信息；`ClearData` 的 nil 组 `Continue` |
| 升级材料聚合 | 嵌套过程 `sub_4A0218`(1686-1828) | **等价** | 16 用例：倒序拼接 `DelItems`（`黑铁矿/3/2/1` 顺序）、只取前 5 耐久、最大/次大滚动、StdMode 三档、`btValue[13]` 自定义名、`NeedIdentify` 日志、空表差异、byte 截断（400→144） |
| 公会 / 攻城 | `TGuildOfficial.Click`(10049) `TGuildOfficial.GetVariableText`(10055) `TGuildOfficial.SendCustemMsg`(10386) `TCastleOfficial.Click`(1107) `TCastleOfficial.Create`(10364) `TCastleOfficial.SendCustemMsg`(10391) `TMerchant.Click`(3228) | **等价** | `$REQUESTCASTLELIST` 用**字面量期望串**锁死（含 `\` 每两项分行、末尾 `'\ \'`、`%s`/`%d` 位置）；`TCastleOfficial.Click` 的"非成员静默无反应"三分支；`SendCustemMsg` 不调基类 |
| 箱子怪 / 类型 | `TBoxMonster.Create/Operate/Run`(10510/10527/10534)；全部记录与类字段；常量 | **等价** | 常量值**逐条**对照原文：`CMD_RACE_0..12`(10-22) / `LOG_ActionNone=0`(M2Share:87) / `LOG_ItemDisappear=9`(:96) / `sMarket_Def`·`sNpc_def`·`sNpcIcons`(:378/379/381) / `sBlackStone='黑铁矿'`(:4142) / `RC_BOX=30` |

## 9.2 复核发现的两个 **GXX.Core 层真实语义偏差**（本车道无写权限，仅上报）

> 两者**都能被 ObjNpc 的已覆盖路径触达**，故不是理论问题。均已加"复核守卫"用例锁定当前行为，
> Core 侧修好后这些用例会**失败** —— 那正是它们的作用（提醒本车道调用点需重新回读原文）。

### (A) `DelphiRTL.Trim` / `TrimLeft` / `TrimRight` 比 Delphi 的**窄**

现状（`src/GXX.Core/Rtl/DelphiRTL.cs:55-57`）：
```csharp
public static string Trim(string s) => s?.Trim(' ', '\t', '\r', '\n', '\f', '\v') ?? "";
```
只去 6 个字符（#9 #10 #11 #12 #13 #32）。原文 `SysUtils.Trim` 是
```
while (I <= L) and (S[I] <= ' ') do Inc(I);   // 即去所有 #0..#32
```
→ **#0..#8 与 #14..#31 原文会去掉、托管侧不去**。
要求改为：
```csharp
public static string Trim(string s)
{
    if (string.IsNullOrEmpty(s)) return "";
    int i = 0, j = s.Length - 1;
    while (i <= j && s[i] <= ' ') i++;
    if (i > j) return "";
    while (s[j] <= ' ') j--;
    return s.Substring(i, j - i + 1);
}
```
（`TrimLeft`/`TrimRight` 同理。）

**可达后果**（已加 `V1_*` 3 例）：`LoadLevelScriptAction("HERO\u0014.CHECKITEM")`
原文 → `Trim` 去 #20 → `'HERO'` → `CMD_RACE_1`；托管 → `'HERO\u0014'` ≠ `'HERO'` → `CMD_RACE_5`（目标级别解析错）。

### (B) `DelphiRTL.StrToInt64Def` 会 **Trim**，而 Delphi 的 `Val` 只跳**前导**空白

现状（`DelphiRTL.cs:76-81`）：`s = s?.Trim() ?? "";` 后 `long.TryParse`。
原文 `StrToInt64Def` 走 `Val(S, Result, E)`；`Val` **跳前导空白但不接受尾随空白**，尾随空白时 `E <> 0` → 返回 `Default`。
→ **`"123 "` 原文得 `Default`、托管得 `123`**。
要求改为：先只去**前导**空白（`TrimStart(' ')`，且 `Val` 只跳空格与制表符）再 `TryParse`：
```csharp
public static long StrToInt64Def(string s, long def)
{
    s = (s ?? "").TrimStart(' ', '\t');
    return long.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long v) ? v : def;
}
```
**可达后果**（已加 `V2_*` 3 例）：`S$V = "77 "` 时 `GetValNameValue` 的
`nValue := StrToInt64Def(sValue, nValue)` 原文保留 Default（0），托管得 77。

> 注：`DelphiRTL.StrToIntDef`（`int` 版，`:70-75`）有同样问题；`StrToInt`（`:64`）亦然。
> 另：`GXX.M2Server.Engine` 侧若也有同名垫片，请一并统一 —— 本车道不改他人文件。

## 9.3 复核**未发现**问题的项（列出以便审计）

- `DelphiRTL.Pos` 空串返回 0、1-based：与原文一致；ObjNpc 所有调用点传的是非空字面量（`.`/`<`/`$`/`>`/`[`/`]`），空串分支不可达。
- `DelphiRTL.Copy` 的 `Index > Length → ''`、`Count <= 0 → ''`、`Count` 超长截断：与原文一致（已加 `V6_*`）。
- `DelphiRTL.UpperCase` = `ToUpperInvariant`：对 ASCII 与 CJK 与原文 `UpCase` 等价（Delphi 的 `UpperCase` 是逐字节 locale 无关 UpCase）。
- `DelphiRTL.Format` 的 `%s`/`%d`：3 处调用点的实参类型与占位符**逐一对齐**（无 `ParadoxConv` 那类 `%S` 误用）。
- `EnvirWalkDoorCore.DelphiRound`（银行家舍入）：与 Delphi `Round` 一致，6 个半值边界已锁。
- `HUtil32.sub_49ADB8` / `ArrestVariable` / `ArrestStringEx`：ObjNpc 是**调用方**，其语义已由既有单元负责；本车道用 3 例锁死调用点观测行为（含 `ArrestVariable` 取出串**含 `$`**）。
- 整数除法/取模：3 处（`GetUserPrice:2073`、`GetUserItemPrice:3321` 组、`sub_4A0218:1821-1823`）逐处核对。
- `byte` 出参在范围检查关闭下的截断：2 处（`sub_4A0218` 的 4 个 `var Byte`）已用 `(byte)` 显式转换 + 用例锁定（400 → 144）。

## 9.4 复核的**局限（诚实说明）**

1. `ExtractStrings(['.'], [], ...)` 的等效**仍未从 Delphi 7 `Classes.pas` 源码逐行核实**（本机无 Delphi 源码），
   只做了"跳空串 + 不 Trim + 前导/中间/尾随点"的**行为面**锁定（`V3_*` 4 例）。Delphi 实现内部的
   `ItemBuf[0..4095]` 单项上限与 `#0` 截断分支**未覆盖**（ObjNpc 调用点已由 `Pos('.')>0` 保证非空且命令名远短于 4095）。
2. `TGroupItems.ExtractStrings` 属**会话 A 常驻区**，本车道只调用不改；它一旦被改，`V3_*` 会红。
3. 复核是**语义等价**层面的，**不是**"与真实宿主的端到端行为一致" —— 接缝默认实现仍是"无宿主"，
   端到端仍不可运行（见 §7.3）。
4. `GetVariableText`(3,252 行)、`SetValNameValue`(391)、`Get/SetBoxItemValue`(654) 四条 **Seam 例程本身未经复核**
   （它们只有外壳 + 接缝，没有可复核的实现体）。
---

# 10. 第四轮（切片 14 / 15）：吸收 `p6-m2-playersurface` 后**去接缝化 + 解锁增量** ★ 本节优先于 §9

> 前置：`merge main`（main 已含 `p6-m2-playersurface` 的 95 个成员 + 集成方补的三处 Engine 解锁点）。
> 合并后本工作树门禁：**8,101 例全绿**（与调度方给的数字一致，无破窗）。

## 10.1 commit

| # | commit | 内容 |
|---|---|---|
| 14 | `93fbf900` | **去接缝化 / 去重**：删 8 个已落地委托；`GetStdItem`/`GetStdItemName` 改为**转发单一存储**；`GetCastleNameList` 接真实现；`SendMsgToClient` 按命名裁定改名 `SendTo`（扩展方法）；`m_ZVal` 去掉冗余 `?? ""` |
| 15 | `bf92c74c` | **解锁增量 5 例程**：`SendMsgToUser`(9789) `MessageBox`(9800) `SendCustemMsg`(9837，真实现) `TBoxMonster.Initialize`(10521) `TGuildOfficial.Create`(10374) + 21 用例 |

## 10.2 覆盖口径（★ 以本节为准）

| 口径 | 切片 10 | **切片 15** |
|---|---|---|
| Covered 例程 / 112 | 57 | **62** |
| Seam 例程 / 112 | 7 | **6** |
| Missing 例程 / 112 | 48 | **44** |
| **逐行 1:1（含嵌套 `sub_4A0218` 143 行）** | 2,187 | **2,245 / 10,546 = 21.3%** |

门禁：`GXX.M2Server.Tests` **8,122 passed / 0 failed**；`Npc*` 子集 348 例。

## 10.3 去接缝化明细（删掉的接缝 —— 重新引入即为回退）

| 原接缝 | 现状 |
|---|---|
| `GetMyHero` / `GetCurrTarget` / `GetLastHiter` / `GetPoseCreate` | **删除**，改为直读 `TPlayObject.m_MyHero` / `TCreature.m_CurrTarget` / `m_LastHiter` / `GetPoseCreate()` |
| `GetPlayerPVal` / `GetPlayerSString` / `GetPlayerTVal` / `GetPlayerArrayListValue` | **删除**，改为直读 `m_nVal` / `m_sString` / `m_TVal` / `m_ArrayList`（L$ 分支按原文 5837-5845 展开三分支） |
| `GetStdItem` / `GetStdItemName` | 保留名字但改为**转发属性**到 `Engine.PlayerSurfaceMsgSeams.*` —— 三个名字**一个后备存储**（Engine 侧 `PlayerSurfaceItemSeams` 也转发到同一处） |
| `GetCastleNameList` | 默认实现改为 `CastleState.g_CastleManager.GetCastleNameList(list)`（**真实现**，只留可注入点给单测） |
| `SendMsgToClient` | 按裁定**改名 `SendTo`**，并以**扩展方法** `ObjNpcSendToExtensions.SendTo(this TCreature, ...)` 暴露；注释里写明"为什么不能叫 `SendMsg`"（`Engine.TCreature.SendMsg` 是语义不同的入队版） |
| `SendCustemMsg`（委托） | **删除** —— `TNormNpc.SendCustemMsg` 已是真实现，三个子类覆写的 `inherited` 直接走 `base` |
| `m_ZVal` 的 `?? ""` | **去掉**（Engine 已按原文把元素默认值修为 `''`，归一反而掩盖语义） |

> **收益**：原先 8 个"默认恒返回 null"的委托会把"字段语义错"伪装成"分支没命中"；直读真成员后，
> `GetLevelBaseObject*` 的 4 个目标级别分支第一次真正接到了 Engine 的字段上。

## 10.4 切片 15 的 5 例程要点

| 原文行号 | 例程 | 照抄要点 |
|---|---|---|
| 9789-9798 | `SendMsgToUser` | `g_OnlineMsgControl.boDisableUseNpc` 为真**直接退出**；`boShowNPCName` 时拼 `m_sCharName + '/'`（**斜杠**） |
| 9800-9805 | `MessageBox` | 消息体先过 `GetLineVariableText`；`wIdent = RM_MENU_OK`(20118)，**`nParam1` 传 `NativeInt(Self)`** → 托管 `m_nRecogId`；**无** online-msg 门 |
| 9837-9862 | `SendCustemMsg` | 三关门：`boSendCustemMsg` 总开关 → `g_FilterTexts`（**仅当非 nil 且 sMsg 非空**才过滤，过滤后为空则退出）→ `m_boSendMsgFlag` 单次放行；广播体用 `m_sCharName + ': '`（**冒号**）、类型 `t_Cust` |
| 10521-10525 | `TBoxMonster.Initialize` | **先** `m_btDirection := Random(3)`（只取 0..2）**再** `inherited` |
| 10374-10379 | `TGuildOfficial.Create` | `m_btRaceImg := RC_MERCHANT`(50) + `m_wAppr := 8`（**`TCastleOfficial.Create` 只 inherited，两者不一致** —— D30 差异断言） |

## 10.5 依调度方提示做的复核

- **`m_TVal`/`m_ZVal`/`m_sString` 默认值**：Engine 已在 `TPlayObject` 构造时 `MigrateStringVarDefaults` → 元素默认 `''`。
  本车道**去掉**了 `m_ZVal[n] ?? ""`（原来是在兜旧缺陷），并把注释改为"Engine 已补齐"。原 `GetValNameValue_ZValNullElement_TreatedAsEmptyString` 用例仍绿（语义不变）。
- **`m_ItemList` 由 `List<TUserItem>` 改为 `List<TUserItemView>`**：对本车道**无影响** ——
  `sub_4A0218` 只把该列表当**参数**收（`List<object>`），不直接引用 Engine 字段。⚠ 但**接线时**要注意：
  真实 `m_ItemList` 的元素类型是 `TUserItemView`，而 `sub_4A0218` 内按 `TUserItem` 解包 —— 这处**异型**已登记为待处理（见 §10.6）。
- **`GetScriptLabel` 原文坏**（`ObjPlayer.pas:15216`，调度方提示）：本车道**不碰**该路径，也不需要"修"它；
  `TNormNpc.UserSelect`/`GotoLable` 未移植，故无按"标签应当能跳转"写用例的风险。该结论已记入本报告备查。
- **`m_sScriptParams` 不存在（是 `GotoLable` 的过程级局部）**、**`SendFirstMsg` 全仓 0 命中**：已从 §8.6 的缺口清单中**剔除**，不再寻找。

## 10.6 ★ 仍需 Engine 侧补的**精确**缺口（供 `p6-m2-playersurface` 后续批次；本车道不自行声明替身）

用脚本逐个核对后的现状（✅ 已有 / ❌ 缺）：

| 原文需求 | 现状 | 阻塞的例程 |
|---|---|---|
| `TPlayObject.m_nScriptGotoCount` | ❌（只在 `Engine.TScriptPlayer` 上有，`NpcScriptState.cs:31`） | `TNormNpc.Click`(4431) |
| `TPlayObject.m_sRandomString` | ❌（同上，`NpcScriptState.cs:32`） | `TNormNpc.Click` |
| `TPlayObject.m_sNpcSelectItemName` | ❌ | `TNormNpc.Click` |
| `TPlayObject.m_sScriptGoBackLable` / `m_sScriptCurrLable` / `m_sInputData` | ✅（`NpcSession.cs:96/93/120`） | — |
| `TBaseObject.m_boObMode` | ❌ | `TNormNpc.GetShowName`(9609) |
| `FilterShowName(...)`（单元级） | ❌ | `TNormNpc.GetShowName` |
| `m_nWalkSpeed` / `m_nInitWalkSpeed` | ❌ | `TNormNpc.Initialize`(9864) |
| `TPlayObject.m_boSendMsgFlag` | ❌（本车道暂用 `NpcSeams.GetSendMsgFlag`/`ClearSendMsgFlag`） | 已绕过 |
| `g_Config.boSendCustemMsg` / `g_sSendCustMsgCanNotUseNowMsg` / `g_FilterTexts` | ❌（本车道暂用同名列接缝） | 已绕过 |
| `TPlayObject.m_UseItems`（装备格数组） | ❌（本车道暂用 `NpcSeams.GetUseItemsWeapon`，**只读**；`UpgradeWapon` 还要写回） | `UpgradeWapon` 外层体、`TMerchant.GetVariableText` 已用只读接缝绕过 |
| `TPlayObject.m_MyGuild` | ❌（Engine 走 `PlayerSurfaceItemSeams.MyGuild` 委托） | 已绕过 |
| `TCreature.m_ItemList` 元素类型 | ⚠ `List<TUserItemView>` vs 本车道 `sub_4A0218` 按 `TUserItem` 解包 —— **异型待统一** | 接线时必处理 |

> **一句话**：`TNormNpc.Click` 只差 **3 个字段**（`m_nScriptGotoCount`/`m_sRandomString`/`m_sNpcSelectItemName`）
> 就能从"虚外壳"升级为真实现；补上后 `TMerchant.Click`/`TGuildOfficial.Click`/`TCastleOfficial.Click` 三处覆写的
> `inherited` 才真正落到原文逻辑上。这是当前**投产比最高**的一处。

## 10.7 仍被阻塞的四优先方法（更新后的缺口）

| 方法 | 仍缺（❌） |
|---|---|
| `TNormNpc.GotoLable`(9263-9574, 312) | `m_nVal` ✅ 已有；仍缺 `m_sScriptParams` 已确认**不存在**（局部变量）；`LableIsCanJmp`/`SetScriptLabel` ✅ 已在 `NpcSession.cs`；需与 `HandleNpcCmds` 侧的条件/动作执行器对接 |
| `TMerchant.ClientBuyItem`(3367, 323) | `m_nGold` ✅、`AddItemToBag` ✅（`TUserItemView` 版）、`IsEnoughBag` ✅、`IsAddWeightAvailable` ✅、`SendAddItem` ✅ → **基本具备**，只差 `TUserItem`↔`TUserItemView` 的构造桥 |
| `TMerchant.ClientSellItem`(3798, 71) | `m_nGold` ✅、`IncGold` ✅ → **具备** |
| `TMerchant.UpgradeWapon` 外层体(1830-1901, 72) | `m_nGold` ✅、`DecGold` ✅、`GoldChanged` ✅、`SendDelItem` ✅、`RecalcAbilitys` ✅、`FeatureChanged` ✅、`CheckItems` ✅ → 仍缺 `m_UseItems`（**读写**）与 `GotoLable` |
| `TMerchant.UserSelect`(2087, 814) | 大批成员已落地；仍缺 `m_sAutoSendMsg` 等少数；建议**分片**推进（按 `@buy`/`@sell`/`@repair` 切） |

> 结论：**解锁面比预期大得多** —— `ClientSellItem`(71) 与 `ClientBuyItem`(323) 现在基本可直接做，
> 只差 `TUserItemView` 的构造/解包桥。若调度方希望，我下一轮从 `ClientSellItem` 起步（最小、可独立验证）。
---

# 11. 第五轮（切片 17 / 18）★ 本节优先于 §10

## 11.1 commit

| # | commit | 内容 |
|---|---|---|
| 17 | `0706c173` | `TMerchant.ClientSellItem`(3798-3867，71 行) + 嵌套 `sub_4A1C84`(3800-3811) 1:1（+21 用例） |
| 18 | `477e721c` | `TNormNpc.Click`(4431-4442) 由"虚外壳"**收成真实现**（调度方补齐 3 个字段后解锁） |
| — | `7836d0e2` | `merge main`（吸收 `TPlayObject.PlayerSurface.ScriptFields.cs` 的 3 个字段） |

门禁：`GXX.M2Server.Tests` **8,209 passed / 0 failed**（合并后基线 8,131）。

## 11.2 覆盖口径（★ 以本节为准）

| 口径 | 切片 15 | **切片 18** |
|---|---|---|
| Covered 例程 / 112 | 62 | **64** |
| Seam 例程 / 112 | 6 | **5** |
| Missing 例程 / 112 | 44 | **43** |

逐行 1:1（含嵌套过程 `sub_4A0218` 143 行 + `sub_4A1C84` 12 行）≈ **2,328 / 10,546 = 22.1%**。

## 11.3 `TNormNpc.Click` 收成真实现（切片 18）

调度方在 `3698e30d` 补齐 `TPlayObject` 的 `m_nScriptGotoCount`(:149) / `m_sRandomString`(:356) /
`m_sNpcSelectItemName`(:357) 后，本车道按原文 4433-4438 的**行序**落 6 次归零，末尾
`PlayerSurfaceNpcSeams.GotoLable(this, PlayObject, "@main", false)`（Engine 接缝 `NpcSession.cs:44`；
原文第 4 个默认参数 `UseParams = False` 由接缝隐含）。

- `NpcSeams.Click` 委托**已删除** → `TMerchant.Click`(3228)/`TGuildOfficial.Click`(10049)/`TCastleOfficial.Click`(1107)
  三处覆写的 `inherited` 现在**真正落到原文逻辑**（用"6 字段被归零 + GotoLable 收到 `@main`"作为可观测证据，已加用例）。
- `Click` 在托管侧确认为 **`public virtual`**（台账「基类方法 + 子类 inherited ⇒ 必须虚方法」的规程），已复核。

## 11.4 `ClientSellItem`（切片 17）要点与差异断言

- 嵌套函数 `sub_4A1C84`：`StdMode ∈ {25,30}` 时要求 `Dura >= 4000`（边界 4000 允许，3999 拒绝）。
- 三段 `RM_USERSELLITEM_FAIL` 的 **`nParam1` 各不相同**，已逐条断言：
  `禁售 = 0`（3825/3866）、`价格不合理 = 0`（3866）、`IncGold 失败 = -1`（3863）。
- **D33**：3847 `g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice)` —— 传的是**升级武器费**而不是本次售价；
  已用 `nUpgradeWeaponPrice = 12345` vs 售价 50 的用例锁死。
- **D34**：3855-3856 取回 `StdItem` 后**不判 nil** 就读 `NeedIdentify`（原文 AV）。
- `IsFromTradingDlg = True` 时**所有** FAIL/OK 包都被抑制，但"入商品列表"照做 —— 已断言。
- 托管签名 `ref TUserItem`（必需）：3830 的 `GetUserItemPrice` 在 `StdMode = 43` 时会**就地改写 `DuraMax`**，
  已用 `DuraMax: 5000 → 10000` 的用例证明 `ref` 不可省。

## 11.5 ★★ `TUserItem` ↔ `TUserItemView` 的**精确映射需求**（调度方要求 #1）

### 现状（已核实的三处证据）

1. `Engine/AddAbility.cs:64-69` —— `TUserItemView` **只有 3 个成员**：
   ```csharp
   public class TUserItemView
   {
       public ushort wIndex;
       public readonly byte[] BtValue = new byte[14];
       public List<(byte btBindType, byte btPercent, int nValue)> CustomProperties = new();
   }
   ```
2. `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs:85/88/91/94` —— 因容器元素类型换成 `TUserItemView`，
   该车道**不得不新增 4 个委托**去读"本该在物品上的字段"：
   `ItemMakeIndex` / `ItemName` / `ItemDura` / `ItemDuraMax`，且注释已自认
   （`:82`）：**"接缝：待 `TUserItemView` 补全 `TUserItem` 面（或改用 `TUserItem`）后直接取值。"**
3. `Engine/ObjBase.cs` 的 `m_ItemList` 现为 `List<TUserItemView>` —— 而 `GXX.Core.Protocol.TUserItem`
   （`Grobal2.Types6.cs:13`）是**已 1:1 移植的权威记录**（含 `MakeIndex`/`Dura`/`DuraMax`/`boIsBind`/
   `btBindOption`/`NameStr`/`GetBtValue`/`SetBtValue` 及 DB/wire 布局）。

### ObjNpc.pas 侧对物品的实际字段需求（逐字段核对）

| 原文用法（示例行） | `GXX.Core.Protocol.TUserItem` | `TUserItemView` |
|---|---|---|
| `UserItem.wIndex`（3281/3875/1730…） | ✅ | ✅ |
| `UserItem.btValue[n]`（1787/3327-3334…） | ✅ `GetBtValue/SetBtValue` | ✅ `BtValue[]` |
| `UserItem.Dura`（1714/3289/3808/3855…） | ✅ | ❌（靠 `PlayerSurfaceItemSeams.ItemDura` 委托） |
| `UserItem.DuraMax`（3285/3302/3355…，**且被写回**） | ✅ | ❌（靠 `ItemDuraMax` 委托，**只读**，写不回去） |
| `UserItem.MakeIndex`（1715/1788/3858…） | ✅ | ❌（靠 `ItemMakeIndex` 委托） |
| `UserItem.Name` / `NameStr`（1787-1790/3858） | ✅ | ❌（靠 `ItemName` 委托） |
| `UserItem.boIsBind`（3821） | ✅ | ❌ |
| `UserItem.btBindOption`（3821，配 `GetUserItemBindValue`） | ✅ | ❌ |

**结论**：`TUserItemView` 缺 **6 个 ObjNpc 必需字段**（`Dura`/`DuraMax`/`MakeIndex`/`Name`/`boIsBind`/`btBindOption`），
其中 `DuraMax` 还是**可写**的（`GetUserItemPrice` 在 `StdMode = 43` 时写回）—— 而 `PlayerSurfaceItemSeams` 的
4 个委托全是**只读 `Func`**，从类型上就**无法**承载该写回。
→ 这是 `TMerchant.UserSelect`/`ClientBuyItem`/`UpgradeWapon` 外层体当前**共同的硬阻塞**。

### 需求（请调度方二选一；我**不在** `Npc/` 里另造一套物品类型）

**方案 A（推荐，改动最小）**：`ObjBase.m_ItemList` 的元素类型**改回 `GXX.Core.Protocol.TUserItem`**（权威记录），
`TUserItemView` 退化为**按需构造的视图**（在 `GetAccessory.Apply` 里从 `TUserItem + TStdItemView` 现造）。
收益：① `PlayerSurfaceItemSeams` 的 4 个 `ItemXxx` 委托可**全部删除**；② 写回 `DuraMax` 天然可行；
③ 我的 `sub_4A0218`/`ClientSellItem`/`GetUserItemPrice` **一行不用改**即可接线。

**方案 B**：保留 `List<TUserItemView>`，但把 `TUserItemView` 做成**权威记录的包装**（不复制字段）：
```csharp
public class TUserItemView
{
    public GXX.Core.Protocol.TUserItem Item;          // ← 权威记录（承载 Dura/DuraMax/MakeIndex/NameStr/boIsBind/btBindOption）
    public List<(byte btBindType, byte btPercent, int nValue)> CustomProperties = new();
    public ushort wIndex { get => Item.wIndex; set => Item.wIndex = value; }   // 兼容既有读取点
    public byte[] BtValue => ...;                     // 或改为转发 GetBtValue/SetBtValue
    // 再补 Dura / DuraMax（可写）/ MakeIndex / NameStr / boIsBind / btBindOption 的转发属性
}
```
收益同上；代价是要动 `AddAbility.cs` 的既有读取点。

> 无论哪种，**请勿在 `Npc/` 侧要求我加适配器** —— 那会形成"第二套物品表示"，正是本工程已犯 8 次的重复定义模式。

### 附带的一处同型问题（请一并裁定）

`GetItemAddValue`（原文 `ItemUnit.GetItemAddValue(UserItem: pTUserItem; var StdItem: TStdItem)`，ObjNpc.pas:1733）
在托管侧有**两套标准物品表示**：我用的 `GXX.Core.Protocol.TStdItem`（`Grobal2.Types2.cs:26`，1:1 权威）
与 `Engine.TStdItemView`（`AddAbility.cs` 顶部，`GetAccessory.Apply` 的入参）。
→ 接线时需要一层 `TStdItem` ↔ `TStdItemView` 的映射（**字段名还不完全一致**：如 `AniCount` vs `Anicount`、`Name` vs `NameStr`）。
我的 `NpcSeams.GetItemAddValue(ref TUserItem, ref TStdItem)` 目前按**权威侧**定名，若 Engine 决定以 `TStdItemView`
为 `GetAccessory` 的正式入参，请告知，我改签名（这是本车道白名单内的文件）。

## 11.6 剩余 43 条：下一轮建议顺序（按 §11.5 解锁后）

1. **`TMerchant.ClientBuyItem`(3367-3689, 323)** —— 依赖 `AddItemToBag`/`IsEnoughBag`/`IsAddWeightAvailable`/`SendAddItem`
   （均已就绪）+ 物品表示统一（§11.5）。**建议紧接着做**。
2. **`TMerchant.UpgradeWapon` 外层体**(1830-1901, 76) —— 除 `m_UseItems`（读写）与 `GotoLable` 外均已就绪；
   `sub_4A0218` 已覆盖。
3. **`TMerchant.UserSelect`(2087-2900, 814)** 按 `@buy` / `@sell` / `@repair` **分片**推进。
4. **`TNormNpc.GotoLable`(9263-9574, 312)** —— `m_nVal` ✅、`LableIsCanJmp`/`SetScriptLabel` ✅，
   主要待与 `HandleNpcCmds` 的条件/动作执行器对接。
---

# 12. 第六轮（切片 20）：方案 A 第①步已做；**第②步就地停下并报告阻塞** ★ 本节优先于 §11

## 12.1 commit

| # | commit | 内容 |
|---|---|---|
| 20 | `692cb768` | 方案 A 第①步：`ObjBase.m_ItemList` 改 **`List<TUserItem?>`**；`sub_4A0218` 强类型化；第②步以**精确阻塞**形式登记 |

门禁：`dotnet build GXX.slnx` 0 error；`GXX.M2Server.Tests` **8,209 passed / 0 failed**（构建**保持绿**）。
越区检查：本次仅动 `Engine/ObjBase.cs` + `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs`（**已在 `176edd2e` 授权**）
+ 本车道自己的 `Npc/`、`NpcObjNpc*` 测试。

## 12.2 调度方的侦察结论**完全正确**，且已用脚本二次确认

```powershell
Select-String -Path (all src/tests *.cs) -Pattern 'm_ItemList' | ? { 非注释行 }
→ src\GXX.M2Server\Engine\ObjBase.cs:170: public List<TUserItemView> m_ItemList = new();
```
**全仓只有它自己的声明这一处** —— 即上一轮把它的类型改成 `TUserItemView`，**没有产生任何效果**。
`BagItems`（`TCreature.PlayerSurface.Items.cs:180`）指向的是**自己的后备字段 `m_BagItems`**（:186），
双容器至今存在。「改一个零调用方字段的类型 ≠ 接上了容器」这条自省记号**证据充分**。

## 12.3 第①步已做 —— 但元素类型用 **`List<TUserItem?>`（可空）** 而非 `List<TUserItem>`

**这是对裁定的一处细化，理由是保真**：原文 `ObjBase.pas:322 m_ItemList: TList` 的槽位是
`pTUserItem` **指针，允许为 nil** —— 原文多处显式判空：
`ObjNpc.pas:1710`（`if UserItem = nil then Continue`）、`:4254`（`if ItemList = nil then Continue`）等。
值类型元素**无法表达"空槽"**，故按 `PTUserItem` 语义取可空值类型；
这与 p6 车道对 `m_UseItems`（`TUserItemView?[]`，`RecalcChain.cs:101`）的既有做法**一致**。
（若调度方坚持非可空 `List<TUserItem>`，请告知 —— 代价是丢掉原文的空槽语义，且上述判空分支将不可达。）

`sub_4A0218` 同步强类型化为 `List<TUserItem?>`，原文 1710-1711 的判空分支**得以保留**（原 `List<object>` 版是装箱 + 强转）。

## 12.4 ★★ 第②步**就地停下**：改签名会连带打到**另一条车道的测试文件**

裁定第 2 条要求（`BagItems => m_ItemList`、删 `m_BagItems`、删 4 个只读委托）会连带改变
`TCreature.PlayerSurface.Items.cs` 的 **7 个公开成员签名**：
`Bag` / `AddToBag` / `AddItemToBag` / `CheckItems` / `CheckItemsIndex` / `SendAddItem` / `SendDelItem`。

而实测这些成员在 **`tests/GXX.M2Server.Tests/PlayerSurfaceItemsTests.cs`** 里有 **约 40 处调用点**：

| 依赖的旧签名 | 该文件中的调用点（行号） |
|---|---|
| `PlayerSurfaceItemSeams.ItemMakeIndex/ItemName/ItemDura/ItemDuraMax`（**4 个委托的直接赋值**） | 63 / 64 / 65 / 66 / 570 |
| `AddItemToBag(TUserItemView)` | 181 / 190 / 191 / 203 / 208 / 230 / 240 / 241 / 251 / 264 / 277 / 297 / 323 / 324 / 340 / 357 / 358 / 370 |
| `CheckItems(string, out TUserItemView?)` | 311 / 326 / 342 / 343 / 344 / 360 |
| `CheckItemsIndex(string, out TUserItemView?)` | 371 |
| `SendAddItem(TUserItemView)` | 440 / 454 / 469 / 483 / 501 / 520 / 538 / 556 / 583 |
| `SendDelItem(TUserItemView)` | 604 / 620 / 637 / 651 / 665 |
| `PlayerSurfaceItemSeams.ResetDefaults()`（复位 4 个委托） | 27 / 34 |

**该文件的归属是车道 `p6-m2-playersurface`**（分区表：`GXX.CSharp/tests/GXX.M2Server.Tests/PlayerSurface*`），
**不在本车道（`Npc*`）的分区表内**。据「绝不改他人文件」「每次提交可编译」两条纪律：
**本车道未执行第②步** —— 否则提交即构建红（且改的是别人车道的测试）。

### 需要的裁定（二选一）

1. **把 `tests/GXX.M2Server.Tests/PlayerSurfaceItemsTests.cs` 加入本车道分区**（与我已获批的两个 Engine 文件同批），
   我随后一次性完成：`BagItems => m_ItemList`、删 `m_BagItems`、删 4 个委托、签名统一为 `TUserItem?`、
   **并同步改那 40 处调用点**（含把那 4 个委托的赋值改成直接设置 `TUserItem` 的 `MakeIndex`/`Dura`/`DuraMax`/`NameStr`）；
2. **或**由 `p6-m2-playersurface` 自行适配其测试后再落这一步（我这边保持现状，`sub_4A0218` 已就绪但仍接不上真实 `m_ItemList`）。

> 无论哪种，**这 40 处调用点是硬约束**：第②步不可能"只改 Engine 两个文件"就完成。这是我停下的**唯一**原因。

## 12.5 ★ `TStdItem` vs `TStdItemView` 的命名/字段对照表（裁定第 3 条要求「只登记，本次不做」）

| 语义 | 权威侧 `GXX.Core.Protocol.TStdItem`（`Grobal2.Types2.cs:26`） | 视图侧 `Engine.TStdItemView`（`AddAbility.cs`） | 备注 |
|---|---|---|---|
| 名称 | `NameStr`（**属性**，读 `fixed byte Name[61]`） | `Name`（**string 字段**） | **同概念两套命名** |
| DB 名 | `DBNameStr`（属性）/ `fixed byte DBName[61]` | `DBName`（string） | 同上 |
| 动画数 | `AniCount`（`ushort`） | **`Anicount` 与 `AniCount` 两个字段并存** | ⚠ 视图里疑似**重复定义**（大小写不同），需 p6 侧确认哪个是活的 |
| 需求 | `Need`（`int`） | `Need`（`byte`） | **类型不一致**（int vs byte） |
| 需求等级 | `NeedLevel`（`int`） | `NeedLevel`（`byte`） | **类型不一致** |
| HP / MP | `int` | `uint` | **类型不一致** |
| 重量 | `Weight` | ❌ 缺 | 视图不需要 |
| 需鉴定 | `NeedIdentify` | ❌ 缺 | **ObjNpc 需要**（`GetUserItemPrice`/`sub_4A0218`） |
| 最大持久 | `DuraMax` | ❌ 缺 | **ObjNpc 需要**（`GetUserItemPrice`/`AddItemToGoodsList`） |
| 价格 | `Price` | ❌ 缺 | **ObjNpc 需要**（`GetItemPrice`） |
| 可叠加 | `OverLap` | ❌ 缺 | **ObjNpc 需要**（`CheckOverLapItem`） |
| 外形 | `Looks` | ❌ 缺 | |
| 其它 | `Reserved/Reserved1/Color/Light/Horse/Expand1-5/Elements/InsuranceCurrency/InsuranceGold/BagEffect/BodyEffect/Effect` | ❌ 缺 | 视图只覆盖"能力聚合"需要的子集 |

**结论**：`TStdItemView` 是**能力聚合专用子集**（缺 `NeedIdentify`/`DuraMax`/`Price`/`OverLap` 这四个 ObjNpc 必需的），
且与权威侧**存在 2 处同概念异名 + 3 处类型不一致 + 1 处疑似重复定义**。
`GetItemAddValue` 按裁定**保持权威侧定名**（`NpcSeams.GetItemAddValue(ref TUserItem, ref TStdItem)`），已复核无需改。
若将来 Engine 决定以 `TStdItemView` 为 `GetAccessory` 的正式入参，需先补上 4 个字段并消解上述差异 —— **本次仅登记**。

## 12.6 当前状态与下一轮

- **已就绪、未被阻塞**：`sub_4A0218` 已强类型化；`ClientSellItem` 已完成；`m_ItemList` 类型已对齐权威表示。
- **被 §12.4 阻塞**：`BagItems` 接线、删 4 个委托、以及所有需要**真实背包容器**的例程
  （`ClientBuyItem`/`UpgradeWapon` 外层体/`UserSelect` 的 `@buy`/`@sell` 分片）。
- **本轮未做的其它项**（诚实登记）：裁定第 4 条的"`GetAccessory.Apply` 调用处按需现造视图" ——
  该步与第②步是同一批改动（视图现造点就在 `BagItems`/`SendAddItem`/`SendDelItem` 内部），故一并延后。
---

# 13. 第七轮（切片 22）：方案 A **第②步完成** + `Anicount`/`AniCount` 确定性结论 ★ 本节优先于 §12

## 13.1 commit

| # | commit | 内容 |
|---|---|---|
| 22 | `8564552a` | 方案 A 第②步：`m_ItemList` 上移 `TCreature`；`BagItems` 接单一容器；删 `m_BagItems` + 4 个只读委托；7 个公开成员签名收敛为 `TUserItem?`；`PlayerSurfaceItemsTests` 约 40 处适配 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **8,209 passed / 0 failed**。
越区检查为空（仅动 2 个已授权 Engine 文件 + 已授权测试文件）。

## 13.2 第②步的**全部改动点**（依裁定要求列出）

### `src/GXX.M2Server/Engine/ObjBase.cs`
| 改动 | 说明 |
|---|---|
| `m_ItemList` **从 `TPlayObject` 上移到 `TCreature`** | **裁定外的一处必要补充**：`BagItems`/`AddItemToBag`/`IsEnoughBag`/`CheckItems` 都在 `TCreature` 上，而 `m_ItemList` 原声明在 `TPlayObject` → `TCreature.BagItems` 根本**够不到**它（这正是双容器产生的根因）。原文 `ObjBase.pas:322 m_ItemList: TList` 在 **`TBaseObject`** 上 → 落到 `TCreature` 才符合原文归属 |
| 类型 `List<TUserItemView>` → **`List<TUserItem?>`** | 方案 A：`TUserItem` 为唯一存储与权威；可空保留 `pTUserItem` 的空槽语义（你已采纳） |
| 字段注释重写 | 记录裁定、可空理由、**别名 vs 值复制**的代价、以及"改零调用方字段的类型 ≠ 接上容器"的自省条（防重犯） |

### `src/GXX.M2Server/Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs`
| 成员 | 旧 | 新 |
|---|---|---|
| `BagItems` | `List<TUserItemView> => m_BagItems` | **`List<TUserItem?> => m_ItemList`** |
| `m_BagItems` | `readonly List<TUserItemView>` | **已删除** |
| `Bag` | `IReadOnlyList<TUserItemView>` | `IReadOnlyList<TUserItem?>` |
| `AddToBag` | `(TUserItemView)` | `(TUserItem?)` |
| `AddItemToBag` | `virtual bool (TUserItemView)` | `virtual bool (TUserItem?)`（**仍 `virtual`**，原文 :708） |
| `CheckItems` | `int (string, out TUserItemView?)` | `int (string, out TUserItem?)` |
| `CheckItemsIndex` | `int (string, out TUserItemView?)` | `int (string, out TUserItem?)` |
| `SendAddItem` | `(TUserItemView)` | `(TUserItem)` |
| `SendDelItem` | `(TUserItemView)` | `(TUserItem)` |
| **删除** | `PlayerSurfaceItemSeams.ItemMakeIndex` / `ItemName` / `ItemDura` / `ItemDuraMax`（4 个 `Func<TUserItemView, ...>`）+ `ResetDefaults` 里对应 4 行 | 改为**直读 `TUserItem` 字段**（`MakeIndex` / `NameStr` / `Dura` / `DuraMax`） |
| **新增** | —— | `internal static TUserItemView ToItemView(TUserItem)` —— 方案 A 的**视图现造点**（`SendAddItem` 的 `UserItemToClientItem` 调用处用）。⚠ 有损：`TUserItem.btValue` 是 `int[14]` 而视图 `BtValue` 是 `byte[14]` → `(byte)` 窄化（与 `GetAccessory` 读 byte 的口径一致） |
| `GetAccessory.Apply` 入参 | `TUserItemView` **不变**（裁定第 3 条） | `RecalcChain`/`m_UseItems` **一行未改** — 见 `RecalcChain.cs:47-55` |

### `tests/GXX.M2Server.Tests/PlayerSurfaceItemsTests.cs`（约 40 处）
| 类别 | 处数 | 处理 |
|---|---|---|
| `Item(...)` 工厂 | 1 | 返回类型 `TUserItemView` → **`TUserItem`**，`MakeIndex`/`NameStr`/`Dura`/`DuraMax` 改为**直接赋值** |
| `SeamMakeIndex`/`SeamName`/`SeamDura`/`SeamDuraMax` 四个字典 + `WireItemFieldSeams()` | 1 定义 + 6 调用 | **全部删除**（4 个委托已不存在） |
| `AddItemToBag(Item(...))` | 17 | 隐式转换 `TUserItem` → `TUserItem?`，**无改动**（仅工厂返回类型变了） |
| `CheckItems`/`CheckItemsIndex` 的 `out TUserItemView?` | 7 | → `out TUserItem?` |
| `SendAddItem`/`SendDelItem` 实参 | 14 | 工厂返回类型变了即自动适配 |
| `it.BtValue[13] = x` | 3 | → `it.SetBtValue(13, x)`（`TUserItem` 的 `btValue` 是 `fixed int[14]`） |
| `p.Bag[0].wIndex` | 3 | → `p.Bag[0]!.Value.wIndex`（可空值类型） |

## 13.3 ★ 断言语义**变更**的 3 处（依裁定要求单列，未静默改弱）

| 用例 | 原断言 | 现断言 | 性质 |
|---|---|---|---|
| `AddItemToBag_KeepsReferenceSemantics_LikeOriginalPointerList` | `it.wIndex = 999;` 后 `p.Bag[0].wIndex == 999`（**别名共享**） | `it.wIndex = 999;` 后 `p.Bag[0]!.Value.wIndex == 5`（**值复制**：本地副本不影响背包） | **真语义变更** —— 方案 A 的固有代价。注释已说明"写回槽位才生效"是调用方契约 |
| `CheckItems_FindsByName_ReturnsIndexAndItem` | `Assert.Same(b, found)`（引用相等） | `Assert.Equal(b.wIndex, found!.Value.wIndex)` + `Assert.Equal(b.MakeIndex, found!.Value.MakeIndex)`（内容相等） | 断言方式变更，**断言语义（"按名命中并返回该件"）保持** |
| `CheckItems_FirstMatchWins_WhenDuplicates` | `Assert.Same(a, found)` | `Assert.Equal(11, found!.Value.MakeIndex)` + `Assert.NotEqual(b.MakeIndex, ...)`（用 `MakeIndex` 区分 a/b） | 断言方式变更，**"第一个命中者胜出"的语义仍被锁死**（原文 41681 的 `Break`） |

**语义不变的改动**（仅类型/签名适配，已核对）：`SendAddItem_*Bead*` 4 例（`SeamDura[it]=9` → `it.Dura=9`）、
`SendAddItem_FunctionNpcBranch_*`（原靠 `ItemMakeIndex` 委托注入 777 → 改为 `Item(1, 777)` 直设字段）、
`SendDelItem_*` 3 例（`BtValue[13]` → `SetBtValue(13, ...)`）—— 期望值与分支覆盖**均未改**。

## 13.4 ★★ `TStdItemView.Anicount` / `AniCount`：**确定性结论**

### 声明处（`src/GXX.M2Server/Engine/AddAbility.cs`，`class TStdItemView`）
```
:41    public ushort Anicount;      // 小写 c
:60    public ushort AniCount;      // 大写 C
```

### **全部**读写点（`src` **与** `tests`，已按台账 §28.3 两处都搜）
| 字段 | 读 | 写 |
|---|---|---|
| **`Anicount`**（:41） | **`Engine/RecalcChain.cs:55`**：`self.ApplySpecialItemCode(std.Anicount);` —— **特戒代码开关**（原文 `AddAbilitysByCode` 的 `IsShape` 双路之一，`i != U_SHIELD`） | （无生产写点） |
| **`AniCount`**（:60） | **`Engine/GroupItems.cs:476`**：`if (stdItem.AniCount == 0 && activeFengHao != i)` —— **封号令**判定 | `tests/.../FormJ57Tests.cs:384/438/439`：`new TStdItemView { Name = "封号令", AniCount = 1 }`（对象初始化器写入） |

另有**同名但不同物**的其它类型（勿混淆）：`GXX.Core.Protocol.TStdItem.AniCount`（`Grobal2.Types2.cs:33`，**权威侧**）、
`MapCellCore.cs:458`、`EIMapRenderSchedule.cs:92`、`SceneComposer.cs:52`（客户端渲染用）。

### 结论
1. **两个都在用，且用途不同** —— 不是"一活一死"，而是**同一 Delphi 字段被拆成两个托管字段、各被一侧消费**：
   `Anicount` 服务**特戒代码**，`AniCount` 服务**封号令**。
2. **这是真缺陷**：原文只有**一个** `AniCount`（`M2Share/Grobal2` 的 `TStdItem`）。
   任何 `TStdItem` → `TStdItemView` 的构造若只填其中之一，**另一侧就读到默认 0** ——
   典型的"沉默中性值"故障（与 §13.2 删掉的那 4 个委托同型）。当前 `RecalcChain.cs:47` 的
   `StdItemResolver` 由谁提供、是否同时填两个字段，**未经验证**（属 p6 车道）。
3. **建议的最小修法（一行）**：把 `:60` 的字段改成**转发属性**，让两处消费共享同一份数据 ——
   ```csharp
   // AddAbility.cs:60 —— 原文只有一个 AniCount，此处是历史重复定义；改为别名
   public ushort AniCount { get => Anicount; set => Anicount = value; }
   ```
   风险：`FormJ57Tests` 用的是**对象初始化器**（`new TStdItemView { AniCount = 1 }`）→ 属性初始化器同样合法，
   **测试无需改**；`GroupItems.cs:476` 的读取语义变为读 `Anicount`（正是想要的统一）。
   若担心反射列字段（`GetFields`）的调用方，则退而求其次：保留字段、在 `RecalcChain` 侧改读 `AniCount`——
   但那只是把不一致挪个位置，**不推荐**。
4. **本次未改**（`Engine/AddAbility.cs` 不在我分区）—— 结论交你决定由谁落。

## 13.5 方案 A 的**固有代价**（必须长期知晓，已写进 `BagItems` 的代码注释）

元素是**可空值类型** → `BagItems.Add(x)` **复制值**，与原文"加入指针、与调用方共享同一对象"的**别名语义不同**：
1. 调用方改物品后**必须写回槽位**：`var t = BagItems[i]!.Value; ...改 t...; BagItems[i] = t;`
   （`BagItems[i]!.Value.Dura = x` **不可编译** —— 值属性不可变）；
2. `GetUserItemPrice(ref TUserItem, ...)` 这类"按引用就地改写"的调用**不能**直接传 `BagItems[i]`
   （`List<T>` 索引器不可 `ref`），同样要先取出、改完写回；
3. 若要恢复别名语义，唯一途径是回到**方案 B**（包装类持有 `TUserItem` 引用）。
> **当前缺口**：`IReadOnlyList<TUserItem?> Bag` 是只读视图，**没有公开的写回入口**
> （`AddToBag` 只能追加）。后续 `ClientBuyItem`/`UpgradeWapon`/`UserSelect` 分片若需要"就地改背包里那件"，
> 需要补一个最小写入口（如 `bool SetBagItem(int index, TUserItem? item)`）。**本轮未加**（不在裁定范围），已登记为待办。

## 13.6 下一轮

按裁定顺序：`ClientBuyItem`(323) → `UpgradeWapon` 外层体 → `UserSelect` 的 `@buy`/`@sell`/`@repair` 分片。
**新增前置**：§13.5 的"背包写回入口"（若 `ClientBuyItem`/`UpgradeWapon` 需要就地改背包物品）——
到时我会先只申请这一个成员，再动手。
---

# 14. 第八轮（切片 24 / 25）：`AniCount` 重复字段修复 + **D35 偏差正式登记** + `SetBagItem` 入口 ★ 本节优先于 §13

## 14.1 commit

| # | commit | 内容 |
|---|---|---|
| 24 | `2d51be7f` | `TStdItemView.AniCount` 由**重复字段**改为**转发属性**（同一 Delphi 字段被拆成两处、各被一侧消费） |
| 25 | （本节随附） | **D35 偏差正式登记**（报告 + 两处代码注释）+ `SetBagItem` 写回入口 + 5 例 |

门禁：`GXX.M2Server.Tests` **8,281 passed / 0 failed**。

> ⚠ **分区表滞后提示**：调度方已在本轮消息中**直接授权** `Engine/AddAbility.cs`，但 main 的
> `tools/lane-zones.tsv` 里**尚未出现该路径**（`git show main:GXX.CSharp/tools/lane-zones.tsv | grep AddAbility` 为空）。
> 本车道按**直接授权**执行，并在此显著登记 —— 请集成方补一条 `!...Engine/AddAbility.cs`，
> 否则 `verify-lanes.ps1` 会把它报成 OUT-OF-ZONE。
> 另已核实：**全仓无任何调用方依赖"字段"语义**（对 `AniCount`/`Anicount` 无 `ref`/`out`/`GetField`/`nameof` 用法），
> 故改为属性**零破坏**，测试**无需改动**（对象初始化器与属性初始化器等价）。

## 14.2 `AniCount` 修复的实质（属台账 §29「沉默中性值」故障同族）

`TStdItemView`（`Engine/AddAbility.cs`）原先把**同一个** Delphi 字段 `TStdItem.AniCount` 拆成两个托管字段：

| 字段 | 用途 | 消费点 |
|---|---|---|
| `Anicount`（:41，小写 c） | **特戒代码开关** | `Engine/RecalcChain.cs:55` `self.ApplySpecialItemCode(std.Anicount)` |
| `AniCount`（:60，大写 C） | **封号令判定** | `Engine/GroupItems.cs:476` `stdItem.AniCount == 0`（写：`tests/FormJ57Tests.cs:384/438/439` 对象初始化器） |

**危害**：任何构造 `TStdItemView` 的地方（如 `RecalcChain.cs:47` 的 `StdItemResolver`）**只填其一**时，
另一侧**静默读到 0** —— "默认值看起来合法，于是错误被伪装成'分支没命中'"。
**修法**：`:60` 改为 `public ushort AniCount { get => Anicount; set => Anicount = value; }` → 两处消费共享同一份数据。

## 14.3 ★★ 正式偏差 **D35**：值语义 vs 指针语义

| 项 | 内容 |
|---|---|
| **编号** | **D35** |
| **位置** | `Engine/ObjBase.cs`（`m_ItemList` 声明）+ `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs`（`BagItems`/`AddToBag`/`SetBagItem`） |
| **偏离点** | `m_ItemList` 的元素类型由原文的 `pTUserItem` **指针**改为 `TUserItem?` **可空值类型** |
| **原文行为** | `TList` 存指针 → `BagItems.Items[I]^.X := v` 与"调用方手上那件"是**同一对象**，**改一处两处都变**（别名共享）。`ObjNpc.pas:1710/4254` 等处的 `if UserItem = nil` 判空也因此有意义 |
| **托管行为** | `Add`/`SetBagItem` 都是**值复制** → 改本地副本**不影响**背包；必须**显式写回槽位**才生效 |
| **为什么必须偏离** | `GXX.Core.Protocol.TUserItem` 是 `struct`（`Grobal2.Types6.cs:13`，1:1 的 wire/DB 权威布局）。值类型**无法表达**"共享同一实例"。可空（`TUserItem?`）是为了保住原文的**空槽**语义，但保住空槽 ≠ 保住别名 |
| **触发面（真实代码路径）** | ① `ObjNpc` 侧 `GetUserItemPrice(ref TUserItem, ...)` 在 `StdMode = 43` 时**就地改写** `DuraMax`（`:3302-3303`）——传 `BagItems[i]` 会**编译不过**（`List<T>` 索引器不可 `ref`）；② `UpgradeWapon` 的 `User.m_UseItems[U_WEAPON].wIndex := 0`（`:1886`）；③ `ClientBuyItem`/`UserSelect` 的"取出→改→放回"模式 |
| **调用方契约（强制，已写入两处代码注释）** | `var t = BagItems[i]!.Value; ...改 t...; SetBagItem(i, t);` 或 `var t = BagItems[i]!.Value; GetUserItemPrice(ref t, ...); SetBagItem(i, t);` |
| **为什么要"台账条目 + 唯一编号"而不是只写注释** | 这类差别**编译过、多数单测过**，只在"改了一处、另一处没变"时暴露 —— 与 §25.2/§26.2/§29 那些"假完成/沉默中性值"同族；只写注释会被后来人当成实现细节而"顺手简化"掉 |
| **若将来要恢复别名语义** | 唯一途径 = 回到**方案 B**：让 `TUserItemView` **持有 `TUserItem` 的引用**（包装类），使 `BagItems` 的元素成为引用类型。届时 D35 可**注销** |

**已被迫改写的用例（唯一一处真语义变更）**：
`PlayerSurfaceItemsTests.AddItemToBag_KeepsReferenceSemantics_LikeOriginalPointerList`
—— 原断言 `it.wIndex = 999` 后背包读到 999（**别名共享**）；现断言背包仍读到复制进去的 `5`（**值复制**）。
用例名与注释已同步改写并把 D35 指出来，**不是静默适配**。

## 14.4 `SetBagItem`（方案 A 的必要补充入口，已获批准）

```csharp
public bool SetBagItem(int index, TUserItem? item)   // 越界返回 false（不静默忽略）
```
**原文依据**：Delphi `TList.Items[Index]` 是**可写属性**
（`property Items[Index: Integer]: Pointer read Get write Put`）→ 原文完全允许 `BagItems.Items[I] := UserItem;`。
托管侧 `Bag` 是 `IReadOnlyList`（只读）、`AddToBag` 只能追加 → **必须**补一个等价写入口，
否则 D35 契约里的"写回槽位"根本无路可走。

**用例 5 例**：正常写回生效 / `-1` 越界 / 恰好在 `Count` 处越界 / 空背包任意下标 / 写 `null` 清空槽
（末例对应原文 `pTUserItem` 槽可为 nil）。

## 14.5 下一轮

按裁定顺序做 **`ClientBuyItem`(3367-3689, 323 行)** —— 前置（`m_nGold`/`AddItemToBag`/`IsEnoughBag`/
`IsAddWeightAvailable`/`SendAddItem` + `SetBagItem`）**现在全部就绪**。随后 `UpgradeWapon` 外层体 → `UserSelect` 分片。
---

# 15. 第九轮（切片 26）：`TMerchant.ClientBuyItem`(323 行) 1:1 + **D36 新偏差登记** ★ 本节优先于 §14

## 15.1 commit

| # | commit | 内容 |
|---|---|---|
| 26 | `788b1456` | `ClientBuyItem`(3367-3688，323 行) 1:1 + 2 个新接缝 + 24 用例；D36 登记 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,110 passed / 0 failed**（main 基线 9,058 + 本车道新增 52）。
越区检查为空（全部在 `Npc/` 与 `NpcObjNpc*` 内）。

## 15.2 覆盖口径（★ 以本节为准）

| 口径 | 切片 18 | **切片 26** |
|---|---|---|
| Covered 例程 / 112 | 64 | **65** |
| Seam 例程 / 112 | 5 | **5** |
| Missing 例程 / 112 | 43 | **42** |

逐行 1:1（含 3 个嵌套过程 `sub_4A0218` 143 + `sub_4A1C84` 12 + 本方法内 2 段重复块）≈ **2,668 / 10,546 = 25.3%**。

## 15.3 原文**两段逐字重复**代码的处理

`ClientBuyItem` 内部有两处**逐字重复**、且与 `ClientSellItem` 同型的块，均抽为私有方法
（分支顺序与字面串逐行一致，**只消除字面重复**）：
| 抽出方法 | 原文位置 | 说明 |
|---|---|---|
| `BuildNextGoodsDisplay(...)` | `:3486-3531` 与 `:3621-3666`（**两段完全相同**，46 行 × 2） | "增加显示下一个物品"：重算下一件的价/库存/子菜单并拼 `sSendText` |
| `ChargeCastleTax(int)` | `:3445-3455` 与 `:3588-3598`（**两段完全相同**） | 城堡税率上账；**与 `ClientSellItem` 的 D33 同型**：管理器分支传 `nUpgradeWeaponPrice` 而非成交价 |

## 15.4 `ClientBuyItem` 的照抄要点与原文缺陷

- **`n1C` 的四个取值**（已逐条断言）：初值 `1`（**完全没有命中时保持 1**）、
  重量门失败 `2`（`:3677 // 004A2639`）、`IsEnoughBag` 失败 `2`（`:3671`）、
  金币不足或 `nPrice <= 0` `3`（`:3674`）。
  → ⚠ 我首版用例把"未命中"写成 `3`，**回读原文才发现那是初值 1**；已改为断言 1。
- **`nItemCount` 在非叠加路径不赋值**：`:3392` 初值 0，只在叠加分支 `:3469/:3562` 赋值 →
  纯非叠加购买时成功包的 `nParam3` 是 **0**（不是 1）。已断言并注释。
- **`sMakerName` 取的是玩家名**（`:3606 UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName`），
  **不是 NPC 名** —— 我首版期望写成 NPC 名，已按原文改正。
- **`StdMode <= 4 / = 42 / = 31` 时 `nStock` 被复用成 `UserItem.MakeIndex`**（`:3514/:3648`，原文如此）——
  展示串里那一格是 MakeIndex 而非库存数。已断言。
- **`CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) }`**：`{ }` 内是被注释掉的条件（`:3421/:3431/:3538`），照抄保留。
- **重量门**：`m_WAbil.MaxWeight` 默认 0 会让**任何正重量**被拒（`n1C = 2`）—— 这是测试踩到的真实坑，已写进用例注释。
- **`nCount` 夹取**：`:3436-3437` 当 `nCount > UserItem.Dura + 1` 时**夹到 `UserItem.Dura + 1`**（原文注释：防刷物品的修正 2019-11-26）。
- **`:3623 UserItem := List20.Items[0]` 不判 nil**（空槽即 AV，照抄）。
- **`:3614 // List20.Delete(II);`** 注释保留（此路径**不**删商品）。

## 15.5 ★★ 新增偏差 **D36**：`AddItemToBag` 之后对 `ItemFrom` 的改动必须**显式同步回背包**

| 项 | 内容 |
|---|---|
| **编号** | **D36**（**D35 的具体后果**，`ClientBuyItem` 特有） |
| **位置** | `src/GXX.M2Server/Npc/ObjNpcMerchantBuy.cs`（`ClientBuyItem` 的"新占一格"路径） |
| **原文行为** | `:3546/:3568/:3582` 先 `PlayObject.AddItemToBag(UserItem)`，**之后** `:3603-3608` 才改 `UserItem.ItemFrom.ItemForm/MakerName/DateTime`。因为 `AddItemToBag` 加的是**指针**，这些改动**自动**反映到背包里那件 |
| **托管行为** | `AddItemToBag` 是**值复制**（D35）→ 入包发生在改动**之前** → 若不做处理，背包里那件的 `ItemFrom` **永远是 `ifUnknow` / 空 MakerName / DateTime=0** |
| **危害等级** | 高：`ItemFrom` 是**物品来源追溯**（谁卖的、何时、什么渠道），错值是**静默**的（不抛异常、不失败，只在 DB/审计里表现为"来源未知"） |
| **处置（已落地）** | 在 `:3608` 之后插入 `PlayObject.SetBagItem(PlayObject.Bag.Count - 1, UserItem);` —— `AddItemToBag` 是 Append 语义，故下标恒为 `Bag.Count - 1`，**三条子分支（全叠/部分叠/非叠）都成立** |
| **回归守卫** | `ClientBuyItem_SetsItemFromToShopBuy_AndSyncsIntoBag_D36`（断言背包里的 `ItemForm == ifShopBuy`、`MakerName == 玩家名`、`DateTime > 0`） |
| **同一族的其它位置（待查）** | `SendAddItem`（`Engine/PlayerSurface/Items.cs`）本身接收**值参**，其调用方若先入包再改物品，同样需要同步 —— 后续 `UpgradeWapon`/`UserSelect` 分片时会逐点复核 |

## 15.6 新增接缝（2 个）

| 接缝 | 精确签名 | 原文出处 |
|---|---|---|
| `OverLapItems` | `Func<TPlayObject, TStdItem, ushort, TUserItem?>` | `ObjBase.pas:1343` 声明 / `:1873` 实现（`ObjPlayer.pas:31435` 另有二参重载）；ObjNpc.pas:3441 |
| `CopyToUserItemFromName` | `delegate bool (string sItemName, ref TUserItem item)` | `UsrEngn.pas:284`；ObjNpc.pas:3553 —— **必须 `ref`**（调用方随后读新分配的 `MakeIndex`，:3555） |

新增常量：`ObjNpcConst.LOG_ItemBuy = 11`（M2Share.pas:98）。

## 15.7 下一轮

按裁定顺序：**`UpgradeWapon` 外层体**(1830-1901, 76 行，`sub_4A0218` 已覆盖) → **`UserSelect`(2087-2900, 814)** 按
`@buy` / `@sell` / `@repair` 分片。
> `UpgradeWapon` 外层体的前置检查项：`m_UseItems[U_WEAPON]`（读写，**D35 同族**：改完需写回）、`GotoLable`（未移植）。
> 若需要新的 Engine 侧成员，我会**先只提出那一个**再动手（沿用 `SetBagItem` 的做法）。
---

# 16. 第十轮（切片 28）：`UpgradeWapon` 外层体 1:1 + **`m_UseItems` 同型缺陷的精确提案** ★ 本节优先于 §15

## 16.1 commit

| # | commit | 内容 |
|---|---|---|
| 28 | `4c2c14cf` | `UpgradeWapon` 外层体(1830-1901，72 行) 1:1 + 5 接缝 + 4 常量 + 20 用例 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,221 passed / 0 failed**（main 基线 9,110 + 本车道 111）。越区检查为空。

## 16.2 覆盖口径

| 口径 | 切片 26 | **切片 28** |
|---|---|---|
| Covered / 112 | 65 | **66** |
| Seam / 112 | 5 | **4** |
| Missing / 112 | 42 | **42** |

逐行 1:1 ≈ **2,740 / 10,546 = 26.0%**。
`1684 UpgradeWapon` 由 **Seam → Covered**（嵌套 `sub_4A0218` + 外层体 1830-1901 现全覆盖）。

## 16.3 前置核实②：`GotoLable` 未移植 → 走 Engine 既有接缝 ✅（已按预期处理）

`PlayerSurfaceNpcSeams.GotoLable(this, User, label, false)`（`NpcSession.cs:44`）。
本过程用到 3 个标签，均为**新声明常量**（原文出处是 **NpcCommon.pas**，不是 M2Share —— 易错点）：
`sNF_Upgradeing='~@upgradenow_ing'`(:75)、`sNF_UpgradeOK='~@upgradenow_ok'`(:77)、`sNF_UpgradeFail='~@upgradenow_fail'`(:79)。

## 16.4 ★★ 前置核实①：`m_UseItems` 是**视图类型** —— 与 `m_ItemList` **同型缺陷**，需裁定

**核实结果（与调度方预期不同，故先报告后处理）**：
| | 原文 | 托管现状 |
|---|---|---|
| 声明 | `ObjBase.pas:882 m_UseItems: THumanUseItems` | `Engine/RecalcChain.cs:101 public TUserItemView?[] m_UseItems` |
| 元素类型 | `THumanUseItems = array[0..MAX_USE_ITEM_COUNT-1] of **TUserItem**`（`Grobal2.pas:4169`，**权威值类型**） | `**TUserItemView**`（只有 `wIndex`/`BtValue`/`CustomProperties`） |

**后果**：`UpgradeWapon` 需要 `m_UseItems[U_WEAPON]` 的 **`MakeIndex`**（:1883 日志）、
整件赋给 `UpgradeInfo.UserItem`（:1880，类型是权威 `TUserItem`）—— **视图类型承载不了**，
`TUserItemView` → `TUserItem` 也**不是**无损转换。
⇒ 这不是"改完写回"就够的，而是**元素类型选错**（正是 §26 对 `m_ItemList` 的那条教训在另一个字段上重现）。

### 调用点全量（**同时搜了 `src` 与 `tests`**，台账 §28.3）：共 **33 处 / 8 个文件**

| 文件 | 处数 | 归属 |
|---|---|---|
| `tests/.../PlayerSurfaceItemsTests.cs` | 18 | **本车道（已特批）** |
| `tests/.../RecalcChainTests.cs` | 6 | p6-m2-playersurface |
| `Engine/RecalcChain.cs` | 3 | **无人认领**（不在 `Engine/PlayerSurface/**` 内） |
| `src/GXX.M2Server/StruckSettlementCore.cs` | 2 | 会话 A 常驻区 |
| `src/GXX.M2Server/CopyMonActThinkCopyCore.cs` | 1 | 会话 A 常驻区 |
| `tests/.../CopyMonActThinkCopyCoreTests.cs` | 1 | 其它车道 |
| `Engine/PlayerSurface/TCreature.PlayerSurface.Base.cs` | 1 | p6-m2-playersurface |
| `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs` | 1 | **本车道（已特批）** |

⇒ **要在本车道内完成该类型纠正是不可能的**（需动 4 个他人源文件 + 2 个他人测试文件）。

### 本轮采取的处置（零跨文件，已落地且可逆）★ 请裁定

**已做**：不动 Engine，在 Npc 侧用**两个按权威侧定名的接缝**表达"读写武器格"：
- `NpcSeams.GetUseItemsWeapon`（**读**，此前已存在，`Func<TPlayObject, TUserItem>`）
- `NpcSeams.SetUseItemsWeapon`（**写回**，本轮新增，`Action<TPlayObject, TUserItem>`）

于是 `UpgradeWapon` 的 :1880/:1886 得以 1:1 落地（**D35 契约：取出 → 改 `wIndex := 0` → 写回**），
代价是多留 **1 个**接缝。

**请二选一**：
1. **（推荐）扩展方案 A 到 `m_UseItems`**：把 `RecalcChain.cs:101` 的元素类型改为 **`TUserItem?[]`**（与 `m_ItemList` 完全同型），
   `GetAccessory.Apply` 调用处按需现造视图（`TCreature.PlayerSurface.Items.cs` 已有 `ToItemView`）。
   需要一并适配上表 **8 个文件 / 33 处**（其中 2 个源文件 + 2 个测试文件不在我分区）。
   → 收益：`SetUseItemsWeapon` 接缝**可以删掉**，改为直读直写；`m_UseItems` 与 `m_ItemList` 口径统一。
   → 需要你扩我的分区（或另派一个"物品容器口径统一"任务）。
2. **保留现状**：`m_UseItems` 继续是视图数组，由**集成方**在 wiring 时实现
   `GetUseItemsWeapon`/`SetUseItemsWeapon` 两个接缝的视图↔权威转换（有损：`MakeIndex` 拿不到，
   除非另找来源）。**不推荐** —— 会重演 §26 的"双容器/双表示"问题。

> 无论选哪个，**本轮交付不阻塞**：`UpgradeWapon` 已 1:1 落地并通过 20 例，接口是权威侧语义。

## 16.5 ★ "先入包、后改物品"这一族的**逐点复核结果**（D36 同族普查）

按调度方要求，我对 `UpgradeWapon` 外层体逐点核对了这一族：

| 位置 | 原文动作 | 是否 D36 同族 | 处置 |
|---|---|---|---|
| `:1878-1880` | `UpgradeInfo.UserItem := User.m_UseItems[U_WEAPON]`（**存进记录**） | 否（只是读取+保存） | 直接赋值 |
| `:1885-1886` | `SendDelItem(武器)` → `wIndex := 0`（**清空槽位**） | **是（D35 家族）** | 取出→改→`SetUseItemsWeapon` 写回 ✅ |
| `:1893` | `m_UpgradeWeaponList.Add(UpgradeInfo)` | 否（`TUpgradeInfo` 是**引用类型**，无 D35 问题） | 直接 Add |

**结论**：`UpgradeWapon` 内**没有**"先入包（`AddItemToBag`）后改物品"的调用点 —— 它不往背包加东西，
只**摘除**武器并清空槽位。故 **D36 在此不适用**，唯一落点是 :1886 的槽位写回（已处理）。
> 对照：`ClientBuyItem` 的 D36 落点是 `AddItemToBag` **之后**改 `ItemFrom`（§15.5）。
> `UserSelect`(2087-2900) 分片时会继续做同族普查。

## 16.6 本轮新增接缝 / 常量（5 接缝 + 4 常量）

| 名称 | 精确签名 | 原文出处 |
|---|---|---|
| `SetUseItemsWeapon` | `Action<TPlayObject, TUserItem>` | `ObjNpc.pas:1886`（见 §16.4） |
| `g_sCannotUpgradeWeapon` | `string`（默认 `"你的武器[%Item]不允许升级"`） | `M2Share.pas:8165`（`:21346/:21352` StringConf 覆盖） |
| `g_boGameLogGold` | `bool` | `M2Share.pas:3748`；ObjNpc.pas:1862 |
| `SysMsgFB` | `Action<TCreature, string, int, int, TMsgType>` | `ObjPlayer.pas:**1286**` `SysMsg(sMsg; FColor, BColor: Integer; MsgType)` —— **第二个重载**（第一个见既有 `SysMsg`） |
| 常量 `LOG_ItemUpgrade` | `= 14` | `M2Share.pas:101` |
| 常量 `sNF_Upgradeing/OK/Fail` | 见 §16.3 | `NpcCommon.pas:75/77/79` |

## 16.7 `UpgradeWapon` 的照抄要点与原文缺陷

- **:1858 的 `Exit` 跳过出口标签**：禁升级分支 `Exit` 得**早**，所以 :1897-1900 的 `GotoLable` **一次都不执行**
  —— 已用 `Assert.Empty(_labels)` 锁死（"没有跳转"本身就是原文行为，不是遗漏）。
- **规则号是 17**（不是 4/8）：已用"规则 ≠17 不拦"与"=17 拦住"两条差异断言锁死。
- **:1854 的第二条件是 `Length(StdItem.Name) > 0`**，与规则判断**与**在一起 → 已单独用例（空名不拦）。
- **:1850 用 `>=`**（恰好够钱可以升）→ 已用"499 拒 / 500 过"边界对锁死。
- **:1856 的 `%Item` 带百分号**（不是 `%s`）→ 断言替换结果为 `你的武器[屠龙]不允许升级`。
- **:1864 日志的 `(Data1, Data2) = (扣费后余额, OldGold)`**（**先存旧值**再扣）→ 断言 `.../9500/10000/扣费:500`。
- **:1889 先 `RecalcAbilitys` 后 `FeatureChanged` 再 `SendMsg(RM_ABILITY)`**（顺序照抄）。
- ⚠ **`sub_4A0218` 有副作用**：它**消耗背包里的黑铁矿**（并因此额外发包/写 `LOG_ItemDisappear` 日志）。
  两个用例曾因此误判为失败（"期望 1 条消息、实得 2 条"）—— 已在用例注释里写明，
  期望值改为"**包含**目标条目"；`Sub4A0218OutputsAreStored` 改为在**另一条等价玩家**上先算期望值。
- ⚠ `UpgradeWapon` 的**税收块传的就是 `nUpgradeWeaponPrice`**，此处**不是** D33（D33 是 `ClientBuyItem`/`ClientSellItem` 的
  管理器分支"传错了量"；这里两个分支本来就该传它）—— 已在用例注释里区分，避免后人误"修"。

## 16.8 下一轮

**`UserSelect`(2087-2900, 814 行)** 按 `@buy` / `@sell` / `@repair` **分片**，每片独立提交 + 独立门禁。
首片建议 `@repair`（最小），随后 `@sell`、`@buy`。
> `UserSelect` 的依赖普查会在首片开始前完成（含 D36 同族、"先入包后改物品"、`m_UseItems` 读写点）。
---

# 17. 第十一轮（切片 30）：`m_UseItems` **口径统一**（两容器现已同口径）★ 本节优先于 §16

## 17.1 commit

| # | commit | 内容 |
|---|---|---|
| 30 | `f1d5d0a2` | `m_UseItems` 元素类型 `TUserItemView?[]` → **`TUserItem?[]`**；8 文件一次性适配；**删除 2 个替身接缝**；`ToItemView` 上移 `TCreature` |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,264 passed / 0 failed**（合并 main 后复跑）。
越区检查为空。**动手前后各 `merge main` 一次，均无冲突**（按调度方要求）。

## 17.2 结果：两个容器现已**完全同口径**

| 容器 | 原文 | 统一前（托管） | **统一后（托管）** |
|---|---|---|---|
| `m_ItemList`（背包） | `ObjBase.pas:322 TList` of `pTUserItem` | `List<TUserItemView>`（且零调用方/双容器） | **`List<TUserItem?>`**（§13，方案 A 第①步） |
| `m_UseItems`（装备槽） | `ObjBase.pas:882 THumanUseItems` = `array[..] of **TUserItem**` | **`TUserItemView?[]`**（视图类型选错） | **`TUserItem?[]`** |

⇒ `GXX.Core.Protocol.TUserItem` 现为**两个容器的唯一存储与权威**；
`Engine.TUserItemView` 在**两处**都退化为"能力聚合用的轻量视图"，由调用处按需现造（`TCreature.ToItemView`）。

## 17.3 一次性适配的全部改动点（8 文件，编译一次通过）

| 文件 | 改动 |
|---|---|
| `Engine/RecalcChain.cs` | ① 声明 `m_UseItems` → `TUserItem?[]`（含口径统一说明）；② `RecalcAbilitys` 循环：`userItem.Value.wIndex`，并**现造视图** `TUserItemView view = TCreature.ToItemView(userItem.Value);` 供 `GetItemAddValue`/`GetAccessory.Apply` |
| `Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs` | `ToItemView` **由 `TPlayObject` 上移到 `TCreature`**（`RecalcChain` 以 `TCreature.ToItemView` 调用，必须在那一层可见）；另更新两处过时注释 |
| `Npc/ObjNpcMerchantUpgrade.cs` | `UpgradeWapon` 的武器格读写改**直读直写**：`User.m_UseItems[UseSlots.U_WEAPON] ?? default` / `= Weapon` |
| `Npc/ObjNpcMerchant.cs` | `$USERWEAPON`(`GetVariableText`) 的武器格读取改直读 |
| `Npc/ObjNpcSeams.cs` | **删除 `GetUseItemsWeapon` 与 `SetUseItemsWeapon` 两个替身接缝**（声明 + `ResetDefaults` 各 2 行）→ 留注释说明为何删 |
| `tests/RecalcChainTests.cs` | 6 处 `new TUserItemView { wIndex = N }` → `new TUserItem { wIndex = N }`（`replace_all` 一次） |
| `tests/PlayerSurfaceItemsTests.cs` | 2 个 `UseItems_*` 用例：① 反射守卫类型断言 `typeof(TUserItemView[])` → **`typeof(TUserItem?[])`**；② 把"就地改 `wIndex`"改写为 **D35 契约**（取出 → 改 → 写回），因为值类型元素**不能**原地改（原地写**不可编译**） |
| `tests/NpcObjNpcUpgradeWaponTests.cs` / `NpcObjNpcMerchant2Tests.cs` | 去掉接缝接线，改为直填 `p.m_UseItems[UseSlots.U_WEAPON]`；武器格断言改为直接读槽位 |

### ★ 两个文件**零改动**（正是"最小化改动"要求的达成）
| 文件 | 为何零改动 |
|---|---|
| `src/GXX.M2Server/StruckSettlementCore.cs` | 仅 **2 处字符串字面量/注释**提到 `m_UseItems`（`:328` 中文标签、`:474` 注释掉的 Delphi 行）—— 无需类型适配 |
| `src/GXX.M2Server/CopyMonActThinkCopyCore.cs` | 仅 **1 处**（`:461` 注释掉的 Delphi 行 `// m_UseItems := TSmartObject(Source).m_UseItems;`） |
| （同族）`tests/.../CopyMonActThinkCopyCoreTests.cs`、`Engine/PlayerSurface/TCreature.PlayerSurface.Base.cs` | 同上，均为注释/文档 |

⇒ **顺序会话的常驻区（`StruckSettlementCore.cs`/`CopyMonActThinkCopyCore.cs`）实际零改动**，
因此**不存在冲突风险**（这也是"动手前后各 merge 一次都无冲突"的原因）。

## 17.4 等价性论证（为何"现造视图"不改变行为）

`RecalcBonus.GetItemAddValue(TUserItemView userItem, TStdItemView std)` 的**全部写入**都是 `std.X = ...`
（已逐行核对：`RecalcBonus.cs:56-63` 共 8 处，全是 `std.DC1/DC2/MC1/MC2/SC1/SC2/AC1/AC2`），**从不写物品本身**。
⇒ 传入"由权威记录现造的一次性视图"与原文"就地改权威记录"在**本循环内**行为一致；
且 `GetAccessory.Apply` 随后消费**同一个视图**，故聚合结果不变。
（`:1889` 的 `RecalcAbilitys` 在 `UpgradeWapon` 里被调用后，装备槽内容本身未变，仅槽位 `wIndex` 被清空 —— 两者互不影响。）

`GetAccessory.Apply` 的入参**保持 `TUserItemView`**（视图 ≠ 存储），与 `m_ItemList` 完全同构 ✅。

## 17.5 去接缝化的净收益

| | 统一前 | **统一后** |
|---|---|---|
| `m_UseItems` 读写接缝 | `GetUseItemsWeapon` + `SetUseItemsWeapon`（2 个） | **0（已删除）** |
| 视图↔权威转换责任 | 落在**集成方 wiring**（且有损：拿不到 `MakeIndex`） | 落在 `TCreature.ToItemView`（**唯一一处**，有损点与 `SendAddItem` 共用、已登记） |
| `UpgradeWapon` 的 :1880/:1886 | 经接缝（权威侧语义，但需宿主实现） | **直读直写**，1:1 无中间层 |

这正是调度方强调的"**正式归属落地后去掉替身**"。**本轮净减 2 个接缝。**

## 17.6 D35 的适用范围**扩大**（登记更新）

原 D35 只覆盖 `m_ItemList`（§14.3）。本轮后 **D35 同时覆盖 `m_UseItems`**：
两者元素都是可空**值类型** → "取出 → 改 → **写回槽位**"的调用方契约**对两个容器都成立**。
- `m_ItemList` → 写回入口 `TCreature.SetBagItem(int, TUserItem?)`
- `m_UseItems` → 直接写 `arr[i] = item;`（数组索引器**可**赋值；`List<T>` 索引器不可 `ref` 但**可**赋值，两者都行）

> ⚠ 之前 `m_UseItems` 是**引用**类型数组时，`p.m_UseItems[i]!.wIndex = 0` 能编译 —— 现在**编译不过**了，
> 这正是 D35 从"隐性风险"变成"编译期强制"的地方（`PlayerSurfaceItemsTests` 的两个用例已按新契约改写）。

## 17.7 ⚠ 分区表授权路径有笔误（请修正）

调度方本轮把两个文件写成 `GXX.CSharp/src/GXX.M2Server/**Engine**/...`，但**真实路径在 `src/GXX.M2Server/` 根下**：
```
分区表写的 .../Engine/StruckSettlementCore.cs        -> Test-Path = False   ← 不存在
分区表写的 .../Engine/CopyMonActThinkCopyCore.cs     -> Test-Path = False   ← 不存在
真实路径   src/GXX.M2Server/StruckSettlementCore.cs   -> True
真实路径   src/GXX.M2Server/CopyMonActThinkCopyCore.cs -> True
```
本车道按**具名直接授权**执行，且这两个文件**恰好零改动**（见 §17.3），故实际未触碰；
但**若将来真要改它们，`verify-lanes.ps1` 会因路径不匹配而报 OUT-OF-ZONE**。请把分区表两条改为实际路径。

## 17.8 下一轮

按调度方顺序：**`UserSelect`(2087-2900, 814 行)** 按 `@buy` / `@sell` / `@repair` **分片**，
每片独立提交 + 独立门禁；**首片建议 `@repair`**（最小）。开工前做依赖普查（D36 同族 / `m_UseItems` 读写点 —— 后者现已直读直写）。
---

# 18. 第十二轮（切片 32）：`UserSelect` 首片 `@repair` + **分片前提的实测纠正** ★ 本节优先于 §17

## 18.1 commit

| # | commit | 内容 |
|---|---|---|
| 32 | `edae49bc` | `@repair` 分片：`SuperRepairItem`(2089-2092) + `RepairItem`(2262-2265) + 两条 `case` 分支(2702-2706/2737-2741) 1:1 + 1 接缝 + 4 常量 + 14 用例 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,295 passed / 0 failed**。越区检查为空。

## 18.2 ★★ 实测纠正：三条分片**并不互相独立** —— 它们共享"派发基础设施"

`UserSelect`(2087-2899) 的结构实测为：
```
2089-2546   20 个**嵌套过程**（SuperRepairItem / BuyItem / SellItem / RepairItem / … ）
2547-2696   派发前置：解析 sData → sLabel / sMsg，以及 nCode 赋值
2691-2694   nIndex := g_NpcProcessCommand.IndexOf(sLabel); nIndex := Integer(...Objects[nIndex]);
2696-2899   case nIndex of  →  30+ 个 nNF_* 分支
```
**结论**：`@repair` / `@sell` / `@buy` 三个分片各自的"内容"（嵌套过程 + `case` 分支）确实很小，
但它们**都依赖同一套派发基础设施**：`g_NpcProcessCommand`（标签→命令号表，`NpcCommon.pas:1906-1960`）、
`nNF_*` 枚举（`NpcCommon.pas:10-88`）、以及 2547-2696 的标签/参数解析。
⇒ 调度方"首片 `@repair` 最小"的判断，按**嵌套过程行数**成立，
但按**可独立完成的闭环**不成立 —— 任何一片都无法在派发体移植前真正"接上"。

## 18.3 本片实际交付（1:1）

| 原文 | 内容 | 托管落点 |
|---|---|---|
| 2089-2092 | `procedure SuperRepairItem(User)`：`User.SendMsg(Self, RM_SENDUSERSREPAIR, 0, NativeInt(Self), 0, 0, '')` | `TMerchant.SuperRepairItem` |
| 2262-2265 | `procedure RepairItem(User)`：`User.SendMsg(Self, RM_SENDUSERREPAIR, 0, NativeInt(Self), 0, 0, '')` | `TMerchant.RepairItem` |
| 2702-2706 | `case nNF_SuperRepair: if m_boS_repair then SuperRepairItem(PlayObject);` | `UserSelectRepairCommands` 的 `case` |
| 2737-2741 | `case nNF_Repair: if m_boRepair then RepairItem(PlayObject);` | 同上 |

**新增接缝 1 个**：`NpcSeams.NpcProcessCommandIndexOf` : `Func<string, int>`
（原文 `g_NpcProcessCommand.IndexOf(sLabel)` 的等价物；返回 `-1` = "标签不在表中"，
对应原文 2692 的 `nIndex >= 0` 为假）。**新增常量 4 个**：
`sNF_Repair='@repair'`/`sNF_RepairOK='~@repair'`（`NpcCommon.pas:33/35`）、
`nNF_SuperRepair=9`/`nNF_Repair=12`（`NpcCommon.pas:26/32`）。

### ★ 登记口径（**避免过度声称**）
`UserSelect` 在登记表里**仍保持 `Missing`** —— 派发体（含 2547-2696 的解析与其余 30+ 个 `nNF_*` 分支）
**未移植**。本片以"分片进度"形式记录，**不**把 `UserSelect` 标为 Covered。

### 写入 `UserSelectRepairCommands` 的理由（以及为什么它不是"新 API"）
派发体未移植，但本片两条分支是**可独立验证的完整语义单元**；该方法的**名字直接标出**它对应
原文 `UserSelect` 内 `@repair` 的那两条 `case`，等派发体移植时**原样搬进 `case` 后本方法即删除**。
返回 `bool` 以区分"命中本片两条分支"与"该命令号不属 `@repair` 族" —— **刻意不做静默兜底**
（呼应调度方第 3 条提醒：静默兜底会把"没实现"伪装成"没命中"）。

## 18.4 20 个嵌套过程清单（供后续分片排期）

| 原文行 | 嵌套过程 | 归属分片建议 |
|---|---|---|
| 2089-2092 | `SuperRepairItem` | **@repair（本片 ✅）** |
| 2262-2265 | `RepairItem` | **@repair（本片 ✅）** |
| 2094-2176 | `BuyItem`（83 行，含 `label RefBuy` + `goto`） | `@buy` |
| 2257-2260 | `SellItem` | `@sell` |
| 2177-2205 | `RemoteMsg` | `$RMST`（离线消息族） |
| 2206-2211 | `AutoGetExp` | 离线挂机 |
| 2212-2256 | `DealGold` | 交易金币 |
| 2267-2270 | `ArmRemoveStoneItem` | 卸装 |
| 2272-2306 | `MakeDurg` | 制药 |
| 2307-2310 | `ItemPrices` | 询价 |
| 2311-2315 | `Storage` | 仓库 |
| 2316-2320 | `GetBack` | 取回 |
| 2321-2325 | `BigStorage` / 2326-2331 `BigGetBack` | 大仓库 |
| 2332-2340 | `GetPreviousPage` / 2341-2346 `GetNextPage` | 翻页 |
| 2347-2407 | `MakeHeroName` | 英雄命名 |
| 2408-2460 | `MakeDeputyHeroName` | 副将命名 |
| 2461-2485 | `InPutInteger` / 2486-2507 `InPutString` | 输入 |
| 2508-2546 | `PlayDrink` | 斗酒 |

## 18.5 依赖普查结果（按调度方要求，开工前已做）

| 普查项 | 本片结果 |
|---|---|
| **D36 同族**（"先入包、后改物品"） | **本片无** —— 两条分支只发包 `RM_SENDUSERSREPAIR`/`RM_SENDUSERREPAIR`，不碰背包/物品 |
| **`m_UseItems` 读写点** | **本片无** |
| 查调用点（`src` + `tests`） | `SuperRepairItem`/`RepairItem` 在 `src`/`tests` 中**此前零引用**（原文里只在 `UserSelect` 的 `case` 内被调用）—— 已按原文接上，**不是**"零调用方的错实现" |
| `params` 重载族 | 本片测试辅助方法均为固定参数，**无 `params`** |
| `THumData` 巨型结构值复制 | 本片不涉及 |

## 18.6 下一轮的建议（需裁定）

三条路，请择一：
1. **先把派发基础设施做成一个独立提交**：移植 `g_NpcProcessCommand`（标签→命令号表，
   `NpcCommon.pas:1906-1960`，含 30+ 条注册）+ `nNF_*` 全部常量 + 2547-2696 的标签/参数解析。
   做完后，三条分片就退化为"填 `case` 分支"，**每片都会很小**。
   → 代价：该基础设施**属 NpcCommon 面**，不在 `Npc.*` 归属内，需要新的分区授权（或另派车道）。
2. **由另一条车道先移植 `g_NpcProcessCommand`/`nNF_*`**，我再按原计划分片。
3. **继续以"分片 + 单接缝"推进**：每片照本片做法落地（嵌套过程 + `case` 分支 + 用
   `NpcProcessCommandIndexOf` 接缝），最后一次性把接缝换成真表。缺点是期间 `UserSelect` 无法端到端验证。

**我倾向 1 或 2**（先补基础设施，避免三片各留半截）。
---

# 19. 第十三轮（切片 34）：NPC **派发基础设施** 1:1 落地 ★ 本节优先于 §18

## 19.1 commit

| # | commit | 内容 |
|---|---|---|
| 34 | `001e26a1` | `Npc/NpcProcessCommand.cs`：`NpcCommon.pas` 的 **68 组 `nNF_*`/`sNF_*` 常量** + **`g_NpcProcessCommand` 表与 68 条注册**；**删 `NpcProcessCommandIndexOf` 替身接缝**；常量单一来源收敛；+27 用例 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,308 passed / 0 failed**。越区检查为空。

## 19.2 落地内容（逐条标注原文行）

| 原文 | 托管 |
|---|---|
| `NpcCommon.pas:10-144` 的 68 个 `nNF_*`（值 **1..68**） | `NpcProcessCmd.nNF_*`（每条都带 `/// nNF_X = N（NpcCommon.pas:行）`） |
| `NpcCommon.pas:33-…` 的 68 个 `sNF_*` 标签 | `NpcProcessCmd.sNF_*` |
| `NpcCommon.pas:1899-1962` 的 68 条 `g_NpcProcessCommand.AddObject(sLabel, TObject(nNF))` | `NpcProcessCmd.g_NpcProcessCommand`：`AddObject` / `IndexOf` / `GetCommand` / `Count` / `Order` / `Init` / `Reset`，静态构造调 `Init()` |

**查表语义照抄**：`IndexOf` 未命中返回 `-1`（对应原文 `nIndex >= 0` 为假）；
`GetCommand` 合并原文 2691-2694 的两步（`IndexOf` + `Objects[nIndex]`），未命中同样 `-1`。
⚠ `TStringList.IndexOf` 默认 **`CaseSensitive = False`** → 用 `OrdinalIgnoreCase`（已加差异用例）。

## 19.3 去替身：`NpcProcessCommandIndexOf` 接缝**已删除**

上一轮为"派发基础设施未移植"加的单一接缝，在本轮基础设施落地后**按"正式归属落地后去掉替身"
删除** —— 调用方直接 `NpcProcessCmd.g_NpcProcessCommand.GetCommand(sLabel)`。
**本轮净减 1 个接缝**（累计：§13 删 8 个、§14 删 1 个（`Click`）、§17 删 2 个、本轮删 1 个）。

## 19.4 常量**单一来源**收敛（消除重复声明）

上一轮曾把 `sNF_Repair/RepairOK`、`nNF_SuperRepair/Repair`（以及更早的 `sNF_Upgradeing/OK/Fail`）
放在 `ObjNpcConst`；本轮基础设施落地后**这 7 条已移出** `ObjNpcConst`，
统一声明在 `NpcProcessCmd`（原文同属 `NpcCommon.pas`），并把 4 个文件里的引用同步更新
（`ObjNpcMerchantUpgrade.cs`、`ObjNpcUserSelect.cs`、`NpcObjNpcUpgradeWaponTests.cs`、
`NpcObjNpcUserSelectRepairTests.cs`）。`ObjNpcConst` 中留了**注释指向新位置**，不留别名。

## 19.5 登记口径（继续克制，未变）

**`UserSelect` 在登记表里仍保持 `Missing`** —— 派发体（2547-2899）与 2547-2696 的
**标签/参数解析段**仍未移植。本轮只落"基础设施"（常量 + 表），**不**把 `UserSelect` 标 Covered。
⇒ **基础设施提交尚未完全收口**：原任务书的第 3 项"`2547-2696` 解析"**仍是下一轮内容**
（它依赖 `GetValidStr3` 一族字符串解析与 `nCode` 语义，是独立的一块）。**已在 §19.6 列出**。

## 19.6 下一轮的唯一待办 + 之后的排期

1. **（下一轮）`2547-2696` 解析段**：`sData` → `sLabel`/`sMsg` 的提取 + `nCode` 赋值。
   依赖 `GetValidStr3` 一族（`HUtil32`，部分已移植）——开工前会先做依赖普查并**只申请缺的那几个**。
2. 然后三条分片退化为"填 `case`"：`@repair`（嵌套过程已成 ✅，只需把 `UserSelectRepairCommands`
   的两个 `case` 搬进真 `switch` 并删除该临时方法）→ `@sell` → `@buy`。
3. `UserSelectRepairCommands` 的**删除条件**已写进其 XML 注释（可执行判据：
   `UserSelect` 由 `Missing` → `Covered`，且该方法在 `src`/`tests` 中零引用，`grep` 可验）。

## 19.7 依赖普查（本轮，开工前）

| 项 | 结果 |
|---|---|
| D36 同族 / `m_UseItems` 读写点 | **均无**（本文件只落常量与查表，不碰背包/物品） |
| 查调用点（`src` + `tests`） | `g_NpcProcessCommand` 此前**零引用**（全新基础设施）；`nNF_*`/`sNF_*` 的 7 条旧引用已随 §19.4 收敛 |
| `params` 重载族 | 无 |
| 巨型结构值复制 | 无 |
---

# 20. 第十四轮：`2547-2696` 解析段的**依赖普查**结果与两处前置（本轮**未落代码**）★ 本节优先于 §19

> 本轮按调度方要求"**开工前先做依赖普查，缺的 `GetValidStr3` 一族只申请那几个**"。
> 普查发现**两个需裁定的前置**，故**先报告、未动代码**（工作树干净，最后一次提交仍是全绿的 `4ceb74b0`）。

## 20.1 好消息：`GetValidStr3_Ex` **已存在，无需申请**

| 依赖 | 现状 |
|---|---|
| `GetValidStr3_Ex` | ✅ **已存在**：`GXX.Core/Util/HUtil32.cs:303` `public static string GetValidStr3_Ex(string str, ref string dest, char divider)`（Client 车道已在用，见 `DxImageButtonEx.cs:431`） |
| `Pos` / `Copy` / `Length` | ✅ `DelphiRTL.Pos` / `Copy` 已存在 |
| `m_boCastle` | ✅ `ObjNpcClasses.cs:109` |
| `m_sNpcSelectItemName` | ✅ `TPlayObject.PlayerSurface.ScriptFields.cs:9`（调度方第七轮补的 3 字段之一） |
| `m_sScriptLable` / `m_sInputData` / `m_boMessageBox` / `m_sYesLable` / `m_sNoLable` | ✅ `TPlayObject.PlayerSurface.NpcSession.cs:10` |
| `LableIsCanJmp` | ✅ `TPlayObject.PlayerSurface.NpcSession.cs:346` |
| `AllowSelect` | ✅ 本车道已覆盖（`ObjNpcLabels.cs`） |
| `nMaxInputStringLen` | ✅ `Grobal2.Types5.cs:321`（`g_Config` 面） |

⇒ **`GetValidStr3_Ex` 一族不需要申请**（这是本次普查最直接的收益）。

## 20.2 ★ 前置 A（需裁定）：`TMerchant.UserSelect` 的 `inherited`(2527) 指向**未移植的基类**

原文：
```
2087  procedure TMerchant.UserSelect(PlayObject: TPlayObject; sData: string);
2527    inherited;                                  ← 调基类
9807  procedure TNormNpc.UserSelect(PlayObject: TPlayObject; sData: string);   ← 基类，**本车道未移植**
```
托管侧现状：**`TNormNpc.UserSelect` 连虚外壳都没有**（`grep 'void UserSelect('` 在 `src`/`tests` 全仓**零命中**）。

⇒ 这正是台账那条规程的适用场景：**「基类方法 + 子类 `inherited` ⇒ 托管侧必须落 `public virtual` 外壳 + 接缝」**。
三条分片的派发体（`@repair`/`@sell`/`@buy`）**都坐在这个未移植的基类之上** —— 所以：
- **要么**先落 `TNormNpc.UserSelect` 的**虚外壳 + 接缝**（最小：外壳转发接缝，基类实体 9807-… 另行安排）；
- **要么**把基类实体一并移植（9807 起，长度未测，可能很大）。

**请裁定走哪条**（我建议前者：先落虚外壳，才能让 `inherited` 语义成立并让 `TMerchant.UserSelect` 可编译地往下写）。
⚠ 该虚外壳位于 `Npc/**`（我分区内），但它**是覆写链的基类**，`TGuildOfficial`/`TCastleOfficial` 等是否有 `UserSelect` 覆写需一并核查 —— 我会在动手前查清并报告。

## 20.3 ★ 前置 B（需裁定）：城堡 `m_boUnderWar` 未移植

原文 2533：`if not m_boCastle or not ((m_Castle <> nil) and TUserCastle(m_Castle).m_boUnderWar) and (PlayObject <> nil) then`
| 依赖 | 现状 |
|---|---|
| `m_boCastle` | ✅ |
| `m_Castle` | ⚠ **托管侧无此字段** —— 本车道一直用接缝 `NpcSeams.GetNpcCastle(this)`（`ObjNpcMerchant.cs:164/546`、`ObjNpcGuildCastle.cs:131`、`ObjNpcMerchantBuy.cs:457`、`ObjNpcMerchantUpgrade.cs:103`） |
| `TUserCastle.m_boUnderWar` | ❌ **未移植**（`ArcherGuardCore.cs:24`、`CanWalkCore.cs:80` 只有注释提到它） |

⇒ 需要 **1 个新的读取面**。两个选择：
1. **新增接缝** `NpcSeams.GetCastleUnderWar` : `Func<object, bool>`（最小、零跨文件）；
2. 或申请在 `Engine/Castle.cs` 的 `TUserCastle` 上加 `m_boUnderWar` 字段（更"正式归属"，但要动他人文件 + 该字段在原文里的赋值点也要一并处理）。

**请裁定**（我建议 1，代价最小且与既有 `GetNpcCastle` 同族）。

## 20.4 其它已确认的易抄错点（下一轮实现时逐条处理）

| 原文 | 风险 | 托管写法 |
|---|---|---|
| 2529 `if not (ClassNameIs(TMerchant.ClassName)) then Exit;` | `ClassNameIs` 是**精确类名**比较，**不是** `is`/派生判定 | `if (GetType() != typeof(TMerchant)) return;` |
| 2527 `inherited;` | 见 §20.2 | 虚外壳 + 接缝 |
| 2538 `sMsg := GetValidStr3_Ex(sData, sLabel, #13);` | **原地修改** `sLabel`（`ref`）+ 返回**剩余串**；`#13` 是分隔符 | 照抄 `ref` 语义，不改成返回值风格 |
| 2540-2545 `if (Length(sLabel)>=2) and (sLabel[2]='@') and (sLabel[Length(sLabel)]=')')` → `nPos := Pos('(',sLabel)` → 截断 | 三重条件 + **`Pos('(')` 找不到时返回 0** | `DelphiRTL.Pos("(", sLabel) > 0` |
| 2531-2533/2892-2897 `try … except on E: Exception do MainOutMessage(Format(sExceptionMsg,[sData,nCode]))` | `nCode` 是**分段进度标记**（0..21），异常里要报出来 | 照抄 `try/catch` + `nCode` |
| 2535 `(sData <> '') and (sData[1] = '@')` | **`sData[1]` 是 1-based**，且已先判空 | 逐条对照 |

## 20.5 调度方提醒的字符串坑（已记入本轮约束）

- `GetValidStr3_Ex` 的**空分隔符 / 连续分隔符 / 首尾分隔符**：切分处将写**差异断言**（空串、仅分隔符、连续分隔符、首尾分隔符）。
- **不写依赖 `Pos("")` 能命中的断言** —— 台账记录 Delphi `Pos('')` 语义**至今未修**，托管侧行为不可依赖。

## 20.6 本轮状态（诚实登记）

- **本轮未落任何代码**：普查发现 §20.2/§20.3 两处前置需裁定，故**停下报告**（与第十三轮同做法）。
- 工作树**干净**，最后一次提交 `4ceb74b0` **全绿**（9,308 例），**无** `WIP` 提交。
- `UserSelect` 登记**继续 `Missing`**（未变）。
---

# 21. 第十五轮（切片 37）：`TNormNpc.UserSelect` 基类落点 + **对裁定前提的修正** + 偏差 D37 ★ 本节优先于 §20

## 21.1 commit

| # | commit | 内容 |
|---|---|---|
| 37 | `8a9dc8e3` | `TNormNpc.UserSelect`(9807-9835) **真实现**（虚分派链基类落点）+ 新接缝 `GetCastleUnderWar`（偏差 **D37**）+ 17 用例 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,325 passed / 0 failed**。越区检查为空。

## 21.2 ★★ 对裁定 (1) 前提的修正：基类实体**实测只有 30 行**，故落**真实现**而非外壳

裁定 (1) 选"只落虚外壳 + 接缝"，前提写的是"**基类实体（9807 起）长度未测**"。动手前先测：

| 项 | 实测 |
|---|---|
| `TNormNpc.UserSelect` 实体长度 | **9807-9835 = 30 行** |
| 依赖 | **全部已就位**：`m_nScriptGotoCount`（第七轮补的字段）、`HUtil32.GetValidStr3_Ex`（`HUtil32.cs:303`）、`GotoLable` 接缝、`m_sScriptCurrLable`/`m_sScriptGoBackLable`、`NpcProcessCmd.sNF_Back` |
| 需要的新接缝 | **0 个** |

⇒ **直接落真实现**，比"外壳 + 一次性接缝"更省：**少一个必须日后删除的接缝**，且**多收口一条登记**（`9807` Missing → **Covered**）。
> 若调度方仍偏好外壳形态：删除实体体、改为转发接缝即可，**两处调用点不变**（已在代码注释写明）。

## 21.3 裁定要求的**动手前核查**：`UserSelect` 覆写链共 **4 处**（已全仓 `.pas` 搜过）

| 原文行 | 形态 | 属主 |
|---|---|---|
| `:305` | `procedure UserSelect(...); **virtual**;` | **`TNormNpc`**（虚声明） |
| `:411` | `override;`（声明区） | `TMerchant` |
| `:444` | `override; // FFEA`（声明区） | `TGuildOfficial` |
| `:481` | `override; // FFEA`（声明区） | `TCastleOfficial` |
| `:9807` | 实现体 | **`TNormNpc`** ← 本轮落地 |
| `:1186` | 实现体 | `TCastleOfficial` |
| `:2087` | 实现体 | `TMerchant` |
| `:10101` | 实现体 | `TGuildOfficial` |

⇒ 覆写者是 **`TMerchant`/`TGuildOfficial`/`TCastleOfficial` 三个**（声明区 411/444/481 与实现体一一对应，**无第四者**）。
托管侧这三者都还存在且都未移植 `UserSelect` —— 它们日后落 `override` 时 `inherited` 会落到本轮这个基类体上 ✅（**虚链完整**）。

**已验证**：`UserSelect_IsVirtual`（反射 `IsVirtual`）；`UserSelect_IsOverridableViaBaseCall`
（用派生 `ProbeNpc` 覆写并调 `base`，断言**基类体真的被执行**）—— 直接针对"**只写外壳不接 `base` = 完全无效果**"那条实测教训。

## 21.4 照抄的原文细节（9811-9831）

- **9811 在 9814 之前** → 非标签串也归零 `m_nScriptGotoCount`（已单测）。
- **9816** `GetValidStr3_Ex(sData, sLabel, #13)`：**原地改 `ref sLabel`**、返回剩余串；本方法**丢弃**剩余串。**照抄 `ref` 语义**，未改成"返回元组"风格。
- **9819 的守卫在 9821 之先** → 若 `CurrLable` 恰等于 `@back`，清栈逻辑**根本不执行**（已写**差异断言**）。
- **9823-9824 赋值顺序照抄**（先存旧值进 `GoBackLable`，再覆盖 `CurrLable`）。
- **9826-9831 只清一层**（`CurrLable <> ''` 清 `CurrLable`，否则清 `GoBackLable`）。
- **9817-9818 `@HeroMap` 特殊直跳** `GotoLable`，**不**走标签栈（已单测栈未动）。

## 21.5 ★ 偏差 **D37**：`GetCastleUnderWar` 接缝（裁定 B①，默认**抛异常**）

| 项 | 内容 |
|---|---|
| **编号** | **D37** |
| **位置** | `Npc/ObjNpcSeams.cs`（`Func<object,bool> GetCastleUnderWar`） |
| **原文** | `ObjNpc.pas:2533` `TUserCastle(m_Castle).m_boUnderWar` |
| **为何不落字段** | `TUserCastle` 已移植，但该字段的**赋值点在未移植的城堡战逻辑里** → 加字段会**恒为 false**，即"伪装成正式归属的**静默中性值**"，比接缝更糟 |
| **默认行为** | **抛 `NotSupportedException`**（台账 §25.2）——未接线时**立即暴露**，不静默 false |
| **删除条件（可执行）** | 当 `TUserCastle.m_boUnderWar` 落地**且赋值点接通**时删除本接缝、改直读。判据：`grep -n 'm_boUnderWar' src/GXX.M2Server/Engine/Castle.cs` 出现**赋值**（`=` 左侧）而非仅声明 |
| **触发面（窄路径，明确写出）** | **`m_boCastle = true` 的城堡 NPC 调用 `UserSelect` 时目前会抛**；绝大多数 NPC 的 `m_boCastle` 为假，**不会走到**该接缝 |

## 21.6 覆盖口径

| 口径 | 切片 34 | **切片 37** |
|---|---|---|
| Covered / 112 | 66 | **67** |
| Seam / 112 | 4 | **4** |
| Missing / 112 | 42 | **41** |

**`UserSelect` 三条（`TMerchant` 2087 / `TGuildOfficial` 10101 / `TCastleOfficial` 1186）继续 `Missing`** ✅（派发体未落地）。
累计去替身：§13 减 8、§14 减 1、§17 减 2、§19 减 1、**本轮 ±0**（按裁定新增 D37）。

## 21.7 下一轮

仍待办：**`TMerchant.UserSelect` 解析段 2526-2566 + 门控链 2569-2590**（易抄错点清单见 §20.4）。
**本轮新查出的额外依赖**（供下轮直接申请）：
- `g_FunctionNPC` —— 已在 `DamageHealthCore.cs:78` 出现（**他人文件**），需确认托管暴露形态；
- `g_ManageNPC` —— **全仓缺** → 需 **1 个新接缝**；
- `g_MissionNPC` —— 本车道已有（`ObjNpcLabels.cs:64`）。
---

# 22. 第十六轮（切片 39）：3 个全局身份接缝 + **普查自我纠正（`LableIsCanJmp` 其实未移植）** ★ 本节优先于 §21

## 22.1 commit

| # | commit | 内容 |
|---|---|---|
| 39 | `7e018f50` | 3 个全局身份接缝（`IsFunctionNpc`/`IsManageNpc`/`IsMissionNpc`）；解析段**暂缓**并留下落点注释 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,325 passed / 0 failed**。越区检查为空。

## 22.2 ★★ 普查自我纠正：`LableIsCanJmp` **在 `src` 无任何代码声明**

| 轮次 | 我的结论 | 实际 |
|---|---|---|
| §20.4（上轮） | `LableIsCanJmp` ✅ 已存在（`NpcSession.cs:346`） | ❌ **错** —— `:346` 是**注释行**（讨论 `m_CanJmpScriptLableList` 恒为空的那段说明） |
| 本轮实测 | `Select-String -Pattern 'LableIsCanJmp' \| ? { 非注释行 }` → **无输出** | **全仓没有任何代码声明** |

**根因**：上轮那条普查用的是 `\bLableIsCanJmp\b`，**没有过滤注释**；而 `src` 里它**只出现在注释中**。
**教训（已记）**：**普查脚本必须排除注释行**（我在其它普查里做了 `-notmatch '^\s*///'`，这一条漏了）。
⇒ 这也说明"✅ 已存在"必须附**声明处**（文件:行 + 那一行**是代码**），而不是"某处提到过"。

## 22.3 因此解析段**暂缓**，并**申请 1 个新接缝**

门控链 2573/2575 依赖 `PlayObject.LableIsCanJmp(sLabel)`。**请裁定新增**：
| 项 | 内容 |
|---|---|
| 名称 | `NpcSeams.LableIsCanJmp` |
| 签名 | `Func<TPlayObject, string, bool>` |
| 原文 | `ObjPlayer.pas` 的 `function TPlayObject.LableIsCanJmp(sLabel: string): Boolean`（`ObjNpc.pas:2573/2575` 调用） |
| 建议默认 | **`false`**（忠实：见 §22.4 的判据 —— 它对应 `m_CanJmpScriptLableList` 查询"未命中"，而该表在原文里**恒为空**、只命中 `@main`/`@HeroMap`/Yes/No 几个硬编码项） |
| 删除条件（可执行） | 当该函数在托管侧落地时删接缝改直调；判据：`grep -n 'LableIsCanJmp' src/GXX.M2Server/Engine/` 出现**代码声明**（`bool LableIsCanJmp(`） |

> 另外只差一个 `using GXX.Core.Rtl;`（`DelphiRTL`）—— 无需申请，下轮直接加。

## 22.4 本轮已落地：3 个全局身份接缝（**默认 `false` 是忠实的，不是静默占位**）

原文 2572/2581/2590 用 `Self = g_FunctionNPC` / `Self = g_ManageNPC` / `Self = g_MissionNPC`（**对象同一性**）。
| 接缝 | 原文 | 默认 |
|---|---|---|
| `IsFunctionNpc` | `Self = g_FunctionNPC`（2572/2581/2590） | `false` |
| `IsManageNpc` | `Self = g_ManageNPC`（2572）—— 该全局**全仓未移植** | `false` |
| `IsMissionNpc` | `Self = g_MissionNPC`（2572/2581/2590） | `false` |

**★ 为什么这里用 `false` 而 D37（`GetCastleUnderWar`）必须抛** —— 这个区别是刻意的，已写进代码注释：
- 这三个是 `M2Share.pas` 的**未初始化全局 = nil** ⇒ 原文在初始化前 `Self = g_FunctionNPC` **本来就恒为 false**。
  `_ => false` 是**忠实表达**（等价于 nil），**不是**"静默中性值"。
- D37 那种情况是"**真值不可知**"（字段存在与否都不确定），**才必须抛**。
> 判据（可复用）：**能把默认值对应到原文某个已定义状态（如 nil）就是忠实；对应不到就必须抛。**

## 22.5 解析段（2527-2596）的**逐段规格**（已复核，下轮按此机械落地，避免重读原文）

1. 2527 `inherited;` → `base.UserSelect(...)`（落到第十五轮那个真实现）。
2. 2529-2530 `if not (ClassNameIs(TMerchant.ClassName)) then Exit;`
   —— ★ **精确类名**比较 → `GetType() != typeof(TMerchant)`（**不是** `is`）。
3. 2533 `not m_boCastle or not ((m_Castle<>nil) and underWar) and (PlayObject<>nil)`
   —— ★ **Delphi 优先级 `not` > `and` > `or`** → `(!m_boCastle) || ((!castleUnderWar) && (PlayObject != null))`。
   `m_Castle` 走 `NpcSeams.GetNpcCastle(this)`；`underWar` 走 `NpcSeams.GetCastleUnderWar`（**D37**，窄路径）。
4. 2535 `(sData <> '') and (sData[1] = '@')` —— `sData[1]` **1-based**、且**已先判空** → `sData[0]`。
5. 2538 `sMsg := GetValidStr3_Ex(sData, sLabel, #13);` —— **两个出口都接**（`ref sLabel` + 返回值）。
6. 2540-2545 三重条件 + `Pos('(')`（**找不到返回 0**）→ `DelphiRTL.Copy(sLabel, 1, nPos-1)`。
7. 2549-2562 `@FOUNDRYITEM_`/`@SHOWITEM_`：后缀存 `m_sNpcSelectItemName`、`sLabel` 归一成前缀；否则置 `''`。
8. 2564-2566 `m_sScriptLable := sData; m_sInputData := sMsg;`
9. 2569 `boAllowSelect := AllowSelect(sLabel)`（本车道已覆盖）。
10. 2572-2575 三全局之一 → `boCanGoto := LableIsCanJmp(sLabel) and boAllowSelect`；否则 `boCanGoto := LableIsCanJmp(sLabel)`。
11. 2576-2579 `not boCanGoto and m_boMessageBox` → `CompareLStr(sLabel, m_sYesLable/m_sNoLable, **Length(sLabel)**)`
    —— ★ 长度参数是 **`Length(sLabel)`**，不是被比较串的长度。
12. 2581-2588 函数/任务 NPC 且 `not boAllowSelect` → `MainOutMessage(...)` + **早退**。
13. 2590 `boCanJmp := boCanGoto or (IsFunctionNpc and allowSelect) or (IsMissionNpc and allowSelect)`。
14. 2592-2596 `SameText(sLabel, sNF_SendMsg)` 且 `sMsg = ''` → **早退**。
15. 2531-2533/2892-2897 `try … except on E` → `catch` 里报 `sExceptionMsg`（含 **`nCode`**，分段 0..21）。

## 22.6 覆盖口径（未变）

Covered **67** / Seam **4** / Missing **41**；**`UserSelect` 三条继续 `Missing`** ✅。
`UserSelectPrepare` 的落点已在 `ObjNpcUserSelect.cs` 里留**注释块**（写明暂缓原因与所需接缝）。
---

# 23. 第十七轮（切片 41）：`UserSelect` **解析段 + 门控链**（2527-2596）1:1 ★ 本节优先于 §22

## 23.1 commit

| # | commit | 内容 |
|---|---|---|
| 41 | `9c23c909` | `UserSelectPrepare`（原文 2527-2596）1:1 + 新接缝 `LableIsCanJmp`（默认 `false`）+ 29 用例 |

门禁：`dotnet build GXX.slnx` **0 error**；`GXX.M2Server.Tests` **9,354 passed / 0 failed**。越区检查为空。

## 23.2 五个易抄错点**全部写成差异断言**（本轮的核心交付）

| # | 原文 | 我写的差异断言 | 若误抄会怎样 |
|---|---|---|---|
| ① | 2529 `ClassNameIs(TMerchant.ClassName)` = **精确类名** | `Prepare_DerivedClassInstance_ReturnsFalse_LikeClassNameIs`：先断言 `d is TMerchant` **为真**，再断言 `UserSelectPrepare` **返回 false** | 抄成 `is` → 派生类不再 `Exit`，**行为放宽** |
| ② | 2533 **Delphi 优先级 `not` > `and` > `or`** | `Prepare_CastleNpcWithUnderWar_DoesNotEnterParseBranch` + 两条对照（no-underWar / no-castle 均**进入**） | 抄成 `(not A or not X) and Y` → 城堡 NPC 的解析**整体反过来** |
| ③ | 2538 `GetValidStr3_Ex` **两个出口**（`ref sLabel` + 返回剩余串） | `Prepare_BothOutletsArePopulated`（同时断言 `sLabel` 与 `sMsg`） | 抄成"只取一个" → 丢掉标签或丢掉输入数据 |
| ④ | 2542 `Pos('(')` **找不到返回 0** | `Prepare_LabelWithParenButNoClosingParen_IsNotTruncated` + `Prepare_LabelWithoutSecondAt_IsNotTruncated` | 抄成"没找到也截断" → 标签被截掉尾巴 |
| ⑤ | 2578 长度参数是 **`Length(sLabel)`** | `Prepare_MessageBoxBranch_UsesLabelLengthAsPrefixLength`（Yes 标签**带长后缀**仍命中） | 抄成被比较串长度 → 前缀比较退化为全等 |

## 23.3 切分处**差异断言**（按纪律：空串 / 仅分隔符 / 连续分隔符 / 首尾分隔符）

| 输入 `sData` | 期望 `sLabel` | 期望 `sMsg`（剩余串） | 说明 |
|---|---|---|---|
| `"@标签\r剩余串"` | `"@标签"` | `"剩余串"` | 常规 |
| `"\r"` | `""` | — | **仅分隔符** |
| `"\r@a"` | `""` | — | **首分隔符**：`sData[0]='\r'` ≠ `'@'` ⇒ 2535 门不通过（`m_sScriptLable` 也**未**被写） |
| `"@a\r\rb"` | `"@a"` | `"\rb"` | **连续分隔符**：剩余串**原样**不再切 |
| `"@a\r"` | `"@a"` | `""` | **尾分隔符** |

**未写任何依赖 `Pos("")` 的断言**（台账记录其 Delphi 语义至今未修）。

## 23.4 新接缝 / 其它

- **`NpcSeams.LableIsCanJmp`** : `Func<TPlayObject, string, bool>`，**默认 `false`** ——
  忠实依据：`m_CanJmpScriptLableList` 在原文里**恒为空**（`GetScriptLabel`(`ObjPlayer.pas:15216`) **原文就是坏的**），
  实际只命中 `@main`/`@HeroMap`/Yes/No 等**硬编码**分支 ⇒ "查表未命中"是原文真实行为。
  删除条件（可执行）：`grep -n 'LableIsCanJmp' src/GXX.M2Server/Engine/` 出现**代码声明**（`bool LableIsCanJmp(`）。
- `using GXX.Core.Rtl;` 已加（`DelphiRTL.Pos`/`Copy`）。
- **2535 的 `sData[1]` 1-based 且已先判空** → `sData[0]`，已由"首分隔符"用例覆盖。
- **2586 / 2595 两处早退**已各有正反两例（`Prepare_FunctionNpcWithDisallowedSelect_ExitsAndMessages`；
  `Prepare_SendMsgLabelWithEmptyMsg_Exits` + `...WithNonEmptyMsg_Continues`）。
- **2527 `inherited` 真的落到基类**：`Prepare_CallsBaseUserSelect_ResettingGotoCount` 断言
  `m_nScriptGotoCount` 被基类归零；`Prepare_DerivedClass_StillCallsBaseBeforeExiting` 断言
  派生类即使立刻 `Exit`，**基类也已经被调用**（顺序正确）。
- `nCode` 的 `try/catch` 已照抄（异常里报 `sData` + `Code: {nCode}`）；本轮**未**构造触发异常的用例
  （需要让被测路径抛异常，属人为注入，收益低 —— 如实登记为**未覆盖**项）。

## 23.5 覆盖口径（未变，继续克制）

Covered **67** / Seam **4** / Missing **41**；**`UserSelect` 三条（`TMerchant` 2087 / `TGuildOfficial` 10101 /
`TCastleOfficial` 1186）继续 `Missing`** ✅ —— 本方法是**解析段**，派发体（2597-2899）仍未落地。
`UserSelectPrepare` 已在代码注释里写明**删除条件**（`UserSelect` 由 `Missing`→`Covered` 且本方法零引用）。

## 23.6 下一轮

`UserSelect` 只剩**派发体 2597-2899**（约 300 行）：`switch (NpcProcessCmd.g_NpcProcessCommand.GetCommand(sLabel))`
+ 30+ 个 `nNF_*` 分支。届时：
1. 把 `UserSelectPrepare` 内联进 `TMerchant.UserSelect`、并把 `UserSelectRepairCommands` 的两个 `case`
   搬进新 `switch`，两个临时方法一并删除（两处删除条件都已写明）；
2. `UserSelect` 三条登记同时由 `Missing` → `Covered`。