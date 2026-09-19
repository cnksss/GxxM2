unit ViewLevel;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, Grids, ObjBase, ObjHero,
  SpinEditEx, ObjPlayer, ExtCtrls, Grobal2;

type
  TfrmViewLevel = class(TForm)
    btnSave: TButton;
    grp1: TGroupBox;
    GridHumanInfo: TStringGrid;
    Label1: TLabel;
    cbbDefJob: TComboBox;
    rbDef: TRadioButton;
    GroupBox1: TGroupBox;
    GroupBox3: TGroupBox;
    lbl25: TLabel;
    lbl27: TLabel;
    lbl9: TLabel;
    lbl11: TLabel;
    lbl14: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    seAC1: TSpinEditLongWord;
    seAC2: TSpinEditLongWord;
    seMAC1: TSpinEditLongWord;
    seMAC2: TSpinEditLongWord;
    seDC1: TSpinEditLongWord;
    seDC2: TSpinEditLongWord;
    seMC1: TSpinEditLongWord;
    seMC2: TSpinEditLongWord;
    seSC1: TSpinEditLongWord;
    seSC2: TSpinEditLongWord;
    seMaxHP: TSpinEditLongWord;
    seMaxMP: TSpinEditLongWord;
    GroupBox4: TGroupBox;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    Label13: TLabel;
    seAddAC1: TSpinEditLongWord;
    seAddAC2: TSpinEditLongWord;
    seAddMAC1: TSpinEditLongWord;
    seAddMAC2: TSpinEditLongWord;
    seAddDC1: TSpinEditLongWord;
    seAddDC2: TSpinEditLongWord;
    seAddMC1: TSpinEditLongWord;
    seAddMC2: TSpinEditLongWord;
    seAddSC1: TSpinEditLongWord;
    seAddSC2: TSpinEditLongWord;
    seAddMaxHP: TSpinEditLongWord;
    seAddMaxMP: TSpinEditLongWord;
    rbCustomAutoCalc: TRadioButton;
    rbCustomSetValue: TRadioButton;
    GroupBox5: TGroupBox;
    lstDefLevels: TListBox;
    Label4: TLabel;
    cbbDefUserType: TComboBox;
    GroupBox6: TGroupBox;
    seCustomLevel: TSpinEditLongWord;
    lstCustomLevels: TListBox;
    Label2: TLabel;
    cbbCustomJob: TComboBox;
    Label3: TLabel;
    cbbCustomUserType: TComboBox;
    seDefLevel: TSpinEditLongWord;
    rbCustom: TRadioButton;
    btnInitCustom: TButton;
    Label14: TLabel;
    Label15: TLabel;
    seMaxWeight: TSpinEditLongWord;
    seMaxWearWeight: TSpinEditLongWord;
    Label16: TLabel;
    seMaxHandWeight: TSpinEditLongWord;
    Label17: TLabel;
    Label18: TLabel;
    seAddMaxWeight: TSpinEditLongWord;
    seAddMaxWearWeight: TSpinEditLongWord;
    Label19: TLabel;
    seAddMaxHandWeight: TSpinEditLongWord;
    procedure btnSaveClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure cbbDefJobChange(Sender: TObject);
    procedure cbbDefUserTypeChange(Sender: TObject);
    procedure seDefLevelChange(Sender: TObject);
    procedure lstDefLevelsClick(Sender: TObject);
    procedure lstCustomLevelsClick(Sender: TObject);
    procedure seCustomLevelChange(Sender: TObject);
    procedure btnInitCustomClick(Sender: TObject);
    procedure rbDefClick(Sender: TObject);
    procedure seAC1Change(Sender: TObject);
  private
    PlayObject: TPlayObject;
    HeroObject: THeroObject;
    procedure RecalcHuman();
    procedure RefView();
    procedure RefCustomView();
    { Private declarations }
  public
    procedure Open;
    { Public declarations }
  end;

var
  frmViewLevel: TfrmViewLevel;

implementation

uses
  M2Share, Envir;

{$R *.dfm}
{ TfrmLevel }

procedure TfrmViewLevel.Open;
begin
  if g_BaseAbilConfig.UseDefault then
    rbDef.Checked := True
  else
    rbCustom.Checked := True;

  PlayObject := TPlayObject.Create;
  PlayObject.m_Abil.Level := 1;
  PlayObject.m_btJob := 0;
  PlayObject.m_sMapName := '0';
  PlayObject.m_PEnvir := g_MapManager.FindMap('0');
  PlayObject.m_nCurrX := 330;
  PlayObject.m_nCurrY := 266;

  HeroObject := THeroObject.Create;
  HeroObject.m_Abil.Level := 1;
  HeroObject.m_btJob := 0;
  HeroObject.m_sMapName := '0';
  HeroObject.m_PEnvir := g_MapManager.FindMap('0');
  HeroObject.m_nCurrX := 330;
  HeroObject.m_nCurrY := 266;

  seDefLevel.Value := 1;
  cbbDefUserType.ItemIndex := 0;
  cbbDefJob.ItemIndex := 0;
  RefView();
  RefCustomView;
  btnSave.Enabled := False;

  ShowModal;
  PlayObject.Free;
  HeroObject.Free;
end;

(*
  var
  nJob, nLevel: Integer;
  sFileName, sSection, sLevel: string;
  Ini: TIniFileEx;
  BaseAbil: PBaseAbilInfo;
  begin
  sFileName := g_sSelfFilePath + sBaseAbilConfigFileName;
  Ini := TIniFileEx.Create(sFileName);
  try
  Ini.WriteBool('Setup', 'UseDefault', g_BaseAbilConfig.UseDefault);

  for nJob := JOB_WARR to JOB_TAOS do
  begin
  sSection := 'Hum_' + IntToStr(nJob);

  Ini.WriteBool(sSection, 'AutoCalcLevel1000', g_BaseAbilConfig.HumAbil[nJob].AutoCalcLevel1000);

  for nLevel := Low(g_BaseAbilConfig.HumAbil[nJob].Base) to High(g_BaseAbilConfig.HumAbil[nJob].Base) do
  begin
  BaseAbil := @g_BaseAbilConfig.HumAbil[nJob].Base[nLevel];

  sLevel := IntToStr(nLevel + 1);

  Ini.WriteInteger(sSection, 'AC1_' + sLevel, BaseAbil.AC1);
  Ini.WriteInteger(sSection, 'AC2_' + sLevel, BaseAbil.AC2);
  Ini.WriteInteger(sSection, 'MAC1_' + sLevel, BaseAbil.MAC1);
  Ini.WriteInteger(sSection, 'MAC2_' + sLevel, BaseAbil.MAC2);
  Ini.WriteInteger(sSection, 'DC1_' + sLevel, BaseAbil.DC1);
  Ini.WriteInteger(sSection, 'DC2_' + sLevel, BaseAbil.DC2);
  Ini.WriteInteger(sSection, 'MC1_' + sLevel, BaseAbil.MC1);
  Ini.WriteInteger(sSection, 'MC2_' + sLevel, BaseAbil.MC2);
  Ini.WriteInteger(sSection, 'SC1_' + sLevel, BaseAbil.SC1);
  Ini.WriteInteger(sSection, 'SC2_' + sLevel, BaseAbil.SC2);
  Ini.WriteInteger(sSection, 'MaxHP_' + sLevel, BaseAbil.MaxHP);
  Ini.WriteInteger(sSection, 'MaxMP_' + sLevel, BaseAbil.MaxMP);
  end;

  BaseAbil := @g_BaseAbilConfig.HumAbil[nJob].Add;
  Ini.WriteInteger(sSection, 'AddAC1', BaseAbil.AC1);
  Ini.WriteInteger(sSection, 'AddAC2', BaseAbil.AC2);
  Ini.WriteInteger(sSection, 'AddMAC1', BaseAbil.MAC1);
  Ini.WriteInteger(sSection, 'AddMAC2', BaseAbil.MAC2);
  Ini.WriteInteger(sSection, 'AddDC1', BaseAbil.DC1);
  Ini.WriteInteger(sSection, 'AddDC2', BaseAbil.DC2);
  Ini.WriteInteger(sSection, 'AddMC1', BaseAbil.MC1);
  Ini.WriteInteger(sSection, 'AddMC2', BaseAbil.MC2);
  Ini.WriteInteger(sSection, 'AddSC1', BaseAbil.SC1);
  Ini.WriteInteger(sSection, 'AddSC2', BaseAbil.SC2);
  Ini.WriteInteger(sSection, 'AddMaxHP', BaseAbil.MaxHP);
  Ini.WriteInteger(sSection, 'AddMaxMP', BaseAbil.MaxMP);
  end;

  for nJob := JOB_WARR to JOB_TAOS do
  begin
  sSection := 'Hero_' + IntToStr(nJob);

  Ini.WriteBool(sSection, 'AutoCalcLevel1000', g_BaseAbilConfig.HeroAbil[nJob].AutoCalcLevel1000);

  for nLevel := Low(g_BaseAbilConfig.HeroAbil[nJob].Base) to High(g_BaseAbilConfig.HeroAbil[nJob].Base) do
  begin
  BaseAbil := @g_BaseAbilConfig.HeroAbil[nJob].Base[nLevel];

  sLevel := IntToStr(nLevel + 1);

  Ini.WriteInteger(sSection, 'AC1_' + sLevel, BaseAbil.AC1);
  Ini.WriteInteger(sSection, 'AC2_' + sLevel, BaseAbil.AC2);
  Ini.WriteInteger(sSection, 'MAC1_' + sLevel, BaseAbil.MAC1);
  Ini.WriteInteger(sSection, 'MAC2_' + sLevel, BaseAbil.MAC2);
  Ini.WriteInteger(sSection, 'DC1_' + sLevel, BaseAbil.DC1);
  Ini.WriteInteger(sSection, 'DC2_' + sLevel, BaseAbil.DC2);
  Ini.WriteInteger(sSection, 'MC1_' + sLevel, BaseAbil.MC1);
  Ini.WriteInteger(sSection, 'MC2_' + sLevel, BaseAbil.MC2);
  Ini.WriteInteger(sSection, 'SC1_' + sLevel, BaseAbil.SC1);
  Ini.WriteInteger(sSection, 'SC2_' + sLevel, BaseAbil.SC2);
  Ini.WriteInteger(sSection, 'MaxHP_' + sLevel, BaseAbil.MaxHP);
  Ini.WriteInteger(sSection, 'MaxMP_' + sLevel, BaseAbil.MaxMP);
  end;

  BaseAbil := @g_BaseAbilConfig.HeroAbil[nJob].Add;
  Ini.WriteInteger(sSection, 'AddAC1', BaseAbil.AC1);
  Ini.WriteInteger(sSection, 'AddAC2', BaseAbil.AC2);
  Ini.WriteInteger(sSection, 'AddMAC1', BaseAbil.MAC1);
  Ini.WriteInteger(sSection, 'AddMAC2', BaseAbil.MAC2);
  Ini.WriteInteger(sSection, 'AddDC1', BaseAbil.DC1);
  Ini.WriteInteger(sSection, 'AddDC2', BaseAbil.DC2);
  Ini.WriteInteger(sSection, 'AddMC1', BaseAbil.MC1);
  Ini.WriteInteger(sSection, 'AddMC2', BaseAbil.MC2);
  Ini.WriteInteger(sSection, 'AddSC1', BaseAbil.SC1);
  Ini.WriteInteger(sSection, 'AddSC2', BaseAbil.SC2);
  Ini.WriteInteger(sSection, 'AddMaxHP', BaseAbil.MaxHP);
  Ini.WriteInteger(sSection, 'AddMaxMP', BaseAbil.MaxMP);
  end;
  finally
  Ini.Free;
  end;
*)
procedure TfrmViewLevel.btnSaveClick(Sender: TObject);
const
  Values: array [Boolean] of string = ('0', '1');
var
  nJob, nLevel: Integer;
  sFileName, sLevel: string;
  Ini: TStringList;
  BaseAbil: PBaseAbilInfo;
begin
  Ini := TStringList.Create();
  try
    Ini.Add('[Setup]');
    Ini.Add('UseDefault=' + Values[g_BaseAbilConfig.UseDefault]);

    for nJob := JOB_WARR to JOB_TAOS do
    begin
      Ini.Add('');
      Ini.Add('[Hum_' + IntToStr(nJob) + ']');

      Ini.Add('AutoCalcLevel1000=' + Values[g_BaseAbilConfig.HumAbil[nJob].AutoCalcLevel1000]);

      for nLevel := Low(g_BaseAbilConfig.HumAbil[nJob].Base) to High(g_BaseAbilConfig.HumAbil[nJob].Base) do
      begin
        BaseAbil := @g_BaseAbilConfig.HumAbil[nJob].Base[nLevel];

        sLevel := IntToStr(nLevel + 1);

        Ini.Add('AC1_' + sLevel + '=' + IntToStr(BaseAbil.AC1));
        Ini.Add('AC2_' + sLevel + '=' + IntToStr(BaseAbil.AC2));
        Ini.Add('MAC1_' + sLevel + '=' + IntToStr(BaseAbil.MAC1));
        Ini.Add('MAC2_' + sLevel + '=' + IntToStr(BaseAbil.MAC2));
        Ini.Add('DC1_' + sLevel + '=' + IntToStr(BaseAbil.DC1));
        Ini.Add('DC2_' + sLevel + '=' + IntToStr(BaseAbil.DC2));
        Ini.Add('MC1_' + sLevel + '=' + IntToStr(BaseAbil.MC1));
        Ini.Add('MC2_' + sLevel + '=' + IntToStr(BaseAbil.MC2));
        Ini.Add('SC1_' + sLevel + '=' + IntToStr(BaseAbil.SC1));
        Ini.Add('SC2_' + sLevel + '=' + IntToStr(BaseAbil.SC2));
        Ini.Add('MaxHP_' + sLevel + '=' + IntToStr(BaseAbil.MaxHP));
        Ini.Add('MaxMP_' + sLevel + '=' + IntToStr(BaseAbil.MaxMP));
        Ini.Add('MaxWeight_' + sLevel + '=' + IntToStr(BaseAbil.MaxWeight));
        Ini.Add('MaxWearWeight_' + sLevel + '=' + IntToStr(BaseAbil.MaxWearWeight));
        Ini.Add('MaxHandWeight_' + sLevel + '=' + IntToStr(BaseAbil.MaxHandWeight));
      end;

      Ini.Add('');

      BaseAbil := @g_BaseAbilConfig.HumAbil[nJob].Add;
      Ini.Add('AddAC1=' + IntToStr(BaseAbil.AC1));
      Ini.Add('AddAC2=' + IntToStr(BaseAbil.AC2));
      Ini.Add('AddMAC1=' + IntToStr(BaseAbil.MAC1));
      Ini.Add('AddMAC2=' + IntToStr(BaseAbil.MAC2));
      Ini.Add('AddDC1=' + IntToStr(BaseAbil.DC1));
      Ini.Add('AddDC2=' + IntToStr(BaseAbil.DC2));
      Ini.Add('AddMC1=' + IntToStr(BaseAbil.MC1));
      Ini.Add('AddMC2=' + IntToStr(BaseAbil.MC2));
      Ini.Add('AddSC1=' + IntToStr(BaseAbil.SC1));
      Ini.Add('AddSC2=' + IntToStr(BaseAbil.SC2));
      Ini.Add('AddMaxHP=' + IntToStr(BaseAbil.MaxHP));
      Ini.Add('AddMaxMP=' + IntToStr(BaseAbil.MaxMP));
      Ini.Add('AddMaxWeight=' + IntToStr(BaseAbil.MaxWeight));
      Ini.Add('AddMaxWearWeight=' + IntToStr(BaseAbil.MaxWearWeight));
      Ini.Add('AddMaxHandWeight=' + IntToStr(BaseAbil.MaxHandWeight));
    end;

    for nJob := JOB_WARR to JOB_TAOS do
    begin
      Ini.Add('');
      Ini.Add('[Hero_' + IntToStr(nJob) + ']');

      Ini.Add('AutoCalcLevel1000=' + Values[g_BaseAbilConfig.HeroAbil[nJob].AutoCalcLevel1000]);

      for nLevel := Low(g_BaseAbilConfig.HeroAbil[nJob].Base) to High(g_BaseAbilConfig.HeroAbil[nJob].Base) do
      begin
        BaseAbil := @g_BaseAbilConfig.HeroAbil[nJob].Base[nLevel];

        sLevel := IntToStr(nLevel + 1);

        Ini.Add('AC1_' + sLevel + '=' + IntToStr(BaseAbil.AC1));
        Ini.Add('AC2_' + sLevel + '=' + IntToStr(BaseAbil.AC2));
        Ini.Add('MAC1_' + sLevel + '=' + IntToStr(BaseAbil.MAC1));
        Ini.Add('MAC2_' + sLevel + '=' + IntToStr(BaseAbil.MAC2));
        Ini.Add('DC1_' + sLevel + '=' + IntToStr(BaseAbil.DC1));
        Ini.Add('DC2_' + sLevel + '=' + IntToStr(BaseAbil.DC2));
        Ini.Add('MC1_' + sLevel + '=' + IntToStr(BaseAbil.MC1));
        Ini.Add('MC2_' + sLevel + '=' + IntToStr(BaseAbil.MC2));
        Ini.Add('SC1_' + sLevel + '=' + IntToStr(BaseAbil.SC1));
        Ini.Add('SC2_' + sLevel + '=' + IntToStr(BaseAbil.SC2));
        Ini.Add('MaxHP_' + sLevel + '=' + IntToStr(BaseAbil.MaxHP));
        Ini.Add('MaxMP_' + sLevel + '=' + IntToStr(BaseAbil.MaxMP));
        Ini.Add('MaxWeight_' + sLevel + '=' + IntToStr(BaseAbil.MaxWeight));
        Ini.Add('MaxWearWeight_' + sLevel + '=' + IntToStr(BaseAbil.MaxWearWeight));
        Ini.Add('MaxHandWeight_' + sLevel + '=' + IntToStr(BaseAbil.MaxHandWeight));
      end;

      Ini.Add('');

      BaseAbil := @g_BaseAbilConfig.HeroAbil[nJob].Add;
      Ini.Add('AddAC1=' + IntToStr(BaseAbil.AC1));
      Ini.Add('AddAC2=' + IntToStr(BaseAbil.AC2));
      Ini.Add('AddMAC1=' + IntToStr(BaseAbil.MAC1));
      Ini.Add('AddMAC2=' + IntToStr(BaseAbil.MAC2));
      Ini.Add('AddDC1=' + IntToStr(BaseAbil.DC1));
      Ini.Add('AddDC2=' + IntToStr(BaseAbil.DC2));
      Ini.Add('AddMC1=' + IntToStr(BaseAbil.MC1));
      Ini.Add('AddMC2=' + IntToStr(BaseAbil.MC2));
      Ini.Add('AddSC1=' + IntToStr(BaseAbil.SC1));
      Ini.Add('AddSC2=' + IntToStr(BaseAbil.SC2));
      Ini.Add('AddMaxHP=' + IntToStr(BaseAbil.MaxHP));
      Ini.Add('AddMaxMP=' + IntToStr(BaseAbil.MaxMP));
      Ini.Add('AddMaxWeight=' + IntToStr(BaseAbil.MaxWeight));
      Ini.Add('AddMaxWearWeight=' + IntToStr(BaseAbil.MaxWearWeight));
      Ini.Add('AddMaxHandWeight=' + IntToStr(BaseAbil.MaxHandWeight));
    end;

    sFileName := g_sSelfFilePath + sBaseAbilConfigFileName;
    Ini.SaveToFile(sFileName);

    btnSave.Enabled := False;
  finally
    Ini.Free;
  end;
end;

procedure TfrmViewLevel.FormCreate(Sender: TObject);
var
  I: Integer;
  S: string;
begin
  GridHumanInfo.Cells[0, 0] := '属性';
  GridHumanInfo.Cells[1, 0] := '数值';
  GridHumanInfo.Cells[0, 1] := '经验值';
  GridHumanInfo.Cells[0, 2] := '防御';
  GridHumanInfo.Cells[0, 3] := '魔防';
  GridHumanInfo.Cells[0, 4] := '攻击力';
  GridHumanInfo.Cells[0, 5] := '魔法';
  GridHumanInfo.Cells[0, 6] := '道术';
  GridHumanInfo.Cells[0, 7] := '生命值';
  GridHumanInfo.Cells[0, 8] := '魔法值';
  GridHumanInfo.Cells[0, 9] := '背包';
  GridHumanInfo.Cells[0, 10] := '负重';
  GridHumanInfo.Cells[0, 11] := '腕力';

  for I := Low(g_BaseAbilConfig.HumAbil[0].Base) to High(g_BaseAbilConfig.HumAbil[0].Base) do
  begin
    S := IntToStr(I + 1) + '级';
    lstDefLevels.Items.Add(S);
    lstCustomLevels.Items.Add(S);
  end;
end;

procedure TfrmViewLevel.RecalcHuman;
begin
  PlayObject.RecalcLevelAbilitys(True);
  PlayObject.RecalcAbilitys;
  PlayObject.HasLevelUp(0, False);

  HeroObject.RecalcLevelAbilitys(True);
  HeroObject.RecalcAbilitys;
  HeroObject.HasLevelUp(0, False);
end;

procedure TfrmViewLevel.RefView;
begin
  RecalcHuman();
  if cbbDefUserType.ItemIndex = 1 then
  begin
    GridHumanInfo.Cells[1, 1] := IntToStr(HeroObject.m_Abil.MaxExp);
    GridHumanInfo.Cells[1, 2] := IntToStr(HeroObject.m_WAbil.AC1) + '/' + IntToStr(HeroObject.m_WAbil.AC2);
    GridHumanInfo.Cells[1, 3] := IntToStr(HeroObject.m_WAbil.MAC1) + '/' + IntToStr(HeroObject.m_WAbil.MAC2);
    GridHumanInfo.Cells[1, 4] := IntToStr(HeroObject.m_WAbil.DC1) + '/' + IntToStr(HeroObject.m_WAbil.DC2);
    GridHumanInfo.Cells[1, 5] := IntToStr(HeroObject.m_WAbil.MC1) + '/' + IntToStr(HeroObject.m_WAbil.MC2);
    GridHumanInfo.Cells[1, 6] := IntToStr(HeroObject.m_WAbil.SC1) + '/' + IntToStr(HeroObject.m_WAbil.SC2);
    GridHumanInfo.Cells[1, 7] := IntToStr(HeroObject.m_WAbil.HP) + '/' + IntToStr(HeroObject.m_WAbil.MaxHP);
    GridHumanInfo.Cells[1, 8] := IntToStr(HeroObject.m_WAbil.MP) + '/' + IntToStr(HeroObject.m_WAbil.MaxMP);
    GridHumanInfo.Cells[1, 9] := IntToStr(HeroObject.m_WAbil.Weight) + '/' + IntToStr(HeroObject.m_WAbil.MaxWeight);
    GridHumanInfo.Cells[1, 10] := IntToStr(HeroObject.m_WAbil.WearWeight) + '/' + IntToStr(HeroObject.m_WAbil.MaxWearWeight);
    GridHumanInfo.Cells[1, 11] := IntToStr(HeroObject.m_WAbil.HandWeight) + '/' + IntToStr(HeroObject.m_WAbil.MaxHandWeight);
  end
  else
  begin
    GridHumanInfo.Cells[1, 1] := IntToStr(PlayObject.m_Abil.MaxExp);
    GridHumanInfo.Cells[1, 2] := IntToStr(PlayObject.m_WAbil.AC1) + '/' + IntToStr(PlayObject.m_WAbil.AC2);
    GridHumanInfo.Cells[1, 3] := IntToStr(PlayObject.m_WAbil.MAC1) + '/' + IntToStr(PlayObject.m_WAbil.MAC2);
    GridHumanInfo.Cells[1, 4] := IntToStr(PlayObject.m_WAbil.DC1) + '/' + IntToStr(PlayObject.m_WAbil.DC2);
    GridHumanInfo.Cells[1, 5] := IntToStr(PlayObject.m_WAbil.MC1) + '/' + IntToStr(PlayObject.m_WAbil.MC2);
    GridHumanInfo.Cells[1, 6] := IntToStr(PlayObject.m_WAbil.SC1) + '/' + IntToStr(PlayObject.m_WAbil.SC2);
    GridHumanInfo.Cells[1, 7] := IntToStr(PlayObject.m_WAbil.HP) + '/' + IntToStr(PlayObject.m_WAbil.MaxHP);
    GridHumanInfo.Cells[1, 8] := IntToStr(PlayObject.m_WAbil.MP) + '/' + IntToStr(PlayObject.m_WAbil.MaxMP);
    GridHumanInfo.Cells[1, 9] := IntToStr(PlayObject.m_WAbil.Weight) + '/' + IntToStr(PlayObject.m_WAbil.MaxWeight);
    GridHumanInfo.Cells[1, 10] := IntToStr(PlayObject.m_WAbil.WearWeight) + '/' + IntToStr(PlayObject.m_WAbil.MaxWearWeight);
    GridHumanInfo.Cells[1, 11] := IntToStr(PlayObject.m_WAbil.HandWeight) + '/' + IntToStr(PlayObject.m_WAbil.MaxHandWeight);
  end;
end;

procedure TfrmViewLevel.cbbDefJobChange(Sender: TObject);
begin
  PlayObject.m_btJob := cbbDefJob.ItemIndex;
  HeroObject.m_btJob := cbbDefJob.ItemIndex;
  RefView();
end;

procedure TfrmViewLevel.cbbDefUserTypeChange(Sender: TObject);
begin
  RefView;
end;

procedure TfrmViewLevel.seDefLevelChange(Sender: TObject);
begin
  if seDefLevel.Value < 1 then
    seDefLevel.Value := 1;
  PlayObject.m_Abil.Level := seDefLevel.Value;
  HeroObject.m_Abil.Level := seDefLevel.Value;
  RefView();
end;

procedure TfrmViewLevel.lstDefLevelsClick(Sender: TObject);
begin
  seDefLevel.Value := lstDefLevels.ItemIndex + 1;
end;

procedure TfrmViewLevel.lstCustomLevelsClick(Sender: TObject);
begin
  seCustomLevel.Value := lstCustomLevels.ItemIndex + 1;
end;

procedure TfrmViewLevel.seCustomLevelChange(Sender: TObject);
begin
  RefCustomView;
end;

procedure TfrmViewLevel.RefCustomView;
var
  HumBaseAbil: PHumBaseAbil;
  ActorBaseAbil: PActorBaseAbil;
  BaseAbil: PBaseAbilInfo;
  nLevel: Integer;
  OldEnabled: Boolean;
begin
  OldEnabled := btnSave.Enabled;

  if cbbCustomUserType.ItemIndex = 0 then
    HumBaseAbil := @g_BaseAbilConfig.HumAbil
  else
    HumBaseAbil := @g_BaseAbilConfig.HeroAbil;

  ActorBaseAbil := @HumBaseAbil[cbbCustomJob.ItemIndex];
  nLevel := seCustomLevel.Value - 1;
  if nLevel < 0 then
    nLevel := 0
  else if nLevel >= High(ActorBaseAbil.Base) then
    nLevel := High(ActorBaseAbil.Base);

  BaseAbil := @ActorBaseAbil.Base[nLevel];
  seAC1.Value := BaseAbil.AC1;
  seAC2.Value := BaseAbil.AC2;

  seMAC1.Value := BaseAbil.MAC1;
  seMAC2.Value := BaseAbil.MAC2;

  seDC1.Value := BaseAbil.DC1;
  seDC2.Value := BaseAbil.DC2;

  seMC1.Value := BaseAbil.MC1;
  seMC2.Value := BaseAbil.MC2;

  seSC1.Value := BaseAbil.SC1;
  seSC2.Value := BaseAbil.SC2;

  seMaxHP.Value := BaseAbil.MaxHP;
  seMaxMP.Value := BaseAbil.MaxMP;

  seMaxWeight.Value := BaseAbil.MaxWeight;
  seMaxWearWeight.Value := BaseAbil.MaxWearWeight;
  seMaxHandWeight.Value := BaseAbil.MaxHandWeight;

  if ActorBaseAbil.AutoCalcLevel1000 then
    rbCustomAutoCalc.Checked := True
  else
    rbCustomSetValue.Checked := True;

  ActorBaseAbil.AutoCalcLevel1000 := rbCustomAutoCalc.Checked;
  GroupBox4.Enabled := not rbCustomAutoCalc.Checked;
  seAddAC1.Enabled := not rbCustomAutoCalc.Checked;
  seAddAC2.Enabled := not rbCustomAutoCalc.Checked;
  seAddMAC1.Enabled := not rbCustomAutoCalc.Checked;
  seAddMAC2.Enabled := not rbCustomAutoCalc.Checked;
  seAddDC1.Enabled := not rbCustomAutoCalc.Checked;
  seAddDC2.Enabled := not rbCustomAutoCalc.Checked;
  seAddMC1.Enabled := not rbCustomAutoCalc.Checked;
  seAddMC2.Enabled := not rbCustomAutoCalc.Checked;
  seAddSC1.Enabled := not rbCustomAutoCalc.Checked;
  seAddSC2.Enabled := not rbCustomAutoCalc.Checked;
  seAddMaxHP.Enabled := not rbCustomAutoCalc.Checked;
  seAddMaxMP.Enabled := not rbCustomAutoCalc.Checked;
  seAddMaxWeight.Enabled := not rbCustomAutoCalc.Checked;
  seAddMaxWearWeight.Enabled := not rbCustomAutoCalc.Checked;
  seAddMaxHandWeight.Enabled := not rbCustomAutoCalc.Checked;

  BaseAbil := @ActorBaseAbil.Add;

  seAddAC1.Value := BaseAbil.AC1;
  seAddAC2.Value := BaseAbil.AC2;

  seAddMAC1.Value := BaseAbil.MAC1;
  seAddMAC2.Value := BaseAbil.MAC2;

  seAddDC1.Value := BaseAbil.DC1;
  seAddDC2.Value := BaseAbil.DC2;

  seAddMC1.Value := BaseAbil.MC1;
  seAddMC2.Value := BaseAbil.MC2;

  seAddSC1.Value := BaseAbil.SC1;
  seAddSC2.Value := BaseAbil.SC2;

  seAddMaxHP.Value := BaseAbil.MaxHP;
  seAddMaxMP.Value := BaseAbil.MaxMP;

  seAddMaxWeight.Value := BaseAbil.MaxWeight;
  seAddMaxWearWeight.Value := BaseAbil.MaxWearWeight;
  seAddMaxHandWeight.Value := BaseAbil.MaxHandWeight;

  btnSave.Enabled := OldEnabled;
end;

procedure TfrmViewLevel.btnInitCustomClick(Sender: TObject);
var
  nJob, nLevel: Integer;
  Obj: TSmartObject;
  BaseAbil: PBaseAbilInfo;
begin
  if Application.MessageBox('确定要初始化自定义等级属性', '询问', MB_YESNO + MB_ICONQUESTION) <> mrYes then
    Exit;

  Obj := TPlayObject.Create;
  try
    Obj.m_boDummyObject := True;

    Obj.m_Abil.Level := 1;
    Obj.m_btJob := 0;
    Obj.m_sMapName := '0';
    Obj.m_PEnvir := g_MapManager.FindMap('0');
    Obj.m_nCurrX := 330;
    Obj.m_nCurrY := 266;

    for nJob := JOB_WARR to JOB_TAOS do
    begin
      Obj.m_btJob := nJob;

      g_BaseAbilConfig.HumAbil[nJob].AutoCalcLevel1000 := True;

      for nLevel := Low(g_BaseAbilConfig.HumAbil[nJob].Base) to High(g_BaseAbilConfig.HumAbil[nJob].Base) do
      begin
        Obj.m_Abil.Level := nLevel + 1;
        Obj.RecalcLevelAbilitys(True);
        Obj.RecalcAbilitys;

        BaseAbil := @g_BaseAbilConfig.HumAbil[nJob].Base[nLevel];
        BaseAbil.AC1 := Obj.m_WAbil.AC1;
        BaseAbil.AC2 := Obj.m_WAbil.AC2;
        BaseAbil.MAC1 := Obj.m_WAbil.MAC1;
        BaseAbil.MAC2 := Obj.m_WAbil.MAC2;
        BaseAbil.DC1 := Obj.m_WAbil.DC1;
        BaseAbil.DC2 := Obj.m_WAbil.DC2;
        BaseAbil.MC1 := Obj.m_WAbil.MC1;
        BaseAbil.MC2 := Obj.m_WAbil.MC2;
        BaseAbil.SC1 := Obj.m_WAbil.SC1;
        BaseAbil.SC2 := Obj.m_WAbil.SC2;
        BaseAbil.MaxHP := Obj.m_WAbil.MaxHP;
        BaseAbil.MaxMP := Obj.m_WAbil.MaxMP;
        BaseAbil.MaxWeight := Obj.m_WAbil.MaxWeight;
        BaseAbil.MaxWearWeight := Obj.m_WAbil.MaxWearWeight;
        BaseAbil.MaxHandWeight := Obj.m_WAbil.MaxHandWeight;
      end;
    end;
  finally
    Obj.Free;
  end;

  Obj := THeroObject.Create;
  try
    Obj.m_boDummyObject := True;
    Obj.m_Abil.Level := 1;
    Obj.m_btJob := 0;
    Obj.m_sMapName := '0';
    Obj.m_PEnvir := g_MapManager.FindMap('0');
    Obj.m_nCurrX := 330;
    Obj.m_nCurrY := 266;

    for nJob := JOB_WARR to JOB_TAOS do
    begin
      Obj.m_btJob := nJob;

      g_BaseAbilConfig.HeroAbil[nJob].AutoCalcLevel1000 := True;

      for nLevel := Low(g_BaseAbilConfig.HeroAbil[nJob].Base) to High(g_BaseAbilConfig.HeroAbil[nJob].Base) do
      begin
        Obj.m_Abil.Level := nLevel + 1;
        Obj.RecalcLevelAbilitys(True);
        Obj.RecalcAbilitys;

        BaseAbil := @g_BaseAbilConfig.HeroAbil[nJob].Base[nLevel];
        BaseAbil.AC1 := Obj.m_WAbil.AC1;
        BaseAbil.AC2 := Obj.m_WAbil.AC2;
        BaseAbil.MAC1 := Obj.m_WAbil.MAC1;
        BaseAbil.MAC2 := Obj.m_WAbil.MAC2;
        BaseAbil.DC1 := Obj.m_WAbil.DC1;
        BaseAbil.DC2 := Obj.m_WAbil.DC2;
        BaseAbil.MC1 := Obj.m_WAbil.MC1;
        BaseAbil.MC2 := Obj.m_WAbil.MC2;
        BaseAbil.SC1 := Obj.m_WAbil.SC1;
        BaseAbil.SC2 := Obj.m_WAbil.SC2;
        BaseAbil.MaxHP := Obj.m_WAbil.MaxHP;
        BaseAbil.MaxMP := Obj.m_WAbil.MaxMP;
        BaseAbil.MaxWeight := Obj.m_WAbil.MaxWeight;
        BaseAbil.MaxWearWeight := Obj.m_WAbil.MaxWearWeight;
        BaseAbil.MaxHandWeight := Obj.m_WAbil.MaxHandWeight;
      end;
    end;
  finally
    Obj.Free;
  end;

  RefCustomView;
  btnSave.Enabled := True;
end;

procedure TfrmViewLevel.rbDefClick(Sender: TObject);
begin
  if rbDef.Checked then
  begin
    if not g_BaseAbilConfig.UseDefault then
    begin
      g_BaseAbilConfig.UseDefault := True;
      btnSave.Enabled := True;
    end;
  end
  else if rbCustom.Checked then
  begin
    if g_BaseAbilConfig.UseDefault then
    begin
      g_BaseAbilConfig.UseDefault := False;
      btnSave.Enabled := True;
    end;
  end;
end;

procedure TfrmViewLevel.seAC1Change(Sender: TObject);
var
  HumBaseAbil: PHumBaseAbil;
  ActorBaseAbil: PActorBaseAbil;
  BaseAbil: PBaseAbilInfo;
  nLevel: Integer;
begin
  if cbbCustomUserType.ItemIndex = 0 then
    HumBaseAbil := @g_BaseAbilConfig.HumAbil
  else
    HumBaseAbil := @g_BaseAbilConfig.HeroAbil;

  ActorBaseAbil := @HumBaseAbil[cbbCustomJob.ItemIndex];
  nLevel := seCustomLevel.Value - 1;
  if nLevel < 0 then
    nLevel := 0
  else if nLevel >= High(ActorBaseAbil.Base) then
    nLevel := High(ActorBaseAbil.Base);

  BaseAbil := @ActorBaseAbil.Base[nLevel];

  if Sender = seAC1 then
    BaseAbil.AC1 := seAC1.Value
  else if Sender = seAC2 then
    BaseAbil.AC2 := seAC2.Value
  else if Sender = seMAC1 then
    BaseAbil.MAC1 := seMAC1.Value
  else if Sender = seMAC2 then
    BaseAbil.MAC2 := seMAC2.Value
  else if Sender = seDC1 then
    BaseAbil.DC1 := seDC1.Value
  else if Sender = seDC2 then
    BaseAbil.DC2 := seDC2.Value
  else if Sender = seMC1 then
    BaseAbil.MC1 := seMC1.Value
  else if Sender = seMC2 then
    BaseAbil.MC2 := seMC2.Value
  else if Sender = seSC1 then
    BaseAbil.SC1 := seSC1.Value
  else if Sender = seSC2 then
    BaseAbil.SC2 := seSC2.Value
  else if Sender = seMaxHP then
    BaseAbil.MaxHP := seMaxHP.Value
  else if Sender = seMaxMP then
    BaseAbil.MaxMP := seMaxMP.Value
  else if Sender = seMaxWeight then
    BaseAbil.MaxWeight := seMaxWeight.Value
  else if Sender = seMaxWearWeight then
    BaseAbil.MaxWearWeight := seMaxWearWeight.Value
  else if Sender = seMaxHandWeight then
    BaseAbil.MaxHandWeight := seMaxHandWeight.Value
  else if (Sender = rbCustomAutoCalc) or (Sender = rbCustomSetValue) then
  begin
    ActorBaseAbil.AutoCalcLevel1000 := rbCustomAutoCalc.Checked;
    GroupBox4.Enabled := not rbCustomAutoCalc.Checked;
    seAddAC1.Enabled := not rbCustomAutoCalc.Checked;
    seAddAC2.Enabled := not rbCustomAutoCalc.Checked;
    seAddMAC1.Enabled := not rbCustomAutoCalc.Checked;
    seAddMAC2.Enabled := not rbCustomAutoCalc.Checked;
    seAddDC1.Enabled := not rbCustomAutoCalc.Checked;
    seAddDC2.Enabled := not rbCustomAutoCalc.Checked;
    seAddMC1.Enabled := not rbCustomAutoCalc.Checked;
    seAddMC2.Enabled := not rbCustomAutoCalc.Checked;
    seAddSC1.Enabled := not rbCustomAutoCalc.Checked;
    seAddSC2.Enabled := not rbCustomAutoCalc.Checked;
    seAddMaxHP.Enabled := not rbCustomAutoCalc.Checked;
    seAddMaxMP.Enabled := not rbCustomAutoCalc.Checked;
    seAddMaxWeight.Enabled := not rbCustomAutoCalc.Checked;
    seAddMaxWearWeight.Enabled := not rbCustomAutoCalc.Checked;
    seAddMaxHandWeight.Enabled := not rbCustomAutoCalc.Checked;
  end
  else if Sender = seAddAC1 then
    ActorBaseAbil.Add.AC1 := seAddAC1.Value
  else if Sender = seAddAC2 then
    ActorBaseAbil.Add.AC2 := seAddAC2.Value
  else if Sender = seAddMAC1 then
    ActorBaseAbil.Add.MAC1 := seAddMAC1.Value
  else if Sender = seAddMAC2 then
    ActorBaseAbil.Add.MAC2 := seAddMAC2.Value
  else if Sender = seAddDC1 then
    ActorBaseAbil.Add.DC1 := seAddDC1.Value
  else if Sender = seAddDC2 then
    ActorBaseAbil.Add.DC2 := seAddDC2.Value
  else if Sender = seAddMC1 then
    ActorBaseAbil.Add.MC1 := seAddMC1.Value
  else if Sender = seAddMC2 then
    ActorBaseAbil.Add.MC2 := seAddMC2.Value
  else if Sender = seAddSC1 then
    ActorBaseAbil.Add.SC1 := seAddSC1.Value
  else if Sender = seAddSC2 then
    ActorBaseAbil.Add.SC2 := seAddSC2.Value
  else if Sender = seAddMaxHP then
    ActorBaseAbil.Add.MaxHP := seAddMaxHP.Value
  else if Sender = seAddMaxMP then
    ActorBaseAbil.Add.MaxMP := seAddMaxMP.Value
  else if Sender = seAddMaxWeight then
    ActorBaseAbil.Add.MaxWeight := seAddMaxWeight.Value
  else if Sender = seAddMaxWearWeight then
    ActorBaseAbil.Add.MaxWearWeight := seAddMaxWearWeight.Value
  else if Sender = seAddMaxHandWeight then
    ActorBaseAbil.Add.MaxHandWeight := seAddMaxHandWeight.Value;

  btnSave.Enabled := True;
end;

end.
