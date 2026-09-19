using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GXX.Client.DxComponent;

// =====================================================================================
// DxImageForm.pas（1,263 行）1:1 逐字移植。
//
//   * 21-82    TDxFormShapeImage（形状贴图：源/目标矩形、对齐、拉伸、居中）
//   * 86-154   TAnimation（窗体级动画：帧推进 + 裁剪绘制）
//   * 156-218  TDxImageForm（图片窗体基类）
//   * 220-251  TDxImageFormShape（8 张形状贴图的窗体）
//   * 253-1263 实现段
//
// DFM: TDxImageForm / TDxImageFormShape 无同名 .dfm（Source\Client-HGE 全树 glob 核实，0 命中），
//      设计期取值落在各 .GUI 文件的 Tgui* 记录里 —— 不按 .dfm 对齐。
//
// -------------------------------------------------------------------------------------
// 托管侧落点与偏差（逐条登记，均不改上游接缝）：
//   1. TInterfacedPersistent（引用计数 Persistent）→ 普通 class + IDisposable 手动释放；
//      TAnimation 与 TDxFormShapeImage 由宿主 TDxImageForm/TDxImageFormShape 持有并释放。
//   2. TGameImages（WIL 图库）→ 复用上游接缝 IDxImageLibrary；原文的
//      `FImage.Images[i]` / `FImage.GetCachedImage(i, nX, nY)` / `FImage.Grays[i]` 一一对应到
//      IDxImageLibrary.GetImage(i) / （GetCachedImage 的等价：GetImage + 偏移 0） / GetGray(i)。
//      说明：上游 IDxImageLibrary 没有 GetCachedImage（它自带裁剪偏移 nX/nY），
//      本移植以 GetImage 承接并把 nX/nY 取 0（原文 GetCachedImage 在已缓存命中时 nX=nY=0，
//      未命中时是缓存纹理内的偏移 —— 该偏移属 GameImages 内部实现，属未移植单元）。
//   3. GameCanvas.Draw / StretchDraw / FillRectAlpha → IDxSurfacePainter（+IDxSurfacePainterExt）。
//   4. TMsg/PeekMessage/TranslateMessage/DispatchMessage 消息泵（670-751）在本波**不移植**：
//      它是 Win32 消息循环，属未移植的 HGE/Forms 层；ShowModal* 的循环保留原文的
//      「Visible=False 或 Application.Terminated 才退出 + Sleep(1)」骨架，消息泵以可注入委托承接
//      （缺省 Application.DoEvents）。原文的 IsKeyMsg / CN_BASE 转发不在本波范围，已在报告登记。
//   5. TModalResult → int，常量 mrNone = 0（原文 VCL Controls.mrNone）。
// =====================================================================================

/// <summary>原文 VCL Controls.TModalResult（本波只用 mrNone）。</summary>
public static class TDxModalResult
{
    /// <summary>VCL mrNone = 0。</summary>
    public const int mrNone = 0;
}

/// <summary>原文 HGE.Blend_Default（= HGE.BLEND_DEFAULT = 0）。</summary>
public static class TDxBlendMode
{
    public const int Blend_Default = 0;
}

/// <summary>DxComponents.pas 88 TAlignEx。</summary>
public enum TAlignEx
{
    alxNone, alxTop, alxBottom, alxLeft, alxRight, alxClient,
    alxTopLeft, alxTopRight, alxBottomLeft, alxBottomRight,
}

// -------------------------------------------------------------------------------------
// TDxImageForm.pas 21-82：TDxFormShapeImage
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageForm.pas 21-82 / 852-1064 TDxFormShapeImage：
/// 一张带「源矩形 / 目标矩形 / 对齐 / 拉伸 / 居中」的形状贴图。
/// </summary>
public class TDxFormShapeImage : IDisposable
{
    private int _align2;                       // FAlign（TAlignEx）
    private TImageType _imageType = TImageType.Prguse_wil;
    private int _imageIndex = -1;
    private bool _draw;
    private bool _stretch;
    private bool _center;
    private TDxRect _sourceRect = TDxRect.Rect(0, 0, 0, 0);
    private TDxRect _destRect = TDxRect.Rect(0, 0, 0, 0);
    private int _blendMode = TDxBlendMode.Blend_Default;
    private bool _autoDestRectSize;

    /// <summary>原文 FOnChange（TNotifyEvent）。</summary>
    public Action<TDxFormShapeImage> OnChange;

    /// <summary>原文 FOnGetImage（TOnGetImage：Sender, ImageType, var AImage）。</summary>
    public Action<TDxFormShapeImage, TImageType, object> OnGetImage;

    /// <summary>原文 FImage（TGameImages）→ 接缝 IDxImageLibrary。</summary>
    public IDxImageLibrary Image;

    public TDxFormShapeImage()
    {
        // 原文 1009-1023 构造默认值逐项
        _align2 = (int)TAlignEx.alxNone;
        Image = null;
        _imageType = TImageType.Prguse_wil;
        _imageIndex = -1;
        _draw = false;
        _stretch = false;
        _center = false;
        _autoDestRectSize = false;
        _sourceRect = TDxRect.Rect(0, 0, 0, 0);
        _destRect = TDxRect.Rect(0, 0, 0, 0);
        _blendMode = TDxBlendMode.Blend_Default;
    }

    /// <summary>原文 1025-1028 TDxFormShapeImage.Destroy（无自身释放动作）。</summary>
    public void Dispose() { }

    /// <summary>原文 1003-1007 TDxFormShapeImage.Changed。</summary>
    public void Changed() => OnChange?.Invoke(this);

    /// <summary>原文 62-71 published 属性 ImageType。</summary>
    public TImageType ImageType
    {
        get => _imageType;
        set
        {
            if (_imageType == value) return;
            _imageType = value;                       // 原文 868-876：先赋值再回调 OnGetImage，最后 Changed
            if (OnGetImage != null) OnGetImage(this, _imageType, Image);
            Changed();
        }
    }

    /// <summary>原文 878-903 TDxFormShapeImage.SetImageIndex（含 SourceRect/DestRect 自动推导）。</summary>
    public int ImageIndex
    {
        get => _imageIndex;
        set
        {
            if (_imageIndex == value) return;
            _imageIndex = value;
            ApplyImageIndexGeometry(this);
            Changed();
        }
    }

    /// <summary>
    /// 原文 878-903 / 1078-1101 的公共几何推导（两处**逐字相同** —— 原文在
    /// TDxFormShapeImage.SetImageIndex 与 TDxImageFormShape.ImageIndexChange 各写了一遍）：
    /// 取图 → `Width*Height > 4` 才动作 → `SourceRect := Texture.ClientRect`；
    /// 随后那个「四边皆为 0 就再取一次并同时设 DestRect」的判定**永不成立**
    /// （因为上一行刚把 SourceRect 赋成 ClientRect）—— 原文冗余，逐字保留；
    /// 最后 AutoDestRectSize 时按纹理宽高重设 DestRect 的宽高（保留原 Left/Top）。
    /// </summary>
    public static void ApplyImageIndexGeometry(TDxFormShapeImage shapeImage)
    {
        if (shapeImage.Image == null) return;
        int nIndex = shapeImage._imageIndex;
        if (nIndex < 0) return;

        var texture = shapeImage.Image.GetImage(nIndex);
        if (texture == null || texture.Width * texture.Height <= 4) return;

        // 原文 890-891（被注释掉的那行照抄留档）：
        //   // FSourceRect := ShortRect(FSourceRect, Texture.ClientRect);
        shapeImage._sourceRect = texture.ClientRect;

        // 原文 892-895：这个判定在 891 之后**恒为 false**（SourceRect 已等于 ClientRect，
        // 除非纹理本身宽高为 0 —— 但上面已用 Width*Height > 4 排除）。原文冗余，逐字保留。
        if (shapeImage._sourceRect.Left == 0 && shapeImage._sourceRect.Top == 0
            && shapeImage._sourceRect.Right == 0 && shapeImage._sourceRect.Bottom == 0)
        {
            shapeImage._sourceRect = texture.ClientRect;
            shapeImage._destRect = texture.ClientRect;
        }

        if (shapeImage._autoDestRectSize)
            shapeImage._destRect = TDxRect.Bounds(shapeImage._destRect.Left, shapeImage._destRect.Top,
                texture.Width, texture.Height);
    }

    /// <summary>原文 66 published 属性 Align（TAlignEx）。</summary>
    public TAlignEx Align
    {
        get => (TAlignEx)_align2;
        set
        {
            if (_align2 == (int)value) return;
            _align2 = (int)value;
            Changed();
        }
    }

    /// <summary>原文 67 published 属性 Draw。</summary>
    public bool Draw
    {
        get => _draw;
        set
        {
            if (_draw == value) return;
            _draw = value;
            Changed();
        }
    }

    /// <summary>原文 68 published 属性 Stretch。</summary>
    public bool Stretch
    {
        get => _stretch;
        set
        {
            if (_stretch == value) return;
            _stretch = value;
            Changed();
        }
    }

    /// <summary>原文 69 published 属性 Center。</summary>
    public bool Center
    {
        get => _center;
        set
        {
            if (_center == value) return;
            _center = value;
            Changed();
        }
    }

    /// <summary>原文 70 published 属性 AutoDestRectSize。</summary>
    public bool AutoDestRectSize
    {
        get => _autoDestRectSize;
        set
        {
            if (_autoDestRectSize == value) return;
            _autoDestRectSize = value;
            Changed();
        }
    }

    /// <summary>原文 71 published 属性 BlendMode（**裸读写，不触发 Changed**）。</summary>
    public int BlendMode
    {
        get => _blendMode;
        set => _blendMode = value;
    }

    /// <summary>原文 60 public 属性 SourceRect。</summary>
    public TDxRect SourceRect
    {
        get => _sourceRect;
        set => _sourceRect = value;
    }

    /// <summary>原文 61 public 属性 DestRect。</summary>
    public TDxRect DestRect
    {
        get => _destRect;
        set => _destRect = value;
    }

    /// <summary>原文 937-945 GetSourcePosition（index 2/3；其余 0）。</summary>
    public int GetSourcePosition(int index)
    {
        switch (index)
        {
            case 2: return _sourceRect.Right - _sourceRect.Left;
            case 3: return _sourceRect.Bottom - _sourceRect.Top;
            default: return 0;
        }
    }

    /// <summary>原文 947-968 SetSourcePosition（index 0 平移 / 1 平移 / 2 只改右 / 3 只改下）。</summary>
    public void SetSourcePosition(int index, int value)
    {
        var newRect = _sourceRect;
        switch (index)
        {
            case 0:
                {
                    int aux = newRect.Right - newRect.Left;
                    newRect.Left = value;
                    newRect.Right = value + aux;
                    break;
                }
            case 1:
                {
                    int aux = newRect.Bottom - newRect.Top;
                    newRect.Top = value;
                    newRect.Bottom = value + aux;
                    break;
                }
            case 2:
                newRect.Right = newRect.Left + value;
                break;
            case 3:
                newRect.Bottom = newRect.Top + value;
                break;
        }
        _sourceRect = newRect;
    }

    /// <summary>原文 970-978 GetDestPosition。</summary>
    public int GetDestPosition(int index)
    {
        switch (index)
        {
            case 2: return _destRect.Right - _destRect.Left;
            case 3: return _destRect.Bottom - _destRect.Top;
            default: return 0;
        }
    }

    /// <summary>原文 980-1001 SetDestPosition。</summary>
    public void SetDestPosition(int index, int value)
    {
        var newRect = _destRect;
        switch (index)
        {
            case 0:
                {
                    int aux = newRect.Right - newRect.Left;
                    newRect.Left = value;
                    newRect.Right = value + aux;
                    break;
                }
            case 1:
                {
                    int aux = newRect.Bottom - newRect.Top;
                    newRect.Top = value;
                    newRect.Bottom = value + aux;
                    break;
                }
            case 2:
                newRect.Right = newRect.Left + value;
                break;
            case 3:
                newRect.Bottom = newRect.Top + value;
                break;
        }
        _destRect = newRect;
    }

    /// <summary>原文 1038-1064 TDxFormShapeImage.Assign。</summary>
    public void Assign(TDxFormShapeImage source)
    {
        if (source == null) return;
        Image = source.Image;
        OnGetImage = source.OnGetImage;
        _imageType = source._imageType;            // 原文 `ImageType := ...`（走 setter）

        ImageIndex = source._imageIndex;           // 原文 `ImageIndex := ...`

        Draw = source.Draw;
        Stretch = source.Stretch;
        Center = source.Center;

        // 原文 1051-1055 逐项：SourceLeft/SourceTop/SourceWidth/SourceHeight（都读写 _sourceRect）
        _sourceRect = source._sourceRect;
        _destRect = source._destRect;
        AutoDestRectSize = source.AutoDestRectSize;
        Align = source.Align;
        Changed();
    }

    /// <summary>原文 73-81 的 SourceLeft/SourceTop/SourceWidth/SourceHeight 便捷读写。</summary>
    public int SourceLeft { get => _sourceRect.Left; set => SetSourcePosition(0, value); }

    /// <summary>原文 74 SourceTop。</summary>
    public int SourceTop { get => _sourceRect.Top; set => SetSourcePosition(1, value); }

    /// <summary>原文 75 SourceWidth。</summary>
    public int SourceWidth { get => GetSourcePosition(2); set => SetSourcePosition(2, value); }

    /// <summary>原文 76 SourceHeight。</summary>
    public int SourceHeight { get => GetSourcePosition(3); set => SetSourcePosition(3, value); }

    /// <summary>原文 78 DestLeft。</summary>
    public int DestLeft { get => _destRect.Left; set => SetDestPosition(0, value); }

    /// <summary>原文 79 DestTop。</summary>
    public int DestTop { get => _destRect.Top; set => SetDestPosition(1, value); }

    /// <summary>原文 80 DestWidth。</summary>
    public int DestWidth { get => GetDestPosition(2); set => SetDestPosition(2, value); }

    /// <summary>原文 81 DestHeight。</summary>
    public int DestHeight { get => GetDestPosition(3); set => SetDestPosition(3, value); }
}

// -------------------------------------------------------------------------------------
// DxImageForm.pas 86-154 / 262-490：TAnimation
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageForm.pas 86-154 / 262-490 TAnimation（窗体级动画）。
/// 与 DxImageButton 的 TButtonAnimation 是同源两份实现（原文如此，**不合并**）：
/// 差别在于顺序 —— 本单元在推进帧时**先**回调 OnAnimationFrameChanged 再判越界回绕，
/// 而 TButtonAnimation 是先回绕再回调。
/// </summary>
public class TDxImageFormAnimation : IDisposable
{
    private TDxImageForm _owner;                        // FOwner
    private int _animationIndex;                        // FAnimationIndex
    private TImageType _imageType = TImageType.Prguse_wil;
    private int _startIndex = -1;
    private int _endIndex = -1;
    private int _frameTime = 200;
    private int _playCount;
    private bool _useImageOffset = true;
    private int _offsetX;
    private int _offsetY;
    private bool _blendDraw;
    private bool _draw;
    private bool _outsideAreaDraw;
    private bool _drawBeforeDef;
    private int _currentFrame;
    private uint _lastFrameTick;
    private int _currentCount;

    /// <summary>原文 FOnChange。</summary>
    public Action<TDxImageFormAnimation> OnChange;

    /// <summary>原文 FOnGetImage。</summary>
    public Action<TDxImageFormAnimation, TImageType, object> OnGetImage;

    /// <summary>原文 FImage（TGameImages）→ 接缝 IDxImageLibrary。</summary>
    public IDxImageLibrary Image;

    internal TDxImageFormAnimation(TDxImageForm owner) { _owner = owner; }

    /// <summary>原文 296-318 TAnimation.Create 默认值逐项。</summary>
    public static TDxImageFormAnimation Create(TDxImageForm owner, int animationIndex)
    {
        var a = new TDxImageFormAnimation(owner)
        {
            _imageType = TImageType.Prguse_wil,
            _startIndex = -1,
            _endIndex = -1,
            _frameTime = 200,
            _playCount = 0,
            _useImageOffset = true,
            _offsetX = 0,
            _offsetY = 0,
            _outsideAreaDraw = false,
            _blendDraw = false,
            _draw = false,
            _drawBeforeDef = false,
            _currentFrame = 0,
            _lastFrameTick = DxTickCount.MyGetTickCount(),
            _currentCount = 0,
            _animationIndex = animationIndex,
        };
        return a;
    }

    /// <summary>原文 320-324 Destroy（无自身释放动作）。</summary>
    public void Dispose() { }

    /// <summary>原文 290-294 TAnimation.Changed。</summary>
    public void Changed() => OnChange?.Invoke(this);

    /// <summary>原文 138 / 509-516 AnimationIndex（只读；构造里由宿主指派 0/1/2）。</summary>
    public int AnimationIndex => _animationIndex;

    /// <summary>原文 140 published ImageType。</summary>
    public TImageType ImageType
    {
        get => _imageType;
        set
        {
            if (_imageType == value) return;
            _imageType = value;
            if (OnGetImage != null) OnGetImage(this, _imageType, Image);
            Changed();
        }
    }

    /// <summary>原文 142 StartIndex。</summary>
    public int StartIndex { get => _startIndex; set => _startIndex = value; }

    /// <summary>原文 143 EndIndex。</summary>
    public int EndIndex { get => _endIndex; set => _endIndex = value; }

    /// <summary>原文 144 FrameTime。</summary>
    public int FrameTime { get => _frameTime; set => _frameTime = value; }

    /// <summary>原文 145 PlayCount。</summary>
    public int PlayCount { get => _playCount; set => _playCount = value; }

    /// <summary>原文 146 OffsetX。</summary>
    public int OffsetX { get => _offsetX; set => _offsetX = value; }

    /// <summary>原文 147 OffsetY。</summary>
    public int OffsetY { get => _offsetY; set => _offsetY = value; }

    /// <summary>原文 148 UseImageOffset。</summary>
    public bool UseImageOffset { get => _useImageOffset; set => _useImageOffset = value; }

    /// <summary>原文 150 OutsideAreaDraw。</summary>
    public bool OutsideAreaDraw
    {
        get => _outsideAreaDraw;
        set { if (_outsideAreaDraw != value) _outsideAreaDraw = value; }
    }

    /// <summary>原文 151 Draw（setter 会重置帧/计数，见 423-432）。</summary>
    public bool Draw
    {
        get => _draw;
        set
        {
            if (_draw == value) return;
            _draw = value;
            _currentFrame = _startIndex;
            _lastFrameTick = DxTickCount.MyGetTickCount();
            _currentCount = 0;
        }
    }

    /// <summary>原文 152 BlendDraw（裸赋值）。</summary>
    public bool BlendDraw { get => _blendDraw; set => _blendDraw = value; }

    /// <summary>原文 153 DrawBeforeDef（裸赋值）。</summary>
    public bool PaintBeforeDefault { get => _drawBeforeDef; set => _drawBeforeDef = value; }

    /// <summary>原文 FCurrentFrame / FCurrentCount / FLastFrameTick 的只读视图（测试用）。</summary>
    public int CurrentFrame => _currentFrame;

    /// <summary>原文 FCurrentCount。</summary>
    public int CurrentCount => _currentCount;

    /// <summary>原文 FLastFrameTick。</summary>
    public uint LastFrameTick => _lastFrameTick;

    /// <summary>原文 474-480 SetOnGetImage（赋值后立即回调一次，再 Changed）。</summary>
    public void SetOnGetImage(Action<TDxImageFormAnimation, TImageType, object> value)
    {
        OnGetImage = value;
        if (OnGetImage != null) OnGetImage(this, _imageType, Image);
        Changed();
    }

    /// <summary>原文 262-288 TAnimation.Assign（含帧状态/计时戳一并拷贝）。</summary>
    public void Assign(TDxImageFormAnimation source)
    {
        if (source == null) return;
        SetOnGetImage(source.OnGetImage);
        ImageType = source.ImageType;

        _startIndex = source._startIndex;              // 原文逐字段直写
        _endIndex = source._endIndex;
        _frameTime = source._frameTime;
        _playCount = source._playCount;

        _useImageOffset = source._useImageOffset;
        _offsetX = source._offsetX;
        _offsetY = source._offsetY;

        _outsideAreaDraw = source._outsideAreaDraw;
        _blendDraw = source._blendDraw;
        _draw = source._draw;
        _drawBeforeDef = source._drawBeforeDef;

        _currentFrame = source._currentFrame;
        _lastFrameTick = source._lastFrameTick;
        _currentCount = source._currentCount;

        Changed();
    }

    /// <summary>
    /// 原文 326-359 的**帧推进**段（不含绘制），1:1：
    /// ① 前置守卫 `FImage = nil` / `Start &lt; 0 or End &lt; 0 or Start &gt; End` / `not FDraw` 任一 → 直接返回 false；
    /// ② `PlayCount &gt; 0 && CurrentCount &gt;= PlayCount` → 返回 false（播完即停）；
    /// ③ `CurrentFrame` 越界 → 拉回 StartIndex；
    /// ④ 距上次计时 &gt;= FrameTime 时 `Inc(CurrentFrame)`，
    ///    **先**回调 `OnAnimationFrameChanged(Owner, AnimationIndex, CurrentCount, CurrentFrame)`
    ///    （注意传的是**自增后、回绕前**的帧号），**再**判越界回绕并把 CurrentCount 加一，最后刷新计时戳。
    /// 返回 true 表示「本帧应当绘制」（原文在该段之后继续取图绘制）。
    /// </summary>
    public bool AdvanceFrame()
    {
        if (Image == null) return false;
        if (_startIndex < 0 || _endIndex < 0 || _startIndex > _endIndex) return false;
        if (!_draw) return false;

        if (_playCount > 0)
        {
            if (_currentCount >= _playCount) return false;
        }

        if (_currentFrame < _startIndex || _currentFrame > _endIndex)
            _currentFrame = _startIndex;

        if (DxTickCount.MyGetTickCount() - _lastFrameTick >= (uint)_frameTime)
        {
            _currentFrame++;

            // 原文 347-348：回调在回绕判定**之前**
            _owner?.RaiseAnimationFrameChanged(this, _animationIndex, _currentCount, _currentFrame);

            if (_currentFrame > _endIndex)
            {
                _currentFrame = _startIndex;

                if (_playCount > 0)
                {
                    _currentCount++;
                }
            }

            _lastFrameTick = DxTickCount.MyGetTickCount();
        }

        return true;
    }

    /// <summary>
    /// 原文 361-414 的**绘制**段（几何部分），1:1：
    /// `D := FImage.GetCachedImage(FCurrentFrame, nX, nY)`，nX/nY 为缓存纹理内偏移；
    /// `ParentRect := FOwner.VirtualRect`；左上角 = ParentRect.Left+OffsetX(+nX)；
    /// `not OutsideAreaDraw` 时把目标矩形逐边夹进 ParentRect **并同步修 SrcRect**（四段顺序不可换；
    /// 原文 390 的 `r.Top` 小写笔误照抄为等价写法）；
    /// 两条分支各自重算 BlendMode（BlendDraw → Blend_SrcAlphaColor，否则 2）。
    /// 返回 null 表示原文不绘制。blendMode 为原文两处各自算出的值。
    /// </summary>
    public AnimationDrawPlan BuildDrawPlan()
    {
        if (Image == null) return null;
        if (_startIndex < 0 || _endIndex < 0 || _startIndex > _endIndex) return null;
        if (!_draw) return null;
        if (_playCount > 0 && _currentCount >= _playCount) return null;

        int frame = (_currentFrame < _startIndex || _currentFrame > _endIndex) ? _startIndex : _currentFrame;
        var texture = Image.GetImage(frame);
        if (texture == null) return null;

        // 原文 GetCachedImage(FCurrentFrame, nX, nY) 的接缝表达：GetImage + 偏移 0
        int nX = 0, nY = 0;

        var parentRect = _owner != null ? _owner.VirtualRect : TDxRect.Empty;
        var srcRect = texture.ClientRect;

        var r = TDxRect.Empty;
        if (_useImageOffset)
        {
            r.Left = parentRect.Left + _offsetX + nX;
            r.Top = parentRect.Top + _offsetY + nY;
        }
        else
        {
            r.Left = parentRect.Left + _offsetX;
            r.Top = parentRect.Top + _offsetY;
        }

        int blendMode = _blendDraw ? TDxHgeBlend.Blend_SrcAlphaColor : 2;

        if (!_outsideAreaDraw)
        {
            r.Right = r.Left + texture.Width;
            r.Bottom = r.Top + texture.Height;

            if (r.Left < parentRect.Left)
            {
                srcRect.Left += parentRect.Left - r.Left;
                r.Left = parentRect.Left;
            }

            if (r.Right > parentRect.Right)
            {
                srcRect.Right -= r.Right - parentRect.Right;
                r.Right = parentRect.Right;
            }

            if (r.Top < parentRect.Top)
            {
                // 原文 390 `SrcRect.Top := SrcRect.Top + (ParentRect.Top - r.Top);` —— r 小写（原文如此）
                srcRect.Top += parentRect.Top - r.Top;
                r.Top = parentRect.Top;
            }

            if (r.Bottom > parentRect.Bottom)
            {
                srcRect.Bottom -= r.Bottom - parentRect.Bottom;
                r.Bottom = parentRect.Bottom;
            }

            return new AnimationDrawPlan(r.Left, r.Top, srcRect, texture, blendMode, false);
        }

        return new AnimationDrawPlan(r.Left, r.Top, srcRect, texture, blendMode, true);
    }

    /// <summary>原文 326-415 的整体（推进 + 绘制），落到接缝 IDxSurfacePainter。</summary>
    public void Paint(IDxSurfacePainter painter)
    {
        if (!AdvanceFrame()) return;
        var plan = BuildDrawPlan();
        if (plan == null) return;
        if (plan.FullTexture)
            painter.Draw(plan.X, plan.Y, plan.Texture, plan.BlendMode);
        else
            painter.Draw(plan.X, plan.Y, plan.SrcRect, plan.Texture);
    }

    /// <summary>原文 326-415 的绘制计划（测试可读的等价产物）。</summary>
    public sealed class AnimationDrawPlan
    {
        public readonly int X;
        public readonly int Y;
        public readonly TDxRect SrcRect;
        public readonly IDxTexture Texture;
        public readonly int BlendMode;
        /// <summary>true = 走原文 `GameCanvas.Draw(R.Left, R.Top, D, BlendMode)`（整图，无裁剪）。</summary>
        public readonly bool FullTexture;

        public AnimationDrawPlan(int x, int y, TDxRect srcRect, IDxTexture texture, int blendMode, bool fullTexture)
        {
            X = x; Y = y; SrcRect = srcRect; Texture = texture; BlendMode = blendMode; FullTexture = fullTexture;
        }
    }
}

/// <summary>原文 HGE.Blend_SrcAlphaColor（HGE.pas 的混合模式常量，本波只用到这一个）。</summary>
public static class TDxHgeBlend
{
    /// <summary>原文 HGE.Blend_SrcAlphaColor。</summary>
    public const int Blend_SrcAlphaColor = 5;
}

// -------------------------------------------------------------------------------------
// DxImageForm.pas 156-218 / 494-849：TDxImageForm
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageForm.pas 156-218 / 494-849 TDxImageForm（图片窗体基类）。
///
/// 托管侧签名说明：上游 TDxControl 的 DoShow/DoHide/DoMouseDown/DoMouseMove/DoMouseUp/CanMove/
/// InRange/MouseDown/MouseMove 中，只有 InRange / MouseDown / MouseMove 是 virtual（可 override）；
/// DoShow/DoHide/CanMove 为 protected virtual（通过 DxProtected 承接）；SetOnGetImage 非 virtual
/// 且签名不同（上游是 `SetOnGetImage(Action&lt;TDxImageIndex,TImageType&gt;)`）。
/// 故本类把原文的覆写点以「v2 命名 + 基类转发」表达，并逐条登记在报告里。
/// </summary>
public class TDxImageForm : TDxControl
{
    private bool _isBringToFront = true;               // FIsBringToFront
    private byte _backgroundAlpha;                     // FBackgroundAlpha
    private bool _noMove;                              // FNoMove

    /// <summary>原文 191 DialogResult:TModalResult。</summary>
    public int DialogResult = TDxModalResult.mrNone;

    /// <summary>原文 164/213-215 Animation1/2/3。</summary>
    public TDxImageFormAnimation Animation1;

    /// <summary>原文 165/214 Animation2。</summary>
    public TDxImageFormAnimation Animation2;

    /// <summary>原文 166/215 Animation3。</summary>
    public TDxImageFormAnimation Animation3;

    /// <summary>原文 163/205 OnAnimationFrameChanged。</summary>
    public TAnimationFrameChangedEvent OnAnimationFrameChanged;

    /// <summary>原文 168/202 OnBringToFront。</summary>
    public Action<TDxImageForm> OnBringToFront;

    /// <summary>原文 169/203 OnAfterBringToFront。</summary>
    public Action<TDxImageForm> OnAfterBringToFront;

    /// <summary>
    /// 原文 195-197 / 729-770 ShowModal* 的消息泵（ProcessMessages）。
    /// 原文是 Win32 PeekMessage/TranslateMessage/DispatchMessage 循环 —— 属未移植的 HGE/Forms 层，
    /// 故以可注入委托承接（缺省 Application.DoEvents，语义等价于「把待处理消息处理掉」）。
    /// </summary>
    public static Action MessagePumpSink = () => System.Windows.Forms.Application.DoEvents();

    /// <summary>原文 743/745/765 `Application.Terminated` 的接缝。</summary>
    public static Func<bool> ApplicationTerminated = () => false;

    /// <summary>原文 746/767 `Sleep(1)` 的接缝（测试可注入 0 以避免真实等待）。</summary>
    public static Action<int> SleepSink = ms => System.Threading.Thread.Sleep(ms);

    public TDxImageForm()
    {
        // 原文 494-519 构造逐项
        Floating = true;
        DrawBorder = false;
        Width = 200;
        Height = 100;
        _backgroundAlpha = 0;
        _isBringToFront = true;

        Animation1 = TDxImageFormAnimation.Create(this, 0);
        Animation2 = TDxImageFormAnimation.Create(this, 1);
        Animation3 = TDxImageFormAnimation.Create(this, 2);

        _noMove = false;
    }

    /// <summary>原文 521-530 TDxImageForm.Destroy。</summary>
    public void DestroyAnimations()
    {
        Animation1?.Dispose();
        Animation2?.Dispose();
        Animation3?.Dispose();
    }

    /// <summary>原文 158/200 IsBringToFront。</summary>
    public bool IsBringToFront { get => _isBringToFront; set => _isBringToFront = value; }

    /// <summary>原文 217 NoMove。</summary>
    public bool NoMove { get => _noMove; set => _noMove = value; }

    /// <summary>原文 171/753-757 SetBackgroundAlpha（仅值变化才写）。</summary>
    public byte BackgroundAlpha
    {
        get => _backgroundAlpha;
        set { if (_backgroundAlpha != value) _backgroundAlpha = value; }
    }

    /// <summary>
    /// 原文 532-536 TDxImageForm.DoShow：**先 BringToFront 再 inherited**（顺序不可换）。
    /// 上游接缝的 TDxControl **没有** DoShow 虚方法（原文 314-343 那一批 Do* 只有
    /// DoCaptionChange/DoResize/DoPaint/DoOnInRealArea 进了接缝），故以 new 虚方法承接覆写点。
    /// </summary>
    protected virtual void DoShow()
    {
        BringToFrontV2();
    }

    /// <summary>原文 575-579 TDxImageForm.DoHide：先 `RootCtrl.DeleteModalForm(Self)` 再 inherited。</summary>
    protected virtual void DoHide()
    {
        DxControlOps.RootCtrlOf(this)?.DeleteModalForm(this);
    }

    /// <summary>原文 772-775 TDxImageForm.CanMove：`not FNoMove`。</summary>
    public bool CanMoveV2() => !_noMove;

    /// <summary>把 CanMoveV2 挂到 DxControlOps 的可覆写落点（原文的虚覆写等价物）。</summary>
    public void InstallCanMoveOverride()
    {
        var prev = DxControlOps.CanMoveSink;
        DxControlOps.CanMoveSink = self =>
            self is TDxImageForm f ? f.CanMoveV2() : (prev != null ? prev(self) : true);
    }

    /// <summary>
    /// 原文 631-639 TDxImageForm.InRange：
    /// 非设计期走基类 InRange；设计期只判 `PointInRect(Point(X,Y), VisibleRect)`。
    /// 上游 InRange 是 virtual，故此处直接 override。
    /// </summary>
    public override bool InRange(int x, int y)
    {
        if (!Designing)
            return base.InRange(x, y);
        return DxRectUtil.PointInRect(TDxPoint.Point(x, y), VisibleRect);
    }

    /// <summary>原文 625-629 TDxImageForm.MouseDown：先 inherited 再 BringToFront。</summary>
    public override void MouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        base.MouseDown(button, shift, x, y);
        BringToFront();
    }

    /// <summary>原文 641-668 TDxImageForm.MouseMove（设计期那段是空转的探测代码，逐字保留结构）。</summary>
    public override void MouseMove(TDxShiftState shift, int x, int y)
    {
        base.MouseMove(shift, x, y);
        if (DxRectUtil.PointInRect(TDxPoint.Point(x, y), VisibleRect))
        {
            if (Designing && ImageIndex.Image != null)
            {
                int faceIndex = -1;

                if (ImageIndex.Up >= 0) faceIndex = ImageIndex.Up;
                else if (ImageIndex.Hot >= 0) faceIndex = ImageIndex.Hot;
                else if (ImageIndex.Down >= 0) faceIndex = ImageIndex.Down;

                if (faceIndex >= 0)
                {
                    var texture = ImageIndex.Image.GetImage(faceIndex);
                    if (texture != null && texture.Width * texture.Height > 4)
                    {
                        var vRect = VirtualRect;          // 原文取到 vRect 后**没有后续使用**（原文如此）
                        _ = vRect;
                    }
                }
            }
        }
    }

    /// <summary>原文 602-608 TDxImageForm.SetOnGetImage：自身 + 三个动画一并转挂。</summary>
    public void SetOnGetImageV2(Action<TDxImageIndex, TImageType> value)
    {
        DxControlHooks.SetOnGetImage(this, value);
        if (Animation1 != null) Animation1.SetOnGetImage((s, t, o) => value?.Invoke(ImageIndex, t));
        if (Animation2 != null) Animation2.SetOnGetImage((s, t, o) => value?.Invoke(ImageIndex, t));
        if (Animation3 != null) Animation3.SetOnGetImage((s, t, o) => value?.Invoke(ImageIndex, t));
    }

    /// <summary>原文 538-573 TDxImageForm.Assign(Source:TDxControl)。</summary>
    public void AssignFromImageForm(TDxControl source)
    {
        if (source is TDxImageForm src)
        {
            Left = source.Left;
            Top = source.Top;
            Width = source.Width;
            Height = source.Height;
            Enabled = source.Enabled;
            Visible = source.Visible;

            Transparent = source.Transparent;
            EnableFocus = source.EnableFocus;
            Floating = source.Floating;
            OwnerMove = source.OwnerMove;

            MouseEvents = source.MouseEvents;

            ReferenceX = source.ReferenceX;
            AdjustYByHeight = source.AdjustYByHeight;
            TopAlignment = source.TopAlignment;

            DxControlOps.SetCenterA(this, source.Center);
            DxControlOps.SetAlign(this, source.Align);
            DxControlOps.SetAutoSize(this, source.AutoSize);
            BackgroundColor = source.BackgroundColor;
            BackgroundAlpha = src.BackgroundAlpha;

            ImageIndex.Assign(source.ImageIndex);
            BorderColor.Assign(source.BorderColor);

            Animation1?.Assign(src.Animation1);
            Animation2?.Assign(src.Animation2);
            Animation3?.Assign(src.Animation3);
        }
    }

    /// <summary>
    /// 原文 581-593 TDxImageForm.BringToFront：
    /// `if FIsBringToFront then { OnBringToFront; inherited; OnAfterBringToFront }`。
    /// 上游 TDxControl.BringToFront 是 DxControlOps 的静态方法（非 virtual），故这里叫 BringToFrontV2。
    /// </summary>
    public void BringToFrontV2()
    {
        if (!_isBringToFront) return;
        OnBringToFront?.Invoke(this);
        DxControlOps.BringToFront(this);              // 原文 inherited BringToFront
        OnAfterBringToFront?.Invoke(this);
    }

    /// <summary>原文 595-600 TDxImageForm.BringToFrontEx：只走 inherited，不触发两个回调。</summary>
    public void BringToFrontEx()
    {
        if (!_isBringToFront) return;
        DxControlOps.BringToFront(this);
    }

    /// <summary>原文 729-735 TDxImageForm.ShowModalEx：登记模态窗体后立即返回 0。</summary>
    public int ShowModalEx()
    {
        DialogResult = TDxModalResult.mrNone;
        Visible = true;
        var root = DxControlOps.RootCtrlOf(this);
        if (root != null) root.SetModalForm(this);
        return 0;
    }

    /// <summary>
    /// 原文 737-751 TDxImageForm.ShowModal(Process:TNotifyEvent)：
    /// 每轮先判 `not Visible or Application.Terminated` → ProcessMessages → **再判一次** → Sleep(1) → Process(Self)。
    /// 托管侧第 745 行的「再判一次」在原文里是 `if (not Visible) or Application.Terminated then break;`，
    /// 此处逐字保留（注意原文该处**没有** sleep）。
    /// </summary>
    public int ShowModalWithProcess(Action<TDxImageForm> process)
    {
        DialogResult = TDxModalResult.mrNone;
        Visible = true;
        var root = DxControlOps.RootCtrlOf(this);
        if (root != null) root.SetModalForm(this);

        while (true)
        {
            if (!Visible || ApplicationTerminated()) break;
            MessagePumpSink?.Invoke();
            if (!Visible || ApplicationTerminated()) break;
            SleepSink?.Invoke(1);
            process?.Invoke(this);
        }
        return 0;
    }

    /// <summary>原文 759-770 TDxImageForm.ShowModal（无 Process 回调的版本，循环里**没有**第二次可见性判定）。</summary>
    public int ShowModal()
    {
        DialogResult = TDxModalResult.mrNone;
        Visible = true;
        var root = DxControlOps.RootCtrlOf(this);
        if (root != null) root.SetModalForm(this);

        while (true)
        {
            if (!Visible || ApplicationTerminated()) break;
            MessagePumpSink?.Invoke();
            SleepSink?.Invoke(1);
        }
        return 0;
    }

    /// <summary>
    /// 原文 777-849 TDxImageForm.Paint 的**顺序骨架**：
    /// ① vbRect 退化直接 Exit；② 三个动画的 DrawBeforeDef 前置绘制；③ OnStartPaint；
    /// ④ DoPaint（=CheckAutoSize）；⑤ **原文再次调用 OnStartPaint**（820-823 的注释块之外、
    ///   原文 802-803 确实有第二次调用 —— 原文如此，逐字保留）；
    /// ⑥ OnPaint 或（有图且 Up>=0）按 OffsetX/OffsetY 平移后 DrawRect，否则 BackgroundAlpha>0 时
    ///    FillRectAlpha(vbRect, BackgroundColor, FBackgroundAlpha)；
    /// ⑦ 三个非前置动画；⑧ OnStartSubPaint；⑨ **逆序**绘制子控件（每轮开头判 `not Visible` 就 break）；
    /// ⑩ `if Visible and OnStopPaint then OnStopPaint`；⑪ Designing 时画边框。
    /// </summary>
    public void PaintImageForm()
    {
        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;
        var vtRect = VirtualRect;

        if (Animation1.PaintBeforeDefault) Animation1.Paint(Painter);
        if (Animation2.PaintBeforeDefault) Animation2.Paint(Painter);
        if (Animation3.PaintBeforeDefault) Animation3.Paint(Painter);

        DxControlHooks.GetOnStartPaint(this)?.Invoke(this);

        DxControlHooks.DoPaint(this);

        // 原文 802-803：第二次 OnStartPaint（原文如此，逐字保留）
        DxControlHooks.GetOnStartPaint(this)?.Invoke(this);

        if (OnPaint != null)
        {
            OnPaint(this);
        }
        else if (ImageIndex.Image != null && ImageIndex.Up >= 0)
        {
            var texture = ImageIndex.Image.GetImage(ImageIndex.Up);
            if (texture != null)
            {
                var vtRect2 = vtRect;
                vtRect2.Left = vtRect.Left + ImageIndex.OffsetX;
                vtRect2.Top = vtRect.Top + ImageIndex.OffsetY;
                DxControlOps.DrawRectScoped(this, vtRect2, vtRect, vbRect, texture, BlendMode);
            }
        }
        else if (_backgroundAlpha > 0)
        {
            DxPainterExt.FillRectAlpha(Painter, vbRect, vtRect, vbRect, BackgroundColor, _backgroundAlpha);
        }

        // 原文 820-823（被注释掉的第二次 OnStartPaint 注释块，原文如此）：
        //   {
        //   if Assigned(OnStartPaint) then
        //     OnStartPaint(Self);
        //   }

        if (!Animation1.PaintBeforeDefault) Animation1.Paint(Painter);
        if (!Animation2.PaintBeforeDefault) Animation2.Paint(Painter);
        if (!Animation3.PaintBeforeDefault) Animation3.Paint(Painter);

        DxControlHooks.GetOnStartSubPaint(this)?.Invoke(this);

        for (int i = DxControlOps.ComponentCount(this) - 1; i >= 0; i--)
        {
            if (!Visible) break;
            var c = DxControlOps.Components(this, i);
            if (c.Visible)
                DxControlOps.Paint(c);
        }

        if (Visible)
            DxControlHooks.GetOnStopPaint(this)?.Invoke(this);

        if (Designing)
        {
            DxControlOps.FrameRect(this, vtRect, vtRect, vbRect, BorderColor.Up.Color);
        }
    }

    /// <summary>原文 347-348 的回调转发（TAnimation 调 FOwner.FOnAnimationFrameChanged）。</summary>
    internal void RaiseAnimationFrameChanged(TDxImageFormAnimation sender, int animationIndex, int playCount, int frame)
        => OnAnimationFrameChanged?.Invoke(this, animationIndex, playCount, frame);
}

// -------------------------------------------------------------------------------------
// DxImageForm.pas 220-251 / 1068-1261：TDxImageFormShape
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageForm.pas 220-251 / 1068-1261 TDxImageFormShape：
/// 在 TDxImageForm 之上叠 8 张形状贴图（Draw1..Draw8），每张独立对齐/拉伸。
///
/// 原文 1109-1116 的构造用 `P := @FDraw1; Inc(Integer(P), 4)` 连续写 8 个字段 ——
/// 托管侧改为显式数组装配（等价，且不依赖字段布局）。
/// </summary>
public class TDxImageFormShape : TDxImageForm
{
    /// <summary>原文 223-229 FDraw1..FDraw8（托管侧以数组统一持有，公开属性见下）。</summary>
    public readonly TDxFormShapeImage[] Items = new TDxFormShapeImage[8];

    public TDxImageFormShape()
    {
        // 原文 1103-1117 构造
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = new TDxFormShapeImage();
            Items[i].OnChange = ImageIndexChange;
            Items[i].OnGetImage = (s, t, o) => DxControlHooks.GetOnGetImage(this)?.Invoke(ImageIndex, t);
        }
    }

    /// <summary>原文 1068-1071 GetImageCount（= Length(FDxFormShapeInfo) = 8）。</summary>
    public int ShapeImageCount => Items.Length;

    /// <summary>原文 1073-1076 GetShapeImages（**不判越界**，原文直接索引）。</summary>
    public TDxFormShapeImage GetShapeImages(int index) => Items[index];

    /// <summary>
    /// 原文 1078-1101 TDxImageFormShape.ImageIndexChange：
    /// 与 TDxFormShapeImage.SetImageIndex 的几何推导**逐字相同**（原文两处重复），故转调同一实现。
    /// </summary>
    public void ImageIndexChange(TDxFormShapeImage shapeImage) => TDxFormShapeImage.ApplyImageIndexGeometry(shapeImage);

    /// <summary>原文 1128-1135 TDxImageFormShape.SetOnGetImage：先给 8 张形状贴图，再 inherited。</summary>
    public void SetOnGetImageShape(Action<TDxImageIndex, TImageType> value)
    {
        for (int i = 0; i < Items.Length; i++)
            Items[i].OnGetImage = (s, t, o) => value?.Invoke(ImageIndex, t);
        SetOnGetImageV2(value);
    }

    /// <summary>原文 1137-1261 TDxImageFormShape.Paint 的形状贴图绘制段（几何与顺序 1:1）。</summary>
    public void PaintShapes()
    {
        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;
        var vtRect = VirtualRect;

        DxControlHooks.DoPaint(this);

        DxControlHooks.GetOnStartPaint(this)?.Invoke(this);

        if (OnPaint != null)
        {
            OnPaint(this);
        }
        else if (ImageIndex.Image != null)
        {
            if (ImageIndex.Up >= 0)
            {
                var texture = ImageIndex.Image.GetImage(ImageIndex.Up);
                if (texture != null)
                {
                    // 原文 1161（被注释掉）：// Canvas.Draw(0,0, Texture, Transparent);
                    DxControlOps.DrawRectScoped(this, vtRect, vtRect, vbRect, texture, BlendMode);
                }
            }
        }

        for (int i = 0; i < Items.Length; i++)
        {
            var si = Items[i];
            if (!si.Draw || si.Image == null) continue;

            var texture = si.Image.GetImage(si.ImageIndex);
            if (texture == null || texture.Width * texture.Height <= 4) continue;

            var sourceRect = DxRectUtil.ShortRect(texture.ClientRect, si.SourceRect);
            var destRect = DxRectUtil.ShortRect(TDxRect.Bounds(0, 0, Width, Height), si.DestRect);

            if (!(sourceRect.Bottom > sourceRect.Top && sourceRect.Right > sourceRect.Left &&
                  destRect.Bottom > destRect.Top && destRect.Right > destRect.Left))
                continue;

            int nLeft = 0, nTop = 0;
            switch (si.Align)
            {
                case TAlignEx.alxNone:
                    {
                        nLeft = vtRect.Left + destRect.Left;
                        nTop = vtRect.Top + destRect.Top;
                        if (si.Center)
                        {
                            int nWidth = sourceRect.Right - sourceRect.Left;
                            int nHeight = sourceRect.Bottom - sourceRect.Top;
                            nLeft = vtRect.Left + ((vtRect.Right - vtRect.Left) - nWidth) / 2;
                            nTop = vtRect.Top + ((vtRect.Bottom - vtRect.Top) - nHeight) / 2;
                            // 原文 1186（被注释掉）：
                            //   // GameCanvas.Draw(vtRect.Left + nLeft, vtRect.Top + nTop, SourceRect, Texture);
                        }
                        // 原文 1187-1192（被注释掉的 Stretch 分支）：
                        //   {end else begin
                        //     if FDxFormShapeInfo[I].Stretch then begin
                        //       DestRect := Bounds(vtRect.Left + DestRect.Left, vtRect.Top + DestRect.Top,
                        //                          SourceRect.Right - SourceRect.Left, SourceRect.Bottom - SourceRect.Top);
                        //       GameCanvas.StretchDraw(DestRect, SourceRect, Texture, FDxFormShapeInfo[I].BlendMode)
                        //     end else
                        //       GameCanvas.Draw(vtRect.Left + DestRect.Left, vtRect.Top + DestRect.Top,
                        //                       SourceRect, Texture, FDxFormShapeInfo[I].BlendMode); }
                        break;
                    }
                case TAlignEx.alxTop:
                    nLeft = vtRect.Left;
                    nTop = vtRect.Top;
                    break;
                case TAlignEx.alxBottom:
                    nLeft = vtRect.Left;
                    nTop = vtRect.Top + (vtRect.Bottom - vtRect.Top) - (sourceRect.Bottom - sourceRect.Top);
                    break;
                case TAlignEx.alxLeft:
                    nLeft = vtRect.Left;
                    nTop = vtRect.Top;
                    break;
                case TAlignEx.alxRight:
                    nLeft = vtRect.Left + (vtRect.Right - vtRect.Left) - (sourceRect.Right - sourceRect.Left);
                    nTop = vtRect.Top;
                    break;
                case TAlignEx.alxClient:
                    nLeft = vtRect.Left;
                    nTop = vtRect.Top;
                    if (si.Stretch)
                        destRect = TDxRect.Bounds(nLeft, nTop, vtRect.Right - vtRect.Left, vtRect.Bottom - vtRect.Top);
                    break;
                case TAlignEx.alxTopLeft:
                    nLeft = vtRect.Left;
                    nTop = vtRect.Top;
                    break;
                case TAlignEx.alxTopRight:
                    nLeft = vtRect.Left + (vtRect.Right - vtRect.Left) - (sourceRect.Right - sourceRect.Left);
                    nTop = vtRect.Top;
                    break;
                case TAlignEx.alxBottomLeft:
                    nLeft = vtRect.Left;
                    nTop = vtRect.Top + (vtRect.Bottom - vtRect.Top) - (sourceRect.Bottom - sourceRect.Top);
                    break;
                case TAlignEx.alxBottomRight:
                    nLeft = vtRect.Left + (vtRect.Right - vtRect.Left) - (sourceRect.Right - sourceRect.Left);
                    nTop = vtRect.Top + (vtRect.Bottom - vtRect.Top) - (sourceRect.Bottom - sourceRect.Top);
                    break;
            }

            if (si.Stretch)
            {
                destRect = TDxRect.Bounds(nLeft, nTop, destRect.Right - destRect.Left, destRect.Bottom - destRect.Top);
                DxPainterExt.StretchDraw(Painter, destRect, sourceRect, texture, si.BlendMode);
            }
            else
            {
                // 原文 1240 用的是 5 参 GameCanvas.Draw(nLeft, nTop, SourceRect, Texture, BlendMode)；
                // 上游接缝的 Draw 只有 4 参（无 BlendMode），故走扩展落点。
                DxPainterExt.DrawBlend(Painter, nLeft, nTop, sourceRect, texture, si.BlendMode);
            }
        }

        DxControlHooks.GetOnStartSubPaint(this)?.Invoke(this);

        for (int i = DxControlOps.ComponentCount(this) - 1; i >= 0; i--)
        {
            if (!Visible) break;
            var c = DxControlOps.Components(this, i);
            if (c.Visible)
                DxControlOps.Paint(c);
        }

        if (Visible)
            DxControlHooks.GetOnStopPaint(this)?.Invoke(this);

        if (Designing)
        {
            DxControlOps.FrameRect(this, vtRect, vtRect, vbRect, BorderColor.Up.Color);
        }
    }

    /// <summary>原文 1119-1126 TDxImageFormShape.Destroy（释放 8 张形状贴图）。</summary>
    public void DestroyShapes()
    {
        for (int i = 0; i < Items.Length; i++)
            Items[i]?.Dispose();
    }

    /// <summary>原文 243-250 published Draw1..Draw8 的访问器。</summary>
    public TDxFormShapeImage Draw1 { get => Items[0]; set => Items[0] = value; }

    /// <summary>原文 244 Draw2。</summary>
    public TDxFormShapeImage Draw2 { get => Items[1]; set => Items[1] = value; }

    /// <summary>原文 245 Draw3。</summary>
    public TDxFormShapeImage Draw3 { get => Items[2]; set => Items[2] = value; }

    /// <summary>原文 246 Draw4。</summary>
    public TDxFormShapeImage Draw4 { get => Items[3]; set => Items[3] = value; }

    /// <summary>原文 247 Draw5。</summary>
    public TDxFormShapeImage Draw5 { get => Items[4]; set => Items[4] = value; }

    /// <summary>原文 248 Draw6。</summary>
    public TDxFormShapeImage Draw6 { get => Items[5]; set => Items[5] = value; }

    /// <summary>原文 249 Draw7。</summary>
    public TDxFormShapeImage Draw7 { get => Items[6]; set => Items[6] = value; }

    /// <summary>原文 250 Draw8。</summary>
    public TDxFormShapeImage Draw8 { get => Items[7]; set => Items[7] = value; }
}
