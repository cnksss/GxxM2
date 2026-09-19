unit MirNewUI205Dlg;

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
  Math,
  ComCtrls,
  Actor,
  GameImages,
  DxCanvas,
  HGECanvas,
  SerialWindowsDlg;
type
  TNewUI205Windows = class(TSerialWindows) // 传奇续章
  public
    constructor Create; override;
    destructor Destroy(); override;
    procedure LoadFromStream(MemoryStream:TMemoryStream); override;

    //procedure PaintText(Sender: TObject); stdcall;
    //procedure AddBackPaint(Sender: TObject); stdcall;
    procedure ShowMDlg(face:Integer; mname, msgstr:string; boSetBagItemPos:Boolean = True; IsDesigning:Boolean = False); override;

    procedure ItemBagDirectPaint(Sender:TObject); override;

    procedure CloseDNewAccountDlg; override;
    procedure CloseDChgPwDlg; override;
    procedure CloseDLoginDlg; override;

    procedure OpenDMenuDlg(IsTrading:Boolean); override;
    procedure OpenDSellDlg(nPage:Integer); override;
    //function DMessageDlg(MsgStr: string; DlgButtons: TMsgDlgButtons; DefaultText: string = ''; MaxLen: Integer = 0): TModalResult; override;

    //procedure OnEdtKeySelDlgKeyTextKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState); stdcall;

    //procedure SetMagicKeyDlg(SetMagic: PTClientMagic; magname: string; var curkey: Word); override;

    //procedure DoFnButtonClick(Sender: TObject); override;
    //procedure DoDHeroStateDlgDirectPaint(Sender: TObject); override;
  end;

implementation

uses
  ClMain,
  MShare,
  SDK,
  //LoadDxControl,
  ConfigShare;

constructor TNewUI205Windows.Create;
begin
  inherited;
  ClientVersion := cvMirNewUI205;
end;

destructor TNewUI205Windows.Destroy();
begin
  inherited;
end;

procedure TNewUI205Windows.LoadFromStream(MemoryStream:TMemoryStream);
begin
  inherited;

  g_MerchantImageIndex.Assign(DMerchantDlg.ImageIndex);
  g_MerchantCloseButtonRect := DMerchantDlgClose.ClientRect;

  DGuildManageNext.BringToFront;
  DGuildManagePrevious.BringToFront;
end;

{------------------------------------------------------------------------}
(*
procedure TNewUI205Windows.HeroStateDlgDirectPaint(Sender: TObject);
var
  d: TTexture;
  vtRect: TRect;
  PaintRect: TRect;
  nFaceIndex: Integer;
  Abil: TAbility;
  Color: TColor;
  S: string;
begin
  if g_MyHero = nil then Exit;

  if not g_ClientConfig.boUseOldSerialWindows then
  begin
    case g_MyHero.m_btSex of
      0: nFaceIndex := g_MyHero.m_btJob + 635;
      1: nFaceIndex := g_MyHero.m_btJob + 638;
    else
      nFaceIndex := -1;
    end;

    vtRect := DHeroStateDlg.VirtualRect;
    if nFaceIndex >= 0 then
    begin
      if g_MyHero.m_boDeath then
        d := g_WMain3Images.Grays[nFaceIndex]
      else
        d := g_WMain3Images.Images[nFaceIndex];
      if d <> nil then
      begin
        GameCanvas.Draw(vtRect.Left + 20, vtRect.Top + 26, d);
      end;
    end;
    Abil := g_MyHero.m_Abil;
    if Abil.HP < 0 then Abil.HP := 0;
    if Abil.MP < 0 then Abil.MP := 0;
    if Abil.HP > Abil.MaxHP then Abil.HP := Abil.MaxHP;
    if Abil.MP > Abil.MaxMP then Abil.MP := Abil.MaxMP;
    if Abil.Exp > Abil.MaxExp then Abil.Exp := Abil.MaxExp;

    if Abil.MaxHP > 0 then
    begin
      d := g_WMain3Images.Images[386];                                                                // HP
      if d <> nil then
      begin
        PaintRect := d.ClientRect;
        PaintRect.Right := Round(d.Width * Abil.HP / Abil.MaxHP);
        GameCanvas.Draw(vtRect.Left + 76 + 4, vtRect.Top + 24 + 10, PaintRect, d);
      end;
    end;

    if Abil.MaxMP > 0 then
    begin
      d := g_WMain3Images.Images[387];                                                                // MP
      if d <> nil then
      begin
        PaintRect := d.ClientRect;
        PaintRect.Right := Round(d.Width * Abil.MP / Abil.MaxMP);
        GameCanvas.Draw(vtRect.Left + 80 + 4, vtRect.Top + 35 + 10, PaintRect, d);
      end;
    end;

    if Abil.MaxExp > 0 then
    begin
      d := g_WMain3Images.Images[388];                                                                // EXP
      if d <> nil then
      begin
        PaintRect := d.ClientRect;
        PaintRect.Right := Round(d.Width * Abil.Exp / Abil.MaxExp);
        GameCanvas.Draw(vtRect.Left + 80 + 4, vtRect.Top + 50 + 10, PaintRect, d);
      end;
    end;

    if (g_MyHero.m_nNameColor <> GetRGB(g_ClientConfig.btPKLevel1NameColor)) or
       (g_MyHero.m_nNameColor <> GetRGB(g_ClientConfig.btPKLevel2NameColor)) then
      Color := clWhite
    else
      Color := g_MyHero.m_nNameColor;

    BoldTextOut(vtRect.Left + 70 + 4 + (100 - CurrentFont.TextWidth(g_MyHero.m_sUserName)) div 2, vtRect.Top + 5 + 10, g_MyHero.m_sUserName, Color);
    BoldTextOut(vtRect.Left + 4 + 4, vtRect.Top + 72 + 10, IntToStr(g_MyHero.m_Abil.Level), Color);
  end
  else
  begin
    case g_MyHero.m_btSex of
      0: nFaceIndex := g_MyHero.m_btJob + 635;
      1: nFaceIndex := g_MyHero.m_btJob + 638;
    else
      nFaceIndex := -1;
    end;

    vtRect := DHeroStateDlg.VirtualRect;
    if nFaceIndex >= 0 then
    begin
      if g_MyHero.m_boDeath then
        d := g_WMain3Images.Grays[nFaceIndex]
      else
        d := g_WMain3Images.Images[nFaceIndex];
      if d <> nil then
      begin
        GameCanvas.Draw(vtRect.Left + 16, vtRect.Top + 16, d);
      end;
    end;
    Abil := g_MyHero.m_Abil;
    if Abil.HP < 0 then Abil.HP := 0;
    if Abil.MP < 0 then Abil.MP := 0;
    if Abil.HP > Abil.MaxHP then Abil.HP := Abil.MaxHP;
    if Abil.MP > Abil.MaxMP then Abil.MP := Abil.MaxMP;
    if Abil.Exp > Abil.MaxExp then Abil.Exp := Abil.MaxExp;

    if Abil.MaxHP > 0 then
    begin
      d := g_WMain3Images.Images[386];                                                                // HP
      if d <> nil then
      begin
        PaintRect := d.ClientRect;
        PaintRect.Right := Round(d.Width / Abil.MaxHP * Abil.HP);
        GameCanvas.Draw(vtRect.Left + 76, vtRect.Top + 24, PaintRect, d);
      end;
    end;

    if Abil.MaxMP > 0 then
    begin
      d := g_WMain3Images.Images[387];                                                                // MP
      if d <> nil then
      begin
        PaintRect := d.ClientRect;
        PaintRect.Right := Round(d.Width / Abil.MaxMP * Abil.MP);
        GameCanvas.Draw(vtRect.Left + 80, vtRect.Top + 35, PaintRect, d);
      end;
    end;

    if Abil.MaxExp > 0 then
    begin
      d := g_WMain3Images.Images[388];                                                                // EXP
      if d <> nil then
      begin
        PaintRect := d.ClientRect;
        PaintRect.Right := Round(d.Width / Abil.MaxExp * Abil.Exp);
        GameCanvas.Draw(vtRect.Left + 80, vtRect.Top + 50, PaintRect, d);
      end;
    end;

    if (g_MyHero.m_nNameColor <> GetRGB(g_ClientConfig.btPKLevel1NameColor)) or
       (g_MyHero.m_nNameColor <> GetRGB(g_ClientConfig.btPKLevel2NameColor)) then
      Color := clWhite
    else
      Color := g_MyHero.m_nNameColor;

    BoldTextOut(vtRect.Left + 70 + (100 - CurrentFont.TextWidth(g_MyHero.m_sUserName)) div 2, vtRect.Top + 5, g_MyHero.m_sUserName, Color);

    S := IntToStr(g_MyHero.m_Abil.Level);
    if Length(S) > 3 then
      BoldTextOut(vtRect.Left + 4, vtRect.Top + 72, S, Color)
    else
      BoldTextOut(vtRect.Left + 4 + (18 - CurrentFont.TextWidth(S)) div 2, vtRect.Top + 72, S, Color);

    S := g_MyHero.m_sLoyalPoint + '%';
    BoldTextOut(vtRect.Left + 99 + (42 - CurrentFont.TextWidth(S)) div 2, vtRect.Top + 63, S, Color);
  end;
end;

procedure TNewUI205Windows.PaintText(Sender: TObject);
var
  vtRect: TRect;
begin

  if Sender = DDeleteHumanDlg then
  begin
    vtRect := TDxControl(Sender).VirtualRect;
    //GameCanvas.(vtRect.Left + 43, vtRect.Top + 30, Texture);
    if CurrentFont <> nil then
    begin
      CurrentFont.TextOut(vtRect.Left + 58, vtRect.Top + 75, '角色名称', clYellow);
      CurrentFont.TextOut(vtRect.Left + 140, vtRect.Top + 75, '等 级', clYellow);
      CurrentFont.TextOut(vtRect.Left + 188, vtRect.Top + 75, '职 业', clYellow);
      CurrentFont.TextOut(vtRect.Left + 233, vtRect.Top + 75, '性 别', clYellow);
    end;
  end;

  if Sender = DSelectChr then
  begin
    vtRect := TDxControl(Sender).VirtualRect;
    if (SCREENWIDTH = 1024) and (g_ConfigClient.boShow1024) then
    begin
      CurrentFont.TextOut(vtRect.Left + 77, vtRect.Bottom - 120, '名 称', clWhite);
      CurrentFont.TextOut(vtRect.Left + 77, vtRect.Bottom - 90, '等 级', clWhite);
      CurrentFont.TextOut(vtRect.Left + 77, vtRect.Bottom - 60, '职 业', clWhite);

      CurrentFont.TextOut(vtRect.Left + 680, vtRect.Bottom - 120, '名 称', clWhite);
      CurrentFont.TextOut(vtRect.Left + 680, vtRect.Bottom - 90, '等 级', clWhite);
      CurrentFont.TextOut(vtRect.Left + 680, vtRect.Bottom - 60, '职 业', clWhite);
    end
    else
    begin
      CurrentFont.TextOut(vtRect.Left + 20, vtRect.Bottom - 90, '名 称', clWhite);
      CurrentFont.TextOut(vtRect.Left + 20, vtRect.Bottom - 60, '等 级', clWhite);
      CurrentFont.TextOut(vtRect.Left + 20, vtRect.Bottom - 30, '职 业', clWhite);

      CurrentFont.TextOut(vtRect.Left + 535, vtRect.Bottom - 90, '名 称', clWhite);
      CurrentFont.TextOut(vtRect.Left + 535, vtRect.Bottom - 60, '等 级', clWhite);
      CurrentFont.TextOut(vtRect.Left + 535, vtRect.Bottom - 30, '职 业', clWhite);
    end;
  end;
end;

procedure TNewUI205Windows.AddBackPaint(Sender: TObject);
var
  vtRect: TRect;
begin
  if Sender = DLoginOK then
  begin
    vtRect := DLoginOK.VirtualRect;
    Windows.InflateRect(vtRect, -10, -10);
    GameCanvas.FillRect(vtRect, clBlack);
    //GameCanvas.DrawBlend(vtRect.Left, vtRect.Top, Texture);
    {Texture := g_WUINImages.Images[DLoginOK.ImageIndex.hot];
    GameCanvas.Draw(vtRect.Left, vtRect.Top, Texture);}
  end;
end;
*)

procedure TNewUI205Windows.ShowMDlg(face:Integer; mname, msgstr:string; boSetBagItemPos:Boolean; IsDesigning:Boolean);
var
  OffsetX, OffsetY:Integer;
begin
  //inherited; //HZQ 20230605 发现NewUi205多调用了一次，所以屏蔽掉
  MerchantFace := face;
  MerchantName := mname;
  MDlgStr := msgstr;

  OffsetX := g_ConfigClient.nNPCMsgDlgTextOffsetX;
  OffsetY := g_ConfigClient.nNPCMsgDlgTextOffsetY;

  if boSetBagItemPos then begin
    AddNpcMemo(DMerchantDlg, 38 + OffsetX, 43 + OffsetY, DMerchantDlgClick, MDlgStr); // 默认对话框
  end else begin
    AddNpcMemo(DMerchantDlg, 20 + OffsetX, 16 + OffsetY, DMerchantDlgClick, MDlgStr); // 自定义背景对话框
  end;

  if DMerchantDlgClose.Visible then
    DMerchantDlgClose.BringToFront;

  { TODO -opiaoyun -cGUI : NPC界面能否移动 【2013-07-23】 }
  if g_ClientConfig.boNPCGuiCanMove or frmMain.boNpcDlgCanMove then
    DMerchantDlg.Floating := True
  else
    DMerchantDlg.Floating := False;

  DMerchantDlg.Visible := True;

  // 打开自定义大对话框时，不修改背包位置 chongchong 2015-03-12
  if boSetBagItemPos then begin
    DItemBag.Left := SCREENWIDTH - DItemBag.Width - 10;
    DItemBag.Top := 90;
  end;

  // HZQ 20230717 增加使用旧的英雄对话框的自动躲避NPC对话框功能
  if g_ClientConfig.boUseOldSerialWindows then begin
      if (DHeroStateDlg185 <> nil) and  DHeroStateDlg185.Visible
      and (not g_ClientConfig.boHeroStateDlgNoMove) {and (not g_ClientConfig.boNPCGuiCanMove)} then begin
        DHeroStateDlg185.Left := DMerchantDlg.Width;
      end;
  end else begin
      if (DHeroStateDlg <> nil) and  DHeroStateDlg.Visible
      and (not g_ClientConfig.boHeroStateDlgNoMove) {and (not g_ClientConfig.boNPCGuiCanMove)} then begin
        DHeroStateDlg.Left := DMerchantDlg.Width;
      end;
  end;

  RequireAddPoints := True;
  LastestClickTime := MyGetTickCount;
end;

procedure TNewUI205Windows.ItemBagDirectPaint(Sender:TObject);
var
  vtRect:TRect;
  DItemBag:TDxImageButton;
  S, sTempName:string;
begin
  if g_MySelf = nil then Exit;
  DItemBag := TDxImageButton(Sender);
  with DItemBag do begin
    vtRect := VirtualRect;
    DGoldValue.Caption := GetGoldStr(g_MySelf.m_nGold);

    if not g_ClientConfig.boShowBagGameInfo then begin
      DItemBagGoldIcon.Visible := False;
      DlblItemBagGold.Visible := False;

      DItemBagGirdIcon.Visible := False;
      DlblItemBagGird.Visible := False;

      DItemBagDiamondIcon.Visible := False;
      DlblItemBagDiamond.Visible := False;
      Exit;
    end;

    DItemBagGoldIcon.Visible := FItemBagGoldIcon_Visible;
    DlblItemBagGold.Visible := FlblItemBagGold_Visible;
    if FlblItemBagGold_Visible then begin
      if g_ClientConfig.boShowBagGameGoldSeparator then
        sTempName := GetGoldStr(g_MySelf.m_nGameGold)
      else
        sTempName := IntToStr(g_MySelf.m_nGameGold);

      S := StringReplace(FlblItemBagGameGold_Text, '<$name>', g_sGameGoldName, [rfIgnoreCase]);
      S := StringReplace(S, '<$value>', sTempName, [rfIgnoreCase]);

      DlblItemBagGold.Caption := S;
    end;

    DItemBagGirdIcon.Visible := FItemBagGirdIcon_Visible;
    DlblItemBagGird.Visible := FlblItemBagGird_Visible;
    if FlblItemBagGird_Visible then begin
      if g_ClientConfig.boShowBagGameGoldSeparator then
        sTempName := GetGoldStr(g_nGameGird)
      else
        sTempName := IntToStr(g_nGameGird);

      S := StringReplace(FlblItemBagGird_Text, '<$name>', g_sGameGirdName, [rfIgnoreCase]);
      S := StringReplace(S, '<$value>', sTempName, [rfIgnoreCase]);

      DlblItemBagGird.Caption := S;
    end;

    DItemBagDiamondIcon.Visible := FItemBagDiamondIcon_Visible;
    DlblItemBagDiamond.Visible := FlblItemBagDiamond_Visible;
    if FlblItemBagDiamond_Visible then begin
      if g_ClientConfig.boShowBagGameGoldSeparator then
        sTempName := GetGoldStr(g_nGameDiamond)
      else
        sTempName := IntToStr(g_nGameDiamond);

      S := StringReplace(FlblItemBagDiamond_Text, '<$name>', g_sGameDiamondName, [rfIgnoreCase]);
      S := StringReplace(S, '<$value>', sTempName, [rfIgnoreCase]);

      DlblItemBagDiamond.Caption := S;
    end;
  end;
end;

procedure TNewUI205Windows.CloseDChgPwDlg;
begin
  inherited;
  if g_nClientLoginMode = CLIENT_MODE_NORMAL then begin
      DLoginNew.Visible := True;
      DLoginChgPw.Visible := True;
  end;

  DLoginNew.BringToFront;
  DLoginChgPw.BringToFront;
end;

procedure TNewUI205Windows.CloseDNewAccountDlg;
begin
  inherited;
  if g_nClientLoginMode = CLIENT_MODE_NORMAL then begin
    DLoginNew.Visible := True;
    DLoginChgPw.Visible := True;
  end;

  DLoginNew.BringToFront;
  DLoginChgPw.BringToFront;
end;

procedure TNewUI205Windows.CloseDLoginDlg;
begin
  inherited;
  DLoginNew.Visible := False;
  DLoginChgPw.Visible := False;
end;

procedure TNewUI205Windows.OpenDMenuDlg(IsTrading:Boolean);
begin
  inherited OpenDMenuDlg(IsTrading);
  DMenuDlg.Left := 4;
  DMenuDlg.Top := 216;
end;

procedure TNewUI205Windows.OpenDSellDlg(nPage:Integer);
begin
  inherited OpenDSellDlg(nPage);
  DSellDlg.Top := 214;
end;

(*
function TNewUI205Windows.DMessageDlg(MsgStr: string;
  DlgButtons: TMsgDlgButtons; DefaultText: string; MaxLen: Integer): TModalResult;
var
  XBase, I, lx, ly, nSpace: Integer;
  Ctrl: TDxControl;
begin
  if DMsgDlg.Visible then Exit;

  if not g_ConfigClient.boCustomUI then
  begin
    XBase := 300;
    lx := XBase;
    ly := 126;
    nSpace := 110;
  end
  else
  begin
    XBase := DMsgDlgCancel.Left;
    lx := XBase;
    ly := DMsgDlgCancel.Top;
    nSpace := FMsgDlgButtonSpace;
  end;

  DScreen.ClearHint;
  HintWindows.Clear;

  for I := DMsgDlg.ControlCount - 1 downto 0 do
  begin
    Ctrl := TDxControl(DMsgDlg.Control[I]);
    if (Ctrl <> DMsgDlgLabel) and (Ctrl <> EdDlgEdit) and (Ctrl <> DMsgDlgYes) and (Ctrl <> DMsgDlgNo) and (Ctrl <> DMsgDlgOk) and (Ctrl <> DMsgDlgCancel) then
    begin
      Ctrl.Free;
    end;
  end;

  DMsgDlgLabel.Visible := False;
  AddMessageDlg(DMsgDlg, DMsgDlgLabel.Left, DMsgDlgLabel.Top, MsgStr);

  ViewDlgEdit := False;

  EdDlgEdit.Visible := False;
  DMsgDlg.Floating := True;
  DMsgDlgOk.Visible := False;
  DMsgDlgYes.Visible := False;
  DMsgDlgCancel.Visible := False;
  DMsgDlgNo.Visible := False;
  DMsgDlg.Left := (SCREENWIDTH - DMsgDlg.Width) div 2;
  DMsgDlg.Top := (SCREENHEIGHT - DMsgDlg.Height) div 2;

  if mbCancel in DlgButtons then
  begin
    DMsgDlgCancel.Left := lx;
    DMsgDlgCancel.Top := ly;
    DMsgDlgCancel.Visible := True;
    lx := lx - nSpace;
  end;
  if mbNo in DlgButtons then
  begin
    DMsgDlgNo.Left := lx;
    DMsgDlgNo.Top := ly;
    DMsgDlgNo.Visible := True;
    lx := lx - nSpace;
  end;
  if mbYes in DlgButtons then
  begin
    DMsgDlgYes.Left := lx;
    DMsgDlgYes.Top := ly;
    DMsgDlgYes.Visible := True;
    lx := lx - nSpace;
  end;
  if (mbOk in DlgButtons) or (lx = XBase) then
  begin
    DMsgDlgOk.Left := lx;
    DMsgDlgOk.Top := ly;
    DMsgDlgOk.Visible := True;
  end;

  HideAllControls;

  if mbAbort in DlgButtons then
  begin
    ViewDlgEdit := True;
    DMsgDlg.Floating := False;

    EdDlgEdit.Text := DefaultText;
    if MaxLen > 0 then
    begin
      EdDlgEdit.MaxLength := MaxLen;
    end
    else
    begin
      EdDlgEdit.MaxLength := g_ClientConfig.nMaxInputStringLen;
    end;

    EdDlgEdit.Width := DMsgDlg.Width - 70;
    EdDlgEdit.Left := (DMsgDlg.Width - EdDlgEdit.Width) div 2;
    EdDlgEdit.Top := (DMsgDlg.Height - EdDlgEdit.Height) div 2 - 10;
    EdDlgEdit.Visible := True;
  end;

  DScreen.ClearHint;
  HintWindows.Clear;
  DMsgDlg.EnableFocus := not EdDlgEdit.Visible;

{$IF IsMultiThreadRender = 1}
  DMsgDlg.ShowModal();
{$ELSE}
  frmMain.TimerRender.Enabled := False;
  DMsgDlg.ShowModal(frmMain.RenderNotifyEvent);
  frmMain.TimerRender.Enabled := True;
{$IFEND}
  DlgEditText := EdDlgEdit.Text;
  // DScreen.AddChatBoardString('TSerialWindows.DMessageDlg '+DlgEditText, clGreen, clWhite);
  // if PlayScene.EdChat.Visible then
   // PlayScene.EdChat.SetFocus;
  ViewDlgEdit := False;
  RestoreHideControls;
  Result := DMsgDlg.DialogResult;

  for I := DMsgDlg.ControlCount - 1 downto 0 do
  begin
    Ctrl := TDxControl(DMsgDlg.Control[I]);
    if (Ctrl <> DMsgDlgLabel) and (Ctrl <> EdDlgEdit) and (Ctrl <> DMsgDlgYes) and (Ctrl <> DMsgDlgNo) and (Ctrl <> DMsgDlgOk) and (Ctrl <> DMsgDlgCancel) then
    begin
      Ctrl.Free;
    end;
  end;
end;
*)

(*
procedure TNewUI205Windows.DoDHeroStateDlgDirectPaint(Sender: TObject);
var
  D: TTexture;
  vtRect: TRect;
  nFaceIndex: Integer;
  Abil: TAbility;
  Color: TColor;
  S: string;
begin
  if g_MyHero = nil then Exit;

  if (g_MyHero.m_nNameColor <> GetRGB(g_ClientConfig.btPKLevel1NameColor)) or
     (g_MyHero.m_nNameColor <> GetRGB(g_ClientConfig.btPKLevel2NameColor)) then
    Color := clWhite
  else
    Color := g_MyHero.m_nNameColor;

  if LblHeroStateName.Visible then
  begin
    if LblHeroStateName.CaptionColor.Up.Color <> Color then
      LblHeroStateName.CaptionColor.Up.Color := Color;

    if LblHeroStateName.Caption <> g_MyHero.m_sUserName then
      LblHeroStateName.Caption := g_MyHero.m_sUserName;
  end;

  if BtnHeroStateIcon.Visible then
  begin
    case g_MyHero.m_btSex of
      0: nFaceIndex := 1215 - g_MyHero.m_btJob;
      1: nFaceIndex := 1212 - g_MyHero.m_btJob;
    else
      nFaceIndex := -1;
    end;

    vtRect := BtnHeroStateIcon.VirtualRect;
    if nFaceIndex >= 0 then
    begin
      if g_MyHero.m_boDeath then
        D := g_WUINImages.Grays[nFaceIndex]
      else
        D := g_WUINImages.Images[nFaceIndex];

      if D <> nil then
        GameCanvas.Draw(vtRect.Left + (BtnHeroStateIcon.Width - D.Width) div 2, vtRect.Top + (BtnHeroStateIcon.Height - D.Height) div 2, D);
    end;
  end;

  Abil := g_MyHero.m_Abil;
  if Abil.HP < 0 then Abil.HP := 0;
  if Abil.MP < 0 then Abil.MP := 0;
  if Abil.HP > Abil.MaxHP then Abil.HP := Abil.MaxHP;
  if Abil.MP > Abil.MaxMP then Abil.MP := Abil.MaxMP;
  if Abil.Exp > Abil.MaxExp then Abil.Exp := Abil.MaxExp;

  if (Abil.MaxHP > 0) and BtnHeroStateHP.Visible and (BtnHeroStateHP.ImageIndex.Up >= 0) then
  begin
    D := BtnHeroStateHP.ImageIndex.Image.Images[BtnHeroStateHP.ImageIndex.Up];
    if D <> nil then
      BtnHeroStateHP.Width := Round(D.Width / Abil.MaxHP * Abil.HP);
  end;

  if (Abil.MaxMP > 0) and BtnHeroStateMP.Visible and (BtnHeroStateMP.ImageIndex.Up >= 0) then
  begin
    D := BtnHeroStateMP.ImageIndex.Image.Images[BtnHeroStateMP.ImageIndex.Up];
    if D <> nil then
      BtnHeroStateMP.Width := Round(D.Width / Abil.MaxMP * Abil.MP);
  end;

  if g_MyHero.m_boTrainingNG and (g_MyHero.m_AbilNG.MaxNH > 0) and BtnHeroStateNG.Visible and (BtnHeroStateNG.ImageIndex.Up >= 0) then
  begin
    D := BtnHeroStateNG.ImageIndex.Image.Images[BtnHeroStateNG.ImageIndex.Up];                                                                // 内力
    if d <> nil then
      BtnHeroStateNG.Width := Round(d.Width / g_MyHero.m_AbilNG.MaxNH * g_MyHero.m_AbilNG.NH);
  end;

  if (Abil.MaxExp > 0) and BtnHeroStateExp.Visible and (BtnHeroStateExp.ImageIndex.Up >= 0) then
  begin
    D := BtnHeroStateExp.ImageIndex.Image.Images[BtnHeroStateExp.ImageIndex.Up];                                                              // EXP
    if D <> nil then
      BtnHeroStateExp.Width := Round(D.Width / Abil.MaxExp * Abil.Exp);
  end;

  S := IntToStr(g_MyHero.m_Abil.Level);

  if LblHeroStateLevel.CaptionColor.Up.Color <> Color then
    LblHeroStateLevel.CaptionColor.Up.Color := Color;

  if LblHeroStateLevel.Caption <> S then
    LblHeroStateLevel.Caption := S;

  case g_MyHero.m_btJob of
    0: S := '战';
    1: S := '法';
    2: S := '道';
  else
    S := '';
  end;

  if LblHeroStateJob.CaptionColor.Up.Color <> Color then
    LblHeroStateJob.CaptionColor.Up.Color := Color;
  LblHeroStateJob.Caption := S;

  if g_MyHero.m_nState and $04000000 <> 0 then                                                      // 石化
    Color := clRed
  else
    Color := clWhite;
  if LblHeroStateSH.CaptionColor.Up.Color <> Color then
    LblHeroStateSH.CaptionColor.Up.Color := Color;

  if g_MyHero.m_nState and $00080000 <> 0 then                                                      // 冰冻
    Color := clRed
  else
    Color := clWhite;
  if LblHeroStateBD.CaptionColor.Up.Color <> Color then
    LblHeroStateBD.CaptionColor.Up.Color := Color;

  // 处理旋风斩技能问题 [网罩]变色 piaoyun 2013-10-26
  if (g_MyHero.m_boCobweb) and (not g_MyHero.m_boDuanJin) then                                      // 网罩
    Color := clRed
  else
    Color := clWhite;
  if LblHeroStateWZ.CaptionColor.Up.Color <> Color then
    LblHeroStateWZ.CaptionColor.Up.Color := Color;

  if g_MyHero.m_boFixedHero then
  begin
    if g_MyHero.m_boIsDeputy then
      LblHeroStateDeputy.Caption := '副'
    else
      LblHeroStateDeputy.Caption := '主';
  end
  else
  begin
    if g_MyHero.m_boIsDeputy then
      LblHeroStateDeputy.Caption := '卧'
    else
      LblHeroStateDeputy.Caption := '白';
  end;
end;
*)

end.
