using System;
using GXX.Client.GUI.DxComponent;
using GXX.Core.Rtl;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【接缝】SerialWindowsDlg.pas TSerialWindows（1.5MB / 38,480 行的客户端主 UI 基类，未移植）。
//
// 本文件只承载 GUI/Mir 下 5 个派生窗口（HeroWindowsDlg / Mir185WindowsDlg / MirSequelDlg /
// Mir176WindowsDlg / MirNewUI205Dlg）实际用到的成员，并按原名保留；方法体为原文对应行的
// 1:1 摘录（已标注行号），其余成员待 SerialWindowsDlg 单元移植后补齐。
//
// 控件的几何/可见性由 SerialWindowsDlg.pas:17323 TSerialWindows.LoadFromStream 由 GUI 资源流
// 载入（该族窗口**没有** .dfm，布局来自 Mir.GUI 资源），因此本接缝中控件为逻辑控件对象
// （非 WinForms 控件），几何初值由 Open/测试直接设定，与原文运行期行为一致。
// ============================================================================================

public class TSerialWindows : TFrmDlg
{
    // ---------------- TSerialWindows 类声明的控件字段（TSerialWindowsDlg.pas:83 起） ----------------
    public TDxImageForm DLogin;                      // 84
    public TDxEdit DEdId_;                           // 85
    public TDxEdit DEdPasswd_;                       // 86
    public TDxImageButton DImageButtonAccount;       // 87
    public TDxImageButton DImageButtonPassWord;      // 88
    public TDxImageButton DLoginChgPw;               // 89
    public TDxImageButton DLoginClose;               // 90
    public TDxImageButton DLoginNew;                 // 91
    public TDxImageButton DLoginOK;                  // 92
    public TDxImageForm DSelectChr;                  // 165
    public TDxImageForm DDeleteHumanDlg;             // 179
    /// <summary>SerialWindowsDlg.pas:17377 DMerchantDlgHelp_ → DMerchantDlgHelp 的别名（176 版在 LoadFromStream 中直接置 Visible）。</summary>
    public TDxImageButton DMerchantDlgHelp_;
    public TDxLabel DGoldValue;                      // 353
    public TDxLabel DlblItemBagGold;                 // 362
    public TDxLabel DlblItemBagGird;                 // 361
    public TDxLabel DlblItemBagDiamond;              // 360
    public TDxLabel DlblItemBagRefresh;
    public TDxLabel DStateWeightLabel;               // 468
    public TDxLabel DStateWearWeightLabel;           // 467
    public TDxLabel DStateHandWeightLabel;           // 459
    public TDxSexPanel DStateForm1;                  // 422: TDxSexPanel
    public TDxPageControl DStatePageControl;         // 420
    public TDxImageForm DStateForm2;                 // 441
    public TDxImageForm DUserState1;                 // 660
    public TDxSexPanel DUserStateForm1;              // 669: TDxSexPanel
    public TDxScrollBox DStateMemo3;                 // 450
    public TDxScrollBox DStateMemo4;                 // 472
    public TDxTabSheet DStateTabSheet5;              // 496
    public TDxImageButton DStateWin;                 // 410
    public TDxImageButton DChangeState;              // 411
    public TDxImageButton DSWTitleActive;            // 413
    public TDxImageButton DSWTitleButton1;
    public TDxImageButton DSWTitleButton2;
    public TDxImageButton DSWTitleButton3;
    public TDxImageButton DSWTitleButton4;
    public TDxImageButton DSWTitlePageUp;
    public TDxImageButton DSWTitlePageDown;
    public TDxImageButton DSWBujuk;                  // 428
    public TDxImageButton DSWBelt;
    public TDxImageButton DSWBoots;
    public TDxImageButton DSWCharm;
    public TDxImageButton DSUSTitleActive;
    public TDxImageButton DSUSTitleButton1;
    public TDxImageButton DSUSTitleButton2;
    public TDxImageButton DSUSTitleButton3;
    public TDxImageButton DSUSTitleButton4;
    public TDxImageButton DSUSTitlePageUp;
    public TDxImageButton DSUSTitlePageDown;
    public TDxImageButton DBujukUS1;                 // 674
    public TDxImageButton DBeltUS1;
    public TDxImageButton DBootsUS1;
    public TDxImageButton DCharmUS1;
    public TDxImageButton DBottomLeftImageButton1;   // 297
    public TDxImageButton DRecallHero;               // 329
    public TDxImageButton DMyHeroState;              // 324
    public TDxImageButton DMyHeroBag;                // 323
    public TDxImageButton DRecallDeputyHero;         // 328
    public TDxImageButton DItemBagUpgrade;           // 358
    public TDxImageButton DOpenShop;                 // 327
    public TDxImageButton DCloseBag;                 // 351
    public TDxImageButton DGold;                     // 352
    public TDxImageButton DItemBagGoldIcon;          // 357
    public TDxImageButton DItemBagGirdIcon;          // 356
    public TDxImageButton DItemBagDiamondIcon;       // 355
    public TDxImageButton DStPageUp;                 // 471
    public TDxImageButton DGuildManageNext;          // 1123
    public TDxImageButton DGuildManagePrevious;      // 1184
    public TDxImageForm DStMagBack1;                 // 473
    public TDxImageForm DStMagBack2;
    public TDxImageForm DStMagBack3;
    public TDxImageForm DStMagBack4;
    public TDxImageForm DStMagBack5;
    public TDxImageForm DStMagBack6;                 // 493
    // ---- 成就/日志/底部功能条（续章版 LoadFromStream 逐项设图号） ----
    public TDxImageForm DServerDlg;
    public TDxImageButton DMyBag;
    public TDxImageButton DMyMagic;
    public TDxImageButton DMyState;
    public TDxImageButton DVoice;
    public TDxImageButton DWeb;
    public TDxImageButton DBotMission;
    public TDxImageButton DActionLog;
    public TDxImageButton DControlHelp;
    public TDxImageButton DBotExit;
    public TDxImageButton DBotFriend;
    public TDxImageButton DBotTrade;
    public TDxImageButton DBotRank;
    public TDxImageButton DBotWhisper;
    public TDxImageButton DBotLogout;
    public TDxImageButton DBotGuild;
    public TDxImageButton DBotGroup;
    public TDxImageButton DBotMiniMap;
    public TDxImageButton DBotFunc1;
    public TDxImageButton DBotFunc2;
    public TDxImageButton DBotFunc3;
    public TDxImageButton DBotFunc4;
    public TDxImageButton DBotFunc5;
    public TDxImageButton DBotFunc6;
    public TDxMemo DChatMemo;
    public TDxImageGrid DItemGrid;                   // 359
    public TDxLabel DStateGameTimeGirdLabel;         // 458
    public TDxLabel DStateLabelAC;                   // 442
    public TDxLabel DStateLabelMAC;
    public TDxLabel DStateLabelDC;
    public TDxLabel DStateLabelMC;
    public TDxLabel DStateLabelSC;
    public TDxLabel DStateLabelHP;
    public TDxLabel DStateLabelMP;
    public TDxLabel DStateExpLabel;                  // 454
    public TDxLabel DStateMaxExpLabel;
    public TDxLabel DStateHitPointLabel;             // 461
    public TDxLabel DStateSpeedPointLabel;           // 460
    public TDxLabel DStateAntiMagicLabel;            // 462
    public TDxLabel DStateAntiPoisonLabel;
    public TDxLabel DStatePoisonRecoverLabel;        // 463
    public TDxLabel DStateHealthRecoverLabel;        // 464
    public TDxLabel DStateSpellRecoverLabel;         // 465
    public TDxLabel DStateGameDiamondLabel;          // 455
    public TDxLabel DStateGameGoldLabel;             // 456
    public TDxLabel DStateGameGirdLabel;             // 457
    public TDxLabel LabelDUserState1CharName;
    public TDxLabel LabelDUserState1RankName;
    public TDxImageButton DJewelryBoxUS1;
    public TDxImageButton DGodBlessUS1;

    // ---------------- TSerialWindows 的数据成员 ----------------
    /// <summary>SerialWindowsDlg.pas:2164-2166（176 版保存的状态标签三态字色）。</summary>
    protected TSaveUIColor FStateWeightLabelColor = new();
    protected TSaveUIColor FStateWearWeightLabelColor = new();
    protected TSaveUIColor FStateHandWeightLabelColor = new();

    /// <summary>SerialWindowsDlg.pas:2604 FMsgDlgButtonSpace。</summary>
    protected int FMsgDlgButtonSpace;
    /// <summary>SerialWindowsDlg.pas:2605 FItemBagShowState。</summary>
    protected byte FItemBagShowState;

    /// <summary>SerialWindowsDlg.pas:2606-2614 包裹游戏币标签模板/可见性（由 LoadFromStream 读流填充）。</summary>
    protected string FlblItemBagGameGold_Text = string.Empty;
    protected byte FlblItemBagGold_Visible;
    protected byte FItemBagGoldIcon_Visible;
    protected string FlblItemBagGird_Text = string.Empty;
    protected byte FlblItemBagGird_Visible;
    protected byte FItemBagGirdIcon_Visible;
    protected string FlblItemBagDiamond_Text = string.Empty;
    protected byte FlblItemBagDiamond_Visible;
    protected byte FItemBagDiamondIcon_Visible;

    /// <summary>SerialWindowsDlg.pas:2823 TSerialWindows.ClientVersion。</summary>
    public TClientVersion ClientVersion;

    /// <summary>SerialWindowsDlg.pas:2623 NewStateWindows:TStateWindows。</summary>
    public TStateWindows NewStateWindows;

    /// <summary>SerialWindowsDlg.pas 技能列表分页起点（protected 字段，见 10311-10318 的 StPageUpClick）。</summary>
    public int MagicIndex;

    public TSerialWindows()
    {
        ClientVersion = TClientVersion.cvSerial;   // SerialWindowsDlg.pas:3283
        NewStateWindows = new TStateWindows();     // SerialWindowsDlg.pas:3284
        CreateControls();
    }

    /// <summary>
    /// 建立控件对象（原文由 SerialWindowsDlg.pas:17346 LoadControlFromStream(Mir.GUI) 从资源流创建，
    /// 本接缝在构造期直接建立同名逻辑控件，几何取 Mir.GUI 的默认值，供派生窗口按版本覆盖）。
    /// 【接缝：待 LoadDxControlEx 与 GUI 资源栈移植后改由资源流创建】
    /// </summary>
    private void CreateControls()
    {
        static TDxControl Rect(TDxControl c, int l, int t, int w, int h)
        {
            c.Left = l; c.Top = t; c.Width = w; c.Height = h;
            return c;
        }

        // ---- 顶层窗口 ----
        DMerchantDlg = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 280, 220);
        DMerchantDlgClose = (TDxImageButton)Rect(new TDxImageButton(), 252, 4, 24, 20);
        DMerchantDlgClose.Visible = false;
        DMerchantDlgHelp = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 20, 20);
        DMerchantDlgHelp_ = DMerchantDlgHelp;
        DItemBag = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 340, 230);
        DMenuDlg = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 120, 200);
        DSellDlg = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 260, 240);
        DHeroStateDlg185 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 250, 220);
        // DHeroStateDlg：与 DHeroStateDlg185 是**互斥**的两个英雄状态窗（205/续章用 185 版，
        // 连击版用新版）。原文 MirNewUI205Dlg.pas:362 与 367 两处都判 `<> nil`，正说明资源里
        // 只会创建其中一个，故此处保持 null，由资源栈按版本创建。
        DHeroStateDlg = null;

        // ---- 登录/注册 ----
        DLogin = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 400, 300);
        DEdId_ = (TDxEdit)Rect(new TDxEdit(), 100, 100, 120, 18);
        DEdPasswd_ = (TDxEdit)Rect(new TDxEdit(), 100, 130, 120, 18);
        DImageButtonAccount = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 60, 18);
        DImageButtonPassWord = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 60, 18);
        DLoginChgPw = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 80, 22);
        DLoginClose = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 20, 20);
        DLoginNew = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 80, 22);
        DLoginOK = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 80, 22);
        DSelectChr = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 700, 500);
        DDeleteHumanDlg = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 280, 200);
        DServerDlg = (TDxServerDlg)Rect(new TDxServerDlg(), 0, 0, 300, 200);

        // ---- 物品包裹 ----
        DGoldValue = (TDxLabel)Rect(new TDxLabel(), 0, 0, 100, 16);
        DlblItemBagGold = (TDxLabel)Rect(new TDxLabel(), 0, 0, 100, 16);
        DlblItemBagGird = (TDxLabel)Rect(new TDxLabel(), 0, 0, 100, 16);
        DlblItemBagDiamond = (TDxLabel)Rect(new TDxLabel(), 0, 0, 100, 16);
        DlblItemBagRefresh = (TDxLabel)Rect(new TDxLabel(), 0, 0, 100, 16);
        DGold = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DCloseBag = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DOpenShop = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DItemBagUpgrade = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DItemBagGoldIcon = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 16, 16);
        DItemBagGirdIcon = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 16, 16);
        DItemBagDiamondIcon = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 16, 16);
        DItemGrid = (TDxImageGrid)Rect(new TDxImageGrid(), 0, 0, 300, 200);

        // ---- 人物状态 ----
        DStateWin = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 40, 40);
        DChangeState = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DUserState1 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 250, 220);
        DStateForm1 = (TDxSexPanel)Rect(new TDxSexPanel(), 0, 0, 110, 160);
        DStateForm2 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 110, 160);
        DUserStateForm1 = (TDxSexPanel)Rect(new TDxSexPanel(), 0, 0, 110, 160);
        DStatePageControl = (TDxPageControl)Rect(new TDxPageControl(), 0, 0, 220, 200);
        DStateMemo3 = (TDxScrollBox)Rect(new TDxScrollBox(), 0, 0, 200, 200);
        DStateMemo4 = (TDxScrollBox)Rect(new TDxScrollBox(), 0, 0, 200, 200);
        DStateTabSheet5 = (TDxTabSheet)Rect(new TDxTabSheet(), 0, 0, 220, 200);
        DStateWeightLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 120, 16);
        DStateWearWeightLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 120, 16);
        DStateHandWeightLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 120, 16);
        DStateLabelAC = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateLabelMAC = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateLabelDC = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateLabelMC = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateLabelSC = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateLabelHP = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateLabelMP = (TDxLabel)Rect(new TDxLabel(), 0, 0, 80, 16);
        DStateExpLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateMaxExpLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateHitPointLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateSpeedPointLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateAntiMagicLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateAntiPoisonLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStatePoisonRecoverLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateHealthRecoverLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateSpellRecoverLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateGameDiamondLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateGameGoldLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateGameGirdLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStateGameTimeGirdLabel = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DStPageUp = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DStMagBack1 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 40, 40);
        DStMagBack2 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 40, 40);
        DStMagBack3 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 40, 40);
        DStMagBack4 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 40, 40);
        DStMagBack5 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 40, 40);
        DStMagBack6 = (TDxImageForm)Rect(new TDxImageForm(), 0, 0, 40, 40);
        LabelDUserState1CharName = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        LabelDUserState1RankName = (TDxLabel)Rect(new TDxLabel(), 0, 0, 160, 16);
        DJewelryBoxUS1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DGodBlessUS1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);

        // ---- 称号/装备格 ----
        DSWTitleActive = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWTitleButton1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWTitleButton2 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWTitleButton3 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWTitleButton4 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWTitlePageUp = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWTitlePageDown = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWBujuk = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWBelt = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWBoots = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSWCharm = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitleActive = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitleButton1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitleButton2 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitleButton3 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitleButton4 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitlePageUp = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DSUSTitlePageDown = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBujukUS1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBeltUS1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBootsUS1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DCharmUS1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);

        // ---- 英雄入口 ----
        DRecallHero = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DMyHeroState = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DMyHeroBag = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DRecallDeputyHero = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBottomLeftImageButton1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DGuildManageNext = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DGuildManagePrevious = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);

        // ---- 底部功能条/聊天 ----
        DMyBag = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DMyMagic = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DMyState = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DVoice = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DWeb = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotMission = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DActionLog = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DControlHelp = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotExit = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFriend = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotTrade = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotRank = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotWhisper = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotLogout = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotGuild = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotGroup = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotMiniMap = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFunc1 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFunc2 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFunc3 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFunc4 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFunc5 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DBotFunc6 = (TDxImageButton)Rect(new TDxImageButton(), 0, 0, 24, 24);
        DChatMemo = (TDxMemo)Rect(new TDxMemo(), 0, 0, 300, 120);
    }

    // ==========================================================================================
    // 原实现摘录（供派生类 inherited 调用；行号指向 SerialWindowsDlg.pas）
    // ==========================================================================================

    /// <summary>
    /// SerialWindowsDlg.pas:17323 TSerialWindows.LoadFromStream。
    /// 原文由 GUI 资源流载入全部控件几何并建立 DEdId := DEdId_ 之类的别名；本接缝建立别名
    /// （DEdId_/DEdPasswd_/DMerchantDlg_ 已由资源树提供，此处只做代数映射），几何由子类按
    /// 各自版本（185/176/续章/205）覆盖。
    /// 【接缝：待 LoadDxControlEx 与 GUI 资源栈移植后接入流式控件加载】
    /// </summary>
    public virtual void LoadFromStream(System.IO.MemoryStream MemoryStream)
    {
        // SerialWindowsDlg.pas:17344-17351：MakeControlAddrList + LoadControlFromStream(Mir) +
        // PatchLoadControlFromStream(msDefaultUI)。
        // SerialWindowsDlg.pas:17357-17379：DEdId/DEdPasswd/DEdNewId/… 与 DMerchantDlg/DItemBag 等别名赋值。
        // SerialWindowsDlg.pas:17383-17397：登录/注册文本框 MaxLength 设定。
        // 以上均依赖未移植的 LoadDxControlEx / DxComponents 全量控件，本车道窗口不触及。
    }

    /// <summary>
    /// SerialWindowsDlg.pas:4968 TSerialWindows.OpenUserState（连击/续章/205 版走 NewStateWindows）。
    /// </summary>
    public override void OpenUserState()
    {
        if ((ClientVersion == TClientVersion.cvSerial || ClientVersion == TClientVersion.cvMirSequel ||
             ClientVersion == TClientVersion.cvMirNewUI205) && MShareGlobals.g_ConfigClient.boUseOldSerialWindows == 0)
        {
            NewStateWindows.OpenUserState();
            return;
        }

        LabelDUserState1CharName.Caption = MShareGlobals.g_UserState1.UserNameStr;
        LabelDUserState1CharName.CaptionColor.Up.Value = MShareGlobals.g_UserState1.NAMECOLOR;

        LabelDUserState1RankName.Caption = MShareGlobals.g_UserState1.GuildNameStr + " " + MShareGlobals.g_UserState1.GuildRankNameStr;

        DJewelryBoxUS1.Visible = MShareGlobals.g_UserState1.JewelryBoxStatus == GXX.Core.Protocol.TJewelryBoxStatus.jbsOpen;
        DGodBlessUS1.Visible = MShareGlobals.g_UserState1.ShowGodBless != 0;

        OpenDUserState1Dlg();
    }

    /// <summary>SerialWindowsDlg.pas:4452 OpenDUserState1Dlg（打开人物状态窗口）。</summary>
    public virtual void OpenDUserState1Dlg()
    {
        DUserState1.Visible = true;
    }

    /// <summary>
    /// SerialWindowsDlg.pas:10325 TSerialWindows.MySelfAbilChange（176 版不走上层守卫，直接执行本段）。
    /// </summary>
    public override void MySelfAbilChange()
    {
        if (MShareGlobals.g_MySelf == null)
            return;

        DStateForm1.IsMale = MShareGlobals.g_MySelf.m_btSex == 0;
        DStateForm1.UseSetting2 = MShareGlobals.g_ConfigClient.boStateWindowsType == 0;

        DStateLabelAC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.AC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.AC2);
        DStateLabelMAC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MAC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MAC2);
        DStateLabelDC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.DC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.DC2);
        DStateLabelMC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MC2);
        DStateLabelSC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.SC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.SC2);
        DStateLabelHP.Caption = DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.HP) + "/" + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MaxHP);
        DStateLabelMP.Caption = DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MP) + "/" + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MaxMP);

        DStateExpLabel.Caption = AddSpace("当前经验") + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.Exp);
        DStateMaxExpLabel.Caption = AddSpace("升级经验") + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MaxExp);

        DStateWeightLabel.Caption = AddSpace("背包重量") + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.Weight) + "/" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MaxWeight);
        if (MShareGlobals.g_MySelf.m_Abil.Weight > MShareGlobals.g_MySelf.m_Abil.MaxWeight)
        {
            DStateWeightLabel.CaptionColor.Up.Value = TColor.clRed;
            DStateWeightLabel.CaptionColor.Hot.Value = TColor.clRed;
            DStateWeightLabel.CaptionColor.Down.Value = TColor.clRed;
        }
        else
        {
            DStateWeightLabel.CaptionColor.Up.Value = FStateWeightLabelColor.Up;
            DStateWeightLabel.CaptionColor.Hot.Value = FStateWeightLabelColor.Hot;
            DStateWeightLabel.CaptionColor.Down.Value = FStateWeightLabelColor.Down;
            DStateWeightLabel.CaptionColor.Disabled.Value = FStateWeightLabelColor.Disabled;
        }

        DStateWearWeightLabel.Caption = AddSpace("穿戴重量") + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.WearWeight) + "/" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MaxWearWeight);
        if (MShareGlobals.g_MySelf.m_Abil.WearWeight > MShareGlobals.g_MySelf.m_Abil.MaxWearWeight)
        {
            DStateWearWeightLabel.CaptionColor.Up.Value = TColor.clRed;
            DStateWearWeightLabel.CaptionColor.Hot.Value = TColor.clRed;
            DStateWearWeightLabel.CaptionColor.Down.Value = TColor.clRed;
        }
        else
        {
            DStateWearWeightLabel.CaptionColor.Up.Value = FStateWearWeightLabelColor.Up;
            DStateWearWeightLabel.CaptionColor.Hot.Value = FStateWearWeightLabelColor.Hot;
            DStateWearWeightLabel.CaptionColor.Down.Value = FStateWearWeightLabelColor.Down;
            DStateWearWeightLabel.CaptionColor.Disabled.Value = FStateWearWeightLabelColor.Disabled;
        }

        DStateHandWeightLabel.Caption = AddSpace("腕力") + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.HandWeight) + "/" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MaxHandWeight);
        if (MShareGlobals.g_MySelf.m_Abil.HandWeight > MShareGlobals.g_MySelf.m_Abil.MaxHandWeight)
        {
            DStateHandWeightLabel.CaptionColor.Up.Value = TColor.clRed;
            DStateHandWeightLabel.CaptionColor.Hot.Value = TColor.clRed;
            DStateHandWeightLabel.CaptionColor.Down.Value = TColor.clRed;
        }
        else
        {
            DStateHandWeightLabel.CaptionColor.Up.Value = FStateHandWeightLabelColor.Up;
            DStateHandWeightLabel.CaptionColor.Hot.Value = FStateHandWeightLabelColor.Hot;
            DStateHandWeightLabel.CaptionColor.Down.Value = FStateHandWeightLabelColor.Down;
            DStateHandWeightLabel.CaptionColor.Disabled.Value = FStateHandWeightLabelColor.Disabled;
        }

        // 中毒恢复 体力恢复 魔法躲避 后面补+ chongchong 2014-04-18
        DStateHitPointLabel.Caption = AddSpace("精确度") + DelphiRTL.IntToStr(MShareGlobals.g_nMyHitPoint);
        DStateSpeedPointLabel.Caption = AddSpace("敏捷度") + DelphiRTL.IntToStr(MShareGlobals.g_nMySpeedPoint);
        DStateAntiMagicLabel.Caption = AddSpace("魔法躲避") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMyAntiMagic * 10) + "%";
        DStateAntiPoisonLabel.Caption = AddSpace("毒物躲避") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMyAntiPoison * 10) + "%";

        DStatePoisonRecoverLabel.Caption = AddSpace("中毒恢复") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMyPoisonRecover * 10) + "%";
        DStateHealthRecoverLabel.Caption = AddSpace("体力恢复") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMyHealthRecover * 10) + "%";
        DStateSpellRecoverLabel.Caption = AddSpace("魔法恢复") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMySpellRecover * 10) + "%";

        // 人物状态中去掉金刚石、元宝、灵符显示 chongchong 2014-04-18
        DStateGameDiamondLabel.Caption = AddSpace(MShareGlobals.g_sGameDiamondName) + DelphiRTL.IntToStr(MShareGlobals.g_nGameDiamond);
        DStateGameGoldLabel.Caption = AddSpace(MShareGlobals.g_sGameGoldName) + DelphiRTL.IntToStr(ActorUiFields.GetGameGold(MShareGlobals.g_MySelf));
        DStateGameGirdLabel.Caption = AddSpace(MShareGlobals.g_sGameGirdName) + DelphiRTL.IntToStr(MShareGlobals.g_nGameGird);
    }

    /// <summary>SerialWindowsDlg.pas:10327-10335 内部函数 AddSpace（不足 12 字符补空格）。</summary>
    protected static string AddSpace(string Str)
    {
        string Result = Str;
        while (true)
        {
            if (Result.Length < 12)
                Result = Result + " ";
            else
                break;
        }
        return Result;
    }

    /// <summary>SerialWindowsDlg.pas:10300 TSerialWindows.StPageUpClick（技能列表翻页）。</summary>
    public virtual void StPageUpClick(object Sender, int X, int Y)
    {
        int CountOnPage = 5;
        if (ReferenceEquals(Sender, DStPageUp))
        {
            if (MagicIndex > 0)
                MagicIndex -= CountOnPage;

            if (MagicIndex < 0) MagicIndex = 0;
        }
        else
        {
            if (MagicIndex + CountOnPage < MShareGlobals.g_MagicList.Count)
            {
                MagicIndex += CountOnPage;
            }
        }
    }

    /// <summary>
    /// SerialWindowsDlg.pas:8084 TSerialWindows.StateMemo4DirectPaint（176 版覆写此方法，
    /// 原实现在 ClientVersion/boStateWindowsType 分支下取图，本接缝保留空实现占位）。
    /// </summary>
    public virtual void StateMemo4DirectPaint(object Sender) { }

    /// <summary>
    /// SerialWindowsDlg.pas:9723 TSerialWindows.ItemBagDirectPaint（176 版覆写此方法）。
    /// 原实现约 130 行且依赖 DxScrollBox/DlblItemBagRefresh 等未移植控件，
    /// 本接缝保留空实现占位。
    /// </summary>
    public virtual void ItemBagDirectPaint(object Sender) { }

    /// <summary>SerialWindowsDlg.pas:3657/3712/3534 CloseDNewAccountDlg/CloseDChgPwDlg/CloseDLoginDlg 的上层实现。</summary>
    public virtual void CloseDNewAccountDlg() { }
    public virtual void CloseDChgPwDlg() { }
    public virtual void CloseDLoginDlg() { }

    /// <summary>SerialWindowsDlg.pas:3973 TSerialWindows.OpenDMenuDlg。</summary>
    public virtual void OpenDMenuDlg(bool IsTrading) { }

    /// <summary>SerialWindowsDlg.pas:4302 TSerialWindows.OpenDSellDlg。</summary>
    public virtual void OpenDSellDlg(int nPage) { }

    /// <summary>
    /// 测试接缝：FlblItemBag*_Text / F*Icon_Visible / FlblItemBag*_Visible 在原文由
    /// SerialWindowsDlg.pas:17346 LoadControlFromStream 从 Mir.GUI 资源流读入；本接缝未接入
    /// 资源栈，故提供显式注入点供测试覆盖 ItemBagDirectPaint 的三条货币分支。
    /// </summary>
    public void SetBagLabelsForTests(string gameGoldText, bool goldVisible,
        string girdText, bool girdVisible, string diamondText, bool diamondVisible)
    {
        FlblItemBagGameGold_Text = gameGoldText;
        FlblItemBagGold_Visible = goldVisible ? (byte)1 : (byte)0;
        FItemBagGoldIcon_Visible = goldVisible ? (byte)1 : (byte)0;
        FlblItemBagGird_Text = girdText;
        FlblItemBagGird_Visible = girdVisible ? (byte)1 : (byte)0;
        FItemBagGirdIcon_Visible = girdVisible ? (byte)1 : (byte)0;
        FlblItemBagDiamond_Text = diamondText;
        FlblItemBagDiamond_Visible = diamondVisible ? (byte)1 : (byte)0;
        FItemBagDiamondIcon_Visible = diamondVisible ? (byte)1 : (byte)0;
    }

    /// <summary>
    /// 测试接缝：单独注入 FlblItemBag*_Visible 与 F*Icon_Visible 两组资源字段
    /// （原文两者来自 GUI 资源的不同项，ItemBagDirectPaint 分别独立读取）。
    /// </summary>
    public void SetBagIconVisibleForTests(bool goldIcon, bool girdIcon, bool diamondIcon)
    {
        FItemBagGoldIcon_Visible = goldIcon ? (byte)1 : (byte)0;
        FItemBagGirdIcon_Visible = girdIcon ? (byte)1 : (byte)0;
        FItemBagDiamondIcon_Visible = diamondIcon ? (byte)1 : (byte)0;
    }

    /// <summary>测试接缝：单独注入 FlblItemBag*_Visible 标签可见性。</summary>
    public void SetBagLabelVisibleForTests(bool gold, bool gird, bool diamond)
    {
        FlblItemBagGold_Visible = gold ? (byte)1 : (byte)0;
        FlblItemBagGird_Visible = gird ? (byte)1 : (byte)0;
        FlblItemBagDiamond_Visible = diamond ? (byte)1 : (byte)0;
    }
}
