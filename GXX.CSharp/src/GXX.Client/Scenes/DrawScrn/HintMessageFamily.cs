// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 的**提示消息族** 1:1 移植。
//
//   原文区间              类
//   99-113    THintMessage（基类）
//   115-134   THintImage
//   136-154   TWinHintImage
//   156-175   TLineBGHintImage
//   177-178   TFiexdHeightLine
//   180-206   THintPlayImage
//   208-214   THintPlayImageEx
//   216-244   THintItemProgress
//   246-270   THintText
//   272-277   THintFixedWidthText
//   279-291   TCountdownText
//   293-316   THintImageNumber
//   318-375   THintLines
//   735-737   procedure ProcessHintText（单元级自由过程）
//   748-2131  全部实现
//
// 可见性说明（偏离登记 D-P10-08）：原文 `ProcessHintText` 是**单元级过程**，直接写
// 各类的 `private`/`protected` 字段（Delphi 同单元可见）。托管侧无"同单元"概念，这些字段一律
// 落为 `public`；这是**可见性放宽**（不改变任何控制流或数值），已登记。
// ============================================================================================

using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Rtl;
using GXX.Core.Util;
using TRect = GXX.Client.GUI.DxComponent.TRect;

namespace GXX.Client.Scenes;

// ---------------------------------------------------------------------------------------------
// THintMessage（DrawScrn.pas:99-113 / 748-761）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:100-113 THintMessage（提示行元素基类；`virtual` 分派原样保留）。</summary>
public class THintMessage
{
    public int FWidth;
    public int FHeight;

    /// <summary>原文 `FOwner:THintLines`。</summary>
    public THintLines FOwner;

    /// <summary>DrawScrn.pas:106 constructor Create(AOwner:THintLines); virtual;</summary>
    public THintMessage(THintLines aOwner)
    {
        FWidth = 0;
        FHeight = 0;
        FOwner = aOwner;
    }

    /// <summary>DrawScrn.pas:107/755-757 procedure Initialize; virtual;（原文空体）。</summary>
    public virtual void Initialize()
    {
    }

    /// <summary>DrawScrn.pas:108/759-761 procedure Paint(OwnerRect, DestRect, HintWinRect:TRect); virtual;（原文空体）。</summary>
    public virtual void Paint(TRect ownerRect, TRect destRect, TRect hintWinRect)
    {
    }

    /// <summary>原文 `destructor Destroy`（THintMessage 未声明，托管侧无资源可释放）。</summary>
    public virtual void Free()
    {
    }

    /// <summary>DrawScrn.pas:110 property Owner:THintLines read FOwner。</summary>
    public THintLines Owner => FOwner;

    /// <summary>DrawScrn.pas:111 property Width:Integer read FWidth write FWidth。</summary>
    public int Width { get => FWidth; set => FWidth = value; }

    /// <summary>DrawScrn.pas:112 property Height:Integer read FHeight write FHeight。</summary>
    public int Height { get => FHeight; set => FHeight = value; }
}

// ---------------------------------------------------------------------------------------------
// THintImage（DrawScrn.pas:115-134 / 961-1035）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:115-134 THintImage。</summary>
public class THintImage : THintMessage
{
    public TGameImages FGameImages;
    public int FImageIndex;

    public int FOffsetX;
    public int FOffsetY;
    public int FShowBG;
    public int FFixedHeight;

    public bool FIsVerticalAlignTop;

    public THintImage(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:127/961-992 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        TTexture texture;
        if (FGameImages != null)
        {
            texture = FGameImages[FImageIndex];

            if (texture != null)
            {
                FWidth = texture.Width;
                if (FFixedHeight <= 0)
                    FHeight = texture.Height;
                else
                    FHeight = FFixedHeight;
            }

            if (FShowBG != 0)
            {
                if (FShowBG > 0)
                    texture = DrawScrnEnv.g_WNewopUIImages[250];
                else
                    texture = DrawScrnEnv.g_WNewopUIImages[251];

                if (texture != null)
                {
                    FWidth = DrawScrnRect.Max(FWidth, texture.Width);

                    if (FFixedHeight <= 0)
                        FHeight = DrawScrnRect.Max(FHeight, texture.Height);
                    else
                        FHeight = FFixedHeight;
                }
            }
        }
    }

    /// <summary>DrawScrn.pas:128/994-1035 procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TTexture texture;
        int x, y, h;
        if (FGameImages != null)
        {
            h = 0;
            if (FShowBG != 0)
            {
                if (FShowBG > 0)
                    texture = DrawScrnEnv.g_WNewopUIImages[250];
                else
                    texture = DrawScrnEnv.g_WNewopUIImages[251];

                if (texture != null)
                {
                    x = paintRect.Left + Owner.OffsetX + (FWidth - texture.Width) / 2 + FOffsetX;

                    h = texture.Height;

                    if (FIsVerticalAlignTop)
                        y = paintRect.Top + Owner.OffsetY + FOffsetY;
                    else
                        y = paintRect.Top + Owner.OffsetY + FOffsetY + (ownerRect.Bottom - ownerRect.Top - texture.Height) / 2;
                    DrawScrnEnv.GameCanvas.Draw(x, y, texture);
                }
            }

            texture = FGameImages[FImageIndex];
            if (texture != null)
            {
                x = paintRect.Left + Owner.OffsetX + (FWidth - texture.Width) / 2 + FOffsetX;

                if (FIsVerticalAlignTop)
                {
                    if (h == 0)
                        y = paintRect.Top + Owner.OffsetY + FOffsetY;
                    else
                        y = paintRect.Top + Owner.OffsetY + FOffsetY + (h - texture.Height) / 2;
                }
                else
                    y = paintRect.Top + Owner.OffsetY + FOffsetY + (ownerRect.Bottom - ownerRect.Top - texture.Height) / 2;
                DrawScrnEnv.GameCanvas.Draw(x, y, texture);
            }
        }
    }

    /// <summary>DrawScrn.pas:129 property GameImages。</summary>
    public TGameImages GameImages { get => FGameImages; set => FGameImages = value; }

    /// <summary>DrawScrn.pas:130 property ImageIndex。</summary>
    public int ImageIndex { get => FImageIndex; set => FImageIndex = value; }

    /// <summary>DrawScrn.pas:132 property OffsetX。</summary>
    public int OffsetX { get => FOffsetX; set => FOffsetX = value; }

    /// <summary>DrawScrn.pas:133 property OffsetY。</summary>
    public int OffsetY { get => FOffsetY; set => FOffsetY = value; }
}

// ---------------------------------------------------------------------------------------------
// TWinHintImage（DrawScrn.pas:136-154 / 1039-1072）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:136-154 TWinHintImage（位置随提示窗变化，不随行）。</summary>
public class TWinHintImage : THintMessage
{
    public TGameImages FGameImages;
    public int FImageIndex;

    public int FOffsetX;
    public int FOffsetY;

    /// <summary>原文 143 注释：不确定是否会影响到 API 接口，移动到 protected 消除警告。</summary>
    public int FShowBG;

    /// <summary>原文 145 `FFixedHeight:Integer;`（protected）。</summary>
    public int FFixedHeight;

    public TWinHintImage(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:147/1039-1057 procedure Initialize; override（原文把量宽体整段注释掉）。</summary>
    public override void Initialize()
    {
        FWidth = 0;
        FHeight = 0;
    }

    /// <summary>DrawScrn.pas:148/1059-1072 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TTexture texture;
        int x, y;
        if (FGameImages != null)
        {
            texture = FGameImages[FImageIndex];
            if (texture != null)
            {
                x = hintWinRect.Left + (hintWinRect.Right - hintWinRect.Left - texture.Width) + FOffsetX;
                y = hintWinRect.Top + FOffsetY;
                DrawScrnEnv.GameCanvas.Draw(x, y, texture);
            }
        }
    }

    /// <summary>DrawScrn.pas:149 property GameImages。</summary>
    public TGameImages GameImages { get => FGameImages; set => FGameImages = value; }

    /// <summary>DrawScrn.pas:150 property ImageIndex。</summary>
    public int ImageIndex { get => FImageIndex; set => FImageIndex = value; }

    /// <summary>DrawScrn.pas:152 property OffsetX。</summary>
    public int OffsetX { get => FOffsetX; set => FOffsetX = value; }

    /// <summary>DrawScrn.pas:153 property OffsetY。</summary>
    public int OffsetY { get => FOffsetY; set => FOffsetY = value; }
}

// ---------------------------------------------------------------------------------------------
// TLineBGHintImage（DrawScrn.pas:156-175 / 1076-1114）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:156-175 TLineBGHintImage（行背景图；FTextureWidth 供 THintLines.GetSize 取 MinWidth）。</summary>
public class TLineBGHintImage : THintMessage
{
    public int FTextureWidth;

    public TGameImages FGameImages;
    public int FImageIndex;

    public int FOffsetX;
    public int FOffsetY;

    /// <summary>原文 164「移动到 protected 消除警告」。</summary>
    public int FShowBG;

    /// <summary>原文 166 `FFixedHeight:Integer;`（protected）。</summary>
    public int FFixedHeight;

    public TLineBGHintImage(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:168/1076-1091 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        TTexture texture;
        FWidth = 0;
        FTextureWidth = 0;

        // 不要这个，不然装备名居中对齐不行 2019-10-21 17:59:44
        if (FGameImages != null)
        {
            texture = FGameImages[FImageIndex];
            if (texture != null)
            {
                FHeight = texture.Height;
                FTextureWidth = texture.Width;
            }
        }
    }

    /// <summary>DrawScrn.pas:169/1093-1114 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TTexture texture;
        int x, y;
        if (FGameImages != null)
        {
            texture = FGameImages[FImageIndex];
            if (texture != null)
            {
                x = hintWinRect.Left + DrawScrnEnv.HintWindowBorderWidth.Left;
                y = paintRect.Top + Owner.OffsetY + FOffsetY + (ownerRect.Bottom - ownerRect.Top - texture.Height) / 2;

                DrawScrnEnv.GameCanvas.Draw(x, y, texture);
            }
        }
    }

    /// <summary>DrawScrn.pas:170 property GameImages。</summary>
    public TGameImages GameImages { get => FGameImages; set => FGameImages = value; }

    /// <summary>DrawScrn.pas:171 property ImageIndex。</summary>
    public int ImageIndex { get => FImageIndex; set => FImageIndex = value; }

    /// <summary>DrawScrn.pas:173 property OffsetX。</summary>
    public int OffsetX { get => FOffsetX; set => FOffsetX = value; }

    /// <summary>DrawScrn.pas:174 property OffsetY。</summary>
    public int OffsetY { get => FOffsetY; set => FOffsetY = value; }
}

// ---------------------------------------------------------------------------------------------
// TFiexdHeightLine（DrawScrn.pas:177-178）
// ---------------------------------------------------------------------------------------------

/// <summary>
/// DrawScrn.pas:177-178 TFiexdHeightLine（`= class(THintMessage) end;`，原文**空类**，
/// 只用于 THintLines.AddFixedHeightLine 承载固定行高；托管侧同样不新增任何成员）。
/// </summary>
public class TFiexdHeightLine : THintMessage
{
    public TFiexdHeightLine(THintLines aOwner) : base(aOwner)
    {
    }
}

// ---------------------------------------------------------------------------------------------
// THintPlayImage（DrawScrn.pas:180-206 / 1118-1152）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:180-206 THintPlayImage（逐帧播放图）。</summary>
public class THintPlayImage : THintMessage
{
    public TGameImages FGameImages;
    public int FImageIndex;
    public int FPlayCount;
    public int FPlayTime;
    public uint FPlayTick;
    public int FPlayImageIndex;

    public int FOffsetX;
    public int FOffsetY;
    public bool FBlendDraw;

    public int FFixedWidth;
    public int FFixedHeight;

    public THintPlayImage(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:196/1118-1122 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        FWidth = FFixedWidth;
        FHeight = FFixedHeight;
    }

    /// <summary>DrawScrn.pas:197/1124-1152 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TTexture texture;
        int x, y;
        uint curTick;
        if (FGameImages != null)
        {
            texture = DrawScrnEnv.GetCachedImage(FGameImages, FPlayImageIndex, out int ptX, out int ptY);
            if (texture != null)
            {
                // 修正物品和对应的特效对不上 chongchong 2015-04-11
                x = paintRect.Left + Owner.OffsetX + FOffsetX + ptX;
                y = paintRect.Top + Owner.OffsetY + FOffsetY + ptY;

                if (FBlendDraw)
                    DrawScrnEnv.GameCanvas.DrawBlend(x, y, texture);
                else
                    DrawScrnEnv.GameCanvas.Draw(x, y, texture);
            }

            curTick = DrawScrnEnv.MyGetTickCount;
            if (curTick - FPlayTick > (uint)FPlayTime)
            {
                FPlayTick = curTick;
                FPlayImageIndex++;
                if (FPlayImageIndex - FImageIndex + 1 > FPlayCount)
                    FPlayImageIndex = FImageIndex;
            }
        }
    }

    /// <summary>DrawScrn.pas:198 property GameImages。</summary>
    public TGameImages GameImages { get => FGameImages; set => FGameImages = value; }

    /// <summary>DrawScrn.pas:199 property ImageIndex。</summary>
    public int ImageIndex { get => FImageIndex; set => FImageIndex = value; }

    /// <summary>DrawScrn.pas:201 property OffsetX。</summary>
    public int OffsetX { get => FOffsetX; set => FOffsetX = value; }

    /// <summary>DrawScrn.pas:202 property OffsetY。</summary>
    public int OffsetY { get => FOffsetY; set => FOffsetY = value; }

    /// <summary>DrawScrn.pas:203 property PlayCount。</summary>
    public int PlayCount { get => FPlayCount; set => FPlayCount = value; }

    /// <summary>DrawScrn.pas:204 property PlayTime。</summary>
    public int PlayTime { get => FPlayTime; set => FPlayTime = value; }

    /// <summary>DrawScrn.pas:205 property BlendDraw。</summary>
    public bool BlendDraw { get => FBlendDraw; set => FBlendDraw = value; }
}

// ---------------------------------------------------------------------------------------------
// THintPlayImageEx（DrawScrn.pas:208-214 / 1158-1198）
// ---------------------------------------------------------------------------------------------

/// <summary>
/// DrawScrn.pas:208-214 THintPlayImageEx（NewopUI 播放图，按 `Images[]` 而非 `GetCachedImage`，
/// 且 **不** 播放 `GetCachedImage` 的原点偏移 —— 与基类 THintPlayImage 的差异必须保留）。
/// </summary>
public class THintPlayImageEx : THintPlayImage
{
    public int FIncSpacing;

    public THintPlayImageEx(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:212/1158-1169 procedure Initialize; override（比基类多算 FIncSpacing）。</summary>
    public override void Initialize()
    {
        TTexture texture;
        if (FGameImages != null)
        {
            texture = FGameImages[FImageIndex];
            if (texture != null)
            {
                FWidth = texture.Width + FIncSpacing;
                FHeight = texture.Height;
            }
        }
    }

    /// <summary>DrawScrn.pas:213/1171-1198 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TTexture texture;
        int x, y;
        uint curTick;
        if (FGameImages != null)
        {
            texture = FGameImages[FPlayImageIndex];
            if (texture != null)
            {
                // 修正物品和对应的特效对不上 chongchong 2015-04-11
                x = paintRect.Left + Owner.OffsetX + FOffsetX;
                y = paintRect.Top + Owner.OffsetY + FOffsetY;

                if (FBlendDraw)
                    DrawScrnEnv.GameCanvas.DrawBlend(x, y, texture);
                else
                    DrawScrnEnv.GameCanvas.Draw(x, y, texture);
            }

            curTick = DrawScrnEnv.MyGetTickCount;
            if (curTick - FPlayTick > (uint)FPlayTime)
            {
                FPlayTick = curTick;
                FPlayImageIndex++;
                if (FPlayImageIndex - FImageIndex + 1 > FPlayCount)
                    FPlayImageIndex = FImageIndex;
            }
        }
    }
}

// ---------------------------------------------------------------------------------------------
// THintItemProgress（DrawScrn.pas:216-244 / 1204-1316）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:216-244 THintItemProgress（进度条提示元素）。</summary>
public class THintItemProgress : THintMessage
{
    public TColor FTextColor;
    public string FText = "";
    public TGameImages FGameImages;
    public int FProgressIndex;
    public int FPlayCount;

    public int FMaxValue;
    public int FCurValue;

    public uint FPlayTick;
    public int FPlayImageIndex;

    public int FOffsetX;
    public int FOffsetY;

    public THintItemProgress(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:233/1204-1223 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        TTexture texture;
        if (FGameImages != null)
        {
            if (FProgressIndex == 1)
                texture = FGameImages[640];
            else
                texture = FGameImages[620];

            if (texture != null)
            {
                FWidth = texture.Width + 2;
                FHeight = texture.Height;
            }
        }
        else
        {
            FWidth = 0;
            FHeight = 0;
        }
    }

    /// <summary>DrawScrn.pas:234/1225-1316 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TTexture textureBg, texture;
        int x, y, textW, textH;
        uint curTick;
        TRect r;
        if (FGameImages != null)
        {
            if (FProgressIndex == 1)
                textureBg = FGameImages[640];
            else
                textureBg = FGameImages[620];

            if ((textureBg != null) && (FPlayCount > 0))
            {
                // 修正物品和对应的特效对不上 chongchong 2015-04-11
                x = paintRect.Left + Owner.OffsetX + FOffsetX;
                y = paintRect.Top + Owner.OffsetY + FOffsetY;

                DrawScrnEnv.GameCanvas.Draw(x, y, textureBg);
            }

            if (FMaxValue == 0) return;

            if ((FCurValue > 0) && (FPlayCount > 0))
            {
                if (FPlayCount >= 10)
                {
                    curTick = DrawScrnEnv.MyGetTickCount;
                    if (curTick - FPlayTick > 500)
                    {
                        FPlayTick = curTick;
                        FPlayImageIndex++;
                        if (FPlayImageIndex > FPlayCount - 10)
                            FPlayImageIndex = 0;
                    }

                    if (FProgressIndex == 1)
                        texture = FGameImages[650 + FPlayImageIndex];
                    else
                        texture = FGameImages[630 + FPlayImageIndex];

                    if (texture != null)
                    {
                        // 修正物品和对应的特效对不上 chongchong 2015-04-11
                        if (textureBg != null)
                        {
                            x = paintRect.Left + Owner.OffsetX + FOffsetX + (textureBg.Width - texture.Width) / 2;
                            y = paintRect.Top + Owner.OffsetY + FOffsetY + (textureBg.Height - texture.Height) / 2;
                        }
                        else
                        {
                            x = paintRect.Left + Owner.OffsetX + FOffsetX;
                            y = paintRect.Top + Owner.OffsetY + FOffsetY;
                        }

                        r = DrawScrnRect.Rect(0, 0, texture.Width, texture.Height);
                        r.Right = r.Left + (int)Math.Round((double)(r.Right - r.Left) / FMaxValue * FCurValue);

                        DrawScrnEnv.GameCanvas.Draw(x, y, r, texture);
                    }
                }
                else
                {
                    if (FProgressIndex == 1)
                        texture = FGameImages[640 + FPlayCount];
                    else
                        texture = FGameImages[620 + FPlayCount];

                    if (texture != null)
                    {
                        // 修正物品和对应的特效对不上 chongchong 2015-04-11
                        if (textureBg != null)
                        {
                            x = paintRect.Left + Owner.OffsetX + FOffsetX + (textureBg.Width - texture.Width) / 2;
                            y = paintRect.Top + Owner.OffsetY + FOffsetY + (textureBg.Height - texture.Height) / 2;
                        }
                        else
                        {
                            x = paintRect.Left + Owner.OffsetX + FOffsetX;
                            y = paintRect.Top + Owner.OffsetY + FOffsetY;
                        }

                        r = DrawScrnRect.Rect(0, 0, texture.Width, texture.Height);
                        r.Right = r.Left + (int)Math.Round((double)(r.Right - r.Left) / FMaxValue * FCurValue);

                        DrawScrnEnv.GameCanvas.Draw(x, y, r, texture);
                    }
                }
            }

            if ((textureBg != null) && (FText.Length > 0))
            {
                textW = DrawScrnEnv.TextWidth(DrawScrnEnv.CurrentFont, FText);
                textH = DrawScrnEnv.g_CurrentFontHeight;

                // 修正物品和对应的特效对不上 chongchong 2015-04-11
                x = paintRect.Left + Owner.OffsetX + FOffsetX + (textureBg.Width - textW) / 2;
                y = paintRect.Top + Owner.OffsetY + FOffsetY + (textureBg.Height - textH) / 2;

                DrawScrnEnv.BoldTextOut(x, y, FText, FTextColor);
            }
        }
    }

    /// <summary>DrawScrn.pas:235 property GameImages。</summary>
    public TGameImages GameImages { get => FGameImages; set => FGameImages = value; }

    /// <summary>DrawScrn.pas:237 property TextColor。</summary>
    public TColor TextColor { get => FTextColor; set => FTextColor = value; }

    /// <summary>DrawScrn.pas:238 property OffsetX。</summary>
    public int OffsetX { get => FOffsetX; set => FOffsetX = value; }

    /// <summary>DrawScrn.pas:239 property OffsetY。</summary>
    public int OffsetY { get => FOffsetY; set => FOffsetY = value; }

    /// <summary>DrawScrn.pas:240 property PlayCount。</summary>
    public int PlayCount { get => FPlayCount; set => FPlayCount = value; }

    /// <summary>DrawScrn.pas:241 property ProgressIndex。</summary>
    public int ProgressIndex { get => FProgressIndex; set => FProgressIndex = value; }

    /// <summary>DrawScrn.pas:242 property MaxValue。</summary>
    public int MaxValue { get => FMaxValue; set => FMaxValue = value; }

    /// <summary>DrawScrn.pas:243 property CurValue。</summary>
    public int CurValue { get => FCurValue; set => FCurValue = value; }
}

// ---------------------------------------------------------------------------------------------
// THintText（DrawScrn.pas:246-270 / 764-870）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:246-270 THintText（提示文本行）。</summary>
public class THintText : THintMessage
{
    public string FCaption = "";
    public TColor FColor;
    public int FSize;
    public TFontStyles FStyle;
    public bool FIsStroke;
    public string FFontName = "";

    public THintText(THintLines aOwner) : base(aOwner)
    {
        FCaption = "";
        FColor = TColor.clWhite;
        FSize = 0;
        FStyle = TFontStyles.fsNone;
        FIsStroke = false;
        FFontName = "";
    }

    /// <summary>
    /// DrawScrn.pas:255/775-781 protected procedure SetCaption(Value:string)。
    /// <para>★ 原文缺陷（照抄 + 差异断言）：`property Caption` 的写访问器是**字段本身**
    /// （265 行 `write FCaption`），`SetCaption` 全文无调用点 —— 即**死代码**。此处保留同形。</para>
    /// </summary>
    protected void SetCaption(string value)
    {
        if (FCaption != value)
        {
            FCaption = value;
        }
        // Initialize;
    }

    /// <summary>DrawScrn.pas:256/783-789 procedure SetSize(Value:Integer)（改后调 Initialize）。</summary>
    public void SetSize(int value)
    {
        if (FSize != value)
        {
            FSize = value;
        }
        Initialize();
    }

    /// <summary>DrawScrn.pas:257/791-797 procedure SetStyle(Value:TFontStyles)。</summary>
    public void SetStyle(TFontStyles value)
    {
        if (FStyle != value)
        {
            FStyle = value;
        }
        // Initialize;
    }

    /// <summary>DrawScrn.pas:258/799-805 procedure SetIsStroke(Value:Boolean)。</summary>
    public void SetIsStroke(bool value)
    {
        if (FIsStroke != value)
        {
            FIsStroke = value;
        }
        // Initialize;
    }

    /// <summary>DrawScrn.pas:262/807-829 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        THGEFont hgeFont = null;

        if (FFontName != "")
            hgeFont = DrawScrnEnv.FindFont(FFontName, FSize, FStyle);

        if (hgeFont == null)
            hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, FSize, FStyle);

        if (hgeFont != null)
        {
            if (FIsStroke)
            {
                FHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp") + 2;
                FWidth = DrawScrnEnv.TextWidth(hgeFont, FCaption); //  + 2 去掉这个，不然 {攻击|247}: 2-3 这样的对不齐
            }
            else
            {
                FHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp");
                FWidth = DrawScrnEnv.TextWidth(hgeFont, FCaption);
            }
        }
    }

    /// <summary>DrawScrn.pas:263/831-870 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        TRect aRect;
        THGEFont hgeFont;
        TTexture d;
        if (FCaption != "")
        {
            if (FCaption == "-")
            {
                aRect = DrawScrnRect.Bounds(ownerRect.Left + 4 + 2,
                    ownerRect.Top + Owner.OffsetY + (ownerRect.Bottom - ownerRect.Top - 2) / 2,
                    ownerRect.Right - ownerRect.Left - 8, 2);

                // 修改悬浮框中的分界线路径放到NewopUI.pak的00046中
                d = DrawScrnEnv.g_WNewopUIImages[46];
                if (d == null)
                    DrawScrnEnv.GameCanvas.FillRect(aRect, unchecked((int)0xFF8C715A), unchecked((int)0xFF8C715A), DrawScrnEnv.clBlack1, DrawScrnEnv.clBlack1);
                else
                {
                    if (d.Width >= aRect.Right - aRect.Left)
                        DrawScrnEnv.GameCanvas.Draw(aRect.Left, aRect.Top, DrawScrnRect.Rect(0, 0, aRect.Right - aRect.Left, d.Height), d);

                    // 当分隔条长度不够时，拉升绘制分隔条 chongchong 2015-11-11
                    else
                        DrawScrnEnv.GameCanvas.StretchDraw(DrawScrnRect.Rect(aRect.Left, aRect.Top, aRect.Right, aRect.Top + d.Height), d);
                }
            }
            else
            {
                hgeFont = null;

                if (FFontName != "")
                    hgeFont = DrawScrnEnv.FindFont(FFontName, FSize, FStyle);

                if (hgeFont == null)
                    hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, FSize, FStyle);

                if (hgeFont != null)
                {
                    if (FIsStroke)
                        DrawScrnEnv.BoldTextOut(hgeFont,
                            paintRect.Left + Owner.OffsetX,
                            paintRect.Top + Owner.OffsetY + (paintRect.Bottom - paintRect.Top - DrawScrnEnv.TextHeight(hgeFont, FCaption)) / 2,
                            FCaption, FColor, TColor.clBlack);
                    else
                        hgeFont.TextOut(paintRect.Left + Owner.OffsetX,
                            paintRect.Top + Owner.OffsetY + (paintRect.Bottom - paintRect.Top - DrawScrnEnv.TextHeight(hgeFont, FCaption)) / 2,
                            FCaption, FColor);
                }
            }
        }
    }

    /// <summary>DrawScrn.pas:265 property Caption。</summary>
    public string Caption { get => FCaption; set => FCaption = value; }

    /// <summary>DrawScrn.pas:266 property Color。</summary>
    public TColor Color { get => FColor; set => FColor = value; }

    /// <summary>DrawScrn.pas:267 property Size:Integer read FSize write SetSize。</summary>
    public int Size { get => FSize; set => SetSize(value); }

    /// <summary>DrawScrn.pas:268 property Style:TFontStyles read FStyle write SetStyle。</summary>
    public TFontStyles Style { get => FStyle; set => SetStyle(value); }

    /// <summary>DrawScrn.pas:269 property IsStroke:Boolean read FIsStroke write SetIsStroke。</summary>
    public bool IsStroke { get => FIsStroke; set => SetIsStroke(value); }
}

// ---------------------------------------------------------------------------------------------
// THintFixedWidthText（DrawScrn.pas:272-277 / 874-879）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:272-277 THintFixedWidthText（`&lt;TextW:N:X:Y&gt;` 定宽文本）。</summary>
public class THintFixedWidthText : THintText
{
    public int FMinWidth;

    public THintFixedWidthText(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:276/874-879 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        base.Initialize();
        if (FMinWidth >= FWidth)
            FWidth = FMinWidth;
    }
}

// ---------------------------------------------------------------------------------------------
// TCountdownText（DrawScrn.pas:279-291 / 883-956）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:279-291 TCountdownText（倒计时文本）。</summary>
public class TCountdownText : THintMessage
{
    public int FCountdownValue;
    public uint FStartTick;
    public TColor FTextColor;
    public int FOffsetX;
    public int FOffsetY;

    public TCountdownText(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:287/883-925 function GetShowText:string。</summary>
    public string GetShowText()
    {
        int nTemp, nDay, nHour, sMin, sSec;
        if (FCountdownValue <= 0)
            return "0秒";

        nTemp = FCountdownValue;
        nDay = 0;
        nHour = 0;
        sMin = 0;

        if (nTemp >= 86400) // 60 * 60 * 24
        {
            nDay = FCountdownValue / 86400;
            nTemp = FCountdownValue % 86400;
        }

        if (nTemp >= 3600)
        {
            nHour = nTemp / 3600;
            nTemp = nTemp % 3600;
        }

        if (nTemp >= 60)
        {
            sMin = nTemp / 60;
            nTemp = nTemp % 60;
        }

        sSec = nTemp;

        string result = "";
        if (nDay > 0)
        {
            result = string.Format("{0}天{1}时{2}分{3}秒", nDay, nHour, sMin, sSec);
        }
        else if (nHour > 0)
        {
            result = string.Format("{0}时{1}分{2}秒", nHour, sMin, sSec);
        }
        else if (sMin > 0)
        {
            result = string.Format("{0}分{1}秒", sMin, sSec);
        }
        else
            result = string.Format("{0}秒", sSec);

        return result;
    }

    /// <summary>DrawScrn.pas:289/927-936 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        THGEFont hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, 9, TFontStyles.fsNone);
        if (hgeFont != null)
        {
            FHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp") + 2;
            FWidth = DrawScrnEnv.TextWidth(hgeFont, GetShowText() + "      ") + 2;
        }
    }

    /// <summary>DrawScrn.pas:290/938-956 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        THGEFont hgeFont;
        string s;
        if (FCountdownValue > 0)
        {
            if (DrawScrnEnv.MyGetTickCount - FStartTick >= 1000)
            {
                FStartTick = DrawScrnEnv.MyGetTickCount;
                FCountdownValue--;
            }
        }

        hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, 9, TFontStyles.fsNone);

        if (hgeFont != null)
        {
            s = GetShowText();
            DrawScrnEnv.BoldTextOut(hgeFont,
                paintRect.Left + Owner.OffsetX + FOffsetX,
                paintRect.Top + Owner.OffsetY + (paintRect.Bottom - paintRect.Top - DrawScrnEnv.TextHeight(hgeFont, s)) / 2 + FOffsetY,
                s, FTextColor, TColor.clBlack);
        }
    }
}

// ---------------------------------------------------------------------------------------------
// THintImageNumber（DrawScrn.pas:293-316 / 1320-1357）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:293-316 THintImageNumber（位图数字）。</summary>
public class THintImageNumber : THintMessage
{
    public TGameImages FGameImages;
    public int FNumberIndex;
    public int FNumberSpace;
    public string FNumberValue = "";

    public int FOffsetX;
    public int FOffsetY;

    public THintImageNumber(THintLines aOwner) : base(aOwner)
    {
    }

    /// <summary>DrawScrn.pas:305/1320-1337 procedure Initialize; override。</summary>
    public override void Initialize()
    {
        TTexture texture;
        int ii, nIndex;
        FWidth = 0;
        FHeight = 0;
        if ((FGameImages != null) && (FNumberIndex >= 0) && (FNumberIndex <= 9) && (FNumberValue.Length > 0))
        {
            for (ii = 1; ii <= FNumberValue.Length; ii++)
            {
                nIndex = 1230 + (FNumberIndex * 10) + DelphiRTL.StrToInt(FNumberValue[ii - 1].ToString());
                texture = FGameImages[nIndex];
                if (texture != null)
                {
                    if (FHeight < texture.Height) FHeight = texture.Height;
                    FWidth = FWidth + texture.Width + FNumberSpace;
                }
            }
        }
    }

    /// <summary>DrawScrn.pas:306/1339-1357 procedure Paint; override。</summary>
    public override void Paint(TRect ownerRect, TRect paintRect, TRect hintWinRect)
    {
        int px, py, index, i;
        TTexture texture;
        px = paintRect.Left + Owner.OffsetX + FOffsetX;
        py = paintRect.Top + Owner.OffsetY + FOffsetY;

        if ((FGameImages != null) && (FNumberValue.Length > 0))
        {
            for (i = 1; i <= FNumberValue.Length; i++)
            {
                index = 1230 + (FNumberIndex * 10) + DelphiRTL.StrToInt(FNumberValue[i - 1].ToString());
                texture = FGameImages[index];
                if (texture != null)
                {
                    // ★ 原文 1352 用的是**属性** `Height`（= FHeight，不是 texture.Height）
                    DrawScrnEnv.GameCanvas.Draw(px, py + (Height - texture.Height) / 2, texture);
                    px = px + texture.Width + FNumberSpace;
                }
            }
        }
    }
}

// ---------------------------------------------------------------------------------------------
// THintLines（DrawScrn.pas:318-375 / 1361-2160）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:318-375 THintLines（提示文本行容器）。</summary>
public class THintLines
{
    public List<object> FList;
    public int FOffsetX, FOffsetY;
    public int FWidth;
    public int FHeight;
    public int FMinWidth; // 扩展最底宽度(用于背景图) 2019-12-16 09:35:50
    public int FItemHeight;
    public TAlignment FAlignment;

    /// <summary>
    /// ask 模式的图标加文字不好处理，只能这样先处理着，如果还有这样的，就用 Group: Byte 来整。
    /// 这个主要目地是为了处理对齐，单个偏移不行，会叠加，只有在对齐时整体偏移才可以。
    /// </summary>
    public bool FIsItemIconText;

    /// <summary>DrawScrn.pas:337/1361-1372 constructor Create。</summary>
    public THintLines()
    {
        FWidth = 0;
        FHeight = 0;
        FItemHeight = 0;
        FOffsetX = DrawScrnEnv.HintWindowBorderWidth.Left;
        FOffsetY = DrawScrnEnv.HintWindowBorderWidth.Top;
        FAlignment = TAlignment.taLeftJustify;
        FIsItemIconText = false;
        FList = new List<object>();
    }

    /// <summary>DrawScrn.pas:338/1374-1383 destructor Destroy; override。</summary>
    public void Free()
    {
        for (int i = 0; i < FList.Count; i++)
        {
            ((THintMessage)FList[i]).Free();
        }
    }

    /// <summary>DrawScrn.pas:374/1385-1418 procedure GetSize。</summary>
    public void GetSize()
    {
        THintMessage hintMessage;
        int maxWidth;
        FWidth = 0;
        FHeight = 0;
        maxWidth = 0;

        for (int i = 0; i < FList.Count; i++)
        {
            hintMessage = (THintMessage)FList[i];
            hintMessage.Initialize();
            FHeight = DrawScrnRect.Max(FHeight, hintMessage.Height);
            FWidth = FWidth + hintMessage.Width;

            if (hintMessage is TLineBGHintImage)
            {
                FMinWidth = ((TLineBGHintImage)hintMessage).FTextureWidth;
            }
        }

        // ★ 原文缺陷（照抄 + 差异断言锁死 D-P10-D01）：`maxWidth` 全文只被初始化为 0，
        //   故 `FWidth < MaxWidth` 恒为 False ⇒ 该 if 是**死分支**。此处保留同形。
        if (FWidth < maxWidth)
        {
            FWidth = maxWidth;
        }

        if (DrawScrnEnv.CurrentFont != null)
        {
            if (FWidth <= 0)
                FWidth = DrawScrnEnv.TextWidth(DrawScrnEnv.CurrentFont, "0");

            if (FHeight <= 0)
                FHeight = DrawScrnEnv.g_CurrentFontHeight;
        }
        FItemHeight = FHeight + 2;
    }

    /// <summary>DrawScrn.pas:333/1420-1433 function Get(Index:Integer):string。</summary>
    public string Get(int index)
    {
        THintMessage hintMessage;
        if ((index >= 0) && (index < FList.Count))
        {
            hintMessage = (THintMessage)FList[index];
            if (hintMessage is THintText)
                return ((THintText)hintMessage).FCaption;
            return "";
        }
        return "";
    }

    /// <summary>DrawScrn.pas:334/1435-1438 function GetCount:Integer。</summary>
    public int GetCount() => FList.Count;

    /// <summary>DrawScrn.pas:332/1440-1449 procedure Put(Index:Integer; const S:string)。</summary>
    public void Put(int index, string s)
    {
        THintMessage hintMessage;
        if ((index >= 0) && (index < FList.Count))
        {
            hintMessage = (THintMessage)FList[index];
            if (hintMessage is THintText)
                ((THintText)hintMessage).FCaption = s;
        }
    }

    /// <summary>DrawScrn.pas:341/2015-2033 procedure Add(S, Color, FontSize, FontStyles, IsStroke, FontName)。</summary>
    public void Add(string s, TColor color, int fontSize = 9, TFontStyles fontStyles = TFontStyles.fsNone,
        bool isStroke = false, string fontName = "")
    {
        var list = new List<object>();
        try
        {
            DrawScrnHintTextParser.ProcessHintText(s, this, list, color, fontSize, fontStyles, isStroke, fontName);

            for (int i = 0; i < list.Count; i++)
            {
                FList.Add(list[i]);
            }
        }
        finally
        {
            // List.Free（托管侧无资源）
        }

        GetSize();
    }

    /// <summary>DrawScrn.pas:357/2035-2043 procedure AddFixedHeightLine(Height:Integer)。</summary>
    public void AddFixedHeightLine(int height)
    {
        var line = new TFiexdHeightLine(this);
        line.FHeight = height;
        FList.Add(line);
        GetSize();
    }

    /// <summary>DrawScrn.pas:349/2045-2063 procedure Insert(Index, S, Color, FontSize, FontStyles, IsStroke, FontName)。</summary>
    public void Insert(int index, string s, TColor color, int fontSize = 9, TFontStyles fontStyles = TFontStyles.fsNone,
        bool isStroke = false, string fontName = "")
    {
        var list = new List<object>();
        try
        {
            DrawScrnHintTextParser.ProcessHintText(s, this, list, color, fontSize, fontStyles, isStroke, fontName);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                FList.Insert(index, list[i]);
            }
        }
        finally
        {
            // List.Free
        }

        GetSize();
    }

    /// <summary>DrawScrn.pas:339/2065-2073 procedure Clear。</summary>
    public void Clear()
    {
        for (int i = 0; i < FList.Count; i++)
        {
            ((THintMessage)FList[i]).Free();
        }
        FList.Clear();
    }

    /// <summary>DrawScrn.pas:340/2075-2081 procedure Delete(Index:Integer)。</summary>
    public void Delete(int index)
    {
        if ((index >= 0) && (index < FList.Count))
        {
            ((THintMessage)FList[index]).Free();
            FList.RemoveAt(index); // 原文 Classes.TList.Delete(Index) 1:1
        }
    }

    /// <summary>DrawScrn.pas:335/2083-2089 function GetObject(Index:Integer):THintMessage。</summary>
    public THintMessage GetObject(int index)
    {
        if ((index >= 0) && (index < FList.Count))
            return (THintMessage)FList[index];
        return null;
    }

    /// <summary>DrawScrn.pas:359/2091-2114 procedure Paint(OwnerRect, PaintRect:TRect)。</summary>
    public void Paint(TRect ownerRect, TRect paintRect)
    {
        int nX;
        THintMessage hintMessage;
        nX = paintRect.Left;
        // 将带播放的特效放在最前面放 chongchong 2015-01-22
        for (int i = 0; i < FList.Count; i++)
        {
            hintMessage = (THintMessage)FList[i];
            if ((hintMessage is THintPlayImage) || (hintMessage is THintPlayImageEx) || (hintMessage is THintImage))
            {
                hintMessage.Paint(paintRect, DrawScrnRect.Bounds(nX, paintRect.Top, hintMessage.Width, FItemHeight), ownerRect);
            }
            nX += hintMessage.Width;
        }

        nX = paintRect.Left;
        for (int i = 0; i < FList.Count; i++)
        {
            hintMessage = (THintMessage)FList[i];
            if (!((hintMessage is THintPlayImage) || (hintMessage is THintPlayImageEx) || (hintMessage is THintImage)))
            {
                hintMessage.Paint(paintRect, DrawScrnRect.Bounds(nX, paintRect.Top, hintMessage.Width, FItemHeight), ownerRect);
            }
            nX += hintMessage.Width;
        }
    }

    /// <summary>DrawScrn.pas:360/2116-2143 procedure PaintWithoutWinHintImage(OwnerRect, PaintRect; var HaveWinHintImage:Boolean)。</summary>
    public void PaintWithoutWinHintImage(TRect ownerRect, TRect paintRect, ref bool haveWinHintImage)
    {
        int nX;
        THintMessage hintMessage;
        nX = paintRect.Left;
        // 将带播放的特效放在最前面放 chongchong 2015-01-22
        for (int i = 0; i < FList.Count; i++)
        {
            hintMessage = (THintMessage)FList[i];

            if (hintMessage is TWinHintImage)
            {
                haveWinHintImage = true;
            }
            else if ((hintMessage is THintPlayImage) || (hintMessage is THintPlayImageEx) || (hintMessage is THintImage))
            {
                hintMessage.Paint(paintRect, DrawScrnRect.Bounds(nX, paintRect.Top, hintMessage.Width, FItemHeight), ownerRect);
            }
            nX += hintMessage.Width;
        }

        nX = paintRect.Left;
        for (int i = 0; i < FList.Count; i++)
        {
            hintMessage = (THintMessage)FList[i];
            if (!((hintMessage is THintPlayImage) || (hintMessage is THintPlayImageEx) || (hintMessage is THintImage) || (hintMessage is TWinHintImage)))
            {
                hintMessage.Paint(paintRect, DrawScrnRect.Bounds(nX, paintRect.Top, hintMessage.Width, FItemHeight), ownerRect);
            }
            nX += hintMessage.Width;
        }
    }

    /// <summary>DrawScrn.pas:361/2145-2160 procedure PaintWinHintImage(OwnerRect, PaintRect:TRect)。</summary>
    public void PaintWinHintImage(TRect ownerRect, TRect paintRect)
    {
        int nX;
        THintMessage hintMessage;
        nX = paintRect.Left;
        // 将带播放的特效放在最前面放 chongchong 2015-01-22
        for (int i = 0; i < FList.Count; i++)
        {
            hintMessage = (THintMessage)FList[i];

            if (hintMessage is TWinHintImage)
            {
                hintMessage.Paint(paintRect, DrawScrnRect.Bounds(nX, paintRect.Top, hintMessage.Width, FItemHeight), ownerRect);
            }
            nX += hintMessage.Width;
        }
    }

    /// <summary>DrawScrn.pas:362 property OffsetX。</summary>
    public int OffsetX { get => FOffsetX; set => FOffsetX = value; }

    /// <summary>DrawScrn.pas:363 property OffsetY。</summary>
    public int OffsetY { get => FOffsetY; set => FOffsetY = value; }

    /// <summary>DrawScrn.pas:364 property Count:Integer read GetCount。</summary>
    public int Count => GetCount();

    /// <summary>DrawScrn.pas:365 property Width:Integer read FWidth。</summary>
    public int Width => FWidth;

    /// <summary>DrawScrn.pas:366 property Height:Integer read FHeight。</summary>
    public int Height => FHeight;

    /// <summary>DrawScrn.pas:367 property MinWidth:Integer read FMinWidth。</summary>
    public int MinWidth => FMinWidth;

    /// <summary>DrawScrn.pas:368 property ItemHeight:Integer read FItemHeight。</summary>
    public int ItemHeight => FItemHeight;

    /// <summary>DrawScrn.pas:369 property Alignment:TAlignment。</summary>
    public TAlignment Alignment { get => FAlignment; set => FAlignment = value; }

    /// <summary>DrawScrn.pas:370 property IsItemIconText:Boolean。</summary>
    public bool IsItemIconText { get => FIsItemIconText; set => FIsItemIconText = value; }

    /// <summary>DrawScrn.pas:371 property Objects[Index:Integer]:THintMessage read GetObject。</summary>
    public THintMessage Objects(int index) => GetObject(index);

    /// <summary>DrawScrn.pas:372 property Strings[Index:Integer]:string read Get write Put（读侧）。</summary>
    public string Strings(int index) => Get(index);

    /// <summary>DrawScrn.pas:372 property Strings[Index:Integer]:string read Get write Put（写侧）。</summary>
    public void SetStrings(int index, string value) => Put(index, value);
}

// ---------------------------------------------------------------------------------------------
// procedure ProcessHintText（DrawScrn.pas:735-737 声明 / 1453-2013 实现）
// ---------------------------------------------------------------------------------------------

/// <summary>
/// DrawScrn.pas:1453-2013 `procedure ProcessHintText(S:string; Owner:THintLines; List:TList;
///   Color:TColor = clWhite; FontSize:Integer = 9; FontStyles:TFontStyles = [];
///   IsStroke:Boolean = False; FontName:string = '')`。
/// </summary>
public static class DrawScrnHintTextParser
{
    /// <summary>原文 735-737 的单元级过程（含 8 个嵌套例程 + 折行/`&lt;Tag:…&gt;` 解析）。</summary>
    public static void ProcessHintText(string s, THintLines owner, List<object> list, TColor color,
        int fontSize = 9, TFontStyles fontStyles = TFontStyles.fsNone, bool isStroke = false, string fontName = "")
    {
        var ctx = new Ctx(owner, list, color, fontSize, fontStyles, isStroke, fontName);

        int index1 = DelphiRTL.Pos("<", s);
        if (index1 == 0)
        {
            ctx.NewHintText(s);
        }
        else
        {
            string strB = "";
            while (s != "")
            {
                strB = strB + DelphiRTL.Copy(s, 1, index1 - 1);
                s = DelphiRTL.Copy(s, index1, int.MaxValue);

                int index2 = DelphiRTL.Pos(">", s);
                if (index2 == 0)
                {
                    ctx.NewHintText(strB + s);
                    break;
                }
                else
                {
                    var spec = new HintImageSpec();
                    spec.IsVerticalAlignTop = false;

                    string strC = DelphiRTL.Copy(s, 1, index2);
                    if (!ctx.CheckHintImage(strC, spec))
                    {
                        strB = strB + strC;
                    }
                    else
                    {
                        if (strB.Length > 0)
                            ctx.NewHintText(strB);

                        if (spec.IsFixedTextWidth)
                            ctx.NewFixedWidthHintText(spec.ShowText, spec.OffsetX, spec.OffsetY);
                        else if (spec.IsPlayImg)
                            ctx.NewHintPlayImage(strC, spec.GameImages, spec.ImageIndex, spec.PlayCount, spec.PlayTime, spec.OffsetX, spec.OffsetY, spec.IsBlendDraw, spec.IncSpacing, spec.FixedHeight);
                        else if (spec.IsWinImage)
                            ctx.NewHintWinImage(strC, spec.GameImages, spec.ImageIndex, spec.OffsetX, spec.OffsetY, spec.MaxValue, spec.CurValue);
                        else if (spec.IsLineBGImage)
                            ctx.NewHintLineBGImage(strC, spec.GameImages, spec.ImageIndex, spec.OffsetX, spec.OffsetY, spec.MaxValue, spec.CurValue);
                        else if (spec.IsNewopUIPlayImg)
                            ctx.NewHintPlayImageEx(strC, spec.GameImages, spec.ImageIndex, spec.PlayCount, spec.PlayTime, spec.OffsetX, spec.OffsetY, spec.IsBlendDraw, spec.IncSpacing);
                        else if (spec.IsItemProgress)
                            ctx.NewHintItemProgress(spec.TextColor, spec.ShowText, spec.GameImages, spec.ImageIndex, spec.PlayCount, spec.MaxValue, spec.CurValue, spec.OffsetX, spec.OffsetY);
                        else if (spec.IsImgNumber)
                            ctx.NewHintImageNumber(spec.ShowText, spec.GameImages, spec.ImageIndex, spec.PlayCount, spec.OffsetX, spec.OffsetY);
                        else if (spec.IsCountdown)
                        {
                            ctx.NewHintCountdownText(spec.MaxValue, spec.TextColor, spec.OffsetX, spec.OffsetY);
                        }
                        else
                            ctx.NewHintImage(strC, spec.GameImages, spec.ImageIndex, spec.OffsetX, spec.OffsetY, spec.ShowBG, spec.FixedHeight, spec.IsVerticalAlignTop);

                        strB = "";
                    }

                    s = DelphiRTL.Copy(s, index2 + 1, int.MaxValue);
                    index1 = DelphiRTL.Pos("<", s);
                    if (index1 == 0)
                    {
                        s = strB + s;
                        if (s != "")
                            ctx.NewHintText(s);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// DrawScrn.pas:1711-1930 的嵌套 `function CheckHintImage(...):Boolean` 的
    /// **多返回值载体**（原文 24 个 `var` 形参 + 返回值）。
    /// </summary>
    public sealed class HintImageSpec
    {
        public TGameImages GameImages;
        public int ImageIndex;
        public int OffsetX;
        public int OffsetY;
        public int ShowBG;
        public int FixedHeight;
        public bool IsVerticalAlignTop;
        public bool IsPlayImg;
        public bool IsNewopUIPlayImg;
        public int PlayCount;
        public int PlayTime;
        public bool IsBlendDraw;
        public bool IsItemProgress;
        public bool IsImgNumber;
        public int MaxValue;
        public int CurValue;
        public byte TextColor;
        public string ShowText = "";
        public bool IsCountdown;
        public int IncSpacing;
        public bool IsWinImage;
        public bool IsLineBGImage;
        public bool IsFixedTextWidth;
    }

    /// <summary>原文 1453-2013 内部的全部嵌套例程与调用点上下文。</summary>
    private sealed class Ctx
    {
        private readonly THintLines _owner;
        private readonly List<object> _list;
        private readonly TColor _color;
        private readonly int _fontSize;
        private readonly TFontStyles _fontStyles;
        private readonly bool _isStroke;
        private readonly string _fontName;

        public Ctx(THintLines owner, List<object> list, TColor color, int fontSize,
            TFontStyles fontStyles, bool isStroke, string fontName)
        {
            _owner = owner;
            _list = list;
            _color = color;
            _fontSize = fontSize;
            _fontStyles = fontStyles;
            _isStroke = isStroke;
            _fontName = fontName;
        }

        /// <summary>原文 1456-1551 的嵌套 procedure NewHintText(Text:WideString)。</summary>
        public void NewHintText(string text)
        {
            THintText Make(string caption, TColor color)
            {
                var h = new THintText(_owner);
                h.Size = _fontSize;
                h.Style = _fontStyles;
                h.Color = color;
                h.IsStroke = _isStroke;
                h.Caption = caption;
                h.FFontName = _fontName;
                _list.Add(h);
                return h;
            }

            if (text.Length == 0)
            {
                Make(text, _color);
                return;
            }

            int index1 = DelphiRTL.Pos("{", text); // Pos('{', Text) HEZUQING 解决重载的问题 20230423
            int index2;
            if (index1 > 0)
                index2 = DelphiRTL.Pos("}", text);
            else
                index2 = 0;

            while ((index1 > 0) && (index2 > 0) && (text != ""))
            {
                string s1 = DelphiRTL.Copy(text, 1, index1 - 1);
                string s2 = DelphiRTL.Copy(text, index1 + 1, index2 - index1 - 1);

                if (s1.Length > 0)
                {
                    Make(s1, _color);
                }

                bool boCustomColorText = false;
                if (s2.Length > 0)
                {
                    int index3 = DelphiRTL.Pos("|", s2);
                    if (index3 > 0)
                    {
                        string s3 = DelphiRTL.Copy(s2, 1, index3 - 1);
                        string s4 = DelphiRTL.Copy(s2, index3 + 1, int.MaxValue);

                        int nColor = DelphiRTL.StrToIntDef(s4, -1);
                        if ((nColor >= 0) && (nColor <= 255))
                        {
                            boCustomColorText = true;

                            if (s3.Length > 0)
                            {
                                Make(s3, DrawScrnEnv.GetTColor((byte)nColor));
                            }
                        }
                    }
                }

                if (!boCustomColorText)
                {
                    Make("{" + s2 + "}", _color);
                }

                text = DelphiRTL.Copy(text, index2 + 1, int.MaxValue);
                index1 = DelphiRTL.Pos("{", text);
                if (index1 > 0)
                    index2 = DelphiRTL.Pos("}", text);
                else
                    index2 = 0;
            }

            if (text.Length > 0)
            {
                Make(text, _color);
            }
        }

        /// <summary>原文 1553-1572 的嵌套 procedure NewFixedWidthHintText(Text; FixedWidth; AColor)。</summary>
        public void NewFixedWidthHintText(string text, int fixedWidth, int aColor)
        {
            var hintText = new THintFixedWidthText(_owner);
            hintText.FMinWidth = fixedWidth;
            hintText.Size = _fontSize;
            hintText.Style = _fontStyles;
            if ((aColor >= 0) && (aColor <= 255))
                hintText.Color = DrawScrnEnv.GetTColor((byte)aColor);
            else
                hintText.Color = _color;
            hintText.IsStroke = _isStroke;
            hintText.Caption = text;
            hintText.FFontName = _fontName;
            _list.Add(hintText);
        }

        /// <summary>原文 1574-1590 的嵌套 procedure NewHintImage。</summary>
        public void NewHintImage(string text, TGameImages aGameImages, int aImageIndex,
            int offsetX, int offsetY, int showBG, int fixedHeight, bool isVerticalAlignTop)
        {
            if (aGameImages != null)
            {
                var hintImage = new THintImage(_owner);
                hintImage.FIsVerticalAlignTop = isVerticalAlignTop;
                hintImage.FGameImages = aGameImages;
                hintImage.FImageIndex = aImageIndex;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                hintImage.FShowBG = showBG;
                hintImage.FFixedHeight = fixedHeight;

                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1592-1606 的嵌套 procedure NewHintImageNumber。</summary>
        public void NewHintImageNumber(string text, TGameImages aGameImages, int numberIndex,
            int numberSpace, int offsetX, int offsetY)
        {
            if (aGameImages != null)
            {
                var hintImage = new THintImageNumber(_owner);
                hintImage.FGameImages = aGameImages;
                hintImage.FNumberIndex = numberIndex;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                hintImage.FNumberSpace = numberSpace;
                hintImage.FNumberValue = text;
                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1608-1627 的嵌套 procedure NewHintPlayImage。</summary>
        public void NewHintPlayImage(string text, TGameImages aGameImages, int aImageIndex, int aPlayCount,
            int aPlayTime, int offsetX, int offsetY, bool isBlendDraw, int fixedWith, int fixedHeight)
        {
            if (aGameImages != null)
            {
                var hintImage = new THintPlayImage(_owner);
                hintImage.FGameImages = aGameImages;
                hintImage.FImageIndex = aImageIndex;
                hintImage.FPlayImageIndex = aImageIndex;
                hintImage.FPlayCount = aPlayCount;
                hintImage.FPlayTime = aPlayTime;
                hintImage.FPlayTick = DrawScrnEnv.MyGetTickCount;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                hintImage.FBlendDraw = isBlendDraw;
                hintImage.FFixedWidth = fixedWith;
                hintImage.FFixedHeight = fixedHeight;
                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1629-1641 的嵌套 procedure NewHintWinImage。</summary>
        public void NewHintWinImage(string text, TGameImages aGameImages, int aImageIndex,
            int offsetX, int offsetY, int horzAligment, int vertAligment)
        {
            if (aGameImages != null)
            {
                var hintImage = new TWinHintImage(_owner);
                hintImage.FGameImages = aGameImages;
                hintImage.FImageIndex = aImageIndex;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1643-1655 的嵌套 procedure NewHintLineBGImage。</summary>
        public void NewHintLineBGImage(string text, TGameImages aGameImages, int aImageIndex,
            int offsetX, int offsetY, int horzAligment, int vertAligment)
        {
            if (aGameImages != null)
            {
                var hintImage = new TLineBGHintImage(_owner);
                hintImage.FGameImages = aGameImages;
                hintImage.FImageIndex = aImageIndex;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1657-1675 的嵌套 procedure NewHintPlayImageEx。</summary>
        public void NewHintPlayImageEx(string text, TGameImages aGameImages, int aImageIndex, int aPlayCount,
            int aPlayTime, int offsetX, int offsetY, bool isBlendDraw, int incSpacing)
        {
            if (aGameImages != null)
            {
                var hintImage = new THintPlayImageEx(_owner);
                hintImage.FGameImages = aGameImages;
                hintImage.FImageIndex = aImageIndex;
                hintImage.FPlayImageIndex = aImageIndex;
                hintImage.FPlayCount = aPlayCount;
                hintImage.FPlayTime = aPlayTime;
                hintImage.FPlayTick = DrawScrnEnv.MyGetTickCount;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                hintImage.FBlendDraw = isBlendDraw;
                hintImage.FIncSpacing = incSpacing;
                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1677-1696 的嵌套 procedure NewHintItemProgress。</summary>
        public void NewHintItemProgress(byte aColor, string text, TGameImages aGameImages, int aProgressIndex,
            int aPlayCount, int aMaxValue, int aCurValue, int offsetX, int offsetY)
        {
            if (aGameImages != null)
            {
                var hintImage = new THintItemProgress(_owner);
                hintImage.TextColor = DrawScrnEnv.GetTColor(aColor);
                hintImage.FText = text;
                hintImage.FGameImages = aGameImages;
                hintImage.FProgressIndex = aProgressIndex;
                hintImage.FPlayCount = aPlayCount;
                hintImage.FMaxValue = aMaxValue;
                hintImage.FCurValue = aCurValue;
                hintImage.FOffsetX = offsetX;
                hintImage.FOffsetY = offsetY;
                hintImage.FPlayTick = DrawScrnEnv.MyGetTickCount;
                hintImage.FPlayImageIndex = 0;
                _list.Add(hintImage);
            }
        }

        /// <summary>原文 1698-1709 的嵌套 procedure NewHintCountdownText。</summary>
        public void NewHintCountdownText(int aMaxValue, byte aColor, int offsetX, int offsetY)
        {
            var hintImage = new TCountdownText(_owner);
            hintImage.FCountdownValue = aMaxValue;
            hintImage.FStartTick = DrawScrnEnv.MyGetTickCount;
            hintImage.FTextColor = DrawScrnEnv.GetTColor(aColor);
            hintImage.FOffsetX = offsetX;
            hintImage.FOffsetY = offsetY;
            _list.Add(hintImage);
        }

        /// <summary>原文 1711-1930 的嵌套 function CheckHintImage（返回 True 时 spec 已填好）。</summary>
        public bool CheckHintImage(string text, HintImageSpec spec)
        {
            // '<Img:N:F:X:Y>'
            spec.GameImages = null;

            if (text.Length <= 2)
                return false;

            string temp = DelphiRTL.Copy(text, 2, text.Length - 2);
            string sName = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "", s7 = "", s8 = "", s9 = "";
            temp = HUtil32.GetValidStr3_Ex(temp, ref sName, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s1, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s2, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s3, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s4, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s5, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s6, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s7, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s8, ':');
            temp = HUtil32.GetValidStr3_Ex(temp, ref s9, ':');

            int i1 = DelphiRTL.StrToIntDef(s1, -1);
            int i2 = DelphiRTL.StrToIntDef(s2, -1);
            spec.IsPlayImg = false;
            spec.IsNewopUIPlayImg = false;
            spec.ShowBG = 0;
            spec.FixedHeight = 0;
            spec.IsVerticalAlignTop = false;

            spec.IsBlendDraw = true;
            spec.IsItemProgress = false;
            spec.IsImgNumber = false;
            spec.IsCountdown = false;
            spec.IncSpacing = 0;
            spec.IsWinImage = false;
            spec.IsLineBGImage = false;
            spec.IsFixedTextWidth = false;

            bool result = false;

            // <img: xxxx
            if (DelphiRTL.UpperCase(sName) == "TEXTW" && (i1 >= 0))
            {
                spec.IsFixedTextWidth = true;
                spec.OffsetX = i1;
                spec.OffsetY = i2;

                temp = DelphiRTL.Copy(text, 2, text.Length - 2);
                temp = HUtil32.GetValidStr3_Ex(temp, ref sName, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s1, ':');
                spec.ShowText = HUtil32.GetValidStr3_Ex(temp, ref s2, ':');

                result = true;
            }
            else if (SameText(sName, "Img") && (i2 >= 0))
            {
                if (i1 >= 0)
                {
                    result = true;

                    if (i2 < DrawScrnEnv.g_EffectImageList.Count)
                        spec.GameImages = (TGameImages)DrawScrnEnv.g_EffectImageList[i2];

                    spec.ImageIndex = i1;
                    spec.OffsetX = DelphiRTL.StrToIntDef(s3, 0);
                    spec.OffsetY = DelphiRTL.StrToIntDef(s4, 0);
                    spec.ShowBG = DelphiRTL.StrToIntDef(s5, 0);
                    spec.FixedHeight = DelphiRTL.StrToIntDef(s6, 0);
                    spec.IsVerticalAlignTop = DelphiRTL.StrToIntDef(s7, 0) == 1;
                }
            }
            else if (SameText(sName, "Looks") && (i1 >= 0))
            {
                result = true;
                spec.GameImages = DrawScrnEnv.g_WBagItemImages[i1];
                spec.ImageIndex = i1 % 10000;
                spec.OffsetX = DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s3, 0);
                spec.ShowBG = DelphiRTL.StrToIntDef(s4, 0);
                spec.FixedHeight = DelphiRTL.StrToIntDef(s5, 0);
                spec.IsVerticalAlignTop = DelphiRTL.StrToIntDef(s6, 0) == 1;
            }
            else if (SameText(sName, "DnItems") && (i1 >= 0))
            {
                result = true;
                spec.GameImages = DrawScrnEnv.g_WDnItemImages[i1];
                spec.ImageIndex = i1 % 10000;
                spec.OffsetX = DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s3, 0);
                spec.ShowBG = DelphiRTL.StrToIntDef(s4, 0);
                spec.FixedHeight = DelphiRTL.StrToIntDef(s5, 0);
                spec.IsVerticalAlignTop = DelphiRTL.StrToIntDef(s6, 0) == 1;
            }
            else if (SameText(sName, "StateItem") && (i1 >= 0))
            {
                result = true;
                spec.GameImages = DrawScrnEnv.g_WStateItemImages[i1];
                spec.ImageIndex = i1 % 10000;
                spec.OffsetX = DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s3, 0);
                spec.ShowBG = DelphiRTL.StrToIntDef(s4, 0);
                spec.FixedHeight = DelphiRTL.StrToIntDef(s5, 0);
                spec.IsVerticalAlignTop = DelphiRTL.StrToIntDef(s6, 0) == 1;
            }

            // 格式: <NewopUIPlay:N:C:T:X:Y:M>
            // N表示播放开始图片,C表示播放张数,T表示播放速度(毫秒),X是横向坐标,Y是纵向坐标;M绘制模式
            else if (SameText(sName, "NewopPlayImg") && (i2 >= 0))
            {
                result = true;
                spec.IsNewopUIPlayImg = true;

                spec.PlayCount = i2;
                spec.PlayTime = DelphiRTL.StrToIntDef(s3, 0);

                if (spec.PlayTime <= 0)
                    spec.PlayTime = 100;

                if (spec.PlayCount > 0)
                    spec.GameImages = DrawScrnEnv.g_WNewopUIImages;

                spec.ImageIndex = i1;

                spec.OffsetX = DelphiRTL.StrToIntDef(s4, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s5, 0);
                spec.IsBlendDraw = DelphiRTL.StrToIntDef(s6, 0) != 0;
                spec.IncSpacing = DelphiRTL.StrToIntDef(s7, 0);
            }

            else if (SameText(sName, "NewopUI") && (i1 >= 0))
            {
                result = true;
                spec.GameImages = DrawScrnEnv.g_WNewopUIImages;
                spec.ImageIndex = i1;
                spec.OffsetX = DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s3, 0);
            }

            // 提示窗图片（位置随提示窗变化，不随行）
            else if (SameText(sName, "WinNewopUI") && (i1 >= 0))
            {
                result = true;
                spec.IsWinImage = true;
                spec.GameImages = DrawScrnEnv.g_WNewopUIImages;
                spec.ImageIndex = i1;
                spec.OffsetX = DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s3, 0);
                spec.MaxValue = DelphiRTL.StrToIntDef(s4, 0); // 水平对齐(0:左;1:中;1:右)
                spec.CurValue = DelphiRTL.StrToIntDef(s5, 0); // 垂直对齐(0:上;1:中;1:下)
            }

            // 提示窗图片（位置随提示窗变化，不随行）
            else if (SameText(sName, "LineNewopUI") && (i1 >= 0))
            {
                result = true;
                spec.IsLineBGImage = true;
                spec.GameImages = DrawScrnEnv.g_WNewopUIImages;
                spec.ImageIndex = i1;
                spec.OffsetX = DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s3, 0);
                spec.MaxValue = DelphiRTL.StrToIntDef(s4, 0); // 水平对齐(0:左;1:中;1:右)
                spec.CurValue = DelphiRTL.StrToIntDef(s5, 0); // 垂直对齐(0:上;1:中;1:下)
            }

            // 格式: <PlayImg:F:N:C:T:X:Y:M/@Label>
            // F表示WIL文件序号,N表示播放开始图片,C表示播放张数,T表示播放速度(毫秒),X是横向坐标,Y是纵向坐标;M绘制模式
            else if (SameText(sName, "PlayImg") && (i1 >= 0) && (i2 >= 0))
            {
                result = true;
                spec.IsPlayImg = true;

                spec.PlayCount = DelphiRTL.StrToIntDef(s3, 0);
                spec.PlayTime = DelphiRTL.StrToIntDef(s4, 0);

                if (spec.PlayTime <= 0)
                    spec.PlayTime = 100;

                if ((i1 < DrawScrnEnv.g_EffectImageList.Count) && (spec.PlayCount > 0))
                    spec.GameImages = (TGameImages)DrawScrnEnv.g_EffectImageList[i1];

                spec.ImageIndex = i2;

                spec.OffsetX = DelphiRTL.StrToIntDef(s5, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s6, 0);
                spec.IsBlendDraw = DelphiRTL.StrToIntDef(s7, 0) != 0;
                spec.FixedHeight = DelphiRTL.StrToIntDef(s8, 0);
                spec.IncSpacing = DelphiRTL.StrToIntDef(s9, 0);
            }

            // 格式: <ItemProgress:N:C:M:V:X:Y:S/@Label>
            // N表示进度条序号; C表示进度条显示图片数量；M:进度条最大值; V:当前值; X,Y: 坐标偏移；R: 显示值颜色; S: 进度条显示的值
            else if (SameText(sName, "ItemProgress") && (i1 >= 0) && (i2 >= 0))
            {
                result = true;
                spec.IsItemProgress = true;

                spec.GameImages = DrawScrnEnv.g_WNewopUIImages;
                spec.ImageIndex = i1;
                spec.PlayCount = i2;

                spec.MaxValue = DelphiRTL.StrToIntDef(s3, 0);
                spec.CurValue = DelphiRTL.StrToIntDef(s4, 0);

                spec.OffsetX = DelphiRTL.StrToIntDef(s5, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s6, 0);
                spec.TextColor = (byte)DelphiRTL.StrToIntDef(s7, 0);
                spec.ShowText = s8;
            }
            else if (SameText(sName, "ImgNum") && (i1 >= 0) && (i1 <= 9) && (i2 > 0))
            {
                result = true;
                spec.IsImgNumber = true;

                spec.GameImages = DrawScrnEnv.g_WNewopUIImages;

                spec.ImageIndex = i1;                                  // 数字类型
                spec.ShowText = DelphiRTL.IntToStr(i2);                // 数字值
                spec.PlayCount = DelphiRTL.StrToIntDef(s3, 0);         // 字符间隔
                spec.OffsetX = DelphiRTL.StrToIntDef(s4, 0);           // X
                spec.OffsetY = DelphiRTL.StrToIntDef(s5, 0);           // Y
            }
            else if (SameText(sName, "Countdown") && (i1 >= 0))
            {
                result = true;
                spec.IsCountdown = true;
                spec.MaxValue = i1;
                spec.TextColor = (byte)DelphiRTL.StrToIntDef(s2, 0);
                spec.OffsetX = DelphiRTL.StrToIntDef(s3, 0);
                spec.OffsetY = DelphiRTL.StrToIntDef(s4, 0);
            }

            return result;
        }

        private static bool SameText(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
