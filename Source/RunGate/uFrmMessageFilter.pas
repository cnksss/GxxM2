unit uFrmMessageFilter;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, IniFiles;

type
  TFrmMessageFilter = class(TForm)
    lstFilterText: TListBox;
    Label1: TLabel;
    btnAdd: TButton;
    btnDel: TButton;
    btnOK: TButton;
    btnEdit: TButton;
    GroupBox2: TGroupBox;
    chkFilterSayMsg: TCheckBox;
    rbAllBlock: TRadioButton;
    rbSelfBolck: TRadioButton;
    rbConnClose: TRadioButton;
    rbDisMsg: TRadioButton;
    rbDisMsgorSys: TRadioButton;
    chkFilterSayTriggerScript: TCheckBox;
    Label2: TLabel;
    edtWarnSayMsg: TEdit;
    procedure lstFilterTextClick(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
    procedure btnEditClick(Sender: TObject);
    procedure lstFilterTextDblClick(Sender: TObject);
    procedure btnAddClick(Sender: TObject);
    procedure btnDelClick(Sender: TObject);
    procedure rbAllBlockClick(Sender: TObject);
    procedure chkFilterSayMsgClick(Sender: TObject);
    procedure edtWarnSayMsgChange(Sender: TObject);
    procedure chkFilterSayTriggerScriptClick(Sender: TObject);
  private
    { Private declarations }
    procedure Open;
  public
    { Public declarations }
  end;

  function ShowFrmMessageFilter(MainForm: TForm): Boolean;

implementation

uses
  GateShare;

{$R *.dfm}

function ShowFrmMessageFilter(MainForm: TForm): Boolean;
var
  FrmMessageFilter: TFrmMessageFilter;
begin
  FrmMessageFilter := TFrmMessageFilter.Create(nil);
  try
    FrmMessageFilter.Left := MainForm.Left + (MainForm.Width - FrmMessageFilter.Width) div 2;
    FrmMessageFilter.Top := MainForm.Top + (MainForm.Height - FrmMessageFilter.Height) div 2;
    FrmMessageFilter.Open;
    Result := FrmMessageFilter.ShowModal = mrOk;
  finally
    FrmMessageFilter.Free;
  end;
end;

procedure TFrmMessageFilter.Open;
var
  I: Integer;
begin
  g_WordFilterList.Lock;
  try
    lstFilterText.Clear;
    for I := 0 to g_WordFilterList.Count - 1 do
      lstFilterText.Items.Add(g_WordFilterList.strings[I]);
  finally
    g_WordFilterList.UnLock;
  end;

  btnDel.Enabled := False;
  btnEdit.Enabled := False;
  chkFilterSayMsg.Checked := g_boFilterSayMsg;
  case g_FilterSayMsgMode of
    fsmmAllBlock:     rbAllBlock.Checked := True;
    fsmmSelfBolck:    rbSelfBolck.Checked := True;
    fsmmClose:        rbConnClose.Checked := True;
    fsmmDisMsg:       rbDisMsg.Checked := True;
    fsmmDisMsgorSys:  rbDisMsgorSys.Checked := True;
  end;

  chkFilterSayMsgClick(chkFilterSayMsg);
  edtWarnSayMsg.Text := g_WarnSayMsg;

  chkFilterSayTriggerScript.Checked := g_boFilterSayTriggerScript;
end;

procedure TFrmMessageFilter.lstFilterTextClick(Sender: TObject);
begin
  if (lstFilterText.ItemIndex >= 0) and
    (lstFilterText.ItemIndex < lstFilterText.Items.Count) then
  begin
    btnDel.Enabled := True;
    btnEdit.Enabled := True;
  end;
end;

procedure TFrmMessageFilter.btnOKClick(Sender: TObject);
var
  I: Integer;
  IniFile: TIniFile;
begin
  g_WordFilterList.Lock;
  try
    g_WordFilterList.Clear;
    for I := 0 to lstFilterText.Items.Count - 1 do
      g_WordFilterList.Add(lstFilterText.Items.Strings[I]);
  finally
    g_WordFilterList.UnLock;
  end;
  g_WordFilterList.SaveToFile(g_sWordFilterFileName);

  IniFile := TIniFile.Create(g_sIniFileName);
  IniFile.WriteBool(GateClass, 'FilterSayMsg', g_boFilterSayMsg);
  IniFile.WriteInteger(GateClass, 'FilterSayMsgMode', Integer(g_FilterSayMsgMode));
  IniFile.WriteString(GateClass, 'WarnSayMsg', g_WarnSayMsg);

  IniFile.WriteBool(GateClass, 'FilterSayTriggerScript', g_boFilterSayTriggerScript);

  IniFile.Free;

  ModalResult := mrOk;
end;

procedure TFrmMessageFilter.btnEditClick(Sender: TObject);
var
  sInputText: string;
begin
  if (lstFilterText.ItemIndex >= 0) and (lstFilterText.ItemIndex < lstFilterText.Items.Count) then
  begin
    sInputText := lstFilterText.Items[lstFilterText.ItemIndex];
    if not InputQuery('增加过滤文字', '请输入新的文字:', sInputText) then Exit;
  end;

  if sInputText = '' then
  begin
    Application.MessageBox('请输入正确的文本！！！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;

  lstFilterText.Items[lstFilterText.ItemIndex] := sInputText;
end;

procedure TFrmMessageFilter.lstFilterTextDblClick(
  Sender: TObject);
begin
  btnEditClick(Sender);
end;

procedure TFrmMessageFilter.btnAddClick(Sender: TObject);
var
  sInputText: string;
begin
  //  sInputText:= InputBox('增加过滤文字', '请输入新的文字:', '');
  if not InputQuery('增加过滤文字', '请输入新的文字:', sInputText) then Exit;

  if sInputText = '' then
  begin
    Application.MessageBox('请输入正确的文本！！！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  lstFilterText.Items.Add(sInputText);
end;

procedure TFrmMessageFilter.btnDelClick(Sender: TObject);
var
  nSelectIndex: Integer;
begin
  nSelectIndex := lstFilterText.ItemIndex;
  if (nSelectIndex >= 0) and (nSelectIndex < lstFilterText.Items.Count) then
  begin
    lstFilterText.Items.Delete(nSelectIndex);
  end;

  if nSelectIndex >= lstFilterText.Items.Count then
    lstFilterText.ItemIndex := nSelectIndex - 1
  else
    lstFilterText.ItemIndex := nSelectIndex;

  if lstFilterText.ItemIndex < 0 then
  begin
    btnDel.Enabled := False;
    btnEdit.Enabled := False;
  end;
end;

procedure TFrmMessageFilter.chkFilterSayMsgClick(Sender: TObject);
begin
  g_boFilterSayMsg := chkFilterSayMsg.Checked;
  btnEdit.Enabled := False;
  btnDel.Enabled := False;
  btnOK.Enabled := True;
  lstFilterText.Enabled := g_boFilterSayMsg;
  rbAllBlock.Enabled := g_boFilterSayMsg;
  rbSelfBolck.Enabled := g_boFilterSayMsg;
  rbConnClose.Enabled := g_boFilterSayMsg;
  rbDisMsg.Enabled := g_boFilterSayMsg;
  rbDisMsgorSys.Enabled := g_boFilterSayMsg;
  edtWarnSayMsg.Enabled := g_boFilterSayMsg;
  btnAdd.Enabled := g_boFilterSayMsg;
end;

procedure TFrmMessageFilter.rbAllBlockClick(Sender: TObject);
var
  RadioButton: TRadioButton;
begin
  RadioButton := Sender as TRadioButton;
  g_FilterSayMsgMode := TFilterSayMsgMode(RadioButton.Tag);
end;

procedure TFrmMessageFilter.edtWarnSayMsgChange(Sender: TObject);
begin
  g_WarnSayMsg := edtWarnSayMsg.Text;
end;

procedure TFrmMessageFilter.chkFilterSayTriggerScriptClick(
  Sender: TObject);
begin
  g_boFilterSayTriggerScript := chkFilterSayTriggerScript.Checked;
end;

end.
