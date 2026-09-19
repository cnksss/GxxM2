unit uCombatPowerUtils;

interface

uses
  Windows, Classes, SysUtils, IniFiles, SyncObjs, M2Share, ObjBase,
  {$IFDEF CPUX64}
  //VMProtectSDK,
  {$ENDIF}
  ObjPlayer, Grobal2;

type
  PCombatPowerVarRecord = ^TCombatPowerVarRecord;

  TCombatPowerVarRecord = record
    VarName: string[40];
    Value0: Integer;
    Value1: Integer;
    Value2: Integer;
    Desc: string;
  end;

  TVarNodeData = TCombatPowerVarRecord;

  PVarNodeData = PCombatPowerVarRecord;

  TCombatPowerVarMgr = class(TObject)
  private
    FCriticalSection: TRTLCriticalSection;

    FVarList: TList;
    FSortVarList: TList;

    function DoSearch(const VarName: string; var Index: Integer): Boolean;
    function GetCount: Integer;
    function GetItems(Index: Integer): PCombatPowerVarRecord;
  public
    constructor Create; virtual;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PCombatPowerVarRecord read GetItems;

    procedure Lock;
    procedure UnLock;

    procedure Clear;
    function Add(const VarName: string; Value0, Value1, Value2: Integer; Desc: string): PCombatPowerVarRecord;
    function Remove(const VarName: string): Boolean;
    function GetValueRecord(const VarName: string): PCombatPowerVarRecord;

    procedure LoadConfig;
    procedure SaveConfig;
  end;

type
  TCombatPowerAttrib = (cpaMaxHP, cpaMaxMP, cpaAC1, cpaAC2, cpaMAC1, cpaMAC2, cpaDC1, cpaDC2, cpaMC1, cpaMC2, cpaSC1, cpaSC2,
    cpaAntiMagic, cpaAntiPoison, cpaPoisonRecover, cpaHPRecover, cpaMPRecover, cpaHitSpeed, cpaSpellSpeed, cpaHitPoint,
    cpaSpeedPoint, cpaLuckOrUnLuck, cpaParalysisRate, cpaMDParalysisRate, cpaFrozenRate, cpaCobwebWindingRate, cpaRevival,
    cpaMagicShield, cpaBlastHit {0}, cpaDamageAdd {1}, cpaDamageDec {2}, cpaSpellDamageDec {3}, cpaCloseDefense {4},
    cpaDamageRebound {5}, cpaAddMonDropRate{6}, cpaMaxHPAdd{7}, cpaMaxMPAdd {8}, cpaAngryValueTimAdd {9}, cpaGroupDamageAdd {10},
    cpaAddHuamDropRate{11}, cpaAddUndropRate{12}, cpaUnParalysis {13}, cpaUnMagicShield {14}, cpaUnRevival {15}, cpaUnPosion {16},
    cpaUnTamming {17}, cpaUnFireCross {18}, cpaUnFrozen {19}, cpaUnCobwebWinding {20}, cpaFatalBlowRate {21}, cpaFatalBlowPower
    {22}, cpaFatalBlowDefense, {23}
      cpaUnBlastHit {24}
);

  TJobDefCombatPowerValue = array[TCombatPowerAttrib] of Integer;

  PJobDefCombatPowerValue = ^TJobDefCombatPowerValue;

  TDefCombatPowerValue = array[0..2] of TJobDefCombatPowerValue;

const
  CombatPowerAttribNames: array[TCombatPowerAttrib] of string = ('MaxHP', 'MaxMP', '防御下限', '防御上限', '魔防下限', '魔防上限', '攻击下限',
    '攻击上限', '魔法下限', '魔法上限', '道术下限', '道术上限', '魔法躲避', '毒物躲避', '中毒恢复', '体力恢复', '魔法恢复', '攻击速度', '魔法速度', '精确度', '敏捷度', '幸运 / 诅咒',
    '麻痹机率', '魔道麻痹机率', '冰冻机率', '蛛网机率', '复活', '护身', '暴击几率', '攻击伤害', '伤害吸收', '魔法防御', '忽视防御', '伤害反弹', '怪物暴率', '体力增加', '魔力增加',
    '怒气恢复', '合击伤害', '人物爆率', '防爆出率', '防止麻痹', '防止护身', '防止复活', '防止全毒', '防止诱惑', '防止火墙', '防止冰冻', '防止蛛网', '致命一击几率', '致命一击伤害',
    '致命一击防御', '暴击抗性');

var
  g_DefCombatPowerValue: TDefCombatPowerValue;
  g_CombatPowerVarMgr: TCombatPowerVarMgr;

procedure LoadDefCombatPowerConfig;

procedure SaveDefCombatPowerConfig;

procedure RecalcPlayCombatPower(SmartObject: TSmartObject);

implementation

const
  CombatPowerAttribIdents: array[TCombatPowerAttrib] of string = ('MaxHP', 'MaxMP', 'AC1', 'AC2', 'MAC1', 'MAC2', 'DC1', 'DC2',
    'MC1', 'MC2', 'SC1', 'SC2', 'AntiMagic', 'AntiPoison', 'PoisonRecover', 'HPRecover', 'MPRecover', 'HitSpeed', 'SpellSpeed',
    'HitPoint', 'SpeedPoint', 'LuckOrUnLuck', 'ParalysisRate', 'MDParalysisRate', 'FrozenRate', 'CobwebWindingRate', 'Revival',
    'MagicShield', 'BlastHit', 'DamageAdd', 'DamageDec', 'SpellDamageDec', 'CloseDefense', 'DamageRebound', 'AddMonDropRate',
    'MaxHPAdd', 'MaxMPAdd', 'AngryValueTimAdd', 'GroupDamageAdd', 'AddHuamDropRate', 'AddUndropRate', 'UnParalysis',
    'UnMagicShield', 'UnRevival', 'UnPosion', 'UnTamming', 'UnFireCross', 'UnFrozen', 'UnCobwebWinding', 'FatalBlowRate',
    'FatalBlowPower', 'FatalBlowDefense', 'UnBlastHit');

procedure LoadDefCombatPowerConfig;
var
  FileName, Ident: string;
  Attr: TCombatPowerAttrib;
  IniFile: TIniFile;
begin
  FileName := g_Config.sEnvirDir + 'CombatPower.ini';
  if not FileExists(FileName) then
    Exit;

  IniFile := TIniFile.Create(FileName);

  for Attr := Low(TCombatPowerAttrib) to High(TCombatPowerAttrib) do
  begin
    Ident := CombatPowerAttribIdents[Attr];
    g_DefCombatPowerValue[0][Attr] := IniFile.ReadInteger('DefaultAttrib', Ident + '_0', g_DefCombatPowerValue[0][Attr]);
    g_DefCombatPowerValue[1][Attr] := IniFile.ReadInteger('DefaultAttrib', Ident + '_1', g_DefCombatPowerValue[1][Attr]);
    g_DefCombatPowerValue[2][Attr] := IniFile.ReadInteger('DefaultAttrib', Ident + '_2', g_DefCombatPowerValue[2][Attr]);
  end;

  IniFile.Free;
end;

procedure SaveDefCombatPowerConfig;
var
  FileName, Ident: string;
  Attr: TCombatPowerAttrib;
  IniFile: TIniFile;
begin
  FileName := g_Config.sEnvirDir + 'CombatPower.ini';

  IniFile := TIniFile.Create(FileName);
  for Attr := Low(TCombatPowerAttrib) to High(TCombatPowerAttrib) do
  begin
    Ident := CombatPowerAttribIdents[Attr];
    IniFile.WriteInteger('DefaultAttrib', Ident + '_0', g_DefCombatPowerValue[0][Attr]);
    IniFile.WriteInteger('DefaultAttrib', Ident + '_1', g_DefCombatPowerValue[1][Attr]);
    IniFile.WriteInteger('DefaultAttrib', Ident + '_2', g_DefCombatPowerValue[2][Attr]);
  end;

  IniFile.Free;
end;

procedure RecalcPlayCombatPower(SmartObject: TSmartObject);
var
  I64: Int64;
  I, n01, nVarValue, PowerValue: Integer;
  sVarName: string;
  JobValues: PJobDefCombatPowerValue;
  VarRecord: PCombatPowerVarRecord;
  Player: TPlayObject;
begin
  if not g_Config.boOpenCombatPowerCalc then
  begin
    SmartObject.m_nCombatPower := 0;
    Exit;
  end;

  if SmartObject.m_btJob = 1 then
    JobValues := @g_DefCombatPowerValue[1]
  else if SmartObject.m_btJob = 2 then
    JobValues := @g_DefCombatPowerValue[2]
  else
    JobValues := @g_DefCombatPowerValue[0];

  (*
    cpaAC1, cpaAC2, cpaMAC1, cpaMAC2,
    cpaDC1, cpaDC2, cpaMC1, cpaMC2,
    cpaSC1, cpaSC2, cpaAntiMagic, cpaAntiPoison,
    cpaPoisonRecover, cpaHPRecover, cpaMPRecover, cpaHitSpeed, cpaSpellSpeed,
    cpaHitPoint, cpaSpeedPoint,

    cpaBlastHit {0}, cpaDamageAdd {1}, cpaDamageDec {2}, cpaSpellDamageDec {3},
    cpaCloseDefense {4}, cpaDamageRebound {5}, cpaMaxHPAdd{7}, cpaMaxMPAdd {8},                     // 6  怪物爆率
    cpaAngryValueTimAdd {9}, cpaGroupDamageAdd {10}, cpaUnParalysis {13}, cpaUnMagicShield {14},    // 11 人物爆率  12 防爆几率
    cpaUnRevival {15}, cpaUnPosion {16}, cpaUnTamming {17}, cpaUnFireCross {18},
    cpaUnFrozen {19}, cpaUnCobwebWinding {20},
    cpaFatalBlowRate {21}, cpaFatalBlowPower {22}, cpaFatalBlowDefense {23}
  *)
  I64 := Round(SmartObject.m_WAbil.MaxHP / 1000 * JobValues[cpaMaxHP]) + Round(SmartObject.m_WAbil.MaxMP / 1000 * JobValues[cpaMaxMP])
    + SmartObject.m_WAbil.AC1 * JobValues[cpaAC1] + SmartObject.m_WAbil.AC2 * JobValues[cpaAC2] + SmartObject.m_WAbil.MAC1 *
    JobValues[cpaMAC1] + SmartObject.m_WAbil.MAC2 * JobValues[cpaMAC2] + SmartObject.m_WAbil.DC1 * JobValues[cpaDC1] + SmartObject.m_WAbil.DC2
    * JobValues[cpaDC2] + SmartObject.m_WAbil.MC1 * JobValues[cpaMC1] + SmartObject.m_WAbil.MC2 * JobValues[cpaMC2] + SmartObject.m_WAbil.SC1
    * JobValues[cpaSC1] + SmartObject.m_WAbil.SC2 * JobValues[cpaSC2] + SmartObject.m_nAntiMagic * JobValues[cpaAntiMagic] +
    SmartObject.m_btAntiPoison * JobValues[cpaAntiPoison] + SmartObject.m_nPoisonRecover * JobValues[cpaPoisonRecover] +
    SmartObject.m_nHealthRecover * JobValues[cpaHPRecover] + SmartObject.m_nSpellRecover * JobValues[cpaMPRecover] + SmartObject.m_nAttackSpeed
    * JobValues[cpaHitSpeed] + SmartObject.m_nSpellSpeed * JobValues[cpaSpellSpeed] + SmartObject.m_btHitPoint * JobValues[cpaHitPoint]
    + SmartObject.m_btSpeedPoint * JobValues[cpaSpeedPoint] + SmartObject.m_nLuck * JobValues[cpaLuckOrUnLuck] + SmartObject.m_dwParalysisRate
    * JobValues[cpaParalysisRate] + SmartObject.m_dwMDParalysisRate * JobValues[cpaMDParalysisRate] + SmartObject.m_dwFrozenRate *
    JobValues[cpaFrozenRate] + SmartObject.m_dwCobwebWindingRate * JobValues[cpaCobwebWindingRate] + Integer(SmartObject.m_boRevival)
    * JobValues[cpaRevival] + Integer(SmartObject.m_boMagicShield) * JobValues[cpaMagicShield] + SmartObject.m_WAbil.NewValue[0] *
    JobValues[cpaBlastHit] +           // 爆击几率
    SmartObject.m_WAbil.NewValue[1] * JobValues[cpaDamageAdd] +          // 伤害增加
    SmartObject.m_WAbil.NewValue[2] * JobValues[cpaDamageDec] +          // 伤害减少
    SmartObject.m_WAbil.NewValue[3] * JobValues[cpaSpellDamageDec] +     // 魔法防御
    SmartObject.m_WAbil.NewValue[4] * JobValues[cpaCloseDefense] +       // 忽视防御
    SmartObject.m_WAbil.NewValue[5] * JobValues[cpaDamageRebound] +      // 伤害反弹
    SmartObject.m_WAbil.NewValue[6] * JobValues[cpaAddMonDropRate] +     // 怪物爆率
    SmartObject.m_WAbil.NewValue[7] * JobValues[cpaMaxHPAdd] +           // 体力增加
    SmartObject.m_WAbil.NewValue[8] * JobValues[cpaMaxMPAdd] +           // 魔力增加
    SmartObject.m_WAbil.NewValue[9] * JobValues[cpaAngryValueTimAdd] +   // 怒气恢复
    SmartObject.m_WAbil.NewValue[10] * JobValues[cpaGroupDamageAdd] +    // 合击攻击
    SmartObject.m_WAbil.NewValue[11] * JobValues[cpaAddHuamDropRate] +   // 人物爆率
    SmartObject.m_WAbil.NewValue[12] * JobValues[cpaAddUndropRate] +     // 防爆出率
    SmartObject.m_WAbil.NewValue[13] * JobValues[cpaUnParalysis] +       // 防止麻痹
    SmartObject.m_WAbil.NewValue[14] * JobValues[cpaUnMagicShield] +     // 防止护身
    SmartObject.m_WAbil.NewValue[15] * JobValues[cpaUnRevival] +         // 防止复活
    SmartObject.m_WAbil.NewValue[16] * JobValues[cpaUnPosion] +          // 防止全毒
    SmartObject.m_WAbil.NewValue[17] * JobValues[cpaUnTamming] +         // 防止诱惑
    SmartObject.m_WAbil.NewValue[18] * JobValues[cpaUnFireCross] +       // 防止火墙
    SmartObject.m_WAbil.NewValue[19] * JobValues[cpaUnFrozen] +          // 防止冰冻
    SmartObject.m_WAbil.NewValue[20] * JobValues[cpaUnCobwebWinding] +   // 防止蛛网
    SmartObject.m_WAbil.NewValue[21] * JobValues[cpaFatalBlowRate] +     // 致命一击几率
    SmartObject.m_WAbil.NewValue[22] * JobValues[cpaFatalBlowPower] +    // 致命一击攻击
    SmartObject.m_WAbil.NewValue[23] * JobValues[cpaFatalBlowDefense] +  // 致命一击防御
    SmartObject.m_WAbil.NewValue[24] * JobValues[cpaUnBlastHit];         // 暴击抗性


  if g_Config.boOpenCombatPowerVarCalc and (SmartObject.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    Player := TPlayObject(SmartObject);

    g_CombatPowerVarMgr.Lock;
    try
      for I := 0 to g_CombatPowerVarMgr.Count - 1 do
      begin
        VarRecord := g_CombatPowerVarMgr.Items[I];
        sVarName := VarRecord.VarName;
        if sVarName = '' then
          Continue;
        n01 := GetValNameNo(sVarName);

        nVarValue := 0;
        if n01 >= 0 then
        begin
          case n01 of
            0..999:
              begin                                                                                       // P
                //nVarValue := Player.m_nVal[n01];
              end;
            1000..1999:
              begin                                                                                       // D
                nVarValue := Player.m_DyVal[n01 - 1000];
              end;
            2000..2999:
              begin                                                                                       // M
                nVarValue := Player.m_nMval[n01 - 2000];
              end;
            3000..3999:
              begin                                                                                       // N
                nVarValue := Player.m_nInteger[n01 - 3000];
              end;
            4000..4999:
              begin                                                                                       // I
                // nVarValue := g_Config.GlobaDyMval[n01 - 4000];
              end;
            5000..5999:
              begin                                                                                       // G
                // nVarValue := g_Config.GlobalVal[n01 - 5000];
              end;
            6000..6999:
              begin                                                                                       // A
                // nVarValue := StrToInt64Def(g_Config.GlobalAVal[n01 - 6000], nVarValue);
              end;
            7000..7999:
              begin                                                                                       // S
                // nVarValue := StrToInt64Def(Player.m_sString[n01 - 7000], nVarValue);
              end;

            // 私有变量 U-数字型 chongchong 2014-10-18
              8000..8499:
              begin
                nVarValue := Player.m_UVal[n01 - 8000];
              end;

            // 私有变量 T-字符串型 chongchong 2014-10-18
              8500..8999:
              begin
                // nVarValue := StrToInt64Def(Player.m_TVal[n01 - 8255], nVarValue);
              end;

            // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
              9000..9499:
              begin
                nVarValue := Player.m_JVal[n01 - 9000];
              end;
          end;
        end
        else if (Length(sVarName) > 2) and (UpCase(sVarName[1]) = 'S') and (sVarName[2] = '$') then
        begin
          n01 := Player.m_StringList.GetIndex(UpperCase(sVarName));
          if n01 >= 0 then
          begin
            nVarValue := StrToInt64Def(Player.m_StringList.Strings[n01], 0);
          end;
        end
        else if (Length(sVarName) > 2) and (UpCase(sVarName[1]) = 'N') and (sVarName[2] = '$') then
        begin
          n01 := Player.m_IntegerList.GetIndex(UpperCase(sVarName));
          if n01 >= 0 then
          begin
            nVarValue := Integer(Player.m_IntegerList.Objects[n01]);
          end;
        end;

        if nVarValue <> 0 then
        begin
          if Player.m_btJob = 1 then
            PowerValue := VarRecord.Value1
          else if Player.m_btJob = 2 then
            PowerValue := VarRecord.Value2
          else
            PowerValue := VarRecord.Value0;

          if PowerValue <> 0 then
          begin
            I64 := I64 + nVarValue * PowerValue;
          end;
        end;
      end;
    finally
      g_CombatPowerVarMgr.UnLock;
    end;
  end;

  SmartObject.m_nCombatPower := I64;

end;

{ TCombatPowerVarMgr }

constructor TCombatPowerVarMgr.Create;
begin
  InitializeCriticalSection(FCriticalSection);
  FVarList := TList.Create;
  FSortVarList := TList.Create;
end;

destructor TCombatPowerVarMgr.Destroy;
begin
  DeleteCriticalSection(FCriticalSection);
  Clear;
  FVarList.Free;
  FSortVarList.Free;
  inherited;
end;

procedure TCombatPowerVarMgr.Clear;
var
  I: Integer;
begin
  FSortVarList.Clear;
  for I := 0 to FVarList.Count - 1 do
  begin
    Dispose(PCombatPowerVarRecord(FVarList[I]));
  end;
  FVarList.Clear;
end;

function TCombatPowerVarMgr.Add(const VarName: string; Value0, Value1, Value2: Integer; Desc: string): PCombatPowerVarRecord;
var
  Index: Integer;
begin
  Result := nil;
  if not DoSearch(VarName, Index) then
  begin
    New(Result);
    Result.VarName := VarName;
    Result.Value0 := Value0;
    Result.Value1 := Value1;
    Result.Value2 := Value2;
    Result.Desc := Desc;

    FVarList.Add(Result);
    FSortVarList.Insert(Index, Result);
  end;
end;

function TCombatPowerVarMgr.Remove(const VarName: string): Boolean;
var
  Index: Integer;
  Rec: PCombatPowerVarRecord;
begin
  Result := False;

  if DoSearch(VarName, Index) then
    FSortVarList.Delete(Index);

  for Index := 0 to FVarList.Count - 1 do
  begin
    Rec := FVarList.Items[Index];
    if SameText(Rec.VarName, VarName) then
    begin
      FVarList.Delete(Index);
      Dispose(Rec);
      Result := True;
    end;
  end;
end;

function TCombatPowerVarMgr.DoSearch(const VarName: string; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Result := False;
  L := 0;
  H := Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := AnsiCompareText(PCombatPowerVarRecord(FSortVarList[I]).VarName, VarName);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        //if Duplicates <> dupAccept then L := I;
      end;
    end;
  end;
  Index := L;
end;

function TCombatPowerVarMgr.GetCount: Integer;
begin
  Result := FVarList.Count;
end;

procedure TCombatPowerVarMgr.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TCombatPowerVarMgr.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

function TCombatPowerVarMgr.GetValueRecord(const VarName: string): PCombatPowerVarRecord;
var
  Index: Integer;
begin
  Result := nil;
  if DoSearch(VarName, Index) then
  begin
    Result := FSortVarList.Items[Index];
  end;
end;

function TCombatPowerVarMgr.GetItems(Index: Integer): PCombatPowerVarRecord;
begin
  Result := FVarList.Items[Index];
end;

procedure TCombatPowerVarMgr.LoadConfig;
var
  I: Integer;
  SL: TStringList;
  FileName, Section, Desc: string;
  Value0, Value1, Value2: Integer;
  IniFile: TIniFile;
begin
  FileName := g_Config.sEnvirDir + 'CombatPower.ini';
  if not FileExists(FileName) then
    Exit;

  Clear;

  IniFile := TIniFile.Create(FileName);

  SL := TStringList.Create;
  try
    IniFile.ReadSections(SL);

    for I := 0 to SL.Count - 1 do
    begin
      Section := SL.Strings[I];
      if SameText('DefaultAttrib', Section) then
        Continue;

      Value0 := IniFile.ReadInteger(Section, 'Value0', 0);
      Value1 := IniFile.ReadInteger(Section, 'Value1', 0);
      Value2 := IniFile.ReadInteger(Section, 'Value2', 0);
      Desc := IniFile.ReadString(Section, 'Desc', '');

      Add(Section, Value0, Value1, Value2, Desc);
    end;
  finally
    SL.Free;
  end;

  IniFile.Free;
end;

procedure TCombatPowerVarMgr.SaveConfig;
var
  I: Integer;
  FileName: string;
  IniFile: TIniFile;
  Rec: PCombatPowerVarRecord;
  SL: TStringList;
begin
  FileName := g_Config.sEnvirDir + 'CombatPower.ini';
  IniFile := TIniFile.Create(FileName);

  SL := TStringList.Create;
  try
    IniFile.ReadSections(SL);

    for I := 0 to SL.Count - 1 do
    begin
      if not SameText(SL.Strings[I], 'DefaultAttrib') then
      begin
        IniFile.EraseSection(SL.Strings[I]);
      end;
    end;
  finally
    SL.Free;
  end;
  for I := 0 to FVarList.Count - 1 do
  begin
    Rec := FVarList.Items[I];

    IniFile.WriteInteger(Rec.VarName, 'Value0', Rec.Value0);
    IniFile.WriteInteger(Rec.VarName, 'Value1', Rec.Value1);
    IniFile.WriteInteger(Rec.VarName, 'Value2', Rec.Value2);

    IniFile.WriteString(Rec.VarName, 'Desc', Rec.Desc);
  end;
  IniFile.Free;
end;

end.

