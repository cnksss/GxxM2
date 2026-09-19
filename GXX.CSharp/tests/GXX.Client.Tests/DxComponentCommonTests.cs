using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DxComponent 地基接缝层 1:1 测试。
/// 覆盖：
///   * DxComponents.pas 934-991 的 TRect 六个工具函数（含 PointInRect 的四边**闭区间**语义）
///   * DxControls.pas 34-150 的 TDxFont / TDxCaptionColor / TDxBorderColor / TDxImageIndex
///   * DxControls.pas 1799-1896 的 TDxControl 构造默认值
///   * DxControls.pas 2219-2251 的 VisibleRect / VirtualRect
///   * DxControls.pas 3095-3145 ReallyPaintRect / 3199-3205 ReallyRect / 3208-3223 CanDraw
///   * DxControls.pas 3752-3850 DrawCaption（含 Bold 四次偏移描边序列）
///   * DxControls.pas 2375-2411 GetPosition / SetPosition
///   * HGEFontEx.pas 448-502 / 693-769 TextWidth/TextHeight/GetImageInfos 的公式镜像
/// </summary>
public sealed class DxComponentCommonTests
{
    private static TDxRect R(int l, int t, int r, int b) => TDxRect.Rect(l, t, r, b);

    private static TDxLabel NewLabel(int left, int top, int width, int height)
    {
        var c = new TDxLabel { Left = left, Top = top, Width = width, Height = height };
        return c;
    }

    // =====================================================================
    // DxComponents.pas 900-991：TRect 工具
    // =====================================================================

    /// <summary>用例 1：ShortRect = 交集（Max 左边/Top，Min 右边/Bottom）。</summary>
    [Fact]
    public void ShortRect_IsIntersection()
    {
        Assert.Equal(R(5, 6, 7, 8), DxRectUtil.ShortRect(R(0, 0, 10, 10), R(5, 6, 7, 8)));
        Assert.Equal(R(5, 6, 7, 8), DxRectUtil.ShortRect(R(5, 6, 7, 8), R(0, 0, 10, 10)));
        // 无交集时产生「反向」矩形（Right < Left），原文不判空 —— 调用方须自行比较
        var none = DxRectUtil.ShortRect(R(0, 0, 1, 1), R(5, 5, 6, 6));
        Assert.True(none.Right < none.Left);
    }

    /// <summary>用例 2：LongRect = 并集。</summary>
    [Fact]
    public void LongRect_IsUnion()
    {
        Assert.Equal(R(0, 0, 10, 10), DxRectUtil.LongRect(R(0, 0, 10, 10), R(5, 6, 7, 8)));
        Assert.Equal(R(0, 0, 10, 10), DxRectUtil.LongRect(R(5, 6, 7, 8), R(0, 0, 10, 10)));
        Assert.Equal(R(-3, -3, 2, 2), DxRectUtil.LongRect(R(-3, -3, 1, 1), R(0, 0, 2, 2)));
    }

    /// <summary>用例 3：MoveRect 整体平移（含负偏移）。</summary>
    [Fact]
    public void MoveRect_ShiftsBothCorners()
    {
        Assert.Equal(R(5, 7, 15, 17), DxRectUtil.MoveRect(R(0, 0, 10, 10), new TDxPoint(5, 7)));
        Assert.Equal(R(-5, -7, 5, 3), DxRectUtil.MoveRect(R(0, 0, 10, 10), new TDxPoint(-5, -7)));
        Assert.Equal(R(0, 0, 10, 10), DxRectUtil.MoveRect(R(0, 0, 10, 10), new TDxPoint(0, 0)));
    }

    /// <summary>
    /// 用例 4（差异断言）：PointInRect 的右边界与下边界**也算命中**（&lt;= 而非 &lt;）——
    /// 这与 GDI 的 PtInRect（右/下开区间）**相反**，是本工程里最容易踩的一处差异。
    /// </summary>
    [Fact]
    public void PointInRect_IncludesRightAndBottomEdges()
    {
        var rect = R(0, 0, 10, 10);
        Assert.True(DxRectUtil.PointInRect(new TDxPoint(0, 0), rect));
        Assert.True(DxRectUtil.PointInRect(new TDxPoint(10, 0), rect));    // 右边界命中
        Assert.True(DxRectUtil.PointInRect(new TDxPoint(0, 10), rect));    // 下边界命中
        Assert.True(DxRectUtil.PointInRect(new TDxPoint(10, 10), rect));   // 右下角命中
        Assert.False(DxRectUtil.PointInRect(new TDxPoint(11, 0), rect));
        Assert.False(DxRectUtil.PointInRect(new TDxPoint(0, 11), rect));
        Assert.False(DxRectUtil.PointInRect(new TDxPoint(-1, 0), rect));
        Assert.False(DxRectUtil.PointInRect(new TDxPoint(0, -1), rect));

        // 与 System.Drawing.Rectangle.Contains 的差异：后者不含右/下边界
        var gdi = new System.Drawing.Rectangle(0, 0, 10, 10);
        Assert.False(gdi.Contains(10, 0));
        Assert.True(DxRectUtil.PointInRect(new TDxPoint(10, 0), rect));
    }

    /// <summary>用例 5：ShrinkRect 四边同时内缩。</summary>
    [Fact]
    public void ShrinkRect_ShrinksAllSides()
    {
        Assert.Equal(R(1, 2, 9, 8), DxRectUtil.ShrinkRect(R(0, 0, 10, 10), 1, 2));
        Assert.Equal(R(-1, -1, 11, 11), DxRectUtil.ShrinkRect(R(0, 0, 10, 10), -1, -1));
        Assert.Equal(R(0, 0, 10, 10), DxRectUtil.ShrinkRect(R(0, 0, 10, 10), 0, 0));
    }

    /// <summary>用例 6：RectInRect（闭区间包含）与 OverlapRect（严格重叠）的边界差异。</summary>
    [Fact]
    public void RectInRectAndOverlapRect_BoundarySemantics()
    {
        Assert.True(DxRectUtil.RectInRect(R(0, 0, 10, 10), R(0, 0, 10, 10)));   // 自身算包含
        Assert.True(DxRectUtil.RectInRect(R(1, 1, 9, 9), R(0, 0, 10, 10)));
        Assert.False(DxRectUtil.RectInRect(R(0, 0, 11, 10), R(0, 0, 10, 10)));

        Assert.False(DxRectUtil.OverlapRect(R(0, 0, 10, 10), R(10, 0, 20, 10)));  // 仅贴边不算重叠
        Assert.True(DxRectUtil.OverlapRect(R(0, 0, 11, 10), R(10, 0, 20, 10)));
        Assert.False(DxRectUtil.OverlapRect(R(0, 0, 10, 10), R(0, 0, 10, 10)) == false); // 自身算重叠
    }

    /// <summary>用例 7：TDxRect.Bounds 与 Width/Height 的换算（Right/Bottom 为左/上+宽/高）。</summary>
    [Fact]
    public void DxRect_BoundsAndSize()
    {
        var r = TDxRect.Bounds(3, 4, 10, 20);
        Assert.Equal(R(3, 4, 13, 24), r);
        Assert.Equal(10, r.Width);
        Assert.Equal(20, r.Height);
        Assert.Equal(R(0, 0, 0, 0), TDxRect.Empty);
    }

    // =====================================================================
    // DxControls.pas 34-150：字体 / 颜色 / 图片索引
    // =====================================================================

    /// <summary>用例 8：TDxFont 构造默认值（772-786）逐项。</summary>
    [Fact]
    public void TDxFont_Defaults()
    {
        var f = new TDxFont();
        Assert.Equal(TDxColor.clWhite, f.Color);
        Assert.Equal(TDxColor.clBlack, f.BColor);
        Assert.Equal("", f.Name);
        Assert.Equal(9, f.Size);
        Assert.False(f.Bold);
        Assert.Empty(f.Style);
    }

    /// <summary>用例 9：TDxFont 的 setter 全部走「值变才 Changed」（786-790）。</summary>
    [Fact]
    public void TDxFont_ChangedOnlyOnRealValueChange()
    {
        var f = new TDxFont();
        int changed = 0;
        f.OnChange = _ => changed++;

        f.Color = TDxColor.clWhite;      // 与默认同值 → 不触发
        Assert.Equal(0, changed);

        f.Color = TDxColor.clRed;
        Assert.Equal(1, changed);

        f.Size = 9;                      // 同值
        Assert.Equal(1, changed);

        f.Size = 12;
        f.Bold = true;
        Assert.Equal(3, changed);
    }

    /// <summary>用例 10：TDxFont.Assign 逐属性拷贝（含 Style 集合与 Alignment 接缝）。</summary>
    [Fact]
    public void TDxFont_AssignCopiesEveryProperty()
    {
        var src = new TDxFont { Name = "SimSun", Size = 14, Bold = true, Color = 0x123456, BColor = 0x654321 };
        src.StyleSet = new[] { "fsBold", "fsItalic" };
        src.Alignment = TDxAlignment.taCenter;

        var dst = new TDxFont();
        dst.Assign(src);

        Assert.True(dst.ValueEquals(src));
        Assert.Equal("SimSun", dst.Name);
        Assert.Equal(14, dst.Size);
        Assert.Equal(new[] { "fsBold", "fsItalic" }, dst.Style);
    }

    /// <summary>用例 11：TDxCaptionColor 构造（855-882）—— 五态全 Bold，四白一 BtnFace。</summary>
    [Fact]
    public void TDxCaptionColor_Defaults()
    {
        var c = new TDxCaptionColor();
        Assert.True(c.Up.Bold);
        Assert.True(c.Hot.Bold);
        Assert.True(c.Down.Bold);
        Assert.True(c.Checked.Bold);
        Assert.True(c.Disabled.Bold);

        Assert.Equal(TDxColor.clWhite, c.Up.Color);
        Assert.Equal(TDxColor.clWhite, c.Hot.Color);
        Assert.Equal(TDxColor.clWhite, c.Down.Color);
        Assert.Equal(TDxColor.clWhite, c.Checked.Color);
        Assert.Equal(TDxColor.clBtnFace, c.Disabled.Color);
    }

    /// <summary>用例 12：子字体变更冒泡到 TDxCaptionColor.OnChange（FontChange）。</summary>
    [Fact]
    public void TDxCaptionColor_BubblesFontChanges()
    {
        var c = new TDxCaptionColor();
        int n = 0;
        c.OnChange = _ => n++;

        c.Up.Color = TDxColor.clRed;
        Assert.Equal(1, n);

        c.Up.Color = TDxColor.clRed;   // 同值不触发
        Assert.Equal(1, n);

        c.Hot.Size = 20;
        Assert.Equal(2, n);
    }

    /// <summary>
    /// 用例 13（差异断言）：TDxBorderColor 覆盖了 TDxCaptionColor 的默认色与粗体 ——
    /// Up=$00608490 / Hot=Down=$005894B8 且四态 **Bold=False**（父类默认是 True）。
    /// </summary>
    [Fact]
    public void TDxBorderColor_OverridesParentDefaults()
    {
        var b = new TDxBorderColor();
        Assert.Equal(0x00608490, b.Up.Color);
        Assert.Equal(0x005894B8, b.Hot.Color);
        Assert.Equal(0x005894B8, b.Down.Color);
        Assert.Equal(TDxColor.clBtnFace, b.Disabled.Color);

        Assert.False(b.Up.Bold);
        Assert.False(b.Hot.Bold);
        Assert.False(b.Down.Bold);
        Assert.False(b.Disabled.Bold);

        // 差异断言：父类同名字段默认 True
        Assert.True(new TDxCaptionColor().Up.Bold);
    }

    /// <summary>用例 14：TDxImageIndex 构造默认（954-970）—— 五个索引全 -1，ImageType=Prguse_wil。</summary>
    [Fact]
    public void TDxImageIndex_Defaults()
    {
        var i = new TDxImageIndex();
        Assert.Equal(-1, i.Up);
        Assert.Equal(-1, i.Hot);
        Assert.Equal(-1, i.Down);
        Assert.Equal(-1, i.Checked);
        Assert.Equal(-1, i.Disabled);
        Assert.Equal(0, i.OffsetX);
        Assert.Equal(0, i.OffsetY);
        Assert.Equal(TImageType.Prguse_wil, i.ImageType);
        Assert.Null(i.Image);
    }

    /// <summary>用例 15：TDxImageIndex.SetOnGetImage 赋值后**立即回调一次**（159-165 同形）。</summary>
    [Fact]
    public void TDxImageIndex_SetOnGetImage_InvokesImmediately()
    {
        var i = new TDxImageIndex();
        var calls = new List<TImageType>();
        i.SetOnGetImage((s, t) => calls.Add(t));

        Assert.Single(calls);
        Assert.Equal(TImageType.Prguse_wil, calls[0]);

        i.SetImageType(TImageType.UI_wil);
        Assert.Equal(2, calls.Count);
        Assert.Equal(TImageType.UI_wil, calls[1]);
    }

    /// <summary>用例 16：BeginUpdate/EndUpdate 期间不触发 Changed（原文 FUpdateValue 计数）。</summary>
    [Fact]
    public void TDxImageIndex_BeginEndUpdate_SuppressesChanges()
    {
        var i = new TDxImageIndex();
        int n = 0;
        i.OnChange = _ => n++;

        i.BeginUpdate();
        i.Up = 3;
        i.Hot = 4;
        Assert.Equal(0, n);          // 更新期内被吞

        i.EndUpdate();
        Assert.Equal(1, n);          // 归零时补一次

        i.EndUpdate();               // 已在 0：EndUpdate 里 _updateValue 不再 >0，但归零仍触发 Changed
        Assert.Equal(2, n);

        i.EndUpdate();
        Assert.Equal(3, n);          // 原文如此：FUpdateValue 已是 0 时 EndUpdate 依然 Changed
    }

    // =====================================================================
    // DxControls.pas 1799-1896：TDxControl 构造默认值
    // =====================================================================

    /// <summary>用例 17：TDxControl（借 TDxControlProbe 实例，它不覆写任何默认值）逐项默认值。</summary>
    [Fact]
    public void TDxControl_ConstructorDefaults()
    {
        var c = new TDxControlProbe();
        Assert.Equal(TMouseEvents.Default, c.MouseEvents);
        Assert.Equal("", c.Caption);
        Assert.Equal(TReferenceX.rxLeft, c.ReferenceX);
        Assert.False(c.AdjustYByHeight);
        Assert.False(c.TopAlignment);
        Assert.True(c.AutoSize);
        Assert.Equal(TDxAlignment.taLeftJustify, c.Alignment);
        Assert.True(c.Transparent);
        Assert.Equal(TDxColor.clWhite, c.BackgroundColor);
        Assert.False(c.DrawBorder);
        Assert.False(c.EnableFocus);
        Assert.Equal(0, c.TabOrder);
        Assert.Equal(TDxAlign.alNone, c.Align);
        Assert.False(c.Center);
        Assert.False(c.OwnerMove);
        Assert.False(c.Floating);
        Assert.True(c.Designing);
        Assert.True(c.Visible);
        Assert.True(c.Enabled);
        Assert.True(c.CanMouse);
        Assert.True(c.EnableMouse);
        Assert.Equal(0, c.BlendMode);
        Assert.Equal(0, c.MouseDownBlendMode);
        Assert.Equal(0, c.MouseMoveBlendMode);
        Assert.Equal(TDxRect.Bounds(0, 0, 0, 0), c.ClientRect);
        Assert.Equal(0, c.ControlID);
        Assert.Equal("", c.ShowName);
        Assert.Equal(0, c.AutoSizeSetFlag);
        Assert.NotNull(c.ImageIndex);
        Assert.NotNull(c.BorderColor);
        Assert.NotNull(c.DrawCaptionFont);
    }

    /// <summary>
    /// 用例 18（差异断言）：MouseEvents 默认是三键都开（1855），
    /// **不是** TMouseEvents 的 0 值 —— 写成 None 会让控件收不到任何鼠标事件。
    /// </summary>
    [Fact]
    public void TDxControl_MouseEventsDefaultIsThreeButtons()
    {
        var c = new TDxLine();
        Assert.Equal(TMouseEvents.mbLeft | TMouseEvents.mbRight | TMouseEvents.mbMiddle, c.MouseEvents);
        Assert.NotEqual(TMouseEvents.None, c.MouseEvents);
    }

    /// <summary>用例 19：GetPosition/SetPosition（2375-2411）—— 平移不改尺寸，宽高只改右下。</summary>
    [Fact]
    public void SetPosition_WidthHeightOnlyMoveRightBottom()
    {
        var c = NewLabel(10, 20, 30, 40);
        Assert.Equal(R(10, 20, 40, 60), c.ClientRect);

        c.SetPosition(2, 100);                       // 只改 Right
        Assert.Equal(R(10, 20, 110, 60), c.ClientRect);
        Assert.Equal(100, c.Width);

        c.SetPosition(3, 50);                        // 只改 Bottom
        Assert.Equal(R(10, 20, 110, 70), c.ClientRect);
        Assert.Equal(50, c.Height);

        c.SetPosition(0, 7);                         // 平移：Left 变，宽不变
        Assert.Equal(R(7, 20, 107, 70), c.ClientRect);

        c.SetPosition(1, 8);
        Assert.Equal(R(7, 8, 107, 58), c.ClientRect);

        Assert.Equal(0, c.GetPosition(0));
        Assert.Equal(0, c.GetPosition(1));
        Assert.Equal(100, c.GetPosition(2));
        Assert.Equal(50, c.GetPosition(3));
        Assert.Equal(0, c.GetPosition(9));           // 越界 index → 0
    }

    // =====================================================================
    // DxControls.pas 2219-2251 / 3095-3223：矩形几何
    // =====================================================================

    /// <summary>用例 20：无父控件时 VisibleRect = VirtualRect = ClientRect（2225-2228 / 2245-2248）。</summary>
    [Fact]
    public void VisibleAndVirtualRect_WithoutOwner_EqualClientRect()
    {
        var c = NewLabel(10, 20, 30, 40);
        Assert.Equal(c.ClientRect, c.VirtualRect);
        Assert.Equal(c.ClientRect, c.VisibleRect);
    }

    /// <summary>用例 21：有父控件时 VisibleRect = 子区域 ∩ 父可见区（2230-2237）。</summary>
    [Fact]
    public void VisibleRect_IsIntersectionWithOwnerSurface()
    {
        var c = NewLabel(10, 20, 30, 40);         // ClientRect = (10,20,40,60)
        c.DxOwner = new TDxLabel();
        c.VirtualRectOverride = () => R(10, 20, 40, 60);
        c.DxOwner.VirtualRectOverride = () => R(0, 0, 100, 100);
        c.DxOwner.ClientRect = R(0, 0, 100, 100);

        Assert.Equal(R(10, 20, 40, 60), c.VirtualRect);
        Assert.Equal(R(10, 20, 40, 60), c.VisibleRect);

        // 父可见区被裁到 (25,25,100,100) → 交集只留右下
        c.DxOwner.VirtualRectOverride = () => R(0, 0, 100, 100);
        var clipped = DxRectUtil.ShortRect(c.VirtualRect, R(25, 25, 100, 100));
        Assert.Equal(R(25, 25, 40, 60), clipped);
    }

    /// <summary>用例 22：ReallyRect（3199-3205）—— 可视区换算到虚拟区坐标。</summary>
    [Fact]
    public void ReallyRect_SubtractsVirtualOrigin()
    {
        Assert.Equal(R(0, 0, 10, 20), TDxControl.ReallyRect(R(100, 200, 110, 220), R(100, 200, 0, 0)));
        Assert.Equal(R(5, 5, 15, 25), TDxControl.ReallyRect(R(105, 205, 115, 225), R(100, 200, 0, 0)));
        // 可见区完全在虚拟区左侧 → Left 为负
        var r = TDxControl.ReallyRect(R(90, 190, 100, 200), R(100, 200, 0, 0));
        Assert.Equal(-10, r.Left);
        Assert.Equal(-10, r.Top);
    }

    /// <summary>用例 23：CanDraw 与 CanDraw(DestRect)（3208-3223）。</summary>
    [Fact]
    public void CanDraw_RequiresPositiveIntersection()
    {
        var c = NewLabel(0, 0, 10, 10);

        Assert.True(c.CanDraw());
        Assert.True(c.CanDraw(R(5, 5, 20, 20)));
        Assert.False(c.CanDraw(R(10, 0, 20, 10)));       // 交集宽度为 0
        Assert.False(c.CanDraw(R(0, 10, 10, 20)));       // 交集高度为 0
        Assert.False(c.CanDraw(R(11, 0, 20, 10)));       // 完全在外

        var invisible = NewLabel(0, 0, 0, 0);
        Assert.False(invisible.CanDraw());
    }

    /// <summary>用例 24：ReallyPaintRect 无裁剪时 nX/nY 取目标矩形左上角。</summary>
    [Fact]
    public void ReallyPaintRect_NoClipping()
    {
        var c = NewLabel(0, 0, 100, 100);
        var vt = R(0, 0, 100, 100);
        var vb = R(0, 0, 100, 100);

        var paint = c.ReallyPaintRect(R(10, 10, 40, 30), R(0, 0, 30, 20), vt, vb, out int nx, out int ny);
        Assert.Equal(10, nx);
        Assert.Equal(10, ny);
        Assert.Equal(R(0, 0, 30, 20), paint);
    }

    /// <summary>用例 25（差异断言）：ReallyPaintRect 的 nWidth 基准在 vb.Right &lt; dest.Right 时用 vb.Right。</summary>
    [Fact]
    public void ReallyPaintRect_ClipUsesVisibleRectRight()
    {
        var c = NewLabel(0, 0, 100, 100);
        var vt = R(0, 0, 100, 100);
        var vb = R(0, 0, 50, 100);               // 可见区只到 x=50

        var paint = c.ReallyPaintRect(R(10, 0, 90, 10), R(0, 0, 80, 10), vt, vb, out int nx, out int ny);
        Assert.Equal(10, nx);                     // vb.Left(0) 不大于 dest.Left(10) → nLeft = 0
        Assert.Equal(0, ny);
        // nWidth = 50 - 10 = 40 → 宽度落到 40（若误用 dest.Right 会得到 80）
        Assert.Equal(R(0, 0, 40, 10), paint);
    }

    /// <summary>用例 26：ReallyPaintRect 在不可见区（Bottom&lt;=Top）时返回空矩形且 nX=nY=-1。</summary>
    [Fact]
    public void ReallyPaintRect_InvisibleReturnsEmptyAndMinusOne()
    {
        var c = NewLabel(0, 0, 100, 100);
        var paint = c.ReallyPaintRect(R(10, 10, 40, 30), R(0, 0, 30, 20),
            R(0, 0, 100, 100), R(0, 0, 100, 0), out int nx, out int ny);

        Assert.Equal(R(0, 0, 0, 0), paint);
        Assert.Equal(-1, nx);
        Assert.Equal(-1, ny);
    }

    /// <summary>用例 27：ReallyPaintRect 的 nLeft/nTop 在目标矩形越过可见区左上时生效。</summary>
    [Fact]
    public void ReallyPaintRect_DestAboveVisibleGetsOffset()
    {
        var c = NewLabel(0, 0, 100, 100);
        var vt = R(0, 0, 100, 100);
        var vb = R(20, 20, 100, 100);

        var paint = c.ReallyPaintRect(R(0, 0, 100, 100), R(0, 0, 100, 100), vt, vb, out int nx, out int ny);
        Assert.Equal(20, nx);                     // nLeft = vb.Left - dest.Left = 20
        Assert.Equal(20, ny);
        // nWidth = dest.Right - dest.Left - nLeft = 100 - 0 - 20 = 80（vb.Right=100 不小于 dest.Right=100）
        // paintRect.Left = SrcRect.Left + nLeft = 20 → Right = 20 + 80 = 100
        Assert.Equal(R(20, 20, 100, 100), paint);
    }

    // =====================================================================
    // DxControls.pas 3752-3850：DrawCaption
    // =====================================================================

    private static TDxLabel CaptionHost(out TDxRecordingPainter painter, bool bold)
    {
        var c = new TDxLabel { Left = 0, Top = 0, Width = 100, Height = 20 };
        c.FontEnv.GetImageInfos = (f, s) => TDxFontEnv.BuildImageInfos(s);
        c.FontEnv.TextWidth = (f, s) => TDxFontEnv.MeasureTextWidth(s);
        c.FontEnv.TextHeight = (f, s) => TDxFontEnv.MeasureTextHeight(s);
        c.FontEnv.FindFont = (n, sz, st) => new object();
        painter = new TDxRecordingPainter();
        c.Painter = painter;
        return c;
    }

    /// <summary>用例 28：非 Bold 时只发出一次 TextRect（3777-3821 的 else 分支）。</summary>
    [Fact]
    public void DrawCaption_NonBoldEmitsSingleTextRect()
    {
        var c = CaptionHost(out var painter, bold: false);
        var font = new TDxFont { Color = 0x112233, Bold = false };
        var infos = TDxFontEnv.BuildImageInfos("AB");

        c.DrawCaption(new object(), font, TDxAlignment.taLeftJustify, infos,
            R(0, 0, 12, 12), R(0, 0, 100, 20), R(0, 0, 100, 20), 0, 0);

        Assert.Single(painter.Ops);
        Assert.StartsWith("TextRect(", painter.Ops[0]);
        Assert.Contains("112233", painter.Ops[0]);
    }

    /// <summary>用例 29（差异断言）：Bold 时要先画 4 次偏移 1 像素的描边，最后才画正文 —— 共 5 次。</summary>
    [Fact]
    public void DrawCaption_BoldEmitsFourOffsetsThenBody()
    {
        var c = CaptionHost(out var painter, bold: true);
        var font = new TDxFont { Color = 0x112233, BColor = 0x445566, Bold = true };
        var infos = TDxFontEnv.BuildImageInfos("A");

        c.DrawCaption(new object(), font, TDxAlignment.taLeftJustify, infos,
            R(0, 0, 6, 12), R(0, 0, 100, 20), R(0, 0, 100, 20), 0, 0);

        Assert.Equal(5, painter.Ops.Count);
        Assert.Contains("445566", painter.Ops[0]);
        Assert.Contains("445566", painter.Ops[1]);
        Assert.Contains("445566", painter.Ops[2]);
        Assert.Contains("445566", painter.Ops[3]);
        Assert.Contains("112233", painter.Ops[4]);      // 正文最后

        // 描边顺序：(-1,0) (+1,0) (0,-1) (0,+1)；垂直居中量 nTop = (20-12) div 2 = 4
        Assert.StartsWith("TextRect(-1,4,", painter.Ops[0]);
        Assert.StartsWith("TextRect(1,4,", painter.Ops[1]);
        Assert.StartsWith("TextRect(0,3,", painter.Ops[2]);
        Assert.StartsWith("TextRect(0,5,", painter.Ops[3]);
        Assert.StartsWith("TextRect(0,4,", painter.Ops[4]);
    }

    /// <summary>用例 30（差异断言）：taLeftJustify/taCenter/taRightJustify 的 nLeft 公式互不相同。</summary>
    [Fact]
    public void DrawCaption_AlignmentProducesDistinctLeftOffsets()
    {
        var dest = R(0, 0, 20, 10);
        var vt = R(0, 0, 100, 20);
        var vb = R(0, 0, 100, 20);
        var infos = TDxFontEnv.BuildImageInfos("A");

        int LeftFor(TDxAlignment a)
        {
            var c = CaptionHost(out var p, bold: false);
            c.Alignment = a;
            c.DrawCaption(new object(), new TDxFont(), a, infos, dest, vb, vt, 0, 0);
            var op = p.Ops[0];
            int open = op.IndexOf('(') + 1;
            int comma = op.IndexOf(',', open);
            return int.Parse(op.Substring(open, comma - open));
        }

        int left = LeftFor(TDxAlignment.taLeftJustify);
        int center = LeftFor(TDxAlignment.taCenter);
        int right = LeftFor(TDxAlignment.taRightJustify);

        Assert.Equal(0, left);                    // nLeft = X = 0
        Assert.Equal(40, center);                 // (100 - 20) div 2 = 40
        Assert.Equal(80, right);                  // 100 - 20 = 80
        Assert.NotEqual(left, center);
        Assert.NotEqual(center, right);
    }

    /// <summary>用例 31：空文本图像表 → 一次都不画（3785 的 Length &gt; 0 守卫）。</summary>
    [Fact]
    public void DrawCaption_EmptyImageInfosDrawsNothing()
    {
        var c = CaptionHost(out var painter, bold: false);
        c.DrawCaption(new object(), new TDxFont(), TDxAlignment.taLeftJustify,
            new List<TDxTextImageInfo>(), R(0, 0, 10, 10), R(0, 0, 100, 20), R(0, 0, 100, 20), 0, 0);
        Assert.Empty(painter.Ops);
    }

    /// <summary>
    /// 用例 32（差异断言）：文本为 "-" 时走的是「画一条 1 像素中横线」的特例（3839-3844），
    /// **不**查字形、**不**发 TextRect。
    /// </summary>
    [Fact]
    public void DrawCaption_DashCaptionDrawsSinglePixelLine()
    {
        var c = CaptionHost(out var painter, bold: false);
        var vt = R(0, 0, 100, 21);
        c.DrawCaption(new object(), new TDxFont { Color = 0xABCDEF }, "-", R(0, 0, 100, 21), vt, vt, 0, 0);

        Assert.Single(painter.Ops);
        Assert.StartsWith("FillRect(", painter.Ops[0]);
        // Top = 0 + (21-0) div 2 = 10，Bottom = 11 → 高 1
        Assert.Contains("(0,10,100,11)", painter.Ops[0]);
        Assert.Contains("ABCDEF", painter.Ops[0]);
    }

    /// <summary>用例 33：空标题 → 不画（3838 的 ACaption &lt;&gt; '' 守卫）。</summary>
    [Fact]
    public void DrawCaption_EmptyCaptionDrawsNothing()
    {
        var c = CaptionHost(out var painter, bold: false);
        var vt = R(0, 0, 100, 20);
        c.DrawCaption(new object(), new TDxFont(), "", R(0, 0, 100, 20), vt, vt, 0, 0);
        Assert.Empty(painter.Ops);
    }

    // =====================================================================
    // DxControls.pas 2123-2138：CheckAutoSize
    // =====================================================================

    /// <summary>用例 34：CheckAutoSize 只在 (AutoSize 且 flag=0 且 Up&gt;=0 且 图面积&gt;=4) 时定尺一次。</summary>
    [Fact]
    public void CheckAutoSize_SetsSizeOnceAndLatchesFlag()
    {
        var c = NewLabel(5, 6, 10, 10);
        var lib = new TDxImageLibraryStub().Add(new TDxTextureStub(64, 32));
        c.ImageIndex.Image = lib;
        c.ImageIndex.Up = 0;

        c.CallCheckAutoSizeBase();
        Assert.Equal(64, c.Width);
        Assert.Equal(32, c.Height);
        Assert.Equal(0xFF, c.AutoSizeSetFlag);

        // 第二次不再改（flag 已置位）
        c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(8, 8));
        c.CallCheckAutoSizeBase();
        Assert.Equal(64, c.Width);
    }

    /// <summary>用例 35（差异断言）：图面积必须 **&gt;= 4** —— 1×1 / 1×3 这类小图不触发定尺。</summary>
    [Fact]
    public void CheckAutoSize_RequiresAreaAtLeastFour()
    {
        foreach (var (w, h, expectChange) in new[] { (1, 1, false), (2, 2, true), (1, 3, false), (1, 4, true) })
        {
            var c = NewLabel(0, 0, 10, 10);
            c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(w, h));
            c.ImageIndex.Up = 0;
            c.CallCheckAutoSizeBase();
            Assert.Equal(expectChange, c.Width == w);
        }
    }

    /// <summary>用例 36：AutoSize=False 时不定尺；ImageIndex.Up &lt; 0 时不定尺。</summary>
    [Fact]
    public void CheckAutoSize_GuardsOnAutoSizeAndFaceIndex()
    {
        var c = NewLabel(0, 0, 10, 10);
        c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(50, 50));
        c.ImageIndex.Up = 0;
        c.AutoSize = false;
        c.CallCheckAutoSizeBase();
        Assert.Equal(10, c.Width);

        var c2 = NewLabel(0, 0, 10, 10);
        c2.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(50, 50));
        c2.ImageIndex.Up = -1;                    // 无 up 面
        c2.CallCheckAutoSizeBase();
        Assert.Equal(10, c2.Width);
    }

    // =====================================================================
    // HGEFontEx.pas 448-502 / 693-769：字体度量镜像
    // =====================================================================

    /// <summary>用例 37：MeasureTextWidth —— 纯 ASCII 走 FFontWidth(6)。</summary>
    [Fact]
    public void MeasureTextWidth_AsciiUsesFontWidthSix()
    {
        Assert.Equal(0, TDxFontEnv.MeasureTextWidth(""));
        Assert.Equal(6, TDxFontEnv.MeasureTextWidth("A"));
        Assert.Equal(18, TDxFontEnv.MeasureTextWidth("ABC"));
    }

    /// <summary>
    /// 用例 38（差异断言）：含双字节（GBK）字符时按 `DoubleWidth*双字节数 + FontWidth*单字节数` ——
    /// 原文用 **AnsiString 字节数** 减 WideString 长度算双字节字符数，故一个汉字 = 12 像素。
    /// </summary>
    [Fact]
    public void MeasureTextWidth_DoubleByteCharsUseDoubleWidth()
    {
        // 一个字（GBK 2 字节）：nsCount=2, nwCount=1, nCount=1
        // 结果 = 12*1 + (1-1)*6 = 12
        Assert.Equal(12, TDxFontEnv.MeasureTextWidth("一"));

        // 两个汉字：nsCount=4, nwCount=2, nCount=2 → 12*2 + 0 = 24
        Assert.Equal(24, TDxFontEnv.MeasureTextWidth("一二"));

        // 汉字 + ASCII：nsCount=3, nwCount=2, nCount=1 → 12*1 + (2-1)*6 = 18
        Assert.Equal(18, TDxFontEnv.MeasureTextWidth("一A"));
    }

    /// <summary>用例 39：MeasureTextHeight —— 纯 ASCII 与含双字节都推导为 12（Max(12,12)）。</summary>
    [Fact]
    public void MeasureTextHeight_AlwaysTwelveByDefaults()
    {
        Assert.Equal(12, TDxFontEnv.MeasureTextHeight(""));
        Assert.Equal(12, TDxFontEnv.MeasureTextHeight("A"));
        Assert.Equal(12, TDxFontEnv.MeasureTextHeight("一"));
    }

    /// <summary>用例 40：SplitTextLines / BuildImageInfos —— 一行一项，结尾换行不产生空行。</summary>
    [Fact]
    public void BuildImageInfos_OneEntryPerLine()
    {
        Assert.Empty(TDxFontEnv.BuildImageInfos(""));
        Assert.Single(TDxFontEnv.BuildImageInfos("A"));
        Assert.Single(TDxFontEnv.BuildImageInfos("A\r\n"));      // 结尾换行不算新行
        Assert.Equal(2, TDxFontEnv.BuildImageInfos("A\nB").Count);
        Assert.Equal(2, TDxFontEnv.BuildImageInfos("A\r\nB").Count);
        Assert.Equal(3, TDxFontEnv.BuildImageInfos("A\n\nB").Count);   // 中间空行占一项
    }

    /// <summary>用例 41（差异断言）：空行那一项的 Width/Height 都是 0（原文 713 的 Length(LineText) &gt; 0 守卫）。</summary>
    [Fact]
    public void BuildImageInfos_EmptyLineProducesZeroSizedEntry()
    {
        var infos = TDxFontEnv.BuildImageInfos("A\n\nB");
        Assert.Equal(3, infos.Count);
        Assert.Equal(6, infos[0].Width);
        Assert.Equal(0, infos[1].Width);        // 中间空行
        Assert.Equal(0, infos[1].Height);
        Assert.Equal(6, infos[2].Width);
        Assert.Empty(infos[1].ImageIndexs);       // 空行没有字形
        Assert.Single(infos[0].ImageIndexs);
        Assert.Single(infos[2].ImageIndexs);
    }

    /// <summary>用例 42：BuildImageInfos 的行宽 = Σ 单字宽；行高 = 单字高。</summary>
    [Fact]
    public void BuildImageInfos_AccumulatesWidths()
    {
        var infos = TDxFontEnv.BuildImageInfos("AB");
        Assert.Single(infos);
        Assert.Equal(12, infos[0].Width);
        Assert.Equal(12, infos[0].Height);

        var mixed = TDxFontEnv.BuildImageInfos("A一");
        Assert.Single(mixed);
        Assert.Equal(18, mixed[0].Width);
    }
}

/// <summary>测试辅助：最薄的 TDxControl 具体化（不覆写任何默认值，用于测基类默认值）。</summary>
internal sealed class TDxControlProbe : TDxControl
{
}

/// <summary>测试辅助：把 TDxControl 的 protected CheckAutoSizeBase 暴露出来。</summary>
internal static class DxControlTestExtensions
{
    public static void CallCheckAutoSizeBase(this TDxControl c)
        => DxTestAccess.Invoke(c);

    public static void CallDoCaptionChange(this TDxControl c)
        => DxTestAccess.InvokeCaptionChange(c);
}

/// <summary>测试辅助：受保护成员的调用入口（原文保护级在托管侧的等价开口）。</summary>
internal static class DxTestAccess
{
    private static readonly System.Reflection.MethodInfo CheckMethod =
        typeof(TDxControl).GetMethod("CheckAutoSizeBase",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

    private static readonly System.Reflection.MethodInfo CaptionMethod =
        typeof(TDxControl).GetMethod("DoCaptionChange",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

    public static void Invoke(TDxControl c) => CheckMethod.Invoke(c, null);

    public static void InvokeCaptionChange(TDxControl c) => CaptionMethod.Invoke(c, null);
}
