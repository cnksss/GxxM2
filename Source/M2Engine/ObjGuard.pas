unit ObjGuard;

interface

uses
  Windows, Classes, Grobal2, ObjNpc, SysUtils, Math, M2Definition;

type
  TSuperGuard = class(TNormNpc)
    n564: Integer;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Run; override;
  end;

  TMoveSuperGuard = class(TNormNpc)
    MovePoint: array of TPoint;
    nIdx: Integer;
    nKeepCount: Integer;
    nKeepMaxCount: Integer;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Initialize(); override;
    function AttackTarget(): Boolean;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Run; override;
  end;

implementation

uses
  ObjBase, M2Share, ItemEvent, ObjSmartMon;

{ TSuperGuard }

function TSuperGuard.AttackTarget(): Boolean;
var
  nOldX, nOldY: Integer;
  btOldDir: Byte;
  wHitMode: Word;
begin
  Result := False;
  // ?????修复引擎带刀护卫报错 piaoyun 2013-11-13
  if (m_TargetCret = nil) or (m_TargetCret.m_ObjGame <> Obj_Actor) then
    Exit;

  if m_TargetCret.m_PEnvir = m_PEnvir then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      nOldX := m_nCurrX;
      nOldY := m_nCurrY;
      btOldDir := m_btDirection;
      m_TargetCret.GetBackPosition(m_nCurrX, m_nCurrY);
      m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
      wHitMode := 0;
      _Attack(wHitMode, m_TargetCret);
      m_TargetCret.SetLastHiter(Self);
      m_TargetCret.m_ExpHitter := nil;
      m_nCurrX := nOldX;
      m_nCurrY := nOldY;
      m_btDirection := btOldDir;
      TurnTo(m_btDirection);
      BreakHolySeizeMode();
      // MainOutMessage('_Attack(wHitMode, m_TargetCret)');
    end;
    Result := True;
  end
  else
  begin
    DelTargetCreat();
  end;
end;

constructor TSuperGuard.Create;
begin
  inherited;
  m_btRaceServer := 11;
  m_nViewRange := 7;
  m_nLight := 4;
end;

destructor TSuperGuard.Destroy;
begin

  inherited;
end;

function TSuperGuard.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

procedure TSuperGuard.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  ErrCode: Integer;
begin
  ErrCode := 0;
  try
    if m_Master <> nil then
      m_Master := nil;                                                        // 不允许召唤为宝宝
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      ErrCode := 0;
      m_nHitDelay := 0;
      m_VisibleActors.Lock;
      try
        for I := 0 to m_VisibleActors.Count - 1 do
        begin
          VisibleBaseObject := m_VisibleActors[I].Item;

          if VisibleBaseObject <> nil then
          begin
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
            if (BaseObject = nil) or (BaseObject.m_boDeath) or (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) or (g_Config.boGuardNotAttackPlayMoster
              and (BaseObject.m_btRaceServer = RC_PLAYMOSTER)) then
              Continue;                                                                        // 不攻击镖车
            // if (BaseObject.m_nCopyHumanLevel > 0) and (not g_Config.boAllowGuardAttack) then Continue; {不攻击分身}

            if (BaseObject.m_btRaceServer = RC_ARCHERGUARD{弓箭手}) then
              Continue;
            if (BaseObject.m_btRaceServer = RC_MOVE_ARCHERGUARD{巡回弓箭手}) then
              Continue;
            if (BaseObject is TCopyMon) then
              Continue;

            // 增加大刀卫士不攻击宠物 2020-03-26 00:37:21
            if (BaseObject.m_boGamePet) and (g_Config.boDisableAllAttackPet or (BaseObject.m_boDisableAllAttackPet <> 0)) then
              Continue;

            ErrCode := 3;
            if ((BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and (TSmartObject(BaseObject).PKLevel
              >= 2)) or (((BaseObject.m_btRaceServer >= RC_MONSTER) and (BaseObject.m_btRaceServer <> 157)) and (not BaseObject.m_boMission))
              then
            begin
              ErrCode := 4;
              SetTargetCreat(BaseObject);
              Break;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end;
    ErrCode := 5;
    if m_TargetCret <> nil then
      AttackTarget();
    ErrCode := 6;
    inherited;
    ErrCode := 7;
  except
    MainOutMessage('TSuperGuard.Run Error, ErrCode = ' + IntToStr(ErrCode));
  end;
end;

{ TMoveSuperGuard }

function TMoveSuperGuard.AttackTarget: Boolean;
var
  nOldX, nOldY: Integer;
  btOldDir: Byte;
  wHitMode: Word;
begin
  Result := False;
  // ?????修复引擎带刀护卫报错 piaoyun 2013-11-13
  if (m_TargetCret = nil) or (m_TargetCret.m_boDeath) or (m_TargetCret.m_boGhost) then
    Exit;

  if m_TargetCret.m_PEnvir = m_PEnvir then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      nOldX := m_nCurrX;
      nOldY := m_nCurrY;
      btOldDir := m_btDirection;
      m_TargetCret.GetBackPosition(m_nCurrX, m_nCurrY);
      m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
      wHitMode := 0;
      _Attack(wHitMode, m_TargetCret);
      m_TargetCret.SetLastHiter(Self);
      m_TargetCret.m_ExpHitter := nil;
      m_nCurrX := nOldX;
      m_nCurrY := nOldY;
      m_btDirection := btOldDir;
      TurnTo(m_btDirection);
      BreakHolySeizeMode();
      // MainOutMessage('_Attack(wHitMode, m_TargetCret)');
    end;
    Result := True;
  end
  else
  begin
    DelTargetCreat();
  end;
end;

constructor TMoveSuperGuard.Create;
begin
  inherited Create;
  m_btRaceServer := RC_GUARD;
  m_nViewRange := 7;
  //m_boKeepRun := True;
  //m_boRunAll := False;
  MovePoint := nil;
  m_nTargetX := -1;
  nIdx := 0;
  nKeepCount := 0;
  nKeepMaxCount := 0;
end;

destructor TMoveSuperGuard.Destroy;
begin
  MovePoint := nil;
  inherited Destroy;
end;

procedure TMoveSuperGuard.Initialize;
begin
  inherited;
  m_btRaceServer := RC_GUARD;
end;

function TMoveSuperGuard.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

procedure TMoveSuperGuard.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  ItemObj: TItemObject;
  ErrCode: Integer;
begin
  ErrCode := 0;
  try
    if m_Master <> nil then
      m_Master := nil;                                                                                //不允许召唤为宝宝

    ErrCode := 1;
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_nHitDelay := 0;
      ErrCode := 2;
      m_VisibleActors.Lock;
      try
        ErrCode := 3;
        for I := 0 to m_VisibleActors.Count - 1 do
        begin
          ErrCode := 4;
          VisibleBaseObject := m_VisibleActors[I].Item;

          if VisibleBaseObject <> nil then
          begin
            ErrCode := 5;
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);

            ErrCode := 6;
            if (BaseObject = nil) or (BaseObject.m_boDeath) or (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) or (g_Config.boGuardNotAttackPlayMoster
              and (BaseObject.m_btRaceServer = RC_PLAYMOSTER)) then
              Continue;

            ErrCode := 7;
            if (BaseObject.m_btRaceServer = RC_ARCHERGUARD{弓箭手}) then
              Continue;

            ErrCode := 8;
            if (BaseObject.m_btRaceServer = RC_MOVE_ARCHERGUARD{巡回弓箭手}) then
              Continue;

            ErrCode := 9;
            // 增加大刀卫士不攻击宠物 2020-03-26 00:37:21
            if (BaseObject.m_boGamePet) and (g_Config.boDisableAllAttackPet or (BaseObject.m_boDisableAllAttackPet <> 0)) then
              Continue;

            ErrCode := 10;
            if ((BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and (TSmartObject(BaseObject).PKLevel
              >= 2)) then
            begin
              ErrCode := 11;
              SetTargetCreat(BaseObject);

              ErrCode := 12;
              if m_TargetCret <> nil then   //修复 m_TargetCret 有时候为 nil By 一支笔 at:2021-09-24 14:19:36
                SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
              Break;
            end
            else if ((BaseObject.m_btRaceServer >= RC_MONSTER) and (not BaseObject.m_boMission)) then
            begin
              ErrCode := 13;
              if (BaseObject.m_Master <> nil) then
              begin
                if (not g_Config.boMoveSuperGuardAttackBB) then
                  Continue;
              end
              else
              begin
                if not g_Config.boMoveSuperGuardAttackMon then
                  Continue;
              end;

              ErrCode := 14;
              SetTargetCreat(BaseObject);

              ErrCode := 15;
              if m_TargetCret <> nil then  //修复 m_TargetCret 有时候为 nil By 一支笔 at:2021-09-24 14:19:36
                SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
              Break;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end;

    ErrCode := 16;
    if m_TargetCret <> nil then
    begin
      ErrCode := 17;
      AttackTarget();
    end
    else
    begin
      ErrCode := 18;
      if Length(MovePoint) > 1 then
      begin
        ErrCode := 19;
        if m_boWalkWaitLocked then
        begin
          if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
          begin
            m_boWalkWaitLocked := False;
          end;
        end;

        ErrCode := 20;
        if g_Config.boMoveSuperGuardPickItem and (m_PEnvir <> nil) then
        begin
          ErrCode := 21;
          ItemObj := m_PEnvir.GetItem(m_nCurrX, m_nCurrY);
          if (ItemObj <> nil) and (not ItemObj.m_boGhost) then
          begin
            ErrCode := 22;
            g_MoveGuardPickItemList.Lock;
            try
              ErrCode := 23;
              if g_MoveGuardPickItemList.IndexOf(ItemObj.m_sName) >= 0 then
              begin
                ErrCode := 24;
                ItemObj.MakeGhost;
                SendRefMsg(RM_ITEMHIDE, 0, NativeInt(ItemObj), m_nCurrX, m_nCurrY, '');
              end;
            finally
              g_MoveGuardPickItemList.UnLock;
            end;
          end;
        end;

        ErrCode := 25;
        if not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) then
        begin
          ErrCode := 26;
          m_dwWalkTick := MyGetTickCount();
          m_nWalkDelay := 0;
          Inc(m_nWalkCount);
          if m_nWalkCount > m_nWalkStep then
          begin
            m_nWalkCount := 0;
            m_boWalkWaitLocked := True;
            m_dwWalkWaitTick := MyGetTickCount();
          end;

          ErrCode := 27;
          if (m_nTargetX <> -1) then
          begin
            if ((m_nCurrX = m_nTargetX) and (m_nCurrY = m_nTargetY)) or (nKeepCount > nKeepMaxCount) then
            begin
              m_nTargetX := -1;
            end;
          end;

          ErrCode := 28;
          if m_nTargetX = -1 then
          begin
            ErrCode := 29;
            Inc(nIdx);
            if nIdx > High(MovePoint) then
              nIdx := 0;
            if nIdx < 0 then
              nIdx := 0;

            ErrCode := 30;
            SetTargetXY(MovePoint[nIdx].X, MovePoint[nIdx].Y);

            ErrCode := 31;
            nKeepMaxCount := MAX(abs(m_nCurrX - m_nTargetX), abs(m_nCurrY - m_nTargetY));
            nKeepMaxCount := nKeepMaxCount + Round(nKeepMaxCount * 0.5);

            ErrCode := 32;
            nKeepCount := 0;
          end;

          ErrCode := 33;
          if m_nTargetX <> -1 then
          begin
            ErrCode := 34;
            GotoTargetXY;

            ErrCode := 35;
            Inc(nKeepCount);
          end;
        end;
      end;
    end;

    ErrCode := 36;
    inherited Run;
  except
    MainOutMessage(Format('TMoveSuperGuard.Run; Code=%d', [ErrCode]));
  end;
end;

end.

