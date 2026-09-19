// 自定义怪物
unit ObjCustomMon;

interface

uses
  Windows, Classes, SysUtils, Grobal2, ObjBase, GameEvent, ObjMon, uCustomMonsterUtils, M2Share, Math, ObjPlayer, M2Definition;

type
  TCustomMonster = class(TMonster)
  private
    FMonsterType: TMonsterType; // 怪物类型
    FMoveOption: TMoveOption; // 移动选项
    FProtectPt: TPoint; // 守护坐标
    FProtectRange: Integer; // 守护范围
    FAttackIndex: Integer; // 当前使用哪种攻击
    FCustomMonsterConfig: TCustomMonsterConfig;
    function CheckValidAttackIndex: Boolean;
    procedure DigUp;
    procedure DigUpAll;
    procedure Add_AdditionalsDamage(AttackConfig: PMonsterServerConfig; AttackTarget: TBaseObject);
    procedure ImprisonRange(AttackTarget: TBaseObject; nTime, nRange: Integer; TargetList: TList);
    function SingleAttack(AttackTarget: TBaseObject; AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList):
      Boolean;
    function GroupAttack(AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
    function LineAttack(Dir: Byte; AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
    function SwordWideAttack(Dir: Byte; AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
    function Dir8Attack(AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
    function Dir16Attack(AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
    procedure QuickSort(TargetList: TList; L, R: Integer);
    procedure DoPushed(TargetList: TList; Range: Integer; PushedHighLevel: Boolean);
    // procedure GetCopySelfPos(var nCurrX, nCurrY: Integer);
  public
    // 威力计算：'使用DC', '使用MC', '使用SC'
    function GetAttackPower(): Integer; override;
    function AttackTarget(): Boolean; override;
    procedure Wondering(); override;
    procedure GotoTargetXY(); override;
    procedure RunToTargetXY(); override;
    function IsProperTarget(BaseObject: TBaseObject): Boolean; override; // FFF4
  public
    constructor Create(ACustomMonsterConfig: TCustomMonsterConfig; AProtectPt: TPoint); reintroduce;
    procedure Run; override;
    property CustomMonsterConfig: TCustomMonsterConfig read FCustomMonsterConfig;
  end;

implementation

uses
  Envir{$IF MULTI_THREAD = 1}, M2Threads{$IFEND};

{ TCustomMonster }
constructor TCustomMonster.Create(ACustomMonsterConfig: TCustomMonsterConfig; AProtectPt: TPoint);
begin
  inherited Create();
  FCustomMonsterConfig := ACustomMonsterConfig;
  m_nViewRange := FCustomMonsterConfig.ServerBaseConfig.ViewRange;
  FMonsterType := FCustomMonsterConfig.ServerBaseConfig.MonsterType;
  FMoveOption := FCustomMonsterConfig.ServerBaseConfig.MoveOption;
  FProtectRange := FCustomMonsterConfig.ServerBaseConfig.ProtectRange;
  m_nLight := FCustomMonsterConfig.ServerBaseConfig.LightRange;
  FProtectPt := AProtectPt;
  if FMonsterType = mtStoneMode then
    m_boStoneMode := True
  else if FMonsterType = mtDigUP then
    m_boFixedHideMode := True;
  if m_boStoneMode then
    m_nCharStatusEx := STATE_STONE_MODE;
  FAttackIndex := -1;
  if m_nNextHitTime = 0 then
    m_nNextHitTime := 50;
  m_dwSearchTime := Random(1500) + 1000;
end;

procedure TCustomMonster.DigUp;
begin
  m_nCharStatusEx := 0;
  m_nCharStatus := GetCharStatus();
  if (FMonsterType = mtDigUP) and m_boFixedHideMode then
    m_boFixedHideMode := False;
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  if (FMonsterType = mtStoneMode) and m_boStoneMode then
    m_boStoneMode := False;
  // 修正石化怪物后，苏醒后瞬间攻击速度快 2020-03-23 21:34:16
  m_dwRunTick := MyGetTickCount + 800;
  m_dwHitTick := MyGetTickCount + 800;
end;

procedure TCustomMonster.DigUpAll;
var
  I: Integer;
  List10: TList;
  BaseObject: TBaseObject;
begin
  DigUp();
  List10 := TList.Create;
  try
    GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 7, List10);
    for I := 0 to List10.Count - 1 do
    begin
      BaseObject := TBaseObject(List10.Items[I]);
      if BaseObject <> nil then
      begin
        if BaseObject.m_boStoneMode then
        begin
          if BaseObject is TCustomMonster then
          begin
            TCustomMonster(BaseObject).DigUp;
          end;
        end;
      end;
    end; // for
  finally
    List10.Free;
  end;
end;

function TCustomMonster.GetAttackPower(): Integer;
begin
  if CheckValidAttackIndex then
  begin
    case FCustomMonsterConfig.MonsterServerConfigs[FAttackIndex].AttackPowerCalc of
      mapcMC:
        Result := GetAttackPower(m_WAbil.MC1, Max(m_WAbil.MC2 - m_WAbil.MC1, 1));
      mapcSC:
        Result := GetAttackPower(m_WAbil.SC1, Max(m_WAbil.SC2 - m_WAbil.SC1, 1));
    else
      Result := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    end;
  end
  else
    Result := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
end;

function TCustomMonster.CheckValidAttackIndex: Boolean;
begin
  Result := (FAttackIndex >= Low(FCustomMonsterConfig.MonsterServerConfigs)) and (FAttackIndex <= High(FCustomMonsterConfig.MonsterServerConfigs));
end;

function TCustomMonster.SingleAttack(AttackTarget: TBaseObject; AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList:
  TList): Boolean;
var
  nDamage, nPower, nSuckDamagePoint: Integer;
  btGetBackHP, btGetBackMP: Integer;
  Int64Value: Int64;
  SmartObject: TSmartObject;
begin
  Result := False;
  // 自定义怪物BB
  if (m_Master <> nil) and (m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
  begin
    if (AttackTarget.m_btRaceServer = RC_PLAYOBJECT) then
    begin
      if g_Config.boSlaveNotAttackHuman then
      begin
        Exit;
      end
      else if (m_PEnvir <> nil) and (m_PEnvir.m_boSlaveNotAttackHuman) then
      begin
        Exit;
      end;
    end
    else if (AttackTarget.m_btRaceServer = RC_HEROOBJECT) then
    begin
      if g_Config.boSlaveNotAttackHero then
      begin
        Exit;
      end
      else if (m_PEnvir <> nil) and (m_PEnvir.m_boSlaveNotAttackHero) then
      begin
        Exit;
      end
    end;
  end;

  if not AttackConfig.AttackIgnoreDefence then // 忽视目标防御
  begin
    if AttackConfig.AttackPowerCalc = mapcDC then
    begin
      if not CanCloseDefense then
        // 如未勾选，则看看MonSpAbilList.txt中的忽视目标防御 chongchong 2017-05-11
        nDamage := AttackTarget.GetHitStruckDamage(Self, BasePower, nil)
      else
        nDamage := AttackTarget.GetHitStruckDamage(Self, BasePower, nil, 4); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    end
    else
    begin
      if not CanCloseDefense then
        nDamage := AttackTarget.GetMagStruckDamage(Self, BasePower, nil)
      else
        nDamage := AttackTarget.GetMagStruckDamage(Self, BasePower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    end;
  end
  else
  begin
    if AttackConfig.AttackPowerCalc = mapcDC then
    begin
      // nDamage := BasePower;
      nDamage := AttackTarget.GetHitStruckDamage(Self, BasePower, nil, 4); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    end
    else
    begin
      if not CanCloseDefense then
        nDamage := AttackTarget.GetMagStruckDamage(Self, BasePower, nil)
      else
        nDamage := AttackTarget.GetMagStruckDamage(Self, BasePower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    end;
  end;

  if AttackConfig.AttackPowerCalc = mapcDC then
    nDamage := AttackTarget.NewAbilPower(2, nDamage)
  else
    nDamage := AttackTarget.NewAbilPower(3, nDamage);

  if nDamage > 0 then
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害

  nDamage := GetPowerRateAdd(AttackTarget, nDamage);
  if nDamage > 0 then
  begin
    // 吸血
    if (AttackConfig.Additionals[5].Checked) //
      and (Random(AttackConfig.Additionals[5].Rate) = 0) //
      and (AttackConfig.Additionals[5].Time > 0) then
    begin
      btGetBackHP := Round(nDamage / 100 * AttackConfig.Additionals[5].Time);
      if btGetBackHP > 0 then
      begin
        Int64Value := Int64(m_WAbil.HP) + btGetBackHP;
        if Int64Value <= m_WAbil.MaxHP then
          m_WAbil.HP := Int64Value
        else
          m_WAbil.HP := m_WAbil.MaxHP;
        Result := True;
      end;
    end;

    // 吸蓝
    if (AttackConfig.Additionals[6].Checked) //
      and (Random(AttackConfig.Additionals[6].Rate) = 0) //
      and (AttackConfig.Additionals[6].Time > 0) then
    begin
      btGetBackMP := Round(nDamage / 100 * AttackConfig.Additionals[6].Time);
      if AttackTarget.m_WAbil.MP < btGetBackMP then
        btGetBackMP := AttackTarget.m_WAbil.MP;
      if btGetBackMP > 0 then
      begin
        Int64Value := m_WAbil.MP + btGetBackMP;
        if Int64Value <= m_WAbil.MaxMP then
          m_WAbil.MP := Int64Value
        else
          m_WAbil.MP := m_WAbil.MaxMP;
        AttackTarget.m_WAbil.MP := Max(AttackTarget.m_WAbil.MP - btGetBackMP, 0);
        AttackTarget.HealthSpellChanged(Max(0, AttackConfig.AttackDelayTime - 100));
        Result := True;
      end;
    end;

    nDamage := GetNextDamage(nDamage);
    // 修正伤害吸收后，显示血量显示错误 chongchong 2018-12-09 15:44:07
    // 伤害吸收 chongchong 2016-03-18
    if AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(AttackTarget);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
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

    // 2021-04-17
    if (Random(AttackTarget.m_btSpeedPoint) >= m_btHitPoint) then
      nDamage := 0;

    if nDamage > 0 then
    begin
      // 怪物伤害封顶 XXXXXXXXXXXXXXXXXX chongchong 2016-01-30
      nDamage := AttackTarget.GetAttackPowerMax(nDamage);
      nDamage := AttackTarget.StruckDamage(nDamage, Self, FAttackIndex + 1);
    end;

    if AttackConfig.AttackPowerCalc = mapcDC then
    begin
      AttackTarget.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
        NativeInt(Self), IntToStr(FAttackIndex + 1), AttackConfig.AttackDelayTime);
    end
    else
    begin
      AttackTarget.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
        NativeInt(Self), 'MAG:' + IntToStr(MakeLong(FAttackIndex + 1, 0)), AttackConfig.AttackDelayTime);
    end;

    // TargeTBaseObject.SendDelayMsg(BaseObject, RM_MAGSTRUCK, 0, nPowerNG, 0, UserMagic.wMagIdx, '', 200);

      // AttackTarget.SendDelayMsg(AttackTarget, RM_STRUCK, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP, NativeInt(Self), '', BASE_DELAY + 200);
    { if AttackTarget.DamageRebound() then
      begin                                                   // 反弹伤害
      StruckDamage(nDamage);
      SendDelayMsg(TBaseObject(RM_STRUCK), RM_10102, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(AttackTarget), '', AttackConfig.AttackDelayTime);
      end; }
    // 反弹伤害
    nPower := AttackTarget.DamageReboundPower(nDamage);
    if nPower > 0 then
    begin
      nPower := StruckDamage(nPower, nil, FAttackIndex + 1);
      SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(AttackTarget), 'FT',
        AttackConfig.AttackDelayTime);
    end;
  end;

  if AttackTarget <> m_TargetCret then
    TargetList.Add(AttackTarget);

  Add_AdditionalsDamage(AttackConfig, AttackTarget);
end;

function TCustomMonster.GroupAttack(AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  TargetCount: Integer;
begin
  Result := False;
  BaseObjectList := TList.Create;
  try
    if (AttackConfig.AttackMode = mamNear) and (not AttackConfig.NearAttackTargetCenter) then
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, AttackConfig.AttackGroupRange, BaseObjectList)
    else
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, AttackConfig.AttackGroupRange, BaseObjectList);
    TargetCount := 0;
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
      { (not BaseObject.m_boHideMode or m_boCoolEye) and }
        IsProperTarget(BaseObject) and
      // 怪物不攻击脱机人物 chongchong 2015-09-07
        (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
        then
      begin
        Inc(TargetCount);
      end;
    end;
    if (AttackConfig.AttackPowerInc > 0) and (TargetCount > 2) then
      BasePower := BasePower + (TargetCount - 1) * AttackConfig.AttackPowerInc;
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
      { (not BaseObject.m_boHideMode or m_boCoolEye) and }
        IsProperTarget(BaseObject) and
      // 怪物不攻击脱机人物 chongchong 2015-09-07
        (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
        then
      begin
        if SingleAttack(BaseObject, AttackConfig, BasePower, TargetList) then
          Result := True;
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

function TCustomMonster.LineAttack(Dir: Byte; AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
var
  I: Integer;
  nX, nY, TargetCount: Integer;
  BaseObject: TBaseObject;
  ncX, ncY: Integer;
begin
  Result := False;
  if AttackConfig.AttackTeleportRush then
  begin
    ncX := m_nCurrX;
    ncY := m_nCurrY;
  end
  else
  begin
    if SingleAttack(m_TargetCret, AttackConfig, BasePower, TargetList) then
      Result := True;

    ncX := m_TargetCret.m_nCurrX;
    ncY := m_TargetCret.m_nCurrY;
  end;

  TargetCount := 0;
  for I := 1 to AttackConfig.AttackGroupRange - 1 do
  begin
    if m_PEnvir.GetNextPosition(ncX, ncY, Dir, I, nX, nY) then
    begin
      BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));

      if (BaseObject <> nil) //
        and (not BaseObject.m_boDeath) //
        and (not BaseObject.m_boGhost) //
        and IsProperTarget(BaseObject) //
        and (not (g_Config.boMonNoAttackOffLinePlayer // 怪物不攻击脱机人物 chongchong 2015-09-07
        and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) //
        and TPlayObject(BaseObject).m_boOffLine)) then
        Inc(TargetCount);
    end;
  end;

  if (AttackConfig.AttackPowerInc > 0) and (TargetCount > 2) then
    BasePower := BasePower + (TargetCount - 1) * AttackConfig.AttackPowerInc;

  for I := 1 to AttackConfig.AttackGroupRange - 1 do
  begin
    if m_PEnvir.GetNextPosition(ncX, ncY, Dir, I, nX, nY) then
    begin
      BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));

      if (BaseObject <> nil) //
        and (not BaseObject.m_boDeath) //
        and (not BaseObject.m_boGhost) //
        and IsProperTarget(BaseObject) // 怪物不攻击脱机人物 chongchong 2015-09-07
        and (not (g_Config.boMonNoAttackOffLinePlayer //
        and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) //
        and TPlayObject(BaseObject).m_boOffLine)) //
        and SingleAttack(BaseObject, AttackConfig, BasePower, TargetList) then
        Result := True;
    end;
  end;
end;

function TCustomMonster.SwordWideAttack(Dir: Byte; AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList):
  Boolean;
var
  NewDir, nC: Byte;
  I, nX, nY, TargetCount: Integer;
  BaseObject: TBaseObject;
begin
  nC := 0;
  Result := False;
  TargetCount := 0;
  for I := 1 to AttackConfig.AttackGroupRange do
  begin
    if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, Dir, I, nX, nY) then
    begin
      BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
      { (not BaseObject.m_boHideMode or m_boCoolEye) and }
        IsProperTarget(BaseObject) and
      // 怪物不攻击脱机人物 chongchong 2015-09-07
        (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
        then
      begin
        Inc(TargetCount);
      end;
    end;
  end;
  while (True) do
  begin
    NewDir := (Dir + g_Config.WideAttack[nC]) mod 8;
    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, NewDir, I, nX, nY) then
      begin
        BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
        { (not BaseObject.m_boHideMode or m_boCoolEye) and }
          IsProperTarget(BaseObject) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          Inc(TargetCount);
        end;
      end;
    end;
    Inc(nC);
    if nC >= 3 then
      Break;
  end;
  if (AttackConfig.AttackPowerInc > 0) and (TargetCount > 2) then
    BasePower := BasePower + (TargetCount - 1) * AttackConfig.AttackPowerInc;
  nC := 0;
  // 修复半月攻击无伤害 chongchong 2014-11-17
  for I := 1 to AttackConfig.AttackGroupRange do
  begin
    if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, Dir, I, nX, nY) then
    begin
      BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
      { (not BaseObject.m_boHideMode or m_boCoolEye) and }
        IsProperTarget(BaseObject) and
      // 怪物不攻击脱机人物 chongchong 2015-09-07
        (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
        then
      begin
        if SingleAttack(BaseObject, AttackConfig, BasePower, TargetList) then
          Result := True;
      end;
    end;
  end;
  while (True) do
  begin
    NewDir := (Dir + g_Config.WideAttack[nC]) mod 8;
    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, NewDir, I, nX, nY) then
      begin
        BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
        { (not BaseObject.m_boHideMode or m_boCoolEye) and }
          IsProperTarget(BaseObject) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          if SingleAttack(BaseObject, AttackConfig, BasePower, TargetList) then
            Result := True;
        end;
      end;
    end;
    Inc(nC);
    if nC >= 3 then
      Break;
  end;
end;

function TCustomMonster.Dir8Attack(AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
var
  I, nDir: Integer;
  nX, nY, TargetCount: Integer;
  BaseObject: TBaseObject;
begin
  Result := False;
  TargetCount := 0;
  for nDir := 0 to 7 do
  begin
    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, I, nX, nY) then
      begin
        BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));
        if (BaseObject <> nil) and IsProperTarget(BaseObject) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          Inc(TargetCount);
        end;
      end;
    end;
  end;
  if (AttackConfig.AttackPowerInc > 0) and (TargetCount > 2) then
    BasePower := BasePower + (TargetCount - 1) * AttackConfig.AttackPowerInc;
  for nDir := 0 to 7 do
  begin
    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, I, nX, nY) then
      begin
        BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));
        if (BaseObject <> nil) and IsProperTarget(BaseObject) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          if SingleAttack(BaseObject, AttackConfig, BasePower, TargetList) then
            Result := True;
        end;
      end;
    end;
  end;
end;

function TCustomMonster.Dir16Attack(AttackConfig: PMonsterServerConfig; BasePower: Integer; TargetList: TList): Boolean;
var
  I, nDir: Integer;
  nX, nY: Integer;
  BaseObject: TBaseObject;
  AttackList: TList;
  A: Extended;
begin
  Result := False;
  AttackList := TList.Create;
  try
    // 原8个方向攻击目标
    for nDir := 0 to 7 do
    begin
      for I := 1 to AttackConfig.AttackGroupRange do
      begin
        if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, I, nX, nY) then
        begin
          BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));
          if (BaseObject <> nil) and IsProperTarget(BaseObject) and
          // 怪物不攻击脱机人物 chongchong 2015-09-07
            (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
            then
          begin
            AttackList.Add(BaseObject);
            // if SingleAttack(BaseObject, AttackConfig, BasePower) then
            // Result := True;
          end;
        end;
      end;
    end;
    // 扩展的8方向 chongchong 2014-09-14
    for nDir := 0 to 7 do
    begin
      A := (nDir * 2 + 1) * 22.5 / 180 * PI;
      for I := 1 to AttackConfig.AttackGroupRange do
      begin
        nX := m_nCurrX + Trunc(I * Cos(A));
        nY := m_nCurrY + Trunc(I * Sin(A));
        BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));
        if (BaseObject <> nil) and IsProperTarget(BaseObject) and (AttackList.IndexOf(BaseObject) = -1) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          AttackList.Add(BaseObject);
        end;
      end;
    end;
    if (AttackConfig.AttackPowerInc > 0) and (AttackList.Count > 2) then
      BasePower := BasePower + (AttackList.Count - 1) * AttackConfig.AttackPowerInc;
    for I := 0 to AttackList.Count - 1 do
    begin
      BaseObject := AttackList.Items[I];
      if SingleAttack(BaseObject, AttackConfig, BasePower, TargetList) then
        Result := True;
    end;
  finally
    AttackList.Free;
  end;
end;

procedure TCustomMonster.QuickSort(TargetList: TList; L, R: Integer);

  function SCompare(Item1, Item2: TBaseObject): Integer;
  begin
    Result := (Abs(Item2.m_nCurrX - m_nCurrX) + Abs(Item2.m_nCurrY - m_nCurrY)) - (Abs(Item1.m_nCurrX - m_nCurrX) + Abs(Item1.m_nCurrY
      - m_nCurrY));
  end;

var
  I, J: Integer;
  P, T: Pointer;
begin
  repeat
    I := L;
    J := R;
    P := TargetList.Items[(L + R) shr 1];
    repeat
      while SCompare(TargetList.Items[I], P) < 0 do
        Inc(I);
      while SCompare(TargetList.Items[J], P) > 0 do
        Dec(J);
      if I <= J then
      begin
        T := TargetList.Items[I];
        TargetList.Items[I] := TargetList.Items[J];
        TargetList.Items[J] := T;
        Inc(I);
        Dec(J);
      end;
    until I > J;
    if L < J then
      QuickSort(TargetList, L, J);
    L := I;
  until I >= R;
end;

procedure TCustomMonster.DoPushed(TargetList: TList; Range: Integer; PushedHighLevel: Boolean);

  function DoPushedWith100(Player: TBaseObject; MagicID: Integer; TargetX, TargetY: Integer; Range: Integer; PushedHighLevel:
    Boolean): Boolean;

    function CanMotaebo(BaseObject: TBaseObject): Boolean;
    var
      nC: Integer;
    begin
      // 宠物无实体模式 2019-11-15 22:18:42
      if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
      begin
        Result := True;
        Exit;
      end;
      Result := False;
      if PushedHighLevel then
        Result := (not BaseObject.m_boStickMode) and Player.IsProperTarget(BaseObject) and (Player.m_Abil.Level >= BaseObject.m_Abil.Level)
      else
      begin
        if (Player.m_Abil.Level > BaseObject.m_Abil.Level) and (not BaseObject.m_boStickMode) then
        begin
          nC := Player.m_Abil.Level - BaseObject.m_Abil.Level;
          if Random(20) < ((1 * 4) + 6 + nC) then
          begin
            if Player.IsProperTarget(BaseObject) then
              Result := True;
          end;
        end;
      end;
    end;

  var
    I: Integer;
    PoseCreate: TBaseObject;
    nX, nY: Integer;
    nOldX, nOldY, nSelfStep: Integer;
    sPushedInfo: string;
    PushedObjectList: TList;
    BaseObject_30: TBaseObject;
  begin
    Result := False;
    nSelfStep := 0;
    PushedObjectList := TList.Create;
    try
      PoseCreate := Player.GetPoseCreate();
      if (PoseCreate <> nil) then
      begin
        for I := 0 to Range - 1 do
        begin // Max(2, nMagicLevel + 1)
          PoseCreate := Player.GetPoseCreate();
          if PoseCreate <> nil then
          begin
            // 追心刺修改: CanMotaebo函数中不能加几率，不然这里可以推，下面的CanMotaebo(BaseObject_30)又不能推，就会出现 穿过目标导致目标卡位 chongchong 2017-11-18
            if not CanMotaebo(PoseCreate) then
              Break;
            if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, Player.m_btDirection, 2, nX, nY) then
            begin // 推动第二格的角色
              BaseObject_30 := Player.m_PEnvir.GetMovingObject(nX, nY, True);
              if (BaseObject_30 <> nil) and CanMotaebo(BaseObject_30) then
              begin
                nOldX := BaseObject_30.m_nCurrX;
                nOldY := BaseObject_30.m_nCurrY;
                BaseObject_30.CharPushed_Skill100(Player.m_btDirection, 1);
                if (nOldX <> BaseObject_30.m_nCurrX) or (nOldY <> BaseObject_30.m_nCurrY) then
                  if PushedObjectList.IndexOf(BaseObject_30) < 0 then
                  begin
                    BaseObject_30.m_btPushedStep := 1;
                    PushedObjectList.Add(BaseObject_30);
                  end
                  else
                  begin
                    BaseObject_30.m_btPushedStep := Player.m_btPushedStep + 1;
                  end;
              end;
            end;
            nOldX := PoseCreate.m_nCurrX;
            nOldY := PoseCreate.m_nCurrY;
            if PoseCreate.CharPushed_Skill100(Player.m_btDirection, 1) <> 1 then
              Break; // 推动第一格的角色
            if (nOldX <> PoseCreate.m_nCurrX) or (nOldY <> PoseCreate.m_nCurrY) then
            begin
              if PushedObjectList.IndexOf(PoseCreate) < 0 then
              begin
                PoseCreate.m_btPushedStep := 1;
                PushedObjectList.Add(PoseCreate);
              end
              else
              begin
                PoseCreate.m_btPushedStep := PoseCreate.m_btPushedStep + 1;
              end;
            end;
            Player.GetFrontPosition(nX, nY);
            if Player.m_PEnvir.MoveToMovingObject(Player.m_nCurrX, Player.m_nCurrY, Player, nX, nY, False) then
            begin // 自己也往前走动
              Player.m_nCurrX := nX;
              Player.m_nCurrY := nY;
              Inc(nSelfStep);
              Result := True;
            end;
          end;
          // 004C32D7  if PoseCreate <> nil  then begin
        end;
        // 004C32DD for i:=0 to Max(2,nMagicLevel + 1) do begin
      end
      else
      begin
        // 004C32E8 if PoseCreate <> nil  then begin
        for I := 0 to Range - 1 do
        begin
          Player.GetFrontPosition(nX, nY); // sub_004B2790
          if Player.m_PEnvir.MoveToMovingObject(Player.m_nCurrX, Player.m_nCurrY, Player, nX, nY, False) then
          begin
            Player.m_nCurrX := nX;
            Player.m_nCurrY := nY;
            Inc(nSelfStep);
          end
          else
          begin
            if not Player.m_PEnvir.CanWalk(nX, nY, True) then
            begin
              Break;
            end;
          end;
        end;
      end;
      sPushedInfo := '';
      // for I := 0 to PushedObjectList.Count - 1 do
      // begin
      // PoseCreate := TBaseObject(PushedObjectList.Items[I]);
      // PushedObject.nRecogId := NativeInt(PoseCreate);
      // PushedObject.nCurrX := PoseCreate.m_nCurrX;
      // PushedObject.nCurrY := PoseCreate.m_nCurrY;
      // PushedObject.btDir := PoseCreate.m_btDirection;
      // PushedObject.btStep := PoseCreate.m_btPushedStep;
      // sPushedInfo := sPushedInfo + EncodeBuffer(@PushedObject, SizeOf(TPushedObject)) + '/';
      // end;
    finally
      PushedObjectList.Free;
    end;
    Player.SendRefMsg(RM_CUSTOM_PUSH, MakeLong(Player.m_btDirection, MagicID), Player.m_nCurrX, Player.m_nCurrY, nSelfStep,
      sPushedInfo);
  end;

var
  I, J: Integer;
  PushObject: TBaseObject;
  PushList: TList;
  Dir: Byte;
begin
  // DoPushedWith100(Self, 1000, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 4, True);
  // Exit;
  if Range = 0 then
    Exit;
  PushList := TList.Create;
  try
    for I := 0 to TargetList.Count - 1 do
    begin
      PushObject := TargetList.Items[I];
      if PushObject <> nil then
      begin
        // 目标不能被冲撞
        if (PushObject.m_boStickMode) or (PushObject.m_boDeath) or (PushObject.m_boGhost) then
        begin
          Break;
        end;
        // 目标等级高于自已等级，并且不支持推动高等级
        if (m_Abil.Level < PushObject.m_Abil.Level) and (not PushedHighLevel) then
        begin
          Break;
        end;
        PushList.Add(PushObject);
      end;
    end;
    if (m_TargetCret <> nil) and (not m_TargetCret.m_boStickMode) and (not m_TargetCret.m_boDeath) and (not m_TargetCret.m_boGhost)
      and (not ((m_Abil.Level < m_TargetCret.m_Abil.Level) and (not PushedHighLevel))) then
    begin
      PushList.Add(m_TargetCret);
    end;
    if PushList.Count > 0 then
    begin
      QuickSort(PushList, 0, PushList.Count - 1)
    end;
    for I := 1 to Range do
    begin
      for J := 0 to PushList.Count - 1 do
      begin
        PushObject := PushList.Items[J];
        Dir := GetNextDirection(m_nCurrX, m_nCurrY, PushObject.m_nCurrX, PushObject.m_nCurrY);
        PushObject.CharPushed(Dir, 1);
      end;
      {
        if FCustomMonsterConfig.ServerBaseConfig.MoveOption <> moNoMove then
        begin
        GetFrontPosition(nX, nY);
        if m_PEnvir.MoveToMovingObject(m_nCurrX, m_nCurrY, Self, nX, nY, False) then
        begin // 自己也往前走动
        m_nCurrX := nX;
        m_nCurrY := nY;
        SendRefMsg(RM_RUSH, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
        end;
        end;
      }
    end;
    m_dwHitTick := MyGetTickCount;
    m_dwWalkTick := MyGetTickCount;
    m_nWalkDelay := 0;
  finally
    PushList.Free;
  end;
end;

// 使用分身术时，获取分身的坐标
(*
  procedure TCustomMonster.GetCopySelfPos(var nCurrX, nCurrY: Integer);
  var
  I, II, nX, nY: Integer;
  begin
  nCurrX := m_nCurrX;
  nCurrY := m_nCurrY;
  for I := 5 downto 1 do
  begin
  if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, I, nX, nY) and
  m_PEnvir.CanWalk(nX, nY, True) then
  begin
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, nX, nY);
  nCurrX := nX;
  nCurrY := nY;
  Exit;
  end;
  end;
  for I := 5 downto 1 do
  begin
  for II := DR_UP to DR_UPLEFT do
  begin
  if II <> m_btDirection then
  begin
  if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, II, I, nX, nY) and
  m_PEnvir.CanWalk(nX, nY, True) then
  begin
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, nX, nY);
  nCurrX := nX;
  nCurrY := nY;
  Exit;
  end;
  end;
  end;
  end;
  end;
*)
function TCustomMonster.AttackTarget: Boolean;
var
  bt06, bt07: Byte;
  I, J, K, M: Integer;
  AttackConfig: PMonsterServerConfig;
  MySideCount: Integer;
  nPower: Integer;
  IsHealthSpellChanged: Boolean;
  nX, nY: Integer;
  MonObj, RemMonObj: TBaseObject;
  BaseObjectList, FrientObjectList: TList;
  MaxAttackNearRange: Integer;
  SendTargetEffect: Boolean;
  ClientAttackConfig: PClientAttackConfig;
  TargetList: TList;
  sSendMsg: AnsiString;
  boCanAttack: Boolean;
  EffectEvent: TCustomEffectEvent;
  nStartX, nStartY, nEndX, nEndY: Integer;
  IsFriend: Boolean;
  SelfKeepPlay: TSelfKeepPlay;
  KeepTime: Integer;
  S: AnsiString;
  Flag: TWalkFlagArr;
  // Magic: pTMagic;
  // TempMagic: TUserMagic;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if (m_btRaceServer = 154) or (m_btRaceServer = RC_BOX2) or FCustomMonsterConfig.ServerBaseConfig.NoAttack then
  begin
    m_TargetCret := nil;
    Exit;
  end;
  // 诱惑之光把怪物搞黄名后，怪物不再攻击 chongchong 2018-01-10
  if m_boHolySeize then
    Exit;
  // 优先尝试自定义攻击
  TargetList := TList.Create;
  try
    for I := Low(FCustomMonsterConfig.MonsterServerConfigs) to High(FCustomMonsterConfig.MonsterServerConfigs) do
    begin
      AttackConfig := @FCustomMonsterConfig.MonsterServerConfigs[I];
      if AttackConfig.AttackEnabled then
      begin
        Randomize;
        ClientAttackConfig := @FCustomMonsterConfig.ClientAttackConfigs[I];
        SendTargetEffect := ClientAttackConfig.Target_MultiPlay and ((ClientAttackConfig.Target_StartIndex >= 0) or (ClientAttackConfig.Target_StartIndex2
          >= 0)) and (ClientAttackConfig.Target_PlayCount > 0);
        TargetList.Clear;
        bt06 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        if (tick_diff(m_dwHitTick, MyGetTickCount) >= m_nNextHitTime) and (m_WAbil.HP < m_WAbil.MaxHP / 100 * AttackConfig.AttackHPPercent)
          and (AttackConfig.AttackRate > 0) and (Random(100) <= AttackConfig.AttackRate) then
        begin
          if AttackConfig.OperateMode = momAttack then
          begin
            // Magic := UserEngine.FindMagic(1000);
            // TempMagic.MagicInfo := Magic;
            //
            // TempMagic.MagicAttr := Magic.MagicAttr;
            // TempMagic.wMagIdx := Magic.wMagicId;
            //
            // TempMagic.btLevel := 0;
            // TempMagic.btNewLevel := 0;
            // TempMagic.btKey := Integer(0);
            // TempMagic.nTranPoint := 0;
            // TempMagic.boUsesItemAdd := False;
            // TempMagic.btLevel := 3;
            // m_NoTriggerAttackStruckScript := True;
            // m_TargetCret.m_NoTriggerAttackStruckScript := True;
            //

            // MagicManager.DoSpell(TSmartObject(Self), @TempMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret, False, 1);
            MySideCount := GetMapBaseObjectCount(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 1);
            if (MySideCount > AttackConfig.AttackTargetCount) then
            begin
              // 如果自定义怪物为守护状态，被推到守护区域之外，直接飞回守护坐标 chongchong 2018-06-28 00:38:19
              if (FMoveOption = moProtect) then
              begin
                if ((Abs(m_TargetCret.m_nCurrX - FProtectPt.X) > FProtectRange) or (Abs(m_TargetCret.m_nCurrY - FProtectPt.Y) >
                  FProtectRange)) and ((m_nCurrX <> FProtectPt.X) and (m_nCurrY <> FProtectPt.Y)) then
                begin
                  DelTargetCreat();
                  // SetTargetXY(FProtectPt.X, FProtectPt.Y);
                  SpaceMove(m_PEnvir.sMapName, FProtectPt.X, FProtectPt.Y, 0);
                  Exit;
                end;
              end;
              // FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange 现在不只是配置近攻距离了，也算远攻距离
              MaxAttackNearRange := Max(AttackConfig.AttackNearRange, FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange);
              if ((AttackConfig.AttackMode = mamNear) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= MaxAttackNearRange) and (Abs(m_nCurrY
                - m_TargetCret.m_nCurrY) <= MaxAttackNearRange)) or ((AttackConfig.AttackMode = mamFar) and (Abs(m_nCurrX -
                m_TargetCret.m_nCurrX) <= Max(MaxAttackNearRange, g_Config.nMagicAttackRage)) and // g_Config.nMagicAttackRage
                (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= Max(MaxAttackNearRange, g_Config.nMagicAttackRage))) then
              begin
                Result := True;
                KeepTime := ClientAttackConfig.SelfKeep_KeepTime;
                if (ClientAttackConfig.SelfKeep_File >= 0) and (ClientAttackConfig.SelfKeep_PlayCount > 0) and (KeepTime > 0) then
                begin
                  SelfKeepPlay.SelfKeep_File := ClientAttackConfig.SelfKeep_File;
                  SelfKeepPlay.SelfKeep_StartIndex := ClientAttackConfig.SelfKeep_StartIndex;
                  SelfKeepPlay.SelfKeep_PlayCount := ClientAttackConfig.SelfKeep_PlayCount;
                  SelfKeepPlay.SelfKeep_PlayTime := ClientAttackConfig.SelfKeep_PlayTime;
                  SelfKeepPlay.SelfKeep_DrawMode := ClientAttackConfig.SelfKeep_DrawMode;
                  SelfKeepPlay.SelfKeep_KeepTime := KeepTime;
                  SelfKeepPlay.SelfKeep_StartIndex2 := ClientAttackConfig.SelfKeep_StartIndex2;
                  SelfKeepPlay.SelfKeep_DrawOrder := ClientAttackConfig.SelfKeep_DrawOrder;
                  SelfKeepPlay.SelfKeep_DrawMode2 := ClientAttackConfig.SelfKeep_DrawMode2;
                  SetLength(S, SizeOf(SelfKeepPlay));
                  Move(SelfKeepPlay, S[1], Length(S));
                  m_MagicSelfPlayTick := MyGetTickCount;
                  m_MagicSelfPlay := SelfKeepPlay;
                  SendRefMsg(RM_CUSTOM_MAGIC_SELFKEEP_PLAY, Length(S), 0, 0, 0, S, 1000);
                end;
                FAttackIndex := I;
                m_btDirection := bt06;
                // m_dwHitTick := MyGetTickCount;
                nPower := GetAttackPower;
                if AttackConfig.AttackPowerRate >= 0 then
                  nPower := Round(nPower / 100 * AttackConfig.AttackPowerRate);
                IsHealthSpellChanged := False;
                if AttackConfig.AttackTarget = matSingle then
                  IsHealthSpellChanged := SingleAttack(m_TargetCret, AttackConfig, nPower, TargetList)
                else if AttackConfig.AttackTarget = matGroup then
                  IsHealthSpellChanged := GroupAttack(AttackConfig, nPower, TargetList)
                else if AttackConfig.AttackTarget = matLine then
                  IsHealthSpellChanged := LineAttack(bt06, AttackConfig, nPower, TargetList)
                else if AttackConfig.AttackTarget = matSwordWide then
                  IsHealthSpellChanged := SwordWideAttack(bt06, AttackConfig, nPower, TargetList)
                else if AttackConfig.AttackTarget = matDir8 then
                  IsHealthSpellChanged := Dir8Attack(AttackConfig, nPower, TargetList)
                else if AttackConfig.AttackTarget = matDir16 then
                  IsHealthSpellChanged := Dir16Attack(AttackConfig, nPower, TargetList);
                if (AttackConfig.Additionals[11].Checked) and (Random(AttackConfig.Additionals[11].Rate) = 0) and (AttackConfig.Additionals
                  [11].Time > 0) then
                begin
                  ImprisonRange(m_TargetCret, AttackConfig.Additionals[11].Time, AttackConfig.AdditionalImprisonRange, TargetList);
                end;
                sSendMsg := Format('%d|%d|%d|', [m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, NativeInt(m_TargetCret)]);
                if (ClientAttackConfig.Fly_StartIndex >= 0) and (ClientAttackConfig.Fly_PlayCount > 0) and ((ClientAttackConfig.Explosion_StartIndex
                  >= 0) or (ClientAttackConfig.Explosion_StartIndex2 >= 0)) and (ClientAttackConfig.Explosion_PlayCount > 0) and (ClientAttackConfig.Explosion_KeepPlay)
                  and (ClientAttackConfig.Explosion_KeepTime > 0) then
                begin
                  if (not ClientAttackConfig.Explosion_KeepMultiPlay) or (ClientAttackConfig.Explosion_KeepAttackRange = 0) then
                  begin
                    if m_PEnvir.GetEvent(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY) = nil then
                    begin
                      EffectEvent := TCustomEffectEvent.Create(Self, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, ET_CUSTOM_EFF,
                        ClientAttackConfig.Explosion_KeepTime * 1000, nPower, AttackConfig.AttackIgnoreDefence, AttackConfig.AdditionalHP0,
                        m_wAppr, I, ClientAttackConfig.Explosion_KeepAttackRange, ClientAttackConfig.Explosion_KeepAttackInterval,
                        AttackConfig.Additionals);
                      g_EventManager.AddEvent(EffectEvent);
                    end;
                  end
                  else
                  begin
                    nStartX := m_TargetCret.m_nCurrX - ClientAttackConfig.Explosion_KeepAttackRange;
                    nEndX := m_TargetCret.m_nCurrX + ClientAttackConfig.Explosion_KeepAttackRange;
                    nStartY := m_TargetCret.m_nCurrY - ClientAttackConfig.Explosion_KeepAttackRange;
                    nEndY := m_TargetCret.m_nCurrY + ClientAttackConfig.Explosion_KeepAttackRange;
                    for K := nStartX to nEndX do
                    begin
                      for M := nStartY to nEndY do
                      begin
                        if m_PEnvir.GetEvent(K, M) = nil then
                        begin
                          EffectEvent := TCustomEffectEvent.Create(Self, K, M, ET_CUSTOM_EFF, ClientAttackConfig.Explosion_KeepTime
                            * 1000, nPower, AttackConfig.AttackIgnoreDefence, AttackConfig.AdditionalHP0, m_wAppr, I, 0,
                            ClientAttackConfig.Explosion_KeepAttackInterval, AttackConfig.Additionals);
                          g_EventManager.AddEvent(EffectEvent);
                        end;
                      end;
                    end;
                  end;
                end
                // 持续播放目标效果 chongchong 2015-03-10
                else if ClientAttackConfig.Target_KeepPlay and (ClientAttackConfig.Target_KeepTime > 0) and ((ClientAttackConfig.Target_StartIndex
                  >= 0) or (ClientAttackConfig.Target_StartIndex2 >= 0)) and (ClientAttackConfig.Target_PlayCount > 0) then
                begin
                  if (not ClientAttackConfig.Target_KeepMultiPlay) or (ClientAttackConfig.Target_KeepAttackRange = 0) then
                  begin
                    if m_PEnvir.GetEvent(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY) = nil then
                    begin
                      EffectEvent := TCustomEffectEvent.Create(Self, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, ET_CUSTOM_EFF,
                        ClientAttackConfig.Target_KeepTime * 1000, nPower, AttackConfig.AttackIgnoreDefence, AttackConfig.AdditionalHP0,
                        m_wAppr, I, ClientAttackConfig.Target_KeepAttackRange, ClientAttackConfig.Target_KeepAttackInterval,
                        AttackConfig.Additionals);
                      g_EventManager.AddEvent(EffectEvent);
                    end;
                  end
                  else
                  begin
                    nStartX := m_TargetCret.m_nCurrX - ClientAttackConfig.Target_KeepAttackRange;
                    nEndX := m_TargetCret.m_nCurrX + ClientAttackConfig.Target_KeepAttackRange;
                    nStartY := m_TargetCret.m_nCurrY - ClientAttackConfig.Target_KeepAttackRange;
                    nEndY := m_TargetCret.m_nCurrY + ClientAttackConfig.Target_KeepAttackRange;
                    for K := nStartX to nEndX do
                    begin
                      for M := nStartY to nEndY do
                      begin
                        if m_PEnvir.GetEvent(K, M) = nil then
                        begin
                          EffectEvent := TCustomEffectEvent.Create(Self, K, M, ET_CUSTOM_EFF, ClientAttackConfig.Target_KeepTime *
                            1000, nPower, AttackConfig.AttackIgnoreDefence, AttackConfig.AdditionalHP0, m_wAppr, I, 0,
                            ClientAttackConfig.Target_KeepAttackInterval, AttackConfig.Additionals);
                          g_EventManager.AddEvent(EffectEvent);
                        end;
                      end;
                    end;
                  end;
                end;
                // 多目标特效 chongchong 2014-09-28
                if SendTargetEffect then
                begin
                  for J := 0 to TargetList.Count - 1 do
                  begin
                    MonObj := TargetList.Items[J];
                    sSendMsg := sSendMsg + IntToStr(NativeInt(MonObj)) + ',';
                    if ClientAttackConfig.Target_KeepPlay and (ClientAttackConfig.Target_KeepTime > 0) and ((ClientAttackConfig.Target_StartIndex
                      >= 0) or (ClientAttackConfig.Target_StartIndex2 >= 0)) and (ClientAttackConfig.Target_PlayCount > 0) then
                    begin
                      if (not ClientAttackConfig.Target_KeepMultiPlay) or (ClientAttackConfig.Target_KeepAttackRange = 0) then
                      begin
                        if m_PEnvir.GetEvent(MonObj.m_nCurrX, MonObj.m_nCurrY) = nil then
                        begin
                          EffectEvent := TCustomEffectEvent.Create(Self, MonObj.m_nCurrX, MonObj.m_nCurrY, ET_CUSTOM_EFF,
                            ClientAttackConfig.Target_KeepTime * 1000, nPower, AttackConfig.AttackIgnoreDefence, AttackConfig.AdditionalHP0,
                            m_wAppr, I, ClientAttackConfig.Target_KeepAttackRange, ClientAttackConfig.Target_KeepAttackInterval,
                            AttackConfig.Additionals);
                          g_EventManager.AddEvent(EffectEvent);
                        end;
                      end
                      else
                      begin
                        nStartX := MonObj.m_nCurrX - ClientAttackConfig.Target_KeepAttackRange;
                        nEndX := MonObj.m_nCurrX + ClientAttackConfig.Target_KeepAttackRange;
                        nStartY := MonObj.m_nCurrY - ClientAttackConfig.Target_KeepAttackRange;
                        nEndY := MonObj.m_nCurrY + ClientAttackConfig.Target_KeepAttackRange;
                        for K := nStartX to nEndX do
                        begin
                          for M := nStartY to nEndY do
                          begin
                            if m_PEnvir.GetEvent(K, M) = nil then
                            begin
                              EffectEvent := TCustomEffectEvent.Create(Self, K, M, ET_CUSTOM_EFF, ClientAttackConfig.Target_KeepTime
                                * 1000, nPower, AttackConfig.AttackIgnoreDefence, AttackConfig.AdditionalHP0, m_wAppr, I, 0,
                                ClientAttackConfig.Target_KeepAttackInterval, AttackConfig.Additionals);
                              g_EventManager.AddEvent(EffectEvent);
                            end;
                          end;
                        end;
                      end;
                    end;
                  end;
                end;
                SendRefMsg(RM_ATTACK01 + I, bt06, m_nCurrX, m_nCurrY, 0, sSendMsg);
                if AttackConfig.AttackTeleportRush then
                begin
                  // 禁锢不允许瞬移攻击 chongchong 2018-05-28
                  if (not m_boImprison) then
                  begin
                    // if AttackConfig.AttackTeleportRunHum then Flag := Flag + [wf_Hum];
                    // if AttackConfig.AttackTeleportRunMon then Flag := Flag + [wf_Mon];
                    // if AttackConfig.AttackTeleportRunNpc then Flag := Flag + [wf_Npc];
                    // if AttackConfig.AttackTeleportRunGuard then Flag := Flag + [wf_Guard];
                    // if AttackConfig.AttackTeleportWarDisHumRun then Flag := Flag + [wf_War];
                    // if AttackConfig.AttackTeleportRunObstacle then Flag := Flag + [wf_Obstacle];
                    Flag := Flag + [wf_Hum];
                    bt06 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                    if AttackConfig.AttackTeleportRush then
                      m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, bt06, AttackConfig.AttackTeleportDistance, nX, nY);
                    if MagCanMoveTarget(nX, nY, Flag, False, 12) then
                    begin
                      SendRefMsg(RM_TURN, m_btDirection, m_nCurrX, m_nCurrY, 1, '');
                      // SendRefMsg(RM_CUSTOM_MAGICMOVE, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
                    end;
                  end;
                end;
                if (AttackConfig.Additionals[4].Checked) and (Random(AttackConfig.Additionals[4].Rate) = 0) and (AttackConfig.Additionals
                  [4].Time > 0) then
                begin
                  DoPushed(TargetList, AttackConfig.Additionals[4].Time, AttackConfig.AdditionalHighLevel4);
                end;
                if AttackConfig.MoveTarget and (Random(100) < AttackConfig.MoveTargetRate) then
                begin
                  if (m_TargetCret.m_Abil.Level < m_Abil.Level) or AttackConfig.MoveTargetHighLevel then
                  begin
                    GetFrontPosition(nX, nY);
                    m_TargetCret.SpaceMove(m_TargetCret.m_PEnvir.sMapName, nX, nY, 0);
                  end;
                end;
                if IsHealthSpellChanged then
                  HealthSpellChanged;
                // 攻击时召唤怪物 chongchong 2014-08-10 22:44:31
                if AttackConfig.EnabledCallMonster and (AttackConfig.CallMonstersRate > 0) and (Random(100) <= AttackConfig.CallMonstersRate)
                  then
                begin
                  for J := Low(AttackConfig.CallMonsters) to High(AttackConfig.CallMonsters) do
                  begin
                    if (Length(AttackConfig.CallMonsters[J]) > 0) and (AttackConfig.CallMonsterNums[J] > 0) then
                    begin
                      for K := 0 to AttackConfig.CallMonsterNums[J] - 1 do
                      begin
                        GetFrontPosition(nX, nY);
                        MonObj := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, nX, nY, AttackConfig.CallMonsters[J]);
                        if MonObj <> nil then
                        begin
                          MonObj.RecalcAbilitys;
                          if MonObj.m_WAbil.HP < MonObj.m_WAbil.MaxHP then
                          begin
                            MonObj.m_WAbil.HP := Min(MonObj.m_WAbil.HP + (MonObj.m_WAbil.MaxHP - MonObj.m_WAbil.HP) div 2, MonObj.m_WAbil.MaxHP);
                          end;
                          MonObj.RefNameColor;
                        end;
                      end;
                    end;
                  end;
                end;
                m_dwHitTick := MyGetTickCount;
                if AttackConfig.AttackSelfDie then
                begin
                  m_LastHiter := nil;
                  m_ExpHitter := nil;
                  m_WAbil.HP := 0;
                  Die;
                end;
                {
                  // 攻击时创建分身 chongchong 2014-08-10 22:44:57
                  if AttackConfig.CopySelf and
                  (AttackConfig.CopySelfMaxCount > 0) and
                  (AttackConfig.CopySelfTime > 0) then
                  begin
                  sCopySelfName := m_sCharName;
                  GetCopySelfPos(nX, nY);
                  MakeCopySelf(sCopySelfName, nX, nY, AttackConfig.CopySelfMaxCount, AttackConfig.CopySelfTime);
                  end;
                }
              end
              // 超出攻击范围时，向目标靠近
              else
              begin
                if (FMoveOption = moMoveNormal) then
                begin
                  // 瞬移移动 chongchong 2014-09-06
                  if AttackConfig.AttackTeleportAttack and (AttackConfig.AttackTeleportRate > 0) and (Random(100) <= AttackConfig.AttackTeleportRate)
                    and ((Abs(m_TargetCret.m_nCurrX - m_nCurrX) >= AttackConfig.AttackTeleportTargetDistance) or (Abs(m_TargetCret.m_nCurrY
                    - m_nCurrY) >= AttackConfig.AttackTeleportTargetDistance)) then
                  begin
                    if AttackConfig.AttackTeleportDistance = 9 then
                    begin
                      bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY, 1);
                      // if AttackConfig.AttackTeleportRush then
                      // begin
                      // m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, bt07, AttackConfig.AttackTeleportDistance,
                      // nX, nY);
                      // SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      // MagCanMoveTarget(nX, nY, Flag, False, 12);
                      // end
                      // else
                      // begin
                      // 瞬移前先指定目标，不然飞过去后又往其他地方走 TMonster.Run 1087 if nMinRange <= 1 then GotoTargetXY
                      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      SpaceMove(m_PEnvir.sMapName, nX, nY, 1);
                      // end;
                    end
                    else
                    begin
                      // if AttackConfig.AttackTeleportRush then
                      // begin
                      // bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      // m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, bt07, AttackConfig.AttackTeleportDistance,
                      // nX, nY);
                      // SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      // MagCanMoveTarget(nX, nY, Flag, False, 12);
                      // end
                      // else
                      // begin
                      bt07 := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);
                      m_TargetCret.GetBackPosition(bt07, m_nCurrX, m_nCurrY, nX, nY, AttackConfig.AttackTeleportDistance);
                      // 瞬移前先指定目标，不然飞过去后又往其他地方走 TMonster.Run 1087 if nMinRange <= 1 then GotoTargetXY
                      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      SpaceMove(m_PEnvir.sMapName, nX, nY, 1);
                      // end;
                    end;
                  end
                  else
                  begin
                    if (AttackConfig.AttackMode = mamNear) and (FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange > 1) then
                    begin
                      bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY,
                        FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange);
                      SetTargetXY(nX, nY);
                    end
                    else
                      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                  end;
                end
                else if (FMoveOption = moProtect) then
                begin
                  if (Abs(m_TargetCret.m_nCurrX - FProtectPt.X) <= FProtectRange) and (Abs(m_TargetCret.m_nCurrY - FProtectPt.Y)
                    <= FProtectRange) then
                  begin
                    if (AttackConfig.AttackMode = mamNear) and (FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange > 1) then
                    begin
                      bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                      m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY,
                        FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange);
                      SetTargetXY(nX, nY);
                    end
                    else
                      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                  end
                  else
                  begin
                    DelTargetCreat();
                    SetTargetXY(FProtectPt.X, FProtectPt.Y);
                  end;
                end;
              end;
              Exit;
            end;
          end
          else // 保护模式 chongchong 2014-09-06
          begin
            SendTargetEffect := ClientAttackConfig.Target_MultiPlay and (ClientAttackConfig.Target_File >= 0) and ((ClientAttackConfig.Target_StartIndex
              >= 0) or (ClientAttackConfig.Target_StartIndex2 >= 0)) and (ClientAttackConfig.Target_PlayCount > 0);
            BaseObjectList := TList.Create;
            FrientObjectList := TList.Create;
            try
              GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, FCustomMonsterConfig.ServerBaseConfig.ViewRange, BaseObjectList);
              for J := 0 to BaseObjectList.Count - 1 do
              begin
                MonObj := TBaseObject(BaseObjectList[J]);
                if (Master <> nil) and (Master.m_btRaceServer = RC_PLAYOBJECT) then
                  IsFriend := Master.IsProperFriend(MonObj)
                else
                  IsFriend := IsProperFriend(MonObj);
                if IsFriend then
                begin
                  FrientObjectList.Add(MonObj);
                end;
              end;
              // 定位保护目标 chongchong 2014-09-07
              if ((AttackConfig.ProtectSelfRate > 0) and (Random(100) <= AttackConfig.ProtectSelfRate)) or (FrientObjectList.Count
                = 0) then
                MonObj := Self
              else
                MonObj := FrientObjectList.Items[Random(FrientObjectList.Count)];
              if MonObj <> nil then
              begin
                Result := True;
                KeepTime := ClientAttackConfig.SelfKeep_KeepTime;
                if (ClientAttackConfig.SelfKeep_File >= 0) and (ClientAttackConfig.SelfKeep_PlayCount > 0) and (KeepTime > 0) then
                begin
                  SelfKeepPlay.SelfKeep_File := ClientAttackConfig.SelfKeep_File;
                  SelfKeepPlay.SelfKeep_StartIndex := ClientAttackConfig.SelfKeep_StartIndex;
                  SelfKeepPlay.SelfKeep_PlayCount := ClientAttackConfig.SelfKeep_PlayCount;
                  SelfKeepPlay.SelfKeep_PlayTime := ClientAttackConfig.SelfKeep_PlayTime;
                  SelfKeepPlay.SelfKeep_DrawMode := ClientAttackConfig.SelfKeep_DrawMode;
                  SelfKeepPlay.SelfKeep_KeepTime := KeepTime;
                  SelfKeepPlay.SelfKeep_StartIndex2 := ClientAttackConfig.SelfKeep_StartIndex2;
                  SelfKeepPlay.SelfKeep_DrawOrder := ClientAttackConfig.SelfKeep_DrawOrder;
                  SelfKeepPlay.SelfKeep_DrawMode2 := ClientAttackConfig.SelfKeep_DrawMode2;
                  SetLength(S, SizeOf(SelfKeepPlay));
                  Move(SelfKeepPlay, S[1], Length(S));
                  m_MagicSelfPlayTick := MyGetTickCount;
                  m_MagicSelfPlay := SelfKeepPlay;
                  SendRefMsg(RM_CUSTOM_MAGIC_SELFKEEP_PLAY, Length(S), 0, 0, 0, S, 1000);
                end;
                // 客户端展现特效 chongchong 2014-09-08
                // SendRefMsg(RM_ATTACK01 + I, bt06, m_nCurrX, m_nCurrY, 0,
                // IntToStr(MonObj.m_nCurrX) + '|' + IntToStr(MonObj.m_nCurrY) + '|' + IntToStr(NativeInt(MonObj)));
                BaseObjectList.Clear;
                FrientObjectList.Clear;
                RemMonObj := MonObj;
                sSendMsg := Format('%d|%d|%d|', [RemMonObj.m_nCurrX, RemMonObj.m_nCurrY, NativeInt(RemMonObj)]);
                ;
                GetMapBaseObjects(MonObj.m_PEnvir, MonObj.m_nCurrX, MonObj.m_nCurrY, AttackConfig.ProtectTargetRange,
                  BaseObjectList);
                for J := 0 to BaseObjectList.Count - 1 do
                begin
                  MonObj := TBaseObject(BaseObjectList[J]);
                  if (Master <> nil) and (Master.m_btRaceServer = RC_PLAYOBJECT) then
                    IsFriend := Master.IsProperFriend(MonObj)
                  else
                    IsFriend := IsProperFriend(MonObj);
                  if IsFriend then
                  begin
                    // 加血 chongchong 2014-09-07
                    if AttackConfig.ProtectAddHP and (AttackConfig.ProtectAddHPRate > 0) and (Random(100) <= AttackConfig.ProtectAddHPRate)
                      and (AttackConfig.ProtectAddHPPercent > 0) then
                    begin
                      MonObj.m_WAbil.HP := Min(MonObj.m_WAbil.HP + Round(MonObj.m_WAbil.MaxHP / 100 * AttackConfig.ProtectAddHPPercent),
                        MonObj.m_WAbil.MaxHP);
                      MonObj.HealthSpellChanged;
                    end;
                    // 加防御 chongchong 2014-09-07
                    if AttackConfig.ProtectAddDefence and (AttackConfig.ProtectAddDefenceRate > 0) and (Random(100) <=
                      AttackConfig.ProtectAddDefenceRate) and (AttackConfig.ProtectAddDefenceTime > 0) then
                    begin
                      MonObj.m_dwDefenceUpAddRate := AttackConfig.ProtectAddDefencePercent;
                      MonObj.DefenceUp(AttackConfig.ProtectAddDefenceTime);
                    end;
                    // 加魔御 chongchong 2014-09-07
                    if AttackConfig.ProtectAddMagDefence and (AttackConfig.ProtectAddMagDefenceRate > 0) and (Random(100) <=
                      AttackConfig.ProtectAddMagDefenceRate) and (AttackConfig.ProtectAddMagDefenceTime > 0) then
                    begin
                      MonObj.m_dwMagDefenceUpAddRate := AttackConfig.ProtectAddMagDefencePercent;
                      MonObj.MagDefenceUp(AttackConfig.ProtectAddDefenceTime);
                    end;
                    // 加攻击伤害 chongchong 2014-09-08
                    if AttackConfig.ProtectAddDC and (AttackConfig.ProtectAddDCRate > 0) and (Random(100) <= AttackConfig.ProtectAddDCRate)
                      and (AttackConfig.ProtectAddDCPercent > 0) and (AttackConfig.ProtectAddDCTime > 0) then
                    begin
                      nPower := GetAttackPower(MonObj.m_WAbil.DC1, Max(MonObj.m_WAbil.DC2 - MonObj.m_WAbil.DC1, 1));
                      nPower := Round(nPower / 100 * AttackConfig.ProtectAddDCPercent);
                      MonObj.m_AddNewExtTicks[anet_AddDC2] := MyGetTickCount + LongWord(AttackConfig.ProtectAddDCTime) * 1000;
                      if MonObj.m_AddNewExtValues[anet_AddDC2] <> nPower then
                      begin
                        MonObj.m_AddNewExtValues[anet_AddDC2] := nPower;
                        MonObj.RecalcAbilitys;
                        if MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
                          MonObj.SendMsg(MonObj, RM_ABILITY, 0, 0, 0, 0, '');
                      end;
                    end;
                    // 加魔法伤害 chongchong 2014-09-08
                    if AttackConfig.ProtectAddMC and (AttackConfig.ProtectAddMCRate > 0) and (Random(100) <= AttackConfig.ProtectAddMCRate)
                      and (AttackConfig.ProtectAddMCPercent > 0) and (AttackConfig.ProtectAddMCTime > 0) then
                    begin
                      nPower := GetAttackPower(MonObj.m_WAbil.MC1, Max(MonObj.m_WAbil.MC2 - MonObj.m_WAbil.MC1, 1));
                      nPower := Round(nPower / 100 * AttackConfig.ProtectAddMCPercent);
                      MonObj.m_AddNewExtTicks[anet_AddMC2] := MyGetTickCount + LongWord(AttackConfig.ProtectAddMCTime) * 1000;
                      if MonObj.m_AddNewExtValues[anet_AddMC2] <> nPower then
                      begin
                        MonObj.m_AddNewExtValues[anet_AddMC2] := nPower;
                        MonObj.RecalcAbilitys;
                        if MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
                          MonObj.SendMsg(MonObj, RM_ABILITY, 0, 0, 0, 0, '');
                      end;
                    end;
                    // 加道术伤害 chongchong 2014-09-08
                    if AttackConfig.ProtectAddSC and (AttackConfig.ProtectAddSCRate > 0) and (Random(100) <= AttackConfig.ProtectAddSCRate)
                      and (AttackConfig.ProtectAddSCPercent > 0) and (AttackConfig.ProtectAddSCTime > 0) then
                    begin
                      nPower := GetAttackPower(MonObj.m_WAbil.SC1, Max(MonObj.m_WAbil.SC2 - MonObj.m_WAbil.SC1, 1));
                      nPower := Round(nPower / 100 * AttackConfig.ProtectAddSCPercent);
                      MonObj.m_AddNewExtTicks[anet_AddSC2] := MyGetTickCount + LongWord(AttackConfig.ProtectAddSCTime) * 1000;
                      if MonObj.m_AddNewExtValues[anet_AddSC2] <> nPower then
                      begin
                        MonObj.m_AddNewExtValues[anet_AddSC2] := nPower;
                        MonObj.RecalcAbilitys;
                        if MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
                          MonObj.SendMsg(MonObj, RM_ABILITY, 0, 0, 0, 0, '');
                      end;
                    end;
                    if SendTargetEffect and (MonObj <> RemMonObj) then
                      sSendMsg := sSendMsg + IntToStr(NativeInt(MonObj)) + ',';
                  end;
                end;
                m_dwHitTick := MyGetTickCount;
                // 客户端展现特效 chongchong 2014-09-08
                if sSendMsg <> '' then
                begin
                  SendRefMsg(RM_ATTACK01 + I, bt06, m_nCurrX, m_nCurrY, 0, sSendMsg);
                end;
              end;
            finally
              BaseObjectList.Free;
              FrientObjectList.Free;
            end;
            Exit;
          end;
        end;
      end;
    end;
  finally
    TargetList.Free;
  end;
  // 如果没有使用自定义攻击，则使用默认攻击
  // 修正不一定要正角度才允许攻击 chongchong 2016-07-14
  GetAttackDir(m_TargetCret, FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange, bt06);
  boCanAttack := (Abs(m_TargetCret.m_nCurrX - m_nCurrX) <= FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange) and (Abs(m_TargetCret.m_nCurrY
    - m_nCurrY) <= FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange);
  ;
  {
    // 默认攻击距离大于1时，不知道攻击了 chongchong 2016-04-17
    // 这一段要构成正角度才允许攻击
    boCanAttack := False;
    for I := 1 to FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange do
    begin
    boCanAttack := GetAttackDir(m_TargetCret, I, bt06);
    if boCanAttack then Break;
    end;
  }
  if boCanAttack then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      // MainOutMessage('攻击间隔' + IntToStr(MyGetTickCount - m_dwHitTick));
      FAttackIndex := -1;
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      AttackDir(m_TargetCret, 0, bt06, 1);
      BreakHolySeizeMode();
    end;
    Result := True;
  end
  else
  begin
    if (m_TargetCret.m_PEnvir = m_PEnvir) then
    begin
      if (FMoveOption = moMoveNormal) then
      begin
        if (AttackConfig.AttackMode = mamNear) and (FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange > 1) then
        begin
          bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
          m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY, FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange);
          SetTargetXY(nX, nY);
        end
        else
          SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      end
      else if (FMoveOption = moProtect) then
      begin
        if (Abs(m_TargetCret.m_nCurrX - FProtectPt.X) <= FProtectRange) and (Abs(m_TargetCret.m_nCurrY - FProtectPt.Y) <=
          FProtectRange) then
        begin
          if (AttackConfig.AttackMode = mamNear) and (FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange > 1) then
          begin
            bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
            m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY, FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange);
            SetTargetXY(nX, nY);
          end
          else
            SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        end
        else
        begin
          DelTargetCreat();
          SetTargetXY(FProtectPt.X, FProtectPt.Y);
        end;
      end;
    end
    else
    begin
      DelTargetCreat();
    end;
  end;
end;

procedure TCustomMonster.Add_AdditionalsDamage(AttackConfig: PMonsterServerConfig; AttackTarget: TBaseObject);
begin
  // 目标不防毒
  if (not AttackTarget.UnPosion) then
  begin
    // 中绿毒
    if (AttackConfig.Additionals[0].Checked) and (Random(AttackConfig.Additionals[0].Rate) = 0) and (AttackConfig.Additionals[0].Time
      > 0) then
    begin
      AttackTarget.MakePosion(POISON_DECHEALTH, AttackConfig.Additionals[0].Time, AttackConfig.AdditionalHP0);
    end;
    // 中红毒
    if (AttackConfig.Additionals[1].Checked) and (Random(AttackConfig.Additionals[1].Rate) = 0) and (AttackConfig.Additionals[1].Time
      > 0) then
    begin
      AttackTarget.MakePosion(POISON_DAMAGEARMOR, AttackConfig.Additionals[1].Time, 0);
    end;
  end;
  // 麻痹
  if (not AttackTarget.UnParalysis) and (AttackConfig.Additionals[2].Checked) and (Random(AttackConfig.Additionals[2].Rate) = 0)
    and (AttackConfig.Additionals[2].Time > 0) then
  begin
    AttackTarget.MakePosion(POISON_STONE, AttackConfig.Additionals[2].Time, 0);
  end;
  // 冰冻
  if (not AttackTarget.UnFrozen) and (AttackConfig.Additionals[3].Checked) and (Random(AttackConfig.Additionals[3].Rate) = 0) and
    (AttackConfig.Additionals[3].Time > 0) then
  begin
    AttackTarget.MakeFrozen(AttackConfig.Additionals[3].Time);
  end;
  // 冰封
  if ((not AttackTarget.m_boUnForeverFrozen) or (Random(100) >= AttackTarget.m_nUnForeverFrozenRate)) and (AttackConfig.Additionals
    [10].Checked) and (Random(AttackConfig.Additionals[10].Rate) = 0) and (AttackConfig.Additionals[10].Time > 0) then
  begin
    AttackTarget.OpenForeverFrozen(AttackConfig.Additionals[10].Time);
  end;
  // 蛛网
  if (not AttackTarget.UnCobwebWinding) and (AttackConfig.Additionals[7].Checked) and (Random(AttackConfig.Additionals[7].Rate) =
    0) and (AttackConfig.Additionals[7].Time > 0) then
  begin
    AttackTarget.OpenCobwebWinding(AttackConfig.Additionals[7].Time);
  end;
  // 0防御
  if (AttackConfig.Additionals[8].Checked) and (Random(AttackConfig.Additionals[8].Rate) = 0) and (AttackConfig.Additionals[8].Time
    > 0) then
  begin
    // AttackTarget.ZeroArmor(AttackConfig.Additionals[8].Time);
    AttackTarget.OpenZeroAC(AttackConfig.Additionals[8].Time);
  end;
  // 0魔御
  if (AttackConfig.Additionals[9].Checked) and (Random(AttackConfig.Additionals[9].Rate) = 0) and (AttackConfig.Additionals[9].Time
    > 0) then
  begin
    AttackTarget.OpenZeroMAC(AttackConfig.Additionals[9].Time);
  end;
end;

procedure TCustomMonster.ImprisonRange(AttackTarget: TBaseObject; nTime, nRange: Integer; TargetList: TList);
var
  I: Integer;
  MagicEvent: pTMagicEvent;
  Event: TImprisonCurtainEvent;
  Obj: TBaseObject;
  nX, nY, nMaxX, nMaxY, nMinX, nMinY: Integer;
begin
  if AttackTarget.m_boImprison then
    Exit;
  if nRange = 0 then
  begin
    if (AttackTarget <> nil) and (not AttackTarget.m_boDeath) and (not AttackTarget.m_boGhost) and (not AttackTarget.m_boUnImprison)
      then
    begin
      New(MagicEvent);
      FillChar(MagicEvent^, SizeOf(TMagicEvent), #0);
      MagicEvent.FormNPC := True;
      MagicEvent.BaseObjectList_2 := TList.Create;
      MagicEvent.dwStartTick := MyGetTickCount;
      MagicEvent.dwTime := nTime * 1000;
      MagicEvent.Events_2 := TList.Create;
      MagicEvent.Envir := m_PEnvir;
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        UserEngine.m_MagicEventList.LockW(4);
      try
{$IFEND}
        UserEngine.m_MagicEventList.Add(MagicEvent);
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UserEngine.m_MagicEventList.UnLockW;
      end;
{$IFEND}
      AttackTarget.m_dwImprisonTick := MyGetTickCount;
      AttackTarget.m_dwImprisonTime := nTime * 1000;
      AttackTarget.m_nImprisonRange := nRange;
      AttackTarget.m_nImprisonPos.X := AttackTarget.m_nCurrX;
      AttackTarget.m_nImprisonPos.Y := AttackTarget.m_nCurrY;
      AttackTarget.m_boImprison := True;
      MagicEvent.BaseObjectList_2.Add(AttackTarget);
      Event := TImprisonCurtainEvent.Create(m_PEnvir, AttackTarget.m_nCurrX - 1, AttackTarget.m_nCurrY, ET_HOLYCURTAIN, nTime *
        1000);
      g_EventManager.AddEvent(Event);
      MagicEvent.Events_2.Add(Event);
      Event := TImprisonCurtainEvent.Create(m_PEnvir, AttackTarget.m_nCurrX + 1, AttackTarget.m_nCurrY, ET_HOLYCURTAIN, nTime *
        1000);
      g_EventManager.AddEvent(Event);
      MagicEvent.Events_2.Add(Event);
      Event := TImprisonCurtainEvent.Create(m_PEnvir, AttackTarget.m_nCurrX, AttackTarget.m_nCurrY - 1, ET_HOLYCURTAIN, nTime *
        1000);
      g_EventManager.AddEvent(Event);
      MagicEvent.Events_2.Add(Event);
      Event := TImprisonCurtainEvent.Create(m_PEnvir, AttackTarget.m_nCurrX, AttackTarget.m_nCurrY + 1, ET_HOLYCURTAIN, nTime *
        1000);
      g_EventManager.AddEvent(Event);
      MagicEvent.Events_2.Add(Event);
    end;
    Exit;
  end;
  New(MagicEvent);
  FillChar(MagicEvent^, SizeOf(TMagicEvent), #0);
  MagicEvent.FormNPC := True;
  MagicEvent.BaseObjectList_2 := TList.Create;
  MagicEvent.dwStartTick := MyGetTickCount;
  MagicEvent.dwTime := nTime * 1000;
  MagicEvent.Events_2 := TList.Create;
  MagicEvent.Envir := m_PEnvir;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_MagicEventList.LockW(4);
  try
{$IFEND}
    UserEngine.m_MagicEventList.Add(MagicEvent);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_MagicEventList.UnLockW;
  end;
{$IFEND}
  if TargetList.Count > 0 then
  begin
    for I := 0 to TargetList.Count - 1 do
    begin
      Obj := TargetList.Items[I];
      if Obj = nil then
        Continue;
      if Obj.m_boDeath then
        Continue;
      if Obj.m_boGhost then
        Continue;
      if Obj.m_boUnImprison then
        Continue;
      if (Abs(Obj.m_nCurrX - AttackTarget.m_nCurrX) <= nRange) and (Abs(Obj.m_nCurrY - AttackTarget.m_nCurrY) <= nRange) then
      begin
        Obj.m_dwImprisonTick := MyGetTickCount;
        Obj.m_dwImprisonTime := nTime * 1000;
        Obj.m_nImprisonRange := nRange;
        Obj.m_nImprisonPos.X := AttackTarget.m_nCurrX;
        Obj.m_nImprisonPos.Y := AttackTarget.m_nCurrY;
        Obj.m_boImprison := True;
        MagicEvent.BaseObjectList_2.Add(Obj);
      end;
    end;
  end
  else
  begin
    AttackTarget.m_dwImprisonTick := MyGetTickCount;
    AttackTarget.m_dwImprisonTime := nTime * 1000;
    AttackTarget.m_nImprisonRange := nRange;
    AttackTarget.m_nImprisonPos.X := AttackTarget.m_nCurrX;
    AttackTarget.m_nImprisonPos.Y := AttackTarget.m_nCurrY;
    AttackTarget.m_boImprison := True;
    MagicEvent.BaseObjectList_2.Add(AttackTarget);
  end;
  nMinX := AttackTarget.m_nCurrX - nRange;
  nMaxX := AttackTarget.m_nCurrX + nRange;
  nMinY := AttackTarget.m_nCurrY - nRange;
  nMaxY := AttackTarget.m_nCurrY + nRange;
  for nX := nMinX to nMaxX do
  begin
    for nY := nMinY to nMaxY do
    begin
      if ((nX < nMaxX) and (nY = nMinY)) or ((nY < nMaxY) and (nX = nMinX)) or (nX = nMaxX) or (nY = nMaxY) then
      begin
        Event := TImprisonCurtainEvent.Create(AttackTarget.m_PEnvir, nX, nY, ET_HOLYCURTAIN, nTime * 1000);
        g_EventManager.AddEvent(Event);
        MagicEvent.Events_2.Add(Event);
      end;
    end;
  end;
end;

procedure TCustomMonster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if (m_btRaceServer = 155) and (m_Master <> nil) and (m_Master.m_PEnvir <> m_PEnvir) then
  begin
    MakeGhost;
    Exit;
  end;
  if (m_btRaceServer = RC_BOX2) then
    Exit;
  if not m_boDeath and not m_boGhost and CanMove then
  begin
    if m_boStoneMode or m_boFixedHideMode then
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
                if (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 2) and (Abs(m_nCurrY - BaseObject.m_nCurrY) <= 2) then
                begin
                  DigUpAll();
                  Break;
                end;
              end;
            end;
          end;
        end; // for
      finally
        m_VisibleActors.UnLock;
      end;
    end
    else if (not m_boAnimal) or (m_btRaceServer = 154 { 魔王岭怪物，不攻击 } ) then
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        if (m_Master = nil) or ((m_Master <> nil) and (not m_Master.m_boSlaveRelax)) then
        begin
          SearchTarget();
          m_dwSearchEnemyTick := MyGetTickCount();
        end;
      end;
    end;
  end;
  // 魔王岭怪物不攻击
  if (m_btRaceServer = 154) then
  begin
    m_TargetCret := nil;
    if m_Master <> nil then
      m_Master := nil;
    if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
    begin
      if (Length(m_nMissionPoints) > 0) and (Abs(m_nMissionPoints[Length(m_nMissionPoints) - 1].X - m_nCurrX) <= 1) and (Abs(m_nMissionPoints
        [Length(m_nMissionPoints) - 1].Y - m_nCurrY) <= 1) then
      begin
        // MainOutMessage(Format('2 m_nMissionX:%d m_nMissionY:%d m_nCurrX:%d m_nCurrY:%d m_nTargetX:%d m_nTargetY:%d', [m_nMissionX, m_nMissionY, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY]));
        m_boNoItem := True;
        MakeGhost;
        Exit;
      end;
      if m_boWalkWaitLocked then
      begin
        if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
        begin
          m_boWalkWaitLocked := False;
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
        // 004A9151
        if not m_boRunAwayMode then
        begin
          if not m_boNoAttackMode then
          begin
            m_nTargetX := -1;
            if m_boMission and (Length(m_nMissionPoints) > 0) and (m_nMissionPointIndex < Length(m_nMissionPoints)) then
            begin
              if (m_nMissionPointIndex < 0) then
                m_nMissionPointIndex := 0;
              if (Abs(m_nCurrX - m_nMissionPoints[m_nMissionPointIndex].X) <= 3) and (Abs(m_nCurrY - m_nMissionPoints[m_nMissionPointIndex].Y)
                <= 3) then
              begin
                Inc(m_nMissionPointIndex);
                if m_nMissionPointIndex >= Length(m_nMissionPoints) then
                  m_nMissionPointIndex := Length(m_nMissionPoints) - 1;
              end;
              m_nTargetX := m_nMissionPoints[m_nMissionPointIndex].X;
              m_nTargetY := m_nMissionPoints[m_nMissionPointIndex].Y;
                // MainOutMessage(Format('3 m_nMissionX:%d m_nMissionY:%d m_nCurrX:%d m_nCurrY:%d m_nTargetX:%d m_nTargetY:%d', [m_nMissionX, m_nMissionY, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY]));
            end; // 004A91D3
          end; // 004A91D3  if not bo2C0 then begin
        end
        else
        begin // 004A9344
          if (m_dwRunAwayTime > 0) and ((MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime) then
          begin
            m_boRunAwayMode := False;
            m_dwRunAwayTime := 0;
          end;
        end; // 004A937E
        if m_nTargetX <> -1 then
        begin
          GotoTargetXY(); // 004A93B5 0FFEF
        end
        else
        begin
          Wondering(); // FFEE   //Jacky
        end; // 004A93D8
      end;
      // 004A93D8  if not bo510 and (tick_diff(m_dwWalkTick, MyGetTickCount) > n4FC) then begin
    end; // 004A93D8
    Exit;
  end;
  inherited;
end;

procedure TCustomMonster.Wondering;
begin
  if FMoveOption = moMoveNormal then
    inherited;
end;

procedure TCustomMonster.GotoTargetXY;
begin
  if FMoveOption = moMoveNormal then
  begin
    {
      // ---------------- 保持最近攻击距离 chongchong 2014-09-05
      nRange := FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange;
      if nRange > 1 then
      begin
      if m_TargetCret <> nil then
      begin
      if (Abs(m_nTargetX - m_TargetCret.m_nCurrX) < nRange) or
      (Abs(m_nTargetY - m_TargetCret.m_nCurrY) < nRange) then
      begin
      bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY, nRange);
      SetTargetXY(nX, nY);
      end;
      end;
      end;
      //-------------------------------------------------------
    }
    inherited;
  end
  else if (FMoveOption = moProtect) and (Abs(m_nTargetX - FProtectPt.X) <= FProtectRange) and (Abs(m_nTargetY - FProtectPt.Y) <=
    FProtectRange) then
  begin
    {
      // ---------------- 保持最近攻击距离 chongchong 2014-09-05
      nRange := FCustomMonsterConfig.ServerBaseConfig.MinAttackNearRange;
      if nRange > 1 then
      begin
      if m_TargetCret <> nil then
      begin
      if (Abs(m_nTargetX - m_TargetCret.m_nCurrX) < nRange) or
      (Abs(m_nTargetY - m_TargetCret.m_nCurrY) < nRange) then
      begin
      bt07 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      m_TargetCret.GetBackPosition(bt07, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nX, nY, nRange);
      if (Abs(nX - FProtectPt.X) <= FProtectRange) and
      (Abs(nY - FProtectPt.Y) <= FProtectRange) then
      begin
      SetTargetXY(nX, nY);
      end;
      end;
      end;
      end;
      //------------------------------------------------------- }

    inherited;
  end
end;

procedure TCustomMonster.RunToTargetXY;
begin
  if FMoveOption = moMoveNormal then
    inherited
  else if (FMoveOption = moProtect) and (Abs(m_nTargetX - FProtectPt.X) <= FProtectRange) and (Abs(m_nTargetY - FProtectPt.Y) <=
    FProtectRange) then
    inherited;
end;

function TCustomMonster.IsProperTarget(BaseObject: TBaseObject): Boolean;
begin
  if m_btRaceServer = 155 then
  begin
    Result := False;
    if BaseObject = nil then
      Exit;
    Result := (BaseObject.m_btRaceServer = 108) or ((BaseObject.m_btRaceServer = 154) and (BaseObject.m_btRaceImg = 156)
      { 自定义怪物 - 魔王岭怪物 } );
  end
  else
  begin
    Result := inherited IsProperTarget(BaseObject);
  end;
end;

end.

