unit ObjMon2;

interface

uses
  Windows, Classes, Grobal2, ObjBase, ObjMon, SysUtils, ObjPlayer, M2Definition;

type
  TStickMonster = class(TAnimalObject)
    bo550: Boolean;
    n554: Integer;
    n558: Integer;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean; virtual;
    procedure sub_FFEA; virtual;
    procedure sub_FFE9; virtual;
    procedure VisbleActors; virtual; // FFE8
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    procedure Run; override;
  end;

  TBeeQueen = class(TAnimalObject)
    BBList: TList;
  private
    procedure MakeChildBee;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    procedure Run; override;
  end;

  TCentipedeKingMonster = class(TStickMonster)
    m_dwAttickTick: LongWord; // 0x560
  private
    function sub_4A5B0C: Boolean;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean; override;
    procedure sub_FFE9; override;
    procedure Run; override;
  end;

  TBigHeartMonster = class(TAnimalObject)
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean; virtual;
    procedure Run; override;
  end;

  TSpiderHouseMonster = class(TAnimalObject)
    BBList: TList;
  private
    procedure GenBB;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    procedure Run; override;
  end;

  TExplosionSpider = class(TMonster)
    dw558: LongWord;
  private
    procedure sub_4A65C4;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    function AttackTarget(): Boolean; override; // FFEB
  end;

  TGuardUnit = class(TAnimalObject)
    // dw54C: LongWord;                                                                                // 0x54C
    // m_nX550: Integer;                                                                               // 0x550
    // m_nY554: Integer;                                                                               // 0x554
    m_nDirection: Integer; // 0x558
  public
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override; // FFF4
    procedure Struck(hiter: TBaseObject); override; // FFEC
  end;

  TArcherGuard = class(TGuardUnit)
    m_boAttackType: Boolean; // 攻击白名，或者指定PK值的玩家
    m_nPKpoint: Integer; // 攻击PK值小于多少的角色
  private
    procedure sub_4A6B30(TargeTBaseObject: TBaseObject);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    procedure Initialize; override;
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override;
  end;

  TMoveArcherGuard = class(TGuardUnit)
    m_boAttackType: Boolean; // 攻击白名，或者指定PK值的玩家
    m_nPKpoint: Integer; // 攻击PK值小于多少的角色

    MovePoint: array of TPoint;
    nIdx: Integer;
    nKeepCount: Integer;
    nKeepMaxCount: Integer;
  private
    procedure sub_4A6B30(TargeTBaseObject: TBaseObject);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    procedure Initialize; override;
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override;
  end;

  TDevilkingArcherGuard = class(TGuardUnit) // 魔王岭弓箭手
  private
    procedure sub_4A6B30(TargeTBaseObject: TBaseObject);
  public
    constructor Create(); override;
    destructor Destroy; override;
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override; // FFF4
    procedure Run; override;
  end;

  // Mon27-6 冰柱怪物 piaoyun 2013-11-16
  TIcicleMonster = class(TGuardUnit)
    m_boAttackType: Boolean;
    m_nPKpoint: Integer;
  private
    procedure AttackTarget(TargeTBaseObject: TBaseObject);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    procedure Initialize; override;
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override;
  end;

  TArcherPolice = class(TArcherGuard)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TCastleDoor = class(TGuardUnit)
    dw55C: LongWord;
    dw560: LongWord;
    m_boOpened: Boolean;
    bo565n: Boolean;
    bo566n: Boolean;
    bo567n: Boolean;
  private
    procedure SetMapXYFlag(nFlag: Integer);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Die; override;
    procedure Run; override;
    procedure Initialize(); override;
    procedure Close;
    procedure Open;
    procedure RefStatus;
  end;

  TWallStructure = class(TGuardUnit)
    n55C: Integer;
    dw560: LongWord;
    boSetMapFlaged: Boolean;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Initialize; override;
    procedure Die; override;
    procedure Run; override;
    procedure RefStatus;
  end;

  TSoccerBall = class(TAnimalObject)
  private
    n550: Integer; // 足球前进步数
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Struck(hiter: TBaseObject); override;
    procedure Run; override;
  end;

  TExperienceMon = class(TAnimalObject)
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Struck(hiter: TBaseObject); override;
    procedure GiveHitterExp(Hitter: TBaseObject; IsMagic: Boolean; Power: Integer); override;
  end;

  TGuardMonster = class(TAnimalObject)
    n564: Integer;
    CanMoveMode: Boolean;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Run; override;
  end;

implementation

uses
  Math, M2Share, HUtil32, Castle, Guild, ItemEvent;
{ TDevilkingArcherGuard }

constructor TDevilkingArcherGuard.Create;
begin
  inherited;
  m_nViewRange := 12;
  m_boWantRefMsg := True;
  m_Castle := nil;
  m_nDirection := -1;
  m_btRaceServer := 109;
end;

destructor TDevilkingArcherGuard.Destroy;
begin

  inherited;
end;

function TDevilkingArcherGuard.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  Result := False;
  if BaseObject = nil then
    Exit;
  Result := (BaseObject.m_btRaceServer = 108) or ((BaseObject.m_btRaceServer = 154) and (BaseObject.m_btRaceImg = 156)
    { 自定义怪物 - 魔王岭怪物 } );
  { if (BaseObject <> nil) and
    (BaseObject.m_btRaceServer >= RC_ANIMAL) and
    (BaseObject.m_btRaceServer <> 109) and
    (BaseObject.m_btRaceServer <> RC_ARCHERGUARD) then Result := True;

    if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) then Result := False;

    if (BaseObject <> nil) and
    (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or
    BaseObject.m_boStoneMode or
    ((BaseObject.m_btRaceServer >= 10) and
    (BaseObject.m_btRaceServer < 50) and
    (BaseObject.m_btRaceServer = RC_ARCHERGUARD) and
    (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and
    (BaseObject.m_btRaceServer = 109)) or
    (BaseObject = Self) then begin
    Result := False;
    end;
    if m_LastHiter = BaseObject then Result := True;
    if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = 109) then
    Result := True; }
end;

procedure TDevilkingArcherGuard.sub_4A6B30(TargeTBaseObject: TBaseObject);
var
  nPower: Integer;
  WAbil: pTAbility;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  if TargeTBaseObject <> nil then
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY);
    WAbil := @m_WAbil;
    nPower := SmallInt(WAbil.DC2 - WAbil.DC1) + 1;
    if nPower > 0 then
      nPower := Random(nPower);

    nPower := nPower + WAbil.DC1;

    // 吸收伤害
    if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(TargeTBaseObject);

      // 伤害吸收百分比 2020-09-17 20:11:44
      nPower := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nPower);

      if (SmartObject.m_nSuckDamagePoint > 0) and (nPower > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;

        if Random(100) < SmartObject.m_nSuckDamageProbability then
        begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower := Max(nPower - nSuckDamagePoint, 0);
        end;
      end;
    end;

    if nPower > 0 then
    begin
      if not CanCloseDefense then // 忽视目标防御
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil)
      else
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil, 4); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37

      nPower := TargeTBaseObject.NewAbilPower(2, nPower); // 物伤减少
    end;

    nPower := NewAbilPower(1, nPower); // 元素增加攻击伤害

    nPower := GetNextDamage(nPower);

    // 怪物伤害封顶 chongchong 2016-09-07
    nPower := TargeTBaseObject.GetAttackPowerMax(nPower);

    if nPower > 0 then
    begin
      TargeTBaseObject.SetLastHiter(Self);
      TargeTBaseObject.m_ExpHitter := nil;
      nPower := TargeTBaseObject.StruckDamage(nPower, Self, 0);
      TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, TargeTBaseObject.m_WAbil.HP,
        TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX),
        abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);

      if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and
        (Random(Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
      begin // 麻痹
        TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;

      nPower := TargeTBaseObject.DamageReboundPower(nPower);
      if nPower > 0 then
      begin // 反弹伤害
        nPower := StruckDamage(nPower, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject), 'FT',
          _MAX(abs(TargeTBaseObject.m_nCurrX - m_nCurrX), abs(TargeTBaseObject.m_nCurrY - m_nCurrY)) * 50 + 600);
      end;
    end;
    SendRefMsg(RM_FLYAXE, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(TargeTBaseObject), '');
  end;
end;

procedure TDevilkingArcherGuard.Run;
var
  I, nAbs, nRage: Integer;
  BaseObject: TBaseObject;
  TargeTBaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if (m_Master <> nil) and (m_Master.m_PEnvir <> m_PEnvir) then
  begin
    MakeGhost;
    Exit;
  end;

  nRage := 9999;
  TargeTBaseObject := nil;
  if not m_boDeath and not m_boGhost and CanMove() then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      m_VisibleActors.Lock;
      try
        for I := 0 to m_VisibleActors.Count - 1 do
        begin
          VisibleBaseObject := m_VisibleActors[I].Item;
          if VisibleBaseObject <> nil then
          begin
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
            if BaseObject = nil then
              Continue;
            if BaseObject.m_boDeath then
              Continue;

            // 怪物不攻击脱机人物 chongchong 2015-09-07
            if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
            then
            begin
              Continue;
            end;

            if IsProperTarget(BaseObject) then
            begin
              nAbs := abs(m_nCurrX - BaseObject.m_nCurrX) + abs(m_nCurrY - BaseObject.m_nCurrY);
              if nAbs < nRage then
              begin
                nRage := nAbs;
                TargeTBaseObject := BaseObject;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;

      if TargeTBaseObject <> nil then
      begin
        SetTargetCreat(TargeTBaseObject);
      end
      else
      begin
        DelTargetCreat();
      end;
    end;
    if m_TargetCret <> nil then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime then
      begin
        m_dwHitTick := MyGetTickCount();
        sub_4A6B30(m_TargetCret);
      end;
    end
    else
    begin
      if (m_nDirection >= 0) and (m_btDirection <> m_nDirection) then
      begin
        TurnTo(m_nDirection);
      end;
    end;
  end;
  inherited;
end;

{ TStickMonster }

constructor TStickMonster.Create;
begin
  inherited;
  bo550 := False;
  m_nViewRange := 7;
  m_nRunTime := 250;
  m_dwSearchTime := Random(1500) + 2500;
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 85;
  n554 := 4;
  n558 := 4;
  m_boFixedHideMode := True;
  m_boStickMode := True;
  m_boAnimal := True;
end;

destructor TStickMonster.Destroy;
begin

  inherited;
end;

function TStickMonster.AttackTarget: Boolean;
var
  btDir: Byte;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if GetAttackDir(m_TargetCret, btDir) then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      // MainOutMessage('攻击间隔：：：' + IntToStr(tick_diff(m_dwHitTick, MyGetTickCount)));
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      Attack(m_TargetCret, btDir);
    end;
    Result := True;
    Exit;
  end;
  if m_TargetCret.m_PEnvir = m_PEnvir then
  begin
    SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
  end
  else
  begin
    DelTargetCreat();
  end;
end;

procedure TStickMonster.sub_FFE9();
begin
  m_boFixedHideMode := False;
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
end;

procedure TStickMonster.VisbleActors();
var
  I: Integer;
  VisibleBaseObject: pTVisibleBaseObject;
resourcestring
  sExceptionMsg = '[Exception] TStickMonster.VisbleActors Dispose';
begin
  SendRefMsg(RM_DIGDOWN, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  try
    m_VisibleActors.Lock;
    try
      for I := 0 to m_VisibleActors.Count - 1 do
      begin
        VisibleBaseObject := m_VisibleActors[I].Item;
        if VisibleBaseObject <> nil then
          Dispose(VisibleBaseObject);
      end;
      m_VisibleActors.Clear;
    finally
      m_VisibleActors.UnLock;
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
  m_boFixedHideMode := True;
end;

procedure TStickMonster.sub_FFEA();
var
  BaseObject: TBaseObject;
  I: Integer;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  m_VisibleActors.Lock;
  try
    for I := 0 to m_VisibleActors.Count - 1 do
    begin
      VisibleBaseObject := m_VisibleActors[I].Item;
      if VisibleBaseObject <> nil then
      begin
        BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
        if BaseObject = nil then
          Continue;
        if BaseObject.m_boDeath then
          Continue;
        if IsProperTarget(BaseObject) then
        begin
          if not BaseObject.m_boHideMode or m_boCoolEye then
          begin
            if (abs(m_nCurrX - BaseObject.m_nCurrX) < n554) and (abs(m_nCurrY - BaseObject.m_nCurrY) < n554) then
            begin
              sub_FFE9();
              Break;
            end;
          end;
        end;
      end;
    end;
  finally
    m_VisibleActors.UnLock;
  end;
end;

function TStickMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

procedure TStickMonster.Run;
var
  bo05: Boolean;
begin
  if not m_boGhost and not m_boDeath and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      if m_boFixedHideMode then
      begin
        sub_FFEA();
      end
      else
      begin
        if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
        begin
          m_nHitDelay := 0;
          SearchTarget();
        end;
        bo05 := False;
        if m_TargetCret <> nil then
        begin
          if (abs(m_TargetCret.m_nCurrX - m_nCurrX) > n558) or (abs(m_TargetCret.m_nCurrY - m_nCurrY) > n558) then
          begin
            bo05 := True;
          end;
        end
        else
          bo05 := True;
        if bo05 then
        begin
          VisbleActors();
        end
        else
        begin
          if AttackTarget then
          begin
            inherited;
            Exit;
          end;
        end;
      end;
    end;
  end;
  inherited;
end;

{ TSoccerBall }

constructor TSoccerBall.Create;
begin
  inherited;
  m_boAnimal := False;
  m_boSuperMan := True;
  n550 := 0;
  m_nTargetX := -1;
end;

destructor TSoccerBall.Destroy;
begin

  inherited;
end;

procedure TSoccerBall.Run;
var
  n08, n0C: Integer;
begin
  try
    if n550 > 0 then
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, 1, n08, n0C) then
      begin
        if m_PEnvir.CanWalk(n08, n0C, False) then
        begin
          case m_btDirection of //
            { 0: m_btDirection := 4; //20100629 修改
              1: m_btDirection := 7;
              2: m_btDirection := 6;
              3: m_btDirection := 5;
              4: m_btDirection := 0;
              5: m_btDirection := 3;
              6: m_btDirection := 2;
              7: m_btDirection := 1; }
            0:
              m_btDirection := 4;
            1:
              m_btDirection := 5;
            2:
              m_btDirection := 6;
            3:
              m_btDirection := 7;
            4:
              m_btDirection := 0;
            5:
              m_btDirection := 1;
            6:
              m_btDirection := 2;
            7:
              m_btDirection := 3;
          end; // case
          // m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, n550, {m_nTargetX, m_nTargetY}n08, n0C);//20100629 修改
        end;
      end;
    end
    else
    begin // 004A78A1
      m_nTargetX := -1;
    end;

    if m_nTargetX <> -1 then
    begin
      GotoTargetXY();
      if (m_nTargetX = m_nCurrX) and (m_nTargetY = m_nCurrY) then
        n550 := 0;
      if n550 > 0 then
        Dec(n550); // 20100629 增加
    end;
  except
    MainOutMessage('TSoccerBall.Run');
  end;
  inherited;
end;

procedure TSoccerBall.Struck(hiter: TBaseObject);
begin
  if hiter = nil then
    Exit;
  m_btDirection := hiter.m_btDirection;
  n550 := Random(4) + (n550 + 4);
  n550 := _MIN(20, n550);
  m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, n550, m_nTargetX, m_nTargetY);
end;

{ TBeeQueen }

constructor TBeeQueen.Create;
begin
  inherited;
  m_nViewRange := 9;
  m_nRunTime := 250;
  m_dwSearchTime := Random(1500) + 2500;
  m_dwSearchTick := MyGetTickCount();
  m_boStickMode := True;
  BBList := TList.Create;
end;

destructor TBeeQueen.Destroy;
begin
  BBList.Free;
  inherited;
end;

procedure TBeeQueen.MakeChildBee;
begin
  if BBList.Count >= 15 then
    Exit;
  SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  SendDelayMsg(Self, RM_ZEN_BEE, 0, 0, 0, 0, '', 500);
end;

function TBeeQueen.Operate(ProcessMsg: pTProcessMessage): Boolean;
var
  BB: TBaseObject;
begin
  if ProcessMsg.wIdent = RM_ZEN_BEE then
  begin
    BB := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, m_nCurrX, m_nCurrY, g_Config.sBee);
    if BB <> nil then
    begin
      BB.SetTargetCreat(m_TargetCret);
      BBList.Add(BB);
    end;
  end;
  Result := inherited Operate(ProcessMsg);
end;

procedure TBeeQueen.Run;
var
  I: Integer;
  BB: TBaseObject;
begin
  if not m_boGhost and not m_boDeath and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      if tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime then
      begin
        m_dwHitTick := MyGetTickCount();
        SearchTarget();
        if m_TargetCret <> nil then
          MakeChildBee();
      end;
      for I := BBList.Count - 1 downto 0 do
      begin
        BB := TBaseObject(BBList.Items[I]);
        if (BB <> nil) and (BB.m_boDeath) or (BB.m_boGhost) then
          BBList.Delete(I);
      end;
    end;
  end;
  inherited;
end;

{ TCentipedeKingMonster }

constructor TCentipedeKingMonster.Create;
begin
  inherited;
  m_nViewRange := 8; // 增大视角范围 chongchong 2015-11-18
  n554 := 4;
  n558 := 6;
  m_boAnimal := False;
  m_dwAttickTick := MyGetTickCount();
end;

destructor TCentipedeKingMonster.Destroy;
begin

  inherited;
end;

function TCentipedeKingMonster.sub_4A5B0C: Boolean;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  Result := False;
  m_VisibleActors.Lock;
  try
    for I := 0 to m_VisibleActors.Count - 1 do
    begin
      VisibleBaseObject := m_VisibleActors[I].Item;

      if VisibleBaseObject <> nil then
      begin
        BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
        if BaseObject = nil then
          Continue;
        if BaseObject.m_boDeath then
          Continue;
        if BaseObject.m_boGhost then
          Continue;

        // 怪物不攻击脱机人物 chongchong 2015-09-07
        if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
        begin
          Continue;
        end;

        if IsProperTarget(BaseObject) then
        begin
          if (abs(m_nCurrX - BaseObject.m_nCurrX) <= m_nViewRange) and (abs(m_nCurrY - BaseObject.m_nCurrY) <= m_nViewRange) then
          begin
            Result := True;
            Break;
          end;
        end;
      end;
    end;
  finally
    m_VisibleActors.UnLock;
  end;
end;

function TCentipedeKingMonster.AttackTarget: Boolean;
var
  WAbil: pTAbility;
  I, nPower: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  Result := False;
  if not sub_4A5B0C then
  begin
    Exit;
  end;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    SendAttackMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY);
    WAbil := @m_WAbil;
    nPower := SmallInt(WAbil.DC2 - WAbil.DC1) + 1;
    if nPower > 0 then
      nPower := Random(nPower);
    nPower := nPower + WAbil.DC1;

    // nPower := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
    m_VisibleActors.Lock;
    try
      for I := 0 to m_VisibleActors.Count - 1 do
      begin
        VisibleBaseObject := m_VisibleActors[I].Item;

        if VisibleBaseObject <> nil then
        begin
          BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
          if BaseObject = nil then
            Continue;
          if BaseObject.m_boDeath then
            Continue;
          if BaseObject.m_boGhost then
            Continue;

          // 怪物不攻击脱机人物 chongchong 2015-09-07
          if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
          then
          begin
            Continue;
          end;

          if IsProperTarget(BaseObject) then
          begin
            if (abs(m_nCurrX - BaseObject.m_nCurrX) <= m_nViewRange) and (abs(m_nCurrY - BaseObject.m_nCurrY) <= m_nViewRange)
            then
            begin
              m_dwTargetFocusTick := MyGetTickCount();
              SendDelayMsg(Self, RM_DELAYMAGIC, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 2,
                NativeInt(BaseObject), '', 500);
              if Random(4) = 0 then
              begin
                if Random(3) <> 0 then
                begin
                  if (Random(BaseObject.m_btAntiPoison + 20) = 0) and (not BaseObject.UnPosion) then // 防毒
                    BaseObject.MakePosion(POISON_DECHEALTH, 60, 3);
                end
                else
                begin
                  if BaseObject.CanStone() then
                  begin
                    BaseObject.MakePosion(POISON_STONE, 5, 0);
                  end;
                end;
              end;
              m_TargetCret := BaseObject;
            end;
          end;
        end;
      end;
    finally
      m_VisibleActors.UnLock;
    end;
  end;
  Result := True;
end;

procedure TCentipedeKingMonster.sub_FFE9;
begin
  inherited;
  m_WAbil.HP := m_WAbil.MaxHP;
end;

procedure TCentipedeKingMonster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if not m_boGhost and not m_boDeath and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      if m_boFixedHideMode then
      begin
        if (MyGetTickCount - m_dwAttickTick) > 10000 then
        begin
          m_VisibleActors.Lock;
          try
            for I := 0 to m_VisibleActors.Count - 1 do
            begin
              VisibleBaseObject := m_VisibleActors[I].Item;

              if VisibleBaseObject <> nil then
              begin
                BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
                if BaseObject = nil then
                  Continue;
                if BaseObject.m_boDeath then
                  Continue;
                if BaseObject.m_boGhost then
                  Continue;

                // 怪物不攻击脱机人物 chongchong 2015-09-07
                if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
                then
                begin
                  Continue;
                end;

                if IsProperTarget(BaseObject) then
                begin
                  if not BaseObject.m_boHideMode or m_boCoolEye then
                  begin
                    if (abs(m_nCurrX - BaseObject.m_nCurrX) < n554) and (abs(m_nCurrY - BaseObject.m_nCurrY) < n554) then
                    begin
                      sub_FFE9();
                      m_dwAttickTick := MyGetTickCount();
                      Break;
                    end;
                  end;
                end;
              end;
            end;
          finally
            m_VisibleActors.UnLock;
          end;
        end;
      end
      else
      begin
        if (MyGetTickCount - m_dwAttickTick) > 3000 then
        begin
          if AttackTarget() then
          begin
            inherited;
            Exit;
          end;
          if (MyGetTickCount - m_dwAttickTick) > 10000 then
          begin
            VisbleActors();
            m_dwAttickTick := MyGetTickCount();
          end;
        end;
      end;
    end;
  end;
  inherited;
end;

{ TBigHeartMonster }

constructor TBigHeartMonster.Create;
begin
  inherited;
  m_nViewRange := 16;
  m_boAnimal := False;
end;

destructor TBigHeartMonster.Destroy;
begin

  inherited;
end;

function TBigHeartMonster.AttackTarget(): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower: Integer;
  WAbil: pTAbility;
begin
  Result := False;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    WAbil := @m_WAbil;

    nPower := SmallInt(WAbil.DC2 - WAbil.DC1) + 1;
    if nPower > 0 then
      nPower := Random(nPower);
    nPower := nPower + WAbil.DC1;

    BaseObjectList := TList.Create;
    try
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, m_nViewRange, BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if BaseObject = nil then
          Continue;
        if BaseObject.m_boDeath then
          Continue;

        // 怪物不攻击脱机人物 chongchong 2015-09-07
        if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
        begin
          Continue;
        end;

        if IsProperTarget(BaseObject) then
        begin
          if (abs(m_nCurrX - BaseObject.m_nCurrX) <= m_nViewRange) and (abs(m_nCurrY - BaseObject.m_nCurrY) <= m_nViewRange) then
          begin
            SendDelayMsg(Self, RM_DELAYMAGIC, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 1,
              NativeInt(BaseObject), '', 200);
            SendRefMsg(RM_10205, 0, BaseObject.m_nCurrX, BaseObject.m_nCurrY, 1 { type } , '');
          end;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;

    Result := True;
  end;
  // inherited;

end;

procedure TBigHeartMonster.Run;
begin
  if not m_boGhost and not m_boDeath and CanMove then
  begin
    if m_VisibleActors.Count > 0 then
      AttackTarget();
  end;
  inherited;
end;

{ TSpiderHouseMonster }

constructor TSpiderHouseMonster.Create;
begin
  inherited;
  m_nViewRange := 9;
  m_nRunTime := 250;
  m_dwSearchTime := Random(1500) + 2500;
  m_dwSearchTick := 0;
  m_boStickMode := True;
  BBList := TList.Create;
end;

destructor TSpiderHouseMonster.Destroy;
begin
  BBList.Free;
  inherited;
end;

procedure TSpiderHouseMonster.GenBB;
begin
  if BBList.Count < 15 then
  begin
    SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    SendDelayMsg(Self, RM_ZEN_BEE, 0, 0, 0, 0, '', 500);
  end;
end;

function TSpiderHouseMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;
var
  BB: TBaseObject;
  n08, n0C: Integer;
begin
  if ProcessMsg.wIdent = RM_ZEN_BEE then
  begin
    n08 := m_nCurrX;
    n0C := m_nCurrY + 1;
    if m_PEnvir.CanWalk(n08, n0C, True) then
    begin
      BB := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, n08, n0C, g_Config.sSpider);
      if BB <> nil then
      begin
        BB.SetTargetCreat(m_TargetCret);
        BBList.Add(BB);
      end;
    end;
  end;
  Result := inherited Operate(ProcessMsg);
end;

procedure TSpiderHouseMonster.Run;
var
  I: Integer;
  BB: TBaseObject;
begin
  if not m_boGhost and not m_boDeath and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      if tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime then
      begin
        m_dwHitTick := MyGetTickCount();
        SearchTarget();
        if m_TargetCret <> nil then
          GenBB();
      end;
      for I := BBList.Count - 1 downto 0 do
      begin
        if BBList.Count <= 0 then
          Break;
        BB := TBaseObject(BBList.Items[I]);
        if BB <> nil then
        begin
          if BB.m_boDeath or (BB.m_boGhost) then
            BBList.Delete(I);
        end;
      end; // for
    end;
  end;
  inherited;
end;

{ TExplosionSpider }

constructor TExplosionSpider.Create;
begin
  inherited;
  m_nViewRange := 5;
  m_nRunTime := 250;
  m_dwSearchTime := Random(1500) + 2500;
  m_dwSearchTick := 0;
  dw558 := MyGetTickCount();
end;

destructor TExplosionSpider.Destroy;
begin

  inherited;
end;

procedure TExplosionSpider.sub_4A65C4;
var
  WAbil: pTAbility;
  I, nPower, n10, n1, n2: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  SmartObject: TSmartObject;
  nSuckDamagePoint: Integer;
begin
  m_WAbil.HP := 0;
  WAbil := @m_WAbil;

  nPower := SmallInt(WAbil.DC2 - WAbil.DC1) + 1;
  if nPower > 0 then
    nPower := Random(nPower);
  nPower := nPower + WAbil.DC1;

  // nPower := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
  m_VisibleActors.Lock;
  try
    for I := 0 to m_VisibleActors.Count - 1 do
    begin
      VisibleBaseObject := m_VisibleActors[I].Item;

      if VisibleBaseObject <> nil then
      begin
        BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
        if BaseObject = nil then
          Continue;
        if BaseObject.m_boDeath then
          Continue;
        if BaseObject.m_boGhost then
          Continue;

        // 怪物不攻击脱机人物 chongchong 2015-09-07
        if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
        begin
          Continue;
        end;

        if IsProperTarget(BaseObject) then
        begin
          if (abs(m_nCurrX - BaseObject.m_nCurrX) <= 1) and (abs(m_nCurrY - BaseObject.m_nCurrY) <= 1) then
          begin
            if not CanCloseDefense then
            begin // 忽视目标防御
              n1 := BaseObject.GetHitStruckDamage(Self, nPower div 2, nil);
              n2 := BaseObject.GetMagStruckDamage(Self, nPower div 2, nil);
            end
            else
            begin
              n1 := nPower div 2;
              n2 := nPower div 2;
            end;

            if n1 + n2 > 0 then
            begin
              n1 := BaseObject.NewAbilPower(2, n1); // 物伤减少
              n2 := BaseObject.NewAbilPower(3, n2); // 物伤减少

              n10 := n1 + n2;
              n10 := NewAbilPower(1, n10); // 元素增加攻击伤害

              n10 := GetNextDamage(n10);

              // 怪物伤害封顶 chongchong 2016-09-07
              n10 := BaseObject.GetAttackPowerMax(n10);

              // 吸收伤害
              if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
              begin
                SmartObject := TSmartObject(BaseObject);

                // 伤害吸收百分比 2020-09-17 20:11:44
                n10 := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, n10);

                if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
                begin
                  n10 := Max(0, n10 - SmartObject.GetNGDecPower);
                  SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                  SmartObject.RefAbilNH;
                end;

                if (SmartObject.m_nSuckDamagePoint > 0) and (n10 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
                begin
                  if Random(100) < SmartObject.m_nSuckDamageProbability then
                  begin
                    nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * n10);
                    if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
                      nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
                    Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
                    n10 := Max(n10 - nSuckDamagePoint, 0);
                  end;
                end;
              end;

              if n10 > 0 then
              begin
                n10 := BaseObject.StruckDamage(n10, Self, 0);
                BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n10, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                  NativeInt(Self), '', 700);

                if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and
                  (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
                begin // 麻痹
                  BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
                end;

                n10 := BaseObject.DamageReboundPower(n10);
                if n10 > 0 then
                begin
                  n10 := StruckDamage(n10, nil, 0);
                  SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n10, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject),
                    'FT', 700);
                end;
                // BaseObject.SendMsg(TBaseObject(RM_STRUCK), RM_10101, n10, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP, NativeInt(Self), '');
              end;
            end;
          end;
        end;
      end;
    end;
  finally
    m_VisibleActors.UnLock;
  end;
end;

function TExplosionSpider.AttackTarget: Boolean;
var
  btDir: Byte;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if GetAttackDir(m_TargetCret, btDir) then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      sub_4A65C4();
    end;
    Result := True;
  end
  else
  begin
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      // 004A8FE3
    end
    else
    begin
      DelTargetCreat();
      // 004A9009
    end;
  end;

end;

procedure TExplosionSpider.Run;
begin
  if not m_boDeath and not m_boGhost then
  begin
    if (MyGetTickCount - dw558) > 60 * 1000 then
    begin
      dw558 := MyGetTickCount();
      sub_4A65C4();
    end;

    // 爆裂蜘蛛单独刷时不爆 2019-09-01 00:59:07
    if (MyGetTickCount - m_dwSearchTick > m_dwSearchTime) or ((MyGetTickCount - m_dwSearchTick > 1000) and (m_TargetCret = nil))
    then
    begin
      m_dwSearchTick := MyGetTickCount();
      SearchTarget();
    end;
  end;

  inherited;
end;

{ TGuardUnit }

procedure TGuardUnit.Struck(hiter: TBaseObject);
begin
  inherited;
  if m_Castle <> nil then
  begin
    bo2B0 := True;
    m_dw2B4Tick := MyGetTickCount();
  end;
end;

function TGuardUnit.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  Result := False;
  if m_Castle <> nil then
  begin
    if m_LastHiter = BaseObject then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.bo2B0) then
    begin
      if (MyGetTickCount - BaseObject.m_dw2B4Tick) < 2 * 60 * 1000 then
      begin
        Result := True;
      end
      else
        BaseObject.bo2B0 := False;
      if BaseObject.m_Castle <> nil then
      begin
        BaseObject.bo2B0 := False;
        Result := False;
      end;
    end;
    if TUserCastle(m_Castle).m_boUnderWar then
      Result := True;
    if TUserCastle(m_Castle).m_MasterGuild <> nil then
    begin
      if BaseObject.m_Master = nil then
      begin
        if (TUserCastle(m_Castle).m_MasterGuild = BaseObject.m_MyGuild) or
          (TUserCastle(m_Castle).m_MasterGuild.IsAllyGuild(TGUild(BaseObject.m_MyGuild))) then
        begin
          if m_LastHiter <> BaseObject then
            Result := False;
        end;
      end
      else
      begin // 004A6988
        if (TUserCastle(m_Castle).m_MasterGuild = BaseObject.m_Master.m_MyGuild) or
          (TUserCastle(m_Castle).m_MasterGuild.IsAllyGuild(TGUild(BaseObject.m_Master.m_MyGuild))) then
        begin
          if (m_LastHiter <> BaseObject.m_Master) and (m_LastHiter <> BaseObject) then
            Result := False;
        end;
      end;
    end; // 004A69EF
    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
      ((BaseObject.m_btRaceServer >= 10) and (BaseObject.m_btRaceServer < 50)) or (BaseObject = Self) or
      (BaseObject.m_Castle = Self.m_Castle) then
    begin
      Result := False;
    end;
    Exit;
  end; // 004A6A41
  if m_LastHiter = BaseObject then
    Result := True;
  if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
    Result := True;
  if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and
    (TSmartObject(BaseObject).PKLevel >= 2) then
    Result := True;
  if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
    (BaseObject = Self) then
    Result := False;
end;

{ TArcherGuard }

constructor TArcherGuard.Create; // 004A6AB4
begin
  inherited;
  m_nViewRange := 12;
  m_boWantRefMsg := True;
  m_Castle := nil;
  m_nDirection := -1;
  m_btRaceServer := RC_ARCHERGUARD;
  m_boAttackType := False;
  m_nPKpoint := 0; // 攻击PK值小于多少的角色
end;

destructor TArcherGuard.Destroy;
begin

  inherited;
end;

procedure TArcherGuard.Initialize;
begin
  inherited;
  m_boAttackType := GetArcherGuardPKMon(m_sCharName, m_nPKpoint);
  // MainOutMessage(m_sCharName+ ' m_boAttackType '+booltostr(m_boAttackType));
end;

function TArcherGuard.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  Result := False;
  if m_boAttackType then
  begin
    if m_LastHiter = BaseObject then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
      Result := True;
    // MainOutMessage(m_sCharName+ ' m_nPkPoint '+IntToStr2(TSmartObject(BaseObject).m_nPkPoint));
    if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and
      (TSmartObject(BaseObject).m_nPKpoint <= m_nPKpoint) then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
      (BaseObject = Self) then
      Result := False;
  end
  else
  begin
    if m_Castle <> nil then
    begin
      if m_LastHiter = BaseObject then
        Result := True;

      if (BaseObject <> nil) and (BaseObject.bo2B0) then
      begin
        if (MyGetTickCount - BaseObject.m_dw2B4Tick) < 2 * 60 * 1000 then
          Result := True
        else
          BaseObject.bo2B0 := False;

        if BaseObject.m_Castle <> nil then
        begin
          BaseObject.bo2B0 := False;
          Result := False;
        end;
      end;

      if TUserCastle(m_Castle).m_boUnderWar then
        Result := True;

      if TUserCastle(m_Castle).m_MasterGuild <> nil then
      begin
        if (BaseObject <> nil) and (BaseObject.m_Master = nil) then
        begin
          if (TUserCastle(m_Castle).m_MasterGuild = BaseObject.m_MyGuild) //
            or (TUserCastle(m_Castle).m_MasterGuild.IsAllyGuild(TGUild(BaseObject.m_MyGuild))) then
          begin
            if m_LastHiter <> BaseObject then
              Result := False;
          end;
        end
        else
        begin
          if (TUserCastle(m_Castle).m_MasterGuild = BaseObject.m_Master.m_MyGuild) or
            (TUserCastle(m_Castle).m_MasterGuild.IsAllyGuild(TGUild(BaseObject.m_Master.m_MyGuild))) then
          begin
            if (m_LastHiter <> BaseObject.m_Master) and (m_LastHiter <> BaseObject) then
              Result := False;
          end;
        end;
      end;

      if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
        ((BaseObject.m_btRaceServer >= 10) and (BaseObject.m_btRaceServer < 50)) or (BaseObject = Self) or
        (BaseObject.m_Castle = Self.m_Castle) then
      begin
        Result := False;
      end;
      Exit;
    end;

    if m_LastHiter = BaseObject then
      Result := True;

    if (BaseObject <> nil) and (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
      Result := True;

    if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and
      (TSmartObject(BaseObject).PKLevel >= 2) then
      Result := True;

    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
      (BaseObject = Self) then
      Result := False;
  end;
end;

procedure TArcherGuard.sub_4A6B30(TargeTBaseObject: TBaseObject); // 004A6B30
var
  nPower, nSuckDamagePoint: Integer;
  // WAbil: pTAbility;
  SmartObject: TSmartObject;
begin
  if TargeTBaseObject <> nil then
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY);
    // WAbil := @m_WAbil;

    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));

    if (nPower > 0) then
    begin
      if (not CanCloseDefense) then // 忽视目标防御
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil)
      else
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil, 4); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    end;

    nPower := TargeTBaseObject.NewAbilPower(2, nPower); // 物伤减少

    nPower := GetNextDamage(nPower);

    // 怪物伤害封顶 chongchong 2016-09-07
    nPower := TargeTBaseObject.GetAttackPowerMax(nPower);

    if nPower > 0 then
    begin
      nPower := NewAbilPower(1, nPower);
      // 受弓箭手攻击时，支持SetSuckDamage设置的伤害吸收 chongchong 2015-03-29
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);

        // 伤害吸收百分比 2020-09-17 20:11:44
        nPower := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nPower);

        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;

        if (SmartObject.m_nSuckDamagePoint > 0) and (nPower > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
              nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower := Max(nPower - nSuckDamagePoint, 0);
          end;
        end;
      end;

      TargeTBaseObject.SetLastHiter(Self);
      TargeTBaseObject.m_ExpHitter := nil;
      nPower := TargeTBaseObject.StruckDamage(nPower, Self, 0);
      TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, TargeTBaseObject.m_WAbil.HP,
        TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX),
        abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);
      if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and
        (Random(Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
      begin // 麻痹
        TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;

      nPower := TargeTBaseObject.DamageReboundPower(nPower);
      if nPower > 0 then
      begin // 反弹伤害
        nPower := StruckDamage(nPower, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject), 'FT',
          _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX), abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);
      end;
    end;
    SendRefMsg(RM_FLYAXE, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(TargeTBaseObject), '');
  end;
end;

procedure TArcherGuard.Run;
var
  Code: Integer;
  I, nAbs, nRage: Integer;
  BaseObject: TBaseObject;
  TargeTBaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  Code := 0;
  try
    nRage := 9999;
    TargeTBaseObject := nil;
    if not m_boDeath and not m_boGhost and CanMove then
    begin
      Code := 1;
      if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
      begin
        Code := 2;
        m_dwWalkTick := MyGetTickCount();
        m_nWalkDelay := 0;
        Code := 3;
        m_VisibleActors.Lock;
        try
          Code := 4;
          for I := 0 to m_VisibleActors.Count - 1 do
          begin
            Code := 5;
            VisibleBaseObject := m_VisibleActors[I].Item;

            Code := 6;
            if VisibleBaseObject <> nil then
            begin
              Code := 7;
              BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
              if BaseObject = nil then
                Continue;
              if BaseObject.m_boDeath then
                Continue;
              if BaseObject.m_boGhost then
                Continue;

              Code := 8;
              if (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) then
                Continue; // 不攻击镖车
              if (BaseObject.m_btRaceServer = RC_ARCHERGUARD { 弓箭手 } ) then
                Continue;
              if (BaseObject.m_btRaceServer = RC_MOVE_ARCHERGUARD { 巡回弓箭手 } ) then
                Continue;

              Code := 9;
              if IsProperTarget(BaseObject) then
              begin
                Code := 10;
                nAbs := abs(m_nCurrX - BaseObject.m_nCurrX) + abs(m_nCurrY - BaseObject.m_nCurrY);
                if nAbs < nRage then
                begin
                  nRage := nAbs;
                  TargeTBaseObject := BaseObject;
                end;
              end;
            end;
          end;
        finally
          m_VisibleActors.UnLock;
        end;

        Code := 11;
        if TargeTBaseObject <> nil then
        begin
          Code := 12;
          SetTargetCreat(TargeTBaseObject);
        end
        else
        begin
          Code := 13;
          DelTargetCreat();
        end;
      end;

      Code := 14;
      if m_TargetCret <> nil then
      begin
        Code := 15;
        if tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime then
        begin
          Code := 16;
          m_dwHitTick := MyGetTickCount();
          Code := 17;
          sub_4A6B30(m_TargetCret);
        end;
      end
      else
      begin
        Code := 18;
        if (m_nDirection >= 0) and (m_btDirection <> m_nDirection) then
        begin
          Code := 19;
          TurnTo(m_nDirection);
        end;
      end;
    end;
    Code := 20;
    inherited;
  except
    MainOutMessage('[Exception] TArcherGuard:Run Error; Code=' + IntToStr(Code));
  end;
end;

{ TArcherPolice }

constructor TArcherPolice.Create; // 004A6E14
begin
  inherited;
  m_btRaceServer := 20;
end;

destructor TArcherPolice.Destroy;
begin

  inherited;
end;

{ TCastleDoor }

constructor TCastleDoor.Create; // 004A6E60
begin
  inherited;
  m_boAnimal := False;
  m_boStickMode := True;
  m_boOpened := False;
  m_btAntiPoison := 200;
end;

destructor TCastleDoor.Destroy;
begin

  inherited;
end;

procedure TCastleDoor.SetMapXYFlag(nFlag: Integer); // 004A6FB4
var
  bo06: Boolean;
begin
  m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY - 2, True);
  m_PEnvir.SetMapXYFlag(m_nCurrX + 1, m_nCurrY - 1, True);
  m_PEnvir.SetMapXYFlag(m_nCurrX + 1, m_nCurrY - 2, True);
  if nFlag = 1 then
    bo06 := False
  else
    bo06 := True;
  m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY - 1, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY - 2, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX + 1, m_nCurrY - 1, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX + 1, m_nCurrY - 2, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX - 1, m_nCurrY, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX - 2, m_nCurrY, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX - 1, m_nCurrY - 1, bo06);
  m_PEnvir.SetMapXYFlag(m_nCurrX - 1, m_nCurrY + 1, bo06);
  if nFlag = 0 then
  begin
    m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY - 2, False);
    m_PEnvir.SetMapXYFlag(m_nCurrX + 1, m_nCurrY - 1, False);
    m_PEnvir.SetMapXYFlag(m_nCurrX + 1, m_nCurrY - 2, False);
  end;
end;

procedure TCastleDoor.Open;
begin
  if m_boDeath then
    Exit;
  m_btDirection := 7;
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  m_boOpened := True;
  m_boStoneMode := True;
  SetMapXYFlag(0);
  bo2B9 := False;
end;

procedure TCastleDoor.Close;
begin
  if m_boDeath then
    Exit;
  if (m_WAbil.HP > 0) and (m_WAbil.MaxHP > 0) then
  begin
    m_btDirection := 3 - Round(m_WAbil.HP / m_WAbil.MaxHP * 3.0);
  end
  else
    m_btDirection := 3;

  if (m_btDirection - 3) >= 0 then
    m_btDirection := 0;
  SendRefMsg(RM_DIGDOWN, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  m_boOpened := False;
  m_boStoneMode := False;
  SetMapXYFlag(1);
  bo2B9 := True;
end;

procedure TCastleDoor.Die;
begin
  inherited;
  dw560 := MyGetTickCount();
  SetMapXYFlag(2);
end;

procedure TCastleDoor.Run;
var
  n08: Integer;
begin
  if m_boDeath and (m_Castle <> nil) then
    m_dwDeathTick := MyGetTickCount()
  else
    m_nHealthTick := 0;
  if not m_boOpened then
  begin
    if (m_WAbil.HP > 0) and (m_WAbil.MaxHP > 0) then
    begin
      n08 := 3 - Round(m_WAbil.HP / m_WAbil.MaxHP * 3.0);
    end
    else
      n08 := 3;
    if (m_btDirection <> n08) and (n08 < 3) then
    begin
      m_btDirection := n08;
      SendRefMsg(RM_TURN, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    end;
  end;
  inherited;
end;

procedure TCastleDoor.RefStatus; // 004A6F24
var
  n08: Integer;
begin
  if (m_WAbil.HP > 0) and (m_WAbil.MaxHP > 0) then
  begin
    n08 := 3 - Round(m_WAbil.HP / m_WAbil.MaxHP * 3.0);
  end
  else
    n08 := 3;

  if (n08 - 3) >= 0 then
    n08 := 0;
  m_btDirection := n08;
  SendRefMsg(RM_ALIVE, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
end;

procedure TCastleDoor.Initialize; // 0x004A6ECC
begin
  // m_btDirection:=0;
  inherited;
  {
    if m_WAbil.HP > 0 then begin
    if m_boOpened then begin
    SetMapXYFlag(0);
    exit;
    end;
    SetMapXYFlag(1);
    exit;
    end;
    SetMapXYFlag(2);
  }
end;

{ TWallStructure }

constructor TWallStructure.Create;
begin
  inherited;
  m_boAnimal := False;
  m_boStickMode := True;
  boSetMapFlaged := False;
  m_btAntiPoison := 200;
end;

destructor TWallStructure.Destroy;
begin

  inherited;
end;

procedure TWallStructure.Initialize;
begin
  m_btDirection := 0;
  inherited;
end;

procedure TWallStructure.RefStatus;
var
  n08: Integer;
begin
  if (m_WAbil.HP > 0) and (m_WAbil.MaxHP > 0) then
  begin
    n08 := 3 - Round(m_WAbil.HP / m_WAbil.MaxHP * 3.0);
  end
  else
  begin
    n08 := 4;
  end;
  if n08 >= 5 then
    n08 := 0;
  m_btDirection := n08;
  SendRefMsg(RM_ALIVE, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
end;

procedure TWallStructure.Die;
begin
  if m_btDirection <> 4 then
  begin
    m_btDirection := 4; // 增加死亡前发送最后的状态
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  end;
  inherited;
  dw560 := MyGetTickCount();
end;

procedure TWallStructure.Run;
var
  n08: Integer;
begin
  if m_boDeath then
  begin
    m_dwDeathTick := MyGetTickCount();
    if boSetMapFlaged then
    begin
      m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY, True);
      boSetMapFlaged := False;
    end;
  end
  else
  begin
    m_nHealthTick := 0;
    if not boSetMapFlaged then
    begin
      m_PEnvir.SetMapXYFlag(m_nCurrX, m_nCurrY, False);
      boSetMapFlaged := True;
    end;
  end;
  if (m_WAbil.HP > 0) and (m_WAbil.MaxHP > 0) then
  begin
    n08 := 3 - Round(m_WAbil.HP / m_WAbil.MaxHP * 3.0);
  end
  else
  begin
    n08 := 4;
  end;
  if (m_btDirection <> n08) and (n08 < 5) then
  begin
    m_btDirection := n08;
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  end;
  inherited;
end;

{ TIcicleMonster }

constructor TIcicleMonster.Create;
begin
  inherited;
  m_nViewRange := 8;
  m_boWantRefMsg := True;
  m_Castle := nil;
  m_nDirection := -1;
  m_boAttackType := False;
  m_nPKpoint := 0;
end;

destructor TIcicleMonster.Destroy;
begin

  inherited;
end;

procedure TIcicleMonster.Initialize;
begin
  inherited;

end;

function TIcicleMonster.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  Result := False;
  { if m_boAttackType then
    begin
    if m_LastHiter = BaseObject then Result := True;
    if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
    Result := True;
    // MainOutMessage(m_sCharName+ ' m_nPkPoint '+IntToStr2(TSmartObject(BaseObject).m_nPkPoint));
    if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and (TSmartObject(BaseObject).m_nPkPoint <= m_nPkPoint) then Result := True;
    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or (BaseObject = Self) then Result := False;
    end
    else }
  begin // 004A6A41
    if m_LastHiter = BaseObject then
      Result := True;
    if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
      (BaseObject = Self) then
      Result := False;
  end;
end;

procedure TIcicleMonster.Run;
var
  I, nAbs, nRage: Integer;
  BaseObject: TBaseObject;
  TargeTBaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  nRage := 10;
  TargeTBaseObject := nil;
  if not m_boDeath and not m_boGhost and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      m_VisibleActors.Lock;
      try
        for I := 0 to m_VisibleActors.Count - 1 do
        begin
          VisibleBaseObject := m_VisibleActors[I].Item;

          if VisibleBaseObject <> nil then
          begin
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
            if BaseObject = nil then
              Continue;
            if BaseObject.m_boDeath then
              Continue;
            if (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) then
              Continue; // 不攻击镖车
            // 怪物不攻击脱机人物 chongchong 2015-09-07
            if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
            then
            begin
              Continue;
            end;

            if IsProperTarget(BaseObject) then
            begin
              nAbs := abs(m_nCurrX - BaseObject.m_nCurrX) + abs(m_nCurrY - BaseObject.m_nCurrY);
              if nAbs < nRage then
              begin
                nRage := nAbs;
                TargeTBaseObject := BaseObject;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;

      if TargeTBaseObject <> nil then
      begin
        SetTargetCreat(TargeTBaseObject);
      end
      else
      begin
        DelTargetCreat();
      end;
    end;
    if m_TargetCret <> nil then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime then
      begin
        m_dwHitTick := MyGetTickCount();
        AttackTarget(m_TargetCret);
      end;
    end
    else
    begin
      if (m_nDirection >= 0) and (m_btDirection <> m_nDirection) then
      begin
        TurnTo(m_nDirection);
      end;
    end;
  end;
  inherited;
end;

procedure TIcicleMonster.AttackTarget(TargeTBaseObject: TBaseObject);
var
  nPower: Integer;
  // WAbil: pTAbility;

  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  if TargeTBaseObject <> nil then
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY);
    // WAbil := @m_WAbil;

    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if nPower > 0 then
    begin
      if not CanCloseDefense then // 忽视目标防御
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil)
      else
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil, 4); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37

      nPower := TargeTBaseObject.NewAbilPower(2, nPower); // 物伤减少
    end;

    nPower := GetPowerRateAdd(TargeTBaseObject, nPower);

    // 吸收伤害
    if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(TargeTBaseObject);

      // 伤害吸收百分比 2020-09-17 20:11:44
      nPower := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nPower);

      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nPower := Max(0, nPower - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;

      if (SmartObject.m_nSuckDamagePoint > 0) and (nPower > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
        if Random(100) < SmartObject.m_nSuckDamageProbability then
        begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower := Max(nPower - nSuckDamagePoint, 0);
        end;
      end;
    end;

    nPower := NewAbilPower(1, nPower); // 元素增加攻击伤害

    nPower := GetNextDamage(nPower);

    // 怪物伤害封顶 chongchong 2016-09-07
    nPower := TargeTBaseObject.GetAttackPowerMax(nPower);

    if nPower > 0 then
    begin
      TargeTBaseObject.SetLastHiter(Self);
      TargeTBaseObject.m_ExpHitter := nil;
      nPower := TargeTBaseObject.StruckDamage(nPower, Self, 0);
      TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, TargeTBaseObject.m_WAbil.HP,
        TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX),
        abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);
      if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and
        (Random(Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
      begin // 麻痹
        TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;

      nPower := TargeTBaseObject.DamageReboundPower(nPower);
      if nPower > 0 then
      begin // 反弹伤害
        nPower := StruckDamage(nPower, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject), 'FT',
          _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX), abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);
      end;
    end;
    // SendRefMsg(RM_FLYAXE, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(TargeTBaseObject), '');
    SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(TargeTBaseObject), '');
  end;
end;

{ TMoveArcherGuard }

constructor TMoveArcherGuard.Create; // 004A6AB4
begin
  inherited;
  m_nViewRange := 12;
  m_boWantRefMsg := True;
  m_Castle := nil;
  m_nDirection := -1;
  m_btRaceServer := RC_ARCHERGUARD;
  m_boAttackType := False;
  m_nPKpoint := 0; // 攻击PK值小于多少的角色

  MovePoint := nil;
  m_nTargetX := -1;
  nIdx := 0;
  nKeepCount := 0;
  nKeepMaxCount := 0;
end;

destructor TMoveArcherGuard.Destroy;
begin
  MovePoint := nil;
  inherited;
end;

procedure TMoveArcherGuard.Initialize;
begin
  inherited;
  m_boAttackType := GetArcherGuardPKMon(m_sCharName, m_nPKpoint);
  // MainOutMessage(m_sCharName+ ' m_boAttackType '+booltostr(m_boAttackType));
end;

function TMoveArcherGuard.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  Result := False;
  if m_boAttackType then
  begin
    if m_LastHiter = BaseObject then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
      Result := True;
    // MainOutMessage(m_sCharName+ ' m_nPkPoint '+IntToStr2(TSmartObject(BaseObject).m_nPkPoint));
    if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and
      (TSmartObject(BaseObject).m_nPKpoint <= m_nPKpoint) then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
      (BaseObject = Self) then
      Result := False;
  end
  else
  begin
    if m_Castle <> nil then
    begin
      if m_LastHiter = BaseObject then
        Result := True;
      if (BaseObject <> nil) and (BaseObject.bo2B0) then
      begin
        if (MyGetTickCount - BaseObject.m_dw2B4Tick) < 2 * 60 * 1000 then
        begin
          Result := True;
        end
        else
          BaseObject.bo2B0 := False;
        if BaseObject.m_Castle <> nil then
        begin
          BaseObject.bo2B0 := False;
          Result := False;
        end;
      end;
      if TUserCastle(m_Castle).m_boUnderWar then
        Result := True;
      if TUserCastle(m_Castle).m_MasterGuild <> nil then
      begin
        if (BaseObject <> nil) and (BaseObject.m_Master = nil) then
        begin
          if (TUserCastle(m_Castle).m_MasterGuild = BaseObject.m_MyGuild) or
            (TUserCastle(m_Castle).m_MasterGuild.IsAllyGuild(TGUild(BaseObject.m_MyGuild))) then
          begin
            if m_LastHiter <> BaseObject then
              Result := False;
          end;
        end
        else
        begin // 004A6988
          if (TUserCastle(m_Castle).m_MasterGuild = BaseObject.m_Master.m_MyGuild) or
            (TUserCastle(m_Castle).m_MasterGuild.IsAllyGuild(TGUild(BaseObject.m_Master.m_MyGuild))) then
          begin
            if (m_LastHiter <> BaseObject.m_Master) and (m_LastHiter <> BaseObject) then
              Result := False;
          end;
        end;
      end; // 004A69EF
      if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
        ((BaseObject.m_btRaceServer >= 10) and (BaseObject.m_btRaceServer < 50)) or (BaseObject = Self) or
        (BaseObject.m_Castle = Self.m_Castle) then
      begin
        Result := False;
      end;
      Exit;
    end; // 004A6A41
    if m_LastHiter = BaseObject then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_btRaceServer = RC_ARCHERGUARD) then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and
      (TSmartObject(BaseObject).PKLevel >= 2) then
      Result := True;
    if (BaseObject <> nil) and (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode or
      (BaseObject = Self) then
      Result := False;
  end;
end;

procedure TMoveArcherGuard.sub_4A6B30(TargeTBaseObject: TBaseObject); // 004A6B30
var
  nPower, nSuckDamagePoint: Integer;
  // WAbil: pTAbility;
  SmartObject: TSmartObject;
begin
  if TargeTBaseObject <> nil then
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY);
    // WAbil := @m_WAbil;

    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if nPower > 0 then
    begin
      if not CanCloseDefense then // 忽视目标防御
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil)
      else
        nPower := TargeTBaseObject.GetHitStruckDamage(Self, nPower, nil, 4); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37

      nPower := TargeTBaseObject.NewAbilPower(2, nPower); // 物伤减少
    end;

    nPower := NewAbilPower(1, nPower); // 元素增加攻击伤害

    nPower := GetNextDamage(nPower);

    nPower := GetPowerRateAdd(TargeTBaseObject, nPower);

    // 怪物伤害封顶 chongchong 2016-09-07
    nPower := TargeTBaseObject.GetAttackPowerMax(nPower);

    if nPower > 0 then
    begin
      // 受弓箭手攻击时，支持SetSuckDamage设置的伤害吸收 chongchong 2015-03-29
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);

        // 伤害吸收百分比 2020-09-17 20:11:44
        nPower := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nPower);

        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;

        if (SmartObject.m_nSuckDamagePoint > 0) and (nPower > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
              nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower := Max(nPower - nSuckDamagePoint, 0);
          end;
        end;
      end;

      TargeTBaseObject.SetLastHiter(Self);
      TargeTBaseObject.m_ExpHitter := nil;
      nPower := TargeTBaseObject.StruckDamage(nPower, Self, 0);
      TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, TargeTBaseObject.m_WAbil.HP,
        TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX),
        abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);
      if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and
        (Random(Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
      begin // 麻痹
        TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;

      nPower := TargeTBaseObject.DamageReboundPower(nPower);
      if nPower > 0 then
      begin // 反弹伤害
        nPower := StruckDamage(nPower, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject), 'FT',
          _MAX(abs(m_nCurrX - TargeTBaseObject.m_nCurrX), abs(m_nCurrY - TargeTBaseObject.m_nCurrY)) * 50 + 600);
      end;
    end;
    SendRefMsg(RM_FLYAXE, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(TargeTBaseObject), '');
  end;
end;

procedure TMoveArcherGuard.Run;
var
  I, nAbs, nRage: Integer;
  BaseObject: TBaseObject;
  TargeTBaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  ItemObj: TItemObject;
begin
  nRage := 9999;
  TargeTBaseObject := nil;
  if not m_boDeath and not m_boGhost and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
    begin
      // m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      m_VisibleActors.Lock;
      try
        for I := 0 to m_VisibleActors.Count - 1 do
        begin
          VisibleBaseObject := m_VisibleActors[I].Item;

          if VisibleBaseObject <> nil then
          begin
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
            if BaseObject = nil then
              Continue;
            if BaseObject.m_boDeath then
              Continue;
            if (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) then
              Continue; // 不攻击镖车
            if (BaseObject.m_btRaceServer = RC_GUARD) then
              Continue; // 不攻击卫士
            if (BaseObject.m_btRaceServer = RC_ARCHERGUARD { 弓箭手 } ) then
              Continue;
            if (BaseObject.m_btRaceServer = RC_MOVE_ARCHERGUARD { 巡回弓箭手 } ) then
              Continue;

            if IsProperTarget(BaseObject) then
            begin
              nAbs := abs(m_nCurrX - BaseObject.m_nCurrX) + abs(m_nCurrY - BaseObject.m_nCurrY);
              if nAbs < nRage then
              begin
                nRage := nAbs;
                TargeTBaseObject := BaseObject;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;

      if TargeTBaseObject <> nil then
      begin
        m_TargetCret := TargeTBaseObject;
      end
      else
      begin
        m_TargetCret := nil;
      end;
    end;

    if m_TargetCret <> nil then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime then
      begin
        m_dwHitTick := MyGetTickCount();
        sub_4A6B30(m_TargetCret);
      end;
    end
    else
    begin
      if Length(MovePoint) > 1 then
      begin
        if m_boWalkWaitLocked then
        begin
          if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
          begin
            m_boWalkWaitLocked := False;
          end;
        end;

        if g_Config.boMoveArcherGuardPickItem then
        begin
          ItemObj := m_PEnvir.GetItem(m_nCurrX, m_nCurrY);
          if (ItemObj <> nil) and (not ItemObj.m_boGhost) then
          begin
            g_MoveGuardPickItemList.Lock;
            try
              if g_MoveGuardPickItemList.IndexOf(ItemObj.m_sName) >= 0 then
              begin
                ItemObj.MakeGhost;
                SendRefMsg(RM_ITEMHIDE, 0, NativeInt(ItemObj), m_nCurrX, m_nCurrY, '');
              end;
            finally
              g_MoveGuardPickItemList.UnLock;
            end;
          end;
        end;

        if not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) then
        begin
          m_dwWalkTick := MyGetTickCount();
          m_nWalkDelay := 0;
          Inc(m_nWalkCount);
          if m_nWalkCount > m_nWalkStep then
          begin
            m_nWalkCount := 0;
            m_boWalkWaitLocked := True;
            m_dwWalkWaitTick := MyGetTickCount();
          end;
          if (m_nTargetX <> -1) then
          begin
            if ((m_nCurrX = m_nTargetX) and (m_nCurrY = m_nTargetY)) or (nKeepCount > nKeepMaxCount) then
            begin
              m_nTargetX := -1;
            end;
          end;
          if m_nTargetX = -1 then
          begin
            Inc(nIdx);
            if nIdx > High(MovePoint) then
              nIdx := 0;
            if nIdx < 0 then
              nIdx := 0;
            SetTargetXY(MovePoint[nIdx].X, MovePoint[nIdx].Y);
            nKeepMaxCount := Max(abs(m_nCurrX - m_nTargetX), abs(m_nCurrY - m_nTargetY));
            nKeepMaxCount := nKeepMaxCount + Round(nKeepMaxCount * 0.5);
            nKeepCount := 0;
          end;
          if m_nTargetX <> -1 then
          begin
            GotoTargetXY;
            Inc(nKeepCount);
          end;
        end;
      end;
    end;
  end;
  inherited;
end;

{ TExperienceMon }

constructor TExperienceMon.Create;
begin
  inherited;
  m_boAnimal := False;
  m_boSuperMan := True;
end;

destructor TExperienceMon.Destroy;
begin

  inherited;
end;

procedure TExperienceMon.GiveHitterExp(Hitter: TBaseObject; IsMagic: Boolean; Power: Integer);
begin
  if (m_dwFightExp <= 0) then
    Exit;

  if m_Abil.AC1 = 0 then
  begin
    if Hitter.m_btRaceServer = RC_PLAYOBJECT then
      TPlayObject(Hitter).GetExp(m_dwFightExp, False, False)
    else if (Hitter.m_btRaceServer = RC_HEROOBJECT) and (Hitter.m_Master <> nil) then
      TPlayObject(Hitter.m_Master).GetExp(m_dwFightExp, True, False);
  end
  else if (m_Abil.AC1 = 1) then
  begin
    if not IsMagic then
    begin
      if Hitter.m_btRaceServer = RC_PLAYOBJECT then
        TPlayObject(Hitter).GetExp(m_dwFightExp, False, False)
      else if (Hitter.m_btRaceServer = RC_HEROOBJECT) and (Hitter.m_Master <> nil) then
        TPlayObject(Hitter.m_Master).GetExp(m_dwFightExp, True, False);
    end;
  end
  else if (m_Abil.AC1 = 2) then
  begin
    if IsMagic then
    begin
      if Hitter.m_btRaceServer = RC_PLAYOBJECT then
        TPlayObject(Hitter).GetExp(m_dwFightExp, False, False)
      else if (Hitter.m_btRaceServer = RC_HEROOBJECT) and (Hitter.m_Master <> nil) then
        TPlayObject(Hitter.m_Master).GetExp(m_dwFightExp, True, False);
    end;
  end;
end;

procedure TExperienceMon.Struck(hiter: TBaseObject);
begin
  inherited;

end;

function TGuardMonster.AttackTarget(): Boolean;
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

constructor TGuardMonster.Create();
begin
  inherited;
  CanMoveMode := False;;
  // m_btRaceServer := 11;
  m_nViewRange := 7;
  m_nLight := 4;
end;

destructor TGuardMonster.Destroy;
begin

  inherited;
end;

function TGuardMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

procedure TGuardMonster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  ErrCode: Integer;
  IsCanMove: Boolean;
  nX, nY: Integer;
begin
  inherited;
  ErrCode := 0;
  if m_boDeath or m_boGhost then
    Exit;

  if m_TargetCret <> nil then
  begin
    if (abs(m_TargetCret.m_nCurrX - m_nCurrX) > 8) or (abs(m_TargetCret.m_nCurrY - m_nCurrY) > 8) or
      (m_TargetCret.m_PEnvir <> m_PEnvir) then
    begin
      DelTargetCreat;
    end;
  end;

  try
    if (m_Master <> nil) then
    begin
      // 天关宝宝不让带出地图
      if (m_PEnvir <> m_Master.m_PEnvir) and (m_PEnvir.m_boGuardianLevel) then
      begin
        MakeGhost;
        Exit;
      end
      else if (m_PEnvir <> m_Master.m_PEnvir) and (m_PEnvir.m_boMirror) then // 主人从镜像地图换到非镜像地图
      begin
        SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
        Exit;
      end;
    end;

    if (m_Master <> nil) and m_Master.m_boSlaveRelax then
      Exit;

    if m_boWalkWaitLocked then
    begin
      if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
      begin
        m_boWalkWaitLocked := False;
      end;
    end;
    IsCanMove := (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay);

    if not m_boWalkWaitLocked and CanMoveMode then
    begin
      if IsCanMove then
      begin
        m_dwWalkTick := MyGetTickCount();
        m_nWalkDelay := 0;
        Inc(m_nWalkCount);
        if m_nWalkCount > m_nWalkStep then
        begin
          m_nWalkCount := 0;
          m_boWalkWaitLocked := True;
          m_dwWalkWaitTick := MyGetTickCount();
        end; // 004A9151
      end;

      if (m_Master <> nil) and (m_Master.m_boSlaveRelax) and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave) then
      begin
        DelTargetCreat;
        m_boTarget := False;
      end;

      if IsCanMove and (m_Master <> nil) then
      begin
        // 目标超过主人一段距离，删除目标让宝宝回去 chongchong 2017-07-01
        if m_TargetCret <> nil then
        begin
          if (abs(m_TargetCret.m_nCurrX - m_Master.m_nCurrX) > 20) or (abs(m_TargetCret.m_nCurrY - m_Master.m_nCurrY) > 20) or
            (m_PEnvir <> m_Master.m_PEnvir) then
          begin
            DelTargetCreat;
          end;
        end;

        if m_TargetCret = nil then
        begin
          begin
            m_Master.GetBackPosition(nX, nY);
            if (abs(m_nTargetX - nX) > 1) or (abs(m_nTargetY - nY { nX } ) > 1) then
            begin // 004A922D
              m_nTargetX := nX;
              m_nTargetY := nY;
              if (abs(m_nCurrX - nX) <= 2) and (abs(m_nCurrY - nY) <= 2) and // 修正怪物宝宝会和人物叠一起  chongchong 2015-09-11
                (m_nCurrX <> m_Master.m_nCurrX) and (m_nCurrY <> m_Master.m_nCurrY) then
              begin
                if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
                begin
                  m_nTargetX := m_nCurrX;
                  m_nTargetY := m_nCurrY;
                end // 004A92A5
              end;
            end; // 004A92A5
          end;
        end;
        if ((m_PEnvir <> m_Master.m_PEnvir) or (abs(m_nCurrX - m_Master.m_nCurrX) > 20) or
          (abs(m_nCurrY - m_Master.m_nCurrY) > 20)) and (m_nTargetX <> -1) and (m_nTargetY <> -1) then
        begin
          SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
        end // 004A92A5 if m_TargetCret = nil then begin
      end;
      if IsCanMove then
      begin
        if (m_nTargetX <> -1) then
        begin
          GotoTargetXY(); // 004A93B5 0FFEF
        end
        else
        begin
          if m_TargetCret = nil then
            Wondering(); // FFEE   //Jacky
        end; // 004A93D8
      end;
    end;
    // if m_Master <> nil then m_Master := nil;                                                        // 不允许召唤为宝宝
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
            if (BaseObject = nil) or (BaseObject.m_boDeath) or (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) or
              (g_Config.boGuardNotAttackPlayMoster and (BaseObject.m_btRaceServer = RC_PLAYMOSTER)) then
              Continue; // 不攻击镖车
            // if (BaseObject.m_nCopyHumanLevel > 0) and (not g_Config.boAllowGuardAttack) then Continue; {不攻击分身}

            if (BaseObject.m_btRaceServer = RC_ARCHERGUARD { 弓箭手 } ) then
              Continue;
            if (BaseObject.m_btRaceServer = RC_MOVE_ARCHERGUARD { 巡回弓箭手 } ) then
              Continue;
            // if (BaseObject is TCopyMon) then Continue;
            // 增加大刀卫士不攻击宠物 2020-03-26 00:37:21
            if (BaseObject.m_boGamePet) and (g_Config.boDisableAllAttackPet or (BaseObject.m_boDisableAllAttackPet <> 0)) then
              Continue;

            ErrCode := 3;

            if IsProperTarget(BaseObject) then
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
  except
    MainOutMessage('TGuardMonster.Run Error, ErrCode = ' + IntToStr(ErrCode));
  end;
end;

end.
