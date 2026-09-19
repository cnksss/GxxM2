unit RouteEdit;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, DBShare, StdCtrls, Grids, VirtualTrees, Spin, SpinEditEx, uRunGateList;

const
  WM_STARTEDITING = WM_USER + 778;

type
  TfrmRouteEdit = class(TForm)
    Label1: TLabel;
    EditSelGate: TEdit;
    GroupBox1: TGroupBox;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    edtGateIP1: TEdit;
    edtGateIP2: TEdit;
    edtGatePort1: TEdit;
    edtGatePort2: TEdit;
    edtGateIP3: TEdit;
    edtGatePort3: TEdit;
    edtGateIP4: TEdit;
    edtGatePort4: TEdit;
    edtGateIP5: TEdit;
    edtGatePort5: TEdit;
    edtGateIP6: TEdit;
    edtGatePort6: TEdit;
    edtGateIP7: TEdit;
    edtGatePort7: TEdit;
    edtGateIP8: TEdit;
    edtGatePort8: TEdit;
    grp1: TGroupBox;
    btnCancel: TButton;
    btnOK: TButton;
    chkOpenRunGate2: TCheckBox;
    seGameGateDisconnectCount: TSpinEditEx;
    lbl1: TLabel;
    lbl2: TLabel;
    vstRunGate: TVirtualStringTree;
    btnAdd: TButton;
    btnDel: TButton;
    edtDBPort1: TEdit;
    edtDBPort2: TEdit;
    edtDBPort3: TEdit;
    edtDBPort4: TEdit;
    edtDBPort5: TEdit;
    edtDBPort6: TEdit;
    edtDBPort7: TEdit;
    edtDBPort8: TEdit;
    procedure btnOKClick(Sender: TObject);
    procedure vstRunGateCreateEditor(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
    procedure vstRunGateEditing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstRunGateNodeClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
    procedure FormCreate(Sender: TObject);
    procedure btnAddClick(Sender: TObject);
    procedure vstRunGateGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure btnDelClick(Sender: TObject);
    procedure vstRunGateKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure chkOpenRunGate2Click(Sender: TObject);
    procedure vstRunGatePaintText(Sender: TBaseVirtualTree;
      const TargetCanvas: TCanvas; Node: PVirtualNode;
      Column: TColumnIndex; TextType: TVSTTextType);
    procedure EditSelGateChange(Sender: TObject);
  private
    FRouteInfo: PTRouteInfo;

    FIsChanged: Boolean;
    procedure DoOpen();
    procedure WMStartEditing(var Message: TMessage); message WM_STARTEDITING;
    { Private declarations }
  public
    { Public declarations }
  end;

  function ShowFrmRouteEdit(RouteInfo: PTRouteInfo; IsEdit: Boolean = False): Boolean;

implementation

uses HUtil32;

{$R *.dfm}

type
  TPropertyEditLink = class(TInterfacedObject, IVTEditLink)
  private
    FEdit: TWinControl;        // One of the property editor classes.
    FTree: TVirtualStringTree; // A back reference to the tree calling.
    FNode: PVirtualNode;       // The node being edited.
    FColumn: Integer;          // The column of the node being edited.
  protected
    procedure EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
  public
    destructor Destroy; override;

    function BeginEdit: Boolean; stdcall;
    function CancelEdit: Boolean; stdcall;
    function EndEdit: Boolean; stdcall;
    function GetBounds: TRect; stdcall;
    function PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean; stdcall;
    procedure ProcessMessage(var Message: TMessage); stdcall;
    procedure SetBounds(R: TRect); stdcall;
  end;

destructor TPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CanAdvance: Boolean;
begin
  CanAdvance := True;

  case Key of
    VK_ESCAPE:
      begin
        Key := 0;//ESC will be handled in EditKeyUp()
      end;
    VK_RETURN:
      if CanAdvance then
      begin
        Key := 0;
        FTree.EndEditNode;
        Abort;
      end;
    VK_UP,
    VK_DOWN:
      begin
        // Consider special cases before finishing edit mode.
        CanAdvance := ssCtrl in Shift;
        if FEdit is TSpinEditEx then
          CanAdvance := True;
        if CanAdvance then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
    VK_INSERT, VK_DELETE:
      begin
        if Shift = [ssCtrl] then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
  end;
end;

procedure TPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_ESCAPE:
      begin
        FTree.CancelEditNode;
        Key := 0;
      end;//VK_ESCAPE
  end;//case
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.EndEdit: Boolean;
var
  NodeData: PRunGateInfo;
  TempValue: Integer;
  TempString: string;
  IsChanged: Boolean;
begin
  Result := True;
  IsChanged := False;

  NodeData := FTree.GetNodeData(FNode);
  if FEdit is TEdit then
  begin
    if FColumn = 1 then
    begin
      TempString := Trim((FEdit as TEdit).Text);

      if not SameText(TempString, NodeData.IP) then
      begin
        NodeData.IP := (FEdit as TEdit).Text;
        IsChanged := True;
      end;
    end;
  end
  else if FEdit is TSpinEditEx then
  begin
    TempValue := (FEdit as TSpinEditEx).Value;
    case FColumn of
      2:
        if NodeData.Port <> TempValue then
        begin
          NodeData.Port := TempValue;
          IsChanged := True;
        end;
      3:
        if NodeData.DBPort <> TempValue then
        begin
          NodeData.DBPort := TempValue;
          IsChanged := True;
        end;
      4:
        if NodeData.Level <> TempValue then
        begin
          NodeData.Level := TempValue;
          IsChanged := True;
        end;
    end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged then
  begin
    if (FTree.Owner is TFrmRouteEdit) then
    begin
      TFrmRouteEdit(FTree.Owner).FIsChanged := True;
    end;
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  NodeData: PRunGateInfo;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;

  // determine what edit type actually is needed
  FEdit.Free;
  FEdit := nil;
  NodeData := FTree.GetNodeData(Node);
  case FColumn of
    2, 3, 4:
      begin
        FEdit := TSpinEditEx.Create(nil);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;

          MinValue := 0;
          MaxValue := High(Word);

          case FColumn of
            2: Value := NodeData.Port;
            3: Value := NodeData.DBPort;
            4: Value := NodeData.Level;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    1:
      begin
        FEdit := TEdit.Create(nil);

        with FEdit as TEdit do
        begin
          Visible := False;
          Parent := Tree;
          MaxLength := 15;
          Text := NodeData.IP;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TPropertyEditLink.ProcessMessage(var Message: TMessage);

begin
  FEdit.WindowProc(Message);
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TPropertyEditLink.SetBounds(R: TRect);

var
  Dummy: Integer;

begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;


function ShowFrmRouteEdit(RouteInfo: PTRouteInfo; IsEdit: Boolean = False): Boolean;
var
  frmRouteEdit: TfrmRouteEdit;
begin
  frmRouteEdit := TfrmRouteEdit.Create(nil);
  try
    if IsEdit then
      frmRouteEdit.Caption := '修改网关路由'
    else
      frmRouteEdit.Caption := '增加网关路由';
    frmRouteEdit.FRouteInfo := RouteInfo;
    frmRouteEdit.DoOpen;
    Result := (frmRouteEdit.ShowModal = mrOK) and (frmRouteEdit.FIsChanged);

    if Result then
    begin
      SaveServerInfo;
    end;
  finally
    frmRouteEdit.Free;
  end;
end;

{ TfrmRouteEdit }

procedure TfrmRouteEdit.DoOpen;
var
  I: Integer;
  Node: PVirtualNode;
  NodeData: PRunGateInfo;
begin
  EditSelGate.Text := FRouteInfo.sSelGateIP;
  edtGateIP1.Text := FRouteInfo.sGameGateIP[0];
  edtGatePort1.Text := IntToStr(FRouteInfo.nGameGatePort[0]);
  edtDBPort1.Text := IntToStr(FRouteInfo.nGameGateDBPort[0]);

  edtGateIP2.Text := FRouteInfo.sGameGateIP[1];
  edtGatePort2.Text := IntToStr(FRouteInfo.nGameGatePort[1]);
  edtDBPort2.Text := IntToStr(FRouteInfo.nGameGateDBPort[1]);

  edtGateIP3.Text := FRouteInfo.sGameGateIP[2];
  edtGatePort3.Text := IntToStr(FRouteInfo.nGameGatePort[2]);
  edtDBPort3.Text := IntToStr(FRouteInfo.nGameGateDBPort[2]);

  edtGateIP4.Text := FRouteInfo.sGameGateIP[3];
  edtGatePort4.Text := IntToStr(FRouteInfo.nGameGatePort[3]);
  edtDBPort4.Text := IntToStr(FRouteInfo.nGameGateDBPort[3]);

  edtGateIP5.Text := FRouteInfo.sGameGateIP[4];
  edtGatePort5.Text := IntToStr(FRouteInfo.nGameGatePort[4]);
  edtDBPort5.Text := IntToStr(FRouteInfo.nGameGateDBPort[4]);

  edtGateIP6.Text := FRouteInfo.sGameGateIP[5];
  edtGatePort6.Text := IntToStr(FRouteInfo.nGameGatePort[5]);
  edtDBPort6.Text := IntToStr(FRouteInfo.nGameGateDBPort[5]);

  edtGateIP7.Text := FRouteInfo.sGameGateIP[6];
  edtGatePort7.Text := IntToStr(FRouteInfo.nGameGatePort[6]);
  edtDBPort7.Text := IntToStr(FRouteInfo.nGameGateDBPort[6]);

  edtGateIP8.Text := FRouteInfo.sGameGateIP[7];
  edtGatePort8.Text := IntToStr(FRouteInfo.nGameGatePort[7]);
  edtDBPort8.Text := IntToStr(FRouteInfo.nGameGateDBPort[7]);

  chkOpenRunGate2.Checked := FRouteInfo.EnabledRunGate2List;
  seGameGateDisconnectCount.Value := FRouteInfo.GameGateDisconnectCount;

  vstRunGate.Clear;
  vstRunGate.BeginUpdate;
  try
    for I := 0 to FRouteInfo.RunGate2List.Count - 1 do
    begin
      Node := vstRunGate.AddChild(nil);
      NodeData := vstRunGate.GetNodeData(Node);
      NodeData^ := FRouteInfo.RunGate2List.Items[I]^;

      Node.CheckType := ctCheckBox;

      if NodeData.Enabled then
        Node.CheckState := csCheckedNormal
      else
        Node.CheckState := csUncheckedNormal;

      NodeData.IsConnect := GetTickCount - NodeData.LastResponseTick <= 3000;
    end;
  finally
    vstRunGate.EndUpdate;
  end;

  seGameGateDisconnectCount.Enabled := chkOpenRunGate2.Checked;
  vstRunGate.Enabled := chkOpenRunGate2.Checked;
  btnAdd.Enabled := chkOpenRunGate2.Checked;
  btnDel.Enabled := chkOpenRunGate2.Checked;

  FIsChanged := False;
end;

procedure TfrmRouteEdit.btnOKClick(Sender: TObject);
var
  sSelGateIP, sGameGateIP: string;
  nGameGatePort: Integer;

  Node, NextNode: PVirtualNode;
  NodeData, NextNodeData: PRunGateInfo;
  IsEnable: Boolean;
begin
  sSelGateIP := Trim(EditSelGate.Text);
  if not IsIPaddr(sSelGateIP) then
  begin
    MessageBox(Handle, '角色网关输入错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    EditSelGate.SetFocus;
    Exit;
  end;

  sGameGateIP := Trim(edtGateIP1.Text);
  nGameGatePort := StrToIntDef(edtGatePort1.Text, 0);
  if not IsIPaddr(sGameGateIP) then
  begin
    MessageBox(Handle, '游戏网关一输入错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtGateIP1.SetFocus;
    Exit;
  end;
  if nGameGatePort <= 0 then
  begin
    MessageBox(Handle, '游戏网关一输入错误！！！', '错误信息', MB_OK + MB_ICONERROR);
    edtGatePort1.SetFocus;
    Exit;
  end;

  vstRunGate.EndEditNode;
  if chkOpenRunGate2.Checked then
  begin
    Node := vstRunGate.GetFirst();
    while Node <> nil do
    begin
      NodeData := vstRunGate.GetNodeData(Node);

      if not IsIPaddr(Trim(NodeData.IP)) then
      begin
        MessageBox(Handle, 'IP输入错误！', '错误信息', MB_OK + MB_ICONERROR);
        vstRunGate.Selected[Node] := True;
        vstRunGate.FocusedNode := Node;
        vstRunGate.EditNode(Node, 1);
        Exit;
      end;

      if NodeData.Port <= 0 then
      begin
        MessageBox(Handle, '端口输入错误！', '错误信息', MB_OK + MB_ICONERROR);
        vstRunGate.Selected[Node] := True;
        vstRunGate.FocusedNode := Node;
        vstRunGate.EditNode(Node, 2);
        Exit;
      end;

      NextNode := vstRunGate.GetNext(Node);
      while NextNode <> nil do
      begin
        NextNodeData := vstRunGate.GetNodeData(NextNode);

        if SameText(Trim(NextNodeData.IP), Trim(NodeData.IP)) and
          ((NextNodeData.Port = NodeData.Port) or (NextNodeData.DBPort = NodeData.DBPort)) then
        begin
          MessageBox(Handle, PChar('IP端口输入重复' + '[' + IntToStr(Node.Index) + ']'), '错误信息', MB_OK + MB_ICONERROR);
          vstRunGate.Selected[NextNode] := True;
          vstRunGate.FocusedNode := NextNode;
          Exit;
        end;
        NextNode := vstRunGate.GetNext(NextNode);
      end;

      Node := vstRunGate.GetNext(Node);
    end;
  end;

  FRouteInfo.sSelGateIP := sSelGateIP;
  FRouteInfo.sGameGateIP[0] := sGameGateIP;
  FRouteInfo.nGameGatePort[0] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[0] := StrToIntDef(Trim(edtDBPort1.Text), 0);
  FRouteInfo.nGateCount := 1;

  FRouteInfo.EnabledRunGate2List := chkOpenRunGate2.Checked;
  FRouteInfo.GameGateDisconnectCount := seGameGateDisconnectCount.Value;

  if FIsChanged then
  begin
    FRouteInfo.RunGate2List.Clear;
    vstRunGate.EndEditNode;
    if chkOpenRunGate2.Checked then
    begin
      Node := vstRunGate.GetFirst();
      while Node <> nil do
      begin
        NodeData := vstRunGate.GetNodeData(Node);

        IsEnable := Node.CheckState = csCheckedNormal;
        FRouteInfo.RunGate2List.Add(IsEnable, NodeData.IP, NodeData.Port, NodeData.DBPort, NodeData.Level);
        Node := vstRunGate.GetNext(Node);
      end;
    end;
    FRouteInfo.RunGate2List.DoSort;
  end;

  sGameGateIP := Trim(edtGateIP2.Text);
  nGameGatePort := StrToIntDef(edtGatePort2.Text, 0);
  if (not IsIPaddr(sGameGateIP)) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[1] := sGameGateIP;
  FRouteInfo.nGameGatePort[1] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[1] := StrToIntDef(Trim(edtDBPort2.Text), 0);
  FRouteInfo.nGateCount := 2;

  sGameGateIP := Trim(edtGateIP3.Text);
  nGameGatePort := StrToIntDef(edtGatePort3.Text, 0);
  if (not IsIPaddr(sGameGateIP)) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[2] := sGameGateIP;
  FRouteInfo.nGameGatePort[2] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[2] := StrToIntDef(Trim(edtDBPort3.Text), 0);
  FRouteInfo.nGateCount := 3;

  sGameGateIP := Trim(edtGateIP4.Text);
  nGameGatePort := StrToIntDef(edtGatePort4.Text, 0);
  if (not IsIPaddr(sGameGateIP)) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[3] := sGameGateIP;
  FRouteInfo.nGameGatePort[3] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[3] := StrToIntDef(Trim(edtDBPort4.Text), 0);
  FRouteInfo.nGateCount := 4;

  sGameGateIP := Trim(edtGateIP5.Text);
  nGameGatePort := StrToIntDef(edtGatePort5.Text, 0);
  if (not IsIPaddr(sGameGateIP)) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[4] := sGameGateIP;
  FRouteInfo.nGameGatePort[4] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[4] := StrToIntDef(Trim(edtDBPort5.Text), 0);
  FRouteInfo.nGateCount := 5;

  sGameGateIP := Trim(edtGateIP6.Text);
  nGameGatePort := StrToIntDef(edtGatePort6.Text, 0);
  if (not IsIPaddr(FRouteInfo.sGameGateIP[5])) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[5] := sGameGateIP;
  FRouteInfo.nGameGatePort[5] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[5] := StrToIntDef(Trim(edtDBPort6.Text), 0);
  FRouteInfo.nGateCount := 6;

  sGameGateIP := Trim(edtGateIP7.Text);
  nGameGatePort := StrToIntDef(edtGatePort7.Text, 0);
  if (not IsIPaddr(FRouteInfo.sGameGateIP[6])) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[6] := sGameGateIP;
  FRouteInfo.nGameGatePort[6] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[6] := StrToIntDef(Trim(edtDBPort7.Text), 0);
  FRouteInfo.nGateCount := 7;

  sGameGateIP := Trim(edtGateIP8.Text);
  nGameGatePort := StrToIntDef(edtGatePort8.Text, 0);
  if (not IsIPaddr(FRouteInfo.sGameGateIP[7])) or (nGameGatePort <= 0) then
  begin
    ModalResult := mrOK;
    Exit;
  end;
  FRouteInfo.sGameGateIP[7] := sGameGateIP;
  FRouteInfo.nGameGatePort[7] := nGameGatePort;
  FRouteInfo.nGameGateDBPort[7] := StrToIntDef(Trim(edtDBPort8.Text), 0);
  FRouteInfo.nGateCount := 8;
  ModalResult := mrOK;
end;

procedure TfrmRouteEdit.vstRunGateCreateEditor(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
begin
  EditLink := TPropertyEditLink.Create;
end;

procedure TfrmRouteEdit.vstRunGateEditing(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
begin
  Allowed := Node <> nil;
end;

procedure TfrmRouteEdit.vstRunGateNodeClick(Sender: TBaseVirtualTree;
  const HitInfo: THitInfo);
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn >= 0)  then
    PostMessage(Self.Handle, WM_STARTEDITING, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
end;

procedure TfrmRouteEdit.WMStartEditing(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstRunGate.EditNode(Node, Message.LParam);
end;

procedure TfrmRouteEdit.FormCreate(Sender: TObject);
begin
  FIsChanged := False;
  vstRunGate.NodeDataSize := SizeOf(TRunGateInfo);
end;

procedure TfrmRouteEdit.btnAddClick(Sender: TObject);
var
  NodeData: PRunGateInfo;
  Node: PVirtualNode;
begin
  if vstRunGate.RootNodeCount >= 100 then
  begin
    MessageBox(Handle, '备用网关已达到最大数量,不能再增加！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;

  if vstRunGate.IsEditing then
    vstRunGate.EndEditNode;

  Node := vstRunGate.AddChild(nil);
  Node.CheckType := ctCheckBox;
  Node.CheckState := csCheckedNormal;
  NodeData := vstRunGate.GetNodeData(Node);
  NodeData.Enabled := True;
  NodeData.IP := '';
  NodeData.Port := 7200;
  NodeData.Level := 10;
  NodeData.IsConnect := True;
  NodeData.LastResponseTick := 0;

  vstRunGate.Selected[Node] := True;
  vstRunGate.FocusedNode := Node;

  vstRunGate.EditNode(Node, 1);

  FIsChanged := True;
end;

procedure TfrmRouteEdit.vstRunGateGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: WideString);
var
  NodeData: PRunGateInfo;
begin
  NodeData := Sender.GetNodeData(Node);
  case Column of
    0: CellText := IntToStr(Node.Index);
    1: CellText := NodeData.IP;
    2: CellText := IntToStr(NodeData.Port);
    3: CellText := IntToStr(NodeData.DBPort);
    4: CellText := IntToStr(NodeData.Level);
  end;
end;

procedure TfrmRouteEdit.btnDelClick(Sender: TObject);
var
  Node, SelNode: PVirtualNode;
begin
  Node := vstRunGate.FocusedNode;
  if Node <> nil then
  begin
    SelNode := vstRunGate.GetNext(Node);
    if SelNode = nil then
      SelNode := vstRunGate.GetPrevious(Node);

    vstRunGate.DeleteNode(Node);

    if SelNode <> nil then
    begin
      vstRunGate.Selected[SelNode] := True;
      vstRunGate.FocusedNode := SelNode;
    end;

    FIsChanged := True;
  end;
end;

procedure TfrmRouteEdit.vstRunGateKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if (Shift = [ssCtrl]) then
  begin
    if (Key = VK_INSERT) then
      btnAdd.Click
    else if (Key = VK_DELETE) then
      btnDel.Click;
  end;
end;

procedure TfrmRouteEdit.chkOpenRunGate2Click(Sender: TObject);
begin
  seGameGateDisconnectCount.Enabled := chkOpenRunGate2.Checked;
  vstRunGate.Enabled := chkOpenRunGate2.Checked;
  btnAdd.Enabled := chkOpenRunGate2.Checked;
  btnDel.Enabled := chkOpenRunGate2.Checked;
end;

procedure TfrmRouteEdit.vstRunGatePaintText(Sender: TBaseVirtualTree;
  const TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType);
var
  NodeData: PRunGateInfo;
begin
  NodeData := Sender.GetNodeData(Node);
  if not NodeData.IsConnect then
    TargetCanvas.Font.Color := clRed
  else
    TargetCanvas.Font.Color := Sender.Font.Color;
end;

procedure TfrmRouteEdit.EditSelGateChange(Sender: TObject);
begin
  FIsChanged := True;
end;

end.
