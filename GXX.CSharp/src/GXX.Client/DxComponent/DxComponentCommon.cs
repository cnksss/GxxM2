using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GXX.Client.DxComponent;

// =====================================================================================
// 本文件是 DxComponent 控件族的“地基”接缝层。
//
// 依据：
//   * DxComponents.pas 17-56（枚举）/ 18-23（TSaveUIColor）/ 900-991（TRect 工具函数）
//   * DxControls.pas 34-150（TDxFont / TDxCaptionColor / TDxBorderColor / TDxImageIndex）
//   * DxControls.pas 152-574（TDxControl 字段与属性）/ 1799-1896（构造函数默认值）
//   * DxControls.pas 3095-3145（ReallyPaintRect）/ 3752-3850（DrawCaption）
//   * DxControls.pas 2219-2251（GetVisibleRect / GetVirtualRect）
//   * HGEFontEx.pas 16-23（TImageInfo/TImageInfos）/ 474-502（TextWidth）/ 448-472（TextHeight）
//
// 未移植的依赖绝不顺手移植：TDxControlEngine（控件树/焦点/绘制遍历）、THGEFont（字形图集）、
// GameImages（WIL 库）在本文件里只以“接缝”形态出现，形如 IDxSurfacePainter / TDxFontLookupFn。
// =====================================================================================

/// <summary>Delphi Types.TRect（4 个 int，Left/Top/Right/Bottom 右/下为闭区间）。</summary>
public struct TDxRect : IEquatable<TDxRect>
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public TDxRect(int left, int top, int right, int bottom)
    {
        Left = left; Top = top; Right = right; Bottom = bottom;
    }

    /// <summary>Delphi Windows.Rect。</summary>
    public static TDxRect Rect(int left, int top, int right, int bottom) => new(left, top, right, bottom);

    /// <summary>Delphi Windows.Bounds(Left, Top, Width, Height) → Right/Bottom 为左+宽、上+高。</summary>
    public static TDxRect Bounds(int left, int top, int width, int height)
        => new(left, top, left + width, top + height);

    public static readonly TDxRect Empty = new(0, 0, 0, 0);

    public readonly int Width => Right - Left;
    public readonly int Height => Bottom - Top;

    public readonly bool Equals(TDxRect other)
        => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;

    public override readonly bool Equals(object obj) => obj is TDxRect r && Equals(r);
    public override readonly int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
    public override readonly string ToString() => $"({Left},{Top},{Right},{Bottom})";
}

/// <summary>Delphi Types.TPoint。</summary>
public struct TDxPoint : IEquatable<TDxPoint>
{
    public int X;
    public int Y;

    public TDxPoint(int x, int y) { X = x; Y = y; }

    public static TDxPoint Point(int x, int y) => new(x, y);

    public readonly bool Equals(TDxPoint other) => X == other.X && Y == other.Y;
    public override readonly bool Equals(object obj) => obj is TDxPoint p && Equals(p);
    public override readonly int GetHashCode() => HashCode.Combine(X, Y);
    public override readonly string ToString() => $"({X},{Y})";
}

/// <summary>TColor（Delphi $00BBGGRR 的 32 位整数形态）。</summary>
public static class TDxColor
{
    public const int clBlack = 0x000000;
    public const int clWhite = 0xFFFFFF;
    public const int clGray = 0x808080;
    public const int clBtnFace = 0xF0F0F0;
    public const int clRed = 0x0000FF;      // Delphi clRed（BGR）
    public const int clSilver = 0xC0C0C0;
}

/// <summary>DxComponents.pas 900-991 —— TRect 工具函数 1:1（DxComponents 实现段）。</summary>
public static class DxRectUtil
{
    /// <summary>DxComponents.pas 934-940：交集（就地取 Max/Min，不判空）。</summary>
    public static TDxRect ShortRect(TDxRect rect1, TDxRect rect2)
        => new(Math.Max(rect1.Left, rect2.Left), Math.Max(rect1.Top, rect2.Top),
               Math.Min(rect1.Right, rect2.Right), Math.Min(rect1.Bottom, rect2.Bottom));

    /// <summary>DxComponents.pas 943-949：并集（Min/Max）。</summary>
    public static TDxRect LongRect(TDxRect rect1, TDxRect rect2)
        => new(Math.Min(rect1.Left, rect2.Left), Math.Min(rect1.Top, rect2.Top),
               Math.Max(rect1.Right, rect2.Right), Math.Max(rect1.Bottom, rect2.Bottom));

    /// <summary>DxComponents.pas 952-958：整体平移。</summary>
    public static TDxRect MoveRect(TDxRect rect, TDxPoint point)
        => new(rect.Left + point.X, rect.Top + point.Y, rect.Right + point.X, rect.Bottom + point.Y);

    /// <summary>DxComponents.pas 961-965：命中判定 —— **四边皆为闭区间**（Right/Bottom 也算在内）。</summary>
    public static bool PointInRect(TDxPoint point, TDxRect rect)
        => point.X >= rect.Left && point.X <= rect.Right && point.Y >= rect.Top && point.Y <= rect.Bottom;

    /// <summary>DxComponents.pas 969-975：四边向内收缩 hIn/vIn。</summary>
    public static TDxRect ShrinkRect(TDxRect rect, int hIn, int vIn)
        => new(rect.Left + hIn, rect.Top + vIn, rect.Right - hIn, rect.Bottom - vIn);

    /// <summary>DxComponents.pas 979-983：rect1 完整落在 rect2 内（闭区间）。</summary>
    public static bool RectInRect(TDxRect rect1, TDxRect rect2)
        => rect1.Left >= rect2.Left && rect1.Right <= rect2.Right
        && rect1.Top >= rect2.Top && rect1.Bottom <= rect2.Bottom;

    /// <summary>DxComponents.pas 987-991：真重叠（严格不等）。</summary>
    public static bool OverlapRect(TDxRect rect1, TDxRect rect2)
        => rect1.Left < rect2.Right && rect1.Right > rect2.Left
        && rect1.Top < rect2.Bottom && rect1.Bottom > rect2.Top;
}

// -------------------------------------------------------------------------------------
// 枚举（DxComponents.pas 27-54）
// -------------------------------------------------------------------------------------

/// <summary>DxComponents.pas 27 TClientVersion。</summary>
public enum TClientVersion
{
    cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205,
}

/// <summary>DxComponents.pas 28 TReferenceX。</summary>
public enum TReferenceX { rxLeft, rxCenter, rxRight }

/// <summary>DxComponents.pas 30 TButtonAnimationShowType。</summary>
public enum TButtonAnimationShowType
{
    astAlwaysShow, astNormalShow, astHotShow, astDownShow, astCheckShow, astEnableShow, astDisableShow,
}

/// <summary>DxComponents.pas 33-36 TGuiType（顺序即数值）。</summary>
public enum TGuiType
{
    t_None, t_Form, t_Button, t_Edit, t_Label, t_Grid,
    t_ScrollBox, t_ChatMemo, t_PopupMenu, t_ComboBox, t_PageControl, t_TabSheet,
    t_TreeView, t_ListView, t_Line, t_FormShape, t_ImageEdit,
    t_TrackBar, t_MainBottomForm, t_MagicBall, t_SexPanel, t_GroupAttackProgress, t_ImageProgress, t_SwitchButton,
}

/// <summary>DxComponents.pas 39-41 TImageType（一行一条，顺序即数值）。</summary>
public enum TImageType
{
    Prguse_wil, Prguse2_wil, Prguse3_wil, ChrSel_wil, Prguse_16_wil, Prguse2_16_wil, Prguse3_16_wil, ChrSel_16_wil,
    UI_wil, UI1_wil, UI2_wil, Prguse_wis, Prguse2_wis, Prguse3_wis, NewopUI_Pak, UI3_wil, UIN_wil, NSelect_wil, UICommon_wil,
    NewUI1_PAK, NewUI2_PAK, NewUI3_PAK, NewUI4_PAK, NewUI5_PAK, Mobile_Pak,
}

/// <summary>DxComponents.pas 44 TMouseEvents = set of TMouseButton。</summary>
[Flags]
public enum TMouseEvents
{
    None = 0,
    mbLeft = 1,
    mbRight = 2,
    mbMiddle = 4,
    /// <summary>原文构造默认值 [mbLeft, mbRight, mbMiddle]。</summary>
    Default = mbLeft | mbRight | mbMiddle,
}

/// <summary>VCL Controls.TMouseButton（原文引用）。</summary>
public enum TDxMouseButton { mbLeft, mbRight, mbMiddle }

/// <summary>VCL Controls.TShiftState = set of (ssShift, ssAlt, ssCtrl, ssLeft, ssRight, ssMiddle, ssDouble)。</summary>
[Flags]
public enum TDxShiftState
{
    None = 0,
    ssShift = 1,
    ssAlt = 2,
    ssCtrl = 4,
    ssLeft = 8,
    ssRight = 16,
    ssMiddle = 32,
    ssDouble = 64,
}

/// <summary>DxComponents.pas 48 TProgressValueType。</summary>
public enum TProgressValueType { vtNone, vtValueAndMax, vtValue, vtPercentage }

/// <summary>DxComponents.pas 51 TLineStyle。</summary>
public enum TLineStyle { lsNone, lsHorizontal, lsVertical, lsCircle, lsTriangle }

/// <summary>DxComponents.pas 53 TButtonStyle。</summary>
public enum TButtonStyle { bsButton, bsRadio, bsCheckBox }

/// <summary>VCL Controls.TAlignment。</summary>
public enum TDxAlignment { taLeftJustify, taRightJustify, taCenter }

/// <summary>VCL Controls.TAlign（TProgressSetting 未用到，TDxControl.Align 用）。</summary>
public enum TDxAlign { alNone, alTop, alBottom, alLeft, alRight, alClient, alCustom }

// -------------------------------------------------------------------------------------
// 字体 / 颜色 / 图片索引（DxControls.pas 34-150）
// -------------------------------------------------------------------------------------

/// <summary>DxControls.pas 34-67 / 772-786 TDxFont（默认值逐项照抄：clWhite/clBlack/''/[]/9/False）。</summary>
public sealed class TDxFont
{
    private int _color = TDxColor.clWhite;
    private int _bColor = TDxColor.clBlack;
    private string _name = "";                  // 原文注释掉的默认 '宋体'，实际赋 ''
    private int _size = 9;
    private bool _bold;
    private readonly HashSet<string> _style = new();   // TFontStyles（原文 []）

    /// <summary>字体变更回调（原文 TDxFont.OnChange）。</summary>
    public Action<TDxFont> OnChange;

    public int Color { get => _color; set { if (_color != value) { _color = value; Changed(); } } }
    public int BColor { get => _bColor; set { if (_bColor != value) { _bColor = value; Changed(); } } }
    public string Name { get => _name; set { if (_name != value) { _name = value; Changed(); } } }
    public int Size { get => _size; set { if (_size != value) { _size = value; Changed(); } } }
    public bool Bold { get => _bold; set { if (_bold != value) { _bold = value; Changed(); } } }

    /// <summary>原文 Style:TFontStyles（测试用最小集合接缝）。</summary>
    public IReadOnlyCollection<string> Style => _style;
    public void ClearStyle() { if (_style.Count > 0) { _style.Clear(); Changed(); } }

    /// <summary>原文 Assign 里的 Style 拷贝落点（TFontStyles 赋值，见 TDxFont.StyleSet）。</summary>
    private void SetStyle(IReadOnlyCollection<string> value)
    {
        _style.Clear();
        if (value != null)
            foreach (var s in value) _style.Add(s);
        Changed();
    }

    /// <summary>
    /// 接缝：文本横向对齐。原文 TDxControl.DrawCaption 的 9 参重载内部取 Self.Alignment；
    /// TDxImageButton 构造里 Alignment := taCenter，故按钮族默认居中。此字段是等价落点。
    /// </summary>
    public TDxAlignment Alignment = TDxAlignment.taLeftJustify;

    /// <summary>DxControls.pas 786-790 TDxFont.Changed。</summary>
    public void Changed() => OnChange?.Invoke(this);

    /// <summary>DxControls.pas（TDxFont.Assign）：逐属性拷贝（走 setter → 逐项触发 Changed）。</summary>
    public void Assign(TDxFont source)
    {
        if (source == null) return;
        Name = source.Name;
        Size = source.Size;
        SetStyle(source.Style);
        Bold = source.Bold;
        Color = source.Color;
        BColor = source.BColor;
        Alignment = source.Alignment;
    }

    /// <summary>原文 Assign 里的 Style 拷贝落点（TFontStyles 赋值）。</summary>
    public IReadOnlyCollection<string> StyleSet
    {
        set
        {
            _style.Clear();
            if (value != null)
                foreach (var s in value) _style.Add(s);
            Changed();
        }
    }

    public TDxFont Clone()
    {
        var f = new TDxFont
        {
            _name = _name, _size = _size, _bold = _bold, _color = _color, _bColor = _bColor,
            Alignment = Alignment,
        };
        foreach (var s in _style) f._style.Add(s);
        return f;
    }

    /// <summary>值相等（测试差异断言用；原文无此方法）。</summary>
    public bool ValueEquals(TDxFont other)
        => other != null && Name == other.Name && Size == other.Size && Bold == other.Bold
        && Color == other.Color && BColor == other.BColor && Alignment == other.Alignment;

    public override string ToString() => $"{Name}/{Size}/Bold={Bold}/Color={Color:X6}";
}

/// <summary>
/// DxControls.pas 69-98 TDxCaptionColor。Up/Hot/Down/Checked/Disabled 五个字体，
/// 构造里全部 Bold := True，Color 依次 clWhite×4 + clBtnFace。
/// </summary>
public class TDxCaptionColor
{
    private TDxFont _up;
    private TDxFont _hot;
    private TDxFont _down;
    private TDxFont _checked;
    private TDxFont _disabled;

    /// <summary>原文 OnChange（子字体变更时冒泡）。</summary>
    public Action<TDxCaptionColor> OnChange;

    public TDxCaptionColor()
    {
        _up = new TDxFont();
        _hot = new TDxFont();
        _down = new TDxFont();
        _checked = new TDxFont();
        _disabled = new TDxFont();

        _up.Bold = true;
        _hot.Bold = true;
        _down.Bold = true;
        _checked.Bold = true;
        _disabled.Bold = true;

        _up.Color = TDxColor.clWhite;
        _hot.Color = TDxColor.clWhite;
        _down.Color = TDxColor.clWhite;
        _checked.Color = TDxColor.clWhite;
        _disabled.Color = TDxColor.clBtnFace;

        _up.OnChange = FontChange;
        _hot.OnChange = FontChange;
        _down.OnChange = FontChange;
        _checked.OnChange = FontChange;
        _disabled.OnChange = FontChange;
    }

    /// <summary>DxControls.pas FontChange —— 只冒泡，不改值。</summary>
    public void FontChange(TDxFont sender) => OnChange?.Invoke(this);

    public TDxFont Up
    {
        get => _up;
        set { if (!ReferenceEquals(_up, value)) { _up = value ?? new TDxFont(); _up.OnChange = FontChange; OnChange?.Invoke(this); } }
    }

    public TDxFont Hot
    {
        get => _hot;
        set { if (!ReferenceEquals(_hot, value)) { _hot = value ?? new TDxFont(); _hot.OnChange = FontChange; OnChange?.Invoke(this); } }
    }

    public TDxFont Down
    {
        get => _down;
        set { if (!ReferenceEquals(_down, value)) { _down = value ?? new TDxFont(); _down.OnChange = FontChange; OnChange?.Invoke(this); } }
    }

    public TDxFont Checked
    {
        get => _checked;
        set { if (!ReferenceEquals(_checked, value)) { _checked = value ?? new TDxFont(); _checked.OnChange = FontChange; OnChange?.Invoke(this); } }
    }

    public TDxFont Disabled
    {
        get => _disabled;
        set { if (!ReferenceEquals(_disabled, value)) { _disabled = value ?? new TDxFont(); _disabled.OnChange = FontChange; OnChange?.Invoke(this); } }
    }

    /// <summary>原文 Assign：逐字体 Assign（颜色/粗体等全部覆盖）。</summary>
    public virtual void Assign(TDxCaptionColor source)
    {
        if (source == null) return;
        _up.Assign(source.Up);
        _hot.Assign(source.Hot);
        _down.Assign(source.Down);
        _checked.Assign(source.Checked);
        _disabled.Assign(source.Disabled);
        OnChange?.Invoke(this);
    }
}

/// <summary>
/// DxControls.pas 100-103 / 940-952 TDxBorderColor = TDxCaptionColor + 构造覆盖
/// （Up=$00608490/Hot=$005894B8/Down=$005894B8 全部 Bold=False，Disabled=clBtnFace）。
/// </summary>
public sealed class TDxBorderColor : TDxCaptionColor
{
    public TDxBorderColor()
    {
        Up.Color = 0x00608490;
        Up.Bold = false;
        Hot.Color = 0x005894B8;
        Hot.Bold = false;
        Down.Color = 0x005894B8;
        Down.Bold = false;
        Disabled.Color = TDxColor.clBtnFace;
        Disabled.Bold = false;
    }
}

/// <summary>
/// DxControls.pas 105-150 / 954-970 TDxImageIndex。默认五项索引全 -1（=无图），
/// ImageType 默认 Prguse_wil，Image/OnGetImage 默认 nil。
/// </summary>
public sealed class TDxImageIndex
{
    private int _up = -1;
    private int _hot = -1;
    private int _down = -1;
    private int _checked = -1;
    private int _disabled = -1;
    private int _offsetX;
    private int _offsetY;
    private int _updateValue;

    public Action<TDxImageIndex> OnChange;
    public Action<TDxImageIndex, TImageType> OnGetImage;

    /// <summary>接缝：TGameImages（WIL 图库）尚未移植，用最小图库接口承接。</summary>
    public IDxImageLibrary Image;

    public TImageType ImageType { get; set; } = TImageType.Prguse_wil;

    public int Up { get => _up; set { if (_up != value) { _up = value; Changed(); } } }
    public int Hot { get => _hot; set { if (_hot != value) { _hot = value; Changed(); } } }
    public int Down { get => _down; set { if (_down != value) { _down = value; Changed(); } } }
    public int Checked { get => _checked; set { if (_checked != value) { _checked = value; Changed(); } } }
    public int Disabled { get => _disabled; set { if (_disabled != value) { _disabled = value; Changed(); } } }
    public int OffsetX { get => _offsetX; set { if (_offsetX != value) { _offsetX = value; Changed(); } } }
    public int OffsetY { get => _offsetY; set { if (_offsetY != value) { _offsetY = value; Changed(); } } }

    /// <summary>DxControls.pas SetOnGetImage：赋值后若已挂则**立即回调一次**。</summary>
    public void SetOnGetImage(Action<TDxImageIndex, TImageType> value)
    {
        OnGetImage = value;
        if (OnGetImage != null)
            OnGetImage(this, ImageType);
        Changed();
    }

    /// <summary>DxControls.pas SetImageType：变更后若已挂回调则**回调一次**，再 Changed。</summary>
    public void SetImageType(TImageType value)
    {
        if (ImageType != value)
        {
            ImageType = value;
            if (OnGetImage != null)
                OnGetImage(this, ImageType);
            Changed();
        }
    }

    /// <summary>DxControls.pas TDxImageIndex.Changed。</summary>
    public void Changed()
    {
        if (_updateValue == 0)
            OnChange?.Invoke(this);
    }

    /// <summary>DxControls.pas BeginUpdate。</summary>
    public void BeginUpdate() => _updateValue++;

    /// <summary>DxControls.pas EndUpdate：递减到 0 才 Changed。</summary>
    public void EndUpdate()
    {
        if (_updateValue > 0)
            _updateValue--;
        if (_updateValue == 0)
            Changed();
    }

    /// <summary>原文 Assign：逐项拷贝（ImageType 走字段直写，不触发 OnGetImage）。</summary>
    public void Assign(TDxImageIndex source)
    {
        if (source == null) return;
        ImageType = source.ImageType;
        Up = source.Up;
        Hot = source.Hot;
        Down = source.Down;
        Checked = source.Checked;
        Disabled = source.Disabled;
        OffsetX = source.OffsetX;
        OffsetY = source.OffsetY;
    }
}

// -------------------------------------------------------------------------------------
// 接缝：绘制 / 字体 / 图库
// -------------------------------------------------------------------------------------

/// <summary>
/// 接缝：纹理（TTexture / TGameImages.Images[]）尚未移植时的最小面。
/// Width/Height 对应原文 TTexture.Width/Height。
/// </summary>
public interface IDxTexture
{
    int Width { get; }
    int Height { get; }
    /// <summary>原文 TTexture.ClientRect = Rect(0,0,Width,Height)。</summary>
    TDxRect ClientRect { get; }
}

/// <summary>接缝：TGameImages（WIL 图库）最小面 —— Images[] / Grays[]。</summary>
public interface IDxImageLibrary
{
    IDxTexture GetImage(int index);
    IDxTexture GetGray(int index);
}

/// <summary>最小 IDxTexture 实现（测试与接缝默认用）。</summary>
public sealed class TDxTextureStub : IDxTexture
{
    public int Width { get; }
    public int Height { get; }
    public TDxRect ClientRect => TDxRect.Rect(0, 0, Width, Height);

    public TDxTextureStub(int width, int height) { Width = width; Height = height; }
}

/// <summary>最小 IDxImageLibrary 实现（数组式图库，测试用）。</summary>
public sealed class TDxImageLibraryStub : IDxImageLibrary
{
    private readonly List<IDxTexture> _images = new();
    private readonly List<IDxTexture> _grays = new();

    public IDxTexture GetImage(int index)
        => index >= 0 && index < _images.Count ? _images[index] : null;

    public IDxTexture GetGray(int index)
        => index >= 0 && index < _grays.Count ? _grays[index] : null;

    public TDxImageLibraryStub Add(IDxTexture texture) { _images.Add(texture); _grays.Add(texture); return this; }
    public TDxImageLibraryStub AddGray(IDxTexture texture) { _grays.Add(texture); return this; }
}

/// <summary>
/// HGEFontEx.pas 16-20 TImageInfo（单行文本的字形/尺寸信息）。
/// </summary>
public sealed class TDxTextImageInfo
{
    public int Width;
    public int Height;
    public readonly List<int> ImageIndexs = new();

    public TDxTextImageInfo() { }

    public TDxTextImageInfo(int width, int height)
    {
        Width = width;
        Height = height;
    }
}

/// <summary>
/// 接缝：THGEFont（字形图集）在本批未移植，控件只需以下四件事：
/// TextHeight / TextWidth / GetImageInfos / FindFont。全部以委托注入。
/// </summary>
public sealed class TDxFontEnv
{
    /// <summary>界面上可用的字形字体（原文 TextureFonts.FindFont(Name, Size, Style)）。</summary>
    public Func<string, int, IReadOnlyCollection<string>, object> FindFont;

    /// <summary>THGEFont.TextHeight（HGEFontEx.pas 448-472）。</summary>
    public Func<object, string, int> TextHeight;

    /// <summary>THGEFont.TextWidth（HGEFontEx.pas 474-502）。</summary>
    public Func<object, string, int> TextWidth;

    /// <summary>THGEFont.GetImageInfos（HGEFontEx.pas 693-769）：一行一项。</summary>
    public Func<object, string, List<TDxTextImageInfo>> GetImageInfos;

    /// <summary>
    /// HGEFontEx.pas 51-54 / 178-180 的默认字模尺寸（FFontWidth=6 / FFontHeight=12 /
    /// FDoubleFontWidth=12）—— 依据 TextWidth(474-502) 与 TextHeight(448-472) 的逐字公式，
    /// 作为无注入时的缺省度量，保证几何公式可独立单测。
    /// </summary>
    public const int DefaultFontWidth = 6;
    public const int DefaultFontHeight = 12;
    public const int DefaultDoubleFontWidth = 12;
    public const int DefaultDoubleFontHeight = 12;

    /// <summary>
    /// TextWidth 的纯公式实现（HGEFontEx.pas 488-502 1:1）。
    /// 原文：nsCount := Length(Text)（AnsiString 字节数）、nwCount := Length(WideString(Text))；
    /// 二者相等（纯 ASCII）→ FFontWidth * Length(Text)；
    /// 否则 nCount := nsCount - nwCount（双字节字符数），
    /// 结果 = FDoubleFontWidth * Max(nCount,0) + Max(nwCount - nCount, 0) * FFontWidth。
    /// 本移植的 string 已是 Unicode，nwCount 恒等于文本长度（UTF-16 码元数）；
    /// GBK 原文按字节计数，故此处把 nsCount 取为 GBK 字节数（GXX.Core.EncodingInit.GBK）。
    /// </summary>
    public static int MeasureTextWidth(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        int nsCount = GXX.Core.EncodingInit.GBK.GetByteCount(text);   // AnsiString 字节数
        int nwCount = text.Length;                                    // WideString 码元数
        if (nsCount == nwCount)
            return DefaultFontWidth * text.Length;

        int nCount = nsCount - nwCount;   // 双字节字符数
        return DefaultDoubleFontWidth * Math.Max(nCount, 0)
             + Math.Max(nwCount - nCount, 0) * DefaultFontWidth;
    }

    /// <summary>
    /// TextHeight 的纯公式实现（HGEFontEx.pas 462-472 1:1）：
    /// `if Length(Text) = Length(WideString(Text)) then FFontHeight else Max(FFontHeight, FDoubleFontHeight)`。
    /// 纯 ASCII → FFontHeight(12)；含双字节字符 → Max(12, FDoubleFontHeight=12) = 12。
    /// </summary>
    public static int MeasureTextHeight(string text)
    {
        int nsCount = string.IsNullOrEmpty(text) ? 0 : GXX.Core.EncodingInit.GBK.GetByteCount(text);
        int nwCount = text?.Length ?? 0;
        if (nsCount == nwCount)
            return DefaultFontHeight;
        return Math.Max(DefaultFontHeight, DefaultDoubleFontHeight);
    }

    /// <summary>
    /// GetImageInfos 的纯实现（HGEFontEx.pas 703-762）：按行拆分，每行一项；
    /// 空行也占一项（且该行有 Length(LineText)&gt;0 守卫，故空的项 Width/Height 保持 0）。
    /// 每行宽度 = Σ 单字宽度、行高 = 单字高度最大值。
    /// </summary>
    public static List<TDxTextImageInfo> BuildImageInfos(string text)
    {
        var result = new List<TDxTextImageInfo>();
        if (text == null) return result;

        var lines = SplitTextLines(text);
        foreach (var line in lines)
        {
            var info = new TDxTextImageInfo();
            if (line.Length > 0)
            {
                foreach (var ch in line)
                {
                    // 逐字形：宽度累加、高度取最大（HGEFontEx.pas 733-734）
                    int w = ch > 0xFF ? DefaultDoubleFontWidth
                          : ch > 0x7F ? DefaultDoubleFontWidth
                          : DefaultFontWidth;
                    info.Width += Math.Max(w, 0);
                    info.Height = Math.Max(info.Height, DefaultFontHeight);
                    info.ImageIndexs.Add(-1);
                }
            }
            result.Add(info);
        }
        return result;
    }

    /// <summary>
    /// Delphi Classes.TStringList.Text 的行拆分：以 #13#10 / #13 / #10 断行；
    /// 结尾换行不产生额外空行（TStringList.SetTextStr 语义）。
    /// </summary>
    public static List<string> SplitTextLines(string text)
    {
        var lines = new List<string>();
        if (string.IsNullOrEmpty(text)) return lines;

        int start = 0;
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '\r')
            {
                lines.Add(text.Substring(start, i - start));
                i++;
                if (i < text.Length && text[i] == '\n') i++;
                start = i;
            }
            else if (text[i] == '\n')
            {
                lines.Add(text.Substring(start, i - start));
                i++;
                start = i;
            }
            else i++;
        }
        if (start < text.Length)
            lines.Add(text.Substring(start));
        return lines;
    }
}

/// <summary>
/// 接缝：DxCanvas / HGECanvas 的实际绘制面。控件只把「要画什么」交给它，
/// 不在本车道实现 GDI+/DirectX 光栅化（原文 GameCanvas.* / HGEFont.TextRect 的等价物）。
/// </summary>
public interface IDxSurfacePainter
{
    /// <summary>放行一次绘制前的守卫（原文 GameCanvas.Active / Initialized）。</summary>
    bool Active { get; }

    /// <summary>TDxControl.FillRect（DxControls.pas 3246-3285）。</summary>
    void FillRect(TDxRect destRect, TDxRect virtualRect, TDxRect visibleRect, int color);

    /// <summary>TDxControl.FrameRect（DxControls.pas 3330-3393）。</summary>
    void FrameRect(TDxRect destRect, TDxRect virtualRect, TDxRect visibleRect, int color);

    /// <summary>TDxControl.DrawRect（DxControls.pas 3445-3494）。</summary>
    void DrawRect(TDxRect destRect, TDxRect virtualRect, TDxRect visibleRect, IDxTexture texture, int blendMode);

    /// <summary>GameCanvas.Draw(x, y, SrcRect, Texture)（DxImageProgress / DxCanvas 用）。</summary>
    void Draw(int x, int y, TDxRect srcRect, IDxTexture texture);

    /// <summary>GameCanvas.Draw(x, y, Texture, BlendMode)（DXTrackBar 滑块用）。</summary>
    void Draw(int x, int y, IDxTexture texture, int blendMode);

    /// <summary>GameCanvas.Line(Pt1, Pt2, Color)（DXTrackBar / DxLine 用）。</summary>
    void Line(TDxPoint pt1, TDxPoint pt2, int color);

    /// <summary>GameCanvas.Circle(x, y, radius, Color)（DxLine 用）。</summary>
    void Circle(int x, int y, int radius, int color);

    /// <summary>
    /// HGEFont.TextRect（HGEFontEx.pas 834+）。Bold 时原文先画 4 次偏移描边再画正文，故这里一次调用
    /// 只表达「一次 TextRect」；偏移序列由 TDxControl.DrawCaption 逐次发出。
    /// </summary>
    void TextRect(int x, int y, TDxRect srcRect, List<TDxTextImageInfo> imageInfos, int color,
        int blendMode, byte alpha, int expandLineHeight);
}

/// <summary>默认空绘制器（headless 测试与未接线时的安全落点）。</summary>
public sealed class TDxNullPainter : IDxSurfacePainter
{
    public bool Active => false;
    public void FillRect(TDxRect d, TDxRect v, TDxRect vb, int c) { }
    public void FrameRect(TDxRect d, TDxRect v, TDxRect vb, int c) { }
    public void DrawRect(TDxRect d, TDxRect v, TDxRect vb, IDxTexture t, int b) { }
    public void Draw(int x, int y, TDxRect src, IDxTexture t) { }
    public void Draw(int x, int y, IDxTexture t, int b) { }
    public void Line(TDxPoint p1, TDxPoint p2, int c) { }
    public void Circle(int x, int y, int r, int c) { }
    public void TextRect(int x, int y, TDxRect src, List<TDxTextImageInfo> infos, int c, int b, byte a, int e) { }
}

/// <summary>
/// 记录式绘制器（headless 断言用）：把每次绘制调用记成可比较的操作序列。
/// </summary>
public sealed class TDxRecordingPainter : IDxSurfacePainter
{
    public bool Active { get; set; } = true;

    public readonly List<string> Ops = new();

    public void Clear() => Ops.Clear();

    public void FillRect(TDxRect d, TDxRect v, TDxRect vb, int c)
        => Ops.Add($"FillRect({d},{v},{vb},{c:X6})");

    public void FrameRect(TDxRect d, TDxRect v, TDxRect vb, int c)
        => Ops.Add($"FrameRect({d},{v},{vb},{c:X6})");

    public void DrawRect(TDxRect d, TDxRect v, TDxRect vb, IDxTexture t, int b)
        => Ops.Add($"DrawRect({d},{v},{vb},tex{(t == null ? "-" : $"{t.Width}x{t.Height}")},{b})");

    public void Draw(int x, int y, TDxRect src, IDxTexture t)
        => Ops.Add($"Draw({x},{y},{src},tex{(t == null ? "-" : $"{t.Width}x{t.Height}")})");

    public void Draw(int x, int y, IDxTexture t, int b)
        => Ops.Add($"Draw({x},{y},tex{(t == null ? "-" : $"{t.Width}x{t.Height}")},{b})");

    public void Line(TDxPoint p1, TDxPoint p2, int c)
        => Ops.Add($"Line({p1},{p2},{c:X6})");

    public void Circle(int x, int y, int r, int c)
        => Ops.Add($"Circle({x},{y},{r},{c:X6})");

    public void TextRect(int x, int y, TDxRect src, List<TDxTextImageInfo> infos, int c, int b, byte a, int e)
        => Ops.Add($"TextRect({x},{y},{src},n={(infos?.Count ?? 0)},{c:X6},{b},{a},{e})");
}

// -------------------------------------------------------------------------------------
// TDxControl 基类
// -------------------------------------------------------------------------------------

/// <summary>
/// DxControls.pas 152-574 TDxControl 的**几何与状态**部分（WinForms Control 派生）。
///
/// 已 1:1 落地的部分：
///   * 构造函数默认值（1799-1896）：ClientRect=Bounds(0,0,0,0)、Visible/Enabled=True、
///     MouseEvents=[mbLeft,mbRight,mbMiddle]、Caption=''、ReferenceX=rxLeft、
///     AutoSize=True、Alignment=taLeftJustify、Transparent=True、BackgroundColor=clWhite、
///     DrawBorder=False、EnableFocus=False、TabOrder=0、Align=alNone、Center=False、OwnerMove=False、
///     BlendMode=Blend_Default。
///   * VisibleRect / VirtualRect（2219-2251）。
///   * ReallyPaintRect（3095-3145）/ ReallyRect（3199-3205）/ CanDraw（3208-3223）。
///   * DrawCaption（3752-3850，含 Bold 四次偏移描边序列）。
///   * SetPosition/GetPosition（2375-2411）：Left/Top 平移不改尺寸，Width/Height 只改右下。
///   * CheckAutoSize（2123-2138）。
///
/// 接缝：TDxControlEngine（控件树/焦点/输入派发）、THGEFont、GameImages 均以接口注入，
/// 详见 IDxSurfacePainter / TDxFontEnv。
/// </summary>
public abstract class TDxControl : Control
{
    // ---- 原文私有字段（DxControls.pas 156-241）----
    protected TDxRect FClientRect = TDxRect.Empty;      // FClientRect（原点是父容器坐标）
    private bool _visible = true;                       // FVisible
    private bool _enabled = true;                       // FEnabled
    private bool _designing = true;                      // FDesigning（原文构造 := True）
    private bool _floating;                              // FFloating
    private TMouseEvents _mouseEvents = TMouseEvents.Default;
    private bool _canMouse = true;                       // FCanMouse
    private bool _enableMouse = true;                    // FEnableMouse
    private bool _transparent = true;                     // FTransparent
    private int _backgroundColor = TDxColor.clWhite;      // FBackgroundColor
    private bool _drawBorder;                             // FDrawBorder
    private bool _autoSize = true;                         // FAutoSize
    private TDxAlignment _alignment = TDxAlignment.taLeftJustify;   // FAlignment
    private TDxAlign _align = TDxAlign.alNone;             // FAlign
    private bool _center;                                  // FCenter
    private bool _ownerMove;                               // FOwnerMove
    private bool _enableFocus;                             // FEnableFocus
    private int _tabOrder;                                 // FTabOrder
    private int _blendMode;                                // FBlendMode = Blend_Default
    private int _mouseDownBlendMode;                       // FMouseDownBlendMode
    private int _mouseMoveBlendMode;                       // FMouseMoveBlendMode
    private int _controlID;                                // FControlID
    private int _autoSizeSetFlag;                          // FAutoSizeSetFlag（HZQ 20230609）
    private TReferenceX _referenceX = TReferenceX.rxLeft;  // FReferenceX
    private bool _adjustYByHeight;                         // FAdjustYByHeight
    private bool _topAlignment;                            // FTopAlignment
    private string _caption = "";                          // FCaption
    private string _showName = "";                         // FShowName
    private string _hint = "";                             // FHint
    private bool _mouseDowned;                             // FMouseDowned
    private bool _mouseMoveed;                             // FMouseMoveed
    private bool _checked;                                 // FChecked（TDxImageButton 段）

    /// <summary>GameCanvas.Active（绘制守卫）。</summary>
    protected IDxSurfacePainter Painter = new TDxNullPainter();

    /// <summary>字体/字形接缝。</summary>
    public TDxFontEnv FontEnv = new();

    /// <summary>父控件（原文 FOwner，用于 VisibleRect/VirtualRect 递推；避免与 Control.Owner 撞名）。</summary>
    public TDxControl DxOwner;

    public TDxControl()
    {
        // DxControls.pas 1803-1896 逐项
        base.Visible = true;
        FClientRect = TDxRect.Bounds(0, 0, 0, 0);
        ImageIndex = new TDxImageIndex();
        ImageIndex.OnChange = _ => ImageIndexChange(this);
        BorderColor = new TDxBorderColor();
        DrawCaptionFont = new TDxFont();
    }

    // ---- 原文公有属性（DxControls.pas 477-573）----

    /// <summary>原文 TDxControl.Alignment。</summary>
    public TDxAlignment Alignment
    {
        get => _alignment;
        set => _alignment = value;
    }

    /// <summary>原文 TDxControl.Caption（setter 走 SetCaptionA → DoCaptionChange）。</summary>
    public string Caption
    {
        get => _caption;
        set
        {
            if (_caption != value)
            {
                _caption = value;
                DoCaptionChange();
            }
        }
    }

    /// <summary>原文 TDxControl.AutoSize（setter 走 ImageIndexChange + DoCaptionChange）。</summary>
    public bool AutoSize
    {
        get => _autoSize;
        set
        {
            if (_autoSize != value)
            {
                _autoSize = value;
                ImageIndexChange(this);
                DoCaptionChange();
            }
        }
    }

    /// <summary>原文 TDxControl.ClientRect（原点是父容器坐标；WinForms 的 Bounds 与之等价）。</summary>
    public TDxRect ClientRect
    {
        get => FClientRect;
        set
        {
            var newRect = value;
            DoResize(ref newRect);
            FClientRect = newRect;
            base.Bounds = new System.Drawing.Rectangle(newRect.Left, newRect.Top, newRect.Width, newRect.Height);
            Repaint();
        }
    }

    /// <summary>原文 TDxControl.VisibleRect（2219-2238）。</summary>
    public TDxRect VisibleRect
    {
        get
        {
            if (DxOwner == null)
                return ClientRect;

            var ownerSurface = DxOwner.VisibleRect;
            var mySurface = DxRectUtil.MoveRect(ClientRect, new TDxPoint(DxOwner.VirtualRect.Left, DxOwner.VirtualRect.Top));
            return DxRectUtil.ShortRect(mySurface, ownerSurface);
        }
    }

    /// <summary>原文 TDxControl.VirtualRect（2242-2251）。</summary>
    public TDxRect VirtualRect
    {
        get
        {
            if (DxOwner == null)
                return ClientRect;

            return DxRectUtil.MoveRect(ClientRect, new TDxPoint(DxOwner.VirtualRect.Left, DxOwner.VirtualRect.Top));
        }
    }

    /// <summary>原文 TDxControl.Left（index 0：平移，不改宽高）。</summary>
    public new int Left
    {
        get => FClientRect.Left;
        set => SetPosition(0, value);
    }

    /// <summary>原文 TDxControl.Top（index 1）。</summary>
    public new int Top
    {
        get => FClientRect.Top;
        set => SetPosition(1, value);
    }

    /// <summary>原文 TDxControl.Width（index 2：只改右边界）。</summary>
    public new int Width
    {
        get => FClientRect.Right - FClientRect.Left;
        set => SetPosition(2, value);
    }

    /// <summary>原文 TDxControl.Height（index 3：只改下边界）。</summary>
    public new int Height
    {
        get => FClientRect.Bottom - FClientRect.Top;
        set => SetPosition(3, value);
    }

    /// <summary>原文 TDxControl.Enabled（默认 True）。</summary>
    public new bool Enabled { get => _enabled; set => _enabled = value; }

    /// <summary>原文 TDxControl.Designing（原文构造 := True）。</summary>
    public bool Designing { get => _designing; set => _designing = value; }

    /// <summary>原文 TDxControl.Center（默认 False）。</summary>
    public bool Center { get => _center; set => _center = value; }

    /// <summary>原文 TDxControl.OwnerMove（默认 False）。</summary>
    public bool OwnerMove { get => _ownerMove; set => _ownerMove = value; }

    /// <summary>原文 TDxControl.EnableFocus（默认 False）。</summary>
    public bool EnableFocus { get => _enableFocus; set => _enableFocus = value; }

    /// <summary>原文 TDxControl.EnableMouse（默认 True）。</summary>
    public bool EnableMouse { get => _enableMouse; set => _enableMouse = value; }

    /// <summary>原文 TDxControl.CanMouse（只读，默认 True）。</summary>
    public bool CanMouse => _canMouse;

    /// <summary>原文 TDxControl.Transparent（默认 True）。</summary>
    public bool Transparent { get => _transparent; set => _transparent = value; }

    /// <summary>原文 TDxControl.BackgroundColor（默认 clWhite）。</summary>
    public int BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

    /// <summary>原文 TDxControl.DrawBorder（默认 False）。</summary>
    public bool DrawBorder { get => _drawBorder; set => _drawBorder = value; }

    /// <summary>原文 TDxControl.Floating（默认 False）。</summary>
    public bool Floating { get => _floating; set => _floating = value; }

    /// <summary>原文 TDxControl.MouseEvents（默认 [mbLeft,mbRight,mbMiddle]）。</summary>
    public TMouseEvents MouseEvents { get => _mouseEvents; set => _mouseEvents = value; }

    /// <summary>原文 TDxControl.Align（默认 alNone）。</summary>
    public TDxAlign Align { get => _align; set => _align = value; }

    /// <summary>原文 TDxControl.ReferenceX（默认 rxLeft）。</summary>
    public TReferenceX ReferenceX { get => _referenceX; set => _referenceX = value; }

    /// <summary>原文 TDxControl.AdjustYByHeight（默认 False）。</summary>
    public bool AdjustYByHeight { get => _adjustYByHeight; set => _adjustYByHeight = value; }

    /// <summary>原文 TDxControl.TopAlignment（默认 False）。</summary>
    public bool TopAlignment { get => _topAlignment; set => _topAlignment = value; }

    /// <summary>原文 TDxControl.TabOrder（默认 0）。</summary>
    public int TabOrder { get => _tabOrder; set => _tabOrder = value; }

    /// <summary>原文 TDxControl.ControlID（默认 0）。</summary>
    public int ControlID { get => _controlID; set => _controlID = value; }

    /// <summary>原文 TDxControl.ShowName（默认 ''）。</summary>
    public string ShowName { get => _showName; set => _showName = value; }

    /// <summary>原文 TDxControl.Hint（默认 ''）。</summary>
    public string HintText { get => _hint; set => _hint = value; }

    /// <summary>原文 TDxControl.MouseDowned（默认 False）。</summary>
    public bool MouseDowned { get => _mouseDowned; set { if (_mouseDowned != value) { _mouseDowned = value; Repaint(); } } }

    /// <summary>原文 TDxControl.MouseMoveed（默认 False）。</summary>
    public bool MouseMoveed { get => _mouseMoveed; set { if (_mouseMoveed != value) { _mouseMoveed = value; Repaint(); } } }

    /// <summary>原文 TDxImageButton.Checked（默认 False）。</summary>
    public bool Checked { get => _checked; set { if (_checked != value) _checked = value; } }

    /// <summary>原文 TDxControl.BlendMode（protected，默认 Blend_Default=0）。</summary>
    public int BlendMode { get => _blendMode; set => _blendMode = value; }

    /// <summary>原文 TDxControl.MouseDownBlendMode（默认 Blend_Default）。</summary>
    public int MouseDownBlendMode { get => _mouseDownBlendMode; set => _mouseDownBlendMode = value; }

    /// <summary>原文 TDxControl.MouseMoveBlendMode（默认 Blend_Default）。</summary>
    public int MouseMoveBlendMode { get => _mouseMoveBlendMode; set => _mouseMoveBlendMode = value; }

    /// <summary>原文 TDxControl.ImageIndex。</summary>
    public TDxImageIndex ImageIndex { get; set; }

    /// <summary>原文 TDxControl.BorderColor。</summary>
    public TDxBorderColor BorderColor { get; set; }

    /// <summary>原文 TDxControl.FDrawCaptionFont（DxControls.pas 1890）。</summary>
    public TDxFont DrawCaptionFont { get; set; }

    /// <summary>原文 TDxControl.OnPaint（TNotifyEvent）。</summary>
    public Action<TDxControl> OnPaint;

    /// <summary>原文 TDxControl.OnInRealArea（TOnInRealArea）。</summary>
    public Action<TDxControl, int, int, BoolRef> OnInRealArea;

    /// <summary>FAutoSizeSetFlag（HZQ 20230609）——原文置 $FF 后不再重复自动定尺。</summary>
    public int AutoSizeSetFlag { get => _autoSizeSetFlag; set => _autoSizeSetFlag = value; }

    /// <summary>原文属性 OnGetImage（转发到 FImageIndex.OnGetImage）。</summary>
    public void SetOnGetImage(Action<TDxImageIndex, TImageType> value)
    {
        ImageIndex.SetOnGetImage(value);
    }

    /// <summary>原文 TDxImageIndex.OnChange → TDxControl.ImageIndexChange。</summary>
    protected virtual void ImageIndexChange(TDxControl sender) { }

    /// <summary>原文 TDxControl.DoCaptionChange（虚方法，子类覆写）。</summary>
    protected virtual void DoCaptionChange() { }

    /// <summary>原文 TDxControl.DoResize（alNone 时不动，见 2718+）。</summary>
    protected virtual void DoResize(ref TDxRect newRect) { }

    /// <summary>原文 TDxControl.Repaint。</summary>
    public virtual void Repaint() { }

    // ---- 原文 2375-2411：GetPosition / SetPosition ----

    /// <summary>原文 TDxControl.GetPosition(Index)：0/1 → 0，2 → 宽，3 → 高。</summary>
    public int GetPosition(int index)
    {
        switch (index)
        {
            case 2: return FClientRect.Right - FClientRect.Left;
            case 3: return FClientRect.Bottom - FClientRect.Top;
            default: return 0;
        }
    }

    /// <summary>原文 TDxControl.SetPosition(Index, Value) 1:1。</summary>
    public void SetPosition(int index, int value)
    {
        var newRect = FClientRect;
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

        DoResize(ref newRect);
        FClientRect = newRect;
        base.Bounds = new System.Drawing.Rectangle(newRect.Left, newRect.Top, newRect.Width, newRect.Height);
        Repaint();
    }

    // ---- 原文 2123-2138：CheckAutoSize ----

    /// <summary>DxControls.pas 2123-2138 CheckAutoSize 1:1（原文 FAutoSizeSetFlag=0 才生效）。</summary>
    protected virtual void CheckAutoSizeBase()
    {
        if (AutoSize && _autoSizeSetFlag == 0)
        {
            if (ImageIndex.Up >= 0 && ImageIndex.Image != null)
            {
                var d = ImageIndex.Image.GetImage(ImageIndex.Up);
                if (d != null && d.Width * d.Height >= 4)
                {
                    FClientRect = TDxRect.Bounds(FClientRect.Left, FClientRect.Top, d.Width, d.Height);
                    base.Bounds = new System.Drawing.Rectangle(
                        FClientRect.Left, FClientRect.Top, FClientRect.Width, FClientRect.Height);
                    _autoSizeSetFlag = 0xFF;
                }
            }
        }
    }

    // ---- 原文 3199-3223：ReallyRect / CanDraw ----

    /// <summary>DxControls.pas 3199-3205 ReallyRect 1:1。</summary>
    public static TDxRect ReallyRect(TDxRect aVisibleRect, TDxRect aVirtualRect)
        => new(aVisibleRect.Left - aVirtualRect.Left,
               aVisibleRect.Top - aVirtualRect.Top,
               aVisibleRect.Left - aVirtualRect.Left + aVisibleRect.Width,
               aVisibleRect.Top - aVirtualRect.Top + aVisibleRect.Height);

    /// <summary>DxControls.pas 3208-3214 CanDraw(DestRect) 1:1。</summary>
    public bool CanDraw(TDxRect destRect)
    {
        var aVisibleRect = DxRectUtil.ShortRect(VisibleRect, destRect);
        return aVisibleRect.Right > aVisibleRect.Left && aVisibleRect.Bottom > aVisibleRect.Top;
    }

    /// <summary>DxControls.pas 3217-3223 CanDraw 1:1。</summary>
    public bool CanDraw()
    {
        var aVisibleRect = VisibleRect;
        return aVisibleRect.Right > aVisibleRect.Left && aVisibleRect.Bottom > aVisibleRect.Top;
    }

    // ---- 原文 3095-3145：ReallyPaintRect ----

    /// <summary>
    /// DxControls.pas 3095-3145 ReallyPaintRect 1:1（Texture 重载的 Texture = nil 分支）。
    /// nX/nY 为纹理内坐标；返回值为可绘制区域（virtual 坐标系）。无交集时返回 Rect(0,0,0,0)
    /// 且 nX=nY=-1（与原文一致）。
    /// </summary>
    public TDxRect ReallyPaintRect(TDxRect destRect, TDxRect srcRect, TDxRect vtRect, TDxRect vbRect,
        out int nX, out int nY)
    {
        nX = -1;
        nY = -1;
        var result = TDxRect.Rect(0, 0, 0, 0);

        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return result;

        int nLeft = vbRect.Left > destRect.Left ? vbRect.Left - destRect.Left : 0;
        nX = destRect.Left + nLeft;

        int nTop = vbRect.Top > destRect.Top ? vbRect.Top - destRect.Top : 0;
        nY = destRect.Top + nTop;

        if (destRect.Bottom - destRect.Top <= nTop || destRect.Right - destRect.Left <= nLeft) return result;

        int nWidth, nHeight;
        if (vbRect.Right < destRect.Right)
            nWidth = vbRect.Right - destRect.Left;
        else
            nWidth = destRect.Right - destRect.Left;

        if (destRect.Bottom > vbRect.Bottom)
            nHeight = vbRect.Bottom - destRect.Top;
        else
            nHeight = destRect.Bottom - destRect.Top;

        nWidth -= nLeft;
        nHeight -= nTop;

        if (nHeight <= 0 || nWidth <= 0) return result;

        // 原文：if Texture <> nil then SrcRect := ShortRect(SrcRect, Texture.ClientRect);
        // 本重载 Texture = nil（DrawCaption 调用处未传），故此行不执行。
        var paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(srcRect.Left + nLeft, srcRect.Top + nTop, nWidth, nHeight), srcRect);

        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return result;

        return paintRect;
    }

    // ---- 原文 3752-3850：DrawCaption ----

    /// <summary>
    /// DxControls.pas 3752-3775 DrawCaption(HGEFont, AFont, ACaption, ADestRect, X, Y)
    /// —— 原文走 FDrawCaptionFont 并取自身 VisibleRect/VirtualRect。
    /// </summary>
    public void DrawCaption(object hgeFont, TDxFont aFont, string aCaption, TDxRect aDestRect,
        int x = 0, int y = 0)
    {
        DrawCaptionFont.Color = aFont.Color;
        var aVisibleRect = VisibleRect;
        var aVirtualRect = VirtualRect;
        DrawCaption(hgeFont, DrawCaptionFont, aCaption, aDestRect, aVisibleRect, aVirtualRect, x, y);
    }

    /// <summary>
    /// DxControls.pas 3830-3850 DrawCaption(HGEFont, AFont, ACaption, ADestRect, AVisibleRect, AVirtualRect, X, Y)
    /// —— 特例：ACaption = '-' 时画一条 1 像素中横线（原文 3839-3844），不走字形。
    /// </summary>
    public void DrawCaption(object hgeFont, TDxFont aFont, string aCaption,
        TDxRect aDestRect, TDxRect aVisibleRect, TDxRect aVirtualRect, int x = 0, int y = 0)
    {
        if (string.IsNullOrEmpty(aCaption)) return;

        if (aCaption == "-")
        {
            var paintRect = aVirtualRect;
            paintRect.Top = aVirtualRect.Top + aVirtualRect.Height / 2;
            paintRect.Bottom = paintRect.Top + 1;
            Painter.FillRect(paintRect, aVirtualRect, aVisibleRect, aFont.Color);
        }
        else
        {
            var textImages = GetImageInfos(hgeFont, aCaption);
            DrawCaption(hgeFont, aFont, textImages, aDestRect, aVisibleRect, aVirtualRect, x, y);
        }
    }

    /// <summary>DxControls.pas 3823-3828 DrawCaption(..., ATextImages, ...) —— 取自身 Alignment。</summary>
    public void DrawCaption(object hgeFont, TDxFont aFont, List<TDxTextImageInfo> aTextImages,
        TDxRect aDestRect, TDxRect aVisibleRect, TDxRect aVirtualRect,
        int x = 0, int y = 0, int expandLineHeight = 0)
    {
        DrawCaption(hgeFont, aFont, Alignment, aTextImages, aDestRect, aVisibleRect, aVirtualRect,
            x, y, expandLineHeight);
    }

    /// <summary>
    /// DxControls.pas 3777-3821 DrawCaption（含 AAlignment）1:1。
    /// 关键保真点：
    ///   * DestRect 先作模板，PaintRect = Rect(0, 0, DestRect 宽, DestRect 高)；
    ///   * 三种对齐各自用 `(AVirtualRect 高 - DestRect 高) div 2` 求垂直居中（Delphi div 向零截断）；
    ///   * 最终绘制区 = ReallyPaintRect（nX/nY 为纹理内坐标）；
    ///   * AFont.Bold 时**先画 4 次偏移 1 像素的同色描边**（nX±1 / nY±1），再画正文（原文顺序不可换）。
    /// </summary>
    public void DrawCaption(object hgeFont, TDxFont aFont, TDxAlignment aAlignment,
        List<TDxTextImageInfo> aTextImages, TDxRect aDestRect, TDxRect aVisibleRect, TDxRect aVirtualRect,
        int x = 0, int y = 0, int expandLineHeight = 0)
    {
        if (aTextImages == null || aTextImages.Count == 0) return;

        var destRect = aDestRect;
        var paintRect = TDxRect.Rect(0, 0, destRect.Right - destRect.Left, destRect.Bottom - destRect.Top);

        int nLeft, nTop;
        switch (aAlignment)
        {
            case TDxAlignment.taLeftJustify:
                nLeft = x;
                nTop = (aVirtualRect.Height - destRect.Height) / 2 + y;
                destRect = DxRectUtil.MoveRect(destRect, new TDxPoint(nLeft, nTop));
                break;
            case TDxAlignment.taRightJustify:
                nLeft = aVirtualRect.Width - destRect.Width + x;
                nTop = (aVirtualRect.Height - destRect.Height) / 2 + y;
                destRect = DxRectUtil.MoveRect(destRect, new TDxPoint(nLeft, nTop));
                break;
            default: // taCenter
                nLeft = (aVirtualRect.Width - destRect.Width) / 2 + x;
                nTop = (aVirtualRect.Height - destRect.Height) / 2 + y;
                destRect = DxRectUtil.MoveRect(destRect, new TDxPoint(nLeft, nTop));
                break;
        }

        int nX, nY;
        paintRect = ReallyPaintRect(destRect, paintRect, aVirtualRect, aVisibleRect, out nX, out nY);

        if (paintRect.Right > paintRect.Left && paintRect.Bottom > paintRect.Top)
        {
            if (aFont.Bold)
            {
                Painter.TextRect(nX - 1, nY, paintRect, aTextImages, aFont.BColor, 2, 255, expandLineHeight);
                Painter.TextRect(nX + 1, nY, paintRect, aTextImages, aFont.BColor, 2, 255, expandLineHeight);
                Painter.TextRect(nX, nY - 1, paintRect, aTextImages, aFont.BColor, 2, 255, expandLineHeight);
                Painter.TextRect(nX, nY + 1, paintRect, aTextImages, aFont.BColor, 2, 255, expandLineHeight);
                Painter.TextRect(nX, nY, paintRect, aTextImages, aFont.Color, 2, 255, expandLineHeight);
            }
            else
            {
                Painter.TextRect(nX, nY, paintRect, aTextImages, aFont.Color, 2, 255, expandLineHeight);
            }
        }
    }

    /// <summary>原文 HGEFont.GetImageInfos 的接缝调用（无字体时返回空表）。</summary>
    public List<TDxTextImageInfo> GetImageInfos(object hgeFont, string text)
    {
        if (hgeFont == null) return new List<TDxTextImageInfo>();
        if (FontEnv.GetImageInfos != null)
            return FontEnv.GetImageInfos(hgeFont, text);
        return TDxFontEnv.BuildImageInfos(text);
    }

    /// <summary>原文 TextureFonts.FindFont(Name, Size, Style)。</summary>
    public object FindFont(TDxFont font)
    {
        if (font == null) return null;
        if (FontEnv.FindFont != null)
            return FontEnv.FindFont(font.Name, font.Size, font.Style);
        return null;
    }

    /// <summary>原文 HGEFont.TextHeight('0')。</summary>
    public int TextHeight(object hgeFont, string text)
    {
        if (hgeFont == null) return 0;
        if (FontEnv.TextHeight != null)
            return FontEnv.TextHeight(hgeFont, text);
        return TDxFontEnv.MeasureTextHeight(text);
    }

    /// <summary>DxControls.pas 3073-3078 DoPaint（CLIENTEXE=1 分支 = CheckAutoSize）。</summary>
    protected virtual void DoPaint()
    {
        CheckAutoSizeBase();
    }

    /// <summary>DxControls.pas 3956-3994 的 InRange 基类形态（含透明纹理逐像素判定接缝）。</summary>
    public virtual bool InRange(int x, int y)
    {
        if (DxRectUtil.PointInRect(new TDxPoint(x, y), VisibleRect))
        {
            var inRange = new BoolRef(true);
            var vRect = VirtualRect;
            DoOnInRealArea(x - vRect.Left, y - vRect.Top, inRange);
            return inRange.Value;
        }
        return false;
    }

    /// <summary>DxControls.pas 2765-2767 DoOnInRealArea（默认仅回调）。</summary>
    protected virtual void DoOnInRealArea(int x, int y, BoolRef isRealArea)
    {
        OnInRealArea?.Invoke(this, x, y, isRealArea);
    }

    // ---- 原文 3895-3908 的鼠标三段（子类覆写；基类只 Repaint）----

    public virtual void MouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        // 原文 TDxControl.MouseDown 在 PopupMenu <> nil 时先摆菜单位置；TDxPopupMenu 未移植 → 接缝。
        PopupMenuHook?.Invoke(this, x, y);
    }

    public virtual void MouseMove(TDxShiftState shift, int x, int y) { }

    public virtual void MouseUp(TDxMouseButton button, TDxShiftState shift, int x, int y) { }

    /// <summary>原文 TDxControl.CanMove（4055+）。</summary>
    protected virtual bool CanMove() => true;

    /// <summary>接缝：TDxPopupMenu 尚未移植，鼠标按下时的菜单弹出钩子。</summary>
    public Action<TDxControl, int, int> PopupMenuHook;

    /// <summary>DxControls.pas 3905-3908 DoMouseUp（基类仅 Repaint）。</summary>
    protected virtual void DoMouseUp() { }
}

/// <summary>Delphi `var X:Boolean` 出参的表达（InRange/OnInRealArea 需要可变布尔）。</summary>
public sealed class BoolRef
{
    public bool Value;
    public BoolRef(bool value) { Value = value; }
}
