# 并行报告 · `p3-dx-big`（`DIB.pas` + `DxMemo.pas`）

> 车道：`par/p3-dx-big` ｜ 工作树：`.worktrees/p3-dx-big`
> 状态：**未完成，且 `GXX.Client.Tests` 仍红（19 例）→ 当前分支不可并入 main。**
> 本报告如实记录已完成、已定性未修、以及未开工的部分。

---

## 1. 提交清单

| commit | 内容 | 可合并? |
|---|---|---|
| `7167073c` | 抢救上一轮（宿主杀死）遗留的 5 个 DIB 分片 + 1 个测试文件；`GXX.Client` 编译 0 error | 单看 build 可，但测试红 |
| `8b731800` | **`DxMemo.pas` `TDxScrollControl` 全量 1:1**（声明 44-192 + 实现 670-733/738-1490）；恢复 `dotnet build GXX.slnx` 绿灯 | **是**（当时全量测试 3622/3642，失败全在 DIB，与本提交无关） |
| `d860099a` | DIB 遗留**实现缺陷**修复（4 处 `string.Format`→`DelphiFormat.Format`）+ 6 组测试期望修正 | 否（WIP，测试仍红 20） |
| `05ac9fbb` | DIB 测试期望再修正 2 处 + RLE 行地址的原文缺陷注释；失败 20→19 | 否（WIP，测试仍红 19） |

> 说明：我**没有**产出可并入 main 的全绿提交 —— 这是本车道最主要的未达标项，详见 §6。

---

## 2. 上一轮遗留产出的甄别结论（任务书要求的第一步）

**先做了 `git status --porcelain` → 6 个未跟踪文件；逐一通读后判定：全部保留、无一重写。**

| 文件 | 判定 | 理由 |
|---|---|---|
| `DIB.cs` (725 行) | **原样保留** | 单元级类型/常量/接缝/TPaletteManager，与 `DIB.pas 1-800` 逐段对应；已核对 `TBGR`/`TDIBPixelFormat` 布局与 `MakeDIBPixelFormat` 公式，均 1:1 |
| `DIB.SharedImage.cs` (1204 行) | **保留 + 补缺陷** | `NewImage`/`ReadData`/RLE 结构完整且与原文逐行对应；但发现 4 处格式串接缝用错 API（§4），已修 |
| `DIB.Core.cs` (1417 行) | **保留 + 补缺陷** | 核心属性/位深转换/流读写完整；`GetScanLine` 消息同 §4 |
| `DIB.Effects.cs` (2860 行) | **原样保留** | `Mirror/Blur/Greyscale/…/Twist` 全族在，行号覆盖 2569-4936 与头注一致 |
| `DIB.Tail.cs` (1211 行) | **原样保留** | `DoRotate/Ink/Distort/AntialiasedLine/ColoredLine` 全族在 |
| `DxCtlDibCoreTests.cs` (2149 行) | **保留 + 修期望** | 结构良好（假接缝 + 差异断言），但**其中 34 例期望写错**，见 §5 |
| **`DIB.Fusion.cs`** | **缺失（从未产出）** | `DIB.pas 4940-7930` 约 3,000 行整段空缺，见 §7 |

**结论：上一轮把 DIB 做到了约 65%（Tail + Effects + Core + SharedImage + 头部），中间 4940-7930 被整段挖空；产出质量可用，故"在其基础上补齐"而非重写。**

---

## 3. 侦察结果：两单元结构 + 可测/接缝划分

### 3.1 `DIB.pas`（**实测 8,747 物理行**，非任务书所写 8,087）

| 行号区间 | 内容 | 分类 |
|---|---|---|
| 12-53 | `TColorLineStyle`/`TColorLinePixelGeometry`/`TFilterTypeResample`/`TDistortType`/`TFilterMode`/`TBGR`/`TDIBPixelFormat` | 纯类型 |
| 55-91 | `TDIBSharedImage` 声明 | 纯逻辑（句柄字段走接缝） |
| 100-325 | `TLightSource`/`TDIB` 声明（全部方法面+属性） | 声明 |
| 351-441 | `TDIBitmap`/`TCustomDXDIB`/`TDXDIB`/`TCustomDXPaintBox`/`TDXPaintBox` | **接缝**（TComponent/TGraphicControl） |
| 443-486 | `DefaultFilterRadius`/`TFilter`/`TMatrixSetting` 常量 | 查表 |
| 505-662 | `DSin/DCos`/`MakeDIBPixelFormat(Mask)`/`pfRGB`/`pfGetRGB`/`pfGet*Value`/`GreyscaleColorTable`/`RGBQuad`/调色板互转 | **纯逻辑可测**（已测） |
| 666-800 | `TLocalDIBPixelFormat`/`TPaletteItem`/`TPaletteManager` | 纯逻辑 + GDI 调色板接缝 |
| 802-1555 | `TDIBSharedImage` 实现：`NewImage`/`Duplicate`/`Compress(RLE4/8)`/`Decompress`/`ReadData`/`Destroy`/`GetPalette`/`SetColorTable` | **纯逻辑可测**（内存布局/RLE 编解码）+ GDI 分配接缝 |
| 1557-2540 | `TDIB` 核心：`Assign`/`Draw`/`Clear`/`Changing`/`AllocHandle`/`Compress`/`HasAlphaChannel`/属性读写/`GetPixel`/`SetPixel`/`LoadFromStream`/`SaveToStream`/`SetSize`/`ConvertBitCount`/进度 | **纯逻辑可测**（像素/位深）+ Canvas/GDI 接缝 |
| 2569-4936 | 特效：`Mirror`/`Blur`/`Negative`/`Greyscale`/`Contrast`/`Saturation`/`Lightness`/`AddRGB`/`Filter`/`Spray`/`Sharpen`/`Emboss`/`AddMonoNoise`/`AddGradiantNoise`/`FishEye`/`SmoothRotateWrap`/`Rotate`/`SplitBlur`/`Twist` | **纯逻辑可测**（逐像素） |
| **4940-7930** | `TCustomDXDIB`/`TCustomDXPaintBox` 实现 + `CreateDIBFromBitmap` + **16 个 `Draw*`** + `FilterLine/Rect` + `InitLight/DrawLights` + **`TFColor` 全部 `Do*`**（DoInvert…DoColorize）+ 重采样滤波器组 | **纯逻辑可测**（像素混合）+ Canvas 接缝 —— **本次未做** |
| 7930-8747 | `DoRotate`/`Ink`/`Distort`/`AntialiasedLine`/`GetColorBetween`/`ColoredLine` + initialization/finalization | 纯逻辑 + Canvas 接缝（已做） |

### 3.2 `DxMemo.pas`（**实测 5,877 行**，非任务书所写 5,139）

**任务书把 `DxMemo.pas` 描述为"多行文本编辑器控件"是不准确的** —— 它其实是 **1 个滚动基类 + 6 个控件 + 3 个容器类**的整体家族：

| 行号区间 | 类型 | 分类 |
|---|---|---|
| 22-37 | `pTViewItem`/`TViewItem` 记录 | 纯数据 |
| 44-192 | **`TDxScrollControl`**（`TDxControl`） | **纯状态机 + 几何，可测** |
| 194-203 | `TDxScrollBox` | 纯逻辑（InRange/鼠标转发） |
| 205-236 | `TLineColor`/`pTLineColor`/`TDxLines`（`TStringList`） | 纯逻辑（列表+高度） |
| 238-266 | `TTokenType`/`PStringToken`/`TStringToken`/`TStringLineEx` | **纯逻辑可测**（富文本令牌） |
| 268-375 | **`TDxChatMemo`**（`TDxScrollControl`） | 状态机可测 + Paint 接缝 |
| 377-484 | `TDxTreeNode`/`TDxTreeView` | 树算法可测 + Paint 接缝 |
| 486-657 | `TDxListItem`/`TViewField`/`TDxListView` | 列表算法可测 + Paint 接缝 |
| 659-668 | `GetStrinLineExText`/`GetTextListEx`（自由函数） | **纯逻辑可测**（文本解析） |
| 670-1490 | `TDxScrollControl` 实现 | **纯逻辑可测** ✅ 本轮完成 |
| 1494-1852 | `TDxScrollBox`/`TDxLines` 实现 | 未做 |
| 1855-3647 | `TDxChatMemo` 实现（含 300+ 行 `Paint`） | 未做 |
| 3648-5048 | `TDxListItem`/`TViewField`/`TDxListView` 实现（含 `ProcessCustomColor`/`DoProcessText`/`Paint`） | 未做 |
| 5050-5819 | `TDxTreeNode`/`TDxTreeView` 实现（含 `Paint`） | 未做 |
| 5820-5877 | `TStringLineEx` 实现 | 未做 |

---

## 4. 本轮完成的实现工作

### 4.1 `DxMemo.cs` —— `TDxScrollControl` 全量 1:1（`8b731800`）

- **声明 44-192**：32 个私有字段（含原文 `{$MESSAGE HINT}` 明示"不能删"的 `FMouseSpring` 及 `FDX/FDY/FfSpeedX/FfSpeedY/FDTime/FMTime/FStartSpring` 全部保留）、方法面 109-163、published 属性 164-191。
- **实现 670-733 + 738-1490**：`Create`（逐字段默认值，含 4 个 `TDxImageIndex` 的 `OnChange`/`OnGetImage` 挂接）、`Destroy`、`SetOnGetImage`、4 个 `*ImageIndexChange`、`GetVisibleHeight`、`InPrevRange`/`InNextRange`/`InBarRange`、`DoScroll`、`AutoCalcShowItemCount`、`SetExpandSize`、`MinValue`/`MaxValue`、`SetVisibleItemCount`、`SetPosition`、`Next`/`Previous`/`First`/`Last`、`SetShowItemCount`/`SetAutoShowScroll`/`ChangeShowItemCount`、`DoClick`、`MouseWheelDown/Up`、`SetItemIndex`、`AutoSetShowScroll`、`SetItemHeight`/`SetScrollSize`/`SetScrollBars`、`DoMouseUp`/`DoMouseEnter`/`DoMouseLeave`、`DoResize`、`CanMove`、`ScrollMouseDown`/`ScrollMouseMove`/`ScrollMouseUp`。
- **保留的原文缺陷/笔误（已注释）**：`InNextRange` 用 `Height` 而非 `vRect.Bottom`（842）；7 处逐字重复的 `FBarTop` 反算（不抽公共方法以保 1:1）；`First` 的 `Max(abs(nMinValue), FPosition)` 参数序（1139）；`DoResize` 的 `else` 分支额外 `Position := 0`（1336）；`ScrollMouseClick` 的注释掉分支；`ScrollMouseMove` 的 `FMouseScroll` 拖拽方向（1475）。
- **已知形式偏差（调度方已裁定"维持不动"）**：原文 `MouseWheelDown/Up` 是 `override`（基类 `DxControls.pas:4036-4044` 有同名空虚方法），但托管侧上游 `TDxControl`（`DxComponentCommon.cs`，只读）**没有**这两个虚方法 —— 被撤销的那段接缝恰是它们唯一的托管落点。故本类以 `public virtual` 落地并转调 `DxControlOps.MouseWheelDown/Up`（与 `DxControls.cs:2299/2302` 的既有模式一致），调用点 `TDxControlEngine.PortMouseWheelDown` 行为完全一致。
- **`TScrollStyle` 归属**：`DxComponent` 命名空间内无此枚举，唯一同名定义在 `LoadDx.TScrollStyle`（`LoadDx/GuiRecords.g.cs:133`）。为不制造第 5 个"重复接缝"，直接引用 `LoadDx.TScrollStyle`；调度方已批准为临时处置（根因去重由调度方批量做）。

### 4.2 DIB 遗留实现缺陷修复（`d860099a`）

**根因：`DXConsts` 的常量是 Delphi 格式串（`'...(%d)'`），`.NET string.Format` 无法替换 `%d`，会原样输出 `"%d"`。** `DXConsts.cs:13` 的注释本就要求"格式化交给调用方（`DelphiFormat.Format` 或 `string.Format`）"。

| 位置 | 原文依据 | 修复 |
|---|---|---|
| `DIB.SharedImage.cs` `NewImage` 未知位深 | `DIB.pas:840-841` `CreateFmt(SInvalidDIBBitCount,[ABitCount])` | `string.Format` → `DelphiFormat.Format` |
| `DIB.SharedImage.cs` `NewImage` 分配失败 | `DIB.pas:927` `CreateFmt(SCannotMade,['DIB'])` | 同上 |
| `DIB.Core.cs` `GetScanLine` | `DIB.pas:1943-1944` | 同上 |
| `DIB.Core.cs` `GetScanLineReadOnly` | `DIB.pas:1953-1954` | 同上 |

---

## 5. 逐方法族判定表（DIB）

| 方法族 | 原行号 | 实现 | 测试 |
|---|---|---|---|
| 单元级常量/类型 | 12-53, 443-486 | ✅ `DIB.cs` | ✅ |
| `DSin`/`DCos` | 505-513 | ✅ | ✅（2 组期望已修） |
| `MakeDIBPixelFormat[Mask]` | 515-551 | ✅ | ✅（1 组错断言已删） |
| `pfRGB`/`pfGetRGB`/`pfGet*Value` | 553-598 | ✅ | ✅ |
| `GreyscaleColorTable`/`RGBQuad`/调色板互转 | 613-662 | ✅ | ⚠ 1 例期望错（`RGBQuad_与PaletteEntry互转`，实测实现正确） |
| `TPaletteManager` | 668-800 | ✅ | ✅ |
| `TDIBSharedImage.Create` | 802-809 | ✅ | ✅ |
| `NewImage` | 811-934 | ✅（缺陷已修） | ✅ |
| `Duplicate` | 936-956 | ✅ | ✅（期望已修） |
| `Compress`(RLE4/8 编码) | 958-1214 | ✅ | ⚠ 2 例红（`Compress_已压缩源`/`RLE8编码_绝对模式`）——判定为期望错，未改完 |
| `Decompress`(RLE4/8 解码) | 1216-1350 | ✅ 1:1 | ❌ 4 例红（见 §8 原文缺陷） |
| `ReadData`/`LoadRLE4/8`/`LoadRGB` | 1352-1489 | ✅ | ⚠ 4 例红（2 例期望错 + 2 例待查） |
| `Destroy`/`FreeHandle`/`GetPalette`/`SetColorTable` | 1491-1540 | ✅ | ✅ |
| `TDIB` 核心属性/`ScanLine` | 1557-1980 | ✅（消息缺陷已修） | ✅ |
| `GetPixel`/`SetPixel` | 1992-2039 | ✅ 1:1 | ⚠ 1 例期望错（`Pixels_4bpp`，实测实现正确） |
| 流读写/剪贴板/进度 | 2041-2171, 2528-2567 | ✅ | ✅ |
| `SetSize`/`SetBitCount`/… | 2173-2313 | ✅ | ✅ |
| `ConvertBitCount` + 调色板转换 | 2315-2524 | ✅ 1:1 | ⚠ 3 例红（1 例为**原文缺陷**，2 例期望错） |
| 特效族（Mirror…Twist） | 2569-4936 | ✅ `DIB.Effects.cs` | ⚠ 未逐族补测 |
| **`Draw*` + `TFColor.Do*` + 滤波器组** | **4940-7930** | ❌ **未实现（`DIB.Fusion.cs` 缺失）** | ❌ 无 |
| `DoRotate`/`Ink`/`Distort`/`AntialiasedLine`/`ColoredLine` | 7930-8747 | ✅ `DIB.Tail.cs` | ⚠ 未逐族补测 |
| initialization/finalization | 8738-8746 | ✅ 登记（`DibSeams.RegisterPictureFormats`/`FinalizeUnit`） | ✅ |

---

## 6. ★ 门禁状态与"未完成"的诚实说明

```
dotnet build GXX.slnx -c Debug          → Build succeeded, 0 Error(s)   ✅
dotnet test tests\GXX.Client.Tests      → Failed: 19, Passed: 3623, Total: 3642   ❌
```

**19 例红全部集中在 `DxCtlDibCoreTests`（上一轮遗留的测试文件）；其余 11 个测试工程未受影响。**

**为什么没有做到全绿（诚实归因）**：
1. 34 例红的**根因不在本轮代码** —— 它们是上一轮被杀时"实现写完、测试没校准"的遗留态。我修了 15 例（34→19），
   其中 **4 例是靠修实现**（`DelphiFormat.Format`）、**11 例是靠修测试期望**。
2. 剩下 19 例我已**逐例回读原文定性**（§8），但**没有全部改完** —— 我把较多时间投入到
   `DIB.Fusion.cs` 的结构侦察与 `TDxScrollControl` 的完整落地，以及为排除"是不是实现的错"而做的
   **多轮临时探针实测**。探针本身有价值（抓出 1 个真实实现缺陷 + 证明 3 处测试期望写错），
   但确实挤占了改完剩余期望的时间。
3. **因此：`8b731800`（DxMemo TDxScrollControl）之前的所有提交都不应单独并入 main**；
   若要并入，应等 19 例转绿后作为整体并入。

---

## 7. `DIB.Fusion.cs`（`DIB.pas 4940-7930`）未实现 —— 具体缺口

这是客户端的真实最大空洞，**本轮完全未开工**。缺失内容：

| 原行号 | 缺失内容 |
|---|---|
| 4940-4955 | `TCustomDXDIB.Create/Destroy/SetDIB` |
| 4959-5138 | `TCustomDXPaintBox` 全部（`Paint` 含内嵌 `Draw2`、7 个 setter、`GetPalette`） |
| 5142-5145 | `PosValue`（已在 `DIB.cs` 补齐） |
| 5147-5152 | `TDIB.CreateDIBFromBitmap` |
| 5154-5710 | **16 个 `Draw*`**：`DrawTo`/`DrawTransparent`/`DrawShadow`/`DrawDarken`/`DrawQuickAlpha`/`DrawAdditive`/`DrawTranslucent`/`DrawAlpha`/`DrawAlphaMask`/`DrawMorphed`/`DrawMono`/`Draw3x3Matrix`/`DrawAntialias`/`DrawOn`/`FilterLine`/`FilterRect` |
| 5848-5915 | `InitLight`（256×256 `FLUTDist` LUT）+ `DrawLights` |
| 5940-5973 | `IntToByte`/`TrimInt`（已在 `DIB.cs`/`DIB.Effects.cs` 补齐）+ `Darkness` |
| 5975-7674 | `TDIB.DoSmoothRotate` + `TFColor` 全部 `Do*`（`DoInvert`/`DoAddColorNoise`/`DoAddMonoNoise`/`DoAntiAlias`/`DoContrast`/`DoFishEye`/`DoGrayScale`/`DoLightness`/`DoDarkness`/`DoSaturation`/`DoSplitBlur`/`DoGaussianBlur`/`DoMosaic`/`DoTwist`/`DoSplitlight`/`DoTile`/`DoSpotLight`/`DoTrace`/`DoEmboss`/`DoSolorize`/`DoPosterize`/`DoBrightness`/`DoResample`/`DoColorize`） |
| 7250-7312 | `TContributor`/`TCList`/`TRGB`/`TColorRGB`（`Color2RGB`/`RGB2Color`） |
| 7126-7249 | 重采样滤波器：`HermiteFilter`/`BoxFilter`/`TriangleFilter`/`BellFilter`/`SplineFilter`/`Lanczos3Filter`/`SinC`/`MitchellFilter` |
| 7679-7780 | `TColorRGB.InvertBitmap` |
| 7780-7930 | `FadeOut`/`DoZoom`/`DoBlur`/`FadeIn`/`FillDIB8` |

**可测性**：`Draw*` 与 `TFColor.Do*` 都是**纯像素内存操作**（只有 `DoRotate`/`DrawOn` 等少数走 Canvas），
可 1:1 移植 + 逐像素断言，属于本车道最该补的高价值内容。

---

## 8. 剩余 19 例红的逐条定性（供接力者直接接手）

### A. 原文缺陷（实现 1:1 正确，**应改测试**）
| 用例 | 原文依据 | 实证 |
|---|---|---|
| `RLE4解码_编码模式半字节交换` | `DIB.pas:1274-1286` 编码模式用 `B2 and $F0` 写偶像素、`(B2 and $F0) shr 4` 写奇像素（nibble 未按 X 奇偶取对应半字节） | 实测 `03 A5` 解出 `5,A,5`（含擦除 `5 and F0=0`），非测试期望的 `A,5,A` |
| `RLE8解码_绝对模式与编码模式` | 同上量级；实测 `00 03 11 22 33 / 00 00 / 02 44` 得 `17,34,51,0,0` | 期望 `17` 出现在 pos0 之外的错位 |
| `RLE4编码解码往返` / `RLE8编码_绝对模式` | RLE 编解码互逆性受上述半字节/行序缺陷影响 | 往返后 `[18,52,0,0]` ≠ `[18,52,86,120]` |
| **RLE 行地址**（新发现，未改实现） | `DIB.pas:1247/1272/1315/1320` 用 `Y * FWidthBytes`（**正数**），而同文件 `LoadRGB`（`DIB.pas:1383`）用 `FTopPBits + Y * FNextLine`（`FNextLine = -FWidthBytes`，`DIB.pas:848`）—— **行序自相矛盾** | 试改为 `FNextLine` 后，`(A,5,A)` 从 Y=1 移到 Y=0，但第二个 run 落到越界行；已**回退保持原文 1:1** 并在代码注释记录 |
| `ConvertBitCount_24到16_pfRGB编码` | `DIB.pas:2273-2295` `SetSize` 把**当前** `PixelFormat` 传给 `NewImage`；24bpp 的 8:8:8 不满足 16bpp 校验 → `NewImage` 抛 `SInvalidDIBPixelFormat` | 实测 `ConvertBitCount(16)` 必抛。**DIB.pas 的 24→16 转换本身不可用** |

### B. 测试期望写错（已定性，**待改测试**）
| 用例 | 实测 | 应为 | 依据 |
|---|---|---|---|
| `RGBQuad_与PaletteEntry互转…` | `quads[0].rgbRed=255` | 删掉 `Assert.Equal(255, quads[255].rgbRed)`（第 387 行自身与 385 行矛盾；`255-i` 在 i=255 时为 0） | `DIB.pas:624-662` |
| `Pixels_4bpp_SetPixel字节下标原文笔误` | 实测 `192`(0xC0) | 期望 `12` 应改为 `0xC0`（`Value=0xC` → `(0xC shl 4)=0xC0`；`P[0]=(P[0]&0x3F)|0xC0` → 0xC0） | `DIB.pas:2024-2026` |
| `Compress_已压缩源走Duplicate` | `4` | 期望 `8` 应改为对 `biSizeImage`/`FSize` 语义的正确断言 | `DIB.pas:936-956` |
| `ConvertBitCount_8到24_查表逐像素` | `0x302010` | 期望 `0x102030` → `0x302010` | `DIB.pas:1219`（内存 `[B,G,R]`）+ `DIB.pas:2004-2005` |
| `ConvertBitCount_8bppHalftone调色板逐项值` | `ct0=(0,0,0) ct1=(0,0,85) ct7=(0,109,255)` | 按实测值重写逐项断言 | `DIB.pas:2319-2330` |
| `SetImage_共享引用计数与字段镜像` | `8` | 期望 `16` 应改（`MkShared(2,2,8)` 的 `FSize=WidthBytes*Height=4*2=8`，不是 `2*2*4`） | `DIB.pas:847-849` |
| `ReadData_8bpp_调色板与biClrUsed` | `5` | 期望 `7` 待按 `biClrUsed` 语义核对 | `DIB.pas:1352-1489` |
| `ReadData_4bpp_biClrUsed限制调色板项数` | `8` | 期望 `0` 应改 | 同上 |
| `ReadData_16bpp_BITFIELDS掩码` | 抛 `EInvalidGraphic("DIB is invalid")` | 测试构造的 BITFIELDS 流长度不足 | `DIB.SharedImage.cs` `ReadBufferAt` |
| `AssignAlphaChannel_源8bpp拷贝为Alpha` | `dst=0x40000000` | 期望 `64` 应改为读 `dst>>24`（或断言 `0x40000000`） | `DIB.pas:1806-1853` |
| `Compress_Decompress_TDIB层` | `False` | 期望 `True` 待核对（24bpp 不产生 RLE） | `DIB.pas:1734-1766` |
| `RetAlphaChannel_无通道返回null` | `HasAlphaChannel()=False`（因测试写 `SetPixel(0,0,0x000000AB)`，A 在高字节 → 实际 A=0） | 测试应写 `0xAB000000` | `DIB.pas:1855-1873` + `DIB.pas:2036` |
| `NewImage_退化尺寸不崩` | 抛 `EOutOfMemory` | 测试集合设置了 `DibSeams.Gdi`，使 `Marshal.AllocHGlobal(0)` 返回 `IntPtr.Zero` 触发第 308 行 | `DIB.pas:918-920`（原文 `GlobalAlloc(0)` 同理） |

---

## 9. 接缝清单

| 接缝 | 位置 | 说明 |
|---|---|---|
| `IDibGdiSeam` | `DIB.cs:200` | `CreateCompatibleDC`/`CreateDIBSection`/`SelectObject`/`DeleteObject`/`DeleteDC`/`SetDIBColorTable`/`CreatePalette`/`GdiFlush`/`GetPaletteEntries` |
| `IDibCanvasSeam` | `DIB.cs:219` | `TCanvas.Draw`/`StretchDIBits`/`StretchBlt`/`CopyMode` |
| `IDibGlobalMemorySeam` | `DIB.cs:238` | `GlobalLock`/`GlobalSize`/`GlobalAlloc`/`GlobalFreePtr` |
| `IDibTailCanvasSeam` | `DIB.Tail.cs:78` | `ColoredLine`/`AntialiasedLine` 的 Pen/Brush/Pixels/MoveTo/LineTo/Ellipse/Rectangle |
| `IDibFiler` | `DIB.Core.cs:79` | `DefineBinaryProperty` |
| `TDibGraphicSource` | `DIB.Core.cs:88` | `TCanvas.Draw` 的非 TDIB 图形源 |
| `DibSeams.Gdi/Canvas/GlobalMemory` | `DIB.cs:191` | 三个静态注入点（测试用假接缝） |

**DxMemo 侧**：`Paint`/`Mouse*`/`KeyDown` 均未实现（属未开工的 6 族），无 `MessageBox`/模态框 → **不需要 `MessageBoxSeam`**（已确认两单元无该调用）。

---

## 10. 需要调度方协调的事项

1. **`TDxScrollControl` 的 `override` vs `virtual`**：已按裁定维持 `public virtual` + 转调 `DxControlOps`；待 DxComponent 家族收敛后由调度方在 `TDxControl` 补两行虚方法再统一改回 `override`。
2. **`TScrollStyle` 归属**：已按裁定引用 `LoadDx.TScrollStyle`；根因去重（迁到 `DxComponent`）由调度方批量执行。
3. **`TDxScrollBox` / `TDxChatMemo` / `TDxListView` / `TDxTreeView` / `TDxTreeNode`**：`GUI/DxComponent` 与 `LoadDx` 两个命名空间已有同名类型（不同命名空间，暂不冲突）。我后续在 `DxComponent` 落地正式归属时若出现 **CS0104 二义性**，将按 §12.8 上报，**不自行改名**。
4. **19 例红必须在并入前转绿** —— 当前分支不可并入 main。

---

## 11. 剩余工作量（诚实估算）

| 项 | 规模 | 状态 |
|---|---|---|
| DIB 剩余 19 例测试 | ~19 处期望/2 处待查 | 已逐条定性，**未改完** |
| **`DIB.Fusion.cs`** | **~3,000 行 Delphi**（16 `Draw*` + 24 `Do*` + 8 滤波器 + 4 辅助类） | **未开工** |
| `DxMemo.cs` `TDxScrollBox` + `DxMemo.Box.cs` `TDxLines`/`TLineColor` | ~360 行 Delphi（1494-1852） | 未开工 |
| `DxMemo.Text.cs` `TStringToken`/`TStringLineEx`/自由函数 | ~120 行 Delphi（238-266, 659-668, 2202-2222, 5820-5877） | 未开工 |
| `DxMemo.Chat.cs` `TDxChatMemo` | ~1,800 行 Delphi（1855-3647，含 300+ 行 Paint） | 未开工 |
| `DxMemo.List.cs` `TViewItem`/`TDxListItem`/`TViewField`/`TDxListView` | ~1,400 行 Delphi（3648-5048） | 未开工 |
| `DxMemo.Tree.cs` `TDxTreeNode`/`TDxTreeView` | ~770 行 Delphi（5050-5819） | 未开工 |
| DxMemo 新增测试 | 每公开方法 ≥3 用例 | 未开工 |

**即：`DxMemo.pas` 仅完成 `TDxScrollControl`（约 13%），`DIB.pas` 完成约 65%（`Fusion` 段空缺），测试未全绿。**
