unit uCustomMonsterUtils;

interface

uses
  Windows, SysUtils, Classes, IniFilesEx, Grobal2, CheckUnit;

const
  MonsterClientActionNames: array [TMonsterClientActionType] of string = ('站', '走', '默认攻击', '被攻击', '死亡', '石化苏醒', '攻击1', '攻击2',
    '攻击3', '攻击4', '攻击5', '攻击6');

  MonsterTypeNames: array [TMonsterType] of string = ('普通怪物', '石化怪物', '苏醒怪物');

  MoveOptionNames: array [TMoveOption] of string = ('自由移动', '不可移动', '守护区域');

  MonsterClientActionSections: array [TMonsterClientActionType] of string = ('ActStand', 'ActWalk', 'ActDefAttack', 'ActStruck',
    'ActDie', 'ActStoneMode', 'ActAttack1', 'ActAttack2', 'ActAttack3', 'ActAttack4', 'ActAttack5', 'ActAttack6');

  MonsterDrawOrder2Names: array [TMonsterDrawOrder2] of string = ('自身→特效1→特效2', '特效1→自身→特效2', '特效1→特效2→自身');

  MonsterSoundTypeNames: array [TMonsterSoundType] of string = ('Normal', 'DigUP', 'Attack', 'Struck', 'Die', 'Attack1',
    'Attack2', 'Attack3', 'Attack4', 'Attack5', 'Attack6');

type
  // 附加伤害
  TAdditionalDamage = record
    Checked: Boolean;
    Rate: Byte;
    Time: Word;
  end;

  TAdditionalDamageArray = array [0 .. 11] of TAdditionalDamage;

  PMonsterServerConfig = ^TMonsterServerConfig;

  TMonsterServerConfig = record
    AttackEnabled: Boolean; // 启用攻击
    OperateMode: TCustomOperateMode; // 操作模式 chongchong 2014-09-06

    AttackSelfDie: Boolean; // 自杀式攻击
    AttackDelayTime: Integer;

    // AttackWaitTime: Integer;                          // 使用间隔
    AttackHPPercent: Integer; // 使用条件HP%<
    AttackRate: Integer; // 使用几率
    AttackTargetCount: Integer; // 目标数量
    AttackMode: TCustomAttackMode; // 攻击方式
    AttackTarget: TCustomAttackTarget; // 攻击目标
    AttackPowerCalc: TCustomAttackPowerCalc; // 威力计算
    AttackPowerRate: Integer; // 威力倍数

    AttackTeleportAttack: Boolean; // 瞬移攻击
    AttackTeleportTargetDistance: Integer; // 瞬移攻击 -- 目标距离 (与攻击目标相隔大于或等于指定距离时，触发瞬移)
    AttackTeleportDistance: Integer; // 瞬移距离 -- 触发瞬移后，向目标靠近距离
    AttackTeleportRate: Integer; // 瞬移几率
    AttackTeleportRush: Boolean; // 突进型瞬移 By 一支笔 at:2021-07-06 15:29:03

    AttackIgnoreDefence: Boolean; // 无视物理防御

    AttackNearRange: Integer; // 近攻范围
    AttackGroupRange: Integer; // 群攻范围
    NearAttackTargetCenter: Boolean; // 近攻类型的攻击，群伤以目标为中心

    AttackPowerInc: Integer; // 随目标数量递增攻击力

    Additionals: TAdditionalDamageArray;

    AdditionalHP0: Integer; // 绿毒掉血
    AdditionalHighLevel4: Boolean; // 可推动高等级
    AdditionalImprisonRange: Integer; // 禁锢格数

    EnabledCallMonster: Boolean; // 允许召唤怪物

    CallMonstersRate: Integer; // 怪物召唤几率
    CallMonsters: array [0 .. 3] of string[ITEM_NAME_LEN]; // 召唤怪物1-4
    CallMonsterNums: array [0 .. 3] of Integer; // 怪物1-4数量

    MoveTarget: Boolean; // 移动目标到身边
    MoveTargetRate: Integer; // 移动几率
    MoveTargetHighLevel: Boolean; // 可移动高等级

    // 保护模式
    ProtectAddHP: Boolean; // 加血
    ProtectAddHPRate: Integer; // 加血几率
    ProtectAddHPPercent: Integer; // 加血百分比

    ProtectAddDefence: Boolean; // 加防御
    ProtectAddDefenceRate: Integer; // 加防御几率
    ProtectAddDefencePercent: Integer; // 加防御比例
    ProtectAddDefenceTime: Integer; // 加防御时间

    ProtectAddMagDefence: Boolean; // 加魔御
    ProtectAddMagDefenceRate: Integer; // 加魔御几率
    ProtectAddMagDefencePercent: Integer; // 加魔御比例
    ProtectAddMagDefenceTime: Integer; // 加魔御时间

    ProtectAddDC: Boolean; // 加攻击力
    ProtectAddDCRate: Integer; // 加攻击几率
    ProtectAddDCPercent: Integer; // 加攻击百分比
    ProtectAddDCTime: Integer; // 加攻击时间

    ProtectAddMC: Boolean; // 加魔法力
    ProtectAddMCRate: Integer; // 加魔法几率
    ProtectAddMCPercent: Integer; // 加魔法百分比
    ProtectAddMCTime: Integer; // 加魔法时间

    ProtectAddSC: Boolean; // 加道术力
    ProtectAddSCRate: Integer; // 加道术几率
    ProtectAddSCPercent: Integer; // 加道术百分比
    ProtectAddSCTime: Integer; // 加道术时间

    ProtectTargetRange: Integer; // 保护目标范围
    ProtectSelfRate: Integer; // 自我保护几率
  end;

  TMonsterServerBaseConfig = record
    ViewRange: Byte; // 视觉范围
    MonsterType: TMonsterType; // 怪物类型
    MoveOption: TMoveOption; // 移动选项
    ProtectRange: Integer; // 守护区域
    MinAttackNearRange: Integer; // 最近攻击距离 chongchong 2014-09-04
    LightRange: Byte; // 怪物照亮范围 chongchong 2015-03-04
    NoAttack: Boolean; // 怪物不攻击 chongchong 2017-06-28
  end;

  TMonsterServerConfigs = array [Low(AttackConfigNames) .. High(AttackConfigNames)] of TMonsterServerConfig;

  TCustomMonsterConfig = class(TObject)
  private
    FMonsterName: string;
    FMonsterRace: Word;
    FMonsterAppr: Word;
    FIsChanged: Boolean;
  public
    ClientBaseConfig: TClientBaseConfig;
    ClientActions: TMonsterClientActions;
    ClientAttackConfigs: TClientAttackConfigs;

    ServerBaseConfig: TMonsterServerBaseConfig;
    MonsterServerConfigs: TMonsterServerConfigs;
  public
    constructor Create(AMonsterName: string; AMonsterRace, AMonsterAppr: Word);
    destructor Destroy; override;

    procedure SaveToIniFile;
    procedure LoadFromIniFile;
    procedure SetChanged(Value: Boolean = True);

    property MonsterName: string read FMonsterName write FMonsterName;
    property MonsterRace: Word read FMonsterRace;
    property MonsterAppr: Word read FMonsterAppr;
    property IsChanged: Boolean read FIsChanged;
  end;

procedure SaveCustomMonsterClientConfigs(MonsterConfigs: TList; FileName: string);

implementation

uses M2Share;

procedure SaveCustomMonsterClientConfigs(MonsterConfigs: TList; FileName: string);
var
  I, Len: Integer;
  CRC: Cardinal;
  P: PAnsiChar;
  MS: TMemoryStream;
  MonsterConfig: TCustomMonsterConfig;
  ClientConfig: TClientCustomMonsterConfig;
begin
  MS := TMemoryStream.Create;
  try
    MS.Write(ClientCustomMonsterConfigFlag, SizeOf(ClientCustomMonsterConfigFlag));

    Len := MonsterConfigs.Count;
    MS.Write(Len, SizeOf(Len));

    CRC := 0;
    MS.Write(CRC, SizeOf(CRC));

    for I := 0 to MonsterConfigs.Count - 1 do
    begin
      MonsterConfig := MonsterConfigs.Items[I];

      ClientConfig.wMonsterAppr := MonsterConfig.FMonsterAppr;
      ClientConfig.BaseConfig := MonsterConfig.ClientBaseConfig;
      ClientConfig.Actions := MonsterConfig.ClientActions;
      ClientConfig.AttackConfigs := MonsterConfig.ClientAttackConfigs;

      MS.Write(ClientConfig, SizeOf(ClientConfig));
    end;

    Len := SizeOf(ClientCustomMonsterConfigFlag) + SizeOf(Len) + SizeOf(CRC);
    P := MS.Memory;
    Inc(P, Len);
    CRC := BufferCRC(P, MS.Size - Len);

    Len := SizeOf(ClientCustomMonsterConfigFlag) + SizeOf(Len);
    MS.Seek(Len, soFromBeginning);
    MS.Write(CRC, SizeOf(CRC));

    MS.SaveToFile(FileName);
  finally
    MS.Free;
  end;
end;

{ TCustomMonsterConfig }

constructor TCustomMonsterConfig.Create(AMonsterName: string; AMonsterRace, AMonsterAppr: Word);
var
  I, J: Integer;
  ActionType: TMonsterClientActionType;
  ClientAttackConfig: PClientAttackConfig;
  MonsterServerConfig: PMonsterServerConfig;
  SoundType: TMonsterSoundType;
begin
  FIsChanged := False;

  FMonsterName := AMonsterName;
  FMonsterRace := AMonsterRace;
  FMonsterAppr := AMonsterAppr;

  ClientBaseConfig.DrawMode := mdmBlend;
  ClientBaseConfig.DrawMode2 := mdmBlend;
  ClientBaseConfig.DrawOrder := mdoSelf_Eff1_Eff2;
  ClientBaseConfig.DieNoCalcDir := False;
  for SoundType := Low(TMonsterSoundType) to High(TMonsterSoundType) do
    ClientBaseConfig.Sounds[SoundType] := '';

  ClientBaseConfig.HPBgOffsetX := 0;
  ClientBaseConfig.HPBgOffsetY := 0;
  ClientBaseConfig.HPOffsetX := 0;
  ClientBaseConfig.HPOffsetY := 0;
  ClientBaseConfig.HPFile := -1;
  ClientBaseConfig.HPStartIndex := -1;

  ClientBaseConfig.HPTextOffsetX := 0;
  ClientBaseConfig.HPTextOffsetY := 0;

  for ActionType := Low(TMonsterClientActionType) to High(TMonsterClientActionType) do
  begin
    ClientActions[ActionType].ActionType := ActionType;
    ClientActions[ActionType].ActionFile := -1;
    ClientActions[ActionType].StartIndex := -1;
    ClientActions[ActionType].PlayCount := 0;
    ClientActions[ActionType].EmptyCount := 0;
    ClientActions[ActionType].PlayTime := 100;
    ClientActions[ActionType].EffectFile := -1;
    ClientActions[ActionType].EffectIndex := -1;
    ClientActions[ActionType].EffectFile2 := -1;
    ClientActions[ActionType].EffectIndex2 := -1;
    ClientActions[ActionType].CalcDir := True;
  end;

  for I := Low(ClientAttackConfigs) to High(ClientAttackConfigs) do
  begin
    ClientAttackConfig := @ClientAttackConfigs[I];

    ClientAttackConfig.Fly_File := -1;
    ClientAttackConfig.Fly_StartIndex := -1;
    ClientAttackConfig.Fly_PlayCount := 0;
    ClientAttackConfig.Fly_EmptyCount := 0;
    ClientAttackConfig.Fly_PlayTime := 100;
    ClientAttackConfig.Fly_DrawMode := mdmBlend;
    ClientAttackConfig.Fly_DirCount := mdcDir8;
    ClientAttackConfig.Fly_CalcDir := True;
    ClientAttackConfig.Fly_LightRange := 0;

    ClientAttackConfig.FlyEff_File := -1;
    ClientAttackConfig.FlyEff_StartIndex := -1;
    ClientAttackConfig.FlyEff_DrawMode := mdmBlend;

    ClientAttackConfig.Self_File := -1;
    ClientAttackConfig.Self_StartIndex := -1;
    ClientAttackConfig.Self_PlayCount := 0;
    ClientAttackConfig.Self_EmptyCount := 0;
    ClientAttackConfig.Self_PlayTime := 100;
    // ClientAttackConfig.Self_PlayMode := mpmAttack;
    ClientAttackConfig.Self_DrawOrder := mdoPriorSelf;
    ClientAttackConfig.Self_DrawMode := mdmBlend;

    ClientAttackConfig.Self_DirCalcType := mdctNone;
    ClientAttackConfig.Self_DirCount := mdcDir8;

    ClientAttackConfig.Self_PlayDelayAction := False;
    ClientAttackConfig.Self_LightRange := 0;

    ClientAttackConfig.SelfKeep_File := -1;
    ClientAttackConfig.SelfKeep_StartIndex := 0;
    ClientAttackConfig.SelfKeep_StartIndex2 := -1;
    ClientAttackConfig.SelfKeep_PlayCount := 0;
    ClientAttackConfig.SelfKeep_PlayTime := 0;
    ClientAttackConfig.SelfKeep_DrawOrder := mdoPriorSelf;
    ClientAttackConfig.SelfKeep_DrawMode := mdmBlend;
    ClientAttackConfig.SelfKeep_DrawMode2 := mdmBlend;
    ClientAttackConfig.SelfKeep_KeepTime := 0;
    // ClientAttackConfig.SelfKeep_KeepTime2 := 0;

    ClientAttackConfig.Explosion_File := -1;
    ClientAttackConfig.Explosion_StartIndex := -1;
    ClientAttackConfig.Explosion_StartIndex2 := -1;
    ClientAttackConfig.Explosion_PlayCount := 0;
    ClientAttackConfig.Explosion_PlayTime := 100;
    ClientAttackConfig.Explosion_DrawMode := mdmBlend;
    ClientAttackConfig.Explosion_DrawMode2 := mdmBlend;
    ClientAttackConfig.Explosion_LockDraw := False;
    ClientAttackConfig.Explosion_LightRange := 0;

    ClientAttackConfig.Explosion_KeepPlay := False; // 持久伤害 chongchong 2015-03-10
    ClientAttackConfig.Explosion_KeepTime := 30; // 持久伤害时间 chongchong 2015-03-10
    ClientAttackConfig.Explosion_KeepAttackRange := 0; // 持久伤害范围 chongchong 2015-03-10
    ClientAttackConfig.Explosion_KeepMultiPlay := False; // 持久伤害 每格单独播放 chongchong 2015-03-10
    ClientAttackConfig.Explosion_KeepAttackInterval := 5; // 持久伤害间隔 chongchong 2015-03-10
    ClientAttackConfig.Explosion_KeepLightRange := 0;

    ClientAttackConfig.Target_File := -1;
    ClientAttackConfig.Target_StartIndex := -1;
    ClientAttackConfig.Target_StartIndex2 := -1;
    ClientAttackConfig.Target_PlayCount := 0;
    ClientAttackConfig.Target_PlayTime := 100;
    ClientAttackConfig.Target_DrawMode := mdmBlend;
    ClientAttackConfig.Target_DrawMode2 := mdmBlend;
    ClientAttackConfig.Target_MultiPlay := False;
    ClientAttackConfig.Target_LockDraw := False;
    ClientAttackConfig.Target_LightRange := 0; // 照亮范围 chongchong 2015-03-04

    ClientAttackConfig.Target_KeepPlay := False; // 持久伤害 chongchong 2015-03-10
    ClientAttackConfig.Target_KeepTime := 30; // 持久伤害时间 chongchong 2015-03-10
    ClientAttackConfig.Target_KeepAttackRange := 0; // 持久伤害范围 chongchong 2015-03-10
    ClientAttackConfig.Target_KeepMultiPlay := False; // 持久伤害 每格单独播放 chongchong 2015-03-10
    ClientAttackConfig.Target_KeepAttackInterval := 5; // 持久伤害间隔 chongchong 2015-03-10
    ClientAttackConfig.Target_KeepLightRange := 0;
  end;

  ServerBaseConfig.ViewRange := 8;
  ServerBaseConfig.MonsterType := mtNormal;
  ServerBaseConfig.MoveOption := moMoveNormal;
  ServerBaseConfig.ProtectRange := 0;
  ServerBaseConfig.MinAttackNearRange := 1;
  ServerBaseConfig.LightRange := 0;
  ServerBaseConfig.NoAttack := False;

  for I := Low(MonsterServerConfigs) to High(MonsterServerConfigs) do
  begin
    MonsterServerConfig := @MonsterServerConfigs[I];

    MonsterServerConfig.AttackEnabled := False;
    MonsterServerConfig.OperateMode := momAttack;

    MonsterServerConfig.AttackSelfDie := False;
    MonsterServerConfig.AttackDelayTime := 400;

    // MonsterServerConfig.AttackWaitTime := 0;
    MonsterServerConfig.AttackHPPercent := 101;
    MonsterServerConfig.AttackRate := 100;
    MonsterServerConfig.AttackTargetCount := 0;
    MonsterServerConfig.AttackMode := mamNear;
    MonsterServerConfig.AttackTarget := matSingle;
    MonsterServerConfig.AttackPowerCalc := mapcDC;
    MonsterServerConfig.AttackPowerRate := 100;

    MonsterServerConfig.AttackTeleportAttack := False;
    MonsterServerConfig.AttackTeleportTargetDistance := 5;
    MonsterServerConfig.AttackTeleportDistance := 4;
    MonsterServerConfig.AttackTeleportRate := 0;
    MonsterServerConfig.AttackTeleportRush := False;

    MonsterServerConfig.AttackIgnoreDefence := False;

    MonsterServerConfig.AttackNearRange := 1;
    MonsterServerConfig.AttackGroupRange := 2;
    MonsterServerConfig.NearAttackTargetCenter := False;
    MonsterServerConfig.AttackPowerInc := 0;

    {
      MonsterServerConfig.HumanDefenceMagic := -1;
      MonsterServerConfig.HumanDefenceNewLevel := 0;
      MonsterServerConfig.HumanDefenceTime := 0;
      MonsterServerConfig.HumanDefenceHPPercent := 101;
      MonsterServerConfig.HumanAttackMagic := -1;
      MonsterServerConfig.HumanAttackNewLevel := 0;
      MonsterServerConfig.HumanAttackRate := 0;
      MonsterServerConfig.OnlyUseHumanMagic := False;
    }

    for J := Low(MonsterServerConfig.Additionals) to High(MonsterServerConfig.Additionals) do
    begin
      MonsterServerConfig.Additionals[J].Checked := False;
      MonsterServerConfig.Additionals[J].Rate := 3;
      MonsterServerConfig.Additionals[J].Time := 3;
    end;
    MonsterServerConfig.AdditionalHP0 := 3;
    MonsterServerConfig.AdditionalHighLevel4 := False;
    MonsterServerConfig.AdditionalImprisonRange := 3;

    MonsterServerConfig.EnabledCallMonster := False;
    MonsterServerConfig.CallMonstersRate := 0;
    for J := Low(MonsterServerConfig.CallMonsters) to High(MonsterServerConfig.CallMonsters) do
    begin
      MonsterServerConfig.CallMonsters[J] := '';
      MonsterServerConfig.CallMonsterNums[J] := 0;
    end;

    MonsterServerConfig.MoveTarget := False;
    MonsterServerConfig.MoveTargetRate := 0;
    MonsterServerConfig.MoveTargetHighLevel := False;
    {
      MonsterServerConfig.CopySelf := False;
      MonsterServerConfig.CopySelfMaxCount := 1;
      MonsterServerConfig.CopySelfTime := 10;
    }

    MonsterServerConfig.ProtectAddHP := False;
    MonsterServerConfig.ProtectAddHPRate := 0;
    MonsterServerConfig.ProtectAddHPPercent := 50;

    MonsterServerConfig.ProtectAddDefence := False;
    MonsterServerConfig.ProtectAddDefenceRate := 0;
    MonsterServerConfig.ProtectAddDefencePercent := 100;
    MonsterServerConfig.ProtectAddDefenceTime := 60;

    MonsterServerConfig.ProtectAddMagDefence := False;
    MonsterServerConfig.ProtectAddMagDefenceRate := 0;
    MonsterServerConfig.ProtectAddMagDefencePercent := 100;
    MonsterServerConfig.ProtectAddMagDefenceTime := 60;

    MonsterServerConfig.ProtectAddDC := False;
    MonsterServerConfig.ProtectAddDCRate := 0;
    MonsterServerConfig.ProtectAddDCPercent := 10;
    MonsterServerConfig.ProtectAddDCTime := 10;

    MonsterServerConfig.ProtectAddMC := False;
    MonsterServerConfig.ProtectAddMCRate := 0;
    MonsterServerConfig.ProtectAddMCPercent := 10;
    MonsterServerConfig.ProtectAddMCTime := 10;

    MonsterServerConfig.ProtectAddSC := False;
    MonsterServerConfig.ProtectAddSCRate := 0;
    MonsterServerConfig.ProtectAddSCPercent := 10;
    MonsterServerConfig.ProtectAddSCTime := 10;

    MonsterServerConfig.ProtectTargetRange := 3;
    MonsterServerConfig.ProtectSelfRate := 50;
  end;

  LoadFromIniFile;
end;

destructor TCustomMonsterConfig.Destroy;
begin
  inherited;
end;

procedure TCustomMonsterConfig.SetChanged(Value: Boolean);
begin
  FIsChanged := Value;
end;

procedure TCustomMonsterConfig.LoadFromIniFile;
var
  FileName, SectionName: string;
  IniFile: TIniFileEx;
  I, J, IntRead: Integer;
  ActionType: TMonsterClientActionType;
  ClientAction: PMonsterClientAction;
  ClientAttackConfig: PClientAttackConfig;
  MonsterServerConfig: PMonsterServerConfig;
  SoundType: TMonsterSoundType;
  // AttackDirType: TMonsterAttackDirType;
begin
  FIsChanged := False;

  FileName := g_Config.sSmartMonsterDir + FMonsterName + '.ini';
  if not FileExists(FileName) then
    Exit;
  IniFile := TIniFileEx.Create(FileName);
  try

    SectionName := 'ClientConfig';
    IntRead := IniFile.ReadInteger(SectionName, 'DrawMode', Integer(ClientBaseConfig.DrawMode));
    if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
      ClientBaseConfig.DrawMode := TCustomDrawMode(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'DrawMode2', Integer(ClientBaseConfig.DrawMode2));
    if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
      ClientBaseConfig.DrawMode2 := TCustomDrawMode(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'DrawOrder', Integer(ClientBaseConfig.DrawOrder));
    if (IntRead >= Integer(Low(TMonsterDrawOrder2))) and (IntRead <= Integer(High(TMonsterDrawOrder2))) then
      ClientBaseConfig.DrawOrder := TMonsterDrawOrder2(IntRead);

    ClientBaseConfig.DieNoCalcDir := IniFile.ReadBool(SectionName, 'DieNoCalcDir', ClientBaseConfig.DieNoCalcDir);

    ClientBaseConfig.HPBgOffsetX := IniFile.ReadInteger(SectionName, 'HPBgOffsetX', ClientBaseConfig.HPBgOffsetX);
    ClientBaseConfig.HPBgOffsetY := IniFile.ReadInteger(SectionName, 'HPBgOffsetY', ClientBaseConfig.HPBgOffsetY);

    ClientBaseConfig.HPOffsetX := IniFile.ReadInteger(SectionName, 'HPOffsetX', ClientBaseConfig.HPOffsetX);
    ClientBaseConfig.HPOffsetY := IniFile.ReadInteger(SectionName, 'HPOffsetY', ClientBaseConfig.HPOffsetY);

    ClientBaseConfig.HPFile := IniFile.ReadInteger(SectionName, 'HPFile', ClientBaseConfig.HPFile);
    ClientBaseConfig.HPStartIndex := IniFile.ReadInteger(SectionName, 'HPStartIndex', ClientBaseConfig.HPStartIndex);

    ClientBaseConfig.HPTextOffsetX := IniFile.ReadInteger(SectionName, 'HPTextOffsetX', ClientBaseConfig.HPTextOffsetX);
    ClientBaseConfig.HPTextOffsetY := IniFile.ReadInteger(SectionName, 'HPTextOffsetY', ClientBaseConfig.HPTextOffsetY);

    SectionName := 'ClientSounds';
    for SoundType := Low(TMonsterSoundType) to High(TMonsterSoundType) do
      ClientBaseConfig.Sounds[SoundType] := IniFile.ReadString(SectionName, MonsterSoundTypeNames[SoundType], '');

    for ActionType := Low(TMonsterClientActionType) to High(TMonsterClientActionType) do
    begin
      ClientAction := @ClientActions[ActionType];
      SectionName := MonsterClientActionSections[ActionType];

      ClientAction.ActionFile := IniFile.ReadInteger(SectionName, 'ActionFile', ClientAction.ActionFile);
      ClientAction.StartIndex := IniFile.ReadInteger(SectionName, 'StartIndex', ClientAction.StartIndex);
      ClientAction.PlayCount := IniFile.ReadInteger(SectionName, 'PlayCount', ClientAction.PlayCount);
      ClientAction.EmptyCount := IniFile.ReadInteger(SectionName, 'EmptyCount', ClientAction.EmptyCount);
      ClientAction.PlayTime := IniFile.ReadInteger(SectionName, 'PlayTime', ClientAction.PlayTime);
      ClientAction.EffectFile := IniFile.ReadInteger(SectionName, 'EffectFile', ClientAction.EffectFile);
      ClientAction.EffectIndex := IniFile.ReadInteger(SectionName, 'EffectIndex', ClientAction.EffectIndex);
      ClientAction.EffectFile2 := IniFile.ReadInteger(SectionName, 'EffectFile2', ClientAction.EffectFile2);
      ClientAction.EffectIndex2 := IniFile.ReadInteger(SectionName, 'EffectIndex2', ClientAction.EffectIndex2);
      ClientAction.CalcDir := IniFile.ReadBool(SectionName, 'CalcDir', ClientAction.CalcDir);
    end;

    for I := Low(ClientAttackConfigs) to High(ClientAttackConfigs) do
    begin
      ClientAttackConfig := @ClientAttackConfigs[I];
      SectionName := 'ClientAttack' + IntToStr(I);

      ClientAttackConfig.Fly_File := IniFile.ReadInteger(SectionName, 'Fly_File', ClientAttackConfig.Fly_File);
      ClientAttackConfig.Fly_StartIndex := IniFile.ReadInteger(SectionName, 'Fly_StartIndex', ClientAttackConfig.Fly_StartIndex);
      ClientAttackConfig.Fly_PlayCount := IniFile.ReadInteger(SectionName, 'Fly_PlayCount', ClientAttackConfig.Fly_PlayCount);
      ClientAttackConfig.Fly_EmptyCount := IniFile.ReadInteger(SectionName, 'Fly_EmptyCount', ClientAttackConfig.Fly_EmptyCount);
      ClientAttackConfig.Fly_PlayTime := IniFile.ReadInteger(SectionName, 'Fly_PlayTime', ClientAttackConfig.Fly_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'Fly_DrawMode', Integer(ClientAttackConfig.Fly_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.Fly_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Fly_DirCount', Integer(ClientAttackConfig.Fly_DirCount));
      if (IntRead >= Integer(Low(TCustomDirCount))) and (IntRead <= Integer(High(TCustomDirCount))) then
        ClientAttackConfig.Fly_DirCount := TCustomDirCount(IntRead);

      ClientAttackConfig.Fly_CalcDir := IniFile.ReadBool(SectionName, 'Fly_CalcDir', ClientAttackConfig.Fly_CalcDir);
      ClientAttackConfig.Fly_LightRange := IniFile.ReadInteger(SectionName, 'Fly_LightRange', ClientAttackConfig.Fly_LightRange);

      ClientAttackConfig.FlyEff_File := IniFile.ReadInteger(SectionName, 'FlyEff_File', ClientAttackConfig.FlyEff_File);
      ClientAttackConfig.FlyEff_StartIndex := IniFile.ReadInteger(SectionName, 'FlyEff_StartIndex',
        ClientAttackConfig.FlyEff_StartIndex);
      IntRead := IniFile.ReadInteger(SectionName, 'FlyEff_DrawMode', Integer(ClientAttackConfig.FlyEff_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.FlyEff_DrawMode := TCustomDrawMode(IntRead);

      ClientAttackConfig.Self_File := IniFile.ReadInteger(SectionName, 'Self_File', ClientAttackConfig.Self_File);
      ClientAttackConfig.Self_StartIndex := IniFile.ReadInteger(SectionName, 'Self_StartIndex',
        ClientAttackConfig.Self_StartIndex);
      ClientAttackConfig.Self_PlayCount := IniFile.ReadInteger(SectionName, 'Self_PlayCount', ClientAttackConfig.Self_PlayCount);
      ClientAttackConfig.Self_EmptyCount := IniFile.ReadInteger(SectionName, 'Self_EmptyCount',
        ClientAttackConfig.Self_EmptyCount);
      ClientAttackConfig.Self_PlayTime := IniFile.ReadInteger(SectionName, 'Self_PlayTime', ClientAttackConfig.Self_PlayTime);

      {
        IntRead := IniFile.ReadInteger(SectionName, 'Self_PlayMode', Integer(ClientAttackConfig.Self_PlayMode));
        if (IntRead >= Integer(Low(TMonsterPlayMode))) and (IntRead <= Integer(High(TMonsterPlayMode))) then
        ClientAttackConfig.Self_PlayMode := TMonsterPlayMode(IntRead);
      }

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DrawOrder', Integer(ClientAttackConfig.Self_DrawOrder));
      if (IntRead >= Integer(Low(TCustomDrawOrder))) and (IntRead <= Integer(High(TCustomDrawOrder))) then
        ClientAttackConfig.Self_DrawOrder := TCustomDrawOrder(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DrawMode', Integer(ClientAttackConfig.Self_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.Self_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DirCalcType', Integer(ClientAttackConfig.Self_DirCalcType));
      if (IntRead >= Integer(Low(TCustomDirCalcType))) and (IntRead <= Integer(High(TCustomDirCalcType))) then
        ClientAttackConfig.Self_DirCalcType := TCustomDirCalcType(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Self_DirCount', Integer(ClientAttackConfig.Self_DirCount));
      if (IntRead >= Integer(Low(TCustomDirCount))) and (IntRead <= Integer(High(TCustomDirCount))) then
        ClientAttackConfig.Self_DirCount := TCustomDirCount(IntRead);

      if IniFile.ValueExists(SectionName, 'Self_AttackDirType') then
      begin
        if IniFile.ReadBool(SectionName, 'Self_AttackDirType', False) then
        begin
          ClientAttackConfig.Self_DirCalcType := mdctCenter;

          IntRead := IniFile.ReadInteger(SectionName, 'Self_AttackDir', -1);
          if (IntRead >= Integer(Low(TCustomDirCount))) and (IntRead <= Integer(High(TCustomDirCount))) then
            ClientAttackConfig.Self_DirCount := TCustomDirCount(IntRead);
        end;
      end;

      ClientAttackConfig.Self_PlayDelayAction := IniFile.ReadBool(SectionName, 'Self_PlayDelayAction',
        ClientAttackConfig.Self_PlayDelayAction);
      ClientAttackConfig.Self_LightRange := IniFile.ReadInteger(SectionName, 'Self_LightRange',
        ClientAttackConfig.Self_LightRange);

      ClientAttackConfig.SelfKeep_File := IniFile.ReadInteger(SectionName, 'SelfKeep_File', ClientAttackConfig.SelfKeep_File);
      ClientAttackConfig.SelfKeep_StartIndex := IniFile.ReadInteger(SectionName, 'SelfKeep_StartIndex',
        ClientAttackConfig.SelfKeep_StartIndex);
      ClientAttackConfig.SelfKeep_StartIndex2 := IniFile.ReadInteger(SectionName, 'SelfKeep_StartIndex2',
        ClientAttackConfig.SelfKeep_StartIndex2);
      ClientAttackConfig.SelfKeep_PlayCount := IniFile.ReadInteger(SectionName, 'SelfKeep_PlayCount',
        ClientAttackConfig.SelfKeep_PlayCount);
      ClientAttackConfig.SelfKeep_PlayTime := IniFile.ReadInteger(SectionName, 'SelfKeep_PlayTime',
        ClientAttackConfig.SelfKeep_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'SelfKeep_DrawOrder', Integer(ClientAttackConfig.SelfKeep_DrawOrder));
      if (IntRead >= Integer(Low(TCustomDrawOrder))) and (IntRead <= Integer(High(TCustomDrawOrder))) then
        ClientAttackConfig.SelfKeep_DrawOrder := TCustomDrawOrder(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'SelfKeep_DrawMode', Integer(ClientAttackConfig.SelfKeep_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.SelfKeep_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'SelfKeep_DrawMode2', Integer(ClientAttackConfig.SelfKeep_DrawMode2));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.SelfKeep_DrawMode2 := TCustomDrawMode(IntRead);

      ClientAttackConfig.SelfKeep_KeepTime := IniFile.ReadInteger(SectionName, 'SelfKeep_KeepTime',
        ClientAttackConfig.SelfKeep_KeepTime);
      // ClientAttackConfig.SelfKeep_KeepTime2 := IniFile.ReadInteger(SectionName, 'SelfKeep_KeepTime2', ClientAttackConfig.SelfKeep_KeepTime2);

      ClientAttackConfig.Explosion_File := IniFile.ReadInteger(SectionName, 'Explosion_File', ClientAttackConfig.Explosion_File);
      ClientAttackConfig.Explosion_StartIndex := IniFile.ReadInteger(SectionName, 'Explosion_StartIndex',
        ClientAttackConfig.Explosion_StartIndex);
      ClientAttackConfig.Explosion_StartIndex2 := IniFile.ReadInteger(SectionName, 'Explosion_StartIndex2',
        ClientAttackConfig.Explosion_StartIndex2);
      ClientAttackConfig.Explosion_PlayCount := IniFile.ReadInteger(SectionName, 'Explosion_PlayCount',
        ClientAttackConfig.Explosion_PlayCount);
      ClientAttackConfig.Explosion_PlayTime := IniFile.ReadInteger(SectionName, 'Explosion_PlayTime',
        ClientAttackConfig.Explosion_PlayTime);

      IntRead := IniFile.ReadInteger(SectionName, 'Explosion_DrawMode', Integer(ClientAttackConfig.Explosion_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.Explosion_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Explosion_DrawMode2', Integer(ClientAttackConfig.Explosion_DrawMode2));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.Explosion_DrawMode2 := TCustomDrawMode(IntRead);

      ClientAttackConfig.Explosion_LockDraw := IniFile.ReadBool(SectionName, 'Explosion_LockDraw',
        ClientAttackConfig.Explosion_LockDraw);
      ClientAttackConfig.Explosion_LightRange := IniFile.ReadInteger(SectionName, 'Explosion_LightRange',
        ClientAttackConfig.Explosion_LightRange);

      ClientAttackConfig.Explosion_KeepPlay := IniFile.ReadBool(SectionName, 'Explosion_KeepPlay',
        ClientAttackConfig.Explosion_KeepPlay);
      ClientAttackConfig.Explosion_KeepTime := IniFile.ReadInteger(SectionName, 'Explosion_KeepTime',
        ClientAttackConfig.Explosion_KeepTime);
      ClientAttackConfig.Explosion_KeepAttackRange := IniFile.ReadInteger(SectionName, 'Explosion_KeepAttackRange',
        ClientAttackConfig.Explosion_KeepAttackRange);
      ClientAttackConfig.Explosion_KeepMultiPlay := IniFile.ReadBool(SectionName, 'Explosion_KeepMultiPlay',
        ClientAttackConfig.Explosion_KeepMultiPlay);
      ClientAttackConfig.Explosion_KeepAttackInterval := IniFile.ReadInteger(SectionName, 'Explosion_KeepAttackInterval',
        ClientAttackConfig.Explosion_KeepAttackInterval);
      ClientAttackConfig.Explosion_KeepLightRange := IniFile.ReadInteger(SectionName, 'Explosion_KeepLightRange',
        ClientAttackConfig.Explosion_KeepLightRange);

      ClientAttackConfig.Target_File := IniFile.ReadInteger(SectionName, 'Target_File', ClientAttackConfig.Target_File);
      ClientAttackConfig.Target_StartIndex := IniFile.ReadInteger(SectionName, 'Target_StartIndex',
        ClientAttackConfig.Target_StartIndex);
      ClientAttackConfig.Target_StartIndex2 := IniFile.ReadInteger(SectionName, 'Target_StartIndex2',
        ClientAttackConfig.Target_StartIndex2);
      ClientAttackConfig.Target_PlayCount := IniFile.ReadInteger(SectionName, 'Target_PlayCount',
        ClientAttackConfig.Target_PlayCount);
      ClientAttackConfig.Target_PlayTime := IniFile.ReadInteger(SectionName, 'Target_PlayTime',
        ClientAttackConfig.Target_PlayTime);
      IntRead := IniFile.ReadInteger(SectionName, 'Target_DrawMode', Integer(ClientAttackConfig.Target_DrawMode));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.Target_DrawMode := TCustomDrawMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'Target_DrawMode2', Integer(ClientAttackConfig.Target_DrawMode2));
      if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
        ClientAttackConfig.Target_DrawMode2 := TCustomDrawMode(IntRead);

      ClientAttackConfig.Target_MultiPlay := IniFile.ReadBool(SectionName, 'Target_MultiPlay',
        ClientAttackConfig.Target_MultiPlay);
      ClientAttackConfig.Target_LockDraw := IniFile.ReadBool(SectionName, 'Target_LockDraw', ClientAttackConfig.Target_LockDraw);
      ClientAttackConfig.Target_LightRange := IniFile.ReadInteger(SectionName, 'Target_LightRange',
        ClientAttackConfig.Target_LightRange);

      ClientAttackConfig.Target_KeepPlay := IniFile.ReadBool(SectionName, 'Target_KeepPlay', ClientAttackConfig.Target_KeepPlay);
      ClientAttackConfig.Target_KeepTime := IniFile.ReadInteger(SectionName, 'Target_KeepTime',
        ClientAttackConfig.Target_KeepTime);
      ClientAttackConfig.Target_KeepAttackRange := IniFile.ReadInteger(SectionName, 'Target_KeepAttackRange',
        ClientAttackConfig.Target_KeepAttackRange);
      ClientAttackConfig.Target_KeepMultiPlay := IniFile.ReadBool(SectionName, 'Target_KeepMultiPlay',
        ClientAttackConfig.Target_KeepMultiPlay);
      ClientAttackConfig.Target_KeepAttackInterval := IniFile.ReadInteger(SectionName, 'Target_KeepAttackInterval',
        ClientAttackConfig.Target_KeepAttackInterval);
      ClientAttackConfig.Target_KeepLightRange := IniFile.ReadInteger(SectionName, 'Target_KeepLightRange',
        ClientAttackConfig.Target_KeepLightRange);
    end;

    SectionName := 'ServerConfig';
    ServerBaseConfig.ViewRange := IniFile.ReadInteger(SectionName, 'ViewRange', ServerBaseConfig.ViewRange);

    IntRead := IniFile.ReadInteger(SectionName, 'MonsterType', Integer(ServerBaseConfig.MonsterType));
    if (IntRead >= Integer(Low(TMonsterType))) and (IntRead <= Integer(High(TMonsterType))) then
      ServerBaseConfig.MonsterType := TMonsterType(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'MoveOption', Integer(ServerBaseConfig.MoveOption));
    if (IntRead >= Integer(Low(TMoveOption))) and (IntRead <= Integer(High(TMoveOption))) then
      ServerBaseConfig.MoveOption := TMoveOption(IntRead);

    ServerBaseConfig.ProtectRange := IniFile.ReadInteger(SectionName, 'ProtectRange', ServerBaseConfig.ProtectRange);
    ServerBaseConfig.MinAttackNearRange := IniFile.ReadInteger(SectionName, 'MinAttackNearRange',
      ServerBaseConfig.MinAttackNearRange);
    ServerBaseConfig.LightRange := IniFile.ReadInteger(SectionName, 'LightRange', ServerBaseConfig.LightRange);
    ServerBaseConfig.NoAttack := IniFile.ReadBool(SectionName, 'NoAttack', ServerBaseConfig.NoAttack);

    for I := Low(MonsterServerConfigs) to High(MonsterServerConfigs) do
    begin
      MonsterServerConfig := @MonsterServerConfigs[I];
      SectionName := 'ServerAttack' + IntToStr(I);

      MonsterServerConfig.AttackEnabled := IniFile.ReadBool(SectionName, 'AttackEnabled', MonsterServerConfig.AttackEnabled);

      IntRead := IniFile.ReadInteger(SectionName, 'OperateMode', Integer(MonsterServerConfig.OperateMode));
      if (IntRead >= Integer(Low(TCustomOperateMode))) and (IntRead <= Integer(High(TCustomOperateMode))) then
        MonsterServerConfig.OperateMode := TCustomOperateMode(IntRead);

      MonsterServerConfig.AttackSelfDie := IniFile.ReadBool(SectionName, 'AttackSelfDie', MonsterServerConfig.AttackSelfDie);
      MonsterServerConfig.AttackDelayTime := IniFile.ReadInteger(SectionName, 'AttackDelayTime',
        MonsterServerConfig.AttackDelayTime);
      // MonsterServerConfig.AttackWaitTime := IniFile.ReadInteger(SectionName, 'AttackWaitTime', MonsterServerConfig.AttackWaitTime);
      MonsterServerConfig.AttackHPPercent := IniFile.ReadInteger(SectionName, 'AttackHPPercent',
        MonsterServerConfig.AttackHPPercent);
      MonsterServerConfig.AttackRate := IniFile.ReadInteger(SectionName, 'AttackRate', MonsterServerConfig.AttackRate);
      MonsterServerConfig.AttackTargetCount := IniFile.ReadInteger(SectionName, 'AttackTargetCount',
        MonsterServerConfig.AttackTargetCount);

      IntRead := IniFile.ReadInteger(SectionName, 'AttackMode', Integer(MonsterServerConfig.AttackMode));
      if (IntRead >= Integer(Low(TCustomAttackMode))) and (IntRead <= Integer(High(TCustomAttackMode))) then
        MonsterServerConfig.AttackMode := TCustomAttackMode(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'AttackTarget', Integer(MonsterServerConfig.AttackTarget));
      if (IntRead >= Integer(Low(TCustomAttackTarget))) and (IntRead <= Integer(High(TCustomAttackTarget))) then
        MonsterServerConfig.AttackTarget := TCustomAttackTarget(IntRead);

      IntRead := IniFile.ReadInteger(SectionName, 'AttackPowerCalc', Integer(MonsterServerConfig.AttackPowerCalc));
      if (IntRead >= Integer(Low(TCustomAttackPowerCalc))) and (IntRead <= Integer(High(TCustomAttackPowerCalc))) then
        MonsterServerConfig.AttackPowerCalc := TCustomAttackPowerCalc(IntRead);

      MonsterServerConfig.AttackPowerRate := IniFile.ReadInteger(SectionName, 'AttackPowerRate',
        MonsterServerConfig.AttackPowerRate);
      MonsterServerConfig.AttackTeleportAttack := IniFile.ReadBool(SectionName, 'AttackTeleportAttack',
        MonsterServerConfig.AttackTeleportAttack);
      MonsterServerConfig.AttackTeleportTargetDistance := IniFile.ReadInteger(SectionName, 'AttackTeleportTargetDistance',
        MonsterServerConfig.AttackTeleportTargetDistance);
      MonsterServerConfig.AttackTeleportDistance := IniFile.ReadInteger(SectionName, 'AttackTeleportDistance',
        MonsterServerConfig.AttackTeleportDistance);
      MonsterServerConfig.AttackTeleportRate := IniFile.ReadInteger(SectionName, 'AttackTeleportRate',
        MonsterServerConfig.AttackTeleportRate);
      MonsterServerConfig.AttackTeleportRush := IniFile.ReadBool(SectionName, 'AttackTeleportRush',
        MonsterServerConfig.AttackTeleportRush);

      MonsterServerConfig.AttackIgnoreDefence := IniFile.ReadBool(SectionName, 'AttackIgnoreDefence',
        MonsterServerConfig.AttackIgnoreDefence);

      MonsterServerConfig.AttackNearRange := IniFile.ReadInteger(SectionName, 'AttackNearRange',
        MonsterServerConfig.AttackNearRange);
      MonsterServerConfig.AttackGroupRange := IniFile.ReadInteger(SectionName, 'AttackGroupRange',
        MonsterServerConfig.AttackGroupRange);
      MonsterServerConfig.NearAttackTargetCenter := IniFile.ReadBool(SectionName, 'NearAttackTargetCenter',
        MonsterServerConfig.NearAttackTargetCenter);
      MonsterServerConfig.AttackPowerInc := IniFile.ReadInteger(SectionName, 'AttackPowerInc',
        MonsterServerConfig.AttackPowerInc);

      MonsterServerConfig.MoveTarget := IniFile.ReadBool(SectionName, 'MoveTarget', MonsterServerConfig.MoveTarget);
      MonsterServerConfig.MoveTargetRate := IniFile.ReadInteger(SectionName, 'MoveTargetRate',
        MonsterServerConfig.MoveTargetRate);
      MonsterServerConfig.MoveTargetHighLevel := IniFile.ReadBool(SectionName, 'MoveTargetHighLevel',
        MonsterServerConfig.MoveTargetHighLevel);

      SectionName := 'Additionals' + IntToStr(I);
      for J := Low(MonsterServerConfig.Additionals) to High(MonsterServerConfig.Additionals) do
      begin
        MonsterServerConfig.Additionals[J].Checked := IniFile.ReadBool(SectionName, 'Checked' + IntToStr(J),
          MonsterServerConfig.Additionals[J].Checked);
        MonsterServerConfig.Additionals[J].Rate := IniFile.ReadInteger(SectionName, 'Rate' + IntToStr(J),
          MonsterServerConfig.Additionals[J].Rate);
        MonsterServerConfig.Additionals[J].Time := IniFile.ReadInteger(SectionName, 'Time' + IntToStr(J),
          MonsterServerConfig.Additionals[J].Time);
      end;
      MonsterServerConfig.AdditionalHP0 := IniFile.ReadInteger(SectionName, 'HP0', MonsterServerConfig.AdditionalHP0);
      MonsterServerConfig.AdditionalHighLevel4 := IniFile.ReadBool(SectionName, 'HighLevel4',
        MonsterServerConfig.AdditionalHighLevel4);
      MonsterServerConfig.AdditionalImprisonRange := IniFile.ReadInteger(SectionName, 'AdditionalImprisonRange',
        MonsterServerConfig.AdditionalImprisonRange);

      SectionName := 'CallMonster' + IntToStr(I);
      MonsterServerConfig.EnabledCallMonster := IniFile.ReadBool(SectionName, 'EnabledCallMonster',
        MonsterServerConfig.EnabledCallMonster);
      MonsterServerConfig.CallMonstersRate := IniFile.ReadInteger(SectionName, 'CallMonstersRate',
        MonsterServerConfig.CallMonstersRate);
      for J := Low(MonsterServerConfig.CallMonsters) to High(MonsterServerConfig.CallMonsters) do
      begin
        MonsterServerConfig.CallMonsters[J] := IniFile.ReadString(SectionName, 'MonsterName' + IntToStr(J),
          MonsterServerConfig.CallMonsters[J]);
        MonsterServerConfig.CallMonsterNums[J] := IniFile.ReadInteger(SectionName, 'MonsterNum' + IntToStr(J),
          MonsterServerConfig.CallMonsterNums[J]);
      end;

      {
        SectionName := 'CopySelf' + IntToStr(I);
        MonsterServerConfig.CopySelf := IniFile.ReadBool(SectionName, 'IsCreate', MonsterServerConfig.CopySelf);
        MonsterServerConfig.CopySelfMaxCount := IniFile.ReadInteger(SectionName, 'MaxCount', MonsterServerConfig.CopySelfMaxCount);
        MonsterServerConfig.CopySelfTime := IniFile.ReadInteger(SectionName, 'Time', MonsterServerConfig.CopySelfTime);
      }

      SectionName := 'Protect' + IntToStr(I);
      MonsterServerConfig.ProtectAddHP := IniFile.ReadBool(SectionName, 'ProtectAddHP', MonsterServerConfig.ProtectAddHP);
      MonsterServerConfig.ProtectAddHPRate := IniFile.ReadInteger(SectionName, 'ProtectAddHPRate',
        MonsterServerConfig.ProtectAddHPRate);
      MonsterServerConfig.ProtectAddHPPercent := IniFile.ReadInteger(SectionName, 'ProtectAddHPPercent',
        MonsterServerConfig.ProtectAddHPPercent);

      MonsterServerConfig.ProtectAddDefence := IniFile.ReadBool(SectionName, 'ProtectAddDefence',
        MonsterServerConfig.ProtectAddDefence);
      MonsterServerConfig.ProtectAddDefenceRate := IniFile.ReadInteger(SectionName, 'ProtectAddDefenceRate',
        MonsterServerConfig.ProtectAddDefenceRate);
      MonsterServerConfig.ProtectAddDefencePercent := IniFile.ReadInteger(SectionName, 'ProtectAddDefencePercent',
        MonsterServerConfig.ProtectAddDefencePercent);
      MonsterServerConfig.ProtectAddDefenceTime := IniFile.ReadInteger(SectionName, 'ProtectAddDefenceTime',
        MonsterServerConfig.ProtectAddDefenceTime);

      MonsterServerConfig.ProtectAddMagDefence := IniFile.ReadBool(SectionName, 'ProtectAddMagDefence',
        MonsterServerConfig.ProtectAddMagDefence);
      MonsterServerConfig.ProtectAddMagDefenceRate := IniFile.ReadInteger(SectionName, 'ProtectAddMagDefenceRate',
        MonsterServerConfig.ProtectAddMagDefenceRate);
      MonsterServerConfig.ProtectAddMagDefencePercent := IniFile.ReadInteger(SectionName, 'ProtectAddMagDefencePercent',
        MonsterServerConfig.ProtectAddMagDefencePercent);
      MonsterServerConfig.ProtectAddMagDefenceTime := IniFile.ReadInteger(SectionName, 'ProtectAddMagDefenceTime',
        MonsterServerConfig.ProtectAddMagDefenceTime);

      MonsterServerConfig.ProtectAddDC := IniFile.ReadBool(SectionName, 'ProtectAddDC', MonsterServerConfig.ProtectAddDC);
      MonsterServerConfig.ProtectAddDCRate := IniFile.ReadInteger(SectionName, 'ProtectAddDCRate',
        MonsterServerConfig.ProtectAddDCRate);
      MonsterServerConfig.ProtectAddDCPercent := IniFile.ReadInteger(SectionName, 'ProtectAddDCPercent',
        MonsterServerConfig.ProtectAddDCPercent);
      MonsterServerConfig.ProtectAddDCTime := IniFile.ReadInteger(SectionName, 'ProtectAddDCTime',
        MonsterServerConfig.ProtectAddDCTime);

      MonsterServerConfig.ProtectAddMC := IniFile.ReadBool(SectionName, 'ProtectAddMC', MonsterServerConfig.ProtectAddMC);
      MonsterServerConfig.ProtectAddMCRate := IniFile.ReadInteger(SectionName, 'ProtectAddMCRate',
        MonsterServerConfig.ProtectAddMCRate);
      MonsterServerConfig.ProtectAddMCPercent := IniFile.ReadInteger(SectionName, 'ProtectAddMCPercent',
        MonsterServerConfig.ProtectAddMCPercent);
      MonsterServerConfig.ProtectAddMCTime := IniFile.ReadInteger(SectionName, 'ProtectAddMCTime',
        MonsterServerConfig.ProtectAddMCTime);

      MonsterServerConfig.ProtectAddSC := IniFile.ReadBool(SectionName, 'ProtectAddSC', MonsterServerConfig.ProtectAddSC);
      MonsterServerConfig.ProtectAddSCRate := IniFile.ReadInteger(SectionName, 'ProtectAddSCRate',
        MonsterServerConfig.ProtectAddSCRate);
      MonsterServerConfig.ProtectAddSCPercent := IniFile.ReadInteger(SectionName, 'ProtectAddSCPercent',
        MonsterServerConfig.ProtectAddSCPercent);
      MonsterServerConfig.ProtectAddSCTime := IniFile.ReadInteger(SectionName, 'ProtectAddSCTime',
        MonsterServerConfig.ProtectAddSCTime);

      MonsterServerConfig.ProtectTargetRange := IniFile.ReadInteger(SectionName, 'ProtectTargetRange',
        MonsterServerConfig.ProtectTargetRange);
      MonsterServerConfig.ProtectSelfRate := IniFile.ReadInteger(SectionName, 'ProtectSelfRate',
        MonsterServerConfig.ProtectSelfRate);
    end;
  finally
    IniFile.Free;
  end;
end;

procedure TCustomMonsterConfig.SaveToIniFile;
var
  FileName, SectionName: string;
  IniFile: TIniFileEx;
  I, J: Integer;
  ActionType: TMonsterClientActionType;
  ClientAction: PMonsterClientAction;
  ClientAttackConfig: PClientAttackConfig;
  MonsterServerConfig: PMonsterServerConfig;

  SoundType: TMonsterSoundType;
begin
  FIsChanged := False;

  if not DirectoryExists(g_Config.sSmartMonsterDir) then
    ForceDirectories(g_Config.sSmartMonsterDir);

  FileName := g_Config.sSmartMonsterDir + FMonsterName + '.ini';
  IniFile := TIniFileEx.Create(FileName);
  try
    SectionName := 'ClientConfig';
    IniFile.WriteInteger(SectionName, 'DrawMode', Integer(ClientBaseConfig.DrawMode));
    IniFile.WriteInteger(SectionName, 'DrawMode2', Integer(ClientBaseConfig.DrawMode2));
    IniFile.WriteInteger(SectionName, 'DrawOrder', Integer(ClientBaseConfig.DrawOrder));
    IniFile.WriteInteger(SectionName, 'DieNoCalcDir', Integer(ClientBaseConfig.DieNoCalcDir));
    IniFile.WriteInteger(SectionName, 'HPBgOffsetX', ClientBaseConfig.HPBgOffsetX);
    IniFile.WriteInteger(SectionName, 'HPBgOffsetY', ClientBaseConfig.HPBgOffsetY);
    IniFile.WriteInteger(SectionName, 'HPOffsetX', ClientBaseConfig.HPOffsetX);
    IniFile.WriteInteger(SectionName, 'HPOffsetY', ClientBaseConfig.HPOffsetY);
    IniFile.WriteInteger(SectionName, 'HPFile', ClientBaseConfig.HPFile);
    IniFile.WriteInteger(SectionName, 'HPStartIndex', ClientBaseConfig.HPStartIndex);

    IniFile.WriteInteger(SectionName, 'HPTextOffsetX', ClientBaseConfig.HPTextOffsetX);
    IniFile.WriteInteger(SectionName, 'HPTextOffsetY', ClientBaseConfig.HPTextOffsetY);

    SectionName := 'ClientSounds';
    for SoundType := Low(TMonsterSoundType) to High(TMonsterSoundType) do
      IniFile.WriteString(SectionName, MonsterSoundTypeNames[SoundType], ClientBaseConfig.Sounds[SoundType]);

    for ActionType := Low(TMonsterClientActionType) to High(TMonsterClientActionType) do
    begin
      ClientAction := @ClientActions[ActionType];
      SectionName := MonsterClientActionSections[ActionType];

      IniFile.WriteInteger(SectionName, 'ActionFile', ClientAction.ActionFile);
      IniFile.WriteInteger(SectionName, 'StartIndex', ClientAction.StartIndex);
      IniFile.WriteInteger(SectionName, 'PlayCount', ClientAction.PlayCount);
      IniFile.WriteInteger(SectionName, 'EmptyCount', ClientAction.EmptyCount);
      IniFile.WriteInteger(SectionName, 'PlayTime', ClientAction.PlayTime);
      IniFile.WriteInteger(SectionName, 'EffectFile', ClientAction.EffectFile);
      IniFile.WriteInteger(SectionName, 'EffectIndex', ClientAction.EffectIndex);
      IniFile.WriteInteger(SectionName, 'EffectFile2', ClientAction.EffectFile2);
      IniFile.WriteInteger(SectionName, 'EffectIndex2', ClientAction.EffectIndex2);
      IniFile.WriteBool(SectionName, 'CalcDir', ClientAction.CalcDir);
    end;

    for I := Low(ClientAttackConfigs) to High(ClientAttackConfigs) do
    begin
      ClientAttackConfig := @ClientAttackConfigs[I];
      SectionName := 'ClientAttack' + IntToStr(I);

      IniFile.WriteInteger(SectionName, 'Fly_File', ClientAttackConfig.Fly_File);
      IniFile.WriteInteger(SectionName, 'Fly_StartIndex', ClientAttackConfig.Fly_StartIndex);
      IniFile.WriteInteger(SectionName, 'Fly_PlayCount', ClientAttackConfig.Fly_PlayCount);
      IniFile.WriteInteger(SectionName, 'Fly_EmptyCount', ClientAttackConfig.Fly_EmptyCount);
      IniFile.WriteInteger(SectionName, 'Fly_PlayTime', ClientAttackConfig.Fly_PlayTime);
      IniFile.WriteInteger(SectionName, 'Fly_DrawMode', Integer(ClientAttackConfig.Fly_DrawMode));
      IniFile.WriteInteger(SectionName, 'Fly_DirCount', Integer(ClientAttackConfig.Fly_DirCount));
      IniFile.WriteBool(SectionName, 'Fly_CalcDir', ClientAttackConfig.Fly_CalcDir);
      IniFile.WriteInteger(SectionName, 'Fly_LightRange', ClientAttackConfig.Fly_LightRange);

      IniFile.WriteInteger(SectionName, 'FlyEff_File', ClientAttackConfig.FlyEff_File);
      IniFile.WriteInteger(SectionName, 'FlyEff_StartIndex', ClientAttackConfig.FlyEff_StartIndex);
      IniFile.WriteInteger(SectionName, 'FlyEff_DrawMode', Integer(ClientAttackConfig.FlyEff_DrawMode));

      IniFile.WriteInteger(SectionName, 'Self_File', ClientAttackConfig.Self_File);
      IniFile.WriteInteger(SectionName, 'Self_StartIndex', ClientAttackConfig.Self_StartIndex);
      IniFile.WriteInteger(SectionName, 'Self_PlayCount', ClientAttackConfig.Self_PlayCount);
      IniFile.WriteInteger(SectionName, 'Self_EmptyCount', ClientAttackConfig.Self_EmptyCount);
      IniFile.WriteInteger(SectionName, 'Self_PlayTime', ClientAttackConfig.Self_PlayTime);

      // IniFile.WriteInteger(SectionName, 'Self_PlayMode', Integer(ClientAttackConfig.Self_PlayMode));
      IniFile.WriteInteger(SectionName, 'Self_DrawOrder', Integer(ClientAttackConfig.Self_DrawOrder));
      IniFile.WriteInteger(SectionName, 'Self_DrawMode', Integer(ClientAttackConfig.Self_DrawMode));

      IniFile.WriteInteger(SectionName, 'Self_DirCalcType', Integer(ClientAttackConfig.Self_DirCalcType));
      IniFile.WriteInteger(SectionName, 'Self_DirCount', Integer(ClientAttackConfig.Self_DirCount));

      IniFile.DeleteKey(SectionName, 'Self_AttackDir');
      IniFile.DeleteKey(SectionName, 'Self_AttackDirType');

      IniFile.WriteBool(SectionName, 'Self_PlayDelayAction', ClientAttackConfig.Self_PlayDelayAction);
      IniFile.WriteInteger(SectionName, 'Self_LightRange', ClientAttackConfig.Self_LightRange);

      IniFile.WriteInteger(SectionName, 'SelfKeep_File', ClientAttackConfig.SelfKeep_File);
      IniFile.WriteInteger(SectionName, 'SelfKeep_StartIndex', ClientAttackConfig.SelfKeep_StartIndex);
      IniFile.WriteInteger(SectionName, 'SelfKeep_StartIndex2', ClientAttackConfig.SelfKeep_StartIndex2);
      IniFile.WriteInteger(SectionName, 'SelfKeep_PlayCount', ClientAttackConfig.SelfKeep_PlayCount);
      IniFile.WriteInteger(SectionName, 'SelfKeep_PlayTime', ClientAttackConfig.SelfKeep_PlayTime);
      IniFile.WriteInteger(SectionName, 'SelfKeep_DrawMode', Integer(ClientAttackConfig.SelfKeep_DrawMode));
      IniFile.WriteInteger(SectionName, 'SelfKeep_KeepTime', ClientAttackConfig.SelfKeep_KeepTime);
      IniFile.WriteInteger(SectionName, 'SelfKeep_DrawOrder', Integer(ClientAttackConfig.SelfKeep_DrawOrder));
      IniFile.WriteInteger(SectionName, 'SelfKeep_DrawMode2', Integer(ClientAttackConfig.SelfKeep_DrawMode2));

      IniFile.WriteInteger(SectionName, 'Explosion_File', ClientAttackConfig.Explosion_File);
      IniFile.WriteInteger(SectionName, 'Explosion_StartIndex', ClientAttackConfig.Explosion_StartIndex);
      IniFile.WriteInteger(SectionName, 'Explosion_StartIndex2', ClientAttackConfig.Explosion_StartIndex2);
      IniFile.WriteInteger(SectionName, 'Explosion_PlayCount', ClientAttackConfig.Explosion_PlayCount);
      IniFile.WriteInteger(SectionName, 'Explosion_PlayTime', ClientAttackConfig.Explosion_PlayTime);
      IniFile.WriteInteger(SectionName, 'Explosion_DrawMode', Integer(ClientAttackConfig.Explosion_DrawMode));
      IniFile.WriteInteger(SectionName, 'Explosion_DrawMode2', Integer(ClientAttackConfig.Explosion_DrawMode2));
      IniFile.WriteBool(SectionName, 'Explosion_LockDraw', ClientAttackConfig.Explosion_LockDraw);
      IniFile.WriteInteger(SectionName, 'Explosion_LightRange', ClientAttackConfig.Explosion_LightRange);

      IniFile.WriteBool(SectionName, 'Explosion_KeepPlay', ClientAttackConfig.Explosion_KeepPlay);
      IniFile.WriteInteger(SectionName, 'Explosion_KeepTime', ClientAttackConfig.Explosion_KeepTime);
      IniFile.WriteInteger(SectionName, 'Explosion_KeepAttackRange', ClientAttackConfig.Explosion_KeepAttackRange);
      IniFile.WriteBool(SectionName, 'Explosion_KeepMultiPlay', ClientAttackConfig.Explosion_KeepMultiPlay);
      IniFile.WriteInteger(SectionName, 'Explosion_KeepAttackInterval', ClientAttackConfig.Explosion_KeepAttackInterval);
      IniFile.WriteInteger(SectionName, 'Explosion_KeepLightRange', ClientAttackConfig.Explosion_KeepLightRange);

      IniFile.WriteInteger(SectionName, 'Target_File', ClientAttackConfig.Target_File);
      IniFile.WriteInteger(SectionName, 'Target_StartIndex', ClientAttackConfig.Target_StartIndex);
      IniFile.WriteInteger(SectionName, 'Target_StartIndex2', ClientAttackConfig.Target_StartIndex2);
      IniFile.WriteInteger(SectionName, 'Target_PlayCount', ClientAttackConfig.Target_PlayCount);
      IniFile.WriteInteger(SectionName, 'Target_PlayTime', ClientAttackConfig.Target_PlayTime);
      IniFile.WriteInteger(SectionName, 'Target_DrawMode', Integer(ClientAttackConfig.Target_DrawMode));
      IniFile.WriteInteger(SectionName, 'Target_DrawMode2', Integer(ClientAttackConfig.Target_DrawMode2));
      IniFile.WriteBool(SectionName, 'Target_MultiPlay', ClientAttackConfig.Target_MultiPlay);
      IniFile.WriteBool(SectionName, 'Target_LockDraw', ClientAttackConfig.Target_LockDraw);
      IniFile.WriteInteger(SectionName, 'Target_LightRange', ClientAttackConfig.Target_LightRange);

      IniFile.WriteBool(SectionName, 'Target_KeepPlay', ClientAttackConfig.Target_KeepPlay);
      IniFile.WriteInteger(SectionName, 'Target_KeepTime', ClientAttackConfig.Target_KeepTime);
      IniFile.WriteInteger(SectionName, 'Target_KeepAttackRange', ClientAttackConfig.Target_KeepAttackRange);
      IniFile.WriteBool(SectionName, 'Target_KeepMultiPlay', ClientAttackConfig.Target_KeepMultiPlay);
      IniFile.WriteInteger(SectionName, 'Target_KeepAttackInterval', ClientAttackConfig.Target_KeepAttackInterval);
      IniFile.WriteInteger(SectionName, 'Target_KeepLightRange', ClientAttackConfig.Target_KeepLightRange);
    end;

    SectionName := 'ServerConfig';
    IniFile.WriteInteger(SectionName, 'ViewRange', ServerBaseConfig.ViewRange);
    IniFile.WriteInteger(SectionName, 'MonsterType', Integer(ServerBaseConfig.MonsterType));
    IniFile.WriteInteger(SectionName, 'MoveOption', Integer(ServerBaseConfig.MoveOption));
    IniFile.WriteInteger(SectionName, 'ProtectRange', ServerBaseConfig.ProtectRange);
    IniFile.WriteInteger(SectionName, 'MinAttackNearRange', ServerBaseConfig.MinAttackNearRange);
    IniFile.WriteInteger(SectionName, 'LightRange', ServerBaseConfig.LightRange);
    IniFile.WriteBool(SectionName, 'NoAttack', ServerBaseConfig.NoAttack);

    for I := Low(MonsterServerConfigs) to High(MonsterServerConfigs) do
    begin
      MonsterServerConfig := @MonsterServerConfigs[I];
      SectionName := 'ServerAttack' + IntToStr(I);

      IniFile.WriteBool(SectionName, 'AttackEnabled', MonsterServerConfig.AttackEnabled);
      IniFile.WriteInteger(SectionName, 'OperateMode', Integer(MonsterServerConfig.OperateMode));

      IniFile.WriteBool(SectionName, 'AttackSelfDie', MonsterServerConfig.AttackSelfDie);
      IniFile.WriteInteger(SectionName, 'AttackDelayTime', MonsterServerConfig.AttackDelayTime);

      // IniFile.WriteInteger(SectionName, 'AttackWaitTime', MonsterServerConfig.AttackWaitTime);
      IniFile.WriteInteger(SectionName, 'AttackHPPercent', MonsterServerConfig.AttackHPPercent);
      IniFile.WriteInteger(SectionName, 'AttackRate', MonsterServerConfig.AttackRate);
      IniFile.WriteInteger(SectionName, 'AttackTargetCount', MonsterServerConfig.AttackTargetCount);

      IniFile.WriteInteger(SectionName, 'AttackMode', Integer(MonsterServerConfig.AttackMode));
      IniFile.WriteInteger(SectionName, 'AttackTarget', Integer(MonsterServerConfig.AttackTarget));
      IniFile.WriteInteger(SectionName, 'AttackPowerCalc', Integer(MonsterServerConfig.AttackPowerCalc));
      IniFile.WriteInteger(SectionName, 'AttackPowerRate', MonsterServerConfig.AttackPowerRate);
      IniFile.WriteBool(SectionName, 'AttackTeleportAttack', MonsterServerConfig.AttackTeleportAttack);
      IniFile.WriteInteger(SectionName, 'AttackTeleportTargetDistance', MonsterServerConfig.AttackTeleportTargetDistance);
      IniFile.WriteInteger(SectionName, 'AttackTeleportDistance', MonsterServerConfig.AttackTeleportDistance);
      IniFile.WriteInteger(SectionName, 'AttackTeleportRate', MonsterServerConfig.AttackTeleportRate);
      IniFile.WriteBool(SectionName, 'AttackTeleportRush', MonsterServerConfig.AttackTeleportRush);

      IniFile.WriteBool(SectionName, 'AttackIgnoreDefence', MonsterServerConfig.AttackIgnoreDefence);

      IniFile.WriteInteger(SectionName, 'AttackNearRange', MonsterServerConfig.AttackNearRange);
      IniFile.WriteInteger(SectionName, 'AttackGroupRange', MonsterServerConfig.AttackGroupRange);
      IniFile.WriteBool(SectionName, 'NearAttackTargetCenter', MonsterServerConfig.NearAttackTargetCenter);
      IniFile.WriteInteger(SectionName, 'AttackPowerInc', MonsterServerConfig.AttackPowerInc);

      IniFile.WriteBool(SectionName, 'MoveTarget', MonsterServerConfig.MoveTarget);
      IniFile.WriteInteger(SectionName, 'MoveTargetRate', MonsterServerConfig.MoveTargetRate);
      IniFile.WriteBool(SectionName, 'MoveTargetHighLevel', MonsterServerConfig.MoveTargetHighLevel);

      {
        IniFile.WriteInteger(SectionName, 'HumanDefenceMagic', MonsterServerConfig.HumanDefenceMagic);
        IniFile.WriteInteger(SectionName, 'HumanDefenceNewLevel', MonsterServerConfig.HumanDefenceNewLevel);
        IniFile.WriteInteger(SectionName, 'HumanDefenceTime', MonsterServerConfig.HumanDefenceTime);
        IniFile.WriteInteger(SectionName, 'HumanDefenceHPPercent', MonsterServerConfig.HumanDefenceHPPercent);
        IniFile.WriteInteger(SectionName, 'HumanAttackMagic', MonsterServerConfig.HumanAttackMagic);
        IniFile.WriteInteger(SectionName, 'HumanAttackNewLevel', MonsterServerConfig.HumanAttackNewLevel);
        IniFile.WriteInteger(SectionName, 'HumanAttackRate', MonsterServerConfig.HumanAttackRate);
        IniFile.WriteBool(SectionName, 'OnlyUseHumanMagic', MonsterServerConfig.OnlyUseHumanMagic);
      }

      SectionName := 'Additionals' + IntToStr(I);
      for J := Low(MonsterServerConfig.Additionals) to High(MonsterServerConfig.Additionals) do
      begin
        IniFile.WriteBool(SectionName, 'Checked' + IntToStr(J), MonsterServerConfig.Additionals[J].Checked);
        IniFile.WriteInteger(SectionName, 'Rate' + IntToStr(J), MonsterServerConfig.Additionals[J].Rate);
        IniFile.WriteInteger(SectionName, 'Time' + IntToStr(J), MonsterServerConfig.Additionals[J].Time);
      end;
      IniFile.WriteInteger(SectionName, 'HP0', MonsterServerConfig.AdditionalHP0);
      IniFile.WriteBool(SectionName, 'HighLevel4', MonsterServerConfig.AdditionalHighLevel4);
      IniFile.WriteInteger(SectionName, 'AdditionalImprisonRange', MonsterServerConfig.AdditionalImprisonRange);

      SectionName := 'CallMonster' + IntToStr(I);
      IniFile.WriteBool(SectionName, 'EnabledCallMonster', MonsterServerConfig.EnabledCallMonster);
      IniFile.WriteInteger(SectionName, 'CallMonstersRate', MonsterServerConfig.CallMonstersRate);
      for J := Low(MonsterServerConfig.CallMonsters) to High(MonsterServerConfig.CallMonsters) do
      begin
        IniFile.WriteString(SectionName, 'MonsterName' + IntToStr(J), MonsterServerConfig.CallMonsters[J]);
        IniFile.WriteInteger(SectionName, 'MonsterNum' + IntToStr(J), MonsterServerConfig.CallMonsterNums[J]);
      end;

      {
        SectionName := 'CopySelf' + IntToStr(I);
        IniFile.WriteBool(SectionName, 'IsCreate', MonsterServerConfig.CopySelf);
        IniFile.WriteInteger(SectionName, 'MaxCount', MonsterServerConfig.CopySelfMaxCount);
        IniFile.WriteInteger(SectionName, 'Time', MonsterServerConfig.CopySelfTime);
      }

      SectionName := 'Protect' + IntToStr(I);
      IniFile.WriteBool(SectionName, 'ProtectAddHP', MonsterServerConfig.ProtectAddHP);
      IniFile.WriteInteger(SectionName, 'ProtectAddHPRate', MonsterServerConfig.ProtectAddHPRate);
      IniFile.WriteInteger(SectionName, 'ProtectAddHPPercent', MonsterServerConfig.ProtectAddHPPercent);

      IniFile.WriteBool(SectionName, 'ProtectAddDefence', MonsterServerConfig.ProtectAddDefence);
      IniFile.WriteInteger(SectionName, 'ProtectAddDefenceRate', MonsterServerConfig.ProtectAddDefenceRate);
      IniFile.WriteInteger(SectionName, 'ProtectAddDefencePercent', MonsterServerConfig.ProtectAddDefencePercent);
      IniFile.WriteInteger(SectionName, 'ProtectAddDefenceTime', MonsterServerConfig.ProtectAddDefenceTime);

      IniFile.WriteBool(SectionName, 'ProtectAddMagDefence', MonsterServerConfig.ProtectAddMagDefence);
      IniFile.WriteInteger(SectionName, 'ProtectAddMagDefenceRate', MonsterServerConfig.ProtectAddMagDefenceRate);
      IniFile.WriteInteger(SectionName, 'ProtectAddMagDefencePercent', MonsterServerConfig.ProtectAddMagDefencePercent);
      IniFile.WriteInteger(SectionName, 'ProtectAddMagDefenceTime', MonsterServerConfig.ProtectAddMagDefenceTime);

      IniFile.WriteBool(SectionName, 'ProtectAddDC', MonsterServerConfig.ProtectAddDC);
      IniFile.WriteInteger(SectionName, 'ProtectAddDCRate', MonsterServerConfig.ProtectAddDCRate);
      IniFile.WriteInteger(SectionName, 'ProtectAddDCPercent', MonsterServerConfig.ProtectAddDCPercent);
      IniFile.WriteInteger(SectionName, 'ProtectAddDCTime', MonsterServerConfig.ProtectAddDCTime);

      IniFile.WriteBool(SectionName, 'ProtectAddMC', MonsterServerConfig.ProtectAddMC);
      IniFile.WriteInteger(SectionName, 'ProtectAddMCRate', MonsterServerConfig.ProtectAddMCRate);
      IniFile.WriteInteger(SectionName, 'ProtectAddMCPercent', MonsterServerConfig.ProtectAddMCPercent);
      IniFile.WriteInteger(SectionName, 'ProtectAddMCTime', MonsterServerConfig.ProtectAddMCTime);

      IniFile.WriteBool(SectionName, 'ProtectAddSC', MonsterServerConfig.ProtectAddSC);
      IniFile.WriteInteger(SectionName, 'ProtectAddSCRate', MonsterServerConfig.ProtectAddSCRate);
      IniFile.WriteInteger(SectionName, 'ProtectAddSCPercent', MonsterServerConfig.ProtectAddSCPercent);
      IniFile.WriteInteger(SectionName, 'ProtectAddSCTime', MonsterServerConfig.ProtectAddSCTime);

      IniFile.WriteInteger(SectionName, 'ProtectTargetRange', MonsterServerConfig.ProtectTargetRange);
      IniFile.WriteInteger(SectionName, 'ProtectSelfRate', MonsterServerConfig.ProtectSelfRate);
    end;
  finally
    IniFile.Free;
  end;
end;

end.
