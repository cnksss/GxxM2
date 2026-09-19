using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

/// <summary>
/// DxImageButton.pas 94-... TDxImageButton 的**最小接缝面**（只保留 TDxLabel 依赖到的部分）。
///
/// 依据 DxImageButton.pas 466-489（构造函数）：
///   AutoSize := True; Width := 100; Height := 20; Alignment := taCenter;
///   FClickSound := csNone; FButtonStyle := bsButton; FChecked := False;
///   FCaptionColor := TDxCaptionColor.Create; FCaptionDownOffsetX := 1; FCaptionDownOffsetY := 1;
///   FButtonDownOffsetX := 0; FButtonDownOffsetY := 0; FDrawAligment := daFill;
///   FCaptionOffsetX := 0; FCaptionOffsetY := 0; FExpandWidth := 0; GuiType := t_Button;
///
/// 接缝：TDxImageButton 的图片绘制、动画（TButtonAnimation）、按键音、PopupMenu 弹出等
/// 未在本车道移植；待 DxImageButton.pas 归属批次移植后由该类型接管。
/// </summary>
public class TDxImageButton : TDxControl
{
    /// <summary>DxImageButton.pas 46-54 TClickSound。</summary>
    public enum TClickSound { csNone, csStone, csGlass, csNorm }

    /// <summary>DxImageButton.pas FButtonStyle（默认 bsButton）。</summary>
    public TButtonStyle Style { get; set; } = TButtonStyle.bsButton;

    /// <summary>DxImageButton.pas FChecked（原文 104：Checked := False）。</summary>
    public bool ButtonChecked { get => Checked; set => Checked = value; }

    /// <summary>DxImageButton.pas FCaptionColor。</summary>
    public TDxCaptionColor CaptionColor { get; protected set; }

    /// <summary>DxImageButton.pas FCaptionDownOffsetX（默认 1）。</summary>
    public int CaptionDownOffsetX { get; set; } = 1;

    /// <summary>DxImageButton.pas FCaptionDownOffsetY（默认 1）。</summary>
    public int CaptionDownOffsetY { get; set; } = 1;

    /// <summary>DxImageButton.pas FButtonDownOffsetX（默认 0）。</summary>
    public int ButtonDownOffsetX { get; set; }

    /// <summary>DxImageButton.pas FButtonDownOffsetY（默认 0）。</summary>
    public int ButtonDownOffsetY { get; set; }

    /// <summary>DxImageButton.pas FCaptionOffsetX（默认 0）。</summary>
    public int CaptionOffsetX { get; set; }

    /// <summary>DxImageButton.pas FCaptionOffsetY（默认 0）。</summary>
    public int CaptionOffsetY { get; set; }

    /// <summary>DxImageButton.pas FExpandWidth（默认 0）。</summary>
    public int ExpandWidth { get; set; }

    /// <summary>DxImageButton.pas FClickSound（默认 csNone）。</summary>
    public TClickSound ClickSound { get; set; } = TClickSound.csNone;

    /// <summary>DxImageButton.pas FDrawAligment（TDrawAligment，默认 daFill = 0）。</summary>
    public int DrawAligment { get; set; }

    /// <summary>接缝：TDxImageButton.CheckAutoSize（504-509）—— 仅 Style = bsButton 时才走基类。</summary>
    protected override void CheckAutoSizeBase()
    {
        if (Style == TButtonStyle.bsButton)
            base.CheckAutoSizeBase();
    }

    public TDxImageButton()
    {
        // DxImageButton.pas 471-488 逐项（注意 Alignment 被覆盖为 taCenter）
        AutoSize = true;
        Width = 100;
        Height = 20;
        Alignment = TDxAlignment.taCenter;
        ClickSound = TClickSound.csNone;
        Style = TButtonStyle.bsButton;
        Checked = false;
        CaptionColor = new TDxCaptionColor();
        CaptionDownOffsetX = 1;
        CaptionDownOffsetY = 1;
        ButtonDownOffsetX = 0;
        ButtonDownOffsetY = 0;
        DrawAligment = 0;   // daFill
        CaptionOffsetX = 0;
        CaptionOffsetY = 0;
        ExpandWidth = 0;
        GuiType = TGuiType.t_Button;
    }

    /// <summary>DxImageButton.pas 488：GuiType := t_Button。</summary>
    public TGuiType GuiType { get; set; } = TGuiType.t_None;

    /// <summary>
    /// DxImageButton.pas 511-... MouseDown：原文先处理 PopupMenu 定位再走基类。
    /// TDxPopupMenu 未移植 → 交由基类接缝。
    /// </summary>
    public override void MouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        base.MouseDown(button, shift, x, y);
    }
}

/// <summary>
/// DxLabel.pas 1:1 逐字移植（1-262）。TDxLabel = class(TDxImageButton)。
///
/// 保真要点：
///   * 构造默认值（41-53）：Transparent=True、AutoSize=True、Caption=Name（本移植 Name 默认空串）、
///     Width=32、Height=16、CaptionDownOffsetX/Y=0、FRowSpacing=0、FExpandLineHeight=0。
///   * SetRowSpacing（55-61）：值变化才赋值并 DoCaptionChange。
///   * DoCaptionChange（118-152）：**AutoSize 为假时连字体都不查**（整段包在 `if AutoSize then` 里）；
///     行高统一取 `HGEFont.TextHeight('0') + FRowSpacing`（**与 Paint 的取值公式不同**，
///     Paint 用每行 TextImages[I].Height + FExpandLineHeight）；Bold 时宽高各 +2。
///   * Paint（154-243）：可见/虚拟矩形守卫 → DoPaint → OnPaint 回调 → 非 Transparent 时 FillRect
///     背景 → 取字体状态 → **`if Style &lt;&gt; bsButton then begin X := 0; Y := 0; end;`** →
///     逐行 `if TextImages[I].Height &lt;= 0 then Inc(nHeight, HGEFont.TextHeight('0') + FExpandLineHeight)
///     else Inc(nHeight, TextImages[I].Height + FExpandLineHeight)` → Bold +2 →
///     DrawCaption → 边框（Bold 且按下/选中时再内缩 1 像素重画一次）。
///   * InRange（245-260）：点不在 VisibleRect 内直接 False；在则给 OnInRealArea 一次改判机会。
/// </summary>
public sealed class TDxLabel : TDxImageButton
{
    private int _rowSpacing;
    private int _expandLineHeight;

    /// <summary>DxLabel.pas 19 FRowSpacing。</summary>
    public int RowSpacing
    {
        get => _rowSpacing;
        set
        {
            if (_rowSpacing != value)
            {
                _rowSpacing = value;
                DoCaptionChange();
            }
        }
    }

    /// <summary>DxLabel.pas 20 FExpandLineHeight（published 直写字段，无 setter）。</summary>
    public int ExpandLineHeight { get => _expandLineHeight; set => _expandLineHeight = value; }

    // DFM: TDxLabel 无同名 .dfm（DxLabel.dfm 在 Source/Client-HGE 全树不存在），
    // 布局属性的设计期取值落在各 .GUI 文件的 TguiLabel 记录里（见报告第 4 项）。

    public TDxLabel()
    {
        // DxLabel.pas 43-52 逐项
        Transparent = true;
        AutoSize = true;
        Caption = "";             // 原文 Caption := Name（Name 默认空串）
        Width = 32;
        Height = 16;
        CaptionDownOffsetX = 0;
        CaptionDownOffsetY = 0;
        _rowSpacing = 0;
        _expandLineHeight = 0;
    }

    /// <summary>DxLabel.pas 118-152 DoCaptionChange 1:1。</summary>
    protected override void DoCaptionChange()
    {
        // 原文 124-126：先查字体；字体为 nil 时**整段不执行**（连 AutoSize 都不看）。
        var hgeFont = FindFont(CaptionColor.Up);
        if (hgeFont == null) return;

        // 原文 127：TextImages 在 AutoSize 判断之前就取了（125-127 顺序）。
        var textImages = GetImageInfos(hgeFont, Caption);

        if (AutoSize)
        {
            int nWidth = 0;
            int nHeight = 0;
            // 原文 132：行高 = TextHeight('0') + FRowSpacing（与 Paint 的公式不同）
            int lineHeight = TextHeight(hgeFont, "0") + _rowSpacing;
            for (int i = 0; i <= textImages.Count - 1; i++)
            {
                if (nWidth < textImages[i].Width)
                    nWidth = textImages[i].Width;
                nHeight += lineHeight;
            }

            if (CaptionColor.Up.Bold)
            {
                nWidth += 2;
                nHeight += 2;
            }
            Width = nWidth;
            Height = nHeight;
        }
    }

    /// <summary>
    /// DxLabel.pas 154-243 Paint 1:1。
    /// 返回本次绘制用到的（宽, 高）计算值，供 headless 断言（原文以局部变量存在）。
    /// </summary>
    public TDxPoint PaintTo(IDxSurfacePainter painter)
    {
        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return new TDxPoint(0, 0);
        var vtRect = VirtualRect;

        Painter = painter ?? Painter;
        DoPaint();

        // 原文 169-170：OnPaint 在 DoPaint 之后、背景填充之前
        OnPaint?.Invoke(this);

        if (!Transparent)
            Painter.FillRect(vtRect, vtRect, vbRect, BackgroundColor);

        int x = 0;
        int y = 0;
        var font = SelectCaptionFont();
        if (Style != TButtonStyle.bsButton)
        {
            x = 0;
            y = 0;
        }

        int nWidth = 0;
        int nHeight = 0;
        if (Caption != "")
        {
            var hgeFont = FindFont(font);

            if (hgeFont != null)
            {
                var textImages = GetImageInfos(hgeFont, Caption);

                nWidth = 0;
                nHeight = 0;
                for (int i = 0; i <= textImages.Count - 1; i++)
                {
                    if (nWidth < textImages[i].Width)
                        nWidth = textImages[i].Width;

                    if (textImages[i].Height <= 0)
                        nHeight += TextHeight(hgeFont, "0") + _expandLineHeight;
                    else
                        nHeight += textImages[i].Height + _expandLineHeight;
                }

                if (font.Bold)
                {
                    nWidth += 2;
                    nHeight += 2;
                }

                DrawCaption(hgeFont, font, textImages,
                    TDxRect.Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, x, y, _expandLineHeight);
            }
        }

        if (DrawBorder)
        {
            var borderFont = SelectBorderFont();
            Painter.FrameRect(vtRect, vtRect, vbRect, borderFont.Color);
            if (borderFont.Bold && (MouseDowned || Checked))
            {
                vtRect = DxRectUtil.ShrinkRect(vtRect, 1, 1);
                Painter.FrameRect(vtRect, vtRect, vbRect, borderFont.Color);
            }
        }

        return new TDxPoint(nWidth, nHeight);
    }

    /// <summary>
    /// DxLabel.pas 175-189 的字体状态选择 1:1。
    /// 原文此处 `Font` 无 else 兜底（四个状态在 TDxCaptionColor 构造里必非 nil）——
    /// 本移植给 Disabled 兜底以避免 null 解引用，语义分支与原文一致。
    /// </summary>
    private TDxFont SelectCaptionFont()
    {
        if (Enabled)
        {
            if (MouseDowned || Checked)
                return CaptionColor.Down;
            if (MouseMoveed)
                return CaptionColor.Hot;
            return CaptionColor.Up;
        }
        return CaptionColor.Disabled;
    }

    /// <summary>DxLabel.pas 226-236 的边框颜色状态选择 1:1。</summary>
    private TDxFont SelectBorderFont()
    {
        if (Enabled)
        {
            if (MouseDowned || Checked)
                return BorderColor.Down;
            if (MouseMoveed)
                return BorderColor.Hot;
            return BorderColor.Up;
        }
        return BorderColor.Disabled;
    }

    /// <summary>
    /// DxLabel.pas 245-260 InRange 1:1。
    /// 命中域是 **VisibleRect**（不是 ClientRect），且 OnInRealArea 可以把结果改回 False。
    /// </summary>
    public override bool InRange(int x, int y)
    {
        if (DxRectUtil.PointInRect(new TDxPoint(x, y), VisibleRect))
        {
            bool boInrange = true;
            if (OnInRealArea != null)
            {
                var vRect = VirtualRect;
                var box = new BoolRef(boInrange);
                OnInRealArea(this, x - vRect.Left, y - vRect.Top, box);
                boInrange = box.Value;
            }
            return boInrange;
        }
        return false;
    }

    /// <summary>DxLabel.pas 63-116 Assign（同类型才拷贝，逐属性照抄顺序）。</summary>
    public void Assign(TDxControl source)
    {
        if (source is TDxLabel src)
        {
            Left = src.Left;
            Top = src.Top;
            Width = src.Width;
            Height = src.Height;
            Enabled = src.Enabled;
            Visible = src.Visible;

            Transparent = src.Transparent;
            EnableFocus = src.EnableFocus;
            Floating = src.Floating;
            OwnerMove = src.OwnerMove;

            MouseEvents = src.MouseEvents;

            ReferenceX = src.ReferenceX;
            AdjustYByHeight = src.AdjustYByHeight;
            TopAlignment = src.TopAlignment;

            Center = src.Center;
            Align = src.Align;
            AutoSize = src.AutoSize;
            BackgroundColor = src.BackgroundColor;
            BlendMode = src.BlendMode;

            ImageIndex.Assign(src.ImageIndex);
            BorderColor.Assign(src.BorderColor);

            ButtonDownOffsetX = src.ButtonDownOffsetX;
            ButtonDownOffsetY = src.ButtonDownOffsetY;

            Caption = src.Caption;
            Alignment = src.Alignment;

            CaptionColor.Assign(src.CaptionColor);
            CaptionDownOffsetX = src.CaptionDownOffsetX;
            CaptionDownOffsetY = src.CaptionDownOffsetY;

            Checked = src.Checked;
            DrawBorder = src.DrawBorder;
            HintText = src.HintText;
            Style = src.Style;

            RowSpacing = src.RowSpacing;
            ExpandLineHeight = src.ExpandLineHeight;
        }
    }
}
