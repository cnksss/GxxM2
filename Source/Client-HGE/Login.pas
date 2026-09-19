unit Login;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Variants,
  Classes,
  Graphics,
  Controls,
  Forms,
  Dialogs,
  StdCtrls,
  ComCtrls,
  IniFiles,
  HUtil32,
  MShare,
  SDK,
  ExtCtrls,
  RzPanel,
  RzRadGrp,
  DxComponents,
  HardInfo;

type
  TGameZone = record
    GameName:string;
    GamePlanFile:string;
    ResourceDir:string;
    ServerIP:string;
    RunGatePassword:string;
    MicroIP:string;
    MicroPassword:string;
    ServerPort:DWORD;
    MicroPort:DWORD;
    MicroMode:Boolean;
    ShowOpenDoor:Boolean;
  end;
  PGameZone = ^TGameZone;

  TFrmLogin = class(TForm)
    GroupBox1:TGroupBox;
    ListView:TListView;
    ButtonStart:TButton;
    ButtonClose:TButton;
    CheckBoxWindowMode:TCheckBox;
    ButtonAdd:TButton;
    ButtonDel:TButton;
    ButtonSave:TButton;
    EditServerAddr:TEdit;
    EditServerPort:TEdit;
    EditServerName:TEdit;
    Label1:TLabel;
    Label2:TLabel;
    Label3:TLabel;
    ComboBoxScreenMode:TComboBox;
    ComboBoxBitCount:TComboBox;
    CheckBoxVSync:TCheckBox;
    CheckBoxD3DFormat:TCheckBox;
    RadioGroup:TRzRadioGroup;
    CheckBoxDepthStencil:TCheckBox;
    CheckBoxHardware:TCheckBox;
    Label4:TLabel;
    EditRunGatePassword:TEdit;
    lblColor:TLabel;
    lblScreenMode:TLabel;
    Label5:TLabel;
    edtMicroIP:TEdit;
    Label6:TLabel;
    edtMicroPort:TEdit;
    chkMicroMode:TCheckBox;
    Label7:TLabel;
    edtMicroPassword:TEdit;
    Label8:TLabel;
    EditResourceDir:TEdit;
    btnUpdatte:TButton;
    GroupBox2:TGroupBox;
    Label9:TLabel;
    edtGamePlanFile:TEdit;
    chkShowOpenDoor:TCheckBox;
    chkCustomUI:TCheckBox;
    lblGameAccount:TLabel;
    lblGameAccountPassword:TLabel;
    edtGameAccountPassword:TEdit;
    cbbGameAccount:TComboBox;
    btnUpdateAccount:TButton;
    btnDelAccount:TButton;
    chkShowPropertyGroupCaption:TCheckBox;
    chkShow1024UI: TCheckBox;
    Label10: TLabel;
    cbbClientMode: TComboBox;
    procedure ButtonStartClick(Sender:TObject);
    procedure ButtonCloseClick(Sender:TObject);
    procedure FormCreate(Sender:TObject);
    procedure ListViewClick(Sender:TObject);
    procedure ButtonDelClick(Sender:TObject);
    procedure ButtonSaveClick(Sender:TObject);
    procedure ButtonAddClick(Sender:TObject);
    procedure FormDestroy(Sender:TObject);
    procedure CheckBoxWindowModeClick(Sender:TObject);
    procedure EditRunGatePasswordChange(Sender:TObject);
    procedure FormClose(Sender:TObject; var Action:TCloseAction);
    procedure btnUpdatteClick(Sender:TObject);
    procedure btnUpdateAccountClick(Sender:TObject);
    procedure btnDelAccountClick(Sender:TObject);
    procedure cbbGameAccountChange(Sender:TObject);
    procedure cbbClientModeChange(Sender: TObject);
  private
    { Private declarations }
    procedure RefGameList;
    procedure LoadGameList;
    procedure UnLoadGameList;
    procedure SaveGameList;
    procedure LoadConfig(psLastAccount:PString = nil);
    procedure SaveConfig;
  private
    m_slstAccount:TStringList;
  protected
    function AddGameAccount(const sAccount, sPassword:string):Boolean;
    procedure RemoveGameAccount(const sAccount:string);
    procedure RemoveAllAccount();
    procedure UpdateGameAccountDropList(sAccount:string = '');
    procedure CreateParams(var Params:TCreateParams); override;
  public
    procedure GetGetAdapterModeCount;
  end;

var
  FrmLogin:TFrmLogin;
  g_GameList:TList;
implementation
uses GameImages;
{$R *.dfm}
// TDevMode = TDeviceMode; //TDeviceMode;是个结构类型

procedure TFrmLogin.GetGetAdapterModeCount;
var
  I:Integer;
  DevMode:TDevMode;
  Done:Boolean;
  ModeNum:Integer;
  boFind:Boolean;
begin
  {
  900*600 39322500
  800*600 39322400
  1024*768 50332672
  }
  ComboBoxScreenMode.Items.Clear;
  ComboBoxScreenMode.Items.AddObject(Format('%d*%d', [900, 600]),
    TObject(MakeLong(900, 600)));
  ModeNum := 0; // 图形模式索引值从0开始
  ZeroMemory(@DevMode, SizeOf(TDevMode)); // 确保内存分配
  DevMode.dmSize := System.SizeOf(TDevMode); // 先要对dmSize进行初始化(MSDN上的解释)
  // EnumDisplaySettings:枚举当前显卡所支持的所有图形模式
  Done := Windows.EnumDisplaySettings(nil, ModeNum, DevMode);
  // nil:表示了呼叫当前线程上的计算机使用的显示设备(即屏幕)
  while Done do begin
    with DevMode do begin
      // DevMode.dmPelsWidth:水平分辨率(单位:点数)
      // DevMode.dmPelsHeight垂直分辨率(单位:点数)
      // DevMode.dmDisplayFrequency:屏幕的刷新频率
      // DevMode.dmBitsPerPel:显示设备的颜色分辨率(单位:位数);4位16色,8位256色,16位增强色,32位真彩色
     { case dmBitsPerPel of
        4:  s:='16 色'; // 2^4
        8:  s:='256 色'; // 2^8
        16: s:='增强色 (16位)'; // 2^16
        32: s:='真彩色 (32位)';// 2^32
      end;}
     // L//istBox1.Items.Add(Format('%d × %d，%s ，%d 赫兹',[dmPelsWidth,dmPelsHeight,s,dmDisplayFrequency]));
      boFind := False;
      for I := 0 to ComboBoxScreenMode.Items.Count - 1 do begin
        if Integer(ComboBoxScreenMode.Items.Objects[I]) = MakeLong(dmPelsWidth,
          dmPelsHeight) then begin
          boFind := True;
          break;
        end;
      end;
      if not boFind then
        ComboBoxScreenMode.Items.AddObject(Format('%d*%d', [dmPelsWidth,
          dmPelsHeight]), TObject(MakeLong(dmPelsWidth, dmPelsHeight)));
    end;
    System.Inc(ModeNum);
    ZeroMemory(@DevMode, SizeOf(TDevMode)); // 确保内存分配
    DevMode.dmSize := System.SizeOf(TDevMode);
    Done := Windows.EnumDisplaySettings(nil, ModeNum, DevMode);
  end;

  if ComboBoxScreenMode.ItemIndex < 0 then begin // 800*600 39322400
    for I := 0 to ComboBoxScreenMode.Items.Count - 1 do begin
      if Integer(ComboBoxScreenMode.Items.Objects[I]) = MakeLong(g_nScreenWidth,
        g_nScreenHeight) then begin
        ComboBoxScreenMode.ItemIndex := I;
        break;
      end;
    end;
  end;

  if ComboBoxScreenMode.ItemIndex < 0 then begin // 800*600 39322400
    for I := 0 to ComboBoxScreenMode.Items.Count - 1 do begin
      if Integer(ComboBoxScreenMode.Items.Objects[I]) = 39322400 then begin
        ComboBoxScreenMode.ItemIndex := I;
        break;
      end;
    end;
  end;

  if ComboBoxScreenMode.ItemIndex < 0 then begin // 1024*768 50332672
    for I := 0 to ComboBoxScreenMode.Items.Count - 1 do begin
      if Integer(ComboBoxScreenMode.Items.Objects[I]) = 50332672 then begin
        ComboBoxScreenMode.ItemIndex := I;
        break;
      end;
    end;

  end;

  if ComboBoxScreenMode.ItemIndex < 0 then begin // 900*600 39322500
    for I := 0 to ComboBoxScreenMode.Items.Count - 1 do begin
      if Integer(ComboBoxScreenMode.Items.Objects[I]) = 39322500 then begin
        ComboBoxScreenMode.ItemIndex := I;
        break;
      end;
    end;
  end;
end;

procedure TFrmLogin.ButtonStartClick(Sender:TObject);
var
  ListItem:TListItem;
  GameZone:PGameZone;
begin
  ListItem := ListView.Selected;
  if ListItem <> nil then begin
    GameZone := PGameZone(ListItem.Data);
    g_ResourcesDir := GameZone.ResourceDir; //资源目录
    g_sSelfResourcePath := IncludeTrailingPathDelimiter(g_sSelfFilePath + g_ResourcesDir);

    {$IF TESTMODE = 1}
    g_TestModeResourceDir := GameZone.ResourceDir; //添加Resource目录
    g_TestModeGamePlanFile := GameZone.GamePlanFile;
    g_TestModeShowOpenDoor := GameZone.ShowOpenDoor;
    g_TestModeUseCustomUI := chkCustomUI.Checked;
    g_TestModeShowPropertyGroupCption := chkShowPropertyGroupCaption.Checked;
    g_TestModeShow1024UI := chkShow1024UI.Checked;

    g_TestDefaultAccount := cbbGameAccount.Text;
    g_TestDefaultAccountPassword := edtGameAccountPassword.Text;
    g_nUcGameId := 1;
    {$IFEND}

    g_sServerAddr := GameZone.ServerIP;
    g_nServerPort := GameZone.ServerPort;

    g_boWindowMode := CheckBoxWindowMode.Checked;
    g_sRunGatePassword := GameZone.RunGatePassword;

    if GameZone.MicroMode then begin
      g_boAutoUpdate := True;
      g_sUpdateAddr := GameZone.MicroIP;
      g_nUpdatePort := GameZone.MicroPort;

      g_sUpdateGateAddr := GameZone.MicroIP;
      g_nUpdateGatePort := GameZone.MicroPort;

      g_sUpdatePassword := GameZone.MicroPassword;
    end else begin
      g_boAutoUpdate := False;
    end;

    ButtonStart.ModalResult := mrOk;
    g_nScreenWidth := LoWord(Integer(ComboBoxScreenMode.Items.Objects[ComboBoxScreenMode.ItemIndex]));
    g_nScreenHeight := HiWord(Integer(ComboBoxScreenMode.Items.Objects[ComboBoxScreenMode.ItemIndex]));

    if ComboBoxScreenMode.ItemIndex = 5 then g_ConfigClient.boShow1024 := True;

    if RadioGroup.ItemIndex < 0 then RadioGroup.ItemIndex := 6;
    g_ClientVersion := TClientVersion(RadioGroup.ItemIndex);
    g_boVSync := CheckBoxVSync.Checked;
    g_boD3DFormat := CheckBoxD3DFormat.Checked;
    if ComboBoxBitCount.ItemIndex = 0 then
      g_nBitCount := 32
    else
      g_nBitCount := 16;

    g_boDepthStencil := CheckBoxDepthStencil.Checked;
    g_boHardware := CheckBoxHardware.Checked;
    g_nClientLoginMode := cbbClientMode.ItemIndex;
    if not g_nClientLoginMode in [0,1,2] then g_nClientLoginMode := 0;
    

    SaveConfig;
    Close;
  end else begin
    Application.MessageBox('请选择你要登陆的服务器！', '提示信息', MB_OK + MB_ICONINFORMATION);
  end;
end;

procedure TFrmLogin.cbbClientModeChange(Sender: TObject);
begin
    g_nClientLoginMode := cbbClientMode.ItemIndex;
end;

procedure TFrmLogin.cbbGameAccountChange(Sender:TObject);
var
  sAccount:string;
begin
  sAccount := cbbGameAccount.Text;
  if sAccount <> '' then begin
    edtGameAccountPassword.Text := m_slstAccount.Values[sAccount];
  end;
end;

procedure TFrmLogin.ButtonCloseClick(Sender:TObject);
begin
  ButtonStart.ModalResult := mrNone;
  ButtonClose.ModalResult := mrOk;
  Close;
end;

procedure TFrmLogin.RefGameList;
var
  I, nSelectedIndex:Integer;
  GameZone:PGameZone;
  ListItem:TListItem;
begin
  nSelectedIndex := ListView.ItemIndex;
  ListView.Items.Clear;
  for I := 0 to g_GameList.Count - 1 do begin
    GameZone := g_GameList.Items[I];
    ListItem := ListView.Items.Add;
    ListItem.Data := GameZone;
    ListItem.Caption := GameZone.GameName;
    ListItem.SubItems.Add(GameZone.ServerIP);
    ListItem.SubItems.Add(IntToStr(GameZone.ServerPort));
    ListItem.SubItems.Add(GameZone.RunGatePassword);
  end;

  if ListView.Items.Count > nSelectedIndex then begin
      ListView.ItemIndex := nSelectedIndex;
  end;
end;

procedure TFrmLogin.LoadGameList;
var
  I:Integer;
  IniFile:TIniFile;
  StringList:TStringList;
  GameZone:PGameZone;
  sSection, sGameName, sServerIP, sMicroIp, sResDir, sGamePlanFile, sRunGatePassword, sMicroPassword:string;
  dwServerPort, dwMicroPort:DWORD;
  bMicroMode, bShowOpenDoor:Boolean;
begin
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'ServerList.ini');
  StringList := TStringList.Create;
  IniFile.ReadSections(StringList);
  for I := 0 to StringList.Count - 1 do begin
    sSection := StringList.Strings[I];
    sGameName := IniFile.ReadString(sSection, 'ServerName', '');
    sGamePlanFile := IniFile.ReadString(sSection, 'GamePlanFile', 'Newopui.pak');
    sResDir := IniFile.ReadString(sSection, 'ResourceDir', 'Resources');
    sServerIP := IniFile.ReadString(sSection, 'ServerAddr', '');
    sRunGatePassword := IniFile.ReadString(sSection, 'RunGatePassword', 'GxxM2');
    dwServerPort := IniFile.ReadInteger(sSection, 'ServerPort', 7000);
    sMicroIP := IniFile.ReadString(sSection, 'MicroAddr', '');
    sMicroPassword := IniFile.ReadString(sSection, 'MicroPassword', 'GxxM2');
    dwMicroPort := IniFile.ReadInteger(sSection, 'MicroPort', 0);
    bMicroMode := IniFile.ReadBool(sSection, 'MicroMode', False);
    bShowOpenDoor := IniFile.ReadBool(sSection, 'ShowOpenDoor', True);

    if (sGameName <> '') and (sServerIP <> '') and (dwServerPort <> 0) then begin
      GameZone := New(PGameZone);
      GameZone.GameName := sGameName;
      GameZone.GamePlanFile := sGamePlanFile;
      GameZone.ResourceDir := sResDir;
      GameZone.ServerIP := sServerIP;
      GameZone.RunGatePassword := sRunGatePassword;
      GameZone.ServerPort := dwServerPort;
      GameZone.MicroIP := sMicroIp;
      GameZone.MicroPassword := sMicroPassword;
      GameZone.MicroPort := dwMicroPort;
      GameZone.MicroMode := bMicroMode;
      GameZone.ShowOpenDoor := bShowOpenDoor;
      g_GameList.Add(GameZone);
    end;
  end;
  StringList.Free;
  IniFile.Free;
  if g_GameList.Count = 0 then begin
    New(GameZone);
    GameZone.GameName := 'GxxM2';
    GameZone.GamePlanFile := 'Newopui.pak';
    GameZone.ResourceDir := 'Resources';
    GameZone.ServerIP := '127.0.0.1';
    GameZone.RunGatePassword := 'GxxM2';
    GameZone.ServerPort := 7000;
    GameZone.MicroIP := '';
    GameZone.MicroPassword := 'GxxM2';
    GameZone.MicroPort := 0;
    GameZone.MicroMode := False;
    GameZone.ShowOpenDoor := True;
    g_GameList.Add(GameZone);
  end;
end;

procedure TFrmLogin.UnLoadGameList;
var
  I:Integer;
begin
  for I := 0 to g_GameList.Count - 1 do begin
    Dispose(PGameZone(g_GameList.Items[I]));
  end;
  g_GameList.Clear;
end;

procedure TFrmLogin.UpdateGameAccountDropList(sAccount:string);
var
  sName:string;
  i, sDefIndex:Integer;
begin
  cbbGameAccount.Clear;
  if m_slstAccount.Count > 0 then begin
    sDefIndex := 0;
    cbbGameAccount.Items.BeginUpdate;
    for i := 0 to m_slstAccount.Count - 1 do begin
      sName := m_slstAccount.Names[i];
      cbbGameAccount.Items.Add(sName);
      if SameText(sAccount, sName) then begin
        sDefIndex := i;
      end;
    end;
    cbbGameAccount.Items.EndUpdate;

    cbbGameAccount.ItemIndex := sDefIndex;
    edtGameAccountPassword.Text := m_slstAccount.ValueFromIndex[sDefIndex];
  end;
end;

procedure TFrmLogin.SaveGameList;
var
  I:Integer;
  IniFile:TIniFile;
  GameZone:PGameZone;
  sSection:string;
  slstSections:TStringList;
begin
  //DeleteFile(ExtractFilePath(Application.ExeName) + '\ServerList.ini');
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'ServerList.ini');
  try
    slstSections := TStringList.Create;
    IniFile.ReadSections(slstSections);
    for i := 0 to slstSections.Count - 1 do begin
      IniFile.EraseSection(slstSections[i]);
    end;
    slstSections.Free;

    for I := 0 to g_GameList.Count - 1 do begin
      sSection := IntToStr(i);
      GameZone := g_GameList.Items[I];
      IniFile.WriteString(sSection, 'ServerName', GameZone.GameName);
      IniFile.WriteString(sSection, 'GamePlanFile', GameZone.GamePlanFile);
      IniFile.WriteString(sSection, 'ResourceDir', GameZone.ResourceDir);
      IniFile.WriteString(sSection, 'ServerAddr', GameZone.ServerIP);
      IniFile.WriteString(sSection, 'RunGatePassword', GameZone.RunGatePassword);
      IniFile.WriteInteger(sSection, 'ServerPort', GameZone.ServerPort);
      IniFile.WriteString(sSection, 'MicroAddr', GameZone.MicroIP);
      IniFile.WriteString(sSection, 'MicroPassword', GameZone.MicroPassword);
      IniFile.WriteInteger(sSection, 'MicroPort', GameZone.MicroPort);
      IniFile.WriteBool(sSection, 'MicroMode', GameZone.MicroMode);
      IniFile.WriteBool(sSection, 'ShowOpenDoor', GameZone.ShowOpenDoor);
    end;
  finally
    IniFile.Free;
  end;
end;

function TFrmLogin.AddGameAccount(const sAccount, sPassword:string):Boolean;
var
  nIndex:Integer;
begin
  nIndex := m_slstAccount.IndexOfName(sAccount);
  if nIndex >= 0 then begin
    m_slstAccount[nIndex] := Format('%s=%s', [sAccount, sPassword]);
    Result := False;
  end else begin
    m_slstAccount.Add(Format('%s=%s', [sAccount, sPassword]));
    Result := True;
  end;
end;

procedure TFrmLogin.RemoveGameAccount(const sAccount:string);
var
  nIndex:Integer;
begin
  nIndex := m_slstAccount.IndexOfName(sAccount);
  if nIndex >= 0 then begin
    m_slstAccount.Delete(nIndex);
  end;
end;

procedure TFrmLogin.RemoveAllAccount();
begin
  m_slstAccount.Clear;
end;

procedure TFrmLogin.LoadConfig(psLastAccount:PString);
var
  IniFile:TIniFile;
  i:Integer;
  sAccount, sPassword:string;
begin
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'Config.ini');
  try
    m_slstAccount.Clear;
    IniFile.ReadSection('GameAccount', m_slstAccount); //读取账号
    for i := 0 to m_slstAccount.Count - 1 do begin
      sAccount := m_slstAccount[i];
      sPassword := IniFile.ReadString('GameAccount', sAccount, '');
      m_slstAccount[i] := Format('%s=%s', [sAccount, sPassword]);
    end;

    if psLastAccount <> nil then begin
        psLastAccount^ := IniFile.ReadString('LastGameAccount', 'Account', '');
    end;

    g_boWindowMode := IniFile.ReadBool('Setup', 'WindowMode', True);
    g_nScreenWidth := IniFile.ReadInteger('Setup', 'ScreenWidth', g_nScreenWidth);
    g_nScreenHeight := IniFile.ReadInteger('Setup', 'ScreenHeight', g_nScreenHeight);
    g_ClientVersion := TClientVersion(IniFile.ReadInteger('Setup', 'ClientVersion', Integer(g_ClientVersion)));
    g_nClientLoginMode := IniFile.ReadInteger('Setup', 'ClientMode', g_nClientLoginMode);
    
    g_boVSync := IniFile.ReadBool('Setup', 'VSync', g_boVSync);

    g_boD3DFormat := IniFile.ReadBool('Setup', 'D3DFormat', g_boD3DFormat);
    g_nBitCount := IniFile.ReadInteger('Setup', 'BitCount', g_nBitCount);
    g_sRunGatePassword := IniFile.ReadString('Setup', 'RunGatePassword',
      g_sRunGatePassword);
    g_boDepthStencil := IniFile.ReadBool('Setup', 'DepthStencil', g_boDepthStencil);
    g_boHardware := IniFile.ReadBool('Setup', 'DepthStencil', g_boHardware);

    {$IF TESTMODE = 1}
    g_TestModeUseCustomUI := IniFile.ReadBool('Setup', 'UseCustomUI', True);
    g_TestModeShowPropertyGroupCption := IniFile.ReadBool('Setup', 'ShowProperGroupCation', True);
    g_TestModeShow1024UI := IniFile.ReadBool('Setup', 'Show1024UI', False);
    if m_slstAccount.Count > 0 then begin
      g_TestDefaultAccount := m_slstAccount.Names[0];
      g_TestDefaultAccountPassword := m_slstAccount.ValueFromIndex[0];
    end;
    {$IFEND}

  finally
    IniFile.Free;
  end;

  GetGetAdapterModeCount;
  RadioGroup.ItemIndex := Integer(g_ClientVersion);
  CheckBoxWindowMode.Checked := g_boWindowMode;
  // ComboBoxScreenMode.ItemIndex := g_nScreenMode;
  ComboBoxBitCount.ItemIndex := (g_nBitCount - 16) div 16;
  cbbClientMode.ItemIndex := g_nClientLoginMode;
  CheckBoxVSync.Checked := g_boVSync;
  CheckBoxD3DFormat.Checked := g_boD3DFormat;
  CheckBoxHardware.Checked := g_boHardware;
  CheckBoxDepthStencil.Checked := g_boDepthStencil;

  {$IF TESTMODE = 1}
  chkCustomUI.Checked := g_TestModeUseCustomUI;
  chkShowPropertyGroupCaption.Checked := g_TestModeShowPropertyGroupCption;
  chkShow1024UI.Checked := g_TestModeShow1024UI;
  cbbGameAccount.Text := g_TestDefaultAccount;
  edtGameAccountPassword.Text := g_TestDefaultAccountPassword;
  {$IFEND}
end;

procedure TFrmLogin.SaveConfig;
var
  i:Integer;
  IniFile:TIniFile;
begin
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'Config.ini');

  {$IF TESTMODE = 1}
  g_TestModeUseCustomUI := chkCustomUI.Checked;
  g_TestModeShowPropertyGroupCption := chkShowPropertyGroupCaption.Checked;
  {$IFEND}

  try
    IniFile.WriteBool('Setup', 'WindowMode', g_boWindowMode);
    IniFile.WriteInteger('Setup', 'ScreenWidth', g_nScreenWidth);
    IniFile.WriteInteger('Setup', 'ScreenHeight', g_nScreenHeight);
    IniFile.WriteInteger('Setup', 'ClientVersion', Integer(g_ClientVersion));

    IniFile.WriteBool('Setup', 'VSync', g_boVSync);

    IniFile.WriteBool('Setup', 'D3DFormat', g_boD3DFormat);
    IniFile.WriteInteger('Setup', 'BitCount', g_nBitCount);
    IniFile.WriteString('Setup', 'RunGatePassword', g_sRunGatePassword);
    IniFile.WriteBool('Setup', 'Hardware', g_boHardware);
    IniFile.WriteBool('Setup', 'DepthStencil', g_boDepthStencil);

    IniFile.WriteInteger('Setup', 'ClientMode', g_nClientLoginMode);

    {$IF TESTMODE = 1}
    IniFile.WriteBool('Setup', 'UseCustomUI', g_TestModeUseCustomUI);
    IniFile.WriteBool('Setup', 'ShowProperGroupCation', g_TestModeShowPropertyGroupCption);
    IniFile.WriteBool('Setup', 'Show1024UI', g_TestModeShow1024UI);
    {$IFEND}

    IniFile.EraseSection('GameAccount');
    for i := 0 to m_slstAccount.Count - 1 do begin
      IniFile.WriteString('GameAccount', m_slstAccount.Names[i], m_slstAccount.ValueFromIndex[i]);
    end;

    if cbbGameAccount.ItemIndex >= 0 then begin
        IniFile.WriteString('LastGameAccount', 'Account', Trim(cbbGameAccount.Items[cbbGameAccount.ItemIndex]));
    end else begin
        IniFile.EraseSection('LastGameAccount');
    end;

  finally // piaoyun 2013-5-26
    IniFile.Free;
  end;
end;

procedure TFrmLogin.FormClose(Sender:TObject; var Action:TCloseAction);
begin
  SaveConfig;
  SaveGameList();
end;

procedure TFrmLogin.FormCreate(Sender:TObject);
var
   nIndex:Integer;
   sLastAccount:string;
begin
  g_GameList := TList.Create;
  m_slstAccount := TStringList.Create;
  m_slstAccount.NameValueSeparator := '=';

  LoadConfig(@sLastAccount);
  LoadGameList;
  RefGameList;
  GetGetAdapterModeCount;
  if ListView.Items.Count > 0 then
    ListView.itemindex := 0;

  ListViewClick(nil);

  UpdateGameAccountDropList();

  if sLastAccount <> '' then begin
      nIndex := cbbGameAccount.Items.IndexOf(sLastAccount);
      if nIndex >= 0 then begin
         cbbGameAccount.ItemIndex := nIndex;
      end;
  end;

  edtGameAccountPassword.Text := m_slstAccount.ValueFromIndex[cbbGameAccount.ItemIndex];

  //ButtonStart.SetFocus;
end;

procedure TFrmLogin.ListViewClick(Sender:TObject);
var
  ListItem:TListItem;
  GameZone:PGameZone;
begin
  ListItem := ListView.Selected;
  if ListItem <> nil then begin
    GameZone := PGameZone(ListItem.Data);
    EditServerName.Text := GameZone.GameName;
    EditResourceDir.Text := GameZone.ResourceDir;
    EditServerAddr.Text := GameZone.ServerIP;
    EditRunGatePassword.Text := GameZone.RunGatePassword;
    EditServerPort.Text := IntToStr(GameZone.ServerPort);

    edtGamePlanFile.Text := GameZone.GamePlanFile;
    edtMicroIP.Text := GameZone.MicroIP;
    edtMicroPassword.Text := GameZone.MicroPassword;
    edtMicroPort.Text := IntToStr(GameZone.MicroPort);
    chkMicroMode.Checked := GameZone.MicroMode;
    chkShowOpenDoor.Checked := GameZone.ShowOpenDoor;
  end;
end;

procedure TFrmLogin.ButtonDelClick(Sender:TObject);
var
  I:Integer;
  ListItem:TListItem;
  GameZone:PGameZone;
begin
  ListItem := ListView.Selected;
  if ListItem <> nil then begin
    GameZone := PGameZone(ListItem.Data);
    ListView.DeleteSelected;
    for I := 0 to g_GameList.Count - 1 do begin
      if GameZone = g_GameList.Items[I] then begin
        Dispose(PGameZone(g_GameList.Items[I]));
        g_GameList.Delete(I);
        RefGameList;
        Break;
      end;
    end;
  end;
  SaveGameList;
end;

procedure TFrmLogin.ButtonSaveClick(Sender:TObject);
begin
  SaveGameList;
end;

procedure TFrmLogin.btnDelAccountClick(Sender:TObject);
var
  sAccount:string;
  nIndex:Integer;
begin
  sAccount := Trim(cbbGameAccount.Text);
  if sAccount <> EmptyStr then begin
    nIndex := m_slstAccount.IndexOfName(sAccount);
    if nIndex >= 0 then begin
      m_slstAccount.Delete(nIndex);
      UpdateGameAccountDropList();
    end;
  end;
end;

procedure TFrmLogin.btnUpdateAccountClick(Sender:TObject);
var
  sAccount:string;
begin
  sAccount := Trim(cbbGameAccount.Text);
  if sAccount <> EmptyStr then begin
    if AddGameAccount(sAccount, Trim(edtGameAccountPassword.Text)) then begin
      UpdateGameAccountDropList(sAccount);
    end;
  end;
end;

procedure TFrmLogin.btnUpdatteClick(Sender:TObject);
var
  ListItem:TListItem;
  GameZone:PGameZone;
  sGameName, sServerIP, sMicroIP, sResourceDir, sGamePlanFile, sRunGatePassword, sMicroPassword:string;
  dwServerPort, dwMicroPort:DWORD;
  bMicroMode, bShowOpenDoor:Boolean;
begin
  ListItem := ListView.Selected;
  if ListItem = nil then exit;

  GameZone := ListItem.Data;

  sGameName := Trim(EditServerName.Text);
  sGamePlanFile := Trim(edtGamePlanFile.Text);
  sResourceDir := Trim(EditResourceDir.Text);
  sServerIP := Trim(EditServerAddr.Text);
  sRunGatePassword := Trim(EditRunGatePassword.Text);
  dwServerPort := Str_ToInt(Trim(EditServerPort.Text), 0);
  sMicroIP := Trim(edtMicroIP.Text);
  sMicroPassword := Trim(edtMicroPassword.Text);
  dwMicroPort := Str_ToInt(Trim(edtMicroPort.Text), 0);
  bMicroMode := chkMicroMode.Checked;
  bShowOpenDoor := chkShowOpenDoor.Checked;

  if sGameName = '' then begin
    Application.MessageBox('服务器名称，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditServerName.SetFocus;
    Exit;
  end;

  if sServerIp = '' then begin
    Application.MessageBox('服务器地址，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditServerAddr.SetFocus;
    Exit;
  end;

  if sRunGatePassword = '' then begin
    Application.MessageBox('登录密码，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditRunGatePassword.SetFocus;
    Exit;
  end;

  if (dwServerPort = 0) or (dwServerPort > 65535) then begin
    Application.MessageBox('服务器端口，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditServerPort.SetFocus;
    Exit;
  end;

  if sGamePlanFile = '' then begin
    Application.MessageBox('必备补丁，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    edtGamePlanFile.SetFocus;
    Exit;
  end;

  if bMicroMode then begin
    if sMicroIp = '' then begin
      Application.MessageBox('微端地址，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
      edtMicroIP.SetFocus;
      Exit;
    end;

    if sMicroPassword = '' then begin
      Application.MessageBox('微端密码，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
      edtMicroPassword.SetFocus;
      Exit;
    end;

    if (dwMicroPort = 0) or (dwServerPort > 65535) then begin
      Application.MessageBox('微端端口，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
      edtMicroPort.SetFocus;
      Exit;
    end;
  end;

  GameZone.GameName := sGameName;
  GameZone.GamePlanFile := sGamePlanFile;
  GameZone.ResourceDir := sResourceDir;
  GameZone.ServerIP := sServerIp;
  GameZone.RunGatePassword := sRunGatePassword;
  GameZone.ServerPort := dwServerPort;
  GameZone.MicroIP := sMicroIP;
  GameZone.MicroPassword := sMicroPassword;
  GameZone.MicroPort := dwMicroPort;
  GameZone.MicroMode := bMicroMode;
  GameZone.ShowOpenDoor := bShowOpenDoor;
  RefGameList;

  SaveGameList();

end;

procedure TFrmLogin.ButtonAddClick(Sender:TObject);
var
  I:Integer;
  GameZone:PGameZone;
  sGameName, sServerIP, sMicroIP, sResourceDir, sGamePlanFile, sRunGatePassword, sMicroPassword:string;
  dwServerPort, dwMicroPort:DWORD;
  bMicroMode, bShowOpenDoor:Boolean;
begin
  sGameName := Trim(EditServerName.Text);
  sGamePlanFile := Trim(edtGamePlanFile.Text);
  sResourceDir := Trim(EditResourceDir.Text);
  sServerIP := Trim(EditServerAddr.Text);
  sRunGatePassword := Trim(EditRunGatePassword.Text);
  dwServerPort := Str_ToInt(Trim(EditServerPort.Text), 0);
  sMicroIP := Trim(edtMicroIP.Text);
  sMicroPassword := Trim(edtMicroPassword.Text);
  dwMicroPort := Str_ToInt(Trim(edtMicroPort.Text), 0);
  bMicroMode := chkMicroMode.Checked;
  bShowOpenDoor := chkShowOpenDoor.Checked;

  if sGameName = '' then begin
    Application.MessageBox('服务器名称，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditServerName.SetFocus;
    Exit;
  end;

  if sGamePlanFile = '' then begin
    Application.MessageBox('必备补丁，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    edtGamePlanFile.SetFocus;
    Exit;
  end;

  for I := 0 to g_GameList.Count - 1 do begin
    GameZone := g_GameList.Items[I];
    if (GameZone.GameName = sGameName) and (GameZone.ServerIP = sServerIP) then begin
      Application.MessageBox('该服务器已经存在！', '提示信息', MB_OK + MB_ICONINFORMATION);
      EditServerName.SetFocus;
      Exit;
    end;
  end;

  if sServerIp = '' then begin
    Application.MessageBox('服务器地址，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditServerAddr.SetFocus;
    Exit;
  end;

  if sRunGatePassword = '' then begin
    Application.MessageBox('登录密码，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditRunGatePassword.SetFocus;
    Exit;
  end;

  if (dwServerPort = 0) or (dwServerPort > 65535) then begin
    Application.MessageBox('服务器端口，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    EditServerPort.SetFocus;
    Exit;
  end;

  if bMicroMode then begin
    if sMicroIp = '' then begin
      Application.MessageBox('微端地址，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
      edtMicroIP.SetFocus;
      Exit;
    end;

    if sMicroPassword = '' then begin
      Application.MessageBox('微端密码，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
      edtMicroPassword.SetFocus;
      Exit;
    end;

    if (dwMicroPort = 0) or (dwServerPort > 65535) then begin
      Application.MessageBox('微端端口，输入不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
      edtMicroPort.SetFocus;
      Exit;
    end;
  end;

  New(GameZone);
  GameZone.GameName := sGameName;
  GameZone.GamePlanFile := sGamePlanFile;
  GameZone.ResourceDir := sResourceDir;
  GameZone.ServerIP := sServerIp;
  GameZone.RunGatePassword := sRunGatePassword;
  GameZone.ServerPort := dwServerPort;
  GameZone.MicroIP := sMicroIP;
  GameZone.MicroPassword := sMicroPassword;
  GameZone.MicroPort := dwMicroPort;
  GameZone.MicroMode := bMicroMode;
  GameZone.ShowOpenDoor := bShowOpenDoor;
  g_GameList.Add(GameZone);
  RefGameList;

  SaveGameList();
end;

procedure TFrmLogin.FormDestroy(Sender:TObject);
begin
  UnLoadGameList;
  g_GameList.Free;
  m_slstAccount.Free;
end;

procedure TFrmLogin.CheckBoxWindowModeClick(Sender:TObject);
begin
  g_boWindowMode := CheckBoxWindowMode.Checked;
end;

procedure TFrmLogin.CreateParams(var Params:TCreateParams);
begin
  inherited;
  with Params do begin
    ExStyle := ExStyle or WS_EX_APPWINDOW;
    WndParent := GetDesktopwindow;
  end;
end;

procedure TFrmLogin.EditRunGatePasswordChange(Sender:TObject);
begin
  g_sRunGatePassword := EditRunGatePassword.Text;
end;

end.
