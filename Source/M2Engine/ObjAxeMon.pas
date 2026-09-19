unit ObjAxeMon;

interface

uses
  Windows, Classes, Grobal2, ObjBase, ObjMon, M2Definition;

type
  TDualAxeMonster = class(TMonster)
    bo558: Boolean;
    m_nAttackCount: Integer; // 0x55C
    m_nAttackMax: Integer; // 0x560
  private
    procedure FlyAxeAttack(Target: TBaseObject);
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean; override; // FFEB
    procedure Run; override;
  end;

  TThornDarkMonster = class(TDualAxeMonster)
  private
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TArcherMonster = class(TDualAxeMonster)
  private
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

implementation

uses
  M2Share, HUtil32, Math;

{ TDualAxeMonster }

procedure TDualAxeMonster.FlyAxeAttack(Target: TBaseObject);
var
  WAbil: pTAbility;
  nDamage, nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  if m_PEnvir.CanFly(m_nCurrX, m_nCurrY, Target.m_nCurrX, Target.m_nCurrY) then
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, Target.m_nCurrX, Target.m_nCurrY);
    WAbil := @m_WAbil;
    nDamage := (Random(Max(m_WAbil.DC2 - m_WAbil.DC1, 1))) + WAbil.DC1;
    if nDamage > 0 then
    begin
      if not CanCloseDefense then // 忽视目标防御
        nDamage := Target.GetHitStruckDamage(Self, nDamage, nil);
      nDamage := Target.NewAbilPower(2, nDamage);
    end;

    if nDamage > 0 then
    begin
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害

      nDamage := GetPowerRateAdd(Target, nDamage); // 2020-09-12 23:24:45

      nDamage := GetNextDamage(nDamage);

      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := Target.GetAttackPowerMax(nDamage);

      // 受祖玛弓箭手等怪物攻击时，支持SetSuckDamage设置的伤害吸收 chongchong 2015-03-29
      if Target.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(Target);

        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);

        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;

        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nDamage);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
              nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nDamage := Max(nDamage - nSuckDamagePoint, 0);
          end;
        end;
      end;

      nDamage := Target.StruckDamage(nDamage, Self, 0);
      Target.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, Target.m_WAbil.HP, Target.m_WAbil.MaxHP, NativeInt(Self), '',
        _MAX(abs(m_nCurrX - Target.m_nCurrX), abs(m_nCurrY - Target.m_nCurrY)) * 50 + 600);

      nDamage := Target.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(Target), 'FT', 300);
      end;
    end;
    if (not Target.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(Target.m_btAntiPoison
      + m_dwParalysisRate, 0)) = 0) then
    begin // 麻痹
      Target.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
    end;
    SendRefMsg(RM_FLYAXE, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(Target), '');
  end;

end;

function TDualAxeMonster.AttackTarget: Boolean; // 00459B14
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;

  { TODO -ochongchong -c新增 : 英雄召唤的宝宝在安全区停止攻击 【2013-08-15】 }
  if m_TargetCret.InSafeZone then
    Exit;

  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 7) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 7) then
    begin
      if (m_nAttackMax - 1) > m_nAttackCount then
      begin
        Inc(m_nAttackCount);
        m_dwTargetFocusTick := MyGetTickCount();
        FlyAxeAttack(m_TargetCret);
      end
      else
      begin
        // 修复弓箭怪有时候不打 chongchong (原为Random(5));
        if Random(3) = 0 then
        begin
          m_nAttackCount := 0;
        end;
      end;
      Result := True;
      Exit;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 11) then
      begin
        SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      end;
    end
    else
    begin
      DelTargetCreat();
    end;
  end;
end;

constructor TDualAxeMonster.Create;
begin
  inherited;
  bo558 := False;
  m_nViewRange := 5;
  m_nRunTime := 250;
  m_dwSearchTime := 3000;
  m_nAttackCount := 0;
  m_nAttackMax := 2;
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 87;
end;

destructor TDualAxeMonster.Destroy;
begin

  inherited;
end;

procedure TDualAxeMonster.Run; // 00459C98
var
  I, nAbs, nRage: Integer;
  BaseObject: TBaseObject;
  TargeTBaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  nRage := 9999;
  TargeTBaseObject := nil;
  if not m_boDeath and not bo558 and not m_boGhost and (CanMove) then
  begin

    if (MyGetTickCount - m_dwSearchEnemyTick) >= 3000 then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      m_VisibleActors.Lock;
      try
        for I := 0 to m_VisibleActors.Count - 1 do
        begin
          VisibleBaseObject := m_VisibleActors[I].Item;

          if VisibleBaseObject <> nil then
          begin
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
            if BaseObject.m_boDeath then
              Continue;
            if IsProperTarget(BaseObject) then
            begin
              if not BaseObject.m_boHideMode or m_boCoolEye then
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
        end;
      finally
        m_VisibleActors.UnLock;
      end;

      if TargeTBaseObject <> nil then
      begin
        SetTargetCreat(TargeTBaseObject);
      end;
    end;

    if (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) and (m_TargetCret <> nil) then
    begin
      m_nWalkDelay := 0;
      if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 4) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 4) then
      begin
        if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 2) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 2) then
        begin
          if Random(3) = 0 then
          begin
            GetBackPosition(m_nTargetX, m_nTargetY);
          end;
        end
        else
        begin
          GetBackPosition(m_nTargetX, m_nTargetY);
        end;
      end;
    end;
  end;
  inherited;
end;

{ TThornDarkMonster }

constructor TThornDarkMonster.Create; // 00459EE4
begin
  inherited;
  m_nAttackMax := 3;
  m_btRaceServer := 93;
end;

destructor TThornDarkMonster.Destroy;
begin

  inherited;
end;

{ TArcherMonster }

constructor TArcherMonster.Create;
begin
  inherited;
  m_nAttackMax := 6;
  m_btRaceServer := 104;
end;

destructor TArcherMonster.Destroy;
begin

  inherited;
end;

end.

