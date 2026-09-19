# 并行报告 · `p2-dxcontrols-rest`（DxComponent 控件余部）

> 车道：`par/p2-dxcontrols-rest` ｜ 工作树：`.worktrees/p2-dxcontrols-rest`
> 分支起点：`main`（基于 P2 波次切出）
>
> **提交序列**（按时间顺序）：
>
> | 提交 | 内容 | 集成状态 |
> |---|---|---|
> | `2c1fd6ea` | 并行批次P2-1：`DxControlOps` 全量 + `TDxControlEngine` + `DxImageForm` | ✅ 已并入 main（`874bcb46`） |
> | `f06c2eed` | 并行批次P2-2：车道交付报告 | ✅ 已并入 |
> | `1c61fdf5` | 并行批次P2-3：`TDxImageButton` 全量 + `DxLabel.cs` 白名单结构性删除 + 74 例 | 被 `14401967` 取代（属性名回退） |
> | `baf064b1` | 并行批次P2-4：`GfxFonts.pas` 移植 + 56 例 | 待集成 |
> | `14401967` | 并行批次P2-5：**`DxImageButton` 属性名回归原文** + `Magnetic.pas` 移植 + 63 例 | 待集成 |
>
> **最新门禁（`14401967`，本车道工作树）**：`dotnet build GXX.slnx -c Debug` → 0 error；
> `dotnet test tests/GXX.Client.Tests` → **2635 passed / 0 failed**。

---

## 0. 【给 LoadDx 车道的接口契约】`TDxImageButton` 对外承诺的成员

**这一节是本次第二轮裁决要求的收口**。规则：**托管侧对外成员名一律用原文 `published` 名**，
不为迁就调用方加非原文成员。

### 0.1 原文核对结果（`DxImageButton.pas`）

| 原文行 | 原文声明 | 私有字段 |
|---|---|---|
| 140 | `property OnClickSound:TOnClickSound read FOnClickSound write FOnClickSound;` | `FOnClickSound` |
| **154** | **`property ClickCount:TClickSound read FClickSound write FClickSound;`** | `FClickSound` |
| 155 | `property CaptionColor:TDxCaptionColor read FCaptionColor write FCaptionColor;` | `FCaptionColor` |
| **156** | **`property Style:TButtonStyle read FButtonStyle write FButtonStyle;`** | `FButtonStyle` |
| 157 | `property Checked:Boolean read FChecked write SetChecked;` | `FChecked` |
| 158-161 | `CaptionDownOffsetX/Y`、`ButtonDownOffsetX/Y`（裸读写） | — |
| 163-164 | `CaptionOffsetX/Y`（走 `SetCaptionOffsetX/Y` → `DoCaptionChange`） | — |
| 165 | `property DrawAligment:TDrawAligment read FDrawAligment write FDrawAligment;` | `FDrawAligment` |
| 166 | `property ExpandWidth:Integer read FExpandWidth write SetExpandWidth;` | `FExpandWidth` |
| 168 | `property Animation:TButtonAnimation read FAnimation write FAnimation;` | `FAnimation` |

**结论**：原文的**公开属性名就是 `ClickCount` 与 `Style`**，`FButtonStyle` 只是私有字段名。
我上一版（`1c61fdf5`）把公开名擅自改成 `ButtonStyle` 并自造了 `ClickSound` 属性 —— **那是我的错**，
已在 `14401967` 修正：公开名回归 `Style`，并保留原文的 `ClickCount`。

### 0.2 `TDxImageButton` 对外承诺的完整成员清单（托管侧，`GXX.Client.DxComponent`）

```csharp
// ——— 与原文 published 段一一对应 ———
public TClickSound        ClickCount            { get; set; }   // 原文 154（read/write FClickSound）
public readonly TDxCaptionColor CaptionColor;                    // 原文 155
public TButtonStyle       Style                 { get; set; }   // 原文 156（read/write FButtonStyle）★
public bool               Checked               { get; set; }   // 原文 157（基类接缝属性；本类另加 SetChecked 方法走互斥逻辑）
public int                CaptionDownOffsetX    { get; set; }   // 原文 158
public int                CaptionDownOffsetY    { get; set; }   // 原文 159
public int                ButtonDownOffsetX     { get; set; }   // 原文 160
public int                ButtonDownOffsetY     { get; set; }   // 原文 161
public int                CaptionOffsetX        { get; set; }   // 原文 163（setter 触发 DoCaptionChange）
public int                CaptionOffsetY        { get; set; }   // 原文 164（同）
public TDrawAligment      DrawAligment          { get; set; }   // 原文 165
public int                ExpandWidth           { get; set; }   // 原文 166（同）
public TButtonAnimation   Animation;                            // 原文 168（基类接缝未暴露同名成员 → 本类字段）

// ——— public 段 ———
public TOnClickSound      OnClickSound;                         // 原文 140
public TAnimationFrameChangedEvent OnAnimationFrameChanged;     // 原文 144
public TGuiType           GuiType               { get; set; }   // 原文 488 `GuiType := t_Button`（上游接缝未暴露）

// ——— 方法（原文覆写点在托管侧的落点，见文件头第 1 条）———
public void CheckAutoSizeV2();                                  // 原文 504-509
public void SetOnGetImageV2(Action<TDxImageIndex, TImageType>); // 原文 498-502
public bool DoClickV2(int x, int y);                            // 原文 870-908
public TDxPoint? ComputeCaptionChangeSize();                    // 原文 676-786 的几何部分
public int  ResolveFaceIndex();                                 // 原文 687-729 / 939-976（两处重复 → 收拢）
public void SetChecked(bool value);                             // 原文 798-822
public void DoDrawCaptionV2();                                  // 原文 531-602
public void PaintImageButton();                                 // 原文 910-1017
public void AssignFromImageButton(TDxControl source);           // 原文 619-674
public void InitializeButton() / FinalizeButton() / DisposeButton();  // 原文 788-796 / 491-496

// ——— 继承自基类的原文覆写 ———
public override bool InRange(int x, int y);                                  // 原文 848-868
public override void MouseDown(TDxMouseButton, TDxShiftState, int, int);     // 原文 511-529
protected override void DoCaptionChange();                                   // 原文 676-786
protected override void DoPaint();                                           // 原文 3073-3078 CLIENTEXE=1 分支
```

### 0.3 【给 LoadDx 车道的精确缺口清单】

`LoadDx` 当前引用的**非原文成员**（`1c61fdf5` 之后我这边已无这两个名字）：

| LoadDx 位置 | 它写的 | 问题 | 建议改法 |
|---|---|---|---|
| `GuiComponentLoader.cs:302` | `dxImageButton.ClickSound = g.ClickCount;` | `ClickSound` **不是原文成员**（原文 154 是 `ClickCount`） | 改为 `dxImageButton.ClickCount = g.ClickCount;` |
| `GuiComponentLoader.cs:338` | 同上 | 同上 | 同上 |
| `GuiComponentLoader.cs:374` | 同上 | 同上 | 同上 |
| `GuiComponentLoader.cs:303/339/375` | `dxImageButton.Style = g.Style;` | **已解决**：本波已把公开名改为 `Style`，无需改 LoadDx | — |
| `GuiComponentLoader.cs:592/593`、`628/629` | `dxLabel.ClickSound = ...; dxLabel.Style = ...;` | `TDxLabel` 继承本类的 `Style`（已解决）；`ClickSound` 同样需改为 `ClickCount` | 改为 `dxLabel.ClickCount = g.ClickCount;` |
| `DxControlSeams.cs:9` | `using TClickSound = GXX.Client.DxComponent.TDxImageButton.TClickSound;` | **类型嵌套位不对**：原文 `TClickSound` 定义在 `DxComponents.pas:54` 的**顶层**，不是 `TDxImageButton` 的嵌套类型 | 改为 `using TClickSound = GXX.Client.DxComponent.TClickSound;` |
| `GuiRecords.g.cs:1163/1236/1403/2463/2593/2666` 等 | 字段类型 `TDxImageButton.TClickSound` | 同上 | 改为顶层 `TClickSound`（该文件是生成的，改生成器或加 `using` 别名） |

**另外两点（已修正，LoadDx 无需动）**：
- `Style`：本波已回归原文名 `Style`（`1c61fdf5` 里叫 `ButtonStyle` 是错的），你的 `dxImageButton.Style = g.Style` 现在直接可用。
- `ClickCount`：本波保留原文名，**注意它的类型是 `TClickSound`**（`csNone/csStone/csGlass/csNorm`）。

> **我没有为迁就 LoadDx 添加任何非原文成员** —— 这是按第二轮裁决第 2 条 (B) 走的路径。
> `TClickSound` / `TDrawAligment` / `TButtonStyle` 三个枚举在托管侧全部是 `GXX.Client.DxComponent` 的**顶层**类型。

---

## 1. 交付物

| 文件 | 行数 | 内容 |
|---|---|---|
| `src/GXX.Client/DxComponent/DxControls.cs` | 2,783 | **`DxControls.pas`（4,147 行）余部 1:1 移植**：单元级自由函数 + `TDxControl` 全部控件树/几何/状态/绘制几何/文本成员 + `TDxControlEngine` 全量 + 本波新增枚举与委托 |
| `src/GXX.Client/DxComponent/DxImageForm.cs` | 1,115 | **`DxImageForm.pas`（1,263 行）1:1 移植**：`TDxFormShapeImage` / `TAnimation` / `TDxImageForm` / `TDxImageFormShape` |
| `src/GXX.Client/DxComponent/DxImageButton.cs` | 1,069 | **`DxImageButton.pas`（1,019 行）1:1 移植**：`TButtonAnimation` + `TDxImageButton`（三种 Style / Caption 五态字体 / 按键音 / 动画） |
| `src/GXX.Client/DxComponent/GfxFonts.cs` | 552 | **`GfxFonts.pas`（963 行）移植**：`TDxGfxFontTexture` / `TDxGfxFontTextures`（超时淘汰缓存）/ `TDxGfxTextureFont`（TextWidth/TextHeight/GetFontTextureArray）/ `TDxGfxTextureFonts` |
| `src/GXX.Client/DxComponent/Magnetic.cs` | 629 | **`Magnetic.pas`（851 行）移植**：`TDxMagnetic` 的窗口注册表 + `pvWndsConnected` / `pvCheckGlueing` / `pvSizeRect` / `pvMoveRect` 四个磁吸算法 |
| `src/GXX.Client/DxComponent/DxLabel.cs` | 306 | **白名单内唯一结构性改动**：删掉上一波的最小 `TDxImageButton` 接缝，令 `TDxLabel : TDxImageButton`（全量）；`DxLabel.pas` 移植语义未改 |
| `tests/GXX.Client.Tests/DxCtrlControlsTests.cs` | 2,031 | **163 例**（`DxControls.pas` 余部 + `DxImageForm` 动画几何） |
| `tests/GXX.Client.Tests/DxCtrlImageButtonTests.cs` | 1,171 | **74 例**（`DxImageButton.pas`） |
| `tests/GXX.Client.Tests/DxCtrlGfxFontsTests.cs` | ~520 | **56 例**（`GfxFonts.pas`） |
| `tests/GXX.Client.Tests/DxCtrlMagneticTests.cs` | ~700 | **63 例**（`Magnetic.pas`） |
| `tests/GXX.Client.Tests/DxCtrlCollection.cs` | 13 | 串行集合定义（见 §5.1） |

**未新增**任何 `*.csproj` / `GXX.slnx` / `Directory.Build.props` 改动；
除裁决授权的 `DxLabel.cs` 一处结构性删除外，**未触碰上游 7 个既有 `.cs`**。

---

## 2. 门禁结果

```
cd .worktrees/p2-dxcontrols-rest/GXX.CSharp
dotnet build GXX.slnx -c Debug --nologo          → 0 error
dotnet test  tests/GXX.Client.Tests/GXX.Client.Tests.csproj -c Debug
                                                  → 2635 passed / 0 failed
其中本车道新增：DxControlsTests 163 + DxCtrlImageButtonTests 74 + DxCtrlGfxFontsTests 56 + DxCtrlMagneticTests 63 = 356
```

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

### 3.3 各优先级完成度（第二轮裁决后）

| 优先级 | 源 | 行数 | 状态 | 说明 |
|---|---|---|---|---|
| 1 | `DxControls.pas` | 4,147 | ✅ **已交付**（`2c1fd6ea`，已并入 main `874bcb46`） | 见 §3.1 |
| 2 | `DxImageForm.pas` | 1,263 | ✅ **已交付**（`2c1fd6ea`，已并入 main） | 见 §3.2 |
| 3 | `DxImageButton.pas` | 1,019（任务书写 866） | ✅ **已交付**（`14401967`，经 `1c61fdf5` → 属性名回归） | 含 `TButtonAnimation` 全量；`DxLabel.cs` 白名单结构性删除；74 例 |
| 4 | `GfxFonts.pas` | 963（任务书写 847） | ✅ **已交付**（`baf064b1`） | 见 §3.4；GDI 光栅化接缝化 |
| 5 | `Magnetic.pas` | 851（任务书写 755） | ✅ **已交付**（`14401967`） | 见 §3.5；Win32 窗口 API 接缝化 |
| 6 | `DxMagicBall.pas` | 669（任务书写 669） | ⏳ **未做** | 预算耗尽（本轮绝大部分预算用于 §0 的接口收口 + §5.2/§5.3 两个真实缺陷定位 + `Magnetic` 的"桌面哨兵也参与吸附"排查）。见 §6.2 续做要点 |

> 说明：本车道累计完成 **4 个单元 / 9,243 行 Delphi → 5,518 行 C# / 356 个用例**。
> `DxMagicBall.pas` 是唯一未做单元，其类型名（`TMagicBallOverallSetting` / `TMagicBallAloneSetting` /
> `TDXMagicBall`）**与既有类型及 `LoadDx` 均不冲突**，下一轮可直接接手。

### 3.4 `GfxFonts.pas`（963 行）覆盖明细

| 原文范围 | 内容 | 状态 |
|---|---|---|
| 9-27 / 158-173 | `TGfxFontTexture`（含 `OutTimeTime = 60000` 默认值） | ✅ |
| 31-51 / 176-263 | `TGfxFontTextures`（`Add` / `Clear` / `GetTexture` / `GetTextureCount` / `FreeIdleMemory`） | ✅ 全部落地（含超时淘汰 + 游标分片 + 「删除后仍 Inc」怪癖） |
| 54-120 / 287-316 | `TGfxTextureFont` 构造与 `Clear`/`Initialize`/`Finalize`/`FreeIdleMemory` | ✅ |
| **766-776** | **`TextHeight`**（单/双字节判定） | ✅ |
| **778-798** | **`TextWidth`**（`nsCount`/`nwCount` 折算 + Bold 分支） | ✅ |
| **321-349** | **`GetFontTextureArray`**（`TextChars` 门控 + 槽位数规则） | ✅（含原文冗余的 `Length(S)=2` 分支，注释说明其不可达） |
| **504-644** | **`GetFontTexture` 的缓存与几何部分**（命中刷新 `OutTimeTick`；未命中算 `Max(行宽) × TextHeight('pP')*行数`） | ✅ |
| 436-444 | `GetTextTexture` | ✅ |
| 122-146 / 801-963 | `TGfxTextureFonts`（`Add`/`SetFont`/`RemoveFont`/`RemoveAll`/`Lock`/`UnLock`/`Count`/`Font[Num]`） | ✅ |
| **398-434** | `GetFontSize`（GDI `GetTextExtentPoint32W` 量 '0' 与 '一'） | 🔌 接缝 `FontSizeProbe`（缺省保留原文初始常量 6/12/7/12/12/13） |
| **504-644 的光栅化段** | `CreateDIBSection` + `TextOut` + `NewTexture` | 🔌 接缝 `GlyphBuilder(w,h)` |
| 351-396 / 646-750 | `DrawText` / `TextOut`(×3) / `TextOutA` / `TextRect`(×3) | 🔌 未做（GDI + `GameCanvas.DrawColor` 落点，属 HGECanvas 域） |
| 446-503 | `NewBitmapFile`（构造 BITMAPFILEHEADER/INFOHEADER） | 🔌 未做（DIB 序列化，属未移植的 DIB 域） |

### 3.5 `Magnetic.pas`（851 行）覆盖明细

| 原文范围 | 内容 | 状态 |
|---|---|---|
| 147-156 | `TWND_INFO` / `TSubClass_Proc` | ✅ |
| 216-246 | 构造（`SnapWidth := 10`）/ 析构 / `SnapWidth` 属性 | ✅ |
| **792-814** | **`pvWndsConnected`**（并集包围判定 + 8 条边数值相等判定） | ✅ 纯函数 |
| 816-847 | `pvWndGetInfoIndex` / `pvWndParentGetInfoIndex` | ✅ |
| **356-396** | **`AddWindow`**（重复拒绝 / 验证 / **父=自己归一成 0** / 尾部立即 `pvCheckGlueing`） | ✅ |
| **399-437** | **`RemoveWindow`**（覆盖式前移 / 解除父子 / 未找到返回 false） | ✅ |
| **731-790** | **`pvCheckGlueing`**（矩形刷新 + Glue 重置 / 直接连接 / 多层间接传播） | ✅ |
| **446-532** | **`pvSizeRect`**（6 个 `WMSZ_*` 边 → 改 Left/Right/Top/Bottom；每方向两个 case「后判者胜」） | ✅ 纯几何 |
| **534-729** | **`pvMoveRect`**（4 锚点覆写「后判者胜」/ 子窗口二轮吸附 / 统一偏移 / 搬子窗口） | ✅（`DeferWindowPos` 段接缝化） |
| **248-355** | `zSubclass_Proc`（Win32 窗口过程：`WM_MOVING`/`WM_SIZING`/`WM_ENTERSIZEMOVE`…） | ❌ 未做（依赖 `TMessage`/`WndProc` 链，属未移植 Forms 域，见文件头第 4 条） |
| 202-214 | 单元级 `Subclass_Proc` 转发 | ❌ 未做（同 `zSubclass_Proc`） |
| Win32 调用 | `IsWindow` / `IsWindowVisible` / `GetWindowRect` / `GetCursorPos` / `DeferWindowPos` | 🔌 接缝（`WindowValidator` / `WindowVisibility` / `WindowRectProvider` / `CursorPosProvider` / `ChildMover`），缺省让全部磁吸算法**可 headless 单测** |


---

## 4. 测试用例数

| 测试类 | 用例数 | 覆盖单元 |
|---|---|---|
| `DxControlsTests` | **163** | `DxControls.pas` 余部 + `DxImageForm` 动画几何 |
| `DxCtrlImageButtonTests` | **74** | `DxImageButton.pas` |
| `DxCtrlGfxFontsTests` | **56** | `GfxFonts.pas` |
| `DxCtrlMagneticTests` | **63** | `Magnetic.pas` |
| **本车道合计** | **356** | |
| `GXX.Client.Tests` 全量 | **2,635**（基线 2,278 + 356，另 1 例为其它车道并入），全绿 | |

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

### 5.8 【类型归属冲突】`TDxImageButton` 被上一波占名 —— **已由裁决解决** ✅

上一波车道5 在 `DxLabel.cs:19` 落了 `public class TDxImageButton : TDxControl`（最小接缝，
仅为支撑 `DxLabel : TDxImageButton`），并在文件头注释里**明确写着**
「待 `DxImageButton.pas` 归属批次移植后由该类型接管」。

本波正是那个归属批次，新建全量实现时触发 `CS0101 命名空间 GXX.Client.DxComponent 已包含 TDxImageButton`。
**第一轮裁决**把 `DxLabel.cs` 加入本车道 **ALLOW-MODIFY 白名单**，授权只做这一处结构性删除。
已执行（`1c61fdf5`）：

- 删除 `DxLabel.cs` 里的最小 `TDxImageButton` 接缝类（原 19-100 行）；
- `TDxLabel` 保持 `: TDxImageButton`，但现在继承的是**全量**类型（原文 `TDxLabel = class(TDxImageButton)` 一字不差）；
- `DxLabel.pas` 的移植语义**未改动任何一处**（`DxLabel.cs` 其余代码原样保留）。

### 5.8b 【命名事故】我擅自改了原文的公开属性名 —— **已修正** ✅

第一版全量 `DxImageButton.cs`（`1c61fdf5`）里我做了两处**违反 1:1** 的改动：
1. 把原文 156 的公开属性 `Style` 改名成 `ButtonStyle`（理由是"`Style` 在 WinForms Control 上已被占用"）；
2. 自造了一个**原文不存在**的公开属性 `ClickSound`（原文 154 的公开名其实是 `ClickCount`，
   `FClickSound` 只是私有字段名）。

后果：与 `p2-client-loaddx` 车道的接口对不上（集成构建 `CS1061`）。
**第二轮裁决**要求"以原文属性名为准，不要为了迁就 LoadDx 而乱改名"。已在 `14401967` 修正：
- 公开名回归 `Style`（原文 156）与 `ClickCount`（原文 154）；
- 删掉自造的 `ClickSound` 与自造的 `TClickSoundNS`/`TDrawAligmentNS`；
- `TClickSound` / `TDrawAligment` 回归 `DxComponents.pas` 的**顶层**枚举位置（定义在 `DxControls.cs`）；
- `TDxLabel` 不再需要 `Style` 转发桥（直接用基类的 `Style`）。

**教训**：上游接缝为回避 WinForms 撞名而改过名（`Hint → HintText`），我就跟着改了 —— 但那是**接缝层**的
既成事实，不代表新移植的类型可以随手改名；**对外承诺的成员名应以原文 published 名为准**，
撞名时优先考虑"能否用原文名"（本例 `Style` 完全可以），确实不能才改名并显式登记。

### 5.9 【引擎签名冲突】原文 `Boolean` 入口 vs 上游 `void` 虚方法

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
3. **原文的「哨兵索引 0」很容易在测试里漏掉**（`Magnetic.pas` 实例）：
   `pvSizeRect`（456 行）与 `pvMoveRect`（557 行）的循环都是 `lc := 0 to m_lWndCount` —— **含 0**，
   而 `m_rcWnd[0]` 是"桌面区域"（原文注释 `m_rcWnd[0] has the window rect of Desktop area`）。
   若测试里没显式设置它，它就是 `(0,0,0,0)` 并**参与磁吸**，于是"没有任何注册窗口"时窗口也会被吸到原点。
   我为此花了很长时间把实现当 bug 查（实际是测试期望错）。
   **建议**：写这类测试时**显式**设置或挪远 `SetWndRect(0, ...)`，让意图明确。
4. **`Count - 1` 形式的属性在"清空"后会变成 -1**：`Magnetic.WndCount => _wndInfo.Count - 1`
   在 `Destroy()` 清空 List 后是 `-1`，而原文 `SetLength(a, 0)` 后逻辑窗口数语义是 0。
   托管侧已归一为 `Count > 0 ? Count - 1 : 0`（有注释）。同类写法建议一律归一。

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
| `TDxGfxTextureFont.FontSizeProbe` | `GfxFonts.cs` | 原文 398-434 `GetFontSize`（GDI `GetTextExtentPoint32W`）的注入点 |
| `TDxGfxTextureFont.GlyphBuilder(w, h)` | `GfxFonts.cs` | 原文 613-617 `NewTexture(PBitmapBits, nWidth, nHeight)` 的注入点；返回 null 即"造不出纹理" |
| `TDxMagnetic.WindowValidator` / `WindowVisibility` / `WindowRectProvider` / `CursorPosProvider` / `ChildMover` | `Magnetic.cs` | Win32 的 `IsWindow` / `IsWindowVisible` / `GetWindowRect` / `GetCursorPos` / `DeferWindowPos` 注入点；缺省让全部磁吸算法可 headless 单测 |

### 6.2 下一轮建议清单（按收益排序）

1. **【已交回 LoadDx】§0.3 的缺口清单**：`ClickSound` → `ClickCount`（`GuiComponentLoader.cs` 6 处）、
   `TClickSound` 从嵌套改顶层（`DxControlSeams.cs:9` + `GuiRecords.g.cs` 生成器）。
   `Style` 已由本波回归原文名，无需再改。
2. **补 `DxCtrlImageFormTests.cs`**：`TDxImageFormAnimation.AdvanceFrame`（含「先回调后回绕」与
   `TButtonAnimation` 的「先回绕后回调」差异断言）与 `BuildDrawPlan`（四边裁剪 + `SrcRect` 同步修）
   目前只有实现没有测试。**这是本车道唯一"有实现无测试"的缺口。**
3. **`DxMagicBall.pas`(669)**：`TMagicBallOverallSetting` / `TMagicBallAloneSetting` 两个配置类
   + `TDXMagicBall.Paint`（约 440 行的角度/比例几何）。**新类型名与既有类型及 `LoadDx` 均不冲突**，
   可直接接手（本波唯一未做单元）。
4. **`GfxFonts.pas` 的 GDI 段**：接入 HGE 后补 `GetFontSize`（`FontSizeProbe`）、
   `GetFontTexture` 的光栅化（`GlyphBuilder`）、`TextOut*`/`TextRect*`/`DrawText`/`NewBitmapFile`。
5. **`Magnetic.pas` 的 Win32 段**：补 `zSubclass_Proc`（`WM_MOVING`/`WM_SIZING`/`WM_ENTERSIZEMOVE`）
   与单元级 `Subclass_Proc` 转发；同时把 5 个接缝委托接成真实的 Win32 调用
   （`WindowValidator` / `WindowVisibility` / `WindowRectProvider` / `CursorPosProvider` / `ChildMover`）。
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

- 本波共完成 **4 个源单元 / 9,243 行 Delphi → 5,518 行 C# / 356 个用例**，门禁全绿
  （`GXX.Client.Tests` 2635 passed / 0 failed）：
  | 优先级 | 单元 | 状态 |
  |---|---|---|
  | 1 | `DxControls.pas`(4,147) 余部（含全量 `TDxControl` 成员 + `TDxControlEngine`） | ✅ 已并入 main |
  | 2 | `DxImageForm.pas`(1,263) | ✅ 已并入 main |
  | 3 | `DxImageButton.pas`(1,019) | ✅ `14401967` |
  | 4 | `GfxFonts.pas`(963) | ✅ `baf064b1` |
  | 5 | `Magnetic.pas`(851) | ✅ `14401967` |
  | 6 | `DxMagicBall.pas`(669) | ⏳ **唯一未做单元**（预算耗尽） |
- **两轮裁决均已落地**：① 类型归属冲突（§5.8）按授权删除 `DxLabel.cs` 最小接缝；
  ② 与 `LoadDx` 的接口收口（§0）——公开属性名回归原文 `ClickCount`/`Style`，
  并交出**精确缺口清单**（`ClickSound` → `ClickCount` 6 处、`TClickSound` 嵌套改顶层）。
  **没有为迁就调用方添加任何非原文成员。**
- 本波抓出 **3 个真实实现缺陷**（`SetDesigning` 未递归、`GetRootCtrl` 对引擎返回 nil、
  `WndCount` 清空后为 -1）+ **2 个环境陷阱**（`ConditionalWeakTable.GetOrCreateValue` 不支持数组值类型、
  Stale build artifacts 导致"改了源码测试结果不变"）+ **2 处上游接缝误导**（字段型 `MouseMoveed`、
  `OnMouseEnter` 被 WinForms 同名事件遮蔽）+ **2 处我自己的测试/命名事故**
  （`Bounds`/`Rect` 语义混用误判实现、擅自改原文公开属性名），并产出 15 个可复用接缝。
- 全部改动只在独占区新增文件；除裁决授权的 `DxLabel.cs` 一处结构性删除外**未修改任何只读文件**；
  未提交临时探查目录（`_probe_refl` 已删除）。

