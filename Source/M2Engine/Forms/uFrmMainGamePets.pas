unit uFrmMainGamePets;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, SpinEditEx, ComCtrls, Grids,
  ExtCtrls, M2Share,
  {$IFDEF CPUX64}
  //VMProtectSDK,
  {$ENDIF}
  Grobal2, ColorIndexEdit, M2Threads, M2Definition, Vcl.Samples.Spin;

type
  TLevelExpScheme = (s_OldLevelExp, s_StdLevelExp, s_2Mult, s_5Mult, s_8Mult, s_10Mult, s_20Mult, s_30Mult, s_40Mult, s_50Mult,
    s_60Mult, s_70Mult, s_80Mult, s_90Mult, s_100Mult, s_150Mult, s_200Mult, s_250Mult, s_300Mult);

  TFrmGamePets = class(TForm)
    pgcMain: TPageControl;
    ts1: TTabSheet;
    grp1: TGroupBox;
    GroupBox1: TGroupBox;
    grp3: TGroupBox;
    Label20: TLabel;
    Label21: TLabel;
    Label22: TLabel;
    cbbPetShowFile1: TComboBox;
    sePetShowCount1: TSpinEditEx;
    sePetShowStart1: TSpinEditEx;
    lstMonsterList: TListBox;
    lstGamePets: TListBox;
    btnAddPet: TButton;
    btnDelPet: TButton;
    btnEditPet: TButton;
    btnSavePet: TButton;
    lbl2: TLabel;
    edtPetName: TEdit;
    TabSheet1: TTabSheet;
    GroupBox198: TGroupBox;
    Label647: TLabel;
    GridLevelExp: TStringGrid;
    cbbLevelExp: TComboBox;
    GroupBox199: TGroupBox;
    Label648: TLabel;
    Label649: TLabel;
    sePetHighLevel: TSpinEditEx;
    sePetHighLevelGetExp: TSpinEditEx;
    TabSheet2: TTabSheet;
    lbl1: TLabel;
    sePetCaptureRate: TSpinEditEx;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    cbbPetShowFile2: TComboBox;
    sePetShowCount2: TSpinEditEx;
    sePetShowStart2: TSpinEditEx;
    Label4: TLabel;
    sePetShowTime1: TSpinEditEx;
    Label5: TLabel;
    sePetShowTime2: TSpinEditEx;
    bvl1: TBevel;
    GroupBox2: TGroupBox;
    Label6: TLabel;
    sePetAddHP: TSpinEditEx;
    cbbPetAddHPType: TComboBox;
    Label7: TLabel;
    sePetAddDC1: TSpinEditEx;
    cbbPetAddDCType1: TComboBox;
    Label8: TLabel;
    sePetAddSC1: TSpinEditEx;
    cbbPetAddSCType1: TComboBox;
    sePetAddSC2: TSpinEditEx;
    cbbPetAddSCType2: TComboBox;
    sePetAddDC2: TSpinEditEx;
    cbbPetAddDCType2: TComboBox;
    Label9: TLabel;
    sePetAddAC1: TSpinEditEx;
    cbbPetAddACType1: TComboBox;
    sePetAddAC2: TSpinEditEx;
    cbbPetAddACType2: TComboBox;
    Label10: TLabel;
    sePetAddMAC1: TSpinEditEx;
    cbbPetAddMACType1: TComboBox;
    sePetAddMAC2: TSpinEditEx;
    cbbPetAddMACType2: TComboBox;
    lbl3: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    Label13: TLabel;
    Label14: TLabel;
    sePetAddMC1: TSpinEditEx;
    cbbPetAddMCType1: TComboBox;
    sePetAddMC2: TSpinEditEx;
    cbbPetAddMCType2: TComboBox;
    Label15: TLabel;
    lbl4: TLabel;
    Label16: TLabel;
    sePetShowOffsetX1: TSpinEditEx;
    Label17: TLabel;
    sePetShowOffsetY1: TSpinEditEx;
    Label18: TLabel;
    sePetShowOffsetX2: TSpinEditEx;
    Label19: TLabel;
    sePetShowOffsetY2: TSpinEditEx;
    Label23: TLabel;
    grp2: TGroupBox;
    chkPetHPToMaster: TCheckBox;
    chkPetDCToMaster: TCheckBox;
    chkPetMCToMaster: TCheckBox;
    chkPetSCToMaster: TCheckBox;
    chkPetACToMaster: TCheckBox;
    chkPetMACToMaster: TCheckBox;
    lbl5: TLabel;
    sePetAbilToMasterRate: TSpinEditEx;
    lbl6: TLabel;
    grp4: TGroupBox;
    chkOpenGamePet: TCheckBox;
    btnSaveExp: TButton;
    btnSavePetParams: TButton;
    grp5: TGroupBox;
    lbl7: TLabel;
    sePetUseItemIntervalTime: TSpinEditEx;
    GroupBox74: TGroupBox;
    Label24: TLabel;
    Label145: TLabel;
    chkPetFixExp: TCheckBox;
    sePetBaseExp: TSpinEditEx;
    sePetAddExp: TSpinEditEx;
    grp6: TGroupBox;
    chkLevelDifference: TCheckBox;
    seLevelDifference: TSpinEditEx;
    lbl8: TLabel;
    seHPScale: TSpinEditEx;
    lbl9: TLabel;
    GroupBox3: TGroupBox;
    chkPetShowMasterName: TCheckBox;
    Label164: TLabel;
    sePetNameColor: TColorIndexEdit;
    edtPetSuffixName: TEdit;
    Label166: TLabel;
    grp7: TGroupBox;
    chkCapturePetNeedItem: TCheckBox;
    chkCaptureOKDecDura: TCheckBox;
    grp8: TGroupBox;
    lbl10: TLabel;
    seGamePetMaxCount: TSpinEditEx;
    Label25: TLabel;
    seGamePetNameCount: TSpinEditEx;
    grp9: TGroupBox;
    lbl11: TLabel;
    seGamePetRecallTime: TSpinEditEx;
    lbl12: TLabel;
    grp10: TGroupBox;
    chkEnabledPetAttack: TCheckBox;
    chkDisableMonAttackPet: TCheckBox;
    chkDisableAllAttackPet: TCheckBox;
    chkPetNoEntity: TCheckBox;
    chkPetSleepControlBySlave: TCheckBox;
    chkPetNoShowHPProgress: TCheckBox;
    grp11: TGroupBox;
    chkEnabledPetPickup: TCheckBox;
    chkPetPickupFullToMaster: TCheckBox;
    chkPetPickupToMaster: TCheckBox;
    chkPetQuickPickup: TCheckBox;
    chkPetRangePickup: TCheckBox;
    sePetPickupRange: TSpinEditEx;
    chkEnablePetUseClientPickItems: TCheckBox;
    chkPetOnlyPickMonsterItem: TCheckBox;
    chkGamePetKillMonTrigger: TCheckBox;
    procedure lstGamePetsClick(Sender: TObject);
    procedure btnEditPetClick(Sender: TObject);
    procedure btnAddPetClick(Sender: TObject);
    procedure btnDelPetClick(Sender: TObject);
    procedure btnSavePetClick(Sender: TObject);
    procedure lstMonsterListDblClick(Sender: TObject);
    procedure lstMonsterListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure chkOpenGamePetClick(Sender: TObject);
    procedure chkEnabledPetAttackClick(Sender: TObject);
    procedure chkEnabledPetPickupClick(Sender: TObject);
    procedure chkPetPickupFullToMasterClick(Sender: TObject);
    procedure sePetAbilToMasterRateChange(Sender: TObject);
    procedure chkPetHPToMasterClick(Sender: TObject);
    procedure chkPetDCToMasterClick(Sender: TObject);
    procedure chkPetMCToMasterClick(Sender: TObject);
    procedure chkPetSCToMasterClick(Sender: TObject);
    procedure chkPetACToMasterClick(Sender: TObject);
    procedure chkPetMACToMasterClick(Sender: TObject);
    procedure btnSavePetParamsClick(Sender: TObject);
    procedure cbbLevelExpClick(Sender: TObject);
    procedure GridLevelExpSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
    procedure btnSaveExpClick(Sender: TObject);
    procedure sePetHighLevelChange(Sender: TObject);
    procedure sePetHighLevelGetExpChange(Sender: TObject);
    procedure chkPetFixExpClick(Sender: TObject);
    procedure sePetBaseExpChange(Sender: TObject);
    procedure sePetAddExpChange(Sender: TObject);
    procedure chkPetShowMasterNameClick(Sender: TObject);
    procedure sePetNameColorChange(Sender: TObject);
    procedure edtPetSuffixNameChange(Sender: TObject);
    procedure sePetUseItemIntervalTimeChange(Sender: TObject);
    procedure chkCapturePetNeedItemClick(Sender: TObject);
    procedure chkCaptureOKDecDuraClick(Sender: TObject);
    procedure seGamePetMaxCountChange(Sender: TObject);
    procedure seGamePetNameCountChange(Sender: TObject);
    procedure seGamePetRecallTimeChange(Sender: TObject);
    procedure chkPetPickupToMasterClick(Sender: TObject);
    procedure chkDisableMonAttackPetClick(Sender: TObject);
    procedure chkDisableAllAttackPetClick(Sender: TObject);
    procedure chkPetSleepControlBySlaveClick(Sender: TObject);
    procedure chkPetNoEntityClick(Sender: TObject);
    procedure chkPetNoShowHPProgressClick(Sender: TObject);
    procedure chkPetQuickPickupClick(Sender: TObject);
    procedure chkPetRangePickupClick(Sender: TObject);
    procedure sePetPickupRangeChange(Sender: TObject);
    procedure chkEnablePetUseClientPickItemsClick(Sender: TObject);
    procedure chkPetOnlyPickMonsterItemClick(Sender: TObject);
    procedure chkGamePetKillMonTriggerClick(Sender: TObject);
  private
    { Private declarations }
    boOpened: Boolean;
    boModValued: Boolean;

    procedure RefreshGamePetConfigList;
    procedure ModValue();
  public
    { Public declarations }
    procedure DoOpen;
  end;

var
  FrmGamePets: TFrmGamePets;

implementation

var
  SelGamePetConfig: PTGamePetConfig = nil;

{$R *.dfm}

{ TFrmGamePets }

procedure TFrmGamePets.DoOpen;
var
  I: Integer;
  MonInfo: pTMonInfo;
begin
  boOpened := False;

{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.MonsterList.LockR(11);
  try
{$IFEND}
    for I := 0 to UserEngine.MonsterList.Count - 1 do
    begin
      MonInfo := UserEngine.MonsterList.Items[I];

      if (MonInfo.btRace >= RC_ANIMAL) and (not (MonInfo.btRace in [RC_PLAYMOSTER{人形怪}, RC_ARCHERGUARD{弓箭手}, RC_MOVE_ARCHERGUARD
        {巡回弓箭手}, RC_TRUCKOBJECT{押镖车}, 55{练功师}, 110, 111{沙巴克城墙}])) then
        lstMonsterList.Items.Add(MonInfo.sName);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.MonsterList.UnLockR;
  end;
{$IFEND}


  cbbPetShowFile1.Clear;
  cbbPetShowFile2.Clear;
  cbbPetShowFile1.Items.Add('根据Appr计算');
  cbbPetShowFile2.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
  begin
    cbbPetShowFile1.Items.Add(g_EffectImageList.Strings[I]);
    cbbPetShowFile2.Items.Add(g_EffectImageList.Strings[I]);
  end;

  cbbLevelExp.AddItem('原始经验值', TObject(s_OldLevelExp));
  cbbLevelExp.AddItem('标准经验值', TObject(s_StdLevelExp));
  cbbLevelExp.AddItem('当前1/2倍经验', TObject(s_2Mult));
  cbbLevelExp.AddItem('当前1/5倍经验', TObject(s_5Mult));
  cbbLevelExp.AddItem('当前1/8倍经验', TObject(s_8Mult));
  cbbLevelExp.AddItem('当前1/10倍经验', TObject(s_10Mult));
  cbbLevelExp.AddItem('当前1/20倍经验', TObject(s_20Mult));
  cbbLevelExp.AddItem('当前1/30倍经验', TObject(s_30Mult));
  cbbLevelExp.AddItem('当前1/40倍经验', TObject(s_40Mult));
  cbbLevelExp.AddItem('当前1/50倍经验', TObject(s_50Mult));
  cbbLevelExp.AddItem('当前1/60倍经验', TObject(s_60Mult));
  cbbLevelExp.AddItem('当前1/70倍经验', TObject(s_70Mult));
  cbbLevelExp.AddItem('当前1/80倍经验', TObject(s_80Mult));
  cbbLevelExp.AddItem('当前1/90倍经验', TObject(s_90Mult));
  cbbLevelExp.AddItem('当前1/100倍经验', TObject(s_100Mult));
  cbbLevelExp.AddItem('当前1/150倍经验', TObject(s_150Mult));
  cbbLevelExp.AddItem('当前1/200倍经验', TObject(s_200Mult));
  cbbLevelExp.AddItem('当前1/250倍经验', TObject(s_250Mult));
  cbbLevelExp.AddItem('当前1/300倍经验', TObject(s_300Mult));

  GridLevelExp.ColWidths[0] := 50;
  GridLevelExp.ColWidths[1] := 200;
  GridLevelExp.Cells[0, 0] := '等级';
  GridLevelExp.Cells[1, 0] := '经验值';

  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[0, I] := IntToStr(I);
    GridLevelExp.Cells[1, I] := IntToStr(g_Config.dwPetNeedExps[I]);
  end;

  sePetHighLevel.Value := g_Config.nPetHighLevel;
  sePetHighLevelGetExp.Value := g_Config.nPetHighLevelGetExp;

  chkPetFixExp.Checked := g_Config.boPetUseFixExp;
  sePetBaseExp.Value := g_Config.nPetBaseExp;
  sePetAddExp.Value := g_Config.nPetAddExp;

  RefreshGamePetConfigList;

  if lstGamePets.Items.Count > 0 then
  begin
    lstGamePets.ItemIndex := 0;
    lstGamePets.OnClick(lstGamePets);
  end;

  chkOpenGamePet.Checked := g_Config.boOpenGamePet;
  chkEnabledPetAttack.Checked := g_Config.boEnabledPetAttack;
  chkDisableMonAttackPet.Checked := g_Config.boDisableMonAttackPet;
  chkDisableAllAttackPet.Checked := g_Config.boDisableAllAttackPet;
  chkEnabledPetPickup.Checked := g_Config.boEnabledPetPickup;
  chkPetOnlyPickMonsterItem.Checked := g_Config.boPetOnlyPickMonsterItem;
  chkPetPickupToMaster.Checked := g_Config.boPetPickupToMaster;
  chkPetPickupFullToMaster.Checked := g_Config.boPetPickupFullToMaster;
  chkPetPickupFullToMaster.Enabled := not g_Config.boPetPickupToMaster;
  chkPetQuickPickup.Checked := g_Config.boPetQuickPickup;
  chkPetRangePickup.Checked := g_Config.boPetRangePickup;
  sePetPickupRange.Value := g_Config.btPetPickupRange;

  chkPetNoEntity.Checked := g_Config.boPetNoEntity;
  chkPetSleepControlBySlave.Checked := g_Config.boPetSleepControlBySlave;
  chkPetNoShowHPProgress.Checked := g_Config.boPetNoShowHPProgress;


  //VMProtectBegin('VMProtect_FrmUseClientPickItems');

  {$IF NEED_KEY = 1}
  if g_nKey_UseClientPickItems = 1 then
  begin
    chkEnablePetUseClientPickItems.Checked := g_Config.boEnablePetUseClientPickItems;
  end
  else
  begin
    chkEnablePetUseClientPickItems.Visible := False;
  end;
  {$ELSE}
  chkEnablePetUseClientPickItems.Checked := g_Config.boEnablePetUseClientPickItems;
  {$IFEND}

  //VMProtectEnd();


  sePetAbilToMasterRate.Value := g_Config.nPetAbilToMasterRate;
  chkPetHPToMaster.Checked := g_Config.boPetHPToMaster;
  chkPetDCToMaster.Checked := g_Config.boPetDCToMaster;
  chkPetMCToMaster.Checked := g_Config.boPetMCToMaster;
  chkPetSCToMaster.Checked := g_Config.boPetSCToMaster;
  chkPetACToMaster.Checked := g_Config.boPetACToMaster;
  chkPetMACToMaster.Checked := g_Config.boPetMACToMaster;

  sePetUseItemIntervalTime.Value := g_Config.dwPetUseItemIntervalTime;
  chkCapturePetNeedItem.Checked := g_Config.boCapturePetNeedItem;
  chkCaptureOKDecDura.Checked := g_Config.boCaptureOKDecDura;

  chkPetShowMasterName.Checked := g_Config.boPetShowMasterName;
  sePetNameColor.Value := g_Config.btPetNameColor;
  edtPetSuffixName.Text := g_Config.sPetSuffixName;

  seGamePetMaxCount.Value := g_Config.nGamePetMaxCount;
  seGamePetNameCount.Value := g_Config.nGamePetNameCount;
  seGamePetRecallTime.Value := g_Config.nGamePetRecallTime;

  chkGamePetKillMonTrigger.Checked := g_Config.boGamePetKillMonTrigger;

  pgcMain.ActivePageIndex := 0;

  btnSavePet.Enabled := False;
  btnSavePetParams.Enabled := False;

  boOpened := True;
  ShowModal;
end;

procedure TFrmGamePets.lstGamePetsClick(Sender: TObject);
begin
  if lstGamePets.Items.Count > 0 then
  begin
    btnEditPet.Enabled := True;
    btnDelPet.Enabled := True;

    SelGamePetConfig := PTGamePetConfig(lstGamePets.Items.Objects[lstGamePets.ItemIndex]);

    edtPetName.Text := SelGamePetConfig.Name;
    sePetCaptureRate.Value := SelGamePetConfig.CaptureRate;

    chkLevelDifference.Checked := SelGamePetConfig.EnabledLevelDifference;
    seLevelDifference.Value := SelGamePetConfig.LevelDifference;
    seHPScale.Value := SelGamePetConfig.HPScale;

    cbbPetShowFile1.ItemIndex := SelGamePetConfig.ShowFile1;
    sePetShowStart1.Value := SelGamePetConfig.ShowStart1;
    sePetShowCount1.Value := SelGamePetConfig.ShowCount1;
    sePetShowTime1.Value := SelGamePetConfig.ShowTime1;
    sePetShowOffsetX1.Value := SelGamePetConfig.ShowOffsetX1;
    sePetShowOffsetY1.Value := SelGamePetConfig.ShowOffsetY1;

    cbbPetShowFile2.ItemIndex := SelGamePetConfig.ShowFile2;
    sePetShowStart2.Value := SelGamePetConfig.ShowStart2;
    sePetShowCount2.Value := SelGamePetConfig.ShowCount2;
    sePetShowTime2.Value := SelGamePetConfig.ShowTime2;
    sePetShowOffsetX2.Value := SelGamePetConfig.ShowOffsetX2;
    sePetShowOffsetY2.Value := SelGamePetConfig.ShowOffsetY2;

    sePetAddHP.Value := SelGamePetConfig.AddHP;
    cbbPetAddHPType.ItemIndex := Integer(SelGamePetConfig.IsAddHPRate);

    sePetAddDC1.Value := SelGamePetConfig.AddDC1;
    cbbPetAddDCType1.ItemIndex := Integer(SelGamePetConfig.IsAddDC1Rate);
    sePetAddDC2.Value := SelGamePetConfig.AddDC2;
    cbbPetAddDCType2.ItemIndex := Integer(SelGamePetConfig.IsAddDC2Rate);

    sePetAddMC1.Value := SelGamePetConfig.AddMC1;
    cbbPetAddMCType1.ItemIndex := Integer(SelGamePetConfig.IsAddMC1Rate);
    sePetAddMC2.Value := SelGamePetConfig.AddMC2;
    cbbPetAddMCType2.ItemIndex := Integer(SelGamePetConfig.IsAddMC2Rate);

    sePetAddSC1.Value := SelGamePetConfig.AddSC1;
    cbbPetAddSCType1.ItemIndex := Integer(SelGamePetConfig.IsAddSC1Rate);
    sePetAddSC2.Value := SelGamePetConfig.AddSC2;
    cbbPetAddSCType2.ItemIndex := Integer(SelGamePetConfig.IsAddSC2Rate);

    sePetAddAC1.Value := SelGamePetConfig.AddAC1;
    cbbPetAddACType1.ItemIndex := Integer(SelGamePetConfig.IsAddAC1Rate);
    sePetAddAC2.Value := SelGamePetConfig.AddAC2;
    cbbPetAddACType2.ItemIndex := Integer(SelGamePetConfig.IsAddAC2Rate);

    sePetAddMAC1.Value := SelGamePetConfig.AddMAC1;
    cbbPetAddMACType1.ItemIndex := Integer(SelGamePetConfig.IsAddMAC1Rate);
    sePetAddMAC2.Value := SelGamePetConfig.AddMAC2;
    cbbPetAddMACType2.ItemIndex := Integer(SelGamePetConfig.IsAddMAC2Rate);
  end
  else
  begin
    btnEditPet.Enabled := False;
    btnDelPet.Enabled := False;

    SelGamePetConfig := nil;
  end;
end;

procedure TFrmGamePets.btnEditPetClick(Sender: TObject);
begin
  if SelGamePetConfig = nil then
  begin
    Application.MessageBox('请选择一个需要修改的怪物！', '错误信息', MB_OK + MB_ICONERROR);
    lstGamePets.SetFocus;
    Exit;
  end;

  SelGamePetConfig.Name := edtPetName.Text;
  SelGamePetConfig.CaptureRate := sePetCaptureRate.Value;

  SelGamePetConfig.EnabledLevelDifference := chkLevelDifference.Checked;
  SelGamePetConfig.LevelDifference := seLevelDifference.Value;
  SelGamePetConfig.HPScale := seHPScale.Value;

  SelGamePetConfig.ShowFile1 := cbbPetShowFile1.ItemIndex;
  SelGamePetConfig.ShowStart1 := sePetShowStart1.Value;
  SelGamePetConfig.ShowCount1 := sePetShowCount1.Value;
  SelGamePetConfig.ShowTime1 := sePetShowTime1.Value;
  SelGamePetConfig.ShowOffsetX1 := sePetShowOffsetX1.Value;
  SelGamePetConfig.ShowOffsetY1 := sePetShowOffsetY1.Value;

  SelGamePetConfig.ShowFile2 := cbbPetShowFile2.ItemIndex;
  SelGamePetConfig.ShowStart2 := sePetShowStart2.Value;
  SelGamePetConfig.ShowCount2 := sePetShowCount2.Value;
  SelGamePetConfig.ShowTime2 := sePetShowTime2.Value;
  SelGamePetConfig.ShowOffsetX2 := sePetShowOffsetX2.Value;
  SelGamePetConfig.ShowOffsetY2 := sePetShowOffsetY2.Value;

  SelGamePetConfig.AddHP := sePetAddHP.Value;
  SelGamePetConfig.IsAddHPRate := cbbPetAddHPType.ItemIndex = 1;

  SelGamePetConfig.AddDC1 := sePetAddDC1.Value;
  SelGamePetConfig.IsAddDC1Rate := cbbPetAddDCType1.ItemIndex = 1;
  SelGamePetConfig.AddDC2 := sePetAddDC2.Value;
  SelGamePetConfig.IsAddDC2Rate := cbbPetAddDCType2.ItemIndex = 1;

  SelGamePetConfig.AddMC1 := sePetAddMC1.Value;
  SelGamePetConfig.IsAddMC1Rate := cbbPetAddMCType1.ItemIndex = 1;
  SelGamePetConfig.AddMC2 := sePetAddMC2.Value;
  SelGamePetConfig.IsAddMC2Rate := cbbPetAddMCType2.ItemIndex = 1;

  SelGamePetConfig.AddSC1 := sePetAddSC1.Value;
  SelGamePetConfig.IsAddSC1Rate := cbbPetAddSCType1.ItemIndex = 1;
  SelGamePetConfig.AddSC2 := sePetAddSC2.Value;
  SelGamePetConfig.IsAddSC2Rate := cbbPetAddSCType2.ItemIndex = 1;

  SelGamePetConfig.AddAC1 := sePetAddAC1.Value;
  SelGamePetConfig.IsAddAC1Rate := cbbPetAddACType1.ItemIndex = 1;
  SelGamePetConfig.AddAC2 := sePetAddAC2.Value;
  SelGamePetConfig.IsAddAC2Rate := cbbPetAddACType2.ItemIndex = 1;

  SelGamePetConfig.AddMAC1 := sePetAddMAC1.Value;
  SelGamePetConfig.IsAddMAC1Rate := cbbPetAddMACType1.ItemIndex = 1;
  SelGamePetConfig.AddMAC2 := sePetAddMAC2.Value;
  SelGamePetConfig.IsAddMAC2Rate := cbbPetAddMACType2.ItemIndex = 1;

  RefreshGamePetConfigList;
  btnSavePet.Enabled := True;
end;

procedure TFrmGamePets.RefreshGamePetConfigList;
var
  I: Integer;
  GamePetConfig: PTGamePetConfig;
begin
  lstGamePets.Clear;
  for I := 0 to g_GamePetConfigList.Count - 1 do
  begin
    GamePetConfig := g_GamePetConfigList.Items[I];
    lstGamePets.Items.AddObject(GamePetConfig.Name, TObject(GamePetConfig));
  end;
end;

procedure TFrmGamePets.btnAddPetClick(Sender: TObject);
var
  GamePetConfig: PTGamePetConfig;
  S: string;
begin
  S := Trim(edtPetName.Text);
  if Length(S) = 0 then
  begin
    Application.MessageBox('请输入怪物名称！', '错误信息', MB_OK + MB_ICONERROR);
    edtPetName.SetFocus;
    Exit;
  end;

  if lstGamePets.Items.IndexOf(S) >= 0 then
  begin
    Application.MessageBox(PChar('怪物 ' + S + ' 已经存在！'), '错误信息', MB_OK + MB_ICONERROR);
    edtPetName.SetFocus;
    Exit;
  end;

  New(GamePetConfig);
  g_GamePetConfigList.Add(GamePetConfig);

  GamePetConfig.Name := edtPetName.Text;
  GamePetConfig.CaptureRate := sePetCaptureRate.Value;

  GamePetConfig.EnabledLevelDifference := chkLevelDifference.Checked;
  GamePetConfig.LevelDifference := seLevelDifference.Value;
  GamePetConfig.HPScale := seHPScale.Value;

  GamePetConfig.ShowFile1 := cbbPetShowFile1.ItemIndex;
  GamePetConfig.ShowStart1 := sePetShowStart1.Value;
  GamePetConfig.ShowCount1 := sePetShowCount1.Value;
  GamePetConfig.ShowTime1 := sePetShowTime1.Value;
  GamePetConfig.ShowOffsetX1 := sePetShowOffsetX1.Value;
  GamePetConfig.ShowOffsetY1 := sePetShowOffsetY1.Value;

  GamePetConfig.ShowFile2 := cbbPetShowFile2.ItemIndex;
  GamePetConfig.ShowStart2 := sePetShowStart2.Value;
  GamePetConfig.ShowCount2 := sePetShowCount2.Value;
  GamePetConfig.ShowTime2 := sePetShowTime2.Value;
  GamePetConfig.ShowOffsetX2 := sePetShowOffsetX2.Value;
  GamePetConfig.ShowOffsetY2 := sePetShowOffsetY2.Value;

  GamePetConfig.AddHP := sePetAddHP.Value;
  GamePetConfig.IsAddHPRate := cbbPetAddHPType.ItemIndex = 1;

  GamePetConfig.AddDC1 := sePetAddDC1.Value;
  GamePetConfig.IsAddDC1Rate := cbbPetAddDCType1.ItemIndex = 1;
  GamePetConfig.AddDC2 := sePetAddDC2.Value;
  GamePetConfig.IsAddDC2Rate := cbbPetAddDCType2.ItemIndex = 1;

  GamePetConfig.AddMC1 := sePetAddMC1.Value;
  GamePetConfig.IsAddMC1Rate := cbbPetAddMCType1.ItemIndex = 1;
  GamePetConfig.AddMC2 := sePetAddMC2.Value;
  GamePetConfig.IsAddMC2Rate := cbbPetAddMCType2.ItemIndex = 1;

  GamePetConfig.AddSC1 := sePetAddSC1.Value;
  GamePetConfig.IsAddSC1Rate := cbbPetAddSCType1.ItemIndex = 1;
  GamePetConfig.AddSC2 := sePetAddSC2.Value;
  GamePetConfig.IsAddSC2Rate := cbbPetAddSCType2.ItemIndex = 1;

  GamePetConfig.AddAC1 := sePetAddAC1.Value;
  GamePetConfig.IsAddAC1Rate := cbbPetAddACType1.ItemIndex = 1;
  GamePetConfig.AddAC2 := sePetAddAC2.Value;
  GamePetConfig.IsAddAC2Rate := cbbPetAddACType2.ItemIndex = 1;

  GamePetConfig.AddMAC1 := sePetAddMAC1.Value;
  GamePetConfig.IsAddMAC1Rate := cbbPetAddMACType1.ItemIndex = 1;
  GamePetConfig.AddMAC2 := sePetAddMAC2.Value;
  GamePetConfig.IsAddMAC2Rate := cbbPetAddMACType2.ItemIndex = 1;

  RefreshGamePetConfigList;

  lstGamePets.ItemIndex := lstGamePets.Items.Count - 1;
  SelGamePetConfig := GamePetConfig;

  btnSavePet.Enabled := True;
end;

procedure TFrmGamePets.btnDelPetClick(Sender: TObject);
var
  I: Integer;
  IsOK: Boolean;
begin
  if SelGamePetConfig = nil then
  begin
    Application.MessageBox('请选择一个需要修改的物品！', '错误信息', MB_OK + MB_ICONERROR);
    lstGamePets.SetFocus;
    Exit;
  end;

  IsOK := False;
  for I := 0 to g_GamePetConfigList.Count - 1 do
  begin
    if SelGamePetConfig = g_GamePetConfigList.Items[I] then
    begin
      Dispose(SelGamePetConfig);
      g_GamePetConfigList.Delete(I);

      IsOK := True;
      Break;
    end;
  end;

  if not IsOK then
  begin
    Application.MessageBox('删除失败！', '错误信息', MB_OK + MB_ICONERROR);
  end
  else
  begin
    SelGamePetConfig := nil;
    RefreshGamePetConfigList;
  end;
end;

procedure TFrmGamePets.btnSavePetClick(Sender: TObject);
begin
  SaveGamePetsConfig;
  btnSavePet.Enabled := False;
end;

procedure TFrmGamePets.lstMonsterListDblClick(Sender: TObject);
begin
  if lstMonsterList.ItemIndex >= 0 then
  begin
    edtPetName.Text := lstMonsterList.Items[lstMonsterList.ItemIndex];
  end;
end;

procedure TFrmGamePets.ModValue;
begin
  boModValued := True;
  btnSavePet.Enabled := True;
  btnSavePetParams.Enabled := True;
end;

procedure TFrmGamePets.lstMonsterListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  I: Integer;
  sMonName: string;
begin
  case Key of
    Word('F'):
      begin
        if ssCtrl in Shift then
        begin
          Key := 0;
          sMonName := '';
          if not InputQuery('怪物查找', '输入怪物名称:', sMonName) then
            Exit;
          if sMonName = '' then
            Exit;
          for I := 0 to TListBox(Sender).Items.Count - 1 do
          begin
            if TListBox(Sender).Items.Strings[I] = sMonName then
            begin
              TListBox(Sender).ItemIndex := I;
              Break;
            end;
          end;
        end;
      end;
  end;
end;

procedure TFrmGamePets.chkOpenGamePetClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenGamePet := chkOpenGamePet.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetNoEntityClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetNoEntity := chkPetNoEntity.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetNoShowHPProgressClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetNoShowHPProgress := chkPetNoShowHPProgress.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetSleepControlBySlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetSleepControlBySlave := chkPetSleepControlBySlave.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkEnabledPetAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnabledPetAttack := chkEnabledPetAttack.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkEnabledPetPickupClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnabledPetPickup := chkEnabledPetPickup.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetOnlyPickMonsterItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetOnlyPickMonsterItem := chkPetOnlyPickMonsterItem.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetPickupToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetPickupToMaster := chkPetPickupToMaster.Checked;
  ModValue();

  chkPetPickupFullToMaster.Enabled := not g_Config.boPetPickupToMaster;
end;

procedure TFrmGamePets.chkPetPickupFullToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetPickupFullToMaster := chkPetPickupFullToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.sePetAbilToMasterRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPetAbilToMasterRate := sePetAbilToMasterRate.Value;
  ModValue();
end;

procedure TFrmGamePets.chkPetHPToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetHPToMaster := chkPetHPToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetDCToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetDCToMaster := chkPetDCToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetMCToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetMCToMaster := chkPetMCToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetSCToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetSCToMaster := chkPetSCToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetACToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetACToMaster := chkPetACToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetMACToMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetMACToMaster := chkPetMACToMaster.Checked;
  ModValue();
end;

procedure TFrmGamePets.btnSavePetParamsClick(Sender: TObject);
begin
  // 开启宠物系统
  Config.WriteBool('Setup', 'OpenGamePet', g_Config.boOpenGamePet);

  // 允许宠物攻击
  Config.WriteBool('Setup', 'EnabledPetAttack', g_Config.boEnabledPetAttack);

  Config.WriteBool('Setup', 'DisableMonAttackPet', g_Config.boDisableMonAttackPet);
  Config.WriteBool('Setup', 'DisableAllAttackPet', g_Config.boDisableAllAttackPet);

  // 允许宠物捡物
  Config.WriteBool('Setup', 'EnabledPetPickup', g_Config.boEnabledPetPickup);

  Config.WriteBool('Setup', 'PetOnlyPickMonsterItem', g_Config.boPetOnlyPickMonsterItem);

  // 宠物直接捡物到主人背包
  Config.WriteBool('Setup', 'PetPickupToMaster', g_Config.boPetPickupToMaster);

  // 宠物包满时捡到物品放主人包裹
  Config.WriteBool('Setup', 'PetPickupFullToMaster', g_Config.boPetPickupFullToMaster);

  Config.WriteBool('Setup', 'PetQuickPickup', g_Config.boPetQuickPickup);

  Config.WriteBool('Setup', 'PetRangePickup', g_Config.boPetRangePickup);
  Config.WriteInteger('Setup', 'PetPickupRange', g_Config.btPetPickupRange);

  // 宝宝无实体
  Config.WriteBool('Setup', 'PetNoEntity', g_Config.boPetNoEntity);

  Config.WriteBool('Setup', 'PetNoShowHPProgress', g_Config.boPetNoShowHPProgress);

  // 宠物休息受宝宝控制
  Config.WriteBool('Setup', 'PetSleepControlBySlave', g_Config.boPetSleepControlBySlave);

  // 宠物叠加属性给主人倍率
  Config.WriteInteger('Setup', 'PetAbilToMasterRate', g_Config.nPetAbilToMasterRate);

  // 叠加HP给主人
  Config.WriteBool('Setup', 'PetHPToMaster', g_Config.boPetHPToMaster);

  // 叠加攻击给主人
  Config.WriteBool('Setup', 'PetDCToMaster', g_Config.boPetDCToMaster);

  // 叠加魔法给主人
  Config.WriteBool('Setup', 'PetMCToMaster', g_Config.boPetMCToMaster);

  // 叠加道术给主人
  Config.WriteBool('Setup', 'PetSCToMaster', g_Config.boPetSCToMaster);

  // 叠加防御给主人
  Config.WriteBool('Setup', 'PetACToMaster', g_Config.boPetACToMaster);

  // 叠加魔防给主人
  Config.WriteBool('Setup', 'PetMACToMaster', g_Config.boPetMACToMaster);

  Config.WriteInteger('Setup', 'PetUseItemIntervalTime', g_Config.dwPetUseItemIntervalTime);
  Config.WriteBool('Setup', 'CapturePetNeedItem', g_Config.boCapturePetNeedItem);
  Config.WriteBool('Setup', 'CaptureOKDecDura', g_Config.boCaptureOKDecDura);

  Config.WriteBool('Setup', 'PetShowMasterName', g_Config.boPetShowMasterName);
  Config.WriteInteger('Setup', 'PetNameColor', g_Config.btPetNameColor);
  Config.WriteString('Setup', 'PetSuffixName', g_Config.sPetSuffixName);

  Config.WriteInteger('Setup', 'GamePetMaxCount', g_Config.nGamePetMaxCount);
  Config.WriteInteger('Setup', 'GamePetNameCount', g_Config.nGamePetNameCount);

  Config.WriteInteger('Setup', 'GamePetRecallTime', g_Config.nGamePetRecallTime);

  Config.WriteBool('Setup', 'GamePetKillMonTrigger', g_Config.boGamePetKillMonTrigger);

  //VMProtectBegin('VMProtect_WirteIniUseClientPickItems');

  if g_nKey_UseClientPickItems = 1 then
  begin
    Config.WriteBool('Setup', 'EnablePetUseClientPickItems', g_Config.boEnablePetUseClientPickItems);
  end;

  //VMProtectEnd();

  UserEngine.SendServerConfig();
  btnSavePetParams.Enabled := False;
end;

procedure TFrmGamePets.cbbLevelExpClick(Sender: TObject);
const
  HIGH_VALUE = 4200000000;
var
  I: Integer;
  LevelExpScheme: TLevelExpScheme;
  dwOneLevelExp: LongWord;
  dwExp: LongWord;
  Int64Value: Int64;
  IsHighLongWord: Boolean;
begin
  if not boOpened then
    Exit;
  if Application.MessageBox('升级经验计划设置的经验将立即生效，是否确认使用此经验计划？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDNO then
  begin
    Exit;
  end;
  // cbbLevelExp.AddItem('原始经验值', TObject(s_OldLevelExp));
  // cbbLevelExp.AddItem('标准经验值', TObject(s_StdLevelExp));
  LevelExpScheme := TLevelExpScheme(cbbLevelExp.Items.Objects[cbbLevelExp.ItemIndex]);
  case LevelExpScheme of                                                                            //
    s_OldLevelExp:
      g_Config.dwPetNeedExps := g_dwOldNeedExps;
    s_StdLevelExp:
      begin
        IsHighLongWord := False;
        g_Config.dwPetNeedExps := g_dwOldNeedExps;
        dwOneLevelExp := 4000000000 div (High(g_Config.dwPetNeedExps){ div 2});
        for I := 1 to MAXCHANGELEVEL do
        begin
          if (26 + I) > MAXCHANGELEVEL then
            Break;

          if IsHighLongWord then
            dwExp := HIGH_VALUE
          else
          begin
            Int64Value := Int64(dwOneLevelExp) * Int64(I);
            if Int64Value >= HIGH_VALUE then
            begin
              IsHighLongWord := True;
              Int64Value := HIGH_VALUE;
            end;
            dwExp := Int64Value;
          end;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[26 + I] := dwExp;
        end;
      end;
    s_2Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 2;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_5Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 5;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_8Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 8;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_10Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 10;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_20Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 20;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_30Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 30;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_40Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 40;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_50Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 50;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_60Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 60;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_70Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 70;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_80Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 80;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_90Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 90;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_100Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 100;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_150Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 150;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_200Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 200;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_250Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 250;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
    s_300Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwPetNeedExps[I] div 300;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwPetNeedExps[I] := dwExp;
        end;
      end;
  end;
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[1, I] := IntToStr(g_Config.dwPetNeedExps[I]);
  end;
  ModValue();
end;

procedure TFrmGamePets.GridLevelExpSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TFrmGamePets.btnSaveExpClick(Sender: TObject);
var
  I: Integer;
  dwExp: Int64;
  NeedExps: TLevelNeedExp;
begin
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    dwExp := StrToInt64Def(GridLevelExp.Cells[1, I], 0);
    if (dwExp > High(LongWord)) then
    begin
      Application.MessageBox(PChar('等级 ' + IntToStr(I) + ' 升级经验设置错误！'), '错误信息', MB_OK + MB_ICONERROR);
      GridLevelExp.Row := I;
      GridLevelExp.SetFocus;
      Exit;
    end;
    NeedExps[I] := dwExp;
  end;
  g_Config.dwPetNeedExps := NeedExps;

  Config.WriteBool('Setup', 'PetUseFixExp', g_Config.boPetUseFixExp);
  Config.WriteInteger('Setup', 'PetBaseExp', g_Config.nPetBaseExp);
  Config.WriteInteger('Setup', 'PetAddExp', g_Config.nPetAddExp);
  Config.WriteInteger('Setup', 'PetHighLevel', g_Config.nPetHighLevel);
  Config.WriteInteger('Setup', 'PetHighLevelGetExp', g_Config.nPetHighLevelGetExp);

  for I := Low(g_Config.dwPetNeedExps) to High(g_Config.dwPetNeedExps) do
  begin
    ExpConfig.WriteString('GamePetExp', 'Level' + IntToStr(I), IntToStr(g_Config.dwPetNeedExps[I]));
  end;

  boModValued := False;
  btnSaveExp.Enabled := False;
end;

procedure TFrmGamePets.sePetHighLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPetHighLevel := sePetHighLevel.Value;
  ModValue();
end;

procedure TFrmGamePets.sePetHighLevelGetExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPetHighLevelGetExp := sePetHighLevelGetExp.Value;
  ModValue();
end;

procedure TFrmGamePets.chkPetFixExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetUseFixExp := chkPetFixExp.Checked;
  ModValue();
end;

procedure TFrmGamePets.sePetBaseExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPetBaseExp := sePetBaseExp.Value;
  ModValue();
end;

procedure TFrmGamePets.sePetAddExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPetAddExp := sePetAddExp.Value;
  ModValue();
end;

procedure TFrmGamePets.chkPetShowMasterNameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetShowMasterName := chkPetShowMasterName.Checked;
  ModValue();
end;

procedure TFrmGamePets.sePetNameColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btPetNameColor := Byte(sePetNameColor.Value);
  ModValue();
end;

procedure TFrmGamePets.edtPetSuffixNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sPetSuffixName := Trim(edtPetSuffixName.Text);
  ModValue();
end;

procedure TFrmGamePets.sePetUseItemIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwPetUseItemIntervalTime := sePetUseItemIntervalTime.Value;
  ModValue();
end;

procedure TFrmGamePets.chkCapturePetNeedItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCapturePetNeedItem := chkCapturePetNeedItem.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkCaptureOKDecDuraClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCaptureOKDecDura := chkCaptureOKDecDura.Checked;
  ModValue();
end;

procedure TFrmGamePets.seGamePetMaxCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGamePetMaxCount := seGamePetMaxCount.Value;
  ModValue();
end;

procedure TFrmGamePets.seGamePetNameCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGamePetNameCount := seGamePetNameCount.Value;
  ModValue();
end;

procedure TFrmGamePets.seGamePetRecallTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGamePetRecallTime := seGamePetRecallTime.Value;
  ModValue();
end;

procedure TFrmGamePets.chkDisableMonAttackPetClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableMonAttackPet := chkDisableMonAttackPet.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkDisableAllAttackPetClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableAllAttackPet := chkDisableAllAttackPet.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetQuickPickupClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetQuickPickup := chkPetQuickPickup.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkPetRangePickupClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPetRangePickup := chkPetRangePickup.Checked;
  ModValue();
end;

procedure TFrmGamePets.sePetPickupRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btPetPickupRange := sePetPickupRange.Value;
  ModValue();
end;

procedure TFrmGamePets.chkEnablePetUseClientPickItemsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnablePetUseClientPickItems := chkEnablePetUseClientPickItems.Checked;
  ModValue();
end;

procedure TFrmGamePets.chkGamePetKillMonTriggerClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGamePetKillMonTrigger := chkGamePetKillMonTrigger.Checked;
  ModValue();
end;

end.

