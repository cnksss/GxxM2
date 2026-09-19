using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

/// <summary>
/// DxImageProgress.pas 1:1 逐字移植（1-402）。
///
/// 覆盖两类内容：
///   1. TProgressSetting（23-87 / 109-225）—— 设置对象 + Changed 通知链
///   2. TDxImageProgress（89-105 / 229-400）—— 控件：构造默认值、进度几何、值文本、RecallAutoSize
///
/// 保真要点（原文易错处，逐条固化）：
///   * 构造默认值（109-141）：Font.Bold=True、Font.Color=clWhite、ImageType=Prguse_wil、
///     ImageBG=-1、ImageProgress=-1、ImageProgressX/Y=0、ValueType=vtValue、ValueSplite='-'
///     （原文拼写，保留）、ValueAlignment=taCenter、ValuePrefix/Suffix=''、
///     Max=100、Min=0、**Value=50**（不是 0）。
///   * SetImageType / SetOnGetImage 都会在**赋值之后**立刻回调一次 OnGetImage（149-165）。
///   * Changed（194-200）：**先 FOwner.RecallAutoSize，再 OnChange**（顺序不可换）。
///   * Assign（202-225）：走 `OnGetImage := ...`（属性写 → 会回调）而 ImageType 是**直写字段**
///     （不触发 OnGetImage）—— 这种不对称是原文如此。
///   * SetFont（167-170）用 FFont.**Assign**(Value)（拷贝而非替换引用；FFont 永不为 nil）。
///   * 控件构造（229-238）：Width=90、Height=20，且把 OnGetImage 接到 FProgressSetting 上。
///   * Paint（246-337）三块绘制的**四条守卫各不相同**（见 PaintTo 逐条注释）。
///   * RecallAutoSize（350-366）：AutoSize 为假直接 Exit；**要求 D.Width * D.Height &gt; 4**
///     （严格大于 4，故 1×1/2×2/1×4 都不触发）。
///
/// 接缝：GameCanvas.Draw / TextureFonts.FindFont / HGEFont（HGECanvas/HGEFontEx 未移植）
///       → IDxSurfacePainter / TDxFontEnv / IDxImageLibrary。
/// </summary>
public sealed class TProgressSetting
{
    /// <summary>DxImageProgress.pas 25 FOwner:TDxImageProgress。</summary>
    public TDxImageProgress Owner { get; internal set; }

    private readonly TDxFont _font = new();
    private TImageType _imageType = TImageType.Prguse_wil;
    private int _imageBG = -1;
    private int _imageProgress = -1;
    private int _imageProgressX;
    private int _imageProgressY;
    private TProgressValueType _valueType = TProgressValueType.vtValue;
    private string _valueSplite = "-";
    private TDxAlignment _valueAlignment = TDxAlignment.taCenter;
    private string _valuePrefix = "";
    private string _valueSuffix = "";
    private uint _max = 100;
    private uint _min;
    private uint _value = 50;

    /// <summary>DxImageProgress.pas 29 FOnChange（TNotifyEvent）。</summary>
    public Action<TProgressSetting> OnChange;

    /// <summary>DxImageProgress.pas 30 FOnGetImage（TOnGetImage）。</summary>
    public Action<TProgressSetting, TImageType, IDxImageLibrary> OnGetImage;

    /// <summary>DxImageProgress.pas 109-141 构造函数 1:1。</summary>
    public TProgressSetting(TDxImageProgress owner)
    {
        Owner = owner;

        _font.Bold = true;
        _font.Color = TDxColor.clWhite;
        _font.OnChange = FontChange;

        _imageType = TImageType.Prguse_wil;

        Owner = owner;               // 原文 121：FOwner := AOwner（重复赋值，照抄）
        _imageBG = -1;
        _imageProgress = -1;

        _imageProgressX = 0;
        _imageProgressY = 0;

        _valueType = TProgressValueType.vtValue;
        _valueSplite = "-";
        _valueAlignment = TDxAlignment.taCenter;
        _valuePrefix = "";
        _valueSuffix = "";

        _max = 100;
        _min = 0;
        _value = 50;

        Image = null;
        OnChange = null;
        OnGetImage = null;
    }

    /// <summary>DxImageProgress.pas 65 Image:TGameImages（只读属性）。</summary>
    public IDxImageLibrary Image { get; private set; }

    /// <summary>接缝 SetImage(Value)：原文由 OnGetImage 回调提供。</summary>
    public void SetImage(IDxImageLibrary value) => Image = value;

    /// <summary>DxImageProgress.pas 69 Font:TDxFont（read FFont write SetFont）。</summary>
    public TDxFont Font
    {
        get => _font;
        set => SetFont(value);
    }

    /// <summary>DxImageProgress.pas 71 / 149-157 ImageType。</summary>
    public TImageType ImageType
    {
        get => _imageType;
        set => SetImageType(value);
    }

    /// <summary>DxImageProgress.pas 72 / 178-184 ImageBG（默认 -1）。</summary>
    public int ImageBG
    {
        get => _imageBG;
        set { if (_imageBG != value) { _imageBG = value; Changed(); } }
    }

    /// <summary>DxImageProgress.pas 73 / 186-192 ImageProgress（默认 -1）。</summary>
    public int ImageProgress
    {
        get => _imageProgress;
        set { if (_imageProgress != value) { _imageProgress = value; Changed(); } }
    }

    /// <summary>DxImageProgress.pas 75 ImageProgressX（published 直写字段，无 setter）。</summary>
    public int ImageProgressX { get => _imageProgressX; set => _imageProgressX = value; }

    /// <summary>DxImageProgress.pas 76 ImageProgressY（published 直写字段）。</summary>
    public int ImageProgressY { get => _imageProgressY; set => _imageProgressY = value; }

    /// <summary>DxImageProgress.pas 78 ValueType（published 直写字段，默认 vtValue）。</summary>
    public TProgressValueType ValueType { get => _valueType; set => _valueType = value; }

    /// <summary>DxImageProgress.pas 79 ValueSplite（原文拼写；默认 '-'）。</summary>
    public string ValueSplite { get => _valueSplite; set => _valueSplite = value; }

    /// <summary>DxImageProgress.pas 80 ValueAlignment（默认 taCenter）。</summary>
    public TDxAlignment ValueAlignment { get => _valueAlignment; set => _valueAlignment = value; }

    /// <summary>DxImageProgress.pas 81 ValuePrefix（默认 ''）。</summary>
    public string ValuePrefix { get => _valuePrefix; set => _valuePrefix = value; }

    /// <summary>DxImageProgress.pas 82 ValueSuffix（默认 ''）。</summary>
    public string ValueSuffix { get => _valueSuffix; set => _valueSuffix = value; }

    /// <summary>DxImageProgress.pas 84 Max:LongWord（默认 100；published 直写字段）。</summary>
    public uint Max { get => _max; set => _max = value; }

    /// <summary>DxImageProgress.pas 85 Min:LongWord（默认 0；published 直写字段）。</summary>
    public uint Min { get => _min; set => _min = value; }

    /// <summary>DxImageProgress.pas 86 Value:LongWord（默认 50；published 直写字段）。</summary>
    public uint Value { get => _value; set => _value = value; }

    /// <summary>DxImageProgress.pas 149-157 SetImageType 1:1。</summary>
    public void SetImageType(TImageType value)
    {
        if (_imageType != value)
        {
            _imageType = value;
            if (OnGetImage != null)
                OnGetImage(this, _imageType, Image);
            Changed();
        }
    }

    /// <summary>DxImageProgress.pas 159-165 SetOnGetImage 1:1（赋值后**无条件**回调一次 + Changed）。</summary>
    public void SetOnGetImage(Action<TProgressSetting, TImageType, IDxImageLibrary> value)
    {
        OnGetImage = value;
        if (OnGetImage != null)
            OnGetImage(this, _imageType, Image);
        Changed();
    }

    /// <summary>DxImageProgress.pas 167-170 SetFont：FFont.Assign(Value)（拷贝）。</summary>
    public void SetFont(TDxFont value)
    {
        _font.Assign(value);
    }

    /// <summary>DxImageProgress.pas 172-176 FontChange(stdcall)：只转发 FOnChange。</summary>
    public void FontChange(TDxFont sender)
    {
        OnChange?.Invoke(this);
    }

    /// <summary>DxImageProgress.pas 194-200 Changed 1:1：**先** Owner.RecallAutoSize，**再** OnChange。</summary>
    public void Changed()
    {
        Owner?.RecallAutoSize();

        OnChange?.Invoke(this);
    }

    /// <summary>DxImageProgress.pas 202-225 Assign 1:1。</summary>
    public void Assign(TProgressSetting source)
    {
        if (source is TProgressSetting src)
        {
            // 原文 205：OnGetImage := TProgressSetting(Source).OnGetImage;
            //          （属性写 → 立即回调一次 → 但此时 _imageType 还是旧值）
            SetOnGetImage(src.OnGetImage);
            _imageType = src._imageType;         // 原文 206：FImageType 直写（不回调查询）

            _font.Assign(src._font);             // 原文 208：FFont.Assign

            _imageBG = src._imageBG;
            _imageProgress = src._imageProgress;

            _valueType = src._valueType;
            _valueSplite = src._valueSplite;
            _valueAlignment = src._valueAlignment;
            _valuePrefix = src._valuePrefix;
            _valueSuffix = src._valueSuffix;

            _max = src._max;
            _min = src._min;
            _value = src._value;

            Changed();
        }
    }
}

/// <summary>DxImageProgress.pas 89-105 / 229-400 TDxImageProgress 1:1。</summary>
public sealed class TDxImageProgress : TDxControl
{
    /// <summary>DxImageProgress.pas 91 FProgressSetting。</summary>
    public TProgressSetting ProgressSetting { get; }

    /// <summary>接缝：TGameImages（WIL 图库）—— 原文通过 OnGetImage 注入。</summary>
    public IDxImageLibrary ImageLibrary { get; set; }

    // DFM: TDxImageProgress 无同名 .dfm（DxImageProgress.dfm 在 Source/Client-HGE 全树不存在），
    // 布局属性的设计期取值落在各 .GUI 文件的 TguiProgress 记录里（见报告第 4 项）。

    public TDxImageProgress()
    {
        // DxImageProgress.pas 231-237 逐项
        ProgressSetting = new TProgressSetting(this);
        // 原文 234：FProgressSetting.OnGetImage := OnGetImage;（FProgressSetting 的 TOnGetImage 属性
        // 由控件的 OnGetImage 属性提供；因为 FProgressSetting 刚构造、FOnGetImage 仍为 nil，
        // 这次赋值**不会**触发回调）。此处等价：设置项先负责把图库写回，再转发控件级通知。
        ProgressSetting.OnGetImage = ProgressSettingOnGetImage;

        Width = 90;
        Height = 20;
    }

    /// <summary>ProgressSetting 查询图库时的落点：先写回 ImageLibrary，再转发控件级 OnGetImage。</summary>
    private void ProgressSettingOnGetImage(TProgressSetting sender, TImageType imageType, IDxImageLibrary image)
    {
        sender.SetImage(ImageLibrary);
        ControlOnGetImage?.Invoke(ImageIndex, imageType);
    }

    /// <summary>控件级 OnGetImage 通知的暂存（原文由 TDxControl.OnGetImage 属性持有）。</summary>
    private Action<TDxImageIndex, TImageType> ControlOnGetImage;

    /// <summary>DxImageProgress.pas 339-343 SetOnGetImage：先 inherited 再转发给 ProgressSetting。</summary>
    public void SetOnGetImage(Action<TDxImageIndex, TImageType> value)
    {
        base.SetOnGetImage(value);
        ControlOnGetImage = value;
        ProgressSetting.OnGetImage = ProgressSettingOnGetImage;
    }

    /// <summary>
    /// DxImageProgress.pas 246-337 Paint 的绘制判定 1:1（不落像素，只产出绘制请求）。
    /// 四条守卫彼此独立：
    ///   背景块    ：Image &lt;&gt; nil 且 ImageBG &gt;= 0 且取到图 → Draw(vt.Left, vt.Top, SrcRect, D)
    ///              其中 SrcRect = Rect(0, 0, Min(D.Width, Width), Min(D.Height, Height))
    ///   进度块    ：Image &lt;&gt; nil 且 ImageProgress &gt;= 0 且 Max &gt; Min 且 Value &gt;= Min 且取到图
    ///              SrcRect = Rect(0, 0, D.Width, D.Height) 后 Right := Left +
    ///              Round((Right-Left) / (Max-Min) * Min(Value, Max))
    ///   值文本块  ：**ImageBG &gt;= 0**（不是 ImageProgress！）且 Max &gt; Min 且 Value &gt;= Min
    ///   Designing ：最后 FrameRect(vt, vt, vb, BorderColor.Up.Color)
    /// </summary>
    public TDxImageProgressPaintResult PaintTo(IDxSurfacePainter painter)
    {
        var result = new TDxImageProgressPaintResult();

        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left)
        {
            // 原文如此（256）：退化时**连 Designing 边框都不画**，直接 Exit。
            return result;
        }
        var vtRect = VirtualRect;

        Painter = painter ?? Painter;

        if (ProgressSetting.Image != null)
        {
            if (ProgressSetting.ImageBG >= 0)
            {
                var d = ProgressSetting.Image.GetImage(ProgressSetting.ImageBG);
                if (d != null)
                {
                    var srcRect = TDxRect.Rect(0, 0, Math.Min(d.Width, Width), Math.Min(d.Height, Height));
                    Painter.Draw(vtRect.Left, vtRect.Top, srcRect, d);
                    result.BackgroundSrcRect = srcRect;
                    result.DrewBackground = true;
                }
            }

            if (ProgressSetting.ImageProgress >= 0
                && ProgressSetting.Max > ProgressSetting.Min
                && ProgressSetting.Value >= ProgressSetting.Min)
            {
                var d = ProgressSetting.Image.GetImage(ProgressSetting.ImageProgress);
                if (d != null)
                {
                    var srcRect = ComputeProgressSrcRect(d.Width, d.Height, ProgressSetting.Max,
                        ProgressSetting.Min, ProgressSetting.Value);
                    Painter.Draw(vtRect.Left + ProgressSetting.ImageProgressX,
                        vtRect.Top + ProgressSetting.ImageProgressY, srcRect, d);
                    result.ProgressSrcRect = srcRect;
                    result.DrewProgress = true;
                }
            }

            if (ProgressSetting.ImageBG >= 0
                && ProgressSetting.Max > ProgressSetting.Min
                && ProgressSetting.Value >= ProgressSetting.Min)
            {
                string s = ComputeValueText(ProgressSetting.ValueType, ProgressSetting.ValuePrefix,
                    ProgressSetting.ValueSuffix, ProgressSetting.ValueSplite, ProgressSetting.Max,
                    ProgressSetting.Min, ProgressSetting.Value);
                result.ValueText = s;

                if (s.Length > 0)
                {
                    var hgeFont = FindFont(ProgressSetting.Font);
                    if (hgeFont != null)
                    {
                        var textImages = GetImageInfos(hgeFont, s);

                        int nWidth = 0;
                        int nHeight = 0;
                        for (int i = 0; i <= textImages.Count - 1; i++)
                        {
                            if (nWidth < textImages[i].Width)
                                nWidth = textImages[i].Width;

                            if (textImages[i].Height <= 0)
                                nHeight += TextHeight(hgeFont, "0");
                            else
                                nHeight += textImages[i].Height;
                        }

                        if (ProgressSetting.Font.Bold)
                        {
                            nWidth += 2;
                            nHeight += 2;
                        }

                        DrawCaption(hgeFont, ProgressSetting.Font, ProgressSetting.ValueAlignment, textImages,
                            TDxRect.Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, 0, 0);
                        result.DrewValueText = true;
                    }
                }
            }
        }

        if (Designing)
        {
            Painter.FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
            result.DrewDesignFrame = true;
        }

        return result;
    }

    /// <summary>
    /// DxImageProgress.pas 275-277 的进度源矩形 1:1：
    /// SrcRect := Rect(0, 0, D.Width, D.Height);
    /// SrcRect.Right := SrcRect.Left + Round((SrcRect.Right - SrcRect.Left)
    ///                 / (Max - Min) * Min(Value, Max));
    /// 注意除法是**浮点**（/ 而非 div），Round 为银行家舍入，Min 是 Delphi Math.Min(LongWord)。
    /// </summary>
    public static TDxRect ComputeProgressSrcRect(int dWidth, int dHeight, uint max, uint min, uint value)
    {
        var srcRect = TDxRect.Rect(0, 0, dWidth, dHeight);
        srcRect.Right = srcRect.Left + TDxCanvasColorTables.DelphiRound(
            (srcRect.Right - srcRect.Left) / (double)(max - min) * Math.Min(value, max));
        return srcRect;
    }

    /// <summary>
    /// DxImageProgress.pas 284-291 的值文本 1:1（含原文的分支顺序与遗漏）：
    ///   vtValueAndMax → Prefix + Value + Splite + Max + Suffix
    ///   vtValue       → Prefix + Value + Suffix
    ///   vtPercentage  → Prefix + Format('%d%%', [Round(Value / (Max - Min) * 100)]) + Suffix
    ///   其余（**含 vtNone**）→ ''（空串 → 调用方因此不绘制）
    /// </summary>
    public static string ComputeValueText(TProgressValueType valueType, string prefix, string suffix,
        string splite, uint max, uint min, uint value)
    {
        if (valueType == TProgressValueType.vtValueAndMax)
            return prefix + value + splite + max + suffix;
        if (valueType == TProgressValueType.vtValue)
            return prefix + value + suffix;
        if (valueType == TProgressValueType.vtPercentage)
            return prefix + TDxCanvasColorTables.DelphiRound(value / (double)(max - min) * 100) + "%" + suffix;
        return "";
    }

    /// <summary>
    /// DxImageProgress.pas 350-366 RecallAutoSize 1:1。
    /// AutoSize 为假 → 直接 Exit；仅当 ImageBG &gt;= 0 且取到图且 **D.Width * D.Height &gt; 4** 才定尺。
    /// </summary>
    public void RecallAutoSize()
    {
        if (!AutoSize) return;

        if (ProgressSetting.Image != null)
        {
            if (ProgressSetting.ImageBG >= 0)
            {
                var d = ProgressSetting.Image.GetImage(ProgressSetting.ImageBG);
                if (d != null && d.Width * d.Height > 4)
                {
                    Width = d.Width;
                    Height = d.Height;
                }
            }
        }
    }

    /// <summary>DxImageProgress.pas 368-400 Assign 1:1。</summary>
    public void Assign(TDxControl source)
    {
        if (source is TDxImageProgress src)
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

            ImageIndex.Assign(src.ImageIndex);
            BorderColor.Assign(src.BorderColor);

            ProgressSetting.Assign(src.ProgressSetting);
        }
    }
}

/// <summary>TDxImageProgress.PaintTo 的绘制结果（headless 断言用）。</summary>
public sealed class TDxImageProgressPaintResult
{
    public bool DrewBackground;
    public bool DrewProgress;
    public bool DrewValueText;
    public bool DrewDesignFrame;
    public TDxRect BackgroundSrcRect;
    public TDxRect ProgressSrcRect;
    public string ValueText = "";
}
