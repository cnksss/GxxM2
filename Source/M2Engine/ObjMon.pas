unit ObjMon;

interface

uses
  Windows, Classes, SysUtils, Grobal2, ObjBase, GameEvent, ObjPlayer, M2Threads, M2Definition;

type
  TMonster = class(TAnimalObject)
    m_dwThinkTick: Cardinal; // 0x550
    bo554: Boolean; // 0x554
    m_boDupMode: Boolean; // 0x555
  private
    function Think: Boolean;
    function MakeClone(sMonName: string; OldMon: TBaseObject): TBaseObject;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    function AttackTarget(): Boolean; virtual; // FFEB
    // function MagicAttackTarget: Boolean; virtual; //人物魔法攻击
    procedure Run; override;
  end;

  TChickenDeer = class(TMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
  end;

  TATMonster = class(TMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
  end;

  TCobwebMonster = class(TATMonster) // 蜘蛛网攻击
  private
    function MonAttackTarget: Boolean;
    function CobwebWindingAttack(): Boolean; // 蜘蛛网攻击
  public
    function AttackTarget(): Boolean; override;
  end;

  TMon36_XMonster = class(TATMonster)
    m_boIsFirst: Boolean;
  private
    function AttackTarget36_5: Boolean;
    function AttackTarget0: Boolean;
    procedure MagicAttack;
    // procedure FlyAttack;
    procedure MagicAttackGroup(boSelfRage: Boolean = True; nRage: Integer = 5);
    function MagPushArround(PlayObject: TBaseObject): Integer;
    // function SwordWideAttack(nSecPwr: Integer): Boolean;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    function AttackTarget(): Boolean; override;
    procedure RefreshAppr;
  end;

  TTwoKindAttackMonster = class(TATMonster) // 二种攻击的怪物
  private
    FFrozenTick: DWORD;
    function OneAttack: Boolean;
    function TwoAttack(): Boolean;
  public
    constructor Create; override;
    function AttackTarget(): Boolean; override;
  end;

  TMon38_0Monster = class(TAnimalObject) // Mon38-0 不能移动的怪物 piaoyun 2014-01-03
  private
    m_nAttackRage: Integer; // 攻击范围
    procedure NowDigUP;
  public
    constructor Create(); override;
    function AttackTarget(): Boolean;
    procedure Run; override;
  end;

  TMon38_11Monster = class(TATMonster) // Mon38-11 piaoyun 2014-01-03
  public
    function AttackTarget(): Boolean; override;
  end;

  TMon38_12Monster = class(TATMonster) // Mon38_12 piaoyun 2014-01-03
  private
    procedure MagicAttackGroup(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1);
    procedure AttackTarget0;
  public
    function AttackTarget(): Boolean; override;
  end;

  TMon38_13Monster = class(TATMonster) // Mon38_13 piaoyun 2014-01-03
  private
    procedure MagicAttackGroup(boSelfRage: Boolean; nRage: Integer);
    procedure MagicAttack;
  public
    function AttackTarget(): Boolean; override;
  end;

  TMagicAttackMonster = class(TMonster) // 魔法攻击的怪物
  public
    constructor Create; override;
    procedure Run; override;
    function MagicAttackTarget: Boolean; virtual;
    function AttackTarget(): Boolean; override;
  end;

  TMon35_2Monster = class(TMagicAttackMonster) // Mon38_12 piaoyun 2014-01-03
  private
    procedure MagicAttackGroup(boSelfRage: Boolean; nRage: Integer; Multiple: Integer = 1; nType: Integer = 1);
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TExplosionAttackMonster = class(TMagicAttackMonster) // 冰咆哮怪物
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TLineMagicAttackMonster = class(TMagicAttackMonster) // 直线魔法攻击怪物
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TMLSBAttackMonster = class(TMagicAttackMonster) // 魔龙石碑怪物
  public
    constructor Create; override;
    function AttackTarget(): Boolean; override;
  end;

  TExtinguishDayFireAttackMonster = class(TMagicAttackMonster) // 灭天火怪物
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TFireIceAttackMonster = class(TMagicAttackMonster) // 寒冰掌怪物
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TFireCrossMonster = class(TMagicAttackMonster) // 火墙怪物
  private
    function OneAttack: Boolean;
    function TwoAttack(): Boolean;
  public
    function AttackTarget(): Boolean; override;
  end;

  TIcePeakMonster = class(TMonster) // 雪域卫士
    m_dwStartRunTick: Cardinal;
  private
    procedure MeltStone;
    procedure MeltStoneAll;
  public
    constructor Create; override;
    procedure Run; override;
  end;

  TTruckMonster = class(TMonster) // 押镖车
    m_boSendRefMsg: Boolean;
    m_dwSendRefMsgTick: Cardinal;
    m_boEnterAnotherMap: Boolean;
    m_nGateX, m_nGateY: Integer;
  public
    constructor Create; override;
    procedure Run; override;
  end;

  TFoxMonster = class(TAnimalObject) // 狐狸
    m_dwThinkTick: Cardinal;
    bo554: Boolean;
    m_boDupMode: Boolean;
    m_boWonderingEx: Boolean;
  private
    function Think: Boolean;
    function WonderingEx: Boolean;
  public
    constructor Create(); override;
    function AttackTarget(): Boolean; virtual;
    function MagicAttackTarget: Boolean; virtual; // 人物魔法攻击
    procedure Run; override;
  end;

  // Mon29-0 乌龟怪物
  TTortoiseMonster = class(TMagicAttackMonster) // 乌龟魔法攻击 piaoyun 2013-11-16
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TStoneFoxMonster = class(TFoxMonster) // 石化攻击
  public
    function AttackTarget: Boolean; override;
  end;

  TFoxMagicAttackMonster = class(TMagicAttackMonster) // 狐狸魔法攻击
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TDamageSpellAttackMonster = class(TMagicAttackMonster) // 狐狸魔法攻击  吸蓝
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TDamageArmorAttackMonster = class(TMagicAttackMonster) // 狐狸魔法攻击  减防御
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  // 真狐月天珠下属[青龙白虎朱雀玄武]类 -- piaoyun 2013-12-04
  TMeteoriteRainAttackMonster = class(TAnimalObject) // 流星火雨怪物 怪物不能移动
  private
    m_ShowFireTick: Integer;
  public
    constructor Create(); override;
    function AttackTarget(): Boolean;
    procedure Run; override;
  end;

  // 真狐月天珠类[Mon33-10] -- piaoyun 2013-12-04
  TMagicAttackNotMoveMonster = class(TAnimalObject) // 怪物不能移动  魔法远程攻击
  private
    m_LastStep: Integer;
    m_ForeverFrozenTick: Cardinal; // 永恒冰冻时间间隔 piaoyun 2013-12-04
    m_SlaveObjectList: TList;
    m_boCalledSlave: Boolean; // 神石已召唤
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure CallSlave;
    function AttackTarget(): Boolean;
    procedure Run; override;
  end;

  // 狐狸天珠类[Mon33-10] -- piaoyun 2013-12-04
  TMagicAttackNotMoveMonster2 = class(TAnimalObject) // 怪物不能移动  魔法远程攻击
  private
    m_LastStep: Integer;
    m_ForeverFrozenTick: Cardinal; // 永恒冰冻时间间隔 piaoyun 2013-12-04
    m_SlaveObjectList: TList;
    m_boCalledSlave: Boolean; // 神石已召唤
    m_nOldNextHitTime: Integer;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Initialize(); override;
    procedure CallSlave;
    function AttackTarget(): Boolean;
    procedure Run; override;
  end;

  // 血灵教主[mon39] - chongchong 2014-09-10
  TXueLingLeader = class(TAnimalObject)
  private
    m_boCalledCustodians: array[0..3] of Boolean; // 是否召唤护法
    m_ForeverFrozenTick: Cardinal; // 永恒冰冻时间间隔
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean;
    procedure Run; override;
    procedure Wondering(); override;
  end;

  TFireSpiritMonster = class(TMagicAttackMonster) // 火灵
  public
    function MagicAttackTarget: Boolean; override;
    procedure Run; override;
  end;

  TLionMonster = class(TATMonster) // 狮子
  public
    function AttackTarget(): Boolean; override;
    procedure Run; override;
    procedure GotoTargetXY; override;
  end;

  TSlowATMonster = class(TATMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TScorpion = class(TATMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TSpitSpider = class(TATMonster)
    m_boUsePoison: Boolean;
  private
    procedure SpitAttack(btDir: Byte);
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean; { virtual;// } override; // FFEB
  end;

  THighRiskSpider = class(TSpitSpider)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TBigPoisionSpider = class(TSpitSpider)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TGasAttackMonster = class(TATMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget: Boolean; override;
    function sub_4A9C78(bt05: Byte): TBaseObject; virtual; // FFEA
  end;

  TCowMonster = class(TATMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TMagCowMonster = class(TATMonster)
  private
    procedure sub_4A9F6C(btDir: Byte);
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget: Boolean; override;
  end;

  TCowKingMonster = class(TATMonster)
  private
    dwJumpTime: Cardinal;
    dwRunTime: Cardinal;
    bo55C: Boolean;
    bo55D: Boolean;
    n560: Integer;
    dw564: Cardinal;
    dw568: Cardinal;
    dw56C: Cardinal;
    dw570: Cardinal;
  public
    constructor Create(); override;
    procedure Run; override;
    procedure Attack(TargeTBaseObject: TBaseObject; nDir: Integer); override;
    procedure Initialize(); override;
  end;

  TElectronicScolpionMon = class(TMonster)
    m_boIsFirst: Boolean;
  private
    m_boUseMagic: Boolean;
    procedure LightingAttack(nDir: Integer);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    procedure RefreshAppr;
  end;

  TLightingZombi = class(TMonster)
  private
    procedure LightingAttack(nDir: Integer);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
  end;

  TDigOutZombi = class(TMonster)
  private
    procedure sub_4AA8DC;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
  end;

  TZilKinZombi = class(TATMonster)
    dw558: Cardinal;
    nZilKillCount: Integer;
    dw560: Cardinal;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Die; override;
    procedure Run; override;
  end;

  TWhiteSkeleton = class(TATMonster)
    m_boIsFirst: Boolean;
  private
    procedure sub_4AAD54;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure RecalcAbilitys(); override;
    procedure Run; override;
  end;

  TScultureMonster = class(TMonster)
  private
    procedure MeltStone;
    procedure MeltStoneAll;
    procedure LightingAttack;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
  end;

  TScultureKingMonster = class(TMonster)
    m_nDangerLevel: Integer;
    m_SlaveObjectList: TList;
  private
    procedure MeltStone;
    procedure CallSlave;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Attack(TargeTBaseObject: TBaseObject; nDir: Integer); override;
    procedure Run; override;
  end;

  TGasMothMonster = class(TGasAttackMonster) // 楔蛾
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run; override;
    function sub_4A9C78(bt05: Byte): TBaseObject; override; // FFEA
  end;

  TGasDungMonster = class(TGasAttackMonster)
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TElfMonster = class(TMonster)
    boIsFirst: Boolean; // 0x558
    dwAppearNowTick: Cardinal;
  private
    procedure AppearNow;
    procedure ResetElfMon;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure RecalcAbilitys(); override;
    procedure Run; override;
  end;

  // 修正神兽攻击
  (*
    TElfWarriorMonster = class(TATMonster)
    n55C: Integer;
    boIsFirst: Boolean;                                     // 0x560
    dwDigDownTick: Cardinal;                                // 0x564
    private
    procedure AppearNow;
    procedure ResetElfMon;
    public
    constructor Create(); override;
    destructor Destroy; override;
    procedure RecalcAbilitys(); override;
    procedure Run; override;
    { TODO -ochongchong -c新增 : 英雄召唤的宝宝在安全区停止攻击 【2013-08-15】 }
    function AttackTarget: Boolean; override;
    end;
  *)
  TElfWarriorMonster = class(TSpitSpider)
    n55C: Integer;
    boIsFirst: Boolean; // 0x560
    dwDigDownTick: Cardinal; // 0x564
  private
    procedure AppearNow;
    procedure ResetElfMon;
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure RecalcAbilitys(); override;
    procedure Run; override;
    { TODO -ochongchong -c新增 : 英雄召唤的宝宝在安全区停止攻击 【2013-08-15】 }
    function AttackTarget: Boolean; override;
  end;

  TDevilkingMonster = class(TAnimalObject) // 魔王岭怪物
    m_dwThinkTick: Cardinal; // 0x550
    bo554: Boolean; // 0x554
    m_boDupMode: Boolean; // 0x555
  private
    // function Think: Boolean;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    procedure Run; override;
    procedure GotoTargetXY(); override;
    procedure RunToTargetXY(); override;
  end;

  /// ///////////////////////恶魔蝙蝠类 piaoyun 2013-08-21///////////////////////
  TDevilBat = class(TMonster)
    // 恶魔蝙蝠  施毒术,气功波,抗拒,野蛮对它无效，只有捆魔咒可以捆住,只有刺杀的第2格能攻击到 攻击方式靠近人物自爆攻击
  public
    constructor Create(); override;
    destructor Destroy; override;
    function AttackTarget(): Boolean; override;
    procedure Run; override;
  end;

  /// ///////////////////////////////////////////////////////////////////////////
  TWealthAnimalMon = class(TATMonster) // 富贵兽 20090517
    m_nGameGird: Integer; // 灵符赏金值
  public
    constructor Create(); override;
    destructor Destroy; override;
    function StruckDamage(nDamage: Integer; StruckFrom: TBaseObject; MagicID: Integer; IsSetPKPower: Boolean = True): Integer;
      override; // 受攻击,减身上装备的持久
    procedure StruckDamage1(nDamage: Integer; StruckFrom: TBaseObject);
    // 受指定物品攻击，掉血，并累计灵符赏金值，并红字提示
    procedure Run; override;
    procedure Die; override;
    function AttackTarget(): Boolean; override;
  end;

implementation

uses
  Math, UsrEngn, M2Share, ObjCustomMon;

{ TDevilkingMonster }
constructor TDevilkingMonster.Create; // 004A8B74
begin
  inherited;
  m_boDupMode := False;
  bo554 := False;
  m_dwThinkTick := MyGetTickCount();
  m_nViewRange := 5;
  m_nRunTime := 250;
  m_dwSearchTime := 3000 + Random(2000);
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 108;
  m_boMission := True;
  m_nMissionPointIndex := 0;
end;

destructor TDevilkingMonster.Destroy;
begin
  inherited;
end;

function TDevilkingMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

(*
  function TDevilkingMonster.Think(): Boolean;                                                        // 004A8E54
  var
  nOldX, nOldY: Integer;
  begin
  Result := False;
  if (MyGetTickCount - m_dwThinkTick) > 3 * 1000 then
  begin
  m_dwThinkTick := MyGetTickCount();
  if ((m_Master = nil) or (not InSafeZone) or ((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))) then // 防止安全区宝宝被挤出安全区
  if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then m_boDupMode := True;
  if not IsProperTarget {FFFF4}(m_TargetCret) then m_TargetCret := nil;
  end;                                                                                              // 004A8ED2
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
  end;
  procedure TDevilkingMonster.GotoTargetXY;
  function WalkToNext(nX, nY: Integer): Boolean;
  begin
  Result := WalkTo(GetNextDirection(m_nCurrX, m_nCurrY, nX, nY), False);
  end;
  procedure GotoNext(Path: TPath);
  var
  I: Integer;
  begin
  if Length(Path) > 0 then
  begin
  for I := 0 to Length(Path) - 1 do
  begin
  if (Path[I].X <> m_nCurrX) or (Path[I].Y <> m_nCurrY) then
  begin
  WalkToNext(Path[I].X, Path[I].Y);
  break;
  end;
  end;
  end;
  end;
  var
  Path: TPath;
  begin
  if ((m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)) and (m_nTargetX >= 0) and (m_nTargetY >= 0) then
  begin
  g_FindPath.BaseObject := Self;
  Path := g_FindPath.FindPath1(m_PEnvir, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, False, False);
  if (Length(Path) > 0) then
  begin
  GotoNext(Path);
  end;
  end
  else
  Path := nil;
  end;
*)
// 不用寻路算法。用英雄的处理方式看下到底么样，如果有问题再考虑还原上面的寻路 chongchong 2017-05-12
procedure TDevilkingMonster.GotoTargetXY;
var
  I: Integer;
  nDir: Integer;
  n10: Integer;
  n14: Integer;
  n20: Integer;
  nOldX: Integer;
  nOldY: Integer;
begin
  if (m_nTargetX <> m_nCurrX) or (m_nTargetY <> m_nCurrY) then
  begin
    n10 := m_nTargetX;
    n14 := m_nTargetY;
    // dwTick3F4 := MyGetTickCount();
    nOldX := m_nCurrX;
    nOldY := m_nCurrY;
    nDir := GetNextDirection(m_nCurrX, m_nCurrY, n10, n14); // 20081018 增加
    WalkTo(nDir, False);
    if (m_nTargetX = m_nCurrX) and (m_nTargetY = m_nCurrY) then
      Exit;
    n20 := Random(3);
    for I := DR_UP to DR_UPLEFT do
    begin
      if (nOldX = m_nCurrX) and (nOldY = m_nCurrY) then
      begin
        { if n20 <> 0 then Inc(nDir)           //20080304 修改
          else if nDir > 0 then Dec(nDir)
          else nDir := DR_UPLEFT;
          if (nDir > DR_UPLEFT) then nDir := DR_UP; }
        if n20 <> 0 then
          Inc(nDir); // 20080304 修改
        if (nDir > DR_UPLEFT) then
          nDir := DR_UP; // 20080304 修改
        WalkTo(nDir, False);
        if (m_nTargetX = m_nCurrX) and (m_nTargetY = m_nCurrY) then
        begin
          Break;
        end;
      end;
    end;
  end;
end;

procedure TDevilkingMonster.RunToTargetXY;
var
  nTargetX, nTargetY: Integer;
begin
  if ((m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)) then
  begin
    if GotoNearRuntoXY(m_nTargetX, m_nTargetY, nTargetX, nTargetY) then
    begin
      if (Abs(m_nCurrX - nTargetX) > 1) or (Abs(m_nCurrY - nTargetY) > 1) then
      begin
        RunTo(GetNextDirection(m_nCurrX, m_nCurrY, nTargetX, nTargetY), False);
      end
      else
      begin
        WalkTo(GetNextDirection(m_nCurrX, m_nCurrY, nTargetX, nTargetY), False);
      end;
    end;
  end;
end;

procedure TDevilkingMonster.Run;
begin
  if m_TargetCret <> nil then
    m_TargetCret := nil;
  if m_Master <> nil then
    m_Master := nil;
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    // MainOutMessage(Format('1 m_nMissionX:%d m_nMissionY:%d m_nCurrX:%d m_nCurrY:%d m_nTargetX:%d m_nTargetY:%d', [m_nMissionX, m_nMissionY, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY]));
    if (Length(m_nMissionPoints) > 0) and (Abs(m_nMissionPoints[Length(m_nMissionPoints) - 1].X - m_nCurrX) <= 1) and (Abs(m_nMissionPoints
      [Length(m_nMissionPoints) - 1].Y - m_nCurrY) <= 1) then
    begin
      // MainOutMessage(Format('2 m_nMissionX:%d m_nMissionY:%d m_nCurrX:%d m_nCurrY:%d m_nTargetX:%d m_nTargetY:%d', [m_nMissionX, m_nMissionY, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY]));
      m_boNoItem := True;
      MakeGhost;
      inherited;
      Exit;
    end;
    { if Think then begin
      inherited;
      Exit;
      end; }
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
      end; // 004A9151
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
  inherited;
end;

{ TMonster }
constructor TMonster.Create; // 004A8B74
begin
  inherited;
  m_boDupMode := False;
  bo554 := False;
  m_dwThinkTick := MyGetTickCount();
  m_nViewRange := 5;
  m_nRunTime := 250;
  m_dwSearchTime := 3000 + Random(2000);
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 80;
end;

destructor TMonster.Destroy;
begin
  inherited;
end;

function TMonster.MakeClone(sMonName: string; OldMon: TBaseObject): TBaseObject;
var
  ElfMon: TBaseObject;
begin
  Result := nil;
  ElfMon := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, m_nCurrX, m_nCurrY, sMonName);
  if ElfMon <> nil then
  begin
    ElfMon.m_Master := OldMon.m_Master;
    ElfMon.m_dwMasterRoyaltyStartTick := OldMon.m_dwMasterRoyaltyStartTick;
    ElfMon.m_dwMasterRoyaltyTime := OldMon.m_dwMasterRoyaltyTime;
    ElfMon.m_btSlaveMakeLevel := OldMon.m_btSlaveMakeLevel;
    ElfMon.m_btSlaveExpLevel := OldMon.m_btSlaveExpLevel;
    ElfMon.m_boChangeAbility := OldMon.m_boChangeAbility;
    ElfMon.m_ChangeAbility := OldMon.m_ChangeAbility;
    // 宝宝变身时，把类型传过去 chongchong 2013-11-22
    ElfMon.m_BBType := OldMon.m_BBType;
    // 宝宝变身时，把宝宝的叠加属性传过去 chongchong 2014-12-31
    Move(OldMon.m_nBBAttrPlusValues, ElfMon.m_nBBAttrPlusValues, SizeOf(OldMon.m_nBBAttrPlusValues));
    ElfMon.RecalcAbilitys;
    ElfMon.RefNameColor;
    if OldMon.m_Master <> nil then
      OldMon.m_Master.m_SlaveList.Add(ElfMon);
    ElfMon.m_WAbil := OldMon.m_WAbil;
    ElfMon.m_wStatusTimeArr := OldMon.m_wStatusTimeArr;
    ElfMon.m_TargetCret := OldMon.m_TargetCret;
    ElfMon.m_dwTargetFocusTick := OldMon.m_dwTargetFocusTick;
    ElfMon.m_LastHiter := OldMon.m_LastHiter;
    ElfMon.m_LastHiterTick := OldMon.m_LastHiterTick;
    ElfMon.m_btDirection := OldMon.m_btDirection;
    Result := ElfMon;
  end;
end;

function TMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

function TMonster.Think(): Boolean; // 004A8E54
var
  nOldX, nOldY: Integer;
begin
  Result := False;
  if (MyGetTickCount - m_dwThinkTick) > 3 * 1000 then
  begin
    m_dwThinkTick := MyGetTickCount();
    if ((m_Master = nil) or (not InSafeZone) or ((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))) then
    // 防止安全区宝宝被挤出安全区
    begin
      if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then
        m_boDupMode := True
    end
    // 不让宝宝和NPC叠到一起 2020-09-07 00:01:53
    else if (m_Master <> nil) and (m_PEnvir.GetXYNpcObjCount(m_nCurrX, m_nCurrY) >= 1) then
    begin
      m_boDupMode := True;
    end;
    {
      // 修正宝宝不能锁定人物 chongchong 2016-05-07
      if (m_Master <> nil) and (m_boTarget) then
      begin
      if (not m_Master.IsProtectTarget(m_TargetCret)) then
      DelTargetCreat
      end
      else }
    if not IsProperTarget { FFFF4 } (m_TargetCret) then
      DelTargetCreat;
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
end;

function TMonster.AttackTarget(): Boolean; // 004A8F34
var
  bt06: Byte;
begin
  Result := False;
  if (m_TargetCret <> nil) and (not m_TargetCret.m_boDeath) and (not m_TargetCret.m_boGhost) then
  begin
    // 如果宝宝攻击人物的威力为0，则不攻击人物 chongchong 2015-12-05
    if (m_nSlaveAttackHumPowerRate = 0) and (Master <> nil) and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) then
    begin
      DelTargetCreat();
      Exit;
    end;
    if GetAttackDir(m_TargetCret, bt06) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        Attack(m_TargetCret, bt06); // FFED
        BreakHolySeizeMode();
        if m_wAppr = 622 then
        begin
          if Random(10) = 0 then
          begin
            m_TargetCret.MakePosion(POISON_STONE, Random(3) + 2, 0);
          end
        end;
      end;
      Result := True;
    end
    else
    begin
      if m_TargetCret.m_PEnvir = m_PEnvir then
      begin
        SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY); { 0FFF0h }
      end
      else
      begin
        DelTargetCreat();
      end;
    end;
  end;
end;
  (*
  procedure TMonster.Run;
  var
  nX, nY, nMinRange: Integer;
  begin
  if not m_boGhost and
  not m_boDeath and
  not m_boFixedHideMode and
  not m_boStoneMode and
  CanMove then
  begin
  if (m_Master <> nil) then
  begin
  // 天关宝宝不让带出地图
  if (m_PEnvir <> m_Master.m_PEnvir) and (m_PEnvir.m_boGuardianLevel) then
  begin
  MakeGhost;
  Exit;
  end
  else if (m_PEnvir <> m_Master.m_PEnvir) and (m_PEnvir.m_boMirror) then     // 主人从镜像地图换到非镜像地图
  begin
  SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
  Exit;
  end;
  end;
  if Think then
  begin
  inherited;
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
  end;                                                  // 004A9151
  if (m_Master <> nil) and m_Master.m_boSlaveRelax then
  begin
  DelTargetCreat;
  m_boTarget := False;
  end;
  if not m_boRunAwayMode then
  begin
  if not m_boNoAttackMode then
  begin
  if m_TargetCret <> nil then
  begin
  if AttackTarget {FFEB} then
  begin
  inherited;
  Exit;
  end;
  end
  else
  begin
  m_nTargetX := -1;
  if m_boMission and (Length(m_nMissionPoints) > 0) and (m_nMissionPointIndex < Length(m_nMissionPoints)) then
  begin
  if (m_nMissionPointIndex < 0) then
  m_nMissionPointIndex := 0;
  if (Abs(m_nCurrX - m_nMissionPoints[m_nMissionPointIndex].X) <= 3) and
  (Abs(m_nCurrY - m_nMissionPoints[m_nMissionPointIndex].Y) <= 3) then
  begin
  Inc(m_nMissionPointIndex);
  if m_nMissionPointIndex >= Length(m_nMissionPoints) then
  m_nMissionPointIndex := Length(m_nMissionPoints) - 1;
  end;
  m_nTargetX := m_nMissionPoints[m_nMissionPointIndex].X;
  m_nTargetY := m_nMissionPoints[m_nMissionPointIndex].y;
  end;                                            // 004A91D3
  end;
  end;                                                // 004A91D3  if not bo2C0 then begin
  if m_Master <> nil then
  begin
  // 目标超过主人一段距离，删除目标让宝宝回去 chongchong 2017-07-01
  if m_TargetCret <> nil then
  begin
  if (abs(m_TargetCret.m_nCurrX - m_Master.m_nCurrX) > 20) or
  (abs(m_TargetCret.m_nCurrY - m_Master.m_nCurrY) > 20) or
  (m_PEnvir <> m_Master.m_PEnvir) then
  begin
  DelTargetCreat;
  end;
  end;
  if m_TargetCret = nil then
  begin
  if ((m_btRaceServer = 155) and (m_btRaceImg = 156){自定义怪物 - 魔王岭宝宝}) or
  (Self is TCustomMonster and (TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MoveOption = moNoMove)) then
  begin
  end
  else
  begin
  m_Master.GetBackPosition(nX, nY);
  if (abs(m_nTargetX - nX) > 1) or (abs(m_nTargetY - nY {nX}) > 1) then
  begin                                           // 004A922D
  m_nTargetX := nX;
  m_nTargetY := nY;
  if (abs(m_nCurrX - nX) <= 2) and (abs(m_nCurrY - nY) <= 2) and
  // 修正怪物宝宝会和人物叠一起  chongchong 2015-09-11
  (m_nCurrX <> m_Master.m_nCurrX) and (m_nCurrY <> m_Master.m_nCurrY) then
  begin
  if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
  begin
  m_nTargetX := m_nCurrX;
  m_nTargetY := m_nCurrY;
  end                                         // 004A92A5
  end;
  end;                                            // 004A92A5
  end;
  end;                                              // 004A92A5 if m_TargetCret = nil then begin
  if not (
  ((m_btRaceServer = 155) and (m_btRaceImg = 156)){自定义怪物 - 魔王岭宝宝} or
  // 加了不可移动的自定义怪物不让飞走 chongchong 2019-03-19 11:55:06
  (Self is TCustomMonster and (TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MoveOption = moNoMove))
  ) then
  begin
  if (not m_Master.m_boSlaveRelax) and
  ((m_PEnvir <> m_Master.m_PEnvir) or (abs(m_nCurrX - m_Master.m_nCurrX) > 20) or  (abs(m_nCurrY - m_Master.m_nCurrY) > 20)) and
  // 目标才让飞 chongchong 2019-03-19 11:55:06
  (m_nTargetX <> -1) and (m_nTargetY <> -1) then
  begin
  SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
  end
  end;
  end;                                                // 004A937E if m_Master <> nil then begin
  end
  else
  begin                                                 // 004A9344
  if (m_dwRunAwayTime > 0) and ((MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime) then
  begin
  m_boRunAwayMode := False;
  m_dwRunAwayTime := 0;
  end;
  end;                                                  // 004A937E
  if (m_Master <> nil) and m_Master.m_boSlaveRelax then
  begin
  inherited;
  Exit;
  end;                                                  // 004A93A6
  if m_nTargetX <> -1 then
  begin
  // 修正自定义怪攻击距离大于1时，怪物朝下的方向攻击玩家，攻击距离只有一隔 chongchong 2014-09-11
  if Self is TCustomMonster then
  begin
  nMinRange := TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MinAttackNearRange;
  if nMinRange <= 1 then
  GotoTargetXY
  else
  begin
  if m_TargetCret <> nil then
  begin
  if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > nMinRange) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > nMinRange) then
  GotoTargetXY
  else if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <> 0) and
  (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <> 0) and
  (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <> Abs(m_nCurrY - m_TargetCret.m_nCurrY)) then
  GotoTargetXY;
  end
  else
  GotoTargetXY;
  end;
  end
  //-----------------------------------------------------------------------
  else
  GotoTargetXY();                                     // 004A93B5 0FFEF
  end
  else
  begin
  if m_TargetCret = nil then Wondering();             // FFEE   //Jacky
  end;                                                  // 004A93D8
  end;                                                    // 004A93D8  if not bo510 and (tick_diff(m_dwWalkTick, MyGetTickCount) > n4FC) then begin
  end;                                                      // 004A93D8
  inherited;
  end;
*)
// 修正宝宝的移动速度影响其攻击速度 2019-09-26 00:26:01
procedure TMonster.Run;
var
  nX, nY, nMinRange: Integer;
  IsCanMove: Boolean;
  boEnabledPetPickup: Boolean;
  SmartObject: TSmartObject;
begin
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
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
    if Think then
    begin
      inherited;
      Exit;
    end;
    if m_boWalkWaitLocked then
    begin
      if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
      begin
        m_boWalkWaitLocked := False;
      end;
    end;
    // 不确定此处的修改会不会让系统的怪物攻击速度变得更快 2019-09-26 00:31:41
    IsCanMove := (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay);
    // 提高宠物拴物速度 2019-12-19 11:20:40
    if m_boGamePet and (g_Config.boPetQuickPickup) and (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
    begin
      boEnabledPetPickup := ((m_btGamePetEnablePick = 0) and g_Config.boEnabledPetPickup) or (m_btGamePetEnablePick = 1);
      if boEnabledPetPickup then
      begin
        if g_Config.boPetRangePickup then
        begin
          if PickRangeItem(False, True, True, 0) then
            Exit;
        end;
        StartPickUpItem(True, True, 0, False);
      end;
    end;
    if not m_boWalkWaitLocked then
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
      if not m_boRunAwayMode then
      begin
        if not m_boNoAttackMode then
        begin
          if (m_TargetCret <> nil) and (tick_diff(m_dwStationTick, MyGetTickCount) > m_nWalkSpeed) then
          // 怪物站稳了再打，不能一跑过来就打 2020-11-01 00:37:46
          begin
            if AttackTarget { FFEB } then
            begin
              inherited;
              Exit;
            end;
          end
          else if IsCanMove then
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
            end // 004A91D3
            else
            begin
              if m_boGamePet and (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
              begin
                boEnabledPetPickup := ((m_btGamePetEnablePick = 0) and g_Config.boEnabledPetPickup) or (m_btGamePetEnablePick = 1);
                if boEnabledPetPickup then
                begin
                  if ((not m_Master.m_boSlaveRelax) or (m_boGamePet and (not g_Config.boPetSleepControlBySlave))) and ((m_PEnvir
                    <> m_Master.m_PEnvir) or (Abs(m_nCurrX - m_Master.m_nCurrX) > 20) or (Abs(m_nCurrY - m_Master.m_nCurrY) > 20))
                    then
                  begin
                    m_Master.GetBackPosition(nX, nY);
                    m_nTargetX := nX;
                    m_nTargetY := nY;
                    SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
                    Exit;
                  end
                  else
                  begin
                    if g_Config.boPetRangePickup then
                    begin
                      if PickRangeItem(False, True, True, 0) then
                        Exit;
                    end;
                    if StartPickUpItem(True, True, 0) then
                    begin
                      Exit;
                    end;
                  end;
                end;
              end
              // 加入宝宝等自动捡物 2020-03-27 21:53:00
              else if (m_Master <> nil) and (m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
              begin
                SmartObject := TSmartObject(m_Master);
                if not m_PEnvir.m_boNoAutoRangePickItem then // 禁止范围拾取
                begin
                  if SmartObject.m_boSlaveAutoPickItem and (g_nKey_UseClientPickItems <> 0) then
                  begin
                    if SmartObject.m_btSlaveAutoPickItemRange > 0 then
                    begin
                      if PickRangeItem(SmartObject.m_boSlaveAutoPickAll, SmartObject.m_boAutoPickPlayDropItem, SmartObject.m_boAutoPickPlayScatterItem,
                        SmartObject.m_dwAutoPickScatterToPickTime) then
                        Exit;
                    end;
                    if StartPickUpItem(SmartObject.m_boAutoPickPlayDropItem, SmartObject.m_boAutoPickPlayScatterItem, SmartObject.m_dwAutoPickScatterToPickTime)
                      then
                    begin
                      Exit;
                    end;
                  end;
                end;
              end;
            end;
          end;
        end; // 004A91D3  if not bo2C0 then begin
        if IsCanMove and (m_Master <> nil) then
        begin
          // 目标超过主人一段距离，删除目标让宝宝回去 chongchong 2017-07-01
          if m_TargetCret <> nil then
          begin
            if (Abs(m_TargetCret.m_nCurrX - m_Master.m_nCurrX) > 20) or (Abs(m_TargetCret.m_nCurrY - m_Master.m_nCurrY) > 20) or (m_PEnvir
              <> m_Master.m_PEnvir) then
            begin
              DelTargetCreat;
            end;
          end;
          if m_TargetCret = nil then
          begin
            if ((m_btRaceServer = 155) and (m_btRaceImg = 156) { 自定义怪物 - 魔王岭宝宝 } ) or (Self is TCustomMonster and (TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MoveOption
              = moNoMove)) then
            begin
            end
            else
            begin
              m_Master.GetBackPosition(nX, nY);
              if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY { nX } ) > 1) then
              begin // 004A922D
                m_nTargetX := nX;
                m_nTargetY := nY;
                if (Abs(m_nCurrX - nX) <= 2) and (Abs(m_nCurrY - nY) <= 2) and
                // 修正怪物宝宝会和人物叠一起  chongchong 2015-09-11
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
          end; // 004A92A5 if m_TargetCret = nil then begin
          if not (((m_btRaceServer = 155) and (m_btRaceImg = 156)) { 自定义怪物 - 魔王岭宝宝 } or
            // 加了不可移动的自定义怪物不让飞走 chongchong 2019-03-19 11:55:06
            (Self is TCustomMonster and (TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MoveOption = moNoMove))) then
          begin
            if ((not m_Master.m_boSlaveRelax) or (m_boGamePet and (not g_Config.boPetSleepControlBySlave))) and ((m_PEnvir <>
              m_Master.m_PEnvir) or (Abs(m_nCurrX - m_Master.m_nCurrX) > 20) or (Abs(m_nCurrY - m_Master.m_nCurrY) > 20)) and
            // 目标才让飞 chongchong 2019-03-19 11:55:06
              (m_nTargetX <> -1) and (m_nTargetY <> -1) then
            begin
              SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
            end
          end;
        end; // 004A937E if m_Master <> nil then begin
      end
      else
      begin // 004A9344
        if (m_dwRunAwayTime > 0) and ((MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime) then
        begin
          m_boRunAwayMode := False;
          m_dwRunAwayTime := 0;
        end;
      end; // 004A937E
      if (m_Master <> nil) and m_Master.m_boSlaveRelax and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave) then
      begin
        inherited;
        Exit;
      end; // 004A93A6
      if IsCanMove then
      begin
        if (m_nTargetX <> -1) then
        begin
          // 修正自定义怪攻击距离大于1时，怪物朝下的方向攻击玩家，攻击距离只有一隔 chongchong 2014-09-11
          if Self is TCustomMonster then
          begin
            nMinRange := TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MinAttackNearRange;
            if nMinRange <= 1 then
              GotoTargetXY
            else
            begin
              if m_TargetCret <> nil then
              begin
                if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > nMinRange) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > nMinRange) then
                  GotoTargetXY
                else if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <> 0) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <> 0) and (Abs(m_nCurrX
                  - m_TargetCret.m_nCurrX) <> Abs(m_nCurrY - m_TargetCret.m_nCurrY)) then
                  GotoTargetXY;
              end
              else
                GotoTargetXY;
            end;
          end
          // -----------------------------------------------------------------------
          else
            GotoTargetXY(); // 004A93B5 0FFEF
        end
        else
        begin
          if m_TargetCret = nil then
            Wondering(); // FFEE   //Jacky
        end; // 004A93D8
      end;
    end;
    // 004A93D8  if not bo510 and (tick_diff(m_dwWalkTick, MyGetTickCount) > n4FC) then begin
  end; // 004A93D8
  inherited;
end;

{ TChickenDeer }
constructor TChickenDeer.Create; // 004A93E8
begin
  inherited;
  m_nViewRange := 5;
end;

destructor TChickenDeer.Destroy;
begin
  inherited;
end;

procedure TChickenDeer.Run; // 004A9438
var
  I, nC, n10, n14: Integer;
  BaseObject1C, BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  n10 := 9999;
  BaseObject := nil;
  BaseObject1C := nil;
  if not m_boDeath and not bo554 and not m_boGhost and CanMove then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay then
    begin
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
              if not BaseObject.m_boHideMode or m_boCoolEye then
              begin
                nC := Abs(m_nCurrX - BaseObject.m_nCurrX) + Abs(m_nCurrY - BaseObject.m_nCurrY);
                if nC < n10 then
                begin
                  n10 := nC;
                  BaseObject1C := BaseObject;
                end;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
      if BaseObject1C <> nil then
      begin
        m_boRunAwayMode := True;
        m_TargetCret := BaseObject1C;
      end
      else
      begin
        m_boRunAwayMode := False;
        m_TargetCret := nil;
      end;
    end; //
    if m_boRunAwayMode and (m_TargetCret <> nil) and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay) then
    begin
      m_nWalkDelay := 0;
      if (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6) and (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6) then
      begin
        n14 := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, n14, 5, m_nTargetX, m_nTargetY);
      end;
    end;
  end;
  inherited;
end;

{ TATMonster }
constructor TATMonster.Create; // 004A9690
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
end;

destructor TATMonster.Destroy;
begin
  inherited;
end;

procedure TATMonster.Run;
begin
  if not m_boDeath and not bo554 and not m_boGhost and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      // Ctrl + R 宝宝锁定目标 chongchong 2016-03-16
      if m_boTarget and (m_TargetCret <> nil) and (not m_TargetCret.m_boDeath) and (not m_TargetCret.m_boGhost) and (m_TargetCret.m_PEnvir
        = m_PEnvir) and (Abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 20) and (Abs(m_TargetCret.m_nCurrY - m_nCurrY) <= 20) then
      begin
      end
      else if (m_Master = nil) or ((m_Master <> nil) and ((not m_Master.m_boSlaveRelax) or (m_boGamePet and (not g_Config.boPetSleepControlBySlave))))
        then
      begin
        m_boTarget := False;
        SearchTarget();
      end;
    end;
  end;
  inherited;
end;

{ TSlowATMonster }
constructor TSlowATMonster.Create;
begin
  inherited;
end;

destructor TSlowATMonster.Destroy;
begin
  inherited;
end;

{ TScorpion }
constructor TScorpion.Create;
begin
  inherited;
  m_boAnimal := True;
end;

destructor TScorpion.Destroy;
begin
  inherited;
end;

{ TSpitSpider }
constructor TSpitSpider.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
  m_boAnimal := True;
  m_boUsePoison := True;
end;

destructor TSpitSpider.Destroy;
begin
  inherited;
end;

procedure TSpitSpider.SpitAttack(btDir: Byte);
var
  WAbil: pTAbility;
  nC, n10, n14, n18, n1C, nPower: Integer;
  BaseObject: TBaseObject;
  RandomValue: Integer;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  m_btDirection := btDir;
  WAbil := @m_WAbil;
  n1C := WAbil.DC2 - WAbil.DC1 + 1;
  if n1C > 0 then
    n1C := Random(n1C);
  n1C := n1C + WAbil.DC1;
  // n1C := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
  if n1C <= 0 then
    Exit;
  SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  nC := 0;
  if (m_Master <> nil) then
    n1C := Round(n1C * (g_Config.nSlavePowerRate / 100));
  while (nC < 5) do
  begin
    n10 := 0;
    while (n10 < 5) do
    begin
      if g_Config.SpitMap[btDir, nC, n10] = 1 then
      begin
        {
          (0, 0, 0, 0, 0),
          (0, 0, 0, 0, 0),
          (0, 0, 1, 0, 0),
          (0, 0, 1, 0, 0)),
        }
        n14 := m_nCurrX - 2 + n10;
        n18 := m_nCurrY - 2 + nC;
        BaseObject := m_PEnvir.GetMovingObject(n14, n18, True);
        if (BaseObject <> nil) and (BaseObject <> Self) and (IsProperTarget(BaseObject)) and (Random(BaseObject.m_btSpeedPoint) <
          m_btHitPoint) then
        begin
          if not CanCloseDefense then // 忽视目标防御
            n1C := BaseObject.GetMagStruckDamage(Self, n1C, nil)
          else
            n1C := BaseObject.GetMagStruckDamage(Self, n1C, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
          n1C := BaseObject.NewAbilPower(3, n1C);
          n1C := GetPowerRateAdd(BaseObject, n1C);
          n1C := NewAbilPower(1, n1C); // 元素增加攻击伤害
          n1C := GetNextDamage(n1C);
          // 怪物伤害封顶 chongchong 2016-09-07
          n1C := BaseObject.GetAttackPowerMax(n1C);
          // 吸收伤害
          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(BaseObject);
            // 伤害吸收百分比 2020-09-17 20:11:44
            n1C := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, n1C);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              n1C := Max(0, n1C - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            if (SmartObject.m_nSuckDamagePoint > 0) and (n1C > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin
              if Random(100) < SmartObject.m_nSuckDamageProbability then
              begin
                nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * n1C);
                if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
                  nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
                Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
                n1C := Max(n1C - nSuckDamagePoint, 0);
              end;
            end;
          end;
          if n1C > 0 then
          begin
            n1C := BaseObject.StruckDamage(n1C, Self, 0);
            BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n1C, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
              NativeInt(Self), '', 300);
            if m_boUsePoison then
            begin
              Randomize;
              RandomValue := Random( { m_btAntiPoison + } 20);
              // OutputDebugString(PChar(IntToStr(RandomValue)));
              if (not BaseObject.UnPosion) and (RandomValue = 0) then // 防毒
                BaseObject.MakePosion(POISON_DECHEALTH, 30, 1);
              // if Random(2) = 0 then
              // BaseObject.MakePosion(POISON_STONE,5,1);
            end;
            if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(BaseObject.m_btAntiPoison
              + m_dwParalysisRate, 0)) = 0) then
            begin // 麻痹
              BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
            end;
            nPower := BaseObject.DamageReboundPower(n1C);
            if nPower > 0 then
            begin // 反弹伤害
              nPower := StruckDamage(nPower, nil, 0);
              SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 300);
            end;
          end;
        end;
      end;
      Inc(n10);
      {
        if n10 >= 5 then break;
      }
    end;
    Inc(nC);
    // if nC >= 5 then break;
  end; // while
end;

function TSpitSpider.AttackTarget: Boolean;
var
  btDir: Byte;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if TargetInSpitRange(m_TargetCret, btDir) then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      SpitAttack(btDir);
      BreakHolySeizeMode();
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

{ THighRiskSpider }
constructor THighRiskSpider.Create;
begin
  inherited;
  m_boAnimal := False;
  m_boUsePoison := False;
end;

destructor THighRiskSpider.Destroy;
begin
  inherited;
end;

{ TBigPoisionSpider }
constructor TBigPoisionSpider.Create;
begin
  inherited;
  m_boAnimal := False;
  m_boUsePoison := True;
end;

destructor TBigPoisionSpider.Destroy;
begin
  inherited;
end;

{ TGasAttackMonster }
constructor TGasAttackMonster.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
  m_boAnimal := True;
end;

destructor TGasAttackMonster.Destroy;
begin
  inherited;
end;

function TGasAttackMonster.sub_4A9C78(bt05: Byte): TBaseObject;
var
  WAbil: pTAbility;
  n10: Integer;
  BaseObject: TBaseObject;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := nil;
  m_btDirection := bt05;
  WAbil := @m_WAbil;
  n10 := WAbil.DC2 - WAbil.DC1 + 1;
  if n10 > 0 then
    n10 := Random(n10);
  n10 := n10 + WAbil.DC1;
  // n10 := Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC);
  if n10 > 0 then
  begin
    if (m_Master <> nil) then
      n10 := Round(n10 * (g_Config.nSlavePowerRate / 100));
    SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    BaseObject := GetPoseCreate();
    if (BaseObject <> nil) and IsProperTarget(BaseObject) and
    // 怪物不攻击脱机人物 chongchong 2015-09-07
      (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
      and (Random(BaseObject.m_btSpeedPoint) < m_btHitPoint) then
    begin
      if not CanCloseDefense then // 忽视目标防御
        n10 := BaseObject.GetMagStruckDamage(Self, n10, nil)
      else
        n10 := BaseObject.GetMagStruckDamage(Self, n10, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      n10 := BaseObject.NewAbilPower(3, n10);
      n10 := GetPowerRateAdd(BaseObject, n10);
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
        BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n10, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP, NativeInt(Self),
          '', 300);
        if BaseObject.CanStone(20) then
        begin
          BaseObject.MakePosion(POISON_STONE, 5, 0)
        end;
        if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(BaseObject.m_btAntiPoison
          + m_dwParalysisRate, 0)) = 0) then
        begin
          BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
        end;
        n10 := BaseObject.DamageReboundPower(n10);
        if n10 > 0 then
        begin // 反弹伤害
          n10 := StruckDamage(n10, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n10, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 300);
        end;
        Result := BaseObject;
      end;
    end;
  end;
end;

function TGasAttackMonster.AttackTarget(): Boolean;
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
      sub_4A9C78(btDir);
      BreakHolySeizeMode();
    end;
    Result := True;
  end
  else
  begin
    if m_TargetCret.m_PEnvir = m_PEnvir then
      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)
    else
      DelTargetCreat();
  end;
end;

{ TCowMonster }
constructor TCowMonster.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
end;

destructor TCowMonster.Destroy;
begin
  inherited;
end;

{ TMagCowMonster }
constructor TMagCowMonster.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
end;

destructor TMagCowMonster.Destroy;
begin
  inherited;
end;

procedure TMagCowMonster.sub_4A9F6C(btDir: Byte);
var
  WAbil: pTAbility;
  n10: Integer;
  BaseObject: TBaseObject;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  m_btDirection := btDir;
  WAbil := @m_WAbil;
  n10 := WAbil.DC2 - WAbil.DC1 + 1;
  if n10 > 0 then
    n10 := Random(n10);

  n10 := n10 + WAbil.DC1;
  // n10 := Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC);
  if n10 > 0 then
  begin
    if (m_Master <> nil) then
      n10 := Round(n10 * (g_Config.nSlavePowerRate / 100));

    SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    BaseObject := GetPoseCreate();
    if (BaseObject <> nil) and IsProperTarget(BaseObject) and
    // 怪物不攻击脱机人物 chongchong 2015-09-07
      (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
      then
    begin
      if not CanCloseDefense then // 忽视目标防御
        n10 := BaseObject.GetMagStruckDamage(Self, n10, nil)
      else
        n10 := BaseObject.GetMagStruckDamage(Self, n10, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      n10 := BaseObject.NewAbilPower(3, n10);
      n10 := GetPowerRateAdd(BaseObject, n10);
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
        BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n10, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP, NativeInt(Self),
          '', 300);

        if (not BaseObject.UnParalysis) //
          and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) //
          and (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
          BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime

        n10 := BaseObject.DamageReboundPower(n10);
        if n10 > 0 then
        begin // 反弹伤害
          n10 := StruckDamage(n10, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, n10, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 300);
        end;
      end;
    end;
  end;
end;

function TMagCowMonster.AttackTarget: Boolean;
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
      sub_4A9F6C(btDir);
      BreakHolySeizeMode();
    end;
    Result := True;
  end
  else
  begin
    if m_TargetCret.m_PEnvir = m_PEnvir then
      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)
    else
      DelTargetCreat();
  end;
end;

{ TCowKingMonster }
constructor TCowKingMonster.Create;
begin
  inherited;

  m_dwSearchTime := Random(1500) + 500;
  dwJumpTime := MyGetTickCount();
  dwRunTime := MyGetTickCount();
  m_boMagStruckMonKeepMoveSpeed := True;
  n560 := 0;
  bo55C := False;
  bo55D := False;
end;

procedure TCowKingMonster.Attack(TargeTBaseObject: TBaseObject; nDir: Integer);
var
  WAbil: pTAbility;
  nPower: Integer;
begin
  WAbil := @m_WAbil;
  nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
  if m_Master <> nil then
    nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));

  HitMagAttackTarget(TargeTBaseObject, nPower div 2, nPower div 2, True);
  // inherited;
end;

procedure TCowKingMonster.Initialize;
begin
  dw56C := m_nNextHitTime;
  dw570 := m_nWalkSpeed;

  inherited;
end;

procedure TCowKingMonster.Run;
var
  tmpX, tmpY, tmpHP: Integer;
begin
  if (not m_boDeath) and (not bo554) and (not m_boGhost) then
  begin
    if MyGetTickCount - dwJumpTime >= 30 * 1000 then
    begin
      dwJumpTime := MyGetTickCount();
      if (m_TargetCret <> nil) and (SiegeInspection() >= 5) then
      begin
        m_TargetCret.GetBackPosition(tmpX, tmpY);

        if m_PEnvir.CanWalk(tmpX, tmpY, False) then
          SpaceMove(m_PEnvir.sMapName, tmpX, tmpY, 0)
        else
          MapRandomMove(m_PEnvir.sMapName, 0);

        Exit;
      end;
    end;

    if MyGetTickCount - dwRunTime >= 2 * 1000 then
    begin
      dwRunTime := MyGetTickCount();
      tmpHP := 7 - m_WAbil.HP div (m_WAbil.MaxHP div 7);
      if n560 <> tmpHP then
      begin
        n560 := tmpHP;
        if tmpHP >= 2 then
        begin
          bo55C := True;
          dw564 := MyGetTickCount();
        end;
      end;

      if bo55C then
      begin
        if MyGetTickCount - dw564 <= 5000 then
        begin // 恢复正常攻击
          m_nNextHitTime := dw56C;
        end
        else // 发狂
        begin
          bo55C := False;
          bo55D := True;
          dw568 := MyGetTickCount();
        end;
      end;

      if bo55D then
      begin
        // 发狂10秒
        if MyGetTickCount - dw568 < 8000 then
        begin
          m_nNextHitTime := 500;
          m_nWalkSpeed := 400;
        end
        else
        begin
          bo55D := False;
          m_nNextHitTime := dw56C;
          m_nWalkSpeed := dw570;
        end;
      end;
    end;
  end;

  inherited;
end;

{ TLightingZombi }
constructor TLightingZombi.Create;
begin
  inherited;
  // m_nViewRange := 6;
  m_dwSearchTime := Random(1500) + 1500;
  m_boAnimal := False;
end;

destructor TLightingZombi.Destroy;
begin
  inherited;
end;

procedure TLightingZombi.LightingAttack(nDir: Integer);
var
  nSX, nSY, nTX, nTY, nPwr: Integer;
  WAbil: pTAbility;
begin
  m_btDirection := nDir;
  SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, 1, nSX, nSY) then
  begin
    m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, nDir, 9, nTX, nTY);
    WAbil := @m_WAbil;
    // nPwr := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
    nPwr := WAbil.DC2 - WAbil.DC1 + 1;
    if nPwr > 0 then
      nPwr := Random(nPwr);
    nPwr := nPwr + WAbil.DC1;
    if nPwr > 0 then
    begin
      if (m_Master <> nil) then
        nPwr := Round(nPwr * (g_Config.nSlavePowerRate / 100));
      MagPassThroughMagic(nSX, nSY, nTX, nTY, nDir, nPwr, 0, True);
    end;
  end;
  BreakHolySeizeMode();
end;

procedure TLightingZombi.Run;
var
  nAttackDir: Integer;
begin
  if (not m_boDeath) and (not bo554) and (not m_boGhost) and CanMove and ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    if (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) and (m_TargetCret <> nil) and (Abs(m_nCurrX -
      m_TargetCret.m_nCurrX) <= 4) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 4) then
    begin
      m_nWalkDelay := 0;
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 2) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 2) and (Random(3) <> 0) then
      begin
        inherited;
        Exit;
      end;
      GetBackPosition(m_nTargetX, m_nTargetY);
    end;
    if (m_Master <> nil) and (m_Master.m_boSlaveRelax) and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave) then
    begin
      inherited;
      Exit; // 休息状态 退出
    end
    else if (m_TargetCret <> nil) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) < 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) < 6)
      and (tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay) then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      nAttackDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      LightingAttack(nAttackDir);
    end;
  end;
  inherited;
end;

{ TDigOutZombi }
constructor TDigOutZombi.Create;
begin
  inherited;
  bo554 := False;
  m_nViewRange := 7;
  m_dwSearchTime := Random(1500) + 2500;
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 95;
  m_boFixedHideMode := True;
end;

destructor TDigOutZombi.Destroy;
begin
  inherited;
end;

procedure TDigOutZombi.sub_4AA8DC;
var
  Event: TGameEvent;
begin
  Event := TGameEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, 1, 5 * 60 * 1000, True);
  g_EventManager.AddEvent(Event);
  m_boFixedHideMode := False;
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(Event), '');
end;

procedure TDigOutZombi.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if (not m_boGhost) and (not m_boDeath) and CanMove and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay)
    then
  begin
    m_nWalkDelay := 0;
    if m_boFixedHideMode then
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
                if (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 3) and (Abs(m_nCurrY - BaseObject.m_nCurrY) <= 3) then
                begin
                  sub_4AA8DC();
                  m_dwWalkTick := MyGetTickCount;
                  m_nWalkDelay := 1000;
                  Break;
                end;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end
    else
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();
      end;
    end;
  end;
  inherited;
end;

{ TZilKinZombi }
constructor TZilKinZombi.Create;
begin
  inherited;
  m_nViewRange := 6;
  m_dwSearchTime := Random(1500) + 2500;
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 96;
  nZilKillCount := 0;
  if Random(3) = 0 then
  begin
    nZilKillCount := Random(3) + 1;
  end;
end;

destructor TZilKinZombi.Destroy;
begin
  inherited;
end;

procedure TZilKinZombi.Die;
begin
  inherited;
  // 已经获取过一次要掉的装备，不用再重新获取 chongchong 2015-09-06
  // 复活后不再掉装备  chongchong 2015-09-06
  m_boMonGetRandomItems := False;
  if nZilKillCount > 0 then
  begin
    dw558 := MyGetTickCount();
    dw560 := (Random(20) + 4) * 1000;
  end;
  Dec(nZilKillCount);
end;

procedure TZilKinZombi.Run;
begin
  if m_boDeath and (not m_boGhost) and (nZilKillCount >= 0) and CanMove and (m_VisibleActors.Count > 0) and ((MyGetTickCount -
    dw558) >= dw560) then
  begin
    m_Abil.MaxHP := m_Abil.MaxHP shr 1;
    m_dwFightExp := m_dwFightExp div 2;
    m_Abil.HP := m_Abil.MaxHP;
    m_WAbil.HP := m_Abil.MaxHP;
    ReAlive();
    m_dwWalkTick := MyGetTickCount;
    m_nWalkDelay := 1000;
  end;
  inherited;
end;

{ TWhiteSkeleton }
constructor TWhiteSkeleton.Create;
begin
  inherited;
  m_boIsFirst := True;
  m_boFixedHideMode := True;
  m_btRaceServer := 100;
  m_nViewRange := 6;
end;

destructor TWhiteSkeleton.Destroy;
begin
  inherited;
end;

procedure TWhiteSkeleton.RecalcAbilitys;
begin
  inherited;
  sub_4AAD54();
end;

procedure TWhiteSkeleton.Run;
begin
  if m_boIsFirst then
  begin
    m_boIsFirst := False;
    m_btDirection := 5;
    m_boFixedHideMode := False;
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    m_dwWalkTick := MyGetTickCount;
    m_nWalkDelay := 1800;
  end;
  inherited;
end;

procedure TWhiteSkeleton.sub_4AAD54;
var
  Int64Value: Int64;
begin
  {
    // 修正骷髅宝宝攻击速度过快 chongchong 2016-02-01
    if m_Master <> nil then
    begin
    m_nNextHitTime := 3000 - m_btSlaveMakeLevel * 400;
    m_nWalkSpeed := 1200 - m_btSlaveMakeLevel * 200;
    end;
  }
  // 召唤骷髅修正 chongchong 2017-06-21
  if g_Config.boSlaveLevelupUseNewAttr then
  begin
    if m_boChangeAbility and (m_ChangeAbility.WalkSpeed > 0) then
    begin
      Int64Value := m_ChangeAbility.WalkSpeed + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9MoveSpeed;
      m_nWalkSpeed := Min(High(Cardinal), Int64Value);
    end
    else
    begin
      m_nWalkSpeed := Max(10, m_nInitWalkSpeed - m_btSlaveExpLevel * g_Config.nSlave9MoveSpeed);
    end;
    if m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
    begin
      Int64Value := m_ChangeAbility.NextHitTime + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9HitSpeed;
      m_nNextHitTime := Min(High(Cardinal), Int64Value);
    end
    else
    begin
      m_nNextHitTime := Max(100, m_nInitNextHitTime - m_btSlaveExpLevel * g_Config.nSlave9HitSpeed);
    end;
  end
  else
  begin
    if m_btSlaveMakeLevel <= 3 then
    begin
      m_nNextHitTime := Max(200, 3000 - m_btSlaveMakeLevel * 600);
      m_nWalkSpeed := Max(200, 1200 - m_btSlaveMakeLevel * 250);
    end
    else
    begin
      m_nNextHitTime := Max(200, 3000 - 3 * 600 - (m_btSlaveMakeLevel - 3) * 100);
      m_nWalkSpeed := Max(200, 1200 - 3 * 250 - (m_btSlaveMakeLevel - 3) * 50);
    end;
  end;
  // 修正宝宝变异骷髅攻击时，人物不停的使用神圣战甲术会让宝宝攻击变慢或不攻击 2019-08-09 12:22:40
  // TMonster.Run中攻击有这样的判断
  // if not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) then
  // begin
  // m_dwWalkTick := MyGetTickCount;
  // m_nWalkDelay := m_nNextHitTime;
  // end;
end;

{ TScultureMonster }
constructor TScultureMonster.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
  m_nViewRange := 7;
  m_boStoneMode := True;
  m_nCharStatusEx := STATE_STONE_MODE;
end;

destructor TScultureMonster.Destroy;
begin
  inherited;
end;

procedure TScultureMonster.MeltStone;
begin
  m_nCharStatusEx := 0;
  m_nCharStatus := GetCharStatus();
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  m_boStoneMode := False;
end;

procedure TScultureMonster.MeltStoneAll;
var
  I: Integer;
  List10: TList;
  BaseObject: TBaseObject;
begin
  MeltStone();
  List10 := TList.Create;
  GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 7, List10);
  for I := 0 to List10.Count - 1 do
  begin
    BaseObject := TBaseObject(List10.Items[I]);
    if BaseObject <> nil then
    begin
      if BaseObject.m_boStoneMode then
      begin
        if BaseObject is TScultureMonster then
        begin
          TScultureMonster(BaseObject).MeltStone
        end;
      end;
    end;
  end; // for
  List10.Free;
end;

procedure TScultureMonster.LightingAttack;
var
  BaseObjectList: TList;
  I: Integer;
  NewHP: Integer;
  TargeTBaseObject: TBaseObject;
begin
  SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(Self), '');
  // 修改为范围群体攻击 piaoyun 2013-11-12
  BaseObjectList := TList.Create;
  try
    // 遍历范围内的对象
    m_TargetCret.GetMapBaseObjects(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, BaseObjectList);
    // 开始攻击遍历出来的链表对象
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (TargeTBaseObject <> nil) and (not TargeTBaseObject.m_boDeath) and (not TargeTBaseObject.m_boGhost) and (not
        TargeTBaseObject.m_boHideMode or m_boCoolEye) and IsProperTarget(TargeTBaseObject) and (not TargeTBaseObject.UnParalysis)
        and (Random(5) = 0) then
        TargeTBaseObject.MakePosion(POISON_STONE, 3, 0); // 目标麻痹3秒
    end;
  finally
    BaseObjectList.Free;
  end;
  NewHP := Min(m_WAbil.HP + Round(m_WAbil.HP * 5 / 100), m_WAbil.MaxHP);
  if m_WAbil.HP <> NewHP then
  begin
    m_WAbil.HP := NewHP;
    HealthSpellChanged;
  end;
end;

procedure TScultureMonster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if (not m_boGhost) and (not m_boDeath) and CanMove and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay)
    then
  begin
    m_nWalkDelay := 0;
    if m_boStoneMode then
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
                  MeltStoneAll();
                  Break;
                end;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end
    else
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();
      end;
    end;
    // 魔龙教主 chongchong 2014-11-14
    if (m_TargetCret <> nil) and (m_wAppr = 218) and (Random(15) = 0) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) < 6) and (Abs(m_nCurrY
      - m_TargetCret.m_nCurrY) < 6) and (tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay) then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      LightingAttack;
    end;
  end;
  inherited;
end;

{ TScultureKingMonster }
constructor TScultureKingMonster.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
  m_nViewRange := 8;
  m_boStoneMode := True;
  m_nCharStatusEx := STATE_STONE_MODE;
  m_btDirection := 5;
  m_nDangerLevel := 5;
  m_SlaveObjectList := TList.Create;
end;

destructor TScultureKingMonster.Destroy;
begin
  m_SlaveObjectList.Free;
  inherited;
end;

procedure TScultureKingMonster.MeltStone;
var
  Event: TGameEvent;
begin
  m_nCharStatusEx := 0;
  m_nCharStatus := GetCharStatus();
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  m_boStoneMode := False;
  Event := TGameEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, 6, 5 * 60 * 1000, True);
  g_EventManager.AddEvent(Event);
  // 魔龙教主点亮区域调大 chongchong 2015-03-04
  // if m_wAppr = 218 then
  // m_nLight := 3;
end;

procedure TScultureKingMonster.CallSlave;
var
  I: Integer;
  nC: Integer;
  n10, n14: Integer;
  BaseObject: TBaseObject;
begin
  nC := Random(6) + 6;
  GetFrontPosition(n10, n14);
  for I := 1 to nC do
  begin
    if m_SlaveObjectList.Count >= 30 then
      Break;
    BaseObject := UserEngine.RegenMonsterByName(m_sMapName, n10, n14, g_Config.sZuma[Random(4)]);
    if BaseObject <> nil then
    begin
      m_SlaveObjectList.Add(BaseObject);
    end;
  end; // for
end;

procedure TScultureKingMonster.Attack(TargeTBaseObject: TBaseObject; nDir: Integer);
var
  WAbil: pTAbility;
  nPower: Integer;
begin
  if TargeTBaseObject <> nil then
  begin
    WAbil := @m_WAbil;
    nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    HitMagAttackTarget(TargeTBaseObject, 0, nPower, True);
  end;
end;

procedure TScultureKingMonster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if (not m_boGhost) and (not m_boDeath) and CanMove and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay)
    then
  begin
    m_nWalkDelay := 0;
    if m_boStoneMode then
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
                  MeltStone();
                  Break;
                end;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end
    else
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();
        // CallSlave(); //测试用
        if (m_nDangerLevel > m_WAbil.HP / m_WAbil.MaxHP * 5) and (m_nDangerLevel > 0) then
        begin
          Dec(m_nDangerLevel);
          CallSlave();
        end;
        if m_WAbil.HP = m_WAbil.MaxHP then
          m_nDangerLevel := 5;
      end;
    end;
    for I := m_SlaveObjectList.Count - 1 downto 0 do
    begin
      if m_SlaveObjectList.Count <= 0 then
        Break;
      BaseObject := TBaseObject(m_SlaveObjectList.Items[I]);
      if BaseObject <> nil then
      begin
        if BaseObject.m_boDeath or BaseObject.m_boGhost then
          m_SlaveObjectList.Delete(I);
      end;
    end; // for
  end;
  inherited;
end;

{ TGasMothMonster }
constructor TGasMothMonster.Create;
begin
  inherited;
  m_nViewRange := 7;
end;

destructor TGasMothMonster.Destroy;
begin
  inherited;
end;

function TGasMothMonster.sub_4A9C78(bt05: Byte): TBaseObject;
var
  BaseObject: TBaseObject;
begin
  BaseObject := inherited sub_4A9C78(bt05);
  if (BaseObject <> nil) and (Random(3) = 0) and (BaseObject.m_boHideMode) then
  begin
    BaseObject.m_wStatusTimeArr[STATE_TRANSPARENT { 8 0x70 } ] := 1;
  end;
  Result := BaseObject;
end;

procedure TGasMothMonster.Run;
begin
  if (not m_boDeath) and (not bo554) and (not m_boGhost) and CanMove and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed
    + m_nWalkDelay) then
  begin
    m_nWalkDelay := 0;
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      sub_4C959C();
    end;
  end;
  inherited;
end;

{ TGasDungMonster }
constructor TGasDungMonster.Create;
begin
  inherited;
  m_nViewRange := 7;
end;

destructor TGasDungMonster.Destroy;
begin
  inherited;
end;

{ TElfMonster }
procedure TElfMonster.AppearNow; // 神兽
begin
  boIsFirst := False;
  m_boFixedHideMode := False;
  // SendRefMsg (RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  // Appear;
  // ResetElfMon;
  RecalcAbilitys;
  // m_dwWalkTick := m_dwWalkTick + 800;                       //
  m_nWalkDelay := 800;
  dwAppearNowTick := MyGetTickCount;
end;

constructor TElfMonster.Create;
begin
  inherited;
  m_nViewRange := 6;
  m_boFixedHideMode := True;
  m_boNoAttackMode := True;
  boIsFirst := True;
  dwAppearNowTick := MyGetTickCount;
end;

destructor TElfMonster.Destroy;
begin
  inherited;
end;

procedure TElfMonster.RecalcAbilitys;
begin
  inherited;
  ResetElfMon();
end;

procedure TElfMonster.ResetElfMon();
var
  Int64Value: Int64;
begin
  if g_Config.boSlaveLevelupUseNewAttr then
  begin
    if m_boChangeAbility and (m_ChangeAbility.WalkSpeed > 0) then
    begin
      Int64Value := m_ChangeAbility.WalkSpeed + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9MoveSpeed;
      m_nWalkSpeed := Min(High(Cardinal), Int64Value);
    end
    else
    begin
      m_nWalkSpeed := Max(10, m_nInitWalkSpeed - m_btSlaveExpLevel * g_Config.nSlave9MoveSpeed);
    end;
    if m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
    begin
      Int64Value := m_ChangeAbility.NextHitTime + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9HitSpeed;
      m_nNextHitTime := Min(High(Cardinal), Int64Value);
    end
    else
    begin
      m_nNextHitTime := Max(100, m_nInitNextHitTime - m_btSlaveExpLevel * g_Config.nSlave9HitSpeed);
    end;
  end
  else
  begin
    if m_btSlaveMakeLevel <= 3 then
    begin
      m_nWalkSpeed := 500 - m_btSlaveMakeLevel * 50;
    end
    else
    begin
      m_nWalkSpeed := 500 - 3 * 50 - (m_btSlaveMakeLevel - 3) * 20;
    end;
    m_dwWalkTick := MyGetTickCount;
    m_nWalkDelay := 2000;
  end;
end;

procedure TElfMonster.Run;
var
  boChangeFace: Boolean;
  ElfMon: TBaseObject;
  ErrorCode: Integer;
begin
  ErrorCode := 0;
  try
    ErrorCode := 1;
    if boIsFirst then
    begin
      ErrorCode := 2;
      boIsFirst := False;
      m_boFixedHideMode := False;
      SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
      ErrorCode := 3;
      ResetElfMon();
    end;
    ErrorCode := 4;
    if m_boDeath then
    begin
      ErrorCode := 5;
      if (MyGetTickCount - m_dwDeathTick > 2 * 1000) then
      begin
        ErrorCode := 6;
        MakeGhost();
      end;
    end
    else
    begin
      ErrorCode := 7;
      boChangeFace := False;
      ErrorCode := 8;
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        ErrorCode := 9;
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();
      end;
      // 主人不攻击，别人只攻击宝宝时，宝宝不动 chongchong 2017-06-30
      // if m_TargetCret <> nil then boChangeFace := True;
      ErrorCode := 10;
      if (m_Master = nil) or (
        // 这里的逻辑为有主人
        ((m_Master <> nil) and ((m_Master.m_TargetCret <> nil) or (m_Master.m_LastHiter <> nil))) or
        // 人物或英雄打宝宝不还击
        ((m_TargetCret <> nil) and (m_TargetCret.m_Master = nil) and (not (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT,
        RC_HEROOBJECT])))) then
        boChangeFace := True;
      ErrorCode := 11;
      if boChangeFace and (MyGetTickCount - dwAppearNowTick >= 2000) then
      begin
        ErrorCode := 12;
        // ElfMon:=MakeClone(sDogz1,Self);
        ElfMon := MakeClone(m_sCharName + '1', Self);
        if ElfMon <> nil then
        begin
          ErrorCode := 13;
          SendRefMsg(RM_DISAPPEAR, 0, NativeInt(ElfMon), 0, 0, '');
          ErrorCode := 14;
          ElfMon.m_boAutoChangeColor := m_boAutoChangeColor;
          ElfMon.m_nSlaveAttackHumPowerRate := m_nSlaveAttackHumPowerRate;
          if ElfMon is TElfWarriorMonster then
            TElfWarriorMonster(ElfMon).AppearNow; // SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
          m_Master := nil;
          ErrorCode := 15;
          if m_WAbil.HP > ElfMon.m_WAbil.HP then
          begin
            if m_WAbil.HP <= ElfMon.m_WAbil.MaxHP then
              ElfMon.m_WAbil.HP := m_WAbil.HP
            else
              ElfMon.m_WAbil.HP := ElfMon.m_WAbil.MaxHP;
          end;
          ErrorCode := 16;
          KickException();
        end;
      end;
    end;
    ErrorCode := 30;
    inherited;
  except
    MainOutMessage('TElfMonster.Run Error: ' + IntToStr(ErrorCode));
  end;
end;

{ TElfWarriorMonster 攻击状态 }
procedure TElfWarriorMonster.AppearNow;
begin
  boIsFirst := False;
  m_boFixedHideMode := False;
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  RecalcAbilitys;
  // m_dwWalkTick := m_dwWalkTick + 800;
  m_nWalkDelay := 800;
  dwDigDownTick := MyGetTickCount();
end;

constructor TElfWarriorMonster.Create;
begin
  inherited;
  m_nViewRange := 6;
  m_boFixedHideMode := True;
  boIsFirst := True;
  m_boUsePoison := False;
end;

destructor TElfWarriorMonster.Destroy;
begin
  inherited;
end;

procedure TElfWarriorMonster.RecalcAbilitys;
begin
  inherited;
  ResetElfMon();
end;

procedure TElfWarriorMonster.ResetElfMon();
var
  Int64Value: Int64;
begin
  if g_Config.boSlaveLevelupUseNewAttr then
  begin
    if m_boChangeAbility and (m_ChangeAbility.WalkSpeed > 0) then
    begin
      Int64Value := m_ChangeAbility.WalkSpeed + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9MoveSpeed;
      m_nWalkSpeed := Min(High(Cardinal), Int64Value);
    end
    else
    begin
      m_nWalkSpeed := Max(10, m_nInitWalkSpeed - m_btSlaveExpLevel * g_Config.nSlave9MoveSpeed);
    end;
    if m_boChangeAbility and (m_ChangeAbility.NextHitTime > 0) then
    begin
      Int64Value := m_ChangeAbility.NextHitTime + (m_btSlaveExpLevel - m_ChangeAbility.SlaveExpLevel) * g_Config.nSlave9HitSpeed;
      m_nNextHitTime := Min(High(Cardinal), Int64Value);
    end
    else
    begin
      m_nNextHitTime := Max(100, m_nInitNextHitTime - m_btSlaveExpLevel * g_Config.nSlave9HitSpeed);
    end;
  end
  else
  begin
    if m_btSlaveMakeLevel <= 3 then
    begin
      m_nNextHitTime := 1500 - m_btSlaveMakeLevel * 100;
      m_nWalkSpeed := 500 - m_btSlaveMakeLevel * 50;
      m_dwWalkTick := MyGetTickCount;
      m_nWalkDelay := 2000;
    end
    else
    begin
      m_nNextHitTime := 1500 - 3 * 100 - (m_btSlaveMakeLevel - 3) * 100;
      m_nWalkSpeed := 500 - 3 * 50 - (m_btSlaveMakeLevel - 3) * 30;
      m_dwWalkTick := MyGetTickCount;
      m_nWalkDelay := 2000;
    end;
  end;
end;

function TElfWarriorMonster.AttackTarget: Boolean;
begin
  { TODO -ochongchong -c新增 : 英雄召唤的宝宝在安全区停止攻击 【2013-08-15】 }
  if (m_Master <> nil) and (m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT,
    RC_HEROOBJECT]) and m_TargetCret.InSafeZone then
  begin
    Result := False;
  end
  else
  begin
    Result := inherited AttackTarget;
  end;
end;

procedure TElfWarriorMonster.Run;
var
  boChangeFace: Boolean;
  ElfMon: TBaseObject;
  ElfName: string;
begin
  if boIsFirst then
  begin
    boIsFirst := False;
    m_boFixedHideMode := False;
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    ResetElfMon();
  end;
  if m_boDeath then
  begin
    if (MyGetTickCount - m_dwDeathTick > 2 * 1000) then
    begin
      MakeGhost();
    end;
  end
  else
  begin
    boChangeFace := True;
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    {
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then
      begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
      end;
    }
    if m_TargetCret <> nil then
      boChangeFace := False;
    if (m_Master <> nil) and (
      // 这里的逻辑为有主人
      ((m_Master.m_TargetCret <> nil) or (m_Master.m_LastHiter <> nil))) then
      boChangeFace := False;
    if boChangeFace then
    begin
      if (MyGetTickCount - dwDigDownTick) > g_Config.nElfWarriorMonsterDownDelay * 1000 then
      begin
        ElfName := m_sCharName;
        ElfMon := nil;
        if ElfName[Length(ElfName)] = '1' then
        begin
          ElfName := Copy(ElfName, 1, Length(ElfName) - 1);
          ElfMon := MakeClone(ElfName, Self);
        end;
        if ElfMon <> nil then
        begin
          ElfMon.m_boCheckHPOutOfRange := False;
          SendRefMsg(RM_DIGDOWN, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
          SendRefMsg(RM_CHANGEFACE, 0, NativeInt(Self), NativeInt(ElfMon), 0, '');
          // MainOutMessage(format('%d    %d', [NativeInt(Self), NativeInt(ElfMon)]));
          ElfMon.m_boAutoChangeColor := m_boAutoChangeColor;
          ElfMon.m_nSlaveAttackHumPowerRate := m_nSlaveAttackHumPowerRate;
          if ElfMon is TElfMonster then
            TElfMonster(ElfMon).AppearNow;
          if m_WAbil.HP > ElfMon.m_WAbil.HP then
          begin
            ElfMon.m_WAbil.HP := m_WAbil.HP;
          end;
          m_Master := nil;
          KickException();
        end
        else
        begin
          dwDigDownTick := MyGetTickCount();
        end;
      end;
    end
    else
    begin
      dwDigDownTick := MyGetTickCount();
    end;
  end;
  inherited;
end;

{ TElectronicScolpionMon }
constructor TElectronicScolpionMon.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
  m_boUseMagic := False;
  m_boIsFirst := True;
end;

destructor TElectronicScolpionMon.Destroy;
begin
  inherited;
end;

procedure TElectronicScolpionMon.LightingAttack(nDir: Integer);
var
  WAbil: pTAbility;
  nPower, nDamage: Integer;
  btGetBackHP: Integer;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  if m_TargetCret <> nil then
  begin
    m_btDirection := nDir;
    WAbil := @m_WAbil;
    nPower := GetAttackPower(WAbil.MC1, WAbil.MC2 - WAbil.MC1);
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
        + m_dwParalysisRate, 0)) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    if ((m_wAppr = 619) and (Random(2) = 0)) or ((m_wAppr = 638) and (Random(5) = 0)) then
    begin
      // 中绿毒
      if (not m_TargetCret.UnPosion) then
        m_TargetCret.MakePosion(POISON_DECHEALTH, Random(30) + 30, 0);
    end
    else if (m_wAppr = 628) and (Random(3) = 0) then
    begin
      // 中绿毒
      if (not m_TargetCret.UnPosion) then
        m_TargetCret.MakePosion(POISON_DECHEALTH, Random(30) + 30, 0);
    end
    else if (m_wAppr = 622) and (Random(10) = 0) then
    begin
      // 冰冻效果
      if (not m_TargetCret.UnFrozen) then
        m_TargetCret.MakeFrozen(Random(3) + 2);
    end
    else if (m_wAppr = 619) then
    begin
      SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(Self), '');
    end
    else
      SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;
end;

procedure TElectronicScolpionMon.RefreshAppr;
begin
  if (m_wAppr = 628) then
    m_boFixedHideMode := True;
end;

procedure TElectronicScolpionMon.Run;
var
  nAttackDir: Integer;
  nX, nY: Integer;
begin
  if (m_wAppr = 628) { or (m_wAppr = 628) } then
  begin
    if m_boIsFirst then
    begin
      m_boIsFirst := False;
      m_btDirection := 5;
      m_boFixedHideMode := False;
      SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    end;
  end;
  if not m_boDeath and not bo554 and not m_boGhost and CanMove then
  begin
    // 血量低于一半时开始用魔法攻击
    if m_WAbil.HP < m_WAbil.MaxHP div 2 then
      m_boUseMagic := True
    else
      m_boUseMagic := False;
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    // 修复人物隐身后攻击怪物，怪物不掉血，取消人物隐身后，怪物突然死亡 chongchong 2014-11-14
    if m_TargetCret = nil then
    begin
      inherited; // 退出前要调用基类的方法 chongchong 2014-11-14
      Exit;
    end;
    nX := Abs(m_nCurrX - m_TargetCret.m_nCurrX);
    nY := Abs(m_nCurrY - m_TargetCret.m_nCurrY);
    if (m_wAppr = 614) or (m_wAppr = 638) then
    begin
      if (nX <= 3) and (nY <= 3) then
      begin
        if ((nX = 3) or (nY = 3)) then
        begin
          if (tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay) then
          begin
            m_dwHitTick := MyGetTickCount();
            m_nHitDelay := 0;
            nAttackDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
            LightingAttack(nAttackDir);
          end;
        end;
      end;
    end
    else if (nX <= 2) and (nY <= 2) then
    begin
      if m_boUseMagic or ((nX = 2) or (nY = 2)) then
      begin
        if (tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay) then
        begin
          m_dwHitTick := MyGetTickCount();
          m_nHitDelay := 0;
          nAttackDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
          LightingAttack(nAttackDir);
        end;
      end;
    end;
  end;
  inherited Run;
end;

function TCobwebMonster.MonAttackTarget: Boolean;
var
  btDir: Byte;
  nX, nY: Integer;
  Obj: TBaseObject;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    // 修复雷炎蛛王 24-7 没有隔位攻击 (add GetAttackDir(m_TargetCret, 2, btDir) or) chongchong 2014-05-20
    if GetAttackDir(m_TargetCret, 2, btDir) or GetAttackDir(m_TargetCret, btDir) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        Attack(m_TargetCret, btDir);
        BreakHolySeizeMode();
        // 直线范围攻击 - 2格范围 piaoyun 2013-11-15
        m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, 1, nX, nY);
        Obj := m_PEnvir.GetMovingObject(nX, nY, True);
        if (Obj <> nil) and IsProperTarget(Obj) then
          Attack(m_TargetCret, btDir);
        m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, 2, nX, nY);
        Obj := m_PEnvir.GetMovingObject(nX, nY, True);
        if (Obj <> nil) and IsProperTarget(Obj) then
          Attack(m_TargetCret, btDir);
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
end;

function TCobwebMonster.CobwebWindingAttack(): Boolean; // 蜘蛛网攻击
var
  WAbil: pTAbility;
  nPower, nDamage: Integer;
  btGetBackHP: Integer;
  bt06: Byte;
  BaseObjectList: TList;
  I: Integer;
  TargeTBaseObject: TBaseObject;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    // 修复雷炎蛛王 24-7 没有隔位攻击 (add GetAttackDir(m_TargetCret, 2, btDir) or) chongchong 2014-05-20
    if GetAttackDir(m_TargetCret, 2, bt06) or GetAttackDir(m_TargetCret, bt06) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        m_btDirection := bt06;
        { WAbil := @m_WAbil;
          nPower := SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1;
          if nPower > 0 then
          nPower := Random(nPower);
          nPower := nPower + LoWord(WAbil.DC); }
        WAbil := @m_WAbil;
        nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
        if (m_Master <> nil) then
          nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
        if nPower > 0 then
        begin
          // 修改为范围群体攻击 piaoyun 2013-11-12
          BaseObjectList := TList.Create;
          try
            // 遍历范围内的对象
            m_TargetCret.GetMapBaseObjects(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, BaseObjectList);
              // 开始攻击遍历出来的链表对象
            for I := 0 to BaseObjectList.Count - 1 do
            begin
              TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
              // 检测目标是否正确
              if IsProperTarget(TargeTBaseObject) and
              // 怪物不攻击脱机人物 chongchong 2015-09-07
                (not (g_Config.boMonNoAttackOffLinePlayer and (TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(TargeTBaseObject).m_boOffLine))
                then
              begin
                if not CanCloseDefense then // 忽视目标防御
                  nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil)
                else
                  nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
                // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
                nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);
                nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
                nDamage := GetPowerRateAdd(TargeTBaseObject, nDamage);
                nDamage := GetNextDamage(nDamage);
                // 吸收伤害
                if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                begin
                  SmartObject := TSmartObject(TargeTBaseObject);
                  // 伤害吸收百分比 2020-09-17 20:11:44
                  nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
                  if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
                  begin
                    nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
                    SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                    SmartObject.RefAbilNH;
                  end;
                  if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
                  begin
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
                // 怪物伤害封顶 chongchong 2016-09-07
                nDamage := TargeTBaseObject.GetAttackPowerMax(nDamage);
                btGetBackHP := LoByte(m_WAbil.MP);
                if btGetBackHP <> 0 then
                  Inc(m_WAbil.HP, nDamage div btGetBackHP);
                nDamage := TargeTBaseObject.StruckDamage(nDamage, Self, 0);
                TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, TargeTBaseObject.m_WAbil.HP,
                  TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', 200);
                if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random
                  (Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
                begin
                  TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
                end;
                nPower := TargeTBaseObject.DamageReboundPower(nDamage);
                if nPower > 0 then
                begin // 反弹伤害
                  nPower := StruckDamage(nPower, nil, 0);
                  SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject),
                    'FT', 200);
                end;
                // 范围内蛛网攻击 piaoyun 2013-11-12
                // 修复蜘蛛网罩住自己 piaoyun 2013-11-30
                if (TargeTBaseObject <> Self) and (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
                  TargeTBaseObject.OpenCobwebWinding(Random(5) + 2);
              end;
            end;
          finally
            BaseObjectList.Free;
          end;
        end;
        SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
        // 自身蜘蛛网
        m_TargetCret.OpenCobwebWinding(Random(5) + 2);
        BreakHolySeizeMode();
      end;
      Result := True;
    end
    else
    begin
      if m_TargetCret.m_PEnvir = m_PEnvir then
      begin
        SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY); { 0FFF0h }
      end
      else
      begin
        DelTargetCreat(); { 0FFF1h }
      end;
    end;
  end;
end;

function TCobwebMonster.AttackTarget(): Boolean; // 蜘蛛网攻击
begin
  if Random(5) = 0 then
    Result := CobwebWindingAttack
  else
    Result := MonAttackTarget;
end;

{ TMon36_XMonster }
constructor TMon36_XMonster.Create;
begin
  inherited;
  m_boIsFirst := True;
end;

destructor TMon36_XMonster.Destroy;
begin
  inherited;
end;

procedure TMon36_XMonster.RefreshAppr;
begin
  if (m_wAppr = 601) { or (m_wAppr = 628) } then
    m_boFixedHideMode := True;
end;

procedure TMon36_XMonster.Run;
begin
  if (m_wAppr = 601) then
  begin
    if m_boIsFirst then
    begin
      m_boIsFirst := False;
      m_btDirection := 5;
      // m_boFixedHideMode := False;
      SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    end;
  end;
  m_boFixedHideMode := False;
  inherited Run;
end;

function TMon36_XMonster.AttackTarget0(): Boolean;
var
  nDir: Byte;
  boBigAttack: Boolean; // 重击
begin
  Result := False;
  boBigAttack := False;
  if m_TargetCret = nil then
    Exit;
  if not GetAttackDir(m_TargetCret, nDir) then
  begin
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    end
    else
    begin
      DelTargetCreat();
    end;
    Exit;
  end;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    m_dwTargetFocusTick := MyGetTickCount();
    // Attack(m_TargetCret, bt06);
    // Mon36-11 重击威力为普通威力的2倍  (重击几率1/5,HP低于70%重击1/3)
    if (m_wAppr = 610) then
    begin //
      if (Random(5) = 0) or ((m_WAbil.HP <= Round(m_WAbil.MaxHP * 0.7)) and (Random(3) = 0)) then
      begin
        AttackDir(m_TargetCret, 0, nDir, 2, False);
        boBigAttack := True;
      end
      else
        AttackDir(m_TargetCret, 0, nDir);
    end
    // Mon36-27 重击威力为普通威力的2倍
    else if (m_wAppr = 626) then
    begin //
      if (Random(5) = 0) then
      begin
        AttackDir(m_TargetCret, 0, nDir, 2, False);
        boBigAttack := True;
      end
      else
        AttackDir(m_TargetCret, 0, nDir);
    end
    else if (m_wAppr = 600) or (m_wAppr = 609) or (m_wAppr = 616) or (m_wAppr = 620) or (m_wAppr = 609) then
      AttackDir(m_TargetCret, 0, nDir, 1, False)
    else
      AttackDir(m_TargetCret, 0, nDir);
    BreakHolySeizeMode();
    if (m_wAppr = 610) then
    begin
      if boBigAttack then
        SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(Self), ''); // 重击
    end
    // 发送特效
    else if (m_wAppr = 600) or (m_wAppr = 607) or (m_wAppr = 609) or (m_wAppr = 620) then
    begin
      if m_TargetCret <> nil then
        SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
    end
    else if (m_wAppr = 616) then
    begin
      if m_TargetCret <> nil then
        SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(Self), '');
    end
    else if (m_wAppr = 626) then // 四级烈火 -- 重击
    begin
      if boBigAttack then
        SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(Self), ''); // 重击
    end;
    if ((m_wAppr = 624) and (Random(10) = 0)) or ((m_wAppr = 625) and (Random(10) = 0)) then
    begin
      // 中绿毒
      if (not m_TargetCret.UnPosion) then
        m_TargetCret.MakePosion(POISON_DECHEALTH, Random(30) + 30, 0);
    end;
    // Mon33-10 -- 中毒参考触龙神 piaoyun 2013-12-10
    if (m_wAppr = 609) then
    begin
      if Random(3) <> 0 then
      begin
        if (Random(m_TargetCret.m_btAntiPoison + 5) = 0) and (not m_TargetCret.UnPosion) then
          m_TargetCret.MakePosion(POISON_DECHEALTH, 60, 3);
      end;
    end;
    // Mon37-0  Mon37-1  Mon37-2  Mon37-4
    if ((m_wAppr = 360) or (m_wAppr = 362)) and (Random(15) = 0) then
    begin
      if (not m_TargetCret.UnParalysis) then
        m_TargetCret.MakePosion(POISON_STONE, Random(3) + 2, 0);
    end;
  end;
  Result := True;
end;

function TMon36_XMonster.AttackTarget36_5: Boolean;

  function GetRangeTargetCount(nX, nY, nRange: Integer): Integer;
  var
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    I: Integer;
  begin
    Result := 0;
    BaseObjectList := TList.Create;
    if GetMapBaseObjects(m_PEnvir, nX, nY, nRange, BaseObjectList) then
    begin
      for I := BaseObjectList.Count - 1 downto 0 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
        begin
          BaseObjectList.Delete(I);
        end;
      end;
      Result := BaseObjectList.Count;
    end;
    BaseObjectList.Free;
  end;

var
  nDir: Byte;
  BaseObjectList: TList;
  I: Integer;
  BaseObject: TBaseObject;
  nPower, nDamage: Integer;
  WAbil: pTAbility;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if not GetAttackDir(m_TargetCret, nDir) then
  begin
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    end
    else
    begin
      DelTargetCreat();
    end;
    Exit;
  end;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    m_dwTargetFocusTick := MyGetTickCount();
    WAbil := @m_WAbil;
    nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    BaseObjectList := TList.Create;
    try
      if (m_wAppr = 364) then
      begin
        GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 1, BaseObjectList);
        AttackDir(m_TargetCret, 0, nDir, 1, False);
      end
      else
        GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 3, BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject <> nil) and (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
          Continue;
        // 怪物不攻击脱机人物 chongchong 2015-09-07
        if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
          then
        begin
          Continue;
        end;
        if not CanCloseDefense then // 忽视目标防御
          nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
        else
          nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
        nDamage := BaseObject.NewAbilPower(3, nDamage);
        nDamage := GetPowerRateAdd(BaseObject, nDamage);
        nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
        nDamage := GetNextDamage(nDamage);
        // 怪物伤害封顶 chongchong 2016-09-07
        nDamage := BaseObject.GetAttackPowerMax(nDamage);
        // 吸收伤害
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(BaseObject);
          // 伤害吸收百分比 2020-09-17 20:11:44
          nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin
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
        if nDamage > 0 then
        begin
          nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 200);
          nDamage := BaseObject.DamageReboundPower(nDamage);
          if nDamage > 0 then
          begin // 反弹伤害
            nDamage := StruckDamage(nDamage, nil, 0);
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
          end;
        end;
        if (m_wAppr = 626) then
        begin
          // 中绿毒
          if (not BaseObject.UnPosion) then
            BaseObject.MakePosion(POISON_DECHEALTH, 60 + Random(30), 0);
        end;
        if ((m_wAppr = 364) or (m_wAppr = 365)) and (Random(10) = 0) then
        begin
          // 石化
          if (not BaseObject.UnParalysis) then
            BaseObject.MakePosion(POISON_STONE, Random(3) + 4, 0);
        end;
      end;
      if (m_wAppr = 607) then
        SendRefMsg(RM_LIGHTINGEX, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')
      else
        SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(Self), '');
    finally
      BaseObjectList.Free;
    end;
  end;
  Result := True;
end;

// 气功波攻击
function TMon36_XMonster.MagPushArround(PlayObject: TBaseObject): Integer;
var
  I, nDir, push: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  Result := 0;
  // boPushSameLevel := True;
  PlayObject.m_VisibleActors.Lock;
  try
    for I := 0 to PlayObject.m_VisibleActors.Count - 1 do
    begin
      VisibleBaseObject := PlayObject.m_VisibleActors[I].Item;
      if VisibleBaseObject <> nil then
      begin
        BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
        if (Abs(PlayObject.m_nCurrX - BaseObject.m_nCurrX) <= 1) and (Abs(PlayObject.m_nCurrY - BaseObject.m_nCurrY) <= 1) then
        begin
          if (not BaseObject.m_boDeath) and (BaseObject <> PlayObject) and (not BaseObject.m_boStickMode) then
          begin
            if ((PlayObject.m_Abil.Level > BaseObject.m_Abil.Level) or ( { boPushSameLevel and } (PlayObject.m_Abil.Level =
              BaseObject.m_Abil.Level))) then
            begin
              { levelgap := PlayObject.m_Abil.Level - BaseObject.m_Abil.Level;
                if boPushSameLevel and (PlayObject.m_Abil.Level = BaseObject.m_Abil.Level) then
                nValue := Random(10)
                else
                nValue := Random(20);
              }
              // if (nValue < 6 + nPushLevel * 3 + levelgap) then
              // begin
              if PlayObject.IsProperTarget(BaseObject) then
              begin
                push := 1 + { MAX(0, nPushLevel - 1) } + Random(5);
                nDir := GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, BaseObject.m_nCurrX, BaseObject.m_nCurrY);
                BaseObject.CharPushed(nDir, push);
                Inc(Result);
              end;
              // end;
            end;
          end;
        end;
      end;
    end;
  finally
    PlayObject.m_VisibleActors.UnLock;
  end;
end;

// 远程魔法攻击
procedure TMon36_XMonster.MagicAttack;
var
  nPower: Integer;
  nDamage: Integer;
  btGetBackHP: Byte;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
  nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
  if (m_Master <> nil) then
    nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
  if not CanCloseDefense then // 忽视目标防御
    nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
  else
    nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
  nDamage := m_TargetCret.NewAbilPower(3, nDamage);
  nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
  nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
  nDamage := GetNextDamage(nDamage);
  // 怪物伤害封顶 chongchong 2016-09-07
  nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
  // 吸收伤害
  if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
  begin
    SmartObject := TSmartObject(m_TargetCret);
    // 伤害吸收百分比 2020-09-17 20:11:44
    nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
    if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
    begin
      nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
      SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
      SmartObject.RefAbilNH;
    end;
    if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
    begin
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
  if nDamage > 0 then
  begin
    btGetBackHP := LoByte(m_WAbil.MP);
    if btGetBackHP <> 0 then
      Inc(m_WAbil.HP, nDamage div btGetBackHP);
    nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
    m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
      NativeInt(Self), '', 200);
    if (m_wAppr = 628) then
    begin
      // 中绿毒
      if (not m_TargetCret.UnPosion) then
        m_TargetCret.MakePosion(POISON_DECHEALTH, 3, 0);
    end;
    nDamage := m_TargetCret.DamageReboundPower(nDamage);
    if nDamage > 0 then
    begin // 反弹伤害
      nDamage := StruckDamage(nDamage, nil, 0);
      SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
    end;
  end;
  // SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
end;

// 群体魔法攻击 piaoyun 2013-12-12
procedure TMon36_XMonster.MagicAttackGroup(boSelfRage: Boolean; nRage: Integer);
var
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower, nDamage, I: Integer;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  BaseObjectList := TList.Create;
  try
    if boSelfRage then
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nRage, BaseObjectList)
    else
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nRage, BaseObjectList);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
        Continue;
      // 怪物不攻击脱机人物 chongchong 2015-09-07
      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
      begin
        Continue;
      end;
      if not CanCloseDefense then // 忽视目标防御
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := BaseObject.NewAbilPower(3, nDamage);
      nDamage := GetPowerRateAdd(BaseObject, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := BaseObject.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
        BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
          NativeInt(Self), '', 200);
        nDamage := BaseObject.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
        end;
      end;
      // 中绿毒
      if m_wAppr = 620 then
      begin
        if (not BaseObject.UnPosion) then
          BaseObject.MakePosion(POISON_DECHEALTH, Random(30) + 30, 0);
      end;
    end;
    if m_wAppr = 620 then
      SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(Self), '')
    else
      SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')
  finally
    BaseObjectList.Free;
  end;
end;

// 半月攻击
(*
  function TMon36_XMonster.SwordWideAttack(nSecPwr: Integer): Boolean;
  var
  nC, n10, nPower, nPower1, nPower2, nAttackPower1, nAttackPower2, nDefensePower: Integer;
  nX, nY: Integer;
  BaseObject: TBaseObject;
  begin
  Result := False;
  nC := 0;
  if (nSecPwr > 0) then
  begin
  { if m_MagicBanwolSkill <> nil then
  begin
  nPower1 := nSecPwr;
  nPower2 := Min(Cardinal(Round(nSecPwr / (m_MagicBanwolSkill.MagicInfo.btTrainLv + 10) * (m_MagicBanwolSkill.btLevel + 2))), High(Integer));
  nAttackPower1 := GetSkillAttackPowerNG(Self, m_MagicBanwolSkill, nPower1);
  nAttackPower2 := GetSkillAttackPowerNG(Self, m_MagicBanwolSkill, nPower2);
  end
  else
  begin
  nPower1 := nSecPwr;
  nPower2 := nSecPwr;
  end;}
  while (True) do
  begin
  n10 := (m_btDirection + g_Config.WideAttack[nC]) mod 8;
  if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, n10, 1, nX, nY) then
  begin
  BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
  if (BaseObject <> nil) and IsProperTarget(BaseObject) then
  begin
  if m_TargetCret = BaseObject then
  begin                                                                                     // 正面攻击怪物
  //nDefensePower := GetSkillDefensePowerNG(BaseObject, m_MagicBanwolSkill, nPower1);
  nPower := GetSkillLastPowerNG(nPower1, nAttackPower1, nDefensePower);
  nPower := GetMagicPercentPower(25, nPower, BaseObject);
  if nPower > 0 then
  Result := DirectAttack(BaseObject, nPower, 25);
  end
  else
  begin                                                                                     // 其他方向的怪物 攻击减小
  nDefensePower := GetSkillDefensePowerNG(BaseObject, m_MagicBanwolSkill, nPower2);
  nPower := GetSkillLastPowerNG(nPower2, nAttackPower2, nDefensePower);
  nPower := GetMagicPercentPower(25, nPower, BaseObject);
  if nPower > 0 then
  Result := DirectAttack(BaseObject, nPower, 25);
  end;
  SetTargetCreat(BaseObject);
  end;
  end;
  Inc(nC);
  if nC >= 3 then Break;
  end;
  end;
  end;
*)
function TMon36_XMonster.AttackTarget: Boolean;
var
  nX, nY: Integer;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if (m_wAppr = 364) then
  begin
    Result := AttackTarget36_5;
    Exit;
  end;
  if (m_wAppr = 626) and (Random(8) = 0) then
  begin
    AttackTarget36_5;
    Exit;
  end;
  if (m_wAppr = 604) or (m_wAppr = 605) or (m_wAppr = 607) or (m_wAppr = 623) then
  begin
    if (Random(3) = 0) then
    begin
      AttackTarget36_5;
      Exit;
    end;
  end;
  if (m_wAppr = 620) then
  begin
    if (Random(5) = 0) then
    begin
      MagicAttackGroup(False, 2);
      Exit;
    end;
    // 雷电术
  end;
  // 远程魔法攻击
  if (m_wAppr = 365) then
  begin
    if (Random(3) = 0) then
    begin
      MagicAttack;
      Exit;
    end;
  end;
  // 远程魔法攻击
  if (m_wAppr = 366) then
  begin
    nX := Abs(m_nCurrX - m_TargetCret.m_nCurrX);
    nY := Abs(m_nCurrY - m_TargetCret.m_nCurrY);
    if (nX <= 6) and (nY <= 6) and (nX >= 2) and (nY >= 2) then
    begin
      MagicAttackGroup;
    end
    else if (Random(3) = 0) then
    begin
      AttackTarget36_5;
      Exit;
    end;
  end;
  // Mon36-15 近程物理攻击 + 远程魔法
  // Mon36-20 远程吐口水攻击
  // MON36-22
  if (m_wAppr = 621) or (m_wAppr = 628) or (m_wAppr = 629) then
  begin
    nX := Abs(m_nCurrX - m_TargetCret.m_nCurrX);
    nY := Abs(m_nCurrY - m_TargetCret.m_nCurrY);
    if (nX <= 6) and (nY <= 6) and (nX >= 2) and (nY >= 2) then
    begin
      MagicAttack;
      Exit;
    end;
  end;
  // Mon36-19, Mon36-28
  // Mon36-17 气功波 + 物理攻击
  if (m_wAppr = 616) and (Random(8) = 0) then
  begin
    SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(Self), '');
    MagPushArround(TBaseObject(Self));
    Exit;
  end;
  // 默认攻击
  Result := AttackTarget0;
end;

{ ----------------------------TTwoKindAttackMonster---------------------------------- }
// 使用物理攻击 piaoyun 2013-11-20
function TTwoKindAttackMonster.OneAttack: Boolean;
var
  bt06: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, bt06) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        Attack(m_TargetCret, bt06);
        BreakHolySeizeMode();
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
end;

// 使用魔法攻击 piaoyun 2013-11-20
function TTwoKindAttackMonster.TwoAttack(): Boolean;

  function GetRangeTargetCount(nX, nY, nRange: Integer): Integer;
  var
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    I: Integer;
  begin
    Result := 0;
    BaseObjectList := TList.Create;
    if GetMapBaseObjects(m_PEnvir, nX, nY, nRange, BaseObjectList) then
    begin
      for I := BaseObjectList.Count - 1 downto 0 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) or (g_Config.boMonNoAttackOffLinePlayer
          and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine) then
        begin
          BaseObjectList.Delete(I);
        end;
      end;
      Result := BaseObjectList.Count;
    end;
    BaseObjectList.Free;
  end;

var
  nX, nY: Integer;
  MonObj: TBaseObject;
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  WAbil: pTAbility;
  nPower, nDamage: Integer;
  btGetBackHP: Integer;
  bt06: Byte;
  nHitCmd: Integer;
  nAttackRange: Integer;
  Monster: pTMonInfo;
  sMakeMonName: string;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  nAttackRange := 8;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, bt06) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        if (m_wAppr = 255) then
          nHitCmd := 1
        else
          nHitCmd := Random(2);
        m_btDirection := bt06;
        WAbil := @m_WAbil;
        nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
        if (m_Master <> nil) then
          nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
        // ------------------------------------Mon26-5 群攻 冰冻-------------------------------------
        // 修复Mon26-5 怪物 攻击范围以及几率 piaoyun 2013-11-15
        if (m_wAppr = 255) then
        begin
          if (m_WAbil.HP < Round(m_WAbil.MaxHP * 0.8)) and (MyGetTickCount - FFrozenTick > 15 * 1000 { 大于15秒 } ) then
          begin
            // 血量小于 80% 开始触发 8*8范围冰冻
            FFrozenTick := MyGetTickCount;
            BaseObjectList := TList.Create;
            try
              GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nAttackRange, BaseObjectList);
              for I := 0 to BaseObjectList.Count - 1 do
              begin
                BaseObject := TBaseObject(BaseObjectList.Items[I]);
                if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
                  Continue;
                // 怪物不攻击脱机人物 chongchong 2015-09-07
                if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
                  then
                begin
                  Continue;
                end;
                if not CanCloseDefense then // 忽视目标防御
                  nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
                else
                  nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
                // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
                nDamage := BaseObject.NewAbilPower(3, nDamage);
                nDamage := GetPowerRateAdd(BaseObject, nDamage);
                nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
                nDamage := GetNextDamage(nDamage);
                // 怪物伤害封顶 chongchong 2016-09-07
                nDamage := BaseObject.GetAttackPowerMax(nDamage);
                // 吸收伤害
                if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                begin
                  SmartObject := TSmartObject(BaseObject);
                  // 伤害吸收百分比 2020-09-17 20:11:44
                  nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
                  if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
                  begin
                    nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
                    SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                    SmartObject.RefAbilNH;
                  end;
                  if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
                  begin
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
                if nDamage > 0 then
                begin
                  btGetBackHP := LoByte(m_WAbil.MP);
                  if btGetBackHP <> 0 then
                    Inc(m_WAbil.HP, nDamage div btGetBackHP);
                  nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
                  BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                    NativeInt(Self), '', 200);
                  BaseObject.MakeFrozen(3);
                  nDamage := BaseObject.DamageReboundPower(nDamage);
                  if nDamage > 0 then
                  begin // 反弹伤害
                    nDamage := StruckDamage(nDamage, nil, 0);
                    SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT',
                      200);
                  end;
                end;
              end;
            finally
              BaseObjectList.Free;
            end;
            SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
            BreakHolySeizeMode();
            Result := True;
            Exit;
          end
          // 2 * 2范围攻击
          // else if ((Random(2) = 0) and (GetRangeTargetCount(m_nCurrX, m_nCurrY, 2) >= 2)  or Random(5) = 0) then
          else if (Random(Max(1, 5 - GetRangeTargetCount(m_nCurrX, m_nCurrY, 2))) = 0) then
          begin // Mon26-5 群攻 冰冻
            BaseObjectList := TList.Create;
            try
              if GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 2, BaseObjectList) then
              begin
                { for I := 0 to BaseObjectList.Count - 1 do
                  begin
                  BaseObject := TBaseObject(BaseObjectList.Items[I]);
                  if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then Continue;
                  BaseObject.MakeFrozen(Random(12) + 3);                                                // 冰冻
                  end; }
                for I := 0 to BaseObjectList.Count - 1 do
                begin
                  BaseObject := TBaseObject(BaseObjectList.Items[I]);
                  if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
                    Continue;
                  // 怪物不攻击脱机人物 chongchong 2015-09-07
                  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
                    then
                  begin
                    Continue;
                  end;
                  if not CanCloseDefense then // 忽视目标防御
                    nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
                  else
                    nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
                  // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
                  nDamage := BaseObject.NewAbilPower(3, nDamage);
                  nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
                  nDamage := GetNextDamage(nDamage);
                  // 怪物伤害封顶 chongchong 2016-09-07
                  nDamage := BaseObject.GetAttackPowerMax(nDamage);
                  if nDamage > 0 then
                  begin
                    btGetBackHP := LoByte(m_WAbil.MP);
                    if btGetBackHP <> 0 then
                      Inc(m_WAbil.HP, nDamage div btGetBackHP);
                    nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
                    BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                      NativeInt(Self), '', 200);
                    nDamage := BaseObject.DamageReboundPower(nDamage);
                    if nDamage > 0 then
                    begin // 反弹伤害
                      nDamage := StruckDamage(nDamage, nil, 0);
                      SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject),
                        'FT', 200);
                    end;
                  end;
                end;
              end;
            finally
              BaseObjectList.Free;
            end;
            SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
            BreakHolySeizeMode();
            Result := True;
            Exit;
          end;
        end;
        // ------------------------------------Mon26-0  Mon26-1 群体攻击-------------------------
        if (m_wAppr = 250) or (m_wAppr = 251) then
        begin // Mon26-0  Mon26-1 群体攻击
          BaseObjectList := TList.Create;
          if GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, BaseObjectList) then
          begin
            for I := 0 to BaseObjectList.Count - 1 do
            begin
              BaseObject := TBaseObject(BaseObjectList.Items[I]);
              if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
                Continue;
              if not CanCloseDefense then // 忽视目标防御
                nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
              else
                nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
              nDamage := BaseObject.NewAbilPower(3, nDamage);
              nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
              nDamage := GetNextDamage(nDamage);
              /// 怪物伤害封顶 chongchong 2016-09-07
              nDamage := BaseObject.GetAttackPowerMax(nDamage);
              if nDamage > 0 then
              begin
                btGetBackHP := LoByte(m_WAbil.MP);
                if btGetBackHP <> 0 then
                  Inc(m_WAbil.HP, nDamage div btGetBackHP);
                nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
                BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                  NativeInt(Self), '', 200);
                if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max
                  (BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
                begin
                  BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
                end;
                nDamage := BaseObject.DamageReboundPower(nDamage);
                if nDamage > 0 then
                begin // 反弹伤害
                  nDamage := StruckDamage(nDamage, nil, 0);
                  SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT',
                    200);
                end;
              end;
            end;
          end;
          SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
          BreakHolySeizeMode();
          BaseObjectList.Free;
          Result := True;
          Exit;
        end;
        // ------------------------------------Mon26-6 群体攻击-------------------------
        if (m_wAppr = 256) then
        begin // Mon26-6 群体攻击
          if (m_WAbil.HP < Round(m_WAbil.MaxHP / 3)) and (Random(3) = 0) and (m_SlaveList.Count <= 0) then
          begin
            sMakeMonName := m_sCharName;
{$IF MULTI_THREAD = 1}
            if g_MultiThreadRun then
              UserEngine.MonsterList.LockR(4);
            try
{$IFEND}
              for I := 0 to UserEngine.MonsterList.Count - 1 do
              begin
                Monster := UserEngine.MonsterList.Items[I];
                if (Monster.btRace = 123) and (Monster.wAppr = 267) then
                begin
                  sMakeMonName := Monster.sName;
                  Break;
                end;
              end;
{$IF MULTI_THREAD = 1}
            finally
              if g_MultiThreadRun then
                UserEngine.MonsterList.UnLockR;
            end;
{$IFEND}
            for I := 0 to 3 do
            begin
              GetFrontPosition(nX, nY);
              MonObj := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, nX, nY, sMakeMonName);
              if MonObj <> nil then
              begin
                MonObj.m_Master := Self;
                MonObj.m_dwMasterRoyaltyStartTick := MyGetTickCount;
                MonObj.m_dwMasterRoyaltyTime := 60 * 60 * 1000;
                MonObj.m_btSlaveMakeLevel := 1;
                MonObj.m_btSlaveExpLevel := 1;
                MonObj.RecalcAbilitys;
                if MonObj.m_WAbil.HP < MonObj.m_WAbil.MaxHP then
                begin
                  MonObj.m_WAbil.HP := MonObj.m_WAbil.HP + (MonObj.m_WAbil.MaxHP - MonObj.m_WAbil.HP) div 2;
                end;
                MonObj.RefNameColor;
                m_SlaveList.Add(MonObj);
              end;
            end;
          end
          else
          begin
            BaseObjectList := TList.Create;
            // 修复Mon26-6 怪物 攻击范围以及几率 piaoyun 2013-11-15
            if GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, { Random(2) + 1 } nAttackRange,
              BaseObjectList) then
            begin
              for I := 0 to BaseObjectList.Count - 1 do
              begin
                BaseObject := TBaseObject(BaseObjectList.Items[I]);
                if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
                  Continue;
                // 怪物不攻击脱机人物 chongchong 2015-09-07
                if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
                  then
                begin
                  Continue;
                end;
                if not CanCloseDefense then // 忽视目标防御
                  nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
                else
                  nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
                // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
                nDamage := BaseObject.NewAbilPower(3, nDamage);
                nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
                nDamage := GetNextDamage(nDamage);
                // 怪物伤害封顶 chongchong 2016-09-07
                nDamage := BaseObject.GetAttackPowerMax(nDamage);
                if nDamage > 0 then
                begin
                  btGetBackHP := LoByte(m_WAbil.MP);
                  if btGetBackHP <> 0 then
                    Inc(m_WAbil.HP, nDamage div btGetBackHP);
                  nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
                  BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                    NativeInt(Self), '', 200);
                  if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max
                    (BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
                  begin
                    BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
                  end;
                  nDamage := BaseObject.DamageReboundPower(nDamage);
                  if nDamage > 0 then
                  begin // 反弹伤害
                    nDamage := StruckDamage(nDamage, nil, 0);
                    SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT',
                      200);
                  end;
                end;
              end;
            end;
            BaseObjectList.Free;
          end;
          if Random(2) = 0 then
            SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')
          else
            SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
          BreakHolySeizeMode();
          Result := True;
          Exit;
        end;
        /// /////////////////Mon27-2 群体攻击 piaoyun 2013-11-15/////////////////
        if (m_wAppr = 262) and (Random(4) = 0) then
        begin
          BaseObjectList := TList.Create;
          try
            if GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 2, BaseObjectList) then
            begin
              for I := 0 to BaseObjectList.Count - 1 do
              begin
                BaseObject := TBaseObject(BaseObjectList.Items[I]);
                if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
                  Continue;
                // 怪物不攻击脱机人物 chongchong 2015-09-07
                if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
                  then
                begin
                  Continue;
                end;
                if not CanCloseDefense then // 忽视目标防御
                  nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
                else
                  nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
                // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
                nDamage := BaseObject.NewAbilPower(3, nDamage);
                nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
                nDamage := GetNextDamage(nDamage);
                // 怪物伤害封顶 chongchong 2016-09-07
                nDamage := BaseObject.GetAttackPowerMax(nDamage);
                if nDamage > 0 then
                begin
                  btGetBackHP := LoByte(m_WAbil.MP);
                  if btGetBackHP <> 0 then
                    Inc(m_WAbil.HP, nDamage div btGetBackHP);
                  nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
                  BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                    NativeInt(Self), '', 200);
                  if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max
                    (BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
                  begin
                    BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
                  end;
                  nDamage := BaseObject.DamageReboundPower(nDamage);
                  if nDamage > 0 then
                  begin // 反弹伤害
                    nDamage := StruckDamage(nDamage, nil, 0);
                    SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT',
                      200);
                  end;
                end;
              end;
            end;
          finally
            BaseObjectList.Free;
          end;
          SendRefMsg(RM_LIGHTINGEX, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
          BreakHolySeizeMode();
          Result := True;
          Exit;
        end;
        /// /////////////////////////////////////////////////////////////////////
        // ----------------------------------------------------------------------------------
        if not CanCloseDefense then // 忽视目标防御
          nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
        else
          nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
        nDamage := m_TargetCret.NewAbilPower(3, nDamage);
        nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
        nDamage := GetNextDamage(nDamage);
        // 怪物伤害封顶 chongchong 2016-09-07
        nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
        if nDamage > 0 then
        begin
          btGetBackHP := LoByte(m_WAbil.MP);
          if btGetBackHP <> 0 then
            Inc(m_WAbil.HP, nDamage div btGetBackHP);
          nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
          m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
            NativeInt(Self), '', 200);
          if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
            + m_dwParalysisRate, 0)) = 0) then
          begin
            m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
          end;
          nDamage := m_TargetCret.DamageReboundPower(nDamage);
          if nDamage > 0 then
          begin // 反弹伤害
            nDamage := StruckDamage(nDamage, nil, 0);
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
          end;
        end;
        SendRefMsg(RM_LIGHTING, nHitCmd, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
        BreakHolySeizeMode();
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
end;

constructor TTwoKindAttackMonster.Create;
begin
  inherited;
  FFrozenTick := MyGetTickCount;
end;

function TTwoKindAttackMonster.AttackTarget(): Boolean;
begin
  if Random(4) = 0 then
    Result := TwoAttack
  else
    Result := OneAttack;
end;

{ ------------------------------------------------------------------------------ }
constructor TMagicAttackMonster.Create;
begin
  inherited;
  m_nViewRange := 7;
  m_boMagicAttack := True;
end;

function TMagicAttackMonster.MagicAttackTarget: Boolean; // 人物魔法攻击
{
  procedure MagicAttack;
  var
  bt06: Byte;
  nPower: Integer;
  nDamage: Integer;
  btGetBackHP: Byte;
  wMagicID: Word;
  begin
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
  nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
  if (m_Master <> nil) then
  nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
  nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower);
  if nDamage > 0 then begin
  btGetBackHP := LoByte(m_WAbil.MP);
  if btGetBackHP <> 0 then Inc(m_WAbil.HP, nDamage div btGetBackHP);
  m_TargetCret.StruckDamage(nDamage);
  m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP, NativeInt(Self), '', 200);
  if m_TargetCret.DamageRebound then begin // 反弹伤害
  nDamage := StruckDamage(nDamage);
  SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
  end;
  end;
  SendRefMsg(RM_LIGHTINGEX, wMagicID, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end; }
begin
  Result := False;
  { if m_TargetCret = nil then Exit;
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) then begin
    if (m_nTargetX = -1) or (Random(2) = 0) then begin
    // MagicAttack;
    Result := True;
    Exit;
    end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then begin
    if (abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) then begin
    SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    end;
    end else begin
    DelTargetCreat();
    end;
    end; }
end;

function TMagicAttackMonster.AttackTarget(): Boolean;
var
  bt06: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if m_boMagicAttack then
    begin
      Result := MagicAttackTarget();
    end
    else
    begin
      if GetAttackDir(m_TargetCret, bt06) then
      begin
        if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
        begin
          m_dwHitTick := MyGetTickCount();
          m_nHitDelay := 0;
          m_dwTargetFocusTick := MyGetTickCount();
          Attack(m_TargetCret, bt06);
          BreakHolySeizeMode();
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
  end;
end;

procedure TMagicAttackMonster.Run; // 不近身，使用魔法攻击怪物
begin
  if not m_boDeath and not bo554 and not m_boGhost and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    m_nTargetX := -1;
    m_nTargetY := -1;
    if (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) and (m_TargetCret <> nil) then
    begin
      m_nWalkDelay := 0; //
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 4) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 4) then
      begin
        if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 2) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 2) then
        begin
          if Random(2) = 0 then
            GetBackPosition(m_nTargetX, m_nTargetY);
        end
        else
        begin
          if Random(5) = 0 then
            GetBackPosition(m_nTargetX, m_nTargetY);
        end;
      end;
    end;
  end;
  inherited;
  m_nTargetX := -1;
  m_nTargetY := -1;
end;

// 冰咆哮怪物
function TExplosionAttackMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    I: Integer;
    BaseObjectList: TList;
    TargeTBaseObject: TBaseObject;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if (m_wAppr = 231) then
    begin // Mon24-1
      if (m_WAbil.HP < Round(m_WAbil.MaxHP / 2)) and (Random(3) = 0) then
      begin // 使用治愈术
        IncHealthSpell(nPower, 0);
        SendRefMsg(RM_LIGHTINGEX, 2, m_nCurrX, m_nCurrY, NativeInt(Self), '');
        Exit;
      end;
    end
    else
    begin
      if m_TargetCret.m_wStatusTimeArr[POISON_DECHEALTH] <= 0 then
      begin
        if (Random(m_TargetCret.m_btAntiPoison) = 0) then
        begin
          if (not m_TargetCret.UnPosion) then // 防毒
            m_TargetCret.MakePosion(POISON_DECHEALTH, Random(60) + 10, Round(nPower * 10 / 100) + 1);
          // Mon24-1和Mon26-2冰咆哮怪物施毒不显示施毒效果 chongchong 2014-05-20
          // SendRefMsg(RM_LIGHTINGEX, 6, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), ''); // 施毒术
          Exit;
        end;
      end;
    end;
    BaseObjectList := TList.Create;
    GetMapBaseObjects(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, g_Config.nSnowWindRange, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (Abs(m_nCurrX - TargeTBaseObject.m_nCurrX) <= 6) and (Abs(m_nCurrY - TargeTBaseObject.m_nCurrY) <= 6) then
      begin
        if IsProperTarget(TargeTBaseObject) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(TargeTBaseObject).m_boOffLine))
          then
        begin
          SetTargetCreat(TargeTBaseObject);
          if not CanCloseDefense then // 忽视目标防御
            nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil)
          else
            nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
          // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
          nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);
          nDamage := GetPowerRateAdd(TargeTBaseObject, nDamage);
          nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
          nDamage := GetNextDamage(nDamage);
          // 怪物伤害封顶 chongchong 2016-09-07
          nDamage := TargeTBaseObject.GetAttackPowerMax(nDamage);
          // 吸收伤害
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(TargeTBaseObject);
            // 伤害吸收百分比 2020-09-17 20:11:44
            nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin
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
          if nDamage > 0 then
          begin
            btGetBackHP := LoByte(m_WAbil.MP);
            if btGetBackHP <> 0 then
              Inc(m_WAbil.HP, nDamage div btGetBackHP);
            nDamage := TargeTBaseObject.StruckDamage(nDamage, Self, 0);
            TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, TargeTBaseObject.m_WAbil.HP, TargeTBaseObject.m_WAbil.MaxHP,
              NativeInt(Self), '', 200);
            if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max
              (TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
            begin
              TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
            end;
            nDamage := TargeTBaseObject.DamageReboundPower(nDamage);
            if nDamage > 0 then
            begin // 反弹伤害
              nDamage := StruckDamage(nDamage, nil, 0);
              SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject), 'FT',
                200);
            end;
          end;
        end;
      end;
    end;
    BaseObjectList.Free;
    SendRefMsg(RM_LIGHTINGEX, 33, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), ''); // 冰咆哮
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TExplosionAttackMonster.Run;
begin
  inherited;
end;

// 直线魔法攻击怪物
function TLineMagicAttackMonster.MagicAttackTarget: Boolean;
var
  btDir: Byte;
  nX, nY: Integer;
  MinValue: Integer;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 3) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 3) then
  begin
    if GetAttackDir(m_TargetCret, 3, btDir) or GetAttackDir(m_TargetCret, 2, btDir) or GetAttackDir(m_TargetCret, btDir) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        Result := True;
        Attack(m_TargetCret, btDir);
        Exit;
      end;
    end
    else
    begin
      // btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      // MinValue := Min(abs(m_nCurrX - m_TargetCret.m_nCurrX), abs(m_nCurrY - m_TargetCret.m_nCurrY));
      // MinValue := Max(MinValue, 1);
      MinValue := 2;
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, btDir, MinValue, nX, nY) then
      begin
        SetTargetXY(nX, nY);
        Exit;
      end;
    end;
  end;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 3) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 3) then
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

procedure TLineMagicAttackMonster.Run;
begin
  inherited;
end;

// 魔龙石碑怪物
constructor TMLSBAttackMonster.Create;
begin
  inherited;
  m_nViewRange := 2;
  m_boMagicAttack := False;
end;

function TMLSBAttackMonster.AttackTarget: Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower, nDamage: Integer;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := inherited AttackTarget;
  BaseObjectList := TList.Create;
  try
    GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 2, BaseObjectList);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject <> m_TargetCret) and (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (not
        BaseObject.m_boHideMode or m_boCoolEye) and IsProperTarget(BaseObject) and
      // 怪物不攻击脱机人物 chongchong 2015-09-07
        (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
        then
      begin
        if not CanCloseDefense then // 忽视目标防御
          nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
        else
          nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
        nDamage := BaseObject.NewAbilPower(3, nDamage);
        nDamage := GetPowerRateAdd(BaseObject, nDamage);
        nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
        nDamage := GetNextDamage(nDamage);
        // 怪物伤害封顶 chongchong 2016-09-07
        nDamage := BaseObject.GetAttackPowerMax(nDamage);
        // 吸收伤害
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(BaseObject);
          // 伤害吸收百分比 2020-09-17 20:11:44
          nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin
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
        if nDamage > 0 then
        begin
          nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 200);
          nDamage := BaseObject.DamageReboundPower(nDamage);
          if nDamage > 0 then
          begin // 反弹伤害
            nDamage := StruckDamage(nDamage, nil, 0);
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
          end;
        end;
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

// 灭天火怪物
function TExtinguishDayFireAttackMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    wMagicID: Word;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    wMagicID := 45;
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    // 目标不防毒 chongchong 2015-03-11
    if (m_TargetCret.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0) and (not m_TargetCret.UnPosion) then
    begin
      if (Random(m_TargetCret.m_btAntiPoison) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_DAMAGEARMOR, 60, 10);
        wMagicID := 6; // 施毒术
      end;
    end;
    if wMagicID <> 6 then
    begin
      nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
      if (m_Master <> nil) then
        nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
      if not CanCloseDefense then // 忽视目标防御
        nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := m_TargetCret.NewAbilPower(3, nDamage);
      nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(m_TargetCret);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        btGetBackHP := LoByte(m_WAbil.MP);
        if btGetBackHP <> 0 then
          Inc(m_WAbil.HP, nDamage div btGetBackHP);
        nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
        m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
          NativeInt(Self), '', 200);
        m_TargetCret.DamageSpell(nDamage); // 减蓝
        if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
          + m_dwParalysisRate, 0)) = 0) then
        begin
          m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
        end;
        nDamage := m_TargetCret.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
        end;
      end;
    end;
    SendRefMsg(RM_LIGHTINGEX, wMagicID, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TExtinguishDayFireAttackMonster.Run;
begin
  inherited;
end;

// 火焰冰怪物
function TFireIceAttackMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPush: Integer;
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    if m_TargetCret.m_boGhost or m_TargetCret.m_boDeath then
      Exit;
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if (m_WAbil.HP < Round(m_WAbil.MaxHP / 2)) and (Random(3) = 0) then
    begin // 使用治愈术
      // SendDelayMsg(Self, RM_MAGHEALING, 0, nPower, 0, 2, '', 800);
      IncHealthSpell(nPower, 0);
      SendRefMsg(RM_LIGHTINGEX, 2, m_nCurrX, m_nCurrY, NativeInt(Self), '');
      Exit;
    end;
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
        + m_dwParalysisRate, 0)) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
      if Random(3) = 0 then
      begin // 推动目标
        nPush := Max(Random(3), 1);
        SendDelayMsg(Self, RM_DELAYPUSHED, m_btDirection, MakeLong(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY), nPush, NativeInt
          (m_TargetCret), '', 600);
        // for I := 0 to nStep - 1 do
        // if m_TargetCret.CharPushed(m_btDirection, 1) <> 1 then Break;
      end;
    end;
    SendRefMsg(RM_LIGHTINGEX, 44, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), ''); // 寒冰掌
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TFireIceAttackMonster.Run;
begin
  inherited;
end;

{ ------------------------------------------------------------------------------ }
// 雪域卫士
constructor TIcePeakMonster.Create;
begin
  inherited;
  m_dwSearchTime := Random(1500) + 1500;
  m_nViewRange := 7;
  m_boStoneMode := True;
  m_nCharStatusEx := STATE_STONE_MODE;
  m_dwStartRunTick := MyGetTickCount;
end;

procedure TIcePeakMonster.MeltStone;
var
  IcePeakEvent: TIcePeakEvent;
begin
  if m_boStoneMode then
  begin
    m_dwStartRunTick := MyGetTickCount + 2000;
    m_nCharStatusEx := 0;
    m_nCharStatus := GetCharStatus();
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
    m_boStoneMode := False;
    IcePeakEvent := TIcePeakEvent.Create(Self);
    g_EventManager.AddEvent(IcePeakEvent);
  end;
end;

procedure TIcePeakMonster.MeltStoneAll;
var
  I: Integer;
  List10: TList;
  BaseObject: TBaseObject;
begin
  MeltStone();
  List10 := TList.Create;
  GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 7, List10);
  for I := 0 to List10.Count - 1 do
  begin
    BaseObject := TBaseObject(List10.Items[I]);
    if BaseObject <> nil then
    begin
      if BaseObject.m_boStoneMode then
      begin
        if BaseObject is TIcePeakMonster then
        begin
          TScultureMonster(BaseObject).MeltStone;
        end;
      end;
    end;
  end; // for
  List10.Free;
end;

procedure TIcePeakMonster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if (not m_boGhost) and (not m_boDeath) and CanMove and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay)
    then
  begin
    m_nWalkDelay := 0;
    if m_boStoneMode then
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
                  MeltStoneAll();
                  Break;
                end;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end
    else
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();
      end;
    end;
  end;
  if (MyGetTickCount > m_dwStartRunTick) or m_boStoneMode or m_boDeath then
    inherited;
end;

{ ------------------------------------------------------------------------------ }
// 押镖车
constructor TTruckMonster.Create;
begin
  inherited;
  m_nViewRange := 9; // 6
  m_nRunTime := 250;
  m_btRaceServer := 122;
  m_boSendRefMsg := False;
  m_dwSendRefMsgTick := MyGetTickCount();
  m_boEnterAnotherMap := False;
  m_nGateX := -1;
  m_nGateY := -1;
end;

procedure TTruckMonster.Run;
var
  nX, nY: Integer;
begin
  DelTargetCreat;
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    // 镖车休息 piaoyun 2013-08-20
    if (m_Master <> nil) and (m_Master.m_boSlaveRelax) and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave) then
    begin
      inherited;
      Exit; // 休息状态 退出
    end;
    if Think then
    begin
      inherited;
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
      if not m_boRunAwayMode then
      begin
        if (m_Master <> nil) and (m_Master.m_PEnvir = m_PEnvir) and (Abs(m_nCurrX - m_Master.m_nCurrX) <= m_Master.m_nViewRange)
          and (Abs(m_nCurrY - m_Master.m_nCurrY) <= m_Master.m_nViewRange) then
        begin
          m_boEnterAnotherMap := False;
          m_boSendRefMsg := False;
          m_Master.GetBackPosition(nX, nY);
          if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY) > 1) then
          begin
            m_nTargetX := nX;
            m_nTargetY := nY;
            if (Abs(m_nCurrX - nX) <= 2) and (Abs(m_nCurrY - nY) <= 2) then
            begin
              if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
              begin
                m_nTargetX := m_nCurrX;
                m_nTargetY := m_nCurrY;
              end
            end;
          end;
        end
        else if (m_Master <> nil) and (m_Master.m_PEnvir <> m_PEnvir) then
        begin
          if m_boEnterAnotherMap then
          begin
            m_boSendRefMsg := False;
            nX := m_nGateX;
            nY := m_nGateY;
            if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY) > 1) then
            begin
              m_nTargetX := nX;
              m_nTargetY := nY;
              if (Abs(m_nCurrX - nX) <= 2) and (Abs(m_nCurrY - nY) <= 2) then
              begin
                if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
                begin
                  m_nTargetX := m_nCurrX;
                  m_nTargetY := m_nCurrY;
                end
              end;
            end;
          end
          else
          begin
            m_nTargetX := -1;
            m_nTargetY := -1;
            if (m_Master <> nil) then
            begin
              if (not m_boSendRefMsg) then
              begin
                m_boSendRefMsg := True;
                // m_Master.SysMsg(Format(g_sTruckMonsterNotCanMoveMsg, [m_nCurrX, m_nCurrY]), c_Green, t_Hint);
                m_Master.SysMsg(Format(g_sTruckMonsterNotCanMoveMsg, [m_PEnvir.sMapName, m_PEnvir.sMapDesc, m_nCurrX, m_nCurrY]),
                  c_Green, t_Hint);
              end;
            end;
          end;
        end
        else
        begin
          m_nTargetX := -1;
          m_nTargetY := -1;
          m_boEnterAnotherMap := False;
          if (m_Master <> nil) and (m_Master.m_PEnvir = m_PEnvir) then
          begin
            if (not m_boSendRefMsg) then
            begin
              m_boSendRefMsg := True;
              // m_Master.SysMsg(Format(g_sTruckMonsterNotCanMoveMsg, [m_nCurrX, m_nCurrY]), c_Green, t_Hint);
              m_Master.SysMsg(Format(g_sTruckMonsterNotCanMoveMsg, [m_PEnvir.sMapName, m_PEnvir.sMapDesc, m_nCurrX, m_nCurrY]),
                c_Green, t_Hint);
            end;
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
      if m_nTargetX <> -1 then
        GotoTargetXY();
    end;
  end;
  inherited;
end;

// 狐狸
{ ------------------------------------------------------------------------------ }
constructor TFoxMonster.Create; // 004A8B74
begin
  inherited;
  m_boDupMode := False;
  bo554 := False;
  m_dwThinkTick := MyGetTickCount();
  m_nViewRange := 5;
  m_nRunTime := 250;
  m_dwSearchTime := 3000 + Random(2000);
  m_dwSearchTick := MyGetTickCount();
  m_btRaceServer := 80;
  m_boWonderingEx := False;
end;

function TFoxMonster.Think(): Boolean; // 004A8E54
var
  nOldX, nOldY: Integer;
begin
  Result := False;
  if (MyGetTickCount - m_dwThinkTick) > 3 * 1000 then
  begin
    m_dwThinkTick := MyGetTickCount();
    if ((m_Master = nil) or (not InSafeZone) or ((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))) then
    // 防止安全区宝宝被挤出安全区
    begin
      if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then
        m_boDupMode := True;
    end
    // 不让宝宝和NPC叠到一起 2020-09-07 00:01:53
    else if (m_Master <> nil) and (m_PEnvir.GetXYNpcObjCount(m_nCurrX, m_nCurrY) >= 1) then
    begin
      m_boDupMode := True;
    end;
    if not IsProperTarget { FFFF4 } (m_TargetCret) then
      m_TargetCret := nil;
  end;
  // 004A8ED2
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
end;

function TFoxMonster.MagicAttackTarget: Boolean; // 人物魔法攻击

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
        + m_dwParalysisRate, 0)) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    SendRefMsg(RM_LIGHTINGEX, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

function TFoxMonster.AttackTarget(): Boolean; // 004A8F34
var
  bt06: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if m_boMagicAttack then
    begin
      if GetAttackDir(m_TargetCret, bt06) then
      begin
        Result := MagicAttackTarget();
        m_boWonderingEx := True;
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
    end
    else
    begin
      if GetAttackDir(m_TargetCret, bt06) then
      begin
        if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
        begin
          m_dwHitTick := MyGetTickCount();
          m_nHitDelay := 0;
          m_dwTargetFocusTick := MyGetTickCount();
          Attack(m_TargetCret, bt06);
          BreakHolySeizeMode();
        end;
        m_boWonderingEx := True;
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
  end;
end;

function TFoxMonster.WonderingEx: Boolean;
var
  bt06: Byte;
  nX, nY, nCount: Integer;
  nTargetX: Integer;
  nTargetY: Integer;
begin
  Result := False;
  if (m_TargetCret <> nil) and m_boWonderingEx then
  begin
    if m_boMagicAttack then
    begin
      if GetAttackDir(m_TargetCret, bt06) then
      begin
        bt06 := Random(9);
        nCount := 0;
        while True do
        begin
          if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, bt06, 1, nX, nY) and m_PEnvir.CanWalkEx2(Self, nX, nY, False) and
            m_PEnvir.GetNextPosition(nX, nY, GetNextDirection(nX, nY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY), 1, nTargetX,
            nTargetY) and (m_TargetCret.m_nCurrX = nTargetX) and (m_TargetCret.m_nCurrY = nTargetY) then
          begin
            Result := True;
            SetTargetXY(nX, nY);
            GotoTargetXY;
            m_boWonderingEx := False;
            Break;
          end
          else
          begin
            bt06 := GetNextDirection(bt06);
          end;
          Inc(nCount);
          if nCount >= 7 then
            Break;
        end;
      end;
    end
    else
    begin
      if GetAttackDir(m_TargetCret, bt06) then
      begin
        bt06 := Random(9);
        nCount := 0;
        while True do
        begin
          if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, bt06, 1, nX, nY) and m_PEnvir.CanWalkEx2(Self, nX, nY, False) and
            m_PEnvir.GetNextPosition(nX, nY, GetNextDirection(nX, nY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY), 1, nTargetX,
            nTargetY) and (m_TargetCret.m_nCurrX = nTargetX) and (m_TargetCret.m_nCurrY = nTargetY) then
          begin
            Result := True;
            SetTargetXY(nX, nY);
            GotoTargetXY;
            m_boWonderingEx := False;
            Break;
          end
          else
          begin
            bt06 := GetNextDirection(bt06);
          end;
          Inc(nCount);
          if nCount >= 7 then
            Break;
        end;
      end;
    end;
  end;
end;

procedure TFoxMonster.Run;
var
  nX, nY: Integer;
begin
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    if Think then
    begin
      inherited;
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
      end; // 004A9151
      if not m_boRunAwayMode then
      begin
        if not m_boNoAttackMode then
        begin
          if m_TargetCret <> nil then
          begin
            if Random(3) = 0 then
            begin
              if WonderingEx then
              begin
                inherited;
                Exit;
              end;
            end;
            if AttackTarget { FFEB } then
            begin
              inherited;
              Exit;
            end;
          end
          else
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
            end; // 004A91D3
          end;
        end; // 004A91D3  if not bo2C0 then begin
        if m_Master <> nil then
        begin
          if m_TargetCret = nil then
          begin
            m_Master.GetBackPosition(nX, nY);
            if (Abs(m_nTargetX - nX) > 1) or (Abs(m_nTargetY - nY { nX } ) > 1) then
            begin // 004A922D
              m_nTargetX := nX;
              m_nTargetY := nY;
              if (Abs(m_nCurrX - nX) <= 2) and (Abs(m_nCurrY - nY) <= 2) then
              begin
                if m_PEnvir.GetMovingObject(nX, nY, True) <> nil then
                begin
                  m_nTargetX := m_nCurrX;
                  m_nTargetY := m_nCurrY;
                end // 004A92A5
              end;
            end; // 004A92A5
          end; // 004A92A5 if m_TargetCret = nil then begin
          if ((not m_Master.m_boSlaveRelax) or (m_boGamePet and (not g_Config.boPetSleepControlBySlave))) and ((m_PEnvir <>
            m_Master.m_PEnvir) or (Abs(m_nCurrX - m_Master.m_nCurrX) > 20) or (Abs(m_nCurrY - m_Master.m_nCurrY) > 20)) then
          begin
            SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
          end; // 004A937E
        end; // 004A937E if m_Master <> nil then begin
      end
      else
      begin // 004A9344
        if (m_dwRunAwayTime > 0) and ((MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime) then
        begin
          m_boRunAwayMode := False;
          m_dwRunAwayTime := 0;
        end;
      end; // 004A937E
      if (m_Master <> nil) and m_Master.m_boSlaveRelax and ((not m_boGamePet) or g_Config.boPetSleepControlBySlave) then
      begin
        inherited;
        Exit;
      end; // 004A93A6
      if m_nTargetX <> -1 then
      begin
        GotoTargetXY(); // 004A93B5 0FFEF
      end
      else
      begin
        if m_TargetCret = nil then
          Wondering(); // FFEE   //Jacky
      end; // 004A93D8
    end;
    // 004A93D8  if not bo510 and (tick_diff(m_dwWalkTick, MyGetTickCount) > n4FC) then begin
  end; // 004A93D8
  inherited;
end;

// 石化攻击
function TStoneFoxMonster.AttackTarget: Boolean;
begin
  Result := False;
  if (m_TargetCret <> nil) and inherited AttackTarget then
  begin
    if (Random(8) = 0) and (m_TargetCret.CanStone()) and (not m_TargetCret.UnParalysis) then
      m_TargetCret.MakePosion(POISON_STONE, Random(6) + 2, 0);
    Result := True;
  end;
end;

// 狐狸魔法攻击
function TFoxMagicAttackMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
        + m_dwParalysisRate, 0)) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;
// 群体魔法攻击 piaoyun 2013-12-14

  procedure MagicAttackGroup;
  var
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    nPower, nDamage, I: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    BaseObjectList := TList.Create;
    try
      // 目标范围2格内群攻 piaoyun 2013-12-14
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 2, BaseObjectList);
      nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
      if (m_Master <> nil) then
        nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
          Continue;
        // 怪物不攻击脱机人物 chongchong 2015-09-07
        if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
          then
        begin
          Continue;
        end;
        if not CanCloseDefense then // 忽视目标防御
          nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
        else
          nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
        nDamage := BaseObject.NewAbilPower(3, nDamage);
        nDamage := GetPowerRateAdd(BaseObject, nDamage);
        nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
        nDamage := GetNextDamage(nDamage);
        // 怪物伤害封顶 chongchong 2016-09-07
        nDamage := BaseObject.GetAttackPowerMax(nDamage);
        // 吸收伤害
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(BaseObject);
          // 伤害吸收百分比 2020-09-17 20:11:44
          nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin
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
        if nDamage > 0 then
        begin
          nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 200);
          nDamage := BaseObject.DamageReboundPower(nDamage);
          if nDamage > 0 then
          begin // 反弹伤害
            nDamage := StruckDamage(nDamage, nil, 0);
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
          end;
        end;
      end;
      SendRefMsg(RM_LIGHTINGEX, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
    finally
      BaseObjectList.Free;
    end;
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        if m_wAppr = 607 then
        begin
          if Random(5) = 0 then
          begin
            MagicAttackGroup;
            Result := True;
            Exit;
          end;
        end;
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TFoxMagicAttackMonster.Run;
begin
  inherited;
end;

// 狐狸魔法攻击  吸蓝
function TDamageSpellAttackMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    wMagicID: Word;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    wMagicID := 1;
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      if Random(3) = 0 then
      begin
        m_TargetCret.DamageSpell(nDamage); // 减蓝
        wMagicID := 2;
        MagMakeDefenceAreaDown(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, nDamage, 2);
      end;
      if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
        + m_dwParalysisRate, 0)) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    SendRefMsg(RM_LIGHTING, wMagicID, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TDamageSpellAttackMonster.Run;
begin
  inherited;
end;

// 狐狸魔法攻击  减防御
function TDamageArmorAttackMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    wMagicID: Word;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    wMagicID := 1;
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      if Random(3) = 0 then
      begin
        m_TargetCret.ZeroArmor(Random(3) + 1); // 0防御
        wMagicID := 2;
      end;
      if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
        + m_dwParalysisRate, 0)) = 0) then
      begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    SendRefMsg(RM_LIGHTING, wMagicID, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TDamageArmorAttackMonster.Run;
begin
  inherited;
end;

{ ------------------------------------------------------------------------------ }
// 流星火雨怪物 怪物不能移动
constructor TMeteoriteRainAttackMonster.Create();
begin
  m_ShowFireTick := 0;
  inherited;
end;

function TMeteoriteRainAttackMonster.AttackTarget(): Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    I: Integer;
    BaseObjectList: TList;
    TargeTBaseObject: TBaseObject;
    FireBurnEvent: TFireBurnEvent;
    // 火圈范围 piaoyun 2013-12-02
    FireRange: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    BaseObjectList := TList.Create;
    GetMapBaseObjects(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, g_Config.nSkill58AttackRange,
      BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (TargeTBaseObject <> nil) and (not TargeTBaseObject.m_boDeath) and (not TargeTBaseObject.m_boGhost) and (Abs(m_nCurrX -
        TargeTBaseObject.m_nCurrX) <= 6) and (Abs(m_nCurrY - TargeTBaseObject.m_nCurrY) <= 6) then
      begin
        if IsProperTarget(TargeTBaseObject) then
        begin
          if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then
          begin
            SetTargetCreat(TargeTBaseObject);
            if not CanCloseDefense then // 忽视目标防御
              nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil)
            else
              nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
            // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
            nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);
            nDamage := GetPowerRateAdd(TargeTBaseObject, nDamage);
            nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
            nDamage := GetNextDamage(nDamage);
            // 怪物伤害封顶 chongchong 2016-09-07
            nDamage := TargeTBaseObject.GetAttackPowerMax(nDamage);
            // 吸收伤害
            if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            begin
              SmartObject := TSmartObject(TargeTBaseObject);
              // 伤害吸收百分比 2020-09-17 20:11:44
              nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
              if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
              begin
                nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
                SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                SmartObject.RefAbilNH;
              end;
              if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
              begin
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
            if nDamage > 0 then
            begin
              btGetBackHP := LoByte(m_WAbil.MP);
              if btGetBackHP <> 0 then
                Inc(m_WAbil.HP, nDamage div btGetBackHP);
              nDamage := TargeTBaseObject.StruckDamage(nDamage, Self, 0);
              TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, TargeTBaseObject.m_WAbil.HP,
                TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', 200);
              if (not TargeTBaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random
                (Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
              begin
                TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
              end;
              nDamage := TargeTBaseObject.DamageReboundPower(nDamage);
              if nDamage > 0 then
              begin // 反弹伤害
                nDamage := StruckDamage(nDamage, nil, 0);
                SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject),
                  'FT', 200);
              end;
            end;
          end;
        end;
      end;
    end;
    BaseObjectList.Free;
    if (m_TargetCret <> nil) and (MyGetTickCount - m_ShowFireTick > 20 * 1000) then
    begin
      Randomize;
      FireRange := 3 + Random(5);
      m_ShowFireTick := MyGetTickCount;
      nPower := m_TargetCret.GetMagStruckDamage(Self, nPower, nil);
      nPower := m_TargetCret.NewAbilPower(3, nPower);
      if m_PEnvir.GetEvent(m_nCurrX, m_nCurrY - FireRange) = nil then
      begin
        FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX, m_nCurrY - FireRange, ET_FIREMON33_7, 10 * 1000, nPower, True);
        g_EventManager.AddEvent(FireBurnEvent);
      end;
      if m_PEnvir.GetEvent(m_nCurrX - FireRange, m_nCurrY) = nil then
      begin
        FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX - FireRange, m_nCurrY, ET_FIREMON33_7, 10 * 1000, nPower, True);
        g_EventManager.AddEvent(FireBurnEvent);
      end;
      if m_PEnvir.GetEvent(m_nCurrX + FireRange, m_nCurrY) = nil then
      begin
        FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX + FireRange, m_nCurrY, ET_FIREMON33_7, 10 * 1000, nPower, True);
        g_EventManager.AddEvent(FireBurnEvent);
      end;
      if m_PEnvir.GetEvent(m_nCurrX, m_nCurrY + FireRange) = nil then
      begin
        FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX, m_nCurrY + FireRange, ET_FIREMON33_7, 10 * 1000, nPower, True);
        g_EventManager.AddEvent(FireBurnEvent);
      end;
      SendRefMsg(RM_LIGHTINGEX, 58, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), ''); // 流星火雨
    end;
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      MagicAttack;
      Result := True;
      Exit;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
      begin
        DelTargetCreat();
      end;
    end
    else
    begin
      DelTargetCreat();
    end;
  end;
end;

procedure TMeteoriteRainAttackMonster.Run;
begin
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    if m_TargetCret <> nil then
      AttackTarget;
  end;
  inherited;
end;

{ ------------------------------------------------------------------------------ }
// 怪物不能移动  魔法远程攻击  --- 真狐月天珠
constructor TMagicAttackNotMoveMonster.Create();
begin
  inherited;
  m_LastStep := 0;
  m_ForeverFrozenTick := 0;
  m_boCalledSlave := False;
  m_SlaveObjectList := TList.Create;
end;

destructor TMagicAttackNotMoveMonster.Destroy;
begin
  m_SlaveObjectList.Free;
  inherited;
end;

// 召唤神石
procedure TMagicAttackNotMoveMonster.CallSlave;
var
  nX, nY: Integer;
  BaseObject: TBaseObject;
  nRange: Integer;
begin
  if (m_SlaveObjectList.Count > 0) or (m_boCalledSlave) then
    Exit;
  nRange := 3 + Random(4); // 四兽离灵珠距离  3--7格
  GetFrontPosition(nX, nY);
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX + nRange, nY, g_Config.sFoxBeas[0]); // 青龙
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX - nRange, nY, g_Config.sFoxBeas[1]); // 白虎
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX, nY + nRange, g_Config.sFoxBeas[2]); // 朱雀
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX, nY - nRange, g_Config.sFoxBeas[3]); // 玄武
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  m_boCalledSlave := True;
end;

function TMagicAttackNotMoveMonster.AttackTarget(): Boolean;

  procedure MagicAttack(nType: Byte);
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    if m_TargetCret = nil then
      Exit;
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      case nType of
        1:
          begin
            if (not m_TargetCret.UnParalysis) { and m_boParalysis } and (Random(12) = 0) then
            begin
              m_TargetCret.MakePosion(POISON_STONE, 3, 0); // 目标麻痹3秒
            end;
          end;
        2:
          begin
            if (Random(8) = 0) then
              m_TargetCret.MakeFrozen(3); // 目标冰冻3秒
          end;
      end;
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    // SendRefMsg(RM_LIGHTING, nType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '', 200 * nType);
  end;
/// ///////////////////////////////////////////////////////////////////////////
// 8方向网状闪电攻击

  procedure MagicAttack2();
  var
    Dir: Byte;
    I, nX, nY: Integer;
    Obj: TBaseObject;
    nPower, nDamage: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    if m_TargetCret = nil then
      Exit;
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    if nDamage > 0 then
    begin
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    end;
    if nDamage = 0 then
      Exit;
    for Dir := 0 to 7 do
    begin
      for I := 1 to 8 do
      begin
        if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, Dir, I, nX, nY) then
        begin
          Obj := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));
          if (Obj <> nil) and IsProperTarget(Obj) and
          // 怪物不攻击脱机人物 chongchong 2015-09-07
            (not (g_Config.boMonNoAttackOffLinePlayer and (Obj.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Obj).m_boOffLine))
            then
          begin
            nPower := GetPowerRateAdd(Obj, nDamage);
            // 吸收伤害
            if Obj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            begin
              SmartObject := TSmartObject(Obj);
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
            nPower := Obj.StruckDamage(nPower, Self, 0);
            Obj.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nPower, Obj.m_WAbil.HP, Obj.m_WAbil.MaxHP, NativeInt(Self), '', 200);
          end;
        end;
      end;
    end;
  end;
/// ///////////////////////////////////////////////////////////////////////////
// 永恒冻结

  procedure MagicAttack3();
  var
    I: Integer;
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    nCur, nCount, nMax: Integer;
  begin
    BaseObjectList := TList.Create;
    try
      // 获取8*8范围链表对象
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 8, BaseObjectList);
      // 排除不可攻击对象
      for I := BaseObjectList.Count - 1 downto 0 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject = nil) or (BaseObject.m_boDeath) or (BaseObject.m_boGhost) or (Self = BaseObject) or (not IsProperTarget(BaseObject))
          or (not BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) or
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine)
          then
        begin
          BaseObjectList.Delete(I);
          Continue;
        end;
      end;
      nCur := 0;
      nCount := BaseObjectList.Count;
      nMax := Min(nCount, 4);
      Randomize;
      for I := 0 to nCount - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[Random(nCount)]);
        if BaseObject = nil then
          Continue;
        // 加入冰冻效果，人物不能移动
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
        begin
          (*
            TSmartObject(BaseObject).m_dwChangeModeExTick[9] := MyGetTickCount + 15 * 1000;
            //TSmartObject(BaseObject).m_boCanUseItem := True;  // 允许使用物品
            //TSmartObject(BaseObject).m_dwChangeModeExTick[8] := MyGetTickCount + 15 * 1000;
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            TPlayObject(TSmartObject(BaseObject)).SendDefMessage(SM_SENDACTIONMSG, 0, MakeWord(0, 0), 0, 0, '')
            else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
            THeroObject(TSmartObject(BaseObject)).SendDefMessage(SM_SENDACTIONMSG, 0, MakeWord(0, 0), 0, 0, '');
            //SendRefMsg(RM_EFFECTSTEP, 9999, 0, 0, 0, '');
          *)
          // 如果没有防永恒冰冻
          if ((not TSmartObject(BaseObject).m_boUnForeverFrozen) or (Random(100) >= BaseObject.m_nUnForeverFrozenRate)) then
          begin
            TSmartObject(BaseObject).OpenForeverFrozen(5);
            Inc(nCur);
          end;
          if nCur > nMax then
            Break;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
  end;

var
  nEfftctType: Integer;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_nHitDelay := 0;
    if (m_WAbil.HP < Round(m_WAbil.MaxHP * 0.9)) and (m_WAbil.HP > Round((m_WAbil.MaxHP * 0.8))) then
    begin
      // 召唤神石
      CallSlave;
    end;
    // 如果血低于80%
    if m_WAbil.HP < Round((m_WAbil.MaxHP * 0.8)) then
    begin
      // 时间大于45秒
      if MyGetTickCount - m_ForeverFrozenTick > 45 * 1000 then
      begin
        m_ForeverFrozenTick := MyGetTickCount;
        MagicAttack3;
      end;
    end;
    nEfftctType := 0;
    m_dwHitTick := MyGetTickCount();
    if Random(2) = 0 then
    begin
      MagicAttack2;
      nEfftctType := nEfftctType + 1;
    end;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 7) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 7) then
    begin
      MagicAttack(1);
      nEfftctType := nEfftctType + 2;
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 3) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 3) then
      begin
        MagicAttack(2);
        nEfftctType := nEfftctType + 4;
      end;
      Result := True;
      SendRefMsg(RM_LIGHTING, nEfftctType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
      Exit;
    end;
    // 尝试删除攻击目标
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
      begin
        DelTargetCreat();
      end;
    end
    else
    begin
      DelTargetCreat();
    end;
  end;
end;

procedure TMagicAttackNotMoveMonster.Run;
var
  nCurStep: Integer;
  I: Integer;
  BaseObject: TBaseObject;
begin
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    if m_TargetCret <> nil then
      AttackTarget;
    // 发送阶段值到客户端--改变外观及特效
    nCurStep := Max(0, 4 - m_WAbil.HP div (m_WAbil.MaxHP div 5));
    if nCurStep < 0 then
      nCurStep := 0;
    if m_LastStep <> nCurStep then
    begin
      SendRefMsg(RM_EFFECTSTEP, nCurStep, 0, 0, 0, '');
      m_LastStep := nCurStep;
    end;
  end;
  // 清理死亡的宝宝
  if m_SlaveObjectList <> nil then
  begin
    for I := m_SlaveObjectList.Count - 1 downto 0 do
    begin
      if m_SlaveObjectList.Count <= 0 then
        Break;
      BaseObject := TBaseObject(m_SlaveObjectList.Items[I]);
      if BaseObject <> nil then
      begin
        if BaseObject.m_boDeath or BaseObject.m_boGhost then
          m_SlaveObjectList.Delete(I);
      end;
    end;
  end;
  inherited;
end;

{ ------------------------------------------------------------------------------ }
// 怪物不能移动  魔法远程攻击  --- 狐狸天珠
constructor TMagicAttackNotMoveMonster2.Create();
begin
  inherited;
  m_LastStep := 0;
  m_ForeverFrozenTick := 0;
  m_boCalledSlave := False;
  m_SlaveObjectList := TList.Create;
end;

destructor TMagicAttackNotMoveMonster2.Destroy;
begin
  m_SlaveObjectList.Free;
  inherited;
end;

procedure TMagicAttackNotMoveMonster2.Initialize;
begin
  inherited;
  m_nOldNextHitTime := m_nNextHitTime;
end;

// 召唤神石
procedure TMagicAttackNotMoveMonster2.CallSlave;
var
  nX, nY: Integer;
  BaseObject: TBaseObject;
  nRange: Integer;
begin
  if (m_SlaveObjectList.Count > 0) or (m_boCalledSlave) then
    Exit;
  nRange := 4 + Random(3); // 四兽离灵珠距离  3--7格
  GetFrontPosition(nX, nY);
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX + nRange, nY, g_Config.sFoxBeas2[0]); // 青龙
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  {
    BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX - nRange, nY, g_Config.sFoxBeas2[1]); // 白虎
    if BaseObject <> nil then
    begin
    m_SlaveObjectList.Add(BaseObject);
    end;
  }
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX, nY + nRange, g_Config.sFoxBeas2[1]); // 朱雀
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  BaseObject := UserEngine.RegenMonsterByName(m_sMapName, nX, nY - nRange, g_Config.sFoxBeas[2]); // 玄武
  if BaseObject <> nil then
  begin
    m_SlaveObjectList.Add(BaseObject);
  end;
  m_boCalledSlave := True;
end;

function TMagicAttackNotMoveMonster2.AttackTarget(): Boolean;

  function SingleAttack: Boolean;
  var
    nPower, nDamage: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    Result := False;
    if m_TargetCret = nil then
      Exit;
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    if nDamage > 0 then
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage = 0 then
      Exit;
    if (m_TargetCret <> nil) and (not m_TargetCret.m_boDeath) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boHideMode
      or m_boCoolEye) and IsProperTarget(m_TargetCret) and
    // 怪物不攻击脱机人物 chongchong 2015-09-07
      (not (g_Config.boMonNoAttackOffLinePlayer and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(m_TargetCret).m_boOffLine))
      then
    begin
      Result := True;
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(m_TargetCret, RM_STRUCK, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP, NativeInt(Self),
        '', 200);
      if (Random(3) = 0) and (m_TargetCret.UnParalysis) then
        m_TargetCret.MakePosion(POISON_STONE, Random(3) + 3, 0);
    end;
  end;

  function ThuderAttack: Boolean;
  var
    nPower, nDamage: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    Result := False;
    if m_TargetCret = nil then
      Exit;
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    if nDamage > 0 then
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage = 0 then
      Exit;
    if (m_TargetCret <> nil) and (not m_TargetCret.m_boDeath) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boHideMode
      or m_boCoolEye) and IsProperTarget(m_TargetCret) and
    // 怪物不攻击脱机人物 chongchong 2015-09-07
      (not (g_Config.boMonNoAttackOffLinePlayer and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(m_TargetCret).m_boOffLine))
      then
    begin
      Result := True;
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(m_TargetCret, RM_STRUCK, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP, NativeInt(Self),
        '', 200);
      if Random(3) = 0 then
        m_TargetCret.MakeFrozen(Random(3) + 3);
    end;
  end;

  function MoveTargetAttack: Boolean;
  var
    nPower, nDamage: Integer;
    nX, nY: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    Result := False;
    if m_TargetCret = nil then
      Exit;
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    if nDamage > 0 then
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage = 0 then
      Exit;
    if (m_TargetCret <> nil) and (not m_TargetCret.m_boDeath) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boHideMode
      or m_boCoolEye) and IsProperTarget(m_TargetCret) and
    // 怪物不攻击脱机人物 chongchong 2015-09-07
      (not (g_Config.boMonNoAttackOffLinePlayer and (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(m_TargetCret).m_boOffLine))
      then
    begin
      Result := True;
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(m_TargetCret, RM_STRUCK, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP, NativeInt(Self),
        '', 200);
      if (m_Abil.Level < m_TargetCret.m_Abil.Level) then
      begin
        GetFrontPosition(nX, nY);
        m_TargetCret.SpaceMove(m_TargetCret.m_PEnvir.sMapName, nX, nY, 0);
      end;
    end;
  end;

  function GroupAttack: Boolean;
  var
    I: Integer;
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    nPower, nDamage: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    Result := True;
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) and (m_btRaceServer <> RC_HEROOBJECT) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    if nDamage > 0 then
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    if nDamage = 0 then
      Exit;
    BaseObjectList := TList.Create;
    try
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 5, BaseObjectList);
      Randomize;
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (not BaseObject.m_boHideMode or
          m_boCoolEye) and IsProperTarget(BaseObject) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          nPower := GetPowerRateAdd(BaseObject, nDamage);
          nPower := GetNextDamage(nPower);
          // 怪物伤害封顶 chongchong 2016-09-07
          nPower := BaseObject.GetAttackPowerMax(nPower);
          // 吸收伤害
          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(BaseObject);
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
          nPower := BaseObject.StruckDamage(nPower, Self, 0);
          BaseObject.SendDelayMsg(BaseObject, RM_STRUCK, nPower, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP, NativeInt(Self),
            '', 200);
          case Random(5) of
            0:
              begin
                BaseObject.AbilityDown(0, 5, 30, True);
              end;
            1:
              begin
                BaseObject.AbilityDown(3, 5, 30, True);
              end;
          end;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
  end;

var
  nEfftctType: Integer;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_nHitDelay := 0;
    CallSlave;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 7) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 7) then
    begin
      case Random(4) of
        0:
          begin
            nEfftctType := 1;
            Result := SingleAttack;
          end;
        1:
          begin
            nEfftctType := 2;
            Result := ThuderAttack;
          end;
        {
          2:
          begin
          nEfftctType := 4;
          Result := MoveTargetAttack(m_TargetCret);
          end;
        }
      else
        begin
          nEfftctType := 3;
          Result := GroupAttack;
        end;
      end;
      if Result then
      begin
        m_dwHitTick := MyGetTickCount();
        SendRefMsg(RM_LIGHTING, nEfftctType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
        Exit;
      end;
    end;
    // 尝试删除攻击目标
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
      begin
        DelTargetCreat();
      end;
    end
    else
    begin
      DelTargetCreat();
    end;
  end;
end;

procedure TMagicAttackNotMoveMonster2.Run;
var
  nCurStep: Integer;
  I: Integer;
  BaseObject: TBaseObject;
begin
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    if m_TargetCret <> nil then
      AttackTarget;
    // 发送阶段值到客户端--改变外观及特效
    if m_WAbil.HP <= (m_WAbil.MaxHP div 2) then
    begin
      nCurStep := 1;
      if m_nNextHitTime <> Round(m_nOldNextHitTime * 0.8) then
        m_nNextHitTime := Round(m_nOldNextHitTime * 0.8);
    end
    else
    begin
      nCurStep := 0;
      if m_nNextHitTime <> m_nOldNextHitTime then
      begin
        m_nNextHitTime := m_nOldNextHitTime;
      end;
    end;
    if m_LastStep <> nCurStep then
    begin
      SendRefMsg(RM_EFFECTSTEP, nCurStep, 0, 0, 0, '');
      m_LastStep := nCurStep;
    end;
  end;
  // 清理死亡的宝宝
  for I := m_SlaveObjectList.Count - 1 downto 0 do
  begin
    if m_SlaveObjectList.Count <= 0 then
      Break;
    BaseObject := TBaseObject(m_SlaveObjectList.Items[I]);
    if BaseObject <> nil then
    begin
      if BaseObject.m_boDeath or BaseObject.m_boGhost then
        m_SlaveObjectList.Delete(I);
    end;
  end;
  inherited;
end;

{ ------------------------------------------------------------------------------ }
// 火灵
function TFireSpiritMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    wMagicID: Word;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    if Random(3) = 0 then
      wMagicID := 200
    else
      wMagicID := 199;
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    if m_PEnvir.CanFly(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY) then
    begin
      nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
      if (m_Master <> nil) and (m_btRaceServer <> RC_HEROOBJECT) then
        nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
      if not CanCloseDefense then // 忽视目标防御
        nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := m_TargetCret.NewAbilPower(3, nDamage);
      nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(m_TargetCret);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        if wMagicID = 200 then
          nDamage := nDamage + Max(Round(nDamage / 2), 1);
        btGetBackHP := LoByte(m_WAbil.MP);
        if btGetBackHP <> 0 then
          Inc(m_WAbil.HP, nDamage div btGetBackHP);
        nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
        m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
          NativeInt(Self), '', 200);
        if (not m_TargetCret.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(m_TargetCret.m_btAntiPoison
          + m_dwParalysisRate, 0)) = 0) then
        begin
          m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
        end;
        nDamage := m_TargetCret.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
        end;
      end;
      SendRefMsg(RM_LIGHTING, wMagicID, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
    end;
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TFireSpiritMonster.Run;
begin
  inherited;
end;

{ ------------------------------------------------------------------------------ }
// 狮子
function TLionMonster.AttackTarget(): Boolean;
var
  bt06: Byte;

  procedure MagicAttack;
  var
    I: Integer;
    nPower: Integer;
    nDamage: Integer;
    BaseObject: TBaseObject;
    nX, nY: Integer;
    nTargetX, nTargetY: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    if nDamage > 0 then
    begin
      if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, 1, nX, nY) then
      begin
        m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, 4, nTargetX, nTargetY);
        for I := 0 to 2 do
        begin
          BaseObject := TBaseObject(m_PEnvir.GetMovingObject(nX, nY, True));
          if BaseObject <> nil then
          begin
            if IsProperTarget(BaseObject) and
            // 怪物不攻击脱机人物 chongchong 2015-09-07
              (not (g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
              then
            begin
              nPower := GetPowerRateAdd(BaseObject, nDamage);
              nPower := GetNextDamage(nPower);
              // 怪物伤害封顶 chongchong 2016-09-07
              nPower := BaseObject.GetAttackPowerMax(nPower);
              // 吸收伤害
              if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
              begin
                SmartObject := TSmartObject(BaseObject);
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
              if Random(10) >= BaseObject.m_nAntiMagic then
              begin
                BaseObject.SendDelayMsg(Self, RM_MAGSTRUCK, 0, nPower, 0, 0, '', 600);
              end;
            end;
          end;
          if not ((Abs(nX - nTargetX) <= 0) and (Abs(nY - nTargetY) <= 0)) then
          begin
            { nDir := }
            GetNextDirection(nX, nY, nTargetX, nTargetY);
            if not m_PEnvir.GetNextPosition(nX, nY, m_btDirection, 1, nX, nY) then
              Break;
          end
          else
            Break;
        end;
      end;
    end;
    SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 3) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 3) then
  begin
    if Random(3) = 0 then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
  end;
  if GetAttackDir(m_TargetCret, bt06) then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      Attack(m_TargetCret, bt06);
      BreakHolySeizeMode();
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

procedure TLionMonster.GotoTargetXY;

  procedure Walk;
  var
    I: Integer;
    nDir: Integer;
    n10: Integer;
    n14: Integer;
    n20: Integer;
    nOldX: Integer;
    nOldY: Integer;
  begin
    n10 := m_nTargetX;
    n14 := m_nTargetY;
    // dwTick3F4 := MyGetTickCount();
    nDir := DR_DOWN;
    if n10 > m_nCurrX then
    begin
      nDir := DR_RIGHT;
      if n14 > m_nCurrY then
        nDir := DR_DOWNRIGHT;
      if n14 < m_nCurrY then
        nDir := DR_UPRIGHT;
    end
    else
    begin
      if n10 < m_nCurrX then
      begin
        nDir := DR_LEFT;
        if n14 > m_nCurrY then
          nDir := DR_DOWNLEFT;
        if n14 < m_nCurrY then
          nDir := DR_UPLEFT;
      end
      else
      begin
        if n14 > m_nCurrY then
          nDir := DR_DOWN
        else if n14 < m_nCurrY then
          nDir := DR_UP;
      end;
    end;
    nOldX := m_nCurrX;
    nOldY := m_nCurrY;
    WalkTo(nDir, False);
    n20 := Random(3);
    for I := DR_UP to DR_UPLEFT do
    begin
      if (nOldX = m_nCurrX) and (nOldY = m_nCurrY) then
      begin
        if n20 <> 0 then
          Inc(nDir)
        else if nDir > 0 then
          Dec(nDir)
        else
          nDir := DR_UPLEFT;
        if (nDir > DR_UPLEFT) then
          nDir := DR_UP;
        WalkTo(nDir, False);
      end;
    end;
  end;

  function Run: Boolean;
  begin
    Result := RunTo(GetNextDirection(m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY), False);
  end;

begin
  if ((m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)) then
  begin
    if (Abs(m_nCurrX - m_nTargetX) >= 3) or (Abs(m_nCurrY - m_nTargetY) >= 3) then
    begin
      if not Run then
        Walk;
    end
    else
    begin
      Walk;
    end;
  end;
end;

procedure TLionMonster.Run;
begin
  inherited;
end;

{ ----------------------------TFireCrossMonster---------------------------------- }
// 火墙怪物
function TFireCrossMonster.OneAttack: Boolean;
var
  bt06: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, bt06) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        Attack(m_TargetCret, bt06);
        if (m_TargetCret.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) and (Random(3) = 0) then
        begin // 中毒
          if (not m_TargetCret.UnPosion) then // 防毒
            if (Random(m_TargetCret.m_btAntiPoison) = 0) then
              m_TargetCret.MakePosion(POISON_DECHEALTH, Random(6) + 3, 20);
        end;
        BreakHolySeizeMode();
      end;
      Result := True;
    end
    else
    begin
      if m_TargetCret.m_PEnvir = m_PEnvir then
      begin
        SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY); { 0FFF0h }
        // 004A8FE3
      end
      else
      begin
        DelTargetCreat(); { 0FFF1h }
        // 004A9009
      end;
    end;
  end;
end;

function TFireCrossMonster.TwoAttack(): Boolean;

  function GetRangeTargetCount(nX, nY, nRange: Integer): Integer;
  var
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    I: Integer;
  begin
    Result := 0;
    BaseObjectList := TList.Create;
    if GetMapBaseObjects(m_PEnvir, nX, nY, nRange, BaseObjectList) then
    begin
      for I := BaseObjectList.Count - 1 downto 0 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) or
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          ((g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          BaseObjectList.Delete(I);
        end;
      end;
      Result := BaseObjectList.Count;
    end;
    BaseObjectList.Free;
  end;

var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  WAbil: pTAbility;
  nPower, nDamage: Integer;
  btGetBackHP: Integer;
  bt06: Byte;
  nHTime: Integer;
  FireBurnEvent: TFireBurnEvent;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, bt06) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        m_btDirection := bt06;
        WAbil := @m_WAbil;
        nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
        if (m_Master <> nil) then
          nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
        if { (GetRangeTargetCount(m_nCurrX, m_nCurrY, 5) > 2) and } (Random(3) = 0) then
        begin // 火墙
          nHTime := Random(6) + 3;
          nPower := Round(nPower * (g_Config.nFireCrossPowerRate / 100));
          if m_PEnvir.GetEvent(m_nCurrX, m_nCurrY - 1) = nil then
          begin
            FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX, m_nCurrY - 1, ET_FIRE, nHTime * 1000, nPower);
            g_EventManager.AddEvent(FireBurnEvent);
          end;
          if m_PEnvir.GetEvent(m_nCurrX - 1, m_nCurrY) = nil then
          begin
            FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX - 1, m_nCurrY, ET_FIRE, nHTime * 1000, nPower);
            g_EventManager.AddEvent(FireBurnEvent);
          end;
          if m_PEnvir.GetEvent(m_nCurrX, m_nCurrY) = nil then
          begin
            FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX, m_nCurrY, ET_FIRE, nHTime * 1000, nPower);
            g_EventManager.AddEvent(FireBurnEvent);
          end;
          if m_PEnvir.GetEvent(m_nCurrX + 1, m_nCurrY) = nil then
          begin
            FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX + 1, m_nCurrY, ET_FIRE, nHTime * 1000, nPower);
            g_EventManager.AddEvent(FireBurnEvent);
          end;
          if m_PEnvir.GetEvent(m_nCurrX, m_nCurrY + 1) = nil then
          begin
            FireBurnEvent := TFireBurnEvent.Create(Self, m_nCurrX, m_nCurrY + 1, ET_FIRE, nHTime * 1000, nPower);
            g_EventManager.AddEvent(FireBurnEvent);
          end;
          SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
          BreakHolySeizeMode();
          Exit;
        end;
        BaseObjectList := TList.Create;
        if GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3, BaseObjectList) then
        begin
          for I := BaseObjectList.Count - 1 downto 0 do
          begin
            BaseObject := TBaseObject(BaseObjectList.Items[I]);
            if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
              Continue;
            // 怪物不攻击脱机人物 chongchong 2015-09-07
            if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
              then
            begin
              Continue;
            end;
            if (BaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) and (Random(3) = 0) then
            begin
              if (not BaseObject.UnPosion) then // 防毒
                if (Random(BaseObject.m_btAntiPoison) = 0) then
                  BaseObject.MakePosion(POISON_DECHEALTH, Random(6) + 3, 20);
            end;
            if not CanCloseDefense then // 忽视目标防御
              nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
            else
              nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
            nDamage := BaseObject.NewAbilPower(3, nDamage);
            nDamage := GetPowerRateAdd(BaseObject, nDamage);
            nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
            nDamage := GetNextDamage(nDamage);
            // 怪物伤害封顶 chongchong 2016-09-07
            nDamage := BaseObject.GetAttackPowerMax(nDamage);
            // 吸收伤害
            if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            begin
              SmartObject := TSmartObject(BaseObject);
              // 伤害吸收百分比 2020-09-17 20:11:44
              nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
              if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
              begin
                nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
                SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                SmartObject.RefAbilNH;
              end;
              if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
              begin
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
            if nDamage > 0 then
            begin
              btGetBackHP := LoByte(m_WAbil.MP);
              if btGetBackHP <> 0 then
                Inc(m_WAbil.HP, nDamage div btGetBackHP);
              nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
              BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
                NativeInt(Self), '', 200);
              if (not BaseObject.UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate)) and (Random(Max(BaseObject.m_btAntiPoison
                + m_dwParalysisRate, 0)) = 0) then
              begin
                BaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0); // g_Config.nAttackPosionTime
              end;
              nDamage := BaseObject.DamageReboundPower(nDamage);
              if nDamage > 0 then
              begin // 反弹伤害
                nDamage := StruckDamage(nDamage, nil, 0);
                SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT',
                  200);
              end;
            end;
          end;
        end;
        BaseObjectList.Free;
        BreakHolySeizeMode();
        SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
      end;
      Result := True;
    end
    else
    begin
      if m_TargetCret.m_PEnvir = m_PEnvir then
      begin
        SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY); { 0FFF0h }
        // 004A8FE3
      end
      else
      begin
        DelTargetCreat(); { 0FFF1h }
        // 004A9009
      end;
    end;
  end;
end;

function TFireCrossMonster.AttackTarget(): Boolean;
begin
  if Random(4) = 0 then
    Result := TwoAttack
  else
    Result := OneAttack;
end;

{ TDevilBat }
constructor TDevilBat.Create;
begin
  inherited;
  m_boAnimal := False; // 不是动物,即不能挖
  m_boStickMode := True; // 不能冲撞,气功，抗拒
  m_btAntiPoison := 200; // 中毒躲避
  m_nViewRange := 11;
end;

destructor TDevilBat.Destroy;
begin
  inherited;
end;

function TDevilBat.AttackTarget: Boolean;
var
  bt06: Byte;
begin
  Result := False;
  if GetAttackDir(m_TargetCret, bt06) then
  begin
    if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
    begin
      m_dwHitTick := MyGetTickCount();
      m_nHitDelay := 0;
      m_dwTargetFocusTick := MyGetTickCount();
      Attack(m_TargetCret, bt06);
      m_WAbil.HP := 0; // 死亡
    end;
    Result := True;
  end
  else
  begin
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11) then
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

procedure TDevilBat.Run;
begin
  try
    if not m_boGhost and not m_boDeath and (m_wStatusTimeArr[POISON_STONE { 5 } ] = 0) and (m_wStatusTimeArr[STATE_CONTINUOUSMAGICLOCK]
      = 0) then
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget(); // 搜索可攻击目标
      end;
      if tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay then
      begin
        m_dwWalkTick := MyGetTickCount();
        m_nWalkDelay := 0;
        if not m_boNoAttackMode then
        begin
          if m_TargetCret <> nil then
          begin
            if AttackTarget then
            begin
              inherited;
              Exit;
            end;
          end
          else
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
            end;
          end;
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
    inherited;
  except
    MainOutMessage('{异常} TDevilBat.Run');
  end;
end;

{ TTortoiseMonster }
function TTortoiseMonster.MagicAttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    if not CanCloseDefense then // 忽视目标防御
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
    else
      nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
    nDamage := m_TargetCret.NewAbilPower(3, nDamage);
    nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
    nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
    nDamage := GetNextDamage(nDamage);
    // 怪物伤害封顶 chongchong 2016-09-07
    nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
    // 吸收伤害
    if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(m_TargetCret);
      // 伤害吸收百分比 2020-09-17 20:11:44
      nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
      begin
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
    if nDamage > 0 then
    begin
      btGetBackHP := LoByte(m_WAbil.MP);
      if btGetBackHP <> 0 then
        Inc(m_WAbil.HP, nDamage div btGetBackHP);
      nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
      m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
        NativeInt(Self), '', 200);
      {
        if (not m_TargetCret.m_boUnParalysis) and m_boParalysis and (Random(Max(m_TargetCret.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
        begin
        m_TargetCret.MakePosion(POISON_STONE, m_dwParalysisTime, 0);                                 // g_Config.nAttackPosionTime
        end;
      }
      nDamage := m_TargetCret.DamageReboundPower(nDamage);
      if nDamage > 0 then
      begin // 反弹伤害
        nDamage := StruckDamage(nDamage, nil, 0);
        SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
      end;
    end;
    SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 10) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 10) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        MagicAttack;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TTortoiseMonster.Run;
begin
  inherited;
end;

{ TNoMoveMonster }
function TMon38_0Monster.AttackTarget: Boolean;

  procedure MagicAttack;
  var
    nPower: Integer;
    nDamage: Integer;
    btGetBackHP: Byte;
    I: Integer;
    BaseObjectList: TList;
    TargeTBaseObject: TBaseObject;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    BaseObjectList := TList.Create;
    try
      GetMapBaseObjects(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nAttackRage, BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (Abs(m_nCurrX - TargeTBaseObject.m_nCurrX) <= m_nAttackRage) and (Abs(m_nCurrY - TargeTBaseObject.m_nCurrY) <=
          m_nAttackRage) then
        begin
          if IsProperTarget(TargeTBaseObject) and
          // 怪物不攻击脱机人物 chongchong 2015-09-07
            (not (g_Config.boMonNoAttackOffLinePlayer and (TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(TargeTBaseObject).m_boOffLine))
            then
          begin
            // if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then
            // begin
            SetTargetCreat(TargeTBaseObject);
            if not CanCloseDefense then // 忽视目标防御
              nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil)
            else
              nDamage := TargeTBaseObject.GetMagStruckDamage(Self, nPower, nil, 1);
            // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
            nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);
            nDamage := GetPowerRateAdd(TargeTBaseObject, nDamage);
            nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
            nDamage := GetNextDamage(nDamage);
            // 怪物伤害封顶 chongchong 2016-09-07
            nDamage := TargeTBaseObject.GetAttackPowerMax(nDamage);
            // 吸收伤害
            if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            begin
              SmartObject := TSmartObject(TargeTBaseObject);
              // 伤害吸收百分比 2020-09-17 20:11:44
              nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
              if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
              begin
                nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
                SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                SmartObject.RefAbilNH;
              end;
              if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
              begin
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
            if nDamage > 0 then
            begin
              btGetBackHP := LoByte(m_WAbil.MP);
              if btGetBackHP <> 0 then
                Inc(m_WAbil.HP, nDamage div btGetBackHP);
              nDamage := TargeTBaseObject.StruckDamage(nDamage, Self, 0);
              TargeTBaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, TargeTBaseObject.m_WAbil.HP,
                TargeTBaseObject.m_WAbil.MaxHP, NativeInt(Self), '', 200);
              {
                if (not TargeTBaseObject.m_boUnParalysis) and m_boParalysis and (Random(Max(TargeTBaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) then
                begin
                TargeTBaseObject.MakePosion(POISON_STONE, m_dwParalysisTime, 0);                     // g_Config.nAttackPosionTime
                end;
              }
              nDamage := TargeTBaseObject.DamageReboundPower(nDamage);
              if nDamage > 0 then
              begin // 反弹伤害
                nDamage := StruckDamage(nDamage, nil, 0);
                SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(TargeTBaseObject),
                  'FT', 200);
              end;
            end;
            // end;
          end;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
    SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  end;

begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    // 4 * 4范围搜索怪物
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= m_nAttackRage) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= m_nAttackRage) then
    begin
      MagicAttack;
      Result := True;
      Exit;
    end;
    if m_TargetCret.m_PEnvir <> m_PEnvir then
    begin
      DelTargetCreat();
      Exit;
    end;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > m_nAttackRage) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > m_nAttackRage) then
    begin
      DelTargetCreat();
    end;
  end;
end;

procedure TMon38_0Monster.NowDigUP;
{ var
  Event: TGameEvent; }
begin
  { Event := TGameEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, 1, 5 * 60 * 1000, True);
    g_EventManager.AddEvent(Event);
    m_boFixedHideMode := False;
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, NativeInt(Event), '');
  }
  { m_btDirection := 5;
    m_boFixedHideMode := False;
    SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, ''); }
  m_nCharStatusEx := 0;
  m_nCharStatus := GetCharStatus();
  SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
  m_boStoneMode := False;
end;

constructor TMon38_0Monster.Create;
begin
  inherited;
  m_nAttackRage := 4;
  // m_boFixedHideMode := True;
  m_boStoneMode := True;
  m_nCharStatusEx := STATE_STONE_MODE;
  m_nViewRange := 7;
end;

procedure TMon38_0Monster.Run;
var
  I: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  if not m_boGhost and not m_boDeath and
  // not m_boFixedHideMode and
  // not m_boStoneMode and
    CanMove then
  begin
    // if m_boFixedHideMode then
    if m_boStoneMode then
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
                if (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 3) and (Abs(m_nCurrY - BaseObject.m_nCurrY) <= 3) then
                begin
                  NowDigUP();
                  m_dwWalkTick := MyGetTickCount;
                  m_nWalkDelay := 1000;
                  Break;
                end;
              end;
            end;
          end;
        end;
      finally
        m_VisibleActors.UnLock;
      end;
    end
    else
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret =
        nil)) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();
      end;
      if m_TargetCret <> nil then
        AttackTarget;
    end;
  end;
  inherited;
end;

{ TMon32_11Monster }
function TMon38_11Monster.AttackTarget: Boolean;
var
  nDir: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, nDir) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        if (Random(3) = 0) and (m_wAppr <> 640) then
        begin
          // 狂暴攻击
          AttackDir(m_TargetCret, 0, nDir, 2, False);
          SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
        end
        else
        begin
          AttackDir(m_TargetCret, 0, nDir, 1, False);
          SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
        end;
        BreakHolySeizeMode();
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
end;

{ TMon32_13Monster }
function TMon38_13Monster.AttackTarget: Boolean;
var
  nDir: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, nDir) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        if (Random(5) = 0) then
        begin
          MagicAttackGroup(True, 8)
        end
        else if (Random(3) = 0) then
        begin
          MagicAttack
        end
        else
        begin
          Attack(m_TargetCret, nDir);
          BreakHolySeizeMode();
        end;
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
end;

procedure TMon38_13Monster.MagicAttack;
var
  nPower: Integer;
  nDamage: Integer;
  I, nX, nY: Integer;
  Obj: TBaseObject;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
  nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
  if (m_Master <> nil) then
    nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
  if not CanCloseDefense then // 忽视目标防御
    nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil)
  else
    nDamage := m_TargetCret.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
  nDamage := m_TargetCret.NewAbilPower(3, nDamage);
  nDamage := GetPowerRateAdd(m_TargetCret, nDamage);
  nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
  nDamage := GetNextDamage(nDamage);
  // 怪物伤害封顶 chongchong 2016-09-07
  nDamage := m_TargetCret.GetAttackPowerMax(nDamage);
  // 吸收伤害
  if m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
  begin
    SmartObject := TSmartObject(m_TargetCret);
    // 伤害吸收百分比 2020-09-17 20:11:44
    nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
    if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
    begin
      nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
      SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
      SmartObject.RefAbilNH;
    end;
    if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
    begin
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
  if nDamage > 0 then
  begin
    nDamage := m_TargetCret.StruckDamage(nDamage, Self, 0);
    m_TargetCret.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_TargetCret.m_WAbil.HP, m_TargetCret.m_WAbil.MaxHP,
      NativeInt(Self), '', 200);
    nDamage := m_TargetCret.DamageReboundPower(nDamage);
    if nDamage > 0 then
    begin // 反弹伤害
      nDamage := StruckDamage(nDamage, nil, 0);
      SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(m_TargetCret), 'FT', 200);
    end;
    // 直线范围攻击 - 4格范围 piaoyun 2014-01-03
    for I := 1 to 4 do
    begin
      m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, I, nX, nY);
      Obj := m_PEnvir.GetMovingObject(nX, nY, True);
      if (Obj <> nil) and IsProperTarget(Obj) and (Obj <> m_TargetCret) and
      // 怪物不攻击脱机人物 chongchong 2015-09-07
        (not (g_Config.boMonNoAttackOffLinePlayer and (Obj.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Obj).m_boOffLine)) then
        Attack(Obj, m_btDirection);
    end;
  end;
  SendRefMsg(RM_LIGHTING, 2, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
end;

procedure TMon38_13Monster.MagicAttackGroup(boSelfRage: Boolean; nRage: Integer);
var
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower, nDamage, I: Integer;
  SmartObject: TSmartObject;
  nSuckDamagePoint: Integer;
begin
  BaseObjectList := TList.Create;
  try
    if boSelfRage then
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nRage, BaseObjectList)
    else
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nRage, BaseObjectList);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
        Continue;
      // 怪物不攻击脱机人物 chongchong 2015-09-07
      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
      begin
        Continue;
      end;
      if not CanCloseDefense then // 忽视目标防御
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := BaseObject.NewAbilPower(3, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := nDamage * 2; // 双倍攻击
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := BaseObject.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
        BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
          NativeInt(Self), '', 200);
        nDamage := BaseObject.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
        end;
      end;
    end;
    SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')
  finally
    BaseObjectList.Free;
  end;
end;

{ TMon38_12Monster }
function TMon38_12Monster.AttackTarget: Boolean;
var
  nDir: Byte;
begin
  Result := False;
  if m_TargetCret <> nil then
  begin
    if GetAttackDir(m_TargetCret, nDir) then
    begin
      if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
      begin
        m_dwHitTick := MyGetTickCount();
        m_nHitDelay := 0;
        m_dwTargetFocusTick := MyGetTickCount();
        if m_wAppr = 342 then
        begin
          if (Random(3) = 0) then
          begin
            MagicAttackGroup(True, 3, 1, 3);
          end
          else
          begin
            // 普通物理攻击
            Attack(m_TargetCret, nDir);
            BreakHolySeizeMode();
          end;
        end
        // 物理群攻
        else if (Random(3) = 0) then
        begin
          AttackTarget0;
        end
        // 魔法群攻1
        else if (Random(5) = 0) then
        begin
          MagicAttackGroup(True, 3, 1, 1);
        end
        // 魔法群攻2
        else if (Random(3) = 0) then
        begin
          MagicAttackGroup(True, 6, 2, 2);
        end
        else
        begin
          // 普通物理攻击
          Attack(m_TargetCret, nDir);
          BreakHolySeizeMode();
        end;
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
end;

// 物理群攻
procedure TMon38_12Monster.AttackTarget0;
var
  BaseObjectList: TList;
  I: Integer;
  BaseObject: TBaseObject;
  nPower, nDamage: Integer;
  WAbil: pTAbility;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  WAbil := @m_WAbil;
  nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);
  if (m_Master <> nil) then
    nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
  BaseObjectList := TList.Create;
  try
    GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, 4, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
        Continue;
      // 怪物不攻击脱机人物 chongchong 2015-09-07
      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
      begin
        Continue;
      end;
      if not CanCloseDefense then // 忽视目标防御
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := BaseObject.NewAbilPower(3, nDamage);
      nDamage := GetPowerRateAdd(BaseObject, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := BaseObject.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
        BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
          NativeInt(Self), '', 200);
        nDamage := BaseObject.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
        end;
      end;
    end;
    SendRefMsg(RM_LIGHTING, 0, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
  finally
    BaseObjectList.Free;
  end;
end;

procedure TMon38_12Monster.MagicAttackGroup(boSelfRage: Boolean; nRage: Integer; Multiple: Integer; nType: Integer);
var
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower, nDamage, I: Integer;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  BaseObjectList := TList.Create;
  try
    if boSelfRage then
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nRage, BaseObjectList)
    else
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nRage, BaseObjectList);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
        Continue;
      if not CanCloseDefense then // 忽视目标防御
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := BaseObject.NewAbilPower(3, nDamage);
      nDamage := GetPowerRateAdd(BaseObject, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := nDamage * Multiple; // N倍攻击
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := BaseObject.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
        if nType = 2 then
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 2000)
        else
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 200);
        if (nType = 1) and (not BaseObject.UnParalysis)
        { and m_boParalysis and (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) } then
        begin // 麻痹
          BaseObject.MakePosion(POISON_STONE, 3, 0);
        end;
        nDamage := BaseObject.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          if nType = 2 then
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 2000)
          else
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
        end;
      end;
    end;
    SendRefMsg(RM_LIGHTING, nType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')
  finally
    BaseObjectList.Free;
  end;
end;

{ TMon35_2Monster }
function TMon35_2Monster.MagicAttackTarget: Boolean;
var
  Dir: Byte;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_dwHitTick := MyGetTickCount();
    m_nHitDelay := 0;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 6) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 6) then
    begin
      if (m_nTargetX = -1) or (Random(2) = 0) then
      begin
        // Mon35-2b 打击目标时面向目标 chongchong 2014-05-20
        Dir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        TurnTo(Dir);
        MagicAttackGroup(True, 3, 1, 3);
        ;
        Result := True;
        Exit;
      end;
    end;
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 6) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 6) then
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

procedure TMon35_2Monster.Run;
begin
  inherited;
end;

procedure TMon35_2Monster.MagicAttackGroup(boSelfRage: Boolean; nRage, Multiple, nType: Integer);
var
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower, nDamage, I: Integer;
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
begin
  BaseObjectList := TList.Create;
  try
    if boSelfRage then
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, nRage, BaseObjectList)
    else
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nRage, BaseObjectList);
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject)) then
        Continue;
      // 怪物不攻击脱机人物 chongchong 2015-09-07
      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_boOffLine) and g_Config.boMonNoAttackOffLinePlayer
        then
      begin
        Continue;
      end;
      if not CanCloseDefense then // 忽视目标防御
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil)
      else
        nDamage := BaseObject.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
      nDamage := BaseObject.NewAbilPower(3, nDamage);
      nDamage := GetPowerRateAdd(BaseObject, nDamage);
      nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
      nDamage := nDamage * Multiple; // N倍攻击
      nDamage := GetNextDamage(nDamage);
      // 怪物伤害封顶 chongchong 2016-09-07
      nDamage := BaseObject.GetAttackPowerMax(nDamage);
      // 吸收伤害
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        // 伤害吸收百分比 2020-09-17 20:11:44
        nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
        begin
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
      if nDamage > 0 then
      begin
        nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
        if nType = 2 then
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 2000)
        else
          BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
            NativeInt(Self), '', 200);
        if (nType = 1) and (not BaseObject.UnParalysis)
        { and m_boParalysis and (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) } then
        begin // 麻痹
          BaseObject.MakePosion(POISON_STONE, 3, 0);
        end;
        nDamage := BaseObject.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin // 反弹伤害
          nDamage := StruckDamage(nDamage, nil, 0);
          if nType = 2 then
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 2000)
          else
            SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, m_WAbil.HP, m_WAbil.MaxHP, NativeInt(BaseObject), 'FT', 200);
        end;
      end;
    end;
    SendRefMsg(RM_LIGHTING, nType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '')
  finally
    BaseObjectList.Free;
  end;
end;

{ TXueLingLeader }
constructor TXueLingLeader.Create;
begin
  inherited;
  m_nLight := 5;
  m_ForeverFrozenTick := 0;
  FillChar(m_boCalledCustodians, SizeOf(m_boCalledCustodians), 0);
end;

destructor TXueLingLeader.Destroy;
begin
  inherited;
end;

function TXueLingLeader.AttackTarget: Boolean;

  procedure MagicAttack1;
  var
    I, nPower, nDamage: Integer;
    BaseObjectList: TList;
    Obj: TBaseObject;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
    if (m_Master <> nil) then
      nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
    BaseObjectList := TList.Create;
    try
      // GetMapBaseObjects(m_PEnvir, m_nCurrX - 1, m_nCurrY - 2, 10, BaseObjectList);
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 10, BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        Obj := TBaseObject(BaseObjectList[I]);
        if (Obj <> nil) and IsProperTarget(Obj) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (Obj.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Obj).m_boOffLine))
          then
        begin
          if not CanCloseDefense then // 忽视目标防御
            nDamage := Obj.GetMagStruckDamage(Self, nPower, nil)
          else
            nDamage := Obj.GetMagStruckDamage(Self, nPower, nil, 1); // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
          nDamage := Obj.NewAbilPower(3, nDamage);
          if nDamage > 0 then
          begin
            nDamage := NewAbilPower(1, nDamage); // 元素增加攻击伤害
          end;
          nDamage := GetPowerRateAdd(Obj, nDamage);
          nDamage := GetNextDamage(nDamage);
          // 怪物伤害封顶 chongchong 2016-09-07
          nDamage := Obj.GetAttackPowerMax(nDamage);
          // 吸收伤害
          if Obj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(Obj);
            // 伤害吸收百分比 2020-09-17 20:11:44
            nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin
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
          if nDamage > 0 then
          begin
            nDamage := Obj.StruckDamage(nDamage, Self, 0);
            Obj.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, Obj.m_WAbil.HP, Obj.m_WAbil.MaxHP, NativeInt(Self), '',
              200);
          end;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
  end;
// 永恒冻结

  procedure MagicAttack2();
  var
    I: Integer;
    BaseObjectList: TList;
    BaseObject: TBaseObject;
    nCur, nCount, nMax, nPower, nDamage: Integer;
    nSuckDamagePoint: Integer;
    SmartObject: TSmartObject;
  begin
    BaseObjectList := TList.Create;
    try
      // 获取8*8范围链表对象
      // GetMapBaseObjects(m_PEnvir, m_nCurrX - 1, m_nCurrY - 2, 10, BaseObjectList);
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 10, BaseObjectList);
      // 排除不可攻击对象
      for I := BaseObjectList.Count - 1 downto 0 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (BaseObject.m_boDeath) or (BaseObject.m_boGhost) or (Self = BaseObject) or (not IsProperTarget(BaseObject)) or (not
          BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) or
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          ((g_Config.boMonNoAttackOffLinePlayer and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOffLine))
          then
        begin
          BaseObjectList.Delete(I);
          Continue;
        end;
      end;
      nCur := 0;
      nCount := BaseObjectList.Count;
      nMax := Min(nCount, 4);
      Randomize;
      nPower := GetAttackPower(m_WAbil.MC1, Max(m_WAbil.MC2 - m_WAbil.MC1, 1));
      if (m_Master <> nil) then
        nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
      for I := 0 to nCount - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[Random(nCount)]);
        // 加入冰冻效果，人物不能移动
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
        begin
          nDamage := GetPowerRateAdd(BaseObject, nPower);
          // 吸收伤害
          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(BaseObject);
            // 伤害吸收百分比 2020-09-17 20:11:44
            nDamage := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nDamage);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nDamage := Max(0, nDamage - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            if (SmartObject.m_nSuckDamagePoint > 0) and (nDamage > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin
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
          // 冰冻增加伤害(取魔法伤害值) chongchong 2014-09-12
          if nDamage > 0 then
          begin
            nDamage := BaseObject.StruckDamage(nDamage, Self, 0);
            BaseObject.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, BaseObject.m_WAbil.HP, BaseObject.m_WAbil.MaxHP,
              NativeInt(Self), '', 200);
          end;
          // 如果没有防永恒冰冻
          if ((not TSmartObject(BaseObject).m_boUnForeverFrozen) or (Random(100) >= BaseObject.m_nUnForeverFrozenRate)) then
          begin
            TSmartObject(BaseObject).OpenForeverFrozen(2);
            Inc(nCur);
          end;
          if nCur > nMax then
            Break;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
  end;

  procedure MagicAttack4;
  var
    I, nPower, nDamage: Integer;
    BaseObjectList: TList;
    Obj: TBaseObject;
  begin
    BaseObjectList := TList.Create;
    try
      // GetMapBaseObjects(m_PEnvir, m_nCurrX - 1, m_nCurrY - 2, 10, BaseObjectList);
      GetMapBaseObjects(m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 10, BaseObjectList);
      nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
      if (m_Master <> nil) then
        nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));
      nPower := nPower + nPower div 2;
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        Obj := TBaseObject(BaseObjectList[I]);
        if (Obj <> nil) and IsProperTarget(Obj) and
        // 怪物不攻击脱机人物 chongchong 2015-09-07
          (not (g_Config.boMonNoAttackOffLinePlayer and (Obj.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Obj).m_boOffLine))
          then
        begin
          nDamage := Obj.StruckDamage(nPower, Self, 0);
          Obj.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, Obj.m_WAbil.HP, Obj.m_WAbil.MaxHP, NativeInt(Self), '', 200);
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
  end;

  procedure MagicAttack5;
  var
    I: Integer;
    BaseObjectList: TList;
    Obj: TBaseObject;
    NewHP: Integer;
  begin
    BaseObjectList := TList.Create;
    try
      GetMapBaseObjects(m_PEnvir, m_nCurrX - 1, m_nCurrY - 2, 10, BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        Obj := TBaseObject(BaseObjectList[I]);
        if (Obj <> nil) and IsProperFriend(Obj) then
        begin
          NewHP := Min(Round(Obj.m_WAbil.HP / 100 * 110), Obj.m_WAbil.MaxHP);
          if Obj.m_WAbil.HP <> NewHP then
          begin
            Obj.m_WAbil.HP := NewHP;
            Obj.HealthSpellChanged;
          end;
        end;
      end;
    finally
      BaseObjectList.Free;
    end;
  end;

var
  nEffectType: Integer;
begin
  Result := False;
  if m_TargetCret = nil then
    Exit;
  if tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay then
  begin
    m_nHitDelay := 0;
    if (m_WAbil.HP <= Round(m_WAbil.MaxHP * 0.8)) then
    begin
      if not m_boCalledCustodians[0] then
      begin
        m_boCalledCustodians[0] := True;
        UserEngine.RegenMonsterByName(m_sMapName, m_nCurrX - 1, m_nCurrY - 2, g_Config.sXueLingCustodian[0]);
        SendRefMsg(RM_LIGHTING, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '');
        SendRefMsg(RM_LIGHTINGEX, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '', 500);
        Exit;
      end;
    end;
    if (m_WAbil.HP <= Round(m_WAbil.MaxHP * 0.6)) then
    begin
      if not m_boCalledCustodians[1] then
      begin
        m_boCalledCustodians[1] := True;
        UserEngine.RegenMonsterByName(m_sMapName, m_nCurrX - 1, m_nCurrY - 2, g_Config.sXueLingCustodian[1]);
        SendRefMsg(RM_LIGHTING, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '');
        SendRefMsg(RM_LIGHTINGEX, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '', 500);
        Exit;
      end;
    end;
    if (m_WAbil.HP <= Round(m_WAbil.MaxHP * 0.4)) then
    begin
      if not m_boCalledCustodians[2] then
      begin
        m_boCalledCustodians[2] := True;
        UserEngine.RegenMonsterByName(m_sMapName, m_nCurrX - 1, m_nCurrY - 2, g_Config.sXueLingCustodian[2]);
        SendRefMsg(RM_LIGHTING, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '');
        SendRefMsg(RM_LIGHTINGEX, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '', 500);
        Exit;
      end;
    end;
    if (m_WAbil.HP <= Round(m_WAbil.MaxHP * 0.2)) then
    begin
      if not m_boCalledCustodians[3] then
      begin
        m_boCalledCustodians[3] := True;
        UserEngine.RegenMonsterByName(m_sMapName, m_nCurrX - 1, m_nCurrY - 2, g_Config.sXueLingCustodian[3]);
        SendRefMsg(RM_LIGHTING, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '');
        SendRefMsg(RM_LIGHTINGEX, 3, m_nCurrX, m_nCurrY, NativeInt(Self), '', 500);
        Exit;
      end;
    end;
    if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 8) and (Abs(m_nCurrY - m_TargetCret.m_nCurrY) <= 8) then
    begin
      m_dwHitTick := MyGetTickCount();
      if Random(100) < 30 then
      begin
        nEffectType := 4;
        MagicAttack4;
      end
      else if Random(100) < 30 then
      begin
        nEffectType := 5;
        MagicAttack5;
      end
      else if Random(100) < 30 then
      begin
        nEffectType := 2;
        MagicAttack2;
      end
      else
      begin
        nEffectType := 1;
        MagicAttack1;
      end;
      // 攻击特效以目标为中心，不固定 chongchong 2014-09-20
      // SendRefMsg(RM_LIGHTING, nEffectType, m_nCurrX, m_nCurrY, NativeInt(Self), '');
      // SendRefMsg(RM_LIGHTINGEx, nEffectType, m_nCurrX, m_nCurrY, NativeInt(Self), '', 500);
      SendRefMsg(RM_LIGHTING, nEffectType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
      SendRefMsg(RM_LIGHTINGEX, nEffectType, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '', 500);
      Exit;
    end;
    // 尝试删除攻击目标
    if m_TargetCret.m_PEnvir = m_PEnvir then
    begin
      if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) > 8) or (Abs(m_nCurrY - m_TargetCret.m_nCurrY) > 8) then
      begin
        DelTargetCreat();
      end;
    end
    else
    begin
      DelTargetCreat();
    end;
  end;
end;

procedure TXueLingLeader.Run;
begin
  if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then
  begin
    if ((MyGetTickCount - m_dwSearchEnemyTick) > 8000) or (((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))
      then
    begin
      m_dwSearchEnemyTick := MyGetTickCount();
      SearchTarget();
    end;
    if m_TargetCret <> nil then
      AttackTarget;
  end;
  inherited;
end;

procedure TXueLingLeader.Wondering;
begin
end;

{ TWealthAnimalMon 富贵兽 20090517 }
constructor TWealthAnimalMon.Create;
begin
  inherited;
  m_boAnimal := False; // 不是动物,即不能挖
  m_boStickMode := True; // 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)
  m_btAntiPoison := 200; // 中毒躲避
  m_Abil.NewValue[13] := 100; // 防麻痹
  m_Abil.NewValue[16] := 100; // 防毒
  m_Abil.NewValue[18] := 100; // 防火墙
  m_Abil.NewValue[17] := 100; // 防诱惑
  // m_Abil.NewValue[15] := 100;         // 防复活
  m_Abil.NewValue[14] := 100; // 防护身
  m_Abil.NewValue[19] := 100; // 防冰冻
  m_Abil.NewValue[20] := 100; // 防蛛网
  m_nViewRange := 0;
  // m_nGameGird:= g_Config.nMonGameGird;  //灵符赏金值
end;

destructor TWealthAnimalMon.Destroy;
begin
  inherited;
end;

// 受普通攻击,不处理
function TWealthAnimalMon.StruckDamage(nDamage: Integer; StruckFrom: TBaseObject; MagicID: Integer; IsSetPKPower: Boolean):
  Integer;
begin
  Result := 0;
end;

// 攻击过程不处理 20090603
function TWealthAnimalMon.AttackTarget(): Boolean;
begin
  Result := False;
end;

// 受指定物品攻击，掉血，并累计灵符赏金值，并红字提示
procedure TWealthAnimalMon.StruckDamage1(nDamage: Integer; StruckFrom: TBaseObject);
begin
  if (nDamage > 0) and (not m_boDeath) then
  begin
    DamageHealth(nDamage, StruckFrom); // 掉血
    if m_boCrazyMode then
    begin // 狂化模式，累计灵符赏金值  20090603

      // Inc(m_nGameGird, abs(g_Config.nIncMonGameGird -(g_Config.nIncMonGameGird div 3))+ Random(g_Config.nIncMonGameGird div 3)+ 1);

      // if (Random(3) = 0) and g_Config.boShowMonSysHint then UserEngine.SendBroadCastMsgExt(Format_ToStr('悬赏捕杀富贵兽，目前赏金额度已经提高到%d张%s，请勇士们速速前往猎杀。', [ m_nGameGird, g_Config.sGameGird]), t_Say);
    end
    else
    begin
      if Random(100 { g_Config.nMon79CrazyRate } ) = 0 then
      begin
        OpenCrazyMode(100 { g_Config.nMon79CrazyTime } ); // 狂化模式 20090904
        UserEngine.SendBroadCastMsgExt('系统公告：被围困的富贵兽，已经狂躁不安。豪华宝物，近在咫尺。请勇士们速速集结，杀怪夺宝。', t_Say);
      end;
    end;
  end;
end;

procedure TWealthAnimalMon.Die;
begin
  try
    if (m_nGameGird > 0) then
    begin
      if (m_LastHiter <> nil) then
      begin
        if (m_LastHiter.m_btRaceServer = RC_PLAYOBJECT) then
        begin
          TPlayObject(m_LastHiter).IncGameGird(m_nGameGird);
          TPlayObject(m_LastHiter).GameGoldChanged;

          {
            UserEngine.SendBroadCastMsgExt(Format('恭喜%s在富贵兽狂暴的时候把富贵兽消灭了，获得了%d张%s', [m_LastHiter.m_sCharName, m_nGameGird, g_Config.sGameGird]), t_Say);
            if g_boGameLogGameGird then
            begin//记录灵符日志 20090528
            AddGameDataLog(Format(g_sGameLogMsg1, [LOG_GameGird, m_LastHiter.m_sMapName,
            m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, m_LastHiter.m_sCharName, g_Config.sGameGird,
            TPlayObject(m_LastHiter).m_nGameGird, '+('+inttostr(m_nGameGird)+')', m_sCharName]));
            end;
          }

          m_nGameGird := 0;
        end;
      end
      else if (m_ExpHitter <> nil) then
      begin
        if (m_ExpHitter.m_btRaceServer = RC_PLAYOBJECT) then
        begin
          TPlayObject(m_ExpHitter).IncGameGird(m_nGameGird);
          TPlayObject(m_ExpHitter).GameGoldChanged;

          {
            UserEngine.SendBroadCastMsgExt(Format_ToStr('恭喜%s在富贵兽狂暴的时候把富贵兽消灭了，获得了%d张%s', [m_ExpHitter.m_sCharName, m_nGameGird, g_Config.sGameGird]), t_Say);

            if g_boGameLogGameGird then
            begin//记录灵符日志 20090528
            AddGameDataLog(Format(g_sGameLogMsg1, [LOG_GameGird, m_ExpHitter.m_sMapName,
            m_ExpHitter.m_nCurrX, m_ExpHitter.m_nCurrY, m_ExpHitter.m_sCharName, g_Config.sGameGird,
            TPlayObject(m_ExpHitter).m_nGameGird, '+('+inttostr(m_nGameGird)+')', m_sCharName]));
            end;
          }

          m_nGameGird := 0;
        end;
      end;
    end;
  except
    MainOutMessage('TWealthAnimalMon.Die');
  end;
  inherited;
end;

procedure TWealthAnimalMon.Run;
begin
  if (not m_boDeath) and (not m_boGhost) and (m_wStatusTimeArr[POISON_STONE] = 0) and (m_wStatusTimeArr[POISON_LOCKSPELL { 7 } ] =
    0) then
  begin
    if tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;

      if (Random(20) = 0) then
      begin
        if (Random(4) = 1) then
          TurnTo(Random(8)); // 转向
      end
      else if (Random(6) = 0) then
      begin
        if (Random(6) = 1) then
          SendRefMsg(RM_DIGUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '') // 跳的动作
        else if (Random(3) = 1) then
          SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, ''); // 攻击动作
      end;
    end;
  end;
  inherited;
end;

end.

