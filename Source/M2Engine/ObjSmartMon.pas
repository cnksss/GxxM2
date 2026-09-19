unit ObjSmartMon;

interface

uses
  Windows, Classes, SysUtils, StrUtils, Math, EDcode, ObjBase, Envir, Grobal2, UnitPath, SDK, ObjPlayer, WideStrUtils, M2Threads,
  M2Definition, System.Types;

type
  // 人形怪
  THumMon = class(TSmartObject)
  private
    m_nDieDropUseItemRate: Integer;
    m_boRunWithAttack: Boolean;
    m_nRunWithAttackRate: Integer;
    m_boButchUseItem: Boolean; // 挖取身上物品
    m_nButchUseItemRate: Integer; // 挖取身上物品几率
    m_boButchListItem: Boolean; // 挖取列表物品
    m_boButchItemTrigger: Boolean; // 挖取触发
    m_nButchChargeMode: Integer; // 挖取收费模式
    m_nButchChargeCount: Integer; // 挖取收费值
    m_boOnlyButchItemDelGold: Boolean; // 仅得到物品时收费
    m_ButchItemList: TList;
    m_dwThinkTick: LongWord;
    // function MovePoint(Range: Word): TPath;
    function LoadMonitems(FileName: string): Integer;
  protected
    function ActThink(wMagicID: Word): Boolean; override;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Initialize; override;
    function CheckRestrictRange: Boolean;
    function CheckTargetRestrictRange: Boolean;
    procedure ScatterBagItems(ItemOfCreat: TBaseObject; KillMe: TBaseObject); override;
    procedure DropUseItems(BaseObject: TBaseObject); override;
    procedure RecalcAbilitys; override;
    procedure RecalcLevelAbilitys(IsSysDef: Boolean); override;
    procedure Struck(hiter: TBaseObject); override;
    procedure SetTargetCreat(BaseObject: TBaseObject); override; // FFF2
    function Walk(nIdent: Integer): Boolean; override;
    procedure GotoTargetXY(); override;
    procedure RunToTargetXY; override;
    function RunToNext(nX, nY: Integer): Boolean; override;
    function WalkToNext(nX, nY: Integer): Boolean; override;
    procedure Wondering(); override;
    function GotoNext(): Boolean; overload; override;
    function GotoNext(Path: TPath): Boolean; overload; override;
    function GotoNext(nX, nY: Integer; boRun: Boolean): Boolean; overload; override;
    function AllowUseMagic(wMagIdx: Word; ShowLowMPHit: Boolean = False): Boolean; override;
    function TakeItemsToPlayer(Player: TPlayObject): Boolean;
  end;

  TCopyMon = class(TSmartObject)
  private
    FCreateTick: LongWord;
  public
    m_nInheritedPercent: Integer;
    m_boAliveRebelled: Boolean; // 叛变后还活着
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Copy(Source: TBaseObject);
    procedure ScatterBagItems(ItemOfCreat: TBaseObject; KillMe: TBaseObject); override;
    procedure DropUseItems(BaseObject: TBaseObject); override;
    procedure Struck(hiter: TBaseObject); override;
    function ActThink(wMagicID: Word): Boolean; override;
    function RunToNext(nX, nY: Integer): Boolean; override;
    function AllowUseMagic(wMagIdx: Word; ShowLowMPHit: Boolean = False): Boolean; override;
    procedure Die(); override;
    procedure RecalcAbilitys(); override; // FFF7
    procedure RecalcAbilitys_Add();
    procedure RecalcLevelAbilitys(IsSysDef: Boolean); override;
    procedure MakeGhost; override;
    property CreateTick: LongWord read FCreateTick;
  end;

  TMoonObjectEx = class(TAnimalObject)
    // 修复之后月灵类 -- piaoyun 2013-6-23
    m_nAttackRange: Integer;
    m_dwThinkTick: LongWord; // 0x550
    m_boDupMode: Boolean; // 0x555
    m_nWalkSpeedDB: Integer;
  private
    FAvoidTargetDir: Byte;
    FFAvoidTargetTick: LongWord;
    FIsAvoidTargetFirst: Boolean;
    FDelTargetSetTick: LongWord;
    procedure HighAttack();
    procedure LowAttack();
    function Think(): Boolean; // 004A8E54
    procedure FlyAttack(Target: TBaseObject);
  public
    procedure RecalcAbilitys(); override;
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    procedure Initialize; override;
    procedure SearchTarget(); override;
    // function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    function AttackTarget: Boolean;
  end;

implementation

uses
  M2Share, HUtil32, ObjNpc, IdSrvClient, ObjHero;
{ ------------------------------------------------------------------------------ }

constructor THumMon.Create();
begin
  inherited;
  m_btRaceServer := RC_PLAYMOSTER;
  m_nDieDropUseItemRate := 30;
  m_boButch := False;
  m_nBodyLeathery := 200;
  m_nViewRange := 8;
  m_boButchUseItem := False; // 挖取身上物品
  m_nButchUseItemRate := 10; // 挖取身上物品几率
  m_boButchListItem := False; // 挖取列表物品
  m_boButchItemTrigger := False; // 挖取触发
  m_nButchChargeMode := 0; // 挖取收费模式
  m_nButchChargeCount := 0; // 挖取收费值
  m_boOnlyButchItemDelGold := True;
  m_ButchItemList := TList.Create;
  m_dwThinkTick := MyGetTickCount;
end;

destructor THumMon.Destroy;
var
  I: Integer;
begin
  if m_ButchItemList <> nil then
  begin
    for I := 0 to m_ButchItemList.Count - 1 do
      Dispose(pTUserItem(m_ButchItemList.Items[I]));
  end;
  m_ButchItemList.Free;
  inherited;
end;

procedure THumMon.Struck(hiter: TBaseObject);

  function CanSetTarget: Boolean;
  begin
    Result := IsProperTarget(hiter);
    {
      if (m_TargetCret = nil) then
      begin
      Exit;
      end;
      if (m_TargetCret <> nil) and Result then
      begin
      Result := ((abs(m_nCurrX - m_TargetCret.m_nCurrX) + abs(m_nCurrY - m_TargetCret.m_nCurrY)) > (abs(m_nCurrX - hiter.m_nCurrX) + abs(m_nCurrY - hiter.m_nCurrY))) or
      ((m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_TargetCret.m_nCurrX) + abs(m_Master.m_nCurrY - m_TargetCret.m_nCurrY)) > (abs(m_Master.m_nCurrX - hiter.m_nCurrX) + abs(m_Master.m_nCurrY - hiter.m_nCurrY))));
      end;
    }
  end;

begin
  m_dwStruckTick := MyGetTickCount;
  if hiter <> nil then
  begin
    if ((m_Master = nil) or ((m_Master <> nil) and (not m_Master.m_boSlaveRelax))) and CanSetTarget then
    begin
      // 人形怪攻击主人的时候，不要扯蛋去攻击宝宝 chongchong 2016-01-23

      // if not ((m_TargetCret <> nil) and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) and (hiter.m_Master = m_TargetCret)) then   // 修人形怪不打英雄 2019-09-08 00:23:52
      SetTargetCreat(hiter);
    end;
  end;
  if m_boAnimal then
  begin
    m_nMeatQuality := m_nMeatQuality - Random(300);
    if m_nMeatQuality < 0 then
      m_nMeatQuality := 0;
  end;
  m_dwHitTick := m_dwHitTick + LongWord(150 - Min(130, m_Abil.Level * 4));
end;

procedure THumMon.SetTargetCreat(BaseObject: TBaseObject);
begin
  if m_boNoAttackMode then
    Exit;
  inherited;
end;

procedure THumMon.RecalcAbilitys;
var
  MonsterConfig: pTPlayMonsterConfig;
begin
  inherited;
  MonsterConfig := GetPlayMonsterConfig(m_sCharName);
  if MonsterConfig <> nil then
  begin
    {
      if (not m_boNoDropItem) and (not MonsterConfig.DropItem) then
      m_boNoDropItem := True;
    }
    if (not m_boNoDropUseItem) and (not MonsterConfig.boDieDropUseItem) then
      m_boNoDropUseItem := True;
    // 禁止爆包裹物品 chongchong 2015-01-13
    if not MonsterConfig.boDieDropBagItem then
      m_boNoDropItem := True;
    m_boButch := MonsterConfig.boButchUseItem or MonsterConfig.boButchListItem;
  end;

  // 修正人形怪不算宝宝叠加属性 2021-04-06
  (*
    if m_Master = nil then
    begin
    if m_boChangeAbility then
    begin
    if m_ChangeAbility.MaxHP <> 0 then
    begin
    if m_ChangeAbility.boMaxHPPercentage then
    Int64Value := Round(m_WAbil.MaxHP + m_WAbil.MaxHP / 100 * Integer(m_ChangeAbility.MaxHP))
    else
    Int64Value := Int64(m_WAbil.MaxHP) + Integer(m_ChangeAbility.MaxHP);
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MaxHP) then
    Int64Value := High(m_WAbil.MaxHP);
    m_WAbil.MaxHP := Int64Value;
    end;
    if m_ChangeAbility.MaxMP <> 0 then
    begin
    if m_ChangeAbility.boMaxMPPercentage then
    Int64Value := Round(m_WAbil.MaxMP + m_WAbil.MaxMP / 100 * Integer(m_ChangeAbility.MaxMP))
    else
    Int64Value := Int64(m_WAbil.MaxMP) + Integer(m_ChangeAbility.MaxMP);
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MaxMP) then
    Int64Value := High(m_WAbil.MaxMP);
    m_WAbil.MaxMP := Int64Value;
    end;
    if m_ChangeAbility.AC1 <> 0 then
    begin
    if m_ChangeAbility.boAC1Percentage then
    Int64Value := Round(m_WAbil.AC1 + m_WAbil.AC1 / 100 * m_ChangeAbility.AC1)
    else
    Int64Value := m_WAbil.AC1 + m_ChangeAbility.AC1;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.AC1) then
    Int64Value := High(m_WAbil.AC1);
    m_WAbil.AC1 := Int64Value;
    end;
    if m_ChangeAbility.AC2 <> 0 then
    begin
    if m_ChangeAbility.boAC2Percentage then
    Int64Value := Round(m_WAbil.AC2 + m_WAbil.AC2 / 100 * m_ChangeAbility.AC2)
    else
    Int64Value := m_WAbil.AC2 + m_ChangeAbility.AC2;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.AC2) then
    Int64Value := High(m_WAbil.AC2);
    m_WAbil.AC2 := Int64Value;
    end;
    if m_ChangeAbility.MAC1 <> 0 then
    begin
    if m_ChangeAbility.boMAC1Percentage then
    Int64Value := Round(m_WAbil.MAC1 + m_WAbil.MAC1 / 100 * m_ChangeAbility.MAC1)
    else
    Int64Value := m_WAbil.MAC1 + m_ChangeAbility.MAC1;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MAC1) then
    Int64Value := High(m_WAbil.MAC1);
    m_WAbil.MAC1 := Int64Value;
    end;
    if m_ChangeAbility.MAC2 <> 0 then
    begin
    if m_ChangeAbility.boMAC2Percentage then
    Int64Value := Round(m_WAbil.MAC2 + m_WAbil.MAC2 / 100 * m_ChangeAbility.MAC2)
    else
    Int64Value := m_WAbil.MAC2 + m_ChangeAbility.MAC2;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MAC2) then
    Int64Value := High(m_WAbil.MAC2);
    m_WAbil.MAC2 := Int64Value;
    end;
    if m_ChangeAbility.DC1 <> 0 then
    begin
    if m_ChangeAbility.boDC1Percentage then
    Int64Value := Round(m_WAbil.DC1 + m_WAbil.DC1 / 100 * m_ChangeAbility.DC1)
    else
    Int64Value := m_WAbil.DC1 + m_ChangeAbility.DC1;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.DC1) then
    Int64Value := High(m_WAbil.DC1);
    m_WAbil.DC1 := Int64Value;
    end;
    if m_ChangeAbility.DC2 <> 0 then
    begin
    if m_ChangeAbility.boDC2Percentage then
    Int64Value := Round(m_WAbil.DC2 + m_WAbil.DC2 / 100 * m_ChangeAbility.DC2)
    else
    Int64Value := m_WAbil.DC2 + m_ChangeAbility.DC2;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.DC2) then
    Int64Value := High(m_WAbil.DC2);
    m_WAbil.DC2 := Int64Value;
    end;
    if m_ChangeAbility.MC1 <> 0 then
    begin
    if m_ChangeAbility.boMC1Percentage then
    Int64Value := Round(m_WAbil.MC1 + m_WAbil.MC1 / 100 * m_ChangeAbility.MC1)
    else
    Int64Value := m_WAbil.MC1 + m_ChangeAbility.MC1;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MC1) then
    Int64Value := High(m_WAbil.MC1);
    m_WAbil.MC1 := Int64Value;
    end;
    if m_ChangeAbility.MC2 <> 0 then
    begin
    if m_ChangeAbility.boMC2Percentage then
    Int64Value := Round(m_WAbil.MC2 + m_WAbil.MC2 / 100 * m_ChangeAbility.MC2)
    else
    Int64Value := m_WAbil.MC2 + m_ChangeAbility.MC2;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MC2) then
    Int64Value := High(m_WAbil.MC2);
    m_WAbil.MC2 := Int64Value;
    end;
    if m_ChangeAbility.SC1 <> 0 then
    begin
    if m_ChangeAbility.boSC1Percentage then
    Int64Value := Round(m_WAbil.SC1 + m_WAbil.SC1 / 100 * m_ChangeAbility.SC1)
    else
    Int64Value := m_WAbil.SC1 + m_ChangeAbility.SC1;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.SC1) then
    Int64Value := High(m_WAbil.SC1);
    m_WAbil.SC1 := Int64Value;
    end;
    if m_ChangeAbility.SC2 <> 0 then
    begin
    if m_ChangeAbility.boSC2Percentage then
    Int64Value := Round(m_WAbil.SC2 + m_WAbil.SC2 / 100 * m_ChangeAbility.SC2)
    else
    Int64Value := m_WAbil.SC2 + m_ChangeAbility.SC2;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.SC2) then
    Int64Value := High(m_WAbil.SC2);
    m_WAbil.SC2 := Int64Value;
    end;
    if m_ChangeAbility.WalkSpeed <> 0 then
    begin
    if m_ChangeAbility.boWalkSpeedPercentage then
    Int64Value := Round(m_nInitWalkSpeed + m_nWalkSpeed / 100 * m_ChangeAbility.WalkSpeed)
    else
    Int64Value := m_nInitWalkSpeed + m_ChangeAbility.WalkSpeed;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_nWalkSpeed) then
    Int64Value := High(m_nWalkSpeed);
    m_nWalkSpeed := Int64Value;
    end
    else
    begin
    m_nWalkSpeed := m_nInitWalkSpeed;
    end;
    if m_ChangeAbility.NextHitTime <> 0 then
    begin
    if m_ChangeAbility.boNextHitTimePercentage then
    Int64Value := Round(m_nInitNextHitTime + m_nNextHitTime / 100 * m_ChangeAbility.NextHitTime)
    else
    Int64Value := m_nInitNextHitTime + m_ChangeAbility.NextHitTime;
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_nNextHitTime) then
    Int64Value := High(m_nNextHitTime);
    m_nNextHitTime := Int64Value;
    end
    else
    begin
    m_nNextHitTime := m_nInitNextHitTime;
    end;
    if m_boChangeAbilitySetHMP then
    begin
    if m_ChangeAbility.HP <> 0 then
    begin
    if m_ChangeAbility.boHPPercentage then
    Int64Value := Round(m_WAbil.HP + m_WAbil.HP / 100 * Integer(m_ChangeAbility.HP))
    else
    Int64Value := Int64(m_WAbil.HP) + Integer(m_ChangeAbility.HP);
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.HP) then
    Int64Value := High(m_WAbil.HP);
    m_WAbil.HP := Int64Value;
    if m_WAbil.HP >= m_WAbil.MaxHP then m_WAbil.HP := m_WAbil.MaxHP;
    end;
    if m_ChangeAbility.MP <> 0 then
    begin
    if m_ChangeAbility.boMPPercentage then
    Int64Value := Round(m_WAbil.MP + m_WAbil.MP / 100 * Integer(m_ChangeAbility.MP))
    else
    Int64Value := Int64(m_WAbil.MP) + Integer(m_ChangeAbility.MP);
    if Int64Value < 0 then
    Int64Value := 0
    else if Int64Value > High(m_WAbil.MP) then
    Int64Value := High(m_WAbil.MP);
    m_WAbil.MP := Int64Value;
    if m_WAbil.MP >= m_WAbil.MaxHP then m_WAbil.MP := m_WAbil.MaxMP;
    end;
    end;
    end;
    end
    else
    begin
    if not g_Config.boSlaveLevelupUseNewAttr then
    begin
    n8 := m_Abil.MaxHP;
    if m_boChangeAbility and (m_ChangeAbility.DC1 > 0) then
    m_WAbil.DC1 := m_ChangeAbility.DC1
    else
    m_WAbil.DC1 := m_WAbil.DC1;
    if m_boChangeAbility and (m_ChangeAbility.DC2 > 0) then
    begin
    Int64Value := m_ChangeAbility.DC2 + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * 2;
    m_WAbil.DC2 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    m_WAbil.DC2 := Round(m_btSlaveExpLevel * 2 + m_WAbil.DC2);
    end;
    if m_boChangeAbility and (m_ChangeAbility.MaxHP > 0) then
    begin
    Int64Value := Int64(m_ChangeAbility.MaxHP) + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * 60;
    m_WAbil.MaxHP := Min(High(LongWord), Int64Value);
    end
    else
    begin
    n8 := n8 + Round(m_Abil.MaxHP * 0.15) * m_btSlaveExpLevel;
    m_WAbil.MaxHP := Min(Round(m_Abil.MaxHP + m_btSlaveExpLevel * 60), n8);
    end;
    if m_boChangeAbility then
    begin
    if m_ChangeAbility.AC1 > 0 then m_WAbil.AC1 := m_ChangeAbility.AC1;
    if m_ChangeAbility.AC2 > 0 then m_WAbil.AC2 := m_ChangeAbility.AC2;
    if m_ChangeAbility.MAC1 > 0 then m_WAbil.MAC1 := m_ChangeAbility.MAC1;
    if m_ChangeAbility.MAC2 > 0 then m_WAbil.MAC2 := m_ChangeAbility.MAC2;
    if m_ChangeAbility.WalkSpeed > 0 then m_nWalkSpeed := m_ChangeAbility.WalkSpeed;
    if m_ChangeAbility.NextHitTime > 0 then m_nNextHitTime := m_ChangeAbility.NextHitTime;
    end;
    end
    else
    begin
    m_WAbil.DC1 := m_WAbil.DC1;
    if g_Config.boSlaveLevelupAddLowerAttr then
    begin
    if m_boChangeAbility and (m_ChangeAbility.DC1 > 0) then
    begin
    Int64Value := m_ChangeAbility.DC1 + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9DC;
    m_WAbil.DC1 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := m_WAbil.DC1 + m_btSlaveExpLevel * g_Config.nSlave9DC;
    m_WAbil.DC1 := Min(High(Integer), Int64Value);
    end;
    if m_boChangeAbility and (m_ChangeAbility.AC1 > 0) then
    begin
    Int64Value := m_ChangeAbility.AC1 + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9AC;
    m_WAbil.AC1 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := m_Abil.AC1 + m_btSlaveExpLevel * g_Config.nSlave9AC;
    m_WAbil.AC1 := Min(High(Integer), Int64Value);
    end;
    if m_boChangeAbility and (m_ChangeAbility.MAC1 > 0) then
    begin
    Int64Value := m_ChangeAbility.MAC1 + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9MAC;
    m_WAbil.MAC1 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := m_Abil.MAC1 + m_btSlaveExpLevel * g_Config.nSlave9MAC;
    m_WAbil.MAC1 := Min(High(Integer), Int64Value);
    end;
    end
    else if m_boChangeAbility then
    begin
    if m_ChangeAbility.AC1 > 0 then m_WAbil.AC1 := m_ChangeAbility.AC1;
    if m_ChangeAbility.MAC1 > 0 then m_WAbil.MAC1 := m_ChangeAbility.MAC1;
    if m_ChangeAbility.DC1 > 0 then m_WAbil.DC1 := m_ChangeAbility.DC1;
    end;
    if m_boChangeAbility and (m_ChangeAbility.DC2 > 0) then
    begin
    Int64Value := m_ChangeAbility.DC2 + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9DC;
    m_WAbil.DC2 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := m_WAbil.DC2 + m_btSlaveExpLevel * g_Config.nSlave9DC;
    m_WAbil.DC2 := Min(High(Integer), Int64Value);
    end;
    if m_boChangeAbility and (m_ChangeAbility.MaxHP > 0) then
    begin
    Int64Value := Int64(m_ChangeAbility.MaxHP) + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9HP;
    m_WAbil.MaxHP := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := Int64(m_Abil.MaxHP) + m_btSlaveExpLevel * g_Config.nSlave9HP;
    m_WAbil.MaxHP := Min(High(LongWord), Int64Value);
    end;
    if m_boChangeAbility and (m_ChangeAbility.AC2 > 0) then
    begin
    Int64Value := Int64(m_ChangeAbility.AC2) + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9AC;
    m_WAbil.AC2 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := Int64(m_Abil.AC2) + m_btSlaveExpLevel * g_Config.nSlave9AC;
    m_WAbil.AC2 := Min(High(Integer), Int64Value);
    end;
    if m_boChangeAbility and (m_ChangeAbility.MAC2 > 0) then
    begin
    Int64Value := Int64(m_ChangeAbility.MAC2) + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9MAC;
    m_WAbil.MAC2 := Min(High(LongWord), Int64Value);
    end
    else
    begin
    Int64Value := Int64(m_Abil.MAC2) + m_btSlaveExpLevel * g_Config.nSlave9MAC;
    m_WAbil.MAC2 := Min(High(Integer), Int64Value);
    end;
    if m_boChangeAbility and (m_ChangeAbility.WalkSpeed > 0) then
    begin
    Int64Value := Int64(m_ChangeAbility.WalkSpeed) + ( m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9MoveSpeed;
    m_nWalkSpeed := Min(High(LongWord), Int64Value);
    end
    else
    begin
    m_nWalkSpeed := Max(10, m_nInitWalkSpeed - m_btSlaveExpLevel * g_Config.nSlave9MoveSpeed);
    end;
    if m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
    begin
    Int64Value := Int64(m_ChangeAbility.NextHitTime) + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9HitSpeed;
    m_nNextHitTime := Min(High(LongWord), Int64Value);
    end
    else
    begin
    m_nNextHitTime := Max(100, m_nInitNextHitTime - m_btSlaveExpLevel * g_Config.nSlave9HitSpeed);
    end;
    end;
    if m_boChangeAbility then
    begin
    if m_ChangeAbility.MC1 > 0 then m_WAbil.MC1 := m_ChangeAbility.MC1;
    if m_ChangeAbility.MC2 > 0 then m_WAbil.MC2 := m_ChangeAbility.MC2;
    if m_ChangeAbility.SC1 > 0 then m_WAbil.SC1 := m_ChangeAbility.SC1;
    if m_ChangeAbility.SC2 > 0 then m_WAbil.SC2 := m_ChangeAbility.SC2;
    if m_boChangeAbilitySetHMP and (m_ChangeAbility.HP > 0) then
    begin
    m_WAbil.HP := m_ChangeAbility.HP;
    end;
    if m_ChangeAbility.MP > 0 then m_WAbil.MP := m_ChangeAbility.MP;
    end;
    end;
  *)
end;

procedure THumMon.RecalcLevelAbilitys(IsSysDef: Boolean);
var
  I: Integer;
  MonInfo: pTMonInfo;
begin
  inherited RecalcLevelAbilitys(IsSysDef);
  if UserEngine.FindMonster(m_sCharName, I) then
  begin
    MonInfo := UserEngine.MonsterList.Items[I];
    m_Abil.MaxHP := MonInfo.nHP;
    m_Abil.MaxMP := MonInfo.nMP;
    m_Abil.AC1 := MonInfo.nAC;
    m_Abil.AC2 := MonInfo.nAC;
    m_Abil.MAC1 := MonInfo.nMAC;
    m_Abil.MAC2 := MonInfo.nMAC;
    m_Abil.DC1 := MonInfo.nDC;
    m_Abil.DC2 := MonInfo.nMaxDC;
    m_Abil.MC1 := MonInfo.nMC;
    m_Abil.MC2 := MonInfo.nMC;
    m_Abil.SC1 := MonInfo.nSC;
    m_Abil.SC2 := MonInfo.nSC;
  end;
end;

procedure THumMon.Initialize;
var
  UserItem: pTUserItem;
  I, Level: Integer;
  Magic: pTMagic;
  UserMagic: pTUserMagic;
  StdItem: pTStdItem;
  StdItemIdx: Integer;
  MonsterConfig: pTPlayMonsterConfig;
  sFileName: string;
begin
  inherited Initialize;
  MonsterConfig := GetPlayMonsterConfig(m_sCharName);
  if MonsterConfig <> nil then
  begin
    m_btJob := MonsterConfig.Job;
    m_btGender := MonsterConfig.Gender;
    m_btHair := MonsterConfig.Hair;
    m_boInfiniteMagic := MonsterConfig.NonUseSpellPoint;
    m_boProtectMode := MonsterConfig.boProtectMode;
    m_nProtectRange := MonsterConfig.nProtectRange;
    // m_nDieDropItemRate := MonsterConfig.nDieDropItemRate;
    m_boButchUseItem := MonsterConfig.boButchUseItem; // 挖取身上物品
    m_nButchUseItemRate := MonsterConfig.nButchUseItemRate; // 挖取身上物品几率
    m_boButchListItem := MonsterConfig.boButchListItem; // 挖取列表物品
    m_boButchItemTrigger := MonsterConfig.boButchItemTrigger; // 挖取触发
    m_nButchChargeMode := MonsterConfig.nButchChargeMode; // 挖取收费模式
    m_nButchChargeCount := MonsterConfig.nButchChargeCount; // 挖取收费值
    m_boOnlyButchItemDelGold := MonsterConfig.boOnlyButchItemDelGold;
    m_nDieDropUseItemRate := MonsterConfig.nDieDropUseItemRate;
    m_boRunWithAttack := MonsterConfig.boRunWithAttack;
    m_nRunWithAttackRate := MonsterConfig.nRunWithAttackRate;
    m_boNoAttackMode := MonsterConfig.boNoAttackMode;
    m_boButch := MonsterConfig.boButchUseItem or MonsterConfig.boButchListItem;
    for I := Low(THumanUseItems) to High(THumanUseItems) do
    begin
      StdItem := UserEngine.GetStdItemEx(MonsterConfig.UseItems[I], StdItemIdx);
      if StdItem <> nil then
      begin
        New(UserItem);
        if UserEngine.CopyToUserItemFromItem(StdItem, StdItemIdx, UserItem) then
        begin
          if Random(g_Config.nMonRandomAddValue { 10 } ) = 0 then
            UserEngine.RandomUpgradeItem(UserItem);
          if StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26] then
          begin
            if (StdItem.Shape = 130) or (StdItem.Shape = 131) or (StdItem.Shape = 132) then
            begin // 神秘装备
              UserEngine.GetUnknowItemValue(UserItem);
            end;
          end;
          UserEngine.RandomItemNewAbil(u_Mon, UserItem);
        end;
        m_UseItems[I] := UserItem^;
        Dispose(UserItem);
        if (StdItem.Need in [101, 102]) and (not m_UseItems[I].boStartTime) then
        begin // 计时装备
          m_UseItems[I].nLimitTime := StdItem.NeedLevel;
          m_UseItems[I].boStartTime := True;
        end;
      end;
    end;
    for I := 0 to MonsterConfig.Magics.Count - 1 do
    begin
      if FindMagic(MonsterConfig.Magics.Strings[I]) = nil then
      begin
        Magic := UserEngine.FindMagic(MonsterConfig.Magics.Strings[I]);
        if Magic <> nil then
        begin
          if (Magic.btJob = 99) or (Magic.btJob = m_btJob) then
          begin
            Level := Integer(MonsterConfig.Magics.Objects[I]);
            New(UserMagic);
            UserMagic.MagicInfo := Magic;
            UserMagic.MagicAttr := Magic.MagicAttr;
            UserMagic.wMagIdx := Magic.wMagicID;
            UserMagic.btLevel := LoByte(Level);
            UserMagic.btNewLevel := HiByte(Level);
            UserMagic.btKey := VK_F1;
            UserMagic.nTranPoint := Magic.MaxTrain[3];
            m_MagicList.Add(UserMagic);
          end;
        end;
      end;
    end;
  end;
  sFileName := g_Config.sEnvirDir + 'MonUseItems\' + m_sCharName + '-Item.txt';
  LoadMonitems(sFileName);
  HasLevelUp(0);
  m_Abil.HP := m_Abil.MaxHP;
  m_Abil.MP := m_Abil.MaxMP;
  m_WAbil.HP := m_WAbil.MaxHP;
  m_WAbil.MP := m_WAbil.MaxMP;
  // m_WAbil := m_Abil;
  RefGameSpeed();
end;

function THumMon.LoadMonitems(FileName: string): Integer;
var
  SL: TStringList;
  I: Integer;
  s28, s2C, s30: string;
  n18, n1C: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
begin
  Result := 0;
  if not FileExists(FileName) then
    Exit;
  SL := TStringList.Create;
  try
    SL.LoadFromFile(FileName);
    for I := 0 to SL.Count - 1 do
    begin
      s28 := SL.Strings[I];
      if (s28 <> '') and (s28[1] <> ';') then
      begin
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        n18 := Str_ToInt(s30, -1);
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        n1C := Str_ToInt(s30, -1);
        s28 := GetValidStr3(s28, s30, [' ', #9]);
        if s30 <> '' then
        begin
          if s30[1] = '"' then
            ArrestStringEx(s30, '"', '"', s30);
        end;
        s2C := s30;
        s28 := GetValidStr3(s28, s30, [' ', #9]);
        // 20:=Str_ToInt(s30,1);
        if (n18 > 0) and (n1C > 0) and (s2C <> '') then
        begin
          if Random(n1C) <= (n18 - 1) then
          begin
            if (CompareText(s2C, sSTRING_GOLDNAME) <> 0) then
            begin
              StdItem := UserEngine.GetStdItem(s2C);
              if StdItem <> nil then
              begin
                New(UserItem);
                if UserEngine.CopyToUserItemFromName(s2C, UserItem) then
                begin
                  if Random(m_nItemAddValueRate) = 0 then
                    UserEngine.RandomUpgradeItem(UserItem);
                  if StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26] then
                  begin
                    if (StdItem.Shape = 130) or (StdItem.Shape = 131) or (StdItem.Shape = 132) then
                    begin // 神秘装备
                      UserEngine.GetUnknowItemValue(UserItem);
                    end;
                  end;
                  UserEngine.RandomItemNewAbil(m_nItemNewAddValueRate, UserItem); // 新属性
                  m_ButchItemList.Add(UserItem);
                  Inc(Result);
                end
                else
                  Dispose(UserItem);
              end;
            end;
          end;
        end;
      end;
    end;
    SL.Free;
  except
    SL.Free;
    MainOutMessage('[Exception] THumMon:LoadMonitems');
  end;
end;

function THumMon.CheckRestrictRange: Boolean;
begin
  Result := False;
  if m_Master = nil then
  begin
    Result := m_boProtectMode and ((abs(m_nInitX - m_nCurrX) > m_nProtectRange) or (abs(m_nInitY - m_nCurrY) > m_nProtectRange) or
      m_boLockAttack);
    { if not Result then begin
      Result := (MyGetTickCount > m_dwGoHomeTick) and
      ((abs(m_nInitX - m_nCurrX) > 1) or
      (abs(m_nInitY - m_nCurrY) > 1));
      end; }
  end;
end;

function THumMon.CheckTargetRestrictRange: Boolean;
begin
  Result := False;
  if (m_Master = nil) and (m_TargetCret <> nil) then
  begin
    Result := m_boProtectMode and ((abs(m_nInitX - m_TargetCret.m_nCurrX) > m_nProtectRange) or
      (abs(m_nInitY - m_TargetCret.m_nCurrY) > m_nProtectRange) or m_boLockAttack);
    { if not Result then begin
      Result := (MyGetTickCount > m_dwGoHomeTick) and
      ((abs(m_nInitX - m_nCurrX) > 1) or
      (abs(m_nInitY - m_nCurrY) > 1));
      end; }
  end;
end;

function THumMon.AllowUseMagic(wMagIdx: Word; ShowLowMPHit: Boolean): Boolean;
begin
  Result := False;
  if inherited AllowUseMagic(wMagIdx, ShowLowMPHit) then
  begin
    Result := True;
  end;
end;

function THumMon.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  // Result:=False;
  if ProcessMsg.wIdent = RM_STRUCK then
  begin
    if (TObject(ProcessMsg.BaseObject) = Self) and (TBaseObject(ProcessMsg.nParam3 { AttackBaseObject } ) <> nil) and
      (TBaseObject(ProcessMsg.nParam3
      { AttackBaseObject } ) <> Self) then
    begin
      SetLastHiter(TBaseObject(ProcessMsg.nParam3 { AttackBaseObject } ));
      Struck(TBaseObject(ProcessMsg.nParam3 { AttackBaseObject } )); { 0FFEC }
      BreakHolySeizeMode();
      if (m_Master <> nil) and (TBaseObject(ProcessMsg.nParam3) <> m_Master) and
        (TBaseObject(ProcessMsg.nParam3).m_btRaceServer = RC_PLAYOBJECT) then
      begin
        m_Master.SetPKFlag(TBaseObject(ProcessMsg.nParam3));
      end;
      if g_Config.boMonSayMsg then
        MonsterSayMsg(TBaseObject(ProcessMsg.nParam3), s_UnderFire);
    end;
    Result := True;
  end
  else
  begin
    Result := inherited Operate(ProcessMsg);
  end;
end;

function THumMon.ActThink(wMagicID: Word): Boolean;
var
  nWalkTime: LongWord;
  nDir: Byte;
  nX, nY: Integer;
  boDupMode: Boolean;
  nOldX, nOldY: Integer;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if (m_boNoAttackMode) then
    Exit;
  boDupMode := False;
  if MyGetTickCount - m_dwThinkTick >= 2000 then
  begin
    m_dwThinkTick := MyGetTickCount;
    if ((m_Master = nil) or (not InSafeZone) or ((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))) then
    // 防止安全区宝宝被挤出安全区
    begin
      if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then
        boDupMode := True;
    end
    // 不让宝宝和NPC叠到一起 2020-09-07 00:01:53
    else if (m_Master <> nil) and (m_PEnvir.GetXYNpcObjCount(m_nCurrX, m_nCurrY) >= 1) then
    begin
      boDupMode := True;
    end;
    if boDupMode then
    begin
      nOldX := m_nCurrX;
      nOldY := m_nCurrY;
      WalkTo(Random(8), False);
      if (nOldX <> m_nCurrX) or (nOldY <> m_nCurrY) then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;
  case m_btJob of
    0:
      nWalkTime := g_Config.dwMonsterWarrorWalkTime;
    1:
      nWalkTime := g_Config.dwMonsterWizardWalkTime;
    2:
      nWalkTime := g_Config.dwMonsterTaoistWalkTime;
  else
    nWalkTime := 500;
  end;
  // 如果设置为围着人跑，则围着人跑，否则边跑边攻击
  if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
  begin
    if (m_btJob = 0) and m_boRunWithAttack and (MyGetTickCount - m_dwLastActThinkTick > nWalkTime) and
      (Random(m_nRunWithAttackRate) = 0) then
    begin
      // 英雄保持与目标的攻击距离，围绕目标攻击
      if (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 1) and (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= 1) then
      begin
        nDir := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);
        if Random(2) = 0 then
          nDir := (nDir + 1) mod 8
        else
          nDir := (nDir - 1) mod 8;
        m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nDir, 1, nX, nY);
        if ((nX <> m_nCurrX) or (nY <> m_nCurrY)) and m_PEnvir.CanWalk(nX, nY, False) then
        begin
          SetTargetXY(nX, nY);
          GotoTargetXY;
          Result := True;
          Exit;
        end;
      end;
    end;
    m_dwLastActThinkTick := MyGetTickCount;
    try
      if (m_btJob = 0) then
      begin
        Result := inherited ActThink(wMagicID);
        Exit;
      end
      else
      begin
        if m_boProtectMode then
        begin
          m_MovePath := MovePoint(3);
          m_nMoveIndex := 0;
          if GotoNext() then
          begin
            Result := True;
          end;
        end;
      end
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] THumMon:ActThink ');
        MainOutMessage(E.Message);
      end;
    end;
  end;
end;

procedure THumMon.Run;
var
  nSelectMagic: Integer;
  nWalkTime: LongWord;
  AttackTime: Integer;
  boCanAttack: Boolean;
  ErrCode: Integer;
  nDir, nX, nY: Integer;
  nTargetX, nTargetY: Integer;
  nTempX, nTempY: Integer;
  nDir1, nDir2: Byte;
begin
  ErrCode := 0;
  try
    if m_OPEnvir <> m_PEnvir then
    begin
      ErrCode := 1;
      m_OPEnvir := m_PEnvir;
      m_nMoveIndex := -1;
      SetLength(m_MovePath, 0);
      m_nMoveSameCount := 0;
      m_nOLastDir := -1;
      m_nLastDir := -1;
    end;
    ErrCode := 2;
    if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and (CanMove) then
    begin
      ErrCode := 3;
      if Think then
      begin
        ErrCode := 4;
        inherited;
        Exit;
      end;
      ErrCode := 5;
      if (m_TargetCret <> nil) and ((m_TargetCret = Self) or m_TargetCret.m_boDeath or m_TargetCret.m_boGhost or
        (not IsProperTarget(m_TargetCret))) then
        DelTargetCreat();
      ErrCode := 8;
      if (m_Master <> nil) and (m_Master.m_boDeath or m_Master.m_boGhost) then
      begin
        inherited;
        Exit;
      end;
      ErrCode := 9;
      if m_boFireHitSkill and ((MyGetTickCount - m_SkillUseTick[26]) >= GetMagicCD(SKILL_FIRESWORD)) then
      begin
        m_boFireHitSkill := False;
      end;
      ErrCode := 10;
      if m_boSWordHitSkill and ((MyGetTickCount - m_SkillUseTick[56]) >= GetMagicCD(SKILL_56)) then
      begin
        m_boSWordHitSkill := False;
      end;
      ErrCode := 11;
      if m_bo42Skill and ((MyGetTickCount - m_SkillUseTick[42]) >= GetMagicCD(SKILL_42)) then
      begin // 龙影剑法
        m_bo42Skill := False;
      end;
      ErrCode := 12;
      if m_bo66Skill and ((MyGetTickCount - m_SkillUseTick[66]) >= GetMagicCD(SKILL_66)) then
      begin // 开天斩
        m_bo66Skill := False;
      end;
      ErrCode := 12;
      if m_bo113Skill and ((MyGetTickCount - m_SkillUseTick[113]) >= GetMagicCD(SKILL_113)) then
      begin // 断空斩
        m_bo113Skill := False;
      end;
      if m_bo115Skill and ((MyGetTickCount - m_SkillUseTick[115]) >= GetMagicCD(SKILL_115)) then
      begin // 血魄一击
        m_bo115Skill := False;
      end;
      (*
        // 逐日剑法 chongchong 2013-12-11
        if m_boSWordHitSkill and ((MyGetTickCount - m_SkillUseTick[SKILL_56]) >=  GetMagicCD(SKILL_56)) then
        begin
        m_boSWordHitSkill := False;
        end;
      *)
      ErrCode := 13;
      if m_TargetCret = nil then
      begin
        m_nTargetX := -1;
        m_nTargetY := -1;
      end;
      ErrCode := 14;
      if (MyGetTickCount - m_dwSearchTargetTick > 1000) and (not CheckRestrictRange) then
      begin
        ErrCode := 15;
        if ((m_TargetCret = nil) or (MyGetTickCount - m_dwStruckTick > 5000)) then
        begin
          ErrCode := 16;
          if (m_Master <> nil) then
          begin
            ErrCode := 17;
            if (not m_Master.m_boSlaveRelax) then
            begin
              m_dwSearchTargetTick := MyGetTickCount();
              ErrCode := 18;
              SearchTarget();
            end;
          end
          else
          begin
            if not CheckRestrictRange then
            begin
              ErrCode := 19; //
              m_dwSearchTargetTick := MyGetTickCount();
              SearchTarget();
            end
            else
              DelTargetCreat();
          end;
        end;
      end;
      ErrCode := 20;
      if (m_Master <> nil) and (m_Master.m_boSlaveRelax) then
      begin
        DelTargetCreat();
      end;
      ErrCode := 21;
      if { (m_Master <> nil) and } m_boChangeAbility and (m_ChangeAbility.WalkSpeed > 0) then
      begin
        nWalkTime := m_ChangeAbility.WalkSpeed;
      end
      else
      begin
        case m_btJob of
          0:
            nWalkTime := g_Config.dwMonsterWarrorWalkTime;
          1:
            nWalkTime := g_Config.dwMonsterWizardWalkTime;
          2:
            nWalkTime := g_Config.dwMonsterTaoistWalkTime;
        else
          nWalkTime := 500;
        end;
      end;
      ErrCode := 22;
      if (m_Master = nil) then
      begin
        ErrCode := 23;
        if CheckRestrictRange then
        begin // 超过范围返回出生地
          ErrCode := 24;
          DelTargetCreat();
          if not m_boLockAttack then
          begin
            ErrCode := 25;
            m_boLockAttack := True;
            m_dwGoHomeTick := MyGetTickCount;
            if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
            begin
              if m_btRaceServer <> RC_MOONOBJECT then
              begin
                // SetTargetXY(m_nInitX, m_nInitY);
                // RuntoTargetXY;
                SpaceMove(m_PEnvir.sMapName, m_nInitX, m_nInitY, 0);
              end
              else
              begin
                SetTargetXY(m_nInitX, m_nInitY);
                GotoTargetXY;
              end;
              m_boLockAttack := False;
            end;
          end;
        end
        // 没有攻击目标，返回守护坐标chongchong 2017-12-20
        else if (m_TargetCret = nil) and m_boProtectMode then
        begin
          ErrCode := 24;
          DelTargetCreat();
          if not m_boLockAttack then
          begin
            ErrCode := 25;
            m_boLockAttack := True;
            m_dwGoHomeTick := MyGetTickCount;
            if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
            begin
              if m_btRaceServer <> RC_MOONOBJECT then
              begin
                SetTargetXY(m_nInitX, m_nInitY);
                RunToTargetXY;
              end
              else
              begin
                SetTargetXY(m_nInitX, m_nInitY);
                GotoTargetXY;
              end;
              m_boLockAttack := False;
            end;
          end;
        end;
      end;
      ErrCode := 28;
      if (m_TargetCret <> nil) then
        nSelectMagic := SelectMagic
      else
        nSelectMagic := 0;
      ErrCode := 29;
      if (nSelectMagic <= 0) and (m_btJob <> 0) and not g_Config.boWarrorAttack then
      begin
        DelTargetCreat();
      end;
      { if TargetCret <> m_TargetCret then begin
        m_nMoveIndex := -1;
        SetLength(m_MovePath, 0);
        end; }
      ErrCode := 30;
      if (m_TargetCret <> nil) then
      begin
        if (Length(m_MovePath) > 0) then
        begin
          ErrCode := 31;
          if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
          begin
            ErrCode := 32;
            m_dwMoveTimeTick := MyGetTickCount;
            if GotoNext() then
            begin
              inherited;
              Exit;
            end;
          end
          else
          begin
            inherited;
            Exit;
          end;
        end;
        ErrCode := 33;
        // 人形怪计算武器速度 chongchong 2013-11-18
        if { (m_Master <> nil) and } m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
        begin
          AttackTime := m_ChangeAbility.NextHitTime;
        end
        else
        begin
          AttackTime := g_Config.dwMonsterWarrorAttackTime;
          AttackTime := Max(0, AttackTime - (g_Config.dwIncSpeedDecInterval * m_nHitSpeed)); // 防止负数出错
        end;
        case m_btJob of
          0:
            begin
              if (m_TargetCret <> nil) then
              begin
                if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
                begin
                  ErrCode := 34;
                  if ActThink(nSelectMagic) then
                  begin
                    ErrCode := 35;
                    inherited;
                    Exit;
                  end;
                  ErrCode := 36;
                end;
                if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) and (tick_diff(m_dwHitTick, MyGetTickCount) >= AttackTime) then
                begin
                  if (m_TargetCret <> nil) and (not m_boNoAttackMode) and StartAttack(nSelectMagic) then
                  begin
                    m_dwHitTick := MyGetTickCount();
                    if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                    begin
                      m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                    end;
                  end;
                end
                else
                begin
                  inherited;
                  Exit;
                end;
              end;
            end;
          1:
            begin
              // 躲避正在攻击自己的对象 chongchong 2015-05-05
              if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
              begin
                if (m_LastHiter <> nil) and (abs(m_nCurrX - m_LastHiter.m_nCurrX) < 3) and
                  (abs(m_nCurrY - m_LastHiter.m_nCurrY) < 3) then
                begin
                  nDir := GetNextDirection(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, m_nCurrX, m_nCurrY);
                  if m_boRunWithAttack and (Random(m_nRunWithAttackRate) = 0) then
                  begin
                    nDir := Random(8);
                  end
                  else if Random(20) = 0 then
                    nDir := Random(8);
                  m_PEnvir.GetNextPosition(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, nDir, 5, nX, nY);
                  if (nX > 0) and (nY > 0) then
                  begin
                    SetTargetXY(nX, nY);
                    RunToTargetXY;
                  end;
                end;
              end;
              if { (m_Master <> nil) and } m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
              begin
                AttackTime := m_ChangeAbility.NextHitTime;
              end
              else
              begin
                AttackTime := g_Config.dwMonsterWizardAttackTime;
              end;
              if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > AttackTime) then
              begin
                ErrCode := 37;
                if (abs(m_nCurrX - m_TargetCret.m_nCurrX) > g_Config.nMagicAttackRage) or
                  (abs(m_nCurrY - m_TargetCret.m_nCurrY) > g_Config.nMagicAttackRage) then
                begin
                  SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                  if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
                  begin
                    RunToTargetXY;
                  end;
                  inherited;
                  Exit;
                end
                else if ActThink(nSelectMagic) then
                begin
                  inherited;
                  Exit;
                end;
                // 修正人形怪使用灭天火目标不掉血 chongchong 2014-06-25
                // 原因在于 TMagicManager.DoSpell SKILL_45:
                if (nSelectMagic = SKILL_45) then
                begin
                  boCanAttack := MyGetTickCount - m_SkillUseTick[SKILL_45] >= GetMagicCD(SKILL_45);
                end
                else
                  boCanAttack := True;
                ErrCode := 39;
                if (m_TargetCret <> nil) and (not m_boNoAttackMode) and boCanAttack then
                begin
                  if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= g_Config.nMagicAttackRage) and
                    (abs(m_nCurrY - m_TargetCret.m_nCurrY) <= g_Config.nMagicAttackRage) and StartAttack(nSelectMagic) then
                  begin
                    m_dwHitTick := MyGetTickCount();
                    if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                    begin
                      m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                    end;
                  end;
                end;
              end;
            end;
          2:
            begin
              // 躲避正在攻击自己的对象 chongchong 2015-05-05
              if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
              begin
                if (m_LastHiter <> nil) and (abs(m_nCurrX - m_LastHiter.m_nCurrX) < 3) and
                  (abs(m_nCurrY - m_LastHiter.m_nCurrY) < 3) then
                begin
                  nDir := GetNextDirection(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, m_nCurrX, m_nCurrY);
                  if m_boRunWithAttack and (Random(m_nRunWithAttackRate) = 0) then
                  begin
                    nDir := Random(8);
                  end
                  else if Random(20) = 0 then
                    nDir := Random(8);
                  m_PEnvir.GetNextPosition(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, nDir, 5, nX, nY);
                  if (nX > 0) and (nY > 0) then
                  begin
                    SetTargetXY(nX, nY);
                    RunToTargetXY;
                  end;
                end;
              end;
              if { (m_Master <> nil) and } m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
              begin
                AttackTime := m_ChangeAbility.NextHitTime;
              end
              else
              begin
                AttackTime := g_Config.dwMonsterTaoistAttackTime;
              end;
              if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > AttackTime) then
              begin
                ErrCode := 40;
                if (abs(m_nCurrX - m_TargetCret.m_nCurrX) > g_Config.nMagicAttackRage) or
                  (abs(m_nCurrY - m_TargetCret.m_nCurrY) > g_Config.nMagicAttackRage) then
                begin
                  SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                  if (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
                  begin
                    RunToTargetXY;
                  end;
                  inherited;
                  Exit;
                end
                else if ActThink(nSelectMagic) then
                begin
                  inherited;
                  Exit;
                end;
                ErrCode := 41;
                if (m_TargetCret <> nil) and (not m_boNoAttackMode) then
                begin
                  if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= g_Config.nMagicAttackRage) and
                    (abs(m_nCurrY - m_TargetCret.m_nCurrY) <= g_Config.nMagicAttackRage) and StartAttack(nSelectMagic) then
                  begin
                    m_dwHitTick := MyGetTickCount();
                    if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                    begin
                      m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                    end;
                  end;
                end;
              end;
            end;
        end;
        ErrCode := 42;
        Wondering;
        ErrCode := 43;
      end
      else if (m_TargetCret = nil) then
      begin
        if (m_Master = nil) and (not m_boProtectMode) and m_boMission and (Length(m_nMissionPoints) > 0) and
          (m_nMissionPointIndex < Length(m_nMissionPoints)) and (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
        begin
          m_nTargetX := -1;
          if (m_nMissionPointIndex < 0) then
            m_nMissionPointIndex := 0;
          if (abs(m_nCurrX - m_nMissionPoints[m_nMissionPointIndex].X) <= 3) and
            (abs(m_nCurrY - m_nMissionPoints[m_nMissionPointIndex].Y) <= 3) then
          begin
            Inc(m_nMissionPointIndex);
          end;
          m_nTargetX := m_nMissionPoints[m_nMissionPointIndex].X;
          m_nTargetY := m_nMissionPoints[m_nMissionPointIndex].Y; // 004A91D3
          if (abs(m_nCurrX - m_nTargetX) >= 4) or (abs(m_nCurrY - m_nTargetY) >= 4) then
          begin
            if m_nTargetX <> -1 then
            begin
              if Random(5) = 0 then
              begin
                if GotoNearRuntoXY(m_nTargetX, m_nTargetY, nTargetX, nTargetY) then
                begin
                  nDir1 := GetNextDirection(m_nCurrX, m_nCurrY, nTargetX, nTargetY);
                  if ((m_nTargetX <> nTargetX) or (m_nTargetY <> nTargetY)) and GotoNearRuntoXY(nTargetX, nTargetY, m_nTargetX,
                    m_nTargetY, nTempX, nTempY) then
                  begin
                    nDir2 := GetNextDirection(nTargetX, nTargetY, nTempX, nTempY);
                    // 当前步骤和下一步的方向相反，就是来回搞chongchong 2017-10-26
                    if GetDifferenceDirection(nDir1) = nDir2 then
                    begin
                      // OutputDebugString('aaaa');
                      m_dwMoveTimeTick := MyGetTickCount;
                      Exit;
                    end;
                  end;
                  if (abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1) then
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
              end
              else
              begin
                if Random(2) = 0 then
                  RunToTargetXY
                else
                  GotoTargetXY;
              end;
            end;
          end
          else
          begin
            GotoTargetXY;
          end;
        end
        else
          inherited Wondering;
      end;
    end
    // 修正人形怪死亡后尸体会变位置 2019-03-06 12:15:18
    else if (not m_boGhost) and (not m_boDeath) then
      inherited Wondering;
    inherited;
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] THumMon:Run; Code =' + IntToStr(ErrCode));
      MainOutMessage(E.Message);
    end;
  end
end;

procedure THumMon.ScatterBagItems(ItemOfCreat: TBaseObject; KillMe: TBaseObject);
var
  I, II, DropWide: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  boCanDrop: Boolean;
  MonDrop: pTMonDrop;
  ItemCreateName: string;
  IsChange: Boolean;
  // DropRate: Integer;
resourcestring
  sExceptionMsg = '[Exception] THumMon.ScatterBagItems';
begin
  if m_boAngryRing or m_boNoDropItem { or (m_btButch = 0) } then
    Exit; // 不死戒指
  {
    DropRate := 0;
    if ItemOfCreat <> nil then
    DropRate := ItemOfCreat.m_WAbil.NewValue[11];
    UserEngine.MonGetRandomItems(Self, DropRate);
  }
  DropWide := g_Config.nScatterItemRange;
  try
    if m_ItemList <> nil then
    begin
      for I := m_ItemList.Count - 1 downto 0 do
      begin
        if m_ItemList.Count <= 0 then
          Break;
        UserItem := m_ItemList.Items[I];
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if StdItem = nil then
          Continue;
        if (GetUserItemBindValue(UserItem, ubNoScatter) and UserItem.boIsBind) or g_ItemRules.Get(UserItem.wIndex, 13) then
          Continue; // 禁止爆出
        boCanDrop := True;
        if not g_ItemRules.Get(UserItem.wIndex, 6) then
        begin
          // 防暴几率 chongchong 2015-04-16
          if Random(100) < UserItem.btNewValue[12] then
            Continue;
          IsChange := False;
          g_MonDropLimitList.Lock;
          try
            if (g_MonDropLimitList.Count > 0) then
            begin
              for II := 0 to g_MonDropLimitList.Count - 1 do
              begin
                if (CompareText(StdItem.Name, g_MonDropLimitList.Strings[II]) = 0) then
                begin
                  MonDrop := pTMonDrop(g_MonDropLimitList.Objects[II]);
                  if MonDrop <> nil then
                  begin
                    if (MonDrop.nDropCount < MonDrop.nCountLimit) then
                    begin
                      Inc(MonDrop.nDropCount);
                      MonDrop.nNoDropCount := MonDrop.nCountLimit - MonDrop.nDropCount;
                      IsChange := True;
                    end
                    else
                    begin
                      boCanDrop := False;
                    end;
                  end;
                  Break;
                end;
              end;
            end;
          finally
            g_MonDropLimitList.UnLock;
          end;
          if IsChange then
          begin
            SaveMonDropLimitList;
          end;
          if ItemOfCreat <> nil then
            ItemCreateName := ItemOfCreat.m_sCharName
          else
            ItemCreateName := '?';
          if not g_DropLimitMgr.DropItem(m_PEnvir, StdItem.Name, m_sCharName, ItemCreateName, Point(m_nCurrX, m_nCurrY)) then
          begin
            boCanDrop := False;
          end;
          if not boCanDrop then
            Continue;
          UserItem.ItemFrom.ItemForm := ifMonDrop;
          UserItem.ItemFrom.sMapName := m_PEnvir.sMapDesc;
          UserItem.ItemFrom.sMonName := FilterShowName(m_sCharName);
          UserItem.ItemFrom.DateTime := Now();
          if (ItemOfCreat <> nil) then
          begin
            if (ItemOfCreat.Master <> nil) then
            begin
              if ItemOfCreat.Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
                UserItem.ItemFrom.sMakerName := ItemOfCreat.Master.m_sCharName
              else
                UserItem.ItemFrom.sMakerName := FilterShowName(ItemOfCreat.Master.m_sCharName);
            end
            else
            begin
              if ItemOfCreat.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
                UserItem.ItemFrom.sMakerName := ItemOfCreat.m_sCharName
              else
                UserItem.ItemFrom.sMakerName := FilterShowName(ItemOfCreat.m_sCharName);
            end;
          end;
          if DropItemDown(UserItem, DropWide, True, ItemOfCreat, Self) then
          begin
            m_ItemList.Delete(I);
            Dispose(UserItem);
          end;
        end;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg + ' ' + m_sCharName);
  end;
end;

procedure THumMon.DropUseItems(BaseObject: TBaseObject);
var
  I: Integer;
  nRate, nNewRate: Integer;
  StdItem: pTStdItem;
resourcestring
  sExceptionMsg = '[Exception] THumMon.DropUseItems';
begin
  try
    if m_boAngryRing or m_boNoDropUseItem or m_PEnvir.m_boNODROPUSEITEMS then
      Exit;
    for I := Low(THumanUseItems) to High(THumanUseItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
      if (StdItem <> nil) and (StdItem.Reserved and 8 <> 0) then
      begin
        if StdItem.NeedIdentify = 1 then
          AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, m_UseItems[I].MakeIndex, '0'); // ??????
        m_UseItems[I].wIndex := 0;
      end;
    end;
    nRate := m_nDieDropUseItemRate;
    if not g_Config.boDropUseItem then
    begin
      if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[11] > 0) then
      begin // 增加爆率
        nRate := Max(nRate - nRate * m_CurrTarget.m_WAbil.NewValue[11] div 100, 0);
      end;
      if (m_CurrTarget <> nil) and (m_CurrTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
        (TSmartObject(m_CurrTarget).m_dwKillMonBurstRate > 0) then
      begin
        nRate := Round(nRate / TSmartObject(m_CurrTarget).m_dwKillMonBurstRate * 100)
      end;
    end;
    for I := Low(THumanUseItems) to High(THumanUseItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
      if StdItem = nil then
        Continue;
      if (GetUserItemBindValue(@m_UseItems[I], ubNoScatter) and m_UseItems[I].boIsBind) or
        g_ItemRules.Get(m_UseItems[I].wIndex, 13) then
        Continue; // 禁止爆出
      if Random(100) < m_UseItems[I].btNewValue[12] then
        Continue;
      if g_Config.boDropUseItem then
      begin
        if PKLevel > 2 then
          nNewRate := Round(g_Config.DieDropUseItemRates[I] / (g_Config.nDieRedDropUseItemOneRate / 10))
        else
          nNewRate := g_Config.DieDropUseItemRates[I];
        nNewRate := nNewRate + nRate;
        // 怪物暴率 chongchong 2015-04-16
        if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[11] > 0) then
        begin
          nNewRate := Max(nNewRate - nNewRate * m_CurrTarget.m_WAbil.NewValue[11] div 100, 0);
        end;
        if (m_CurrTarget <> nil) and (m_CurrTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
          (TSmartObject(m_CurrTarget).m_dwKillMonBurstRate > 0) then
        begin
          nNewRate := Round(nNewRate / TSmartObject(m_CurrTarget).m_dwKillMonBurstRate * 100)
        end;
        if (Random(nNewRate) <> 0) and (not g_ItemRules.Get(m_UseItems[I].wIndex, 6)) then
          Continue;
      end
      else
      begin
        if (Random(nRate) <> 0) and (not g_ItemRules.Get(m_UseItems[I].wIndex, 6)) then
          Continue;
      end;
      if InDisableTakeOffList(m_UseItems[I].wIndex) or (GetUserItemBindValue(@m_UseItems[I], ubNoTakeOff) and
        m_UseItems[I].boIsBind) then
        Continue; // 检查是否在禁止取下列表,如果在列表中则不掉此物品
      m_UseItems[I].ItemFrom.ItemForm := ifMonDrop;
      m_UseItems[I].ItemFrom.sMapName := m_PEnvir.sMapDesc;
      m_UseItems[I].ItemFrom.sMonName := FilterShowName(m_sCharName);
      m_UseItems[I].ItemFrom.DateTime := Now();
      if (BaseObject <> nil) then
      begin
        if (BaseObject.Master <> nil) then
        begin
          if BaseObject.Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
            m_UseItems[I].ItemFrom.sMakerName := BaseObject.Master.m_sCharName
          else
            m_UseItems[I].ItemFrom.sMakerName := FilterShowName(BaseObject.Master.m_sCharName);
        end
        else
        begin
          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
            m_UseItems[I].ItemFrom.sMakerName := BaseObject.m_sCharName
          else
            m_UseItems[I].ItemFrom.sMakerName := FilterShowName(BaseObject.m_sCharName);
        end;
      end
      else if m_ExpHitter <> nil then
      begin
        if (m_ExpHitter.Master <> nil) then
        begin
          if m_ExpHitter.Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
            m_UseItems[I].ItemFrom.sMakerName := m_ExpHitter.Master.m_sCharName
          else
            m_UseItems[I].ItemFrom.sMakerName := FilterShowName(m_ExpHitter.Master.m_sCharName);
        end
        else
        begin
          if m_ExpHitter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
            m_UseItems[I].ItemFrom.sMakerName := m_ExpHitter.m_sCharName
          else
            m_UseItems[I].ItemFrom.sMakerName := FilterShowName(m_ExpHitter.m_sCharName);
        end;
      end;
      if DropItemDown(@m_UseItems[I], 2, True, BaseObject, Self) then
      begin
        StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
        if StdItem <> nil then
        begin
          if StdItem.Reserved and 10 = 0 then
          begin
            m_UseItems[I].wIndex := 0;
          end;
        end;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
end;
{ ------------------------------------------------------------------------------ }

constructor TCopyMon.Create();
begin
  inherited;
  m_nViewRange := 8;
  m_btRaceServer := RC_PLAYMOSTER;
  FCreateTick := MyGetTickCount;
  m_nInheritedPercent := 100;
  // 2020-09-30 调整分身运行间隔
  m_nRunTime := 100;
  m_boAliveRebelled := False;
end;

destructor TCopyMon.Destroy;
begin
  inherited;
end;

function TCopyMon.AllowUseMagic(wMagIdx: Word; ShowLowMPHit: Boolean): Boolean;
var
  UserMagic: pTUserMagic;
begin
  Result := False;
  if (wMagIdx < Length(m_UserMagics)) and m_PEnvir.AllowMagics(wMagIdx) then
  begin
    UserMagic := m_UserMagics[wMagIdx]; // FindMagic(wMagIdx);
    if UserMagic <> nil then
    begin
      if UserMagic.btKey > 0 then
      begin
        if inherited AllowUseMagic(wMagIdx, ShowLowMPHit) then
        begin
          Result := True;
        end;
      end;
    end;
  end;
end;

procedure TCopyMon.Struck(hiter: TBaseObject);

  function CanSetTarget: Boolean;
  begin
    Result := IsProperTarget(hiter);
    if (m_TargetCret = nil) then
    begin
      Exit;
    end;
    if (m_TargetCret <> nil) and Result then
    begin
      Result := ((abs(m_nCurrX - m_TargetCret.m_nCurrX) + abs(m_nCurrY - m_TargetCret.m_nCurrY)) >
        (abs(m_nCurrX - hiter.m_nCurrX) + abs(m_nCurrY - hiter.m_nCurrY))) or
        ((m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_TargetCret.m_nCurrX) + abs(m_Master.m_nCurrY - m_TargetCret.m_nCurrY))
        > (abs(m_Master.m_nCurrX - hiter.m_nCurrX) + abs(m_Master.m_nCurrY - hiter.m_nCurrY))));
    end;
  end;

begin
  m_dwStruckTick := MyGetTickCount;
  if hiter <> nil then
  begin
    if ((m_Master = nil) or (m_Master <> nil) and (not m_Master.m_boSlaveRelax)) and CanSetTarget then
      SetTargetCreat(hiter);
  end;
  if m_boAnimal then
  begin
    m_nMeatQuality := m_nMeatQuality - Random(300);
    if m_nMeatQuality < 0 then
      m_nMeatQuality := 0;
  end;
  m_dwHitTick := m_dwHitTick + LongWord(150 - Min(130, m_Abil.Level * 4));
end;

function TCopyMon.Operate(ProcessMsg: pTProcessMessage): Boolean;
var
  BaseObject: TBaseObject;
begin
  // Result:=False;
  if ProcessMsg.wIdent = RM_STRUCK then
  begin
    if (TObject(ProcessMsg.BaseObject) = Self) and (TBaseObject(ProcessMsg.nParam3 { AttackBaseObject } ) <> nil) then
    begin
      BaseObject := TBaseObject(ProcessMsg.nParam3 { AttackBaseObject } );
      SetLastHiter(BaseObject);
      Struck(TBaseObject(ProcessMsg.nParam3 { AttackBaseObject } )); { 0FFEC }
      BreakHolySeizeMode();
      if (m_Master <> nil) and (TBaseObject(ProcessMsg.nParam3) <> m_Master) and
        (TBaseObject(ProcessMsg.nParam3).m_btRaceServer = RC_PLAYOBJECT) then
      begin
        m_Master.SetPKFlag(TBaseObject(ProcessMsg.nParam3));
      end;
      if g_Config.boMonSayMsg then
        MonsterSayMsg(TBaseObject(ProcessMsg.nParam3), s_UnderFire);
    end;
    Result := True;
  end
  else
  begin
    Result := inherited Operate(ProcessMsg);
  end;
end;

function TCopyMon.ActThink(wMagicID: Word): Boolean;
begin
  { TODO -ochongchong -c新增 : 英雄分身在安全区停止攻击 【2013-08-18】 }
  if (m_Master <> nil) and (m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
    (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and m_TargetCret.InSafeZone then
  begin
    Result := True;
  end
  else
    Result := inherited ActThink(wMagicID);
end;

procedure TCopyMon.Run;
var
  I, nSelectMagic, nX, nY: Integer;
  nWalkTime: LongWord;
  UserMagic, TestMagic: pTUserMagic;
  NoMagicAttact: Boolean;
  dwAttackTime: LongWord;
begin
  if m_OPEnvir <> m_PEnvir then
  begin
    m_OPEnvir := m_PEnvir;
    m_nMoveIndex := -1;
    SetLength(m_MovePath, 0);
    m_nMoveSameCount := 0;
    m_nOLastDir := -1;
    m_nLastDir := -1;
  end;
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and (CanMove) then
  begin
    if Think then
    begin
      // MainOutMessage('Think:'+m_sCharName);
      inherited;
      Exit;
    end;
    if (m_Master <> nil) and (m_Master.m_boDeath or m_Master.m_boGhost) then
    begin
      for I := m_Master.m_SlaveList.Count - 1 downto 0 do
      begin
        if m_Master.m_SlaveList.Items[I] = Self then
        begin
          m_Master.m_SlaveList.Delete(I);
          Break;
        end;
      end;
      m_Master := nil;
      MakeGhost;
      // MainOutMessage('m_Master.m_boDeath:'+m_sCharName);
      inherited;
      Exit;
    end;
    if not m_boAliveRebelled then
    begin
      if (m_Master = nil) then
      begin
        MakeGhost;
        // MainOutMessage('(m_Master = nil):'+m_sCharName);
        inherited;
        Exit;
      end
      else if (not m_boGhost) and (not m_boDeath) then
      begin
        if (m_PEnvir <> m_Master.m_PEnvir) or (abs(m_nCurrX - m_Master.m_nCurrX) > 20) or (abs(m_nCurrY - m_Master.m_nCurrY) > 20)
        then
        begin
          m_Master.GetBackPosition(nX, nY);
          if not m_Master.m_PEnvir.CanWalk(nX, nY, True) then
          begin
            for I := 0 to 7 do
            begin
              if m_Master.m_PEnvir.GetNextPosition(m_Master.m_nCurrX, m_Master.m_nCurrY, I, 1, nX, nY) then
              begin
                if m_Master.m_PEnvir.CanWalk(nX, nY, True) then
                begin
                  Break;
                end;
              end;
            end;
          end;
          DelTargetCreat;
          m_nTargetX := nX;
          m_nTargetY := nY;
          SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
          Exit;
        end;
      end;
    end;
    // 英雄分身持续开盾 chongchong 2017-10-28
    if m_btJob = 1 then
    begin
      if AllowUseMagic(SKILL_SHIELD) and (m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] = 0) then
      begin
        UserMagic := m_UserMagics[SKILL_SHIELD];
        if UserMagic <> nil then
        begin
          DoSpell(UserMagic, m_nCurrX, m_nCurrY, nil);
          Exit;
        end;
      end;
    end;
    if m_boFireHitSkill and ((MyGetTickCount - m_SkillUseTick[26]) > 20 * 1000) then
    begin
      m_boFireHitSkill := False;
    end;
    if m_boSWordHitSkill and ((MyGetTickCount - m_SkillUseTick[56]) > 20 * 1000) then
    begin
      m_boSWordHitSkill := False;
    end;
    if m_bo42Skill and ((MyGetTickCount - m_SkillUseTick[42]) > 20 * 1000) then
    begin // 龙影剑法
      m_bo42Skill := False;
    end;
    if m_bo66Skill and ((MyGetTickCount - m_SkillUseTick[66]) > 20 * 1000) then
    begin // 开天斩
      m_bo66Skill := False;
    end;
    if m_bo113Skill and ((MyGetTickCount - m_SkillUseTick[113]) > 20 * 1000) then
    begin // 开天斩
      m_bo113Skill := False;
    end;
    if m_bo115Skill and ((MyGetTickCount - m_SkillUseTick[115]) > 20 * 1000) then
    begin // 开天斩
      m_bo115Skill := False;
    end;
    if (m_TargetCret <> nil) and (m_TargetCret.m_boDeath or m_TargetCret.m_boGhost) then
      DelTargetCreat();
    if not IsProperTarget(m_TargetCret) then
      DelTargetCreat();
    if m_TargetCret = nil then
    begin
      m_nTargetX := -1;
      m_nTargetY := -1;
    end;
    // 分身时间改为和英雄一致 chongchong 2015-11-24
    if g_Config.boCopyMonInheritedMasterSpeed then
    begin
      nWalkTime := g_Config.dwMoveFrameTime;
      if m_nMoveSpeed <> 0 then // 行走速度  自动调整变速后时间间隔
        nWalkTime := Max(nWalkTime - (g_Config.dwIncMoveSpeedDecInterval * m_nMoveSpeed), 0);
    end
    else
    begin
      case m_btJob of
        0:
          nWalkTime := g_Config.dwHeroWarrorWalkTime;
        1:
          nWalkTime := g_Config.dwHeroWizardWalkTime;
        2:
          nWalkTime := g_Config.dwHeroTaoistWalkTime;
      else
        nWalkTime := 500;
      end;
    end;
    if (MyGetTickCount - m_dwSearchTargetTick > 1000) then
    begin
      { TODO -ochongchong -c修改 : 分身和主人攻击同一目标 【2013-08-24】 }
      if (m_Master <> nil) and (m_Master.m_TargetCret <> nil) and (m_Master.m_TargetCret <> Self) and
        g_Config.boAlwaysFollowMasterAttack and (not m_Master.m_TargetCret.m_boGhost) and (not m_Master.m_TargetCret.m_boDeath)
        and (m_Master.m_TargetCret.Master <> m_Master) then // 不攻击主人或自己的宝宝 chongchong 2015-10-28
      begin
        if not((m_nSlaveAttackHumPowerRate = 0) and (m_Master.m_TargetCret.m_btRaceServer = RC_PLAYOBJECT)) then
        begin
          SetTargetCreat(m_Master.m_TargetCret);
          if m_btJob = 0 then
          begin
            if (m_TargetCret <> nil) and (abs(m_nCurrX - m_TargetCret.m_nCurrX) > 4) or (abs(m_nCurrY - m_TargetCret.m_nCurrY) > 4)
            then
            begin
              m_dwSearchTargetTick := MyGetTickCount();
              if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
              begin
                // 跑到主人那里去一鸟，跟主人并肩作战 chongchong 2013-10-29
                // SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                SetTargetXY(m_Master.m_nCurrX, m_Master.m_nCurrY);
                RunToTargetXY;
              end;
              Exit;
            end;
          end;
        end;
        // 修正分身和主人同一目标时，攻击追击目标又向主人位置移动2018-08-17 23:30:51
        {
          else if (m_TargetCret <> nil) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > g_Config.nMagicAttackRage) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > g_Config.nMagicAttackRage) then
          begin
          m_dwSearchTargetTick := MyGetTickCount();
          if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
          begin
          // 跑到主人那里去一鸟，跟主人并肩作战 chongchong 2013-10-29
          //SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
          SetTargetXY(m_Master.m_nCurrX, m_Master.m_nCurrY);
          RuntoTargetXY;
          end;
          Exit;
          end;
        }
      end
      else if ((m_TargetCret = nil) or (MyGetTickCount - m_dwStruckTick > 5000)) then
      begin
        if (m_Master <> nil) then
        begin
          if (not m_Master.m_boSlaveRelax) { and (not InSafeZone) } then
          begin
            m_dwSearchTargetTick := MyGetTickCount();
            SearchTarget();
            if (m_TargetCret <> nil) and (InSafeZone) then
            begin
              if (Master <> nil) and (Master.m_btRaceServer = RC_PLAYOBJECT) then
              begin
                if m_TargetCret.m_btRaceServer = RC_PLAYOBJECT then
                  DelTargetCreat
                else if (m_TargetCret.Master <> nil) and (m_TargetCret.Master.m_btRaceServer = RC_PLAYOBJECT) then
                  DelTargetCreat;
              end;
            end;
          end;
        end
        else
        begin
          if (m_TargetCret <> nil) and (MyGetTickCount - m_dwStruckTick > 30000) then
            DelTargetCreat();
          if m_boAliveRebelled then
          begin
            m_dwSearchTargetTick := MyGetTickCount();
            SearchTarget();
          end;
        end;
      end;
    end;
    { if  (not InSafeZone) then begin
      if ((m_TargetCret = nil) or (MyGetTickCount - m_dwStruckTick > 1000)) or (MyGetTickCount - m_dwSearchTargetTick > 1000) then begin
      m_dwSearchTargetTick := MyGetTickCount();
      SearchTarget();
      end;
      end; }
    if m_Master <> nil then
    begin
      if (m_TargetCret = nil) and (not m_Master.m_boSlaveRelax) then
      begin
        if (m_Master.m_TargetCret <> nil) and (m_Master.m_TargetCret.m_Master <> m_Master) and
          (m_Master.m_TargetCret.Master <> Master) and (m_Master.m_TargetCret = Self) then
          SetTargetCreat(m_Master.m_TargetCret);
      end;
      if (m_Master.m_boSlaveRelax) then
        DelTargetCreat();
    end;
    // 选择技能 chongchong 2018-06-02
    if (m_TargetCret <> nil) then
      nSelectMagic := SelectMagic
    else
      nSelectMagic := 0; // 2020-10-23 初始化
    if (nSelectMagic <= 0) and (m_btJob = 2) and (m_TargetCret <> nil) then
    begin
      NoMagicAttact := m_MagicList.Count = 0;
      if not NoMagicAttact then
      begin
        NoMagicAttact := True;
        for I := 0 to m_MagicList.Count - 1 do
        begin
          TestMagic := m_MagicList[I];
          if (TestMagic <> nil) and (TestMagic.btKey > 0) and
            (not(TestMagic.wMagIdx in [SKILL_HEALLING { 治愈术 } , SKILL_UNAMYOUNSUL
            { 解毒术 } , SKILL_ILKWANG { 精神力战法 } , SKILL_74 { 分身术 } ])) then
          begin
            NoMagicAttact := False;
            Break;
          end;
        end;
      end;
      if not((g_Config.boCopyMonWarrorAttack and NoMagicAttact) or (g_Config.boCopyMon700HPUseBaseAttack and
        (m_TargetCret.m_WAbil.MaxHP < 700))) then
      begin
        DelTargetCreat();
      end;
    end;
    { if TargetCret <> m_TargetCret then begin
      m_nMoveIndex := -1;
      SetLength(m_MovePath, 0);
      end; }
    if (m_TargetCret <> nil) then
    begin
      if (Length(m_MovePath) > 0) then
      begin
        if MyGetTickCount - m_dwMoveTimeTick > nWalkTime + m_nWalkDelay then
        begin
          if GotoNext() then
          begin
            m_dwMoveTimeTick := MyGetTickCount;
            // 修正分身被推后前三刀攻击过快 2021-01-28
            if not(m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
              m_dwHitTick := MyGetTickCount;
            // inherited;
            // Exit;     修正分身躲避到攻击间隔慢 chongchong 2018-06-02
          end;
        end
        else
        begin
          inherited;
          Exit;
        end;
      end;
      if g_Config.boCopyMonInheritedMasterSpeed then
      begin
        if m_btJob = 0 then
        begin
          dwAttackTime := g_Config.dwHitFrameTime;
          if m_nAttackSpeed <> 0 then // 攻击速度  自动调整变速后时间间隔
            dwAttackTime := Max(dwAttackTime - (g_Config.dwIncSpeedDecInterval * m_nAttackSpeed), 0);
        end
        else
        begin
          // 客户端参考：TfrmMain.CanUseMagic
          // 两次魔法攻击之间的间隔时间 piaoyun 2013-07-20
          // dwMagicDelayTime := g_ClientConfig.dwMagicHitFrameTime + 50 + g_dwMagicDelayTime;
          dwAttackTime := g_Config.dwMagicHitFrameTime + 600;
          if m_nSpellSpeed <> 0 then // 魔法攻击速度  自动调整变速后时间间隔
            dwAttackTime := Max(dwAttackTime - m_nSpellSpeed * g_ClientConfig.dwIncSpellSpeedDecInterval, 0);
        end;
      end
      else
      begin
        if m_btJob = 0 then
          dwAttackTime := g_Config.dwHeroWarrorAttackTime
        else if m_btJob = 1 then
          dwAttackTime := g_Config.dwHeroWizardAttackTime
        else
          dwAttackTime := g_Config.dwHeroTaoistAttackTime;
      end;
      case m_btJob of
        0:
          begin
            if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > dwAttackTime) then
            begin
              if ActThink(nSelectMagic) then
              begin
                inherited;
                Exit;
              end;
              if (m_TargetCret <> nil) and StartAttack(nSelectMagic) then
              begin
                m_boLastAvoidTarget := False;
                m_dwHitTick := MyGetTickCount();
                if (nSelectMagic > 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                begin
                  m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                end;
              end;
            end;
          end;
        1:
          begin
            if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > dwAttackTime) then
            begin
              if ActThink(nSelectMagic) then
              begin
                inherited;
                Exit;
              end;
              if (m_TargetCret <> nil) and StartAttack(nSelectMagic) then
              begin
                m_dwHitTick := MyGetTickCount();
                m_boLastAvoidTarget := False;
                if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                begin
                  m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                end;
              end;
            end;
          end;
        2:
          begin
            if (m_TargetCret <> nil) and (tick_diff(m_dwHitTick, MyGetTickCount) > dwAttackTime) then
            begin
              if ActThink(nSelectMagic) then
              begin
                inherited;
                Exit;
              end;
              if (m_TargetCret <> nil) and StartAttack(nSelectMagic) then
              begin
                m_dwHitTick := MyGetTickCount();
                m_boLastAvoidTarget := False;
                if (nSelectMagic >= 0) and (nSelectMagic <= High(m_SkillUseTick)) then
                begin
                  m_SkillUseTick[nSelectMagic] := MyGetTickCount;
                end;
              end;
            end;
          end;
      end;
    end;
    Wondering;
  end;
  inherited;
end;

procedure TCopyMon.ScatterBagItems(ItemOfCreat: TBaseObject; KillMe: TBaseObject);
begin
end;

procedure TCopyMon.DropUseItems(BaseObject: TBaseObject);
begin
end;

procedure TCopyMon.Copy(Source: TBaseObject);
var
  I: Integer;
  UserMagic: pTUserMagic;
begin
  // if Source = nil then Exit;
  // m_nViewRange := Max(Source.m_nViewRange - 2, 10);
  m_Master := Source;
  m_btJob := Source.m_btJob;
  m_btHair := Source.m_btHair;
  m_btGender := Source.m_btGender;
  m_PEnvir := Source.m_PEnvir;
  m_sMapName := Source.m_sMapName;
  m_btDirection := Source.m_btDirection;
  m_Abil := Source.m_Abil;
  m_WAbil := Source.m_WAbil;
  m_WAbil.HP := m_WAbil.MaxHP;
  m_WAbil.MP := m_WAbil.MaxMP;
  m_Abil.HP := m_Abil.MaxHP;
  m_Abil.MP := m_Abil.MaxMP;
  Move(Source.m_Abil.NewValue, m_Abil.NewValue, SizeOf(m_Abil.NewValue));
  Move(Source.m_WAbil.NewValue, m_WAbil.NewValue, SizeOf(m_WAbil.NewValue));
  if Source is TSmartObject then
  begin
    m_boShowFashion := TSmartObject(Source).m_boShowFashion;
    // m_UseItems := TSmartObject(Source).m_UseItems;
    Move(TSmartObject(Source).m_UseItems, m_UseItems, SizeOf(m_UseItems));
    Move(TSmartObject(Source).m_JewelryBoxItems, m_JewelryBoxItems, SizeOf(m_JewelryBoxItems));
    Move(TSmartObject(Source).m_GodBlessItems, m_GodBlessItems, SizeOf(m_GodBlessItems));
    m_nMoveSpeed := TSmartObject(Source).m_nMoveSpeed;
    m_nNpcAttackSpeed := TSmartObject(Source).m_nNpcAttackSpeed;
    m_nAttackSpeed := TSmartObject(Source).m_nAttackSpeed;
    m_nSpellSpeed := TSmartObject(Source).m_nSpellSpeed;; // 魔法速度  -10 ~ +10
  end;
  if m_Master is TSmartObject then
  begin
    for I := 0 to TSmartObject(m_Master).m_MagicList.Count - 1 do
    begin
      New(UserMagic);
      UserMagic^ := pTUserMagic(TSmartObject(m_Master).m_MagicList.Items[I])^;
      { 分身可用技能与主人相同 chongchong 2013-09-15 }
      if UserMagic.MagicInfo.btJob = 0 then // 战士技能自动开启
        UserMagic.btKey := VK_F1;
      m_MagicList.Add(UserMagic);
    end;
  end;
end;

procedure TCopyMon.Die();
var
  I: Integer;
begin
  inherited;
  // 分身死亡时立即消失 chongchong 2014-05-21
  if m_Master <> nil then
  begin
    for I := m_Master.m_SlaveList.Count - 1 downto 0 do
    begin
      if m_Master.m_SlaveList.Count <= 0 then
        Break;
      if m_Master.m_SlaveList.Items[I] = Self then
      begin
        m_Master.m_SlaveList.Delete(I);
        Break;
      end;
    end;
  end;
  m_Master := nil;
  MakeGhost;
end;
// 分身断筋处理 piaoyun 2013-10-26

function TCopyMon.RunToNext(nX, nY: Integer): Boolean;
begin
  if m_boDuanJin then
    Result := WalkToNext(nX, nY)
  else
    Result := inherited RunToNext(nX, nY);
end;

procedure TCopyMon.RecalcAbilitys;
begin
  inherited RecalcAbilitys;
end;

procedure TCopyMon.RecalcAbilitys_Add;
var
  Int64Value: Int64;
  nTemp: Integer;
begin
  // 继承属性值比例
  if m_nInheritedPercent <> 100 then
  begin
    if m_Master <> nil then
    begin
      m_WAbil.AC1 := Round(m_Master.m_WAbil.AC1 / 100 * m_nInheritedPercent);
      m_WAbil.AC2 := Round(m_Master.m_WAbil.AC2 / 100 * m_nInheritedPercent);
      m_WAbil.MAC1 := Round(m_Master.m_WAbil.MAC1 / 100 * m_nInheritedPercent);
      m_WAbil.MAC2 := Round(m_Master.m_WAbil.MAC2 / 100 * m_nInheritedPercent);
      m_WAbil.DC1 := Round(m_Master.m_WAbil.DC1 / 100 * m_nInheritedPercent);
      m_WAbil.DC2 := Round(m_Master.m_WAbil.DC2 / 100 * m_nInheritedPercent);
      m_WAbil.MC1 := Round(m_Master.m_WAbil.MC1 / 100 * m_nInheritedPercent);
      m_WAbil.MC2 := Round(m_Master.m_WAbil.MC2 / 100 * m_nInheritedPercent);
      m_WAbil.SC1 := Round(m_Master.m_WAbil.SC1 / 100 * m_nInheritedPercent);
      m_WAbil.SC2 := Round(m_Master.m_WAbil.SC2 / 100 * m_nInheritedPercent);
      // m_WAbil.HP := Round(m_Master.m_WAbil.HP / 100 * m_nInheritedPercent);
      // m_WAbil.MP := Round(m_Master.m_WAbil.MP / 100 * m_nInheritedPercent);
      m_WAbil.MaxHP := Round(m_Master.m_WAbil.MaxHP / 100 * m_nInheritedPercent);
      m_WAbil.MaxMP := Round(m_Master.m_WAbil.MaxMP / 100 * m_nInheritedPercent);
    end
    else
    begin
      m_WAbil.AC1 := Round(m_Abil.AC1 / 100 * m_nInheritedPercent);
      m_WAbil.AC2 := Round(m_Abil.AC2 / 100 * m_nInheritedPercent);
      m_WAbil.MAC1 := Round(m_Abil.MAC1 / 100 * m_nInheritedPercent);
      m_WAbil.MAC2 := Round(m_Abil.MAC2 / 100 * m_nInheritedPercent);
      m_WAbil.DC1 := Round(m_Abil.DC1 / 100 * m_nInheritedPercent);
      m_WAbil.DC2 := Round(m_Abil.DC2 / 100 * m_nInheritedPercent);
      m_WAbil.MC1 := Round(m_Abil.MC1 / 100 * m_nInheritedPercent);
      m_WAbil.MC2 := Round(m_Abil.MC2 / 100 * m_nInheritedPercent);
      m_WAbil.SC1 := Round(m_Abil.SC1 / 100 * m_nInheritedPercent);
      m_WAbil.SC2 := Round(m_Abil.SC2 / 100 * m_nInheritedPercent);
      // m_WAbil.HP := Round(m_Abil.HP / 100 * m_nInheritedPercent);
      // m_WAbil.MP := Round(m_Abil.MP / 100 * m_nInheritedPercent);
      m_WAbil.MaxHP := Round(m_Abil.MaxHP / 100 * m_nInheritedPercent);
      m_WAbil.MaxMP := Round(m_Abil.MaxMP / 100 * m_nInheritedPercent);
    end;
  end;
  if m_boChangeAbility then
  begin
    if m_ChangeAbility.MaxHP <> 0 then
    begin
      // 分身改为指定属性，而不是属性加？？？？ 2021-01-14
      if m_ChangeAbility.boMaxHPPercentage then
        Int64Value := Round(m_WAbil.MaxHP + m_WAbil.MaxHP / 100 * Integer(m_ChangeAbility.MaxHP))
      else
        Int64Value := m_ChangeAbility.MaxHP; // 2021-01-14 m_WAbil.MaxHP + Integer(m_ChangeAbility.MaxHP);
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.MaxHP) then
        Int64Value := High(m_WAbil.MaxHP);
      m_WAbil.MaxHP := Int64Value;
    end;
    if m_ChangeAbility.MaxMP <> 0 then
    begin
      if m_ChangeAbility.boMaxMPPercentage then
        Int64Value := Round(m_WAbil.MaxMP + m_WAbil.MaxMP / 100 * Integer(m_ChangeAbility.MaxMP))
      else
        Int64Value := m_ChangeAbility.MaxMP; // 2021-01-14 m_WAbil.MaxMP + Integer(m_ChangeAbility.MaxMP);
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.MaxMP) then
        Int64Value := High(m_WAbil.MaxMP);
      m_WAbil.MaxMP := Int64Value;
    end;
    if m_ChangeAbility.AC1 <> 0 then
    begin
      if m_ChangeAbility.boAC1Percentage then
        Int64Value := Round(m_WAbil.AC1 + m_WAbil.AC1 / 100 * m_ChangeAbility.AC1)
      else
        Int64Value := m_WAbil.AC1 + m_ChangeAbility.AC1;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.AC1) then
        Int64Value := High(m_WAbil.AC1);
      m_WAbil.AC1 := Int64Value;
    end;
    if m_ChangeAbility.AC2 <> 0 then
    begin
      if m_ChangeAbility.boAC2Percentage then
        Int64Value := Round(m_WAbil.AC2 + m_WAbil.AC2 / 100 * m_ChangeAbility.AC2)
      else
        Int64Value := m_WAbil.AC2 + m_ChangeAbility.AC2;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.AC2) then
        Int64Value := High(m_WAbil.AC2);
      m_WAbil.AC2 := Int64Value;
    end;
    if m_ChangeAbility.MAC1 <> 0 then
    begin
      if m_ChangeAbility.boMAC1Percentage then
        Int64Value := Round(m_WAbil.MAC1 + m_WAbil.MAC1 / 100 * m_ChangeAbility.MAC1)
      else
        Int64Value := m_WAbil.MAC1 + m_ChangeAbility.MAC1;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.MAC1) then
        Int64Value := High(m_WAbil.MAC1);
      m_WAbil.MAC1 := Int64Value;
    end;
    if m_ChangeAbility.MAC2 <> 0 then
    begin
      if m_ChangeAbility.boMAC2Percentage then
        Int64Value := Round(m_WAbil.MAC2 + m_WAbil.MAC2 / 100 * m_ChangeAbility.MAC2)
      else
        Int64Value := m_WAbil.MAC2 + m_ChangeAbility.MAC2;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.MAC2) then
        Int64Value := High(m_WAbil.MAC2);
      m_WAbil.MAC2 := Int64Value;
    end;
    if m_ChangeAbility.DC1 <> 0 then
    begin
      if m_ChangeAbility.boDC1Percentage then
        Int64Value := Round(m_WAbil.DC1 + m_WAbil.DC1 / 100 * m_ChangeAbility.DC1)
      else
        Int64Value := m_WAbil.DC1 + m_ChangeAbility.DC1;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.DC1) then
        Int64Value := High(m_WAbil.DC1);
      m_WAbil.DC1 := Int64Value;
    end;
    if m_ChangeAbility.DC2 <> 0 then
    begin
      if m_ChangeAbility.boDC2Percentage then
        Int64Value := Round(m_WAbil.DC2 + m_WAbil.DC2 / 100 * m_ChangeAbility.DC2)
      else
        Int64Value := m_WAbil.DC2 + m_ChangeAbility.DC2;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.DC2) then
        Int64Value := High(m_WAbil.DC2);
      m_WAbil.DC2 := Int64Value;
    end;
    if m_ChangeAbility.MC1 <> 0 then
    begin
      if m_ChangeAbility.boMC1Percentage then
        Int64Value := Round(m_WAbil.MC1 + m_WAbil.MC1 / 100 * m_ChangeAbility.MC1)
      else
        Int64Value := m_WAbil.MC1 + m_ChangeAbility.MC1;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.MC1) then
        Int64Value := High(m_WAbil.MC1);
      m_WAbil.MC1 := Int64Value;
    end;
    if m_ChangeAbility.MC2 <> 0 then
    begin
      if m_ChangeAbility.boMC2Percentage then
        Int64Value := Round(m_WAbil.MC2 + m_WAbil.MC2 / 100 * m_ChangeAbility.MC2)
      else
        Int64Value := m_WAbil.MC2 + m_ChangeAbility.MC2;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.MC2) then
        Int64Value := High(m_WAbil.MC2);
      m_WAbil.MC2 := Int64Value;
    end;
    if m_ChangeAbility.SC1 <> 0 then
    begin
      if m_ChangeAbility.boSC1Percentage then
        Int64Value := Round(m_WAbil.SC1 + m_WAbil.SC1 / 100 * m_ChangeAbility.SC1)
      else
        Int64Value := m_WAbil.SC1 + m_ChangeAbility.SC1;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.SC1) then
        Int64Value := High(m_WAbil.SC1);
      m_WAbil.SC1 := Int64Value;
    end;
    if m_ChangeAbility.SC2 <> 0 then
    begin
      if m_ChangeAbility.boSC2Percentage then
        Int64Value := Round(m_WAbil.SC2 + m_WAbil.SC2 / 100 * m_ChangeAbility.SC2)
      else
        Int64Value := m_WAbil.SC2 + m_ChangeAbility.SC2;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_WAbil.SC2) then
        Int64Value := High(m_WAbil.SC2);
      m_WAbil.SC2 := Int64Value;
    end;
    if m_ChangeAbility.WalkSpeed <> 0 then
    begin
      if m_ChangeAbility.boWalkSpeedPercentage then
        Int64Value := Round(m_nInitWalkSpeed + m_nWalkSpeed / 100 * m_ChangeAbility.WalkSpeed)
      else
        Int64Value := m_nInitWalkSpeed + m_ChangeAbility.WalkSpeed;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_nWalkSpeed) then
        Int64Value := High(m_nWalkSpeed);
      m_nWalkSpeed := Int64Value;
    end
    else
    begin
      m_nWalkSpeed := m_nInitWalkSpeed;
    end;
    if m_ChangeAbility.NextHitTime <> 0 then
    begin
      if m_ChangeAbility.boNextHitTimePercentage then
        Int64Value := Round(m_nInitNextHitTime + m_nNextHitTime / 100 * m_ChangeAbility.NextHitTime)
      else
        Int64Value := m_nInitNextHitTime + m_ChangeAbility.NextHitTime;
      if Int64Value < 0 then
        Int64Value := 0
      else if Int64Value > High(m_nNextHitTime) then
        Int64Value := High(m_nNextHitTime);
      m_nNextHitTime := Int64Value;
    end
    else
    begin
      m_nNextHitTime := m_nInitNextHitTime;
    end;
    if m_boChangeAbilitySetHMP then
    begin
      if m_ChangeAbility.HP <> 0 then
      begin
        if m_ChangeAbility.boHPPercentage then
          Int64Value := Round(m_WAbil.HP + m_WAbil.HP / 100 * Integer(m_ChangeAbility.HP))
        else
          Int64Value := m_WAbil.HP + Integer(m_ChangeAbility.HP);
        if Int64Value < 0 then
          Int64Value := 0
        else if Int64Value > High(m_WAbil.HP) then
          Int64Value := High(m_WAbil.HP);
        m_WAbil.HP := Int64Value;
        if m_WAbil.HP >= m_WAbil.MaxHP then
          m_WAbil.HP := m_WAbil.MaxHP;
      end;
      if m_ChangeAbility.MP <> 0 then
      begin
        if m_ChangeAbility.boMPPercentage then
          Int64Value := Round(m_WAbil.MP + m_WAbil.MP / 100 * Integer(m_ChangeAbility.MP))
        else
          Int64Value := m_WAbil.MP + Integer(m_ChangeAbility.MP);
        if Int64Value < 0 then
          Int64Value := 0
        else if Int64Value > High(m_WAbil.MP) then
          Int64Value := High(m_WAbil.MP);
        m_WAbil.MP := Int64Value;
        if m_WAbil.MP >= m_WAbil.MaxHP then
          m_WAbil.MP := m_WAbil.MaxMP;
      end;
    end;
  end;
  // 修正npc加攻击速度不能和装备速度叠加 chongchong 2018-05-18
  nTemp := m_nNpcAttackSpeed + m_nHitSpeed;
  if nTemp >= High(m_nAttackSpeed) then
    nTemp := High(m_nAttackSpeed)
  else if nTemp <= Low(m_nAttackSpeed) then
    nTemp := Low(m_nAttackSpeed);
  m_nAttackSpeed := nTemp;
  RefGameSpeed;
end;

procedure TCopyMon.RecalcLevelAbilitys(IsSysDef: Boolean);
begin
  inherited RecalcLevelAbilitys(IsSysDef);
end;
// 分身到时消灭要向英雄一样有特效

procedure TCopyMon.MakeGhost;
begin
  SendRefMsg(RM_HEROLOGOUT, 0, NativeInt(Self), m_nCurrX, m_nCurrY, '');
  inherited;
end;
{ TMoonObjectEx }

constructor TMoonObjectEx.Create();
begin
  try
    inherited;
    m_nViewRange := Max(g_Config.nMonthSpiritAttackRange, 4); // 月灵可视范围
    m_nAttackRange := Max(g_Config.nMonthSpiritAttackRange, 4); // 攻击范围
    m_boDupMode := False;
    m_dwThinkTick := MyGetTickCount();
    m_btRaceServer := RC_MOONOBJECT;
    FFAvoidTargetTick := MyGetTickCount;
    FDelTargetSetTick := MyGetTickCount - 4000;
  except
    MainOutMessage('[Exception] TMoonObjectEx:Create');
  end;
end;

destructor TMoonObjectEx.Destroy;
begin
  inherited;
end;
// 高倍攻击-重击

procedure TMoonObjectEx.HighAttack();
var
  nPower: Integer;
  WAbil: pTAbility;
  _Index: Integer;
  MonHPProgress: pTMonHPProgress;
  Player: TPlayObject;
begin
  // 获取下一个方位
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
  SendRefMsg(RM_LIGHTING, 200, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  WAbil := @m_WAbil;
  {
    nPower := SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1;
    if nPower > 0 then
    nPower := Random(nPower);
    nPower := nPower + LoWord(WAbil.DC);
    // nPower := (Random(Integer(HiWord(WAbil.MC) - LoWord(WAbil.MC)) + 1) + LoWord(WAbil.MC));
    // 这段似乎废话，不是只有道士可以召唤么？ -- piaoyun 2013-6-23
    (*if m_Master <> nil then
    case m_Master.m_btJob of
    0:
    begin
    nMPower := SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.DC); ;
    end;
    1:
    begin
    nMPower := SmallInt(HiWord(WAbil.MC) - LoWord(WAbil.MC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.MC);
    end;
    2:
    begin
    nMPower := SmallInt(HiWord(WAbil.SC) - LoWord(WAbil.SC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.SC);
    end;
    end; *)
    nMPower := SmallInt(HiWord(WAbil.SC) - LoWord(WAbil.SC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.SC);
    nPower := _MAX(Round((nPower + nMPower) * 3 / 4), 1);
    nPower := Round(nPower * (g_Config.nMonthSpiritHighPowerRate / 100));
  }
  nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
  nPower := Round(nPower * (g_Config.nMonthSpiritHighPowerRate / 100));
  nPower := GetPowerRateAdd(m_TargetCret, nPower);
  SendDelayMsg(Self, RM_DELAYMAGIC, nPower, MakeLong(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY), 2,
    NativeInt(m_TargetCret), '', 800);
  // 宝宝攻击也要显示大血条 2020-07-14 16:08:12
  _Index := g_MonHPProgressList.IndexOf(m_TargetCret.m_sCharName);
  if _Index >= 0 then
  begin
    MonHPProgress := pTMonHPProgress(g_MonHPProgressList.Objects[_Index]);
    Player := nil;
    if (m_TargetCret.m_ExpHitter <> nil) then
    begin
      if (m_TargetCret.m_ExpHitter.m_btRaceServer = RC_PLAYOBJECT) then
        Player := TPlayObject(m_TargetCret.m_ExpHitter)
      else if (m_TargetCret.m_ExpHitter.Master <> nil) and (m_TargetCret.m_ExpHitter.Master.m_btRaceServer = RC_PLAYOBJECT) then
        Player := TPlayObject(m_TargetCret.m_ExpHitter.Master);
    end;
    if Player <> nil then
    begin
      MonHPProgress.m_ExpHinterName := Player.m_sCharName;
      m_DefMsg := MakeDefaultMsg(SM_SENDBIGHPPROGRESS, NativeInt(Self), 0, 0, 0);
      Player.SendSocketEx(@m_DefMsg, PAnsiChar(MonHPProgress), SizeOf(TMonHPProgress));
    end
  end;
end;
// 低倍攻击-轻击

procedure TMoonObjectEx.LowAttack();
var
  nPower: Integer;
  WAbil: pTAbility;
  _Index: Integer;
  MonHPProgress: pTMonHPProgress;
  Player: TPlayObject;
begin
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
  SendRefMsg(RM_LIGHTING, 199, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  WAbil := @m_WAbil;
  {
    nPower := SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1;
    if nPower > 0 then
    nPower := Random(nPower);
    nPower := nPower + LoWord(WAbil.DC);
    // nPower := (Random(Integer(HiWord(WAbil.MC) - LoWord(WAbil.MC)) + 1) + LoWord(WAbil.MC));
    nMPower := 0;
    // 这段似乎废话，不是只有道士可以召唤么？ -- piaoyun 2013-6-23
    (*if m_Master <> nil then
    case m_Master.m_btJob of
    0:
    begin
    nMPower := SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.DC); ;
    end;
    1:
    begin
    nMPower := SmallInt(HiWord(WAbil.MC) - LoWord(WAbil.MC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.MC);
    end;
    2:
    begin
    nMPower := SmallInt(HiWord(WAbil.SC) - LoWord(WAbil.SC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.SC);
    end;
    end; *)
    if m_Master <> nil then
    begin
    nMPower := SmallInt(HiWord(WAbil.SC) - LoWord(WAbil.SC)) + 1;
    if nMPower > 0 then
    nMPower := Random(nMPower);
    nMPower := nMPower + LoWord(WAbil.SC);
    end;
    nPower := _MAX(Round((nPower + nMPower) * 3 / 4), 1);
  }
  nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
  nPower := GetPowerRateAdd(m_TargetCret, nPower);
  SendDelayMsg(Self, RM_DELAYMAGIC, nPower, MakeLong(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY), 2,
    NativeInt(m_TargetCret), '', 800);
  // 宝宝攻击也要显示大血条 2020-07-14 16:08:12
  _Index := g_MonHPProgressList.IndexOf(m_TargetCret.m_sCharName);
  if _Index >= 0 then
  begin
    MonHPProgress := pTMonHPProgress(g_MonHPProgressList.Objects[_Index]);
    Player := nil;
    if (m_TargetCret.m_ExpHitter <> nil) then
    begin
      if (m_TargetCret.m_ExpHitter.m_btRaceServer = RC_PLAYOBJECT) then
        Player := TPlayObject(m_TargetCret.m_ExpHitter)
      else if (m_TargetCret.m_ExpHitter.Master <> nil) and (m_TargetCret.m_ExpHitter.Master.m_btRaceServer = RC_PLAYOBJECT) then
        Player := TPlayObject(m_TargetCret.m_ExpHitter.Master);
    end;
    if Player <> nil then
    begin
      MonHPProgress.m_ExpHinterName := Player.m_sCharName;
      m_DefMsg := MakeDefaultMsg(SM_SENDBIGHPPROGRESS, NativeInt(Self), 0, 0, 0);
      Player.SendSocketEx(@m_DefMsg, PAnsiChar(MonHPProgress), SizeOf(TMonHPProgress));
    end
  end;
end;

function TMoonObjectEx.AttackTarget: Boolean;
begin
  Result := False;
  try
    if m_TargetCret <> nil then
    begin
      { TODO -ochongchong -c新增 : 英雄召唤的宝宝在安全区停止攻击 【2013-08-15】 }
      if (m_Master <> nil) and (m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
        (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and m_TargetCret.InSafeZone then
        Exit;
      if m_TargetCret.m_boDeath or m_TargetCret.m_boGhost then
        Exit;
      // 判断攻击范围
      if (m_TargetCret.m_PEnvir = m_PEnvir) and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= m_nAttackRange) and
        (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= m_nAttackRange) then
      begin
        if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
        begin
          m_dwHitTick := MyGetTickCount();
          m_nHitDelay := 0;
          m_dwTargetFocusTick := MyGetTickCount();
          FlyAttack(m_TargetCret); // 飞行攻击
          BreakHolySeizeMode();
        end;
        // 月灵走到了攻击范围，不用再走了 chongchong 2018-05-30
        if (m_TargetCret.m_nCurrX = m_nTargetX) and (m_TargetCret.m_nCurrY = m_nTargetY) then
        begin
          m_nTargetX := -1;
          m_nTargetY := -1;
        end;
        Result := True;
      end
      else
      begin
        if m_TargetCret.m_PEnvir = m_PEnvir then
        begin
          SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        end
        else
        begin
          DelTargetCreat();
        end;
      end;
    end;
  except
    MainOutMessage('[Exception] TMoonObjectEx:AttackTarget:Boolean');
  end;
end;

function TMoonObjectEx.Think: Boolean;
var
  nOldX, nOldY: Integer;
begin
  Result := False;
  try
    if (MyGetTickCount - m_dwThinkTick) > 3 * 1000 then
    begin
      m_dwThinkTick := MyGetTickCount();
      if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then
        m_boDupMode := True;
      if not IsProperTarget { FFFF4 } (m_TargetCret) then
      begin
        if m_TargetCret <> nil then
        begin
          m_TargetCret := nil;
          FDelTargetSetTick := MyGetTickCount;
        end;
      end;
    end; // 004A8ED2
    if m_boDupMode then
    begin
      nOldX := m_nCurrX;
      nOldY := m_nCurrY;
      WalkTo(Random(8), False);
      if (nOldX <> m_nCurrX) or (nOldY <> m_nCurrY) then
      begin
        m_boDupMode := False;
        Result := True;
      end;
    end;
  except
    MainOutMessage('[Exception] TMoonObjectEx:Think');
  end;
end;

procedure TMoonObjectEx.FlyAttack(Target: TBaseObject);
begin
  try
    if AttackTarget then
    begin
      if Random(g_Config.nMonthSpiritHighAttackRate) = 0 then
        HighAttack()
      else
        LowAttack();
    end;
  except
    MainOutMessage('[Exception] TMoonObjectEx:FlyAttack');
  end;
end;

procedure TMoonObjectEx.RecalcAbilitys;
begin
  try
    inherited;
    // 走路速度 20090106 由DB设置的走路速度控制
    if m_Master <> nil then
      m_nWalkSpeed := _MAX(200, m_nWalkSpeedDB - m_btSlaveMakeLevel * 50);
    m_dwWalkTick := MyGetTickCount;
    m_nWalkDelay := 2000;
    // 修复数据库中月灵.attack_spd = 0时，召唤月灵攻击怪物时M2重启 chongchong 2014-07-05
    if m_nNextHitTime < 500 then
      m_nNextHitTime := 500;
    {
      // 月灵速度--数据库基准 piaoyun 2013-12-18
      if m_nNextHitTimeOrg = 0 then
      m_nNextHitTimeOrg := m_nNextHitTime;
      if m_nWalkSpeedOrg = 0 then
      m_nWalkSpeedOrg := m_nWalkSpeed;
      ////////////////////////////////////////////////////////////////////////////
      m_nNextHitTime := m_nNextHitTimeOrg - m_btSlaveMakeLevel * 100;
      m_nWalkSpeed := m_nWalkSpeedOrg - m_btSlaveMakeLevel * 50;
      m_dwWalkTick := MyGetTickCount;
      m_nWalkDelay := 2000;
      // 修复数据库中月灵.attack_spd = 0时，召唤月灵攻击怪物时M2重启 chongchong 2014-07-05
      if m_nNextHitTime < 0 then m_nNextHitTime := 0;
    }
  except
    MainOutMessage('[Exception] TMoonObjectEx:RecalcAbilitys');
  end;
end;

procedure TMoonObjectEx.SearchTarget();
begin
  { TODO -opiaoyun -c修复 : 修复月灵召唤后出错【2013-6-6】 }
  if (g_Config.boMonthSpiritAttackSame) and (m_Master <> nil) and (m_Master.m_TargetCret <> nil)
  { and (not m_Master.m_TargetCret.m_boDeath) } then
  begin
    if (not m_Master.m_TargetCret.m_boDeath) then
    begin
      if { (m_TargetCret = nil) and } (not m_Master.m_boSlaveRelax) then
      begin
        if (m_Master.m_TargetCret <> nil) and (m_Master.m_TargetCret.m_Master <> m_Master) and
          (m_Master.m_TargetCret.Master <> Master) then
        begin
          SetTargetCreat(m_Master.m_TargetCret);
        end;
      end;
    end;
  end
  else
    inherited;
  (*
    if (g_Config.boMonthSpiritAttackSame) and (m_Master <> nil) and (not m_Master.m_TargetCret.m_boDeath) then
    begin
    if (m_TargetCret = nil) and (not m_Master.m_boSlaveRelax) then begin
    if (m_Master.m_TargetCret <> nil) and (m_Master.m_TargetCret.m_Master <> m_Master) and (m_Master.m_TargetCret.Master <> Master) then
    SetTargetCreat(m_Master.m_TargetCret);
    end;
    end else inherited;
  *)
end;

procedure TMoonObjectEx.Run;
var
  nX, nY: Integer;
  nDir1: Integer;
  btArrDirs: array [0 .. 6] of Byte;
  nTargetX, nTargetY, nIndex: Integer;
  IsAttack: Boolean;
begin
  try
    if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and (m_wStatusTimeArr[POISON_STONE
      { 5 0x6A } ] = 0) then
    begin
      if (m_TargetCret <> nil) and (m_TargetCret.m_boDeath or m_TargetCret.m_boGhost) then
      begin
        DelTargetCreat;
        DeleteMsg(RM_DELAYMAGIC);
        DeleteMsg(RM_MAGSTRUCK);
      end;
      // 休息 chongchong 2014-12-09
      if (m_Master <> nil) and (m_Master.m_boSlaveRelax) then
      begin
        DelTargetCreat;
        inherited;
        Exit;
      end;
      // 修正英雄跟随时，英雄宝宝月灵还攻击目标 chongchong 2018-07-26 14:56:23
      if (m_TargetCret <> nil) and (m_Master <> nil) and (m_Master.m_btRaceServer = RC_HEROOBJECT) and
        (THeroObject(m_Master).m_btAttackMode in [1, 2]) then
      begin
        DelTargetCreat;
        inherited;
        Exit;
      end;
      if Think then
      begin
        // 检测是否重叠，重叠则随机移动一格
        inherited;
        Exit;
      end;
      // 当月灵主人有攻击目标后则和主人攻击同一目标
      if (g_Config.boMonthSpiritAttackSame) and (m_Master <> nil) and (m_Master.m_TargetCret <> nil) and
        (not m_Master.m_TargetCret.m_boDeath) and
      // 目标和自己不是同一个主要，才可以攻击
        (m_Master.m_TargetCret.Master <> Master) then
      begin
        // 修正选中宝宝不攻击人物选项，对月灵效 2021-01-15
        IsAttack := True;
        if (m_Master.m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) then
        begin
          if g_Config.boSlaveNotAttackHuman then
            IsAttack := False
          else if (m_Master.m_TargetCret.m_PEnvir <> nil) and (m_Master.m_TargetCret.m_PEnvir.m_boSlaveNotAttackHuman) then
            IsAttack := False;
        end
        else if (m_Master.m_TargetCret.m_btRaceServer = RC_HEROOBJECT) then
        begin
          if g_Config.boSlaveNotAttackHero then
            IsAttack := False
          else if (m_Master.m_TargetCret.m_PEnvir <> nil) and (m_Master.m_TargetCret.m_PEnvir.m_boSlaveNotAttackHero) then
            IsAttack := False;
        end;
        if IsAttack and (not((m_Master.m_btRaceServer = RC_HEROOBJECT) and (THeroObject(m_Master).m_btAttackMode in [1, 2]))) then
          SetTargetCreat(m_Master.m_TargetCret);
      end
      else
      begin
        if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or
          (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil)) then
        begin
          m_dwSearchEnemyTick := MyGetTickCount();
          SearchTarget(); // 搜索目标
        end;
      end;
      if m_boWalkWaitLocked then
      begin
        if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
        begin
          m_boWalkWaitLocked := False;
        end;
      end;
      if (m_TargetCret <> nil) and (m_Master <> nil) and (m_Master.InSafeZone) and
        ((m_TargetCret.m_btRaceServer in [0, 1]) or ((m_TargetCret.Master <> nil) and (m_TargetCret.Master.m_btRaceServer in [0,
        1]))) then
      begin
        DelTargetCreat;
      end;
      if not m_boRunAwayMode then
      begin
        if not m_boNoAttackMode then
        begin
          if m_TargetCret <> nil then
          begin
            if AttackTarget { FFEB } then
            begin
              // inherited;
              // exit;
            end;
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
        // 004A9151
        if not m_boRunAwayMode then
        begin
          if not m_boNoAttackMode then
          begin
            if m_TargetCret <> nil then
            begin
              if (MyGetTickCount - FFAvoidTargetTick) >= 2400 then
              begin
                if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 3) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 3) then
                begin
                  if FIsAvoidTargetFirst then
                  begin
                    nDir1 := (FAvoidTargetDir + 1) mod 8;
                    if GetGotoXY(nDir1, nTargetX, nTargetY) then
                    begin
                      if WalkTo(nDir1, False) then
                      begin
                        FIsAvoidTargetFirst := False;
                        FFAvoidTargetTick := MyGetTickCount;
                        m_nTargetX := -1;
                        inherited;
                        Exit;
                      end;
                    end;
                  end;
                  nDir1 := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);
                  if (nDir1 in [0, 2, 4, 6]) then
                  begin
                    btArrDirs[0] := (nDir1 + 1) mod 8;
                    btArrDirs[1] := (nDir1 + 2) mod 8;
                    btArrDirs[2] := (nDir1 + 6) mod 8;
                    btArrDirs[3] := (nDir1 + 0) mod 8;
                    btArrDirs[4] := (nDir1 + 7) mod 8;
                    btArrDirs[5] := (nDir1 + 3) mod 8;
                    btArrDirs[6] := (nDir1 + 5) mod 8;
                  end
                  else
                  begin
                    btArrDirs[0] := (nDir1 + 2) mod 8;
                    btArrDirs[1] := (nDir1 + 1) mod 8;
                    btArrDirs[2] := (nDir1 + 0) mod 8;
                    btArrDirs[3] := (nDir1 - 2 + 8) mod 8;
                    btArrDirs[4] := (nDir1 - 1 + 8) mod 8;
                    btArrDirs[5] := (nDir1 + 3) mod 8;
                    btArrDirs[6] := (nDir1 - 3 + 8) mod 8;
                  end;
                  for nIndex := Low(btArrDirs) to High(btArrDirs) do
                  begin
                    nTargetX := m_nCurrX;
                    nTargetY := m_nCurrY;
                    if GetGotoXY(btArrDirs[nIndex], nTargetX, nTargetY) then
                    begin
                      if WalkTo(btArrDirs[nIndex], False) then
                      begin
                        FAvoidTargetDir := btArrDirs[nIndex];
                        FIsAvoidTargetFirst := True;
                        FFAvoidTargetTick := MyGetTickCount;
                        m_nTargetX := -1;
                        inherited;
                        Exit;
                      end;
                    end;
                  end;
                  inherited;
                  Exit;
                end
                else
                begin
                  FIsAvoidTargetFirst := True;
                end;
              end;
            end
            else
            begin
              m_nTargetX := -1;
              if m_boMission and (Length(m_nMissionPoints) > 0) and (m_nMissionPointIndex < Length(m_nMissionPoints)) then
              begin
                if (m_nMissionPointIndex < 0) then
                  m_nMissionPointIndex := 0;
                if (abs(m_nCurrX - m_nMissionPoints[m_nMissionPointIndex].X) <= 3) and
                  (abs(m_nCurrY - m_nMissionPoints[m_nMissionPointIndex].Y) <= 3) then
                begin
                  Inc(m_nMissionPointIndex);
                  if m_nMissionPointIndex >= Length(m_nMissionPoints) then
                    m_nMissionPointIndex := Length(m_nMissionPoints) - 1;
                end;
                m_nTargetX := m_nMissionPoints[m_nMissionPointIndex].X;
                m_nTargetY := m_nMissionPoints[m_nMissionPointIndex].Y;
              end;
            end;
          end;
          if m_Master <> nil then
          begin
            if (m_TargetCret = nil) and (tick_diff(FDelTargetSetTick, MyGetTickCount) >= 2000) then
            begin
              m_Master.GetBackPosition(nX, nY);
              if (abs(m_nTargetX - nX) > 1) or (abs(m_nTargetY - nY { nX } ) > 1) then
              begin
                m_nTargetX := nX;
                m_nTargetY := nY;
                if (abs(m_nCurrX - nX) <= 2) and (abs(m_nCurrY - nY) <= 2) then
                begin
                  if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
                  begin
                    m_nTargetX := m_nCurrX;
                    m_nTargetY := m_nCurrY;
                  end
                end;
              end;
            end;
            if (not m_Master.m_boSlaveRelax) and ((m_PEnvir <> m_Master.m_PEnvir) or (abs(m_nCurrX - m_Master.m_nCurrX) > 20) or
              (abs(m_nCurrY - m_Master.m_nCurrY) > 20)) then
            begin
              SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
            end;
          end;
        end
        else
        begin
          if (m_dwRunAwayTime > 0) and ((MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime) then
          begin
            m_boRunAwayMode := False;
            m_dwRunAwayTime := 0;
          end;
        end;
        if (m_Master <> nil) and m_Master.m_boSlaveRelax then
        begin
          inherited;
          Exit;
        end;
        if m_nTargetX <> -1 then
        begin
          GotoTargetXY();
        end
        else
        begin
          if m_TargetCret = nil then
            Wondering();
        end;
      end;
    end;
  except
    MainOutMessage('[Exception] TMonster:Run');
  end;
  inherited;
end;
(*
  function TMoonObjectEx.Operate(ProcessMsg: pTProcessMessage): Boolean;
  begin
  // Result:=False;
  if ProcessMsg.wIdent = RM_STRUCK then
  begin
  if (ProcessMsg.BaseObject = Self) and (TBaseObject(ProcessMsg.nParam3 {AttackBaseObject}) <> nil) and (TBaseObject(ProcessMsg.nParam3 {AttackBaseObject}) <> Self) then
  begin
  SetLastHiter(TBaseObject(ProcessMsg.nParam3 {AttackBaseObject}));
  Struck(TBaseObject(ProcessMsg.nParam3 {AttackBaseObject})); {0FFEC}
  BreakHolySeizeMode();
  if (m_Master <> nil) and
  (TBaseObject(ProcessMsg.nParam3) <> m_Master) and
  (TBaseObject(ProcessMsg.nParam3).m_btRaceServer = RC_PLAYOBJECT) then
  begin
  m_Master.SetPKFlag(TBaseObject(ProcessMsg.nParam3));
  end;
  if g_Config.boMonSayMsg then MonsterSayMsg(TBaseObject(ProcessMsg.nParam3), s_UnderFire);
  end;
  Result := True;
  end else
  begin
  Result := inherited Operate(ProcessMsg);
  end;
  end;
*)

procedure TMoonObjectEx.Initialize;
begin
  inherited;
end;

function THumMon.Walk(nIdent: Integer): Boolean;
begin
  if CanMove then
  begin
    Result := inherited Walk(nIdent);
  end
  else
    Result := False;
end;

function THumMon.GotoNext: Boolean;
begin
  Result := inherited GotoNext();
  m_dwMoveTimeTick := MyGetTickCount;
end;

function THumMon.GotoNext(Path: TPath): Boolean;
begin
  Result := inherited GotoNext(Path);
  m_dwMoveTimeTick := MyGetTickCount;
end;

function THumMon.GotoNext(nX, nY: Integer; boRun: Boolean): Boolean;
begin
  Result := inherited GotoNext(nX, nY, boRun);
  m_dwMoveTimeTick := MyGetTickCount;
end;
// 人形怪断筋处理 piaoyun 2013-10-26

function THumMon.RunToNext(nX, nY: Integer): Boolean;
begin
  if m_boDuanJin or m_boCobwebWindingStatus then
    Result := inherited WalkToNext(nX, nY)
  else
    Result := inherited RunToNext(nX, nY);
end;

function THumMon.WalkToNext(nX, nY: Integer): Boolean;
begin
  Result := inherited WalkToNext(nX, nY);
  m_dwMoveTimeTick := MyGetTickCount;
end;

procedure THumMon.GotoTargetXY();
begin
  inherited GotoTargetXY;
  m_dwMoveTimeTick := MyGetTickCount;
end;

procedure THumMon.RunToTargetXY;
begin
  if m_boDuanJin or m_boCobwebWindingStatus then
    GotoTargetXY
  else
    inherited RunToTargetXY;
  m_dwMoveTimeTick := MyGetTickCount;
end;

procedure THumMon.Wondering();
begin
  Exit;
  {
    if (Random(100) = 0) then
    if (Random(4) = 1) then
    TurnTo(Random(8))
    else
    WalkTo(m_btDirection, False);
  }
end;

function THumMon.TakeItemsToPlayer(Player: TPlayObject): Boolean;
var
  Idx: Integer;
  Count: Cardinal;
  StdItem: pTStdItem;
  UserItem, UserItem32: pTUserItem;
  OverLapItem: pTUserItem;
  Msg, sUserItemName, sStdItemName: string;
begin
  Result := False;
  if Player = nil then
    Exit;
  if m_PEnvir = nil then
    Exit;
  case m_nButchChargeMode of
    0:
      Count := Player.m_nGold;
    1:
      Count := Player.m_nGameGold;
    2:
      Count := Player.m_nGameDiamond;
    3:
      Count := Player.m_nGameGird;
  else
    Count := Player.m_nGold;
  end;
  if Count >= m_nButchChargeCount then
  begin
    // 挖取扣费
    if (m_nButchChargeCount > 0) and (not m_boOnlyButchItemDelGold) then
    begin
      case m_nButchChargeMode of
        0:
          begin
            Player.DecGold(m_nButchChargeCount);
            Player.GoldChanged;
            AddGameDataLog(LOG_GoldChange, LOG_ActionNone, Player, sSTRING_GOLDNAME, 0, m_sCharName, Player.m_nGold,
              -m_nButchChargeCount, '挖取扣费');
          end;
        1:
          begin
            Player.DecGameGold(m_nButchChargeCount);
            Player.GameGoldChanged;
            AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, Player, g_Config.sGameGoldName, 0, m_sCharName, Player.m_nGameGold,
              -m_nButchChargeCount, '挖取扣费');
          end;
        2:
          begin
            Player.DecGameDiamond(m_nButchChargeCount);
            Player.NewGamePointChanged;
            AddGameDataLog(LOG_GameDiamondChange, LOG_ActionNone, Player, g_Config.sGameDiamondName, 0, m_sCharName,
              Player.m_nGameDiamond, -m_nButchChargeCount, '挖取扣费');
          end;
        3:
          begin
            Player.DecGameGird(m_nButchChargeCount);
            Player.NewGamePointChanged;
            AddGameDataLog(LOG_GameGirdChange, LOG_ActionNone, Player, g_Config.sGameGirdName, 0, m_sCharName, Player.m_nGameGird,
              -m_nButchChargeCount, '挖取扣费');
          end;
      else
        begin
          Player.DecGold(m_nButchChargeCount);
          Player.GoldChanged;
          AddGameDataLog(LOG_GoldChange, LOG_ActionNone, Player, sSTRING_GOLDNAME, 0, m_sCharName, Player.m_nGold,
            -m_nButchChargeCount, '挖取扣费');
        end;
      end;
    end;
    StdItem := nil;
    UserItem32 := nil;
    if m_boButchUseItem and (Random(m_nButchUseItemRate) <= 0) then
    begin
      Idx := Random(High(THumanUseItems) + 1);
      UserItem := @m_UseItems[Idx];
      if (UserItem <> nil) and (UserItem.wIndex > 0) then
      begin
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if StdItem <> nil then
        begin
          New(UserItem32);
          UserItem32^ := UserItem^;
          m_UseItems[Idx].wIndex := 0;
        end;
      end;
    end;
    if (UserItem32 = nil) and m_boButchListItem and (m_ButchItemList.Count > 0) then
    begin
      Idx := Random(m_ButchItemList.Count);
      if (Idx >= 0) and (Idx < m_ButchItemList.Count) then
      begin
        UserItem32 := m_ButchItemList.Items[Idx];
        m_ButchItemList.Delete(Idx);
        StdItem := UserEngine.GetStdItem(UserItem32.wIndex);
        if (UserItem32 <> nil) and (StdItem = nil) then
        begin
          Dispose(UserItem32);
          UserItem32 := nil;
        end;
      end;
    end;
    if (StdItem <> nil) and (UserItem32 <> nil) then
    begin
      OverLapItem := OverLapItems(Player, StdItem, UserItem32.Dura); // 物品重叠
      if (OverLapItem = nil) then
      begin
        UserItem32.ItemFrom.ItemForm := ifButchItem;
        UserItem32.ItemFrom.sMapName := m_PEnvir.sMapDesc;
        UserItem32.ItemFrom.sMonName := FilterShowName(m_sCharName);
        UserItem32.ItemFrom.sMakerName := Player.m_sCharName;
        UserItem32.ItemFrom.DateTime := Now();
        if Player.AddItemToBag(UserItem32) then
        begin
          // 挖取扣费
          if (m_nButchChargeCount > 0) and (m_boOnlyButchItemDelGold) then
          begin
            case m_nButchChargeMode of
              0:
                begin
                  Player.DecGold(m_nButchChargeCount);
                  Player.GoldChanged;
                  AddGameDataLog(LOG_GoldChange, LOG_ActionNone, Player, sSTRING_GOLDNAME, 0, m_sCharName, Player.m_nGold,
                    -m_nButchChargeCount, '挖取扣费');
                end;
              1:
                begin
                  Player.DecGameGold(m_nButchChargeCount);
                  Player.GameGoldChanged;
                  AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, Player, g_Config.sGameGoldName, 0, m_sCharName,
                    Player.m_nGameGold, -m_nButchChargeCount, '挖取扣费');
                end;
              2:
                begin
                  Player.DecGameDiamond(m_nButchChargeCount);
                  Player.NewGamePointChanged;
                  AddGameDataLog(LOG_GameDiamondChange, LOG_ActionNone, Player, g_Config.sGameDiamondName, 0, m_sCharName,
                    Player.m_nGameDiamond, -m_nButchChargeCount, '挖取扣费');
                end;
              3:
                begin
                  Player.DecGameGird(m_nButchChargeCount);
                  Player.NewGamePointChanged;
                  AddGameDataLog(LOG_GameGirdChange, LOG_ActionNone, Player, g_Config.sGameGirdName, 0, m_sCharName,
                    Player.m_nGameGird, -m_nButchChargeCount, '挖取扣费');
                end;
            else
              begin
                Player.DecGold(m_nButchChargeCount);
                Player.GoldChanged;
                AddGameDataLog(LOG_GoldChange, LOG_ActionNone, Player, sSTRING_GOLDNAME, 0, m_sCharName, Player.m_nGold,
                  -m_nButchChargeCount, '挖取扣费');
              end;
            end;
          end;
          Player.SendAddItem(UserItem32);
          Result := True;
          if StdItem.NeedIdentify = 1 then
          begin
            AddGameDataLog(LOG_ButchItem, LOG_ActionNone, Player, StdItem.Name, UserItem32.MakeIndex, m_sCharName);
          end;
          { TODO -ochongchong -c新增 : 物品规则 - 挖取提示【2013-07-27】 }
          if (Length(StdItem.Name) > 0) and g_ItemRules.Get(UserItem32.wIndex, 19) then
          begin
            Msg := g_sButchItemHintMsg;
            sStdItemName := ProcessItemName(StdItem.Name);
            if (UserItem32.btValue[13] = 1) and (Length(UserItem32.Name) > 0) then
              sUserItemName := ProcessItemName(UserItem32.Name)
            else
              sUserItemName := sStdItemName;
            if Pos('%showdbitem', LowerCase(Msg)) > 0 then
            begin
              SetSayItem(UserItem32);
              Msg := WideReplaceText(Msg, '%ShowDBItem', Format('{[%s]/%d}', [sStdItemName, UserItem32.MakeIndex]));
            end;
            if Pos('%showitem', LowerCase(Msg)) > 0 then
            begin
              SetSayItem(UserItem32);
              Msg := WideReplaceText(Msg, '%ShowItem', Format('{[%s]/%d}', [sUserItemName, UserItem32.MakeIndex]));
            end;
            Msg := AnsiReplaceText(Msg, '%s', FilterShowName(m_sCharName));
            Msg := AnsiReplaceText(Msg, '%item', sUserItemName);
            Msg := AnsiReplaceText(Msg, '%dbitem', sStdItemName);
            Msg := AnsiReplaceText(Msg, '%name', Player.m_sCharName);
            Msg := AnsiReplaceText(Msg, '%m', m_PEnvir.sMapDesc);
            Msg := AnsiReplaceText(Msg, '%x', IntToStr(m_nCurrX));
            Msg := AnsiReplaceText(Msg, '%y', IntToStr(m_nCurrY));
            UserEngine.SendBroadCastMsg(Msg, g_Config.btDropItemFColor, g_Config.btDropItemBColor, t_hint); // sysmsg
          end;
        end
        else
          Dispose(UserItem32);
      end
      else
      begin
        // 挖取扣费
        if (m_nButchChargeCount > 0) and (m_boOnlyButchItemDelGold) then
        begin
          case m_nButchChargeMode of
            0:
              begin
                Player.DecGold(m_nButchChargeCount);
                Player.GoldChanged;
                AddGameDataLog(LOG_GoldChange, LOG_ActionNone, Player, sSTRING_GOLDNAME, 0, m_sCharName, Player.m_nGold,
                  -m_nButchChargeCount, '挖取扣费');
              end;
            1:
              begin
                Player.DecGameGold(m_nButchChargeCount);
                Player.GameGoldChanged;
                AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, Player, g_Config.sGameGoldName, 0, m_sCharName,
                  Player.m_nGameGold, -m_nButchChargeCount, '挖取扣费');
              end;
            2:
              begin
                Player.DecGameDiamond(m_nButchChargeCount);
                Player.NewGamePointChanged;
                AddGameDataLog(LOG_GameDiamondChange, LOG_ActionNone, Player, g_Config.sGameDiamondName, 0, m_sCharName,
                  Player.m_nGameDiamond, -m_nButchChargeCount, '挖取扣费');
              end;
            3:
              begin
                Player.DecGameGird(m_nButchChargeCount);
                Player.NewGamePointChanged;
                AddGameDataLog(LOG_GameGirdChange, LOG_ActionNone, Player, g_Config.sGameGirdName, 0, m_sCharName,
                  Player.m_nGameGird, -m_nButchChargeCount, '挖取扣费');
              end;
          else
            begin
              Player.DecGold(m_nButchChargeCount);
              Player.GoldChanged;
              AddGameDataLog(LOG_GoldChange, LOG_ActionNone, Player, sSTRING_GOLDNAME, 0, m_sCharName, Player.m_nGold,
                -m_nButchChargeCount, '挖取扣费');
            end;
          end;
        end;
        Result := True;
        if StdItem.NeedIdentify = 1 then
        begin
          AddGameDataLog(LOG_ButchItem, LOG_ActionNone, Player, StdItem.Name, UserItem32.MakeIndex, m_sCharName);
        end;
        { TODO -ochongchong -c新增 : 物品规则 - 挖取提示【2013-07-27】 }
        if (Length(StdItem.Name) > 0) and g_ItemRules.Get(UserItem32.wIndex, 19) then
        begin
          Msg := g_sButchItemHintMsg;
          sStdItemName := ProcessItemName(StdItem.Name);
          if (UserItem32.btValue[13] = 1) and (Length(UserItem32.Name) > 0) then
            sUserItemName := ProcessItemName(UserItem32.Name)
          else
            sUserItemName := sStdItemName;
          if Pos('%showdbitem', LowerCase(Msg)) > 0 then
          begin
            SetSayItem(UserItem32);
            Msg := WideReplaceText(Msg, '%ShowDBItem', Format('{[%s]/%d}', [sStdItemName, UserItem32.MakeIndex]));
          end;

          if Pos('%showitem', LowerCase(Msg)) > 0 then
          begin
            SetSayItem(UserItem32);
            Msg := WideReplaceText(Msg, '%ShowItem', Format('{[%s]/%d}', [sUserItemName, UserItem32.MakeIndex]));
          end;

          Msg := AnsiReplaceText(Msg, '%s', FilterShowName(m_sCharName));
          Msg := AnsiReplaceText(Msg, '%item', sUserItemName);
          Msg := AnsiReplaceText(Msg, '%dbitem', sStdItemName);
          Msg := AnsiReplaceText(Msg, '%name', Player.m_sCharName);
          Msg := AnsiReplaceText(Msg, '%m', m_PEnvir.sMapDesc);
          Msg := AnsiReplaceText(Msg, '%x', IntToStr(m_nCurrX));
          Msg := AnsiReplaceText(Msg, '%y', IntToStr(m_nCurrY));

          UserEngine.SendBroadCastMsg(Msg, g_Config.btDropItemFColor, g_Config.btDropItemBColor, t_hint); // sysmsg
        end;

        Dispose(UserItem32);
      end;
    end
    else
    begin
      Player.m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(Player, '@NoButchItem', False);
    end;
  end;
end;

end.
