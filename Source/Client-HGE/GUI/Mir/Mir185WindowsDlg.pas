unit Mir185WindowsDlg;

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
  T185Windows = class(TSerialWindows) // 185°æ±¾
  private
    DPrevState:TDxImageButton;
    DNextState:TDxImageButton;
  public
    constructor Create; override;
    destructor Destroy(); override;
    procedure LoadFromStream(MemoryStream:TMemoryStream); override;
    //procedure BottomRightDirectPaint(Sender: TObject); override;

    procedure DPrevStateClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DNextStateClick(Sender:TObject; X, Y:Integer); stdcall;
  end;
implementation
uses
  ClMain,
  MShare,
  SDK;

constructor T185Windows.Create;
begin
  inherited;
  ClientVersion := cv185;
end;

destructor T185Windows.Destroy();
begin
  inherited;
end;

procedure T185Windows.LoadFromStream(MemoryStream:TMemoryStream);
begin
  inherited;
  DImageButtonAccount.Visible := False;
  DImageButtonPassWord.Visible := False;

  DLogin.ImageIndex.ImageType := Prguse_wil; // Prguse_wil;
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

  DBottomLeftImageButton1.Visible := False;
  DMerchantDlgHelp_.Visible := False;
  DChangeState.Visible := False;

  DSWTitleActive.Visible := False;
  DSWTitleButton1.Visible := False;
  DSWTitleButton2.Visible := False;
  DSWTitleButton3.Visible := False;
  DSWTitleButton4.Visible := False;
  DSWTitlePageUp.Visible := False;
  DSWTitlePageDown.Visible := False;

  DSUSTitleActive.Visible := False;
  DSUSTitleButton1.Visible := False;
  DSUSTitleButton2.Visible := False;
  DSUSTitleButton3.Visible := False;
  DSUSTitleButton4.Visible := False;
  DSUSTitlePageUp.Visible := False;
  DSUSTitlePageDown.Visible := False;

  DUserState1.ImageIndex.ImageType := Prguse3_wil;
  DUserState1.ImageIndex.Up := 207;

  DStateWin.ImageIndex.ImageType := Prguse3_wil;
  DStateWin.ImageIndex.Up := 207;

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
end;

procedure T185Windows.DPrevStateClick(Sender:TObject; X, Y:Integer);
begin
  if DStatePageControl.ActivePageIndex <= 0 then
    DStatePageControl.ActivePageIndex := 3 // DStatePageControl.PageCount - 2
  else
    DStatePageControl.ActivePageIndex := DStatePageControl.ActivePageIndex - 1;
end;

procedure T185Windows.DNextStateClick(Sender:TObject; X, Y:Integer);
begin
  if DStatePageControl.ActivePageIndex >= 3 {DStatePageControl.PageCount - 2} then
    DStatePageControl.ActivePageIndex := 0
  else
    DStatePageControl.ActivePageIndex := DStatePageControl.ActivePageIndex + 1;
end;

end.
