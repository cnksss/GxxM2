using System.IO;
using GXX.Client.GUI.Mir;
using GXX.Client.DxComponent;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次P1：MirSequelDlg.pas TSequelWindows（传奇续章，全文 273 行）1:1 测试。
/// 有效覆写仅构造/析构/LoadFromStream（49-61 的一批覆写在原文注释块内）。
/// </summary>
public sealed class GuiMirSequelWindowsTests
{
    private static TSequelWindows Loaded()
    {
        var w = new TSequelWindows();
        using var ms = new MemoryStream();
        w.LoadFromStream(ms);
        return w;
    }

    [Fact]
    public void SequelWindows_Ctor_ClientVersionIsMirSequel()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSequelWindows();
            Assert.Equal(TClientVersion.cvMirSequel, w.ClientVersion);   // MirSequelDlg.pas:73
            w.Destroy();                                                  // 76-79
        });
    }

    [Fact]
    public void SequelWindows_LoadFromStream_LoginAndServerDlg()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();

            Assert.Equal(TImageType.UI3_wil, w.DLogin.ImageIndex.ImageType);       // 87
            Assert.Equal(1, w.DLogin.ImageIndex.Up);                              // 88
            Assert.Equal(TImageType.UI3_wil, w.DServerDlg.ImageIndex.ImageType);   // 120
            Assert.Equal(0, w.DServerDlg.ImageIndex.Up);                          // 121
            Assert.Equal(TImageType.UI3_wil, w.DHeroStateDlg185.ImageIndex.ImageType); // 128
            Assert.Equal(10, w.DHeroStateDlg185.ImageIndex.Up);                   // 129
        });
    }

    [Fact]
    public void SequelWindows_LoadFromStream_SelectChrDependsOnScreenWidth()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            // 123：if SCREENWIDTH <> 1024 then 设 DSelectChr 图号
            ScreenSize.SCREENWIDTH = 800;
            var w1 = Loaded();
            Assert.Equal(TImageType.UI3_wil, w1.DSelectChr.ImageIndex.ImageType);
            Assert.Equal(2, w1.DSelectChr.ImageIndex.Up);

            GuiReset.All();
            ScreenSize.SCREENWIDTH = 1024;
            var w2 = Loaded();
            Assert.Equal(TImageType.Prguse_wil, w2.DSelectChr.ImageIndex.ImageType); // 未被覆盖（构造默认）
            Assert.Equal(-1, w2.DSelectChr.ImageIndex.Up);
        });
    }

    [Fact]
    public void SequelWindows_LoadFromStream_BottomButtons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();

            // 每项三态（Up/Hot/Down）按原文 2 递增：131-252
            AssertImage(w.DMyBag, 30, 31, 32);
            AssertImage(w.DMyMagic, 40, 41, 42);
            AssertImage(w.DMyState, 50, 51, 52);
            AssertImage(w.DVoice, 60, 61, 62);
            AssertImage(w.DWeb, 80, 81, 82);
            AssertImage(w.DBotMission, 90, 91, 92);
            AssertImage(w.DActionLog, 521, 522, 523);
            AssertImage(w.DControlHelp, 100, 101, 102);
            AssertImage(w.DBotExit, 110, 111, 112);
            AssertImage(w.DBotFriend, 120, 121, 122);
            AssertImage(w.DBotTrade, 130, 131, 132);
            AssertImage(w.DBotRank, 140, 141, 142);
            AssertImage(w.DBotWhisper, 150, 151, 152);
            AssertImage(w.DBotLogout, 160, 161, 162);
            AssertImage(w.DBotGuild, 170, 171, 172);
            AssertImage(w.DBotGroup, 190, 191, 192);
            AssertImage(w.DBotMiniMap, 200, 201, 202);
            AssertImage(w.DBotFunc1, 220, 221, 222);
            AssertImage(w.DBotFunc2, 230, 231, 232);
            AssertImage(w.DBotFunc3, 240, 241, 242);
            AssertImage(w.DBotFunc4, 250, 251, 252);
            AssertImage(w.DBotFunc5, 260, 261, 262);
            AssertImage(w.DBotFunc6, 270, 271, 272);
        });
    }

    private static void AssertImage(TDxImageButton b, int up, int hot, int down)
    {
        Assert.Equal(TImageType.UI3_wil, b.ImageIndex.ImageType);
        Assert.Equal(up, b.ImageIndex.Up);
        Assert.Equal(hot, b.ImageIndex.Hot);
        Assert.Equal(down, b.ImageIndex.Down);
    }

    [Fact]
    public void SequelWindows_LoadFromStream_BagAndMagicOffsets()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            // 135/136、142/143、149/150、156/157 是在「原几何」基础上减 14/12/10 的相对调整。
            // 接缝默认几何以 (100,100) 为基线显式设定，验证相对位移语义。
            var w = new TSequelWindows();
            w.DMyBag.Left = 100; w.DMyBag.Top = 100;
            w.DMyMagic.Left = 100; w.DMyMagic.Top = 100;
            w.DMyState.Left = 100; w.DMyState.Top = 100;
            w.DVoice.Left = 100; w.DVoice.Top = 100;
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            Assert.Equal(86, w.DMyBag.Left);      // 100 - 14
            Assert.Equal(90, w.DMyBag.Top);       // 100 - 10
            Assert.Equal(86, w.DMyMagic.Left);    // 100 - 14
            Assert.Equal(88, w.DMyMagic.Top);     // 100 - 12
            Assert.Equal(86, w.DMyState.Left);
            Assert.Equal(88, w.DMyState.Top);
            Assert.Equal(86, w.DVoice.Left);
            Assert.Equal(88, w.DVoice.Top);
        });
    }

    [Fact]
    public void SequelWindows_LoadFromStream_ChatMemoImageIndices()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();

            Assert.Equal(TImageType.UI3_wil, w.DChatMemo.PrevImageIndex.ImageType);   // 254
            Assert.Equal(501, w.DChatMemo.PrevImageIndex.Up);                          // 255
            Assert.Equal(502, w.DChatMemo.PrevImageIndex.Hot);                         // 256
            Assert.Equal(503, w.DChatMemo.PrevImageIndex.Down);                        // 257

            Assert.Equal(TImageType.UI3_wil, w.DChatMemo.NextImageIndex.ImageType);   // 259
            Assert.Equal(504, w.DChatMemo.NextImageIndex.Up);                          // 260
            Assert.Equal(505, w.DChatMemo.NextImageIndex.Hot);                         // 261
            Assert.Equal(506, w.DChatMemo.NextImageIndex.Down);                        // 262

            Assert.Equal(TImageType.UI3_wil, w.DChatMemo.BarImageIndex.ImageType);    // 264
            Assert.Equal(507, w.DChatMemo.BarImageIndex.Up);                           // 265
            Assert.Equal(508, w.DChatMemo.BarImageIndex.Hot);                          // 266
            Assert.Equal(509, w.DChatMemo.BarImageIndex.Down);                         // 267

            // 270：ScrollImageIndex 只设 ImageType 与 Up（原文未设 Hot/Down）
            Assert.Equal(TImageType.UI3_wil, w.DChatMemo.ScrollImageIndex.ImageType); // 269
            Assert.Equal(500, w.DChatMemo.ScrollImageIndex.Up);                        // 270
            Assert.Equal(-1, w.DChatMemo.ScrollImageIndex.Hot);
            Assert.Equal(-1, w.DChatMemo.ScrollImageIndex.Down);
        });
    }
}

/// <summary>
/// 并行批次P1：MirNewUI205Dlg.pas TNewUI205Windows（全文 763 行）1:1 测试。
/// 有效覆写：LoadFromStream(90)、ShowMDlg(325)、ItemBagDirectPaint(377)、
/// CloseDChgPwDlg(445)、CloseDNewAccountDlg(457)、CloseDLoginDlg(469)、
/// OpenDMenuDlg(476)、OpenDSellDlg(483)。
/// </summary>
public sealed class GuiMirNewUI205Tests
{
    private static TNewUI205Windows Make()
    {
        var w = new TNewUI205Windows();
        ScreenSize.SCREENWIDTH = 1024;
        w.DItemBag.Left = 0; w.DItemBag.Top = 0;
        w.DItemBag.Width = 340; w.DItemBag.Height = 230;
        w.DMerchantDlg.Left = 0; w.DMerchantDlg.Top = 0;
        w.DMerchantDlg.Width = 280;
        return w;
    }

    [Fact]
    public void NewUI205_Ctor_ClientVersionIs205()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TNewUI205Windows();
            Assert.Equal(TClientVersion.cvMirNewUI205, w.ClientVersion);   // MirNewUI205Dlg.pas:82
            w.Destroy();                                                    // 85-88
        });
    }

    [Fact]
    public void NewUI205_LoadFromStream_SnapshotsMerchantImageAndBringsGuildButtons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.DMerchantDlg.ImageIndex.ImageType = TImageType.UIN_wil;
            w.DMerchantDlg.ImageIndex.Up = 77;
            w.DMerchantDlg.ImageIndex.Down = 78;
            w.DMerchantDlgClose.Left = 5; w.DMerchantDlgClose.Top = 6;
            w.DMerchantDlgClose.Width = 30; w.DMerchantDlgClose.Height = 31;

            using var ms = new MemoryStream();
            w.LoadFromStream(ms);                                       // 90

            Assert.Equal(TImageType.UIN_wil, MShareGlobals.g_MerchantImageIndex.Image); // 94
            Assert.Equal(77, MShareGlobals.g_MerchantImageIndex.Up);
            Assert.Equal(78, MShareGlobals.g_MerchantImageIndex.Down);
            Assert.Equal(-1, MShareGlobals.g_MerchantImageIndex.Hot);   // 未被赋值（源亦为 -1）
            Assert.Equal(5, MShareGlobals.g_MerchantCloseButtonRect.Left);   // 95
            Assert.Equal(6, MShareGlobals.g_MerchantCloseButtonRect.Top);
            Assert.Equal(35, MShareGlobals.g_MerchantCloseButtonRect.Right);
            Assert.Equal(37, MShareGlobals.g_MerchantCloseButtonRect.Bottom);
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_DefaultDialogOffsets()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            MShareGlobals.g_ConfigClient.nNPCMsgDlgTextOffsetX = 5;
            MShareGlobals.g_ConfigClient.nNPCMsgDlgTextOffsetY = 7;

            w.ShowMDlg(3, "商人", "文本", true, false);                  // 325

            Assert.Equal(3, w.MerchantFace);                            // 330
            Assert.Equal("商人", w.MerchantName);                        // 331
            Assert.Equal("文本", w.MDlgStr);                             // 332
            Assert.Equal(1, w.AddNpcMemoCount);                         // 338 默认对话框（38+5, 43+7）
            Assert.Equal("文本", w.LastAddNpcMemoText);
            Assert.Same(w.DMerchantDlg, w.LastAddNpcMemoOwner);
            Assert.True(w.DMerchantDlg.Visible);                        // 352
            Assert.True(w.RequireAddPoints);                            // 373
            Assert.True(w.LastestClickTime != 0);                       // 374
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_CustomBackgroundOffsets()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            MShareGlobals.g_ConfigClient.nNPCMsgDlgTextOffsetX = 0;
            MShareGlobals.g_ConfigClient.nNPCMsgDlgTextOffsetY = 0;

            w.ShowMDlg(1, "n", "s", false, false);                      // 340 自定义背景对话框（20, 16）
            Assert.Equal(1, w.AddNpcMemoCount);
            // 305 注释保证：boSetBagItemPos=False 时不修改背包位置
            Assert.Equal(0, w.DItemBag.Left);
            Assert.Equal(0, w.DItemBag.Top);
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_ItemBagPositionUsesScreenWidth()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.DItemBag.Width = 340;
            ScreenSize.SCREENWIDTH = 1024;
            w.ShowMDlg(1, "n", "s", true, false);                       // 355-358

            Assert.Equal(1024 - 340 - 10, w.DItemBag.Left);
            Assert.Equal(90, w.DItemBag.Top);
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_FloatingFollowsConfigOrMainForm()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();

            w.ShowMDlg(0, "n", "s", true, false);                       // 347-350
            Assert.False(w.DMerchantDlg.Floating);

            MShareGlobals.g_ConfigClient.boNPCGuiCanMove = 1;
            w.ShowMDlg(0, "n", "s", true, false);
            Assert.True(w.DMerchantDlg.Floating);

            MShareGlobals.g_ConfigClient.boNPCGuiCanMove = 0;
            frmMain.boNpcDlgCanMove = true;
            w.ShowMDlg(0, "n", "s", true, false);
            Assert.True(w.DMerchantDlg.Floating);

            frmMain.boNpcDlgCanMove = false;
            w.ShowMDlg(0, "n", "s", true, false);
            Assert.False(w.DMerchantDlg.Floating);
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_HeroStateDlgMovesBesideMerchant_NewPath()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.DMerchantDlg.Width = 280;
            MShareGlobals.g_ConfigClient.boUseOldSerialWindows = 0;      // 366 else 分支
            w.DHeroStateDlg.Visible = true;
            w.DHeroStateDlg.Left = 999;
            w.DHeroStateDlg185.Visible = true;
            w.DHeroStateDlg185.Left = 888;

            w.ShowMDlg(0, "n", "s", true, false);
            Assert.Equal(280, w.DHeroStateDlg.Left);                     // 369
            Assert.Equal(888, w.DHeroStateDlg185.Left);                  // 旧窗口未被动到

            // boHeroStateDlgNoMove=1 → 不移动（363 / 368）
            w.DHeroStateDlg.Left = 999;
            MShareGlobals.g_ConfigClient.boHeroStateDlgNoMove = 1;
            w.ShowMDlg(0, "n", "s", true, false);
            Assert.Equal(999, w.DHeroStateDlg.Left);
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_HeroStateDlg185Moves_OldPath()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.DMerchantDlg.Width = 280;
            MShareGlobals.g_ConfigClient.boUseOldSerialWindows = 1;      // 361
            w.DHeroStateDlg185.Visible = true;
            w.DHeroStateDlg185.Left = 777;
            w.DHeroStateDlg.Visible = true;
            w.DHeroStateDlg.Left = 999;

            w.ShowMDlg(0, "n", "s", true, false);
            Assert.Equal(280, w.DHeroStateDlg185.Left);                  // 364
            Assert.Equal(999, w.DHeroStateDlg.Left);                     // 新窗口未被动到
        });
    }

    [Fact]
    public void NewUI205_ShowMDlg_CloseButtonBringToFrontOnlyWhenVisible()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.DMerchantDlgClose.Visible = false;
            w.ShowMDlg(0, "n", "s", true, false);                       // 343 不 BringToFront
            w.DMerchantDlgClose.Visible = true;
            w.ShowMDlg(0, "n", "s", true, false);                       // 344 BringToFront
            Assert.True(w.DMerchantDlgClose.Visible);
        });
    }

    [Fact]
    public void NewUI205_ItemBagDirectPaint_NullSelfExits()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            MShareGlobals.g_MySelf = null;
            w.DGoldValue.Caption = "哨兵";
            w.ItemBagDirectPaint(BagButtonShim(w));                           // 383
            Assert.Equal("哨兵", w.DGoldValue.Caption);
        });
    }

    [Fact]
    public void NewUI205_ItemBagDirectPaint_GameInfoOffHidesAllIcons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            var self = new THumActor();
            GXX.Client.GUI.Mir.ActorUiFields.SetGold(self, 500);
            MShareGlobals.g_MySelf = self;
            MShareGlobals.GetGoldStrHandler = g => "G" + g;
            MShareGlobals.g_ConfigClient.boShowBagGameInfo = 0;         // 389

            w.DItemBagGoldIcon.Visible = true;
            w.DlblItemBagGold.Visible = true;
            w.DItemBagGirdIcon.Visible = true;
            w.DlblItemBagGird.Visible = true;
            w.DItemBagDiamondIcon.Visible = true;
            w.DlblItemBagDiamond.Visible = true;

            w.ItemBagDirectPaint(BagButtonShim(w));

            Assert.Equal("G500", w.DGoldValue.Caption);                 // 387
            Assert.False(w.DItemBagGoldIcon.Visible);                   // 390
            Assert.False(w.DlblItemBagGold.Visible);                    // 391
            Assert.False(w.DItemBagGirdIcon.Visible);                   // 393
            Assert.False(w.DlblItemBagGird.Visible);                    // 394
            Assert.False(w.DItemBagDiamondIcon.Visible);                // 396
            Assert.False(w.DlblItemBagDiamond.Visible);                 // 397
        });
    }

    [Fact]
    public void NewUI205_ItemBagDirectPaint_ThreeCurrenciesWithSeparator()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            var self = new THumActor();
            GXX.Client.GUI.Mir.ActorUiFields.SetGold(self, 1);
            GXX.Client.GUI.Mir.ActorUiFields.SetGameGold(self, 2000);
            MShareGlobals.g_MySelf = self;
            MShareGlobals.GetGoldStrHandler = g => "S" + g;
            MShareGlobals.g_ConfigClient.boShowBagGameInfo = 1;
            MShareGlobals.g_ConfigClient.boShowBagGameGoldSeparator = 1;  // 走 GetGoldStr 分支
            MShareGlobals.g_nGameGird = 3000;
            MShareGlobals.g_nGameDiamond = 4000;
            MShareGlobals.g_sGameGoldName = "元宝";
            MShareGlobals.g_sGameGirdName = "灵符";
            MShareGlobals.g_sGameDiamondName = "金刚石";

            // Flbl*_Text / FItemBag*Icon_Visible 为 LoadFromStream 从 GUI 资源读入的保护字段，
            // 测试直接注入（接缝语义：模板与可见性来自资源流）。
            w.SetBagLabelsForTests("<$name>：<$value>", true,
                                   "<$name>：<$value>", true,
                                   "<$name>：<$value>", true);

            w.ItemBagDirectPaint(BagButtonShim(w));

            Assert.Equal("元宝：S2000", w.DlblItemBagGold.Caption);      // 409/410/412
            Assert.Equal("灵符：S3000", w.DlblItemBagGird.Caption);      // 423/424/426
            Assert.Equal("金刚石：S4000", w.DlblItemBagDiamond.Caption); // 437/438/440
            Assert.True(w.DItemBagGoldIcon.Visible);
            Assert.True(w.DlblItemBagGold.Visible);
            Assert.True(w.DItemBagGirdIcon.Visible);
            Assert.True(w.DlblItemBagGird.Visible);
            Assert.True(w.DItemBagDiamondIcon.Visible);
            Assert.True(w.DlblItemBagDiamond.Visible);
        });
    }

    [Fact]
    public void NewUI205_ItemBagDirectPaint_NoSeparatorUsesIntToStr()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            var self = new THumActor();
            GXX.Client.GUI.Mir.ActorUiFields.SetGameGold(self, 2000);
            MShareGlobals.g_MySelf = self;
            MShareGlobals.GetGoldStrHandler = g => "S" + g;
            MShareGlobals.g_ConfigClient.boShowBagGameInfo = 1;
            MShareGlobals.g_ConfigClient.boShowBagGameGoldSeparator = 0;  // IntToStr 分支
            MShareGlobals.g_nGameGird = 3000;
            MShareGlobals.g_nGameDiamond = 4000;
            MShareGlobals.g_sGameGoldName = "元宝";
            MShareGlobals.g_sGameGirdName = "灵符";
            MShareGlobals.g_sGameDiamondName = "金刚石";
            w.SetBagLabelsForTests("<$name>=<$value>", true,
                                   "<$name>=<$value>", true,
                                   "<$name>=<$value>", true);

            w.ItemBagDirectPaint(BagButtonShim(w));

            Assert.Equal("元宝=2000", w.DlblItemBagGold.Caption);        // 407
            Assert.Equal("灵符=3000", w.DlblItemBagGird.Caption);        // 421
            Assert.Equal("金刚石=4000", w.DlblItemBagDiamond.Caption);   // 435
        });
    }

    [Fact]
    public void NewUI205_ItemBagDirectPaint_LabelFlagsGateCaptions()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            var self = new THumActor();
            MShareGlobals.g_MySelf = self;
            MShareGlobals.GetGoldStrHandler = g => g.ToString();
            MShareGlobals.g_ConfigClient.boShowBagGameInfo = 1;
            // 三种货币的「图标可见性」与「标签可见性」在原文是两组独立资源字段：
            // 此处图标全开、标签仅金币开，用于验证两组字段互不牵连。
            w.SetBagLabelsForTests("<$name>", true, "<$name>", false, "<$name>", false);
            w.SetBagIconVisibleForTests(true, true, true);

            w.DItemBagGoldIcon.Visible = false;
            w.DItemBagGirdIcon.Visible = true;
            w.DItemBagDiamondIcon.Visible = true;
            w.DlblItemBagGold.Caption = "";
            w.DlblItemBagGird.Caption = "哨兵";
            w.DlblItemBagDiamond.Caption = "哨兵";

            w.ItemBagDirectPaint(BagButtonShim(w));

            // FlblItemBagGold_Visible=1 → 写 Caption（403-412）
            Assert.Equal("元宝", w.DlblItemBagGold.Caption);
            // FlblItemBagGird/Diamond_Visible=0 → 保留调用前哨兵值，不写（417/431）
            Assert.Equal("哨兵", w.DlblItemBagGird.Caption);
            Assert.Equal("哨兵", w.DlblItemBagDiamond.Caption);
            // Icon 可见性直接取 F*Icon_Visible（401/415/429），与 label 可见性同一来源
            Assert.True(w.DItemBagGoldIcon.Visible);
            Assert.True(w.DItemBagGirdIcon.Visible);
            Assert.True(w.DItemBagDiamondIcon.Visible);
            Assert.True(w.DlblItemBagGold.Visible);
            Assert.False(w.DlblItemBagGird.Visible);
            Assert.False(w.DlblItemBagDiamond.Visible);
        });
    }

    /// <summary>
    /// 原文缺陷（MirNewUI205Dlg.pas:384）：Sender 被硬转成 TDxImageButton，而 DItemBag 类声明为
    /// TDxImageForm（SerialWindowsDlg.pas:340）。移植保持形参与强转不变，测试用按钮替身驱动分支。
    /// </summary>
    private static TDxImageButton BagButtonShim(TNewUI205Windows w)
    {
        var b = new TDxImageButton();
        b.Left = w.DItemBag.Left; b.Top = w.DItemBag.Top;
        b.Width = w.DItemBag.Width; b.Height = w.DItemBag.Height;
        return b;
    }

    [Fact]
    public void NewUI205_CloseDLoginDlg_HidesBothButtons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.DLoginNew.Visible = true;
            w.DLoginChgPw.Visible = true;
            w.CloseDLoginDlg();                                         // 469-474
            Assert.False(w.DLoginNew.Visible);
            Assert.False(w.DLoginChgPw.Visible);
        });
    }

    [Fact]
    public void NewUI205_CloseDNewAccountDlg_NormalLoginModeRestoresButtons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            MShareGlobals.g_nClientLoginMode = MShareGlobals.CLIENT_MODE_NORMAL;
            w.DLoginNew.Visible = false;
            w.DLoginChgPw.Visible = false;
            w.CloseDNewAccountDlg();                                    // 457-467
            Assert.True(w.DLoginNew.Visible);
            Assert.True(w.DLoginChgPw.Visible);
        });
    }

    [Fact]
    public void NewUI205_CloseDNewAccountDlg_NonNormalModeKeepsHidden()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            MShareGlobals.g_nClientLoginMode = 1;                       // 用户中心模式
            w.DLoginNew.Visible = false;
            w.DLoginChgPw.Visible = false;
            w.CloseDNewAccountDlg();
            Assert.False(w.DLoginNew.Visible);
            Assert.False(w.DLoginChgPw.Visible);
        });
    }

    [Fact]
    public void NewUI205_CloseDChgPwDlg_NormalLoginModeRestoresButtons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            MShareGlobals.g_nClientLoginMode = MShareGlobals.CLIENT_MODE_NORMAL;
            w.CloseDChgPwDlg();                                         // 445-455
            Assert.True(w.DLoginNew.Visible);
            Assert.True(w.DLoginChgPw.Visible);

            MShareGlobals.g_nClientLoginMode = 2;                       // 定制登录器
            w.DLoginNew.Visible = false;
            w.DLoginChgPw.Visible = false;
            w.CloseDChgPwDlg();
            Assert.False(w.DLoginNew.Visible);
            Assert.False(w.DLoginChgPw.Visible);
        });
    }

    [Fact]
    public void NewUI205_OpenDMenuDlg_And_OpenDSellDlg_Positions()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Make();
            w.OpenDMenuDlg(true);                                       // 476-481
            Assert.Equal(4, w.DMenuDlg.Left);
            Assert.Equal(216, w.DMenuDlg.Top);

            w.OpenDSellDlg(2);                                          // 483-487
            Assert.Equal(214, w.DSellDlg.Top);
        });
    }
}
