unit uFrmGameSpeed;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, GateShare, ExtCtrls, ColorIndexEdit, HUtil32, IniFiles,
  VirtualTrees, Themes, SpinEditEx, uFrmInterval, ImgList, ComCtrls, RunGateUtils,
  MirClientContext;

const
  WM_STARTEDITING = WM_USER + 778;

type
  TFrmGameSpeed = class(TForm)
    GroupBox1: TGroupBox;
    btnSave: TButton;
    btnClose: TButton;
    GroupBox2: TGroupBox;
    lbl2: TLabel;
    Label1: TLabel;
    seFColor: TColorIndexEdit;
    seBColor: TColorIndexEdit;
    vstAntiPlugAction: TVirtualStringTree;
    GroupBox3: TGroupBox;
    Label6: TLabel;
    seLockTime: TSpinEditEx;
    Label7: TLabel;
    chkSaveLockStatus: TCheckBox;
    chkShowLockLog: TCheckBox;
    lbl3: TLabel;
    edtShowLockMsg: TEdit;
    lbl1: TLabel;
    cbbMsgType: TComboBox;
    lbl4: TLabel;
    edtPreview: TEdit;
    GroupBox5: TGroupBox;
    Label5: TLabel;
    Label8: TLabel;
    seDealTryAttackTime: TSpinEditEx;
    seBrutalAttackTime: TSpinEditEx;
    ilCheck: TImageList;
    chkDealTryAttackHint: TCheckBox;
    chkBrutalAttackHint: TCheckBox;
    Label3: TLabel;
    grp2: TGroupBox;
    lbl5: TLabel;
    seContinueSpeedPassIncTime: TSpinEditEx;
    Label9: TLabel;
    GroupBox6: TGroupBox;
    trckbrSpeedValue: TTrackBar;
    Label4: TLabel;
    seUserShopSearchTime: TSpinEditEx;
    chkUserShopSearchHint: TCheckBox;
    Label10: TLabel;
    seUserShopBuyTime: TSpinEditEx;
    chkUserShopBuyHint: TCheckBox;
    Label2: TLabel;
    seTakeOnItemTime: TSpinEditEx;
    chkTakeOnItemHint: TCheckBox;
    chkSpeedClearData: TCheckBox;
    chkShowAttackLog: TCheckBox;
    chkShowDropConcurrentLog: TCheckBox;
    GroupBox4: TGroupBox;
    lbl6: TLabel;
    seSumSpeedCheckTime: TSpinEditEx;
    Label11: TLabel;
    seSumSpeedMaxCount: TSpinEditEx;
    lbl8: TLabel;
    lblSpeedValue: TLabel;
    chkContinueSpeedCloseSocket: TCheckBox;
    seContinueSpeedCount: TSpinEditEx;
    lbl7: TLabel;
    trckbrCollectCount: TTrackBar;
    lbl9: TLabel;
    Label12: TLabel;
    chkZeroCompensationValueClearPool: TCheckBox;
    lbl10: TLabel;
    seClientUploadPickItemsTime: TSpinEditEx;
    lbl11: TLabel;
    procedure edtShowLockMsgChange(Sender: TObject);
    procedure btnCloseClick(Sender: TObject);
    procedure btnSaveClick(Sender: TObject);
    procedure seFColorChange(Sender: TObject);
    procedure seBColorChange(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure vstAntiPlugActionGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure vstAntiPlugActionDrawText(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: WideString; const CellRect: TRect;
      var DefaultDraw: Boolean);
    procedure vstAntiPlugActionEditing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstAntiPlugActionNodeClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
    procedure vstAntiPlugActionCreateEditor(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
    procedure vstAntiPlugActionChecked(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure btnDefaultClick(Sender: TObject);
    procedure cbbMsgTypeChange(Sender: TObject);
    procedure seLockTimeChange(Sender: TObject);
    procedure chkSaveLockStatusClick(Sender: TObject);
    procedure chkShowLockLogClick(Sender: TObject);
    procedure chkSpeedClearDataClick(Sender: TObject);
    procedure seDealTryAttackTimeChange(Sender: TObject);
    procedure seBrutalAttackTimeChange(Sender: TObject);
    procedure vstAntiPlugActionGetHint(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex;
      var LineBreakStyle: TVTTooltipLineBreakStyle;
      var HintText: WideString);
    procedure chkDealTryAttackHintClick(Sender: TObject);
    procedure chkBrutalAttackHintClick(Sender: TObject);
    procedure chkShowAttackLogClick(Sender: TObject);
    procedure seContinueSpeedPassIncTimeChange(Sender: TObject);
    procedure chkShowDropConcurrentLogClick(Sender: TObject);
    procedure trckbrSpeedValueChange(Sender: TObject);
    procedure vstAntiPlugActionChecking(Sender: TBaseVirtualTree;
      Node: PVirtualNode; var NewState: TCheckState; var Allowed: Boolean);
    procedure chkContinueSpeedCloseSocketClick(Sender: TObject);
    procedure seContinueSpeedCountChange(Sender: TObject);
    procedure seUserShopSearchTimeChange(Sender: TObject);
    procedure seUserShopBuyTimeChange(Sender: TObject);
    procedure seTakeOnItemTimeChange(Sender: TObject);
    procedure chkUserShopSearchHintClick(Sender: TObject);
    procedure chkUserShopBuyHintClick(Sender: TObject);
    procedure chkTakeOnItemHintClick(Sender: TObject);
    procedure vstAntiPlugActionAfterCellPaint(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      CellRect: TRect);
    procedure seSumSpeedCheckTimeChange(Sender: TObject);
    procedure seSumSpeedMaxCountChange(Sender: TObject);
    procedure chkZeroCompensationValueClearPoolClick(Sender: TObject);
  private
    { Private declarations }
    FRunGateManager: TRunGateManager;

    procedure RefreshCtrlsStatus();
    procedure SetSaveStatus(boSave: Boolean);

    procedure WMStartEditing(var Message: TMessage); message WM_STARTEDITING;
  public
    { Public declarations }
  end;

  function ShowFrmGameSpeed(MainForm: TForm; RunGateManager: TRunGateManager): Boolean;

implementation

{$R *.dfm}

type
  PNodeData = ^TNodeData;
  TNodeData = record
    ActionMode: TAntiPlugActionMode;
  end;

type
  TPropertyEditLink = class(TInterfacedObject, IVTEditLink)
  private
    FEdit: TWinControl;        // One of the property editor classes.
    FTree: TVirtualStringTree; // A back reference to the tree calling.
    FNode: PVirtualNode;       // The node being edited.
    FColumn: Integer;          // The column of the node being edited.

    procedure OnBtnIntervalClick(Sender: TObject);
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
  S: string;
  NodeData: PNodeData;
  TempValue: Integer;
  IsChanged: Boolean;
  ProcessMode: TActionProcessMode;
  SumProcessMode: TSumActionProcessMode;
begin
  Result := True;
  IsChanged := False;

  NodeData := FTree.GetNodeData(FNode);
  if FEdit is TComboBox then
  begin
    TempValue := (FEdit as TComboBox).ItemIndex;
    case FColumn of
      2:
        begin
          ProcessMode := TActionProcessMode((FEdit as TComboBox).Items.Objects[TempValue]);
          if g_Config.ActionList[NodeData.ActionMode].ProcessMode <> ProcessMode then
          begin
            g_Config.ActionList[NodeData.ActionMode].ProcessMode := ProcessMode;
            IsChanged := True;
          end;
        end;
      3:
        begin
          SumProcessMode := TSumActionProcessMode(TempValue);
          if g_Config.ActionList[NodeData.ActionMode].SumProcessMode <> SumProcessMode then
          begin
            g_Config.ActionList[NodeData.ActionMode].SumProcessMode := SumProcessMode;
            IsChanged := True;
          end;
        end;
    end;
  end
  else if FEdit is TSpinEditEx then
  begin
    TempValue := (FEdit as TSpinEditEx).Value;
    case FColumn of
      1:
        if g_Config.ActionList[NodeData.ActionMode].nInterval <> TempValue then
        begin
          g_Config.ActionList[NodeData.ActionMode].nInterval := TempValue;
          IsChanged := True;
        end;

      {
      4:
        if g_Config.ActionList[NodeData.ActionMode].nFloatingInterval <> TempValue then
        begin
          g_Config.ActionList[NodeData.ActionMode].nFloatingInterval := TempValue;
          IsChanged := True;
        end;
      
      5:
        if g_Config.ActionList[NodeData.ActionMode].nCollectCount <> TempValue then
        begin
          g_Config.ActionList[NodeData.ActionMode].nCollectCount := TempValue;
          IsChanged := True;
        end;
      6:
        if g_Config.ActionList[NodeData.ActionMode].nCollectSpeedCount <> TempValue then
        begin
          g_Config.ActionList[NodeData.ActionMode].nCollectSpeedCount := TempValue;
          IsChanged := True;
        end;
      }

      7:
        if g_Config.ActionList[NodeData.ActionMode].nCompensationValue <> TempValue then
        begin
          g_Config.ActionList[NodeData.ActionMode].nCompensationValue := TempValue;
          IsChanged := True;
        end;
    end;
  end
  else if FEdit is TEdit then
  begin
    S := (FEdit as TEdit).Text;
    case FColumn of
      6:
        if not SameText(g_Config.ActionList[NodeData.ActionMode].sHintText, S) then
        begin
          g_Config.ActionList[NodeData.ActionMode].sHintText := S;
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
    if (FTree.Owner is TFrmGameSpeed) then
    begin
      TFrmGameSpeed(FTree.Owner).SetSaveStatus(True);
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
  NodeData: PNodeData;
  ProcessMode: TActionProcessMode;
  SumProcessMode: TSumActionProcessMode;
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
    1, 4 {, 5, 6}:
      begin
        if (Column = 1) then
        begin
          if NodeData.ActionMode in [amHit, amSpell, amWalk, amRun,
            amWalkToHit {走路到攻击},     amHitToWalk {攻击到走路},
            amRunToHit {跑步到攻击},      amHitToRun {攻击到跑步},
            amWalkToSpell {走路到魔法},   amSpellToWalk {魔法到走路},
            amRunToSpell {跑步到魔法},    amSpellToRun {魔法到跑步},
            amTurnToHit {转向到攻击},   (*amHitToTurn {攻击到转向},*)
            amTurnToSpell {转向到魔法}, (*amSpellToTurn {魔法到转向},*)
            amCutMeatToHit {挖肉到攻击},  amCutMeatToSpell {挖肉到魔法},
            (*amMoveToTurn {移动到转向},*)    amTurnToMove {转向到移动},
            (*amMoveToCutMeat {移动到挖肉},*) amCutMeatToMove {挖肉到移动}] then
          begin
            FEdit := TButton.Create(nil);
            TButton(FEdit).Font.Assign(FTree.Font);

            with FEdit as TButton do
            begin
              Visible := False;
              Parent := Tree;
              Caption := '设置';
              OnKeyDown := EditKeyDown;
              OnKeyUp := EditKeyUp;
              Tag := Integer(NodeData.ActionMode);
              OnClick := OnBtnIntervalClick;
            end;
            Exit;
          end;
        end;

        FEdit := TSpinEditEx.Create(nil);
        TSpinEditEx(FEdit).Font.Assign(FTree.Font);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;


          MaxValue := High(Integer);

          if Column = 1 then
            MinValue := 1

          else if Column = 4 then
          begin
            MinValue := 0;
            MaxValue := 30000;
          end
          {
          else if Column = 6 then
          begin
            MinValue := 2;
            MaxValue := 100;
          end
          }
          else
            MinValue := 1;

          case FColumn of
            1: Value := g_Config.ActionList[NodeData.ActionMode].nInterval;
            //4: Value := g_Config.ActionList[NodeData.ActionMode].nFloatingInterval;
            //5: Value := g_Config.ActionList[NodeData.ActionMode].nCollectCount;
            //6: Value := g_Config.ActionList[NodeData.ActionMode].nCollectSpeedCount;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    2:
      begin
        FEdit := TComboBox.Create(nil);
        TComboBox(FEdit).Font.Assign(FTree.Font);
        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;

          if NodeData.ActionMode in [amHit, amSpell, amWalkToHit, amRunToHit, amWalkToSpell, amRunToSpell, amCutMeatToHit, amCutMeatToSpell,
            amMoveToTurn, amTurnToMove, amMoveToCutMeat, amCutMeatToMove] then
          begin
            for ProcessMode := apmRebound to High(TActionProcessMode) do
            begin
              Items.AddObject(ActionProcessModeNames[ProcessMode], TObject(ProcessMode));
              if g_Config.ActionList[NodeData.ActionMode].ProcessMode = ProcessMode then
              begin
                ItemIndex := Integer(ProcessMode) - Integer(apmRebound);
              end;
            end;
          end
          else if NodeData.ActionMode in [amHitConcurrent, amSpellConcurrent, amMoveConcurrent] then
          begin
            for ProcessMode := apmOffline to High(TActionProcessMode) do
            begin
              Items.AddObject(ActionProcessModeNames2[ProcessMode], TObject(ProcessMode));
              if g_Config.ActionList[NodeData.ActionMode].ProcessMode = ProcessMode then
              begin
                ItemIndex := Integer(ProcessMode) - Integer(apmOffline);
              end;
            end;
          end
          else
          begin
            for ProcessMode := Low(TActionProcessMode) to High(TActionProcessMode){apmOffline} do
            begin
              if ProcessMode <> apmFakeAttackPass then
              begin
                Items.AddObject(ActionProcessModeNames[ProcessMode], TObject(ProcessMode));
                if g_Config.ActionList[NodeData.ActionMode].ProcessMode = ProcessMode then
                begin
                  if ProcessMode = apmNoProcess then          // apmOffline去掉了，这个要减一个
                    ItemIndex := Integer(ProcessMode) - 1
                  else
                    ItemIndex := Integer(ProcessMode);
                end;
              end;
            end;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    3:
      begin
        FEdit := TComboBox.Create(nil);
        TComboBox(FEdit).Font.Assign(FTree.Font);
        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;

          for SumProcessMode := Low(TSumActionProcessMode) to High(TSumActionProcessMode) do
          begin
            Items.Add(SumActionProcessModeNames[SumProcessMode]);
            if g_Config.ActionList[NodeData.ActionMode].SumProcessMode = SumProcessMode then
            begin
              ItemIndex := Integer(SumProcessMode);
            end;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    6:
      begin
        FEdit := TEdit.Create(nil);
        TEdit(FEdit).Font.Assign(FTree.Font);
        with FEdit as TEdit do
        begin
          Visible := False;
          Parent := Tree;

          Text := g_Config.ActionList[NodeData.ActionMode].sHintText;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    7:
      begin
        FEdit := TSpinEditEx.Create(nil);
        TSpinEditEx(FEdit).Font.Assign(FTree.Font);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          MaxValue := 0;
          MinValue := 0;
          Value := g_Config.ActionList[NodeData.ActionMode].nCompensationValue;
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

  if FColumn = 2 then
  begin
    R.Right := R.Left + 76 - 3;
  end;

  Windows.InflateRect(R, -1, 0);
  FEdit.BoundsRect := R;
end;

procedure TPropertyEditLink.OnBtnIntervalClick(Sender: TObject);
var
  Btn: TButton;
begin
  if Sender is TButton then
  begin
    Btn := Sender as TButton;
    if Btn.Tag in [Integer(amHit), Integer(amSpell), Integer(amWalk), Integer(amRun),
      Integer(amWalkToHit) {走路到攻击},          Integer(amHitToWalk) {攻击到走路},
      Integer(amRunToHit) {跑步到攻击},           Integer(amHitToRun) {攻击到跑步},
      Integer(amWalkToSpell) {走路到魔法},        Integer(amSpellToWalk) {魔法到走路},
      Integer(amRunToSpell) {跑步到魔法},         Integer(amSpellToRun) {魔法到跑步},
      Integer(amTurnToHit) {转向到攻击},          (*Integer(amHitToTurn) {攻击到转向},*)
      Integer(amTurnToSpell) {转向到魔法},        (*Integer(amSpellToTurn) {魔法到转向},*)
      Integer(amCutMeatToHit) {挖肉到攻击},       Integer(amCutMeatToSpell) {挖肉到魔法},
      (*Integer(amMoveToTurn) {移动到转向},*)     Integer(amTurnToMove) {转向到移动},
      (*Integer(amMoveToCutMeat) {移动到挖肉},*)  Integer(amCutMeatToMove) {挖肉到移动}]  then
      ShowFrmInterval(TAntiPlugActionMode(Btn.Tag));
  end;
end;

function ShowFrmGameSpeed(MainForm: TForm; RunGateManager: TRunGateManager): Boolean;
var
  FrmGameSpeed: TFrmGameSpeed;

  OnTreeChecking: TVTCheckChangingEvent;
  OnTreeChecked: TVTChangeEvent;
begin
  FrmGameSpeed := TFrmGameSpeed.Create(nil);
  try
    FrmGameSpeed.FRunGateManager := RunGateManager;
    FrmGameSpeed.Left := MainForm.Left + (MainForm.Width - FrmGameSpeed.Width) div 2;
    FrmGameSpeed.Top := MainForm.Top + (MainForm.Height - FrmGameSpeed.Height) div 2;

    OnTreeChecking := FrmGameSpeed.vstAntiPlugAction.OnChecking;
    OnTreeChecked := FrmGameSpeed.vstAntiPlugAction.OnChecked;
    
    FrmGameSpeed.vstAntiPlugAction.OnChecking := nil;
    FrmGameSpeed.vstAntiPlugAction.OnChecked := nil;
                                                                         
    FrmGameSpeed.RefreshCtrlsStatus;

    FrmGameSpeed.vstAntiPlugAction.OnChecking := OnTreeChecking;
    FrmGameSpeed.vstAntiPlugAction.OnChecked := OnTreeChecked;

    Result := FrmGameSpeed.ShowModal = mrOk;
  finally
    FrmGameSpeed.Free;
  end;
end;

procedure TFrmGameSpeed.FormCreate(Sender: TObject);
var
  ActionMode: TAntiPlugActionMode;
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  vstAntiPlugAction.NodeDataSize := SizeOf(TNodeData);

  // 并发不让设置 chongchong 2018-11-12 22:30:21
  for ActionMode := Low(TAntiPlugActionMode) to {High(TAntiPlugActionMode)} amCutMeatToMove do
  begin
    Node := vstAntiPlugAction.AddChild(nil);
    NodeData := vstAntiPlugAction.GetNodeData(Node);
    vstAntiPlugAction.CheckType[Node] := ctCheckBox;

    {
    if ActionMode >= amHitConcurrent then
    begin
      vstAntiPlugAction.CheckState[Node] := csCheckedNormal;
    end;
    }
    
    NodeData.ActionMode := ActionMode;
  end;
end;

procedure TFrmGameSpeed.RefreshCtrlsStatus();
var
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  Node := vstAntiPlugAction.GetFirst();
  while Node <> nil do
  begin
    NodeData := vstAntiPlugAction.GetNodeData(Node);

    if g_Config.ActionList[NodeData.ActionMode].boEnabled or (NodeData.ActionMode >= amHitConcurrent) then
      vstAntiPlugAction.CheckState[Node] := csCheckedNormal
    else
      vstAntiPlugAction.CheckState[Node] := csUncheckedNormal;

    Node := vstAntiPlugAction.GetNext(Node);
  end;

  seLockTime.Value := g_Config.nLockTime;
  chkSaveLockStatus.Checked := g_Config.boSaveLockStatus;
  chkShowLockLog.Checked := g_Config.boShowLockLog;
  edtShowLockMsg.Text := g_Config.sShowLockMsg;

  chkSpeedClearData.Checked := g_Config.boSpeedClearData;

  cbbMsgType.ItemIndex := g_Config.btMsgType;
  seFColor.Value := g_Config.btMsgFColor;
  seBColor.Value := g_Config.btMsgBColor;

  seUserShopSearchTime.Value := g_Config.dwUserShop_Search_Interval;
  chkUserShopSearchHint.Checked := g_Config.boUserShop_Search_ShowHint;

  seUserShopBuyTime.Value := g_Config.dwUserShop_Buy_Interval;
  chkUserShopBuyHint.Checked := g_Config.boUserShop_Buy_ShowHint;

  seTakeOnItemTime.Value := g_Config.dwTakeOn_Item_Interval;
  chkTakeOnItemHint.Checked := g_Config.boTakeOn_Item_ShowHint;

  seDealTryAttackTime.Value := g_Config.dwDealTry_Attack_Interval;
  chkDealTryAttackHint.Checked := g_Config.boDealTry_Attack_ShowHint;

  seBrutalAttackTime.Value := g_Config.dwBrutal_Attack_Interval;
  chkBrutalAttackHint.Checked := g_Config.boBrutal_Attack_ShowHint;

  chkShowAttackLog.Checked := g_Config.boShowAttackLog;
  //chkShowDropConcurrentLog.Checked := g_Config.boShowDropConcurrentLog;

  trckbrCollectCount.Position := g_Config.dwCollectCount;
  trckbrSpeedValue.Position := g_Config.dwSpeedValue;

  lblSpeedValue.Caption := Format('[%d/%d]', [trckbrSpeedValue.Position, trckbrCollectCount.Position]);

  seContinueSpeedPassIncTime.Value := g_Config.dwContinueSpeedPassIncTime;
  chkContinueSpeedCloseSocket.Checked := g_Config.boContinueSpeedCloseSocket;
  seContinueSpeedCount.Value := g_Config.nContinueSpeedCount;

  seSumSpeedCheckTime.Value := g_Config.nSumSpeedCheckTime;
  seSumSpeedMaxCount.Value := g_Config.nSumSpeedMaxCount;

  chkZeroCompensationValueClearPool.Checked := g_Config.boZeroCompensationValueClearPool;

  seClientUploadPickItemsTime.Value := g_Config.dwClientUploadPickItemsTime;

  SetSaveStatus(False);
end;

procedure TFrmGameSpeed.SetSaveStatus(boSave: Boolean);
begin
  btnSave.Enabled := boSave;
end;

procedure TFrmGameSpeed.trckbrSpeedValueChange(Sender: TObject);
begin
  lblSpeedValue.Caption := Format('[%d/ %d]', [trckbrSpeedValue.Position, trckbrCollectCount.Position]);
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.btnCloseClick(Sender: TObject);
begin
  Close;
end;

procedure TFrmGameSpeed.btnSaveClick(Sender: TObject);
var
  IniFile: TIniFile;
  ActionMode: TAntiPlugActionMode;
  Action: PAntiPlugAction;
  Section: string;
  dwCollectCount, dwSpeedValue: LongWord;
begin
  if vstAntiPlugAction.IsEditing then
    vstAntiPlugAction.EndEditNode;

  dwCollectCount := trckbrCollectCount.Position;
  if dwCollectCount < 3 then
    dwCollectCount := 3;

  dwSpeedValue := trckbrSpeedValue.Position;
  if dwSpeedValue < 3 then
    dwSpeedValue := 3;

  if dwSpeedValue >= dwCollectCount then
  begin
    Application.MessageBox('“超速次数”必须 < “总记录数”', '错误', MB_OK or MB_ICONERROR);
    trckbrSpeedValue.SetFocus;
    Exit;
  end;

  g_Config.dwSpeedValue := dwSpeedValue;
  g_Config.dwCollectCount := dwCollectCount;

  g_Config.dwClientUploadPickItemsTime := seClientUploadPickItemsTime.Value;

  IniFile := TIniFile.Create(g_sIniFileName);

  for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
  begin
    Section := AntiPlugActionModeSections[ActionMode];

    Action := @g_Config.ActionList[ActionMode];
    IniFile.WriteBool(Section, 'Enabled', Action.boEnabled);
    IniFile.WriteInteger(Section, 'Interval', Action.nInterval);
    IniFile.WriteInteger(Section, 'ProcessMode', Integer(Action.ProcessMode));
    IniFile.WriteBool(Section, 'ProcessScript', Action.boProcessScript);
    IniFile.WriteInteger(Section, 'SumProcessMode', Integer(Action.SumProcessMode));
    //IniFile.WriteInteger(Section, 'FloatingInterval', Action.nFloatingInterval);
    //IniFile.WriteInteger(Section, 'CollectCount', Action.nCollectCount);
    //IniFile.WriteInteger(Section, 'CollectSpeedCount', Action.nCollectSpeedCount);
    IniFile.WriteBool(Section, 'ShowHint', Action.boShowHint);
    IniFile.WriteString(Section, 'HintText', Action.sHintText);
    IniFile.WriteInteger(Section, 'CompensationValue', Action.nCompensationValue);
    IniFile.WriteBool(Section, 'Debug', Action.boDebug);
  end;

  IniFile.WriteInteger('Setup', 'LockTime', g_Config.nLockTime);
  IniFile.WriteBool('Setup', 'SaveLockStatus', g_Config.boSaveLockStatus);
  IniFile.WriteBool('Setup', 'ShowLockLog', g_Config.boShowLockLog);
  IniFile.WriteString('Setup', 'ShowLockMsg', g_Config.sShowLockMsg);

  IniFile.WriteBool('Setup', 'SpeedClearData', g_Config.boSpeedClearData);

  IniFile.WriteInteger('Setup', 'MsgType', g_Config.btMsgType);
  IniFile.WriteInteger('Setup', 'MsgFColor', g_Config.btMsgFColor);
  IniFile.WriteInteger('Setup', 'MsgBColor', g_Config.btMsgBColor);

  IniFile.WriteInteger('Setup', 'UserShopSearchInterval', g_Config.dwUserShop_Search_Interval);
  IniFile.WriteBool('Setup', 'UserShopSearchShowHint', g_Config.boUserShop_Search_ShowHint);

  IniFile.WriteInteger('Setup', 'UserShopBuyInterval', g_Config.dwUserShop_Buy_Interval);
  IniFile.WriteBool('Setup', 'UserShopBuyShowHint', g_Config.boUserShop_Buy_ShowHint);

  IniFile.WriteInteger('Setup', 'TakeOnItemInterval', g_Config.dwTakeOn_Item_Interval);
  IniFile.WriteBool('Setup', 'TakeOnItemShowHint', g_Config.boTakeOn_Item_ShowHint);
  
  IniFile.WriteInteger('Setup', 'DealTryAttackInterval', g_Config.dwDealTry_Attack_Interval);
  IniFile.WriteBool('Setup', 'DealTryAttackShowHint', g_Config.boDealTry_Attack_ShowHint);

  IniFile.WriteInteger('Setup', 'BrutalAttackInterval', g_Config.dwBrutal_Attack_Interval);
  IniFile.WriteBool('Setup', 'BrutalAttackShowHint', g_Config.boBrutal_Attack_ShowHint);

  IniFile.WriteBool('Setup', 'ShowAttackLog', g_Config.boShowAttackLog);
  //IniFile.WriteBool('Setup', 'ShowDropConcurrentLog', g_Config.boShowDropConcurrentLog);
  IniFile.WriteInteger('Setup', 'ContinueSpeedPassIncTime', g_Config.dwContinueSpeedPassIncTime);

  IniFile.WriteInteger('Setup', 'CollectCount', g_Config.dwCollectCount);
  IniFile.WriteInteger('Setup', 'SpeedValue', g_Config.dwSpeedValue);

  IniFile.WriteBool('Setup', 'ContinueSpeedCloseSocket', g_Config.boContinueSpeedCloseSocket);
  IniFile.WriteInteger('Setup', 'ContinueSpeedCount', g_Config.nContinueSpeedCount);

  IniFile.WriteInteger('Setup', 'SumSpeedCheckTime', g_Config.nSumSpeedCheckTime);
  IniFile.WriteInteger('Setup', 'SumSpeedMaxCount', g_Config.nSumSpeedMaxCount);

  IniFile.WriteBool('Setup', 'ZeroCompensationValueClearPool', g_Config.boZeroCompensationValueClearPool);

  IniFile.WriteInteger('Setup', 'ClientUploadPickItemsTime', g_Config.dwClientUploadPickItemsTime);

  IniFile.Free;

  SetSaveStatus(False);
end;

procedure TFrmGameSpeed.seBColorChange(Sender: TObject);
begin
  g_Config.btMsgBColor := seBColor.Value;
  edtPreview.Color := ColorIndexToTColor(g_Config.btMsgBColor);
  edtPreview.Font.Color := ColorIndexToTColor(g_Config.btMsgFColor);
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seFColorChange(Sender: TObject);
begin
  g_Config.btMsgFColor := seFColor.Value;
  edtPreview.Color := ColorIndexToTColor(g_Config.btMsgBColor);
  edtPreview.Font.Color := ColorIndexToTColor(g_Config.btMsgFColor);
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.cbbMsgTypeChange(Sender: TObject);
begin
  g_Config.btMsgType := cbbMsgType.ItemIndex;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seLockTimeChange(Sender: TObject);
begin
  g_Config.nLockTime := seLockTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkSaveLockStatusClick(Sender: TObject);
begin
  g_Config.boSaveLockStatus := chkSaveLockStatus.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkShowLockLogClick(Sender: TObject);
begin
  g_Config.boShowLockLog := chkShowLockLog.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.edtShowLockMsgChange(Sender: TObject);
begin
  g_Config.sShowLockMsg := edtShowLockMsg.Text;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkSpeedClearDataClick(Sender: TObject);
begin
  g_Config.boSpeedClearData := chkSpeedClearData.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seDealTryAttackTimeChange(Sender: TObject);
begin
  g_Config.dwDealTry_Attack_Interval := seDealTryAttackTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkDealTryAttackHintClick(Sender: TObject);
begin
  g_Config.boDealTry_Attack_ShowHint := chkDealTryAttackHint.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seBrutalAttackTimeChange(Sender: TObject);
begin
  g_Config.dwBrutal_Attack_Interval := seBrutalAttackTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkBrutalAttackHintClick(Sender: TObject);
begin
  g_Config.boBrutal_Attack_ShowHint := chkBrutalAttackHint.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.vstAntiPlugActionGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: WideString);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);

  case Column of
    0:
      begin
        if NodeData.ActionMode >= amHitConcurrent then
          CellText := AntiPlugActionModeNames[NodeData.ActionMode] + ' >'
        else
          CellText := AntiPlugActionModeNames[NodeData.ActionMode] + ' <';
      end;
    1:
      begin
        if NodeData.ActionMode in [amHit, amSpell, amWalk, amRun,
          amWalkToHit {走路到攻击},     amHitToWalk {攻击到走路},
          amRunToHit {跑步到攻击},      amHitToRun {攻击到跑步},
          amWalkToSpell {走路到魔法},   amSpellToWalk {魔法到走路},
          amRunToSpell {跑步到魔法},    amSpellToRun {魔法到跑步},
          amTurnToHit {转向到攻击},   (*amHitToTurn {攻击到转向},*)
          amTurnToSpell {转向到魔法}, (*amSpellToTurn {魔法到转向},*)
          amCutMeatToHit {挖肉到攻击},  amCutMeatToSpell {挖肉到魔法},
          (*amMoveToTurn {移动到转向},*)    amTurnToMove {转向到移动},
          (*amMoveToCutMeat {移动到挖肉},*) amCutMeatToMove {挖肉到移动}] then
          CellText := '设置'
        else
          CellText := IntToStr(g_Config.ActionList[NodeData.ActionMode].nInterval);
      end;
    2:
      begin
        if NodeData.ActionMode >= amHitConcurrent then
          CellText := ActionProcessModeNames2[g_Config.ActionList[NodeData.ActionMode].ProcessMode]
        else
          CellText := ActionProcessModeNames[g_Config.ActionList[NodeData.ActionMode].ProcessMode];
      end;
    3:
      begin
        CellText := SumActionProcessModeNames[g_Config.ActionList[NodeData.ActionMode].SumProcessMode];
      end;
    //4: CellText := IntToStr(g_Config.ActionList[NodeData.ActionMode].nFloatingInterval);

    {
    5:
      begin
        if NodeData.ActionMode >= amHitConcurrent then
          CellText := '-'
        else
          CellText := IntToStr(g_Config.ActionList[NodeData.ActionMode].nCollectCount);
      end;
    6:
      begin
        if NodeData.ActionMode >= amHitConcurrent then
          CellText := '-'
        else
          CellText := IntToStr(g_Config.ActionList[NodeData.ActionMode].nCollectSpeedCount);
      end;
    }
    5: CellText := ' ';
    6: CellText := g_Config.ActionList[NodeData.ActionMode].sHintText;
    7:
      begin
        if NodeData.ActionMode in [amHit, amWalk, amRun] then
          CellText := IntToStr(g_Config.ActionList[NodeData.ActionMode].nCompensationValue)
        else
          CellText := ' ';
      end;
    8: CellText := ' ';
  end;

end;

procedure TFrmGameSpeed.vstAntiPlugActionDrawText(Sender: TBaseVirtualTree;
  TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
  const Text: WideString; const CellRect: TRect; var DefaultDraw: Boolean);
var
  //Details: TThemedElementDetails;
  //R: TRect;
  Pt: TPoint;
  ImageIndex: Integer;
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  TargetCanvas.Font.Color := Sender.Font.Color;
  if (NodeData.ActionMode >= amHitConcurrent) and (Column = 0) then
    TargetCanvas.Font.Color := clRed;
  if Column in [5, 8] then
  begin
    if Column = 5 then
      ImageIndex := Integer(g_Config.ActionList[NodeData.ActionMode].boShowHint)
    else
      ImageIndex := Integer(g_Config.ActionList[NodeData.ActionMode].boDebug);

    Pt.X := CellRect.Left + (CellRect.Right - CellRect.Left - ilCheck.Width) div 2;
    Pt.Y := CellRect.Top + (CellRect.Bottom - CellRect.Top - ilCheck.Height) div 2;
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
  end;
  (*
  else if Column = 2 then
  begin
    ImageIndex := Integer(g_Config.ActionList[NodeData.ActionMode].boProcessScript);
    Pt.X := CellRect.Left + 76;
    Pt.Y := CellRect.Top + (CellRect.Bottom - CellRect.Top - ilCheck.Height) div 2;
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
    Pt.X := Pt.X + ilCheck.Width + 3;
    TextY := CellRect.Top + (CellRect.Bottom - CellRect.Top - TargetCanvas.TextHeight('脚本')) div 2;
    TargetCanvas.TextOut(Pt.X, TextY, '脚本');

    {
    Pt.X := Pt.X + TargetCanvas.TextWidth('脚本') + 5;

    ImageIndex := Integer(g_Config.ActionList[NodeData.ActionMode].boProcessLock);
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
    Pt.X := Pt.X + ilCheck.Width + 3;
    TargetCanvas.TextOut(Pt.X, TextY, '锁定');
    }
  end;
  *)
end;

procedure TFrmGameSpeed.WMStartEditing(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstAntiPlugAction.EditNode(Node, Message.LParam);
end;

procedure TFrmGameSpeed.vstAntiPlugActionEditing(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
var
  NodeData: PNodeData;
begin
  Allowed := Node <> nil;
  if Allowed then
  begin
    NodeData := Sender.GetNodeData(Node);
    if (NodeData.ActionMode >= amHitConcurrent) and (Column in [1, 2]) then
      Allowed := False
    else if (not (NodeData.ActionMode in [amHit, amWalk, amRun])) and (Column = 7) then
      Allowed := False;
  end;
end;

procedure TFrmGameSpeed.vstAntiPlugActionNodeClick(
  Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  NodeData: PNodeData;
  R: TRect;
  P: TPoint;
begin
  if Assigned(HitInfo.HitNode) and Assigned(Sender.FocusedNode) then
  begin
    if HitInfo.HitColumn in [5, 8] then
    begin
      NodeData := Sender.GetNodeData(Sender.FocusedNode);

      if HitInfo.HitColumn = 5 then
        g_Config.ActionList[NodeData.ActionMode].boShowHint := not g_Config.ActionList[NodeData.ActionMode].boShowHint
      else
        g_Config.ActionList[NodeData.ActionMode].boDebug := not g_Config.ActionList[NodeData.ActionMode].boDebug;
      Sender.InvalidateNode(Sender.FocusedNode);

      SetSaveStatus(True);
    end
    else if HitInfo.HitColumn = 2 then
    begin
      NodeData := Sender.GetNodeData(Sender.FocusedNode);
      R := vstAntiPlugAction.GetDisplayRect(Sender.FocusedNode, HitInfo.HitColumn, False);
      GetCursorPos(P);
      P := vstAntiPlugAction.ScreenToClient(P);
      if P.X < R.Left + 76 then
      begin
        PostMessage(Self.Handle, WM_STARTEDITING, WPARAM(HitInfo.HitNode), HitInfo.HitColumn)
      end
      else
      begin
        g_Config.ActionList[NodeData.ActionMode].boProcessScript := not g_Config.ActionList[NodeData.ActionMode].boProcessScript;
        Sender.InvalidateNode(Sender.FocusedNode);
        SetSaveStatus(True);
      end;
    end
    {
    else if HitInfo.HitColumn = 3 then
    begin
      NodeData := Sender.GetNodeData(Sender.FocusedNode);
      R := vstAntiPlugAction.GetDisplayRect(Sender.FocusedNode, HitInfo.HitColumn, False);
      GetCursorPos(P);
      P := vstAntiPlugAction.ScreenToClient(P);
      if P.X <= R.Left + (R.Right - R.Left) div 2 then
        g_Config.ActionList[NodeData.ActionMode].boProcessScript := not g_Config.ActionList[NodeData.ActionMode].boProcessScript
      else
        g_Config.ActionList[NodeData.ActionMode].boProcessLock := not g_Config.ActionList[NodeData.ActionMode].boProcessLock;
      Sender.InvalidateNode(Sender.FocusedNode);

      SetSaveStatus(True);
    end
    }
    else if (HitInfo.HitColumn > 0) then
      PostMessage(Self.Handle, WM_STARTEDITING, WPARAM(HitInfo.HitNode), HitInfo.HitColumn)
  end;
  
end;

procedure TFrmGameSpeed.vstAntiPlugActionCreateEditor(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TPropertyEditLink.Create;
end;

procedure TFrmGameSpeed.vstAntiPlugActionChecked(Sender: TBaseVirtualTree;
  Node: PVirtualNode);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if vstAntiPlugAction.CheckState[Node] = csCheckedNormal then
      g_Config.ActionList[NodeData.ActionMode].boEnabled := True
    else
      g_Config.ActionList[NodeData.ActionMode].boEnabled := False;
    SetSaveStatus(True);
  end;
end;

procedure TFrmGameSpeed.btnDefaultClick(Sender: TObject);
begin
  {
  g_Config := g_DefaultConfig;
  vstAntiPlugAction.Invalidate;
  RefreshCtrlsStatus;
  SetSaveStatus(True);
  }
end;

procedure TFrmGameSpeed.vstAntiPlugActionGetHint(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex;
  var LineBreakStyle: TVTTooltipLineBreakStyle; var HintText: WideString);
var
  NodeData: PNodeData;
begin
  {
  if Column = 6 then
  begin
    HintText := '如果采集次数为8，超速次数为5，则最后的8次间隔中，有5次超速视为超速'
  end
  else
  }
  if Column = 0 then
  begin
    NodeData := Sender.GetNodeData(Node);
    if NodeData <> nil then
    begin
      if NodeData.ActionMode = amHitConcurrent then
        HintText := '此项默认勾选，取消后游戏中将出现多倍攻击，需求封双倍请配合攻击间隔设置！'
      else if NodeData.ActionMode = amSpellConcurrent then
        HintText := '此项默认勾选，取消后游戏中将出现多倍魔法，需求封双倍请配合攻击间隔设置！'
      else if NodeData.ActionMode = amMoveConcurrent then
        HintText := '此项默认勾选，取消后游戏中可能会出现暗杀或飞机速度的玩家！';
    end;
  end
  else if Column = 7 then
  begin
    HintText := '补偿值小于或等于0表示关闭'
  end;
end;

procedure TFrmGameSpeed.chkShowAttackLogClick(Sender: TObject);
begin
  g_Config.boShowAttackLog := chkShowAttackLog.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seContinueSpeedPassIncTimeChange(Sender: TObject);
begin
  g_Config.dwContinueSpeedPassIncTime := seContinueSpeedPassIncTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkShowDropConcurrentLogClick(Sender: TObject);
begin
  //g_Config.boShowDropConcurrentLog := chkShowDropConcurrentLog.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.vstAntiPlugActionChecking(Sender: TBaseVirtualTree;
  Node: PVirtualNode; var NewState: TCheckState; var Allowed: Boolean);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    Allowed := not (NodeData.ActionMode >= amHitConcurrent);
  end;
end;

procedure TFrmGameSpeed.chkContinueSpeedCloseSocketClick(Sender: TObject);
begin
  g_Config.boContinueSpeedCloseSocket := chkContinueSpeedCloseSocket.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seContinueSpeedCountChange(Sender: TObject);
begin
  g_Config.nContinueSpeedCount := seContinueSpeedCount.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seUserShopSearchTimeChange(Sender: TObject);
begin
  g_Config.dwUserShop_Search_Interval := seUserShopSearchTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seUserShopBuyTimeChange(Sender: TObject);
begin
  g_Config.dwUserShop_Buy_Interval := seUserShopBuyTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seTakeOnItemTimeChange(Sender: TObject);
begin
  g_Config.dwTakeOn_Item_Interval := seTakeOnItemTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkUserShopSearchHintClick(Sender: TObject);
begin
  g_Config.boUserShop_Search_ShowHint := chkUserShopSearchHint.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkUserShopBuyHintClick(Sender: TObject);
begin
  g_Config.boUserShop_Buy_ShowHint := chkUserShopBuyHint.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkTakeOnItemHintClick(Sender: TObject);
begin
  g_Config.boTakeOn_Item_ShowHint := chkTakeOnItemHint.Checked;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.vstAntiPlugActionAfterCellPaint(
  Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; CellRect: TRect);
var
  //Details: TThemedElementDetails;
  //R: TRect;
  Pt: TPoint;
  TextY: Integer;
  ImageIndex: Integer;
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  
  if Column = 2 then
  begin
    TargetCanvas.Font.Color := Sender.Font.Color;
    TargetCanvas.Brush.Style := bsClear;

    ImageIndex := Integer(g_Config.ActionList[NodeData.ActionMode].boProcessScript);
    Pt.X := CellRect.Left + 76;
    Pt.Y := CellRect.Top + (CellRect.Bottom - CellRect.Top - ilCheck.Height) div 2;
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
    Pt.X := Pt.X + ilCheck.Width + 3;
    TextY := CellRect.Top + (CellRect.Bottom - CellRect.Top - TargetCanvas.TextHeight('脚本')) div 2;
    TargetCanvas.TextOut(Pt.X, TextY, '脚本');
  end;
end;

procedure TFrmGameSpeed.seSumSpeedCheckTimeChange(Sender: TObject);
begin
  g_Config.nSumSpeedCheckTime := seSumSpeedCheckTime.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.seSumSpeedMaxCountChange(Sender: TObject);
begin
  g_Config.nSumSpeedMaxCount := seSumSpeedMaxCount.Value;
  SetSaveStatus(True);
end;

procedure TFrmGameSpeed.chkZeroCompensationValueClearPoolClick(
  Sender: TObject);
begin
  g_Config.boZeroCompensationValueClearPool := chkZeroCompensationValueClearPool.Checked;
  SetSaveStatus(True);
end;

end.

