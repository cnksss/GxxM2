using GXX.Client.GUI.DxComponent;
using GXX.Core.Protocol;

namespace GXX.Client.GUI.Mir;

/// <summary>
/// MirNewUI205Dlg.pas TNewUI205Windows（205 版 / 传奇续章版装备窗口）1:1 移植（全文 763 行）。
///
/// 原文 102-323 行的 HeroStateDlgDirectPaint / PaintText / AddBackPaint 与 489-618 的 DMessageDlg、
/// 620-761 的 DoDHeroStateDlgDirectPaint 全部处于 `(* *)` 注释块内，故实际生效的覆写为：
/// LoadFromStream(90)、ShowMDlg(325)、ItemBagDirectPaint(377)、CloseDChgPwDlg(445)、
/// CloseDNewAccountDlg(457)、CloseDLoginDlg(469)、OpenDMenuDlg(476)、OpenDSellDlg(483)。
/// </summary>
public class TNewUI205Windows : TSerialWindows
{
    /// <summary>MirNewUI205Dlg.pas:79 constructor TNewUI205Windows.Create。</summary>
    public TNewUI205Windows()
    {
        // inherited;      // SerialWindowsDlg.pas:3280
        ClientVersion = TClientVersion.cvMirNewUI205;
    }

    /// <summary>MirNewUI205Dlg.pas:85 destructor TNewUI205Windows.Destroy。</summary>
    public void Destroy()
    {
        // inherited;
    }

    /// <summary>MirNewUI205Dlg.pas:90 procedure TNewUI205Windows.LoadFromStream(MemoryStream:TMemoryStream)。</summary>
    public override void LoadFromStream(System.IO.MemoryStream MemoryStream)
    {
        base.LoadFromStream(MemoryStream);                     // 92 inherited;

        MShareGlobals.g_MerchantImageIndex.Assign(DMerchantDlg.ImageIndex);      // 94
        MShareGlobals.g_MerchantCloseButtonRect = DMerchantDlgClose.ClientRect;  // 95

        DGuildManageNext.BringToFront();                       // 97
        DGuildManagePrevious.BringToFront();                   // 98
    }

    /// <summary>
    /// MirNewUI205Dlg.pas:325 procedure TNewUI205Windows.ShowMDlg(face:Integer; mname, msgstr:string;
    /// boSetBagItemPos:Boolean; IsDesigning:Boolean)。
    /// 原文 329 行注释说明：NewUi205 会比基类多调用一次，故屏蔽 inherited。
    /// </summary>
    public override void ShowMDlg(int face, string mname, string msgstr,
        bool boSetBagItemPos = true, bool IsDesigning = false)
    {
        // inherited; //HZQ 20230605 发现NewUi205多调用了一次，所以屏蔽掉   // 329
        MerchantFace = face;                                   // 330
        MerchantName = mname;                                  // 331
        MDlgStr = msgstr;                                      // 332

        int OffsetX = MShareGlobals.g_ConfigClient.nNPCMsgDlgTextOffsetX;  // 334
        int OffsetY = MShareGlobals.g_ConfigClient.nNPCMsgDlgTextOffsetY;  // 335

        if (boSetBagItemPos)                                   // 337
        {
            AddNpcMemo(DMerchantDlg, 38 + OffsetX, 43 + OffsetY, DMerchantDlgClick, MDlgStr); // 338 默认对话框
        }
        else
        {
            AddNpcMemo(DMerchantDlg, 20 + OffsetX, 16 + OffsetY, DMerchantDlgClick, MDlgStr); // 340 自定义背景对话框
        }

        if (DMerchantDlgClose.Visible)                         // 343
            DMerchantDlgClose.BringToFront();                  // 344

        // { TODO -opiaoyun -cGUI : NPC界面能否移动 【2013-07-23】 }
        if (MShareGlobals.g_ConfigClient.boNPCGuiCanMove != 0 || frmMain.boNpcDlgCanMove) // 347
            DMerchantDlg.Floating = true;                      // 348
        else
            DMerchantDlg.Floating = false;                     // 350

        DMerchantDlg.Visible = true;                           // 352

        // 打开自定义大对话框时，不修改背包位置 chongchong 2015-03-12
        if (boSetBagItemPos)                                   // 355
        {
            DItemBag.Left = ScreenSize.SCREENWIDTH - DItemBag.Width - 10; // 356
            DItemBag.Top = 90;                                 // 357
        }

        // HZQ 20230717 增加使用旧的英雄对话框的自动躲避NPC对话框功能
        if (MShareGlobals.g_ConfigClient.boUseOldSerialWindows != 0)  // 361
        {
            if (DHeroStateDlg185 != null && DHeroStateDlg185.Visible
                && MShareGlobals.g_ConfigClient.boHeroStateDlgNoMove == 0) // 362-363
            {
                DHeroStateDlg185.Left = DMerchantDlg.Width;    // 364
            }
        }
        else
        {
            if (DHeroStateDlg != null && DHeroStateDlg.Visible
                && MShareGlobals.g_ConfigClient.boHeroStateDlgNoMove == 0) // 367-368
            {
                DHeroStateDlg.Left = DMerchantDlg.Width;       // 369
            }
        }

        RequireAddPoints = true;                               // 373
        LastestClickTime = MShareGlobals.MyGetTickCount;       // 374
    }

    /// <summary>MirNewUI205Dlg.pas:377 procedure TNewUI205Windows.ItemBagDirectPaint(Sender:TObject)。</summary>
    public override void ItemBagDirectPaint(object Sender)
    {
        if (MShareGlobals.g_MySelf == null) return;            // 383
        var DItemBag = (TDxImageButton)Sender;                 // 384
        var vtRect = DItemBag.VirtualRect;                     // 386
        DGoldValue.Caption = MShareGlobals.GetGoldStr((uint)ActorUiFields.GetGold(MShareGlobals.g_MySelf)); // 387

        if (MShareGlobals.g_ConfigClient.boShowBagGameInfo == 0) // 389
        {
            DItemBagGoldIcon.Visible = false;                  // 390
            DlblItemBagGold.Visible = false;                   // 391

            DItemBagGirdIcon.Visible = false;                  // 393
            DlblItemBagGird.Visible = false;                   // 394

            DItemBagDiamondIcon.Visible = false;               // 396
            DlblItemBagDiamond.Visible = false;                // 397
            return;                                            // 398 Exit;
        }

        string S, sTempName;                                   // 381
        DItemBagGoldIcon.Visible = FItemBagGoldIcon_Visible != 0;      // 401
        DlblItemBagGold.Visible = FlblItemBagGold_Visible != 0;        // 402
        if (FlblItemBagGold_Visible != 0)                              // 403
        {
            if (MShareGlobals.g_ConfigClient.boShowBagGameGoldSeparator != 0)  // 404
                sTempName = MShareGlobals.GetGoldStr((uint)ActorUiFields.GetGameGold(MShareGlobals.g_MySelf)); // 405
            else
                sTempName = GXX.Core.Rtl.DelphiRTL.IntToStr(ActorUiFields.GetGameGold(MShareGlobals.g_MySelf)); // 407

            S = FlblItemBagGameGold_Text.Replace("<$name>", MShareGlobals.g_sGameGoldName, System.StringComparison.OrdinalIgnoreCase); // 409
            S = S.Replace("<$value>", sTempName, System.StringComparison.OrdinalIgnoreCase);                                          // 410

            DlblItemBagGold.Caption = S;                       // 412
        }

        DItemBagGirdIcon.Visible = FItemBagGirdIcon_Visible != 0;      // 415
        DlblItemBagGird.Visible = FlblItemBagGird_Visible != 0;        // 416
        if (FlblItemBagGird_Visible != 0)                              // 417
        {
            if (MShareGlobals.g_ConfigClient.boShowBagGameGoldSeparator != 0)  // 418
                sTempName = MShareGlobals.GetGoldStr(MShareGlobals.g_nGameGird);   // 419
            else
                sTempName = GXX.Core.Rtl.DelphiRTL.IntToStr(MShareGlobals.g_nGameGird); // 421

            S = FlblItemBagGird_Text.Replace("<$name>", MShareGlobals.g_sGameGirdName, System.StringComparison.OrdinalIgnoreCase); // 423
            S = S.Replace("<$value>", sTempName, System.StringComparison.OrdinalIgnoreCase);                                      // 424

            DlblItemBagGird.Caption = S;                       // 426
        }

        DItemBagDiamondIcon.Visible = FItemBagDiamondIcon_Visible != 0; // 429
        DlblItemBagDiamond.Visible = FlblItemBagDiamond_Visible != 0;   // 430
        if (FlblItemBagDiamond_Visible != 0)                            // 431
        {
            if (MShareGlobals.g_ConfigClient.boShowBagGameGoldSeparator != 0)  // 432
                sTempName = MShareGlobals.GetGoldStr(MShareGlobals.g_nGameDiamond); // 433
            else
                sTempName = GXX.Core.Rtl.DelphiRTL.IntToStr(MShareGlobals.g_nGameDiamond); // 435

            S = FlblItemBagDiamond_Text.Replace("<$name>", MShareGlobals.g_sGameDiamondName, System.StringComparison.OrdinalIgnoreCase); // 437
            S = S.Replace("<$value>", sTempName, System.StringComparison.OrdinalIgnoreCase);                                             // 438

            DlblItemBagDiamond.Caption = S;                    // 440
        }
    }

    /// <summary>MirNewUI205Dlg.pas:445 procedure TNewUI205Windows.CloseDChgPwDlg。</summary>
    public override void CloseDChgPwDlg()
    {
        base.CloseDChgPwDlg();                                 // 447 inherited;
        if (MShareGlobals.g_nClientLoginMode == MShareGlobals.CLIENT_MODE_NORMAL) // 448
        {
            DLoginNew.Visible = true;                          // 449
            DLoginChgPw.Visible = true;                        // 450
        }

        DLoginNew.BringToFront();                              // 453
        DLoginChgPw.BringToFront();                            // 454
    }

    /// <summary>MirNewUI205Dlg.pas:457 procedure TNewUI205Windows.CloseDNewAccountDlg。</summary>
    public override void CloseDNewAccountDlg()
    {
        base.CloseDNewAccountDlg();                            // 459 inherited;
        if (MShareGlobals.g_nClientLoginMode == MShareGlobals.CLIENT_MODE_NORMAL) // 460
        {
            DLoginNew.Visible = true;                          // 461
            DLoginChgPw.Visible = true;                        // 462
        }

        DLoginNew.BringToFront();                              // 465
        DLoginChgPw.BringToFront();                            // 466
    }

    /// <summary>MirNewUI205Dlg.pas:469 procedure TNewUI205Windows.CloseDLoginDlg。</summary>
    public override void CloseDLoginDlg()
    {
        base.CloseDLoginDlg();                                 // 471 inherited;
        DLoginNew.Visible = false;                             // 472
        DLoginChgPw.Visible = false;                           // 473
    }

    /// <summary>MirNewUI205Dlg.pas:476 procedure TNewUI205Windows.OpenDMenuDlg(IsTrading:Boolean)。</summary>
    public override void OpenDMenuDlg(bool IsTrading)
    {
        base.OpenDMenuDlg(IsTrading);                          // 478 inherited OpenDMenuDlg(IsTrading);
        DMenuDlg.Left = 4;                                     // 479
        DMenuDlg.Top = 216;                                    // 480
    }

    /// <summary>MirNewUI205Dlg.pas:483 procedure TNewUI205Windows.OpenDSellDlg(nPage:Integer)。</summary>
    public override void OpenDSellDlg(int nPage)
    {
        base.OpenDSellDlg(nPage);                              // 485 inherited OpenDSellDlg(nPage);
        DSellDlg.Top = 214;                                    // 486
    }
}
