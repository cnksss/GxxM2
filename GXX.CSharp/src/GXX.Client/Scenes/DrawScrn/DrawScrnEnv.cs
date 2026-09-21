// ============================================================================================
// 车道 `p10-client-scrn`（DrawScrn.pas 5,542 行 + DropItemsMgr.pas 570 行）：
//   ① DrawScrn.pas 的**单元常量 / 共享记录类型**；
//   ② DrawScrn.pas 引用、而工程中尚无**正式归属**的外部类型与全局（接缝）。
//
// 设计原则（沿用台账 §25.2「接缝不得静默返回中性值」与 §12.8「只有在正式归属文件缺失时
// 才允许车道造接缝，且必须写明『待 <单元> 移植后由该类型接管』」）：
//   * 每一条接缝都标注**原文出处**（单元 + 行号），默认值等价于原文的哪一个分支；
//   * 已有正式归属的类型**一律复用**、不复刻：
//       TRect / TTexture / TGameImages / DxRect              → GXX.Client.GUI.DxComponent
//       TColor / THGEFont / TGameCanvas / ScreenSize / frmDlg → GXX.Client.GUI.Mir
//       TList / TShiftState / TMouseButton / TPoint / TFontStyles / MShareHintFont
//                                                            → GXX.Client.GUI.Share
//       TGList / TGStringList                                → GXX.Core.Protocol.SDK
//       TStringList / DelphiRTL / HUtil32                     → GXX.Core.Util
//   * 尚无归属的（TAlignment / TImageInfo / TStringToken / TStringLineEx / GetTextListEx /
//     GetStrinLineExText / TDrawScrnCanvas / DrawScrn 的 g_* 全局）在本文件与
//     `DrawScrnText.cs` 里落地，并在报告 §「归属申请」逐条登记。
// ============================================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Util;
using TGList = GXX.Core.Protocol.SDK.TGList;
using TGStringList = GXX.Core.Protocol.SDK.TGStringList;
using TList = GXX.Client.GUI.Share.TList;
using TRect = GXX.Client.GUI.DxComponent.TRect;

namespace GXX.Client.Scenes;

// ---------------------------------------------------------------------------------------------
// 1. DrawScrn.pas 单元常量（30-38）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:30-38 单元常量（命名原样保留）。</summary>
public static class DrawScrnConst
{
    /// <summary>DrawScrn.pas:31 MAXSYSLINE = 8。</summary>
    public const int MAXSYSLINE = 8;

    /// <summary>DrawScrn.pas:33 BOTTOMBOARD = 1。</summary>
    public const int BOTTOMBOARD = 1;

    /// <summary>DrawScrn.pas:34 VIEWCHATLINE = 9。</summary>
    public const int VIEWCHATLINE = 9;

    /// <summary>DrawScrn.pas:36 HEALTHBAR_BLACK = 0。</summary>
    public const int HEALTHBAR_BLACK = 0;

    /// <summary>DrawScrn.pas:37 HEALTHBAR_RED = 1。</summary>
    public const int HEALTHBAR_RED = 1;

    /// <summary>DrawScrn.pas:38 AREASTATEICONBASE = 150。</summary>
    public const int AREASTATEICONBASE = 150;
}

/// <summary>DrawScrn.pas:40-43 的注释块（HintWindow_BorderSpace 已注释掉，原文如此，不落地）。</summary>
// var HintWindow_BorderSpace: Integer = 8;   ← 原文整体被 { } 注释掉

// ---------------------------------------------------------------------------------------------
// 2. 枚举 / 记录（DrawScrn.pas:45-97、444-453、592-601）
// ---------------------------------------------------------------------------------------------

/// <summary>DrawScrn.pas:47 TMoveMessageOrientation（滚动消息方向 piaoyun 2013-08-02）。</summary>
public enum TMoveMessageOrientation
{
    /// <summary>水平</summary>
    mbHorizontal = 0,

    /// <summary>垂直</summary>
    mbVertical = 1,

    /// <summary>不滚动</summary>
    mbNone = 2,
}

/// <summary>
/// DrawScrn.pas:50-67 TMoveMsg（滚动消息结构体）。
/// <para>原文 `record` + `pTMoveMsg = ^TMoveMsg`（New/Dispose 堆对象）→ 托管 `sealed class`
/// （引用语义一致，`pTMoveMsg` 以 using 别名保留，见 `MoveMsgFamily.cs`）。</para>
/// </summary>
public sealed class TMoveMsg
{
    /// <summary>原文 TMoveMsg.Text:TStringLineEx。</summary>
    public TStringLineEx Text;

    /// <summary>原文 TMoveMsg.Alpha:Byte（字段存在但全文未被读写，原文如此）。</summary>
    public byte Alpha;

    public int Count;
    public int Y;
    public int Width;
    public int Height;
    public TMoveMessageOrientation Orientation;

    public bool ShowFrame;
    public byte FrameColor;
    public byte btFontSize;
    public bool boFontBold;

    public int nMarqueeTime;
}

/// <summary>DrawScrn.pas:69-77 TDelayMsg（延迟消息；pTDelayMsg = ^TDelayMsg → 托管 class）。</summary>
public sealed class TDelayMsg
{
    public long RecogId;
    public string Msg = "";
    public uint Time;
    public TColor FColor;
    public TColor BColor;
    public int X;
    public TStringLineEx Tokens;
}

/// <summary>DrawScrn.pas:80-84 THintMsg（提示信息结构体；托管值语义 struct）。</summary>
public struct THintMsg
{
    public string Msg;
    public TColor FColor;
}

/// <summary>DrawScrn.pas:86-90 THintWindowInfo（pTHintWindowInfo = ^THintWindowInfo → 托管 class）。</summary>
public sealed class THintWindowInfo
{
    public bool Visible;

    /// <summary>原文 `HintList:TList`（Delphi Classes.TList → 托管 `List&lt;object&gt;`，见 D-P10-07）。</summary>
    public List<object> HintList = new List<object>();
}

/// <summary>DrawScrn.pas:93-97 TSysMsg（系统消息结构体；pTSysMsg = ^TSysMsg → 托管 class）。</summary>
public sealed class TSysMsg
{
    public uint Time;
    public TColor FColor;
    public TColor BColor;
}

/// <summary>
/// DrawScrn.pas:444-453 TDrawScreenNewMsgCacheText（新消息缓存文本；
/// PDrawScreenNewMsgCacheText = ^TDrawScreenNewMsgCacheText → 托管 class）。
/// </summary>
public sealed class TDrawScreenNewMsgCacheText
{
    public string sMsg = "";
    public byte FColor;
    public byte BColor;
    public byte FontSize;
    public int nX;
    public int nY;
    public int nCount;
    public uint nAddTick;
    public int nTime;
    public int nDrawType;
}

/// <summary>
/// DrawScrn.pas:592-601 TMoveHintMsgRecord（PMoveHintMsgRecord = ^TMoveHintMsgRecord → 托管 class）。
/// </summary>
public sealed class TMoveHintMsgRecord
{
    public string Msg = "";
    public byte FColor;
    public byte BColor;
    public int nX;
    public int nY;
    public int nYOffset;
    public uint LastUpdateTick;
}

// ---------------------------------------------------------------------------------------------
// 3. 尚无正式归属的基础类型接缝
// ---------------------------------------------------------------------------------------------

/// <summary>
/// Delphi Classes.TAlignment（DrawScrn.pas 用到 taLeftJustify/taRightJustify/taCenter）。
/// 【归属申请】`GXX.Client.DxComponent` 已有 `TDxAlignment`（同值域），但原文此处用的是
/// Classes.TAlignment；在 DxComponent 侧统一之前，本类型由本车道承载（见交付报告 §归属申请）。
/// </summary>
public enum TAlignment
{
    taLeftJustify = 0,
    taRightJustify = 1,
    taCenter = 2,
}

/// <summary>
/// HGEFontEx.pas:15-23 `TImageIndexs = array of Integer` / `TImageInfo = record Width,Height:Integer; ImageIndexs:TImageIndexs`。
/// 【归属申请】HGEFontEx.pas 全量移植后应由 `GXX.Client.DxComponent`（DxComponents.cs 头注释已声明该单元归属）接管。
/// </summary>
public sealed class TImageInfo
{
    public int Width;
    public int Height;
    public List<int> ImageIndexs = new List<int>();
}

/// <summary>Delphi Windows.TRTLCriticalSection（DrawScrn 的 THintWindows / TDropItemsMgr 使用）。</summary>
public sealed class TRTLCriticalSection
{
    private readonly object _critical = new object();

    public void Enter() => Monitor.Enter(_critical);
    public void Leave() => Monitor.Exit(_critical);
}

/// <summary>
/// Delphi Windows 的矩形/数值辅助（DrawScrn.pas 大量使用 `Bounds` / `Rect` / `OffsetRect` /
/// `Max` / `Min`；`Bounds` 与 `Rect` 同义，此处都提供以保持调用点 1:1）。
/// </summary>
public static class DrawScrnRect
{
    /// <summary>Windows.Rect(Left, Top, Right, Bottom)。</summary>
    public static TRect Rect(int left, int top, int right, int bottom) => new TRect(left, top, right, bottom);

    /// <summary>Windows.Bounds(X, Y, Width, Height) = Rect(X, Y, X + Width, Y + Height)。</summary>
    public static TRect Bounds(int x, int y, int width, int height) => DxRect.Bounds(x, y, width, height);

    /// <summary>Windows.OffsetRect(var R:TRect; DX, DY:Integer)：原地平移。</summary>
    public static void OffsetRect(ref TRect rect, int dx, int dy)
    {
        rect.Left += dx;
        rect.Top += dy;
        rect.Right += dx;
        rect.Bottom += dy;
    }

    /// <summary>Delphi Math.Max（Integer 重载）。</summary>
    public static int Max(int a, int b) => a > b ? a : b;

    /// <summary>Delphi Math.Min（Integer 重载）。</summary>
    public static int Min(int a, int b) => a < b ? a : b;
}

// ---------------------------------------------------------------------------------------------
// 4. DrawScrn 的绘制出口（HGE.pas GameCanvas / HGEFontEx.pas THGEFont / ClFunc.pas BoldTextOut）
// ---------------------------------------------------------------------------------------------

/// <summary>绘制操作种类（供测试逐坐标断言）。</summary>
public enum ScrnDrawKind
{
    /// <summary>GameCanvas.Draw(nX, nY, Texture)</summary>
    Draw,

    /// <summary>GameCanvas.Draw(nX, nY, SrcRect, Texture)</summary>
    DrawRect,

    /// <summary>GameCanvas.StretchDraw(DestRect, Texture)</summary>
    StretchDraw,

    /// <summary>GameCanvas.FillRect(DestRect, c1, c2, b1, b2)</summary>
    FillRect,

    /// <summary>GameCanvas.FillRectAlpha(DestRect, Color, Alpha)</summary>
    FillRectAlpha,

    /// <summary>GameCanvas.DrawBlend(nX, nY, Texture)</summary>
    DrawBlend,

    /// <summary>ClFunc.BoldTextOut / BoldTextOutEx</summary>
    Text,

    /// <summary>HGEFont.TextRect(X, Y, SrcRect, ImageIndexs, Color)</summary>
    TextRect,
}

/// <summary>一次绘制调用（headless 记录；真实 HGE 接入后本记录器退化为空实现）。</summary>
public sealed class ScrnDrawOp
{
    public ScrnDrawKind Kind;
    public int X;
    public int Y;
    public TRect Rect;
    public TTexture Texture;
    public string Text = "";
    public TColor FColor;
    public TColor BColor = TColor.clBlack;
    public int Alpha = 255;
    public int Color1;
    public int Color2;

    public override string ToString()
        => $"{Kind}({X},{Y})[{Rect.Left},{Rect.Top},{Rect.Right},{Rect.Bottom}] '{Text}' a={Alpha}";
}

/// <summary>
/// HGE.pas `GameCanvas` / ClFunc.pas `BoldTextOut` / HGEFontEx.pas `THGEFont.TextRect` 的 headless 出口。
/// 【归属申请】HGE 单元与 HGEFontEx 单元全量移植后，本类应并入 `GXX.Client.HGE` 的真实画布；
/// `GXX.Client.GUI.Mir.MShareGlobals.GameCanvas`（另一条车道的接缝）亦应同时合并（见报告 §归属申请）。
/// </summary>
public sealed class TDrawScrnCanvas
{
    /// <summary>全部绘制调用（按发生顺序）。</summary>
    public readonly List<ScrnDrawOp> Ops = new List<ScrnDrawOp>();

    public void Clear() => Ops.Clear();

    /// <summary>HGE.pas GameCanvas.Draw(nX, nY:Integer; Texture:TTexture)。</summary>
    public void Draw(int nX, int nY, TTexture texture)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.Draw, X = nX, Y = nY, Texture = texture });

    /// <summary>HGE.pas GameCanvas.Draw(nX, nY:Integer; SrcRect:TRect; Texture:TTexture)。</summary>
    public void Draw(int nX, int nY, TRect srcRect, TTexture texture)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.DrawRect, X = nX, Y = nY, Rect = srcRect, Texture = texture });

    /// <summary>HGE.pas GameCanvas.StretchDraw(DestRect:TRect; Texture:TTexture)。</summary>
    public void StretchDraw(TRect destRect, TTexture texture)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.StretchDraw, Rect = destRect, Texture = texture });

    /// <summary>DxCanvas.pas GameCanvas.FillRect(ARect, Color1, Color2, BorderLight, BorderDark)。</summary>
    public void FillRect(TRect destRect, int color1, int color2, TColor borderLight, TColor borderDark)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.FillRect, Rect = destRect, Color1 = color1, Color2 = color2, FColor = borderLight, BColor = borderDark });

    /// <summary>HGE.pas GameCanvas.FillRectAlpha(DestRect:TRect; Color:TColor; Alpha:Byte)。</summary>
    public void FillRectAlpha(TRect destRect, TColor color, byte alpha)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.FillRectAlpha, Rect = destRect, FColor = color, Alpha = alpha });

    /// <summary>HGE.pas GameCanvas.DrawBlend(nX, nY:Integer; Texture:TTexture)。</summary>
    public void DrawBlend(int nX, int nY, TTexture texture)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.DrawBlend, X = nX, Y = nY, Texture = texture });

    /// <summary>ClFunc.pas BoldTextOut（已定位好的文字出口）。</summary>
    public void Text(int nX, int nY, string s, TColor fcolor, TColor bcolor, byte alpha = 255)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.Text, X = nX, Y = nY, Text = s ?? "", FColor = fcolor, BColor = bcolor, Alpha = alpha });

    /// <summary>HGEFontEx.pas THGEFont.TextRect(X, Y, SrcRect, ImageIndexs, Color)。</summary>
    public void TextRect(int nX, int nY, TRect srcRect, TColor color)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.TextRect, X = nX, Y = nY, Rect = srcRect, FColor = color });

    /// <summary>HGEFontEx.pas THGEFont.TextOut(X, Y, ImageIndexs, Color)。</summary>
    public void TextImageIndexs(int nX, int nY, TColor color)
        => Ops.Add(new ScrnDrawOp { Kind = ScrnDrawKind.Text, X = nX, Y = nY, FColor = color });
}

// ---------------------------------------------------------------------------------------------
// 5. DrawScrn.pas 引用的全局（接缝）
// ---------------------------------------------------------------------------------------------

/// <summary>
/// DrawScrn.pas 引用的 MShare.pas / SDK.pas / ClMain.pas / GameConfigDlg.pas / FState.pas 全局。
/// 命名 100% 保留原文（<c>g_</c> 前缀），使 .pas → .cs 可逐行对照。
/// </summary>
public static class DrawScrnEnv
{
    // ---------------- 屏幕尺寸（SDK.pas:324 SCREENWIDTH/SCREENHEIGHT） ----------------
    /// <summary>SDK.pas:324 SCREENWIDTH（复用车道 GUI/Mir 的 ScreenSize 接缝）。</summary>
    public static int SCREENWIDTH => ScreenSize.SCREENWIDTH;

    /// <summary>SDK.pas:325 SCREENHEIGHT。</summary>
    public static int SCREENHEIGHT => ScreenSize.SCREENHEIGHT;

    // ---------------- 时钟（MShare.pas MyGetTickCount） ----------------
    /// <summary>MyGetTickCount（原文 Windows GetTickCount，uint 回绕）。测试可注入以精确落点。</summary>
    public static Func<uint> MyGetTickCountFn = () => MShareGlobals.MyGetTickCount;

    /// <summary>原文 MyGetTickCount。</summary>
    public static uint MyGetTickCount => MyGetTickCountFn();

    /// <summary>MShare.pas tick_diff(tick_start, tick_end) = tick_end - tick_start（uint 回绕）。</summary>
    public static uint tick_diff(uint tickStart, uint tickEnd) => unchecked(tickEnd - tickStart);

    // ---------------- 调色板 / 颜色 ----------------
    /// <summary>MShare.pas:2897 function GetRGB(c256:Byte):Integer。</summary>
    public static int GetRGB(byte c256) => MShareGlobals.GetRGB(c256);

    /// <summary>同上但返回 TColor（Delphi Integer→TColor 隐式转换的等价写法）。</summary>
    public static TColor GetTColor(byte c256) => new TColor(MShareGlobals.GetRGB(c256));

    /// <summary>HUtil32.pas:212 function ColorIndexToTColor(ColorIndex:Byte):TColor。</summary>
    public static TColor ColorIndexToTColor(byte colorIndex)
        => new TColor(GXX.Client.ReadResources.GDefColorTable.GetARGB32(colorIndex) & 0x00FFFFFF);

    /// <summary>Delphi Graphics.clLime = $0000FF00（GUI/Mir 的 TColor 未定义，此处补齐）。</summary>
    public static readonly TColor clLime = new TColor(0x0000FF00);

    /// <summary>Delphi Graphics.clNone = $1FFFFFFF（GetTextListEx 的 LineBackColor 默认值）。</summary>
    public static readonly TColor clNone = new TColor(0x1FFFFFFF);

    /// <summary>MShare.pas clBlack1（提示窗分界线填充用的边框色）。</summary>
    public static readonly TColor clBlack1 = new TColor(0x00000000);

    // ---------------- 字体（MShare.pas g_sCurFontName / HGE.pas CurrentFont / HGEFontEx TextureFonts） ----------------
    /// <summary>MShare.pas g_sCurFontName（当前字形名）。</summary>
    public static string g_sCurFontName = "宋体";

    /// <summary>HGE.pas CurrentFont（全局字体对象）。</summary>
    public static THGEFont CurrentFont = MShareGlobals.CurrentFont;

    /// <summary>MShare.pas g_CurrentFontHeight（CurrentFont 的行高）。</summary>
    public static int g_CurrentFontHeight = 14;

    /// <summary>HGEFontEx.pas THGEFonts.FindFont(AFontName, AFontSize, AFontStyles)。</summary>
    public static Func<string, int, TFontStyles, THGEFont> FindFontFn;

    /// <summary>
    /// 原文 `TextureFonts.FindFont(...)`。
    /// 默认实现返回全局 <see cref="CurrentFont"/>（非 null）—— 对应原文"字体已注册"的常规分支；
    /// 测试可用 <see cref="FindFontFn"/> 置 null 走原文 `HGEFont = nil` 的守卫分支。
    /// </summary>
    public static THGEFont FindFont(string name, int size, TFontStyles style = TFontStyles.fsNone)
        => FindFontFn != null ? FindFontFn(name, size, style) : CurrentFont;

    /// <summary>HGEFontEx.pas THGEFont.TextHeight。</summary>
    public static Func<THGEFont, string, int> FontTextHeightFn;

    /// <summary>HGEFontEx.pas THGEFont.TextWidth。</summary>
    public static Func<THGEFont, string, int> FontTextWidthFn;

    /// <summary>
    /// HGEFontEx.pas THGEFont.TextHeight 的调用点。
    /// 默认走 `GXX.Client.DxComponent.TDxFontEnv.MeasureTextHeight`（HGEFontEx.pas:462-472 的纯公式）。
    /// </summary>
    public static int TextHeight(THGEFont font, string s)
        => FontTextHeightFn != null
         ? FontTextHeightFn(font, s)
         : GXX.Client.DxComponent.TDxFontEnv.MeasureTextHeight(s);

    /// <summary>HGEFontEx.pas `TextHeight('Pp')`（原文固定以 'Pp' 取行高）。</summary>
    public static int TextHeightPp(THGEFont font) => TextHeight(font, "Pp");

    /// <summary>HGEFontEx.pas THGEFont.TextWidth 的调用点（默认走 THGEFont.TextWidth 接缝）。</summary>
    public static int TextWidth(THGEFont font, string s)
        => FontTextWidthFn != null ? FontTextWidthFn(font, s) : (font?.TextWidth(s) ?? 0);

    /// <summary>HGEFontEx.pas THGEFont.GetImageInfo(Text):TImageInfo。</summary>
    public static Func<THGEFont, string, TImageInfo> FontGetImageInfoFn;

    /// <summary>HGEFontEx.pas THGEFont.GetImageInfo 的调用点。</summary>
    public static TImageInfo GetImageInfo(THGEFont font, string s)
    {
        if (FontGetImageInfoFn != null) return FontGetImageInfoFn(font, s);
        return new TImageInfo { Width = TextWidth(font, s), Height = TextHeight(font, s) };
    }

    // ---------------- 图库（MShare.pas g_W*Images / TGameImages.GetCachedImage） ----------------
    /// <summary>MShare.pas g_WNewopUIImages（NewopUI.pak 图库）。</summary>
    public static TGameImages g_WNewopUIImages = new TGameImages(1200);

    /// <summary>MShare.pas g_WBagItemImages（背包物品图库）。</summary>
    public static TLookImages g_WBagItemImages = new TLookImages();

    /// <summary>MShare.pas g_WDnItemImages（地上物品图库）。</summary>
    public static TLookImages g_WDnItemImages = new TLookImages();

    /// <summary>MShare.pas g_WStateItemImages（状态物品图库）。</summary>
    public static TLookImages g_WStateItemImages = new TLookImages();

    /// <summary>MShare.pas g_EffectImageList（特效图库列表；Objects[I] 为 TGameImages）。</summary>
    public static TGList g_EffectImageList = new TGList();

    /// <summary>GameImages.pas TGameImages.GetCachedImage(Index; out X, out Y):TTexture。</summary>
    public static Func<TGameImages, int, (TTexture Texture, int X, int Y)> GetCachedImageFn;

    /// <summary>
    /// 原文 `FGameImages.GetCachedImage(Index, X, Y)`。
    /// 默认实现退化为 `Images[Index]`（X/Y 取 0）并**不隐瞒**这一退化：报告 §偏离登记 已登记 D-P10-03。
    /// </summary>
    public static TTexture GetCachedImage(TGameImages images, int index, out int x, out int y)
    {
        if (GetCachedImageFn != null)
        {
            var r = GetCachedImageFn(images, index);
            x = r.X;
            y = r.Y;
            return r.Texture;
        }
        x = 0;
        y = 0;
        return images != null ? images[index] : null;
    }

    // ---------------- 客户端配置（MShare.pas g_ClientConfig） ----------------
    /// <summary>
    /// MShare.pas `g_ClientConfig.HintWindowBorderWidth:TRect`（左/上/右/下四边内缩，原文 8/8/8/8）。
    /// 【归属申请】TConfigClient 的正式归属在 `GXX.Client.GUI.Mir`，待其补上该字段后本接缝删除。
    /// </summary>
    public static TRect HintWindowBorderWidth = new TRect(8, 8, 8, 8);

    /// <summary>MShare.pas g_ClientConfig.boShowHintWindowFrame（提示窗边框开关）。</summary>
    public static bool boShowHintWindowFrame = true;

    /// <summary>MShare.pas g_ClientConfig.btHintWindowbackgroundColor（提示窗背景色索引）。</summary>
    public static byte btHintWindowbackgroundColor = 0;

    /// <summary>MShare.pas g_ClientConfig.btHintWindowbackgroundAlpha（提示窗背景透明度）。</summary>
    public static byte btHintWindowbackgroundAlpha = 100;

    /// <summary>MShare.pas g_ClientConfig.boShowGreenHint（左上角绿色信息开关）。</summary>
    public static bool boShowGreenHint = true;

    /// <summary>MShare.pas g_ClientConfig.boGreenHintNewStyle（新式绿色信息）。</summary>
    public static bool boGreenHintNewStyle = false;

    /// <summary>MShare.pas g_ClientConfig.boHumStruckShowNumber（人物受击显示血量）。</summary>
    public static bool boHumStruckShowNumber = true;

    /// <summary>MShare.pas g_ClientConfig.boMonStruckShowNumber（怪物受击显示血量）。</summary>
    public static bool boMonStruckShowNumber = true;

    /// <summary>MShare.pas g_LastHintMakeIndex（THintWindows.Clear 回写 -1）。</summary>
    public static int g_LastHintMakeIndex = -1;

    // ---------------- 绿色信息（DrawScreen 的 g_MySelf/g_FocusCret/g_MyHero/鼠标坐标） ----------------
    /// <summary>MShare.pas:1864 g_MySelf（复用车道 GUI/Mir 的 MShareGlobals）。</summary>
    public static TActor g_MySelf { get => MShareGlobals.g_MySelf; set => MShareGlobals.g_MySelf = value; }

    /// <summary>MShare.pas g_FocusCret（当前选中目标）。</summary>
    public static TActor g_FocusCret;

    /// <summary>MShare.pas:1865 g_MyHero。</summary>
    public static TActor g_MyHero;

    /// <summary>MShare.pas g_nMouseCurrX（鼠标当前格 X）。</summary>
    public static int g_nMouseCurrX;

    /// <summary>MShare.pas g_nMouseCurrY。</summary>
    public static int g_nMouseCurrY;

    /// <summary>MShare.pas g_nMouseX（鼠标屏幕 X）。</summary>
    public static int g_nMouseX;

    /// <summary>MShare.pas g_nMouseY。</summary>
    public static int g_nMouseY;

    /// <summary>MShare.pas g_nAreaStateValue（区域状态位：$01 shl I 战斗/安全标记，$04 攻城区域）。</summary>
    public static int g_nAreaStateValue;

    /// <summary>MShare.pas g_sGoldName（金币名）。</summary>
    public static string g_sGoldName = "金币";

    /// <summary>MShare.pas:1439 g_sGameGoldName。</summary>
    public static string g_sGameGoldName { get => MShareGlobals.g_sGameGoldName; set => MShareGlobals.g_sGameGoldName = value; }

    /// <summary>MShare.pas g_nMoveMouseX（鼠标悬停提示 X）。</summary>
    public static int g_nMoveMouseX;

    /// <summary>MShare.pas g_nMoveMouseY。</summary>
    public static int g_nMoveMouseY;

    // ---------------- 插件/配置开关 ----------------
    /// <summary>MShare.pas PlugInEnabled（外挂插件启用）。</summary>
    public static bool PlugInEnabled;

    /// <summary>GameConfigDlg.pas g_ConfigDlg.ConfigCheckeds[ckShowGreenHint]。</summary>
    public static Func<int, bool> ConfigCheckedFn = _ => false;

    /// <summary>GameConfigDlg.pas g_ConfigDlg.GetShowItem(DBName):pTShowItem。</summary>
    public static Func<string, object> GetShowItemFn;

    /// <summary>原文 `g_ConfigDlg.GetShowItem(DBName)`。</summary>
    public static object GetShowItem(string dbName) => GetShowItemFn?.Invoke(dbName);

    /// <summary>原文 `g_ConfigDlg.ConfigCheckeds[ckShowGreenHint]`（GameConfigDlg.pas:100 ckShowGreenHint = 4）。</summary>
    public static bool ConfigCheckeds_ckShowGreenHint => ConfigCheckedFn(4);

    // ---------------- 提示字体三函数（MShare.pas:11735/11740/11752，接缝在 GUI/Share） ----------------
    /// <summary>MShare.pas:11735 GetHintFontSize。</summary>
    public static int GetHintFontSize => MShareHintFont.GetHintFontSize();

    /// <summary>MShare.pas:11740 GetHintFontStyle。</summary>
    public static TFontStyles GetHintFontStyle(TFontStyles styles) => MShareHintFont.GetHintFontStyle(styles);

    /// <summary>MShare.pas:11752 GetHintFontStroke。</summary>
    public static bool GetHintFontStroke(bool isStroke = false) => MShareHintFont.GetHintFontStroke(isStroke);

    // ---------------- ClMain.pas FrmDlg（NPC 对话框 / 聊天框 / 随机码弹窗） ----------------
    /// <summary>ClMain.pas FrmDlg.DChatMemo（聊天框）。</summary>
    public static IChatMemo DChatMemo = new ChatMemoSeam();

    /// <summary>ClMain.pas FrmDlg.DMerchantDlg（NPC 对话框；null = 未打开）。</summary>
    public static Func<bool> DMerchantDlgVisibleFn = () => false;

    /// <summary>ClMain.pas FrmDlg.DMerchantDlg.VirtualRect。</summary>
    public static Func<TRect> DMerchantDlgVirtualRectFn = () => default;

    /// <summary>ClMain.pas FrmDlg.OpenDRandomCodeDlg。</summary>
    public static Action OpenDRandomCodeDlgFn;

    /// <summary>ClMain.pas FrmDlg.DMerchantDlg.Visible。</summary>
    public static bool DMerchantDlg_Visible => DMerchantDlgVisibleFn();

    /// <summary>ClMain.pas FrmDlg.DMerchantDlg.VirtualRect。</summary>
    public static TRect DMerchantDlg_VirtualRect => DMerchantDlgVirtualRectFn();

    /// <summary>ClMain.pas FrmDlg.OpenDRandomCodeDlg。</summary>
    public static void OpenDRandomCodeDlg() => OpenDRandomCodeDlgFn?.Invoke();

    // ---------------- PlayScn.pas 场景（IsValidActorEx） ----------------
    /// <summary>PlayScn.pas:6954-6967 IsValidActorEx（默认恒真，见 D-P10-04）。</summary>
    public static Func<TActor, bool> IsValidActorExFn = _ => true;

    /// <summary>原文 `PlayScene.IsValidActorEx(Actor)`。</summary>
    public static bool IsValidActorEx(TActor actor) => IsValidActorExFn(actor);

    // ---------------- 画布 / 文本绘制出口 ----------------
    /// <summary>HGE.pas GameCanvas（headless 出口）。</summary>
    public static TDrawScrnCanvas GameCanvas = new TDrawScrnCanvas();

    /// <summary>ClFunc.pas BoldTextOut(X, Y, S, FColor)（用 CurrentFont）。</summary>
    public static void BoldTextOut(int x, int y, string s, TColor fcolor)
        => GameCanvas.Text(x, y, s, fcolor, TColor.clBlack);

    /// <summary>ClFunc.pas BoldTextOut(X, Y, S, FColor, BColor)。</summary>
    public static void BoldTextOut(int x, int y, string s, TColor fcolor, TColor bcolor)
        => GameCanvas.Text(x, y, s, fcolor, bcolor);

    /// <summary>ClFunc.pas BoldTextOut(X, Y, S, FColor, BColor, Alpha)。</summary>
    public static void BoldTextOut(int x, int y, string s, TColor fcolor, TColor bcolor, int alpha)
        => GameCanvas.Text(x, y, s, fcolor, bcolor, (byte)Math.Clamp(alpha, 0, 255));

    /// <summary>ClFunc.pas BoldTextOut(HGEFont, X, Y, S, FColor, BColor)。</summary>
    public static void BoldTextOut(THGEFont font, int x, int y, string s, TColor fcolor, TColor bcolor)
        => GameCanvas.Text(x, y, s, fcolor, bcolor);

    /// <summary>ClFunc.pas BoldTextOut(HGEFont, X, Y, S, FColor, BColor, Alpha)。</summary>
    public static void BoldTextOut(THGEFont font, int x, int y, string s, TColor fcolor, TColor bcolor, int alpha)
        => GameCanvas.Text(x, y, s, fcolor, bcolor, (byte)Math.Clamp(alpha, 0, 255));

    /// <summary>ClFunc.pas BoldTextOutEx(HGEFont, X, Y, S, FColor, BColor, Alpha)。</summary>
    public static void BoldTextOutEx(THGEFont font, int x, int y, string s, TColor fcolor, TColor bcolor, int alpha)
        => GameCanvas.Text(x, y, s, fcolor, bcolor, (byte)Math.Clamp(alpha, 0, 255));

    /// <summary>MShare.pas/ClMain.pas DebugOutStr(S:string)。</summary>
    public static Action<string> DebugOutStrFn;

    /// <summary>原文 DebugOutStr。</summary>
    public static void DebugOutStr(string s) => DebugOutStrFn?.Invoke(s);

    /// <summary>StrUtils.pas AnsiReplaceText（大小写不敏感的全量替换）。</summary>
    public static string AnsiReplaceText(string s, string a, string b)
        => s == null ? "" : s.Replace(a, b);

    /// <summary>测试复位：把本类全部接缝恢复到单元初始化默认值。</summary>
    public static void ResetForTests()
    {
        MyGetTickCountFn = () => MShareGlobals.MyGetTickCount;
        g_sCurFontName = "宋体";
        CurrentFont = new THGEFont();
        MShareGlobals.CurrentFont = CurrentFont;
        g_CurrentFontHeight = 14;
        FindFontFn = null;
        FontTextHeightFn = null;
        FontTextWidthFn = null;
        FontGetImageInfoFn = null;
        g_WNewopUIImages = new TGameImages(1200);
        g_WBagItemImages = new TLookImages();
        g_WDnItemImages = new TLookImages();
        g_WStateItemImages = new TLookImages();
        g_EffectImageList = new TGList();
        GetCachedImageFn = null;
        HintWindowBorderWidth = new TRect(8, 8, 8, 8);
        boShowHintWindowFrame = true;
        btHintWindowbackgroundColor = 0;
        btHintWindowbackgroundAlpha = 100;
        boShowGreenHint = true;
        boGreenHintNewStyle = false;
        boHumStruckShowNumber = true;
        boMonStruckShowNumber = true;
        g_LastHintMakeIndex = -1;
        g_MySelf = null;
        g_FocusCret = null;
        g_MyHero = null;
        g_nMouseCurrX = 0;
        g_nMouseCurrY = 0;
        g_nMouseX = 0;
        g_nMouseY = 0;
        g_nAreaStateValue = 0;
        g_sGoldName = "金币";
        g_sGameGoldName = "元宝";
        g_nMoveMouseX = 0;
        g_nMoveMouseY = 0;
        PlugInEnabled = false;
        ConfigCheckedFn = _ => false;
        GetShowItemFn = null;
        DChatMemo = new ChatMemoSeam();
        DMerchantDlgVisibleFn = () => false;
        DMerchantDlgVirtualRectFn = () => default;
        OpenDRandomCodeDlgFn = null;
        IsValidActorExFn = _ => true;
        GameCanvas = new TDrawScrnCanvas();
        DebugOutStrFn = null;
        ScreenSize.SCREENWIDTH = 1024;
        ScreenSize.SCREENHEIGHT = 768;
    }
}

/// <summary>
/// MShare.pas `g_WBagItemImages.Looks[I]`（按 looks 取图库）。
/// 原文是 `Looks: array[...] of TGameImages`，托管侧以可注入索引器承载。
/// </summary>
public sealed class TLookImages
{
    /// <summary>Looks[Index]（越界返回 null，对应原文数组越界的 nil 分支）。</summary>
    public Func<int, TGameImages> Resolver;

    public TGameImages this[int index] => Resolver?.Invoke(index);
}

/// <summary>ClMain.pas / DxMemo.pas `TDxChatMemo` 在 DrawScrn.pas 里用到的两个方法。</summary>
public interface IChatMemo
{
    /// <summary>TDxChatMemo.Add(Str, FColor, BColor)。</summary>
    void Add(string str, int fcolor, int bcolor);

    /// <summary>TDxChatMemo.AddTop(Str, FColor, BColor, TimeOut)。</summary>
    void AddTop(string str, int fcolor, int bcolor, int timeOut);
}

/// <summary>
/// 【接缝：待 `GXX.Client.DxComponent.DxMemo` 的 TDxChatMemo 移植后接入真实实现】
/// 默认实现只记录调用，供测试断言 TDrawScreen.AddChatBoardString / AddTopChatBoardString 的转发。
/// </summary>
public sealed class ChatMemoSeam : IChatMemo
{
    public readonly List<(string Str, int FColor, int BColor)> Adds = new List<(string, int, int)>();
    public readonly List<(string Str, int FColor, int BColor, int TimeOut)> TopAdds = new List<(string, int, int, int)>();

    public void Add(string str, int fcolor, int bcolor) => Adds.Add((str, fcolor, bcolor));

    public void AddTop(string str, int fcolor, int bcolor, int timeOut) => TopAdds.Add((str, fcolor, bcolor, timeOut));
}
