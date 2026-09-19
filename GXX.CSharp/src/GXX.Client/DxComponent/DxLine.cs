using System;

namespace GXX.Client.DxComponent;

/// <summary>
/// DxLine.pas 1:1 逐字移植（1-134）。TDxLine = class(TDxControl)。
///
/// 保真要点：
///   * 构造（37-51）：**AutoSize := False**（覆盖基类默认 True）、Floating := False、
///     DrawBorder := False、Width := 200、Height := 12、FLineStyle := lsNone、
///     FLineColor 四态全部 clGray。
///   * SetStyle（59-69）：整段交换宽高的代码体原文**被注释掉**，故实际只有
///     `if FLineStyle &lt;&gt; Value then FLineStyle := Value;`（不改 Width/Height）——原文如此。
///   * Paint（88-132）：可见/虚拟守卫 → DoPaint → Designing 时 FrameRect(BorderColor.Up.Color)
///     → 取 FLineColor 状态 → `if Font &lt;&gt; nil` 后按 FLineStyle 分四种画法：
///       lsHorizontal：y = vt.Top + (Height - 1) div 2，从 vt.Left 到 vt.Right
///       lsVertical  ：x = vt.Left + (Width - 1) div 2，从 vt.Top 到 vt.Bottom
///       lsCircle    ：圆心 (vt.Left + Width div 2, vt.Top + Height div 2)，
///                     半径 Max(Width div 2, Height div 2)
///       lsTriangle  ：Pt1 = (vt.Left + (Width-1) div 2, vt.Top)
///                     Pt2 = (vt.Left, vt.Bottom)、Pt3 = (vt.Right, vt.Bottom)，画三条边
///     注意：**lsNone 什么都不画**；几何用的是 Width/Height 字段（不是 vtRect 的宽高）。
///   * InRange（71-86）：与 TDxLabel.InRange 同形（VisibleRect 命中 + OnInRealArea 改判）。
///
/// 接缝：GameCanvas.Line / GameCanvas.Circle（HGECanvas 未移植）→ IDxSurfacePainter。
/// </summary>
public sealed class TDxLine : TDxControl
{
    private TLineStyle _lineStyle;

    /// <summary>DxLine.pas 30 / 59-69 Style（默认 lsNone）。</summary>
    public TLineStyle Style
    {
        get => _lineStyle;
        set
        {
            if (_lineStyle != value)
            {
                // 原文 62-66：设计期把水平/垂直互换宽高的代码整段被注释掉 —— 照抄（不做）。
                _lineStyle = value;
            }
        }
    }

    /// <summary>DxLine.pas 31 LineColor（TDxBorderColor，四态 clGray）。</summary>
    public TDxBorderColor LineColor { get; private set; }

    // DFM: TDxLine 无同名 .dfm（DxLine.dfm 在 Source/Client-HGE 全树不存在），
    // 布局属性的设计期取值落在各 .GUI 文件的 TguiLine 记录里（见报告第 4 项）。

    public TDxLine()
    {
        // DxLine.pas 39-50 逐项
        AutoSize = false;
        Floating = false;
        DrawBorder = false;
        Width = 200;
        Height = 12;
        _lineStyle = TLineStyle.lsNone;
        LineColor = new TDxBorderColor();
        // 原文 47-50 是对 FLineColor 四态字段的**直写**（不触发 OnChange）。
        LineColor.Up.Color = TDxColor.clGray;
        LineColor.Hot.Color = TDxColor.clGray;
        LineColor.Down.Color = TDxColor.clGray;
        LineColor.Disabled.Color = TDxColor.clGray;
    }

    /// <summary>
    /// DxLine.pas 88-132 Paint 1:1。
    /// 返回本次实际请求的绘制操作数（原文无返回值，仅为 headless 断言方便）。
    /// </summary>
    public int PaintTo(IDxSurfacePainter painter)
    {
        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return 0;
        var vtRect = VirtualRect;

        Painter = painter ?? Painter;
        DoPaint();

        int ops = 0;

        if (Designing)
        {
            // 原文 102 是注释掉的 FillRectAlpha；只留 FrameRect（103）
            Painter.FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
            ops++;
        }

        var font = SelectLineFont();
        if (font != null)
        {
            switch (_lineStyle)
            {
                case TLineStyle.lsHorizontal:
                    Painter.Line(
                        new TDxPoint(vtRect.Left, vtRect.Top + (Height - 1) / 2),
                        new TDxPoint(vtRect.Right, vtRect.Top + (Height - 1) / 2),
                        font.Color);
                    ops++;
                    break;
                case TLineStyle.lsVertical:
                    Painter.Line(
                        new TDxPoint(vtRect.Left + (Width - 1) / 2, vtRect.Top),
                        new TDxPoint(vtRect.Left + (Width - 1) / 2, vtRect.Bottom),
                        font.Color);
                    ops++;
                    break;
                case TLineStyle.lsCircle:
                    Painter.Circle(
                        vtRect.Left + Width / 2,
                        vtRect.Top + Height / 2,
                        Math.Max(Width / 2, Height / 2),
                        font.Color);
                    ops++;
                    break;
                case TLineStyle.lsTriangle:
                    {
                        var pt1 = new TDxPoint(vtRect.Left + (Width - 1) / 2, vtRect.Top);
                        var pt2 = new TDxPoint(vtRect.Left, vtRect.Bottom);
                        var pt3 = new TDxPoint(vtRect.Right, vtRect.Bottom);
                        Painter.Line(pt1, pt2, font.Color);
                        Painter.Line(pt1, pt3, font.Color);
                        Painter.Line(pt2, pt3, font.Color);
                        ops += 3;
                        break;
                    }
                default:
                    // lsNone：什么都不画
                    break;
            }
        }

        return ops;
    }

    /// <summary>DxLine.pas 106-116 的字体状态选择 1:1。</summary>
    private TDxFont SelectLineFont()
    {
        if (Enabled)
        {
            if (MouseDowned)
                return LineColor.Down;
            if (MouseMoveed)
                return LineColor.Hot;
            return LineColor.Up;
        }
        return LineColor.Disabled;
    }

    /// <summary>
    /// DxLine.pas 71-86 InRange 1:1。
    /// **与 TDxControl 基类 InRange 的差异**：本覆写不做透明纹理逐像素判定，只看 VisibleRect。
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
}
