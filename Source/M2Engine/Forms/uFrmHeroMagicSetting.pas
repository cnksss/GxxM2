unit uFrmHeroMagicSetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, VirtualTrees, uCustomHeroMagic, M2Share, Grobal2,
  StdCtrls, Spin, SpinEditEx, uCustomMagicUtils, ExtCtrls,
  uFrmHeroMagicCondition, M2Definition;

const
  WM_STARTEDITING_MAGIC = WM_USER + 801;

type
  TFrmHeroMagicSetting = class(TForm)
    pgcMain: TPageControl;
    ts1: TTabSheet;
    ts2: TTabSheet;
    ts3: TTabSheet;
    vstWizardMagic: TVirtualStringTree;
    vstTaosMagic: TVirtualStringTree;
    vstWarrMagic: TVirtualStringTree;
    pnl1: TPanel;
    btnSave: TButton;
    lbl34: TLabel;
    procedure FormCreate(Sender: TObject);
    procedure vstWarrMagicFreeNode(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure vstWarrMagicGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstWarrMagicNodeClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
    procedure vstWarrMagicEditing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstWarrMagicCreateEditor(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; out EditLink: IVTEditLink);
    procedure vstWarrMagicKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure btnSaveClick(Sender: TObject);
    procedure vstWarrMagicChecked(Sender: TBaseVirtualTree;
      Node: PVirtualNode);
    procedure vstWarrMagicDrawText(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect;
      var DefaultDraw: Boolean);
  private
    { Private declarations }
    procedure WMStartEditingMagic(var Message: TMessage); message WM_STARTEDITING_MAGIC;

    procedure SetCanSave;
  public
    { Public declarations }
  end;

  function ShowFrmHeroMagicSetting: Boolean;


implementation

{$R *.dfm}

type
  PNodeData = ^TNodeData;
  TNodeData = record
    HeroMagic: PHeroMagic;
    MagicName: string;
  end;

function ShowFrmHeroMagicSetting: Boolean;
var
  FrmHeroMagicSetting: TFrmHeroMagicSetting;
begin
  FrmHeroMagicSetting := TFrmHeroMagicSetting.Create(nil);
  try
    Result := FrmHeroMagicSetting.ShowModal = mrOK;
  finally
    FrmHeroMagicSetting.Free;
  end;
end;

type
  TMagicPropertyEditLink = class(TInterfacedObject, IVTEditLink)
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

destructor TMagicPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TMagicPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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

procedure TMagicPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
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

function TMagicPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

//----------------------------------------------------------------------------------------------------------------------

function TMagicPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

//----------------------------------------------------------------------------------------------------------------------

function TMagicPropertyEditLink.EndEdit: Boolean;
var
  TempValue: Integer;
  NodeData: PNodeData;
  IsChanged: Boolean;
begin
  IsChanged := False;
  Result := True;

  NodeData := FTree.GetNodeData(FNode);
  if NodeData = nil then Exit;
  case FColumn of
    1:
      begin
        if (FEdit as TComboBox).ItemIndex >= 0 then
        begin
          TempValue := Integer((FEdit as TComboBox).Items.Objects[(FEdit as TComboBox).ItemIndex]);
          IsChanged := NodeData^.HeroMagic.MagicID <> TempValue;
          if IsChanged then
          begin
            NodeData^.HeroMagic.MagicID := TempValue;
            NodeData^.MagicName := (FEdit as TComboBox).Items[(FEdit as TComboBox).ItemIndex];
          end;
        end;
      end;
    3:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := NodeData^.HeroMagic.UseRate <> TempValue;
        if IsChanged then
        begin
          NodeData^.HeroMagic.UseRate := TempValue;
        end;
      end;
    4:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := NodeData^.HeroMagic.AttackRange <> TempValue;
        if IsChanged then
        begin
          NodeData^.HeroMagic.AttackRange := TempValue;
        end;
      end;
    5:
      begin
        TempValue := (FEdit as TComboBox).ItemIndex;
        IsChanged := Integer(NodeData^.HeroMagic.AttackTarget) <> TempValue;
        if IsChanged then
        begin
          NodeData^.HeroMagic.AttackTarget := TMagicAttackTarget(TempValue);
        end;
      end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged then
  begin
    NodeData.HeroMagic.IsChanged := True;

    if (FTree.Owner is TFrmHeroMagicSetting) then
    begin
      TFrmHeroMagicSetting(FTree.Owner).SetCanSave;
    end;
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

function TMagicPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

//----------------------------------------------------------------------------------------------------------------------

function TMagicPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  I: Integer;
  NodeData: PNodeData;
  Magic: pTMagic;
  CustomMagicConfig: TCustomMagicConfig;
  AttackTarget: TMagicAttackTarget;
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
    1:
      begin
        FEdit := TComboBox.Create(nil);

        with (FEdit as TComboBox) do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;
          Clear;

          if Tree.Tag = 0 then
          begin
            for I := 0 to UserEngine.m_MagicList.Count - 1 do
            begin
              Magic := UserEngine.m_MagicList.Items[I];

              if CheckIsCustomMagic(Magic.wMagicId) and (Magic.MagicAttr = mtHero) then
              begin
                CustomMagicConfig := GetCustomMagicConfig(Magic.wMagicId);
                if (CustomMagicConfig <> nil) and (CustomMagicConfig.IsMagicWarr) then
                begin
                  Items.AddObject(Magic.sMagicName, TObject(CustomMagicConfig.MagicID));

                  if NodeData.HeroMagic.MagicID = CustomMagicConfig.MagicID then
                  begin
                    ItemIndex := Items.Count - 1;
                  end;
                end;
              end;
            end;
          end
          else
          begin
            for I := 0 to UserEngine.m_MagicList.Count - 1 do
            begin
              Magic := UserEngine.m_MagicList.Items[I];

              if CheckIsCustomMagic(Magic.wMagicId) and (Magic.MagicAttr = mtHero) then
              begin
                CustomMagicConfig := GetCustomMagicConfig(Magic.wMagicId);
                if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                begin
                  Items.AddObject(Magic.sMagicName, TObject(CustomMagicConfig.MagicID));

                  if NodeData.HeroMagic.MagicID = CustomMagicConfig.MagicID then
                  begin
                    ItemIndex := Items.Count - 1;
                  end;
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
        FEdit := TSpinEditEx.Create(nil);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          MinValue := 0;
          MaxValue := 0;

          Value := NodeData^.HeroMagic.UseRate;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    4:
      begin
        FEdit := TSpinEditEx.Create(nil);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          MinValue := 0;
          MaxValue := 20;

          Value := NodeData^.HeroMagic.AttackRange;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    5:
      begin
        FEdit := TComboBox.Create(nil);

        with (FEdit as TComboBox) do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;

          if FTree.Tag = 1 then
          begin
            for AttackTarget := Low(TMagicAttackTarget) to matMaster do
            begin
              Items.Add(TMagicAttackTargetNames[AttackTarget]);
            end;
          end
          else
          begin
            for AttackTarget := Low(TMagicAttackTarget) to High(TMagicAttackTarget) do
            begin
              Items.Add(TMagicAttackTargetNames[AttackTarget]);
            end;
          end;

          ItemIndex := Integer(NodeData.HeroMagic.AttackTarget);

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TMagicPropertyEditLink.ProcessMessage(var Message: TMessage);

begin
  FEdit.WindowProc(Message);
end;

//----------------------------------------------------------------------------------------------------------------------

procedure TMagicPropertyEditLink.SetBounds(R: TRect);

var
  Dummy: Integer;

begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

function GetHeroMagicUseConditionText(Condition: PHeroMagicUseCondition): string;
begin
  Result := '';
  if Condition.HeroLevelCheck.boChecked then
  begin
    if Condition.HeroLevelCheck.CompareType = hlctTargetLevel then
    begin
      Result := Result + Format('(英雄等级 %s 目标等级) + ', [TCompareSymbolNames[Condition.HeroLevelCheck.CompareSymbol]]);
    end
    else if Condition.HeroLevelCheck.CompareType = hlctLevelNumber then
    begin
      Result := Result + Format('(英雄等级 %s %d) + ', [TCompareSymbolNames[Condition.HeroLevelCheck.CompareSymbol], Condition.HeroLevelCheck.CompareValue]);
    end;
  end;

  if Condition.HeroHPCheck.boChecked then
  begin
    if Condition.HeroHPCheck.CompareType = hhpctNumber then
    begin
      Result := Result + Format('(英雄HP %s %d) + ', [TCompareSymbolNames[Condition.HeroHPCheck.CompareSymbol], Condition.HeroHPCheck.CompareValue]);
    end
    else
    begin
      Result := Result + Format('(英雄HP %s %d%%) + ', [TCompareSymbolNames[Condition.HeroHPCheck.CompareSymbol], Condition.HeroHPCheck.CompareValue]);
    end;
  end;

  if Condition.HeroMPCheck.boChecked then
  begin
    if Condition.HeroMPCheck.CompareType = hhpctNumber then
    begin
      Result := Result + Format('(英雄MP %s %d) + ', [TCompareSymbolNames[Condition.HeroMPCheck.CompareSymbol], Condition.HeroMPCheck.CompareValue]);
    end
    else
    begin
      Result := Result + Format('(英雄MP %s %d%%) + ', [TCompareSymbolNames[Condition.HeroMPCheck.CompareSymbol], Condition.HeroMPCheck.CompareValue]);
    end;
  end;

  if Condition.TargetHPCheck.boChecked then
  begin
    if Condition.TargetHPCheck.CompareType = hhpctNumber then
    begin
      Result := Result + Format('(目标HP %s %d) + ', [TCompareSymbolNames[Condition.TargetHPCheck.CompareSymbol], Condition.TargetHPCheck.CompareValue]);
    end
    else
    begin
      Result := Result + Format('(目标HP %s %d%%) + ', [TCompareSymbolNames[Condition.TargetHPCheck.CompareSymbol], Condition.TargetHPCheck.CompareValue]);
    end;
  end;

  if Condition.TargetMPCheck.boChecked then
  begin
    if Condition.TargetMPCheck.CompareType = hhpctNumber then
    begin
      Result := Result + Format('(目标MP %s %d) + ', [TCompareSymbolNames[Condition.TargetMPCheck.CompareSymbol], Condition.TargetMPCheck.CompareValue]);
    end
    else
    begin
      Result := Result + Format('(目标MP %s %d%%) + ', [TCompareSymbolNames[Condition.TargetMPCheck.CompareSymbol], Condition.TargetMPCheck.CompareValue]);
    end;
  end;

  if Condition.TargetStatusCheck.boPoisonDamageArmor then
  begin
    Result := Result + '红毒 + ';
  end;

  if Condition.TargetStatusCheck.boPoisonDecHealth then
  begin
    Result := Result + '绿毒 + ';
  end;

  if Condition.TargetStatusCheck.boPoisoning then
  begin
    Result := Result + '中毒 + ';
  end;

  if Condition.TargetStatusCheck.boPoisonStone then
  begin
    Result := Result + '麻痹 + ';
  end;

  if Condition.TargetStatusCheck.boFrozen then
  begin
    Result := Result + '冰冻 + ';
  end;

  if Condition.TargetStatusCheck.boForeverFrozen then
  begin
    Result := Result + '冰封 + ';
  end;

  if Condition.TargetStatusCheck.boForeverFrozen then
  begin
    Result := Result + '蛛网 + ';
  end;

  //-----------------------------------------------------

  if Condition.TargetStatusCheck.boUnPoisonDamageArmor then
  begin
    Result := Result + '无红毒 + ';
  end;

  if Condition.TargetStatusCheck.boUnPoisonDecHealth then
  begin
    Result := Result + '无绿毒 + ';
  end;

  if Condition.TargetStatusCheck.boUnPoisoning then
  begin
    Result := Result + '无毒 + ';
  end;

  if Condition.TargetStatusCheck.boUnPoisonStone then
  begin
    Result := Result + '无麻痹 + ';
  end;

  if Condition.TargetStatusCheck.boUnFrozen then
  begin
    Result := Result + '无冰冻 + ';
  end;

  if Condition.TargetStatusCheck.boUnForeverFrozen then
  begin
    Result := Result + '无冰封 + ';
  end;

  if Condition.TargetStatusCheck.boUnForeverFrozen then
  begin
    Result := Result + '无蛛网 + ';
  end;

  //-----------------------------------------------------
  if Condition.FriendCountCheck.boChecked then
  begin
    Result := Result + Format('(目标周围%d格朋友数量 > %d) + ', [Condition.FriendCountCheck.nCheckRange, Condition.FriendCountCheck.nCheckValue]);
  end;

  if Condition.EnemyCountCheck.boChecked then
  begin
    Result := Result + Format('(目标周围%d格敌人数量 > %d) + ', [Condition.EnemyCountCheck.nCheckRange, Condition.EnemyCountCheck.nCheckValue]);
  end;

  if Condition.boStraightLineCheck then
  begin
    Result := Result + '[直线] + ';
  end;

  if Length(Result) > 0 then
  begin
    Result := Copy(Result, 1, Length(Result) - 3);
  end;
end;

procedure TFrmHeroMagicSetting.FormCreate(Sender: TObject);
var
  I: Integer;
  HeroMagic: PHeroMagic;
  Tree: TVirtualStringTree;
  Node: PVirtualNode;
  NodeData: PNodeData;
  Magic: pTMagic;
begin
  vstWarrMagic.NodeDataSize := SizeOf(TNodeData);
  vstWarrMagic.Tag := 0;

  vstWizardMagic.NodeDataSize := SizeOf(TNodeData);
  vstWizardMagic.Tag := 1;

  vstTaosMagic.NodeDataSize := SizeOf(TNodeData);
  vstTaosMagic.Tag := 2;
  pgcMain.ActivePageIndex := 0;

  for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
  begin
    HeroMagic := g_CustomHeroMagicMgr.Items[I];
    if HeroMagic.MagicType = mtWarrAttack then
      Tree := vstWarrMagic
    else if HeroMagic.MagicType = mtWizardAttack then
      Tree := vstWizardMagic
    else
      Tree := vstTaosMagic;

    Node := Tree.AddChild(nil);
    Node.CheckType := ctCheckBox;
    if HeroMagic.Checked then
      Node.CheckState := csCheckedNormal
    else
      Node.CheckState := csUncheckedNormal;
      
    NodeData := Tree.GetNodeData(Node);
    NodeData.HeroMagic := HeroMagic;

    if CheckIsCustomMagic(HeroMagic.MagicID) then
    begin
      Magic := UserEngine.FindHeroMagic(HeroMagic.MagicID);
      if Magic <> nil then
      begin
        NodeData.MagicName := Magic.sMagicName
      end
      else
      begin
        NodeData.MagicName := '-';
      end;
    end
    else
    begin
      Magic := UserEngine.FindHeroMagic(HeroMagic.MagicID, mtHero);
      if Magic <> nil then
      begin
        NodeData.MagicName := Magic.sMagicName
      end
      else
      begin
        NodeData.MagicName := '-';
      end;
    end;
  end;

  btnSave.Enabled := False;
end;

procedure TFrmHeroMagicSetting.vstWarrMagicFreeNode(
  Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  NodeData.MagicName := '';
end;

procedure TFrmHeroMagicSetting.vstWarrMagicGetText(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);

  case Column of
    1: CellText := NodeData.MagicName;
    2:
      begin
        if NodeData.HeroMagic.IsCustomMagic then
          CellText := '自定义技能'
        else
          CellText := '普通技能';
      end;
    3: CellText := IntToStr(NodeData.HeroMagic.UseRate);
    4: CellText := IntToStr(NodeData.HeroMagic.AttackRange);
    5: CellText := TMagicAttackTargetNames[NodeData.HeroMagic.AttackTarget];
    6: CellText := GetHeroMagicUseConditionText(@NodeData.HeroMagic.Condition);
    7: if NodeData.HeroMagic.IsCustomMagic then CellText := '设置...';
  end;
end;

procedure TFrmHeroMagicSetting.WMStartEditingMagic(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WParam);
  // Note: the test whether a node can really be edited is done in the OnEditing event.

  if pgcMain.ActivePageIndex = 0 then
    vstWarrMagic.EditNode(Node, Message.LParam)
  else if pgcMain.ActivePageIndex = 1 then
    vstWizardMagic.EditNode(Node, Message.LParam)
  else if pgcMain.ActivePageIndex = 2 then
    vstTaosMagic.EditNode(Node, Message.LParam);
end;

procedure TFrmHeroMagicSetting.vstWarrMagicNodeClick(
  Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  NodeData: PNodeData;
  Condition: THeroMagicUseCondition;
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn >= 0) then
  begin
    if HitInfo.HitColumn = 7 then
    begin
      NodeData := Sender.GetNodeData(HitInfo.HitNode);
      if NodeData.HeroMagic.IsCustomMagic then
      begin
        Condition := NodeData.HeroMagic.Condition;
        if ShowFrmHeroMagicCondition(Condition) then
        begin
          if not CompareMem(@NodeData.HeroMagic.Condition, @Condition, SizeOf(Condition)) then
          begin
            NodeData.HeroMagic.Condition := Condition;
            NodeData.HeroMagic.IsChanged := True;
            SetCanSave;
          end;
        end;
      end;
    end
    else
      PostMessage(Self.Handle, WM_STARTEDITING_MAGIC, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
  end;
end;

procedure TFrmHeroMagicSetting.vstWarrMagicEditing(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var Allowed: Boolean);
var
  NodeData: PNodeData;
begin
  Allowed := (Node <> nil);
  if Allowed then
  begin
    NodeData := Sender.GetNodeData(Node);
    if not NodeData.HeroMagic.IsCustomMagic then
    begin
      Allowed := Column = 3;
    end
    else
    begin
      if Sender.Tag = 0 then
      begin
        Allowed := Column in [1, 3, 4];
      end
      else
      begin
        Allowed := Column in [1, 3, 4, 5];
      end;
    end;
  end;
end;

procedure TFrmHeroMagicSetting.vstWarrMagicCreateEditor(
  Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TMagicPropertyEditLink.Create;
end;

procedure TFrmHeroMagicSetting.vstWarrMagicKeyUp(Sender: TObject;
  var Key: Word; Shift: TShiftState);
var
  NodeData: PNodeData;
  HeroMagic: PHeroMagic;
  Node: PVirtualNode;
  Tree: TVirtualStringTree;
begin
  if (ssCtrl in Shift) then
  begin
    Tree := TVirtualStringTree(Sender);
    if Key = VK_INSERT then
    begin
      HeroMagic := g_CustomHeroMagicMgr.Add;
      HeroMagic.Checked := True;

      if Sender = vstWarrMagic then
        HeroMagic.MagicType := mtWarrAttack
      else if Sender = vstWizardMagic then
        HeroMagic.MagicType := mtWizardAttack
      else
        HeroMagic.MagicType := mtTaosAttack;

      HeroMagic.MagicID := 0;
      HeroMagic.IsCustomMagic := True;
      HeroMagic.UseRate := 0;
      HeroMagic.Checked := True;
      HeroMagic.AttackTarget := matEnemy;

      Node := Tree.AddChild(nil);
      NodeData := Tree.GetNodeData(Node);
      NodeData.HeroMagic := HeroMagic;

      Node.CheckType := ctCheckBox;
      if HeroMagic.Checked then
        Node.CheckState := csCheckedNormal
      else
        Node.CheckState := csUncheckedNormal;

      Tree.Selected[Node] := True;
      Tree.FocusedNode := Node;
      Tree.EditNode(Node, 0);

      SetCanSave;
    end
    else if Key = VK_DELETE then
    begin
      if Tree.FocusedNode <> nil then
      begin
        NodeData := Tree.GetNodeData(Tree.FocusedNode);
        if NodeData.HeroMagic.IsCustomMagic then
        begin
          Node := Tree.GetNext(Tree.FocusedNode);
          if Node = nil then
            Node := Tree.GetPrevious(Tree.FocusedNode);

          if g_CustomHeroMagicMgr.Remove(NodeData.HeroMagic) then
          begin
            Tree.DeleteNode(Tree.FocusedNode);
            if Node <> nil then
            begin
              Tree.FocusedNode := Node;
              Tree.Selected[Node] := True;
            end;

            SetCanSave;
          end;
        end;
      end;
    end;
  end;
end;

procedure TFrmHeroMagicSetting.btnSaveClick(Sender: TObject);
begin
  g_CustomHeroMagicMgr.SaveToFile;
  btnSave.Enabled := False;

  if pgcMain.ActivePageIndex = 0 then
    vstWarrMagic.Invalidate
  else if pgcMain.ActivePageIndex = 1 then
    vstWizardMagic.Invalidate
  else if pgcMain.ActivePageIndex = 2 then
    vstTaosMagic.Invalidate;
end;

procedure TFrmHeroMagicSetting.SetCanSave;
begin
  btnSave.Enabled := True;
end;

procedure TFrmHeroMagicSetting.vstWarrMagicChecked(
  Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if Node.CheckState = csCheckedNormal then
    begin
      NodeData.HeroMagic.Checked := True;
      NodeData.HeroMagic.IsChanged := True;
      SetCanSave;
    end
    else if Node.CheckState = csUncheckedNormal then
    begin
      NodeData.HeroMagic.Checked := False;
      NodeData.HeroMagic.IsChanged := True;
      SetCanSave;
    end;
  end;
end;

procedure TFrmHeroMagicSetting.vstWarrMagicDrawText(
  Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; const Text: string; const CellRect: TRect;
  var DefaultDraw: Boolean);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if NodeData.HeroMagic.IsChanged then
      TargetCanvas.Font.Color := clRed
    else if Sender.Selected[Node] and (Sender.Focused) then
      TargetCanvas.Font.Color := clHighlightText
    else
      TargetCanvas.Font.Color := Sender.Font.Color;
  end;
end;

end.
