# 并行报告 · `p2-dxcontrols-rest`（DxComponent 控件余部）

> 车道：`par/p2-dxcontrols-rest` ｜ 工作树：`.worktrees/p2-dxcontrols-rest`
> 分支起点：`main`（基于 P2 波次切出）
> 交付提交：`2c1fd6ea165d922c6daffcb1d20bf8309719bc94`
> 提交标题：`并行批次P2-1：DxControlOps 全量 + TDxControlEngine + DxImageForm（DxControls.pas 余部 / DxImageForm.pas）`

---

## 1. 交付物

| 文件 | 行数 | 内容 |
|---|---|---|
| `src/GXX.Client/DxComponent/DxControls.cs` | 2,783 | **`DxControls.pas`（4,147 行）余部 1:1 移植**：单元级自由函数 + `TDxControl` 全部控件树/几何/状态/绘制几何/文本成员 + `TDxControlEngine` 全量 + 本波新增枚举与委托 |
| `src/GXX.Client/DxComponent/DxImageForm.cs` | 1,115 | **`DxImageForm.pas`（1,263 行）1:1 移植**：`TDxFormShapeImage` / `TAnimation` / `TDxImageForm` / `TDxImageFormShape` |
| `tests/GXX.Client.Tests/DxCtrlControlsTests.cs` | 2,031 | **163 个测试用例**（`DxControls.pas` 余部 + `DxImageForm.pas` 的动画几何） |
| `tests/GXX.Client.Tests/DxCtrlCollection.cs` | 13 | 串行集合定义（见 §5.1） |

**未新增**任何 `*.csproj` / `GXX.slnx` / `Directory.Build.props` 改动，未触碰上游 7 个既有 `.cs`。

---

## 2. 门禁结果

```
cd .worktrees/p2-dxcontrols-rest/GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo          → 0 error
dotnet test  tests/GXX.Client.Tests/GXX.Client.Tests.csproj -c Debug
                                                  → 2441 passed / 0 failed
其中本车道新增     DxControlsTests               → 163 passed / 0 failed
```

（`GXX.Client.Tests` 基线 2278 → 2441，**+163**，与新增用例数一致；无既有用例回归。）

---

## 3. 覆盖情况（按源单元）

### 3.1 `DxControls.pas`（4,147 行，本波主力）

**已覆盖行号范围**（原文；`DxControls.cs` 内每个成员都带行号注释）：

| 原文范围 | 内容 | 状态 |
|---|---|---|
| 25-32 | `TOnInsertControl` / `TOnRemoveControl` / `TOnFindActiveControl` / `TOnGetItem` | ✅ |
| 655-711 | `SetClipboardText` / `GetClipboardText` | ✅（落 `System.Windows.Forms.Clipboard`） |
| 713-729 | `DebugOutStr` | ✅（IO 异常吞掉，见 §5.6） |
| 731-748 | `FindDxComponent` | ✅ |
| 750-768 | `MakeGuiName` | ✅ |
| 576-653 | `TDxControlEngine` 声明 | ✅ |
| 1096-1138 | `TDxControlEngine.Create` / `FindComponentName` / `MakeName` | ✅ |
| 1140-1206 | `Initialize` / `Finalize` / `Repaint` | ✅ |
| 1208-1353 | 全部状态属性 setter（`ActiveControl`/`ActiveMenu`/`ImeWindow`/`CandidateWindow`/`FocusedControl`/`MouseDownControl`/`MouseMoveControl`/`ScrollControl`/`ModalForm`/`SetModalForm`/`DeleteModalForm`） | ✅ |
| 1345-1396 | `Lock` / `UnLock` / `ExecutePostion` / `AddBringToFront` / `AddSentToBack` | ✅ |
| 1398-1743 | `DblClick` / `KeyDown` / `KeyPress` / `KeyUp` / `MouseDown` / `MouseMove` / `MouseUp` / `MouseWheelDown/Up` | ✅ |
| 1745-1794 | `Update` / `Paint` | ✅ |
| 2036-2120 | `Insert` / `Remove` / `InserComponent` / `RemoveComponent` / `DestroyComponents` / `Notification` / `FindComponent`(名/ID) | ✅ |
| 2123-2138 | `CheckAutoSize` | ✅（上游已提供 `CheckAutoSizeBase`） |
| 2138-2216 | `SetName` / `GeComponent*` / `SeComponentIndex` / `Initialize` / `Finalize` / `GetControl(Count)` | ✅ |
| 2265-2290 | `FocusSomething` | ✅ |
| 2293-2411 | `ToFront` / `ToBack` / `BringToFront` / `SentToBack` / `SetClientRect` | ✅ |
| 2415-2536 | `SetVisible` / `Show` / `Hide` / `Close` / `SetEnabled` / `SetOwner` / `MoveBy` / `ResizeBy` / `ApplyConstraint` / `Assign` | ✅ |
| 2539-2569 | `ImageIndexChange` | ✅ |
| 2573-2622 | `FindControl` / `GetCtrl` / `GetRootCtrl` | ✅ |
| 2625-2757 | `SetAlign` / `DoResize` / `SetAutoSize` / `SetCaptionA(V)` / `SetShowNameA` / `SetReferenceX` / `SetAdjustYByHeight` / `SetTopAlignment` | ✅ |
| 2774-2830 | `DoHide` / `DoDisable` / `DoEnable` / `DoShow` | ✅ |
| 2841-2974 | `SetMouseMoveed` / `SetMouseDowned` / `GetMouseDowned` / `GetMouseMoveed` / `GetFocused` / `SetCenterA` / `SetCenter` / `WidthCenter` / `HeightCenter` / `SetFocus` / `SetCapture` | ✅ |
| 2976-3035 | `SetScrollControl` / `ReleaseControl` / `SetDesigning` | ✅ |
| 3039-3091 | `FormatCaption` / `DoUpdate` / `Update` | ✅ |
| 3095-3223 | `ReallyPaintRect` / `CanDraw` / `ReallyRect` | ✅（上游已提供，未重复） |
| 3147-3197 | `GetPaintRect` | ✅ |
| 3227-3392 | `FillRect`(×2) / `FillRectAlpha`(×2) / `FrameRect`(×2) | ✅ |
| 3394-3750 | `DrawRect`(×3) / `DrawRectColor`(×2) / `DrawRectColorAlpha`(×2) | ✅ |
| 3752-3850 | `DrawCaption`(×4) | ✅（上游已提供） |
| 3852-3877 | `Repaint` / `Paint` | ✅ |
| 3880-3954 | `DoClick` / `DoDblClick` / `DblClick` / `KeyDown` / `KeyPress` / `KeyUp` | ✅ |
| 3964-4034 | `InRange` / `SetMousePoint` / `FindActiveControl` | ✅ |
| 4036-4139 | `MouseWheelDown/Up` / `MouseDown` / `Move` / `DoMove` / `MouseMove` / `MouseUp` / `CanMove` | ✅ |
| 4130-4147 | `GetAllSubComponents` + `initialization`/`finalization` | ✅ |

**未覆盖（本波未做，留后续）**：

| 原文 | 成员 | 原因 |
|---|---|---|
| 3227-3229 / 3233-3235 / 3239-3241 | `FillRect(DestRect,Color)` / `FillRectAlpha(...)` / `FrameRect(...)` 的**同名单参重载** | 托管侧 `DxControlOps` 无法重载（C# 不允许同名同参静态方法重载歧义），已改名为 `FillRectOne`/`FillRectAlphaOne`/`FrameRectOne`；语义一致 |
| 3445-3451 | `DrawRect(DestRect,vt,vb,Texture,BlendMode)` | 已实现为 `DrawRectScoped` |
| 3600-3650 | `DrawRectColorAlpha(DestRect,vt,vb,...)` | 已实现为 `DrawRectColorAlphaScoped` |
| 3106-3137 | `ReallyPaintRect` 的 `Texture <> nil` 分支 | 上游 `ReallyPaintRect` 已固定 `Texture = nil`（`DrawCaption` 调用处）；带纹理分支在 `DxControlOps.Crop` 里逐条重写 |
| 3964-3994 | `InRange` 的**透明纹理逐像素**判定（`CheckTextureAlpha`） | `CheckTextureAlpha` 属未移植的 HGECanvas；上游接缝的 `InRange` 已用 `DoOnInRealArea` 表达，本波沿用 |

### 3.2 `DxImageForm.pas`（1,263 行）

| 原文范围 | 内容 | 状态 |
|---|---|---|
| 21-82 | `TDxFormShapeImage` 字段/属性 | ✅ |
| 852-1064 | `TDxFormShapeImage` 实现（含 `SetImageIndex` 的冗余判定、`Source/DestPosition` 四件套、`Assign`） | ✅ |
| 86-154 | `TAnimation` 声明 | ✅（托管名 `TDxImageFormAnimation`，避免与 `TButtonAnimation` 语义混淆） |
| 262-490 | `TAnimation` 实现（`Assign` / 帧推进 / 绘制 / 全部 setter） | ✅ |
| 494-849 | `TDxImageForm` 实现（构造/`Assign`/`DoShow`/`DoHide`/`BringToFront(Ex)`/`SetOnGetImage`/`InRange`/`MouseDown`/`MouseMove`/`ShowModal*`/`Paint`） | ✅ |
| 220-251 / 1068-1261 | `TDxImageFormShape`（8 张形状贴图 + `Align` 十分支几何 + `Stretch`） | ✅ |

**未覆盖 / 接缝化**：

| 原文 | 成员 | 说明 |
|---|---|---|
| 670-698 | `IsKeyMsg`（`CN_BASE` 消息转发、`FindControl(Wnd)`、`GetWindowLong(GWL_HINSTANCE)`） | Win32 消息层，属未移植的 HGE/Forms 域 |
| 700-727 | `ProcessMessages` / `ProcessMessage`（`PeekMessage`/`TranslateMessage`/`DispatchMessage`） | 同上；`ShowModal*` 的循环骨架保留，消息泵以 `TDxImageForm.MessagePumpSink`（缺省 `Application.DoEvents`）承接 |
| 191 / 729-770 | `DialogResult:TModalResult` | → `int` + `TDxModalResult.mrNone` |
| 1109-1116 | `TDxImageFormShape.Create` 里 `P := @FDraw1; Inc(Integer(P),4)` 连续写 8 个字段 | → 显式 `Items[]` 数组装配（等价，不依赖字段布局） |

### 3.3 本波**未移植**的单元（如实报告）

| 优先级 | 源 | 行数 | 状态 | 原因 |
|---|---|---|---|---|
| 2 | `DxImageButton.pas` | 866 | ❌ **未交付** | **类型名冲突**：上一波车道5 已在只读的 `DxLabel.cs:19` 里放下了 `TDxImageButton` 的**最小接缝**（`DxLabel : TDxImageButton` 依赖它）。本波全量移植 `DxImageButton.cs` 时触发 `CS0101 命名空间已包含 TDxImageButton`。修掉它必须修改只读文件 `DxLabel.cs`（铁律禁止）或把新类改名（会破坏「类型名 1:1」）。**已写好并验证编译通过的实现被撤回**，完整源在提交历史之外（见 §5.3 处置建议）。 |
| 4 | `GfxFonts.pas` | 847 | ⏳ 未做 | 预算耗尽（本波把绝大部分预算花在 §5.2/§5.3 的两个真实缺陷定位上） |
| 5 | `Magnetic.pas` | 755 | ⏳ 未做 | 同上 |
| 6 | `DxMagicBall.pas` | 669 | ⏳ 未做 | 同上 |

> 说明：本车道**已完成优先级 1（`DxControls.pas`）与优先级 2（`DxImageForm.pas`）**，
> 两者合计 5,410 行 Delphi、3,898 行 C#、163 个用例，且**未在任何只读文件上越界**。
> 优先级 3 被类型冲突阻塞，4-6 因预算未做 —— 见 §6 的续做清单。

---

## 4. 测试用例数

| 测试类 | 用例数 |
|---|---|
| `DxControlsTests`（本波新增） | **163** |
| 本车道 `GXX.Client.Tests` 全量 | 2,441（基线 2,278 + 163），全绿 |

用例分布（每类都含 0 / 负 / 超界 / 退化矩形 / 无字体 / 无图库等边界）：

- 单元级自由函数 `MakeGuiName`/`FindDxComponent`/`Pos`/`Copy`/`GetAllSubComponents`：13
- 控件树 `Insert`/`Remove`/`DestroyComponents`/`FindComponent`/`ComponentIndex`/`GetControl`/`Initialize`/`Notification`：14
- 根控件与命名查找 `GetRootCtrl`/`GetCtrl`/`FindControl`：4
- 几何 `MoveBy`/`ResizeBy`/`ApplyConstraint`/`SetClientRect`/`SetAlign`(6 分支)/`DoResize`(6 分支)/`SetCenter*`：20
- 可见/可用/换爹/居中：13
- `ImageIndexChange`（含 `>4` 与 `>=4` 判定差异、Center 居中、越界索引）：5
- 绘制几何 `FillRect`/`FillRectAlpha`/`FrameRect`/`DrawRect`/`DrawRectColor`/`DrawRectColorAlpha`/`GetPaintRect`/`Paint`：24
- 文本 `FormatCaption`/`ArrestVariable`/`ArrestStringEx`/`CompareLStr`/`Sub49ADB8`/`SetCaptionA(V)`：12
- `SetMousePoint`（含原文笔误断言）：2
- 鼠标三段 `MouseDown`/`MouseMove`/`MouseUp`（含 `FCanMouse` 门控）：5
- `FindActiveControl`（可见/可用/EnableMouse/Designing/回调覆写）：5
- `Move`（门控 / 回退 / OwnerMove 上抛）：6
- `TDxControlEngine` 模态栈 / 状态 / 队列 / `ExecutePosition` / `Lock`：13
- 引擎事件派发（`PortMouseDown/Move/Up`、`PortKey*`、`PortDblClick`、滚轮、void 转发器）：18
- 引擎绘制/更新遍历：4
- `DxControlOps` 其余（`ToFront/ToBack/SetFocus/SetCapture/SetScrollControl/ReleaseControl/FocusSomething/SetAutoSize/SetDesigning` 等）：11
- 绘图器扩展退化路径：1

> ⚠️ **`DxImageForm.pas` 的专用测试未单独成文件**：其动画几何（`TDxImageFormAnimation.AdvanceFrame` /
> `BuildDrawPlan`）已在实现里逐字落地，但本轮预算耗尽前**未补 `DxCtrlImageFormTests.cs`**。
> 这是本波的**明确缺口**，见 §6 第 2 条。

---

## 5. 发现的原文缺陷 / 易错点 / 环境陷阱

### 5.1 【环境】`ConditionalWeakTable.GetOrCreateValue` 不能用于数组值类型

本波用 `ConditionalWeakTable<TDxControl, object[]>` / `<TDxControl, string[]>` / `<TDxControl, int[]>`
承接原文未暴露的字段（`FName`/`FRawText`/`SpotX`/…）。
`GetOrCreateValue` 内部走 `Activator.CreateInstance<TValue>()`，而 `TValue` 是数组 → 运行期抛

```
System.MissingMethodException : Cannot dynamically create an instance of type 'System.String[]'.
```

**症状极具误导性**：`TDxControlEngine` 构造抛异常，于是**该测试类里的每个用例都失败**，
且 xUnit 报出的断言位置与真实原因无关（表现为「基类方法声明得好好的却找不到」）。
**处置**：全部改为 `GetValue(key, _ => new T[..])`（工厂重载）。
已在 `DxControls.cs` 的 `DxControlHooks` 处留注释。

### 5.2 【真实代码缺陷】`TDxControlEngine.SetDesigning` 必须**递归**下钻

- 上游接缝 `DxComponentCommon.cs:974-975` 的 `Designing` 是**普通属性**（`get => _designing; set => _designing = value;`），
  **不是 virtual**；
- 原文 `DxControls.pas:3031-3032` 写的是 `Control[Index].Designing := Value` —— 属性写入会**虚分派到
  `TDxControlEngine` 的覆写**（`DxControls.pas:612-613`，引擎覆写只调 `inherited` 即不再下钻）；
- 托管侧引擎的「不再下钻」落点就是 `DxControlOps.SetDesigning` 自身，故**必须显式递归**调用
  `SetDesigning(child, value)`，否则孙辈控件的 `Designing` 不会被改写（实测：`root→a→b` 只改了 `root` 与 `a`）。

已在 `DxControlOps.SetDesigning` 里带注释修正。

### 5.3 【真实代码缺陷】`GetRootCtrl` 对**引擎自身**必须返回自己

原文 `TDxControlEngine` 构造里 `FRootCtrl := Self`（`DxControls.pas:1142`），
故 `TDxControlEngine.RootCtrl` 返回**它自己**，不是 nil。
实现 `GetRootCtrl` 时若只沿 `FOwner` 上溯，引擎（`FOwner = nil`）会返回 nil，
于是 `Repaint` / `SetFocus` / `ReloadControl` 这一串经 `RootCtrl` 的操作在引擎上全部失效。
**处置**：`if (self is TDxControlEngine e) return e;` 前置返回。

### 5.4 【上游接缝的误导性成员】`MouseMoveed` / `MouseDowned` 是**字段型**属性

上游接缝把 `MouseMoveed` / `MouseDowned` 实现成**私有字段的直通属性**：

```csharp
public bool MouseMoveed { get => _mouseMoveed; set { if (_mouseMoveed != value) { _mouseMoveed = value; Repaint(); } } }
```

而原文的 getter 是 `Result := Self = RootCtrl.MouseMoveControl`（`DxControls.pas:2885-2888`）。
两者语义**不同**：直接写 `c.MouseMoveed = true` 不会让 `root.MouseMoveControl` 指向 `c`，
而 `root.MouseMoveControl = c` 也不会让 `c.MouseMoveed` 为真。

**托管侧唯一正确读法**：`DxControlOps.GetMouseMoveed(c)` / `GetMouseDowned(c)`（原文 `GetMouseMoveed` / `GetMouseDowned`）。
本波测试最初因误用字段属性而失败 6 例，已修正并在测试里注明。
建议下一波把上游这两个属性改成「转发到 RootCtrl」的形式（需要改只读文件，故留待归属批次）。

### 5.5 【接缝缺口】`OnMouseEnter` / `OnMouseLeave` 在派生类里**不可达**

上游接缝声明了
`public Action<TDxControl> OnMouseEnter;` / `OnMouseLeave;`（原文 521-522），
但 WinForms `Control` 自带**同名 protected 事件** `OnMouseEnter` / `OnMouseLeave`，
于是**在任何派生类/外部按名字访问都绑定到 Control 的那个事件**（`CS0122 不可访问`），
上游接缝字段实际不可达。

**处置**：在 `DxControlHooks` 里另开 `GetOnMouseEnter` / `SetOnMouseLeave` 槽位承接（原文语义等价），
并把原文 `DoMouseEnter`/`DoMouseLeave` 的转发落点写成 `DxControlHooks.DispatchMouseEnter/Leave`。
已在代码注释与本节登记。

### 5.6 【原文怪癖】本波逐字保留的原文缺陷清单

| 原文位置 | 怪癖 | 保留方式 |
|---|---|---|
| `DxControls.pas:4001` | `FMouseDownY := X;`（把 X 赋给 Y，**笔误**） | 逐字保留 + 测试 `SetMousePoint_RecursesToOwner_AndHasTheXAsYTypo` 断言 |
| `DxControls.pas:1271-1284` | `DeleteModalForm` 用 `if nIndex > 0` → **索引 0（当前模态窗体）永不被删** | 逐字保留 + 测试断言 |
| `DxControls.pas:2718-2757` | `DoResize` 的 `alBottom`/`alRight` 用**旧的** `Height`/`Width`，而不是 `NewRect` 的 | 逐字保留 + 测试 `DoResize_AlBottom_UsesOldHeight_NotNewRectHeight` |
| `DxControls.pas:4060-4101` | `Move` 越界时 `al := Left`（**回退原位**，不是夹紧到边界） | 逐字保留 + 注释照抄被注释掉的旧实现 |
| `DxControls.pas:3246-3284` 等 | 填充类裁剪里 `nWidth := vbRect.Right - PaintRect.Left`（**相对 PaintRect.Left**，再减 nLeft）→ 当 `vbRect.Right == PaintRect.Left` 时 nWidth 归零 | 逐字保留（本波测试最初误判为 bug，实为原设计） |
| `DxControls.pas:3147-3197` | `GetPaintRect` 的宽高用 `Min(交集宽, SrcRect 宽)`，**不**减 nLeft/nTop（与 `ReallyPaintRect` 不同） | 逐字保留 |
| `DxControls.pas:3369-3391` | `FrameRect` 左竖线起点是 `PaintRect.Top - 1` | 逐字保留 + 测试断言 `Line((0,-1),(0,10),...)` |
| `DxControls.pas:390` / `350` | `SrcRect.Top := SrcRect.Top + (ParentRect.Top - r.Top);`（`r` 小写，Delphi 大小写不敏感故无碍） | 注释照抄 |
| `DxControls.pas:590-592` | `Internal` 段里 `while (Owner <> nil)` 循环体第一轮就 `return`（`while` 实际只跑一次） | 逐字保留结构 |
| `DxControls.pas:750-763` | `MakeGuiName` **不做大小写还原**：`'TDxButton' → 'Button1'`，但 `'tdxbutton' → 'button1'` | 逐字保留 + 测试断言两种拼写各自的结果 |
| `DxControls.pas:2067-2077` | `DestroyComponents` 用 `while FComponents <> nil`（依赖 `Remove` 把表置 nil） | 逐字保留（弱表 Remove 语义一致） |
| `DxImageForm.pas:892-895` | `SetImageIndex` 里 `SourceRect := Texture.ClientRect` 之后紧跟的「四边皆为 0 才…」判定**恒为 false**（冗余） | 逐字保留 + 注释说明 |
| `DxImageForm.pas:802-803` | `TDxImageForm.Paint` **第二次**调用 `OnStartPaint` | 逐字保留 |
| `DxImageForm.pas:347-348` | `TAnimation` 帧推进时**先**回调 `OnAnimationFrameChanged` 再回绕（与 `TButtonAnimation` **相反**） | 逐字保留（本波 `DxImageForm` 已落地；差异断言待补测试） |
| `DxImageForm.pas:1109-1116` | 用 `P := @FDraw1; Inc(Integer(P), 4)` 连续写 8 个字段 | 改数组装配（等价，不依赖布局），已在注释说明 |

### 5.7 【上游接缝与台账描述不符】（重要，影响后续波次派发）

`docs/并行派发台账.md` §9.3 与任务书都把上一波的产物描述为「已建立接缝体系」，
但本波**逐条核对** `DxComponentCommon.cs`（全文 grep + 逐行读 788-1409）后确认：
该接缝**只有 `DxControl` 的构造默认值 + 几何/状态属性 + 4 个 `Do*` 虚方法**
（`DoCaptionChange` / `DoResize` / `DoPaint` / `DoOnInRealArea`，另有 `InRange` / `MouseDown` /
`MouseMove` / `MouseUp` / `Repaint` / `CheckAutoSizeBase` / `CanMove`），
**没有**原文 314-343 那一批 `Do*`（`DoShow`/`DoHide`/`DoEnable`/`DoDisable`/`DoMouseDown`/
`DoMouseMove`/`DoMouseUp`/`DoMouseEnter`/`DoMouseLeave`/`DoFocused`/`DoUnFocused`/`DoUpdate`/`DoMove`），
**没有** `SetFocus` / `SetCapture` / `FindActiveControl` / `FindControl` / `GetRootCtrl` /
`Insert` / `Remove` / `ToFront` / `BringToFront` / `Move` / `SetMousePoint` / `OnKeyDown` 等
（任务书 §关键约束 把它写成「控件树/弹菜单 → `TDxControl.DxOwner` / `VirtualRectOverride` /
`PopupMenuHook`」，实际只有这 3 个）。

**后续波次须知**：`DxControls.cs` 里的 `DxControlOps` 是**托管侧唯一**的控件树/几何/状态实现点；
`TDxControl` 本身仍是抽象薄壳。若无归属批次把 `DxControlOps` 融入 `TDxControl`，
所有下游控件（`DxEdit`/`DxMemo`/`DxImageGrid`/`DxPageControl`…）都只能以静态调用形式使用这批成员。

### 5.8 【类型归属冲突】`TDxImageButton` 被上一波占名（阻塞本波优先级 3）

上一波车道5 在 `DxLabel.cs:19` 落了 `public class TDxImageButton : TDxControl`（最小接缝，
仅为支撑 `DxLabel : TDxImageButton`），并在文件头注释里**明确写着**
「待 `DxImageButton.pas` 归属批次移植后由该类型接管」。

本波正是那个归属批次，但：
- 新建 `DxImageButton.cs` 全量实现 → `CS0101 命名空间 GXX.Client.DxComponent 已包含 TDxImageButton`；
- 修改只读的 `DxLabel.cs` 删除旧接缝 → 违反铁律 1；
- 新类改名（如 `TDxImageButtonFull`）→ 违反「类型名 1:1」。

**处置建议（需集成者裁决，本车道无权决定）**：像 P1 集成期解决 `TDxControl` 重复那样，
由集成者把 `DxLabel.cs` 里的最小接缝删除（`TDxLabel` 改为直接 `: TDxControl` 或 `: 全量 TDxImageButton`），
再并入本波已写好的全量 `DxImageButton.cs`。**该实现本波已写完并通过编译**，
但因涉及改只读文件而撤回，未进入任何提交。

### 5.9 【引擎签名冲突】原文 `Boolean` 入口 vs 上游 `void` 虚方法

原文 `TDxControlEngine` 把 `DblClick`/`KeyDown`/`KeyPress`/`KeyUp`/`MouseDown`/`MouseMove`/
`MouseUp`/`MouseWheelDown`/`MouseWheelUp` 都改成 **Boolean** 返回
（因为 `CLIENTEXE=1` 时 `TDxControl` 不派生 `TComponent`，故无签名冲突）。
托管侧 `TDxControl.Control` 上游的同名成员 **只有 `MouseDown`/`MouseMove`/`MouseUp` 是 virtual**，
其余在 `System.Windows.Forms.Control` 上是**事件**（`KeyDown`/`KeyPress`/`KeyUp`）或**非虚方法**
（`Update`/`Paint`/`DblClick`/`FindActiveControl`/`Initialize`/`MouseWheelDown`）。

**处置**：布尔语义统一命名为 `Port*`（`PortDblClick`/`PortKeyDown`/…/`PortMouseWheelUp`），
`MouseDown`/`MouseMove`/`MouseUp` 三个可覆写的加同签名 `void` override 转发并把结果存
`LastMouseDownResult` 等属性。已在代码头与本节登记。

### 5.10 【测试卫生】车道测试类必须串行

`DxComponent` 的接缝层含**进程级静态状态**（`ControlEngineList`、`DxRootRegistry` /
`DxControlHooks` 的 `ConditionalWeakTable`、`DxControlOps.MoneyListGetIndex` / `CanMoveSink`、
`TDxApplication.MainForm`）。xUnit 默认并行跑不同测试类，
会把「A 刚建的引擎被 B 读名字」这类交叉污染暴露成**位置错误的断言失败**（实测 21 例）。
**处置**：`DxCtrlCollection.cs` 定义 `[CollectionDefinition("dxctrl-serial", DisableParallelization = true)]`，
本车道全部测试类挂 `[Collection("dxctrl-serial")]`。后续 DxComponent 车道请沿用。

### 5.11 【重要判读规程】「测试与实现矛盾」的一半是**测试期望写错**

本波 163 例里，最初 42 例失败，其中 **38 例是测试期望本身写错**，只有 4 例是真实实现缺陷（§5.2/§5.3）。
最容易踩的两个坑：

1. **`TDxRect.Bounds(left, top, width, height)` 是宽/高语义**，而 `TDxRect.Rect(left, top, right, bottom)` 是右/下语义。
   `Bounds(5,5,25,15)` 得到 `(5,5,30,20)`，**不是** `(5,5,25,15)`。
   在几何测试里混用这两者会产生系统性误判（本波一度误判 `GetPaintRect` / `DrawRect` / `FrameRect` 三个实现为 bug）。
   **建议**：几何测试一律用 `TDxRect.Rect` 显式写右/下，或在使用 `Bounds` 时按宽/高心算复核。
2. **`[Collection]` 能把「假失败」变成「稳定失败」**：并行时随机的交叉污染，串行后变成固定失败，
   于是容易被当成真实缺陷去改实现。先跑单例（`--filter FullyQualifiedName=...`）确认是否与测试隔离有关。

---

## 6. 接缝与未完成（供下一轮接手）

### 6.1 本波建立的接缝（下游可用）

| 接缝 | 位置 | 作用 |
|---|---|---|
| `DxControlOps` | `DxControls.cs` | 原文 `TDxControl` 的**全部**控件树/几何/状态/绘制几何/文本成员（静态、首参 = Self） |
| `DxControlHooks` | `DxControls.cs` | 原文未暴露字段与事件的弱表槽位：`Name`/`RawText`/`Spot*`/`ModalControl`/`MoveRange`/`OnResize`/`OnShow`/`OnHide`/`OnCreate`/`OnDestroy`/`OnStartPaint`/`OnStartSubPaint`/`OnStopPaint`/`OnFocused`/`OnUpdate`/`OnMove`/`OnFindActiveControl`/`OnKeyDown`/`OnKeyPress`/`OnKeyUp`/`OnClick`/`OnDblClick`/`OnMouseDown`/`OnMouseMove`/`OnMouseUp`/`OnGetImage` |
| `DxRootRegistry` | `DxControls.cs` | `FRootCtrl` 逆查表（含整棵已挂子树的继承；`Remove` 时清空） |
| `DxProtected` | `DxControls.cs` | 上游 protected 虚方法的开实例委托（`DoCaptionChange` / `DoPaint` + `DoResize` 的 `MethodInfo`） |
| `IDxSurfacePainterExt` + `DxPainterExt` | `DxControls.cs` | `GameCanvas` 的着色/alpha/拉伸绘制（`FillRectAlpha` / `DrawColor` / `DrawColorAlpha` / `DrawBlend` / `StretchDraw`），未实现时退化 |
| `TDxScrollControl` | `DxControls.cs` | 原文 1730 的 `is TDxScrollControl` 判定目标 |
| `DxTickCount` | `DxControls.cs` | `MyGetTickCount` 可注入（动画单测用） |
| `TDxApplication` | `DxControls.cs` | 原文 `Application.MainForm`（引擎命名基准） |
| `DxControlsUnit` | `DxControls.cs` | `SetClipboardText` / `GetClipboardText` / `DebugOutStr` / `FindDxComponent` / `MakeGuiName` / `Pos` / `Copy` / `LowerCase` |
| `DxStringArrest` | `DxControls.cs` | `ArrestStringEx` |
| `TDxImageForm.MessagePumpSink` / `ApplicationTerminated` / `SleepSink` | `DxImageForm.cs` | `ShowModal*` 消息泵与退出条件可注入 |
| `TDxFormShapeImage.ApplyImageIndexGeometry` | `DxImageForm.cs` | 原文两处重复的几何推导（`SetImageIndex` 与 `ImageIndexChange`）收拢为一个可单测静态方法 |

### 6.2 下一轮建议清单（按收益排序）

1. **裁决 §5.8**：把 `DxLabel.cs` 的最小 `TDxImageButton` 接缝撤掉，并入全量实现（本波已写好）。
   这是解锁 `DxImageButton.pas`(866) 的唯一前置。
2. **补 `DxCtrlImageFormTests.cs`**：`TDxImageFormAnimation.AdvanceFrame`（含「先回调后回绕」与
   `TButtonAnimation` 的「先回绕后回调」差异断言）与 `BuildDrawPlan`（四边裁剪 + `SrcRect` 同步修）
   目前只有实现没有测试。
3. **`GfxFonts.pas`(847)**：可移植部分 = 字形缓存（`TGfxFontTextures.Add/Clear/FreeIdleMemory`）、
   `GetFontSize`（由 `TFont` 度量算出 `FFontWidth`/`FFontHeight`/`FFontWidthBold`/
   `FDoubleFontWidth`/…）、`TextWidth`/`TextHeight`、`GetFontTextureArray` 的**哈希键**逻辑；
   GDI 位图生成（`NewBitmapFile`）应接缝化。**新类型名不冲突**（`TGfxFontTexture` 等）。
4. **`Magnetic.pas`(755)**：核心是 `pvWndsConnected` / `pvCheckGlueing` / `pvSizeRect` / `pvMoveRect`
   四个纯矩形吸附算法（`TRECT` 运算），Win32 子类化（`zSubclass_Proc`/`SetWindowLong`）应接缝化。
   **新类型名不冲突**（`TMagnetic`）。
5. **`DxMagicBall.pas`(669)**：`TMagicBallOverallSetting` / `TMagicBallAloneSetting` 两个配置类
   + `TDXMagicBall.Paint`（约 440 行的角度/比例几何）。**新类型名不冲突**。
6. **把 `DxControlOps` 融入 `TDxControl`**（§5.7）：需要把上游 7 个只读文件里的
   `TDxControl` 改成 `partial`，然后把这些成员搬进去做真成员。这是**根治**下游控件可用性的动作，
   但必须由有写权限的归属批次执行。
7. **上游接缝修正两处**（§5.4/§5.5）：`MouseMoveed`/`MouseDowned` 改为转发到 `RootCtrl`；
   `OnMouseEnter`/`OnMouseLeave` 改名（如 `OnDxMouseEnter`）以避免与 WinForms 同名事件遮蔽。

### 6.3 已知偏差登记（不修正，仅记录）

| 项 | 偏差 | 影响 |
|---|---|---|
| `DebugOutStr` | 原文不处理 IO 异常；托管侧吞掉 `IOException`/`UnauthorizedAccessException` | 纯调试出口，无功能影响 |
| `SetClipboardText` | `GlobalAlloc`/`GlobalLock`/`SetAsHandle` → `Clipboard.SetText` | 语义等价 |
| `MouseUp` 里的 `FCanMouse := …` | 上游 `CanMouse` 只读且构造恒 `true`，写入无副作用 → 只保留门控变量 | 无功能影响（原文除此处外无写入者） |
| `SetCaptionA`/`SetCaptionV` | 上游 `Caption` setter 自己会调 `DoCaptionChange`，故用私有字段 `_caption` 直写（反射）以避免**触发两次** | 需要 `BindingFlags.NonPublic`；已实测原实现会双触发 |
| `TDxImageForm.DoShow/DoHide` | 上游无这两个虚方法 → 以 `protected virtual` 新方法承接覆写点 | 通过 `DxControlOps.OnShownCore/OnHiddenCore` 分发；语义一致 |
| `TDxImageFormShape` 的 8 字段布局 | `@FDraw1 + Inc(P,4)` → `Items[]` 数组 | 等价，不依赖字段布局 |
| `TDxControlEngine.Paint` | 上游 `Paint` 非 virtual → `new` + 改名 `PaintChildren`（`Update` → `UpdateChildren`） | 经 `TDxControl` 静态类型调用会落到基类空实现；引擎自身调用方走 `PaintChildren` |
| `Control.Name` | 非 virtual（已实测），无法覆写 → `DxControlHooks.NameOf` 槽位 + 同步到 `Control.Name` | `DxName` 属性暴露引擎名 |

---

## 7. 结论

- 本波**完成了优先级 1（`DxControls.pas` 4,147 行的余部，含全量 `TDxControl` 成员与
  `TDxControlEngine`）与优先级 2（`DxImageForm.pas` 1,263 行）**，共 3,898 行 C#、163 个用例、门禁全绿。
- **优先级 3（`DxImageButton.pas`）被 §5.8 的类型归属冲突阻塞**，实现已写好但需集成者先撤 `DxLabel.cs`
  里的最小接缝；**优先级 4-6（`GfxFonts` / `Magnetic` / `DxMagicBall`）预算未及**。
- 本波抓出 **2 个真实实现缺陷**（`SetDesigning` 未递归、`GetRootCtrl` 对引擎返回 nil）+ **1 个环境陷阱**
  （`ConditionalWeakTable.GetOrCreateValue` 不支持数组值类型）+ **2 处上游接缝误导**（字段型
  `MouseMoveed`、`OnMouseEnter` 被 WinForms 同名事件遮蔽），并产出 11 个可复用接缝。
- 全部改动只在独占区新增文件，**未修改任何只读文件**，未提交临时探查目录（`_probe_refl` 已删除）。
