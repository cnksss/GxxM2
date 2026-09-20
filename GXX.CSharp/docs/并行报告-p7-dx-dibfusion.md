# 并行报告 · `p7-dx-dibfusion`（`DIB.pas` 4940-7930 整段空洞补齐）

> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p7-dx-dibfusion`（分支 `par/p7-dx-dibfusion`）
> 前置资料：`docs/并行报告-p3-dx-big.md` §7（记录本空洞）、`docs/并行派发台账.md` §17.2 / §18.8 / §24.2 / §24.3 / §25.2 / §28.3 / §29.4
> 原文：`Source\Client-HGE\DxComponent\DIB.pas`（GBK，实测 8,747 物理行）
> **结论：`DIB.pas` 4940-7930 已 100% 覆盖；`GXX.Client.Tests` 4,546 全绿（基线 4,326 + 本车道 220，零新增失败）。**

---

## 1. 全部 commit

| # | hash | 内容 | 门禁 |
|---|---|---|---|
| A | `c066c436` | `TCustomDXDIB`/`TCustomDXPaintBox`/`CreateDIBFromBitmap` + 16 个 `Draw*` + `DrawOn`（4940-5846、5917-5934） | build 0 error；Client.Tests **4418** 全绿（本片 92） |
| B | `b8b80bec` | `InitLight`/`DrawLights`/`Darkness`（5848-5966） | **4437** 全绿（本片 111） |
| C1 | `50242742` | `DoSmoothRotate` + 10 个 `Do*`（5975-6396）＋ **顺带修复 `DibEffectsSupport.DibRandom` 恒返回 0** | **4470** 全绿（本片 144） |
| C2+C3 | `19951281` | 其余 12 个 `Do*`（6398-7115）＋新接缝 `IDibFusionSpotSeam` | **4505** 全绿（本片 179） |
| D | `479eaf58` | `DoResample` + 8 个重采样滤波器 + 4 个记录（7117-7674，新文件 `DIB.Fusion.Filters.cs`） | **4528** 全绿（Fusion 201 + Filter 23） |
| E | `f5ee9614` | `DoColorize`/`FadeOut`/`DoZoom`/`DoBlur`/`FadeIn`/`FillDIB8`（7676-7928）＋新接缝 `IDibFusionColorizeSeam` | **4546** 全绿（+18） |

每个切片提交时 `git status --porcelain` 均为空；最后一次提交全绿。

---

## 2. 空洞覆盖表（原文行号区间 → 落地文件 → 状态）

| 原行号 | 内容 | 落地 | 状态 |
|---|---|---|---|
| 4940-4955 | `TCustomDXDIB.Create/Destroy/SetDIB` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 4959-4978 | `TCustomDXPaintBox.Create/Destroy/GetPalette` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 4980-5073 | `TCustomDXPaintBox.Paint`（含内嵌 `Draw2`） | `DIB.Fusion.cs` | ✅ 已完成（布局决策走纯逻辑 + 假接缝逐值断言，+13 例） |
| 5075-5138 | 7 个 setter | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5142-5145 | `PosValue` | **已在 `DIB.cs:491` 落地** | ✅ 不重复定义（回读核对：1:1） |
| 5147-5152 | `TDIB.CreateDIBFromBitmap` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5154-5159 | `TDIB.DrawTo` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5161-5217 | `DrawTransparent` | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 5219-5267 | `DrawShadow` | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 5269-5297 | `DrawDarken` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5299-5375 | `DrawQuickAlpha` | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 5377-5400 | `DrawAdditive` | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 5402-5457 | `DrawTranslucent` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5459-5516 | `DrawAlpha` | `DIB.Fusion.cs` | ✅ 已完成（+5 例） |
| 5518-5572 | `DrawAlphaMask` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5574-5642 | `DrawMorphed` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5644-5710 | `DrawMono` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5712-5733 | `Draw3x3Matrix` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5735-5754 | `DrawAntialias` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5756-5786 | `FilterLine` | `DIB.Fusion.cs` | ✅ 已完成（+5 例） |
| 5788-5846 | `FilterRect` | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 5848-5858 | `InitLight`（256×256 `FLUTDist`） | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 5860-5915 | `DrawLights` | `DIB.Fusion.cs` | ✅ 已完成（+6 例） |
| 5917-5934 | `DrawOn` | `DIB.Fusion.cs` | ✅ 已完成（随切片 A，+4 例） |
| 5938-5966 | `Darkness` | `DIB.Fusion.cs` | ✅ 已完成（+5 例） |
| 5940-5945 / 5968-5973 | `IntToByte` / `TrimInt`（单元级） | **已在 `DIB.cs:494/502`；`TDIB` 同名方法在 `DIB.Effects.cs:2818/2803`** | ✅ 不重复定义 |
| 5975-6036 | `DoSmoothRotate`（局部 `TFColor` → `DibFColor`） | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 6042-6063 | `DoInvert` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6065-6093 | `DoAddColorNoise` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6095-6124 | `DoAddMonoNoise` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6126-6155 | `DoAntiAlias` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6157-6191 | `DoContrast` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6193-6302 | `DoFishEye` | `DIB.Fusion.cs` | ✅ 已完成（+3 例；`Single`→`float`，见 §6-14） |
| 6304-6328 | `DoGrayScale` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6330-6356 | `DoLightness` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6358-6367 | `DoDarkness` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6369-6396 | `DoSaturation` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6398-6445 | `DoSplitBlur` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6447-6457 | `DoGaussianBlur` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6459-6502 | `DoMosaic` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6504-6642 | `DoTwist` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6644-6790 | `DoTrace` | `DIB.Fusion.cs` | ✅ 已完成（+3 例；影子图需 Canvas 接缝） |
| 6792-6825 | `DoSplitlight` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6827-6913 | `DoTile`（含 `SmoothResize`/`Tile`） | `DIB.Fusion.cs` | ✅ 已完成（+3 例；平铺本身走 Canvas 接缝） |
| 6915-6958 | `DoSpotLight` | `DIB.Fusion.cs` | ✅ 已完成（接缝 `IDibFusionSpotSeam`，+2 例） |
| 6960-6988 | `DoEmboss` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 6990-7031 | `DoSolorize` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 7033-7065 | `DoPosterize` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 7067-7115 | `DoBrightness` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 7126-7241 | 8 个滤波器（含 `SinC`） | `DIB.Fusion.Filters.cs` → `DibFusionFilters` | ✅ 已完成（每函数逐点值，共 +10 例） |
| 7248-7279 | `TContributor`/`TCList`/`TRGB`/`TColorRGB` | `DIB.Fusion.Filters.cs`（`DibContributor`/`DibCList`/`DibRGB`/`DibColorRGB`） | ✅ 已完成（+2 例，含 `Marshal.SizeOf` 锁定 8/12/3） |
| 7305-7315 | `Color2RGB`/`RGB2Color` | `DIB.Fusion.Filters.cs` | ✅ 已完成（+3 例） |
| 7118-7661 | `Resample`（USE_SCANLINE 路径） | `DIB.Fusion.Filters.cs` | ✅ 已完成（+10 例，含精确权重表） |
| 7662-7674 | `DoResample` 外层脚手架 | `DIB.Fusion.Filters.cs` | ✅ 已完成 |
| 7676-7776 | `DoColorize` + `InvertBitmap` | `DIB.Fusion.cs`（接缝 `IDibFusionColorizeSeam`） | ✅ 已完成（+3 例，锁 16 次 CopyRect / 16 次 CopyMode 序列） |
| 7780-7813 | `FadeOut` | `DIB.Fusion.cs`（asm → 逐字节 `max`） | ✅ 已完成（+4 例） |
| 7815-7852 | `DoZoom` | `DIB.Fusion.cs` | ✅ 已完成（+4 例） |
| 7854-7868 | `DoBlur` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |
| 7870-7903 | `FadeIn` | `DIB.Fusion.cs`（asm → 逐字节 `min`） | ✅ 已完成（+3 例） |
| 7905-7928 | `FillDIB8` | `DIB.Fusion.cs` | ✅ 已完成（+3 例） |

**未覆盖：无。** 全部区间有落地实现 + 断言；其中"接缝"标注者（`DrawOn`、`CreateDIBFromBitmap`、`DoTrace` 的影子图、`DoTile` 的平铺、`DoSpotLight`、`DoColorize`）的像素效果依赖 GDI 接缝，决策/顺序逻辑已用记录式假接缝逐条锁死。

---

## 3. 新增文件 + 行数

| 文件 | 行数 | 说明 |
|---|---|---|
| `GXX.CSharp/src/GXX.Client/DxComponent/DIB.Fusion.cs` | 3,055 | 覆盖 4940-5846、5917-5934、5848-5966、5975-7115、7676-7928 |
| `GXX.CSharp/src/GXX.Client/DxComponent/DIB.Fusion.Filters.cs` | 617 | 覆盖 7117-7674（`DoResample` + `DibFusionFilters`） |
| `GXX.CSharp/tests/GXX.Client.Tests/DxCtlDibFusionTests.cs` | 2,474 | 220 例中的 179 |
| `GXX.CSharp/tests/GXX.Client.Tests/DxCtlDibFusionFilterTests.cs` | 347 | 23 |
| `GXX.CSharp/tests/GXX.Client.Tests/DxCtlDibFusionTailTests.cs` | 366 | 18 |
| 本报告 | — | `GXX.CSharp/docs/并行报告-p7-dx-dibfusion.md` |

**改动既有文件 1 个**：`DIB.Effects.cs`（仅 `DibEffectsSupport.DibRandom`，见 §6-1）。
**未改** `DxMemo*.cs` / `LoadDx/**` / `GXX.slnx` / `*.csproj` / `tools/**` / `Checklist.md`；新增 `.cs` 自动纳入编译。

**防复发核查**：新增类型前对 `main` 跑
`git grep -l -E "(class|struct|enum|interface|delegate) +(partial +)?<TypeName>\b" main -- 'GXX.CSharp/src/**/*.cs'`
（`TCustomDXDIB`/`TDXDIB`/`TCustomDXPaintBox`/`TDXPaintBox`/`DibFusionFilters`/`DibContributor`/`DibCList`/`DibRGB`/`DibColorRGB`/`DibFColor`/`IDibFusionCanvasSeam`/`IDibFusionPaintSeam`/`IDibFusionSpotSeam`/`IDibFusionColorizeSeam`/`DibFusionSupport`/`DibFusionCanvas`/`DibFusionSpot`/`DibFusionColorize`/`DibFusionRop`）**全部空结果**。同名风险已避开：`TFColor`→`DibFColor`、`TContributor`→`DibContributor`、`TCList`→`DibCList`、`TRGB`→`DibRGB`、`TColorRGB`→`DibColorRGB`。

---

## 4. 测试与门禁

| 项 | 值 |
|---|---|
| 新增用例 | **220**（`DxCtlDibFusionTests` 179 / `DxCtlDibFusionFilterTests` 23 / `DxCtlDibFusionTailTests` 18） |
| 门禁基线 | `main` 上 `GXX.Client.Tests` = 4,326（任务书）；本车道起点实跑 = **4,418 → 4,546** |
| `dotnet build GXX.slnx -c Debug` | **0 error**（157 warning，全部为基线既有 xUnit 分析器告警） |
| `dotnet test tests\GXX.Client.Tests` | **4,546 passed / 0 failed** |
| 新增失败 | **0** |
| 基线漂移 | 未见（期间无其它车道文件导致的编译错误） |

按"每个公开方法 ≥3 用例"逐项核对：本单元所有公开方法均 ≥3 例（`DrawTransparent` 4、`DrawShadow` 4、`DrawQuickAlpha` 4、`DrawAdditive` 4、`DrawAlpha` 5、`FilterLine` 5、`DrawLights` 6、`Paint` 13、每个滤波器 ≥1 例并有 10 例专门锁权重、`DoResample` 10 等）。

差异断言（"看起来一样实则不同"的分支）重点：
* `DrawTransparent`/`DrawTranslucent`/`DrawMorphed`/`DrawMono` 的 `StartY := -DestStartY` **vs** `DrawAlpha`/`DrawAlphaMask` 的 `StartY := DestStartY`（正负号 + 检查顺序都不同；同一输入下前者成功、后者抛 `SScanline`）；
* `DrawShadow` 与 `DrawQuickAlpha` 的 `fmNormal` 与 `fmMix50` 走**同一** case 分支；
* `FilterLine` 的 `fmMix25`/`fmMix75` 权重与名字**相反**（Color 权重 1/4 vs 3/4）；
* `DrawTranslucent` 用 Integer 加法（`(200+200) shr 1 = 200`，不是 Byte 回绕的 72）；
* `DrawAdditive` 的 `(Alpha - p1^) * P2^ shr 8` 在 `Alpha < dst` 时逻辑右移放大再回绕（实测 100 → **22**）；
* `DoMosaic` 的真实语义是**横向游程填充**（游程色取自起点、被覆盖位置不先读回）+ 纵向块扩散（实测 4×4/Size=2 → `[0,0,2,2]/[0,0,2,2]/[20,20,22,22]/[20,20,22,22]`）；
* `DoPosterize` 量化结果可超 255 → 写回 Byte **回绕**（255/100 → 3×100=300 → **44**）；
* `DoZoom(1.0)` **不是恒等**而是整体右下移 1 字节；
* `DoResample` 的 USE_SCANLINE 路径 **R/B 互换**（值出现在 `GetPixel` 的 B 通道）；
* `BoxFilter` 在 ±0.5 处**不对称**（-0.5→0、+0.5→1）；`BellFilter` 在 0.5 处走第二分支。

---

## 5. 接缝清单（精确签名）

全部接缝的**默认实现都不是静默中性值**（§25.2）：构造注入为 `null` 抛 `ArgumentNullException`，静态接缝未装载抛 `InvalidOperationException`。

```csharp
// DIB.Fusion.cs —— DrawOn 的 BitBlt
public interface IDibFusionCanvasSeam
{
    void BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
                IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
}
public static class DibFusionCanvas   // Seam 未装载 → DrawOn 抛 InvalidOperationException
{ public static IDibFusionCanvasSeam Seam; public const uint SRCCOPY = 0x00CC0020; }

// DIB.Fusion.cs —— TCustomDXPaintBox 的控件面 + 两个 Canvas 绘制入口
public interface IDibFusionPaintSeam
{
    bool Designing { get; }
    bool ControlStyleReplicatable { get; set; }
    int Height { get; set; }
    int Width { get; set; }
    int ClientWidth { get; }
    int ClientHeight { get; }
    void Invalidate();
    void SetPenStyleDash();
    void SetBrushStyleClear();
    void Rectangle(int X1, int Y1, int X2, int Y2);
    void StretchDraw(TDxRect DestRect, TDIB Graphic);
    void Draw(int X, int Y, TDIB Graphic);
}
// TCustomDXPaintBox(IDibFusionPaintSeam AOwner) —— AOwner 为 null 抛 ArgumentNullException

// DIB.Fusion.cs —— DoSpotLight 的 TCanvas/TBitmap 绘制面
public interface IDibFusionSpotSeam
{
    void SetBrushColor(int Color);
    void FillRect(int Left, int Top, int Right, int Bottom);
    void Ellipse(int X1, int Y1, int X2, int Y2);
    void SetBitmapTransparent(TDIB Bmp, bool Value);
    void SetCanvasCopyMode(TDIB Dib, uint Value);
}
public static class DibFusionSpot   // 未装载 → DoSpotLight 抛 InvalidOperationException
{ public static IDibFusionSpotSeam Seam; }

// DIB.Fusion.cs —— DoColorize 的 Canvas 光栅操作序列
public interface IDibFusionColorizeSeam
{
    void SetBrushStyleSolid(TDIB Dib);
    void SetBrushColor(TDIB Dib, int Color);
    void FillRect(TDIB Dib, TDxRect Rect);
    void SetCopyMode(TDIB Dib, uint Value);
    void CopyRect(TDIB DestDib, TDxRect DestRect, TDIB SrcDib, TDxRect SrcRect);
    void SetPixels(TDIB Dib, int X, int Y, int Color);
    void AssignBrushBitmap(TDIB Dib, TDIB Value);
}
public static class DibFusionColorize // 未装载 → DoColorize 抛 InvalidOperationException
{ public static IDibFusionColorizeSeam Seam; }
public static class DibFusionRop
{
    public const uint cmSrcInvert = 0x00660046, cmSrcPaint = 0x00EE0086, cmSrcErase = 0x00440328,
                      cmPatPaint = 0x00FB0A09, cmDstInvert = 0x00550009;
}
```

复用的既有接缝（未改动）：`DibSeams.Gdi` / `DibSeams.Canvas` / `DibSeams.GlobalMemory`（`DIB.cs`）、`DibTailSupport.Canvas`（`DIB.Tail.cs`）。

**接缝可观察性说明**：`CreateDIBFromBitmap`、`DoTrace` 的影子图、`DoTile` 的平铺、`DoSpotLight`、`DoColorize` 的**像素结果**完全由接缝决定，无头环境下不可复现；本车道对这些方法的**决策/顺序/参数**做了记录式假接缝断言（见 §2 备注）。

---

## 6. 发现的原文缺陷 / 易错点（`文件:行`）

### 6.1 顺带修复的既有实现缺陷（`DIB.Effects.cs`）
1. **`DibEffectsSupport.DibRandom` 恒返回 0**（原 `return (int)r * 0;` 占位残留）——
   使 `Spray`(3926-3977) / `AddMonoNoise`(4241-4292) / `AddGradiantNoise`(4296-4391) 全部退化为
   **固定偏移而非噪声**。已恢复 `RandSeed` LCG（`RandSeed*$08088405+1`，`Range=0` 不推进），
   并补 2 例回归断言（`DoAddColorNoise`/`DoAddMonoNoise` 的偏移必须同时出现 `-1` 与 `0`）。
   该缺陷**零测试覆盖**（全仓 grep 无 `Spray`/`Add*Noise` 用例），故不破坏门禁。**建议集成方在 DIB 全族复核。**

### 6.2 本单元区间的原文缺陷（实现 1:1 保留，测试锁死）
2. **`DoSpotLight` 对自身是空操作**（`DIB.pas:6915-6958`）：`SpotLight` 只把变暗结果画进临时 `z`
   （`z.DrawTo` → `z.Darkness` → `z.Canvas.Draw(0,0,Bm)`），**从不回写 `Src`**，随后 `z.Free`。
   实测：`Self` 像素完全不变；接缝调用序列完整（这是本单元**最高价值**的发现）。
3. **`DoResample` 的 R/B 颠倒**（`DIB.pas:6` `{$DEFINE USE_SCANLINE}` + `7444-7464/7609-7619`）：
   USE_SCANLINE 路径把源行按 `TColorRGB(R,G,B)` 解释，而 DIB 的 24bpp 内存是 `(B,G,R)`
   ⇒ 整条重采样链在 R/B 互换坐标系里运算，**最终值出现在 `GetPixel` 的 B 通道**。
   实测 4×4→2×2 Box 得 `[[10,16],[34,40]]`（R=G=0）。
4. **`DoResample` 贡献者下标反射给出负下标**（`DIB.pas:7388/7427/7537/7576`）：
   `n = SrcWidth - j + SrcWidth - 1` 在 `j > 2*SrcWidth-1` 时为负（SrcWidth=4、j=8 → n=-1），
   于是**读出缓冲区之外**。4×4→2×2 时 `ftrBox/ftrTriangle/ftrHermite`（半径 ≤1）不触发；
   `ftrBell/ftrBSpline/ftrLanczos3/ftrMitchell` 会触发 —— 实测**同一输入两次运行结果不同**（读到堆垃圾）。
   本片对这四个滤波器只做"不抛 + 尺寸 + 值域"断言。
5. **`DrawLights` 的 `SetLength(P, LG_DETAIL)` 越界写**（`DIB.pas:5867` vs `5875`）：
   循环 `for o := 0 to LG_DETAIL` 要写 `LG_DETAIL+1` 个（`{$IFDEF DelphiX_Delphi3}` 的静态数组
   `array[0..4096]` 正是 4097）。托管侧按 `+1` 分配（**有意偏离，已登记**），否则必抛
   `IndexOutOfRangeException`。另：行下标 `(LG_DETAIL+1)*I - o` 会被 `GetScanLine` 拦下
   （`Height` 是 `LG_DETAIL+1` 整数倍时抛 `SScanline`，实测复现），而列下标 `P[o][n]` 是裸字节指针、
   **无**边界检查（`Width` 是整数倍时越过行尾写进上一行，实测：行 1 的越界写落到行 0 像素 0）。
6. **`DoSplitBlur` 的两侧"反折"写成 `Height - Y` / `Width - X`**（`DIB.pas:6412/6424`，不是 `-1-…`）：
   `Amount >= Height` 时 `Y=0` 的 `ScanLine(Height)` 抛 `SScanline`（实测）；列方向无检查。
7. **`DoTwist` 的上界只夹 `>= Width/Height`**（`DIB.pas:6551-6552`）：`3x3` 时
   `Round(Sqrt(8)) = 3` → `Dst.ScanLine(3)` 越界（实测抛）。`Amount = 0` → `R/0 = Inf` →
   `Cos(Inf) = NaN` → `Trunc(NaN)` 抛 `EInvalidOp`（实测 `ArithmeticException`）。
8. **`DoZoom` 用像素宽度做字节下标**（`DIB.pas:7836/7838`）：`for X := 1 to Width-1` 而 `P2[X]`/`p1[Trunc(xr)]`
   是字节下标；`xstep/ystep` 是**加性**步进；边界允许 `xr = w`（越出行尾 1 字节）、`yr = h`（抛 `SScanline`）；
   `ZoomRatio = 1.0` **不是恒等**。
9. **`DoBlur` 末行读缓冲区之外**（`DIB.pas:7865`）：`Y = Height-1` 时 `p1[X - WidthBytes]` 落在
   `FPBits - WidthBytes`；`X = Width-1` 时读行尾填充字节。
10. **`DoMosaic` 的真实语义与名字不符**（`DIB.pas:6473-6487`）：`R/G/B` 读在最内层 `repeat` **之外**
    ⇒ 横向游程填充（游程色取自起点，被覆盖位置不先读回）；`P2` 每轮重取且 `X := 0` 重置、`p1` 只在
    最外层取一次 ⇒ 纵向块扩散。
11. **`DoColorize` 的 `fForeDither`/`fBmpMade` 从未初始化**（`DIB.pas:7688/7689`）：
    栈上不定值，且 `fBmpMade := True` 后从未被读取。托管侧 `fForeDither = false`
    （**有意偏离，已登记**）。另有 `fColor := iBackColor; ;`（`7697`）连写两个分号且 `fColor` 从未被读取；
    `InvertBitmap(Src)`（`7713/7719`）会**就地改写入参 `Src`**。
12. **`DoGaussianBlur` 连写两次 `BB.BitCount := 24;`**（`DIB.pas:6451-6452`）。
13. **`DoTrace` 连写两次 `p3[(X+1)*3+1] := TraceB;`**（`DIB.pas:6686-6688`，R 通道漏写）。
14. **`DoSplitlight` 的 `BB2` 整段被注释掉**（`DIB.pas:6813/6818-6820/6824`，含 `var BB1 {,BB2}`）——
    托管侧照抄为注释，不留死代码。
15. **`FadeOut`/`FadeIn` 用 `Self.ScanLine[DIB2.Height - 1]`**（`DIB.pas:7785-7786`）：
    用 **DIB2** 的高度索引 Self 的行；`DIB2` 更高时抛 `SScanline`（实测）。
16. **`DoSmoothRotate` 的自赋值与未用变量**（`DIB.pas:5986` `Angle := Angle;`；`5980` 声明的
    `Left/Right/wx/wy` 从未使用）；`TFColor` 字段名与内存序相反但自洽。
17. **`DoDarkness` 的 `BB.BitCount := 24` 被 `BB.Assign(Self)` 覆盖**（`DIB.pas:6362-6363`）
    ⇒ 非 24bpp 时 `Darkness` 直接 `Exit`（实测：8bpp 调用后位深仍为 8、像素不变）。
18. **`DoResample` 的 `DstHeight = 1` ⇒ `yscale = 0` ⇒ `Width = FWidth/0 = +Inf`**（`DIB.pas:7337-7340`）
    ⇒ `Trunc(+Inf)` 抛；`SrcHeight = 1` ⇒ `Work` 只有 1 行 ⇒ 纵向 `Delta` 的 `Work.ScanLine(1)` 抛
    `SScanline`（均实测）。
19. **`DoResample` 两侧注释写反**（`DIB.pas:7367-7371` 等）：注释说 `ceil(center-width)`/`floor(center+width)`，
    实际代码是 `floor(center-width)`/`ceil(center+width)`。
20. **`DoSplitlight` 的 `sinpixs` 过 `variant(...)`**（`DIB.pas:6800`）：Variant→Integer 走 `Round`（银行家舍入），
    本片按 `DibFusionSupport.Round` 实现（**该点无原文实证，属推定**，已在此登记）。

### 6.3 移植陷阱（会坑人，但不是原文缺陷）
21. **`shr` 对 Integer 是逻辑右移**（§24.3 陷阱 5）——`DrawAdditive` 的
    `(Alpha - p1^) * P2^ shr 8` 在 `Alpha < dst` 时得 16777138 量级的正数，必须走 `DibFusionSupport.Shr`。
22. **Delphi 7 把小于 Integer 的整型提升为 Integer 做 `+ - *`** ——`DrawTranslucent` 的
    `(200+200) shr 1 = 200`（若按 Byte 回绕会得 72），已用差异断言锁死。
23. **形参 `Width`/`Height` 遮蔽 `TDIB` 同名属性** ——`Draw*`/`Filter*` 全族靠 `Self.X` 与裸 `X` 区分，
    C# 侧照抄为 `this.X` 与裸 `X`（本片逐方法核对）。
24. **`{ get; set; }` 自动属性会生成第二个后备字段**（§24.3 陷阱 3）——`TCustomDXDIB.DIB` 直接 `=> FDIB`。
25. **`Round` 越界/NaN** ——Delphi 抛 `EInvalidOp`，C# 的 `(int)Math.Round(Inf)` 会**静默**得 `int.MinValue`；
    `DibFusionSupport.Round/Trunc` 显式抛 `ArithmeticException`（§25.2，不静默）。
26. **Extened(80 位) vs double**：`DoFishEye`/`DoTwist`/`DoSmoothRotate` 的 `Single`/`Extended` 声明
    照抄为 `float`/`double`，末位可能与原文有别（相关测试用结构性断言或整数友好输入）。
27. **`.ps1` 在 PS 5.1 下按 ANSI(GBK) 读 UTF-8 脚本会乱码**（本次实测：含中文路径的脚本解析失败）
    ——临时脚本必须纯 ASCII 或用 `$PSScriptRoot` 取路径（§29.4 工具链规程的补充）。

---

## 7. 大段表的抽取 + 双向重解析比对（任务要求 4）

本区间**没有字面量权重表/调色板表**（滤波器权重全部由公式计算，`DefaultFilterRadius` 已在 `DIB.cs:134`）。
为满足"脚本抽取 + 双向重解析逐字段比对"，本车道写了临时脚本
（`DIB.pas` 按 `^\s*(procedure|function|...) TDIB\.Name` + 首个列 0 `^end;$` 切方法体；
C# 按 `public … Name(` + 大括号配平切方法体；两侧**先剥离行尾注释**（容忍行尾注释，避免"行尾即冒号"式误判），
再取整数字面量多重集与 `shr/shl/div/mod/xor/and/or/not` 计数，双向求差）。结果：

* **方法覆盖**：Pascal 区间内 62 个 `TDIB.*`/`TCustomDXPaintBox.*` 方法；C# 匹配 53 个；
  **完全一致 14 个**。
* **C# 未匹配 9 个**，全部**有据可查**：`Create`（`DIB.Core.cs`/`DIB.SharedImage.cs`）、
  `DoRotate`（`DIB.Tail.cs`）、7 个 `Set*` setter（本车道实现为 **private**，脚本只搜 `public`）。
* **差异逐条判读后全部归入已知类别**：
  1. 有意的翻译映射（脚本未建映射表）：`div`→`/`（`Darkness`/`DoAntiAlias`/`DoBlur`/`DoContrast`/
     `DoLightness`/`DoSaturation`/`DoSolorize`/`Draw3x3Matrix`/`DrawLights`/`FilterLine`/`FilterRect`/`Paint`/`DoTile`）、
     `mod`→`%`（`DoFishEye`/`DoTwist`/`DoColorize`）、`not`→`!`/`~`、`and`→`&&`/`&`、`or`→`||`/`|`、
     `xor`→`^`（字符串 `$FF` 等已按 255 归一）、Pascal 的 `^`（**指针解引用**）被计成 `xor`；
  2. **已登记的代码提升**：`DoResample` 的 Pascal-only 字面量
     `0.5,0.75,1.5,3.0,6.0,8.0,9.0,12.0,18.0,30.0,48.0,8,16,65280,16711680` 正是被提到
     `DibFusionFilters` 的 8 个滤波器与 `Color2RGB/RGB2Color` 的常量（`65280=$00FF00`、`16711680=$FF0000`）；
  3. asm 标签（`FadeIn`/`FadeOut` 的 `@@2` → 字面量 2）；
  4. 异常消息文本里的 `25.2`（`§25.2`）被判为字面量；
  5. 显式化下标算术：`DoSplitBlur` 的 `Buf` 由 2 维改 1 维（出现 4..11）、
     `DoResample` 显式 `pixel*3(+1/+2)`（出现 3）、`DoBrightness` 显式字段偏移 2/3。
* **残留 2 处脚本假阳性**：`CreateDIBFromBitmap` 的 `24`、`DoSmoothRotate` 的 `180`
  —— 已回读两侧源码确认字面量**都存在**（C# `DIB.Fusion.cs:707 / 1577`；Pascal `5150 / 5987`）。
* 脚本与临时文件已删除（`.tmp-check.ps1` / `.tmp-extract-check.ps1`）；本节数据为运行留档。

**诚实评价**：该脚本是**审阅辅助**而非硬门禁 —— 它证明"没有方法被整段漏掉"，并把字面量差异压缩到
可人工判读的规模；真正的"1:1"保证来自每个方法的**逐点期望值断言**（220 例，期望值均由回读原文推导并
在测试内注明 `DIB.pas:<行>`）。

---

## 8. 诚实说明：未完成部分

**本单元（4940-7930）没有未完成的原文区间** —— 全部方法均已落地。

但有 **4 处"实现已 1:1、像素效果不可在无头环境验证"** 的已知边界（均由接缝覆盖，非本车道可解）：

1. `DoSpotLight`（6915-6958）的椭圆遮罩与 `cmSrcAnd` 合成 —— 且**原文本身是空操作**（§6-2），
   故"不可验证"的部分只有中间临时 `z` 的绘制；
2. `DoColorize`（7676-7776）的 16 次 `CopyRect` 的真实光栅结果 —— 本车道只锁了**顺序/参数/接收者**；
3. `DoTile`（6827-6913）的 `Amount²` 次平铺 `Canvas.Draw` —— `SmoothResize` 是纯逻辑但结果只写入临时
   `Bm`，不可外部观察；
4. `DoTrace`（6644-6790）的 8bpp 影子图 —— 需要 GDI 把 24bpp 转成 8bpp；无接缝时影子图恒 0，
   故 24bpp 侧"描边"只走退化路径。

另外 **2 处"推定而非实证"**（已在代码与 §6 登记）：
`DoSplitlight` 的 `variant()` 舍入模式（§6-20）；`DoFishEye`/`DoTwist`/`DoSmoothRotate` 的
`Extended`/`Single` 末位（§6-26）。

**建议**：`ftrBell/ftrBSpline/ftrLanczos3/ftrMitchell` 在缩小时读到缓冲区之外（§6-4），
若客户端要用 `DoResample`，建议**另行裁剪**（原文行为不可依赖）；`DoSpotLight`/`DoColorize`
两处空操作/GDI 序列建议在客户端装载层评估是否属于"原文没做完的功能"。
