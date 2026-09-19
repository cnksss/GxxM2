unit GMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, StdCtrls, INIFiles, ExtCtrls, DataBackUp,
  Spin, JSocket, RzSpnEdt, Mask, RzEdit, RzBtnEdt, ShlObj, ActiveX, ShellApi,
  Common, DBTables, DB, ComObj, ADODB, RzPanel, RzRadGrp, StrUtils, Buttons,
  RzButton, pngimage, SQLite3DataBase, MySQLDataBase, MySQLCli, MySQLWrap,
  uSqliteDB, SpinEditEx, MySqlCreateTableSql, DateUtils;

{$R uac.res}

const
sProgramName = '引擎控制台';

type
  TfrmMain = class(TForm)
    PageControl1: TPageControl;

    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    PageControl2: TPageControl;
    PageControl3: TPageControl;
    TabSheet4: TTabSheet;
    TabSheet5: TTabSheet;
    TabSheet6: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    EditGameDir: TEdit;
    ButtonNext1: TButton;

    ButtonNext2: TButton;
    GroupBox2: TGroupBox;
    ButtonPrv2: TButton;
    EditGameName: TEdit;
    Label3: TLabel;
    Label4: TLabel;
    EditGameExtIPaddr: TEdit;
    GroupBox5: TGroupBox;
    ButtonStartGame: TButton;
    CheckBoxM2Server: TCheckBox;
    CheckBoxDBServer: TCheckBox;
    CheckBoxLoginServer: TCheckBox;
    CheckBoxLogServer: TCheckBox;
    CheckBoxLoginGate: TCheckBox;
    CheckBoxSelGate: TCheckBox;
    CheckBoxRunGate: TCheckBox;
    CheckBoxRunGate1: TCheckBox;
    CheckBoxRunGate2: TCheckBox;
    TimerStartGame: TTimer;
    TimerStopGame: TTimer;
    TimerCheckRun: TTimer;
    MemoLog: TMemo;
    ButtonReLoadConfig: TButton;
    GroupBox3: TGroupBox;
    GroupBox8: TGroupBox;
    Label11: TLabel;
    Label12: TLabel;
    EditSelGate_MainFormX: TSpinEdit;
    EditSelGate_MainFormY: TSpinEdit;
    TabSheet7: TTabSheet;
    GroupBox9: TGroupBox;
    GroupBox10: TGroupBox;
    Label13: TLabel;
    Label14: TLabel;
    EditLoginServer_MainFormX: TSpinEdit;
    EditLoginServer_MainFormY: TSpinEdit;
    TabSheet8: TTabSheet;
    GroupBox11: TGroupBox;
    GroupBox12: TGroupBox;
    Label15: TLabel;
    Label16: TLabel;
    EditDBServer_MainFormX: TSpinEdit;
    EditDBServer_MainFormY: TSpinEdit;
    TabSheet9: TTabSheet;
    GroupBox13: TGroupBox;
    GroupBox14: TGroupBox;
    Label17: TLabel;
    Label18: TLabel;
    EditLogServer_MainFormX: TSpinEdit;
    EditLogServer_MainFormY: TSpinEdit;
    TabSheet10: TTabSheet;
    GroupBox15: TGroupBox;
    GroupBox16: TGroupBox;
    Label19: TLabel;
    Label20: TLabel;
    EditM2Server_MainFormX: TSpinEdit;
    EditM2Server_MainFormY: TSpinEdit;
    TabSheet11: TTabSheet;
    ButtonSave: TButton;
    ButtonGenGameConfig: TButton;
    ButtonPrv3: TButton;
    ButtonNext3: TButton;
    TabSheet12: TTabSheet;
    ButtonPrv4: TButton;
    ButtonNext4: TButton;
    ButtonPrv5: TButton;
    ButtonNext5: TButton;
    ButtonPrv6: TButton;
    ButtonNext6: TButton;
    ButtonPrv7: TButton;
    ButtonNext7: TButton;
    ButtonPrv8: TButton;
    ButtonNext8: TButton;
    ButtonPrv9: TButton;
    GroupBox17: TGroupBox;
    GroupBox18: TGroupBox;
    Label21: TLabel;
    Label22: TLabel;
    EditRunGate_MainFormX: TSpinEdit;
    EditRunGate_MainFormY: TSpinEdit;
    GroupBox19: TGroupBox;
    Label23: TLabel;
    EditRunGate_Connt: TSpinEdit;
    TabSheet13: TTabSheet;
    ButtonLoginServerConfig: TButton;
    chkDoubleLineMode: TCheckBox;
    ServerSocket: TServerSocket;
    Timer: TTimer;
    GroupBox22: TGroupBox;
    LabelRunGate_GatePort1: TLabel;
    EditRunGate_GatePort1: TEdit;
    LabelLabelRunGate_GatePort2: TLabel;
    EditRunGate_GatePort2: TEdit;
    LabelRunGate_GatePort3: TLabel;
    EditRunGate_GatePort3: TEdit;
    LabelRunGate_GatePort4: TLabel;
    EditRunGate_GatePort4: TEdit;
    LabelRunGate_GatePort5: TLabel;
    EditRunGate_GatePort5: TEdit;
    LabelRunGate_GatePort6: TLabel;
    EditRunGate_GatePort6: TEdit;
    LabelRunGate_GatePort7: TLabel;
    EditRunGate_GatePort7: TEdit;
    EditRunGate_GatePort8: TEdit;
    LabelRunGate_GatePort78: TLabel;
    ButtonRunGateDefault: TButton;
    ButtonSelGateDefault: TButton;
    ButtonGeneralDefalult: TButton;
    ButtonLoginGateDefault: TButton;
    ButtonLoginSrvDefault: TButton;
    ButtonDBServerDefault: TButton;
    ButtonLogServerDefault: TButton;
    ButtonM2ServerDefault: TButton;
    GroupBox24: TGroupBox;
    Label29: TLabel;
    EditSelGate_GatePort: TEdit;
    TabSheet15: TTabSheet;
    GroupBox27: TGroupBox;
    CheckBoxboLoginGate_GetStart: TCheckBox;
    GroupBoxSelGate_GetStart: TGroupBox;
    CheckBoxboSelGate_GetStart: TCheckBox;
    GroupBox32: TGroupBox;
    Label61: TLabel;
    Label62: TLabel;
    EditM2Server_TestLevel: TSpinEdit;
    EditM2Server_TestGold: TSpinEdit;
    Label49: TLabel;
    EditSelGate_GatePort1: TEdit;
    GroupBox33: TGroupBox;
    Label50: TLabel;
    Label51: TLabel;
    EditLoginServerGatePort: TEdit;
    EditLoginServerServerPort: TEdit;
    GroupBox34: TGroupBox;
    CheckBoxboLoginServer_GetStart: TCheckBox;
    GroupBox35: TGroupBox;
    CheckBoxDBServerGetStart: TCheckBox;
    GroupBox36: TGroupBox;
    Label52: TLabel;
    Label53: TLabel;
    EditDBServerGatePort: TEdit;
    EditDBServerServerPort: TEdit;
    GroupBox37: TGroupBox;
    CheckBoxLogServerGetStart: TCheckBox;
    GroupBox38: TGroupBox;
    Label54: TLabel;
    EditLogServerPort: TEdit;
    GroupBox39: TGroupBox;
    Label55: TLabel;
    EditM2ServerGatePort: TEdit;
    GroupBox40: TGroupBox;
    CheckBoxM2ServerGetStart: TCheckBox;
    Label56: TLabel;
    EditM2ServerMsgSrvPort: TEdit;
    GroupBox41: TGroupBox;
    LabelVersion: TLabel;
    Label60: TLabel;
    CheckBoxRunGate3: TCheckBox;
    CheckBoxRunGate4: TCheckBox;
    CheckBoxRunGate5: TCheckBox;
    CheckBoxRunGate6: TCheckBox;
    CheckBoxRunGate7: TCheckBox;
    GroupBox44: TGroupBox;
    CheckBoxboRunGate_GetMinimize: TCheckBox;
    TimerStart: TTimer;
    TabSheet66: TTabSheet;
    GroupBox21: TGroupBox;
    ListViewDataBackup: TListView;
    GroupBox29: TGroupBox;
    lbl1: TLabel;
    lbl2: TLabel;
    lbl3: TLabel;
    lbl4: TLabel;
    lbl5: TLabel;
    lbl6: TLabel;
    RadioButtonBackMode1: TRadioButton;
    EditSource: TRzButtonEdit;
    EditDest: TRzButtonEdit;
    RadioButtonBackMode2: TRadioButton;
    EditHour1: TRzSpinEdit;
    EditHour2: TRzSpinEdit;
    EditMin1: TRzSpinEdit;
    EditMin2: TRzSpinEdit;
    ButtonBackChg: TButton;
    ButtonBackDel: TButton;
    ButtonBackAdd: TButton;
    ButtonBackSave: TButton;
    ButtonBackStart: TButton;
    LabelBackMsg: TLabel;
    TimerClose: TTimer;
    chkAutoStartServer: TCheckBox;
    lbl7: TLabel;
    EditAutoStartDelayTime: TSpinEdit;
    lbl8: TLabel;
    TimerAutoStartServer: TTimer;
    LabelNetComIPaddr: TLabel;
    EditGameExtNetComIPaddr: TEdit;
    CheckBoxSelGate1: TCheckBox;
    CheckBoxboSelGate_GetStart1: TCheckBox;
    TabSheet3: TTabSheet;
    EditEnvirFilePath: TRzButtonEdit;
    Label5: TLabel;
    Label6: TLabel;
    EditDBName: TEdit;
    MemoLog1: TMemo;
    ButtonStdMode: TButton;
    ButtonUnbindItem: TButton;
    ButtonUnTakeOffItem: TButton;
    ButtonChangeItemNameColorWhite: TButton;
    ButtonMapEvent: TButton;
    TabSheet14: TTabSheet;
    pgc1: TPageControl;
    ts1: TTabSheet;
    ts2: TTabSheet;
    CheckGroupClear: TRzCheckGroup;
    grp1: TGroupBox;
    lbl9: TLabel;
    btnMyGetTxtDel: TRzRapidFireButton;
    btnMyGetTxtAdd: TRzRapidFireButton;
    btnMyGetTxtOpen: TRzRapidFireButton;
    edtMyGetTXT: TEdit;
    lstMyGetTXT: TListBox;
    grp2: TGroupBox;
    lbl10: TLabel;
    btnMyGetFileOpen: TRzRapidFireButton;
    btnMyGetFileAdd: TRzRapidFireButton;
    btnMyGetFileDel: TRzRapidFireButton;
    edtMyGetFile: TEdit;
    lstMyGetFile: TListBox;
    grp3: TGroupBox;
    lbl11: TLabel;
    btnMyGetDirOpen: TRzRapidFireButton;
    btnMyGetDirAdd: TRzRapidFireButton;
    btnMyGetDirDel: TRzRapidFireButton;
    edtMyGetDir: TEdit;
    lstMyGetDir: TListBox;
    img1: TImage;
    ClearServerOpenDialog: TOpenDialog;
    btnStartClear: TRzBitBtn;
    btnClearSave: TRzBitBtn;
    chkDynamicIPMode: TCheckBox;
    chkIsCompress: TCheckBox;
    chkAutoStart: TCheckBox;
    GroupBox28: TGroupBox;
    Label7: TLabel;
    Label8: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    Label26: TLabel;
    Label27: TLabel;
    Label45: TLabel;
    Label46: TLabel;
    edtRunGate_DBPort1: TEdit;
    edtRunGate_DBPort2: TEdit;
    edtRunGate_DBPort3: TEdit;
    edtRunGate_DBPort4: TEdit;
    edtRunGate_DBPort5: TEdit;
    edtRunGate_DBPort6: TEdit;
    edtRunGate_DBPort7: TEdit;
    edtRunGate_DBPort8: TEdit;
    grp4: TGroupBox;
    CheckBoxboRunGate_GetMultiThread: TCheckBox;
    lbl14: TLabel;
    edtRunGate_DBPortMulThread: TEdit;
    grp5: TGroupBox;
    lbl15: TLabel;
    sePortInc: TSpinEdit;
    btn2: TButton;
    lbl16: TLabel;
    ts3: TTabSheet;
    grp6: TGroupBox;
    chkTimerStart: TCheckBox;
    chkEmbeddedWindow: TCheckBox;
    GroupBox26: TGroupBox;
    Label31: TLabel;
    Label32: TLabel;
    Label33: TLabel;
    Label34: TLabel;
    Label35: TLabel;
    Label36: TLabel;
    Label37: TLabel;
    Label38: TLabel;
    Label39: TLabel;
    Label40: TLabel;
    Label41: TLabel;
    Label42: TLabel;
    Label43: TLabel;
    Label44: TLabel;
    edtLoginAccount: TEdit;
    edtLoginAccountPasswd: TEdit;
    edtLoginAccountUserName: TEdit;
    edtLoginAccountSSNo: TEdit;
    edtLoginAccountBirthDay: TEdit;
    edtLoginAccountQuiz: TEdit;
    edtLoginAccountAnswer: TEdit;
    edtLoginAccountQuiz2: TEdit;
    edtLoginAccountAnswer2: TEdit;
    edtLoginAccountMobilePhone: TEdit;
    edtLoginAccountMemo: TEdit;
    edtLoginAccountEMail: TEdit;
    edtLoginAccountMemo2: TEdit;
    chkFullEditMode: TCheckBox;
    ButtonLoginAccountOK: TButton;
    edtLoginAccountPhone: TEdit;
    Panel1: TPanel;
    Label30: TLabel;
    edtSearchLoginAccount: TEdit;
    ButtonSearchLoginAccount: TButton;
    lbl17: TLabel;
    EditLoginGate_GatePort: TEdit;
    Label9: TLabel;
    Label10: TLabel;
    EditLoginGate_MainFormX: TSpinEdit;
    EditLoginGate_MainFormY: TSpinEdit;
    CheckBoxboLoginGate_GetMinimize: TCheckBox;
    CheckBoxboSelGate_GetMinimize: TCheckBox;
    CheckBoxboLoginServer_GetMinimize: TCheckBox;
    CheckBoxDBServerGetMinimize: TCheckBox;
    CheckBoxLogServerGetMinimize: TCheckBox;
    CheckBoxM2ServerGetMinimize: TCheckBox;
    pnlProgramWindow: TPanel;
    grp7: TGroupBox;
    EditHeroDB: TEdit;
    rbBDE: TRadioButton;
    rbSqlite: TRadioButton;
    edtSqliteDB: TRzButtonEdit;
    Label2: TLabel;
    EditLoginServerControlPort: TEdit;
    btnSetPath: TButton;
    grp8: TGroupBox;
    rbDataSaveSqlite: TRadioButton;
    grpDataSaveMySql: TGroupBox;
    rbDataSaveMySql: TRadioButton;
    lblSrcDBPort: TLabel;
    lblSrcDBServer: TLabel;
    edtDataSaveDBServer: TEdit;
    seDataSaveDBPort: TSpinEditEx;
    lblSrcDBUser: TLabel;
    lblSrcDBPassword: TLabel;
    edtDataSaveDBUser: TEdit;
    edtDataSaveDBPassword: TEdit;
    Label28: TLabel;
    edtDataSaveDataBase: TEdit;
    lblMySqlLinkTest: TLabel;
    lblMySqlDoInit: TLabel;
    chkSelGate_GetMultiThread: TCheckBox;
    Button1: TButton;
    Button2: TButton;
    dtpDate: TDateTimePicker;
    dtpTime: TDateTimePicker;
    procedure ButtonNext1Click(Sender: TObject);
    procedure ButtonPrv2Click(Sender: TObject);
    procedure ButtonNext2Click(Sender: TObject);
    procedure ButtonPrv3Click(Sender: TObject);
    procedure ButtonSaveClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure ButtonGenGameConfigClick(Sender: TObject);
    procedure ButtonStartGameClick(Sender: TObject);
    procedure TimerStartGameTimer(Sender: TObject);
    procedure CheckBoxDBServerClick(Sender: TObject);
    procedure CheckBoxLoginServerClick(Sender: TObject);
    procedure CheckBoxM2ServerClick(Sender: TObject);
    procedure CheckBoxLogServerClick(Sender: TObject);
    procedure CheckBoxLoginGateClick(Sender: TObject);
    procedure CheckBoxSelGateClick(Sender: TObject);
    procedure CheckBoxRunGateClick(Sender: TObject);
    procedure TimerStopGameTimer(Sender: TObject);
    procedure TimerCheckRunTimer(Sender: TObject);
    procedure ButtonReLoadConfigClick(Sender: TObject);
    procedure EditLoginGate_MainFormXChange(Sender: TObject);
    procedure EditLoginGate_MainFormYChange(Sender: TObject);
    procedure EditSelGate_MainFormXChange(Sender: TObject);
    procedure EditSelGate_MainFormYChange(Sender: TObject);
    procedure EditLoginServer_MainFormXChange(Sender: TObject);
    procedure EditLoginServer_MainFormYChange(Sender: TObject);
    procedure EditDBServer_MainFormXChange(Sender: TObject);
    procedure EditDBServer_MainFormYChange(Sender: TObject);
    procedure EditLogServer_MainFormXChange(Sender: TObject);
    procedure EditLogServer_MainFormYChange(Sender: TObject);
    procedure EditM2Server_MainFormXChange(Sender: TObject);
    procedure EditM2Server_MainFormYChange(Sender: TObject);
    procedure MemoLogChange(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure ButtonNext3Click(Sender: TObject);
    procedure ButtonNext4Click(Sender: TObject);
    procedure ButtonNext5Click(Sender: TObject);
    procedure ButtonNext6Click(Sender: TObject);
    procedure ButtonNext7Click(Sender: TObject);
    procedure ButtonPrv4Click(Sender: TObject);
    procedure ButtonPrv5Click(Sender: TObject);
    procedure ButtonPrv6Click(Sender: TObject);
    procedure ButtonPrv7Click(Sender: TObject);
    procedure ButtonPrv8Click(Sender: TObject);
    procedure ButtonNext8Click(Sender: TObject);
    procedure ButtonPrv9Click(Sender: TObject);
    procedure EditRunGate_ConntChange(Sender: TObject);
    procedure ButtonLoginServerConfigClick(Sender: TObject);
    procedure chkDoubleLineModeClick(Sender: TObject);
    procedure ButtonRunGateDefaultClick(Sender: TObject);
    procedure ButtonGeneralDefalultClick(Sender: TObject);
    procedure ButtonLoginGateDefaultClick(Sender: TObject);
    procedure ButtonSelGateDefaultClick(Sender: TObject);
    procedure ButtonLoginSrvDefaultClick(Sender: TObject);
    procedure ButtonDBServerDefaultClick(Sender: TObject);
    procedure ButtonLogServerDefaultClick(Sender: TObject);
    procedure ButtonM2ServerDefaultClick(Sender: TObject);
    procedure ButtonSearchLoginAccountClick(Sender: TObject);
    procedure chkFullEditModeClick(Sender: TObject);
    procedure ButtonLoginAccountOKClick(Sender: TObject);
    procedure edtLoginAccountChange(Sender: TObject);
    procedure CheckBoxboLoginGate_GetStartClick(Sender: TObject);
    procedure CheckBoxboSelGate_GetStartClick(Sender: TObject);
    procedure ButtonM2SuspendClick(Sender: TObject);
    procedure EditM2Server_TestLevelChange(Sender: TObject);
    procedure EditM2Server_TestGoldChange(Sender: TObject);
    procedure CheckBoxboLoginServer_GetStartClick(Sender: TObject);
    procedure CheckBoxDBServerGetStartClick(Sender: TObject);
    procedure CheckBoxLogServerGetStartClick(Sender: TObject);
    procedure CheckBoxM2ServerGetStartClick(Sender: TObject);
    procedure CheckBoxM2ServerGetMinimizeClick(Sender: TObject);
    procedure CheckBoxLogServerGetMinimizeClick(Sender: TObject);
    procedure CheckBoxDBServerGetMinimizeClick(Sender: TObject);
    procedure CheckBoxboLoginServer_GetMinimizeClick(Sender: TObject);
    procedure CheckBoxboRunGate_GetMinimizeClick(Sender: TObject);
    procedure CheckBoxboSelGate_GetMinimizeClick(Sender: TObject);
    procedure CheckBoxboLoginGate_GetMinimizeClick(Sender: TObject);
    procedure TimerStartTimer(Sender: TObject);
    procedure EditSourceButtonClick(Sender: TObject);
    procedure EditDestButtonClick(Sender: TObject);
    procedure RadioButtonBackMode1Click(Sender: TObject);
    procedure RadioButtonBackMode2Click(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure TimerCloseTimer(Sender: TObject);
    procedure ListViewDataBackupClick(Sender: TObject);
    procedure ButtonBackChgClick(Sender: TObject);
    procedure ButtonBackDelClick(Sender: TObject);
    procedure ButtonBackAddClick(Sender: TObject);
    procedure ButtonBackSaveClick(Sender: TObject);
    procedure ButtonBackStartClick(Sender: TObject);
    procedure TimerAutoStartServerTimer(Sender: TObject);
    procedure chkAutoStartServerClick(Sender: TObject);
    procedure EditAutoStartDelayTimeChange(Sender: TObject);
    procedure CheckBoxboSelGate_GetStart1Click(Sender: TObject);
    procedure CheckBoxSelGate1Click(Sender: TObject);
    procedure CheckBoxboRunGate_GetMultiThreadClick(Sender: TObject);
    procedure ButtonStdModeClick(Sender: TObject);
    procedure ButtonMapEventClick(Sender: TObject);
    procedure ButtonChangeItemNameColorWhiteClick(Sender: TObject);
    procedure ButtonUnbindItemClick(Sender: TObject);
    procedure ButtonUnTakeOffItemClick(Sender: TObject);
    procedure btnMyGetTxtOpenClick(Sender: TObject);
    procedure btnMyGetTxtAddClick(Sender: TObject);
    procedure btnMyGetTxtDelClick(Sender: TObject);
    procedure btnClearSaveClick(Sender: TObject);
    procedure btnStartClearClick(Sender: TObject);
    procedure chkDynamicIPModeClick(Sender: TObject);
    procedure chkTimerStartClick(Sender: TObject);
    procedure chkAutoStartClick(Sender: TObject);
    procedure btn2Click(Sender: TObject);
    procedure chkEmbeddedWindowClick(Sender: TObject);
    procedure edtSqliteDBButtonClick(Sender: TObject);
    procedure rbBDEClick(Sender: TObject);
    procedure rbSqliteClick(Sender: TObject);
    procedure btnSetPathClick(Sender: TObject);
    procedure rbDataSaveSqliteClick(Sender: TObject);
    procedure lblMySqlLinkTestClick(Sender: TObject);
    procedure lblMySqlDoInitClick(Sender: TObject);
    procedure chkSelGate_GetMultiThreadClick(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    m_boOpen: Boolean;
    m_nStartStatus: Integer;
    m_dwShowTick: LongWord;
    m_StartTime: TDateTime;

    //FOldCaption: string;

    procedure RefGameConsole();
    procedure GenGameConfig();
    procedure GenDBServerConfig();
    procedure GenLoginServerConfig();
    procedure GenLogServerConfig();
    procedure GenM2ServerConfig();
    procedure GenLoginGateConfig();
    procedure GenSelGateConfig();
    procedure GenRunGateConfig;
    procedure GenBackupConfig;
    procedure GetMutRunGateConfing(nGateIndex: Integer);
    procedure GetMutRunGateConfingEx;       // add chongchong 多线程网关配置生成 2014-06-22
    procedure GetMutRunGateZero;
    procedure StartGame();
    procedure StopGame();
    procedure MainOutMessage(sMsg: string);


    procedure ProcessDBServerMsg(wIdent: Word; sData: string);
    procedure ProcessLoginSrvMsg(wIdent: Word; sData: string);
    procedure ProcessLoginSrvGetUserAccount(sData: string);
    procedure ProcessLoginSrvChangeUserAccountStatus(sData: string);
    procedure UserAccountEditMode(boChecked: Boolean);
    procedure ProcessLogServerMsg(wIdent: Word; sData: string);

    procedure ProcessLoginGateMsg(wIdent: Word; sData: string);
    procedure ProcessLoginGate1Msg(wIdent: Word; sData: string);

    procedure ProcessSelGateMsg(wIdent: Word; sData: string);
    procedure ProcessSelGate1Msg(wIdent: Word; sData: string);

    procedure ProcessRunGateMsg(wIdent: Word; sData: string);
    procedure ProcessM2ServerMsg(wIdent: Word; sData: string);
   

    procedure GenMutSelGateConfigEx;      // 2019-09-28 16:17:55
    procedure GetMutSelGateZero;          // 2019-09-28 16:17:58
    procedure GenMutSelGateConfig(nIndex: Integer);
    procedure GenMutLoginGateConfig(nIndex: Integer);


    procedure LoadBackList();

    procedure RefBackListToView();
    { Private declarations }
  protected
    procedure GetMutLoginGateZero;
    procedure RefGameDebug();
    function StartService(): Boolean;
    procedure StopService();

  public
    procedure ProcessMessage(var Msg: TMsg; var Handled: Boolean);
    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
    procedure SaveBackList();
    { Public declarations }
  end;

var
  frmMain: TfrmMain;

implementation

uses GShare, HUtil32, Grobal2, EDcode, GHeroDBConfig, GLoginServer, GBDEtoSqlite;

{$R *.dfm}

////////////////////////////////////////////////////////////////////////////////
// 在ListBox中添加行 piaoyun 2013-08-28

procedure ListBoxAdd(ListBox: TListBox; AddStr: string);
var
  i: Integer;
begin
  for i := 0 to ListBox.Items.Count - 1 do
  begin
    if ListBox.Items.Strings[i] = AddStr then
    begin
      application.MessageBox('此文件路径已在列表中，请重新选择！！', '提示信息', MB_ICONASTERISK);
      Exit;
    end;
  end;
  ListBox.Items.Add(AddStr);
end;

// 在ListBox中删除选择行 piaoyun 2013-08-28

procedure ListBoxDel(ListBox: TListBox);
begin
  ListBox.Items.BeginUpdate;
  try
    ListBox.DeleteSelected;
  finally
    ListBox.Items.EndUpdate;
  end;
end;

procedure ClearModValue();
begin
  frmMain.btnClearSave.Enabled := True;
end;

// 清空文件 piaoyun 2013-08-28

procedure ClearTxt(TxtName: string);
var
  f: textfile;
begin
  assignfile(f, TxtName);
  rewrite(f);
  closefile(f);
end;

// 保存自定义清理路径配置 piaoyun 2013-08-28

function Clear_SaveConfig(): Boolean;
var
  I: Integer;
begin
  //Result := False;
  g_IniConf.WriteInteger('ClearServer', 'MyGetTxtNum', frmMain.lstMyGetTXT.Items.Count);
  if frmMain.lstMyGetTXT.Items.Count <> 0 then begin
    for I := 0 to frmMain.lstMyGetTXT.Items.Count - 1 do begin
      g_IniConf.WriteString('ClearServer', 'MyGetTxt' + IntToStr(i), frmMain.lstMyGetTXT.Items.Strings[i]);
    end;
  end;

  g_IniConf.WriteInteger('ClearServer', 'MyGetFileNum', frmMain.lstMyGetFile.Items.Count);
  if frmMain.lstMyGetFile.Items.Count <> 0 then begin
    for I := 0 to frmMain.lstMyGetFile.Items.Count - 1 do begin
      g_IniConf.WriteString('ClearServer', 'MyGetFile' + IntToStr(i), frmMain.lstMyGetFile.Items.Strings[i]);
    end;
  end;

  g_IniConf.WriteInteger('ClearServer', 'MyGetDirNum', frmMain.lstMyGetDir.Items.Count);
  if frmMain.lstMyGetDir.Items.Count <> 0 then begin
    for I := 0 to frmMain.lstMyGetDir.Items.Count - 1 do begin
      g_IniConf.WriteString('ClearServer', 'MyGetDir' + IntToStr(i), frmMain.lstMyGetDir.Items.Strings[i]);
    end;
  end;
  Result := True;
end;

// 加载自定义清理路径配置 piaoyun 2013-08-28

procedure Clear_LoadConfig();
var
  nMyGetTxtNum, nMyGetFileNum, nMyGetDirNum, I: Integer;
begin
  nMyGetTxtNum := g_IniConf.ReadInteger('ClearServer', 'MyGetTxtNum', 0);
  nMyGetFileNum := g_IniConf.ReadInteger('ClearServer', 'MyGetFileNum', 0);
  nMyGetDirNum := g_IniConf.ReadInteger('ClearServer', 'MyGetDirNum', 0);
  if nMyGetTxtNum <> 0 then
  begin
    frmMain.lstMyGetTXT.Items.Clear;
    for I := 0 to nMyGetTxtNum - 1 do
    begin
      frmMain.lstMyGetTXT.Items.Add(g_IniConf.ReadString('ClearServer', 'MyGetTxt' + IntToStr(I), '读取配置文件错误'));
    end;
  end;

  if nMyGetFileNum <> 0 then
  begin
    frmMain.lstMyGetFile.Items.Clear;
    for I := 0 to nMyGetFileNum - 1 do
    begin
      frmMain.lstMyGetFile.Items.Add(g_IniConf.ReadString('ClearServer', 'MyGetFile' + IntToStr(I), '读取配置文件错误'));
    end;
  end;
  if nMyGetDirNum <> 0 then
  begin
    frmMain.lstMyGetDir.Items.Clear;
    for I := 0 to nMyGetDirNum - 1 do
    begin
      frmMain.lstMyGetDir.Items.Add(g_IniConf.ReadString('ClearServer', 'MyGetDir' + IntToStr(I), '读取配置文件错误'));
    end;
  end;
end;
////////////////////////////////////////////////////////////////////////////////


//文件夹浏览函数    uses ShlObj, ActiveX

function SelectDirCB(Wnd: HWND; uMsg: UINT; lParam, lpData: lParam): Integer stdcall;
begin
  if (uMsg = BFFM_INITIALIZED) and (lpData <> 0) then
    SendMessage(Wnd, BFFM_SETSELECTION, Integer(True), lpData);
  Result := 0;
end;

function SelectDirectory(const Caption: string; const Root: WideString;
  var Directory: string; Owner: THandle): Boolean;
var
  WindowList: Pointer;
  BrowseInfo: TBrowseInfo;
  Buffer: PChar;
  RootItemIDList, ItemIDList: PItemIDList;
  ShellMalloc: IMalloc;
  IDesktopFolder: IShellFolder;
  Eaten, Flags: LongWord;
begin
  Result := False;
  if not DirectoryExists(Directory) then
    Directory := '';
  FillChar(BrowseInfo, SizeOf(BrowseInfo), 0);
  if (ShGetMalloc(ShellMalloc) = S_OK) and (ShellMalloc <> nil) then
  begin
    Buffer := ShellMalloc.Alloc(MAX_PATH);
    try
      RootItemIDList := nil;
      if Root <> '' then
      begin
        SHGetDesktopFolder(IDesktopFolder);
        IDesktopFolder.ParseDisplayName(Application.Handle, nil,
          POleStr(Root), Eaten, RootItemIDList, Flags);
      end;
      with BrowseInfo do
      begin
        hwndOwner := Owner;                                                                         //Application.Handle;
        pidlRoot := RootItemIDList;
        pszDisplayName := Buffer;
        lpszTitle := PChar(Caption);
        ulFlags := BIF_RETURNONLYFSDIRS;
        if Directory <> '' then
        begin
          lpfn := SelectDirCB;
          lParam := Integer(PChar(Directory));
        end;
      end;
      WindowList := DisableTaskWindows(0);
      try
        ItemIDList := ShBrowseForFolder(BrowseInfo);
      finally
        EnableTaskWindows(WindowList);
      end;
      Result := ItemIDList <> nil;
      if Result then
      begin
        ShGetPathFromIDList(ItemIDList, Buffer);
        ShellMalloc.Free(ItemIDList);
        Directory := Buffer;
      end;
    finally
      ShellMalloc.Free(Buffer);
    end;
  end;
end;

procedure ClearSetupIni;
const
  MAXCHANGELEVEL = 1000;
var
  I: Integer;
  SetupIni: TIniFile;
  FileName: string;
  SL: TStringList;
begin
  FileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConfigFile;
  if not FileExists(FileName) then Exit;

  SL := TStringList.Create;
  try
    SetupIni := TIniFile.Create(FileName);
    try
      SetupIni.DeleteKey('Setup', 'HighLevel');
      SetupIni.DeleteKey('Setup', 'HighLevelGetExp');
      SetupIni.DeleteKey('Setup', 'MaxUpLevelCount');
      SetupIni.DeleteKey('Setup', 'LimitChangeExp');
      SetupIni.DeleteKey('Exp', 'KillMonExpMultiple');
      SetupIni.DeleteKey('Exp', 'HighLevelKillMonFixExp');
      SetupIni.DeleteKey('Exp', 'UseFixExp');
      SetupIni.DeleteKey('Exp', 'BaseExp');
      SetupIni.DeleteKey('Exp', 'AddExp');
      SetupIni.DeleteKey('Exp', 'HighLevelGroupFixExp');

      for I := 1 to MAXCHANGELEVEL do
        SetupIni.DeleteKey('Exp', 'Level' + IntToStr(I));

      for I := 1 to MAXCHANGELEVEL do
        SetupIni.DeleteKey('Exp', 'LevelExpRate' + IntToStr(I));

      for I := 1 to MAXCHANGELEVEL do
        SetupIni.DeleteKey('HeroExp', 'Level' + IntToStr(I));

      for I := 1 to MAXCHANGELEVEL do
        SetupIni.DeleteKey('MedicineExp', 'Level' + IntToStr(I));

      for I := 1 to MAXCHANGELEVEL do
        SetupIni.DeleteKey('WineExp', 'Level' + IntToStr(I));

      SetupIni.DeleteKey('Setup', 'IncAlcoholTime');
      SetupIni.DeleteKey('Setup', 'DecDrinkTime');
      SetupIni.DeleteKey('Setup', 'MaxAlcoholValue');
      SetupIni.DeleteKey('Setup', 'IncAlcoholValue');
      SetupIni.DeleteKey('Setup', 'DecMedicineValue');
      SetupIni.DeleteKey('Setup', 'DecMedicineTime');

      SL.Clear;
      SetupIni.ReadSection('Exp', SL);
      if SL.Count = 0 then
        SetupIni.EraseSection('Exp');

      SL.Clear;
      SetupIni.ReadSection('HeroExp', SL);
      if SL.Count = 0 then
        SetupIni.EraseSection('HeroExp');

      SL.Clear;
      SetupIni.ReadSection('MedicineExp', SL);
      if SL.Count = 0 then
        SetupIni.EraseSection('MedicineExp');

      SL.Clear;
      SetupIni.ReadSection('WineExp', SL);
      if SL.Count = 0 then
        SetupIni.EraseSection('WineExp');

      for I := 0 to 499 do
      begin
        SetupIni.DeleteKey('Setup', 'GlobalVal' + IntToStr(I));
        SetupIni.DeleteKey('Setup', 'GlobalStrVal' + IntToStr(I));
      end;
    finally
      SetupIni.Free;
    end;
  finally
    SL.Free;
  end;
end;


function ClearGlobal(FileName: string): Boolean;
var
  Config: TIniFile;
  I: integer;
begin
  //Result := False;
  Config := TIniFile.Create(FileName);
  try
    for I := 0 to 999 do
    begin
      Config.WriteInteger('Setup', 'GlobalVal' + IntToStr(I), 0);
    end;

    for I := 0 to 999 do
    begin
      Config.WriteString('Setup', 'GlobalStrVal' + IntToStr(I), '');
    end;
  finally
    Config.Free;
  end;
    //sleep(2000);
  Result := True;
end;

procedure TfrmMain.MainOutMessage(sMsg: string);
begin
  sMsg := '[' + DateTimeToStr(Now) + '] ' + sMsg;
  MemoLog.Lines.Add(sMsg);
end;

procedure TfrmMain.ButtonNext1Click(Sender: TObject);
var
  sGameDirectory: string;
  sHeroDBName: string;
  sGameName: string;
  sExtIPAddr: string;
  sExtNetComIPaddr: string;

  sDataSaveDBServer: string;
  wDataSaveDBPort: Word;
  sDataSaveDBUser: string;
  sDataSaveDBPassword: string;
  sDataSaveDataBase: string;
begin
  sGameDirectory := Trim(EditGameDir.Text);

  g_boUseSqliteDB := rbSqlite.Checked;
  if g_boUseSqliteDB then
  begin
    sHeroDBName := Trim(edtSqliteDB.Text);
  end
  else
  begin
    sHeroDBName := Trim(EditHeroDB.Text);
  end;

  if rbDataSaveSqlite.Checked then
    g_nDataSaveDBType := 0
  else
  begin
    g_nDataSaveDBType := 1;
  end;

  sDataSaveDBServer := edtDataSaveDBServer.Text;
  wDataSaveDBPort := seDataSaveDBPort.Value;
  sDataSaveDBUser := edtDataSaveDBUser.Text;
  sDataSaveDBPassword := edtDataSaveDBPassword.Text;
  sDataSaveDataBase := edtDataSaveDataBase.Text;

  sGameName := Trim(EditGameName.Text);
  sExtIPAddr := Trim(EditGameExtIPaddr.Text);
  if sGameName = '' then
  begin
    Application.MessageBox('游戏服务器名称输入不正确！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
    EditGameName.SetFocus;
    Exit;
  end;
  if (sExtIPAddr = '') or not IsIPaddr(sExtIPAddr) then
  begin
    Application.MessageBox('游戏服务器外部IP地址输入不正确！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
    EditGameExtIPaddr.SetFocus;
    Exit;
  end;

  if g_boDoubleLineMode then
  begin
    sExtNetComIPaddr := Trim(EditGameExtNetComIPaddr.Text);

    if (sExtNetComIPaddr = '') or not IsIPaddr(sExtNetComIPaddr) then
    begin
      Application.MessageBox('游戏服务器外部IP地址2输入不正确！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
      EditGameExtNetComIPaddr.SetFocus;
      Exit;
    end;
  end;

  if (sGameDirectory = '') or not DirectoryExists(sGameDirectory) then
  begin
    Application.MessageBox('游戏目录输入不正确！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
    EditGameDir.SetFocus;
    Exit;
  end;
  if not (sGameDirectory[Length(sGameDirectory)] = '\') then
  begin
    Application.MessageBox('游戏目录名称最后一个字符必须为"\"！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
    EditGameDir.SetFocus;
    Exit;
  end;
  if sHeroDBName = '' then
  begin
    Application.MessageBox('游戏数据库名称输入不正确！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
    EditHeroDB.SetFocus;
    Exit;
  end;

  if g_boUseSqliteDB then
  begin
    if not FileExists(sHeroDBName) then
    begin
      Application.MessageBox('游戏数据库不存在！！！', '提示信息', MB_OK + MB_ICONEXCLAMATION);
      edtSqliteDB.SetFocus;
      Exit;
    end;
    g_sSqliteDBName := sHeroDBName;
  end
  else
  begin
    g_sHeroDBName := sHeroDBName;
  end;

  // 判断Mysql信息是否正确
  if g_nDataSaveDBType = 1 then
  begin

  end;

  g_sDataSaveDBServer := sDataSaveDBServer;
  g_wDataSaveDBPort := wDataSaveDBPort;
  g_sDataSaveDBUser := sDataSaveDBUser;
  g_sDataSaveDBPassword := sDataSaveDBPassword;
  g_sDataSaveDataBase := sDataSaveDataBase;

  g_sOldGameDirectory := g_sGameDirectory;
  g_sGameDirectory := sGameDirectory;

  g_sGameName := sGameName;
  g_sExtIPaddr := sExtIPAddr;
  g_sExtNetComIPaddr := sExtNetComIPAddr;
  // 增加动态IP支持 piaoyun 2013-08-30
  g_boDynamicIPMode := chkDynamicIPMode.Checked;

  //Caption := FOldCaption + ' [' + g_sGameDirectory + ']';
  Caption := Format('%s - [%s]', [sProgramName,  g_sGameDirectory]);

  PageControl3.ActivePageIndex := 1;
end;

procedure TfrmMain.ButtonPrv2Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 0;
end;

procedure TfrmMain.ButtonNext2Click(Sender: TObject);
var
  nPort: Integer;
begin
  nPort := Str_ToInt(Trim(EditLoginGate_GatePort.Text), -1);
  if (nPort < 0) or (nPort > 65535) then
  begin
    Application.MessageBox('网关端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditLoginGate_GatePort.SetFocus;
    Exit;
  end;
  g_nLoginGate_GatePort := nPort;
  PageControl3.ActivePageIndex := 2;
end;

procedure TfrmMain.ButtonPrv3Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 1;
end;

procedure TfrmMain.ButtonNext3Click(Sender: TObject);
var
  nPort: Integer;
begin
  nPort := Str_ToInt(Trim(EditSelGate_GatePort.Text), -1);
  if (nPort < 0) or (nPort > 65535) then
  begin
    Application.MessageBox('网关端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditSelGate_GatePort.SetFocus;
    Exit;
  end;
  g_nSeLGate_GatePort := nPort;

  nPort := Str_ToInt(Trim(EditSelGate_GatePort1.Text), -1);
  if (nPort < 0) or (nPort > 65535) then
  begin
    Application.MessageBox('网关端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditSelGate_GatePort1.SetFocus;
    Exit;
  end;
  g_nSeLGate_GatePort1 := nPort;
  PageControl3.ActivePageIndex := 3;
end;

procedure TfrmMain.ButtonNext4Click(Sender: TObject);
var
  nPort1, nPort2, nPort3, nPort4, nPort5, nPort6, nPort7, nPort8: Integer;
  nPort21, nPort22, nPort23, nPort24, nPort25, nPort26, nPort27, nPort28, nPort29: Integer;
begin
  nPort1 := Str_ToInt(Trim(EditRunGate_GatePort1.Text), -1);
  nPort2 := Str_ToInt(Trim(EditRunGate_GatePort2.Text), -1);
  nPort3 := Str_ToInt(Trim(EditRunGate_GatePort3.Text), -1);
  nPort4 := Str_ToInt(Trim(EditRunGate_GatePort4.Text), -1);
  nPort5 := Str_ToInt(Trim(EditRunGate_GatePort5.Text), -1);
  nPort6 := Str_ToInt(Trim(EditRunGate_GatePort6.Text), -1);
  nPort7 := Str_ToInt(Trim(EditRunGate_GatePort7.Text), -1);
  nPort8 := Str_ToInt(Trim(EditRunGate_GatePort8.Text), -1);

  nPort21 := Str_ToInt(Trim(edtRunGate_DBPort1.Text), -1);
  nPort22 := Str_ToInt(Trim(edtRunGate_DBPort2.Text), -1);
  nPort23 := Str_ToInt(Trim(edtRunGate_DBPort3.Text), -1);
  nPort24 := Str_ToInt(Trim(edtRunGate_DBPort4.Text), -1);
  nPort25 := Str_ToInt(Trim(edtRunGate_DBPort5.Text), -1);
  nPort26 := Str_ToInt(Trim(edtRunGate_DBPort6.Text), -1);
  nPort27 := Str_ToInt(Trim(edtRunGate_DBPort7.Text), -1);
  nPort28 := Str_ToInt(Trim(edtRunGate_DBPort8.Text), -1);

  nPort29 := Str_ToInt(Trim(edtRunGate_DBPortMulThread.Text), -1);


  if (nPort1 < 0) or (nPort1 > 65535) then
  begin
    Application.MessageBox('网关一端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort1.SetFocus;
    Exit;
  end;
  if (nPort2 < 0) or (nPort2 > 65535) then
  begin
    Application.MessageBox('网关二端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort2.SetFocus;
    Exit;
  end;
  if (nPort3 < 0) or (nPort3 > 65535) then
  begin
    Application.MessageBox('网关三端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort3.SetFocus;
    Exit;
  end;
  if (nPort4 < 0) or (nPort4 > 65535) then
  begin
    Application.MessageBox('网关四端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort4.SetFocus;
    Exit;
  end;
  if (nPort5 < 0) or (nPort5 > 65535) then
  begin
    Application.MessageBox('网关五端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort5.SetFocus;
    Exit;
  end;
  if (nPort6 < 0) or (nPort6 > 65535) then
  begin
    Application.MessageBox('网关六端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort6.SetFocus;
    Exit;
  end;
  if (nPort7 < 0) or (nPort7 > 65535) then
  begin
    Application.MessageBox('网关七端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort7.SetFocus;
    Exit;
  end;
  if (nPort8 < 0) or (nPort8 > 65535) then
  begin
    Application.MessageBox('网关八端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditRunGate_GatePort8.SetFocus;
    Exit;
  end;

  if (nPort21 < 0) or (nPort21 > 65535) then
  begin
    Application.MessageBox('网关一端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort1.SetFocus;
    Exit;
  end;
  if (nPort22 < 0) or (nPort22 > 65535) then
  begin
    Application.MessageBox('网关二端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort2.SetFocus;
    Exit;
  end;
  if (nPort23 < 0) or (nPort23 > 65535) then
  begin
    Application.MessageBox('网关三端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort3.SetFocus;
    Exit;
  end;
  if (nPort24 < 0) or (nPort24 > 65535) then
  begin
    Application.MessageBox('网关四端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort4.SetFocus;
    Exit;
  end;
  if (nPort25 < 0) or (nPort25 > 65535) then
  begin
    Application.MessageBox('网关五端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort5.SetFocus;
    Exit;
  end;
  if (nPort26 < 0) or (nPort26 > 65535) then
  begin
    Application.MessageBox('网关六端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort6.SetFocus;
    Exit;
  end;
  if (nPort27 < 0) or (nPort27 > 65535) then
  begin
    Application.MessageBox('网关七端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort7.SetFocus;
    Exit;
  end;
  if (nPort28 < 0) or (nPort28 > 65535) then
  begin
    Application.MessageBox('网关八端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPort8.SetFocus;
    Exit;
  end;
  if (nPort29 < 0) or (nPort29 > 65535) then
  begin
    Application.MessageBox('多线程网关DBServer连接端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtRunGate_DBPortMulThread.SetFocus;
    Exit;
  end;

  g_RunGateInfo[0].nGatePort := nPort1;
  g_RunGateInfo[1].nGatePort := nPort2;
  g_RunGateInfo[2].nGatePort := nPort3;
  g_RunGateInfo[3].nGatePort := nPort4;
  g_RunGateInfo[4].nGatePort := nPort5;
  g_RunGateInfo[5].nGatePort := nPort6;
  g_RunGateInfo[6].nGatePort := nPort7;
  g_RunGateInfo[7].nGatePort := nPort8;

  g_RunGateInfo[0].nDBPort := nPort21;
  g_RunGateInfo[1].nDBPort := nPort22;
  g_RunGateInfo[2].nDBPort := nPort23;
  g_RunGateInfo[3].nDBPort := nPort24;
  g_RunGateInfo[4].nDBPort := nPort25;
  g_RunGateInfo[5].nDBPort := nPort26;
  g_RunGateInfo[6].nDBPort := nPort27;
  g_RunGateInfo[7].nDBPort := nPort28;

  g_nRunGateDBPort_MulThread := nPort29;

  PageControl3.ActivePageIndex := 4;
end;

procedure TfrmMain.ButtonNext5Click(Sender: TObject);
var
  nGatePort, nServerPort, nControlPort: Integer;
begin
  nGatePort := Str_ToInt(Trim(EditLoginServerGatePort.Text), -1);
  nServerPort := Str_ToInt(Trim(EditLoginServerServerPort.Text), -1);
  nControlPort := Str_ToInt(Trim(EditLoginServerControlPort.Text), -1);

  if (nGatePort < 0) or (nGatePort > 65535) then
  begin
    Application.MessageBox('网关端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditLoginServerGatePort.SetFocus;
    Exit;
  end;

  if (nServerPort < 0) or (nServerPort > 65535) then
  begin
    Application.MessageBox('通讯端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditLoginServerServerPort.SetFocus;
    Exit;
  end;

  if (nControlPort < 0) or (nControlPort > 65535) then
  begin
    Application.MessageBox('通讯端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditLoginServerControlPort.SetFocus;
    Exit;
  end;

  g_nLoginServer_GatePort := nGatePort;
  g_nLoginServer_ServerPort := nServerPort;
  g_nLoginServer_ControlPort := nControlPort;
  PageControl3.ActivePageIndex := 5;
end;

procedure TfrmMain.ButtonNext6Click(Sender: TObject);
var
  nGatePort, nServerPort: Integer;
begin
  nGatePort := Str_ToInt(Trim(EditDBServerGatePort.Text), -1);
  nServerPort := Str_ToInt(Trim(EditDBServerServerPort.Text), -1);

  if (nGatePort < 0) or (nGatePort > 65535) then
  begin
    Application.MessageBox('网关端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditDBServerGatePort.SetFocus;
    Exit;
  end;
  if (nServerPort < 0) or (nServerPort > 65535) then
  begin
    Application.MessageBox('通讯端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditDBServerServerPort.SetFocus;
    Exit;
  end;

  g_nDBServer_Config_GatePort := nGatePort;
  g_nDBServer_Config_ServerPort := nServerPort;
  PageControl3.ActivePageIndex := 6;
end;

procedure TfrmMain.ButtonNext7Click(Sender: TObject);
var
  nPort: Integer;
begin
  nPort := Str_ToInt(Trim(EditLogServerPort.Text), -1);
  if (nPort < 0) or (nPort > 65535) then
  begin
    Application.MessageBox('端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditLogServerPort.SetFocus;
    Exit;
  end;
  g_nLogServer_Port := nPort;
  PageControl3.ActivePageIndex := 7;
end;

procedure TfrmMain.ButtonNext8Click(Sender: TObject);
var
  nGatePort, nMsgSrvPort: Integer;
begin
  nGatePort := Str_ToInt(Trim(EditM2ServerGatePort.Text), -1);
  nMsgSrvPort := Str_ToInt(Trim(EditM2ServerMsgSrvPort.Text), -1);
  if (nGatePort < 0) or (nGatePort > 65535) then
  begin
    Application.MessageBox('网关端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditM2ServerGatePort.SetFocus;
    Exit;
  end;
  if (nMsgSrvPort < 0) or (nMsgSrvPort > 65535) then
  begin
    Application.MessageBox('通讯端口设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditM2ServerMsgSrvPort.SetFocus;
    Exit;
  end;
  g_nM2Server_GatePort := nGatePort;
  g_nM2Server_MsgSrvPort := nMsgSrvPort;
  PageControl3.ActivePageIndex := 8;
end;

procedure TfrmMain.ButtonPrv4Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 2;
end;

procedure TfrmMain.ButtonPrv5Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 3;
end;

procedure TfrmMain.ButtonPrv6Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 4;
end;

procedure TfrmMain.ButtonPrv7Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 5;
end;

procedure TfrmMain.ButtonPrv8Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 6;
end;

procedure TfrmMain.ButtonPrv9Click(Sender: TObject);
begin
  PageControl3.ActivePageIndex := 7;
end;

procedure TfrmMain.ButtonSaveClick(Sender: TObject);
begin
  //  ButtonSave.Enabled:=False;
  g_IniConf.WriteInteger('GameConf', 'dwStopTimeOut', g_dwStopTimeOut);
  g_IniConf.WriteString('GameConf', 'GameDirectory', g_sGameDirectory);

  g_IniConf.WriteBool('GameConf', 'UseSqliteDB', g_boUseSqliteDB);
  g_IniConf.WriteString('GameConf', 'HeroDBName', g_sHeroDBName);
  g_IniConf.WriteString('GameConf', 'SqliteDBName', g_sSqliteDBName);


  g_IniConf.WriteInteger('DataSaveDB', 'DataSaveDBType', g_nDataSaveDBType);
  g_IniConf.WriteString('DataSaveDB', 'DataSaveDBServer', g_sDataSaveDBServer);
  g_IniConf.WriteInteger('DataSaveDB', 'DataSaveDBPort', g_wDataSaveDBPort);
  g_IniConf.WriteString('DataSaveDB', 'DataSaveDBUser', g_sDataSaveDBUser);
  g_IniConf.WriteString('DataSaveDB', 'DataSaveDBPassword', g_sDataSaveDBPassword);
  g_IniConf.WriteString('DataSaveDB', 'DataSaveDataBase', g_sDataSaveDataBase);


  g_IniConf.WriteString('GameConf', 'GameName', g_sGameName);
  g_IniConf.WriteString('GameConf', 'ExtIPaddr', g_sExtIPaddr);
  g_IniConf.WriteString('GameConf', 'ExtNetComIPaddr', g_sExtNetComIPaddr);

  g_IniConf.WriteInteger('GameConf', 'AutoStartDelayTime', g_nAutoStartDelayTime);
  g_IniConf.WriteBool('GameConf', 'GetAutoStartServer', g_boAutoStartServer);
  //g_IniConf.WriteBool('GameConf', 'GetAutoStartBackUpServer', g_boAutoStartBackUpServer);
  g_IniConf.WriteBool('GameConf', 'DynamicIPMode', g_boDynamicIPMode);

  g_IniConf.WriteBool('GameConf', 'DoubleLineMode', g_boDoubleLineMode);
  g_IniConf.WriteInteger('DBServer', 'MainFormX', g_nDBServer_MainFormX);
  g_IniConf.WriteInteger('DBServer', 'MainFormY', g_nDBServer_MainFormY);
  g_IniConf.WriteInteger('DBServer', 'GatePort', g_nDBServer_Config_GatePort);
  g_IniConf.WriteInteger('DBServer', 'ServerPort', g_nDBServer_Config_ServerPort);
  g_IniConf.WriteBool('DBServer', 'DisableAutoGame', g_boDBServer_DisableAutoGame);
  g_IniConf.WriteBool('DBServer', 'GetStart', g_boDBServer_GetStart);
  g_IniConf.WriteBool('DBServer', 'GetMinimize', g_boDBServer_GetMinimize);


  g_IniConf.WriteInteger('M2Server', 'MainFormX', g_nM2Server_MainFormX);
  g_IniConf.WriteInteger('M2Server', 'MainFormY', g_nM2Server_MainFormY);
  g_IniConf.WriteInteger('M2Server', 'TestLevel', g_nM2Server_TestLevel);
  g_IniConf.WriteInteger('M2Server', 'TestGold', g_nM2Server_TestGold);

  g_IniConf.WriteInteger('M2Server', 'GatePort', g_nM2Server_GatePort);
  g_IniConf.WriteInteger('M2Server', 'MsgSrvPort', g_nM2Server_MsgSrvPort);
  g_IniConf.WriteBool('M2Server', 'GetStart', g_boM2Server_GetStart);
  g_IniConf.WriteBool('M2Server', 'GetMinimize', g_boM2Server_GetMinimize);

  g_IniConf.WriteInteger('RunGate', 'Count', g_nRunGate_Count);
  g_IniConf.WriteBool('RunGate', 'GetMinimize', g_boRunGate_GetMinimize);
  g_IniConf.WriteBool('RunGate', 'GetMultiThread', g_boRunGate_GetMultiThread);

  g_IniConf.WriteInteger('RunGate', 'GatePort1', g_RunGateInfo[0].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort2', g_RunGateInfo[1].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort3', g_RunGateInfo[2].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort4', g_RunGateInfo[3].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort5', g_RunGateInfo[4].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort6', g_RunGateInfo[5].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort7', g_RunGateInfo[6].nGatePort);
  g_IniConf.WriteInteger('RunGate', 'GatePort8', g_RunGateInfo[7].nGatePort);

  g_IniConf.WriteInteger('RunGate', 'DBPort1', g_RunGateInfo[0].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort2', g_RunGateInfo[1].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort3', g_RunGateInfo[2].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort4', g_RunGateInfo[3].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort5', g_RunGateInfo[4].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort6', g_RunGateInfo[5].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort7', g_RunGateInfo[6].nDBPort);
  g_IniConf.WriteInteger('RunGate', 'DBPort8', g_RunGateInfo[7].nDBPort);

  g_IniConf.WriteInteger('RunGate', 'DBPort_MulThread', g_nRunGateDBPort_MulThread);

  g_IniConf.WriteInteger('LoginGate', 'MainFormX', g_nLoginGate_MainFormX);
  g_IniConf.WriteInteger('LoginGate', 'MainFormY', g_nLoginGate_MainFormY);
  g_IniConf.WriteBool('LoginGate', 'GetStart', g_boLoginGate_GetStart);
  g_IniConf.WriteBool('LoginGate', 'GetMinimize', g_boLoginGate_GetMinimize);
  g_IniConf.WriteInteger('LoginGate', 'GatePort', g_nLoginGate_GatePort);

  g_IniConf.WriteInteger('SelGate', 'MainFormX', g_nSelGate_MainFormX);
  g_IniConf.WriteInteger('SelGate', 'MainFormY', g_nSelGate_MainFormY);
  g_IniConf.WriteBool('SelGate', 'GetStart', g_boSelGate_GetStart);
  g_IniConf.WriteBool('SelGate', 'GetStart1', g_boSelGate_GetStart1);
  g_IniConf.WriteBool('SelGate', 'GetMinimize', g_boSelGate_GetMinimize);
  g_IniConf.WriteInteger('SelGate', 'GatePort', g_nSeLGate_GatePort);
  g_IniConf.WriteInteger('SelGate', 'GatePort1', g_nSeLGate_GatePort1);
  g_IniConf.WriteBool('SelGate', 'GetMultiThread', g_boSelGate_GetMultiThread);     // 2019-09-28 15:38:06


  g_IniConf.WriteInteger('LoginServer', 'MainFormX', g_nLoginServer_MainFormX);
  g_IniConf.WriteInteger('LoginServer', 'MainFormY', g_nLoginServer_MainFormY);
  g_IniConf.WriteInteger('LoginServer', 'GatePort', g_nLoginServer_GatePort);
  g_IniConf.WriteInteger('LoginServer', 'ServerPort', g_nLoginServer_ServerPort);
  g_IniConf.WriteInteger('LoginServer', 'ControlPort', g_nLoginServer_ControlPort);
  g_IniConf.WriteBool('LoginServer', 'GetStart', g_boLoginServer_GetStart);
  g_IniConf.WriteBool('LoginServer', 'GetMinimize', g_boLoginServer_GetMinimize);

  g_IniConf.WriteInteger('LogServer', 'MainFormX', g_nLogServer_MainFormX);
  g_IniConf.WriteInteger('LogServer', 'MainFormY', g_nLogServer_MainFormY);

  g_IniConf.WriteInteger('LogServer', 'Port', g_nLogServer_Port);
  g_IniConf.WriteBool('LogServer', 'GetStart', g_boLogServer_GetStart);
  g_IniConf.WriteBool('LogServer', 'GetMinimize', g_boLogServer_GetMinimize);

  Application.MessageBox('配置文件已经保存完毕...', '提示信息', MB_OK + MB_ICONINFORMATION);
  if Application.MessageBox('是否生成新的游戏服务器配置文件...', '提示信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
  begin
    ButtonGenGameConfigClick(ButtonGenGameConfig);
  end;
  PageControl3.ActivePageIndex := 0;
  PageControl1.ActivePageIndex := 0;

  //Caption := m_sCapTion + '-' + g_sGameName;
  //Application.Title := '引擎控制台';//Caption;
  //FOldCaption := Caption;
   Caption := Format('%s - [%s]', [sProgramName,  g_sGameDirectory]);
end;

procedure TfrmMain.SaveBackList();
var
  I: Integer;
  BackUpTask: TBackUpTask;
  Conini: Tinifile;
begin
  DeleteFile(ExtractFilePath(ParamStr(0)) + 'BackList.txt');
  Conini := Tinifile.Create(ExtractFilePath(ParamStr(0)) + 'BackList.txt');
  if Conini <> nil then
  begin
    for I := 0 to g_BackUpManager.m_BackUpList.Count - 1 do
    begin
      BackUpTask := TBackUpTask(g_BackUpManager.m_BackUpList.Items[I]);
      Conini.WriteString(IntToStr(I), 'Source', BackUpTask.SourceDirectory);
      Conini.WriteString(IntToStr(I), 'Save', BackUpTask.DestDirectory);
      Conini.WriteInteger(IntToStr(I), 'Hour', BackUpTask.Hour);
      Conini.WriteInteger(IntToStr(I), 'Min', BackUpTask.Min);
      Conini.WriteInteger(IntToStr(I), 'BackMode', BackUpTask.Mode);
      Conini.WriteBool(IntToStr(I), 'GetBack', BackUpTask.Start);
      // 是否压缩 piaoyun 2013-08-30
      Conini.WriteBool(IntToStr(I), 'IsCompress', BackUpTask.IsCompress);
    end;
    Conini.Free;
  end;
end;

procedure TfrmMain.LoadBackList();
var
  I: Integer;
  List: TStringList;
  Conini: Tinifile;
  BackUpTask: TBackUpTask;
  sSource, sDest: string;
  wHour, wMin: Word;
  btBackMode: Byte;
  boGetBack: Boolean;
  IsCompress: Boolean;
begin
  ButtonBackDel.Enabled := False;
  ButtonBackChg.Enabled := False;
  List := TStringList.Create;
  Conini := Tinifile.Create(ExtractFilePath(ParamStr(0)) + 'BackList.txt');
  Conini.ReadSections(List);
  if Conini <> nil then
  begin
    for I := 0 to List.Count - 1 do
    begin
      sSource := Conini.ReadString(List.Strings[I], 'Source', '');
      sDest := Conini.ReadString(List.Strings[I], 'Save', '');
      wHour := Conini.ReadInteger(List.Strings[I], 'Hour', 0);
      wMin := Conini.ReadInteger(List.Strings[I], 'Min', 0);
      btBackMode := Conini.ReadInteger(List.Strings[I], 'BackMode', 0);
      boGetBack := Conini.ReadBool(List.Strings[I], 'GetBack', True);
      IsCompress := Conini.ReadBool(List.Strings[I], 'IsCompress', True);
      if (sSource <> '') and (sDest <> '') then
      begin
        BackUpTask := TBackUpTask.Create;
        BackUpTask.SourceDirectory := sSource;
        BackUpTask.DestDirectory := sDest;
        BackUpTask.Mode := btBackMode;
        BackUpTask.Hour := wHour;
        BackUpTask.Min := wMin;
        BackUpTask.Start := boGetBack;
        // 是否压缩 piaoyun 2013-08-30
        BackUpTask.IsCompress := IsCompress;
        g_BackUpManager.Add(BackUpTask);
      end;
    end;
    Conini.Free;
  end;
  List.Free;
end;

procedure TfrmMain.RefBackListToView();
var
  I: Integer;
  BackUpTask: TBackUpTask;
  ListItem: TListItem;
begin
  ListViewDataBackup.Items.Clear;
  for I := 0 to g_BackUpManager.m_BackUpList.Count - 1 do
  begin
    BackUpTask := TBackUpTask(g_BackUpManager.m_BackUpList.Items[I]);

    ListItem := ListViewDataBackup.Items.Add;
    ListItem.Caption := BackUpTask.SourceDirectory;
    ListItem.SubItems.AddObject(BackUpTask.DestDirectory, BackUpTask);
    ListItem.SubItems.Add(IntToStr(BackUpTask.BackUpCount));
    ListItem.SubItems.Add(IntToStr(BackUpTask.FailCount));
    if BackUpTask.Start then
      ListItem.SubItems.Add('启动')
    else
      ListItem.SubItems.Add('停止');
  end;
end;

procedure TfrmMain.FormCreate(Sender: TObject);
var
  I: Integer;

  Conini: Tinifile;
  FileName: string;
begin
  {asm
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  end; }
  chkEmbeddedWindow.Enabled := True;

  ButtonStartGame.Caption := g_sButtonStartGame;
  FileName := ExtractFilePath(ParamStr(0)) + 'BackConfig.txt';
  Conini := Tinifile.Create(FileName);
  chkAutoStart.Checked := Conini.ReadBool('Setup', 'AutoStart', False);
  Conini.Free;

  LabelVersion.Caption := m_sVersion;

  m_boOpen := False;
  g_nFormIdx := g_IniConf.ReadInteger('Setup', 'FormID', g_nFormIdx);
  Application.OnMessage := ProcessMessage;
  PageControl1.ActivePageIndex := 0;
  PageControl3.ActivePageIndex := 0;
  m_nStartStatus := 0;

  MemoLog.Clear;
  g_BackUpManager := TBackUpManager.Create;
  LoadConfig();
  Clear_LoadConfig;
  LoadBackList();
  RefBackListToView();

  //Caption := m_sCapTion + '-' + g_sGameName;
  //Application.Title := '引擎控制台';
  //FOldCaption := Caption;
  //Caption := Caption + ' [' + g_sGameDirectory + ']';
  Caption := Format('%s - [%s]', [sProgramName,  g_sGameDirectory]);

  if not StartService() then Exit;
  RefGameConsole();
  for I := 0 to CheckGroupClear.Items.Count - 1 do
    CheckGroupClear.ItemChecked[I] := True;
  m_boOpen := True;
  TimerStart.Enabled := True;
  //MainOutMessage('游戏控制器启动成功...');
//  SetWindowPos(Self.Handle,HWND_TOPMOST,Self.Left,Self.Top,Self.Width,Self.Height,$40);
end;

procedure TfrmMain.ButtonGenGameConfigClick(Sender: TObject);
begin
  //  ButtonGenGameConfig.Enabled:=False;
  GenGameConfig();
  RefGameConsole();
  Application.MessageBox('引擎配置文件已经生成完毕...', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TfrmMain.GenGameConfig;
begin
  GenDBServerConfig();
  GenLoginServerConfig();
  GenLogServerConfig();
  GenM2ServerConfig();
  GenLoginGateConfig();
  GenSelGateConfig();
  GenRunGateConfig();

  GenBackupConfig();
end;

procedure TfrmMain.GenDBServerConfig;

  function GetRunGatePort(nIndex: Integer): Integer;
  var
    I, nC: Integer;
  begin
    Result := 0;
    nC := 0;
    for I := 0 to Length(g_RunGateInfo) - 1 do
    begin
      if g_RunGateInfo[I].boGetStart then
      begin
        if nC = nIndex then
        begin
          Result := g_RunGateInfo[I].nGatePort;
          break;
        end;
        Inc(nC);
      end;
    end;
  end;

  function GetRunGateDBPort(nIndex: Integer): Integer;
  var
    I, nC: Integer;
  begin
    Result := 0;
    nC := 0;
    for I := 0 to Length(g_RunGateInfo) - 1 do
    begin
      if g_RunGateInfo[I].boGetStart then
      begin
        if nC = nIndex then
        begin
          Result := g_RunGateInfo[I].nDBPort;
          break;
        end;
        Inc(nC);
      end;
    end;
  end;
var
  IniGameConf: Tinifile;
  sIniFile, sFileName: string;
  SaveList: TStringList;
begin
  sIniFile := g_sGameDirectory + g_sDBServer_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sDBServer_ConfigFile);

  IniGameConf.WriteString('Setup', 'ServerName', g_sGameName);
  IniGameConf.WriteString('Setup', 'ServerAddr', g_sDBServer_Config_ServerAddr);
  IniGameConf.WriteInteger('Setup', 'ServerPort', g_nDBServer_Config_ServerPort);
  IniGameConf.WriteString('Setup', 'MapFile', g_sGameDirectory + g_sDBServer_Config_MapFile);
  IniGameConf.WriteBool('Setup', 'ViewHackMsg', g_boDBServer_Config_ViewHackMsg);
  IniGameConf.WriteBool('Setup', 'DoubleLineMode', g_boDoubleLineMode);
  // 增加动态IP支持 piaoyun 2013-08-30
  IniGameConf.WriteBool('Setup', 'DynamicIPMode', g_boDynamicIPMode);

  IniGameConf.WriteBool('Setup', 'DisableAutoGame', g_boDBServer_DisableAutoGame);
  IniGameConf.WriteString('Setup', 'GateAddr', g_sDBServer_Config_GateAddr);
  IniGameConf.WriteInteger('Setup', 'GatePort', g_nDBServer_Config_GatePort);

  IniGameConf.WriteString('Server', 'IDSAddr', g_sLoginServer_ServerAddr);                          //登录服务器IP
  IniGameConf.WriteInteger('Server', 'IDSPort', g_nLoginServer_ServerPort);                         //登录服务器端口


  IniGameConf.WriteBool('Setup', 'UseSqliteDB', g_boUseSqliteDB);
  IniGameConf.WriteString('Setup', 'DBName', g_sHeroDBName);
  IniGameConf.WriteString('Setup', 'SqliteDBName', g_sSqliteDBName);

  IniGameConf.WriteInteger('DataSaveDB', 'DataSaveDBType', g_nDataSaveDBType);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBServer', g_sDataSaveDBServer);
  IniGameConf.WriteInteger('DataSaveDB', 'DataSaveDBPort', g_wDataSaveDBPort);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBUser', g_sDataSaveDBUser);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBPassword', g_sDataSaveDBPassword);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDataBase', g_sDataSaveDataBase);


  IniGameConf.WriteInteger('DBClear', 'Interval', g_nDBServer_Config_Interval);
  IniGameConf.WriteInteger('DBClear', 'Level1', g_nDBServer_Config_Level1);
  IniGameConf.WriteInteger('DBClear', 'Level2', g_nDBServer_Config_Level2);
  IniGameConf.WriteInteger('DBClear', 'Level3', g_nDBServer_Config_Level3);
  IniGameConf.WriteInteger('DBClear', 'Day1', g_nDBServer_Config_Day1);
  IniGameConf.WriteInteger('DBClear', 'Day2', g_nDBServer_Config_Day2);
  IniGameConf.WriteInteger('DBClear', 'Day3', g_nDBServer_Config_Day3);
  IniGameConf.WriteInteger('DBClear', 'Month1', g_nDBServer_Config_Month1);
  IniGameConf.WriteInteger('DBClear', 'Month2', g_nDBServer_Config_Month2);
  IniGameConf.WriteInteger('DBClear', 'Month3', g_nDBServer_Config_Month3);
  IniGameConf.WriteString('DB', 'Dir', sIniFile + g_sDBServer_Config_Dir);
  IniGameConf.WriteString('DB', 'IdDir', sIniFile + g_sDBServer_Config_IdDir);
  IniGameConf.WriteString('DB', 'HumDir', sIniFile + g_sDBServer_Config_HumDir);
  IniGameConf.WriteString('DB', 'FeeDir', sIniFile + g_sDBServer_Config_FeeDir);
  IniGameConf.WriteString('DB', 'BackupDir', sIniFile + g_sDBServer_Config_BackupDir);
  IniGameConf.WriteString('DB', 'ConnectDir', sIniFile + g_sDBServer_Config_ConnectDir);
  IniGameConf.WriteString('DB', 'LogDir', sIniFile + g_sDBServer_Config_LogDir);

  IniGameConf.Free;

  SaveList := TStringList.Create;
  SaveList.Add(g_sLocalIPaddr);

  if g_boDoubleLineMode then
  begin
    SaveList.Add(g_sLocalIPaddr1);
    SaveList.Add(g_sExtIPaddr);
    SaveList.Add(g_sExtNetComIPaddr);
  end else
  begin
    SaveList.Add(g_sExtIPaddr);
  end;

  SaveList.SaveToFile(sIniFile + g_sDBServer_AddrTableFile);

  SaveList.Clear;

  sFileName := sIniFile + g_sDBServer_GateListFile;
  if not FileExists(sFileName) then
    SaveList.SaveToFile(sFileName);

  IniGameConf := Tinifile.Create(sFileName);
  IniGameConf.EraseSection('GateDBPort0');

  case g_nRunGate_Count of
    1:
      begin
        SaveList.Add(Format('%s %s %d', [g_sLocalIPaddr, g_sExtIPaddr, GetRunGatePort(0)]));
        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
      end;

    2:
      begin
        SaveList.Add(Format('%s %s %d %s %d', [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0), g_sExtIPaddr, GetRunGatePort(1)]));
        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
      end;

    3:
      begin
        SaveList.Add(Format('%s %s %d %s %d %s %d',
          [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0),
            g_sExtIPaddr, GetRunGatePort(1),
            g_sExtIPaddr, GetRunGatePort(2)]));
        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
        IniGameConf.WriteInteger('GateDBPort0', '3', GetRunGateDBPort(2));
      end;

    4:
      begin
        SaveList.Add(Format('%s %s %d %s %d %s %d %s %d', [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0),
            g_sExtIPaddr, GetRunGatePort(1),
            g_sExtIPaddr, GetRunGatePort(2),
            g_sExtIPaddr, GetRunGatePort(3)]));
        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
        IniGameConf.WriteInteger('GateDBPort0', '3', GetRunGateDBPort(2));
        IniGameConf.WriteInteger('GateDBPort0', '4', GetRunGateDBPort(3));
      end;
    5:
      begin
        SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0),
            g_sExtIPaddr, GetRunGatePort(1),
            g_sExtIPaddr, GetRunGatePort(2),
            g_sExtIPaddr, GetRunGatePort(3),
            g_sExtIPaddr, GetRunGatePort(4)]));
        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
        IniGameConf.WriteInteger('GateDBPort0', '3', GetRunGateDBPort(2));
        IniGameConf.WriteInteger('GateDBPort0', '4', GetRunGateDBPort(3));
        IniGameConf.WriteInteger('GateDBPort0', '5', GetRunGateDBPort(4));
      end;

    6:
      begin
        SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0),
            g_sExtIPaddr, GetRunGatePort(1),
            g_sExtIPaddr, GetRunGatePort(2),
            g_sExtIPaddr, GetRunGatePort(3),
            g_sExtIPaddr, GetRunGatePort(4),
            g_sExtIPaddr, GetRunGatePort(5)]));

        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
        IniGameConf.WriteInteger('GateDBPort0', '3', GetRunGateDBPort(2));
        IniGameConf.WriteInteger('GateDBPort0', '4', GetRunGateDBPort(3));
        IniGameConf.WriteInteger('GateDBPort0', '5', GetRunGateDBPort(4));
        IniGameConf.WriteInteger('GateDBPort0', '6', GetRunGateDBPort(5));
      end;

    7:
      begin
        SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0),
            g_sExtIPaddr, GetRunGatePort(1),
            g_sExtIPaddr, GetRunGatePort(2),
            g_sExtIPaddr, GetRunGatePort(3),
            g_sExtIPaddr, GetRunGatePort(4),
            g_sExtIPaddr, GetRunGatePort(5),
            g_sExtIPaddr, GetRunGatePort(6)]));

        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
        IniGameConf.WriteInteger('GateDBPort0', '3', GetRunGateDBPort(2));
        IniGameConf.WriteInteger('GateDBPort0', '4', GetRunGateDBPort(3));
        IniGameConf.WriteInteger('GateDBPort0', '5', GetRunGateDBPort(4));
        IniGameConf.WriteInteger('GateDBPort0', '6', GetRunGateDBPort(5));
        IniGameConf.WriteInteger('GateDBPort0', '7', GetRunGateDBPort(6));
      end;

    8:
      begin
        SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr,
          g_sExtIPaddr, GetRunGatePort(0),
            g_sExtIPaddr, GetRunGatePort(1),
            g_sExtIPaddr, GetRunGatePort(2),
            g_sExtIPaddr, GetRunGatePort(3),
            g_sExtIPaddr, GetRunGatePort(4),
            g_sExtIPaddr, GetRunGatePort(5),
            g_sExtIPaddr, GetRunGatePort(6),
            g_sExtIPaddr, GetRunGatePort(7)]));

        IniGameConf.WriteInteger('GateDBPort0', '1', GetRunGateDBPort(0));
        IniGameConf.WriteInteger('GateDBPort0', '2', GetRunGateDBPort(1));
        IniGameConf.WriteInteger('GateDBPort0', '3', GetRunGateDBPort(2));
        IniGameConf.WriteInteger('GateDBPort0', '4', GetRunGateDBPort(3));
        IniGameConf.WriteInteger('GateDBPort0', '5', GetRunGateDBPort(4));
        IniGameConf.WriteInteger('GateDBPort0', '6', GetRunGateDBPort(5));
        IniGameConf.WriteInteger('GateDBPort0', '7', GetRunGateDBPort(6));
        IniGameConf.WriteInteger('GateDBPort0', '8', GetRunGateDBPort(7));
      end;
  end;
  IniGameConf.Free;

  if g_boDoubleLineMode then
  begin
    case g_nRunGate_Count of
      1: SaveList.Add(Format('%s %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0)]));

      2: SaveList.Add(Format('%s %s %d %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1)]));

      3: SaveList.Add(Format('%s %s %d %s %d %s %d',
          [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1),
            g_sExtNetComIPaddr, GetRunGatePort(2)]));

      4: SaveList.Add(Format('%s %s %d %s %d %s %d %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1),
            g_sExtNetComIPaddr, GetRunGatePort(2),
            g_sExtNetComIPaddr, GetRunGatePort(3)]));

      5: SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1),
            g_sExtNetComIPaddr, GetRunGatePort(2),
            g_sExtNetComIPaddr, GetRunGatePort(3),
            g_sExtNetComIPaddr, GetRunGatePort(4)]));

      6: SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1),
            g_sExtNetComIPaddr, GetRunGatePort(2),
            g_sExtNetComIPaddr, GetRunGatePort(3),
            g_sExtNetComIPaddr, GetRunGatePort(4),
            g_sExtNetComIPaddr, GetRunGatePort(5)]));

      7: SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1),
            g_sExtNetComIPaddr, GetRunGatePort(2),
            g_sExtNetComIPaddr, GetRunGatePort(3),
            g_sExtNetComIPaddr, GetRunGatePort(4),
            g_sExtNetComIPaddr, GetRunGatePort(5),
            g_sExtNetComIPaddr, GetRunGatePort(6)]));

      8: SaveList.Add(Format('%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d %s %d', [g_sLocalIPaddr1,
          g_sExtNetComIPaddr, GetRunGatePort(0),
            g_sExtNetComIPaddr, GetRunGatePort(1),
            g_sExtNetComIPaddr, GetRunGatePort(2),
            g_sExtNetComIPaddr, GetRunGatePort(3),
            g_sExtNetComIPaddr, GetRunGatePort(4),
            g_sExtNetComIPaddr, GetRunGatePort(5),
            g_sExtNetComIPaddr, GetRunGatePort(6),
            g_sExtNetComIPaddr, GetRunGatePort(7)]));
    end;
  end;

  SaveList.SaveToFile(sIniFile + g_sDBServer_ServerinfoFile);
  SaveList.Free;

  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_Dir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_IdDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_HumDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_FeeDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_BackupDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_ConnectDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_LogDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  dtpDate.Date := Now;
  dtpTime.Time := Now;
end;

procedure TfrmMain.GenLoginServerConfig;
var
  IniGameConf: Tinifile;
  sIniFile: string;
  SaveList: TStringList;
begin
  sIniFile := g_sGameDirectory + g_sLoginServer_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sLoginServer_ConfigFile);
  IniGameConf.WriteInteger('Server', 'ReadyServers', g_sLoginServer_ReadyServers);

  IniGameConf.WriteString('Server', 'EnableMakingID', BoolToStr(g_sLoginServer_EnableMakingID));
  IniGameConf.WriteString('Server', 'EnableTrial', BoolToStr(g_sLoginServer_EnableTrial));
  IniGameConf.WriteString('Server', 'TestServer', BoolToStr(g_sLoginServer_TestServer));
  // 增加动态IP支持 piaoyun 2013-08-30
  IniGameConf.WriteBool('Server', 'DynamicIPMode', g_boDynamicIPMode);
  IniGameConf.WriteString('Server', 'GateAddr', g_sAllIPaddr);                                      //g_sLoginServer_GateAddr
  IniGameConf.WriteInteger('Server', 'GatePort', g_nLoginServer_GatePort);
  IniGameConf.WriteString('Server', 'ServerAddr', g_sAllIPaddr);                                    //g_sLoginServer_ServerAddr
  IniGameConf.WriteString('Server', 'ServerName', g_sGameName);
  IniGameConf.WriteInteger('Server', 'ServerPort', g_nLoginServer_ServerPort);
  IniGameConf.WriteInteger('Server', 'ControlPort', g_nLoginServer_ControlPort);

  IniGameConf.WriteString('DB', 'IdDir', sIniFile + g_sLoginServer_IdDir);
  IniGameConf.WriteString('DB', 'FeedIDList', sIniFile + g_sLoginServer_FeedIDList);
  IniGameConf.WriteString('DB', 'FeedIPList', sIniFile + g_sLoginServer_FeedIPList);
  IniGameConf.WriteString('DB', 'CountLogDir', sIniFile + g_sLoginServer_CountLogDir);
  IniGameConf.WriteString('DB', 'WebLogDir', sIniFile + g_sLoginServer_WebLogDir);

  IniGameConf.WriteString('DB', 'ChrLogDir', sIniFile + g_sLoginServer_ChrLogDir);
  IniGameConf.WriteString('DB', 'IDLogDir', sIniFile + g_sLoginServer_IDLogDir);

  IniGameConf.WriteInteger('DataSaveDB', 'DataSaveDBType', g_nDataSaveDBType);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBServer', g_sDataSaveDBServer);
  IniGameConf.WriteInteger('DataSaveDB', 'DataSaveDBPort', g_wDataSaveDBPort);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBUser', g_sDataSaveDBUser);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBPassword', g_sDataSaveDBPassword);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDataBase', g_sDataSaveDataBase);


  IniGameConf.Free;

  SaveList := TStringList.Create;

  if (not SelGate.boGetStart) and (not SelGate1.boGetStart) then
  begin
    SaveList.Add(Format('%s %s %s %s %s:%d', [g_sGameName, 'Title1', g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort]));
    if g_boDoubleLineMode then
      SaveList.Add(Format('%s %s %s %s %s:%d', [g_sGameName, 'Title2', g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort]));
  end
  else if SelGate.boGetStart and SelGate1.boGetStart then
  begin
    SaveList.Add(Format('%s %s %s %s %s:%d %s:%d', [g_sGameName, 'Title1', g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort, g_sExtIPaddr, g_nSeLGate_GatePort1]));
    if g_boDoubleLineMode then
      SaveList.Add(Format('%s %s %s %s %s:%d %s:%d', [g_sGameName, 'Title2', g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort, g_sExtIPaddr, g_nSeLGate_GatePort1]));
  end
  else if SelGate.boGetStart then
  begin
    SaveList.Add(Format('%s %s %s %s %s:%d', [g_sGameName, 'Title1', g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort]));
    if g_boDoubleLineMode then
      SaveList.Add(Format('%s %s %s %s %s:%d', [g_sGameName, 'Title2', g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort]));
  end
  else
  begin
    SaveList.Add(Format('%s %s %s %s %s:%d', [g_sGameName, 'Title1', g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort1]));
    if g_boDoubleLineMode then
      SaveList.Add(Format('%s %s %s %s %s:%d', [g_sGameName, 'Title2', g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort1]));
  end;

  SaveList.SaveToFile(sIniFile + g_sLoginServer_AddrTableFile);

  SaveList.Clear;

  SaveList.Add(g_sLocalIPaddr);

  if g_boDoubleLineMode then
  begin
    SaveList.Add(g_sLocalIPaddr1);
    SaveList.Add(g_sExtIPaddr);
    SaveList.Add(g_sExtNetComIPaddr);
  end else
  begin
    SaveList.Add(g_sExtIPaddr);
  end;

  SaveList.SaveToFile(sIniFile + g_sLoginServer_ServeraddrFile);

  SaveList.Clear;
  SaveList.Add(Format('%s %s %d', [g_sGameName, g_sGameName, g_nLimitOnlineUser]));
  SaveList.SaveToFile(sIniFile + g_sLoginServerUserLimitFile);
  SaveList.Free;

  sIniFile := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_IdDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  sIniFile := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_CountLogDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  sIniFile := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_WebLogDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
end;

procedure TfrmMain.GenLogServerConfig;
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  sIniFile := g_sGameDirectory + g_sLogServer_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sLogServer_ConfigFile);
  IniGameConf.WriteString('Setup', 'ServerName', g_sGameName);
  IniGameConf.WriteInteger('Setup', 'Port', g_nLogServer_Port);
  IniGameConf.WriteString('Setup', 'BaseDir', sIniFile + g_sLogServer_BaseDir);

  sIniFile := sIniFile + g_sLogServer_BaseDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  IniGameConf.Free;
end;

procedure TfrmMain.GenM2ServerConfig;
var
  IniGameConf: Tinifile;
  sIniFile: string;
  SaveList: TStringList;
begin
  sIniFile := g_sGameDirectory + g_sM2Server_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sM2Server_ConfigFile);

  IniGameConf.WriteString('Server', 'ServerName', g_sGameName);
  IniGameConf.WriteInteger('Server', 'ServerNumber', g_nM2Server_ServerNumber);
  IniGameConf.WriteInteger('Server', 'ServerIndex', g_nM2Server_ServerIndex);
                        
  IniGameConf.WriteInteger('Server', 'EditionId', g_nM2Server_EditionId);
  IniGameConf.WriteInteger('Server', 'AreaId', g_nM2Server_AreaId);
  
  IniGameConf.WriteString('Server', 'VentureServer', BoolToStr(g_boM2Server_VentureServer));
  IniGameConf.WriteString('Server', 'TestServer', BoolToStr(g_boM2Server_TestServer));
  IniGameConf.WriteInteger('Server', 'TestLevel', g_nM2Server_TestLevel);
  IniGameConf.WriteInteger('Server', 'TestGold', g_nM2Server_TestGold);
  IniGameConf.WriteInteger('Server', 'TestServerUserLimit', g_nLimitOnlineUser);
  IniGameConf.WriteString('Server', 'ServiceMode', BoolToStr(g_boM2Server_ServiceMode));
  IniGameConf.WriteString('Server', 'NonPKServer', BoolToStr(g_boM2Server_NonPKServer));

  IniGameConf.WriteString('Server', 'DBAddr', g_sDBServer_Config_ServerAddr);
  IniGameConf.WriteInteger('Server', 'DBPort', g_nDBServer_Config_ServerPort);
  IniGameConf.WriteString('Server', 'IDSAddr', g_sLoginServer_ServerAddr);
  IniGameConf.WriteInteger('Server', 'IDSPort', g_nLoginServer_ServerPort);
  IniGameConf.WriteString('Server', 'MsgSrvAddr', g_sAllIPaddr);                                    //g_sM2Server_MsgSrvAddr
  IniGameConf.WriteInteger('Server', 'MsgSrvPort', g_nM2Server_MsgSrvPort);
  IniGameConf.WriteString('Server', 'LogServerAddr', g_sLogServer_ServerAddr);
  IniGameConf.WriteInteger('Server', 'LogServerPort', g_nLogServer_Port);
  IniGameConf.WriteString('Server', 'GateAddr', g_sAllIPaddr);                                      //g_sM2Server_GateAddr
  IniGameConf.WriteInteger('Server', 'GatePort', g_nM2Server_GatePort);

  IniGameConf.WriteBool('Server', 'UseSqliteDB', g_boUseSqliteDB);
  IniGameConf.WriteString('Server', 'DBName', g_sHeroDBName);
  IniGameConf.WriteString('Server', 'SqliteDBName', g_sSqliteDBName);

  IniGameConf.WriteInteger('Server', 'UserFull', g_nLimitOnlineUser);

  IniGameConf.WriteString('Share', 'BaseDir', sIniFile + g_sM2Server_BaseDir);
  IniGameConf.WriteString('Share', 'GuildDir', sIniFile + g_sM2Server_GuildDir);
  IniGameConf.WriteString('Share', 'GuildFile', sIniFile + g_sM2Server_GuildFile);
  IniGameConf.WriteString('Share', 'VentureDir', sIniFile + g_sM2Server_VentureDir);
  IniGameConf.WriteString('Share', 'ConLogDir', sIniFile + g_sM2Server_ConLogDir);
  IniGameConf.WriteString('Share', 'LogDir', sIniFile + g_sM2Server_LogDir);
  IniGameConf.WriteString('Share', 'PlugDir', sIniFile);
  IniGameConf.WriteString('Share', 'BoxsDir', sIniFile + g_sM2Server_BoxsDir);

  IniGameConf.WriteString('Share', 'CastleDir', sIniFile + g_sM2Server_CastleDir);
  IniGameConf.WriteString('Share', 'EnvirDir', sIniFile + g_sM2Server_EnvirDir);
  IniGameConf.WriteString('Share', 'MapDir', sIniFile + g_sM2Server_MapDir);
  IniGameConf.WriteString('Share', 'NoticeDir', sIniFile + g_sM2Server_NoticeDir);
  IniGameConf.WriteString('Share', 'CastleFile', sIniFile + g_sM2Server_CastleFile);

  IniGameConf.WriteInteger('DataSaveDB', 'DataSaveDBType', g_nDataSaveDBType);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBServer', g_sDataSaveDBServer);
  IniGameConf.WriteInteger('DataSaveDB', 'DataSaveDBPort', g_wDataSaveDBPort);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBUser', g_sDataSaveDBUser);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDBPassword ', g_sDataSaveDBPassword);
  IniGameConf.WriteString('DataSaveDB', 'DataSaveDataBase', g_sDataSaveDataBase);


  IniGameConf.Free;

  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_BaseDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_GuildDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_VentureDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConLogDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_LogDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_CastleDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_MapDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  sIniFile := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_NoticeDir;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  sIniFile := g_sGameDirectory + g_sM2Server_Directory;
  SaveList := TStringList.Create;
  SaveList.Add('GM');
  SaveList.SaveToFile(sIniFile + g_sM2Server_AbuseFile);

  SaveList.Clear;
  SaveList.Add(g_sLocalIPaddr);
  SaveList.SaveToFile(sIniFile + g_sM2Server_RunAddrFile);

  SaveList.Clear;
  SaveList.Add(g_sLocalIPaddr);
  SaveList.SaveToFile(sIniFile + g_sM2Server_ServerTableFile);
  SaveList.Free;
end;

procedure TfrmMain.GenLoginGateConfig;
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  sIniFile := g_sGameDirectory + g_sLoginGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  IniGameConf := Tinifile.Create(sIniFile + g_sLoginGate_ConfigFile);
  IniGameConf.WriteString('LoginGate', 'Title', g_sGameName);
  IniGameConf.WriteString('LoginGate', 'GateAddr', g_sLoginGate_GateAddr);

  IniGameConf.WriteString('LoginGate', 'ServerAddr', g_sLoginGate_ServerAddr);
  IniGameConf.WriteInteger('LoginGate', 'ServerPort', g_nLoginServer_GatePort {g_nLoginGate_ServerPort});
  IniGameConf.WriteInteger('LoginGate', 'GatePort', g_nLoginGate_GatePort);

  IniGameConf.WriteInteger('LoginGate', 'Count', 1);
  IniGameConf.WriteString('LoginGate', 'ServerAddr1', g_sLoginGate_ServerAddr);
  IniGameConf.WriteInteger('LoginGate', 'ServerPort1', g_nLoginServer_GatePort {g_nLoginGate_ServerPort});
  IniGameConf.WriteInteger('LoginGate', 'GatePort1', g_nLoginGate_GatePort);

  IniGameConf.Free;
end;

procedure TfrmMain.GenSelGateConfig();
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  sIniFile := g_sGameDirectory + g_sSelGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  IniGameConf := Tinifile.Create(sIniFile + g_sSelGate_ConfigFile);
  IniGameConf.WriteString('SelGate', 'Title', g_sGameName);
  IniGameConf.WriteString('SelGate', 'ServerAddr', g_sSelGate_ServerAddr);
  IniGameConf.WriteInteger('SelGate', 'ServerPort', g_nDBServer_Config_GatePort {g_nSelGate_ServerPort});
  IniGameConf.WriteString('SelGate', 'GateAddr', g_sSelGate_GateAddr);
  IniGameConf.WriteInteger('SelGate', 'GatePort', g_nSeLGate_GatePort);
  //IniGameConf.WriteBool('SelGate', 'DynamicIPDisMode', g_boDynamicIPMode);
  {IniGameConf.WriteInteger('SelGate', 'ShowLogLevel', g_nSelGate_ShowLogLevel);
  //IniGameConf.WriteInteger('SelGate', 'MaxConnOfIPaddr', g_nSelGate_MaxConnOfIPaddr);
  IniGameConf.WriteInteger('SelGate', 'BlockMethod', g_nSelGate_BlockMethod);
  IniGameConf.WriteInteger('SelGate', 'KeepConnectTimeOut', g_nSelGate_KeepConnectTimeOut);  }
  IniGameConf.Free;
end;

procedure TfrmMain.GenMutLoginGateConfig(nIndex: Integer);
var
  IniGameConf: Tinifile;
  sIniFile: string;
  sGateAddr: string;
  nGatePort: Integer;
  sServerAddr: string;
begin
  sIniFile := g_sGameDirectory + g_sLoginGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  if g_boDoubleLineMode then
  begin
    case nIndex of
      0:
        begin
          sGateAddr := g_sExtIPaddr;
          nGatePort := g_nLoginGate_GatePort;
          sServerAddr := g_sLoginGate_ServerAddr;
        end;
      1:
        begin
          sGateAddr := g_sExtNetComIPaddr;
          nGatePort := g_nLoginGate_GatePort;
          sServerAddr := g_sLoginGate_ServerAddr1;
        end;
    end;
  end
  else
  begin
    sGateAddr := g_sAllIPaddr;
    nGatePort := g_nLoginGate_GatePort;
    sServerAddr := g_sLoginGate_ServerAddr;
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sLoginGate_ConfigFile);
  IniGameConf.WriteString('LoginGate', 'Title', g_sGameName);

  IniGameConf.WriteString('LoginGate', 'ServerAddr', sServerAddr);
  IniGameConf.WriteInteger('LoginGate', 'ServerPort', g_nLoginServer_GatePort);
  IniGameConf.WriteString('LoginGate', 'GateAddr', sGateAddr);
  IniGameConf.WriteInteger('LoginGate', 'GatePort', nGatePort);
  (*
  IniGameConf.WriteInteger('LoginGate', 'Count', 1);
  IniGameConf.WriteString('LoginGate', 'ServerAddr1', g_sLoginGate_ServerAddr);
  IniGameConf.WriteInteger('LoginGate', 'ServerPort1', g_nLoginServer_GatePort {g_nLoginGate_ServerPort});
  IniGameConf.WriteInteger('LoginGate', 'GatePort1', g_nLoginGate_GatePort);
  *)
  
  IniGameConf.Free;
end;


procedure TfrmMain.GetMutLoginGateZero;               // 2019-09-28 16:18:10
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  sIniFile := g_sGameDirectory + g_sSelGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sLoginGate_ConfigFile);
  IniGameConf.WriteInteger('LoginGate', 'Count', 0);
  IniGameConf.Free;
end;

procedure TfrmMain.GenMutSelGateConfigEx;       // 2019-09-28 16:18:04
var
  IniGameConf: Tinifile;
  sIniFile: string;
  //sGateAddr: string;
  {nGatePort,} SelGateIndex: Integer;
  //sServerAddr: string;
begin
  sIniFile := g_sGameDirectory + g_sSelGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sSelGate_ConfigFile);
  SelGateIndex := 0;
  if SelGate.boGetStart then
  begin
    Inc(SelGateIndex);
    IniGameConf.WriteString('SelGates_iocp', 'ServerAddr' + IntToStr(SelGateIndex), g_sSelGate_ServerAddr);
    IniGameConf.WriteInteger('SelGates_iocp', 'ServerPort' + IntToStr(SelGateIndex), g_nDBServer_Config_GatePort);
    IniGameConf.WriteInteger('SelGates_iocp', 'GatePort' + IntToStr(SelGateIndex), g_nSeLGate_GatePort);
  end;

  if SelGate1.boGetStart then
  begin
    Inc(SelGateIndex);
    IniGameConf.WriteString('SelGates_iocp', 'ServerAddr' + IntToStr(SelGateIndex), g_sSelGate_ServerAddr);
    IniGameConf.WriteInteger('SelGates_iocp', 'ServerPort' + IntToStr(SelGateIndex), g_nDBServer_Config_GatePort);
    IniGameConf.WriteInteger('SelGates_iocp', 'GatePort' + IntToStr(SelGateIndex), g_nSeLGate_GatePort1);
  end;

  IniGameConf.WriteInteger('SelGates_iocp', 'Count', SelGateIndex);

  IniGameConf.Free;
end;

procedure TfrmMain.GetMutSelGateZero;               // 2019-09-28 16:18:10
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  sIniFile := g_sGameDirectory + g_sSelGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sSelGate_ConfigFile);
  IniGameConf.WriteInteger('SelGates_iocp', 'Count', 0);
  IniGameConf.Free;
end;

procedure TfrmMain.GenMutSelGateConfig(nIndex: Integer);
var
  IniGameConf: Tinifile;
  sIniFile: string;
  sGateAddr: string;
  nGatePort: Integer;
  sServerAddr: string;
begin
  sIniFile := g_sGameDirectory + g_sSelGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  case nIndex of                                                                                    //
    0:
      begin
        sGateAddr := g_sSelGate_GateAddr;
        nGatePort := g_nSeLGate_GatePort;
        sServerAddr := g_sSelGate_ServerAddr;
      end;
    1:
      begin
        sGateAddr := g_sSelGate_GateAddr1;
        nGatePort := g_nSeLGate_GatePort1;
        sServerAddr := g_sSelGate_ServerAddr;
      end;
  end;
  IniGameConf := Tinifile.Create(sIniFile + g_sSelGate_ConfigFile);
  IniGameConf.WriteString('SelGate', 'Title', g_sGameName);

  IniGameConf.WriteString('SelGate', 'ServerAddr', sServerAddr);
  IniGameConf.WriteInteger('SelGate', 'ServerPort', g_nDBServer_Config_GatePort {g_nSelGate_ServerPort});
  IniGameConf.WriteString('SelGate', 'GateAddr', sGateAddr);
  IniGameConf.WriteInteger('SelGate', 'GatePort', nGatePort);
  IniGameConf.Free;
end;

procedure TfrmMain.GetMutRunGateConfing(nGateIndex: Integer);
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  if (nGateIndex >= 0) and (nGateIndex < MAXRUNGATECOUNT) then
  begin
    sIniFile := g_sGameDirectory + g_sRunGate_Directory;
    if not DirectoryExists(sIniFile) then
    begin
      CreateDir(sIniFile);
    end;
    IniGameConf := Tinifile.Create(sIniFile + g_sRunGate_ConfigFile);
    IniGameConf.WriteString('GameGate', 'Title', g_sGameName + '(' + IntToStr(g_RunGateInfo[nGateIndex].nGatePort) + ')');
    IniGameConf.WriteString('GameGate', 'ServerAddr', g_sRunGate_ServerAddr);
    IniGameConf.WriteInteger('GameGate', 'ServerPort', g_nM2Server_GatePort);
    IniGameConf.WriteString('GameGate', 'GateAddr', g_sAllIPaddr);                                   //g_RunGateInfo[nGateIndex].sGateAddr
    IniGameConf.WriteInteger('GameGate', 'GatePort', g_RunGateInfo[nGateIndex].nGatePort);
    IniGameConf.WriteInteger('GameGate', 'DBPort', g_RunGateInfo[nGateIndex].nDBPort);
    IniGameConf.Free;
  end;
end;

procedure TfrmMain.GetMutRunGateConfingEx;
var
  IniGameConf: Tinifile;
  sIniFile, PortStr: string;
  I, RunGateIndex: Integer;
begin
  sIniFile := g_sGameDirectory + g_sRunGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sRunGate_ConfigFile);
  RunGateIndex := 0;
  PortStr := '';
  for I := Low(g_RunGateInfo) to High(g_RunGateInfo) do
  begin
    if g_RunGateInfo[I].boGetStart then
    begin
      Inc(RunGateIndex);
      IniGameConf.WriteInteger('GameGates', 'Port' + IntToStr(RunGateIndex), g_RunGateInfo[I].nGatePort);
      PortStr := PortStr + IntToStr(g_RunGateInfo[I].nGatePort) + ',';
    end;
  end;
  if Length(PortStr) > 0 then
    PortStr := '(' + Copy(PortStr, 1, Length(PortStr) - 1) + ')';

  IniGameConf.WriteString('GameGate', 'Title', PortStr);
  IniGameConf.WriteInteger('GameGates', 'Count', RunGateIndex);

  IniGameConf.WriteString('GameGate', 'ServerAddr', g_sRunGate_ServerAddr);
  IniGameConf.WriteInteger('GameGate', 'ServerPort', g_nM2Server_GatePort);
  IniGameConf.WriteString('GameGate', 'GateAddr', g_sAllIPaddr);
  IniGameConf.WriteInteger({'Server'RunGate}'GameGate', 'DBPort', g_nRunGateDBPort_MulThread);

  IniGameConf.Free;
end;

procedure TfrmMain.GetMutRunGateZero;
var
  IniGameConf: Tinifile;
  sIniFile: string;
begin
  sIniFile := g_sGameDirectory + g_sRunGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;

  IniGameConf := Tinifile.Create(sIniFile + g_sRunGate_ConfigFile);
  IniGameConf.WriteInteger('GameGates', 'Count', 0);
  IniGameConf.Free;
end;

procedure TfrmMain.GenRunGateConfig;
var
  I{, nIndex}: Integer;
  IniGameConf: Tinifile;
  sIniFile: string;
  SaveList: TStringList;
begin
  sIniFile := g_sGameDirectory + g_sRunGate_Directory;
  if not DirectoryExists(sIniFile) then
  begin
    CreateDir(sIniFile);
  end;
  IniGameConf := Tinifile.Create(sIniFile + g_sRunGate_ConfigFile);
  IniGameConf.WriteString('GameGate', 'Title', g_sGameName);
  g_nRunGate_Count := 0;
  //nIndex := 0;

  { TODO -opiaoyun -c修改 : 运行网关参数配置，先配置一个~~~ 【2013-09-07】 }
  IniGameConf.WriteString({'Server'RunGate}'GameGate', 'ServerAddr', g_sRunGate_ServerAddr);
  IniGameConf.WriteInteger({'Server'RunGate}'GameGate', 'ServerPort', g_nM2Server_GatePort);
  IniGameConf.WriteString({'Server'RunGate}'GameGate', 'GateAddr', g_sAllIPaddr);           //g_RunGateInfo[I].sGateAddr
  IniGameConf.WriteInteger({'Server'RunGate}'GameGate', 'GatePort', g_RunGateInfo[0].nGatePort);

  if g_boRunGate_GetMultiThread then
    IniGameConf.WriteInteger({'Server'RunGate}'GameGate', 'DBPort', g_nRunGateDBPort_MulThread)
  else
    IniGameConf.WriteInteger({'Server'RunGate}'GameGate', 'DBPort', g_RunGateInfo[0].nDBPort);
  IniGameConf.Free;

  SaveList := TStringList.Create;
  try
    SaveList.Add(g_sDBServer_Config_ServerAddr);
    SaveList.SaveToFile(sIniFile + g_sRunGate_AddrTableFile);
  finally
    SaveList.Free;
  end;

  Inc(g_nRunGate_Count);
  //Inc(nIndex);

  // 配置其他网关
  for I := 1 to Length(g_RunGateInfo) - 1 do
  begin
    if g_RunGateInfo[I].boGetStart then
    begin
      Inc(g_nRunGate_Count);

      sIniFile := g_sGameDirectory + Format(g_sRunGate_DirectoryEx,[i]);
      // 没有目录则跳过
      if DirectoryExists(sIniFile) then
      begin
        IniGameConf := Tinifile.Create(sIniFile + g_sRunGate_ConfigFile);
        try
          IniGameConf.WriteString('GameGate', 'Title', g_sGameName);
          IniGameConf.WriteString({'RunGate'}'GameGate' , 'ServerAddr', g_sRunGate_ServerAddr);
          IniGameConf.WriteInteger({'RunGate'}'GameGate', 'ServerPort', g_nM2Server_GatePort);
          IniGameConf.WriteString({'RunGate'}'GameGate', 'GateAddr', g_sAllIPaddr);           //g_RunGateInfo[I].sGateAddr
          IniGameConf.WriteInteger({'RunGate'}'GameGate', 'GatePort', g_RunGateInfo[I].nGatePort);

          if g_boRunGate_GetMultiThread then
            IniGameConf.WriteInteger('GameGates', 'DBPort', g_nRunGateDBPort_MulThread)
          else
            IniGameConf.WriteInteger({'RunGate'}'GameGate', 'DBPort', g_RunGateInfo[I].nDBPort);
        finally
          IniGameConf.Free;
        end;
      end;
    end;
  end;
end;

procedure TfrmMain.RefGameConsole;
begin
  m_boOpen := False;
  //EditLoginSrvProgram.Text := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_ProgramFile;
  //EditLogServerProgram.Text := g_sGameDirectory + g_sLogServer_Directory + g_sLogServer_ProgramFile;
  //EditLoginGateProgram.Text := g_sGameDirectory + g_sLoginGate_Directory + g_sLoginGate_ProgramFile;
  //EditSelGateProgram.Text := g_sGameDirectory + g_sSelGate_Directory + g_sSelGate_ProgramFile;
  //EditRunGateProgram.Text := g_sGameDirectory + g_sRunGate_Directory + g_sRunGate_ProgramFile;
  //EditRunGate1Program.Text := g_sGameDirectory + g_sRunGate_Directory + g_sRunGate_ProgramFile;
  //EditRunGate2Program.Text := g_sGameDirectory + g_sRunGate_Directory + g_sRunGate_ProgramFile;

  chkEmbeddedWindow.Checked := g_boEmbeddedWindow;

  CheckBoxM2Server.Checked := g_boM2Server_GetStart;
  CheckBoxM2Server.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sM2Server_Directory, g_sM2Server_ProgramFile]);

  CheckBoxDBServer.Checked := g_boDBServer_GetStart;
  CheckBoxDBServer.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sDBServer_Directory, g_sDBServer_ProgramFile]);

  CheckBoxLoginServer.Checked := g_boLoginServer_GetStart;
  CheckBoxLoginServer.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sLoginServer_Directory, g_sLoginServer_ProgramFile]);

  CheckBoxLogServer.Checked := g_boLogServer_GetStart;
  CheckBoxLogServer.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sLogServer_Directory, g_sLogServer_ProgramFile]);

  CheckBoxLoginGate.Checked := g_boLoginGate_GetStart;
  CheckBoxLoginGate.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sLoginGate_Directory, g_sLoginGate_ProgramFile]);

  CheckBoxSelGate.Checked := g_boSelGate_GetStart;
  CheckBoxSelGate.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sSelGate_Directory, g_sSelGate_ProgramFile]);

  CheckBoxSelGate1.Checked := g_boSelGate_GetStart1;
  CheckBoxSelGate1.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sSelGate_Directory, g_sSelGate_ProgramFile]);

  CheckBoxRunGate.Checked := g_RunGateInfo[0].boGetStart;
  CheckBoxRunGate.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, g_sRunGate_Directory, g_sRunGate_ProgramFile]);

  CheckBoxRunGate1.Checked := g_RunGateInfo[1].boGetStart;
  CheckBoxRunGate1.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[1]), g_sRunGate_ProgramFile]);

  CheckBoxRunGate2.Checked := g_RunGateInfo[2].boGetStart;
  CheckBoxRunGate2.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[2]), g_sRunGate_ProgramFile]);

  CheckBoxRunGate3.Checked := g_RunGateInfo[3].boGetStart;
  CheckBoxRunGate3.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[3]), g_sRunGate_ProgramFile]);

  CheckBoxRunGate4.Checked := g_RunGateInfo[4].boGetStart;
  CheckBoxRunGate4.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[4]), g_sRunGate_ProgramFile]);

  CheckBoxRunGate5.Checked := g_RunGateInfo[5].boGetStart;
  CheckBoxRunGate5.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[5]), g_sRunGate_ProgramFile]);

  CheckBoxRunGate6.Checked := g_RunGateInfo[6].boGetStart;
  CheckBoxRunGate6.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[6]), g_sRunGate_ProgramFile]);

  CheckBoxRunGate7.Checked := g_RunGateInfo[7].boGetStart;
  CheckBoxRunGate7.Hint := Format('程序所在位置: %s%s%s', [g_sGameDirectory, Format(g_sRunGate_DirectoryEx,[7]), g_sRunGate_ProgramFile]);

  GenDBServerConfig;

  EditGameDir.Text := g_sGameDirectory;


  rbBDE.Checked := not g_boUseSqliteDB;
  rbSqlite.Checked := g_boUseSqliteDB;
  EditHeroDB.Enabled := not g_boUseSqliteDB;
  edtSqliteDB.Enabled := g_boUseSqliteDB;

  EditHeroDB.Text := g_sHeroDBName;
  edtSqliteDB.Text := g_sSqliteDBName;

  rbDataSaveMySql.Checked := g_nDataSaveDBType = 1;
  rbDataSaveSqlite.Checked := g_nDataSaveDBType <> 1;
  edtDataSaveDBServer.Text := g_sDataSaveDBServer;
  seDataSaveDBPort.Value := g_wDataSaveDBPort;
  edtDataSaveDBUser.Text := g_sDataSaveDBUser;
  edtDataSaveDBPassword.Text := g_sDataSaveDBPassword;
  edtDataSaveDataBase.Text := g_sDataSaveDataBase;

  grpDataSaveMySql.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDBServer.Enabled := rbDataSaveMySql.Checked;
  seDataSaveDBPort.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDBUser.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDBPassword.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDataBase.Enabled := rbDataSaveMySql.Checked;

  lblMySqlLinkTest.Enabled := rbDataSaveMySql.Checked;
  lblMySqlDoInit.Enabled := rbDataSaveMySql.Checked;


  EditGameName.Text := g_sGameName;
  EditGameExtIPaddr.Text := g_sExtIPaddr;
  EditGameExtNetComIPaddr.Text := g_sExtNetComIPaddr;
  chkDoubleLineMode.Checked := g_boDoubleLineMode;
  // 增加动态IP支持 piaoyun 2013-08-30
  chkDynamicIPMode.Checked := g_boDynamicIPMode;
  EditGameExtIPaddr.Enabled := not chkDynamicIPMode.Checked;
  EditGameExtNetComIPaddr.Enabled := not chkDynamicIPMode.Checked;
  //////////////////////////////////////////////////////////////////////////////

  EditGameExtNetComIPaddr.Visible := g_boDoubleLineMode;
  LabelNetComIPaddr.Visible := g_boDoubleLineMode;

  EditLoginGate_MainFormX.Value := g_nLoginGate_MainFormX;
  EditLoginGate_MainFormY.Value := g_nLoginGate_MainFormY;
  CheckBoxboLoginGate_GetStart.Checked := g_boLoginGate_GetStart;
  CheckBoxboLoginGate_GetMinimize.Checked := g_boLoginGate_GetMinimize;
  EditLoginGate_GatePort.Text := IntToStr(g_nLoginGate_GatePort);

  EditSelGate_MainFormX.Value := g_nSelGate_MainFormX;
  EditSelGate_MainFormY.Value := g_nSelGate_MainFormY;
  CheckBoxboSelGate_GetStart.Checked := g_boSelGate_GetStart;
  CheckBoxboSelGate_GetStart1.Checked := g_boSelGate_GetStart1;
  CheckBoxboSelGate_GetMinimize.Checked := g_boSelGate_GetMinimize;
  EditSelGate_GatePort.Text := IntToStr(g_nSeLGate_GatePort);
  EditSelGate_GatePort1.Text := IntToStr(g_nSeLGate_GatePort1);
  chkSelGate_GetMultiThread.Checked := g_boSelGate_GetMultiThread;      //2019-09-28 15:45:01

  //CheckBoxboRunGate_GetStart.Checked := g_boRunGate_GetStart;
  CheckBoxboRunGate_GetMinimize.Checked := g_boRunGate_GetMinimize;
  CheckBoxboRunGate_GetMultiThread.Checked := g_boRunGate_GetMultiThread;

  EditRunGate_Connt.Value := g_nRunGate_Count;
  EditRunGate_GatePort1.Text := IntToStr(g_RunGateInfo[0].nGatePort);
  EditRunGate_GatePort2.Text := IntToStr(g_RunGateInfo[1].nGatePort);
  EditRunGate_GatePort3.Text := IntToStr(g_RunGateInfo[2].nGatePort);
  EditRunGate_GatePort4.Text := IntToStr(g_RunGateInfo[3].nGatePort);
  EditRunGate_GatePort5.Text := IntToStr(g_RunGateInfo[4].nGatePort);
  EditRunGate_GatePort6.Text := IntToStr(g_RunGateInfo[5].nGatePort);
  EditRunGate_GatePort7.Text := IntToStr(g_RunGateInfo[6].nGatePort);
  EditRunGate_GatePort8.Text := IntToStr(g_RunGateInfo[7].nGatePort);

  edtRunGate_DBPort1.Text := IntToStr(g_RunGateInfo[0].nDBPort);
  edtRunGate_DBPort2.Text := IntToStr(g_RunGateInfo[1].nDBPort);
  edtRunGate_DBPort3.Text := IntToStr(g_RunGateInfo[2].nDBPort);
  edtRunGate_DBPort4.Text := IntToStr(g_RunGateInfo[3].nDBPort);
  edtRunGate_DBPort5.Text := IntToStr(g_RunGateInfo[4].nDBPort);
  edtRunGate_DBPort6.Text := IntToStr(g_RunGateInfo[5].nDBPort);
  edtRunGate_DBPort7.Text := IntToStr(g_RunGateInfo[6].nDBPort);
  edtRunGate_DBPort8.Text := IntToStr(g_RunGateInfo[7].nDBPort);

  edtRunGate_DBPortMulThread.Text := IntToStr(g_nRunGateDBPort_MulThread);

  EditLoginServer_MainFormX.Value := g_nLoginServer_MainFormX;
  EditLoginServer_MainFormY.Value := g_nLoginServer_MainFormY;
  EditLoginServerGatePort.Text := IntToStr(g_nLoginServer_GatePort);
  EditLoginServerServerPort.Text := IntToStr(g_nLoginServer_ServerPort);
  EditLoginServerControlPort.Text := IntToStr(g_nLoginServer_ControlPort);
  CheckBoxboLoginServer_GetStart.Checked := g_boLoginServer_GetStart;
  CheckBoxboLoginServer_GetMinimize.Checked := g_boLoginServer_GetMinimize;

  EditDBServer_MainFormX.Value := g_nDBServer_MainFormX;
  EditDBServer_MainFormY.Value := g_nDBServer_MainFormY;
  EditDBServerGatePort.Text := IntToStr(g_nDBServer_Config_GatePort);
  EditDBServerServerPort.Text := IntToStr(g_nDBServer_Config_ServerPort);
  //CheckBoxDisableAutoGame.Checked := g_boDBServer_DisableAutoGame;
  CheckBoxDBServerGetStart.Checked := g_boDBServer_GetStart;
  CheckBoxDBServerGetMinimize.Checked := g_boDBServer_GetMinimize;

  EditLogServer_MainFormX.Value := g_nLogServer_MainFormX;
  EditLogServer_MainFormY.Value := g_nLogServer_MainFormY;
  EditLogServerPort.Text := IntToStr(g_nLogServer_Port);
  CheckBoxLogServerGetStart.Checked := g_boLogServer_GetStart;
  CheckBoxLogServerGetMinimize.Checked := g_boLogServer_GetMinimize;

  EditM2Server_MainFormX.Value := g_nM2Server_MainFormX;
  EditM2Server_MainFormY.Value := g_nM2Server_MainFormY;
  EditM2Server_TestLevel.Value := g_nM2Server_TestLevel;
  EditM2Server_TestGold.Value := g_nM2Server_TestGold;
  EditM2ServerGatePort.Text := IntToStr(g_nM2Server_GatePort);
  EditM2ServerMsgSrvPort.Text := IntToStr(g_nM2Server_MsgSrvPort);

  CheckBoxM2ServerGetStart.Checked := g_boM2Server_GetStart;
  CheckBoxM2ServerGetMinimize.Checked := g_boM2Server_GetMinimize;

  //CheckBoxAutoStartBackUpServer.Checked := g_boAutoStartBackUpServer;
  chkAutoStartServer.Checked := g_boAutoStartServer;
  EditAutoStartDelayTime.Value := g_nAutoStartDelayTime;
  TimerAutoStartServer.Enabled := g_boAutoStartServer;

  m_boOpen := True;
end;

procedure TfrmMain.CheckBoxDBServerClick(Sender: TObject);
begin
  g_boDBServer_GetStart := CheckBoxDBServer.Checked;
end;

procedure TfrmMain.CheckBoxLoginServerClick(Sender: TObject);
begin
  g_boLoginServer_GetStart := CheckBoxLoginServer.Checked;
end;

procedure TfrmMain.CheckBoxM2ServerClick(Sender: TObject);
begin
  g_boM2Server_GetStart := CheckBoxM2Server.Checked;
end;

procedure TfrmMain.CheckBoxLogServerClick(Sender: TObject);
begin
  g_boLogServer_GetStart := CheckBoxLogServer.Checked;
end;

procedure TfrmMain.CheckBoxLoginGateClick(Sender: TObject);
begin
  g_boLoginGate_GetStart := CheckBoxLoginGate.Checked;
end;

procedure TfrmMain.CheckBoxSelGateClick(Sender: TObject);
begin
  g_boSelGate_GetStart := CheckBoxSelGate.Checked;
end;

procedure TfrmMain.CheckBoxRunGateClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_RunGateInfo[TCheckBox(Sender).Tag].boGetStart := TCheckBox(Sender).Checked;
  GenRunGateConfig();
  GenDBServerConfig();
end;

procedure TfrmMain.ButtonStartGameClick(Sender: TObject);
var
  sFileName: string;
  Config: TIniFile;
  boFirstRun: Boolean;
begin
  SetWindowPos(Self.Handle, Self.Handle, Self.Left, Self.Top, Self.Width, Self.Height, SWP_SHOWWINDOW);
  case m_nStartStatus of
    0:
      begin
        boFirstRun := False;

        sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConfigFile;
        if FileExists(sFileName) then
        begin
          Config := TIniFile.Create(sFileName);
          boFirstRun := Config.ReadBool('Setup', 'FirstRun', True);
          Config.Free;
        end;

        if boFirstRun then
        begin
          if Application.MessageBox('第一次运行本程序，是否清理服务端数据?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
          begin
            PageControl1.ActivePageIndex := 5;
            Exit;
          end;
        end;

        if Application.MessageBox('是否确认启动游戏服务器?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
        begin
          chkEmbeddedWindow.Enabled := False;
          StartGame();
          if boFirstRun then
          begin
            sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConfigFile;
            if FileExists(sFileName) then
            begin
              Config := TIniFile.Create(sFileName);
              Config.WriteBool('Setup', 'FirstRun', False);
              Config.Free;
            end;
          end;
        end;
      end;
    1:
      begin
        if Application.MessageBox('是否确认中止启动游戏服务器?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
        begin
          TimerStartGame.Enabled := False;
          m_nStartStatus := 2;
          ButtonStartGame.Caption := g_sButtonStopGame;
        end;
      end;
    2:
      begin
        if Application.MessageBox('是否确认停止游戏服务器?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
        begin
          StopGame();
        end;
      end;
    3:
      begin
        if Application.MessageBox('是否确认中止启动游戏服务器?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
        begin
          TimerStopGame.Enabled := False;
          m_nStartStatus := 2;
          ButtonStartGame.Caption := g_sButtonStopGame;
        end;
      end;
  end;
end;

procedure TfrmMain.StartGame;
var
  I: Integer;
begin
  if not g_boHeroDBOK then Exit;
  FillChar(DBServer, SizeOf(TProgram), #0);
  DBServer.boGetStart := g_boDBServer_GetStart;
  DBServer.boMinimize := g_boDBServer_GetMinimize;
  DBServer.boReStart := True;
  DBServer.sDirectory := g_sGameDirectory + g_sDBServer_Directory;
  DBServer.sProgramFile := g_sDBServer_ProgramFile;
  DBServer.nMainFormX := g_nDBServer_MainFormX;
  DBServer.nMainFormY := g_nDBServer_MainFormY;


  FillChar(LoginServer, SizeOf(TProgram), #0);
  LoginServer.boGetStart := g_boLoginServer_GetStart;
  LoginServer.boMinimize := g_boLoginServer_GetMinimize;
  LoginServer.boReStart := True;
  LoginServer.sDirectory := g_sGameDirectory + g_sLoginServer_Directory;
  LoginServer.sProgramFile := g_sLoginServer_ProgramFile;
  LoginServer.nMainFormX := g_nLoginServer_MainFormX;
  LoginServer.nMainFormY := g_nLoginServer_MainFormY;

  FillChar(LogServer, SizeOf(TProgram), #0);
  LogServer.boGetStart := g_boLogServer_GetStart;
  LogServer.boMinimize := g_boLogServer_GetMinimize;
  LogServer.boReStart := True;
  LogServer.sDirectory := g_sGameDirectory + g_sLogServer_Directory;
  LogServer.sProgramFile := g_sLogServer_ProgramFile;
  LogServer.nMainFormX := g_nLogServer_MainFormX;
  LogServer.nMainFormY := g_nLogServer_MainFormY;

  FillChar(M2Server, SizeOf(TProgram), #0);
  M2Server.boGetStart := g_boM2Server_GetStart;
  M2Server.boMinimize := g_boM2Server_GetMinimize;
  M2Server.boReStart := True;
  M2Server.sDirectory := g_sGameDirectory + g_sM2Server_Directory;
  M2Server.sProgramFile := g_sM2Server_ProgramFile;
  M2Server.nMainFormX := g_nM2Server_MainFormX;
  M2Server.nMainFormY := g_nM2Server_MainFormY;

  FillChar(RunGate, SizeOf(RunGate), #0);
  for I := Low(RunGate) to High(RunGate) do
  begin
    RunGate[I].btStartStatus := 0;
    RunGate[I].boGetStart := g_RunGateInfo[I].boGetStart;
    RunGate[I].boMinimize := g_boRunGate_GetMinimize;
    RunGate[I].boReStart := True;
    RunGate[I].sDirectory := g_sGameDirectory + g_sRunGate_Directory;
    RunGate[I].sProgramFile := g_sRunGate_ProgramFile;
  end;

  FillChar(SelGate, SizeOf(TProgram), #0);
  SelGate.boGetStart := g_boSelGate_GetStart;
  SelGate.boMinimize := g_boSelGate_GetMinimize;
  SelGate.boReStart := True;
  SelGate.sDirectory := g_sGameDirectory + g_sSelGate_Directory;
  SelGate.sProgramFile := g_sSelGate_ProgramFile;
  SelGate.nMainFormX := g_nSelGate_MainFormX;
  SelGate.nMainFormY := g_nSelGate_MainFormY;


  FillChar(SelGate1, SizeOf(TProgram), #0);
  SelGate1.boGetStart := g_boSelGate_GetStart1;
  SelGate1.boMinimize := g_boSelGate_GetMinimize;
  SelGate1.boReStart := True;
  SelGate1.sDirectory := g_sGameDirectory + g_sSelGate_Directory;
  SelGate1.sProgramFile := g_sSelGate_ProgramFile;
  SelGate1.nMainFormX := g_nSelGate_MainFormX;
  SelGate1.nMainFormY := g_nSelGate_MainFormY;


  FillChar(LoginGate, SizeOf(TProgram), #0);
  LoginGate.boGetStart := g_boLoginGate_GetStart;
  LoginGate.boMinimize := g_boLoginGate_GetMinimize;
  LoginGate.boReStart := True;
  LoginGate.sDirectory := g_sGameDirectory + g_sLoginGate_Directory;
  LoginGate.sProgramFile := g_sLoginGate_ProgramFile;
  LoginGate.nMainFormX := g_nLoginGate_MainFormX;
  LoginGate.nMainFormY := g_nLoginGate_MainFormY;

  FillChar(LoginGate1, SizeOf(TProgram), #0);
  LoginGate1.boGetStart := g_boLoginGate_GetStart and g_boDoubleLineMode;
  LoginGate1.boMinimize := g_boLoginGate_GetMinimize;
  LoginGate1.boReStart := True;
  LoginGate1.sDirectory := g_sGameDirectory + g_sLoginGate_Directory;
  LoginGate1.sProgramFile := g_sLoginGate_ProgramFile;
  LoginGate1.nMainFormX := g_nLoginGate_MainFormX;
  LoginGate1.nMainFormY := g_nLoginGate_MainFormY;


  ButtonStartGame.Caption := g_sButtonStopStartGame;
  m_nStartStatus := 1;
  TimerStartGame.Enabled := True;
end;

procedure TfrmMain.StopGame;
begin
  ButtonStartGame.Caption := g_sButtonStopStopGame;
  MainOutMessage('正在开始停止服务器...');
  TimerCheckRun.Enabled := False;
  TimerStopGame.Enabled := True;
  m_nStartStatus := 3;
end;

procedure TfrmMain.TimerStartGameTimer(Sender: TObject);
var
  I, nRetCode: Integer;
begin
  if DBServer.boGetStart then
  begin
    case DBServer.btStartStatus of
      0:
        begin
          nRetCode := RunProgram(DBServer, IntToStr(Self.Handle), 0);
          if nRetCode = 0 then
          begin
            DBServer.btStartStatus := 1;
            DBServer.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, DBServer.ProcessInfo.dwProcessId);
          end else
          begin

          end;
          Exit;
        end;
      1:
        begin                                                                                       //如果状态为1 则还没启动完成
          //        DBServer.btStartStatus:=2;
          Exit;
        end;
    end;
  end;
  if LoginServer.boGetStart then
  begin
    case LoginServer.btStartStatus of                                                               //
      0:
        begin
          nRetCode := RunProgram(LoginServer, IntToStr(Self.Handle), 0);
          if nRetCode = 0 then
          begin
            LoginServer.btStartStatus := 1;
            LoginServer.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LoginServer.ProcessInfo.dwProcessId);
          end else
          begin
            LoginServer.btStartStatus := 9;
          end;
          Exit;
        end;
      1:
        begin                                                                                       //如果状态为1 则还没启动完成
          //        LoginServer.btStartStatus:=2;
          Exit;
        end;
    end;
  end;

  if LogServer.boGetStart then
  begin
    case LogServer.btStartStatus of                                                                 //
      0:
        begin
          nRetCode := RunProgram(LogServer, IntToStr(Self.Handle), 0);
          if nRetCode = 0 then
          begin
            LogServer.btStartStatus := 1;
            LogServer.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LogServer.ProcessInfo.dwProcessId);
          end else
          begin
            LogServer.btStartStatus := 9;
          end;
          Exit;
        end;
      1:
        begin                                                                                       //如果状态为1 则还没启动完成
          //        LogServer.btStartStatus:=2;
          Exit;
        end;
    end;
  end;

  if M2Server.boGetStart then
  begin
    case M2Server.btStartStatus of                                                                  //
      0:
        begin
          nRetCode := RunProgram(M2Server, IntToStr(Self.Handle), 0);
          if nRetCode = 0 then
          begin
            M2Server.btStartStatus := 1;
            M2Server.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, M2Server.ProcessInfo.dwProcessId);
          end else
          begin
            M2Server.btStartStatus := 9;
          end;
          Exit;
        end;
      1:
        begin                                                                                       //如果状态为1 则还没启动完成
          //        M2Server.btStartStatus:=2;
          Exit;
        end;
    end;
  end;

  if g_boRunGate_GetMultiThread then
  begin
    // add chongchong 多线程网关配置生成 2014-06-22
    GetMutRunGateConfingEx;
    if RunGate[0].boGetStart then
    begin
      case RunGate[0].btStartStatus of                                                              //
        0:
          begin
            nRetCode := RunProgram(RunGate[0], IntToStr(Self.Handle), 0);
            if nRetCode = 0 then
            begin
              RunGate[0].btStartStatus := 1;
              RunGate[0].ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, RunGate[0].ProcessInfo.dwProcessId);
            end else
            begin
              RunGate[0].btStartStatus := 9;
            end;
            Exit;
          end;
        1:
          begin                                                                                     //如果状态为1 则还没启动完成
          //        RunGate.btStartStatus:=2;
            Exit;
          end;
      end;
    end;
  end
  else
  begin
    GetMutRunGateZero;
    for I := Low(RunGate) to High(RunGate) do
    begin
      if RunGate[I].boGetStart and (RunGate[I].btStartStatus = 1) then
      begin
        Exit;
      end;
    end;

    for I := Low(RunGate) to High(RunGate) do
    begin
      if RunGate[I].boGetStart then
      begin
        if RunGate[I].btStartStatus = 0 then
        begin
          GetMutRunGateConfing(I);
          nRetCode := RunProgram(RunGate[I], IntToStr(Self.Handle), 0);
          if nRetCode = 0 then
          begin
            RunGate[I].btStartStatus := 1;
            RunGate[I].ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, RunGate[I].ProcessInfo.dwProcessId);
          end else
          begin
            RunGate[I].btStartStatus := 9;
          end;
          //MainOutMessage('RunGate:'+IntToStr(I));
          Exit;
        end;
      end;
    end;
  end;

  if g_boSelGate_GetMultiThread then                                           // 2019-09-28 15:54:17
  begin
    GenMutSelGateConfigEx;

    if SelGate.boGetStart then
    begin
      case SelGate.btStartStatus of                                                                   //
        0:
          begin
            GenMutSelGateConfig(0);
            nRetCode := RunProgram(SelGate, IntToStr(Self.Handle), 0);
            if nRetCode = 0 then
            begin
              SelGate.btStartStatus := 1;
              SelGate.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate.ProcessInfo.dwProcessId);
            end else
            begin
              SelGate.btStartStatus := 9;

            end;
            Exit;
          end;
        1:
          begin                                                                                       //如果状态为1 则还没启动完成
            //        SelGate.btStartStatus:=2;
            Exit;
          end;
      end;
    end
    else if SelGate1.boGetStart then
    begin
      case SelGate1.btStartStatus of                                                                  //
        0:
          begin
            GenMutSelGateConfig(1);
            nRetCode := RunProgram(SelGate1, IntToStr(Self.Handle), 0, 0);
            if nRetCode = 0 then
            begin
              SelGate1.btStartStatus := 1;
              SelGate1.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate1.ProcessInfo.dwProcessId);
            end else
            begin
              SelGate1.btStartStatus := 9;

            end;
            Exit;
          end;
        1:
          begin                                                                                       //如果状态为1 则还没启动完成
            //        SelGate1.btStartStatus:=2;
            Exit;
          end;
      end;
    end;
  end
  else
  begin
    GetMutSelGateZero;

    if SelGate.boGetStart then
    begin
      case SelGate.btStartStatus of                                                                   //
        0:
          begin
            GenMutSelGateConfig(0);
            nRetCode := RunProgram(SelGate, IntToStr(Self.Handle), 0);
            if nRetCode = 0 then
            begin
              SelGate.btStartStatus := 1;
              SelGate.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate.ProcessInfo.dwProcessId);
            end else
            begin
              SelGate.btStartStatus := 9;
            end;
            Exit;
          end;
        1:
          begin                                                                                       //如果状态为1 则还没启动完成
            //        SelGate.btStartStatus:=2;
            Exit;
          end;
      end;
    end;

    if SelGate1.boGetStart then
    begin
      case SelGate1.btStartStatus of                                                                  //
        0:
          begin
            GenMutSelGateConfig(1);
            nRetCode := RunProgram(SelGate1, IntToStr(Self.Handle), 0, 1);
            if nRetCode = 0 then
            begin
              SelGate1.btStartStatus := 1;
              SelGate1.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate1.ProcessInfo.dwProcessId);
            end else
            begin
              SelGate1.btStartStatus := 9;

            end;
            Exit;
          end;
        1:
          begin                                                                                       //如果状态为1 则还没启动完成
            //        SelGate1.btStartStatus:=2;
            Exit;
          end;
      end;
    end;
  end;

  if LoginGate.boGetStart then
  begin
    case LoginGate.btStartStatus of                                                                 //
      0:
        begin
          GenMutLoginGateConfig(0);
          nRetCode := RunProgram(LoginGate, IntToStr(Self.Handle), 0);
          if nRetCode = 0 then
          begin
            LoginGate.btStartStatus := 1;
            LoginGate.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LoginGate.ProcessInfo.dwProcessId);
          end else
          begin
            LoginGate.btStartStatus := 9;

          end;
          Exit;
        end;
      1:
        begin                                                                                       //如果状态为1 则还没启动完成
          //        LoginGate.btStartStatus:=2;
          Exit;
        end;
    end;
  end;

  if LoginGate1.boGetStart then
  begin
    case LoginGate1.btStartStatus of                                                                //
      0:
        begin
          GenMutLoginGateConfig(1);
          nRetCode := RunProgram(LoginGate1, IntToStr(Self.Handle), 0, 1);
          if nRetCode = 0 then
          begin
            LoginGate1.btStartStatus := 1;
            LoginGate1.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LoginGate1.ProcessInfo.dwProcessId);
          end else
          begin
            LoginGate1.btStartStatus := 9;

          end;
          Exit;
        end;
      1:
        begin                                                                                       //如果状态为1 则还没启动完成
          //        LoginGate.btStartStatus:=2;
          Exit;
        end;
    end;
  end;

  TimerStartGame.Enabled := False;
  TimerCheckRun.Enabled := True;
  ButtonStartGame.Caption := g_sButtonStopGame;
  m_nStartStatus := 2;

  chkEmbeddedWindow.Enabled := True;

  {if (not g_BackUpManager.Start) and g_boAutoStartBackUpServer then
    ButtonBackStartClick(Self); }
end;

procedure TfrmMain.TimerStopGameTimer(Sender: TObject);
{$J+}
  const nM2DelayTick: LongWord = 0;
{$j-}
var
  dwExitCode: LongWord;
  I: Integer;
begin
  if LoginGate.boGetStart and (LoginGate.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(LoginGate.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if LoginGate.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(LoginGate, 0);
          if g_boDoubleLineMode then
            MainOutMessage('正常关闭超时，登录网关一已被强行停止...')
          else
            MainOutMessage('正常关闭超时，登录网关已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(LoginGate.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      LoginGate.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(LoginGate.ProcessHandle);
      LoginGate.btStartStatus := 0;
      if g_boDoubleLineMode then
        MainOutMessage('登录网关一已停止...')
      else
        MainOutMessage('登录网关已停止...');
    end;
  end;

  if LoginGate1.boGetStart and (LoginGate1.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(LoginGate1.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if LoginGate1.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(LoginGate1, 0);
          MainOutMessage('正常关闭超时，登录网关二已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(LoginGate1.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      LoginGate1.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(LoginGate1.ProcessHandle);
      LoginGate1.btStartStatus := 0;
      MainOutMessage('登录网关二已停止...');
    end;
  end;

  if SelGate.boGetStart and (SelGate.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(SelGate.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if SelGate.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(SelGate, 0);
          if g_boDoubleLineMode then
            MainOutMessage('正常关闭超时，角色网关一已被强行停止...')
          else
            MainOutMessage('正常关闭超时，角色网关已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(SelGate.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      SelGate.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(SelGate.ProcessHandle);
      SelGate.btStartStatus := 0;
      if g_boDoubleLineMode then
        MainOutMessage('角色网关一已停止...')
      else
        MainOutMessage('角色网关已停止...');
    end;
  end;

  if SelGate1.boGetStart and (SelGate1.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(SelGate1.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if SelGate1.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(SelGate1, 0);
          MainOutMessage('正常关闭超时，角色网关二已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(SelGate1.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      SelGate1.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(SelGate1.ProcessHandle);
      SelGate1.btStartStatus := 0;
      MainOutMessage('角色网关二已停止...');
    end;
  end;

  if g_boRunGate_GetMultiThread then
  begin
    if RunGate[0].boGetStart and (RunGate[0].btStartStatus in [2, 3]) then
    begin
      GetExitCodeProcess(RunGate[0].ProcessHandle, dwExitCode);
      if dwExitCode = STILL_ACTIVE then
      begin
        if RunGate[0].btStartStatus = 3 then
        begin
          if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
          begin
            StopProgram(RunGate[0], 0);
            MainOutMessage('正常关闭超时，游戏网关已被强行停止...');
          end;
          Exit;                                                                                     //如果正在关闭则等待，不处理下面
        end;
        SendProgramMsg(RunGate[0].MainFormHandle, GS_QUIT, '');
        g_dwStopTick := GetTickCount();
        RunGate[0].btStartStatus := 3;
        Exit;
      end else
      begin
        CloseHandle(RunGate[0].ProcessHandle);
        RunGate[0].btStartStatus := 0;
        MainOutMessage('游戏网关已停止...');
      end;
    end;
  end else
  begin
    for I := Low(RunGate) to High(RunGate) do
    begin
      if RunGate[I].boGetStart and (RunGate[I].btStartStatus in [2, 3]) then
      begin
        GetExitCodeProcess(RunGate[I].ProcessHandle, dwExitCode);
        if dwExitCode = STILL_ACTIVE then
        begin
          if RunGate[I].btStartStatus = 3 then
          begin
            if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
            begin
              StopProgram(RunGate[I], 0);
              case I of
                0: MainOutMessage('正常关闭超时，游戏网关一已被强行停止...');
                1: MainOutMessage('正常关闭超时，游戏网关二已被强行停止...');
                2: MainOutMessage('正常关闭超时，游戏网关三已被强行停止...');
                3: MainOutMessage('正常关闭超时，游戏网关四已被强行停止...');
                4: MainOutMessage('正常关闭超时，游戏网关五已被强行停止...');
                5: MainOutMessage('正常关闭超时，游戏网关六已被强行停止...');
                6: MainOutMessage('正常关闭超时，游戏网关七已被强行停止...');
                7: MainOutMessage('正常关闭超时，游戏网关八已被强行停止...');
              end;
            end;
            Exit;                                                                                   //如果正在关闭则等待，不处理下面
          end;
          SendProgramMsg(RunGate[I].MainFormHandle, GS_QUIT, '');
          g_dwStopTick := GetTickCount();
          RunGate[I].btStartStatus := 3;
          Exit;
        end else
        begin
          CloseHandle(RunGate[I].ProcessHandle);
          RunGate[I].btStartStatus := 0;
          case I of
            0: MainOutMessage('游戏网关一已停止...');
            1: MainOutMessage('游戏网关二已停止...');
            2: MainOutMessage('游戏网关三已停止...');
            3: MainOutMessage('游戏网关四已停止...');
            4: MainOutMessage('游戏网关五已停止...');
            5: MainOutMessage('游戏网关六已停止...');
            6: MainOutMessage('游戏网关七已停止...');
            7: MainOutMessage('游戏网关八已停止...');
          end;
          Exit;
        end;
      end;
    end;
  end;


  if M2Server.boGetStart and (M2Server.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(M2Server.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if M2Server.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(M2Server, 1000);
          MainOutMessage('正常关闭超时，游戏引擎主程序已被强行停止...');
        end;
        Exit;     //如果正在关闭则等待，不处理下面
      end;
      if nM2DelayTick = 0 then begin
        MainOutMessage('游戏引擎主程序将在3秒后关闭...');
        nM2DelayTick := GetTickCount;
      end else if GetTickCount - nM2DelayTick >= 3000 then begin
        SendProgramMsg(M2Server.MainFormHandle, GS_QUIT, '');
        g_dwStopTick := GetTickCount();
        M2Server.btStartStatus := 3;
      end;
      Exit;
    end else
    begin
      nM2DelayTick := 0;
      CloseHandle(M2Server.ProcessHandle);
      M2Server.btStartStatus := 0;
      MainOutMessage('游戏引擎主程序已停止...');
    end;
  end;

  if LoginServer.boGetStart and (LoginServer.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(LoginServer.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if LoginServer.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(LoginServer, 1000);
          MainOutMessage('正常关闭超时，游戏引擎主程序已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(LoginServer.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      LoginServer.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(LoginServer.ProcessHandle);
      LoginServer.btStartStatus := 0;
      MainOutMessage('登录服务器已停止...');
    end;
  end;

  if LogServer.boGetStart and (LogServer.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(LogServer.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if LogServer.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut then
        begin
          StopProgram(LogServer, 0);
          MainOutMessage('正常关闭超时，游戏引擎主程序已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(LogServer.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      LogServer.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(LogServer.ProcessHandle);
      LogServer.btStartStatus := 0;
      MainOutMessage('游戏日志服务器已停止...');
    end;
  end;

  if DBServer.boGetStart and (DBServer.btStartStatus in [2, 3]) then
  begin
    GetExitCodeProcess(DBServer.ProcessHandle, dwExitCode);
    if dwExitCode = STILL_ACTIVE then
    begin
      if DBServer.btStartStatus = 3 then
      begin
        if GetTickCount - g_dwStopTick > g_dwStopTimeOut * 2 then
        begin
          StopProgram(DBServer, 0);
          MainOutMessage('正常关闭超时，游戏引擎主程序已被强行停止...');
        end;
        Exit;                                                                                       //如果正在关闭则等待，不处理下面
      end;
      SendProgramMsg(DBServer.MainFormHandle, GS_QUIT, '');
      g_dwStopTick := GetTickCount();
      DBServer.btStartStatus := 3;
      Exit;
    end else
    begin
      CloseHandle(DBServer.ProcessHandle);
      DBServer.btStartStatus := 0;
      MainOutMessage('游戏数据库服务器已停止...');
    end;
  end;
  TimerStopGame.Enabled := False;
  ButtonStartGame.Caption := g_sButtonStartGame;
  m_nStartStatus := 0;         
      if FileExists(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-shm') then
      begin
        try
          DeleteFile(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-shm');
        except

        end;  
      end;
      if FileExists(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-wal') then
      begin
        try
          DeleteFile(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-wal');
        except

        end;  
      end;
end;

procedure TfrmMain.TimerCheckRunTimer(Sender: TObject);
var
  dwExitCode: LongWord;
  I, nRetCode: Integer;
begin
  if DBServer.boGetStart then
  begin
    GetExitCodeProcess(DBServer.ProcessHandle, dwExitCode);
    if dwExitCode <> STILL_ACTIVE then
    begin
      nRetCode := RunProgram(DBServer, IntToStr(Self.Handle), 0);

      if nRetCode = 0 then
      begin
        CloseHandle(DBServer.ProcessHandle);
        DBServer.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, DBServer.ProcessInfo.dwProcessId);
        MainOutMessage('数据库异常关闭，已被重新启动...');
      end;
    end;
  end;

  if LoginServer.boGetStart then
  begin
    GetExitCodeProcess(LoginServer.ProcessHandle, dwExitCode);
    if dwExitCode <> STILL_ACTIVE then
    begin
      nRetCode := RunProgram(LoginServer, IntToStr(Self.Handle), 0);
      if nRetCode = 0 then
      begin
        CloseHandle(LoginServer.ProcessHandle);
        LoginServer.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LoginServer.ProcessInfo.dwProcessId);
        MainOutMessage('登录服务器异常关闭，已被重新启动...');
      end;
    end;
  end;

  if LogServer.boGetStart then
  begin
    GetExitCodeProcess(LogServer.ProcessHandle, dwExitCode);
    if dwExitCode <> STILL_ACTIVE then
    begin
      nRetCode := RunProgram(LogServer, IntToStr(Self.Handle), 0);
      if nRetCode = 0 then
      begin
        CloseHandle(LogServer.ProcessHandle);
        LogServer.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LogServer.ProcessInfo.dwProcessId);
        MainOutMessage('日志服务器异常关闭，已被重新启动...');
      end;
    end;
  end;

  if M2Server.boGetStart then
  begin
    GetExitCodeProcess(M2Server.ProcessHandle, dwExitCode);
    if dwExitCode <> STILL_ACTIVE then
    begin
      nRetCode := RunProgram(M2Server, IntToStr(Self.Handle), 0);
      if nRetCode = 0 then
      begin
        CloseHandle(M2Server.ProcessHandle);
        M2Server.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, M2Server.ProcessInfo.dwProcessId);
        MainOutMessage('游戏引擎服务器异常关闭，已被重新启动...');
      end;
    end;
  end;

  if g_boRunGate_GetMultiThread then
  begin
    if RunGate[0].boGetStart then
    begin
      GetExitCodeProcess(RunGate[0].ProcessHandle, dwExitCode);
      if dwExitCode <> STILL_ACTIVE then
      begin
        nRetCode := RunProgram(RunGate[0], IntToStr(Self.Handle), 0);
        if nRetCode = 0 then
        begin
          CloseHandle(RunGate[0].ProcessHandle);
          RunGate[0].ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, RunGate[0].ProcessInfo.dwProcessId);
          MainOutMessage('游戏网关异常关闭，已被重新启动...');
        end;
      end;
    end;
  end else
  begin
    for I := Low(RunGate) to High(RunGate) do
    begin
      if RunGate[I].boGetStart then
      begin
        GetExitCodeProcess(RunGate[I].ProcessHandle, dwExitCode);
        if dwExitCode <> STILL_ACTIVE then
        begin
          RunGate[I].MainFormHandle := 0;
          GetMutRunGateConfing(I);
          nRetCode := RunProgram(RunGate[I], IntToStr(Self.Handle), 2000);
          if nRetCode = 0 then
          begin
            CloseHandle(RunGate[I].ProcessHandle);
            RunGate[I].ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, RunGate[I].ProcessInfo.dwProcessId);
          //RunGate[I].btStartStatus:=3;
            case I of
              0: MainOutMessage('游戏网关一异常关闭，已被重新启动...');
              1: MainOutMessage('游戏网关二异常关闭，已被重新启动...');
              2: MainOutMessage('游戏网关三异常关闭，已被重新启动...');
              3: MainOutMessage('游戏网关四异常关闭，已被重新启动...');
              4: MainOutMessage('游戏网关五异常关闭，已被重新启动...');
              5: MainOutMessage('游戏网关六异常关闭，已被重新启动...');
              6: MainOutMessage('游戏网关七异常关闭，已被重新启动...');
              7: MainOutMessage('游戏网关八异常关闭，已被重新启动...');
            end;
          end;
          Break;
        end;
      end;
    end;
  end;


  if g_boSelGate_GetMultiThread then                                           // 2019-09-28 15:54:17
  begin
    GenMutSelGateConfigEx;

    if SelGate.boGetStart then
    begin
      GetExitCodeProcess(SelGate.ProcessHandle, dwExitCode);
      if dwExitCode <> STILL_ACTIVE then
      begin
        nRetCode := RunProgram(SelGate, IntToStr(Self.Handle), 0);
        if nRetCode = 0 then
        begin
          CloseHandle(SelGate.ProcessHandle);
          SelGate.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate.ProcessInfo.dwProcessId);
          MainOutMessage('角色网关异常关闭，已被重新启动...');
        end;
      end;
    end
    else if SelGate1.boGetStart then
    begin
      GetExitCodeProcess(SelGate1.ProcessHandle, dwExitCode);
      if dwExitCode <> STILL_ACTIVE then
      begin
        nRetCode := RunProgram(SelGate1, IntToStr(Self.Handle), 0);
        if nRetCode = 0 then
        begin
          CloseHandle(SelGate1.ProcessHandle);
          SelGate1.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate1.ProcessInfo.dwProcessId);
          MainOutMessage('角色网关异常关闭，已被重新启动...');
        end;
      end;
    end;
  end
  else
  begin
    if SelGate.boGetStart then
    begin
      GetExitCodeProcess(SelGate.ProcessHandle, dwExitCode);
      if dwExitCode <> STILL_ACTIVE then
      begin
        nRetCode := RunProgram(SelGate, IntToStr(Self.Handle), 0);
        if nRetCode = 0 then
        begin
          CloseHandle(SelGate.ProcessHandle);
          SelGate.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate.ProcessInfo.dwProcessId);
          MainOutMessage('角色网关一异常关闭，已被重新启动...');
        end;
      end;
    end;

    if SelGate1.boGetStart then
    begin
      GetExitCodeProcess(SelGate1.ProcessHandle, dwExitCode);
      if dwExitCode <> STILL_ACTIVE then
      begin
        nRetCode := RunProgram(SelGate1, IntToStr(Self.Handle), 0, 1);
        if nRetCode = 0 then
        begin
          CloseHandle(SelGate1.ProcessHandle);
          SelGate1.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, SelGate1.ProcessInfo.dwProcessId);
          MainOutMessage('角色网关二异常关闭，已被重新启动...');
        end;
      end;
    end;
  end;

  if LoginGate.boGetStart then
  begin
    GetExitCodeProcess(LoginGate.ProcessHandle, dwExitCode);
    if dwExitCode <> STILL_ACTIVE then
    begin
      nRetCode := RunProgram(LoginGate, IntToStr(Self.Handle), 0);
      if nRetCode = 0 then
      begin
        CloseHandle(LoginGate.ProcessHandle);
        LoginGate.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LoginGate.ProcessInfo.dwProcessId);
        MainOutMessage('登录网关一异常关闭，已被重新启动...');
      end;
    end;
  end;

  if LoginGate1.boGetStart then
  begin
    GetExitCodeProcess(LoginGate1.ProcessHandle, dwExitCode);
    if dwExitCode <> STILL_ACTIVE then
    begin
      nRetCode := RunProgram(LoginGate1, IntToStr(Self.Handle), 0, 1);
      if nRetCode = 0 then
      begin
        CloseHandle(LoginGate1.ProcessHandle);
        LoginGate1.ProcessHandle := OpenProcess(PROCESS_ALL_ACCESS, False, LoginGate1.ProcessInfo.dwProcessId);
        MainOutMessage('登录网关二异常关闭，已被重新启动...');
      end;
    end;
  end;
end;

procedure TfrmMain.ProcessMessage(var Msg: TMsg; var Handled: Boolean);
begin
  if Msg.message = WM_SENDPROCMSG then
  begin
    //    ShowMessage('asfd');
    Handled := True;
  end;
end;

procedure TfrmMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: string;
  wIdent, wRecog: Word;
begin
  wIdent := HiWord(MsgData.From);
  wRecog := LoWord(MsgData.From);
  //ProgramType:=TProgamType(LoWord(MsgData.From));
  sData := StrPas(MsgData.CopyDataStruct^.lpData);
  case TProgamType(wRecog) of                                                                       //
    tDBServer: ProcessDBServerMsg(wIdent, sData);
    tLoginSrv: ProcessLoginSrvMsg(wIdent, sData);
    tLogServer: ProcessLogServerMsg(wIdent, sData);
    tM2Server: ProcessM2ServerMsg(wIdent, sData);
    tLoginGate: ProcessLoginGateMsg(wIdent, sData);
    tLoginGate1: ProcessLoginGate1Msg(wIdent, sData);
    tSelGate: ProcessSelGateMsg(wIdent, sData);
    tSelGate1: ProcessSelGate1Msg(wIdent, sData);
    tRunGate: ProcessRunGateMsg(wIdent, sData);
  end;
  if wIdent = SG_ACTIVE then g_dwStopTick := GetTickCount;
end;



procedure ShowAppEmbedded(WindowHandle: THandle; Container: TWinControl);
const
  WM_SET_PARENT_WINDOW = WM_USER + 123;
var
  FAppThreadID: Cardinal;
begin
  /// Set running app window styles.
  {
  WindowStyle := GetWindowLong(WindowHandle, GWL_STYLE);
  WindowStyle := WindowStyle
                 //- WS_CAPTION
                 //- WS_BORDER
                 - WS_OVERLAPPED
                 - WS_THICKFRAME;
  SetWindowLong(WindowHandle,GWL_STYLE,WindowStyle);
  }

  /// Attach container app input thread to the running app input thread, so that
  ///  the running app receives user input.
  FAppThreadID := GetWindowThreadProcessId(WindowHandle, nil);
  AttachThreadInput(GetCurrentThreadId, FAppThreadID, True);

  /// Changing parent of the running app to our provided container control
  //Windows.SetParent(WindowHandle,Container.Handle);
  SendMessage(WindowHandle, WM_SET_PARENT_WINDOW, 1, Container.Handle);       // 用上面的命令，在另一个程序内部，用GetParent找不到

  SendMessage(Container.Handle, WM_UPDATEUISTATE, UIS_INITIALIZE, 0);
  UpdateWindow(WindowHandle);

  /// This prevents the parent control to redraw on the area of its child windows (the running app)
  SetWindowLong(Container.Handle, GWL_STYLE, GetWindowLong(Container.Handle,GWL_STYLE) or WS_CLIPCHILDREN);
  /// Make the running app to fill all the client area of the container

  SetWindowPos(WindowHandle, 0, 0, 0, Container.ClientWidth, Container.ClientHeight, SWP_NOZORDER or SWP_NOSIZE);
  SetForegroundWindow(WindowHandle);
end;

procedure TfrmMain.ProcessDBServerMsg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle :=  StrToInt64Def(sData, 0); //Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          DBServer.MainFormHandle := nHandle;

          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);
      end;
    SG_STARTOK:
      begin
        DBServer.btStartStatus := 2;
        MainOutMessage(sData);
        if DBServer.boMinimize then
          SendMessage(DBServer.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
      end;
    SG_CHECKCODEADDR:
      begin

      end;
    3: ;
  end;
end;

procedure TfrmMain.ProcessLoginGateMsg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := StrToInt64Def(sData, 0); //Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          LoginGate.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);

      end;
    SG_STARTOK:
      begin
        LoginGate.btStartStatus := 2;
        MainOutMessage(sData);
        if LoginGate.boMinimize then
          SendMessage(LoginGate.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
      end;
    2: ;
    3: ;
  end;
end;

procedure TfrmMain.ProcessLoginGate1Msg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := StrToInt64Def(sData, 0);  //Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          LoginGate1.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);

      end;
    SG_STARTOK:
      begin
        LoginGate1.btStartStatus := 2;
        MainOutMessage(sData);
        if LoginGate1.boMinimize then
          SendMessage(LoginGate1.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
      end;
    2: ;
    3: ;
  end;
end;

procedure TfrmMain.ProcessSelGateMsg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := StrToInt64Def(sData, 0); //Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          SelGate.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);
      end;
    SG_STARTOK:
      begin
        if SelGate.btStartStatus <> 2 then
        begin
          SelGate.btStartStatus := 2;
          if SelGate.boMinimize then
            SendMessage(SelGate.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
        end else
        begin
          SelGate.btStartStatus := 2;
        end;
        MainOutMessage(sData);
      end;
  end;
end;

procedure TfrmMain.ProcessSelGate1Msg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := StrToInt64Def(sData, 0); //Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          SelGate1.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);
      end;
    SG_STARTOK:
      begin
        if SelGate1.btStartStatus <> 2 then
        begin
          SelGate1.btStartStatus := 2;
          if SelGate1.boMinimize then
            SendMessage(SelGate1.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
        end else
        begin
          SelGate1.btStartStatus := 2;
        end;
        MainOutMessage(sData);
      end;
  end;
end;

procedure TfrmMain.ProcessM2ServerMsg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := TWindowHandle(StrToInt64Def(sData, 0)); //Str_ToInt(sData, 0);
        if nHandle <> 0 then begin
          M2Server.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);
      end;
    SG_STARTOK:
      begin
        M2Server.btStartStatus := 2;
        MainOutMessage(sData);
        if M2Server.boMinimize then
          SendMessage(M2Server.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
      end;

    SG_CHECKCODEADDR:
      begin

      end;

  end;
end;

procedure TfrmMain.ProcessLoginSrvMsg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := StrToInt64Def(sData, 0);//Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          LoginServer.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);
      end;
    SG_STARTOK:
      begin
        LoginServer.btStartStatus := 2;
        MainOutMessage(sData);
        if LoginServer.boMinimize then
          SendMessage(LoginServer.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
      end;
    SG_USERACCOUNT:
      begin
        ProcessLoginSrvGetUserAccount(sData);
      end;
    SG_USERACCOUNTCHANGESTATUS:
      begin
        ProcessLoginSrvChangeUserAccountStatus(sData);
      end;
  end;
end;

procedure TfrmMain.ProcessLogServerMsg(wIdent: Word; sData: string);
var
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle := StrToInt64Def(sData, 0);  //Str_ToInt(sData, 0);
        if nHandle <> 0 then
        begin
          LogServer.MainFormHandle := nHandle;
          if chkEmbeddedWindow.Checked then
          begin
            ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        MainOutMessage(sData);
      end;
    SG_STARTOK:
      begin
        LogServer.btStartStatus := 2;
        MainOutMessage(sData);
        if LogServer.boMinimize then
          SendMessage(LogServer.MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
      end;
  end;
end;

procedure TfrmMain.ProcessRunGateMsg(wIdent: Word; sData: string);
var
  I: Integer;
  nHandle: TWindowHandle;
begin
  case wIdent of
    SG_FORMHANDLE:
      begin
        nHandle :=  StrToInt64Def(sData, 0); //Str_ToInt(sData, 0);
        if g_boRunGate_GetMultiThread then
        begin
          if nHandle <> 0 then begin
            RunGate[0].MainFormHandle := nHandle;
            if chkEmbeddedWindow.Checked then
            begin
              ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
            end;
          end;
        end else
        begin
          //MainOutMessage('SG_FORMHANDLE:'+sData);
          for I := Low(RunGate) to High(RunGate) do
          begin
            if RunGate[I].boGetStart and (RunGate[I].MainFormHandle = 0) then
            begin
              RunGate[I].MainFormHandle := nHandle;
              if chkEmbeddedWindow.Checked then
              begin
                ShowAppEmbedded(nHandle, pnlProgramWindow);   // 2017-04-06
              end;
              Break;
            end;
          end;
        end;
      end;
    SG_STARTNOW:
      begin
        if g_boRunGate_GetMultiThread then
        begin
          //MainOutMessage(sData);  // 去数字提示 chongchong 2015-09-22
        end else
        begin
          //MainOutMessage('SG_STARTNOW:'+sData);
          nHandle := Str_ToInt(sData, 0);
          for I := Low(RunGate) to High(RunGate) do
          begin
            if RunGate[I].MainFormHandle = nHandle then
            begin
              case I of
                0: MainOutMessage('正在启动游戏网关一...');
                1: MainOutMessage('正在启动游戏网关二...');
                2: MainOutMessage('正在启动游戏网关三...');
                3: MainOutMessage('正在启动游戏网关四...');
                4: MainOutMessage('正在启动游戏网关五...');
                5: MainOutMessage('正在启动游戏网关六...');
                6: MainOutMessage('正在启动游戏网关七...');
                7: MainOutMessage('正在启动游戏网关八...');
              end;
              Break;
            end;
          end;
        end;
      end;
    SG_STARTOK:
      begin
        if g_boRunGate_GetMultiThread then
        begin
          RunGate[0].btStartStatus := 2;
          //MainOutMessage(sData);      // 去数字提示 chongchong 2015-09-22
          if g_boRunGate_GetMinimize then
            SendMessage(RunGate[0].MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
        end else
        begin
          //MainOutMessage('SG_STARTOK:'+sData);
          nHandle := Str_ToInt(sData, 0);
          for I := Low(RunGate) to High(RunGate) do
          begin
            if RunGate[I].MainFormHandle = nHandle then
            begin
              RunGate[I].btStartStatus := 2;
              case I of
                0: MainOutMessage('游戏网关一启动完成...');
                1: MainOutMessage('游戏网关二启动完成...');
                2: MainOutMessage('游戏网关三启动完成...');
                3: MainOutMessage('游戏网关四启动完成...');
                4: MainOutMessage('游戏网关五启动完成...');
                5: MainOutMessage('游戏网关六启动完成...');
                6: MainOutMessage('游戏网关七启动完成...');
                7: MainOutMessage('游戏网关八启动完成...');
              end;
              if g_boRunGate_GetMinimize then
                SendMessage(RunGate[I].MainFormHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
              Break;
            end;
          end;
        end;

      end;
  end;
end;

procedure TfrmMain.ButtonReLoadConfigClick(Sender: TObject);
begin
  LoadConfig();
  RefGameConsole();

  //Caption := m_sCapTion + '-' + g_sGameName;
  //Application.Title := Caption;
  //FOldCaption := Caption;
  //Caption := FOldCaption + ' [' + g_sGameDirectory + ']';
  Caption := Format('%s - [%s]', [sProgramName,  g_sGameDirectory]);

  Application.MessageBox('配置重加载完成...', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TfrmMain.EditLoginGate_MainFormXChange(Sender: TObject);
begin
  if EditLoginGate_MainFormX.Text = '' then
  begin
    EditLoginGate_MainFormX.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nLoginGate_MainFormX := EditLoginGate_MainFormX.Value;
end;

procedure TfrmMain.EditLoginGate_MainFormYChange(Sender: TObject);
begin
  if EditLoginGate_MainFormY.Text = '' then
  begin
    EditLoginGate_MainFormY.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nLoginGate_MainFormY := EditLoginGate_MainFormY.Value;
end;

procedure TfrmMain.CheckBoxboLoginGate_GetStartClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boLoginGate_GetStart := CheckBoxboLoginGate_GetStart.Checked;
end;

procedure TfrmMain.EditSelGate_MainFormXChange(Sender: TObject);
begin
  if EditSelGate_MainFormX.Text = '' then
  begin
    EditSelGate_MainFormX.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nSelGate_MainFormX := EditSelGate_MainFormX.Value;
end;

procedure TfrmMain.EditSelGate_MainFormYChange(Sender: TObject);
begin
  if EditSelGate_MainFormY.Text = '' then
  begin
    EditSelGate_MainFormY.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nSelGate_MainFormY := EditSelGate_MainFormY.Value;
end;

procedure TfrmMain.CheckBoxboSelGate_GetStartClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boSelGate_GetStart := CheckBoxboSelGate_GetStart.Checked;
end;

procedure TfrmMain.EditLoginServer_MainFormXChange(Sender: TObject);
begin
  if EditLoginServer_MainFormX.Text = '' then
  begin
    EditLoginServer_MainFormX.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nLoginServer_MainFormX := EditLoginServer_MainFormX.Value;
end;

procedure TfrmMain.EditLoginServer_MainFormYChange(Sender: TObject);
begin
  if EditLoginServer_MainFormY.Text = '' then
  begin
    EditLoginServer_MainFormY.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nLoginServer_MainFormY := EditLoginServer_MainFormY.Value;
end;

procedure TfrmMain.CheckBoxboLoginServer_GetStartClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boLoginServer_GetStart := CheckBoxboLoginServer_GetStart.Checked;
end;

procedure TfrmMain.EditDBServer_MainFormXChange(Sender: TObject);
begin
  if EditDBServer_MainFormX.Text = '' then
  begin
    EditDBServer_MainFormX.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nDBServer_MainFormX := EditDBServer_MainFormX.Value;
end;

procedure TfrmMain.EditDBServer_MainFormYChange(Sender: TObject);
begin
  if EditDBServer_MainFormY.Text = '' then
  begin
    EditDBServer_MainFormY.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nDBServer_MainFormY := EditDBServer_MainFormY.Value;
end;

procedure TfrmMain.CheckBoxDBServerGetStartClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boDBServer_GetStart := CheckBoxDBServerGetStart.Checked;
end;

procedure TfrmMain.EditLogServer_MainFormXChange(Sender: TObject);
begin
  if EditLogServer_MainFormX.Text = '' then
  begin
    EditLogServer_MainFormX.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nLogServer_MainFormX := EditLogServer_MainFormX.Value;
end;

procedure TfrmMain.EditLogServer_MainFormYChange(Sender: TObject);
begin
  if EditLogServer_MainFormY.Text = '' then
  begin
    EditLogServer_MainFormY.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nLogServer_MainFormY := EditLogServer_MainFormY.Value;
end;

procedure TfrmMain.CheckBoxLogServerGetStartClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boLogServer_GetStart := CheckBoxLogServerGetStart.Checked;
end;

procedure TfrmMain.EditM2Server_MainFormXChange(Sender: TObject);
begin
  if EditM2Server_MainFormX.Text = '' then
  begin
    EditM2Server_MainFormX.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nM2Server_MainFormX := EditM2Server_MainFormX.Value;
end;

procedure TfrmMain.EditM2Server_MainFormYChange(Sender: TObject);
begin
  if EditM2Server_TestLevel.Text = '' then
  begin
    EditM2Server_TestLevel.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nM2Server_TestLevel := EditM2Server_TestLevel.Value;
end;

procedure TfrmMain.EditM2Server_TestLevelChange(Sender: TObject);
begin
  if EditM2Server_TestLevel.Text = '' then
  begin
    EditM2Server_TestLevel.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nM2Server_TestLevel := EditM2Server_TestLevel.Value;
end;

procedure TfrmMain.EditM2Server_TestGoldChange(Sender: TObject);
begin
  if EditM2Server_TestGold.Text = '' then
  begin
    EditM2Server_TestGold.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nM2Server_TestGold := EditM2Server_TestGold.Value;
end;

procedure TfrmMain.CheckBoxM2ServerGetStartClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boM2Server_GetStart := CheckBoxM2ServerGetStart.Checked;
end;

procedure TfrmMain.MemoLogChange(Sender: TObject);
begin
  if MemoLog.Lines.Count > 100 then
    MemoLog.Clear;
end;

procedure TfrmMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  if not TimerClose.Enabled then
  begin
    if m_nStartStatus = 2 then
    begin
      if Application.MessageBox('游戏服务器正在运行，是否停止游戏服务器 ?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
      begin
        ButtonStartGameClick(ButtonStartGame);
      end;
      CanClose := False;
      Exit;
    end;
    if g_boHeroDBOK then
      if Application.MessageBox('是否确认关闭控制台 ?', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
      begin
        CanClose := True;
      end else
      begin
        CanClose := False;
      end;
    if CanClose and (g_BackUpManager.m_BackUpList.Count > 0) then
    begin
      CanClose := False;
      PageControl1.Enabled := False;
      TimerClose.Enabled := True;
    end;
  end;
end;

procedure TfrmMain.EditRunGate_ConntChange(Sender: TObject);
begin
  if EditRunGate_Connt.Text = '' then
  begin
    EditRunGate_Connt.Text := '0';
  end;
  if not m_boOpen then Exit;
  g_nRunGate_Count := EditRunGate_Connt.Value;
  g_RunGateInfo[0].boGetStart := g_nRunGate_Count >= 1;
  g_RunGateInfo[1].boGetStart := g_nRunGate_Count >= 2;
  g_RunGateInfo[2].boGetStart := g_nRunGate_Count >= 3;
  g_RunGateInfo[3].boGetStart := g_nRunGate_Count >= 4;
  g_RunGateInfo[4].boGetStart := g_nRunGate_Count >= 5;
  g_RunGateInfo[5].boGetStart := g_nRunGate_Count >= 6;
  g_RunGateInfo[6].boGetStart := g_nRunGate_Count >= 7;
  g_RunGateInfo[7].boGetStart := g_nRunGate_Count >= 8;
  g_sDBServer_Config_GateAddr := g_sAllIPaddr;
  //g_sDBServer_Config_GateAddr2 := g_sAllIPaddr;
  RefGameConsole();
end;

procedure TfrmMain.ButtonLoginServerConfigClick(Sender: TObject);
begin
  frmLoginServerConfig.Open;
end;

procedure TfrmMain.chkDoubleLineModeClick(Sender: TObject);
begin
  //EditGameExtIPaddr.Enabled := not CheckBoxDoubleLineMode.Checked;
  g_boDoubleLineMode := chkDoubleLineMode.Checked;
  LabelNetComIPaddr.Visible := chkDoubleLineMode.Checked;
  EditGameExtNetComIPaddr.Visible := chkDoubleLineMode.Checked;
  GroupBoxSelGate_GetStart.Enabled := g_boDoubleLineMode;
  if g_boDoubleLineMode then
    CheckBoxboSelGate_GetStart1.Checked := True
  else
    CheckBoxboSelGate_GetStart1.Checked := False;
end;

function TfrmMain.StartService: Boolean;
begin
  Result := False;
  MainOutMessage('正在启动游戏客户端控制器...');
  //g_SessionList := TStringList.Create;
  //if FileExists(g_sGameFile) then begin
  //  MemoGameList.Lines.LoadFromFile(g_sGameFile);
  //end;
  //g_sNoticeUrl := g_IniConf.ReadString('Client', 'NoticeUrl', m_sNoticeUrl);
  //g_nClientForm := g_IniConf.ReadInteger('Client', 'ClientForm', g_nClientForm);
  //g_nServerPort := g_IniConf.ReadInteger('Client', 'ServerPort', g_nServerPort);
  //g_sServerAddr := g_IniConf.ReadString('Client', 'ServerAddr', g_sServerAddr);

 // g_sServerAddr := g_IniConf.ReadString('Client', 'ServerAddr', g_sServerAddr);
  //g_nServerPort := g_IniConf.ReadInteger('Client', 'ServerPort', g_nServerPort);
  //EditNoticeUrl.Text := g_sNoticeUrl;
  //EditClientForm.Value := g_nClientForm;
  try
    m_dwShowTick := GetTickCount();
  except
    on e: ESocketError do
    begin
      //MainOutMessage(Format('端口%d打开异常，检查端口是否被其它程序占用！！！', [g_nServerPort]));
      MainOutMessage(e.message);
      Exit;
    end;
  end;
  MainOutMessage('游戏控制台启动完成...');
  Result := True;
end;

procedure TfrmMain.StopService;
begin

  g_IniConf.Free;
end;

procedure TfrmMain.ButtonGeneralDefalultClick(Sender: TObject);
begin
  EditGameDir.Text := 'D:\MirServer\';
  EditHeroDB.Text := 'HeroDB';
  edtSqliteDB.Text := 'D:\MirServer\Mud2\DB\BmM2.db';
  EditGameName.Text := 'Bmm2';//'GxxM2'; //'GeeM2'; //HZQ
  EditGameExtIPaddr.Text := '127.0.0.1';
  EditGameExtNetComIPaddr.Text := '127.0.0.2';
  chkDoubleLineMode.Checked := True;
end;

procedure TfrmMain.ButtonRunGateDefaultClick(Sender: TObject);
begin
  EditRunGate_Connt.Value := 3;
  EditRunGate_GatePort1.Text := '7200';
  EditRunGate_GatePort2.Text := '7300';
  EditRunGate_GatePort3.Text := '7400';
  EditRunGate_GatePort4.Text := '7500';
  EditRunGate_GatePort5.Text := '7600';
  EditRunGate_GatePort6.Text := '7700';
  EditRunGate_GatePort7.Text := '7800';
  EditRunGate_GatePort8.Text := '7900';

  edtRunGate_DBPort1.Text := '27201';
  edtRunGate_DBPort2.Text := '27301';
  edtRunGate_DBPort3.Text := '27401';
  edtRunGate_DBPort4.Text := '27501';
  edtRunGate_DBPort5.Text := '27601';
  edtRunGate_DBPort6.Text := '27701';
  edtRunGate_DBPort7.Text := '27801';
  edtRunGate_DBPort8.Text := '27901';

  edtRunGate_DBPortMulThread.Text := '27201';
end;

procedure TfrmMain.ButtonLoginGateDefaultClick(Sender: TObject);
begin
  EditLoginGate_MainFormX.Text := '0';
  EditLoginGate_MainFormY.Text := '0';
  EditLoginGate_GatePort.Text := '7000';
end;

procedure TfrmMain.ButtonSelGateDefaultClick(Sender: TObject);
begin
  EditSelGate_MainFormX.Text := '0';
  EditSelGate_MainFormY.Text := '163';
  EditSelGate_GatePort.Text := '7100';
end;

procedure TfrmMain.ButtonLoginSrvDefaultClick(Sender: TObject);
begin
  EditLoginServer_MainFormX.Text := '251';
  EditLoginServer_MainFormY.Text := '0';
  EditLoginServerGatePort.Text := '5500';
  EditLoginServerServerPort.Text := '5600';
  EditLoginServerControlPort.Text := '0';
  CheckBoxboLoginServer_GetStart.Checked := True;
end;

procedure TfrmMain.ButtonDBServerDefaultClick(Sender: TObject);
begin
  EditDBServer_MainFormX.Text := '0';
  EditDBServer_MainFormY.Text := '326';
  //CheckBoxDisableAutoGame.Checked := False;
  EditDBServerGatePort.Text := '5100';
  EditDBServerServerPort.Text := '6000';
  CheckBoxDBServerGetStart.Checked := True;
end;

procedure TfrmMain.ButtonLogServerDefaultClick(Sender: TObject);
begin
  EditLogServer_MainFormX.Text := '251';
  EditLogServer_MainFormY.Text := '239';
  EditLogServerPort.Text := '10000';
  CheckBoxLogServerGetStart.Checked := True;
end;

procedure TfrmMain.ButtonM2ServerDefaultClick(Sender: TObject);
begin
  EditM2Server_MainFormX.Text := '560';
  EditM2Server_MainFormY.Text := '0';
  EditM2Server_TestLevel.Value := 1;
  EditM2Server_TestGold.Value := 0;
  EditM2ServerGatePort.Text := '5000';
  EditM2ServerMsgSrvPort.Text := '4900';
  CheckBoxM2ServerGetStart.Checked := True;
end;

procedure TfrmMain.ButtonSearchLoginAccountClick(Sender: TObject);
var
  sAccount: string;
begin
  if LoginServer.btStartStatus <> 2 then
  begin
    Application.MessageBox('游戏登录服务器未启动！！！' + #13#13 + '启动游戏登录服务器后才能使用此功能。', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  sAccount := Trim(edtSearchLoginAccount.Text);
  if sAccount = '' then
  begin
    Application.MessageBox('帐号不能为空！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtSearchLoginAccount.SetFocus;
    Exit;
  end;
  edtLoginAccount.Text := '';
  edtLoginAccountPasswd.Text := '';
  edtLoginAccountUserName.Text := '';
  edtLoginAccountSSNo.Text := '';
  edtLoginAccountBirthDay.Text := '';
  edtLoginAccountPhone.Text := '';
  edtLoginAccountMobilePhone.Text := '';
  edtLoginAccountQuiz.Text := '';
  edtLoginAccountAnswer.Text := '';
  edtLoginAccountQuiz2.Text := '';
  edtLoginAccountAnswer2.Text := '';
  edtLoginAccountEMail.Text := '';
  edtLoginAccountMemo.Text := '';
  edtLoginAccountMemo2.Text := '';
  chkFullEditMode.Checked := False;
  UserAccountEditMode(False);
  edtLoginAccount.Enabled := False;
  SendProgramMsg(LoginServer.MainFormHandle, GS_USERACCOUNT, sAccount);
end;

procedure TfrmMain.ProcessLoginSrvGetUserAccount(sData: string);
var
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
  sDefMsg: string;
begin
  if Length(sData) < DEF_BLOCK_SIZE then Exit;
  sDefMsg := Copy(sData, 1, DEF_BLOCK_SIZE);
  sData := Copy(sData, DEF_BLOCK_SIZE + 1, Length(sData) - DEF_BLOCK_SIZE);
  DefMsg := DecodeMessage(sDefMsg);

  case DefMsg.Ident of                                                                              //
    SG_USERACCOUNTNOTFOUND:
      begin
        Application.MessageBox('帐号未找到！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
        Exit;
      end;
  else
    begin
      DecodeString(sData, @AccountInfo, SizeOf(AccountInfo));
    end;
  end;
  edtLoginAccount.Text := AccountInfo.AccountName;
  edtLoginAccountPasswd.Text := AccountInfo.Password;
  edtLoginAccountUserName.Text := AccountInfo.UserName;
  edtLoginAccountSSNo.Text := AccountInfo.IDCard;
  edtLoginAccountBirthDay.Text := AccountInfo.BirthDay;
  edtLoginAccountPhone.Text := AccountInfo.Phone;
  edtLoginAccountMobilePhone.Text := AccountInfo.MobilePhone;
  edtLoginAccountQuiz.Text := AccountInfo.Questions1;
  edtLoginAccountAnswer.Text := AccountInfo.Answers1;
  edtLoginAccountQuiz2.Text := AccountInfo.Questions2;
  edtLoginAccountAnswer2.Text := AccountInfo.Answers2;
  edtLoginAccountEMail.Text := AccountInfo.Mail;
  edtLoginAccountMemo.Text := AccountInfo.Memo;
  edtLoginAccountMemo2.Text := AccountInfo.L2Password;
  ButtonLoginAccountOK.Enabled := False;
end;

procedure TfrmMain.edtLoginAccountChange(Sender: TObject);
begin
  ButtonLoginAccountOK.Enabled := True;
end;

procedure TfrmMain.chkFullEditModeClick(Sender: TObject);
begin
  UserAccountEditMode(chkFullEditMode.Checked);
end;

procedure TfrmMain.UserAccountEditMode(boChecked: Boolean);
begin
  boChecked := chkFullEditMode.Checked;
  edtLoginAccountUserName.Enabled := boChecked;
  edtLoginAccountSSNo.Enabled := boChecked;
  edtLoginAccountBirthDay.Enabled := boChecked;
  edtLoginAccountQuiz.Enabled := boChecked;
  edtLoginAccountAnswer.Enabled := boChecked;
  edtLoginAccountQuiz2.Enabled := boChecked;
  edtLoginAccountAnswer2.Enabled := boChecked;
  edtLoginAccountMobilePhone.Enabled := boChecked;
  edtLoginAccountPhone.Enabled := boChecked;
  edtLoginAccountMemo.Enabled := boChecked;
  edtLoginAccountMemo2.Enabled := boChecked;
  edtLoginAccountEMail.Enabled := boChecked;
end;

procedure TfrmMain.ButtonLoginAccountOKClick(Sender: TObject);
var
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
  sAccount, sPassword, sUserName, sSSNo, sPhone, sQuiz, sAnswer, sEMail, sQuiz2, sAnswer2, sBirthDay, sMobilePhone, sMemo, sMemo2: string;
begin
  sAccount := Trim(edtLoginAccount.Text);
  sPassword := Trim(edtLoginAccountPasswd.Text);
  sUserName := Trim(edtLoginAccountUserName.Text);
  sSSNo := Trim(edtLoginAccountSSNo.Text);
  sPhone := Trim(edtLoginAccountPhone.Text);
  sQuiz := Trim(edtLoginAccountQuiz.Text);
  sAnswer := Trim(edtLoginAccountAnswer.Text);
  sEMail := Trim(edtLoginAccountEMail.Text);
  sQuiz2 := Trim(edtLoginAccountQuiz2.Text);
  sAnswer2 := Trim(edtLoginAccountAnswer2.Text);
  sBirthDay := Trim(edtLoginAccountBirthDay.Text);
  sMobilePhone := Trim(edtLoginAccountMobilePhone.Text);
  sMemo := Trim(edtLoginAccountMemo.Text);
  sMemo2 := Trim(edtLoginAccountMemo2.Text);
  if sAccount = '' then
  begin
    Application.MessageBox('帐号不能不空！！！', '提示信息', MB_OK + MB_ICONERROR);
    edtLoginAccount.SetFocus;
    Exit;
  end;
  if sPassword = '' then
  begin
    Application.MessageBox('密码不能不空！！！', '提示信息', MB_OK + MB_ICONERROR);
    edtLoginAccountPasswd.SetFocus;
    Exit;
  end;
  FillChar(AccountInfo, SizeOf(AccountInfo), 0);

  AccountInfo.AccountName := sAccount;
  AccountInfo.Password := sPassword;
  AccountInfo.UserName := sUserName;
  AccountInfo.IDCard := sSSNo;
  AccountInfo.Phone := sPhone;
  AccountInfo.Questions1 := sQuiz;
  AccountInfo.Answers1 := sAnswer;
  AccountInfo.Mail := sEMail;
  AccountInfo.Questions2 := sQuiz2;
  AccountInfo.Answers2 := sAnswer2;
  AccountInfo.BirthDay := sBirthDay;
  AccountInfo.MobilePhone := sMobilePhone;
  AccountInfo.Memo := sMemo;
  AccountInfo.L2Password:= sMemo2;
  DefMsg := MakeDefaultMsg(0, 0, 0, 0, 0);
  
  SendProgramMsg(LoginServer.MainFormHandle, GS_CHANGEACCOUNTINFO, EncodeMessage(DefMsg) + EncodeBuffer(@AccountInfo, SizeOf(AccountInfo)));
  ButtonLoginAccountOK.Enabled := False;
end;

procedure TfrmMain.ProcessLoginSrvChangeUserAccountStatus(sData: string);
var
  DefMsg: TDefaultMessage;
  sDefMsg: string;
begin
  if Length(sData) < DEF_BLOCK_SIZE then Exit;
  sDefMsg := Copy(sData, 1, DEF_BLOCK_SIZE);
  sData := Copy(sData, DEF_BLOCK_SIZE + 1, Length(sData) - DEF_BLOCK_SIZE);
  DefMsg := DecodeMessage(sDefMsg);
  case DefMsg.Recog of                                                                              //
    -1: Application.MessageBox('指定的帐号不存在！！！', '提示信息', MB_OK + MB_ICONERROR);
    1: Application.MessageBox('帐号更新成功...', '提示信息', MB_OK + MB_ICONINFORMATION);
    2: Application.MessageBox('帐号更新失败！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
  end;                                                                                              // case
end;

procedure TfrmMain.RefGameDebug;
{
var
  CheckCode: TCheckCode;
  dwReturn: LongWord;
}
begin
  {EditM2CheckCodeAddr.Text := IntToHex(g_dwM2CheckCodeAddr, 2);
  FillChar(CheckCode, SizeOf(CheckCode), 0);
  ReadProcessMemory(M2Server.ProcessHandle, Pointer(g_dwM2CheckCodeAddr), @CheckCode, SizeOf(CheckCode), dwReturn);
  if dwReturn = SizeOf(CheckCode) then begin
    EditM2CheckCode.Text := IntToStr(CheckCode.dwThread0);
    EditM2CheckStr.Text := string(CheckCode.sThread0);
  end;

  EditDBCheckCodeAddr.Text := IntToHex(g_dwDBCheckCodeAddr, 2);
  FillChar(CheckCode, SizeOf(CheckCode), 0);
  ReadProcessMemory(DBServer.ProcessHandle, Pointer(g_dwDBCheckCodeAddr), @CheckCode, SizeOf(CheckCode), dwReturn);
  if dwReturn = SizeOf(CheckCode) then begin
    EditDBCheckCode.Text := IntToStr(CheckCode.dwThread0);
    EditDBCheckStr.Text := string(CheckCode.sThread0);
  end;}
end;

procedure TfrmMain.ButtonM2SuspendClick(Sender: TObject);
begin
  SuspendThread(M2Server.ProcessInfo.hThread);
end;

procedure TfrmMain.CheckBoxM2ServerGetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boM2Server_GetMinimize := CheckBoxM2ServerGetMinimize.Checked;
end;

procedure TfrmMain.CheckBoxLogServerGetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boLogServer_GetMinimize := CheckBoxLogServerGetMinimize.Checked;
end;

procedure TfrmMain.CheckBoxDBServerGetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boDBServer_GetMinimize := CheckBoxDBServerGetMinimize.Checked;
end;

procedure TfrmMain.CheckBoxboLoginServer_GetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boLoginServer_GetMinimize := CheckBoxboLoginServer_GetMinimize.Checked;
end;

procedure TfrmMain.CheckBoxboRunGate_GetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boRunGate_GetMinimize := CheckBoxboRunGate_GetMinimize.Checked;
end;

procedure TfrmMain.CheckBoxboSelGate_GetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boSelGate_GetMinimize := CheckBoxboSelGate_GetMinimize.Checked;
end;

procedure TfrmMain.CheckBoxboLoginGate_GetMinimizeClick(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boLoginGate_GetMinimize := CheckBoxboLoginGate_GetMinimize.Checked;
end;

procedure TfrmMain.TimerStartTimer(Sender: TObject);
var
  sSourceDirectory: string;
  sDestDirectory: string;
  BackUpTask: TBackUpTask;
  FrmHeroDB: TFrmHeroDB;
begin
  TimerStart.Enabled := False;

  if g_boUseSqliteDB then begin
    if not ProcessSqliteDB(g_sSqliteDBName) then Exit;

    g_boHeroDBOK := True;
  end else begin
    FrmHeroDB := TFrmHeroDB.Create(nil);
    try
      g_boHeroDBOK := not FrmHeroDB.CheckHeroDB;
      if not g_boHeroDBOK then
        FrmHeroDB.Open;
    finally
      FrmHeroDB.Free;
    end;

    if not g_boHeroDBOK then begin
      Application.MessageBox('数据库异常无法启动！！！', '提示信息', MB_OK + MB_ICONERROR);
      Close;
      Exit;
    end;
  end;


  if g_BackUpManager.m_BackUpList.Count <= 0 then begin
    sDestDirectory := ExtractFilePath(ParamStr(0)) + '数据备份\';
    sSourceDirectory := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_Dir;
    BackUpTask := TBackUpTask.Create;
    BackUpTask.SourceDirectory := sSourceDirectory;
    BackUpTask.DestDirectory := sDestDirectory;
    BackUpTask.Hour := 6;
    BackUpTask.Min := 0;
    BackUpTask.Mode := 1;
    BackUpTask.Start := True;
    BackUpTask.IsCompress := True;
    g_BackUpManager.Add(BackUpTask);

    sSourceDirectory := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_IdDir;
    BackUpTask := TBackUpTask.Create;
    BackUpTask.SourceDirectory := sSourceDirectory;
    BackUpTask.DestDirectory := sDestDirectory;
    BackUpTask.Hour := 6;
    BackUpTask.Min := 0;
    BackUpTask.Mode := 1;
    BackUpTask.Start := True;
    BackUpTask.IsCompress := True;
    g_BackUpManager.Add(BackUpTask);
    RefBackListToView();
    SaveBackList;
  end;

  if chkAutoStart.Checked then
    ButtonBackStart.Click;
end;

procedure TfrmMain.EditSourceButtonClick(Sender: TObject);
var
  NewDir: string;
begin
  NewDir := EditSource.Text;
  if SelectDirectory('请选择你要备份的文件夹', '', NewDir, Handle) then
  begin
    EditSource.Text := NewDir;
  end;
end;

procedure TfrmMain.EditDestButtonClick(Sender: TObject);
var
  NewDir: string;
begin
  NewDir := EditDest.Text;
  if SelectDirectory('请选择备份文件夹', '', NewDir, Handle) then
  begin
    EditDest.Text := NewDir;
  end;
end;

procedure TfrmMain.RadioButtonBackMode1Click(Sender: TObject);
begin
  EditHour2.Enabled := not RadioButtonBackMode1.Checked;
  EditMin2.Enabled := not RadioButtonBackMode1.Checked;
  EditHour1.Enabled := RadioButtonBackMode1.Checked;
  EditMin1.Enabled := RadioButtonBackMode1.Checked;
end;

procedure TfrmMain.RadioButtonBackMode2Click(Sender: TObject);
begin
  EditHour1.Enabled := not RadioButtonBackMode2.Checked;
  EditMin1.Enabled := not RadioButtonBackMode2.Checked;
  EditHour2.Enabled := RadioButtonBackMode2.Checked;
  EditMin2.Enabled := RadioButtonBackMode2.Checked;
end;

procedure TfrmMain.FormDestroy(Sender: TObject);
begin
  g_BackUpManager.Free;
end;

procedure TfrmMain.TimerCloseTimer(Sender: TObject);
begin
  if g_BackUpManager.m_BackUpList.Count > 0 then
  begin
    g_BackUpManager.Clear;
  end else Close;
end;

procedure TfrmMain.ListViewDataBackupClick(Sender: TObject);
var
  ListItem: TListItem;
  BackUpTask: TBackUpTask;
begin
  try
    ListItem := ListViewDataBackup.Selected;
    if ListItem = nil then Exit;

    BackUpTask := TBackUpTask(ListItem.SubItems.Objects[0]);
    EditSource.Text := BackUpTask.SourceDirectory;
    EditDest.Text := BackUpTask.DestDirectory;
    //CheckBoxBackUp.Checked := BackUpTask.Start;
    if BackUpTask.Mode = 0 then
    begin
      RadioButtonBackMode1.Checked := True;
      RadioButtonBackMode2.Checked := False;
      EditHour1.IntValue := BackUpTask.Hour;
      EditMin1.IntValue := BackUpTask.Min;
    end else
    begin
      RadioButtonBackMode1.Checked := False;
      RadioButtonBackMode2.Checked := True;
      EditHour2.IntValue := BackUpTask.Hour;
      EditMin2.IntValue := BackUpTask.Min;
    end;
    EditHour1.Enabled := RadioButtonBackMode1.Checked;
    EditMin1.Enabled := RadioButtonBackMode1.Checked;
    EditHour2.Enabled := RadioButtonBackMode2.Checked;
    EditMin2.Enabled := RadioButtonBackMode2.Checked;
    ButtonBackDel.Enabled := True;
    ButtonBackChg.Enabled := True;
    chkIsCompress.Checked := BackUpTask.IsCompress;
  except
    ButtonBackDel.Enabled := False;
    ButtonBackChg.Enabled := False;
  end;
end;

procedure TfrmMain.ButtonBackChgClick(Sender: TObject);
var
  ListItem: TListItem;
  BackUpTask: TBackUpTask;
  sSource, sDest: string;
  wHour, wMin: Word;
begin
  ListItem := ListViewDataBackup.Selected;
  if ListItem <> nil then
  begin
    sSource := Trim(EditSource.Text);
    sDest := Trim(EditDest.Text);
    if sSource = '' then
    begin
      Application.MessageBox('请选择数据目录！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
      Exit;
    end;
    if sDest = '' then
    begin
      Application.MessageBox('请选择备份目录！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
      Exit;
    end;
    {if g_BackUpManager.FindObject(sSource) <> nil then begin
      Application.MessageBox('此备份目录已经存在！！！', '提示信息', MB_OK + MB_ICONERROR);
      Exit;
    end;}
    if RadioButtonBackMode1.Checked then
    begin
      wHour := EditHour1.IntValue;
      wMin := EditMin1.IntValue;
    end else
    begin
      wHour := EditHour2.IntValue;
      wMin := EditMin2.IntValue;
    end;
    //ListItem.Caption := sSource;
    //ListItem.SubItems.Strings[0] := sDest;
    BackUpTask := TBackUpTask(ListItem.SubItems.Objects[0]);
    BackUpTask.SourceDirectory := sSource;
    BackUpTask.DestDirectory := sDest;
    BackUpTask.Hour := wHour;
    BackUpTask.Min := wMin;
    // 是否压缩 piaoyun 2013-08-30
    BackUpTask.IsCompress := chkIsCompress.Checked;

    if RadioButtonBackMode1.Checked then
    begin
      BackUpTask.Mode := 0;
    end else
    begin
      BackUpTask.Mode := 1;
    end;
    BackUpTask.Start := True; //CheckBoxBackUp.Checked;
    RefBackListToView();
    Application.MessageBox('修改成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
  end;
end;

procedure TfrmMain.ButtonBackDelClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := ListViewDataBackup.Selected;
  if ListItem <> nil then
  begin
    if g_BackUpManager.Delete(ListItem.Caption) then
    begin
      RefBackListToView();
      Application.MessageBox('删除成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
    end else Application.MessageBox('删除失败！！！', '提示信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TfrmMain.ButtonBackAddClick(Sender: TObject);
var
  BackUpTask: TBackUpTask;
  sSource, sDest: string;
  wHour, wMin: Word;
begin
  sSource := Trim(EditSource.Text);
  sDest := Trim(EditDest.Text);
  if sSource = '' then
  begin
    Application.MessageBox('请选择数据目录！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  if sDest = '' then
  begin
    Application.MessageBox('请选择备份目录！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  if g_BackUpManager.Find(sSource) <> nil then
  begin
    Application.MessageBox('此数据目录已经存在！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  if RadioButtonBackMode1.Checked then
  begin
    wHour := EditHour1.IntValue;
    wMin := EditMin1.IntValue;
  end else
  begin
    wHour := EditHour2.IntValue;
    wMin := EditMin2.IntValue;
  end;
  BackUpTask := TBackUpTask.Create;
  BackUpTask.SourceDirectory := sSource;
  BackUpTask.DestDirectory := sDest;
  BackUpTask.Hour := wHour;
  BackUpTask.Min := wMin;
  BackUpTask.Start := True; //CheckBoxBackUp.Checked;
  // 是否压缩 piaoyun 2013-08-30
  BackUpTask.IsCompress := chkIsCompress.Checked;

  if RadioButtonBackMode1.Checked then
  begin
    BackUpTask.Mode := 0;
  end else
  begin
    BackUpTask.Mode := 1;
  end;
  g_BackUpManager.Add(BackUpTask);
  RefBackListToView();
  Application.MessageBox('增加成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TfrmMain.ButtonBackSaveClick(Sender: TObject);
begin
  ButtonBackSave.Enabled := False;
  SaveBackList();
  Application.MessageBox('保存成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
  ButtonBackSave.Enabled := True;
end;

procedure TfrmMain.ButtonBackStartClick(Sender: TObject);
begin
  g_BackUpManager.Start := not g_BackUpManager.Start;
  if g_BackUpManager.Start then
  begin
    ButtonBackStart.Caption := '停止(&T)';
    LabelBackMsg.Font.Color := clGreen;
    LabelBackMsg.Caption := '数据备份功能启动中...';
    //g_BackUpManager.Run;
  end else
  begin
    ButtonBackStart.Caption := '启动(&B)';
    LabelBackMsg.Font.Color := clRed;
    LabelBackMsg.Caption := '数据备份功能已停止...';
  end;
end;

procedure TfrmMain.TimerAutoStartServerTimer(Sender: TObject);
var
  wHour, wMin, wSec, wMSec: Word;
begin
  if g_boAutoStartServer then
  begin
    Inc(g_nAutoStartTimeCount);
    if g_nAutoStartTimeCount >= g_nAutoStartDelayTime then
    begin
      TimerAutoStartServer.Enabled := False;
      SetWindowPos(Self.Handle, Self.Handle, Self.Left, Self.Top, Self.Width, Self.Height, $40);
      case m_nStartStatus of
        0:
          begin
            StartGame();
          end;
      end;
    end;
  end;

  if chkTimerStart.Checked then
  begin
    if ComparedateTime(m_StartTime, Now) > -1 then Exit;


    //TimerAutoStartServer.Enabled := False;
    chkTimerStart.Checked := not chkTimerStart.Checked;
    SetWindowPos(Self.Handle, Self.Handle, Self.Left, Self.Top, Self.Width, Self.Height, $40);
    case m_nStartStatus of
      0:
        begin
          StartGame();
        end;
    end;
  end;
end;

procedure TfrmMain.chkAutoStartServerClick(Sender: TObject);
begin
  g_boAutoStartServer := chkAutoStartServer.Checked;
  //chkTimerStart.Checked := not chkAutoStartServer.Checked;
end;

procedure TfrmMain.EditAutoStartDelayTimeChange(Sender: TObject);
begin
  g_nAutoStartDelayTime := EditAutoStartDelayTime.Value;
end;

procedure TfrmMain.CheckBoxboSelGate_GetStart1Click(Sender: TObject);
begin
  if not m_boOpen then Exit;
  g_boSelGate_GetStart1 := CheckBoxboSelGate_GetStart1.Checked;
end;

procedure TfrmMain.CheckBoxSelGate1Click(Sender: TObject);
begin
  g_boSelGate_GetStart1 := CheckBoxSelGate1.Checked;
end;

procedure TfrmMain.CheckBoxboRunGate_GetMultiThreadClick(Sender: TObject);
begin
  g_boRunGate_GetMultiThread := CheckBoxboRunGate_GetMultiThread.Checked;
end;

procedure TfrmMain.ButtonStdModeClick(Sender: TObject);
var
  Query: TQuery;
  sDatabaseName: string;
  sSQLString: string;
begin
  if Application.MessageBox('是否确认自动转换数据，如果您已经使用本工具修改过或者已经手工修改，不要重复转换，否则会导致数据错乱？',
    '提示信息',
    MB_YESNO + MB_ICONQUESTION) = IDYES then
  begin
    sDatabaseName := EditDBName.Text;
    Query := TQuery.Create(nil);
    Query.DatabaseName := sDatabaseName;
    MemoLog1.Lines.Add('转换单个武器和衣服的Shape 1000开始');
//Shape>=100 单个WIL文件
    sSQLString := 'UPDATE StdItems SET Shape=Shape*10 where (Stdmode=5 or Stdmode=6 or Stdmode=10 or Stdmode=11) and (Shape>99) and (Shape<512)';
    Query.SQL.Clear;
    Query.SQL.Add(sSQLString);
    try
      Query.ExecSQL;
    except
      ShowMessage('数据操作错误');
    end;

{
Weapon.wzl Shape 1~99
Weapon2.wzl Shape 100~149
Weapon3.wzl Shape 150~199
Weapon4.wzl Shape 200~249
Weapon5.wzl Shape 250~299
Hum.wzl Shape 1~99
Hum2.wzl Shape 100~149
Hum3.wzl Shape 150~199
Hum4.wzl Shape 200~249
Hum5.wzl Shape 250~299
}

    MemoLog1.Lines.Add('转换hum2.wil Shape 100~149');
//hum2.wil  65-76 -> 100~149     or Stdmode=10 or Stdmode=11
    sSQLString := 'UPDATE StdItems SET Shape = Shape + 35 where (Stdmode=10 or Stdmode=11) and Shape>64 and Shape<77';
    Query.SQL.Clear;
    Query.SQL.Add(sSQLString);
    try
      Query.ExecSQL;
    except
      ShowMessage('数据操作错误');
    end;

//hum3.wil  80-88 -> 150~199
    MemoLog1.Lines.Add('转换hum3.wil Shape 150~199');
    sSQLString := 'UPDATE StdItems SET Shape = Shape + 70 where (Stdmode=10 or Stdmode=11) and Shape>79 and Shape<89';
    Query.SQL.Clear;
    Query.SQL.Add(sSQLString);
    try
      Query.ExecSQL;
    except
      ShowMessage('数据操作错误');
    end;

//Weapon2.wil    65-78 -> 100~149    100~149
    MemoLog1.Lines.Add('转换Weapon2.wil Shape 100~149');
    sSQLString := 'UPDATE StdItems SET Shape = Shape + 35 where (Stdmode=5 or Stdmode=6) and Shape>64 and Shape<79';
    Query.SQL.Clear;
    Query.SQL.Add(sSQLString);
    try
      Query.ExecSQL;
    except
      ShowMessage('数据操作错误');
    end;

//Weapon3.wil    88-93 -> 150~199
    MemoLog1.Lines.Add('转换Weapon3.wil Shape 150~199');
    sSQLString := 'UPDATE StdItems SET Shape = Shape + 62 where (Stdmode=5 or Stdmode=6) and Shape>87 and Shape<94';
    Query.SQL.Clear;
    Query.SQL.Add(sSQLString);
    try
      Query.ExecSQL;
    except
      ShowMessage('数据操作错误');
    end;

//Weapon2.wis    512  -> 200~220
   { MemoLog.Lines.Add('转换Weapon2.wis Shape 200~220');
    sSQLString := 'UPDATE StdItems SET Shape = Shape - 312 where (Stdmode=5 or Stdmode=6) and Shape>511 and Shape<1000';
    Query.SQL.Clear;
    Query.SQL.Add(sSQLString);
    try
      Query.ExecSQL;
    except
      ShowMessage('数据操作错误');
    end; }
    Query.Free;
    MemoLog1.Lines.Add('-----------------------------------转换完成------------------------------------');
  end;
end;

procedure TfrmMain.ButtonMapEventClick(Sender: TObject);
var
  I: Integer;
  LoadList: TStringList;
  sFileName: string;
  tStr, s01, s02, s03, s04: string;
begin
  sFileName := IncludeTrailingBackslash(EditEnvirFilePath.Text) + 'MapEvent.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      tStr := LoadList.Strings[I];
      if (tStr <> '') and (tStr[1] <> ';') then
      begin
        tStr := GetValidStr3(tStr, s01, [' ', #9]);
        tStr := GetValidStr3(tStr, s02, [' ', #9]);
        tStr := GetValidStr3(tStr, s03, [' ', #9]);
        tStr := GetValidStr3(tStr, s04, [' ', #9]);
        if (s01 <> '') and (s02 <> '') and (s03 <> '') and (s04 <> '') then
        begin
          if Pos(':', s04) > 0 then
          begin
            LoadList.Strings[I] := s01 + ' ' + s02 + ' ' + s03 + ' 0 ' + s04 + ' ' + tStr;
          end;
        end;
      end;
    end;
    LoadList.SaveToFile(sFileName);
    LoadList.Free;
    MemoLog1.Lines.Add('-------------------------------MapEvent转换完成--------------------------------');
  end else
  begin
    MemoLog1.Lines.Add('没有发现“' + sFileName + '”');
  end;
end;

procedure TfrmMain.ButtonChangeItemNameColorWhiteClick(Sender: TObject);
var
  Query: TQuery;
  sDatabaseName: string;
  sSQLString: string;
begin
  sDatabaseName := EditDBName.Text;
  Query := TQuery.Create(nil);
  Query.DatabaseName := sDatabaseName;

  sSQLString := 'UPDATE StdItems SET Color=251';                                                    //where (Stdmode=5 or Stdmode=6 or Stdmode=10 or Stdmode=11)
  Query.SQL.Clear;
  Query.SQL.Add(sSQLString);
  try
    Query.ExecSQL;
    MemoLog1.Lines.Add('---------------------------------颜色调整完成----------------------------------');
  except
    ShowMessage('数据操作错误');
  end;
  Query.Free;
end;


procedure TfrmMain.ButtonUnbindItemClick(Sender: TObject);

  function LoadUnbindList(UnbindList: TStringList; sFileName: string): Boolean;
  var
    tStr, sData, s20: string;
    //tUnbind: pTUnbindInfo;
    LoadList: TStringList;
    I: Integer;
    n10: Integer;
  begin
    Result := False;
    if FileExists(sFileName) then
    begin
      Result := True;
      UnbindList.Clear;
      LoadList := TStringList.Create;
      LoadList.LoadFromFile(sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        tStr := LoadList.Strings[I];
        if (tStr <> '') and (tStr[1] <> ';') then
        begin
        //New(tUnbind);
          tStr := GetValidStr3(tStr, sData, [' ', #9]);
          tStr := GetValidStrCap(tStr, s20, [' ', #9]);
          if (s20 <> '') and (s20[1] = '"') then
            ArrestStringEx(s20, '"', '"', s20);

          n10 := StrToIntDef(sData, 0);
          if n10 > 0 then UnbindList.AddObject(s20, TObject(n10))
          else
          begin
         // Result := -I; //需要取负数
          //Break;
          end;
        end;
      end;
      LoadList.Free;
    end else
    begin
      MemoLog1.Lines.Add('没有发现“' + sFileName + '”');
    end;
  end;
var
  I: Integer;
  Query: TQuery;
  sFileName: string;
  sDatabaseName: string;
  sSQLString: string;
  UnbindList: TStringList;
begin
  UnbindList := TStringList.Create;
  sFileName := IncludeTrailingBackslash(EditEnvirFilePath.Text) + 'UnbindList.txt';
  if not LoadUnbindList(UnbindList, sFileName) then
    ShowMessage('“' + sFileName + '”文件没有发现')
  else
  begin
    sDatabaseName := EditDBName.Text;
    Query := TQuery.Create(nil);
    Query.DatabaseName := sDatabaseName;
    for I := 0 to UnbindList.Count - 1 do
    begin
      Application.ProcessMessages;
      MemoLog1.Lines.Add(Format('正在转换 %s', [UnbindList.Strings[I]]));
      sSQLString := Format('UPDATE StdItems SET Anicount=%d where Name=''%s''', [Integer(UnbindList.Objects[I]), UnbindList.Strings[I]]);
      Query.SQL.Clear;
      Query.SQL.Add(sSQLString);
      try
        Query.ExecSQL;
      except
        ShowMessage('数据操作错误:' + sSQLString);
      end;
    end;
    Query.Free;
  end;
  UnbindList.Free;
  MemoLog1.Lines.Add('-----------------------------------转换完成------------------------------------');
end;

procedure TfrmMain.ButtonUnTakeOffItemClick(Sender: TObject);
var
  Query: TQuery;
  sDatabaseName: string;
  sSQLString: string;
begin
  sDatabaseName := EditDBName.Text;
  Query := TQuery.Create(nil);
  Query.DatabaseName := sDatabaseName;

  sSQLString := 'UPDATE StdItems SET Reserved=0';                                                   //where (Stdmode=5 or Stdmode=6 or Stdmode=10 or Stdmode=11)
  Query.SQL.Clear;
  Query.SQL.Add(sSQLString);
  try
    Query.ExecSQL;
    MemoLog1.Lines.Add('-----------------------------------修复完成------------------------------------');
  except
    ShowMessage('数据操作错误');
  end;
  Query.Free;
end;

{[Share]
BaseDir = D: \MirServer\Mir200\Share\
GuildDir = D: \MirServer\Mir200\GuildBase\Guilds\
GuildFile = D: \MirServer\Mir200\GuildBase\Guildlist.txt
VentureDir = D: \MirServer\Mir200\ShareV\
ConLogDir = D: \MirServer\Mir200\ConLog\
CastleDir = D: \MirServer\Mir200\Castle\
CastleFile = D: \MirServer\Mir200\Castle\List.txt
EnvirDir = D: \MirServer\Mir200\Envir\
MapDir = D: \MirServer\Mir200\Map\
NoticeDir = D: \MirServer\Mir200\Notice\
LogDir = D: \MirServer\Mir200\Log\ }

function GetShareDir(nType: Integer): string;
var
  sFileName: string;
  Config: TIniFile;
begin
  Result := '';
  sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConfigFile;
  if FileExists(sFileName) then
  begin
    Config := TIniFile.Create(sFileName);
    case nType of
      0: sFileName := Config.ReadString('Share', 'BaseDir', '');
      1: sFileName := Config.ReadString('Share', 'GuildDir', '');
      2: sFileName := Config.ReadString('Share', 'GuildFile', '');
      3: sFileName := Config.ReadString('Share', 'ConLogDir', '');
      4: sFileName := Config.ReadString('Share', 'CastleDir', '');
      5: sFileName := Config.ReadString('Share', 'CastleFile', '');
      6: sFileName := Config.ReadString('Share', 'EnvirDir', '');
      7: sFileName := Config.ReadString('Share', 'LogDir', '');
    else sFileName := '';
    end;
    //showmessage(inttostr(nType)+' GetShareDir '+sFileName);
    if not FileExists(sFileName) then
    begin
      Result := g_sGameDirectory + g_sM2Server_Directory + sFileName;
      if not FileExists(Result) then
        Result := sFileName;
    end else Result := sFileName;
    Config.Free;
  end;
end;



procedure TfrmMain.btnMyGetTxtOpenClick(Sender: TObject);
var
  sNewDir: string;
begin
  if Sender = BtnMyGetTxtOpen then
  begin
    ClearServerOpenDialog.Filter := '文本文件(*.txt)|*.txt|所有文件(*.*)|*.*';
    if ClearServerOpenDialog.Execute then
      edtMyGetTXT.Text := ClearServerOpenDialog.FileName;
  end;
  if Sender = BtnMyGetFileOpen then
  begin
    ClearServerOpenDialog.Filter := '文本文件(*.txt)|*.txt|所有文件(*.*)|*.*';
    if ClearServerOpenDialog.Execute then
      edtMyGetFile.Text := ClearServerOpenDialog.FileName;
  end;
  if Sender = BtnMyGetDirOpen then
  begin
    if SelectDirectory('请选择要清空的目录', '', sNewDir, Handle) then
    begin
      edtMyGetDir.Text := sNewdir;
    end;
  end;
end;

procedure TfrmMain.btnMyGetTxtAddClick(Sender: TObject);
begin
  if Sender = btnMyGetTxtAdd then
  begin
    if not FileExists(edtMyGetTXT.Text) then
    begin
      application.MessageBox('数据文件不存在', '提示信息', MB_ICONASTERISK);
      Exit;
    end;
    ListBoxAdd(lstMyGetTXT, edtMyGetTXT.Text);
    ClearModValue();
  end;
  if Sender = btnMyGetFileAdd then
  begin
    if not FileExists(edtMyGetFile.Text) then
    begin
      application.MessageBox('数据文件不存在', '提示信息', MB_ICONASTERISK);
      Exit;
    end;
    ListBoxAdd(lstMyGetFile, edtMyGetFile.Text);
    ClearModValue();
  end;
  if Sender = btnMyGetDirAdd then
  begin
    if not DirectoryExists(edtMyGetDir.Text) then
    begin
      application.MessageBox('目录不存在', '提示信息', MB_ICONASTERISK);
      Exit;
    end;
    ListBoxAdd(lstMyGetDir, edtMyGetDir.Text);
    ClearModValue();
  end;
end;

procedure TfrmMain.btnMyGetTxtDelClick(Sender: TObject);
begin
  if Sender = BtnMyGetFileDel then
  begin
    ListBoxDel(lstMyGetFile);
    ClearModValue();
  end;
  if Sender = BtnMyGetTxtDel then
  begin
    ListBoxDel(lstMyGetTXT);
    ClearModValue();
  end;
  if Sender = BtnMyGetDirDel then
  begin
    ListBoxDel(lstMyGetDir);
    ClearModValue();
  end;
end;

function GetClearAccountDBSql: string;
begin
  Result := 'delete from Account;';
end;

function GetClearRoleDataDBSql: string;
begin
  Result :=
    'delete from HeroItemElementAdd;' + sLineBreak +
    'delete from HeroItemAddDataByte;' + sLineBreak +
    'delete from HeroItemAddDataInt;' + sLineBreak +
    'delete from HeroItemAddDataText;' + sLineBreak +
    'delete from HeroItemFlute;' + sLineBreak +
    'delete from HeroItemProgress;' + sLineBreak +
    'delete from HeroItemProperty;' + sLineBreak +
    'delete from HeroItemValueAdd;' + sLineBreak +
    'delete from HeroItems;' + sLineBreak +
    //'-------------------------------' + sLineBreak +
    'delete from HeroAbil;' + sLineBreak +
    'delete from HeroAbilNG;' + sLineBreak +
    'delete from HeroAbilNpcAdd;' + sLineBreak +
    'delete from HeroAbilWine;' + sLineBreak +

    'delete from HeroGodBlessState;' + sLineBreak +
    'delete from HeroMagic;' + sLineBreak +
    'delete from HeroQuestFlag;' + sLineBreak +
    'delete from HeroStatusTime;' + sLineBreak +

    'delete from HeroSkillPower;' + sLineBreak +

    'delete from Hero;' + sLineBreak +

    //'-------------------------------' + sLineBreak +
    'delete from HumanItemElementAdd;' + sLineBreak +
    'delete from HumanItemAddDataByte;' + sLineBreak +
    'delete from HumanItemAddDataInt;' + sLineBreak +
    'delete from HumanItemAddDataText;' + sLineBreak +
    'delete from HumanItemFlute;' + sLineBreak +
    'delete from HumanItemProgress;' + sLineBreak +
    'delete from HumanItemProperty;' + sLineBreak +
    'delete from HumanItemValueAdd;' + sLineBreak +
    'delete from HumanItems;' + sLineBreak +
    //'-------------------------------' + sLineBreak +
    'delete from HumanAbil;' + sLineBreak +
    'delete from HumanAbilNG;' + sLineBreak +
    'delete from HumanAbilNpcAdd;' + sLineBreak + 
    'delete from HumanAbilWine;' + sLineBreak + 
    'delete from HumanGamePetData;' + sLineBreak + 
    'delete from HumanGodBlessState;' + sLineBreak + 
    'delete from HumanMagic;' + sLineBreak +
    'delete from HumanMagicUseTick;' + sLineBreak + 
    'delete from HumanQuestFlag;' + sLineBreak +
    'delete from HumanStatusTime;' + sLineBreak +
    'delete from HumanVariableT;' + sLineBreak +
    'delete from HumanVariableU;' + sLineBreak +

    'delete from HumanSkillPower;' + sLineBreak +

    'delete from Human;' + sLineBreak;
end;

function GetClearUserShopSql: string;
begin
  Result :=
    'delete from ItemElementAdd where ItemType = 1;' + sLineBreak +
    'delete from ItemAddDataByte where ItemType = 1;' + sLineBreak +
    'delete from ItemAddDataInt where ItemType = 1;' + sLineBreak +
    'delete from ItemAddDataText where ItemType = 1;' + sLineBreak +
    'delete from ItemFlute where ItemType = 1;' + sLineBreak +
    'delete from ItemProgress where ItemType = 1;' + sLineBreak +
    'delete from ItemProperty where ItemType = 1;' + sLineBreak +
    'delete from ItemValueAdd where ItemType = 1;' + sLineBreak +
    'delete from Items where ItemType = 1;' + sLineBreak +
    //'-------------------------------' + sLineBreak +

    'delete from UserShopItem;' + sLineBreak +
    'delete from UserShop;' + sLineBreak;
end;

function GetClearStorageExSql: string;
begin
  Result :=
    'delete from ItemElementAdd where ItemType = 0;' + sLineBreak +
    'delete from ItemAddDataByte where ItemType = 0;' + sLineBreak +
    'delete from ItemAddDataInt where ItemType = 0;' + sLineBreak +
    'delete from ItemAddDataText where ItemType = 0;' + sLineBreak +
    'delete from ItemFlute where ItemType = 0;' + sLineBreak +
    'delete from ItemProgress where ItemType = 0;' + sLineBreak +
    'delete from ItemProperty where ItemType = 0;' + sLineBreak +
    'delete from ItemValueAdd where ItemType = 0;' + sLineBreak +
    'delete from Items where ItemType = 0;' + sLineBreak +
    //'-------------------------------' + sLineBreak +

    'delete from StorageEx;' + sLineBreak;
end;

function GetClearAuctionDataSql: string;
begin
  Result :=
    'delete from ItemElementAdd where ItemType = 5;' + sLineBreak +
    'delete from ItemAddDataByte where ItemType = 5;' + sLineBreak +
    'delete from ItemAddDataInt where ItemType = 5;' + sLineBreak +
    'delete from ItemAddDataText where ItemType = 5;' + sLineBreak +
    'delete from ItemFlute where ItemType = 5;' + sLineBreak +
    'delete from ItemProgress where ItemType = 5;' + sLineBreak +
    'delete from ItemProperty where ItemType = 5;' + sLineBreak +
    'delete from ItemValueAdd where ItemType = 5;' + sLineBreak +
    'delete from Items where ItemType = 5;' + sLineBreak +
    //'-------------------------------' + sLineBreak +

    'delete from AuctionAttention;' + sLineBreak +
    'delete from AuctionData;' + sLineBreak;

    //'-------------------------------' + sLineBreak +
    //'update sqlite_sequence set seq = 0;' + sLineBreak;
end;

procedure TfrmMain.btnClearSaveClick(Sender: TObject);
begin
  if Clear_SaveConfig then
    btnClearSave.Enabled := False;
end;

procedure TfrmMain.btnStartClearClick(Sender: TObject);

  procedure DelFile(dir: string; sFileName: string);
  var
    fhandle: THandle;
    FindData: TWin32FindData;
  begin
    dir := IncludeTrailingBackslash(dir);
    if not DirectoryExists(dir) then
      exit;

    fhandle := FindFirstFile(PChar(dir + sFileName), FindData);
    if fhandle = INVALID_HANDLE_VALUE then Exit;

    repeat
      Application.ProcessMessages;
      if (CompareText(FindData.cFileName, '.') <> 0) and
        (CompareText(FindData.cFileName, '..') <> 0) then
      begin
        if FindData.dwFileAttributes and FILE_ATTRIBUTE_DIRECTORY > 0 then
        begin

        end
        else
          Windows.DeleteFile(PChar(dir + finddata.cFileName));
      end;
    until not FindNextFile(fhandle, FindData);

    Windows.FindClose(fhandle);
  end;
  (*var
    ProcessInfo: TProcessInformation;
    StartUpInfo: TStartupInfo;
    StringList: TStringList;
  begin
    StringList := TStringList.Create;
    try
      if boSubDir then
        StringList.Add('del /S  ' + sFileName)
      else
        StringList.Add('del /S  ' + sFileName);
      StringList.Add('del %0');
      StringList.SaveToFile(BatchFileName);
    finally
      StringList.Free;
    end;
    winexec(pchar(BatchFileName), SW_HIDE);
    {
    FillChar(StartUpInfo, SizeOf(StartUpInfo), $00);
    StartUpInfo.dwFlags := STARTF_USESHOWWINDOW;
    StartUpInfo.wShowWindow := SW_HIDE;
    if CreateProcess(nil, PChar(BatchFileName), nil, nil,
      False, IDLE_PRIORITY_CLASS, nil, PChar(BatchFileName), StartUpInfo,
      ProcessInfo) then begin
      CloseHandle(ProcessInfo.hThread);
      CloseHandle(ProcessInfo.hProcess);
    end;}
  end;
  *)
  // 删除指定目录及子目录，指定类型文件 piaoyun 2013-08-28

  procedure DelFileEx(Path: string; FileExt: string);
  var
    sch: TSearchrec;
  begin
    if RightStr(trim(Path), 1) <> '\' then
      Path := trim(Path) + '\'
    else
      Path := trim(Path);

    if not DirectoryExists(Path) then
    begin
      exit;
    end;

    if FindFirst(Path + '*', faAnyfile, sch) = 0 then
    begin
      repeat
        Application.ProcessMessages;
        if ((sch.Name = '.') or (sch.Name = '..')) then Continue;
        if DirectoryExists(Path + sch.Name) then
        begin
          DelFileEx(Path + sch.Name, FileExt);
        end
        else
        begin
          if (UpperCase(extractfileext(Path + sch.Name)) = UpperCase(FileExt)) or (FileExt = '.*') then
          //Result.Add(Path + sch.Name);
            Windows.DeleteFile(PChar(Path + sch.Name));
        end;
      until FindNext(sch) <> 0;
      SysUtils.FindClose(sch);
    end;
  end;

  procedure DelFileAndDir(dir: string; boSub: Boolean = False);
  var
    fhandle: THandle;
    FindData: TWin32FindData;
  begin
    dir := IncludeTrailingBackslash(dir);
    if not DirectoryExists(dir) then
      exit;

    fhandle := FindFirstFile(PChar(dir + '\*.*'), FindData);
    if fhandle = INVALID_HANDLE_VALUE then Exit;

    repeat
      Application.ProcessMessages;
      if (CompareText(FindData.cFileName, '.') <> 0) and
        (CompareText(FindData.cFileName, '..') <> 0) then
      begin
        if FindData.dwFileAttributes and FILE_ATTRIBUTE_DIRECTORY > 0 then
        begin
          DelFileAndDir(dir + FindData.cFileName, True);
        end
        else
          Windows.DeleteFile(PChar(dir + finddata.cFileName));
      end;
    until not FindNextFile(fhandle, FindData);

    Windows.FindClose(fhandle);
    if boSub then
      RmDir(dir);
  end;

  (*var
    ProcessInfo: TProcessInformation;
    StartUpInfo: TStartupInfo;
    StringList: TStringList;
  begin
    {StringList := TStringList.Create;
    with StringList do begin
      try
        Add('del /S  ' + sFileName);
        Add('for /f "delims=" %%a in (''dir/ad/b/s *'') do rd /s /q "%%a" ');
        Add('del %0');
        SaveToFile(BatchFileName);
      finally
        StringList.Free;
      end;
    end; }
    {winexec(pchar(BatchFileName),SW_HIDE);
   { showmessage('DelFileAndDir 1 '+BatchFileName);
    FillChar(StartUpInfo, SizeOf(StartUpInfo), $00);
    StartUpInfo.dwFlags := STARTF_USESHOWWINDOW;
    StartUpInfo.wShowWindow := SW_HIDE;
    if CreateProcess(nil, PChar(BatchFileName), nil, nil,
      False, IDLE_PRIORITY_CLASS, nil, PChar(BatchFileName), StartUpInfo,
      ProcessInfo) then begin
      CloseHandle(ProcessInfo.hThread);
      CloseHandle(ProcessInfo.hProcess);
      showmessage('DelFileAndDir 2 '+BatchFileName);
    end;}
  end;*)
var
  I: Integer;
  sFilePath: string;
  sFileName: string;
  Config: TIniFile;
  StringList: TStringList;
  StringList1: TStringList;
  SQLite3DB: TSqlite3DataBase;
  MySqlDB: TMySQLDataBase;
  MySqlLib: TMySQLLib;
  sSql: string;
begin
  if Application.MessageBox('是否确认清除服务端数据?', '确认信息', MB_YESNO + MB_ICONQUESTION) <> mrYes then Exit;

  if not SameText(g_sGameDirectory, Copy(Application.ExeName, 1, Length(g_sGameDirectory))) then
  begin
    Application.MessageBox('清理数据路径与控制器所在服务端路径不一致！', '清理失败', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  if not SameText(ButtonStartGame.Caption, g_sButtonStartGame) then
  begin
    Application.MessageBox('请先停止游戏服务器，再清理数据！', '清理失败', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  btnStartClear.Enabled := False;
  sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConfigFile;

  if FileExists(sFileName) then
  begin
    Config := TIniFile.Create(sFileName);
    try
      Config.WriteBool('Setup', 'FirstRun', False);
    finally
      Config.Free;
    end;
  end;

  if CheckGroupClear.ItemChecked[0] then
  begin
    if g_nDataSaveDBType = 0 then
    begin
      SQLite3DB := TSqlite3DataBase.Create();
      try
        SQLite3DB.MustExist := True;
        SQLite3DB.Database := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_IdDir + 'Account.DB';
        SQLite3DB.Connected := True;

        SQLite3DB.Execute(GetClearAccountDBSql);
        Sqlite3DB.Execute('vacuum;')
      finally
        Sqlite3DB.Free;
      end;
    end
    else
    begin
      MySqlLib := TMySQLLib.Create(nil);
      try
        MySqlLib.Load('', 'libmysql-32.dll');

        MySqlDB := TMySQLDataBase.Create(MySqlLib);
        try
          MySqlDB.Init;
          MySqlDB.Connect(g_sDataSaveDBServer, g_sDataSaveDBUser, g_sDataSaveDBPassword, g_sDataSaveDataBase, g_wDataSaveDBPort, CLIENT_MULTI_STATEMENTS);

          MySqlDB.Exec(GetClearAccountDBSql);
        finally
          MySqlDB.Free;
        end;
      finally
        MySqlLib.Free;
      end;
    end;

    sFileName := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_CountLogDir;
    if DirectoryExists(sFileName) then
    begin
      DelFileAndDir(sFileName, False);
    end;

    sFileName := g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_ChrLogDir;
    if DirectoryExists(sFileName) then
    begin
      DelFileAndDir(sFileName, False);
    end;
  end;


  if CheckGroupClear.ItemChecked[1] then
  begin
    if g_nDataSaveDBType = 0 then
    begin
      SQLite3DB := TSqlite3DataBase.Create();
      try
        SQLite3DB.MustExist := True;
        SQLite3DB.Database := g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_HumDir + 'RoleData.DB';
        SQLite3DB.Connected := True;

        SQLite3DB.BeginTransaction;
        try
          SQLite3DB.Execute(GetClearRoleDataDBSql + 'update sqlite_sequence set seq = 0;');
          SQLite3DB.Commit;
        except
          on E: Exception do
          begin
            SQLite3DB.RollBack;
            raise Exception.Create(E.Message);
          end;
        end;
        Sqlite3DB.Execute('vacuum;')
      finally
        Sqlite3DB.Free;
      end;
    end
    else
    begin
      MySqlLib := TMySQLLib.Create(nil);
      try
        MySqlLib.Load('', 'libmysql-32.dll');

        MySqlDB := TMySQLDataBase.Create(MySqlLib);
        try
          MySqlDB.Init;
          MySqlDB.Connect(g_sDataSaveDBServer, g_sDataSaveDBUser, g_sDataSaveDBPassword, g_sDataSaveDataBase, g_wDataSaveDBPort, CLIENT_MULTI_STATEMENTS);

          MySqlDB.StartTransaction;
          try
            MySqlDB.Exec(GetClearRoleDataDBSql);
            MySqlDB.Commit;
          except
            on E: Exception do
            begin
              MySqlDB.RollBack;
              raise Exception.Create(E.Message);
            end;
          end;
        finally
          MySqlDB.Free;
        end;
      finally
        MySqlLib.Free;
      end;
    end;
  end;

  if CheckGroupClear.ItemChecked[2] or CheckGroupClear.ItemChecked[3] or CheckGroupClear.ItemChecked[4] then
  begin
    if g_nDataSaveDBType = 0 then
    begin
      sSql := '';

      if CheckGroupClear.ItemChecked[2] then
        sSql := sSql + GetClearUserShopSql + 'update sqlite_sequence set seq = 0 where name = ''UserShop'';';

      if CheckGroupClear.ItemChecked[3] then
        sSql := sSql + GetClearStorageExSql + 'update sqlite_sequence set seq = 0 where name = ''StorageEx'';';

      if CheckGroupClear.ItemChecked[4] then
        sSql := sSql + GetClearAuctionDataSql;

      SQLite3DB := TSqlite3DataBase.Create();
      try
        SQLite3DB.MustExist := True;
        SQLite3DB.Database := g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB';
        SQLite3DB.Connected := True;

        SQLite3DB.BeginTransaction;
        try
          SQLite3DB.Execute(sSql);
          SQLite3DB.Commit;
        except
          on E: Exception do
          begin
            SQLite3DB.RollBack;
            raise Exception.Create(E.Message);
          end;
        end;
        Sqlite3DB.Execute('vacuum;')
      finally
        Sqlite3DB.Free;
      end;
      if FileExists(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-shm') then
      begin
        try
          DeleteFile(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-shm');
        except

        end;  
      end;
      if FileExists(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-wal') then
      begin
        try
          DeleteFile(g_sGameDirectory + g_sM2Server_Directory + 'M2Data\M2Data.DB-wal');
        except

        end;  
      end;
    end
    else
    begin
      sSql := '';

      if CheckGroupClear.ItemChecked[2] then
        sSql := sSql + GetClearUserShopSql;

      if CheckGroupClear.ItemChecked[3] then
        sSql := sSql + GetClearStorageExSql;

      if CheckGroupClear.ItemChecked[4] then
        sSql := sSql + GetClearAuctionDataSql;


      MySqlLib := TMySQLLib.Create(nil);
      try
        MySqlLib.Load('', 'libmysql-32.dll');

        MySqlDB := TMySQLDataBase.Create(MySqlLib);
        try
          MySqlDB.Init;
          MySqlDB.Connect(g_sDataSaveDBServer, g_sDataSaveDBUser, g_sDataSaveDBPassword, g_sDataSaveDataBase, g_wDataSaveDBPort, CLIENT_MULTI_STATEMENTS);

          MySqlDB.StartTransaction;
          try
            MySqlDB.Exec(sSql);
            MySqlDB.Commit;
          except
            on E: Exception do
            begin
              MySqlDB.RollBack;
              raise Exception.Create(E.Message);
            end;
          end;
        finally
          MySqlDB.Free;
        end;
      finally
        MySqlLib.Free;
      end;
    end;
  end;

  if CheckGroupClear.ItemChecked[5] then
  begin
    sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir + 'market_upg';
    if DirectoryExists(sFileName) then
    begin
      //DelFile(sFileName, '*.upg');
      DelFileEx(sFileName, '.upg');
    end;
  end;

  if CheckGroupClear.ItemChecked[6] then
  begin
    sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir + 'Market_Prices';
    if DirectoryExists(sFileName) then
    begin
      DelFileEx(sFileName, '.prc');
    end;
    sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir + 'Market_Saved';
    if DirectoryExists(sFileName) then
    begin
      DelFileEx(sFileName, '.sav');
    end;
  end;

  if CheckGroupClear.ItemChecked[7] then
  begin
    sFileName := GetShareDir(1);                                                                    //g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_GuildDir;
    if DirectoryExists(sFileName) then
    begin
      DelFile(sFileName, '*.txt');
      DelFile(sFileName, '*.ini');
      DelFile(sFileName, '*.add');
    end;
    sFileName := GetShareDir(2);                                                                    //g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_GuildFile;
   // showmessage('sFileName ' + sFileName);
    if FileExists(sFileName) then
    begin
     // showmessage('sFileName ' + sFileName);
      StringList := TStringList.Create;
      try
        StringList.SaveToFile(sFileName);
      finally
        StringList.Free;
      end;
    end;
  end;

  if CheckGroupClear.ItemChecked[8] then
  begin
    sFilePath := GetShareDir(4);
    if DirectoryExists(sFilePath) then
    begin
      sFileName := GetShareDir(5);
      if FileExists(sFileName) then
      begin
        StringList := TStringList.Create;
        try
          StringList.LoadFromFile(sFileName);
          for I := 0 to StringList.Count - 1 do
          begin
            sFileName := sFilePath + StringList.Strings[I] + '\SabukW.txt';
            if FileExists(sFileName) then
            begin
              Config := TIniFile.Create(sFileName);
              Config.WriteString('Setup', 'OwnGuild', '');
              Config.Free;
            end;

            sFileName := sFilePath + StringList.Strings[I] + '\AttackSabukWall.txt';
            if FileExists(sFileName) then
            begin
              StringList1 := TStringList.Create;
              try
                StringList1.SaveToFile(sFileName);
              finally
                StringList1.Free;
              end;
            end;
          end;
        finally
          StringList.Free;
        end;
      end;
    end;
  end;

  if CheckGroupClear.ItemChecked[9] then
  begin
    sFileName := GetShareDir(3);
    if DirectoryExists(sFileName) then
    begin
      //showmessage(sFileName);
      DelFileAndDir(sFileName);
    end;
    sFileName := GetShareDir(7);
    if DirectoryExists(sFileName) then
    begin
      //showmessage(sFileName);
      DelFile(sFileName, '*.txt');
    end;
  end;

  if CheckGroupClear.ItemChecked[10] then
  begin

    sFileName := g_sGameDirectory + g_sLogServer_Directory + g_sLogServer_BaseDir;
    //showmessage(sFileName);
    if DirectoryExists(sFileName) then
    begin
      //showmessage(sFileName);
      DelFileAndDir(sFileName);
    end;
  end;

  if CheckGroupClear.ItemChecked[11] then
  begin
    ClearSetupIni;
    ClearGlobal(g_sGameDirectory + g_sM2Server_Directory + 'GlobalVal.ini')
  end;

  if CheckGroupClear.ItemChecked[12] then
  begin
    sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir + 'MasterNo';
    if DirectoryExists(sFileName) then
    begin
      DelFileAndDir(sFileName, False);
    end;
  end;

  if CheckGroupClear.ItemChecked[13] then
  begin
    sFileName := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir + 'Npc_data';
    if DirectoryExists(sFileName) then
    begin
      DelFileAndDir(sFileName, False);
    end;
  end;
  //删除国家数据
  if CheckGroupClear.ItemChecked[14] then
  begin
    sFilePath := g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir + 'Nations\';
    Config := TIniFile.Create(sFilePath + 'Nations.ini');
    try
      for i := 1 to 1000 do begin
        sFileName := Trim(Config.ReadString('Names', 'NationalNames' + IntToStr(i), ''));
        if sFileName <> '' then begin
          Config.WriteString('Names', 'NationalNames' + IntToStr(i), '');
          DeleteFile(Format('%s%s.ini', [sFilePath, sFileName]));
        end;
      end;
    finally
      Config.Free;
    end;
  end;

  // 自定义目录清除 piaoyun 2013-08-28

  for i := 0 to lstMyGetTXT.Count - 1 do
  begin
    if FileExists(lstMyGetTXT.Items.Strings[i]) then
    begin
      ClearTxt(lstMyGetTXT.Items.Strings[i]);
    end;
  end;

  for i := 0 to lstMyGetFile.Count - 1 do
  begin
    if FileExists(lstMyGetFile.Items.Strings[i]) then
    begin
      DeleteFile(lstMyGetFile.Items.Strings[i]);
    end;
  end;

  for i := 0 to lstMyGetDir.Count - 1 do
  begin
    if DirectoryExists(lstMyGetDir.Items.Strings[i]) then
    begin
      DelFileAndDir(lstMyGetDir.Items.Strings[i]);
    end;
  end;

  Application.MessageBox('数据清理已完成', '完成', MB_OK + MB_ICONINFORMATION);
  btnStartClear.Enabled := True;
end;

procedure TfrmMain.chkDynamicIPModeClick(Sender: TObject);
begin
  EditGameExtIPaddr.Enabled := not chkDynamicIPMode.Checked;
  EditGameExtNetComIPaddr.Enabled := not chkDynamicIPMode.Checked;
end;

procedure TfrmMain.chkTimerStartClick(Sender: TObject);
begin
  if Sender = chkTimerStart then
  begin
    dtpDate.Enabled := not chkTimerStart.Checked;
    dtpTime.Enabled := not chkTimerStart.Checked;
    TimerAutoStartServer.Enabled := chkTimerStart.Checked;
  end;
  m_StartTime := trunc(dtpDate.Date) + Frac(dtpTime.Time);
end;

procedure TfrmMain.chkAutoStartClick(Sender: TObject);
var
  Conini: Tinifile;
  FileName: string;
begin
  FileName := ExtractFilePath(ParamStr(0)) + 'BackConfig.txt';
  Conini := Tinifile.Create(FileName);
  Conini.WriteBool('Setup', 'AutoStart', chkAutoStart.Checked);
  Conini.Free;
end;

procedure TfrmMain.btn2Click(Sender: TObject);
var
  I: Integer;
begin
  g_nLoginGate_GatePort := g_nLoginGate_GatePort + sePortInc.Value;
  EditLoginGate_GatePort.Text := IntToStr(g_nLoginGate_GatePort);

  ///-----------------
  g_nSeLGate_GatePort := g_nSeLGate_GatePort + sePortInc.Value;
  g_nSeLGate_GatePort1 := g_nSeLGate_GatePort1 + sePortInc.Value;

  EditSelGate_GatePort.Text := IntToStr(g_nSeLGate_GatePort);
  EditSelGate_GatePort1.Text := IntToStr(g_nSeLGate_GatePort1);

  ///-----------------
  for I := 0 to MAXRUNGATECOUNT - 1 do
  begin
    g_RunGateInfo[I].nGatePort := g_RunGateInfo[I].nGatePort + sePortInc.Value;
    g_RunGateInfo[I].nDBPort := g_RunGateInfo[I].nDBPort + sePortInc.Value;
  end;

  EditRunGate_GatePort1.Text := IntToStr(g_RunGateInfo[0].nGatePort);
  EditRunGate_GatePort2.Text := IntToStr(g_RunGateInfo[1].nGatePort);
  EditRunGate_GatePort3.Text := IntToStr(g_RunGateInfo[2].nGatePort);
  EditRunGate_GatePort4.Text := IntToStr(g_RunGateInfo[3].nGatePort);
  EditRunGate_GatePort5.Text := IntToStr(g_RunGateInfo[4].nGatePort);
  EditRunGate_GatePort6.Text := IntToStr(g_RunGateInfo[5].nGatePort);
  EditRunGate_GatePort7.Text := IntToStr(g_RunGateInfo[6].nGatePort);
  EditRunGate_GatePort8.Text := IntToStr(g_RunGateInfo[7].nGatePort);

  edtRunGate_DBPort1.Text := IntToStr(g_RunGateInfo[0].nDBPort);
  edtRunGate_DBPort2.Text := IntToStr(g_RunGateInfo[1].nDBPort);
  edtRunGate_DBPort3.Text := IntToStr(g_RunGateInfo[2].nDBPort);
  edtRunGate_DBPort4.Text := IntToStr(g_RunGateInfo[3].nDBPort);
  edtRunGate_DBPort5.Text := IntToStr(g_RunGateInfo[4].nDBPort);
  edtRunGate_DBPort6.Text := IntToStr(g_RunGateInfo[5].nDBPort);
  edtRunGate_DBPort7.Text := IntToStr(g_RunGateInfo[6].nDBPort);
  edtRunGate_DBPort8.Text := IntToStr(g_RunGateInfo[7].nDBPort);

  g_nRunGateDBPort_MulThread := g_nRunGateDBPort_MulThread + sePortInc.Value;
  edtRunGate_DBPortMulThread.Text := IntToStr(g_nRunGateDBPort_MulThread);
  ///-----------------
  g_nLoginServer_GatePort := g_nLoginServer_GatePort + sePortInc.Value;
  g_nLoginServer_ServerPort := g_nLoginServer_ServerPort + sePortInc.Value;

  if g_nLoginServer_ControlPort > 0 then
  begin
    g_nLoginServer_ControlPort := g_nLoginServer_ControlPort + sePortInc.Value;
  end;

  EditLoginServerGatePort.Text := IntToStr(g_nLoginServer_GatePort);
  EditLoginServerServerPort.Text := IntToStr(g_nLoginServer_ServerPort);
  EditLoginServerControlPort.Text := IntToStr(g_nLoginServer_ControlPort);

  ///-----------------
  g_nDBServer_Config_GatePort := g_nDBServer_Config_GatePort + sePortInc.Value;
  g_nDBServer_Config_ServerPort := g_nDBServer_Config_ServerPort + sePortInc.Value;

  EditDBServerGatePort.Text := IntToStr(g_nDBServer_Config_GatePort);
  EditDBServerServerPort.Text := IntToStr(g_nDBServer_Config_ServerPort);

  ///-----------------
  g_nLogServer_Port := g_nLogServer_Port + sePortInc.Value;
  EditLogServerPort.Text := IntToStr(g_nLogServer_Port);

  ///-----------------
  g_nM2Server_GatePort := g_nM2Server_GatePort + sePortInc.Value;
  g_nM2Server_MsgSrvPort := g_nM2Server_MsgSrvPort + sePortInc.Value;

  EditM2ServerGatePort.Text := IntToStr(g_nM2Server_GatePort);
  EditM2ServerMsgSrvPort.Text := IntToStr(g_nM2Server_MsgSrvPort);
end;

procedure TfrmMain.chkEmbeddedWindowClick(Sender: TObject);
begin
  g_boEmbeddedWindow := chkEmbeddedWindow.Checked;
  g_IniConf.WriteBool('GameConf', 'EmbeddedWindow', g_boEmbeddedWindow);

  (*
  if DBServer.MainFormHandle <> 0 then
  begin
    SendMessage(DBServer.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if LoginServer.MainFormHandle <> 0 then
  begin
    SendMessage(LoginServer.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if LogServer.MainFormHandle <> 0 then
  begin
    SendMessage(LogServer.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if M2Server.MainFormHandle <> 0 then
  begin
    SendMessage(M2Server.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if g_boRunGate_GetMultiThread then
  begin
    // add chongchong 多线程网关配置生成 2014-06-22
    GetMutRunGateConfingEx;
    if RunGate[0].MainFormHandle <> 0 then
    begin
      SendMessage(RunGate[0].MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
    end;
  end else
  begin
    GetMutRunGateZero;
    for I := Low(RunGate) to High(RunGate) do
    begin
      if RunGate[I].MainFormHandle <> 0 then
      begin
        SendMessage(RunGate[I].MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
      end;
    end;
  end;

  if SelGate.MainFormHandle <> 0 then
  begin
    SendMessage(SelGate.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if SelGate1.MainFormHandle <> 0 then
  begin
    SendMessage(SelGate1.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if LoginGate.MainFormHandle <> 0 then
  begin
    SendMessage(LoginGate.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;

  if LoginGate1.MainFormHandle <> 0 then
  begin
    SendMessage(LoginGate1.MainFormHandle, WM_SET_PARENT_WINDOW, 0, 0);
  end;
  *)
end;

procedure TfrmMain.GenBackupConfig;
Label DoExit;
var
  I, Len, Index: Integer;
  BackUpTask: TBackUpTask;
  Conini: Tinifile;

  nMyGetTxtNum, nMyGetFileNum, nMyGetDirNum: Integer;
  S: string;
begin
  DeleteFile(ExtractFilePath(ParamStr(0)) + 'BackList.txt');
  Conini := Tinifile.Create(ExtractFilePath(ParamStr(0)) + 'BackList.txt');
  try
    if Conini <> nil then
    begin
      for I := 0 to g_BackUpManager.m_BackUpList.Count - 1 do
      begin
        BackUpTask := TBackUpTask(g_BackUpManager.m_BackUpList.Items[I]);

        if Length(g_sOldGameDirectory) > 0 then
        begin
          if SameText(LeftStr(BackUpTask.SourceDirectory, Length(g_sOldGameDirectory)), g_sOldGameDirectory) then
          begin
            Len := Length(g_sOldGameDirectory);
            BackUpTask.SourceDirectory := g_sGameDirectory + Copy(BackUpTask.SourceDirectory, Len + 1, MaxInt);
          end
          else
          begin
            ShowMessage('请检查备份路径配置是否正确');
            goto DoExit;
          end;

          if SameText(LeftStr(BackUpTask.DestDirectory, Length(g_sOldGameDirectory)), g_sOldGameDirectory) then
          begin
            Len := Length(g_sOldGameDirectory);
            BackUpTask.DestDirectory := g_sGameDirectory + Copy(BackUpTask.DestDirectory, Len + 1, MaxInt);
          end
          else
          begin
            ShowMessage('请检查备份路径配置是否正确');
            goto DoExit;
          end;
        end
        else
        begin
          ShowMessage('请检查备份路径配置是否正确');
          goto DoExit;
        end;

        Conini.WriteString(IntToStr(I), 'Source', BackUpTask.SourceDirectory);
        Conini.WriteString(IntToStr(I), 'Save', BackUpTask.DestDirectory);
        Conini.WriteInteger(IntToStr(I), 'Hour', BackUpTask.Hour);
        Conini.WriteInteger(IntToStr(I), 'Min', BackUpTask.Min);
        Conini.WriteInteger(IntToStr(I), 'BackMode', BackUpTask.Mode);
        Conini.WriteBool(IntToStr(I), 'GetBack', BackUpTask.Start);
        // 是否压缩 piaoyun 2013-08-30
        Conini.WriteBool(IntToStr(I), 'IsCompress', BackUpTask.IsCompress);
      end;
    end;


    nMyGetTxtNum := g_IniConf.ReadInteger('ClearServer', 'MyGetTxtNum', 0);
    nMyGetFileNum := g_IniConf.ReadInteger('ClearServer', 'MyGetFileNum', 0);
    nMyGetDirNum := g_IniConf.ReadInteger('ClearServer', 'MyGetDirNum', 0);
    if nMyGetTxtNum <> 0 then
    begin
      frmMain.lstMyGetTXT.Items.Clear;
      for I := 0 to nMyGetTxtNum - 1 do
      begin

        S := g_IniConf.ReadString('ClearServer', 'MyGetTxt' + IntToStr(I), '读取配置文件错误');
        if SameText(LeftStr(S, Length(g_sOldGameDirectory)), g_sOldGameDirectory) then
        begin
          Len := Length(g_sOldGameDirectory);
          S := g_sGameDirectory + Copy(S, Len + 1, MaxInt);
        end;
        g_IniConf.WriteString('ClearServer', 'MyGetTxt' + IntToStr(I), S);

        frmMain.lstMyGetTXT.Items.Add(S);
      end;
    end;

    if nMyGetFileNum <> 0 then
    begin
      frmMain.lstMyGetFile.Items.Clear;
      for I := 0 to nMyGetFileNum - 1 do
      begin
        S := g_IniConf.ReadString('ClearServer', 'MyGetFile' + IntToStr(I), '读取配置文件错误');

        if SameText(LeftStr(S, Length(g_sOldGameDirectory)), g_sOldGameDirectory) then
        begin
          Len := Length(g_sOldGameDirectory);
          S := g_sGameDirectory + Copy(S, Len + 1, MaxInt);
        end;
        g_IniConf.WriteString('ClearServer', 'MyGetFile' + IntToStr(I), S);

        frmMain.lstMyGetFile.Items.Add(S);
      end;
    end;
    if nMyGetDirNum <> 0 then
    begin
      frmMain.lstMyGetDir.Items.Clear;
      for I := 0 to nMyGetDirNum - 1 do
      begin
        S := g_IniConf.ReadString('ClearServer', 'MyGetDir' + IntToStr(I), '读取配置文件错误');

        if SameText(LeftStr(S, Length(g_sOldGameDirectory)), g_sOldGameDirectory) then
        begin
          Len := Length(g_sOldGameDirectory);
          S := g_sGameDirectory + Copy(S, Len + 1, MaxInt);
        end;
        g_IniConf.WriteString('ClearServer', 'MyGetDir' + IntToStr(I), S);

        frmMain.lstMyGetDir.Items.Add(S);
      end;
    end;

  DoExit:
    RefBackListToView; 

  finally
    if Conini <> nil then Conini.Free;
  end;
end;

procedure TfrmMain.edtSqliteDBButtonClick(Sender: TObject);
var
  OpenDlg: TOpenDialog;
begin
  OpenDlg := TOpenDialog.Create(nil);
  try
    OpenDlg.Filter := 'Sqlite数据库文件(*.db)|*.db';
    OpenDlg.Title := '指定sqlite数据库文件';
    if OpenDlg.Execute then
    begin
      edtSqliteDB.Text := OpenDlg.FileName;
    end;
  finally
    OpenDlg.Free;
  end;
end;

procedure TfrmMain.rbBDEClick(Sender: TObject);
var
  FrmHeroDB: TFrmHeroDB;
begin
  EditHeroDB.Enabled := True;
  edtSqliteDB.Enabled := False;

  if rbBDE.Checked and Assigned(FrmBDEToSqlite) then
  begin
    FrmHeroDB := TFrmHeroDB.Create(nil);
    try
      g_boHeroDBOK := not FrmHeroDB.CheckHeroDB;
      if not g_boHeroDBOK then
        FrmHeroDB.Open;
    finally
      FrmHeroDB.Free;
    end;
    if Application.MessageBox(PChar('BDE数据库在64位下可能不支持部分脚本命令,是否将BDE数据库转换为SQLite数据库？'),
      '提示信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
    begin
      FrmBDEToSqlite.Open(EditGameDir.Text);
      if FrmBDEToSqlite.ModalResult = mrOk then
      begin
        rbSqlite.Checked := True;
        rbSqlite.SetFocus;
      end;
    end;
  end;
end;

procedure TfrmMain.rbSqliteClick(Sender: TObject);
begin
  EditHeroDB.Enabled := False;
  edtSqliteDB.Enabled := True;
end;

procedure TfrmMain.btnSetPathClick(Sender: TObject);
var
  I: Integer;
  AppPath, sTempPath, S: string;
  SL: TStringList;
  IniFile: TIniFile;
begin
  AppPath := ExtractFilePath(Application.ExeName);

  if Application.MessageBox(PChar('是否要将服务器目录设置为 ' + AppPath + ' ?'), '确认信息', MB_YESNO + MB_ICONQUESTION) = mrNo then
  begin
    Exit;
  end;

  g_IniConf.WriteString('GameConf', 'GameDirectory', AppPath);
  if Length(g_sSqliteDBName) > 0 then
  begin
    g_sSqliteDBName := StringReplace(g_sSqliteDBName, g_sGameDirectory, AppPath, [rfIgnoreCase]);
    g_IniConf.WriteString('GameConf', 'SqliteDBName', g_sSqliteDBName);
  end;

  SL := TStringList.Create;
  try
    g_IniConf.ReadSection('ClearServer', SL);

    for I := 0 to SL.Count - 1 do
    begin
      S := g_IniConf.ReadString('ClearServer', SL[I], '');
      g_IniConf.WriteString('ClearServer', SL[I], StringReplace(S, g_sGameDirectory, AppPath, [rfIgnoreCase]));
    end;
  finally
    SL.Free;
  end;

  S := AppPath + 'BackList.txt';
  if FileExists(S) then
  begin
    SL := TStringList.Create;
    try
      IniFile := TIniFile.Create(S);
      try
        IniFile.ReadSections(SL);

        for I := 0 to SL.Count - 1 do
        begin
          S := IniFile.ReadString(SL[I], 'Source', '');
          if Length(S) > 0 then
          begin
            IniFile.WriteString(SL[I], 'Source', StringReplace(S, g_sGameDirectory, AppPath, [rfIgnoreCase]));
          end;

          S := IniFile.ReadString(SL[I], 'Save', '');
          if Length(S) > 0 then
          begin
            IniFile.WriteString(SL[I], 'Save', StringReplace(S, g_sGameDirectory, AppPath, [rfIgnoreCase]));
          end;
        end;
      finally
        IniFile.Free;
      end;
    finally
      SL.Free;
    end;
  end;

  sTempPath := AppPath + g_sDBServer_Directory;
  S := sTempPath + g_sDBServer_ConfigFile;
  if FileExists(S) then
  begin
    IniFile := TIniFile.Create(S);
    try
      IniFile.WriteString('Setup', 'MapFile', AppPath + g_sDBServer_Config_MapFile);

      if Length(g_sSqliteDBName) > 0 then
      begin
        IniFile.WriteString('Setup', 'SqliteDBName', g_sSqliteDBName);
      end;

      IniFile.WriteString('DB', 'Dir', sTempPath + g_sDBServer_Config_Dir);
      IniFile.WriteString('DB', 'IdDir', sTempPath + g_sDBServer_Config_IdDir);
      IniFile.WriteString('DB', 'HumDir', sTempPath + g_sDBServer_Config_HumDir);
      IniFile.WriteString('DB', 'FeeDir', sTempPath + g_sDBServer_Config_FeeDir);
      IniFile.WriteString('DB', 'BackupDir', sTempPath + g_sDBServer_Config_BackupDir);
      IniFile.WriteString('DB', 'ConnectDir', sTempPath + g_sDBServer_Config_ConnectDir);
      IniFile.WriteString('DB', 'LogDir', sTempPath + g_sDBServer_Config_LogDir);
    finally
      IniFile.Free;
    end;
  end;

  sTempPath := AppPath + g_sLoginServer_Directory;
  S := sTempPath + g_sLoginServer_ConfigFile;
  if FileExists(S) then
  begin
    IniFile := TIniFile.Create(S);
    try
      IniFile.WriteString('DB', 'IdDir', sTempPath + g_sLoginServer_IdDir);
      IniFile.WriteString('DB', 'FeedIDList', sTempPath + g_sLoginServer_FeedIDList);
      IniFile.WriteString('DB', 'FeedIPList', sTempPath + g_sLoginServer_FeedIPList);
      IniFile.WriteString('DB', 'CountLogDir', sTempPath + g_sLoginServer_CountLogDir);
      IniFile.WriteString('DB', 'WebLogDir', sTempPath + g_sLoginServer_WebLogDir);

      IniFile.WriteString('DB', 'ChrLogDir', sTempPath + g_sLoginServer_ChrLogDir);
      IniFile.WriteString('DB', 'IDLogDir', sTempPath + g_sLoginServer_IDLogDir);
    finally
      IniFile.Free;
    end;
  end;

  sTempPath := AppPath + g_sM2Server_Directory;
  S := sTempPath + g_sM2Server_ConfigFile;
  if FileExists(S) then
  begin
    IniFile := TIniFile.Create(S);
    try
      IniFile.WriteString('Server', 'SqliteDBName', g_sSqliteDBName);

      IniFile.WriteString('Share', 'BaseDir', sTempPath + g_sM2Server_BaseDir);
      IniFile.WriteString('Share', 'GuildDir', sTempPath + g_sM2Server_GuildDir);
      IniFile.WriteString('Share', 'GuildFile', sTempPath + g_sM2Server_GuildFile);
      IniFile.WriteString('Share', 'VentureDir', sTempPath + g_sM2Server_VentureDir);
      IniFile.WriteString('Share', 'ConLogDir', sTempPath + g_sM2Server_ConLogDir);
      IniFile.WriteString('Share', 'LogDir', sTempPath + g_sM2Server_LogDir);
      IniFile.WriteString('Share', 'PlugDir', sTempPath);
      IniFile.WriteString('Share', 'BoxsDir', sTempPath + g_sM2Server_BoxsDir);

      IniFile.WriteString('Share', 'CastleDir', sTempPath + g_sM2Server_CastleDir);
      IniFile.WriteString('Share', 'EnvirDir', sTempPath + g_sM2Server_EnvirDir);
      IniFile.WriteString('Share', 'MapDir', sTempPath + g_sM2Server_MapDir);
      IniFile.WriteString('Share', 'NoticeDir', sTempPath + g_sM2Server_NoticeDir);
      IniFile.WriteString('Share', 'CastleFile', sTempPath + g_sM2Server_CastleFile);
    finally
      IniFile.Free;
    end;
  end;

  sTempPath := AppPath + g_sLogServer_Directory;
  S := sTempPath + g_sLogServer_ConfigFile;
  if FileExists(S) then
  begin
    IniFile := TIniFile.Create(S);
    try
      IniFile.WriteString('Setup', 'BaseDir', sTempPath + g_sLogServer_BaseDir);
    finally
      IniFile.Free;
    end;
  end;


  S := '已成功将服务器目录设置为 ' + AppPath +  sLineBreak + sLineBreak + '请重新启动引擎控制台生效';
  Application.MessageBox(PChar(S), '成功', MB_OK + MB_ICONINFORMATION);
end;

procedure TfrmMain.rbDataSaveSqliteClick(Sender: TObject);
begin
  grpDataSaveMySql.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDBServer.Enabled := rbDataSaveMySql.Checked;
  seDataSaveDBPort.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDBUser.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDBPassword.Enabled := rbDataSaveMySql.Checked;
  edtDataSaveDataBase.Enabled := rbDataSaveMySql.Checked;

  lblMySqlLinkTest.Enabled := rbDataSaveMySql.Checked;
  lblMySqlDoInit.Enabled := rbDataSaveMySql.Checked;
end;

procedure TfrmMain.lblMySqlLinkTestClick(Sender: TObject);
var
  MySqlLib: TMySQLLib;
  DB: TMySQLDataBase;
  DefaultCursor: TCursor;
begin
  DefaultCursor := Screen.Cursor;
  Screen.Cursor := crSQLWait;
  MySqlLib := TMySQLLib.Create(nil);
  try
    MySqlLib.Load('', 'libmysql-32.dll');

    DB := TMySQLDataBase.Create(MySqlLib);
    try
      DB.Init;
      DB.Connect(edtDataSaveDBServer.Text, edtDataSaveDBUser.Text, edtDataSaveDBPassword.Text, edtDataSaveDataBase.Text, seDataSaveDBPort.Value, 0);

      ShowMessage('数据库连接成功');
    finally
      DB.Free;
    end;
  finally
    MySqlLib.Free;

    Screen.Cursor := DefaultCursor;
  end;
end;

procedure TfrmMain.lblMySqlDoInitClick(Sender: TObject);
var
  MySqlLib: TMySQLLib;
  DB: TMySQLDataBase;
  DefaultCursor: TCursor;
begin
  if Application.MessageBox('初始化后，数据内容将会全部清空!!!' + sLineBreak + sLineBreak + '你确定要初始化数据库吗？', '数据库初始化', MB_YESNO + MB_ICONQUESTION) <> mrYes then Exit;

  DefaultCursor := Screen.Cursor;
  Screen.Cursor := crSQLWait;

  MySqlLib := TMySQLLib.Create(nil);
  try
    MySqlLib.Load('', 'libmysql-32.dll');

    DB := TMySQLDataBase.Create(MySqlLib);
    try
      DB.Init;
      DB.Connect(edtDataSaveDBServer.Text, edtDataSaveDBUser.Text, edtDataSaveDBPassword.Text, edtDataSaveDataBase.Text, seDataSaveDBPort.Value, CLIENT_MULTI_STATEMENTS);

      DB.StartTransaction;
      try
        DB.Exec(MYSQL_CREATE_ACCOUNT_TABLES);
        DB.ClearResult;

//        DB.Exec(MYSQL_CREATE_HERO_ROLEDATA_TABLES);
//        DB.ClearResult;

        DB.Exec(MYSQL_CREATE_ROLEDATA_TABLES);
        DB.ClearResult;

//        DB.Exec(MYSQL_CREATE_HUMAN_ROLEDATA_TABLES);
//        DB.ClearResult;

//        DB.Exec(MYSQL_CREATE_HUMAN_1_ROLEDATA_TABLES);
//        DB.ClearResult;

//        DB.Exec(MYSQL_TEST);
//        DB.ClearResult;
//        DB.Exec(MYSQL_CREATE_HUMAN_2_ROLEDATA_TABLES);
//        DB.ClearResult;
                      

        DB.Exec(MYSQL_CREATE_M2DATA_TABLES);
        DB.ClearResult;

        DB.Commit;
      except
        on E: Exception do
        begin
          DB.Rollback;
          raise Exception.Create(E.Message);
        end;
      end;

      ShowMessage('数据库已成功初始化');
    finally
      DB.Free;
    end;
  finally
    MySqlLib.Free;
    Screen.Cursor := DefaultCursor;
  end;
end;

procedure TfrmMain.chkSelGate_GetMultiThreadClick(Sender: TObject);
begin
  g_boSelGate_GetMultiThread := chkSelGate_GetMultiThread.Checked;
end;

procedure TfrmMain.Button1Click(Sender: TObject);
begin
  ButtonNext1Click(nil);
  ButtonNext2Click(nil);
  ButtonNext3Click(nil);
  ButtonNext4Click(nil);
  ButtonNext5Click(nil);
  ButtonNext6Click(nil);
  ButtonNext7Click(nil);
  ButtonNext8Click(nil);
  ButtonSaveClick(nil);
end;

end.

