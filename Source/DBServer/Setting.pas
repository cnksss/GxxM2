unit Setting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, IniFiles, Grobal2, Spin, ComCtrls;

type
  TFrmSetting = class(TForm)
    ButtonOK: TButton;
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    CheckBoxDenyChrName: TCheckBox;
    CheckBoxCanDeleteHuman: TCheckBox;
    EditCreateChrNameCount: TSpinEdit;
    CheckBoxCanCreateHuman: TCheckBox;
    CheckBoxCanGetBackDeleteHuman: TCheckBox;
    EditCanDeleteHumanLowLevel: TSpinEdit;
    Label2: TLabel;
    CheckBoxForbidNumberName: TCheckBox;
    CheckBoxForbidLetterName: TCheckBox;
    Label3: TLabel;
    GroupBox2: TGroupBox;
    Label4: TLabel;
    CheckBoxRanking: TCheckBox;
    MemoFilterNewHumanName: TMemo;
    MemoFilterRankingName: TMemo;
    chkUseActiveRunGage: TCheckBox;
    Button1: TButton;
    chkShowBlockIPLog: TCheckBox;
    procedure ButtonOKClick(Sender: TObject);
  private
    procedure Open();
  public
    { Public declarations }
  end;

  function ShowFrmSetting: Boolean;
  
implementation

uses DBShare;

{$R *.dfm}

function ShowFrmSetting: Boolean;
var
  FrmSetting: TFrmSetting;
begin
  FrmSetting := TFrmSetting.Create(nil);
  try
    FrmSetting.Open;
    Result := FrmSetting.ShowModal = mrOK;
  finally
    FrmSetting.Free;
  end;
end;

procedure TFrmSetting.Open();
begin
  CheckBoxDenyChrName.Checked := g_boDenyChrName;
  CheckBoxCanCreateHuman.Checked := g_boCanCreateHuman;
  CheckBoxCanDeleteHuman.Checked := g_boCanDeleteHuman;
  CheckBoxCanGetBackDeleteHuman.Checked := g_boCanGetBackDeleteHuman;
  CheckBoxForbidNumberName.Checked := g_boForbidNumberName;
  CheckBoxForbidLetterName.Checked := g_boForbidLetterName;

  CheckBoxRanking.Checked := g_boCanRanking;

  MemoFilterNewHumanName.Clear;
  MemoFilterRankingName.Clear;
  MemoFilterNewHumanName.Lines.AddStrings(g_FilterNewHumanNameTextList);
  MemoFilterRankingName.Lines.AddStrings(g_FilterRankingNameTextList);

  EditCanDeleteHumanLowLevel.Value := g_nCanDeleteHumanLowLevel;
  EditCreateChrNameCount.Value := g_nCreateChrNameCount;

  chkUseActiveRunGage.Checked := g_boUseActiveRunGage;
  chkShowBlockIPLog.Checked := g_boShowBlockIPLog;
end;

procedure TFrmSetting.ButtonOKClick(Sender: TObject);
var
  Conf: TIniFile;
begin
  g_boCanCreateHuman := CheckBoxCanCreateHuman.Checked;
  g_boCanDeleteHuman := CheckBoxCanDeleteHuman.Checked;
  g_nCanDeleteHumanLowLevel := EditCanDeleteHumanLowLevel.Value;
  g_boCanGetBackDeleteHuman := CheckBoxCanGetBackDeleteHuman.Checked;
  g_boDenyChrName := CheckBoxDenyChrName.Checked;
  g_boForbidNumberName := CheckBoxForbidNumberName.Checked;
  g_boForbidLetterName := CheckBoxForbidLetterName.Checked;
  g_nCreateChrNameCount := EditCreateChrNameCount.Value;
  g_FilterNewHumanNameTextList.Text := MemoFilterNewHumanName.Text;

  g_boUseActiveRunGage := chkUseActiveRunGage.Checked;
  g_boCanRanking := CheckBoxRanking.Checked;
  g_FilterRankingNameTextList.Text := MemoFilterRankingName.Text;

  g_boShowBlockIPLog := chkShowBlockIPLog.Checked;
  
  Conf := TIniFile.Create(g_sConfFileName);
  if Conf <> nil then
  begin
    Conf.WriteBool('Setup', 'CanCreateHuman', g_boCanCreateHuman);                                  //允许建立新人物
    Conf.WriteBool('Setup', 'CanDeleteHuman', g_boCanDeleteHuman);                                  //允许删除人物
    Conf.WriteBool('Setup', 'CanGetBackDeleteHuman', g_boCanGetBackDeleteHuman);                    //允许找回删除的人物
    Conf.WriteInteger('Setup', 'CanDeleteHumanLowLevel', g_nCanDeleteHumanLowLevel);                //以上级别不允许被删除
    Conf.WriteBool('Setup', 'ForbidNumberName', g_boForbidNumberName);                              //禁止建立包含数字的人物名
    Conf.WriteBool('Setup', 'ForbidLetterName', g_boForbidLetterName);                              //禁止建立全英文人物名

    Conf.WriteBool('Setup', 'DenyChrName', g_boDenyChrName);
    Conf.WriteBool('Setup', 'CanRanking', g_boCanRanking);
    Conf.WriteInteger('Setup', 'CreateChrNameCount', g_nCreateChrNameCount);
    conf.WriteBool('Setup', 'UseActiveRunGage', g_boUseActiveRunGage);
    conf.WriteBool('Setup', 'ShowBlockIPLog', g_boShowBlockIPLog);

    Conf.Free;
  end;

  try
    g_FilterNewHumanNameTextList.SaveToFile(g_sFilePath + 'FilterNewHumanNameString.txt');
  except

  end;
  try
    g_FilterRankingNameTextList.SaveToFile(g_sFilePath + 'FilterRankingNameString.txt');
  except

  end;

  ModalResult := mrOK;
end;

end.
