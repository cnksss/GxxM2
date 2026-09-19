unit uFrmHeroMagicCondition;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, SpinEditEx, uCustomHeroMagic;

type
  TFrmHeroMagicCondition = class(TForm)
    grp1: TGroupBox;
    chkHeroLevel: TCheckBox;
    cbbHeroLevelCompareSymbol: TComboBox;
    edtHeroLevelCompareValue: TSpinEditLongWord;
    chkHeroHP: TCheckBox;
    edtHeroHPCompareValue: TSpinEditLongWord;
    cbbHeroHPCompareType: TComboBox;
    cbbHeroHPCompareSymbol: TComboBox;
    cbbHeroLevelCompareType: TComboBox;
    chkHeroMP: TCheckBox;
    edtHeroMPCompareValue: TSpinEditLongWord;
    cbbHeroMPCompareType: TComboBox;
    cbbHeroMPCompareSymbol: TComboBox;
    chkTargetHP: TCheckBox;
    edtTargetHPCompareValue: TSpinEditLongWord;
    cbbTargetHPCompareType: TComboBox;
    cbbTargetHPCompareSymbol: TComboBox;
    chkTargetMP: TCheckBox;
    edtTargetMPCompareValue: TSpinEditLongWord;
    cbbTargetMPCompareType: TComboBox;
    cbbTargetMPCompareSymbol: TComboBox;
    grp2: TGroupBox;
    chkNoPoisonDamageArmor: TCheckBox;
    chkNoPoisonDecHealth: TCheckBox;
    chkNoPoisoning: TCheckBox;
    chkNoPoisonStone: TCheckBox;
    chkNoFrozen: TCheckBox;
    chkNoForeverFrozen: TCheckBox;
    chkNoCobwebWinding: TCheckBox;
    grp3: TGroupBox;
    chkFriendCount: TCheckBox;
    edtFriendCheckValue: TSpinEditLongWord;
    btnOK: TButton;
    chkStraightLineCheck: TCheckBox;
    chkPoisonDamageArmor: TCheckBox;
    chkPoisonDecHealth: TCheckBox;
    chkPoisoning: TCheckBox;
    chkPoisonStone: TCheckBox;
    chkFrozen: TCheckBox;
    chkForeverFrozen: TCheckBox;
    chkCobwebWinding: TCheckBox;
    edtFriendCheckRange: TSpinEditLongWord;
    lbl1: TLabel;
    Label1: TLabel;
    chkEnemyCount: TCheckBox;
    edtEnemyCheckValue: TSpinEditLongWord;
    edtEnemyCheckRange: TSpinEditLongWord;
    procedure btnOKClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure cbbHeroLevelCompareTypeChange(Sender: TObject);
  private
    { Private declarations }
    FCondition: PHeroMagicUseCondition;
    procedure DoOpen;
  public
    { Public declarations }
  end;

  function ShowFrmHeroMagicCondition(var Condition: THeroMagicUseCondition): Boolean;

implementation

{$R *.dfm}

function ShowFrmHeroMagicCondition(var Condition: THeroMagicUseCondition): Boolean;
var
  FrmHeroMagicCondition: TFrmHeroMagicCondition;
begin
  FrmHeroMagicCondition := TFrmHeroMagicCondition.Create(nil);
  try
    FrmHeroMagicCondition.FCondition := @Condition;
    FrmHeroMagicCondition.DoOpen;
    Result := FrmHeroMagicCondition.ShowModal = mrOK;
  finally
    FrmHeroMagicCondition.Free;
  end;
end;

{ TFrmHeroMagicCondition }

procedure TFrmHeroMagicCondition.DoOpen;
begin
  chkHeroLevel.Checked := FCondition.HeroLevelCheck.boChecked;
  cbbHeroLevelCompareSymbol.ItemIndex := Integer(FCondition.HeroLevelCheck.CompareSymbol);
  cbbHeroLevelCompareType.ItemIndex := Integer(FCondition.HeroLevelCheck.CompareType);
  edtHeroLevelCompareValue.Value := FCondition.HeroLevelCheck.CompareValue;

  edtHeroLevelCompareValue.Visible := THeroLevelCompareType(cbbHeroLevelCompareType.ItemIndex) = hlctLevelNumber;

  chkHeroHP.Checked := FCondition.HeroHPCheck.boChecked;
  cbbHeroHPCompareSymbol.ItemIndex := Integer(FCondition.HeroHPCheck.CompareSymbol);
  cbbHeroHPCompareType.ItemIndex := Integer(FCondition.HeroHPCheck.CompareType);
  edtHeroHPCompareValue.Value := FCondition.HeroHPCheck.CompareValue;

  chkHeroMP.Checked := FCondition.HeroMPCheck.boChecked;
  cbbHeroMPCompareSymbol.ItemIndex := Integer(FCondition.HeroMPCheck.CompareSymbol);
  cbbHeroMPCompareType.ItemIndex := Integer(FCondition.HeroMPCheck.CompareType);
  edtHeroMPCompareValue.Value := FCondition.HeroMPCheck.CompareValue;

  chkTargetHP.Checked := FCondition.TargetHPCheck.boChecked;
  cbbTargetHPCompareSymbol.ItemIndex := Integer(FCondition.TargetHPCheck.CompareSymbol);
  cbbTargetHPCompareType.ItemIndex := Integer(FCondition.TargetHPCheck.CompareType);
  edtTargetHPCompareValue.Value := FCondition.TargetHPCheck.CompareValue;

  chkTargetMP.Checked := FCondition.TargetMPCheck.boChecked;
  cbbTargetMPCompareSymbol.ItemIndex := Integer(FCondition.TargetMPCheck.CompareSymbol);
  cbbTargetMPCompareType.ItemIndex := Integer(FCondition.TargetMPCheck.CompareType);
  edtTargetMPCompareValue.Value := FCondition.TargetMPCheck.CompareValue;

  chkPoisonDamageArmor.Checked := FCondition.TargetStatusCheck.boPoisonDamageArmor;
  chkPoisonDecHealth.Checked := FCondition.TargetStatusCheck.boPoisonDecHealth;
  chkPoisoning.Checked := FCondition.TargetStatusCheck.boPoisoning;
  chkPoisonStone.Checked := FCondition.TargetStatusCheck.boPoisonStone;
  chkFrozen.Checked := FCondition.TargetStatusCheck.boFrozen;
  chkForeverFrozen.Checked := FCondition.TargetStatusCheck.boForeverFrozen;
  chkCobwebWinding.Checked := FCondition.TargetStatusCheck.boCobwebWinding;

  chkNoPoisonDamageArmor.Checked := FCondition.TargetStatusCheck.boUnPoisonDamageArmor;
  chkNoPoisonDecHealth.Checked := FCondition.TargetStatusCheck.boUnPoisonDecHealth;
  chkNoPoisoning.Checked := FCondition.TargetStatusCheck.boUnPoisoning;
  chkNoPoisonStone.Checked := FCondition.TargetStatusCheck.boUnPoisonStone;
  chkNoFrozen.Checked := FCondition.TargetStatusCheck.boUnFrozen;
  chkNoForeverFrozen.Checked := FCondition.TargetStatusCheck.boUnForeverFrozen;
  chkNoCobwebWinding.Checked := FCondition.TargetStatusCheck.boUnCobwebWinding;

  chkFriendCount.Checked := FCondition.FriendCountCheck.boChecked;
  edtFriendCheckRange.Value := FCondition.FriendCountCheck.nCheckRange;
  edtFriendCheckValue.Value := FCondition.FriendCountCheck.nCheckValue;

  chkEnemyCount.Checked := FCondition.EnemyCountCheck.boChecked;
  edtEnemyCheckRange.Value := FCondition.EnemyCountCheck.nCheckRange;
  edtEnemyCheckValue.Value := FCondition.EnemyCountCheck.nCheckValue;

  chkStraightLineCheck.Checked := FCondition.boStraightLineCheck;
end;

procedure TFrmHeroMagicCondition.btnOKClick(Sender: TObject);
begin
  FCondition.HeroLevelCheck.boChecked := chkHeroLevel.Checked;
  FCondition.HeroLevelCheck.CompareSymbol := TCompareSymbol(cbbHeroLevelCompareSymbol.ItemIndex);
  FCondition.HeroLevelCheck.CompareType := THeroLevelCompareType(cbbHeroLevelCompareType.ItemIndex);
  FCondition.HeroLevelCheck.CompareValue := edtHeroLevelCompareValue.Value;

  FCondition.HeroHPCheck.boChecked := chkHeroHP.Checked;
  FCondition.HeroHPCheck.CompareSymbol := TCompareSymbol(cbbHeroHPCompareSymbol.ItemIndex);
  FCondition.HeroHPCheck.CompareType := THeroHPCompareType(cbbHeroHPCompareType.ItemIndex);
  FCondition.HeroHPCheck.CompareValue := edtHeroHPCompareValue.Value;

  FCondition.HeroMPCheck.boChecked := chkHeroMP.Checked;
  FCondition.HeroMPCheck.CompareSymbol := TCompareSymbol(cbbHeroMPCompareSymbol.ItemIndex);
  FCondition.HeroMPCheck.CompareType := THeroHPCompareType(cbbHeroMPCompareType.ItemIndex);
  FCondition.HeroMPCheck.CompareValue := edtHeroMPCompareValue.Value;

  FCondition.TargetHPCheck.boChecked := chkTargetHP.Checked;
  FCondition.TargetHPCheck.CompareSymbol := TCompareSymbol(cbbTargetHPCompareSymbol.ItemIndex);
  FCondition.TargetHPCheck.CompareType := THeroHPCompareType(cbbTargetHPCompareType.ItemIndex);
  FCondition.TargetHPCheck.CompareValue := edtTargetHPCompareValue.Value;

  FCondition.TargetMPCheck.boChecked := chkTargetMP.Checked;
  FCondition.TargetMPCheck.CompareSymbol := TCompareSymbol(cbbTargetMPCompareSymbol.ItemIndex);
  FCondition.TargetMPCheck.CompareType := THeroHPCompareType(cbbTargetMPCompareType.ItemIndex);
  FCondition.TargetMPCheck.CompareValue := edtTargetMPCompareValue.Value;

  FCondition.TargetStatusCheck.boPoisonDamageArmor := chkPoisonDamageArmor.Checked;
  FCondition.TargetStatusCheck.boPoisonDecHealth := chkPoisonDecHealth.Checked;
  FCondition.TargetStatusCheck.boPoisoning := chkPoisoning.Checked;
  FCondition.TargetStatusCheck.boPoisonStone := chkPoisonStone.Checked;
  FCondition.TargetStatusCheck.boFrozen := chkFrozen.Checked;
  FCondition.TargetStatusCheck.boForeverFrozen := chkForeverFrozen.Checked;
  FCondition.TargetStatusCheck.boCobwebWinding := chkCobwebWinding.Checked;

  FCondition.TargetStatusCheck.boUnPoisonDamageArmor := chkNoPoisonDamageArmor.Checked;
  FCondition.TargetStatusCheck.boUnPoisonDecHealth := chkNoPoisonDecHealth.Checked;
  FCondition.TargetStatusCheck.boUnPoisoning := chkNoPoisoning.Checked;
  FCondition.TargetStatusCheck.boUnPoisonStone := chkNoPoisonStone.Checked;
  FCondition.TargetStatusCheck.boUnFrozen := chkNoFrozen.Checked;
  FCondition.TargetStatusCheck.boUnForeverFrozen := chkNoForeverFrozen.Checked;
  FCondition.TargetStatusCheck.boUnCobwebWinding := chkNoCobwebWinding.Checked;

  FCondition.FriendCountCheck.boChecked := chkFriendCount.Checked;
  FCondition.FriendCountCheck.nCheckRange := edtFriendCheckRange.Value;
  FCondition.FriendCountCheck.nCheckValue := edtFriendCheckValue.Value;

  FCondition.EnemyCountCheck.boChecked := chkEnemyCount.Checked;
  FCondition.EnemyCountCheck.nCheckRange := edtEnemyCheckRange.Value;
  FCondition.EnemyCountCheck.nCheckValue := edtEnemyCheckValue.Value;

  FCondition.boStraightLineCheck := chkStraightLineCheck.Checked;

  ModalResult := mrOK;
end;

procedure TFrmHeroMagicCondition.FormCreate(Sender: TObject);
var
  CompareSymbol: TCompareSymbol;
  LevelCompareType: THeroLevelCompareType;
  HPCompareType: THeroHPCompareType;
begin
  cbbHeroLevelCompareSymbol.Clear;
  cbbHeroHPCompareSymbol.Clear;
  cbbHeroMPCompareSymbol.Clear;
  cbbTargetHPCompareSymbol.Clear;
  cbbTargetMPCompareSymbol.Clear;

  for CompareSymbol := Low(TCompareSymbol) to High(TCompareSymbol) do
  begin
    cbbHeroLevelCompareSymbol.Items.Add(TCompareSymbolNames[CompareSymbol]);
    cbbHeroHPCompareSymbol.Items.Add(TCompareSymbolNames[CompareSymbol]);
    cbbHeroMPCompareSymbol.Items.Add(TCompareSymbolNames[CompareSymbol]);
    cbbTargetHPCompareSymbol.Items.Add(TCompareSymbolNames[CompareSymbol]);
    cbbTargetMPCompareSymbol.Items.Add(TCompareSymbolNames[CompareSymbol]);
  end;

  cbbHeroLevelCompareType.Clear;
  for LevelCompareType := Low(THeroLevelCompareType) to High(THeroLevelCompareType) do
  begin
    cbbHeroLevelCompareType.Items.Add(THeroLevelCompareTypeNames[LevelCompareType]);
  end;

  cbbHeroHPCompareType.Clear;
  cbbHeroMPCompareType.Clear;
  cbbTargetHPCompareType.Clear;
  cbbTargetMPCompareType.Clear;

  for HPCompareType := Low(THeroHPCompareType) to High(THeroHPCompareType) do
  begin
    cbbHeroHPCompareType.Items.Add(THeroHPCompareTypeNames[HPCompareType]);
    cbbHeroMPCompareType.Items.Add(THeroHPCompareTypeNames[HPCompareType]);
    cbbTargetHPCompareType.Items.Add(THeroHPCompareTypeNames[HPCompareType]);
    cbbTargetMPCompareType.Items.Add(THeroHPCompareTypeNames[HPCompareType]);
  end;
end;

procedure TFrmHeroMagicCondition.cbbHeroLevelCompareTypeChange(
  Sender: TObject);
begin
  edtHeroLevelCompareValue.Visible := THeroLevelCompareType(cbbHeroLevelCompareType.ItemIndex) = hlctLevelNumber;
end;

end.
