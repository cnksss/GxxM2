unit uFrmMagicCD;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, VirtualTrees, MagicIntervalUtils, ParadoxDataSet,
  GateShare, SpinEditEx, Spin, ColorIndexEdit, IniFiles;

const
  WM_STARTEDITING = WM_USER + 779;

type
  TFrmMagicCD = class(TForm)
    vstMagicCD: TVirtualStringTree;
    btnOK: TButton;
    dlgOpen1: TOpenDialog;
    GroupBox1: TGroupBox;
    lbl1: TLabel;
    edtMagicCDMsgText: TEdit;
    lbl2: TLabel;
    cbbMagicCDMsgType: TComboBox;
    Label1: TLabel;
    Label2: TLabel;
    seMagicCDFColor: TColorIndexEdit;
    seMagicCDBColor: TColorIndexEdit;
    lbl3: TLabel;
    seMagicCDShowX: TSpinEditEx;
    Label4: TLabel;
    seMagicCDShowY: TSpinEditEx;
    lbl4: TLabel;
    edtMagicName: TEdit;
    btnSearch: TButton;
    btnSearchNext: TButton;
    procedure FormCreate(Sender: TObject);
    procedure vstMagicCDGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure vstMagicCDFreeNode(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure vstMagicCDDrawText(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: WideString; const CellRect: TRect;
      var DefaultDraw: Boolean);
    procedure vstMagicCDBeforeItemErase(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
      var ItemColor: TColor; var EraseAction: TItemEraseAction);
    procedure vstMagicCDEditing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstMagicCDNodeClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
    procedure vstMagicCDCreateEditor(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
    procedure vstMagicCDKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure btnOKClick(Sender: TObject);
    procedure btnSearchClick(Sender: TObject);
    procedure btnSearchNextClick(Sender: TObject);
    procedure edtMagicNameKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
  private
    { Private declarations }

    procedure DoOpen(MagicDBName: string);
    procedure WMStartEditing(var Message: TMessage); message WM_STARTEDITING;
  public
    { Public declarations }
  end;

  function ShowFrmMaigcCD: Boolean;

implementation

{$R *.dfm}

type
  PMagicData = ^TMagicData;
  TMagicData = record
    MagicID: Word;
    MagicName: string;
    CDTime: LongWord;
  end;

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
        CanAdvance := Shift = [];
        if FEdit is TComboBox then
          CanAdvance := CanAdvance and not TComboBox(FEdit).DroppedDown;
        if CanAdvance then
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
  NodeData: PMagicData;
  TempValue: Integer;
begin
  Result := True;

  NodeData := FTree.GetNodeData(FNode);
  if FEdit is TSpinEditEx then
  begin
    TempValue := (FEdit as TSpinEditEx).Value;
    case FColumn of
      2:
        if NodeData.CDTime <> TempValue then
        begin
          NodeData.CDTime := TempValue;
        end;
    end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

//----------------------------------------------------------------------------------------------------------------------

function TPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  NodeData: PMagicData;
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
    2:
      begin
        FEdit := TSpinEditEx.Create(nil);
        TSpinEditEx(FEdit).Font.Assign(FTree.Font);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;


          MaxValue := High(Integer);
          MinValue := 0;

          Value := NodeData.CDTime;

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
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, R.Left, R.Right);
  Windows.InflateRect(R, -1, 0);
  FEdit.BoundsRect := R;
end;

function ShowFrmMaigcCD: Boolean;
var
  S: string;
  FrmMagicCD: TFrmMagicCD;

  OpenDlg: TOpenDialog;
begin
  Result := False;
  S := ExtractFileDir(ParamStr(0));
  S := ExtractFilePath(S) + 'Mud2\DB\Magic.DB';

  if not FileExists(S) then
  begin
    OpenDlg := TOpenDialog.Create(nil);
    try
      OpenDlg.Filter := '技能数据库(Magic.DB)|*.DB';
      OpenDlg.FileName := 'Magic.DB';
      OpenDlg.Title := '指定技能数据库';

      if OpenDlg.Execute then
        S := OpenDlg.FileName
      else
      begin
        Exit;
      end;
    finally
      OpenDlg.Free;
    end;
  end;

  FrmMagicCD := TFrmMagicCD.Create(nil);
  try
    FrmMagicCD.DoOpen(S);
    Result := FrmMagicCD.ShowModal = mrOK;
  finally
    FrmMagicCD.Free;
  end;
end;

procedure TFrmMagicCD.FormCreate(Sender: TObject);
begin
  vstMagicCD.NodeDataSize := SizeOf(TMagicData);
end;

procedure TFrmMagicCD.vstMagicCDGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: WideString);
var
  NodeData: PMagicData;
begin
  NodeData := Sender.GetNodeData(Node);
  case Column of
    0: CellText := IntToStr(NodeData.MagicID);
    1: CellText := NodeData.MagicName;
    2: CellText := IntToStr(NodeData.CDTime);
  end;
end;

procedure TFrmMagicCD.vstMagicCDFreeNode(Sender: TBaseVirtualTree;
  Node: PVirtualNode);
var
  NodeData: PMagicData;
begin
  NodeData := Sender.GetNodeData(Node);
  NodeData.MagicName := '';
end;

procedure TFrmMagicCD.DoOpen(MagicDBName: string);
var
  I, MagicID: Integer;
  DateSet: TParadoxDataSet;
  MagicDescr: string;
  Node: PVirtualNode;
  NodeData: PMagicData;
  List: TList;
  MagicInterval: PMagicInterval;
begin
  cbbMagicCDMsgType.ItemIndex := g_btMagicCDMsgType;
  edtMagicCDMsgText.Text := g_sMagicCDMsgText;
  seMagicCDFColor.Value := g_btMagicCDFColor;
  seMagicCDBColor.Value := g_btMagicCDBColor;
  seMagicCDShowX.Value := g_nMagicCDShowX;
  seMagicCDShowY.Value := g_nMagicCDShowY;

  List := TList.Create;
  try
    DateSet := TParadoxDataSet.Create(nil);
    try
      DateSet.TableName := MagicDBName;
      DateSet.Open;
      DateSet.First;
      for I := 0 to DateSet.RecordCount - 1 do
      begin
        MagicID := DateSet.FieldByName('MagId').AsInteger;
        if MagicID > 0 then
        begin
          MagicDescr := DateSet.FieldByName('Descr').AsString;
          if (not SameText(MagicDescr, '英雄')) and
            (not SameText(MagicDescr, '静之')) and
            (not SameText(MagicDescr, '怒之')) then
          begin
            if List.IndexOf(Pointer(MagicID)) < 0 then
            begin
              Node := vstMagicCD.AddChild(nil);
              NodeData := vstMagicCD.GetNodeData(Node);

              NodeData.MagicID := MagicID;
              NodeData.MagicName := DateSet.FieldByName('MagName').AsString;

              MagicInterval := g_MagicCDList.Find(MagicID);
              if MagicInterval <> nil then
                NodeData.CDTime := MagicInterval.Interval
              else
                NodeData.CDTime := 0;

              List.Add(Pointer(DateSet.FieldByName('MagId').AsInteger));
            end;
          end;
        end;

        DateSet.Next;
      end;
    finally
      DateSet.Free;
    end;
  finally
    List.Free;
  end;
end;

procedure TFrmMagicCD.vstMagicCDDrawText(Sender: TBaseVirtualTree;
  TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
  const Text: WideString; const CellRect: TRect; var DefaultDraw: Boolean);
begin
  if not (Sender.Focused and (Node = Sender.FocusedNode)) then
    TargetCanvas.Font.Color := Sender.Font.Color;
end;

procedure TFrmMagicCD.vstMagicCDBeforeItemErase(Sender: TBaseVirtualTree;
  TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
  var ItemColor: TColor; var EraseAction: TItemEraseAction);
begin
  if Node.Index mod 2 <> 0 then
  begin
    EraseAction := eaColor;
    ItemColor := $00F9F9F9;
  end;
end;

procedure TFrmMagicCD.vstMagicCDEditing(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
begin
  Allowed := Column = 2;
end;

procedure TFrmMagicCD.vstMagicCDNodeClick(Sender: TBaseVirtualTree;
  const HitInfo: THitInfo);
begin
  if (HitInfo.HitColumn = 2) then
    PostMessage(Self.Handle, WM_STARTEDITING, WPARAM(HitInfo.HitNode), HitInfo.HitColumn)
end;

procedure TFrmMagicCD.WMStartEditing(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstMagicCD.EditNode(Node, Message.LParam);
end;

procedure TFrmMagicCD.vstMagicCDCreateEditor(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
begin
  EditLink := TPropertyEditLink.Create;
end;

procedure TFrmMagicCD.vstMagicCDKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if (Key = VK_RETURN) and (not vstMagicCD.IsEditing) and (vstMagicCD.FocusedNode <> nil) then
  begin
    vstMagicCD.EditNode(vstMagicCD.FocusedNode, 2);
  end;
end;

procedure TFrmMagicCD.btnOKClick(Sender: TObject);
var
  IniFile: TIniFile;
  Node: PVirtualNode;
  NodeData: PMagicData;
  MagicInterval: PMagicInterval;
begin
  g_btMagicCDMsgType := cbbMagicCDMsgType.ItemIndex;
  g_sMagicCDMsgText := edtMagicCDMsgText.Text;
  g_btMagicCDFColor := seMagicCDFColor.Value;
  g_btMagicCDBColor := seMagicCDBColor.Value;
  g_nMagicCDShowX := seMagicCDShowX.Value;
  g_nMagicCDShowY := seMagicCDShowY.Value;

  IniFile := TIniFile.Create(g_sIniFileName);
  try
    IniFile.WriteInteger('MagicCD', 'MsgType', g_btMagicCDMsgType);
    IniFile.WriteString('MagicCD', 'MsgText', g_sMagicCDMsgText);
    IniFile.WriteInteger('MagicCD', 'FColor', g_btMagicCDFColor);
    IniFile.WriteInteger('MagicCD', 'BColor', g_btMagicCDBColor);
    IniFile.WriteInteger('MagicCD', 'ShowX', g_nMagicCDShowX);
    IniFile.WriteInteger('MagicCD', 'ShowY', g_nMagicCDShowY);
  finally
    IniFile.Free;
  end;

  vstMagicCD.EndEditNode;
  g_MagicCDList.Lock;
  try
    g_MagicCDList.Clear;
    Node := vstMagicCD.GetFirst();
    while Node <> nil do
    begin
      NodeData := vstMagicCD.GetNodeData(Node);

      MagicInterval := g_MagicCDList.Add(NodeData.MagicID);
      if MagicInterval <> nil then
        MagicInterval.Interval := NodeData.CDTime;

      Node := vstMagicCD.GetNext(Node);
    end;
  finally
    g_MagicCDList.UnLock;
  end;

  g_MagicCDList.SaveToFile(g_MagicCDListFileName);

  ModalResult := mrOK;
end;

procedure TFrmMagicCD.btnSearchClick(Sender: TObject);
var
  Node: PVirtualNode;
  NodeData: PMagicData;
  MagicName: string;
begin
  MagicName := Trim(edtMagicName.Text);
  if Length(MagicName) = 0 then
  begin
    Application.MessageBox('搜索内容不能为空', '信息', MB_OK + MB_ICONINFORMATION);
    edtMagicName.SetFocus;
    Exit;
  end;

  Node := vstMagicCD.GetFirst();
  while Node <> nil do
  begin
    NodeData := vstMagicCD.GetNodeData(Node);

    if Pos(MagicName, NodeData.MagicName) > 0 then
    begin
      vstMagicCD.FocusedNode := Node;
      vstMagicCD.Selected[Node] := True;
      vstMagicCD.SetFocus;
      Exit;
    end;

    Node := vstMagicCD.GetNext(Node);
  end;
end;

procedure TFrmMagicCD.btnSearchNextClick(Sender: TObject);
var
  Node: PVirtualNode;
  NodeData: PMagicData;
  MagicName: string;
begin
  MagicName := Trim(edtMagicName.Text);
  if Length(MagicName) = 0 then
  begin
    Application.MessageBox('搜索内容不能为空', '信息', MB_OK + MB_ICONINFORMATION);
    edtMagicName.SetFocus;
    Exit;
  end;

  Node := vstMagicCD.FocusedNode;
  if Node = nil then
    Node := vstMagicCD.GetFirst()
  else
    Node := vstMagicCD.GetNext(Node);
    
  while Node <> nil do
  begin
    NodeData := vstMagicCD.GetNodeData(Node);

    if Pos(MagicName, NodeData.MagicName) > 0 then
    begin
      vstMagicCD.FocusedNode := Node;
      vstMagicCD.Selected[Node] := True;
      vstMagicCD.SetFocus;
      Exit;
    end;

    Node := vstMagicCD.GetNext(Node);
  end;
end;

procedure TFrmMagicCD.edtMagicNameKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if (Key = VK_RETURN) and (Length(Trim(edtMagicName.Text)) > 0) then
  begin
    btnSearch.Click;
  end;
end;

end.
