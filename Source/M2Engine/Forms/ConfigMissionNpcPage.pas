unit ConfigMissionNpcPage;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls;

type
  TFrmMissionNpcPageEditDlg = class(TForm)
    GroupBox: TGroupBox;
    ListBoxMissionPageCaptionList: TListBox;
    ButtonMissionNpcAdd: TButton;
    ButtonEnablePickUpDelete: TButton;
    ButtonEnablePickUpSave: TButton;
    EditMissionPage: TEdit;
    ButtonSendMissionNpc: TButton;
    ButtonUp: TButton;
    ButtonDown: TButton;
    procedure ListBoxMissionPageCaptionListClick(Sender: TObject);
    procedure ButtonEnablePickUpDeleteClick(Sender: TObject);
    procedure ButtonMissionNpcAddClick(Sender: TObject);
    procedure ButtonEnablePickUpSaveClick(Sender: TObject);
    procedure ButtonUpClick(Sender: TObject);
    procedure ButtonDownClick(Sender: TObject);
    procedure ButtonSendMissionNpcClick(Sender: TObject);
  private
    { Private declarations }
  public
    procedure Open;
  end;

var
  FrmMissionNpcPageEditDlg: TFrmMissionNpcPageEditDlg;

implementation

uses
  M2Share;
{$R *.dfm}

procedure TFrmMissionNpcPageEditDlg.Open;
begin
  ListBoxMissionPageCaptionList.Items.AddStrings(g_MissionPageCaptionList);
  ShowModal();
end;

procedure TFrmMissionNpcPageEditDlg.ListBoxMissionPageCaptionListClick(Sender: TObject);
begin
  if ListBoxMissionPageCaptionList.ItemIndex >= 0 then
  begin
    EditMissionPage.Text := ListBoxMissionPageCaptionList.Items[ListBoxMissionPageCaptionList.ItemIndex];
    ButtonEnablePickUpDelete.Enabled := True;
  end
  else
    ButtonEnablePickUpDelete.Enabled := False;
end;

procedure TFrmMissionNpcPageEditDlg.ButtonEnablePickUpDeleteClick(Sender: TObject);
begin
  if ListBoxMissionPageCaptionList.ItemIndex >= 0 then
  begin
    ListBoxMissionPageCaptionList.DeleteSelected;
    ButtonEnablePickUpDelete.Enabled := False;
    ButtonEnablePickUpSave.Enabled := True;
  end;
end;

procedure TFrmMissionNpcPageEditDlg.ButtonMissionNpcAddClick(Sender: TObject);
var
  sCaption: string;
begin
  sCaption := Trim(EditMissionPage.Text);
  if sCaption = '' then
  begin
    Application.MessageBox('请输入页面名称！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  if GetMissionPageCaption(sCaption) then
  begin
    Application.MessageBox('此页面名称已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  ListBoxMissionPageCaptionList.Items.Add(sCaption);
  ButtonEnablePickUpSave.Enabled := True;
end;

procedure TFrmMissionNpcPageEditDlg.ButtonEnablePickUpSaveClick(Sender: TObject);
begin
  g_MissionPageCaptionList.Clear;
  g_MissionPageCaptionList.AddStrings(ListBoxMissionPageCaptionList.Items);
  SaveMissionPageCaptionList;
  ButtonEnablePickUpSave.Enabled := False;
end;

procedure TFrmMissionNpcPageEditDlg.ButtonUpClick(Sender: TObject);
var
  sCaption: string;
  ItemIndex: Integer;
begin
  ItemIndex := ListBoxMissionPageCaptionList.ItemIndex;
  if ItemIndex > 0 then
  begin
    sCaption := ListBoxMissionPageCaptionList.Items[ItemIndex];
    ListBoxMissionPageCaptionList.DeleteSelected;
    ListBoxMissionPageCaptionList.Items.Insert(ItemIndex - 1, sCaption);
    ListBoxMissionPageCaptionList.ItemIndex := ItemIndex - 1;
    ButtonEnablePickUpSave.Enabled := True;
  end;
end;

procedure TFrmMissionNpcPageEditDlg.ButtonDownClick(Sender: TObject);
var
  sCaption: string;
  ItemIndex: Integer;
begin
  ItemIndex := ListBoxMissionPageCaptionList.ItemIndex;
  if (ItemIndex >= 0) and (ItemIndex < ListBoxMissionPageCaptionList.Count - 1) then
  begin
    sCaption := ListBoxMissionPageCaptionList.Items[ItemIndex];
    ListBoxMissionPageCaptionList.DeleteSelected;
    ListBoxMissionPageCaptionList.Items.Insert(ItemIndex + 1, sCaption);
    ListBoxMissionPageCaptionList.ItemIndex := ItemIndex + 1;
    ButtonEnablePickUpSave.Enabled := True;
  end;
end;

procedure TFrmMissionNpcPageEditDlg.ButtonSendMissionNpcClick(Sender: TObject);
begin
  UserEngine.SendMissionNpc();
end;

end.

