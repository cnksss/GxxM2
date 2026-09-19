using System;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Core.Protocol;
using GXX.Core.Util;
using TGList = GXX.Core.Protocol.SDK.TGList;
using static GXX.Client.GUI.Mir.MShareGlobals;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// FState.pas TFrmDlg 的**手写实现**部分（声明面由 TFrmDlg.Decl.g.cs 生成）。
//
// 本文件覆盖原文行号：
//   1417-1604  constructor TFrmDlg.Create            —— 本波次未覆盖（见下方注释与交付报告）
//   1644-1712  destructor  TFrmDlg.Destroy           —— 本波次未覆盖（见下方注释与交付报告）
//   23844-23850 procedure TFrmDlg.RefrshDStorageViewDlgText   ✅ 已覆盖
//   23852-23859 procedure TFrmDlg.UpdateGuildJoinCondition    ✅ 已覆盖
//   23875-23878 procedure TFrmDlg.CloseDHeroGodBlessDlg       ✅ 已覆盖（原文空体）
//   23880-23885 procedure TFrmDlg.CloseDHeroJewelryBoxDlg     ✅ 已覆盖（原文空体）
//   24474-24487 function  TFrmDlg.GetLast/GetPre/GetNextHistroySendSay  ✅ 已覆盖（原文恒返回 ''）
// ============================================================================================

public partial class TFrmDlg
{
    /// <summary>
    /// FState.pas:1417-1604 constructor TFrmDlg.Create。
    /// 原文 188 行，逐行对应；编号见每行注释。
    ///
    /// 与原文的两处**已知等价改写**（均无行为差异，已登记在交付报告）：
    ///   1) `FillChar(X, SizeOf(X), 0)` → `Array.Clear(X, 0, X.Length)`。原文对**托管数组字段**
    ///      调 FillChar，在 .NET 里没有对应语义；等价操作是清零数组内容（new 出的数组本就全 0，
    ///      保留该调用以维持"构造即清零"的显式意图）。
    ///   2) `with Memo do begin Parent := frmMain; ... end` → 逐个属性赋值（TMemo 为接缝类型）。
    /// </summary>
    public TFrmDlg()
    {
        Initialized = false;                                        // 1418
        DBackground = new TDxControlEngine();                       // 1419
        DBackground.Width = GXX.Client.GUI.Mir.ScreenSize.SCREENWIDTH;   // 1420
        DBackground.Height = GXX.Client.GUI.Mir.ScreenSize.SCREENHEIGHT; // 1421
        DBackground.OnGetImage = OnGetImage;                        // 1422
        DBackground.OnBackgroundClick = DBackgroundBackgroundClick; // 1423
        DBackground.OnMouseDown = DBackgroundMouseDown;             // 1424
        DSpecialCmdMenu = new TDxPopupMenu(DBackground);            // 1425
        DSpecialCmdMenu.Name = "DSpecialCmdMenu";                   // 1426
        DxPopupMenuExt.SetGuiType(DSpecialCmdMenu, DxPopupMenuExt.TGuiType.t_PopupMenu); // 1427
        DSpecialCmdMenu.Designing = false;                          // 1428
        DSpecialCmdMenu.Visible = false;                            // 1429
        DxPopupMenuExt.SetAlpha(DSpecialCmdMenu, 150);              // 1430
        // 1431-1435：ItemColor 的 Up/Hot/Down/Disabled/Checked 五态都设 clWhite
        DxPopupMenuExt.GetItemColor(DSpecialCmdMenu).SetAll(TColor.clWhite.Value);
        DxPopupMenuExt.GetBorderColor(DSpecialCmdMenu).Up.Value = TColor.clWhite.Value; // 1436
        DxPopupMenuExt.SetBackgroundColor(DSpecialCmdMenu, GetRGB(190)); // 1437
        DxPopupMenuExt.SetDrawBorder(DSpecialCmdMenu, false);       // 1438
        DSpecialCmdMenu.OnClick = DSpecialCmdMenuClick;             // 1439

        m_nDiceCount = 0;                                           // 1441
        m_boPlayDice = false;                                       // 1442

        SelectMenuStr = "";                                         // 1444
        MenuList = new TGList();                                    // 1445
        menuindex = -1;                                             // 1446
        MenuTopLine = 0;                                            // 1447

        boSayItemDlgMoveOutClose = false;                           // 1449
        nSayItemMakeIndex = 0;                                      // 1450

        TradingItemTopLine = 0;                                     // 1452
        TradingItemSelIndex = -1;                                   // 1453

        BoDetailMenu = false;                                       // 1455
        BoStorageMenu = false;                                      // 1456
        BoNoDisplayMaxDura = false;                                 // 1457
        BoMakeDrugMenu = false;                                     // 1458

        m_boRandomCodeClick = false;                                // 1460

        FMiniMapLoadIndex = -1;                                     // 1462
        FMiniMapLoadSurfaceTime = 0;                                // 1463
        FMiniMapSurface = null;                                     // 1464

        BlinkTime = MyGetTickCount;                                 // 1466
        BlinkCount = 0;                                             // 1467
        MagicIndex = 0;                                             // 1468
        HeroMagicIndex = 0;                                         // 1469
        RankingPage = -1;                                           // 1470
        // 1471：原文 `g_SellDlgItem.S.Name := ''`。
        // 托管侧 TStdItem.Name 是 `fixed byte Name[61]`（Delphi string[60] 的字节等价），
        // 清空短串即把长度字节置 0，故固定后写 pName[0] = 0。
        unsafe
        {
            fixed (byte* pName = FStateMShareSeam.g_SellDlgItem.s.Name)
            {
                pName[0] = 0;
            }
        }
        Guild = "";                                                 // 1472
        GuildFlag = "";                                             // 1473
        GuildCommanderMode = 0;                                     // 1474
        GuildStrs = new TStringList();                              // 1475
        GuildStrs2 = new TStringList();                             // 1476
        GuildNotice = new TStringList();                            // 1477
        GuildWJ = new TStringList();                                // 1478
        GuildMembers = new TStringList();                           // 1479
        GuildChats = new TStringList();                             // 1480
        GuildGroupList = new TGuildGroupList();                     // 1481

        ShowGuildList = new TShowGuildList();                       // 1483
        GuildJoinUserList = new TGuildJoinUserList();               // 1484
        ShopTabPage = -1;                                           // 1485
        RankingSelectLine = -1;                                     // 1486
        BoxDlgWideScreen = false;                                   // 1487
        SelDeleteCharName = "";                                     // 1488
        FAbilHPTick = MyGetTickCount;                               // 1489
        FAbilMPTick = MyGetTickCount;                               // 1490
        FAbilHPIndex = 0;                                           // 1491
        FAbilMPIndex = 0;                                           // 1492
        GameGoldDealMenuIndex = 0;                                  // 1493

        dwControlHelpCickTick = MyGetTickCount;                     // 1495

        DEdId = null;                                               // 1497
        DEdPasswd = null;                                           // 1498
        DEdNewId = null;                                            // 1499
        DEdConfirm = null;                                          // 1500
        DEdChgId = null;                                            // 1501
        DEdChrName = null;                                          // 1502
        DscStart = null;                                            // 1503
        DEdChat = null;                                             // 1504
        DMinMapDlg = null;                                          // 1505
        DGuildDlg = null;                                           // 1506
        DChatMemo = null;                                           // 1507
        DWhisperMemo = null;                                        // 1508
        DCheckBoxAutoAnswersWhisper = null;                         // 1509
        DEditAutoAnswersWhisper = null;                             // 1510
        DBotPlusAbil = null;                                        // 1511
        DGrpAllowGroup = null;                                      // 1512
        DMemoGroupMembers = null;                                   // 1513
        DLabelGroupMembersOwner = null;                             // 1514
        DMerchantDlg = null;                                        // 1515
        DMerchantDlgClose = null;                                   // 1516
        DMerchantDlgHelp = null;                                    // 1517
        DMissionMemo = null;                                        // 1518

        LabelDSelectChrServerName = null;                           // 1520
        LabelSelectChrDlgCharName1 = null;                          // 1521
        LabelSelectChrDlgJob1 = null;                               // 1522
        LabelSelectChrDlgLevel1 = null;                             // 1523
        LabelSelectChrDlgCharName2 = null;                          // 1524
        LabelSelectChrDlgJob2 = null;                               // 1525
        LabelSelectChrDlgLevel2 = null;                             // 1526
        LabelSelectChrDlgCharName3 = null;                          // 1527
        LabelSelectChrDlgJob3 = null;                               // 1528
        LabelSelectChrDlgLevel3 = null;                             // 1529

        DMainMenu = null;                                           // 1531

        Memo = new TMemo(FStateClMainSeam.Owner);                   // 1533
        // 1534-1544 with Memo do begin ... end
        Memo.Parent = FStateClMainSeam.FrmMainPlaceholder;           // 1535（原文 Parent := frmMain）
        Memo.Color = TColor.clBlack.Value;                          // 1536
        Memo.Font.Color = TColor.clWhite.Value;                     // 1537
        Memo.Font.Size = 10;                                        // 1538
        Memo.Ctl3D = false;                                         // 1539
        Memo.BorderStyle = 1;                                       // 1540（原文 bsSingle = 1）
        Memo.Visible = false;                                       // 1541
        GuildMemoVisible = false;                                   // 1545

        ShowMiniBigMapXY = false;                                   // 1547

        FIsRingLeft = false;                                        // 1549
        FIsArmRingLeft = false;                                     // 1550

        FIsFashionRingLeft = false;                                 // 1552
        FIsFashionArmRingLeft = false;                              // 1553

        FSayItemHintWin = DrawScrn.CreateHintWindows();             // 1555

        FGameShopPageControlIndex = 0;                              // 1557
        FGameShopPageIndex = 0;                                     // 1558

        FGroupChatBottomSpace = 0;                                  // 1560
        FChatMemoBottomSpace = 0;                                   // 1561
        FEditChatBottomSpace = 0;                                   // 1562
        FChangeChatHeightBottomSpace = 0;                           // 1563

        FDayBrightIconStartIndex = -1;                              // 1565
        FMagicBallStartIndex = -1;                                  // 1566

        FHeroState185IconStartIndex = -1;                           // 1568
        FHeroStateIconStartIndex = -1;                              // 1569

        FHeroBagItem40ImageIndex = -1;                              // 1571
        FBotPlusAbilFlashStartIndex = -1;                           // 1572

        FJewelryBoxUpImageIndex = -1;                               // 1574
        FJewelryBoxDownImageIndex = -1;                             // 1575

        FHeroJewelryBoxUpImageIndex = -1;                           // 1577
        FHeroJewelryBoxDownImageIndex = -1;                         // 1578

        FNpcStoragePage = 0;                                        // 1580

        FAuctionAllItemsSortField = 0;                              // 1582
        FAuctionAllItemsSortASC = true;                             // 1583

        FAuctionMyItemsPage = 1;                                    // 1585
        FAuctionAllItemsPage = 1;                                   // 1586
        FAuctionMyAuctioningItemsPage = 1;                          // 1587
        FAuctionMyItemsBagPage = 1;                                 // 1588
        FAuctionMyItemsBagSelectIndex = -1;                         // 1589

        FAuctionMyItemsPageCount = 0;                               // 1591
        FAuctionAllItemsPageCount = 0;                              // 1592
        FAuctionMyAuctioningItemsPageCount = 0;                     // 1593
        // 1594：(DEF_MAX_BAG_ITEM + AUCTION_BAG_ONE_PAGE_COUNT - 1) div AUCTION_BAG_ONE_PAGE_COUNT
        FAuctionMyItemsBagPageCount = (Grobal2Const.DEF_MAX_BAG_ITEM + FStateSeamConst.AUCTION_BAG_ONE_PAGE_COUNT - 1)
                                      / FStateSeamConst.AUCTION_BAG_ONE_PAGE_COUNT;

        FScreenMagicBtnList = new TList();                          // 1596

        FGuardianLevelStatueMonRecogId = 0;                         // 1598
        Array.Clear(FGuardianLevelRewardItems, 0, FGuardianLevelRewardItems.Length);   // 1599
        FGuardianLevelItemCounts = default; // 1600（原文 FillChar(FGuardianLevelItemCounts, SizeOf, 0)）

        FCurrentBagPage = 0;                                        // 1602
        FExtBagPageCount = 0;                                       // 1603
        FStateMShareSeam.g_ExtBagOpenItemCount = 0;                 // 1604
    }


    /// <summary>
    /// 【本波次未覆盖】FState.pas:1644-1712 destructor TFrmDlg.Destroy。
    /// 原文释放 DBackground 控件树、各 TStringList、FScreenMagicBtnList、ShowGuildList 等；
    /// 托管侧无手工释放语义，但**原文会逐个子控件 Free 并解绑事件**，须待控件树移植后
    /// 按 1:1 结构改写为显式的解绑/清理（不能简单留空，否则事件订阅泄漏）。
    /// </summary>
    public virtual void Destroy()
        => throw new NotSupportedException("TFrmDlg.Destroy: not ported yet (FState.pas:1644-1712)");

    /// <summary>
    /// FState.pas:23844 procedure TFrmDlg.RefrshDStorageViewDlgText(
    ///   IsStorageViewDlgIsExt:Boolean; ACount, AMaxCount, APage, AMaxPage:Integer)。
    /// 原文只做四个字段赋值，**忽略 IsStorageViewDlgIsExt 参数**（原文如此）。
    /// </summary>
    public virtual void RefrshDStorageViewDlgText(bool IsStorageViewDlgIsExt, int ACount, int AMaxCount, int APage, int AMaxPage)
    {
        FDStorageViewDlgCount = ACount;
        FDStorageViewDlgMaxCount = AMaxCount;
        FDStorageViewDlgPage = APage;
        FDStorageViewDlgMaxPage = AMaxPage;
    }

    /// <summary>
    /// FState.pas:23852 procedure TFrmDlg.UpdateGuildJoinCondition(AJob:Integer; ALevel:Integer; AMsg:string)。
    /// 原文把入会条件缓存到三个 protected 字段。
    /// </summary>
    public virtual void UpdateGuildJoinCondition(int AJob, int ALevel, string AMsg)
    {
        FGuildJoinJob = AJob;
        FGuildJoinLevel = ALevel;
        FGuildJoinMsg = AMsg;
    }

    /// <summary>FState.pas:23875 procedure TFrmDlg.CloseDHeroGodBlessDlg（原文**空体**，逐字保留）。</summary>
    public virtual void CloseDHeroGodBlessDlg()
    {
    }

    /// <summary>FState.pas:23880 procedure TFrmDlg.CloseDHeroJewelryBoxDlg（原文**空体**，逐字保留）。</summary>
    public virtual void CloseDHeroJewelryBoxDlg()
    {
    }

    /// <summary>
    /// FState.pas:24474 function TFrmDlg.GetLastHistroySendSay:string。
    /// 原文**恒返回空串**（历史发言功能未实现，逐字保留）。
    /// </summary>
    public virtual string GetLastHistroySendSay()
    {
        return "";
    }

    /// <summary>
    /// FState.pas:24479 function TFrmDlg.GetNextHistroySendSay:string。
    /// 原文**恒返回空串**。
    /// </summary>
    public virtual string GetNextHistroySendSay()
    {
        return "";
    }

    /// <summary>
    /// FState.pas:24484 function TFrmDlg.GetPreHistroySendSay:string。
    /// 原文**恒返回空串**。
    /// </summary>
    public virtual string GetPreHistroySendSay()
    {
        return "";
    }
}

/// <summary>
/// FState.pas 的单元级 var（原文 1116-1117 行）：
///   var FrmDlg:TFrmDlg = nil;
/// 按工程惯例收敛为静态类。
///
/// 说明：FState.pas 是"公共头"单元，但**它本身只有一个单元级全局变量**（FrmDlg）。
/// 任务书里提到的 g_MySelf / g_MyHero / g_UserState1 / g_MagicList / g_AcupointLevels /
/// g_DefColorTable 均**不在**本单元 —— 它们定义在 MShare.pas（详见交付报告的"既往事实更正"）。
/// </summary>
public static class FStateGlobal
{
    /// <summary>FState.pas:1117 var FrmDlg:TFrmDlg = nil。</summary>
    public static TFrmDlg FrmDlg;

    /// <summary>FState.pas:1133 const ConditionOKHitColor = clWhite（implementation 段单元级常量）。</summary>
    public static readonly TColor ConditionOKHitColor = TColor.clWhite;

    /// <summary>测试用：复位单元级状态。</summary>
    public static void ResetForTests()
    {
        FrmDlg = null;
    }
}
