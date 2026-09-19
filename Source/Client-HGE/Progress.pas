unit Progress;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, ExtCtrls, StdCtrls;

type
  TFrmProgress = class(TForm)
    ProgressBar: TProgressBar;
    Timer: TTimer;
    Image: TImage;
    Label1: TLabel;
    LabelMsg: TLabel;
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure TimerTimer(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  FrmProgress: TFrmProgress;

var
  g_boCheckSrceenBitCount: Boolean = False;

implementation

uses MShare, DxComponents;
{$R *.dfm}


procedure TFrmProgress.FormCloseQuery(Sender: TObject;
  var CanClose: Boolean);
begin
  ModalResult := mrOk;
end;

procedure TFrmProgress.TimerTimer(Sender: TObject);
var
  ScreenMode: TDeviceMode;
begin
  if (not g_boCheckSrceenBitCount) and g_boWindowMode and g_ConfigClient.boChangeSrceenBitCount then begin
    g_boCheckSrceenBitCount := True;
    if EnumDisplaySettings(nil, $FFFFFFFF, ScreenMode) then begin
      if ScreenMode.dmBitsPerPel <> 16 then begin
        ZeroMemory(@ScreenMode, SizeOf(ScreenMode)); // 确保内存分配
        with ScreenMode do begin // 填充屏幕数据
          dmSize := SizeOf(ScreenMode); // 确定 Devmode 结构的大小
      // dmPelsWidth  := Width;                    // 所选屏幕宽度
     // dmPelsHeight := Height;                   // 所选屏幕高度
          dmBitsPerPel := 16; // 每象素所选的色彩深度
          dmFields := DM_BITSPERPEL; // 指明有效域
        end;
        if ChangeDisplaySettings(ScreenMode, CDS_FULLSCREEN) <> DISP_CHANGE_SUCCESSFUL then
          ChangeDisplaySettings(ScreenMode, 0);
      end;
    end;
  end;
  // SendGameCenterMsg(CM_CHANGEHANGLE, '0/' + IntToStr(g_MirsClient.nStartIndex));
  ProgressBar.Position := ProgressBar.Position + 1;
  if ProgressBar.Position >= ProgressBar.Max then Close;
end;

procedure TFrmProgress.FormCreate(Sender: TObject);
var
  nIndex: Integer;
  sFileName: string;
  LoadList: TStringList;
begin
  {
  if g_ClientVersion = cvMirReturn then
    sFileName := ExtractFilePath(Application.ExeName) + 'Data\progress3.bmp'
  else
  }
  sFileName := ExtractFilePath(Application.ExeName) + 'Data\progress.bmp';

  if FileExists(sFileName) then begin
    try
      Image.Picture.LoadFromFile(sFileName);
    except

    end;
  end;

  {
  if g_ClientVersion = cvMirReturn then
    sFileName := ExtractFilePath(Application.ExeName) + 'Data\Tips3.dat'
  else
  }
  sFileName := ExtractFilePath(Application.ExeName) + 'Data\Tips.dat';

  if FileExists(sFileName) then begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sFileName);
      for nIndex := LoadList.Count - 1 downto 0 do begin
        if Trim(LoadList[nIndex]) = '' then LoadList.Delete(nIndex);
      end;
    except

    end;
    Randomize;
    nIndex := Random(LoadList.Count - 1);
    if (nIndex >= 0) and (nIndex < LoadList.Count) then begin
      LabelMsg.Caption := LoadList[nIndex];
    end;
    LabelMsg.Left := (Width - LabelMsg.Width) div 2;
    LoadList.Free;
  end;
  Timer.Enabled := True;
end;

end.

