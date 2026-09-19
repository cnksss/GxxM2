using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

// =====================================================================================
// DxImageButton.pas（1,019 行）1:1 逐字移植。
//
//   * 46-54    TClickSound（原文的 TClickSound / TDrawAligment 在 DxComponents.pas 49/54 已定义，
//             上一波车道5 把它们**内嵌**进了 TDxImageButton 的最小接缝类里；
//             本文件把它们提升为 namespace 级别名以保持 1:1 名称可用）
//   * 21-92    TButtonAnimation（按钮级动画：ShowType 门控 + 帧推进 + 裁剪绘制）
//   * 94-169   TDxImageButton（图片按钮：三种 Style / Caption 五态字体 / 按键音 / 动画）
//   * 171-1019 实现段
//
// DFM: TDxImageButton / TButtonAnimation 无同名 .dfm（Source\Client-HGE 全树 glob 核实，0 命中），
//      设计期取值落在各 .GUI 文件的 Tgui* 记录里 —— 不按 .dfm 对齐。
//
// -------------------------------------------------------------------------------------
// 归属说明（并行调度会话裁决）：上一波车道5 在 `DxLabel.cs` 里落了一份 `TDxImageButton`
// **最小接缝**（只为支撑 `TDxLabel : TDxImageButton`）。本文件是 `DxImageButton.pas` 的
// **正式归属**：`DxLabel.cs` 的最接缝已按裁决删除，`TDxLabel` 现继承本文件的全量类型。
//
// -------------------------------------------------------------------------------------
// 托管侧落点与偏差（逐条登记）：
//   1. 原文覆写点 → 托管落点（上游接缝只让 InRange / MouseDown / MouseMove / MouseUp /
//      DoCaptionChange / DoPaint / CheckAutoSizeBase / CanMove 可覆写）：
//        CheckAutoSize  → DoPaint 覆写里调 CheckAutoSizeV2
//        SetOnGetImage  → SetOnGetImageV2
//        DoClick        → DoClickV2
//        DoMouseDown/Move/Up → 原文 604-617 都是空实现，故无需落点
//   2. `property Style:TButtonStyle`（原文 156）→ 托管属性名 **ButtonStyle**
//      （`Style` 在 WinForms Control 上已被占用；改名策略与上游接缝的 `Hint → HintText` 一致）。
//      `DxLabel.cs` 内部保留了私有 `Style` 转发以保持该文件原样。
//   3. TGameImages → IDxImageLibrary（GetImage / GetGray）。
//      `FImage.GetCachedImage(FCurrentFrame, nX, nY)` → GetImage + nX=nY=0（同 DxImageForm）。
//   4. GameCanvas.Draw(x, y, SrcRect, Texture, BlendMode) → IDxSurfacePainterExt.DrawBlend。
//      Blend 常量：`Blend_SrcAlphaColor`（FBlendDraw 时），否则**字面量 2**（原文如此）。
//   5. MyGetTickCount → DxTickCount.MyGetTickCount（可注入，便于动画单测）。
//   6. `TDxLabel.Style` 的私有转发是本文件 ButtonStyle 的兼容层 —— 见 DxLabel.cs 顶部说明。
// =====================================================================================

/// <summary>
/// DxComponents.pas 54 TClickSound。
/// （上一波车道5 把它内嵌在最小接缝类里；现提升为 namespace 级，保持 1:1 名称可用。）
/// </summary>
public enum TClickSoundNS { csNone, csStone, csGlass, csNorm }

/// <summary>
/// DxComponents.pas 49 TDrawAligment。
/// （同上：由最小接缝的内嵌枚举提升为 namespace 级。）
/// </summary>
public enum TDrawAligmentNS { daFill, daBottom }

// -------------------------------------------------------------------------------------
// DxImageButton.pas 21-92 / 179-462：TButtonAnimation
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButton.pas 21-92 / 179-462 TButtonAnimation。
///
/// 与 DxImageForm 的 `TDxImageFormAnimation` 是**同源两份实现**（原文如此，不合并）；差别：
///   * 多一个 FShowType 门控（astAlwaysShow / astNormalShow / astHotShow / astDownShow /
///     astCheckShow / astEnableShow / astDisableShow）；
///   * 帧推进时**先回绕再回调** OnAnimationFrameChanged（`TDxImageFormAnimation` 相反 ——
///     见 `DxCtrlImageFormTests` 里待补的差异断言）。
/// </summary>
public class TButtonAnimation : IDisposable
{
    private TDxImageButton _owner;                                  // FOwner
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
    public Action<TButtonAnimation> OnChange;

    /// <summary>原文 FOnGetImage。</summary>
    public Action<TButtonAnimation, TImageType, object> OnGetImage;

    /// <summary>原文 FImage（TGameImages）→ 接缝 IDxImageLibrary。</summary>
    public IDxImageLibrary Image;

    /// <summary>原文 215-239 TButtonAnimation.Create 逐项。</summary>
    public TButtonAnimation(TDxImageButton owner)
    {
        _owner = owner;
        _imageType = TImageType.Prguse_wil;      // 图库
        _startIndex = -1;                        // 图片序号
        _endIndex = -1;
        _frameTime = 200;
        _playCount = 0;
        _useImageOffset = true;
        _offsetX = 0;
        _offsetY = 0;
        _outsideAreaDraw = false;
        _blendDraw = false;
        _draw = false;                           // 是否绘制
        _drawBeforeDef = false;

        FShowType = TButtonAnimationShowType.astAlwaysShow;   // 原文 220
        _currentFrame = 0;
        _lastFrameTick = DxTickCount.MyGetTickCount();
        _currentCount = 0;
    }

    /// <summary>原文 241-245 Destroy（无自身释放动作）。</summary>
    public void Dispose() { }

    /// <summary>原文 209-213 Changed。</summary>
    public void Changed() => OnChange?.Invoke(this);

    /// <summary>原文 75 published 属性 ImageType。</summary>
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

    /// <summary>原文 77 published 属性 ShowType（默认 astAlwaysShow，见构造 220）。</summary>
    public TButtonAnimationShowType FShowType { get; set; }

    /// <summary>原文 78 StartIndex（裸赋值）。</summary>
    public int StartIndex { get => _startIndex; set => _startIndex = value; }

    /// <summary>原文 79 EndIndex（裸赋值）。</summary>
    public int EndIndex { get => _endIndex; set => _endIndex = value; }

    /// <summary>原文 80 FrameTime（裸赋值）。</summary>
    public int FrameTime { get => _frameTime; set => _frameTime = value; }

    /// <summary>原文 81 PlayCount（裸赋值）。</summary>
    public int PlayCount { get => _playCount; set => _playCount = value; }

    /// <summary>原文 82 OffsetX（裸赋值）。</summary>
    public int OffsetX { get => _offsetX; set => _offsetX = value; }

    /// <summary>原文 83 OffsetY（裸赋值）。</summary>
    public int OffsetY { get => _offsetY; set => _offsetY = value; }

    /// <summary>原文 84 UseImageOffset（裸赋值）。</summary>
    public bool UseImageOffset { get => _useImageOffset; set => _useImageOffset = value; }

    /// <summary>原文 86 OutsideAreaDraw（值变化才写，无回调）。</summary>
    public bool OutsideAreaDraw
    {
        get => _outsideAreaDraw;
        set { if (_outsideAreaDraw != value) _outsideAreaDraw = value; }
    }

    /// <summary>原文 87 Draw（setter 会重置帧/计时戳/计数，见 382-391）。</summary>
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

    /// <summary>原文 88 BlendDraw（裸赋值）。</summary>
    public bool BlendDraw { get => _blendDraw; set => _blendDraw = value; }

    /// <summary>原文 89 DrawBeforeDef（裸赋值）。</summary>
    public bool DrawBeforeDef { get => _drawBeforeDef; set => _drawBeforeDef = value; }

    /// <summary>原文 FCurrentFrame（测试可读）。</summary>
    public int CurrentFrame => _currentFrame;

    /// <summary>原文 FCurrentCount（测试可读）。</summary>
    public int CurrentCount => _currentCount;

    /// <summary>原文 FLastFrameTick（测试可读）。</summary>
    public uint LastFrameTick => _lastFrameTick;

    /// <summary>原文 441-447 SetOnGetImage（赋值后立即回调一次，再 Changed）。</summary>
    public void SetOnGetImage(Action<TButtonAnimation, TImageType, object> value)
    {
        OnGetImage = value;
        if (OnGetImage != null) OnGetImage(this, _imageType, Image);
        Changed();
    }

    /// <summary>原文 418-424 SetImage（值变化才写 + Changed）。</summary>
    public void SetImage(IDxImageLibrary value)
    {
        if (ReferenceEquals(Image, value)) return;
        Image = value;
        Changed();
    }

    /// <summary>
    /// 原文 179-207 TButtonAnimation.Assign（含帧状态/计时戳一并拷贝）。
    /// 注意原文逐字段直写 FShowType/FStartIndex/… 而**只对 OnGetImage/ImageType 走 setter**。
    /// </summary>
    public void Assign(TButtonAnimation source)
    {
        if (source == null) return;
        SetOnGetImage(source.OnGetImage);
        ImageType = source.ImageType;

        FShowType = source.FShowType;

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
        _drawBeforeDef = source.DrawBeforeDef;

        _currentFrame = source._currentFrame;
        _lastFrameTick = source._lastFrameTick;
        _currentCount = source._currentCount;

        Changed();
    }

    /// <summary>
    /// DxImageButton.pas 260-295 的 **IsShow 判定**，1:1（纯逻辑，可单测）。
    /// 判定顺序不可换：先三个绝对型 ShowType，再按 Style 分流；
    /// bsButton 分支 **MouseDowned 先于 MouseMoveed**；
    /// 非 bsButton 分支在 MouseMoveed 时 **Checked 先于 MouseDowned**。
    /// </summary>
    public bool ResolveIsShow()
    {
        if (_owner == null) return false;

        bool isShow;
        if (FShowType == TButtonAnimationShowType.astAlwaysShow)
        {
            isShow = true;
        }
        else if (FShowType == TButtonAnimationShowType.astEnableShow)
        {
            isShow = _owner.Enabled;
        }
        else if (FShowType == TButtonAnimationShowType.astDisableShow)
        {
            isShow = !_owner.Enabled;
        }
        else if (_owner.ButtonStyle == TButtonStyle.bsButton)
        {
            if (_owner.MouseDowned)
            {
                isShow = FShowType == TButtonAnimationShowType.astDownShow;
            }
            else if (_owner.MouseMoveed)
            {
                isShow = FShowType == TButtonAnimationShowType.astHotShow;
            }
            else
            {
                isShow = FShowType == TButtonAnimationShowType.astNormalShow;
            }
        }
        else
        {
            if (_owner.MouseMoveed)
            {
                if (_owner.Checked)
                {
                    isShow = FShowType == TButtonAnimationShowType.astCheckShow;
                }
                else if (_owner.MouseDowned)
                {
                    isShow = FShowType == TButtonAnimationShowType.astDownShow;
                }
                else
                {
                    isShow = FShowType == TButtonAnimationShowType.astHotShow;
                }
            }
            else
            {
                if (_owner.Checked)
                {
                    isShow = FShowType == TButtonAnimationShowType.astCheckShow;
                }
                else
                {
                    isShow = FShowType == TButtonAnimationShowType.astNormalShow;
                }
            }
        }
        return isShow;
    }

    /// <summary>
    /// DxImageButton.pas 256-319 的**帧推进**段，1:1：
    /// ① `FImage = nil` / `Start &lt; 0 or End &lt; 0 or Start &gt; End` / `not FDraw` → false；
    /// ② `not IsShow` → false；③ `PlayCount &gt; 0 and CurrentCount &gt;= PlayCount` → false；
    /// ④ `CurrentFrame` 越界 → 拉回 StartIndex；
    /// ⑤ 距上次计时 `&gt;= FrameTime` 时 `Inc(CurrentFrame)`，**先**判越界回绕并把 CurrentCount 加一，
    ///    **再**回调 OnAnimationFrameChanged（传给回调的 Frame 已是回绕后的 StartIndex），
    ///    最后刷新计时戳。返回 true 表示本帧应当绘制。
    /// </summary>
    public bool AdvanceFrame()
    {
        if (Image == null) return false;
        if (_startIndex < 0 || _endIndex < 0 || _startIndex > _endIndex) return false;
        if (!_draw) return false;

        if (!ResolveIsShow()) return false;

        if (_playCount > 0)
        {
            if (_currentCount >= _playCount) return false;
        }

        if (_currentFrame < _startIndex || _currentFrame > _endIndex)
            _currentFrame = _startIndex;

        if (DxTickCount.MyGetTickCount() - _lastFrameTick >= (uint)_frameTime)
        {
            _currentFrame++;

            if (_currentFrame > _endIndex)
            {
                _currentFrame = _startIndex;

                if (_playCount > 0)
                {
                    _currentCount++;
                }

                // 原文 314-315：回调在回绕**之后**
                _owner?.RaiseAnimationFrameChanged(_owner, 0, _currentCount, _currentFrame);
            }

            _lastFrameTick = DxTickCount.MyGetTickCount();
        }

        return true;
    }

    /// <summary>
    /// DxImageButton.pas 321-373 的**绘制计划**（几何部分），1:1。
    /// `D := FImage.GetCachedImage(FCurrentFrame, nX, nY)`（nX/nY 取 0）；
    /// `ParentRect := FOwner.VirtualRect`；左上 = `ParentRect.Left + OffsetX (+nX)`；
    /// `not OutsideAreaDraw` 时四边逐一夹进 ParentRect **并同步修 SrcRect**（顺序不可换；
    /// 原文 350 的 `r.Top` 小写笔误照抄）；两条分支各自算 BlendMode。
    /// </summary>
    public ButtonAnimationDrawPlan BuildDrawPlan()
    {
        if (Image == null) return null;
        if (_startIndex < 0 || _endIndex < 0 || _startIndex > _endIndex) return null;
        if (!_draw) return null;
        if (_playCount > 0 && _currentCount >= _playCount) return null;
        if (!ResolveIsShow()) return null;

        int frame = (_currentFrame < _startIndex || _currentFrame > _endIndex) ? _startIndex : _currentFrame;
        var texture = Image.GetImage(frame);
        if (texture == null) return null;

        int nX = 0, nY = 0;                       // 原文 GetCachedImage 的缓存内偏移
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
                // 原文 350: `SrcRect.Top := SrcRect.Top + (ParentRect.Top - r.Top);`（r 小写，原文如此）
                srcRect.Top += parentRect.Top - r.Top;
                r.Top = parentRect.Top;
            }

            if (r.Bottom > parentRect.Bottom)
            {
                srcRect.Bottom -= r.Bottom - parentRect.Bottom;
                r.Bottom = parentRect.Bottom;
            }

            return new ButtonAnimationDrawPlan(r.Left, r.Top, srcRect, texture, blendMode, false);
        }

        return new ButtonAnimationDrawPlan(r.Left, r.Top, srcRect, texture, blendMode, true);
    }

    /// <summary>原文 247-374 的整体（推进 + 绘制），落到接缝 `IDxSurfacePainterExt.DrawBlend`。</summary>
    public void Paint(IDxSurfacePainter painter)
    {
        if (!AdvanceFrame()) return;
        var plan = BuildDrawPlan();
        if (plan == null) return;
        DxPainterExt.DrawBlend(painter, plan.X, plan.Y, plan.SrcRect, plan.Texture, plan.BlendMode);
    }

    /// <summary>原文 321-373 的绘制计划（测试可读的等价产物）。</summary>
    public sealed class ButtonAnimationDrawPlan
    {
        public readonly int X;
        public readonly int Y;
        public readonly TDxRect SrcRect;
        public readonly IDxTexture Texture;
        public readonly int BlendMode;
        /// <summary>true = 走原文 `GameCanvas.Draw(R.Left, R.Top, D, BlendMode)`（整图，不裁剪）。</summary>
        public readonly bool FullTexture;

        public ButtonAnimationDrawPlan(int x, int y, TDxRect srcRect, IDxTexture texture, int blendMode,
            bool fullTexture)
        {
            X = x; Y = y; SrcRect = srcRect; Texture = texture; BlendMode = blendMode; FullTexture = fullTexture;
        }
    }
}

// -------------------------------------------------------------------------------------
// DxImageButton.pas 94-169 / 466-1017：TDxImageButton
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButton.pas 94-169 / 466-1017 TDxImageButton（`DxImageButton.pas` 的正式归属类型）。
/// </summary>
public class TDxImageButton : TDxControl
{
    private TClickSound _clickSound = TClickSound.csNone;
    private TButtonStyle _buttonStyle = TButtonStyle.bsButton;
    private int _captionDownOffsetX = 1;
    private int _captionDownOffsetY = 1;
    private int _buttonDownOffsetX;
    private int _buttonDownOffsetY;
    private TDrawAligment _drawAligment = TDrawAligment.daFill;
    private int _captionOffsetX;
    private int _captionOffsetY;
    private int _expandWidth;

    /// <summary>原文 100/155 CaptionColor（TDxCaptionColor）。</summary>
    public readonly TDxCaptionColor CaptionColor = new();

    /// <summary>原文 112/168 Animation（TButtonAnimation）。</summary>
    public TButtonAnimation Animation;

    /// <summary>原文 96/140 OnClickSound。</summary>
    public TOnClickSound OnClickSound;

    /// <summary>原文 114/144 OnAnimationFrameChanged。</summary>
    public TAnimationFrameChangedEvent OnAnimationFrameChanged;

    public TDxImageButton()
    {
        // 原文 466-489 构造逐项
        Animation = new TButtonAnimation(this);

        AutoSize = true;
        Width = 100;
        Height = 20;
        Alignment = TDxAlignment.taCenter;
        _clickSound = TClickSound.csNone;
        _buttonStyle = TButtonStyle.bsButton;
        Checked = false;
        // FCaptionColor := TDxCaptionColor.Create（字段已初始化）
        _captionDownOffsetX = 1;
        _captionDownOffsetY = 1;
        _buttonDownOffsetX = 0;
        _buttonDownOffsetY = 0;

        _drawAligment = TDrawAligment.daFill;
        _captionOffsetX = 0;
        _captionOffsetY = 0;
        _expandWidth = 0;
        GuiType = TGuiType.t_Button;               // 原文 488
    }

    /// <summary>原文 488 `GuiType := t_Button`（上游接缝未暴露，本类补为普通属性）。</summary>
    public TGuiType GuiType { get; set; } = TGuiType.t_None;

    /// <summary>原文 154/140 ClickCount（原文类型 TClickSound）。</summary>
    public TClickSound ClickCount { get => _clickSound; set => _clickSound = value; }

    /// <summary>原文 156 `property Style:TButtonStyle`（托管名 ButtonStyle，理由见文件头第 2 条）。</summary>
    public TButtonStyle ButtonStyle { get => _buttonStyle; set => _buttonStyle = value; }

    /// <summary>原文 158/116 CaptionDownOffsetX。</summary>
    public int CaptionDownOffsetX { get => _captionDownOffsetX; set => _captionDownOffsetX = value; }

    /// <summary>原文 159/117 CaptionDownOffsetY。</summary>
    public int CaptionDownOffsetY { get => _captionDownOffsetY; set => _captionDownOffsetY = value; }

    /// <summary>原文 160/118 ButtonDownOffsetX。</summary>
    public int ButtonDownOffsetX { get => _buttonDownOffsetX; set => _buttonDownOffsetX = value; }

    /// <summary>原文 161/119 ButtonDownOffsetY。</summary>
    public int ButtonDownOffsetY { get => _buttonDownOffsetY; set => _buttonDownOffsetY = value; }

    /// <summary>原文 163/116 CaptionOffsetX（setter 触发 DoCaptionChange）。</summary>
    public int CaptionOffsetX
    {
        get => _captionOffsetX;
        set
        {
            if (_captionOffsetX == value) return;
            _captionOffsetX = value;
            DoCaptionChange();
        }
    }

    /// <summary>原文 164/118 CaptionOffsetY（setter 触发 DoCaptionChange）。</summary>
    public int CaptionOffsetY
    {
        get => _captionOffsetY;
        set
        {
            if (_captionOffsetY == value) return;
            _captionOffsetY = value;
            DoCaptionChange();
        }
    }

    /// <summary>原文 165/108 DrawAligment。</summary>
    public TDrawAligment DrawAligment { get => _drawAligment; set => _drawAligment = value; }

    /// <summary>原文 166/119 ExpandWidth（setter 触发 DoCaptionChange）。</summary>
    public int ExpandWidth
    {
        get => _expandWidth;
        set
        {
            if (_expandWidth == value) return;
            _expandWidth = value;
            DoCaptionChange();
        }
    }

    /// <summary>原文 504-509 TDxImageButton.CheckAutoSize：**只有 bsButton 才走继承的自动定尺**。</summary>
    public void CheckAutoSizeV2()
    {
        if (ButtonStyle == TButtonStyle.bsButton)
            CheckAutoSizeBase();
    }

    /// <summary>原文 3073-3078 `TDxControl.DoPaint` 的 CLIENTEXE=1 分支（= CheckAutoSize）在本类的覆写。</summary>
    protected override void DoPaint() => CheckAutoSizeV2();

    /// <summary>原文 498-502 SetOnGetImage：先 inherited 再转挂 Animation.OnGetImage。</summary>
    public void SetOnGetImageV2(Action<TDxImageIndex, TImageType> value)
    {
        DxControlHooks.SetOnGetImage(this, value);
        Animation?.SetOnGetImage((s, t, o) => value?.Invoke(ImageIndex, t));
    }

    /// <summary>
    /// DxImageButton.pas 676-786 DoCaptionChange 的**几何部分**，1:1。
    /// 先按 Style/鼠标态算出 FaceIndex → 取纹理 D；
    /// Caption 非空时：
    ///   * bsButton 且 D 有效且 `D.Width &gt;= 2` 且 `D.Height &gt;= 2` → 尺寸直接取 D；
    ///   * 否则用 `CaptionColor.Up` 量文本（逐行取最大宽/最大高），Bold 时宽高各 +2；
    ///     若 D 有效则 `nWidth += 1; nWidth += D.Width; nHeight := Max(nHeight, D.Height)`；
    ///     再 `nWidth += FExpandWidth`；
    ///   * 最后 Width := nWidth / Height := nHeight。
    /// Caption 为空时：D 有效则直接按 D 定尺。
    /// 返回 null 表示 AutoSize 为 False 或两种分支都没算（原文整段被 `if AutoSize then` 包住）。
    /// </summary>
    public TDxPoint? ComputeCaptionChangeSize()
    {
        if (!AutoSize) return null;

        IDxTexture d = null;
        int faceIndex = ResolveFaceIndex();

        if (ImageIndex.Image != null && faceIndex >= 0)
        {
            // 原文 733-739（被注释掉的用 Alignment 调 vRect 的块）照抄留档：
            //   {if Texture <> nil then begin
            //      vRect := vtRect;
            //      if Alignment = taLeftJustify then vRect.Left := vRect.Right - Texture.Width;
            //    end; }
            d = ImageIndex.Image.GetImage(faceIndex);
        }

        int nWidth;
        int nHeight;

        if (Caption != "")
        {
            if (ButtonStyle == TButtonStyle.bsButton && d != null && d.Width >= 2 && d.Height >= 2)
            {
                nWidth = d.Width;
                nHeight = d.Height;
            }
            else
            {
                nWidth = 0;
                nHeight = 0;

                var hgeFont = FindFont(CaptionColor.Up);
                if (hgeFont != null)
                {
                    var textImages = GetImageInfos(hgeFont, Caption);
                    for (int i = 0; i < textImages.Count; i++)
                    {
                        if (nWidth < textImages[i].Width) nWidth = textImages[i].Width;
                        if (nHeight < textImages[i].Height) nHeight = textImages[i].Height;
                    }

                    if (CaptionColor.Up.Bold)
                    {
                        nWidth += 2;
                        nHeight += 2;
                    }
                }

                if (d != null)
                {
                    nWidth += 1;
                    nWidth += d.Width;
                    nHeight = Math.Max(nHeight, d.Height);
                }

                nWidth += _expandWidth;
            }

            return new TDxPoint(nWidth, nHeight);
        }

        if (d != null)
        {
            return new TDxPoint(d.Width, d.Height);
        }

        return null;
    }

    /// <summary>原文 676-786 DoCaptionChange：先算尺寸再 Width/Height 落值。</summary>
    protected override void DoCaptionChange()
    {
        var size = ComputeCaptionChangeSize();
        if (size == null) return;
        Width = size.Value.X;
        Height = size.Value.Y;
    }

    /// <summary>
    /// DxImageButton.pas 687-729 / 939-976 的 **FaceIndex 判定**（原文在两处各写了一遍、
    /// 逐字相同 → 收拢为一个方法）：
    ///   bsButton：MouseDowned → Down（&lt;0 时回退 Up）；MouseMoveed → Hot（&lt;0 回退 Up）；否则 Up；
    ///   其他 Style：MouseMoveed 时 Checked →（Checked&gt;=0 ? Checked : Down）；
    ///               `MouseDowned and Down&gt;=0 and Checked&gt;=0` → Down；
    ///               Hot&gt;=0 → Hot；否则 Up；
    ///               非 MouseMoveed 时 Checked →（Checked&gt;=0 ? Checked : Down）；否则 Up。
    /// 返回 -1 表示无可用面（原文各调用点自行判 `&gt;= 0`）。
    /// </summary>
    public int ResolveFaceIndex()
    {
        if (ButtonStyle == TButtonStyle.bsButton)
        {
            if (MouseDowned)
            {
                int fi = ImageIndex.Down;
                if (fi < 0 && ImageIndex.Up >= 0) fi = ImageIndex.Up;
                return fi;
            }
            if (MouseMoveed)
            {
                int fi = ImageIndex.Hot;
                if (fi < 0 && ImageIndex.Up >= 0) fi = ImageIndex.Up;
                return fi;
            }
            return ImageIndex.Up;
        }

        if (MouseMoveed)
        {
            if (Checked)
            {
                return ImageIndex.Checked >= 0 ? ImageIndex.Checked : ImageIndex.Down;
            }
            // add chongchong 2015-08-13：只有当 Down 与 Checked 都有效时才走 Down
            if (MouseDowned && ImageIndex.Down >= 0 && ImageIndex.Checked >= 0)
                return ImageIndex.Down;
            if (ImageIndex.Hot >= 0)
                return ImageIndex.Hot;
            return ImageIndex.Up;
        }

        if (Checked)
        {
            return ImageIndex.Checked >= 0 ? ImageIndex.Checked : ImageIndex.Down;
        }
        return ImageIndex.Up;
    }

    /// <summary>原文 798-822 SetChecked：bsRadio 时先清空**同父下所有** bsRadio 按钮的 Checked。</summary>
    public void SetChecked(bool value)
    {
        switch (ButtonStyle)
        {
            case TButtonStyle.bsRadio:
                {
                    if (value)
                    {
                        ClearSiblingRadios();
                        Checked = true;
                    }
                    else
                    {
                        Checked = value;
                    }
                    break;
                }
            default:
                Checked = value;
                break;
        }
    }

    /// <summary>原文 806-813 / 879-886 的公共片段：清空同父下所有 bsRadio 按钮。</summary>
    private void ClearSiblingRadios()
    {
        if (DxOwner == null) return;
        for (int i = 0; i < DxControlOps.ComponentCount(DxOwner); i++)
        {
            var d = DxControlOps.Components(DxOwner, i);
            if (d is TDxImageButton b && b.ButtonStyle == TButtonStyle.bsRadio)
                b.SetChecked(false);
        }
    }

    /// <summary>
    /// 原文 870-908 TDxImageButton.DoClick 的**状态机部分**（不含 inherited DoClick 的转发）：
    /// 非设计期且 CanMouse 时：
    ///   bsRadio → 先清空同父 bsRadio，再 `if not Checked then { Checked := True; 按键音; inherited }`
    ///             （已选则**既不发音也不触发 OnClick**）；
    ///   bsCheckBox → `Checked := not Checked; 按键音; inherited`；
    ///   其他 → `按键音; inherited`；
    /// 设计期或 `not CanMouse` 时直接 inherited / 不动作。
    /// 返回值 = 是否触发过 inherited（即 OnClick）。
    /// </summary>
    public bool DoClickV2(int x, int y)
    {
        if (!CanMouse) return false;

        if (Designing)
        {
            DxControlOps.DoClick(this, x, y);
            return true;
        }

        switch (ButtonStyle)
        {
            case TButtonStyle.bsRadio:
                {
                    ClearSiblingRadios();
                    if (!Checked)
                    {
                        Checked = true;
                        OnClickSound?.Invoke(this, _clickSound);
                        DxControlOps.DoClick(this, x, y);
                        return true;
                    }
                    return false;
                }
            case TButtonStyle.bsCheckBox:
                {
                    Checked = !Checked;
                    OnClickSound?.Invoke(this, _clickSound);
                    DxControlOps.DoClick(this, x, y);
                    return true;
                }
            default:
                {
                    OnClickSound?.Invoke(this, _clickSound);
                    DxControlOps.DoClick(this, x, y);
                    return true;
                }
        }
    }

    /// <summary>原文 848-868 TDxImageButton.InRange：bsButton 走 inherited；否则只判 VisibleRect + OnInRealArea。</summary>
    public override bool InRange(int x, int y)
    {
        if (ButtonStyle == TButtonStyle.bsButton)
            return base.InRange(x, y);

        if (DxRectUtil.PointInRect(TDxPoint.Point(x, y), VisibleRect))
        {
            var inRange = new BoolRef(true);
            var vRect = VirtualRect;
            OnInRealArea?.Invoke(this, x - vRect.Left, y - vRect.Top, inRange);
            return inRange.Value;
        }
        return false;
    }

    /// <summary>
    /// 原文 511-529 TDxImageButton.MouseDown：先摆 PopupMenu 位置再 inherited。
    /// TDxPopupMenu 未移植 → 以 `PopupMenuHook`（上游接缝的等价物）承接。
    /// 原文 515-527 的定位次序照抄：`vRect := VirtualRect`；
    /// `if vRect.Bottom + PopupMenu.Height &gt; PopupMenu.RootCtrl.Height then Top := vRect.Top - Height
    /// else Top := vRect.Bottom`；再 `ItemIndex := -1; Left := vRect.Left; Width := Width; Visible := True`。
    /// </summary>
    public override void MouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        if (PopupMenuHook != null)
        {
            var vRect = VirtualRect;
            PopupMenuHook(this, vRect.Left, vRect.Top);
        }
        base.MouseDown(button, shift, x, y);
    }

    /// <summary>原文 531-602 TDxImageButton.DoDrawCaption 的**几何与字体选择**部分，1:1。</summary>
    public void DoDrawCaptionV2()
    {
        if (Caption == "") return;

        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;
        var vtRect = VirtualRect;

        // 原文 546-547 先 `X := 0; Y := 0`，Disabled 分支不再改它们（故 Disabled 时 X=Y=0）
        int x = 0;
        int y = 0;
        TDxFont font;
        if (Enabled)
        {
            if (MouseDowned)          // 原文 `if MouseDowned {or Checked} then`（注释掉的 Checked 照抄）
            {
                x = _captionOffsetX + _captionDownOffsetX;
                y = _captionOffsetY + _captionDownOffsetY;
                font = CaptionColor.Down;
            }
            else
            {
                x = _captionOffsetX;
                y = _captionOffsetY;
                font = MouseMoveed ? CaptionColor.Hot : CaptionColor.Up;
            }
        }
        else
        {
            font = CaptionColor.Disabled;
        }

        // 原文 568-574（被注释掉的 bsButton 分支）：
        //   { if FButtonStyle <> bsButton then begin X := 0; Y := 0; end; }

        var hgeFont = FindFont(font);
        if (hgeFont == null) return;

        var textImages = GetImageInfos(hgeFont, Caption);

        int nWidth = 0;
        int nHeight = 0;
        for (int i = 0; i < textImages.Count; i++)
        {
            if (nWidth < textImages[i].Width) nWidth = textImages[i].Width;

            // 原文 587-590：行高为 0 时用 HGEFont.TextHeight('0') 兜底
            if (textImages[i].Height <= 0)
                nHeight += TextHeight(hgeFont, "0");
            else
                nHeight += textImages[i].Height;
        }

        if (font.Bold)
        {
            nWidth += 2;
            nHeight += 2;
        }

        // 原文 598-600：DestRect = Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight)，ExpandLineHeight = 3
        DrawCaption(hgeFont, font, textImages,
            TDxRect.Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, x, y, 3);
    }

    /// <summary>
    /// DxImageButton.pas 910-1017 TDxImageButton.Paint 的**顺序骨架**（1:1）：
    /// ① vbRect 退化直接 Exit；② 设计期画 `BorderColor.Up` 边框；
    /// ③ `Animation.DrawBeforeDef` 时先画动画；④ OnStartPaint；
    /// ⑤ OnPaint 或（有图）按 FaceIndex 取图（`Enabled ? Images : Grays`）→
    ///    taLeftJustify 时 `vRect.Left := vRect.Right - Texture.Width`；
    ///    bsButton 且 MouseDowned 时按 `CaptionOffset + ButtonDownOffset` 平移；
    ///    daFill → `DrawRect(vRect)`；daBottom → 先按纹理高把 Top 下移再 `DrawRect`；
    /// ⑥ DoDrawCaption；⑦ `not DrawBeforeDef` 时画动画；⑧ OnStopPaint。
    /// </summary>
    public void PaintImageButton()
    {
        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;
        var vtRect = VirtualRect;
        DoPaint();

        if (Designing)
        {
            // 原文 924（被注释掉）：// Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
            DxControlOps.FrameRectOne(this, vtRect, BorderColor.Up.Color);
            // 原文 926（被注释掉的调试输出）：
            //   // CurrentFont.TextOut(vtRect.Left + 1, vtRect.Top + 1, Format('Left:%d Top:%d', [...]));
        }

        if (Animation.DrawBeforeDef)
            Animation.Paint(Painter);

        DxControlHooks.GetOnStartPaint(this)?.Invoke(this);

        if (OnPaint != null)
        {
            OnPaint(this);
        }
        else if (ImageIndex.Image != null)
        {
            int faceIndex = ResolveFaceIndex();
            if (faceIndex >= 0)
            {
                var texture = Enabled
                    ? ImageIndex.Image.GetImage(faceIndex)
                    : ImageIndex.Image.GetGray(faceIndex);

                if (texture != null)
                {
                    var vRect = vtRect;
                    if (Alignment == TDxAlignment.taLeftJustify)
                    {
                        vRect.Left = vRect.Right - texture.Width;
                    }

                    if (ButtonStyle == TButtonStyle.bsButton && MouseDowned)
                    {
                        vRect.Left = vRect.Left + _captionOffsetX + _buttonDownOffsetX;
                        vRect.Top = vRect.Top + _captionOffsetY + _buttonDownOffsetY;
                    }

                    // 加入透明模式绘制 chongchong 2014-09-15
                    if (_drawAligment == TDrawAligment.daFill)
                    {
                        DxControlOps.DrawRectScoped(this, vRect, vtRect, vbRect, texture, BlendMode);
                    }
                    else
                    {
                        if (vRect.Bottom - vtRect.Top > texture.Height)
                        {
                            vRect.Top = vRect.Top + (vRect.Bottom - vRect.Top - texture.Height);
                        }
                        DxControlOps.DrawRectScoped(this, vRect, vtRect, vbRect, texture, BlendMode);
                    }
                }
            }
        }

        DoDrawCaptionV2();

        if (!Animation.DrawBeforeDef)
            Animation.Paint(Painter);

        DxControlHooks.GetOnStopPaint(this)?.Invoke(this);
    }

    /// <summary>原文 619-674 TDxImageButton.Assign(Source:TDxControl) 的几何/属性部分。</summary>
    public void AssignFromImageButton(TDxControl source)
    {
        if (source is not TDxImageButton src) return;

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

        // 原文 636（被注释掉）：// Designing := Source.Designing;
        ReferenceX = source.ReferenceX;
        AdjustYByHeight = source.AdjustYByHeight;
        TopAlignment = source.TopAlignment;

        DxControlOps.SetCenterA(this, source.Center);
        DxControlOps.SetAlign(this, source.Align);
        DxControlOps.SetAutoSize(this, source.AutoSize);
        BackgroundColor = source.BackgroundColor;
        BlendMode = src.BlendMode;

        ImageIndex.Assign(source.ImageIndex);
        BorderColor.Assign(source.BorderColor);

        ButtonDownOffsetX = src.ButtonDownOffsetX;
        ButtonDownOffsetY = src.ButtonDownOffsetY;

        Caption = src.Caption;
        Alignment = src.Alignment;

        CaptionColor.Assign(src.CaptionColor);
        CaptionDownOffsetX = src.CaptionDownOffsetX;
        CaptionDownOffsetY = src.CaptionDownOffsetY;

        CaptionOffsetX = src.CaptionOffsetX;
        CaptionOffsetY = src.CaptionOffsetY;

        SetChecked(src.Checked);
        ClickCount = src.ClickCount;
        DrawBorder = src.DrawBorder;
        HintText = src.HintText;
        DxControlHooks.SetModalControl(this, DxControlHooks.GetModalControl(src));
        MouseDownBlendMode = src.MouseDownBlendMode;
        MouseMoveBlendMode = src.MouseMoveBlendMode;
        ButtonStyle = src.ButtonStyle;

        Animation.Assign(src.Animation);
    }

    /// <summary>原文 314-315 的回调转发（TButtonAnimation 调 FOwner.FOnAnimationFrameChanged）。</summary>
    internal void RaiseAnimationFrameChanged(TDxImageButton owner, int animationIndex, int playCount, int frame)
        => OnAnimationFrameChanged?.Invoke(owner, animationIndex, playCount, frame);

    /// <summary>原文 788-791 Initialize（空实现）。</summary>
    public void InitializeButton() { }

    /// <summary>原文 793-796 Finalize（空实现）。</summary>
    public void FinalizeButton() { }

    /// <summary>原文 491-496 Destroy（释放 CaptionColor 与 Animation；托管侧由 GC 承担，此处显式释放动画）。</summary>
    public void DisposeButton()
    {
        Animation?.Dispose();
    }
}
