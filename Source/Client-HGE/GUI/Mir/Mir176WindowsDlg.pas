unit Mir176WindowsDlg;

interface

uses
  Windows,
  Messages,
  SysUtils,
  StrUtils,
  Classes,
  Graphics,
  Controls,
  Forms,
  Dialogs,
  StdCtrls,
  Grids,
  DxImageForm,
  DxImageButton,
  DxPageControl,
  DxEdit,
  DxLabel,
  DxMemo,
  DxImageGrid,
  DxPopupMenu,
  DxComboBox,
  DxLine,
  DxControls,
  DxComponents,
  Grobal2,
  ClFunc,
  HUtil32,
  MapUnit,
  SoundUtil,
  HGE,
  ComCtrls,
  Actor,
  GameImages,
  DxCanvas,
  HGECanvas,
  SerialWindowsDlg;
type
  T176Windows = class(TSerialWindows) // 176版本
  private
    DPrevState:TDxImageButton;
    DNextState:TDxImageButton;

    FStateWeightLabelColor:TSaveUIColor;
    FStateWearWeightLabelColor:TSaveUIColor;
    FStateHandWeightLabelColor:TSaveUIColor;
  public
    constructor Create; override;
    destructor Destroy(); override;
    procedure LoadFromStream(MemoryStream:TMemoryStream); override;
    //procedure BottomRightDirectPaint(Sender: TObject); override;
    procedure StateMemo4DirectPaint(Sender:TObject); override;
    procedure StPageUpClick(Sender:TObject; X, Y:Integer); override;
    procedure ItemBagDirectPaint(Sender:TObject); override;

    procedure DPrevStateClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DNextStateClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure OpenUserState(); override;
    procedure MySelfAbilChange; override;
  end;
implementation
uses
  ClMain,
  MShare,
  SDK;

constructor T176Windows.Create;
begin
  inherited;
  ClientVersion := cv176;
end;

destructor T176Windows.Destroy();
begin
  inherited;
end;

procedure T176Windows.LoadFromStream(MemoryStream:TMemoryStream);
begin
  inherited;

  DImageButtonAccount.Visible := False;
  DImageButtonPassWord.Visible := False;

  DLogin.ImageIndex.ImageType := Prguse_wil;
  DLogin.ImageIndex.Up := 60;

  DLoginOK.ImageIndex.ImageType := Prguse_wil;
  DLoginOK.ImageIndex.Up := -1;
  DLoginOK.ImageIndex.Down := 62;
  DLoginOK.OnPaint := nil;
  DLoginOK.Left := 169;
  DLoginOK.Top := 163;

  DLoginNew.ImageIndex.ImageType := Prguse_wil;
  DLoginNew.ImageIndex.Up := -1;
  DLoginNew.ImageIndex.Down := 61;
  DLoginNew.OnPaint := nil;
  DLoginNew.Left := 25;
  DLoginNew.Top := 207;

  DLoginChgPw.ImageIndex.ImageType := Prguse_wil;
  DLoginChgPw.ImageIndex.Up := -1;
  DLoginChgPw.ImageIndex.Down := 53;
  DLoginChgPw.OnPaint := nil;
  DLoginChgPw.Left := 130;
  DLoginChgPw.Top := 207;

  DLoginClose.Left := 252;
  DLoginClose.Top := 28;

  DEdId_.Left := 98;
  DEdId_.Top := 85;

  DEdPasswd_.Left := 98;
  DEdPasswd_.Top := 117;

  DItemBagUpgrade.Visible := False;

  DItemBag.ImageIndex.ImageType := Prguse_wil;
  DItemBag.ImageIndex.Up := 3;

  DItemGrid.Left := 20;
  DItemGrid.Top := 14;

  DGold.Left := 10;
  DGold.Top := 190;

  DCloseBag.Left := 310;
  DCloseBag.Top := 203;

  DOpenShop.Visible := False;
  DMerchantDlgHelp.Visible := False;

  DStateGameTimeGirdLabel.Visible := False;

  DBottomLeftImageButton1.Visible := False;

  DChangeState.Visible := False;

  DSWTitleActive.Visible := False;
  DSWTitleButton1.Visible := False;
  DSWTitleButton2.Visible := False;
  DSWTitleButton3.Visible := False;
  DSWTitleButton4.Visible := False;
  DSWTitlePageUp.Visible := False;
  DSWTitlePageDown.Visible := False;

  DMerchantDlgHelp_.Visible := False;
  // 隐藏4格
  DSWBujuk.Visible := False;
  DSWBelt.Visible := False;
  DSWBoots.Visible := False;
  DSWCharm.Visible := False;

  DSUSTitleActive.Visible := False;
  DSUSTitleButton1.Visible := False;
  DSUSTitleButton2.Visible := False;
  DSUSTitleButton3.Visible := False;
  DSUSTitleButton4.Visible := False;
  DSUSTitlePageUp.Visible := False;
  DSUSTitlePageDown.Visible := False;

  // 隐藏4格
  DBujukUS1.Visible := False;
  DBeltUS1.Visible := False;
  DBootsUS1.Visible := False;
  DCharmUS1.Visible := False;

  DUserState1.ImageIndex.ImageType := Prguse_wil;
  DUserState1.ImageIndex.Up := 370;

  DStateWin.ImageIndex.ImageType := Prguse_wil;
  DStateWin.ImageIndex.Up := 370;

  DPrevState := TDxImageButton.Create(DStateWin);
  DNextState := TDxImageButton.Create(DStateWin);
  DPrevState.Designing := False;
  DNextState.Designing := False;

  DPrevState.OnGetImage := DStateWin.OnGetImage;
  DPrevState.ImageIndex.ImageType := Prguse_wil;
  DPrevState.ImageIndex.Down := 373;
  DPrevState.Left := 7;
  DPrevState.Top := 128;
  DPrevState.OnClick := DPrevStateClick;
  DPrevState.OnClickSound := DLoginNewClickSound;
  DPrevState.ClickCount := csGlass;

  DNextState.OnGetImage := DStateWin.OnGetImage;
  DNextState.ImageIndex.ImageType := Prguse_wil;
  DNextState.ImageIndex.Down := 372;
  DNextState.Left := 7;
  DNextState.Top := 187;
  DNextState.OnClick := DNextStateClick;
  DNextState.OnClickSound := DLoginNewClickSound;
  DNextState.ClickCount := csGlass;

  // while DStatePageControl.PageCount > 4 do
    // DStatePageControl.Control[DStatePageControl.ControlCount - 1].Free;
  DStateTabSheet5.TabVisible := False;

  DMerchantDlgHelp_.Visible := False;
  DRecallHero.Visible := False;
  DMyHeroState.Visible := False;
  DMyHeroBag.Visible := False;
  DRecallDeputyHero.Visible := False;

  DStateForm1.UseSetting2 := True;

  DStateForm2.ImageIndex.ImageType := Prguse_wil;
  DStateForm2.ImageIndex.Up := -1;

  DStateMemo3.Height := 234;
  //DStateForm3.ImageIndex.ImageType := Prguse_wil;
  //DStateForm3.ImageIndex.Up := 382;

  DUserStateForm1.UseSetting2 := True;

  //DStateMemo4.ImageIndex.ImageType := Prguse_wil;
  //DStateMemo4.ImageIndex.Up := 383;
  DStateMemo4.Width := 168;
  DStateMemo4.Height := 199;

  DStMagBack6.Visible := False;

  DStMagBack1.Left := DStMagBack1.Left + 1;
  DStMagBack2.Left := DStMagBack2.Left + 1;
  DStMagBack3.Left := DStMagBack3.Left + 1;
  DStMagBack4.Left := DStMagBack4.Left + 1;
  DStMagBack5.Left := DStMagBack5.Left + 1;

  DStMagBack1.Top := DStMagBack1.Top + 2;
  DStMagBack2.Top := DStMagBack2.Top + 2;
  DStMagBack3.Top := DStMagBack3.Top + 2;
  DStMagBack4.Top := DStMagBack4.Top + 2;
  DStMagBack5.Top := DStMagBack5.Top + 2;

  FStateWeightLabelColor.Up := DStateWeightLabel.CaptionColor.Up.Color;
  FStateWeightLabelColor.Hot := DStateWeightLabel.CaptionColor.Hot.Color;
  FStateWeightLabelColor.Down := DStateWeightLabel.CaptionColor.Down.Color;
  FStateWeightLabelColor.Disabled := DStateWeightLabel.CaptionColor.Disabled.Color;

  FStateWearWeightLabelColor.Up := DStateWearWeightLabel.CaptionColor.Up.Color;
  FStateWearWeightLabelColor.Hot := DStateWearWeightLabel.CaptionColor.Hot.Color;
  FStateWearWeightLabelColor.Down := DStateWearWeightLabel.CaptionColor.Down.Color;
  FStateWearWeightLabelColor.Disabled := DStateWearWeightLabel.CaptionColor.Disabled.Color;

  FStateHandWeightLabelColor.Up := DStateHandWeightLabel.CaptionColor.Up.Color;
  FStateHandWeightLabelColor.Hot := DStateHandWeightLabel.CaptionColor.Hot.Color;
  FStateHandWeightLabelColor.Down := DStateHandWeightLabel.CaptionColor.Down.Color;
  FStateHandWeightLabelColor.Disabled := DStateHandWeightLabel.CaptionColor.Disabled.Color;
end;

procedure T176Windows.OpenUserState();
begin
  inherited;
  DUserStateForm1.IsMale := pTHumFeature(@g_UserState1.feature.Buffer).btGender = 0;
  DUserState1.Visible := True;
end;

procedure T176Windows.MySelfAbilChange;

  function AddSpace(Str:string):string;
  begin
    Result := Str;
    while True do
      if Length(Result) < 12 then
        Result := Result + ' '
      else
        Break;
  end;
begin
  if g_MySelf = nil then Exit;
  DStateForm1.IsMale := g_MySelf.m_btSex = 0;

  DStateLabelAC.Caption := IntToStr(g_MySelf.m_Abil.AC1) + '-' + IntToStr(g_MySelf.m_Abil.AC2);
  DStateLabelMAC.Caption := IntToStr(g_MySelf.m_Abil.MAC1) + '-' + IntToStr(g_MySelf.m_Abil.MAC2);
  DStateLabelDC.Caption := IntToStr(g_MySelf.m_Abil.DC1) + '-' + IntToStr(g_MySelf.m_Abil.DC2);
  DStateLabelMC.Caption := IntToStr(g_MySelf.m_Abil.MC1) + '-' + IntToStr(g_MySelf.m_Abil.MC2);
  DStateLabelSC.Caption := IntToStr(g_MySelf.m_Abil.SC1) + '-' + IntToStr(g_MySelf.m_Abil.SC2);
  DStateLabelHP.Caption := IntToStr(g_MySelf.m_Abil.HP) + '/' + IntToStr(g_MySelf.m_Abil.MaxHP);
  DStateLabelMP.Caption := IntToStr(g_MySelf.m_Abil.MP) + '/' + IntToStr(g_MySelf.m_Abil.MaxMP);

  DStateExpLabel.Caption := AddSpace('当前经验') + IntToStr(g_MySelf.m_Abil.Exp);
  DStateMaxExpLabel.Caption := AddSpace('升级经验') + IntToStr(g_MySelf.m_Abil.MaxExp);

  DStateWeightLabel.Caption := AddSpace('背包重量') + IntToStr(g_MySelf.m_Abil.Weight) + '/' + IntToStr(g_MySelf.m_Abil.MaxWeight);
  if g_MySelf.m_Abil.Weight > g_MySelf.m_Abil.MaxWeight then begin
    DStateWeightLabel.CaptionColor.Up.Color := clRed;
    DStateWeightLabel.CaptionColor.Hot.Color := clRed;
    DStateWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DStateWeightLabel.CaptionColor.Up.Color := FStateWeightLabelColor.Up;
    DStateWeightLabel.CaptionColor.Hot.Color := FStateWeightLabelColor.Hot;
    DStateWeightLabel.CaptionColor.Down.Color := FStateWeightLabelColor.Down;
    DStateWeightLabel.CaptionColor.Disabled.Color := FStateWeightLabelColor.Disabled;
  end;

  DStateWearWeightLabel.Caption := AddSpace('穿戴重量') + IntToStr(g_MySelf.m_Abil.WearWeight) + '/' + IntToStr(g_MySelf.m_Abil.MaxWearWeight);
  if g_MySelf.m_Abil.WearWeight > g_MySelf.m_Abil.MaxWearWeight then begin
    DStateWearWeightLabel.CaptionColor.Up.Color := clRed;
    DStateWearWeightLabel.CaptionColor.Hot.Color := clRed;
    DStateWearWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DStateWearWeightLabel.CaptionColor.Up.Color := FStateWearWeightLabelColor.Up;
    DStateWearWeightLabel.CaptionColor.Hot.Color := FStateWearWeightLabelColor.Hot;
    DStateWearWeightLabel.CaptionColor.Down.Color := FStateWearWeightLabelColor.Down;
    DStateWearWeightLabel.CaptionColor.Disabled.Color := FStateWearWeightLabelColor.Disabled;
  end;

  DStateHandWeightLabel.Caption := AddSpace('腕力') + IntToStr(g_MySelf.m_Abil.HandWeight) + '/' + IntToStr(g_MySelf.m_Abil.MaxHandWeight);
  if g_MySelf.m_Abil.HandWeight > g_MySelf.m_Abil.MaxHandWeight then begin
    DStateHandWeightLabel.CaptionColor.Up.Color := clRed;
    DStateHandWeightLabel.CaptionColor.Hot.Color := clRed;
    DStateHandWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DStateHandWeightLabel.CaptionColor.Up.Color := FStateHandWeightLabelColor.Up;
    DStateHandWeightLabel.CaptionColor.Hot.Color := FStateHandWeightLabelColor.Hot;
    DStateHandWeightLabel.CaptionColor.Down.Color := FStateHandWeightLabelColor.Down;
    DStateHandWeightLabel.CaptionColor.Disabled.Color := FStateHandWeightLabelColor.Disabled;
  end;

  DStateHitPointLabel.Caption := AddSpace('精确度') + IntToStr(g_nMyHitPoint);
  DStateSpeedPointLabel.Caption := AddSpace('敏捷度') + IntToStr(g_nMySpeedPoint);
  DStateAntiMagicLabel.Caption := AddSpace('魔法躲避') + IntToStr(g_nMyAntiMagic * 10) + '%';
  DStateAntiPoisonLabel.Caption := AddSpace('毒物躲避') + IntToStr(g_nMyAntiPoison * 10) + '%';

  DStatePoisonRecoverLabel.Caption := AddSpace('中毒恢复') + '+' + IntToStr(g_nMyPoisonRecover * 10) + '%';
  DStateHealthRecoverLabel.Caption := AddSpace('体力恢复') + '+' + IntToStr(g_nMyHealthRecover * 10) + '%';
  DStateSpellRecoverLabel.Caption := AddSpace('魔法恢复') + '+' + IntToStr(g_nMySpellRecover * 10) + '%';

  DStateGameDiamondLabel.Visible := False;
  DStateGameGirdLabel.Visible := False;
  DStateGameGoldLabel.Visible := False;
  DStateGameTimeGirdLabel.Visible := False;
end;

procedure T176Windows.DPrevStateClick(Sender:TObject; X, Y:Integer);
begin
  if DStatePageControl.ActivePageIndex <= 0 then
    DStatePageControl.ActivePageIndex := 3 // ;//DStatePageControl.PageCount - 1
  else
    DStatePageControl.ActivePageIndex := DStatePageControl.ActivePageIndex - 1;
end;

procedure T176Windows.DNextStateClick(Sender:TObject; X, Y:Integer);
begin
  if DStatePageControl.ActivePageIndex >= 3 {DStatePageControl.PageCount - 1} then
    DStatePageControl.ActivePageIndex := 0
  else
    DStatePageControl.ActivePageIndex := DStatePageControl.ActivePageIndex + 1;
end;

procedure T176Windows.StPageUpClick(Sender:TObject; X, Y:Integer);
begin
  if Sender = DStPageUp then begin
    if MagicIndex > 0 then
      Dec(MagicIndex, 5);

    if MagicIndex < 0 then MagicIndex := 0;
  end
  else begin
    if MagicIndex + 5 < g_MagicList.Count then begin
      Inc(MagicIndex, 5);
    end;
  end;
end;

procedure T176Windows.StateMemo4DirectPaint(Sender:TObject);
var
  vtRect:TRect;
  d:TTexture;
begin
  vtRect := TDxControl(Sender).VirtualRect;
  d := g_WMainImages.Images[383];
  if d <> nil then
    GameCanvas.Draw(vtRect.Left, vtRect.Top, d.ClientRect, d);
end;

procedure T176Windows.ItemBagDirectPaint(Sender:TObject);
var
  ItemName, sLine1, sLine2, sLine3:string;
  n, I, nLeft, nTop, nY:Integer;
  IsUseableLine2, IsUseableLine3:Boolean;

  vtRect:TRect;
  DItemBag:TDxImageButton;

  ItemDesc:TStringList;
begin
  if g_MySelf = nil then Exit;
  DItemBag := TDxImageButton(Sender);
  with DItemBag do begin
    vtRect := VirtualRect;

    CurrentFont.TextOut(vtRect.Left + 62, vtRect.Top + 183, GetGoldStr(g_MySelf.m_nGold), clWhite);

    {
    if g_ClientConfig.boShowBagGameGold or g_ClientConfig.boShowBagGameGird or g_ClientConfig.boShowBagGameDiamond then
    begin
      nLeft := g_ClientConfig.nShowBagGameGoldX;
      nTop := g_ClientConfig.nShowBagGameGoldY;

      if g_ClientConfig.boShowBagGameGoldSeparator then
      begin
        if g_ClientConfig.boShowBagGameGold then
        begin
          CurrentFont.TextOut(vtRect.Left + nLeft, vtRect.Top + nTop, g_sGameGoldName + '：' + GetGoldStr(g_MySelf.m_nGameGold), clWhite);
          nTop := nTop + 14;
        end;

        if g_ClientConfig.boShowBagGameGird then
        begin
          CurrentFont.TextOut(vtRect.Left + nLeft, vtRect.Top + nTop, g_sGameGirdName + '：' + GetGoldStr(g_nGameGird), clWhite);
          nTop := nTop + 14;
        end;

        if g_ClientConfig.boShowBagGameDiamond then
        begin
          CurrentFont.TextOut(vtRect.Left + nLeft, vtRect.Top + nTop, g_sGameDiamondName + '：' + GetGoldStr(g_nGameDiamond), clWhite);
          nTop := nTop + 14;
        end;
      end
      else
      begin
        if g_ClientConfig.boShowBagGameGold then
        begin
          CurrentFont.TextOut(vtRect.Left + nLeft, vtRect.Top + nTop, g_sGameGoldName + '：' + IntToStr(g_MySelf.m_nGameGold), clWhite);
          nTop := nTop + 14;
        end;

        if g_ClientConfig.boShowBagGameGird then
        begin
          CurrentFont.TextOut(vtRect.Left + nLeft, vtRect.Top + nTop, g_sGameGirdName + '：' + IntToStr(g_nGameGird), clWhite);
          nTop := nTop + 14;
        end;

        if g_ClientConfig.boShowBagGameDiamond then
        begin
          CurrentFont.TextOut(vtRect.Left + nLeft, vtRect.Top + nTop, g_sGameDiamondName + '：' + IntToStr(g_nGameDiamond), clWhite);
          nTop := nTop + 14;
        end;
      end;
    end
    else
    begin
      if (not g_boShowBagInfo) or (g_ClientConfig.btSuspensionShowItem > 0) then
        CurrentFont.TextOut(vtRect.Left + 68, vtRect.Top + 214, 'Alt + R 刷新包裹', clWhite);
    end;
    }

  end;

  if (g_ClientConfig.btSuspensionShowItem = 0) and (g_MouseItem.S.Name <> '') and g_boShowBagInfo then begin
    GetMouseItemInfo(g_MySelf, @g_MouseItem, True, ItemName, sLine1, sLine2, sLine3, IsUseableLine2, IsUseableLine3);

    nLeft := vtRect.Left + 68;
    nTop := vtRect.Top + 214;

    if ItemName <> '' then begin
      nY := nTop;
      if sLine2 <> '' then
        nY := nTop + 14;
      if sLine3 <> '' then
        nY := nTop + 14 * 2;

      n := DrawItemHintOldStyle(nLeft, nTop, ItemName, clYellow);
      DrawItemHintOldStyle(nLeft + n, nTop, sLine1);

      if not IsUseableLine2 then
        DrawItemHintOldStyle(nLeft, nTop + 14, sLine2, clRed)
      else
        DrawItemHintOldStyle(nLeft, nTop + 14, sLine2);

      if not IsUseableLine3 then
        DrawItemHintOldStyle(nLeft, nTop + 14 * 2, sLine3, clRed)
      else
        DrawItemHintOldStyle(nLeft, nTop + 14 * 2, sLine3);

      if not g_ClientConfig.boDescSupportRenamItem then
        ItemDesc := GetItemDesc(@g_MouseItem, g_MouseItem.S.DBName)
      else begin
        ItemDesc := GetItemDesc(@g_MouseItem, g_MouseItem.S.Name);
        if g_ClientConfig.boNoRenameDescReadDefault and
          ((ItemDesc = nil) or (ItemDesc.Count = 0)) then begin
          ItemDesc := GetItemDesc(@g_MouseItem, g_MouseItem.S.DBName)
        end;
      end;
      if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
        for I := 0 to ItemDesc.Count - 1 do begin
          CurrentFont.TextOut(nLeft, nY + 14 * (I + 1), ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        end;
      end;
    end;
  end;
end;

end.
