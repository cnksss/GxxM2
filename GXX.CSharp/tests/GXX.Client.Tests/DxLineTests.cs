using System;
using System.Linq;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DxLine.pas 1-134 的 1:1 测试。
/// 覆盖构造默认值、SetStyle 不换宽高（原文注释掉的分支）、
/// 四种线型的几何公式（水平/垂直/圆/三角）、lsNone 不画、InRange。
/// </summary>
public sealed class DxLineTests
{
    private static TDxRect R(int l, int t, int r, int b) => TDxRect.Rect(l, t, r, b);

    /// <summary>
    /// 建立一个 TDxLine。注意：**必须显式关掉 Designing** —— TDxControl 构造里
    /// FDesigning := True（1850），而 DxLine.Paint 会在 Designing 时先画一次边框，
    /// 会污染线型几何的绘制序列断言。
    /// </summary>
    private static TDxLine NewLine(int width = 200, int height = 12)
        => new() { Left = 0, Top = 0, Width = width, Height = height, Designing = false };

    // =====================================================================
    // 构造（37-51）
    // =====================================================================

    /// <summary>用例 1：构造默认值逐项（39-50）。</summary>
    [Fact]
    public void Constructor_Defaults()
    {
        var c = new TDxLine();
        Assert.False(c.AutoSize);            // 40
        Assert.False(c.Floating);            // 41
        Assert.False(c.DrawBorder);          // 42
        Assert.Equal(200, c.Width);          // 43
        Assert.Equal(12, c.Height);          // 44
        Assert.Equal(TLineStyle.lsNone, c.Style);   // 45
        Assert.NotNull(c.LineColor);
    }

    /// <summary>用例 2：FLineColor 四态全部 clGray（47-50）。</summary>
    [Fact]
    public void Constructor_LineColorAllGray()
    {
        var c = new TDxLine();
        Assert.Equal(TDxColor.clGray, c.LineColor.Up.Color);
        Assert.Equal(TDxColor.clGray, c.LineColor.Hot.Color);
        Assert.Equal(TDxColor.clGray, c.LineColor.Down.Color);
        Assert.Equal(TDxColor.clGray, c.LineColor.Disabled.Color);
    }

    /// <summary>
    /// 用例 3（差异断言）：TDxLine 覆写了基类的 AutoSize=True → False、
    /// Floating/DrawBorder=False、Width/Height=200/12（基类分别是 True / 0 / 0）。
    /// </summary>
    [Fact]
    public void Constructor_OverridesBaseDefaults()
    {
        var c = new TDxLine();
        var probe = new TDxControlProbe();
        Assert.True(probe.AutoSize);
        Assert.False(c.AutoSize);
        Assert.Equal(200, c.Width);
        Assert.Equal(0, probe.Width);
    }

    // =====================================================================
    // SetStyle（59-69）
    // =====================================================================

    /// <summary>
    /// 用例 4（差异断言）：SetStyle 里「设计期水平/垂直互换宽高」的代码被整段注释掉，
    /// 故切换 lsHorizontal → lsVertical **不会**交换 Width/Height（原文如此）。
    /// </summary>
    [Fact]
    public void SetStyle_DoesNotSwapWidthAndHeight()
    {
        var c = NewLine(200, 12);
        c.Style = TLineStyle.lsHorizontal;
        Assert.Equal(200, c.Width);
        Assert.Equal(12, c.Height);

        c.Style = TLineStyle.lsVertical;
        Assert.Equal(200, c.Width);          // 若实现被注释掉的分支，这里会变成 12
        Assert.Equal(12, c.Height);
        Assert.Equal(TLineStyle.lsVertical, c.Style);
    }

    /// <summary>用例 5：SetStyle 值变才赋值（59-69）。</summary>
    [Fact]
    public void SetStyle_AssignsOnlyOnChange()
    {
        var c = NewLine();
        Assert.Equal(TLineStyle.lsNone, c.Style);

        c.Style = TLineStyle.lsNone;
        Assert.Equal(TLineStyle.lsNone, c.Style);

        c.Style = TLineStyle.lsCircle;
        Assert.Equal(TLineStyle.lsCircle, c.Style);
    }

    // =====================================================================
    // PaintTo（88-132）
    // =====================================================================

    /// <summary>用例 6：lsNone 不画任何线（case 无 else）。</summary>
    [Fact]
    public void Paint_LsNoneDrawsNothing()
    {
        var c = NewLine();
        c.Style = TLineStyle.lsNone;
        var p = new TDxRecordingPainter();

        Assert.Equal(0, c.PaintTo(p));
        Assert.Empty(p.Ops);
    }

    /// <summary>
    /// 用例 7（差异断言）：lsHorizontal 的 y 用 `vt.Top + (Height-1) div 2`，**用 Height 字段**而非 vtRect 高度；
    /// 水平线从 vt.Left 拉到 vt.Right。
    /// </summary>
    [Fact]
    public void Paint_HorizontalGeometry()
    {
        var c = NewLine(200, 12);
        c.Style = TLineStyle.lsHorizontal;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Single(p.Ops);
        // (12-1) div 2 = 5
        Assert.Equal("Line((0,5),(200,5),808080)", p.Ops[0]);
    }

    /// <summary>用例 8：Height 为偶数/奇数时 (Height-1) div 2 的取值（向零截断的整数除法）。</summary>
    [Theory]
    [InlineData(12, 5)]
    [InlineData(13, 6)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 1)]
    public void Paint_HorizontalMidlineFormula(int height, int expectedY)
    {
        var c = NewLine(200, height);
        c.Style = TLineStyle.lsHorizontal;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Equal($"Line((0,{expectedY}),(200,{expectedY}),808080)", p.Ops[0]);
    }

    /// <summary>用例 9：lsVertical 的 x 用 `vt.Left + (Width-1) div 2`，从 vt.Top 拉到 vt.Bottom。</summary>
    [Fact]
    public void Paint_VerticalGeometry()
    {
        var c = NewLine(200, 12);
        c.Style = TLineStyle.lsVertical;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        // (200-1) div 2 = 99
        Assert.Equal("Line((99,0),(99,12),808080)", p.Ops[0]);
    }

    /// <summary>用例 10：lsCircle 的圆心与半径公式（`Width div 2` / `Height div 2` / `Max(...)`）。</summary>
    [Fact]
    public void Paint_CircleGeometry()
    {
        var c = NewLine(200, 12);
        c.Style = TLineStyle.lsCircle;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        // 圆心 (0 + 200/2, 0 + 12/2) = (100, 6)；半径 Max(100, 6) = 100
        Assert.Equal("Circle(100,6,100,808080)", p.Ops[0]);
    }

    /// <summary>用例 11：lsCircle 的半径取 Max(Width div 2, Height div 2) —— 高大于宽时取高的一半。</summary>
    [Fact]
    public void Paint_CircleRadiusUsesMaxOfBothHalves()
    {
        var c = NewLine(10, 40);
        c.Style = TLineStyle.lsCircle;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Equal("Circle(5,20,20,808080)", p.Ops[0]);
    }

    /// <summary>用例 12：lsTriangle 画三条边，顶点为 (左侧中, vt.Top) / (vt.Left, vt.Bottom) / (vt.Right, vt.Bottom)。</summary>
    [Fact]
    public void Paint_TriangleDrawsThreeEdges()
    {
        var c = NewLine(200, 12);
        c.Style = TLineStyle.lsTriangle;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Equal(3, p.Ops.Count);
        Assert.Equal("Line((99,0),(0,12),808080)", p.Ops[0]);
        Assert.Equal("Line((99,0),(200,12),808080)", p.Ops[1]);
        Assert.Equal("Line((0,12),(200,12),808080)", p.Ops[2]);
    }

    /// <summary>用例 13：Designing=True 时先画一次边框（101-104），线本体不受影响。</summary>
    [Fact]
    public void Paint_DesigningDrawsBorderFirst()
    {
        var c = NewLine(200, 12);
        c.Style = TLineStyle.lsHorizontal;
        c.Designing = true;
        c.BorderColor.Up.Color = 0x606080;
        var p = new TDxRecordingPainter();
        var ops = c.PaintTo(p);

        Assert.Equal(2, ops);
        Assert.Equal(2, p.Ops.Count);
        Assert.StartsWith("FrameRect(", p.Ops[0]);
        Assert.Contains("606080", p.Ops[0]);
        Assert.StartsWith("Line(", p.Ops[1]);
    }

    /// <summary>用例 14：VisibleRect 退化（高或宽为 0）时立即 Exit，连 Designing 边框都不画（96）。</summary>
    [Fact]
    public void Paint_DegenerateVisibleRectExitsBeforeDesignFrame()
    {
        var c = NewLine(0, 0);
        c.Style = TLineStyle.lsHorizontal;
        c.Designing = true;
        var p = new TDxRecordingPainter();

        Assert.Equal(0, c.PaintTo(p));
        Assert.Empty(p.Ops);
    }

    /// <summary>
    /// 用例 15（差异断言）：线颜色按 Enabled/MouseDowned/MouseMoveed 选，Disabled 独立 ——
    /// Down/Hot 与 Up 三态颜色若要区分，必须各自赋值。
    /// </summary>
    [Fact]
    public void Paint_LineColorStateSelection()
    {
        string ColorOf(Action<TDxLine> apply)
        {
            var c = NewLine(200, 12);
            c.Style = TLineStyle.lsHorizontal;
            c.LineColor.Up.Color = 0x010101;
            c.LineColor.Hot.Color = 0x020202;
            c.LineColor.Down.Color = 0x030303;
            c.LineColor.Disabled.Color = 0x040404;
            apply(c);
            var p = new TDxRecordingPainter();
            c.PaintTo(p);
            var m = System.Text.RegularExpressions.Regex.Match(p.Ops[0], @",([0-9A-F]{6})\)$");
            return m.Groups[1].Value;
        }

        Assert.Equal("010101", ColorOf(c => { }));
        Assert.Equal("020202", ColorOf(c => c.MouseMoveed = true));
        Assert.Equal("030303", ColorOf(c => c.MouseDowned = true));
        Assert.Equal("040404", ColorOf(c => c.Enabled = false));
        // 差异断言：同时按下与禁用时，Disabled 优先（原文 106-116 的 else 结构）
        Assert.Equal("040404", ColorOf(c => { c.Enabled = false; c.MouseDowned = true; }));
    }

    /// <summary>用例 16：线宽高用的是 Width/Height 字段，**与 vtRect 的宽高无关**（Left/Top 平移不影响长度）。</summary>
    [Fact]
    public void Paint_UsesWidthHeightFieldsNotVirtualRectSize()
    {
        var c = new TDxLine { Left = 30, Top = 40, Width = 200, Height = 12, Designing = false };
        c.Style = TLineStyle.lsHorizontal;
        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        // vtRect = (30,40,230,52) → y = 40 + 5 = 45；x 从 30 到 230
        Assert.Equal("Line((30,45),(230,45),808080)", p.Ops[0]);
    }

    // =====================================================================
    // InRange（71-86）
    // =====================================================================

    /// <summary>用例 17：InRange 命中 VisibleRect（闭区间）并回调 OnInRealArea（相对虚拟区坐标）。</summary>
    [Fact]
    public void InRange_HitsVisibleRectAndReportsRelativeCoords()
    {
        var c = new TDxLine { Left = 10, Top = 20, Width = 200, Height = 12, Designing = false };
        c.VirtualRectOverride = () => R(10, 20, 210, 32);

        Assert.True(c.InRange(10, 20));
        Assert.True(c.InRange(210, 32));      // 右下角（闭区间）
        Assert.False(c.InRange(211, 32));
        Assert.False(c.InRange(10, 33));

        int gx = -1, gy = -1;
        c.OnInRealArea = (s, x, y, box) => { gx = x; gy = y; };
        Assert.True(c.InRange(15, 25));
        Assert.Equal(5, gx);
        Assert.Equal(5, gy);
    }

    /// <summary>用例 18：OnInRealArea 可把命中改判为 False；不在范围内则不回调。</summary>
    [Fact]
    public void InRange_OnInRealAreaCanVeto()
    {
        var c = new TDxLine { Left = 0, Top = 0, Width = 200, Height = 12, Designing = false };
        c.VirtualRectOverride = () => R(0, 0, 200, 12);

        bool called = false;
        c.OnInRealArea = (s, x, y, box) => { called = true; box.Value = false; };

        Assert.False(c.InRange(5, 5));
        Assert.True(called);

        called = false;
        Assert.False(c.InRange(500, 500));
        Assert.False(called);
    }
}
