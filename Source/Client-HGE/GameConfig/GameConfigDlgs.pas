unit GameConfigDlgs;

interface
uses
  Windows,
  Controls,
  SysUtils,
  IniFiles,
  Classes,
  Dialogs,
  GameConfigDlg;

type
  TConfigDlgManage = class
    ConfigDlgList:TStringList;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadPlugIn();
    procedure UnLoadPlugIn();
    procedure Initialize;
    procedure Finalize;
    function GetConfigDlg(ConfigDlgType:TConfigDlgType):TGameConfigObject;
  end;

//function LoadControlFromMemory(Address, Memory:Pointer; Size:Integer; sUiName:string):Integer;

//function LoadControlFromMemory(objRootControl:TObject; Memory:Pointer; Size:Integer; sUiName:string = ''):Integer;
function LoadControlFromStream(ControlAddrList:THashedStringList; streamUI:TStream; sUiName:string = ''):Integer;

var
  ConfigDlgManage:TConfigDlgManage;

implementation
uses
  ClMain,
  MShare,
  SDK,
  DxComponents,
  LoadDxControlEx, //LoadDxControl
  FState,
  JSYConfigDlg,
  MirConfigDlg,
  Grobal2;

//function LoadControlFromMemory(Address, Memory:Pointer; Size:Integer; sUiName:string):Integer;
//begin
//  Result := LoadDxControl.LoadControlFromMemory(Address, Memory, Size, FrmDlg.DBackground, sUiName);
//end;

function LoadControlFromStream(ControlAddrList:THashedStringList; streamUI:TStream; sUiName:string = ''):Integer;
var
    msDefaultUI:TMemoryStream;
begin
    msDefaultUI := LoadDxControlEx.LoadCompressedUIData('MIR_CONFIG_DLG_UI', 'ZDAT'); //必须存在，否则报错
    try
       Result := LoadDxControlEx.LoadControlFromStream(streamUI, FrmDlg.DBackground, ControlAddrList, sUiName);
       LoadDxControlEx.PatchLoadControlFromStream(msDefaultUI, FrmDlg.DBackground, ControlAddrList, sUiName);
    finally
       msDefaultUI.Free;
    end;
end; 

constructor TConfigDlgManage.Create();
begin
  ConfigDlgList := TStringList.Create;
end;

destructor TConfigDlgManage.Destroy;
begin
  UnLoadPlugIn();
  ConfigDlgList.Free;
end;

procedure TConfigDlgManage.LoadPlugIn();
var
  {$IF TESTMODE = 0}
  I:Integer;
  {$IFEND}
  ConfigObject:TGameConfigObject;
begin
  ConfigObject := TJSYConfigDlg.Create;
  ConfigDlgList.AddObject('', ConfigObject);
  {$IF TESTMODE = 0}
  for I := 0 to Length(g_ConfigClient.ClientConfigs) - 1 {44} do begin
    ConfigObject.ConfigCheckeds[TConfigChecked(I)] := g_ConfigClient.ClientConfigs[I];
  end;
  ConfigObject.ConfigCheckeds[ckSceneShake] := g_ConfigClient.ClientConfigs[51];

  {$IFEND}
  case g_ClientVersion of
    cv176,
      cv185,
      cvHero,
      cvSerial,
      cvMirSequel,
      cvMirNewUI205:begin
        ConfigObject := TMirConfigDlg.Create;
        ConfigDlgList.AddObject('', ConfigObject);
        {$IF TESTMODE = 0}
        for I := Low(g_ConfigClient.ClientConfigs) to High(g_ConfigClient.ClientConfigs) do begin
          ConfigObject.ConfigCheckeds[TConfigChecked(I)] := g_ConfigClient.ClientConfigs[I];
        end;
        {$IFEND}
      end;
  end;
end;

procedure TConfigDlgManage.UnLoadPlugIn();
var
  I:Integer;
begin
  for I := 0 to ConfigDlgList.Count - 1 do begin
    TGameConfigObject(ConfigDlgList.Objects[I]).Free;
  end;
  ConfigDlgList.Clear;
end;

procedure TConfigDlgManage.Initialize;
var
  I:Integer;
begin
  for I := 0 to ConfigDlgList.Count - 1 do begin
    TGameConfigObject(ConfigDlgList.Objects[I]).Initialize(frmMain.Handle,
      MakeLong(g_nScreenWidth, g_nScreenHeight), g_ClientVersion, g_boWindowMode);
  end;
end;

procedure TConfigDlgManage.Finalize;
var
  I:Integer;
begin
  for I := 0 to ConfigDlgList.Count - 1 do begin
    if g_ClientConfig.btConfigDlgType = 0 then begin
      if not (TGameConfigObject(ConfigDlgList.Objects[I]) is TJSYConfigDlg) then
        TGameConfigObject(ConfigDlgList.Objects[I]).Finalize;
    end
    else begin
      if TGameConfigObject(ConfigDlgList.Objects[I]) is TJSYConfigDlg then
        TGameConfigObject(ConfigDlgList.Objects[I]).Finalize;
    end;
  end;

  {
  for I := 0 to ConfigDlgList.Count - 1 do
  begin
    TGameConfigObject(ConfigDlgList.Objects[I]).Finalize;
  end;
  }
end;

function TConfigDlgManage.GetConfigDlg(ConfigDlgType:TConfigDlgType):TGameConfigObject;
var
  I:Integer;
  ConfigObject:TGameConfigObject;
begin
  Result := nil;
  for I := 0 to ConfigDlgList.Count - 1 do begin
    ConfigObject := TGameConfigObject(ConfigDlgList.Objects[I]);
    if ConfigObject.ConfigDlgType = ConfigDlgType then begin
      Result := ConfigObject;
      Exit;
    end;
  end;
  if ConfigDlgList.Count > 0 then begin
    Result := TGameConfigObject(ConfigDlgList.Objects[0]);
  end;
end;

initialization
  ConfigDlgManage := TConfigDlgManage.Create;
finalization
  ConfigDlgManage.Free;
end.
