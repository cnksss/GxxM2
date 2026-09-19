unit ViewOnlineHuman;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, Grids, ExtCtrls, StdCtrls;

type
  TfrmViewOnlineHuman = class(TForm)
    PanelStatus: TPanel;
    GridHuman: TStringGrid;
    Timer: TTimer;
    Panel1: TPanel;
    ButtonRefGrid: TButton;
    Label1: TLabel;
    ComboBoxSort: TComboBox;
    EditSearchName: TEdit;
    ButtonSearch: TButton;
    ButtonView: TButton;
    ButtonKickPlayOffLine: TButton;
    chkDummyHero: TCheckBox;
    chkDummy: TCheckBox;
    ButtonKickDummyObject: TButton;
    chkHuman: TCheckBox;
    lbl1: TLabel;
    chkHumanHero: TCheckBox;
    cbbRefreshTime: TComboBox;
    tmrRefresh: TTimer;
    procedure FormCreate(Sender: TObject);
    procedure ButtonRefGridClick(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure ComboBoxSortClick(Sender: TObject);
    procedure GridHumanDblClick(Sender: TObject);
    procedure TimerTimer(Sender: TObject);
    procedure ButtonSearchClick(Sender: TObject);
    procedure ButtonViewClick(Sender: TObject);
    procedure ButtonKickPlayOffLineClick(Sender: TObject);
    procedure ButtonKickDummyObjectClick(Sender: TObject);
    procedure chkDummyClick(Sender: TObject);
    procedure tmrRefreshTimer(Sender: TObject);
    procedure cbbRefreshTimeChange(Sender: TObject);
  private
    ViewList: TStringList;
    dwTimeOutTick: LongWord;
    procedure RefGridSession();
    procedure GetOnlineList();
    procedure SortOnlineList(nSort: Integer);
    procedure ShowHumanInfo();
    { Private declarations }

    procedure ResetRefreshTime;
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmViewOnlineHuman: TfrmViewOnlineHuman;

implementation

uses
  UsrEngn, M2Share, ObjBase, HUtil32, Grobal2, HumanInfo, ObjPlayer;

{$R *.dfm}

{ TfrmViewOnlineHuman }

procedure TfrmViewOnlineHuman.Open;
begin
  frmHumanInfo := TfrmHumanInfo.Create(Owner);
  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  RefGridSession();
  Timer.Enabled := True;
  ShowModal;
  Timer.Enabled := False;
  frmHumanInfo.Free;
end;

procedure TfrmViewOnlineHuman.GetOnlineList;
var
  I: Integer;
begin
  ViewList.Clear;
  if chkHumanHero.Checked or chkDummyHero.Checked then
  begin
    UserEngine.m_HeroObjectList.LockR(9);
    try
      for I := 0 to UserEngine.m_HeroObjectList.Count - 1 do
      begin
        if TBaseObject(UserEngine.m_HeroObjectList.Objects[I]).m_boDummyObject then
        begin
          if chkDummyHero.Checked then
            ViewList.AddObject(UserEngine.m_HeroObjectList.Strings[I], UserEngine.m_HeroObjectList.Objects[I]);
        end
        else
        begin
          if chkHumanHero.Checked then
            ViewList.AddObject(UserEngine.m_HeroObjectList.Strings[I], UserEngine.m_HeroObjectList.Objects[I]);
        end;
      end;
    finally
      UserEngine.m_HeroObjectList.UnLockR;
    end;
  end;

  if chkDummy.Checked or chkHuman.Checked then
  begin
    UserEngine.m_PlayObjectList.LockR(58);
    try
      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
        if TBaseObject(UserEngine.m_PlayObjectList.Objects[I]).m_boDummyObject then
        begin
          if chkDummy.Checked then
            ViewList.AddObject(UserEngine.m_PlayObjectList.Strings[I], UserEngine.m_PlayObjectList.Objects[I]);
        end
        else
        begin
          if chkHuman.Checked then
            ViewList.AddObject(UserEngine.m_PlayObjectList.Strings[I], UserEngine.m_PlayObjectList.Objects[I]);
        end;
      end;
    finally
      UserEngine.m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TfrmViewOnlineHuman.RefGridSession;
const
  BoolStr: array[Boolean] of string = ('否', '是');
var
  I: Integer;
  CurObj: TSmartObject;
begin
  PanelStatus.Caption := '正在取得数据...';
  GridHuman.Visible := False;
  GridHuman.Cells[0, 1] := '';
  GridHuman.Cells[1, 1] := '';
  GridHuman.Cells[2, 1] := '';
  GridHuman.Cells[3, 1] := '';
  GridHuman.Cells[4, 1] := '';
  GridHuman.Cells[5, 1] := '';
  GridHuman.Cells[6, 1] := '';
  GridHuman.Cells[7, 1] := '';
  GridHuman.Cells[8, 1] := '';
  GridHuman.Cells[9, 1] := '';
  GridHuman.Cells[10, 1] := '';
  GridHuman.Cells[11, 1] := '';
  GridHuman.Cells[12, 1] := '';
  GridHuman.Cells[13, 1] := '';
  GridHuman.Cells[14, 1] := '';
  GridHuman.Cells[15, 1] := '';
  GridHuman.Cells[16, 1] := '';
  GridHuman.Cells[17, 1] := '';
  GridHuman.Cells[18, 1] := '';

  if ViewList.Count <= 0 then
  begin
    GridHuman.RowCount := 2;
    GridHuman.FixedRows := 1;
  end
  else
  begin
    GridHuman.RowCount := ViewList.Count + 1;
  end;

  for I := 0 to ViewList.Count - 1 do
  begin
    CurObj := TSmartObject(ViewList.Objects[I]);
    GridHuman.Cells[0, I + 1] := IntToStr(I);
    GridHuman.Cells[1, I + 1] := CurObj.m_sCharName;
    GridHuman.Cells[3, I + 1] := IntToSex(CurObj.m_btGender);
    GridHuman.Cells[4, I + 1] := IntToJob(CurObj.m_btJob);
    GridHuman.Cells[5, I + 1] := IntToStr(CurObj.m_Abil.Level);
    GridHuman.Cells[6, I + 1] := CurObj.m_sMapName;
    GridHuman.Cells[7, I + 1] := IntToStr(CurObj.m_nCurrX) + ':' + IntToStr(CurObj.m_nCurrY);

    if CurObj.m_btRaceServer = RC_HEROOBJECT then
    begin
      if CurObj.m_boDummyObject then
        GridHuman.Cells[2, I + 1] := '假人英雄'
      else
        GridHuman.Cells[2, I + 1] := '玩家英雄';

      GridHuman.Cells[8, I + 1] := '';

      if CurObj.m_Master <> nil then
      begin
        GridHuman.Cells[9, I + 1] := TPlayObject(CurObj.m_Master).m_sIPaddr;
        GridHuman.Cells[10, I + 1] := IntToStr(TPlayObject(CurObj.m_Master).m_btPermission);
        GridHuman.Cells[11, I + 1] := TPlayObject(CurObj.m_Master).m_sIPLocal;
          // GetIPLocal(PlayObject.m_sIPaddr);
      end
      else
      begin
        GridHuman.Cells[9, I + 1] := '';
        GridHuman.Cells[10, I + 1] := '';
        GridHuman.Cells[11, I + 1] := '';
      end;

      GridHuman.Cells[12, I + 1] := '0';
      GridHuman.Cells[13, I + 1] := '0';
      GridHuman.Cells[14, I + 1] := '0';
      GridHuman.Cells[15, I + 1] := '0';
      GridHuman.Cells[16, I + 1] := '0';
      GridHuman.Cells[17, I + 1] := BoolStr[False];
      GridHuman.Cells[18, I + 1] := '';
    end
    else if CurObj.m_btRaceServer = RC_PLAYOBJECT then
    begin
      if CurObj.m_boDummyObject then
        GridHuman.Cells[2, I + 1] := '假人'
      else
        GridHuman.Cells[2, I + 1] := '玩家';

      GridHuman.Cells[8, I + 1] := TPlayObject(CurObj).m_sUserID;
      GridHuman.Cells[9, I + 1] := TPlayObject(CurObj).m_sIPaddr;
      GridHuman.Cells[10, I + 1] := IntToStr(CurObj.m_btPermission);
      GridHuman.Cells[11, I + 1] := TPlayObject(CurObj).m_sIPLocal;
        // GetIPLocal(PlayObject.m_sIPaddr);
      GridHuman.Cells[12, I + 1] := IntToStr(TPlayObject(CurObj).m_nGameGold);
      GridHuman.Cells[13, I + 1] := IntToStr(TPlayObject(CurObj).m_nGamePoint);
      GridHuman.Cells[14, I + 1] := IntToStr(TPlayObject(CurObj).m_nPayMentPoint);
      GridHuman.Cells[15, I + 1] := IntToStr(TPlayObject(CurObj).m_nGameDiamond);
      GridHuman.Cells[16, I + 1] := IntToStr(TPlayObject(CurObj).m_nGameGird);
      GridHuman.Cells[17, I + 1] := BoolStr[TPlayObject(CurObj).m_boOffLine];
      GridHuman.Cells[18, I + 1] := TPlayObject(CurObj).m_sAutoSendMsg;
    end;
  end;
  GridHuman.Visible := True;
end;

procedure TfrmViewOnlineHuman.FormCreate(Sender: TObject);
begin
  if (g_Config.btOnlineUserRefreshTime < cbbRefreshTime.Items.Count) then
  begin
    cbbRefreshTime.ItemIndex := g_Config.btOnlineUserRefreshTime;
  end;
  ResetRefreshTime;

  ViewList := TStringList.Create;
  GridHuman.Cells[0, 0] := '序号';
  GridHuman.Cells[1, 0] := '角色名称';
  GridHuman.Cells[2, 0] := '角色类型';
  GridHuman.Cells[3, 0] := '性别';
  GridHuman.Cells[4, 0] := '职业';
  GridHuman.Cells[5, 0] := '等级';
  GridHuman.Cells[6, 0] := '地图';
  GridHuman.Cells[7, 0] := '座标';
  GridHuman.Cells[8, 0] := '登录帐号';
  GridHuman.Cells[9, 0] := '登录IP';
  GridHuman.Cells[10, 0] := '权限';
  GridHuman.Cells[11, 0] := '所在地区';
  GridHuman.Cells[12, 0] := g_Config.sGameGoldName;
  GridHuman.Cells[13, 0] := g_Config.sGamePointName;
  GridHuman.Cells[14, 0] := g_Config.sPayMentPointName;
  GridHuman.Cells[15, 0] := g_Config.sGameDiamondName;
  GridHuman.Cells[16, 0] := g_Config.sGameGirdName;
  GridHuman.Cells[17, 0] := '离线挂机';
  GridHuman.Cells[18, 0] := '自动回复';
end;

procedure TfrmViewOnlineHuman.ButtonRefGridClick(Sender: TObject);
begin
  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  RefGridSession();
end;

procedure TfrmViewOnlineHuman.FormDestroy(Sender: TObject);
begin
  ViewList.Free;
end;

procedure TfrmViewOnlineHuman.ComboBoxSortClick(Sender: TObject);
begin
  if ComboBoxSort.ItemIndex < 0 then
    Exit;
  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  SortOnlineList(ComboBoxSort.ItemIndex);
  RefGridSession();
end;

procedure TfrmViewOnlineHuman.SortOnlineList(nSort: Integer);
var
  I: Integer;
  sIPaddr: string;
  sIPLocal: string;
  btPermission: Integer;
  btType: Integer;
  SortList: TStringList;
begin
  SortList := TStringList.Create;
  case nSort of
    0:
      begin
        ViewList.Sort;
        Exit;
      end;
    1:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          if TBaseObject(ViewList.Objects[I]).m_btRaceServer = RC_PLAYOBJECT then
          begin
            if not TBaseObject(ViewList.Objects[I]).m_boDummyObject then
              btType := 0
            else
              btType := 1;
          end
          else
          begin
            if not TBaseObject(ViewList.Objects[I]).m_boDummyObject then
              btType := 2
            else
              btType := 3;
          end;
          SortList.AddObject(IntToStr(btType), ViewList.Objects[I]);
        end;
      end;
    2:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          SortList.AddObject(IntToStr(TBaseObject(ViewList.Objects[I]).m_btGender), ViewList.Objects[I]);
        end;
      end;
    3:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          SortList.AddObject(IntToStr(TBaseObject(ViewList.Objects[I]).m_btJob), ViewList.Objects[I]);
        end;
      end;
    4:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          SortList.AddObject(IntToStr(TBaseObject(ViewList.Objects[I]).m_Abil.Level), ViewList.Objects[I]);
        end;
      end;
    5:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          SortList.AddObject(TBaseObject(ViewList.Objects[I]).m_sMapName, ViewList.Objects[I]);
        end;
      end;
    6:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          if TBaseObject(ViewList.Objects[I]).m_btRaceServer = RC_PLAYOBJECT then
            sIPaddr := TPlayObject(ViewList.Objects[I]).m_sIPaddr
          else if TBaseObject(ViewList.Objects[I]).m_Master <> nil then
            sIPaddr := TPlayObject(TBaseObject(ViewList.Objects[I]).m_Master).m_sIPaddr;

          SortList.AddObject(sIPaddr, ViewList.Objects[I]);
        end;
      end;
    7:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          if TBaseObject(ViewList.Objects[I]).m_btRaceServer = RC_PLAYOBJECT then
            btPermission := TPlayObject(ViewList.Objects[I]).m_btPermission
          else if TBaseObject(ViewList.Objects[I]).m_Master <> nil then
            btPermission := TPlayObject(TBaseObject(ViewList.Objects[I]).m_Master).m_btPermission
          else
            btPermission := 0;

          SortList.AddObject(IntToStr(btPermission), ViewList.Objects[I]);
        end;
      end;
    8:
      begin
        for I := 0 to ViewList.Count - 1 do
        begin
          if TBaseObject(ViewList.Objects[I]).m_btRaceServer = RC_PLAYOBJECT then
            sIPLocal := TPlayObject(ViewList.Objects[I]).m_sIPLocal
          else if TBaseObject(ViewList.Objects[I]).m_Master <> nil then
            sIPLocal := TPlayObject(TBaseObject(ViewList.Objects[I]).m_Master).m_sIPLocal;

          SortList.AddObject(sIPLocal, ViewList.Objects[I]);
        end;
      end;
  end;
  ViewList.Free;
  ViewList := SortList;
  ViewList.Sort;
end;

procedure TfrmViewOnlineHuman.GridHumanDblClick(Sender: TObject);
begin
  ShowHumanInfo();
end;

procedure TfrmViewOnlineHuman.TimerTimer(Sender: TObject);
begin
  if (MyGetTickCount - dwTimeOutTick > 100000) and (ViewList.Count > 0) then
  begin
    ViewList.Clear;
    RefGridSession();
  end;
end;

procedure TfrmViewOnlineHuman.ButtonSearchClick(Sender: TObject);
var
  I: Integer;
  sHumanName: string;
  BaseObject: TBaseObject;
begin
  sHumanName := Trim(EditSearchName.Text);
  if sHumanName = '' then
  begin
    Application.MessageBox('请输入一个角色名称！', '错误信息', MB_OK + MB_ICONEXCLAMATION);
    Exit;
  end;

  for I := 0 to ViewList.Count - 1 do
  begin
    BaseObject := TBaseObject(ViewList.Objects[I]);
    if CompareText(BaseObject.m_sCharName, sHumanName) = 0 then
    begin
      GridHuman.Row := I + 1;
      Exit;
    end;
  end;
  Application.MessageBox('角色没有在线！', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TfrmViewOnlineHuman.ButtonViewClick(Sender: TObject);
begin
  ShowHumanInfo();
end;

procedure TfrmViewOnlineHuman.ShowHumanInfo;
var
  nSelIndex: Integer;
  sPlayObjectName: string;
  BaseObject: TBaseObject;
begin
  nSelIndex := GridHuman.Row;
  Dec(nSelIndex);
  if (nSelIndex < 0) or (ViewList.Count <= nSelIndex) then
  begin
    Application.MessageBox('请先选择一个要查看的角色！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  sPlayObjectName := GridHuman.Cells[1, nSelIndex + 1];

  BaseObject := TBaseObject(ViewList.Objects[nSelIndex]);

  if BaseObject = nil then
  begin
    Application.MessageBox(PChar(sPlayObjectName + ' 此角色已经不在线！'), '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;

  frmHumanInfo.BaseObject := BaseObject;
  frmHumanInfo.Top := Self.Top + 20;
  frmHumanInfo.Left := Self.Left;
  frmHumanInfo.Open();
end;

procedure TfrmViewOnlineHuman.ButtonKickPlayOffLineClick(Sender: TObject);
var
  I, Index: Integer;
  Player: TPlayObject;
begin
  // 踢假人报错 chongchong 2013-12-29
  UserEngine.m_PlayObjectList.LockR(59);
  try
    for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
    begin
      Player := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
      if Player.m_boOffLine and (not g_SellPlayerList.Search(Player.m_sCharName, Index)) then
        Player.MakeGhost;
    end;
  finally
    UserEngine.m_PlayObjectList.UnLockR;
  end;

  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  RefGridSession();
end;

procedure TfrmViewOnlineHuman.ButtonKickDummyObjectClick(Sender: TObject);
var
  I: Integer;
begin
  // 踢假人报错 chongchong 2013-12-29
  UserEngine.m_PlayObjectList.LockR(60);
  try
    for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
    begin
      if TPlayObject(UserEngine.m_PlayObjectList.Objects[I]).m_boDummyObject then
      begin
        TPlayObject(UserEngine.m_PlayObjectList.Objects[I]).MakeGhost;
      end;
    end;
  finally
    UserEngine.m_PlayObjectList.UnLockR;
  end;

  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  RefGridSession();
end;

procedure TfrmViewOnlineHuman.chkDummyClick(Sender: TObject);
begin
  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  RefGridSession();
end;

procedure TfrmViewOnlineHuman.ResetRefreshTime;
begin
  tmrRefresh.Enabled := False;

  case cbbRefreshTime.ItemIndex of
    1:
      begin
        tmrRefresh.Interval := 30000;
        tmrRefresh.Enabled := True;
      end;
    2:
      begin
        tmrRefresh.Interval := 60000;
        tmrRefresh.Enabled := True;
      end;
    3:
      begin
        tmrRefresh.Interval := 90000;
        tmrRefresh.Enabled := True;
      end;
  end;
end;

procedure TfrmViewOnlineHuman.tmrRefreshTimer(Sender: TObject);
begin
  dwTimeOutTick := MyGetTickCount();
  GetOnlineList();
  RefGridSession();
end;

procedure TfrmViewOnlineHuman.cbbRefreshTimeChange(Sender: TObject);
begin
  g_Config.btOnlineUserRefreshTime := cbbRefreshTime.ItemIndex;
  Config.WriteInteger('Setup', 'OnlineUserRefreshTime', g_Config.btOnlineUserRefreshTime);
  ResetRefreshTime;
end;

end.

