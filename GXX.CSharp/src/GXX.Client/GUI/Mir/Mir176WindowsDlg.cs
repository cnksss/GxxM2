using GXX.Client.DxComponent;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.Client.GUI.Mir;

/// <summary>
/// Mir176WindowsDlg.pas T176Windows（1.76 版本装备窗口）1:1 移植（全文 505 行）。
///
/// 本族窗口无 .dfm（布局由 Mir.GUI 资源流载入），LoadFromStream 覆写只设本版本可见性/图号/几何。
/// StPageUpClick / StateMemo4DirectPaint / ItemBagDirectPaint / MySelfAbilChange / OpenUserState
/// 均按原文继承链语义移植：176 版的 MySelfAbilChange 调 inherited 时**不经过**基类的
/// `g_ClientVersion in [cvSerial, cvMirSequel, cvMirNewUI205]` 守卫（原文如此）。
/// </summary>
public class T176Windows : TSerialWindows
{
    private TDxImageButton DPrevState;
    private TDxImageButton DNextState;

    private TSaveUIColor FStateWeightLabelColor = new();
    private TSaveUIColor FStateWearWeightLabelColor = new();
    private TSaveUIColor FStateHandWeightLabelColor = new();

    /// <summary>Mir176WindowsDlg.pas:70 constructor T176Windows.Create。</summary>
    public T176Windows()
    {
        // inherited;      // SerialWindowsDlg.pas:3280
        ClientVersion = TClientVersion.cv176;
    }

    /// <summary>Mir176WindowsDlg.pas:76 destructor T176Windows.Destroy。</summary>
    public void Destroy()
    {
        // inherited;
    }

    /// <summary>Mir176WindowsDlg.pas:81 procedure T176Windows.LoadFromStream(MemoryStream:TMemoryStream)。</summary>
    public override void LoadFromStream(System.IO.MemoryStream MemoryStream)
    {
        base.LoadFromStream(MemoryStream);                     // 83 inherited;

        DImageButtonAccount.Visible = false;                   // 85
        DImageButtonPassWord.Visible = false;                  // 86

        DLogin.ImageIndex.ImageType = TImageType.Prguse_wil;   // 88
        DLogin.ImageIndex.Up = 60;                             // 89

        DLoginOK.ImageIndex.ImageType = TImageType.Prguse_wil; // 91
        DLoginOK.ImageIndex.Up = -1;                           // 92
        DLoginOK.ImageIndex.Down = 62;                         // 93
        DLoginOK.OnPaint = null;                               // 94
        DLoginOK.Left = 169;                                   // 95
        DLoginOK.Top = 163;                                    // 96

        DLoginNew.ImageIndex.ImageType = TImageType.Prguse_wil; // 98
        DLoginNew.ImageIndex.Up = -1;                           // 99
        DLoginNew.ImageIndex.Down = 61;                         // 100
        DLoginNew.OnPaint = null;                               // 101
        DLoginNew.Left = 25;                                    // 102
        DLoginNew.Top = 207;                                    // 103

        DLoginChgPw.ImageIndex.ImageType = TImageType.Prguse_wil; // 105
        DLoginChgPw.ImageIndex.Up = -1;                           // 106
        DLoginChgPw.ImageIndex.Down = 53;                         // 107
        DLoginChgPw.OnPaint = null;                               // 108
        DLoginChgPw.Left = 130;                                   // 109
        DLoginChgPw.Top = 207;                                    // 110

        DLoginClose.Left = 252;                                // 112
        DLoginClose.Top = 28;                                  // 113

        DEdId_.Left = 98;                                      // 115
        DEdId_.Top = 85;                                       // 116

        DEdPasswd_.Left = 98;                                  // 118
        DEdPasswd_.Top = 117;                                  // 119

        DItemBagUpgrade.Visible = false;                       // 121

        DItemBag.ImageIndex.ImageType = TImageType.Prguse_wil; // 123
        DItemBag.ImageIndex.Up = 3;                            // 124

        DItemGrid.Left = 20;                                   // 126
        DItemGrid.Top = 14;                                    // 127

        DGold.Left = 10;                                       // 129
        DGold.Top = 190;                                       // 130

        DCloseBag.Left = 310;                                  // 132
        DCloseBag.Top = 203;                                   // 133

        DOpenShop.Visible = false;                             // 135
        DMerchantDlgHelp.Visible = false;                      // 136

        DStateGameTimeGirdLabel.Visible = false;               // 138

        DBottomLeftImageButton1.Visible = false;               // 140

        DChangeState.Visible = false;                          // 142

        DSWTitleActive.Visible = false;                        // 144
        DSWTitleButton1.Visible = false;                       // 145
        DSWTitleButton2.Visible = false;                       // 146
        DSWTitleButton3.Visible = false;                       // 147
        DSWTitleButton4.Visible = false;                       // 148
        DSWTitlePageUp.Visible = false;                        // 149
        DSWTitlePageDown.Visible = false;                      // 150

        DMerchantDlgHelp_.Visible = false;                     // 152
        // 隐藏4格
        DSWBujuk.Visible = false;                              // 154
        DSWBelt.Visible = false;                               // 155
        DSWBoots.Visible = false;                              // 156
        DSWCharm.Visible = false;                              // 157

        DSUSTitleActive.Visible = false;                       // 159
        DSUSTitleButton1.Visible = false;                      // 160
        DSUSTitleButton2.Visible = false;                      // 161
        DSUSTitleButton3.Visible = false;                      // 162
        DSUSTitleButton4.Visible = false;                      // 163
        DSUSTitlePageUp.Visible = false;                       // 164
        DSUSTitlePageDown.Visible = false;                     // 165

        // 隐藏4格
        DBujukUS1.Visible = false;                             // 168
        DBeltUS1.Visible = false;                              // 169
        DBootsUS1.Visible = false;                             // 170
        DCharmUS1.Visible = false;                             // 171

        DUserState1.ImageIndex.ImageType = TImageType.Prguse_wil; // 173
        DUserState1.ImageIndex.Up = 370;                          // 174

        DStateWin.ImageIndex.ImageType = TImageType.Prguse_wil;   // 176
        DStateWin.ImageIndex.Up = 370;                            // 177

        DPrevState = new TDxImageButton(DStateWin);            // 179
        DNextState = new TDxImageButton(DStateWin);            // 180
        DPrevState.Designing = false;                          // 181
        DNextState.Designing = false;                          // 182

        DPrevState.OnGetImage = DStateWin.OnGetImage;          // 184
        DPrevState.ImageIndex.ImageType = TImageType.Prguse_wil; // 185
        DPrevState.ImageIndex.Down = 373;                      // 186
        DPrevState.Left = 7;                                   // 187
        DPrevState.Top = 128;                                  // 188
        DPrevState.OnClick = DPrevStateClick;                  // 189
        DPrevState.OnClickSound = DLoginNewClickSound;         // 190
        DPrevState.ClickCount = TClickSound.csGlass;           // 191

        DNextState.OnGetImage = DStateWin.OnGetImage;          // 193
        DNextState.ImageIndex.ImageType = TImageType.Prguse_wil; // 194
        DNextState.ImageIndex.Down = 372;                      // 195
        DNextState.Left = 7;                                   // 196
        DNextState.Top = 187;                                  // 197
        DNextState.OnClick = DNextStateClick;                  // 198
        DNextState.OnClickSound = DLoginNewClickSound;         // 199
        DNextState.ClickCount = TClickSound.csGlass;           // 200

        // while DStatePageControl.PageCount > 4 do
        // DStatePageControl.Control[DStatePageControl.ControlCount - 1].Free;
        DStateTabSheet5.TabVisible = false;                    // 204

        DMerchantDlgHelp_.Visible = false;                     // 206
        DRecallHero.Visible = false;                           // 207
        DMyHeroState.Visible = false;                          // 208
        DMyHeroBag.Visible = false;                            // 209
        DRecallDeputyHero.Visible = false;                     // 210

        DStateForm1.UseSetting2 = true;                        // 212

        DStateForm2.ImageIndex.ImageType = TImageType.Prguse_wil; // 214
        DStateForm2.ImageIndex.Up = -1;                           // 215

        DStateMemo3.Height = 234;                              // 217
        // DStateForm3.ImageIndex.ImageType := Prguse_wil;
        // DStateForm3.ImageIndex.Up := 382;

        DUserStateForm1.UseSetting2 = true;                    // 221

        // DStateMemo4.ImageIndex.ImageType := Prguse_wil;
        // DStateMemo4.ImageIndex.Up := 383;
        DStateMemo4.Width = 168;                               // 225
        DStateMemo4.Height = 199;                              // 226

        DStMagBack6.Visible = false;                           // 228

        DStMagBack1.Left = DStMagBack1.Left + 1;               // 230
        DStMagBack2.Left = DStMagBack2.Left + 1;               // 231
        DStMagBack3.Left = DStMagBack3.Left + 1;               // 232
        DStMagBack4.Left = DStMagBack4.Left + 1;               // 233
        DStMagBack5.Left = DStMagBack5.Left + 1;               // 234

        DStMagBack1.Top = DStMagBack1.Top + 2;                 // 236
        DStMagBack2.Top = DStMagBack2.Top + 2;                 // 237
        DStMagBack3.Top = DStMagBack3.Top + 2;                 // 238
        DStMagBack4.Top = DStMagBack4.Top + 2;                 // 239
        DStMagBack5.Top = DStMagBack5.Top + 2;                 // 240

        FStateWeightLabelColor.Up = DStateWeightLabel.CaptionColor.Up.Value;             // 242
        FStateWeightLabelColor.Hot = DStateWeightLabel.CaptionColor.Hot.Value;           // 243
        FStateWeightLabelColor.Down = DStateWeightLabel.CaptionColor.Down.Value;         // 244
        FStateWeightLabelColor.Disabled = DStateWeightLabel.CaptionColor.Disabled.Value; // 245

        FStateWearWeightLabelColor.Up = DStateWearWeightLabel.CaptionColor.Up.Value;             // 247
        FStateWearWeightLabelColor.Hot = DStateWearWeightLabel.CaptionColor.Hot.Value;           // 248
        FStateWearWeightLabelColor.Down = DStateWearWeightLabel.CaptionColor.Down.Value;         // 249
        FStateWearWeightLabelColor.Disabled = DStateWearWeightLabel.CaptionColor.Disabled.Value; // 250

        FStateHandWeightLabelColor.Up = DStateHandWeightLabel.CaptionColor.Up.Value;             // 252
        FStateHandWeightLabelColor.Hot = DStateHandWeightLabel.CaptionColor.Hot.Value;           // 253
        FStateHandWeightLabelColor.Down = DStateHandWeightLabel.CaptionColor.Down.Value;         // 254
        FStateHandWeightLabelColor.Disabled = DStateHandWeightLabel.CaptionColor.Disabled.Value; // 255
    }

    /// <summary>Mir176WindowsDlg.pas:258 procedure T176Windows.OpenUserState()。</summary>
    public override void OpenUserState()
    {
        base.OpenUserState();                                  // 260 inherited;
        // 原文 261：DUserStateForm1.IsMale := pTHumFeature(@g_UserState1.feature.Buffer).btGender = 0;
        // 托管侧以 THumFeature 的字段偏移 2（btGender）等价读取（Grobal2.Types1.cs THumFeature）。
        DUserStateForm1.IsMale = GetUserStateGender() == 0;
        DUserState1.Visible = true;                            // 262
    }

    /// <summary>
    /// pTHumFeature(@g_UserState1.Feature.Buffer)^.btGender 的托管等价：
    /// THumFeature 为 Pack=1 顺序布局（btHair@0、btWeapon@1、btGender@2）。
    /// </summary>
    private static unsafe byte GetUserStateGender()
    {
        fixed (byte* p = MShareGlobals.g_UserState1.Feature.Buffer)
            return ((THumFeature*)p)->btGender;
    }

    /// <summary>Mir176WindowsDlg.pas:265 procedure T176Windows.MySelfAbilChange。</summary>
    public override void MySelfAbilChange()
    {
        // 原文 267-275：内部函数 AddSpace（与 TSerialWindows.AddSpace 同语义，此处原文为局部函数）
        string AddSpace(string Str)
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

        if (MShareGlobals.g_MySelf == null) return;                                  // 277
        DStateForm1.IsMale = MShareGlobals.g_MySelf.m_btSex == 0;                    // 278

        DStateLabelAC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.AC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.AC2);   // 280
        DStateLabelMAC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MAC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MAC2); // 281
        DStateLabelDC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.DC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.DC2);   // 282
        DStateLabelMC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MC2);   // 283
        DStateLabelSC.Caption = DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.SC1) + "-" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.SC2);   // 284
        DStateLabelHP.Caption = DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.HP) + "/" + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MaxHP); // 285
        DStateLabelMP.Caption = DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MP) + "/" + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MaxMP); // 286

        DStateExpLabel.Caption = AddSpace("当前经验") + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.Exp);        // 288
        DStateMaxExpLabel.Caption = AddSpace("升级经验") + DelphiRTL.IntToStr((int)MShareGlobals.g_MySelf.m_Abil.MaxExp);  // 289

        DStateWeightLabel.Caption = AddSpace("背包重量") + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.Weight) + "/" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MaxWeight); // 291
        if (MShareGlobals.g_MySelf.m_Abil.Weight > MShareGlobals.g_MySelf.m_Abil.MaxWeight)   // 292
        {
            DStateWeightLabel.CaptionColor.Up.Value = TColor.clRed;                          // 293
            DStateWeightLabel.CaptionColor.Hot.Value = TColor.clRed;                         // 294
            DStateWeightLabel.CaptionColor.Down.Value = TColor.clRed;                        // 295
        }
        else
        {
            DStateWeightLabel.CaptionColor.Up.Value = FStateWeightLabelColor.Up;             // 298
            DStateWeightLabel.CaptionColor.Hot.Value = FStateWeightLabelColor.Hot;           // 299
            DStateWeightLabel.CaptionColor.Down.Value = FStateWeightLabelColor.Down;         // 300
            DStateWeightLabel.CaptionColor.Disabled.Value = FStateWeightLabelColor.Disabled; // 301
        }

        DStateWearWeightLabel.Caption = AddSpace("穿戴重量") + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.WearWeight) + "/" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MaxWearWeight); // 304
        if (MShareGlobals.g_MySelf.m_Abil.WearWeight > MShareGlobals.g_MySelf.m_Abil.MaxWearWeight) // 305
        {
            DStateWearWeightLabel.CaptionColor.Up.Value = TColor.clRed;                      // 306
            DStateWearWeightLabel.CaptionColor.Hot.Value = TColor.clRed;                     // 307
            DStateWearWeightLabel.CaptionColor.Down.Value = TColor.clRed;                    // 308
        }
        else
        {
            DStateWearWeightLabel.CaptionColor.Up.Value = FStateWearWeightLabelColor.Up;             // 311
            DStateWearWeightLabel.CaptionColor.Hot.Value = FStateWearWeightLabelColor.Hot;           // 312
            DStateWearWeightLabel.CaptionColor.Down.Value = FStateWearWeightLabelColor.Down;         // 313
            DStateWearWeightLabel.CaptionColor.Disabled.Value = FStateWearWeightLabelColor.Disabled; // 314
        }

        DStateHandWeightLabel.Caption = AddSpace("腕力") + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.HandWeight) + "/" + DelphiRTL.IntToStr(MShareGlobals.g_MySelf.m_Abil.MaxHandWeight); // 317
        if (MShareGlobals.g_MySelf.m_Abil.HandWeight > MShareGlobals.g_MySelf.m_Abil.MaxHandWeight) // 318
        {
            DStateHandWeightLabel.CaptionColor.Up.Value = TColor.clRed;                      // 319
            DStateHandWeightLabel.CaptionColor.Hot.Value = TColor.clRed;                     // 320
            DStateHandWeightLabel.CaptionColor.Down.Value = TColor.clRed;                    // 321
        }
        else
        {
            DStateHandWeightLabel.CaptionColor.Up.Value = FStateHandWeightLabelColor.Up;             // 324
            DStateHandWeightLabel.CaptionColor.Hot.Value = FStateHandWeightLabelColor.Hot;           // 325
            DStateHandWeightLabel.CaptionColor.Down.Value = FStateHandWeightLabelColor.Down;         // 326
            DStateHandWeightLabel.CaptionColor.Disabled.Value = FStateHandWeightLabelColor.Disabled; // 327
        }

        DStateHitPointLabel.Caption = AddSpace("精确度") + DelphiRTL.IntToStr(MShareGlobals.g_nMyHitPoint);        // 330
        DStateSpeedPointLabel.Caption = AddSpace("敏捷度") + DelphiRTL.IntToStr(MShareGlobals.g_nMySpeedPoint);    // 331
        DStateAntiMagicLabel.Caption = AddSpace("魔法躲避") + DelphiRTL.IntToStr(MShareGlobals.g_nMyAntiMagic * 10) + "%";   // 332
        DStateAntiPoisonLabel.Caption = AddSpace("毒物躲避") + DelphiRTL.IntToStr(MShareGlobals.g_nMyAntiPoison * 10) + "%"; // 333

        DStatePoisonRecoverLabel.Caption = AddSpace("中毒恢复") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMyPoisonRecover * 10) + "%"; // 335
        DStateHealthRecoverLabel.Caption = AddSpace("体力恢复") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMyHealthRecover * 10) + "%"; // 336
        DStateSpellRecoverLabel.Caption = AddSpace("魔法恢复") + "+" + DelphiRTL.IntToStr(MShareGlobals.g_nMySpellRecover * 10) + "%";   // 337

        DStateGameDiamondLabel.Visible = false;                // 339
        DStateGameGirdLabel.Visible = false;                   // 340
        DStateGameGoldLabel.Visible = false;                   // 341
        DStateGameTimeGirdLabel.Visible = false;               // 342
    }

    /// <summary>Mir176WindowsDlg.pas:345 procedure T176Windows.DPrevStateClick(Sender:TObject; X, Y:Integer)。</summary>
    public void DPrevStateClick(object Sender, int X, int Y)
    {
        if (DStatePageControl.ActivePageIndex <= 0)
            DStatePageControl.ActivePageIndex = 3;             // 348 ;//DStatePageControl.PageCount - 1
        else
            DStatePageControl.ActivePageIndex = DStatePageControl.ActivePageIndex - 1;
    }

    /// <summary>Mir176WindowsDlg.pas:353 procedure T176Windows.DNextStateClick(Sender:TObject; X, Y:Integer)。</summary>
    public void DNextStateClick(object Sender, int X, int Y)
    {
        if (DStatePageControl.ActivePageIndex >= 3)            // 355 {DStatePageControl.PageCount - 1}
            DStatePageControl.ActivePageIndex = 0;
        else
            DStatePageControl.ActivePageIndex = DStatePageControl.ActivePageIndex + 1;
    }

    /// <summary>Mir176WindowsDlg.pas:361 procedure T176Windows.StPageUpClick(Sender:TObject; X, Y:Integer)。</summary>
    public override void StPageUpClick(object Sender, int X, int Y)
    {
        if (ReferenceEquals(Sender, DStPageUp))                // 363
        {
            if (MagicIndex > 0)                                // 364
                MagicIndex -= 5;                               // 365 Dec(MagicIndex, 5)

            if (MagicIndex < 0) MagicIndex = 0;                // 367
        }
        else
        {
            if (MagicIndex + 5 < MShareGlobals.g_MagicList.Count) // 370
            {
                MagicIndex += 5;                               // 371 Inc(MagicIndex, 5)
            }
        }
    }

    /// <summary>Mir176WindowsDlg.pas:376 procedure T176Windows.StateMemo4DirectPaint(Sender:TObject)。</summary>
    public override void StateMemo4DirectPaint(object Sender)
    {
        var vtRect = ((TDxControl)Sender).VirtualRect;         // 381
        var d = MShareGlobals.g_WMainImages[383];              // 382
        if (d != null)                                         // 383
            MShareGlobals.GameCanvas.Draw(vtRect.Left, vtRect.Top, d.ClientRect, d); // 384
    }

    /// <summary>Mir176WindowsDlg.pas:387 procedure T176Windows.ItemBagDirectPaint(Sender:TObject)。</summary>
    public override void ItemBagDirectPaint(object Sender)
    {
        // 原文 398：if g_MySelf = nil then Exit;
        if (MShareGlobals.g_MySelf == null) return;

        var DItemBag = (TDxImageButton)Sender;                 // 399
        var vtRect = DItemBag.VirtualRect;                     // 401

        // 原文 403：金币数字直接绘制（原文如此；该行在 with DItemBag do 块内）
        MShareGlobals.CurrentFont.TextOut(vtRect.Left + 62, vtRect.Top + 183,
            MShareGlobals.GetGoldStr((uint)ActorUiFields.GetGold(MShareGlobals.g_MySelf)), TColor.clWhite);

        /*
        if g_ClientConfig.boShowBagGameGold or g_ClientConfig.boShowBagGameGird or g_ClientConfig.boShowBagGameDiamond then
        begin
          ... 原文 405-457 的大段注释块（旧版包裹游戏币/灵符/金刚石分行显示）...
        end;
        */

        if (MShareGlobals.g_ConfigClient.btSuspensionShowItem == 0 && MShareGlobals.g_MouseItem.s.NameStr != string.Empty && MShareGlobals.g_boShowBagInfo != 0) // 461
        {
            string[] texts = new string[4];
            bool[] flags = new bool[2];
            MShareGlobals.GetMouseItemInfo(MShareGlobals.g_MySelf, MShareGlobals.g_MouseItem, true, texts, flags); // 462
            string ItemName = texts[0], sLine1 = texts[1], sLine2 = texts[2], sLine3 = texts[3];
            bool IsUseableLine2 = flags[0], IsUseableLine3 = flags[1];

            int nLeft = vtRect.Left + 68;                      // 464
            int nTop = vtRect.Top + 214;                       // 465

            if (ItemName != string.Empty)                      // 467
            {
                int nY = nTop;                                 // 468
                if (sLine2 != string.Empty)                    // 469
                    nY = nTop + 14;                            // 470
                if (sLine3 != string.Empty)                    // 471
                    nY = nTop + 14 * 2;                        // 472

                int n = MShareGlobals.DrawItemHintOldStyle(nLeft, nTop, ItemName, TColor.clYellow); // 474
                MShareGlobals.DrawItemHintOldStyle(nLeft + n, nTop, sLine1);                          // 475

                if (!IsUseableLine2)                           // 477
                    MShareGlobals.DrawItemHintOldStyle(nLeft, nTop + 14, sLine2, TColor.clRed);       // 478
                else
                    MShareGlobals.DrawItemHintOldStyle(nLeft, nTop + 14, sLine2);                     // 480

                if (!IsUseableLine3)                           // 482
                    MShareGlobals.DrawItemHintOldStyle(nLeft, nTop + 14 * 2, sLine3, TColor.clRed);   // 483
                else
                    MShareGlobals.DrawItemHintOldStyle(nLeft, nTop + 14 * 2, sLine3);                 // 485

                GXX.Core.Util.TStringList ItemDesc;            // 396 ItemDesc:TStringList
                if (MShareGlobals.g_ConfigClient.boDescSupportRenamItem == 0)                          // 487
                    ItemDesc = MShareGlobals.GetItemDesc(MShareGlobals.g_MouseItem, MShareGlobals.g_MouseItem.s.DBNameStr); // 488
                else
                {
                    ItemDesc = MShareGlobals.GetItemDesc(MShareGlobals.g_MouseItem, MShareGlobals.g_MouseItem.s.NameStr);   // 490
                    if (MShareGlobals.g_ConfigClient.boNoRenameDescReadDefault != 0 &&
                        (ItemDesc == null || ItemDesc.Count == 0))                                     // 491-492
                    {
                        ItemDesc = MShareGlobals.GetItemDesc(MShareGlobals.g_MouseItem, MShareGlobals.g_MouseItem.s.DBNameStr); // 493
                    }
                }
                if (ItemDesc != null && ItemDesc.Count > 0)        // 496
                {
                    for (int I = 0; I <= ItemDesc.Count - 1; I++)  // 497
                    {
                        MShareGlobals.CurrentFont.TextOut(nLeft, nY + 14 * (I + 1), ItemDesc[I],
                            (TColor)ItemDesc.GetObject(I));        // 498
                    }
                }
            }
        }
    }
}
