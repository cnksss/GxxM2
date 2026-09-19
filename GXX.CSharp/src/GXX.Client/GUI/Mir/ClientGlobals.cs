using System;
using System.Drawing;
using GXX.Client.DxComponent;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using GXX.Core.Util;
using TGList = GXX.Core.Protocol.SDK.TGList;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【接缝】MShare.pas（客户端全局，11,922 行）与 ClFunc.pas 中本车道 5 个窗口实际引用的部分。
//
// 命名 100% 保留 Delphi 原名（g_ 前缀），使 .pas → .cs 可逐行对照；未列出的全局待 MShare
// 单元移植后统一迁入，本文件只做最小定义。
// ============================================================================================

/// <summary>Delphi Graphics.TColor（0x00BBGGRR 32 位值）。</summary>
public readonly struct TColor
{
    public readonly int Value;

    public TColor(int value) => Value = value;

    /// <summary>Windows RGB(r,g,b)：字节序 BGR。</summary>
    public static TColor FromBgr(int b, int g, int r) => new((r << 16) | (g << 8) | b);

    /// <summary>Delphi clBlack = $00000000。</summary>
    public static readonly TColor clBlack = new(0x00000000);

    /// <summary>Delphi clWhite = $00FFFFFF。</summary>
    public static readonly TColor clWhite = new(0x00FFFFFF);

    /// <summary>Delphi clRed = $000000FF。</summary>
    public static readonly TColor clRed = new(0x000000FF);

    /// <summary>Delphi clYellow = $0000FFFF。</summary>
    public static readonly TColor clYellow = new(0x0000FFFF);

    public static implicit operator Color(TColor c) => Color.FromArgb(255, (c.Value >> 16) & 0xFF, (c.Value >> 8) & 0xFF, c.Value & 0xFF);
    public static implicit operator int(TColor c) => c.Value;
}

/// <summary>THGEFont 的最小接缝（HGE.pas THGEFont；本车道只用 TextOut/TextWidth）。</summary>
public sealed class THGEFont
{
    /// <summary>测试捕获：每一次 TextOut(nX, nY, S, Color)。</summary>
    public readonly System.Collections.Generic.List<(int X, int Y, string S, TColor Color)> TextOuts = new();

    /// <summary>测试注入：TextOut 调用时同步回调（用于断言绘制走位）。</summary>
    public Action<int, int, string, TColor> TextOutHandler;

    /// <summary>HGE.pas THGEFont.TextOut(nX, nY:Integer; S:string; Color:TColor)。</summary>
    public void TextOut(int nX, int nY, string S, TColor Color)
    {
        TextOuts.Add((nX, nY, S, Color));
        TextOutHandler?.Invoke(nX, nY, S, Color);
    }

    /// <summary>HGE.pas THGEFont.TextWidth(S:string):Integer。</summary>
    public int TextWidth(string S) => S?.Length * 6 ?? 0;
}

/// <summary>HGE.pas 游戏画布的最小接缝（只记录绘制调用，供测试断言。真实实现待 HGE 单元移植）。</summary>
public sealed class TGameCanvas
{
    public readonly System.Collections.Generic.List<(int X, int Y, TTexture Texture)> Draws = new();
    public readonly System.Collections.Generic.List<(TRect Rect, TTexture Texture)> RectDraws = new();
    public readonly System.Collections.Generic.List<(TRect Rect, TColor Color)> Fills = new();

    /// <summary>HGE.pas GameCanvas.Draw(nX, nY:Integer; Texture:TTexture)。</summary>
    public void Draw(int nX, int nY, TTexture Texture) => Draws.Add((nX, nY, Texture));

    /// <summary>HGE.pas GameCanvas.Draw(nX, nY:Integer; SrcRect:TRect; Texture:TTexture)。</summary>
    public void Draw(int nX, int nY, TRect SrcRect, TTexture Texture) => RectDraws.Add((SrcRect, Texture));

    /// <summary>HGE.pas GameCanvas.FillRect(DestRect:TRect; Color:TColor)。</summary>
    public void FillRect(TRect DestRect, TColor Color) => Fills.Add((DestRect, Color));
}

/// <summary>
/// Actor.pas TActor 中本车道引用、而 Scenes 车道尚未承载的字段（扩展属性，不改动他人文件）。
/// g_Abil 已在 TActorCore 上；此处补 m_nGold / m_nGameGold。
/// </summary>
public static class ActorUiFields
{
    private sealed class Extra
    {
        public int m_nGold;
        public int m_nGameGold;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TActor, Extra> Extras = new();

    private static Extra Of(TActor actor) => Extras.GetOrCreateValue(actor);

    /// <summary>Actor.pas m_nGold（背包金币）。</summary>
    public static int GetGold(TActor actor) => Of(actor).m_nGold;
    public static void SetGold(TActor actor, int value) => Of(actor).m_nGold = value;

    /// <summary>Actor.pas m_nGameGold（元宝）。</summary>
    public static int GetGameGold(TActor actor) => Of(actor).m_nGameGold;
    public static void SetGameGold(TActor actor, int value) => Of(actor).m_nGameGold = value;
}

/// <summary>FState.pas TStateWindows（连击/205 版人物状态窗口，未移植）的最小接缝。</summary>
public class TStateWindows
{
    public int OpenUserStateCount;
    public int MySelfAbilChangeCount;

    /// <summary>SerialWindowsDlg.pas:4971 NewStateWindows.OpenUserState。</summary>
    public virtual void OpenUserState() => OpenUserStateCount++;

    /// <summary>SerialWindowsDlg.pas:10339 NewStateWindows.MySelfAbilChange。</summary>
    public virtual void MySelfAbilChange() => MySelfAbilChangeCount++;
}

/// <summary>ClMain.pas 主窗体（未移植）中本车道引用到的部分。</summary>
public static class frmMain
{
    /// <summary>ClMain.pas frmMain.boNpcDlgCanMove。</summary>
    public static bool boNpcDlgCanMove;
}

/// <summary>
/// MShare.pas / ClFunc.pas 的单元级全局与函数（原名保留）。
/// </summary>
public static class MShareGlobals
{
    // ---------------- 版本与登录模式 ----------------
    /// <summary>MShare.pas:2324 g_ClientVersion:TClientVersion = cvSerial。</summary>
    public static TClientVersion g_ClientVersion = TClientVersion.cvSerial;

    /// <summary>MShare.pas:72 CLIENT_MODE_NORMAL = 0。</summary>
    public const int CLIENT_MODE_NORMAL = 0;

    /// <summary>MShare.pas:1661 g_nClientLoginMode:Integer = 0。</summary>
    public static int g_nClientLoginMode = 0;

    // ---------------- 配置 ----------------
    /// <summary>MShare.pas:2180 g_ConfigClient:TConfigClient。</summary>
    public static TConfigClient g_ConfigClient = new();

    // ---------------- 场景/人物 ----------------
    /// <summary>MShare.pas:1864 g_MySelf:THumActor（未登录/未进入场景时为 nil）。</summary>
    public static TActor g_MySelf;

    /// <summary>MShare.pas:1888 g_UserState1:TUserStateInfo（观察对象形象；DUserStateForm1.IsMale 读 Feature.Buffer 偏移 2）。</summary>
    public static TUserStateInfo g_UserState1;

    /// <summary>MShare.pas:1708 g_MagicList:TGList（技能列表）。</summary>
    public static TGList g_MagicList = new();

    /// <summary>MShare.pas g_MagicList 分页起点（TSerialWindows 保护字段，此处按全局承载）。</summary>
    public static int MagicIndex;

    // ---------------- 物品 ----------------
    /// <summary>MShare.pas:2042 g_MouseItem:TClientItem（鼠标所指物品）。</summary>
    public static TClientItem g_MouseItem;

    /// <summary>MShare.pas:2259 g_boShowBagInfo:Boolean = False。</summary>
    public static byte g_boShowBagInfo;

    /// <summary>ClMain.pas g_boOpenMerchantBigDlg（NPC 大对话框标志）。</summary>
    public static byte g_boOpenMerchantBigDlg;

    // ---------------- 数值 ----------------
    /// <summary>MShare.pas:1830 g_nMySpeedPoint:Integer。</summary>
    public static int g_nMySpeedPoint;

    /// <summary>MShare.pas:1831 g_nMyHitPoint:Integer（准确）。</summary>
    public static int g_nMyHitPoint;

    /// <summary>MShare.pas:1832 g_nMyAntiPoison:Integer。</summary>
    public static int g_nMyAntiPoison;

    /// <summary>MShare.pas:1833 g_nMyPoisonRecover:Integer。</summary>
    public static int g_nMyPoisonRecover;

    /// <summary>MShare.pas:1834 g_nMyHealthRecover:Integer。</summary>
    public static int g_nMyHealthRecover;

    /// <summary>MShare.pas:1835 g_nMySpellRecover:Integer。</summary>
    public static int g_nMySpellRecover;

    /// <summary>MShare.pas:1836 g_nMyAntiMagic:Integer。</summary>
    public static int g_nMyAntiMagic;

    /// <summary>MShare.pas:1842 g_nGameDiamond:LongWord。</summary>
    public static uint g_nGameDiamond;

    /// <summary>MShare.pas:1843 g_nGameGird:LongWord（灵符）。</summary>
    public static uint g_nGameGird;

    // ---------------- 名称 ----------------
    /// <summary>MShare.pas:1439 g_sGameGoldName:string = '元宝'。</summary>
    public static string g_sGameGoldName = "元宝";

    /// <summary>MShare.pas:1440 g_sGameGirdName:string = '灵符'。</summary>
    public static string g_sGameGirdName = "灵符";

    /// <summary>MShare.pas:1441 g_sGameDiamondName:string = '金刚石'。</summary>
    public static string g_sGameDiamondName = "金刚石";

    // ---------------- 图库 ----------------
    /// <summary>MShare.pas:1505 g_WMainImages:TGameImages。</summary>
    public static TGameImages g_WMainImages = new(1200);

    /// <summary>MShare.pas:1507 g_WMain3Images:TGameImages。</summary>
    public static TGameImages g_WMain3Images = new(1200);

    /// <summary>MShare.pas:1515 g_WUINImages:TGameImages。</summary>
    public static TGameImages g_WUINImages = new(1400);

    /// <summary>MShare.pas g_MerchantImageIndex（NPC 对话框图号快照）。</summary>
    public static TGuiImageIndex g_MerchantImageIndex = new();

    /// <summary>MShare.pas g_MerchantCloseButtonRect（NPC 对话框关闭按钮矩形快照）。</summary>
    public static TRect g_MerchantCloseButtonRect;

    // ---------------- 绘制与字体 ----------------
    /// <summary>HGE.pas GameCanvas（全局绘制目标）。</summary>
    public static TGameCanvas GameCanvas = new();

    /// <summary>HGE.pas CurrentFont（全局字体对象）。</summary>
    public static THGEFont CurrentFont = new();

    /// <summary>Windows GetTickCount（ClMain.pas MyGetTickCount 等价）。</summary>
    public static uint MyGetTickCount => (uint)Environment.TickCount;

    // ================= 函数 =================

    /// <summary>MShare.pas:12129 function GetRGB(c256:Byte):Integer（调色板索引 → BGR）。</summary>
    public static int GetRGB(byte c256) => Palette256[c256];

    /// <summary>MShare.pas 256 色调色板（仅前 16 个系统色有确定值，其余待 MShare 移植后接入）。</summary>
    public static readonly int[] Palette256 = BuildPalette();

    private static int[] BuildPalette()
    {
        var p = new int[256];
        p[0] = 0x00000000;   // clBlack
        p[1] = 0x00800000;   // clMaroon
        p[2] = 0x00008000;   // clGreen
        p[3] = 0x00808000;   // clOlive
        p[4] = 0x00000080;   // clNavy
        p[5] = 0x00800080;   // clPurple
        p[6] = 0x00008080;   // clTeal
        p[7] = 0x00C0C0C0;   // clGray
        p[8] = 0x00808080;   // clSilver
        p[9] = 0x000000FF;   // clRed
        p[10] = 0x0000FF00;  // clLime
        p[11] = 0x0000FFFF;  // clYellow
        p[12] = 0x00FF0000;  // clBlue
        p[13] = 0x00FF00FF;  // clFuchsia
        p[14] = 0x00FFFF00;  // clAqua
        p[15] = 0x00FFFFFF;  // clWhite
        for (int i = 16; i < 256; i++) p[i] = 0x00FFFFFF;
        return p;
    }

    /// <summary>ClFunc.pas:50/203 function GetGoldStr(gold:LongWord):string。</summary>
    public static string GetGoldStr(uint gold) => GetGoldStrHandler?.Invoke(gold) ?? gold.ToString();

    /// <summary>测试注入点（真实千分位/亿万分段实现待 ClFunc 单元移植）。</summary>
    public static Func<uint, string> GetGoldStrHandler;

    /// <summary>
    /// MShare.pas:3026 function DrawItemHintOldStyle(nX, nY:Integer; S:WideString; Color:TColor = clWhite):Integer。
    /// 返回绘制宽度（调用方用 nX + n 续画）。
    /// </summary>
    public static int DrawItemHintOldStyle(int nX, int nY, string S, TColor Color = default)
        => DrawItemHintOldStyleHandler?.Invoke(nX, nY, S, Color) ?? (S?.Length * 6 ?? 0);

    /// <summary>测试注入点（真实文本绘制待 MShare/HGEFontEx 移植）。</summary>
    public static Func<int, int, string, TColor, int> DrawItemHintOldStyleHandler;

    /// <summary>
    /// FState.pas:917 procedure GetMouseItemInfo(Actor:TActor; MouseItem:pTClientItem; Secret:Boolean;
    /// var iname, line1, line2, line3:string; var IsUseableLine2, IsUseableLine3:Boolean)。
    /// 出参以字符串/布尔数组承载：texts[0..3] = iname/line1/line2/line3，flags[0..1] = IsUseableLine2/3。
    /// </summary>
    public static Action<TActor, TClientItem, bool, string[], bool[]> GetMouseItemInfoHandler;

    public static void GetMouseItemInfo(TActor Actor, TClientItem MouseItem, bool Secret,
        string[] texts, bool[] flags)
        => GetMouseItemInfoHandler?.Invoke(Actor, MouseItem, Secret, texts, flags);

    /// <summary>
    /// MShare.pas:9761 function GetItemDesc(Item:PTClientItem; sItemName:string):TStringList。
    /// Objects[i] 为 TColor（Delphi TStringList.Objects）。
    /// </summary>
    public static Func<TClientItem, string, TStringList> GetItemDescHandler;

    public static TStringList GetItemDesc(TClientItem Item, string sItemName)
        => GetItemDescHandler?.Invoke(Item, sItemName);
}

/// <summary>测试用：把全部 MShare 全局复位到单元初始化默认值。</summary>
public static class MShareGlobalsReset
{
    public static void ResetForTests()
    {
        MShareGlobals.g_ClientVersion = TClientVersion.cvSerial;
        MShareGlobals.g_nClientLoginMode = 0;
        MShareGlobals.g_ConfigClient = new TConfigClient();
        MShareGlobals.g_MySelf = null;
        MShareGlobals.g_UserState1 = default;
        MShareGlobals.g_MagicList = new TGList();
        MShareGlobals.MagicIndex = 0;
        MShareGlobals.g_MouseItem = default;
        MShareGlobals.g_boShowBagInfo = 0;
        MShareGlobals.g_boOpenMerchantBigDlg = 0;
        MShareGlobals.g_nMySpeedPoint = 0;
        MShareGlobals.g_nMyHitPoint = 0;
        MShareGlobals.g_nMyAntiPoison = 0;
        MShareGlobals.g_nMyPoisonRecover = 0;
        MShareGlobals.g_nMyHealthRecover = 0;
        MShareGlobals.g_nMySpellRecover = 0;
        MShareGlobals.g_nMyAntiMagic = 0;
        MShareGlobals.g_nGameDiamond = 0;
        MShareGlobals.g_nGameGird = 0;
        MShareGlobals.g_sGameGoldName = "元宝";
        MShareGlobals.g_sGameGirdName = "灵符";
        MShareGlobals.g_sGameDiamondName = "金刚石";
        MShareGlobals.g_WMainImages = new TGameImages(1200);
        MShareGlobals.g_WMain3Images = new TGameImages(1200);
        MShareGlobals.g_WUINImages = new TGameImages(1400);
        MShareGlobals.g_MerchantImageIndex = new TGuiImageIndex();
        MShareGlobals.g_MerchantCloseButtonRect = default;
        MShareGlobals.GameCanvas = new TGameCanvas();
        MShareGlobals.CurrentFont = new THGEFont();
        MShareGlobals.GetGoldStrHandler = null;
        MShareGlobals.DrawItemHintOldStyleHandler = null;
        MShareGlobals.GetMouseItemInfoHandler = null;
        MShareGlobals.GetItemDescHandler = null;
        SoundUtil.PlaySoundHandler = null;
        frmMain.boNpcDlgCanMove = false;
        ScreenSize.SCREENWIDTH = 1024;
        ScreenSize.SCREENHEIGHT = 768;
    }
}
