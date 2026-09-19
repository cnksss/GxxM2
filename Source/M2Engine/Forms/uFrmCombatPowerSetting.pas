unit uFrmCombatPowerSetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ExtCtrls, VirtualTrees, ComCtrls, uCombatPowerUtils,
  SpinEditEx, uFrmCombatPowerAddVar, M2Share, Menus;

const
  WM_STARTEDITING_ITEMDEF = WM_USER + 780;
  WM_STARTEDITING_ITEMVAR = WM_USER + 781;

type
  TFrmCombatPowerSetting = class(TForm)
    pgcMain: TPageControl;
    ts1: TTabSheet;
    ts2: TTabSheet;
    vstDefPower: TVirtualStringTree;
    pnlBottom: TPanel;
    chkOpenCombatPowerCalc: TCheckBox;
    btnRecalHumanCombatPower: TButton;
    pnlVarTop: TPanel;
    chkOpenCombatPowerVarCalc: TCheckBox;
    btnOK: TButton;
    pnlVarBotton: TPanel;
    lbl1: TLabel;
    lbl35: TLabel;
    edtItemSearch: TEdit;
    vstVarPower: TVirtualStringTree;
    btnAddVar: TButton;
    btnDelVar: TButton;
    lbl2: TLabel;
    pmCopy: TPopupMenu;
    mniCopy1: TMenuItem;
    mniCopy2: TMenuItem;
    procedure FormCreate(Sender: TObject);
    procedure vstDefPowerGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstDefPowerBeforeItemErase(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
      var ItemColor: TColor; var EraseAction: TItemEraseAction);
    procedure vstDefPowerCreateEditor(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
    procedure vstDefPowerEditing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstDefPowerNodeClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
    procedure btnOKClick(Sender: TObject);
    procedure vstDefPowerKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure vstDefPowerChange(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure btnAddVarClick(Sender: TObject);
    procedure vstVarPowerGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstVarPowerCreateEditor(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
    procedure vstVarPowerEditing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstVarPowerKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure vstVarPowerNodeClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
    procedure vstVarPowerChange(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure vstVarPowerKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure btnDelVarClick(Sender: TObject);
    procedure chkOpenCombatPowerCalcClick(Sender: TObject);
    procedure chkOpenCombatPowerVarCalcClick(Sender: TObject);
    procedure vstVarPowerFreeNode(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure edtItemSearchChange(Sender: TObject);
    procedure btnRecalHumanCombatPowerClick(Sender: TObject);
    procedure pmCopyPopup(Sender: TObject);
    procedure mniCopy1Click(Sender: TObject);
    procedure mniCopy2Click(Sender: TObject);
    procedure vstDefPowerGetHint(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex;
      var LineBreakStyle: TVTTooltipLineBreakStyle;
      var HintText: string);
  private
    { Private declarations }

    FDefChanged: Boolean;
    FVarChanged: Boolean;

    procedure WMStartEditingItemDef(var Message: TMessage); message WM_STARTEDITING_ITEMDEF;
    procedure WMStartEditingItemVar(var Message: TMessage); message WM_STARTEDITING_ITEMVAR;

    procedure DoConfigChanged(IsDef: Boolean);

    function SearchVarNode(VarName: string): PVirtualNode;
  public
    { Public declarations }
  end;

  procedure ShowFrmCombatPowerSetting;

implementation

uses
  ObjPlayer, UsrEngn, ObjBase;

{$R *.dfm}

type
  PDefNodeData = ^TDefNodeData;
  TDefNodeData = record
    Attrib: TCombatPowerAttrib;
    Value0: Integer;
    Value1: Integer;
    Value2: Integer;
  end;

procedure ShowFrmCombatPowerSetting;
var
  FrmCombatPowerSetting: TFrmCombatPowerSetting;
begin
  FrmCombatPowerSetting := TFrmCombatPowerSetting.Create(nil);
  try
    FrmCombatPowerSetting.ShowModal;
  finally
    FrmCombatPowerSetting.Free;
  end;
end;

type
  TItemDefPropertyEditLink = class(TInterfacedObject, IVTEditLink)
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

  //-------------------------------------------------------------------------------------------------------------
  TItemVarPropertyEditLink = class(TInterfacedObject, IVTEditLink)
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


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

destructor TItemDefPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TItemDefPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CanAdvance: Boolean;
  DefNodeData: PDefNodeData;
  TempValue: Integer;
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
        TempValue := (FEdit as TSpinEditEx).Value;
        FTree.EndEditNode;

        if ssCtrl in Shift then
        begin
          DefNodeData := FTree.GetNodeData(FNode);

          DefNodeData.Value0 := TempValue;
          DefNodeData.Value1 := TempValue;
          DefNodeData.Value2 := TempValue;
        end;

        Abort;
      end;
    VK_UP,
    VK_DOWN:
      begin
        // Consider special cases before finishing edit mode.
        CanAdvance := Shift = [];
        if FEdit is TComboBox then
          CanAdvance := CanAdvance and not TComboBox(FEdit).DroppedDown
        else if FEdit is TSpinEditEx then
          CanAdvance := not (ssAlt in Shift);

        if CanAdvance then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
  end;
end;

procedure TItemDefPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
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

function TItemDefPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemDefPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemDefPropertyEditLink.EndEdit: Boolean;
var
  DefNodeData: PDefNodeData;
  IsChanged: Boolean;
  TempValue: Integer;
begin
  IsChanged := False;
  Result := True;

  DefNodeData := FTree.GetNodeData(FNode);
  if DefNodeData = nil then Exit;
  case FColumn of
    1:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := DefNodeData.Value0 <> TempValue;
        if IsChanged then
          DefNodeData.Value0 := TempValue;
      end;
    2:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := DefNodeData.Value1 <> TempValue;
        if IsChanged then
          DefNodeData.Value1 := TempValue;
      end;
    3:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := DefNodeData.Value2 <> TempValue;
        if IsChanged then
          DefNodeData.Value2 := TempValue;
      end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged and (FTree.Owner is TFrmCombatPowerSetting) then
  begin
    TFrmCombatPowerSetting(FTree.Owner).DoConfigChanged(True);
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemDefPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemDefPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  DefNodeData: PDefNodeData;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;

  // determine what edit type actually is needed
  FEdit.Free;
  FEdit := nil;
  DefNodeData := FTree.GetNodeData(Node);
  case FColumn of
    1, 2, 3:
      begin
        FEdit := TSpinEditEx.Create(nil);
        TSpinEditEx(FEdit).DisableKeyChangeValue := True;
        
        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          MinValue := 0;
          MaxValue := 0;

          if FColumn = 1 then
            Value := DefNodeData.Value0
          else if FColumn = 2 then
            Value := DefNodeData.Value1
          else
            Value := DefNodeData.Value2;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TItemDefPropertyEditLink.ProcessMessage(var Message: TMessage);

begin
  FEdit.WindowProc(Message);
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TItemDefPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

destructor TItemVarPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TItemVarPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CanAdvance: Boolean;
  VarNodeData: PVarNodeData;
  TempValue: Integer;
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

        if (FColumn in [2, 3, 4]) then
        begin
          TempValue := (FEdit as TSpinEditEx).Value;
          FTree.EndEditNode;

          if ssCtrl in Shift then
          begin
            VarNodeData := FTree.GetNodeData(FNode);

            VarNodeData.Value0 := TempValue;
            VarNodeData.Value1 := TempValue;
            VarNodeData.Value2 := TempValue;
          end;
        end
        else
        begin
          FTree.EndEditNode;
        end;

        Abort;
      end;
    VK_UP,
    VK_DOWN:
      begin
        // Consider special cases before finishing edit mode.
        CanAdvance := Shift = [];
        if FEdit is TComboBox then
          CanAdvance := CanAdvance and not TComboBox(FEdit).DroppedDown
        else if FEdit is TSpinEditEx then
          CanAdvance := not (ssAlt in Shift);

        if CanAdvance then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
  end;
end;

procedure TItemVarPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
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

function TItemVarPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemVarPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemVarPropertyEditLink.EndEdit: Boolean;
var
  VarNodeData: PVarNodeData;
  IsChanged: Boolean;
  TempValue: Integer;
  TempS: string;
begin
  IsChanged := False;
  Result := True;

  VarNodeData := FTree.GetNodeData(FNode);
  if VarNodeData = nil then Exit;
  case FColumn of
    1:
      begin
        TempS := (FEdit as TEdit).Text;
        IsChanged := VarNodeData.VarName <> TempS;
        if IsChanged then
          VarNodeData.VarName := TempS;
      end;
    2:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := VarNodeData.Value0 <> TempValue;
        if IsChanged then
          VarNodeData.Value0 := TempValue;
      end;
    3:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := VarNodeData.Value1 <> TempValue;
        if IsChanged then
          VarNodeData.Value1 := TempValue;
      end;
    4:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := VarNodeData.Value2 <> TempValue;
        if IsChanged then
          VarNodeData.Value2 := TempValue;
      end;
    5:
      begin
        TempS := (FEdit as TEdit).Text;
        IsChanged := VarNodeData.Desc <> TempS;
        if IsChanged then
          VarNodeData.Desc := TempS;
      end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged and (FTree.Owner is TFrmCombatPowerSetting) then
  begin
    TFrmCombatPowerSetting(FTree.Owner).DoConfigChanged(False);
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemVarPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

//----------------------------------------------------------------------------------------------------------------------

function TItemVarPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  VarNodeData: PVarNodeData;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;

  // determine what edit type actually is needed
  FEdit.Free;
  FEdit := nil;
  VarNodeData := FTree.GetNodeData(Node);
  case FColumn of
    1, 5:
      begin
        FEdit := TEdit.Create(nil);
        with FEdit as TEdit do
        begin
          Visible := False;
          Parent := Tree;

          if FColumn = 1 then
            Text := VarNodeData.VarName
          else if FColumn = 5 then
            Text := VarNodeData.Desc;
            
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    2, 3, 4:
      begin
        FEdit := TSpinEditEx.Create(nil);
        TSpinEditEx(FEdit).DisableKeyChangeValue := True;

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          MinValue := 0;
          MaxValue := 0;

          if FColumn = 2 then
            Value := VarNodeData.Value0
          else if FColumn = 3 then
            Value := VarNodeData.Value1
          else
            Value := VarNodeData.Value2;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TItemVarPropertyEditLink.ProcessMessage(var Message: TMessage);

begin
  FEdit.WindowProc(Message);
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TItemVarPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

procedure TFrmCombatPowerSetting.FormCreate(Sender: TObject);
var
  Attr: TCombatPowerAttrib;
  Node: PVirtualNode;
  I: Integer;
  DefNodeData: PDefNodeData;
  VarNodeData, VarNodeData2: PVarNodeData;
begin
  vstDefPower.NodeDataSize := SizeOf(TDefNodeData);
  vstVarPower.NodeDataSize := SizeOf(TCombatPowerVarRecord);

  FDefChanged := False;
  FVarChanged := False;

  for Attr := Low(TCombatPowerAttrib) to High(TCombatPowerAttrib) do
  begin
    Node := vstDefPower.AddChild(nil);

    DefNodeData := vstDefPower.GetNodeData(Node);
    DefNodeData.Attrib := Attr;
    DefNodeData.Value0 := g_DefCombatPowerValue[0][Attr];
    DefNodeData.Value1 := g_DefCombatPowerValue[1][Attr];
    DefNodeData.Value2 := g_DefCombatPowerValue[2][Attr];
  end;

  for I := 0 to g_CombatPowerVarMgr.Count - 1 do
  begin
    VarNodeData := g_CombatPowerVarMgr.Items[I];

    Node := vstVarPower.AddChild(nil);
    VarNodeData2 := vstVarPower.GetNodeData(Node);
    VarNodeData2^ := VarNodeData^;
  end;

  chkOpenCombatPowerCalc.Checked := g_Config.boOpenCombatPowerCalc;
  chkOpenCombatPowerVarCalc.Checked := g_Config.boOpenCombatPowerVarCalc;

  pgcMain.ActivePageIndex := 0;

  btnOK.Enabled := False;
end;

procedure TFrmCombatPowerSetting.DoConfigChanged(IsDef: Boolean);
begin
  if IsDef then
    FDefChanged := True
  else
    FVarChanged := True;

  btnOK.Enabled := True;
end;

procedure TFrmCombatPowerSetting.vstDefPowerGetText(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  DefNodeData: PDefNodeData;
begin
  DefNodeData := Sender.GetNodeData(Node);
  if DefNodeData.Attrib in [cpaMaxHP, cpaMaxMP] then
  begin
    case Column of
      0: CellText := CombatPowerAttribNames[DefNodeData.Attrib];
      1: CellText := IntToStr(DefNodeData.Value0) + '‰';
      2: CellText := IntToStr(DefNodeData.Value1) + '‰';
      3: CellText := IntToStr(DefNodeData.Value2) + '‰';
    end;
  end
  else
  begin
    case Column of
      0: CellText := CombatPowerAttribNames[DefNodeData.Attrib];
      1: CellText := IntToStr(DefNodeData.Value0);
      2: CellText := IntToStr(DefNodeData.Value1);
      3: CellText := IntToStr(DefNodeData.Value2);
    end;
  end;
end;

procedure TFrmCombatPowerSetting.vstDefPowerBeforeItemErase(
  Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  ItemRect: TRect; var ItemColor: TColor;
  var EraseAction: TItemEraseAction);
begin
  if Node.Index mod 2 <> 0 then
  begin
    ItemColor := $00FBFBFB;
    EraseAction := eaColor;
  end;
end;

procedure TFrmCombatPowerSetting.vstDefPowerCreateEditor(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TItemDefPropertyEditLink.Create;
end;

procedure TFrmCombatPowerSetting.vstDefPowerEditing(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var Allowed: Boolean);
begin
  Allowed := (Node <> nil) and (Column <> 0);
end;

procedure TFrmCombatPowerSetting.vstDefPowerNodeClick(
  Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  DefNodeData: PDefNodeData;
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn > 0) then
  begin
    DefNodeData := Sender.GetNodeData(HitInfo.HitNode);

    if DefNodeData <> nil then
    begin
      PostMessage(Self.Handle, WM_STARTEDITING_ITEMDEF, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
    end;
  end;
end;

procedure TFrmCombatPowerSetting.WMStartEditingItemDef(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstDefPower.EditNode(Node, Message.LParam);
end;

procedure TFrmCombatPowerSetting.btnOKClick(Sender: TObject);
var
  Node: PVirtualNode;
  VarName: string;
  DefNodeData: PDefNodeData;
  VarNodeData: PVarNodeData;
begin
  if vstDefPower.IsEditing then
    vstDefPower.EndEditNode;

  if vstVarPower.IsEditing then
    vstVarPower.EndEditNode;

  if FDefChanged then
  begin
    Node := vstDefPower.GetFirst();
    while Node <> nil do
    begin
      DefNodeData := vstDefPower.GetNodeData(Node);

      g_DefCombatPowerValue[0][DefNodeData.Attrib] := DefNodeData.Value0;
      g_DefCombatPowerValue[1][DefNodeData.Attrib] := DefNodeData.Value1;
      g_DefCombatPowerValue[2][DefNodeData.Attrib] := DefNodeData.Value2;
      
      Node := vstDefPower.GetNext(Node);
    end;
    SaveDefCombatPowerConfig;
  end;


  Node := vstVarPower.GetFirst();
  while Node <> nil do
  begin
    VarNodeData := vstVarPower.GetNodeData(Node);

    VarName := Trim(VarNodeData.VarName);

    if (VarName = '') then
    begin
      ShowMessage('变量名不能为空');
      vstVarPower.FocusedNode := Node;
      vstVarPower.ScrollIntoView(Node, True);
      Exit;
    end;

     if not CheckCombatPowerVarSupport(VarName) then
    begin
      ShowMessage('不支持的变量名');
      vstVarPower.FocusedNode := Node;
      vstVarPower.ScrollIntoView(Node, True);
      Exit;
    end;

    if ((VarNodeData.Value0 = 0) and (VarNodeData.Value1 = 0) and (VarNodeData.Value2 = 0)) then
    begin
      ShowMessage('战斗力+不能全为0');
      vstVarPower.FocusedNode := Node;
      vstVarPower.ScrollIntoView(Node, True);
      Exit;
    end;

    Node := vstVarPower.GetNext(Node);
  end;

  if FVarChanged then
  begin
    g_CombatPowerVarMgr.Lock;
    try
      g_CombatPowerVarMgr.Clear;
      
      Node := vstVarPower.GetFirst();
      while Node <> nil do
      begin
        VarNodeData := vstVarPower.GetNodeData(Node);
        VarName := Trim(VarNodeData.VarName);

        g_CombatPowerVarMgr.Add(VarName, VarNodeData.Value0, VarNodeData.Value1, VarNodeData.Value2, VarNodeData.Desc);

        Node := vstVarPower.GetNext(Node);
      end;
    finally
      g_CombatPowerVarMgr.UnLock;
    end;

    g_CombatPowerVarMgr.SaveConfig;
  end;

  btnOK.Enabled := False;
  
  FDefChanged := False;
  FVarChanged := False;
end;

procedure TFrmCombatPowerSetting.vstDefPowerKeyDown(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if vstDefPower.FocusedNode = nil then Exit;

  if Key = VK_RETURN then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_ITEMDEF, WPARAM(vstDefPower.FocusedNode), vstDefPower.FocusedColumn);
  end;
end;

procedure TFrmCombatPowerSetting.vstDefPowerChange(
  Sender: TBaseVirtualTree; Node: PVirtualNode);
begin
  if Node <> nil then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_ITEMDEF, WPARAM(Node), vstDefPower.FocusedColumn);
  end;
end;

procedure TFrmCombatPowerSetting.WMStartEditingItemVar(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstVarPower.EditNode(Node, Message.LParam);
end;

procedure TFrmCombatPowerSetting.btnAddVarClick(Sender: TObject);
var
  VarName: string;
  IsBatch: Boolean;
  I, VarIndex, VarCount, VarEndIndex: Integer;
  Node: PVirtualNode;
  VarNodeData: PVarNodeData;
begin
  if not ShowFrmCombatPowerAddVar(VarName, IsBatch, VarIndex, VarCount) then Exit;

  if not IsBatch then
  begin
    Node := SearchVarNode(VarName);

    if Node = nil then
    begin
      Node := vstVarPower.AddChild(nil);

      VarNodeData := vstVarPower.GetNodeData(Node);
      VarNodeData.VarName := VarName;
    end
    else
    begin
      vstVarPower.FocusedNode := Node;
      vstVarPower.ScrollIntoView(Node, True)
    end;
  end
  else
  begin
    VarEndIndex := VarIndex + VarCount - 1;
    for I := VarIndex to VarEndIndex do
    begin
      Node := SearchVarNode(VarName + IntToStr(I));
      if Node = nil then
      begin
        Node := vstVarPower.AddChild(nil);

        VarNodeData := vstVarPower.GetNodeData(Node);
        VarNodeData.VarName := VarName + IntToStr(I);
      end;
    end;
  end;

  DoConfigChanged(False);
end;

function TFrmCombatPowerSetting.SearchVarNode(VarName: string): PVirtualNode;
var
  Node: PVirtualNode;
  VarNodeData: PVarNodeData;
begin
  Result := nil;
  Node := vstVarPower.GetFirst();
  while Node <> nil do
  begin
    VarNodeData := vstVarPower.GetNodeData(Node);

    if SameText(VarNodeData.VarName, VarName) then
    begin
      Result := Node;
      Break;
    end;

    Node := vstVarPower.GetNext(Node);
  end;
end;

procedure TFrmCombatPowerSetting.vstVarPowerGetText(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  VarNodeData: PVarNodeData;
begin
  VarNodeData := Sender.GetNodeData(Node);
  case Column of
    0: CellText := IntToStr(Node.Index);
    1: CellText := VarNodeData.VarName;
    2: CellText := IntToStr(VarNodeData.Value0);
    3: CellText := IntToStr(VarNodeData.Value1);
    4: CellText := IntToStr(VarNodeData.Value2);
    5: CellText := VarNodeData.Desc;
  end;
end;

procedure TFrmCombatPowerSetting.vstVarPowerCreateEditor(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TItemVarPropertyEditLink.Create;
end;

procedure TFrmCombatPowerSetting.vstVarPowerEditing(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var Allowed: Boolean);
begin
  Allowed := (Node <> nil) and (Column > 0);
end;

procedure TFrmCombatPowerSetting.vstVarPowerKeyDown(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if vstVarPower.FocusedNode = nil then Exit;

  if Key = VK_RETURN then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_ITEMVAR, WPARAM(vstVarPower.FocusedNode), vstVarPower.FocusedColumn);
  end;
end;

procedure TFrmCombatPowerSetting.vstVarPowerNodeClick(
  Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  VarNodeData: PVarNodeData;
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn > 0) then
  begin
    VarNodeData := Sender.GetNodeData(HitInfo.HitNode);

    if VarNodeData <> nil then
    begin
      PostMessage(Self.Handle, WM_STARTEDITING_ITEMVAR, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
    end;
  end;
end;

procedure TFrmCombatPowerSetting.vstVarPowerChange(
  Sender: TBaseVirtualTree; Node: PVirtualNode);
begin
  if Node <> nil then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_ITEMVAR, WPARAM(Node), vstVarPower.FocusedColumn);
  end;
end;

procedure TFrmCombatPowerSetting.vstVarPowerKeyUp(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if (ssCtrl in Shift) then
  begin
    if Key = VK_INSERT then
    begin
      vstVarPower.AddChild(nil);
      DoConfigChanged(False);
    end
    else
    if Key = VK_DELETE then
    begin
      vstVarPower.DeleteSelectedNodes;
      DoConfigChanged(False);
    end;
  end;
end;

procedure TFrmCombatPowerSetting.btnDelVarClick(Sender: TObject);
begin
  vstVarPower.DeleteSelectedNodes;
  DoConfigChanged(False);
end;

procedure TFrmCombatPowerSetting.chkOpenCombatPowerCalcClick(Sender: TObject);
begin
  g_Config.boOpenCombatPowerCalc := chkOpenCombatPowerCalc.Checked;
  Config.WriteBool('Setup', 'OpenCombatPowerCalc', g_Config.boOpenCombatPowerCalc);
end;

procedure TFrmCombatPowerSetting.chkOpenCombatPowerVarCalcClick(Sender: TObject);
begin
  g_Config.boOpenCombatPowerVarCalc := chkOpenCombatPowerVarCalc.Checked;
  Config.WriteBool('Setup', 'OpenCombatPowerVarCalc', g_Config.boOpenCombatPowerVarCalc);
end;

procedure TFrmCombatPowerSetting.vstVarPowerFreeNode(
  Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  VarNodeData: PVarNodeData;
begin
  VarNodeData := Sender.GetNodeData(Node);
  VarNodeData.Desc := '';
end;

procedure TFrmCombatPowerSetting.edtItemSearchChange(Sender: TObject);
var
  Node: PVirtualNode;
  VarNodeData: PVarNodeData;
  SearchText: string;
begin
  SearchText := UpperCase(edtItemSearch.Text);
  if Length(SearchText) = 0 then
  begin
    vstVarPower.BeginUpdate;
    try
      Node := vstVarPower.GetFirst();
      while Node <> nil do
      begin
        vstVarPower.IsVisible[Node] := True;
        Node := vstVarPower.GetNext(Node);
      end;
    finally
      vstVarPower.EndUpdate;
    end;
  end
  else
  begin
    vstVarPower.BeginUpdate;
    try
      Node := vstVarPower.GetFirst();
      while Node <> nil do
      begin
        VarNodeData := vstVarPower.GetNodeData(Node);

        if Pos(SearchText, UpperCase(VarNodeData.VarName)) > 0 then
        begin
          vstVarPower.IsVisible[Node] := True;
        end
        else
        begin
          vstVarPower.IsVisible[Node] := False;
        end;

        Node := vstVarPower.GetNext(Node);
      end;
    finally
      vstVarPower.EndUpdate;
    end;
  end;
end;

procedure TFrmCombatPowerSetting.btnRecalHumanCombatPowerClick(
  Sender: TObject);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  UserEngine.m_PlayObjectList.LockR(2911);
  try
    for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boDummyObject) then
        begin
          RecalcPlayCombatPower(PlayObject);
          if PlayObject.m_MyHero <> nil then
          begin
            RecalcPlayCombatPower(TSmartObject(PlayObject.m_MyHero));
          end;
        end;
      end;
    end;
  finally
    UserEngine.m_PlayObjectList.UnLockR;
  end;
end;

procedure TFrmCombatPowerSetting.pmCopyPopup(Sender: TObject);
begin
  if pgcMain.ActivePageIndex = 0  then
  begin
    case vstDefPower.Header.Columns.ClickIndex of
      1:
        begin
          mniCopy1.Visible := True;
          mniCopy2.Visible := True;
          mniCopy1.Caption := '从战士复制到法师为0的值';
          mniCopy2.Caption := '从战士复制到道士为0的值';
        end;
      2:
        begin
          mniCopy1.Visible := True;
          mniCopy2.Visible := True;
          mniCopy1.Caption := '从法师复制到战士为0的值';
          mniCopy2.Caption := '从法师复制到道士为0的值';
        end;
      3:
        begin
          mniCopy1.Visible := True;
          mniCopy2.Visible := True;
          mniCopy1.Caption := '从道士复制到战士为0的值';
          mniCopy2.Caption := '从道士复制到法师为0的值';
        end;
    else
      mniCopy1.Visible := False;
      mniCopy2.Visible := False;
    end;
  end
  else
  begin
    case vstVarPower.Header.Columns.ClickIndex of
      2:
        begin
          mniCopy1.Visible := True;
          mniCopy2.Visible := True;
          mniCopy1.Caption := '从战士复制到法师为0的值';
          mniCopy2.Caption := '从战士复制到道士为0的值';
        end;
      3:
        begin
          mniCopy1.Visible := True;
          mniCopy2.Visible := True;
          mniCopy1.Caption := '从法师复制到战士为0的值';
          mniCopy2.Caption := '从法师复制到道士为0的值';
        end;
      4:
        begin
          mniCopy1.Visible := True;
          mniCopy2.Visible := True;
          mniCopy1.Caption := '从道士复制到战士为0的值';
          mniCopy2.Caption := '从道士复制到法师为0的值';
        end;
    else
      mniCopy1.Visible := False;
      mniCopy2.Visible := False;
    end;
  end;
end;

procedure TFrmCombatPowerSetting.mniCopy1Click(Sender: TObject);
var
  Node: PVirtualNode;
  DefNodeData: PDefNodeData;
  VarNodeData: PVarNodeData;
  IsChanged: Boolean;
begin
  IsChanged := False;

  if pgcMain.ActivePageIndex = 0  then
  begin
    case vstDefPower.Header.Columns.ClickIndex of
      1:          // 从战士复制到法师为0的值
        begin
          Node := vstDefPower.GetFirst();
          while Node <> nil do
          begin
            DefNodeData := vstDefPower.GetNodeData(Node);

            if (DefNodeData.Value0 <> 0) and (DefNodeData.Value1 = 0) then
            begin
              DefNodeData.Value1 := DefNodeData.Value0;
              IsChanged := True;
            end;

            Node := vstDefPower.GetNext(Node);
          end;
        end;
      2:          // 从法师复制到战士为0的值
        begin
          Node := vstDefPower.GetFirst();
          while Node <> nil do
          begin
            DefNodeData := vstDefPower.GetNodeData(Node);

            if (DefNodeData.Value1 <> 0) and (DefNodeData.Value0 = 0) then
            begin
              DefNodeData.Value0 := DefNodeData.Value1;
              IsChanged := True;
            end;

            Node := vstDefPower.GetNext(Node);
          end;
        end;
      3:            // 从道士复制到战士为0的值
        begin
          Node := vstDefPower.GetFirst();
          while Node <> nil do
          begin
            DefNodeData := vstDefPower.GetNodeData(Node);

            if (DefNodeData.Value2 <> 0) and (DefNodeData.Value0 = 0) then
            begin
              DefNodeData.Value0 := DefNodeData.Value2;
              IsChanged := True;
            end;

            Node := vstDefPower.GetNext(Node);
          end;
        end;
    end;

    if IsChanged then
    begin
      vstDefPower.Invalidate;
      DoConfigChanged(True);
    end;
  end
  else
  begin
    case vstVarPower.Header.Columns.ClickIndex of
      2:          // 从战士复制到法师为0的值
        begin
          Node := vstVarPower.GetFirst();
          while Node <> nil do
          begin
            VarNodeData := vstVarPower.GetNodeData(Node);

            if (VarNodeData.Value0 <> 0) and (VarNodeData.Value1 = 0) then
            begin
              VarNodeData.Value1 := VarNodeData.Value0;
              IsChanged := True;
            end;

            Node := vstVarPower.GetNext(Node);
          end;
        end;
      3:          // 从法师复制到战士为0的值
        begin
          Node := vstVarPower.GetFirst();
          while Node <> nil do
          begin
            VarNodeData := vstVarPower.GetNodeData(Node);

            if (VarNodeData.Value1 <> 0) and (VarNodeData.Value0 = 0) then
            begin
              VarNodeData.Value0 := VarNodeData.Value1;
              IsChanged := True;
            end;

            Node := vstVarPower.GetNext(Node);
          end;
        end;
      4:            // 从道士复制到战士为0的值
        begin
          Node := vstVarPower.GetFirst();
          while Node <> nil do
          begin
            VarNodeData := vstVarPower.GetNodeData(Node);

            if (VarNodeData.Value2 <> 0) and (VarNodeData.Value0 = 0) then
            begin
              VarNodeData.Value0 := VarNodeData.Value2;
              IsChanged := True;
            end;

            Node := vstVarPower.GetNext(Node);
          end;
        end;
    end;

    if IsChanged then
    begin
      vstVarPower.Invalidate;
      DoConfigChanged(False);
    end;
  end;
end;

procedure TFrmCombatPowerSetting.mniCopy2Click(Sender: TObject);
var
  Node: PVirtualNode;
  DefNodeData: PDefNodeData;
  VarNodeData: PVarNodeData;
  IsChanged: Boolean;
begin
  IsChanged := False;

  if pgcMain.ActivePageIndex = 0  then
  begin
    case vstDefPower.Header.Columns.ClickIndex of
      1:          // 从战士复制到道士为0的值
        begin
          Node := vstDefPower.GetFirst();
          while Node <> nil do
          begin
            DefNodeData := vstDefPower.GetNodeData(Node);

            if (DefNodeData.Value0 <> 0) and (DefNodeData.Value2 = 0) then
            begin
              DefNodeData.Value2 := DefNodeData.Value0;
              IsChanged := True;
            end;

            Node := vstDefPower.GetNext(Node);
          end;
        end;
      2:          // 从法师复制到道士为0的值
        begin
          Node := vstDefPower.GetFirst();
          while Node <> nil do
          begin
            DefNodeData := vstDefPower.GetNodeData(Node);

            if (DefNodeData.Value1 <> 0) and (DefNodeData.Value2 = 0) then
            begin
              DefNodeData.Value2 := DefNodeData.Value1;
              IsChanged := True;
            end;

            Node := vstDefPower.GetNext(Node);
          end;
        end;
      3:            // 从道士复制到法师为0的值
        begin
          Node := vstDefPower.GetFirst();
          while Node <> nil do
          begin
            DefNodeData := vstDefPower.GetNodeData(Node);

            if (DefNodeData.Value2 <> 0) and (DefNodeData.Value1 = 0) then
            begin
              DefNodeData.Value1 := DefNodeData.Value2;
              IsChanged := True;
            end;

            Node := vstDefPower.GetNext(Node);
          end;
        end;
    end;

    if IsChanged then
    begin
      vstDefPower.Invalidate;
      DoConfigChanged(True);
    end;
  end
  else
  begin
    case vstVarPower.Header.Columns.ClickIndex of
      2:          // 从战士复制到道士为0的值
        begin
          Node := vstVarPower.GetFirst();
          while Node <> nil do
          begin
            VarNodeData := vstVarPower.GetNodeData(Node);

            if (VarNodeData.Value0 <> 0) and (VarNodeData.Value2 = 0) then
            begin
              VarNodeData.Value2 := VarNodeData.Value0;
              IsChanged := True;
            end;

            Node := vstVarPower.GetNext(Node);
          end;
        end;
      3:          // 从法师复制到道士为0的值
        begin
          Node := vstVarPower.GetFirst();
          while Node <> nil do
          begin
            VarNodeData := vstVarPower.GetNodeData(Node);

            if (VarNodeData.Value1 <> 0) and (VarNodeData.Value2 = 0) then
            begin
              VarNodeData.Value2 := VarNodeData.Value1;
              IsChanged := True;
            end;

            Node := vstVarPower.GetNext(Node);
          end;
        end;
      4:            // 从道士复制到法师为0的值
        begin
          Node := vstVarPower.GetFirst();
          while Node <> nil do
          begin
            VarNodeData := vstVarPower.GetNodeData(Node);

            if (VarNodeData.Value2 <> 0) and (VarNodeData.Value1 = 0) then
            begin
              VarNodeData.Value1 := VarNodeData.Value2;
              IsChanged := True;
            end;

            Node := vstVarPower.GetNext(Node);
          end;
        end;
    end;

    if IsChanged then
    begin
      vstVarPower.Invalidate;
      DoConfigChanged(False);
    end;
  end;
end;

procedure TFrmCombatPowerSetting.vstDefPowerGetHint(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var LineBreakStyle: TVTTooltipLineBreakStyle; var HintText: string);
var
  DefNodeData: PDefNodeData;
begin
  DefNodeData := Sender.GetNodeData(Node);
  if DefNodeData.Attrib in [cpaMaxHP, cpaMaxMP] then
  begin
    HintText := '1000点MaxHP/MaxMP增加多少点战斗力'
  end;
end;

end.
