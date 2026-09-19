unit LogManage;

interface

uses
  Windows, Messages, SysUtils, StrUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, StdCtrls, Mask, RzEdit, ExtCtrls, CheckLst, Menus, LDShare,
  Clipbrd, WideStrings, FileSearchPool, ThreadPool, VirtualTrees;

type
  TFrmLogManage = class(TForm)
    Panel: TPanel;
    Label1: TLabel;
    Label2: TLabel;
    DateTimeEditBegin: TRzDateTimeEdit;
    DateTimeEditEnd: TRzDateTimeEdit;
    btnStart: TButton;
    PopupMenu: TPopupMenu;
    pmiCopy: TMenuItem;
    StatusBar: TStatusBar;
    Timer: TTimer;
    chkObjName: TCheckBox;
    edtObjName: TEdit;
    chkItemName: TCheckBox;
    edtItemName: TEdit;
    chkItemID: TCheckBox;
    edtItemID: TEdit;
    chkActObjName: TCheckBox;
    edtActObjName: TEdit;
    pmiCopyLine: TMenuItem;
    N1: TMenuItem;
    pmiExportLine: TMenuItem;
    pmiExportAll: TMenuItem;
    dlgSave: TSaveDialog;
    pnlClient: TPanel;
    vstLog: TVirtualStringTree;
    vstLogType: TVirtualStringTree;
    splLeft: TSplitter;
    chkObjType: TCheckBox;
    cbbActionType: TComboBox;
    procedure FormCreate(Sender: TObject);
    procedure CheckListBoxClickCheck(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure DateTimeEditBeginDateTimeChange(Sender: TObject;
      DateTime: TDateTime);
    procedure DateTimeEditEndDateTimeChange(Sender: TObject;
      DateTime: TDateTime);
    procedure TimerTimer(Sender: TObject);
    procedure btnStartClick(Sender: TObject);
    procedure pmiCopyClick(Sender: TObject);
    procedure vstLogGetText(Sender: TBaseVirtualTree; Node: PVirtualNode;
      Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure vstLogDrawText(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: WideString; const CellRect: TRect;
      var DefaultDraw: Boolean);
    procedure vstLogKeyAction(Sender: TBaseVirtualTree; var CharCode: Word;
      var Shift: TShiftState; var DoDefault: Boolean);
    procedure PopupMenuPopup(Sender: TObject);
    procedure pmiCopyLineClick(Sender: TObject);
    procedure pmiExportLineClick(Sender: TObject);
    procedure pmiExportAllClick(Sender: TObject);
    procedure vstLogTypeGetText(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure vstLogTypeCollapsing(Sender: TBaseVirtualTree;
      Node: PVirtualNode; var Allowed: Boolean);
    procedure vstLogTypeDrawText(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: WideString; const CellRect: TRect;
      var DefaultDraw: Boolean);
    procedure vstLogBeforeItemErase(Sender: TBaseVirtualTree;
      TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
      var ItemColor: TColor; var EraseAction: TItemEraseAction);
    procedure vstLogHeaderClick(Sender: TVTHeader;
      HitInfo: TVTHeaderHitInfo);
    procedure vstLogCompareNodes(Sender: TBaseVirtualTree; Node1,
      Node2: PVirtualNode; Column: TColumnIndex; var Result: Integer);
  private
    { Private declarations }
    FLogDataList: TThreadList;
    FTaskCount: Integer;

    procedure DoSearchFile(Path: string; FileList: TStringList);
    procedure ClearLogDataList;
    procedure QuickSortLogData(List: TList; L, R: Integer);

    procedure OnTaskComplete(Task: TPoolTask);

    procedure WMSYSCommand(var Msg: TWMSYSCommand); message WM_SYSCOMMAND;
  public
    { Public declarations }
  end;

var
  FrmLogManage: TFrmLogManage;

  g_SearchManager: TSearchManager;

const
  LOG_ActionNone          = 00;
  
  LOG_ItemRefining        = 01;  // 炼制物品
  LOG_ItemMake            = 02;  // 制造物品
  LOG_ItemNpcGive         = 03;  // NPC给予
  LOG_ItemNpcRecycle      = 04;  // NPC回收
  LOG_ItemPickup          = 05;  // 捡取物品
  LOG_ItemDrop            = 06;  // 丢弃物品
  LOG_ButchItem           = 07;  // 挖到物品
  LOG_ItemFall            = 08;  // 掉落物品 (死亡掉落等)
  LOG_ItemDisappear       = 09;  // 物品消失 (用完消失，死亡消失，持久为0，限时到时 等)
  LOG_ItemSell            = 10;  // 卖出物品
  LOG_ItemBuy             = 11;  // 买入物品
  LOG_ItemInlaid          = 12;  // 镶嵌物品
  LOG_ItemDisassemble     = 13;  // 拆开镶嵌
  LOG_ItemUpgrade         = 14;  // 物品升级
  LOG_ItemTrading         = 15;  // 交易物品
  LOG_ItemChallenge       = 16;  // 挑战物品
  LOG_ItemPutInto         = 17;  // 放入物品
  LOG_ItemTakeBack        = 18;  // 取回物品
  LOG_ItemMove            = 19;  // 移动物品
  LOG_ItemSplit           = 20;  // 拆分物品
  LOG_ItemOverlap         = 21;  // 叠加物品
  LOG_ItemSellPrice       = 22;  // 卖出费用
  LOG_ItemBuyPrice        = 23;  // 买入费用
  LOG_ItemUpdate          = 24;  // 物品更新
  LOG_ItemTakeOn          = 25;  // 穿戴装备
  LOG_ItemTakeOff         = 26;  // 脱下装备
  LOG_ItemNpcMonDrop      = 27;  // MonDropItems命令爆出

  LOG_GoldChange          = 50;  // 金币改变
  LOG_GameGoldChange      = 51;  // 元宝改变
  LOG_GamePointChange     = 52;  // 游戏点改变
  LOG_GameDiamondChange   = 53;  // 金刚石改变
  LOG_GameGirdChange      = 54;  // 灵符改变
  LOG_CreditPointChange   = 55;  // 声望改变
  LOG_GamegLoryChange     = 56;  // 荣誉值改变
  LOG_LevelChange         = 57;  // 等级改变
  LOG_AbilPointChange     = 58;  // 属性点改变
                                 
  LOG_CastleSaveMoney     = 60;  // 城堡存钱
  LOG_CastleGetMoney      = 61;  // 城堡取钱

  LOG_PlayerDie           = 70;  // 人物死亡
  LOG_PlayerLogon         = 71;  // 人物上线
  LOG_PlayerLogOff        = 72;  // 人物下线
  LOG_PlayerTrading       = 73;  // 角色交易

  ActionNames: array[0..42 - 1] of TLogAction = (
    // 物品相关
    (Action: LOG_ItemRefining;       Text: '炼制物品'),
    (Action: LOG_ItemMake;           Text: '制造物品'),
    (Action: LOG_ItemNpcGive;        Text: 'NPC给予'),
    (Action: LOG_ItemNpcRecycle;     Text: 'NPC回收'),
    (Action: LOG_ItemPickup;         Text: '捡取物品'),
    (Action: LOG_ItemDrop;           Text: '丢弃物品'),
    (Action: LOG_ButchItem;          Text: '挖到物品'),
    (Action: LOG_ItemFall;           Text: '掉落物品'),
    (Action: LOG_ItemDisappear;      Text: '物品消失'),
    (Action: LOG_ItemSell;           Text: '卖出物品'),
    (Action: LOG_ItemBuy;            Text: '买入物品'),
    (Action: LOG_ItemSellPrice;      Text: '卖出费用'),
    (Action: LOG_ItemBuyPrice;       Text: '买入费用'),
    (Action: LOG_ItemInlaid;         Text: '镶嵌物品'),
    (Action: LOG_ItemDisassemble;    Text: '拆开镶嵌'),
    (Action: LOG_ItemUpgrade;        Text: '物品升级'),
    (Action: LOG_ItemTrading;        Text: '交易物品'),
    (Action: LOG_ItemChallenge;      Text: '挑战物品'),
    (Action: LOG_ItemPutInto;        Text: '放入物品'),
    (Action: LOG_ItemTakeBack;       Text: '取回物品'),
    (Action: LOG_ItemMove;           Text: '移动物品'),
    (Action: LOG_ItemSplit;          Text: '拆分物品'),
    (Action: LOG_ItemOverlap;        Text: '叠加物品'),
    (Action: LOG_ItemUpdate;         Text: '物品更新'),
    (Action: LOG_ItemTakeOn;         Text: '穿戴装备'),
    (Action: LOG_ItemTakeOff;        Text: '脱下装备'),
    (Action: LOG_ItemNpcMonDrop;     Text: '命令爆出'),

    // 普通数据
    (Action: LOG_CastleSaveMoney;    Text: '城堡存钱'),
    (Action: LOG_CastleGetMoney;     Text: '城堡取钱'),
    (Action: LOG_GoldChange;         Text: '金币改变'),
    (Action: LOG_GameGoldChange;     Text: '元宝改变'),
    (Action: LOG_GamePointChange;    Text: '游戏点改变'),
    (Action: LOG_GameDiamondChange;  Text: '金刚石改变'),
    (Action: LOG_GameGirdChange;     Text: '灵符改变'),
    (Action: LOG_CreditPointChange;  Text: '声望改变'),
    (Action: LOG_GamegLoryChange;    Text: '荣誉值改变'),
    (Action: LOG_LevelChange;        Text: '等级改变'),
    (Action: LOG_AbilPointChange;    Text: '属性点改变'),

    // 其他动作
    (Action: LOG_PlayerDie;          Text: '人物死亡'),
    (Action: LOG_PlayerLogon;        Text: '人物上线'),
    (Action: LOG_PlayerLogOff;       Text: '人物下线'),
    (Action: LOG_PlayerTrading;      Text: '角色交易')
   );

implementation

uses HUtil32;

{$R *.dfm}

type
  TActionCategory = (acAll, acParent, acSpecify);
  PNodeData = ^TNodeData;
  TNodeData = record
    Category: TActionCategory;
    Action: Byte;
    Text: string;
  end;

function GetActString(nAct: LongWord): string;
var
  I: Integer;
  W1: Word;
  B1{, B2}: Byte;
begin
  // 日志类型暂时只用了Word
  W1 := LoWord(nAct);
  B1 := LoByte(W1);
  //B2 := HiByte(W1);

  Result := '无法分析';
  if nAct >= 0 then
  begin
    for I := 0 to Length(ActionNames) - 1 do
    begin
      if (ActionNames[I].Action = B1) then
      begin
        Result := ActionNames[I].Text;
        Exit;
      end;
    end;
  end;
end;

function LastDirectoryName(Directory: string): string;
var
  I: Integer;
begin
  Result := '';
  if Directory[Length(Directory)] = '\' then
    Directory := Copy(Directory, 1, Length(Directory) - 1);
  for I := Length(Directory) downto 1 do
    if Directory[I] = '\' then
    begin
      Result := Copy(Directory, I + 1, Length(Directory) - I + 1);
      break;
    end;
end;

procedure TFrmLogManage.DateTimeEditBeginDateTimeChange(Sender: TObject;
  DateTime: TDateTime);
begin
  if DateTime > DateTimeEditEnd.Date then
    DateTimeEditEnd.Date := DateTime;
end;

procedure TFrmLogManage.DateTimeEditEndDateTimeChange(Sender: TObject;
  DateTime: TDateTime);
begin
  if DateTime < DateTimeEditBegin.Date then
    DateTimeEditEnd.Date := DateTimeEditBegin.Date;
end;

procedure TFrmLogManage.DoSearchFile(Path: string; FileList: TStringList);
var
  Info: TSearchRec;
  FileName: string;
begin
  Path := IncludeTrailingBackslash(Path);

  if FindFirst(Path + '*.nlf', faAnyFile, Info) = 0 then
  begin
    repeat
      if Info.Attr and faDirectory = faDirectory then Continue;
      if not SameText(Copy(Info.Name, 1, 4), 'log-') then Continue;

      FileName := Path + Info.Name;

      FileList.Add(FileName);

    until FindNext(Info) <> 0;
  end;

  FindClose(Info);
end;

procedure TFrmLogManage.btnStartClick(Sender: TObject);
var
  I, {II, III, IIII,} nDay{, nIdx, nCount, nValue}: Integer;
  Year, Month, Day{, Hour, Min, Sec, MSec}: Word;
  SearchDay: TDate;
  sLogDir{, sLogFile}: string;
  {LoadLines,} FileList: TStringList;
  //sText, s01, s02, s03, s04, s05, s06, s07, s08, s09, s10, s11, s12, s13: string;
  //LogData: pTLogData;
  //ListItem: TListItem;
  sObjName, sActObjName, sItemName, sItemID: string;
  nItemID: Integer;
  SearchWhere: Integer;
  //SL: TWideStringList;

  //StartTick: LongWord;

  Node: PVirtualNode;
  NodeData: PNodeData;
  Task: TSearchTask;
begin
  if btnStart.Tag = 1 then
  begin
    g_SearchManager.CancelAndClearAllTask;
    btnStart.Tag := 0;
    btnStart.Caption := '开始查询';
    Exit;
  end;

  sObjName := Trim(edtObjName.Text);
  sActObjName := Trim(edtActObjName.Text);
  sItemName := Trim(edtItemName.Text);
  sItemID := Trim(edtItemID.Text);

  if chkObjName.Checked and (Length(sObjName) = 0) then
  begin
    Application.MessageBox('请输入查询的人物名称 ！！！', '提示信息', MB_ICONQUESTION);
    edtObjName.SetFocus;
    Exit;
  end;

  if chkActObjName.Checked and (Length(sActObjName) = 0) then
  begin
    Application.MessageBox('请输入查询的交易对象 ！！！', '提示信息', MB_ICONQUESTION);
    edtActObjName.SetFocus;
    Exit;
  end;

  if chkItemName.Checked and (Length(sItemName) = 0) then
  begin
    Application.MessageBox('请输入查询的物品名称 ！！！', '提示信息', MB_ICONQUESTION);
    edtItemName.SetFocus;
    Exit;
  end;

  if chkItemID.Checked and (Length(sItemID) = 0) then
  begin
    Application.MessageBox('请输入查询的物品ID ！！！', '提示信息', MB_ICONQUESTION);
    edtItemID.SetFocus;
    Exit;
  end;

  nItemID := StrToIntDef(sItemID, 0);
  if chkItemID.Checked and (nItemID <= 0) then
  begin
    Application.MessageBox('物品ID必须是正整数 ！！！', '提示信息', MB_ICONQUESTION);
    edtItemID.SetFocus;
    Exit;
  end;

  SearchWhere := 0;
  if chkObjName.Checked then SearchWhere := SearchWhere + 1;
  if chkObjType.Checked then SearchWhere := SearchWhere + 2;
  if chkActObjName.Checked then SearchWhere := SearchWhere + 4;
  if chkItemName.Checked then SearchWhere := SearchWhere + 8;
  if chkItemID.Checked then SearchWhere := SearchWhere + 16;


  btnStart.Tag := 1;
  btnStart.Caption := '停止查询';

  StatusBar.Panels[3].Text := '';

  vstLog.Clear;

  ClearLogDataList;

  g_SearchManager.SearchWhere := SearchWhere;
  g_SearchManager.SearchObjName := sObjName;
  g_SearchManager.SearchActObjName := sActObjName;
  g_SearchManager.SearchActObjType := cbbActionType.ItemIndex;    
  g_SearchManager.SearchItemName := sItemName;
  g_SearchManager.SearchItemID := nItemID;
  g_SearchManager.SearchDataList := FLogDataList;

  Node := vstLogType.GetFirst();
  if Node.CheckState = csCheckedNormal then
  begin
    for I := Low(g_SearchManager.SearchActions) to High(g_SearchManager.SearchActions) do
    begin
      g_SearchManager.SearchActions[I] := True;
    end;
  end
  else
  begin
    for I := Low(g_SearchManager.SearchActions) to High(g_SearchManager.SearchActions) do
    begin
      g_SearchManager.SearchActions[I] := False;
    end;

    Node := vstLogType.GetFirst();
    while Node <> nil do
    begin
      if Node.CheckState = csCheckedNormal then
      begin
        NodeData := vstLogType.GetNodeData(Node);
        if NodeData.Category = acSpecify then
        begin
          g_SearchManager.SearchActions[NodeData.Action] := True;
        end;
      end;

      Node := vstLogType.GetNext(Node);
    end;
  end;

  FileList := TStringList.Create;
  try
    nDay := GetDayCount(DateTimeEditEnd.Date, DateTimeEditBegin.Date);
    for I := 0 to nDay do
    begin
      SearchDay := DateTimeEditBegin.Date + I;
      DecodeDate(SearchDay, Year, Month, Day);
      sLogDir := {ExtractFilePath(ParamStr(0)) + } IncludeTrailingBackslash(sBaseDir) + IntToStr(Year) + '-' + IntToString(Month) + '-' + IntToString(Day);
      if DirectoryExists(sLogDir) then
      begin
        DoSearchFile(sLogDir, FileList);
      end;
    end;

    FTaskCount := FileList.Count;

    if FTaskCount = 0 then
    begin
      StatusBar.Panels[3].Text := '';
      StatusBar.Panels[2].Text := '查询已完成';
      btnStart.Tag := 0;
      btnStart.Caption := '开始查询';
      Exit;
    end;
    
    for I := 0 to FileList.Count - 1 do
    begin
      Task := TSearchTask.Create;
      Task.TaskID := I;
      Task.ShowPanel := StatusBar.Panels[2];
      Task.FileName := FileList.Strings[I];

      g_SearchManager.AddTask(Task);
    end;

  finally
    FileList.Free;
  end;
end;

procedure TFrmLogManage.CheckListBoxClickCheck(Sender: TObject);
//var
  //I: Integer;
begin
  (*
  if CheckListBox.Selected[0] { and CheckListBox.Checked[0] } then
  begin
    for I := 1 to CheckListBox.Count - 1 do
      CheckListBox.Checked[I] := CheckListBox.Checked[0];
  end;
  *)
end;

procedure TFrmLogManage.FormCreate(Sender: TObject);
var
  I: Integer;
  ActorType: TLogActorType;
  ParentNode, Node: PVirtualNode;
  NodeData: PNodeData;
begin
  FLogDataList := TThreadList.Create;

  DateTimeEditBegin.Date := Date;
  DateTimeEditEnd.Date := Date;


  cbbActionType.Clear;
  for ActorType := Low(TLogActorType) to High(TLogActorType) do
  begin
    cbbActionType.Items.Add(LogActorTypeNames[ActorType]);
  end;
  cbbActionType.ItemIndex := 0;

  vstLogType.Clear;
  vstLogType.NodeDataSize := SizeOf(TNodeData);

  ParentNode := vstLogType.AddChild(nil);
  ParentNode.CheckType := ctTriStateCheckBox;
  ParentNode.CheckState := csCheckedNormal;
  NodeData := vstLogType.GetNodeData(ParentNode);
  NodeData.Category := acAll;
  NodeData.Text := '查询所有';

  ParentNode := vstLogType.AddChild(nil);
  ParentNode.CheckType := ctTriStateCheckBox;
  ParentNode.CheckState := csCheckedNormal;
  NodeData := vstLogType.GetNodeData(ParentNode);
  NodeData.Category := acParent;
  NodeData.Text := '物品相关';
  for I := 0 to 25 do
  begin
    Node := vstLogType.AddChild(ParentNode);
    Node.CheckType := ctTriStateCheckBox;
    Node.CheckState := csCheckedNormal;

    NodeData := vstLogType.GetNodeData(Node);
    NodeData.Category := acSpecify;
    NodeData.Action := ActionNames[I].Action;
    NodeData.Text := ActionNames[I].Text;
  end;

  ParentNode := vstLogType.AddChild(nil);
  ParentNode.CheckType := ctTriStateCheckBox;
  ParentNode.CheckState := csCheckedNormal;
  NodeData := vstLogType.GetNodeData(ParentNode);
  NodeData.Category := acParent;
  NodeData.Text := '普通数据';
  for I := 26 to 37 do
  begin
    Node := vstLogType.AddChild(ParentNode);
    Node.CheckType := ctTriStateCheckBox;
    Node.CheckState := csCheckedNormal;

    NodeData := vstLogType.GetNodeData(Node);
    NodeData.Category := acSpecify;
    NodeData.Action := ActionNames[I].Action;
    NodeData.Text := ActionNames[I].Text;
  end;

  ParentNode := vstLogType.AddChild(nil);
  ParentNode.CheckType := ctTriStateCheckBox;
  ParentNode.CheckState := csCheckedNormal;
  NodeData := vstLogType.GetNodeData(ParentNode);
  NodeData.Category := acParent;
  NodeData.Text := '其他动作';
  for I := 38 to 41 do
  begin
    Node := vstLogType.AddChild(ParentNode);
    Node.CheckType := ctTriStateCheckBox;
    Node.CheckState := csCheckedNormal;

    NodeData := vstLogType.GetNodeData(Node);
    NodeData.Category := acSpecify;
    NodeData.Action := ActionNames[I].Action;
    NodeData.Text := ActionNames[I].Text;
  end;


  vstLogType.FullExpand(nil);

  Timer.Enabled := True;

  g_SearchManager.OnTaskComplete := OnTaskComplete;
end;

procedure TFrmLogManage.FormDestroy(Sender: TObject);
begin
  ClearLogDataList;
  FLogDataList.Free;
end;

procedure SetClipboardText(const Text: WideString);
var
  Count: Integer;
  Handle: HGLOBAL;
  Ptr: Pointer;
begin
  Count := (Length(Text)+1)*SizeOf(WideChar);
  Handle := GlobalAlloc(GMEM_MOVEABLE, Count);
  try
    Win32Check(Handle<>0);
    Ptr := GlobalLock(Handle);
    Win32Check(Assigned(Ptr));
    Move(PWideChar(Text)^, Ptr^, Count);
    GlobalUnlock(Handle);
    Clipboard.SetAsHandle(CF_UNICODETEXT, Handle);
  except
    GlobalFree(Handle);
    raise;
  end;
end;

procedure TFrmLogManage.pmiCopyClick(Sender: TObject);
var
  sText: string;
begin
  sText := vstLog.Text[vstLog.FocusedNode, vstLog.FocusedColumn];
  SetClipboardText(sText);
end;

procedure TFrmLogManage.pmiCopyLineClick(Sender: TObject);
var
  S: string;
  Node: PVirtualNode;

  P: Pointer;
  LogData: pTLogData;
begin
  if vstLog.SelectedCount > 0 then
  begin
    Node := vstLog.GetFirstSelected();
    while Node <> nil do
    begin
      P := vstLog.GetNodeData(Node);
      if P <> nil then
      begin
        LogData := pTLogData(P^);

        S := S + IntToStr(LogData.nIndx) + #9 +
          GetActString(LogData.nAct) + #9 +
          LogData.sMapName + #9 +
          IntToStr(LogData.nX) + #9 +
          IntToStr(LogData.nY) + #9 +
          LogData.sObjectName + #9 +
          LogActorTypeNames[LogData.ObjectType] + #9 +
          LogData.sItemName + #9 +
          IntToStr(LogData.nItemIndex) + #9 +
          LogData.sActObjectName + #9 +
          IntToStr(LogData.nData1) + #9 +
          IntToStr(LogData.nData2) + #9 +
          LogData.LogDesc + #9 +
          FormatDateTime('yyyy-mm-dd hh:nn:ss', LogData.Date) + sLineBreak;
      end;

      Node := vstLog.GetNextSelected(Node);
    end;

    if Length(S) > 0 then
    begin
      S := Copy(S, 1, Length(S) - 2);
      SetClipboardText(S);
    end;
  end;
end;

procedure TFrmLogManage.pmiExportLineClick(Sender: TObject);
var
  SL: TStringList;
  Node: PVirtualNode;

  P: Pointer;
  LogData: pTLogData;
begin
  if vstLog.SelectedCount > 0 then
  begin
    SL := TStringList.Create;
    try
      Node := vstLog.GetFirstSelected();
      while Node <> nil do
      begin
        P := vstLog.GetNodeData(Node);
        if P <> nil then
        begin
          LogData := pTLogData(P^);

          SL.Add(
            IntToStr(LogData.nIndx) + #9 +
            GetActString(LogData.nAct) + #9 +
            LogData.sMapName + #9 +
            IntToStr(LogData.nX) + #9 +
            IntToStr(LogData.nY) + #9 +
            LogData.sObjectName + #9 +
            LogActorTypeNames[LogData.ObjectType] + #9 +
            LogData.sItemName + #9 +
            IntToStr(LogData.nItemIndex) + #9 +
            LogData.sActObjectName + #9 +
            IntToStr(LogData.nData1) + #9 +
            IntToStr(LogData.nData2) + #9 +
            LogData.LogDesc + #9 +
            FormatDateTime('yyyy-mm-dd hh:nn:ss', LogData.Date)
          );
        end;

        Node := vstLog.GetNextSelected(Node);
      end;

      if SL.Count > 0 then
      begin
        if dlgSave.Execute then
        begin
          dlgSave.FileName := ChangeFileExt(dlgSave.FileName, '.txt');
          SL.SaveToFile(dlgSave.FileName);
        end;
      end;
    finally
      SL.Free;
    end;
  end;
end;

procedure TFrmLogManage.pmiExportAllClick(Sender: TObject);
var
  SL: TStringList;
  Node: PVirtualNode;

  P: Pointer;
  LogData: pTLogData;
begin
  if vstLog.RootNodeCount > 0 then
  begin
    SL := TStringList.Create;
    try
      Node := vstLog.GetFirst();
      while Node <> nil do
      begin
        P := vstLog.GetNodeData(Node);
        if P <> nil then
        begin
          LogData := pTLogData(P^);

          SL.Add(
            IntToStr(LogData.nIndx) + #9 +
            GetActString(LogData.nAct) + #9 +
            LogData.sMapName + #9 +
            IntToStr(LogData.nX) + #9 +
            IntToStr(LogData.nY) + #9 +
            LogData.sObjectName + #9 +
            LogActorTypeNames[LogData.ObjectType] + #9 +
            LogData.sItemName + #9 +
            IntToStr(LogData.nItemIndex) + #9 +
            LogData.sActObjectName + #9 +
            IntToStr(LogData.nData1) + #9 +
            IntToStr(LogData.nData2) + #9 +
            LogData.LogDesc + #9 +
            FormatDateTime('yyyy-mm-dd hh:nn:ss', LogData.Date)
          );
        end;

        Node := vstLog.GetNext(Node);
      end;

      if SL.Count > 0 then
      begin
        if dlgSave.Execute then
        begin
          dlgSave.FileName := ChangeFileExt(dlgSave.FileName, '.txt');
          SL.SaveToFile(dlgSave.FileName);
        end;
      end;
    finally
      SL.Free;
    end;
  end;
end;

procedure TFrmLogManage.TimerTimer(Sender: TObject);
begin
  Timer.Enabled := False;

end;

procedure TFrmLogManage.ClearLogDataList;
var
  I: Integer;
  List: TList;
begin
  List := FLogDataList.LockList;
  try
    for I := 0 to List.Count - 1 do
    begin
      Dispose(pTLogData(List[I]));
    end;
    List.Clear;
  finally
    FLogDataList.UnlockList;
  end;
end;

procedure TFrmLogManage.OnTaskComplete(Task: TPoolTask);
var
  I: Integer;
  List: TList;
  LogData: pTLogData;
begin
  Dec(FTaskCount);
  if FTaskCount = 0 then
  begin
    List := FLogDataList.LockList;
    try
      if List.Count > 1 then
      begin
        QuickSortLogData(List, 0, List.Count - 1);
      end;

      vstLog.BeginUpdate;
      try
        for I := 0 to List.Count - 1 do
        begin
          LogData := List.Items[I];
          LogData.nIndx := I;

          vstLog.AddChild(nil, LogData);
        end;
      finally
        vstLog.EndUpdate;
      end;
    finally
      FLogDataList.UnlockList;
    end;

    StatusBar.Panels[3].Text := '';
    StatusBar.Panels[2].Text := '查询已完成';
    btnStart.Tag := 0;
    btnStart.Caption := '开始查询';
  end;
end;

procedure TFrmLogManage.QuickSortLogData(List: TList; L, R: Integer);

  function SCompare(Item1, Item2: pTLogData): Integer;
  begin
    Result := Item1.nIndx - Item2.nIndx;
  end;

var
  I, J: Integer;
  P, T: Pointer;
begin
  repeat
    I := L;
    J := R;
    P := List.Items[(L + R) shr 1];
    repeat
      while SCompare(List.Items[I], P) < 0 do
        Inc(I);
      while SCompare(List.Items[J], P) > 0 do
        Dec(J);
      if I <= J then
      begin
        T := List.Items[I];
        List.Items[I] := List.Items[J];
        List.Items[J] := T;
        Inc(I);
        Dec(J);
      end;
    until I > J;
    if L < J then
      QuickSortLogData(List, L, J);
    L := I;
  until I >= R;
end;

procedure TFrmLogManage.vstLogGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: WideString);
var
  P: Pointer;
  LogData: pTLogData;
begin
  P := Sender.GetNodeData(Node);
  if P <> nil then
  begin
    LogData := pTLogData(P^);
    case Column of
      0: CellText := IntToStr(LogData.nIndx);
      1: CellText := GetActString(LogData.nAct);
      2: CellText := LogData.sMapName;
      3: CellText := IntToStr(LogData.nX);
      4: CellText := IntToStr(LogData.nY);
      5: CellText := LogData.sObjectName;
      6: CellText := LogActorTypeNames[LogData.ObjectType];
      7: CellText := LogData.sItemName;
      8: CellText := IntToStr(LogData.nItemIndex);
      9: CellText := LogData.sActObjectName;
      10:CellText := IntToStr(LogData.nData1);
      11:CellText := IntToStr(LogData.nData2);
      12:CellText := LogData.LogDesc;
      13: CellText := FormatDateTime('yyyy-mm-dd hh:nn:ss', LogData.Date);
    end;
  end;
end;

procedure TFrmLogManage.vstLogDrawText(Sender: TBaseVirtualTree;
  TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
  const Text: WideString; const CellRect: TRect; var DefaultDraw: Boolean);
begin
  if Sender.Focused and (Sender.FocusedNode = Node) and (Sender.Selected[Node]) and (Sender.FocusedColumn = Column) then
  begin
    TargetCanvas.Font.Color := clYellow;
    TargetCanvas.Font.Style := [fsBold];
  end;
end;

procedure TFrmLogManage.vstLogKeyAction(Sender: TBaseVirtualTree;
  var CharCode: Word; var Shift: TShiftState; var DoDefault: Boolean);
var
  S: string;
begin
  if (Shift = [ssCtrl]) and (CharCode = Ord('C')) then
  begin
    if (Sender.FocusedNode <> nil) and (Sender.FocusedColumn >= 0) then
    begin
      S := (Sender as TVirtualStringTree).Text[Sender.FocusedNode, Sender.FocusedColumn];
      SetClipboardText(S);
    end;
  end;
end;

procedure TFrmLogManage.PopupMenuPopup(Sender: TObject);
var
  S: string;
begin
  pmiCopy.Enabled := (vstLog.RootNodeCount > 0) and (vstLog.FocusedNode <> nil) and (vstLog.FocusedColumn >= 0);
  pmiCopyLine.Enabled := (vstLog.RootNodeCount > 0) and (vstLog.SelectedCount > 0);
  if (vstLog.FocusedNode <> nil) and (vstLog.FocusedColumn >= 0) then
  begin
    S := (vstLog as TVirtualStringTree).Text[vstLog.FocusedNode, vstLog.FocusedColumn];
    pmiCopy.Caption := '复制 "' + S + '"';
  end;
end;

procedure TFrmLogManage.vstLogTypeGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: WideString);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  CellText := NodeData.Text;
end;

procedure TFrmLogManage.vstLogTypeCollapsing(Sender: TBaseVirtualTree;
  Node: PVirtualNode; var Allowed: Boolean);
begin
  Allowed := False;
end;

procedure TFrmLogManage.vstLogTypeDrawText(Sender: TBaseVirtualTree;
  TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
  const Text: WideString; const CellRect: TRect; var DefaultDraw: Boolean);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  case NodeData.Category of
    acAll:
      begin
        TargetCanvas.Font.Style := [fsBold];
        TargetCanvas.Font.Color := clBlue;
      end;
    acParent:
      begin
        TargetCanvas.Font.Style := [fsBold];
      end;
  end;
end;

procedure TFrmLogManage.vstLogBeforeItemErase(Sender: TBaseVirtualTree;
  TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
  var ItemColor: TColor; var EraseAction: TItemEraseAction);
begin
  if Node.Index mod 2 <> 0 then
  begin
    ItemColor := $00FFFBF7;
    EraseAction := eaColor;
  end;
end;

procedure TFrmLogManage.vstLogHeaderClick(Sender: TVTHeader;
  HitInfo: TVTHeaderHitInfo);
begin
  if HitInfo.Button = mbLeft then
  begin
    with Sender do
    begin
      if HitInfo.Column = 0 then
      begin
        SortColumn := NoColumn;
        Treeview.SortTree(0, sdAscending, False);
      end
      else
      begin
        if SortColumn <> HitInfo.Column then
        begin
          SortColumn := HitInfo.Column;
          SortDirection := sdAscending;
        end
        else
        begin
          if SortDirection = sdAscending then
            SortDirection := sdDescending
          else
            SortDirection := sdAscending;
        end;

        Treeview.SortTree(SortColumn, SortDirection, False);
      end;
    end;
  end;
end;

procedure TFrmLogManage.vstLogCompareNodes(Sender: TBaseVirtualTree; Node1,
  Node2: PVirtualNode; Column: TColumnIndex; var Result: Integer);
var
  Data1: pTLogData;
  Data2: pTLogData;
begin
  Data1 := pTLogData(Sender.GetNodeData(Node1)^);
  Data2 := pTLogData(Sender.GetNodeData(Node2)^);

  case Column of
      0:  Result := Data1.nIndx - Data2.nIndx;
      1:  Result := Data1.nAct - Data2.nAct;
      2:  Result := CompareText(Data1.sMapName, Data2.sMapName);
      3:  Result := Data1.nX - Data2.nX;
      4:  Result := Data1.nY - Data2.nY;
      5:  Result := CompareText(Data1.sObjectName, Data2.sObjectName);
      6:  Result := Integer(Data1.ObjectType) - Integer(Data2.ObjectType);
      7:  Result := CompareText(Data1.sItemName, Data2.sItemName);
      8:  Result := Data1.nItemIndex - Data2.nItemIndex;
      9:  Result := CompareText(Data1.sActObjectName, Data2.sActObjectName);
      10: Result := Data1.nData1 - Data2.nData1;
      11: Result := Data1.nData2 - Data2.nData2;
      12: Result := CompareText(Data1.LogDesc, Data2.LogDesc);
      13: Result := Round(Data1.Date - Data2.Date);
  end;
end;

procedure TFrmLogManage.WMSYSCommand(var Msg: TWMSYSCommand);
begin
  if Msg.CmdType = SC_MINIMIZE then
  begin
    Visible := False;
    //inherited;
  end
  else
  begin
    inherited;
  end;
end;

end.

