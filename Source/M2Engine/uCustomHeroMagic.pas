unit uCustomHeroMagic;

interface

uses
  Windows, Classes, SysUtils, FastIniFile;

type
  TMagicType = (mtWarrAttack, mtWizardAttack, mtTaosAttack);
  TMagicAttackTarget = (matEnemy, matSelf, matMaster, matPartner);

  TCompareSymbol = (csLess, csLessOrEqual, csEqual, csGreater, csGreaterorEqual);
  THeroLevelCompareType = (hlctTargetLevel, hlctLevelNumber);
  THeroHPCompareType = (hhpctNumber, hhpctPercentage);

const
  //TMagicTypeNames = (mtWarrAttack, mtWizardAttack, mtTaosAttack);
  TMagicAttackTargetNames: array[TMagicAttackTarget] of string  = ('敌人', '自己', '主人', '伙伴');

  TCompareSymbolNames: array[TCompareSymbol] of string = ('<', '<=', '=', '>', '>=');
  THeroLevelCompareTypeNames: array[THeroLevelCompareType] of string = ('目标等级', '固定等级');
  THeroHPCompareTypeNames: array[THeroHPCompareType] of string = ('固定值', '百分比');

type
  THeroLevelCheck = record
    boChecked: Boolean;
    CompareSymbol: TCompareSymbol;
    CompareType: THeroLevelCompareType;
    CompareValue: LongWord;
  end;

  THeroHPCheck = record
    boChecked: Boolean;
    CompareSymbol: TCompareSymbol;
    CompareType: THeroHPCompareType;
    CompareValue: LongWord;
  end;

  TTargetStatusCheck = record
    boPoisonDamageArmor: Boolean;
    boPoisonDecHealth: Boolean;
    boPoisoning: Boolean;
    boPoisonStone: Boolean;
    boFrozen: Boolean;
    boForeverFrozen: Boolean;
    boCobwebWinding: Boolean;

    boUnPoisonDamageArmor: Boolean;
    boUnPoisonDecHealth: Boolean;
    boUnPoisoning: Boolean;
    boUnPoisonStone: Boolean;
    boUnFrozen: Boolean;
    boUnForeverFrozen: Boolean;
    boUnCobwebWinding: Boolean;
  end;

  TActorCountCheck = record
    boChecked: Boolean;
    nCheckRange: Integer;
    nCheckValue: Integer;
  end;

  PHeroMagicUseCondition = ^THeroMagicUseCondition;
  THeroMagicUseCondition = record
    HeroLevelCheck: THeroLevelCheck;
    HeroHPCheck: THeroHPCheck;
    HeroMPCheck: THeroHPCheck;
    TargetHPCheck: THeroHPCheck;
    TargetMPCheck: THeroHPCheck;
    TargetStatusCheck: TTargetStatusCheck;
    FriendCountCheck: TActorCountCheck;
    EnemyCountCheck: TActorCountCheck;
    boStraightLineCheck: Boolean;
  end;

  PHeroMagic = ^THeroMagic;
  THeroMagic = record
    Checked: Boolean;
    IsChanged: Boolean;
    MagicType: TMagicType;
    MagicID: Integer;
    IsCustomMagic: Boolean;
    UseRate: Integer;
    AttackRange: Integer;
    AttackTarget: TMagicAttackTarget;
    Condition: THeroMagicUseCondition;
  end;

  TCustomHeroMagicMgr = class(TObject)
  private
    FIsChanged: Boolean;
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PHeroMagic;
  public
    constructor Create;
    destructor Destroy; override;

    procedure LoadFromFile;
    procedure SaveToFile;

    function FindMagic(MagicType: TMagicType; MagicID: Integer): PHeroMagic;
    function AddDefMagic(MagicType: TMagicType; MagicID: Integer; UseRate: Integer;
      AttackRange: Integer = 1; AttackTarget: TMagicAttackTarget = matEnemy): PHeroMagic;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PHeroMagic read GetItems; default;
    function Add: PHeroMagic;
    function Remove(Item: PHeroMagic): Boolean;

    procedure Clear;
  end;

implementation

{ TCustomHeroMagicMgr }
uses
  M2Share, Grobal2;

constructor TCustomHeroMagicMgr.Create;
begin
  FIsChanged := False;
  FList := TList.Create;
end;

destructor TCustomHeroMagicMgr.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

procedure TCustomHeroMagicMgr.Clear;
var
  I: Integer;
  HeroMagic: PHeroMagic;
begin
  for I := 0 to FList.Count - 1 do
  begin
    HeroMagic := FList.Items[I];
    Dispose(HeroMagic);
  end;
  FList.Clear;
end;

function TCustomHeroMagicMgr.Add: PHeroMagic;
begin
  New(Result);
  FillChar(Result^, SizeOf(THeroMagic), 0);
  FList.Add(Result);
end;

function TCustomHeroMagicMgr.Remove(Item: PHeroMagic): Boolean;
var
  Index: Integer;
  HeroMagic: PHeroMagic;
begin
  Result := False;
  Index := FList.IndexOf(Item);
  if Index >= 0 then
  begin
    HeroMagic := FList.Items[Index];
    Dispose(HeroMagic);
    FList.Delete(Index);
    Result := True;
  end
end;

function TCustomHeroMagicMgr.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TCustomHeroMagicMgr.GetItems(Index: Integer): PHeroMagic;
begin
  if (Index >= 0) and (Index < FList.Count) then
  begin
    Result := FList.Items[index];
  end
  else
  begin
    Result := nil;
  end;
end;

function TCustomHeroMagicMgr.FindMagic(MagicType: TMagicType;
  MagicID: Integer): PHeroMagic;
var
  I: Integer;
  HeroMagic: PHeroMagic;
begin
  Result := nil;
  for I := 0 to FList.Count - 1 do
  begin
    HeroMagic := FList.Items[I];
    if (HeroMagic.MagicType = MagicType) and (HeroMagic.MagicID = MagicID) then
    begin
      Result := HeroMagic;
      Break;
    end;
  end;
end;

function TCustomHeroMagicMgr.AddDefMagic(MagicType: TMagicType; MagicID,
  UseRate: Integer; AttackRange: Integer; AttackTarget: TMagicAttackTarget): PHeroMagic;
var
  HeroMagic: PHeroMagic;
begin
  HeroMagic := FindMagic(MagicType, MagicID);
  if HeroMagic = nil then
  begin
    if UserEngine.FindHeroMagic(MagicID, mtHero) <> nil then
    begin
      HeroMagic := Add;
      HeroMagic.Checked := True;
      HeroMagic.IsChanged := False;
      HeroMagic.MagicType := MagicType;
      HeroMagic.MagicID := MagicID;
      HeroMagic.IsCustomMagic := False;
      HeroMagic.UseRate := UseRate;
      HeroMagic.AttackRange := AttackRange;
      HeroMagic.AttackTarget := AttackTarget;
    end;
  end
  else
  begin
    HeroMagic.IsChanged := False;
    HeroMagic.IsCustomMagic := False;
    HeroMagic.IsCustomMagic := False;
    HeroMagic.AttackRange := AttackRange;
    HeroMagic.AttackTarget := AttackTarget;
  end;
  
  Result := HeroMagic;
end;

procedure TCustomHeroMagicMgr.LoadFromFile;
var
  FileName, SectionName: string;
  IniFile: TFastIniFile;
  HeroMagic: PHeroMagic;
  I, HeroMagicCount: Integer;
  MagicID: Integer;
  IsFound: Boolean;
begin
  FIsChanged := False;

  FileName := g_Config.sEnvirDir + 'CustomHeroMagic.ini';

  if FileExists(FileName) then
  begin
    IniFile := TFastIniFile.Create(FileName);
    try
      HeroMagicCount := IniFile.ReadInteger('Setup', 'MagicCount', 0);
      if HeroMagicCount > 0 then
      begin
        for I := 0 to HeroMagicCount - 1 do
        begin
          SectionName := 'HeroMagic' + IntToStr(I + 1);

          MagicID := IniFile.ReadInteger(SectionName, 'MagicID', 0);

          if (MagicID > 0) then
          begin
            if CheckIsCustomMagic(MagicID) then
              IsFound := UserEngine.FindHeroMagic(MagicID) <> nil
            else
              IsFound := UserEngine.FindHeroMagic(MagicID, mtHero) <> nil;

            if IsFound then
            begin
              HeroMagic := Add;
              HeroMagic.MagicID := MagicID;
              HeroMagic.IsCustomMagic := CheckIsCustomMagic(MagicID);

              HeroMagic.Checked := IniFile.ReadBoolean(SectionName, 'Checked', False);
              HeroMagic.MagicType := TMagicType(IniFile.ReadInteger(SectionName, 'MagicType', 0));
              HeroMagic.UseRate := IniFile.ReadInteger(SectionName, 'UseRate', 0);

              HeroMagic.IsChanged := False;

              if HeroMagic.IsCustomMagic then
              begin
                HeroMagic.AttackRange := IniFile.ReadInteger(SectionName, 'AttackRange', 0);
                HeroMagic.AttackTarget := TMagicAttackTarget(IniFile.ReadInteger(SectionName, 'AttackTarget', 0));

                HeroMagic.Condition.HeroLevelCheck.boChecked := IniFile.ReadBoolean(SectionName, 'LevelChecked', False);
                HeroMagic.Condition.HeroLevelCheck.CompareSymbol := TCompareSymbol(IniFile.ReadInteger(SectionName, 'LevelCheckSymbol', 0));
                HeroMagic.Condition.HeroLevelCheck.CompareType := THeroLevelCompareType(IniFile.ReadInteger(SectionName, 'LevelCheckType', 0));
                HeroMagic.Condition.HeroLevelCheck.CompareValue := IniFile.ReadInteger(SectionName, 'LevelCheckValue', 0);


                HeroMagic.Condition.HeroHPCheck.boChecked := IniFile.ReadBoolean(SectionName, 'HeroHPChecked', False);
                HeroMagic.Condition.HeroHPCheck.CompareSymbol := TCompareSymbol(IniFile.ReadInteger(SectionName, 'HeroHPCheckSymbol', 0));
                HeroMagic.Condition.HeroHPCheck.CompareType := THeroHPCompareType(IniFile.ReadInteger(SectionName, 'HeroHPCheckType', 0));
                HeroMagic.Condition.HeroHPCheck.CompareValue := IniFile.ReadInteger(SectionName, 'HeroHPCheckValue', 0);

                HeroMagic.Condition.HeroMPCheck.boChecked := IniFile.ReadBoolean(SectionName, 'HeroMPChecked', False);
                HeroMagic.Condition.HeroMPCheck.CompareSymbol := TCompareSymbol(IniFile.ReadInteger(SectionName, 'HeroMPCheckSymbol', 0));
                HeroMagic.Condition.HeroMPCheck.CompareType := THeroHPCompareType(IniFile.ReadInteger(SectionName, 'HeroMPCheckType', 0));
                HeroMagic.Condition.HeroMPCheck.CompareValue := IniFile.ReadInteger(SectionName, 'HeroMPCheckValue', 0);

                HeroMagic.Condition.TargetHPCheck.boChecked := IniFile.ReadBoolean(SectionName, 'TargetHPChecked', False);
                HeroMagic.Condition.TargetHPCheck.CompareSymbol := TCompareSymbol(IniFile.ReadInteger(SectionName, 'TargetHPCheckSymbol', 0));
                HeroMagic.Condition.TargetHPCheck.CompareType := THeroHPCompareType(IniFile.ReadInteger(SectionName, 'TargetHPCheckType', 0));
                HeroMagic.Condition.TargetHPCheck.CompareValue := IniFile.ReadInteger(SectionName, 'TargetHPCheckValue', 0);

                HeroMagic.Condition.TargetMPCheck.boChecked := IniFile.ReadBoolean(SectionName, 'TargetMPChecked', False);
                HeroMagic.Condition.TargetMPCheck.CompareSymbol := TCompareSymbol(IniFile.ReadInteger(SectionName, 'TargetMPCheckSymbol', 0));
                HeroMagic.Condition.TargetMPCheck.CompareType := THeroHPCompareType(IniFile.ReadInteger(SectionName, 'TargetMPCheckType', 0));
                HeroMagic.Condition.TargetMPCheck.CompareValue := IniFile.ReadInteger(SectionName, 'TargetMPCheckValue', 0);


                HeroMagic.Condition.TargetStatusCheck.boPoisonDamageArmor := IniFile.ReadBoolean(SectionName, 'PoisonDamageArmor', False);
                HeroMagic.Condition.TargetStatusCheck.boPoisonDecHealth := IniFile.ReadBoolean(SectionName, 'PoisonDecHealth', False);
                HeroMagic.Condition.TargetStatusCheck.boPoisoning := IniFile.ReadBoolean(SectionName, 'Poisoning', False);
                HeroMagic.Condition.TargetStatusCheck.boPoisonStone := IniFile.ReadBoolean(SectionName, 'PoisonStone', False);
                HeroMagic.Condition.TargetStatusCheck.boFrozen := IniFile.ReadBoolean(SectionName, 'Frozen', False);
                HeroMagic.Condition.TargetStatusCheck.boForeverFrozen := IniFile.ReadBoolean(SectionName, 'ForeverFrozen', False);
                HeroMagic.Condition.TargetStatusCheck.boCobwebWinding := IniFile.ReadBoolean(SectionName, 'CobwebWinding', False);

                HeroMagic.Condition.TargetStatusCheck.boUnPoisonDamageArmor := IniFile.ReadBoolean(SectionName, 'UnPoisonDamageArmor', False);
                HeroMagic.Condition.TargetStatusCheck.boUnPoisonDecHealth := IniFile.ReadBoolean(SectionName, 'UnPoisonDecHealth', False);
                HeroMagic.Condition.TargetStatusCheck.boUnPoisoning := IniFile.ReadBoolean(SectionName, 'UnPoisoning', False);
                HeroMagic.Condition.TargetStatusCheck.boUnPoisonStone := IniFile.ReadBoolean(SectionName, 'UnPoisonStone', False);
                HeroMagic.Condition.TargetStatusCheck.boUnFrozen := IniFile.ReadBoolean(SectionName, 'UnFrozen', False);
                HeroMagic.Condition.TargetStatusCheck.boUnForeverFrozen := IniFile.ReadBoolean(SectionName, 'UnForeverFrozen', False);
                HeroMagic.Condition.TargetStatusCheck.boUnCobwebWinding := IniFile.ReadBoolean(SectionName, 'UnCobwebWinding', False);

                HeroMagic.Condition.FriendCountCheck.boChecked := IniFile.ReadBoolean(SectionName, 'FriendCountChecked', False);
                HeroMagic.Condition.FriendCountCheck.nCheckRange := IniFile.ReadInteger(SectionName, 'FriendCountCheckRange', 0);
                HeroMagic.Condition.FriendCountCheck.nCheckValue := IniFile.ReadInteger(SectionName, 'FriendCountCheckValue', 0);

                HeroMagic.Condition.EnemyCountCheck.boChecked := IniFile.ReadBoolean(SectionName, 'EnemyCountChecked', False);
                HeroMagic.Condition.EnemyCountCheck.nCheckRange := IniFile.ReadInteger(SectionName, 'EnemyCountCheckRange', 0);
                HeroMagic.Condition.EnemyCountCheck.nCheckValue := IniFile.ReadInteger(SectionName, 'EnemyCountCheckValue', 0);

                HeroMagic.Condition.boStraightLineCheck := IniFile.ReadBoolean(SectionName, 'StraightLineChecked', False);
              end;
            end;
          end;
        end;
      end;
    finally
      IniFile.Free;
    end;
  end;

  // --------------------------------------------------------------战士技能-----
  AddDefMagic(mtWarrAttack, 208, 0, 10);                      // 旋风转  SKILL_208
  AddDefMagic(mtWarrAttack, 204, 0, 10);                      // 十步一杀 SKILL_204
  AddDefMagic(mtWarrAttack, 56, 0, 4);                        // 逐日剑法 SKILL_56
  AddDefMagic(mtWarrAttack, 66, 0, 2);                        // 开天斩 SKILL_66
  AddDefMagic(mtWarrAttack, 113, 0, 4);                       // 断空斩 SKILL_113
  AddDefMagic(mtWarrAttack, 115, 0, 4);                       // 血魄一击 SKILL_115
  AddDefMagic(mtWarrAttack, 12, 0, 2);                        // 刺杀剑术 SKILL_ERGUM
  AddDefMagic(mtWarrAttack, 27, 4, 2);                        // 野蛮冲撞 SKILL_MOOTEBO
  AddDefMagic(mtWarrAttack, 7, 10);                           // 攻杀剑术 SKILL_YEDO
  AddDefMagic(mtWarrAttack, 114, 0);                          // 倚天辟地 SKILL_114
  AddDefMagic(mtWarrAttack, 26, 0);                           // 烈火剑法 SKILL_FIRESWORD
  AddDefMagic(mtWarrAttack, 39, 3);                           // 彻地钉 SKILL_GROUPDEDING
  AddDefMagic(mtWarrAttack, 41, 10);                          // 狮子吼 SKILL_41
  AddDefMagic(mtWarrAttack, 75, 3, 0, matSelf);               // 护体神盾 SKILL_75
  AddDefMagic(mtWarrAttack, 25, 0);                           // 半月弯刀 SKILL_BANWOL
  AddDefMagic(mtWarrAttack, 40, 2);                           // 双龙斩 SKILL_40
  AddDefMagic(mtWarrAttack, 42, 2);                           // 龙影剑法 SKILL_42
  AddDefMagic(mtWarrAttack, 43, 3);                           // 雷霆剑法 SKILL_43

  // --------------------------------------------------------------法师攻击技能-----
  AddDefMagic(mtWizardAttack, 74, 0, 0, matSelf);             // 分身术 SKILL_74
  AddDefMagic(mtWizardAttack, 75, 3, 0, matSelf);             // 护体神盾  SKILL_75
  AddDefMagic(mtWizardAttack, 114, 0);                        // 倚天辟地 SKILL_114
  AddDefMagic(mtWizardAttack, 116, 0, 0);                     // 血魄一击 SKILL_116
  AddDefMagic(mtWizardAttack, 31, 0, 0, matSelf);             // 魔法盾 SKILL_SHIELD
  AddDefMagic(mtWizardAttack, 8, 2);                          // 抗拒火环 SKILL_FIREWIND
  AddDefMagic(mtWizardAttack, 47, 10);                        // 火龙烈焰 SKILL_47
  AddDefMagic(mtWizardAttack, 33, 8);                         // 冰咆哮 SKILL_SNOWWIND
  AddDefMagic(mtWizardAttack, 205, 10);                       // 冰霜雪雨 SKILL_205
  AddDefMagic(mtWizardAttack, 206, 10);                       // 冰霜群雨 SKILL_206
  AddDefMagic(mtWizardAttack, 209, 10);                       // 五雷轰 SKILL_209
  AddDefMagic(mtWizardAttack, 45, 3);                         // 灭天火 SKILL_45
  AddDefMagic(mtWizardAttack, 11, 3);                         // 雷电术 SKILL_LIGHTENING
  AddDefMagic(mtWizardAttack, 37, 0);                         // 群雷术  SKILL_GROUPLIGHTENING
  AddDefMagic(mtWizardAttack, 22, 8);                         // 火墙 SKILL_EARTHFIRE
  AddDefMagic(mtWizardAttack, 24, 3);                         // 地狱雷光 SKILL_LIGHTFLOWER
  AddDefMagic(mtWizardAttack, 23, 3);                         // 爆裂火焰 SKILL_FIREBOOM
  AddDefMagic(mtWizardAttack, 58, 2);                         // 流星火雨 SKILL_58
  AddDefMagic(mtWizardAttack, 67, 3);                         // 先天元力 SKILL_67
  AddDefMagic(mtWizardAttack, 68, 3);                         // 酒气护体 SKILL_68
  AddDefMagic(mtWizardAttack, 9, 3);                          // 地狱火 SKILL_FIRE
  AddDefMagic(mtWizardAttack, 20, 7);                         // 诱惑之光 SKILL_TAMMING
  AddDefMagic(mtWizardAttack, 10, 3);                         // 疾光电影 SKILL_SHOOTLIGHTEN
  AddDefMagic(mtWizardAttack, 44, 3);                         // 寒冰掌 SKILL_44
  AddDefMagic(mtWizardAttack, 32, 3);                         // 圣言术 SKILL_KILLUNDEAD
  AddDefMagic(mtWizardAttack, 1, 0);                          // 火球术 SKILL_FIREBALL
  AddDefMagic(mtWizardAttack, 5, 0);                          // 大火球 SKILL_FIREBALL2

  // --------------------------------------------------------------道士攻击技能-----
  AddDefMagic(mtTaosAttack, 114, 0);                          // 倚天辟地 SKILL_114
  AddDefMagic(mtTaosAttack, 117, 0, 0);                       // 血魄一击 SKILL_117
  AddDefMagic(mtTaosAttack, 50, 0, 0, matSelf);               // 无极真气 SKILL_50
  AddDefMagic(mtTaosAttack, 51, 6);                           // 群体施毒术 SKILL_GROUPAMYOUNSUL
  AddDefMagic(mtTaosAttack, 6, 0);                            // 施毒术 SKILL_AMYOUNSUL
  AddDefMagic(mtTaosAttack, 57, 0);                           // 噬血术 SKILL_57
  AddDefMagic(mtTaosAttack, 13, 5);                           // 灵魂火符 SKILL_FIRECHARM
  AddDefMagic(mtTaosAttack, 52, 5);                           // 飓风破 SKILL_52
  AddDefMagic(mtTaosAttack, 202, 5);                          // 裂神符 SKILL_202
  AddDefMagic(mtTaosAttack, 203, 6);                          // 死亡之眼 SKILL_203
  AddDefMagic(mtTaosAttack, 210, 6);                          // 幽冥火符 SKILL_210
  AddDefMagic(mtTaosAttack, 48, 6);                           // 气功波 SKILL_48
  AddDefMagic(mtTaosAttack, 18, 10, 0, matSelf);              // 隐身术 SKILL_CLOAK
  AddDefMagic(mtTaosAttack, 19, 10, 0, matPartner);           // 集体隐身术 SKILL_BIGCLOAK
  AddDefMagic(mtTaosAttack, 55, 0, 0, matSelf);               // 召唤月灵 SKILL_55
  AddDefMagic(mtTaosAttack, 30, 0, 0, matSelf);               // 召唤神兽 SKILL_SINSU
  AddDefMagic(mtTaosAttack, 17, 0, 0, matSelf);               // 召唤骷髅 SKILL_SKELLETON
  AddDefMagic(mtTaosAttack, 15, 0, 0, matMaster);             // 神圣战甲术 防 SKILL_DEJIWONHO
  AddDefMagic(mtTaosAttack, 14, 0, 0, matMaster);             // 幽灵盾 魔 SKILL_HANGMAJINBUB
  AddDefMagic(mtTaosAttack, 34, 0, 0, matPartner);            // 解毒术 SKILL_UNAMYOUNSUL
  AddDefMagic(mtTaosAttack, 49, 10, 0, matPartner);           // 净化术 SKILL_49
  AddDefMagic(mtTaosAttack, 2, 8, 0, matPartner);             // 治愈术 SKILL_HEALLING
  AddDefMagic(mtTaosAttack, 29, 8, 0, matPartner);            // 群体治愈术 SKILL_BIGHEALLING
  AddDefMagic(mtTaosAttack, 75, 3, 0, matSelf);               // 护体神盾 SKILL_75

end;

procedure TCustomHeroMagicMgr.SaveToFile;
var
  FileName, SectionName: string;
  IniFile: TFastIniFile;
  I: Integer;
  HeroMagic: PHeroMagic;
begin
  FIsChanged := False;

  if not DirectoryExists(g_Config.sEnvirDir) then
    ForceDirectories(g_Config.sEnvirDir);

  FileName := g_Config.sEnvirDir + 'CustomHeroMagic.ini';
  IniFile := TFastIniFile.Create(FileName);

  IniFile.WriteInteger('Setup', 'MagicCount', g_CustomHeroMagicMgr.Count);
  for I := 0 to g_CustomHeroMagicMgr.Count - 1 do
  begin
    HeroMagic := g_CustomHeroMagicMgr.Items[I];
    SectionName := 'HeroMagic' + IntToStr(I + 1);

    IniFile.WriteBoolean(SectionName, 'Checked', HeroMagic.Checked);
    IniFile.WriteInteger(SectionName, 'MagicID', HeroMagic.MagicID);
    IniFile.WriteInteger(SectionName, 'MagicType', Integer(HeroMagic.MagicType));
    IniFile.WriteInteger(SectionName, 'UseRate', Integer(HeroMagic.UseRate));

    HeroMagic.IsChanged := False;
    if HeroMagic.IsCustomMagic then
    begin
      IniFile.WriteInteger(SectionName, 'AttackRange', Integer(HeroMagic.AttackRange));
      IniFile.WriteInteger(SectionName, 'AttackTarget', Integer(HeroMagic.AttackTarget));

      IniFile.WriteBoolean(SectionName, 'LevelChecked', HeroMagic.Condition.HeroLevelCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'LevelCheckSymbol', Integer(HeroMagic.Condition.HeroLevelCheck.CompareSymbol));
      IniFile.WriteInteger(SectionName, 'LevelCheckType', Integer(HeroMagic.Condition.HeroLevelCheck.CompareType));
      IniFile.WriteInteger(SectionName, 'LevelCheckValue', HeroMagic.Condition.HeroLevelCheck.CompareValue);

      IniFile.WriteBoolean(SectionName, 'HeroHPChecked', HeroMagic.Condition.HeroHPCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'HeroHPCheckSymbol', Integer(HeroMagic.Condition.HeroHPCheck.CompareSymbol));
      IniFile.WriteInteger(SectionName, 'HeroHPCheckType', Integer(HeroMagic.Condition.HeroHPCheck.CompareType));
      IniFile.WriteInteger(SectionName, 'HeroHPCheckValue', HeroMagic.Condition.HeroHPCheck.CompareValue);

      IniFile.WriteBoolean(SectionName, 'HeroMPChecked', HeroMagic.Condition.HeroMPCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'HeroMPCheckSymbol', Integer(HeroMagic.Condition.HeroMPCheck.CompareSymbol));
      IniFile.WriteInteger(SectionName, 'HeroMPCheckType', Integer(HeroMagic.Condition.HeroMPCheck.CompareType));
      IniFile.WriteInteger(SectionName, 'HeroMPCheckValue', HeroMagic.Condition.HeroMPCheck.CompareValue);

      IniFile.WriteBoolean(SectionName, 'TargetHPChecked', HeroMagic.Condition.TargetHPCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'TargetHPCheckSymbol', Integer(HeroMagic.Condition.TargetHPCheck.CompareSymbol));
      IniFile.WriteInteger(SectionName, 'TargetHPCheckType', Integer(HeroMagic.Condition.TargetHPCheck.CompareType));
      IniFile.WriteInteger(SectionName, 'TargetHPCheckType', HeroMagic.Condition.TargetHPCheck.CompareValue);

      IniFile.WriteBoolean(SectionName, 'TargetMPChecked', HeroMagic.Condition.TargetMPCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'TargetMPCheckSymbol', Integer(HeroMagic.Condition.TargetMPCheck.CompareSymbol));
      IniFile.WriteInteger(SectionName, 'TargetMPCheckType', Integer(HeroMagic.Condition.TargetMPCheck.CompareType));
      IniFile.WriteInteger(SectionName, 'TargetMPCheckValue', HeroMagic.Condition.TargetMPCheck.CompareValue);

      IniFile.WriteBoolean(SectionName, 'PoisonDamageArmor', HeroMagic.Condition.TargetStatusCheck.boPoisonDamageArmor);
      IniFile.WriteBoolean(SectionName, 'PoisonDecHealth', HeroMagic.Condition.TargetStatusCheck.boPoisonDecHealth);
      IniFile.WriteBoolean(SectionName, 'Poisoning', HeroMagic.Condition.TargetStatusCheck.boPoisoning);
      IniFile.WriteBoolean(SectionName, 'PoisonStone', HeroMagic.Condition.TargetStatusCheck.boPoisonStone);
      IniFile.WriteBoolean(SectionName, 'Frozen', HeroMagic.Condition.TargetStatusCheck.boFrozen);
      IniFile.WriteBoolean(SectionName, 'ForeverFrozen', HeroMagic.Condition.TargetStatusCheck.boForeverFrozen);
      IniFile.WriteBoolean(SectionName, 'CobwebWinding', HeroMagic.Condition.TargetStatusCheck.boCobwebWinding);

      IniFile.WriteBoolean(SectionName, 'UnPoisonDamageArmor', HeroMagic.Condition.TargetStatusCheck.boUnPoisonDamageArmor);
      IniFile.WriteBoolean(SectionName, 'UnPoisonDecHealth', HeroMagic.Condition.TargetStatusCheck.boUnPoisonDecHealth);
      IniFile.WriteBoolean(SectionName, 'UnPoisoning', HeroMagic.Condition.TargetStatusCheck.boUnPoisoning);
      IniFile.WriteBoolean(SectionName, 'UnPoisonStone', HeroMagic.Condition.TargetStatusCheck.boUnPoisonStone);
      IniFile.WriteBoolean(SectionName, 'UnFrozen', HeroMagic.Condition.TargetStatusCheck.boUnFrozen);
      IniFile.WriteBoolean(SectionName, 'UnForeverFrozen', HeroMagic.Condition.TargetStatusCheck.boUnForeverFrozen);
      IniFile.WriteBoolean(SectionName, 'UnCobwebWinding', HeroMagic.Condition.TargetStatusCheck.boUnCobwebWinding);

      IniFile.WriteBoolean(SectionName, 'FriendCountChecked', HeroMagic.Condition.FriendCountCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'FriendCountCheckRange', HeroMagic.Condition.FriendCountCheck.nCheckRange);
      IniFile.WriteInteger(SectionName, 'FriendCountCheckValue', HeroMagic.Condition.FriendCountCheck.nCheckValue);

      IniFile.WriteBoolean(SectionName, 'EnemyCountChecked', HeroMagic.Condition.EnemyCountCheck.boChecked);
      IniFile.WriteInteger(SectionName, 'EnemyCountCheckRange', HeroMagic.Condition.EnemyCountCheck.nCheckRange);
      IniFile.WriteInteger(SectionName, 'EnemyCountCheckValue', HeroMagic.Condition.EnemyCountCheck.nCheckValue);

      IniFile.WriteBoolean(SectionName, 'StraightLineChecked', HeroMagic.Condition.boStraightLineCheck);
    end;
  end;

  IniFile.Free;
end;

initialization
  g_CustomHeroMagicMgr := TCustomHeroMagicMgr.Create;

finalization
  g_CustomHeroMagicMgr.Free;

end.
