unit uFrmBatchEditAccountInfo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, VirtualTrees, ExtCtrls, StdCtrls, JvgGroupBox, AccountDB;

type
  TFrmBatchEditAccountInfo = class(TForm)
    pnlLeft: TPanel;
    vstEnableAccount: TVirtualStringTree;
    Panel1: TPanel;
    pnlRightTitle: TPanel;
    btnSetEnabled: TButton;
    edtDisableAccount: TEdit;
    pnlLeftTitle: TPanel;
    btnSetDisable: TButton;
    btn2: TButton;
    edtEnabledAccount: TEdit;
    lbl1: TLabel;
    Label1: TLabel;
    vstDisableAccount: TVirtualStringTree;
    dlgOpen: TOpenDialog;
    procedure FormCreate(Sender: TObject);
    procedure vstEnableAccountGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure vstEnableAccountBeforeItemErase(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
      var ItemColor: TColor; var EraseAction: TItemEraseAction);
    procedure btnSetDisableClick(Sender: TObject);
    procedure btnSetEnabledClick(Sender: TObject);
    procedure edtEnabledAccountChange(Sender: TObject);
    procedure edtDisableAccountChange(Sender: TObject);
    procedure btn2Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;


  procedure ShowFrmBatchEditAccountInfo;

implementation

{$R *.dfm}

type
  PNodeData = ^TNodeData;
  TNodeData = record
    AccountName: string[14];    // 帐户名
  end;

procedure ShowFrmBatchEditAccountInfo;
var
  FrmBatchEditAccountInfo: TFrmBatchEditAccountInfo;
begin
  FrmBatchEditAccountInfo := TFrmBatchEditAccountInfo.Create(nil);
  try
    FrmBatchEditAccountInfo.ShowModal;
  finally
    FrmBatchEditAccountInfo.Free;
  end;
end;

procedure TFrmBatchEditAccountInfo.FormCreate(Sender: TObject);
var
  I: Integer;
  AccountList: TStringList;

  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  vstEnableAccount.NodeDataSize := SizeOf(TNodeData);
  vstDisableAccount.NodeDataSize := SizeOf(TNodeData);
  
  AccountList := TStringList.Create;
  try
    g_AccountDB.GetAllAccount(AccountList);

    for I := 0 to AccountList.Count - 1 do
    begin
      if Integer(AccountList.Objects[I]) = 0 then
      begin
        Node := vstEnableAccount.AddChild(nil);
        Node.CheckType := ctCheckBox;
        Node.CheckState := csUncheckedNormal;
        NodeData := vstEnableAccount.GetNodeData(Node);
        NodeData.AccountName := AccountList.Strings[I];
      end
      else
      begin
        Node := vstDisableAccount.AddChild(nil);
        Node.CheckType := ctCheckBox;
        Node.CheckState := csUncheckedNormal;
        NodeData := vstDisableAccount.GetNodeData(Node);
        NodeData.AccountName := AccountList.Strings[I];
      end;
    end;

  finally
    AccountList.Free;
  end;
end;

procedure TFrmBatchEditAccountInfo.vstEnableAccountGetText(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: WideString);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  CellText := NodeData.AccountName;
end;

procedure TFrmBatchEditAccountInfo.vstEnableAccountBeforeItemErase(
  Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  ItemRect: TRect; var ItemColor: TColor;
  var EraseAction: TItemEraseAction);
begin
  if Node.Index mod 2 <> 0 then
  begin
    ItemColor := $00FFFBF7;
    EraseAction := eaColor;
  end;
end;

procedure TFrmBatchEditAccountInfo.btnSetDisableClick(Sender: TObject);
var
  I: Integer;
  SL: TStringList;
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  SL := TStringList.Create;
  try
    Node := vstEnableAccount.GetFirstChecked();
    while Node <> nil do
    begin
      NodeData := vstEnableAccount.GetNodeData(Node);
      SL.AddObject(NodeData.AccountName, TObject(Node));
      Node := vstEnableAccount.GetNextChecked(Node);
    end;


    if SL.Count > 0 then
    begin
      if g_AccountDB.EnabledAccounts(SL, False) then
      begin
        for I := 0 to SL.Count - 1 do
        begin
          Node := vstDisableAccount.AddChild(nil);
          Node.CheckType := ctCheckBox;
          Node.CheckState := csUncheckedNormal;
          NodeData := vstDisableAccount.GetNodeData(Node);
          NodeData.AccountName := SL.Strings[I];

          vstEnableAccount.DeleteNode(PVirtualNode(SL.Objects[I]));
        end;

        ShowMessage('已成功禁用' + IntToStr(SL.Count) + '个帐户');
      end;
    end;
  finally
    SL.Free;
  end;
end;

procedure TFrmBatchEditAccountInfo.btnSetEnabledClick(Sender: TObject);
var
  I: Integer;
  SL: TStringList;
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  SL := TStringList.Create;
  try
    Node := vstDisableAccount.GetFirstChecked();
    while Node <> nil do
    begin
      NodeData := vstDisableAccount.GetNodeData(Node);
      SL.AddObject(NodeData.AccountName, TObject(Node));
      Node := vstDisableAccount.GetNextChecked(Node);
    end;


    if SL.Count > 0 then
    begin
      if g_AccountDB.EnabledAccounts(SL, True) then
      begin
        for I := 0 to SL.Count - 1 do
        begin
          Node := vstEnableAccount.AddChild(nil);
          Node.CheckType := ctCheckBox;
          Node.CheckState := csUncheckedNormal;
          NodeData := vstEnableAccount.GetNodeData(Node);
          NodeData.AccountName := SL.Strings[I];

          vstDisableAccount.DeleteNode(PVirtualNode(SL.Objects[I]));
        end;

        ShowMessage('已成功解禁' + IntToStr(SL.Count) + '个帐户');
      end;
    end;
  finally
    SL.Free;
  end;
end;

procedure TFrmBatchEditAccountInfo.edtEnabledAccountChange(
  Sender: TObject);
var
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  if Length(edtEnabledAccount.Text) = 0 then
  begin
    vstEnableAccount.BeginUpdate;
    try
      Node := vstEnableAccount.GetFirst();
      while Node <> nil do
      begin
        vstEnableAccount.IsVisible[Node] := True;
        Node := vstEnableAccount.GetNext(Node);
      end;
    finally
      vstEnableAccount.EndUpdate;
    end;
  end
  else
  begin
    vstEnableAccount.BeginUpdate;
    try
      Node := vstEnableAccount.GetFirst();
      while Node <> nil do
      begin
        NodeData := vstEnableAccount.GetNodeData(Node);
        vstEnableAccount.IsVisible[Node] := Pos(edtEnabledAccount.Text, NodeData.AccountName) > 0;
        Node := vstEnableAccount.GetNext(Node);
      end;
    finally
      vstEnableAccount.EndUpdate;
    end;
  end;
end;

procedure TFrmBatchEditAccountInfo.edtDisableAccountChange(
  Sender: TObject);
var
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  if Length(edtDisableAccount.Text) = 0 then
  begin
    vstDisableAccount.BeginUpdate;
    try
      Node := vstDisableAccount.GetFirst();
      while Node <> nil do
      begin
        vstDisableAccount.IsVisible[Node] := True;
        Node := vstDisableAccount.GetNext(Node);
      end;
    finally
      vstDisableAccount.EndUpdate;
    end;
  end
  else
  begin
    vstDisableAccount.BeginUpdate;
    try
      Node := vstDisableAccount.GetFirst();
      while Node <> nil do
      begin
        NodeData := vstDisableAccount.GetNodeData(Node);
        vstDisableAccount.IsVisible[Node] := Pos(edtDisableAccount.Text, NodeData.AccountName) > 0;
        Node := vstDisableAccount.GetNext(Node);
      end;
    finally
      vstDisableAccount.EndUpdate;
    end;
  end;
end;

procedure TFrmBatchEditAccountInfo.btn2Click(Sender: TObject);
var
  SL: TStringList;
  Node: PVirtualNode;
  NodeData: PNodeData;
  Count: Integer;
begin
  if dlgOpen.Execute then
  begin
    SL := TStringList.Create;
    try
      SL.LoadFromFile(dlgOpen.FileName);

      Count := 0;

      Node := vstEnableAccount.GetFirst();
      while Node <> nil do
      begin
        NodeData := vstEnableAccount.GetNodeData(Node);

        vstDisableAccount.IsVisible[Node] := True;

        if SL.IndexOf(NodeData.AccountName) > 0 then
        begin
          Node.CheckState := csCheckedNormal;
          Inc(Count);
        end
        else
        begin
          Node.CheckState := csUnCheckedNormal;
        end;

        Node := vstEnableAccount.GetNext(Node);
      end;

      vstEnableAccount.Invalidate;
      ShowMessage('已成功导入并选中' + IntToStr(Count) + '个帐户');
    finally
      SL.Free;
    end;
  end;
end;

end.
