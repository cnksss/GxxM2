unit uFrmDummySetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ExtCtrls, StdCtrls, SpinEditEx, ComCtrls,
  Vcl.Samples.Spin;

type
  TFrmDummySetting = class(TForm)
    pgcMain: TPageControl;
    ts1: TTabSheet;
    GroupBox1: TGroupBox;
    lstDummyList: TListBox;
    GroupBox2: TGroupBox;
    Label1: TLabel;
    edtDummyName: TEdit;
    btnDummyAdd: TButton;
    btnDummyDel: TButton;
    GroupBox3: TGroupBox;
    Label2: TLabel;
    Label4: TLabel;
    seDummyHomeX: TSpinEditEx;
    seDummyHomeY: TSpinEditEx;
    edtDummyHomeMap: TEdit;
    btnDummyLogon: TButton;
    ts2: TTabSheet;
    GroupBox5: TGroupBox;
    CheckBoxDummyAutoRepairItem: TCheckBox;
    GroupBox6: TGroupBox;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    EditDummyWarrorAttackTime: TSpinEditEx;
    EditDummyTaoistAttackTime: TSpinEditEx;
    EditDummyWizardAttackTime: TSpinEditEx;
    GroupBox7: TGroupBox;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    EditDummyWarrorWalkTime: TSpinEditEx;
    EditDummyWizardWalkTime: TSpinEditEx;
    EditDummyTaoistWalkTime: TSpinEditEx;
    GroupBox8: TGroupBox;
    CheckBoxDummyAutoRecallHero: TCheckBox;
    GroupBox9: TGroupBox;
    Label13: TLabel;
    Label14: TLabel;
    Label15: TLabel;
    Bevel1: TBevel;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Bevel5: TBevel;
    Label19: TLabel;
    Label20: TLabel;
    Bevel3: TBevel;
    seDummyHPTime_Warrior: TSpinEditEx;
    seDummyMPTime_Warrior: TSpinEditEx;
    seDummyHPTime_DF: TSpinEditEx;
    seDummyHPBase_DF: TSpinEditEx;
    seDummyHPBase_Warrior: TSpinEditEx;
    seDummyMPBase_Warrior: TSpinEditEx;
    seDummyMPTime_DF: TSpinEditEx;
    seDummyMPBase_DF: TSpinEditEx;
    GroupBox10: TGroupBox;
    Label21: TLabel;
    Label22: TLabel;
    Label23: TLabel;
    Label24: TLabel;
    Bevel2: TBevel;
    Label25: TLabel;
    Label26: TLabel;
    Bevel4: TBevel;
    Label27: TLabel;
    Bevel6: TBevel;
    Label28: TLabel;
    seDummyHeroHPTime_Warrior: TSpinEditEx;
    seDummyHeroMPTime_Warrior: TSpinEditEx;
    seDummyHeroHPTime_DF: TSpinEditEx;
    seDummyHeroMPTime_DF: TSpinEditEx;
    seDummyHeroHPBase_DF: TSpinEditEx;
    seDummyHeroMPBase_DF: TSpinEditEx;
    seDummyHeroHPBase_Warrior: TSpinEditEx;
    seDummyHeroMPBase_Warrior: TSpinEditEx;
    grp14: TGroupBox;
    Label29: TLabel;
    Label30: TLabel;
    chkDummyAutoAddHP: TCheckBox;
    seDummyAddHPPercent: TSpinEditEx;
    chkDummyAutoAddMP: TCheckBox;
    seDummyAddMPPercent: TSpinEditEx;
    ts3: TTabSheet;
    GroupBox11: TGroupBox;
    chkDisDummyRun: TCheckBox;
    chkDummyRunHum: TCheckBox;
    chkDummyRunMon: TCheckBox;
    chkDummyRunNpc: TCheckBox;
    chkDummyRunGuard: TCheckBox;
    chkDummySafeArea: TCheckBox;
    chkDummySafeAreaDisNpcRun: TCheckBox;
    chkSafeAreaDisShopStallDummyRun: TCheckBox;
    chkSafeAreaDisOffLineDummyRun: TCheckBox;
    chkDummyWarDisHumRun: TCheckBox;
    chkDummyWarHreoRun: TCheckBox;
    ts4: TTabSheet;
    GroupBox12: TGroupBox;
    lstDisableMoveMap: TListBox;
    btnDisableMoveMapAdd: TButton;
    btnDisableMoveMapDelete: TButton;
    btnDisableMoveMapAddAll: TButton;
    btnDisableMoveMapDeleteAll: TButton;
    btnDisableMoveMapSave: TButton;
    GroupBox13: TGroupBox;
    lstMapList: TListBox;
    ts5: TTabSheet;
    GroupBox14: TGroupBox;
    lstNoAttackMonList: TListBox;
    btnNoAttackMonDel: TButton;
    btnNoAttackMonAddAll: TButton;
    btnNoAttackMonDelAll: TButton;
    btnNoAttackMonSave: TButton;
    GroupBox15: TGroupBox;
    lstMonList: TListBox;
    btnNoAttackMonAdd: TButton;
    ButtonDummySave: TButton;
    btn1: TButton;
    Label3: TLabel;
    Label31: TLabel;
    seDummyLogonTime: TSpinEditEx;
    Label6: TLabel;
    chkDummyLogonRand: TCheckBox;
    procedure FormCreate(Sender: TObject);
    procedure btnDummyLogonClick(Sender: TObject);
    procedure btnDummyAddClick(Sender: TObject);
    procedure btnDummyDelClick(Sender: TObject);
    procedure ButtonDummySaveClick(Sender: TObject);
    procedure edtDummyHomeMapChange(Sender: TObject);
    procedure seDummyHomeXChange(Sender: TObject);
    procedure seDummyHomeYChange(Sender: TObject);
    procedure seDummyLogonTimeChange(Sender: TObject);
    procedure btn1Click(Sender: TObject);
    procedure chkDisDummyRunClick(Sender: TObject);
    procedure CheckBoxDummyAutoRepairItemClick(Sender: TObject);
    procedure EditDummyWarrorAttackTimeChange(Sender: TObject);
    procedure chkDummyAutoAddHPClick(Sender: TObject);
    procedure seDummyAddHPPercentChange(Sender: TObject);
    procedure chkDummyAutoAddMPClick(Sender: TObject);
    procedure seDummyAddMPPercentChange(Sender: TObject);
    procedure seDummyHPTime_WarriorChange(Sender: TObject);
    procedure seDummyHPBase_WarriorChange(Sender: TObject);
    procedure seDummyMPTime_WarriorChange(Sender: TObject);
    procedure seDummyMPBase_WarriorChange(Sender: TObject);
    procedure seDummyHPTime_DFChange(Sender: TObject);
    procedure seDummyHPBase_DFChange(Sender: TObject);
    procedure seDummyMPTime_DFChange(Sender: TObject);
    procedure seDummyMPBase_DFChange(Sender: TObject);
    procedure seDummyHeroHPTime_WarriorChange(Sender: TObject);
    procedure seDummyHeroHPBase_WarriorChange(Sender: TObject);
    procedure seDummyHeroMPTime_WarriorChange(Sender: TObject);
    procedure seDummyHeroMPBase_WarriorChange(Sender: TObject);
    procedure seDummyHeroHPTime_DFChange(Sender: TObject);
    procedure seDummyHeroHPBase_DFChange(Sender: TObject);
    procedure seDummyHeroMPTime_DFChange(Sender: TObject);
    procedure seDummyHeroMPBase_DFChange(Sender: TObject);
    procedure CheckBoxDummyAutoRecallHeroClick(Sender: TObject);
    procedure EditDummyWarrorWalkTimeChange(Sender: TObject);
    procedure EditDummyWizardWalkTimeChange(Sender: TObject);
    procedure EditDummyTaoistWalkTimeChange(Sender: TObject);
    procedure lstDummyListClick(Sender: TObject);
    procedure chkDummyRunHumClick(Sender: TObject);
    procedure chkDummyRunMonClick(Sender: TObject);
    procedure chkDummyRunNpcClick(Sender: TObject);
    procedure chkDummyRunGuardClick(Sender: TObject);
    procedure chkDummySafeAreaClick(Sender: TObject);
    procedure chkDummyWarDisHumRunClick(Sender: TObject);
    procedure chkDummyWarHreoRunClick(Sender: TObject);
    procedure chkDummySafeAreaDisNpcRunClick(Sender: TObject);
    procedure chkSafeAreaDisShopStallDummyRunClick(Sender: TObject);
    procedure chkSafeAreaDisOffLineDummyRunClick(Sender: TObject);
    procedure EditDummyWizardAttackTimeChange(Sender: TObject);
    procedure EditDummyTaoistAttackTimeChange(Sender: TObject);
    procedure chkDummyLogonRandClick(Sender: TObject);
    procedure lstDisableMoveMapClick(Sender: TObject);
    procedure btnDisableMoveMapAddClick(Sender: TObject);
    procedure btnDisableMoveMapDeleteClick(Sender: TObject);
    procedure btnDisableMoveMapAddAllClick(Sender: TObject);
    procedure btnDisableMoveMapDeleteAllClick(Sender: TObject);
    procedure btnDisableMoveMapSaveClick(Sender: TObject);
    procedure lstNoAttackMonListClick(Sender: TObject);
    procedure btnNoAttackMonAddClick(Sender: TObject);
    procedure btnNoAttackMonDelClick(Sender: TObject);
    procedure btnNoAttackMonAddAllClick(Sender: TObject);
    procedure btnNoAttackMonDelAllClick(Sender: TObject);
    procedure btnNoAttackMonSaveClick(Sender: TObject);
  private
    { Private declarations }

    FIsDummyDisableMoveMapChanged: Boolean;
    FIsDummyNoActiveAttackMonChanged: Boolean;

    procedure ModValue;
    procedure uModValue;
  public
    { Public declarations }
  end;

procedure ShowFrmDummySetting;

implementation

uses
  M2Share, Grobal2, Envir, M2Definition, M2Threads;

{$R *.dfm}

procedure ShowFrmDummySetting;
var
  FrmDummySetting: TFrmDummySetting;
begin
  FrmDummySetting := TFrmDummySetting.Create(nil);
  try
    FrmDummySetting.ShowModal;
  finally
    FrmDummySetting.Free;
  end;
end;

procedure TFrmDummySetting.FormCreate(Sender: TObject);
var
  I: Integer;
  Envir: TEnvirnoment;
  MonInfo: pTMonInfo;
begin
  lstDummyList.Clear;
  lstDummyList.Items.AddStrings(g_DummyNameList);

  edtDummyHomeMap.Text := g_Config.sDummyHomeMap;
  seDummyHomeX.Value := g_Config.nDummyHomeX;
  seDummyHomeY.Value := g_Config.nDummyHomeY;
  seDummyLogonTime.Value := g_Config.nDummyLogonTime;
  chkDummyLogonRand.Checked := g_Config.boDummyLogonRand;

  CheckBoxDummyAutoRepairItem.Checked := g_Config.boDummyAutoRepairItem;
  CheckBoxDummyAutoRecallHero.Checked := g_Config.boDummyAutoRecallHero;

  chkDummyAutoAddHP.Checked := g_Config.boDummyAutoAddHP;
  seDummyAddHPPercent.Value := g_Config.nDummyAddHPPercent;

  chkDummyAutoAddMP.Checked := g_Config.boDummyAutoAddMP;
  seDummyAddMPPercent.Value := g_Config.nDummyAddMPPercent;

  seDummyHPTime_Warrior.Value := g_Config.nDummyHPTime_Warrior;
  seDummyHPBase_Warrior.Value := g_Config.nDummyHPBase_Warrior;

  seDummyMPTime_Warrior.Value := g_Config.nDummyMPTime_Warrior;
  seDummyMPBase_Warrior.Value := g_Config.nDummyMPBase_Warrior;

  seDummyHPTime_DF.Value := g_Config.nDummyHPTime_DF;
  seDummyHPBase_DF.Value := g_Config.nDummyHPBase_DF;

  seDummyMPTime_DF.Value := g_Config.nDummyMPTime_DF;
  seDummyMPBase_DF.Value := g_Config.nDummyMPBase_DF;

  seDummyHeroHPTime_Warrior.Value := g_Config.nDummyHeroHPTime_Warrior;
  seDummyHeroHPBase_Warrior.Value := g_Config.nDummyHeroHPBase_Warrior;

  seDummyHeroMPTime_Warrior.Value := g_Config.nDummyHeroMPTime_Warrior;
  seDummyHeroMPBase_Warrior.Value := g_Config.nDummyHeroMPBase_Warrior;

  seDummyHeroHPTime_DF.Value := g_Config.nDummyHeroHPTime_DF;
  seDummyHeroHPBase_DF.Value := g_Config.nDummyHeroHPBase_DF;

  seDummyHeroMPTime_DF.Value := g_Config.nDummyHeroMPTime_DF;
  seDummyHeroMPBase_DF.Value := g_Config.nDummyHeroMPBase_DF;

  EditDummyWarrorAttackTime.Value := g_Config.dwDummyWarrorAttackTime;
  EditDummyWizardAttackTime.Value := g_Config.dwDummyWizardAttackTime;
  EditDummyTaoistAttackTime.Value := g_Config.dwDummyTaoistAttackTime;

  EditDummyWarrorWalkTime.Value := g_Config.dwDummyWarrorWalkTime;
  EditDummyWizardWalkTime.Value := g_Config.dwDummyWizardWalkTime;
  EditDummyTaoistWalkTime.Value := g_Config.dwDummyTaoistWalkTime;

  chkDisDummyRun.Checked := not g_Config.boDiableDummyRun;
  chkDummyRunHum.Checked := g_Config.boDummyRunHum;
  chkDummyRunMon.Checked := g_Config.boDummyRunMon;
  chkDummyRunNpc.Checked := g_Config.boDummyRunNpc;
  chkDummyRunGuard.Checked := g_Config.boDummyRunGuard;
  chkDummySafeArea.Checked := g_Config.boDummySafeAreaLimited;
  chkDummyWarDisHumRun.Checked := g_Config.boDummyWarDisHumRun;
  chkDummyWarHreoRun.Checked := g_Config.boDummyWarHreoRun;
  chkDummyWarHreoRun.Enabled := g_Config.boDummyWarDisHumRun;
  chkDummySafeAreaDisNpcRun.Checked := g_Config.boDummySafeAreaDisNpcRun;
  chkSafeAreaDisShopStallDummyRun.Checked := g_Config.boSafeAreaDisShopStallDummyRun;
  chkSafeAreaDisOffLineDummyRun.Checked := g_Config.boSafeAreaDisOffLineDummyRun;

  pgcMain.ActivePageIndex := 0;
  btnDummyLogon.Enabled := False;

  for I := 0 to g_MapManager.Count - 1 do
  begin
    Envir := TEnvirnoment(g_MapManager.Items[I]);
    lstMapList.Items.Add(Envir.sMapName);
  end;

  if g_MultiThreadRun then
    UserEngine.MonsterList.LockR(6);
  try
    for I := 0 to UserEngine.MonsterList.Count - 1 do
    begin
      MonInfo := UserEngine.MonsterList.Items[I];
      lstMonList.Items.AddObject(MonInfo.sName, TObject(MonInfo));
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.MonsterList.UnLockR;
  end;

  lstDisableMoveMap.Items.Assign(g_DummyDisableMoveMapList);
  lstNoAttackMonList.Items.Assign(g_DummyNoActiveAttackMonList);

  FIsDummyDisableMoveMapChanged := False;
  FIsDummyNoActiveAttackMonChanged := False;
end;

procedure TFrmDummySetting.lstDummyListClick(Sender: TObject);
var
  ItemIndex: Integer;
begin
  ItemIndex := lstDummyList.ItemIndex;
  if (ItemIndex >= 0) and (ItemIndex < lstDummyList.Items.Count) then
  begin
    btnDummyLogon.Enabled := True;
    btnDummyDel.Enabled := True;
    edtDummyName.Text := lstDummyList.Items.Strings[ItemIndex]
  end
  else
  begin
    btnDummyLogon.Enabled := False;
    btnDummyDel.Enabled := False;
  end;
end;

procedure TFrmDummySetting.btnDummyLogonClick(Sender: TObject);
var
  Index: Integer;
  DummyLogon: TDummyLogon;
begin
  if g_MapManager.FindMap(g_Config.sDummyHomeMap) = nil then
  begin
    Application.MessageBox('出生地图设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    edtDummyHomeMap.SetFocus;
    Exit;
  end;
  DummyLogon.sMapName := g_Config.sDummyHomeMap;
  DummyLogon.nX := g_Config.nDummyHomeX;
  DummyLogon.nY := g_Config.nDummyHomeY;

  for Index := 0 to lstDummyList.Count - 1 do
  begin
    if lstDummyList.Selected[Index] then
    begin
      // MainOutMessage('ListBoxAIList.Selected[Index]1:' + ListBoxAIList.Items.Strings[Index]);
      if (UserEngine.GetPlayObject(lstDummyList.Items.Strings[Index]) = nil) and (not UserEngine.FindDummyLogon(lstDummyList.Items.Strings
        [Index])) then
      begin
        DummyLogon.sCharName := lstDummyList.Items.Strings[Index];
        UserEngine.AddDummyLogon(@DummyLogon);
      end;
    end;
  end;
  btnDummyLogon.Enabled := False;
end;

procedure TFrmDummySetting.btnDummyAddClick(Sender: TObject);
var
  sName: string;
begin
  sName := Trim(edtDummyName.Text);
  if (sName <> '') and (not GetDummyNameList(sName)) then
  begin
    g_DummyNameList.Lock;
    try
      g_DummyNameList.Add(sName);
      lstDummyList.Clear;
      lstDummyList.Items.AddStrings(g_DummyNameList);
    finally
      g_DummyNameList.UnLock;
    end;
    ModValue();
  end;
end;

procedure TFrmDummySetting.btnDummyDelClick(Sender: TObject);
begin
  lstDummyList.DeleteSelected;
  g_DummyNameList.Lock;
  try
    g_DummyNameList.Clear;
    g_DummyNameList.AddStrings(lstDummyList.Items);
  finally
    g_DummyNameList.UnLock;
  end;
  ModValue();
end;

procedure TFrmDummySetting.edtDummyHomeMapChange(Sender: TObject);
begin
  //
end;

procedure TFrmDummySetting.seDummyHomeXChange(Sender: TObject);
begin
  g_Config.nDummyHomeX := seDummyHomeX.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHomeYChange(Sender: TObject);
begin
  g_Config.nDummyHomeY := seDummyHomeY.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyLogonTimeChange(Sender: TObject);
begin
  g_Config.nDummyLogonTime := seDummyLogonTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.ButtonDummySaveClick(Sender: TObject);
var
  I: Integer;
begin
  if edtDummyHomeMap.Text = '' then
  begin
    Application.MessageBox('出生地图设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    edtDummyHomeMap.SetFocus;
    Exit;
  end;
  g_Config.sDummyHomeMap := Trim(edtDummyHomeMap.Text);

  if g_MapManager.FindMap(g_Config.sDummyHomeMap) = nil then
  begin
    Application.MessageBox('出生地图设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    edtDummyHomeMap.SetFocus;
    Exit;
  end;

  // g_Config.sDummyConfigListFileName := Trim(EditDummyConfigListFileName.Text);
  // g_Config.sDummyHeroConfigListFileName := Trim(EditDummyHeroConfigListFileName.Text);
  Config.WriteBool('Setup', 'DummyLogonRand', g_Config.boDummyLogonRand);

  Config.WriteInteger('Setup', 'DummyLogonTime', g_Config.nDummyLogonTime);
  Config.WriteInteger('Setup', 'DummyHomeX', g_Config.nDummyHomeX);
  Config.WriteInteger('Setup', 'DummyHomeY', g_Config.nDummyHomeY);
  Config.WriteString('Setup', 'DummyHomeMap', g_Config.sDummyHomeMap);
  // Config.WriteString('Setup', 'DummyConfigListFileName', g_Config.sDummyConfigListFileName);
  // Config.WriteString('Setup', 'DummyHeroConfigListFileName', g_Config.sDummyHeroConfigListFileName);

  Config.WriteBool('Setup', 'DummyAutoRepairItem', g_Config.boDummyAutoRepairItem);
  Config.WriteBool('Setup', 'DummyAutoRecallHero', g_Config.boDummyAutoRecallHero);
  Config.WriteInteger('Setup', 'DummyWarrorAttackTime', g_Config.dwDummyWarrorAttackTime);
  Config.WriteInteger('Setup', 'DummyWizardAttackTime', g_Config.dwDummyWizardAttackTime);
  Config.WriteInteger('Setup', 'DummyTaoistAttackTime', g_Config.dwDummyTaoistAttackTime);
  Config.WriteInteger('Setup', 'DummyWarrorWalkTime', g_Config.dwDummyWarrorWalkTime);
  Config.WriteInteger('Setup', 'DummyWizardWalkTime', g_Config.dwDummyWizardWalkTime);
  Config.WriteInteger('Setup', 'DummyTaoistWalkTime', g_Config.dwDummyTaoistWalkTime);

  Config.WriteBool('Setup', 'DummyAutoAddHP', g_Config.boDummyAutoAddHP);
  Config.WriteInteger('Setup', 'DummyAddHPPercent', g_Config.nDummyAddHPPercent);

  Config.WriteBool('Setup', 'DummyAutoAddMP', g_Config.boDummyAutoAddMP);
  Config.WriteInteger('Setup', 'DummyAddMPPercent', g_Config.nDummyAddMPPercent);

  Config.WriteInteger('Setup', 'DummyHPTime_Warrior', g_Config.nDummyHPTime_Warrior);
  Config.WriteInteger('Setup', 'DummyHPBase_Warrior', g_Config.nDummyHPBase_Warrior);

  Config.WriteInteger('Setup', 'DummyMPTime_Warrior', g_Config.nDummyMPTime_Warrior);
  Config.WriteInteger('Setup', 'DummyMPBase_Warrior', g_Config.nDummyMPBase_Warrior);

  Config.WriteInteger('Setup', 'DummyHPTime_DF', g_Config.nDummyHPTime_DF);
  Config.WriteInteger('Setup', 'DummyHPBase_DF', g_Config.nDummyHPBase_DF);

  Config.WriteInteger('Setup', 'DummyMPTime_DF', g_Config.nDummyMPTime_DF);
  Config.WriteInteger('Setup', 'DummyMPBase_DF', g_Config.nDummyMPBase_DF);

  Config.WriteInteger('Setup', 'DummyHeroHPTime_Warrior', g_Config.nDummyHeroHPTime_Warrior);
  Config.WriteInteger('Setup', 'DummyHeroHPBase_Warrior', g_Config.nDummyHeroHPBase_Warrior);

  Config.WriteInteger('Setup', 'DummyHeroMPTime_Warrior', g_Config.nDummyHeroMPTime_Warrior);
  Config.WriteInteger('Setup', 'DummyHeroMPBase_Warrior', g_Config.nDummyHeroMPBase_Warrior);

  Config.WriteInteger('Setup', 'DummyHeroHPTime_DF', g_Config.nDummyHeroHPTime_DF);
  Config.WriteInteger('Setup', 'DummyHeroHPBase_DF', g_Config.nDummyHeroHPBase_DF);

  Config.WriteInteger('Setup', 'DummyHeroMPTime_DF', g_Config.nDummyHeroMPTime_DF);
  Config.WriteInteger('Setup', 'DummyHeroMPBase_DF', g_Config.nDummyHeroMPBase_DF);

  Config.WriteBool('Setup', 'DiableDummyRun', g_Config.boDiableDummyRun);
  Config.WriteBool('Setup', 'DummyRunHum', g_Config.boDummyRunHum);
  Config.WriteBool('Setup', 'DummyRunMon', g_Config.boDummyRunMon);
  Config.WriteBool('Setup', 'DummyRunNpc', g_Config.boDummyRunNpc);
  Config.WriteBool('Setup', 'DummyRunGuard', g_Config.boDummyRunGuard);
  Config.WriteBool('Setup', 'DummyWarDisHumRun', g_Config.boDummyWarDisHumRun);
  Config.WriteBool('Setup', 'DummyWarHreoRun', g_Config.boDummyWarHreoRun);
  Config.WriteBool('Setup', 'DummySafeAreaLimited', g_Config.boDummySafeAreaLimited);
  Config.WriteBool('Setup', 'DummySafeAreaDisNpcRun', g_Config.boDummySafeAreaDisNpcRun);
  Config.WriteBool('Setup', 'SafeAreaDisShopStallDummyRun', g_Config.boSafeAreaDisShopStallDummyRun);
  Config.WriteBool('Setup', 'SafeAreaDisOffLineDummyRun', g_Config.boSafeAreaDisOffLineDummyRun);

  SaveDummyNameList;

  if FIsDummyDisableMoveMapChanged then
  begin
    g_DummyDisableMoveMapList.Lock;
    try
      g_DummyDisableMoveMapList.Clear;
      for I := 0 to lstDisableMoveMap.Items.Count - 1 do
      begin
        g_DummyDisableMoveMapList.Add(lstDisableMoveMap.Items.Strings[I])
      end;

      g_DummyDisableMoveMapList.Sorted := True;
    finally
      g_DummyDisableMoveMapList.UnLock;
    end;

    SaveDummyDisableMoveMap();
  end;

  if FIsDummyNoActiveAttackMonChanged then
  begin
    g_DummyNoActiveAttackMonList.Lock;
    try
      g_DummyNoActiveAttackMonList.Clear;
      for I := 0 to lstNoAttackMonList.Items.Count - 1 do
      begin
        g_DummyNoActiveAttackMonList.Add(lstNoAttackMonList.Items.Strings[I]);
      end;

      g_DummyNoActiveAttackMonList.Sorted := True;
    finally
      g_DummyNoActiveAttackMonList.UnLock;
    end;
    SaveDummyNoActiveAttackMonList();
  end;

  uModValue();
end;

procedure TFrmDummySetting.btn1Click(Sender: TObject);
begin
  g_Config.nDummyHPTime_Warrior := 350;
    // 假人－战士职业回血速度 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHPTime_DF := 350;
    // 假人－道法职业回血速度 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroHPTime_Warrior := 350;
    // 假人英雄－战士职业回血速度 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroHPTime_DF := 350;
    // 假人英雄－道法职业回血速度 - 2013-07-16  (+ chongchong)

  g_Config.nDummyMPTime_Warrior := 800;
    // 假人－战士职业回蓝速度 - 2013-07-16  (+ chongchong)
  g_Config.nDummyMPTime_DF := 800;
    // 假人－道法职业回蓝速度 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroMPTime_Warrior := 800;
    // 假人英雄－战士职业回蓝速度 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroMPTime_DF := 800;
    // 假人英雄－道法职业回蓝速度 - 2013-07-16  (+ chongchong)

  g_Config.nDummyHPBase_Warrior := 75;
    // 假人－战士职业回血基数 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHPBase_DF := 75;
    // 假人－道法职业回血基数 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroHPBase_Warrior := 75;
    // 假人英雄－战士职业回血基数 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroHPBase_DF := 75;
    // 假人英雄－道法职业回血基数 - 2013-07-16  (+ chongchong)

  g_Config.nDummyMPBase_Warrior := 18;
    // 假人－战士职业回蓝基数 - 2013-07-16  (+ chongchong)
  g_Config.nDummyMPBase_DF := 18;
    // 假人－道法职业回蓝基数 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroMPBase_Warrior := 18;
    // 假人英雄－战士职业回蓝基数 - 2013-07-16  (+ chongchong)
  g_Config.nDummyHeroMPBase_DF := 18;
    // 假人英雄－道法职业回蓝基数 - 2013-07-16  (+ chongchong)

  seDummyHPTime_Warrior.Value := g_Config.nDummyHPTime_Warrior;
  seDummyHPBase_Warrior.Value := g_Config.nDummyHPBase_Warrior;

  seDummyMPTime_Warrior.Value := g_Config.nDummyMPTime_Warrior;
  seDummyMPBase_Warrior.Value := g_Config.nDummyMPBase_Warrior;

  seDummyHPTime_DF.Value := g_Config.nDummyHPTime_DF;
  seDummyHPBase_DF.Value := g_Config.nDummyHPBase_DF;

  seDummyMPTime_DF.Value := g_Config.nDummyMPTime_DF;
  seDummyMPBase_DF.Value := g_Config.nDummyMPBase_DF;

  seDummyHeroHPTime_Warrior.Value := g_Config.nDummyHeroHPTime_Warrior;
  seDummyHeroHPBase_Warrior.Value := g_Config.nDummyHeroHPBase_Warrior;

  seDummyHeroMPTime_Warrior.Value := g_Config.nDummyHeroMPTime_Warrior;
  seDummyHeroMPBase_Warrior.Value := g_Config.nDummyHeroMPBase_Warrior;

  seDummyHeroHPTime_DF.Value := g_Config.nDummyHeroHPTime_DF;
  seDummyHeroHPBase_DF.Value := g_Config.nDummyHeroHPBase_DF;

  seDummyHeroMPTime_DF.Value := g_Config.nDummyHeroMPTime_DF;
  seDummyHeroMPBase_DF.Value := g_Config.nDummyHeroMPBase_DF;
  ModValue();
end;

procedure TFrmDummySetting.chkDisDummyRunClick(Sender: TObject);
var
  boChecked: Boolean;
begin
  boChecked := not chkDisDummyRun.Checked;
  if boChecked then
  begin
    chkDummyRunHum.Checked := False;
    chkDummyRunHum.Enabled := False;

    chkDummyRunMon.Checked := False;
    chkDummyRunMon.Enabled := False;

    chkDummyRunNpc.Checked := False;
    chkDummyRunNpc.Enabled := False;

    chkDummyRunGuard.Checked := False;
    chkDummyRunGuard.Enabled := False;

    chkDummySafeArea.Checked := False;
    chkDummySafeArea.Enabled := False;

    chkDummySafeAreaDisNpcRun.Checked := False;
    chkDummySafeAreaDisNpcRun.Enabled := False;

    chkSafeAreaDisShopStallDummyRun.Checked := False;
    chkSafeAreaDisShopStallDummyRun.Enabled := False;

    chkSafeAreaDisOffLineDummyRun.Enabled := False;
    chkSafeAreaDisOffLineDummyRun.Checked := False;
  end
  else
  begin
    chkDummyRunHum.Enabled := True;
    chkDummyRunMon.Enabled := True;
    chkDummyRunNpc.Enabled := True;
    chkDummyRunGuard.Enabled := True;

    chkDummySafeArea.Enabled := True;
    chkSafeAreaDisShopStallDummyRun.Enabled := True;
    chkSafeAreaDisOffLineDummyRun.Enabled := True;
    chkDummySafeAreaDisNpcRun.Enabled := True;
  end;

  g_Config.boDiableDummyRun := boChecked;

  ModValue();
end;

procedure TFrmDummySetting.EditDummyWarrorAttackTimeChange(Sender: TObject);
begin
  g_Config.dwDummyWarrorAttackTime := EditDummyWarrorAttackTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyAutoAddHPClick(Sender: TObject);
begin
  g_Config.boDummyAutoAddHP := chkDummyAutoAddHP.Checked;
  ModValue();
end;

procedure TFrmDummySetting.seDummyAddHPPercentChange(Sender: TObject);
begin
  g_Config.nDummyAddHPPercent := seDummyAddHPPercent.Value;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyAutoAddMPClick(Sender: TObject);
begin
  g_Config.boDummyAutoAddMP := chkDummyAutoAddMP.Checked;
  ModValue();
end;

procedure TFrmDummySetting.seDummyAddMPPercentChange(Sender: TObject);
begin
  g_Config.nDummyAddMPPercent := seDummyAddMPPercent.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHPTime_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyHPTime_Warrior := seDummyHPTime_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHPBase_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyHPBase_Warrior := seDummyHPBase_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyMPTime_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyMPTime_Warrior := seDummyMPTime_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyMPBase_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyMPBase_Warrior := seDummyMPBase_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHPTime_DFChange(Sender: TObject);
begin
  g_Config.nDummyHPTime_DF := seDummyHPTime_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHPBase_DFChange(Sender: TObject);
begin
  g_Config.nDummyHPBase_DF := seDummyHPBase_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyMPTime_DFChange(Sender: TObject);
begin
  g_Config.nDummyMPTime_DF := seDummyMPTime_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyMPBase_DFChange(Sender: TObject);
begin
  g_Config.nDummyMPBase_DF := seDummyMPBase_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroHPTime_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyHeroHPTime_Warrior := seDummyHeroHPTime_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroHPBase_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyHeroHPBase_Warrior := seDummyHeroHPBase_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroMPTime_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyHeroMPTime_Warrior := seDummyHeroMPTime_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroMPBase_WarriorChange(Sender: TObject);
begin
  g_Config.nDummyHeroMPBase_Warrior := seDummyHeroMPBase_Warrior.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroHPTime_DFChange(Sender: TObject);
begin
  g_Config.nDummyHeroHPTime_DF := seDummyHeroHPTime_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroHPBase_DFChange(Sender: TObject);
begin
  g_Config.nDummyHeroHPBase_DF := seDummyHeroHPBase_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroMPTime_DFChange(Sender: TObject);
begin
  g_Config.nDummyHeroMPTime_DF := seDummyHeroMPTime_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.seDummyHeroMPBase_DFChange(Sender: TObject);
begin
  g_Config.nDummyHeroMPBase_DF := seDummyHeroMPBase_DF.Value;
  ModValue();
end;

procedure TFrmDummySetting.CheckBoxDummyAutoRepairItemClick(Sender: TObject);
begin
  g_Config.boDummyAutoRepairItem := CheckBoxDummyAutoRepairItem.Checked;
  ModValue();
end;

procedure TFrmDummySetting.CheckBoxDummyAutoRecallHeroClick(Sender: TObject);
begin
  g_Config.boDummyAutoRecallHero := CheckBoxDummyAutoRecallHero.Checked;
  ModValue();
end;

procedure TFrmDummySetting.EditDummyWarrorWalkTimeChange(Sender: TObject);
begin
  g_Config.dwDummyWarrorWalkTime := EditDummyWarrorWalkTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.EditDummyWizardWalkTimeChange(Sender: TObject);
begin
  g_Config.dwDummyWizardWalkTime := EditDummyWizardWalkTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.EditDummyTaoistWalkTimeChange(Sender: TObject);
begin
  g_Config.dwDummyTaoistWalkTime := EditDummyTaoistWalkTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyRunHumClick(Sender: TObject);
begin
  g_Config.boDummyRunHum := chkDummyRunHum.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyRunMonClick(Sender: TObject);
begin
  g_Config.boDummyRunMon := chkDummyRunMon.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyRunNpcClick(Sender: TObject);
begin
  g_Config.boDummyRunNpc := chkDummyRunNpc.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyRunGuardClick(Sender: TObject);
begin
  g_Config.boDummyRunGuard := chkDummyRunGuard.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkDummySafeAreaClick(Sender: TObject);
begin
  g_Config.boDummySafeAreaLimited := chkDummySafeArea.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyWarDisHumRunClick(Sender: TObject);
begin
  g_Config.boDummyWarDisHumRun := chkDummyWarDisHumRun.Checked;

  chkDummyWarHreoRun.Enabled := chkDummyWarDisHumRun.Checked;
  g_Config.boDummyWarHreoRun := chkDummyWarHreoRun.Enabled and chkDummyWarHreoRun.Checked;

  ModValue();
end;

procedure TFrmDummySetting.chkDummyWarHreoRunClick(Sender: TObject);
begin
  g_Config.boDummyWarHreoRun := chkDummyWarHreoRun.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkDummySafeAreaDisNpcRunClick(Sender: TObject);
begin
  g_Config.boDummySafeAreaDisNpcRun := chkDummySafeAreaDisNpcRun.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkSafeAreaDisShopStallDummyRunClick(Sender: TObject);
begin
  g_Config.boSafeAreaDisShopStallDummyRun := chkSafeAreaDisShopStallDummyRun.Checked;
  ModValue();
end;

procedure TFrmDummySetting.chkSafeAreaDisOffLineDummyRunClick(Sender: TObject);
begin
  g_Config.boSafeAreaDisOffLineDummyRun := chkSafeAreaDisOffLineDummyRun.Checked;
  ModValue();
end;

procedure TFrmDummySetting.EditDummyWizardAttackTimeChange(Sender: TObject);
begin
  g_Config.dwDummyWizardAttackTime := EditDummyWizardAttackTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.EditDummyTaoistAttackTimeChange(Sender: TObject);
begin
  g_Config.dwDummyTaoistAttackTime := EditDummyTaoistAttackTime.Value;
  ModValue();
end;

procedure TFrmDummySetting.chkDummyLogonRandClick(Sender: TObject);
begin
  g_Config.boDummyLogonRand := chkDummyLogonRand.Checked;
  ModValue();
end;

procedure TFrmDummySetting.ModValue;
begin
  ButtonDummySave.Enabled := True;
end;

procedure TFrmDummySetting.uModValue;
begin
  ButtonDummySave.Enabled := False;
end;

procedure TFrmDummySetting.lstDisableMoveMapClick(Sender: TObject);
begin
  if lstDisableMoveMap.ItemIndex >= 0 then
    btnDisableMoveMapDelete.Enabled := True;
end;

procedure TFrmDummySetting.btnDisableMoveMapAddClick(Sender: TObject);
var
  I: Integer;
  sMapName: string;
begin
  if lstMapList.Items.Count >= 0 then
  begin
    for I := 0 to lstMapList.Items.Count - 1 do
    begin
      if not lstMapList.Selected[I] then
        Continue;

      sMapName := lstMapList.Items[I];
      if lstDisableMoveMap.Items.IndexOf(sMapName) < 0 then
      begin
        lstDisableMoveMap.Items.Add(sMapName);
      end;
    end;
    FIsDummyDisableMoveMapChanged := True;
    ModValue();
  end;
end;

procedure TFrmDummySetting.btnDisableMoveMapDeleteClick(Sender: TObject);
begin
  if lstDisableMoveMap.ItemIndex >= 0 then
  begin
    lstDisableMoveMap.Items.Delete(lstDisableMoveMap.ItemIndex);
    FIsDummyDisableMoveMapChanged := True;
    ModValue();
  end;

  if lstDisableMoveMap.ItemIndex < 0 then
    btnDisableMoveMapDelete.Enabled := False;
end;

procedure TFrmDummySetting.btnDisableMoveMapAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  lstDisableMoveMap.Items.Clear;
  for I := 0 to lstMapList.Items.Count - 1 do
  begin
    lstDisableMoveMap.Items.Add(lstMapList.Items.Strings[I]);
  end;
  ModValue();
  FIsDummyDisableMoveMapChanged := True;
end;

procedure TFrmDummySetting.btnDisableMoveMapDeleteAllClick(Sender: TObject);
begin
  lstDisableMoveMap.Items.Clear;
  btnDisableMoveMapDelete.Enabled := False;
  FIsDummyDisableMoveMapChanged := True;
  ModValue();
end;

procedure TFrmDummySetting.btnDisableMoveMapSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_DummyDisableMoveMapList.Lock;
  try
    g_DummyDisableMoveMapList.Clear;
    for I := 0 to lstDisableMoveMap.Items.Count - 1 do
    begin
      g_DummyDisableMoveMapList.Add(lstDisableMoveMap.Items.Strings[I])
    end;

    g_DummyDisableMoveMapList.Sorted := True;
  finally
    g_DummyDisableMoveMapList.UnLock;
  end;
  SaveDummyDisableMoveMap();
  FIsDummyDisableMoveMapChanged := False;

end;

procedure TFrmDummySetting.lstNoAttackMonListClick(Sender: TObject);
begin
  if lstNoAttackMonList.ItemIndex >= 0 then
    btnNoAttackMonDel.Enabled := True;
end;

procedure TFrmDummySetting.btnNoAttackMonAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if lstMonList.Items.Count >= 0 then
  begin
    for I := 0 to lstMonList.Items.Count - 1 do
    begin
      if not lstMonList.Selected[I] then
        Continue;

      sItemName := lstMonList.Items[I];
      if lstNoAttackMonList.Items.IndexOf(sItemName) < 0 then
      begin
        lstNoAttackMonList.Items.Add(sItemName);
      end;
    end;

    ModValue();
    FIsDummyNoActiveAttackMonChanged := True;
  end;
end;

procedure TFrmDummySetting.btnNoAttackMonDelClick(Sender: TObject);
begin
  if lstNoAttackMonList.ItemIndex >= 0 then
  begin
    lstNoAttackMonList.Items.Delete(lstNoAttackMonList.ItemIndex);
    ModValue();
    FIsDummyNoActiveAttackMonChanged := True;
  end;
  if lstNoAttackMonList.ItemIndex < 0 then
    btnNoAttackMonDel.Enabled := False;
end;

procedure TFrmDummySetting.btnNoAttackMonAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  lstNoAttackMonList.Items.Clear;
  for I := 0 to lstMonList.Items.Count - 1 do
  begin
    lstNoAttackMonList.Items.Add(lstMonList.Items.Strings[I]);
  end;
  ModValue();
  FIsDummyNoActiveAttackMonChanged := True;
end;

procedure TFrmDummySetting.btnNoAttackMonDelAllClick(Sender: TObject);
begin
  lstNoAttackMonList.Items.Clear;
  btnNoAttackMonDel.Enabled := False;
  ModValue();
  FIsDummyNoActiveAttackMonChanged := True;
end;

procedure TFrmDummySetting.btnNoAttackMonSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_DummyNoActiveAttackMonList.Lock;
  try
    g_DummyNoActiveAttackMonList.Clear;
    for I := 0 to lstNoAttackMonList.Items.Count - 1 do
    begin
      g_DummyNoActiveAttackMonList.Add(lstNoAttackMonList.Items.Strings[I]);
    end;

    g_DummyNoActiveAttackMonList.Sorted := True;
  finally
    g_DummyNoActiveAttackMonList.UnLock;
  end;
  SaveDummyNoActiveAttackMonList();
  FIsDummyNoActiveAttackMonChanged := False;
end;

end.

