using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

/// <summary>
/// DXTrackBar.pas 1:1 逐字移植（1-336）。TDXTrackBar = class(TDxControl)。
///
/// 保真要点（原文的历史坑全部固化）：
///   * 构造（67-78）：AutoSize := True、Width := 100、Height := 20、FMin := 0、FMax := 10，
///     **FPosition 不在构造里赋值**（保持 0，与 FMin 相同）。
///   * SetMax（199-206）：`if FPosition &gt; FMax then FPosition := Max;` ——
///     这里用的是**函数 Max**（两个整数取最大值）而不是字段 FMax；原文如此，照抄。
///   * SetMin（208-215）：`if FPosition &lt; FMin then FPosition := FMin;`（正常夹紧）。
///   * SetPosition（217-231）：**先算夹紧值 V，再拿 V 与 FPosition 比，但赋值用的是原始 Value** ——
///     即 setter 其实**不夹紧**（V 只用于「是否变化」的判断）。原文缺陷，照抄并测死。
///   * Paint（92-197）：两条刻度绘制分支的 nW 公式**不同**：
///       ImageIndex.Down（进度条）分支：`nW := vRect.Right - vRect.Left;`（不减滑块宽）
///       滑块分支：`nW := vRect.Right - vRect.Left - TextureSlider.Width;`
///     且 `Step := nW / (FMax - FMin);` 是 **Single := Integer / Integer**，
///     在 Delphi 里是**整数除法**后再转 Single（二者分母正时等价于整数相除）。
///     nX := Round(Step * (FPosition - FMin))（银行家舍入）；nY := (高 - 滑块高) div 2。
///   * MouseDown（233-237）：`FIsDownSlider := FIsHotSlider;`（按下时继承当前悬停态）。
///   * MouseMove（239-312）：`if not FIsDownSlider then inherited;`（**没按下才走基类**）；
///     拖动分支里 FPosition 直接改写并回调 OnChanggingPosition；
///     悬停分支里更新 FIsHotSlider 后**立即 Exit**；若滑块图为 nil 则末尾 `FIsHotSlider := False`。
///   * MouseUp（314-321）：清按下态 + 回调 OnChangedPosition。
///   * DoMouseUp（328-334）：同样清态 + 回调（与 MouseUp 重复，原文如此）。
///   * CanMove（323-326）：`Result := not FIsDownSlider;`
///
/// 接缝：TextureSlider / TextureTick = ImageIndex 查表（TDxImageIndex + IDxImageLibrary）；
///       GameCanvas.Draw / TDxControl.DrawRect 走 IDxSurfacePainter。
/// </summary>
public sealed class TDXTrackBar : TDxControl
{
    private int _min;
    private int _max = 10;      // 构造 75：FMax := 10（FMin 构造里显式 := 0）
    private int _position;
    private bool _isHotSlider;
    private bool _isDownSlider;

    /// <summary>DXTrackBar.pas 24 FSliderIndex:TDxImageIndex。</summary>
    public TDxImageIndex SliderIndex { get; private set; }

    /// <summary>DXTrackBar.pas 32 FOnChanggingPosition（原文拼写，保留）。</summary>
    public Action<TDXTrackBar> OnChanggingPosition;

    /// <summary>DXTrackBar.pas 33 FOnChangedPosition。</summary>
    public Action<TDXTrackBar> OnChangedPosition;

    // DFM: TDXTrackBar 无同名 .dfm（DXTrackBar.dfm 在 Source/Client-HGE 全树不存在），
    // 布局属性的设计期取值落在各 .GUI 文件的 TguiTrackBar 记录里（见报告第 4 项）。

    public TDXTrackBar()
    {
        // DXTrackBar.pas 69-77 逐项
        AutoSize = true;
        Width = 100;
        Height = 20;

        _min = 0;
        _max = 10;

        SliderIndex = new TDxImageIndex();
    }

    /// <summary>DXTrackBar.pas 57 Min:Integer（默认 0）。</summary>
    public int Min
    {
        get => _min;
        set => SetMin(value);
    }

    /// <summary>DXTrackBar.pas 58 Max:Integer（默认 10）。</summary>
    public int Max
    {
        get => _max;
        set => SetMax(value);
    }

    /// <summary>DXTrackBar.pas 59 Position:Integer（默认 0）。</summary>
    public int Position
    {
        get => _position;
        set => SetPosition(value);
    }

    /// <summary>DXTrackBar.pas 29 FIsHotSlider（只读暴露，测试用）。</summary>
    public bool IsHotSlider => _isHotSlider;

    /// <summary>DXTrackBar.pas 30 FIsDownSlider（只读暴露，测试用）。</summary>
    public bool IsDownSlider => _isDownSlider;

    /// <summary>DXTrackBar.pas 199-206 SetMax 1:1（`FPosition := Max` 是 Delphi 的 Max 函数）。</summary>
    public void SetMax(int value)
    {
        if (_max != value)
        {
            _max = value;
            if (_position > _max)
                _position = Math.Max(_position, _max);   // 原文 FPosition := Max;（= Math.Max 语义）
        }
    }

    /// <summary>DXTrackBar.pas 208-215 SetMin 1:1。</summary>
    public void SetMin(int value)
    {
        if (_min != value)
        {
            _min = value;
            if (_position < _min)
                _position = _min;
        }
    }

    /// <summary>
    /// DXTrackBar.pas 217-231 SetPosition 1:1（**原文不夹紧**）。
    /// V 只参与「是否变化」判断：`if FPosition &lt;&gt; V then FPosition := Value;`
    /// —— 例如 Min=0/Max=10/Position=0 时写 999：V=10 ≠ 0 成立，但写入的是 999。
    /// </summary>
    public void SetPosition(int value)
    {
        int v;
        if (value < _min)
            v = _min;
        else if (value > _max)
            v = _max;
        else
            v = value;

        if (_position != v)
        {
            _position = value;    // 原文如此（不是 V）
        }
    }

    /// <summary>DXTrackBar.pas 86-90 SetOnGetImage 1:1（先 inherited 再挂到 FSliderIndex）。</summary>
    public void SetOnGetImage(Action<TDxImageIndex, TImageType> value)
    {
        base.SetOnGetImage(value);
        SliderIndex.SetOnGetImage(value);
    }

    /// <summary>DXTrackBar.pas 323-326 CanMove 1:1。</summary>
    public bool CanMove() => !_isDownSlider;

    /// <summary>DXTrackBar.pas 233-237 MouseDown 1:1。</summary>
    public override void MouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        base.MouseDown(button, shift, x, y);
        _isDownSlider = _isHotSlider;
    }

    /// <summary>DXTrackBar.pas 314-321 MouseUp 1:1。</summary>
    public override void MouseUp(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        base.MouseUp(button, shift, x, y);
        _isDownSlider = false;

        OnChangedPosition?.Invoke(this);
    }

    /// <summary>DXTrackBar.pas 328-334 DoMouseUp 1:1（与 MouseUp 重复的清态 + 回调）。</summary>
    protected override void DoMouseUp()
    {
        base.DoMouseUp();
        _isDownSlider = false;
        OnChangedPosition?.Invoke(this);
    }

    /// <summary>
    /// DXTrackBar.pas 239-312 MouseMove 1:1 的状态迁移部分。
    /// 返回值 = 是否走到了「拖动分支」（即真的改了 FPosition）。
    /// </summary>
    public bool MouseMoveTo(TDxShiftState shift, int x, int y)
    {
        if (!_isDownSlider)
            base.MouseMove(shift, x, y);

        if (SliderIndex.Image != null)
        {
            var vbRect = VisibleRect;
            if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return false;
            var vtRect = VirtualRect;

            int faceIndex = SliderIndex.Up;
            if (faceIndex >= 0)
            {
                IDxTexture textureSlider;
                if (Enabled)
                    textureSlider = SliderIndex.Image.GetImage(faceIndex);
                else
                    textureSlider = SliderIndex.Image.GetGray(faceIndex);

                if (textureSlider != null)
                {
                    var vRect = vtRect;

                    int nW = textureSlider != null
                        ? vRect.Right - vRect.Left - textureSlider.Width
                        : vRect.Right - vRect.Left;

                    if (_max - _min > 0)
                    {
                        float step = ComputeStep(nW, _max - _min);

                        if (_isDownSlider)
                        {
                            int nPos = ComputeDragPosition(x - vtRect.Left, step) + _min;
                            if (nPos < _min)
                                nPos = _min;
                            else if (nPos > _max)
                                nPos = _max;

                            _position = nPos;

                            OnChanggingPosition?.Invoke(this);
                            return true;
                        }
                        else
                        {
                            int nX = ComputePixelPosition(step, _position - _min);
                            int nH = textureSlider.Height;

                            int nY = (vRect.Bottom - vRect.Top - nH) / 2;

                            // vtRect := vRect; 然后按 nX/nY 摆滑块矩形（原文 296-300）
                            var sliderRect = vRect;
                            sliderRect.Left = vRect.Left + nX;
                            sliderRect.Top = sliderRect.Top + nY;
                            sliderRect.Right = sliderRect.Left + textureSlider.Width;
                            sliderRect.Bottom = sliderRect.Top + textureSlider.Height;

                            _isHotSlider = DxRectUtil.PointInRect(new TDxPoint(x, y), sliderRect);
                            return false;   // 原文此处 Exit
                        }
                    }
                }
            }
        }

        _isHotSlider = false;
        return false;
    }

    /// <summary>
    /// DXTrackBar.pas 170/188/276 的 `Step := nW / (FMax - FMin);`。
    /// Delphi 的 `/` 是浮点除法、`Step:Single`；但 nW 与分母都是 Integer，
    /// 故原文这里**先做整数除法**再隐式转 Single（等价于 `(float)(nW / span)`，向零截断）。
    /// 本方法如实表达该截断（若按浮点除法则 0.5 之类的小步长会被放大一倍）。
    /// </summary>
    public static float ComputeStep(int nW, int span)
        => span == 0 ? 0f : (float)(nW / span);

    /// <summary>DXTrackBar.pas 171/189/291 的 `nX := Round(Step * (FPosition - FMin));`。</summary>
    public static int ComputePixelPosition(float step, int positionOffset)
        => TDxCanvasColorTables.DelphiRound(step * positionOffset);

    /// <summary>
    /// DXTrackBar.pas 279 的 `nPos := Round((X - vtRect.Left) / Step) + FMin;`
    /// Step 为 0 时原文是浮点除零 → ±Inf，Round 后溢出；此处返回 int.MaxValue 让外层夹紧到 FMax。
    /// </summary>
    public static int ComputeDragPosition(int xOffset, float step)
    {
        if (step == 0f) return int.MaxValue;
        return TDxCanvasColorTables.DelphiRound(xOffset / (double)step);
    }

    /// <summary>
    /// DXTrackBar.pas 92-197 Paint 1:1（绘制请求序列）。
    /// 两条分支的 nW 公式**不同**（见类注释），因此进度条与滑块在同一点上的像素位置也不同。
    /// </summary>
    public TDXTrackBarPaintResult PaintTo(IDxSurfacePainter painter)
    {
        var result = new TDXTrackBarPaintResult();

        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return result;
        var vtRect = VirtualRect;

        Painter = painter ?? Painter;
        DoPaint();

        if (Designing)
        {
            Painter.FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
            result.DrewDesignFrame = true;
        }

        IDxTexture textureSlider = null;

        if (OnPaint != null)
        {
            OnPaint(this);
            result.UsedOnPaintOverride = true;
        }
        else
        {
            if (SliderIndex.Image != null)
            {
                int faceIndex = SliderIndex.Up;

                if (faceIndex >= 0)
                {
                    textureSlider = Enabled
                        ? SliderIndex.Image.GetImage(faceIndex)
                        : SliderIndex.Image.GetGray(faceIndex);
                }
            }

            if (ImageIndex.Image != null)
            {
                int faceIndex = ImageIndex.Up;

                if (faceIndex >= 0)
                {
                    var textureTick = Enabled
                        ? ImageIndex.Image.GetImage(faceIndex)
                        : ImageIndex.Image.GetGray(faceIndex);

                    if (textureTick != null)
                    {
                        var vRect = vtRect;
                        // 原文 147：DrawRect(vRect, vtRect, vbRect, TextureTick, BlendMode)
                        Painter.DrawRect(vRect, vtRect, vbRect, textureTick, BlendMode);
                        result.DrewUpTick = true;
                    }
                }

                faceIndex = ImageIndex.Down;
                if (faceIndex >= 0)
                {
                    var textureTick = Enabled
                        ? ImageIndex.Image.GetImage(faceIndex)
                        : ImageIndex.Image.GetGray(faceIndex);

                    if (textureTick != null)
                    {
                        var vRect = vtRect;

                        // 原文 161-166 的滑块宽扣减被注释掉，实际取整宽（168）
                        int nW = vRect.Right - vRect.Left;
                        if (_max - _min > 0)
                        {
                            float step = ComputeStep(nW, _max - _min);
                            int nX = ComputePixelPosition(step, _position - _min);
                            vbRect.Right = vbRect.Left + nX;

                            Painter.DrawRect(vRect, vtRect, vbRect, textureTick, BlendMode);
                            result.DrewDownTick = true;
                            result.TickClipRight = vbRect.Right;
                        }
                    }
                }
            }

            if (textureSlider != null)
            {
                var vRect = vtRect;
                int nW = textureSlider != null
                    ? vRect.Right - vRect.Left - textureSlider.Width
                    : vRect.Right - vRect.Left;

                if (_max - _min > 0)
                {
                    float step = ComputeStep(nW, _max - _min);
                    int nX = ComputePixelPosition(step, _position - _min);
                    int nH = textureSlider.Height;

                    int nY = (vRect.Bottom - vRect.Top - nH) / 2;
                    Painter.Draw(vRect.Left + nX, vRect.Top + nY, textureSlider, BlendMode);
                    result.SliderX = vRect.Left + nX;
                    result.SliderY = vRect.Top + nY;
                    result.DrewSlider = true;
                }
            }
        }

        return result;
    }
}

/// <summary>TDXTrackBar.PaintTo 的绘制结果（headless 断言用）。</summary>
public sealed class TDXTrackBarPaintResult
{
    public bool DrewUpTick;
    public bool DrewDownTick;
    public bool DrewSlider;
    public bool UsedOnPaintOverride;
    public bool DrewDesignFrame;
    public int TickClipRight;
    public int SliderX;
    public int SliderY;
}
