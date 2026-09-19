unit ObjHero;

interface

uses
  Windows, Classes, SysUtils, StrUtils, Math, EDcode, ObjBase, Envir, Grobal2, Graphics, ObjPlayer, DateUtils, uCustomHeroMagic,
  uCustomMagicUtils, M2Definition, System.Types;

const
  HERO_MASTER_RANGE = 20;

type
  THeroObject = class(TSmartObject)
    m_sUserID: string[ACCOUNT_LEN]; // 登录帐号名
    m_btReLevel: Byte;
    m_nSessionID: Integer;
    m_nBagCount: Integer;
    m_boRcdSaved: Boolean;
    m_dwSaveRcdTick: LongWord;
    m_boNewHero: Boolean;
    m_btNewServer: Byte;
    m_boUseGroupSpell: Boolean;
    m_boLastGroupSpellOK: Boolean; // 上次合击是否成功 chongchong 2013-10-09

    m_btAttackMode: Byte; // 攻击模式

    m_btAngryValue: Integer;
    m_dwAddAngryValueTick: LongWord;
    m_dLogonTime: TDateTime; // 登录时间
    m_dwLogonTick: LongWord;
    m_OWAbil: TAbility;
    m_rLoyalPoint: Real; // 忠诚度
    m_boHeroLevel4Magic: Boolean; // 英雄技能四级触发

    m_boIsDeputy: Boolean; // 是否是副将英雄
    m_sMasterName: string;
    m_dwHeroSaveExp: LongWord; // 统计经验值，用来增加忠诚度 chongchong 2013-08-13

    m_dwThinkTick: Integer;
    m_dwThinkTick2: Integer;
    m_HeroAmuHintTick: LongWord; // 没有药品或者毒符的时候，提示时间
    m_HeroMPHintTick: LongWord; // MP低时提示
    m_HeroHPHintTick: LongWord; // HP低时提示

    m_dwMotaeboTick: LongWord; // 最后野蛮时间

    m_dwLastChangePoison: DWORD; // 最后一次红绿毒互换时间

    m_dwGetExp: LongWord;
    m_QuestFlag: TQuestFlag; // 0x128 129

    m_HeroMoveTick: LongWord;
    m_MoveToAttackTick: LongWord;
    m_AttackToMoveTick: LongWord;
    m_boCancelHeroForcePeaceMode: Boolean;
    m_dwCancelHeroForcePeaceModeTick: LongWord;
    m_dwCancelHeroForcePeaceModeTime: LongWord;
    m_boDecAddAngryValueTime: Boolean;
    m_nDecAddAngryValueTimeValue: Integer;
    m_dwDecAddAngryValueTimeTick: LongWord;
    m_dwDecAddAngryValueTimeTime: LongWord;
    m_dwQueryBagItemsTick: LongWord;
    m_LinkItem: pTUserItem;
    m_boAutoDropItemToMasterBag: Boolean;
    m_boAutoPickItemToMasterBag: Boolean;
  private
    m_boLogOut: Boolean;
    FWaitGroupAttack: Boolean;
    FWaitGroupAttackTick: DWORD;
    FErgumSkillUsed: Boolean;
    FRandomWalk: Boolean;
    FLastAttack: Boolean;
    FTaosUseBaseAttack: Boolean;
    FLastDir: Byte;
    FMovePointIndex: Integer;
    FMovePoints: array [0 .. 5] of TPoint;
    FIsStopTempMove: Boolean;
    FIsTempMoveSet: Boolean;
    FTempMoveDir0: Byte;
    FTempMoveDir: Byte;
    FTempMoveCount: Integer;
    FTempMoveDirTick: LongWord;
    function GetMoveTime: Integer;
    procedure SendDelItemList(Items: string; ItemsCount: Integer);
    // procedure SendChangeItems(nWhere: Integer; UserItem: pTUserItem);
    // function GetMyStatus: Integer;
    function GetUserItemWeitht(nWhere: Integer): Integer;
    function RepairWeapon(): Boolean;
    function SuperRepairWeapon(): Boolean;
    function WeaptonMakeLuck(): Boolean;
    // procedure RepairFirDragon(btType: Byte; nItemIdx: Integer; sItemName: string);
    procedure RefBagItemCount(SendMessage: Boolean = True);

    /// /////////////////////////////////////////////////////////////////////////
    procedure HeroAutoMove;
    function HeroAvoidTarget(Target: TBaseObject): Boolean; // 英雄躲避攻击目标
    function HeroAvoidTargetNext(): Boolean; // 英雄二次躲避攻击目标
    function HeroThink(): Boolean;
    function HeroAttackTarget: Boolean;
    function OpenSuperShiled: Boolean;
    function HeroBasicAttackTarget: Boolean;
    function HeroWarrAttackTarget: Boolean; // 战士
    function HeroWizardAttackTarget: Boolean; // 法师
    function HeroTaosAttackTarget: Boolean; // 道士

    function HeroTaosProtectSelfAndFirends: Boolean; // 道士跟随时保存自己及朋友
    function HeroWizardAutoDun(AMaster: TPlayObject): Boolean;
    function GetAttackIntervalTime(RecalHitSpeed: Boolean): LongWord; // 获取各职业攻击间隔
    // function GetMoveIntervalTime: LongWord;

    function GroupAttackProcess: Boolean;
    procedure ProcessAngryChangeDown;
    procedure ProcessAngryChangeUP; // 处理合击及怒气

    function GetMagicInfoEx(nMagic: Integer): pTUserMagic;
    function CheckLastHiterRange(LastHiter: TBaseObject; CheckMaster: Boolean): Boolean;
    function HeroGotoTargetXY(IsFollowMaster: Boolean = False): Boolean;
    function HeroRuntoTargetXY(IsFollowMaster: Boolean = False): Boolean;
    function ContinueousAttack: Boolean;
    function DoMotaebo100(BaseObject: TBaseObject; UserMagic: pTUserMagic): Boolean;
    function CheckInMasterRange(nX, nY: Integer): Boolean;
    function AllowHeroMagicRate(MagicType: TMagicType; MagicID: Word): Boolean;
    function AllowHeroMagicRate2(MagicType: TMagicType; MagicID: Word; CheckValue: Integer): Boolean;
    function CheckHeroMagicUseCondition(Condition: PHeroMagicUseCondition; Target: TBaseObject): Boolean;
    /// /////////////////////////////////////////////////////////////////////////
  public
    procedure GotoTargetXY(); override;
    procedure RunToTargetXY(); override;
    procedure ResetTempMoveStatus;
    procedure IncBeadExp(dwExp: LongWord; IsFromNPC: Boolean = False); // 聚灵珠
  public
    constructor Create(); override;
    destructor Destroy; override;
    function WearFirDragon: Boolean;
    procedure SetPKFlag(BaseObject: TBaseObject); override;
    procedure SetLastHiter(BaseObject: TBaseObject); override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Run; override;
    procedure Initialize; override;
    procedure KickException; override;
    function ReadBook(StdItem: pTStdItem): Boolean;
    function EatItems(StdItem: pTStdItem): Boolean;
    function EatUseItems(nShape: Integer): Boolean;
    function CheckItemBindUse(UserItem: pTUserItem): Boolean;
    function CheckTakeOnItems(nWhere: Integer; StdItem: pTStdItem): Boolean;
    function IsProperFriend(BaseObject: TBaseObject): Boolean; override; // FFF3

    function IsProperTarget(BaseObject: TBaseObject): Boolean; override;
    function IsAttackTarget(BaseObject: TBaseObject): Boolean; override;
    function _Attack(var wHitMode: Word; AttackTarget: TBaseObject; AttackRate: Single = 1.0; NpcReleaseagic: pTUserMagic = nil)
      : Boolean; override;
    function DoSpell(UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; BaseObject: TBaseObject; FormClient: Boolean = False)
      : Boolean; override;
    function IsLcokAttackTarget(BaseObject: TBaseObject): Boolean; // 是否可以锁定攻击对象 chongchong 2013-08-25

    function AddItemToBag(UserItem: pTUserItem): Boolean; override;
    procedure SysMsg(sMsg: AnsiString; MsgColor: TMsgColor; MsgType: TMsgType; boAddPrefix: Boolean = True); overload; override;
    procedure SysMsg(sMsg: AnsiString; FColor, BColor: Integer; MsgType: TMsgType; boAddPrefix: Boolean = True);
      overload; override;
    procedure SendUseitems();
    procedure SendUseMagic();
    procedure SendSocket(DefMsg: pTDefaultMessage; sMsg: AnsiString); virtual;
    procedure SendSocketEx(DefMsg: pTDefaultMessage; Buffer: PAnsiChar; BufferLen: Integer);
    procedure SendDefMessage(wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word; sMsg: string);
    procedure SendAddItem(UserItem: pTUserItem);
    procedure SendDelItem(UserItem: pTUserItem);
    function IsEnoughBag(): Boolean; override;
    procedure HasLevelUp(nLevel: Integer; IsTriggerFunc: Boolean = True; SendHealthSpellChanged: Boolean = True); override;
    procedure RecalcLevelAbilitys(IsSysDef: Boolean); override;
    procedure GainExp(dwExp: LongWord);
    procedure GetExp(dwExp: LongWord; FormNPC: Boolean = False; IsIncBead: Boolean = False; IsFromChangeExp: Boolean = False);
    procedure WinExp(dwExp: LongWord);
    procedure IncExp(dwExp: LongWord);
    procedure GainExpNG(dwExp: LongWord); // 内功
    procedure GetExpNG(dwExp: LongWord); // 内功
    procedure WinExpNG(dwExp: LongWord); // 内功
    procedure IncExpNG(dwExp: LongWord); // 内功

    procedure DoQueryBagItems();
    function LevelUpFuncNG: Boolean;
    procedure MakeSaveRcd(HeroData: PTHeroData);
    function GetShowName(boSuperUser: Boolean = False): string; override;
    procedure LogOn();
    procedure LogOut();
    procedure MakeGhost; override;
    procedure ScatterBagItems(ItemOfCreat: TBaseObject; KillMe: TBaseObject); override;
    procedure DropUseItems(BaseObject: TBaseObject); override;
    procedure DropJewelryBoxItems(BaseObject: TBaseObject); override;
    procedure DropGodBlessItems(BaseObject: TBaseObject); override;
    procedure SendAddMagic(UserMagic: pTUserMagic);
    procedure SendDelMagic(UserMagic: pTUserMagic);
    procedure SendUpdateItem(UserItem: pTUserItem);
    procedure SendUpdateItemName(nWhere: SmallInt; MakeIndex: Integer; NewItemName: string);
    procedure SendUpdateItemColor(nWhere: SmallInt; MakeIndex: Integer; Color: Byte);
    procedure SendUpdateItemDura(nWhere: SmallInt; MakeIndex: Integer; Dura: Word);
    procedure SendUpdateItemDuraMax(nWhere: SmallInt; MakeIndex: Integer; DuraMax: Word);
    procedure SendUpdateItemUpgradeCount(nWhere: SmallInt; MakeIndex: Integer; UpgradeCount: Byte);
    procedure SendUpdateItemNewLook(nWhere: SmallInt; MakeIndex: Integer; NewLook: Word);
    procedure SendUpdateItemNewShape(nWhere: SmallInt; MakeIndex: Integer; NewShape: Word);
    procedure SendUpdateItemHeroM2Light(nWhere: SmallInt; MakeIndex: Integer; HeroM2Light: Byte);
    procedure SendUpdateItemInsuranceCount(nWhere: SmallInt; MakeIndex: Integer; InsuranceCount: Word);
    procedure SendUpdateItemBind(nWhere: SmallInt; MakeIndex: Integer; IsBind: Boolean);
    procedure SendUpdateItemLimitTime(nWhere: SmallInt; MakeIndex: Integer; LimitTime: Integer);
    procedure SendUpdateItemNewValue(nWhere: SmallInt; MakeIndex: Integer; ValueIndex: Byte; Value: Word);
    procedure SendUpdateItemFlute(nWhere: SmallInt; UserItem: pTUserItem);
    procedure SendUpdateItemProgress(nWhere: SmallInt; UserItem: pTUserItem; ProgressIndex: Byte);
    procedure SendUpdateItemPropertyText(nWhere: SmallInt; MakeIndex: Integer; sText: string);
    procedure SendUpdateItemPropertyColor(nWhere: SmallInt; MakeIndex: Integer; Color: Byte);
    procedure SendUpdateItemPropertyValue(nWhere: SmallInt; UserItem: pTUserItem; PropertyIndex: Byte);
    function CanSend: Boolean;
    function FindGroupMagic: pTUserMagic;
    function GetGroupMagicId: Integer;
    procedure RecalcAbilitys; override;
    procedure Die; override;
    procedure ProcessRulesItems; // 处理规则物品
    procedure RestHero();
    procedure ClientTakeOnItemsEx(btWhere: Byte; nItemIdx: Integer; sItemName: string);
    procedure ClientTakeOffItemsEx(btWhere: Byte; nItemIdx: Integer; sItemName: string);
    procedure SendLoyalPoint;
    procedure SetTargetCreat(BaseObject: TBaseObject); override;
    procedure Struck(hiter: TBaseObject); override;
    procedure SearchTarget; override;
    procedure WeightChanged(); override;
    function FollowMaster: Boolean;
    function RunToNext(nX, nY: Integer): Boolean; override;
    procedure DelTargetCreat(); override;
    function HeroGotoNext(IsFollowMaster: Boolean): Boolean;
    function GotoNextOne(nX, nY: Integer; boRun: Boolean): Boolean; override;
    function WalkTo(btDir: Byte; boFlag: Boolean): Boolean; override;
    function RunTo(btDir: Byte; boFlag: Boolean): Boolean; override;
    function GetQuestFlagStatus(nFlag: Integer): Integer;
    procedure SetQuestFlagStatus(nFlag, nValue: Integer);
  public
    function AllowUseMagic(wMagIdx: Word; ShowLowMPHit: Boolean = False): Boolean; override;
  end;

implementation

uses
  M2Share, Guild, HUtil32, ObjNpc, IdSrvClient, ItmUnit, GameEvent, Castle, Magic, uCombatPowerUtils;

const
  // 治愈，群体治愈术最低间隔 chongchong 2018-08-02 23:31:06
  SKILL_HEALLING_CD_MIN = 3000;

  { ------------------------------------------------------------------------------ }

constructor THeroObject.Create();
begin
  inherited;
  m_nViewRange := 8;
  m_btRaceServer := RC_HEROOBJECT;
  m_boUseGroupSpell := False;
  m_boLastGroupSpellOK := True;
  m_nBagCount := 0;
  m_boLogOut := False;
  FillChar(m_OWAbil, SizeOf(TAbility), 0);
  m_rLoyalPoint := 0.00;
  m_boIsDeputy := False;
  m_sMasterName := '';
  m_dwSaveRcdTick := MyGetTickCount();
  m_boNewHero := False;
  m_btNewServer := 0;
  m_dwThinkTick := MyGetTickCount();
  m_dwThinkTick2 := MyGetTickCount();
  m_dwHeroSaveExp := 0;

  m_btAttackMode := 0;

  m_HeroAmuHintTick := 0;
  m_HeroMPHintTick := 0;
  m_HeroHPHintTick := 0;

  m_dwMotaeboTick := MyGetTickCount;

  m_dwGetExp := 0;

  // 2020-09-30 调整英雄运行间隔
  m_nRunTime := 100;

  m_btNameColor := g_Config.btHeroNameColor;

  { 开启隔位刺杀 chongchong 2013-09-14 }
  m_boUseThrusting := True;

  m_sRankLevelName := '%s';

  FWaitGroupAttack := False;
  FWaitGroupAttackTick := 0;

  FillChar(m_QuestFlag, SizeOf(TQuestFlag), #0);

  FErgumSkillUsed := False;
  FRandomWalk := False;
  FLastAttack := False;

  FTaosUseBaseAttack := False;

  FLastDir := 0;
  FMovePointIndex := 0;
  FillChar(FMovePoints, SizeOf(FMovePoints), 0);

  FIsTempMoveSet := False;
  FIsStopTempMove := False;
  FTempMoveDir0 := 0;
  FTempMoveDir := 0;
  FTempMoveCount := 0;
  FTempMoveDirTick := MyGetTickCount;

  m_HeroMoveTick := MyGetTickCount;

  m_MoveToAttackTick := 0;
  m_AttackToMoveTick := 0;

  m_boCancelHeroForcePeaceMode := False;
  m_dwCancelHeroForcePeaceModeTick := MyGetTickCount;
  m_dwCancelHeroForcePeaceModeTime := 0;

  m_boDecAddAngryValueTime := False;
  m_nDecAddAngryValueTimeValue := 0;
  m_dwDecAddAngryValueTimeTick := MyGetTickCount;
  m_dwDecAddAngryValueTimeTime := 0;

  m_boAutoDropItemToMasterBag := False;
  m_boAutoPickItemToMasterBag := False;

  m_dwQueryBagItemsTick := MyGetTickCount();

  if (g_PluginManager <> nil) then
  begin
    g_PluginManager.HookHeroObjectCreate(Self);
  end;
end;

destructor THeroObject.Destroy;
begin
  inherited;

  if (g_PluginManager <> nil) then
  begin
    g_PluginManager.HookHeroObjectFree(Self);
  end;
end;

procedure THeroObject.SearchTarget;
var
  BaseObject, BaseObject18: TBaseObject;
  I, nC, n10: Integer;
  VisibleBaseObject: pTVisibleBaseObject;
  IsAttackMaster: Boolean;
  IsAttackMasterBB: Boolean;

  function CheckHumanLock: Boolean;
  var
    J: Integer;
    SlaveObj: TBaseObject;
  begin
    Result := True;
    if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    begin
      Result := False;
      if BaseObject.InSafeZone then
        Exit;

      // 对方锁定了自己或主人
      if (BaseObject.m_TargetCret <> nil) and ((BaseObject.m_TargetCret = Self) or (BaseObject.m_TargetCret.Master = m_Master))
        and (BaseObject.m_boPKFlag) then
        Result := True;

      if BaseObject.m_SlaveList.Count > 0 then
      begin
        for J := 0 to BaseObject.m_SlaveList.Count - 1 do
        begin
          SlaveObj := BaseObject.m_SlaveList[J];
          if (SlaveObj <> nil) and (SlaveObj.m_TargetCret <> nil) and
            ((SlaveObj.m_TargetCret = Self) or (SlaveObj.m_TargetCret.Master = m_Master)) and (SlaveObj.m_boPKFlag) then
          begin
            Result := True;
            Break;
          end;
        end;
      end;

      // 假人英雄个傻B，主人都打了，还傻不拉叽
      if m_boDummyObject and (m_Master <> nil) and (m_Master.m_TargetCret = BaseObject) then
        Result := True;

      // 别人攻击我或主人
      if (BaseObject.m_LastHiter = Self) or ((m_Master <> nil) and (m_Master.m_LastHiter = BaseObject)) and (BaseObject.m_boPKFlag)
      then
        Result := True;
    end;
  end;

begin
  if (m_btAttackMode = 3) and (m_Master <> nil) and (m_Master.m_TargetCret <> nil) then
  begin
    if (not m_boTarget) or (m_TargetCret = nil) then
    begin
      if IsAttackTarget(m_Master.m_TargetCret) then
        SetTargetCreat(m_Master.m_TargetCret);
      m_dwTargetFocusTick := MyGetTickCount();
      Exit;
    end;
  end;

  BaseObject18 := nil;
  n10 := 999;
  m_VisibleActors.Lock;
  try
    for I := 0 to m_VisibleActors.Count - 1 do
    begin
      VisibleBaseObject := m_VisibleActors[I].Item;

      if VisibleBaseObject <> nil then
      begin
        BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
        if (BaseObject <> nil) and (BaseObject <> Self) and (BaseObject <> m_Master) then
        begin
          if not BaseObject.m_boDeath then
          begin
            if (BaseObject.m_btRaceServer <> RC_GUARD) and // 不主动攻击大刀
            // (BaseObject.m_btRaceServer <> RC_ARCHERGUARD) and //不主动攻击弓箭手
              (BaseObject.m_btRaceServer <> 55) and // 不主动攻击练功师
              (not(BaseObject.m_btRaceServer in [RC_NPC .. RC_ANIMAL])) then
            begin // 不主动攻击NPC
              if (IsProperTarget(BaseObject) and (not BaseObject.m_boHideMode or m_boCoolEye)) then
              begin
                if m_btAttatckMode in [HAM_ALL, HAM_DEAR, HAM_MASTER, HAM_GROUP, HAM_GUILD, HAM_PKATTACK, HAM_NATION] then
                begin
                  if not CheckHumanLock then
                    Continue;
                end;

                IsAttackMaster := (BaseObject.m_TargetCret = m_Master) or (BaseObject.m_LastHiter = m_Master);
                IsAttackMasterBB := ((BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.Master = Master)) or
                  ((BaseObject.m_LastHiter <> nil) and (BaseObject.m_LastHiter.Master = Master));

                // 不主动攻击反击型怪物 chongchong 2016-05-07
                if (BaseObject.m_btRaceServer >= RC_ANIMAL) and (BaseObject.m_Master = nil) and (BaseObject.m_boAnimal) then
                begin
                  if (BaseObject.m_LastHiter <> Self) and (BaseObject.m_TargetCret <> Self) and (not IsAttackMaster) and
                    (not IsAttackMasterBB) then
                  begin
                    Continue;
                  end;
                end;

                // 人物攻击自己的宝宝或英雄的宝宝时，英雄不要帮忙打 chongchong 2016-03-20
                if BaseObject.Master = Master then
                begin
                  Continue;
                end;

                // 非保护模式不主动攻击怪物 chongchong 2016-06-06
                if not m_boProtectStatus then
                begin
                  if (not(IsAttackMaster or IsAttackMasterBB)) then
                    Continue;
                end;

                nC := abs(m_nCurrX - BaseObject.m_nCurrX) + abs(m_nCurrY - BaseObject.m_nCurrY);
                // 因为攻击不了跟自己站一个坐标的目标，排除掉 ( ++++ (nC <> 0) and ) chongchong 2016-03-22
                if (nC <> 0) and (nC < n10) then
                begin
                  n10 := nC;

                  // 因为攻击不了跟自己站一个坐标的目标，排除掉 ( ++++ if nC = 1 then Break; ) chongchong 2016-03-22
                  if nC = 1 then
                  begin
                    BaseObject18 := BaseObject;
                    Break;
                  end;

                  BaseObject18 := BaseObject;
                end;
              end;
            end;
          end;
        end;
      end;
    end;
  finally
    m_VisibleActors.UnLock;
  end;

  if BaseObject18 <> nil then
  begin
    if (m_TargetCret = nil) or (m_TargetCret.m_boDeath) or (m_TargetCret.m_boGhost) then
    begin
      // 修改英雄，主人进了安全区，英雄回来 chongchong 2017-10-31
      if (m_Master <> nil) and ((not m_Master.InSafeZone) or (m_Master.InSafeZone and InSafeZone)) then
      begin
        if IsAttackTarget(BaseObject18) then
          SetTargetCreat(BaseObject18);
      end;

      m_dwTargetFocusTick := MyGetTickCount();
    end;
  end; // SetTargetCreat(BaseObject18);
end;

procedure THeroObject.SetTargetCreat(BaseObject: TBaseObject);
var
  MasterObj: TBaseObject;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  if BaseObject = nil then
    Exit;

  if (BaseObject.Master = Master) then
    Exit;

  if ((not m_boTarget) or (m_TargetCret = nil) or (m_TargetCret.m_boDeath) or (m_TargetCret.m_boGhost)) and (BaseObject <> Self)
    and (BaseObject <> m_Master) then
  begin
    // 修正英雄锁定目标后有时候会换目标 chongchong 2015-05-21
    if (m_boTarget) and (m_TargetCret <> nil) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boDeath) and
      (m_TargetCret.m_PEnvir = m_PEnvir) then
    begin
      Exit;
    end;

    // PK时，主人回到安全区，英雄不在攻击了chongchong 2017-12-11
    MasterObj := BaseObject.Master;
    if MasterObj = nil then
    begin
      MasterObj := BaseObject;
    end;
    if ((MasterObj <> nil) and (MasterObj.m_btRaceServer = RC_PLAYOBJECT)) and (m_Master <> nil) and (m_Master.InSafeZone) then
    begin
      Exit;
    end;

    // 禁止英雄攻击怪物 2019-07-18 20:37:03
    if g_Config.boDisableHeroAttackMonster then
    begin
      IsHuman := BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT { , RC_PLAYMOSTER } ];
      if not IsHuman then
      begin
        _Master := BaseObject.Master;
        if (_Master <> nil) then
        begin
          IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT { , RC_PLAYMOSTER } ];
        end;
      end;

      if not IsHuman then
        Exit;
    end;

    if not((BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode) then
      inherited;

    ResetTempMoveStatus;
  end;
end;

procedure THeroObject.Initialize;
begin
  // 英雄 修复套装属性加HP/MP时，待HP/MP满后收回重召英雄 HP/MP 又不满 chongchong 2014-11-29
  // 把 inherited 搞到最上面了(本来在最下面) chongchong 2014-11-29
  inherited;
  RecalcLevelAbilitys(False);
  RecalcAbilitys;
end;

procedure THeroObject.Struck(hiter: TBaseObject);

  function CanSetTarget: Boolean;
  begin
    Result := IsProperTarget(hiter);
    if (m_TargetCret = nil) then
    begin
      Exit;
    end;
    if (m_TargetCret <> nil) and Result then
    begin
      if (m_nCurrX - m_TargetCret.m_nCurrX = 0) or (m_nCurrY - m_TargetCret.m_nCurrY = 0) or
        ((abs(m_nCurrX - m_TargetCret.m_nCurrX) = 1) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) = 1)) then
      begin
        Result := False;
      end
      else
      begin
        Result := ((abs(m_nCurrX - m_TargetCret.m_nCurrX) + abs(m_nCurrY - m_TargetCret.m_nCurrY)) >
          (abs(m_nCurrX - hiter.m_nCurrX) + abs(m_nCurrY - hiter.m_nCurrY))) or
          ((not m_boProtectStatus) and (not m_boTarget) and
          ((abs(m_Master.m_nCurrX - m_TargetCret.m_nCurrX) + abs(m_Master.m_nCurrY - m_TargetCret.m_nCurrY)) >
          (abs(m_Master.m_nCurrX - hiter.m_nCurrX) + abs(m_Master.m_nCurrY - hiter.m_nCurrY))));
      end;
    end;
  end;

begin
  m_dwStruckTick := MyGetTickCount;
  if hiter <> nil then
  begin
    if ((m_Master = nil) or ((m_Master <> nil) and (not m_Master.m_boSlaveRelax) and
      ((not m_boGamePet) or g_Config.boPetSleepControlBySlave))) and CanSetTarget and (not m_boTarget) then
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

function THeroObject.AllowUseMagic(wMagIdx: Word; ShowLowMPHit: Boolean): Boolean;
var
  UserMagic: pTUserMagic;
  nSpellPoint: Integer;
  DecHP: Int64;
begin
  {
    if (wMagIdx in [SKILL_114]) and (wMagIdx < Length(m_UserMagics)) and m_PEnvir.AllowMagics(wMagIdx) and g_Config.boSkill114AttackUseNG then
    begin
    Result := False;
    UserMagic := m_UserMagics[wMagIdx];

    if UserMagic = nil then Exit;
    if MagicManager.IsWarrSkill(UserMagic.wMagIdx) then Exit;
    if (UserMagic.btKey = 0) then Exit;

    nSpellPoint := GetSpellPoint(UserMagic);

    if (not m_boTrainingNG) then
    begin
    Exit;
    end;
    // 检测内力值是否达到要求 piaoyun 2013-09-13
    if m_AbilNG.NH < nSpellPoint then Exit;

    Result := True;
    end
    else }
  if (wMagIdx in [SKILL_116, SKILL_117]) and (wMagIdx < Length(m_UserMagics)) and m_PEnvir.AllowMagics(wMagIdx) then
  begin
    Result := False;
    UserMagic := m_UserMagics[wMagIdx];

    if UserMagic = nil then
      Exit;
    if MagicManager.IsWarrSkill(UserMagic.wMagIdx) then
      Exit;
    if (UserMagic.btKey = 0) then
      Exit;

    nSpellPoint := GetSpellPoint(UserMagic);

    if g_Config.boSkill115UseNG then
    begin
      if (not m_boTrainingNG) then
      begin
        if g_Config.boSkill115NGNoEnoughDecHP then
        begin
          if g_Config.nSkill115NGNoEnoughDecHPType = 0 then
          begin
            DecHP := Round(m_WAbil.HP / 100 * g_Config.nSkill115NGNoEnoughDecHPValue);
            if m_WAbil.HP <= DecHP + 1 then
            begin
              Exit;
            end;
          end
          else
          begin
            DecHP := g_Config.nSkill115NGNoEnoughDecHPValue;
            if m_WAbil.HP <= DecHP + 1 then
            begin
              Exit;
            end;
          end;
        end
        else
        begin
          Exit;
        end;
      end
      else
      begin
        // 检测内力值是否达到要求 piaoyun 2013-09-13
        if (m_AbilNG.NH < nSpellPoint) then
        begin
          if g_Config.boSkill115NGNoEnoughDecHP then
          begin
            if g_Config.nSkill115NGNoEnoughDecHPType = 0 then
            begin
              DecHP := Round(m_WAbil.HP / 100 * g_Config.nSkill115NGNoEnoughDecHPValue);
              if m_WAbil.HP <= DecHP + 1 then
              begin
                Exit;
              end;
            end
            else
            begin
              DecHP := g_Config.nSkill115NGNoEnoughDecHPValue;
              if m_WAbil.HP <= DecHP + 1 then
              begin
                Exit;
              end;
            end;
          end
          else
          begin
            Exit;
          end;
        end;
      end;
    end
    else
    begin
      if (m_WAbil.MP <= 0) or (nSpellPoint > m_WAbil.MP) then
        Exit;
    end;

    Result := True;
  end
  else
  begin
    Result := False;
    if inherited AllowUseMagic(wMagIdx, ShowLowMPHit) then
    begin
      Result := True;
    end;
  end;
end;

{ TODO -ochongchong -c修改 : 英雄跟随主人算法改变 【2013-08-06】 }

function THeroObject.FollowMaster: Boolean;

  function GetDirXY(nTargetX, nTargetY: Integer): Byte;
  var
    n10: Integer;
    n14: Integer;
  begin
    n10 := nTargetX;
    n14 := nTargetY;
    Result := DR_DOWN; // 南
    if n10 > m_nCurrX then
    begin
      Result := DR_RIGHT; // 东
      if n14 > m_nCurrY then
        Result := DR_DOWNRIGHT; // 东南向
      if n14 < m_nCurrY then
        Result := DR_UPRIGHT; // 东北向
    end
    else
    begin
      if n10 < m_nCurrX then
      begin
        Result := DR_LEFT; // 西
        if n14 > m_nCurrY then
          Result := DR_DOWNLEFT; // 西南向
        if n14 < m_nCurrY then
          Result := DR_UPLEFT; // 西北向
      end
      else
      begin
        if n14 > m_nCurrY then
          Result := DR_DOWN // 南
        else if n14 < m_nCurrY then
          Result := DR_UP; // 正北
      end;
    end;
  end;

  function GotoMasterXY(var nTargetX, nTargetY: Integer): Boolean;
  var
    I, nDir: Integer;
    boRet: Boolean;
    btArrDirs: array [0 .. 8] of Byte;
    IsChange: Boolean;
  begin
    Result := False;
    boRet := False;
    if (m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_nCurrX) > 3) or (abs(m_Master.m_nCurrY - m_nCurrY) > 3)) and
      (not m_boProtectStatus) then
    begin
      nTargetX := m_nCurrX;
      nTargetY := m_nCurrY;
      nDir := GetDirXY(m_Master.m_nCurrX, m_Master.m_nCurrY);

      IsChange := ((FMovePoints[0].X = FMovePoints[2].X) and (FMovePoints[0].X = FMovePoints[4].X) and (FMovePoints[0].X <> 0) and
        (FMovePoints[0].Y = FMovePoints[2].Y) and (FMovePoints[0].Y = FMovePoints[4].Y) and (FMovePoints[0].Y <> 0) and
        (FMovePoints[1].X = FMovePoints[3].X) and (FMovePoints[1].X = FMovePoints[5].X) and (FMovePoints[1].X <> 0) and
        (FMovePoints[1].Y = FMovePoints[3].Y) and (FMovePoints[1].Y = FMovePoints[5].Y) and (FMovePoints[1].Y <> 0)) or
        ((FMovePoints[0].X = FMovePoints[3].X) and (FMovePoints[0].X <> 0) and (FMovePoints[1].X = FMovePoints[4].X) and
        (FMovePoints[1].X <> 0) and (FMovePoints[2].X = FMovePoints[5].X) and (FMovePoints[2].X <> 0) and
        (FMovePoints[0].Y = FMovePoints[3].Y) and (FMovePoints[0].Y <> 0) and (FMovePoints[1].Y = FMovePoints[4].Y) and
        (FMovePoints[1].Y <> 0) and (FMovePoints[2].Y = FMovePoints[5].Y) and (FMovePoints[2].Y <> 0));

      if IsChange then
      begin
        for I := 0 to Length(FMovePoints) - 1 do
        begin
          FMovePoints[I].X := 0;
          FMovePoints[I].Y := 0;
        end;

        Exit;
      end;

      case nDir of
        DR_UP:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_UPRIGHT;
            btArrDirs[2] := DR_UPLEFT;

            btArrDirs[3] := m_btLastDir;

            if Random(2) = 0 then
            begin
              btArrDirs[4] := DR_LEFT;
              btArrDirs[5] := DR_RIGHT;
            end
            else
            begin
              btArrDirs[4] := DR_RIGHT;
              btArrDirs[5] := DR_LEFT;
            end;

            // 添加新的方向 chongchong 2018-01-10
            btArrDirs[6] := DR_DOWNLEFT;
            btArrDirs[7] := DR_DOWNRIGHT;
            btArrDirs[8] := DR_DOWN;

            I := 0;
            while I <= 8 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if boRet then
              begin
                if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                begin
                  m_btLastDir := btArrDirs[I];
                  Result := True;
                  Break;
                end;
              end;

              Inc(I);
            end;

            {
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_UPRIGHT;
              btArrDirs[2] := DR_UPLEFT;
              btArrDirs[3] := DR_LEFT;
              btArrDirs[4] := DR_RIGHT;

              for I := 0 to 4 do
              begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
              Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
              Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then Break;
              end;
            }
          end;
        DR_UPRIGHT:
          begin
            if (m_Master.m_btDirection = DR_RIGHT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY - 1;
                m_btLastDir := DR_UPRIGHT;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_UP) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY - 1;
                m_btLastDir := DR_UPRIGHT;
                Result := True;
              end;
            end;

            if not boRet then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_UP;
              btArrDirs[2] := DR_RIGHT;
              btArrDirs[3] := m_btLastDir;
              btArrDirs[4] := DR_DOWNRIGHT;
              btArrDirs[5] := DR_DOWN;

              // 添加新的方向 chongchong 2018-01-10
              btArrDirs[6] := DR_UPLEFT;
              btArrDirs[7] := DR_LEFT;
              btArrDirs[8] := DR_DOWNLEFT;

              I := 0;
              while I <= 8 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if boRet then
                begin
                  if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                  begin
                    m_btLastDir := btArrDirs[I];
                    Result := True;
                    Break;
                  end;
                end;

                Inc(I);
              end;

              {
                btArrDirs[0] := nDir;
                btArrDirs[1] := DR_UP;
                btArrDirs[2] := DR_RIGHT;
                btArrDirs[3] := DR_DOWNRIGHT;
                btArrDirs[4] := DR_DOWN;

                for I := 0 to 4 do
                begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then Break;
                end;
              }
            end;
          end;
        DR_RIGHT:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_UPRIGHT;
            btArrDirs[2] := DR_DOWNRIGHT;
            btArrDirs[3] := m_btLastDir;

            if Random(2) = 0 then
            begin
              btArrDirs[4] := DR_DOWN;
              btArrDirs[5] := DR_UP;
            end
            else
            begin
              btArrDirs[4] := DR_UP;
              btArrDirs[5] := DR_DOWN;
            end;

            // 添加新的方向 chongchong 2018-01-10
            btArrDirs[6] := DR_DOWNLEFT;
            btArrDirs[7] := DR_UPLEFT;
            btArrDirs[8] := DR_LEFT;

            I := 0;
            while I <= 8 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;
              if I >= 3 then
                boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if boRet then
              begin
                if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                begin
                  m_btLastDir := btArrDirs[I];
                  Result := True;
                  Break;
                end;
              end;

              Inc(I);
            end;

            {
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_UPRIGHT;
              btArrDirs[2] := DR_DOWNRIGHT;
              btArrDirs[3] := DR_DOWN;

              for I := 0 to 3 do
              begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
              Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
              Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then Break;
              end;
            }
          end;
        DR_DOWNRIGHT:
          begin
            if (m_Master.m_btDirection = DR_RIGHT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY + 1;
                m_btLastDir := DR_DOWNRIGHT;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_DOWN) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY + 1;
                m_btLastDir := DR_DOWNRIGHT;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_RIGHT;
              btArrDirs[2] := DR_DOWN;
              btArrDirs[3] := m_btLastDir;
              btArrDirs[4] := DR_DOWNLEFT;
              btArrDirs[5] := DR_UPRIGHT;

              // 添加新的方向 chongchong 2018-01-10
              btArrDirs[6] := DR_UPLEFT;
              btArrDirs[7] := DR_UP;
              btArrDirs[8] := DR_LEFT;

              I := 0;
              while I <= 8 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;

                if I >= 3 then
                  boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if boRet then
                begin
                  if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                  begin
                    m_btLastDir := btArrDirs[I];
                    Result := True;
                    Break;
                  end;
                end;

                Inc(I);
              end;

              {
                btArrDirs[0] := nDir;
                btArrDirs[1] := DR_RIGHT;
                btArrDirs[2] := DR_DOWN;
                btArrDirs[3] := DR_DOWNLEFT;
                for I := 0 to 3 do
                begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then Break;
                end;
              }
            end;
          end;
        DR_DOWN:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_DOWNRIGHT;
            btArrDirs[2] := DR_DOWNLEFT;
            btArrDirs[3] := m_btLastDir;

            if Random(2) = 0 then
            begin
              btArrDirs[4] := DR_LEFT;
              btArrDirs[5] := DR_RIGHT;
            end
            else
            begin
              btArrDirs[4] := DR_RIGHT;
              btArrDirs[5] := DR_LEFT;
            end;

            // 添加新的方向 chongchong 2018-01-10
            btArrDirs[6] := DR_UPLEFT;
            btArrDirs[7] := DR_UPRIGHT;
            btArrDirs[8] := DR_UP;

            I := 0;
            while I <= 8 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if boRet then
              begin
                if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                begin
                  m_btLastDir := btArrDirs[I];
                  Result := True;
                  Break;
                end;
              end;

              Inc(I);
            end;

            {
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_DOWNRIGHT;
              btArrDirs[2] := DR_DOWNLEFT;
              btArrDirs[3] := DR_LEFT;
              btArrDirs[4] := DR_RIGHT;

              for I := 0 to 4 do
              begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
              Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
              Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then Break;
              end;
            }
          end;
        DR_DOWNLEFT:
          begin
            if (m_Master.m_btDirection = DR_LEFT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY + 1;
                m_btLastDir := DR_DOWNLEFT;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_DOWN) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY + 1;
                m_btLastDir := DR_DOWNLEFT;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_DOWN;
              btArrDirs[2] := DR_LEFT;
              btArrDirs[3] := m_btLastDir;
              btArrDirs[4] := DR_DOWNRIGHT;
              btArrDirs[5] := DR_UPLEFT;

              // 添加新的方向 chongchong 2018-01-10
              btArrDirs[6] := DR_UPRIGHT;
              btArrDirs[7] := DR_UP;

              I := 0;
              while I <= 7 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;

                if I >= 3 then
                  boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if boRet then
                begin
                  if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                  begin
                    m_btLastDir := btArrDirs[I];
                    Result := True;
                    Break;
                  end;
                end;

                Inc(I);
              end;

              {
                btArrDirs[0] := nDir;
                btArrDirs[1] := DR_DOWN;
                btArrDirs[2] := DR_LEFT;
                btArrDirs[3] := DR_DOWNRIGHT;
                btArrDirs[4] := DR_UPLEFT;

                for I := 0 to 4 do
                begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;

                if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then Break;
                end;
              }
            end;
          end;
        DR_LEFT:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_DOWNLEFT;
            btArrDirs[2] := DR_UPLEFT;
            btArrDirs[3] := m_btLastDir;

            if Random(2) = 0 then
            begin
              btArrDirs[4] := DR_DOWN;
              btArrDirs[5] := DR_UP;
            end
            else
            begin
              btArrDirs[4] := DR_UP;
              btArrDirs[5] := DR_DOWN;
            end;

            // 添加新的方向 chongchong 2018-01-10
            btArrDirs[6] := DR_UPRIGHT;
            btArrDirs[7] := DR_DOWNRIGHT;
            btArrDirs[8] := DR_RIGHT;

            I := 0;
            while I <= 8 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if boRet then
              begin
                if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                begin
                  m_btLastDir := btArrDirs[I];
                  Result := True;
                  Break;
                end;
              end;

              Inc(I);
            end;

            {
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_DOWNLEFT;
              btArrDirs[2] := DR_UPLEFT;
              btArrDirs[3] := DR_DOWN;
              btArrDirs[4] := DR_UP;

              for I := 0 to 4 do
              begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;
              if I >= 3 then
              Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
              Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then Break;
              end;
            }
          end;
        DR_UPLEFT:
          begin
            if (m_Master.m_btDirection = DR_LEFT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY - 1;
                m_btLastDir := DR_UPLEFT;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_UP) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY - 1;
                m_btLastDir := DR_UPLEFT;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_LEFT;
              btArrDirs[2] := DR_UP;
              btArrDirs[3] := m_btLastDir;
              btArrDirs[4] := DR_DOWNLEFT;
              btArrDirs[5] := DR_DOWN;

              // 添加新的方向 chongchong 2018-01-10
              btArrDirs[6] := DR_UPRIGHT;
              btArrDirs[7] := DR_DOWNRIGHT;
              btArrDirs[8] := DR_RIGHT;

              I := 0;
              while I <= 8 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;

                if I >= 3 then
                  boRet := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  boRet := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if boRet then
                begin
                  if GetDifferenceDirection(btArrDirs[I]) <> m_btLastDir then
                  begin
                    m_btLastDir := btArrDirs[I];
                    Result := True;
                    Break;
                  end;
                end;

                Inc(I);
              end;

              {
                btArrDirs[0] := nDir;
                btArrDirs[1] := DR_LEFT;
                btArrDirs[2] := DR_UP;
                btArrDirs[3] := DR_DOWNLEFT;
                btArrDirs[4] := DR_DOWN;

                for I := 0 to 4 do
                begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then Break;
                end;
              }
            end;
          end;
      end;
    end;
  end;

  function GotoMasterXY2(var nTargetX, nTargetY: Integer): Boolean;
  var
    I, nDir: Integer;
    btArrDirs: array [0 .. 7] of Byte;
  begin
    Result := False;
    if (m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_nCurrX) > 3) or (abs(m_Master.m_nCurrY - m_nCurrY) > 3)) and
      (not m_boProtectStatus) then
    begin
      nTargetX := m_nCurrX;
      nTargetY := m_nCurrY;
      nDir := GetDirXY(m_Master.m_nCurrX, m_Master.m_nCurrY);
      case nDir of
        DR_UP:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_UPRIGHT;
            btArrDirs[2] := DR_UPLEFT;
            btArrDirs[3] := DR_LEFT;
            btArrDirs[4] := DR_RIGHT;

            for I := 4 downto 0 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_UPRIGHT:
          begin
            if (m_Master.m_btDirection = DR_RIGHT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_UP) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_UP;
              btArrDirs[2] := DR_RIGHT;
              btArrDirs[3] := DR_DOWNRIGHT;
              btArrDirs[4] := DR_DOWN;

              for I := 4 downto 0 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
        DR_RIGHT:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_UPRIGHT;
            btArrDirs[2] := DR_DOWNRIGHT;
            btArrDirs[3] := DR_DOWN;

            for I := 3 downto 0 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_DOWNRIGHT:
          begin
            if (m_Master.m_btDirection = DR_RIGHT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_DOWN) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_RIGHT;
              btArrDirs[2] := DR_DOWN;
              btArrDirs[3] := DR_DOWNLEFT;
              for I := 3 downto 0 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
        DR_DOWN:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_DOWNRIGHT;
            btArrDirs[2] := DR_DOWNLEFT;
            btArrDirs[3] := DR_LEFT;
            btArrDirs[4] := DR_RIGHT;

            for I := 4 downto 0 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_DOWNLEFT:
          begin
            if (m_Master.m_btDirection = DR_LEFT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_DOWN) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_DOWN;
              btArrDirs[2] := DR_LEFT;
              btArrDirs[3] := DR_DOWNRIGHT;
              btArrDirs[4] := DR_UPLEFT;

              for I := 4 downto 0 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;

                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
        DR_LEFT:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_DOWNLEFT;
            btArrDirs[2] := DR_UPLEFT;
            btArrDirs[3] := DR_DOWN;
            btArrDirs[4] := DR_UP;

            for I := 4 downto 0 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;
              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_UPLEFT:
          begin
            if (m_Master.m_btDirection = DR_LEFT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_UP) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_LEFT;
              btArrDirs[2] := DR_UP;
              btArrDirs[3] := DR_DOWNLEFT;
              btArrDirs[4] := DR_DOWN;

              for I := 4 downto 0 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
      end;
    end;
  end;

const
  FOLLOW_MASTER_RANGE = 20;
var
  I: Integer;
  nWalkTime: LongWord;
  nX, nY: Integer;
  nTargetX, nTargetY: Integer;
  ErrCode: Integer;
  IsFllow: Boolean;
begin
  Result := False;

  ErrCode := 0;
  try
    if m_boLogOut then
      Exit;

    { TODO -ochongchong -c添加 : 英雄休息时不随主人移动 }
    if (m_btAttackMode = 2) then
    begin
      if (not g_Config.boHeroNoMoveOnSleep) and (m_PEnvir <> m_Master.m_PEnvir) then
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

        ErrCode := 501;
        DelTargetCreat;
        m_nTargetX := nX;
        m_nTargetY := nY;

        ErrCode := 502;
        SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);

        FMovePoints[FMovePointIndex] := Point(m_nCurrX, m_nCurrY);
        FLastDir := m_btDirection;
        Inc(FMovePointIndex);
        if FMovePointIndex > High(FMovePoints) then
          FMovePointIndex := 0;

        ResetTempMoveStatus;
        Result := True;

        ErrCode := 503;
        if m_boProtectStatus then
        begin
          ErrCode := 504;
          m_nProtectTargetX := nX;
          m_nProtectTargetY := nY;
        end;
      end;

      Exit;
    end;

    if m_boProtectStatus then
    begin
      ErrCode := 1;
      nTargetX := m_nProtectTargetX;
      nTargetY := m_nProtectTargetY;
    end
    else
    begin
      ErrCode := 2;
      nTargetX := m_Master.m_nCurrX;
      nTargetY := m_Master.m_nCurrY;

      case m_Master.m_btDirection of
        DR_UP:
          begin
            nTargetY := nTargetY + 1;
          end;
        DR_UPRIGHT:
          begin
            nTargetX := nTargetX - 1;
            nTargetY := nTargetY + 1;
          end;
        DR_RIGHT:
          begin
            nTargetX := nTargetX - 1;
          end;
        DR_DOWNRIGHT:
          begin
            nTargetX := nTargetX - 1;
            nTargetY := nTargetY - 1;
          end;
        DR_DOWN:
          begin
            nTargetY := nTargetY - 1;
          end;
        DR_DOWNLEFT:
          begin
            nTargetX := nTargetX + 1;
            nTargetY := nTargetY - 1;
          end;
        DR_LEFT:
          begin
            nTargetX := nTargetX + 1;
          end;
        DR_UPLEFT:
          begin
            nTargetX := nTargetX + 1;
            nTargetY := nTargetY + 1;
          end;
      end;
    end;

    ErrCode := 3;

    // 修改英雄不同屏回到主人身边 chongchong 2017-10-31
    if (((not m_boProtectStatus) and (m_PEnvir <> m_Master.m_PEnvir)) or ((abs(m_nCurrX - nTargetX) >= FOLLOW_MASTER_RANGE) or
      (abs(m_nCurrY - nTargetY) >= FOLLOW_MASTER_RANGE))) and (g_Config.boHeroFollowMasterWithDiffScreen or (m_TargetCret = nil))
    then
    begin
      IsFllow := False;
      if (m_PEnvir <> m_Master.m_PEnvir) then
        IsFllow := True
      else if ((abs(m_nCurrX - nTargetX) >= FOLLOW_MASTER_RANGE) or (abs(m_nCurrY - nTargetY) >= FOLLOW_MASTER_RANGE)) and
        ((not m_boTarget) or (m_TargetCret = nil)) then
        IsFllow := True;

      if IsFllow then
      begin
        ErrCode := 4;
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

        ErrCode := 5;
        DelTargetCreat;
        m_nTargetX := nX;
        m_nTargetY := nY;

        ErrCode := 6;
        SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);

        FMovePoints[FMovePointIndex] := Point(m_nCurrX, m_nCurrY);
        FLastDir := m_btDirection;
        Inc(FMovePointIndex);
        if FMovePointIndex > High(FMovePoints) then
          FMovePointIndex := 0;

        ResetTempMoveStatus;
        Result := True;

        ErrCode := 7;
        if m_boProtectStatus then
        begin
          ErrCode := 6;
          m_nProtectTargetX := nX;
          m_nProtectTargetY := nY;
        end;
        Exit;
      end;
    end;

    ErrCode := 9;
    if ((m_TargetCret = nil) and (m_btAttackMode <> 2)) or
      (g_Config.boHeroFollowMasterWithDiffScreen and ((abs(nX - m_Master.m_nCurrX) >= HERO_MASTER_RANGE) or
      (abs(nY - m_Master.m_nCurrY) >= HERO_MASTER_RANGE))) then
    begin
      ErrCode := 10;
      // 自动捡物 2020-03-28 01:34:58

      if not m_PEnvir.m_boNoAutoRangePickItem then // 禁止范围拾取
      begin
        if g_Config.boHeroPickUpItem or (m_boSelfAutoPickItem {$IF NEED_KEY = 1} and (g_nKey_UseClientPickItems <> 0)
{$IFEND}) then
        begin // 捡物
          if m_boSelfAutoPickItem and (m_btSelfAutoPickItemRange > 0) then
          begin
            if PickRangeItem(m_boSelfAutoPickAll, m_boAutoPickPlayDropItem, m_boAutoPickPlayScatterItem,
              m_dwAutoPickScatterToPickTime) then
            begin
              m_nMoveIndex := -1;
              SetLength(m_MovePath, 0);
              Exit;
            end;
          end;

          if m_boSelfAutoPickItem then
          begin
            if StartPickUpItem(m_boAutoPickPlayDropItem, m_boAutoPickPlayScatterItem, m_dwAutoPickScatterToPickTime) then
            begin
              m_nMoveIndex := -1;
              SetLength(m_MovePath, 0);
              Exit;
            end;
          end
          else if StartPickUpItem(True, True, 0) then
          begin
            m_nMoveIndex := -1;
            SetLength(m_MovePath, 0);
            Exit;
          end;
        end;
      end;

      nWalkTime := GetMoveTime;

      ErrCode := 12;
      if (m_Master <> nil) and (MyGetTickCount - m_dwMoveTimeTick > nWalkTime) then
      begin
        (*
          if ((FMovePoints[0].X = FMovePoints[2].X) and (FMovePoints[0].X = FMovePoints[4].X) and (FMovePoints[0].X <> 0) and
          (FMovePoints[0].Y = FMovePoints[2].Y) and (FMovePoints[0].Y = FMovePoints[4].Y) and (FMovePoints[0].Y <> 0) and
          (FMovePoints[1].X = FMovePoints[3].X) and (FMovePoints[1].X = FMovePoints[5].X) and (FMovePoints[1].X <> 0) and
          (FMovePoints[1].Y = FMovePoints[3].Y) and (FMovePoints[1].Y = FMovePoints[5].Y) and (FMovePoints[1].Y <> 0)) and

          ((Abs(m_nCurrX - m_Master.m_nCurrX) >= 5) or  (Abs(m_nCurrY - m_Master.m_nCurrY) >= 5)) then
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
          break;
          end;
          end;
          end;
          end;


          ErrCode := 511;
          DelTargetCreat;
          m_nTargetX := nX;
          m_nTargetY := nY;

          ErrCode := 512;
          SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);

          FMovePoints[FMovePointIndex] := Point(m_nCurrX, m_nCurrY);
          FLastDir := m_btDirection;
          Inc(FMovePointIndex);
          if FMovePointIndex > High(FMovePoints) then
          FMovePointIndex := 0;
          ResetTempMoveStatus;
          Result := True;
          end;
        *)

        ErrCode := 512;
        if GotoMasterXY(nTargetX, nTargetY) then
        begin
          (*
            // ++++++++++++++++++++ 回到主人身边时，有一个怪物在路上，英雄反复跑，又要往主人身边跑，又要躲怪 chongchong 2017-11-13
            if (m_LastHiter <> nil) and (abs(nTargetX - m_LastHiter.m_nCurrX) < 3) and (abs(nTargetY - m_LastHiter.m_nCurrY) < 3) and (m_btAttackMode <> 1{非跟随状态}) then
            begin
            if GotoMasterXY2(nTargetX, nTargetY) then
            begin
            if (m_LastHiter <> nil) and (abs(nTargetX - m_LastHiter.m_nCurrX) < 3) and (abs(nTargetY - m_LastHiter.m_nCurrY) < 3) and (not m_Master.InSafeZone) then
            begin
            end
            else
            begin
            SetTargetXY(nTargetX, nTargetY);

            if (Abs(m_nCurrX - nTargetX) > 1) or (Abs(m_nCurrY - nTargetY) > 1) then
            begin
            HeroRuntoTargetXY;
            Result := True;
            end
            else
            begin
            HeroGotoTargetXY;
            Result := True;
            end;

            ResetTempMoveStatus;
            end;
            end;
            end
            else
          *)
          begin
            // ++++++++++++++++++++ 回到主人身边时，有一个怪物在路上，英雄反复跑，又要往主人身边跑，又要躲怪 chongchong 2017-11-13

            SetTargetXY(nTargetX, nTargetY);

            if (abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1) then
            begin
              HeroRuntoTargetXY;
              Result := True;
            end
            else
            begin
              HeroGotoTargetXY;
              Result := True;
            end;

            ResetTempMoveStatus;
          end;
        end
        else if (m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_nCurrX) = 3) or (abs(m_Master.m_nCurrY - m_nCurrY) = 3)) and
          (not m_boProtectStatus) then
        begin
          nTargetX := m_nCurrX;
          nTargetY := m_nCurrY;

          if GetGotoXY(GetDirXY(m_Master.m_nCurrX, m_Master.m_nCurrY), nTargetX, nTargetY) then
          begin
            m_btLastDir := GetDirXY(m_Master.m_nCurrX, m_Master.m_nCurrY);
            SetTargetXY(nTargetX, nTargetY);
            HeroGotoTargetXY;
            Result := True;
            ResetTempMoveStatus;
            // Break;
          end;
        end
        else if (m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_nCurrX) > 3) or (abs(m_Master.m_nCurrY - m_nCurrY) > 3)) and
          (not m_boProtectStatus) then
        begin
          for I := 0 to 7 do
          begin
            nTargetX := m_nCurrX;
            nTargetY := m_nCurrY;

            if GetGotoXY(I, nTargetX, nTargetY) then
            begin
              m_btLastDir := I;
              SetTargetXY(nTargetX, nTargetY);
              HeroGotoTargetXY;
              Result := True;
              ResetTempMoveStatus;
              Break;
            end;
          end;
        end
        else
        begin
          m_btLastDir := 8;
        end;
      end;
    end;
  except
    MainOutMessage(Format('THeroObject.FollowMaster Error; Code = %d', [ErrCode]));
  end;
end;

function THeroObject.FindGroupMagic: pTUserMagic;
begin
  Result := FindMagic(GetGroupMagicId);
end;

function THeroObject.GetGroupMagicId: Integer;
begin
  Result := 0;
  if m_Master = nil then
    Exit;
  case m_Master.m_btJob of
    0:
      begin
        case m_btJob of
          0:
            Result := 60;
          1:
            Result := 62;
          2:
            Result := 61;
        end;
      end;
    1:
      begin
        case m_btJob of
          0:
            Result := 62;
          1:
            Result := 65;
          2:
            Result := 64;
        end;
      end;
    2:
      begin
        case m_btJob of
          0:
            Result := 61;
          1:
            Result := 64;
          2:
            Result := 63;
        end;
      end;
  end;
end;

procedure THeroObject.SetPKFlag(BaseObject: TBaseObject);
begin
  inherited SetPKFlag(BaseObject);
end;

procedure THeroObject.SetLastHiter(BaseObject: TBaseObject);
begin
  if m_boTarget then
  begin
    if BaseObject = m_TargetCret then
    begin
      inherited SetLastHiter(BaseObject);
    end
    else
    begin
      m_LastHiter := BaseObject;
      m_LastHiterTick := MyGetTickCount();
      m_ExpHitterTick := MyGetTickCount();
    end;
  end
  else
  begin
    inherited SetLastHiter(BaseObject);
  end;
end;

function THeroObject.Operate(ProcessMsg: pTProcessMessage): Boolean;
var
  ClientAbilityNG: TClientAbilityNG;
  Int64Value: Int64;
  MessageBodyWL: TMessageBodyWL;
  Feature: array [0 .. 255] of AnsiChar;
  AttackFrom: TBaseObject;
  S: AnsiString;
resourcestring
  sExceptionMsg = '[Exception] THeroObject.Operate ';
begin
  Result := True;
  if ProcessMsg = nil then
  begin
    Result := False;
    Exit;
  end;
  // try
  case ProcessMsg.wIdent of
    RM_MAKEGHOST:
      begin
        SendDefMessage(SM_HEROLOGOUT_OK, NativeInt(Self), 0, 0, 0, '');
        MakeGhost;
        // MainOutMessage('RM_MAKEGHOST');
      end;
    RM_MYHEROLOGON:
      begin
        m_DefMsg := MakeDefaultMsg(SM_MYHEROLOGON, NativeInt(ProcessMsg.BaseObject), ProcessMsg.nParam2, ProcessMsg.nParam3,
          ProcessMsg.wParam { MakeWord(m_btDirection, m_btGender) } );
        // MessageBodyWL.lParam1 := TBaseObject(ProcessMsg.BaseObject).GetFeatureToLongOld()
        MessageBodyWL.lParam1 := TBaseObject(ProcessMsg.BaseObject).GetFeatureToLong(@Feature);
        MessageBodyWL.lParam2 := TBaseObject(ProcessMsg.BaseObject).m_nCharStatus;
        MessageBodyWL.lTag1 := 0; // TBaseObject(ProcessMsg.BaseObject).GetFeatureEx;
        MessageBodyWL.lTag2 := NativeInt(TBaseObject(ProcessMsg.BaseObject).m_Master);

        SetLength(S, SizeOf(TMessageBodyWL) + MessageBodyWL.lParam1);
        Move(MessageBodyWL, S[1], SizeOf(TMessageBodyWL));
        Move(Feature[0], S[1 + SizeOf(TMessageBodyWL)], MessageBodyWL.lParam1);
        SendSocket(@m_DefMsg, S);
      end;
    RM_HEROLOGON_OK:
      begin
        SendDefMessage(SM_HEROLOGON_OK, NativeInt(Self), BoolToInt(TPlayObject(m_Master).m_boFixedHero) { 有没有评定 } ,
          BoolToInt(m_boIsDeputy)
          { 是不是副将 } , 0, Format('%.2f', [m_rLoyalPoint]));
      end;

    RM_REFABILNG:
      begin // 刷新内力
        SendDefMessage(SM_REFABILNG, NativeInt(ProcessMsg.BaseObject), ProcessMsg.nParam2, ProcessMsg.nParam3, 0, '')
      end;
    RM_LEVELUPNG:
      begin // 内功升级
        m_DefMsg := MakeDefaultMsg(SM_LEVELUPNG, NativeInt(ProcessMsg.BaseObject), ProcessMsg.nParam1, ProcessMsg.nParam2,
          ProcessMsg.wParam);
        SendSocket(@m_DefMsg, '');
      end;
    RM_ABILITYNG: // 内功属性
      begin
        ClientAbilityNG.Level := m_AbilNG.Level;
        ClientAbilityNG.NH := m_AbilNG.NH;
        ClientAbilityNG.MaxNH := m_AbilNG.MaxNH;
        ClientAbilityNG.Exp := m_AbilNG.Exp;
        ClientAbilityNG.MaxExp := m_AbilNG.MaxExp;
        ClientAbilityNG.NGDamage := GetNGAddPower;
        ClientAbilityNG.UnNGDamage := GetNGDecPower;

        m_DefMsg := MakeDefaultMsg(SM_HEROABILITYNG, 0, 0, 0, 0);
        SendSocketEx(@m_DefMsg, @ClientAbilityNG, SizeOf(ClientAbilityNG));
      end;
    RM_ABILITYALCOHOL:
      begin // 酒属性
        m_DefMsg := MakeDefaultMsg(SM_HEROABILITYALCOHOL, 0, 0, 0, 0);
        SendSocketEx(@m_DefMsg, @m_Alcohol, SizeOf(TAbilityAlcohol));
      end;
    RM_ABILITYMERIDIANS:
      begin // 经脉
        m_DefMsg := MakeDefaultMsg(SM_HEROABILITYMERIDIANS, 0, 0, 0, 0);
        SendSocketEx(@m_DefMsg, @m_HumMeridians, SizeOf(THumMeridians));
      end;
    RM_CONTINUOUSMAGICORDER:
      begin // 连击顺序
        SendDefMessage(SM_CONTINUOUSMAGICORDER, BoolToInt(m_boOpenLastContinuous),
          MakeWord(m_ContinuousMagicOrder[0], m_ContinuousMagicOrder[1]),
          MakeWord(m_ContinuousMagicOrder[2], m_ContinuousMagicOrder[3]), 1, '');
      end;
    RM_TRAININGNG:
      begin // 是否修炼内功心法 界面相应显示内功心法界面  series=0 人物 series=1 英雄
        SendDefMessage(SM_TRAININGNG, ProcessMsg.wParam, ProcessMsg.nParam1, ProcessMsg.nParam2, 1, '');
      end;

    RM_SENDDELITEMLIST:
      begin // 10148  004D9D48  //SM_DELITEMS
        SendDelItemList(ProcessMsg.sMsg, ProcessMsg.nParam1);
      end;
    RM_STRUCK:
      begin
        if (m_Master <> nil) and (not m_Master.m_boGhost) and (not Self.m_boLogOut) and (not m_boGhost) then
        begin
          AttackFrom := TBaseObject(ProcessMsg.nParam3);
          if (TObject(ProcessMsg.BaseObject) = Self) and (AttackFrom <> nil) and (AttackFrom <> Self) then
          begin
            SetLastHiter(AttackFrom);
            Struck(AttackFrom); { 0FFEC }
            BreakHolySeizeMode();
            if (AttackFrom <> m_Master) and (AttackFrom.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
              (ProcessMsg.wParam > 0) then
            begin
              // A英雄攻击B英雄时，A英雄的主人也要灰名 chongchong 2017-12-08
              if AttackFrom.m_Master <> nil then
              begin
                m_Master.SetPKFlag(AttackFrom.m_Master);
              end;

              m_Master.SetPKFlag(AttackFrom);
            end;
            if g_Config.boMonSayMsg then
              MonsterSayMsg(AttackFrom, s_UnderFire);
          end;
        end;
      end;

    RM_WINEXP:
      begin
        if ProcessMsg.wParam = 0 then
          m_DefMsg := MakeDefaultMsg(SM_HEROWINEXP, m_Abil.Exp, LoWord(ProcessMsg.nParam1), HiWord(ProcessMsg.nParam1), 0)
        else
          m_DefMsg := MakeDefaultMsg(SM_HEROWINEXP, m_AbilNG.Exp, LoWord(ProcessMsg.nParam1), HiWord(ProcessMsg.nParam1), 1);
        SendSocket(@m_DefMsg, '');
        // MainOutMessage('RM_WINEXP:' + IntToStr(ProcessMsg.nParam1));
      end;

    RM_ABILITY:
      begin
        m_DefMsg := MakeDefaultMsg(SM_HEROABILITY, NativeInt(Self), MakeWord(m_btJob, m_btGender), 0, 0);
        SendSocket(@m_DefMsg, zLibCompressBuffer(@m_WAbil, SizeOf(TAbility)));
      end;
    RM_SENDUSEITEMS:
      SendUseitems();
    RM_QUERYBAGITEMS:
      begin
        DoQueryBagItems();
        RefBagItemCount;
      end;

    RM_SENDMYMAGIC:
      SendUseMagic;
    RM_WEIGHTCHANGED:
      begin
        SendDefMessage(SM_HEROWEIGHTCHANGED, m_WAbil.Weight, m_WAbil.WearWeight, m_WAbil.HandWeight, 0, '');
      end;
    { RM_FEATURECHANGED: begin
      SendDefMessage(SM_FEATURECHANGED,
      NativeInt(ProcessMsg.BaseObject),
      LoWord(ProcessMsg.nParam1),
      HiWord(ProcessMsg.nParam1),
      ProcessMsg.wParam,
      '');
      end; }
    RM_MAGIC_LVEXP:
      begin
        SendDefMessage(SM_HEROMAGIC_LVEXP, ProcessMsg.nParam1, ProcessMsg.nParam2, LoWord(ProcessMsg.nParam3),
          HiWord(ProcessMsg.nParam3), ProcessMsg.sMsg);
      end;
    RM_DURACHANGE:
      begin
        SendDefMessage(SM_HERODURACHANGE, ProcessMsg.nParam1, ProcessMsg.wParam, LoWord(ProcessMsg.nParam2),
          HiWord(ProcessMsg.nParam2), '');
      end;

    RM_SUBABILITY:
      begin
        {
          SendDefMessage(SM_HEROSUBABILITY,
          MakeLong(MakeWord(m_nAntiMagic, 0), 0),
          MakeWord(m_btHitPoint, m_btSpeedPoint),
          MakeWord(m_btAntiPoison, m_nPoisonRecover),
          MakeWord(m_nHealthRecover, m_nSpellRecover),
          IntToStr(m_nNPRecoverTime) + '/' + IntToStr(m_nNPRecoverPoint));                          // 增加内力恢复速度 %  //内力恢复速度加几点
        }
        SendDefMessage(SM_HEROSUBABILITY, MakeLong(MakeWord(m_nAntiMagic, 0), m_btSpeedPoint), m_btHitPoint,
          MakeWord(m_btAntiPoison, m_nPoisonRecover), MakeWord(m_nHealthRecover, m_nSpellRecover),
          IntToStr(m_nNPRecoverTime) + '/' + IntToStr(m_nNPRecoverPoint));

      end;
    RM_REFANGRYVALUE:
      begin
        SendDefMessage(SM_HEROANGERVALUE, NativeInt(ProcessMsg.BaseObject), ProcessMsg.wParam, ProcessMsg.nParam1,
          ProcessMsg.nParam2, '');
      end;
    RM_UPDATEJEWELRYBOX:
      begin
        if m_Master <> nil then
          TPlayObject(m_Master).SendJewelryBox(True);
      end;
    RM_UPDATEGODBLESS:
      begin
        if m_Master <> nil then
          TPlayObject(m_Master).SendUpdateGodBless(True);
      end;
    RM_FENGHAO:
      begin
        if m_Master <> nil then
          TPlayObject(m_Master).SendUpdateFengHao(True);
      end;
    RM_INCHEALTH:
      begin
        Int64Value := Int64(m_WAbil.HP) + LongWord(ProcessMsg.nParam1);
        m_WAbil.HP := Min(Int64Value, m_WAbil.MaxHP);

        Int64Value := Int64(m_WAbil.MP) + LongWord(ProcessMsg.nParam2);
        m_WAbil.MP := Min(Int64Value, m_WAbil.MaxMP);
        HealthSpellChanged();
      end;
    RM_HERO_ATTACK_MODE:
      begin
        SendDefMessage(SM_HERO_ATTACK_MODE, NativeInt(Self), ProcessMsg.wParam, ProcessMsg.nParam1, ProcessMsg.nParam2,
          ProcessMsg.sMsg);
      end;
  else
    begin
      Result := inherited Operate(ProcessMsg);
    end;
  end;
  { except
    MainOutMessage('ProcessMsg.wIdent:' + IntToStr(ProcessMsg.wIdent));
    end; }
end;

function THeroObject.WearFirDragon: Boolean;
var
  StdItem: pTStdItem;
begin
  Result := False;
  if m_UseItems[U_BUJUK].wIndex > 0 then
  begin
    StdItem := UserEngine.GetStdItem(m_UseItems[U_BUJUK].wIndex);
    if (StdItem <> nil) and (StdItem.StdMode = 25) and (StdItem.Shape = 9) then
    begin
      Result := True;
    end;
  end;
end;
(*
  procedure THeroObject.RepairFirDragon(btType: Byte; nItemIdx: Integer; sItemName: string);
  var
  I, n14: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  sUserItemName: string;
  boRepairOK: Boolean;
  ItemList: TList;
  OldDura: Word;
  begin
  boRepairOK := False;
  StdItem := nil;
  UserItem := nil;
  n14 := -1;
  if (m_Master <> nil) and WearFirDragon then
  begin
  if m_UseItems[U_BUJUK].Dura < m_UseItems[U_BUJUK].DuraMax then
  begin
  OldDura := m_UseItems[U_BUJUK].Dura;
  ItemList := m_ItemList;
  if ItemList <> nil then
  begin
  for I := 0 to ItemList.Count - 1 do
  begin
  UserItem := ItemList.Items[I];
  if (UserItem <> nil) and (UserItem.MakeIndex = nItemIdx) then
  begin
  // 取自定义物品名称
  sUserItemName := '';
  if UserItem.btValue[13] = 1 then
  sUserItemName := ItemUnit.GetCustomItemName(UserItem.MakeIndex, UserItem.wIndex);
  if sUserItemName = '' then
  sUserItemName := UserEngine.GetStdItemName(UserItem.wIndex);

  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if StdItem <> nil then
  begin
  if CompareText(sUserItemName, sItemName) = 0 then
  begin
  n14 := I;
  Break;
  end;
  end;
  end;
  UserItem := nil;
  end;
  if (StdItem <> nil) and (UserItem <> nil) and (StdItem.StdMode = 42) then
  begin
  if m_UseItems[U_BUJUK].Dura + UserItem.DuraMax < m_UseItems[U_BUJUK].DuraMax then
  Inc(m_UseItems[U_BUJUK].Dura, UserItem.DuraMax)
  else
  m_UseItems[U_BUJUK].Dura := m_UseItems[U_BUJUK].DuraMax;
  boRepairOK := True;
  DelBagItem(n14);
  end;
  end;
  end;
  end;
  if boRepairOK then
  begin
  if OldDura <> m_UseItems[U_BUJUK].Dura then
  SendMsg(Self, RM_DURACHANGE, U_BUJUK, m_UseItems[U_BUJUK].Dura, m_UseItems[U_BUJUK].DuraMax, 0, '');
  SendDefMessage(SM_REPAIRFIRDRAGON_OK, 0, 0, 0, 0, '');
  end
  else
  begin
  SendDefMessage(SM_REPAIRFIRDRAGON_FAIL, 0, 0, 0, 0, '');
  end;
  end;
*)

function THeroObject.CheckLastHiterRange(LastHiter: TBaseObject; CheckMaster: Boolean): Boolean;
begin
  Result := False;
  if LastHiter <> nil then
  begin
    if CheckMaster then
    begin
      if (abs(LastHiter.m_nCurrX - m_Master.m_nCurrX) <= 10) and (abs(LastHiter.m_nCurrY - m_Master.m_nCurrY) <= 10) and
        (LastHiter.m_PEnvir = m_Master.m_PEnvir) then
        Result := True;
    end
    else if (abs(LastHiter.m_nCurrX - m_nCurrX) <= 10) and (abs(LastHiter.m_nCurrY - m_nCurrY) <= 10) and
      (LastHiter.m_PEnvir = m_PEnvir) then
      Result := True;
  end;
end;

function THeroObject.CheckInMasterRange(nX, nY: Integer): Boolean;
begin
  if g_Config.boHeroFollowMasterWithDiffScreen then
  begin
    Result := (abs(nX - m_Master.m_nCurrX) < HERO_MASTER_RANGE) and (abs(nY - m_Master.m_nCurrY) < HERO_MASTER_RANGE);
  end
  else
    Result := True;
end;

procedure THeroObject.Run;
var
  nX, nY: Integer;
  I: Integer;
  SlaveObject: TBaseObject;
  nErrCode: Integer;
  boCheckMasterRange: Boolean;
  nTemp: Integer;
  UserMagic: pTUserMagic;
  boInSafeArea: Boolean;
resourcestring
  sExceptionMsg = '[Exception] THeroObject.Run; Error code: %d';
begin
  boCheckMasterRange := True;
  nErrCode := 0;
  try
    if m_OPEnvir <> m_PEnvir then
    begin
      m_OPEnvir := m_PEnvir;
      m_nMoveIndex := -1;
      SetLength(m_MovePath, 0);
      m_nMoveSameCount := 0;
      m_nOLastDir := -1;
      m_nLastDir := -1;
    end;

    // 如果主人挂了，英雄自杀 // 人物死亡后过十秒钟才收英雄 chongchong 2013-12-10
    if (m_Master = nil) //
      or ((m_Master <> nil) // -
      and (m_Master.m_boDeath or m_Master.m_boGhost) // -
      and (MyGetTickCount - m_Master.m_dwDeathTick >= g_Config.dwHeroLogonTimeMasterDie * 1000)) then
    begin
      // MakeGhost();
      nErrCode := 1;
      LogOut;
      nErrCode := 2;
      inherited;
      nErrCode := 3;
      Exit;
    end;

    nErrCode := 4;
    if m_boLogOut or m_boGhost or m_boDeath { or m_boFixedHideMode or m_boStoneMode or (not CanMove) } then
    begin
      nErrCode := 5;
      inherited;
      nErrCode := 6;
      Exit;
    end;

    // 取消强制和平模式 chongchong 2017-03-29
    if g_nKey_HeroExt <> 0 then
    begin
      if (m_Master <> nil) and m_boCancelHeroForcePeaceMode then
      begin
        if (MyGetTickCount - m_dwCancelHeroForcePeaceModeTick <= m_dwCancelHeroForcePeaceModeTime) //
          or (m_dwCancelHeroForcePeaceModeTime = 0) then
        begin
          if m_btAttatckMode <> m_Master.m_btAttatckMode then
            m_btAttatckMode := m_Master.m_btAttatckMode;
        end
        else
        begin
          m_btAttatckMode := HAM_PEACE;
          m_boCancelHeroForcePeaceMode := False;
          m_dwCancelHeroForcePeaceModeTime := 0;
          DelTargetCreat;
        end;
      end;

      if m_boDecAddAngryValueTime then
      begin
        if (MyGetTickCount - m_dwDecAddAngryValueTimeTick > m_dwDecAddAngryValueTimeTime) //
          and (m_dwDecAddAngryValueTimeTime > 0) then
        begin
          m_boDecAddAngryValueTime := False;
          m_nDecAddAngryValueTimeValue := 0;
        end;
      end;
    end;

    nErrCode := 7;
    if Length(g_sHeroHPLessMsg) > 0 then
    begin
      if (MyGetTickCount - m_HeroHPHintTick) > 3000 then
      begin
        if m_WAbil.HP <= m_WAbil.MaxHP div 10 then
        begin
          // m_Master.SendScreenMsg('英雄HP不足，请及时补充！', 250, 0, 14, 350);
          m_Master.SendMsg(m_Master, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, g_Config.sHeroSayPrefix + g_sHeroHPLessMsg);
        end;

        m_HeroHPHintTick := MyGetTickCount;
      end;
    end;

    nErrCode := 8;
    if Length(g_sHeroMPLessMsg) > 0 then
    begin
      if m_btJob in [JOB_WIZARD, JOB_TAOS] then
      begin
        { TODO -ochongchong -c新增 : 英雄低MP时提示 【2013-09-13】 }
        if (MyGetTickCount - m_HeroMPHintTick) > 3000 then
        begin
          if m_WAbil.MP <= m_WAbil.MaxMP div 20 then
          begin
            // m_Master.SendScreenMsg('英雄MP不足，请及时补充！', 250, 0, 14, 350);
            m_Master.SendMsg(m_Master, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, g_Config.sHeroSayPrefix + g_sHeroMPLessMsg);
          end;

          m_HeroMPHintTick := MyGetTickCount;
        end;
      end;
    end;

    // 英雄增加进出安全区名字重新着色逻辑 Cursor 2023-06-10 14:05:22
    if (m_Master <> nil) and (m_Master.m_MyGuild <> nil) then
    begin
      if TGUild(m_Master.m_MyGuild).GuildWarList.Count > 0 then
      begin
        boInSafeArea := InSafeArea();
        if boInSafeArea <> m_boInSafeArea then
        begin
          m_boInSafeArea := boInSafeArea;
          RefNameColor();
        end;
      end;
    end;

    nErrCode := 9;
    // 修正英雄不同屏回到主人身边，设置攻击间隔时无效 chongchong 2017-04-18
    if (m_PEnvir <> m_Master.m_PEnvir) then
    begin
      nErrCode := 10;
      { TODO -ochongchong -c添加 : 英雄休息时不随主人移动 }
      if (m_btAttackMode <> 2) or not g_Config.boHeroNoMoveOnSleep then
      begin
        // 守护模式时，如果主人换了地图，英雄不用再守护 chongchong 2015-10-19
        if m_boProtectStatus then
          m_boProtectStatus := False;

        m_Master.GetBackPosition(nX, nY);
        if not m_Master.m_PEnvir.CanWalk(nX, nY, True) then
        begin
          for I := 0 to 7 do
          begin
            if m_Master.m_PEnvir.GetNextPosition(m_Master.m_nCurrX, m_Master.m_nCurrY, I, 1, nX, nY) then
            begin
              if m_Master.m_PEnvir.CanWalk(nX, nY, True) then
                Break;
            end;
          end;
        end;

        DelTargetCreat;
        m_nTargetX := nX;
        m_nTargetY := nY;

        SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
      end
    end
    else if (not m_boProtectStatus) //
      and ((abs(m_nCurrX - m_Master.m_nCurrX) > Max(HERO_MASTER_RANGE, g_Config.nMagicAttackRage * 2)) //
      or (abs(m_nCurrY - m_Master.m_nCurrY) > Max(HERO_MASTER_RANGE, g_Config.nMagicAttackRage * 2))) then
    begin
      // 英雄锁定目标后，不用跟主人了 chongchong 2017-10-31

      // if (((not m_boTarget) or (m_TargetCret = nil)) and (m_btAttackMode in [0, 3])) or ((m_btAttackMode <> 1) and g_Config.boHeroFollowMasterWithDiffScreen) then
      // 千里追凶 chongchong 2017-12-11
      if (m_TargetCret = nil) or (m_btAttackMode = 1 { 休息状态 } ) or
        ((m_btAttackMode in [0, 3]) and g_Config.boHeroFollowMasterWithDiffScreen) then
      begin
        DelTargetCreat;
        if FollowMaster then
        begin
          inherited;
          nErrCode := 15;
          Exit;
        end
        else
          boCheckMasterRange := False;
      end;
    end;

    nErrCode := 16;
    if m_boFixedHideMode or m_boStoneMode or (not CanMove) then
    begin
      nErrCode := 17;
      inherited;
      nErrCode := 18;
      Exit;
    end;

    // 修正英雄捡物时跑来跑去，到处窜 2019-10-22 00:12:33
    if (m_SelItemObject <> nil) //
      and (m_nTargetX = m_SelItemObject.m_nMapX) //
      and (m_nTargetY = m_SelItemObject.m_nMapY) //
      and (MyGetTickCount - m_dwTargetFocusTick <= 5000) then
    begin
      nX := abs(m_nCurrX - m_nTargetX);
      nY := abs(m_nCurrY - m_nTargetY);
      if (nX + nY > 0) and (nX + nY < 4) then
      begin
        GotoTargetXY;
        Exit;
      end;
    end;

    nErrCode := 19;
    if (m_wStatusTimeArr[POISON_STONE] <> 0) or m_boForeverFrozen then
      Exit;

    // 是否自动开盾
    // m_HeroAutoDong := m_HeroHuman.m_HeroAutoDong;

    if HeroThink then
    begin
      nErrCode := 20;
      inherited;
      nErrCode := 21;
      Exit;
    end;

    nErrCode := 22;
    if g_Config.boHeroAutoSuperShiled and (not m_boSuperShiled) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_75);
      if (UserMagic <> nil) then
      begin
        nTemp := g_Config.nOpenSuperShiledRate -
          Round(g_Config.nOpenSuperShiledRate / 100 * ((UserMagic.btLevel + UserMagic.btNewLevel) *
          g_Config.nOpenSuperShiledLevelUpAddRate));

        if (nTemp <= 0) or (Random(nTemp) = 0) then
        begin
          nErrCode := 23;
          if OpenSuperShiled then
          begin
            nErrCode := 24;
            inherited;
            nErrCode := 25;
            Exit;
          end;
        end;
      end;
    end;

    nErrCode := 26;
    if m_boUseGroupSpell then
    begin
      if (m_btAngryValue = 0) then
      begin
        m_boUseGroupSpell := False;
      end
      else
      begin
        // 主号无目标，按Ctrl+S合击，再攻击目标，英雄跟过来攻击 chongchong 2018-06-25 23:16:25
        if (m_btAngryValue > 0) //
          and (m_Master <> nil) //
          and (m_Master.m_TargetCret <> nil) //
          and (m_TargetCret <> m_Master.m_TargetCret) //
          and (abs(m_Master.m_nCurrX - m_Master.m_TargetCret.m_nCurrX) <= 10) //
          and (abs(m_Master.m_nCurrY - m_Master.m_TargetCret.m_nCurrY) <= 10) //
          and (tick_diff(m_Master.m_dwSetTargetCretTick, MyGetTickCount) >= 100) //
          and IsProperTarget(m_Master.m_TargetCret) then
        begin
          m_btAttackMode := 0; // 切换到攻击模式
          SendMsg(Self, RM_HERO_ATTACK_MODE, 0, MakeWord(255, 252), 0, 0, '');

          FollowMaster;

          m_boTarget := False;
          SetTargetCreat(m_Master.m_TargetCret);
          if m_TargetCret <> nil then
          begin
            m_nTargetX := m_TargetCret.m_nCurrX;
            m_nTargetY := m_TargetCret.m_nCurrY;
            m_boTarget := True;
          end;
        end;
      end;
    end;

    // ChangeModeEx 模式(1-10) 时间(1-65535) 附加值(1-65535)
    // 说明: 1=无敌 2=隐身 3=HP 4=MP 5=攻击力 6=魔法力 7=道术力 8=攻击速度 9=禁止攻击 10=锁定
    // 休息或跟随主人状态，就别管怪了，不然在攻击范围内但打不到怪的时候不是一般的蛋疼 chongchong 2013-08-14
    if (m_btAttackMode in [1, 2]) or ((m_dwChangeModeExTick[8] > 0) and (MyGetTickCount() <= m_dwChangeModeExTick[8]) { 禁止攻击 } )
    //
      or ((m_dwChangeModeExTick[9] > 0) and (MyGetTickCount() <= m_dwChangeModeExTick[9]) { 锁定 } ) //
      or (m_nChangeAppr >= 0) then
    begin
      nErrCode := 27;
      DelTargetCreat;
      m_boTarget := False;

      if not m_boUseGroupSpell then
        ProcessAngryChangeUP
      else
        ProcessAngryChangeDown;

      // 跟随状态不躲怪 chongchong 2017-11-16
      (*
        // 跟随状态也要躲怪 chongchong 2015-10-19
        if ((m_btAttackMode = 1) or ((m_dwChangeModeExTick[8] > 0) and (MyGetTickCount() <= m_dwChangeModeExTick[8]))) and
        (not ((m_dwChangeModeExTick[9] > 0) and (MyGetTickCount() <= m_dwChangeModeExTick[9])){锁定}) then
        begin
        if (m_LastHiter <> nil) and (abs(m_nCurrX - m_LastHiter.m_nCurrX) < 3) and (abs(m_nCurrY - m_LastHiter.m_nCurrY) < 3) and (Random(2) = 0) then
        begin
        if (m_PEnvir = m_Master.m_PEnvir) and
        ((abs(m_nCurrX - m_Master.m_nCurrX) > (g_Config.nMagicAttackRage - 2)) or
        (abs(m_nCurrY - m_Master.m_nCurrY) > (g_Config.nMagicAttackRage - 2))) then
        begin
        n14 := GetNextDirection(m_nCurrX, m_nCurrY, m_Master.m_nCurrX, m_Master.m_nCurrY);
        m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, n14, 5, m_nTargetX, m_nTargetY);
        end;
        begin
        n14 := GetNextDirection(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, m_nCurrX, m_nCurrY);
        m_PEnvir.GetNextPosition(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY, n14, 5, m_nTargetX, m_nTargetY);
        end;

        if (m_nTargetX > 0) and (m_nTargetY > 0) then
        HeroAutoMove;

        SetTargetXY(m_LastHiter.m_nCurrX, m_LastHiter.m_nCurrY);
        end;

        if m_btJob = 2 then
        HeroTaosProtectSelfAndFirends
        else if m_btJob = 1 then
        HeroWizardAutoDun(TPlayObject(m_Master));
        end
        else if m_btAttackMode = 2 then     // 休息状态要能开盾或治疗术
      *)
      begin
        if m_btJob = 2 then
          HeroTaosProtectSelfAndFirends
        else if m_btJob = 1 then
          HeroWizardAutoDun(TPlayObject(m_Master));
      end;
      nErrCode := 28;
    end
    else if m_btAttackMode in [0, 3] then // 英雄攻击状态
    begin
      nErrCode := 29;
      m_Master.GetBackPosition(nX, nY);
      if (abs(m_nCurrX - nX) > 1) or (abs(m_nCurrY - nY) > 1) then
      begin
        m_nTargetX := nX;
        m_nTargetY := nY;
      end;

      nErrCode := 30;
      // if (not InSafeZone) then
      begin
        // 如果没有锁定目标，开始寻怪
        if (not m_boTarget) then
        begin
          nErrCode := 31;
          if ((m_TargetCret <> nil) //
            and (m_TargetCret.m_btRaceServer <> 55) //
            and (MyGetTickCount - m_dwStruckTick > GetAttackIntervalTime(False) * 8))
          { 如果寻到怪，并且2秒内都没有攻击(没攻击的原因可能是蓝不够等)，考虑重搜 }
            or (m_TargetCret = nil) or m_TargetCret.m_boGhost //
            or m_TargetCret.m_boDeath { or (MyGetTickCount - m_dwSearchTargetTick > 8000) } then
          begin
            nErrCode := 32;
            m_dwSearchTargetTick := MyGetTickCount();
            SearchTarget();
            nErrCode := 33;
          end
          else if ((m_TargetCret = nil) or m_TargetCret.m_boGhost or m_TargetCret.m_boDeath) //
            and (m_Master.m_TargetCret <> nil) //
            and IsAttackTarget(m_Master.m_TargetCret) then
          begin
            nErrCode := 34;
            m_dwSearchTargetTick := MyGetTickCount();
            SetTargetCreat(m_Master.m_TargetCret);
            nErrCode := 35;
          end;
        end
        else if m_TargetCret <> nil then
        begin
          // 锁定目标死后，或目标进入安全区，重新寻怪~
          if m_TargetCret.m_boGhost //
            or m_TargetCret.m_boDeath //
            or ((m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and m_TargetCret.InSafeZone) //
            or ((m_TargetCret.m_Master <> nil) and (m_TargetCret.m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
            m_TargetCret.InSafeZone) then
          begin
            DelTargetCreat;
            nErrCode := 36;
            m_boTarget := False;
            m_dwSearchTargetTick := MyGetTickCount();
            SearchTarget();
            nErrCode := 37;
          end // 锁定的目标飞走后。不要锁定了 2020-11-24
          else if (m_TargetCret.m_PEnvir <> m_PEnvir) //
            or (abs(m_TargetCret.m_nCurrX - m_nCurrX) > 20) //
            or (abs(m_TargetCret.m_nCurrY - m_nCurrY) > 20) then
          begin
            DelTargetCreat;
            nErrCode := 36;
            m_boTarget := False;
            m_dwSearchTargetTick := MyGetTickCount();
            SearchTarget();
            nErrCode := 37;
          end;
        end;
      end;

      nErrCode := 38;
      { 英雄没有目标 }
      if (not m_boProtectStatus) and (not m_boTarget) and (m_TargetCret = nil) then
      begin
        // 主人回了安全区，英雄不打了chongchong 2017-10-31
        if (not m_Master.InSafeArea) or (m_Master.InSafeZone and InSafeZone) then
        begin
          nErrCode := 391;
          { 主人有攻击目标 }
          if (m_Master.m_TargetCret <> nil) //
            and (m_Master.m_TargetCret.m_Master <> m_Master) //
            and (m_Master.m_TargetCret.Master <> Master) //
            and IsAttackTarget(m_Master.m_TargetCret) then
          begin
            nErrCode := 40;
            SetTargetCreat(m_Master.m_TargetCret);
            nErrCode := 41;
          end { 或者有对象在攻击主人 }
          else if (m_Master.m_LastHiter <> nil) //
            and (m_Master.m_LastHiter.m_Master <> m_Master) //
            and (m_Master.m_LastHiter.Master <> Master) //
            and CheckLastHiterRange(m_Master.m_LastHiter, True) //
            and (MyGetTickCount - m_Master.m_LastHiterTick < 3000)
          { 2013-09-13 添加，攻击主人都已经是很久以前的事了，英雄不做计较 }
            and IsAttackTarget(m_Master.m_LastHiter) then
          begin
            nErrCode := 42;
            SetTargetCreat(m_Master.m_LastHiter);
            nErrCode := 43;
          end;

          nErrCode := 44;
          { 如果还是没找到目标，帮宝宝 }
          if m_TargetCret = nil then
          begin
            nErrCode := 45;
            for I := 0 to m_SlaveList.Count - 1 do
            begin
              nErrCode := 46;
              SlaveObject := m_SlaveList.Items[I];
              if (SlaveObject.m_TargetCret <> nil) //
                and (SlaveObject.m_TargetCret.m_Master <> m_Master) //
                and (SlaveObject.m_TargetCret.Master <> Master) //
                and (SlaveObject.m_TargetCret.Master <> Self) //
                and IsAttackTarget(SlaveObject.m_TargetCret) then
              begin
                nErrCode := 47;
                SetTargetCreat(SlaveObject.m_TargetCret);
                nErrCode := 48;
              end
              else if (SlaveObject.m_LastHiter <> nil) //
                and (SlaveObject.m_LastHiter.m_Master <> m_Master) //
                and (SlaveObject.m_LastHiter.Master <> Master) //
                and CheckLastHiterRange(SlaveObject.m_LastHiter, True) //
                and (MyGetTickCount - SlaveObject.m_LastHiterTick <= 3000) //
                and (not(SlaveObject.m_LastHiter.m_btRaceServer in [11, 12]) { 大刀卫士，巡逻卫士 } ) //
                and IsAttackTarget(SlaveObject.m_LastHiter) then
              begin
                nErrCode := 49;
                SetTargetCreat(SlaveObject.m_LastHiter);
                nErrCode := 50;
              end;

              if m_TargetCret <> nil then
                Break;
            end;
          end;
        end;
      end;

      nErrCode := 51;
      // 守护时协助主人
      if (m_boProtectStatus) //
        and (not m_boTarget) //
        and ((m_Master.m_TargetCret <> nil) or (m_Master.m_LastHiter <> nil)) then
      begin
        nErrCode := 52;

        // 主人回了安全区，英雄不打了chongchong 2017-10-31
        if (not m_Master.InSafeArea) or (m_Master.InSafeZone and InSafeZone) then
        begin
          if (m_Master.m_LastHiter <> nil) //
            and (m_Master.m_LastHiter.m_Master <> m_Master) //
            and (m_Master.m_LastHiter.Master <> Master) //
            and (abs(m_Master.m_LastHiter.m_nCurrX - m_nProtectTargetX) <= g_Config.nGuardRange) //
            and (abs(m_Master.m_LastHiter.m_nCurrY - m_nProtectTargetY) <= g_Config.nGuardRange) then
          begin
            nErrCode := 53;
            SetTargetCreat(m_Master.m_LastHiter);
          end;
        end;
      end;

      nErrCode := 54;
      { 当有人攻击英雄时，英雄傻傻不还手 }
      if m_TargetCret = nil then
      begin
        // 主人回了安全区，英雄不打了chongchong 2017-10-31
        if (not m_Master.InSafeArea) or (m_Master.InSafeZone and InSafeZone) then
        begin
          nErrCode := 55;
          if (m_LastHiter <> nil) // 是可攻击对象才设置为攻击目标 chongchong 2014-08-07 21:59:01
            and IsProperTarget(m_LastHiter) //
            and (m_LastHiter.m_Master <> m_Master) //
            and (m_LastHiter.Master <> Master) //
            and CheckLastHiterRange(m_LastHiter, False) then
            SetTargetCreat(m_LastHiter);
        end;
      end;

      nErrCode := 56;
      { 全职英雄攻击范围 }
      if boCheckMasterRange then
      begin
        if (m_TargetCret <> nil) //
          and (not m_boProtectStatus) //
          and (not m_boTarget) //
          and (not CheckInMasterRange(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)) then
          DelTargetCreat;
      end;

      nErrCode := 57;
      { 目标死了就不要打了 chongchong 2013-11-13 }
      if m_TargetCret <> nil then
      begin
        if m_TargetCret.m_boDeath or m_TargetCret.m_boGhost then
          DelTargetCreat
          // 人物或英雄跑到安全区后，就不打了 chongchong 2014-01-16
        else if ((m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and m_TargetCret.InSafeZone) or
          ((m_TargetCret.m_Master <> nil) and (m_TargetCret.m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
          m_TargetCret.InSafeZone) then
          DelTargetCreat;
      end;

      if (m_btAttackMode = 3) //
        and (m_Master <> nil) //
        and (m_Master.m_TargetCret <> nil) //
        and (not m_Master.m_TargetCret.m_boDeath) //
        and (not m_Master.m_TargetCret.m_boGhost) then
      begin
        // 主人回了安全区，英雄不打了chongchong 2017-10-31
        if (not m_Master.InSafeArea) or (m_Master.InSafeZone and InSafeZone) then
        begin
          if (not m_boTarget) or (m_TargetCret = nil) then
          begin
            if IsAttackTarget(m_Master.m_TargetCret) then
              SetTargetCreat(m_Master.m_TargetCret);
            m_dwTargetFocusTick := MyGetTickCount();
          end;
        end;
      end;

      nErrCode := 58;
      // 如果使用了合击
      if GroupAttackProcess then
      begin
        nErrCode := 59;
        inherited;
        nErrCode := 60;
        Exit;
      end
      else if FWaitGroupAttack and (MyGetTickCount - FWaitGroupAttackTick <= GetAttackIntervalTime(False) + 100) then
      begin
        inherited;
        Exit;
      end;

      nErrCode := 61;
      if ContinueousAttack then
      begin
        nErrCode := 62;
        inherited;
        nErrCode := 63;
        Exit;
      end;

      nErrCode := 64;
      if (m_boProtectStatus and not m_boProtectOK) or (m_boProtectStatus and (m_TargetCret = nil)) then
      begin // 是否守护状态
        if (m_nCurrX <> m_nProtectTargetX) or (m_nCurrY <> m_nProtectTargetY) then
        begin
          nErrCode := 68;
          m_nTargetX := m_nProtectTargetX;
          m_nTargetY := m_nProtectTargetY;
          HeroAutoMove(); // 英雄移动
        end
        else
        begin
          m_boProtectOK := True;
          // SysMsg(Format(g_sHeroProtectDone, [nX, nY]), 255, 252, t_Hint);
        end;

        nErrCode := 69;
        inherited;
        nErrCode := 70;
        Exit;
      end;

      // 如果目标不存在，守护
      // if m_TargetCret = nil then
      // begin
      // if m_boProtectStatus then  //是否守护状态
      // begin
      // if (m_nCurrX <> m_nProtectTargetX) or
      // (m_nCurrY <> m_nProtectTargetY) then
      // begin
      // nErrCode := 68;
      // m_nTargetX := m_nProtectTargetX;
      // m_nTargetY := m_nProtectTargetY;
      // HeroAutoMove();  //英雄移动
      // end;
      // nErrCode := 69;
      // inherited;
      // nErrCode := 70;
      // Exit;
      // end;
      // end;

      { 尝试非合击攻击 }
      if HeroAttackTarget then
      begin
        nErrCode := 65;
        inherited;
        nErrCode := 66;
        Exit;
      end;
      nErrCode := 67;
    end;
    nErrCode := 71;

    if (m_TargetCret = nil) and (not((m_btAttackMode = 2) and g_Config.boHeroNoMoveOnSleep)) then
      FollowMaster;

    nErrCode := 72;
    inherited;
    nErrCode := 73;
  except
    MainOutMessage(Format(sExceptionMsg, [nErrCode]));
  end;
end;

function THeroObject.CanSend: Boolean;
begin
  Result := (m_Master <> nil) and (not m_Master.m_boOffline) and (not m_Master.m_boDummyObject);
end;

procedure THeroObject.SysMsg(sMsg: AnsiString; MsgColor: TMsgColor; MsgType: TMsgType; boAddPrefix: Boolean);
begin
  if CanSend then
  begin
    if boAddPrefix then
      sMsg := g_Config.sHeroSayPrefix + sMsg;
    m_Master.SysMsg(sMsg, MsgColor, MsgType);
  end;
end;

procedure THeroObject.SysMsg(sMsg: AnsiString; FColor, BColor: Integer; MsgType: TMsgType; boAddPrefix: Boolean);
begin
  if CanSend then
  begin
    if boAddPrefix then
      sMsg := g_Config.sHeroSayPrefix + sMsg;
    m_Master.SysMsg(sMsg, FColor, BColor, MsgType);
  end;
end;

procedure THeroObject.SendSocket(DefMsg: pTDefaultMessage; sMsg: AnsiString);
begin
  if CanSend then
    TPlayObject(m_Master).SendSocket(DefMsg, sMsg);
end;

procedure THeroObject.SendSocketEx(DefMsg: pTDefaultMessage; Buffer: PAnsiChar; BufferLen: Integer);
begin
  if CanSend then
    TPlayObject(m_Master).SendSocketEx(DefMsg, Buffer, BufferLen);
end;

procedure THeroObject.SendDefMessage(wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word; sMsg: string);
begin
  if CanSend then
    TPlayObject(m_Master).SendDefMessage(wIdent, nRecog, nParam, nTag, nSeries, sMsg);
end;

procedure THeroObject.SendUseitems();
var
  I, nC: Integer;
  Item: pTStdItem;
  sSENDMSG: AnsiString;
  ClientItem: pTClientItem;
  InBuf: array [0 .. 102400] of AnsiChar;
  InBytes: Integer;
  Buffer: PAnsiChar;
begin
  if not CanSend then
    Exit;
  sSENDMSG := '';
  InBytes := 0;

  nC := 0;
  // MainOutMessage('TPlayObject.SendUseitems2');
  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
    if m_UseItems[I].wIndex > 0 then
    begin
      Item := UserEngine.GetStdItem(m_UseItems[I].wIndex);
      if Item <> nil then
      begin
        Inc(nC);
        Inc(InBytes, SizeOf(TClientItem) + 1);
        Buffer := @InBuf;

        Buffer := Buffer + (InBytes - (SizeOf(TClientItem) + 1));
        Buffer^ := AnsiChar(I);
        Inc(Buffer);

        ClientItem := pTClientItem(Buffer);

        UserItemToClientItem(@m_UseItems[I], Item, ClientItem, True, False);
      end;
    end;
  end;

  sSENDMSG := zLibCompressBuffer(@InBuf, InBytes);

  { 当死亡掉空装备后，身上没有装备时不发送到客户端更新 2013-08-26 }
  // if sSENDMSG <> '' then
  begin
    m_DefMsg := MakeDefaultMsg(SM_SENDHEROUSEITEMS, 0, Length(sSENDMSG), 0, nC);
    SendSocket(@m_DefMsg, sSENDMSG);
  end;
end;

procedure THeroObject.SendUseMagic();
var
  I: Integer;
  sSENDMSG: AnsiString;
  UserMagic: pTUserMagic;
  ClientMagic: pTClientMagic;
  InBuf: PAnsiChar;
  InBytes: Integer;
  Buffer: PAnsiChar;
begin
  if not CanSend then
    Exit;
  sSENDMSG := '';
  if m_MagicList.Count > 0 then
  begin
    InBytes := m_MagicList.Count * SizeOf(TClientMagic);
    GetMem(InBuf, InBytes + 1);
    try
      Buffer := InBuf;
      for I := 0 to m_MagicList.Count - 1 do
      begin
        UserMagic := m_MagicList.Items[I];
        ClientMagic := pTClientMagic(Buffer);

        UserMagicToClientMagic(UserMagic, ClientMagic);

        ClientMagic.dwInterval := 0;
        ClientMagic.dwRealInterval := 0;
        ClientMagic.dwLastUseTick := 0;
        Buffer := Buffer + SizeOf(TClientMagic);
      end;

      sSENDMSG := zLibCompressBuffer(InBuf, InBytes);
    finally
      FreeMem(InBuf);
    end;

    if sSENDMSG <> '' then
    begin
      m_DefMsg := MakeDefaultMsg(SM_SENDMYHEROMAGIC, 0, Length(sSENDMSG), 0, m_MagicList.Count);
      SendSocket(@m_DefMsg, sSENDMSG);
    end;
  end;
end;

function THeroObject.GetUserItemWeitht(nWhere: Integer): Integer;
var
  I: Integer;
  n14: Integer;
  StdItem: pTStdItem;
begin
  n14 := 0;
  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
    if (nWhere = -1) or (not(I = nWhere) and not(I = 1) and not(I = 2)) then
    begin
      StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
      if StdItem <> nil then
        Inc(n14, StdItem.Weight);
    end;
  end;

  for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
  begin
    if (nWhere = -1) or (I < U_JEWELRYITEM1) or (I > U_JEWELRYITEM6) or (I <> nWhere - U_JEWELRYITEM1) then
    begin
      StdItem := UserEngine.GetStdItem(m_JewelryBoxItems[I].wIndex);
      if StdItem <> nil then
        n14 := Min(High(Word), n14 + StdItem.Weight);
    end;
  end;

  for I := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
  begin
    if (nWhere = -1) or (I < U_GODBLESSITEM1) or (I > U_GODBLESSITEM12) or (I <> nWhere - U_GODBLESSITEM1) then
    begin
      StdItem := UserEngine.GetStdItem(m_GodBlessItems[I].wIndex);
      if StdItem <> nil then
        n14 := Min(High(Word), n14 + StdItem.Weight);
    end;
  end;
  Result := n14;
end;

function THeroObject.CheckTakeOnItems(nWhere: Integer; StdItem: pTStdItem): Boolean;
var
  Castle: TUserCastle;
begin
  Result := False;
  if (StdItem.StdMode = 10) and (m_btGender <> 0) then
  begin
    SysMsg(sWearNotOfWoMan, c_Red, t_Hint);
    Exit;
  end;
  if (StdItem.StdMode = 11) and (m_btGender <> 1) then
  begin
    SysMsg(sWearNotOfMan, c_Red, t_Hint);
    Exit;
  end;
  if (StdItem.StdMode = 16) and (StdItem.AniCount = 1) then
  begin // 斗笠
    SysMsg(sWearNotOfHero, c_Red, t_Hint);
    Exit;
  end;

  if CheckOverLapItem(StdItem) then
  begin
    Exit;
  end;

  if nWhere >= 0 then
  begin
    if (nWhere = 1) or (nWhere = 2) then
    begin
      if StdItem.Weight > m_WAbil.MaxHandWeight then
      begin
        SysMsg(sHandWeightNot, c_Red, t_Hint);
        Exit;
      end;
    end
    else
    begin
      if (StdItem.Weight + GetUserItemWeitht(nWhere)) > m_WAbil.MaxWearWeight then
      begin
        SysMsg(sWearWeightNot, c_Red, t_Hint);
        Exit;
      end;
    end;
  end;

  Castle := g_CastleManager.IsCastleMember(Self);
  case StdItem.Need of //
    0:
      begin
        if m_Abil.Level >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sLevelNot, c_Red, t_Hint);
        end;
      end;
    1:
      begin
        if m_WAbil.DC2 >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sDCNot, c_Red, t_Hint);
        end;
      end;
    10:
      begin
        if (m_btJob = LoWord(StdItem.NeedLevel)) and (m_Abil.Level >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sJobOrLevelNot, c_Red, t_Hint);
        end;
      end;
    11:
      begin
        if (m_btJob = LoWord(StdItem.NeedLevel)) and (m_WAbil.DC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sJobOrDCNot, c_Red, t_Hint);
        end;
      end;
    12:
      begin
        if (m_btJob = LoWord(StdItem.NeedLevel)) and (m_WAbil.MC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sJobOrMCNot, c_Red, t_Hint);
        end;
      end;
    13:
      begin
        if (m_btJob = LoWord(StdItem.NeedLevel)) and (m_WAbil.SC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sJobOrSCNot, c_Red, t_Hint);
        end;
      end;
    14:
      begin
        if m_Abil.Level = StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sLevelNotEqual, c_Red, t_Hint);
        end;
      end;
    18:
      begin // Need=18(表示穿戴需等级，装备可提高内力恢复速度) NeedLevel=50(等级条件) Stock=3(提高内力恢复速度)
        if m_Abil.Level >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sLevelNot, c_Red, t_Hint);
        end;
      end;
    19:
      begin // Need=19(表示穿戴需攻击力，装备可提高内力恢复速度%) NeedLevel=50(攻击力条件) Stock=3(提高内力恢复速度%)
        if (m_WAbil.DC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sDCNot, c_Red, t_Hint);
        end;
      end;
    20:
      begin // Need=20(表示穿戴需魔法，装备可提高内力恢复速度%) NeedLevel=50(魔法条件) Stock=3(提高内力恢复速度%)
        if (m_WAbil.MC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sMCNot, c_Red, t_Hint);
        end;
      end;
    21:
      begin // Need=21(表示穿戴需道术，装备可提高内力恢复速度%) NeedLevel=50(道术条件) Stock=3(提高内力恢复速度%)
        if (m_WAbil.SC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sSCNot, c_Red, t_Hint);
        end;
      end;
    22:
      begin // Need=22(表示穿戴需等级，装备可提高内力恢复速度+点) NeedLevel=50(等级条件) Stock=3(每次可提高内力值)
        if m_Abil.Level >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sLevelNot, c_Red, t_Hint);
        end;
      end;
    23:
      begin // Need=23(表示穿戴需攻击力，装备可提高内力恢复速度+点) NeedLevel=50(攻击力条件) Stock=3(每次可提高内力值)
        if (m_WAbil.DC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sDCNot, c_Red, t_Hint);
        end;
      end;
    24:
      begin // Need=24(表示穿戴需魔法，装备可提高内力恢复速度+点) NeedLevel=50(魔法条件) Stock=3(每次可提高内力值)
        if (m_WAbil.MC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sMCNot, c_Red, t_Hint);
        end;
      end;
    25:
      begin // Need=25(表示穿戴需道术，装备可提高内力恢复速度+点) NeedLevel=50(道术条件) Stock=3(每次可提高内力值)
        if (m_WAbil.SC2 >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sSCNot, c_Red, t_Hint);
        end;
      end;
    2:
      begin
        if m_WAbil.MC2 >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sMCNot, c_Red, t_Hint);
        end;
      end;
    3:
      begin
        if m_WAbil.SC2 >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sSCNot, c_Red, t_Hint);
        end;
      end;
    4:
      begin
        if m_btReLevel >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sReNewLevelNot, c_Red, t_Hint);
        end;
      end;
    40:
      begin
        if m_btReLevel >= LoWord(StdItem.NeedLevel) then
        begin
          if m_Abil.Level >= HiWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sLevelNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sReNewLevelNot, c_Red, t_Hint);
        end;
      end;
    41:
      begin
        if m_btReLevel >= LoWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.DC2 >= HiWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sDCNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sReNewLevelNot, c_Red, t_Hint);
        end;
      end;
    42:
      begin
        if m_btReLevel >= LoWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.MC2 >= HiWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sMCNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sReNewLevelNot, c_Red, t_Hint);
        end;
      end;
    43:
      begin
        if m_btReLevel >= LoWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.SC2 >= HiWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sSCNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sReNewLevelNot, c_Red, t_Hint);
        end;
      end;
    44:
      begin
        if m_btReLevel >= LoWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.CreditPoint >= HiWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sCreditPointNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sReNewLevelNot, c_Red, t_Hint);
        end;
      end;
    5:
      begin
        { TODO -ochongchong -c添加 : 声望按等级计算 【2013-08-12】 }
        if g_Config.boCreditPointWithLevel then
        begin
          if m_WAbil.Level >= StdItem.NeedLevel then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sLevelNot, c_Red, t_Hint);
          end;
        end
        else if m_WAbil.CreditPoint >= StdItem.NeedLevel then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sCreditPointNot, c_Red, t_Hint);
        end;
      end;
    50: // 等级+声望 chongchong 2013-12-26
      begin
        { TODO -ochongchong -c添加 : 声望按等级计算 【2013-08-12】 }
        if g_Config.boCreditPointWithLevel then
        begin
          if m_WAbil.Level >= HiWord(StdItem.NeedLevel) then
          begin
            if m_Abil.Level >= LoWord(StdItem.NeedLevel) then
            begin
              Result := True;
            end
            else
            begin
              SysMsg(g_sLevelNot, c_Red, t_Hint);
            end;
          end
          else
          begin
            SysMsg(g_sCreditPointNot, c_Red, t_Hint);
          end;
        end
        else if m_WAbil.CreditPoint >= HiWord(StdItem.NeedLevel) then
        begin
          if m_Abil.Level >= LoWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sLevelNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sCreditPointNot, c_Red, t_Hint);
        end;
      end;
    51: // 攻击力+声望 chongchong 2013-12-26
      begin
        { TODO -ochongchong -c添加 : 声望按等级计算 【2013-08-12】 }
        if g_Config.boCreditPointWithLevel then
        begin
          if m_WAbil.Level >= HiWord(StdItem.NeedLevel) then
          begin
            if m_WAbil.DC2 >= LoWord(StdItem.NeedLevel) then
            begin
              Result := True;
            end
            else
            begin
              SysMsg(g_sDCNot, c_Red, t_Hint);
            end;
          end
          else
          begin
            SysMsg(g_sCreditPointNot, c_Red, t_Hint);
          end;
        end
        else if m_WAbil.CreditPoint >= HiWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.DC2 >= LoWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sDCNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sCreditPointNot, c_Red, t_Hint);
        end;
      end;
    52: // 魔法力+声望 chongchong 2013-12-26
      begin
        { TODO -ochongchong -c添加 : 声望按等级计算 【2013-08-12】 }
        if g_Config.boCreditPointWithLevel then
        begin
          if m_WAbil.Level >= HiWord(StdItem.NeedLevel) then
          begin
            if m_WAbil.MC2 >= LoWord(StdItem.NeedLevel) then
            begin
              Result := True;
            end
            else
            begin
              SysMsg(g_sMCNot, c_Red, t_Hint);
            end;
          end
          else
          begin
            SysMsg(g_sCreditPointNot, c_Red, t_Hint);
          end;
        end
        else if m_WAbil.CreditPoint >= HiWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.MC2 >= LoWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sMCNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sCreditPointNot, c_Red, t_Hint);
        end;
      end;
    53: // 精神力+声望 chongchong 2013-12-26
      begin
        { TODO -ochongchong -c添加 : 声望按等级计算 【2013-08-12】 }
        if g_Config.boCreditPointWithLevel then
        begin
          if m_WAbil.Level >= HiWord(StdItem.NeedLevel) then
          begin
            if m_WAbil.SC2 >= LoWord(StdItem.NeedLevel) then
            begin
              Result := True;
            end
            else
            begin
              SysMsg(g_sSCNot, c_Red, t_Hint);
            end;
          end
          else
          begin
            SysMsg(g_sCreditPointNot, c_Red, t_Hint);
          end;
        end
        else if m_WAbil.CreditPoint >= HiWord(StdItem.NeedLevel) then
        begin
          if m_WAbil.SC2 >= LoWord(StdItem.NeedLevel) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sSCNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sCreditPointNot, c_Red, t_Hint);
        end;
      end;
    6:
      begin
        if (m_MyGuild <> nil) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sGuildNot, c_Red, t_Hint);
        end;
      end;
    60:
      begin
        if (m_MyGuild <> nil) and (m_nGuildRankNo = 1) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sGuildMasterNot, c_Red, t_Hint);
        end;
      end;
    7:
      begin
        // if (m_MyGuild <> nil) and (UserCastle.m_MasterGuild = m_MyGuild) then begin
        if (m_MyGuild <> nil) and (Castle <> nil) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sSabukHumanNot, c_Red, t_Hint);
        end;
      end;
    70:
      begin
        // if (m_MyGuild <> nil) and (UserCastle.m_MasterGuild = m_MyGuild) and (m_nGuildRankNo = 1) then begin
        if (m_MyGuild <> nil) and (Castle <> nil) and (m_nGuildRankNo = 1) then
        begin
          if m_Abil.Level >= StdItem.NeedLevel then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sLevelNot, c_Red, t_Hint);
          end;
        end
        else
        begin
          SysMsg(g_sSabukMasterManNot, c_Red, t_Hint);
        end;
      end;
    8:
      begin
        if (m_Master <> nil) and (TPlayObject(m_Master).m_nMemberType <> 0) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sMemberNot, c_Red, t_Hint);
        end;
      end;
    81:
      begin
        if (m_Master <> nil) and (TPlayObject(m_Master).m_nMemberType = LoWord(StdItem.NeedLevel)) and
          (TPlayObject(m_Master).m_nMemberLevel >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sMemberTypeNot, c_Red, t_Hint);
        end;
      end;
    82:
      begin
        if (m_Master <> nil) and (TPlayObject(m_Master).m_nMemberType >= LoWord(StdItem.NeedLevel)) and
          (TPlayObject(m_Master).m_nMemberLevel >= HiWord(StdItem.NeedLevel)) then
        begin
          Result := True;
        end
        else
        begin
          SysMsg(g_sMemberTypeNot, c_Red, t_Hint);
        end;
      end;
    101:
      Result := True;
    102:
      Result := True;
    103:
      Result := True;
    104:
      Result := True;
  end;
  // if not Result then SysMsg(g_sCanottWearIt,c_Red,t_Hint);
end;

function THeroObject.CheckItemBindUse(UserItem: pTUserItem): Boolean;
var
  I: Integer;
  ItemBind: pTItemBind;
begin
  Result := True;
  g_ItemBindAccount.Lock;
  try
    for I := 0 to g_ItemBindAccount.Count - 1 do
    begin
      ItemBind := g_ItemBindAccount.Items[I];
      if ItemBind <> nil then
      begin
        if (ItemBind.nMakeIdex = UserItem.MakeIndex) and (ItemBind.nItemIdx = UserItem.wIndex) then
        begin
          Result := False;
          if (CompareText(ItemBind.sBindName, m_sUserID) = 0) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sItemIsNotThisAccount, c_Red, t_Hint);
          end;
          Exit;
        end;
      end;
    end;
  finally
    g_ItemBindAccount.UnLock;
  end;

  if m_Master <> nil then
  begin
    g_ItemBindIPaddr.Lock;
    try
      for I := 0 to g_ItemBindIPaddr.Count - 1 do
      begin
        ItemBind := g_ItemBindIPaddr.Items[I];
        if ItemBind <> nil then
        begin
          if (ItemBind.nMakeIdex = UserItem.MakeIndex) and (ItemBind.nItemIdx = UserItem.wIndex) then
          begin
            Result := False;
            if (CompareText(ItemBind.sBindName, TPlayObject(m_Master).m_sIPaddr) = 0) then
            begin
              Result := True;
            end
            else
            begin
              SysMsg(g_sItemIsNotThisIPaddr, c_Red, t_Hint);
            end;
            Exit;
          end;
        end;
      end;
    finally
      g_ItemBindIPaddr.UnLock;
    end;
  end;

  g_ItemBindCharName.Lock;
  try
    for I := 0 to g_ItemBindCharName.Count - 1 do
    begin
      ItemBind := g_ItemBindCharName.Items[I];
      if ItemBind <> nil then
      begin
        if (ItemBind.nMakeIdex = UserItem.MakeIndex) and (ItemBind.nItemIdx = UserItem.wIndex) then
        begin
          Result := False;
          if (CompareText(ItemBind.sBindName, m_sCharName) = 0) then
          begin
            Result := True;
          end
          else
          begin
            SysMsg(g_sItemIsNotThisCharName, c_Red, t_Hint);
          end;
          Break;
        end;
      end;
    end;
  finally
    g_ItemBindCharName.UnLock;
  end;
end;

procedure THeroObject.ClientTakeOnItemsEx(btWhere: Byte; nItemIdx: Integer; sItemName: string);
var
  I, n14: Integer;
  UserItem, TakeOffItem: pTUserItem;
  StdItem, StdItem20: pTStdItem;
  StdItem58: TStdItem;
  sUserItemName, sTempItem: string;
  boMysteriousMan: Boolean; // 神秘人
  TakeOffItemEx: TUserItem;
begin
  StdItem := nil;
  UserItem := nil;
  n14 := -1;
  boMysteriousMan := m_boMysteriousMan; // 神秘人
  for I := 0 to m_ItemList.Count - 1 do
  begin
    UserItem := m_ItemList.Items[I];
    if (UserItem <> nil) and (UserItem.MakeIndex = nItemIdx) then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem <> nil then
      begin
        if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
          sUserItemName := UserItem.Name
        else
          sUserItemName := StdItem.Name;

        if CompareText(sUserItemName, sItemName) = 0 then
        begin
          n14 := I;
          Break;
        end;
      end;
    end;
    UserItem := nil;
  end;

  if (StdItem <> nil) and (UserItem <> nil) then
  begin
    if CheckUserItems(btWhere, StdItem) then
    begin
      StdItem58 := StdItem^;
      ItemUnit.GetItemAddValue(UserItem, StdItem58);
      if CheckTakeOnItems(btWhere, @StdItem58) { and TPlayObject(m_Master).CheckItemBindUse(UserItem) } then
      begin
        TakeOffItem := nil;
        if btWhere in [Low(THumanUseItems) .. High(THumanUseItems)] then
        begin

          if m_UseItems[btWhere].wIndex > 0 then
          begin
            StdItem20 := UserEngine.GetStdItem(m_UseItems[btWhere].wIndex);
            if (StdItem20 <> nil) and (StdItem20.StdMode in [15, 19, 20, 21, 22, 23, 24, 26]) then
            begin
              if (not m_boUserUnLockDurg) and (m_UseItems[btWhere].btValue[7] <> 0) then
              begin
                SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
                Exit;
              end;
            end;
            if not m_boUserUnLockDurg and ((StdItem20.Reserved and 2) <> 0) then
            begin
              SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
              Exit;
            end; // 004DAE78
            if (StdItem20.Reserved and 4) <> 0 then
            begin
              SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
              Exit;
            end;
            if InDisableTakeOffList(m_UseItems[btWhere].wIndex) or
              (GetUserItemBindValue(@m_UseItems[btWhere], ubNoTakeOff) and m_UseItems[btWhere].boIsBind) then
            begin
              SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
              Exit;
            end;

            if (m_UseItems[btWhere].btValue[13] = 1) and (m_UseItems[btWhere].Name <> '') then
              sUserItemName := m_UseItems[btWhere].Name
            else
              sUserItemName := UserEngine.GetStdItemName(m_UseItems[btWhere].wIndex);

            m_nCurrentItemMakeIndex := m_UseItems[btWhere].MakeIndex;
            m_nCurrentItemPos := btWhere;
            m_sCurrentItemName := UserEngine.GetStdItemName(m_UseItems[btWhere].wIndex);
            m_sCurrentItemNewName := sUserItemName;

            m_PickUpOrDropItem := m_UseItems[btWhere];
            m_boStopTakeOff := False;
            m_boStopTakeOn := False;
            g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroBeginTakeOff', False);

            m_PickUpOrDropItem.MakeIndex := 0;
            m_PickUpOrDropItem.wIndex := 0;

            m_nCurrentItemMakeIndex := 0;
            m_nCurrentItemPos := 0;
            m_sCurrentItemName := '';
            m_sCurrentItemNewName := '';
            if m_boStopTakeOff then
              Exit;

            if (StdItem <> nil) and (StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26]) and m_boUserUnLockDurg and
              (m_UseItems[btWhere].btValue[7] <> 0) then
            begin // 使用过神水的神秘装备下次可以脱下来
              m_boUserUnLockDurg := False;
              m_UseItems[btWhere].btValue[7] := 0;
            end;
            New(TakeOffItem);
            TakeOffItem^ := m_UseItems[btWhere];
          end; // 004DAEC7 if m_UseItems[btWhere].wIndex > 0 then begin

          if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
            sUserItemName := UserItem.Name
          else
            sUserItemName := UserEngine.GetStdItemName(UserItem.wIndex);

          m_nCurrentItemMakeIndex := UserItem.MakeIndex;
          m_nCurrentItemPos := btWhere;
          m_sCurrentItemName := UserEngine.GetStdItemName(UserItem.wIndex);
          m_sCurrentItemNewName := sUserItemName;

          m_PickUpOrDropItem := UserItem^;
          m_boStopTakeOn := False;
          g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroBeginTakeOn', False);

          m_PickUpOrDropItem.MakeIndex := 0;
          m_PickUpOrDropItem.wIndex := 0;

          m_nCurrentItemMakeIndex := 0;
          m_nCurrentItemPos := 0;
          m_sCurrentItemName := '';
          m_sCurrentItemNewName := '';
          if m_boStopTakeOn then
          begin
            if TakeOffItem <> nil then
              Dispose(TakeOffItem);
            Exit;
          end;
          if (UserItem.btValue[8] <> 0) and (StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26]) then
          begin
            UserItem.btValue[8] := 0;
          end;

          m_UseItems[btWhere] := UserItem^;
          DelBagItem(n14);

          if (StdItem58.Need in [101, 102]) and (not m_UseItems[btWhere].boStartTime) then
          begin
            m_UseItems[btWhere].nLimitTime := StdItem58.NeedLevel;
            m_UseItems[btWhere].boStartTime := True;
            SysMsg(Format('您的限时物品[%s]开始计时，有效时间%d分钟。', [StdItem58.Name, StdItem58.NeedLevel]), c_Red, t_System);
          end;

          if TakeOffItem <> nil then
          begin
            TakeOffItemEx := TakeOffItem^;

            if AddItemToBag(TakeOffItem) then
            begin
              SendAddItem(TakeOffItem);
            end
            else
            begin
              DropItemDown(TakeOffItem, 3, False, Self, nil);
              Dispose(TakeOffItem);
            end;

            if g_FunctionNPC <> nil then
            begin
              TPlayObject(m_Master).m_nScriptGotoCount := 0;
              m_PickUpOrDropItem := TakeOffItemEx;

              if (TakeOffItemEx.btValue[13] = 1) and (TakeOffItemEx.Name <> '') then
                sTempItem := TakeOffItemEx.Name
              else
                sTempItem := UserEngine.GetStdItemName(TakeOffItemEx.wIndex);
              m_nCurrentItemMakeIndex := TakeOffItemEx.MakeIndex;
              m_nCurrentItemPos := btWhere;
              m_sCurrentItemName := UserEngine.GetStdItemName(TakeOffItemEx.wIndex);
              m_sCurrentItemNewName := sTempItem;

              g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroTakeOff' + IntToStr(btWhere), False);

              m_PickUpOrDropItem.MakeIndex := 0;
              m_PickUpOrDropItem.wIndex := 0;

              m_nCurrentItemMakeIndex := 0;
              m_nCurrentItemPos := 0;
              m_sCurrentItemName := '';
              m_sCurrentItemNewName := '';
            end;
          end;

          RecalcAbilitys();
          SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
          SendMsg(Self, RM_SUBABILITY, 0, 0, 0, 0, '');
          SendDefMessage(SM_HEROTAKEONITEM, nItemIdx, btWhere, 0, 0, sItemName);
          FeatureChanged();
          if boMysteriousMan <> m_boMysteriousMan then // 神秘人
            RefShowName;

          if g_FunctionNPC <> nil then
          begin
            m_nCurrentItemMakeIndex := UserItem.MakeIndex;
            m_nCurrentItemPos := btWhere;
            m_sCurrentItemName := StdItem58.Name;
            m_sCurrentItemNewName := sUserItemName;

            g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroTakeOn' + IntToStr(btWhere), False);
            g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroTakeOnEx', False);

            m_nCurrentItemMakeIndex := 0;
            m_nCurrentItemPos := 0;
            m_sCurrentItemName := '';
            m_sCurrentItemNewName := '';
          end;
        end;
      end;
    end;
  end;
end;

procedure THeroObject.ClientTakeOffItemsEx(btWhere: Byte; nItemIdx: Integer; sItemName: string);
var
  StdItem: pTStdItem;
  UserItem: pTUserItem;
  sUserItemName: string;
  boMysteriousMan: Boolean; // 神秘人
begin
  boMysteriousMan := m_boMysteriousMan; // 神秘人
  if (btWhere in [Low(THumanUseItems) .. High(THumanUseItems)]) then
  begin
    if m_UseItems[btWhere].wIndex > 0 then
    begin
      if m_UseItems[btWhere].MakeIndex = nItemIdx then
      begin
        StdItem := UserEngine.GetStdItem(m_UseItems[btWhere].wIndex);
        if (StdItem <> nil) then
        begin
          if (StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26]) then
          begin
            if (not m_boUserUnLockDurg) and (m_UseItems[btWhere].btValue[7] <> 0) then
            begin
              SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
              Exit;
            end;
          end;
          if not m_boUserUnLockDurg and ((StdItem.Reserved and 2) <> 0) then
          begin
            SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
            Exit;
          end;
          if (StdItem.Reserved and 4) <> 0 then
          begin
            SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
            Exit;
          end;
          if InDisableTakeOffList(m_UseItems[btWhere].wIndex) or
            (GetUserItemBindValue(@m_UseItems[btWhere], ubNoTakeOff) and m_UseItems[btWhere].boIsBind) then
          begin
            SysMsg(g_sCanotTakeOffItem { '无法取下物品！' } , c_Red, t_Hint);
            Exit;
          end;

          if (m_UseItems[btWhere].btValue[13] = 1) and (m_UseItems[btWhere].Name <> '') then
            sUserItemName := m_UseItems[btWhere].Name
          else
            sUserItemName := UserEngine.GetStdItemName(m_UseItems[btWhere].wIndex);

          m_nCurrentItemMakeIndex := m_UseItems[btWhere].MakeIndex;
          m_nCurrentItemPos := btWhere;
          m_sCurrentItemName := UserEngine.GetStdItemName(m_UseItems[btWhere].wIndex);
          m_sCurrentItemNewName := sUserItemName;

          m_PickUpOrDropItem := m_UseItems[btWhere];
          m_boStopTakeOff := False;
          g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroBeginTakeOff', False);
          m_PickUpOrDropItem.MakeIndex := 0;
          m_PickUpOrDropItem.wIndex := 0;

          m_nCurrentItemMakeIndex := 0;
          m_nCurrentItemPos := 0;
          m_sCurrentItemName := '';
          m_sCurrentItemNewName := '';
          if m_boStopTakeOff then
            Exit;
          // 取自定义物品名称
          sUserItemName := '';
          if m_UseItems[btWhere].btValue[13] = 1 then
            sUserItemName := m_UseItems[btWhere].Name;
          if sUserItemName = '' then
            sUserItemName := UserEngine.GetStdItemName(m_UseItems[btWhere].wIndex);

          if CompareText(sUserItemName, sItemName) = 0 then
          begin
            if (StdItem <> nil) and (StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26]) and m_boUserUnLockDurg and
              (m_UseItems[btWhere].btValue[7] <> 0) then
            begin // 使用过神水的神秘装备下次可以脱下来
              m_boUserUnLockDurg := False;
              m_UseItems[btWhere].btValue[7] := 0;
            end;
            New(UserItem);
            UserItem^ := m_UseItems[btWhere];
            if AddItemToBag(UserItem) then
            begin
              m_UseItems[btWhere].wIndex := 0;
              SendAddItem(UserItem);
              RecalcAbilitys();
              SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
              SendMsg(Self, RM_SUBABILITY, 0, 0, 0, 0, '');
              SendDefMessage(SM_HEROTAKEOFFITEM, nItemIdx, btWhere, 0, 0, sItemName);
              FeatureChanged();

              if boMysteriousMan <> m_boMysteriousMan then // 神秘人
                RefShowName;

              if g_FunctionNPC <> nil then
              begin
                TPlayObject(m_Master).m_nScriptGotoCount := 0;
                m_PickUpOrDropItem := UserItem^;

                if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
                  m_sCurrentItemNewName := UserItem.Name
                else
                  m_sCurrentItemNewName := StdItem.Name;

                g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroTakeOff' + IntToStr(btWhere), False);
                m_PickUpOrDropItem.MakeIndex := 0;
                m_PickUpOrDropItem.wIndex := 0;
                m_sCurrentItemNewName := '';
              end;
            end
            else
            begin
              Dispose(UserItem);
            end;
          end;
        end;
      end;
    end;
  end;
end;

procedure THeroObject.DoQueryBagItems();
var
  I, nC: Integer;
  StdItem: pTStdItem;
  sSENDMSG: AnsiString;
  ClientItem: pTClientItem;
  UserItem: pTUserItem;
  InBuf: array [0 .. 102400] of AnsiChar;
  InBytes: Integer;
  Buffer: PAnsiChar;
begin
  sSENDMSG := '';

  InBytes := 0;
  nC := 0;

  for I := 0 to m_ItemList.Count - 1 do
  begin
    UserItem := m_ItemList.Items[I];
    if (UserItem = nil) then
      Continue;
    // sItemNewName:=GetItemName(UserItem.MakeIndex);
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if StdItem <> nil then
    begin
      Inc(nC);
      Inc(InBytes, SizeOf(TClientItem));
      Buffer := @InBuf;
      Buffer := Buffer + (InBytes - SizeOf(TClientItem));
      ClientItem := pTClientItem(Buffer);

      UserItemToClientItem(UserItem, StdItem, ClientItem, True, True);
    end;
  end;

  sSENDMSG := zLibCompressBuffer(@InBuf, InBytes);
  if sSENDMSG <> '' then
  begin
    m_DefMsg := MakeDefaultMsg(SM_HEROBAGITEMS, NativeInt(Self), Length(sSENDMSG), 0, nC);
    SendSocket(@m_DefMsg, sSENDMSG);
  end;
end;

function THeroObject.IsAttackTarget(BaseObject: TBaseObject): Boolean;

  function sub_4C88E4(): Boolean;
  begin
    Result := True;
  end;

  function CheckHumanLock: Boolean;
  begin
    Result := True;
    if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    begin
      Result := False;
      if BaseObject.InSafeZone or InSafeZone then
      begin
        Result := False;
        Exit
      end;

      if not((BaseObject.m_btRaceServer = RC_PLAYOBJECT) and BaseObject.m_boDummyObject) then
      begin
        // 对方锁定了自己或主人
        if (BaseObject.m_TargetCret <> nil) and ((BaseObject.m_TargetCret = Self) or (BaseObject.m_TargetCret = m_Master) or
          (BaseObject.m_TargetCret = Master)) then
          Result := True;

        // 对方锁定了自己的宝宝 或主人的宝宝
        if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_Master <> nil) and
          ((BaseObject.m_TargetCret.m_Master = Self) or (BaseObject.m_TargetCret.m_Master = m_Master) or
          (BaseObject.m_TargetCret.m_Master = Master)) then
          Result := True;
      end
      // 假人只有攻击到人物的时候才反击 chongchong 2013-11-11
      else
      begin
        // 对方攻击了自己或主人
        if (BaseObject.m_LastHiter <> nil) and ((BaseObject.m_LastHiter = Self) or (BaseObject.m_LastHiter = m_Master) or
          (BaseObject.m_LastHiter = Master)) then
          Result := True;

        // 对方攻击了自己的宝宝 或主人的宝宝
        if (BaseObject.m_LastHiter <> nil) and (BaseObject.m_LastHiter.m_Master <> nil) and
          ((BaseObject.m_LastHiter.m_Master = Self) or (BaseObject.m_LastHiter.m_Master = m_Master) or
          (BaseObject.m_LastHiter.m_Master = Master)) then
          Result := True;
      end;

      // 是主人或自己的攻击目标
      if { (m_LastHiter = BaseObject) or } ((m_Master <> nil) and ((m_Master.m_LastHiter = BaseObject) or
        (m_Master.m_TargetCret = BaseObject))) then
        Result := True;

      // 被锁定的目标 chongchong 2013-12-25
      if m_boTarget and (m_TargetCret = BaseObject) then
        Result := True;
    end
    // 人物或英雄的宝宝
    else if (BaseObject.m_Master <> nil) and (BaseObject.m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    begin
      Result := False;
      if BaseObject.InSafeZone or InSafeZone then
      begin
        Result := False;
        Exit
      end;

      // 对方锁定了自己或主人
      if (BaseObject.m_TargetCret <> nil) and ((BaseObject.m_TargetCret = Self) or (BaseObject.m_TargetCret = m_Master) or
        (BaseObject.m_TargetCret = Master)) then
        Result := True;

      // 对方的主人锁定了自己或主人
      if (BaseObject.m_Master.m_TargetCret <> nil) and
        ((BaseObject.m_Master.m_TargetCret = Self) or (BaseObject.m_Master.m_TargetCret = m_Master) or
        (BaseObject.m_Master.m_TargetCret = Master)) then
        Result := True;

      // 对方锁定了自己的宝宝 或主人的宝宝
      if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_Master <> nil) and
        ((BaseObject.m_TargetCret.m_Master = Self) or (BaseObject.m_TargetCret.m_Master = m_Master) or
        (BaseObject.m_TargetCret.m_Master = Master)) then
        Result := True;

      // 对方的主人锁定了自己的宝宝 或主人的宝宝
      if (BaseObject.m_Master.m_TargetCret <> nil) and (BaseObject.m_Master.m_TargetCret.m_Master <> nil) and
        ((BaseObject.m_Master.m_TargetCret.m_Master = Self) or (BaseObject.m_Master.m_TargetCret.m_Master = m_Master) or
        (BaseObject.m_Master.m_TargetCret.m_Master = Master)) then
        Result := True;

      // 是主人或自己的攻击目标
      if { (m_LastHiter = BaseObject) or } ((m_Master <> nil) and (m_Master.m_LastHiter = BaseObject)) then
        Result := True;

      // 被锁定的目标 chongchong 2013-12-25
      if m_boTarget and (m_TargetCret = BaseObject) then
        Result := True;
    end;
  end;

// 只有人物被怪攻击 英雄被怪攻击 或者人物攻击怪物 锁定怪物 英雄才会去攻击对应怪物 chongchong 2013-12-25
  function CheckLockMyFriend: Boolean;
  begin
    Result := False;
    // 对方锁定了自己或主人
    if (BaseObject.m_TargetCret <> nil) and ((BaseObject.m_TargetCret = Self) or (BaseObject.m_TargetCret = m_Master) or
      (BaseObject.m_TargetCret = Master)) then
      Result := True;

    // 对方锁定了自己的宝宝 或主人的宝宝
    if (BaseObject.m_TargetCret <> nil) and (BaseObject.m_TargetCret.m_Master <> nil) and
      ((BaseObject.m_TargetCret.m_Master = Self) or (BaseObject.m_TargetCret.m_Master = m_Master) or
      (BaseObject.m_TargetCret.m_Master = Master)) then
      Result := True;

    // 对方攻击了自己或主人
    if (BaseObject.m_LastHiter <> nil) and ((BaseObject.m_LastHiter = Self) or (BaseObject.m_LastHiter = m_Master) or
      (BaseObject.m_LastHiter = Master)) then
      Result := True;

    // 对方攻击了自己的宝宝 或主人的宝宝
    if (BaseObject.m_LastHiter <> nil) and (BaseObject.m_LastHiter.m_Master <> nil) and
      ((BaseObject.m_LastHiter.m_Master = Self) or (BaseObject.m_LastHiter.m_Master = m_Master) or
      (BaseObject.m_LastHiter.m_Master = Master)) then
      Result := True;

    // 被锁定的目标 chongchong 2013-12-25
    if m_boTarget and (m_TargetCret = BaseObject) then
      Result := True;

    if m_boDummyObject and (m_Master <> nil) and (m_Master.m_TargetCret = BaseObject) then
      Result := True;
  end;

var
  MyGuild: TGUild;
  nErrCode: Integer;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  Result := False;
  nErrCode := 0;
  try
    nErrCode := 1;

    if (BaseObject = nil) or (m_Master = nil) or (BaseObject = Self) or (BaseObject = m_Master) or (BaseObject = Master) or
      (m_btAttackMode = 2) then
      Exit;

    nErrCode := 2;

    if (BaseObject.m_btRaceServer = 108) or { (BaseObject.m_btRaceServer = 55) or }
      (BaseObject.m_btRaceServer = RC_GUARD) // or // or (BaseObject.m_btRaceServer = RC_ARCHERGUARD) or
    { (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) } then
      Exit; // 魔王岭怪物 练功师 大刀 弓箭手 押镖车

    if ((BaseObject.m_btRaceServer = 154) and (BaseObject.m_btRaceImg = 156) { 自定义怪物 - 魔王岭怪物 } ) then
      Exit;

    nErrCode := 3;

    // 管理员模式不打
    if (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode then
      Exit;

    // 禁止英雄攻击怪物 2019-07-18 20:37:03
    if g_Config.boDisableHeroAttackMonster then
    begin
      IsHuman := BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT { , RC_PLAYMOSTER } ];
      if not IsHuman then
      begin
        _Master := BaseObject.Master;
        if (_Master <> nil) then
        begin
          IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT { , RC_PLAYMOSTER } ];
        end;
      end;

      if not IsHuman then
        Exit;
    end;

    nErrCode := 4;

    if (BaseObject.m_btRaceServer in [RC_GUARD, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD { , 111 沙巴克城墙 } ]) then
    begin
      nErrCode := 5;
      // 对方锁定了自己或主人
      if (BaseObject.m_TargetCret <> nil) and ((BaseObject.m_TargetCret = Self) or (BaseObject.m_TargetCret.Master = m_Master))
      then
        Result := True;

      nErrCode := 6;
      // 别人攻击我或主人
      if (m_LastHiter = BaseObject) or ((m_Master <> nil) and (m_Master.m_LastHiter = BaseObject)) then
        Result := True;

      nErrCode := 7;

      { 全职英雄攻击范围 }
      if Result and (not CheckInMasterRange(BaseObject.m_nCurrX, BaseObject.m_nCurrY)) then
        Result := False;

      nErrCode := 8;
      if (not Result) and m_boTarget and (m_TargetCret = BaseObject) then
        Result := True;

      nErrCode := 9;
      Exit;
    end;

    nErrCode := 10;
    if (BaseObject.m_PEnvir <> m_PEnvir) then
      Exit;

    // 假人和人物一样，去掉假人的单独检测  chongchong 2013-11-24
    {
      if m_boDummyObject and (BaseObject.m_TargetCret = Self) and (m_btAttatckMode <> HAM_PEACE) then
      Result := True;
    }
    nErrCode := 11;
    case m_btAttatckMode of
      HAM_ALL { 0 } :
        begin
          nErrCode := 12;
          if (BaseObject.m_btRaceServer < RC_NPC { 10 } ) or (BaseObject.m_btRaceServer > RC_PEACENPC { 15 } ) then
            Result := CheckLockMyFriend; // True;

          nErrCode := 13;
          if Result then
            Result := CheckHumanLock;

          nErrCode := 14;
          if g_Config.boNonPKServer then
            Result := sub_4C88E4();
        end;
      HAM_PEACE { 1 } :
        begin
          nErrCode := 15;
          if (BaseObject.m_btRaceServer >= RC_ANIMAL) then
          begin
            nErrCode := 16;
            Result := CheckLockMyFriend;

            nErrCode := 17;
            if Result then
              Result := CheckHumanLock;

            nErrCode := 18;
            if Result then
            begin
              if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) then
                Result := False;
            end;
          end;
        end;
      HAM_DEAR { 2 } :
        begin
          nErrCode := 19;

          if (BaseObject.m_btRaceServer < RC_NPC { 10 } ) or (BaseObject.m_btRaceServer > RC_PEACENPC { 15 } ) then
          begin
            Result := CheckLockMyFriend; // True;

            nErrCode := 20;
            if Result then
              Result := CheckHumanLock;
          end;

          nErrCode := 21;
          if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
          begin
            nErrCode := 22;
            // 爱人不攻击
            if BaseObject = TPlayObject(m_Master).m_DearHuman then
              Result := False
              // 爱人的宝宝不攻击
            else if (BaseObject.Master <> nil) and (BaseObject.Master = TPlayObject(m_Master).m_DearHuman) then
              Result := False;
          end;
        end;
      HAM_MASTER { 3 } :
        begin
          nErrCode := 23;

          if (BaseObject.m_btRaceServer < RC_NPC { 10 } ) or (BaseObject.m_btRaceServer > RC_PEACENPC { 15 } ) then
            Result := CheckLockMyFriend;

          if Result then
            Result := CheckHumanLock;

          nErrCode := 24;

          if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
          begin
            nErrCode := 25;

            // 师傅不攻击
            if (BaseObject = TPlayObject(m_Master).m_MasterHuman) then
              Result := False
              // 师傅的宝宝不攻击
            else if (BaseObject.Master <> nil) and (BaseObject.Master = TPlayObject(m_Master).m_MasterHuman) then
              Result := False
              // 徒弟不攻击
            else if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MasterHuman = m_Master) then
              Result := False
              // 徒弟的宝宝不攻击
            else if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) and
              (TPlayObject(BaseObject.Master).m_MasterHuman = m_Master) then
              Result := False;
          end;
        end;
      HAM_GROUP { 4 } :
        begin
          nErrCode := 26;
          if (BaseObject.m_btRaceServer < RC_NPC) or (BaseObject.m_btRaceServer > RC_PEACENPC) then
            Result := CheckLockMyFriend; // True;

          nErrCode := 27;
          if Result then
            Result := CheckHumanLock;

          nErrCode := 28;

          if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
          begin
            // 本组的人不攻击
            if TPlayObject(m_Master).IsGroupMember(TPlayObject(BaseObject)) then
              Result := False
              // 本组的人的宝宝不攻击
            else if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Master)
              .IsGroupMember(TPlayObject(BaseObject.Master)) then
              Result := False;
          end;

          nErrCode := 29;
          if g_Config.boNonPKServer then
            Result := sub_4C88E4();
        end;
      HAM_GUILD { 5 } :
        begin
          nErrCode := 30;
          if (BaseObject.m_btRaceServer < RC_NPC) or (BaseObject.m_btRaceServer > RC_PEACENPC) then
            Result := CheckLockMyFriend; // True;

          nErrCode := 31;
          if Result then
            Result := CheckHumanLock;

          nErrCode := 32;
          MyGuild := TGUild(m_MyGuild);
          if MyGuild = nil then
          begin
            if m_Master <> nil then
              MyGuild := TGUild(m_Master.m_MyGuild);
          end;

          nErrCode := 33;
          if MyGuild <> nil then
          begin
            nErrCode := 34;
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            begin
              nErrCode := 35;
              if MyGuild.IsMember(BaseObject.m_sCharName) then
                Result := False;

              nErrCode := 36;
              if m_boGuildWarArea and (BaseObject.m_MyGuild <> nil) then
              begin
                if MyGuild.IsAllyGuild(TGUild(BaseObject.m_MyGuild)) then
                  Result := False;
              end;
            end
            else if (BaseObject.Master <> nil) then
            begin
              nErrCode := 37;

              if MyGuild.IsMember(BaseObject.Master.m_sCharName) then
                Result := False;
              if m_boGuildWarArea and (BaseObject.Master.m_MyGuild <> nil) then
              begin
                nErrCode := 38;
                if MyGuild.IsAllyGuild(TGUild(BaseObject.Master.m_MyGuild)) then
                  Result := False;
              end;
            end;
          end;

          nErrCode := 39;
          if g_Config.boNonPKServer then
            Result := sub_4C88E4();
        end;
      HAM_PKATTACK { 6 } :
        begin
          // 假人和人物一样，去掉假人的单独检测  chongchong 2013-11-24
          {
            if m_boDummyObject then
            begin
            if BaseObject.m_Master <> nil then
            begin
            if not (BaseObject.Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
            begin
            Result := True;
            end
            else
            begin
            if (not BaseObject.InSafeZone) and ((BaseObject.m_TargetCret = Self) or
            (BaseObject.m_Master.m_TargetCret = Self) or
            (BaseObject.Master.m_TargetCret = Self)) then
            Result := True;
            end;
            end
            else
            begin
            Result := CheckHumanLock;
            if (BaseObject.m_btRaceServer >= RC_ANIMAL) or (BaseObject.m_TargetCret = Self) then
            Result := True;
            end;
            end
            else
          }
          begin
            nErrCode := 40;
            if (BaseObject.m_btRaceServer < RC_NPC) or (BaseObject.m_btRaceServer > RC_PEACENPC) then
              Result := CheckLockMyFriend; // True;

            nErrCode := 41;
            if Result then
              Result := CheckHumanLock;

            nErrCode := 42;
            if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
            begin
              if Result and (TSmartObject(BaseObject).PKLevel >= 2) then
                Result := True
              else
                Result := False;
            end;
          end;

          nErrCode := 43;
          if g_Config.boNonPKServer then
            Result := sub_4C88E4();
        end;
      HAM_NATION:
        begin // 国家攻击模式

          nErrCode := 44;
          if (BaseObject.m_btRaceServer < RC_NPC) or (BaseObject.m_btRaceServer > RC_PEACENPC) then
          begin
            // 不主动攻击反击型怪物 chongchong 2014-07-27
            // Result := True;
            Result := CheckLockMyFriend;
          end;

          nErrCode := 45;
          if Result then
            Result := CheckHumanLock;

          nErrCode := 46;
          // 国家怪物
          if (m_Master <> nil) and (m_Master.m_btNation > 0) and (BaseObject.m_btNation > 0) and
            (m_Master.m_btNation = BaseObject.m_btNation) and (BaseObject.m_btRaceServer >= RC_ANIMAL) then
          begin
            if not BaseObject.m_boCanAttackSameNationPlayer then
            begin
              Result := False;
            end;
          end;

          nErrCode := 47;
          if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(m_Master).m_btNation > 0) then
          begin
            nErrCode := 48;
            if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and
              (TPlayObject(BaseObject).m_btNation = TPlayObject(m_Master).m_btNation) then
              Result := False
            else if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) and
              (TPlayObject(BaseObject.Master).m_btNation = TPlayObject(m_Master).m_btNation) then
              Result := False;
          end;

          nErrCode := 49;
          if g_Config.boNonPKServer then
            Result := sub_4C88E4();
        end;
    end;

    nErrCode := 50;
    if (BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode then
      Result := False;

    if (m_Master <> nil) and (m_Master.m_btNation > 0) and (BaseObject.m_btNation > 0) and
      (m_Master.m_btNation = BaseObject.m_btNation) and (BaseObject.m_btRaceServer >= RC_ANIMAL) and
      (not BaseObject.m_boAllowSameNationPlayerAttack) then
    begin
      Result := False;
    end;

    nErrCode := 51;
    { 全职英雄攻击范围 }
    if Result and (not CheckInMasterRange(BaseObject.m_nCurrX, BaseObject.m_nCurrY)) and (not m_boProtectStatus) then
      Result := False;

    nErrCode := 52;
    BreakCrazyMode();

  except
    MainOutMessage('THeroObject.IsAttackTarget Error; ErrorCode = ' + IntToStr(nErrCode));
  end;
end;

function THeroObject._Attack(var wHitMode: Word; AttackTarget: TBaseObject; AttackRate: Single = 1.0;
  NpcReleaseagic: pTUserMagic = nil): Boolean;
var
  _Index: Integer;
  MonHPProgress: pTMonHPProgress;
begin
  // 增加英雄攻击目标显示大血条 2019-08-26 14:30:47
  Result := inherited _Attack(wHitMode, AttackTarget, AttackRate, NpcReleaseagic);
  if Result then
  begin
    if (AttackTarget <> nil) and (AttackTarget.m_btRaceServer >= RC_ANIMAL) then
    begin
      _Index := g_MonHPProgressList.IndexOf(AttackTarget.m_sCharName);
      if _Index >= 0 then
      begin
        MonHPProgress := pTMonHPProgress(g_MonHPProgressList.Objects[_Index]);

        if (AttackTarget.m_ExpHitter <> nil) then
        begin
          if AttackTarget.m_ExpHitter.Master <> nil then
            MonHPProgress.m_ExpHinterName := AttackTarget.m_ExpHitter.Master.m_sCharName
          else
            MonHPProgress.m_ExpHinterName := AttackTarget.m_ExpHitter.m_sCharName;
        end
        else
          MonHPProgress.m_ExpHinterName := '';

        m_DefMsg := MakeDefaultMsg(SM_SENDBIGHPPROGRESS, NativeInt(AttackTarget), 0, 0, 0);
        SendSocketEx(@m_DefMsg, PAnsiChar(MonHPProgress), SizeOf(TMonHPProgress));
      end;
    end;
  end;
end;

function THeroObject.DoSpell(UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; BaseObject: TBaseObject;
  FormClient: Boolean): Boolean;
var
  _Index: Integer;
  MonHPProgress: pTMonHPProgress;
  CustomMagicConfig: TCustomMagicConfig;
  boAttack: Boolean;
begin
  // 增加英雄攻击目标显示大血条 2019-08-26 14:30:47
  Result := inherited DoSpell(UserMagic, nTargetX, nTargetY, BaseObject, FormClient);

  if Result and (BaseObject <> nil) and (BaseObject.m_btRaceServer >= RC_ANIMAL) and (UserMagic <> nil) then
  begin
    boAttack := True;
    if CheckIsCustomMagic(UserMagic.wMagIdx) then
    begin
      CustomMagicConfig := GetCustomMagicConfig(UserMagic.wMagIdx);
      if CustomMagicConfig.ServerConfig.OperateMode = momProtect then
        boAttack := False;
    end
    else if UserMagic.wMagIdx in [2 { 治愈术 } , 29 { 群体治愈术 } , SKILL_UNAMYOUNSUL { 解毒 } , SKILL_49 { 净化术 } , SKILL_75 { 护体神盾 } ,
      SKILL_SHIELD { 魔法盾 } , SKILL_50 { 无极真气 } , SKILL_SKELLETON { 召唤骷髅 } , SKILL_SINSU { 召唤神兽 } , SKILL_76 { 召唤圣兽 } , SKILL_55
    { 召唤月灵 } , SKILL_CLOAK { 隐身术 } , SKILL_BIGCLOAK { 集体隐身术 } , SKILL_73 { 道力盾 } , SKILL_87 { 武力盾 } , SKILL_88 { 新武力盾 } , SKILL_89
    { 新道力盾] } ] then
    begin
      boAttack := False;
    end;

    if boAttack then
    begin
      _Index := g_MonHPProgressList.IndexOf(BaseObject.m_sCharName);
      if _Index >= 0 then
      begin
        MonHPProgress := pTMonHPProgress(g_MonHPProgressList.Objects[_Index]);

        if (BaseObject.m_ExpHitter <> nil) then
        begin
          if BaseObject.m_ExpHitter.Master <> nil then
            MonHPProgress.m_ExpHinterName := BaseObject.m_ExpHitter.Master.m_sCharName
          else
            MonHPProgress.m_ExpHinterName := BaseObject.m_ExpHitter.m_sCharName;
        end
        else
          MonHPProgress.m_ExpHinterName := '';

        m_DefMsg := MakeDefaultMsg(SM_SENDBIGHPPROGRESS, NativeInt(BaseObject), 0, 0, 0);
        SendSocketEx(@m_DefMsg, PAnsiChar(MonHPProgress), SizeOf(TMonHPProgress));
      end;
    end;
  end;
end;
(*
  function THeroObject.IsProperTarget(BaseObject: TBaseObject): Boolean;
  begin
  Result := IsAttackTarget(BaseObject);
  (*if Result then begin
  if BaseObject.m_Master <> nil then begin
  Result := IsAttackTarget(BaseObject.Master);
  if InSafeZone or BaseObject.InSafeZone then Result := False; {检测是否是在安全区}
  end;

  if Result and (BaseObject <> nil) and (BaseObject.m_btRaceServer = 55) then begin
  Result := False;
  end;
  end;
  end;
*)

function THeroObject.IsProperTarget(BaseObject: TBaseObject): Boolean;
var
  MasterObj: TBaseObject;
begin
  Result := False;
  try
    if (BaseObject = nil) or (BaseObject = Self) then
      Exit;

    // PK时，主人不在安全区，英雄在安全区，目标不在安全区，不让打目标 chongchong 2017-12-11
    MasterObj := BaseObject.Master;
    if MasterObj = nil then
    begin
      MasterObj := BaseObject;
    end;
    if ((MasterObj <> nil) and (MasterObj.m_btRaceServer = RC_PLAYOBJECT)) and (InSafeZone) then
    begin
      Exit;
    end;

    // 当队友在吸引火力的时候，英雄用群攻攻击一群怪物，只打到一个 chongchong 2015-09-13
    if (BaseObject.m_btRaceServer >= RC_ANIMAL) and (BaseObject.m_Master = nil) then
    begin
      // 不可被同国家人物攻击的怪物，不让攻击  chongchong 2017-07-29
      if (m_Master <> nil) and (m_Master.m_btNation > 0) and (BaseObject.m_btNation > 0) and
        (m_Master.m_btNation = BaseObject.m_btNation) and (not BaseObject.m_boAllowSameNationPlayerAttack) then
      begin
        Result := False;
      end
      else
      begin
        // 石化怪物不攻击 chongchong 2017-11-24
        if not BaseObject.m_boStoneMode then
          Result := True;
      end;
    end
    else
      Result := IsAttackTarget(BaseObject);

    // chongchong 2014-12-29
    // 主人打别人，别人不还击，然后把英雄召唤出来打别人（别人也不还击），
    // 这时候就出现英雄打别人不掉血，要用CTRL+W锁定打才掉血。
    // 也就是只要对方不还击，英雄怎么打都不掉血
    if (not Result) and (m_Master <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
      Result := m_Master.IsAttackTarget(BaseObject);

    // 修正上线找几个人和英雄，站一堆，全体攻击用群攻技能攻击这一堆，英雄不受伤害 chongchong 2017-10-28
    if (not Result) and (m_Master <> nil) and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
      Result := m_Master.IsAttackTarget(BaseObject);

    // 修复英雄砍npc时可以攻击到安全区内的人(开天斩) chongchong 2015-04-02
    if Result then
    begin
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
      begin
        if BaseObject.InSafeZone then
          Result := False;

        if (m_btAttatckMode = HAM_PEACE) then
          Result := False;
      end;
    end;
  except
    MainOutMessage('[Exception] THeroObject:IsProperTarget');
  end;
end;

// 修复英雄 行会模式和红名模式 道士英雄 使用技能（神圣战甲术）（幽灵盾）失败 chongchong 2013-11-17

function THeroObject.IsProperFriend(BaseObject: TBaseObject): Boolean;
begin
  Result := not IsAttackTarget(BaseObject);
end;

{ TODO -ochongchong -c增加 : 根据攻击模式来判断是否可以锁定对象 【2013-08-25】 }

function THeroObject.IsLcokAttackTarget(BaseObject: TBaseObject): Boolean;
var
  MyGuild: TGUild;
begin
  Result := False;

  // 不能锁定主人
  if (BaseObject = nil) or (m_Master = nil) or (BaseObject = Self) or (BaseObject = m_Master) or (BaseObject = Master) then
    Exit;

  // 不能锁定自己或主人的宝宝 chongchong 2016-03-20
  if BaseObject.Master = Master then
    Exit;

  // 系统NPC不锁定
  if (BaseObject.m_btRaceServer = 108) { or (BaseObject.m_btRaceServer = 55) } or (BaseObject.m_btRaceServer = RC_GUARD)
  // or // or (BaseObject.m_btRaceServer = RC_ARCHERGUARD) or
  { (BaseObject.m_btRaceServer = RC_TRUCKOBJECT) } then
    Exit; // 魔王岭怪物 练功师 大刀 弓箭手 押镖车

  if ((BaseObject.m_btRaceServer = 154) and (BaseObject.m_btRaceImg = 156) { 自定义怪物 - 魔王岭怪物 } ) then
    Exit;

  // 不在同一地图不锁定
  if (BaseObject.m_PEnvir <> m_PEnvir) then
    Exit;

  // 人物或英雄在安全区不锁定
  if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and (BaseObject.InSafeZone) then
    Exit; // 安全区不能锁定

  if m_boDummyObject and (BaseObject.m_TargetCret = Self) and (m_btAttatckMode <> HAM_PEACE) then
    Result := True;

  // 不让锁定目标在石化状态，就是没开壳的那种祖玛雕像那种 chongchong 2013-12-25
  if ((BaseObject.m_boAdminMode or BaseObject.m_boTempAdminMode) or BaseObject.m_boStoneMode) then
  begin
    Result := False;
    Exit;
  end;

  case m_btAttatckMode of
    HAM_ALL { 0 } :
      begin
        Result := True;
      end;
    HAM_PEACE { 1 } :
      begin
        if BaseObject.m_btRaceServer >= RC_ANIMAL then
          Result := True;
      end;
    HAM_DEAR { 2 } :
      begin
        Result := True;
        if (m_Master <> nil) and (BaseObject = TPlayObject(m_Master).m_DearHuman) then
          Result := False
        else if (m_Master <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_Master = TPlayObject(m_Master).m_DearHuman)
        then
          Result := False;
      end;
    HAM_MASTER { 3 } :
      begin
        Result := True;
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        begin
          if (BaseObject.Master = Self) or (BaseObject.m_Master = Self) then
            Result := False
          else if (m_Master <> nil) and ((BaseObject = m_Master) or (BaseObject.m_Master = m_Master) or
            (BaseObject.Master = m_Master)) then
            Result := False;
        end;
      end;
    HAM_GROUP { 4 } :
      begin
        Result := True;

        if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (m_Master <> nil) then
        begin
          if TPlayObject(m_Master).IsGroupMember(TPlayObject(BaseObject)) then
            Result := False;
        end;

        if (BaseObject.m_btRaceServer = RC_HEROOBJECT) and (m_Master <> nil) and (BaseObject.m_Master <> nil) then
        begin
          if TPlayObject(m_Master).IsGroupMember(TPlayObject(BaseObject.m_Master)) then
            Result := False;
        end;
      end;
    HAM_GUILD { 5 } :
      begin
        Result := True;

        MyGuild := TGUild(m_MyGuild);
        if MyGuild = nil then
        begin
          if m_Master <> nil then
            MyGuild := TGUild(m_Master.m_MyGuild);
        end;

        if MyGuild <> nil then
        begin
          // 判断人物所在行会
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          begin
            if MyGuild.IsMember(BaseObject.m_sCharName) then
              Result := False;
            if m_boGuildWarArea and (BaseObject.m_MyGuild <> nil) then
            begin
              if MyGuild.IsAllyGuild(TGUild(BaseObject.m_MyGuild)) then
                Result := False;
            end;
          end
          // 判断英雄主要所在行会
          else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) and (BaseObject.Master <> nil) then
          begin
            if MyGuild.IsMember(BaseObject.Master.m_sCharName) then
              Result := False;
            if m_boGuildWarArea and (BaseObject.Master.m_MyGuild <> nil) then
            begin
              if MyGuild.IsAllyGuild(TGUild(BaseObject.Master.m_MyGuild)) then
                Result := False;
            end;
          end;
        end;
      end;
    HAM_PKATTACK { 6 } :
      begin
        Result := True;
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
        begin
          if Result and (TSmartObject(BaseObject).PKLevel >= 2) then
            Result := True
          else
            Result := False;
        end;
      end;
    HAM_NATION:
      begin // 国家攻击模式
        Result := True;

        // 国家怪物
        if (m_btNation > 0) and (BaseObject.m_btNation > 0) and (m_btNation = BaseObject.m_btNation) and
          (BaseObject.m_btRaceServer >= RC_ANIMAL) then
        begin
          if not BaseObject.m_boCanAttackSameNationPlayer then
          begin
            Result := False;
          end;
        end;

        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          if (TPlayObject(Self).m_btNation = TPlayObject(BaseObject).m_btNation) and (TPlayObject(Self).m_btNation > 0) then
            Result := False;
      end;
  end;
end;

{
  function THeroObject.GetMyStatus: Integer;
  begin
  Result := m_nHungerStatus div 1000;
  if Result > 4 then Result := 4;
  end;
}

// 使用祝福油
function THeroObject.WeaptonMakeLuck: Boolean;
var
  StdItem: pTStdItem;
  nRand: Integer;
  boMakeLuck: Boolean;
begin
  Result := False;
  if m_UseItems[U_WEAPON].wIndex <= 0 then
    Exit;
  nRand := 0;
  StdItem := UserEngine.GetStdItem(m_UseItems[U_WEAPON].wIndex);
  if StdItem <> nil then
  begin
    nRand := abs(StdItem.DC2 - StdItem.DC1) div 5;
  end;
  if Random(g_Config.nWeaponMakeUnLuckRate { 20 } ) = 1 then
  begin
    // 不调用这个函数，这个函数判断了 英雄杀人不诅咒武器
    // 在此处，加诅咒不是杀人，而是祝福油
    // MakeWeaponUnlock();

    if m_UseItems[U_WEAPON].btValue[3] > 0 then
    begin
      Dec(m_UseItems[U_WEAPON].btValue[3]);
      SysMsg(g_sTheWeaponIsCursed, c_Red, t_Hint);
    end
    else
    begin
      if m_UseItems[U_WEAPON].btValue[4] < 10 then
      begin
        Inc(m_UseItems[U_WEAPON].btValue[4]);
        SysMsg(g_sTheWeaponIsCursed, c_Red, t_Hint);
      end;
    end;

    RecalcAbilitys();
    // SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
    // SendMsg(Self, RM_SUBABILITY, 0, 0, 0, 0, '');
  end
  else
  begin
    boMakeLuck := False;
    if m_UseItems[U_WEAPON].btValue[4] > 0 then
    begin
      Dec(m_UseItems[U_WEAPON].btValue[4]);
      SysMsg(g_sWeaptonMakeLuck { '武器被加幸运了.' } , c_Green, t_Hint);
      boMakeLuck := True;
    end
    else if m_UseItems[U_WEAPON].btValue[3] < g_Config.nWeaponMakeLuckPoint1 { 1 } then
    begin
      Inc(m_UseItems[U_WEAPON].btValue[3]);
      SysMsg(g_sWeaptonMakeLuck { '武器被加幸运了.' } , c_Green, t_Hint);
      boMakeLuck := True;
    end
    else if (m_UseItems[U_WEAPON].btValue[3] < g_Config.nWeaponMakeLuckPoint2 { 3 } ) and
      (Random(nRand + g_Config.nWeaponMakeLuckPoint2Rate
      { 6 } ) = 1) then
    begin
      Inc(m_UseItems[U_WEAPON].btValue[3]);
      SysMsg(g_sWeaptonMakeLuck { '武器被加幸运了.' } , c_Green, t_Hint);
      boMakeLuck := True;
    end
    else if (m_UseItems[U_WEAPON].btValue[3] < g_Config.nWeaponMakeLuckPoint3 { 7 } ) and
      (Random(nRand * g_Config.nWeaponMakeLuckPoint3Rate
      { 10 + 30 } ) = 1) then
    begin
      Inc(m_UseItems[U_WEAPON].btValue[3]);
      SysMsg(g_sWeaptonMakeLuck { '武器被加幸运了.' } , c_Green, t_Hint);
      boMakeLuck := True;
    end;

    if boMakeLuck then
    begin
      RecalcAbilitys();
      // SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
      // SendMsg(Self, RM_SUBABILITY, 0, 0, 0, 0, '');
    end;

    if not boMakeLuck then
      SysMsg(g_sWeaptonNotMakeLuck { '无效' } , c_Green, t_Hint);
  end;
  Result := True;
end;

function THeroObject.RepairWeapon: Boolean;
var
  nDura: Integer;
  UserItem: pTUserItem;
  IsZeroDura: Boolean;
begin
  Result := False;
  UserItem := @m_UseItems[U_WEAPON];
  if (UserItem.wIndex <= 0) or (UserItem.DuraMax <= UserItem.Dura) then
    Exit;
  Dec(UserItem.DuraMax, (UserItem.DuraMax - UserItem.Dura) div g_Config.nRepairItemDecDura { 30 } );
  nDura := _MIN(5000, UserItem.DuraMax - UserItem.Dura);
  if nDura > 0 then
  begin
    IsZeroDura := UserItem.Dura <= 0;

    Inc(UserItem.Dura, nDura);
    SendMsg(Self, RM_DURACHANGE, 1, UserItem.Dura, UserItem.DuraMax, 0, '');
    SysMsg(g_sWeaponRepairSuccess { '武器修复成功.' } , c_Green, t_Hint);
    Result := True;

    if IsZeroDura then
    begin
      RecalcAbilitys;
      SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
    end;
  end;
end;

function THeroObject.SuperRepairWeapon: Boolean;
var
  IsZeroDura: Boolean;
begin
  Result := False;
  if m_UseItems[U_WEAPON].wIndex <= 0 then
    Exit;
  IsZeroDura := m_UseItems[U_WEAPON].Dura <= 0;
  m_UseItems[U_WEAPON].Dura := m_UseItems[U_WEAPON].DuraMax;
  SendMsg(Self, RM_DURACHANGE, 1, m_UseItems[U_WEAPON].Dura, m_UseItems[U_WEAPON].DuraMax, 0, '');
  SysMsg(g_sWeaponRepairSuccess { '武器修复成功.' } , c_Green, t_Hint);
  Result := True;

  if IsZeroDura then
  begin
    RecalcAbilitys;
    SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
  end;
end;

function THeroObject.EatUseItems(nShape: Integer): Boolean;
begin
  Result := False;
  case nShape of
    1:
      begin

      end;
    2:
      begin

      end;
    3:
      begin

      end;
    4:
      begin
        if WeaptonMakeLuck() then
          Result := True;
      end;
    5:
      begin

      end;
    9:
      begin
        if RepairWeapon() then
          Result := True;
      end;
    10:
      begin
        if SuperRepairWeapon() then
          Result := True;
      end;
    11:
      begin

      end;
  end;
end;

function THeroObject.EatItems(StdItem: pTStdItem): Boolean;
var
  bo06: Boolean;
  Int64Value, Int64Value2: Integer;
begin
  Result := False;
  if m_PEnvir.m_boNODRUG then
  begin
    SysMsg(sCanotUseDrugOnThisMap, c_Red, t_Hint);
    Exit;
  end;
  case StdItem.StdMode of
    0:
      begin
        case StdItem.Shape of
          1:
            begin
              IncHealthSpell(StdItem.AC1, StdItem.MAC1);
              Result := True;
            end;
          2:
            begin
              m_boUserUnLockDurg := True;
              Result := True;
            end;
          101:
            begin
              Int64Value := StdItem.AC1 + Random(StdItem.AC2 + 1);
              if Int64Value > 100 then
                Int64Value := 100;
              Int64Value := Round(m_WAbil.MaxHP / 100 * Int64Value);

              Int64Value2 := StdItem.MAC1 + Random(StdItem.MAC2 + 1);
              if Int64Value2 > 100 then
                Int64Value2 := 100;
              Int64Value2 := Round(m_WAbil.MaxMP / 100 * Int64Value2);

              IncHealthSpell(Int64Value, Int64Value2);
              Result := True;
            end;
          102:
            begin
              Int64Value := StdItem.AC1 + Random(StdItem.AC2 + 1);

              if StdItem.MAC1 > 0 then
              begin
                if Int64Value > 100 then
                  Int64Value := 100;
                Int64Value := Round(m_AbilNG.MaxNH / 100 * Int64Value);
              end;

              Int64Value := m_AbilNG.NH + Int64Value;
              m_AbilNG.NH := Min(Int64Value, m_AbilNG.MaxNH);
              Result := True;

              SendMsg(Self, RM_ABILITYNG, 0, 0, 0, 0, ''); // 内功属性
            end;
        else
          begin
            {
              if ((StdItem.AC + m_nIncHealth) < 500) and (StdItem.AC > 0) then begin
              Inc(m_nIncHealth,StdItem.AC);
              end;
              if ((StdItem.MAC + m_nIncSpell) < 500) and (StdItem.MAC > 0) then begin
              Inc(m_nIncSpell,StdItem.MAC);
              end;
            }
            if (StdItem.AC1 > 0) then
            begin
              Inc(m_nIncHealth, StdItem.AC1);
            end;
            if (StdItem.MAC1 > 0) then
            begin
              Inc(m_nIncSpell, StdItem.MAC1);
            end;
            Result := True;
          end;
        end;
      end;
    1:
      begin
        { nOldStatus := GetMyStatus();
          Inc(m_nHungerStatus, StdItem.DuraMax div 10);
          m_nHungerStatus := _MIN(5000, m_nHungerStatus);
          if nOldStatus <> GetMyStatus() then
          RefMyStatus();
          Result := True; }
      end;
    2:
      Result := True;
    3:
      begin
        if StdItem.Shape = 12 then
        begin
          bo06 := False;
          if StdItem.DC1 > 0 then
          begin
            m_wStatusArrValue[0 { 0x218 } ] := StdItem.DC1;
            m_dwStatusArrTimeOutTick[0 { 0x220 } ] := MyGetTickCount + StdItem.MAC2 * 1000;

            if (Length(g_sDCUpTime) > 0) then
            begin
              SysMsg(StringReplace(g_sDCUpTime, '%t', IntToStr(StdItem.MAC2), []), c_Green, t_Hint);
            end;

            bo06 := True;
          end;
          if StdItem.MC1 > 0 then
          begin
            m_wStatusArrValue[1 { 0x219 } ] := StdItem.MC1;
            m_dwStatusArrTimeOutTick[1 { 0x224 } ] := MyGetTickCount + StdItem.MAC2 * 1000;

            if (Length(g_sMCUpTime) > 0) then
            begin
              SysMsg(StringReplace(g_sMCUpTime, '%t', IntToStr(StdItem.MAC2), []), c_Green, t_Hint);
            end;

            bo06 := True;
          end;
          if StdItem.SC1 > 0 then
          begin
            m_wStatusArrValue[2 { 0x21A } ] := StdItem.SC1;
            m_dwStatusArrTimeOutTick[2 { 0x228 } ] := MyGetTickCount + StdItem.MAC2 * 1000;

            if (Length(g_sSCUpTime) > 0) then
            begin
              SysMsg(StringReplace(g_sSCUpTime, '%t', IntToStr(StdItem.MAC2), []), c_Green, t_Hint);
            end;

            bo06 := True;
          end;
          if StdItem.AC2 > 0 then
          begin
            m_wStatusArrValue[3 { 0x21B } ] := StdItem.AC2;
            m_dwStatusArrTimeOutTick[3 { 0x22C } ] := MyGetTickCount + StdItem.MAC2 * 1000;

            if (Length(g_sHitSpeedUpTime) > 0) then
            begin
              SysMsg(StringReplace(g_sHitSpeedUpTime, '%t', IntToStr(StdItem.MAC2), []), c_Green, t_Hint);
            end;

            bo06 := True;
          end;
          if StdItem.AC1 > 0 then
          begin
            m_wStatusArrValue[4 { 0x21C } ] := StdItem.AC1;
            m_dwStatusArrTimeOutTick[4 { 0x230 } ] := MyGetTickCount + StdItem.MAC2 * 1000;

            if (Length(g_sMaxHPUpTime) > 0) then
            begin
              SysMsg(StringReplace(g_sMaxHPUpTime, '%t', IntToStr(StdItem.MAC2), []), c_Green, t_Hint);
            end;

            bo06 := True;
          end;
          if StdItem.MAC1 > 0 then
          begin
            m_wStatusArrValue[5 { 0x21D } ] := StdItem.MAC1;
            m_dwStatusArrTimeOutTick[5 { 0x234 } ] := MyGetTickCount + StdItem.MAC2 * 1000;

            if (Length(g_sMaxMPUpTime) > 0) then
            begin
              SysMsg(StringReplace(g_sMaxMPUpTime, '%t', IntToStr(StdItem.MAC2), []), c_Green, t_Hint);
            end;

            bo06 := True;
          end;
          if bo06 then
          begin
            RecalcAbilitys();
            SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
            Result := True;
          end;
        end
        else
        begin
          Result := EatUseItems(StdItem.Shape);
        end;
      end;
  end;
end;

function THeroObject.ReadBook(StdItem: pTStdItem): Boolean;
var
  Magic: pTMagic;
  UserMagic: pTUserMagic;
begin
  Result := False;
  Magic := UserEngine.FindHeroMagic(StdItem.Name);
  if Magic <> nil then
  begin
    if not IsTrainingSkill(Magic.wMagicId, Magic.MagicAttr) then
    begin
      if Magic.wMagicId in [60 .. 65] then
      begin
        if Magic.wMagicId <> GetGroupMagicId then
          Exit;
      end;
      if (Magic.btJob = 99) or (Magic.btJob = m_btJob) then
      begin
        if m_Abil.Level >= Magic.TrainLevel[0] then
        begin
          New(UserMagic);
          UserMagic.MagicInfo := Magic;
          UserMagic.MagicAttr := Magic.MagicAttr;
          UserMagic.wMagIdx := Magic.wMagicId;
          UserMagic.btKey := VK_F1;
          UserMagic.btLevel := 0;
          UserMagic.btNewLevel := 0;
          UserMagic.nTranPoint := 0;
          UserMagic.boUsesItemAdd := False;
          m_MagicList.Add(UserMagic);
          RecalcAbilitys();
          SendAddMagic(UserMagic);
          Result := True;

          if g_FunctionNPC <> nil then
          begin
            m_nLearnMagicID := Magic.wMagicId;
            g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroLearnMagic', False);
            m_nLearnMagicID := 0;
          end;
        end;
      end;
    end;
  end;
end;

procedure THeroObject.SendAddItem(UserItem: pTUserItem);
var
  StdItem: pTStdItem;
  ClientItem: TClientItem;
  _dwRecordBeadExp: LongWord;
begin
  if not CanSend then
    Exit;
  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if StdItem = nil then
    Exit;

  UserItemToClientItem(UserItem, StdItem, @ClientItem, True, True);

  m_DefMsg := MakeDefaultMsg(SM_HEROADDITEM, NativeInt(Self), 0, 0, 1);
  SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem));

  if (m_dwRecordBeadExp > 0) and (StdItem.StdMode = 49) and (UserItem.Dura < UserItem.DuraMax) then
  begin
    _dwRecordBeadExp := m_dwRecordBeadExp;
    m_dwRecordBeadExp := 0;
    IncBeadExp(_dwRecordBeadExp, False);
  end;
end;

procedure THeroObject.SendDelItem(UserItem: pTUserItem);
var
  sItemName: string;
  StdItem: pTStdItem;
begin
  if not CanSend then
    Exit;
  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if StdItem <> nil then
  begin
    if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
      sItemName := UserItem.Name
    else
      sItemName := StdItem.Name;

    SendDefMessage(SM_HERODELITEM, UserItem.MakeIndex, 0, 0, 0, sItemName);
  end;
end;

function THeroObject.IsEnoughBag(): Boolean;
begin
  Result := False;
  if m_ItemList.Count < m_nBagCount then
    Result := True;
end;

function THeroObject.AddItemToBag(UserItem: pTUserItem): Boolean;
begin
  Result := False;
  if m_ItemList.Count < m_nBagCount then
  begin
    m_ItemList.Add(UserItem);
    WeightChanged();
    Result := True;
  end;
end;

procedure THeroObject.RefBagItemCount(SendMessage: Boolean);
var
  I: Integer;
  // nOldBagCount: Integer;
begin
  // nOldBagCount := m_nBagCount;
  for I := High(g_Config.HeroBagItemCounts) downto Low(g_Config.HeroBagItemCounts) do
  begin
    if m_Abil.Level >= g_Config.HeroBagItemCounts[I] then
    begin
      case I of
        0:
          m_nBagCount := 10;
        1:
          m_nBagCount := 20;
        2:
          m_nBagCount := 30;
        3:
          m_nBagCount := 35;
        4:
          m_nBagCount := 40;
      end;
      Break;
    end;
  end;
  if { (nOldBagCount <> m_nBagCount) and } SendMessage then
  begin
    SendDefMessage(SM_HEROBAGCOUNT, NativeInt(Self), m_nBagCount, 0, 0, '');
  end;
end;

procedure THeroObject.RecalcLevelAbilitys(IsSysDef: Boolean);
var
  Int64Value: Int64;
  boUseSysDef: Boolean;
  BaseAbil, AddAbil: PBaseAbilInfo;
  nLevel, n, nMaxValue, nLevelSub: Cardinal;
begin
  if g_Config.btMaxLevel = 0 then
    nMaxValue := High(Word)
  else if g_Config.btMaxLevel = 1 then
    nMaxValue := High(Integer)
  else
    nMaxValue := High(LongWord);

  nLevel := m_Abil.Level;

  boUseSysDef := g_BaseAbilConfig.UseDefault or IsSysDef or
    ((not g_BaseAbilConfig.UseDefault) and (nLevel > High(g_BaseAbilConfig.HumAbil[0].Base) + 1) and
    g_BaseAbilConfig.HumAbil[m_btJob].AutoCalcLevel1000);

  if boUseSysDef then
  begin
    case m_btJob of
      2:
        begin
          Int64Value := Round((nLevel / g_Config.nLevelValueOfTaosHP + g_Config.nLevelValueOfTaosHPRate) * nLevel);
          Int64Value := 14 + Max(0, Int64Value);
          m_Abil.MaxHP := Min(nMaxValue, Int64Value);

          Int64Value := Round((Int64(nLevel) / g_Config.nLevelValueOfTaosMP) * 2.2 * nLevel);
          Int64Value := 13 + Max(0, Int64Value);
          m_Abil.MaxMP := Min(nMaxValue, Int64Value);

          { TODO -ochongchong -c添加 : 英雄HP/MP根据比例计算 【2013-08-11】 }
          Int64Value := Round(m_Abil.MaxHP / 100 * g_Config.dwHeroTaosHPMPRate);
          m_Abil.MaxHP := Min(Int64Value, nMaxValue);

          Int64Value := Round(m_Abil.MaxMP / 100 * g_Config.dwHeroTaosHPMPRate);
          m_Abil.MaxMP := Min(Int64Value, nMaxValue);

          if nLevel > 1000 then
          begin
            m_Abil.MaxWeight := High(Word);
            m_Abil.MaxWearWeight := High(Word);
            m_Abil.MaxHandWeight := High(Word);
          end
          else
          begin
            m_Abil.MaxWeight := Min(High(Word), Max(50 + Round((nLevel / 4) * nLevel), 0));
            m_Abil.MaxWearWeight := Min(High(Word), Max(15 + Round((nLevel / 50) * nLevel), 0));
            m_Abil.MaxHandWeight := Min(High(Word), Max(12 + Round((nLevel / 42) * nLevel), 0));
          end;
          n := nLevel div 7;
          m_Abil.DC1 := Max(n - 1, 0);
          m_Abil.DC2 := Max(1, n);

          m_Abil.MC1 := 0;
          m_Abil.MC2 := 0;

          m_Abil.SC1 := Max(n - 1, 0);
          m_Abil.SC2 := Max(1, n);

          m_Abil.AC1 := 0;
          m_Abil.AC2 := 0;

          n := Round(nLevel / 6);
          m_Abil.MAC1 := n div 2;
          m_Abil.MAC2 := n + 1;
        end;
      1:
        begin
          Int64Value := Round((Int64(nLevel) / g_Config.nLevelValueOfWizardHP + g_Config.nLevelValueOfWizardHPRate) * nLevel);
          Int64Value := 14 + Max(0, Int64Value);
          m_Abil.MaxHP := Min(nMaxValue, Int64Value);

          Int64Value := Round((Int64(nLevel) / 5 + 2) * 2.2 * nLevel);
          Int64Value := 13 + Max(0, Int64Value);
          m_Abil.MaxMP := Min(nMaxValue, Int64Value);

          { TODO -ochongchong -c添加 : 英雄HP/MP根据比例计算 【2013-08-11】 }
          Int64Value := Round(m_Abil.MaxHP / 100 * g_Config.dwHeroWizardHPMPRate);
          m_Abil.MaxHP := Min(Int64Value, nMaxValue);

          Int64Value := Round(m_Abil.MaxMP / 100 * g_Config.dwHeroWizardHPMPRate);
          m_Abil.MaxMP := Min(Int64Value, nMaxValue);

          if nLevel > 1000 then
          begin
            m_Abil.MaxWeight := High(Word);
            m_Abil.MaxWearWeight := High(Word);
            m_Abil.MaxHandWeight := High(Word);
          end
          else
          begin
            m_Abil.MaxWeight := Min(High(Word), Max(50 + Round((nLevel / 5) * nLevel), 0));
            m_Abil.MaxWearWeight := Min(High(Word), Max(15 + Round((nLevel / 100) * nLevel), 0));
            m_Abil.MaxHandWeight := Min(High(Word), Max(12 + Round((nLevel / 90) * nLevel), 0));
          end;
          n := nLevel div 7;
          m_Abil.DC1 := Max(n - 1, 0);
          m_Abil.DC2 := Max(1, n);
          m_Abil.MC1 := Max(n - 1, 0);
          m_Abil.MC2 := Max(1, n);
          m_Abil.SC1 := 0;
          m_Abil.SC2 := 0;

          m_Abil.AC1 := 0;
          m_Abil.AC2 := 0;

          m_Abil.MAC1 := 0;
          m_Abil.MAC2 := 0;
        end;
      0:
        begin
          Int64Value := Round((Int64(nLevel) / g_Config.nLevelValueOfWarrHP + g_Config.nLevelValueOfWarrHPRate + nLevel / 20)
            * nLevel);
          Int64Value := 14 + Max(0, Int64Value);
          m_Abil.MaxHP := Min(nMaxValue, Int64Value);

          Int64Value := Round(Int64(nLevel) * 3.5);
          Int64Value := 11 + Max(0, Int64Value);
          m_Abil.MaxMP := Min(nMaxValue, Int64Value);

          { TODO -ochongchong -c添加 : 英雄HP/MP根据比例计算 【2013-08-11】 }

          Int64Value := Round(m_Abil.MaxHP / 100 * g_Config.dwHeroWarrHPMPRate);
          m_Abil.MaxHP := Min(Int64Value, nMaxValue);

          Int64Value := Round(m_Abil.MaxMP / 100 * g_Config.dwHeroWarrHPMPRate);
          m_Abil.MaxMP := Min(Int64Value, nMaxValue);

          if nLevel > 1000 then
          begin
            m_Abil.MaxWeight := High(Word);
            m_Abil.MaxWearWeight := High(Word);
            m_Abil.MaxHandWeight := High(Word);
          end
          else
          begin
            m_Abil.MaxWeight := Min(High(Word), Max(50 + Round((nLevel / 3) * nLevel), 0));
            m_Abil.MaxWearWeight := Min(High(Word), Max(15 + Round((nLevel / 20) * nLevel), 0));
            m_Abil.MaxHandWeight := Min(High(Word), Max(12 + Round((nLevel / 13) * nLevel), 0));
          end;

          m_Abil.DC1 := Max((nLevel div 5) - 1, 1);
          m_Abil.DC2 := Max(1, (nLevel div 5));
          m_Abil.SC1 := 0;
          m_Abil.SC2 := 0;

          m_Abil.MC1 := 0;
          m_Abil.MC2 := 0;

          m_Abil.AC1 := 0;
          m_Abil.AC2 := nLevel div 7;

          m_Abil.MAC1 := 0;
          m_Abil.MAC2 := 0;
        end;
    end;
  end
  else
  begin
    if nLevel <= 0 then
      nLevel := 1;
    if (nLevel > 0) and (nLevel <= High(g_BaseAbilConfig.HeroAbil[0].Base) + 1) then
    begin
      BaseAbil := @g_BaseAbilConfig.HeroAbil[m_btJob].Base[nLevel - 1];

      m_Abil.MaxHP := Min(nMaxValue, BaseAbil.MaxHP);
      m_Abil.MaxMP := Min(nMaxValue, BaseAbil.MaxMP);
      m_Abil.MaxWeight := Min(High(Word), BaseAbil.MaxWeight);
      m_Abil.MaxWearWeight := Min(High(Word), BaseAbil.MaxWearWeight);
      m_Abil.MaxHandWeight := Min(High(Word), BaseAbil.MaxHandWeight);

      m_Abil.DC1 := BaseAbil.DC1;
      m_Abil.DC2 := BaseAbil.DC2;
      m_Abil.MC1 := BaseAbil.MC1;
      m_Abil.MC2 := BaseAbil.MC2;

      m_Abil.SC1 := BaseAbil.SC1;
      m_Abil.SC2 := BaseAbil.SC2;

      m_Abil.AC1 := BaseAbil.AC1;
      m_Abil.AC2 := BaseAbil.AC2;

      m_Abil.MAC1 := BaseAbil.MAC1;
      m_Abil.MAC2 := BaseAbil.MAC2;
    end
    else
    begin
      nLevelSub := nLevel - High(g_BaseAbilConfig.HeroAbil[m_btJob].Base) - 1;

      BaseAbil := @g_BaseAbilConfig.HeroAbil[m_btJob].Base[High(g_BaseAbilConfig.HeroAbil[m_btJob].Base)];
      AddAbil := @g_BaseAbilConfig.HeroAbil[m_btJob].Add;

      Int64Value := BaseAbil.MaxHP + AddAbil.MaxHP * nLevelSub;
      m_Abil.MaxHP := Min(nMaxValue, Int64Value);

      Int64Value := BaseAbil.MaxMP + AddAbil.MaxMP * nLevelSub;
      m_Abil.MaxMP := Min(nMaxValue, Int64Value);

      Int64Value := BaseAbil.MaxWeight + AddAbil.MaxWeight * nLevelSub;
      m_Abil.MaxWeight := Min(High(Word), Int64Value);

      Int64Value := BaseAbil.MaxWearWeight + AddAbil.MaxWearWeight * nLevelSub;
      m_Abil.MaxWearWeight := Min(High(Word), Int64Value);

      Int64Value := BaseAbil.MaxHandWeight + AddAbil.MaxHandWeight * nLevelSub;
      m_Abil.MaxHandWeight := Min(High(Word), Int64Value);

      Int64Value := BaseAbil.DC1 + AddAbil.DC1 * nLevelSub;
      m_Abil.DC1 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.DC2 + AddAbil.DC2 * nLevelSub;
      m_Abil.DC2 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.MC1 + AddAbil.MC1 * nLevelSub;
      m_Abil.MC1 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.MC2 + AddAbil.MC2 * nLevelSub;
      m_Abil.MC2 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.SC1 + AddAbil.SC1 * nLevelSub;
      m_Abil.SC1 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.SC2 + AddAbil.SC2 * nLevelSub;
      m_Abil.SC2 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.AC1 + AddAbil.AC1 * nLevelSub;
      m_Abil.AC1 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.AC2 + AddAbil.AC2 * nLevelSub;
      m_Abil.AC2 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.MAC1 + AddAbil.MAC1 * nLevelSub;
      m_Abil.MAC1 := Min(High(Integer), Int64Value);

      Int64Value := BaseAbil.MAC2 + AddAbil.MAC2 * nLevelSub;
      m_Abil.MAC2 := Min(High(Integer), Int64Value);
    end;
  end;
  if m_Abil.MaxHP <= 0 then
    m_Abil.MaxHP := 15;
  if m_Abil.MaxMP <= 0 then
    m_Abil.MaxMP := 15;
  if m_Abil.HP > m_Abil.MaxHP then
    m_Abil.HP := m_Abil.MaxHP;
  if m_Abil.MP > m_Abil.MaxMP then
    m_Abil.MP := m_Abil.MaxMP;
end;

procedure THeroObject.HasLevelUp(nLevel: Integer; IsTriggerFunc: Boolean; SendHealthSpellChanged: Boolean);
begin
  m_Abil.MaxExp := GetLevelExp(m_Abil.Level);
  RecalcLevelAbilitys(False);
  RecalcAbilitys();
  SendRefMsg(RM_LEVELUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');

  if (m_Master <> nil) and ((abs(m_nCurrX - m_Master.m_nCurrX) > g_Config.nSendRefMsgRange) or
    (abs(m_nCurrY - m_Master.m_nCurrY) > m_Master.m_nViewRange)) then
    m_Master.SendMsg(Self, RM_LEVELUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');

  RefBagItemCount;

  if IsTriggerFunc and (g_FunctionNPC <> nil) and (m_Master <> nil) then
  begin
    TPlayObject(m_Master).m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroLevelUp', False);
  end;
end;

procedure THeroObject.GainExp(dwExp: LongWord);
begin
  WinExp(dwExp);
end;

procedure THeroObject.GetExp(dwExp: LongWord; FormNPC: Boolean; IsIncBead: Boolean; IsFromChangeExp: Boolean);
var
  OldLevel: LongWord;
  dwAddExp: LongWord;
  lwExp: LongWord;
  nMaxLevel: LongWord;
  nMaxUpLevelCount: Integer;
  IsLeveUped: Boolean;
label
  RefExp;
begin
  if g_Config.btMaxLevel = 0 then
    nMaxLevel := MAXUPLEVEL
  else if g_Config.btMaxLevel = 1 then
    nMaxLevel := High(LongInt)
  else
    nMaxLevel := High(LongWord);

  if not IsFromChangeExp then
  begin
    if m_Abil.Level >= g_Config.dwHeroHighLevel then
      dwExp := Max(g_Config.dwHeroHighLevelGetExp, 0);
  end;

  lwExp := dwExp;

  { TODO -ochongchong -c新增 : 英雄忠诚度 - 经验增加时增加忠诚度 【2013-08-13】 }
  Inc(m_dwHeroSaveExp, dwExp);
  if (g_Config.dwHeroFealtyExp > 0) and (g_Config.dwHeroFealtyExpAdd > 0) and (m_rLoyalPoint < 100) and
    (m_dwHeroSaveExp >= g_Config.dwHeroFealtyExp) then
  begin
    m_rLoyalPoint := Min(100, m_rLoyalPoint + g_Config.dwHeroFealtyExpAdd / 100);
    SendLoyalPoint;

    if m_rLoyalPoint = 100 then
      m_boHeroLevel4Magic := True;

    m_dwHeroSaveExp := 0;
  end;

  OldLevel := m_Abil.Level;

  nMaxUpLevelCount := 0;
  IsLeveUped := False;
RefExp:

  Inc(nMaxUpLevelCount);

  if m_Abil.MaxExp > m_Abil.Exp then
    dwAddExp := m_Abil.MaxExp - m_Abil.Exp
  else
    dwAddExp := 0;

  if (lwExp >= dwAddExp) and (m_Abil.MaxExp > m_Abil.Exp) then
  begin
    if m_Abil.Level < nMaxLevel then
    begin
      Inc(m_Abil.Level);
      lwExp := lwExp - dwAddExp;
      m_Abil.Exp := 0;

      // IncBeadExp(dwAddExp);
      AddBodyLuck(dwAddExp * 0.002);

      IsLeveUped := True;
      // HasLevelUp(m_Abil.Level - 1, True, False);
      m_Abil.MaxExp := GetLevelExp(m_Abil.Level);
      IncHealthSpell(2000, 2000, False);

      if lwExp > 0 then
      begin
        if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
        begin
          goto RefExp;
          Exit;
        end
        else
        begin
          Inc(m_Abil.Exp, lwExp);
        end;
      end;
    end;
  end
  else
  begin
    if m_Abil.MaxExp > m_Abil.Exp then
    begin
      Inc(m_Abil.Exp, lwExp);
      // IncBeadExp(lwExp);
      AddBodyLuck(lwExp * 0.002);
    end
    else
    begin
      if lwExp > 0 then
      begin
        Inc(m_Abil.Exp, lwExp);
        lwExp := 0;
        AddBodyLuck(lwExp * 0.002);
      end;

      Inc(m_Abil.Level);
      Dec(m_Abil.Exp, m_Abil.MaxExp);

      IsLeveUped := True;
      // HasLevelUp(m_Abil.Level - 1, True, False);
      m_Abil.MaxExp := GetLevelExp(m_Abil.Level);
      IncHealthSpell(2000, 2000, False);

      if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
      begin
        goto RefExp;
        Exit;
      end;
    end;
  end;

  m_dwGetExp := dwExp;

  // 聚灵珠加经验 chongchong 2013-12-10
  if IsIncBead then
    IncBeadExp(dwExp, FormNPC);

  // MMB的，非要把打一堆怪物经验按照人物和英雄穿插排列
  // SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '');

  if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
    m_Master.SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '')
  else
    SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '');

  if g_FunctionNPC <> nil then
  begin
    TPlayObject(m_Master).m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroGetExp', False);

    if (not FormNPC) then
    begin
      g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroKillMonGetExp', False);
    end;
  end;

  // 这里总的变一次
  if IsLeveUped then
  begin
    AddGameDataLog(LOG_LevelChange, LOG_ActionNone, Self, '等级', 0, '0', m_Abil.Level, OldLevel);

    RecalcLevelAbilitys(False);
    RecalcAbilitys();

    RefBagItemCount;

    SendRefMsg(RM_LEVELUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');

    if (m_Master <> nil) and ((abs(m_nCurrX - m_Master.m_nCurrX) > g_Config.nSendRefMsgRange) or
      (abs(m_nCurrY - m_Master.m_nCurrY) > m_Master.m_nViewRange)) then
      m_Master.SendMsg(Self, RM_LEVELUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');

    if (g_FunctionNPC <> nil) and (m_Master <> nil) then
    begin
      TPlayObject(m_Master).m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroLevelUp', False);
    end;

    SendRefMsg(RM_HEALTHSPELLCHANGED, 0, 0, 0, 0, ''); // 刷新HP
  end;
end;

procedure THeroObject.WinExp(dwExp: LongWord);
// var
// dwGetExp: LongWord;
begin
  if m_Abil.Level > g_Config.nLimitExpLevel then
  begin
    dwExp := g_Config.nLimitExpValue;
    GetExp(dwExp, False, False);
  end
  else if dwExp > 0 then
  begin
    dwExp := g_Config.dwKillMonExpMultiple * dwExp; // 系统指定杀怪经验倍数
    // dwExp := LongWord(m_nKillMonExpMultiple) * dwExp; //人物指定的杀怪经验倍数
    if m_nKillMonExpRate > 0 then
      dwExp := Round((m_nKillMonExpRate / 100) * dwExp); // 人物指定的杀怪经验倍数
    if m_PEnvir.m_boEXPRATE then
      dwExp := Round((m_PEnvir.m_nEXPRATE / 100) * dwExp); // 地图上指定杀怪经验倍数
    if m_boExpItem then
    begin // 物品经验倍数
      dwExp := Round(m_rExpItem * dwExp);
    end;
    // if m_boExpGroupItem then begin //套装经验倍数
    // dwExp := Round(m_rExpGroupItem * dwExp);
    // end;

    if m_Abil.Level >= g_Config.dwHeroHighLevel then
      dwExp := Max(g_Config.dwHeroHighLevelGetExp, 0);

    GetExp(GetLevelExpRate(dwExp), False, False);
  end;
end;

procedure THeroObject.IncExp(dwExp: LongWord);
var
  dwAddExp: LongWord;
  lwExp: LongWord;
  OldLevel, nMaxLevel: LongWord;
  nMaxUpLevelCount: Integer;
label
  RefExp;
begin
  if g_Config.btMaxLevel = 0 then
    nMaxLevel := High(Word)
  else if g_Config.btMaxLevel = 1 then
    nMaxLevel := High(Integer)
  else
    nMaxLevel := High(LongWord);

  if m_Abil.Level >= g_Config.dwHeroHighLevel then
    dwExp := Max(g_Config.dwHeroHighLevelGetExp, 0);

  lwExp := dwExp;
  nMaxUpLevelCount := 0;

  OldLevel := m_Abil.Level;

RefExp:
  Inc(nMaxUpLevelCount);
  dwAddExp := m_Abil.MaxExp - m_Abil.Exp;
  if (lwExp >= dwAddExp) and (m_Abil.MaxExp > m_Abil.Exp) then
  begin
    if m_Abil.Level < nMaxLevel then
    begin
      Inc(m_Abil.Level);

      lwExp := lwExp - dwAddExp;
      m_Abil.Exp := 0;

      HasLevelUp(m_Abil.Level - 1);

      IncHealthSpell(2000, 2000);

      if lwExp > 0 then
      begin
        if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
        begin
          goto RefExp;
          Exit;
        end
        else
        begin
          Inc(m_Abil.Exp, lwExp);
        end;
      end;
    end;
  end
  else
  begin
    if m_Abil.MaxExp > m_Abil.Exp then
    begin
      Inc(m_Abil.Exp, lwExp);
    end
    else
    begin
      Inc(m_Abil.Level);
      Dec(m_Abil.Exp, m_Abil.MaxExp);

      HasLevelUp(m_Abil.Level - 1);

      IncHealthSpell(2000, 2000);
      if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
      begin
        goto RefExp;
        Exit;
      end;
    end;
  end;

  if OldLevel <> m_Abil.Level then
  begin
    AddGameDataLog(LOG_LevelChange, LOG_ActionNone, Self, '等级', 0, '0', m_Abil.Level, OldLevel);
  end;

  SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '');
  m_dwGetExp := dwExp;
end;

procedure THeroObject.GainExpNG(dwExp: LongWord);
begin
  WinExpNG(dwExp);
end;

procedure THeroObject.GetExpNG(dwExp: LongWord);
var
  dwAddExp: LongWord;
  OldLevel, lwExp: LongWord;
  nMaxUpLevelCount: Integer;
label
  RefExp;
begin
  if not m_boTrainingNG then
    Exit;
  if dwExp = 0 then
    Exit;

  if m_AbilNG.Level >= g_Config.nNGMaxLevelLimte then
  begin
    Exit;
  end;

  lwExp := dwExp;
  nMaxUpLevelCount := 0;
  OldLevel := m_AbilNG.Level;

RefExp:
  Inc(nMaxUpLevelCount);
  dwAddExp := m_AbilNG.MaxExp - m_AbilNG.Exp;
  if (lwExp >= dwAddExp) and (m_AbilNG.MaxExp > m_AbilNG.Exp) then
  begin
    if m_AbilNG.Level < MAXNG_LEVEL then
    begin
      Inc(m_AbilNG.Level);
      lwExp := lwExp - dwAddExp;
      m_AbilNG.Exp := 0;

      HasLevelUpNG(m_AbilNG.Level - 1);

      if lwExp > 0 then
      begin
        if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
        begin
          goto RefExp;
          Exit;
        end
        else
        begin
          Inc(m_AbilNG.Exp, lwExp);
        end;
      end;
    end;
  end
  else
  begin
    if m_AbilNG.MaxExp > m_AbilNG.Exp then
    begin
      Inc(m_AbilNG.Exp, lwExp);
    end
    else
    begin
      Inc(m_AbilNG.Level);
      Dec(m_AbilNG.Exp, m_AbilNG.MaxExp);

      HasLevelUpNG(m_AbilNG.Level - 1);

      if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
      begin
        goto RefExp;
        Exit;
      end;
    end;
  end;

  m_dwGetExp := dwExp;
  SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '');
  if g_FunctionNPC <> nil then
  begin
    TPlayObject(m_Master).m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroGetNGExp', False);
  end;

  if OldLevel <> m_AbilNG.Level then
  begin
    AddGameDataLog(LOG_LevelChange, LOG_ActionNone, Self, '内功等级', 0, '0', m_AbilNG.Level, OldLevel);
  end;
end;

procedure THeroObject.WinExpNG(dwExp: LongWord);
begin
  if not m_boTrainingNG then
    Exit;

  if m_AbilNG.Level > g_Config.nLimitExpLevel then
  begin
    dwExp := g_Config.nLimitExpValue;
    GetExpNG(dwExp);
  end
  else if dwExp > 0 then
  begin
    dwExp := g_Config.dwKillMonExpMultiple * dwExp; // 系统指定杀怪经验倍数
    if m_nKillMonExpRate > 0 then
      dwExp := Round((m_nKillMonExpRate / 100) * dwExp); // 人物指定的杀怪经验倍数
    if m_PEnvir.m_boEXPRATE then
      dwExp := Round((m_PEnvir.m_nEXPRATE / 100) * dwExp); // 地图上指定杀怪经验倍数
    if m_boExpItem then
    begin // 物品经验倍数
      dwExp := Round(m_rExpItem * dwExp);
    end;

    dwExp := abs(Round(dwExp * g_Config.nNGKillMonExpMultiple / 100)); // 内功杀怪经验倍数

    if m_AbilNG.Level >= g_Config.dwHeroHighLevel then
      dwExp := Max(g_Config.dwHeroHighLevelGetExp, 0);

    GetExpNG(dwExp);
  end;
end;

procedure THeroObject.IncExpNG(dwExp: LongWord);
var
  dwAddExp: LongWord;
  OldLevel, lwExp: LongWord;
  nMaxUpLevelCount: Integer;
label
  RefExp;
begin
  if not m_boTrainingNG then
    Exit;

  if m_Abil.Level >= g_Config.dwHeroHighLevel then
    dwExp := Max(g_Config.dwHeroHighLevelGetExp, 0);

  lwExp := dwExp;
  nMaxUpLevelCount := 0;
  OldLevel := m_AbilNG.Level;

RefExp:

  Inc(nMaxUpLevelCount);
  dwAddExp := m_AbilNG.MaxExp - m_AbilNG.Exp;
  if (lwExp >= dwAddExp) and (m_AbilNG.MaxExp > m_AbilNG.Exp) then
  begin
    if m_AbilNG.Level < MAXNG_LEVEL then
    begin
      Inc(m_AbilNG.Level);

      lwExp := lwExp - dwAddExp;
      m_AbilNG.Exp := 0;

      HasLevelUpNG(m_AbilNG.Level - 1);

      if lwExp > 0 then
      begin
        if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
        begin
          goto RefExp;
          Exit;
        end
        else
        begin
          Inc(m_AbilNG.Exp, lwExp);
        end;
      end;

    end;
  end
  else
  begin
    if m_AbilNG.MaxExp > m_AbilNG.Exp then
    begin
      Inc(m_AbilNG.Exp, lwExp);
    end
    else
    begin
      Inc(m_AbilNG.Level);
      Dec(m_AbilNG.Exp, m_AbilNG.MaxExp);

      HasLevelUpNG(m_AbilNG.Level - 1);

      if nMaxUpLevelCount < g_Config.nMaxUpLevelCount then
      begin
        goto RefExp;
        Exit;
      end;
    end;
  end;
  SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '');

  if OldLevel <> m_AbilNG.Level then
  begin
    AddGameDataLog(LOG_LevelChange, LOG_ActionNone, Self, '内功等级', 0, '0', m_AbilNG.Level, OldLevel);
  end;
end;

// 内功升级触发

function THeroObject.LevelUpFuncNG: Boolean;
begin
  Result := False;
  if (g_FunctionNPC <> nil) and (m_Master <> nil) then
  begin
    TPlayObject(m_Master).m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroNGLevelUp', False);
    Result := True;
  end;
end;

procedure THeroObject.MakeSaveRcd(HeroData: PTHeroData);
var
  I, n1C, n2C, n3C: Integer;
  BagItems: pTBagItems;
  HumMagics: pTHumMagics;
  HumNGMagics: pTHumNGMagics;
  HumContinuousMagics: pTHumContinuousMagics;
  UserMagic: pTUserMagic;
begin
  HeroData.sAccount := m_sUserID;
  HeroData.sChrName := m_sCharName;
  HeroData.sCurMap := m_sMapName;

  HeroData.btStatus := m_btAttackMode;

  HeroData.wCurX := m_nCurrX;
  HeroData.wCurY := m_nCurrY;
  HeroData.btDir := m_btDirection;
  HeroData.btHair := m_btHair;
  HeroData.btSex := m_btGender;
  HeroData.btJob := m_btJob;

  Move(m_Abil, HeroData.Abil, SizeOf(TOAbility));

  HeroData.Abil.HP := m_WAbil.HP;
  HeroData.Abil.MP := m_WAbil.MP;
  // HeroData.Abil.CreditPoint := m_WAbil.CreditPoint; -- piaoyun

  HeroData.wStatusTimeArr := m_wStatusTimeArr;
  HeroData.wStatusTimeArr[STATE_TRANSPARENT] := 0; // 隐身不保存
  {
    HeroData.sHomeMap := m_sHomeMap;
    HeroData.wHomeX := m_nHomeX;
    HeroData.wHomeY := m_nHomeY;
  }
  HeroData.nPKPOINT := m_nPkPoint;
  HeroData.sMasterName := m_sMasterName;

  HeroData.btReLevel := m_btReLevel;
  HeroData.rLoyalPoint := m_rLoyalPoint;

  HeroData.btAttackMode := m_btAttatckMode;
  HeroData.btIncHealth := m_nIncHealth;
  HeroData.btIncSpell := m_nIncSpell;
  HeroData.btIncHealing := m_nIncHealing;
  HeroData.btFightZoneDieCount := m_nFightZoneDieCount;
  HeroData.nRevivalTime := m_nCheckRevivalTime;
  HeroData.nHungerStatus := m_nHungerStatus;

  // HeroData.IsNewServer := 0;

  HeroData.QuestFlag := m_QuestFlag;

  if m_boSaveKillMonExpRate then
  begin
    HeroData.boSaveKillMonExpRate := True;
    HeroData.nKillMonExpRate := m_nKillMonExpRate;
    HeroData.dwKillMonExpRateTime := m_dwKillMonExpRateTime;
  end
  else
  begin
    HeroData.boSaveKillMonExpRate := False;
    HeroData.nKillMonExpRate := 100;
    HeroData.dwKillMonExpRateTime := 0;
  end;

  HeroData.boTrainingNG := m_boTrainingNG; // 是否学习过内功
  HeroData.boTrainingXF := m_boTrainingXF; // 是否学习过心法
  HeroData.nDrinkWineQuality := m_nDrinkWineQuality; // 饮酒时酒的品质
  HeroData.nDrinkWineAlcohol := m_nDrinkWineAlcohol; // 饮酒时酒的度数
  HeroData.boDrinkWineDrunk := m_boDrinkWineDrunk; // 人是否喝酒醉了
  HeroData.AbilNG := m_AbilNG; // 内功属性
  HeroData.Alcohol := m_Alcohol; // 酒属性
  HeroData.Meridians := m_HumMeridians;

  HeroData.boOpenLastContinuous := m_boOpenLastContinuous; // 第四个连击是否开启
  HeroData.ContinuousMagicOrder[0] := m_ContinuousMagicOrder[0]; // 连击顺序
  HeroData.ContinuousMagicOrder[1] := m_ContinuousMagicOrder[1]; // 连击顺序
  HeroData.ContinuousMagicOrder[2] := m_ContinuousMagicOrder[2]; // 连击顺序
  HeroData.btLastContinuousMagicOrder := m_ContinuousMagicOrder[3]; // 连击顺序
  {
    HumItems := @PrivateData.HumItems;
    HumItems[U_DRESS] := m_UseItems[U_DRESS];
    HumItems[U_WEAPON] := m_UseItems[U_WEAPON];
    HumItems[U_RIGHTHAND] := m_UseItems[U_RIGHTHAND];
    HumItems[U_HELMET] := m_UseItems[U_NECKLACE];
    HumItems[U_NECKLACE] := m_UseItems[U_HELMET];
    HumItems[U_ARMRINGL] := m_UseItems[U_ARMRINGL];
    HumItems[U_ARMRINGR] := m_UseItems[U_ARMRINGR];
    HumItems[U_RINGL] := m_UseItems[U_RINGL];
    HumItems[U_RINGR] := m_UseItems[U_RINGR];
    HumItems[U_BUJUK] := m_UseItems[U_BUJUK];
    HumItems[U_BELT] := m_UseItems[U_BELT];
    HumItems[U_BOOTS] := m_UseItems[U_BOOTS];
    HumItems[U_CHARM] := m_UseItems[U_CHARM];
    HumItems[U_HAT] := m_UseItems[U_HAT];
    HumItems[U_DRUM] := m_UseItems[U_DRUM];                   // 鼓
    HumItems[U_HORSE] := m_UseItems[U_HORSE];                 // 马牌 chongchong 2013-10-12
    HumItems[U_SHIELD] := m_UseItems[U_SHIELD];               // 盾牌 chongchong 2013-09-16
  }
  Move(m_UseItems, HeroData.HumItems, SizeOf(THumanUseItems));

  BagItems := @HeroData.BagItems;
  for I := 0 to m_ItemList.Count - 1 do
  begin
    if I >= MAX_HERO_BAG_ITEM then
      Break;
    BagItems[I] := pTUserItem(m_ItemList.Items[I])^;
  end;

  n1C := 0;
  n2C := 0;
  n3C := 0;
  HumMagics := @HeroData.Magics;
  HumNGMagics := @HeroData.NGMagics;
  HumContinuousMagics := @HeroData.ContinuousMagics;
  for I := 0 to m_MagicList.Count - 1 do
  begin
    UserMagic := m_MagicList.Items[I];
    if UserMagic.MagicAttr = mtContinuous then
    begin
      if n3C < Length(HeroData.ContinuousMagics) then
      begin
        HumContinuousMagics[n3C].MagicAttr := UserMagic.MagicAttr;
        HumContinuousMagics[n3C].wMagIdx := UserMagic.wMagIdx;
        HumContinuousMagics[n3C].btLevel := UserMagic.btLevel;
        HumContinuousMagics[n3C].btNewLevel := UserMagic.btNewLevel;
        HumContinuousMagics[n3C].btKey := UserMagic.btKey;
        HumContinuousMagics[n3C].nTranPoint := UserMagic.nTranPoint;
        HumContinuousMagics[n3C].boUsesItemAdd := UserMagic.boUsesItemAdd;
        Inc(n3C);
      end;
    end
    else if UserMagic.MagicAttr in [mtDefense, mtAttack] then
    begin
      if n2C < Length(HeroData.NGMagics) then
      begin
        HumNGMagics[n2C].MagicAttr := UserMagic.MagicAttr;
        HumNGMagics[n2C].wMagIdx := UserMagic.wMagIdx;
        HumNGMagics[n2C].btLevel := UserMagic.btLevel;
        HumNGMagics[n2C].btNewLevel := UserMagic.btNewLevel;
        HumNGMagics[n2C].btKey := UserMagic.btKey;
        HumNGMagics[n2C].nTranPoint := UserMagic.nTranPoint;
        HumNGMagics[n2C].boUsesItemAdd := UserMagic.boUsesItemAdd;
        Inc(n2C);
      end;
    end
    else
    begin
      if n1C < Length(HeroData.Magics) then
      begin
        HumMagics[n1C].MagicAttr := UserMagic.MagicAttr;
        HumMagics[n1C].wMagIdx := UserMagic.wMagIdx;
        HumMagics[n1C].btLevel := UserMagic.btLevel;
        HumMagics[n1C].btNewLevel := UserMagic.btNewLevel;
        HumMagics[n1C].btKey := UserMagic.btKey;
        HumMagics[n1C].nTranPoint := UserMagic.nTranPoint;
        HumMagics[n1C].boUsesItemAdd := UserMagic.boUsesItemAdd;
        Inc(n1C);
      end;
    end;
  end;

  // 首饰盒状态 0:未激活; 1:激活; 2:开启 chongchong 2013-10-19
  HeroData.JewelryBoxStatus := m_nJewelryBoxStatus;

  // 保存首饰盒中的数据 chongchong 2013-10-20
  Move(m_JewelryBoxItems, HeroData.JewelryBoxItems, SizeOf(THumanJewelryBoxItems));

  // 显示时装 chonchong 2013-10-23
  HeroData.boShowFashion := m_boShowFashion;

  HeroData.boShowGodBless := m_boShowGodBless;
  Move(m_GodBlessItemsState, HeroData.GodBlessItemsState, SizeOf(TGodBlessItemsState));
  Move(m_GodBlessItems, HeroData.GodBlessItems, SizeOf(THumanGodBlessItems));

  n3C := 0;
  for I := 0 to m_FengHaoItems.Count - 1 do
  begin
    if n3C < Length(HeroData.FengHaoItems) then
    begin
      HeroData.FengHaoItems[n3C] := pTUserItem(m_FengHaoItems.Items[I])^;
      Inc(n3C);
    end
    else
      Break;
  end;
  HeroData.nActiveFengHao := m_ActiveFengHao;

  FillChar(HeroData.AddSaveAbil, SizeOf(HeroData.AddSaveAbil), 0);
  Move(m_AddSaveAbil, HeroData.AddSaveAbil, Min(SizeOf(HeroData.AddSaveAbil), SizeOf(m_AddSaveAbil)));

  if m_boSaveKillMonBurstRate and (m_dwKillMonBurstRate > 0) then
  begin
    HeroData.boSaveKillMonBurstRate := m_boSaveKillMonBurstRate;
    HeroData.nKillMonBurstRate := m_dwKillMonBurstRate;
    HeroData.dwKillMonBurstRateTime := m_dwKillMonBurstRateTime;
  end
  else
  begin
    HeroData.boSaveKillMonBurstRate := False;
    HeroData.nKillMonBurstRate := 0;
    HeroData.dwKillMonBurstRateTime := 0;
  end;

  if m_boAttackHumSavePowerRate then
  begin
    HeroData.boAttackHumSavePowerRate := m_boAttackHumSavePowerRate;
    HeroData.dwAttackHumPowerRateTime := m_dwAttackHumPowerRateTime;
    HeroData.nAttackHumPowerRate := m_nAttackHumPowerRate;
  end
  else
  begin
    HeroData.boAttackHumSavePowerRate := False;
    HeroData.dwAttackHumPowerRateTime := 0;
    HeroData.nAttackHumPowerRate := 100;
  end;

  if m_boAttackMonSavePowerRate then
  begin
    HeroData.boAttackMonSavePowerRate := m_boAttackMonSavePowerRate;
    HeroData.dwAttackMonPowerRateTime := m_dwAttackMonPowerRateTime;
    HeroData.nAttackMonPowerRate := m_nAttackMonPowerRate;
  end
  else
  begin
    HeroData.boAttackMonSavePowerRate := False;
    HeroData.dwAttackMonPowerRateTime := 0;
    HeroData.nAttackMonPowerRate := 100;
  end;

  if m_boSaveHighLevelKillMonFixExpTime and (m_dwHighLevelKillMonFixExpTime > 0) then
  begin
    HeroData.dwHighLevelKillMonFixExpTimeLeft := m_dwHighLevelKillMonFixExpTime;
  end
  else
  begin
    HeroData.dwHighLevelKillMonFixExpTimeLeft := 0;
  end;

  for I := Low(m_NpcSkillPowerAdd) to High(m_NpcSkillPowerAdd) do
  begin
    if m_NpcSkillPowerAdd[I].IsSave and ((m_NpcSkillPowerAdd[I].HumanAttackPercent <> 0) or
      (m_NpcSkillPowerAdd[I].HumanAttackValue <> 0) or (m_NpcSkillPowerAdd[I].MonAttackPercent <> 0) or
      (m_NpcSkillPowerAdd[I].MonAttackValue <> 0) or (m_NpcSkillPowerAdd[I].DefensePercent <> 0) or
      (m_NpcSkillPowerAdd[I].DefenseValue <> 0)) then
    begin
      HeroData.NpcSkillPowerAdd[I].HumanAttackPercent := m_NpcSkillPowerAdd[I].HumanAttackPercent;
      HeroData.NpcSkillPowerAdd[I].HumanAttackValue := m_NpcSkillPowerAdd[I].HumanAttackValue;
      HeroData.NpcSkillPowerAdd[I].MonAttackPercent := m_NpcSkillPowerAdd[I].MonAttackPercent;
      HeroData.NpcSkillPowerAdd[I].MonAttackValue := m_NpcSkillPowerAdd[I].MonAttackValue;
      HeroData.NpcSkillPowerAdd[I].DefensePercent := m_NpcSkillPowerAdd[I].DefensePercent;
      HeroData.NpcSkillPowerAdd[I].DefenseValue := m_NpcSkillPowerAdd[I].DefenseValue;
      HeroData.NpcSkillPowerAdd[I].RemainingTime := m_NpcSkillPowerAdd[I].RemainingTime;
    end
    else
    begin
      FillChar(HeroData.NpcSkillPowerAdd[I], SizeOf(HeroData.NpcSkillPowerAdd[I]), 0);
    end;
  end;

end;

function THeroObject.GetShowName(boSuperUser: Boolean): string;
begin
  // 修复关闭显示神秘人后，英雄戴斗笠还是显示神秘人 chongchong 2014-04-30
  if m_boMysteriousMan and (not boSuperUser) and g_Config.boShowMysteriousMan then
  begin
    Result := g_Config.sMysteriousManName;
    Exit;
  end;

  if (m_PEnvir.m_nSecretFlag and SecretFlag_ShowEqualName <> 0) and (Length(m_PEnvir.m_sSecretShowName) > 0) then
  begin
    Result := m_PEnvir.m_sSecretShowName;
    Exit;
  end;

  if (m_PEnvir.m_nSecretFlag2 and SecretFlag_ShowEqualName <> 0) and (Length(m_PEnvir.m_sSecretShowName2) > 0) then
  begin
    Result := m_PEnvir.m_sSecretShowName2;
    Exit;
  end;

  // 英雄名带主人时，分2行显示 chongchong 2017-11-16
  if (m_Master <> nil) and g_Config.boHeroShowMasterName then
  begin
    // Result := m_Master.m_sCharName + g_Config.sHeroSuffixName + ' ' + AnsiReplaceText(m_sRankLevelName, '%s', m_sCharName);
    Result := AnsiReplaceText(m_sRankLevelName, '%s', m_sCharName) + '\' + m_Master.m_sCharName + g_Config.sHeroSuffixName;
  end
  else
  begin
    Result := AnsiReplaceText(m_sRankLevelName, '%s', m_sCharName);
  end;
end;

procedure THeroObject.SendLoyalPoint;
begin
  SendDefMessage(SM_HEROLOYAL, NativeInt(Self), 0, 0, 0, Format('%.2f', [m_rLoyalPoint]));
end;

procedure THeroObject.LogOn();
var
  I: Integer;
  sItem: string;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  S: string;
begin
  m_dLogonTime := Now();
  m_dwLogonTick := MyGetTickCount();

  { TODO -ochongchong -c新增 : 英雄攻击模式随人物调整 【2013-08-15】 }
  if m_Master <> nil then
  begin
    if g_Config.boHeroForcePeaceMode then
      m_btAttatckMode := HAM_PEACE
    else
      m_btAttatckMode := m_Master.m_btAttatckMode;

    m_sHomeMap := m_Master.m_sHomeMap;
    m_nHomeX := m_Master.m_nHomeX;
    m_nHomeY := m_Master.m_nHomeY;
  end;

  // 上线消失
  if not m_boDummyObject then
  begin
    for I := 0 to Length(m_UseItems) - 1 do
    begin
      if (m_UseItems[I].wIndex > 0) and g_ItemRules.Get(m_UseItems[I].wIndex, 5) then
      begin
        StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
        if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
        begin
          AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, m_UseItems[I].MakeIndex, '0', 0, 0, '上线消失-装备物品');
        end;

        m_UseItems[I].wIndex := 0;
      end;
    end;

    for I := m_ItemList.Count - 1 downto 0 do
    begin
      UserItem := m_ItemList.Items[I];
      if g_ItemRules.Get(UserItem.wIndex, 5) then
      begin
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
        begin
          AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, UserItem.MakeIndex, '0', 0, 0, '上线消失-背包物品');
        end;

        m_ItemList.Delete(I);
        Dispose(UserItem);
      end;
    end;
  end;

  if m_boNewHero then
  begin
    { TODO -ochongchong -c添加 : 主将英雄出生等级 【2013-08-11】 }
    if not m_boIsDeputy then
      m_Abil.Level := g_Config.dwHeroMasterStartLevel
    else
      m_Abil.Level := g_Config.dwHeroSlaveStartLevel;

    New(UserItem);
    if UserEngine.CopyToUserItemFromName(g_Config.sHeroCandle, UserItem) then
    begin
      UserItem.ItemFrom.ItemForm := ifSysGive;
      UserItem.ItemFrom.sMakerName := m_sCharName;
      UserItem.ItemFrom.DateTime := Now();

      m_ItemList.Add(UserItem);
    end
    else
      Dispose(UserItem);
    New(UserItem);
    if UserEngine.CopyToUserItemFromName(g_Config.sHeroBasicDrug, UserItem) then
    begin
      UserItem.ItemFrom.ItemForm := ifSysGive;
      UserItem.ItemFrom.sMakerName := m_sCharName;
      UserItem.ItemFrom.DateTime := Now();

      m_ItemList.Add(UserItem);
    end
    else
      Dispose(UserItem);

    New(UserItem);
    if UserEngine.CopyToUserItemFromName(g_Config.sHeroWoodenSword, UserItem) then
    begin
      UserItem.ItemFrom.ItemForm := ifSysGive;
      UserItem.ItemFrom.sMakerName := m_sCharName;
      UserItem.ItemFrom.DateTime := Now();

      m_ItemList.Add(UserItem);
    end
    else
      Dispose(UserItem);

    New(UserItem);

    if m_btGender = 0 then
      sItem := g_Config.sHeroClothsMan
    else
      sItem := g_Config.sHeroClothsWoman;

    if UserEngine.CopyToUserItemFromName(sItem, UserItem) then
    begin
      UserItem.ItemFrom.ItemForm := ifSysGive;
      UserItem.ItemFrom.sMakerName := m_sCharName;
      UserItem.ItemFrom.DateTime := Now();

      m_ItemList.Add(UserItem);
    end
    else
      Dispose(UserItem);

    // 检查背包中的物品是否合法
    if m_ItemList.Count > 0 then
    begin
      for I := m_ItemList.Count - 1 downto 0 do
      begin
        if m_ItemList.Count <= 0 then
          Break;
        UserItem := m_ItemList.Items[I];

        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if StdItem = nil then
        begin
          m_ItemList.Delete(I);
          Dispose(UserItem);
        end
        else
        begin
          if (StdItem.Need = 103) and (UserItem.boStartTime) then
            SysMsg(Format('您的限时物品[%s]开始计时，有效时间%d分钟。', [StdItem.Name, StdItem.NeedLevel]), c_Red, t_System)
          else if StdItem.Need = 104 then
          begin
            if MinutesBetween(Now, UserItem.ItemFrom.DateTime) >= UserItem.nLimitTime then
            begin
              if StdItem <> nil then
              begin
                SysMsg(Format('限时物品[%s]已到期！', [StdItem.Name]), c_Red, t_System);

                if (StdItem.NeedIdentify = 1) then
                begin
                  AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, UserItem.MakeIndex, '0', 0, 0, '限时物品到期');
                end;

                if (g_FunctionNPC <> nil) then
                begin
                  if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
                  begin
                    TPlayObject(m_Master).m_nScriptGotoCount := 0;
                    m_sExpiredItemName := StdItem.Name;
                    g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroItemExpired', False);
                    m_sExpiredItemName := '';
                  end;
                end;
              end;

              m_ItemList.Delete(I);
              Dispose(UserItem);
            end
            else
            begin
              SysMsg(Format('您的限时物品[%s]开始计时，到期时间%s', [StdItem.Name, GetIncMinuteTime(UserItem.ItemFrom.DateTime,
                StdItem.NeedLevel)]), c_Red, t_System)
            end;
          end
        end;
      end;
    end;
  end;

  // 这里单独加一个，发送给主人的，不然有可能导致SM_HEROLOGON_OK跑到了SM_MYHEROLOGON后面，图标不显示  chongchong 2017-11-30
  SendMsg(Self, RM_MYHEROLOGON, MakeWord(m_btDirection, m_btGender), NativeInt(Self), m_nCurrX, m_nCurrY, '');

  SendRefMsg(RM_HEROLOGON, MakeWord(m_btDirection, m_btGender), NativeInt(Self), m_nCurrX, m_nCurrY, '');
  FLastDir := m_btDirection;

  {
    if (g_ManageNPC <> nil) and (m_Master <> nil) then
    begin
    g_ManageNPC.GotoLable(TPlayObject(m_Master), '@HeroLogin', False);
    end;
  }

  m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] := 0; // 除去魔法盾防御
  m_dwStatusArrTick[STATE_BUBBLEDEFENCEUP] := 0; // 除去魔法盾防御

  m_wStatusTimeArr[STATE_NEWHITBUBBLEDEFENCEUP] := 0; // 除去新武力防御
  m_dwStatusArrTick[STATE_NEWHITBUBBLEDEFENCEUP] := 0; // 除去新武力防御

  m_wStatusTimeArr[STATE_NEWMAGBUBBLEDEFENCEUP] := 0; // 除去新道力防御
  m_dwStatusArrTick[STATE_NEWMAGBUBBLEDEFENCEUP] := 0; // 除去新道力防御

  if g_Config.boSkill15OfflineClear then
  begin
    m_wStatusTimeArr[STATE_DEFENCEUP] := 0;
    m_wStatusTimeArr[STATE_MAGDEFENCEUP] := 0;
  end;

  m_nCharStatus := GetCharStatus();

  RecalcLevelAbilitys(False);
  RecalcAbilitys;

  m_Abil.MaxExp := GetLevelExp(m_Abil.Level);

  if m_boTrainingNG then
  begin
    m_AbilNG.MaxExp := GetLevelExpNG(m_AbilNG.Level);
    RecalcLevelAbilitysNG()
  end;

  SendMsg(Self, RM_ABILITY, 0, 0, 0, 0, '');
  SendMsg(Self, RM_SUBABILITY, 0, 0, 0, 0, '');
  SendMsg(Self, RM_QUERYBAGITEMS, 0, 0, 0, 0, '');
  SendMsg(Self, RM_SENDUSEITEMS, 0, 0, 0, 0, '');
  SendMsg(Self, RM_SENDMYMAGIC, 0, 0, 0, 0, '');

  SendMsg(Self, RM_HEROLOGON_OK, 0, 0, 0, 0, '');
  SendMsg(Self, RM_TRAININGNG, NativeInt(Self), Integer(m_boTrainingNG), Integer(m_boTrainingXF), 1, '');
  // 是否修炼内功心法 界面相应显示内功心法界面  series=0 人物 series=1 英雄
  SendMsg(Self, RM_ABILITYALCOHOL, 0, 0, 0, 0, ''); // 酒属性

  if m_boTrainingNG then
  begin
    SendMsg(Self, RM_ABILITYNG, 0, 0, 0, 0, ''); // 内功属性
    SendMsg(Self, RM_ABILITYMERIDIANS, 0, 0, 0, 0, ''); // 经络
    SendMsg(Self, RM_CONTINUOUSMAGICORDER, 0, 0, 0, 0, ''); // 连击顺序
  end;

  SendMsg(Self, RM_UPDATEJEWELRYBOX, 0, 0, 0, 0, ''); // 首饰盒 chongchong 2013-10-20
  SendMsg(Self, RM_UPDATEGODBLESS, 0, 0, 0, 0, ''); // 更新神佑袋 chongchong 2013-10-20
  SendMsg(Self, RM_FENGHAO, 0, 0, 0, 0, ''); // 更新称号 chongchong 2014-05-26

  RefShowName();

  if g_Config.boHeroCalcWeaponSpeed then
    RefGameSpeed();

  RefAbilNH(); // 刷新内力 让别人看到
  WeightChanged;
  if m_boDummyObject then
    m_btAttackMode := 0;

  if m_btAttackMode > 3 then
    m_btAttackMode := 0;
  m_boSlaveRelax := m_btAttackMode = 2;

  case m_btAttackMode of
    0:
      S := g_sHeroAttack;
    1:
      S := g_sHeroFollow;
    2:
      S := g_sHeroRest;
    3:
      S := g_sHeroFollowAttack;
  end;

  if m_btAttackMode in [0 .. 3] then
  begin
    SendMsg(Self, RM_HERO_ATTACK_MODE, m_btAttackMode, MakeWord(255, 252), 0, 0, S);
  end;

  m_btNewServer := 0;

  { TODO -ochongchong -c新增 : 英雄忠诚度 - 召唤增加 【2013-08-13】 }
  m_rLoyalPoint := Min(100, m_rLoyalPoint + g_Config.dwHeroFealtyCallAdd / 100);
  SendLoyalPoint;

  // 英雄登录快捷键提示 -- piaoyun 2013-07-14
  SysMsg(g_sHeroLoginMsg, c_Green, t_Hint);

  // 英雄登录是否允许召唤宝宝提示 -- chongchong 2013-08-10
  if Self.m_btJob = JOB_TAOS then
  begin
    if g_Config.boHeroCanCallBB then
      SysMsg(g_sHeroOnMakeSlave, c_Green, t_Hint)
    else
      SysMsg(g_sHeroOffMakeSlave, c_Green, t_Hint);
  end;

  // 修复新创建的英雄首次登录给装备无法放入到包裹 chongchong 2013-11-11
  RefBagItemCount(False);
  if (g_ManageNPC <> nil) and (m_Master <> nil) then
  begin
    g_ManageNPC.GotoLable(TPlayObject(m_Master), '@HeroLogin', False);
  end;

  if not g_Config.boSaveRevivalTime then
    m_nCheckRevivalTime := g_Config.dwRevivalTime div 1000;

  if m_boSaveKillMonExpRate and (m_nKillMonExpRate <> 100) then
  begin
    if m_dwKillMonExpRateTime = 0 then
    begin
      if Length(g_sKillMonExpRateForeverMsg) > 0 then
      begin
        S := StringReplace(g_sKillMonExpRateForeverMsg, '%g', FloatToStr(m_nKillMonExpRate / 100), [rfReplaceAll]);
        SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint)
      end;
    end
    else
    begin
      // SysMsg(Format(g_sKillMonExpRateMsg, [m_nKillMonExpRate / 100, m_dwKillMonExpRateTime]), 255, 249, t_Hint)
      if Length(g_sKillMonExpRateMsg) > 0 then
      begin
        S := StringReplace(g_sKillMonExpRateMsg, '%g', FloatToStr(m_nKillMonExpRate / 100), [rfReplaceAll]);
        S := StringReplace(S, '%d', IntToStr(m_dwKillMonExpRateTime), [rfReplaceAll]);

        SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
      end;
    end;
  end;

  if m_boAttackHumSavePowerRate and (m_nAttackHumPowerRate <> 100) and m_boAttackMonSavePowerRate and
    (m_nAttackMonPowerRate <> 100) and (m_nAttackHumPowerRate = m_nAttackMonPowerRate) and
    (m_dwAttackHumPowerRateTime = m_dwAttackMonPowerRateTime) then
  begin
    if m_dwAttackHumPowerRateTime = 0 then
    begin
      // SysMsg(Format(g_sChangePowerRateForeverMsg, [m_nPowerRate / 100]), g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint)

      if Length(g_sChangePowerRateForeverMsg) > 0 then
      begin
        S := StringReplace(g_sChangePowerRateForeverMsg, '%g', FloatToStr(m_nAttackHumPowerRate / 100), [rfReplaceAll]);
        SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint)
      end;
    end
    else
    begin
      // SysMsg(Format(g_sChangePowerRateMsg, [m_nPowerRate / 100, m_dwPowerRateTime]), g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint)

      if Length(g_sChangePowerRateMsg) > 0 then
      begin
        S := StringReplace(g_sChangePowerRateMsg, '%g', FloatToStr(m_nAttackHumPowerRate / 100), [rfReplaceAll]);
        S := StringReplace(S, '%d', IntToStr(m_dwAttackHumPowerRateTime), [rfReplaceAll]);

        SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
      end;
    end;
  end
  else
  begin
    if m_boAttackHumSavePowerRate and (m_nAttackHumPowerRate <> 100) then
    begin
      if m_dwAttackHumPowerRateTime = 0 then
      begin
        if Length(g_sChangeAttackHumPowerRateForeverMsg) > 0 then
        begin
          S := StringReplace(g_sChangeAttackHumPowerRateForeverMsg, '%g', FloatToStr(m_nAttackHumPowerRate / 100),
            [rfReplaceAll]);
          SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint)
        end;
      end
      else
      begin
        if Length(g_sChangeAttackHumPowerRateMsg) > 0 then
        begin
          S := StringReplace(g_sChangeAttackHumPowerRateMsg, '%g', FloatToStr(m_nAttackHumPowerRate / 100), [rfReplaceAll]);
          S := StringReplace(S, '%d', IntToStr(m_dwAttackHumPowerRateTime), [rfReplaceAll]);

          SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
        end;
      end;
    end;

    if m_boAttackMonSavePowerRate and (m_nAttackMonPowerRate <> 100) then
    begin
      if m_dwAttackMonPowerRateTime = 0 then
      begin
        if Length(g_sChangeAttackMonPowerRateForeverMsg) > 0 then
        begin
          S := StringReplace(g_sChangeAttackMonPowerRateForeverMsg, '%g', FloatToStr(m_nAttackMonPowerRate / 100),
            [rfReplaceAll]);
          SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint)
        end;
      end
      else
      begin
        if Length(g_sChangeAttackMonPowerRateMsg) > 0 then
        begin
          S := StringReplace(g_sChangeAttackMonPowerRateMsg, '%g', FloatToStr(m_nAttackMonPowerRate / 100), [rfReplaceAll]);
          S := StringReplace(S, '%d', IntToStr(m_dwAttackMonPowerRateTime), [rfReplaceAll]);

          SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
        end;
      end;
    end;
  end;

  if m_boSaveKillMonBurstRate and (m_dwKillMonBurstRate <> 0) then
  begin
    if m_dwKillMonBurstRateTime = 0 then
    begin
      if Length(g_sKillMonBurstRateForeverMsg) > 0 then
        SysMsg(Format(g_sKillMonBurstRateForeverMsg, [m_dwKillMonBurstRate / 100]), 255, 249, t_Hint);
    end
    else
    begin
      if Length(g_sKillMonBurstRateMsg) > 0 then
        SysMsg(Format(g_sKillMonBurstRateMsg, [m_dwKillMonBurstRate / 100, m_dwKillMonBurstRateTime]), 255, 249, t_Hint);
    end;
  end;

  if m_dwHighLevelKillMonFixExpTime > 0 then
  begin
    SysMsg(Format(g_sHighLevelKillMonFixExpMsg, [m_dwHighLevelKillMonFixExpTime]), g_Config.btGreenMsgFColor,
      g_Config.btGreenMsgBColor, t_Hint)
  end;

  { SysMsg('状态更改：Ctrl+E', c_Red, t_Hint);
    SysMsg('指定攻击目标：Ctrl+W', c_Red, t_Hint);
    SysMsg(Format('守护位置：Ctrl+Q (英雄人物达到%d级后方可使用)', [g_Config.nNeedGuardLevel]), c_Red, t_Hint);
    SysMsg('使用合击技：Ctrl+S (学会合击技方可使用)', c_Red, t_Hint); }
end;

procedure THeroObject.LogOut();
var
  ErrCode: Integer;
begin
  ErrCode := 1;
  try
    if m_boLogOut then
      Exit;
    m_boLogOut := True;

    ErrCode := 2;
    { TODO -ochongchong -c新增 : 英雄忠诚度 - 召回减少 【2013-08-13】 }
    m_rLoyalPoint := Max(0, m_rLoyalPoint - g_Config.dwHeroFealtyCallBackDel / 100);
    SendLoyalPoint;

    ErrCode := 3;
    // 英雄退出提示 -- piaoyun 2013-07-14
    SysMsg(g_sHeroClose { 神奇的力量散去，你的英雄开始沉睡。 } , c_Green, t_Hint);
    SendRefMsg(RM_HEROLOGOUT, 0, NativeInt(Self), m_nCurrX, m_nCurrY, '');
    // m_Master.SendUpdateMsg(Self, RM_HEROLOGOUT, 0, NativeInt(Self), m_nCurrX, m_nCurrY, '');
    // SendMsg(Self, RM_MAKEGHOST, 0, 0, 0, 0, '');
    SendDefMessage(SM_HEROLOGOUT_OK, NativeInt(Self), 0, 0, 0, '');

    ErrCode := 4;
    MakeGhost;

    ErrCode := 5;
    if (g_ManageNPC <> nil) and (m_Master <> nil) then
    begin
      g_ManageNPC.GotoLable(TPlayObject(m_Master), '@HeroLogOut', False);
    end;

  except
    MainOutMessage('[Exception] THeroObject:LogOut, Code = ' + IntToStr(ErrCode));
  end;
end;

procedure THeroObject.MakeGhost;
var
  I: Integer;
  UserItem: pTUserItem;
  ErrCode: Integer;
begin
  ErrCode := 1;
  try
    if (m_Master <> nil) then
    begin
      ErrCode := 2;
      // 爆出装备的物品
      for I := Low(THumanUseItems) to High(THumanUseItems) do
      begin
        if (m_UseItems[I].wIndex > 0) and g_ItemRules.Get(m_UseItems[I].wIndex, 22) then
        begin
          if DropItemDown(@m_UseItems[I], 2, True, nil, Self) then
          begin
            m_UseItems[I].wIndex := 0;
          end;
        end;
      end;

      ErrCode := 2;
      for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
      begin
        if (m_JewelryBoxItems[I].wIndex > 0) and g_ItemRules.Get(m_JewelryBoxItems[I].wIndex, 22) then
        begin
          if DropItemDown(@m_JewelryBoxItems[I], 2, True, nil, Self) then
          begin
            m_JewelryBoxItems[I].wIndex := 0;
          end;
        end;
      end;

      ErrCode := 3;
      for I := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
      begin
        if (m_GodBlessItems[I].wIndex > 0) and g_ItemRules.Get(m_GodBlessItems[I].wIndex, 22) then
        begin
          if DropItemDown(@m_GodBlessItems[I], 2, True, nil, Self) then
          begin
            m_GodBlessItems[I].wIndex := 0;
          end;
        end;
      end;

      ErrCode := 4;
      // 爆出包裹的物品
      for I := m_ItemList.Count - 1 downto 0 do
      begin
        UserItem := m_ItemList.Items[I];
        if g_ItemRules.Get(UserItem.wIndex, 22) then
        begin
          if DropItemDown(UserItem, 2, True, nil, Self) then
          begin
            m_ItemList.Delete(I);
            Dispose(UserItem);
          end;
        end;
      end;

      ErrCode := 5;
      TPlayObject(m_Master).m_MyHero := nil;
      if m_boIsDeputy then
        TPlayObject(m_Master).m_dwRecallDeputyHeroTick := MyGetTickCount
      else
        TPlayObject(m_Master).m_dwRecallHeroTick := MyGetTickCount;

      ErrCode := 6;
      // 英雄死亡后还有血量和图标(如果英雄和主人不在同一屏幕就不会给主人发消息，这里单独发一次)chongchong 2013-10-08
      SendDefMessage(SM_DISAPPEARMYHERO, NativeInt(Self), 0, 0, 0, '');
    end;
    ErrCode := 7;
    m_Master := nil;

    ErrCode := 8;
    inherited;
  except
    MainOutMessage('[Exception] THeroObject:MakeGhost, Code = ' + IntToStr(ErrCode));
  end;
end;

procedure THeroObject.RestHero();
var
  I: Integer;
  Obj: TBaseObject;
  boFound: Boolean;
  S: string;
begin
  if g_nKey_HeroExt = 1 then
  begin
    if m_btAttackMode >= 3 then
      m_btAttackMode := 0
    else
      Inc(m_btAttackMode);
    m_boSlaveRelax := m_btAttackMode = 2;
    m_boProtectStatus := False;

    boFound := False;
    for I := 0 to High(g_Config.boHeroStatus) do
    begin
      if g_Config.boHeroStatus[m_btAttackMode] then
      begin
        boFound := True;
        Break;
      end
      else
      begin
        Inc(m_btAttackMode);
        if m_btAttackMode > High(g_Config.boHeroStatus) then
          m_btAttackMode := 0;
      end;
    end;

    if not boFound then
      m_btAttackMode := 0;
  end
  else
  begin
    if m_btAttackMode >= 2 then
      m_btAttackMode := 0
    else
      Inc(m_btAttackMode);
    m_boSlaveRelax := m_btAttackMode = 2;
    m_boProtectStatus := False;
  end;

  case m_btAttackMode of
    0:
      begin
        S := g_sHeroAttack;
      end;
    1:
      begin
        // 英雄跟随时宝宝不打怪 chongchong 2015-05-17
        for I := 0 to m_SlaveList.Count - 1 do
        begin
          Obj := m_SlaveList[I];
          if Obj.m_TargetCret <> nil then
            Obj.DelTargetCreat;
        end;
        S := g_sHeroFollow;
      end;
    2:
      begin
        // 英雄休息时宝宝不打怪 chongchong 2015-05-17
        for I := 0 to m_SlaveList.Count - 1 do
        begin
          Obj := m_SlaveList[I];
          if Obj.m_TargetCret <> nil then
            Obj.DelTargetCreat;
        end;

        S := g_sHeroRest;
      end;
    3:
      begin
        S := g_sHeroFollowAttack;
      end;
  end;

  if m_btAttackMode in [0 .. 3] then
  begin
    SendMsg(Self, RM_HERO_ATTACK_MODE, m_btAttackMode, MakeWord(255, 252), 0, 0, S);
  end;
end;

procedure THeroObject.SendDelItemList(Items: string; ItemsCount: Integer);
begin
  m_DefMsg := MakeDefaultMsg(SM_HERODELITEMS, 0, ItemsCount, 0, 0);
  SendSocket(@m_DefMsg, EncodeString(Items));
end;

procedure THeroObject.ScatterBagItems(ItemOfCreat: TBaseObject; KillMe: TBaseObject);
var
  I, DropWide, nRate, nDelCount: Integer;
  pu: pTUserItem;
  DelItems: string;
  boDropall: Boolean;
  StdItem: pTStdItem;
resourcestring
  sExceptionMsg = '[Exception] THeroObject.ScatterBagItems';
begin
  if m_boAngryRing or m_boNoDropItem or (m_boDummyObject and (not m_boDropBagItem)) then
    Exit; // 不死戒指

  DelItems := '';
  nDelCount := 0;

  boDropall := False;
  DropWide := 2;
  if g_Config.boDieRedScatterHeroBagAll and (PKLevel >= 2) then
  begin
    boDropall := True;
  end;

  nRate := g_Config.nDieScatterHeroBagRate { 3 };
  if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
  begin // 增加爆率
    nRate := _MAX(nRate - nRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
  end;

  // 非红名掉1/3 //红名全掉
  try
    for I := m_ItemList.Count - 1 downto 0 do
    begin
      if m_ItemList.Count <= 0 then
        Break;
      pu := pTUserItem(m_ItemList[I]);
      StdItem := UserEngine.GetStdItem(pu.wIndex);
      if StdItem = nil then
        Continue;
      if (GetUserItemBindValue(pu, ubNoScatter) and pu.boIsBind) or g_ItemRules.Get(pu.wIndex, 13) then
        Continue; // 禁止爆出
      // 死亡必爆
      if g_ItemRules.Get(pu.wIndex, 6) then
      begin
        // 防暴几率 chongchong 2015-04-16
        if Random(100) < pu.btNewValue[12] then
          Continue;
      end;

      if boDropall or (Random(nRate { 3 } ) = 0) or g_ItemRules.Get(pu.wIndex, 6) then
      begin
        if pu.wInsuranceCount > 0 then
        begin
          Dec(pu.wInsuranceCount);
          SendUpdateItemInsuranceCount(-1, pu.MakeIndex, pu.wInsuranceCount);

          m_sDropInsuranceItemName := StdItem.Name;
          m_nDropInsuranceItemCount := pu.wInsuranceCount;
          m_nDropInsuranceItemCurrency := StdItem.InsuranceCurrency;
          m_nDropInsuranceItemGold := StdItem.InsuranceGold;

          g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroDropInsuranceItem', False);

          Continue;
        end;

        if DropItemDown(pTUserItem(m_ItemList[I]), DropWide, True, ItemOfCreat, Self) then
        begin
          if StdItem <> nil then
          begin
            if (pu.btValue[13] = 1) and (pu.Name <> '') then
              DelItems := DelItems + Format('%s/%d/', [pu.Name, pu.MakeIndex])
            else
              DelItems := DelItems + Format('%s/%d/', [StdItem.Name, pu.MakeIndex]);
            Inc(nDelCount)
          end;
          m_ItemList.Delete(I);
          Dispose(pu); // 修改
        end;
      end;
    end;

    if DelItems <> '' then
      SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

procedure THeroObject.DropUseItems(BaseObject: TBaseObject);
var
  I: Integer;
  nRate, nNewRate, nDelCount: Integer;
  StdItem: pTStdItem;
  DelItems: string;
  Castle: TUserCastle; // 攻城区域
resourcestring
  sExceptionMsg = '[Exception] THeroObject.DropUseItems';
begin
  DelItems := '';
  nDelCount := 0;
  try
    if m_PEnvir.m_boNoDropItem then
      Exit; // 地图禁止死亡掉物品 piaoyun 2013-07-25
    if m_boAngryRing or m_boNoDropUseItem or m_PEnvir.m_boNODROPUSEITEMS or (m_boDummyObject and (not m_boDropUseItem)) then
      Exit;

    // 攻城区域不掉装备 piaoyun 2013-07-25
    if g_Config.boWarNoDropUseItem and (Master <> nil) then
    begin
      // 检测攻城区域
      Castle := g_CastleManager.InCastleWarArea(Master);
      if (Castle <> nil) and Castle.m_boUnderWar { 攻城 } then
        Exit;
    end;

    for I := Low(THumanUseItems) to High(THumanUseItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
      if StdItem <> nil then
      begin
        if StdItem.Reserved and 8 <> 0 then
        begin
          if (m_UseItems[I].btValue[13] = 1) and (m_UseItems[I].Name <> '') then
            DelItems := DelItems + Format('%s/%d/', [m_UseItems[I].Name, m_UseItems[I].MakeIndex])
          else
            DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_UseItems[I].MakeIndex]);

          if StdItem.NeedIdentify = 1 then
          begin
            AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, m_UseItems[I].MakeIndex, '0', 0,
              StdItem.Reserved, '掉落-Reserved');
          end;

          m_UseItems[I].wIndex := 0;
          Inc(nDelCount);
        end;
      end;
    end;

    if m_boDummyObject then
      nRate := m_nDieDropUseItemRate
    else
    begin
      if PKLevel > 2 then
        nRate := g_Config.nDieRedDropHeroUseItemRate
      else
        nRate := g_Config.nDieDropHeroUseItemRate;
    end;

    if not g_Config.boDropUseItem then
    begin
      if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
      begin // 增加爆率
        nRate := _MAX(nRate - nRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
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
      // 死亡必爆
      if g_ItemRules.Get(m_UseItems[I].wIndex, 6) then
      begin
        if DropItemDown(@m_UseItems[I], 2, True, BaseObject, Self) then
        begin
          StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
          if StdItem <> nil then
          begin
            if StdItem.Reserved and 10 = 0 then
            begin
              if (m_UseItems[I].btValue[13] = 1) and (m_UseItems[I].Name <> '') then
                DelItems := DelItems + Format('%s/%d/', [m_UseItems[I].Name, m_UseItems[I].MakeIndex])
              else
                DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_UseItems[I].MakeIndex]);

              m_UseItems[I].wIndex := 0;
              Inc(nDelCount);
            end;
          end;
        end;

        Continue;
      end;

      // 防暴几率 chongchong 2015-04-16
      if Random(100) < m_UseItems[I].btNewValue[12] then
        Continue;

      if g_Config.boDropUseItem then
      begin
        if PKLevel > 2 then
          nNewRate := Round(g_Config.DieDropUseItemRates[I] / (g_Config.nDieRedDropUseItemOneRate / 10))
        else
          nNewRate := g_Config.DieDropUseItemRates[I];

        nNewRate := nNewRate + nRate;

        if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
        begin // 增加爆率
          nNewRate := Max(nNewRate - nNewRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
        end;

        if (Random(nNewRate) <> 0) then
          Continue;
      end
      else
      begin
        if (Random(nRate) <> 0) then
          Continue;
      end;

      if InDisableTakeOffList(m_UseItems[I].wIndex) or (GetUserItemBindValue(@m_UseItems[I], ubNoTakeOff) and
        m_UseItems[I].boIsBind) then
        Continue; // 检查是否在禁止取下列表,如果在列表中则不掉此物品

      if m_UseItems[I].wInsuranceCount > 0 then
      begin
        Dec(m_UseItems[I].wInsuranceCount);
        SendUpdateItemInsuranceCount(I, m_UseItems[I].MakeIndex, m_UseItems[I].wInsuranceCount);

        m_sDropInsuranceItemName := StdItem.Name;
        m_nDropInsuranceItemCount := m_UseItems[I].wInsuranceCount;
        m_nDropInsuranceItemCurrency := StdItem.InsuranceCurrency;
        m_nDropInsuranceItemGold := StdItem.InsuranceGold;

        g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroDropInsuranceItem', False);

        Continue;
      end;

      if m_PEnvir.m_boDELDROPITEM then
      begin
        StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
        if StdItem <> nil then
        begin
          if StdItem.Reserved and 10 = 0 then
          begin
            DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_UseItems[I].MakeIndex]);
            m_UseItems[I].wIndex := 0;
            Inc(nDelCount);
          end;
        end;
      end
      else
      begin
        if DropItemDown(@m_UseItems[I], 2, True, BaseObject, Self) then
        begin
          StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
          if StdItem <> nil then
          begin
            if StdItem.Reserved and 10 = 0 then
            begin
              if (m_UseItems[I].btValue[13] = 1) and (m_UseItems[I].Name <> '') then
                DelItems := DelItems + Format('%s/%d/', [m_UseItems[I].Name, m_UseItems[I].MakeIndex])
              else
                DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_UseItems[I].MakeIndex]);

              Inc(nDelCount);
            end;
          end;
          m_UseItems[I].wIndex := 0;
        end;
      end;
    end;

    if DelItems <> '' then
      SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

procedure THeroObject.DropJewelryBoxItems(BaseObject: TBaseObject);
var
  I: Integer;
  nRate, nNewRate, nDelCount: Integer;
  StdItem: pTStdItem;
  DelItems: string;
resourcestring
  sExceptionMsg = '[Exception] THeroObject.DropUseItems';
begin
  DelItems := '';
  nDelCount := 0;
  try
    if m_boAngryRing or m_boNoDropUseItem or m_PEnvir.m_boNODROPUSEITEMS or (m_boDummyObject and (not m_boDropUseItem)) then
      Exit;
    for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_JewelryBoxItems[I].wIndex);
      if StdItem <> nil then
      begin
        if StdItem.Reserved and 8 <> 0 then
        begin
          if (m_JewelryBoxItems[I].btValue[13] = 1) and (m_JewelryBoxItems[I].Name <> '') then
            DelItems := DelItems + Format('%s/%d/', [m_JewelryBoxItems[I].Name, m_JewelryBoxItems[I].MakeIndex])
          else
            DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_JewelryBoxItems[I].MakeIndex]);

          if StdItem.NeedIdentify = 1 then
          begin
            AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, m_JewelryBoxItems[I].MakeIndex, '0', 0,
              StdItem.Reserved, '掉落-Reserved');
          end;

          m_JewelryBoxItems[I].wIndex := 0;
          Inc(nDelCount);
        end;
      end;
    end;

    if m_boDummyObject then
      nRate := m_nDieDropUseItemRate
    else
    begin
      if PKLevel > 2 then
        nRate := g_Config.nDieRedDropHeroUseItemRate
      else
        nRate := g_Config.nDropHeroJewelryBoxItemRate;
    end;

    if not g_Config.boDropUseItem then
    begin
      if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
      begin // 增加爆率
        nRate := _MAX(nRate - nRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
      end;
    end;

    for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_JewelryBoxItems[I].wIndex);
      if StdItem = nil then
        Continue;
      if (GetUserItemBindValue(@m_JewelryBoxItems[I], ubNoScatter) and m_JewelryBoxItems[I].boIsBind) or
        g_ItemRules.Get(m_JewelryBoxItems[I].wIndex, 13) then
        Continue; // 禁止爆出

      if g_ItemRules.Get(m_JewelryBoxItems[I].wIndex, 6) then
      begin
        if DropItemDown(@m_JewelryBoxItems[I], 2, True, BaseObject, Self) then
        begin
          StdItem := UserEngine.GetStdItem(m_JewelryBoxItems[I].wIndex);
          if StdItem <> nil then
          begin
            if StdItem.Reserved and 10 = 0 then
            begin
              if (m_JewelryBoxItems[I].btValue[13] = 1) and (m_JewelryBoxItems[I].Name <> '') then
                DelItems := DelItems + Format('%s/%d/', [m_JewelryBoxItems[I].Name, m_JewelryBoxItems[I].MakeIndex])
              else
                DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_JewelryBoxItems[I].MakeIndex]);

              m_JewelryBoxItems[I].wIndex := 0;
              Inc(nDelCount);
            end;
          end;
        end;

        Continue;
      end;

      // 防暴几率 chongchong 2015-04-16
      if Random(100) < m_JewelryBoxItems[I].btNewValue[12] then
        Continue;

      if g_Config.boDropUseItem then
      begin
        if PKLevel > 2 then
          nNewRate := Round(g_Config.DieDropUseItemRates[I] / (g_Config.nDieRedDropUseItemOneRate / 10))
        else
          nNewRate := g_Config.DieDropUseItemRates[I];

        nNewRate := nNewRate + nRate;

        if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
        begin // 增加爆率
          nNewRate := Max(nNewRate - nNewRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
        end;

        if (Random(nNewRate) <> 0) then
          Continue;
      end
      else
      begin
        if (Random(nRate) <> 0) then
          Continue;
      end;

      if InDisableTakeOffList(m_JewelryBoxItems[I].wIndex) or (GetUserItemBindValue(@m_JewelryBoxItems[I], ubNoTakeOff) and
        m_JewelryBoxItems[I].boIsBind) then
        Continue; // 检查是否在禁止取下列表,如果在列表中则不掉此物品

      if m_JewelryBoxItems[I].wInsuranceCount > 0 then
      begin
        Dec(m_JewelryBoxItems[I].wInsuranceCount);
        SendUpdateItemInsuranceCount(U_JEWELRYITEM1 + I, m_JewelryBoxItems[I].MakeIndex, m_JewelryBoxItems[I].wInsuranceCount);

        m_sDropInsuranceItemName := StdItem.Name;
        m_nDropInsuranceItemCount := m_JewelryBoxItems[I].wInsuranceCount;
        m_nDropInsuranceItemCurrency := StdItem.InsuranceCurrency;
        m_nDropInsuranceItemGold := StdItem.InsuranceGold;

        g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroDropInsuranceItem', False);

        Continue;
      end;

      if m_PEnvir.m_boDELDROPITEM then
      begin
        StdItem := UserEngine.GetStdItem(m_JewelryBoxItems[I].wIndex);
        if StdItem <> nil then
        begin
          if StdItem.Reserved and 10 = 0 then
          begin
            DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_JewelryBoxItems[I].MakeIndex]);
            m_JewelryBoxItems[I].wIndex := 0;
            Inc(nDelCount);
          end;
        end;
      end
      else
      begin
        if DropItemDown(@m_JewelryBoxItems[I], 2, True, BaseObject, Self) then
        begin
          StdItem := UserEngine.GetStdItem(m_JewelryBoxItems[I].wIndex);
          if StdItem <> nil then
          begin
            if StdItem.Reserved and 10 = 0 then
            begin
              if (m_JewelryBoxItems[I].btValue[13] = 1) and (m_JewelryBoxItems[I].Name <> '') then
                DelItems := DelItems + Format('%s/%d/', [m_JewelryBoxItems[I].Name, m_JewelryBoxItems[I].MakeIndex])
              else
                DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_JewelryBoxItems[I].MakeIndex]);

              m_JewelryBoxItems[I].wIndex := 0;
              Inc(nDelCount);
            end;
          end;
        end;
      end;
    end;

    if DelItems <> '' then
      SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

procedure THeroObject.DropGodBlessItems(BaseObject: TBaseObject);
var
  I: Integer;
  nRate, nNewRate, nDelCount: Integer;
  StdItem: pTStdItem;
  DelItems: string;
resourcestring
  sExceptionMsg = '[Exception] THeroObject.DropUseItems';
begin
  DelItems := '';
  nDelCount := 0;
  try
    if m_boAngryRing or m_boNoDropUseItem or m_PEnvir.m_boNODROPUSEITEMS or (m_boDummyObject and (not m_boDropUseItem)) then
      Exit;
    for I := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_GodBlessItems[I].wIndex);
      if StdItem <> nil then
      begin
        if StdItem.Reserved and 8 <> 0 then
        begin
          if (m_GodBlessItems[I].btValue[13] = 1) and (m_GodBlessItems[I].Name <> '') then
            DelItems := DelItems + Format('%s/%d/', [m_GodBlessItems[I].Name, m_GodBlessItems[I].MakeIndex])
          else
            DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_GodBlessItems[I].MakeIndex]);

          if StdItem.NeedIdentify = 1 then
          begin
            AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, m_GodBlessItems[I].MakeIndex, '0', 0,
              StdItem.Reserved, '掉落-Reserved');
          end;
          m_GodBlessItems[I].wIndex := 0;
          Inc(nDelCount);
        end;
      end;
    end;

    if m_boDummyObject then
      nRate := m_nDieDropUseItemRate
    else
    begin
      if PKLevel > 2 then
        nRate := g_Config.nDieRedDropHeroUseItemRate
      else
        nRate := g_Config.nDropHeroGodBlessItemRate;
    end;

    if not g_Config.boDropUseItem then
    begin
      if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
      begin // 增加爆率
        nRate := _MAX(nRate - nRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
      end;
    end;

    for I := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
    begin
      StdItem := UserEngine.GetStdItem(m_GodBlessItems[I].wIndex);
      if StdItem = nil then
        Continue;
      if (GetUserItemBindValue(@m_GodBlessItems[I], ubNoScatter) and m_GodBlessItems[I].boIsBind) or
        g_ItemRules.Get(m_GodBlessItems[I].wIndex, 13) then
        Continue; // 禁止爆出

      if g_ItemRules.Get(m_GodBlessItems[I].wIndex, 6) then
      begin
        if DropItemDown(@m_GodBlessItems[I], 2, True, BaseObject, Self) then
        begin
          StdItem := UserEngine.GetStdItem(m_GodBlessItems[I].wIndex);
          if StdItem <> nil then
          begin
            if StdItem.Reserved and 10 = 0 then
            begin
              if (m_GodBlessItems[I].btValue[13] = 1) and (m_GodBlessItems[I].Name <> '') then
                DelItems := DelItems + Format('%s/%d/', [m_GodBlessItems[I].Name, m_GodBlessItems[I].MakeIndex])
              else
                DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_GodBlessItems[I].MakeIndex]);

              m_GodBlessItems[I].wIndex := 0;
              Inc(nDelCount);
            end;
          end;
        end;
      end;

      // 防暴几率 chongchong 2015-04-16
      if Random(100) < m_GodBlessItems[I].btNewValue[12] then
        Continue;

      if g_Config.boDropUseItem then
      begin
        if PKLevel > 2 then
          nNewRate := Round(g_Config.DieDropUseItemRates[I] / (g_Config.nDieRedDropUseItemOneRate / 10))
        else
          nNewRate := g_Config.DieDropUseItemRates[I];

        nNewRate := nNewRate + nRate;

        if (m_CurrTarget <> nil) and (m_CurrTarget.m_WAbil.NewValue[6] > 0) then
        begin // 增加爆率
          nNewRate := Max(nNewRate - nNewRate * m_CurrTarget.m_WAbil.NewValue[6] div 100, 0);
        end;

        if (Random(nNewRate) <> 0) then
          Continue;
      end
      else
      begin
        if (Random(nRate) <> 0) then
          Continue;
      end;

      if InDisableTakeOffList(m_GodBlessItems[I].wIndex) or (GetUserItemBindValue(@m_GodBlessItems[I], ubNoTakeOff) and
        m_GodBlessItems[I].boIsBind) then
        Continue; // 检查是否在禁止取下列表,如果在列表中则不掉此物品

      if m_GodBlessItems[I].wInsuranceCount > 0 then
      begin
        Dec(m_GodBlessItems[I].wInsuranceCount);
        SendUpdateItemInsuranceCount(U_GODBLESSITEM1 + I, m_GodBlessItems[I].MakeIndex, m_GodBlessItems[I].wInsuranceCount);

        m_sDropInsuranceItemName := StdItem.Name;
        m_nDropInsuranceItemCount := m_GodBlessItems[I].wInsuranceCount;
        m_nDropInsuranceItemCurrency := StdItem.InsuranceCurrency;
        m_nDropInsuranceItemGold := StdItem.InsuranceGold;

        g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroDropInsuranceItem', False);

        Continue;
      end;

      if m_PEnvir.m_boDELDROPITEM then
      begin
        StdItem := UserEngine.GetStdItem(m_GodBlessItems[I].wIndex);
        if StdItem <> nil then
        begin
          if StdItem.Reserved and 10 = 0 then
          begin
            DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_GodBlessItems[I].MakeIndex]);
            m_GodBlessItems[I].wIndex := 0;
            Inc(nDelCount);
          end;
        end;
      end
      else
      begin
        if DropItemDown(@m_GodBlessItems[I], 2, True, BaseObject, Self) then
        begin
          StdItem := UserEngine.GetStdItem(m_GodBlessItems[I].wIndex);
          if StdItem <> nil then
          begin
            if StdItem.Reserved and 10 = 0 then
            begin
              if (m_GodBlessItems[I].btValue[13] = 1) and (m_GodBlessItems[I].Name <> '') then
                DelItems := DelItems + Format('%s/%d/', [m_GodBlessItems[I].Name, m_GodBlessItems[I].MakeIndex])
              else
                DelItems := DelItems + Format('%s/%d/', [StdItem.Name, m_GodBlessItems[I].MakeIndex]);

              m_GodBlessItems[I].wIndex := 0;
              Inc(nDelCount);
            end;
          end;
        end;
      end;
    end;
    if DelItems <> '' then
      SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

procedure THeroObject.SendAddMagic(UserMagic: pTUserMagic);
var
  ClientMagic: TClientMagic;
begin
  if not CanSend then
    Exit;

  UserMagicToClientMagic(UserMagic, @ClientMagic);

  ClientMagic.dwInterval := 0;
  ClientMagic.dwRealInterval := 0;
  ClientMagic.dwLastUseTick := 0;
  m_DefMsg := MakeDefaultMsg(SM_HEROADDMAGIC, 0, 0, 0, 1);
  SendSocketEx(@m_DefMsg, @ClientMagic, SizeOf(TClientMagic));
end;

procedure THeroObject.SendDelMagic(UserMagic: pTUserMagic);
begin
  // 删除技能时记录的技能指针对应也清一下 2020-06-15
  if UserMagic = m_MagicSuperShiledSkill then
    m_MagicSuperShiledSkill := nil
  else if UserMagic = m_MagicOneSwordSkill then
    m_MagicOneSwordSkill := nil
  else if UserMagic = m_MagicPowerHitSkill then
    m_MagicPowerHitSkill := nil
  else if UserMagic = m_MagicErgumSkill then
    m_MagicErgumSkill := nil
  else if UserMagic = m_MagicBanwolSkill then
    m_MagicBanwolSkill := nil
  else if UserMagic = m_MagicFireSwordSkill then
    m_MagicFireSwordSkill := nil
  else if UserMagic = m_MagicCrsSkill then
    m_MagicCrsSkill := nil
  else if UserMagic = m_Magic42Skill then
    m_Magic42Skill := nil
  else if UserMagic = m_Magic43Skill then
    m_Magic43Skill := nil
  else if UserMagic = m_MagicSwordSkill then
    m_MagicSwordSkill := nil
  else if UserMagic = m_Magic66Skill then
    m_Magic66Skill := nil
  else if UserMagic = m_Magic113Skill then
    m_Magic113Skill := nil
  else if UserMagic = m_Magic115Skill then
    m_Magic115Skill := nil;

  if not CanSend then
    Exit;
  m_DefMsg := MakeDefaultMsg(SM_HERODELMAGIC, UserMagic.wMagIdx, 0, 0, 1);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItem(UserItem: pTUserItem);
var
  StdItem: pTStdItem;
  ClientItem: TClientItem;
begin
  if not CanSend then
    Exit;
  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if StdItem <> nil then
  begin
    UserItemToClientItem(UserItem, StdItem, @ClientItem, True, True);

    m_DefMsg := MakeDefaultMsg(SM_HEROUPDATEITEM, NativeInt(Self), 0, 0, 1);
    SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem));
  end;
end;

procedure THeroObject.SendUpdateItemName(nWhere: SmallInt; MakeIndex: Integer; NewItemName: string);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_NAME, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, 0);
  SendSocket(@m_DefMsg, EncodeString(NewItemName));
end;

procedure THeroObject.SendUpdateItemColor(nWhere: SmallInt; MakeIndex: Integer; Color: Byte);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_COLOR, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, Color);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemDura(nWhere: SmallInt; MakeIndex: Integer; Dura: Word);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_DURA, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, Dura);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemDuraMax(nWhere: SmallInt; MakeIndex: Integer; DuraMax: Word);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_DURAMAX, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, DuraMax);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemUpgradeCount(nWhere: SmallInt; MakeIndex: Integer; UpgradeCount: Byte);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_UPGRADECOUNT, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, UpgradeCount);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemNewLook(nWhere: SmallInt; MakeIndex: Integer; NewLook: Word);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_NEWLOOK, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, NewLook);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemNewShape(nWhere: SmallInt; MakeIndex: Integer; NewShape: Word);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_NEWSHAPE, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, NewShape);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemHeroM2Light(nWhere: SmallInt; MakeIndex: Integer; HeroM2Light: Byte);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_HEROM2LIGHT, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, HeroM2Light);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemInsuranceCount(nWhere: SmallInt; MakeIndex: Integer; InsuranceCount: Word);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_InsuranceCount, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, InsuranceCount);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemBind(nWhere: SmallInt; MakeIndex: Integer; IsBind: Boolean);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_BIND, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, Word(IsBind));
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemLimitTime(nWhere: SmallInt; MakeIndex: Integer; LimitTime: Integer);
begin
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_HERO_LIMITTIME, MakeIndex, nWhere, LoWord(LimitTime), HiWord(LimitTime));
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemNewValue(nWhere: SmallInt; MakeIndex: Integer; ValueIndex: Byte; Value: Word);
begin
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_HERO_NEWVALUE, MakeIndex, nWhere, MakeWord(0, ValueIndex), Value);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemFlute(nWhere: SmallInt; UserItem: pTUserItem);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_FLUTE, UserItem.MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, UserItem.btFluteCount);
  SendSocketEx(@m_DefMsg, @UserItem.Flutes[0], SizeOf(UserItem.Flutes));
end;

procedure THeroObject.SendUpdateItemProgress(nWhere: SmallInt; UserItem: pTUserItem; ProgressIndex: Byte);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_PROGRESS, UserItem.MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, ProgressIndex);
  SendSocketEx(@m_DefMsg, @UserItem.Progress[ProgressIndex], SizeOf(UserItem.Progress[ProgressIndex]));
end;

procedure THeroObject.SendUpdateItemPropertyText(nWhere: SmallInt; MakeIndex: Integer; sText: string);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_PROPERTYTEXT, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, 0);
  SendSocket(@m_DefMsg, EncodeString(sText));
end;

procedure THeroObject.SendUpdateItemPropertyColor(nWhere: SmallInt; MakeIndex: Integer; Color: Byte);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_PROPERTYCOLOR, MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, Color);
  SendSocket(@m_DefMsg, '');
end;

procedure THeroObject.SendUpdateItemPropertyValue(nWhere: SmallInt; UserItem: pTUserItem; PropertyIndex: Byte);
var
  IsHero: Boolean;
begin
  IsHero := True;
  m_DefMsg := MakeDefaultMsg(SM_UPDATEITEM_PROPERTYVALUES, UserItem.MakeIndex, nWhere, Integer(IsHero) shl 1 or 0, PropertyIndex);
  SendSocketEx(@m_DefMsg, @UserItem.CustomProperty.Properties[PropertyIndex],
    SizeOf(UserItem.CustomProperty.Properties[PropertyIndex]));
end;

procedure THeroObject.KickException;
begin
  Self.LogOut;
end;

procedure THeroObject.Die; // 英雄死亡
var
  nCode: Integer;
  nDecExp: Integer;
  boPK: Boolean;
  guildwarkill: Boolean;
  ALastHiter: TBaseObject;
  Castle: TUserCastle;
resourcestring
  sExceptionMsg = '[Exception] THeroObject.Die Name:%s Error:%d';
begin
  m_boDeath := True;
  m_dwDeathTick := MyGetTickCount();
  m_nIncSpell := 0;
  m_nIncHealth := 0;
  m_nIncHealing := 0;
  nCode := 0;
  ALastHiter := m_LastHiter;

  try
    if (m_LastHiter <> nil) and (m_LastHiter <> m_Master) then
    begin
      if m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
      begin
        SysMsg(Format(g_sHeroKilledByMsg, [DelNumber(m_LastHiter.m_sCharName)]), c_Red, t_Hint);
      end
      else if (m_LastHiter.m_Master <> nil) and (m_LastHiter.m_Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
      begin
        SysMsg(Format(g_sHeroKilledByMsg, [DelNumber(m_LastHiter.m_Master.m_sCharName)]), c_Red, t_Hint);
      end;

      boPK := False;
      if (not g_Config.boVentureServer) and (not m_PEnvir.m_boFightZone) and (not m_PEnvir.m_boFight2Zone) and
        (not m_PEnvir.m_boFight3Zone) and (not m_PEnvir.m_boFight4Zone) then
      begin
        if (m_LastHiter <> nil) and (PKLevel < 2) then
        begin
          if (m_LastHiter.m_btRaceServer = RC_PLAYOBJECT) or (m_LastHiter.m_btRaceServer = RC_NPC) then
          begin
            { 修改日期2004/07/21，允许NPC杀死人物 }
            boPK := True;
          end;

          if (m_LastHiter.Master <> nil) and (m_LastHiter.Master.m_btRaceServer = RC_PLAYOBJECT) then
          begin
            m_LastHiter := m_LastHiter.Master;
            boPK := True;
          end;
        end;
      end;

      if boPK and (m_LastHiter <> nil) and (m_Master <> nil) then
      begin
        guildwarkill := False;
        if (m_Master.m_MyGuild <> nil) and (m_LastHiter.m_MyGuild <> nil) then
        begin
          if GetGuildRelation(m_Master, m_LastHiter) = 2 then
            guildwarkill := True;
        end;
        Castle := g_CastleManager.InCastleWarArea(m_Master);
        if ((Castle <> nil) and Castle.m_boUnderWar) or (m_boInFreePKArea) then
          guildwarkill := True;

        if (not guildwarkill) and (TPlayObject(m_Master).m_btAttatckMode = HAM_NATION) and
          (TPlayObject(m_LastHiter).m_btAttatckMode = HAM_NATION) and (TPlayObject(m_Master).m_btNation > 0) and
          (TPlayObject(m_LastHiter).m_btNation > 0) and (TPlayObject(m_Master).m_btNation <> TPlayObject(m_LastHiter).m_btNation)
        then // 国战模式杀人不加PK
          guildwarkill := True;

        if (not guildwarkill) then
        begin
          if (not m_LastHiter.IsGoodKilling(Self)) and (ALastHiter <> nil) then
          begin
            // 人物杀英雄，增加人物PK值
            if (ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER]) then
            begin
              { 修改 : 杀英雄武器诅咒机率 【2013-08-17】 }
              if g_Config.boKillHeroWeaponUnlock and (Random(g_Config.dwKillHeroWeaponUnlockRate) = 0) then
                ALastHiter.MakeWeaponUnlock;

              if ALastHiter.m_boDummyObject then
                TSmartObject(ALastHiter).IncPkPoint(g_Config.nDummyAddPKPoint { 100 } )
              else
                TSmartObject(ALastHiter).IncPkPoint(g_Config.nKillHumanAddPKPoint { 100 } );

              if ALastHiter.m_btRaceServer = RC_PLAYOBJECT then
                ALastHiter.SysMsg(g_sYouMurderedMsg { '你犯了谋杀罪！' } , c_Red, t_Hint);
            end
            // 英雄杀英雄，增加杀人英雄的主人PK值
            else if (ALastHiter.m_btRaceServer = RC_HEROOBJECT) and (ALastHiter.m_Master <> nil) then
            begin
              // TSmartObject(ALastHiter).IncPkPoint(g_Config.dwKillHeroAddPKPoint);
              TSmartObject(ALastHiter.m_Master).IncPkPoint(g_Config.dwKillHeroAddPKPoint);

              if ALastHiter.m_Master.m_btRaceServer = RC_PLAYOBJECT then
              begin
                ALastHiter.m_Master.SysMsg(g_sYouMurderedMsg { '你犯了谋杀罪！' } , c_Red, t_Hint);
              end;
            end
            else if g_Config.boSlaveKillHumanIncPK then
            begin
              if ALastHiter.Master <> nil then
              begin
                TSmartObject(ALastHiter.Master).IncPkPoint(g_Config.nKillHumanAddPKPoint { 100 } );
                TSmartObject(ALastHiter.Master).SysMsg(g_sYouMurderedMsg { '你犯了谋杀罪！' } , c_Red, t_Hint);
              end;
            end;
          end
          else
            m_LastHiter.SysMsg(g_sYouProtectedByLawOfDefense { '[你受到正当规则保护。]' } , c_Green, t_Hint);
        end;
      end;

      if (ALastHiter <> nil) then
      begin
        if (g_Config.boKillByHumanDropHeroUseItem and (ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) or
          (g_Config.boKillByMonstDropHeroUseItem and (not(ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]))) then
          DropUseItems(nil);

        if (g_Config.boKillByHumanDropHeroJewelryBoxItem and (ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) or
          (g_Config.boKillByMonstDropHeroJewelryBoxItem and (not(ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])))
        then
          DropJewelryBoxItems(nil);

        if (g_Config.boKillByHumanDropHeroGodBlessItem and (ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) or
          (g_Config.boKillByMonstDropHeroGodBlessItem and (not(ALastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]))) then
          DropGodBlessItems(nil);
      end;
    end;

    { TODO -ochongchong -c添加 : 英雄死亡后掉现有经验的一定比率 【2013-08-11】 }
    nDecExp := Round(m_Abil.Exp * (g_Config.dwHeroDieExpRate / 100));
    if nDecExp > 0 then
      SysMsg(Format(g_sHeroDieDecExp, [nDecExp]), c_Red, t_Hint);

    { TODO -ochongchong -c新增 : 英雄忠诚度 - 死亡降低 【2013-08-13】 }
    m_rLoyalPoint := Max(0, m_rLoyalPoint - g_Config.dwHeroFealtyDeathDel / 100);
    SendLoyalPoint;

    if m_Abil.Exp >= nDecExp then
      Dec(m_Abil.Exp, nDecExp)
    else
      m_Abil.Exp := 0;

    SendRefMsg(RM_DEATH, m_btDirection, m_nCurrX, m_nCurrY, 1, '');
    m_Master.SendUpdateMsg(Self, RM_DEATH, m_btDirection, m_nCurrX, m_nCurrY, 1, '');
    nCode := 2;
    // 执行杀怪触发
    if (not m_PEnvir.m_boFightZone) and (not m_PEnvir.m_boFight3Zone) and (not m_boAnimal) then
    begin
      nCode := 3;
      if g_Config.boDieScatterHeroBag then
        ScatterBagItems(nil, ALastHiter);
      ProcessRulesItems;
    end;

    // 英雄死亡触发 chongchong 2015-09-07
    if (g_FunctionNPC <> nil) and (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
    begin
      TPlayObject(m_Master).m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroDie', False);
    end;

  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg, [m_sCharName, nCode]));
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure THeroObject.ProcessRulesItems;
var
  I: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  sName: string;
  IsSendItems: Boolean;
begin
  IsSendItems := False;
  { 新增 : 物品规则 - 死亡消失【2013-07-27】 }
  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
    StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
    if StdItem = nil then
      Continue;
    sName := StdItem.Name;
    if (m_UseItems[I].wIndex > 0) and (g_ItemRules.Get(m_UseItems[I].wIndex, 18) or (StdItem.Reserved and 8 <> 0)) then
    { TODO : 当仅reserved值为 8 时，死亡时装备消失 2013-08-26 }
    begin
      if (StdItem.NeedIdentify = 1) then
      begin
        AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, m_UseItems[I].MakeIndex, '0', StdItem.Reserved, 0,
          '死亡消失-装备物品');
      end;

      m_UseItems[I].wIndex := 0;
      IsSendItems := True;
    end;
  end;

  if IsSendItems then
    SendUseitems;

  for I := m_ItemList.Count - 1 downto 0 do
  begin
    UserItem := m_ItemList.Items[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if StdItem = nil then
      Continue;
    sName := StdItem.Name;
    if g_ItemRules.Get(UserItem.wIndex, 18) then
    begin
      if (StdItem.NeedIdentify = 1) then
      begin
        AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, UserItem.MakeIndex, '0', StdItem.Reserved, 0,
          '死亡消失-背包物品');
      end;

      m_ItemList.Delete(I);
      Dispose(UserItem);
    end;
  end;
end;

procedure THeroObject.GotoTargetXY;
var
  nTargetX, nTargetY: Integer;
  nTempX, nTempY: Integer;
  nDir1, nDir2: Byte;
begin
  if not m_boCanWalk then
    Exit;

  if ((m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)) then
  begin
    if GotoNearGotoXY(m_nTargetX, m_nTargetY, nTargetX, nTargetY) then
    begin
      nDir1 := GetNextDirection(m_nCurrX, m_nCurrY, nTargetX, nTargetY);

      if ((m_nTargetX <> nTargetX) or (m_nTargetY <> nTargetY)) and GotoNearGotoXY(nTargetX, nTargetY, m_nTargetX, m_nTargetY,
        nTempX, nTempY) then
      begin
        nDir2 := GetNextDirection(nTargetX, nTargetY, nTempX, nTempY);

        // 当前步骤和下一步的方向相反，就是来回搞chongchong 2017-10-26
        if GetDifferenceDirection(nDir1) = nDir2 then
        begin
          m_dwMoveTimeTick := MyGetTickCount;
          Exit;
        end;
      end;

      if not WalkTo(nDir1, False) then
      begin
        nDir2 := (nDir1 + 1) mod 8;
        if not WalkTo(nDir2, False) then
        begin
          nDir2 := (nDir1 - 1) mod 8;
          if not WalkTo(nDir2, False) then
          begin
            WalkTo(Random(8), False);
          end;
        end;
      end;
    end
    else
      RunTo(Random(8), False)
  end;
end;

procedure THeroObject.RunToTargetXY;
var
  nTargetX, nTargetY: Integer;
  I, J, nTempX, nTempY, nToX, nToY: Integer;
  nDir1, nDir2: Byte;
  boMove: Boolean;
  nRunTime: Integer;
  Points: array [0 .. 12, 0 .. 12] of Boolean;
  MapCellInfo: pTMapCellinfo;
  Col, Row: Integer;
  IsChange, IsFound: Boolean;
begin
  nRunTime := GetMoveTime;
  if not m_boCanRun then
    Exit;

  if ((m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)) then
  begin
    if (not FIsTempMoveSet) and (not FIsStopTempMove) then
    begin
      IsChange := (FMovePoints[0].X = FMovePoints[2].X) //
        and (FMovePoints[0].X = FMovePoints[4].X) //
        and (FMovePoints[0].X <> 0) //
        and (FMovePoints[0].Y = FMovePoints[2].Y) //
        and (FMovePoints[0].Y = FMovePoints[4].Y) //
        and (FMovePoints[0].Y <> 0) //
        and (FMovePoints[1].X = FMovePoints[3].X) //
        and (FMovePoints[1].X = FMovePoints[5].X) //
        and (FMovePoints[1].X <> 0) //
        and (FMovePoints[1].Y = FMovePoints[3].Y) //
        and (FMovePoints[1].Y = FMovePoints[5].Y) //
        and (FMovePoints[1].Y <> 0);

      if not IsChange then
      begin
        if (m_TargetCret <> nil) then
          IsChange := (MyGetTickCount - m_TargetCret.m_dwStationTick >= 2000) and
            (MyGetTickCount - m_dwSetTargetCretTick >= 10000)
        else if m_boProtectStatus and (m_nTargetX = m_nProtectTargetX) and (m_nTargetY = m_nProtectTargetY) then
          IsChange := (MyGetTickCount - m_dwProtectTargetTick >= 10000);
      end;

      if IsChange then
      begin
        if MyGetTickCount - m_HeroMoveTick >= nRunTime then
        begin
          for Col := -6 to 6 do
          begin
            for Row := -6 to 6 do
            begin
              Points[Row + 6, Col + 6] := m_PEnvir.GetMapCellInfo(m_nCurrX + Row, m_nCurrY + Col, MapCellInfo) and
                (MapCellInfo.chFlag = 0);
            end;
          end;

          if m_TargetCret <> nil then
          begin
            nToX := m_TargetCret.m_nCurrX;
            nToY := m_TargetCret.m_nCurrY;
          end
          else
          begin
            nToX := m_nTargetX;
            nToY := m_nTargetY;
          end;

          if (nToY > m_nCurrY) or ((nToY = m_nCurrY) and (nToX > m_nCurrX)) then // 门往右下开
          begin
            IsFound := False;
            // 45度往右上尝试
            for I := 1 to 3 do
            begin
              if Points[6 + I, 6 - I] then
              begin
                IsFound := True;
                for J := 1 to 3 do
                begin
                  if not Points[6 + I + J, 6 - I + J] then
                  begin
                    IsFound := False;
                    Break;
                  end;
                end;

                if IsFound then
                  Break;
              end;
            end;

            if IsFound then
            begin
              if I = 1 then
              begin
                if WalkTo(DR_UPRIGHT, False) then
                begin
                  FTempMoveDir0 := DR_UPRIGHT;

                  FIsTempMoveSet := True;
                  FTempMoveDir := DR_DOWNRIGHT;
                  FTempMoveCount := 3;
                  FTempMoveDirTick := MyGetTickCount;

                  Exit;
                end;
              end
              else if I >= 2 then
              begin
                if RunTo(DR_UPRIGHT, False) then
                begin
                  FTempMoveDir0 := DR_UPRIGHT;

                  FIsTempMoveSet := True;
                  FTempMoveDir := DR_DOWNRIGHT;
                  FTempMoveCount := 3;
                  FTempMoveDirTick := MyGetTickCount;

                  Exit;
                end;
              end;
            end
            else
            begin
              IsFound := False;
              // 45度往左下尝试
              for I := 1 to 3 do
              begin
                if Points[6 - I, 6 + I] then
                begin
                  IsFound := True;
                  for J := 1 to 3 do
                  begin
                    if not Points[6 - I + J, 6 + I + J] then
                    begin
                      IsFound := False;
                      Break;
                    end;
                  end;

                  if IsFound then
                    Break;
                end;
              end;

              if IsFound then
              begin
                if I = 1 then
                begin
                  if WalkTo(DR_DOWNLEFT, False) then
                  begin
                    FTempMoveDir0 := DR_DOWNLEFT;

                    FIsTempMoveSet := True;
                    FTempMoveDir := DR_DOWNRIGHT;
                    FTempMoveCount := 3;
                    FTempMoveDirTick := MyGetTickCount;

                    Exit;
                  end;
                end
                else if I >= 2 then
                begin
                  if RunTo(DR_DOWNLEFT, False) then
                  begin
                    FTempMoveDir0 := DR_DOWNLEFT;

                    FIsTempMoveSet := True;
                    FTempMoveDir := DR_DOWNRIGHT;
                    FTempMoveCount := 3;
                    FTempMoveDirTick := MyGetTickCount;

                    Exit;
                  end;
                end;
              end;
            end;
          end
          else if nToY < m_nCurrY then // 门往左上开
          begin
            IsFound := False;
            // 45度往右上尝试
            for I := 1 to 3 do
            begin
              if Points[6 + I, 6 - I] then
              begin
                IsFound := True;
                for J := 1 to 3 do
                begin
                  if not Points[6 + I - J, 6 - I - J] then
                  begin
                    IsFound := False;
                    Break;
                  end;
                end;

                if IsFound then
                  Break;
              end;
            end;

            if IsFound then
            begin
              if I = 1 then
              begin
                if WalkTo(DR_UPRIGHT, False) then
                begin
                  FIsTempMoveSet := True;
                  FTempMoveDir := DR_UPLEFT;
                  FTempMoveCount := 3;
                  FTempMoveDirTick := MyGetTickCount;

                  Exit;
                end;
              end
              else if I >= 2 then
              begin
                if RunTo(DR_UPRIGHT, False) then
                begin
                  FIsTempMoveSet := True;
                  FTempMoveDir := DR_UPLEFT;
                  FTempMoveCount := 3;
                  FTempMoveDirTick := MyGetTickCount;

                  Exit;
                end;
              end;
            end
            else
            begin
              IsFound := False;
              // 45度往左下尝试
              for I := 1 to 3 do
              begin
                if Points[6 - I, 6 + I] then
                begin
                  IsFound := True;
                  for J := 1 to 3 do
                  begin
                    if not Points[6 - I - J, 6 + I - J] then
                    begin
                      IsFound := False;
                      Break;
                    end;
                  end;

                  if IsFound then
                    Break;
                end;
              end;

              if IsFound then
              begin
                if I = 1 then
                begin
                  if WalkTo(DR_DOWNLEFT, False) then
                  begin
                    FIsTempMoveSet := True;
                    FTempMoveDir := DR_UPLEFT;
                    FTempMoveCount := 3;
                    FTempMoveDirTick := MyGetTickCount;

                    Exit;
                  end;
                end
                else if I >= 2 then
                begin
                  if RunTo(DR_DOWNLEFT, False) then
                  begin
                    FIsTempMoveSet := True;
                    FTempMoveDir := DR_UPLEFT;
                    FTempMoveCount := 3;
                    FTempMoveDirTick := MyGetTickCount;

                    Exit;
                  end;
                end;
              end;
            end;
          end;
        end;
      end;
    end
    else
    begin
      if (MyGetTickCount - FTempMoveDirTick >= 5000) then
        FIsTempMoveSet := False;

      if (FIsTempMoveSet) and (FTempMoveCount > 0) then
      begin
        if MyGetTickCount - m_HeroMoveTick >= nRunTime then
        begin
          if FTempMoveCount > 2 then
          begin
            RunTo(FTempMoveDir, False);
            FTempMoveCount := FTempMoveCount - 2;
          end
          else
          begin
            if WalkTo(FTempMoveDir, False) then
              FTempMoveCount := FTempMoveCount - 1
            else
              WalkTo(FTempMoveDir0, False);
          end;

          if FTempMoveCount = 0 then
          begin
            FIsStopTempMove := True;
            FIsTempMoveSet := False;
          end;
        end;
        Exit;
      end;
    end;

    if GotoNearRuntoXY(m_nTargetX, m_nTargetY, nTargetX, nTargetY) then
    begin
      boMove := True;

      nDir1 := GetNextDirection(m_nCurrX, m_nCurrY, nTargetX, nTargetY);
      if ((m_nTargetX <> nTargetX) or (m_nTargetY <> nTargetY)) //
        and GotoNearRuntoXY(nTargetX, nTargetY, m_nTargetX, m_nTargetY, nTempX, nTempY) then
      begin
        nDir2 := GetNextDirection(nTargetX, nTargetY, nTempX, nTempY);

        // 当前步骤和下一步的方向相反，就是来回搞chongchong 2017-10-26
        if ((m_nTargetX <> nTargetX) or (m_nTargetY <> nTargetY)) and (GetDifferenceDirection(nDir1) = nDir2) then
          boMove := False;
      end;

      if not boMove then
      begin
        WalkTo(Random(8), False);
        m_dwMoveTimeTick := MyGetTickCount;
        Exit;
      end;

      if ((abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1)) then
      begin
        RunTo(nDir1, False);
      end
      else
      begin
        if not WalkTo(nDir1, False) then
        begin
          if not WalkTo((nDir1 - 1 + 8) mod 8, False) then
          begin
            if not WalkTo((nDir1 + 1) mod 8, False) then
            begin
              WalkTo(Random(8), False);
            end;
          end;
        end;
      end;
    end
    else
      WalkTo(Random(8), False);
  end;
end;

function THeroObject.HeroGotoTargetXY(IsFollowMaster: Boolean): Boolean;
var
  nTempTime, nWalkTime: Integer;
begin
  Result := False;
  if (m_nTargetX = -1) or (m_nTargetY = -1) then
    Exit;

  if not m_boCanWalk then
    Exit;

  nTempTime := GetMoveTime;

  if False { IsFollowMaster } then
  begin
    case m_btJob of
      0:
        begin
          if nTempTime < g_Config.dwWalkIntervalTime then
            nWalkTime := nTempTime
          else
            nWalkTime := g_Config.dwWalkIntervalTime;
        end;
      1:
        begin
          if nTempTime < g_Config.dwWalkIntervalTime then
            nWalkTime := nTempTime
          else
            nWalkTime := g_Config.dwWalkIntervalTime;
        end;
      2:
        begin
          if nTempTime < g_Config.dwWalkIntervalTime then
            nWalkTime := nTempTime
          else
            nWalkTime := g_Config.dwWalkIntervalTime;
        end;
    else
      nWalkTime := 500;
    end;
  end
  else
    nWalkTime := nTempTime;

  if ((MyGetTickCount - m_dwMoveTimeTick) > nWalkTime) then
  begin
    GotoTargetXY;
    m_dwMoveTimeTick := MyGetTickCount;
    Result := True;
  end;
end;

function THeroObject.HeroRuntoTargetXY(IsFollowMaster: Boolean): Boolean;
var
  nTempTime, nWalkTime: Integer;
begin
  Result := False;

  if (m_nTargetX = -1) or (m_nTargetY = -1) then
    Exit;

  if not(m_boCanRun) then
    Exit;

  if m_boDuanJin or m_boCobwebWindingStatus then
  begin
    if not m_boCanWalk then
      Exit;

    HeroGotoTargetXY;
    Exit;
  end;

  nTempTime := GetMoveTime;

  if False { IsFollowMaster } then
  begin
    case m_btJob of
      0:
        begin
          if nTempTime < g_Config.dwRunIntervalTime then
            nWalkTime := nTempTime
          else
            nWalkTime := g_Config.dwRunIntervalTime;
        end;
      1:
        begin
          if nTempTime < g_Config.dwRunIntervalTime then
            nWalkTime := nTempTime
          else
            nWalkTime := g_Config.dwRunIntervalTime;
        end;
      2:
        begin
          if nTempTime < g_Config.dwRunIntervalTime then
            nWalkTime := nTempTime
          else
            nWalkTime := g_Config.dwRunIntervalTime;
        end;
    else
      nWalkTime := 500;
    end;
  end
  else
    nWalkTime := nTempTime;

  if ((MyGetTickCount - m_dwMoveTimeTick) >= nWalkTime) then
  begin
    RunToTargetXY;
    m_dwMoveTimeTick := MyGetTickCount;
    Result := True;
  end;
end;

procedure THeroObject.HeroAutoMove();
var
  nWalkTime: Integer;
begin
  nWalkTime := GetMoveTime;
  if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
  begin
    if m_nTargetX <> -1 then
    begin
      if (abs(m_nCurrX - m_nTargetX) > 1) or (abs(m_nCurrY - m_nTargetY) > 1) then
        HeroRuntoTargetXY
      else
        HeroGotoTargetXY;
    end;
  end;
end;

function THeroObject.HeroAvoidTarget(Target: TBaseObject): Boolean; // 英雄躲避攻击目标
var
  nWalkTime, nAttackTime: Integer;
  nDir1, nIndex: Integer;
  nTargetX, nTargetY: Integer;
  btArrDirs: array [0 .. 6] of Byte;
begin
  Result := False;
  // 这里是用的躲避间隔，非移动间隔 chongchong 2018-05-29
  case m_btJob of
    0:
      nWalkTime := g_Config.dwHeroWarrorWalkTime;
    1:
      nWalkTime := g_Config.dwHeroAvoidTime;
    2:
      nWalkTime := g_Config.dwHeroAvoidTime;
  else
    nWalkTime := 500;
  end;

  // 道法攻击速度设置很快时，躲避几率变得很低chongchong 2017-12-16
  nAttackTime := GetAttackIntervalTime(False);
  if (nAttackTime < nWalkTime) //
    and (MyGetTickCount - m_dwLastAttackTick >= nAttackTime) //
    and (MyGetTickCount - m_dwLastAttackTick <= nWalkTime) then
    Result := True;

  if MyGetTickCount - m_dwMoveTimeTick > nWalkTime then
  begin
    nDir1 := GetNextDirection(Target.m_nCurrX, Target.m_nCurrY, m_nCurrX, m_nCurrY);
    if (nDir1 in [0, 2, 4, 6]) then
    begin
      btArrDirs[0] := (nDir1 + 1) mod 8;
      btArrDirs[1] := (nDir1 + 2) mod 8;
      btArrDirs[2] := (nDir1 + 6) mod 8;
      btArrDirs[3] := (nDir1 + 0) mod 8;
      btArrDirs[4] := (nDir1 + 7) mod 8;
      btArrDirs[5] := (nDir1 + 3) mod 8;
      btArrDirs[6] := (nDir1 + 5) mod 8;

      for nIndex := Low(btArrDirs) to High(btArrDirs) do
      begin
        nTargetX := m_nCurrX;
        nTargetY := m_nCurrY;
        if GetRuntoXY(btArrDirs[nIndex], nTargetX, nTargetY) then
        begin
          SetTargetXY(nTargetX, nTargetY);
          if (abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1) then
          begin
            if RunTo(btArrDirs[nIndex], False) then
            begin
              Result := True;
              m_dwMoveTimeTick := MyGetTickCount;

              m_boAvoidTargetFirst := True;
              m_btAvoidTargetDir := btArrDirs[nIndex];
            end;
          end
          else if WalkTo(btArrDirs[nIndex], False) then
          begin
            Result := True;
            m_dwMoveTimeTick := MyGetTickCount;

            m_boAvoidTargetFirst := True;
            m_btAvoidTargetDir := btArrDirs[nIndex];
          end;
          Break;
        end;
      end;
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

      for nIndex := Low(btArrDirs) to High(btArrDirs) do
      begin
        nTargetX := m_nCurrX;
        nTargetY := m_nCurrY;

        if GetRuntoXY(btArrDirs[nIndex], nTargetX, nTargetY) then
        begin
          SetTargetXY(nTargetX, nTargetY);
          if (abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1) then
          begin
            if RunTo(btArrDirs[nIndex], False) then
            begin
              Result := True;
              m_dwMoveTimeTick := MyGetTickCount;

              m_boAvoidTargetFirst := True;
              m_btAvoidTargetDir := btArrDirs[nIndex];
            end;
          end
          else if WalkTo(btArrDirs[nIndex], False) then
          begin
            Result := True;
            m_dwMoveTimeTick := MyGetTickCount;

            m_boAvoidTargetFirst := True;
            m_btAvoidTargetDir := btArrDirs[nIndex];
          end;
          Break;
        end;
      end;
    end;
  end;
end;

function THeroObject.HeroAvoidTargetNext: Boolean; // 英雄躲避攻击目标
var
  nWalkTime: Integer;
  nDir1: Integer;
  nTargetX, nTargetY: Integer;
begin
  Result := False;

  case m_btJob of
    0:
      nWalkTime := g_Config.dwHeroWarrorWalkTime;
    1:
      nWalkTime := g_Config.dwHeroAvoidTime; // g_Config.dwHeroWizardWalkTime;
    2:
      nWalkTime := g_Config.dwHeroAvoidTime; // g_Config.dwHeroTaoistWalkTime;
  else
    nWalkTime := 500;
  end;

  {
    // 道法攻击速度设置很快时，躲避几率变得很低chongchong 2017-12-16
    nAttackTime := GetAttackIntervalTime(False);
    if nAttackTime < nWalkTime then
    begin
    if (MyGetTickCount - m_dwLastAttackTick >= nAttackTime) and
    (MyGetTickCount - m_dwLastAttackTick <= nWalkTime) then
    begin
    Result := True;
    end;
    end;
  }

  if (MyGetTickCount - m_dwMoveTimeTick) > nWalkTime then
  begin
    nTargetX := m_nCurrX;
    nTargetY := m_nCurrY;

    nDir1 := (m_btAvoidTargetDir + 1) mod 8;
    if GetRuntoXY(nDir1, nTargetX, nTargetY) then
    begin
      SetTargetXY(nTargetX, nTargetY);
      if (abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1) then
      begin
        if RunTo(nDir1, False) then
        begin
          Result := True;
          m_dwMoveTimeTick := MyGetTickCount;

          m_boAvoidTargetFirst := False;
        end;
      end
      else
      begin
        if WalkTo(nDir1, False) then
        begin
          Result := True;
          m_dwMoveTimeTick := MyGetTickCount;

          m_boAvoidTargetFirst := False;
        end;
      end;
    end;
  end;
end;

function THeroObject.HeroThink(): Boolean;
var
  nOldX, nOldY: Integer;
  UserMagic: pTUserMagic;
  boDupMode: Boolean;
  nHeroDir: Integer;
  nCount: Integer;

  function CheckHumanLock(BaseObject: TBaseObject): Boolean;
  begin
    Result := False;
    if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    begin
      if BaseObject.InSafeZone then
        Exit;

      // 对方锁定了自己或主人
      if (BaseObject.m_TargetCret <> nil) and ((BaseObject.m_TargetCret = Self) or (BaseObject.m_TargetCret.Master = m_Master))
      then
        Result := True;

      // 别人攻击我或主人
      if (BaseObject.m_LastHiter = Self) or ((m_Master <> nil) and (m_Master.m_LastHiter = BaseObject)) then
        Result := True;
    end;
  end;

begin
  Result := False;
  try
    if (m_TargetCret <> nil) and (((m_TargetCret.m_boAdminMode or m_TargetCret.m_boTempAdminMode) and (not m_boTarget)) or
      m_TargetCret.m_boStoneMode) then
    begin
      DelTargetCreat;
    end;

    boDupMode := False;
    if (MyGetTickCount - m_dwThinkTick) > 500 then
    begin
      m_dwThinkTick := MyGetTickCount();

      if ((m_Master = nil) or (not InSafeZone) or ((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))) then
      // 防止安全区宝宝被挤出安全区
      begin
        if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then
          boDupMode := True;
      end
      // 不让英雄和NPC叠到一起 2020-09-07 00:01:58
      else if (m_Master <> nil) and (m_PEnvir.GetXYNpcObjCount(m_nCurrX, m_nCurrY) >= 1) then
      begin
        boDupMode := True;
      end;
    end;

    if boDupMode then
    begin
      nOldX := m_nCurrX;
      nOldY := m_nCurrY;
      WalkTo(Random(8), False);
      if (nOldX <> m_nCurrX) or (nOldY <> m_nCurrY) then
      begin
        Result := True;
      end;
    end
    else if (not m_boProtectStatus) and (m_btAttackMode <> 2) and (m_TargetCret = nil) and (m_Master <> nil) and
      (abs(m_nCurrX - m_Master.m_nCurrX) <= 3) and (abs(m_nCurrY - m_Master.m_nCurrY) <= 3) and
      (GetNextDirection(m_Master.m_nCurrX, m_Master.m_nCurrY, m_nCurrX, m_nCurrY) = m_Master.m_btDirection) then
    begin
      nHeroDir := GetNextDirection(m_nCurrX, m_nCurrY, m_Master.m_nCurrX, m_Master.m_nCurrY);

      if (abs(m_nCurrX - m_Master.m_nCurrX) = 3) or (abs(m_nCurrY - m_Master.m_nCurrY) = 3) then
      begin
        if RunTo(nHeroDir, False) then
        begin
          Exit;
        end;
      end
      else if (abs(m_nCurrX - m_Master.m_nCurrX) = 2) or (abs(m_nCurrY - m_Master.m_nCurrY) = 2) then
      begin
        if WalkTo(nHeroDir, False) then
        begin
          Exit;
        end;
      end;

      nHeroDir := (nHeroDir + 1) mod 8;
      if not WalkTo(nHeroDir, False) then
      begin
        nCount := 0;
        while True do
        begin
          nHeroDir := Random(8);
          if nHeroDir <> GetDifferenceDirection(m_Master.m_btDirection) then
          begin
            WalkTo(nHeroDir, False);
            Break;
          end;

          Inc(nCount);
          if nCount >= 10 then
            Break;
        end;
      end;
    end;

    // 英雄一直攻击，不要打30秒停手 （不注释为：打了英雄停手，英雄打30秒也停手） chongchong 2017-12-11
    {
      if (MyGetTickCount - m_dwThinkTick2) > 3000 then
      begin
      m_dwThinkTick2 := MyGetTickCount();
      if (m_TargetCret <> nil) and (not m_boTarget) and ((not IsProperTarget(m_TargetCret)) or CheckHumanLock(m_TargetCret)) then
      begin
      DelTargetCreat;
      end;
      end;
    }
    // 英雄忠诚度达到指定值后并且相关技能(灵魂火符，烈火剑法，灭天火)满3级，自动切换到四级状态 【2013-08-15】
    if (m_rLoyalPoint >= g_Config.dwHeroGotoLV4 / 100) then
    begin
      case m_btJob of
        0:
          begin
            UserMagic := FindMagic(SKILL_FIRESWORD);
            if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
            begin
              UserMagic.btLevel := 4; // 升级为4级技能  220
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级烈火剑法!', c_Green, t_Hint);
            end;

            {
              UserMagic := FindMagic(SKILL_87);
              if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
              begin
              UserMagic.btLevel := 4;                                                               //升级为4级技能
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级武力盾! ', c_Green, t_Hint);
              end;

              UserMagic := FindMagic(SKILL_88);
              if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
              begin
              UserMagic.btLevel := 4;                                                               //升级为4级技能
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级新武士盾!', c_Green, t_Hint);
              end;
            }
          end;
        1:
          begin
            UserMagic := FindMagic(SKILL_45);
            if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
            begin
              UserMagic.btLevel := 4; // 升级为4级技能
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级灭天火!', c_Green, t_Hint);
            end;

            {
              UserMagic := FindMagic(SKILL_SHIELD);
              if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
              begin
              UserMagic.btLevel := 4;                                                               //升级为4级技能
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级魔法盾!', c_Green, t_Hint);
              end;
            }
          end;
        2:
          begin
            UserMagic := FindMagic(SKILL_FIRECHARM);
            if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
            begin
              UserMagic.btLevel := 4; // 升级为4级技能
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级灵魂火符!', c_Green, t_Hint);
            end;

            {
              UserMagic := FindMagic(SKILL_73);
              if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
              begin
              UserMagic.btLevel := 4;
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级道力盾!', c_Green, t_Hint);
              end;

              UserMagic := FindMagic(SKILL_89);
              if (UserMagic <> nil) and (UserMagic.btLevel = 3) then
              begin
              UserMagic.btLevel := 4;
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于你们亲密的关系,您的英雄已经领悟了4级新道士盾!', c_Green, t_Hint);
              end;
            }
          end;
      end;
    end
    else if g_Config.dwHeroGotoLV4 < 20000 then // 忠诚度低于触发值时,4级降为3级 20080609
    begin
      case m_btJob of
        0:
          begin
            UserMagic := FindMagic(SKILL_FIRESWORD);
            if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
            begin
              UserMagic.btLevel := 3; // 4级降为3级
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级烈火剑法已恢复到3级!', c_Green, t_Hint);
            end;

            {
              UserMagic := FindMagic(SKILL_87);
              if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
              begin
              UserMagic.btLevel := 3;
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级武力盾已恢复到3级!', c_Green, t_Hint);
              end;

              UserMagic := FindMagic(SKILL_88);
              if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
              begin
              UserMagic.btLevel := 3;
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级新武士盾已恢复到3级!', c_Green, t_Hint);
              end;
            }
          end;
        1:
          begin
            UserMagic := FindMagic(SKILL_45);
            if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
            begin
              UserMagic.btLevel := 3; // 4级降为3级
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级灭天火已恢复到3级!', c_Green, t_Hint);
            end;

            {
              UserMagic := FindMagic(SKILL_SHIELD);
              if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
              begin
              UserMagic.btLevel := 3;                                                               //升级为4级技能
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级魔法盾已恢复到3级!', c_Green, t_Hint);
              end;
            }
          end;
        2:
          begin
            UserMagic := FindMagic(SKILL_FIRECHARM);
            if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
            begin
              UserMagic.btLevel := 3; // 4级降为3级
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级灵魂火符已恢复到3级!', c_Green, t_Hint);
            end;

            {
              UserMagic := FindMagic(SKILL_73);
              if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
              begin
              UserMagic.btLevel := 3;
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级道力盾已恢复到3级!', c_Green, t_Hint);
              end;

              UserMagic := FindMagic(SKILL_89);
              if (UserMagic <> nil) and (UserMagic.btLevel = 4) then
              begin
              UserMagic.btLevel := 3;
              SendMsg(m_Master, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
              SysMsg('由于英雄的忠诚度不够,英雄的4级新道士盾已恢复到3级!', c_Green, t_Hint);
              end;
            }
          end;
      end;
    end;
  except
    MainOutMessage('[Exception] TPlayObject:HeroThink');
  end;
end;

function THeroObject.OpenSuperShiled: Boolean;
begin
  Result := False;
  if AllowUseMagic(SKILL_75) and ((MyGetTickCount - m_dwLastSuperShiledTimeTick) >= GetMagicCD(SKILL_75)) then
  begin
    m_boSuperShiled := True; // 护体神盾
    m_dwSuperShiledValidTimeTick := MyGetTickCount(); // 护体神盾有效时间
    m_dwLastSuperShiledTimeTick := MyGetTickCount(); // 护体神盾使用间隔
    if g_Config.boShowSuperShiledEffect or g_Config.boShowSuperShiledSound then
    begin
      SendRefMsg(RM_SENDSUPERSHILEDEFFECT, m_btDirection, Integer(g_Config.boShowSuperShiledEffect),
        Integer(g_Config.boShowSuperShiledSound), 0, '');
    end;
    Result := True;
  end;
end;

function THeroObject.HeroBasicAttackTarget: Boolean;
var
  btDir: Byte;
begin
  Result := False;

  // 2019-10-21 12:37:45
  if (not m_boCanHit) then
    Exit;

  try
    if m_TargetCret <> nil then
    begin
      if GetAttackDir(m_TargetCret, btDir) then
      begin
        if (MyGetTickCount - m_dwLastAttackTick) > GetAttackIntervalTime(True) then
        begin
          m_dwLastAttackTick := MyGetTickCount;
          AttackDir(nil, 0, btDir);
        end;
        Result := True;
      end;
    end;
  except
    MainOutMessage('[Exception] TPlayObject:HeroBasicAttackTarget');
  end;
end;

function THeroObject.HeroWarrAttackTarget: Boolean; // 战士
var
  btDir: Byte;
  btAttack: Word;
  UserMagic: pTUserMagic;
  nSpellPoint: Integer;
  TempX, TempY, NewX, NewY: Integer;
  nDistance, OffsetX, OffsetY: Integer;
  IsCanAttack, IsChasingTarget: Boolean;
  AttackDis: Integer;
  UseSkillErgum: Boolean;
  CanSwordWideAttack, CanSwordCrsAttack, CanSkill42Attack: Boolean;
  CustomUserMagic: pTUserMagic;
  CustomMagicConfig: TCustomMagicConfig;
  I: Integer;
  boFindMagic: Boolean;
  HeroMagic: PHeroMagic;
  boConditionOK: Boolean;
  IsFuckTarget: Boolean;
  nSkill66Range: Integer;

  function GetMagicSpell(UserMagic: pTUserMagic): Integer;
  begin
    Result := Round(UserMagic.MagicInfo.wSpell / (UserMagic.MagicInfo.btTrainLv + 1) * (UserMagic.btLevel + 1));
  end;

  function TargetInSwordWideAttackRange(nDir: Integer): Boolean; // 半月的范围 有2个怪物可以攻击
  var
    nX, nY, nC, n10, nMonCount: Integer;
    BaseObject: TBaseObject;
  const
    WideAttack: array [0 .. 2] of Integer = (7, 1, 2);
  begin
    Result := False;
    nMonCount := 0;

    // BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, BaseObject.m_TargetCret.m_nCurrX, BaseObject.m_TargetCret.m_nCurrY, g_Config.nSkill64PowerRange {攻击范围}, TargetObjects);

    nC := 0;
    while (True) do
    begin
      n10 := (nDir + WideAttack[nC]) mod 8;
      m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, n10, 1, nX, nY);
      BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (BaseObject <> m_TargetCret) and
        IsProperTarget(BaseObject) then
      begin
        Inc(nMonCount);
        if nMonCount >= 1 then // 除了正目标外，还有一个可以攻击的对象
        begin
          Result := True;
          Break;
        end;
      end;

      Inc(nC);
      if nC >= 3 then
        Break;
    end;
  end;

  function TargetInSwordCrsAttackRange(nDir: Integer): Boolean; // 双龙斩 有2个怪物可以攻击
  const
    CrsAttack: array [0 .. 6] of Byte = (7, 1, 2, 3, 4, 5, 6);
  var
    nX, nY, n10, nC, nMonCount: Integer;
    BaseObject: TBaseObject;
  begin
    Result := False;
    nMonCount := 0;

    nC := 0;
    while (True) do
    begin
      n10 := (nDir + CrsAttack[nC]) mod 8;
      m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, n10, 1, nX, nY);
      BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (BaseObject <> m_TargetCret) and
        IsProperTarget(BaseObject) then
      begin
        Inc(nMonCount);
        if nMonCount >= 1 then // 除了正目标外，还有一个可以攻击的对象
        begin
          Result := True;
          Break;
        end;
      end;
      Inc(nC);
      if nC >= 7 then
        Break;
    end;
  end;

  function TargetInSkill42AttackRange(nDir: Integer): Boolean; // 检测是否在龙影剑法攻击范围
  var
    nX, nY, I, II, nDirTemp, nMonCount: Integer;
    BaseObject: TBaseObject;
  begin
    Result := False;

    nMonCount := 0;
    for I := -1 to 1 do
    begin
      nX := m_nCurrX;
      nY := m_nCurrY;
      nDirTemp := nDir + I;

      if nDirTemp > DR_UPLEFT then
        nDirTemp := DR_UP;
      if nDirTemp < DR_UP then
        nDirTemp := DR_UPLEFT;

      for II := 0 to 2 do
      begin
        m_PEnvir.GetNextPosition(nX, nY, nDirTemp, 1, nX, nY);
        BaseObject := m_PEnvir.GetMovingObject(nX, nY, True);

        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (BaseObject <> m_TargetCret) and
          IsProperTarget(BaseObject) then
        begin
          Inc(nMonCount);
          if nMonCount >= 1 then // 除了正目标外，还有一个可以攻击的对象
          begin
            Result := True;
            Break;
          end;
        end;
      end;
    end;
  end;

begin
  Result := False;
  UserMagic := nil;
  if m_TargetCret = nil then
    Exit;

  {
    Result := OpenSuperShiled;
    if Result then Exit;
  }

  if tick_diff(m_dwMotaeboTick, MyGetTickCount) <= 200 then
  begin
    Exit;
  end;

  if (not m_boCanHit) and (not m_boCanSpell) then
  begin
    Exit;
  end;

  if (MyGetTickCount - m_dwLastAttackTick < GetAttackIntervalTime(True)) then
  begin
    Result := True;

    UseSkillErgum := False;
    if (not m_boFireHitSkill) and AllowUseMagic(SKILL_ERGUM) and (g_Config.dwHeroWarriorDefaultSkill = SKILL_ERGUM) then
      AttackDis := 2
    else if (not m_boFireHitSkill) and AllowUseMagic(SKILL_ERGUM) and (Random(g_Config.dwHeroWarrAttacSkillErgumRate) = 0) then
    begin
      UseSkillErgum := True;
      AttackDis := 2;
    end
    else
    begin
      AttackDis := 1;
    end;

    // 英雄保持与目标的攻击距离，围绕目标攻击
    if (g_Config.dwHeroWarrAttackMoveRate < 500) and (Random(g_Config.dwHeroWarrAttackMoveRate) = 0) and
      ((MyGetTickCount - m_dwMoveTimeTick) > GetMoveTime) and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= AttackDis) and
      (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= AttackDis) and
      (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove)) and (not FRandomWalk) then
    begin
      btDir := GetNextDirection(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_nCurrX, m_nCurrY);

      if UseSkillErgum and (Random(3) = 0) then
        nDistance := 2
      else
        nDistance := 1;

      if Random(2) = 0 then
        btDir := (btDir + 1) mod 8
      else
        btDir := (btDir - 1) mod 8;

      m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btDir, nDistance, NewX, NewY);

      if ((NewX <> m_nCurrX) or (NewY <> m_nCurrY)) and m_PEnvir.CanWalk(NewX, NewY, False) then
      begin
        SetTargetXY(NewX, NewY);
        HeroGotoTargetXY;

        FRandomWalk := True;
        Exit;
      end;
    end;
  end;

  // 修正战士跑步前去攻击时，前三刀过快 chongchong 2015-08-04
  // IsCanAttack := (MyGetTickCount - m_dwMoveTimeTick >= GetAttackIntervalTime);

  if (MyGetTickCount - m_TargetCret.m_dwStationTick <= 600) and (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])
  then
  begin
    // 追目标时，可以攻击，不然出刀慢
    IsCanAttack := True;
    IsChasingTarget := True;
  end
  else
  begin
    IsChasingTarget := False;

    // if MyGetTickCount - m_TargetCret.m_dwStationTick <= 800 then
    // begin
    // m_dwLastAttackTick := m_dwLastAttackTick + 100;
    // end;
    // 不追目标，出刀在移动后一段时间，不然跑步前去攻击时，前三刀过快
    // IsCanAttack := MyGetTickCount - m_dwStationTick >= 800;
    // 看看上次移动到这次的时间够不够客户端播放移动帧的时间，不够则补之 chongchong 2018-07-17 01:43:50

    if m_HeroMoveTick < m_dwLastAttackTick then
    begin
      IsCanAttack := MyGetTickCount - m_dwMoveTimeTick >= GetAttackIntervalTime(True) * 2 + GetMoveTime + 180 - m_MoveToAttackTick
        - m_AttackToMoveTick;
    end
    else
      IsCanAttack := True;

    {
      if MyGetTickCount - m_HeroMoveTick >= GetMoveTime then
      begin
      IsCanAttack := (MyGetTickCount - m_dwMoveTimeTick >= GetAttackIntervalTime(True));
      end
      else
      begin
      m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
      IsCanAttack := (MyGetTickCount - m_dwMoveTimeTick >= GetAttackIntervalTime(True) + GetMoveTime + 200 - (MyGetTickCount - m_HeroMoveTick));
      end;
    }
  end;

  try
    OffsetX := abs(m_TargetCret.m_nCurrY - m_nCurrY);
    OffsetY := abs(m_TargetCret.m_nCurrX - m_nCurrX);

    nSkill66Range := 2;
    if g_Config.boHeroSkill66HighAttackNoUseRate and (m_Abil.Level >= m_TargetCret.m_Abil.Level) then
    begin
      nSkill66Range := 4;
    end;

    { SKILL_208 旋风斩 }
    if AllowUseMagic(SKILL_208) and (not m_boSWordHitSkill) and
      (MyGetTickCount - m_SkillUseTick[SKILL_208] >= GetMagicCD(SKILL_208)) and
      ((MyGetTickCount - m_dwLastAttackTick) >= GetAttackIntervalTime(True)) and IsCanAttack and
      ((OffsetX <= g_Config.nSkill208Rage) and (OffsetY <= g_Config.nSkill208Rage)) and
      AllowHeroMagicRate(mtWarrAttack, SKILL_208) and (not m_boImprison) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_208);
      if UserMagic <> nil then
      begin
        nSpellPoint := GetSpellPoint(UserMagic);
        if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
        begin
          DamageSpell(nSpellPoint);
          HealthSpellChanged();
        end;
        DoSpell(UserMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nil);
      end;
    end
    { 204 十步一杀 }
    else if AllowUseMagic(SKILL_204) and (not m_boSWordHitSkill) and
      (MyGetTickCount - m_SkillUseTick[SKILL_204] >= GetMagicCD(SKILL_204)) and
      ((MyGetTickCount - m_dwLastAttackTick) >= GetAttackIntervalTime(True)) and IsCanAttack and
      ((OffsetX <= 6) and (OffsetY <= 6)) and
      ((OffsetX > 4) or (OffsetY > 4) or (MyGetTickCount - m_SkillUseTick[SKILL_204] >= Max(GetMagicCD(SKILL_204), 15000))) and
    // 修复英雄远距离攻击与对象不在一条线上(打偏了) chongchong 2013-12-18
    // ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) and
      AllowHeroMagicRate(mtWarrAttack, SKILL_204) and (not m_boImprison) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_204);
      if UserMagic <> nil then
      begin
        nSpellPoint := GetSpellPoint(UserMagic);
        if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
        begin
          DamageSpell(nSpellPoint);
          HealthSpellChanged();
        end;
        DoSpell(UserMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nil);
      end;
    end
    { 56 逐日剑法 }
    else if AllowUseMagic(SKILL_56) and (not m_boSWordHitSkill) and
      (MyGetTickCount - m_SkillUseTick[SKILL_56] >= GetMagicCD(SKILL_56)) and
      ((MyGetTickCount - m_dwLastAttackTick) >= GetAttackIntervalTime(True)) and IsCanAttack and
      ((OffsetX <= 4) and (OffsetY <= 4)) and // 修复英雄远距离攻击与对象不在一条线上(打偏了) chongchong 2013-12-18
      ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) and AllowHeroMagicRate(mtWarrAttack, SKILL_56) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_56);
      if UserMagic <> nil then
      begin
        nSpellPoint := GetSpellPoint(UserMagic);
        if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
        begin
          DamageSpell(nSpellPoint);
          HealthSpellChanged();
        end;
      end;

      // MainOutMessage('~~~~HeroAttack: ' + IntToStr(MyGetTickCount - m_dwLastAttackTick));

      m_SkillUseTick[SKILL_56] := MyGetTickCount();
      m_boSWordHitSkill := True;
      btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      btAttack := 15;
      AttackDir(nil, btAttack, btDir);
      m_dwLastAttackTick := MyGetTickCount();
      m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;

      Result := True;
      FRandomWalk := False;
      FLastAttack := True;
      Exit;
    end
    { 66 开天斩4格攻击范围 }
    else if AllowUseMagic(SKILL_66) and (not m_bo66Skill) and (MyGetTickCount - m_SkillUseTick[SKILL_66] > GetMagicCD(SKILL_66)
      { g_Config.nHeroSkill66CD * 1000 } ) and ((MyGetTickCount - m_dwLastAttackTick) >= GetAttackIntervalTime(True)) and
      IsCanAttack and ((OffsetX <= nSkill66Range) and (OffsetY <= nSkill66Range)) and
    // 修复英雄远距离攻击与对象不在一条线上(打偏了) chongchong 2013-12-18
      ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) and AllowHeroMagicRate(mtWarrAttack, SKILL_66) then
    begin
      m_SkillUseTick[SKILL_66] := MyGetTickCount();
      m_bo66Skill := True;

      // 英雄开天斩重击几率 2019-07-08 16:22:18

      if not g_Config.boHeroSkill66HighAttackNoUseRate then
      begin
        m_bo66SkillCls := (Random(g_Config.nHeroSkill66HighAttackRate) = 0);
      end
      else
      begin
        m_bo66SkillCls := (m_Abil.Level >= m_TargetCret.m_Abil.Level)
      end;

      if m_bo66SkillCls then
      begin
        btAttack := 11;
      end
      else
      begin
        btAttack := 19;
      end;

      UserMagic := GetMagicInfoEx(SKILL_66);
      if UserMagic <> nil then
      begin
        nSpellPoint := GetSpellPoint(UserMagic);
        if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
        begin
          DamageSpell(nSpellPoint);
          HealthSpellChanged();
        end;
      end;

      // MainOutMessage('~~~~HeroAttack: ' + IntToStr(MyGetTickCount - m_dwLastAttackTick));
      btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      AttackDir(nil, btAttack, btDir);
      m_dwLastAttackTick := MyGetTickCount;
      m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
      Result := True;
      FRandomWalk := False;
      FLastAttack := True;
      Exit;
    end
    { 113 断空斩4格攻击范围 }
    else if AllowUseMagic(SKILL_113) and (not m_bo113Skill) and
      (MyGetTickCount - m_SkillUseTick[SKILL_113] > GetMagicCD(SKILL_113)
      { g_Config.nHeroSkill113CD * 1000 } ) and ((MyGetTickCount - m_dwLastAttackTick) >= GetAttackIntervalTime(True)) and
      IsCanAttack and ((OffsetX <= 4) and (OffsetY <= 4)) and
    // 修复英雄远距离攻击与对象不在一条线上(打偏了) chongchong 2013-12-18
      ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) and AllowHeroMagicRate(mtWarrAttack, SKILL_113) then
    begin
      m_SkillUseTick[SKILL_113] := MyGetTickCount();
      m_bo113Skill := True;
      btAttack := 20;

      UserMagic := GetMagicInfoEx(SKILL_113);
      if UserMagic <> nil then
      begin
        nSpellPoint := GetSpellPoint(UserMagic);
        if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
        begin
          DamageSpell(nSpellPoint);
          HealthSpellChanged();
        end;
      end;

      // MainOutMessage('~~~~HeroAttack: ' + IntToStr(MyGetTickCount - m_dwLastAttackTick));

      btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      AttackDir(nil, btAttack, btDir);
      m_dwLastAttackTick := MyGetTickCount;
      m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
      Result := True;
      FRandomWalk := False;
      FLastAttack := True;
      Exit;
    end
    { 115 血魄一击4格攻击范围 }
    else if AllowUseMagic(SKILL_115) and (not m_bo115Skill) and
      (MyGetTickCount - m_SkillUseTick[SKILL_115] > GetMagicCD(SKILL_115)
      { g_Config.nHeroSkill115CD * 1000 } ) and ((MyGetTickCount - m_dwLastAttackTick) >= GetAttackIntervalTime(True)) and
      IsCanAttack and ((OffsetX <= 4) and (OffsetY <= 4)) and
    // 修复英雄远距离攻击与对象不在一条线上(打偏了) chongchong 2013-12-18
      ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) and AllowHeroMagicRate(mtWarrAttack, SKILL_115) and Allow115HitSkill
    then
    begin
      btAttack := 21;
      if not g_Config.boSkill115UseNG then
      begin
        UserMagic := GetMagicInfoEx(SKILL_115);
        if UserMagic <> nil then
        begin
          nSpellPoint := GetSpellPoint(UserMagic);
          if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
          begin
            DamageSpell(nSpellPoint);
            HealthSpellChanged();
          end;
        end;
      end;

      btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      AttackDir(nil, btAttack, btDir);
      m_dwLastAttackTick := MyGetTickCount;
      m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
      Result := True;
      FRandomWalk := False;
      FLastAttack := True;
      Exit;
    end;

    // 有他妈闲得蛋疼的人用无限使用的自定义技能2格以上的攻击距离攻击，就一直不近身 2019-09-17 23:23:52
    if (not IsChasingTarget) and ((OffsetX >= 2) or (OffsetY >= 2)) and
      (MyGetTickCount - m_dwLastAttackTick >= GetAttackIntervalTime(True)) and (Random(g_Config.dwHeroWarrNearFireSword) = 0) and
      (not FRandomWalk) and FLastAttack and AllowUseMagic(SKILL_FIRESWORD) and
      (MyGetTickCount - m_SkillUseTick[26] >= GetMagicCD(SKILL_FIRESWORD)) then
    begin
      SetTargetXY(NewX, NewY);
      HeroGotoTargetXY;
      Result := True;
      FLastAttack := False;
      FRandomWalk := False;
      Exit;
    end;

    // 执行战士自定义技能 chongchong 2018-01-06
    for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
    begin
      HeroMagic := g_CustomHeroMagicMgr.Items[I];
      if (HeroMagic.MagicType = mtWarrAttack) and HeroMagic.IsCustomMagic and (HeroMagic.AttackRange > 1) then
      begin
        CustomUserMagic := m_CustomSkill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID];

        if (CustomUserMagic <> nil) and (CustomUserMagic.btKey > 0) and AllowHeroMagicRate(mtWarrAttack, HeroMagic.MagicID) then
        begin
          // 判断执行条件 chongchong 2018-01-06
          if CheckHeroMagicUseCondition(@HeroMagic.Condition, m_TargetCret) then
          begin
            CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
            if (CustomMagicConfig <> nil) and CustomMagicConfig.IsMagicWarr then
            begin
              boConditionOK := False;

              nSpellPoint := GetSpellPoint(CustomUserMagic);

              if CustomMagicConfig.ServerConfig.IsAttackUseNG then
              begin
                if m_boTrainingNG and (m_AbilNG.NH >= nSpellPoint) then
                begin
                  if nSpellPoint > 0 then
                  begin
                    m_AbilNG.NH := Max(0, m_AbilNG.NH - nSpellPoint);
                    RefAbilNH;
                  end;

                  boConditionOK := boConditionOK;
                end
              end
              else
              begin
                if (m_WAbil.MP >= nSpellPoint) then
                begin
                  if (nSpellPoint > 0) then
                  begin
                    DamageSpell(nSpellPoint);
                    HealthSpellChanged();
                  end;

                  boConditionOK := True;
                end;
              end;

              if boConditionOK then
              begin
                if CustomMagicConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch then
                begin
                  AllowCustomSkill(HeroMagic.MagicID, True);
                  if m_boCustomSKill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID] then
                  begin
                    btAttack := CM_CUSTOM_HIT001 + (HeroMagic.MagicID - CUSTOM_MAGIC_START_ID);
                    AttackDir(nil, btAttack, btDir);
                    m_dwLastAttackTick := MyGetTickCount();
                    m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
                    FRandomWalk := False;
                    FLastAttack := True;
                    Result := True;
                    Exit;
                  end;
                end
                else if AllowCustomSkill(HeroMagic.MagicID, True) then
                begin
                  btAttack := CM_CUSTOM_HIT001 + (HeroMagic.MagicID - CUSTOM_MAGIC_START_ID);
                  AttackDir(nil, btAttack, btDir);
                  m_dwLastAttackTick := MyGetTickCount();
                  m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
                  FRandomWalk := False;
                  FLastAttack := True;
                  Result := True;
                  Exit;
                end;
              end;
            end;
          end;
        end;
      end;
    end;

    // 低血逃跑
    if m_WAbil.HP < TPlayObject(m_Master).m_nHeroDodgeHPPercent then
    begin
      if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 3) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 3) then
      begin
        HeroAvoidTarget(m_TargetCret);
        m_boLastAvoidTarget := True;
        FLastAttack := False;
      end;

      Result := False;
      Exit;
    end;

    // 近身攻击
    if GetAttackDir(m_TargetCret, btDir) then
    begin
      FErgumSkillUsed := False;

      Result := True;
      if (MyGetTickCount - m_dwLastAttackTick >= GetAttackIntervalTime(True)) { and IsCanAttack } then
      begin
        if (not m_boFireHitSkill) and AllowUseMagic(SKILL_ERGUM) and (Random(g_Config.dwHeroWarrAttacSkillErgumRate) = 0) and
          (not FRandomWalk) and FLastAttack then
        begin
          GetBackPosition(btDir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, NewX, NewY, 2);
          if ((m_nCurrX <> NewX) or (m_nCurrY <> NewY)) and m_PEnvir.CanWalk(NewX, NewY, False) then
          begin
            SetTargetXY(NewX, NewY);
            HeroGotoTargetXY;
            Result := True;
            FLastAttack := False;
            Exit;
          end;
        end;

        // m_dwLastAttackTick := MyGetTickCount;
        btAttack := 0; // 普通攻击
        boFindMagic := False;

        // AttackSideCount := GetMapBaseObjectCount(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 1);

        CanSwordWideAttack := TargetInSwordWideAttackRange(btDir);
        CanSwordCrsAttack := TargetInSwordCrsAttackRange(btDir);
        CanSkill42Attack := TargetInSkill42AttackRange(btDir);

        { 野蛮冲撞 }
        if g_Config.boHeroCanUseMootebo //
          and (not m_TargetCret.m_boStickMode) //
          and AllowUseMagic(SKILL_MOOTEBO) //
          and AllowHeroMagicRate(mtWarrAttack, SKILL_MOOTEBO) { (Random(4) = 0) }   //
          and (m_Abil.Level > m_TargetCret.m_Abil.Level) //
          and (MyGetTickCount - m_SkillUseTick[SKILL_MOOTEBO] >= 1000 * 7) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_MOOTEBO);
          if (UserMagic <> nil) then
          begin
            m_SkillUseTick[SKILL_MOOTEBO] := MyGetTickCount;
            nSpellPoint := GetSpellPoint(UserMagic);
            if m_WAbil.MP >= nSpellPoint then
            begin
              if nSpellPoint > 0 then
              begin
                DamageSpell(nSpellPoint);
                HealthSpellChanged();
              end;
              m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
              if DoMotaebo(m_btDirection, UserMagic.btLevel) then
              begin
                FLastAttack := True;
                m_dwMotaeboTick := MyGetTickCount;
                m_dwLastAttackTick := MyGetTickCount() + 200;
                m_MoveToAttackTick := 0;

                if UserMagic.btLevel < 3 then
                begin
                  if UserMagic.MagicInfo.TrainLevel[UserMagic.btLevel] < m_Abil.Level then
                  begin
                    TrainSkill(UserMagic, Random(3) + 1);
                    if not CheckMagicLevelup(UserMagic) then
                    begin
                      SendDelayMsg(Self, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                        MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint,
                        IntToStr(Integer(UserMagic.MagicAttr)), 1000);
                    end;
                  end;
                end;

                Exit;
              end;
            end;
          end;
        end;

        if (m_MagicPowerHitSkill <> nil) and AllowUseMagic(m_MagicPowerHitSkill.wMagIdx) then
        begin
          Dec(m_btAttackSkillCount);
          if m_btAttackSkillPointCount = m_btAttackSkillCount then
          begin
            m_boPowerHit := True;
            btAttack := 3; // 攻杀剑术
          end;
          if m_btAttackSkillCount <= 0 then
          begin
            m_btAttackSkillCount := 10 { 原来为7，减少几率 } - m_MagicPowerHitSkill.btLevel;
            m_btAttackSkillPointCount := Random(m_btAttackSkillCount);
          end;
        end;

        // 倚天劈地 chongchong 2013-12-11
        if AllowUseMagic(SKILL_114) //
          and (MyGetTickCount - m_SkillUseTick[SKILL_114] > GetMagicCD(SKILL_114) { g_Config.nHeroSkill114HitWaitTime * 1000 } )
          and ((abs(m_TargetCret.m_nCurrY - m_nCurrY) <= g_Config.nSkill114AttackRange) //
          and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= g_Config.nSkill114AttackRange)) //
          and AllowHeroMagicRate(mtWarrAttack, SKILL_114) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_114);
          btAttack := 255;
          boFindMagic := True;
        end
        else if (btAttack = 0) { 26 烈火剑法 }
          and AllowUseMagic(SKILL_FIRESWORD) //
          and (MyGetTickCount - m_SkillUseTick[26] >= GetMagicCD(SKILL_FIRESWORD)) //
          and AllowHeroMagicRate(mtWarrAttack, SKILL_FIRESWORD) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_FIRESWORD);
          if UserMagic <> nil then
          begin
            nSpellPoint := GetSpellPoint(UserMagic);
            if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
            begin
              DamageSpell(nSpellPoint);
              HealthSpellChanged();
            end;
          end;

          m_SkillUseTick[26] := MyGetTickCount();
          m_boFireHitSkill := True;
          btAttack := 7;
          boFindMagic := True;
        end
        /// /////////////////////////////////////////////////////////////////////
        { 39 彻地钉 }
        else if AllowUseMagic(SKILL_GROUPDEDING) and
          (MyGetTickCount - m_SkillUseTick[SKILL_GROUPDEDING] >= GetMagicCD(SKILL_GROUPDEDING)) and
          AllowHeroMagicRate(mtWarrAttack, SKILL_GROUPDEDING) { (Random(3) = 0) } then
        begin
          // m_SkillUseTick[SKILL_GROUPDEDING] := MyGetTickCount;
          UserMagic := GetMagicInfoEx(SKILL_GROUPDEDING);
          btAttack := 255;
          boFindMagic := True;
        end
        { 41 狮子吼 }
        else if AllowUseMagic(SKILL_41) and (MyGetTickCount - m_SkillUseTick[SKILL_41] >= GetMagicCD(SKILL_41)) and
          AllowHeroMagicRate(mtWarrAttack, SKILL_41) { (Random(10) = 0) } and (m_TargetCret.m_Abil.Level <= m_Abil.Level) and
          (not(m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_41);
          btAttack := 255;
          boFindMagic := True;
        end

        { 75 护体神盾 }
        else if (not m_boSuperShiled) and AllowUseMagic(SKILL_75) and (not g_Config.boHeroAutoSuperShiled) and
          ((MyGetTickCount - m_dwLastSuperShiledTimeTick) >= GetMagicCD(SKILL_75)) and AllowHeroMagicRate(mtWarrAttack, SKILL_75)
        { (Random(3) = 0) } then
        begin
          // m_dwLastSuperShiledTimeTick := MyGetTickCount;
          UserMagic := GetMagicInfoEx(SKILL_75);
          btAttack := 255;
          boFindMagic := True;
        end
        /// /////////////////////////////////////////////////////////////////////
        { 25 半月弯刀 }
        else if AllowUseMagic(SKILL_BANWOL) and (g_Config.dwHeroWarriorDefaultSkill <> SKILL_BANWOL) and CanSwordWideAttack
        { and (Random(3) = 0) } and AllowHeroMagicRate(mtWarrAttack, SKILL_BANWOL) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_BANWOL);
          btAttack := 5;
          boFindMagic := True;
        end
        { 40 双龙斩 }
        else if AllowUseMagic(SKILL_40) and CanSwordCrsAttack and AllowHeroMagicRate(mtWarrAttack, SKILL_40) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_40);
          if UserMagic <> nil then
          begin
            nSpellPoint := GetSpellPoint(UserMagic);
            if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
            begin
              DamageSpell(nSpellPoint);
              HealthSpellChanged();
            end;
          end;

          m_boCrsHitkill := True;
          btAttack := 8;
          boFindMagic := True;
        end
        { 42 龙影剑法 }
        else if AllowUseMagic(SKILL_42) and CanSkill42Attack and
          (MyGetTickCount - m_SkillUseTick[SKILL_42] >= GetMagicCD(SKILL_42)) and AllowHeroMagicRate(mtWarrAttack, SKILL_42) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_42);
          if UserMagic <> nil then
          begin
            nSpellPoint := GetSpellPoint(UserMagic);
            if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
            begin
              DamageSpell(nSpellPoint);
              HealthSpellChanged();
            end;
          end;

          m_bo42Skill := True;
          btAttack := 9;
          boFindMagic := True;
        end
        { 43 雷霆剑法 }
        else if AllowUseMagic(SKILL_43) and (MyGetTickCount - m_SkillUseTick[SKILL_43] > GetMagicCD(SKILL_43)) and
          AllowHeroMagicRate(mtWarrAttack, SKILL_43) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_43);
          if UserMagic <> nil then
          begin
            nSpellPoint := GetSpellPoint(UserMagic);
            if (m_WAbil.MP >= nSpellPoint) and (nSpellPoint > 0) then
            begin
              DamageSpell(nSpellPoint);
              HealthSpellChanged();
            end;
          end;

          m_bo43Skill := True;
          btAttack := 10;
          boFindMagic := True;
          m_SkillUseTick[SKILL_43] := MyGetTickCount;
        end;

        if not boFindMagic then
        begin
          for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
          begin
            HeroMagic := g_CustomHeroMagicMgr.Items[I];
            if (HeroMagic.MagicType = mtWarrAttack) and HeroMagic.IsCustomMagic then
            begin
              CustomUserMagic := m_CustomSkill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID];

              if (CustomUserMagic <> nil) and (CustomUserMagic.btKey > 0) and AllowHeroMagicRate(mtWarrAttack, HeroMagic.MagicID)
              then
              begin
                // 判断执行条件 chongchong 2018-01-06
                if CheckHeroMagicUseCondition(@HeroMagic.Condition, m_TargetCret) then
                begin
                  CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                  if (CustomMagicConfig <> nil) and CustomMagicConfig.IsMagicWarr then
                  begin
                    if CustomMagicConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch then
                    begin
                      AllowCustomSkill(HeroMagic.MagicID, True);
                      if m_boCustomSKill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID] then
                      begin
                        btAttack := CM_CUSTOM_HIT001 + (HeroMagic.MagicID - CUSTOM_MAGIC_START_ID);
                        boFindMagic := True;
                        Break;
                      end;
                    end
                    else if AllowCustomSkill(HeroMagic.MagicID, True) then
                    begin
                      btAttack := CM_CUSTOM_HIT001 + (HeroMagic.MagicID - CUSTOM_MAGIC_START_ID);
                      boFindMagic := True;
                      Break;
                    end;
                  end;
                end;
              end;
            end;
          end;
        end;

        if not boFindMagic then
        begin
          if btAttack <> 3 then // 修复学习默认技能后，攻杀剑术不可用 chongchong 2013-10-26
          begin
            if g_Config.dwHeroWarriorDefaultSkill = SKILL_ERGUM then
            begin
              if AllowUseMagic(SKILL_ERGUM) then
                btAttack := 4; // 刺杀剑术
            end
            else if g_Config.dwHeroWarriorDefaultSkill = SKILL_BANWOL then
            begin
              if AllowUseMagic(SKILL_BANWOL) then
                btAttack := 5; // 半月弯刀
            end
            else
            begin
              if CheckIsCustomMagic(g_Config.dwHeroWarriorDefaultSkill) then
              begin
                AllowCustomSkill(g_Config.dwHeroWarriorDefaultSkill, True);
                if m_boCustomSKill[g_Config.dwHeroWarriorDefaultSkill - CUSTOM_MAGIC_START_ID] then
                begin
                  btAttack := CM_CUSTOM_HIT001 + (g_Config.dwHeroWarriorDefaultSkill - CUSTOM_MAGIC_START_ID);
                end;
              end;
            end;
          end;
        end;

        if IsCanAttack then
        begin
          if btAttack = 255 then
          begin
            if UserMagic.wMagIdx = SKILL_75 then
            begin
              if OpenSuperShiled then
                m_dwLastAttackTick := MyGetTickCount();
            end
            else if DoSpell(UserMagic, m_nCurrX, m_nCurrY, nil) then
              m_dwLastAttackTick := MyGetTickCount();

            m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
          end
          else
          begin
            // MainOutMessage('~~~~HeroAttack: ' + IntToStr(MyGetTickCount - m_dwLastAttackTick));
            AttackDir(nil, btAttack, btDir);
            m_dwLastAttackTick := MyGetTickCount();
            m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
            FRandomWalk := False;
            FLastAttack := True;
          end;
        end;
      end;
    end
    else if (not m_boFireHitSkill) and AllowUseMagic(SKILL_ERGUM) and ((OffsetX = 2) or (OffsetY = 2)) and
      ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) then
    begin
      if IsCanAttack and (MyGetTickCount - m_dwLastAttackTick >= GetAttackIntervalTime(True)) then
      begin
        { 野蛮冲撞 }
        if g_Config.boHeroCanUseMootebo and (not m_TargetCret.m_boStickMode) and AllowUseMagic(SKILL_MOOTEBO) and
          AllowHeroMagicRate(mtWarrAttack, SKILL_MOOTEBO) { (Random(4) = 0) } and (m_Abil.Level > m_TargetCret.m_Abil.Level) and
          ((MyGetTickCount - m_SkillUseTick[SKILL_MOOTEBO]) >= 1000 * 7) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_MOOTEBO);
          if (UserMagic <> nil) then
          begin
            m_SkillUseTick[SKILL_MOOTEBO] := MyGetTickCount;
            nSpellPoint := GetSpellPoint(UserMagic);
            if m_WAbil.MP >= nSpellPoint then
            begin
              if nSpellPoint > 0 then
              begin
                DamageSpell(nSpellPoint);
                HealthSpellChanged();
              end;
              m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
              if DoMotaebo(m_btDirection, UserMagic.btLevel) then
              begin
                m_dwMotaeboTick := MyGetTickCount;
                m_dwLastAttackTick := MyGetTickCount + 200;
                m_MoveToAttackTick := 0;

                FLastAttack := True;
                if UserMagic.btLevel < 3 then
                begin
                  if UserMagic.MagicInfo.TrainLevel[UserMagic.btLevel] < m_Abil.Level then
                  begin
                    TrainSkill(UserMagic, Random(3) + 1);
                    if not CheckMagicLevelup(UserMagic) then
                    begin
                      SendDelayMsg(Self, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
                        MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint,
                        IntToStr(Integer(UserMagic.MagicAttr)), 1000);
                    end;
                  end;
                end;
                Exit;
              end;
            end;
          end;
        end;

        if (not FErgumSkillUsed) or (Random(g_Config.dwHeroWarrAttakNear) > 0) then
        begin
          btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
          btAttack := 4;
          // MainOutMessage('~~~~HeroAttack: ' + IntToStr(MyGetTickCount - m_dwLastAttackTick));
          AttackDir(nil, btAttack, btDir);
          m_dwLastAttackTick := MyGetTickCount();
          m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
          FErgumSkillUsed := True;
          FRandomWalk := False;
          FLastAttack := True;
        end
        else
        begin
          btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
          GetBackPosition(btDir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, NewX, NewY, 1);
          SetTargetXY(NewX, NewY);
          HeroGotoTargetXY;
          FLastAttack := False;
        end;
      end;

      Result := True;
      Exit;
    end;

    IsFuckTarget := False;
    if g_Config.boHeroHitCmp and (MyGetTickCount - m_TargetCret.m_dwStationTick <= 600) and
      (m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    begin
      IsFuckTarget := ((MyGetTickCount - m_HeroMoveTick > GetMoveTime + g_Config.nWarrCmpInvTime) and
        (abs(m_TargetCret.m_nCurrX - m_nCurrX) >= 3) or (abs(m_TargetCret.m_nCurrY - m_nCurrY) >= 3))
    end;

    { 如果怪物不在攻击范围内，则走到怪身边去打 }
    if ((not Result) or IsFuckTarget) and (m_Master <> nil) and (m_PEnvir = m_Master.m_PEnvir) then
    begin
      // 设置目标后一直没有攻击则删除目标，因为是攻击不到了 chongchong 2017-12-11
      if (m_boTarget) and (m_TargetCret <> nil) and (MyGetTickCount - m_dwSetTargetCretTick >= 60000) and
        (MyGetTickCount - m_dwLastAttackTick >= 60000) then
      begin
        m_boTarget := False;
        DelTargetCreat;
        Exit;
      end;

      TempX := m_TargetCret.m_nCurrX;
      TempY := m_TargetCret.m_nCurrY;

      SetTargetXY(TempX, TempY);

      // 留下600的看能否保证英雄在跑的时候同步到了客户端，不然在客户端看起来还有3、4格，但服务器上只剩2格 chongchong 2017-11-18
      if (MyGetTickCount - m_HeroMoveTick >= 400) and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 2) and
        (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= 2) then
      begin
        HeroGotoTargetXY;
      end
      else
      begin
        if (abs(m_TargetCret.m_nCurrX - m_nCurrX) >= 3) or (abs(m_TargetCret.m_nCurrY - m_nCurrY) >= 3) then
        begin
          HeroRuntoTargetXY;
        end
        else
        begin
          if Random(6) = 0 then
            HeroGotoTargetXY
          else
            HeroRuntoTargetXY;
        end;
      end;
    end;
  except
    MainOutMessage('[Exception] THeroObject:HeroWarrAttackTarget');
  end;
end;

function THeroObject.HeroWizardAutoDun(AMaster: TPlayObject): Boolean;
var
  UserMagic: pTUserMagic;
begin
  Result := False;
  if ((not m_boIsDeputy) and AMaster.m_boHeroAutoShield) or (m_boIsDeputy and AMaster.m_boAssistantHeroAutoShield) and
    ((MyGetTickCount - m_dwLastAttackTick) > GetAttackIntervalTime(False)) then
  begin
    { 自动开启魔法盾 }
    if AllowUseMagic(SKILL_SHIELD) and (m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] = 0) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_SHIELD);
      if UserMagic <> nil then
      begin
        DoSpell(UserMagic, m_nCurrX, m_nCurrY, nil);
        m_dwLastAttackTick := MyGetTickCount;
        Result := True;
      end;
    end;
  end;
end;

function THeroObject.HeroWizardAttackTarget: Boolean; // 法师
var
  UserMagic: pTUserMagic;
  MySideCount: Integer;
  AttackSideCount: Integer;
  HasSingleMagic: Boolean;
  ObjMaster: TPlayObject;
  NoMagicAttack: Boolean;
  ErrCode: Integer;
  Event: TObject;
  OffsetX, OffsetY: Integer;
  IsCanAttack: Boolean;
  I: Integer;
  HeroMagic: PHeroMagic;
  CustomUserMagic: pTUserMagic;
  CustomMagicConfig: TCustomMagicConfig;
  IsUseCustomMagic: Boolean;
  CustomMagicTarget: TMagicAttackTarget;
  nWalkTime: Integer;
begin
  Result := False;

  if m_Master = nil then
    Exit;

  ErrCode := 0;
  try
    ObjMaster := TPlayObject(m_Master);
    // 法师英雄改成优化开盾 2017-10-28
    if HeroWizardAutoDun(ObjMaster) then
      Exit;

    ErrCode := 1;
    { TODO -ochongchong -c添加 : 道法无技能时使用物理攻击 【2013-08-11】 }
    NoMagicAttack := False;
    if (m_btAttackMode in [0, 3]) then
      NoMagicAttack := m_MagicList.Count = 0;

    ErrCode := 2;
    if NoMagicAttack then
    begin
      // 低血逃跑
      if m_WAbil.HP <= ObjMaster.m_nHeroDodgeHPPercent then
      begin
        if HeroAvoidTarget(m_TargetCret) then
          m_boLastAvoidTarget := True;
        Result := True;
      end
      else if m_TargetCret <> nil then
      begin
        Result := True;
        ErrCode := 3;
        if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 2) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 2) then
          HeroBasicAttackTarget
        else
        begin
          ErrCode := 4;
          SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);

          ErrCode := 5;
          if Random(5) = 0 then
            HeroGotoTargetXY
          else
            HeroRuntoTargetXY;
        end;
        ErrCode := 6;
      end;
      Exit;
    end;

    case m_btJob of
      0:
        nWalkTime := g_Config.dwHeroWarrorWalkTime;
      1:
        nWalkTime := g_Config.dwHeroAvoidTime; // g_Config.dwHeroWizardWalkTime;
      2:
        nWalkTime := g_Config.dwHeroAvoidTime; // g_Config.dwHeroTaoistWalkTime;
    else
      nWalkTime := 500;
    end;

    // MB的，又要参考LEG，躲避目标连续2次，围绕目标 chongchong 2018-07-20 22:55:28
    if m_boAvoidTargetFirst and (m_dwLastAttackTick >= m_dwMoveTimeTick) then
    begin
      if (MyGetTickCount - m_dwMoveTimeTick) > nWalkTime then
      begin
        HeroAvoidTargetNext;
        m_boAvoidTargetFirst := False;
        Result := True;
        Exit;
      end
      else
      begin
        Result := True;
        Exit;
      end;
    end
    else if (m_TargetCret <> nil) and (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 3) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 3)
      and (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) then
    begin
      if HeroAvoidTarget(m_TargetCret) then
      begin
        Result := True;
        m_boLastAvoidTarget := True;
        Exit;
      end;
    end;

    if (m_TargetCret <> nil) and g_Config.boHeroDFAvoidTargetRight and m_boAvoidTargetFirst and
      (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) then
    begin
      if HeroAvoidTargetNext() then
      begin
        Result := True;
        m_boLastAvoidTarget := True;
        Exit;
      end;
    end;

    // 躲避正在攻击自己的对象 chongchong 2015-05-05
    if (m_LastHiter <> nil) and ((m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) or
      (not g_Config.boHeroNotAvoidLastHinter)) and (abs(m_nCurrX - m_LastHiter.m_nCurrX) < 3) and
      (abs(m_nCurrY - m_LastHiter.m_nCurrY) < 3) and (m_TargetCret <> nil
      { Random(2) = 0 无目标是跟随主人，这个容易导致跟随主人有问题 2017-12-14 } ) and
      (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) then
    begin
      if HeroAvoidTarget(m_LastHiter) then
      begin
        Result := True;
        m_boLastAvoidTarget := True;
        Exit;
      end;
    end;

    if (m_LastHiter <> nil) and ((m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) or
      (not g_Config.boHeroNotAvoidLastHinter)) and g_Config.boHeroDFAvoidTargetRight and m_boAvoidTargetFirst and
      (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) then
    begin
      if HeroAvoidTargetNext() then
      begin
        Result := True;
        m_boLastAvoidTarget := True;
        Exit;
      end;
    end;

    if (m_TargetCret <> nil) and (abs(m_nCurrX - m_TargetCret.m_nCurrX) < g_Config.nMagicAttackRage) and
      (abs(m_nCurrY - m_TargetCret.m_nCurrY) < g_Config.nMagicAttackRage) and (m_TargetCret.m_PEnvir = m_PEnvir) then
    begin // 006329
      ErrCode := 7;
      { 没有魔法，删除目标 }
      if (m_WAbil.MP <= 3) then
      begin
        DelTargetCreat;
        Exit;
      end;

      // 不追目标，出刀在移动后一段时间，不然跑步前去攻击时，前三刀过快

      // 这样搞的原因是如果仅仅用(MyGetTickCount - m_dwStationTick >= GetAttackIntervalTime(False))这样来判断，前三刀过快是处理了，但躲避速度又变慢
      {
        if m_boLastAvoidTarget then
        IsCanAttack := True
        else
        IsCanAttack := (MyGetTickCount - m_dwStationTick >= GetAttackIntervalTime(False));
      }
      // NND，道法英雄在追击的时候，又说攻击速度太慢 fuck 2018-07-10 11:06:47
      IsCanAttack := True;

      ErrCode := 8;
      if ((MyGetTickCount - m_dwLastAttackTick) > GetAttackIntervalTime(False)) and IsCanAttack then
      begin
        OffsetX := abs(m_TargetCret.m_nCurrY - m_nCurrY);
        OffsetY := abs(m_TargetCret.m_nCurrX - m_nCurrX);

        if (m_WAbil.MP > 3) and (g_Config.btHeroRecallCopySelfHPRate < 100) and AllowUseMagic(SKILL_74) and
          (GetCopyHumanCount = 0) and (m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * g_Config.btHeroRecallCopySelfHPRate)) and
          (MyGetTickCount - m_SkillUseTick[SKILL_74] > GetMagicCD(SKILL_74)) then
        begin
          ErrCode := 9;
          UserMagic := GetMagicInfoEx(SKILL_74);
          ErrCode := 10;
          DoSpell(UserMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret);
          m_dwLastAttackTick := MyGetTickCount;
          m_MoveToAttackTick := MyGetTickCount - m_HeroMoveTick;
          m_dwMoveTimeTick := MyGetTickCount;
        end
        else if (m_WAbil.MP > 3) and (m_WAbil.HP > ObjMaster.m_nHeroDodgeHPPercent) then
        begin
          ErrCode := 11;
          MySideCount := GetMapBaseObjectCount(m_PEnvir, m_nCurrX, m_nCurrY, 1);
          AttackSideCount := GetMapBaseObjectCount(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 1);
          UserMagic := nil;

          HasSingleMagic := AllowUseMagic(SKILL_45 { 灭天火 } ) or AllowUseMagic(SKILL_LIGHTENING { 雷电术 } ) or
            AllowUseMagic(SKILL_FIRE
            { 地狱火 } ) or AllowUseMagic(SKILL_44 { 寒冰掌 } ) or AllowUseMagic(SKILL_FIREBALL { 火球 } ) or
            AllowUseMagic(SKILL_FIREBALL2
            { 大火球 } );

          ErrCode := 12;
          Randomize;

          Event := m_PEnvir.GetEvent(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);

          { 74 分身术 }
          if (g_Config.btHeroRecallCopySelfHPRate = 100) and AllowUseMagic(SKILL_74) and (GetCopyHumanCount = 0) and
            (MyGetTickCount - m_SkillUseTick[SKILL_74] > GetMagicCD(SKILL_74)) and AllowHeroMagicRate(mtWizardAttack, SKILL_74)
          then
          begin
            UserMagic := GetMagicInfoEx(SKILL_74);
          end
          { 75 护体神盾 }
          else if (not m_boSuperShiled) and AllowUseMagic(SKILL_75) and (not g_Config.boHeroAutoSuperShiled) and
            ((MyGetTickCount - m_dwLastSuperShiledTimeTick) > GetMagicCD(SKILL_75)) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_75)
          { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_75);
          end
          // 倚天劈地 chongchong 2013-12-11
          else if AllowUseMagic(SKILL_114) and (MyGetTickCount - m_SkillUseTick[SKILL_114] > GetMagicCD(SKILL_114)
            { g_Config.nHeroSkill114HitWaitTime * 1000 } ) and
            ((abs(m_TargetCret.m_nCurrY - m_nCurrY) <= g_Config.nSkill114AttackRange) and
            (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= g_Config.nSkill114AttackRange)) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_114) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_114);
          end
          { 5 血魄一击(法) }
          else if AllowUseMagic(SKILL_116) and AllowHeroMagicRate(mtWizardAttack, SKILL_116) and
            (MyGetTickCount - m_SkillUseTick[SKILL_116] > GetMagicCD(SKILL_116) { 1000 * g_Config.nHeroSkill116CD } ) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_116) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_116);
          end
          { 31 魔法盾 }
          else if AllowUseMagic(SKILL_SHIELD) and (m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] = 0) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_SHIELD) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_SHIELD);
          end
          { 8 抗拒火环 }
          else if AllowUseMagic(SKILL_FIREWIND) and (m_Abil.Level > m_TargetCret.m_Abil.Level) and (MySideCount > 0) and
            (Random(4) <= MySideCount) and AllowHeroMagicRate(mtWizardAttack, SKILL_FIREWIND) { (Random(2) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_FIREWIND);
          end
          { 47 火龙烈焰 }
          else if AllowUseMagic(SKILL_47) and ((AttackSideCount > 1) or (not HasSingleMagic)) and
            AllowHeroMagicRate2(mtWizardAttack, SKILL_47, AttackSideCount) { (Random(10) <= AttackSideCount) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_47);
          end
          { 33 冰咆哮 }
          else if AllowUseMagic(SKILL_SNOWWIND) and ((AttackSideCount > 1) or (not HasSingleMagic)) and
            (MyGetTickCount - m_SkillUseTick[SKILL_SNOWWIND] > GetMagicCD(SKILL_SNOWWIND)) and
            AllowHeroMagicRate2(mtWizardAttack, SKILL_SNOWWIND, AttackSideCount) { (Random(8) <= AttackSideCount) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_SNOWWIND);
          end
          { 205 冰霜雪雨 }
          else if AllowUseMagic(SKILL_205) and ((AttackSideCount > 1) or (not HasSingleMagic)) and
            (MyGetTickCount - m_SkillUseTick[SKILL_205] > GetMagicCD(SKILL_205)) and
            AllowHeroMagicRate2(mtWizardAttack, SKILL_205, AttackSideCount)
          { (Random(8) <= AttackSideCount) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_205);
          end
          { 206 冰霜群雨 }
          else if AllowUseMagic(SKILL_206) and ((AttackSideCount > 1) or (not HasSingleMagic)) and
            (MyGetTickCount - m_SkillUseTick[SKILL_206] > GetMagicCD(SKILL_206)) and
            AllowHeroMagicRate2(mtWizardAttack, SKILL_206, AttackSideCount)
          { (Random(8) <= AttackSideCount) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_206);
          end
          { 209 五雷轰 }
          else if AllowUseMagic(SKILL_209) and ((AttackSideCount > 1) or (not HasSingleMagic)) and
            (MyGetTickCount - m_SkillUseTick[SKILL_209] > GetMagicCD(SKILL_209)) and
            AllowHeroMagicRate2(mtWizardAttack, SKILL_209, AttackSideCount)
          { (Random(8) <= AttackSideCount) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_209);
          end
          { 45 灭天火 }
          else if AllowUseMagic(SKILL_45) and (MyGetTickCount - m_SkillUseTick[SKILL_45] > GetMagicCD(SKILL_45)) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_45) { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_45);
          end
          { 11 雷电术 }
          else if AllowUseMagic(SKILL_LIGHTENING) and AllowHeroMagicRate(mtWizardAttack, SKILL_LIGHTENING) { (Random(3) = 0) }
          then
          begin
            if AllowUseMagic(SKILL_LIGHTENING) then
              UserMagic := GetMagicInfoEx(SKILL_LIGHTENING);
          end { 37 英雄群雷术 }
          else if AllowUseMagic(SKILL_GROUPLIGHTENING) and AllowHeroMagicRate(mtWizardAttack, SKILL_GROUPLIGHTENING)
          { (Random(3) = 0) } then
          begin
            if AllowUseMagic(SKILL_GROUPLIGHTENING) and ((AttackSideCount >= 2) or (not HasSingleMagic)) and
              AllowHeroMagicRate(mtWizardAttack, SKILL_GROUPLIGHTENING) then
            begin
              UserMagic := GetMagicInfoEx(SKILL_GROUPLIGHTENING);
            end;
          end { 22 火墙 }
          else if AllowUseMagic(SKILL_EARTHFIRE) and ((AttackSideCount >= 1) or (not HasSingleMagic)) and
            AllowHeroMagicRate2(mtWizardAttack, SKILL_EARTHFIRE, AttackSideCount) { (Random(8) <= AttackSideCount) } and
            ((Event = nil) or (not(Event is TFireBurnEvent)) or (TFireBurnEvent(Event).m_boClose)) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_EARTHFIRE);
          end
          { 24 地狱雷光 }
          else if AllowUseMagic(SKILL_LIGHTFLOWER) and (m_TargetCret.m_btLifeAttrib = LA_UNDEAD) and
            ((AttackSideCount >= 4) or (not HasSingleMagic)) and AllowHeroMagicRate(mtWizardAttack, SKILL_LIGHTFLOWER)
          { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_LIGHTFLOWER);
          end
          { 23 爆裂火焰 }
          else if AllowUseMagic(SKILL_FIREBOOM) and ((AttackSideCount > 1) or (not HasSingleMagic)) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_FIREBOOM) { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_FIREBOOM);
          end
          { 45 灭天火 }
          { else if AllowUseMagic(SKILL_450) and ((Random(2) = 0) or (m_HeroMagic[11] = nil)) then
            begin
            UserMagic := GetMagicInfoEx(SKILL_45);
            end }
          { 58 流星火雨 }
          else if AllowUseMagic(SKILL_58) and AllowHeroMagicRate(mtWizardAttack, SKILL_58) { (Random(2) = 0) } and
            ((AttackSideCount > 1) or (not HasSingleMagic)) and (MyGetTickCount - m_SkillUseTick[SKILL_58] > GetMagicCD(SKILL_58)
            { 1000 * g_Config.nHeroSkill58WaitTime } ) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_58);
          end
          { 英雄先天元力 }
          else if AllowUseMagic(SKILL_67) and AllowHeroMagicRate(mtWizardAttack, SKILL_67) { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_67);
          end
          { 英雄酒气护体 }
          else if AllowUseMagic(SKILL_68) and AllowHeroMagicRate(mtWizardAttack, SKILL_68) { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_68);
          end
          { 英雄地狱火 }
          else if AllowUseMagic(SKILL_FIRE) and AllowHeroMagicRate(mtWizardAttack, SKILL_FIRE) { (Random(3) = 0) } and
            ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_FIRE);
          end
          { 英雄诱惑之光 }
          else if AllowUseMagic(SKILL_TAMMING) and AllowHeroMagicRate(mtWizardAttack, SKILL_TAMMING) { (Random(7) = 0) } and
          { 诱惑之光不对人物使用 }
            (m_TargetCret.m_btRaceServer >= RC_ANIMAL) and
            (not(m_TargetCret.m_btRaceServer in [RC_PLAYMOSTER { 人形怪 } , RC_ARCHERGUARD { 弓箭手 } , RC_MOVE_ARCHERGUARD { 巡回弓箭手 } ,
            RC_TRUCKOBJECT { 押镖车 } , 55 { 练功师 } , 110, 111 { 沙巴克城墙 } ])) and (m_TargetCret.m_Master <> Self) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_TAMMING);
          end
          { 英雄疾光电影 }
          else if AllowUseMagic(SKILL_SHOOTLIGHTEN) and AllowHeroMagicRate(mtWizardAttack, SKILL_SHOOTLIGHTEN)
          { (Random(3) = 0) } and ((OffsetX = OffsetY) or (OffsetX = 0) or (OffsetY = 0)) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_SHOOTLIGHTEN);
          end
          { 英雄寒冰掌 }
          else if AllowUseMagic(SKILL_44) and AllowHeroMagicRate(mtWizardAttack, SKILL_44) { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_44);
          end
          { 英雄圣言术 }
          else if AllowUseMagic(SKILL_KILLUNDEAD) and (m_TargetCret.m_btLifeAttrib = LA_UNDEAD) and
            AllowHeroMagicRate(mtWizardAttack, SKILL_KILLUNDEAD) { (Random(3) = 0) } then
          begin
            UserMagic := GetMagicInfoEx(SKILL_KILLUNDEAD);
          end
          { 1 火球术 }
          else if AllowUseMagic(SKILL_FIREBALL) and AllowHeroMagicRate(mtWizardAttack, SKILL_FIREBALL) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_FIREBALL);
          end
          { 5 大火球术 }
          else if AllowUseMagic(SKILL_FIREBALL2) and AllowHeroMagicRate(mtWizardAttack, SKILL_FIREBALL2) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_FIREBALL2);
          end;

          IsUseCustomMagic := False;
          CustomMagicTarget := matEnemy;

          if UserMagic = nil then
          begin
            for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
            begin
              HeroMagic := g_CustomHeroMagicMgr.Items[I];
              if (HeroMagic.MagicType = mtWizardAttack) and HeroMagic.IsCustomMagic then
              begin
                CustomUserMagic := m_CustomSkill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID];
                if (CustomUserMagic <> nil) and (CustomUserMagic.btKey > 0) and
                  AllowHeroMagicRate(mtWizardAttack, HeroMagic.MagicID) then
                begin
                  if (HeroMagic.AttackTarget = matEnemy) then
                  begin
                    // 判断执行条件 chongchong 2018-01-06
                    if CheckHeroMagicUseCondition(@HeroMagic.Condition, m_TargetCret) then
                    begin
                      CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                      if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                      begin
                        if AllowCustomSkill(HeroMagic.MagicID, True) then
                        begin
                          UserMagic := CustomUserMagic;
                          IsUseCustomMagic := True;
                          CustomMagicTarget := matEnemy;
                          Break;
                        end;
                      end;
                    end;
                  end
                  else if (HeroMagic.AttackTarget = matSelf) then
                  begin
                    // 判断执行条件 chongchong 2018-01-06
                    if CheckHeroMagicUseCondition(@HeroMagic.Condition, Self) then
                    begin
                      CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                      if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                      begin
                        if AllowCustomSkill(HeroMagic.MagicID, True) then
                        begin
                          UserMagic := CustomUserMagic;
                          IsUseCustomMagic := True;
                          CustomMagicTarget := matSelf;
                          Break;
                        end;
                      end;
                    end;
                  end
                  else if (HeroMagic.AttackTarget = matMaster) then
                  begin
                    if (abs(m_nCurrX - m_Master.m_nCurrX) < g_Config.nMagicAttackRage) and
                      (abs(m_nCurrY - m_Master.m_nCurrY) < g_Config.nMagicAttackRage) and
                      CheckHeroMagicUseCondition(@HeroMagic.Condition, m_Master) then
                    begin
                      CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                      if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                      begin
                        if AllowCustomSkill(HeroMagic.MagicID, True) then
                        begin
                          UserMagic := CustomUserMagic;
                          IsUseCustomMagic := True;
                          CustomMagicTarget := matMaster;
                          Break;
                        end;
                      end;
                    end;
                  end;
                end;
              end
            end;
          end;

          if UserMagic <> nil then
          begin
            Result := True;
            ErrCode := 13;
            // 地狱雷光 (跑到怪物面前去打，不然打不到)
            if UserMagic.wMagIdx = SKILL_LIGHTFLOWER then // 修复英雄不放地狱雷光  By 一支笔 at:2021-08-12 14:38:06
            begin
              ErrCode := 14;
              if (abs(m_nCurrX - m_TargetCret.m_nCurrX) > 2) or (abs(m_nCurrY - m_TargetCret.m_nCurrY) > 2) then
              begin
                ErrCode := 15;
                m_nTargetX := m_TargetCret.m_nCurrX;
                m_nTargetY := m_TargetCret.m_nCurrY;
                HeroAutoMove;
              end;
              if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 3) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 3) then
              begin
                DoSpell(UserMagic, m_nCurrX, m_nCurrY, m_TargetCret);
                m_dwLastAttackTick := MyGetTickCount;
                m_dwMoveTimeTick := MyGetTickCount;
              end;
            end
            else if UserMagic.wMagIdx = SKILL_75 then
            begin
              ErrCode := 16;
              OpenSuperShiled;
              m_dwLastAttackTick := MyGetTickCount;
              m_dwMoveTimeTick := MyGetTickCount;
            end
            else
            begin // 魔法攻击时判断是否能打中目标，如果打不中，就跑一下 chongchong 2013-12-30
              if (UserMagic.wMagIdx in [SKILL_FIREBALL, SKILL_FIREBALL2 { 火球，大火球 } , SKILL_FIRECHARM { 灵魂火符 } , SKILL_44 { 寒冰掌 } ,
                SKILL_MABE { 火焰冰 } , SKILL_105 { 惊雷爆 } , SKILL_107 { 双龙破 } , SKILL_108 { 虎啸诀 } , SKILL_109 { 八卦掌 } , SKILL_110
                { 三焰咒 }
                ]) and MagCanHitTarget(m_nCurrX, m_nCurrY, m_TargetCret) then
              begin
                ErrCode := 17;
                DoSpell(UserMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret);
                m_dwLastAttackTick := MyGetTickCount;
                m_dwMoveTimeTick := MyGetTickCount;
              end
              else if not(UserMagic.wMagIdx in [SKILL_FIREBALL, SKILL_FIREBALL2 { 火球，大火球 } , SKILL_FIRECHARM { 灵魂火符 } , SKILL_44
                { 寒冰掌 } , SKILL_MABE { 火焰冰 } , SKILL_105 { 惊雷爆 } , SKILL_107 { 双龙破 } , SKILL_108 { 虎啸诀 } , SKILL_109 { 八卦掌 } ,
                SKILL_110
                { 三焰咒 } ]) then
              begin
                ErrCode := 17;

                if IsUseCustomMagic then
                begin
                  if CustomMagicTarget = matEnemy then
                  begin
                    DoSpell(UserMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret);
                    m_dwLastAttackTick := MyGetTickCount;
                    m_dwMoveTimeTick := MyGetTickCount;
                  end
                  else if CustomMagicTarget = matSelf then
                  begin
                    DoSpell(UserMagic, Self.m_nCurrX, Self.m_nCurrY, Self);
                    m_dwLastAttackTick := MyGetTickCount;
                    m_dwMoveTimeTick := MyGetTickCount;
                  end
                  else if CustomMagicTarget = matMaster then
                  begin
                    DoSpell(UserMagic, m_Master.m_nCurrX, m_Master.m_nCurrY, m_Master);
                    m_dwLastAttackTick := MyGetTickCount;
                    m_dwMoveTimeTick := MyGetTickCount;
                  end;
                end
                else
                begin
                  DoSpell(UserMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret);
                  m_dwLastAttackTick := MyGetTickCount;
                  m_dwMoveTimeTick := MyGetTickCount;
                end;
              end
              else
              begin
                ErrCode := 18;
                SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                HeroGotoTargetXY;
                Exit;
              end;
            end;
          end;
        end;
      end;
    end
    else if { m_boTarget and } (m_TargetCret <> nil) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boDeath) then
    begin
      /// else 如果目标不在攻击范围，并且锁定对象，再往目标那边跑 chongchong 2013-12-27
      ErrCode := 29;
      SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
      ErrCode := 30;
      HeroRuntoTargetXY;
      Result := True;
    end;
  except
    MainOutMessage('[Exception] THeroObject:HeroWizardAttackTarget; Error=' + IntToStr(ErrCode));
  end;
end;

function THeroObject.HeroTaosAttackTarget: Boolean;
var
  btDir: Byte;
  // boSet:Boolean;
  UserMagic: pTUserMagic;
  TargeTBaseObject: TBaseObject;
  TargeX, TargeY: Integer;
  MySideCount, AttackSideCount: Integer;
  n14: Byte;
  HaveBBCount: Integer;
  HaveBBType: Integer;
  NoMagicAttact: Boolean;
  Index: Integer;
  TestMagic: pTUserMagic;
  HeroMagic: PHeroMagic;
  CustomUserMagic: pTUserMagic;
  CustomMagicConfig: TCustomMagicConfig;

  { 检测毒时，用完全匹配检测，不然只有一种毒时，打另一种失败，就会不停的打毒 }

  function CheckAmulet(nShape, nCount: Integer): Boolean;
  begin
    Result := CheckUserItemTypeEx(Self, nShape, nCount) or
      ((g_Config.nHeroNeedMagicItem = 2) and (GetUserItemListEx(Self, nShape, nCount) >= 0)) or (g_Config.nHeroNeedMagicItem = 0);
  end;

  procedure ProtectFirend(btBy: Byte);
  var
    I, Count: Integer;
    BaseObjectList: TList;
    BaseObject: TBaseObject;
  begin
    if (TargeTBaseObject = nil) or (UserMagic <> nil) then
      Exit;

    if (m_TargetCret <> nil) or g_Config.boHeroNoTargetRecallBB then
    begin
      HaveBBCount := 0;
      if g_Config.boHeroCanCallBB then
      begin
        if g_Config.dwHeroCallBBCount = 0 then
        begin
          if (UserMagic = nil) and (MyGetTickCount - m_SkillUseTick[SKILL_55] > 1000 * 10) and AllowUseMagic(SKILL_55) and
            CheckUserItem(Self, 5, 5) and AllowRecallMoonSlave (* 本函数会根据人物设置的月灵数量来判断是否可创建月灵，在此取消 *) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_55);
          end;
          if (UserMagic = nil) and (MyGetTickCount - m_SkillUseTick[SKILL_SINSU] > 1000 * 10) and AllowUseMagic(SKILL_SINSU) and
            CheckUserItem(Self, 5, 5) and AllowRecallSlaveCount(SKILL_SINSU) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_SINSU);
          end;
          if (UserMagic = nil) and (MyGetTickCount - m_SkillUseTick[SKILL_SKELLETON] > 1000 * 10) and
            AllowUseMagic(SKILL_SKELLETON) and CheckUserItem(Self, 5, 1) and AllowRecallSlaveCount(SKILL_SKELLETON) then
          begin
            UserMagic := GetMagicInfoEx(SKILL_SKELLETON);
          end;
        end
        else
        begin

          HaveBBCount := GetSlaveCount(bb_MonthSpirit);
          if HaveBBCount > 0 then
            HaveBBType := 1
          else
          begin
            HaveBBCount := GetSlaveCount(bb_Dogz);
            if HaveBBCount > 0 then
              HaveBBType := 2
            else
            begin
              HaveBBCount := GetSlaveCount(bb_BoneFamm);
              if HaveBBCount > 0 then
                HaveBBType := 3
            end;
          end;

          if (HaveBBCount > 0) then
          begin
            if HaveBBCount < g_Config.dwHeroCallBBCount then
            begin
              case HaveBBType of
                1: { 55 召唤月灵 }
                  if (MyGetTickCount - m_SkillUseTick[SKILL_55] > 1000 * 10) and AllowUseMagic(SKILL_55) and
                    CheckUserItem(Self, 5, 5) { and AllowRecallMoonSlave (*本函数会根据人物设置的月灵数量来判断是否可创建月灵，在此取消*) } then
                  begin
                    UserMagic := GetMagicInfoEx(SKILL_55);
                  end;

                2: { 30 召唤神兽 }
                  if (MyGetTickCount - m_SkillUseTick[SKILL_SINSU] > 1000 * 10) and AllowUseMagic(SKILL_SINSU) and
                    CheckUserItem(Self, 5, 5) { and AllowRecallSlaveCount(SKILL_SINSU) } then
                  begin
                    UserMagic := GetMagicInfoEx(SKILL_SINSU);
                  end;
                3: { 17 召唤骷髅 }
                  if (MyGetTickCount - m_SkillUseTick[SKILL_SKELLETON] > 1000 * 10) and AllowUseMagic(SKILL_SKELLETON) and
                    CheckUserItem(Self, 5, 1) { and AllowRecallSlaveCount(SKILL_SKELLETON) } then
                  begin
                    UserMagic := GetMagicInfoEx(SKILL_SKELLETON);
                  end;
              end;
            end;
          end
          else
          begin
            { 55 召唤月灵 }
            if (MyGetTickCount - m_SkillUseTick[SKILL_55] > 1000 * 10) and AllowUseMagic(SKILL_55) and CheckUserItem(Self, 5, 5)
            { and AllowRecallMoonSlave } then
              UserMagic := GetMagicInfoEx(SKILL_55)
              { 30 召唤神兽 }
            else if (MyGetTickCount - m_SkillUseTick[SKILL_SINSU] > 1000 * 10) and AllowUseMagic(SKILL_SINSU) and
              CheckUserItem(Self, 5, 5) { and AllowRecallSlaveCount(SKILL_SINSU) } then
              UserMagic := GetMagicInfoEx(SKILL_SINSU)
              { 17 召唤骷髅 }
            else if (MyGetTickCount - m_SkillUseTick[SKILL_SKELLETON] > 1000 * 10) and AllowUseMagic(SKILL_SKELLETON) and
              CheckUserItem(Self, 5, 1) { and AllowRecallSlaveCount(SKILL_SKELLETON) } then
              UserMagic := GetMagicInfoEx(SKILL_SKELLETON);
          end;
        end;
      end;

      if (UserMagic <> nil) then
        Exit;
    end;

    { 15 神圣战甲术 防 }
    if (m_TargetCret <> nil) and AllowUseMagic(SKILL_DEJIWONHO) and (TargeTBaseObject = Self.m_Master) and
      (TargeTBaseObject.m_wStatusTimeArr[STATE_DEFENCEUP] = 0) and
    // (MyGetTickCount - m_SkillUseTick[SKILL_DEJIWONHO] > 1000) and
      CheckUserItem(Self, 5, 1) and AllowHeroMagicRate(mtTaosAttack, SKILL_DEJIWONHO) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_DEJIWONHO);
    end
    { 14 幽灵盾 魔 }
    else if (m_TargetCret <> nil) and AllowUseMagic(SKILL_HANGMAJINBUB) and
      (TargeTBaseObject.m_wStatusTimeArr[STATE_MAGDEFENCEUP] = 0) and (TargeTBaseObject = Self.m_Master) and
    // (MyGetTickCount - m_SkillUseTick[SKILL_HANGMAJINBUB] > 1000) and
      CheckUserItem(Self, 5, 1) and AllowHeroMagicRate(mtTaosAttack, SKILL_HANGMAJINBUB) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_HANGMAJINBUB);
    end
    { 34 解毒术 }
    else if AllowUseMagic(SKILL_UNAMYOUNSUL) and ((TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <> 0) or
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <> 0) or (TargeTBaseObject.m_wStatusTimeArr[POISON_STONE] <> 0)) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_UNAMYOUNSUL) { (Random(btBy) = 0) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_UNAMYOUNSUL);
    end
    { 2 治愈术 }
    else if ((AllowUseMagic(SKILL_HEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_HEALLING] >= SKILL_HEALLING_CD_MIN)) or
      (AllowUseMagic(SKILL_BIGHEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_BIGHEALLING] >= SKILL_HEALLING_CD_MIN))) and
      (((m_TargetCret <> nil) and (TargeTBaseObject.m_WAbil.HP < Round(TargeTBaseObject.m_WAbil.MaxHP / 100 * 80))) or
      ((m_TargetCret = nil) and (TargeTBaseObject.m_WAbil.HP < TargeTBaseObject.m_WAbil.MaxHP))) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_HEALLING) { (Random(btBy) = 0) } then
    begin
      if AllowUseMagic(SKILL_HEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_HEALLING] >= SKILL_HEALLING_CD_MIN) then
        UserMagic := GetMagicInfoEx(SKILL_HEALLING);

      if not(AllowUseMagic(SKILL_BIGHEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_BIGHEALLING] >= SKILL_HEALLING_CD_MIN))
      then
        Exit;

      Count := 0;
      BaseObjectList := TList.Create;
      try
        TargeTBaseObject.GetMapBaseObjects(TargeTBaseObject.m_PEnvir, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY, 2,
          BaseObjectList);
        for I := 0 to BaseObjectList.Count - 1 do
        begin
          BaseObject := TBaseObject(BaseObjectList[I]);
          if TargeTBaseObject.IsProperFriend(BaseObject) then
          begin
            if BaseObject.m_WAbil.HP < Round(BaseObject.m_WAbil.MaxHP / 100 * 90) then
            begin
              Inc(Count);
              if Count >= 2 then
              begin
                UserMagic := GetMagicInfoEx(SKILL_BIGHEALLING);
                Break;
              end;
            end;
          end;
        end;
      finally
        BaseObjectList.Free;
      end;
    end
    { 75 护体神盾 }
    else if (TargeTBaseObject = Self) and (not m_boSuperShiled) and AllowUseMagic(SKILL_75) and
      (not g_Config.boHeroAutoSuperShiled) and ((MyGetTickCount - m_dwLastSuperShiledTimeTick) > GetMagicCD(SKILL_75)) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_75)
    { (Random(3) = 0) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_75);
    end;

    if UserMagic = nil then
    begin
      for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
      begin
        HeroMagic := g_CustomHeroMagicMgr.Items[I];
        if (HeroMagic.MagicType = mtTaosAttack) and HeroMagic.IsCustomMagic then
        begin
          CustomUserMagic := m_CustomSkill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID];
          if (CustomUserMagic <> nil) and (CustomUserMagic.btKey > 0) and AllowHeroMagicRate(mtTaosAttack, HeroMagic.MagicID) then
          begin
            if (HeroMagic.AttackTarget = matSelf) then
            begin
              // 判断执行条件 chongchong 2018-01-06
              if (TargeTBaseObject = Self) and CheckHeroMagicUseCondition(@HeroMagic.Condition, TargeTBaseObject) then
              begin
                CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                begin
                  if AllowCustomSkill(HeroMagic.MagicID, True) then
                  begin
                    UserMagic := CustomUserMagic;
                    Break;
                  end;
                end;
              end;
            end
            else if (HeroMagic.AttackTarget = matMaster) then
            begin
              if (TargeTBaseObject = m_Master) and CheckHeroMagicUseCondition(@HeroMagic.Condition, TargeTBaseObject) then
              begin
                CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                begin
                  if AllowCustomSkill(HeroMagic.MagicID, True) then
                  begin
                    UserMagic := CustomUserMagic;
                    Break;
                  end;
                end;
              end;
            end
            else if (HeroMagic.AttackTarget = matPartner) then
            begin
              if CheckHeroMagicUseCondition(@HeroMagic.Condition, TargeTBaseObject) then
              begin
                CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                begin
                  if AllowCustomSkill(HeroMagic.MagicID, True) then
                  begin
                    UserMagic := CustomUserMagic;
                    Break;
                  end;
                end;
              end;
            end;
          end;
        end
      end;
    end;
  end;

  procedure ProtectHuman(btBy: Byte);
  begin
    if (m_Master = nil) or (UserMagic <> nil) then
      Exit;
    TargeTBaseObject := m_Master;
    TargeX := m_Master.m_nCurrX;
    TargeY := m_Master.m_nCurrY;
    ProtectFirend(btBy);
  end;

  procedure ProtectSelf(btBy: Byte);
  begin
    if UserMagic <> nil then
      Exit;
    TargeTBaseObject := Self;
    TargeX := m_nCurrX;
    TargeY := m_nCurrY;
    ProtectFirend(btBy);
  end;

  procedure AttackMonster(btBy: Byte);
  var
    I: Integer;
    TempMagic: pTUserMagic;
    SkillAmyounsulRate: Integer; // 施毒几率
    NoSigleMagic: Boolean;
  begin
    if (UserMagic <> nil) or (m_TargetCret = nil) then
      Exit;

    TargeTBaseObject := m_TargetCret;
    TargeX := m_TargetCret.m_nCurrX;
    TargeY := m_TargetCret.m_nCurrY;

    // 防毒几率越高，使用几率越小
    if (TargeTBaseObject.m_WAbil.NewValue[16] <= 20) then
      SkillAmyounsulRate := 1
    else if (TargeTBaseObject.m_WAbil.NewValue[16] <= 50) then
      SkillAmyounsulRate := 2
    else if (TargeTBaseObject.m_WAbil.NewValue[16] <= 80) then
      SkillAmyounsulRate := 3
    else
      SkillAmyounsulRate := 4;

    // 中毒躲避几率越高，使用几率越小
    if TargeTBaseObject.m_btAntiPoison >= 160 then
    begin
      SkillAmyounsulRate := SkillAmyounsulRate * 2;
      if SkillAmyounsulRate < 40 then
        SkillAmyounsulRate := 40;
    end
    else if TargeTBaseObject.m_btAntiPoison >= 80 then
    begin
      SkillAmyounsulRate := SkillAmyounsulRate * 2;
      if SkillAmyounsulRate < 20 then
        SkillAmyounsulRate := 20;
    end
    else if TargeTBaseObject.m_btAntiPoison >= 40 then
    begin
      SkillAmyounsulRate := SkillAmyounsulRate * 2;
      if SkillAmyounsulRate < 10 then
        SkillAmyounsulRate := 10;
    end
    else if TargeTBaseObject.m_btAntiPoison >= 20 then
    begin
      SkillAmyounsulRate := SkillAmyounsulRate * 2;
      if SkillAmyounsulRate < 6 then
        SkillAmyounsulRate := 6;
    end
    else if TargeTBaseObject.m_btAntiPoison >= 10 then
    begin
      SkillAmyounsulRate := SkillAmyounsulRate * 2;
      if SkillAmyounsulRate < 4 then
        SkillAmyounsulRate := 4;
    end;

    TempMagic := GetMagicInfoEx(SKILL_50);

    NoSigleMagic := (not AllowUseMagic(SKILL_AMYOUNSUL) { 施毒 } ) and (not AllowUseMagic(SKILL_57) { 嗜血 } ) and
      (not AllowUseMagic(SKILL_FIRECHARM)
      { 灵魂火符 } );

    // 倚天劈地 chongchong 2013-12-11
    if AllowUseMagic(SKILL_114) and (MyGetTickCount - m_SkillUseTick[SKILL_114] > GetMagicCD(SKILL_114)
      { g_Config.nHeroSkill114HitWaitTime * 1000 } ) and ((abs(m_TargetCret.m_nCurrY - m_nCurrY) <= g_Config.nSkill114AttackRange)
      and (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= g_Config.nSkill114AttackRange)) and AllowHeroMagicRate(mtTaosAttack, SKILL_114)
    then
    begin
      UserMagic := GetMagicInfoEx(SKILL_114);
    end
    { 5 血魄一击(法) }
    else if AllowUseMagic(SKILL_117) and AllowHeroMagicRate(mtTaosAttack, SKILL_117) and
      (MyGetTickCount - m_SkillUseTick[SKILL_117] > GetMagicCD(SKILL_117) { 1000 * g_Config.nHeroSkill117CD } ) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_117) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_117);
    end
    { 50 无极真气 }
    else if AllowUseMagic(SKILL_50) and (m_wStatusArrValue[2] = 0) and // 无极真气冷却时间 chongchong 2013-11-21
      (MyGetTickCount - m_SkillUseTick[SKILL_50] >= GetMagicCD(SKILL_50)) and (TempMagic <> nil) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_50) then // 无极真气使用频率降低 Random(btBy)
    begin
      UserMagic := TempMagic;
      // AbilityUp(TempMagic, 2);
    end
    { 51 群体施毒术 中毒类型 - 红毒 }
    else if AllowUseMagic(SKILL_GROUPAMYOUNSUL) and
      (MyGetTickCount - m_SkillUseTick[SKILL_GROUPAMYOUNSUL] >= GetMagicCD(SKILL_GROUPAMYOUNSUL)) and
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0) and CheckAmulet(2, 1) and // CheckUserItem(Self, 2, 1) and
      ((AttackSideCount > 1) or NoSigleMagic) and AllowHeroMagicRate2(mtTaosAttack, SKILL_GROUPAMYOUNSUL, AttackSideCount + 1)
    { (Random(6) < AttackSideCount) } then
    begin
      // m_HeroAttackAmuPorc := m_HeroAmu21Porc;
      UserMagic := GetMagicInfoEx(SKILL_GROUPAMYOUNSUL);
    end
    { 51 群体施毒术 中毒类型 - 绿毒 }
    else if AllowUseMagic(SKILL_GROUPAMYOUNSUL) and
      (MyGetTickCount - m_SkillUseTick[SKILL_GROUPAMYOUNSUL] >= GetMagicCD(SKILL_GROUPAMYOUNSUL)) and
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] = 0) and CheckAmulet(1, 1) and // CheckUserItem(Self, 1, 1) and
      ((AttackSideCount > 1) or NoSigleMagic) and AllowHeroMagicRate2(mtTaosAttack, SKILL_GROUPAMYOUNSUL, AttackSideCount + 1)
    { (Random(6) < AttackSideCount) } then
    begin
      // m_HeroAttackAmuPorc := m_HeroAmu11Porc;
      UserMagic := GetMagicInfoEx(SKILL_GROUPAMYOUNSUL);
    end
    { 6 施毒术 中毒类型 - 绿毒 }
    else if AllowUseMagic(SKILL_AMYOUNSUL) and (MyGetTickCount - m_SkillUseTick[SKILL_AMYOUNSUL] >= GetMagicCD(SKILL_AMYOUNSUL))
      and (TargeTBaseObject.m_WAbil.HP >= g_Config.dwHeroTaoUsePoisonMinHP) and
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] = 0) and (TargeTBaseObject.m_btRaceServer <> 55) and // 练功师中不了毒
      CheckAmulet(1, 1) and // CheckUserItem(Self, 1, 1) and
      (TargeTBaseObject.m_WAbil.NewValue[16] < 100) and (Random(SkillAmyounsulRate) = 0) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_AMYOUNSUL) then
    begin
      // m_HeroAttackAmuPorc := m_HeroAmu11Porc;
      UserMagic := GetMagicInfoEx(SKILL_AMYOUNSUL);
    end
    { 6 施毒术 中毒类型 - 红毒 }
    else if AllowUseMagic(SKILL_AMYOUNSUL) and (MyGetTickCount - m_SkillUseTick[SKILL_AMYOUNSUL] >= GetMagicCD(SKILL_AMYOUNSUL))
      and (TargeTBaseObject.m_WAbil.HP >= g_Config.dwHeroTaoUsePoisonMinHP) and
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0) and (TargeTBaseObject.m_btRaceServer <> 55) and // 练功师中不了毒
      CheckAmulet(2, 1) and // CheckUserItem(Self, 2, 1) and
      (TargeTBaseObject.m_WAbil.NewValue[16] < 100) and (Random(SkillAmyounsulRate) = 0) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_AMYOUNSUL) then
    begin
      // m_HeroAttackAmuPorc := m_HeroAmu21Porc;
      UserMagic := GetMagicInfoEx(SKILL_AMYOUNSUL);
    end
    { 57 噬血术 }
    else if AllowUseMagic(SKILL_57) and CheckUserItem(Self, 5, 1) and (Random(btBy) = 0) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_57) and (MyGetTickCount - m_SkillUseTick[SKILL_57] >= GetMagicCD(SKILL_57)) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_57);
    end
    { 202 裂神符 }
    else if AllowUseMagic(SKILL_202) and CheckUserItem(Self, 5, 1) and AllowHeroMagicRate(mtTaosAttack, SKILL_202)
    { (Random(5) = 0) } and (MyGetTickCount - m_SkillUseTick[SKILL_202] >= GetMagicCD(SKILL_202)) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_202);
    end
    { 203 死亡之眼 }
    else if AllowUseMagic(SKILL_203) and AllowHeroMagicRate(mtTaosAttack, SKILL_203) { (Random(5) = 0) } and
      (MyGetTickCount - m_SkillUseTick[SKILL_203] >= GetMagicCD(SKILL_203)) and ((AttackSideCount > 1) or NoSigleMagic) and
      AllowHeroMagicRate2(mtTaosAttack, SKILL_203, AttackSideCount + 1) { (Random(6) < AttackSideCount) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_203);
    end
    { 210 幽冥火符 }
    else if AllowUseMagic(SKILL_210) and CheckUserItem(Self, 5, 1) and AllowHeroMagicRate(mtTaosAttack, SKILL_210)
    { (Random(5) = 0) } and (MyGetTickCount - m_SkillUseTick[SKILL_210] >= GetMagicCD(SKILL_210)) and
      ((AttackSideCount > 1) or NoSigleMagic) and AllowHeroMagicRate2(mtTaosAttack, SKILL_210, AttackSideCount + 1)
    { (Random(6) < AttackSideCount) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_210);
    end
    { 52 英雄飓风破 }
    else if AllowUseMagic(SKILL_52) and CheckUserItem(Self, 5, 1) and AllowHeroMagicRate(mtTaosAttack, SKILL_52)
    { (Random(5) = 0) } and (MyGetTickCount - m_SkillUseTick[SKILL_52] >= GetMagicCD(SKILL_52)) and
      ((AttackSideCount > 1) or NoSigleMagic) and AllowHeroMagicRate2(mtTaosAttack, SKILL_52, AttackSideCount + 1)
    { (Random(6) < AttackSideCount) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_52);
    end
    { 13 灵魂火符 }
    else if AllowUseMagic(SKILL_FIRECHARM) and CheckUserItem(Self, 5, 1) and AllowHeroMagicRate(mtTaosAttack, SKILL_FIRECHARM)
    { (Random(5) = 0) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_FIRECHARM);
    end
    (*
      { 50 无极真气 }
      else if AllowUseMagic(SKILL_50) and (m_wStatusArrValue[2] = 0) and
      // 无极真气冷却时间 chongchong 2013-11-21
      (MyGetTickCount - m_SkillUseTick[SKILL_50] >= GetMagicCD(SKILL_50)) and
      (TempMagic <> nil) and
      (Random(10) = 0) then                                                                         // 无极真气使用频率降低 Random(btBy)
      begin
      UserMagic := TempMagic;
      //AbilityUp(TempMagic, 2);
      end
    *)
    { 48 气功波 }
    else if AllowUseMagic(SKILL_48) and (m_Abil.Level > TargeTBaseObject.m_Abil.Level) and (MySideCount > 2) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_48) { (Random(6) = 0) } then
    begin
      UserMagic := GetMagicInfoEx(SKILL_48);
    end
    (*
      { 18 隐身术 当锁定攻击目标时}
      else if m_boTarget and AllowUseMagic(SKILL_CLOAK) and
      (m_wStatusTimeArr[STATE_TRANSPARENT {0x70}] = 0) and
      CheckUserItem(Self, 5, 1) and (MySideCount > 0) and
      (Random(btBy) <= MySideCount) and   // 周边的怪越多时，使用隐身的几率越高 chongchong 2013-09-15
      (not TargeTBaseObject.m_boCoolEye) and
      (TargeTBaseObject.m_btRaceServer <> RC_PLAYOBJECT) then
      begin
      UserMagic := GetMagicInfoEx(SKILL_CLOAK);
      end
    *)
    { 18 隐身术 }
    else if AllowUseMagic(SKILL_CLOAK) and (m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] = 0) and CheckUserItem(Self, 5, 1) and
      (MySideCount > 3) and (Random(btBy) = 0) { (Random(btBy) <= MySideCount) } and
    // 周边的怪越多时，使用隐身的几率越高 chongchong 2013-09-15
      (not TargeTBaseObject.m_boCoolEye) and (TargeTBaseObject.m_btRaceServer <> RC_PLAYOBJECT) and
      AllowHeroMagicRate(mtTaosAttack, SKILL_CLOAK) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_CLOAK);
    end;

    (* 暂时不用
      { 38 诅咒术 }
      else if AllowUseMagic(SKILL_38) and
      (TargeTBaseObject.m_wStatusTimeArr[0] = 0) and
      (TargeTBaseObject.m_wStatusTimeArr[1] = 0) and
      (TargeTBaseObject.m_wStatusTimeArr[2] = 0) and
      CheckUserItem(Self, 5, 1) then
      begin
      UserMagic := GetMagicInfoEx(SKILL_38);
      end
    *)

    if UserMagic = nil then
    begin
      for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
      begin
        HeroMagic := g_CustomHeroMagicMgr.Items[I];
        if (HeroMagic.MagicType = mtTaosAttack) and HeroMagic.IsCustomMagic then
        begin
          CustomUserMagic := m_CustomSkill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID];
          if (CustomUserMagic <> nil) and (CustomUserMagic.btKey > 0) and AllowHeroMagicRate(mtTaosAttack, HeroMagic.MagicID) then
          begin
            if (HeroMagic.AttackTarget = matEnemy) then
            begin
              // 判断执行条件 chongchong 2018-01-06
              if CheckHeroMagicUseCondition(@HeroMagic.Condition, TargeTBaseObject) then
              begin
                CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                begin
                  if AllowCustomSkill(HeroMagic.MagicID, True) then
                  begin
                    UserMagic := CustomUserMagic;
                    Break;
                  end;
                end;
              end;
            end;
          end;
        end
      end;
    end;
  end;

  procedure ProtectSlave(btBy: Byte);
  var
    SlaveObject: TBaseObject;
    // I          :Integer;
  begin
    if UserMagic <> nil then
      Exit;
    // SlaveObject:=Nil;
    if (Random(2) = 0) or (m_Master.m_SlaveList.Count = 0) then
    begin
      if m_SlaveList.Count <= 0 then
        Exit;

      SlaveObject := m_SlaveList.Items[Random(m_SlaveList.Count)];
      if (SlaveObject <> nil) and (abs(m_nCurrX - SlaveObject.m_nCurrX) < g_Config.nMagicAttackRage) and
        (abs(m_nCurrY - SlaveObject.m_nCurrY) < g_Config.nMagicAttackRage) then
      begin
        TargeTBaseObject := SlaveObject;
        TargeX := SlaveObject.m_nCurrX;
        TargeY := SlaveObject.m_nCurrY;
        ProtectFirend(btBy);
      end;
    end
    else
    begin
      if m_Master.m_SlaveList.Count <= 0 then
        Exit;

      SlaveObject := m_Master.m_SlaveList.Items[Random(m_Master.m_SlaveList.Count)];

      if (SlaveObject <> nil) and (abs(m_nCurrX - SlaveObject.m_nCurrX) < g_Config.nMagicAttackRage) and
        (abs(m_nCurrY - SlaveObject.m_nCurrY) < g_Config.nMagicAttackRage) then
      begin
        TargeTBaseObject := SlaveObject;
        TargeX := SlaveObject.m_nCurrX;
        TargeY := SlaveObject.m_nCurrY;
        ProtectFirend(0);
      end;
    end;
  end;

var
  TempX, TempY, TempAttackSideCount, MaxAttackSideCount: Integer;
  DirArray: array [DR_UP .. DR_UPLEFT] of Integer;
  I, Index1, Index2, AddHPRandom: Integer;
  IsCanAttack: Boolean;
  nWalkTime: Integer;
begin
  UserMagic := nil;
  Result := False;
  {
    Result := OpenSuperShiled;
    if Result then Exit;
  }
  try
    if m_TargetCret <> nil then
    begin
      { TODO -ochongchong -c新增 : 当英雄包裹内没有药品或者毒符的时候提示 【2013-08-15】 }
      if (MyGetTickCount - m_HeroAmuHintTick) > 3000 then
      begin
        if (not CheckAmulet(1, 1)) and (AllowUseMagic(SKILL_GROUPAMYOUNSUL) or AllowUseMagic(SKILL_AMYOUNSUL)) and
          (Length(sNeedGreenPoison) > 0) then
        begin
          // m_Master.SendScreenMsg(g_Config.sHeroSayPrefix + '灰色药粉已经用完！', 250, 0, 14, 350);
          m_Master.SendMsg(m_Master, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, g_Config.sHeroSayPrefix + sNeedGreenPoison);
        end;

        if (not CheckAmulet(2, 1)) and (AllowUseMagic(SKILL_GROUPAMYOUNSUL) or AllowUseMagic(SKILL_AMYOUNSUL)) and
          (Length(sNeedRedPoison) > 0) then
        begin
          // m_Master.SendScreenMsg(g_Config.sHeroSayPrefix + '黄色药粉已经用完！', 250, 0, 14, 350);                 // c_Blue, t_Hint);
          m_Master.SendMsg(m_Master, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, g_Config.sHeroSayPrefix + sNeedRedPoison);
        end;

        if (not CheckUserItem(Self, 5, 1)) and (AllowUseMagic(SKILL_57) or AllowUseMagic(SKILL_FIRECHARM) or
          AllowUseMagic(SKILL_CLOAK) or AllowUseMagic(SKILL_BIGCLOAK)) and (Length(sNeedFu) > 0) then
        begin
          // m_Master.SendScreenMsg(g_Config.sHeroSayPrefix + '护身符已经用完！', 250, 0, 14, 350);                   // c_Blue, t_Hint);
          m_Master.SendMsg(m_Master, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, g_Config.sHeroSayPrefix + sNeedFu);
        end;

        m_HeroAmuHintTick := MyGetTickCount;
      end;

      // 盛大的道士英雄身边大于3个怪才隐身，而且英雄隐身不会停止移动，一般隐身1-2两秒就开始移动，主要是躲避怪物为主 chongchong 2013-11-12
      if m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] > 4 then
      begin
        m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] := 4;
      end;

      // 道士英雄无技能时或所有技能关闭时，物理攻击不躲避 chongchong 2017-06-19
      NoMagicAttact := False;
      if { g_Config.boHeroNoSkillUseBaseAttack and } (m_btAttackMode in [0, 3]) then
      begin
        NoMagicAttact := m_MagicList.Count = 0;

        if not NoMagicAttact then
        begin
          NoMagicAttact := True;

          for Index := 0 to m_MagicList.Count - 1 do
          begin
            TestMagic := m_MagicList[Index];
            if (TestMagic <> nil) and (TestMagic.btKey > 0) and
              (not(TestMagic.wMagIdx in [SKILL_HEALLING { 治愈术 } , SKILL_UNAMYOUNSUL { 解毒术 } , SKILL_ILKWANG { 精神力战法 } ])) then
            begin
              NoMagicAttact := False;
              Break;
            end;
          end;
        end;
      end;

      case m_btJob of
        0:
          nWalkTime := g_Config.dwHeroWarrorWalkTime;
        1:
          nWalkTime := g_Config.dwHeroAvoidTime; // g_Config.dwHeroWizardWalkTime;
        2:
          nWalkTime := g_Config.dwHeroAvoidTime; // g_Config.dwHeroTaoistWalkTime;
      else
        nWalkTime := 500;
      end;

      { 如果是锁定目标，并且自己周围的怪比较多，找一个好位置 (因为锁定的目标可能离自己较远) chongchong 2013-09-15 }
      // 锁定并物理攻击的时候，硬扛着，不要晃来晃去 chongchong 2017-06-20
      if m_boTarget and (not(NoMagicAttact or (g_Config.boWarrorAttack and (m_WAbil.MP <= 15)))) then
      begin
        MySideCount := GetMapBaseObjectCount(m_PEnvir, m_nCurrX, m_nCurrY, 1);

        if not g_Config.boHeroNotAvoidLastHinter then
        begin
          // 如果自己周边的怪比较多
          if (MySideCount >= 3) and (MyGetTickCount - m_dwMoveTimeTick >= 1000) then
          begin
            { 遍历怪物周边八个方向的可攻击点 }
            { 先把方向打乱 }
            for n14 := DR_UP to DR_UPLEFT do
              DirArray[n14] := n14;

            for n14 := 0 to 10 do
            begin
              Index1 := Random(DR_UPLEFT + 1);
              Index2 := Random(DR_UPLEFT + 1);
              if Index1 <> Index2 then
              begin
                TempX := DirArray[Index1];
                DirArray[Index1] := DirArray[Index2];
                DirArray[Index2] := TempX;
              end;
            end;

            MaxAttackSideCount := 99999;
            for n14 := DR_UP to DR_UPLEFT do
            begin
              m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, DirArray[n14], g_Config.nMagicAttackRage - 1,
                TempX, TempY);

              TempAttackSideCount := GetMapBaseObjectCount(m_TargetCret.m_PEnvir, TempX, TempY, 2);
              if TempAttackSideCount < MaxAttackSideCount then
              begin
                MaxAttackSideCount := TempAttackSideCount;
                m_nTargetX := TempX;
                m_nTargetY := TempY;
              end;
            end;
            SetTargetXY(m_nTargetX, m_nTargetY);
            HeroRuntoTargetXY;
            Result := True;
            Exit;
          end;
        end;
      end;

      // 守护模式，当主人身边的怪较多时，打集体隐身 chongchong 2013-09-15
      if m_boProtectStatus and (abs(m_nCurrX - m_Master.m_nCurrX) < g_Config.nMagicAttackRage) and // 主人在英雄的魔法范围内
        (abs(m_nCurrY - m_Master.m_nCurrY) < g_Config.nMagicAttackRage) and
        (m_Master.m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] = 0) and
        (MyGetTickCount - m_dwLastAttackTick > GetAttackIntervalTime(False)) and (Random(8) = 0) then
      begin
        // 主人身边的怪有三个以上
        if (GetMapBaseObjectCount(m_PEnvir, m_Master.m_nCurrX, m_Master.m_nCurrY, 1) > 3) and AllowUseMagic(SKILL_BIGCLOAK) and
          CheckUserItem(Self, 5, 1) then
        begin
          UserMagic := GetMagicInfoEx(SKILL_BIGCLOAK);

          if UserMagic <> nil then
          begin
            DoSpell(UserMagic, m_Master.m_nCurrX, m_Master.m_nCurrY, m_Master);;
            m_dwLastAttackTick := MyGetTickCount;
            Exit;
          end;
        end;
      end;

      if NoMagicAttact or (g_Config.boWarrorAttack and (m_WAbil.MP <= 15)) then
      begin
        // 低血逃跑
        if (m_WAbil.HP <= TPlayObject(m_Master).m_nHeroDodgeHPPercent) then
        begin
          if HeroAvoidTarget(m_TargetCret) then
          begin
            Result := True;
            m_boLastAvoidTarget := True;
          end;
        end
        else
        begin
          Result := True;

          if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 2) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 2) then
            HeroBasicAttackTarget
          else
          begin
            SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
            if Random(5) = 0 then
              HeroGotoTargetXY
            else
              HeroRuntoTargetXY;
          end;

          Exit;
        end;
      end
      // MB的，又要参考LEG，躲避目标连续2次，围绕目标 chongchong 2018-07-20 22:55:28
      else if m_boAvoidTargetFirst and (m_dwLastAttackTick >= m_dwMoveTimeTick) then
      begin
        if (MyGetTickCount - m_dwMoveTimeTick) > nWalkTime then
        begin
          HeroAvoidTargetNext;
          m_boAvoidTargetFirst := False;
          Result := True;
          Exit;
        end
        else
        begin
          Result := True;
          Exit;
        end;
      end
      else if ((m_TargetCret <> nil) or (m_LastHiter <> nil)) and
        (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) and
        (not FTaosUseBaseAttack) and g_Config.boHeroDFAvoidTargetRight and m_boAvoidTargetFirst and HeroAvoidTargetNext() then
      begin
        Result := True;
        m_boLastAvoidTarget := True;
        Exit;
      end
      else if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < 3) and (abs(m_nCurrY - m_TargetCret.m_nCurrY) < 3) and
        (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) and
        (not FTaosUseBaseAttack) then
      begin
        if HeroAvoidTarget(m_TargetCret) then
        begin
          Result := True;
          m_boLastAvoidTarget := True;
          Exit;
        end;
      end
      // 躲避正在攻击自己的对象 chongchong 2015-05-05
      else if (m_LastHiter <> nil) and ((m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) or
        (not g_Config.boHeroNotAvoidLastHinter)) and (abs(m_nCurrX - m_LastHiter.m_nCurrX) < 3) and
        (abs(m_nCurrY - m_LastHiter.m_nCurrY) < 3) and (m_TargetCret <> nil
        { Random(2) = 0 无目标是跟随主人，这个容易导致跟随主人有问题 2017-12-14 } ) and
        (not(m_boTarget and m_boTargetAgain and g_Config.boHeroTargetAgainNoMove and g_Config.boHeroTargetAgainNoMoveDF)) then
      begin
        { 没有魔法可用，并且允许无魔法时使用物理攻击 }
        if (g_Config.boWarrorAttack and (m_WAbil.MP <= 15)) or
          (g_Config.boHero700HPUseBaseAttack and (m_TargetCret.m_WAbil.MaxHP < g_Config.dwHero700HPValue)) then
        begin
          FTaosUseBaseAttack := True;
          if GetAttackDir(m_TargetCret, btDir) then
          begin
            if ((MyGetTickCount - m_dwLastAttackTick) > GetAttackIntervalTime(True)) and
            { add 2018-07-10 11:09:20 } (MyGetTickCount - m_dwStationTick >= GetAttackIntervalTime(False))
            { 跑步前去攻击时，前三刀过快 chongchong 2018-07-10 11:09:09 } then
            begin
              AttackDir(nil, 0, btDir);
              m_dwLastAttackTick := MyGetTickCount;
            end;
          end
          else
          begin
            { 使用物理攻击时，对象不在攻击范围内，跑过去打 }
            SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
            HeroGotoTargetXY;
            Exit;
          end;
        end
        else if (not FTaosUseBaseAttack) then
        begin
          if HeroAvoidTarget(m_LastHiter) then
          begin
            Result := True;
            m_boLastAvoidTarget := True;
            Exit;
          end;
        end;
      end;

      // 不追目标，出刀在移动后一段时间，不然跑步前去攻击时，前三刀过快
      {
        if m_boLastAvoidTarget then
        IsCanAttack := True
        else
        IsCanAttack := (MyGetTickCount - m_dwStationTick >= GetAttackIntervalTime(False));
      }
      // NND，道法英雄在追击的时候，又说攻击速度太慢 fuck 2018-07-10 11:06:47
      IsCanAttack := True;

      if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < g_Config.nMagicAttackRage) and
        (abs(m_nCurrY - m_TargetCret.m_nCurrY) < g_Config.nMagicAttackRage) then
      begin
        if ((MyGetTickCount - m_dwLastAttackTick) > GetAttackIntervalTime(False)) and IsCanAttack then
        begin
          // RefTaosCorpsItem;                                 //符毒等
          if (m_WAbil.MP > 15) then
          begin
            if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 10) then
              ProtectSelf(10)
            else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 20) then
              ProtectSelf(20)
            else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 30) then
              ProtectSelf(30)
            else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 40) then
              ProtectSelf(40)
            else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 85) then
              ProtectSelf(50)
            else
              ProtectSelf(60);
          end;

          { 如果不是低血量，则保护主人 }
          if (m_WAbil.HP > TPlayObject(m_Master).m_nHeroDodgeHPPercent) then
          begin
            MySideCount := GetMapBaseObjectCount(m_PEnvir, m_nCurrX, m_nCurrY, 2);
            AttackSideCount := GetMapBaseObjectCount(m_TargetCret.m_PEnvir, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 1);

            // AttackMonster(3);
            AttackMonster(2);
            if { (UserMagic = nil) and } (m_Master <> nil) and (abs(m_nCurrX - m_Master.m_nCurrX) < g_Config.nMagicAttackRage) and
              (abs(m_nCurrY - m_Master.m_nCurrY) < g_Config.nMagicAttackRage) and (m_WAbil.MP > 15) then
            begin
              // 没有魔法的时候，保护主人和宝宝
              if (UserMagic = nil) then
              begin
                Result := True;
                ProtectHuman(30);
                ProtectSlave(30);
              end
              // 有一定的几率给主人加血
              else if not((m_boTarget) and (m_TargetCret <> nil) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boDeath)
                and (m_TargetCret.m_PEnvir = m_PEnvir)) then
              begin
                // 主人的血量越低，给主人加血的几率越高 chongchong 2013-09-15
                case Round(m_Master.m_WAbil.HP / m_Master.m_WAbil.MaxHP * 10) of
                  0, 1:
                    AddHPRandom := 3;
                  2:
                    AddHPRandom := 2;
                  {
                    0, 1: AddHPRandom := 9;
                    2: AddHPRandom := 8;
                    3: AddHPRandom := 7;
                    4: AddHPRandom := 4;
                    5: AddHPRandom := 3;
                    6: AddHPRandom := 2;
                    7: AddHPRandom := 1;
                    8: AddHPRandom := 0;
                  }
                else
                  AddHPRandom := -1;
                end;
                {
                  // 也不是每次都要鸟主人，所以用10
                  if Random(10) <= AddHPRandom then
                }
                if Random(6) <= AddHPRandom then
                begin
                  if AllowUseMagic(SKILL_HEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_HEALLING] >= SKILL_HEALLING_CD_MIN)
                  then
                  begin
                    UserMagic := GetMagicInfoEx(SKILL_HEALLING);

                    if UserMagic <> nil then
                    begin
                      FTaosUseBaseAttack := False;

                      Result := True;
                      DoSpell(UserMagic, m_Master.m_nCurrX, m_Master.m_nCurrY, m_Master);

                      // HZQ 消除永真条件
                      if { (UserMagic.wMagIdx >= Low(m_SkillUseTick)) and } (UserMagic.wMagIdx <= High(m_SkillUseTick)) then
                      begin
                        m_SkillUseTick[UserMagic.wMagIdx] := MyGetTickCount;
                      end;

                      m_dwLastAttackTick := MyGetTickCount;
                      // boMove := True;
                      // UserMagic := nil;
                    end;
                  end;

                  (*
                    // 主人血量低于 30% 再来一个集体隐身
                    else if AddHPRandom >= 7 then
                    begin
                    if (m_Master.m_wStatusTimeArr[STATE_TRANSPARENT {0x70}] = 0) and
                    AllowUseMagic(SKILL_BIGCLOAK) and
                    (MyGetTickCount - m_SkillUseTick[SKILL_BIGCLOAK] > GetAttackIntervalTime) and
                    CheckUserItem(Self, 5, 1) then
                    begin
                    UserMagic := GetMagicInfoEx(SKILL_BIGCLOAK);

                    if UserMagic <> nil then
                    begin
                    Result := True;
                    DoSpell(UserMagic, m_Master.m_nCurrX, m_Master.m_nCurrY, m_Master);
                    if (UserMagic.wMagIdx >= Low(m_SkillUseTick)) and (UserMagic.wMagIdx <= High(m_SkillUseTick)) then
                    begin
                    m_SkillUseTick[UserMagic.wMagIdx] := MyGetTickCount;
                    end;
                    m_dwLastAttackTick := MyGetTickCount;
                    boMove := True;
                    UserMagic := nil;
                    end;
                    end;
                    end;
                  *)
                end;
              end;
            end;
          end;

          // 最后再看下，是否有可攻击的技能 chongchong 2013-12-27
          if UserMagic = nil then
          begin
            { 57 噬血术 }
            if AllowUseMagic(SKILL_57) and CheckUserItem(Self, 5, 1) and
              (MyGetTickCount - m_SkillUseTick[SKILL_57] >= GetMagicCD(SKILL_57)) and AllowHeroMagicRate(mtTaosAttack, SKILL_57)
            then
            begin
              TargeTBaseObject := m_TargetCret;
              TargeX := m_TargetCret.m_nCurrX;
              TargeY := m_TargetCret.m_nCurrY;
              UserMagic := GetMagicInfoEx(SKILL_57);
            end
            { 13 灵魂火符 }
            else if AllowUseMagic(SKILL_FIRECHARM) and CheckUserItem(Self, 5, 1) and
              AllowHeroMagicRate(mtTaosAttack, SKILL_FIRECHARM) then
            begin
              TargeTBaseObject := m_TargetCret;
              TargeX := m_TargetCret.m_nCurrX;
              TargeY := m_TargetCret.m_nCurrY;
              UserMagic := GetMagicInfoEx(SKILL_FIRECHARM);
            end
            else
            begin
              for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
              begin
                HeroMagic := g_CustomHeroMagicMgr.Items[I];
                if (HeroMagic.MagicType = mtTaosAttack) and HeroMagic.IsCustomMagic then
                begin
                  CustomUserMagic := m_CustomSkill[HeroMagic.MagicID - CUSTOM_MAGIC_START_ID];
                  if (CustomUserMagic <> nil) and (CustomUserMagic.btKey > 0) and
                    AllowHeroMagicRate(mtTaosAttack, HeroMagic.MagicID) then
                  begin
                    if (HeroMagic.AttackTarget = matEnemy) then
                    begin
                      // 判断执行条件 chongchong 2018-01-06
                      if CheckHeroMagicUseCondition(@HeroMagic.Condition, m_TargetCret) then
                      begin
                        CustomMagicConfig := GetCustomMagicConfig(HeroMagic.MagicID);
                        if (CustomMagicConfig <> nil) and (not CustomMagicConfig.IsMagicWarr) then
                        begin
                          if AllowCustomSkill(HeroMagic.MagicID, True) then
                          begin
                            TargeTBaseObject := m_TargetCret;
                            TargeX := m_TargetCret.m_nCurrX;
                            TargeY := m_TargetCret.m_nCurrY;

                            UserMagic := CustomUserMagic;
                            Break;
                          end;
                        end;
                      end;
                    end;
                  end;
                end
              end;
            end;
          end;

          if UserMagic <> nil then
          begin
            FTaosUseBaseAttack := False;

            if UserMagic.wMagIdx = SKILL_75 then
            begin
              OpenSuperShiled;
              m_dwLastAttackTick := MyGetTickCount;
              m_dwMoveTimeTick := MyGetTickCount;
            end // 魔法攻击时判断是否能打中目标，如果打不中，就跑一下 chongchong 2013-12-30
            else if MagCanHitTarget(m_nCurrX, m_nCurrY, m_TargetCret) or (UserMagic.wMagIdx = SKILL_50) then
            begin
              // S := '使用技能' + UserMagic.MagicInfo.sMagicName;
              // OutputDebugString(PChar(S));

              DoSpell(UserMagic, TargeX, TargeY, TargeTBaseObject);
              m_dwLastAttackTick := MyGetTickCount;
              m_dwMoveTimeTick := MyGetTickCount;
            end
            else
            begin
              SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
              HeroGotoTargetXY;
              Exit;
            end;
          end
          else
          begin
            // 修正勾选道士无魔法时物理攻击，有魔法时跑来跑去 +NoMagicAttact 2020-10-21
            { 没有魔法可用，并且允许无魔法时使用物理攻击 }
            if (g_Config.boWarrorAttack and NoMagicAttact) or
              (g_Config.boHero700HPUseBaseAttack and (m_TargetCret.m_Abil.MaxHP < g_Config.dwHero700HPValue)) then
            begin
              FTaosUseBaseAttack := True;
              if GetAttackDir(m_TargetCret, btDir) and (MyGetTickCount - m_dwLastAttackTick > GetAttackIntervalTime(True)) and
              { add 2018-07-10 11:09:20 } (MyGetTickCount - m_dwStationTick >= GetAttackIntervalTime(False))
              { 跑步前去攻击时，前三刀过快 chongchong 2018-07-10 11:09:09 } then
              begin
                AttackDir(nil, 0, btDir);
                m_dwLastAttackTick := MyGetTickCount;
              end
              else if (abs(m_nCurrX - m_TargetCret.m_nCurrX) > 1) or (abs(m_nCurrY - m_TargetCret.m_nCurrY) > 1) then
              // 跑近了就不跑了 2020-10-18 00:08:39
              begin
                m_boAvoidTargetFirst := False; // 跑过去后不躲 2020-10-18 00:08:51
                { 使用物理攻击时，对象不在攻击范围内，跑过去打 }
                SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);

                if (abs(m_nCurrX - m_TargetCret.m_nCurrX) > 2) or (abs(m_nCurrY - m_TargetCret.m_nCurrY) > 2) then
                  HeroRuntoTargetXY
                else
                  HeroGotoTargetXY;
                Exit;
              end;
            end
            else
            begin
              FTaosUseBaseAttack := False;
            end;
          end;
        end;

        Result := True;
      end
      /// else 如果目标不在攻击范围，并且锁定对象，先隐身，再往目标那边跑 chongchong 2013-09-15
      else if { m_boTarget and } (m_TargetCret <> nil) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boDeath) and
        (MyGetTickCount - m_dwLastAttackTick > GetAttackIntervalTime(False)) then
      begin
        (*
          { 18 隐身术 }
          if AllowUseMagic(SKILL_CLOAK) and
          (m_wStatusTimeArr[STATE_TRANSPARENT {0x70}] = 0) and
          CheckUserItem(Self, 5, 1) and
          (not m_TargetCret.m_boCoolEye) and
          (m_TargetCret.m_btRaceServer <> RC_PLAYOBJECT) then
          begin
          UserMagic := GetMagicInfoEx(SKILL_CLOAK);

          if UserMagic <> nil then
          begin
          DoSpell(UserMagic, TargeX, TargeY, m_TargetCret); ;
          m_dwLastAttackTick := MyGetTickCount;
          end;
          end;
        *)
        SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        HeroRuntoTargetXY;
        Result := True;
      end;

      // if (abs(m_nCurrX - m_TargetCret.m_nCurrX) < g_Config.nMagicAttackRage) and
      if m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] <> 0 then
        Result := True;
    end
    { 当无攻击目标时，给主人和宝宝回血 }
    else if (m_Master <> nil) and ((abs(m_nCurrX - m_Master.m_nCurrX) < 3) or (abs(m_nCurrY - m_Master.m_nCurrY) < 3)) and
      (m_Master.m_PEnvir = m_PEnvir) then
    begin
      if (MyGetTickCount - m_dwLastAttackTick) > GetAttackIntervalTime(False) then
      begin
        if m_WAbil.MP > 15 then
        begin
          // RefTaosCorpsItem;
          ProtectHuman(3);
          ProtectSelf(3);
          if (m_SlaveList.Count > 0) or (m_Master.m_SlaveList.Count > 0) then
            ProtectSlave(3);
          if UserMagic <> nil then
          begin
            Result := True;
            FTaosUseBaseAttack := False;
            if UserMagic.wMagIdx = SKILL_75 then
              OpenSuperShiled
            else
              DoSpell(UserMagic, TargeX, TargeY, TargeTBaseObject);

            // HZQ 消除永真条件
            if { (UserMagic.wMagIdx >= Low(m_SkillUseTick)) and } (UserMagic.wMagIdx <= High(m_SkillUseTick)) then
              m_SkillUseTick[UserMagic.wMagIdx] := MyGetTickCount
            else if CheckIsCustomMagic(UserMagic.wMagIdx) then
              m_CustomSkillUseTick[UserMagic.wMagIdx - CUSTOM_MAGIC_START_ID] := MyGetTickCount;

            m_dwLastAttackTick := MyGetTickCount;
          end;
        end;
      end;
    end;
  except
    MainOutMessage('[Exception] TPlayObject:HeroTaosAttackTarget');
  end;
end;

function THeroObject.HeroTaosProtectSelfAndFirends: Boolean;
var
  UserMagic: pTUserMagic;
  TargeTBaseObject: TBaseObject;
  TargeX, TargeY: Integer;

  procedure ProtectFirend(btBy: Byte);
  var
    I, Count: Integer;
    BaseObjectList: TList;
    BaseObject: TBaseObject;
  begin
    if (TargeTBaseObject = nil) or (UserMagic <> nil) then
      Exit;

    // 1114英雄细节需求：只在攻击模式，并且英雄有了攻击目标，才会打魔防;
    { 15 神圣战甲术 防 }
    if (m_TargetCret <> nil) and (m_btAttackMode in [0, 3]) and AllowUseMagic(SKILL_DEJIWONHO) and
      (TargeTBaseObject = Self.m_Master) and (TargeTBaseObject.m_wStatusTimeArr[STATE_DEFENCEUP] = 0) and
    // (MyGetTickCount - m_SkillUseTick[SKILL_DEJIWONHO] > 1000) and
      CheckUserItem(Self, 5, 1) and AllowHeroMagicRate2(mtTaosAttack, SKILL_DEJIWONHO, High(Integer)) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_DEJIWONHO);
    end
    { 14 幽灵盾 魔 }
    else if (m_TargetCret <> nil) and (m_btAttackMode in [0, 3]) and AllowUseMagic(SKILL_HANGMAJINBUB) and
      (TargeTBaseObject.m_wStatusTimeArr[STATE_MAGDEFENCEUP] = 0) and (TargeTBaseObject = Self.m_Master) and
    // (MyGetTickCount - m_SkillUseTick[SKILL_HANGMAJINBUB] > 1000) and
      CheckUserItem(Self, 5, 1) and AllowHeroMagicRate2(mtTaosAttack, SKILL_HANGMAJINBUB, High(Integer)) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_HANGMAJINBUB);
    end
    { 34 解毒术 }
    else if AllowUseMagic(SKILL_UNAMYOUNSUL) and ((TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <> 0) or
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <> 0) or (TargeTBaseObject.m_wStatusTimeArr[POISON_STONE] <> 0)) and
      (Random(btBy) = 0) and AllowHeroMagicRate2(mtTaosAttack, SKILL_UNAMYOUNSUL, High(Integer)) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_UNAMYOUNSUL);
    end
    { 49 净化术 }
    else if AllowUseMagic(SKILL_49) and ((TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <> 0) or
      (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <> 0) or (TargeTBaseObject.m_wStatusTimeArr[POISON_STONE] <> 0)) and
      (Random(btBy) = 0) and AllowHeroMagicRate(mtTaosAttack, SKILL_49) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_49);
    end

    { 2 治愈术 }
    else if ((AllowUseMagic(SKILL_HEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_HEALLING] >= SKILL_HEALLING_CD_MIN)) or
      (AllowUseMagic(SKILL_BIGHEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_BIGHEALLING] >= SKILL_HEALLING_CD_MIN))) and
      (TargeTBaseObject.m_WAbil.HP < Round(TargeTBaseObject.m_WAbil.MaxHP / 100 * 90)) and (Random(btBy) = 0) and
      AllowHeroMagicRate2(mtTaosAttack, SKILL_HEALLING, High(Integer)) then
    begin
      if AllowUseMagic(SKILL_HEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_HEALLING] >= SKILL_HEALLING_CD_MIN) then
        UserMagic := GetMagicInfoEx(SKILL_HEALLING);

      if not(AllowUseMagic(SKILL_BIGHEALLING) and (MyGetTickCount - m_SkillUseTick[SKILL_BIGHEALLING] >= SKILL_HEALLING_CD_MIN))
      then
        Exit;

      Count := 0;
      BaseObjectList := TList.Create;
      try
        TargeTBaseObject.GetMapBaseObjects(TargeTBaseObject.m_PEnvir, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY, 2,
          BaseObjectList);
        for I := 0 to BaseObjectList.Count - 1 do
        begin
          BaseObject := TBaseObject(BaseObjectList[I]);
          if TargeTBaseObject.IsProperFriend(BaseObject) then
          begin
            if BaseObject.m_WAbil.HP <= Round(BaseObject.m_WAbil.MaxHP / 100 * 90) then
            begin
              Inc(Count);
              if Count >= 2 then
              begin
                UserMagic := GetMagicInfoEx(SKILL_BIGHEALLING);
                Break;
              end;
            end;
          end;
        end;
      finally
        BaseObjectList.Free;
      end;
    end
    { 75 护体神盾 }
    else if (TargeTBaseObject = Self) and (not m_boSuperShiled) and AllowUseMagic(SKILL_75) and
      (not g_Config.boHeroAutoSuperShiled) and ((MyGetTickCount - m_dwLastSuperShiledTimeTick) > GetMagicCD(SKILL_75)) and
      (Random(3) = 0) and AllowHeroMagicRate2(mtTaosAttack, SKILL_75, High(Integer)) then
    begin
      UserMagic := GetMagicInfoEx(SKILL_75);
    end
    // 休息或跟随状态打隐身 chongchong 2017-12-08
    else if (TargeTBaseObject = Self) and
      ((AllowUseMagic(SKILL_CLOAK) and AllowHeroMagicRate2(mtTaosAttack, SKILL_CLOAK, High(Integer))) or
      (AllowUseMagic(SKILL_BIGCLOAK) and AllowHeroMagicRate2(mtTaosAttack, SKILL_BIGCLOAK, High(Integer)))) and
      (m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] = 0) and CheckUserItem(Self, 5, 1) and
      (MyGetTickCount - m_SkillUseTick[SKILL_CLOAK] >= 10000) and (MyGetTickCount - m_SkillUseTick[SKILL_BIGCLOAK] >= 10000) and
      (Random(3) = 0) then
    begin
      if AllowUseMagic(SKILL_CLOAK) and AllowHeroMagicRate2(mtTaosAttack, SKILL_CLOAK, High(Integer)) then
        UserMagic := GetMagicInfoEx(SKILL_CLOAK);

      if not AllowUseMagic(SKILL_BIGCLOAK) and AllowHeroMagicRate2(mtTaosAttack, SKILL_BIGCLOAK, High(Integer)) then
        Exit;
      Count := 0;
      BaseObjectList := TList.Create;
      try
        TargeTBaseObject.GetMapBaseObjects(TargeTBaseObject.m_PEnvir, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY, 2,
          BaseObjectList);
        for I := 0 to BaseObjectList.Count - 1 do
        begin
          BaseObject := TBaseObject(BaseObjectList[I]);
          if TargeTBaseObject.IsProperFriend(BaseObject) then
          begin
            Inc(Count);
            if Count >= 2 then
            begin
              UserMagic := GetMagicInfoEx(SKILL_BIGCLOAK);
              Break;
            end;
          end;
        end;
      finally
        BaseObjectList.Free;
      end;
    end
  end;

  procedure ProtectHuman(btBy: Byte);
  begin
    if (m_Master = nil) or (UserMagic <> nil) then
      Exit;
    TargeTBaseObject := m_Master;
    TargeX := m_Master.m_nCurrX;
    TargeY := m_Master.m_nCurrY;
    ProtectFirend(btBy);
  end;

  procedure ProtectSelf(btBy: Byte);
  begin
    if UserMagic <> nil then
      Exit;
    TargeTBaseObject := Self;
    TargeX := m_nCurrX;
    TargeY := m_nCurrY;
    ProtectFirend(btBy);
  end;

  procedure ProtectSlave(btBy: Byte);
  var
    SlaveObject: TBaseObject;
  begin
    if UserMagic <> nil then
      Exit;
    if (Random(2) = 0) or (m_Master.m_SlaveList.Count = 0) then
    begin
      if m_SlaveList.Count <= 0 then
        Exit;

      SlaveObject := m_SlaveList.Items[Random(m_SlaveList.Count)];
      if (SlaveObject <> nil) and (abs(m_nCurrX - SlaveObject.m_nCurrX) < g_Config.nMagicAttackRage) and
        (abs(m_nCurrY - SlaveObject.m_nCurrY) < g_Config.nMagicAttackRage) then
      begin
        TargeTBaseObject := SlaveObject;
        TargeX := SlaveObject.m_nCurrX;
        TargeY := SlaveObject.m_nCurrY;
        ProtectFirend(btBy);
      end;
    end
    else
    begin
      if m_Master.m_SlaveList.Count <= 0 then
        Exit;

      SlaveObject := m_Master.m_SlaveList.Items[Random(m_Master.m_SlaveList.Count)];

      if (SlaveObject <> nil) and (abs(m_nCurrX - SlaveObject.m_nCurrX) < g_Config.nMagicAttackRage) and
        (abs(m_nCurrY - SlaveObject.m_nCurrY) < g_Config.nMagicAttackRage) then
      begin
        TargeTBaseObject := SlaveObject;
        TargeX := SlaveObject.m_nCurrX;
        TargeY := SlaveObject.m_nCurrY;
        ProtectFirend(0);
      end;
    end;
  end;

begin
  UserMagic := nil;
  Result := False;

  if (MyGetTickCount - m_dwLastAttackTick) < GetAttackIntervalTime(False) then
  begin
    Exit;
  end;

  if m_WAbil.MP <= 15 then
  begin
    Exit;
  end;

  if (m_Master <> nil) and (abs(m_nCurrX - m_Master.m_nCurrX) < g_Config.nMagicAttackRage) and
    (abs(m_nCurrY - m_Master.m_nCurrY) < g_Config.nMagicAttackRage) and (m_Master.m_PEnvir = m_PEnvir) and
    (m_Master.m_WAbil.HP <= Round(m_Master.m_WAbil.MaxHP / 100 * 10)) then
  begin
    ProtectHuman(1);
  end
  else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 10) then
    ProtectSelf(1)
  else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 20) then
    ProtectSelf(4)
  else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 30) then
    ProtectSelf(7)
  else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 40) then
    ProtectSelf(15)
  else if m_WAbil.HP <= Round(m_WAbil.MaxHP / 100 * 85) then
    ProtectSelf(20)
  else
    ProtectSelf(30);

  if m_Master.m_WAbil.HP < m_Master.m_WAbil.MaxHP then
    ProtectHuman(1)
  else if m_WAbil.HP < m_WAbil.MaxHP then
    ProtectSelf(1)
  else if (m_SlaveList.Count > 0) or (m_Master.m_SlaveList.Count > 0) then
    ProtectSlave(1);

  if UserMagic <> nil then
  begin
    Result := True;
    if UserMagic.wMagIdx = SKILL_75 then
      OpenSuperShiled
    else
      DoSpell(UserMagic, TargeX, TargeY, TargeTBaseObject);

    // HZQ 消除永真条件
    if { (UserMagic.wMagIdx >= Low(m_SkillUseTick)) and } (UserMagic.wMagIdx <= High(m_SkillUseTick)) then
    begin
      m_SkillUseTick[UserMagic.wMagIdx] := MyGetTickCount;
    end;
    m_dwLastAttackTick := MyGetTickCount;
  end;
end;

function THeroObject.HeroAttackTarget(): Boolean;

  function GetDirXY(nTargetX, nTargetY: Integer): Byte;
  var
    n10: Integer;
    n14: Integer;
  begin
    n10 := nTargetX;
    n14 := nTargetY;
    Result := DR_DOWN; // 南
    if n10 > m_nCurrX then
    begin
      Result := DR_RIGHT; // 东
      if n14 > m_nCurrY then
        Result := DR_DOWNRIGHT; // 东南向
      if n14 < m_nCurrY then
        Result := DR_UPRIGHT; // 东北向
    end
    else
    begin
      if n10 < m_nCurrX then
      begin
        Result := DR_LEFT; // 西
        if n14 > m_nCurrY then
          Result := DR_DOWNLEFT; // 西南向
        if n14 < m_nCurrY then
          Result := DR_UPLEFT; // 西北向
      end
      else
      begin
        if n14 > m_nCurrY then
          Result := DR_DOWN // 南
        else if n14 < m_nCurrY then
          Result := DR_UP; // 正北
      end;
    end;
  end;

  function GotoMasterXY(var nTargetX, nTargetY: Integer): Boolean;
  var
    I, nDir: Integer;
    btArrDirs: array [0 .. 7] of Byte;
  begin
    Result := False;
    if (m_Master <> nil) and ((abs(m_Master.m_nCurrX - m_nCurrX) > 3) or (abs(m_Master.m_nCurrY - m_nCurrY) > 3)) and
      (not m_boProtectStatus) then
    begin
      nTargetX := m_nCurrX;
      nTargetY := m_nCurrY;
      nDir := GetDirXY(m_Master.m_nCurrX, m_Master.m_nCurrY);
      case nDir of
        DR_UP:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_UPRIGHT;
            btArrDirs[2] := DR_UPLEFT;
            btArrDirs[3] := DR_LEFT;
            btArrDirs[4] := DR_RIGHT;

            for I := 0 to 4 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_UPRIGHT:
          begin
            if (m_Master.m_btDirection = DR_RIGHT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_UP) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_UP;
              btArrDirs[2] := DR_RIGHT;
              btArrDirs[3] := DR_DOWNRIGHT;
              btArrDirs[4] := DR_DOWN;

              for I := 0 to 4 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
        DR_RIGHT:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_UPRIGHT;
            btArrDirs[2] := DR_DOWNRIGHT;
            btArrDirs[3] := DR_DOWN;

            for I := 0 to 3 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_DOWNRIGHT:
          begin
            if (m_Master.m_btDirection = DR_RIGHT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_DOWN) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX + 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX + 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_RIGHT;
              btArrDirs[2] := DR_DOWN;
              btArrDirs[3] := DR_DOWNLEFT;
              for I := 0 to 3 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
        DR_DOWN:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_DOWNRIGHT;
            btArrDirs[2] := DR_DOWNLEFT;
            btArrDirs[3] := DR_LEFT;
            btArrDirs[4] := DR_RIGHT;

            for I := 0 to 4 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;

              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_DOWNLEFT:
          begin
            if (m_Master.m_btDirection = DR_LEFT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_DOWN) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY + 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY + 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_DOWN;
              btArrDirs[2] := DR_LEFT;
              btArrDirs[3] := DR_DOWNRIGHT;
              btArrDirs[4] := DR_UPLEFT;

              for I := 0 to 4 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;

                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
        DR_LEFT:
          begin
            btArrDirs[0] := nDir;
            btArrDirs[1] := DR_DOWNLEFT;
            btArrDirs[2] := DR_UPLEFT;
            btArrDirs[3] := DR_DOWN;
            btArrDirs[4] := DR_UP;

            for I := 0 to 4 do
            begin
              nTargetX := m_nCurrX;
              nTargetY := m_nCurrY;
              if I >= 3 then
                Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
              else
                Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

              if Result then
                Break;
            end;
          end;
        DR_UPLEFT:
          begin
            if (m_Master.m_btDirection = DR_LEFT) then
            begin
              if (abs(m_Master.m_nCurrY - m_nCurrY) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end
            else if (m_Master.m_btDirection = DR_UP) then
            begin
              if (abs(m_Master.m_nCurrX - m_nCurrX) = 1) and m_PEnvir.CanWalkEx2(Self, m_nCurrX - 1, m_nCurrY - 1, False) then
              begin
                nTargetX := m_nCurrX - 1;
                nTargetY := m_nCurrY - 1;
                Result := True;
              end;
            end;

            if not Result then
            begin
              btArrDirs[0] := nDir;
              btArrDirs[1] := DR_LEFT;
              btArrDirs[2] := DR_UP;
              btArrDirs[3] := DR_DOWNLEFT;
              btArrDirs[4] := DR_DOWN;

              for I := 0 to 4 do
              begin
                nTargetX := m_nCurrX;
                nTargetY := m_nCurrY;
                if I >= 3 then
                  Result := GetGotoXY(btArrDirs[I], nTargetX, nTargetY)
                else
                  Result := GetRuntoXY(btArrDirs[I], nTargetX, nTargetY);

                if Result then
                  Break;
              end;
            end;
          end;
      end;
    end;
  end;

var
  nTargetX, nTargetY: Integer;
begin
  Result := False;

  // 英雄进入安全区内就不打PK目标 chongchong 2014-10-30
  if (m_TargetCret <> nil) and (m_btAttackMode in [0, 3]) then
  begin
    if (m_TargetCret.m_btRaceServer = RC_PLAYOBJECT) or
      ((m_TargetCret.Master <> nil) and (m_TargetCret.Master.m_btRaceServer = RC_PLAYOBJECT)) then
    begin
      if (m_Master <> nil) and (m_Master.InSafeZone) then
      begin
        DelTargetCreat;
        Exit;
      end
      else if (m_TargetCret.InSafeZone) or InSafeZone then
      begin
        if GotoMasterXY(nTargetX, nTargetY) then
        begin
          SetTargetXY(nTargetX, nTargetY);
          ResetTempMoveStatus;
          if (abs(m_nCurrX - nTargetX) > 1) or (abs(m_nCurrY - nTargetY) > 1) then
          begin
            if HeroRuntoTargetXY then
            begin
              Result := True;
            end;
          end
          else
          begin
            if HeroGotoTargetXY then
            begin
              Result := True;
            end;
          end;
        end
        else if ((abs(m_Master.m_nCurrX - m_nCurrX) > 3) or (abs(m_Master.m_nCurrY - m_nCurrY) > 3)) then
        begin
          WalkTo(Random(8), False);
        end;

        Exit;
      end;
    end;
  end;

  try
    if (m_TargetCret <> nil) and (not((m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
      (m_TargetCret.InSafeZone))) then
    begin
      // if m_Abil.Level > 6 then
      begin
        case m_btJob of
          JOB_WARR:
            Result := HeroWarrAttackTarget;
          JOB_WIZARD:
            Result := HeroWizardAttackTarget;
          JOB_TAOS:
            Result := HeroTaosAttackTarget;
        else
          Exit;
        end
      end;

      // 引擎是否使用物理攻击 - 战士除外
      // else if (m_btJob = JOB_WARR) or g_Config.boWarrorAttack then
      // Result := HeroBasicAttackTarget;                    // 物理攻击

      if (not Result) and (m_TargetCret <> nil) then
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
      // 道士职业，无目标时进入函数，给主人加血
      if m_btJob = JOB_TAOS then
        Result := HeroTaosAttackTarget
      else if m_btJob = JOB_WIZARD then
        Result := HeroWizardAttackTarget;
    end;

    { else if m_HeroAutoDong then
      begin
      case m_btJob of
      JOB_WIZARD:
      begin
      Result := HeroWizardAttackTarget;
      end;
      JOB_TAOS:
      begin
      Result := HeroTaosAttackTarget;
      end;
      end;
      end
    }
  except
    MainOutMessage('[Exception] TPlayObject:HeroAttackTarget')
  end;
end;

function THeroObject.GroupAttackProcess(): Boolean;
var
  GroupMagic: pTUserMagic;
  UseGroupMagicOK: Boolean;
  Range: Integer;
  AngryAgainValue: Integer;
  Offset1, Offset2: Integer;
  boRun: Boolean;
begin
  Result := False;

  if m_btAngryValue <= 0 then
  begin
    m_btAngryValue := 0;
    m_boUseGroupSpell := False; // 怒气减完，准备回复
  end;

  if not m_boUseGroupSpell then // 未使用合击，回复怒气
  begin
    ProcessAngryChangeUP;
  end
  else // 使用合击
  begin
    // 攻击失败可再次攻击怒槽值 chongchong 2013-10-26
    // MainOutMessage('开始合击~~~~~~~~~~~~~~~~~~~~~~~~~~~~');

    AngryAgainValue := g_Config.dwAngryAgainValue;
    if AngryAgainValue = 0 then
      AngryAgainValue := 1
    else if AngryAgainValue > 100 then
      AngryAgainValue := 100;

    UseGroupMagicOK := False;

    if (m_Master.m_btJob = 0) and (m_btJob = 0) then
      boRun := (m_TargetCret <> nil) and (not m_Master.m_boDeath) and (not m_Master.m_boGhost) and
        ((m_boLastGroupSpellOK and (m_btAngryValue >= g_Config.btMaxAngryValue)) or
        (not m_boLastGroupSpellOK and (m_btAngryValue >= Round(g_Config.btMaxAngryValue / 100 * AngryAgainValue)))) and
        (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= 10) and (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= 10) and
        (abs(m_Master.m_nCurrX - m_nCurrX) <= 10) and (abs(m_Master.m_nCurrY - m_nCurrY) <= 10) and
        (abs(m_TargetCret.m_nCurrX - m_Master.m_nCurrX) <= 10) and (abs(m_TargetCret.m_nCurrY - m_Master.m_nCurrY) <= 10)
    else
      boRun := (m_TargetCret <> nil) and (not m_Master.m_boDeath) and (not m_Master.m_boGhost) and
        ((m_boLastGroupSpellOK and (m_btAngryValue >= g_Config.btMaxAngryValue)) or
        (not m_boLastGroupSpellOK and (m_btAngryValue >= Round(g_Config.btMaxAngryValue / 100 * AngryAgainValue)))) and
        (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= g_Config.nMagicAttackRage) and
        (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= g_Config.nMagicAttackRage) and
        (abs(m_TargetCret.m_nCurrX - m_Master.m_nCurrX) <= g_Config.nMagicAttackRage) and
        (abs(m_TargetCret.m_nCurrY - m_Master.m_nCurrY) <= g_Config.nMagicAttackRage);

    if boRun then
    begin
      GroupMagic := FindMagic(GetGroupMagicId);
      if (GroupMagic <> nil) then
      begin
        { 破魂斩合击技能 }
        Range := g_Config.nSkill60PowerRange; // 视野范围 默认为 4
        if (m_btJob = JOB_WARR) and (GroupMagic.wMagIdx = SKILL_60) and CanLineAttack(Range) and TSmartObject(m_Master)
          .CanLineAttack(Range) then
        begin
          if ((m_Master.m_nCurrX = m_TargetCret.m_nCurrX) or (m_TargetCret.m_nCurrY = m_TargetCret.m_nCurrY) or
            (abs(m_Master.m_nCurrX - m_TargetCret.m_nCurrX) = abs(m_Master.m_nCurrY - m_TargetCret.m_nCurrY))) and
            ((m_nCurrX = m_TargetCret.m_nCurrX) or (m_nCurrY = m_TargetCret.m_nCurrY) or
            (abs(m_nCurrX - m_TargetCret.m_nCurrX) = abs(m_nCurrY - m_TargetCret.m_nCurrY))) then
          begin
            // 刚飞过来的，等会再使用合击，不然人物会先释放，因为英雄还有一个飞的魔法和加载地图等
            if MyGetTickCount - m_dwMapMoveTick <= 100 then
            begin
              FWaitGroupAttack := True;
              FWaitGroupAttackTick := MyGetTickCount - GetAttackIntervalTime(False);
            end
            else
            begin
              m_Master.m_btDirection := GetNextDirection(m_Master.m_nCurrX, m_Master.m_nCurrY, m_TargetCret.m_nCurrX,
                m_TargetCret.m_nCurrY);
              m_Master.AttackDir(m_TargetCret, 12, m_Master.m_btDirection);
              TPlayObject(m_Master).m_dwLastAttackTick := MyGetTickCount();

              m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
              m_dwTargetFocusTick := MyGetTickCount();
              AttackDir(m_TargetCret, 12, m_btDirection);
              m_dwLastAttackTick := MyGetTickCount();

              UseGroupMagicOK := True;
            end;
          end;
        end
        else
        begin
          // 修正到目标的合击距离不能超过10 chongchong 2018-07-02 12:00:56
          Offset1 := g_Config.nMagicAttackRage; // 原来 10
          Offset2 := g_Config.nMagicAttackRage; // 原来 10
          // 战士职业合击必须离目标3格以内 chongchong 2013-10-26
          // 在此处再一次判断是由于战战合击的间距g_Config.nSkill60PowerRange可能大于2
          if m_btJob = 0 then
            Offset1 := 2;
          if m_Master.m_btJob = 0 then
            Offset2 := 2;
          if (abs(m_TargetCret.m_nCurrX - m_nCurrX) <= Offset1) and (abs(m_TargetCret.m_nCurrY - m_nCurrY) <= Offset1) and
            (abs(m_Master.m_nCurrX - m_TargetCret.m_nCurrX) <= Offset2) and
            (abs(m_Master.m_nCurrY - m_TargetCret.m_nCurrY) <= Offset2) then
          begin
            if (AllowUseMagic(GroupMagic.wMagIdx, True)) then
            begin
              if (MyGetTickCount - m_dwLastAttackTick <= GetAttackIntervalTime(False)) and
                (MyGetTickCount - TPlayObject(m_Master).m_dwActionTick <= 1200) then
              begin
                FWaitGroupAttack := True;
                FWaitGroupAttackTick := MyGetTickCount;
                // MainOutMessage('合击时间不到~~~~~~~~~~~~~~~~~~~~~~~~~~~~');
                Exit;
              end;

              // 由于客户端劈星斩及雷霆一击动画不对，修正魔法由非战士职业的来发 chongchong 2013-10-26
              if m_btJob = 0 then
              begin
                if TSmartObject(m_Master).DoSpell(GroupMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret) then
                begin // 使用魔法
                  UseGroupMagicOK := True;
                end;
              end
              else if DoSpell(GroupMagic, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret) then
              begin // 使用魔法
                UseGroupMagicOK := True;
              end;
            end
            else
            begin
              // MainOutMessage('合击条件不足~~~~~~~~~~~~~~~~~~~~');
            end;
          end
          else
          begin
            // MainOutMessage('合击距离不对~~~~~~~~~~~~~~~~~~~~');
          end;
        end;

        if UseGroupMagicOK then
        begin
          m_boUseGroupSpell := False;

          m_btAngryValue := 0; // Dec(m_btAngryValue, g_Config.btMaxAngryValue);
          SendMsg(Self, RM_REFANGRYVALUE, MakeWord(m_btAngryValue, g_Config.btMaxAngryValue), 0, 0, 0, '');

          // HZQ 消除永真条件
          if { (GroupMagic.wMagIdx >= Low(m_SkillUseTick)) and } (GroupMagic.wMagIdx <= High(m_SkillUseTick)) then
          begin
            m_SkillUseTick[GroupMagic.wMagIdx] := MyGetTickCount;
          end;

          m_dwHitTick := MyGetTickCount();
          m_dwLastAttackTick := MyGetTickCount;
          // MainOutMessage('合击成功~~~~~~~~~~~~~~~~~~~~~~~~~~~~');
        end;
      end;
    end
    else
    begin
      // MainOutMessage('合击他妈的搞不清~~~~~~~~~~~~~~~~~~~~~~~~~~~~');
    end;

    { 合击使用不成功，开始减怒气 }
    if not UseGroupMagicOK then
    begin
      ProcessAngryChangeDown;
    end
    else
    begin
      FWaitGroupAttack := False;
    end;

    Result := UseGroupMagicOK;
    m_boLastGroupSpellOK := Result;

    // 在周围一堆怪物的情况下，用了合击时锁定怪物，可能锁定的怪物较远，导致走不过去也攻击不了，傻着不动了 chongchong 2016-04-21
    {
      if not m_boBeforeGroupSpellTarget then
      m_boTarget := False;
    }
  end;
end;

procedure THeroObject.ProcessAngryChangeUP;
var
  nTime: Integer;
begin
  // 不允许使用合击
  if not g_Config.boHeroJointAttack then
  begin
    if m_btAngryValue <> 0 then
    begin
      m_btAngryValue := 0;
      m_boUseGroupSpell := False;
      SendMsg(Self, RM_REFANGRYVALUE, MakeWord(0, g_Config.btMaxAngryValue), 0, 0, 0, '');
    end;
    Exit;
  end;

  if m_btAngryValue < g_Config.btMaxAngryValue then
  begin
    nTime := g_Config.nAddAngryValueTime;

    if m_boDecAddAngryValueTime then
    begin
      nTime := nTime - m_nDecAddAngryValueTimeValue;
      if nTime < 0 then
        nTime := 0;
    end;

    nTime := Max(0, Round(nTime / 100 * (100 - m_WAbil.NewValue[9])));
    if MyGetTickCount() - m_dwAddAngryValueTick > nTime then
    begin
      m_dwAddAngryValueTick := MyGetTickCount();
      // 减火龙之心，加怒气
      if (WearFirDragon and (m_UseItems[U_BUJUK].Dura > 0)) or ((g_nKey_HeroExt = 1) and g_Config.boNoNeedFirDragon) then
      begin
        if not g_Config.boNoNeedFirDragon then
        begin
          if m_UseItems[U_BUJUK].Dura >= g_Config.nDecFirDragonPoint then
            Dec(m_UseItems[U_BUJUK].Dura, g_Config.nDecFirDragonPoint)
          else
            m_UseItems[U_BUJUK].Dura := 0;
        end;

        Inc(m_btAngryValue, g_Config.nAddAngryValue);

        // SendMsg(Self, RM_DURACHANGE, U_BUJUK, , m_UseItems[U_BUJUK].DuraMax, 0, '');
        SendMsg(Self, RM_REFANGRYVALUE, MakeWord(m_btAngryValue, g_Config.btMaxAngryValue), 1, m_UseItems[U_BUJUK].Dura, 0, '');
      end;
    end;
  end;
end;

procedure THeroObject.ProcessAngryChangeDown;
var
  nTime: Integer;
begin
  // 等待合击的时候，不要减怒气，不然减怒气速度很快的时候，战战合击很难成功 2020-11-25 22:47:11
  if FWaitGroupAttack and (MyGetTickCount - FWaitGroupAttackTick <= GetAttackIntervalTime(False) + 100) then
  begin
    Exit;
  end;

  // 不允许使用合击
  if not g_Config.boHeroJointAttack then
  begin
    if m_btAngryValue <> 0 then
    begin
      m_btAngryValue := 0;
      m_boUseGroupSpell := False;
      SendMsg(Self, RM_REFANGRYVALUE, MakeWord(0, g_Config.btMaxAngryValue), 0, 0, 0, '');
    end;
    Exit;
  end;

  if m_btAngryValue > 0 then
  begin
    nTime := g_Config.nAddAngryValueTime;

    if m_boDecAddAngryValueTime then
    begin
      nTime := nTime - m_nDecAddAngryValueTimeValue;
      if nTime < 0 then
        nTime := 0;
    end;

    nTime := Max(0, Round(nTime / 100 * (100 - m_WAbil.NewValue[9])));

    if MyGetTickCount() - m_dwAddAngryValueTick > nTime then
    begin
      m_dwAddAngryValueTick := MyGetTickCount();

      Dec(m_btAngryValue, g_Config.nAddAngryValue);
      if m_btAngryValue <= 0 then
      begin
        m_btAngryValue := 0;
        m_boUseGroupSpell := False; // 怒气减完，准备回复
      end;

      SendMsg(Self, RM_REFANGRYVALUE, MakeWord(m_btAngryValue, g_Config.btMaxAngryValue), 0, 0, 0, '');
    end;
  end;
end;

function THeroObject.ContinueousAttack: Boolean;

  function GetHitMode(MagicID: Integer): Integer;
  begin
    if MagicID = SKILL_101 then
      Result := 16 // 三绝杀
    else if MagicID = SKILL_102 then
      Result := 17 // 断岳斩
    else if MagicID = SKILL_103 then
      Result := 18 // 横扫千军
    else
      Result := 0;
  end;

var
  MagicList: TList;
  UserMagic: pTUserMagic;
  I, II, nSpellPoint, nMagicCount: Integer;
  bt06: Byte;
  wHitMode: Word;
begin
  Result := False;
  if m_boGhost or m_boDeath or (not m_boCanSpell) (* or m_boCobweb {被网罩住不能使用魔法} *) or m_boDuanJin then
  begin
    if m_boContinuous then
      m_boContinuous := False;
    Exit;
  end;
  if g_Config.boContinuousAttackUseNG and (not m_boTrainingNG) then
  begin
    if m_boContinuous then
      m_boContinuous := False;
    Exit;
  end;

  if (m_TargetCret = nil) then
  begin
    if m_boContinuous then
      m_boContinuous := False;
    Exit;
  end;

  // 英雄连击不攻击怪物 chongchong 2013-11-10
  if (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(m_Master).m_boHeroContinuousNoHitMon) and
    (not(m_TargetCret.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) then
  begin
    if m_boContinuous then
      m_boContinuous := False;
    Exit;
  end;

  if not m_boContinuous then
  begin
    if (MyGetTickCount - m_dwUseContinuousMagicTick < Max(0, g_Config.nUseContinuousMagicTime - m_nContinuousMagic_DecTime) * 1000)
    then
      Exit;

    MagicList := TList.Create;
    try
      { 取所有的连击技能 }
      for I := 0 to m_MagicList.Count - 1 do
      begin
        UserMagic := m_MagicList.Items[I];
        if (UserMagic.MagicAttr = mtContinuous) then
          MagicList.Add(UserMagic);
      end;

      if (MagicList.Count = 0) then
        Exit;

      SetLength(m_CurrContinuousMagic, 0);
      m_nCurrContinuousMagicIndex := 0;

      if m_boOpenLastContinuous then // 第四个连击开启
        nMagicCount := Length(m_ContinuousMagicOrder)
      else
        nMagicCount := Length(m_ContinuousMagicOrder) - 1;

      // 检测是否有重复的连击
      for I := 0 to nMagicCount - 1 do
      begin
        if m_ContinuousMagicOrder[I] > 1 then
        begin
          for II := I + 1 to nMagicCount - 1 do
          begin
            if m_ContinuousMagicOrder[I] = m_ContinuousMagicOrder[II] then
            begin
              m_ContinuousMagicOrder[I] := 0;
              Break;
            end;
          end;
        end;
      end;

      // 去掉固定的连击技能
      for I := 0 to nMagicCount - 1 do
      begin
        if m_ContinuousMagicOrder[I] > 1 then
        begin
          for II := 0 to MagicList.Count - 1 do
          begin
            UserMagic := MagicList.Items[II];
            if UserMagic.wMagIdx = m_ContinuousMagicOrder[I] then
            begin
              MagicList.Delete(II);
              Break;
            end;
          end;
        end;
      end;

      for I := 0 to nMagicCount - 1 do
      begin
        UserMagic := nil;
        // 获取随机的连击技能
        if m_ContinuousMagicOrder[I] = 1 then
        begin
          if MagicList.Count > 0 then
          begin
            II := Random(MagicList.Count);
            UserMagic := MagicList.Items[II];
            MagicList.Delete(II);
          end;
        end
        else if m_ContinuousMagicOrder[I] > 1 then
          UserMagic := FindContinuousMagic(m_ContinuousMagicOrder[I]);

        if (UserMagic <> nil) then
        begin
          SetLength(m_CurrContinuousMagic, Length(m_CurrContinuousMagic) + 1);
          m_CurrContinuousMagic[Length(m_CurrContinuousMagic) - 1] := UserMagic^;
        end
      end;

      if Length(m_CurrContinuousMagic) > 0 then
      begin
        m_boContinuous := True;
        m_dwUseContinuousMagicTick := MyGetTickCount;
      end;
    finally
      MagicList.Free;
    end;
  end;

  if not m_boContinuous then
    Exit;

  case m_btJob of
    0:
      begin
        // 当释放了一次连击后，目标跑动了打不到时，下次的连击不要从头开始 chongchong 2017-11-15
        if (not GetAttackDir(m_TargetCret, bt06)) and (not GetAttackDir(m_TargetCret, 2, bt06)) then
        begin
          // m_boContinuous := False;
          Exit;
        end;
      end;
    1, 2:
      begin
        if ((abs(m_TargetCret.m_nCurrX - m_nCurrX) >= 8) or (abs(m_TargetCret.m_nCurrY - m_nCurrY) >= 8)) then
        begin
          m_boContinuous := False;
          Exit;
        end;
      end;
  end;

  if ((MyGetTickCount - m_dwLastAttackTick) > 1000) and (MyGetTickCount - m_dwMoveTimeTick >= GetAttackIntervalTime(False)) then
  begin
    if m_nCurrContinuousMagicIndex < Length(m_CurrContinuousMagic) then
    begin
      nSpellPoint := GetSpellPoint(@m_CurrContinuousMagic[m_nCurrContinuousMagicIndex]);
      if (nSpellPoint > 0) then
      begin
        { 如果 连击用MP时MP不够 或者连击用内功时内存值不够，退出连击 }
        if not((m_WAbil.MP >= nSpellPoint) or (g_Config.boContinuousAttackUseNG and (m_AbilNG.NH >= nSpellPoint))) then
        begin
          m_boContinuous := False;
          Exit;
        end
      end;

      m_dwLastAttackTick := MyGetTickCount;
      if g_Config.boContinuousAttackUseNG then
      begin
        if (nSpellPoint > 0) and (m_AbilNG.NH >= nSpellPoint) then
        begin
          m_AbilNG.NH := Max(0, m_AbilNG.NH - nSpellPoint);
          RefAbilNH;
        end;
      end
      else if (nSpellPoint > 0) and (m_WAbil.MP >= nSpellPoint) then
      begin
        DamageSpell(nSpellPoint);
        HealthSpellChanged();
      end;

      if m_btJob = 0 then
      begin
        if m_CurrContinuousMagic[m_nCurrContinuousMagicIndex].wMagIdx = 100 then
        begin
          // 追心刺
          if m_btDirection <> bt06 then
          begin
            m_btDirection := bt06;
          end;

          DoMotaebo100(m_TargetCret, @m_CurrContinuousMagic[m_nCurrContinuousMagicIndex]);

          // 使用技能后才计算技能cd时间 chongchong 2017-12-10
          if m_nCurrContinuousMagicIndex = 0 then
          begin
            m_dwUseContinuousMagicTick := MyGetTickCount;
          end;
        end
        else
        begin
          wHitMode := GetHitMode(m_CurrContinuousMagic[m_nCurrContinuousMagicIndex].wMagIdx);
          if wHitMode > 0 then
          begin
            AttackDir(m_TargetCret, wHitMode, bt06);

            // 使用技能后才计算技能cd时间 chongchong 2017-12-10
            if m_nCurrContinuousMagicIndex = 0 then
            begin
              m_dwUseContinuousMagicTick := MyGetTickCount;
            end;
          end
          else
          begin
            m_boContinuous := False;
            Exit;
          end;
        end;
      end
      else
      begin
        DoSpell(@m_CurrContinuousMagic[m_nCurrContinuousMagicIndex], m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, m_TargetCret);

        // 使用技能后才计算技能cd时间 chongchong 2017-12-10
        if m_nCurrContinuousMagicIndex = 0 then
        begin
          m_dwUseContinuousMagicTick := MyGetTickCount;
        end;
      end;

      Inc(m_nCurrContinuousMagicIndex);
    end
    else
    begin
      m_boContinuous := False;
      Exit;
    end;
  end;
  Result := True;
end;

function THeroObject.GetAttackIntervalTime(RecalHitSpeed: Boolean): LongWord; // 获取各职业攻击间隔
begin
  Result := 0;
  case m_btJob of
    JOB_WARR:
      begin
        Result := g_Config.dwHeroWarrorAttackTime;

        // 要支持 H.CHANGESPEED chongchong 2018-05-11
        if m_nNpcAttackSpeed <> 0 then
          Result := Max(Result - (g_Config.dwIncSpeedDecInterval * m_nNpcAttackSpeed), 50);
      end;
    JOB_WIZARD:
      begin
        Result := g_Config.dwHeroWizardAttackTime;

        // 要支持 H.CHANGESPEED chongchong 2018-05-11
        if m_nSpellSpeed <> 0 then
          Result := Max(Result - m_nSpellSpeed * g_ClientConfig.dwIncSpeedDecInterval, 50);
      end;
    JOB_TAOS:
      begin
        Result := g_Config.dwHeroTaoistAttackTime;

        // 要支持 H.CHANGESPEED chongchong 2018-05-11
        if m_nSpellSpeed <> 0 then
          Result := Max(Result - m_nSpellSpeed * g_ClientConfig.dwIncSpeedDecInterval, 50);
      end;
  end;

  { TODO -ochongchong -c新增 : 英雄计算武器速度 }
  if RecalHitSpeed and (Result <> 0) and (m_nHitSpeed > 0) and g_Config.boHeroCalcWeaponSpeed then
  begin
    Result := Max(Result - g_Config.dwIncSpeedDecInterval * m_nHitSpeed, 50); // 防止负数出错
  end;
end;

function THeroObject.GetMagicInfoEx(nMagic: Integer): pTUserMagic;
begin
  Result := nil;
  if (nMagic > 0) { and (nMagic < 99) } then
    Result := m_UserMagics[nMagic];
end;

procedure THeroObject.WeightChanged;
begin
  m_WAbil.Weight := RecalcBagWeight;
  SendDefMessage(SM_HEROWEIGHTCHANGED, m_WAbil.Weight, m_WAbil.WearWeight, m_WAbil.HandWeight, 0, '');
end;

function THeroObject.RunToNext(nX, nY: Integer): Boolean;
begin
  if m_boDuanJin or m_boCobwebWindingStatus then
    Result := WalkToNext(nX, nY)
  else
    Result := inherited RunToNext(nX, nY);
end;

function THeroObject.HeroGotoNext(IsFollowMaster: Boolean): Boolean;
begin
  if (m_nMoveIndex = Length(m_MovePath) - 1) and (abs(m_MovePath[m_nMoveIndex].X - m_nCurrX) <= 1) and
    (abs(m_MovePath[m_nMoveIndex].Y - m_nCurrY) <= 1) then
  begin
    Result := False;
    m_nMoveIndex := -1;
    SetLength(m_MovePath, 0);
    m_MovePath := nil;
  end
  else
    Result := GotoNext;
end;

function THeroObject.DoMotaebo100(BaseObject: TBaseObject; UserMagic: pTUserMagic): Boolean;

  function CanMotaebo(BaseObject: TBaseObject): Boolean;
  begin
    Result := False;

    if ((m_Abil.Level > BaseObject.m_Abil.Level) or (g_Config.boDoMotaebo100PushSameLevel and
      (BaseObject.m_Abil.Level = m_Abil.Level))) and (not BaseObject.m_boStickMode) then
    begin
      // 追心刺修改: CanMotaebo函数中不能加几率，不然这里可以推，下面的CanMotaebo(BaseObject_30)又不能推，就会出现 穿过目标导致目标卡位 chongchong 2017-11-18
      // nC := m_Abil.Level - BaseObject.m_Abil.Level;
      // if Random(20) < ((UserMagic.btLevel * 4) + 6 + nC) then
      begin
        if IsProperTarget(BaseObject) then
          Result := True;
      end;
    end;
  end;

var
  I, n20: Integer;
  PoseCreate: TBaseObject;
  BaseObject_30: TBaseObject;
  BaseObject_34: TBaseObject;
  nX, nY: Integer;
  nOldX, nOldY, nSelfStep: Integer;
  sPushedInfo: string;
  PushedObject: TPushedObject;
  PushedObjectList: TList;
resourcestring
  sExceptionMsg1 = '[Exception] TPlayObject.DoMotaebo100 GotoLable1';
  sExceptionMsg2 = '[Exception] TPlayObject.DoMotaebo100 GotoLable2';
begin
  Result := False;
  // m_btDirection := nDir;
  BaseObject_34 := nil;
  nSelfStep := 0;

  PushedObjectList := TList.Create;
  try
    PoseCreate := GetPoseCreate();
    if (PoseCreate <> nil) and (BaseObject.m_nCurrX = PoseCreate.m_nCurrX) and (BaseObject.m_nCurrY = PoseCreate.m_nCurrY) then
    begin
      for I := 0 to g_Config.nDoMotaebo100PushDistance - 1 do
      begin // Max(2, nMagicLevel + 1)
        PoseCreate := GetPoseCreate();
        if PoseCreate <> nil then
        begin
          if IsProperTarget(PoseCreate) then
          begin
            BaseObject_34 := PoseCreate;
          end;

          // 追心刺修改: CanMotaebo函数中不能加几率，不然这里可以推，下面的CanMotaebo(BaseObject_30)又不能推，就会出现 穿过目标导致目标卡位 chongchong 2017-11-18
          if not CanMotaebo(PoseCreate) then
            Break;

          if UserMagic.btLevel >= 3 then
          begin
            if m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, 2, nX, nY) then
            begin // 推动第二格的角色
              BaseObject_30 := m_PEnvir.GetMovingObject(nX, nY, True);
              if (BaseObject_30 <> nil) and CanMotaebo(BaseObject_30) then
              begin
                nOldX := BaseObject_30.m_nCurrX;
                nOldY := BaseObject_30.m_nCurrY;
                BaseObject_30.CharPushed_Skill100(m_btDirection, 1);
                if (nOldX <> BaseObject_30.m_nCurrX) or (nOldY <> BaseObject_30.m_nCurrY) then
                  if PushedObjectList.IndexOf(BaseObject_30) < 0 then
                  begin
                    BaseObject_30.m_btPushedStep := 1;
                    PushedObjectList.Add(BaseObject_30);
                  end
                  else
                  begin
                    BaseObject_30.m_btPushedStep := m_btPushedStep + 1;
                  end;
              end;
            end;
          end;

          nOldX := PoseCreate.m_nCurrX;
          nOldY := PoseCreate.m_nCurrY;

          if PoseCreate.CharPushed_Skill100(m_btDirection, 1) <> 1 then
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

          GetFrontPosition(nX, nY);

          if m_boImprison then
          begin
            if (nX < m_nImprisonPos.X - m_nImprisonRange) or (nX > m_nImprisonPos.X + m_nImprisonRange) or
              (nY < m_nImprisonPos.Y - m_nImprisonRange) or (nY > m_nImprisonPos.Y + m_nImprisonRange) then
            begin
              Break;
            end;
          end;

          if m_PEnvir.MoveToMovingObject(m_nCurrX, m_nCurrY, Self, nX, nY, False) then
          begin // 自己也往前走动
            m_nCurrX := nX;
            m_nCurrY := nY;
            // SendRefMsg(RM_RUSH, nDir, m_nCurrX, m_nCurrY, 0, '');
            Inc(nSelfStep);
            Result := True;
          end;
        end; // 004C32D7  if PoseCreate <> nil  then begin
      end; // 004C32DD for i:=0 to Max(2,nMagicLevel + 1) do begin
    end
    else
    begin // 004C32E8 if PoseCreate <> nil  then begin
      for I := 0 to g_Config.nDoMotaebo100PushDistance - 1 do
      begin
        GetFrontPosition(nX, nY); // sub_004B2790
        if m_PEnvir.MoveToMovingObject(m_nCurrX, m_nCurrY, Self, nX, nY, False) then
        begin
          m_nCurrX := nX;
          m_nCurrY := nY;
          Inc(nSelfStep);
        end
        else
        begin
          if not m_PEnvir.CanWalk(nX, nY, True) then
          begin
            Break;
          end;
        end;
      end;
    end;

    sPushedInfo := '';
    for I := 0 to PushedObjectList.Count - 1 do
    begin
      PoseCreate := TBaseObject(PushedObjectList.Items[I]);
      PushedObject.nRecogId := NativeInt(PoseCreate);
      PushedObject.nCurrX := PoseCreate.m_nCurrX;
      PushedObject.nCurrY := PoseCreate.m_nCurrY;
      PushedObject.btDir := PoseCreate.m_btDirection;
      PushedObject.btStep := PoseCreate.m_btPushedStep;
      sPushedInfo := sPushedInfo + EncodeBuffer(@PushedObject, SizeOf(TPushedObject)) + '/';
    end;
  finally
    PushedObjectList.Free;
  end;

  SendRefMsg(RM_100HIT, m_btDirection, m_nCurrX, m_nCurrY, nSelfStep, sPushedInfo);

  if BaseObject_34 = nil then
  begin
    BaseObject_34 := BaseObject;
  end;

  // 战技连击锁定 chongchong 2013-11-11
  if (BaseObject <> nil) and (Random(100) < g_Config.btWarrContinuousStatusLocks[0]) and IsProperTarget(BaseObject) then
  begin
    BaseObject.OpenContinuousMagicLock(g_Config.nWarrContinuousStatusLockTimes[0]);
    // BaseObject.MakePosion(POISON_STONE, 2, 0);
  end;

  if (BaseObject_34 <> nil) and IsProperTarget(BaseObject_34) then
  begin
    // 破魔法盾chongchong 2013-11-15
    if (Random(100) <= g_Config.Skill100BreakDefenceUpRate) and (BaseObject_34.m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] <> 0) then
    begin
      BaseObject_34.m_boAbilMagBubbleDefence := False;
      BaseObject_34.m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] := 0; // m_boAbilMagBubbleDefence := False;
      BaseObject_34.m_dwStatusArrTick[STATE_BUBBLEDEFENCEUP { 0x214 } ] := MyGetTickCount();
      BaseObject_34.m_nCharStatus := BaseObject_34.GetCharStatus();
      BaseObject_34.StatusChanged();

      BaseObject_34.SendRefMsg(RM_BROKENSHIELD, BaseObject_34.m_btDirection, BaseObject_34.m_nCurrX,
        BaseObject_34.m_nCurrY, 0, '');
    end;

    // 破新武力盾chongchong 2013-11-15
    if (Random(100) <= g_Config.Skill100BreakDefenceUpRate) and (BaseObject_34.m_wStatusTimeArr[STATE_NEWHITBUBBLEDEFENCEUP] <> 0)
    then
    begin
      BaseObject_34.m_wStatusTimeArr[STATE_NEWHITBUBBLEDEFENCEUP] := 0; // m_boAbilMagBubbleDefence := False;
      BaseObject_34.m_dwStatusArrTick[STATE_NEWHITBUBBLEDEFENCEUP { 0x214 } ] := MyGetTickCount();
      BaseObject_34.m_nCharStatus := BaseObject_34.GetCharStatus();
      BaseObject_34.StatusChanged();

      BaseObject_34.SendRefMsg(RM_BROKENSHIELD, BaseObject_34.m_btDirection, BaseObject_34.m_nCurrX,
        BaseObject_34.m_nCurrY, 0, '');
    end;

    // 破新道力盾chongchong 2013-11-15
    if (Random(100) <= g_Config.Skill100BreakDefenceUpRate) and (BaseObject_34.m_wStatusTimeArr[STATE_NEWMAGBUBBLEDEFENCEUP] <> 0)
    then
    begin
      BaseObject_34.m_wStatusTimeArr[STATE_NEWMAGBUBBLEDEFENCEUP] := 0; // m_boAbilMagBubbleDefence := False;
      BaseObject_34.m_dwStatusArrTick[STATE_NEWMAGBUBBLEDEFENCEUP { 0x214 } ] := MyGetTickCount();
      BaseObject_34.m_nCharStatus := BaseObject_34.GetCharStatus();
      BaseObject_34.StatusChanged();

      BaseObject_34.SendRefMsg(RM_BROKENSHIELD, BaseObject_34.m_btDirection, BaseObject_34.m_nCurrX,
        BaseObject_34.m_nCurrY, 0, '');
    end;

    n20 := GetAttackPower(m_WAbil.DC1, Integer((m_WAbil.DC2 - m_WAbil.DC1)));

    n20 := GetNewLevelPower(n20, UserMagic); // 取强化技能攻击伤害
    n20 := Round(n20 * (g_Config.SkillContinuousPowerRates[0] / 100)); // 攻击力倍数
    // 技能每提升一级伤害增加
    n20 := Min(LongWord(n20 + Round(n20 * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));

    n20 := GetSkillContinuousBlastHitPower(Self, BaseObject_34, UserMagic, n20); // 连击暴击

    n20 := GetPowerRateAdd(BaseObject_34, n20); // 2020-09-12 23:24:45

    n20 := GetMagicPercentPower(100, n20, BaseObject_34); // 装备技能威力 2020-11-06 22:12:27

    if Random(100) < g_Config.SkillContinuousCloseDefenseRates[0] then
      n20 := BaseObject_34.GetHitStruckDamage(Self, n20, nil); // 忽视目标防御

    n20 := BaseObject_34.NewAbilPower(2, n20); // 物伤减少

    n20 := BaseObject_34.GetAttackPowerMax(n20); // 伤害土封顶

    n20 := BaseObject_34.StruckDamage(n20, Self, UserMagic.wMagIdx);
    BaseObject_34.SendRefMsg(RM_STRUCK, n20, BaseObject_34.m_WAbil.HP, BaseObject_34.m_WAbil.MaxHP, NativeInt(Self), '');
    if BaseObject_34.m_btRaceServer <> RC_PLAYOBJECT then
    begin
      BaseObject_34.SendMsg(BaseObject_34, RM_STRUCK, n20, BaseObject_34.m_WAbil.HP, BaseObject_34.m_WAbil.MaxHP,
        NativeInt(Self), '');
    end;

    m_wCurrMagicId := 100;
    m_CurrTarget := BaseObject_34;

    BaseObject_34.m_CurrTarget := Self;
    BaseObject_34.m_wCurrMagicId := 100;
  end;
end;

procedure THeroObject.IncBeadExp(dwExp: LongWord; IsFromNPC: Boolean = False);
var
  I: Integer;
  dwItemExp, dwUseExp: LongWord;
  dwAddExp: LongWord;
  dLastDate: PDouble;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  nInt64: Int64;
  nDura: Integer;
begin
  if dwExp > 0 then
  begin
    for I := 0 to m_ItemList.Count - 1 do
    begin
      UserItem := m_ItemList.Items[I];
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem = nil) or (StdItem.StdMode <> 49) or (UserItem.Dura >= UserItem.DuraMax) then
        Continue;

      // 21亿修改这里不确定chongchong 2015-02-14
      // if (StdItem.AC <= 0) or (m_WAbil.Level <= StdItem.AC) then
      if (StdItem.AC1 <= 0) or (m_WAbil.Level <= StdItem.AC1) then
      begin
        if StdItem.Reserved1 > 0 then
        begin
          dLastDate := @UserItem.btValue[4];
          if Date > dLastDate^ then
            Continue; // 超过天数不能在聚集经验
        end;

        dwItemExp := UserItem.btValue[0];

        // 当NPC获取经验时，收集比例为0时，按100%收集 chongchong 2015-07-29
        if IsFromNPC and (StdItem.Shape = 0) then
          dwAddExp := dwExp
        else
          dwAddExp := Round(StdItem.Shape / 100 * dwExp);

        if g_FunctionNPC <> nil then
        begin
          TPlayObject(m_Master).m_nScriptGotoCount := 0;
          m_dwBeadExp := dwAddExp;
          m_nBeadSource := StdItem.Source;
          g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroGetBeadExp', False);
          dwAddExp := m_dwBeadExp;
          m_dwBeadExp := 0;
          m_nBeadSource := 0;
        end;
        if dwAddExp < 1 then
          Exit;

        nInt64 := dwItemExp + dwAddExp;
        nDura := nInt64 div 10000;
        if nDura > 0 then
        begin
          if UserItem.Dura + nDura >= UserItem.DuraMax then
          begin
            dwUseExp := (UserItem.DuraMax - UserItem.Dura) * 10000 - dwItemExp;

            if IsFromNPC and (StdItem.Shape = 0) then
              dwUseExp := dwUseExp
            else
              dwUseExp := Round(dwUseExp * 100 / StdItem.Shape);

            dwExp := dwExp - dwUseExp;
            nInt64 := 0;

            UserItem.Dura := UserItem.DuraMax;
          end
          else
          begin
            UserItem.Dura := Min(UserItem.Dura + nDura, UserItem.DuraMax);
            nInt64 := nInt64 mod 10000;
            dwExp := 0;
          end;
        end
        else
        begin
          dwExp := 0;
        end;

        UserItem.btValue[0] := Min(nInt64, High(LongWord));

        SendUpdateItem(UserItem);

        { if dwItemExp + dwAddExp >= 10000 then begin
          UserItem.Dura := UserItem.Dura + 1;
          PExp^ := Min(dwItemExp + dwAddExp - 10000, High(LongWord));
          SendUpdateItem(UserItem);
          end else begin
          PExp^ := Min(dwItemExp + dwAddExp, High(LongWord));
          end; }

        if UserItem.Dura >= UserItem.DuraMax then
        begin
          SysMsg(StdItem.Name + '的经验已聚满！', 251, 249, t_Hint);
        end;

        if dwExp = 0 then
          Break;
      end;
    end;

    if g_Config.boRecordBeadExp and (dwExp > 0) then
    begin
      nInt64 := m_dwRecordBeadExp + dwExp;
      m_dwRecordBeadExp := Min(nInt64, High(LongWord));
    end;
  end;
end;

procedure THeroObject.DelTargetCreat;
begin
  {
    if m_boTarget and (m_TargetCret <> nil) and (not m_TargetCret.m_boGhost) and (not m_TargetCret.m_boDeath) then
    begin

    end
    else
  }
  inherited;
  ResetTempMoveStatus;
end;

procedure THeroObject.RecalcAbilitys;
var
  CurHP, CurMP: LongWord;
  I, nValue: Integer;
  BBObj: TBaseObject;
begin
  // 英雄 修复套装属性加HP/MP时，待HP/MP满后收回重召英雄 HP/MP 又不满 chongchong 2014-11-29
  CurHP := m_WAbil.HP;
  CurMP := m_WAbil.MP;
  inherited RecalcAbilitys;
  if CurHP <= m_WAbil.MaxHP then
    m_WAbil.HP := CurHP;
  if CurMP <= m_WAbil.MaxMP then
    m_WAbil.MP := CurMP;

  {
    if g_Config.boSlaveLevelupUseNewAttr then
    begin
    for I := 0 to m_SlaveList.Count - 1 do
    begin
    BaseObject := m_SlaveList.Items[I];
    if not (BaseObject.m_boDeath or BaseObject.m_boGhost) then
    begin
    //BaseObject.RecalcLevelAbilitys(False);
    BaseObject.RecalcAbilitys;
    end;
    end;
    end;
  }
  // 修正npc加攻击速度不能和装备速度叠加 chongchong 2018-05-18
  if g_Config.boHeroCalcWeaponSpeed then
  begin
    nValue := m_nHitSpeed + m_nNpcAttackSpeed;
    if nValue > High(m_nAttackSpeed) then
      m_nAttackSpeed := High(m_nAttackSpeed)
    else if nValue < Low(m_nAttackSpeed) then
      m_nAttackSpeed := Low(m_nAttackSpeed)
    else
      m_nAttackSpeed := nValue;

    RefGameSpeed;
  end;

  RecalcPlayCombatPower(Self);

  // 人物属性重算，就重算宝宝属性叠加 2019-07-06 18:28:13
  for I := 0 to m_SlaveList.Count - 1 do
  begin
    BBObj := m_SlaveList.Items[I];
    if BBObj = nil then
      Continue;

    if BBObj.m_boDeath or BBObj.m_boGhost then
      Continue;
    if BBObj.m_boAddMasterAttrToSelf then
    begin
      BBObj.RecalcAbilitys;
    end;
  end;

  if (g_FunctionNPC <> nil) and (not m_boDummyObject) and (m_Master <> nil) and (m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(m_Master).m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(TPlayObject(m_Master), '@HeroRecalcAbilitys', False);
  end;
end;

function THeroObject.GetQuestFlagStatus(nFlag: Integer): Integer;
var
  n10, n14: Integer;
begin
  Result := 0;
  Dec(nFlag);
  if nFlag < 0 then
    Exit;
  n10 := nFlag div 8;
  n14 := (nFlag mod 8);
  if (n10 - SizeOf(TQuestFlag)) < 0 then
  begin
    if ((128 shr n14) and (m_QuestFlag[n10])) <> 0 then
      Result := 1
    else
      Result := 0;
  end;
end;

procedure THeroObject.SetQuestFlagStatus(nFlag: Integer; nValue: Integer);
var
  n10, n14: Integer;
  bt15: Byte;
begin
  Dec(nFlag);
  if nFlag < 0 then
    Exit;
  n10 := nFlag div 8;
  n14 := (nFlag mod 8);
  if (n10 - SizeOf(TQuestFlag)) < 0 then
  begin
    bt15 := m_QuestFlag[n10];
    if nValue = 0 then
    begin
      m_QuestFlag[n10] := (not(128 shr n14)) and (bt15);
    end
    else
    begin
      m_QuestFlag[n10] := (128 shr n14) or (bt15);
    end;
  end;
end;

function THeroObject.RunTo(btDir: Byte; boFlag: Boolean): Boolean;
var
  nRunTime: LongWord;
  nMoveTick: LongWord;
  OldX, OldY: Integer;
begin
  Result := False;

  // 英雄被野蛮攻击后，停留一会，参照所谓的leg chongchong 2018-07-20 22:18:54
  if m_dwPushedTick <> 0 then
  begin
    if tick_diff(m_dwPushedTick, MyGetTickCount) >= 1000 then
    begin
      m_dwPushedTick := 0;
    end;
    Exit;
  end;

  if not m_boCanRun then
  begin
    Exit;
  end;

  nRunTime := GetMoveTime;
  nRunTime := Round(nRunTime / 100 * 90);

  nMoveTick := MyGetTickCount - m_HeroMoveTick;
  if nMoveTick >= nRunTime then
  begin
    OldX := m_nCurrX;
    OldY := m_nCurrY;
    Result := inherited RunTo(btDir, boFlag);
    if Result then
    begin
      m_AttackToMoveTick := MyGetTickCount - m_dwLastAttackTick;

      // 英雄跑去掉隐身chongchong 2017-12-09
      if m_boTransparent and (m_boHideMode) then
      begin
        m_wStatusTimeArr[STATE_TRANSPARENT { 0 0x70 } ] := 1;
        m_dwDecStatusArrTick[STATE_TRANSPARENT] := 0; // 2020-03-23 17:00:02
      end;

      if btDir <> FLastDir then
      begin
        if (FMovePointIndex < 0) or (FMovePointIndex > High(FMovePoints)) then
          FMovePointIndex := 0;
        FMovePoints[FMovePointIndex] := Point(OldX, OldY);
        FLastDir := btDir;
        Inc(FMovePointIndex);
        if (FMovePointIndex < 0) or (FMovePointIndex > High(FMovePoints)) then
          FMovePointIndex := 0;
      end;

      // SysMsg('移动间隔' + IntToStr(nMoveTick), c_Red, t_Hint);
      m_HeroMoveTick := MyGetTickCount;
      m_boLastAvoidTarget := False;
    end;
  end;
end;

function THeroObject.WalkTo(btDir: Byte; boFlag: Boolean): Boolean;
var
  nWalkTime: LongWord;
  nMoveTick: LongWord;
  OldX, OldY: Integer;
begin
  Result := False;

  // 英雄被野蛮攻击后，停留一会，参照所谓的leg chongchong 2018-07-20 22:18:54
  if m_dwPushedTick <> 0 then
  begin
    if tick_diff(m_dwPushedTick, MyGetTickCount) >= 1000 then
    begin
      m_dwPushedTick := 0;
    end;
    Exit;
  end;

  // 2019-10-21 14:22:51
  if not m_boCanWalk then
  begin
    Exit;
  end;

  nWalkTime := GetMoveTime;

  nWalkTime := Round(nWalkTime / 100 * 90);
  nMoveTick := MyGetTickCount - m_HeroMoveTick;
  if nMoveTick >= nWalkTime then
  begin
    OldX := m_nCurrX;
    OldY := m_nCurrY;
    Result := inherited WalkTo(btDir, boFlag);
    if Result then
    begin
      m_AttackToMoveTick := MyGetTickCount - m_dwLastAttackTick;

      if btDir <> FLastDir then
      begin
        FMovePoints[FMovePointIndex] := Point(OldX, OldY);
        FLastDir := btDir;
        Inc(FMovePointIndex);
        if FMovePointIndex > High(FMovePoints) then
          FMovePointIndex := 0;
      end;
      // SysMsg('移动间隔' + IntToStr(nMoveTick), c_Red, t_Hint);
      m_HeroMoveTick := MyGetTickCount;

      m_boLastAvoidTarget := False;
    end;
  end;
end;

procedure THeroObject.ResetTempMoveStatus;
begin
  FIsTempMoveSet := False;
  FIsStopTempMove := False;
end;

function THeroObject.AllowHeroMagicRate(MagicType: TMagicType; MagicID: Word): Boolean;
var
  I: Integer;
  HeroMagic: PHeroMagic;
begin
  Result := False;

  Randomize;
  for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
  begin
    HeroMagic := g_CustomHeroMagicMgr.Items[I];
    if (HeroMagic.MagicType = MagicType) and (HeroMagic.MagicID = MagicID) then
    begin
      if HeroMagic.Checked then
      begin
        Result := Random(HeroMagic.UseRate) = 0;
      end;
      Break;
    end;
  end;
end;

function THeroObject.AllowHeroMagicRate2(MagicType: TMagicType; MagicID: Word; CheckValue: Integer): Boolean;
var
  I: Integer;
  HeroMagic: PHeroMagic;
begin
  Result := False;
  Randomize;
  for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
  begin
    HeroMagic := g_CustomHeroMagicMgr.Items[I];
    if (HeroMagic.MagicType = MagicType) and (HeroMagic.MagicID = MagicID) then
    begin
      if HeroMagic.Checked then
      begin
        Result := Random(HeroMagic.UseRate) <= CheckValue;
      end;
      Break;
    end;
  end;
end;

function THeroObject.CheckHeroMagicUseCondition(Condition: PHeroMagicUseCondition; Target: TBaseObject): Boolean;
var
  I, PercentageValue, nCount: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
begin
  Result := True;
  if Condition.HeroLevelCheck.boChecked then
  begin
    if Condition.HeroLevelCheck.CompareType = hlctTargetLevel then
    begin
      case Condition.HeroLevelCheck.CompareSymbol of
        csLess:
          Result := m_Abil.Level < Target.m_Abil.Level;
        csLessOrEqual:
          Result := m_Abil.Level <= Target.m_Abil.Level;
        csEqual:
          Result := m_Abil.Level = Target.m_Abil.Level;
        csGreater:
          Result := m_Abil.Level > Target.m_Abil.Level;
        csGreaterorEqual:
          Result := m_Abil.Level >= Target.m_Abil.Level;
      end
    end
    else // if Condition.HeroLevelCheck.CompareType = hlctLevelNumber then
    begin
      case Condition.HeroLevelCheck.CompareSymbol of
        csLess:
          Result := m_Abil.Level < Condition.HeroLevelCheck.CompareValue;
        csLessOrEqual:
          Result := m_Abil.Level <= Condition.HeroLevelCheck.CompareValue;
        csEqual:
          Result := m_Abil.Level = Condition.HeroLevelCheck.CompareValue;
        csGreater:
          Result := m_Abil.Level > Condition.HeroLevelCheck.CompareValue;
        csGreaterorEqual:
          Result := m_Abil.Level >= Condition.HeroLevelCheck.CompareValue;
      end
    end;
  end;

  if Result and Condition.HeroHPCheck.boChecked then
  begin
    if Condition.HeroHPCheck.CompareType = hhpctNumber then
    begin
      case Condition.HeroHPCheck.CompareSymbol of
        csLess:
          Result := m_WAbil.HP < Condition.HeroHPCheck.CompareValue;
        csLessOrEqual:
          Result := m_WAbil.HP <= Condition.HeroHPCheck.CompareValue;
        csEqual:
          Result := m_WAbil.HP = Condition.HeroHPCheck.CompareValue;
        csGreater:
          Result := m_WAbil.HP > Condition.HeroHPCheck.CompareValue;
        csGreaterorEqual:
          Result := m_WAbil.HP >= Condition.HeroHPCheck.CompareValue;
      end
    end
    else // if Condition.HeroHPCheck.CompareType = hhpctPercentage then
    begin
      if m_WAbil.MaxHP > 0 then
        PercentageValue := Round(m_WAbil.HP / m_WAbil.MaxHP * 100)
      else
        PercentageValue := 0;

      case Condition.HeroHPCheck.CompareSymbol of
        csLess:
          Result := PercentageValue < Condition.HeroHPCheck.CompareValue;
        csLessOrEqual:
          Result := PercentageValue <= Condition.HeroHPCheck.CompareValue;
        csEqual:
          Result := PercentageValue = Condition.HeroHPCheck.CompareValue;
        csGreater:
          Result := PercentageValue > Condition.HeroHPCheck.CompareValue;
        csGreaterorEqual:
          Result := PercentageValue >= Condition.HeroHPCheck.CompareValue;
      end
    end;
  end;

  if Result and Condition.HeroMPCheck.boChecked then
  begin
    if Condition.HeroMPCheck.CompareType = hhpctNumber then
    begin
      case Condition.HeroMPCheck.CompareSymbol of
        csLess:
          Result := m_WAbil.MP < Condition.HeroMPCheck.CompareValue;
        csLessOrEqual:
          Result := m_WAbil.MP <= Condition.HeroMPCheck.CompareValue;
        csEqual:
          Result := m_WAbil.MP = Condition.HeroMPCheck.CompareValue;
        csGreater:
          Result := m_WAbil.MP > Condition.HeroMPCheck.CompareValue;
        csGreaterorEqual:
          Result := m_WAbil.MP >= Condition.HeroMPCheck.CompareValue;
      end
    end
    else // if Condition.HeroMPCheck.CompareType = hhpctPercentage then
    begin
      if m_WAbil.MaxMP > 0 then
        PercentageValue := Round(m_WAbil.MP / m_WAbil.MaxHP * 100)
      else
        PercentageValue := 0;

      case Condition.HeroMPCheck.CompareSymbol of
        csLess:
          Result := PercentageValue < Condition.HeroMPCheck.CompareValue;
        csLessOrEqual:
          Result := PercentageValue <= Condition.HeroMPCheck.CompareValue;
        csEqual:
          Result := PercentageValue = Condition.HeroMPCheck.CompareValue;
        csGreater:
          Result := PercentageValue > Condition.HeroMPCheck.CompareValue;
        csGreaterorEqual:
          Result := PercentageValue >= Condition.HeroMPCheck.CompareValue;
      end
    end;
  end;

  if Result and Condition.TargetHPCheck.boChecked then
  begin
    if Condition.TargetHPCheck.CompareType = hhpctNumber then
    begin
      case Condition.TargetHPCheck.CompareSymbol of
        csLess:
          Result := Target.m_WAbil.HP < Condition.TargetHPCheck.CompareValue;
        csLessOrEqual:
          Result := Target.m_WAbil.HP <= Condition.TargetHPCheck.CompareValue;
        csEqual:
          Result := Target.m_WAbil.HP = Condition.TargetHPCheck.CompareValue;
        csGreater:
          Result := Target.m_WAbil.HP > Condition.TargetHPCheck.CompareValue;
        csGreaterorEqual:
          Result := Target.m_WAbil.HP >= Condition.TargetHPCheck.CompareValue;
      end
    end
    else // if Condition.TargetHPCheck.CompareType = hhpctPercentage then
    begin
      if Target.m_WAbil.MaxHP > 0 then
        PercentageValue := Round(Target.m_WAbil.HP / Target.m_WAbil.MaxHP * 100)
      else
        PercentageValue := 0;

      case Condition.TargetHPCheck.CompareSymbol of
        csLess:
          Result := PercentageValue < Condition.TargetHPCheck.CompareValue;
        csLessOrEqual:
          Result := PercentageValue <= Condition.TargetHPCheck.CompareValue;
        csEqual:
          Result := PercentageValue = Condition.TargetHPCheck.CompareValue;
        csGreater:
          Result := PercentageValue > Condition.TargetHPCheck.CompareValue;
        csGreaterorEqual:
          Result := PercentageValue >= Condition.TargetHPCheck.CompareValue;
      end
    end;
  end;

  if Result and Condition.TargetMPCheck.boChecked then
  begin
    if Condition.TargetMPCheck.CompareType = hhpctNumber then
    begin
      case Condition.TargetMPCheck.CompareSymbol of
        csLess:
          Result := Target.m_WAbil.MP < Condition.TargetMPCheck.CompareValue;
        csLessOrEqual:
          Result := Target.m_WAbil.MP <= Condition.TargetMPCheck.CompareValue;
        csEqual:
          Result := Target.m_WAbil.MP = Condition.TargetMPCheck.CompareValue;
        csGreater:
          Result := Target.m_WAbil.MP > Condition.TargetMPCheck.CompareValue;
        csGreaterorEqual:
          Result := Target.m_WAbil.MP >= Condition.TargetMPCheck.CompareValue;
      end
    end
    else // if Condition.TargetMPCheck.CompareType = hMPctPercentage then
    begin
      if Target.m_WAbil.MaxMP > 0 then
        PercentageValue := Round(Target.m_WAbil.MP / Target.m_WAbil.MaxMP * 100)
      else
        PercentageValue := 0;

      case Condition.TargetMPCheck.CompareSymbol of
        csLess:
          Result := PercentageValue < Condition.TargetMPCheck.CompareValue;
        csLessOrEqual:
          Result := PercentageValue <= Condition.TargetMPCheck.CompareValue;
        csEqual:
          Result := PercentageValue = Condition.TargetMPCheck.CompareValue;
        csGreater:
          Result := PercentageValue > Condition.TargetMPCheck.CompareValue;
        csGreaterorEqual:
          Result := PercentageValue >= Condition.TargetMPCheck.CompareValue;
      end
    end;
  end;

  // 中红毒
  if Result and Condition.TargetStatusCheck.boPoisonDamageArmor then
  begin
    Result := Target.m_wStatusTimeArr[POISON_DAMAGEARMOR] > 0;
  end;

  // 中绿毒
  if Result and Condition.TargetStatusCheck.boPoisonDecHealth then
  begin
    Result := Target.m_wStatusTimeArr[POISON_DECHEALTH] > 0;
  end;

  // 中毒
  if Result and Condition.TargetStatusCheck.boPoisoning then
  begin
    Result := (Target.m_wStatusTimeArr[POISON_DAMAGEARMOR] > 0) or (Target.m_wStatusTimeArr[POISON_DECHEALTH] > 0);
  end;

  // 被麻痹
  if Result and Condition.TargetStatusCheck.boPoisonStone then
  begin
    Result := (Target.m_wStatusTimeArr[POISON_STONE] > 0);
  end;

  // 被冰冻
  if Result and Condition.TargetStatusCheck.boFrozen then
  begin
    Result := (Target.m_wStatusTimeArr[STATE_FROZEN] > 0);
  end;

  // 被永恒冰冻
  if Result and Condition.TargetStatusCheck.boForeverFrozen then
  begin
    Result := Target.m_boForeverFrozen;
  end;

  // 中蛛网
  if Result and Condition.TargetStatusCheck.boCobwebWinding then
  begin
    Result := Target.m_boCobwebWindingStatus;
  end;

  // 中红毒
  if Result and Condition.TargetStatusCheck.boUnPoisonDamageArmor then
  begin
    Result := Target.m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0;
  end;

  // 中绿毒
  if Result and Condition.TargetStatusCheck.boUnPoisonDecHealth then
  begin
    Result := Target.m_wStatusTimeArr[POISON_DECHEALTH] = 0;
  end;

  // 中毒
  if Result and Condition.TargetStatusCheck.boUnPoisoning then
  begin
    Result := (Target.m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0) or (Target.m_wStatusTimeArr[POISON_DECHEALTH] = 0);
  end;

  // 被麻痹
  if Result and Condition.TargetStatusCheck.boUnPoisonStone then
  begin
    Result := (Target.m_wStatusTimeArr[POISON_STONE] = 0);
  end;

  // 被冰冻
  if Result and Condition.TargetStatusCheck.boUnFrozen then
  begin
    Result := (Target.m_wStatusTimeArr[STATE_FROZEN] = 0);
  end;

  // 被永恒冰冻
  if Result and Condition.TargetStatusCheck.boUnForeverFrozen then
  begin
    Result := not Target.m_boForeverFrozen;
  end;

  // 中蛛网
  if Result and Condition.TargetStatusCheck.boUnCobwebWinding then
  begin
    Result := not Target.m_boCobwebWindingStatus;
  end;

  if Result and Condition.FriendCountCheck.boChecked and (Condition.FriendCountCheck.nCheckRange > 0) then
  begin
    nCount := 0;
    BaseObjectList := TList.Create;
    try
      Target.GetMapBaseObjects(Target.m_PEnvir, Target.m_nCurrX, Target.m_nCurrY, Condition.FriendCountCheck.nCheckRange,
        BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList[I]);

        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (BaseObject <> Target) and
          Target.IsProperFriend(BaseObject) then
        begin
          Inc(nCount);
        end;
      end;
    finally
      BaseObjectList.Free;
    end;

    Result := nCount > Condition.FriendCountCheck.nCheckValue;
  end;

  if Result and Condition.EnemyCountCheck.boChecked and (Condition.EnemyCountCheck.nCheckRange > 0) then
  begin
    nCount := 0;
    BaseObjectList := TList.Create;
    try
      Target.GetMapBaseObjects(Target.m_PEnvir, Target.m_nCurrX, Target.m_nCurrY, Condition.EnemyCountCheck.nCheckRange,
        BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList[I]);

        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and (BaseObject <> Target) and
          IsProperTarget(BaseObject) then
        begin
          Inc(nCount);
        end;
      end;
    finally
      BaseObjectList.Free;
    end;

    Result := nCount > Condition.EnemyCountCheck.nCheckValue;
  end;

  if Result and Condition.boStraightLineCheck then
  begin
    Result := (abs(Target.m_nCurrX - m_nCurrX) = 0) or (abs(Target.m_nCurrY - m_nCurrY) = 0) or
      (abs(Target.m_nCurrX - m_nCurrX) = abs(Target.m_nCurrY - m_nCurrY));
  end;
end;

function THeroObject.GotoNextOne(nX, nY: Integer; boRun: Boolean): Boolean;
var
  nTargetX, nTargetY: Integer;
  nTempX, nTempY: Integer;
  nDir1, nDir2: Byte;
  nRunTime: Integer;
  nMoveTick: LongWord;
begin
  if (nX = m_nCurrX) and (nY = m_nCurrY) then
  begin
    Result := True;
    Exit;
  end;

  nRunTime := GetMoveTime;

  nRunTime := Round(nRunTime / 100 * 90);
  nMoveTick := MyGetTickCount - m_HeroMoveTick;

  // 英雄行动时间未到，返回True chongchong 2018-01-07
  // function TSmartObject.StartPickUpItem: Boolean;
  // 不然在开始捡物时，会将物品加入到失败列表 m_PickUpItemFailList.Add
  if nMoveTick < nRunTime then
  begin
    Result := True;
    Exit;
  end;

  Result := False;
  if (abs(nX - m_nCurrX) <= 2) and (abs(nY - m_nCurrY) <= 2) then
  begin
    if (abs(nX - m_nCurrX) <= 1) and (abs(nY - m_nCurrY) <= 1) then
    begin
      Result := WalkToNext(nX, nY);
    end
    else
    begin
      if boRun then
        Result := RunToNext(nX, nY)
      else
        Result := WalkToNext(nX, nY);
    end;
  end;

  if not Result then
  begin
    // 修改 优化算法，不用寻路（寻路算法占用cpu） chongchong 2017-05-04
    if boRun then
    begin
      if GotoNearRuntoXY(nX, nY, nTargetX, nTargetY) then
      begin
        if ((nX <> nTargetX) or (nY <> nTargetY)) and GotoNearRuntoXY(nTargetX, nTargetY, nX, nY, nTempX, nTempY) then
        begin
          nDir1 := GetNextDirection(m_nCurrX, m_nCurrY, nTargetX, nTargetY);
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
          Result := RunToNext(nTargetX, nTargetY);
        end
        else
        begin
          Result := WalkToNext(nTargetX, nTargetY);
        end;
      end;
    end
    else
    begin
      if GotoNearGotoXY(nX, nY, nTargetX, nTargetY) then
      begin
        Result := WalkToNext(nTargetX, nTargetY);
      end;
    end;
  end;

  m_RunPos.nAttackCount := 0;
end;

function THeroObject.GetMoveTime: Integer;
begin
  case m_btJob of
    0:
      Result := g_Config.dwHeroWarrorWalkTime;
    1:
      Result := g_Config.dwHeroWizardWalkTime;
    2:
      Result := g_Config.dwHeroTaoistWalkTime;
  else
    Result := 500;
  end;

  // 要支持 H.CHANGESPEED chongchong 2018-05-11
  if m_nMoveSpeed <> 0 then // 行走速度 自动调整变速后时间间隔
    Result := Max(Result - (g_Config.dwIncMoveSpeedDecInterval * m_nMoveSpeed), 50);
end;

end.
