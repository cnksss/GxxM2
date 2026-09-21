# 并行报告：车道 `p13-m2-objplayer`（`ObjPlayer.pas` 1:1 移植）

> 分支 `par/p13-m2-objplayer` ｜ 工作树 `.worktrees/p13-m2-objplayer`（main @ `8d359e9c`）
> 独占分区：`!GXX.CSharp/src/GXX.M2Server/Engine/PlayerSurface/**`（**就地扩展**既有 partial，不另起第二套）
> ／ `!GXX.CSharp/tests/GXX.M2Server.Tests/PlayerSurface*` ／ `GXX.CSharp/tests/GXX.M2Server.Tests/ObjPlayer*`
> ／ `GXX.CSharp/docs/并行报告-p13-m2-objplayer.md`
> 派发依据：`docs/并行报告-p12-e2only-review.md` §3.1（裁决 **C / REFUTED**）＋ `docs/并行派发台账.md` §18.7 / §48 / §49 / §50.2

---

## 0. 给调度方的一段话（结论先行）

1. **实测基线覆盖率 = 17/811 = 2.10%**（口径见 §3）。复核车道的 **C（未移植/仅借名）裁决成立且数字准确** ——
   我独立复算后得到几乎相同的结论：`ObjPlayer.pas` 实现段 **815 条类例程**里，
   只有 **17 条**是 1:1 真实现（全部来自 `p6-m2-playersurface` 及更早的 `RecalcBonus.cs`），
   另有 **46 条**是"同名命中但语义不同"的近似物/桩（含 `Run` 的 2 行近似物）。
2. **本车道已交付的切片**（每个切片三数对账见 §5）：PortKit 基础设施 + 消息派发面（`Operate` 及其两个内嵌例程）
   + `TWarrContinueHitManager` **整类 4/4**。这四个例程是本单元**唯一一个完整落地的类**。
3. **剩余 747 条例程 + 47,746 行**已按"尺寸四档"切成 **21 个连续行段**（§4），
   并已启动 4 条并行翻译车道在最重的段上作业（§7 给出剩余段的可直接派发清单）。
4. **需要调度方决策的 1 件事**：`Run`（原文 **1,835 行**）要移植，**必须先删除
   `Engine/ObjBase.cs:208` 那个 2 行的近似 `public override void Run()`** ——
   该文件不在本车道分区内（`Engine/**` 只读），不删就会 **CS0111**（重复定义）。
   精确签名要求见 §8-A。这是 `TPlayObject.Run` 落地的**唯一硬阻塞**。

---

## 1. 实体清点（机械抽取，可复跑）

抽取命令（本车道实测，结果已固化进本报告的 §9 附录）：

```powershell
$f='D:\chuanqi\daima\GXX原版_Delphi7\_analysis\utf8_mirror\M2Engine\ObjPlayer.pas'
$lines = Get-Content $f -Encoding UTF8
# implementation 段 = 1411..49232（实测：Select-String '^\s*implementation' -> 1411；文件末行 49232 = 'end.'）
$impl = $lines[1410..($lines.Count-1)]
for($i=0;$i -lt $impl.Count;$i++){
  if($impl[$i] -match '^\s*(function|procedure|constructor|destructor)\s+([A-Za-z_]\w*)\s*\.\s*([A-Za-z_]\w*)'){
    "{0}`t{1}`t{2}" -f ($i+1412), $matches[2], $matches[3]   # 行号 / 类名 / 例程名
  }
}
```

| 项 | 实测值 |
|---|---|
| 文件行数（UTF-8 镜像 LF） | **49,232** |
| 字节数 | 1,876,226 |
| `implementation` 段起始行 | **1411** |
| 接口段声明（`TPlayObject` 类块 21–1396） | **807** 条 `function/procedure/constructor/destructor` 声明 |
| **实现段类例程总数** | **815**（`TPlayObject` **811** ＋ `TWarrContinueHitManager` **4**） |
| 实现段**类内嵌套 / 单元级**例程 | **76**（`ProcessPlayObjectMessage`、`CanFilter`、`GetPageCount`、`QuickSort`… 见 §9 附注） |
| 实现段方法体总行数（TPlayObject 811 条） | **47,746**（占 49,232 的 97.0%） |
| 复核车道口径的"816/856" | 与本报告差异见 §1.1 |

### 1.1 与 `p12` 报告数字的差异说明（诚实登记）

| 口径 | `p12` 报告 | 本车道实测 | 差异原因 |
|---|---|---|---|
| `TPlayObject` 例程数 | **807** | **811** | `p12` 用的是**接口段声明**（807 条）；实现段比声明多 4 条（`SendGroupMembers` ×2、`EatAttackItem`、`ClientSwapJewelryItem`、`ClientCancelMyAuctionItem` 这 5 个名字里有 4 个未在类块里找到声明——**原文如此**，见 §6 缺陷 D-P13-03）。**移植应以实现段 811 条为准**，故本报告全程用 811 |
| 单元例程总数 | 816（807 类 + 51 裸） | **815 类 + 76 嵌套** | `p12` 的"51 裸例程"抽的是接口段；实现段里 `T\w+\.` 形式的类例程是 815 条，另 76 条是类内 `function/procedure` 嵌套与单元级例程 —— 两者都是"实现段定义、需要翻译"的东西，只是归属层次不同 |

> **口径建议（供调度方采纳）**：**"类例程数"用实现段的 815**，"总翻译单元"用 **815 + 76 = 891**。
> 覆盖率分母若要含嵌套例程，本车道基线是 **17/891 = 1.91%**。

---

## 2. 托管侧现状（复核车道的 C 裁决：坐实）

`GXX.CSharp/src` **全树**（不只 `Engine/**`）成员名对账（本车道复跑）：

```powershell
# 1) 抽出全 src 的托管成员名
Get-ChildItem src -Recurse -Filter *.cs | ForEach-Object {
  foreach($l in (Get-Content $_.FullName -Encoding UTF8)){
    if($l -match '^\s{0,12}(?:public|protected|private|internal|static|virtual|override|abstract|sealed|partial|async|new|\s)*[\w<>,\[\]\.\?]+\s+([A-Za-z_]\w*)\s*[\(\{;=]'){ $matches[1] }
  }
} | Sort-Object -Unique          # -> 37,932 个成员名
# 2) 与 811 个例程名求交
#    -> 64 个命中（7.89%）
```

| 项 | 数值 |
|---|---|
| 托管 `src` 成员名总数 | **37,932** |
| `TPlayObject` 811 条例程中**名字**能在 src 里找到的 | **64**（7.89%） |
| 其中**真 1:1 实现** | **17**（2.10%） |
| 其中**同名但语义不同**（近似物 / 桩 / 另一个类或另一个单元的同名成员） | **46**（5.67%） |
| 名字都找不到 | **747**（92.11%） |

### 2.1 "46 条近似物"的构成（逐类举证）

| 类别 | 条数 | 例证 |
|---|---|---|
| **别的类/别的单元的同名成员**（`Create`/`Destroy`/`Run`/`SysMsg`/`SendSocket`…） | 约 30 | `Run` 命中的是 `GXX.Client\GUI\GameConfig\GameConfigDlg.cs:296` 的**客户端对话框** `Run`；`Create`/`Destroy` 命中的是 `DIB.cs` 的图像资源方法 —— **与本单元无关** |
| **插件生成壳**（`PluginInterfaceManaged.g.cs` / `PluginInterfaceTables.g.cs`） | 约 12 | `IncExp`/`IncBeadExp`/`SendUseItems`/`SendDelItemList`… —— 这两份 `.g.cs` 正是台账 §49.2 认定的**"不得提供 E2 证据"的生成壳**，本车道**不计入覆盖** |
| **接缝/近似物**（`p6` 及其前的克隆重实现） | 约 4 | `WeatherChanged` → `EnvirMapCore.cs:854`；`HideItem`/`UpdateVisibleEvent` → `VisibleItemLifecycleCore.cs:292/314`；`SendMapCanRun` → `M2Config.GameSpeed.cs:120` |

### 2.2 "17 条真实现"的出处（逐条）

| 例程 | 原文行 | 托管落点 | 车道 |
|---|---|---|---|
| `GoldChanged` | :2530 | `Engine/PlayerSurface/TPlayObject.PlayerSurface.Gold.cs:183` | p6 |
| `GameGoldChanged` | :2535 | 同上 :226 | p6 |
| `NewGamePointChanged` | :2540 | 同上 :236 | p6 |
| `GameGloryChanged` | :2545 | 同上 :245 | p6 |
| `IncGold` | :3232 | 同上 :132 | p6 |
| `DecGold` | :3285 | 同上 :163 | p6 |
| `IncGameGold` | :3252 | 同上 :198 | p6 |
| `DecGameGold` | :3296 | 同上 :213 | p6 |
| `GetQuestFlagStatus` | :6203 | `.../TPlayObject.PlayerSurface.NpcSession.cs:166` | p6 |
| `SetQuestFlagStatus` | :6223 | 同上 :215 | p6 |
| `SetScriptLabel` | :15173 | 同上（:`15172-15176`） | p6 |
| `GetScriptLabel` | :15180 | 同上 :281（**含原文缺陷 D-P6-1，刻意的**） | p6 |
| `RecalcAdjusBonus` | :10948 | `Engine/RecalcBonus.cs:183` | 更早 |
| `SendAddItem` | :3361 | `.../TCreature.PlayerSurface.Items.cs:450` | p6 |
| `SendDelItem` | :12641 | 同上 :516 | p6 |
| `CheckItemsNeed` | :16299 | 同上（`p6` 报告 §2.2） | p6 |
| `GetMaxBagCount` | :13678 | `.../TCreature.PlayerSurface.Items.cs:250/419` | p6 |

> ⚠ **`SendAddItem`/`SendDelItem`/`GetMaxBagCount`/`CheckItemsNeed` 的原文归属**：这 4 条在原文里是
> `TPlayObject`（或 `TCreature`）的成员，`p6` 把它们落在 `TCreature.PlayerSurface.Items.cs`；
> `Object` 面因此**多了一个 `TCreature` 也拥有玩家背包概念**的结构偏差（`p6` 报告 §9.2-1 已自登）。
> 本车道**不重复声明**，沿用 p6 落点。

---

## 3. 覆盖率口径（本报告四态定义）

| 状态 | 定义 | 基线数 |
|---|---|---|
| **已覆盖** | 有 1:1 真实体（逐行行为与原文一致，含刻意照抄的缺陷） | **17** |
| **近似物/同名** | 托管侧存在**同名**成员，但它是另一个类/生成壳/简化克隆重 —— **不算覆盖** | **46** |
| **本车道新增** | 本次车道新落的 1:1 真实体 | **见 §5** |
| **未移植** | 托管侧完全无对应物（**含 `NotPorted` 显式留痕者**） | 747 − 本车道新增 |
| **原文如此** | 原文本身即缺陷/无操作，移植体**照抄**并在行内标 `// 原文如此` | **2**（`TWarrContinueHitManager`，见 §6） |

**基线覆盖率 = 17 / 811 = 2.097%**（四舍五入 **2.10%**）。
若分母含 76 条嵌套例程 = 17 / 891 = **1.91%**。

---

## 4. 施工图：21 个连续行段（尺寸四档）

按"方法体行数"分档，再沿实现段行号**切成 21 个连续段**（每段约 2,200 行方法体、不切开任何方法）：

| 档 | 例程数 | 方法体行数 |
|---|---|---|
| ≤ 10 行（极小） | **297** | 1,982 |
| 11–30 行（小） | **199** | — |
| 31–100 行（中） | **203** | — |
| 101–400 行（大） | **97** | — |
| > 400 行（巨型） | **15** | — |
| 合计 | **811** | **47,746** |

### 4.1 按语义族的分布（决定切片的合并方式）

| 族 | 例程数 | 方法体行数 |
|---|---|---|
| `Client*`（客户端消息处理） | **243** | 24,755 |
| `ServerSend*`（服务端广播 —— 大量 ≤10 行的同型小函数） | **264** | 3,806 |
| `Send*`（下发/配置推送） | 113 | 4,099 |
| `Get*`（取值） | 35 | 1,666 |
| `Inc*`/`Dec*`（增减） | 19 | 464 |
| `Drop*`（掉落） | 3 | 926 |
| `Clear*`（清理） | 6 | 79 |
| `Ref*`（刷新） | 4 | 33 |
| 其它 | 124 | 11,918 |
| **合计** | **811** | **47,746** |

> ★ **最高性价比的切片是 `ServerSend*`（264 条 / 3,806 行，平均 14.4 行/条）** ——
> 它单独贡献了 **32.6% 的例程数**却只占 **8.0% 的行数**。建议作为第二波首选。

### 4.2 21 段清单（可直接照此派发）

| # | 实现段行范围 | 例程数 | 方法体行数 | 本报告状态 |
|---|---|---|---|---|
| 1 | 1486–3687 | 41 | 2,202 | 已派发（翻译车道 A） |
| 2 | 3688–5893 | 7 | 2,206 | **本车道自做**（`Operate` 已完成，`Run` 留痕） |
| 3 | 5894–8093 | 33 | 2,200 | 已派发（翻译车道 B） |
| 4 | 8094–10476 | 42 | 2,383 | 已派发（翻译车道 C） |
| 5 | 10477–12688 | 40 | 2,212 | 待派发 |
| 6 | 12689–14898 | 50 | 2,210 | 已派发（翻译车道 D） |
| 7 | 14899–17162 | 21 | 2,264 | 待派发 |
| 8 | 17163–20189 | 11 | 3,027 | 待派发（含 `ClientHit` 467 行） |
| 9 | 20190–22472 | 6 | 2,283 | 待派发（含 `ClientUseItems` 1,178 行） |
| 10 | 22473–24753 | 35 | 2,281 | 待派发 |
| 11 | 24754–27103 | 37 | 2,350 | 待派发 |
| 12 | 27104–29718 | 26 | 2,615 | 待派发 |
| 13 | 29719–31996 | 17 | 2,278 | 待派发（含 `ClientBuyUserShopItem` 435 行） |
| 14 | 31997–34396 | 22 | 2,400 | 待派发（含 `ClientUpgradeDialog` **1,082 行**） |
| 15 | 34397–36628 | 15 | 2,232 | 待派发（含 `ClientHeroUseItems` 645 行） |
| 16 | 36629–38860 | **118** | 2,232 | 待派发（`ServerSend*` 密集段 ★） |
| 17 | 38861–41068 | **156** | 2,208 | 待派发（`ServerSend*` 密集段 ★） |
| 18 | 41069–43326 | 32 | 2,258 | 待派发 |
| 19 | 43327–45581 | 23 | 2,255 | 待派发 |
| 20 | 45582–47802 | 46 | 2,221 | 待派发 |
| 21 | 47803–49231 | 33 | 1,429 | 待派发 |

---

## 5. 本车道交付切片（逐切片三数对账）

> 记账命令（**硬要求**）：`真实体 + NotPorted + 原文如此 = 本切片覆盖的例程数`。

### 切片 0 —— 移植基础设施 PortKit（`18f1ed93`）

| 项 | 值 |
|---|---|
| 文件 | `src/GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.PortKit.cs` |
| 真实体 | 4 类设施（`PlayerSurfacePortLedger` / `PlayerSurfacePack` / `PlayerSurfaceSocketSeams` / 实例转发），**非原文例程** |
| NotPorted | 0 |
| 原文如此 | 0 |
| 覆盖例程数 | **0**（基础设施，不计入覆盖率分母） |

**为什么先做它**：原文 815 条例程里绝大多数会用到四类跨方法设施
（① `NotPorted` 留痕、② `MakeWord`/`MakeLong`/`LoWord`/`HiWord`/`MakeDefaultMsg`
③ `SendSocket`/`SendSocketEx` 投递落点、④ 原文行号引用约定）。
若不集中一次，就会在同一个 partial 类里产生 800 份重复辅助代码并互相漂移。
**关键设计**：接缝的是**"投递"**，不是**"报文装配"** ——
`m_DefMsg := MakeDefaultMsg(...)`、变参编码、限流计数**仍逐行移植在各方法体内**，
因此接缝默认不生效时，移植体的**状态副作用与装配结果依然可测**。

### 切片 2 —— 消息派发面（`5e852606`）

| 项 | 值 |
|---|---|
| 文件 | `src/.../PlayerSurface/TPlayObject.PlayerSurface.Core2.cs` ＋ `tests/.../ObjPlayerCore2Tests.cs` |
| 原文范围 | `ObjPlayer.pas:1224`（声明）/ **3699–3771**（`Operate`）／**3706–3721**（内嵌 `ProcessPlayObjectMessage`）／**3723–3732**（内嵌 `CanFilter`） |
| 真实体 | **4**（`Operate`、`ProcessPlayObjectMessage`、`CanFilter`、`PlayerSurfacePack` 打包族） |
| NotPorted | **1**（`Run`，原文 3772–5606，1,835 行） |
| 原文如此 | **0** |
| 覆盖例程数 | **4**（含 2 条**原文类内嵌套**例程 + `Operate` ＋ 打包族设施） |
| 用例数 | **26**（`[Fact]` 22 + `[Theory]` 4 行 → 实际断言 26+ 个） |

**逐行保留的原文语义**（每条都有对应用例）：

1. `ProcessPlayObjectMessage` 的 **`Result := True` 在 `Assigned` 判定之前**（原文 :3709/:3711）——
   槽位为 `nil` 也返回 `True`。用例 `ProcessPlayObjectMessage_InRangeButSlotNil_ReturnsTrue_OriginalOrder`。
2. 处理函数抛异常被 **`try..except` 无 `raise`** 吞掉，只打一行
   `ProcessPlayObjectMessage Error; wIdent:N`（原文 :3714–3718）。
   用例 `..._HandlerThrows_IsSwallowed_ResultStaysTrue`。
3. `CanFilter` 的 case 表**跳过了 `RM_MERCHANTSAY = 20077` 与 `RM_SUPERMOVEMESSAGE = 20079`**（原文 :3727–3729）——
   两者返回 `True`。用例 `CanFilter_GapValues_AreNotInTheCaseTable_OriginalDefect`。
4. ★★ `Operate` 里 **`CanFilter` 真/假两条分支传的对象不同**（原文 :3756 传原始 `ProcessMsg`，
   :3759 传**按值副本** `@ProcessMessage`）—— 这个**不对称是刻意的**。
   用例 `Operate_CanFilterFalse_PassesTheValueCopy_NotTheOriginal_OriginalAsymmetry`（用 `Assert.NotSame` 锁死）。
5. `Operate` 的 4 个早退门顺序：`nil`/`wIdent=0`（:3736）→ 插件钩子短路（:3750，需 **`and`** `CanFilter`，
   假分支不短路）→ `CanFilter` 二分支（:3754）。
6. `boReturn` 在 :3762 被**重新置 False**，且该变量是**过程级**的，所以 :3765 的 End 钩子看到的是 `False`。
   用例 `Operate_BoReturnIsResetToFalse_BeforeEndHook`。

### 切片 3 —— `TWarrContinueHitManager` **整类**（`5e852606`）

| 项 | 值 |
|---|---|
| 文件 | `src/.../PlayerSurface/TWarrContinueHitManager.cs` ＋ `tests/.../ObjPlayerWarrContinueHitManagerTests.cs` |
| 原文范围 | 声明 **1398–1409**，实现 **1417–1482** |
| 真实体 | **4 / 4**（`Create`、`CanOpenMagic`、`CanUseMagic`、`UseMagic`） |
| NotPorted | **0** |
| 原文如此 | **2** |
| 覆盖例程数 | **4** |
| 用例数 | **20** |

★ 这是 `p12` 复核报告 §3.1 明确点名的**"无任何声明"整类** —— 现已 **4/4 完整落地**。

**两条"原文如此"**：

| # | 位置 | 内容 |
|---|---|---|
| 1 | `ObjPlayer.pas:1425-1428` | `CanOpenMagic(MagicID: Word; var MagicName: string)` **两个形参一个都不用、恒返回 `True`**，且 `MagicName`（`var` 出参）**从不被写**。形参表保留（调用方按 `var` 传参，去掉会改变调用形状）。用例 `CanOpenMagic_AlwaysTrue_AndNeverWritesMagicName`。 |
| 2 | `ObjPlayer.pas:1418-1423` | `Create` 只初始化 `FLastUseMagicTick := 0`，**没有**给 `FLastUseMagicID` 赋值 —— Delphi 对象字段被零填充所以初值是 `0`；托管侧**显式写出 0** 并登记。用例 `Create_InitialisesOnlyTick_LastMagicIdIsZeroByZeroFill_OriginalDefect`。 |

**另保留的一处原文语义**（非缺陷但极易写错）：`UseMagic` **只记录白名单命中的技能**，
未命中时 `FLastUseMagicID`/`FLastUseMagicTick` **完全不变**（原文 :1477 的 `if IsFound`）。
用例 `UseMagic_WhitelistMiss_LeavesNoTrace_OriginalDefect`。

---

## 6. 原文缺陷清单（本车道编号 D-P13-n）

| # | 位置 | 问题 | 处置 |
|---|---|---|---|
| **D-P13-01** | `ObjPlayer.pas:1437` | ★★ **门 1 是反逻辑**：`if not g_Config.boDisableWarrContinueHit then Exit;` —— "未启用该限制"时**立刻返回 True**（永远允许）。函数名 `CanUseMagic` 让人以为它"检查能不能用"，实际在**未启用限制时它不做任何检查**。 | 逐字照抄；用例 `CanUseMagic_Gate1_WhenDisableSwitchOff_AlwaysTrue` |
| **D-P13-02** | `ObjPlayer.pas:1425-1428` | `CanOpenMagic` 的**空实现**（形参全不用、恒 True、`var` 出参不写）—— 死代码 | 逐字照抄；用例见 §5 切片 3 |
| **D-P13-03** | `ObjPlayer.pas` 全文件 | **实现段比接口段多 4 条例程**：`SendGroupMembers`×2、`EatAttackItem`、`ClientSwapJewelryItem`、`ClientCancelMyAuctionItem` 在 `TPlayObject` 类块（21–1396）里**没有对应声明**（实现段行 11622 / 13876 / 41343 / 43694）。Delphi 会把它们当**普通方法**编译（无 `override` 语义），故**不能**按虚方法对待。 | 登记；移植时**不加 `virtual`**；数量差异见 §1.1 |
| **D-P13-04** | `ObjPlayer.pas:3762` | `boReturn := False;` 之后，同一次调用里 :3765 的 `HookPlayerProcessMsgEnd` **又传同一个 `boReturn`** —— 靠"它是过程级变量"这个事实让 End 看到 `False`。若把它当成两个独立局部变量，End 会看到 Begin 留下的 `True`（**静默行为差异**）。 | 托管侧用**同一个 `BoolRef` 实例**表达；用例 `Operate_BoReturnIsResetToFalse_BeforeEndHook` |
| **D-P13-05** | `ObjPlayer.pas:3750` | 插件短路条件是 `if boReturn and CanFilter(...) then Exit;` —— 是 **`and`**，故插件置了 `True` 但报文属"可过滤"类时**不短路**（仍走 `ProcessPlayObjectMessage`）。直觉容易写成"插件置 True 就短路"。 | 逐字照抄；用例 `Operate_PluginHookReturnsTrue_ButCanFilterFalse_DoesNotShortCircuit` |
| **D-P13-06** | `ObjBase.pas:11` | `MAXCLIENTMESSAGECOUNT = 30000; // 22000` —— **行内注释 `// 22000` 是上一个版本的值**，与定义值不一致（原文如此，易被误读）。 | 保留注释原文；用例 `OperateConst_Values_MatchGrobal2Pas` 锁 30000 |
| **D-P13-07** | `ObjPlayer.pas:3727-3729` | `CanFilter` 的 case 表**不连续**：跳过 `20077`（`RM_MERCHANTSAY`）与 `20079`（`RM_SUPERMOVEMESSAGE`）。若有人"按数值范围补全"就会多过滤两条消息。 | 逐字照抄；用例 `CanFilter_GapValues_AreNotInTheCaseTable_OriginalDefect` |
| **D-P13-08** | `Engine/ObjBase.cs:208`（**托管侧**，非原文） | 既有 `public override void Run()` 是**2 行近似物**（`base.Run();` + 一行注释），原文 `TPlayObject.Run` 是 **1,835 行**。它现在**挡住了**真正的移植（同签名 `override` 会 CS0111）。 | **登记为硬阻塞**，见 §8-A |

---

## 7. 偏离登记

| # | 位置 | 偏离 | 理由 |
|---|---|---|---|
| **D-P13-A** | `PlayerSurfaceOperateSeams.InheritedOperate` | 用**接缝**表达原文 `inherited Operate(ProcessMsg): Boolean`，而非在本类里重写基类语义 | 托管 `TCreature` 只有 `Operate()`（drain，void）与 `protected Operate(TProcessMessageRef)`（void），**没有**布尔版同名方法。按 §14.2「不造第三份实现」用接缝如实表达"转调基类"，而不是自制一个基类语义的克隆 |
| **D-P13-B** | `PlayerSurfaceSocketSeams` | `SendSocket`/`SendSocketEx`/`SendViewMsg` 是**投递接缝**，默认「无宿主，丢弃」 | 原文该面走 `TUserEngine` + 网关 socket（`ObjBase.pas:30980` 一带 + `UsrEngn.pas`），托管侧未移植。**报文装配仍逐行移植**，故装配结果可测 |
| **D-P13-C** | `PlayerSurfaceWarrContinueConfig` | `g_Config.boDisableWarrContinueHit` / `nWarrContinueHitMinInterval` / `g_WarrContinueMagicIDList` 走**可注入配置接缝**，默认值取"未启用限制"侧 | 三者属 M2Share 的 `TConfig` 与全局 `TList`，托管侧未切出。默认 `BoDisableWarrContinueHit = false` ⇒ 与"配置未装载"时的原文行为一致 |
| **D-P13-D** | `m_DefMsg` 的落点 | 声明为 `TPlayObject` 的实例字段并配 `SendSocketRef`/`SendSocketExRef` | 原文 `m_DefMsg` 属 `TBaseObject`（托管侧未切出该层，`p6` 把 `TBaseObject` 成员落在 `TCreature` 上）。本车道按同一手法落在 `TPlayObject`（它是 `TCreature` 的子类，语义上更靠原文的实际使用点） |
| **D-P13-E** | `TProcessMessage.BaseObject` | 类型是 `nint`（`MsgQueueConsumeCore.cs:15`），不是对象引用 | 原文是 `TBaseObject(ProcessMsg.BaseObject)` **指针强转**；托管无对应安全转换 ⇒ 接缝以 `nint` **原样透传**，不 `as` 强转（`as` 对值类型是编译错误，且会掩盖原文的指针语义） |
| **D-P13-F** | 未新建任何 `TPlayObject` 替身 | 全部新成员以 `partial` 落在**既有** `PlayerSurface/**` 目录 | 任务书 §14.2 + 本车道分区要求"就地扩展，不要另起第二套 `TPlayObject`/`TCreature`" |

---

## 8. 阻塞项（精确到成员名 + 原文行号）

### 8-A ★★ 硬阻塞：`Engine/ObjBase.cs:208` 的近似 `Run()` 必须删除

**现状**：

```csharp
// GXX.CSharp/src/GXX.M2Server/Engine/ObjBase.cs:208-212（本车道只读，未改动）
public override void Run()
{
    base.Run();
    // 心跳：超时踢线由 UserEngine 统一处理
}
```

**为什么挡住**：原文 `TPlayObject.Run`（`ObjPlayer.pas:3772-5606`）返回 **void**，
托管 `TCreature.Run()` 也是 void ⇒ 正确的移植体必须是 `public override void Run()`。
但 `ObjBase.cs:208` 已经有同签名的 `override`，再声明就是 **CS0111 重复成员**。

**要求（二选一）**：

```csharp
// 方案 1（推荐）：删掉 ObjBase.cs:208-212 那 5 行 —— 由本车道在
//                  PlayerSurface/TPlayObject.PlayerSurface.Run.cs（将来）里 1:1 实现。
// 方案 2：把 ObjBase.cs:208 的 override 改成 non-virtual 的私有近似物并改名（如 RunApprox），
//         但这会在虚分派链上留下一个"假 Run"，不推荐。
```

**在落地前**：本车道对 `Run` 只做 `PortNotPorted(nameof(Run), 3772)` 留痕（切片 2），
**不写"近似心跳"冒充移植**（台账 §48.1）。

### 8-B `Run` 的 1,835 行里逐段依赖的未移植面

| 原文行段 | 依赖的未移植成员 |
|---|---|
| :3825-3833 | `DoClientClose()`（声明 :1127，实现未移植）、`m_boDelayClose`/`m_dwDelayCloseTick`/`m_dwDelayCloseTime` |
| :3835-3840 | `g_boExitServer`（M2Share 全局） |
| :3842-3854 | `g_Config.btPlayerVarJClearTime`、`g_Config.boOpenCombatPowerCalc`、`g_Config.boOpenCombatPowerVarCalc`、`RecalcPlayCombatPower(Self)`、`m_nClearDayVarTime` |
| :3859-3887 | `ProcessSafeZoneHint`（声明 :546，实现未移植）、`StopCollect`、`m_dwCollectTick` 族、`m_sInviteGroupHuman`（`THashedStringList`） |
| :3889-5606 | `m_sVerifyCode` 族、`TUserCastle`（`Castle.cs` 有类但缺所需成员）、`TItemObject`、`TCustomMagicConfig`、`PClientBufInfo`/`PArrBufInfo`、`THeroObject`（**托管侧未切出**）、`g_CastleManager`、`GetHighHuman`、`SendMsg(BaseObject, wIdent, ...)`（`p6` 报告 §8-G 登记的"同名不同义"待裁定项） |

**建议**：把 `Run` 拆成 5~6 个子切片按段推进（`:3825-3888` / `:3889-4200` / … / `:5400-5606`），
每段各自登记依赖并各自 `NotPorted`。

### 8-C 若干"巨型方法"的依赖面（供后续派发参考）

| 方法 | 原文行 | 行数 | 主要阻塞面 |
|---|---|---|---|
| `ClientSpell` | 19004 | 1,186 | `TMagic`/`TSpellCaster` 施法面、`g_CustomMagicList` |
| `ClientUseItems` | 21295 | 1,178 | 物品使用表（`UseStdmodeFunItem` 族 11–12 行 × 6）、`m_UseItems` 读写 |
| `ClientUpgradeDialog` | 32712 | 1,082 | 武器升级面板状态机、`g_ItemRules` |
| `Create` | 1486 | 1,022 | **~500 个字段的初始化**（原文一次性赋初值）—— 可**部分移植**：凡字段已存在的赋值可逐行落，缺字段的块按段 `NotPorted` |
| `UserLogon` | 8308 | 825 | 登录流程（DB 交互、`IdSrvClient`、`TPlayObject` 全字段） |
| `SearchViewRange` | 15620 | 534 | `TEnvirnoment` 视野遍历 |

---

## 9. 逐例程对账表（815 条的 已覆盖 / 本车道新增 / 近似物·同名 / 未移植）

> **这张表就是施工图**，也是把 `ObjPlayer` 从 `REFUTED` 升级到 `PARTIAL` 的依据。
> 状态口径见 §3。未标注"本车道新增"的行即仍待移植。
> 注：`TPlayObject` 类块（接口段）另有 **76 条类内嵌套 / 单元级**例程（`ProcessPlayObjectMessage`、
> `CanFilter`、`GetPageCount`、`QuickSort`、`CheckGameMoney`、`FindItem`、`MakeInt64`… 共 76 条），
> 它们**不在这张 811 行的表里**，但同样是翻译单元（清单与行号见 §9.2）。

**完整表格 → [`docs/并行报告-p13-m2-objplayer-逐例程对账表.md`](并行报告-p13-m2-objplayer-逐例程对账表.md)**

> 拆成独立文件的原因：815 行 × 5 列 ≈ 950 行，内联会让本报告难以阅读；
> 两份文件**同源**（都由同一份机械抽取结果生成），抽取命令见 §1。

**总表（四态汇总）**

| 状态 | 例程数 | 占 811 比例 |
|---|---|---|
| 已覆盖（p6 及更早的真 1:1 实现） | 17 | **2.10%** |
| ★本车道新增 | 1（`Operate`；另有 2 条原文**类内嵌套**例程不在 811 表内） | 0.12% |
| 近似物 / 同名（**不算覆盖**） | 46 | 5.67% |
| 未移植（含 `NotPorted` 留痕） | 747 | 92.11% |

> 另：`TWarrContinueHitManager` 的 **4 条已 4/4 落地**（§5 切片 3），
> 它不在 811 表内 ⇒ 本单元**真实体合计 = 17 + 1 + 4 = 22 条**，
> 加 2 条原文内嵌例程 = **24 条**。
> 综合覆盖率（含嵌套例程分母 891）= **24 / 891 = 2.69%**；
> 仅类例程口径（分母 815）= **22 / 815 = 2.70%**。

**按实现段行号分段的分布**（施工进度视角）

| 实现段 | 例程数 | 已覆盖 | 本车道新增 | 近似物 | 未移植 |
|---|---|---|---|---|---|
| L0–L1999 | 1 | 0 | 0 | 1 | 0 |
| L2000–L3999 | 44 | 9 | 1 | 13 | 21 |
| L4000–L5999 | 4 | 0 | 0 | 0 | 4 |
| L6000–L7999 | 30 | 2 | 0 | 3 | 25 |
| L8000–L9999 | 37 | 0 | 0 | 4 | 33 |
| L10000–L11999 | 38 | 1 | 0 | 5 | 32 |
| L12000–L13999 | 44 | 2 | 0 | 9 | 33 |
| L14000–L15999 | 28 | 2 | 0 | 2 | 24 |
| L16000–L17999 | 16 | 1 | 0 | 1 | 14 |
| L18000–L19999 | 3 | 0 | 0 | 0 | 3 |
| L20000–L21999 | 6 | 0 | 0 | 0 | 6 |
| L22000–L23999 | 25 | 0 | 0 | 0 | 25 |
| L24000–L25999 | 38 | 0 | 0 | 0 | 38 |
| L26000–L27999 | 18 | 0 | 0 | 0 | 18 |
| L28000–L29999 | 18 | 0 | 0 | 0 | 18 |
| L30000–L31999 | 17 | 0 | 0 | 0 | 17 |
| L32000–L33999 | 15 | 0 | 0 | 0 | 15 |
| L34000–L35999 | 17 | 0 | 0 | 1 | 16 |
| L36000–L37999 | 70 | 0 | 0 | 0 | 70 |
| **L38000–L39999** | **192** | 0 | 0 | 0 | **192** |
| L40000–L41999 | 29 | 0 | 0 | 0 | 29 |
| L42000–L43999 | 27 | 0 | 0 | 0 | 27 |
| L44000–L45999 | 24 | 0 | 0 | 0 | 24 |
| L46000–L47999 | 47 | 0 | 0 | 5 | 42 |
| L48000–L49999 | 23 | 0 | 0 | 2 | 21 |
| **合计** | **811** | **17** | **1** | **46** | **747** |

> ★ **`L38000–L39999` 一段就含 192 条例程（占 23.7%）而方法体只有 2,034 行**
> —— 与 §4.1 的 `ServerSend*` 结论一致：这一带是**同型小函数密集区**，是最该优先批量推进的段。

### 9.2 类内嵌套 / 单元级例程 76 条（同样属于翻译单元）

抽取命令同 §1，正则改用 `^\s*(function|procedure|constructor|destructor)\s+([A-Za-z_]\w*)`（**无类名前缀**）。
分布：`Operate` 内嵌 2 条（`ProcessPlayObjectMessage`、`CanFilter`，**本车道已移植**）、
`Run` 内嵌 0 条，其余 74 条散布在各 `Client*` 巨型方法的 `var` 段之后
（`GetPageCount` ×4、`CheckGameMoney` ×6、`GetUnbindItemInfo` ×4、`GetUnBindItems` ×4、
`FoundUserItem` ×4、`PackageItem` ×2、`QuickSort`/`SCompare`/`MakeInt64`/`MakeHumInt64_Value1/2`、
`GetAnicountFluteCount`、`GetIdxFluteCount`、`SpliteByte`、`PileStones`、`DoExit` ×2、`IsSelf`、
`IsOfGroup`、`RandomDrua`、`AddContinuousMagic`、`GetRandomMagic`、`GetIndex`、`GetNameAndCount`、
`OverLapItems`、`MyGamePetEatItems`、`CheckCanUpgrade`、`CanUpgrade`、`DecMyGameMoney` ×2、
`IncPlayerGameMoney`、`CheckWealthAnimalMon`、`GotoKillMonsterFunc`、`AdjustAb`、`CanMotaebo` 族、
`GetNewName`、`IncGameMoney` ×2、`DecGameMoney` ×3、`GetPageCount` 等）。

---

## 10. 门禁与提交

| 项 | 值 |
|---|---|
| 构建 | `dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false` → **0 error** |
| 测试 | `dotnet test GXX.CSharp/tests/GXX.M2Server.Tests/GXX.M2Server.Tests.csproj -c Debug --nologo` |
| 裸 `=> true;` 自检 | `powershell -NoProfile -ExecutionPolicy Bypass -File GXX.CSharp/tools/audit-stubs.ps1` → 本车道**新增 0 条** |

**提交纪律**：每个切片立即提交（见 §5 各切片的 commit hash）。
**提交前自检**：`git status --porcelain`（含未跟踪项）**只剩本分区内文件** ——
首轮 `verify-lanes.ps1` 报过我在工作树**根目录**留了 6 个探查转储，**已全部移出仓库**
（迁至 `D:\chuanqi\daima\_p13scratch\`，即 `git rev-parse --show-toplevel` 之外），
结论已汇总进本报告，未入库。

---

## 11. 诚实说明

### 11.1 本车道**未做**的部分

1. **747 条例程未移植**（92.11%）。本车道只完成了基础设施 + 消息派发面 + 一个整类。
   `ObjPlayer.pas` 是 **47,746 行**的方法体，1:1 翻译是**数十个切片**的工程量，
   本会话内不可能完成；已按 §4 切成 21 段给出可直接派发的施工图。
2. **`Run`（1,835 行）只留痕未移植**，且它有一个**跨分区的硬阻塞**（§8-A：需删 `ObjBase.cs:208`）。
3. **`Create`（1,022 行）也未移植** —— 它需要声明约 500 个字段，
   其中大量属未移植的 `TBaseObject`/`TEnhanceObject` 面，硬做会造出第三份对象模型（违反 §14.2）。
4. **一条接缝都没有真正接到宿主上**：`PlayerSurfaceSocketSeams` /
   `PlayerSurfaceOperateSeams.InheritedOperate` / `PlayerSurfaceWarrContinueConfig` 全部默认「无宿主」。
   因此**没有任何端到端行为被验证**（无 `TEnvirnoment` 完整面、无 `TBaseObject`、无客户端管道）。

### 11.2 可信度边界

- **可信**：§1 的实体清点（可复跑的机械抽取）；§2 的名字对账（全 src 37,932 成员名 × 811）；
  §3 的四态分类（每条都带托管落点出处，可逐条 `grep` 复核）；
  §5 已交付切片的**逐行语义**（每条都有独立用例锁死，含 8 处差异/缺陷断言）；
  §6 的 8 条原文缺陷（均带精确 `文件:行`）。
- **不完全可信**：§2 的"近似物"判定用**成员名 + 落点文件名**推断，未逐条读那 46 处的实现体
  （其中约 12 条来自 `.g.cs` 生成壳，按台账 §49.2 的口径**必然**不是实现，可高置信度排除；
  其余约 34 条建议由复核车道抽查）。
- **未验证**：任何真实宿主下的端到端玩家会话行为。

### 11.3 基线漂移

本车道工作树 = `8d359e9c`（派发时的 main），**未 rebase**。
`GXX.slnx` / 所有 `*.csproj` / `Directory.Build.props` / `docs/Checklist.md` /
`docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `tools/**` **一律未改**（可用 `git diff --stat` 复核）。

---

*报告完。分支 `par/p13-m2-objplayer`，工作树 `.worktrees/p13-m2-objplayer`。*
