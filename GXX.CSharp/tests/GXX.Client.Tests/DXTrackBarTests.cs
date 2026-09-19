using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DXTrackBar.pas 1-336 的 1:1 测试。
/// 覆盖构造默认值（Position 不初始化）、SetMin/SetMax 的不同夹紧行为、
/// **SetPosition 不夹紧**（原文缺陷）、值→像素映射、拖动状态机与命中测试、
/// Paint 里进度条与滑块两条 nW 公式的差异。
/// </summary>
public sealed class DXTrackBarTests
{
    private static IDxImageLibrary Lib(params (int w, int h)[] sizes)
    {
        var lib = new TDxImageLibraryStub();
        foreach (var (w, h) in sizes)
            lib.Add(new TDxTextureStub(w, h));
        return lib;
    }

    private static TDXTrackBar NewBar(int width = 100, int height = 20)
        => new() { Left = 0, Top = 0, Width = width, Height = height, Designing = false };

    // =====================================================================
    // 构造（67-78）
    // =====================================================================

    /// <summary>用例 1：构造默认值逐项（69-77）。</summary>
    [Fact]
    public void Constructor_Defaults()
    {
        var c = new TDXTrackBar();
        Assert.True(c.AutoSize);            // 70
        Assert.Equal(100, c.Width);         // 71
        Assert.Equal(20, c.Height);         // 72
        Assert.Equal(0, c.Min);             // 74
        Assert.Equal(10, c.Max);            // 75
        Assert.Equal(0, c.Position);        // **原文构造里不赋值 FPosition** → 默认 0
        Assert.NotNull(c.SliderIndex);
        Assert.False(c.IsHotSlider);        // 29
        Assert.False(c.IsDownSlider);       // 30
    }

    /// <summary>
    /// 用例 2（差异断言）：FSliderIndex 五态索引全 -1（TDxImageIndex 构造默认），
    /// 故未接图库时 Paint 的滑块与刻度都取不到纹理。
    /// </summary>
    [Fact]
    public void Constructor_SliderIndexAllMinusOne()
    {
        var c = new TDXTrackBar();
        Assert.Equal(-1, c.SliderIndex.Up);
        Assert.Equal(-1, c.SliderIndex.Hot);
        Assert.Equal(-1, c.SliderIndex.Down);
        Assert.Equal(-1, c.SliderIndex.Checked);
        Assert.Equal(-1, c.SliderIndex.Disabled);
    }

    // =====================================================================
    // SetMax / SetMin（199-215）
    // =====================================================================

    /// <summary>用例 3：SetMax 降低到低于 Position 时把 Position 拉到新 Max（原文 `FPosition := Max` 即 Math.Max）。</summary>
    [Fact]
    public void SetMax_ClampsPositionDown()
    {
        var c = NewBar();
        c.SetPosition(8);
        Assert.Equal(8, c.Position);

        c.SetMax(5);
        Assert.Equal(5, c.Max);
        Assert.Equal(5, c.Position);
    }

    /// <summary>用例 4：SetMax 提高到不低于 Position 时 Position 不动。</summary>
    [Fact]
    public void SetMax_DoesNotTouchPositionWhenPositionFits()
    {
        var c = NewBar();
        c.SetPosition(3);
        c.SetMax(50);
        Assert.Equal(3, c.Position);
        Assert.Equal(50, c.Max);

        c.SetMax(3);                        // 恰好等于 Position
        Assert.Equal(3, c.Position);
    }

    /// <summary>用例 5：SetMin 提高到高于 Position 时把 Position 抬到新 Min。</summary>
    [Fact]
    public void SetMin_ClampsPositionUp()
    {
        var c = NewBar();
        c.SetPosition(2);
        c.SetMin(7);
        Assert.Equal(7, c.Min);
        Assert.Equal(7, c.Position);
    }

    /// <summary>用例 6：SetMin/SetMax 值不变时整个方法体不执行（199/208 的守卫）。</summary>
    [Fact]
    public void SetMinMax_NoOpWhenValueUnchanged()
    {
        var c = NewBar();
        c.SetPosition(4);
        c.SetMax(10);                       // 同值
        c.SetMin(0);                        // 同值
        Assert.Equal(4, c.Position);

        // 差异断言：即使 Position 越界，同值的 SetMax 也不会夹紧
        c.SetPosition(999);
        c.SetMax(10);
        Assert.Equal(999, c.Position);
    }

    // =====================================================================
    // SetPosition（217-231）
    // =====================================================================

    /// <summary>
    /// 用例 7（原文缺陷，照抄）：SetPosition **不夹紧** ——
    /// 夹紧值 V 只用于「值是否变化」的判断，实际写入的是原始 Value。
    /// </summary>
    [Theory]
    [InlineData(0, 10, 0, 999, 999)]      // 超 Max：V=10≠0 → 写入 999（不夹）
    [InlineData(0, 10, 0, -5, 0)]         // 低于 Min 且 V=0 == Position → **不写入**，保持 0
    [InlineData(0, 10, 5, -5, -5)]        // 低于 Min 且 V=0≠5 → 写入 -5（不夹）
    [InlineData(0, 10, 5, 5, 5)]          // 同值 → 不变
    [InlineData(0, 10, 5, 7, 7)]          // 区间内 → 正常写入
    [InlineData(10, 20, 15, 100, 100)]    // 超 Max → 写入 100（不夹）
    public void SetPosition_DoesNotClampTheStoredValue(int min, int max, int initial, int set, int expected)
    {
        var c = NewBar();
        c.Min = min;
        c.Max = max;
        c.SetPosition(initial);
        c.SetPosition(set);
        Assert.Equal(expected, c.Position);
    }

    /// <summary>用例 8（差异断言）：SetPosition 与 SetMax 的夹紧语义**不一致** —— 前者不夹、后者夹到新 Max。</summary>
    [Fact]
    public void SetPositionAndSetMaxDisagreeOnClamping()
    {
        var a = NewBar();
        a.SetPosition(1000);
        Assert.Equal(1000, a.Position);     // 不夹

        var b = NewBar();
        b.SetPosition(8);
        b.SetMax(3);
        Assert.Equal(3, b.Position);        // 夹到新 Max

        // 差异断言：同一目标值 3，用 SetPosition 写不进去（V == Max 但 Position 不同 → 写 999 的场景）
        var d = NewBar();
        d.SetPosition(8);
        d.SetPosition(999);
        Assert.Equal(999, d.Position);
    }

    // =====================================================================
    // 值 → 像素映射（170/171/188/189/276/279/291）
    // =====================================================================

    /// <summary>
    /// 用例 9（差异断言）：`Step := nW / (FMax - FMin)` 在 Delphi 里是 **Integer/Integer**
    /// （先整数除法再隐式转 Single），故 100/12 → 8（不是 8.333…）。
    /// </summary>
    [Theory]
    [InlineData(100, 10, 10f)]      // 100/10 = 10
    [InlineData(100, 12, 8f)]       // 100/12 = 8（截断，不是 8.3333）
    [InlineData(99, 10, 9f)]        // 99/10 = 9
    [InlineData(5, 10, 0f)]         // 5/10 = 0（整数除法！）
    [InlineData(0, 10, 0f)]         // 0/10 = 0
    public void ComputeStep_UsesIntegerDivision(int nW, int span, float expected)
    {
        Assert.Equal(expected, TDXTrackBar.ComputeStep(nW, span));
    }

    /// <summary>用例 10：ComputeStep 的 span 为 0 时返回 0（原文该分支不会走到，因为外层有 `FMax-FMin &gt; 0` 守卫）。</summary>
    [Fact]
    public void ComputeStep_ZeroSpanReturnsZero()
    {
        Assert.Equal(0f, TDXTrackBar.ComputeStep(100, 0));
    }

    /// <summary>用例 11：nX = Round(Step * (FPosition - FMin))，走银行家舍入。</summary>
    [Theory]
    [InlineData(10f, 0, 0)]
    [InlineData(10f, 5, 50)]
    [InlineData(10f, 10, 100)]
    [InlineData(2.5f, 1, 2)]        // 2.5 → 2（偶数）
    [InlineData(3.5f, 1, 4)]        // 3.5 → 4（偶数）
    [InlineData(0.5f, 1, 0)]        // 0.5 → 0（偶数）
    public void ComputePixelPosition_UsesBankersRounding(float step, int offset, int expected)
    {
        Assert.Equal(expected, TDXTrackBar.ComputePixelPosition(step, offset));
    }

    /// <summary>用例 12：nPos = Round((X - vt.Left) / Step) + FMin 的除法部分。</summary>
    [Theory]
    [InlineData(0, 10f, 0)]
    [InlineData(50, 10f, 5)]
    [InlineData(55, 10f, 6)]        // 5.5 → 6（偶数）
    [InlineData(45, 10f, 4)]        // 4.5 → 4（偶数）
    [InlineData(-30, 10f, -3)]
    public void ComputeDragPosition_RoundsQuotient(int xOffset, float step, int expected)
    {
        Assert.Equal(expected, TDXTrackBar.ComputeDragPosition(xOffset, step));
    }

    /// <summary>用例 13：Step 为 0 时返回 int.MaxValue（让外层夹紧到 FMax，避免浮点除零）。</summary>
    [Fact]
    public void ComputeDragPosition_ZeroStepReturnsMaxValue()
    {
        Assert.Equal(int.MaxValue, TDXTrackBar.ComputeDragPosition(10, 0f));
    }

    // =====================================================================
    // PaintTo（92-197）
    // =====================================================================

    /// <summary>用例 14：无图库/无索引 → 除 Designing 边框外什么都不画。</summary>
    [Fact]
    public void Paint_WithoutImagesDrawsNothing()
    {
        var c = NewBar();
        var p = new TDxRecordingPainter();

        var r = c.PaintTo(p);

        Assert.Empty(p.Ops);
        Assert.False(r.DrewUpTick);
        Assert.False(r.DrewDownTick);
        Assert.False(r.DrewSlider);
    }

    /// <summary>用例 15：VisibleRect 退化 → 连 Designing 边框都不画（106-107）。</summary>
    [Fact]
    public void Paint_DegenerateVisibleRectDrawsNothing()
    {
        var c = NewBar(0, 0);
        c.Designing = true;
        var p = new TDxRecordingPainter();

        var r = c.PaintTo(p);

        Assert.Empty(p.Ops);
        Assert.False(r.DrewDesignFrame);
    }

    /// <summary>用例 16：Designing=True → 先画边框（112-114）。</summary>
    [Fact]
    public void Paint_DesigningDrawsFrameFirst()
    {
        var c = NewBar();
        c.Designing = true;
        c.BorderColor.Up.Color = 0x606080;
        var p = new TDxRecordingPainter();

        var r = c.PaintTo(p);

        Assert.True(r.DrewDesignFrame);
        Assert.Single(p.Ops);
        Assert.Contains("606080", p.Ops[0]);
    }

    /// <summary>
    /// 用例 17（差异断言）：进度条（ImageIndex.Down）的 nW = **整个宽度**（161-166 的扣滑块宽被注释掉），
    /// 而滑块（SliderIndex）的 nW = 宽度 **减滑块宽**。二者在同一点上的像素位置不同。
    /// </summary>
    [Fact]
    public void Paint_TickAndSliderUseDifferentWidthFormulas()
    {
        // 刻度图 100×10（Up/Down 都用同一张）；滑块图 10×10。控件 100×20。
        var c = NewBar(100, 20);
        c.ImageIndex.Image = Lib((100, 10));
        c.ImageIndex.Up = 0;
        c.ImageIndex.Down = 0;
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Min = 0;
        c.Max = 10;
        c.Position = 5;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        // 进度条：nW = 100 → Step = 10 → nX = 50 → vbRect.Right = 0 + 50 = 50
        Assert.True(r.DrewDownTick);
        Assert.Equal(50, r.TickClipRight);

        // 滑块：nW = 100 - 10 = 90 → Step = 9 → nX = 45；nY = (20-10) div 2 = 5
        Assert.True(r.DrewSlider);
        Assert.Equal(45, r.SliderX);
        Assert.Equal(5, r.SliderY);

        // 差异断言：两条路径的 x 不同
        Assert.NotEqual(r.TickClipRight, r.SliderX);
    }

    /// <summary>用例 18：Max == Min 时进度条与滑块都不画（169/187 的 `FMax - FMin &gt; 0` 守卫）。</summary>
    [Fact]
    public void Paint_NoDrawWhenRangeIsZero()
    {
        var c = NewBar(100, 20);
        c.ImageIndex.Image = Lib((100, 10));
        c.ImageIndex.Down = 0;
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Min = 5;
        c.Max = 5;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.False(r.DrewDownTick);
        Assert.False(r.DrewSlider);
    }

    /// <summary>
    /// 用例 19（差异断言）：Enabled=True 取 Images[]、Enabled=False 取 Grays[]；
    /// 两者是**两张不同的图**（TDxImageLibraryStub.Add 会同时填 Images 与 Grays，
    /// 故这里只用 AddGray 造出「有灰度、无彩色」的图库）。
    /// </summary>
    [Fact]
    public void Paint_DisabledUsesGrayImages()
    {
        // 只有灰度图：Enabled=True 时取 Images[0] == nil → 不画
        var grayOnly = new TDxImageLibraryStub().AddGray(new TDxTextureStub(10, 10));
        var c = NewBar(100, 20);
        c.SliderIndex.Image = grayOnly;
        c.SliderIndex.Up = 0;
        c.Min = 0; c.Max = 10; c.Position = 5;

        var p = new TDxRecordingPainter();
        Assert.False(c.PaintTo(p).DrewSlider);        // Enabled=True → Images[0] 为 nil

        // 同一图库下禁用 → 走 Grays[0]，滑块画出来
        c.Enabled = false;
        var p2 = new TDxRecordingPainter();
        Assert.True(c.PaintTo(p2).DrewSlider);

        // 启用状态下用 Lib（Images 有图）→ 也能画
        var colored = NewBar(100, 20);
        colored.SliderIndex.Image = Lib((10, 10));
        colored.SliderIndex.Up = 0;
        colored.Min = 0; colored.Max = 10; colored.Position = 5;
        var p3 = new TDxRecordingPainter();
        Assert.True(colored.PaintTo(p3).DrewSlider);
    }

    /// <summary>
    /// 用例 20（差异断言）：OnPaint 被挂上时走**覆写分支** —— 原文 121-122
    /// `if Assigned(OnPaint) then OnPaint(Self) else begin ... end;`，
    /// 即整个默认绘制（刻度+滑块）都不执行。
    /// </summary>
    [Fact]
    public void Paint_OnPaintOverrideSkipsDefaultDrawing()
    {
        var c = NewBar(100, 20);
        c.ImageIndex.Image = Lib((100, 10));
        c.ImageIndex.Up = 0;
        c.ImageIndex.Down = 0;
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;

        int called = 0;
        c.OnPaint = _ => called++;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.Equal(1, called);
        Assert.True(r.UsedOnPaintOverride);
        Assert.False(r.DrewUpTick);
        Assert.False(r.DrewDownTick);
        Assert.False(r.DrewSlider);
        Assert.Empty(p.Ops);
    }

    // =====================================================================
    // 拖动状态机（233-312 / 314-334）
    // =====================================================================

    /// <summary>用例 21：MouseDown 把 Down 态同步为当前的 Hot 态（233-237）。</summary>
    [Fact]
    public void MouseDown_CopiesHotStateToDownState()
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Min = 0;
        c.Max = 10;
        c.Position = 5;

        // 先让滑块变 Hot：Position=5 → 滑块位于 x∈[45,55]，y∈[5,15]
        c.MouseMoveTo(TDxShiftState.None, 50, 10);
        Assert.True(c.IsHotSlider);

        c.MouseDown(TDxMouseButton.mbLeft, TDxShiftState.ssLeft, 50, 10);
        Assert.True(c.IsDownSlider);
    }

    /// <summary>用例 22：不在滑块上按下 → Down 态保持 False，CanMove=True。</summary>
    [Fact]
    public void MouseDown_OutsideSliderLeavesDownStateFalse()
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Position = 5;

        c.MouseMoveTo(TDxShiftState.None, 90, 10);    // 远离滑块
        Assert.False(c.IsHotSlider);

        c.MouseDown(TDxMouseButton.mbLeft, TDxShiftState.ssLeft, 90, 10);
        Assert.False(c.IsDownSlider);
        Assert.True(c.CanMove());
    }

    /// <summary>
    /// 用例 23：拖动分支 —— nPos = Round((X - vt.Left) / Step) + FMin 并夹紧到 [Min, Max]，
    /// 随后回调 OnChanggingPosition。
    /// </summary>
    [Theory]
    [InlineData(0, 0)]        // 最左
    [InlineData(45, 5)]       // 中段：Step = (100-10)/10 = 9 → 45/9 = 5
    [InlineData(90, 10)]      // 最右
    [InlineData(500, 10)]     // 超出右界 → 夹到 Max
    [InlineData(-100, 0)]     // 超出左界 → 夹到 Min
    public void MouseMove_DraggingClampsPositionToRange(int x, int expectedPosition)
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Min = 0;
        c.Max = 10;
        c.Position = 5;

        // 进入按下态（先让 Hot=True）
        c.MouseMoveTo(TDxShiftState.None, 45, 10);
        c.MouseDown(TDxMouseButton.mbLeft, TDxShiftState.ssLeft, 45, 10);
        Assert.True(c.IsDownSlider);

        int changingCalls = 0;
        c.OnChanggingPosition = _ => changingCalls++;

        var changed = c.MouseMoveTo(TDxShiftState.ssLeft, x, 10);

        Assert.True(changed);                              // 走到拖动分支
        Assert.Equal(expectedPosition, c.Position);
        Assert.Equal(1, changingCalls);
    }

    /// <summary>用例 24：未按下时的 MouseMove 走悬停分支并**不**改 Position（原文 290-305 的 Exit）。</summary>
    [Fact]
    public void MouseMove_NotDraggingUpdatesHotOnly()
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Min = 0;
        c.Max = 10;
        c.Position = 5;

        int changingCalls = 0;
        c.OnChanggingPosition = _ => changingCalls++;

        var changed = c.MouseMoveTo(TDxShiftState.None, 90, 10);

        Assert.False(changed);
        Assert.Equal(5, c.Position);                       // 位置未被拖动改写
        Assert.Equal(0, changingCalls);
        Assert.False(c.IsHotSlider);                       // 90 不在滑块矩形内
    }

    /// <summary>用例 25（差异断言）：滑块矩形的 Y 用 `(vRect 高 - 滑块高) div 2`，命中测试用闭区间。</summary>
    [Theory]
    [InlineData(45, 5, true)]      // 左上角
    [InlineData(55, 15, true)]     // 右下角（闭区间命中）
    [InlineData(44, 10, false)]    // 左外侧 1 像素
    [InlineData(56, 10, false)]    // 右外侧 1 像素
    [InlineData(50, 4, false)]     // 上外侧 1 像素
    [InlineData(50, 16, false)]    // 下外侧 1 像素
    public void MouseMove_HotHitTestUsesClosedSliderRect(int x, int y, bool expectedHot)
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Min = 0;
        c.Max = 10;
        c.Position = 5;

        c.MouseMoveTo(TDxShiftState.None, x, y);
        Assert.Equal(expectedHot, c.IsHotSlider);
    }

    /// <summary>用例 26：滑块图为 nil 时末尾把 Hot 置 False（311）。</summary>
    [Fact]
    public void MouseMove_WithoutSliderImageClearsHot()
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Up = -1;                 // 无图索引
        c.MouseMoveTo(TDxShiftState.None, 50, 10);
        Assert.False(c.IsHotSlider);
    }

    /// <summary>用例 27：MouseUp 清 Down 态并回调 OnChangedPosition（314-321）。</summary>
    [Fact]
    public void MouseUp_ClearsDownStateAndFiresChanged()
    {
        var c = NewBar(100, 20);
        c.SliderIndex.Image = Lib((10, 10));
        c.SliderIndex.Up = 0;
        c.Position = 5;

        c.MouseMoveTo(TDxShiftState.None, 50, 10);
        c.MouseDown(TDxMouseButton.mbLeft, TDxShiftState.ssLeft, 50, 10);
        Assert.True(c.IsDownSlider);

        int changedCalls = 0;
        c.OnChangedPosition = _ => changedCalls++;

        c.MouseUp(TDxMouseButton.mbLeft, TDxShiftState.None, 50, 10);

        Assert.False(c.IsDownSlider);
        Assert.Equal(1, changedCalls);
        Assert.True(c.CanMove());
    }

    /// <summary>
    /// 用例 28（差异断言）：Enabled=False 时拖动分支取的是 Grays 图；
    /// 若只有彩色图（无灰度图），整段 `if TextureSlider &lt;&gt; nil` 守卫挡住，
    /// 位置**不会**被拖动改写（即使已处于按下态）。
    /// </summary>
    [Fact]
    public void MouseMove_DisabledWithoutGrayImageDoesNotDrag()
    {
        var grayOnly = new TDxImageLibraryStub().AddGray(new TDxTextureStub(10, 10));
        var c = NewBar(100, 20);
        c.SliderIndex.Image = new TDxImageLibraryStub();   // 空图库：Images/Grays 全 nil
        c.SliderIndex.Up = 0;
        c.Enabled = false;
        c.Min = 0; c.Max = 10; c.Position = 3;
        c.SetDownSliderProbe(true);              // 直接置按下态（测试接缝）

        var changed = c.MouseMoveTo(TDxShiftState.ssLeft, 90, 10);

        Assert.False(changed);
        Assert.Equal(3, c.Position);             // 未被拖动改写
        Assert.False(c.IsHotSlider);

        // 对照：有灰度图时禁用状态下仍能拖动
        var c2 = NewBar(100, 20);
        c2.SliderIndex.Image = grayOnly;
        c2.SliderIndex.Up = 0;
        c2.Enabled = false;
        c2.Min = 0; c2.Max = 10; c2.Position = 3;
        c2.SetDownSliderProbe(true);

        Assert.True(c2.MouseMoveTo(TDxShiftState.ssLeft, 90, 10));
        Assert.Equal(10, c2.Position);           // nW = 100-10 = 90 → Step 9 → 90/9 = 10
    }

    /// <summary>用例 29：SetOnGetImage 同时挂到 SliderIndex（86-90）—— 基类与 SliderIndex 各自立即回调一次。</summary>
    [Fact]
    public void SetOnGetImage_WiresSliderIndex()
    {
        var c = NewBar();
        var seen = new List<TImageType>();
        c.SetOnGetImage((idx, t) => seen.Add(t));

        // base.SetOnGetImage → 控件 ImageIndex 立即回调一次；
        // FSliderIndex.OnGetImage := Value → TDxImageIndex.SetOnGetImage 再立即回调一次
        Assert.Equal(2, seen.Count);
        Assert.All(seen, t => Assert.Equal(TImageType.Prguse_wil, t));

        seen.Clear();
        c.SliderIndex.SetImageType(TImageType.UI_wil);
        Assert.Single(seen);
        Assert.Equal(TImageType.UI_wil, seen[0]);
    }

    /// <summary>用例 30：CanMove = not FIsDownSlider（323-326）。</summary>
    [Fact]
    public void CanMove_IsInverseOfDownState()
    {
        var c = NewBar();
        Assert.True(c.CanMove());
        c.SetDownSliderProbe(true);
        Assert.False(c.CanMove());
    }
}
