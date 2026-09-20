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