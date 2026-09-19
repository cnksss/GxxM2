unit Ranking;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, ComCtrls, IniFiles, uFrmMain, DBShare, Grobal2,
  ExtCtrls, RoleDB;

type
  TFrmRankingDlg = class(TForm)
    GroupBox1: TGroupBox;
    CheckBoxAutoRefRanking: TCheckBox;
    Label1: TLabel;
    Label2: TLabel;
    EditMinLevel: TSpinEdit;
    EditMaxLevel: TSpinEdit;
    Label3: TLabel;
    Label4: TLabel;
    RadioButton1: TRadioButton;
    RadioButton2: TRadioButton;
    EditTime: TSpinEdit;
    EditHour: TSpinEdit;
    Label5: TLabel;
    Label6: TLabel;
    EditMinute1: TSpinEdit;
    EditMinute2: TSpinEdit;
    Label7: TLabel;
    Label8: TLabel;
    ButtonSave: TButton;
    ButtonRefRanking: TButton;
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    TabSheet3: TTabSheet;
    PageControl2: TPageControl;
    TabSheet4: TTabSheet;
    TabSheet5: TTabSheet;
    TabSheet6: TTabSheet;
    TabSheet10: TTabSheet;
    PageControl3: TPageControl;
    TabSheet7: TTabSheet;
    TabSheet8: TTabSheet;
    TabSheet9: TTabSheet;
    TabSheet11: TTabSheet;
    ListViewHum: TListView;
    ListViewWarrior: TListView;
    ListViewWizzard: TListView;
    ListViewMonk: TListView;
    ListViewHero: TListView;
    ListViewHeroWarrior: TListView;
    ListViewHeroWizzard: TListView;
    ListViewHeroMonk: TListView;
    ListViewMaster: TListView;
    Timer: TTimer;
    lbl1: TLabel;
    seRankingCount: TSpinEdit;
    procedure ButtonRefRankingClick(Sender: TObject);
    procedure ButtonSaveClick(Sender: TObject);
    procedure CheckBoxAutoRefRankingClick(Sender: TObject);
    procedure EditMinLevelChange(Sender: TObject);
    procedure EditMaxLevelChange(Sender: TObject);
    procedure EditTimeChange(Sender: TObject);
    procedure EditHourChange(Sender: TObject);
    procedure EditMinute1Change(Sender: TObject);
    procedure EditMinute2Change(Sender: TObject);
    procedure RadioButton1Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure TimerTimer(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure seRankingCountChange(Sender: TObject);
  private
    m_boRefRanking: Boolean;
    procedure RefRanking;
  public
    { Public declarations }
    procedure Open;
  end;

var
  FrmRankingDlg: TFrmRankingDlg;

implementation

{$R *.dfm}

procedure TFrmRankingDlg.Open;
begin
  m_boRefRanking := False;
  CheckBoxAutoRefRanking.Checked := g_boAutoRefRanking;
  seRankingCount.Value := g_nRankingCount;
  EditMinLevel.Value := g_nRankingMinLevel;
  EditMaxLevel.Value := g_nRankingMaxLevel;
  EditTime.Value := g_nRefRankingHour1;
  EditHour.Value := g_nRefRankingHour2;
  EditMinute1.Value := g_nRefRankingMinute1;
  EditMinute2.Value := g_nRefRankingMinute2;
  if g_nAutoRefRankingType = 0 then RadioButton1.Checked := True;
  if g_nAutoRefRankingType = 1 then RadioButton2.Checked := True;
  //RadioButton2.Checked:= Boolean(g_nAutoRefRankingType);
  ButtonSave.Enabled := False;
  Timer.Enabled := True;

  Self.ShowModal;
end;

procedure TFrmRankingDlg.RefRanking;
var
  I: Integer;
  ListItem: TListItem;
  RankData: PTRoleRankData;
begin
  if g_boRefRanking or m_boRefRanking then Exit;

  ListViewHum.Clear;
  ListViewWarrior.Clear;
  ListViewWizzard.Clear;
  ListViewMonk.Clear;
  ListViewHero.Clear;
  ListViewHeroWarrior.Clear;
  ListViewHeroWizzard.Clear;
  ListViewHeroMonk.Clear;
  ListViewMaster.Clear;

  m_boRefRanking := True;
  try
    for I := 0 to g_HumanRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_HumanRankList.Items[I];
      ListItem := ListViewHum.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;
    for I := 0 to g_WarriorRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_WarriorRankList.Items[I];
      ListItem := ListViewWarrior.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;
    for I := 0 to g_WizardRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_WizardRankList.Items[I];
      ListItem := ListViewWizzard.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;
    for I := 0 to g_TaoistRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_TaoistRankList.Items[I];
      ListItem := ListViewMonk.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;

    for I := 0 to g_HeroRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_HeroRankList.Items[I];
      ListItem := ListViewHero.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HeroName);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;

    for I := 0 to g_HeroWarriorRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_HeroWarriorRankList.Items[I];
      ListItem := ListViewHeroWarrior.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HeroName);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;

    for I := 0 to g_HeroWizardRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_HeroWizardRankList.Items[I];
      ListItem := ListViewHeroWizzard.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HeroName);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;

    for I := 0 to g_HeroTaoistRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_HeroTaoistRankList.Items[I];
      ListItem := ListViewHeroMonk.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HeroName);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.Level));
    end;

    for I := 0 to g_MasterRankList.Count - 1 do
    begin
      if I mod 100 = 0 then Application.ProcessMessages;
      RankData := g_MasterRankList.Items[I];
      ListItem := ListViewMaster.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(RankData.HumanName);
      ListItem.SubItems.Add(IntToStr(RankData.MasterCount));
    end;

  finally
    m_boRefRanking := False;
  end;
end;

procedure TFrmRankingDlg.ButtonRefRankingClick(Sender: TObject);
begin
  if g_boRefRanking then Exit;
  
  ButtonRefRanking.Enabled := False;
  g_dwAutoRefRankingTick := GetTickCount;
  RankingEngine.RefRanking;
  RefRanking;
  ButtonRefRanking.Enabled := True;
end;

procedure TFrmRankingDlg.ButtonSaveClick(Sender: TObject);
var
  Conf: TIniFile;
begin
  Conf := TIniFile.Create(g_sConfFileName);
  if Conf <> nil then
  begin
    Conf.WriteBool('Setup', 'AutoRefRanking', g_boAutoRefRanking);
    Conf.WriteInteger('Setup', 'RankingCount', g_nRankingCount);
    Conf.WriteInteger('Setup', 'RankingMinLevel', g_nRankingMinLevel);
    Conf.WriteInteger('Setup', 'RankingMaxLevel', g_nRankingMaxLevel);
    Conf.WriteInteger('Setup', 'RefRankingHour1', g_nRefRankingHour1);
    Conf.WriteInteger('Setup', 'RefRankingHour2', g_nRefRankingHour2);

    Conf.WriteInteger('Setup', 'RefRankingMinute1', g_nRefRankingMinute1);
    Conf.WriteInteger('Setup', 'RefRankingMinute2', g_nRefRankingMinute2);

    Conf.WriteInteger('Setup', 'AutoRefRankingType', g_nAutoRefRankingType);
  end;
  Conf.Free;
  ButtonSave.Enabled := False;
end;

procedure TFrmRankingDlg.CheckBoxAutoRefRankingClick(Sender: TObject);
begin
  g_boAutoRefRanking := CheckBoxAutoRefRanking.Checked;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.EditMinLevelChange(Sender: TObject);
begin
  g_nRankingMinLevel := EditMinLevel.Value;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.EditMaxLevelChange(Sender: TObject);
begin
  g_nRankingMaxLevel := EditMaxLevel.Value;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.EditTimeChange(Sender: TObject);
begin
  g_nRefRankingHour1 := EditTime.Value;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.EditHourChange(Sender: TObject);
begin
  g_nRefRankingHour2 := EditHour.Value;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.EditMinute1Change(Sender: TObject);
begin
  g_nRefRankingMinute1 := EditMinute1.Value;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.EditMinute2Change(Sender: TObject);
begin
  g_nRefRankingMinute2 := EditMinute2.Value;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.RadioButton1Click(Sender: TObject);
begin
  if RadioButton1.Checked then
    g_nAutoRefRankingType := 0
  else
    g_nAutoRefRankingType := 1;
  ButtonSave.Enabled := True;
end;

procedure TFrmRankingDlg.FormCreate(Sender: TObject);
begin
  PageControl1.ActivePageIndex := 0;
  PageControl2.ActivePageIndex := 0;
end;

procedure TFrmRankingDlg.TimerTimer(Sender: TObject);
begin
  Timer.Enabled := False;
  RefRanking;
end;

procedure TFrmRankingDlg.FormCloseQuery(Sender: TObject;
  var CanClose: Boolean);
begin
  if m_boRefRanking then CanClose := False;
end;

procedure TFrmRankingDlg.seRankingCountChange(Sender: TObject);
begin
  g_nRankingCount := seRankingCount.Value;
  ButtonSave.Enabled := True;
end;

end.
