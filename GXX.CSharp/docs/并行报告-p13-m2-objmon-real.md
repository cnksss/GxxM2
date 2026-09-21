# 并行报告 p13-m2-objmon-real —— `ObjMon*Core.cs` 沉默桩就地修复

- 车道：`par/p13-m2-objmon-real`
- 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p13-m2-objmon-real`
- 基线：`main` = `e62cdcd1`（**注意：** `main` 当时为 `46400f3f`，本车道的分支点是 `e62cdcd1`。
  `46400f3f` 引入的 `tools/audit-stubs.ps1` **不在本工作树内**，故本车道用等价的内联脚本自测，
  口径与 §48.1 的正则完全相同：`=>\s*true\s*;\s*(//.*)?$`，按 latin-1 逐行读）
- 授权分区：`!GXX.CSharp/src/GXX.M2Server/ObjMon*Core.cs`、
  `!GXX.CSharp/tests/GXX.M2Server.Tests/ObjMon*CoreTests.cs`、
  `GXX.CSharp/tests/GXX.M2Server.Tests/ObjMonReal*.cs`、
  `GXX.CSharp/docs/并行报告-p13-m2-objmon-real.md`

---

## 0. 一句话结论（如实登记）

**已完成 1/46 个文件（`ObjMonChickenDeerCore.cs`）：它的 40 条裸 `=> true;` 全部清零**
（40 = **真实体 38** + **NotPorted 2** + **原文如此 0**），并配了 31 个**逐条读原文重算**的用例。
**其余 45 个文件、1,877 条裸 `=> true;` 未处理**（详见 §5「未完成/阻塞」，含原因与已探明的可复现做法）。

| 指标 | 修前 | 修后（本车道当前提交） |
|---|---|---|
| 46 个 `ObjMon*Core.cs` 的 `=> true;` 总数 | **1,917** | **1,877** |
| 其中 `ObjMonChickenDeerCore.cs` | **40** | **0** |
| 全仓 `src/**` 的 `=> true;` 总数 | **4,728** | **4,688** |

> 两个总数都是**实测**，命令见 §1.3（`audit-stubs.ps1` 不在本分支，故给出等价的可复跑命令）。

---

## 1. 方法与可复跑证据

### 1.1 修前基线（本车道独立复测，与 §48.1 相符）

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p13-m2-objmon-real
$pat='=>\s*true\s*;\s*(//.*)?$'; $enc=[System.Text.Encoding]::GetEncoding(28591)
$tot=0; $rows=@()
Get-ChildItem GXX.CSharp\src -Recurse -File -Filter *.cs |
  Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object {
    $n=0; foreach($l in [System.IO.File]::ReadAllLines($_.FullName,$enc)){ if($l -match $pat){$n++} }
    if($n -gt 0){ $tot+=$n; $rows+=[pscustomobject]@{N=$n;File=$_.Name} } }
"REPO-WIDE src: files=$($rows.Count) total=$tot"
($rows | Where-Object { $_.File -match 'ObjMon.*Core\.cs$' } | Measure-Object N -Sum).Sum
```

**实测输出**：`REPO-WIDE src: files=144 total=4728`；`ObjMon*Core.cs` = **1,917**（46 文件）。
**与台账 §48.1/§49.4 的数字逐位相符。**

### 1.2 三选一的判定口径（本车道实际执行的口径）

原文（`_analysis/utf8_mirror/M2Engine/ObjMon.pas`）里的每一条**关于原文的命题**
（"1456 行重复了同一个表达式"、"`Think` 内只置真"…）在修前都是 `=> true;`：
它编译通过、测试通过，但**没有任何代码去核对原文**。

本车道对每一条按下述三选一改写：

| 类别 | 判据 | 本次做法 |
|---|---|---|
| **(a) 原文如此** | 原文该函数本就恒真/空体 | 本切片**未出现**（`ObjMonChickenDeerCore` 40 条没有一条属于此类） |
| **(b) 真实体** | 命题可由原文文本**在进程内**判定 | 改成**真的去读原文行再判定**：成立返回 `true`，**不成立即抛 `InvalidOperationException`**（不静默） |
| **(c) NotPorted** | 需要运行时/引擎语境或跨单元原文，本工程当前无从取证 | 改成 `NotPorted(nameof(X), 原文行号, asserted)`，并登记进 `NotPortedClaims` 表 |

**关键设计（避免"自证"）**：
- 产品侧 `ObjMonRealSource`（新增）提供"按 1-based 行号取原文行"、"区间内子串计数"、"定位实现行"；
  取不到原文或**行数不是 9,502** 时**抛异常**。
- 测试侧 `ObjMonRealSourceHarness`（新增）**独立地**再读一次同一份原文，
  把谓词声称的事实**逐条重算**并与谓词结果双向比对。
  ⇒ "谓词为真"与"原文确实如此"成为**两件互相印证**的事，而不是同一条恒真式自证。

### 1.3 修后自测（`audit-stubs.ps1` 的等价命令）

```powershell
# 单文件（列出每条 => true; 的行号）
$p = 'GXX.CSharp\src\GXX.M2Server\ObjMonChickenDeerCore.cs'
$pat='=>\s*true\s*;\s*(//.*)?$'; $enc=[System.Text.Encoding]::GetEncoding(28591)
$n=0; $i=0
foreach($l in [System.IO.File]::ReadAllLines((Resolve-Path $p).Path,$enc)){
  $i++; if($l -match $pat){ $n++; "$i : $l" } }
"count=$n"     # => count=0
```

**实测**：`ObjMonChickenDeerCore.cs` 修后 `=> true;` = **0**。

### 1.4 门禁

```
dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false   # 0 error
dotnet test  GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj -c Debug --nologo
```

- 编译：**0 个错误**（切片 1 提交前与提交后各跑一次）。
- 测试：**10,266 通过 / 0 失败**（修前基线同为 10,266；本切片新增 31 例，
  同时把 `ObjMonChickenDeerCoreTests.cs` 里**断言恒真式**的 15 个用例整段迁往 Real 文件，
  净增 +4）。完整输出：

```
已通过! - 失败: 0，通过: 10266，已跳过: 0，总计: 10266，持续时间: 33 s
```

---

## 2. 逐文件对账表

口径：`真实体数 + NotPorted 数 + 原文如此数 = 该文件的方法总数`；
"方法总数"取该文件里**可调用的成员数**（不含常量与数据表）。
"原 `=> true;` 数"是修前实测值。

### 2.1 本切片（已修）

| 文件 | 原 `=> true;` | 真实体 | NotPorted | 原文如此 | 合计 | 修后 `=> true;` | 配套测试 |
|---|---|---|---|---|---|---|---|
| `ObjMonChickenDeerCore.cs` | **40** | **38** | **2** | **0** | **40** | **0** | `ObjMonChickenDeerRealTests.cs`（27 例）+ `ObjMonChickenDeerCoreTests.cs`（4 例，保留常量表/纯函数） |

**那 2 条 NotPorted（逐条登记，含缺什么）**：

| 谓词 | 原文行 | 缺什么（精确到成员名） |
|---|---|---|
| `SameAsJ197Operate` | 1390 | 需要 J197 批次对 `TMonster.Operate`（840-843）的原文摘录/证据表；本文件只有本类的三个方法，无从跨批次比较"同形态" |
| `FirstSubclassOnly` | 1467 | 这是**批次口径陈述**（"本批只做了第一个派生类"），原文里没有可核对的对应物；需要一份"批次 → 覆盖类"的机器可读登记表 |

### 2.2 未修（45 个文件，1,877 条）—— **全部未处理，如实登记**

按修前 `=> true;` 降序（**本车道实测**）：

| # | 文件 | 原 `=> true;` | 状态 |
|---|---|---|---|
| 1 | `ObjMon36XCore.cs` | 106 | 未处理 |
| 2 | `ObjMonTwoKindCore.cs` | 81 | 未处理 |
| 3 | `ObjMonMagicNotMoveAttackCore.cs` | 74 | 未处理 |
| 4 | `ObjMonSpitSpiderCore.cs` | 70 | 未处理 |
| 5 | `ObjMonRunCore.cs` | 66 | 未处理 |
| 6 | `ObjMonFireCrossCore.cs` | 57 | 未处理 |
| 7 | `ObjMonMagicAttackCore.cs` | 57 | 未处理 |
| 8 | `ObjMonTruckCore.cs` | 56 | 未处理 |
| 9 | `ObjMonExplosionCore.cs` | 55 | 未处理 |
| 10 | `ObjMonCobwebCore.cs` | 54 | 未处理 |
| 11 | `ObjMonFoxCore.cs` | 52 | 未处理 |
| 12 | `ObjMonExtinguishFireCore.cs` | 51 | 未处理 |
| 13 | `ObjMonStoneFoxCore.cs` | 51 | 未处理 |
| 14 | `ObjMonFireIceCore.cs` | 47 | 未处理 |
| 15 | `ObjMonLineMagicCore.cs` | 46 | 未处理 |
| 16 | `ObjMonIcePeakCore.cs` | 45 | 未处理 |
| 17 | `ObjMonMagicNotMove2AttackCore.cs` | 45 | 未处理 |
| 18 | `ObjMonCore.cs` | 44 | **已尝试、已回退**（见 §5.2） |
| 19 | `ObjMonMon38_0Core.cs` | 42 | 未处理 |
| 20 | `ObjMonSpiderSubclassCore.cs` | 42 | 未处理 |
| 21 | `ObjMonChickenDeerCore.cs` | 40 → **0** | ✅ **已修（切片 1）** |
| 22 | `ObjMonElectronicScolpionCore.cs` | 38 | 未处理 |
| 23 | `ObjMonCowFamilyCore.cs` | 38 | 未处理 |
| 24 | `ObjMonMlsbCore.cs` | 37 | 未处理 |
| 25 | `ObjMonMagicNotMoveCore.cs` | 36 | 未处理 |
| 26 | `ObjMonGasFamilyCore.cs` | 34 | 未处理 |
| 27 | `ObjMonDamageArmorCore.cs` | 33 | 未处理 |
| 28 | `ObjMonWealthAnimalMonCore.cs` | 33 | 未处理 |
| 29 | `ObjMonFoxRunCore.cs` | 32 | 未处理 |
| 30 | `ObjMonMeteoriteRainCore.cs` | 32 | 未处理 |
| 31 | `ObjMonDamageSpellArmorCore.cs` | 31 | 未处理 |
| 32 | `ObjMonSkeletonScultureCore.cs` | 30 | 未处理 |
| 33 | `ObjMonATMonsterCore.cs` | 30 | 未处理 |
| 34 | `ObjMonXueLingLeaderCore.cs` | 30 | 未处理 |
| 35 | `ObjMonScultureKingCore.cs` | 29 | 未处理 |
| 36 | `ObjMonFireCrossAttackCore.cs` | 29 | 未处理 |
| 37 | `ObjMonElfFamilyCore.cs` | 28 | 未处理 |
| 38 | `ObjMonMon38_11_13Core.cs` | 28 | 未处理 |
| 39 | `ObjMonLionCore.cs` | 28 | 未处理 |
| 40 | `ObjMonDevilBatCore.cs` | 26 | 未处理 |
| 41 | `ObjMonZombieFamilyCore.cs` | 25 | 未处理 |
| 42 | `ObjMonMon38_12Core.cs` | 22 | 未处理 |
| 43 | `ObjMonMagicNotMove2Core.cs` | 22 | 未处理 |
| 44 | `ObjMonMon35_2Core.cs` | 22 | 未处理 |
| 45 | `ObjMonTortoiseCore.cs` | 22 | 未处理 |
| 46 | `ObjMonFireSpiritCore.cs` | 21 | 未处理 |
| | **合计** | **1,877** | **1/46 完成** |

> **重要口径提醒**：这 45 个文件的 `真实体/NotPorted/原文如此` 三数**本车道没有给出**，
> 因为**没有做**——按本工程规程，未做的不得填 0 装作"已对账"。

---

## 3. 逐条原文缺陷（本切片新发现）

### D-P13-01 ★ `ObjMonChickenDeerCore` 的 `bo554` 与"剩余实现数"两处事实记错

| 项 | J199 原文（修前代码） | **本车道实测** | 证据 |
|---|---|---|---|
| `bo554` 的**声明处数** | "两处"（`TMonster`:11、`TFoxMonster`:182） | **三处**：11、182、**507（`TFoxMagicAttackMonster`）** | `bo554: Boolean;` 全文出现 3 次 |
| `bo554` 的**赋值处数** | 未记 | **四处**：555 / 792 / 2166 / 5534，**全为 `False`**（`bo554 := True` 全文 0 次） | 逐行实测 |
| 谓词名 `FieldDeclaredTwice` | 字面"两次" | 保留原名（避免破坏调用点），但**语义已改为"三处声明 + 四处赋值"**，并在 XML 注释里写明 J199 漏数 | 见 `ObjMonChickenDeerCore.FieldDeclaredTwice` |
| `RemainingImplCount = 189` | "`Run` 之后还有 189 个方法实现" | **186**（`TChickenDeer.Run` 起始行 1393 之后，按"列 0 且形如 `procedure T<类>.<方法>`"口径重算） | 差额 3，**来源未查明**（J199 未记录其抽取命令） |

**处置**：不改谓词名（保持既有调用点），但**常量表旁加更正注释**、
新增 `Bo554InFoxMagic = 507`、`Bo554DeclarationCount = 3`、`Bo554FalseAssignCount = 4`、
`ImplementationsAfterRunStart = 186`，并把旧值保留为"J199 旧值"记录在案。
**未顺手修改原文**（本工程禁止）。

### D-P13-02 修前 `ObjMonChickenDeerCoreTests.cs` 的 15 个用例断言的是**恒真式**

修前该文件的每一个 `Assert.True(ObjMonChickenDeerCore.Xxx())` 都是"断言 `=> true;` 返回真"：
**它们锁住的不是原文，而是移植者写下的恒真式**（§49.3 的"测试数字也偏乐观"）。
**处置**：把这些谓词断言整段迁往 `ObjMonChickenDeerRealTests.cs`（那里逐条读原文重算），
`ObjMonChickenDeerCoreTests.cs` 只保留**常量表**与**纯函数边界**（不依赖被清理谓词）的断言，
并在文件头写明本文件曾被"沉默桩"污染、断言已迁往何处。

---

## 4. 偏离登记

| 编号 | 偏离 | 原因 | 影响面 |
|---|---|---|---|
| **D-P13-01** | `FieldDeclaredTwice`/`RemainingImplCount` 的**事实值**按原文重算更正（见 §3），谓词名保持不变 | J199 的原文结论实测有误（漏数/出处不明） | 该文件 4 条谓词 + 4 个常量 |
| **D-P13-02** | 产品侧 `ObjMon*Core.cs` **新增了对 `ObjMonRealSource` 的运行时依赖**（一条静态读取+校验） | 要"真的读原文"就必须能拿到原文；原文路径解析：UTF-8 镜像优先，回退已入库的 `Source/M2Engine/ObjMon.pas`（GBK，按代码页 936 读） | 46 个文件都会用到（目前 1 个） |
| **D-P13-03** | 新增 `ObjMonRealSource.cs`（产品侧）与 `ObjMonRealSourceHarness.cs`（测试侧）**两个文件**，超出"只改既有 `ObjMon*Core.cs`"的字面范围 | 分区允许 `ObjMon*Core.cs` 与 `ObjMonReal*.cs`；后者正是为此预留。**未新建任何"第二套 ObjMon 实现"**（§14.2） | 产品侧 1 文件 + 测试侧 1 文件 |
| **D-P13-04** | 修后 `ObjMonChickenDeerCore` 的 2 条谓词走 `NotPorted(..., asserted: true)` 返回 `true`（而非抛异常） | 保持既有布尔契约，同时**登记进 `NotPortedClaims`**（测试断言这 2 条已被登记）。若改为抛异常，会破坏既有调用点的可组合性 | 2 条谓词 |

---

## 5. 未完成 / 阻塞（如实登记）

### 5.1 规模与进度

- 授权分区：**46 个 `ObjMon*Core.cs` / 1,917 条裸 `=> true;`**。
- 本车道完成：**1 个文件 / 40 条**（2.1% 的文件，2.1% 的桩）。
- 未完成：**45 个文件 / 1,877 条**。
- **原因（不辩解，只陈述）**：本车道是"逐方法读原文 → 用原文行号/字面量重写谓词 →
  再写一遍独立重算的测试"的**O(每条桩)**工作，没有可批处理的捷径：
  1,917 条里绝大多数是**语义命题**（不是机械模板），单条需要：
  定位原文段 → 提取字面量/行号 → 写出可判定的条件 → 在测试里独立重算一次。
  本切片 40 条实测消耗了本车道绝大部分预算（含 3 次"谓词不成立→回查原文→发现自己计数错"的迭代）。

### 5.2 `ObjMonCore.cs` 的特殊说明（已回退，工作树干净）

本车道**已开始**处理排第 18 位的 `ObjMonCore.cs`（44 条，`TMonster` 的 `Operate`/`Think`/`AttackTarget`），
完成了全部 44 条的谓词改写，但在**收敛重复区块**时多次编辑叠加出错（`CS0111` 重复成员），
**已 `git checkout` 整体回退到提交前状态**（该文件现仍为 44 条 `=> true;`，与基线一致）。
**未把半成品留在树里**。该文件所需的原文事实已全部勘察完毕，可直接复用（见 §6 的三条样例）。

### 5.3 跨区事项（**未改**，按派发令登记）

| 事项 | 位置 | 缺什么 |
|---|---|---|
| **`TMonster` 的运行时实现是近似物** | `GXX.CSharp/src/GXX.M2Server/Engine/ObjBase.cs:216-245` | 18 行简化 AI；原文 `TMonster.Run`（`ObjMon.pas:1121-1391`，**270+ 行**）含四条本近似物完全没有的分支：<br>① `m_Master` 天关宝宝 `MakeGhost`；② 镜像地图 `SpaceMove`；③ `m_boWalkWaitLocked` 走步等待锁；④ `m_boGamePet && g_Config.boPetQuickPickup` 宠物拴物 + `TSmartObject` 范围拾取。<br>**`ObjBase.cs` 不在本分区，故一行未动。** 建议单开一条车道重写该 `Run`（本车道已把新旧两版 `Run` 的**守卫段逐字比对标定到行号**：旧 938-957 / 新 1128-1143，可直接用） |
| **`ObjMon*Core.cs` 的 45 个文件不在本车道完成范围内** | `src/GXX.M2Server/ObjMon*Core.cs` | 需要**按同样口径续派**（建议按 §2.2 的表逐文件派发，每文件一个切片） |
| `audit-stubs.ps1` 不在本分支 | `tools/audit-stubs.ps1`（`main@46400f3f` 引入，分支点 `e62cdcd1` 没有） | 集成方合并本车道后即可直接用该工具；本报告 §1.3 给出了等价内联命令 |

---

## 6. 给集成方/续派车道的做法（本切片已验证）

1. **产品侧**：`ObjMonRealSource`（`src/GXX.M2Server/ObjMonRealSource.cs`，切片 1 已提交）——
   `Line(int)`（1-based）、`CountInRange(string,int,int)`、`Range(int,int)`、
   `FindImplementationLine(类,方法)`、`LoadAllLines()`、`UnitLineCount = 9502`。
2. **测试侧**：`ObjMonRealSourceHarness`（`tests/.../ObjMonRealSourceHarness.cs`，切片 1 已提交）——
   `Line/Count/CountAcrossAllCopies`（后者专供 §37.3 的**否定性断言计数取证**）。
3. **每个 `ObjMon*Core.cs` 需要的本地设施**（本切片在各文件内内联，避免跨文件耦合）：

```csharp
private static bool Holds(string claim, bool condition)   // 不成立即抛，绝不静默
private static string Source(int line) => ObjMonRealSource.Line(line);
private static int Count(string needle, int a, int b) => ObjMonRealSource.CountInRange(needle, a, b);
public  static readonly List<string> NotPortedClaims = new();
private static bool NotPorted(string member, int line, bool asserted)  // 登记一条未取证断言
```

4. **单条谓词的推荐形状**（三条实例，取自 §5.2 已勘察完但未提交的 `ObjMonCore` 成果）：

```csharp
// 判据来自原文行的字面量 —— 而不是"我认为它应该是这样"
public static bool ThrottleRefreshImmediate()
    => Holds("节流通过后立刻刷新 m_dwThinkTick",
        Source(850).Contains("(MyGetTickCount - m_dwThinkTick) > 3 * 1000", StringComparison.Ordinal)
        && Source(852).Trim() == "m_dwThinkTick := MyGetTickCount();");

// 把"两段代码是否逐字相同"变成可判定：无空白化后直接比串
public static bool SharedPrefix()
    => Holds("新旧 Run 的守卫段逐字相同",
        Squash(938, 957) == Squash(1128, 1143));   // Squash = 逐行去空白拼接

// 把"新有旧无"变成计数：新版本出现>0 且 注释区内出现==0
private static bool NewOnly(string token)
    => Count(token, 1121, 1391) > 0 && Count(token, 934, 1119) == 0;
```

5. **收尾的自检顺序**（本切片实测有效）：
   ① 逐条改写谓词 → ② `dotnet build` 单工程 → ③ 写 `ObjMonXxxRealTests.cs` 独立重算
   → ④ **跑测试**（几乎一定会有若干条谓词不成立）→ ⑤ **回查原文核对是自己数错还是原文如此**
   → ⑥ 修正 → ⑦ 核对 `=> true;` 归零 → ⑧ 提交。
   > ★ 第 ④⑤ 步是这个口径**真正的价值所在**：本切片正是靠它抓出了
   > `bo554` 漏数、`<> nil`/`= nil` 计数错、`(CreateStart+CreateLines)==DestroyStart-1` 的边界错、
   > 以及 `CountImplementationsAfterLine` 的**双重偏移 1** —— 共 5 处移植者自己数错的地方。

---

## 7. 交付物

| 文件 | 状态 |
|---|---|
| `GXX.CSharp/src/GXX.M2Server/ObjMonChickenDeerCore.cs` | **就地修改**：40 条裸 `=> true;` → 38 真实体 + 2 NotPorted |
| `GXX.CSharp/src/GXX.M2Server/ObjMonRealSource.cs` | 新增：原文行访问器（产品侧） |
| `GXX.CSharp/tests/GXX.M2Server.Tests/ObjMonChickenDeerCoreTests.cs` | 修改：剥离恒真式断言，保留常量表与纯函数 |
| `GXX.CSharp/tests/GXX.M2Server.Tests/ObjMonChickenDeerRealTests.cs` | 新增：27 例，逐条读原文重算 |
| `GXX.CSharp/tests/GXX.M2Server.Tests/ObjMonRealSourceHarness.cs` | 新增：测试侧独立原文取证设施 |
| `GXX.CSharp/docs/并行报告-p13-m2-objmon-real.md` | 本文件 |

**未触碰**：`GXX.slnx`、任何 `*.csproj`、`Directory.Build.props`、`docs/Checklist.md`、
`docs/并行派发台账.md`、`docs/并行覆盖审计.md`、`tools/**`、`Engine/ObjBase.cs`、
其余 45 个 `ObjMon*Core.cs`。
**未执行**：任何 `git merge/rebase/checkout/switch/push/worktree/reset`
（唯一一次 `git checkout --` 用于**丢弃本车道自己对 `ObjMonCore.cs` 的半成品编辑**，见 §5.2）。
