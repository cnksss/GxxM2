unit uCustomMagicUtils;

interface

uses
  Windows, SysUtils, Classes, IniFilesEx, Grobal2, CheckUnit, M2Threads;

const
  MaxCustomMagicLevel = 9;

type
  TCheckVarType = (cvtMoreThan, cvtLessThan, cvtEqual, cvtNotEqual, cvtQreaterEqual, cvtLessEqual);
  TMagicAttackDecAttributesType = (daAC, daMAC, daDC, daMC, daSC, daHitPoint, daSpeedPoint, daAntiMagic,
    daAntiPoison { , daContinueDecHP, daContinueMP } );

  TMagicProtectAddAttributesType = (aaAC, aaMAC, aaDC, aaMC, aaSC, aaHitPoint, aaSpeedPoint, aaAntiMagic, aaAntiPoison,
    aaNGDamage, aaNGDefense, aaHP, aaMP, aaMaxHP, aaMaxMP,
    aaHide { , aaContinueAddHP, aaContinueAddMP, aaDecAttackDamage, aaDecMagAttackDamage } );

  TItemElementsType = (ietBlastHit { 0 } , ietDamageAdd { 1 } , ietDamageDec { 2 } , ietSpellDamageDec { 3 } ,
    ietCloseDefense { 4 } , ietDamageRebound { 5 } , ietMonDropRate { 6 } , ietMaxHPAdd { 7 } , ietMaxMPAdd { 8 } , // 6  怪物爆率
    ietAngryValueTimAdd { 9 } , ietGroupDamageAdd { 10 } , ietHuamDropRate { 11 } , ietUndropRate { 12 } , // 11 人物爆率  12 防爆出率
    ietUnParalysis { 13 } , ietUnMagicShield { 14 } , ietUnRevival { 15 } , ietUnPosion { 16 } , ietUnTamming { 17 } ,
    ietUnFireCross { 18 } , ietUnFrozen { 19 } , ietUnCobwebWinding { 20 } , ietFatalBlowRate { 21 } , ietFatalBlowPower { 22 } ,
    ietFatalBlowDefense { 23 } , ietUnBlastHit { 24 }
    );

  TBreakDefenseType = (bdtHumDefense, bdtMonDefense, bdtHeroDefense, bdtHumMagDefense, bdtMonMagDefense, bdtHeroMagDefense);

const
  MagicWarrNGOptionNames: array [TMagicWarrNGOption] of string = ('无', '半月弯弓', '烈火剑法', '逐日剑法', '龙影剑法', '刺杀剑术', '开天斩', '双龙斩',
    '断空斩', '自定义技能1', '自定义技能2', '自定义技能3', '自定义技能4', '自定义技能5', '自定义技能6', '自定义技能7', '自定义技能8');

  CustomMagicLevelNames: array [0 .. MaxCustomMagicLevel + 1] of string = ('无强化', '强化1重', '强化2重', '强化3重', '强化4重', '强化5重', '强化6重',
    '强化7重', '强化8重', '强化9重', '9重后每重增加');

  MagicPlusLevelNames: array [TMagicPlusLevel] of string = ('无强化', '强化1-3重', '强化4-6重', '强化7-9重');

  MagicActionTypeNames: array [TMagicActionType] of string = ('魔法动作', '普通砍动作', '跳跃砍动作', '无动作', '自定义动作');

  MagicSoundTypeNames: array [TMagicSoundType] of string = ('ManWarr', 'WomanWarr', 'UseMagic', 'MagicFly', 'MagicExplosion',
    'MagicFail');

  MagicSwitchModeNames: array [TMagicSwitchMode] of string = ('无模式', '开关模式', '攻杀模式');

  CheckVarTypeNames: array [TCheckVarType] of string = ('>', '<', '=', '<>', '>=', '<=');

  MagicAttackDecAttributesTypeIniNames: array [TMagicAttackDecAttributesType] of string = ('AC', 'MAC', 'DC', 'MC', 'SC',
    'HitPoint', 'SpeedPoint', 'AntiMagic', 'AntiPoison' { , 'ContinueDecHP', 'ContinueMP' } );

  MagicAttackDecAttributesTypeNames: array [TMagicAttackDecAttributesType] of string = ('减防御', '减魔御', '减攻击', '减魔法', '减道术', '减准确',
    '减敏捷', '减魔法躲避', '减毒物躲避' { , '持续减HP', '持续减MP' } );

  MagicProtectAddAttributesTypeIniNames: array [TMagicProtectAddAttributesType] of string = ('AC', 'MAC', 'DC', 'MC', 'SC',
    'HitPoint', 'SpeedPoint', 'AntiMagic', 'AntiPoison', 'NGDamage', 'NGDefense', 'HP', 'MP', 'MaxHP', 'MaxMP',
    'Hide' { , ContinueAddHP, ContinueAddMP, DecAttackDamage, DecMagAttackDamage } );

  MagicProtectAddAttributesTypeNames: array [TMagicProtectAddAttributesType] of string = ('加防御', '加魔御', '加攻击', '加魔法', '加道术',
    '加准确', '加敏捷', '加魔法躲避', '加毒物躲避', '加内功伤害', '加内功防御', '单次加HP', '单次加MP', '加MaxHP', '加MaxMP',
    '加隐身' { , '持续加HP', '持续加MP', '抵御物理伤害', '抵御魔法伤害' } );

  ItemElementsTypeIniNames: array [TItemElementsType] of string = ('BlastHit' { 0 } , 'DamageAdd' { 1 } , 'DamageDec' { 2 } ,
    'SpellDamageDec' { 3 } , 'CloseDefense' { 4 } , 'DamageRebound' { 5 } , 'MonDropRate' { 6 } , 'MaxHPAdd' { 7 } ,
    'MaxMPAdd' { 8 } , // 6  怪物爆率
    'AngryValueTimAdd' { 9 } , 'GroupDamageAdd' { 10 } , 'HuamDropRate' { 11 } , 'UndropRate' { 12 } , // 11 人物爆率  12 防爆出率
    'UnParalysis' { 13 } , 'UnMagicShield' { 14 } , 'UnRevival' { 15 } , 'UnPosion' { 16 } , 'UnTamming' { 17 } ,
    'UnFireCross' { 18 } , 'UnFrozen' { 19 } , 'UnCobwebWinding' { 20 } , 'FatalBlowRate' { 21 } , 'FatalBlowPower' { 22 } ,
    'FatalBlowDefense' { 23 } , 'UnBlastHit');

  ItemElementsTypeNames: array [TItemElementsType] of string = ('暴击几率', '攻击伤害', '伤害吸收', '魔法防御', '忽视防御', '伤害反弹', '怪物爆率', '体力增加',
    '魔力增加', '怒气恢复', '合击伤害', '人物爆率', '防爆出率', '防止麻痹', '防止护身', '防止复活', '防止全毒', '防止诱惑', '防止火墙', '防止冰冻', '防止蛛网', '致命一击几率', '致命一击伤害',
    '致命一击防御', '暴击抗性');

  MagicAttackDecValueTypeNames: array [Boolean] of string = ('%', '点');

  MagicAttackDecTimeTypeNames: array [Boolean] of string = ('%', '秒');

  BreakDefenseTypeNames: array [TBreakDefenseType] of string = ('BreakHumDefense', 'BreakMonDefense', 'BreakHeroDefense',
    'BreakHumMagDefense', 'BreakMonMagDefense', 'BreakHeroMagDefense');

type
  // 附加伤害
  TMagicAdditionalDamage = record
    Checked: Boolean;
    Rate: Byte;
    Rate2: Byte;
    Time: Byte;
    TimeUnit: Byte;
    Time2: Byte;
  end;

  // 攻击减属性类型
  TMagicAdditionalDamageArray = array [0 .. 10] of TMagicAdditionalDamage;
  PMagicChangeAttributesRecord = ^TMagicChangeAttributesRecord;

  TMagicChangeAttributesRecord = record
    IsChecked: Boolean; // 启用
    Rate: Integer; // 几率
    RateAdd: Integer; // 几率递增
    LowValue: Integer; // 下限值
    LowValueIsPoint: Boolean; // 下限单位 False: %; True: 点数
    LowValueAdd: Integer; // 下限递增
    HighValue: Integer; // 上限值
    HighValueIsPoint: Boolean; // 上限单位 False: %; True: 点数
    HighValueAdd: Integer; // 上限限递增
    Time: Integer; // 时间
    TimeAdd: Integer; // 时间递增
    TimeAddIsPoint: Boolean; // 递增单位 False: %; True: 分钟
    ShowHint: Boolean;
    HintText: string;
  end;

  PMagicAttackChangeElementRecord = ^TMagicAttackChangeElementRecord;

  TMagicAttackChangeElementRecord = record
    IsChecked: Boolean; // 启用
    Rate: Integer; // 几率
    RateAdd: Integer; // 几率递增
    Value: Integer; // 修改值
    ValueIsPoint: Boolean; // 修改值单位 False: %; True: 点数
    ValueAdd: Integer; // 值递增
    Time: Integer; // 时间
    TimeAdd: Integer; // 时间递增
    TimeAddIsPoint: Boolean; // 递增单位 False: %; True: 分钟
    ShowHint: Boolean;
    HintText: string;
  end;

  TBreakDefenseInfo = record
    IsChecked: Boolean;
    Rate: Integer;
    RateAdd: Integer;
    Value: Integer;
    ValueAdd: Integer;
  end;

  PMagicServerConfig = ^TMagicServerConfig;

  TMagicServerConfig = record
    OperateMode: TCustomOperateMode; // 操作模式 chongchong 2014-09-06
    IsAttackUseNG: Boolean; // 使用内功值释放
    NoChangeDir: Boolean;
    DisableInSafeZone: Boolean; // 禁止安全区使用
    AttackDelayTime: Integer; // 伤害延时时间
    UseInterval: Integer; // 使用间隔
    // FailNoShowEff: Boolean;                             // 时间未到或用失败时不显示魔法
    FailMsg: string[40]; // 失败提示
    SucceedMsg: string[40]; // 成功提示
    CloseMsg: string[40]; // 关闭提示
    IsCheckVarValue: Boolean; // 检查变量
    NeedItem: TMagicNeedItem; // 所需佩戴物品
    NeedItemCount: Integer; // 所需物品数量
    NeedItemCustomItemName: string[ITEM_NAME_LEN];
    NeedItemUseBagItem: Boolean;
    CheckVarName: string[20];
    CheckVarType: TCheckVarType;
    CheckVarValue: Integer;
    CheckVarAdd: Integer;
    AttackMode: TCustomAttackMode; // 攻击方式
    AttackTarget: TCustomAttackTarget; // 攻击目标
    AttackPowerCalc: TCustomAttackPowerCalc; // 威力计算
    AttackPowerRates: array [0 .. MaxCustomMagicLevel + 1] of Integer; // 威力倍数
    AttackPowerLineAdd: Integer; // 线性攻击威力递增
    AttackPowerUndeadAdd: Integer; // 不死系怪物伤害加成
    AttackTeleportAttack: Boolean; // 瞬移攻击
    IsNoTeleportNoAttack: Boolean;
    AttackTeleportRate: Integer; // 瞬移几率
    AttackTeleportRunHum: Boolean;
    AttackTeleportRunMon: Boolean;
    AttackTeleportRunNpc: Boolean;
    AttackTeleportRunGuard: Boolean;
    AttackTeleportRunObstacle: Boolean;
    AttackTeleportWarDisHumRun: Boolean;
    AttackTeleportCannotRunItem: Boolean;
    AttackTeleportRush: Boolean; // 增加突进型瞬移 By 一支笔 at:2021-07-06 14:01:29
    AttackTeleportRushCount: Integer;
    AttackTeleportAfterDamage: Boolean; // 造成伤害后瞬移 By 一支笔 at:2021-07-06 14:02:16
    AttackNearRange: Integer; // 近攻范围
    AttackGroupRange: Integer; // 群攻范围
    AttackLineWidth: Integer; // 直线攻击宽度
    EnableAntiMagic: Boolean;
    EnableHitPoint: Boolean;
    EnabledCallMonster: Boolean; // 允许召唤怪物
    CallMonstersRate: Integer; // 怪物召唤几率
    CallMonstersRoyaltySec: Integer; // 召唤宝宝叛变时间
    CallMonsters: array [0 .. 1] of string[ITEM_NAME_LEN]; // 召唤怪物1-4
    CallMonsterNums: array [0 .. 1] of Integer; // 怪物1-4数量
    CallMonstersLevel: Integer;
    Additionals: TMagicAdditionalDamageArray;
    AdditionalHP0: Integer; // 绿毒掉血
    AdditionalHighLevel4: Boolean; // 可推动高等级
    AdditionalPushedType4: Byte; // 推动类型
    AttackTargetStatus: Boolean;
    AttackTargetStatusTime: Byte;
    AttackTargetStatusTimeUnit: Byte;
    AttackTargetStatusTime2: Byte;
    AttackTargetStatusDelay: Word;
    AttackSubAttrib: array [TMagicAttackDecAttributesType] of TMagicChangeAttributesRecord;
    AttackSubElements: array [TItemElementsType] of TMagicAttackChangeElementRecord;
    AttackBreakDefense: array [TBreakDefenseType] of TBreakDefenseInfo;
    ProtectAddAttrib: array [TMagicProtectAddAttributesType] of TMagicChangeAttributesRecord;
    ProtectAddElements: array [TItemElementsType] of TMagicAttackChangeElementRecord;
    // 保护模式
    ProtectAddHPSlow: Boolean; // 多次回血
    ProtectAddHpSlowCount: Word; // 回血次数
    ProtectTargetStatus: Boolean;
    ProtectTargetStatusTime: Byte;
    ProtectTargetStatusTimeUnit: Byte;
    ProtectTargetStatusTime2: Byte;
    ProtectTargetStatusDelay: Word;
    ProtectTargetRange: Integer; // 保护目标范围
    // ProtectSelfRate: Integer;                         // 自我保护几率
  end;

  TCustomMagicConfig = class(TObject)
  private
    FMagicName: string;
    FMagicID: Word;
    FIsMagicWarr: Boolean;
    // FMagicAttr: TMagicAttr;
    FIsChanged: Boolean;
  public
    ClientBaseConfig: TMagicClientBaseConfig;
    ClientConfigs: TMagicClientConfigs;
    ServerConfig: TMagicServerConfig;
  public
    constructor Create(AMagicName: string; AMagicID: Word; AIsMagicWarr: Boolean);
    destructor Destroy; override;
    property IsMagicWarr: Boolean read FIsMagicWarr write FIsMagicWarr;
    // property MagicAttr: TMagicAttr read FMagicAttr write FMagicAttr;
    procedure SaveToIniFile;
    procedure LoadFromIniFile;
    procedure SetChanged(Value: Boolean = True);

    property MagicName: string read FMagicName write FMagicName;
    property MagicID: Word read FMagicID;
    property IsChanged: Boolean read FIsChanged;
  end;

procedure SaveCustomMagicClientConfigs(MagicConfigs: TList; FileName: string);
function GetCustomMagicConfig(MagicID: Word): TCustomMagicConfig;

implementation

uses M2Share;

procedure SaveCustomMagicClientConfigs(MagicConfigs: TList; FileName: string);
var
  I, Len: Integer;
  CRC: Cardinal;
  P: PAnsiChar;
  MS: TMemoryStream;
  MagicConfig: TCustomMagicConfig;
  ClientConfig: TClientCustomMagicConfig;
begin
  MS := TMemoryStream.Create;
  try
    MS.Write(ClientCustomMagicConfigFlag, SizeOf(ClientCustomMagicConfigFlag));

    Len := MagicConfigs.Count;
    MS.Write(Len, SizeOf(Len));

    Len := SizeOf(TClientCustomMagicConfig);
    MS.Write(Len, SizeOf(Len));

    CRC := 0;
    MS.Write(CRC, SizeOf(CRC));

    for I := 0 to MagicConfigs.Count - 1 do
    begin
      MagicConfig := MagicConfigs.Items[I];

      ClientConfig.wMagicID := MagicConfig.FMagicID;
      ClientConfig.MagicBaseConfig := MagicConfig.ClientBaseConfig;
      ClientConfig.MagicConfigs := MagicConfig.ClientConfigs;

      if MagicConfig.IsMagicWarr then
        ClientConfig.btNearAttackRange := MagicConfig.ServerConfig.AttackNearRange
      else
        ClientConfig.btNearAttackRange := 1;

      ClientConfig.IsAttackUseNG := MagicConfig.ServerConfig.IsAttackUseNG;
      ClientConfig.NoChangeDir := MagicConfig.ServerConfig.NoChangeDir;
      // ClientConfig.FailMsg := MagicConfig.ServerConfig.FailMsg;

      if not MagicConfig.ServerConfig.IsCheckVarValue then
      begin
        ClientConfig.NeedItem := MagicConfig.ServerConfig.NeedItem;
        ClientConfig.NeedItemCount := MagicConfig.ServerConfig.NeedItemCount;
        ClientConfig.NeedItemCustomItemName := MagicConfig.ServerConfig.NeedItemCustomItemName;
        ClientConfig.NeedItemUseBagItem := MagicConfig.ServerConfig.NeedItemUseBagItem;
      end
      else
      begin
        ClientConfig.NeedItem := meiNone;
        ClientConfig.NeedItemCount := 0;
        ClientConfig.NeedItemCustomItemName := '';
        ClientConfig.NeedItemUseBagItem := False;
      end;

      MS.Write(ClientConfig, SizeOf(ClientConfig));
    end;

    Len := SizeOf(ClientCustomMagicConfigFlag) + SizeOf(Len) * 2 + SizeOf(CRC);
    P := MS.Memory;
    Inc(P, Len);
    CRC := BufferCRC(P, MS.Size - Len);

    Len := SizeOf(ClientCustomMagicConfigFlag) + SizeOf(Len) * 2;
    MS.Seek(Len, soFromBeginning);
    MS.Write(CRC, SizeOf(CRC));

    MS.SaveToFile(FileName);
  finally
    MS.Free;
  end;
end;

function GetCustomMagicConfig(MagicID: Word): TCustomMagicConfig;
var
  I: Integer;
  CustomMagicConfig: TCustomMagicConfig;
begin
  Result := nil;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomMagicList.LockR(6);
  try
{$IFEND}
    for I := 0 to UserEngine.m_CustomMagicList.Count - 1 do
    begin
      CustomMagicConfig := UserEngine.m_CustomMagicList[I];

      if CustomMagicConfig.MagicID = MagicID then
      begin
        Result := CustomMagicConfig;
        Exit;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMagicList.UnLockR;
  end;
{$IFEND}
end;

{ TCustomMagicConfig }
constructor TCustomMagicConfig.Create(AMagicName: string; AMagicID: Word; AIsMagicWarr: Boolean);
var
  I: Integer;
  MagicClientConfig: PMagicClientConfig;
  MagicPlusLevel: TMagicPlusLevel;
  SoundType: TMagicSoundType;
  ElementType: TItemElementsType;
  BreakDefenseType: TBreakDefenseType;
begin
  FIsChanged := False;

  FMagicName := AMagicName;
  FMagicID := AMagicID;
  FIsMagicWarr := AIsMagicWarr;

  // ClientBaseConfig.MagicLevelEnabled := False;
  // ClientBaseConfig.MagicWarr := False;
  ClientBaseConfig.MagicLock := False;

  if FIsMagicWarr then
    ClientBaseConfig.MagicActionType := matHit
  else
    ClientBaseConfig.MagicActionType := matSpell;

  ClientBaseConfig.MagicLockSelf := False;
  ClientBaseConfig.MagicSwitchMode := msmNone;
  ClientBaseConfig.SwitchModeNoClose := False;
  ClientBaseConfig.MagicActionStartIndex := 0;
  ClientBaseConfig.MagicActionPlayCount := 0;
  ClientBaseConfig.MagicActionEmptyCount := 0;
  ClientBaseConfig.MagicActionContinue := False;
  ClientBaseConfig.MagicAutoOpen := False;
  ClientBaseConfig.MagicWarrNGOption := mngoNone;

  ClientBaseConfig.NotRaiseHand := False;
  for MagicPlusLevel := Low(ClientConfigs) to High(ClientConfigs) do
  begin
    MagicClientConfig := @ClientConfigs[MagicPlusLevel];

    MagicClientConfig.Icon_File := -1;
    MagicClientConfig.Icon_Index := 0;

    for SoundType := Low(TMagicSoundType) to High(TMagicSoundType) do
      MagicClientConfig.Sounds[SoundType] := '';

    MagicClientConfig.Fly_File := -1;
    MagicClientConfig.Fly_StartIndex := 0;
    MagicClientConfig.Fly_PlayCount := 0;
    MagicClientConfig.Fly_EmptyCount := 0;
    MagicClientConfig.Fly_PlayTime := 100;
    MagicClientConfig.Fly_DrawMode := mdmBlend;
    MagicClientConfig.Fly_DirCount := mdcDir8;
    MagicClientConfig.Fly_CalcDir := True;
    MagicClientConfig.Fly_FireGunMode := False;
    MagicClientConfig.Fly_LightRange := 0;

    MagicClientConfig.FlyEff_File := -1;
    MagicClientConfig.FlyEff_StartIndex := 0;
    MagicClientConfig.FlyEff_DrawMode := mdmBlend;

    MagicClientConfig.Self_File := -1;
    MagicClientConfig.Self_StartIndex := 0;
    MagicClientConfig.Self_SyncHumAction := False;
    MagicClientConfig.Self_PlayCount := 0;
    MagicClientConfig.Self_EmptyCount := 0;
    MagicClientConfig.Self_PlayTime := 100;
    // MagicClientConfig.Self_PlayMode := mpmAttack;
    MagicClientConfig.Self_DrawOrder := mdoPriorSelf;
    MagicClientConfig.Self_DrawMode := mdmBlend;
    MagicClientConfig.Self_DirCalcType := mdctNone;
    MagicClientConfig.Self_DirCount := mdcDir8;
    MagicClientConfig.Self_PlayDelayAction := False;
    MagicClientConfig.Self_LightRange := 0;
    MagicClientConfig.Self_PlayFailNoDraw := False;

    MagicClientConfig.SelfKeep_File := -1;
    MagicClientConfig.SelfKeep_StartIndex := 0;
    MagicClientConfig.SelfKeep_PlayCount := 0;
    MagicClientConfig.SelfKeep_PlayTime := 0;
    MagicClientConfig.SelfKeep_DrawMode := mdmBlend;
    MagicClientConfig.SelfKeep_KeepTime := 0;
    MagicClientConfig.SelfKeep_KeepTime2 := 0;

    MagicClientConfig.FastMove_File := -1;
    MagicClientConfig.FastMove_StartIndex := 0;
    MagicClientConfig.FastMove_PlayCount := 0;
    MagicClientConfig.FastMove_EmptyCount := 0;
    MagicClientConfig.FastMove_PlayTime := 100;
    // MagicClientConfig.FastMove_PlayMode := mpmAttack;
    MagicClientConfig.FastMove_DrawOrder := mdoPriorSelf;
    MagicClientConfig.FastMove_DrawMode := mdmBlend;
    MagicClientConfig.FastMove_CalcDir := False;
    MagicClientConfig.FastMove_NoHitAction := False;
    MagicClientConfig.FastMove_LightRange := 0;

    MagicClientConfig.PreTarget_File := -1;
    MagicClientConfig.PreTarget_StartIndex := 0;
    MagicClientConfig.PreTarget_StartIndex2 := -1;
    MagicClientConfig.PreTarget_PlayCount := 0;
    MagicClientConfig.PreTarget_EmptyCount := 0;
    MagicClientConfig.PreTarget_PlayTime := 100;
    MagicClientConfig.PreTarget_DrawMode := mdmBlend;
    MagicClientConfig.PreTarget_DrawMode2 := mdmBlend;
    MagicClientConfig.PreTarget_CalcDir := False;
    MagicClientConfig.PreTarget_LockDraw := False;
    MagicClientConfig.PreTarget_LightRange := 0;

    MagicClientConfig.Target_File := -1;
    MagicClientConfig.Target_StartIndex := 0;
    MagicClientConfig.Target_StartIndex2 := -1;
    MagicClientConfig.Target_PlayCount := 0;
    MagicClientConfig.Target_PlayTime := 100;
    MagicClientConfig.Target_DrawMode := mdmBlend;
    MagicClientConfig.Target_DrawMode2 := mdmBlend;
    MagicClientConfig.Target_MultiPlay := False;
    MagicClientConfig.Target_LockDraw := False;
    MagicClientConfig.Target_LightRange := 0; // 照亮范围 chongchong 2015-03-04

    MagicClientConfig.Target_KeepPlay := False;; // 持久伤害 chongchong 2015-03-10
    MagicClientConfig.Target_KeepTime := 30; // 持久伤害时间 chongchong 2015-03-10
    MagicClientConfig.Target_KeepTime2 := 0;
    MagicClientConfig.Target_KeepAttackRange := 0; // 持久伤害范围 chongchong 2015-03-10
    MagicClientConfig.Target_KeepMultiPlay := False; // 持久伤害 每格单独播放 chongchong 2015-03-10
    MagicClientConfig.Target_KeepAttackInterval := 5; // 持久伤害间隔 chongchong 2015-03-10
    MagicClientConfig.Target_KeepLightRange := 0;

    MagicClientConfig.TargetStatus1_File := -1;
    MagicClientConfig.TargetStatus1_StartIndex := 0;
    MagicClientConfig.TargetStatus1_PlayCount := 0;
    MagicClientConfig.TargetStatus1_EmptyCount := 0;
    // MagicClientConfig.TargetStatus1_PlayTime := 100;
    MagicClientConfig.TargetStatus1_DrawMode := mdmBlend;
    MagicClientConfig.TargetStatus1_CalcDir := False;

    MagicClientConfig.TargetStatus2_File := -1;
    MagicClientConfig.TargetStatus2_StartIndex := 0;
    MagicClientConfig.TargetStatus2_PlayCount := 0;
    MagicClientConfig.TargetStatus2_EmptyCount := 0;
    // MagicClientConfig.TargetStatus2_PlayTime := 100;
    MagicClientConfig.TargetStatus2_DrawMode := mdmBlend;
    MagicClientConfig.TargetStatus2_CalcDir := False;
  end;

  ServerConfig.OperateMode := momAttack;
  ServerConfig.IsAttackUseNG := False;
  ServerConfig.NoChangeDir := False;
  ServerConfig.DisableInSafeZone := False;
  ServerConfig.AttackDelayTime := 500;
  ServerConfig.UseInterval := 0;
  // ServerConfig.FailNoShowEff := False;

  ServerConfig.FailMsg := '';
  ServerConfig.SucceedMsg := '';
  ServerConfig.CloseMsg := '';

  ServerConfig.IsCheckVarValue := False;
  ServerConfig.NeedItem := meiNone;
  ServerConfig.NeedItemCount := 0;
  ServerConfig.NeedItemCustomItemName := '';
  ServerConfig.NeedItemUseBagItem := False;
  ServerConfig.CheckVarName := '';
  ServerConfig.CheckVarType := cvtMoreThan;
  ServerConfig.CheckVarValue := 0;
  ServerConfig.CheckVarAdd := 0;

  ServerConfig.AttackMode := mamNear;
  ServerConfig.AttackTarget := matSingle;
  ServerConfig.AttackPowerCalc := mapcDC;

  for I := Low(ServerConfig.AttackPowerRates) to High(ServerConfig.AttackPowerRates) do
  begin
    if I < High(ServerConfig.AttackPowerRates) then
    begin
      ServerConfig.AttackPowerRates[I] := 100;
    end
    else
    begin
      ServerConfig.AttackPowerRates[I] := 0;
    end;
  end;

  ServerConfig.AttackPowerLineAdd := 0;
  ServerConfig.AttackPowerUndeadAdd := 0;

  ServerConfig.AttackNearRange := 1;
  ServerConfig.AttackGroupRange := 2;
  ServerConfig.EnableAntiMagic := False;
  ServerConfig.EnableHitPoint := False;

  ServerConfig.EnabledCallMonster := False;
  ServerConfig.CallMonstersRate := 0;
  ServerConfig.CallMonstersRoyaltySec := 0;
  ServerConfig.CallMonstersLevel := 1;
  for I := Low(ServerConfig.CallMonsters) to High(ServerConfig.CallMonsters) do
  begin
    ServerConfig.CallMonsters[I] := '';
    ServerConfig.CallMonsterNums[I] := 0;
  end;

  ServerConfig.AttackTeleportAttack := False;
  ServerConfig.IsNoTeleportNoAttack := False;
  ServerConfig.AttackTeleportRate := 0;
  ServerConfig.AttackTeleportRunHum := False;
  ServerConfig.AttackTeleportRunMon := False;
  ServerConfig.AttackTeleportRunNpc := False;
  ServerConfig.AttackTeleportRunGuard := False;
  ServerConfig.AttackTeleportRunObstacle := False;
  ServerConfig.AttackTeleportWarDisHumRun := False;
  ServerConfig.AttackTeleportCannotRunItem := False;

  ServerConfig.AttackTeleportRush := False;
  ServerConfig.AttackTeleportRushCount := 1;
  ServerConfig.AttackTeleportAfterDamage := False;

  for I := Low(ServerConfig.Additionals) to High(ServerConfig.Additionals) do
  begin
    ServerConfig.Additionals[I].Checked := False;
    ServerConfig.Additionals[I].Rate := 3;
    ServerConfig.Additionals[I].Rate2 := 0;
    ServerConfig.Additionals[I].Time := 3;
    ServerConfig.Additionals[I].TimeUnit := 0;
    ServerConfig.Additionals[I].Time2 := 0;
  end;

  ServerConfig.AdditionalHP0 := 3;
  ServerConfig.AdditionalHighLevel4 := False;
  ServerConfig.AdditionalPushedType4 := 0;

  ServerConfig.AttackTargetStatus := False;
  ServerConfig.AttackTargetStatusTime := 3;
  ServerConfig.AttackTargetStatusTimeUnit := 0;
  ServerConfig.AttackTargetStatusTime2 := 0;
  ServerConfig.AttackTargetStatusDelay := 0;

  FillChar(ServerConfig.AttackSubAttrib, SizeOf(ServerConfig.AttackSubAttrib), 0);

  ServerConfig.AttackSubAttrib[daAC].LowValue := 10;
  ServerConfig.AttackSubAttrib[daAC].HighValue := 10;
  ServerConfig.AttackSubAttrib[daAC].Time := 60;
  ServerConfig.AttackSubAttrib[daAC].HintText := '防御降低%AC1-%AC2，%Time秒';

  ServerConfig.AttackSubAttrib[daMAC].LowValue := 10;
  ServerConfig.AttackSubAttrib[daMAC].HighValue := 10;
  ServerConfig.AttackSubAttrib[daMAC].Time := 60;
  ServerConfig.AttackSubAttrib[daMAC].HintText := '魔御降低%MAC1-%MAC2，%Time秒';

  ServerConfig.AttackSubAttrib[daDC].LowValue := 10;
  ServerConfig.AttackSubAttrib[daDC].HighValue := 10;
  ServerConfig.AttackSubAttrib[daDC].Time := 10;
  ServerConfig.AttackSubAttrib[daDC].HintText := '攻击力降低%DC1-%DC2，%Time秒';

  ServerConfig.AttackSubAttrib[daMC].LowValue := 10;
  ServerConfig.AttackSubAttrib[daMC].HighValue := 10;
  ServerConfig.AttackSubAttrib[daMC].Time := 10;
  ServerConfig.AttackSubAttrib[daMC].HintText := '魔法力降低%MC1-%MC2，%Time秒';

  ServerConfig.AttackSubAttrib[daSC].LowValue := 10;
  ServerConfig.AttackSubAttrib[daSC].HighValue := 10;
  ServerConfig.AttackSubAttrib[daSC].Time := 10;
  ServerConfig.AttackSubAttrib[daSC].HintText := '道术力降低%SC1-%SC2，%Time秒';

  ServerConfig.AttackSubAttrib[daHitPoint].LowValue := 10;
  ServerConfig.AttackSubAttrib[daHitPoint].HighValue := 10;
  ServerConfig.AttackSubAttrib[daHitPoint].Time := 10;
  ServerConfig.AttackSubAttrib[daHitPoint].HintText := '准确降低%Point，%Time秒';

  ServerConfig.AttackSubAttrib[daSpeedPoint].LowValue := 10;
  ServerConfig.AttackSubAttrib[daSpeedPoint].HighValue := 10;
  ServerConfig.AttackSubAttrib[daSpeedPoint].Time := 10;
  ServerConfig.AttackSubAttrib[daSpeedPoint].HintText := '敏捷降低%Point，%Time秒';

  ServerConfig.AttackSubAttrib[daAntiMagic].LowValue := 10;
  ServerConfig.AttackSubAttrib[daAntiMagic].HighValue := 10;
  ServerConfig.AttackSubAttrib[daAntiMagic].Time := 10;
  ServerConfig.AttackSubAttrib[daAntiMagic].HintText := '魔法躲避降低%Point，%Time秒';

  ServerConfig.AttackSubAttrib[daAntiPoison].LowValue := 10;
  ServerConfig.AttackSubAttrib[daAntiPoison].HighValue := 10;
  ServerConfig.AttackSubAttrib[daAntiPoison].Time := 10;
  ServerConfig.AttackSubAttrib[daAntiPoison].HintText := '毒物躲避降低%Point，%Time秒';

  {
    ServerConfig.AttackSubAttrib[daContinueDecHP].LowValue := 10;
    ServerConfig.AttackSubAttrib[daContinueDecHP].HighValue := 10;
    ServerConfig.AttackSubAttrib[daContinueDecHP].Time := 10;
    ServerConfig.AttackSubAttrib[daContinueDecHP].HintText := '毒物躲避降低%Point，%Time秒';

    ServerConfig.AttackSubAttrib[daContinueDecMP].LowValue := 10;
    ServerConfig.AttackSubAttrib[daContinueDecMP].HighValue := 10;
    ServerConfig.AttackSubAttrib[daContinueDecMP].Time := 10;
    ServerConfig.AttackSubAttrib[daContinueDecMP].HintText := '毒物躲避降低%Point，%Time秒';
  }

  FillChar(ServerConfig.AttackSubElements, SizeOf(ServerConfig.AttackSubElements), 0);

  for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
  begin
    ServerConfig.AttackSubElements[ElementType].HintText := ItemElementsTypeNames[ElementType] + '降低%Point, %Time秒';
  end;

  for BreakDefenseType := Low(TBreakDefenseType) to High(TBreakDefenseType) do
  begin
    ServerConfig.AttackBreakDefense[BreakDefenseType].IsChecked := False;
    ServerConfig.AttackBreakDefense[BreakDefenseType].Rate := 0;
    ServerConfig.AttackBreakDefense[BreakDefenseType].RateAdd := 0;
    ServerConfig.AttackBreakDefense[BreakDefenseType].Value := 0;
    ServerConfig.AttackBreakDefense[BreakDefenseType].ValueAdd := 0;
  end;

  ServerConfig.ProtectAddAttrib[aaAC].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaAC].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaAC].Time := 60;
  ServerConfig.ProtectAddAttrib[aaAC].HintText := '防御提升%AC1-%AC2，%Time秒';

  ServerConfig.ProtectAddAttrib[aaMAC].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaMAC].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaMAC].Time := 60;
  ServerConfig.ProtectAddAttrib[aaMAC].HintText := '魔御提升%MAC1-%MAC2，%Time秒';

  ServerConfig.ProtectAddAttrib[aaDC].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaDC].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaDC].Time := 10;
  ServerConfig.ProtectAddAttrib[aaDC].HintText := '攻击力提升%DC1-%DC2，%Time秒';

  ServerConfig.ProtectAddAttrib[aaMC].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaMC].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaMC].Time := 10;
  ServerConfig.ProtectAddAttrib[aaMC].HintText := '魔法力提升%MC1-%MC2，%Time秒';

  ServerConfig.ProtectAddAttrib[aaSC].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaSC].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaSC].Time := 10;
  ServerConfig.ProtectAddAttrib[aaSC].HintText := '道术力提升%SC1-%SC2，%Time秒';

  ServerConfig.ProtectAddAttrib[aaHitPoint].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaHitPoint].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaHitPoint].Time := 10;
  ServerConfig.ProtectAddAttrib[aaHitPoint].HintText := '准确提升%Point，%Time秒';

  ServerConfig.ProtectAddAttrib[aaSpeedPoint].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaSpeedPoint].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaSpeedPoint].Time := 10;
  ServerConfig.ProtectAddAttrib[aaSpeedPoint].HintText := '敏捷提升%Point，%Time秒';

  ServerConfig.ProtectAddAttrib[aaAntiMagic].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaAntiMagic].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaAntiMagic].Time := 10;
  ServerConfig.ProtectAddAttrib[aaAntiMagic].HintText := '魔法躲避提升%Point，%Time秒';

  ServerConfig.ProtectAddAttrib[aaAntiPoison].LowValue := 10;
  ServerConfig.ProtectAddAttrib[aaAntiPoison].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaAntiPoison].Time := 10;
  ServerConfig.ProtectAddAttrib[aaAntiPoison].HintText := '毒物躲避提升%Point，%Time秒';

  ServerConfig.ProtectAddAttrib[aaNGDamage].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaNGDamage].Time := 10;
  ServerConfig.ProtectAddAttrib[aaNGDamage].HintText := '内功伤害提升%AddNGDamage，%Time秒';

  ServerConfig.ProtectAddAttrib[aaNGDefense].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaNGDefense].Time := 10;
  ServerConfig.ProtectAddAttrib[aaNGDefense].HintText := '内功防御提升%AddNGDefense，%Time秒';

  ServerConfig.ProtectAddAttrib[aaHP].HighValue := 50;
  ServerConfig.ProtectAddAttrib[aaHP].HintText := 'HP增加%Point';
  ServerConfig.ProtectAddHPSlow := False;
  ServerConfig.ProtectAddHpSlowCount := 0;

  ServerConfig.ProtectAddAttrib[aaMP].HighValue := 50;
  ServerConfig.ProtectAddAttrib[aaMP].HintText := 'HP增加%Point';

  ServerConfig.ProtectAddAttrib[aaMaxHP].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaMaxHP].Time := 10;
  ServerConfig.ProtectAddAttrib[aaMaxHP].HintText := 'MaxHP提升%Point，%Time秒';

  ServerConfig.ProtectAddAttrib[aaMaxMP].HighValue := 10;
  ServerConfig.ProtectAddAttrib[aaMaxMP].Time := 10;
  ServerConfig.ProtectAddAttrib[aaMaxMP].HintText := 'MaxHP提升%Point，%Time秒';

  ServerConfig.ProtectAddAttrib[aaHide].Time := 10;
  ServerConfig.ProtectAddAttrib[aaHide].TimeAdd := 0;
  ServerConfig.ProtectAddAttrib[aaHide].HintText := '进入隐身状态，%Time秒';

  for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
  begin
    ServerConfig.ProtectAddElements[ElementType].HintText := ItemElementsTypeNames[ElementType] + '提升%Point, %Time秒';
  end;

  ServerConfig.ProtectTargetStatus := False;
  ServerConfig.ProtectTargetStatusTime := 3;
  ServerConfig.ProtectTargetStatusTimeUnit := 0;
  ServerConfig.ProtectTargetStatusTime2 := 0;
  ServerConfig.ProtectTargetStatusDelay := 0;

  ServerConfig.ProtectTargetRange := 3;
  // ServerConfig.ProtectSelfRate := 50;

  LoadFromIniFile;
end;

destructor TCustomMagicConfig.Destroy;
begin
  inherited;
end;

procedure TCustomMagicConfig.SetChanged(Value: Boolean);
begin
  FIsChanged := Value;
end;

procedure TCustomMagicConfig.LoadFromIniFile;
var
  FileName, SectionName: string;
  IniFile: TIniFileEx;
  I, IntRead: Integer;
  MagicClientConfig: PMagicClientConfig;
  MagicPlusLevel: TMagicPlusLevel;

  SoundType: TMagicSoundType;
  ElementType: TItemElementsType;
  DecAttribType: TMagicAttackDecAttributesType;
  IncAttribType: TMagicProtectAddAttributesType;
  sTemp: string;
  BreakDefenseType: TBreakDefenseType;
begin
  FIsChanged := False;

  FileName := g_Config.sCustomMagicDir + FMagicName + '.ini';
  if not FileExists(FileName) then
    Exit;
  IniFile := TIniFileEx.Create(FileName);
  try
    SectionName := 'ClientConfig';
    // ClientBaseConfig.MagicLevelEnabled := IniFile.ReadBool(SectionName, 'MagicLevelEnabled', ClientBaseConfig.MagicLevelEnabled);
    // ClientBaseConfig.MagicWarr := IniFile.ReadBool(SectionName, 'MagicWarr', ClientBaseConfig.MagicWarr);
    ClientBaseConfig.MagicLock := IniFile.ReadBool(SectionName, 'MagicLock', ClientBaseConfig.MagicLock);
    ClientBaseConfig.MagicLockSelf := IniFile.ReadBool(SectionName, 'MagicLockSelf', ClientBaseConfig.MagicLockSelf);

    IntRead := IniFile.ReadInteger(SectionName, 'MagicSwitchMode', Integer(ClientBaseConfig.MagicSwitchMode));
    if (IntRead >= Integer(Low(TMagicSwitchMode))) and (IntRead <= Integer(High(TMagicSwitchMode))) then
      ClientBaseConfig.MagicSwitchMode := TMagicSwitchMode(IntRead);
    ClientBaseConfig.SwitchModeNoClose := IniFile.ReadBool(SectionName, 'SwitchModeNoClose', ClientBaseConfig.SwitchModeNoClose);

    IntRead := IniFile.ReadInteger(SectionName, 'MagicActionType', Integer(ClientBaseConfig.MagicActionType));
    if (IntRead >= Integer(Low(TMagicActionType))) and (IntRead <= Integer(High(TMagicActionType))) then
      ClientBaseConfig.MagicActionType := TMagicActionType(IntRead);

    ClientBaseConfig.MagicActionStartIndex := IniFile.ReadInteger(SectionName, 'MagicActionStartIndex',
      ClientBaseConfig.MagicActionStartIndex);
    ClientBaseConfig.MagicActionPlayCount := IniFile.ReadInteger(SectionName, 'MagicActionPlayCount',
      ClientBaseConfig.MagicActionPlayCount);
    ClientBaseConfig.MagicActionEmptyCount := IniFile.ReadInteger(SectionName, 'MagicActionEmptyCount',
      ClientBaseConfig.MagicActionEmptyCount);
    ClientBaseConfig.MagicActionContinue := IniFile.ReadBool(SectionName, 'MagicActionContinue',
      ClientBaseConfig.MagicActionContinue);
    ClientBaseConfig.MagicAutoOpen := IniFile.ReadBool(SectionName, 'MagicAutoOpen', ClientBaseConfig.MagicAutoOpen);
    ClientBaseConfig.NotRaiseHand := IniFile.ReadBool(SectionName, 'NotRaiseHand', ClientBaseConfig.NotRaiseHand);

    IntRead := IniFile.ReadInteger(SectionName, 'MagicWarrNGOption', Integer(ClientBaseConfig.MagicWarrNGOption));
    if (IntRead >= Integer(Low(TMagicWarrNGOption))) and (IntRead <= Integer(High(TMagicWarrNGOption))) then
      ClientBaseConfig.MagicWarrNGOption := TMagicWarrNGOption(IntRead);

    for MagicPlusLevel := Low(ClientConfigs) to High(ClientConfigs) do
    begin
      MagicClientConfig := @ClientConfigs[MagicPlusLevel];
      SectionName := 'ClientAttack' + IntToStr(Integer(MagicPlusLevel));

      MagicClientConfig.Icon_File := IniFile.ReadInteger(SectionName, 'Icon_File', MagicClientConfig.Icon_File);
      MagicClientConfig.Icon_Index := IniFile.ReadInteger(SectionName, 'Icon_Index', MagicClientConfig.Icon_Index);

      for SoundType := Low(TMagicSoundType) to High(TMagicSoundType) do
        MagicClientConfig.Sounds[SoundType] := IniFile.ReadString(SectionName, MagicSoundTypeNames[SoundType], '');

      MagicClientConfig.Fly_File := IniFile.ReadInteger(SectionName, 'Fly_File', MagicClientConfig.Fly_File);
      MagicClientConfig.Fly_StartIndex := IniFile.ReadInteger(SectionName, 'Fly_StartIndex', MagicClientConfig.Fly_StartIndex);
      MagicClientConfig.Fly_PlayCount := IniFile.ReadInteger(SectionName, 'Fly_PlayCount', MagicClientConfig.Fly_PlayCount);
      MagicClientConfig.Fly_EmptyCount := IniFile.ReadInteger(SectionName, 'Fly_EmptyCount', MagicClientConfig.Fly_EmptyCount);
      MagicClientConfig.Fly_PlayTime := IniFile.ReadInteger(SectionName, 'Fly_PlayTime', MagicClientConfig.Fly_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'Fly_DrawMode', Integer(MagicClientConfig.Fly_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.Fly_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Fly_DirCount', Integer(MagicClientConfig.Fly_DirCount));
      if (IntRead >= Integer(Low(TCustomDirCount))) and (IntRead <= Integer(High(TCustomDirCount))) then
        MagicClientConfig.Fly_DirCount := TCustomDirCount(IntRead);

      MagicClientConfig.Fly_CalcDir := IniFile.ReadBool(SectionName, 'Fly_CalcDir', MagicClientConfig.Fly_CalcDir);
      MagicClientConfig.Fly_FireGunMode := IniFile.ReadBool(SectionName, 'Fly_FireGunMode', MagicClientConfig.Fly_FireGunMode);
      MagicClientConfig.Fly_LightRange := IniFile.ReadInteger(SectionName, 'Fly_LightRange', MagicClientConfig.Fly_LightRange);

      MagicClientConfig.FlyEff_File := IniFile.ReadInteger(SectionName, 'FlyEff_File', MagicClientConfig.FlyEff_File);
      MagicClientConfig.FlyEff_StartIndex := IniFile.ReadInteger(SectionName, 'FlyEff_StartIndex',
        MagicClientConfig.FlyEff_StartIndex);
      IntRead := IniFile.ReadInteger(SectionName, 'FlyEff_DrawMode', Integer(MagicClientConfig.FlyEff_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.FlyEff_DrawMode := TCustomDrawMode(IntRead);

      MagicClientConfig.Self_File := IniFile.ReadInteger(SectionName, 'Self_File', MagicClientConfig.Self_File);
      MagicClientConfig.Self_StartIndex := IniFile.ReadInteger(SectionName, 'Self_StartIndex', MagicClientConfig.Self_StartIndex);
      MagicClientConfig.Self_SyncHumAction := IniFile.ReadBool(SectionName, 'Self_SyncHumAction',
        MagicClientConfig.Self_SyncHumAction);
      MagicClientConfig.Self_PlayCount := IniFile.ReadInteger(SectionName, 'Self_PlayCount', MagicClientConfig.Self_PlayCount);
      MagicClientConfig.Self_EmptyCount := IniFile.ReadInteger(SectionName, 'Self_EmptyCount', MagicClientConfig.Self_EmptyCount);
      MagicClientConfig.Self_PlayTime := IniFile.ReadInteger(SectionName, 'Self_PlayTime', MagicClientConfig.Self_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DrawOrder', Integer(MagicClientConfig.Self_DrawOrder));
      if (IntRead >= Integer(Low(TCustomDrawOrder))) and (IntRead <= Integer(High(TCustomDrawOrder))) then
        MagicClientConfig.Self_DrawOrder := TCustomDrawOrder(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DrawMode', Integer(MagicClientConfig.Self_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.Self_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DirCalcType', Integer(MagicClientConfig.Self_DirCalcType));
      if (IntRead >= Integer(Low(TCustomDirCalcType))) and (IntRead <= Integer(High(TCustomDirCalcType))) then
        MagicClientConfig.Self_DirCalcType := TCustomDirCalcType(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DirCount', Integer(MagicClientConfig.Self_DirCount));
      if (IntRead >= Integer(Low(TCustomDirCount))) and (IntRead <= Integer(High(TCustomDirCount))) then
        MagicClientConfig.Self_DirCount := TCustomDirCount(IntRead);
      MagicClientConfig.Self_PlayDelayAction := IniFile.ReadBool(SectionName, 'Self_PlayDelayAction',
        MagicClientConfig.Self_PlayDelayAction);
      MagicClientConfig.Self_LightRange := IniFile.ReadInteger(SectionName, 'Self_LightRange', MagicClientConfig.Self_LightRange);
      MagicClientConfig.Self_PlayFailNoDraw := IniFile.ReadBool(SectionName, 'Self_PlayFailNoDraw',
        MagicClientConfig.Self_PlayFailNoDraw);

      MagicClientConfig.SelfKeep_File := IniFile.ReadInteger(SectionName, 'SelfKeep_File', MagicClientConfig.SelfKeep_File);
      MagicClientConfig.SelfKeep_StartIndex := IniFile.ReadInteger(SectionName, 'SelfKeep_StartIndex',
        MagicClientConfig.SelfKeep_StartIndex);
      MagicClientConfig.SelfKeep_PlayCount := IniFile.ReadInteger(SectionName, 'SelfKeep_PlayCount',
        MagicClientConfig.SelfKeep_PlayCount);
      MagicClientConfig.SelfKeep_PlayTime := IniFile.ReadInteger(SectionName, 'SelfKeep_PlayTime',
        MagicClientConfig.SelfKeep_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'SelfKeep_DrawMode', Integer(MagicClientConfig.SelfKeep_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.SelfKeep_DrawMode := TCustomDrawMode(IntRead);

      MagicClientConfig.SelfKeep_KeepTime := IniFile.ReadInteger(SectionName, 'SelfKeep_KeepTime',
        MagicClientConfig.SelfKeep_KeepTime);
      MagicClientConfig.SelfKeep_KeepTime2 := IniFile.ReadInteger(SectionName, 'SelfKeep_KeepTime2',
        MagicClientConfig.SelfKeep_KeepTime2);

      MagicClientConfig.FastMove_File := IniFile.ReadInteger(SectionName, 'FastMove_File', MagicClientConfig.FastMove_File);
      MagicClientConfig.FastMove_StartIndex := IniFile.ReadInteger(SectionName, 'FastMove_StartIndex',
        MagicClientConfig.FastMove_StartIndex);
      MagicClientConfig.FastMove_PlayCount := IniFile.ReadInteger(SectionName, 'FastMove_PlayCount',
        MagicClientConfig.FastMove_PlayCount);
      MagicClientConfig.FastMove_EmptyCount := IniFile.ReadInteger(SectionName, 'FastMove_EmptyCount',
        MagicClientConfig.FastMove_EmptyCount);
      MagicClientConfig.FastMove_PlayTime := IniFile.ReadInteger(SectionName, 'FastMove_PlayTime',
        MagicClientConfig.FastMove_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'FastMove_DrawOrder', Integer(MagicClientConfig.FastMove_DrawOrder));
      if (IntRead >= Integer(Low(TCustomDrawOrder))) and (IntRead <= Integer(High(TCustomDrawOrder))) then
        MagicClientConfig.FastMove_DrawOrder := TCustomDrawOrder(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'FastMove_DrawMode', Integer(MagicClientConfig.FastMove_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.FastMove_DrawMode := TCustomDrawMode(IntRead);

      MagicClientConfig.FastMove_CalcDir := IniFile.ReadBool(SectionName, 'FastMove_CalcDir', MagicClientConfig.FastMove_CalcDir);
      MagicClientConfig.FastMove_NoHitAction := IniFile.ReadBool(SectionName, 'FastMove_NoHitAction',
        MagicClientConfig.FastMove_NoHitAction);
      MagicClientConfig.FastMove_LightRange := IniFile.ReadInteger(SectionName, 'FastMove_LightRange',
        MagicClientConfig.FastMove_LightRange);

      MagicClientConfig.PreTarget_File := IniFile.ReadInteger(SectionName, 'PreTarget_File', MagicClientConfig.PreTarget_File);
      MagicClientConfig.PreTarget_StartIndex := IniFile.ReadInteger(SectionName, 'PreTarget_StartIndex',
        MagicClientConfig.PreTarget_StartIndex);
      MagicClientConfig.PreTarget_StartIndex2 := IniFile.ReadInteger(SectionName, 'PreTarget_StartIndex2',
        MagicClientConfig.PreTarget_StartIndex2);
      MagicClientConfig.PreTarget_PlayCount := IniFile.ReadInteger(SectionName, 'PreTarget_PlayCount',
        MagicClientConfig.PreTarget_PlayCount);
      MagicClientConfig.PreTarget_EmptyCount := IniFile.ReadInteger(SectionName, 'PreTarget_EmptyCount',
        MagicClientConfig.PreTarget_EmptyCount);
      MagicClientConfig.PreTarget_PlayTime := IniFile.ReadInteger(SectionName, 'PreTarget_PlayTime',
        MagicClientConfig.PreTarget_PlayTime);
      IntRead := IniFile.ReadInteger(SectionName, 'PreTarget_DrawMode', Integer(MagicClientConfig.PreTarget_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.PreTarget_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'PreTarget_DrawMode2', Integer(MagicClientConfig.PreTarget_DrawMode2));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.PreTarget_DrawMode2 := TCustomDrawMode(IntRead);

      MagicClientConfig.PreTarget_CalcDir := IniFile.ReadBool(SectionName, 'PreTarget_CalcDir',
        MagicClientConfig.PreTarget_CalcDir);
      MagicClientConfig.PreTarget_LockDraw := IniFile.ReadBool(SectionName, 'PreTarget_LockDraw',
        MagicClientConfig.PreTarget_LockDraw);
      MagicClientConfig.PreTarget_LightRange := IniFile.ReadInteger(SectionName, 'PreTarget_LightRange',
        MagicClientConfig.PreTarget_LightRange);

      MagicClientConfig.Target_File := IniFile.ReadInteger(SectionName, 'Target_File', MagicClientConfig.Target_File);
      MagicClientConfig.Target_StartIndex := IniFile.ReadInteger(SectionName, 'Target_StartIndex',
        MagicClientConfig.Target_StartIndex);
      MagicClientConfig.Target_StartIndex2 := IniFile.ReadInteger(SectionName, 'Target_StartIndex2',
        MagicClientConfig.Target_StartIndex2);
      MagicClientConfig.Target_PlayCount := IniFile.ReadInteger(SectionName, 'Target_PlayCount',
        MagicClientConfig.Target_PlayCount);
      MagicClientConfig.Target_PlayTime := IniFile.ReadInteger(SectionName, 'Target_PlayTime', MagicClientConfig.Target_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'Target_DrawMode', Integer(MagicClientConfig.Target_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.Target_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Target_DrawMode2', Integer(MagicClientConfig.Target_DrawMode2));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.Target_DrawMode2 := TCustomDrawMode(IntRead);

      MagicClientConfig.Target_MultiPlay := IniFile.ReadBool(SectionName, 'Target_MultiPlay', MagicClientConfig.Target_MultiPlay);
      MagicClientConfig.Target_LockDraw := IniFile.ReadBool(SectionName, 'Target_LockDraw', MagicClientConfig.Target_LockDraw);
      MagicClientConfig.Target_LightRange := IniFile.ReadInteger(SectionName, 'Target_LightRange',
        MagicClientConfig.Target_LightRange);

      MagicClientConfig.Target_KeepPlay := IniFile.ReadBool(SectionName, 'Target_KeepPlay', MagicClientConfig.Target_KeepPlay);
      MagicClientConfig.Target_KeepTime := IniFile.ReadInteger(SectionName, 'Target_KeepTime', MagicClientConfig.Target_KeepTime);
      MagicClientConfig.Target_KeepTime2 := IniFile.ReadInteger(SectionName, 'Target_KeepTime2',
        MagicClientConfig.Target_KeepTime2);
      MagicClientConfig.Target_KeepAttackRange := IniFile.ReadInteger(SectionName, 'Target_KeepAttackRange',
        MagicClientConfig.Target_KeepAttackRange);
      MagicClientConfig.Target_KeepMultiPlay := IniFile.ReadBool(SectionName, 'Target_KeepMultiPlay',
        MagicClientConfig.Target_KeepMultiPlay);
      MagicClientConfig.Target_KeepAttackInterval := IniFile.ReadInteger(SectionName, 'Target_KeepAttackInterval',
        MagicClientConfig.Target_KeepAttackInterval);
      MagicClientConfig.Target_KeepLightRange := IniFile.ReadInteger(SectionName, 'Target_KeepLightRange',
        MagicClientConfig.Target_KeepLightRange);

      MagicClientConfig.TargetStatus1_File := IniFile.ReadInteger(SectionName, 'TargetStatus1_File',
        MagicClientConfig.TargetStatus1_File);
      MagicClientConfig.TargetStatus1_StartIndex := IniFile.ReadInteger(SectionName, 'TargetStatus1_StartIndex',
        MagicClientConfig.TargetStatus1_StartIndex);
      MagicClientConfig.TargetStatus1_PlayCount := IniFile.ReadInteger(SectionName, 'TargetStatus1_PlayCount',
        MagicClientConfig.TargetStatus1_PlayCount);
      MagicClientConfig.TargetStatus1_EmptyCount := IniFile.ReadInteger(SectionName, 'TargetStatus1_EmptyCount',
        MagicClientConfig.TargetStatus1_EmptyCount);
      // MagicClientConfig.TargetStatus1_PlayTime := IniFile.ReadInteger(SectionName, 'TargetStatus1_PlayTime', MagicClientConfig.TargetStatus1_PlayTime);
      IntRead := IniFile.ReadInteger(SectionName, 'TargetStatus1_DrawMode', Integer(MagicClientConfig.TargetStatus1_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.TargetStatus1_DrawMode := TCustomDrawMode(IntRead);
      MagicClientConfig.TargetStatus1_CalcDir := IniFile.ReadBool(SectionName, 'TargetStatus1_CalcDir',
        MagicClientConfig.TargetStatus1_CalcDir);

      MagicClientConfig.TargetStatus2_File := IniFile.ReadInteger(SectionName, 'TargetStatus2_File',
        MagicClientConfig.TargetStatus2_File);
      MagicClientConfig.TargetStatus2_StartIndex := IniFile.ReadInteger(SectionName, 'TargetStatus2_StartIndex',
        MagicClientConfig.TargetStatus2_StartIndex);
      MagicClientConfig.TargetStatus2_PlayCount := IniFile.ReadInteger(SectionName, 'TargetStatus2_PlayCount',
        MagicClientConfig.TargetStatus2_PlayCount);
      MagicClientConfig.TargetStatus2_EmptyCount := IniFile.ReadInteger(SectionName, 'TargetStatus2_EmptyCount',
        MagicClientConfig.TargetStatus2_EmptyCount);
      // MagicClientConfig.TargetStatus2_PlayTime := IniFile.ReadInteger(SectionName, 'TargetStatus2_PlayTime', MagicClientConfig.TargetStatus2_PlayTime);
      IntRead := IniFile.ReadInteger(SectionName, 'TargetStatus2_DrawMode', Integer(MagicClientConfig.TargetStatus2_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        MagicClientConfig.TargetStatus2_DrawMode := TCustomDrawMode(IntRead);
      MagicClientConfig.TargetStatus2_CalcDir := IniFile.ReadBool(SectionName, 'TargetStatus2_CalcDir',
        MagicClientConfig.TargetStatus2_CalcDir);
    end;

    SectionName := 'ServerConfig';
    IntRead := IniFile.ReadInteger(SectionName, 'OperateMode', Integer(ServerConfig.OperateMode));
    if (IntRead >= Integer(Low(TCustomOperateMode))) and (IntRead <= Integer(High(TCustomOperateMode))) then
      ServerConfig.OperateMode := TCustomOperateMode(IntRead);

    ServerConfig.IsAttackUseNG := IniFile.ReadBool(SectionName, 'IsAttackUseNG', ServerConfig.IsAttackUseNG);
    ServerConfig.NoChangeDir := IniFile.ReadBool(SectionName, 'NoChangeDir', ServerConfig.NoChangeDir);
    ServerConfig.DisableInSafeZone := IniFile.ReadBool(SectionName, 'DisableInSafeZone', ServerConfig.DisableInSafeZone);
    ServerConfig.AttackDelayTime := IniFile.ReadInteger(SectionName, 'AttackDelayTime', ServerConfig.AttackDelayTime);
    ServerConfig.UseInterval := IniFile.ReadInteger(SectionName, 'UseInterval', ServerConfig.UseInterval);

    // ServerConfig.FailNoShowEff := IniFile.ReadBool(SectionName, 'FailNoShowEff', ServerConfig.FailNoShowEff);
    ServerConfig.FailMsg := IniFile.ReadString(SectionName, 'FailMsg', ServerConfig.FailMsg);
    ServerConfig.SucceedMsg := IniFile.ReadString(SectionName, 'SucceedMsg', ServerConfig.SucceedMsg);
    ServerConfig.CloseMsg := IniFile.ReadString(SectionName, 'CloseMsg', ServerConfig.CloseMsg);

    ServerConfig.IsCheckVarValue := IniFile.ReadBool(SectionName, 'IsCheckVarValue', ServerConfig.IsCheckVarValue);

    IntRead := IniFile.ReadInteger(SectionName, 'NeedItem', Integer(ServerConfig.NeedItem));
    if (IntRead >= Integer(Low(TMagicNeedItem))) and (IntRead <= Integer(High(TMagicNeedItem))) then
      ServerConfig.NeedItem := TMagicNeedItem(IntRead);

    ServerConfig.NeedItemCount := IniFile.ReadInteger(SectionName, 'NeedItemCount', ServerConfig.NeedItemCount);
    ServerConfig.NeedItemCustomItemName := IniFile.ReadString(SectionName, 'NeedItemCustomItemName',
      ServerConfig.NeedItemCustomItemName);
    ServerConfig.NeedItemUseBagItem := IniFile.ReadBool(SectionName, 'NeedItemUseBagItem', ServerConfig.NeedItemUseBagItem);

    ServerConfig.CheckVarName := IniFile.ReadString(SectionName, 'CheckVarName', ServerConfig.CheckVarName);
    IntRead := IniFile.ReadInteger(SectionName, 'CheckVarType', Integer(ServerConfig.CheckVarType));
    if (IntRead >= Integer(Low(TCheckVarType))) and (IntRead <= Integer(High(TCheckVarType))) then
      ServerConfig.CheckVarType := TCheckVarType(IntRead);
    ServerConfig.CheckVarValue := IniFile.ReadInteger(SectionName, 'CheckVarValue', ServerConfig.CheckVarValue);
    ServerConfig.CheckVarAdd := IniFile.ReadInteger(SectionName, 'CheckVarAdd', ServerConfig.CheckVarAdd);

    IntRead := IniFile.ReadInteger(SectionName, 'AttackMode', Integer(ServerConfig.AttackMode));
    if (IntRead >= Integer(Low(TCustomAttackMode))) and (IntRead <= Integer(High(TCustomAttackMode))) then
      ServerConfig.AttackMode := TCustomAttackMode(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'AttackTarget', Integer(ServerConfig.AttackTarget));
    if (IntRead >= Integer(Low(TCustomAttackTarget))) and (IntRead <= Integer(High(TCustomAttackTarget))) then
      ServerConfig.AttackTarget := TCustomAttackTarget(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'AttackPowerCalc', Integer(ServerConfig.AttackPowerCalc));
    if (IntRead >= Integer(Low(TCustomAttackPowerCalc))) and (IntRead <= Integer(High(TCustomAttackPowerCalc))) then
      ServerConfig.AttackPowerCalc := TCustomAttackPowerCalc(IntRead);

    for I := Low(ServerConfig.AttackPowerRates) to High(ServerConfig.AttackPowerRates) do
    begin
      ServerConfig.AttackPowerRates[I] := IniFile.ReadInteger(SectionName, 'AttackPowerRate_' + IntToStr(I),
        ServerConfig.AttackPowerRates[I]);
    end;
    ServerConfig.AttackPowerLineAdd := IniFile.ReadInteger(SectionName, 'AttackPowerLineAdd', ServerConfig.AttackPowerLineAdd);
    ServerConfig.AttackPowerUndeadAdd := IniFile.ReadInteger(SectionName, 'AttackPowerUndeadAdd',
      ServerConfig.AttackPowerUndeadAdd);

    ServerConfig.AttackNearRange := IniFile.ReadInteger(SectionName, 'AttackNearRange', ServerConfig.AttackNearRange);
    ServerConfig.AttackGroupRange := IniFile.ReadInteger(SectionName, 'AttackGroupRange', ServerConfig.AttackGroupRange);
    ServerConfig.AttackLineWidth := IniFile.ReadInteger(SectionName, 'AttackLineWidth', ServerConfig.AttackLineWidth);
    ServerConfig.EnableAntiMagic := IniFile.ReadBool(SectionName, 'EnableAntiMagic', ServerConfig.EnableAntiMagic);
    ServerConfig.EnableHitPoint := IniFile.ReadBool(SectionName, 'EnableHitPoint', ServerConfig.EnableHitPoint);

    ServerConfig.AttackTeleportAttack := IniFile.ReadBool(SectionName, 'AttackTeleportAttack', ServerConfig.AttackTeleportAttack);
    ServerConfig.IsNoTeleportNoAttack := IniFile.ReadBool(SectionName, 'IsNoTeleportNoAttack', ServerConfig.IsNoTeleportNoAttack);
    ServerConfig.AttackTeleportRate := IniFile.ReadInteger(SectionName, 'AttackTeleportRate', ServerConfig.AttackTeleportRate);
    ServerConfig.AttackTeleportRunHum := IniFile.ReadBool(SectionName, 'AttackTeleportRunHum', ServerConfig.AttackTeleportRunHum);
    ServerConfig.AttackTeleportRunMon := IniFile.ReadBool(SectionName, 'AttackTeleportRunMon', ServerConfig.AttackTeleportRunMon);
    ServerConfig.AttackTeleportRunNpc := IniFile.ReadBool(SectionName, 'AttackTeleportRunNpc', ServerConfig.AttackTeleportRunNpc);
    ServerConfig.AttackTeleportRunGuard := IniFile.ReadBool(SectionName, 'AttackTeleportRunGuard',
      ServerConfig.AttackTeleportRunGuard);
    ServerConfig.AttackTeleportRunObstacle := IniFile.ReadBool(SectionName, 'AttackTeleportRunObstacle',
      ServerConfig.AttackTeleportRunObstacle);
    ServerConfig.AttackTeleportWarDisHumRun := IniFile.ReadBool(SectionName, 'AttackTeleportWarDisHumRun',
      ServerConfig.AttackTeleportWarDisHumRun);
    ServerConfig.AttackTeleportCannotRunItem := IniFile.ReadBool(SectionName, 'AttackTeleportCannotRunItem',
      ServerConfig.AttackTeleportCannotRunItem);
    ServerConfig.AttackTeleportRush := IniFile.ReadBool(SectionName, 'AttackTeleportRush', ServerConfig.AttackTeleportRush);
    ServerConfig.AttackTeleportRushCount := IniFile.ReadInteger(SectionName, 'AttackTeleportRushCount',
      ServerConfig.AttackTeleportRushCount);
    ServerConfig.AttackTeleportAfterDamage := IniFile.ReadBool(SectionName, 'AttackTeleportAfterDamage',
      ServerConfig.AttackTeleportAfterDamage);

    SectionName := 'CallMonster';
    ServerConfig.EnabledCallMonster := IniFile.ReadBool(SectionName, 'EnabledCallMonster', ServerConfig.EnabledCallMonster);
    ServerConfig.CallMonstersRate := IniFile.ReadInteger(SectionName, 'CallMonstersRate', ServerConfig.CallMonstersRate);
    ServerConfig.CallMonstersRoyaltySec := IniFile.ReadInteger(SectionName, 'CallMonstersRoyaltySec',
      ServerConfig.CallMonstersRoyaltySec);
    ServerConfig.CallMonstersLevel := IniFile.ReadInteger(SectionName, 'CallMonstersLevel', ServerConfig.CallMonstersLevel);
    for I := Low(ServerConfig.CallMonsters) to High(ServerConfig.CallMonsters) do
    begin
      ServerConfig.CallMonsters[I] := IniFile.ReadString(SectionName, 'MonsterName' + IntToStr(I), ServerConfig.CallMonsters[I]);
      ServerConfig.CallMonsterNums[I] := IniFile.ReadInteger(SectionName, 'MonsterNum' + IntToStr(I),
        ServerConfig.CallMonsterNums[I]);;
    end;

    SectionName := 'Additionals';
    for I := Low(ServerConfig.Additionals) to High(ServerConfig.Additionals) do
    begin
      ServerConfig.Additionals[I].Checked := IniFile.ReadBool(SectionName, 'Checked' + IntToStr(I),
        ServerConfig.Additionals[I].Checked);
      ServerConfig.Additionals[I].Rate := IniFile.ReadInteger(SectionName, 'Rate' + IntToStr(I),
        ServerConfig.Additionals[I].Rate);
      ServerConfig.Additionals[I].Rate2 := IniFile.ReadInteger(SectionName, 'Rate2' + IntToStr(I),
        ServerConfig.Additionals[I].Rate2);
      ServerConfig.Additionals[I].Time := IniFile.ReadInteger(SectionName, 'Time' + IntToStr(I),
        ServerConfig.Additionals[I].Time);
      ServerConfig.Additionals[I].TimeUnit := IniFile.ReadInteger(SectionName, 'TimeUnit' + IntToStr(I),
        ServerConfig.Additionals[I].TimeUnit);
      ServerConfig.Additionals[I].Time2 := IniFile.ReadInteger(SectionName, 'Time2' + IntToStr(I),
        ServerConfig.Additionals[I].Time2);
    end;
    ServerConfig.AdditionalHP0 := IniFile.ReadInteger(SectionName, 'HP0', ServerConfig.AdditionalHP0);
    ServerConfig.AdditionalHighLevel4 := IniFile.ReadBool(SectionName, 'HighLevel4', ServerConfig.AdditionalHighLevel4);
    ServerConfig.AdditionalPushedType4 := IniFile.ReadInteger(SectionName, 'PushedType4', ServerConfig.AdditionalPushedType4);

    ServerConfig.AttackTargetStatus := IniFile.ReadBool(SectionName, 'AttackTargetStatus', ServerConfig.AttackTargetStatus);
    ServerConfig.AttackTargetStatusTime := IniFile.ReadInteger(SectionName, 'AttackTargetStatusTime',
      ServerConfig.AttackTargetStatusTime);
    ServerConfig.AttackTargetStatusTimeUnit := IniFile.ReadInteger(SectionName, 'AttackTargetStatusTimeUnit',
      ServerConfig.AttackTargetStatusTimeUnit);
    ServerConfig.AttackTargetStatusTime2 := IniFile.ReadInteger(SectionName, 'AttackTargetStatusTime2',
      ServerConfig.AttackTargetStatusTime2);
    ServerConfig.AttackTargetStatusDelay := IniFile.ReadInteger(SectionName, 'AttackTargetStatusDelay',
      ServerConfig.AttackTargetStatusDelay);

    for DecAttribType := Low(TMagicAttackDecAttributesType) to High(TMagicAttackDecAttributesType) do
    begin
      sTemp := MagicAttackDecAttributesTypeIniNames[DecAttribType];

      ServerConfig.AttackSubAttrib[DecAttribType].IsChecked := IniFile.ReadBool(SectionName, Format('AttackSub%s', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].IsChecked);
      ServerConfig.AttackSubAttrib[DecAttribType].Rate := IniFile.ReadInteger(SectionName, Format('AttackSub%sRate', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].Rate);
      ServerConfig.AttackSubAttrib[DecAttribType].RateAdd := IniFile.ReadInteger(SectionName, Format('AttackSub%sRate2', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].RateAdd);

      ServerConfig.AttackSubAttrib[DecAttribType].LowValue :=
        IniFile.ReadInteger(SectionName, Format('AttackSub%s1Percent', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].LowValue);
      ServerConfig.AttackSubAttrib[DecAttribType].LowValueIsPoint :=
        IniFile.ReadBool(SectionName, Format('AttackSub%s1Unit', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].LowValueIsPoint);
      ServerConfig.AttackSubAttrib[DecAttribType].LowValueAdd :=
        IniFile.ReadInteger(SectionName, Format('AttackSub%s1Percent2', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].LowValueAdd);

      ServerConfig.AttackSubAttrib[DecAttribType].HighValue :=
        IniFile.ReadInteger(SectionName, Format('AttackSub%s2Percent', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HighValue);
      ServerConfig.AttackSubAttrib[DecAttribType].HighValueIsPoint :=
        IniFile.ReadBool(SectionName, Format('AttackSub%s2Unit', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HighValueIsPoint);
      ServerConfig.AttackSubAttrib[DecAttribType].HighValueAdd :=
        IniFile.ReadInteger(SectionName, Format('AttackSub%s2Percent2', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HighValueAdd);

      ServerConfig.AttackSubAttrib[DecAttribType].Time := IniFile.ReadInteger(SectionName, Format('AttackSub%sTime', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].Time);
      ServerConfig.AttackSubAttrib[DecAttribType].TimeAdd := IniFile.ReadInteger(SectionName, Format('AttackSub%sTime2', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].TimeAdd);
      ServerConfig.AttackSubAttrib[DecAttribType].TimeAddIsPoint :=
        IniFile.ReadBool(SectionName, Format('AttackSub%sTime2Unit', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].TimeAddIsPoint);

      ServerConfig.AttackSubAttrib[DecAttribType].ShowHint :=
        IniFile.ReadBool(SectionName, Format('AttackSub%sShowHint', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].ShowHint);
      ServerConfig.AttackSubAttrib[DecAttribType].HintText :=
        IniFile.ReadString(SectionName, Format('AttackSub%sHintText', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HintText);
    end;

    for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
    begin
      sTemp := ItemElementsTypeIniNames[ElementType];

      ServerConfig.AttackSubElements[ElementType].IsChecked := IniFile.ReadBool(SectionName, Format('AttackSub_E_%s', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].IsChecked);
      ServerConfig.AttackSubElements[ElementType].Rate := IniFile.ReadInteger(SectionName, Format('AttackSub_E_%sRate', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].Rate);
      ServerConfig.AttackSubElements[ElementType].RateAdd :=
        IniFile.ReadInteger(SectionName, Format('AttackSub_E_%sRate2', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].RateAdd);
      ServerConfig.AttackSubElements[ElementType].Value :=
        IniFile.ReadInteger(SectionName, Format('AttackSub_E_%sPercent', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].Value);
      ServerConfig.AttackSubElements[ElementType].ValueIsPoint :=
        IniFile.ReadBool(SectionName, Format('AttackSub_E_%sUnit', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].ValueIsPoint);
      ServerConfig.AttackSubElements[ElementType].ValueAdd :=
        IniFile.ReadInteger(SectionName, Format('AttackSub_E_%sPercent2', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].ValueAdd);
      ServerConfig.AttackSubElements[ElementType].Time := IniFile.ReadInteger(SectionName, Format('AttackSub_E_%sTime', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].Time);
      ServerConfig.AttackSubElements[ElementType].TimeAdd :=
        IniFile.ReadInteger(SectionName, Format('AttackSub_E_%sTime2', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].TimeAdd);
      ServerConfig.AttackSubElements[ElementType].TimeAddIsPoint :=
        IniFile.ReadBool(SectionName, Format('AttackSub_E_%sTime2Unit', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].TimeAddIsPoint);
      ServerConfig.AttackSubElements[ElementType].ShowHint :=
        IniFile.ReadBool(SectionName, Format('AttackSub_E_%sShowHint', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].ShowHint);
      ServerConfig.AttackSubElements[ElementType].HintText :=
        IniFile.ReadString(SectionName, Format('AttackSub_E_%sHintText', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].HintText);
    end;

    for BreakDefenseType := Low(TBreakDefenseType) to High(TBreakDefenseType) do
    begin
      sTemp := BreakDefenseTypeNames[BreakDefenseType];

      ServerConfig.AttackBreakDefense[BreakDefenseType].IsChecked := IniFile.ReadBool(SectionName, Format('%s', [sTemp]),
        ServerConfig.AttackBreakDefense[BreakDefenseType].IsChecked);
      ServerConfig.AttackBreakDefense[BreakDefenseType].Rate := IniFile.ReadInteger(SectionName, Format('%s_Rate', [sTemp]),
        ServerConfig.AttackBreakDefense[BreakDefenseType].Rate);
      ServerConfig.AttackBreakDefense[BreakDefenseType].RateAdd := IniFile.ReadInteger(SectionName, Format('%s_RateAdd', [sTemp]),
        ServerConfig.AttackBreakDefense[BreakDefenseType].RateAdd);
      ServerConfig.AttackBreakDefense[BreakDefenseType].Value := IniFile.ReadInteger(SectionName, Format('%s_Value', [sTemp]),
        ServerConfig.AttackBreakDefense[BreakDefenseType].Value);
      ServerConfig.AttackBreakDefense[BreakDefenseType].ValueAdd :=
        IniFile.ReadInteger(SectionName, Format('%s_ValueAdd', [sTemp]), ServerConfig.AttackBreakDefense[BreakDefenseType]
        .ValueAdd);
    end;

    SectionName := 'Protect';
    for IncAttribType := Low(TMagicProtectAddAttributesType) to High(TMagicProtectAddAttributesType) do
    begin
      sTemp := MagicProtectAddAttributesTypeIniNames[IncAttribType];

      ServerConfig.ProtectAddAttrib[IncAttribType].IsChecked := IniFile.ReadBool(SectionName, Format('ProtectAdd%s', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].IsChecked);
      ServerConfig.ProtectAddAttrib[IncAttribType].Rate := IniFile.ReadInteger(SectionName, Format('ProtectAdd%sRate', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].Rate);
      ServerConfig.ProtectAddAttrib[IncAttribType].RateAdd :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd%sRate2', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].RateAdd);
      ServerConfig.ProtectAddAttrib[IncAttribType].LowValue :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd%s1Percent', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].LowValue);
      ServerConfig.ProtectAddAttrib[IncAttribType].LowValueIsPoint :=
        IniFile.ReadBool(SectionName, Format('ProtectAdd%s1Unit', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].LowValueIsPoint);
      ServerConfig.ProtectAddAttrib[IncAttribType].LowValueAdd :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd%s1Percent2', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].LowValueAdd);

      // 为了兼容以前的配置文件
      if IncAttribType = aaHP then
      begin
        ServerConfig.ProtectAddAttrib[IncAttribType].HighValue := IniFile.ReadInteger(SectionName, 'ProtectAddHPPercent',
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValue);
        ServerConfig.ProtectAddAttrib[IncAttribType].HighValueIsPoint := IniFile.ReadBool(SectionName, 'ProtectAddHPUnit',
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValueIsPoint);
        ServerConfig.ProtectAddAttrib[IncAttribType].HighValueAdd := IniFile.ReadInteger(SectionName, 'ProtectAddHPPercent2',
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValueAdd);
      end
      else
      begin
        ServerConfig.ProtectAddAttrib[IncAttribType].HighValue :=
          IniFile.ReadInteger(SectionName, Format('ProtectAdd%s2Percent', [sTemp]),
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValue);
        ServerConfig.ProtectAddAttrib[IncAttribType].HighValueIsPoint :=
          IniFile.ReadBool(SectionName, Format('ProtectAdd%s2Unit', [sTemp]),
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValueIsPoint);
        ServerConfig.ProtectAddAttrib[IncAttribType].HighValueAdd :=
          IniFile.ReadInteger(SectionName, Format('ProtectAdd%s2Percent2', [sTemp]),
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValueAdd);
      end;

      ServerConfig.ProtectAddAttrib[IncAttribType].Time := IniFile.ReadInteger(SectionName, Format('ProtectAdd%sTime', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].Time);
      ServerConfig.ProtectAddAttrib[IncAttribType].TimeAdd :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd%sTime2', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].TimeAdd);
      ServerConfig.ProtectAddAttrib[IncAttribType].TimeAddIsPoint :=
        IniFile.ReadBool(SectionName, Format('ProtectAdd%sTime2Unit', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].TimeAddIsPoint);
      ServerConfig.ProtectAddAttrib[IncAttribType].ShowHint :=
        IniFile.ReadBool(SectionName, Format('ProtectAdd%sShowHint', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].ShowHint);
      ServerConfig.ProtectAddAttrib[IncAttribType].HintText :=
        IniFile.ReadString(SectionName, Format('ProtectAdd%sHintText', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].HintText);
    end;

    for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
    begin
      sTemp := ItemElementsTypeIniNames[ElementType];

      ServerConfig.ProtectAddElements[ElementType].IsChecked := IniFile.ReadBool(SectionName, Format('ProtectAdd_E_%s', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].IsChecked);
      ServerConfig.ProtectAddElements[ElementType].Rate :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd_E_%sRate', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].Rate);
      ServerConfig.ProtectAddElements[ElementType].RateAdd :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd_E_%sRate2', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].RateAdd);
      ServerConfig.ProtectAddElements[ElementType].Value :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd_E_%sPercent', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].Value);
      ServerConfig.ProtectAddElements[ElementType].ValueIsPoint :=
        IniFile.ReadBool(SectionName, Format('ProtectAdd_E_%sUnit', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].ValueIsPoint);
      ServerConfig.ProtectAddElements[ElementType].ValueAdd :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd_E_%sPercent2', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].ValueAdd);
      ServerConfig.ProtectAddElements[ElementType].Time :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd_E_%sTime', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].Time);
      ServerConfig.ProtectAddElements[ElementType].TimeAdd :=
        IniFile.ReadInteger(SectionName, Format('ProtectAdd_E_%sTime2', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].TimeAdd);
      ServerConfig.ProtectAddElements[ElementType].TimeAddIsPoint :=
        IniFile.ReadBool(SectionName, Format('ProtectAdd_E_%sTime2Unit', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].TimeAddIsPoint);
      ServerConfig.ProtectAddElements[ElementType].ShowHint :=
        IniFile.ReadBool(SectionName, Format('ProtectAdd_E_%sShowHint', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].ShowHint);
      ServerConfig.ProtectAddElements[ElementType].HintText :=
        IniFile.ReadString(SectionName, Format('ProtectAdd_E_%sHintText', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].HintText);
    end;

    ServerConfig.ProtectAddHPSlow := IniFile.ReadBool(SectionName, 'ProtectAddHPSlow', ServerConfig.ProtectAddHPSlow);
    ServerConfig.ProtectAddHpSlowCount := IniFile.ReadInteger(SectionName, 'ProtectAddHpSlowCount',
      ServerConfig.ProtectAddHpSlowCount);

    ServerConfig.ProtectTargetStatus := IniFile.ReadBool(SectionName, 'ProtectTargetStatus', ServerConfig.ProtectTargetStatus);
    ServerConfig.ProtectTargetStatusTime := IniFile.ReadInteger(SectionName, 'ProtectTargetStatusTime',
      ServerConfig.ProtectTargetStatusTime);
    ServerConfig.ProtectTargetStatusTimeUnit := IniFile.ReadInteger(SectionName, 'ProtectTargetStatusTimeUnit',
      ServerConfig.ProtectTargetStatusTimeUnit);
    ServerConfig.ProtectTargetStatusTime2 := IniFile.ReadInteger(SectionName, 'ProtectTargetStatusTime2',
      ServerConfig.ProtectTargetStatusTime2);
    ServerConfig.ProtectTargetStatusDelay := IniFile.ReadInteger(SectionName, 'ProtectTargetStatusDelay',
      ServerConfig.ProtectTargetStatusDelay);

    ServerConfig.ProtectTargetRange := IniFile.ReadInteger(SectionName, 'ProtectTargetRange', ServerConfig.ProtectTargetRange);
    // ServerConfig.ProtectSelfRate := IniFile.ReadInteger(SectionName, 'ProtectSelfRate', ServerConfig.ProtectSelfRate);
  finally
    IniFile.Free;
  end;
end;

procedure TCustomMagicConfig.SaveToIniFile;
var
  FileName, SectionName: string;
  IniFile: TIniFileEx;
  I: Integer;
  MagicClientConfig: PMagicClientConfig;
  MagicPlusLevel: TMagicPlusLevel;

  SoundType: TMagicSoundType;

  ElementType: TItemElementsType;
  DecAttribType: TMagicAttackDecAttributesType;
  IncAttribType: TMagicProtectAddAttributesType;
  BreakDefenseType: TBreakDefenseType;
  sTemp: string;
begin
  FIsChanged := False;

  if not DirectoryExists(g_Config.sCustomMagicDir) then
    ForceDirectories(g_Config.sCustomMagicDir);

  FileName := g_Config.sCustomMagicDir + FMagicName + '.ini';
  IniFile := TIniFileEx.Create(FileName);
  try
    SectionName := 'ClientConfig';
    // IniFile.WriteBool(SectionName, 'MagicLevelEnabled', ClientBaseConfig.MagicLevelEnabled);
    // IniFile.WriteBool(SectionName, 'MagicWarr', ClientBaseConfig.MagicWarr);
    IniFile.WriteBool(SectionName, 'MagicLock', ClientBaseConfig.MagicLock);
    IniFile.WriteBool(SectionName, 'MagicLockSelf', ClientBaseConfig.MagicLockSelf);
    IniFile.WriteInteger(SectionName, 'MagicActionType', Integer(ClientBaseConfig.MagicActionType));
    IniFile.WriteInteger(SectionName, 'MagicSwitchMode', Integer(ClientBaseConfig.MagicSwitchMode));
    IniFile.WriteBool(SectionName, 'SwitchModeNoClose', ClientBaseConfig.SwitchModeNoClose);

    IniFile.WriteInteger(SectionName, 'MagicActionStartIndex', ClientBaseConfig.MagicActionStartIndex);
    IniFile.WriteInteger(SectionName, 'MagicActionPlayCount', ClientBaseConfig.MagicActionPlayCount);
    IniFile.WriteInteger(SectionName, 'MagicActionEmptyCount', ClientBaseConfig.MagicActionEmptyCount);
    IniFile.WriteBool(SectionName, 'MagicActionContinue', ClientBaseConfig.MagicActionContinue);
    IniFile.WriteBool(SectionName, 'MagicAutoOpen', ClientBaseConfig.MagicAutoOpen);
    IniFile.WriteInteger(SectionName, 'MagicWarrNGOption', Integer(ClientBaseConfig.MagicWarrNGOption));
    IniFile.WriteBool(SectionName, 'NotRaiseHand', ClientBaseConfig.NotRaiseHand);

    for MagicPlusLevel := Low(ClientConfigs) to High(ClientConfigs) do
    begin
      MagicClientConfig := @ClientConfigs[MagicPlusLevel];
      SectionName := 'ClientAttack' + IntToStr(Integer(MagicPlusLevel));

      IniFile.WriteInteger(SectionName, 'Icon_File', MagicClientConfig.Icon_File);
      IniFile.WriteInteger(SectionName, 'Icon_Index', MagicClientConfig.Icon_Index);

      for SoundType := Low(TMagicSoundType) to High(TMagicSoundType) do
        IniFile.WriteString(SectionName, MagicSoundTypeNames[SoundType], MagicClientConfig.Sounds[SoundType]);

      IniFile.WriteInteger(SectionName, 'Fly_File', MagicClientConfig.Fly_File);
      IniFile.WriteInteger(SectionName, 'Fly_StartIndex', MagicClientConfig.Fly_StartIndex);
      IniFile.WriteInteger(SectionName, 'Fly_PlayCount', MagicClientConfig.Fly_PlayCount);
      IniFile.WriteInteger(SectionName, 'Fly_EmptyCount', MagicClientConfig.Fly_EmptyCount);
      IniFile.WriteInteger(SectionName, 'Fly_PlayTime', MagicClientConfig.Fly_PlayTime);
      IniFile.WriteInteger(SectionName, 'Fly_DrawMode', Integer(MagicClientConfig.Fly_DrawMode));
      IniFile.WriteInteger(SectionName, 'Fly_DirCount', Integer(MagicClientConfig.Fly_DirCount));
      IniFile.WriteBool(SectionName, 'Fly_CalcDir', MagicClientConfig.Fly_CalcDir);
      IniFile.WriteBool(SectionName, 'Fly_FireGunMode', MagicClientConfig.Fly_FireGunMode);
      IniFile.WriteInteger(SectionName, 'Fly_LightRange', MagicClientConfig.Fly_LightRange);

      IniFile.WriteInteger(SectionName, 'FlyEff_File', MagicClientConfig.FlyEff_File);
      IniFile.WriteInteger(SectionName, 'FlyEff_StartIndex', MagicClientConfig.FlyEff_StartIndex);
      IniFile.WriteInteger(SectionName, 'FlyEff_DrawMode', Integer(MagicClientConfig.FlyEff_DrawMode));

      IniFile.WriteInteger(SectionName, 'Self_File', MagicClientConfig.Self_File);
      IniFile.WriteInteger(SectionName, 'Self_StartIndex', MagicClientConfig.Self_StartIndex);
      IniFile.WriteBool(SectionName, 'Self_SyncHumAction', MagicClientConfig.Self_SyncHumAction);
      IniFile.WriteInteger(SectionName, 'Self_PlayCount', MagicClientConfig.Self_PlayCount);
      IniFile.WriteInteger(SectionName, 'Self_EmptyCount', MagicClientConfig.Self_EmptyCount);
      IniFile.WriteInteger(SectionName, 'Self_PlayTime', MagicClientConfig.Self_PlayTime);

      // IniFile.WriteInteger(SectionName, 'Self_PlayMode', Integer(MagicClientConfig.Self_PlayMode));
      IniFile.WriteInteger(SectionName, 'Self_DrawOrder', Integer(MagicClientConfig.Self_DrawOrder));
      IniFile.WriteInteger(SectionName, 'Self_DrawMode', Integer(MagicClientConfig.Self_DrawMode));
      IniFile.WriteInteger(SectionName, 'Self_DirCalcType', Integer(MagicClientConfig.Self_DirCalcType));
      IniFile.WriteInteger(SectionName, 'Self_DirCount', Integer(MagicClientConfig.Self_DirCount));
      IniFile.WriteBool(SectionName, 'Self_PlayDelayAction', MagicClientConfig.Self_PlayDelayAction);
      IniFile.WriteInteger(SectionName, 'Self_LightRange', MagicClientConfig.Self_LightRange);
      IniFile.WriteBool(SectionName, 'Self_PlayFailNoDraw', MagicClientConfig.Self_PlayFailNoDraw);

      IniFile.WriteInteger(SectionName, 'SelfKeep_File', MagicClientConfig.SelfKeep_File);
      IniFile.WriteInteger(SectionName, 'SelfKeep_StartIndex', MagicClientConfig.SelfKeep_StartIndex);
      IniFile.WriteInteger(SectionName, 'SelfKeep_PlayCount', MagicClientConfig.SelfKeep_PlayCount);
      IniFile.WriteInteger(SectionName, 'SelfKeep_PlayTime', MagicClientConfig.SelfKeep_PlayTime);
      IniFile.WriteInteger(SectionName, 'SelfKeep_DrawMode', Integer(MagicClientConfig.SelfKeep_DrawMode));
      IniFile.WriteInteger(SectionName, 'SelfKeep_KeepTime', MagicClientConfig.SelfKeep_KeepTime);
      IniFile.WriteInteger(SectionName, 'SelfKeep_KeepTime2', MagicClientConfig.SelfKeep_KeepTime2);

      IniFile.WriteInteger(SectionName, 'FastMove_File', MagicClientConfig.FastMove_File);
      IniFile.WriteInteger(SectionName, 'FastMove_StartIndex', MagicClientConfig.FastMove_StartIndex);
      IniFile.WriteInteger(SectionName, 'FastMove_PlayCount', MagicClientConfig.FastMove_PlayCount);
      IniFile.WriteInteger(SectionName, 'FastMove_EmptyCount', MagicClientConfig.FastMove_EmptyCount);
      IniFile.WriteInteger(SectionName, 'FastMove_PlayTime', MagicClientConfig.FastMove_PlayTime);

      // IniFile.WriteInteger(SectionName, 'FastMove_PlayMode', Integer(MagicClientConfig.FastMove_PlayMode));
      IniFile.WriteInteger(SectionName, 'FastMove_DrawOrder', Integer(MagicClientConfig.FastMove_DrawOrder));
      IniFile.WriteInteger(SectionName, 'FastMove_DrawMode', Integer(MagicClientConfig.FastMove_DrawMode));
      IniFile.WriteBool(SectionName, 'FastMove_CalcDir', MagicClientConfig.FastMove_CalcDir);
      IniFile.WriteBool(SectionName, 'FastMove_NoHitAction', MagicClientConfig.FastMove_NoHitAction);
      IniFile.WriteInteger(SectionName, 'FastMove_LightRange', MagicClientConfig.FastMove_LightRange);

      IniFile.WriteInteger(SectionName, 'PreTarget_File', MagicClientConfig.PreTarget_File);
      IniFile.WriteInteger(SectionName, 'PreTarget_StartIndex', MagicClientConfig.PreTarget_StartIndex);
      IniFile.WriteInteger(SectionName, 'PreTarget_StartIndex2', MagicClientConfig.PreTarget_StartIndex2);
      IniFile.WriteInteger(SectionName, 'PreTarget_PlayCount', MagicClientConfig.PreTarget_PlayCount);
      IniFile.WriteInteger(SectionName, 'PreTarget_EmptyCount', MagicClientConfig.PreTarget_EmptyCount);
      IniFile.WriteInteger(SectionName, 'PreTarget_PlayTime', MagicClientConfig.PreTarget_PlayTime);
      IniFile.WriteInteger(SectionName, 'PreTarget_DrawMode', Integer(MagicClientConfig.PreTarget_DrawMode));
      IniFile.WriteInteger(SectionName, 'PreTarget_DrawMode2', Integer(MagicClientConfig.PreTarget_DrawMode2));
      IniFile.WriteBool(SectionName, 'PreTarget_CalcDir', MagicClientConfig.PreTarget_CalcDir);
      IniFile.WriteBool(SectionName, 'PreTarget_LockDraw', MagicClientConfig.PreTarget_LockDraw);
      IniFile.WriteInteger(SectionName, 'PreTarget_LightRange', MagicClientConfig.PreTarget_LightRange);

      IniFile.WriteInteger(SectionName, 'Target_File', MagicClientConfig.Target_File);
      IniFile.WriteInteger(SectionName, 'Target_StartIndex', MagicClientConfig.Target_StartIndex);
      IniFile.WriteInteger(SectionName, 'Target_StartIndex2', MagicClientConfig.Target_StartIndex2);
      IniFile.WriteInteger(SectionName, 'Target_PlayCount', MagicClientConfig.Target_PlayCount);
      IniFile.WriteInteger(SectionName, 'Target_PlayTime', MagicClientConfig.Target_PlayTime);
      IniFile.WriteInteger(SectionName, 'Target_DrawMode', Integer(MagicClientConfig.Target_DrawMode));
      IniFile.WriteInteger(SectionName, 'Target_DrawMode2', Integer(MagicClientConfig.Target_DrawMode2));
      IniFile.WriteBool(SectionName, 'Target_MultiPlay', MagicClientConfig.Target_MultiPlay);
      IniFile.WriteBool(SectionName, 'Target_LockDraw', MagicClientConfig.Target_LockDraw);
      IniFile.WriteInteger(SectionName, 'Target_LightRange', MagicClientConfig.Target_LightRange);

      IniFile.WriteBool(SectionName, 'Target_KeepPlay', MagicClientConfig.Target_KeepPlay);
      IniFile.WriteInteger(SectionName, 'Target_KeepTime', MagicClientConfig.Target_KeepTime);
      IniFile.WriteInteger(SectionName, 'Target_KeepTime2', MagicClientConfig.Target_KeepTime2);
      IniFile.WriteInteger(SectionName, 'Target_KeepAttackRange', MagicClientConfig.Target_KeepAttackRange);
      IniFile.WriteBool(SectionName, 'Target_KeepMultiPlay', MagicClientConfig.Target_KeepMultiPlay);
      IniFile.WriteInteger(SectionName, 'Target_KeepAttackInterval', MagicClientConfig.Target_KeepAttackInterval);
      IniFile.WriteInteger(SectionName, 'Target_KeepLightRange', MagicClientConfig.Target_KeepLightRange);

      IniFile.WriteInteger(SectionName, 'TargetStatus1_File', MagicClientConfig.TargetStatus1_File);
      IniFile.WriteInteger(SectionName, 'TargetStatus1_StartIndex', MagicClientConfig.TargetStatus1_StartIndex);
      IniFile.WriteInteger(SectionName, 'TargetStatus1_PlayCount', MagicClientConfig.TargetStatus1_PlayCount);
      IniFile.WriteInteger(SectionName, 'TargetStatus1_EmptyCount', MagicClientConfig.TargetStatus1_EmptyCount);
      // IniFile.WriteInteger(SectionName, 'TargetStatus1_PlayTime', MagicClientConfig.TargetStatus1_PlayTime);
      IniFile.WriteInteger(SectionName, 'TargetStatus1_DrawMode', Integer(MagicClientConfig.TargetStatus1_DrawMode));
      IniFile.WriteBool(SectionName, 'TargetStatus1_CalcDir', MagicClientConfig.TargetStatus1_CalcDir);

      IniFile.WriteInteger(SectionName, 'TargetStatus2_File', MagicClientConfig.TargetStatus2_File);
      IniFile.WriteInteger(SectionName, 'TargetStatus2_StartIndex', MagicClientConfig.TargetStatus2_StartIndex);
      IniFile.WriteInteger(SectionName, 'TargetStatus2_PlayCount', MagicClientConfig.TargetStatus2_PlayCount);
      IniFile.WriteInteger(SectionName, 'TargetStatus2_EmptyCount', MagicClientConfig.TargetStatus2_EmptyCount);
      // IniFile.WriteInteger(SectionName, 'TargetStatus2_PlayTime', MagicClientConfig.TargetStatus2_PlayTime);
      IniFile.WriteInteger(SectionName, 'TargetStatus2_DrawMode', Integer(MagicClientConfig.TargetStatus2_DrawMode));
      IniFile.WriteBool(SectionName, 'TargetStatus2_CalcDir', MagicClientConfig.TargetStatus2_CalcDir);
    end;

    SectionName := 'ServerConfig';

    IniFile.WriteInteger(SectionName, 'OperateMode', Integer(ServerConfig.OperateMode));
    IniFile.WriteBool(SectionName, 'IsAttackUseNG', ServerConfig.IsAttackUseNG);
    IniFile.WriteBool(SectionName, 'NoChangeDir', ServerConfig.NoChangeDir);
    IniFile.WriteBool(SectionName, 'DisableInSafeZone', ServerConfig.DisableInSafeZone);
    IniFile.WriteInteger(SectionName, 'AttackDelayTime', ServerConfig.AttackDelayTime);
    IniFile.WriteInteger(SectionName, 'UseInterval', ServerConfig.UseInterval);

    // IniFile.WriteBool(SectionName, 'FailNoShowEff', ServerConfig.FailNoShowEff);
    IniFile.WriteString(SectionName, 'FailMsg', ServerConfig.FailMsg);
    IniFile.WriteString(SectionName, 'SucceedMsg', ServerConfig.SucceedMsg);
    IniFile.WriteString(SectionName, 'CloseMsg', ServerConfig.CloseMsg);

    IniFile.WriteBool(SectionName, 'IsCheckVarValue', ServerConfig.IsCheckVarValue);
    IniFile.WriteInteger(SectionName, 'NeedItem', Integer(ServerConfig.NeedItem));
    IniFile.WriteInteger(SectionName, 'NeedItemCount', ServerConfig.NeedItemCount);
    IniFile.WriteString(SectionName, 'NeedItemCustomItemName', ServerConfig.NeedItemCustomItemName);
    IniFile.WriteBool(SectionName, 'NeedItemUseBagItem', ServerConfig.NeedItemUseBagItem);
    IniFile.WriteString(SectionName, 'CheckVarName', ServerConfig.CheckVarName);
    IniFile.WriteInteger(SectionName, 'CheckVarType', Integer(ServerConfig.CheckVarType));
    IniFile.WriteInteger(SectionName, 'CheckVarValue', ServerConfig.CheckVarValue);
    IniFile.WriteInteger(SectionName, 'CheckVarAdd', ServerConfig.CheckVarAdd);

    IniFile.WriteInteger(SectionName, 'AttackMode', Integer(ServerConfig.AttackMode));
    IniFile.WriteInteger(SectionName, 'AttackTarget', Integer(ServerConfig.AttackTarget));
    IniFile.WriteInteger(SectionName, 'AttackPowerCalc', Integer(ServerConfig.AttackPowerCalc));
    for I := Low(ServerConfig.AttackPowerRates) to High(ServerConfig.AttackPowerRates) do
    begin
      IniFile.WriteInteger(SectionName, 'AttackPowerRate_' + IntToStr(I), ServerConfig.AttackPowerRates[I]);
    end;
    IniFile.WriteInteger(SectionName, 'AttackPowerLineAdd', ServerConfig.AttackPowerLineAdd);
    IniFile.WriteInteger(SectionName, 'AttackPowerUndeadAdd', ServerConfig.AttackPowerUndeadAdd);

    IniFile.WriteInteger(SectionName, 'AttackNearRange', ServerConfig.AttackNearRange);
    IniFile.WriteInteger(SectionName, 'AttackGroupRange', ServerConfig.AttackGroupRange);
    IniFile.WriteInteger(SectionName, 'AttackLineWidth', ServerConfig.AttackLineWidth);
    IniFile.WriteBool(SectionName, 'EnableAntiMagic', ServerConfig.EnableAntiMagic);
    IniFile.WriteBool(SectionName, 'EnableHitPoint', ServerConfig.EnableHitPoint);

    IniFile.WriteBool(SectionName, 'AttackTeleportAttack', ServerConfig.AttackTeleportAttack);
    IniFile.WriteBool(SectionName, 'IsNoTeleportNoAttack', ServerConfig.IsNoTeleportNoAttack);
    IniFile.WriteInteger(SectionName, 'AttackTeleportRate', ServerConfig.AttackTeleportRate);

    IniFile.WriteBool(SectionName, 'AttackTeleportRunHum', ServerConfig.AttackTeleportRunHum);
    IniFile.WriteBool(SectionName, 'AttackTeleportRunMon', ServerConfig.AttackTeleportRunMon);
    IniFile.WriteBool(SectionName, 'AttackTeleportRunNpc', ServerConfig.AttackTeleportRunNpc);
    IniFile.WriteBool(SectionName, 'AttackTeleportRunGuard', ServerConfig.AttackTeleportRunGuard);
    IniFile.WriteBool(SectionName, 'AttackTeleportRunObstacle', ServerConfig.AttackTeleportRunObstacle);
    IniFile.WriteBool(SectionName, 'AttackTeleportWarDisHumRun', ServerConfig.AttackTeleportWarDisHumRun);
    IniFile.WriteBool(SectionName, 'AttackTeleportCannotRunItem', ServerConfig.AttackTeleportCannotRunItem);

    IniFile.WriteBool(SectionName, 'AttackTeleportRush', ServerConfig.AttackTeleportRush);
    IniFile.WriteInteger(SectionName, 'AttackTeleportRushCount', ServerConfig.AttackTeleportRushCount);
    IniFile.WriteBool(SectionName, 'AttackTeleportAfterDamage', ServerConfig.AttackTeleportAfterDamage);

    SectionName := 'CallMonster';
    IniFile.WriteBool(SectionName, 'EnabledCallMonster', ServerConfig.EnabledCallMonster);
    IniFile.WriteInteger(SectionName, 'CallMonstersRate', ServerConfig.CallMonstersRate);
    IniFile.WriteInteger(SectionName, 'CallMonstersRoyaltySec', ServerConfig.CallMonstersRoyaltySec);
    IniFile.WriteInteger(SectionName, 'CallMonstersLevel', ServerConfig.CallMonstersLevel);
    for I := Low(ServerConfig.CallMonsters) to High(ServerConfig.CallMonsters) do
    begin
      IniFile.WriteString(SectionName, 'MonsterName' + IntToStr(I), ServerConfig.CallMonsters[I]);
      IniFile.WriteInteger(SectionName, 'MonsterNum' + IntToStr(I), ServerConfig.CallMonsterNums[I]);;
    end;

    SectionName := 'Additionals';
    for I := Low(ServerConfig.Additionals) to High(ServerConfig.Additionals) do
    begin
      IniFile.WriteBool(SectionName, 'Checked' + IntToStr(I), ServerConfig.Additionals[I].Checked);
      IniFile.WriteInteger(SectionName, 'Rate' + IntToStr(I), ServerConfig.Additionals[I].Rate);
      IniFile.WriteInteger(SectionName, 'Rate2' + IntToStr(I), ServerConfig.Additionals[I].Rate2);
      IniFile.WriteInteger(SectionName, 'Time' + IntToStr(I), ServerConfig.Additionals[I].Time);
      IniFile.WriteInteger(SectionName, 'TimeUnit' + IntToStr(I), ServerConfig.Additionals[I].TimeUnit);
      IniFile.WriteInteger(SectionName, 'Time2' + IntToStr(I), ServerConfig.Additionals[I].Time2);

      if I = 0 then
        IniFile.WriteInteger(SectionName, 'HP0', ServerConfig.AdditionalHP0)
      else if I = 4 then
      begin
        IniFile.WriteBool(SectionName, 'HighLevel4', ServerConfig.AdditionalHighLevel4);
        IniFile.WriteInteger(SectionName, 'PushedType4', ServerConfig.AdditionalPushedType4);
      end;
    end;

    IniFile.WriteBool(SectionName, 'AttackTargetStatus', ServerConfig.AttackTargetStatus);
    IniFile.WriteInteger(SectionName, 'AttackTargetStatusTime', ServerConfig.AttackTargetStatusTime);
    IniFile.WriteInteger(SectionName, 'AttackTargetStatusTimeUnit', ServerConfig.AttackTargetStatusTimeUnit);
    IniFile.WriteInteger(SectionName, 'AttackTargetStatusTime2', ServerConfig.AttackTargetStatusTime2);
    IniFile.WriteInteger(SectionName, 'AttackTargetStatusDelay', ServerConfig.AttackTargetStatusDelay);

    for DecAttribType := Low(TMagicAttackDecAttributesType) to High(TMagicAttackDecAttributesType) do
    begin
      sTemp := MagicAttackDecAttributesTypeIniNames[DecAttribType];

      IniFile.WriteBool(SectionName, Format('AttackSub%s', [sTemp]), ServerConfig.AttackSubAttrib[DecAttribType].IsChecked);
      IniFile.WriteInteger(SectionName, Format('AttackSub%sRate', [sTemp]), ServerConfig.AttackSubAttrib[DecAttribType].Rate);
      IniFile.WriteInteger(SectionName, Format('AttackSub%sRate2', [sTemp]), ServerConfig.AttackSubAttrib[DecAttribType].RateAdd);

      IniFile.WriteInteger(SectionName, Format('AttackSub%s1Percent', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].LowValue);
      IniFile.WriteBool(SectionName, Format('AttackSub%s1Unit', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].LowValueIsPoint);
      IniFile.WriteInteger(SectionName, Format('AttackSub%s1Percent2', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].LowValueAdd);

      IniFile.WriteInteger(SectionName, Format('AttackSub%s2Percent', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HighValue);
      IniFile.WriteBool(SectionName, Format('AttackSub%s2Unit', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HighValueIsPoint);
      IniFile.WriteInteger(SectionName, Format('AttackSub%s2Percent2', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HighValueAdd);

      IniFile.WriteInteger(SectionName, Format('AttackSub%sTime', [sTemp]), ServerConfig.AttackSubAttrib[DecAttribType].Time);
      IniFile.WriteInteger(SectionName, Format('AttackSub%sTime2', [sTemp]), ServerConfig.AttackSubAttrib[DecAttribType].TimeAdd);
      IniFile.WriteBool(SectionName, Format('AttackSub%sTime2Unit', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].TimeAddIsPoint);

      IniFile.WriteBool(SectionName, Format('AttackSub%sShowHint', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].ShowHint);
      IniFile.WriteString(SectionName, Format('AttackSub%sHintText', [sTemp]),
        ServerConfig.AttackSubAttrib[DecAttribType].HintText);
    end;

    for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
    begin
      sTemp := ItemElementsTypeIniNames[ElementType];

      IniFile.WriteBool(SectionName, Format('AttackSub_E_%s', [sTemp]), ServerConfig.AttackSubElements[ElementType].IsChecked);
      IniFile.WriteInteger(SectionName, Format('AttackSub_E_%sRate', [sTemp]), ServerConfig.AttackSubElements[ElementType].Rate);
      IniFile.WriteInteger(SectionName, Format('AttackSub_E_%sRate2', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].RateAdd);
      IniFile.WriteInteger(SectionName, Format('AttackSub_E_%sPercent', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].Value);
      IniFile.WriteBool(SectionName, Format('AttackSub_E_%sUnit', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].ValueIsPoint);
      IniFile.WriteInteger(SectionName, Format('AttackSub_E_%sPercent2', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].ValueAdd);
      IniFile.WriteInteger(SectionName, Format('AttackSub_E_%sTime', [sTemp]), ServerConfig.AttackSubElements[ElementType].Time);
      IniFile.WriteInteger(SectionName, Format('AttackSub_E_%sTime2', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].TimeAdd);
      IniFile.WriteBool(SectionName, Format('AttackSub_E_%sTime2Unit', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].TimeAddIsPoint);
      IniFile.WriteBool(SectionName, Format('AttackSub_E_%sShowHint', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].ShowHint);
      IniFile.WriteString(SectionName, Format('AttackSub_E_%sHintText', [sTemp]),
        ServerConfig.AttackSubElements[ElementType].HintText);
    end;

    for BreakDefenseType := Low(TBreakDefenseType) to High(TBreakDefenseType) do
    begin
      sTemp := BreakDefenseTypeNames[BreakDefenseType];

      IniFile.WriteBool(SectionName, Format('%s', [sTemp]), ServerConfig.AttackBreakDefense[BreakDefenseType].IsChecked);
      IniFile.WriteInteger(SectionName, Format('%s_Rate', [sTemp]), ServerConfig.AttackBreakDefense[BreakDefenseType].Rate);
      IniFile.WriteInteger(SectionName, Format('%s_RateAdd', [sTemp]), ServerConfig.AttackBreakDefense[BreakDefenseType].RateAdd);
      IniFile.WriteInteger(SectionName, Format('%s_Value', [sTemp]), ServerConfig.AttackBreakDefense[BreakDefenseType].Value);
      IniFile.WriteInteger(SectionName, Format('%s_ValueAdd', [sTemp]), ServerConfig.AttackBreakDefense[BreakDefenseType]
        .ValueAdd);
    end;

    SectionName := 'Protect';

    for IncAttribType := Low(TMagicProtectAddAttributesType) to High(TMagicProtectAddAttributesType) do
    begin
      sTemp := MagicProtectAddAttributesTypeIniNames[IncAttribType];

      IniFile.WriteBool(SectionName, Format('ProtectAdd%s', [sTemp]), ServerConfig.ProtectAddAttrib[IncAttribType].IsChecked);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd%sRate', [sTemp]), ServerConfig.ProtectAddAttrib[IncAttribType].Rate);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd%sRate2', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].RateAdd);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd%s1Percent', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].LowValue);
      IniFile.WriteBool(SectionName, Format('ProtectAdd%s1Unit', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].LowValueIsPoint);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd%s1Percent2', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].LowValueAdd);

      if IncAttribType = aaHP then
      begin
        IniFile.WriteInteger(SectionName, 'ProtectAddHPPercent', ServerConfig.ProtectAddAttrib[IncAttribType].HighValue);
        IniFile.WriteBool(SectionName, 'ProtectAddHPUnit', ServerConfig.ProtectAddAttrib[IncAttribType].HighValueIsPoint);
        IniFile.WriteInteger(SectionName, 'ProtectAddHPPercent2', ServerConfig.ProtectAddAttrib[IncAttribType].HighValueAdd);
      end
      else
      begin
        IniFile.WriteInteger(SectionName, Format('ProtectAdd%s2Percent', [sTemp]),
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValue);
        IniFile.WriteBool(SectionName, Format('ProtectAdd%s2Unit', [sTemp]),
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValueIsPoint);
        IniFile.WriteInteger(SectionName, Format('ProtectAdd%s2Percent2', [sTemp]),
          ServerConfig.ProtectAddAttrib[IncAttribType].HighValueAdd);
      end;

      IniFile.WriteInteger(SectionName, Format('ProtectAdd%sTime', [sTemp]), ServerConfig.ProtectAddAttrib[IncAttribType].Time);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd%sTime2', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].TimeAdd);
      IniFile.WriteBool(SectionName, Format('ProtectAdd%sTime2Unit', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].TimeAddIsPoint);
      IniFile.WriteBool(SectionName, Format('ProtectAdd%sShowHint', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].ShowHint);
      IniFile.WriteString(SectionName, Format('ProtectAdd%sHintText', [sTemp]),
        ServerConfig.ProtectAddAttrib[IncAttribType].HintText);
    end;

    IniFile.WriteBool(SectionName, 'ProtectAddHPSlow', ServerConfig.ProtectAddHPSlow);
    IniFile.WriteInteger(SectionName, 'ProtectAddHpSlowCount', ServerConfig.ProtectAddHpSlowCount);

    IniFile.WriteBool(SectionName, 'ProtectTargetStatus', ServerConfig.ProtectTargetStatus);
    IniFile.WriteInteger(SectionName, 'ProtectTargetStatusTime', ServerConfig.ProtectTargetStatusTime);
    IniFile.WriteInteger(SectionName, 'ProtectTargetStatusTimeUnit', ServerConfig.ProtectTargetStatusTimeUnit);
    IniFile.WriteInteger(SectionName, 'ProtectTargetStatusTime2', ServerConfig.ProtectTargetStatusTime2);
    IniFile.WriteInteger(SectionName, 'ProtectTargetStatusDelay', ServerConfig.ProtectTargetStatusDelay);

    for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
    begin
      sTemp := ItemElementsTypeIniNames[ElementType];

      IniFile.WriteBool(SectionName, Format('ProtectAdd_E_%s', [sTemp]), ServerConfig.ProtectAddElements[ElementType].IsChecked);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd_E_%sRate', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].Rate);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd_E_%sRate2', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].RateAdd);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd_E_%sPercent', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].Value);
      IniFile.WriteBool(SectionName, Format('ProtectAdd_E_%sUnit', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].ValueIsPoint);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd_E_%sPercent2', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].ValueAdd);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd_E_%sTime', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].Time);
      IniFile.WriteInteger(SectionName, Format('ProtectAdd_E_%sTime2', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].TimeAdd);
      IniFile.WriteBool(SectionName, Format('ProtectAdd_E_%sTime2Unit', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].TimeAddIsPoint);
      IniFile.WriteBool(SectionName, Format('ProtectAdd_E_%sShowHint', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].ShowHint);
      IniFile.WriteString(SectionName, Format('ProtectAdd_E_%sHintText', [sTemp]),
        ServerConfig.ProtectAddElements[ElementType].HintText);
    end;

    IniFile.WriteInteger(SectionName, 'ProtectTargetRange', ServerConfig.ProtectTargetRange);
    // IniFile.WriteInteger(SectionName, 'ProtectSelfRate', ServerConfig.ProtectSelfRate);
  finally
    IniFile.Free;
  end;
end;

end.
