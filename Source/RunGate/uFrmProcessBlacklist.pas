unit uFrmProcessBlacklist;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ComCtrls, GateShare, uFrmAddProcessBlack, Menus;

type
  TFrmProcessBlacklist = class(TForm)
    lvProcessBlacklist: TListView;
    lbl1: TLabel;
    cbbSearchField: TComboBox;
    lbl2: TLabel;
    edtSearchText: TEdit;
    btnSearch: TButton;
    btnSearchNext: TButton;
    lbl3: TLabel;
    btnAdd: TButton;
    pmDelete: TPopupMenu;
    mniDelete: TMenuItem;
    procedure btnSearchClick(Sender: TObject);
    procedure btnSearchNextClick(Sender: TObject);
    procedure edtSearchTextKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure btnAddClick(Sender: TObject);
    procedure pmDeletePopup(Sender: TObject);
    procedure mniDeleteClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  procedure ShowFrmProcessBlacklist;
  
implementation

{$R *.dfm}

procedure ShowFrmProcessBlacklist;
var
  I: Integer;
  ProcessInfo: PTProcessInfo;
  Item: TListItem;
  FrmProcessBlacklist: TFrmProcessBlacklist;
begin
  FrmProcessBlacklist := TFrmProcessBlacklist.Create(nil);
  try
    for I := 0 to g_ProcessBlackList.Count - 1 do
    begin
      ProcessInfo := g_ProcessBlacklist.Items[I];

      Item := FrmProcessBlacklist.lvProcessBlacklist.Items.Add;
      Item.Caption := IntToStr(I + 1);
      Item.Data := ProcessInfo;
      Item.SubItems.Add(ProcessInfo.ProcessName);
      Item.SubItems.Add(ProcessInfo.ProcessMD5);
    end;

    FrmProcessBlacklist.btnAdd.Enabled := g_ProcessBlackList.Count < g_ProcessBlackList.MaxCount;
    FrmProcessBlacklist.ShowModal;
  finally
    FrmProcessBlacklist.Free;
  end;
end;

procedure TFrmProcessBlacklist.btnSearchClick(Sender: TObject);
var
  I, StartIndex: Integer;
  Item: TListItem;
  IsFound: Boolean;
begin
  StartIndex := 0;
  for I := StartIndex to lvProcessBlacklist.Items.Count - 1 do
  begin
    IsFound := False;

    Item := lvProcessBlacklist.Items[I];
    if Item.SubItems.Count >= 2 then
    begin
      case cbbSearchField.ItemIndex of
        0:  IsFound := Pos(UpperCase(edtSearchText.Text), UpperCase(Item.SubItems[0])) > 0;    // 进程
        1:  IsFound := Pos(UpperCase(edtSearchText.Text), UpperCase(Item.SubItems[1])) > 0;    // MD5
      end;
    end;

    if IsFound then
    begin
      lvProcessBlacklist.Selected := Item;
      lvProcessBlacklist.Selected.MakeVisible(True);
      lvProcessBlacklist.SetFocus;
      Break;
    end;
  end;
end;

procedure TFrmProcessBlacklist.btnSearchNextClick(Sender: TObject);
var
  I, StartIndex: Integer;
  Item: TListItem;
  IsFound: Boolean;
begin
  StartIndex := lvProcessBlacklist.ItemIndex + 1;
  if StartIndex < 0 then
    StartIndex := 0
  else if StartIndex >= lvProcessBlacklist.Items.Count then
    StartIndex := 0;

  for I := StartIndex to lvProcessBlacklist.Items.Count - 1 do
  begin
    IsFound := False;

    Item := lvProcessBlacklist.Items[I];
    if Item.SubItems.Count >= 2 then
    begin
      case cbbSearchField.ItemIndex of
        0:  IsFound := Pos(UpperCase(edtSearchText.Text), UpperCase(Item.SubItems[0])) > 0;    // 进程
        1:  IsFound := Pos(UpperCase(edtSearchText.Text), UpperCase(Item.SubItems[1])) > 0;    // MD5
      end;
    end;

    if IsFound then
    begin
      lvProcessBlacklist.Selected := Item;
      lvProcessBlacklist.Selected.MakeVisible(True);
      lvProcessBlacklist.SetFocus;
      Break;
    end;
  end;
end;

procedure TFrmProcessBlacklist.edtSearchTextKeyDown(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if (Key = VK_RETURN) and (Length(edtSearchText.Text) > 0) then
  begin
    btnSearch.Click;
  end;
end;

procedure TFrmProcessBlacklist.btnAddClick(Sender: TObject);
var
  ProcessInfo: PTProcessInfo;
  Item: TListItem;
begin
  if ShowAddProcessBlack(ProcessInfo) then
  begin
    Item := lvProcessBlacklist.Items.Add;
    Item.Data := ProcessInfo;
    Item.Caption := IntToStr(lvProcessBlacklist.Items.Count);
    Item.SubItems.Add(ProcessInfo.ProcessName);
    Item.SubItems.Add(UpperCase(ProcessInfo.ProcessMD5));

    SaveProcessBlacklist;
    RebuildProcessBlacklist;

    btnAdd.Enabled := lvProcessBlacklist.Items.Count < g_ProcessBlackList.MaxCount;
  end;
end;

procedure TFrmProcessBlacklist.pmDeletePopup(Sender: TObject);
begin
  if (lvProcessBlacklist.Selected = nil) or (lvProcessBlacklist.Selected.Data = nil) then
  begin
    mniDelete.Visible := False;
    Exit;
  end;

  mniDelete.Visible := True;
end;

procedure TFrmProcessBlacklist.mniDeleteClick(Sender: TObject);
var
  I: Integer;
begin
  if (lvProcessBlacklist.Selected = nil) or (lvProcessBlacklist.Selected.Data = nil) then
  begin
    Exit;
  end;

  g_ProcessBlacklist.Lock;
  try
    g_ProcessBlacklist.Delete(lvProcessBlacklist.Selected.Data);
  finally
    g_ProcessBlacklist.UnLock;
  end;

  SaveProcessBlacklist;
  RebuildProcessBlacklist;

  lvProcessBlacklist.DeleteSelected;
  for I := 0 to lvProcessBlacklist.Items.Count - 1 do
  begin
    lvProcessBlacklist.Items[I].Caption := IntToStr(I + 1);
  end;

  btnAdd.Enabled := lvProcessBlacklist.Items.Count < g_ProcessBlackList.MaxCount; 
end;

end.
