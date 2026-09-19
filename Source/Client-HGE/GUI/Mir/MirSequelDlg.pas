unit MirSequelDlg;

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
  TSequelWindows = class(TSerialWindows) // 传奇续章
  private

  public
    constructor Create; override;
    destructor Destroy(); override;
    procedure LoadFromStream(MemoryStream:TMemoryStream); override;

    {
    procedure BottomLeftInRealArea(Sender: TObject; X, Y: Integer;
      var IsRealArea: Boolean); override;
    procedure BottomLeftDirectPaint(Sender: TObject); override;

    procedure BottomRightInRealArea(Sender: TObject; X, Y: Integer;
      var IsRealArea: Boolean); override;
    procedure BottomRightDirectPaint(Sender: TObject); override;

    procedure BottomCenterInRealArea(Sender: TObject; X, Y: Integer;
      var IsRealArea: Boolean); override;
    procedure BottomCenterDirectPaint(Sender: TObject); override;
    }
  end;

implementation
uses
  ClMain,
  MShare,
  SDK;

constructor TSequelWindows.Create;
begin
  inherited;
  ClientVersion := cvMirSequel;
end;

destructor TSequelWindows.Destroy();
begin
  inherited;
end;

procedure TSequelWindows.LoadFromStream(MemoryStream:TMemoryStream);
begin
  inherited;
  // DLoginDlg.ImageIndex.ImageType := UI3_wil;
  // DLoginDlg.ImageIndex.Up := 2;

  DLogin.ImageIndex.ImageType := UI3_wil;
  DLogin.ImageIndex.Up := 1;

  // 传奇续章差按钮 chongchong 2014-04-15
  (*
  //DImageButtonAccount.Caption := '用户名';
  DImageButtonAccount.Left := 36;
  DImageButtonAccount.Top := 79;
  DImageButtonAccount.Visible := True;

  DEdId.Left := 118;
  DEdId.Top := 81;

  //DImageButtonPassWord.Caption := '密码';
  DImageButtonAccount.Left := 36;
  DImageButtonAccount.Top := 114;

  DEdPasswd.Left := 118;
  DEdPasswd.Top := 117;

  DLoginNew.Left := 30;
  DLoginNew.Top := 155;

  DLoginOK.Left := 160;
  DLoginOK.Top := 155;

  DLoginChgPw.Left := 160;
  DLoginChgPw.Top := 200;
  *)

 // DSelServerDlg.ImageIndex.ImageType := UI3_wil;
 // DSelServerDlg.ImageIndex.Up := 2;

  DServerDlg.ImageIndex.ImageType := UI3_wil;
  DServerDlg.ImageIndex.Up := 0;

  if SCREENWIDTH <> 1024 then begin
    DSelectChr.ImageIndex.ImageType := UI3_wil;
    DSelectChr.ImageIndex.Up := 2;
  end;

  DHeroStateDlg185.ImageIndex.ImageType := UI3_wil;
  DHeroStateDlg185.ImageIndex.Up := 10;

  DMyBag.ImageIndex.ImageType := UI3_wil;
  DMyBag.ImageIndex.Up := 30;
  DMyBag.ImageIndex.Hot := 31;
  DMyBag.ImageIndex.Down := 32;
  DMyBag.Left := DMyBag.Left - 14;
  DMyBag.Top := DMyBag.Top - 10;

  DMyMagic.ImageIndex.ImageType := UI3_wil;
  DMyMagic.ImageIndex.Up := 40;
  DMyMagic.ImageIndex.Hot := 41;
  DMyMagic.ImageIndex.Down := 42;
  DMyMagic.Left := DMyMagic.Left - 14;
  DMyMagic.Top := DMyMagic.Top - 12;

  DMyState.ImageIndex.ImageType := UI3_wil;
  DMyState.ImageIndex.Up := 50;
  DMyState.ImageIndex.Hot := 51;
  DMyState.ImageIndex.Down := 52;
  DMyState.Left := DMyState.Left - 14;
  DMyState.Top := DMyState.Top - 12;

  DVoice.ImageIndex.ImageType := UI3_wil;
  DVoice.ImageIndex.Up := 60;
  DVoice.ImageIndex.Hot := 61;
  DVoice.ImageIndex.Down := 62;
  DVoice.Left := DVoice.Left - 14;
  DVoice.Top := DVoice.Top - 12;

  DWeb.ImageIndex.ImageType := UI3_wil;
  DWeb.ImageIndex.Up := 80;
  DWeb.ImageIndex.Hot := 81;
  DWeb.ImageIndex.Down := 82;

  DBotMission.ImageIndex.ImageType := UI3_wil;
  DBotMission.ImageIndex.Up := 90;
  DBotMission.ImageIndex.Hot := 91;
  DBotMission.ImageIndex.Down := 92;

  DActionLog.ImageIndex.ImageType := UI3_wil;
  DActionLog.ImageIndex.Up := 521;
  DActionLog.ImageIndex.Hot := 522;
  DActionLog.ImageIndex.Down := 523;

  DControlHelp.ImageIndex.ImageType := UI3_wil;
  DControlHelp.ImageIndex.Up := 100;
  DControlHelp.ImageIndex.Hot := 101;
  DControlHelp.ImageIndex.Down := 102;

  DBotExit.ImageIndex.ImageType := UI3_wil;
  DBotExit.ImageIndex.Up := 110;
  DBotExit.ImageIndex.Hot := 111;
  DBotExit.ImageIndex.Down := 112;

  DBotFriend.ImageIndex.ImageType := UI3_wil;
  DBotFriend.ImageIndex.Up := 120;
  DBotFriend.ImageIndex.Hot := 121;
  DBotFriend.ImageIndex.Down := 122;

  DBotTrade.ImageIndex.ImageType := UI3_wil;
  DBotTrade.ImageIndex.Up := 130;
  DBotTrade.ImageIndex.Hot := 131;
  DBotTrade.ImageIndex.Down := 132;

  DBotRank.ImageIndex.ImageType := UI3_wil;
  DBotRank.ImageIndex.Up := 140;
  DBotRank.ImageIndex.Hot := 141;
  DBotRank.ImageIndex.Down := 142;

  DBotWhisper.ImageIndex.ImageType := UI3_wil;
  DBotWhisper.ImageIndex.Up := 150;
  DBotWhisper.ImageIndex.Hot := 151;
  DBotWhisper.ImageIndex.Down := 152;

  DBotLogout.ImageIndex.ImageType := UI3_wil;
  DBotLogout.ImageIndex.Up := 160;
  DBotLogout.ImageIndex.Hot := 161;
  DBotLogout.ImageIndex.Down := 162;

  DBotGuild.ImageIndex.ImageType := UI3_wil;
  DBotGuild.ImageIndex.Up := 170;
  DBotGuild.ImageIndex.Hot := 171;
  DBotGuild.ImageIndex.Down := 172;

  DBotGroup.ImageIndex.ImageType := UI3_wil;
  DBotGroup.ImageIndex.Up := 190;
  DBotGroup.ImageIndex.Hot := 191;
  DBotGroup.ImageIndex.Down := 192;

  DBotMiniMap.ImageIndex.ImageType := UI3_wil;
  DBotMiniMap.ImageIndex.Up := 200;
  DBotMiniMap.ImageIndex.Hot := 201;
  DBotMiniMap.ImageIndex.Down := 202;

  DBotFunc1.ImageIndex.ImageType := UI3_wil;
  DBotFunc1.ImageIndex.Up := 220;
  DBotFunc1.ImageIndex.Hot := 221;
  DBotFunc1.ImageIndex.Down := 222;

  DBotFunc2.ImageIndex.ImageType := UI3_wil;
  DBotFunc2.ImageIndex.Up := 230;
  DBotFunc2.ImageIndex.Hot := 231;
  DBotFunc2.ImageIndex.Down := 232;

  DBotFunc3.ImageIndex.ImageType := UI3_wil;
  DBotFunc3.ImageIndex.Up := 240;
  DBotFunc3.ImageIndex.Hot := 241;
  DBotFunc3.ImageIndex.Down := 242;

  DBotFunc4.ImageIndex.ImageType := UI3_wil;
  DBotFunc4.ImageIndex.Up := 250;
  DBotFunc4.ImageIndex.Hot := 251;
  DBotFunc4.ImageIndex.Down := 252;

  DBotFunc5.ImageIndex.ImageType := UI3_wil;
  DBotFunc5.ImageIndex.Up := 260;
  DBotFunc5.ImageIndex.Hot := 261;
  DBotFunc5.ImageIndex.Down := 262;

  DBotFunc6.ImageIndex.ImageType := UI3_wil;
  DBotFunc6.ImageIndex.Up := 270;
  DBotFunc6.ImageIndex.Hot := 271;
  DBotFunc6.ImageIndex.Down := 272;

  DChatMemo.PrevImageIndex.ImageType := UI3_wil;
  DChatMemo.PrevImageIndex.Up := 501;
  DChatMemo.PrevImageIndex.Hot := 502;
  DChatMemo.PrevImageIndex.Down := 503;

  DChatMemo.NextImageIndex.ImageType := UI3_wil;
  DChatMemo.NextImageIndex.Up := 504;
  DChatMemo.NextImageIndex.Hot := 505;
  DChatMemo.NextImageIndex.Down := 506;

  DChatMemo.BarImageIndex.ImageType := UI3_wil;
  DChatMemo.BarImageIndex.Up := 507;
  DChatMemo.BarImageIndex.Hot := 508;
  DChatMemo.BarImageIndex.Down := 509;

  DChatMemo.ScrollImageIndex.ImageType := UI3_wil;
  DChatMemo.ScrollImageIndex.Up := 500;
end;

end.
