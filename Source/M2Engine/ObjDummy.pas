unit ObjDummy;

interface

uses
  Windows, Classes, SysUtils, Controls, Forms, Math, ObjGame, ItemEvent, Grobal2, Envir, SDK, UnitPath, ObjBase, ObjPlayer,
  M2Definition;

type
  TDummyObject = class(TPlayObject) // 假人
    m_boStart: Boolean;
    m_boInitialized: Boolean;

    m_NotCanPickItemList: TList;
    m_dwStartPickItemTick: LongWord;
    //m_Path: TPath;
    //m_nPostion: Integer;
    //m_nMoveFailCount: Integer;

    m_dwAskTick: LongWord;
    m_nAutoGotoX: Integer;
    m_nAutoGotoY: Integer;
    //m_AutoGotoPath: TPath;
    //m_nAutoGotoPostion: Integer;

    m_boAutoUseMagic: Boolean;
    m_wAutoUseMagicID: Word;
    m_dwAutoUseMagicTime: LongWord;
    m_dwAutoUseMagicTick: LongWord;
  private
    function CanAutoUseMagic: Boolean;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Wondering(); override;
    procedure Initialize; override;
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override;
    procedure Run; override;
    procedure OnSpaceMove; override;
    function Ask: string; override;
    function StartPickUpItem(IsPickPlayDropItem, IsPickPlayScatterItem: Boolean; PickScatterToPickTime: LongWord; MoveToItem:
      Boolean = True): Boolean; override;
    function Walk(nIdent: Integer): Boolean; override;
    function GotoPath(): Boolean;
    function WalkToNext(nX, nY: Integer): Boolean; override;
    function RunToNext(nX, nY: Integer): Boolean; override;
    procedure Start;
    procedure Stop;
  end;

implementation

uses
  M2Share, ObjNpc, IdSrvClient, GameEvent, ObjHero, IniFiles;


// ------------------------------------------------------------------------------
// 假人

constructor TDummyObject.Create();
begin
  inherited;
  m_nSocket := 0;
  m_nGSocketIdx := -1;
  m_nGateIdx := -1;

  m_boInitialized := False;
  m_boDummyObject := True;
  m_nSoftVersionDate := CLIENT_VERSION_NUMBER;
  m_boLoginNoticeOK := True;
  m_boStart := False;

  m_dwAskTick := MyGetTickCount;

  //m_Path := nil;
  //m_nPostion := 0;
  //m_nMoveFailCount := 0;

  m_nAutoGotoX := -1;
  m_nAutoGotoY := -1;
  //m_AutoGotoPath := nil;
  //m_nAutoGotoPostion := -1;

  m_boAutoUseMagic := False;
  m_wAutoUseMagicID := 0;
  m_dwAutoUseMagicTime := 0;
  m_dwAutoUseMagicTick := MyGetTickCount;
end;

destructor TDummyObject.Destroy;
begin
  inherited;
end;

procedure TDummyObject.OnSpaceMove;
begin
  //m_Path := nil;
  //m_nPostion := -1;
  //m_nMoveFailCount := 0;

  m_nAutoGotoX := -1;
  m_nAutoGotoY := -1;
  //m_AutoGotoPath := nil;
  //m_nAutoGotoPostion := -1;
end;

function TDummyObject.StartPickUpItem(IsPickPlayDropItem, IsPickPlayScatterItem: Boolean; PickScatterToPickTime: LongWord;
  MoveToItem: Boolean): Boolean;
var
  BaseObject: TBaseObject;
  nWalkTime: Integer;
begin
  //if m_TargetCret <> nil then
  begin
    Result := False;

    if m_boDeath or m_boGhost {or InSafeZone} or (not IsEnoughBag) then
    begin
      m_SelItemObject := nil;
      m_PickUpItemFailList.Clear;
      Exit;
    end;

    // 检测地图上是否有这个物品
    if (m_SelItemObject <> nil) and (CheckItemExists(m_SelItemObject) <> m_SelItemObject) then
    begin
      m_SelItemObject := nil;
    end;

    if (m_SelItemObject <> nil) and (m_SelItemObject.m_boGhost) then
    begin
      m_SelItemObject := nil;
    end;

    // 检测这个物品是否在自己的坐标上可以捡取
    if (m_SelItemObject <> nil) then
    begin
      if (m_nCurrX <> m_SelItemObject.m_nMapX) or (m_nCurrY <> m_SelItemObject.m_nMapY) then
      begin
        case m_btJob of
          0:
            nWalkTime := g_Config.dwDummyWarrorWalkTime;
          1:
            nWalkTime := g_Config.dwDummyWizardWalkTime;
          2:
            nWalkTime := g_Config.dwDummyTaoistWalkTime;
        else
          nWalkTime := 500;
        end;

        BaseObject := m_PEnvir.GetMovingObjectEx(Self, m_SelItemObject.m_nMapX, m_SelItemObject.m_nMapY, True);
        if BaseObject <> nil then
        begin
          m_SelItemObject := nil;
          self.SetTargetCreat(BaseObject);
          Self.m_boTarget := True;
          m_PickUpItemFailList.Clear;
        end
        else
        begin
          if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
          begin
            if not GotoNextOne(m_SelItemObject.m_nMapX, m_SelItemObject.m_nMapY, True) then
            begin
              m_SelItemObject := nil;
            end
            else
            begin
              Result := True;
              Exit;
            end;
          end;
        end;
      end
      else
      begin // 和自己坐标相同可以捡取
        if DoPickUpItem(m_nCurrX, m_nCurrY, m_SelItemObject, False, False) then
        begin
          m_SelItemObject := nil;
          Result := True;
          Exit;
        end
        else
        begin
          m_PickUpItemFailList.Add(m_SelItemObject);
          m_SelItemObject := nil;
        end;
      end;
    end;

    m_SelItemObject := FindPriorityPickUpItem(IsPickPlayDropItem, IsPickPlayScatterItem, PickScatterToPickTime);
    Result := (m_SelItemObject <> nil);

    if not Result then
    begin
      m_SelItemObject := FindPickUpItem(IsPickPlayDropItem, IsPickPlayScatterItem, PickScatterToPickTime);
      Result := (m_SelItemObject <> nil);
    end;
  end;
end;

procedure TDummyObject.Start;
begin
  if not m_boStart then
  begin
    //m_Path := nil;
    //m_nPostion := -1;
    //m_nMoveFailCount := 0;
    m_boStart := True;
    if g_FunctionNPC <> nil then
    begin
      m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(Self, '@DummyStart', False);
    end;
  end;
end;

procedure TDummyObject.Stop;
begin
  if m_boStart then
  begin
    m_boStart := False;
    //m_Path := nil;
    //m_nPostion := -1;
    //m_nMoveFailCount := 0;

    m_nAutoGotoX := -1;
    m_nAutoGotoY := -1;
    //m_AutoGotoPath := nil;
    //m_nAutoGotoPostion := -1;
    if g_FunctionNPC <> nil then
    begin
      m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(Self, '@DummyStop', False);
    end;
  end;
end;

function TDummyObject.Ask: string;
begin
  if m_SayList.Count > 0 then
  begin
    m_SayList.CustomSort(ObjLongWordSort_2);
    m_SayList.Objects[0] := TObject(MyGetTickCount);
    Result := m_SayList.Strings[0];
  end
  else
    Result := '';
end;

procedure TDummyObject.Initialize;
var
  I: Integer;
  UserMagic: pTUserMagic;
begin
  if not m_boInitialized then
  begin
    m_boInitialized := True;

    AbilCopyToWAbil();

    for I := 0 to m_MagicList.Count - 1 do
    begin
      UserMagic := m_MagicList.Items[I];
      if UserMagic.btLevel > High(UserMagic.MagicInfo.TrainLevel) then
        UserMagic.btLevel := 0;
    end;

    m_boAddtoMapFail := True;

    if m_PEnvir.CanWalk(m_nCurrX, m_nCurrY, True) and AddToMap() then
      m_boAddtoMapFail := False;

    m_nCharStatus := GetCharStatus();
    AddBodyLuck(0);
  end;
end;

function TDummyObject.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  Result := False;
  if inherited IsProperTarget(BaseObject) then
  begin
    Result := True;
    if BaseObject = Self then
      Result := False;
    if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and BaseObject.InSafeZone then
      Result := False;

    if (BaseObject.m_Master = Self) or (BaseObject.Master = Self) then
      Result := False;

    if (BaseObject.m_btRaceServer in [RC_NPC..RC_ANIMAL]) then
      Result := False;
    if (BaseObject.m_btRaceServer = RC_ARCHERGUARD) and (BaseObject.m_LastHiter <> Self) then
      Result := False; // 不主动攻击弓箭手
    if (m_btAttatckMode <> HAM_GUILD) and (BaseObject.m_btRaceServer in [110, 111]) then
    begin // 不主动攻击沙巴克城门 沙巴克左城墙
      Result := False;
    end;
  end;
end;

function NextDirClockwise(btDir: Byte): Byte; // 顺时针
begin
  Result := DR_UP;
  case btDir of
    DR_UP:
      Result := DR_UPRIGHT;
    DR_UPRIGHT:
      Result := DR_RIGHT;
    DR_RIGHT:
      Result := DR_DOWNRIGHT;
    DR_DOWNRIGHT:
      Result := DR_DOWN;
    DR_DOWN:
      Result := DR_DOWNLEFT;
    DR_DOWNLEFT:
      Result := DR_LEFT;
    DR_LEFT:
      Result := DR_UPLEFT;
    DR_UPLEFT:
      Result := DR_UP;
  end;
end;

function NextDirAntiClockwise(btDir: Byte): Byte; // 逆时针
begin
  Result := DR_UP;
  case btDir of
    DR_UP:
      Result := DR_UPLEFT;
    DR_UPRIGHT:
      Result := DR_UP;
    DR_RIGHT:
      Result := DR_UPRIGHT;
    DR_DOWNRIGHT:
      Result := DR_RIGHT;
    DR_DOWN:
      Result := DR_DOWNRIGHT;
    DR_DOWNLEFT:
      Result := DR_DOWN;
    DR_LEFT:
      Result := DR_DOWNLEFT;
    DR_UPLEFT:
      Result := DR_LEFT;
  end;
end;

function TDummyObject.GotoPath(): Boolean;
// label
// REFGOTONEXT;
begin
  Result := False;
  (*
 // REFGOTONEXT:
  while (m_nPostion >= 0) and (m_nPostion < Length(m_Path)) do
  begin
    if (m_Path[m_nPostion].X <> m_nCurrX) or (m_Path[m_nPostion].Y <> m_nCurrY) then
    begin
      if (abs(m_Path[m_nPostion].X - m_nCurrX) <= 1) and (abs(m_Path[m_nPostion].Y - m_nCurrY) <= 1) then
      begin
        if WalkToNext(m_Path[m_nPostion].X, m_Path[m_nPostion].Y) then
        begin
          Inc(m_nPostion);
          Result := True;
          if (m_nPostion < Length(m_Path)) then
            Exit
          else
            break;
        end
        else
          break;
      end
      else
      begin
        if (abs(m_Path[m_nPostion].X - m_nCurrX) > 2) or (abs(m_Path[m_nPostion].Y - m_nCurrY) > 2) then
        begin
          Break;
        end;

        if RunToNext(m_Path[m_nPostion].X, m_Path[m_nPostion].Y) then
        begin
          Inc(m_nPostion);
          Result := True;
          if (m_nPostion < Length(m_Path)) then
            Exit
          else
            break;
        end
        else
          break;
      end;
    end
    else
    begin
      Inc(m_nPostion);
    end;
  end;
  m_nPostion := -1;
  SetLength(m_Path, 0);
  m_Path := nil;
  *)
end;

function TDummyObject.WalkToNext(nX, nY: Integer): Boolean;
begin
  Result := inherited WalkToNext(nX, nY);
  m_dwMoveTimeTick := MyGetTickCount;
end;

function TDummyObject.RunToNext(nX, nY: Integer): Boolean;
begin
  if m_boDuanJin or m_boCobwebWindingStatus then
    Result := WalkToNext(nX, nY)
  else
    Result := inherited RunToNext(nX, nY);
  m_dwMoveTimeTick := MyGetTickCount;
end;

procedure TDummyObject.Wondering();

  function GetPoint(nIndex: Integer; var nX, nY: Integer): Boolean;
  var
    I, II, nMX, nMY, nC, nRange: Integer;
    btDir: Byte;
    GetNextDir: function(btDir: Byte): Byte;
  const
    CheckSteps: array[0..2] of Byte = (2, 4, 6);
  begin
    Result := False;
    // for I := Low(CheckSteps) to High(CheckSteps) do begin
    if (nIndex < Low(CheckSteps)) or (nIndex > High(CheckSteps)) then
      nIndex := Low(CheckSteps);

    nRange := CheckSteps[nIndex];
    for II := nRange downto nRange - 1 do
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, II, nMX, nMY) then
      begin
        if CanMove(nMX, nMY, False) then
        begin
          nX := nMX;
          nY := nMY;
          Result := True;
          Exit;
        end;
      end;
    end;

    if Random(2) = 0 then
      GetNextDir := NextDirClockwise
    else
      GetNextDir := NextDirAntiClockwise;

    nC := 0;
    btDir := m_btDirection;
    while True do
    begin
      btDir := GetNextDir(btDir);
      for I := nRange downto nRange - 1 do
      begin
        if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, btDir, I, nMX, nMY) then
        begin
          if CanMove(nMX, nMY, False) then
          begin
            nX := nMX;
            nY := nMY;
            Result := True;
            Exit;
          end;
        end;
      end;
      Inc(nC);
      if (nC >= 8) then
        Break;
    end;
    // end;
  end;

var
  nX, nY, nIndex, nWalkTime: Integer;
begin
  if m_boStart and (m_TargetCret = nil) and (not m_boGhost) and (not m_boDeath) and (not m_boFixedHideMode) and (not m_boStoneMode)
    and (not m_boShopStall) and (CanMove) then
  begin

    if m_boAutoPickUpItem and StartPickUpItem(True, True, 0) then
    begin
      //m_Path := nil;
      //m_nPostion := 0;
      //m_nMoveFailCount := 0;

      m_nMoveIndex := -1;
      SetLength(m_MovePath, 0);
      Exit;
    end;

    case m_btJob of
      0:
        nWalkTime := g_Config.dwDummyWarrorWalkTime;
      1:
        nWalkTime := g_Config.dwDummyWizardWalkTime;
      2:
        nWalkTime := g_Config.dwDummyTaoistWalkTime;
    else
      nWalkTime := 500;
    end;

    if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
    begin
      {
      if (Length(m_Path) > 0) and GotoPath() then
      begin
        m_dwMoveTimeTick := MyGetTickCount;
        Exit;
      end;
      }

      (*
      for nIndex := 0 to 2 do
      begin
        nX := m_nCurrX;
        nY := m_nCurrY;
        if GetPoint(nIndex, nX, nY) then
        begin
          if (abs(nX - m_nCurrX) > 2) or (abs(nY - m_nCurrY) > 2) then
          begin
            g_FindPath.BaseObject := Self;
            m_Path := g_FindPath.FindPath1(m_PEnvir, m_nCurrX, m_nCurrY, nX, nY, True, False);
            m_nPostion := 0;
            if (Length(m_Path) > 0) and GotoPath() then
            begin
            // inherited;
              m_dwMoveTimeTick := MyGetTickCount;
              Exit;
            end;
          end;
        end;
        end;
      *)

      for nIndex := 0 to 2 do
      begin
        nX := m_nCurrX;
        nY := m_nCurrY;
        if GetPoint(nIndex, nX, nY) then
        begin
          if (abs(nX - m_nCurrX) > 2) or (abs(nY - m_nCurrY) > 2) then
          begin
            SetTargetXY(nX, nY);
            RunToTargetXY;
            m_dwMoveTimeTick := MyGetTickCount;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

procedure TDummyObject.Run;
var
  nSelectMagic, nWalkTime: Integer;
  AttackTime: Integer;
  nRange, btDir, NewX, NewY, nCount: Integer;
  IsInSafe: Boolean;
  GameEvent: TGameEvent;
  nTempX, nTempY: Integer;
  nDir1, nDir2: Byte;
  boReturn: BOOL;
begin
  if m_OPEnvir <> m_PEnvir then
  begin
    m_OPEnvir := m_PEnvir;
    m_nMoveIndex := -1;
    SetLength(m_MovePath, 0);
    m_nMoveSameCount := 0;
    m_nOLastDir := -1;
    m_nLastDir := -1;

    //m_Path := nil;
    //m_nPostion := 0;
    //m_nMoveFailCount := 0;

    m_nAutoGotoX := -1;
    m_nAutoGotoY := -1;
    //m_AutoGotoPath := nil;
    //m_nAutoGotoPostion := -1;
  end;

  if (m_nAutoGotoX >= 0) then
  begin
    if (m_nAutoGotoX = m_nCurrX) and (m_nAutoGotoY = m_nCurrY) then
    begin
      m_nAutoGotoX := -1;
      m_nAutoGotoY := -1;
      //m_AutoGotoPath := nil;
      //m_nAutoGotoPostion := -1;
    end;
  end;

  case m_btJob of
    0:
      nWalkTime := g_Config.dwDummyWarrorWalkTime;
    1:
      nWalkTime := g_Config.dwDummyWizardWalkTime;
    2:
      nWalkTime := g_Config.dwDummyTaoistWalkTime;
  else
    nWalkTime := 500;
  end;

  if (m_nAutoGotoX >= 0) then
  begin
    if (not m_boGhost) and (not m_boDeath) and (not m_boFixedHideMode) and (not m_boStoneMode) and (not m_boShopStall) and (CanMove)
      then
    begin
      if (m_nAutoGotoX >= 0) then
      begin
        if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
        begin
          if (Random(10) = 0) or ((Abs(m_nAutoGotoX - m_nCurrX) <= 1) and ((Abs(m_nAutoGotoY - m_nCurrY) <= 1))) then
          begin
            if GotoNearGotoXY(m_nAutoGotoX, m_nAutoGotoY, NewX, NewY) then
            begin
              WalkTo(GetNextDirection(m_nCurrX, m_nCurrY, NewX, NewY), False);
              m_dwMoveTimeTick := MyGetTickCount;
            end;
          end
          else
          begin
            if GotoNearRuntoXY(m_nAutoGotoX, m_nAutoGotoY, NewX, NewY) then
            begin
              nDir1 := GetNextDirection(m_nCurrX, m_nCurrY, NewX, NewY);

              if ((m_nAutoGotoX <> NewX) or (m_nAutoGotoY <> NewY)) and GotoNearRuntoXY(NewX, NewY, m_nAutoGotoX, m_nAutoGotoY,
                nTempX, nTempY) then
              begin
                nDir2 := GetNextDirection(NewX, NewY, nTempX, nTempY);

                // 当前步骤和下一步的方向相反，就是来回搞chongchong 2017-10-26
                if GetDifferenceDirection(nDir1) = nDir2 then
                begin
                  //OutputDebugString('aaaa');
                  m_dwMoveTimeTick := MyGetTickCount;
                  Exit;
                end;
              end;

              if (Abs(m_nCurrX - NewX) > 1) or (Abs(m_nCurrY - NewY) > 1) then
              begin
                RunTo(nDir1, False);
                m_dwMoveTimeTick := MyGetTickCount;
              end
              else
              begin
                WalkTo(nDir1, False);
                m_dwMoveTimeTick := MyGetTickCount;
              end;
            end;
          end;

          {
          GotoNearRuntoXY(m_nAutoGotoX, m_nAutoGotoY, NewX, NewY)
          SetTargetXY(NewX, m_nAutoGotoY);

          if Random(10) = 0 then
            GotoTargetXY
          else
            RunToTargetXY;
          m_dwMoveTimeTick := MyGetTickCount;
          }
        end;
      end;
    end;
  end;

  if m_boStart and (not m_boGhost) and (not m_boDeath) and (not m_boFixedHideMode) and (not m_boStoneMode) and (not m_boShopStall)
    and (CanMove) then
  begin
    if (g_PluginManager <> nil) then
    begin
      boReturn := False;
      g_PluginManager.HookDummyObjectRunBegin(Self, boReturn);

      if boReturn then
      begin
        inherited;
        Exit;
      end;
    end;

    if (m_TargetCret <> nil) and (m_TargetCret.m_boDeath or m_TargetCret.m_boGhost) then
      DelTargetCreat;

    if not IsProperTarget(m_TargetCret) then
      DelTargetCreat();

    if (m_TargetCret <> nil) and (not m_boStart) then
    begin
      DelTargetCreat()
    end;

    IsInSafe := InSafeZone;
    if (m_TargetCret <> nil) and IsInSafe and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) then
    begin
      DelTargetCreat()
    end;

    if (m_TargetCret <> nil) and IsInSafe and (m_TargetCret.Master <> nil) and (m_TargetCret.Master.m_btRaceServer = RC_PLAYOBJECT)
      then
    begin
      DelTargetCreat()
    end;

    if (m_TargetCret = nil) then
      SearchTarget()
    else if ((MyGetTickCount - m_dwSearchTargetTick > 1000) and ((m_TargetCret = nil) or (MyGetTickCount - m_dwStruckTick > 5000)))
      then
    begin
      // 在和人物PK状态，不要轻易更换目标
      if (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) and (m_TargetCret.m_LastHiter = Self) and (m_TargetCret.m_PEnvir = m_PEnvir)
        and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= m_nViewRange) and (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= m_nViewRange)
        then
      begin

      end
      else
        SearchTarget();

      if (m_TargetCret <> nil) and (m_TargetCret.Master <> nil) and (m_TargetCret.Master.m_PEnvir = m_PEnvir) and (abs(m_TargetCret.Master.m_nCurrX
        - m_nCurrX) <= m_nViewRange) and (abs(m_TargetCret.Master.m_nCurrY - m_nCurrY) <= m_nViewRange) then
      begin
        SetTargetCreat(m_TargetCret.Master);
      end;
    end;

    if (m_TargetCret <> nil) then
      nSelectMagic := SelectMagic
    else
      nSelectMagic := -1;
    if (m_TargetCret <> nil) then
    begin
      if (Length(m_MovePath) > 0) then
      begin
        if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
        begin
          // 道士假人有事没事的跑，蛋疼 chongchong 2015-12-15
          Randomize;
          if not ((m_btJob = 2) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= g_Config.nMagicAttackRage) and (Abs(m_nCurrY -
            m_TargetCret.m_nCurrY) <= g_Config.nMagicAttackRage) and (Random(4) <> 0)) then
          begin
            if GotoNext() then
            begin
              inherited;
              m_dwMoveTimeTick := MyGetTickCount;
              Exit;
            end;
          end;
        end
        else
        begin
          inherited;
          Exit;
        end;
      end;

      if m_boAutoPickUpItem and StartPickUpItem(True, True, 0) then
      begin
        //m_Path := nil;
        //m_nPostion := 0;
        //m_nMoveFailCount := 0;

        m_nMoveIndex := -1;
        SetLength(m_MovePath, 0);

        m_nAutoGotoX := -1;
        m_nAutoGotoY := -1;

        // 修正假人在捡物时攻击不动（无敌一样 打不动假人） chongchong 2018-05-11
        inherited;
        Exit;
      end;

      if (m_nRunAttackRate > 0) and (Random(m_nRunAttackRate) = 0) and (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
      begin
        if m_btJob = 0 then
          nRange := 1
        else
          nRange := 5;

        if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= nRange) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= nRange) then
        begin
          btDir := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);

          if Random(2) = 0 then
            btDir := (btDir + 1) mod 8
          else
            btDir := (btDir - 1) mod 8;

          m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, nRange, NewX, NewY);

          if ((NewX <> m_nCurrX) or (NewY <> m_nCurrY)) and m_PEnvir.CanWalk(NewX, NewY, False) then
          begin
            SetTargetXY(NewX, NewY);

            if nRange = 1 then
              GotoTargetXY
            else
              RuntoTargetXY;

            m_dwMoveTimeTick := MyGetTickCount;

            Exit;
          end;
        end;
      end;

      case m_btJob of
        0:
          begin
            AttackTime := g_Config.dwDummyWarrorAttackTime;

            // 假人计算武器速度 chongchong 2013-11-17
            AttackTime := Max(0, AttackTime - (g_Config.dwIncSpeedDecInterval * m_nHitSpeed)); // 防止负数出错

            if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > AttackTime) then
            begin
              m_dwHitTick := MyGetTickCount();

              if (m_MyHero <> nil) and g_Config.boHeroJointAttack and (THeroObject(m_MyHero).m_btAngryValue >= g_Config.btMaxAngryValue)
                and (THeroObject(m_MyHero).WearFirDragon) and (THeroObject(m_MyHero).m_UseItems[U_BUJUK].Dura > 0) and (THeroObject
                (m_MyHero).FindGroupMagic <> nil) and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 10) and (abs(m_TargetCret.m_nCurrY
                - m_nCurrY) <= 10) and (abs(m_MyHero.m_nCurrX - m_TargetCret.m_nCurrX) <= 10) and (abs(m_MyHero.m_nCurrY -
                m_TargetCret.m_nCurrY) <= 10) and (Random(8) = 0) then
              begin
                if g_Config.boHeroJointAttackFly and ((abs(m_MyHero.m_nCurrX - m_MyHero.m_TargetCret.m_nCurrX) > 2) or (abs(m_MyHero.m_nCurrY
                  - m_MyHero.m_TargetCret.m_nCurrY) > 2)) then
                  m_MyHero.SpaceMove(m_PEnvir.sMapName, m_MyHero.m_TargetCret.m_nCurrX, m_MyHero.m_TargetCret.m_nCurrY, 1);

                m_MyHero.SetTargetCreat(m_TargetCret);
                THeroObject(m_MyHero).m_nTargetX := m_TargetCret.m_nCurrX;
                THeroObject(m_MyHero).m_nTargetY := m_TargetCret.m_nCurrY;
                THeroObject(m_MyHero).m_boUseGroupSpell := True;
                THeroObject(m_MyHero).m_boTarget := True;
                THeroObject(m_MyHero).m_boTargetAgain := False;
                Exit;
              end;

              if (nSelectMagic <> -1) and ActThink(nSelectMagic) then
              begin

                inherited;
                Exit;
              end;

              if (m_TargetCret <> nil) and StartAttack(nSelectMagic) then
              begin
                if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                begin
                  m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                end;
              end;
            end;
          end;
        1:
          begin
            AttackTime := g_Config.dwDummyWizardAttackTime;

            // 假人计算武器速度 chongchong 2013-11-17
            AttackTime := Max(0, AttackTime - (g_Config.dwIncSpeedDecInterval * m_nHitSpeed)); // 防止负数出错

            if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > AttackTime) then
            begin
              m_dwHitTick := MyGetTickCount();
              if ActThink(nSelectMagic) then
              begin

                inherited;
                Exit;
              end;

              if (Length(m_MovePath) = 0) and (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
              begin
                if ((Abs(m_nCurrX - m_TargetCret.m_nCurrX) > g_Config.nMagicAttackRage) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY)
                  > g_Config.nMagicAttackRage)) then
                begin
                  btDir := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);

                  m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, g_Config.nMagicAttackRage - 1,
                    NewX, NewY);

                  if ((NewX <> m_nCurrX) or (NewY <> m_nCurrY)) and m_PEnvir.CanWalk(NewX, NewY, False) then
                  begin
                    SetTargetXY(NewX, NewY);
                    RuntoTargetXY;

                    m_dwMoveTimeTick := MyGetTickCount;

                    Exit;
                  end;
                end
                // 法师看下周边有没有自己打的火墙，有的话，有一定的几率往上站 chongchong 2015-12-15
                else if (m_TargetCret.m_btRaceServer <> RC_PLAYOBJECT) and (Random(20) = 0) then
                begin
                  for nCount := 0 to 7 do
                  begin
                    btDir := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);
                    btDir := (btDir + nCount) mod 8;
                    for nRange := 3 to 6 do
                    begin
                      m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, nRange, NewX, NewY);
                      GameEvent := TGameEvent(m_PEnvir.GetEvent(NewX, NewY));
                      if (GameEvent <> nil) and (GameEvent.m_nEventType = ET_FIRE) and (GameEvent.m_OwnBaseObject = Self) then
                      begin
                        if ((NewX <> m_nCurrX) or (NewY <> m_nCurrY)) and m_PEnvir.CanWalk(NewX, NewY, False) then
                        begin
                          SetTargetXY(NewX, NewY);

                          if (Abs(NewX - m_nCurrX) > 2) or (Abs(NewY - m_nCurrY) > 2) then
                          begin
                            RuntoTargetXY;
                          end
                          else
                          begin
                            GotoTargetXY;
                          end;

                          m_dwMoveTimeTick := MyGetTickCount;
                          Exit;
                        end;
                      end;
                    end;
                  end;
                end;
              end;

              if (m_MyHero <> nil) and g_Config.boHeroJointAttack and (THeroObject(m_MyHero).m_btAngryValue >= g_Config.btMaxAngryValue)
                and (THeroObject(m_MyHero).WearFirDragon) and (THeroObject(m_MyHero).m_UseItems[U_BUJUK].Dura > 0) and (THeroObject
                (m_MyHero).FindGroupMagic <> nil) and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 10) and (abs(m_TargetCret.m_nCurrY
                - m_nCurrY) <= 10) and (abs(m_MyHero.m_nCurrX - m_TargetCret.m_nCurrX) <= 10) and (abs(m_MyHero.m_nCurrY -
                m_TargetCret.m_nCurrY) <= 10) and (Random(8) = 0) then
              begin
                if g_Config.boHeroJointAttackFly and ((abs(m_MyHero.m_nCurrX - m_MyHero.m_TargetCret.m_nCurrX) > 2) or (abs(m_MyHero.m_nCurrY
                  - m_MyHero.m_TargetCret.m_nCurrY) > 2)) then
                  m_MyHero.SpaceMove(m_PEnvir.sMapName, m_MyHero.m_TargetCret.m_nCurrX, m_MyHero.m_TargetCret.m_nCurrY, 1);

                m_MyHero.SetTargetCreat(m_TargetCret);
                THeroObject(m_MyHero).m_nTargetX := m_TargetCret.m_nCurrX;
                THeroObject(m_MyHero).m_nTargetY := m_TargetCret.m_nCurrY;
                THeroObject(m_MyHero).m_boUseGroupSpell := True;
                THeroObject(m_MyHero).m_boTarget := True;
                THeroObject(m_MyHero).m_boTargetAgain := False;
                Exit;
              end;

              if (m_TargetCret <> nil) and StartAttack(nSelectMagic) then
              begin
                if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                begin
                  m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                end;
              end;
            end;
          end;
        2:
          begin
            AttackTime := g_Config.dwDummyTaoistAttackTime;

            // 假人计算武器速度 chongchong 2013-11-17
            AttackTime := Max(0, AttackTime - (g_Config.dwIncSpeedDecInterval * m_nHitSpeed)); // 防止负数出错

            if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > AttackTime) then
            begin
              m_dwHitTick := MyGetTickCount();
              if ActThink(nSelectMagic) then
              begin

                inherited;
                Exit;
              end;

              if (Length(m_MovePath) = 0) and (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) and ((Abs(m_nCurrX - m_TargetCret.m_nCurrX)
                > g_Config.nMagicAttackRage) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > g_Config.nMagicAttackRage)) then
              begin
                btDir := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);

                m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, g_Config.nMagicAttackRage - 1, NewX,
                  NewY);

                if ((NewX <> m_nCurrX) or (NewY <> m_nCurrY)) and m_PEnvir.CanWalk(NewX, NewY, False) then
                begin
                  SetTargetXY(NewX, NewY);
                  RuntoTargetXY;

                  m_dwMoveTimeTick := MyGetTickCount;

                  Exit;
                end;
              end;

              if (m_MyHero <> nil) and g_Config.boHeroJointAttack and (THeroObject(m_MyHero).m_btAngryValue >= g_Config.btMaxAngryValue)
                and (THeroObject(m_MyHero).WearFirDragon) and (THeroObject(m_MyHero).m_UseItems[U_BUJUK].Dura > 0) and (THeroObject
                (m_MyHero).FindGroupMagic <> nil) and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 10) and (abs(m_TargetCret.m_nCurrY
                - m_nCurrY) <= 10) and (abs(m_MyHero.m_nCurrX - m_TargetCret.m_nCurrX) <= 10) and (abs(m_MyHero.m_nCurrY -
                m_TargetCret.m_nCurrY) <= 10) and (Random(8) = 0) then
              begin
                if g_Config.boHeroJointAttackFly and ((abs(m_MyHero.m_nCurrX - m_MyHero.m_TargetCret.m_nCurrX) > 2) or (abs(m_MyHero.m_nCurrY
                  - m_MyHero.m_TargetCret.m_nCurrY) > 2)) then
                  m_MyHero.SpaceMove(m_PEnvir.sMapName, m_MyHero.m_TargetCret.m_nCurrX, m_MyHero.m_TargetCret.m_nCurrY, 1);

                m_MyHero.SetTargetCreat(m_TargetCret);
                THeroObject(m_MyHero).m_nTargetX := m_TargetCret.m_nCurrX;
                THeroObject(m_MyHero).m_nTargetY := m_TargetCret.m_nCurrY;
                THeroObject(m_MyHero).m_boUseGroupSpell := True;
                THeroObject(m_MyHero).m_boTarget := True;
                THeroObject(m_MyHero).m_boTargetAgain := False;
                Exit;
              end;

              if (m_TargetCret <> nil) and StartAttack(nSelectMagic) then
              begin
                if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                begin
                  m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                end;
              end;
            end;
          end;
      end;
    end

    // 假人自动练功
    else
    begin
      if m_boAutoUseMagic and (m_wAutoUseMagicID > 0) and (MyGetTickCount - m_dwAutoUseMagicTick >= m_dwAutoUseMagicTime * 1000)
        then
      begin
        if CanAutoUseMagic then
        begin
          m_TargetCret := Self;
          StartAttack(m_wAutoUseMagicID);
          m_TargetCret := nil;
          if (m_wAutoUseMagicID <= High(m_SkillUseTick)) then
          begin
            m_SkillUseTick[m_wAutoUseMagicID] := MyGetTickCount;
          end;
          m_dwAutoUseMagicTick := MyGetTickCount;
        end;
      end;
    end;
  end
  else if (not m_boGhost) and (not m_boDeath) and (not m_boFixedHideMode) and (not m_boStoneMode) and (not m_boShopStall) then
  begin
    if m_boAutoUseMagic and (m_wAutoUseMagicID > 0) and (MyGetTickCount - m_dwAutoUseMagicTick >= m_dwAutoUseMagicTime * 1000)
      then
    begin
      if CanAutoUseMagic then
      begin
        m_TargetCret := Self;
        StartAttack(m_wAutoUseMagicID);
        m_TargetCret := nil;
        if (m_wAutoUseMagicID <= High(m_SkillUseTick)) then
        begin
          m_SkillUseTick[m_wAutoUseMagicID] := MyGetTickCount;
        end;
        m_dwAutoUseMagicTick := MyGetTickCount;
      end;
    end;
  end;

  if (g_PluginManager <> nil) then
  begin
    g_PluginManager.HookDummyObjectRunEnd(Self);
  end;

  Wondering();
  inherited;
end;
// -----------------------------------------------------------------------------

function TDummyObject.Walk(nIdent: Integer): Boolean;
begin
  if nIdent = RM_RUN then
  begin
    if g_FunctionNPC <> nil then
    begin
      m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(Self, '@Run', False);
    end;
  end
  else
  begin
    if nIdent = RM_WALK then
      if g_FunctionNPC <> nil then
      begin
        m_nScriptGotoCount := 0;
        g_FunctionNPC.GotoLable(Self, '@Walk', False);
      end;
  end;
  Result := inherited Walk(nIdent);
  m_dwMoveTimeTick := MyGetTickCount;
end;

function TDummyObject.CanAutoUseMagic: Boolean; // 检测是否可以使用自动练功
begin
  Result := False;

  if not AllowUseMagic(m_wAutoUseMagicID) then
    Exit;

  if m_wAutoUseMagicID = 75 then
  begin // 自动开启护体神盾
    if (m_MagicSuperShiledSkill <> nil) then
    begin
      if MyGetTickCount - m_dwLastSuperShiledTimeTick >= GetMagicCD(SKILL_75) then
      begin
        m_dwLastSuperShiledTimeTick := MyGetTickCount();
        OpenSuperShiled;
        Result := False;
      end;
    end;
  end
  else if m_wAutoUseMagicID = SKILL_56 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[SKILL_56]) >= GetMagicCD(SKILL_56)) then
    begin
      AllowSWordHitSkill;
      Result := True;
    end;
  end

  // 烈火
  else if m_wAutoUseMagicID = 26 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[26]) >= GetMagicCD(SKILL_FIRESWORD)) then
    begin
      if not m_boFireHitSkill then
        AllowFireHitSkill;
      Result := True;
    end;
  end

  // 龙影剑法
  else if m_wAutoUseMagicID = 42 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[42]) >= GetMagicCD(SKILL_42)) then
    begin
      if not m_bo42Skill then
        Allow42HitSkill;
      Result := True;
    end;
  end

  // 开天斩
  else if m_wAutoUseMagicID = 66 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[66]) >= GetMagicCD(SKILL_66)) then
    begin
      if not m_bo66Skill then
        Allow66HitSkill(nil);
      Result := True;
    end;
  end

  // 断空斩
  else if m_wAutoUseMagicID = 113 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[113]) >= GetMagicCD(SKILL_113)) then
    begin
      if not m_bo113Skill then
        Allow113HitSkill;
      Result := True;
    end;
  end
  else if m_wAutoUseMagicID = 115 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[115]) > GetMagicCD(SKILL_115)) then
    begin
      if not m_bo115Skill then
        Allow115HitSkill;
      Result := True;
    end;
  end

  // 野蛮冲撞
  else if m_wAutoUseMagicID = SKILL_MOOTEBO then
  begin
    if ((MyGetTickCount - m_SkillUseTick[SKILL_MOOTEBO]) > 1000 * 10) then
    begin
      Result := True;
    end;
  end

  // 倚天劈地
  else if m_wAutoUseMagicID = SKILL_114 then
  begin
    if ((MyGetTickCount - m_SkillUseTick[SKILL_114]) > GetMagicCD(SKILL_114)) then
    begin
      Result := True;
    end;
  end

  // 抱月刀法
  else if m_wAutoUseMagicID = 40 then
  begin
    if not m_boCrsHitkill then
    begin
      SkillCrsOnOff(True);
    end;

    Result := True;
  end

  // 英雄彻地钉
  else if m_wAutoUseMagicID = 39 then
  begin
    if (MyGetTickCount - m_SkillUseTick[39] > GetMagicCD(SKILL_GROUPDEDING)) then
    begin
      Result := True;
    end;
  end

  // 半月
  else if m_wAutoUseMagicID = 25 then
  begin
    if not m_boUseHalfMoon then
    begin
      HalfMoonOnOff(True);
    end;
    Result := True;
  end
  else if (m_wAutoUseMagicID >= CUSTOM_MAGIC_START_ID) and (m_wAutoUseMagicID <= CUSTOM_MAGIC_START_ID + CUSTOM_MAGIC_COUNT) then
  begin
    Result := True;
  end
  else
  begin
    Result := True;
  end;
end;

end.

