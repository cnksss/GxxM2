unit uFrmCustomNpc;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, Spin, IniFiles, Grobal2,
  Menus, ExtCtrls, SpinEditEx, VirtualTrees, EDCode, uCustomNpcUtils, M2Threads, CheckUnit, System.ImageList, Vcl.ImgList;

const
  WM_STARTEDITING_NPC = WM_USER + 779;

type
  TFrmCustomNpc = class(TForm)
    dlgSaveNpcs: TSaveDialog;
    ilCheck: TImageList;
    pnlNpcClient: TPanel;
    grp10: TGroupBox;
    vstCustomNpc: TVirtualStringTree;
    pnlNpc: TPanel;
    GroupBox1: TGroupBox;
    grp11: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    seNpcHPBgOffsetX: TSpinEditEx;
    seNpcHPBgOffsetY: TSpinEditEx;
    GroupBox2: TGroupBox;
    Label3: TLabel;
    Label4: TLabel;
    seNpcHPOffsetX: TSpinEditEx;
    seNpcHPOffsetY: TSpinEditEx;
    GroupBox3: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    seNpcHPTextOffsetX: TSpinEditEx;
    seNpcHPTextOffsetY: TSpinEditEx;
    GroupBox4: TGroupBox;
    Label7: TLabel;
    Label8: TLabel;
    cbbNpcHPFile: TComboBox;
    seNpcHPStartIndex: TSpinEditEx;
    vstNpcAction: TVirtualStringTree;
    GroupBox5: TGroupBox;
    Label9: TLabel;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    seNpcCalcStartIndex: TSpinEditEx;
    seNpcCalcPlayCount: TSpinEditEx;
    seNpcCalcEmptyCount: TSpinEditEx;
    seNpcCalcDirCount: TSpinEditEx;
    btnNpcCalcStand: TButton;
    btnNpcCalcStandEffect: TButton;
    btnNpcCalcHit: TButton;
    btnNpcCalcHitEffect: TButton;
    grp12: TGroupBox;
    Label13: TLabel;
    btnNpcFileStandEffect: TButton;
    btnNpcFileActionEffect: TButton;
    cbbNpcBatchFile: TComboBox;
    btnNpcFileStand: TButton;
    btnNpcFileAction: TButton;
    GroupBox6: TGroupBox;
    Label14: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    cbbNpcKeepPlayFile: TComboBox;
    seNpcKeepPlayIndex: TSpinEditEx;
    seNpcKeepPlayCount: TSpinEditEx;
    seNpcKeepPlayTime: TSpinEditEx;
    chkNpcKeepPlayBlendDraw: TCheckBox;
    seKeepPlayOffsetX: TSpinEditEx;
    seKeepPlayOffsetY: TSpinEditEx;
    GroupBox7: TGroupBox;
    Label20: TLabel;
    seNpcBatchTime: TSpinEditEx;
    btnNpcTimeStand: TButton;
    btnNpcTimeAction: TButton;
    grp13: TGroupBox;
    lbl27: TLabel;
    Label21: TLabel;
    Label22: TLabel;
    Label23: TLabel;
    cbbNpcStandDrawMode: TComboBox;
    cbbNpcStandEffectDrawMode: TComboBox;
    cbbNpcActionDrawMode: TComboBox;
    cbbNpcActionEffectDrawMode: TComboBox;
    grp14: TGroupBox;
    lstNpcDrawOrder: TListBox;
    btnNpcMoveTop: TButton;
    btnNpcMoveBottom: TButton;
    pnlNpcBottom: TPanel;
    lbl28: TLabel;
    lbl30: TLabel;
    lbl31: TLabel;
    lbl32: TLabel;
    lbl29: TLabel;
    btnSaveNpc: TButton;
    btnSaveNpcToFile: TButton;
    chkSendCustomNPCConfig: TCheckBox;
    seCustomNpcMoveTime: TSpinEditEx;
    procedure FormCreate(Sender: TObject);
    procedure vstCustomNpcGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstCustomNpcGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstCustomNpcNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure vstNpcActionChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstNpcActionCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
      out EditLink: IVTEditLink);
    procedure vstNpcActionDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
    procedure vstNpcActionEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstNpcActionGetHint(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
      var LineBreakStyle: TVTTooltipLineBreakStyle; var HintText: string);
    procedure vstNpcActionGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstNpcActionGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstNpcActionNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure btnNpcFileStandClick(Sender: TObject);
    procedure btnNpcCalcStandClick(Sender: TObject);
    procedure btnSaveNpcClick(Sender: TObject);
    procedure btnSaveNpcToFileClick(Sender: TObject);
    procedure cbbNpcHPFileChange(Sender: TObject);
    procedure seNpcHPStartIndexChange(Sender: TObject);
    procedure seNpcHPBgOffsetXChange(Sender: TObject);
    procedure seNpcHPBgOffsetYChange(Sender: TObject);
    procedure seNpcHPOffsetXChange(Sender: TObject);
    procedure seNpcHPOffsetYChange(Sender: TObject);
    procedure seNpcHPTextOffsetXChange(Sender: TObject);
    procedure seNpcHPTextOffsetYChange(Sender: TObject);
    procedure chkSendCustomNPCConfigClick(Sender: TObject);
    procedure vstNpcActionBeforeCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
      Column: TColumnIndex; CellPaintMode: TVTCellPaintMode; CellRect: TRect; var ContentRect: TRect);
    procedure btnNpcTimeStandClick(Sender: TObject);
    procedure cbbNpcStandDrawModeChange(Sender: TObject);
    procedure cbbNpcStandEffectDrawModeChange(Sender: TObject);
    procedure cbbNpcActionDrawModeChange(Sender: TObject);
    procedure cbbNpcActionEffectDrawModeChange(Sender: TObject);
    procedure cbbNpcKeepPlayFileChange(Sender: TObject);
    procedure seNpcKeepPlayIndexChange(Sender: TObject);
    procedure seNpcKeepPlayCountChange(Sender: TObject);
    procedure seNpcKeepPlayTimeChange(Sender: TObject);
    procedure chkNpcKeepPlayBlendDrawClick(Sender: TObject);
    procedure btnNpcMoveTopClick(Sender: TObject);
    procedure seKeepPlayOffsetXChange(Sender: TObject);
    procedure seKeepPlayOffsetYChange(Sender: TObject);
    procedure seCustomNpcMoveTimeChange(Sender: TObject);
    procedure vstCustomNpcDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
  private
    { Private declarations }

    FCurrentNpcCustomConfig: TCustomNpcConfig;
    FIsNpcChanged: Boolean;

    procedure SetNpcConfigChanged(IsChanged: Boolean = True);
    procedure WMStartEditingNpc(var Message: TMessage); message WM_STARTEDITING_NPC;
  public
    procedure Open;
    { Public declarations }
  end;

procedure ShowFrmCustomNpc;

implementation

uses
  M2Share, UsrEngn;

procedure ShowFrmCustomNpc;
var
  FrmCustomNpc: TFrmCustomNpc;
begin
  FrmCustomNpc := TFrmCustomNpc.Create(nil);
  try
    FrmCustomNpc.Open;
    FrmCustomNpc.ShowModal;
  finally
    FrmCustomNpc.Free;
  end;
end;

{$R *.dfm}

type
  PNpcConfigNodeData = ^TNpcConfigNodeData;

  TNpcConfigNodeData = record
    Config: TCustomNpcConfig;
  end;

  PNpcNodeData = ^TNpcNodeData;

  TNpcNodeData = record
    ActionType: TNpcActionType;
    Action: PNpcDirAction;
  end;

type
  TNpcPropertyEditLink = class(TInterfacedObject, IVTEditLink)
  private
    FEdit: TWinControl; // One of the property editor classes.
    FTree: TVirtualStringTree; // A back reference to the tree calling.
    FNode: PVirtualNode; // The node being edited.
    FColumn: Integer; // The column of the node being edited.
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

destructor TNpcPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

procedure TNpcPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CanAdvance: Boolean;
begin
  CanAdvance := True;

  case Key of
    VK_ESCAPE:
      begin
        Key := 0; // ESC will be handled in EditKeyUp()
      end;
    VK_RETURN:
      if CanAdvance then
      begin
        Key := 0;
        FTree.EndEditNode;
        Abort;
      end;
    VK_UP, VK_DOWN:
      begin
        // Consider special cases before finishing edit mode.
        CanAdvance := Shift = [];
        if FEdit is TComboBox then
          CanAdvance := CanAdvance and not TComboBox(FEdit).DroppedDown
        else if FEdit is TSpinEditEx then
          CanAdvance := True;
        if CanAdvance then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
  end;
end;

procedure TNpcPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_ESCAPE:
      begin
        FTree.CancelEditNode;
        Key := 0;
      end; // VK_ESCAPE
  end; // case
end;

function TNpcPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

function TNpcPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

function TNpcPropertyEditLink.EndEdit: Boolean;
var
  NodeData: PNpcNodeData;
  TempValue: Integer;
  IsChanged: Boolean;
begin
  Result := True;
  IsChanged := False;

  NodeData := FTree.GetNodeData(FNode);

  if NodeData.ActionType = atStand then
  begin
    if FEdit is TComboBox then
    begin
      TempValue := (FEdit as TComboBox).ItemIndex;
      case FColumn of
        2:
          if NodeData.Action.Std_File <> TempValue then
          begin
            NodeData.Action.Std_File := TempValue;
            IsChanged := True;
          end;
        6:
          if NodeData.Action.Std_EffFile <> TempValue then
          begin
            NodeData.Action.Std_EffFile := TempValue;
            IsChanged := True;
          end;
      end;
    end
    else if FEdit is TSpinEditEx then
    begin
      TempValue := (FEdit as TSpinEditEx).Value;
      case FColumn of
        3:
          if NodeData.Action.Std_Index <> TempValue then
          begin
            NodeData.Action.Std_Index := TempValue;
            IsChanged := True;
          end;
        4:
          if NodeData.Action.Std_Count <> TempValue then
          begin
            NodeData.Action.Std_Count := TempValue;
            IsChanged := True;
          end;
        5:
          if NodeData.Action.Std_Time <> TempValue then
          begin
            NodeData.Action.Std_Time := TempValue;
            IsChanged := True;
          end;
        7:
          if NodeData.Action.Std_EffIndex <> TempValue then
          begin
            NodeData.Action.Std_EffIndex := TempValue;
            IsChanged := True;
          end;
        {
          8:
          if NodeData.Action.Std_EffCount <> TempValue then
          begin
          NodeData.Action.Std_EffCount := TempValue;
          IsChanged := True;
          end;
          9:
          if NodeData.Action.Std_EffTime <> TempValue then
          begin
          NodeData.Action.Std_EffTime := TempValue;
          IsChanged := True;
          end;
        }
      end;
    end;
  end
  else if NodeData.ActionType = atAction then
  begin
    if FEdit is TComboBox then
    begin
      TempValue := (FEdit as TComboBox).ItemIndex;
      case FColumn of
        2:
          if NodeData.Action.Act_File <> TempValue then
          begin
            NodeData.Action.Act_File := TempValue;
            IsChanged := True;
          end;
        6:
          if NodeData.Action.Act_EffFile <> TempValue then
          begin
            NodeData.Action.Act_EffFile := TempValue;
            IsChanged := True;
          end;
      end;
    end
    else if FEdit is TSpinEditEx then
    begin
      TempValue := (FEdit as TSpinEditEx).Value;
      case FColumn of
        3:
          if NodeData.Action.Act_Index <> TempValue then
          begin
            NodeData.Action.Act_Index := TempValue;
            IsChanged := True;
          end;
        4:
          if NodeData.Action.Act_Count <> TempValue then
          begin
            NodeData.Action.Act_Count := TempValue;
            IsChanged := True;
          end;
        5:
          if NodeData.Action.Act_Time <> TempValue then
          begin
            NodeData.Action.Act_Time := TempValue;
            IsChanged := True;
          end;
        7:
          if NodeData.Action.Act_EffIndex <> TempValue then
          begin
            NodeData.Action.Act_EffIndex := TempValue;
            IsChanged := True;
          end;
        {
          8:
          if NodeData.Action.Act_EffCount <> TempValue then
          begin
          NodeData.Action.Act_EffCount := TempValue;
          IsChanged := True;
          end;
          9:
          if NodeData.Action.Act_EffTime <> TempValue then
          begin
          NodeData.Action.Act_EffTime := TempValue;
          IsChanged := True;
          end;
        }
      end;
    end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged then
  begin
    if (FTree.Owner is TFrmCustomNpc) then
    begin
      TFrmCustomNpc(FTree.Owner).SetNpcConfigChanged();
    end;
  end;
end;

function TNpcPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

function TNpcPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  I: Integer;
  NodeData: PNpcNodeData;
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
    3, 4, 5, 7, 8, 9:
      begin
        FEdit := TSpinEditEx.Create(nil);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          if FColumn in [3, 7] then
          begin
            MinValue := -1;
            MaxValue := High(Smallint);
          end
          else if FColumn = 5 then
          begin
            MinValue := 50;
            MaxValue := High(Smallint);
          end
          else
          begin
            MinValue := 0;
            MaxValue := High(Word);
          end;

          if NodeData.ActionType = atStand then
          begin
            case FColumn of
              3:
                Value := NodeData.Action.Std_Index;
              4:
                Value := NodeData.Action.Std_Count;
              5:
                Value := NodeData.Action.Std_Time;
              7:
                Value := NodeData.Action.Std_EffIndex;
              // 8: Value := NodeData.Action.Std_EffCount;
              // 9: Value := NodeData.Action.Std_EffTime;
            end;
          end
          else if NodeData.ActionType = atAction then
          begin
            case FColumn of
              3:
                Value := NodeData.Action.Act_Index;
              4:
                Value := NodeData.Action.Act_Count;
              5:
                Value := NodeData.Action.Act_Time;
              7:
                Value := NodeData.Action.Act_EffIndex;
              // 8: Value := NodeData.Action.Act_EffCount;
              // 9: Value := NodeData.Action.Act_EffTime;
            end;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    2, 6:
      begin
        FEdit := TComboBox.Create(nil);

        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;

          for I := 0 to g_EffectImageList.Count - 1 do
            Items.Add(g_EffectImageList.Strings[I]);

          if NodeData.ActionType = atStand then
          begin
            case FColumn of
              2:
                begin
                  ItemIndex := NodeData.Action.Std_File;
                end;
              6:
                begin
                  ItemIndex := NodeData.Action.Std_EffFile;
                end;
            end;
          end
          else if NodeData.ActionType = atAction then
          begin
            case FColumn of
              2:
                begin
                  ItemIndex := NodeData.Action.Act_File;
                end;
              6:
                begin
                  ItemIndex := NodeData.Action.Act_EffFile;
                end;
            end;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

procedure TNpcPropertyEditLink.ProcessMessage(var Message: TMessage);
begin
  FEdit.WindowProc(Message);
end;

procedure TNpcPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

procedure SetControlEnabled(WinControl: TWinControl; Value: Boolean);
var
  I: Integer;
  Ctrl: TControl;
  WinCtrl: TWinControl;
begin
  for I := 0 to WinControl.ControlCount - 1 do
  begin
    Ctrl := WinControl.Controls[I];
    if Ctrl is TWinControl then
    begin
      WinCtrl := Ctrl as TWinControl;
      if (Ctrl is TTabSheet) or (Ctrl is TPanel) or (Ctrl is TGroupBox) then
        SetControlEnabled(WinCtrl, Value)
      else
        WinCtrl.Enabled := Value;
    end;
  end;
end;

{ TFrmCustomNpc }
procedure TFrmCustomNpc.Open;
var
  I: Integer;
  Node: PVirtualNode;
  CustomNpcConfig: TCustomNpcConfig;
  NpcConfigNodeData: PNpcConfigNodeData;
begin
  FIsNpcChanged := False;

  // 自定义NPC chongchong 2016-03-20
  if g_MultiThreadRun then
    UserEngine.m_CustomNpcList.LockR(4);
  try
    for I := 0 to UserEngine.m_CustomNpcList.Count - 1 do
    begin
      CustomNpcConfig := UserEngine.m_CustomNpcList.Items[I];

      Node := vstCustomNpc.AddChild(nil);
      NpcConfigNodeData := vstCustomNpc.GetNodeData(Node);
      NpcConfigNodeData.Config := CustomNpcConfig;
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomNpcList.UnLockR;
  end;

  chkSendCustomNPCConfig.Checked := g_Config.boSendCustomNpcConfig;
  seCustomNpcMoveTime.Value := g_Config.dwCustomNpcMoveTime;
end;

procedure TFrmCustomNpc.FormCreate(Sender: TObject);
var
  I: Integer;
  DrawMode: TCustomDrawMode;
begin
  cbbNpcHPFile.Clear;
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbNpcHPFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbNpcKeepPlayFile.Clear;
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbNpcKeepPlayFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbNpcStandDrawMode.Items.Clear;
  cbbNpcStandEffectDrawMode.Items.Clear;
  cbbNpcActionDrawMode.Items.Clear;
  cbbNpcActionEffectDrawMode.Items.Clear;

  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
  begin
    cbbNpcStandDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);
    cbbNpcStandEffectDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);
    cbbNpcActionDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);
    cbbNpcActionEffectDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);
  end;

  cbbNpcBatchFile.Clear;
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbNpcBatchFile.Items.Add(g_EffectImageList.Strings[I]);

  {
    SL := TStringList.Create;
    try
    for I := 0 to g_MapManager.Count - 1 do
    begin
    Envir := g_MapManager.Items[I];
    if Envir.m_boMirror or Envir.m_boFB then Continue;

    SL.Add(Envir.sMapName);
    end;

    SL.Sort;
    SL.Insert(0, '*');

    cbbMaps.Items.Text := SL.Text;
    finally
    SL.Free;
    end;
  }

  SetControlEnabled(pnlNpc, False);

  FCurrentNpcCustomConfig := nil;
end;

procedure TFrmCustomNpc.WMStartEditingNpc(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstNpcAction.EditNode(Node, Message.LParam);
end;

type
  THackTree = class(TVirtualStringTree);

procedure RebuildCustomNpcListText;
var
  I, J: Integer;
  CustomNpcConfig: TCustomNpcConfig;
  InBuf: PAnsiChar;
  InBytes, Index: Integer;
  ClientConfig: PClientCustomNpcConfig;
begin
  if g_MultiThreadRun then
    UserEngine.m_CustomNpcList.LockR(5);
  try
    InBytes := UserEngine.m_CustomNpcList.Count * SizeOf(TClientCustomNpcConfig);
    GetMem(InBuf, InBytes);
    try
      ClientConfig := PClientCustomNpcConfig(InBuf);

      for I := 0 to UserEngine.m_CustomNpcList.Count - 1 do
      begin
        CustomNpcConfig := UserEngine.m_CustomNpcList.Items[I];

        ClientConfig^.wNpcAppr := CustomNpcConfig.NpcAppr;
        ClientConfig^.BaseConfig := CustomNpcConfig.ClientBaseConfig;
        // ClientConfig^.Actions := CustomNpcConfig.DirActions;

        // 将选中的放在前面 chongchong 2016-03-21
        Index := 0;
        for J := Low(CustomNpcConfig.DirActions) to High(CustomNpcConfig.DirActions) do
        begin
          if CustomNpcConfig.DirActions[J].Enabled then
          begin
            ClientConfig^.Actions[Index] := CustomNpcConfig.DirActions[J];
            Inc(Index);
          end;
        end;

        ClientConfig^.wDirCount := Index;

        for J := Low(CustomNpcConfig.DirActions) to High(CustomNpcConfig.DirActions) do
        begin
          if not CustomNpcConfig.DirActions[J].Enabled then
          begin
            ClientConfig^.Actions[Index] := CustomNpcConfig.DirActions[J];
            Inc(Index);
          end;
        end;

        Inc(ClientConfig);
      end;

      g_CustomNpcListTextLen := InBytes;
      g_CustomNpcListText := zLibCompressBuffer(InBuf, InBytes);
      g_CustomNpcListTextCRC := BufferCrc(PAnsiChar(g_CustomNpcListText), Length(g_CustomNpcListText));
    finally
      FreeMem(InBuf, InBytes);
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomNpcList.UnLockR;
  end;
end;

procedure TFrmCustomNpc.SetNpcConfigChanged(IsChanged: Boolean);
begin
  if FCurrentNpcCustomConfig <> nil then
  begin
    FCurrentNpcCustomConfig.SetChanged(IsChanged);
    if vstCustomNpc.FocusedNode <> nil then
      vstCustomNpc.InvalidateNode(vstCustomNpc.FocusedNode);

    FIsNpcChanged := True;
    if not btnSaveNpc.Enabled then
      btnSaveNpc.Enabled := True;
  end;
end;

procedure TFrmCustomNpc.vstCustomNpcDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
var
  ConfigNodeData: PNpcConfigNodeData;
begin
  ConfigNodeData := Sender.GetNodeData(Node);
  if ConfigNodeData <> nil then
  begin
    if ConfigNodeData.Config.IsChanged then
      TargetCanvas.Font.Color := clRed
    else if Sender.Selected[Node] and (Sender.Focused) then
      TargetCanvas.Font.Color := clHighlightText
    else
      TargetCanvas.Font.Color := Sender.Font.Color;
  end;
end;

procedure TFrmCustomNpc.vstCustomNpcGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(TCustomNpcConfig);
end;

procedure TFrmCustomNpc.vstCustomNpcGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  ConfigNodeData: PNpcConfigNodeData;
begin
  ConfigNodeData := Sender.GetNodeData(Node);
  if ConfigNodeData <> nil then
    CellText := IntToStr(ConfigNodeData.Config.NpcAppr);
end;

procedure TFrmCustomNpc.vstCustomNpcNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  I: Integer;
  Node: PVirtualNode;
  NodeData: PNpcNodeData;
  OldChanged, OldIsConfigCanSave: Boolean;
  ConfigNodeData: PNpcConfigNodeData;
begin
  if vstNpcAction.IsEditing then
    vstNpcAction.EndEditNode;

  vstNpcAction.Clear;
  FCurrentNpcCustomConfig := nil;

  if vstCustomNpc.FocusedNode = nil then
    Exit;
  SetControlEnabled(pnlNpc, True);
  ConfigNodeData := vstCustomNpc.GetNodeData(vstCustomNpc.FocusedNode);
  if ConfigNodeData = nil then
    Exit;
  OldIsConfigCanSave := FIsNpcChanged;

  FCurrentNpcCustomConfig := ConfigNodeData.Config;
  OldChanged := FCurrentNpcCustomConfig.IsChanged;
  for I := DR_UP to DR_UPLEFT do
  begin
    Node := vstNpcAction.AddChild(nil);
    Node.CheckType := ctCheckBox;

    NodeData := vstNpcAction.GetNodeData(Node);
    NodeData.Action := @FCurrentNpcCustomConfig.DirActions[I];
    NodeData.ActionType := atStand;

    if NodeData.Action.Enabled then
      vstNpcAction.CheckState[Node] := csCheckedNormal
    else
      vstNpcAction.CheckState[Node] := csUnCheckedNormal;

    Node := vstNpcAction.AddChild(nil);
    Node.CheckType := ctNone;
    NodeData := vstNpcAction.GetNodeData(Node);
    NodeData.Action := @FCurrentNpcCustomConfig.DirActions[I];
    NodeData.ActionType := atAction;
  end;

  cbbNpcHPFile.ItemIndex := FCurrentNpcCustomConfig.ClientBaseConfig.HPFile;
  seNpcHPStartIndex.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPStartIndex;

  seNpcHPBgOffsetX.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPBgOffsetX;
  seNpcHPBgOffsetY.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPBgOffsetY;

  seNpcHPOffsetX.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPOffsetX;
  seNpcHPOffsetY.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPOffsetY;

  seNpcHPTextOffsetX.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPTextOffsetX;
  seNpcHPTextOffsetY.Value := FCurrentNpcCustomConfig.ClientBaseConfig.HPTextOffsetY;

  cbbNpcStandDrawMode.ItemIndex := Integer(FCurrentNpcCustomConfig.ClientBaseConfig.StandDrawMode);
  cbbNpcStandEffectDrawMode.ItemIndex := Integer(FCurrentNpcCustomConfig.ClientBaseConfig.StandEffectDrawMode);
  cbbNpcActionDrawMode.ItemIndex := Integer(FCurrentNpcCustomConfig.ClientBaseConfig.ActionDrawMode);
  cbbNpcActionEffectDrawMode.ItemIndex := Integer(FCurrentNpcCustomConfig.ClientBaseConfig.ActionEffectDrawMode);

  cbbNpcKeepPlayFile.ItemIndex := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayFile;
  seNpcKeepPlayIndex.Value := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayIndex;
  seNpcKeepPlayCount.Value := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayCount;
  seNpcKeepPlayTime.Value := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayTime;
  chkNpcKeepPlayBlendDraw.Checked := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayBlendDraw;
  seKeepPlayOffsetX.Value := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayOffsetX;
  seKeepPlayOffsetY.Value := FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayOffsetY;

  lstNpcDrawOrder.Clear;
  case FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder of
    ndoKeep_Chr_Eff:
      begin
        lstNpcDrawOrder.Items.AddObject('持久播放', TObject(0));
        lstNpcDrawOrder.Items.AddObject('角色绘制', TObject(1));
        lstNpcDrawOrder.Items.AddObject('特效播放', TObject(2));
      end;
    ndoKeep_Eff_Chr:
      begin
        lstNpcDrawOrder.Items.AddObject('持久播放', TObject(0));
        lstNpcDrawOrder.Items.AddObject('特效播放', TObject(2));
        lstNpcDrawOrder.Items.AddObject('角色绘制', TObject(1));
      end;
    ndoChr_Keep_Eff:
      begin
        lstNpcDrawOrder.Items.AddObject('角色绘制', TObject(1));
        lstNpcDrawOrder.Items.AddObject('持久播放', TObject(0));
        lstNpcDrawOrder.Items.AddObject('特效播放', TObject(2));
      end;
    ndoChr_Eff_Keep:
      begin
        lstNpcDrawOrder.Items.AddObject('角色绘制', TObject(1));
        lstNpcDrawOrder.Items.AddObject('特效播放', TObject(2));
        lstNpcDrawOrder.Items.AddObject('持久播放', TObject(0));
      end;
    ndoEff_Keep_Chr:
      begin
        lstNpcDrawOrder.Items.AddObject('特效播放', TObject(2));
        lstNpcDrawOrder.Items.AddObject('持久播放', TObject(0));
        lstNpcDrawOrder.Items.AddObject('角色绘制', TObject(1));
      end;
    ndoEff_Chr_Keep:
      begin
        lstNpcDrawOrder.Items.AddObject('特效播放', TObject(2));
        lstNpcDrawOrder.Items.AddObject('角色绘制', TObject(1));
        lstNpcDrawOrder.Items.AddObject('持久播放', TObject(0));
      end;
  end;

  SetNpcConfigChanged(OldChanged);

  FIsNpcChanged := OldIsConfigCanSave;
  if not FIsNpcChanged then
    btnSaveNpc.Enabled := False;
end;

procedure TFrmCustomNpc.vstNpcActionChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  NodeData: PNpcNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if vstNpcAction.CheckState[Node] = csCheckedNormal then
      NodeData.Action.Enabled := True
    else
      NodeData.Action.Enabled := False;

    SetNpcConfigChanged();
  end;
end;

procedure TFrmCustomNpc.vstNpcActionCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TNpcPropertyEditLink.Create;
end;

procedure TFrmCustomNpc.vstNpcActionDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
begin
  TargetCanvas.Font.Color := Sender.Font.Color;
end;

procedure TFrmCustomNpc.vstNpcActionEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var Allowed: Boolean);
begin
  Allowed := (Node <> nil) and (Column > 1);
end;

procedure TFrmCustomNpc.vstNpcActionGetHint(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var LineBreakStyle: TVTTooltipLineBreakStyle; var HintText: string);
var
  NodeData: PNpcNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if Column = 7 then
      HintText := '特效开始图片为-1表示不使用特效';
  end;
end;

procedure TFrmCustomNpc.vstNpcActionGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(TNpcNodeData);
end;

procedure TFrmCustomNpc.vstNpcActionGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  NodeData: PNpcNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData = nil then
    Exit;
  if NodeData.ActionType = atStand then
  begin
    case Column of
      0:
        CellText := NpcDirNames[Node.Index div 2];
      1:
        CellText := NpcActionNames[NodeData.ActionType];
      2:
        begin
          if (NodeData.Action.Std_File < g_EffectImageList.Count) then
            CellText := g_EffectImageList.Strings[NodeData.Action.Std_File];
        end;
      3:
        CellText := IntToStr(NodeData.Action.Std_Index);
      4:
        CellText := IntToStr(NodeData.Action.Std_Count);
      5:
        CellText := IntToStr(NodeData.Action.Std_Time);
      6:
        begin
          if (NodeData.Action.Std_EffFile < g_EffectImageList.Count) then
            CellText := g_EffectImageList.Strings[NodeData.Action.Std_EffFile];
        end;
      7:
        CellText := IntToStr(NodeData.Action.Std_EffIndex);
      {
        8:
        CellText := IntToStr(NodeData.Action.Std_EffCount);
        9:
        CellText := IntToStr(NodeData.Action.Std_EffTime);
      }
    end;
  end
  else if NodeData.ActionType = atAction then
  begin
    case Column of
      0:
        CellText := '';
      1:
        CellText := NpcActionNames[NodeData.ActionType];
      2:
        begin
          if (NodeData.Action.Act_File < g_EffectImageList.Count) then
            CellText := g_EffectImageList.Strings[NodeData.Action.Act_File];
        end;
      3:
        CellText := IntToStr(NodeData.Action.Act_Index);
      4:
        CellText := IntToStr(NodeData.Action.Act_Count);
      5:
        CellText := IntToStr(NodeData.Action.Act_Time);
      6:
        begin
          if (NodeData.Action.Act_EffFile < g_EffectImageList.Count) then
            CellText := g_EffectImageList.Strings[NodeData.Action.Act_EffFile];
        end;
      7:
        CellText := IntToStr(NodeData.Action.Act_EffIndex);
      {
        8:
        CellText := IntToStr(NodeData.Action.Act_EffCount);
        9:
        CellText := IntToStr(NodeData.Action.Act_EffTime);
      }
    end;
  end;
end;

procedure TFrmCustomNpc.vstNpcActionNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn > 1) then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_NPC, WParam(HitInfo.HitNode), HitInfo.HitColumn);
  end;
end;

procedure TFrmCustomNpc.btnNpcFileStandClick(Sender: TObject);
var
  I: Integer;
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;

  if Sender = btnNpcFileStand then
  begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
      FCurrentNpcCustomConfig.DirActions[I].Std_File := cbbNpcBatchFile.ItemIndex;
    end;
  end
  else if Sender = btnNpcFileStandEffect then
  begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
      FCurrentNpcCustomConfig.DirActions[I].Std_EffFile := cbbNpcBatchFile.ItemIndex;
    end;
  end
  else if Sender = btnNpcFileAction then
  begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
      FCurrentNpcCustomConfig.DirActions[I].Act_File := cbbNpcBatchFile.ItemIndex;
    end;
  end
  else if Sender = btnNpcFileActionEffect then
  begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
      FCurrentNpcCustomConfig.DirActions[I].Act_EffFile := cbbNpcBatchFile.ItemIndex;
    end;
  end;
  SetNpcConfigChanged(True);
  vstNpcAction.Invalidate;
end;

procedure TFrmCustomNpc.btnNpcTimeStandClick(Sender: TObject);
var
  I: Integer;
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;

  if Sender = btnNpcTimeStand then
  begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
      FCurrentNpcCustomConfig.DirActions[I].Std_Time := seNpcBatchTime.Value;
    end;
  end
  {
    else if Sender = btnNpcTimeStandEffect then
    begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
    FCurrentNpcCustomConfig.DirActions[I].Std_EffTime := seNpcBatchTime.Value;
    end;
    end
  }
  else if Sender = btnNpcTimeAction then
  begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
      FCurrentNpcCustomConfig.DirActions[I].Act_Time := seNpcBatchTime.Value;
    end;
  end;
  {
    else if Sender = btnNpcTimeActionEffect then
    begin
    for I := Low(FCurrentNpcCustomConfig.DirActions) to High(FCurrentNpcCustomConfig.DirActions) do
    begin
    FCurrentNpcCustomConfig.DirActions[I].Act_EffTime := seNpcBatchTime.Value;
    end;
    end;
  }
  SetNpcConfigChanged(True);
  vstNpcAction.Invalidate;
end;

procedure TFrmCustomNpc.btnNpcCalcStandClick(Sender: TObject);
var
  I: Integer;
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;

  if Sender = btnNpcCalcStand then
  begin
    for I := 1 to seNpcCalcDirCount.Value do
    begin
      FCurrentNpcCustomConfig.DirActions[I - 1].Std_Index := seNpcCalcStartIndex.Value +
        (seNpcCalcPlayCount.Value + seNpcCalcEmptyCount.Value) * (I - 1);
      FCurrentNpcCustomConfig.DirActions[I - 1].Std_Count := seNpcCalcPlayCount.Value;
    end;
  end
  else if Sender = btnNpcCalcStandEffect then
  begin
    for I := 1 to seNpcCalcDirCount.Value do
    begin
      FCurrentNpcCustomConfig.DirActions[I - 1].Std_EffIndex := seNpcCalcStartIndex.Value +
        (seNpcCalcPlayCount.Value + seNpcCalcEmptyCount.Value) * (I - 1);
      // FCurrentNpcCustomConfig.DirActions[I - 1].Std_EffCount := seNpcCalcPlayCount.Value;
    end;
  end
  else if Sender = btnNpcCalcHit then
  begin
    for I := 1 to seNpcCalcDirCount.Value do
    begin
      FCurrentNpcCustomConfig.DirActions[I - 1].Act_Index := seNpcCalcStartIndex.Value +
        (seNpcCalcPlayCount.Value + seNpcCalcEmptyCount.Value) * (I - 1);
      FCurrentNpcCustomConfig.DirActions[I - 1].Act_Count := seNpcCalcPlayCount.Value;
    end;
  end
  else if Sender = btnNpcCalcHitEffect then
  begin
    for I := 1 to seNpcCalcDirCount.Value do
    begin
      FCurrentNpcCustomConfig.DirActions[I - 1].Act_EffIndex := seNpcCalcStartIndex.Value +
        (seNpcCalcPlayCount.Value + seNpcCalcEmptyCount.Value) * (I - 1);
      // FCurrentNpcCustomConfig.DirActions[I - 1].Act_EffCount := seNpcCalcPlayCount.Value;
    end;
  end;

  SetNpcConfigChanged(True);
  vstNpcAction.Invalidate;
end;

procedure TFrmCustomNpc.btnSaveNpcClick(Sender: TObject);
var
  Node: PVirtualNode;
  ConfigNodeData: PNpcConfigNodeData;
begin
  if vstCustomNpc.IsEditing then
    vstCustomNpc.EndEditNode;

  Node := vstCustomNpc.GetFirst();
  while Node <> nil do
  begin
    ConfigNodeData := vstCustomNpc.GetNodeData(Node);
    if (ConfigNodeData <> nil) then
    begin
      if ConfigNodeData.Config.IsChanged then
        ConfigNodeData.Config.SaveToIniFile;
    end;

    Node := vstCustomNpc.GetNext(Node);
  end;
  vstCustomNpc.Invalidate;

  RebuildCustomNpcListText;

  g_Config.dwCustomNpcMoveTime := seCustomNpcMoveTime.Value;
  Config.WriteInteger('Setup', 'CustomNpcMoveTime', g_Config.dwCustomNpcMoveTime);

  FIsNpcChanged := False;
  btnSaveNpc.Enabled := False;
end;

procedure TFrmCustomNpc.btnSaveNpcToFileClick(Sender: TObject);
var
  FileName: string;
begin
  if g_Config.sCustomNpcClientConfigFileName <> '' then
    dlgSaveNpcs.FileName := g_Config.sCustomNpcClientConfigFileName;

  if not dlgSaveNpcs.Execute then
  begin
    // 指定保存目录会改变当前程序目录,
    SetCurrentDirectory(PChar(ExtractFileDir(Application.ExeName)));
    Exit;
  end;

  // 指定保存目录会改变当前程序目录,
  SetCurrentDirectory(PChar(ExtractFileDir(Application.ExeName)));

  FileName := dlgSaveNpcs.FileName;
  FileName := ChangeFileExt(FileName, '.dat');

  g_Config.sCustomNpcClientConfigFileName := FileName;
  Config.WriteString('Setup', 'CustomNpcClientConfigFileName', g_Config.sCustomNpcClientConfigFileName);

  if g_MultiThreadRun then
    UserEngine.m_CustomNpcList.LockR(6);
  try
    SaveCustomNpcClientConfigs(UserEngine.m_CustomNpcList, FileName);
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomNpcList.UnLockR;
  end;

  Showmessage('已经生成自定义NPC登录器配置文件');
end;

procedure TFrmCustomNpc.cbbNpcHPFileChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPFile := cbbNpcHPFile.ItemIndex;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPStartIndexChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPStartIndex := seNpcHPStartIndex.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPBgOffsetXChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPBgOffsetX := seNpcHPBgOffsetX.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPBgOffsetYChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPBgOffsetY := seNpcHPBgOffsetY.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPOffsetXChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPOffsetX := seNpcHPOffsetX.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPOffsetYChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPOffsetY := seNpcHPOffsetY.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPTextOffsetXChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPTextOffsetX := seNpcHPTextOffsetX.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcHPTextOffsetYChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.HPTextOffsetY := seNpcHPTextOffsetY.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.chkSendCustomNPCConfigClick(Sender: TObject);
begin
  g_Config.boSendCustomNpcConfig := chkSendCustomNPCConfig.Checked;
  Config.WriteBool('Setup', 'SendCustomNpcConfig', g_Config.boSendCustomNpcConfig);
end;

procedure TFrmCustomNpc.vstNpcActionBeforeCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; CellPaintMode: TVTCellPaintMode; CellRect: TRect; var ContentRect: TRect);
var
  NodeData: PNpcNodeData;
  ItemColor: Integer;
begin
  NodeData := Sender.GetNodeData(Node);

  if NodeData.ActionType = atStand then
  begin
    ItemColor := $00F2E4D8;
    TargetCanvas.Brush.Color := ItemColor;
    TargetCanvas.FillRect(CellRect);
  end;
end;

procedure TFrmCustomNpc.cbbNpcStandDrawModeChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.StandDrawMode := TCustomDrawMode(cbbNpcStandDrawMode.ItemIndex);
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.cbbNpcStandEffectDrawModeChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.StandEffectDrawMode := TCustomDrawMode(cbbNpcStandEffectDrawMode.ItemIndex);
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.cbbNpcActionDrawModeChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.ActionDrawMode := TCustomDrawMode(cbbNpcActionDrawMode.ItemIndex);
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.cbbNpcActionEffectDrawModeChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.ActionEffectDrawMode := TCustomDrawMode(cbbNpcActionEffectDrawMode.ItemIndex);
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.cbbNpcKeepPlayFileChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayFile := cbbNpcKeepPlayFile.ItemIndex;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcKeepPlayIndexChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayIndex := seNpcKeepPlayIndex.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcKeepPlayCountChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayCount := seNpcKeepPlayCount.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seNpcKeepPlayTimeChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayTime := seNpcKeepPlayTime.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.chkNpcKeepPlayBlendDrawClick(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayBlendDraw := chkNpcKeepPlayBlendDraw.Checked;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.btnNpcMoveTopClick(Sender: TObject);
var
  Index: Integer;
  O1, O2: Integer;
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  Index := lstNpcDrawOrder.ItemIndex;

  if Sender = btnNpcMoveTop then
  begin
    if Index <= 0 then
      Exit;
    lstNpcDrawOrder.Items.Move(Index, Index - 1);
    lstNpcDrawOrder.ItemIndex := Index - 1;
  end
  else
  begin
    if Index < 0 then
      Exit;
    if Index >= lstNpcDrawOrder.Items.Count - 1 then
      Exit;
    lstNpcDrawOrder.Items.Move(Index, lstNpcDrawOrder.ItemIndex + 1);
    lstNpcDrawOrder.ItemIndex := Index + 1;
  end;

  O1 := Integer(lstNpcDrawOrder.Items.Objects[0]);
  O2 := Integer(lstNpcDrawOrder.Items.Objects[1]);

  case O1 of
    0:
      begin
        if O2 = 1 then
          FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder := ndoKeep_Chr_Eff
        else
          FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder := ndoKeep_Eff_Chr
      end;
    1:
      begin
        if O2 = 0 then
          FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder := ndoChr_Keep_Eff
        else
          FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder := ndoChr_Eff_Keep
      end;
    2:
      begin
        if O2 = 0 then
          FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder := ndoEff_Keep_Chr
        else
          FCurrentNpcCustomConfig.ClientBaseConfig.DrawOrder := ndoEff_Chr_Keep
      end;
  end;

  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seKeepPlayOffsetXChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayOffsetX := seKeepPlayOffsetX.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seKeepPlayOffsetYChange(Sender: TObject);
begin
  if FCurrentNpcCustomConfig = nil then
    Exit;
  FCurrentNpcCustomConfig.ClientBaseConfig.KeepPlayOffsetY := seKeepPlayOffsetY.Value;
  SetNpcConfigChanged();
end;

procedure TFrmCustomNpc.seCustomNpcMoveTimeChange(Sender: TObject);
begin
  btnSaveNpc.Enabled := True;
end;

end.
