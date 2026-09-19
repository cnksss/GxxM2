unit HeroWindowsDlg;

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
  THeroWindows = class(TSerialWindows) // Ӣ�۰汾
  private
    {DPrevState: TDxImageButton;
    DNextState: TDxImageButton;

    DHeroPrevState: TDxImageButton;
    DHeroNextState: TDxImageButton;  }
  public
    constructor Create; override;
    destructor Destroy(); override;
    {procedure LoadFromStream(MemoryStream: TMemoryStream); override;
    procedure HeroStateWinDirectPaint(Sender: TObject); override;
    procedure StateMemo4DirectPaint(Sender: TObject); override;
    procedure DPrevStateClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DNextStateClick(Sender: TObject; X, Y: Integer); stdcall;

    procedure DHeroPrevStateClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DHeroNextStateClick(Sender: TObject; X, Y: Integer); stdcall;  }
  end;
implementation
uses
  ClMain,
  MShare,
  SDK;

constructor THeroWindows.Create;
begin
  inherited;
  ClientVersion := cvHero;
end;

destructor THeroWindows.Destroy();
begin
  inherited;
end;

end.
