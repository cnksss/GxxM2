using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DxLabel.pas 1-262 的 1:1 测试。
/// 覆盖构造默认值、RowSpacing/ExpandLineHeight、DoCaptionChange 的两套行高公式差异、
/// Paint 的字体状态选择与尺寸公式、InRange 的 OnInRealArea 改判、Assign 逐属性拷贝。
/// </summary>
public sealed class DxLabelTests
{
    private static TDxRect R(int l, int t, int r, int b) => TDxRect.Rect(l, t, r, b);

    /// <summary>建立一个带字形接缝的 TDxLabel（TextHeight('0')=12、单字宽 6）。</summary>
    private static TDxLabel NewLabel(string caption = "", bool bold = false)
    {
        var c = new TDxLabel { Left = 0, Top = 0, Width = 200, Height = 100 };
        c.FontEnv.GetImageInfos = (f, s) => TDxFontEnv.BuildImageInfos(s);
        c.FontEnv.TextWidth = (f, s) => TDxFontEnv.MeasureTextWidth(s);
        c.FontEnv.TextHeight = (f, s) => TDxFontEnv.MeasureTextHeight(s);
        c.FontEnv.FindFont = (n, sz, st) => new object();
        c.CaptionColor.Up.Bold = bold;
        if (caption != "")
            c.Caption = caption;
        return c;
    }

    // =====================================================================
    // 构造（41-53）
    // =====================================================================

    /// <summary>用例 1：TDxLabel 构造默认值逐项（43-52）。</summary>
    [Fact]
    public void Constructor_Defaults()
    {
        var c = new TDxLabel();

        Assert.True(c.Transparent);          // 44
        Assert.True(c.AutoSize);             // 45
        Assert.Equal("", c.Caption);         // 46（原文 Caption := Name，Name 默认空串）
        Assert.Equal(32, c.Width);           // 47
        Assert.Equal(16, c.Height);          // 48
        Assert.Equal(0, c.CaptionDownOffsetX); // 49
        Assert.Equal(0, c.CaptionDownOffsetY); // 50
        Assert.Equal(0, c.RowSpacing);       // 51
        Assert.Equal(0, c.ExpandLineHeight); // 52
    }

    /// <summary>
    /// 用例 2（差异断言）：TDxLabel 把基类 TDxImageButton 的 CaptionDownOffset 从 1/1 覆盖成 0/0 ——
    /// 若沿用按钮默认值，按下时文字会整体偏移 1 像素。
    /// </summary>
    [Fact]
    public void Constructor_OverridesButtonDownOffsetsToZero()
    {
        Assert.Equal(0, new TDxLabel().CaptionDownOffsetX);
        Assert.Equal(0, new TDxLabel().CaptionDownOffsetY);
        Assert.Equal(1, new TDxImageButton().CaptionDownOffsetX);
        Assert.Equal(1, new TDxImageButton().CaptionDownOffsetY);
    }

    /// <summary>用例 3：TDxImageButton 构造默认（471-488）在 TDxLabel 上可见的部分。</summary>
    [Fact]
    public void InheritedButtonDefaults()
    {
        var c = new TDxLabel();
        Assert.Equal(TDxAlignment.taCenter, c.Alignment);   // 474
        Assert.Equal(TButtonStyle.bsButton, c.Style);       // 476
        Assert.False(c.Checked);                            // 477
        Assert.Equal(0, c.ButtonDownOffsetX);               // 481
        Assert.Equal(0, c.ButtonDownOffsetY);               // 482
        Assert.Equal(TGuiType.t_Button, c.GuiType);         // 488
        Assert.NotNull(c.CaptionColor);
        Assert.NotNull(c.BorderColor);
    }

    // =====================================================================
    // RowSpacing / ExpandLineHeight（55-61）
    // =====================================================================

    /// <summary>用例 4：SetRowSpacing 只在值变化时才调 DoCaptionChange（55-61）。</summary>
    [Fact]
    public void RowSpacing_TriggersCaptionChangeOnlyOnChange()
    {
        var c = NewLabel("A");
        c.CaptionColor.Up.Bold = false;      // 排除 Bold 的 +2 干扰
        c.RowSpacing = 0;
        c.Caption = "A";
        Assert.Equal(12, c.Height);          // 1 行 × (12 + 0)

        c.RowSpacing = 0;                    // 同值 → 不改宽高
        Assert.Equal(0, c.RowSpacing);
        Assert.Equal(12, c.Height);

        c.RowSpacing = 5;
        Assert.Equal(5, c.RowSpacing);
        Assert.Equal(17, c.Height);          // 行高 = 12 + 5

        c.RowSpacing = -3;
        Assert.Equal(-3, c.RowSpacing);
        Assert.Equal(9, c.Height);           // 原文不夹紧负值
    }

    /// <summary>用例 5：ExpandLineHeight 是 published 直写字段（无 setter），赋值不触发 DoCaptionChange。</summary>
    [Fact]
    public void ExpandLineHeight_DoesNotTriggerCaptionChange()
    {
        var c = new TDxLabel();
        c.FontEnv.FindFont = (n, sz, st) => new object();
        c.FontEnv.GetImageInfos = (f, s) => TDxFontEnv.BuildImageInfos(s);
        c.FontEnv.TextHeight = (f, s) => 12;
        c.Caption = "A";
        int before = c.Height;

        c.ExpandLineHeight = 7;
        Assert.Equal(7, c.ExpandLineHeight);
        Assert.Equal(before, c.Height);      // 与 RowSpacing 不同：不重算尺寸
    }

    // =====================================================================
    // DoCaptionChange（118-152）
    // =====================================================================

    /// <summary>用例 6：字体为 nil 时整段不执行（124-126）—— 连 AutoSize 都不进。</summary>
    [Fact]
    public void DoCaptionChange_WithoutFontDoesNothing()
    {
        var c = new TDxLabel { Width = 200, Height = 100 };
        c.FontEnv.FindFont = (n, sz, st) => null;   // 无字体
        c.FontEnv.GetImageInfos = (f, s) => TDxFontEnv.BuildImageInfos(s);
        c.Caption = "ABC";
        Assert.Equal(200, c.Width);
        Assert.Equal(100, c.Height);
    }

    /// <summary>用例 7：AutoSize 时 Width = 各行最大宽（不加 RowSpacing），Height = (TextHeight('0')+RowSpacing) × 行数。</summary>
    [Fact]
    public void DoCaptionChange_AutoSizeUsesRowSpacingInLineHeight()
    {
        var c = NewLabel("AB");
        Assert.Equal(12, c.Width);
        Assert.Equal(12, c.Height);          // 1 行 × (12+0)

        c.RowSpacing = 3;
        c.Caption = "AB";                    // 触发重算（同值不触发，改用下面赋值）
        c.Caption = "ABC";
        Assert.Equal(18, c.Width);
        Assert.Equal(15, c.Height);          // 1 行 × (12+3)
    }

    /// <summary>用例 8：多行时高 = 行数 × (TextHeight('0') + RowSpacing)，宽取最大行宽。</summary>
    [Fact]
    public void DoCaptionChange_MultiLineHeight()
    {
        var c = NewLabel("A\nBBB\nCC");
        Assert.Equal(18, c.Width);           // "BBB" = 3×6
        Assert.Equal(36, c.Height);          // 3 行 × 12
    }

    /// <summary>
    /// 用例 9（差异断言）：AutoSize=False 时 DoCaptionChange **什么都不做** ——
    /// 注意它仍然会执行 `TextImages := HGEFont.GetImageInfos(Caption)`（127），
    /// 只是尺寸保持不动；而 Paint 的公式与这里**不同**（见用例 13）。
    /// </summary>
    [Fact]
    public void DoCaptionChange_AutoSizeFalseKeepsSize()
    {
        var c = NewLabel("ABCDEF");
        Assert.Equal(36, c.Width);

        c.AutoSize = false;                  // 关闭后触发一次（值变）
        Assert.Equal(36, c.Width);           // 保持不动
        c.Caption = "A";
        Assert.Equal(36, c.Width);           // 仍不动
        Assert.Equal(12, c.Height);
    }

    /// <summary>用例 10：Up 字体 Bold 时宽高各 +2（144-147）。</summary>
    [Fact]
    public void DoCaptionChange_BoldAddsTwoPixels()
    {
        var c = NewLabel("AB", bold: true);
        Assert.Equal(14, c.Width);           // 12 + 2
        Assert.Equal(14, c.Height);          // 12 + 2
    }

    /// <summary>用例 11：空标题 → GetImageInfos 返回空表 → 循环零次 → 宽高被置 0（AutoSize 时）。</summary>
    [Fact]
    public void DoCaptionChange_EmptyCaptionSetsZeroSize()
    {
        var c = NewLabel();
        c.Caption = "A";
        Assert.Equal(6, c.Width);

        c.Caption = "";
        Assert.Equal(0, c.Width);
        Assert.Equal(0, c.Height);
    }

    // =====================================================================
    // PaintTo（154-243）
    // =====================================================================

    /// <summary>用例 12：VisibleRect 高度为 0 → 立即 Exit，一次绘制都不发（165）。</summary>
    [Fact]
    public void Paint_ZeroHeightExitsImmediately()
    {
        var c = NewLabel("A");
        c.Height = 0;
        var p = new TDxRecordingPainter();
        var size = c.PaintTo(p);

        Assert.Empty(p.Ops);
        Assert.Equal(new TDxPoint(0, 0), size);
    }

    /// <summary>
    /// 用例 13（差异断言）：Paint 的尺寸公式与 DoCaptionChange **不同** ——
    /// Paint 用 `TextImages[I].Height + FExpandLineHeight`（且 TextImages[I].Height 为 0 时
    /// 退回 `TextHeight('0') + FExpandLineHeight`），**不用 RowSpacing**。
    /// </summary>
    [Fact]
    public void Paint_HeightFormulaIgnoresRowSpacingButUsesExpandLineHeight()
    {
        var c = NewLabel("A");
        c.RowSpacing = 100;                  // 只影响 DoCaptionChange
        c.ExpandLineHeight = 5;              // 只影响 Paint

        var p = new TDxRecordingPainter();
        var size = c.PaintTo(p);

        // TextImages[0].Height = 12 > 0 → 12 + 5 = 17
        Assert.Equal(17, size.Y);
        Assert.Equal(6, size.X);
    }

    /// <summary>用例 14（差异断言）：Bold 时 Paint 的宽高也各 +2。</summary>
    [Fact]
    public void Paint_BoldAddsTwoPixels()
    {
        var c = NewLabel("AB", bold: true);
        var p = new TDxRecordingPainter();
        var size = c.PaintTo(p);
        Assert.Equal(14, size.X);
        Assert.Equal(14, size.Y);
    }

    /// <summary>用例 15：Caption 为空时跳过文本绘制，但不跳过边框（196 的守卫）。</summary>
    [Fact]
    public void Paint_EmptyCaptionSkipsTextButStillDrawsBorder()
    {
        var c = NewLabel();
        c.DrawBorder = true;
        var p = new TDxRecordingPainter();
        var size = c.PaintTo(p);

        Assert.Equal(new TDxPoint(0, 0), size);
        Assert.Single(p.Ops);
        Assert.StartsWith("FrameRect(", p.Ops[0]);
    }

    /// <summary>
    /// 用例 16（差异断言）：字体状态选择优先级
    /// Down(MouseDowned or Checked) &gt; Hot(MouseMoveed) &gt; Up，Disabled 独立。
    /// </summary>
    [Fact]
    public void Paint_FontStateSelectionOrder()
    {
        string ColorOf(TDxLabel c)
        {
            var p = new TDxRecordingPainter();
            c.PaintTo(p);
            // 最后一条 TextRect 是正文（Bold 时前面还有 4 次描边），其颜色参数即所选字体颜色。
            // 注意：nX/nY 之后紧跟的是矩形（内含逗号），故用正则从尾部取 "颜色,blend,alpha,expand"
            var op = p.Ops[p.Ops.Count - 1];
            var m = System.Text.RegularExpressions.Regex.Match(op, @",([0-9A-F]{6}),(\d+),(\d+),(\d+)\)$");
            Assert.True(m.Success, "无法从绘制串解析颜色: " + op);
            return m.Groups[1].Value;
        }

        TDxLabel Setup(Action<TDxLabel> apply)
        {
            var c = NewLabel("A");
            c.CaptionColor.Up.Color = 0x010101;
            c.CaptionColor.Hot.Color = 0x020202;
            c.CaptionColor.Down.Color = 0x030303;
            c.CaptionColor.Disabled.Color = 0x040404;
            apply(c);
            return c;
        }

        Assert.Equal("010101", ColorOf(Setup(c => { })));
        Assert.Equal("020202", ColorOf(Setup(c => c.MouseMoveed = true)));
        Assert.Equal("030303", ColorOf(Setup(c => c.MouseDowned = true)));
        Assert.Equal("030303", ColorOf(Setup(c => c.Checked = true)));
        Assert.Equal("040404", ColorOf(Setup(c => { c.Enabled = false; c.MouseDowned = true; })));
    }

    /// <summary>
    /// 用例 17（差异断言）：`if Style &lt;&gt; bsButton then begin X := 0; Y := 0; end;` ——
    /// 非按钮样式（bsRadio/bsCheckBox）时按下偏移被强制清零。
    ///
    /// 注意对齐：DrawCaption 用的是**字体**的对齐（见用例 30 的差异断言），
    /// 故此处把 Up/Down 字体对齐设为 taLeftJustify 才能直接读出 X/Y 的贡献。
    /// </summary>
    [Fact]
    public void Paint_NonButtonStyleZeroesDownOffsets()
    {
        static TDxPoint TextOrigin(string op)
        {
            var m = System.Text.RegularExpressions.Regex.Match(op, @"^TextRect\((-?\d+),(-?\d+),");
            Assert.True(m.Success, "无法解析 TextRect 起点: " + op);
            return new TDxPoint(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
        }

        var c = NewLabel("A");
        c.AutoSize = false;                      // 固定 200×100，避免自动定尺把文本挤出可见区
        c.Left = 0; c.Top = 0; c.Width = 200; c.Height = 100;
        c.CaptionColor.Up.Alignment = TDxAlignment.taLeftJustify;
        c.CaptionColor.Down.Alignment = TDxAlignment.taLeftJustify;
        // DrawCaption 实际取控件的 Alignment（原文 3823-3828 → Self.Alignment；TDxImageButton 构造置 taCenter）
        c.Alignment = TDxAlignment.taLeftJustify;
        c.CaptionDownOffsetX = 7;
        c.CaptionDownOffsetY = 9;
        c.MouseDowned = true;

        var p1 = new TDxRecordingPainter();
        c.PaintTo(p1);
        // Down 字体默认 Bold=True → 会先发 4 次描边，正文是**最后**一条
        var originButton = TextOrigin(p1.Ops[p1.Ops.Count - 1]);
        Assert.Equal(7, originButton.X);         // bsButton：按下偏移 X=7 生效
        // nTop = (AVirtualRect 高 - DestRect 高) div 2 + Y = (100-14) div 2 + 9 = 43 + 9
        Assert.Equal(52, originButton.Y);

        c.Style = TButtonStyle.bsRadio;
        var p2 = new TDxRecordingPainter();
        c.PaintTo(p2);
        var originRadio = TextOrigin(p2.Ops[p2.Ops.Count - 1]);
        Assert.Equal(0, originRadio.X);          // 非按钮：X/Y 被清零
        Assert.Equal(43, originRadio.Y);         // (100-14) div 2 + 0
        Assert.NotEqual(originButton, originRadio);
    }

    /// <summary>用例 18：Transparent=False 时先填充背景（172-173）。</summary>
    [Fact]
    public void Paint_NonTransparentFillsBackgroundFirst()
    {
        var c = NewLabel("A");
        c.Transparent = false;
        c.BackgroundColor = 0x0A0B0C;

        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Equal(2, p.Ops.Count);
        Assert.StartsWith("FillRect(", p.Ops[0]);
        Assert.Contains("0A0B0C", p.Ops[0]);
        Assert.StartsWith("TextRect(", p.Ops[1]);
    }

    /// <summary>
    /// 用例 19（差异断言）：DrawBorder 且**所选边框字体** Bold 且（按下或选中）时，
    /// 边框要内缩 1 像素再画一次 —— 共两次 FrameRect。
    ///
    /// 易错点：字体是**先按状态选定**、再判 Bold 的（226-241），
    /// 所以「按下」时判的是 **Down** 字体的 Bold，而不是 Up 的 —— 只把 Up 设 Bold 不会触发重画。
    /// </summary>
    [Fact]
    public void Paint_DrawBorderBoldDrawsInnerFrameTwice()
    {
        var c = NewLabel("A");
        c.DrawBorder = true;
        c.BorderColor.Down.Bold = true;

        // 差异断言：仅 Up 为 Bold 时（未按下）只有一次
        var p0 = new TDxRecordingPainter();
        c.PaintTo(p0);
        Assert.Single(p0.Ops.Where(o => o.StartsWith("FrameRect(")));

        // 按下 → 选中 Down 字体；Down.Bold=True 才触发内缩重画
        c.MouseDowned = true;
        var p2 = new TDxRecordingPainter();
        c.PaintTo(p2);
        var frames = p2.Ops.Where(o => o.StartsWith("FrameRect(")).ToList();
        Assert.Equal(2, frames.Count);
        // 第二次内缩 1 像素（注意 AutoSize 已把控件定尺到 6×12）：
        // (0,0,6,12) -> (1,1,5,11)
        Assert.Contains("FrameRect((1,1,5,11),(1,1,5,11)", frames[1]);
    }

    /// <summary>
    /// 用例 19b（差异断言）：按下时若只有 **Up** 字体 Bold、Down 字体不 Bold，
    /// 则**不会**内缩重画 —— 这正是「先按状态选字体、再判 Bold」造成的分支差异。
    /// </summary>
    [Fact]
    public void Paint_DrawBorderUpBoldDoesNotDoubleFrameWhenDownFontNotBold()
    {
        var c = NewLabel("A");
        c.DrawBorder = true;
        c.BorderColor.Up.Bold = true;         // 只设 Up
        c.MouseDowned = true;                 // 此时选中 Down（Bold=False）

        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Single(p.Ops.Where(o => o.StartsWith("FrameRect(")));
    }

    /// <summary>用例 20：OnPaint 回调在 DoPaint 之后、背景填充之前触发（167-170）。</summary>
    [Fact]
    public void Paint_OnPaintCallbackFiresBeforeBackground()
    {
        var c = NewLabel("A");
        c.Transparent = false;
        int order = 0;
        int cbOrder = 0;
        c.OnPaint = _ => cbOrder = ++order;

        var p = new TDxRecordingPainter();
        c.PaintTo(p);

        Assert.Equal(1, cbOrder);
    }

    // =====================================================================
    // InRange（245-260）
    // =====================================================================

    /// <summary>用例 21：命中域是 VisibleRect（含右/下边界）。</summary>
    [Fact]
    public void InRange_UsesVisibleRectWithClosedEdges()
    {
        var c = NewLabel();
        c.Left = 10; c.Top = 20; c.Width = 30; c.Height = 40;   // ClientRect (10,20,40,60)

        // 用 VirtualRectOverride 锁定虚拟区，避免 WinForms 父容器影响
        c.VirtualRectOverride = () => R(10, 20, 40, 60);

        Assert.True(c.InRange(10, 20));
        Assert.True(c.InRange(40, 60));      // 右下角命中（闭区间）
        Assert.False(c.InRange(41, 60));
        Assert.False(c.InRange(10, 61));
        Assert.False(c.InRange(9, 20));

        // 高度为 0（Bottom <= Top）时 VisibleRect 退化但仍覆盖同一条线上的点
        // （PointInRect 是闭区间），而**下一行**的点就不命中了
        c.VirtualRectOverride = () => R(10, 20, 40, 20);
        Assert.True(c.InRange(25, 20));
        Assert.False(c.InRange(25, 19));
        Assert.False(c.InRange(25, 21));
        Assert.False(c.InRange(9, 20));
        Assert.False(c.InRange(41, 20));
    }

    /// <summary>用例 22：OnInRealArea 收到的是**相对虚拟区**的坐标，且能把结果改判为 False。</summary>
    [Fact]
    public void InRange_OnInRealAreaCanVeto()
    {
        var c = NewLabel();
        c.Left = 10; c.Top = 20; c.Width = 30; c.Height = 40;

        int gotX = -1, gotY = -1;
        c.OnInRealArea = (sender, x, y, box) =>
        {
            gotX = x; gotY = y;
            box.Value = false;
        };

        Assert.False(c.InRange(15, 25));     // 在范围内但被改判
        Assert.Equal(5, gotX);               // 15 - 10
        Assert.Equal(5, gotY);               // 25 - 20
    }

    /// <summary>用例 23：不在范围内时**不**触发 OnInRealArea（250-259 的 else 分支）。</summary>
    [Fact]
    public void InRange_OutsideDoesNotInvokeCallback()
    {
        var c = NewLabel();
        c.Left = 10; c.Top = 20; c.Width = 30; c.Height = 40;

        bool called = false;
        c.OnInRealArea = (sender, x, y, box) => called = true;

        Assert.False(c.InRange(100, 100));
        Assert.False(called);
    }

    // =====================================================================
    // Assign（63-116）
    // =====================================================================

    /// <summary>用例 24：Assign 只对同类型生效，逐属性拷贝（含 RowSpacing/ExpandLineHeight）。</summary>
    [Fact]
    public void Assign_CopiesLabelProperties()
    {
        var src = NewLabel("Hello");
        src.AutoSize = false;                // 否则 Assign 里的 Caption 赋值会用自动定尺覆盖 Width/Height
        src.Left = 3; src.Top = 4; src.Width = 77; src.Height = 55;
        src.RowSpacing = 2;
        src.ExpandLineHeight = 6;
        src.CaptionDownOffsetX = 11;
        src.CaptionDownOffsetY = 12;
        src.ButtonDownOffsetX = 13;
        src.ButtonDownOffsetY = 14;
        src.Alignment = TDxAlignment.taRightJustify;
        src.DrawBorder = true;
        src.Checked = true;
        src.Transparent = false;
        src.ReferenceX = TReferenceX.rxCenter;
        src.AdjustYByHeight = true;
        src.TopAlignment = true;
        src.Center = true;
        src.Align = TDxAlign.alClient;
        src.HintText = "hi";
        src.Style = TButtonStyle.bsCheckBox;

        var dst = new TDxLabel();
        dst.Assign(src);

        Assert.Equal(3, dst.Left);
        Assert.Equal(4, dst.Top);
        Assert.Equal(77, dst.Width);
        Assert.Equal(55, dst.Height);
        Assert.Equal("Hello", dst.Caption);
        Assert.Equal(2, dst.RowSpacing);
        Assert.Equal(6, dst.ExpandLineHeight);
        Assert.Equal(11, dst.CaptionDownOffsetX);
        Assert.Equal(12, dst.CaptionDownOffsetY);
        Assert.Equal(13, dst.ButtonDownOffsetX);
        Assert.Equal(14, dst.ButtonDownOffsetY);
        Assert.Equal(TDxAlignment.taRightJustify, dst.Alignment);
        Assert.True(dst.DrawBorder);
        Assert.True(dst.Checked);
        Assert.False(dst.Transparent);
        Assert.Equal(TReferenceX.rxCenter, dst.ReferenceX);
        Assert.True(dst.AdjustYByHeight);
        Assert.True(dst.TopAlignment);
        Assert.True(dst.Center);
        Assert.Equal(TDxAlign.alClient, dst.Align);
        Assert.Equal("hi", dst.HintText);
        Assert.Equal(TButtonStyle.bsCheckBox, dst.Style);
    }

    /// <summary>用例 25（差异断言）：Assign 的源类型不匹配时**什么都不做**（65 的 `Source is TDxLabel`）。</summary>
    [Fact]
    public void Assign_IgnoresNonLabelSource()
    {
        var dst = NewLabel("keep");
        dst.Width = 1234;

        dst.Assign(new TDxLine());

        Assert.Equal(1234, dst.Width);
        Assert.Equal("keep", dst.Caption);
    }

    /// <summary>用例 26：Assign 会把 CaptionColor 一并拷过来（100）。</summary>
    [Fact]
    public void Assign_CopiesCaptionColor()
    {
        var src = NewLabel("A");
        src.CaptionColor.Up.Color = 0x0F0E0D;
        src.CaptionColor.Disabled.Bold = false;

        var dst = new TDxLabel();
        dst.Assign(src);

        Assert.Equal(0x0F0E0D, dst.CaptionColor.Up.Color);
        Assert.False(dst.CaptionColor.Disabled.Bold);
    }
}
