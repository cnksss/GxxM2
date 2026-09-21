# 并行报告 — 车道 `p10-client-scrn`（客户端屏幕提示/飘字绘制族）

**任务**：把 `DrawScrn.pas`（5,542 行）+ `DropItemsMgr.pas`（570 行）1:1 翻译为 C#
**分支**：`par/p10-client-scrn`　**工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p10-client-scrn`
**基线**：main @ `c078f78d`

---

## 0. 门禁实跑（两条命令的实际输出摘要）

| 门禁 | 命令 | 结果 |
|---|---|---|
| build | `dotnet build GXX.CSharp/GXX.slnx -c Debug --nologo -m:1 -p:BuildInParallel=false` | **0 个错误** / 174 个警告 |
| test | `dotnet test GXX.CSharp/tests/GXX.Client.Tests/GXX.Client.Tests.csproj -c Debug --nologo` | **失败 0 / 通过 4824 / 总计 4824** |

**测试增量对账（实跑取证）**：

```
不含本车道用例： dotnet test --filter "FullyQualifiedName!~ScrnDraw"  → 通过 4679 / 总计 4679
含本车道用例 ： dotnet test                                        → 通过 4824 / 总计 4824
本车道新增   ： 4824 − 4679 = 145 例（全部新增，全部通过，未改动任何既有用例）
```

---

## 1. 全部 commit

| hash | 切片 | 内容 |
|---|---|---|
| `0f942b83` | 1 | 环境接缝 + 文本 token 族 + `THintMessage` 族 + `THintLines`/`ProcessHintText` + `THintWindow`/`THintWindows` |
| `c1282415` | 2 | 系统消息族 / 滚动延迟消息族 / 新式消息族（11 个类） |
| `822f9e05` | 3 | `TDrawScreenScrn`（TDrawScreen 全成员）+ MShare 场景全局接缝 |
| `d08a83ee` | 4 | `DropItemsMgr.pas` 整单元 1:1 |
| `965c80d0` | 5 | `ScrnDrawHintTests`（56 例） |
| `872e6368` | 6 | `ScrnDrawMsgTests`（58 例）+ 修正 `DrawScreen` 英雄段 `Format` 占位符 |
| `04159f4a` | 7 | `ScrnDrawDropItemsTests`（31 例） |

---

## 2. 新增文件

| 文件 | 行数 | 说明 |
|---|---|---|
| `src/GXX.Client/Scenes/DrawScrn/DrawScrnEnv.cs` | 729 | 单元常量 / 共享记录 / 尚无归属的基础类型 / 全部外部全局接缝 / headless 画布 |
| `src/GXX.Client/Scenes/DrawScrn/DrawScrnText.cs` | 601 | `TTokenType`/`TStringToken`/`TStringLineEx`/`GetStrinLineExText`/`GetTextListEx`(两重载)/`GetTextListEx2` |
| `src/GXX.Client/Scenes/DrawScrn/HintMessageFamily.cs` | 1936 | `THintMessage` 族 12 类 + `THintLines` + `ProcessHintText` |
| `src/GXX.Client/Scenes/DrawScrn/HintWindowFamily.cs` | 651 | `THintWindow` / `THintWindows` |
| `src/GXX.Client/Scenes/DrawScrn/SysMsgFamily.cs` | 414 | `TDrawSysMsg` / `TDrawSysMsgEx` / `TDrawMoveHintMsg` |
| `src/GXX.Client/Scenes/DrawScrn/MoveMsgFamily.cs` | 850 | `TDrawScreenCenterMsg` / `TDrawDelayMsg` / `TDrawScreenMoveMsg` / `TScreenMoveMsgList` |
| `src/GXX.Client/Scenes/DrawScrn/NewMsgFamily.cs` | 1119 | `TDrawScreenCenterNewlineMsg` / `TDrawScreenNewMoveMsg` / `TScreenNewMoveMsgList` / `TMoveHintMsgList` / `TScreenNewLineMsgList` |
| `src/GXX.Client/Scenes/DrawScrn/TDrawScreenScrn.cs` | 592 | `TDrawScreenScrn`（= 原文 `TDrawScreen` 全成员；见 §4） |
| `src/GXX.Client/Scenes/DrawScrn/DropItemsMgr.cs` | 630 | `TDropItem` / `TDropItemEffect` 用法 / `TPointDropItemList` / `TDropItemsMgr` + 接缝 |
| `tests/GXX.Client.Tests/ScrnDrawHintTests.cs` | 880 | 56 例 |
| `tests/GXX.Client.Tests/ScrnDrawMsgTests.cs` | 1149 | 58 例 |
| `tests/GXX.Client.Tests/ScrnDrawDropItemsTests.cs` | 561 | 31 例 |
| `docs/并行报告-p10-client-scrn.md` | 本文件 | — |

> **未改动**任何既有文件。取证命令与结果：
> ```
> git diff --name-only c078f78d..HEAD          → 13 个文件，**全部为新增**（10 个 src + 3 个 tests，+ 本报告）
> git status --porcelain                       → 空
> ```
> 即 `GUI/Share/**`、`Scenes/Scenes.cs`、`Scenes/DropItemFx.cs`、`Scenes/PlaySceneCore.cs`、
> `GXX.slnx`、任何 `*.csproj`、`Directory.Build.props`、`tools/**`、`docs/Checklist.md`、
> `docs/并行派发台账.md`、`docs/并行覆盖审计.md` **一个字节都没动**。

---

## 3. 逐单元「类数 / 方法数 / 已移植数」

### 3.1 `DrawScrn.pas`（5,542 物理行；interface 1-737，implementation 739-5542）

**方法计数（脚本 `^\s*(procedure|function|constructor|destructor)\s+T\w+\.\w+` 实测）**：

| 类（行号） | 原文方法数 | 已移植 | 落地文件 |
|---|---|---|---|
| `THintMessage`(99) | 3 | 3 | HintMessageFamily.cs |
| `THintImage`(115) | 2 | 2 | 同上 |
| `TWinHintImage`(136) | 2 | 2 | 同上 |
| `TLineBGHintImage`(156) | 2 | 2 | 同上 |
| `TFiexdHeightLine`(177) | **0（原文空类）** | 0 | 同上 |
| `THintPlayImage`(180) | 2 | 2 | 同上 |
| `THintPlayImageEx`(208) | 2 | 2 | 同上 |
| `THintItemProgress`(216) | 2 | 2 | 同上 |
| `THintText`(246) | 7 | 7 | 同上 |
| `THintFixedWidthText`(272) | 1 | 1 | 同上 |
| `TCountdownText`(279) | 3 | 3 | 同上 |
| `THintImageNumber`(293) | 2 | 2 | 同上 |
| `THintLines`(318) | 15 | 15 | 同上 |
| *(单元级)* `ProcessHintText`(735/1453) | 1 | 1 | 同上（`DrawScrnHintTextParser.ProcessHintText`） |
| `THintWindow`(377) | 12 | 12 | HintWindowFamily.cs |
| `THintWindows`(408) | 12 | 12 | 同上 |
| `TDrawSysMsg`(627) | 5 | 5 | SysMsgFamily.cs |
| `TDrawSysMsgEx`(642) | 6 | 6 | 同上 |
| `TDrawMoveHintMsg`(661) | 5 | 5 | 同上 |
| `TDrawScreenCenterMsg`(430) | 5 | 5 | MoveMsgFamily.cs |
| `TDrawDelayMsg`(524) | 6 | 6 | 同上 |
| `TDrawScreenMoveMsg`(539) | 10 | 10 | 同上 |
| `TScreenMoveMsgList`(564) | 8 | 8 | 同上 |
| *(单元级)* `GetTextListEx2`(3540) | 1 | 1 | DrawScrnText.cs |
| `TDrawScreenCenterNewlineMsg`(456) | 8 | 8 | NewMsgFamily.cs |
| `TDrawScreenNewMoveMsg`(484) | 6 | 6 | 同上 |
| `TScreenNewMoveMsgList`(580) | 6 | 6 | 同上 |
| `TMoveHintMsgList`(603) | 6 | 6 | 同上 |
| `TScreenNewLineMsgList`(615) | 6 | 6 | 同上 |
| `TDrawScreen`(674) | **29** | **29** | TDrawScreenScrn.cs（类名差异见 D-P10-01） |
| **合计** | **173 类方法 + 2 单元级过程 = 175** | **175（100%）** | — |

**类覆盖率**：原文 interface 段 **28 个类**（脚本 `^\s{0,4}T\w+\s*=\s*class` 实测 28 条声明，行号
99/115/136/156/177/180/208/216/246/272/279/293/318/377/408/430/456/484/524/539/564/580/603/615/627/642/661/674）。
诊断时（`c078f78d`）其中 **24 个在托管工程里 `<none>`**。当前状态：

* **27 / 28 个类以原文类名落地**（`THintMessage` … `TDrawSysMsgEx`、`TDrawMoveHintMsg`）；
* **第 28 个 `TDrawScreen` 以 `TDrawScreenScrn` 落地**（成员 100% 覆盖，29/29 方法），
  原因与合并步骤见 §4 与 **D-P10-01**；
* 另有 **2 处**本来只在 `GUI/Share/FStateSeams.cs` 里的**接缝**（`THintLines`/`THintWindows`）现已由
  正式归属接管 —— 但**接缝本体仍在**（本车道被禁止改 `GUI/Share/**`），见 §5 与 **D-P10-06**。

### 3.2 `DropItemsMgr.pas`（570 行）

| 类（行号） | 原文方法数 | 已移植 |
|---|---|---|
| `TPointDropItemList`(61) | 8 | 8 |
| `TDropItemsMgr`(89) | 18 | 18 |
| **合计** | **26** | **26（100%）** |

* `TDropItem`(14-56) 是 `record`（无方法），以 `sealed class` 落地；
* `pTDropItem` / `pTDelayMsg` / `pTMoveMsg` / `pTSysMsg` 等指针类型以**引用类型 + `using` 别名**承载；
* 诊断时整单元 `<none>`，现已全部有声明。

---

## 4. ★ 关系 1：与 FState 的「同一份 vs 两份」判定

### 4.1 判定：**同一份**。`FStateSeams.cs` 里的那两个是**接缝**，正式归属是 `DrawScrn.pas`。

**逐条证据（回读原文 + 回读既有代码）**：

| 证据 | 内容 |
|---|---|
| E1 | `GUI/Share/FStateSeams.cs:407-409` 原文写着 `/// DrawScrn.pas:318 THintLines（提示文本行容器）。` + `【接缝：待 DrawScrn.pas 移植后接入】本波次只复刻 GetHitLines 与解构循环用到的成员。` |
| E2 | `FStateSeams.cs:410-411` 自己写明：「车道8 已在其 `TStateWindowsText.cs` 中登记同一缺口，但归属为 `DrawScrn.pas`，**不是** FState.pas —— 见本车道交付报告的"既往车道事实更正"一节」。 |
| E3 | `DrawScrn.pas:318` 与 `:408` 分别是 `THintLines = class` / `THintWindows = class` 的**唯一**声明点（脚本实测：全 Client-HGE 镜像里 `THintLines` 只在 `DrawScrn.pas` 声明）。 |
| E4 | `FState.pas` **不声明**它们，只是**使用**：`FStatePure.cs:75 GetHitLines(THintLines HintLines, string S, TColor DefColor)`；`FState.pas` 的 `uses` 里有 `DrawScrn`。 |
| E5 | 台账 §12.6（第 602-609 行）已由 `p2-client-fstate` 车道**正式更正**：`THintLines`「实为 `DrawScrn.pas:318`」，此前多处接缝注释把它归给 FState.pas 是**错的**。 |
| E6 | `FStateSeams.cs:599-602` 的 `THintWindows` 是**空类**（`public class THintWindows { }`），`FStateSeams.cs:608-612` 的 `static class DrawScrn` 只有一个 `CreateHintWindows()`；两者都标注 `【接缝：待 DrawScrn.pas 移植】`。 |

### 4.2 本车道的处置（**没有改** `GUI/Share/**`）

正式实现落在**本车道独占命名空间** `GXX.Client.Scenes`（`HintMessageFamily.cs` / `HintWindowFamily.cs`），
与 `GXX.Client.GUI.Share` 里的接缝**同名但不同命名空间**。

★ **实测证据（该接缝确实会造成 CS0104）**：本车道的测试文件最初同时 `using GXX.Client.GUI.Share;`
与 `using GXX.Client.Scenes;`，编译直接报 **24 处**
`CS0104: "THintLines"是"GXX.Client.Scenes.THintLines"和"GXX.Client.GUI.Share.THintLines"之间的不明确的引用`。
最终用 `using THintLines = GXX.Client.Scenes.THintLines;` 消除。这正是台账 §12.8 记的"重复接缝碰撞"模式的**第四次复发**，
根因就是**接缝没有被正式归属取代**。

### 4.3 **最小改法（交集成方执行，不改任何 FState 逻辑）**

```
① 删除 FStateSeams.cs:407-452  整段（GXX.Client.GUI.Share.THintLines 及其 HintLine 结构）
② 删除 FStateSeams.cs:599-612  整段（GXX.Client.GUI.Share.THintWindows + static class DrawScrn 接缝）
③ 给下列文件各加一行 using（或在调用点写全名 GXX.Client.Scenes.THintLines / THintWindows）：
     src/GXX.Client/GUI/Share/FStatePure.cs               （GetHitLines 的形参）
     src/GXX.Client/GUI/NewStateWin/TStateWindowsText.cs  （文档注释里的类型引用）
     tests/GXX.Client.Tests/GuiSharePureTests.cs          （第 59 行 NewHintLines()）
④ FStateSeams.cs:612 的 DrawScrn.CreateHintWindows() 调用点（TFrmDlg 侧）改为
     new GXX.Client.Scenes.THintWindows()
```
改完预计影响：**3 处调用点 + 1 处 Create**，**零逻辑改动**（`GetHitLines` 的方法体一行不动 —— 它的形参类型换名后
`Add(text, color, size, style, stroke)` 仍然成立，见下）。

★ **签名兼容性已核对**：接缝的 `THintLines.Add` 与正式归属的 `Add` 签名不同 ——

| | `GUI.Share` 接缝 | 本车道正式归属 |
|---|---|---|
| `Add` 返回值 | `int`（返回行下标） | **`void`**（原文 2015 是 `procedure`） |
| 重载 | `Add(text,color)` + `Add(text,color,size,style,stroke)` | `Add(s, color, fontSize=9, fontStyles=[], isStroke=false, fontName='')` 一个 6 参带默认值 |

`FStatePure.cs:75-…` 的 `GetHitLines` 调用的是 5 参形态且**不用返回值**（需在③时一并核对；
若确实用了返回值，则 `Add` 需补一个返回下标的重载 —— 这属于集成时的**一行**工作，本车道无法验证，
因为 `FStatePure.cs` 不在本车道分区）。**此点已作为 D-P10-06 的附带条件登记。**

---

## 5. ★ 关系 2：与 `Scenes.cs` 的 `TDrawScreen` 逐成员对照

### 5.1 逐成员对照表（`Scenes.cs:693-743` 实测）

| `Scenes.cs` 成员 | `DrawScrn.pas` 出处 | 判定 |
|---|---|---|
| `CurrentScene` | :690 `CurrentScene:TScene` | **重叠**（同源） |
| `ChangeScene(TSceneType)` | :4465-4506 | **重叠但不等价**：Scenes 版**缺** `{$IF IsMultiThreadRender=1}` 临界区（该符号未定义 ⇒ 本车道同按未定义分支），且把 `CurrentScene.OpenScene` 后的判定写成 `CurrentScene == LoginScene`（用的是字段），本车道按原文用 MShare 全局 |
| `KeyPress(ref char)` | :4441-4445 | **重叠**（同源） |
| `KeyDown(ref ushort)` | :4447-4451 | **重叠但丢参**：Scenes 版传 `new object()`，**丢弃 `TShiftState`**；本车道 `KeyDown(ref ushort, TShiftState)` |
| `WelcomeScene` / `LoginScene` / `SelectChrScene` / `LoginNoticeScene` / `PlayScene`（字段） | **不在 `TDrawScreen` 里** | **越界**：原文这 5 个是 **MShare.pas:1457-1461 的单元级全局**（脚本实测声明点在 `MShare.pas`）；本车道按原文改用 `DrawScrnEnv.WelcomeScene` … |
| `ShowLoginSceneShowRandomCodeDlg` / `OpenRandomCodeDlg` | :683 私有 `m_boShowLoginSceneShowRandomCodeDlg`；:4498 `FrmDlg.OpenDRandomCodeDlg` | **形态偏离**：原文是**私有字段 + FrmDlg 调用**；本车道按原文 |
| **其余 24 个成员** | :674-733 | **`Scenes.cs` 里全无声明** |

「其余 24 个」逐条（原文行号 → 本车道成员）：
`m_dwFrameTime`(676)、`m_dwFrameCount`(677)、`m_SysMsgList`(679)、`m_SysMsgListEx`(680)、`m_boInitialize`(681)、
`m_boShowLoginSceneShowRandomCodeDlg`(683)、`FScreenMoveMsgList`(685)、`FScreenNewMoveMsgList`(686)、
`FScreenNewLineMsgList`(687)、`FMoveHintMsgList`(688)、`HintList`(692)、`HintX/HintY/HintWidth/HintHeight`(693)、
`HintUp`(694)、`HintColor`(695)、`DrawDelayMsg`(696)、`DrawScreenCenterMsg`(697)、`Create`(699)、`Destroy`(700)、
`MouseMove`(703)、`MouseDown`(704)、`Initialize`(706)、`Finalize`(707)、`Update`(708)、`AddSysMsg`(711)、
`AddChatBoardString`(714)、`AddTopChatBoardString`(715)、`ClearChatBoard`(716)、`AddMoveMsg`(717)、
`AddNewMoveMsg`(718)、`AddNewLineMsg`(719)、`AddMoveHintMsg`(720)、`ShowHint`(722)、`ClearHint`(723)、
`DrawScreen`(725)、`DrawMsg_TopLevel`(726)、`DrawSysMsg_BottomLevel`(727)、`DrawHint`(728)、`DrawMove`(729)、
`DrawMoveBefor`(730)、`SetShowLoginSceneShowRandomCodeDlg`(732)。

### 5.2 结论与处置

**是同一份**（不是两份），但 `Scenes.cs` 那一份是**早期车道落的部分成员**（且含 6 处越界/形态偏离）。
按任务书「若 `TDrawScreen` 不是 `partial` 则在报告里登记」执行 —— **本车道没有改 `Scenes/Scenes.cs`**：

* 完整 1:1 实现落在 `TDrawScreenScrn`（`Scenes/DrawScrn/TDrawScreenScrn.cs`，592 行，29/29 方法）；
* 二者**类名不冲突** ⇒ 当前门禁可绿；
* 合并说明见 **D-P10-01**。

---

## 6. ★ 关系 3：与 `Scenes/DropItemFx.cs` 的关系

### 6.1 成分拆分（回读实测）

`DropItemFx.cs`（429 行）里有两类东西：

| 成分 | 来源单元 | 是否被本车道取代 |
|---|---|---|
| `DropItemEffectDef`(7)、`FxImage`(24)、`DropItemFxState`(27)、`DropItemDrawOp`(39)、`DropItemFx`(46) | **PlayScn.pas**（掉落物特效帧/闪烁节拍/名字定位） | **不取代** |
| `PointDropItemList`(190)、`ShowItemInfo`(245)、`DropItem`(252)、`DropItemsStore`(312) | **DropItemsMgr.pas**（文件头 :309 自述「`TDropItemsMgr`（246-560）headless 镜像」） | **已被取代** |

### 6.2 为什么**没有删**（任务书要求的"如实登记"分支）

`DropItemsStore` **被分区外的文件引用**：

```
src/GXX.Client/Scenes/PlaySceneCore.cs:194        ← 引用 DropItemsStore
tests/GXX.Client.Tests/FormJ52bTests.cs:155/156/162/186/199
tests/GXX.Client.Tests/FormJ72Tests.cs:613/615
tests/GXX.Client.Tests/FormJ73Tests.cs:1189/1195
```

本车道的例外授权只有 `Scenes/DropItemFx.cs` 一个文件；删掉 `DropItemsStore` 会让 `PlaySceneCore.cs`
与 3 个既有测试文件**编译失败 ⇒ 门禁变红**。因此按任务书「否则如实登记」处理，见 **D-P10-02**。

`PointDropItemList` / `DropItem` 与本车道的 `TPointDropItemList` / `TDropItem` **名称不冲突**（原文带 `T` 前缀），
故当前门禁可绿。（顺带说明：`DropItemFx.PointDropItemList` 的 `RefreshDrawList()` **无参**，
而原文是 `RefreshDrawList(ResetShowItem:Boolean=False)` —— 替身**丢了 `PlugInEnabled + ResetShowItem`
重查 `g_FileItemDB` 这一支**，本车道的 1:1 实现把它补回来了。）

---

## 7. 原文缺陷清单（照抄 + 差异断言锁死）

> 编号用 `D-P10-D<n>`（D = Defect），与 §8 的**偏离登记** `D-P10-<n>` 分开，避免两套编号撞车。

| 编号 | 位置 | 缺陷 | 处置 / 断言 |
|---|---|---|---|
| **D-P10-D01** | `THintLines.GetSize` 1385-1418 | `MaxWidth` 只被初始化为 0 后**再无写入** ⇒ `if FWidth < MaxWidth` 是**死分支** | 保留同形（`maxWidth` 局部变量），注释标注；否定性断言见 §9-③ |
| **D-P10-D02** | `TDrawSysMsgEx.Draw` 2823-2914 | 一整份旧实现（`nShowTime/nDeleteTime/nMinShowTime/OffsetY_Step` 版）被 `(* *)` **整段注释掉** | **不移植**，仅登记；生效版是 2916-3026 |
| **D-P10-D03** | `THintText.SetCaption` 775-781 | `property Caption` 的写访问器是**字段本身**（265 行 `write FCaption`），`SetCaption` 全文**无调用点** ⇒ 死代码 | 保留同形（`protected void SetCaption`）；断言：`t.Caption = "..."` **不**触发 `Initialize`（`THintText_SetSizeReinitializes_ButSetStyleDoesNot`） |
| **D-P10-D04** | `TDropItem.Name/DBName` :30-31 | `string[60]` **短字符串**：赋值时按 **GBK 字节**静默截断到 60（31 个汉字 = 62 字节 → 只剩 30 个） | **复刻**（`DropItemsMgrEnv.SetShortString`）；断言 `DropItem_ShortString60_TruncatesByGbkBytes` |
| **D-P10-D05** | `ProcessHintText` 1754-1765 + 1978 | `<TextW:X:Y:TEXT>` 把 **OffsetY 当成颜色索引**传给 `NewFixedWidthHintText(…, AColor:=OY)` ⇒ `Color := GetRGB(OY)` | 复刻；断言 `ProcessHintText_TextW_UsesOffsetXAsFixedWidth_AndOffsetYAsColorIndex` |
| **D-P10-D06** | `THintWindow.ShowColor` 2246-2253 | 颜色段在 **`/` 之前**（`颜色索引/文字`）—— 与 `Show`（`文字`）方向相反，极易误用；颜色段解析失败回退 `GetRGB(255)` | 复刻；断言 `THintWindow_ShowColor_ColorIndexComesFirst`（含回退 255 的用例） |
| **D-P10-D07** | `TMoveHintMsgList.Destroy` 4107-4121 | 析构循环体**为空** ⇒ 元素不 `Dispose`（内存泄漏）；`TDrawSysMsg.Clear`(2766) 同样只 `Clear` 不 `Dispose`；`TDrawScreenMoveMsg.Update` 的 `try/except` 吞异常 | 复刻 + 注释；断言 `TMoveHintMsgList_Clear_And_FreeIsNoOpLoop` |
| **D-P10-D08** | `THintImageNumber.Paint` 1352 | 用**属性** `Height`（= `FHeight`）而不是 `Texture.Height` 做垂直居中 | 复刻（`py + (Height - texture.Height)/2`），注释标注 |
| **D-P10-D09** | `THintWindow.Draw` 2428-2452 / `TDrawScreenMoveMsg.Update` 3730-3754 | `case` 语句**没有 `else`**：枚举越界时 `LineRect` / `DestRect` 沿用上一轮的值 | 复刻（`default:` 分支不赋值），注释标注 |
| **D-P10-D10** | `TDrawScreen.DrawScreen` 4739-4808 | 绿色信息**只在** `PlugInEnabled and boShowGreenHint and ConfigCheckeds[ckShowGreenHint]` 三重门下生成；但 `$04` 攻城区域段**在该门之外**，却用同一 `Str` 判空 ⇒ 插件关闭时 `Str` 恒空 | 复刻；断言 `TDrawScreenScrn_DrawScreen_AreaStateIconsAndSiegeText` |
| **D-P10-D11** | `TPointDropItemList.RefreshDrawList` 225-241 | 第二轮补足**只判 `Visible`，不判 `ShowItem`** ⇒ "名字不可显示"的可见项照样占绘制位（与第一轮的语义不一致） | 复刻；断言 `RefreshDrawList_SecondPassIgnoresShowItemAndOnlyChecksVisible` |

---

## 8. 偏离登记（D-P10-01 … D-P10-14）

| 编号 | 类型 | 内容 | 影响 | 建议 |
|---|---|---|---|---|
| **D-P10-01** | **跨区请求** | `TDrawScreen` 原文成员落在 `TDrawScreenScrn`（`Scenes.cs:694` 的 `public class TDrawScreen` **没有 `partial`**，且该文件不在本车道分区） | 名称与原文不同；成员 100% 覆盖、方法体零改动 | **集成方最小改法**：给 `Scenes.cs:694` 加 `partial`，删掉其中与原文重叠的 4 个成员（`CurrentScene`/`ChangeScene`/`KeyPress`/`KeyDown`）与 6 个越界成员（5 个场景字段 + `ShowLoginSceneShowRandomCodeDlg`/`OpenRandomCodeDlg`），把 `TDrawScreenScrn.cs` **整文件**搬入并把类名改回 `TDrawScreen`（**方法体一行不用改**）。 |
| **D-P10-02** | **跨区请求** | `DropItemFx.cs` 的 `PointDropItemList`/`DropItem`/`ShowItemInfo`/`DropItemsStore` 已被本车道取代，但**不能删**（`PlaySceneCore.cs:194` + 3 个测试文件在分区外引用 `DropItemsStore`） | 托管工程暂时存在两套掉落物容器 | **集成方最小改法**：① 删 `DropItemFx.cs` 的 `DropItemFx.cs:190-429`（`PointDropItemList`/`ShowItemInfo`/`DropItem`/`DropItemsStore` 四段）；② `PlaySceneCore.cs:194` 改指向 `GXX.Client.Scenes.TDropItemsMgr`；③ 四个测试文件（`FormJ52bTests`/`FormJ72Tests`/`FormJ73Tests`）改用 `TDropItemsMgr`/`TDropItem`。**`DropItemFx` 静态类（PlayScn.pas 那半）必须保留。** |
| **D-P10-06** | **接缝合并** | `GUI/Share/FStateSeams.cs:407-452 / 599-612` 的 `THintLines`/`THintWindows`/`DrawScrn` 接缝仍在（本车道被禁止改 `GUI/Share/**`） | 同时 `using` 两个命名空间会 **CS0104**（本车道测试实测 24 处） | 见 §4.3 的四步最小改法。**附带条件**：搬迁时需核对 `FStatePure.cs:75 GetHitLines` 是否使用了接缝 `Add` 的 **`int` 返回值**（正式归属的 `Add` 是 `void`，1:1 原文）。 |
| **D-P10-07** | 类型映射 | 原文 `Classes.TList` → 托管 `List<object>`（`Items[I]`→`[I]`、`Delete(I)`→`RemoveAt(I)`、`Add`/`Count`/`Clear`/`IndexOf` 同名） | 未复用 `GUI.Share` 的 `TList` 接缝：**它缺 `Insert`**（`THintLines.Insert` 需要） | 若集成方要把 `TList` 接缝补上 `Insert`，可整体替换为 `GXX.Client.GUI.Share.TList`（`using` 别名即可），本车道已全部用 `List<object>`。 |
| **D-P10-08** | 可见性放宽 | 原文 `ProcessHintText` 是**单元级过程**，直接写各类的 `private`/`protected` 字段（Delphi 同单元可见）；托管侧这些字段一律落为 `public` | 仅可见性放宽，**不改任何控制流与数值** | 若日后要收紧，可改为 `internal`（本工程测试在另一程序集，故本轮取 `public`）。 |
| **D-P10-09** | 命名 | `THintWindows.Finalize` → `Finalize_()`；`TDrawScreen.Finalize` → `Finalize_()`；`TScene` 已有的 `Finalize()` 同名冲突 | `System.Object.Finalize` 是保留成员 | 工程既有先例：`TGameImages.Finalize_()`。 |
| **D-P10-10** | 记录→类 | 原文 `record` + `^` 指针（`New/Dispose` 堆对象）→ 托管 `sealed class`（引用语义一致）；`pTMoveMsg`/`pTDelayMsg`/`pTSysMsg`/`pTDropItem`/`PDrawScreenNewMsgCacheText`/`PMoveHintMsgRecord` 以 **`using` 别名**保留原名 | 无行为差异 | — |
| **D-P10-11** | 加固 | `GetCachedImage(Index, out X, out Y)` 原文签名托管侧缺失（`TGameImages` 接缝只有索引器） | 默认实现退化为 `Images[Index]` 且 **X=Y=0**，可注入 `GetCachedImageFn`；**不隐瞒**（`DrawScrnEnv.GetCachedImage` 注释 + 本表） | 待 `GameImages.pas` 全量移植后把 `GetCachedImageFn` 默认值换成真实现；断言 `THintPlayImage_UsesCachedOriginOffset_ExDoesNot` 已把"用了原点偏移"锁死。 |
| **D-P10-12** | 托管差异 | `TPointDropItemList.GetDrawItems` 原文**不判越界**（187-190），托管 `List<T>` 抛 `ArgumentOutOfRangeException` | 越界从"未定义行为"变成"确定性异常" | 断言 `GetDrawItems_OutOfRange_ThrowsInManaged` 已锁死。 |
| **D-P10-13** | 未启用分支 | `Makecode_T`/`Cutecode_T` 按 `SDK.pas:27 ENCRYPOINT = 0` 取**恒等**分支；`ENCRYPOINT=1` 的加扰式（`(pstr xor a)*10+a` / `(pstr div 10) xor t`）**以注释保留**在方法体内 | 与当前编译配置一致 | 若将来切到 `ENCRYPOINT=1`，只需启用注释里的三行。 |
| **D-P10-14** | 接缝新造 | 尚无归属的：`TAlignment`、`TImageInfo`、`TTokenType`/`TStringToken`/`TStringLineEx`（DxMemo.pas:238-266）、`GetStrinLineExText`/`GetTextListEx`（DxMemo.pas:2202-2524）、`TDrawScrnCanvas`、`DrawScrnEnv`/`DropItemsMgrEnv` 的全部全局 | 见 §10 归属申请 | — |
| **D-P10-15** | 接缝默认值 | `DrawScrnEnv.IsValidActorExFn` 默认 **恒真**（原文 `PlayScn.IsValidActorEx` 未移植）；`DrawScrnEnv.FindFontFn`/`FontTextWidthFn`/`FontTextHeightFn` 默认走 `GUI.Mir.THGEFont` 的占位实现（宽 = 6×字符数） | 两者都是**未接线**而非"分支没命中"；报告 §8.1 已登记归属，`ResetForTests()` 可复位 | 待 `PlayScn.pas` / `HGEFontEx.pas` / `MShare.pas` 落地后替换默认值；`IsValidActorEx` 的恒真会**放宽** `DrawScreen` 的绿色信息守卫（只影响显示，不影响协议） |
| **D-P10-16** | 编译期分支 | `{$IF IsMultiThreadRender = 1}` 的 `EnterCriticalSection(g_CriticalSection)`（`ChangeScene` 4467-4505 / `DrawScreen` 4720-4728）**按未定义分支**移植（不加锁） | 与当前编译配置一致；两处均在代码里以注释保留原文 | 若将来 `IsMultiThreadRender` 置 1，需补 `g_CriticalSection` / `g_ActorLock` 接缝 |

### 8.1 归属申请（供集成方排期）

| 本车道载体 | 正式归属 | 依据 |
|---|---|---|
| `TTokenType` / `TStringToken` / `TStringLineEx` / `GetStrinLineExText` / `GetTextListEx`（两重载） | `GXX.Client.DxComponent`（`DxMemo.Text.cs`） | `DxMemo.pas:238-266 / 2202-2224 / 2223-2389 / 2394-2524`；`DxComponent/DxMemo.cs:16` 的头部注释已把它们列为「待移植」且 `DxMemo.Text.cs` 尚不存在 |
| `TImageInfo` / `TImageIndexs` | `GXX.Client.DxComponent`（HGEFontEx.pas 侧） | `HGEFontEx.pas:15-23`；`DxComponentCommon.cs:16` 已声明该单元归属 |
| `TDrawScrnCanvas`（含 `BoldTextOut*` / `FontTextOut`） | `GXX.Client.HGE`（真实 `GameCanvas`/`THGEFont`）；并与 `GXX.Client.GUI.Mir.MShareGlobals.GameCanvas`（另一车道的接缝）**合并为同一个** | `HGE.pas` / `ClFunc.pas`；目前工程里 `MShareGlobals.TGameCanvas` 只有 3 个方法（`Draw`×2 / `FillRect`），**缺** `StretchDraw`/`FillRectAlpha`/`DrawBlend`/`TextRect`，本车道无法扩展它（不在分区） |
| `DrawScrnEnv.HintWindowBorderWidth` / `boShowHintWindowFrame` / `btHintWindowbackgroundColor` / `btHintWindowbackgroundAlpha` / `boShowGreenHint` / `boGreenHintNewStyle` / `boHumStruckShowNumber` / `boMonStruckShowNumber` | `GXX.Client.GUI.Mir.TConfigClient`（MShare.pas `g_ClientConfig`） | `MShare.pas:493 TConfigClient`；车道 GUI/Mir 的 `TConfigClient`（`MirForms.cs:23`）目前**没有**这些字段 |
| `DrawScrnEnv.HintWindows`（MShare.pas:1464 全局） | `GXX.Client.GUI.Mir.MShareGlobals` | `MShare.pas:1464 HintWindows:THintWindows` |
| `DrawScrnEnv.WelcomeScene/LoginScene/SelectChrScene/PlayScene/LoginNoticeScene` | `GXX.Client.GUI.Mir.MShareGlobals` | `MShare.pas:1457-1461` |
| `DrawScrnEnv.g_FocusCret` / `g_MyHero` / `g_nMouseCurrX` / `g_nMouseCurrY` / `g_nMouseX` / `g_nMouseY` / `g_nAreaStateValue` / `g_sGoldName` / `g_nMoveMouseX` / `g_nMoveMouseY` / `ConfigCheckedFn` / `PlugInEnabled` / `IsValidActorExFn` | `GXX.Client.GUI.Mir.MShareGlobals` / `GameConfigDlg` / `PlayScn` | `MShare.pas`（逐条已在代码注释标出） |
| `DropItemsMgrEnv.g_DropItemEffectList` | `GXX.Client.GUI.Mir.MShareGlobals` | `MShare.pas:2229 g_DropItemEffectList:TDropItemEffectList = nil`（13392 Create） |
| `DropItemsMgrEnv.SetShortString` | `GXX.Core.Protocol`（短字符串助手） | 已有 `ShortStr.Set(buf, offset, capacity, value)`；本车道的 `SetShortString(value, capacity)` 是其 **string→string** 便利形态，建议并入 `ShortStr` |

---

## 9. 否定性断言的计数取证（§37.3）

① **`CheckHintImage` 只有 13 个 tag 分支** —— 脚本枚举原文 1711-1930 的 `if/else if` 链：
`TextW`(1754)、`Img`(1766)、`Looks`(1781)、`DnItems`(1791)、`StateItem`(1801)、`NewopPlayImg`(1814)、
`NewopUI`(1835)、`WinNewopUI`(1844)、`LineNewopUI`(1856)、`PlayImg`(1869)、`ItemProgress`(1893)、
`ImgNum`(1909)、`Countdown`(1921) = **13 条**。
断言 `CheckHintImage_AcceptsExactlyThirteenTags` 对 **13/13** 全部返回 True，并对 3 个反例
（`<Foo:1:2>` / `<>` / `<Countdown>`）返回 False，另有 `Assert.Equal(13, tags.Length)` 把计数写进用例。

② **`ProcessHintText` 的 `IsWinImage`/`IsLineBGImage` 未在循环前复位 —— 不是缺陷**（复核结论）：
1959-1967 复位了 7 个标志，**遗漏** `IsWinImage`/`IsLineBGImage`；但 `CheckHintImage` 里
`Result := True` **只可能出现在 `if Length(Text) > 2` 块内**（首个赋值 `Result := False` 在 1720，
其后全部 `Result := True` 都在 1723 的 `if` 体内），而这两个标志在 1749-1751 于该块内被初始化。
⇒ `Result = True ⇒ 标志已初始化`。**该遗漏不可达**，故**不记为缺陷**（仅此说明留证）。

③ **`THintLines.GetSize` 的 `MaxWidth` 死分支**：`maxWidth` 在 1393 被赋 0，全文（1385-1418）
再无写入；`if FWidth < MaxWidth`（1406）在 `FWidth >= 0` 恒成立下**不可达**。保留同形并注释。

④ **`TFiexdHeightLine` 原文是空类**（177-178 `= class(THintMessage) end;`），无任何成员 ⇒
本车道同样不新增成员（`THintLines.AddFixedHeightLine` 只写 `FHeight`）。

⑤ **`TDrawScreenMoveMsg.Initialize/Finalize`、`TDrawScreen.DrawHint`、`THintWindows.UpDate` 原文是空体**
（`//` 或 `{ }` 注释掉全部语句）⇒ 托管侧同样空体，且 `TDrawScreenScrn_Update_TouchesHintWindowsGlobal`
用"不可见窗口**不会**被清掉"反向证明 `UpDate` 确实什么都没做。

⑥ **`TDrawScreen` 的两个旧版 `AddSysMsg`/`AddSysMsgEx`（4538-4600）被 `(* *)` 整段注释** ⇒ 不移植；
`TDrawScreenCenterMsg.Add` 里被注释的逐字折行旧实现（3173-3200）、`THintText.Paint` 里被注释的
`GameCanvas.Draw` 旧路径（1101-1106）、`THintWindow.DrawBackground` 里被注释的 `SourceRect`（2195）
同属此列，均在代码里以注释形式保留。

---

## 10. 测试清单（145 例，全部新增）

### `ScrnDrawHintTests.cs`（56 例）
* `ProcessHintText`：`<TextW>` / `<ImgNum>`（含"缺图不累加"）/ `<Looks>`（`mod 10000`）/ `<DnItems>` / `<StateItem>` /
  `<PlayImg>` / `<NewopPlayImg>`（PlayTime ≤ 0 → 100）/ `<ItemProgress>` / `<WinNewopUI>` / `<LineNewopUI>` /
  `<Countdown>`；未知 tag 字面化；tag + 尾巴；空串；`{文字|颜色}` 在范围/超范围两分支；**13 支计数取证**。
* `THintLines`：`GetSize` 的 `ItemHeight = FHeight+2` 与空容器兜底、`AddFixedHeightLine`、
  `MinWidth` 只来自 `TLineBGHintImage.FTextureWidth`、`Strings/Objects` 只对 `THintText` 生效、`Delete/Clear`、
  `Paint` 的两遍顺序、`PaintWithoutWinHintImage` 的 `HaveWinHintImage`、`PaintWinHintImage` 的 X 公式。
* `TCountdownText`：`GetShowText` 9 个边界（0/-5/59/60/3599/3600/86399/86400/90061）；
  `Paint` 每 1000ms 减 1 且**跨 2500ms 也只减 1**。
* `THintText`：描边 +2 只加高度、`SetSize` 触发 `Initialize` 而 `Caption` 写访问器**不**触发。
* `THintPlayImage`/`Ex`：帧推进与回卷、`GetCachedImage` 原点偏移**有/无**的差异、`FIncSpacing`、`FGameImages=nil` 不动宽高。
* `THintItemProgress`：基础图 620/640 选择、`FMaxValue=0` 只画背景、裁剪宽 `Round(w/Max*Cur)`。
* `THintWindow`：几何与四边夹紧、`HintUp`/`DrawLeft` 先平移后夹紧、矩形下限 20×20、`\` 分行、
  `ShowColor` 的「颜色在前」与回退 255、`SetHintX` 夹紧 vs `SetHintXExt` **不**夹紧、`X` 属性同值不重算、`Clear` 置不可见、
  越屏早退（背景仍画 1 次）、右/中对齐算式。
* `THintWindows`：`Show` 使旧窗失效 + `Draw` 删不可见、`Clear` 回写 `g_LastHintMakeIndex=-1`、
  `Initialize`/`Finalize_` 都是全清、`UpDate` 空实现。

### `ScrnDrawMsgTests.cs`（58 例）
* `TDrawSysMsg`：默认值、**容量 10 淘汰最旧**、上行/下行 16px 步进、`g_MySelf=nil` 早退、**3000ms 淘汰**、`Clear`。
* `TDrawSysMsgEx`：tick 初始化、三档参数（>8 / >5 / else）、每 `nStepTime` +1、OffsetY=16 删队首并复位、
  **alpha = 180 − OffsetY×15 且夹 0**、**每次最多画 6 行**。
* `TDrawMoveHintMsg`：恒向下 16px（与 `TDrawSysMsg` 的差异）。
* `TDrawScreenCenterMsg`：按 `SCREENWIDTH−20` 折行且 token 无损、超时清空、居中算式 `(SCREENHEIGHT−行数×行高)/2`。
* `TDrawDelayMsg`：同 `RecogId` 原地更新（**不**重置跑马灯）、`RecogId` 全删、过期淘汰 + `Count≤5` 关跑马灯、
  `%d`/`%s` 替换 + `X≤0` 兜底 1。
* `TDrawScreenMoveMsg` / `TScreenMoveMsgList`：水平初始几何、滚动裁剪 `SrcRect`、`MoveOver` 收敛、按 `Top` 分组。
* `TMoveHintMsgList`：坐标四种兜底（含 NPC 对话框偏移）、`<40` 每 10ms +2 / `≥40` 再等 1500ms 才删、`Clear`、`Free` 空循环。
* `TDrawScreenCenterNewlineMsg`：`div 100 = 0` 的反向门、`100` 型的透明框算式、`101` 型走 `DrawEx` 且首帧 alpha=0、
  停留超时无缓存 → `m_boShowOver`、`ClearTimeCache` 在 `nTime+8000` 淘汰、显示中只进缓存、空串忽略。
* `TDrawScreenNewMoveMsg`：行高/`Y0,Y1,Y2`/`StepMove`/`StepAlphaChange` 公式、显示中进缓存、空串/无字体返回、
  停留→滚动切换、滚动步进与换行收敛。
* `TDrawScreenScrn`：默认值、`AddSysMsg` 四元组分组建、Initialize/Finalize 翻转、
  `DrawMsg_TopLevel`/`DrawSysMsg_BottomLevel`/`DrawMove`/`DrawMoveBefor` 的**双门**、
  `ShowHint` 转发、`ClearHint`、`ChangeScene` 用 MShare 全局且三支空臂**不改** `CurrentScene`、随机码钩子、
  `AddChatBoardString`/`AddTopChatBoardString` 转发、四个 `Add*Msg` 转发、`DrawScreen` 区域图标（从右往左累加）+
  攻城区域 + 绿色信息三重门 + 焦点血量门（含 `RC_MERCHANT` 例外）+ 英雄段、`DrawHint` 空体、`ClearChatBoard`、`Update`。

### `ScrnDrawDropItemsTests.cs`（31 例）
* Create 预置 512 池、`ClearAndFree`；点表 `MakeLong(X,Y)` 升序不变式、同点复用、`GetItemListIndexByY` 的 `X=65535` 键、
  越界返回 null；`PointCompare`/`IDCompare` **virtual 可覆写且被使用**。
* `AddDropItem`：新项取池 + `Visible=True`、重复 ID 只改 `DBName`、池耗尽新建、命中特效（帧/闪烁 5 字段 + **头插**）、
  无特效 `FileIndex=-1` + 尾插、首个特效也尾插、`PlugInEnabled` 绑定 `ShowItem`。
* `DelDropItem`：双向移除 + 空点表释放 + 还池、点表非空保留、未知 ID 返回 null。
* `Clear`：还池上限 512（513 条时丢弃 1 条）、池上限值。
* `RefreshDrawList`：3 个绘制位 + 清 `NameImageInfo`、第二轮**只判 `Visible`**（易误读点）、补足不重复、
  `ResetShowItem` 走 `g_FileItemDB`（用"不在库里 → 被覆盖成 null"反证）、全局遍历。
* 原文缺陷：**`string[60]` GBK 静默截断**（61 ASCII / 31 汉字 / 恰好 60 边界）+ `AddDropItem` 入参同样被截；
  托管差异：`GetDrawItems` 越界抛异常；`Makecode_T`/`Cutecode_T` 恒等；`Lock/UnLock` 可重入。

---

## 11. 未完成 / 阻塞项（如实登记）

| # | 项 | 状态 | 原因 |
|---|---|---|---|
| 1 | `TDrawScreen` 以原文类名落地 | **未完成**（以 `TDrawScreenScrn` 替代，成员 100%） | `Scenes/Scenes.cs:694` 无 `partial` 且该文件**不在本车道分区**；任务书明确要求「登记」而非自改。见 **D-P10-01**（含逐步最小改法） |
| 2 | 删除 `DropItemFx.cs` 的 `PointDropItemList`/`DropItemsStore` 替身 | **未完成**（如实登记） | 引用方 `PlaySceneCore.cs:194` + 3 个测试文件在分区外，删除会让门禁变红。见 **D-P10-02** |
| 3 | 删除 `GUI/Share/FStateSeams.cs` 的 `THintLines`/`THintWindows` 接缝 | **未完成**（如实登记） | 任务书硬性禁止改 `GUI/Share/**`。见 §4.3 / **D-P10-06** |
| 4 | 真实 HGE / GameImages 接入（`TDrawScrnCanvas` 退役） | **未完成**（接缝，带存在性语义） | `HGE.pas` / `GameImages.pas` 未全量移植；本车道以 headless 画布承载并把每条绘制落成可断言记录。见 §8.1 |
| 5 | `GetCachedImage` 的真实 X/Y 原点 | **未完成**（接缝，默认退化且已声明） | `TGameImages` 接缝只有索引器。见 **D-P10-11** |
| 6 | `TDrawScreenCenterNewlineMsg.Draw` 里被注释的旧折行实现（3173-3200） | **按原文不移植** | 原文已注释，非缺口 |
| 7 | `DrawScrn.pas` 里的 `{$IF IsMultiThreadRender = 1}` 临界区分支 | **按未定义分支移植** | 该条件编译符号在本工程编译配置里未定义；`ChangeScene`/`DrawScreen` 两处已注释标明 |

**没有任何"未跑通"的项**：两条门禁均为实跑全绿（§0），`git status --porcelain` 为空。
