namespace GXX.Client.GUI.DxComponent;

// ============================================================================================
// 【接缝】DxComponent 单元（Source/Client-HGE/DxComponent/DxComponents.pas）—— 本车道仅用到其中
// 的枚举/记录，逐字复刻；未列出的类型待 DxComponent 单元移植后补齐。
// 车道之间不许互相依赖，故此处只做最小定义，不移植 DxComponent 的其余实现。
//
// 颜色一律使用 Delphi TColor 语义的 32 位整数（0x00BBGGRR），与 DxComponents.pas 的 TColor
// 一致；托管显示层的 System.Drawing.Color 转换由 GUI/Mir 侧的 TColor 结构承担。
// ============================================================================================

/// <summary>DxComponents.pas:27 TClientVersion（176 185 英雄版 连击版 传奇续章 205）。</summary>
public enum TClientVersion
{
    cv176 = 0,
    cv185 = 1,
    cvHero = 2,
    cvSerial = 3,
    cvMirSequel = 4,
    // {cvMirs, cvMirReturn,}{cvMirReturn2}
    cvMirNewUI205 = 5,
}

/// <summary>DxComponents.pas:18 TSaveUIColor（Up/Hot/Down/Disabled 四态字色）。</summary>
public sealed class TSaveUIColor
{
    public int Up;
    public int Hot;
    public int Down;
    public int Disabled;
}

/// <summary>DxComponents.pas:39 TImageType（图库索引；本车道仅用 Prguse_wil/Prguse3_wil/UI3_wil）。</summary>
public enum TImageType
{
    Prguse_wil = 0,
    Prguse2_wil = 1,
    Prguse3_wil = 2,
    ChrSel_wil = 3,
    Prguse_16_wil = 4,
    Prguse2_16_wil = 5,
    Prguse3_16_wil = 6,
    ChrSel_16_wil = 7,
    UI_wil = 8,
    UI1_wil = 9,
    UI2_wil = 10,
    Prguse_wis = 11,
    Prguse2_wis = 12,
    Prguse3_wis = 13,
    NewopUI_Pak = 14,
    UI3_wil = 15,
    UIN_wil = 16,
    NSelect_wil = 17,
    UICommon_wil = 18,
    NewUI1_PAK = 19,
    NewUI2_PAK = 20,
    NewUI3_PAK = 21,
    NewUI4_PAK = 22,
    NewUI5_PAK = 23,
    Mobile_Pak = 24,
}

/// <summary>DxComponents.pas:54 TClickSound（csNone, csStone, csGlass, csNorm）。</summary>
public enum TClickSound
{
    csNone = 0,
    csStone = 1,
    csGlass = 2,
    csNorm = 3,
}

/// <summary>
/// Windows.pas TRect（本车道使用的全部字段：Left/Top/Right/Bottom + 只读 Width/Height）。
/// 不复用 System.Windows.Forms 的 Rectangle，保持 Delphi TRect 的可变值类型语义。
/// </summary>
public struct TRect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public TRect(int left, int top, int right, int bottom)
    {
        Left = left; Top = top; Right = right; Bottom = bottom;
    }

    public readonly int Width => Right - Left;
    public readonly int Height => Bottom - Top;
}

/// <summary>Delphi Windows.Bounds / InflateRect（DxControls.pas 与 MirNewUI205Dlg.pas 使用）。</summary>
public static class DxRect
{
    public static TRect Bounds(int x, int y, int width, int height) => new(x, y, x + width, y + height);

    /// <summary>Windows.InflateRect：原地按 dx/dy 缩放矩形。</summary>
    public static void InflateRect(ref TRect r, int dx, int dy)
    {
        r.Left -= dx; r.Right += dx;
        r.Top -= dy; r.Bottom += dy;
    }
}

/// <summary>
/// HGE 纹理句柄的托管接缝（HGE.pas TTexture）：本车道只读 Width/Height/ClientRect。
/// 【接缝：待 HGE 单元移植后接入真实纹理实现】
/// </summary>
public sealed class TTexture
{
    public int Width;
    public int Height;

    public TRect ClientRect => new(0, 0, Width, Height);
}

/// <summary>
/// GameImages.pas TGameImages 的托管接缝：本车道只读 Images[]（越界或未加载返回 null）。
/// 【接缝：待 GameImages 单元移植后接入真实 WIL 图库】
/// </summary>
public sealed class TGameImages
{
    private readonly TTexture[] _images;

    public TGameImages(int count) => _images = new TTexture[count];

    public TGameImages() => _images = System.Array.Empty<TTexture>();

    public TTexture this[int index]
    {
        get => index >= 0 && index < _images.Length ? _images[index] : null;
        set { if (index >= 0 && index < _images.Length) _images[index] = value; }
    }
}

/// <summary>DxComponents.pas:123 TGuiImageIndex（ImageType + Up/Hot/Down/Disabled 图号）。</summary>
public sealed class TGuiImageIndex
{
    public TImageType Image = TImageType.Prguse_wil;
    public int Up = -1;
    public int Hot = -1;
    public int Down = -1;
    public int Disabled = -1;

    /// <summary>Assign(Source)：整体拷贝图库与四态图号。</summary>
    public void Assign(TGuiImageIndex Source)
    {
        Image = Source.Image;
        Up = Source.Up;
        Hot = Source.Hot;
        Down = Source.Down;
        Disabled = Source.Disabled;
    }

    /// <summary>
    /// Assign(Source:TDxImageIndex)：SerialWindowsDlg.pas:94 g_MerchantImageIndex.Assign(DMerchantDlg.ImageIndex)
    /// 的等价拷贝（TDxControl.ImageIndex 与 TGuiImageIndex 同族记录）。
    /// </summary>
    public void Assign(TDxImageIndex Source)
    {
        Image = Source.ImageType;
        Up = Source.Up;
        Hot = Source.Hot;
        Down = Source.Down;
        Disabled = Source.Disabled;
    }
}
