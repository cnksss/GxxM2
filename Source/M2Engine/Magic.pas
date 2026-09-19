unit Magic;

interface

uses
  Windows, Classes, Grobal2, ObjBase, ObjHero, SysUtils, EDcode, ObjPlayer, M2Threads, M2Definition, uMagicACUtils;

type
  TMagicManager = class
  private
    FNpcReleaseMagic: Byte;
    function CheckMagLighteningType(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
      TargeTBaseObject: TBaseObject): Byte;
  public
    constructor Create();
    destructor Destroy; override;
    function MagMakePrivateTransparent(BaseObject: TBaseObject; nHTime: Integer): Boolean;
    function IsWarrSkill(wMagIdx: Integer): Boolean;
    // 施展技能
    function DoSpell(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject: TBaseObject;
      FormClient: Boolean = False; btNpcReleaseMagic: Byte = 0; boDoNotCheck: Boolean = False): Boolean;
    // 检测技能CD --- 尚未冷却返回F，已冷却返回T   --- piaoyun 2013-06-27
    function CheckCD(BaseObject: TSmartObject; SkillID: LongWord; CdTime: LongWord; boDoNotCheck: Boolean): Boolean;
    function MagBigHealing(PlayObject: TSmartObject; nPower, nX, nY: Integer): Boolean;
    function MagPushArround(PlayObject: TSmartObject; UserMagic: pTUserMagic; nPushLevel: Integer): Integer;
    function MagTurnUndead(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY: Integer; nLevel: Integer):
      Boolean;
    function MagMakeHolyCurtain(BaseObject: TSmartObject; nPower: Integer; nX, nY: Integer): Integer;
    function MagMakeImprison(BaseObject: TSmartObject; nTime, nRange: Integer; nX, nY: Integer): Integer;
    function MagMakeGroupTransparent(BaseObject: TSmartObject; nX, nY: Integer; nHTime: Integer): Boolean;
    function MagTamming(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY: Integer; nMagicLevel: Integer):
      Boolean;
    function MagSpiritualism(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nMagicLevel,
      nMagicNewLevel: Integer): Boolean; // 00492368
    function MagSaceMove(BaseObject: TSmartObject; nLevel: Integer): Boolean;
    function MagMakeFireCross(PlayObject: TSmartObject; nDamage, nHTime, nX, nY, nNewLevel: Integer): Integer;
    function MagBigExplosion(BaseObject: TSmartObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer; nRage, PowerRate: Integer):
      Boolean;
    function MagElecBlizzard(BaseObject: TSmartObject; UserMagic: pTUserMagic; nPower: Integer): Boolean;
    function MabMabe(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nLevel, nTargetX,
      nTargetY: Integer): Boolean;
    function MagMakeSlave(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
    function MagMakeMoon(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
    function MagMakeSinSuSlave(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
    function MagMakeBigDogSlave(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
    function MagWindTebo(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
    function MagGroupLightening(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject; var boSpellFire: Boolean): Boolean;
    function MagGroupAmyounsul(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject; var boSpellFail: Boolean): Boolean;
    // 彻地钉技能函数修改 -- 攻击范围 威力倍数 -- piaoyun 2013-06-27
    function MagGroupDeDing(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject): Boolean;
    // 狮子吼技能函数修改 -- 麻痹时间 -- piaoyun 2013-06-27
    function MagGroupMb(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject): Boolean;
    function MagHbFireBall(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean;
    function MagReturn(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nMagicLevel: Integer): Boolean;
    function MagLightening(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject; var boSpellFire: Boolean): Boolean;
    function MagMakeSuperFireCross(PlayObject: TSmartObject; nDamage, nHTime, nX, nY: Integer; nCount: Integer): Integer;
    function MagMakeFireball(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean;
    function MagTreatment(PlayObject: TSmartObject; UserMagic: pTUserMagic; var nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeHellFire(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeQuickLighting(PlayObject: TSmartObject; UserMagic: pTUserMagic; var nTargetX, nTargetY: Integer;
      TargeTBaseObject: TBaseObject): Boolean;
    function MagMakeLighting(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeFireCharm(PlayObject: TBaseObject; { TSmartObject修改 TBaseObject } UserMagic: pTUserMagic; nTargetX, nTargetY:
      Integer; var TargeTBaseObject: TBaseObject; boMove: Boolean): Boolean;
    function MagMakeUnTreatment(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
      TargeTBaseObject: TBaseObject): Boolean;
    function MagMakeLivePlayObject(PlayObject: TSmartObject; UserMagic: pTUserMagic; TargeTBaseObject: TBaseObject): Boolean;
    function MagMakeArrestObject(PlayObject: TSmartObject; UserMagic: pTUserMagic; TargeTBaseObject: TBaseObject): Boolean;
    function MagChangePosition(PlayObject: TSmartObject; nTargetX, nTargetY: Integer): Boolean;
    function MagMakeFireDay(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeCopySelf(PlayObject: TSmartObject; nTargetX, nTargetY: Integer; var TargeTBaseObject: TBaseObject; MagicLevel,
      MagicNewLevel: Byte): Boolean;
    // 死亡之眼技能函数 -- piaoyun 2013-06-24
    function MagBigExplosionAndMakePoison(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer): Boolean;
    // 十步一杀技能函数 -- piaoyun 2013-06-25
    function MagBigExplosionAndMakePoisonByWarr(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer; var
      boMove: Boolean): Boolean;
    // 冰霜雪雨技能函数 -- piaoyun 2013-06-26

      // function MagDoubleBigExplosionEx(PlayObject: TSmartObject; nPower, nX, nY: Integer; nRage, nMagID: Integer; boDecMP: Boolean = False): Boolean;
    function MagDoubleBigExplosionEx(BaseObject: TBaseObject; nPower, nX, nY: Integer; nMagID: Integer; boDecMP: Boolean = False):
      Boolean;
    // 冰霜群雨技能函数 -- piaoyun 2013-06-26
    function MagBigExplosionAndMakePoisonEx(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer
      { ; nRage: Integer } ): Boolean;
    // 旋风斩技能函数 -- piaoyun 2013-09-14
    function MagMakeSkill208(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower: Integer): Boolean;
    // 五雷轰技能函数 -- piaoyun 2013-09-14
    function MagMakeSkill209(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer { ; nRage: Integer } ):
      Boolean;
    // 幽冥火符技能函数 -- piaoyun 2013-09-14
    function MagMakeSkill210(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer { ; nRage: Integer } ):
      Boolean;
    function MagMakeSkillFire_60(BaseObject: TSmartObject; UserMagic: pTUserMagic; TargeTBaseObject: TBaseObject): Boolean;
    function MagMakeSkillFire_61(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
      TargeTBaseObject: TBaseObject): Boolean;
    function MagMakeSkillFire_62(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
      TargeTBaseObject: TBaseObject): Boolean;
    function MagMakeSkillFire_63(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeSkillFire_64(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeSkillFire_65(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
    function MagAbsorbBlood(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean;
    // 流星火雨技能函数
    function MagMeteoriteRain(BaseObject: TSmartObject; UserMagic: pTUserMagic; nX, nY: Integer): Boolean;
    // 流星火雨修改 -- piaoyun 2013-06-26
    // function MagBigExplosionEx(BaseObject: TBaseObject; nPower, nX, nY: Integer; nRage: Integer): Boolean;
    function MagGroupFengPo(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
      TBaseObject): Boolean;
    function MagMakeSkill104(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean; // 凤舞祭
    function MagMakeSkill105(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean; // 惊雷爆
    function MagMakeSkill106(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean; // 冰天雪地
    function MagMakeSkill107(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean; // 双龙破
    function MagMakeSkill108(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean; // 虎啸诀
    function MagMakeSkill109(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean; // 八卦掌
    function MagMakeSkill110(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject): Boolean; // 三焰咒
    function MagMakeSkill111(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean; // 万剑归宗
    // 倚天辟地技能函数修改 -- piaoyun 2013-06-27
    function MagMakeSkill114(BaseObject: TSmartObject; UserMagic: pTUserMagic; var nTargetX, nTargetY: Integer): Boolean;
    // 血魄一击技能函数
    function MagMakeSkill116(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
    function MagMakeSkill117(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
    function MagMakeNewSkill_Test(BaseObject: TSmartObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer; nRage: Integer):
      Boolean;
    function MagCapturePets(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY: Integer; nMagicLevel:
      Integer): Boolean;
    function MagCustomSkill(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject:
      TBaseObject; var sMsg: string; var boMoved: Boolean; btNpcReleaseMagic: Byte; var boSpellFire: Boolean): Boolean;
  end;

function MPow(BaseObject: TBaseObject; UserMagic: pTUserMagic): Integer;

function GetPower(BaseObject: TBaseObject; nPower: Integer; UserMagic: pTUserMagic): Integer;

function GetPower13(BaseObject: TBaseObject; nInt: Integer; UserMagic: pTUserMagic): Integer;

function GetRPow(BaseObject: TBaseObject; nInt1, nInt2: Integer): Integer;

function CheckAmulet(BaseObject: TSmartObject; var nShape: Integer; nCount: Integer; var Idx: Integer): Boolean;

procedure UseAmulet(BaseObject: TSmartObject; nCount: Integer; var Idx: Integer);

function GetNewLevelPower(nPower: Integer; UserMagic: pTUserMagic): Integer;

implementation

uses
  Math, HUtil32, M2Share, GameEvent, Envir, uCustomMagicUtils, uCustomMonsterUtils, ItemEvent, ObjSmartMon;

{ TODO -ochongchong -c新增 : 增加强化技能9重后威力倍数 【2013-08-18】 }
function GetNewLevelPower(nPower: Integer; UserMagic: pTUserMagic): Integer;
var
  Index: Integer;
  Level9Power: Double;
  Rate: Double;
  I64: Int64;
begin
  if (UserMagic <> nil) and (UserMagic.btNewLevel in [1..99]) and (nPower > 0) then
  begin
    Index := -1;
    case UserMagic.MagicInfo.wMagicId of
      3:
        Index := 0; // 基本剑术
      7:
        Index := 1; // 攻杀剑术
      12:
        Index := 2; // 刺杀剑术
      25:
        Index := 3; // 半月弯刀
      26:
        Index := 4; // 烈火剑法
      56:
        Index := 5; // 逐日剑法
      58:
        Index := 20; // 流星火雨
      22:
        Index := 21; // 火墙
      31:
        Index := 22; // 魔法盾
      11:
        Index := 23; // 雷电术
      45:
        Index := 24; // 灭天火
      33:
        Index := 25; // 冰咆哮
      6:
        Index := 40; // 施毒术
      51:
        Index := 40;
      // 群体施毒术SKILL_GROUPAMYOUNSUL chongchong 2014-09-15
      14:
        Index := 41; // 幽灵盾
      15:
        Index := 42; // 神圣战甲术
      13:
        Index := 43; // 灵魂火符
      57:
        Index := 44; // 噬血术
      52:
        Index := 45; // 飓风破
    end;
    if Index = -1 then
    begin
      if UserMagic.btNewLevel <= 9 then
        I64 := Round(nPower * (g_Config.NewLevelMagicPowerRates[UserMagic.btNewLevel - 1] / 100))
      else
      begin
        Level9Power := nPower * (g_Config.NewLevelMagicPowerRates[9 - 1] / 100);
        Rate := 1 + (UserMagic.btNewLevel - 9) * (g_Config.NewLevelMagicPowerRatesAfter9 / 100);
        I64 := Round(Level9Power * Rate);
      end;
    end
    else
    begin
      if UserMagic.btNewLevel <= 9 then
        I64 := Round(nPower * (g_Config.NewLevelMagicPowerRatesSpecific[Index][UserMagic.btNewLevel - 1] / 100))
      else
      begin
        Level9Power := nPower * (g_Config.NewLevelMagicPowerRatesSpecific[Index][9 - 1] / 100);
        Rate := 1 + (UserMagic.btNewLevel - 9) * (g_Config.NewLevelMagicPowerRatesAfter9 / 100);
        I64 := Round(Level9Power * Rate);
      end;
    end;
  end
  else
    I64 := nPower;
  Result := Min(I64, High(Integer));
end;

function MPow(BaseObject: TBaseObject; UserMagic: pTUserMagic): Integer;
var
  PowerSub: Integer;
begin
  if (UserMagic.MagicInfo <> nil) and (UserMagic.MagicInfo.wMaxPower >= UserMagic.MagicInfo.wPower) then
    PowerSub := UserMagic.MagicInfo.wMaxPower - UserMagic.MagicInfo.wPower
  else
    PowerSub := 0;
  if (BaseObject.m_nLuck > 0) and (BaseObject.m_nLuck >= g_Config.nMaxLuckMaxPower) then
    Result := Min(LongWord(UserMagic.MagicInfo.wPower + PowerSub), High(Integer))
  else
    Result := UserMagic.MagicInfo.wPower + Random(PowerSub);
end;

function GetPower(BaseObject: TBaseObject; nPower: Integer; UserMagic: pTUserMagic): Integer;
var
  DefPowerSub: Integer;
begin
  if (UserMagic.MagicInfo <> nil) and (UserMagic.MagicInfo.wDefMaxPower > UserMagic.MagicInfo.wDefPower) then
    DefPowerSub := UserMagic.MagicInfo.wDefMaxPower - UserMagic.MagicInfo.wDefPower
  else
    DefPowerSub := 0;
  if (BaseObject.m_nLuck > 0) and (BaseObject.m_nLuck >= g_Config.nMaxLuckMaxPower) then
    Result := Min(LongWord(Round(nPower / (UserMagic.MagicInfo.btTrainLv + 1) * (UserMagic.btLevel + 1)) + (UserMagic.MagicInfo.wDefPower
      + DefPowerSub)), High(Integer))
  else
    Result := Min(LongWord(Round(nPower / (UserMagic.MagicInfo.btTrainLv + 1) * (UserMagic.btLevel + 1)) + (UserMagic.MagicInfo.wDefPower
      + Random(DefPowerSub))), High(Integer));
end;

function GetPower13(BaseObject: TBaseObject; nInt: Integer; UserMagic: pTUserMagic): Integer;
var
  d10: Double;
  d18: Double;
  Int64Value: Int64;
begin
  d10 := nInt / 3.0;
  d18 := nInt - d10;
  if (BaseObject.m_nLuck > 0) and (BaseObject.m_nLuck >= g_Config.nMaxLuckMaxPower) then
  begin
    if UserMagic.MagicInfo <> nil then
      Int64Value := Round(d18 / (UserMagic.MagicInfo.btTrainLv + 1) * Int64(UserMagic.btLevel + 1) + d10 + (UserMagic.MagicInfo.wDefPower
        + Max(UserMagic.MagicInfo.wDefMaxPower - UserMagic.MagicInfo.wDefPower, 0)))
    else
      Int64Value := Round(d18);
    Result := Min(Int64Value, High(Integer))
  end
  else
  begin
    if UserMagic.MagicInfo <> nil then
      Int64Value := Round(d18 / (UserMagic.MagicInfo.btTrainLv + 1) * Int64(UserMagic.btLevel + 1) + d10 + (UserMagic.MagicInfo.wDefPower
        + Random(UserMagic.MagicInfo.wDefMaxPower - UserMagic.MagicInfo.wDefPower)))
    else
      Int64Value := Round(d18);
    Result := Min(Int64Value, High(Integer));
  end;
end;

function GetRPow(BaseObject: TBaseObject; nInt1, nInt2: Integer): Integer;
begin
  if nInt2 > nInt1 then
  begin
    if (BaseObject.m_nLuck > 0) and (BaseObject.m_nLuck >= g_Config.nMaxLuckMaxPower) then
      Result := Min(LongWord(Max(nInt2 - nInt1 + 1, 1) + nInt1), High(Integer))
    else
      Result := Min(LongWord(Random(nInt2 - nInt1 + 1) + nInt1), High(Integer));
  end
  else
    Result := nInt1;
end;

function CheckBagAmulet(BaseObject: TSmartObject; var nShape: Integer; nCount: Integer; var Idx: Integer; boEqual: Boolean = False):
  Boolean;
var
  AmuletStdItem: pTStdItem;
  UserItem: pTUserItem;
  boNeedMagicItem: Boolean;
begin
  Result := False;
  Idx := -1;
  boNeedMagicItem := False;
  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
  begin // 检测人物需要毒符
    if TPlayObject(BaseObject).m_boDummyObject then
    begin // 假人
      if TPlayObject(BaseObject).m_nDummyNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := TPlayObject(BaseObject).m_nDummyNeedMagicItem = 2;
      end;
    end
    else
    begin
      if g_Config.nHumNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := g_Config.nHumNeedMagicItem = 2;
      end;
    end;
  end
  else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
  begin // 检测英雄需要毒符
    if THeroObject(BaseObject).m_boDummyObject then
    begin // 假人英雄
      if THeroObject(BaseObject).m_nDummyNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := THeroObject(BaseObject).m_nDummyNeedMagicItem = 2;
      end;
    end
    else
    begin
      if g_Config.nHeroNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := g_Config.nHeroNeedMagicItem = 2;
      end;
    end;
  end
  else if (BaseObject.m_btRaceServer <> RC_PLAYOBJECT) then
  begin // 人形怪需要毒符
    if (g_Config.nMonsterNeedMagicItem = 0) or (BaseObject.m_Master <> nil) then
    begin
      Result := True;
    end
    else
    begin
      boNeedMagicItem := g_Config.nMonsterNeedMagicItem = 2;
    end;
  end;
  if boNeedMagicItem then
  begin
    if boEqual then
    begin
      Idx := GetUserItemListEx(BaseObject, nShape, nCount);
      if (Idx < 0) then
      begin
        Idx := GetUserItemList(BaseObject, nShape, nCount);
        boEqual := False;
        // MainOutMessage('boEqual := False:' + inttostr(Idx) + ' nShape:' + inttostr(nShape));
      end;
    end
    else
    begin
      Idx := GetUserItemList(BaseObject, nShape, nCount);
    end;
    // MainOutMessage('CheckBagAmulet1:'+inttostr(Idx)+' nShape:'+inttostr(nShape));
    if (Idx >= 0) and (Idx < BaseObject.m_ItemList.Count) then
    begin
      UserItem := BaseObject.m_ItemList.Items[Idx];
      if (UserItem.wIndex > 0) then
      begin
        AmuletStdItem := UserEngine.GetStdItem(UserItem.wIndex);
        // MainOutMessage('CheckBagAmulet2:'+inttostr(Idx)+' nShape:'+inttostr(nShape));
        if (AmuletStdItem <> nil) and (AmuletStdItem.StdMode = 25) then
        begin
          // MainOutMessage('CheckBagAmulet3:'+inttostr(Idx)+' nShape:'+inttostr(nShape)+' AmuletStdItem.Shape:'+inttostr(AmuletStdItem.Shape));
          if UserItem.Dura <= 0 then
          begin
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
              TPlayObject(BaseObject).SendDelItem(UserItem)
            else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
              THeroObject(BaseObject).SendDelItem(UserItem);
            BaseObject.m_ItemList.Delete(Idx);
            Dispose(UserItem);
            Exit;
          end;
          case nShape of
            5:
              begin
                if (AmuletStdItem.Shape = nShape) and ((Round(UserItem.Dura / g_Config.nMagicItemRate) >= nCount) or (UserItem.Dura
                  > 0)) then
                begin
                  Inc(Idx, Length(BaseObject.m_UseItems));
                  Result := True;
                  Exit;
                end;
              end;
            1, 2:
              begin
                // MainOutMessage('CheckBagAmulet4:'+inttostr(Idx)+' nShape:'+inttostr(nShape));
                if boEqual then
                begin
                  if (AmuletStdItem.Shape = nShape) and ((Round(UserItem.Dura / g_Config.nMagicItemRate) >= nCount) or (UserItem.Dura
                    > 0)) then
                  begin
                    Inc(Idx, Length(BaseObject.m_UseItems));
                    nShape := AmuletStdItem.Shape;
                    Result := True;
                    Exit;
                  end;
                end
                else
                begin
                  if (AmuletStdItem.Shape in [1, 2]) and ((Round(UserItem.Dura / g_Config.nMagicItemRate) >= nCount) or (UserItem.Dura
                    > 0)) then
                  begin
                    Inc(Idx, Length(BaseObject.m_UseItems));
                    nShape := AmuletStdItem.Shape;
                    Result := True;
                    Exit;
                  end;
                end;
              end;
          end;
        end;
      end;
    end;
  end;
end;

// nType 为指定类型 1 为护身符 2 为毒药  3诅咒符    Idx = -1; 不需要毒 符  Idx >= 13 使用包裹中的  Idx < 13 使用身上的
function CheckAmulet(BaseObject: TSmartObject; var nShape: Integer; nCount: Integer; var Idx: Integer): Boolean;
var
  AmuletStdItem: pTStdItem;
  boNeedMagicItem: Boolean;
begin
  Result := False;
  Idx := -1;
  boNeedMagicItem := False;
  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
  begin // 检测人物需要毒符
    if TPlayObject(BaseObject).m_boDummyObject then
    begin // 机器人
      if TPlayObject(BaseObject).m_nDummyNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := True;
      end;
    end
    else
    begin
      if g_Config.nHumNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := True;
      end;
    end;
  end
  else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
  begin // 英雄需要毒 符
    if THeroObject(BaseObject).m_boDummyObject then
    begin // 机器人
      if THeroObject(BaseObject).m_nDummyNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := True;
      end;
    end
    else
    begin
      if g_Config.nHeroNeedMagicItem = 0 then
      begin
        Result := True;
      end
      else
      begin
        boNeedMagicItem := True;
      end;
    end;
  end
  else if (BaseObject.m_btRaceServer <> RC_PLAYOBJECT) then
  begin // 人形怪需要毒 符
    if (g_Config.nMonsterNeedMagicItem = 0) or (BaseObject.m_Master <> nil) then
    begin
      Result := True;
    end
    else
    begin
      boNeedMagicItem := True;
    end;
  end
  else
  begin
    boNeedMagicItem := True;
  end;
  if boNeedMagicItem then
  begin
    if BaseObject.m_UseItems[U_ARMRINGL].wIndex > 0 then
    begin
      AmuletStdItem := UserEngine.GetStdItem(BaseObject.m_UseItems[U_ARMRINGL].wIndex);
      if (AmuletStdItem <> nil) and (AmuletStdItem.StdMode = 25) then
      begin
        if BaseObject.m_UseItems[U_ARMRINGL].Dura <= 0 then
        begin
          if (AmuletStdItem.NeedIdentify = 1) then
          begin
            AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, BaseObject, AmuletStdItem.Name, BaseObject.m_UseItems[U_ARMRINGL].MakeIndex,
              '0', 0, 0, '0持久消失');
          end;
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            TPlayObject(BaseObject).SendDelItem(@BaseObject.m_UseItems[U_ARMRINGL])
          else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
            THeroObject(BaseObject).SendDelItem(@BaseObject.m_UseItems[U_ARMRINGL]);
          BaseObject.m_UseItems[U_ARMRINGL].wIndex := 0;
          Exit;
        end;
        case nShape of
          5:
            begin // 符咒
              if (AmuletStdItem.Shape = nShape) and ((Round(BaseObject.m_UseItems[U_ARMRINGL].Dura / g_Config.nMagicItemRate) >=
                nCount) or (BaseObject.m_UseItems[U_ARMRINGL].Dura > 0)) then
              begin
                Idx := U_ARMRINGL;
                Result := True;
                Exit;
              end;
            end;
          1, 2:
            begin // 毒
              if (AmuletStdItem.Shape in [1, 2]) and ((Round(BaseObject.m_UseItems[U_ARMRINGL].Dura / g_Config.nMagicItemRate) >=
                nCount) or (BaseObject.m_UseItems[U_ARMRINGL].Dura > 0)) then
              begin
                nShape := AmuletStdItem.Shape;
                Idx := U_ARMRINGL;
                Result := True;
                Exit;
              end;
              // end;
            end;
        end;
      end;
    end;
    if BaseObject.m_UseItems[U_BUJUK].wIndex > 0 then
    begin
      AmuletStdItem := UserEngine.GetStdItem(BaseObject.m_UseItems[U_BUJUK].wIndex);
      if (AmuletStdItem <> nil) and (AmuletStdItem.StdMode = 25) then
      begin
        if BaseObject.m_UseItems[U_BUJUK].Dura <= 0 then
        begin
          if (AmuletStdItem.NeedIdentify = 1) then
          begin
            AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, BaseObject, AmuletStdItem.Name, BaseObject.m_UseItems[U_BUJUK].MakeIndex,
              '0', 0, 0, '0持久消失');
          end;
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            TPlayObject(BaseObject).SendDelItem(@BaseObject.m_UseItems[U_BUJUK])
          else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
            THeroObject(BaseObject).SendDelItem(@BaseObject.m_UseItems[U_BUJUK]);
          BaseObject.m_UseItems[U_BUJUK].wIndex := 0;
          Exit;
        end;
        case nShape of
          5:
            begin
              if (AmuletStdItem.Shape = nShape) and ((Round(BaseObject.m_UseItems[U_BUJUK].Dura / g_Config.nMagicItemRate) >=
                nCount) or (BaseObject.m_UseItems[U_BUJUK].Dura > 0)) then
              begin
                Idx := U_BUJUK;
                Result := True;
                Exit;
              end;
            end;
          1, 2:
            begin // 毒
              if (AmuletStdItem.Shape in [1, 2]) and ((Round(BaseObject.m_UseItems[U_BUJUK].Dura / g_Config.nMagicItemRate) >=
                nCount) or (BaseObject.m_UseItems[U_BUJUK].Dura > 0)) then
              begin
                Idx := U_BUJUK;
                nShape := AmuletStdItem.Shape;
                Result := True;
                Exit;
              end;
              // end;
            end;
        end;
      end;
    end;
  end;
end;

// nType 为指定类型 1 为护身符 2 为毒药  3诅咒符
procedure UseAmulet(BaseObject: TSmartObject; nCount: Integer; var Idx: Integer);
var
  UserItem: pTUserItem;
  nNeedMagicItem: Integer;
begin
  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
  begin // 检测人物需要毒符
    if TPlayObject(BaseObject).m_boDummyObject then
    begin // 假人
      nNeedMagicItem := TPlayObject(BaseObject).m_nDummyNeedMagicItem;
    end
    else
    begin
      nNeedMagicItem := g_Config.nHumNeedMagicItem;
    end;
  end
  else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
  begin // 英雄需要毒 符
    if THeroObject(BaseObject).m_boDummyObject then
    begin // 假人英雄
      nNeedMagicItem := THeroObject(BaseObject).m_nDummyNeedMagicItem;
    end
    else
    begin
      nNeedMagicItem := g_Config.nHeroNeedMagicItem;
    end;
  end
  else if (BaseObject.m_btRaceServer <> RC_PLAYOBJECT) then
  begin // 人形怪需要毒 符
    if (BaseObject.m_Master <> nil) then
      nNeedMagicItem := 0
    else
      nNeedMagicItem := g_Config.nMonsterNeedMagicItem;
  end
  else
  begin
    nNeedMagicItem := 1;
  end;
  if (Idx >= Length(BaseObject.m_UseItems)) and (nNeedMagicItem in [1, 2]) then
  begin // 直接使用包裹中物品
    Dec(Idx, Length(BaseObject.m_UseItems));
    if (Idx >= 0) and (Idx < BaseObject.m_ItemList.Count) then
    begin
      UserItem := BaseObject.m_ItemList.Items[Idx];
      if UserItem.Dura > nCount * g_Config.nMagicItemRate then
      begin
        Dec(UserItem.Dura, nCount * g_Config.nMagicItemRate);
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          TPlayObject(BaseObject).SendUpdateItemDura(Idx, UserItem.MakeIndex, False, UserItem.Dura)
        else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
          THeroObject(BaseObject).SendUpdateItemDura(Idx, UserItem.MakeIndex, UserItem.Dura);
      end
      else
      begin
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          TPlayObject(BaseObject).SendDelItem(UserItem)
        else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
          THeroObject(BaseObject).SendDelItem(UserItem);
        BaseObject.m_ItemList.Delete(Idx);
        Dispose(UserItem);
      end;
    end;
  end
  else if (Idx in [Low(THumanUseItems)..High(THumanUseItems)]) and (nNeedMagicItem in [1, 2]) then
  begin // 直接使用身上的物品
    if BaseObject.m_UseItems[Idx].Dura > nCount * g_Config.nMagicItemRate then
    begin
      Dec(BaseObject.m_UseItems[Idx].Dura, nCount * g_Config.nMagicItemRate);
      BaseObject.SendMsg(BaseObject, RM_DURACHANGE, Idx, BaseObject.m_UseItems[Idx].Dura, BaseObject.m_UseItems[Idx].DuraMax, 0,
        '');
    end
    else
    begin
      BaseObject.m_UseItems[Idx].Dura := 0;
      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        TPlayObject(BaseObject).SendDelItem(@BaseObject.m_UseItems[Idx]);
      if BaseObject.m_btRaceServer = RC_HEROOBJECT then
        THeroObject(BaseObject).SendDelItem(@BaseObject.m_UseItems[Idx]);
      BaseObject.m_UseItems[Idx].wIndex := 0;
    end;
  end;
end;

function TMagicManager.MagPushArround(PlayObject: TSmartObject; UserMagic: pTUserMagic; nPushLevel: Integer): Integer;
var
  I, nDir, levelgap, push, nValue: Integer;
  BaseObject: TBaseObject;
  boPushSameLevel: Boolean;
  VisibleBaseObject: pTVisibleBaseObject;
begin
  Result := 0;
  if UserMagic.wMagIdx = SKILL_FIREWIND then
    boPushSameLevel := g_Config.boFireWindPushSameLevel
  else
    boPushSameLevel := g_Config.boQigongPushSameLevel;
  PlayObject.m_VisibleActors.Lock;
  try
    for I := 0 to PlayObject.m_VisibleActors.Count - 1 do
    begin
      VisibleBaseObject := PlayObject.m_VisibleActors[I].Item;
      if VisibleBaseObject <> nil then
      begin
        BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
        if (abs(PlayObject.m_nCurrX - BaseObject.m_nCurrX) <= 1) and (abs(PlayObject.m_nCurrY - BaseObject.m_nCurrY) <= 1) then
        begin
          if (not BaseObject.m_boDeath) and (BaseObject <> PlayObject) and (not BaseObject.m_boStickMode) then
          begin
            if ((PlayObject.m_Abil.Level > BaseObject.m_Abil.Level) or (boPushSameLevel and (PlayObject.m_Abil.Level = BaseObject.m_Abil.Level)))
              then
            begin
              levelgap := PlayObject.m_Abil.Level - BaseObject.m_Abil.Level;
              if boPushSameLevel and (PlayObject.m_Abil.Level = BaseObject.m_Abil.Level) then
                nValue := Random(10)
              else
                nValue := Random(20);
              if (nValue < 6 + nPushLevel * 3 + levelgap) then
              begin
                if PlayObject.IsProperTarget(BaseObject) then
                begin
                  push := 1 + _MAX(0, nPushLevel - 1) + Random(2);
                  nDir := GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, BaseObject.m_nCurrX, BaseObject.m_nCurrY);
                  BaseObject.CharPushed(nDir, push);
                  Inc(Result);
                end;
              end;
            end;
          end;
        end;
      end;
    end;
  finally
    PlayObject.m_VisibleActors.UnLock;
  end;
end;

function TMagicManager.MagBigHealing(PlayObject: TSmartObject; nPower, nX, nY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
begin
  Result := False;
  BaseObjectList := TList.Create;
  PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, nX, nY, 1, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    BaseObject := TBaseObject(BaseObjectList[I]);
    if PlayObject.IsProperFriend(BaseObject) then
    begin
      if BaseObject.m_WAbil.HP < BaseObject.m_WAbil.MaxHP then
      begin
        BaseObject.SendDelayMsg(PlayObject, RM_MAGHEALING, 0, nPower, 0, SKILL_BIGHEALLING, '', 800);
        // BaseObject.SendMsg(PlayObject, RM_MAGHEALING, 0, nPower, 0, 0, '');
        Result := True;
      end;
      if PlayObject.m_boAbilSeeHealGauge then
      begin
        PlayObject.SendMsg(BaseObject, RM_10414, 0, 0, 0, 0, ''); // ?? RM_INSTANCEHEALGUAGE
      end;
    end;
  end;
  BaseObjectList.Free;
end;

constructor TMagicManager.Create;
begin
end;

destructor TMagicManager.Destroy;
begin
  inherited;
end;

// 噬血术
function TMagicManager.MagAbsorbBlood(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  nAddHP: LongWord;
  Int64Value: Int64;
begin
  Result := False;
  if BaseObject.IsProperTarget(TargeTBaseObject) then
  begin
    if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then
    begin
      if ((abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1)) or (BaseObject.m_btRaceServer
        = RC_HEROOBJECT) then
      begin
        nPower := BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + BaseObject.m_WAbil.SC1
          * 2, Integer(BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1) * 2 + 1);
        nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
        nPower := GetSkillLastPowerNG(nPower, // 内功技能威力
          GetSkillAttackPowerNG(BaseObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
        Int64Value := Round(nPower / 100 * g_Config.nSkill57PowerRate);
        nPower := Min(Int64Value, High(Integer));
        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), MakeLong(2, 0), NativeInt(TargeTBaseObject),
            IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), MakeLong(2, 0), NativeInt(TargeTBaseObject),
            IntToStr(UserMagic.wMagIdx), 300); // 噬血术技能延时  500  chongchong 2018-06-01
        // 嗜血术吸血动画再次修改。等目标目标后，自身的吸血动作 chongchong 2014-05-19
        BaseObject.SendRefMsg(RM_10205, 0, 0, 0, SKILL_57, IntToStr(UserMagic.btNewLevel), 600);
        // 900  chongchong 2018-06-01
        // 吸血（给自己加血) chongchong 2014-05-20
        if BaseObject.m_WAbil.HP < BaseObject.m_WAbil.MaxHP then
        begin
          // 加身时要延时 chongchong 2014-05-20
          // BaseObject.IncHealthSpell(Min(LongWord(Round(nPower * (g_Config.nSkill57AddHPRate / 100))), High(Integer)), 0);
          Int64Value := Round(nPower / 100 * g_Config.nSkill57AddHPRate);
          nAddHP := Min(Int64Value, High(LongWord));
          BaseObject.SendDelayMsg(nil, RM_INCHEALTH, 0, nAddHP, 0, 0, '', 1000);
        end;
        if (BaseObject.m_btRaceServer = RC_PLAYMOSTER) or (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
          Result := True
        else if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
          Result := True;
      end;
    end
    else
    begin
      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
    end;
  end;
end;

{ 火球 }
function TMagicManager.MagMakeFireball(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower, nPowerNG: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if PlayObject.MagCanHitTarget(PlayObject.m_nCurrX, PlayObject.m_nCurrY, TargeTBaseObject) then
  begin
    if PlayObject.IsProperTarget(TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        if (TargeTBaseObject.m_nAntiMagic <= Random(10)) then
        begin
          with PlayObject do
          begin
            nPower := GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 -
              m_WAbil.MC1, 1));
          end;
          nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
          // 伤害吸收 chongchong 2016-03-18
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(TargeTBaseObject);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nPower := Max(0, nPower - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            {
              // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
            }
          end;
          nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
            GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
          if FNpcReleaseMagic <> 0 then
            PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
            PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), 500); // 大火球技能延时
          if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
            Result := True;
          if (PlayObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
            RC_PLAYMOSTER]) then
            Result := True;
        end
        else
        begin
          if PlayObject.m_btRaceServer = RC_PLAYOBJECT then
          begin
            PlayObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
          end;
          TargeTBaseObject := nil;
        end;
      end
      else
      begin
        TargeTBaseObject := nil;
      end;
    end
    else
      TargeTBaseObject := nil;
  end
  else
    TargeTBaseObject := nil;
end;

{ 治愈术 }
function TMagicManager.MagTreatment(PlayObject: TSmartObject; UserMagic: pTUserMagic; var nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
begin
  Result := False;
  if TargeTBaseObject = nil then
  begin
    TargeTBaseObject := PlayObject;
    nTargetX := PlayObject.m_nCurrX;
    nTargetY := PlayObject.m_nCurrY;
  end;
  if PlayObject.IsProperFriend(TargeTBaseObject) then
  begin
    nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.SC1 * 2,
      (PlayObject.m_WAbil.SC2 - PlayObject.m_WAbil.SC1) * 2 + 1);
    nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
    if TargeTBaseObject.m_WAbil.HP < TargeTBaseObject.m_WAbil.MaxHP then
    begin
      TargeTBaseObject.SendDelayMsg(PlayObject, RM_MAGHEALING, 0, nPower, 0, UserMagic.wMagIdx, '', 800);
      // TargeTBaseObject.SendMsg(PlayObject, RM_MAGHEALING, 0, nPower, 0, 0, '');
      Result := True;
    end;
    if PlayObject.m_boAbilSeeHealGauge then
      PlayObject.SendMsg(TargeTBaseObject, RM_10414, 0, 0, 0, 0, '');
  end;
end;

{ 地域火 }
function TMagicManager.MagMakeHellFire(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  n1C: Integer;
  n14, n18: Integer;
begin
  Result := False;
  n1C := GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, nTargetX, nTargetY);
  if PlayObject.m_PEnvir.GetNextPosition(PlayObject.m_nCurrX, PlayObject.m_nCurrY, n1C, 1, n14, n18) then
  begin
    PlayObject.m_PEnvir.GetNextPosition(PlayObject.m_nCurrX, PlayObject.m_nCurrY, n1C, 5, nTargetX, nTargetY);
    nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
      PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1 + 1);
    nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
    if PlayObject.MagPassThroughMagic(n14, n18, nTargetX, nTargetY, n1C, nPower, UserMagic.wMagIdx, False) > 0 then
      Result := True;
  end;
end;

{ 疾光电影 }
function TMagicManager.MagMakeQuickLighting(PlayObject: TSmartObject; UserMagic: pTUserMagic; var nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  n1C: Integer;
  n14, n18: Integer;
begin
  Result := False;
  n1C := GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, nTargetX, nTargetY);
  if PlayObject.m_PEnvir.GetNextPosition(PlayObject.m_nCurrX, PlayObject.m_nCurrY, n1C, 1, n14, n18) then
  begin
    PlayObject.m_PEnvir.GetNextPosition(PlayObject.m_nCurrX, PlayObject.m_nCurrY, n1C, 8, nTargetX, nTargetY);
    nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
      PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1 + 1);
    nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
    if PlayObject.MagPassThroughMagic(n14, n18, nTargetX, nTargetY, n1C, nPower, UserMagic.wMagIdx, True, 400) > 0 then
      Result := True;
  end;
end;

{ 雷电术 }
function TMagicManager.MagMakeLighting(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower, nPowerNG: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if TargeTBaseObject = nil then
    Exit;
  if PlayObject.IsProperTarget(TargeTBaseObject) then
  begin
    if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then
    begin
      nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
        PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1 + 1);
      if TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD then
        nPower := Min(LongWord(Round(nPower * 1.5)), High(Integer));
      nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
      nPower := Round(nPower / 100 * g_Config.nSkillLighteningPowerRate);
      // 伤害吸收 chongchong 2016-03-18
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
        }
      end;
      nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
        GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
      if FNpcReleaseMagic <> 0 then
        PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
          IntToStr(UserMagic.wMagIdx))
      else
        PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
          IntToStr(UserMagic.wMagIdx), 250); // 雷电术技能延时
      if TargeTBaseObject.m_btRaceServer >= RC_ANIMAL then
        Result := True;
      if (PlayObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer = RC_HEROOBJECT) then
        Result := True;
    end
    else
    begin
      if PlayObject.m_btRaceServer = RC_PLAYOBJECT then
      begin
        PlayObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
      end;
      TargeTBaseObject := nil;
    end
  end
  else
    TargeTBaseObject := nil;
end;

{ 灵魂火符 }
{ TODO -opiaoyun -c技能 : 灵魂火符函数修改--支持裂神符 2013-6-24 }
function TMagicManager.MagMakeFireCharm(PlayObject: TBaseObject;
  { 修改 TBaseObject }
  UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var TargeTBaseObject: TBaseObject; boMove: Boolean): Boolean;
var
  nPower, nPowerNG, nPower2: Integer;
  // 获取周围的怪物对象，随机抽取

  function GetAroundObject(Targe: TBaseObject): TBaseObject;
  var
    BaseObjectList: TList;
    TargeBase: TBaseObject;
    I: Integer;
  begin
    Result := nil;
    BaseObjectList := TList.Create;
    try
      Targe.GetMapBaseObjects(Targe.m_PEnvir, Targe.m_nCurrX, Targe.m_nCurrY, 3, BaseObjectList);
      for I := BaseObjectList.Count - 1 downto 0 do
      begin
        TargeBase := TBaseObject(BaseObjectList[I]);
        if (not PlayObject.IsProperTarget(TargeBase)) or (Targe = TargeBase) or (PlayObject = TargeBase) or (not Targe.MagCanHitTarget
          (Targe.m_nCurrX, Targe.m_nCurrY, TargeBase)) then
        begin
          BaseObjectList.Delete(I);
        end
        else if g_Config.boSkill202RateOnlyMon then
        begin
          if TargeBase.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
          begin
            BaseObjectList.Delete(I)
          end;
        end;
      end;
      if BaseObjectList.Count > 0 then
        Result := TBaseObject(BaseObjectList[Random(BaseObjectList.Count)]);
    finally
      BaseObjectList.Free;
    end;
  end;

var
  I: Integer;
  AroundBase, TargeBase: TBaseObject;
  n: Integer;
  SmartObject: TSmartObject;
  IsCanHitTarget: Boolean;
  Dis1, Dis2: Integer;
  I64: Int64;
  S: AnsiString;
begin
  Result := False;
  if TargeTBaseObject = nil then
    Exit;
  // 灵魂火符技能攻击
  IsCanHitTarget := PlayObject.MagCanHitTarget(PlayObject.m_nCurrX, PlayObject.m_nCurrY, TargeTBaseObject);
  if IsCanHitTarget then
  begin
    if PlayObject.IsProperTarget(TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        if Random(10) >= TargeTBaseObject.m_nAntiMagic then
        begin
          nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.SC1,
            PlayObject.m_WAbil.SC2 - PlayObject.m_WAbil.SC1 + 1);
          nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
          { TODO -ochongchong -c新增 : 英雄四级触发 (4级灵魂火符击力加强) 【2013-08-15】 }
          if (PlayObject.m_btRaceServer = RC_HEROOBJECT) and (UserMagic.btLevel = 4) and (THeroObject(PlayObject).m_rLoyalPoint >=
            g_Config.dwHeroGotoLV4 / 100) then
          begin
            // nPower := nPower + g_Config.nHeroPowerLV4;
            nPower := nPower + Round(nPower * (g_Config.dwHeroPowerLV4 / 100));
          end
          else if (PlayObject.m_btRaceServer = RC_PLAYOBJECT) and (UserMagic.btLevel = 4) then
          begin
            nPower := nPower + Round(nPower * (g_Config.dwHumSkill13PowerLV4 / 100));
          end;
          // 伤害吸收 chongchong 2016-03-18
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(TargeTBaseObject);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nPower := Max(0, nPower - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            {
              // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
            }
          end;
          if not boMove then
            nPower := Round(nPower / 100 * g_Config.nSkillFireCharmPowerRate)
          else
            nPower := Round(nPower / 100 * g_Config.nSkill202PowerRate);
          nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
            GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
          Dis1 := abs(TargeTBaseObject.m_nCurrX - PlayObject.m_nCurrX);
          Dis2 := abs(TargeTBaseObject.m_nCurrY - PlayObject.m_nCurrY);
          if Dis1 < Dis2 then
            Dis1 := Dis2;
          // MainOutMessage('间隔01 (MagMakeFireCharm):' + IntToStr(MyGetTickCount - g_Debug_Spell_Tick));
          // g_Debug_Spell_Tick := MyGetTickCount;
          if FNpcReleaseMagic <> 0 then
            PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
            PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), 100 + Dis1 * 80); // 灵魂火符技能延时
          if (PlayObject.m_btRaceServer = RC_PLAYOBJECT) and (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
            Result := True;
          if (PlayObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
            RC_PLAYMOSTER]) then
            Result := True;
          // TargeTBaseObject.MagicQuest(PlayObject, UserMagic.wMagIdx, mfs_TagEx);
          // 是否裂神符
          Randomize;
          if boMove and (Random(g_Config.nSkill202Rate) = 0) then
          begin
            n := UserMagic.btLevel * g_Config.nSkill202LevelupCount + g_Config.nSkill202BaseCount;
            begin
              TargeBase := TargeTBaseObject;
              // for I := 2 to (_MIN(UserMagic.btLevel, 3) + 1) do
              for I := 0 to n - 1 do
              begin
                AroundBase := GetAroundObject(TargeBase);
                if AroundBase = nil then
                  break;
                nPower2 := Round(nPower / 100 * Max(g_Config.nSkill202PowerMin, 100 - g_Config.nSkill202PowerDec * (I + 1)));
                if FNpcReleaseMagic <> 0 then
                  PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(AroundBase.m_nCurrX, AroundBase.m_nCurrY), 2,
                    NativeInt(AroundBase), IntToStr(UserMagic.wMagIdx))
                else
                  PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPower2, MakeLong(AroundBase.m_nCurrX, AroundBase.m_nCurrY),
                    2, NativeInt(AroundBase), IntToStr(UserMagic.wMagIdx), 300 + 100 * (I + 2));
                I64 := NativeInt(TargeBase);
                SetLength(S, SizeOf(I64));
                Move(I64, S[1], SizeOf(I64));
                AroundBase.SendRefMsg(RM_10205, 0, 0, 0, 30, S, 400 + 100 * (I + 2));
                TargeBase := AroundBase;
              end;
            end;
          end;
        end
        else
        begin
          if PlayObject.m_btRaceServer = RC_PLAYOBJECT then
          begin
            PlayObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
          end;
        end;
      end;
    end;
  end
  else
    TargeTBaseObject := nil;
end;

// 灭天火技能函数修改 piaoyun 2013-07-25
function TMagicManager.MagMakeFireDay(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower, nPowerNG: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if PlayObject.IsProperTarget(TargeTBaseObject) then
  begin
    if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then
    begin
      nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
        PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1 + 1);
      if TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD then
        nPower := Min(LongWord(Round(nPower * 1.5)), High(Integer));
      nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
      { TODO -ochongchong -c新增 : 英雄四级触发 (4级灭天火击力加强) 【2013-08-15】 }
      if (PlayObject.m_btRaceServer = RC_HEROOBJECT) and (UserMagic.btLevel = 4) and (THeroObject(PlayObject).m_rLoyalPoint >=
        g_Config.dwHeroGotoLV4 / 100) then
      begin
        // nPower := nPower + g_Config.nHeroPowerLV4;
        nPower := nPower + Round(nPower * (g_Config.dwHeroPowerLV4 / 100));
      end
      else if (PlayObject.m_btRaceServer = RC_PLAYOBJECT) and (UserMagic.btLevel = 4) then
      begin
        nPower := nPower + Round(nPower * (g_Config.dwHumSkill45PowerLV4 / 100));
      end;
      // 伤害吸收 chongchong 2016-03-18
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
        }
      end;
      nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
        GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
      // 灭天火技能威力修改 piaoyun 2013-07-25
      nPowerNG := Round(nPowerNG * (g_Config.nMakeFireDayPowerRate / 100));
      if FNpcReleaseMagic <> 0 then
        PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
          IntToStr(UserMagic.wMagIdx))
      else
        PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPowerNG, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
          IntToStr(UserMagic.wMagIdx), 300);
      if g_Config.boPlayObjectReduceMP then
        TargeTBaseObject.DamageSpell(nPowerNG);
      if TargeTBaseObject.m_btRaceServer >= RC_ANIMAL then
        Result := True;
      if (PlayObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
        RC_PLAYMOSTER]) then
        Result := True;
    end
    else
    begin
      if PlayObject.m_btRaceServer = RC_PLAYOBJECT then
      begin
        PlayObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
      end;
      TargeTBaseObject := nil;
    end
  end
  else
    TargeTBaseObject := nil;
end;

{ 解毒术 }
function TMagicManager.MagMakeUnTreatment(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
begin
  Result := False;
  if TargeTBaseObject = nil then
  begin
    TargeTBaseObject := PlayObject;
  end;
  if PlayObject.IsProperFriend { 0FFF3 } (TargeTBaseObject) then
  begin
    if Random(7) - (UserMagic.btLevel + 1) < 0 then
    begin
      if TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <> 0 then
      begin
        TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] := 1;
        Result := True;
      end;
      if TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <> 0 then
      begin
        TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] := 1;
        Result := True;
      end;
      if TargeTBaseObject.m_wStatusTimeArr[POISON_STONE] <> 0 then
      begin
        TargeTBaseObject.m_wStatusTimeArr[POISON_STONE] := 1;
        Result := True;
      end;
    end;
  end;
end;

// 自定义技能 chongchong 2015-03-20
procedure Add_AdditionalsDamage(AttackConfig: PMagicServerConfig; Magic: pTUserMagic; AttackTarget: TBaseObject; Player:
  TSmartObject);
var
  V1, V2: Integer;
  boRecalcAbilitys: Boolean;
  S: string;
  SmartObject: TSmartObject;
  ElementsType: TItemElementsType;
  NexExtType: TAddNewExtType;
begin
  if AttackTarget = nil then
    Exit;
  // 目标不防毒
  if (not AttackTarget.UnPosion) then
  begin
    // 中绿毒
    if (AttackConfig.Additionals[0].Checked) and (Random(100) < AttackConfig.Additionals[0].Rate) and (AttackConfig.Additionals[0].Time
      > 0) then
    begin
      AttackTarget.MakePosion(POISON_DECHEALTH, AttackConfig.Additionals[0].Time, AttackConfig.AdditionalHP0);
      AttackTarget.SetLastHiter(Player);
      AttackTarget.m_PoisonHitter := Player;
    end;
    // 中红毒
    if (AttackConfig.Additionals[1].Checked) and (Random(100) < AttackConfig.Additionals[1].Rate) and (AttackConfig.Additionals[1].Time
      > 0) then
    begin
      AttackTarget.MakePosion(POISON_DAMAGEARMOR, AttackConfig.Additionals[1].Time, 0);
      AttackTarget.m_PoisonHitter := Player;
    end;
  end;
  // 麻痹
  if (not AttackTarget.UnParalysis) and (AttackConfig.Additionals[2].Checked) and (Random(100) < AttackConfig.Additionals[2].Rate)
    and (AttackConfig.Additionals[2].Time > 0) then
  begin
    AttackTarget.MakePosion(POISON_STONE, AttackConfig.Additionals[2].Time, 0);
  end;
  // 冰冻
  if (not AttackTarget.UnFrozen) and (AttackConfig.Additionals[3].Checked) and (Random(100) < AttackConfig.Additionals[3].Rate)
    and (AttackConfig.Additionals[3].Time > 0) then
  begin
    AttackTarget.MakeFrozen(AttackConfig.Additionals[3].Time);
  end;
  // 冰封
  if ((not AttackTarget.m_boUnForeverFrozen) or (Random(100) >= AttackTarget.m_nUnForeverFrozenRate)) and (AttackConfig.Additionals
    [10].Checked) and (Random(100) < AttackConfig.Additionals[10].Rate) and (AttackConfig.Additionals[10].Time > 0) then
  begin
    AttackTarget.OpenForeverFrozen(AttackConfig.Additionals[10].Time);
  end;
  // 蛛网
  if (not AttackTarget.UnCobwebWinding) and (AttackConfig.Additionals[7].Checked) and (Random(100) < AttackConfig.Additionals[7].Rate)
    and (AttackConfig.Additionals[7].Time > 0) then
  begin
    AttackTarget.OpenCobwebWinding(AttackConfig.Additionals[7].Time);
  end;
  boRecalcAbilitys := False;
  // 0防御
  if (AttackConfig.Additionals[8].Checked) and (Random(100) < AttackConfig.Additionals[8].Rate) and (AttackConfig.Additionals[8].Time
    > 0) then
  begin
    // AttackTarget.ZeroArmor(AttackConfig.Additionals[8].Time);
    boRecalcAbilitys := True;
    AttackTarget.OpenZeroAC(AttackConfig.Additionals[8].Time, False);
  end;
  // 0魔御
  if (AttackConfig.Additionals[9].Checked) and (Random(100) < AttackConfig.Additionals[9].Rate) and (AttackConfig.Additionals[9].Time
    > 0) then
  begin
    boRecalcAbilitys := True;
    AttackTarget.OpenZeroMAC(AttackConfig.Additionals[9].Time, False);
  end;
  // 减防御 chongchong 2015-04-06
  if AttackConfig.AttackSubAttrib[daAC].IsChecked and (AttackConfig.AttackSubAttrib[daAC].Rate > 0) and (Random(100) <
    AttackConfig.AttackSubAttrib[daAC].Rate) and ((AttackConfig.AttackSubAttrib[daAC].LowValue > 0) or (AttackConfig.AttackSubAttrib
    [daAC].HighValue > 0)) and (AttackConfig.AttackSubAttrib[daAC].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubAC1] = 0)
    and (AttackTarget.m_AddNewExtValues[anet_SubAC2] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daAC].LowValueIsPoint then
      V1 := Round(AttackTarget.m_WAbil.AC1 / 100 * AttackConfig.AttackSubAttrib[daAC].LowValue)
    else
      V1 := AttackConfig.AttackSubAttrib[daAC].LowValue;
    if not AttackConfig.AttackSubAttrib[daAC].HighValueIsPoint then
      V2 := Round(AttackTarget.m_WAbil.AC2 / 100 * AttackConfig.AttackSubAttrib[daAC].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daAC].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubAC1] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daAC].Time) * 1000;
    AttackTarget.m_AddNewExtTicks[anet_SubAC2] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daAC].Time) * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubAC1] := V1;
    AttackTarget.m_AddNewExtValues[anet_SubAC2] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daAC].ShowHint and (Length
      (AttackConfig.AttackSubAttrib[daAC].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daAC].HintText, '%AC1', IntToStr(V1), [rfIgnoreCase]);
      S := StringReplace(S, '%AC2', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daAC].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daMAC].IsChecked and (AttackConfig.AttackSubAttrib[daMAC].Rate > 0) and (Random(100) <
    AttackConfig.AttackSubAttrib[daMAC].Rate) and ((AttackConfig.AttackSubAttrib[daMAC].LowValue > 0) or (AttackConfig.AttackSubAttrib
    [daMAC].HighValue > 0)) and (AttackConfig.AttackSubAttrib[daMAC].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubMAC1] =
    0) and (AttackTarget.m_AddNewExtValues[anet_SubMAC2] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daMAC].LowValueIsPoint then
      V1 := Round(AttackTarget.m_WAbil.MAC1 / 100 * AttackConfig.AttackSubAttrib[daMAC].LowValue)
    else
      V1 := AttackConfig.AttackSubAttrib[daMAC].LowValue;
    if not AttackConfig.AttackSubAttrib[daMAC].HighValueIsPoint then
      V2 := Round(AttackTarget.m_WAbil.MAC2 / 100 * AttackConfig.AttackSubAttrib[daMAC].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daMAC].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubMAC1] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daMAC].Time) * 1000;
    AttackTarget.m_AddNewExtTicks[anet_SubMAC2] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daMAC].Time) * 1000;
    if (AttackTarget.m_AddNewExtValues[anet_SubMAC1] <> V1) or (AttackTarget.m_AddNewExtValues[anet_SubMAC2] <> V2) then
    begin
      AttackTarget.m_AddNewExtValues[anet_SubMAC1] := V1;
      AttackTarget.m_AddNewExtValues[anet_SubMAC2] := V2;
      boRecalcAbilitys := True;
      if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daMAC].ShowHint and (Length
        (AttackConfig.AttackSubAttrib[daMAC].HintText) > 0) then
      begin
        S := StringReplace(AttackConfig.AttackSubAttrib[daMAC].HintText, '%MAC1', IntToStr(V1), [rfIgnoreCase]);
        S := StringReplace(S, '%MAC2', IntToStr(V2), [rfIgnoreCase]);
        S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daMAC].Time), [rfIgnoreCase]);
        TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
      end;
    end;
  end;
  if AttackConfig.AttackSubAttrib[daDC].IsChecked and (AttackConfig.AttackSubAttrib[daDC].Rate > 0) and (Random(100) <
    AttackConfig.AttackSubAttrib[daDC].Rate) and ((AttackConfig.AttackSubAttrib[daDC].LowValue > 0) or (AttackConfig.AttackSubAttrib
    [daDC].HighValue > 0)) and (AttackConfig.AttackSubAttrib[daDC].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubDC1] = 0)
    and (AttackTarget.m_AddNewExtValues[anet_SubDC2] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daDC].LowValueIsPoint then
      V1 := Round(AttackTarget.m_WAbil.DC1 / 100 * AttackConfig.AttackSubAttrib[daDC].LowValue)
    else
      V1 := AttackConfig.AttackSubAttrib[daDC].LowValue;
    if not AttackConfig.AttackSubAttrib[daDC].HighValueIsPoint then
      V2 := Round(AttackTarget.m_WAbil.DC2 / 100 * AttackConfig.AttackSubAttrib[daDC].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daDC].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubDC1] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daDC].Time) * 1000;
    AttackTarget.m_AddNewExtTicks[anet_SubDC2] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daDC].Time) * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubDC1] := V1;
    AttackTarget.m_AddNewExtValues[anet_SubDC2] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daDC].ShowHint and (Length
      (AttackConfig.AttackSubAttrib[daDC].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daDC].HintText, '%DC1', IntToStr(V1), [rfIgnoreCase]);
      S := StringReplace(S, '%DC2', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daDC].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daMC].IsChecked and (AttackConfig.AttackSubAttrib[daMC].Rate > 0) and (Random(100) <
    AttackConfig.AttackSubAttrib[daMC].Rate) and ((AttackConfig.AttackSubAttrib[daMC].LowValue > 0) or (AttackConfig.AttackSubAttrib
    [daMC].HighValue > 0)) and (AttackConfig.AttackSubAttrib[daMC].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubMC1] = 0)
    and (AttackTarget.m_AddNewExtValues[anet_SubMC2] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daMC].LowValueIsPoint then
      V1 := Round(AttackTarget.m_WAbil.MC1 / 100 * AttackConfig.AttackSubAttrib[daMC].LowValue)
    else
      V1 := AttackConfig.AttackSubAttrib[daMC].LowValue;
    if not AttackConfig.AttackSubAttrib[daMC].HighValueIsPoint then
      V2 := Round(AttackTarget.m_WAbil.MC2 / 100 * AttackConfig.AttackSubAttrib[daMC].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daMC].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubMC1] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daMC].Time) * 1000;
    AttackTarget.m_AddNewExtTicks[anet_SubMC2] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daMC].Time) * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubMC1] := V1;
    AttackTarget.m_AddNewExtValues[anet_SubMC2] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daMC].ShowHint and (Length
      (AttackConfig.AttackSubAttrib[daMC].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daMC].HintText, '%MC1', IntToStr(V1), [rfIgnoreCase]);
      S := StringReplace(S, '%MC2', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daMC].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daSC].IsChecked and (AttackConfig.AttackSubAttrib[daSC].Rate > 0) and (Random(100) <
    AttackConfig.AttackSubAttrib[daSC].Rate) and ((AttackConfig.AttackSubAttrib[daSC].LowValue > 0) or (AttackConfig.AttackSubAttrib
    [daSC].HighValue > 0)) and (AttackConfig.AttackSubAttrib[daSC].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubSC1] = 0)
    and (AttackTarget.m_AddNewExtValues[anet_SubSC2] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daSC].LowValueIsPoint then
      V1 := Round(AttackTarget.m_WAbil.SC1 / 100 * AttackConfig.AttackSubAttrib[daSC].LowValue)
    else
      V1 := AttackConfig.AttackSubAttrib[daSC].LowValue;
    if not AttackConfig.AttackSubAttrib[daSC].HighValueIsPoint then
      V2 := Round(AttackTarget.m_WAbil.SC2 / 100 * AttackConfig.AttackSubAttrib[daSC].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daSC].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubSC1] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daSC].Time) * 1000;
    AttackTarget.m_AddNewExtTicks[anet_SubSC2] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daSC].Time) * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubSC1] := V1;
    AttackTarget.m_AddNewExtValues[anet_SubSC2] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daSC].ShowHint and (Length
      (AttackConfig.AttackSubAttrib[daSC].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daSC].HintText, '%SC1', IntToStr(V1), [rfIgnoreCase]);
      S := StringReplace(S, '%SC2', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daSC].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daHitPoint].IsChecked and (AttackConfig.AttackSubAttrib[daHitPoint].Rate > 0) and (Random(100) <
    AttackConfig.AttackSubAttrib[daHitPoint].Rate) and (AttackConfig.AttackSubAttrib[daHitPoint].HighValue > 0) and (AttackConfig.AttackSubAttrib
    [daHitPoint].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubHitPoint] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daHitPoint].HighValueIsPoint then
      V2 := Round(AttackTarget.m_btHitPoint / 100 * AttackConfig.AttackSubAttrib[daHitPoint].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daHitPoint].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubHitPoint] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daHitPoint].Time) *
      1000;
    AttackTarget.m_AddNewExtValues[anet_SubHitPoint] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daHitPoint].ShowHint and (Length
      (AttackConfig.AttackSubAttrib[daHitPoint].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daHitPoint].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daHitPoint].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daSpeedPoint].IsChecked and (AttackConfig.AttackSubAttrib[daSpeedPoint].Rate > 0) and (Random(100)
    < AttackConfig.AttackSubAttrib[daSpeedPoint].Rate) and (AttackConfig.AttackSubAttrib[daSpeedPoint].HighValue > 0) and (AttackConfig.AttackSubAttrib
    [daSpeedPoint].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubSpeedPoint] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daSpeedPoint].HighValueIsPoint then
      V2 := Round(AttackTarget.m_btSpeedPoint / 100 * AttackConfig.AttackSubAttrib[daSpeedPoint].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daSpeedPoint].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubSpeedPoint] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daSpeedPoint].Time)
      * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubSpeedPoint] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daSpeedPoint].ShowHint and
      (Length(AttackConfig.AttackSubAttrib[daSpeedPoint].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daSpeedPoint].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daSpeedPoint].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daAntiMagic].IsChecked and (AttackConfig.AttackSubAttrib[daAntiMagic].Rate > 0) and (Random(100)
    < AttackConfig.AttackSubAttrib[daAntiMagic].Rate) and (AttackConfig.AttackSubAttrib[daAntiMagic].HighValue > 0) and (AttackConfig.AttackSubAttrib
    [daAntiMagic].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubAntiMagic] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daAntiMagic].HighValueIsPoint then
      V2 := Round(AttackTarget.m_nAntiMagic / 100 * AttackConfig.AttackSubAttrib[daAntiMagic].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daAntiMagic].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubAntiMagic] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daAntiMagic].Time)
      * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubAntiMagic] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daAntiMagic].ShowHint and
      (Length(AttackConfig.AttackSubAttrib[daAntiMagic].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daAntiMagic].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daAntiMagic].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if AttackConfig.AttackSubAttrib[daAntiPoison].IsChecked and (AttackConfig.AttackSubAttrib[daAntiPoison].Rate > 0) and (Random(100)
    < AttackConfig.AttackSubAttrib[daAntiPoison].Rate) and (AttackConfig.AttackSubAttrib[daAntiPoison].HighValue > 0) and (AttackConfig.AttackSubAttrib
    [daAntiPoison].Time > 0) and (AttackTarget.m_AddNewExtValues[anet_SubAntiPoison] = 0) then
  begin
    if not AttackConfig.AttackSubAttrib[daAntiPoison].HighValueIsPoint then
      V2 := Round(AttackTarget.m_btAntiPoison / 100 * AttackConfig.AttackSubAttrib[daAntiPoison].HighValue)
    else
      V2 := AttackConfig.AttackSubAttrib[daAntiPoison].HighValue;
    AttackTarget.m_AddNewExtTicks[anet_SubAntiPoison] := MyGetTickCount + LongWord(AttackConfig.AttackSubAttrib[daAntiPoison].Time)
      * 1000;
    AttackTarget.m_AddNewExtValues[anet_SubAntiPoison] := V2;
    boRecalcAbilitys := True;
    if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and AttackConfig.AttackSubAttrib[daAntiPoison].ShowHint and
      (Length(AttackConfig.AttackSubAttrib[daAntiPoison].HintText) > 0) then
    begin
      S := StringReplace(AttackConfig.AttackSubAttrib[daAntiPoison].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
      S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubAttrib[daAntiPoison].Time), [rfIgnoreCase]);
      TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
    end;
  end;
  if (AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
  begin
    SmartObject := TSmartObject(AttackTarget);
    for ElementsType := Low(TItemElementsType) to High(TItemElementsType) do
    begin
      NexExtType := TAddNewExtType(Integer(anet_SubBlastHit) + Integer(ElementsType));
      if AttackConfig.AttackSubElements[ElementsType].IsChecked and (AttackConfig.AttackSubElements[ElementsType].Rate > 0) and (Random
        (100) < AttackConfig.AttackSubElements[ElementsType].Rate) and (AttackConfig.AttackSubElements[ElementsType].Value > 0)
        and (AttackConfig.AttackSubElements[ElementsType].Time > 0) and (AttackTarget.m_AddNewExtValues[NexExtType] = 0) then
      begin
        if not AttackConfig.AttackSubElements[ElementsType].ValueIsPoint then
        begin
          V1 := 0;
          case NexExtType of
            anet_SubBlastHit:
              V1 := SmartObject.m_WAbil.NewValue[0]; // 爆击几率
            anet_SubDamageAdd:
              V1 := SmartObject.m_WAbil.NewValue[1]; // 伤害增加
            anet_SubDamageDec:
              V1 := SmartObject.m_WAbil.NewValue[2]; // 伤害减少
            anet_SubSpellDamageDec:
              V1 := SmartObject.m_WAbil.NewValue[3]; // 魔法防御
            anet_SubCloseDefense:
              V1 := SmartObject.m_WAbil.NewValue[4]; // 忽视防御
            anet_SubDamageRebound:
              V1 := SmartObject.m_WAbil.NewValue[5]; // 伤害反弹
            anet_SubMonDropRate:
              V1 := SmartObject.m_WAbil.NewValue[6]; // 怪物爆率
            anet_SubMaxHPAdd:
              V1 := SmartObject.m_WAbil.NewValue[7]; // 体力增加
            anet_SubMaxMPAdd:
              V1 := SmartObject.m_WAbil.NewValue[8]; // 魔力增加
            anet_SubAngryValueTimAdd:
              V1 := SmartObject.m_WAbil.NewValue[9]; // 怒气恢复
            anet_SubGroupDamageAdd:
              V1 := SmartObject.m_WAbil.NewValue[10]; // 合击攻击
            anet_SubHuamDropRate:
              V1 := SmartObject.m_WAbil.NewValue[11]; // 人物爆率
            anet_SubUndropRate:
              V1 := SmartObject.m_WAbil.NewValue[12]; // 防爆出率
            anet_SubUnParalysis:
              V1 := SmartObject.m_WAbil.NewValue[13]; // 防止麻痹
            anet_SubUnMagicShield:
              V1 := SmartObject.m_WAbil.NewValue[14]; // 防止护身
            anet_SubUnRevival:
              V1 := SmartObject.m_WAbil.NewValue[15]; // 防止复活
            anet_SubUnPosion:
              V1 := SmartObject.m_WAbil.NewValue[16]; // 防止全毒
            anet_SubUnTamming:
              V1 := SmartObject.m_WAbil.NewValue[17]; // 防止诱惑
            anet_SubUnFireCross:
              V1 := SmartObject.m_WAbil.NewValue[18]; // 防止火墙
            anet_SubUnFrozen:
              V1 := SmartObject.m_WAbil.NewValue[19]; // 防止冰冻
            anet_SubUnCobwebWinding:
              V1 := SmartObject.m_WAbil.NewValue[20]; // 防止蛛网
            anet_SubFatalBlowRate:
              V1 := SmartObject.m_WAbil.NewValue[21]; // 致命一击几率
            anet_SubFatalBlowPower:
              V1 := SmartObject.m_WAbil.NewValue[22]; // 致命一击攻击
            anet_SubFatalBlowDefense:
              V1 := SmartObject.m_WAbil.NewValue[23]; // 致命一击防御
            anet_SubUnBlastHit:
              V1 := SmartObject.m_WAbil.NewValue[24]; // 暴击抗性
          end;
          V1 := Round(V1 / 100 * AttackConfig.AttackSubElements[ElementsType].Value);
        end
        else
        begin
          V1 := AttackConfig.AttackSubElements[ElementsType].Value;
        end;
        AttackTarget.m_AddNewExtTicks[NexExtType] := MyGetTickCount + LongWord(AttackConfig.AttackSubElements[ElementsType].Time)
          * 1000;
        AttackTarget.m_AddNewExtValues[NexExtType] := V1;
        boRecalcAbilitys := True;
        if AttackConfig.AttackSubElements[ElementsType].ShowHint and (Length(AttackConfig.AttackSubElements[ElementsType].HintText)
          > 0) then
        begin
          S := StringReplace(AttackConfig.AttackSubElements[ElementsType].HintText, '%Point', IntToStr(V1), [rfIgnoreCase]);
          S := StringReplace(S, '%Time', IntToStr(AttackConfig.AttackSubElements[ElementsType].Time), [rfIgnoreCase]);
          TSmartObject(AttackTarget).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
        end;
      end;
    end;
  end;
  // 自定义状态 chongchong 2014-09-07
  if AttackConfig.AttackTargetStatus and (AttackConfig.AttackTargetStatusTime > 0) then
  // (not MonObj.m_boCustomMagicStatusArr[UserMagic.wMagIdx - CUSTOM_MAGIC_START_ID]) then
  begin
    AttackTarget.SetCustomMagicStatus(Magic.btNewLevel, Magic.wMagIdx, AttackConfig.AttackTargetStatusTime);
    AttackTarget.SendRefMsg(RM_EFFECTSTEP, 9991, Magic.wMagIdx, Magic.btNewLevel, 0, '', AttackConfig.AttackTargetStatusDelay);
  end;
  if boRecalcAbilitys then
  begin
    AttackTarget.RecalcAbilitys;
    if AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    begin
      AttackTarget.SendMsg(AttackTarget, RM_ABILITY, 0, 0, 0, 0, '');
      AttackTarget.SendMsg(AttackTarget, RM_SUBABILITY, 0, 0, 0, 0, '');
    end;
  end;
end;

function SingleAttack(Player: TSmartObject; m_Target, AttackTarget: TBaseObject; AttackConfig: PMagicServerConfig; BasePower:
  Integer; TargetList: TList; Magic: pTUserMagic; boMagicWarr: Boolean): Boolean;
{
  const
  BASE_DELAY: LongWord = 300;
}
var
  nDamage: Integer;
  btGetBackHP, btGetBackMP: Integer;
  Int64Value: Int64;
  nDelayTime: Integer;
  nSuckDamagePoint: Integer;
  BlastHitType: TBlastHitType;
  MagicACInfo: TMagicACInfo;
begin
  Result := False;
  if AttackTarget = nil then
    Exit;

  // 物理攻击
  if boMagicWarr then
  begin
    if AttackConfig.EnableHitPoint and (Random(AttackTarget.m_btSpeedPoint) >= Player.m_btHitPoint) then
    begin
      if (Player.m_btRaceServer in [RC_PLAYOBJECT]) then
        Player.SendMsg(AttackTarget, RM_ATTACK_MISS, 0, 0, 0, 0, '');

      Exit;
    end;
  end
  else
  begin
    if AttackConfig.EnableAntiMagic and (Random(10) < AttackTarget.m_nAntiMagic) then
    begin
      if (Player.m_btRaceServer in [RC_PLAYOBJECT]) then
        Player.SendMsg(AttackTarget, RM_ATTACK_MISS, 0, 0, 0, 0, '');

      Exit;
    end;
  end;

  // 隐身的怪物，人物也可以攻击的 chongchong 2015-12-03
  if (not AttackTarget.m_boDeath) //
    and (not AttackTarget.m_boGhost) //
  { and (not AttackTarget.m_boHideMode or Player.m_boCoolEye) }
    and Player.IsProperTarget(AttackTarget) then
  begin
    Player.SetTargetCreat(AttackTarget);
    nDamage := BasePower;

    // 元素属性 - 物理伤害减少  魔法伤害减少
    FillChar(MagicACInfo, SizeOf(MagicACInfo), 0);
    MagicACInfo.wMagicId := Magic.wMagIdx;
    MagicACInfo.boEnabled := AttackConfig.AttackBreakDefense[bdtHumDefense].IsChecked or AttackConfig.AttackBreakDefense[bdtMonDefense].IsChecked
      or AttackConfig.AttackBreakDefense[bdtHeroDefense].IsChecked or AttackConfig.AttackBreakDefense[bdtHumMagDefense].IsChecked
      or AttackConfig.AttackBreakDefense[bdtMonMagDefense].IsChecked or AttackConfig.AttackBreakDefense[bdtHeroMagDefense].IsChecked;

    if Random(100) < AttackConfig.AttackBreakDefense[bdtHumDefense].Rate then
      MagicACInfo.btHum := AttackConfig.AttackBreakDefense[bdtHumDefense].Value;

    if Random(100) < AttackConfig.AttackBreakDefense[bdtMonDefense].Rate then
      MagicACInfo.btMon := AttackConfig.AttackBreakDefense[bdtMonDefense].Value;

    if Random(100) < AttackConfig.AttackBreakDefense[bdtHeroDefense].Rate then
      MagicACInfo.btHero := AttackConfig.AttackBreakDefense[bdtHeroDefense].Value;

    if Random(100) < AttackConfig.AttackBreakDefense[bdtHumMagDefense].Rate then
      MagicACInfo.btDefenceHum := AttackConfig.AttackBreakDefense[bdtHumMagDefense].Value;

    if Random(100) < AttackConfig.AttackBreakDefense[bdtMonMagDefense].Rate then
      MagicACInfo.btDefenceMon := AttackConfig.AttackBreakDefense[bdtMonMagDefense].Value;

    if Random(100) < AttackConfig.AttackBreakDefense[bdtHeroMagDefense].Rate then
      MagicACInfo.btDefenceHero := AttackConfig.AttackBreakDefense[bdtHeroMagDefense].Value;

    // 忽视魔法盾 piaoyun 2013-08-18
    nDamage := AttackTarget.GetHitStruckDamage(Player, nDamage, @MagicACInfo, 2);
    if boMagicWarr then
    begin
      /// ////////////////忽视物理防御 piaoyun 2013-08-18////////////////
      if (not Player.CanCloseDefense) then
      begin
        nDamage := AttackTarget.GetHitStruckDamage(Player, nDamage, @MagicACInfo, 1);
        nDamage := AttackTarget.GetHitStruckDamage(Player, nDamage, nil, 3);
      end;
      nDamage := AttackTarget.NewAbilPower(2, nDamage); // 物理伤害减少
    end
    else
    begin
      if (not Player.CanCloseDefense) then
        nDamage := AttackTarget.GetMagStruckDamage(Player, nDamage, @MagicACInfo); // 忽视目标魔法防御

      nDamage := AttackTarget.NewAbilPower(3, nDamage); // 魔法伤害减少
    end;

    // 元素增加攻击伤害	移到这里 2020-04-28
    nDamage := Player.NewAbilPower(1, nDamage);

    // 技能有一定的几率暴击 chongchong 2015-09-11
    nDamage := Player.BlastHit(nDamage, AttackTarget, BlastHitType, 500); // 暴击

    // SetSuckDamage 伤害吸收
    if AttackTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      if (nDamage > 0) //
        and (TSmartObject(AttackTarget).m_nSuckDamagePoint > 0) //
        and (TSmartObject(AttackTarget).m_nSuckDamageRate > 0) //
        and (Random(100) < TSmartObject(AttackTarget).m_nSuckDamageProbability) then
      begin
        nSuckDamagePoint := Round(TSmartObject(AttackTarget).m_nSuckDamageRate / 1000 * nDamage);
        if nSuckDamagePoint > TSmartObject(AttackTarget).m_nSuckDamagePoint then
          nSuckDamagePoint := TSmartObject(AttackTarget).m_nSuckDamagePoint;

        Dec(TSmartObject(AttackTarget).m_nSuckDamagePoint, nSuckDamagePoint);
        nDamage := Max(nDamage - nSuckDamagePoint, 0);
      end;
    end;

    nDamage := Player.GetMagicPercentPower(Magic.wMagIdx, nDamage, AttackTarget); // 增加技能攻击百分百
    nDamage := Player.GetNextDamage(nDamage);
    nDamage := Player.GetPowerRateAdd(AttackTarget, nDamage); // 2020-09-12 23:24:45

    // 不死系怪物伤害加成
    if (AttackTarget.m_btLifeAttrib = LA_UNDEAD) and (AttackConfig.AttackPowerUndeadAdd <> 0) then
    begin
      Int64Value := nDamage + Round(nDamage / 100 * AttackConfig.AttackPowerUndeadAdd);
      nDamage := Min(Int64Value, High(Integer));
      if nDamage < 0 then
        nDamage := 0;
    end;

    // 怪物伤害封顶 chongchong 2016-01-30
    nDamage := AttackTarget.GetAttackPowerMax(nDamage);
    if nDamage > 0 then
    begin
      AttackTarget.SetLastHiter(Player);

      // 吸血
      if (AttackConfig.Additionals[5].Checked) //
        and (Random(100) < AttackConfig.Additionals[5].Rate) //
        and (AttackConfig.Additionals[5].Time > 0) then
      begin
        btGetBackHP := Round(nDamage / 100 * AttackConfig.Additionals[5].Time);
        if btGetBackHP > 0 then
        begin
          Int64Value := Int64(Player.m_WAbil.HP) + btGetBackHP;
          if Int64Value <= Player.m_WAbil.MaxHP then
            Player.m_WAbil.HP := Int64Value
          else
            Player.m_WAbil.HP := Player.m_WAbil.MaxHP;
          Result := True;
        end;
      end;

      // 吸蓝
      if (AttackConfig.Additionals[6].Checked) //
        and (Random(100) < AttackConfig.Additionals[6].Rate) //
        and (AttackConfig.Additionals[6].Time > 0) then
      begin
        btGetBackMP := Round(nDamage / 100 * AttackConfig.Additionals[6].Time);
        if AttackTarget.m_WAbil.MP < btGetBackMP then
          btGetBackMP := AttackTarget.m_WAbil.MP;

        if btGetBackMP > 0 then
        begin
          Int64Value := Player.m_WAbil.MP + btGetBackMP;
          if Int64Value <= Player.m_WAbil.MaxMP then
            Player.m_WAbil.MP := Int64Value
          else
            Player.m_WAbil.MP := Player.m_WAbil.MaxMP;

          AttackTarget.m_WAbil.MP := Max(AttackTarget.m_WAbil.MP - btGetBackMP, 0);
          AttackTarget.HealthSpellChanged(500 { BASE_DELAY } );
          Result := True;
        end;
      end;

      // 技能延时5秒，但对于血小的怪物来说，这里直接减血把怪物搞死了 2020-10-24 02:48:24
      // nDamage := AttackTarget.StruckDamage(nDamage, Player);
      {
        if boMagicWarr then
        nDelayTime := BASE_DELAY + 200
        else
        nDelayTime := BASE_DELAY + 1200;
      }
      nDelayTime := AttackConfig.AttackDelayTime;
      if (nDamage > 0) and (BlastHitType = bhtBlastHit) and (AttackTarget <> m_Target) then
      begin
        AttackTarget.SendRefMsg(RM_SENDBLASTHIT, AttackTarget.m_btDirection, AttackTarget.m_nCurrX, AttackTarget.m_nCurrY, nDamage,
          '', nDelayTime - 200 { 500 } );
      end;

      if (AttackTarget.m_btRaceServer in [55]) then
      begin
        if nDelayTime = 0 then
        begin
          AttackTarget.SendMsg(AttackTarget, RM_STRUCK, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP, NativeInt(Player),
            BlastHitTypeValues[BlastHitType]);
        end
        else
        begin
          AttackTarget.SendDelayMsg(AttackTarget, RM_STRUCK, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
            NativeInt(Player), BlastHitTypeValues[BlastHitType], nDelayTime);
        end;
      end
      else
      begin
        if MagicManager.FNpcReleaseMagic <> 0 then
        begin
          if AttackConfig.AttackPowerCalc = mapcDC then
          begin
            AttackTarget.SendMsg(TBaseObject(RM_STRUCK), RM_10101_EX_2, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
              NativeInt(Player), IntToStr(MakeLong(Magic.wMagIdx, Integer(BlastHitType))));
          end
          else
          begin
            AttackTarget.SendMsg(TBaseObject(RM_STRUCK), RM_10101_EX_2, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
              NativeInt(Player), 'MAG:' + IntToStr(MakeLong(Magic.wMagIdx, Integer(BlastHitType))));
          end;
        end
        else
        begin
          if AttackConfig.AttackPowerCalc = mapcDC then
          begin
            if nDelayTime = 0 then
            begin
              AttackTarget.SendMsg(TBaseObject(RM_STRUCK), RM_10101_2, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
                NativeInt(Player), IntToStr(MakeLong(Magic.wMagIdx, Integer(BlastHitType))));
            end
            else
            begin
              AttackTarget.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101_2, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
                NativeInt(Player), IntToStr(MakeLong(Magic.wMagIdx, Integer(BlastHitType))), nDelayTime);
            end;
          end
          else
          begin
            if nDelayTime = 0 then
            begin
              AttackTarget.SendMsg(TBaseObject(RM_STRUCK), RM_10101_2, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
                NativeInt(Player), 'MAG:' + IntToStr(MakeLong(Magic.wMagIdx, Integer(BlastHitType))));
            end
            else
            begin
              AttackTarget.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101_2, nDamage, AttackTarget.m_WAbil.HP, AttackTarget.m_WAbil.MaxHP,
                NativeInt(Player), 'MAG:' + IntToStr(MakeLong(Magic.wMagIdx, Integer(BlastHitType))), nDelayTime);
            end;
          end;
        end;
      end;

      { 移到这里面TBaseObject.Client10101，
        攻击人物时，在@StruckDamage触发中修改最终的伤害值，会导致反弹的比例对不上
        // 新增自定义技能/伤害反弹  2019-06-04 23:57:55
        nDamage := AttackTarget.DamageReboundPower(nDamage);
        if nDamage > 0 then
        begin                                               // 反弹伤害
        nDamage := Player.StruckDamage(nDamage, nil, 0);
        Player.SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, nDamage, Player.m_WAbil.HP, Player.m_WAbil.MaxHP, NativeInt(AttackTarget), 'FT', nDelayTime + 100);
        end;
      }

      // 麻痹
      if boMagicWarr and (not AttackTarget.UnParalysis) //
        and (Player.m_boParalysis or (Random(100) < Player.m_btFluteStoneParalysisRate)) //
        and (Random(Max(AttackTarget.m_btAntiPoison + Player.m_dwParalysisRate, 0)) = 0) //
      // 刺杀位不允许麻痹目标 2019-10-22 00:56:08
        and (abs(AttackTarget.m_nCurrX - Player.m_nCurrX) <= 1) //
        and (abs(AttackTarget.m_nCurrY - Player.m_nCurrY) <= 1) then
        AttackTarget.MakePosion(POISON_STONE, Player.m_dwParalysisTime, 0); // g_Config.nAttackPosionTime

      // 毒素武器
      if (g_Config.boPoisonWeaponCanMagicAttack or boMagicWarr) //
        and Player.m_boPoisonWeapon //
        and (not AttackTarget.UnPosion) //
        and (Random(Max(AttackTarget.m_btAntiPoison, 0)) = 0) //
        and (Random(100) < Player.m_nPoisonWeaponRate) then
      begin
        AttackTarget.m_PoisonHitter := Player;
        AttackTarget.MakePosion(POISON_DECHEALTH, Player.m_nPoisonWeaponBaseTime + Random(Player.m_nPoisonWeaponRandomTime),
          Player.m_nPoisonWeaponDamageHealth);
      end;

      // 冰冻戒指 chongchong 2013-11-20
      if (g_Config.boFrozenUseMagicStruck or boMagicWarr) //
        and (not AttackTarget.UnFrozen) //
        and (Player.m_boFrozen or (Random(100) < Player.m_btFluteStoneFrozenRate)) //
        and (Random(Max(AttackTarget.m_btAntiPoison + Player.m_dwFrozenRate, 0)) = 0) then
      begin
        AttackTarget.m_PoisonHitter := Player;
        AttackTarget.MakeFrozen(Player.m_dwFrozenTime);
      end;

      // 蛛网戒指 chongchong 2013-11-20
      if (g_Config.boCobwebWindingUseMagicStruck or boMagicWarr) //
        and (not AttackTarget.UnCobwebWinding) //
        and (Player.m_boCobwebWinding or (Random(100) < Player.m_btFluteStoneWindingRate)) //
        and (Random(Max(AttackTarget.m_btAntiPoison + Player.m_dwCobwebWindingRate, 0)) = 0) then
      begin
        AttackTarget.m_PoisonHitter := Player;
        AttackTarget.OpenCobwebWinding(Player.m_dwCobwebWindingTime);
      end;

      // 魔道麻痹 chongchong 2013-11-20
      if (not boMagicWarr) //
        and (not AttackTarget.UnParalysis) //
        and (Player.m_boMDParalysis or (Random(100) < Player.m_btFluteStoneMDParalysisRate)) //
        and (Random(Max(AttackTarget.m_btAntiPoison + Player.m_dwMDParalysisRate, 0)) = 0) then
      begin
        AttackTarget.m_PoisonHitter := Player;
        AttackTarget.MakePosion(POISON_STONE, Player.m_dwMDParalysisTime, 0);
      end;
    end;

    if AttackTarget <> m_Target then
      TargetList.Add(AttackTarget);

    Add_AdditionalsDamage(AttackConfig, Magic, AttackTarget, Player);
  end;
end;

function GroupAttack(Player: TSmartObject; m_Target: TBaseObject; TargetX, TargetY: Integer; AttackConfig: PMagicServerConfig;
  BasePower: Integer; TargetList: TList; Magic: pTUserMagic; boMagicWarr: Boolean): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
begin
  Result := False;

  BaseObjectList := TList.Create;
  try
    if AttackConfig.AttackMode = mamNear then
      Player.GetMapBaseObjects(Player.m_PEnvir, Player.m_nCurrX, Player.m_nCurrY, AttackConfig.AttackGroupRange, BaseObjectList)
    else
      Player.GetMapBaseObjects(Player.m_PEnvir, TargetX, TargetY, AttackConfig.AttackGroupRange, BaseObjectList);

    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);

      if (BaseObject <> nil) //
        and (not BaseObject.m_boDeath) //
        and (not BaseObject.m_boGhost)
      { and (not BaseObject.m_boHideMode or Player.m_boCoolEye) }
        and Player.IsProperTarget(BaseObject) //
        and SingleAttack(Player, m_Target, BaseObject, AttackConfig, BasePower, TargetList, Magic, boMagicWarr) then
        Result := True;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

function LineAttack(Player: TSmartObject; m_Target: TBaseObject; TargetX, TargetY: Integer; Dir: Byte; AttackConfig:
  PMagicServerConfig; BasePower: Integer; TargetList: TList; Magic: pTUserMagic; boMagicWarr: Boolean): Boolean;
var
  BaseObject: TBaseObject;
  I, J, tmpX, tmpY, nX, nY, Power2: Integer;
begin
  Result := False;

  // 直线攻击增加宽度参数 Cursor 2023-06-07 10:09:19
  for J := -AttackConfig.AttackLineWidth to AttackConfig.AttackLineWidth do
  begin
    if not Player.m_PEnvir.GetSitInLinPosition(Player.m_nCurrX, Player.m_nCurrY, Dir, J, tmpX, tmpY) then
      Continue;

    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if Player.m_PEnvir.GetNextPosition(tmpX, tmpY, Dir, I, nX, nY) then
      begin
        BaseObject := TBaseObject(Player.m_PEnvir.GetMovingObject(nX, nY, True));

        if (BaseObject <> nil) //
          and (not BaseObject.m_boDeath) //
          and (not BaseObject.m_boGhost) //
          and Player.IsProperTarget(BaseObject) then
        begin
          Power2 := Max(0, BasePower + Round((I - 1) / 100 * BasePower * AttackConfig.AttackPowerLineAdd));

          if SingleAttack(Player, m_Target, BaseObject, AttackConfig, Power2, TargetList, Magic, boMagicWarr) then
            Result := True;
        end;
      end;
    end;
  end;
end;

function SwordWideAttack(Player: TSmartObject; m_Target: TBaseObject; Dir: Byte; AttackConfig: PMagicServerConfig; BasePower:
  Integer; TargetList: TList; Magic: pTUserMagic; boMagicWarr: Boolean): Boolean;
var
  NewDir, nC: Byte;
  I, nX, nY: Integer;
  BaseObject: TBaseObject;
begin
  nC := 0;
  Result := False;
  // 修复半月攻击无伤害 chongchong 2014-11-17
  for I := 1 to AttackConfig.AttackGroupRange do
  begin
    if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, Dir, I, nX, nY) then
    begin
      BaseObject := Player.m_PEnvir.GetMovingObject(nX, nY, True);
      if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
      { (not BaseObject.m_boHideMode or Player.m_boCoolEye) and }
        Player.IsProperTarget(BaseObject) then
      begin
        if SingleAttack(Player, m_Target, BaseObject, AttackConfig, BasePower, TargetList, Magic, boMagicWarr) then
          Result := True;
      end;
    end;
  end;
  while (True) do
  begin
    NewDir := (Dir + g_Config.WideAttack[nC]) mod 8;
    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, NewDir, I, nX, nY) then
      begin
        BaseObject := Player.m_PEnvir.GetMovingObject(nX, nY, True);
        if (BaseObject <> nil) and (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) and
        { (not BaseObject.m_boHideMode or Player.m_boCoolEye) and }
          Player.IsProperTarget(BaseObject) then
        begin
          if SingleAttack(Player, m_Target, BaseObject, AttackConfig, BasePower, TargetList, Magic, boMagicWarr) then
            Result := True;
        end;
      end;
    end;
    Inc(nC);
    if nC >= 3 then
      break;
  end;
end;

function Dir8Attack(Player: TSmartObject; m_Target: TBaseObject; AttackConfig: PMagicServerConfig; BasePower: Integer; TargetList:
  TList; Magic: pTUserMagic; boMagicWarr: Boolean): Boolean;
var
  I, nDir: Integer;
  nX, nY: Integer;
  BaseObject: TBaseObject;
  Power2: Integer;
begin
  Result := False;
  for nDir := 0 to 7 do
  begin
    for I := 1 to AttackConfig.AttackGroupRange do
    begin
      if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, nDir, I, nX, nY) then
      begin
        BaseObject := TBaseObject(Player.m_PEnvir.GetMovingObject(nX, nY, True));
        if (BaseObject <> nil) and Player.IsProperTarget(BaseObject) then
        begin
          Power2 := BasePower + Round((I - 1) / 100 * BasePower * AttackConfig.AttackPowerLineAdd);
          if Power2 < 0 then
            Power2 := 0;
          if SingleAttack(Player, m_Target, BaseObject, AttackConfig, Power2, TargetList, Magic, boMagicWarr) then
            Result := True;
        end;
      end;
    end;
  end;
end;

function Dir16Attack(Player: TSmartObject; m_Target: TBaseObject; AttackConfig: PMagicServerConfig; BasePower: Integer; TargetList:
  TList; Magic: pTUserMagic; boMagicWarr: Boolean): Boolean;
var
  I, nDir: Integer;
  nX, nY: Integer;
  BaseObject: TBaseObject;
  AttackList: TList;
  A: Extended;
  Power2, nOffsetX, nOffsetY, nDistance: Integer;
begin
  Result := False;
  AttackList := TList.Create;
  try
    // 原8个方向攻击目标
    for nDir := 0 to 7 do
    begin
      for I := 1 to AttackConfig.AttackGroupRange do
      begin
        if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, nDir, I, nX, nY) then
        begin
          BaseObject := TBaseObject(Player.m_PEnvir.GetMovingObject(nX, nY, True));
          if (BaseObject <> nil) and Player.IsProperTarget(BaseObject) then
          begin
            AttackList.Add(BaseObject);
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
        nX := Player.m_nCurrX + Trunc(I * Cos(A));
        nY := Player.m_nCurrY + Trunc(I * Sin(A));
        BaseObject := TBaseObject(Player.m_PEnvir.GetMovingObject(nX, nY, True));
        if (BaseObject <> nil) and Player.IsProperTarget(BaseObject) and (AttackList.IndexOf(BaseObject) = -1) then
        begin
          AttackList.Add(BaseObject);
        end;
      end;
    end;
    for I := 0 to AttackList.Count - 1 do
    begin
      BaseObject := AttackList.Items[I];
      nOffsetX := abs(BaseObject.m_nCurrX - Player.m_nCurrX);
      nOffsetY := abs(BaseObject.m_nCurrY - Player.m_nCurrY);
      if nOffsetX = 0 then
        nDistance := nOffsetY
      else
        nDistance := nOffsetX;
      Power2 := BasePower + Round((nDistance - 1) / 100 * BasePower * AttackConfig.AttackPowerLineAdd);
      if Power2 < 0 then
        Power2 := 0;
      if SingleAttack(Player, m_Target, BaseObject, AttackConfig, Power2, TargetList, Magic, boMagicWarr) then
        Result := True;
    end;
  finally
    AttackList.Free;
  end;
end;

function DoPushedWith100(Player: TSmartObject; MagicID: Integer; TargetX, TargetY: Integer; Range: Integer; PushedHighLevel:
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
  PushedObject: TPushedObject;
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
            break;
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
            break; // 推动第一格的角色
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
            break;
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
  Player.SendRefMsg(RM_CUSTOM_PUSH, MakeLong(Player.m_btDirection, MagicID), Player.m_nCurrX, Player.m_nCurrY, nSelfStep,
    sPushedInfo);
end;

procedure DoPushed(Player: TSmartObject; MagicID: Integer; TargetX, TargetY: Integer; Range: Integer; PushedHighLevel: Boolean;
  PushedType: Integer);

  function CanMotaebo(BaseObject: TBaseObject; PushedDir: Byte): Boolean;
  var
    nC: Integer;
    nTempX, nTempY: Integer;
  begin
    Result := False;
    if BaseObject.m_boImprison then
    begin
      nTempX := BaseObject.m_nCurrX;
      nTempY := BaseObject.m_nCurrY;
      BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, PushedDir, 1, nTempX, nTempY);
      if (nTempX < BaseObject.m_nImprisonPos.X - BaseObject.m_nImprisonRange) or (nTempX > BaseObject.m_nImprisonPos.X +
        BaseObject.m_nImprisonRange) or (nTempY < BaseObject.m_nImprisonPos.Y - BaseObject.m_nImprisonRange) or (nTempY >
        BaseObject.m_nImprisonPos.Y + BaseObject.m_nImprisonRange) then
      begin
        Exit;
      end;
    end;
    // 宠物无实体模式 2019-11-15 22:18:42
    if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
    begin
      Result := True;
      Exit;
    end;
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
  I, J, nX, nY: Integer;
  PoseCreate, PushObject: TBaseObject;
  PushList: TList;
  btNewDir: Integer;
  VisibleBaseObject: pTVisibleBaseObject;
  bo35: Boolean;
begin
  if Range = 0 then
    Exit;
  PushList := TList.Create;
  if PushedType in [0, 2] then
  begin
    try
      for I := 0 to Range - 1 do
      begin
        if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, Player.m_btDirection, I + 1, nX, nY) then
        begin
          PushObject := Player.m_PEnvir.GetMovingObject(nX, nY, True);
          if PushObject <> nil then
          begin
            if not CanMotaebo(PushObject, Player.m_btDirection) then
              break;
            PushList.Add(PushObject);
          end;
        end
      end;
      if I = 0 then
        Exit;
      Range := Min(Range, I + 1);
      for I := 1 to Range do
      begin
        for J := PushList.Count - 1 downto 0 do
        begin
          PushObject := PushList.Items[J];
          if PushObject.m_boImprison then
          begin
            nX := PushObject.m_nCurrX;
            nY := PushObject.m_nCurrY;
            PushObject.m_PEnvir.GetNextPosition(PushObject.m_nCurrX, PushObject.m_nCurrY, Player.m_btDirection, 1, nX, nY);
            if (nX < PushObject.m_nImprisonPos.X - PushObject.m_nImprisonRange) or (nX > PushObject.m_nImprisonPos.X + PushObject.m_nImprisonRange)
              or (nY < PushObject.m_nImprisonPos.Y - PushObject.m_nImprisonRange) or (nY > PushObject.m_nImprisonPos.Y +
              PushObject.m_nImprisonRange) then
            begin
              break;
            end;
          end;
          PushObject.CharPushed(Player.m_btDirection, 1);
        end;
        if PushedType = 0 then
        begin
          Player.GetFrontPosition(nX, nY);
          if Player.m_boImprison then
          begin
            if (nX < Player.m_nImprisonPos.X - Player.m_nImprisonRange) or (nX > Player.m_nImprisonPos.X + Player.m_nImprisonRange)
              or (nY < Player.m_nImprisonPos.Y - Player.m_nImprisonRange) or (nY > Player.m_nImprisonPos.Y + Player.m_nImprisonRange)
              then
            begin
              break;
            end;
          end;
          if Player.m_PEnvir.MoveToMovingObject(Player.m_nCurrX, Player.m_nCurrY, Player, nX, nY, False) then
          begin // 自己也往前走动
            Player.m_nCurrX := nX;
            Player.m_nCurrY := nY;
            Player.SendRefMsg(RM_RUSH, Player.m_btDirection, Player.m_nCurrX, Player.m_nCurrY, 0, '');
          end;
        end;
      end;
    finally
      PushList.Free;
    end;
  end
  else if PushedType = 1 then
  begin
    Player.m_VisibleActors.Lock;
    try
      for I := 0 to Player.m_VisibleActors.Count - 1 do
      begin
        VisibleBaseObject := Player.m_VisibleActors[I].Item;
        if VisibleBaseObject <> nil then
        begin
          PushObject := TBaseObject(VisibleBaseObject.BaseObject);
          if (abs(Player.m_nCurrX - PushObject.m_nCurrX) <= 1) and (abs(Player.m_nCurrY - PushObject.m_nCurrY) <= 1) then
          begin
            if (not PushObject.m_boDeath) and (PushObject <> Player) then
            begin
              btNewDir := GetNextDirection(Player.m_nCurrX, Player.m_nCurrY, PushObject.m_nCurrX, PushObject.m_nCurrY);
              if CanMotaebo(PushObject, btNewDir) then
              begin
                PushObject.CharPushed(btNewDir, Range);
              end;
            end;
          end;
        end;
      end;
    finally
      Player.m_VisibleActors.UnLock;
    end;
  end
  else if PushedType = 3 then
  begin
    for btNewDir := 0 to 7 do
    begin
      if Player.m_PEnvir.GetNextPosition(TargetX, TargetY, btNewDir, 1, nX, nY) then
      begin
        PushObject := Player.m_PEnvir.GetMovingObject(nX, nY, True);
        if (PushObject <> nil) and (not PushObject.m_boDeath) and (PushObject <> Player) then
        begin
          if CanMotaebo(PushObject, btNewDir) then
          begin
            PushObject.CharPushed(btNewDir, Range);
          end;
        end;
      end;
    end;
    // 中心对象也推动 chongchong 2016-04-04
    PushObject := Player.m_PEnvir.GetMovingObject(TargetX, TargetY, True);
    if (PushObject <> nil) and (not PushObject.m_boDeath) and (PushObject <> Player) then
    begin
      btNewDir := GetNextDirection(Player.m_nCurrX, Player.m_nCurrY, TargetX, TargetY);
      if CanMotaebo(PushObject, btNewDir) then
      begin
        PushObject.CharPushed(btNewDir, Range);
      end;
    end;
  end
  else if PushedType = 4 then
  begin
    bo35 := True;
    PoseCreate := Player.GetPoseCreate();
    if (PoseCreate <> nil) then
    begin
      Range := Max(Range, 1);
      for I := 1 to Range do
      begin
        PoseCreate := Player.GetPoseCreate();
        if PoseCreate <> nil then
        begin
          if not CanMotaebo(PoseCreate, Player.m_btDirection) then
            break;
          Player.GetFrontPosition(nX, nY);
          if Player.m_boImprison then
          begin
            if (nX < Player.m_nImprisonPos.X - Player.m_nImprisonRange) or (nX > Player.m_nImprisonPos.X + Player.m_nImprisonRange)
              or (nY < Player.m_nImprisonPos.Y - Player.m_nImprisonRange) or (nY > Player.m_nImprisonPos.Y + Player.m_nImprisonRange)
              then
            begin
              break;
            end;
          end;
          // 自定义技能野蛮可以推动2格目标  2020-04-28
          if Player.m_PEnvir.GetNextPosition(Player.m_nCurrX, Player.m_nCurrY, Player.m_btDirection, 2, nX, nY) then
          begin // 推动第二格的角色
            PushObject := Player.m_PEnvir.GetMovingObject(nX, nY, True);
            if (PushObject <> nil) and CanMotaebo(PushObject, Player.m_btDirection) then
              PushObject.CharPushed(Player.m_btDirection, 1);
          end;
          if PoseCreate.CharPushed(Player.m_btDirection, 1) <> 1 then
            break;
          Player.GetFrontPosition(nX, nY);
          if Player.m_PEnvir.MoveToMovingObject(Player.m_nCurrX, Player.m_nCurrY, Player, nX, nY, False) then
          begin
            Player.m_nCurrX := nX;
            Player.m_nCurrY := nY;
            Player.SendRefMsg(RM_RUSH, Player.m_btDirection, Player.m_nCurrX, Player.m_nCurrY, 0, '');
            bo35 := False;
          end;
        end;
      end;
    end
    else
    begin
      bo35 := False;
      Range := Max(Range, 1);
      for I := 1 to Range do
      begin
        Player.GetFrontPosition(nX, nY); // sub_004B2790
        if Player.m_boImprison then
        begin
          if (nX < Player.m_nImprisonPos.X - Player.m_nImprisonRange) or (nX > Player.m_nImprisonPos.X + Player.m_nImprisonRange)
            or (nY < Player.m_nImprisonPos.Y - Player.m_nImprisonRange) or (nY > Player.m_nImprisonPos.Y + Player.m_nImprisonRange)
            then
          begin
            bo35 := True;
            break;
          end;
        end;
        if Player.m_PEnvir.MoveToMovingObject(Player.m_nCurrX, Player.m_nCurrY, Player, nX, nY, False) then
        begin
          Player.m_nCurrX := nX;
          Player.m_nCurrY := nY;
          Player.SendRefMsg(RM_RUSH, Player.m_btDirection, Player.m_nCurrX, Player.m_nCurrY, 0, '');
        end
        else
        begin
          if not Player.m_PEnvir.CanWalk(nX, nY, True) then
          begin
            bo35 := True;
            break;
          end;
        end;
      end;
    end;
    if bo35 then
    begin
      Player.GetFrontPosition(nX, nY);
      Player.SendRefMsg(RM_RUSHKUNG, Player.m_btDirection, nX, nY, 0, '');
      Player.SysMsg(sMateDoTooweak { 冲撞力不够！ } , c_Red, t_Hint);
    end;
  end
  else if PushedType = 5 then
  begin
    DoPushedWith100(Player, MagicID, TargetX, TargetY, Range, PushedHighLevel);
  end;
end;

function TMagicManager.MagCustomSkill(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject; var sMsg: string; var boMoved: Boolean; btNpcReleaseMagic: Byte; var boSpellFire: Boolean):
  Boolean;
var
  MagicConfig: TCustomMagicConfig;
  ServerConfig: TMagicServerConfig;
  ClientConfig: PMagicClientConfig;
  I, J, K, M, II: Integer;
  BaseObjectList, TempList: TList;
  MonObj, TempObj: TBaseObject;
  nPower, V1, V2: Integer;
  bt06: Byte;
  IsHealthSpellChanged: Boolean;
  TargetList: TList;
  SendTargetEffect: Boolean;
  EffectEvent: TCustomMagicEffectEvent;
  nStartX, nStartY, nEndX, nEndY: Integer;
  Additionals: TAdditionalDamageArray;
  SelfKeepPlay: TSelfKeepPlay;
  S: AnsiString;
  HaveBBCount: Integer;
  KeepTime: LongWord;
  boRecalcAbilitys: Boolean;
  Int64Value: Int64;
  Flag: TWalkFlagArr;
  PowerRate: Integer;
  nLevel: Integer;
  AddHP, AddMP: Integer;
  IsTeleport: Boolean;
  ElementsType: TItemElementsType;
  NexExtType: TAddNewExtType;
  SmartObject: TSmartObject;
  BreakDefenseType: TBreakDefenseType;
  AddAttribType: TMagicProtectAddAttributesType;
  DecAttribType: TMagicAttackDecAttributesType;
  GameEvent: TGameEvent;
begin
  sMsg := '';
  Result := False;
  boMoved := False;
  FNpcReleaseMagic := btNpcReleaseMagic;
  MagicConfig := GetCustomMagicConfig(UserMagic.wMagIdx);
  if MagicConfig = nil then
    Exit;

  if MagicConfig.ServerConfig.DisableInSafeZone then
  begin
    if PlayObject.InSafeZone(PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY) then
    begin
      S := StringReplace(g_sDisableInSafeZoneCustomMagic, '%name', UserMagic.MagicInfo.sMagicName, [rfIgnoreCase]);
      PlayObject.SysMsg(S, c_Red, t_Notice);
      boSpellFire := False;
      Exit;
    end;

    if PlayObject.InSafeZone(PlayObject.m_PEnvir, nTargetX, nTargetY) then
    begin
      S := StringReplace(g_sDisableInSafeZoneCustomMagic, '%name', UserMagic.MagicInfo.sMagicName, [rfIgnoreCase]);
      PlayObject.SysMsg(S, c_Red, t_Notice);
      boSpellFire := False;
      Exit;
    end;
  end;

  nLevel := UserMagic.btNewLevel + UserMagic.btLevel;
  ServerConfig := MagicConfig.ServerConfig;

  for AddAttribType := Low(TMagicProtectAddAttributesType) to High(TMagicProtectAddAttributesType) do
  begin
    ServerConfig.ProtectAddAttrib[AddAttribType].Rate := ServerConfig.ProtectAddAttrib[AddAttribType].Rate + nLevel * ServerConfig.ProtectAddAttrib
      [AddAttribType].RateAdd;

    ServerConfig.ProtectAddAttrib[AddAttribType].LowValue := ServerConfig.ProtectAddAttrib[AddAttribType].LowValue + nLevel *
      ServerConfig.ProtectAddAttrib[AddAttribType].LowValueAdd;

    ServerConfig.ProtectAddAttrib[AddAttribType].HighValue := ServerConfig.ProtectAddAttrib[AddAttribType].HighValue + nLevel *
      ServerConfig.ProtectAddAttrib[AddAttribType].HighValueAdd;

    if not ServerConfig.ProtectAddAttrib[AddAttribType].TimeAddIsPoint then
      ServerConfig.ProtectAddAttrib[AddAttribType].Time := ServerConfig.ProtectAddAttrib[AddAttribType].Time + Round(ServerConfig.ProtectAddAttrib
        [AddAttribType].Time / 100 * nLevel * ServerConfig.ProtectAddAttrib[AddAttribType].TimeAdd)
    else
      ServerConfig.ProtectAddAttrib[AddAttribType].Time := ServerConfig.ProtectAddAttrib[AddAttribType].Time + nLevel *
        ServerConfig.ProtectAddAttrib[AddAttribType].TimeAdd;
  end;

  if ServerConfig.ProtectTargetStatusTimeUnit = 0 then
    ServerConfig.ProtectTargetStatusTime := ServerConfig.ProtectTargetStatusTime + Round(ServerConfig.ProtectTargetStatusTime /
      100 * nLevel * ServerConfig.ProtectTargetStatusTime2)
  else
    ServerConfig.ProtectTargetStatusTime := ServerConfig.ProtectTargetStatusTime + nLevel * ServerConfig.ProtectTargetStatusTime2;

  for ElementsType := Low(TItemElementsType) to High(TItemElementsType) do
  begin
    ServerConfig.ProtectAddElements[ElementsType].Rate := ServerConfig.ProtectAddElements[ElementsType].Rate + nLevel *
      ServerConfig.ProtectAddElements[ElementsType].RateAdd;

    if not ServerConfig.ProtectAddElements[ElementsType].TimeAddIsPoint then
      ServerConfig.ProtectAddElements[ElementsType].Time := ServerConfig.ProtectAddElements[ElementsType].Time + Round(ServerConfig.ProtectAddElements
        [ElementsType].Time / 100 * nLevel * ServerConfig.ProtectAddElements[ElementsType].TimeAdd)
    else
      ServerConfig.ProtectAddElements[ElementsType].Time := ServerConfig.ProtectAddElements[ElementsType].Time + nLevel *
        ServerConfig.ProtectAddElements[ElementsType].TimeAdd;

    ServerConfig.ProtectAddElements[ElementsType].Value := ServerConfig.ProtectAddElements[ElementsType].Value + nLevel *
      ServerConfig.ProtectAddElements[ElementsType].ValueAdd;
  end;

  for DecAttribType := Low(TMagicAttackDecAttributesType) to High(TMagicAttackDecAttributesType) do
  begin
    ServerConfig.AttackSubAttrib[DecAttribType].Rate := ServerConfig.AttackSubAttrib[DecAttribType].Rate + nLevel * ServerConfig.AttackSubAttrib
      [DecAttribType].RateAdd;

    ServerConfig.AttackSubAttrib[DecAttribType].LowValue := ServerConfig.AttackSubAttrib[DecAttribType].LowValue + nLevel *
      ServerConfig.AttackSubAttrib[DecAttribType].LowValueAdd;

    ServerConfig.AttackSubAttrib[DecAttribType].HighValue := ServerConfig.AttackSubAttrib[DecAttribType].HighValue + nLevel *
      ServerConfig.AttackSubAttrib[DecAttribType].HighValueAdd;

    if not ServerConfig.AttackSubAttrib[DecAttribType].TimeAddIsPoint then
      ServerConfig.AttackSubAttrib[DecAttribType].Time := ServerConfig.AttackSubAttrib[DecAttribType].Time + Round(ServerConfig.AttackSubAttrib
        [DecAttribType].Time / 100 * nLevel * ServerConfig.AttackSubAttrib[DecAttribType].TimeAdd)
    else
      ServerConfig.AttackSubAttrib[DecAttribType].Time := ServerConfig.AttackSubAttrib[DecAttribType].Time + nLevel * ServerConfig.AttackSubAttrib
        [DecAttribType].TimeAdd;
  end;

  for ElementsType := Low(TItemElementsType) to High(TItemElementsType) do
  begin
    ServerConfig.AttackSubElements[ElementsType].Rate := ServerConfig.AttackSubElements[ElementsType].Rate + nLevel * ServerConfig.AttackSubElements
      [ElementsType].RateAdd;

    if not ServerConfig.AttackSubElements[ElementsType].TimeAddIsPoint then
      ServerConfig.AttackSubElements[ElementsType].Time := ServerConfig.AttackSubElements[ElementsType].Time + Round(ServerConfig.AttackSubElements
        [ElementsType].Time / 100 * nLevel * ServerConfig.AttackSubElements[ElementsType].TimeAdd)
    else
      ServerConfig.AttackSubElements[ElementsType].Time := ServerConfig.AttackSubElements[ElementsType].Time + nLevel *
        ServerConfig.AttackSubElements[ElementsType].TimeAdd;

    ServerConfig.AttackSubElements[ElementsType].Value := ServerConfig.AttackSubElements[ElementsType].Value + nLevel *
      ServerConfig.AttackSubElements[ElementsType].ValueAdd;
  end;

  for BreakDefenseType := Low(TBreakDefenseType) to High(TBreakDefenseType) do
  begin
    ServerConfig.AttackBreakDefense[BreakDefenseType].Rate := ServerConfig.AttackBreakDefense[BreakDefenseType].Rate + nLevel *
      ServerConfig.AttackBreakDefense[BreakDefenseType].RateAdd;

    ServerConfig.AttackBreakDefense[BreakDefenseType].Value := Min(100, ServerConfig.AttackBreakDefense[BreakDefenseType].Value +
      nLevel * ServerConfig.AttackBreakDefense[BreakDefenseType].ValueAdd);
  end;

  case UserMagic.btNewLevel of
    0:
      ClientConfig := @MagicConfig.ClientConfigs[mplNone];
    1..3:
      ClientConfig := @MagicConfig.ClientConfigs[mpl1_3];
    4..6:
      ClientConfig := @MagicConfig.ClientConfigs[mpl4_6];
  else
    ClientConfig := @MagicConfig.ClientConfigs[mpl7_9];
  end;

  if ServerConfig.OperateMode = momProtect then
  begin
    if TargeTBaseObject = nil then
    begin
      TargeTBaseObject := PlayObject;
      nTargetX := TargeTBaseObject.m_nCurrX;
      nTargetY := TargeTBaseObject.m_nCurrY;
    end;

    KeepTime := ClientConfig.SelfKeep_KeepTime + nLevel * ClientConfig.SelfKeep_KeepTime2;
    if (ClientConfig.SelfKeep_File >= 0) and (ClientConfig.SelfKeep_PlayCount > 0) and (KeepTime > 0) then
    begin
      SelfKeepPlay.SelfKeep_File := ClientConfig.SelfKeep_File;
      SelfKeepPlay.SelfKeep_StartIndex := ClientConfig.SelfKeep_StartIndex;
      SelfKeepPlay.SelfKeep_PlayCount := ClientConfig.SelfKeep_PlayCount;
      SelfKeepPlay.SelfKeep_PlayTime := ClientConfig.SelfKeep_PlayTime;
      SelfKeepPlay.SelfKeep_DrawMode := ClientConfig.SelfKeep_DrawMode;
      SelfKeepPlay.SelfKeep_KeepTime := KeepTime;
      SelfKeepPlay.SelfKeep_StartIndex2 := -1;
      SelfKeepPlay.SelfKeep_DrawOrder := mdoPriorSelf;
      SelfKeepPlay.SelfKeep_DrawMode2 := mdmBlend;
      SetLength(S, SizeOf(SelfKeepPlay));
      Move(SelfKeepPlay, S[1], Length(S));
      PlayObject.m_MagicSelfPlayTick := MyGetTickCount;
      PlayObject.m_MagicSelfPlay := SelfKeepPlay;
      PlayObject.SendRefMsg(RM_CUSTOM_MAGIC_SELFKEEP_PLAY, Length(S), 0, 0, 0, S, 1000);
    end;

    TargetList := TList.Create;
    try
      BaseObjectList := TList.Create;
      try
        PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, nTargetX, nTargetY, ServerConfig.ProtectTargetRange, BaseObjectList);
        for I := 0 to BaseObjectList.Count - 1 do
        begin
          MonObj := TBaseObject(BaseObjectList[I]);
          if (MonObj <> nil) and (not MonObj.m_boGhost) and (not MonObj.m_boDeath) and PlayObject.IsProperFriend(MonObj) then
          begin
            Result := True;
            boRecalcAbilitys := False;

            // 加防御 chongchong 2014-09-08
            if ServerConfig.ProtectAddAttrib[aaAC].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaAC].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaAC].Rate) //
              and ((ServerConfig.ProtectAddAttrib[aaAC].LowValue > 0) or (ServerConfig.ProtectAddAttrib[aaAC].HighValue > 0)) //
              and (ServerConfig.ProtectAddAttrib[aaAC].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaAC].LowValueIsPoint then
                V1 := Round((MonObj.m_WAbil.AC1 - MonObj.m_AddNewExtValues[anet_AddAC1]) / 100 * ServerConfig.ProtectAddAttrib[aaAC].LowValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaAC].LowValue;

              if not ServerConfig.ProtectAddAttrib[aaAC].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.AC2 - MonObj.m_AddNewExtValues[anet_AddAC2]) / 100 * ServerConfig.ProtectAddAttrib[aaAC].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaAC].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddAC1] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaAC].Time) * 1000;
              MonObj.m_AddNewExtTicks[anet_AddAC2] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaAC].Time) * 1000;
              MonObj.m_AddNewExtValues[anet_AddAC1] := Max(MonObj.m_AddNewExtValues[anet_AddAC1], V1);
              MonObj.m_AddNewExtValues[anet_AddAC2] := Max(MonObj.m_AddNewExtValues[anet_AddAC2], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaAC].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaAC].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaAC].HintText, '%AC1', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%AC2', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaAC].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加魔御
            if ServerConfig.ProtectAddAttrib[aaMAC].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaMAC].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaMAC].Rate) //
              and ((ServerConfig.ProtectAddAttrib[aaMAC].LowValue > 0) or (ServerConfig.ProtectAddAttrib[aaMAC].HighValue > 0)) //
              and (ServerConfig.ProtectAddAttrib[aaMAC].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaMAC].LowValueIsPoint then
                V1 := Round((MonObj.m_WAbil.MAC1 - MonObj.m_AddNewExtValues[anet_AddMAC1]) / 100 * ServerConfig.ProtectAddAttrib[aaMAC].LowValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaMAC].LowValue;

              if not ServerConfig.ProtectAddAttrib[aaMAC].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.MAC2 - MonObj.m_AddNewExtValues[anet_AddMAC2]) / 100 * ServerConfig.ProtectAddAttrib[aaMAC].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaMAC].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddMAC1] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaMAC].Time) * 1000;

              MonObj.m_AddNewExtTicks[anet_AddMAC2] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaMAC].Time) * 1000;

              MonObj.m_AddNewExtValues[anet_AddMAC1] := Max(MonObj.m_AddNewExtValues[anet_AddMAC1], V1);
              MonObj.m_AddNewExtValues[anet_AddMAC2] := Max(MonObj.m_AddNewExtValues[anet_AddMAC2], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaMAC].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaMAC].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaMAC].HintText, '%MAC1', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%MAC2', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaMAC].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加物理攻击
            if ServerConfig.ProtectAddAttrib[aaDC].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaDC].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaDC].Rate) //
              and ((ServerConfig.ProtectAddAttrib[aaDC].LowValue > 0) or (ServerConfig.ProtectAddAttrib[aaDC].HighValue > 0)) //
              and (ServerConfig.ProtectAddAttrib[aaDC].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaDC].LowValueIsPoint then
                V1 := Round((MonObj.m_WAbil.DC1 - MonObj.m_AddNewExtValues[anet_AddDC1]) / 100 * ServerConfig.ProtectAddAttrib[aaDC].LowValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaDC].LowValue;

              if not ServerConfig.ProtectAddAttrib[aaDC].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.DC2 - MonObj.m_AddNewExtValues[anet_AddDC2]) / 100 * ServerConfig.ProtectAddAttrib[aaDC].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaDC].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddDC1] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaDC].Time) * 1000;
              MonObj.m_AddNewExtTicks[anet_AddDC2] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaDC].Time) * 1000;
              MonObj.m_AddNewExtValues[anet_AddDC1] := Max(MonObj.m_AddNewExtValues[anet_AddDC1], V1);
              MonObj.m_AddNewExtValues[anet_AddDC2] := Max(MonObj.m_AddNewExtValues[anet_AddDC2], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaDC].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaDC].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaDC].HintText, '%DC1', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%DC2', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaDC].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加魔法攻击力
            if ServerConfig.ProtectAddAttrib[aaMC].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaMC].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaMC].Rate) //
              and ((ServerConfig.ProtectAddAttrib[aaMC].LowValue > 0) or (ServerConfig.ProtectAddAttrib[aaMC].HighValue > 0)) //
              and (ServerConfig.ProtectAddAttrib[aaMC].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaMC].LowValueIsPoint then
                V1 := Round((MonObj.m_WAbil.MC1 - MonObj.m_AddNewExtValues[anet_AddMC1]) / 100 * ServerConfig.ProtectAddAttrib[aaMC].LowValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaMC].LowValue;

              if not ServerConfig.ProtectAddAttrib[aaMC].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.MC2 - MonObj.m_AddNewExtValues[anet_AddMC2]) / 100 * ServerConfig.ProtectAddAttrib[aaMC].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaMC].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddMC1] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaMC].Time) * 1000;
              MonObj.m_AddNewExtTicks[anet_AddMC2] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaMC].Time) * 1000;
              MonObj.m_AddNewExtValues[anet_AddMC1] := Max(MonObj.m_AddNewExtValues[anet_AddMC1], V1);
              MonObj.m_AddNewExtValues[anet_AddMC2] := Max(MonObj.m_AddNewExtValues[anet_AddMC2], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaMC].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaMC].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaMC].HintText, '%MC1', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%MC2', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaMC].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加道术
            if ServerConfig.ProtectAddAttrib[aaSC].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaSC].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaSC].Rate) //
              and ((ServerConfig.ProtectAddAttrib[aaSC].LowValue > 0) or (ServerConfig.ProtectAddAttrib[aaSC].HighValue > 0)) //
              and (ServerConfig.ProtectAddAttrib[aaSC].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaSC].LowValueIsPoint then
                V1 := Round((MonObj.m_WAbil.SC1 - MonObj.m_AddNewExtValues[anet_AddSC1]) / 100 * ServerConfig.ProtectAddAttrib[aaSC].LowValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaSC].LowValue;

              if not ServerConfig.ProtectAddAttrib[aaSC].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.SC2 - MonObj.m_AddNewExtValues[anet_AddSC2]) / 100 * ServerConfig.ProtectAddAttrib[aaSC].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaSC].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddSC1] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaSC].Time) * 1000;
              MonObj.m_AddNewExtTicks[anet_AddSC2] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaSC].Time) * 1000;
              MonObj.m_AddNewExtValues[anet_AddSC1] := Max(MonObj.m_AddNewExtValues[anet_AddSC1], V1);
              MonObj.m_AddNewExtValues[anet_AddSC2] := Max(MonObj.m_AddNewExtValues[anet_AddSC2], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaSC].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaSC].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaSC].HintText, '%SC1', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%SC2', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaSC].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加攻击点
            if ServerConfig.ProtectAddAttrib[aaHitPoint].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaHitPoint].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaHitPoint].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaHitPoint].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaHitPoint].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaHitPoint].HighValueIsPoint then
                V2 := Round((MonObj.m_btHitPoint - MonObj.m_AddNewExtValues[anet_AddHitPoint]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaHitPoint].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaHitPoint].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddHitPoint] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaHitPoint].Time)
                * 1000;
              MonObj.m_AddNewExtValues[anet_AddHitPoint] := Max(MonObj.m_AddNewExtValues[anet_AddHitPoint], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaHitPoint].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaHitPoint].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaHitPoint].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaHitPoint].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            if ServerConfig.ProtectAddAttrib[aaSpeedPoint].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaSpeedPoint].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaSpeedPoint].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaSpeedPoint].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaSpeedPoint].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaSpeedPoint].HighValueIsPoint then
                V2 := Round((MonObj.m_btSpeedPoint - MonObj.m_AddNewExtValues[anet_AddSpeedPoint]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaSpeedPoint].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaSpeedPoint].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddSpeedPoint] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaSpeedPoint].Time)
                * 1000;
              MonObj.m_AddNewExtValues[anet_AddSpeedPoint] := V2;
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaSpeedPoint].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaSpeedPoint].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaSpeedPoint].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaSpeedPoint].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            if ServerConfig.ProtectAddAttrib[aaAntiMagic].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaAntiMagic].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaAntiMagic].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaAntiMagic].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaAntiMagic].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaAntiMagic].HighValueIsPoint then
                V2 := Round((MonObj.m_nAntiMagic - MonObj.m_AddNewExtValues[anet_AddAntiMagic]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaAntiMagic].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaAntiMagic].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddAntiMagic] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaAntiMagic].Time)
                * 1000;
              MonObj.m_AddNewExtValues[anet_AddAntiMagic] := Max(MonObj.m_AddNewExtValues[anet_AddAntiMagic], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaAntiMagic].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaAntiMagic].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaAntiMagic].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaAntiMagic].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            if ServerConfig.ProtectAddAttrib[aaAntiPoison].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaAntiPoison].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaAntiPoison].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaAntiPoison].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaAntiPoison].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaAntiPoison].HighValueIsPoint then
                V2 := Round((MonObj.m_btAntiPoison - MonObj.m_AddNewExtValues[anet_AddAntiPoison]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaAntiPoison].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaAntiPoison].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddAntiPoison] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaAntiPoison].Time)
                * 1000;
              MonObj.m_AddNewExtValues[anet_AddAntiPoison] := Max(MonObj.m_AddNewExtValues[anet_AddAntiPoison], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaAntiPoison].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaAntiPoison].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaAntiPoison].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaAntiPoison].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加内功伤害 chongchong 2014-09-08
            if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
              and TSmartObject(MonObj).m_boTrainingNG //
              and ServerConfig.ProtectAddAttrib[aaNGDamage].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaNGDamage].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaNGDamage].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaNGDamage].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaNGDamage].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaNGDamage].HighValueIsPoint then
                V1 := Round((TSmartObject(MonObj).m_AddNGDamage - MonObj.m_AddNewExtValues[anet_AddNGDamage]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaNGDamage].HighValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaNGDamage].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddNGDamage] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaNGDamage].Time)
                * 1000;
              MonObj.m_AddNewExtValues[anet_AddNGDamage] := Max(MonObj.m_AddNewExtValues[anet_AddNGDamage], V1);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaNGDamage].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaNGDamage].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaNGDamage].HintText, '%AddNGDamage', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaNGDamage].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加内功防御 chongchong 2014-09-08
            if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
              and TSmartObject(MonObj).m_boTrainingNG //
              and ServerConfig.ProtectAddAttrib[aaNGDefense].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaNGDefense].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaNGDefense].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaNGDefense].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaNGDefense].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaNGDefense].HighValueIsPoint then
                V1 := Round((TSmartObject(MonObj).m_DecNGDamage - MonObj.m_AddNewExtValues[anet_AddNGDefense]) / 100 *
                  ServerConfig.ProtectAddAttrib[aaNGDefense].HighValue)
              else
                V1 := ServerConfig.ProtectAddAttrib[aaNGDefense].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddNGDefense] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaNGDefense].Time)
                * 1000;
              MonObj.m_AddNewExtValues[anet_AddNGDefense] := Max(MonObj.m_AddNewExtValues[anet_AddNGDefense], V1);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaNGDefense].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaNGDefense].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaNGDefense].HintText, '%AddNGDefense', IntToStr(V1), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaNGDefense].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加血 chongchong 2014-09-07
            if ServerConfig.ProtectAddAttrib[aaHP].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaHP].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaHP].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaHP].HighValue > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaHP].HighValueIsPoint then
                AddHP := Round(MonObj.m_WAbil.MaxHP / 100 * ServerConfig.ProtectAddAttrib[aaHP].HighValue)
              else
                AddHP := ServerConfig.ProtectAddAttrib[aaHP].HighValue;
              if ServerConfig.ProtectAddHPSlow then
              begin
                MonObj.m_wCurrMagicId := UserMagic.wMagIdx;
                if ServerConfig.ProtectAddHpSlowCount <= 0 then
                  MonObj.m_CustomMagicPerHealing := 0
                else
                  MonObj.m_CustomMagicPerHealing := AddHP div ServerConfig.ProtectAddHpSlowCount;
                Inc(MonObj.m_nIncHealing, AddHP);
              end
              else
              begin
                MonObj.m_WAbil.HP := Min(MonObj.m_WAbil.HP + AddHP, MonObj.m_WAbil.MaxHP);
                MonObj.HealthSpellChanged;
                if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                  and ServerConfig.ProtectAddAttrib[aaHP].ShowHint //
                  and (Length(ServerConfig.ProtectAddAttrib[aaHP].HintText) > 0) then
                begin
                  S := StringReplace(ServerConfig.ProtectAddAttrib[aaHP].HintText, '%Point', IntToStr(AddHP), [rfIgnoreCase]);
                  TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
                end;
              end;
            end;

            // 加蓝
            if ServerConfig.ProtectAddAttrib[aaMP].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaMP].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaMP].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaMP].HighValue > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaMP].HighValueIsPoint then
                AddMP := Round(MonObj.m_WAbil.MaxMP / 100 * ServerConfig.ProtectAddAttrib[aaMP].HighValue)
              else
                AddMP := ServerConfig.ProtectAddAttrib[aaMP].HighValue;
              MonObj.m_WAbil.MP := Min(MonObj.m_WAbil.MP + AddMP, MonObj.m_WAbil.MaxMP);
              MonObj.HealthSpellChanged;
              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaMP].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaMP].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaMP].HintText, '%Point', IntToStr(AddMP), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加最大血量
            if ServerConfig.ProtectAddAttrib[aaMaxHP].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaMaxHP].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaMaxHP].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaMaxHP].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaMaxHP].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaMaxHP].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.MaxHP - MonObj.m_AddNewExtValues[anet_AddMaxHP]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaMaxHP].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaMaxHP].HighValue;
              MonObj.m_AddNewExtTicks[anet_AddMaxHP] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaMaxHP].Time) *
                1000;
              MonObj.m_AddNewExtValues[anet_AddMaxHP] := Max(MonObj.m_AddNewExtValues[anet_AddMaxHP], V2);
              boRecalcAbilitys := True;
              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaMaxHP].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaMaxHP].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaMaxHP].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaMaxHP].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 加最大蓝
            if ServerConfig.ProtectAddAttrib[aaMaxMP].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaMaxMP].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaMaxMP].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaMaxMP].HighValue > 0) //
              and (ServerConfig.ProtectAddAttrib[aaMaxMP].Time > 0) then
            begin
              if not ServerConfig.ProtectAddAttrib[aaMaxMP].HighValueIsPoint then
                V2 := Round((MonObj.m_WAbil.MaxMP - MonObj.m_AddNewExtValues[anet_AddMaxMP]) / 100 * ServerConfig.ProtectAddAttrib
                  [aaMaxMP].HighValue)
              else
                V2 := ServerConfig.ProtectAddAttrib[aaMaxMP].HighValue;

              MonObj.m_AddNewExtTicks[anet_AddMaxMP] := MyGetTickCount + LongWord(ServerConfig.ProtectAddAttrib[aaMaxMP].Time) *
                1000;
              MonObj.m_AddNewExtValues[anet_AddMaxMP] := Max(MonObj.m_AddNewExtValues[anet_AddMaxMP], V2);
              boRecalcAbilitys := True;

              if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) //
                and ServerConfig.ProtectAddAttrib[aaMaxMP].ShowHint //
                and (Length(ServerConfig.ProtectAddAttrib[aaMaxMP].HintText) > 0) then
              begin
                S := StringReplace(ServerConfig.ProtectAddAttrib[aaMaxMP].HintText, '%Point', IntToStr(V2), [rfIgnoreCase]);
                S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddAttrib[aaMaxMP].Time), [rfIgnoreCase]);
                TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
              end;
            end;

            // 隐身 chongchong 2014-09-07
            if ServerConfig.ProtectAddAttrib[aaHide].IsChecked //
              and (ServerConfig.ProtectAddAttrib[aaHide].Rate > 0) //
              and (Random(100) < ServerConfig.ProtectAddAttrib[aaHide].Rate) //
              and (ServerConfig.ProtectAddAttrib[aaHide].Time > 0) then
            begin
              TempList := TList.Create;
              try
                MonObj.GetMapBaseObjects(MonObj.m_PEnvir, MonObj.m_nCurrX, MonObj.m_nCurrY, 9, TempList);
                for II := 0 to TempList.Count - 1 do
                begin
                  TempObj := TBaseObject(TempList.Items[II]);
                  if (TempObj <> nil) and (TempObj.m_btRaceServer >= RC_ANIMAL) and (TempObj.m_TargetCret = MonObj) then
                  begin
                    if (abs(TempObj.m_nCurrX - MonObj.m_nCurrX) > 1) //
                      or (abs(TempObj.m_nCurrY - MonObj.m_nCurrY) > 1) //
                      or (Random(2) = 0) then
                      TempObj.m_TargetCret := nil;
                  end;
                end;
              finally
                TempList.Free;
              end;

              MonObj.m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] := ServerConfig.ProtectAddAttrib[aaHide].Time * 1000;
              MonObj.m_nCharStatus := MonObj.GetCharStatus();
              MonObj.StatusChanged();
              MonObj.m_boHideMode := True;
              MonObj.m_boTransparent := True;
            end;

            // 自定义状态 chongchong 2014-09-07
            if ServerConfig.ProtectTargetStatus and (ServerConfig.ProtectTargetStatusTime > 0) then
            begin
              MonObj.SetCustomMagicStatus(UserMagic.btNewLevel, UserMagic.wMagIdx, ServerConfig.ProtectTargetStatusTime);
              MonObj.SendRefMsg(RM_EFFECTSTEP, 9991, UserMagic.wMagIdx, UserMagic.btNewLevel, 0, '', ServerConfig.ProtectTargetStatusDelay);
            end;

            if (MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
            begin
              SmartObject := TSmartObject(MonObj);
              for ElementsType := Low(TItemElementsType) to High(TItemElementsType) do
              begin
                NexExtType := TAddNewExtType(Integer(anet_AddBlastHit) + Integer(ElementsType));
                if ServerConfig.ProtectAddElements[ElementsType].IsChecked //
                  and (ServerConfig.ProtectAddElements[ElementsType].Rate > 0) //
                  and (Random(100) < ServerConfig.ProtectAddElements[ElementsType].Rate) //
                  and (ServerConfig.ProtectAddElements[ElementsType].Value > 0) //
                  and (ServerConfig.ProtectAddElements[ElementsType].Time > 0) then
                begin
                  if not ServerConfig.ProtectAddElements[ElementsType].ValueIsPoint then
                  begin
                    V1 := 0;
                    case NexExtType of
                      anet_AddBlastHit:
                        V1 := SmartObject.m_WAbil.NewValue[0]; // 爆击几率
                      anet_AddDamageAdd:
                        V1 := SmartObject.m_WAbil.NewValue[1]; // 伤害增加
                      anet_AddDamageDec:
                        V1 := SmartObject.m_WAbil.NewValue[2]; // 伤害减少
                      anet_AddSpellDamageDec:
                        V1 := SmartObject.m_WAbil.NewValue[3]; // 魔法防御
                      anet_AddCloseDefense:
                        V1 := SmartObject.m_WAbil.NewValue[4]; // 忽视防御
                      anet_AddDamageRebound:
                        V1 := SmartObject.m_WAbil.NewValue[5]; // 伤害反弹
                      anet_AddMonDropRate:
                        V1 := SmartObject.m_WAbil.NewValue[6]; // 怪物爆率
                      anet_AddMaxHPAdd:
                        V1 := SmartObject.m_WAbil.NewValue[7]; // 体力增加
                      anet_AddMaxMPAdd:
                        V1 := SmartObject.m_WAbil.NewValue[8]; // 魔力增加
                      anet_AddAngryValueTimAdd:
                        V1 := SmartObject.m_WAbil.NewValue[9]; // 怒气恢复
                      anet_AddGroupDamageAdd:
                        V1 := SmartObject.m_WAbil.NewValue[10]; // 合击攻击
                      anet_AddHuamDropRate:
                        V1 := SmartObject.m_WAbil.NewValue[11]; // 人物爆率
                      anet_AddUndropRate:
                        V1 := SmartObject.m_WAbil.NewValue[12]; // 防爆出率
                      anet_AddUnParalysis:
                        V1 := SmartObject.m_WAbil.NewValue[13]; // 防止麻痹
                      anet_AddUnMagicShield:
                        V1 := SmartObject.m_WAbil.NewValue[14]; // 防止护身
                      anet_AddUnRevival:
                        V1 := SmartObject.m_WAbil.NewValue[15]; // 防止复活
                      anet_AddUnPosion:
                        V1 := SmartObject.m_WAbil.NewValue[16]; // 防止全毒
                      anet_AddUnTamming:
                        V1 := SmartObject.m_WAbil.NewValue[17]; // 防止诱惑
                      anet_AddUnFireCross:
                        V1 := SmartObject.m_WAbil.NewValue[18]; // 防止火墙
                      anet_AddUnFrozen:
                        V1 := SmartObject.m_WAbil.NewValue[19]; // 防止冰冻
                      anet_AddUnCobwebWinding:
                        V1 := SmartObject.m_WAbil.NewValue[20]; // 防止蛛网
                      anet_AddFatalBlowRate:
                        V1 := SmartObject.m_WAbil.NewValue[21]; // 致命一击几率
                      anet_AddFatalBlowPower:
                        V1 := SmartObject.m_WAbil.NewValue[22]; // 致命一击攻击
                      anet_AddFatalBlowDefense:
                        V1 := SmartObject.m_WAbil.NewValue[23]; // 致命一击防御
                      anet_AddUnBlastHit:
                        V1 := SmartObject.m_WAbil.NewValue[24]; // 暴击抗性
                    end;
                    V1 := Round((V1 - MonObj.m_AddNewExtValues[NexExtType]) / 100 * ServerConfig.ProtectAddElements[ElementsType].Value);
                  end
                  else
                    V1 := ServerConfig.ProtectAddElements[ElementsType].Value;

                  MonObj.m_AddNewExtTicks[NexExtType] := MyGetTickCount + LongWord(ServerConfig.ProtectAddElements[ElementsType].Time)
                    * 1000;
                  MonObj.m_AddNewExtValues[NexExtType] := Max(MonObj.m_AddNewExtValues[NexExtType], V1);
                  boRecalcAbilitys := True;

                  if ServerConfig.ProtectAddElements[ElementsType].ShowHint and (Length(ServerConfig.ProtectAddElements[ElementsType].HintText)
                    > 0) then
                  begin
                    S := StringReplace(ServerConfig.ProtectAddElements[ElementsType].HintText, '%Point', IntToStr(V1), [rfIgnoreCase]);
                    S := StringReplace(S, '%Time', IntToStr(ServerConfig.ProtectAddElements[ElementsType].Time), [rfIgnoreCase]);
                    TSmartObject(MonObj).SysMsg(S, g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
                  end;
                end;
              end;
            end;

            if boRecalcAbilitys then
            begin
              MonObj.RecalcAbilitys;
              if MonObj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
              begin
                MonObj.SendMsg(MonObj, RM_ABILITY, 0, 0, 0, 0, '');
                MonObj.SendMsg(MonObj, RM_SUBABILITY, 0, 0, 0, 0, '');
              end;
            end;
            TargetList.Add(MonObj)
          end;
        end;
      finally
        BaseObjectList.Free;
      end;

      SendTargetEffect := ClientConfig.Target_MultiPlay //
        and (ClientConfig.Target_File >= 0) //
        and (ClientConfig.Target_PlayCount > 0);

      // 持续播放目标效果 chongchong 2015-03-10
      nPower := 0;
      KeepTime := ClientConfig.Target_KeepTime * 1000 + nLevel * ClientConfig.Target_KeepTime2;
      if ClientConfig.Target_KeepPlay //
        and (ClientConfig.Target_File >= 0) //
        and (KeepTime > 0) //
        and (ClientConfig.Target_PlayCount > 0) then
      begin
        if (not ClientConfig.Target_KeepMultiPlay) or (ClientConfig.Target_KeepAttackRange = 0) then
        begin
          if PlayObject.m_PEnvir.GetEvent(nTargetX, nTargetY) = nil then
          begin
            EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, nTargetX, nTargetY, ET_CUSTOM_MAGIC_EFF, KeepTime, nPower,
              True, ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, ClientConfig.Target_KeepAttackRange,
              ClientConfig.Target_KeepAttackInterval, Additionals);
            g_EventManager.AddEvent(EffectEvent);
          end;
        end
        else
        begin
          nStartX := nTargetX - ClientConfig.Target_KeepAttackRange;
          nEndX := nTargetX + ClientConfig.Target_KeepAttackRange;
          nStartY := nTargetY - ClientConfig.Target_KeepAttackRange;
          nEndY := nTargetY + ClientConfig.Target_KeepAttackRange;
          for K := nStartX to nEndX do
          begin
            for M := nStartY to nEndY do
            begin
              if ServerConfig.DisableInSafeZone and PlayObject.InSafeZone(PlayObject.m_PEnvir, K, M) then
                Continue;

              if PlayObject.m_PEnvir.GetEvent(K, M) = nil then
              begin
                EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, K, M, ET_CUSTOM_MAGIC_EFF, KeepTime, nPower, True,
                  ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, 0, ClientConfig.Target_KeepAttackInterval,
                  Additionals);
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
          sMsg := sMsg + IntToStr(NativeInt(MonObj)) + ',';
          if ClientConfig.Target_KeepPlay and (KeepTime > 0) and (ClientConfig.Target_PlayCount > 0) then
          begin
            if (not ClientConfig.Target_KeepMultiPlay) or (ClientConfig.Target_KeepAttackRange = 0) then
            begin
              if PlayObject.m_PEnvir.GetEvent(MonObj.m_nCurrX, MonObj.m_nCurrY) = nil then
              begin
                EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, MonObj.m_nCurrX, MonObj.m_nCurrY, ET_CUSTOM_MAGIC_EFF,
                  KeepTime, nPower, False, ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, ClientConfig.Target_KeepAttackRange,
                  ClientConfig.Target_KeepAttackInterval, Additionals);
                g_EventManager.AddEvent(EffectEvent);
              end;
            end
            else
            begin
              nStartX := MonObj.m_nCurrX - ClientConfig.Target_KeepAttackRange;
              nEndX := MonObj.m_nCurrX + ClientConfig.Target_KeepAttackRange;
              nStartY := MonObj.m_nCurrY - ClientConfig.Target_KeepAttackRange;
              nEndY := MonObj.m_nCurrY + ClientConfig.Target_KeepAttackRange;
              for K := nStartX to nEndX do
              begin
                for M := nStartY to nEndY do
                begin
                  if ServerConfig.DisableInSafeZone and PlayObject.InSafeZone(PlayObject.m_PEnvir, K, M) then
                    Continue;

                  if PlayObject.m_PEnvir.GetEvent(K, M) = nil then
                  begin
                    EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, K, M, ET_CUSTOM_MAGIC_EFF, KeepTime, nPower, False,
                      ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, 0, ClientConfig.Target_KeepAttackInterval,
                      Additionals);
                    g_EventManager.AddEvent(EffectEvent);
                  end;
                end;
              end;
            end;
          end;
        end;
      end;
    finally
      TargetList.Free;
    end;
    Exit;
  end;

  bt06 := GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, nTargetX, nTargetY);
  IsTeleport := True;
  if (UserMagic.MagicInfo <> nil) and (UserMagic.MagicInfo.btEffectType <> 0) then
  begin
    if ServerConfig.AttackTeleportAttack and (not ServerConfig.AttackTeleportAfterDamage) then
    begin
      IsTeleport := False;
      // 禁锢不允许瞬移攻击 chongchong 2018-05-28
      if (not PlayObject.m_boImprison) and (Random(100) < ServerConfig.AttackTeleportRate) then
      begin
        if ServerConfig.AttackTeleportRunHum then
          Flag := Flag + [wf_Hum];
        if ServerConfig.AttackTeleportRunMon then
          Flag := Flag + [wf_Mon];
        if ServerConfig.AttackTeleportRunNpc then
          Flag := Flag + [wf_Npc];
        if ServerConfig.AttackTeleportRunGuard then
          Flag := Flag + [wf_Guard];
        if ServerConfig.AttackTeleportWarDisHumRun then
          Flag := Flag + [wf_War];
        if ServerConfig.AttackTeleportRunObstacle then
          Flag := Flag + [wf_Obstacle];
        if ServerConfig.AttackTeleportRush then
          PlayObject.m_PEnvir.GetNextPosition(PlayObject.m_nCurrX, PlayObject.m_nCurrY, bt06, ServerConfig.AttackTeleportRushCount,
            nTargetX, nTargetY);
        if PlayObject.MagCanMoveTarget(nTargetX, nTargetY, Flag, ServerConfig.AttackTeleportCannotRunItem, g_Config.nSendRefMsgRange)
          then
        begin
          boMoved := True;
          IsTeleport := True;
        end;
      end;
    end;
  end;

  if (not IsTeleport) and ServerConfig.IsNoTeleportNoAttack then
    Exit;

  if ((ServerConfig.AttackMode = mamNear) and (abs(PlayObject.m_nCurrX - nTargetX) <= ServerConfig.AttackNearRange) and (abs(PlayObject.m_nCurrY
    - nTargetY) <= ServerConfig.AttackNearRange)) //
    or ((ServerConfig.AttackMode = mamFar) and (abs(PlayObject.m_nCurrX - nTargetX) <= g_Config.nMagicAttackRage) and (abs(PlayObject.m_nCurrY
    - nTargetY) <= g_Config.nMagicAttackRage)) then
  begin
    case ServerConfig.AttackPowerCalc of
      mapcMC:
        nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
          Integer(PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1) + 1);
      mapcSC:
        nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.SC1,
          Integer(PlayObject.m_WAbil.SC2 - PlayObject.m_WAbil.SC1) + 1);
      mapcFromJob:
        begin
          if PlayObject.m_btJob = 1 then
            nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
              Integer(PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1) + 1)
          else if PlayObject.m_btJob = 2 then
            nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.SC1,
              Integer(PlayObject.m_WAbil.SC2 - PlayObject.m_WAbil.SC1) + 1)
          else
            nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.DC1,
              Integer(PlayObject.m_WAbil.DC2 - PlayObject.m_WAbil.DC1) + 1);
        end;
    else
      nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.DC1,
        Integer(PlayObject.m_WAbil.DC2 - PlayObject.m_WAbil.DC1) + 1);
    end;

    // 超过最高强化等级，以最高强化等级来算技能威力
    if UserMagic.btNewLevel >= MaxCustomMagicLevel then
    begin
      if ServerConfig.AttackPowerRates[MaxCustomMagicLevel] >= 0 then
      begin
        PowerRate := ServerConfig.AttackPowerRates[MaxCustomMagicLevel] + ServerConfig.AttackPowerRates[MaxCustomMagicLevel + 1] *
          (UserMagic.btNewLevel - MaxCustomMagicLevel);
        Int64Value := Round(Int64(nPower) / 100 * PowerRate);
        nPower := Min(Int64Value, High(Integer));
      end;
    end
    else if ServerConfig.AttackPowerRates[UserMagic.btNewLevel] >= 0 then
    begin
      Int64Value := Round(Int64(nPower) / 100 * ServerConfig.AttackPowerRates[UserMagic.btNewLevel]);
      nPower := Min(Int64Value, High(Integer));
    end;

    // 元素增加攻击伤害 移后 SingleAttack中，先减防御再算+攻击伤害 2020-04-28 23:59:59
    // nPower := PlayObject.NewAbilPower(1, nPower);
    IsHealthSpellChanged := False;
    PlayObject.m_btDirection := bt06;
    TargetList := TList.Create;
    try
      for I := Low(ServerConfig.Additionals) to High(ServerConfig.Additionals) do
      begin
        ServerConfig.Additionals[I].Rate := ServerConfig.Additionals[I].Rate + nLevel * ServerConfig.Additionals[I].Rate2;
        // 吸血/吸蓝
        if (I <> 5) or (I <> 6) then
        begin
          if ServerConfig.Additionals[I].TimeUnit <> 0 then
            ServerConfig.Additionals[I].Time := Round(nPower / 100 * ServerConfig.Additionals[I].Time);
          ServerConfig.Additionals[I].Time := ServerConfig.Additionals[I].Time + Round(ServerConfig.Additionals[I].Time / 100 *
            nLevel * ServerConfig.Additionals[I].Time2)
        end
        else
          ServerConfig.Additionals[I].Time := ServerConfig.Additionals[I].Time + nLevel * ServerConfig.Additionals[I].Time2;

        Additionals[I].Checked := ServerConfig.Additionals[I].Checked;
        Additionals[I].Rate := ServerConfig.Additionals[I].Rate;
        Additionals[I].Time := ServerConfig.Additionals[I].Time;
      end;

      if ServerConfig.AttackTargetStatusTimeUnit <> 0 then
        ServerConfig.AttackTargetStatusTime := Round(nPower / 100 * ServerConfig.AttackTargetStatusTime);
      ServerConfig.AttackTargetStatusTime := ServerConfig.AttackTargetStatusTime + Round(ServerConfig.AttackTargetStatusTime / 100
        * nLevel * ServerConfig.AttackTargetStatusTime2);

      if ServerConfig.AttackTarget = matSingle then
        IsHealthSpellChanged := SingleAttack(PlayObject, TargeTBaseObject, TargeTBaseObject, @ServerConfig, nPower, TargetList,
          UserMagic, MagicConfig.IsMagicWarr)
      else if ServerConfig.AttackTarget = matGroup then
        IsHealthSpellChanged := GroupAttack(PlayObject, TargeTBaseObject, nTargetX, nTargetY, @ServerConfig, nPower, TargetList,
          UserMagic, MagicConfig.IsMagicWarr)
      else if ServerConfig.AttackTarget = matLine then
        IsHealthSpellChanged := LineAttack(PlayObject, TargeTBaseObject, nTargetX, nTargetY, bt06, @ServerConfig, nPower,
          TargetList, UserMagic, MagicConfig.IsMagicWarr)
      else if ServerConfig.AttackTarget = matSwordWide then
        IsHealthSpellChanged := SwordWideAttack(PlayObject, TargeTBaseObject, bt06, @ServerConfig, nPower, TargetList, UserMagic,
          MagicConfig.IsMagicWarr)
      else if ServerConfig.AttackTarget = matDir8 then
        IsHealthSpellChanged := Dir8Attack(PlayObject, TargeTBaseObject, @ServerConfig, nPower, TargetList, UserMagic, MagicConfig.IsMagicWarr)
      else if ServerConfig.AttackTarget = matDir16 then
        IsHealthSpellChanged := Dir16Attack(PlayObject, TargeTBaseObject, @ServerConfig, nPower, TargetList, UserMagic,
          MagicConfig.IsMagicWarr);

      if (ServerConfig.Additionals[4].Checked) and (Random(100) < ServerConfig.Additionals[4].Rate) and (ServerConfig.Additionals[4].Time
        > 0) then
      begin
        DoPushed(PlayObject, UserMagic.wMagIdx, nTargetX, nTargetY, ServerConfig.Additionals[4].Time, ServerConfig.AdditionalHighLevel4,
          ServerConfig.AdditionalPushedType4);
      end;

      if IsHealthSpellChanged then
        PlayObject.HealthSpellChanged;

      KeepTime := ClientConfig.SelfKeep_KeepTime + nLevel * ClientConfig.SelfKeep_KeepTime2;
      if (ClientConfig.SelfKeep_File >= 0) and (ClientConfig.SelfKeep_PlayCount > 0) and (KeepTime > 0) then
      begin
        SelfKeepPlay.SelfKeep_File := ClientConfig.SelfKeep_File;
        SelfKeepPlay.SelfKeep_StartIndex := ClientConfig.SelfKeep_StartIndex;
        SelfKeepPlay.SelfKeep_PlayCount := ClientConfig.SelfKeep_PlayCount;
        SelfKeepPlay.SelfKeep_PlayTime := ClientConfig.SelfKeep_PlayTime;
        SelfKeepPlay.SelfKeep_DrawMode := ClientConfig.SelfKeep_DrawMode;
        SelfKeepPlay.SelfKeep_KeepTime := KeepTime;
        SelfKeepPlay.SelfKeep_StartIndex2 := -1;
        SelfKeepPlay.SelfKeep_DrawOrder := mdoPriorSelf;
        SelfKeepPlay.SelfKeep_DrawMode2 := mdmBlend;
        SetLength(S, SizeOf(SelfKeepPlay));
        Move(SelfKeepPlay, S[1], Length(S));
        PlayObject.m_MagicSelfPlayTick := MyGetTickCount;
        PlayObject.m_MagicSelfPlay := SelfKeepPlay;
        PlayObject.SendRefMsg(RM_CUSTOM_MAGIC_SELFKEEP_PLAY, Length(S), 0, 0, 0, S, 1000);
      end;

      // 攻击时召唤怪物 chongchong 2014-08-10 22:44:31
      if ServerConfig.EnabledCallMonster //
        and (ServerConfig.CallMonstersRate > 0) //
        and (Random(100) < ServerConfig.CallMonstersRate) //
        and (ServerConfig.CallMonstersRoyaltySec > 0) then
      begin
        for J := Low(ServerConfig.CallMonsters) to High(ServerConfig.CallMonsters) do
        begin
          if (Length(ServerConfig.CallMonsters[J]) > 0) and (ServerConfig.CallMonsterNums[J] > 0) then
          begin
            HaveBBCount := 0;
            for K := 0 to PlayObject.m_SlaveList.Count - 1 do
            begin
              MonObj := PlayObject.m_SlaveList.Items[K];
              if (MonObj.m_BBType = bb_Other) //
                and (not MonObj.m_boDeath) //
                and (not MonObj.m_boGhost) //
                and SameText(MonObj.m_sCharName, ServerConfig.CallMonsters[J]) then
                Inc(HaveBBCount);
            end;

            if HaveBBCount < ServerConfig.CallMonsterNums[J] then
            begin
              PlayObject.GetFrontPosition(nStartX, nStartY);
              PlayObject.MakeSlave(ServerConfig.CallMonsters[J], 3, ServerConfig.CallMonstersLevel, 100, ServerConfig.CallMonstersRoyaltySec
                * 60, bb_Other, 0, not g_Config.boBBAttrPlusAddOnlyMagic);
            end;
          end;
        end;
      end;

      if (TargeTBaseObject <> nil) or (TargetList.Count > 0) then
        Result := True;

      SendTargetEffect := ClientConfig.Target_MultiPlay and (ClientConfig.Target_File >= 0) and (ClientConfig.Target_PlayCount > 0);

      // 持续播放目标效果 chongchong 2015-03-10
      KeepTime := ClientConfig.Target_KeepTime * 1000 + nLevel * ClientConfig.Target_KeepTime2;
      if ClientConfig.Target_KeepPlay and (ClientConfig.Target_File >= 0) //
        and (KeepTime > 0) //
        and (ClientConfig.Target_PlayCount > 0) then
      begin
        if (not ClientConfig.Target_KeepMultiPlay) or (ClientConfig.Target_KeepAttackRange = 0) then
        begin
          GameEvent := TGameEvent(PlayObject.m_PEnvir.GetEvent(nTargetX, nTargetY));
          if (GameEvent = nil) //
            or (not (GameEvent is TCustomMagicEffectEvent)) //
            or ((GameEvent is TCustomMagicEffectEvent) and (TCustomMagicEffectEvent(GameEvent).MagicID <> UserMagic.wMagIdx)) then
          begin
            EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, nTargetX, nTargetY, ET_CUSTOM_MAGIC_EFF, KeepTime, nPower,
              True, ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, ClientConfig.Target_KeepAttackRange,
              ClientConfig.Target_KeepAttackInterval, Additionals);
            g_EventManager.AddEvent(EffectEvent);
          end;
        end
        else
        begin
          nStartX := nTargetX - ClientConfig.Target_KeepAttackRange;
          nEndX := nTargetX + ClientConfig.Target_KeepAttackRange;
          nStartY := nTargetY - ClientConfig.Target_KeepAttackRange;
          nEndY := nTargetY + ClientConfig.Target_KeepAttackRange;
          for K := nStartX to nEndX do
          begin
            for M := nStartY to nEndY do
            begin
              if ServerConfig.DisableInSafeZone and PlayObject.InSafeZone(PlayObject.m_PEnvir, K, M) then
                Continue;
              if PlayObject.m_PEnvir.GetEvent(K, M) = nil then
              begin
                EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, K, M, ET_CUSTOM_MAGIC_EFF, KeepTime, nPower, True,
                  ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, 0, ClientConfig.Target_KeepAttackInterval,
                  Additionals);
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
          sMsg := sMsg + IntToStr(NativeInt(MonObj)) + ',';
          if ClientConfig.Target_KeepPlay and (KeepTime > 0) and (ClientConfig.Target_PlayCount > 0) then
          begin
            if (not ClientConfig.Target_KeepMultiPlay) or (ClientConfig.Target_KeepAttackRange = 0) then
            begin
              if PlayObject.m_PEnvir.GetEvent(MonObj.m_nCurrX, MonObj.m_nCurrY) = nil then
              begin
                EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, MonObj.m_nCurrX, MonObj.m_nCurrY, ET_CUSTOM_MAGIC_EFF,
                  KeepTime, nPower, False, ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, ClientConfig.Target_KeepAttackRange,
                  ClientConfig.Target_KeepAttackInterval, Additionals);
                g_EventManager.AddEvent(EffectEvent);
              end;
            end
            else
            begin
              nStartX := MonObj.m_nCurrX - ClientConfig.Target_KeepAttackRange;
              nEndX := MonObj.m_nCurrX + ClientConfig.Target_KeepAttackRange;
              nStartY := MonObj.m_nCurrY - ClientConfig.Target_KeepAttackRange;
              nEndY := MonObj.m_nCurrY + ClientConfig.Target_KeepAttackRange;
              for K := nStartX to nEndX do
              begin
                for M := nStartY to nEndY do
                begin
                  if ServerConfig.DisableInSafeZone and PlayObject.InSafeZone(PlayObject.m_PEnvir, K, M) then
                    Continue;

                  if PlayObject.m_PEnvir.GetEvent(K, M) = nil then
                  begin
                    EffectEvent := TCustomMagicEffectEvent.Create(PlayObject, K, M, ET_CUSTOM_MAGIC_EFF, KeepTime, nPower, False,
                      ServerConfig.AdditionalHP0, UserMagic.wMagIdx, UserMagic.btNewLevel, 0, ClientConfig.Target_KeepAttackInterval,
                      Additionals);
                    g_EventManager.AddEvent(EffectEvent);
                  end;
                end;
              end;
            end;
          end;
        end;
      end;
    finally
      TargetList.Free;
    end;

    if (UserMagic.MagicInfo <> nil) and (UserMagic.MagicInfo.btEffectType <> 0) then
    begin
      if ServerConfig.AttackTeleportAttack and (ServerConfig.AttackTeleportAfterDamage) then
      begin
        IsTeleport := False;
        // 禁锢不允许瞬移攻击 chongchong 2018-05-28
        if (not PlayObject.m_boImprison) and (Random(100) < ServerConfig.AttackTeleportRate) then
        begin
          if ServerConfig.AttackTeleportRunHum then
            Flag := Flag + [wf_Hum];

          if ServerConfig.AttackTeleportRunMon then
            Flag := Flag + [wf_Mon];

          if ServerConfig.AttackTeleportRunNpc then
            Flag := Flag + [wf_Npc];

          if ServerConfig.AttackTeleportRunGuard then
            Flag := Flag + [wf_Guard];

          if ServerConfig.AttackTeleportWarDisHumRun then
            Flag := Flag + [wf_War];

          if ServerConfig.AttackTeleportRunObstacle then
            Flag := Flag + [wf_Obstacle];

          if ServerConfig.AttackTeleportRush then
            PlayObject.m_PEnvir.GetNextPosition(PlayObject.m_nCurrX, PlayObject.m_nCurrY, bt06, ServerConfig.AttackTeleportRushCount,
              nTargetX, nTargetY);

          if PlayObject.MagCanMoveTarget(nTargetX, nTargetY, Flag, ServerConfig.AttackTeleportCannotRunItem, g_Config.nSendRefMsgRange)
            then
          begin
            boMoved := True;
            IsTeleport := True;
          end;
        end;
      end;
    end;
  end;
end;

{ 复活术 }
function TMagicManager.MagMakeLivePlayObject(PlayObject: TSmartObject; UserMagic: pTUserMagic; TargeTBaseObject: TBaseObject):
  Boolean;
begin
  Result := False;
  if PlayObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
  begin
    if PlayObject.IsProperTargetSKILL_57(TargeTBaseObject) then
    begin
      if (Random(10 + UserMagic.btLevel) + UserMagic.btLevel) >= 8 then
      begin
        if (PlayObject.m_PEnvir <> nil) and ((PlayObject.m_PEnvir.m_nRevivalMaxCount < 0) or (TSmartObject(PlayObject).m_nRevivalCount
          < PlayObject.m_PEnvir.m_nRevivalMaxCount)) then
        begin
          if TSmartObject(PlayObject).m_PEnvir.m_nRevivalMaxCount >= 0 then
            Inc(TSmartObject(PlayObject).m_nRevivalCount);
          TPlayObject(TargeTBaseObject).ReAlive;
          TPlayObject(TargeTBaseObject).SendMsg(TPlayObject(TargeTBaseObject), RM_ABILITY, 0, 0, 0, 0, '');
          Result := True;
        end;
      end;
    end;
  end;
end;

{ 擒龙手 }
function TMagicManager.MagMakeArrestObject(PlayObject: TSmartObject; UserMagic: pTUserMagic; TargeTBaseObject: TBaseObject):
  Boolean;
var
  nX, nY: Integer;
begin
  Randomize;
  Result := True;
  begin
    if PlayObject.IsProperTargetSKILL_71(PlayObject.m_WAbil.Level, TargeTBaseObject) then
    begin
      if (Random(10 + UserMagic.btLevel) + UserMagic.btLevel) >= 5 then
      begin
        PlayObject.GetFrontPosition(nX, nY);
        TargeTBaseObject.SpaceMove(TargeTBaseObject.m_PEnvir.sMapName, nX, nY, 0);
        // 去掉擒龙手挖的动作 piaoyun 2013-07-25
        Result := True;
      end;
    end;
  end
end;

{ 移行换位 }
function TMagicManager.MagChangePosition(PlayObject: TSmartObject; nTargetX, nTargetY: Integer): Boolean;
var
  ItemObject: TItemObject;
begin
  Result := True;
  PlayObject.m_SkillUseTick[72] := MyGetTickCount;
  if not PlayObject.m_boOnHorse then
  begin
    if g_Config.boSkill72DisableStopItem then
    begin
      ItemObject := PlayObject.m_PEnvir.GetItem(nTargetX, nTargetY);
      if (ItemObject <> nil) and (not ItemObject.m_boGhost) then
      begin
        Exit;
      end;
    end;
    if PlayObject.m_PEnvir.CanWalkEx2(PlayObject, nTargetX, nTargetY, False) then
    begin
      PlayObject.SendRefMsg(RM_SPACEMOVE_FIRE2, 0, 0, 0, 0, '');
      PlayObject.SpaceMove(PlayObject.m_sMapName, nTargetX, nTargetY, 0);
    end;
  end;
end;

{ 破魂斩 }
function TMagicManager.MagMakeSkillFire_60(BaseObject: TSmartObject; UserMagic: pTUserMagic; TargeTBaseObject: TBaseObject):
  Boolean;
begin
  Result := False;
end;

// 劈星斩
function TMagicManager.MagMakeSkillFire_61(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower, nPower2: Integer;
  nX, nY: Integer;
  Obj: TBaseObject;
  SmartObject: TSmartObject;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  Result := False;
  if BaseObject.m_Master = nil then
    Exit;
  nPower := 0;
  if BaseObject.IsProperTarget(TargeTBaseObject) then
  begin
    if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
    begin
      if (TargeTBaseObject.m_nAntiMagic <= Random(10)) then
      begin
        with BaseObject do
        begin
          case m_btJob of
            2:
              nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2
                - m_WAbil.SC1, 1));
            0:
              nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.DC1, Max(m_WAbil.DC2
                - m_WAbil.DC1, 1));
          end;
        end;
        with BaseObject.m_Master do
        begin
          case m_btJob of
            2:
              nPower := nPower + GetAttackPower(GetPower(BaseObject.m_Master, MPow(BaseObject.m_Master, UserMagic), UserMagic) +
                m_WAbil.SC1, Max(m_WAbil.SC2 - m_WAbil.SC1, 1));
            0:
              nPower := nPower + GetAttackPower(GetPower(BaseObject.m_Master, MPow(BaseObject.m_Master, UserMagic), UserMagic) +
                m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1));
          end;
        end;
        nPower := Min(LongWord(nPower + Round(nPower * ((UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkillJointAttackLevelRate
          / 100))), High(Integer));
        nPower := nPower + Round(nPower / 100 * (BaseObject.m_WAbil.NewValue[10] + BaseObject.m_Master.m_WAbil.NewValue[10]));
        // 劈星斩改为群体攻击 chongchong 2013-10-09
        for nX := nTargetX - 3 to nTargetX + 3 do
        begin
          for nY := nTargetY - 3 to nTargetY + 3 do
          begin
            if abs(nX - nTargetX) = abs(nY - nTargetY) then
            begin
              Obj := TBaseObject(BaseObject.m_PEnvir.GetMovingObject(nX, nY, True));
              if (Obj <> nil) and BaseObject.IsProperTarget(Obj) then
              begin
                nPower2 := nPower;
                IsHuman := Obj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
                if not IsHuman then
                begin
                  _Master := Obj.Master;
                  if (_Master <> nil) then
                  begin
                    IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
                  end;
                end;
                if IsHuman then
                begin
                  nPower2 := Min(LongWord(Round(nPower2 * (g_Config.nSkill61AttackHumPowerRate / 100))), High(Integer));
                end
                else
                begin
                  nPower2 := Min(LongWord(Round(nPower2 * (g_Config.nSkill61PowerRate / 100))), High(Integer));
                end;
                nPower2 := Obj.GetNextDamage(nPower2);
                // 伤害吸收 chongchong 2016-03-18
                if Obj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                begin
                  SmartObject := TSmartObject(Obj);
                  if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
                  begin
                    nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
                    SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
                    SmartObject.RefAbilNH;
                  end;
                  {
                    // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
                    if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
                    begin // 吸收伤害
                    if Random(100) < SmartObject.m_nSuckDamageProbability then
                    begin
                    nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
                    if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
                    nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
                    Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
                    nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
                    end;
                    end;
                  }
                end;
                if (Obj.m_btRaceServer in [55]) then
                begin
                  if FNpcReleaseMagic <> 0 then
                  begin
                    Obj.SendMsg(Obj, RM_STRUCK, nPower2, Obj.m_WAbil.HP, Obj.m_WAbil.MaxHP, NativeInt(BaseObject), '0');
                  end
                  else
                  begin
                    Obj.SendDelayMsg(Obj, RM_STRUCK, nPower2, Obj.m_WAbil.HP, Obj.m_WAbil.MaxHP, NativeInt(BaseObject), '0', 600);
                  end;
                end
                else
                begin
                  if FNpcReleaseMagic <> 0 then
                    BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(Obj.m_nCurrX, Obj.m_nCurrY), MakeLong(2, 0),
                      NativeInt(Obj), IntToStr(UserMagic.wMagIdx))
                  else
                    BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(Obj.m_nCurrX, Obj.m_nCurrY), MakeLong(2,
                      0), NativeInt(Obj), IntToStr(UserMagic.wMagIdx), 600);
                end;
                if (Obj.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(Obj).m_boSuperShiled and
                  g_Config.UseSkillCloseSuperShileds[4] then // 护体神盾被击破
                  TSmartObject(Obj).CloseSuperShiled;
              end;
            end;
          end;
        end;
      end
      else
      begin
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        begin
          BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
        end;
      end;
    end;
    Result := True;
  end;
end;

// 雷霆一击
function TMagicManager.MagMakeSkillFire_62(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  SmartObject: TSmartObject;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  Result := False;
  if BaseObject.m_Master = nil then
    Exit;
  nPower := 0;
  if BaseObject.IsProperTarget(TargeTBaseObject) then
  begin
    with BaseObject do
    begin
      case m_btJob of
        1:
          nPower := nPower + GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2
            - m_WAbil.MC1, 1));
        0:
          nPower := nPower + GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.DC1, Max(m_WAbil.DC2
            - m_WAbil.DC1, 1));
      end;
    end;
    with BaseObject.m_Master do
    begin
      case m_btJob of
        1:
          nPower := nPower + GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2
            - m_WAbil.MC1, 1));
        0:
          nPower := nPower + GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.DC1, Max(m_WAbil.DC2
            - m_WAbil.DC1, 1));
      end;
    end;
    nPower := Min(LongWord(nPower + Round(nPower * ((UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkillJointAttackLevelRate
      / 100))), High(Integer));
    nPower := nPower + Round(nPower / 100 * (BaseObject.m_WAbil.NewValue[10] + BaseObject.m_Master.m_WAbil.NewValue[10]));
    if (not g_Config.boSkill62NotMagBubbleDefence { 忽视魔法盾 } ) { and (not CanCloseDefense) } then
      nPower := TargeTBaseObject.GetHitStruckDamage(BaseObject, nPower, nil, 2)
    else
      nPower := nPower;
    IsHuman := TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
    if not IsHuman then
    begin
      _Master := TargeTBaseObject.Master;
      if (_Master <> nil) then
      begin
        IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
      end;
    end;
    if IsHuman then
    begin
      nPower := Min(LongWord(Round(nPower * (g_Config.nSkill62AttackHumPowerRate / 100))), High(Integer));
    end
    else
    begin
      nPower := Min(LongWord(Round(nPower * (g_Config.nSkill62PowerRate / 100))), High(Integer));
    end;
    // 伤害吸收 chongchong 2016-03-18
    if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    begin
      SmartObject := TSmartObject(TargeTBaseObject);
      if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
      begin
        nPower := Max(0, nPower - SmartObject.GetNGDecPower);
        SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
        SmartObject.RefAbilNH;
      end;
      {
        // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
      }
    end;
    if FNpcReleaseMagic <> 0 then
      BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), MakeLong(2, 0), NativeInt(TargeTBaseObject),
        IntToStr(UserMagic.wMagIdx))
    else
      BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), MakeLong(2, 0), NativeInt(TargeTBaseObject),
        IntToStr(UserMagic.wMagIdx), 600);
    if (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(TargeTBaseObject).m_boSuperShiled
      and g_Config.UseSkillCloseSuperShileds[4] then // 护体神盾被击破
      TSmartObject(TargeTBaseObject).CloseSuperShiled;
    // end;
    Result := True;
  end;
end;

// 噬魂沼泽
function TMagicManager.MagMakeSkillFire_63(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeBaseObject: TBaseObject;
  nPower, nOldPower, nTime, nTemp: Integer;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  Result := False;
  if BaseObject.m_Master = nil then
    Exit;
  with BaseObject do
    nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2 - m_WAbil.SC1,
      1));
  with BaseObject.m_Master do
    nPower := Min(LongWord(nPower + GetAttackPower(GetPower(BaseObject.m_Master, MPow(BaseObject.m_Master, UserMagic), UserMagic)
      + m_WAbil.SC1, Max(m_WAbil.SC2 - m_WAbil.SC1, 1))), High(Integer));
  nPower := Min(LongWord(nPower + Round(nPower * ((UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkillJointAttackLevelRate
    / 100))), High(Integer));
  nPower := nPower + Round(nPower / 100 * (BaseObject.m_WAbil.NewValue[10] + BaseObject.m_Master.m_WAbil.NewValue[10]));
  nOldPower := nPower;
  BaseObjectList := TList.Create;
  { TODO -ochongchong -c新增 : 合击技能 噬魂沼泽攻击范围 【2013-08-11】 }
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, g_Config.nSkill63PowerRange, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if TargeBaseObject.m_boDeath or (TargeBaseObject.m_boGhost) or (TargeBaseObject = BaseObject) then
      Continue;
    if BaseObject.IsProperTarget(TargeBaseObject) then
    begin
      nPower := nOldPower;
      IsHuman := TargeBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
      if not IsHuman then
      begin
        _Master := TargeBaseObject.Master;
        if (_Master <> nil) then
        begin
          IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
        end;
      end;
      if IsHuman then
      begin
        nPower := Min(LongWord(Round(nPower * (g_Config.nSkill63AttackHumPowerRate / 100))), High(Integer));
      end
      else
      begin
        nPower := Min(LongWord(Round(nPower * (g_Config.nSkill63PowerRate / 100))), High(Integer));
      end;
      // 魔法盾防御增加
      if (nPower > 0) and TargeBaseObject.m_boAbilMagBubbleDefence then
      begin
        nPower := nPower - Round(nPower / 100 * g_Config.nSkill113RateAddWithSkill63);
        if nPower < 0 then
          nPower := 0;
      end;
      // 计算目标敏捷
      if g_Config.boSkill63UseSpeedPoint and (Random(TargeBaseObject.m_btSpeedPoint) >= BaseObject.m_btHitPoint) then
        nPower := 0;
      if nPower > 0 then
      begin
        TargeBaseObject.SendMsg(BaseObject, RM_MAGSTRUCK, 0, nPower, 0, 63, '');
        // 修复噬魂沼泽群体中毒 chongchong 2013-10-10
        if g_Config.boSkill63GreenPoison then
        begin
          nTemp := GetPower13(BaseObject, 40, UserMagic) + GetRPow(BaseObject, BaseObject.m_WAbil.SC1, BaseObject.m_WAbil.SC2) * 2;
          nTime := Min(Round(nTemp * (g_Config.nAmyOunsulTimeRate / 100)), g_Config.nAmyOunsulMaxTime);
          TargeBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , nTime, NativeInt(BaseObject),
            GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nTemp { nPower } / g_Config.nAmyOunsulPoint)), UserMagic)
            { UserMagic.btLevel } , '', 1000);
        end;
        TargeBaseObject.SetLastHiter(BaseObject);
        TargeBaseObject.SetTargetCreat(BaseObject);
        if (TargeBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(TargeBaseObject).m_boSuperShiled
          and g_Config.UseSkillCloseSuperShileds[4] then // 护体神盾被击破
          TSmartObject(TargeBaseObject).CloseSuperShiled;
      end;
      Result := True;
      // end;
    end;
  end;
  BaseObjectList.Free;
end;

{ TODO -ochongchong -c修改 : 合击技能  末日审判由单体攻击改为范围攻击 【2013-08-11】 }
function TMagicManager.MagMakeSkillFire_64(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Boolean;
var
  I, nPower, nBasePower: Integer;
  nTime: Integer;
  TargetObjects: TList;
  TargetObject: TBaseObject;
  SmartObject: TSmartObject;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  Result := False;
  if BaseObject.m_Master = nil then
    Exit;
  if BaseObject.m_TargetCret = nil then
    Exit;
  nPower := 0;
  with BaseObject do
  begin
    case m_btJob of
      1:
        nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 -
          m_WAbil.MC1, 1));
      2:
        nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2 -
          m_WAbil.SC1, 1));
    end;
  end;
  with BaseObject.m_Master do
  begin
    case m_btJob of
      1:
        nPower := Min(LongWord(nPower + GetAttackPower(GetPower(BaseObject.m_Master, MPow(BaseObject.m_Master, UserMagic),
          UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 - m_WAbil.MC1, 1))), High(Integer));
      2:
        nPower := Min(LongWord(nPower + GetAttackPower(GetPower(BaseObject.m_Master, MPow(BaseObject.m_Master, UserMagic),
          UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2 - m_WAbil.SC1, 1))), High(Integer));
    end;
  end;
  nPower := Min(LongWord(nPower + Round(nPower * ((UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkillJointAttackLevelRate
    / 100))), High(Integer));
  nPower := nPower + Round(nPower / 100 * (BaseObject.m_WAbil.NewValue[10] + BaseObject.m_Master.m_WAbil.NewValue[10]));
  nBasePower := nPower;
  if not BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
    Exit;
  TargetObjects := TList.Create;
  try
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, BaseObject.m_TargetCret.m_nCurrX, BaseObject.m_TargetCret.m_nCurrY, g_Config.nSkill64PowerRange
      { 攻击范围 } , TargetObjects);
    for I := 0 to TargetObjects.Count - 1 do
    begin
      nPower := nBasePower;
      TargetObject := TBaseObject(TargetObjects.Items[I]);
      if TargetObject = nil then
        Continue;
      if not BaseObject.IsProperTarget(TargetObject) then
        Continue;
      { 不打自己或自己的主人 }
      if (TargetObject = BaseObject) or (TargetObject.m_Master = BaseObject.m_Master) then
        Continue;
      nTime := (3 * UserMagic.btLevel + 1) * 2;
      IsHuman := TargetObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
      if not IsHuman then
      begin
        _Master := TargetObject.Master;
        if (_Master <> nil) then
        begin
          IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
        end;
      end;
      if IsHuman then
      begin
        nPower := Min(LongWord(Round(nPower * (g_Config.nSkill64AttackHumPowerRate / 100))), High(Integer));
      end
      else
      begin
        nPower := Min(LongWord(Round(nPower * (g_Config.nSkill64PowerRate / 100))), High(Integer));
      end;
      // 吸收伤害
      if TargetObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargetObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
        }
      end;
      if FNpcReleaseMagic <> 0 then
        BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), MakeLong(g_Config.nSkill64PowerRange,
          0), NativeInt(TargetObject), IntToStr(UserMagic.wMagIdx))
      else
        BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), MakeLong(g_Config.nSkill64PowerRange,
          0), NativeInt(TargetObject), IntToStr(UserMagic.wMagIdx), 600);
      if g_Config.boSkill64MakeStone and TargetObject.CanStone then
      begin
        TargetObject.MakePosion(POISON_STONE, nTime, 0);
        TargetObject.m_boFastParalysis := True;
      end;
      if (TargetObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(TargetObject).m_boSuperShiled
        and g_Config.UseSkillCloseSuperShileds[4] then // 护体神盾被击破
        TSmartObject(TargetObject).CloseSuperShiled;
      Result := True;
    end;
  finally
    TargetObjects.Free;
  end;
end;

// 火龙气焰
function TMagicManager.MagMakeSkillFire_65(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPower, nOldPower: Integer;
  IsHuman: Boolean;
  _Master: TBaseObject;
begin
  Result := False;
  if BaseObject.m_Master = nil then
    Exit;
  with BaseObject do
    nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 - m_WAbil.MC1,
      1));
  with BaseObject.m_Master do
    nPower := Min(LongWord(nPower + GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max
      (m_WAbil.MC2 - m_WAbil.MC1, 1))), High(Integer));
  nPower := Min(LongWord(nPower + Round(nPower * ((UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkillJointAttackLevelRate
    / 100))), High(Integer));
  nPower := nPower + Round(nPower / 100 * (BaseObject.m_WAbil.NewValue[10] + BaseObject.m_Master.m_WAbil.NewValue[10]));
  nOldPower := nPower;
  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, g_Config.nSkill65PowerRange, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if (TargeTBaseObject <> nil) and (not TargeTBaseObject.m_boDeath) and (not TargeTBaseObject.m_boGhost) and BaseObject.IsProperTarget
      (TargeTBaseObject) then
    begin
      nPower := nOldPower;
      IsHuman := TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
      if not IsHuman then
      begin
        _Master := TargeTBaseObject.Master;
        if (_Master <> nil) then
        begin
          IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER];
        end;
      end;
      if IsHuman then
      begin
        nPower := Min(LongWord(Round(nPower * (g_Config.nSkill65AttackHumPowerRate / 100))), High(Integer));
      end
      else
      begin
        nPower := Min(LongWord(Round(nPower * (g_Config.nSkill65PowerRate / 100))), High(Integer));
      end;
      TargeTBaseObject.SendMsg(BaseObject, RM_MAGSTRUCK, 0, nPower, 0, 65, '');
      if (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(TargeTBaseObject).m_boSuperShiled
        and g_Config.UseSkillCloseSuperShileds[4] then // 护体神盾被击破
        TSmartObject(TargeTBaseObject).CloseSuperShiled;
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

function TMagicManager.MagMeteoriteRain(BaseObject: TSmartObject; UserMagic: pTUserMagic; nX, nY: Integer): Boolean;
// 流星火雨
var
  I, nRage, nPower, nValue, nPowerNG: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  SmartObject: TSmartObject;
begin
  Result := False;
  nRage := Min(g_Config.nSkill58AttackRange + UserMagic.btLevel, g_Config.nSkill58AttackRange);
  nValue := BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + BaseObject.m_WAbil.MC1,
    Integer(BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1) + 1);
  nValue := Min(LongWord(Round(nValue * (g_Config.nSkill58PowerRate / 100))), High(Integer));
  nValue := GetNewLevelPower(nValue, UserMagic); // 取强化技能攻击伤害
  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if (TargeTBaseObject <> nil) and (not TargeTBaseObject.m_boDeath) and (not TargeTBaseObject.m_boGhost) and BaseObject.IsProperTarget
      (TargeTBaseObject) then
    begin
      if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then
      begin
        nPower := nValue;
        if TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD then
          nPower := Min(LongWord(Round(nPower * 1.5)), High(Integer));
        // 伤害吸收 chongchong 2016-03-18
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower := Max(0, nPower - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
          }
        end;
        nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
          GetSkillAttackPowerNG(BaseObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
        if g_Config.boSkill58PowerTwoAttack then
        begin
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPowerNG, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPowerNG, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 800);
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 1200);
        end
        else
        begin
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 800 + 800);
        end;
        Result := True;
      end
      else
      begin
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        begin
          BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
        end;
      end;
    end;
  end;
  BaseObjectList.Free;
end;

function TMagicManager.MagMakeCopySelf(PlayObject: TSmartObject; nTargetX, nTargetY: Integer; var TargeTBaseObject: TBaseObject;
  MagicLevel, MagicNewLevel: Byte): Boolean;
var
  sName: string;
begin
  sName := PlayObject.m_sCharName;
  TargeTBaseObject := PlayObject.MakeCopySelf(sName, nTargetX, nTargetY, g_Config.nCopySelfMaxCount, g_Config.nCopySelfExistTime +
    (MagicLevel + MagicNewLevel) * g_Config.nCopySelfLevelUpAddExistTime);
  Result := TargeTBaseObject <> nil;
end;

function TMagicManager.IsWarrSkill(wMagIdx: Integer): Boolean; // 是否是战士技能
begin
  Result := wMagIdx in [SKILL_ONESWORD { 3 }, SKILL_ILKWANG { 4 }, SKILL_YEDO { 7 }, SKILL_ERGUM { 12 }, SKILL_BANWOL { 25 },
    SKILL_FIRESWORD { 26 }, SKILL_MOOTEBO { 27 }, SKILL_40 { 40 }, SKILL_42 { 42 }, SKILL_43 { 43 }, SKILL_56 { 56 }, SKILL_60
    { 60 }, SKILL_66,
  { SKILL_75, } SKILL_100..SKILL_103, SKILL_113 { 断空斩 } ]; // , {66} SKILL_75 {护体神盾}
end;

function TMagicManager.CheckCD(BaseObject: TSmartObject; SkillID: LongWord; CdTime: LongWord; boDoNotCheck: Boolean): Boolean;
var
  t: LongWord;
  sMsg: string;
  V: Integer;
begin
  Result := False;
  if boDoNotCheck then
  begin
    Result := True;
    Exit;
  end;

  t := MyGetTickCount;
  if { (SkillID >= Low(BaseObject.m_SkillUseTick)) and } (SkillID <= High(BaseObject.m_SkillUseTick)) then
  begin // HZQ
    if t - BaseObject.m_SkillUseTick[SkillID] >= CdTime then
    begin
      BaseObject.m_SkillUseTick[SkillID] := t;
      Result := True;
    end
    else
    begin
      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (not BaseObject.m_boDummyObject) and (Length(sSkillCDHit) > 0) then
      begin
        V := Max(1, (CdTime - (t - BaseObject.m_SkillUseTick[SkillID])) div 1000);
        sMsg := StringReplace(sSkillCDHit, '%d', IntToStr(V), []);
        BaseObject.SendMsg(BaseObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sMsg);
      end;
      Result := False;
    end;
  end;
end;

function TMagicManager.DoSpell(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
  TBaseObject; FormClient: Boolean; btNpcReleaseMagic: Byte; boDoNotCheck: Boolean): Boolean;
var
  boTrain: Boolean;
  boSpellFail: Boolean;
  boSpellFire: Boolean;
  nPower, nSec: Integer;
  nAmuletIdx: Integer;
  nX: Integer;
  nY: Integer;
  nType: Integer;
  btItemType: Integer;
  nMagicAttackRage: Integer;
  // nSkillWaitTime: Integer;
  boMove: Boolean;
  nIndex: Integer;
  I, nDir: Integer;
  dwTempCDTime, dwTempTime: LongWord;
  sMsg: string;
  boCustomMagicMove: Boolean;
  Int64Value1, Int64Value2: Int64;
  nTime, nRange: Integer;
  DefMsg: TDefaultMessage;
  sLevel: string;
  nTemp: Integer;
  boUse: Boolean;
  SmartObject: TSmartObject;

  procedure GetCopySelfPos(var nCurrX, nCurrY: Integer);
  // 使用分身术时，获取分身的坐标
  var
    I, J, II, nX, nY: Integer;
  begin
    if UserMagic.wMagIdx = SKILL_74 then
    begin
      if BaseObject.m_btRaceServer = RC_HEROOBJECT then
      begin
        for J := 2 to 4 do
        begin
          for I := 2 downto J do
          begin
            if BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, BaseObject.m_btDirection, I, nX, nY)
              and BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
            begin
              BaseObject.m_btDirection := GetNextDirection(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nX, nY);
              nCurrX := nX;
              nCurrY := nY;
              Exit;
            end;
          end;

          for I := 2 downto J do
          begin
            for II := DR_UP to DR_UPLEFT do
            begin
              if II <> BaseObject.m_btDirection then
              begin
                if BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, II, I, nX, nY) //
                  and BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
                begin
                  BaseObject.m_btDirection := GetNextDirection(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nX, nY);
                  nCurrX := nX;
                  nCurrY := nY;
                  Exit;
                end;
              end;
            end;
          end;
        end;
      end;

      for I := 5 downto 1 do
      begin
        if BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, BaseObject.m_btDirection, I, nX, nY) //
          and BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
        begin
          BaseObject.m_btDirection := GetNextDirection(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nX, nY);
          nCurrX := nX;
          nCurrY := nY;
          Exit;
        end;
      end;

      for I := 5 downto 1 do
      begin
        for II := DR_UP to DR_UPLEFT do
        begin
          if (II <> BaseObject.m_btDirection) //
            and BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, II, I, nX, nY) //
            and BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
          begin
            BaseObject.m_btDirection := GetNextDirection(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nX, nY);
            nCurrX := nX;
            nCurrY := nY;
            Exit;
          end;
        end;
      end;
    end;
  end;

begin
  Result := False;
  boMove := False;
  boCustomMagicMove := False;
  if IsWarrSkill(UserMagic.wMagIdx) then
    Exit;

  FNpcReleaseMagic := btNpcReleaseMagic;
  if g_Config.boViewRangeCanMagicAttack then
    nMagicAttackRage := BaseObject.m_nViewRange
  else
    nMagicAttackRage := g_Config.nMagicAttackRage;

  if UserMagic.wMagIdx = SKILL_204 then // 十步一杀
    nMagicAttackRage := Max(nMagicAttackRage, g_Config.nSkill204Distance);

  if (UserMagic.wMagIdx = SKILL_HEALLING) and (TargeTBaseObject = nil) then
  begin
    TargeTBaseObject := BaseObject;
    nTargetX := BaseObject.m_nCurrX;
    nTargetY := BaseObject.m_nCurrY;
  end;

  if (abs(BaseObject.m_nCurrX - nTargetX) > nMagicAttackRage) or (abs(BaseObject.m_nCurrY - nTargetY) > nMagicAttackRage) then
  begin
    { TODO -ochongchong -c新增 : 超出魔法攻击范围提示 【2013-08-16】 }
    if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) //
      and (not (UserMagic.wMagIdx in [ //
      SKILL_41 { 狮子吼 }
      , SKILL_75 { 护体神盾 }
      , SKILL_SHIELD { 魔法盾 }
      , SKILL_FIREWIND { 拒火环 }
      , SKILL_48 { 气功波 }
      , SKILL_LIGHTFLOWER { 地狱雷光 }
      , SKILL_50 { 无极真气 }, SKILL_SKELLETON { 召唤骷髅 }
      , SKILL_SINSU { 召唤神兽 }
      , SKILL_76 { 召唤圣兽 }
      , SKILL_55 { 召唤月灵 }
      , SKILL_114 { 倚天劈地 }
      , SKILL_CLOAK { 隐身术 }
      , SKILL_73 { 道力盾 }
      , SKILL_87 { 武力盾 }
      , SKILL_88 { 新武力盾 }
      , SKILL_89 { 新道力盾 } ])) then
    begin
      if (g_Config.boShowMsgMagicRangeExceed) then
        BaseObject.SendScreenMsg(g_MagicAttackOutRange, 220, 0, 30, 40);

      Exit;
    end;
  end;

  if btNpcReleaseMagic = 0 then
    btItemType := CheckMagLighteningType(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject)
  else
    btItemType := 0;

  // 修正分身术分身出现位置是不是不受鼠标指向坐标的控制 只出现在施法最远距离 chongchong 2015-03-04
  if not FormClient then
    GetCopySelfPos(nTargetX, nTargetY);

  if (btNpcReleaseMagic <> 2) then
  begin
    // 4级技能强化 -- 4级灵魂火符 4级灭天火 chongchong 2013-12-04
    if (UserMagic.wMagIdx = SKILL_FIRECHARM) { 灵魂火符 }
      or (UserMagic.wMagIdx = SKILL_45) { 灭天火 }
      or (UserMagic.wMagIdx = SKILL_SHIELD) { 魔法盾 }
      or (UserMagic.wMagIdx = SKILL_73) { 道力盾 }
      or (UserMagic.wMagIdx = SKILL_87) { 武力盾 }
      or (UserMagic.wMagIdx = SKILL_88) { 新武力盾 }
      or (UserMagic.wMagIdx = SKILL_89) { 新道力盾 } then
    begin
      sLevel := IntToStr(UserMagic.btLevel);
      if UserMagic.MagicInfo <> nil then
        BaseObject.SendRefMsg(RM_SPELL, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, MakeLong(UserMagic.MagicInfo.wMagicId,
          UserMagic.btNewLevel * 2 + btItemType), sLevel);
    end
    else
    begin
      sLevel := '';
      if UserMagic.MagicInfo <> nil then
        BaseObject.SendRefMsg(RM_SPELL, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, MakeLong(UserMagic.MagicInfo.wMagicId,
          UserMagic.btNewLevel * 2 + btItemType), sLevel);
    end;
  end;

  if TargeTBaseObject <> nil then
    BaseObject.m_CurrTargetEx := TargeTBaseObject
  else
    BaseObject.m_CurrTargetEx := nil;

  if (btNpcReleaseMagic = 1) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (UserMagic.MagicInfo <> nil) then
  begin
    DefMsg := MakeDefaultMsg(SM_SPELL, NativeInt(BaseObject), nTargetX, nTargetY, UserMagic.MagicInfo.btEffect);
    TPlayObject(BaseObject).SendSocket(@DefMsg, IntToStr(MakeLong(UserMagic.MagicInfo.wMagicId, UserMagic.btNewLevel * 2 +
      btItemType)) + '|' + sLevel);
  end;

  if (TargeTBaseObject <> nil) //
    and (TargeTBaseObject.m_boDeath) //
    and (UserMagic.MagicInfo <> nil) //
    and (UserMagic.MagicInfo.wMagicId <> SKILL_57) //
    and (UserMagic.MagicInfo.wMagicId <> SKILL_91) //
    and (UserMagic.MagicInfo.wMagicId < 100) then
    TargeTBaseObject := nil;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    SmartObject := TSmartObject(BaseObject);
    SmartObject.m_nSpellMagicID := UserMagic.MagicInfo.wMagicId;
    SmartObject.m_sSpellMagicName := UserMagic.MagicInfo.sMagicName;
    if TargeTBaseObject <> nil then
    begin
      if TargeTBaseObject is TCopyMon then // 分身RaceServer检测改为151
        SmartObject.m_sSpellMagicTargetRace := 151
      else
        SmartObject.m_sSpellMagicTargetRace := TargeTBaseObject.m_btRaceServer;

      SmartObject.m_sSpellMagicTarget := TargeTBaseObject.m_sCharName;
    end
    else
    begin
      SmartObject.m_sSpellMagicTargetRace := 65535;
      SmartObject.m_sSpellMagicTarget := '';
    end;

    SmartObject.m_IsSpellStopMagic := False;
    if g_FunctionNPC <> nil then
    begin
      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        g_FunctionNPC.GotoLable(TPlayObject(SmartObject), '@BeginMagic', False)
      else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) //
        and (BaseObject.m_Master <> nil) //
        and (BaseObject.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
        g_FunctionNPC.GotoLable(TPlayObject(SmartObject.m_Master), '@H.BeginMagic', False)
    end;

    if SmartObject.m_IsSpellStopMagic then
    begin
      SmartObject.m_nSpellMagicID := 0;
      SmartObject.m_sSpellMagicName := '';
      SmartObject.m_sSpellMagicTargetRace := 65535;
      SmartObject.m_sSpellMagicTarget := '';
      Exit;
    end;

    SmartObject.m_nSpellMagicID := 0;
    SmartObject.m_sSpellMagicName := '';
    SmartObject.m_sSpellMagicTargetRace := 65535;
    SmartObject.m_sSpellMagicTarget := '';
  end;

  if UserMagic.wMagIdx in [SKILL_61..SKILL_65] then
  begin
    if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    begin
      if BaseObject.m_Master <> nil then
      begin
        BaseObject.m_Master.m_btDirection := GetNextDirection(BaseObject.m_Master.m_nCurrX, BaseObject.m_Master.m_nCurrY, nTargetX,
          nTargetY);

        if UserMagic.wMagIdx = SKILL_61 then
        begin
          if BaseObject.m_Master.m_btJob = 0 then
            BaseObject.m_Master.AttackDir(TargeTBaseObject, 13, BaseObject.m_Master.m_btDirection);
        end
        else if UserMagic.wMagIdx = SKILL_62 then
        begin
          if BaseObject.m_Master.m_btJob = 0 then
            BaseObject.m_Master.AttackDir(TargeTBaseObject, 14, BaseObject.m_Master.m_btDirection);
        end
        else if (UserMagic.wMagIdx >= SKILL_63) and (UserMagic.wMagIdx <= SKILL_65) then
        begin
          if UserMagic.MagicInfo <> nil then
          begin
            BaseObject.m_Master.SendMsg(BaseObject.m_Master, RM_SPELL3, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, MakeLong
              (UserMagic.MagicInfo.wMagicId, UserMagic.btNewLevel * 2), '');

            BaseObject.m_Master.SendRefMsg(RM_SPELL, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, MakeLong(UserMagic.MagicInfo.wMagicId,
              UserMagic.btNewLevel * 2), '');
          end;
        end;
      end;
    end
    else
    begin
      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) //
        and (UserMagic.wMagIdx in [SKILL_61, SKILL_62]) //
        and (TPlayObject(BaseObject).m_MyHero <> nil) then
      begin
        TPlayObject(BaseObject).m_MyHero.AttackDir(TargeTBaseObject, 13 + UserMagic.wMagIdx - SKILL_61, TPlayObject(BaseObject).m_MyHero.m_btDirection);
      end;

      with BaseObject do
      begin
        // 4级技能强化 -- 4级灵魂火符 4级灭天火 chongchong 2013-12-04
        if (UserMagic.wMagIdx = SKILL_FIRECHARM) { 灵魂火符 }
          or (UserMagic.wMagIdx = SKILL_45) { 灭天火 }
          or (UserMagic.wMagIdx = SKILL_SHIELD) { 魔法盾 }
          or (UserMagic.wMagIdx = SKILL_73) { 道力盾 }
          or (UserMagic.wMagIdx = SKILL_87) { 武力盾 }
          or (UserMagic.wMagIdx = SKILL_88) { 新武力盾 }
          or (UserMagic.wMagIdx = SKILL_89) { 新道力盾 } then
          BaseObject.SendMsg(BaseObject, RM_SPELL3, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, MakeLong(UserMagic.MagicInfo.wMagicId,
            UserMagic.btNewLevel * 2), IntToStr(UserMagic.btLevel))
        else
          BaseObject.SendMsg(BaseObject, RM_SPELL3, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, MakeLong(UserMagic.MagicInfo.wMagicId,
            UserMagic.btNewLevel * 2), '');

        // BaseObject.SendRefMsg(RM_SPELL, UserMagic.MagicInfo.btEffect, nTargetX, nTargetY, UserMagic.MagicInfo.wMagicId, '');
      end;
    end;
  end;

  boTrain := False;
  boSpellFail := False;
  boSpellFire := True;
  if BaseObject.m_boTrainingNG and (BaseObject.m_AbilNG.NH > 0) then
  begin // 学过内力每次攻击减内力
    BaseObject.m_AbilNG.NH := Max(0, BaseObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
    BaseObject.RefAbilNH;
  end;

  if (UserMagic.MagicInfo.wMagicId = SKILL_FIREBALL) { 1 火球术 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_FIREBALL2) { 5 大火球 } then
  begin
    if MagMakeFireball(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_HEALLING { 2 } then
  begin
    if MagTreatment(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_AMYOUNSUL { 6 } then // 施毒术
  begin
    if CheckCD(BaseObject, SKILL_AMYOUNSUL, BaseObject.GetMagicCD(SKILL_AMYOUNSUL), boDoNotCheck) then
    begin
      if BaseObject.m_btRaceServer = RC_HEROOBJECT then
      begin
        boSpellFail := False;
        if MagLightening(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, boSpellFail) then
          boTrain := True
      end
      else
      begin
        if MagLightening(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, boSpellFail) then
          boTrain := True
        else
          boSpellFail := True;
      end;
      // 注销下面一行，解决施毒术失败后仍然有毒雾散开效果的问题 Cursor 2023-07-06 17:20:22
      // boSpellFail := False; // 暂时曲线救国，修复瞬移后施毒术导致暗杀 By 一支笔 at:2021-12-28 15:59:48
    end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_FIREWIND { 8 } then
  begin // 抗拒火环
    if MagPushArround(BaseObject, UserMagic, UserMagic.btLevel) > 0 then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_FIRE { 9 } then
  begin // 地狱火
    if MagMakeHellFire(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_SHOOTLIGHTEN { 10 } then
  begin
    if MagMakeQuickLighting(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_LIGHTENING { 11 } then
  begin // 雷电术
    if MagMakeLighting(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if (UserMagic.MagicInfo.wMagicId = SKILL_FIRECHARM) { 13 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_HANGMAJINBUB) { 14 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_DEJIWONHO) { 15 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_HOLYSHIELD) { 16 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_SKELLETON) { 17 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_CLOAK) { 18 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_BIGCLOAK) { 19 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_38) { 诅咒术 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_46) { 新诅咒术 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_57) { 噬血术 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_202) { 裂神符 }
    or (UserMagic.MagicInfo.wMagicId = SKILL_210) { 幽冥火符 } then
  begin
    boSpellFail := False;
    nType := 5;
    // 检测符
    if CheckAmulet(BaseObject, nType, 1, nAmuletIdx) //
      or CheckBagAmulet(BaseObject, nType, 1, nAmuletIdx, True) //
      or (btNpcReleaseMagic <> 0) then
    begin
      // 使用一张符
      UseAmulet(BaseObject, 1, nAmuletIdx);
      if UserMagic.MagicInfo.wMagicId = SKILL_FIRECHARM { 13 } then // 灵魂火符
      begin
        if MagMakeFireCharm(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, False) then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_202 then // 裂神符 piaoyun 2013-06-24
      begin
        if CheckCD(BaseObject, SKILL_202, BaseObject.GetMagicCD(SKILL_202), boDoNotCheck) then
        begin
          if MagMakeFireCharm(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, True) then
            boTrain := True;
        end
        else
          boSpellFail := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_HANGMAJINBUB { 14 } then // 幽灵盾
      begin
        Int64Value1 := GetPower13(BaseObject, 60, UserMagic) + Int64(BaseObject.m_WAbil.SC1) * 10;
        Int64Value2 := Int64(BaseObject.m_WAbil.SC2) - BaseObject.m_WAbil.SC1 + 1;
        Int64Value1 := Min(Int64Value1, High(Integer));
        Int64Value2 := Min(Int64Value2, High(Integer));
        nPower := BaseObject.GetAttackPower(Int64Value1, Int64Value2);
        // nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害 强化不加时间，只加值 2020-06-15 23:31:01
        if BaseObject.MagMakeDefenceArea(nTargetX, nTargetY, 3, nPower, 1, UserMagic.btNewLevel) > 0 then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_DEJIWONHO { 15 } then // 神圣战甲术
      begin
        Int64Value1 := GetPower13(BaseObject, 60, UserMagic) + Int64(BaseObject.m_WAbil.SC1) * 10;
        Int64Value2 := Int64(BaseObject.m_WAbil.SC2) - BaseObject.m_WAbil.SC1 + 1;
        Int64Value1 := Min(Int64Value1, High(Integer));
        Int64Value2 := Min(Int64Value2, High(Integer));
        nPower := BaseObject.GetAttackPower(Int64Value1, Int64Value2);
        // nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害 强化不加时间，只加值 2020-06-15 23:31:01
        if BaseObject.MagMakeDefenceArea(nTargetX, nTargetY, 3, nPower, 0, UserMagic.btNewLevel) > 0 then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_HOLYSHIELD { 16 } then // 捆魔咒
      begin
        if MagMakeHolyCurtain(BaseObject, GetPower13(BaseObject, 40, UserMagic) + GetRPow(BaseObject, BaseObject.m_WAbil.SC1,
          BaseObject.m_WAbil.SC2) * 3, nTargetX, nTargetY) > 0 then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_SKELLETON { 17 } then // 召唤骷髅
      begin
        // 召唤骷髅时间间隔 chongchong 2013-10-28
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwTempTime := MyGetTickCount - BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId]
        else
          dwTempTime := High(LongWord);

        dwTempCDTime := BaseObject.GetMagicCD(SKILL_SKELLETON);
        if dwTempTime < dwTempCDTime then
        begin
          {
            dwTempTime := dwTempCDTime - dwTempTime;
            if dwTempTime mod 1000 <> 0 then
            dwTempTime := dwTempTime div 1000 + 1
            else
            dwTempTime := dwTempTime div 1000;
          }
        end
        else if MagMakeSlave(BaseObject, UserMagic) then
        begin
          boTrain := True;
          BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId] := MyGetTickCount;
        end;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_CLOAK { 18 } then // 隐身术
      begin
        if MagMakePrivateTransparent(BaseObject, GetPower13(BaseObject, 30, UserMagic) + GetRPow(BaseObject, BaseObject.m_WAbil.SC1,
          BaseObject.m_WAbil.SC2) { * 3 } ) then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_BIGCLOAK { 19 } then // 集体隐身术
      begin
        if MagMakeGroupTransparent(BaseObject, nTargetX, nTargetY, GetPower13(BaseObject, 30, UserMagic) + GetRPow(BaseObject,
          BaseObject.m_WAbil.SC1, BaseObject.m_WAbil.SC2) { * 3 } ) then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_38 { 诅咒术 } then
      begin
        nPower := BaseObject.GetAttackPower(GetPower13(BaseObject, 20, UserMagic) + BaseObject.m_WAbil.SC1 * 2, BaseObject.m_WAbil.SC2
          - BaseObject.m_WAbil.SC1 + 1);

        nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
        if BaseObject.MagMakeDefenceAreaDown(nTargetX, nTargetY, 3, nPower, 0) > 0 then
          boTrain := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_46 { 新诅咒术 } then
      begin
        if CheckCD(BaseObject, SKILL_46, BaseObject.GetMagicCD(SKILL_46), boDoNotCheck) then
        begin
          nSec := BaseObject.GetAttackPower(GetPower13(BaseObject, 20, UserMagic) + BaseObject.m_WAbil.SC1 * 2, BaseObject.m_WAbil.SC2
            - BaseObject.m_WAbil.SC1 + 1);

          nSec := Round(nSec / 100 * g_Config.nSkill46SecRate);
            // 新诅咒术时间倍数不算强化 2020-06-15 23:57:28  Round(GetNewLevelPower(nSec, UserMagic) / 100 * g_Config.nSkill46SecRate);
          nPower := (UserMagic.btLevel + UserMagic.btNewLevel + 1) * g_Config.nSkill46PowerBase;
          if BaseObject.MagMakeDefenceAreaDownEx(nTargetX, nTargetY, 3, nSec, nPower) > 0 then
            boTrain := True;
        end
        else
          boSpellFail := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_57 then
      begin
        if CheckCD(BaseObject, SKILL_57, BaseObject.GetMagicCD(SKILL_57), boDoNotCheck) then // 噬血术
        begin
          if MagAbsorbBlood(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
            boTrain := True;
        end
        else
          boSpellFail := True;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_210 then
      begin // 幽冥火符 piaoyun 2013-09-14
        if CheckCD(BaseObject, SKILL_210, BaseObject.GetMagicCD(SKILL_210), boDoNotCheck) then
        begin
          nPower := BaseObject.GetAttackPower(GetPower13(BaseObject, 30, UserMagic) + BaseObject.m_WAbil.SC1 * 2, BaseObject.m_WAbil.SC2
            - BaseObject.m_WAbil.SC1 + 1);

          nPower := GetNewLevelPower(nPower, UserMagic);
          if MagMakeSkill210(BaseObject, UserMagic, nPower, nTargetX, nTargetY) then
            boTrain := True;
        end
        else
          boSpellFail := True;
      end;
    end
    else // 修复跳过灵魂火符失效 后续动作 piaoyun 2013-09-07
    begin
      boSpellFire := False;
      boSpellFail := True;

      if (Length(sNeedFu) > 0) then
        BaseObject.SendMsg(BaseObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sNeedFu);
    end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_203 then // 死亡之眼 piaoyun 2013-06-24
  begin
    if CheckCD(BaseObject, SKILL_203, BaseObject.GetMagicCD(SKILL_203), boDoNotCheck) then
    begin
      if MagBigExplosionAndMakePoison(BaseObject, UserMagic, BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
        UserMagic), UserMagic) + BaseObject.m_WAbil.SC1, BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1 + 1), nTargetX, nTargetY)
        then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_205 then // 冰霜雪雨 piaoyun 2013-06-26
  begin
    if CheckCD(BaseObject, SKILL_205, BaseObject.GetMagicCD(SKILL_205), boDoNotCheck) then
    begin
      if MagDoubleBigExplosionEx(BaseObject, BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic)
        + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), nTargetX, nTargetY, UserMagic.wMagIdx,
        True) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_206 then // 冰霜群雨 piaoyun 2013-06-26
  begin
    if CheckCD(BaseObject, SKILL_206, BaseObject.GetMagicCD(SKILL_206), boDoNotCheck) then
    begin
      if MagBigExplosionAndMakePoisonEx(BaseObject, UserMagic, BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
        UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), nTargetX, nTargetY)
        then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_209 then // 五雷轰 piaoyun 2013-09-14
  begin
    if CheckCD(BaseObject, SKILL_209, BaseObject.GetMagicCD(SKILL_209), boDoNotCheck) then
    begin
      if MagMakeSkill209(BaseObject, UserMagic, BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic),
        UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), nTargetX, nTargetY) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_TAMMING { 20 } then // 诱惑之光
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      if MagTamming(BaseObject, TargeTBaseObject, nTargetX, nTargetY, UserMagic.btLevel) then
        boTrain := True;
    end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_SPACEMOVE { 21 } then // 瞬息移动
  begin
    if (BaseObject.m_PEnvir <> nil) then
    begin
      BaseObject.SendRefMsg(RM_MAGICFIRE, UserMagic.btNewLevel * 2, MakeWord(UserMagic.MagicInfo.btEffectType, UserMagic.MagicInfo.btEffect),
        MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), '');
      boSpellFire := False;

      if MagSaceMove(BaseObject, UserMagic.btLevel) then
        boTrain := True;
    end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_EARTHFIRE { 22 } then // 火墙
  begin
    if MagMakeFireCross(BaseObject, GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic),
      UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic), GetPower(BaseObject,
      10, UserMagic) + (Word(GetRPow(BaseObject, BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2)) shr 1), nTargetX, nTargetY,
      UserMagic.btNewLevel) > 0 then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_FIREBOOM { 23 } then // 爆裂火焰
  begin
    if MagBigExplosion(BaseObject, UserMagic, GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
      UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic), nTargetX,
      nTargetY, g_Config.nFireBoomRage, g_Config.nFireBoomRagePowerRate { 1 } ) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_LIGHTFLOWER { 24 } then // 地狱雷光
  begin
    if MagElecBlizzard(BaseObject, UserMagic, GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
      UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic)) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_SHOWHP { 28 } then // 心灵启示 chongchong 2015-09-10
  begin
    if (TargeTBaseObject <> nil) and not TargeTBaseObject.m_boShowHP then
    begin
      if Random(6) <= (UserMagic.btLevel + 3) then
      begin
        TargeTBaseObject.m_dwShowHPTick := MyGetTickCount();
        TargeTBaseObject.m_dwShowHPInterval := GetPower13(BaseObject, GetRPow(BaseObject, BaseObject.m_WAbil.SC1, BaseObject.m_WAbil.SC2)
          * 2 + 30, UserMagic) * 1000;

        TargeTBaseObject.SendDelayMsg(TargeTBaseObject, RM_DOOPENHEALTH, 0, 0, 0, 0, '', 1500);
        boTrain := True;
      end;
    end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_90 then // 宠物捕捉
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) //
      and MagCapturePets(BaseObject, TargeTBaseObject, nTargetX, nTargetY, UserMagic.btLevel) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_91 then // 招魂术
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) //
      and MagSpiritualism(BaseObject, TargeTBaseObject, nTargetX, nTargetY, UserMagic.btLevel, UserMagic.btNewLevel) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_BIGHEALLING { 29 } then
  begin // 群体治疗术
    nPower := BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + BaseObject.m_WAbil.SC1 * 2,
      (BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1) * 2 + 1);

    nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
    if MagBigHealing(BaseObject, nPower, nTargetX, nTargetY) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_SHIELD { 魔法盾31 } then
  begin
    boSpellFail := False;
    BaseObject.m_btMagBubbleDefenceNewLevel := UserMagic.btNewLevel;
    if BaseObject.MagBubbleDefenceUp(UserMagic.btLevel, GetPower(BaseObject, GetRPow(BaseObject, BaseObject.m_WAbil.MC1,
      BaseObject.m_WAbil.MC2) + 15, UserMagic)) then
      boTrain := True;

    // 添加魔法盾强化等级 piaoyun 2013-08-18
    BaseObject.m_btMagBubbleDefenceNewLevel := UserMagic.btNewLevel;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_73 { 道力盾73 } then
  begin
    boSpellFail := False;
    if BaseObject.MagBubbleDefenceUp(UserMagic.btLevel, GetPower(BaseObject, GetRPow(BaseObject, BaseObject.m_WAbil.SC1,
      BaseObject.m_WAbil.SC2) + 15, UserMagic)) then
      boTrain := True;

    // 添加魔法盾强化等级 piaoyun 2013-08-18
    BaseObject.m_btMagBubbleDefenceNewLevel := UserMagic.btNewLevel;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_87 { 武力盾 } then
  begin
    boSpellFail := False;
    if BaseObject.MagBubbleDefenceUp(UserMagic.btLevel, GetPower(BaseObject, GetRPow(BaseObject, BaseObject.m_WAbil.DC1,
      BaseObject.m_WAbil.DC2) + 15, UserMagic)) then
      boTrain := True;

    // 添加魔法盾强化等级 piaoyun 2013-08-18
    BaseObject.m_btMagBubbleDefenceNewLevel := UserMagic.btNewLevel;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_88 { 新武力盾 } then
  begin
    boSpellFail := False;
    if BaseObject.NewHitBubbleDefenceUp(UserMagic.btLevel, GetPower(BaseObject, GetRPow(BaseObject, BaseObject.m_WAbil.DC1,
      BaseObject.m_WAbil.DC2) + 15, UserMagic)) then
      boTrain := True;

    // 添加魔法盾强化等级 piaoyun 2013-08-18
    BaseObject.m_btNewHitBubbleDefenceNewLevel := UserMagic.btNewLevel;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_89 { 新道力盾 } then
  begin
    boSpellFail := False;
    if BaseObject.NewMagBubbleDefenceUp(UserMagic.btLevel, GetPower(BaseObject, GetRPow(BaseObject, BaseObject.m_WAbil.DC1,
      BaseObject.m_WAbil.DC2) + 15, UserMagic)) then
      boTrain := True;

    // 添加魔法盾强化等级 piaoyun 2013-08-18
    BaseObject.m_btNewMagBubbleDefenceNewLevel := UserMagic.btNewLevel;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_KILLUNDEAD { 32 } then // 圣言术
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) //
      and MagTurnUndead(BaseObject, TargeTBaseObject, nTargetX, nTargetY, UserMagic.btLevel) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_SNOWWIND { 33 } then // 冰咆哮
  begin
    // 加入冰咆哮使用时间间隔 chongchong 2014-01-02
    // if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    // begin
    if CheckCD(BaseObject, SKILL_SNOWWIND, BaseObject.GetMagicCD(SKILL_SNOWWIND), boDoNotCheck) then
    begin
      if MagBigExplosion(BaseObject, UserMagic, GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
        UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic),
        nTargetX, nTargetY, g_Config.nSnowWindRange, g_Config.nSnowWindPowerRate { 1 } ) then
        boTrain := True;
    end
    else
      boSpellFail := True;

    // end
    // else
    // begin
    // if MagBigExplosion(BaseObject, UserMagic,

        // GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic),
    // nTargetX,
    // nTargetY,
    // g_Config.nSnowWindRange, g_Config.nSnowWindPowerRate {1}) then
    // boTrain := True;
    // end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_UNAMYOUNSUL { 34 } then // 解毒术
  begin
    if MagMakeUnTreatment(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_WINDTEBO { 35 } then
  begin
    if MagWindTebo(BaseObject, UserMagic) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_MABE { 36 } then // 冰焰
  begin
    with BaseObject do
    begin
      nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 -
        m_WAbil.MC1, 1));
    end;

    nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
    nPower := GetSkillLastPowerNG(nPower, // 内功技能威力
      GetSkillAttackPowerNG(BaseObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));

    if MabMabe(BaseObject, TargeTBaseObject, UserMagic, nPower, UserMagic.btLevel, nTargetX, nTargetY) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_GROUPLIGHTENING { 37 群体雷电术 } then
  begin
    if MagGroupLightening(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, boSpellFire) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_GROUPAMYOUNSUL { 51 群体施毒术 } then
  begin
    if CheckCD(BaseObject, SKILL_GROUPAMYOUNSUL, BaseObject.GetMagicCD(SKILL_GROUPAMYOUNSUL), boDoNotCheck) then
    begin
      if MagGroupAmyounsul(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, boSpellFail) then
        boTrain := True
      else
        boSpellFail := True;
    end
    else
      boSpellFail := True;

    boSpellFail := False; // 暂时曲线救国，修复瞬移后群体施毒术导致暗杀 By 一支笔 at:2021-12-28 15:59:48
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_GROUPDEDING then // 39 地钉
  begin
    if CheckCD(BaseObject, SKILL_GROUPDEDING, BaseObject.GetMagicCD(SKILL_GROUPDEDING), boDoNotCheck) then
    begin
      BaseObject.m_SkillUseTick[39] := MyGetTickCount;
      if g_Config.boDedingAllowPK then
      begin
        if MagGroupDeDing(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
          boTrain := True;
      end
      else if (TargeTBaseObject <> nil) //
        and (TargeTBaseObject.m_btRaceServer <> RC_PLAYOBJECT) //
        and MagGroupDeDing(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_41 then
  begin // 狮子吼
    if CheckCD(BaseObject, SKILL_41, BaseObject.GetMagicCD(SKILL_41), boDoNotCheck) then
    begin
      if MagGroupMb(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_44 then // 法师
  begin // 寒冰掌
    if MagHbFireBall(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_45 then
  begin
    if CheckCD(BaseObject, SKILL_45, BaseObject.GetMagicCD(SKILL_45), boDoNotCheck) then
    begin // 灭天火
      if MagMakeFireDay(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_47 then
  begin // 火龙烈炎
    if MagBigExplosion(BaseObject, UserMagic, GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
      UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic), nTargetX,
      nTargetY, g_Config.nFireBoomRage, 100 { 1 } ) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_48 then // 道士
  begin // 气功波
    if MagPushArround(BaseObject, UserMagic, UserMagic.btLevel) > 0 then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_49 then
  begin // 净化术
    boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_50 then
  begin // 无极真气
    if CheckCD(BaseObject, SKILL_50, BaseObject.GetMagicCD(SKILL_50), boDoNotCheck) then
    begin
      BaseObject.m_SkillUseTick[50] := MyGetTickCount;
      if BaseObject.AbilityUp(UserMagic, 2) then
        boTrain := True;
    end
    else
    begin
      BaseObject.SendMsg(BaseObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sElectrodelessFail);
      boSpellFail := True;
    end;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_52 then
  begin // 飓风破
    if MagGroupFengPo(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_54 then
  begin // 骷髅咒
    boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_58 then
  begin // 流星火雨
    // if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    // dwTempTime := g_Config.nHeroSkill58WaitTime
    // else if (BaseObject.m_Master <> nil) and (BaseObject.m_Master.m_btRaceServer = RC_HEROOBJECT) then
    // dwTempTime := g_Config.nHeroSkill58WaitTime * 1000
    // else
    // dwTempTime := BaseObject.GetMagicCD(SKILL_58);
    if CheckCD(BaseObject, SKILL_58, BaseObject.GetMagicCD(SKILL_58), boDoNotCheck { dwTempTime } ) then
    begin
      if MagMeteoriteRain(BaseObject, UserMagic, nTargetX, nTargetY) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_61 then
  begin // 劈星斩
    if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    begin
      if MagMakeSkillFire_61(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    {
      else if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
      begin
      if TPlayObject(BaseObject).m_MyHero <> nil then
      begin
      if MagMakeSkillFire_61(TSmartObject(TPlayObject(BaseObject).m_MyHero),
      UserMagic,
      nTargetX,
      nTargetY,
      TargeTBaseObject) then boTrain := True;
      end
      end
    }
    else
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_62 then
  begin // 雷霆一击
    if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    begin
      if MagMakeSkillFire_62(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_63 then
  begin // 噬魂沼泽
    if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    begin
      if MagMakeSkillFire_63(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_64 then
  begin // 末日审判
    if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    begin
      if MagMakeSkillFire_64(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_65 then
  begin // 火龙气焰
    if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    begin
      if MagMakeSkillFire_65(BaseObject, UserMagic, nTargetX, nTargetY) then
        boTrain := True;
    end
    else
      boTrain := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_69 then // 禁锢术
  begin
    if CheckCD(BaseObject, SKILL_69, BaseObject.GetMagicCD(SKILL_69), boDoNotCheck) then
    begin
      nTime := 3 + (UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkill69AddTime;
      nRange := 2 + (UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkill69AddRange;
      if MagMakeImprison(BaseObject, nTime, nRange, nTargetX, nTargetY) > 0 then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_70 then // 心灵召唤 chongchong 2015-09-10
  begin
    if CheckCD(BaseObject, SKILL_70, BaseObject.GetMagicCD(SKILL_70), boDoNotCheck) then
    begin
      if (BaseObject.m_nCurrX = nTargetX) and (BaseObject.m_nCurrY = nTargetY) then
        nDir := BaseObject.m_btDirection
      else
        nDir := GetNextDirection(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nTargetX, nTargetY);

      BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nDir, 1, nX, nY);
      if not BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
      begin
        nX := BaseObject.m_nCurrX;
        nY := BaseObject.m_nCurrY;
        BaseObject.m_boSlaveRelax := False;
        for nIndex := 0 to BaseObject.m_SlaveList.Count - 1 do
        begin
          TBaseObject(BaseObject.m_SlaveList.Items[nIndex]).SpaceMove(BaseObject.m_PEnvir.sMapName, nX, nY, 1);
          if (TargeTBaseObject <> nil) and BaseObject.IsProperTarget(TargeTBaseObject) then
            TBaseObject(BaseObject.m_SlaveList.Items[nIndex]).m_TargetCret := TargeTBaseObject;
        end;
      end
      else
      begin
        if (TargeTBaseObject <> nil) and BaseObject.IsProperTarget(TargeTBaseObject) then
          BaseObject.m_boSlaveRelax := False;

        I := 1;
        for nIndex := 0 to BaseObject.m_SlaveList.Count - 1 do
        begin
          BaseObject.m_PEnvir.GetNextPosition(BaseObject.m_nCurrX, BaseObject.m_nCurrY, nDir, I, nX, nY);
          if BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
          begin
            TBaseObject(BaseObject.m_SlaveList.Items[nIndex]).SpaceMove(BaseObject.m_PEnvir.sMapName, nX, nY, 1);
            if (TargeTBaseObject <> nil) and BaseObject.IsProperTarget(TargeTBaseObject) then
              TBaseObject(BaseObject.m_SlaveList.Items[nIndex]).m_TargetCret := TargeTBaseObject;

            Inc(I);
          end
          else
          begin
            TBaseObject(BaseObject.m_SlaveList.Items[nIndex]).SpaceMove(BaseObject.m_PEnvir.sMapName, BaseObject.m_nCurrX,
              BaseObject.m_nCurrY, 1);

            if (TargeTBaseObject <> nil) and BaseObject.IsProperTarget(TargeTBaseObject) then
              TBaseObject(BaseObject.m_SlaveList.Items[nIndex]).m_TargetCret := TargeTBaseObject;
          end;
        end;
      end;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_71 then
  begin // 擒龙手
    if CheckCD(BaseObject, SKILL_71, BaseObject.GetMagicCD(SKILL_71), boDoNotCheck) then
    begin
      if MagMakeArrestObject(BaseObject, UserMagic, TargeTBaseObject) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_72 then
  begin // 乾坤大挪移/移形换位
    BaseObject.SendRefMsg(RM_MAGICFIRE, UserMagic.btNewLevel * 2, MakeWord(UserMagic.MagicInfo.btEffectType, UserMagic.MagicInfo.btEffect),
      MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), '');
    boSpellFire := False;
    if CheckCD(BaseObject, SKILL_72, BaseObject.GetMagicCD(SKILL_72), boDoNotCheck) then
    begin
      nTemp := g_Config.nSkill72Rate - Round(g_Config.nSkill72Rate / 100 * ((UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkill72LevelUpRateAdd));

      if (nTemp <= 0) or (Random(nTemp) <= (UserMagic.btLevel + 3)) then
      begin
        if MagChangePosition(BaseObject, nTargetX, nTargetY) then
          boTrain := True;
      end;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_74 then
  // 分身术
  begin
    // 召唤分身时间间隔 chongchong 2013-10-28
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
      dwTempTime := MyGetTickCount - BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId]
    else
      dwTempTime := High(LongWord);

    dwTempCDTime := BaseObject.GetMagicCD(SKILL_74);
    if dwTempTime < dwTempCDTime then
    begin
      {
        dwTempTime := dwTempCDTime - dwTempTime;
        if dwTempTime mod 1000 <> 0 then
        dwTempTime := dwTempTime div 1000 + 1
        else
        dwTempTime := dwTempTime div 1000;
      }
    end
    else if MagMakeCopySelf(BaseObject, nTargetX, nTargetY, TargeTBaseObject, UserMagic.btLevel, UserMagic.btNewLevel) then
    begin
      boTrain := True;
      BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId] := MyGetTickCount;
    end;
  end
  { TODO -opiaoyun -c修改 : 修改召唤月灵需要符咒,case整合在一起 2013-06-24 }
  else if (UserMagic.MagicInfo.wMagicId = SKILL_SINSU) { 30 } // 召唤神兽
    or (UserMagic.MagicInfo.wMagicId = SKILL_76) // 召唤圣兽
    or (UserMagic.MagicInfo.wMagicId = SKILL_55) then // 召唤月灵
  begin
    boSpellFail := True;
    nType := 5;
    if CheckAmulet(BaseObject, nType, 5, nAmuletIdx) //
      or CheckBagAmulet(BaseObject, nType, 5, nAmuletIdx, True) //
      or (btNpcReleaseMagic <> 0) then
    begin
      UseAmulet(BaseObject, 5, nAmuletIdx);
      if UserMagic.MagicInfo.wMagicId = SKILL_SINSU then
      begin
        // 召唤神兽时间间隔 chongchong 2013-10-28
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwTempTime := MyGetTickCount - BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId]
        else
          dwTempTime := High(LongWord);

        dwTempCDTime := BaseObject.GetMagicCD(SKILL_SINSU);
        if dwTempTime < dwTempCDTime then
        begin
          {
            dwTempTime := dwTempCDTime - dwTempTime;
            if dwTempTime mod 1000 <> 0 then
            dwTempTime := dwTempTime div 1000 + 1
            else
            dwTempTime := dwTempTime div 1000;
          }
        end
        else if MagMakeSinSuSlave(BaseObject, UserMagic) then
        begin
          boTrain := True;
          BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId] := MyGetTickCount;
        end;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_76 then
      begin
        // 召唤圣兽时间间隔 chongchong 2013-10-28
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwTempTime := MyGetTickCount - BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId]
        else
          dwTempTime := High(LongWord);
        dwTempCDTime := BaseObject.GetMagicCD(SKILL_76);
        if dwTempTime < dwTempCDTime then
        begin
          {
            dwTempTime := dwTempCDTime - dwTempTime;
            if dwTempTime mod 1000 <> 0 then
            dwTempTime := dwTempTime div 1000 + 1
            else
            dwTempTime := dwTempTime div 1000;
          }
        end
        else if MagMakeBigDogSlave(BaseObject, UserMagic) then
        begin
          boTrain := True;
          BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId] := MyGetTickCount;
        end;
      end
      else if UserMagic.MagicInfo.wMagicId = SKILL_55 then
      // 召唤月灵
      begin
        // 召唤月灵时间间隔 chongchong 2013-10-28
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwTempTime := MyGetTickCount - BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId]
        else
          dwTempTime := High(LongWord);

        dwTempCDTime := BaseObject.GetMagicCD(SKILL_55);
        if dwTempTime < dwTempCDTime then
        begin
          {
            dwTempTime := dwTempCDTime - dwTempTime;
            if dwTempTime mod 1000 <> 0 then
            dwTempTime := dwTempTime div 1000 + 1
            else
            dwTempTime := dwTempTime div 1000;
          }
        end
        else if MagMakeMoon(BaseObject, UserMagic) then
        begin
          boTrain := True;
          BaseObject.m_SkillUseTick[UserMagic.MagicInfo.wMagicId] := MyGetTickCount;
        end;
      end;
      boSpellFail := False;
    end
    else if (Length(sNeedFu) > 0) then
      BaseObject.SendMsg(BaseObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sNeedFu);
  end
  /// //////////////////////连击技能--不计算经验///////////////////////////////
  else if UserMagic.MagicInfo.wMagicId = SKILL_104 then
  begin // 凤舞祭
    MagMakeSkill104(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject);
    boTrain := False; // True
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_105 then
  begin // 惊雷爆
    MagMakeSkill105(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject);
    boTrain := False;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_106 then
  begin // 冰天雪地
    MagMakeSkill106(BaseObject, UserMagic, nTargetX, nTargetY);
    boTrain := False;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_107 then
  begin // 双龙破
    MagMakeSkill107(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject);
    boTrain := False;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_108 then
  begin // 虎啸诀
    MagMakeSkill108(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject);
    boTrain := False;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_109 then
  begin // 八卦掌
    MagMakeSkill109(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject);
    boTrain := False;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_110 then
  begin // 三焰咒
    MagMakeSkill110(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject);
    boTrain := False;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_111 then
  begin // 万剑归宗
    MagMakeSkill111(BaseObject, UserMagic, nTargetX, nTargetY);
    boTrain := False;
  end
  /// /////////////////////////////////////////////////////////////////////////
  else if UserMagic.MagicInfo.wMagicId = SKILL_114 then
  begin // 倚天辟地
    // if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER] then
    // nSkillWaitTime := BaseObject.GetMagicCD(SKILL_114)
    // else
    // nSkillWaitTime := g_Config.nHeroSkill114HitWaitTime * 1000;
    if CheckCD(BaseObject, SKILL_114, BaseObject.GetMagicCD(SKILL_114), boDoNotCheck { nSkillWaitTime } ) then
    begin
      if MagMakeSkill114(BaseObject, UserMagic, nTargetX, nTargetY) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_116 then
  begin // 血魄一击(法)
    // if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER] then
    // nSkillWaitTime := BaseObject.GetMagicCD(SKILL_116)
    // else
    // nSkillWaitTime := g_Config.nHeroSkill116CD * 1000;
    if CheckCD(BaseObject, SKILL_116, BaseObject.GetMagicCD(SKILL_116), boDoNotCheck { nSkillWaitTime } ) then
    begin
      if MagMakeSkill116(BaseObject, UserMagic, nTargetX, nTargetY) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_117 then
  begin // 血魄一击(道)
    // if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER] then
    // nSkillWaitTime := BaseObject.GetMagicCD(SKILL_117)
    // else
    // nSkillWaitTime := g_Config.nHeroSkill117CD * 1000;
    if CheckCD(BaseObject, SKILL_117, BaseObject.GetMagicCD(SKILL_117), boDoNotCheck { nSkillWaitTime } ) then
    begin
      if MagMakeSkill117(BaseObject, UserMagic, nTargetX, nTargetY) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_204 then
  // 十步一杀 piaoyun 2013-06-25
  begin
    if CheckCD(BaseObject, SKILL_204, BaseObject.GetMagicCD(SKILL_204), boDoNotCheck) then
    begin
      if MagBigExplosionAndMakePoisonByWarr(BaseObject, UserMagic, BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
        UserMagic), UserMagic) + BaseObject.m_WAbil.DC1, BaseObject.m_WAbil.DC2 - BaseObject.m_WAbil.DC1 + 1), nTargetX, nTargetY,
        boMove) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  else if UserMagic.MagicInfo.wMagicId = SKILL_208 then
  // 旋风斩 piaoyun 2013-09-15
  begin
    if CheckCD(BaseObject, SKILL_208, BaseObject.GetMagicCD(SKILL_208), boDoNotCheck) then
    begin
      if MagMakeSkill208(BaseObject, UserMagic, BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic),
        UserMagic) + BaseObject.m_WAbil.DC1, BaseObject.m_WAbil.DC2 - BaseObject.m_WAbil.DC1 + 1)) then
        boTrain := True;
    end
    else
      boSpellFail := True;
  end
  // 新技能测试 --- piaoyun
  else if UserMagic.MagicInfo.wMagicId = SKILL_NEW_TEST { 201 } then
  begin
    if MagMakeNewSkill_Test(BaseObject, UserMagic, GetNewLevelPower(BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject,
      UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1 + 1), UserMagic), nTargetX,
      nTargetY, g_Config.nSnowWindRange { 1 } ) then
      boTrain := True;
  end
  else
  begin
    // 自定义技能攻击 chongchong 2015-03-20
    if CheckIsCustomMagic(UserMagic.MagicInfo.wMagicId) then
    begin
      if BaseObject.m_btRaceServer = RC_HEROOBJECT then
        boUse := True
      else
        boUse := BaseObject.AllowCustomSkill(UserMagic.MagicInfo.wMagicId, True);

      if boUse then
      begin
        if MagCustomSkill(BaseObject, UserMagic, nTargetX, nTargetY, TargeTBaseObject, sMsg, boCustomMagicMove, FNpcReleaseMagic,
          boSpellFire) then
        begin
          boTrain := True;
          if Length(sMsg) > 0 then
          begin
            boSpellFire := False;
            if btNpcReleaseMagic = 2 then
            begin
              BaseObject.SendRefMsg(RM_MAGICFIRE_EX, UserMagic.btNewLevel * 2 + btItemType, MakeWord(UserMagic.MagicInfo.btEffectType,
                UserMagic.MagicInfo.btEffect), MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), sMsg);
            end
            else
            begin
              BaseObject.SendRefMsg(RM_MAGICFIRE_EX_2, UserMagic.btNewLevel * 2 + btItemType, MakeWord(UserMagic.MagicInfo.btEffectType,
                UserMagic.MagicInfo.btEffect), MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), IntToStr(UserMagic.MagicInfo.wMagicId)
                + ',' + IntToStr(UserMagic.btLevel) + ';' + sMsg);
            end;
          end;
        end;
      end
      else
        boSpellFail := True;
    end;
  end;

  if boSpellFail then
    Exit;

  if boSpellFire { and boTrain } then
  begin
    // 这个包发送去处就会导致技能的后续动作 -- 如果引擎没处理魔法成功，则客户端为假动作 -- boSpellFail 变量直接决定是否走入这里   --- piaoyun 2013-06-27
    // 4级技能强化 -- 4级灵魂火符 4级灭天火 chongchong 2013-12-04
    if (UserMagic.wMagIdx = SKILL_FIRECHARM) { 灵魂火符 }
      or (UserMagic.wMagIdx = SKILL_45) { 灭天火 }
      or (UserMagic.wMagIdx = SKILL_SHIELD) { 魔法盾 }
      or (UserMagic.wMagIdx = SKILL_73) { 道力盾 }
      or (UserMagic.wMagIdx = SKILL_87) { 武力盾 }
      or (UserMagic.wMagIdx = SKILL_88) { 新武力盾 }
      or (UserMagic.wMagIdx = SKILL_89) { 新道力盾 } then
    begin
      if btNpcReleaseMagic < 2 then
      begin
        BaseObject.SendRefMsg(RM_MAGICFIRE, UserMagic.btNewLevel * 2 + btItemType, MakeWord(UserMagic.MagicInfo.btEffectType,
          UserMagic.MagicInfo.btEffect), MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), IntToStr(UserMagic.btLevel));
      end
      else
      begin
        BaseObject.SendRefMsg(RM_MAGICFIRE_EX_2, UserMagic.btNewLevel * 2 + btItemType, MakeWord(UserMagic.MagicInfo.btEffectType,
          UserMagic.MagicInfo.btEffect), MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), IntToStr(UserMagic.MagicInfo.wMagicId)
          + ',' + IntToStr(UserMagic.btLevel) + ';');
      end;
    end
    else
    begin
      if btNpcReleaseMagic < 2 then
      begin
        BaseObject.SendRefMsg(RM_MAGICFIRE, UserMagic.btNewLevel * 2 + btItemType, MakeWord(UserMagic.MagicInfo.btEffectType,
          UserMagic.MagicInfo.btEffect), MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), '');
      end
      else
      begin
        BaseObject.SendRefMsg(RM_MAGICFIRE_EX_2, UserMagic.btNewLevel * 2 + btItemType, MakeWord(UserMagic.MagicInfo.btEffectType,
          UserMagic.MagicInfo.btEffect), MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), IntToStr(UserMagic.MagicInfo.wMagicId)
          + ',' + IntToStr(0) + ';');
      end;
    end;

    if (UserMagic.wMagIdx >= SKILL_61) and (UserMagic.wMagIdx <= SKILL_65) then
    begin // 合击
      if BaseObject.m_btRaceServer = RC_HEROOBJECT then
      begin
        if (btNpcReleaseMagic < 2) and (BaseObject.m_Master <> nil) then
        begin
          if (UserMagic.wMagIdx >= SKILL_62) and (UserMagic.wMagIdx <= SKILL_65) then
          begin
            BaseObject.m_Master.SendRefMsg(RM_MAGICFIRE, 0, MakeWord(UserMagic.MagicInfo.btEffectType, UserMagic.MagicInfo.btEffect),
              MakeLong(nTargetX, nTargetY), NativeInt(TargeTBaseObject), '');
          end;
        end;
      end;
    end;
  end;

  if (btNpcReleaseMagic <> 0) then
    boTrain := False
  else if boTrain and (UserMagic.wMagIdx in [SKILL_61..SKILL_65]) and (BaseObject.m_btRaceServer <> RC_HEROOBJECT) then
    boTrain := False;

  if (UserMagic.btLevel < UserMagic.MagicInfo.btTrainLv) and (boTrain) then
  begin
    if UserMagic.MagicInfo.TrainLevel[UserMagic.btLevel] <= BaseObject.m_Abil.Level then
    begin
      BaseObject.TrainSkill(UserMagic, Random(3) + 1);
      if not BaseObject.CheckMagicLevelup(UserMagic) then
      begin
        BaseObject.SendDelayMsg(BaseObject, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId, MakeWord(UserMagic.btLevel, UserMagic.btNewLevel),
          UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)), 1000);
      end;
    end;
  end;

  // 十步一杀，发送后续移动效果 -- piaoyun 2013-06-25
  if (UserMagic.MagicInfo.wMagicId = SKILL_204) and boMove then
  begin
    BaseObject.SendRefMsg(RM_MAGICMOVE, BaseObject.m_btDirection, BaseObject.m_nCurrX, BaseObject.m_nCurrY, 0, '');
  end
  else if boCustomMagicMove and CheckIsCustomMagic(UserMagic.MagicInfo.wMagicId) then
  begin
    BaseObject.SendRefMsg(RM_CUSTOM_MAGICMOVE, BaseObject.m_btDirection, BaseObject.m_nCurrX, BaseObject.m_nCurrY, UserMagic.MagicInfo.wMagicId,
      '');
  end;

  { TODO -opiaoyun -c测试 : ★★★★★DoSpell返回结果修改，先测试，可能不行~★★★★★★ }
  Result := True;
end;

function TMagicManager.MagMakePrivateTransparent(BaseObject: TBaseObject; nHTime: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
begin
  Result := False;
  if BaseObject.m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] > 0 then
    Exit;

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, BaseObject.m_nCurrX, BaseObject.m_nCurrY, 9, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) and (TargeTBaseObject.m_TargetCret = BaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - BaseObject.m_nCurrX) > 1) //
        or (abs(TargeTBaseObject.m_nCurrY - BaseObject.m_nCurrY) > 1) //
        or (Random(2) = 0) then
        TargeTBaseObject.m_TargetCret := nil;
    end;
  end;

  BaseObjectList.Free;
  BaseObject.m_dwStatusArrTick[STATE_TRANSPARENT { 0x70 } ] := MyGetTickCount;
  BaseObject.m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] := nHTime; // 004931D2
  BaseObject.m_nCharStatus := BaseObject.GetCharStatus();
  BaseObject.StatusChanged();
  BaseObject.m_boHideMode := True;
  BaseObject.m_boTransparent := True;
  Result := True;
end;

function TMagicManager.MagReturn(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nMagicLevel: Integer):
  Boolean; // 00492368
begin
  TargeTBaseObject.ReAlive;
  TargeTBaseObject.m_WAbil.HP := TargeTBaseObject.m_WAbil.MaxHP;
  TargeTBaseObject.SendMsg(TargeTBaseObject, RM_ABILITY, 0, 0, 0, 0, '');
  Result := True;
end;

function TMagicManager.MagTamming(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nMagicLevel:
  Integer): Boolean; // 00492368
var
  I, SlaveCount: Integer;
  n14: Int64;
  BaseObj: TBaseObject;
begin
  Result := False;
  // 防诱惑修改  -- piaoyun 2013-07-18
  if TargeTBaseObject.UnTamming { 防诱惑 } or (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_ANIMAL,
    RC_PLAYMOSTER]) { 非人物、英雄、攻击NPC } then
    Exit;

  Randomize;

  if (not (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_ANIMAL, RC_PLAYMOSTER { 分身、人形怪 } ])) and ((Random(4
    - nMagicLevel) = 0)) then
  begin
    TargeTBaseObject.m_TargetCret := nil;
    if TargeTBaseObject.m_Master = BaseObject then
    begin
      TargeTBaseObject.OpenHolySeizeMode((nMagicLevel * 5 + 10) * 1000);
      Result := True;
    end
    else
    begin
      if Random(2) = 0 then
      begin
        if TargeTBaseObject.m_Abil.Level <= BaseObject.m_Abil.Level + 2 then
        begin
          if Random(3) = 0 then
          begin
            if Random((BaseObject.m_Abil.Level + 20) + (nMagicLevel * 5)) > (TargeTBaseObject.m_Abil.Level + g_Config.nMagTammingTargetLevel
              { 10 } ) then
            begin
              SlaveCount := 0;
              for I := 0 to BaseObject.m_SlaveList.Count - 1 do
              begin
                BaseObj := BaseObject.m_SlaveList.Items[I];
                if (not BaseObj.m_boDeath) and (not BaseObj.m_boGhost) and ((BaseObj.m_wCallSkill in [SKILL_91, SKILL_TAMMING]) or
                  (BaseObj.m_BBType = bb_Other)) then
                  Inc(SlaveCount);
              end;

              if (TargeTBaseObject.m_btLifeAttrib = 0) and (TargeTBaseObject.m_Abil.Level < g_Config.nMagTammingLevel { 50 } ) and
                (SlaveCount < g_Config.nMagTammingCount { (nMagicLevel + 2) } ) then
              begin
                n14 := TargeTBaseObject.m_WAbil.MaxHP div g_Config.nMagTammingHPRate { 100 };
                if n14 <= 2 then
                  n14 := 2
                else
                  Inc(n14, n14);

                if n14 > High(Integer) then
                  n14 := High(Integer);

                if (TargeTBaseObject.m_Master <> BaseObject) and (Random(n14) = 0) then
                begin
                  TargeTBaseObject.BreakCrazyMode();
                  if TargeTBaseObject.m_Master <> nil then
                  begin
                    TargeTBaseObject.m_WAbil.HP := TargeTBaseObject.m_WAbil.HP div 10;
                    // 叛变的宝宝再次用诱惑之光召唤后会死亡 chongchong 2014-09-01
                    if TargeTBaseObject.m_WAbil.HP = 0 then
                      TargeTBaseObject.m_WAbil.HP := 1;
                  end;

                  if BaseObject.m_btRaceServer = RC_HEROOBJECT then
                  begin
                    if BaseObject.m_TargetCret = TargeTBaseObject then
                      BaseObject.DelTargetCreat;

                    if (BaseObject.m_Master <> nil) and (BaseObject.m_Master.m_TargetCret = TargeTBaseObject) then
                      BaseObject.m_Master.DelTargetCreat;
                  end;

                  TargeTBaseObject.m_Master := BaseObject;
                  TargeTBaseObject.m_dwMasterRoyaltyStartTick := MyGetTickCount;
                  TargeTBaseObject.m_dwMasterRoyaltyTime := LongWord((Random(BaseObject.m_Abil.Level * 2) + (nMagicLevel shl 2) *
                    5 + g_Config.nMasterRoyaltyTime) * 60 * 1000);
                  TargeTBaseObject.m_btSlaveMakeLevel := nMagicLevel;

                  if TargeTBaseObject.m_dwMasterTick = 0 then
                    TargeTBaseObject.m_dwMasterTick := MyGetTickCount();

                  TargeTBaseObject.BreakHolySeizeMode();

                  if LongWord(1500 - nMagicLevel * 200) < LongWord(TargeTBaseObject.m_nWalkSpeed) then
                    TargeTBaseObject.m_nWalkSpeed := 1500 - nMagicLevel * 200;

                  if LongWord(2000 - nMagicLevel * 200) < LongWord(TargeTBaseObject.m_nNextHitTime) then
                    TargeTBaseObject.m_nNextHitTime := 2000 - nMagicLevel * 200;

                  TargeTBaseObject.RefShowName();
                  TargeTBaseObject.m_wCallSkill := SKILL_TAMMING;
                  BaseObject.m_SlaveList.Add(TargeTBaseObject);

                  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) or ((BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer
                    = RC_PLAYOBJECT)) then
                    TargeTBaseObject.RefHumsBB(True);
                end
                else if Random(14) = 0 then
                begin
                  // 叛变的宝宝再次用诱惑之光召唤后会死亡 chongchong 2014-09-01
                  TargeTBaseObject.SetLastHiter(BaseObject);
                  TargeTBaseObject.m_WAbil.HP := 0;
                end;
              end
              else if (TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD) and (Random(20) = 0) then
              begin
                // 叛变的宝宝再次用诱惑之光召唤后会死亡 chongchong 2014-09-01
                TargeTBaseObject.SetLastHiter(BaseObject);
                TargeTBaseObject.m_WAbil.HP := 0;
              end;
            end
            else if not (TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD) and (Random(20) = 0) then
              TargeTBaseObject.OpenCrazyMode(Random(20) + 10);
          end
          else if not (TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD) then
            TargeTBaseObject.OpenCrazyMode(Random(20) + 10); // 变红
        end;
      end
      else
        TargeTBaseObject.OpenHolySeizeMode((nMagicLevel * 5 + 10) * 1000);

      Result := True;
    end;
  end
  else if Random(2) = 0 then
    Result := True;
end;

// 招魂术技能
function TMagicManager.MagSpiritualism(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nMagicLevel,
  nMagicNewLevel: Integer): Boolean; // 00492368
var
  I, RandomValue: Integer;
  IsCanUse: Boolean;
  nSpiritualismBBCount: Integer;
  BaseObj: TBaseObject;
begin
  Result := False;
  if (TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_ANIMAL, RC_PLAYMOSTER]) or (not TargeTBaseObject.m_boDeath)
    { 非人物、英雄、攻击NPC } then
    Exit;

  if (TargeTBaseObject.Master <> nil) and (TargeTBaseObject.Master <> BaseObject) then
    Exit;

  if Random(30) = 0 then
  begin
    TargeTBaseObject.MakeGhost;
    Exit;
  end;

  if TargeTBaseObject.m_wCallSkill = SKILL_91 then
    Exit;

  if g_Config.boSpiritualismDisableUndeadMon and (TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD) then
    Exit;

  IsCanUse := True;
  if g_Config.boSpiritualismLevelDiff then
  begin
    if g_Config.nSpiritualismLevelDiff > 0 then
      IsCanUse := Int64(BaseObject.m_Abil.Level) >= Int64(TargeTBaseObject.m_Abil.Level + g_Config.nSpiritualismLevelDiff)
    else
      IsCanUse := Int64(BaseObject.m_Abil.Level - g_Config.nSpiritualismLevelDiff) >= Int64(TargeTBaseObject.m_Abil.Level)
  end;

  nSpiritualismBBCount := 0;
  for I := BaseObject.m_SlaveList.Count - 1 downto 0 do
  begin
    BaseObj := BaseObject.m_SlaveList.Items[I];
    if (not BaseObj.m_boDeath) and (not BaseObj.m_boGhost) and ((BaseObj.m_wCallSkill in [SKILL_91, SKILL_TAMMING]) or (BaseObject.m_BBType
      = bb_Other)) then
    begin
      Inc(nSpiritualismBBCount);
    end;
  end;

  if nSpiritualismBBCount >= g_Config.nSpiritualismBBCount then
    Exit;

  if IsCanUse then
  begin
    if nMagicNewLevel > 0 then
    begin
      if nMagicNewLevel > 9 then
        RandomValue := g_Config.nSpiritualismRates[9]
      else
        RandomValue := g_Config.nSpiritualismRates[3 + nMagicNewLevel]
    end
    else
    begin
      if nMagicLevel > 3 then
        RandomValue := g_Config.nSpiritualismRates[3]
      else if nMagicLevel < 0 then
        RandomValue := g_Config.nSpiritualismRates[0]
      else
        RandomValue := g_Config.nSpiritualismRates[nMagicLevel]
    end;

    if Random(RandomValue) = 0 then
    begin
      Result := True;
      TargeTBaseObject.ReAlive(True);
      TargeTBaseObject.m_Master := BaseObject;
      TargeTBaseObject.m_dwMasterRoyaltyStartTick := MyGetTickCount;
      TargeTBaseObject.m_dwMasterRoyaltyTime := LongWord((Random(BaseObject.m_Abil.Level * 2) + (nMagicLevel shl 2) * 5 + g_Config.nSpiritualismRoyaltyTime)
        * 60 * 1000);
      TargeTBaseObject.m_btSlaveMakeLevel := nMagicLevel;

      if TargeTBaseObject.m_dwMasterTick = 0 then
        TargeTBaseObject.m_dwMasterTick := MyGetTickCount();

      TargeTBaseObject.BreakHolySeizeMode();

      if LongWord(1500 - nMagicLevel * 200) < LongWord(TargeTBaseObject.m_nWalkSpeed) then
        TargeTBaseObject.m_nWalkSpeed := 1500 - nMagicLevel * 200;

      if LongWord(2000 - nMagicLevel * 200) < LongWord(TargeTBaseObject.m_nNextHitTime) then
        TargeTBaseObject.m_nNextHitTime := 2000 - nMagicLevel * 200;

      TargeTBaseObject.RefShowName();
      TargeTBaseObject.m_wCallSkill := SKILL_91;
      BaseObject.m_SlaveList.Add(TargeTBaseObject);

      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) or ((BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer =
        RC_PLAYOBJECT)) then
        TargeTBaseObject.RefHumsBB(True);
    end
    else if Random(20) = 0 then
      TargeTBaseObject.MakeGhost;
  end;
end;

function TMagicManager.MagTurnUndead(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nLevel: Integer):
  Boolean;
var
  n14: Integer;
begin
  Result := False;
  if TargeTBaseObject.m_boSuperMan or not (TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD) then
    Exit;

  TAnimalObject(TargeTBaseObject).Struck { FFEC } (BaseObject);
  if TargeTBaseObject.m_TargetCret = nil then
  begin
    TAnimalObject(TargeTBaseObject).m_boRunAwayMode := True;
    TAnimalObject(TargeTBaseObject).m_dwRunAwayStart := MyGetTickCount();
    TAnimalObject(TargeTBaseObject).m_dwRunAwayTime := 10 * 1000;
  end;
  BaseObject.SetTargetCreat(TargeTBaseObject);

  // 圣言术同等级攻击 piaoyun 2013-09-16
  if ((Random(2) + (BaseObject.m_Abil.Level - 1)) > TargeTBaseObject.m_Abil.Level) or (g_Config.boMagTurnUndeadSameLevel and ((Random
    (2) + (BaseObject.m_Abil.Level - 1)) >= TargeTBaseObject.m_Abil.Level)) then
  begin
    if TargeTBaseObject.m_Abil.Level < g_Config.nMagTurnUndeadLevel then
    begin
      n14 := BaseObject.m_Abil.Level - TargeTBaseObject.m_Abil.Level;
      if Random(100) < ((nLevel shl 3) - nLevel + 15 + n14) then
      begin
        TargeTBaseObject.SetLastHiter(BaseObject);
        TargeTBaseObject.m_WAbil.HP := 0;
        Result := True;
      end
    end;
  end;
end;

function TMagicManager.MagWindTebo(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
var
  PoseBaseObject: TBaseObject;
begin
  Result := False;
  PoseBaseObject := PlayObject.GetPoseCreate;
  if (PoseBaseObject <> nil) and (PoseBaseObject <> PlayObject) and (not PoseBaseObject.m_boDeath) and (not PoseBaseObject.m_boGhost)
    and (PlayObject.IsProperTarget(PoseBaseObject)) and (not PoseBaseObject.m_boStickMode) then
  begin
    if (abs(PlayObject.m_nCurrX - PoseBaseObject.m_nCurrX) <= 1) and (abs(PlayObject.m_nCurrY - PoseBaseObject.m_nCurrY) <= 1) and
      (PlayObject.m_Abil.Level > PoseBaseObject.m_Abil.Level) then
    begin
      if Random(20) < UserMagic.btLevel * 6 + 6 + (PlayObject.m_Abil.Level - PoseBaseObject.m_Abil.Level) then
      begin
        PoseBaseObject.CharPushed(GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, PoseBaseObject.m_nCurrX,
          PoseBaseObject.m_nCurrY), _MAX(0, UserMagic.btLevel - 1) + 1);
        Result := True;
      end;
    end;
  end;
end;

function TMagicManager.MagSaceMove(BaseObject: TSmartObject; nLevel: Integer): Boolean;
var
  Envir: TEnvirnoment;
  PlayObject: TPlayObject;
begin
  Result := False;
  if BaseObject.m_boImprison then
    Exit;

  if Random(11) < nLevel * 2 + 4 then
  begin
    BaseObject.SendRefMsg(RM_SPACEMOVE_FIRE2, 0, 0, 0, 0, '');
    if BaseObject is TPlayObject then
    begin
      Envir := BaseObject.m_PEnvir;
      BaseObject.MapRandomMove(BaseObject.m_sHomeMap, 1);
      if (Envir <> BaseObject.m_PEnvir) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
      begin
        PlayObject := TPlayObject(BaseObject);
        PlayObject.m_boTimeRecall := False;
      end;
    end;
    Result := True;
  end;
end;

// 群体施毒术
function TMagicManager.MagGroupAmyounsul(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject; var boSpellFail: Boolean): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower: Integer;
  nType, nAmuletIdx: Integer;
  boCanUseMagic: Boolean;
begin
  Result := False;
  boSpellFail := False;
  boCanUseMagic := False;
  if (not g_Config.boSkillGroupAmyounsulRed) and (not g_Config.boSkillGroupAmyounsulGreen) then
    Exit;

  nType := 2;
  if CheckAmulet(PlayObject, nType, 5, nAmuletIdx) or CheckBagAmulet(PlayObject, nType, 5, nAmuletIdx) then
  begin
    if nAmuletIdx < 0 then
    begin
      // 不需要毒，根据怪物中毒类型自动选择不同的下毒类型
      boCanUseMagic := True;
    end
    else if (nAmuletIdx in [Low(THumanUseItems)..High(THumanUseItems)]) then
    begin // 佩戴的毒符
      UseAmulet(PlayObject, 5, nAmuletIdx);
      boCanUseMagic := True;
    end
    else // 包裹的毒符
    begin
      if TargeTBaseObject <> nil then
      begin
        if (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) then
          nType := 1
        else if (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0) then
          nType := 2
        else
          nType := 1;

        if (not g_Config.boSkillGroupAmyounsulRed) or (not g_Config.boSkillGroupAmyounsulGreen) then
        begin
          if (g_Config.boSkillGroupAmyounsulGreen) then
            nType := 1
          else
            nType := 2;
        end;
      end
      else
      begin
        nType := 1;
        if (not g_Config.boSkillGroupAmyounsulRed) or (not g_Config.boSkillGroupAmyounsulGreen) then
        begin
          if (g_Config.boSkillGroupAmyounsulGreen) then
            nType := 1
          else
            nType := 2;
        end;
      end;

      if CheckBagAmulet(PlayObject, nType, 5, nAmuletIdx, True) then
      begin
        if (nAmuletIdx - Length(PlayObject.m_UseItems) >= 0) and (nAmuletIdx - Length(PlayObject.m_UseItems) < PlayObject.m_ItemList.Count)
          then
        begin
          UseAmulet(PlayObject, 5, nAmuletIdx);
          boCanUseMagic := True;
        end;
      end
      else
      begin
        if nType = 1 then
          nType := 2
        else
          nType := 1;

        if (not g_Config.boSkillGroupAmyounsulRed) or (not g_Config.boSkillGroupAmyounsulGreen) then
        begin
          if (g_Config.boSkillGroupAmyounsulGreen) then
            nType := 1
          else
            nType := 2;
        end;

        if CheckBagAmulet(PlayObject, nType, 5, nAmuletIdx, True) then
        begin
          if (nAmuletIdx - Length(PlayObject.m_UseItems) >= 0) and (nAmuletIdx - Length(PlayObject.m_UseItems) < PlayObject.m_ItemList.Count)
            then
          begin
            UseAmulet(PlayObject, 5, nAmuletIdx);
            boCanUseMagic := True;
          end;
        end;
      end;
    end;

    if boCanUseMagic then
    begin
      BaseObjectList := TList.Create;
      PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, nTargetX, nTargetY, _MAX(1, UserMagic.btLevel), BaseObjectList);
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        BaseObject := TBaseObject(BaseObjectList.Items[I]);
        if BaseObject.m_boDeath or (BaseObject.m_boGhost) or (PlayObject = BaseObject) then
          Continue;

        if PlayObject.IsProperTarget(BaseObject) then
        begin
          if nAmuletIdx < 0 then
          begin
            // 不需要毒，根据怪物中毒类型自动选择不同的下毒类型
            if Random(BaseObject.m_btAntiPoison + 7) <= 6 then
            begin
              if (BaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) and g_Config.boSkillGroupAmyounsulGreen then
              begin
                if Random(BaseObject.m_btAntiPoison + 7) <= 6 then
                begin
                  nPower := GetPower13(PlayObject, 40, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                    * 2;

                  BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , Min(GetNewLevelPower(Round(nPower
                    * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                    GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                    1000);

                  BaseObject.SetLastHiter(PlayObject);
                  PlayObject.SetTargetCreat(BaseObject);
                  Result := True;
                end;
              end
              else if (BaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0) and g_Config.boSkillGroupAmyounsulRed then
              begin
                if Random(BaseObject.m_btAntiPoison + 7) <= 6 then
                begin
                  nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                    * 2;

                  BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DAMAGEARMOR { 中毒类型 - 红毒 } , Min(GetNewLevelPower(Round(nPower
                    * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                    GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                    1000);

                  BaseObject.SetLastHiter(PlayObject);
                  PlayObject.SetTargetCreat(BaseObject);
                  Result := True;
                end;
              end
              else
              begin
                if Random(BaseObject.m_btAntiPoison + 7) <= 6 then
                begin
                  nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                    * 2;
                  if (not g_Config.boSkillGroupAmyounsulRed) or (not g_Config.boSkillGroupAmyounsulGreen) then
                  begin
                    if (g_Config.boSkillGroupAmyounsulGreen) then
                    begin
                      BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH, Min(GetNewLevelPower(Round(nPower * (g_Config.nAmyOunsulTimeRate
                        / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject), GetNewLevelPower(Round(UserMagic.btLevel
                        / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx), 1000);
                    end
                    else
                    begin
                      BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DAMAGEARMOR, Min(GetNewLevelPower(Round(nPower * (g_Config.nAmyOunsulTimeRate
                        / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject), GetNewLevelPower(Round(UserMagic.btLevel
                        / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx), 1000);
                    end;
                  end
                  else
                  begin
                    BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH + Random(1) { 中毒类型 } , Min(GetNewLevelPower(Round
                      (nPower * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                      GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                      1000);
                  end;
                  BaseObject.SetLastHiter(PlayObject);
                  PlayObject.SetTargetCreat(BaseObject);
                  Result := True;
                end;
              end;
            end;
          end
          else
          begin // 包裹的毒符
            if Random(BaseObject.m_btAntiPoison + 7) <= 6 then
            begin
              case nType of
                1:
                  begin
                    if g_Config.boSkillGroupAmyounsulGreen then
                    begin
                      nPower := GetPower13(PlayObject, 40, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                        * 2;

                      BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , Min(GetNewLevelPower(Round(nPower
                        * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                        GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                        1000);
                    end
                    else
                      Exit;
                  end;
                2:
                  begin
                    if g_Config.boSkillGroupAmyounsulRed then
                    begin
                      nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                        * 2;

                      BaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DAMAGEARMOR { 中毒类型 - 红毒 } , Min(GetNewLevelPower(Round
                        (nPower * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                        GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                        1000);
                    end
                    else
                      Exit;
                  end;
              end;
              BaseObject.SetLastHiter(PlayObject);
              PlayObject.SetTargetCreat(BaseObject);
              Result := True;
            end;
          end;
        end;
      end;
      BaseObjectList.Free;
    end;
  end;
  boSpellFail := not Result;
end;

// 彻地钉技能特殊处理方式 --- piaoyun 2013-06-23
function TMagicManager.MagGroupDeDing(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower: Integer;
  nRage: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  nRage := Min(g_Config.nDeDingMagicAttackRange + UserMagic.btLevel, g_Config.nDeDingMagicAttackRange);
  BaseObjectList := TList.Create;
  try
    PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, nTargetX, nTargetY, { _MAX(1, UserMagic.btLevel) } nRage, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if BaseObject.m_boDeath or (BaseObject.m_boGhost) or (PlayObject = BaseObject) then
        Continue;
      // 彻地钉禁止PK chongchong 2015-06-10
      if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and g_Config.boDedingDisabledPK then
        Continue;

      if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) and g_Config.boDedingDisabledPK then
        Continue;

      if PlayObject.IsProperTarget(BaseObject) then
      begin
        nPower := PlayObject.GetAttackPower(PlayObject.m_WAbil.DC1, PlayObject.m_WAbil.DC2 - PlayObject.m_WAbil.DC1);
        if nPower > 0 then
        begin
          if not PlayObject.CanCloseDefense then // 忽视目标防御
            nPower := BaseObject.GetHitStruckDamage(PlayObject, nPower, nil)
          else
            nPower := BaseObject.GetHitStruckDamage(PlayObject, nPower, nil, 4);

          // 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37
          nPower := BaseObject.NewAbilPower(2, nPower);
        end;
        nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
        // 伤害吸收 chongchong 2016-05-12
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(BaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower := Max(0, nPower - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
          }
        end;

        nPower := GetSkillLastPowerNG(nPower, GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(BaseObject,
          UserMagic, nPower));

        if nPower > 0 then
        begin
          nPower := Round(nPower / 100 * g_Config.nDeDingMagicBasicPowerRate);
          if FNpcReleaseMagic <> 0 then
            PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 1,
              NativeInt(BaseObject), IntToStr(UserMagic.wMagIdx))
          else
            PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 1,
              NativeInt(BaseObject), IntToStr(UserMagic.wMagIdx), 200);
        end;
        Result := True;
      end;
      PlayObject.SendRefMsg(RM_10205, 0, BaseObject.m_nCurrX, BaseObject.m_nCurrY, 1, '');
    end;
  finally
    BaseObjectList.Free;
  end;
end;

function TMagicManager.MagGroupLightening(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject; var boSpellFire: Boolean): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower: Integer;
  SmartObject: TSmartObject;
  Range: Integer;
begin
  Result := False;
  boSpellFire := False;
  BaseObjectList := TList.Create;

  if UserMagic.btLevel <= 0 then
    Range := g_Config.nSkill37Range
  else
    Range := g_Config.nSkill37Range + UserMagic.btLevel * g_Config.nSkill37RangeAdd;

  PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, nTargetX, nTargetY, Range, BaseObjectList);
  PlayObject.SendRefMsg(RM_MAGICFIRE, 0, MakeWord(UserMagic.MagicInfo.btEffectType, UserMagic.MagicInfo.btEffect), MakeLong(nTargetX,
    nTargetY), NativeInt(TargeTBaseObject), '');

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    BaseObject := TBaseObject(BaseObjectList.Items[I]);
    if BaseObject.m_boDeath or (BaseObject.m_boGhost) or (PlayObject = BaseObject) then
      Continue;

    if PlayObject.IsProperTarget(BaseObject) then
    begin
      if (Random(10) >= BaseObject.m_nAntiMagic) then
      begin
        nPower := PlayObject.GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + PlayObject.m_WAbil.MC1,
          PlayObject.m_WAbil.MC2 - PlayObject.m_WAbil.MC1 + 1);
        if BaseObject.m_btLifeAttrib = LA_UNDEAD then
          nPower := Min(LongWord(Round(nPower * 1.5)), High(Integer));
        nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
        nPower := Round(nPower / 100 * g_Config.nSkillGroupLighteningPowerRate);

        // 伤害吸收 chongchong 2016-05-12
        if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(BaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower := Max(0, nPower - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
          }
        end;

        nPower := GetSkillLastPowerNG(nPower, // 内功技能威力
          GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(BaseObject, UserMagic, nPower));

        if FNpcReleaseMagic <> 0 then
          PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 2,
            NativeInt(BaseObject), IntToStr(UserMagic.wMagIdx))
        else
          PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 2,
            NativeInt(BaseObject), IntToStr(UserMagic.wMagIdx), 200); // 群雷术技能延时
        Result := True;
      end
      else
      begin
        if PlayObject.m_btRaceServer = RC_PLAYOBJECT then
          PlayObject.SendMsg(BaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
      end;

      if (BaseObject.m_nCurrX <> nTargetX) or (BaseObject.m_nCurrY <> nTargetY) then
        PlayObject.SendRefMsg(RM_10205, 0, BaseObject.m_nCurrX, BaseObject.m_nCurrY, 4 { type } , '');
    end;
  end;
  BaseObjectList.Free;
  boSpellFire := not Result;
end;

function TMagicManager.CheckMagLighteningType(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Byte;
{ 检测毒时，用完全匹配检测 }

  function CheckAmuletEx(BaseObject: TSmartObject; nShape, nCount: Integer): Boolean;
  begin
    Result := CheckUserItemTypeEx(BaseObject, nShape, nCount) or (GetUserItemListEx(BaseObject, nShape, nCount) >= 0);
  end;

var
  nType, nAmuletIdx: Integer;
  nCount: Integer;
begin
  Result := 0;
  if (UserMagic.wMagIdx = SKILL_AMYOUNSUL) then
    nCount := 1
  else
    nCount := 5;

  if (UserMagic.wMagIdx in [SKILL_AMYOUNSUL, SKILL_GROUPAMYOUNSUL]) and (TargeTBaseObject <> nil) and PlayObject.IsProperTarget(TargeTBaseObject)
    then
  begin
    nType := 2;
    { TODO -ochongchong -c修改 : 人物毒粉用完后为分开提示 【2013-08-24】 }
    if (PlayObject.m_btRaceServer = RC_PLAYOBJECT) and (g_Config.nHumNeedMagicItem <> 0)
    { and (MyGetTickCount - PlayObject.m_dwAmuHintTick > 10000) } then
    begin
      // 使用装备的毒，装备一种毒，客户端不勾选红绿毒互换，当没有另一种毒没有时不提示 chongchong 2014-10-09
      if (g_Config.nHumNeedMagicItem = 1) and not TPlayObject(PlayObject).m_boAutoCHangePoison and (CheckUserItemTypeEx(PlayObject,
        2, nCount) or CheckUserItemTypeEx(PlayObject, 1, nCount)) then
      begin
      end
      else
      begin
        if not CheckAmuletEx(PlayObject, 2, nCount) then // 红毒
        begin
          if Length(sNeedRedPoison) > 0 then
            PlayObject.SendMsg(PlayObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sNeedRedPoison);

          PlayObject.m_dwAmuHintTick := MyGetTickCount;
        end;

        if not CheckAmuletEx(PlayObject, 1, nCount) then // 绿毒
        begin
          if Length(sNeedGreenPoison) > 0 then
            PlayObject.SendMsg(PlayObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sNeedGreenPoison);

          PlayObject.m_dwAmuHintTick := MyGetTickCount;
        end;
      end;
    end;

    if CheckAmulet(PlayObject, nType, nCount, nAmuletIdx) or CheckBagAmulet(PlayObject, nType, nCount, nAmuletIdx) then
    begin
      if nAmuletIdx < 0 then
      begin
        // 不需要毒，根据怪物中毒类型自动选择不同的下毒类型
        if (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) then
        begin
          Result := 0;
        end
        else if (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0) then
        begin
          Result := 1;
        end
        else
        begin
          Result := 0;
        end;
      end
      else if (nAmuletIdx in [Low(THumanUseItems)..High(THumanUseItems)]) then // 佩戴的毒符
      begin
        case nType of
          1:
            begin
              Result := 0;
            end;
          2:
            begin
              Result := 1;
            end;
        end;
      end
      else
      begin
        if (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0) then
          nType := 1
        else if (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0) then
          nType := 2
        else
          nType := 1;
        if CheckBagAmulet(PlayObject, nType, nCount, nAmuletIdx, True) then
        begin
          if (nAmuletIdx - Length(PlayObject.m_UseItems) >= 0) and (nAmuletIdx - Length(PlayObject.m_UseItems) < PlayObject.m_ItemList.Count)
            then
          begin
            case nType of
              1:
                begin
                  Result := 0;
                end;
              2:
                begin
                  Result := 1;
                end;
            end;
          end;
        end;
      end;
    end
    else
    begin
      { TODO -ochongchong -c修改 : 人物毒粉用完后为分开提示 【2013-08-24】 }
      if PlayObject.m_btRaceServer <> RC_PLAYOBJECT then
      begin
        // 红毒
        if (not CheckAmuletEx(PlayObject, 2, nCount)) and (Length(sNeedRedPoison) > 0) then
          PlayObject.SendMsg(PlayObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sNeedRedPoison);

        // 绿毒
        if (not CheckAmuletEx(PlayObject, 1, nCount)) and (Length(sNeedGreenPoison) > 0) then
          PlayObject.SendMsg(PlayObject, RM_MAGIC_HINT_MSG, 0, 0, 0, 0, sNeedGreenPoison);
      end;
    end;
  end;
end;

// 施毒术
function TMagicManager.MagLightening(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject; var boSpellFire: Boolean): Boolean;
var
  nPower: Integer;
  nType, nAmuletIdx: Integer;
  StdItemIndex1, StdItemIndex2: Integer;
begin
  Result := False;

  if (TargeTBaseObject <> nil) and PlayObject.IsProperTarget(TargeTBaseObject) then
  begin
    nType := 2;

    if (PlayObject.m_btRaceServer = RC_HEROOBJECT) //
      and g_Config.boHeroTaosAutoChangePoison //
      and CheckAmulet(PlayObject, nType, 1, nAmuletIdx) then
    begin
      StdItemIndex1 := GetUserItemListEx(PlayObject, 1, 1); // 绿毒
      StdItemIndex2 := GetUserItemListEx(PlayObject, 2, 1); // 红毒

      // 攻击的对象中了红毒，装备的是红毒，没中绿毒
      if (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <> 0) // 中了红毒
        and (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] = 0) // 没中绿毒
        and (nType = 2) // 装备红毒
        and (StdItemIndex1 >= 0) then // 包裹中有绿毒
      begin
        if (MyGetTickCount - THeroObject(PlayObject).m_dwLastChangePoison) > 3000 then
        begin
          if PlayObject.m_Master <> nil then
            TPlayObject(PlayObject.m_Master).SendDefMessage(SM_CHANGEPOISON, 0, 0, 0, 0, '');

          THeroObject(PlayObject).m_dwLastChangePoison := MyGetTickCount;
        end;
      end
      // 攻击的对象中了绿毒，装备的是绿毒，没中红毒
      else if (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0) // 没中红毒
        and (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <> 0) // 中了绿毒
        and (nType = 1) // 装备绿毒
        and (StdItemIndex2 >= 0) then // 包裹中有红毒
      begin
        // 换成红毒
        if (MyGetTickCount - THeroObject(PlayObject).m_dwLastChangePoison) > 3000 then
        begin
          if PlayObject.m_Master <> nil then
            TPlayObject(PlayObject.m_Master).SendDefMessage(SM_CHANGEPOISON, 0, 0, 0, 0, '');

          THeroObject(PlayObject).m_dwLastChangePoison := MyGetTickCount;
        end;
      end;
    end;

    if CheckAmulet(PlayObject, nType, 1, nAmuletIdx) or CheckBagAmulet(PlayObject, nType, 1, nAmuletIdx) then
    begin
      if nAmuletIdx < 0 then
      begin
        // 不需要毒，根据怪物中毒类型自动选择不同的下毒类型
        if Random(TargeTBaseObject.m_btAntiPoison + 7) <= 6 then
        begin
          if TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0 then
          begin
            if Random(TargeTBaseObject.m_btAntiPoison + 7) <= 6 then
            begin
              nPower := Min(LongWord(GetPower13(PlayObject, 40, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1,
                PlayObject.m_WAbil.SC2) * 2), High(Integer));

              TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , Min(GetNewLevelPower(Round(nPower
                * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                400);

              TargeTBaseObject.SetLastHiter(PlayObject);
              PlayObject.SetTargetCreat(TargeTBaseObject);
              Result := True;
            end;
          end
          else if TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0 then
          begin
            if Random(TargeTBaseObject.m_btAntiPoison + 7) <= 6 then
            begin
              nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                * 2;

              TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DAMAGEARMOR { 中毒类型 - 红毒 } , Min(GetNewLevelPower(Round(nPower
                * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                400);

              TargeTBaseObject.SetLastHiter(PlayObject);
              PlayObject.SetTargetCreat(TargeTBaseObject);
              Result := True;
            end;
          end
          else
          begin
            if Random(TargeTBaseObject.m_btAntiPoison + 7) <= 6 then
            begin
              nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                * 2;

              TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH + Random(1) { 中毒类型 } , Min(GetNewLevelPower(Round
                (nPower * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                400);

              TargeTBaseObject.SetLastHiter(PlayObject);
              PlayObject.SetTargetCreat(TargeTBaseObject);
              Result := True;
            end;
          end;
        end;
      end
      else if (nAmuletIdx in [Low(THumanUseItems)..High(THumanUseItems)]) then // 佩戴的毒符
      begin
        UseAmulet(PlayObject, 1, nAmuletIdx);
        if Random(TargeTBaseObject.m_btAntiPoison + 7) <= 6 then
        begin
          case nType of
            1:
              begin
                nPower := GetPower13(PlayObject, 40, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                  * 2;

                TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , Min(GetNewLevelPower(Round(nPower
                  * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                  GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                  400);
              end;
            2:
              begin
                nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                  * 2;

                TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DAMAGEARMOR { 中毒类型 - 红毒 } , Min(GetNewLevelPower(Round
                  (nPower * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                  GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                  400);
              end;
          end;
          TargeTBaseObject.SetLastHiter(PlayObject);
          PlayObject.SetTargetCreat(TargeTBaseObject);
          Result := True;
        end;
      end
      else
      begin // 包裹的毒符
        if TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <= 0 then
          nType := 1
        else if TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0 then
          nType := 2
        else
          nType := 1 + Random(2); // 使用包裹中的毒符时，随机用一个 chongchong 2014-09-14

        if CheckBagAmulet(PlayObject, nType, 1, nAmuletIdx, True) then
        begin
          if (nAmuletIdx - Length(PlayObject.m_UseItems) >= 0) //
            and (nAmuletIdx - Length(PlayObject.m_UseItems) < PlayObject.m_ItemList.Count) then
          begin
            UseAmulet(PlayObject, 1, nAmuletIdx);
            if Random(TargeTBaseObject.m_btAntiPoison + 7) <= 6 then
            begin
              case nType of
                1:
                  begin
                    nPower := GetPower13(PlayObject, 40, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                      * 2;

                    TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , Min(GetNewLevelPower(Round
                      (nPower * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                      GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                      400);
                  end;
                2:
                  begin
                    nPower := GetPower13(PlayObject, 30, UserMagic) + GetRPow(PlayObject, PlayObject.m_WAbil.SC1, PlayObject.m_WAbil.SC2)
                      * 2;

                    TargeTBaseObject.SendDelayMsg(PlayObject, RM_POISON, POISON_DAMAGEARMOR { 中毒类型 - 红毒 } , Min(GetNewLevelPower(Round
                      (nPower * (g_Config.nAmyOunsulTimeRate / 100)), UserMagic), g_Config.nAmyOunsulMaxTime), NativeInt(PlayObject),
                      GetNewLevelPower(Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint)), UserMagic), IntToStr(UserMagic.wMagIdx),
                      400);
                  end;
              end;

              TargeTBaseObject.SetLastHiter(PlayObject);
              PlayObject.SetTargetCreat(TargeTBaseObject);
              Result := True;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function TMagicManager.MagHbFireBall(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  nDir: Integer;
  levelgap: Integer;
  push: Integer;
begin
  Result := False;

  if not PlayObject.MagCanHitTarget(PlayObject.m_nCurrX, PlayObject.m_nCurrY, TargeTBaseObject) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  if not PlayObject.IsProperTarget(TargeTBaseObject) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  if (abs(TargeTBaseObject.m_nCurrX - nTargetX) > 1) or (abs(TargeTBaseObject.m_nCurrY - nTargetY) > 1) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  if (TargeTBaseObject.m_nAntiMagic > Random(10)) then
  begin
    if PlayObject.m_btRaceServer = RC_PLAYOBJECT then
      PlayObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');

    TargeTBaseObject := nil;
    Exit;
  end;

  with PlayObject do
  begin
    nPower := GetAttackPower(GetPower(PlayObject, MPow(PlayObject, UserMagic), UserMagic) + m_WAbil.MC1, m_WAbil.MC2 - m_WAbil.MC1
      + 1);
  end;

  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := GetSkillLastPowerNG(nPower, // 内功技能威力
    GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));

  if FNpcReleaseMagic <> 0 then
    PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
      IntToStr(UserMagic.wMagIdx))
  else
    PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
      IntToStr(UserMagic.wMagIdx), 500); // 寒冰掌技能延时

  if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
    Result := True;

  if (PlayObject.m_Abil.Level > TargeTBaseObject.m_Abil.Level) and (not TargeTBaseObject.m_boStickMode) then
  begin
    levelgap := PlayObject.m_Abil.Level - TargeTBaseObject.m_Abil.Level;

    if (Random(20) < 6 + UserMagic.btLevel * 3 + levelgap) then
    begin
      push := Random(UserMagic.btLevel) - 1;
      if push > 0 then
      begin
        nDir := GetNextDirection(PlayObject.m_nCurrX, PlayObject.m_nCurrY, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY);
        PlayObject.SendDelayMsg(PlayObject, RM_DELAYPUSHED, nDir, MakeLong(nTargetX, nTargetY), push, NativeInt(TargeTBaseObject),
          '', 500);
      end;
      Result := True;
    end;
  end;
end;

// 产生任意形状的火
function TMagicManager.MagMakeSuperFireCross(PlayObject: TSmartObject; nDamage, nHTime, nX, nY: Integer; nCount: Integer): Integer;

  function MagMakeSuperFireCrossOfDir(btDir: Integer): Integer;
  var
    FireBurnEvent: TFireBurnEvent;
    I, X, Y: Integer;
    nTime: Integer;
  begin
    nTime := 1;
    case btDir of
      DR_UP:
        begin
          for Y := PlayObject.m_nCurrY downto PlayObject.m_nCurrY - 10 do
          begin
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, PlayObject.m_nCurrX, Y, ET_FIRE, nHTime * nTime, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_UPRIGHT:
        begin
          for I := 0 to 6 do
          begin
            X := PlayObject.m_nCurrX + I;
            Y := PlayObject.m_nCurrY - I;
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, X, Y, ET_FIRE, nHTime * nTime * 2, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_RIGHT:
        begin
          for X := PlayObject.m_nCurrX to PlayObject.m_nCurrX + 10 do
          begin
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, X, PlayObject.m_nCurrY, ET_FIRE, nHTime * nTime, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_DOWNRIGHT:
        begin
          for I := 0 to 6 do
          begin
            X := PlayObject.m_nCurrX + I;
            Y := PlayObject.m_nCurrY + I;
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, X, Y, ET_FIRE, nHTime * nTime * 2, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_DOWN:
        begin
          for Y := PlayObject.m_nCurrY to PlayObject.m_nCurrY + 10 do
          begin
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, PlayObject.m_nCurrX, Y, ET_FIRE, nHTime * nTime, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_DOWNLEFT:
        begin
          for I := 0 to 6 do
          begin
            X := PlayObject.m_nCurrX - I;
            Y := PlayObject.m_nCurrY + I;
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, X, Y, ET_FIRE, nHTime * nTime * 2, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_LEFT:
        begin
          for X := PlayObject.m_nCurrX downto PlayObject.m_nCurrX - 10 do
          begin
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, X, PlayObject.m_nCurrY, ET_FIRE, nHTime * nTime, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
      DR_UPLEFT:
        begin
          for I := 0 to 6 do
          begin
            X := PlayObject.m_nCurrX - I;
            Y := PlayObject.m_nCurrY - I;
            FireBurnEvent := TFireBurnEvent.Create(PlayObject, X, Y, ET_FIRE, nHTime * nTime * 2, nDamage);
            g_EventManager.AddEvent(FireBurnEvent);
            Inc(nTime);
          end;
        end;
    end;
    Result := 1;
  end;

var
  I: Integer;
begin
  Result := 0;
  case nCount of
    1:
      begin
        Result := MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
      end;
    3:
      begin
        case PlayObject.m_btDirection of
          DR_UP:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_UPRIGHT);
              Result := MagMakeSuperFireCrossOfDir(DR_UPLEFT);
            end;
          DR_UPRIGHT:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_UP);
              Result := MagMakeSuperFireCrossOfDir(DR_RIGHT);
            end;
          DR_RIGHT:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_UPRIGHT);
              Result := MagMakeSuperFireCrossOfDir(DR_DOWNRIGHT);
            end;
          DR_DOWNRIGHT:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_RIGHT);
              Result := MagMakeSuperFireCrossOfDir(DR_DOWN);
            end;
          DR_DOWN:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_DOWNLEFT);
              Result := MagMakeSuperFireCrossOfDir(DR_DOWNRIGHT);
            end;
          DR_DOWNLEFT:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_DOWN);
              Result := MagMakeSuperFireCrossOfDir(DR_LEFT);
            end;
          DR_LEFT:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_DOWNLEFT);
              Result := MagMakeSuperFireCrossOfDir(DR_UPLEFT);
            end;
          DR_UPLEFT:
            begin
              MagMakeSuperFireCrossOfDir(PlayObject.m_btDirection);
              MagMakeSuperFireCrossOfDir(DR_LEFT);
              Result := MagMakeSuperFireCrossOfDir(DR_UP);
            end;
        end;
      end;
    4:
      begin
        MagMakeSuperFireCrossOfDir(DR_UP);
        MagMakeSuperFireCrossOfDir(DR_LEFT);
        MagMakeSuperFireCrossOfDir(DR_DOWN);
        Result := MagMakeSuperFireCrossOfDir(DR_RIGHT);
      end;
    5:
      begin
        case PlayObject.m_btDirection of
          DR_UP, DR_UPLEFT, DR_UPRIGHT:
            begin
              MagMakeSuperFireCrossOfDir(DR_UP);
              MagMakeSuperFireCrossOfDir(DR_UPRIGHT);
              MagMakeSuperFireCrossOfDir(DR_UPLEFT);
              MagMakeSuperFireCrossOfDir(DR_LEFT);
              Result := MagMakeSuperFireCrossOfDir(DR_RIGHT);
            end;
          DR_LEFT:
            begin
              MagMakeSuperFireCrossOfDir(DR_UP);
              MagMakeSuperFireCrossOfDir(DR_DOWN);
              MagMakeSuperFireCrossOfDir(DR_UPLEFT);
              MagMakeSuperFireCrossOfDir(DR_LEFT);
              Result := MagMakeSuperFireCrossOfDir(DR_DOWNLEFT);
            end;
          DR_RIGHT:
            begin
              MagMakeSuperFireCrossOfDir(DR_UP);
              MagMakeSuperFireCrossOfDir(DR_DOWN);
              MagMakeSuperFireCrossOfDir(DR_UPRIGHT);
              MagMakeSuperFireCrossOfDir(DR_RIGHT);
              Result := MagMakeSuperFireCrossOfDir(DR_DOWNRIGHT);
            end;
          DR_DOWN, DR_DOWNLEFT, DR_DOWNRIGHT:
            begin
              MagMakeSuperFireCrossOfDir(DR_DOWN);
              MagMakeSuperFireCrossOfDir(DR_DOWNRIGHT);
              MagMakeSuperFireCrossOfDir(DR_DOWNLEFT);
              MagMakeSuperFireCrossOfDir(DR_LEFT);
              Result := MagMakeSuperFireCrossOfDir(DR_RIGHT);
            end;
        end;
      end;
    8:
      begin
        for I := DR_UP to DR_UPLEFT do
          Result := MagMakeSuperFireCrossOfDir(I);
      end;
  end;
end;

// 火墙
function TMagicManager.MagMakeFireCross(PlayObject: TSmartObject; nDamage, nHTime, nX, nY, nNewLevel: Integer): Integer;
var
  nType: Integer;
  FireBurnEvent: TFireBurnEvent;
resourcestring
  sDisableInSafeZoneFireCross = '安全区不允许使用.';
  sDisableInSafeZoneFireCross1 = '当前地图不允许使用.';
begin
  Result := 0;
  if g_Config.boDisableInSafeZoneFireCross then
  begin
    if PlayObject.InSafeZone(PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY) then
    begin
      PlayObject.SysMsg(sDisableInSafeZoneFireCross, c_Red, t_Notice);
      Exit;
    end;

    if PlayObject.InSafeZone(PlayObject.m_PEnvir, nX, nY) then
    begin
      PlayObject.SysMsg(sDisableInSafeZoneFireCross, c_Red, t_Notice);
      Exit;
    end;
  end;

  if PlayObject.m_PEnvir.m_boUnAllowFireMagic then
  begin
    PlayObject.SysMsg(sDisableInSafeZoneFireCross1, c_Red, t_Notice);
    Exit;
  end;

  case nNewLevel of
    0:
      nType := ET_FIRE;
    1..3:
      nType := ET_FIRELevel1;
    4..6:
      nType := ET_FIRELevel2;
  else
    nType := ET_FIRELevel3;
  end;

  nHTime := Min(nHTime, g_Config.nFireCrossMaxTime * 60);
  nDamage := Min(LongWord(Round(nDamage * (g_Config.nFireCrossPowerRate / 100))), High(Integer));

  if PlayObject.m_PEnvir.GetEvent(nX, nY - 1) = nil then
  begin
    FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX, nY - 1, nType, nHTime * 1000, nDamage);
    g_EventManager.AddEvent(FireBurnEvent);
  end; // 0492CFC

  if PlayObject.m_PEnvir.GetEvent(nX - 1, nY) = nil then
  begin
    FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX - 1, nY, nType, nHTime * 1000, nDamage);
    g_EventManager.AddEvent(FireBurnEvent);
  end; // 0492D4D

  if PlayObject.m_PEnvir.GetEvent(nX, nY) = nil then
  begin
    FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX, nY, nType, nHTime * 1000, nDamage);
    g_EventManager.AddEvent(FireBurnEvent);
  end; // 00492D9C

  if PlayObject.m_PEnvir.GetEvent(nX + 1, nY) = nil then
  begin
    FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX + 1, nY, nType, nHTime * 1000, nDamage);
    g_EventManager.AddEvent(FireBurnEvent);
  end; // 00492DED

  if PlayObject.m_PEnvir.GetEvent(nX, nY + 1) = nil then
  begin
    FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX, nY + 1, nType, nHTime * 1000, nDamage);
    g_EventManager.AddEvent(FireBurnEvent);
  end; // 00492E3E
  Result := 1;
end;

// 爆裂火焰技能函数修改 piaoyun 2013-07-25
function TMagicManager.MagBigExplosion(BaseObject: TSmartObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer; nRage, PowerRate:
  Integer): Boolean;
var
  I, nPowerNG: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
begin
  Result := False;
  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      BaseObject.SetTargetCreat(TargeTBaseObject);
      nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
        GetSkillAttackPowerNG(BaseObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
      nPowerNG := Round(nPowerNG * (PowerRate / 100));
      TargeTBaseObject.SendDelayMsg(BaseObject, RM_MAGSTRUCK, 0, nPowerNG, 0, UserMagic.wMagIdx, '', 200);
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 地狱雷光技能函数修改 piaoyun 2013-07-25
function TMagicManager.MagElecBlizzard(BaseObject: TSmartObject; UserMagic: pTUserMagic; nPower: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPowerPoint, nPowerNG: Integer;
begin
  Result := False;
  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, BaseObject.m_nCurrX, BaseObject.m_nCurrY, g_Config.nElecBlizzardRange { 2 } ,
    BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if not (TargeTBaseObject.m_btLifeAttrib = LA_UNDEAD) then
    begin
      nPowerPoint := nPower div 10;
    end
    else
      nPowerPoint := nPower;

    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      nPowerNG := GetSkillLastPowerNG(nPowerPoint, // 内功技能威力
        GetSkillAttackPowerNG(BaseObject, UserMagic, nPowerPoint), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPowerPoint));
        // 修改地狱雷光技能威力 piaoyun 2013-07-25
      nPowerNG := Round(nPowerNG * (g_Config.nElecBlizzardPowerRate / 100));
      TargeTBaseObject.SendMsg(BaseObject, RM_MAGSTRUCK, 0, nPowerNG, 0, UserMagic.wMagIdx, '');
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

function TMagicManager.MagMakeHolyCurtain(BaseObject: TSmartObject; nPower: Integer; nX, nY: Integer): Integer; // 004928C0
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  MagicEvent: pTMagicEvent;
  HolyCurtainEvent: THolyCurtainEvent;
begin
  Result := 0;
  if BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
  begin
    BaseObjectList := TList.Create;
    MagicEvent := nil;
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, 1, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if ((TargeTBaseObject.m_btRaceServer = 127) or // 恶魔蝙蝠 不用判断等级
        ((TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) and ((Random(4) + (BaseObject.m_Abil.Level - 1)) > TargeTBaseObject.m_Abil.Level)))
        and (TargeTBaseObject.m_Master = nil) then
      begin
        TargeTBaseObject.OpenHolySeizeMode(nPower * 1000);
        if MagicEvent = nil then
        begin
          New(MagicEvent);
          FillChar(MagicEvent^, SizeOf(TMagicEvent), #0);
          MagicEvent.BaseObjectList_2 := TList.Create;
          MagicEvent.dwStartTick := MyGetTickCount();
          MagicEvent.dwTime := nPower * 1000;
          MagicEvent.Events_2 := TList.Create;
          MagicEvent.Envir := BaseObject.m_PEnvir;
        end;
        MagicEvent.BaseObjectList_2.Add(TargeTBaseObject);
        Inc(Result);
      end
      else
        Result := 0;
    end;
    BaseObjectList.Free;

    if (Result > 0) and (MagicEvent <> nil) then
    begin
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX - 1, nY - 2, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX + 1, nY - 2, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX - 2, nY - 1, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX + 2, nY - 1, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX - 2, nY + 1, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX + 2, nY + 1, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX - 1, nY + 2, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
      HolyCurtainEvent := THolyCurtainEvent.Create(BaseObject.m_PEnvir, nX + 1, nY + 2, ET_HOLYCURTAIN, nPower * 1000);
      g_EventManager.AddEvent(HolyCurtainEvent);
      MagicEvent.Events_2.Add(HolyCurtainEvent);
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        UserEngine.m_MagicEventList.LockW(2);
      try
{$IFEND}
        UserEngine.m_MagicEventList.Add(MagicEvent);
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UserEngine.m_MagicEventList.UnLockW;
      end;
{$IFEND}
    end
    else
    begin
      if MagicEvent <> nil then
      begin
        MagicEvent.BaseObjectList_2.Free;
        MagicEvent.Events_2.Free;
        Dispose(MagicEvent);
      end;
    end;
  end;
end;

function TMagicManager.MagMakeImprison(BaseObject: TSmartObject; nTime, nRange: Integer; nX, nY: Integer): Integer; // 004928C0
var
  I, J: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  MagicEvent: pTMagicEvent;
  nMaxX, nMaxY, nMinX, nMinY: Integer;
  Event: TImprisonCurtainEvent;
begin
  Result := 0;

  if BaseObject.m_PEnvir.CanWalk(nX, nY, True) then
  begin
    BaseObjectList := TList.Create;
    MagicEvent := nil;
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRange, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if TargeTBaseObject = nil then
        Continue;

      if TargeTBaseObject.m_boDeath then
        Continue;

      if (not TargeTBaseObject.m_boUnImprison) //
        and BaseObject.IsProperTarget(TargeTBaseObject) //
        and ((TargeTBaseObject.m_Abil.Level < BaseObject.m_Abil.Level) or (g_Config.boSkill69SameLevel and (TargeTBaseObject.m_Abil.Level
        = BaseObject.m_Abil.Level))) then
      begin
        TargeTBaseObject.m_dwImprisonTick := MyGetTickCount();
        TargeTBaseObject.m_dwImprisonTime := nTime * 1000;
        TargeTBaseObject.m_nImprisonRange := nRange;
        TargeTBaseObject.m_nImprisonPos.X := nX;
        TargeTBaseObject.m_nImprisonPos.Y := nY;
        TargeTBaseObject.m_boImprison := True;
        if MagicEvent = nil then
        begin
          New(MagicEvent);
          FillChar(MagicEvent^, SizeOf(TMagicEvent), #0);
          MagicEvent.BaseObjectList_2 := TList.Create;
          MagicEvent.dwStartTick := MyGetTickCount();
          MagicEvent.dwTime := nTime * 1000;
          MagicEvent.Events_2 := TList.Create;
          MagicEvent.Envir := BaseObject.m_PEnvir;
        end;
        MagicEvent.BaseObjectList_2.Add(TargeTBaseObject);
        Inc(Result);
      end;
    end;
    BaseObjectList.Free;

    if (Result > 0) and (MagicEvent <> nil) then
    begin
      nMinX := nX - nRange;
      nMaxX := nX + nRange;
      nMinY := nY - nRange;
      nMaxY := nY + nRange;
      for I := nMinX to nMaxX do
      begin
        for J := nMinY to nMaxY do
        begin
          if ((I < nMaxX) and (J = nMinY)) or ((J < nMaxY) and (I = nMinX)) or (I = nMaxX) or (J = nMaxY) then
          begin
            Event := TImprisonCurtainEvent.Create(BaseObject.m_PEnvir, I, J, ET_HOLYCURTAIN2, nTime * 1000);
            g_EventManager.AddEvent(Event);
            MagicEvent.Events_2.Add(Event);
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        UserEngine.m_MagicEventList.LockW(3);
      try
{$IFEND}
        UserEngine.m_MagicEventList.Add(MagicEvent);
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UserEngine.m_MagicEventList.UnLockW;
      end;
{$IFEND}
    end
    else
    begin
      if MagicEvent <> nil then
      begin
        MagicEvent.BaseObjectList_2.Free;
        MagicEvent.Events_2.Free;
        Dispose(MagicEvent);
      end;
    end;
  end;
end;

function TMagicManager.MagMakeGroupTransparent(BaseObject: TSmartObject; nX, nY, nHTime: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
begin
  Result := False;
  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, 1, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if BaseObject.IsProperFriend(TargeTBaseObject) then
    begin
      if TargeTBaseObject.m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] = 0 then
      begin // 00493287
        TargeTBaseObject.SendDelayMsg(TargeTBaseObject, RM_TRANSPARENT, 0, nHTime, 0, 0, '', 800);
        Result := True;
      end;
    end
  end;
  BaseObjectList.Free;
end;

// =====================================================================================
// 名称：
// 功能：
// 参数：
// BaseObject       魔法发起人
// TargeTBaseObject 受攻击角色
// nPower           魔法力大小
// nLevel           技能修炼等级
// nTargetX         目标座标X
// nTargetY         目标座标Y
// 返回值：
// =====================================================================================
// 火焰冰技能
function TMagicManager.MabMabe(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nLevel,
  nTargetX, nTargetY: Integer): Boolean;
var
  nLv: Integer;
  nTime: Integer;
begin
  Result := False;

  if BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) and (BaseObject <> TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        if (TargeTBaseObject.m_nAntiMagic <= Random(10)) then
        begin
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower div 3, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower div 3, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), 500);

          if (Random(2) + (BaseObject.m_Abil.Level - 1)) > TargeTBaseObject.m_Abil.Level then
          begin
            nLv := BaseObject.m_Abil.Level - TargeTBaseObject.m_Abil.Level;
            if (Random(g_Config.nMabMabeHitRandRate { 100 } ) < _MAX(g_Config.nMabMabeHitMinLvLimit, (nLevel * 8) - nLevel + 15 +
              nLv)) then
            begin
              if (Random(g_Config.nMabMabeHitSucessRate { 21 } ) < nLevel * 2 + 4) then
              begin
                if TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT then
                begin
                  BaseObject.SetPKFlag(BaseObject);
                  BaseObject.SetTargetCreat(TargeTBaseObject);
                end;

                TargeTBaseObject.SetLastHiter(BaseObject);

                if not BaseObject.CanCloseDefense then // 忽视目标防御
                  nPower := TargeTBaseObject.GetMagStruckDamage(BaseObject, nPower, nil);
                nPower := TargeTBaseObject.NewAbilPower(3, nPower);
                {
                  // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
                  // 伤害吸收 chongchong 2016-05-12
                  if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  begin
                  SmartObject := TSmartObject(TargeTBaseObject);
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
                }
                if FNpcReleaseMagic <> 0 then
                  BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
                    IntToStr(UserMagic.wMagIdx))
                else
                  BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
                    IntToStr(UserMagic.wMagIdx), 500); // 火焰冰技能延时

                if TargeTBaseObject.CanStone and (nPower >= 0) then
                begin
                  if g_Config.nMabMabeHitMabeTimeRate <= 0 then
                    nTime := nPower + Random(nLevel)
                  else
                    nTime := nPower div g_Config.nMabMabeHitMabeTimeRate { 20 } + Random(nLevel);

                  if g_Config.nMaxMabMabeHitMabeTime > 0 then
                    nTime := Min(nTime, g_Config.nMaxMabMabeHitMabeTime);

                  TargeTBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_STONE { 中毒类型 - 麻痹 } , nTime, NativeInt(BaseObject),
                    nLevel, IntToStr(UserMagic.wMagIdx), 550);
                end;
                Result := True;
              end;
            end;
          end;
        end
        else if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
      end;
    end;
  end;
end;

function TMagicManager.MagMakeSinSuSlave(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
var
  I: Integer;
  sMonName: string;
  nMakeLevel, nExpLevel: Integer;
  nCount: Integer;
  dwRoyaltySec: LongWord;
  SlaveObject: TBaseObject;
begin
  Result := False;
  sMonName := g_Config.sDogz;
  nMakeLevel := UserMagic.btLevel;
  nExpLevel := UserMagic.btLevel;
  nCount := g_Config.nDogzCount;
  dwRoyaltySec := 10 * 24 * 60 * 60;

  if not g_Config.boDogzPlugSettingPriority then
  begin
    if UserMagic.btNewLevel > 0 then
    begin
      if UserMagic.btNewLevel <= 3 then
        sMonName := g_Config.sPlusDogzName1_3
      else if UserMagic.btNewLevel <= 6 then
        sMonName := g_Config.sPlusDogzName4_6
      else if UserMagic.btNewLevel <= 9 then
        sMonName := g_Config.sPlusDogzName7_9
      else
        sMonName := g_Config.sPlusDogzName9_N;
      if UserMagic.btNewLevel <= 9 then
        nExpLevel := g_Config.dwPlusDogzLevels[UserMagic.btNewLevel - 1]
      else
        nExpLevel := g_Config.dwPlusDogzLevels[9 - 1] + (UserMagic.btNewLevel - 9) * g_Config.dwPlusDogzAddLevelAfter9;
    end;

    for I := Low(g_Config.DogzArray) to High(g_Config.DogzArray) do
    begin
      if g_Config.DogzArray[I].nHumLevel = 0 then
        break;

      if PlayObject.m_Abil.Level >= g_Config.DogzArray[I].nHumLevel then
      begin
        sMonName := g_Config.DogzArray[I].sMonName;
        nExpLevel := g_Config.DogzArray[I].nLevel;
        nCount := g_Config.DogzArray[I].nCount;
      end;
    end;
  end
  else
  begin
    for I := Low(g_Config.DogzArray) to High(g_Config.DogzArray) do
    begin
      if g_Config.DogzArray[I].nHumLevel = 0 then
        break;

      if PlayObject.m_Abil.Level >= g_Config.DogzArray[I].nHumLevel then
      begin
        sMonName := g_Config.DogzArray[I].sMonName;
        nExpLevel := g_Config.DogzArray[I].nLevel;
        nCount := g_Config.DogzArray[I].nCount;
      end;
    end;

    if UserMagic.btNewLevel > 0 then
    begin
      if UserMagic.btNewLevel <= 3 then
        sMonName := g_Config.sPlusDogzName1_3
      else if UserMagic.btNewLevel <= 6 then
        sMonName := g_Config.sPlusDogzName4_6
      else if UserMagic.btNewLevel <= 9 then
        sMonName := g_Config.sPlusDogzName7_9
      else
        sMonName := g_Config.sPlusDogzName9_N;
      if UserMagic.btNewLevel <= 9 then
        nExpLevel := g_Config.dwPlusDogzLevels[UserMagic.btNewLevel - 1]
      else
        nExpLevel := g_Config.dwPlusDogzLevels[9 - 1] + (UserMagic.btNewLevel - 9) * g_Config.dwPlusDogzAddLevelAfter9;
    end;
  end;

  { -ochongchong -c新增 : 英雄召唤神兽的数量以英雄定义的为准 【2013-08-10】 }
  if (PlayObject.m_btRaceServer = RC_HEROOBJECT) and (g_Config.dwHeroCallBBCount > 0) then
    nCount := g_Config.dwHeroCallBBCount;

  SlaveObject := PlayObject.MakeSlave(sMonName, nMakeLevel, nExpLevel, nCount, dwRoyaltySec, bb_Dogz, UserMagic.btNewLevel, True);

  if SlaveObject <> nil then
  begin
    SlaveObject.m_boAutoChangeColor := g_Config.boBBMonAutoChangeColor;
    Result := True;
  end;
end;

function TMagicManager.MagMakeBigDogSlave(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
var
  I: Integer;
  sMonName: string;
  nMakeLevel, nExpLevel: Integer;
  nCount: Integer;
  dwRoyaltySec: LongWord;
  SlaveObject: TBaseObject;
begin
  Result := False;
  sMonName := g_Config.sBigDogz;
  nMakeLevel := UserMagic.btLevel;
  nExpLevel := UserMagic.btLevel;
  nCount := g_Config.nBigDogzCount;
  dwRoyaltySec := 10 * 24 * 60 * 60;
  for I := Low(g_Config.BigDogzArray) to High(g_Config.BigDogzArray) do
  begin
    if g_Config.BigDogzArray[I].nHumLevel = 0 then
      break;

    if PlayObject.m_Abil.Level >= g_Config.BigDogzArray[I].nHumLevel then
    begin
      sMonName := g_Config.BigDogzArray[I].sMonName;
      nExpLevel := g_Config.BigDogzArray[I].nLevel;
      nCount := g_Config.BigDogzArray[I].nCount;
    end;
  end;

  // 英雄召唤圣兽数量 chongchong 2013-11-09
  if (PlayObject.m_btRaceServer = RC_HEROOBJECT) and (g_Config.dwHeroCallBBCount > 0) then
    nCount := g_Config.dwHeroCallBBCount;

  SlaveObject := PlayObject.MakeSlave(sMonName, nMakeLevel, nExpLevel, nCount, dwRoyaltySec, bb_BigDogz, UserMagic.btNewLevel, not
    g_Config.boBBAttrPlusAddOnlyMagic);

  if SlaveObject <> nil then
  begin
    SlaveObject.m_boAutoChangeColor := g_Config.boBBMonAutoChangeColor;
    Result := True;
  end;
end;

function TMagicManager.MagMakeSlave(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
var
  I: Integer;
  sMonName: string;
  nMakeLevel, nExpLevel: Integer;
  nCount: Integer;
  dwRoyaltySec: LongWord;
  SlaveObject: TBaseObject;
begin
  Result := False;
  sMonName := g_Config.sBoneFamm;
  nMakeLevel := UserMagic.btLevel;
  nExpLevel := UserMagic.btLevel;
  nCount := g_Config.nBoneFammCount;
  dwRoyaltySec := 10 * 24 * 60 * 60;
  if not g_Config.boBonePlugSettingPriority then
  begin
    if UserMagic.btNewLevel > 0 then
    begin
      if UserMagic.btNewLevel <= 3 then
        sMonName := g_Config.sPlusBoneFammName1_3
      else if UserMagic.btNewLevel <= 6 then
        sMonName := g_Config.sPlusBoneFammName4_6
      else if UserMagic.btNewLevel <= 9 then
        sMonName := g_Config.sPlusBoneFammName7_9
      else
        sMonName := g_Config.sPlusBoneFammName9_N;

      if UserMagic.btNewLevel <= 9 then
        nExpLevel := g_Config.dwPlusBoneFammLevels[UserMagic.btNewLevel - 1]
      else
        nExpLevel := g_Config.dwPlusBoneFammLevels[9 - 1] + (UserMagic.btNewLevel - 9) * g_Config.dwPlusBoneFammAddLevelAfter9;
    end;

    for I := Low(g_Config.BoneFammArray) to High(g_Config.BoneFammArray) do
    begin
      if g_Config.BoneFammArray[I].nHumLevel = 0 then
        break;

      if PlayObject.m_Abil.Level >= g_Config.BoneFammArray[I].nHumLevel then
      begin
        sMonName := g_Config.BoneFammArray[I].sMonName;
        nExpLevel := g_Config.BoneFammArray[I].nLevel;
        nCount := g_Config.BoneFammArray[I].nCount;
      end;
    end;
  end
  else
  begin
    for I := Low(g_Config.BoneFammArray) to High(g_Config.BoneFammArray) do
    begin
      if g_Config.BoneFammArray[I].nHumLevel = 0 then
        break;

      if PlayObject.m_Abil.Level >= g_Config.BoneFammArray[I].nHumLevel then
      begin
        sMonName := g_Config.BoneFammArray[I].sMonName;
        nExpLevel := g_Config.BoneFammArray[I].nLevel;
        nCount := g_Config.BoneFammArray[I].nCount;
      end;
    end;

    if UserMagic.btNewLevel > 0 then
    begin
      if UserMagic.btNewLevel <= 3 then
        sMonName := g_Config.sPlusBoneFammName1_3
      else if UserMagic.btNewLevel <= 6 then
        sMonName := g_Config.sPlusBoneFammName4_6
      else if UserMagic.btNewLevel <= 9 then
        sMonName := g_Config.sPlusBoneFammName7_9
      else
        sMonName := g_Config.sPlusBoneFammName9_N;
      if UserMagic.btNewLevel <= 9 then
        nExpLevel := g_Config.dwPlusBoneFammLevels[UserMagic.btNewLevel - 1]
      else
        nExpLevel := g_Config.dwPlusBoneFammLevels[9 - 1] + (UserMagic.btNewLevel - 9) * g_Config.dwPlusBoneFammAddLevelAfter9;
    end;
  end;

  { -ochongchong -c新增 : 英雄召唤骷髅的数量以英雄定义的为准 【2013-08-10】 }
  if (PlayObject.m_btRaceServer = RC_HEROOBJECT) and (g_Config.dwHeroCallBBCount > 0) then
    nCount := g_Config.dwHeroCallBBCount;

  SlaveObject := PlayObject.MakeSlave(sMonName, nMakeLevel, nExpLevel, nCount, dwRoyaltySec, bb_BoneFamm, UserMagic.btNewLevel,
    True);

  if SlaveObject <> nil then
  begin
    SlaveObject.m_boAutoChangeColor := g_Config.boBBMonAutoChangeColor;
    Result := True;
  end;
end;

function TMagicManager.MagMakeMoon(PlayObject: TSmartObject; UserMagic: pTUserMagic): Boolean;
var
  I: Integer;
  sMonName: string;
  nMakeLevel, nExpLevel: Integer;
  nCount: Integer;
  dwRoyaltySec: LongWord;
  SlaveObject: TBaseObject;
begin
  Result := False;
  sMonName := g_Config.sMonthSpirit;
  nMakeLevel := UserMagic.btLevel;
  nExpLevel := UserMagic.btLevel;
  nCount := g_Config.nMonthSpiritCount;
  dwRoyaltySec := 10 * 24 * 60 * 60;

  for I := Low(g_Config.MonthSpiritArray) to High(g_Config.MonthSpiritArray) do
  begin
    if g_Config.MonthSpiritArray[I].nHumLevel = 0 then
      break;
    if PlayObject.m_Abil.Level >= g_Config.MonthSpiritArray[I].nHumLevel then
    begin
      sMonName := g_Config.MonthSpiritArray[I].sMonName;
      nExpLevel := g_Config.MonthSpiritArray[I].nLevel;
      nCount := g_Config.MonthSpiritArray[I].nCount;
    end;
  end;

  { -ochongchong -c新增 : 英雄召唤月灵的数量以英雄定义的为准 【2013-08-10】 }
  if (PlayObject.m_btRaceServer = RC_HEROOBJECT) and (g_Config.dwHeroCallBBCount > 0) then
    nCount := g_Config.dwHeroCallBBCount;

  SlaveObject := PlayObject.MakeSlave(sMonName, nMakeLevel, nExpLevel, nCount, dwRoyaltySec, bb_MonthSpirit, UserMagic.btNewLevel,
    not g_Config.boBBAttrPlusAddOnlyMagic);

  if SlaveObject <> nil then
  begin
    SlaveObject.m_boAutoChangeColor := g_Config.boBBMonAutoChangeColor;
    Result := True;
  end;
end;

function TMagicManager.MagGroupMb(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; TargeTBaseObject:
  TBaseObject): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nTime: Integer;
  nRage: Integer; // 范围
begin
  Result := False;
  BaseObjectList := TList.Create;
  // nTime := 5 * UserMagic.btLevel + 1;
  // 麻痹时间 -- piaoyun 2013-06-27
  if UserMagic.btLevel >= 3 then
  begin
    nTime := g_Config.nSkill41MbTimers[3];
    nRage := g_Config.nSkill41MbRanges[3];
  end
  else
  begin
    nTime := g_Config.nSkill41MbTimers[UserMagic.btLevel];
    nRage := g_Config.nSkill41MbRanges[UserMagic.btLevel];
  end;

  try
    PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, nRage, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      if BaseObject.m_boDeath or (BaseObject.m_boGhost) or (PlayObject = BaseObject) then
        Continue;

      if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (not g_Config.boSkill41MbAttackPlayObject) then
        Continue;

      if (BaseObject.m_btRaceServer <> RC_PLAYOBJECT) //
        and (BaseObject.m_Master <> nil) //
        and (not g_Config.boSkill41MbAttackSlave) then
        Continue;

      if PlayObject.IsProperTarget(BaseObject) then
      begin
        if BaseObject.CanStone then
        begin
          if (BaseObject.m_Abil.Level < PlayObject.m_Abil.Level) or ((not g_Config.boDisableSkill41MbSameLevel) and (BaseObject.m_Abil.Level
            = PlayObject.m_Abil.Level)) then
          begin
            BaseObject.MakePosion(POISON_STONE, nTime, 0);
            BaseObject.m_boFastParalysis := True;
          end;
        end;
      end;

      if BaseObject.m_btRaceServer >= RC_ANIMAL then
        Result := True;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

function TMagicManager.MagGroupFengPo(PlayObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer;
  TargeTBaseObject: TBaseObject): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
  nPower, nPowerNG: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;

  nPower := PlayObject.GetAttackPower(PlayObject.m_WAbil.SC1, Integer((PlayObject.m_WAbil.SC2 - PlayObject.m_WAbil.SC1)));
  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower * g_Config.nSkill52PowerRate / 100);

  BaseObjectList := TList.Create;
  PlayObject.GetMapBaseObjects(PlayObject.m_PEnvir, nTargetX, nTargetY, g_Config.nSkill52AttackRange, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    BaseObject := TBaseObject(BaseObjectList.Items[I]);
    if BaseObject.m_boDeath or (BaseObject.m_boGhost) or (PlayObject = BaseObject) then
      Continue;

    if PlayObject.IsProperTarget(BaseObject) then
    begin
      // 伤害吸收 chongchong 2016-05-12
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower := Max(0, nPower - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
        }
      end;

      nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
        GetSkillAttackPowerNG(PlayObject, UserMagic, nPower), GetSkillDefensePowerNG(BaseObject, UserMagic, nPower));

      if FNpcReleaseMagic <> 0 then
        PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPowerNG, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 1,
          NativeInt(BaseObject), IntToStr(UserMagic.wMagIdx))
      else
        PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPowerNG, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 1,
          NativeInt(BaseObject), IntToStr(UserMagic.wMagIdx), 200);
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 凤舞祭
function TMagicManager.MagMakeSkill104(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;
  if BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        // 连击100%命中 chongchong 2017-11-16
        if True { (TargeTBaseObject.m_nAntiMagic <= Random(10)) } then
        begin
          with BaseObject do
          begin
            nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 -
              m_WAbil.MC1, 1));
          end;

          nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
          nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[4] / 100));
          nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
          nPower := GetSkillContinuousBlastHitPower(BaseObject, TargeTBaseObject, UserMagic, nPower);

          // 连击暴击
          // 伤害吸收 chongchong 2016-05-12
          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(BaseObject);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nPower := Max(0, nPower - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            {
              // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
            }
          end;

          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
          begin
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
              dwDelay := 0
            else
              dwDelay := 400;
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), dwDelay);
          end;
          if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
            Result := True;
          if (BaseObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
            RC_PLAYMOSTER]) then
            Result := True;
        end
        else
        begin
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');

          TargeTBaseObject := nil;
        end;
      end
      else
        TargeTBaseObject := nil;
    end
    else
      TargeTBaseObject := nil;
  end
  else
    TargeTBaseObject := nil;
end;

// 惊雷爆
function TMagicManager.MagMakeSkill105(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  I: Integer;
  nPower, nPower2: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
  Target: TBaseObject;
  BaseObjectList: TList;
begin
  Result := False;
  if not BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  if (abs(TargeTBaseObject.m_nCurrX - nTargetX) > 1) or (abs(TargeTBaseObject.m_nCurrY - nTargetY) > 1) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  nPower := BaseObject.GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + BaseObject.m_WAbil.MC1, Max(BaseObject.m_WAbil.MC2
    - BaseObject.m_WAbil.MC1, 1));
  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[5] / 100));
  nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
  nPower := GetSkillContinuousBlastHitPower(BaseObject, TargeTBaseObject, UserMagic, nPower); // 连击暴击

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, g_Config.nSkill103AttackRange, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    Target := TBaseObject(BaseObjectList.Items[I]);
    if Target.m_boDeath or (Target.m_boGhost) or (Target = BaseObject) then
      Continue;

    if BaseObject.IsProperTarget(Target) then
    begin
      nPower2 := nPower;
      // 伤害吸收 chongchong 2016-05-12
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
          if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
          nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
          end;
          end;
        }
      end;

      if FNpcReleaseMagic <> 0 then
        BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(nTargetX, nTargetY), 2, NativeInt(Target), IntToStr(UserMagic.wMagIdx))
      else
      begin
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwDelay := 100
        else
          dwDelay := 100;
        BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(nTargetX, nTargetY), 2, NativeInt(Target), IntToStr(UserMagic.wMagIdx),
          dwDelay);
      end;

      if (Target.m_btRaceServer >= RC_ANIMAL) then
        Result := True;

      if (BaseObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) //
        or (Target.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) then
        Result := True;
    end;
  end;
end;

// 冰天雪地
function TMagicManager.MagMakeSkill106(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
var
  I, nLevel, nRate, nTime: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPower, nPower2: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;

  nLevel := UserMagic.btNewLevel + UserMagic.btLevel;
  nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.MC1, Integer((BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1)));
  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[6] / 100));
  nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
  nPower := GetSkillContinuousBlastHitPower(BaseObject, nil, UserMagic, nPower); // 连击暴击
  nRate := g_Config.nSkill106AddFrozenRate + nLevel * g_Config.nSkill106AddFrozenRate2;
  nTime := g_Config.nSkill106AddFrozenTime + nLevel * g_Config.nSkill106AddFrozenTime2;

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, 3, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) or (TargeTBaseObject = BaseObject) then
      Continue;

    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      nPower2 := nPower;
      // 伤害吸收 chongchong 2016-05-12
      if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(BaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
          if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
          nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
          end;
          end;
        }
      end;

      // 冰冻
      if (not TargeTBaseObject.UnFrozen) and (Random(100) < nRate) and (nTime > 0) then
        TargeTBaseObject.MakeFrozen(nTime);

      if FNpcReleaseMagic <> 0 then
        BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
          1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
      else
      begin
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwDelay := 0
        else
          dwDelay := 200;
        BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
          1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), dwDelay);
      end;
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 双龙破
function TMagicManager.MagMakeSkill107(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;

  if BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        // 连击100%命中 chongchong 2017-11-16
        if True { (TargeTBaseObject.m_nAntiMagic <= Random(10)) } then
        begin
          with BaseObject do
          begin
            nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.MC1, Max(m_WAbil.MC2 -
              m_WAbil.MC1, 1));
          end;

          nPower := GetNewLevelPower(nPower, UserMagic);
          nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[7] / 100));
          nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
          nPower := GetSkillContinuousBlastHitPower(BaseObject, TargeTBaseObject, UserMagic, nPower);

          // 连击暴击
          // 伤害吸收 chongchong 2016-05-12
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(TargeTBaseObject);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nPower := Max(0, nPower - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            {
              // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
            }
          end;

          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
          begin
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
              dwDelay := 100
            else
              dwDelay := 400;
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), dwDelay); // 双龙破技能延时
          end;
          if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
            Result := True;
          if (BaseObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
            RC_PLAYMOSTER]) then
            Result := True;
        end
        else
        begin
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');

          TargeTBaseObject := nil;
        end;
      end
      else
        TargeTBaseObject := nil;
    end
    else
      TargeTBaseObject := nil;
  end
  else
    TargeTBaseObject := nil;
end;

// 虎啸诀
function TMagicManager.MagMakeSkill108(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;

  if BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        // 连击100%命中 chongchong 2017-11-16
        if True { (TargeTBaseObject.m_nAntiMagic <= Random(10)) } then
        begin
          with BaseObject do
          begin
            nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2 -
              m_WAbil.SC1, 1));
          end;

          nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
          nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[8] / 100));
          nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
          nPower := GetSkillContinuousBlastHitPower(BaseObject, TargeTBaseObject, UserMagic, nPower);

          // 连击暴击
          // 伤害吸收 chongchong 2016-05-12
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(TargeTBaseObject);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nPower := Max(0, nPower - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            {
              // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
            }
          end;

          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
          begin
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
              dwDelay := 0
            else
              dwDelay := 400;
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), dwDelay);
          end;

          if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
            Result := True;

          if (BaseObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
            RC_PLAYMOSTER]) then
            Result := True;
        end
        else
        begin
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');

          TargeTBaseObject := nil;
        end;
      end
      else
        TargeTBaseObject := nil;
    end
    else
      TargeTBaseObject := nil;
  end
  else
    TargeTBaseObject := nil;
end;

// 八卦掌
function TMagicManager.MagMakeSkill109(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;
var
  nPower: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;

  if BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      if (abs(TargeTBaseObject.m_nCurrX - nTargetX) <= 1) and (abs(TargeTBaseObject.m_nCurrY - nTargetY) <= 1) then
      begin
        // 连击100%命中 chongchong 2017-11-16
        if True { (TargeTBaseObject.m_nAntiMagic <= Random(10)) } then
        begin
          with BaseObject do
          begin
            nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2 -
              m_WAbil.SC1, 1));
          end;

          nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
          nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[9] / 100));
          nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
          nPower := GetSkillContinuousBlastHitPower(BaseObject, TargeTBaseObject, UserMagic, nPower);

          // 连击暴击
          // 伤害吸收 chongchong 2016-05-12
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            SmartObject := TSmartObject(TargeTBaseObject);
            if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
            begin
              nPower := Max(0, nPower - SmartObject.GetNGDecPower);
              SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
              SmartObject.RefAbilNH;
            end;
            {
              // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
            }
          end;

          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx))
          else
          begin
            if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
              dwDelay := 100
            else
              dwDelay := 400;
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
              IntToStr(UserMagic.wMagIdx), dwDelay);
          end;

          if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
            Result := True;

          if (BaseObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
            RC_PLAYMOSTER]) then
            Result := True;
        end
        else
        begin
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            BaseObject.SendMsg(TargeTBaseObject, RM_ATTACK_MISS, 0, 0, 0, 0, '');
          TargeTBaseObject := nil;
        end;
      end
      else
        TargeTBaseObject := nil;
    end
    else
      TargeTBaseObject := nil;
  end
  else
    TargeTBaseObject := nil;
end;

// 三焰咒
function TMagicManager.MagMakeSkill110(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer; var
  TargeTBaseObject: TBaseObject): Boolean;

  function CanMotaebo(Target: TBaseObject): Boolean;
  var
    nC: Integer;
  begin
    Result := False;
    if g_Config.boSkill110PushedHighLevel then
      Result := (not Target.m_boStickMode) and BaseObject.IsProperTarget(Target)
    else
    begin
      if (BaseObject.m_Abil.Level > Target.m_Abil.Level) and (not Target.m_boStickMode) then
      begin
        nC := BaseObject.m_Abil.Level - Target.m_Abil.Level;
        if Random(20) < ((1 * 4) + 6 + nC) then
        begin
          if BaseObject.IsProperTarget(Target) then
            Result := True;
        end;
      end;
    end;
  end;

var
  nPower, nLevel, nRate, nRange, btNewDir: Integer;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;
  if not BaseObject.MagCanHitTarget(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  if not BaseObject.IsProperTarget(TargeTBaseObject) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  if (abs(TargeTBaseObject.m_nCurrX - nTargetX) > 1) or (abs(TargeTBaseObject.m_nCurrY - nTargetY) > 1) then
  begin
    TargeTBaseObject := nil;
    Exit;
  end;

  with BaseObject do
  begin
    nPower := GetAttackPower(GetPower(BaseObject, MPow(BaseObject, UserMagic), UserMagic) + m_WAbil.SC1, Max(m_WAbil.SC2 - m_WAbil.SC1,
      1));
  end;

  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[10] / 100));
  nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
  nPower := GetSkillContinuousBlastHitPower(BaseObject, TargeTBaseObject, UserMagic, nPower); // 连击暴击

  // 伤害吸收 chongchong 2016-05-12
  if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
  begin
    SmartObject := TSmartObject(TargeTBaseObject);
    if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
    begin
      nPower := Max(0, nPower - SmartObject.GetNGDecPower);
      SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
      SmartObject.RefAbilNH;
    end;
    {
      // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
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
    }
  end;

  nLevel := UserMagic.btNewLevel + UserMagic.btLevel;
  nRate := g_Config.nSkill110PushedRate + nLevel * g_Config.nSkill110PushedRate2;
  nRange := g_Config.nSkill110PushedRange + nLevel * g_Config.nSkill110PushedRange2;

  // 推动
  if (Random(100) < nRate) and (nRange > 0) then
  begin
    if (not TargeTBaseObject.m_boDeath) and (TargeTBaseObject <> BaseObject) then
    begin
      if CanMotaebo(TargeTBaseObject) then
      begin
        btNewDir := GetNextDirection(BaseObject.m_nCurrX, BaseObject.m_nCurrY, TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY);
        TargeTBaseObject.CharPushed(btNewDir, nRange);
      end;
    end;
  end;

  if FNpcReleaseMagic <> 0 then
    BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
      IntToStr(UserMagic.wMagIdx))
  else
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
      dwDelay := 100
    else
      dwDelay := 400;
    BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(nTargetX, nTargetY), 2, NativeInt(TargeTBaseObject),
      IntToStr(UserMagic.wMagIdx), dwDelay);
  end;

  if (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) then
    Result := True;

  if (BaseObject.m_btRaceServer in [RC_HEROOBJECT, RC_PLAYMOSTER]) or (TargeTBaseObject.m_btRaceServer in [RC_HEROOBJECT,
    RC_PLAYMOSTER]) then
    Result := True;
end;

// 万剑归宗
function TMagicManager.MagMakeSkill111(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPower, nPower2: Integer;
  ToxicSmokeTime: LongWord;
  SmartObject: TSmartObject;
  dwDelay: Cardinal;
begin
  Result := False;

  nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.SC1, Integer((BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1)));
  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower * (g_Config.SkillContinuousPowerRates[11] / 100));
  nPower := Min(LongWord(nPower + Round(nPower * (UserMagic.btLevel * g_Config.nContinuousAttackLevelRate / 100))), High(Integer));
  nPower := GetSkillContinuousBlastHitPower(BaseObject, nil, UserMagic, nPower); // 连击暴击

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, 3, BaseObjectList);
  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) or (TargeTBaseObject = BaseObject) then
      Continue;

    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      nPower2 := nPower;
      // 伤害吸收 chongchong 2016-05-12
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
          if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
          nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
          end;
          end;
        }
      end;

      if FNpcReleaseMagic <> 0 then
        BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
          1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
      else
      begin
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          dwDelay := 100
        else
          dwDelay := 500;
        BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
          1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), dwDelay);
      end;

      // 被万剑归宗攻击人物头顶冒毒烟效果 chongchong 2013-11-10
      if g_Config.boToxicSmoke then
      begin
        if UserMagic.btLevel > High(g_Config.dwToxicSmokeTimes) then
          ToxicSmokeTime := g_Config.dwToxicSmokeTimes[High(g_Config.dwToxicSmokeTimes)]
        else
          ToxicSmokeTime := g_Config.dwToxicSmokeTimes[UserMagic.btLevel];
        TargeTBaseObject.OpenToxicSmoke(ToxicSmokeTime, 1000, nPower);
      end;
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 倚天辟地
function TMagicManager.MagMakeSkill114(BaseObject: TSmartObject; UserMagic: pTUserMagic; var nTargetX, nTargetY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPower, nPower2: Integer;
  nRage: Integer;
  SmartObject: TSmartObject;
begin
  nRage := Min(g_Config.nSkill114AttackRange + UserMagic.btLevel, g_Config.nSkill114AttackRange);
  nTargetX := BaseObject.m_nCurrX;
  nTargetY := BaseObject.m_nCurrY;

  nPower := 0;
  case BaseObject.m_btJob of
    0:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.DC1, Integer((BaseObject.m_WAbil.DC2 - BaseObject.m_WAbil.DC1)));
    1:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.MC1, Integer((BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1)));
    2:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.SC1, Integer((BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1)));
  end;

  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower / 100 * (g_Config.nSkill114PowerRate + (UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkill114LevelUpAddPowerRate));

  BaseObjectList := TList.Create;
  try
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, BaseObject.m_nCurrX, BaseObject.m_nCurrY, { BaseObject.m_nViewRange div 2 }
      nRage, BaseObjectList);

    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) or (TargeTBaseObject = BaseObject) then
        Continue;

      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower2 := nPower;
        // 伤害吸收 chongchong 2016-05-12
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
            if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin // 吸收伤害
            if Random(100) < SmartObject.m_nSuckDamageProbability then
            begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
            end;
            end;
          }
        end;

        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 800);
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
  Result := True;
end;

// 血魄一击(法)
function TMagicManager.MagMakeSkill116(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPower, nPower2: Integer;
  nRage: Integer;
  SmartObject: TSmartObject;
begin
  nRage := Min(g_Config.nSkill116Range + UserMagic.btLevel, g_Config.nSkill116Range);
  nPower := 0;
  case BaseObject.m_btJob of
    0:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.DC1, Integer((BaseObject.m_WAbil.DC2 - BaseObject.m_WAbil.DC1)));
    1:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.MC1, Integer((BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1)));
    2:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.SC1, Integer((BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1)));
  end;

  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower / 100 * (g_Config.nSkill116PowerRate + (UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkill116LevelUpAddPowerRate));

  BaseObjectList := TList.Create;
  try
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, { BaseObject.m_nViewRange div 2 } nRage, BaseObjectList);

    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) or (TargeTBaseObject = BaseObject) then
        Continue;

      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower2 := nPower;
        // 伤害吸收 chongchong 2016-05-12
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
            if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin // 吸收伤害
            if Random(100) < SmartObject.m_nSuckDamageProbability then
            begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
            end;
            end;
          }
        end;
        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 800);
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
  Result := True;
end;

// 血魄一击(道)
function TMagicManager.MagMakeSkill117(BaseObject: TSmartObject; UserMagic: pTUserMagic; nTargetX, nTargetY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPower, nPower2: Integer;
  nRage: Integer;
  SmartObject: TSmartObject;
begin
  nRage := Min(g_Config.nSkill117Range + UserMagic.btLevel, g_Config.nSkill117Range);
  nPower := 0;
  case BaseObject.m_btJob of
    0:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.DC1, Integer((BaseObject.m_WAbil.DC2 - BaseObject.m_WAbil.DC1)));
    1:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.MC1, Integer((BaseObject.m_WAbil.MC2 - BaseObject.m_WAbil.MC1)));
    2:
      nPower := BaseObject.GetAttackPower(BaseObject.m_WAbil.SC1, Integer((BaseObject.m_WAbil.SC2 - BaseObject.m_WAbil.SC1)));
  end;

  nPower := GetNewLevelPower(nPower, UserMagic); // 取强化技能攻击伤害
  nPower := Round(nPower / 100 * (g_Config.nSkill117PowerRate + (UserMagic.btLevel + UserMagic.btNewLevel) * g_Config.nSkill117LevelUpAddPowerRate));

  BaseObjectList := TList.Create;
  try
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nTargetX, nTargetY, { BaseObject.m_nViewRange div 2 } nRage, BaseObjectList);

    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) or (TargeTBaseObject = BaseObject) then
        Continue;

      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower2 := nPower;
        // 伤害吸收 chongchong 2016-05-12
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
            if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin // 吸收伤害
            if Random(100) < SmartObject.m_nSuckDamageProbability then
            begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
            end;
            end;
          }
        end;

        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            1, NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 800);
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
  Result := True;
end;

// 新技能测试
function TMagicManager.MagMakeNewSkill_Test(BaseObject: TSmartObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer; nRage:
  Integer): Boolean; // 00492F4C
var
  I, nPowerNG: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
begin
  Result := False;
  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      BaseObject.SetTargetCreat(TargeTBaseObject);
      nPowerNG := GetSkillLastPowerNG(nPower, // 内功技能威力
        GetSkillAttackPowerNG(BaseObject, UserMagic, nPower), GetSkillDefensePowerNG(TargeTBaseObject, UserMagic, nPower));
      TargeTBaseObject.SendMsg(BaseObject, RM_MAGSTRUCK, 0, nPowerNG, 0, UserMagic.wMagIdx, '');

      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 死亡之眼
function TMagicManager.MagBigExplosionAndMakePoison(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer):
  Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPowerPoint, nPower2: Integer;
  nTemp, nTime: Integer;
  nPowerPoint2: Integer;
  nRage: Integer;
  IsPoison: Boolean;
  IsPoisonRate: Boolean;
  SmartObject: TSmartObject;
  I64: Int64;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  I64 := Round(nPower * (g_Config.nSkill203BasicPowerRate / 100) + UserMagic.btLevel * (g_Config.nSkill203LevelupPowerRate / 100)
    * nPower);

  nPowerPoint := Min(I64, High(Integer));
  nPowerPoint := GetNewLevelPower(nPowerPoint, UserMagic);
  nTime := UserMagic.btLevel * g_Config.nSkill203LevelupMbTimer + g_Config.nSkill203BasicMbTimer;
  I64 := Round(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint));
  I64 := Min(I64, High(Integer));
  nPowerPoint2 := GetNewLevelPower(I64, UserMagic); // ROUND(UserMagic.btLevel / 3 * (nPower / g_Config.nAmyOunsulPoint))
  nRage := g_Config.nSkill203Rage;

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);

    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      IsPoison := False;
      if BaseObject.m_Abil.Level > TargeTBaseObject.m_Abil.Level then
      begin
        if TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT then
        begin
          IsPoison := g_Config.boSkill203MbAttackHuman { 麻痹人物 };
        end
        else
        begin
          if (TargeTBaseObject.m_Master <> nil) and (TargeTBaseObject.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
            IsPoison := g_Config.boSkill203MbAttackSlave { 麻痹宝宝 }
          else
            IsPoison := g_Config.boSkill203MbAttackMon { 麻痹怪物 };
        end;
      end;

      nPower2 := nPowerPoint;
      IsPoisonRate := Random(g_Config.nSkill203PoisonRate) = 0;

      // 伤害吸收 chongchong 2016-05-12
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
          if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
          nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
          end;
          end;
        }
      end;

      if IsPoison and g_Config.boSkill203MbFastParalysis and IsPoisonRate then
      begin
        // 快速麻痹修改
        TargeTBaseObject.m_boFastParalysis := True;
        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), 'F' + IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), 'F' + IntToStr(UserMagic.wMagIdx), 1200);
      end
      else
      begin
        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 1200);
      end;

      if IsPoison and IsPoisonRate then
        TargeTBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_STONE { 中毒类型 - 麻痹 } , nTime, NativeInt(BaseObject), 0,
          IntToStr(UserMagic.wMagIdx), 1500);

      // 修正死亡之眼中毒时间太长 chongchong 2016-08-03
      nTemp := GetPower13(BaseObject, 40, UserMagic) + GetRPow(BaseObject, BaseObject.m_WAbil.SC1, BaseObject.m_WAbil.SC2) * 2;
      nTemp := Min(Round(nTemp * (g_Config.nAmyOunsulTimeRate / 100)), g_Config.nAmyOunsulMaxTime);
      if g_Config.boSkill203Damagearmor then
      begin
        TargeTBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_DAMAGEARMOR { 中毒类型 - 红毒 } , nTemp, NativeInt(BaseObject),
          nPowerPoint2, IntToStr(UserMagic.wMagIdx), 1200);
      end;
      if g_Config.boSkill203DecHealth then
      begin
        TargeTBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_DECHEALTH { 中毒类型 - 绿毒 } , nTemp, NativeInt(BaseObject),
          nPowerPoint2, IntToStr(UserMagic.wMagIdx), 1200);
      end;
      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 十步一杀
function TMagicManager.MagBigExplosionAndMakePoisonByWarr(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer;
  var boMove: Boolean): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPowerPoint: Integer;
  nTime: Integer;
  Flag: TWalkFlagArr;
  nRage: Integer;
  IsPoison: Boolean;
begin
  Result := False;
  // 禁锢不允许使用十步一杀 chongchong 2018-05-28
  if (BaseObject <> nil) and (BaseObject.m_boImprison) then
    Exit;

  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  boMove := False;
  Flag := [];
  if g_Config.boSkill204RunHum then
    Flag := Flag + [wf_Hum];

  if g_Config.boSkill204RunMon then
    Flag := Flag + [wf_Mon];

  if g_Config.boSkill204RunNpc then
    Flag := Flag + [wf_Npc];

  if g_Config.boSkill204RunGuard then
    Flag := Flag + [wf_Guard];

  if g_Config.boSkill204WarDisHumRun then
    Flag := Flag + [wf_War];

  if g_Config.boSkill204RunObstacle then
    Flag := Flag + [wf_Obstacle];
  nRage := g_Config.nSkill204Rage;

  // 十步一杀允许穿越障碍勾选后 人物叠加到目标身上了，正常情况应该和没勾选那样站目标身边 chongchong 2014-09-24
  if BaseObject.MagCanMoveTarget(nX, nY, Flag, g_Config.boSkill204DisableStopItem, g_Config.nSkill204Distance) then
  begin
    boMove := True;
    nX := BaseObject.m_nCurrX;
    nY := BaseObject.m_nCurrY;
    nPowerPoint := Round(nPower * (g_Config.nSkill204BasicPowerRate / 100) + UserMagic.btLevel * (g_Config.nSkill204LevelupPowerRate
      / 100) * nPower);

    nPowerPoint := GetNewLevelPower(nPowerPoint, UserMagic);

    nTime := UserMagic.btLevel * g_Config.nSkill204LevelupMbTimer + g_Config.nSkill204BasicMbTimer;
    BaseObjectList := TList.Create;
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

    for I := 0 to BaseObjectList.Count - 1 do
    begin
      IsPoison := False;
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        { 十步一杀增加同等级可攻击 piaoyun 2013-09-16 }
        if ((BaseObject.m_Abil.Level > TargeTBaseObject.m_Abil.Level) or (g_Config.boSkill204SameLevel and (BaseObject.m_Abil.Level
          >= TargeTBaseObject.m_Abil.Level))) and (Random(g_Config.nSkill204BasicMbRate) = 0) then
        begin
          if TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT then
            IsPoison := g_Config.boSkill204MbAttackHuman
          else
          begin
            if (TargeTBaseObject.m_Master <> nil) and (TargeTBaseObject.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
              IsPoison := g_Config.boSkill204MbAttackSlave
            else
              IsPoison := g_Config.boSkill204MbAttackMon;
          end;
        end;

        if IsPoison and g_Config.boSkill204MbFastParalysis then
        begin
          // 快速麻痹修改 chongchong 2015-02-16 22:55:23
          TargeTBaseObject.m_boFastParalysis := True;
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPowerPoint, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(3, 0), NativeInt(TargeTBaseObject), 'F' + IntToStr(UserMagic.wMagIdx))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPowerPoint, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(3, 0), NativeInt(TargeTBaseObject), 'F' + IntToStr(UserMagic.wMagIdx), 600);
        end
        else
        begin
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPowerPoint, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(3, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPowerPoint, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(3, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 600);
        end;
        if IsPoison then
          TargeTBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_STONE { 中毒类型 - 麻痹 } , nTime, NativeInt(BaseObject), 0,
            IntToStr(UserMagic.wMagIdx), 800);

        Result := True;
      end;
    end;
    BaseObjectList.Free;
  end;
end;

// 冰霜雪雨函数修改 piaoyun 2013-07-25
function TMagicManager.MagDoubleBigExplosionEx(BaseObject: TBaseObject; nPower, nX, nY: Integer; nMagID: Integer; boDecMP: Boolean):
  Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nRage: Integer;
  nBasePower, nNGPower: Integer;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  nRage := g_Config.nSkill205Rage;
  nPower := Round(nPower * (g_Config.nSkill205PowerRate / 100));
  nPower := GetNewLevelPower(nPower, TSmartObject(BaseObject).m_UserMagics[nMagID]);
  nBasePower := nPower;

  nNGPower := GetSkillAttackPowerNG(BaseObject, TSmartObject(BaseObject).m_UserMagics[nMagID], nBasePower);

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);
  try
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (TargeTBaseObject = nil) or (TargeTBaseObject.m_boGhost) or TargeTBaseObject.m_boDeath then
        Continue;

      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower := GetSkillLastPowerNG(nBasePower, // 内功技能威力
          nNGPower, GetSkillDefensePowerNG(TargeTBaseObject, TSmartObject(BaseObject).m_UserMagics[nMagID], nBasePower));

        if boDecMP and g_Config.boSkill205ReduceMP then
          TargeTBaseObject.DamageSpell(nPower div 2);

        if g_Config.boSkill205PowerTwoAttack then
        begin
          if FNpcReleaseMagic <> 0 then
            BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(nMagID))
          else
            BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
              MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(nMagID), 1380);
        end;

        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(nMagID))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(2, 0), NativeInt(TargeTBaseObject), IntToStr(nMagID), 2000);
        Result := True;
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

// 冰霜群雨
function TMagicManager.MagBigExplosionAndMakePoisonEx(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer):
  Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPowerPoint, nPower2: Integer;
  nTime: Integer;
  nRage: Integer;
  IsPoison: Boolean;
  SmartObject: TSmartObject;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  nPowerPoint := Round(nPower * (g_Config.nSkill206BasicPowerRate / 100) + UserMagic.btLevel * (g_Config.nSkill206LevelupPowerRate
    / 100) * nPower);

  nPowerPoint := GetNewLevelPower(nPowerPoint, UserMagic);
  nTime := UserMagic.btLevel * g_Config.nSkill206LevelupMbTimer + g_Config.nSkill206BasicMbTimer;
  nRage := g_Config.nSkill206Rage;

  BaseObjectList := TList.Create;
  BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

  for I := 0 to BaseObjectList.Count - 1 do
  begin
    TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
    if BaseObject.IsProperTarget(TargeTBaseObject) then
    begin
      IsPoison := False;
      { 冰霜群雨增加同等级可攻击 piaoyun 2013-09-16 }
      if (BaseObject.m_Abil.Level > TargeTBaseObject.m_Abil.Level) or (g_Config.boSkill206SameLevel and (BaseObject.m_Abil.Level
        >= TargeTBaseObject.m_Abil.Level)) then
      begin
        if TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT then
        begin
          if g_Config.boSkill206MbAttackHuman and (not g_Config.boSkill206Frozen) then
          begin
            IsPoison := True;
          end
          else if g_Config.boSkill206MbAttackHuman //
            and (g_Config.boSkill206Frozen) //
            and (Random(g_Config.nSkill206FrozenRate) = 0) then
          begin
            TargeTBaseObject.MakeFrozen(nTime);
            // 冰霜群雨冰冻效果 piaoyun 2013-09-16
          end;
        end
        else
        begin
          if (TargeTBaseObject.m_Master <> nil) and (TargeTBaseObject.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
          begin
            if g_Config.boSkill206MbAttackSlave and (not g_Config.boSkill206Frozen) then
            begin
              IsPoison := True;
            end
            else if g_Config.boSkill206MbAttackSlave //
              and (g_Config.boSkill206Frozen) //
              and (Random(g_Config.nSkill206FrozenRate) = 0) then
            begin
              TargeTBaseObject.MakeFrozen(nTime);
              // 冰霜群雨冰冻效果 piaoyun 2013-09-16
            end;
          end
          else
          begin
            if g_Config.boSkill206MbAttackMon and (not g_Config.boSkill206Frozen) then
            begin
              IsPoison := True;
            end
            else if g_Config.boSkill206MbAttackMon //
              and (g_Config.boSkill206Frozen) //
              and (Random(g_Config.nSkill206FrozenRate) = 0) then
            begin
              TargeTBaseObject.MakeFrozen(nTime);
              // 冰霜群雨冰冻效果 piaoyun 2013-09-16
            end;
          end;
        end;
      end;

      nPower2 := nPowerPoint;
      // 伤害吸收 chongchong 2016-05-12
      if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      begin
        SmartObject := TSmartObject(TargeTBaseObject);
        if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
        begin
          nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
          SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
          SmartObject.RefAbilNH;
        end;
        {
          // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
          if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
          begin // 吸收伤害
          if Random(100) < SmartObject.m_nSuckDamageProbability then
          begin
          nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
          if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
          nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
          Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
          nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
          end;
          end;
        }
      end;

      // 冰霜群雨攻击范围修改 piaoyun 2013-07-25
      if IsPoison and g_Config.boSkill206MbFastParalysis then
      begin
        // 快速麻痹修改 chongchong 2015-02-16 22:55:23
        TargeTBaseObject.m_boFastParalysis := True;
        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), 'F' + IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), 'F' + IntToStr(UserMagic.wMagIdx), 600);
      end
      else
      begin
        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(3, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 600);
      end;

      if IsPoison then
        TargeTBaseObject.SendDelayMsg(BaseObject, RM_POISON, POISON_STONE { 中毒类型 - 麻痹 } , nTime, NativeInt(BaseObject), 0,
          IntToStr(UserMagic.wMagIdx), 800);

      Result := True;
    end;
  end;
  BaseObjectList.Free;
end;

// 旋风斩技能函数 piaoyun 2013-09-15
function TMagicManager.MagMakeSkill208(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  PlayObject: TPlayObject;
  nPowerPoint, nPower2: Integer;
  nRage: Integer;
  nAttackCount: Integer; // 能够攻击的对象链表
  ProperTargetList: TList;
  n: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  // 威力倍数
  nPowerPoint := Round(nPower / 100 * g_Config.nSkill208PowerRate);
  nRage := g_Config.nSkill208Rage;
  nPowerPoint := GetNewLevelPower(nPowerPoint, UserMagic); // 取强化技能攻击伤害
  PlayObject := TPlayObject(BaseObject);
  BaseObjectList := TList.Create;
  ProperTargetList := TList.Create;
  try
    // 遍历范围内的对象
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, BaseObject.m_nCurrX, BaseObject.m_nCurrY, nRage, BaseObjectList);
    nAttackCount := 0;

    // 第一轮，先计算出有多少对象能被攻击
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        ProperTargetList.Add(TargeTBaseObject);
        Inc(nAttackCount);
      end;
    end;

    if nAttackCount = 0 then
      Exit;

    // 根据技能等级来确定攻击多少目标
    case UserMagic.btLevel of
      1..3:
        n := 3;
      4..6:
        n := 4;
      7..9:
        n := 5;
    else
      n := 0;
    end;

    // 先攻击全部
    for I := 0 to ProperTargetList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(ProperTargetList.Items[I]);

      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower2 := nPowerPoint;
        // 伤害吸收 chongchong 2016-05-12
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
            if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin // 吸收伤害
            if Random(100) < SmartObject.m_nSuckDamageProbability then
            begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
            end;
            end;
          }
        end;

        if FNpcReleaseMagic <> 0 then
          PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(0, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(0, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 250); // 旋风斩技能延时
      end;
    end;

    // 断筋处理
    if nAttackCount >= n then
    begin
      for I := 0 to n - 1 do
      begin
        TargeTBaseObject := TBaseObject(ProperTargetList.Items[Random(nAttackCount)]);
        if BaseObject.IsProperTarget(TargeTBaseObject) then
        begin
          // 英雄、人形怪断筋处理 piaoyun 2013-10-26
          if ((not g_Config.boSKILL208HeroDuanJin) and (TargeTBaseObject.m_btRaceServer = RC_HEROOBJECT)) or ((not g_Config.boSKILL208PlayMosterDuanJin)
            and (TargeTBaseObject.m_btRaceServer = RC_PLAYMOSTER)) then
            Continue;

          TargeTBaseObject.m_boCobwebWindingStatus := True;
          TargeTBaseObject.m_boDuanJin := True;
          TargeTBaseObject.m_dwCobwebWindingStatusTick := MyGetTickCount + LongWord(n) * 2 * 1000;
          TargeTBaseObject.SendRefMsg(RM_OPENCOBWEBWINDING, 0, NativeInt(TargeTBaseObject), 1, 0, '');
          TargeTBaseObject.SysMsg(Format(g_sCanNotRun, [n * 2]), g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
        end;
      end;
    end
    else
    begin
      for I := 0 to ProperTargetList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(ProperTargetList.Items[I]);
        if BaseObject.IsProperTarget(TargeTBaseObject) then
        begin
          // 英雄、人形怪断筋处理 piaoyun 2013-10-26
          if ((not g_Config.boSKILL208HeroDuanJin) and (TargeTBaseObject.m_btRaceServer = RC_HEROOBJECT)) or ((not g_Config.boSKILL208PlayMosterDuanJin)
            and (TargeTBaseObject.m_btRaceServer = RC_PLAYMOSTER)) then
            Continue;

          TargeTBaseObject.m_boCobwebWindingStatus := True;
          TargeTBaseObject.m_dwCobwebWindingStatusTick := MyGetTickCount + LongWord(n) * 2 * 1000;
          TargeTBaseObject.m_boDuanJin := True;
          TargeTBaseObject.SendRefMsg(RM_OPENCOBWEBWINDING, 0, NativeInt(TargeTBaseObject), 1, 0, '');
          TargeTBaseObject.SysMsg(Format(g_sCanNotRun, [n * 2]), g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
        end;
      end;
    end;
    Result := True;
  finally
    ProperTargetList.Free;
    BaseObjectList.Free;
  end;
end;

// 五雷轰技能函数 piaoyun 2013-09-14
function TMagicManager.MagMakeSkill209(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  PlayObject: TPlayObject;
  nPowerPoint, nPower2: Integer;
  nRage: Integer;
  nAttackCount: Integer; // 能够攻击的对象数量
  SmartObject: TSmartObject;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  // 威力倍数
  nPowerPoint := Round(nPower / 100 * g_Config.nSkill209PowerRate);
  nRage := g_Config.nSkill209Rage;
  nPowerPoint := GetNewLevelPower(nPowerPoint, UserMagic); // 取强化技能攻击伤害
  PlayObject := TPlayObject(BaseObject);
  BaseObjectList := TList.Create;
  try
    // 遍历范围内的对象
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

    // 第一轮，先计算出有多少对象能被攻击
    nAttackCount := 0;
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if BaseObject.IsProperTarget(TargeTBaseObject) then
        Inc(nAttackCount);
    end;

    if nAttackCount = 0 then
      Exit;

    nPowerPoint := nPowerPoint div nAttackCount; // 平分技能伤害
    // 第二轮，开始攻击
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);

      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower2 := nPowerPoint;
        // 伤害吸收 chongchong 2016-05-12
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
            if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin // 吸收伤害
            if Random(100) < SmartObject.m_nSuckDamageProbability then
            begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
            end;
            end;
          }
        end;
        if FNpcReleaseMagic <> 0 then
          PlayObject.SendMsg(PlayObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(0, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          PlayObject.SendDelayMsg(PlayObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(0, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 600);
      end;
    end;
    Result := True;
  finally
    BaseObjectList.Free;
  end;
end;

// 幽冥火符技能函数 piaoyun 2013-09-14
function TMagicManager.MagMakeSkill210(BaseObject: TBaseObject; UserMagic: pTUserMagic; nPower, nX, nY: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  nPowerPoint, nPower2: Integer;
  nRage: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_PLAYMOSTER, RC_HEROOBJECT]) then
    Exit;

  // 威力倍数
  nPowerPoint := Round(nPower / 100 * g_Config.nSkill210PowerRate);
  nRage := g_Config.nSkill210Rage;
  BaseObjectList := TList.Create;
  try
    // 遍历范围内的对象
    BaseObject.GetMapBaseObjects(BaseObject.m_PEnvir, nX, nY, nRage, BaseObjectList);

    // 开始攻击遍历出来的链表对象
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);

      // 检测目标是否正确
      if BaseObject.IsProperTarget(TargeTBaseObject) then
      begin
        nPower2 := nPowerPoint;
        // 伤害吸收 chongchong 2016-05-12
        if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        begin
          SmartObject := TSmartObject(TargeTBaseObject);
          if SmartObject.m_boTrainingNG and (SmartObject.m_AbilNG.NH >= g_Config.nNGHitStruckDecNG) then
          begin
            nPower2 := Max(0, nPower2 - SmartObject.GetNGDecPower);
            SmartObject.m_AbilNG.NH := Max(0, SmartObject.m_AbilNG.NH - g_Config.nNGHitStruckDecNG);
            SmartObject.RefAbilNH;
          end;
          {
            // 这里有计算TBaseObject.ClientMagStruck 2020-09-15
            if (SmartObject.m_nSuckDamagePoint > 0) and (nPower2 > 0) and (SmartObject.m_nSuckDamageRate > 0) then
            begin // 吸收伤害
            if Random(100) < SmartObject.m_nSuckDamageProbability then
            begin
            nSuckDamagePoint := Round(SmartObject.m_nSuckDamageRate / 1000 * nPower2);
            if nSuckDamagePoint > SmartObject.m_nSuckDamagePoint then
            nSuckDamagePoint := SmartObject.m_nSuckDamagePoint;
            Dec(SmartObject.m_nSuckDamagePoint, nSuckDamagePoint);
            nPower2 := Max(nPower2 - nSuckDamagePoint, 0);
            end;
            end;
          }
        end;

        if FNpcReleaseMagic <> 0 then
          BaseObject.SendMsg(BaseObject, RM_DELAYMAGIC_EX, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(0, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx))
        else
          BaseObject.SendDelayMsg(BaseObject, RM_DELAYMAGIC, nPower2, MakeLong(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY),
            MakeLong(0, 0), NativeInt(TargeTBaseObject), IntToStr(UserMagic.wMagIdx), 600);
      end;
    end;
    Result := True;
  finally
    BaseObjectList.Free;
  end;
end;

function TMagicManager.MagCapturePets(BaseObject: TSmartObject; TargeTBaseObject: TBaseObject; nTargetX, nTargetY, nMagicLevel:
  Integer): Boolean;
var
  StdItem: pTStdItem;
  Player: TPlayObject;
  UserItem: pTUserItem;
  GamePetConfig: PTGamePetConfig;
  UseItemWhere: Integer;
  IsCanCapture: Boolean;
begin
  Result := False;
  if BaseObject.m_btRaceServer <> RC_PLAYOBJECT then
    Exit;

  Player := TPlayObject(BaseObject);

  UseItemWhere := -1;
  if g_Config.boCapturePetNeedItem then
  begin
    IsCanCapture := False;
    if Player.m_UseItems[U_CHARM].wIndex <> 0 then
    begin
      StdItem := UserEngine.GetStdItem(Player.m_UseItems[U_CHARM].wIndex);
      if StdItem <> nil then
      begin
        if (StdItem.StdMode = 94) then
        begin
          if Player.m_UseItems[U_CHARM].Dura >= g_Config.nMagicItemRate then
          begin
            IsCanCapture := True;
            UseItemWhere := U_CHARM;
            if not g_Config.boCaptureOKDecDura then
            begin
              Dec(Player.m_UseItems[U_CHARM].Dura, g_Config.nMagicItemRate);
              if Player.m_UseItems[U_CHARM].Dura <= 0 then
              begin
                if (StdItem.NeedIdentify = 1) then
                begin
                  AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Player, StdItem.Name, Player.m_UseItems[U_CHARM].MakeIndex,
                    '0', 0, 0, '0持久消失');
                end;

                Player.SendDelItem(@Player.m_UseItems[U_CHARM]);
                Player.m_UseItems[U_CHARM].wIndex := 0;
                Player.RecalcAbilitys();
              end
              else
                Player.SendUpdateItemDura(U_CHARM, Player.m_UseItems[U_CHARM].MakeIndex, False, Player.m_UseItems[U_CHARM].Dura);
            end;
          end
          else
          begin
            Player.SysMsg(StdItem.Name + '持久不够', g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
            Exit;
          end;
        end;
      end;
    end;

    if not IsCanCapture then
    begin
      if Player.m_UseItems[U_ARMRINGL].wIndex <> 0 then
      begin
        StdItem := UserEngine.GetStdItem(Player.m_UseItems[U_ARMRINGL].wIndex);
        if StdItem <> nil then
        begin
          if (StdItem.StdMode = 94) then
          begin
            if Player.m_UseItems[U_ARMRINGL].Dura >= g_Config.nMagicItemRate then
            begin
              IsCanCapture := True;
              UseItemWhere := U_ARMRINGL;
              if not g_Config.boCaptureOKDecDura then
              begin
                Dec(Player.m_UseItems[U_ARMRINGL].Dura, g_Config.nMagicItemRate);
                if Player.m_UseItems[U_ARMRINGL].Dura <= 0 then
                begin
                  if (StdItem.NeedIdentify = 1) then
                  begin
                    AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Player, StdItem.Name, Player.m_UseItems[U_ARMRINGL].MakeIndex,
                      '0', 0, 0, '0持久消失');
                  end;
                  Player.SendDelItem(@Player.m_UseItems[U_ARMRINGL]);
                  Player.m_UseItems[U_ARMRINGL].wIndex := 0;
                  Player.RecalcAbilitys();
                end
                else
                  Player.SendUpdateItemDura(U_ARMRINGL, Player.m_UseItems[U_ARMRINGL].MakeIndex, False, Player.m_UseItems[U_ARMRINGL].Dura);
              end;
            end
            else
            begin
              Player.SysMsg(StdItem.Name + '持久不够', g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
              Exit;
            end;
          end;
        end;
      end;
    end;

    if not IsCanCapture then
    begin
      Player.SysMsg(g_sNotFoundCaptureItem, g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
      Exit;
    end;
  end;

  if (TargeTBaseObject <> nil) //
    and (TargeTBaseObject.m_btRaceServer >= RC_ANIMAL) //
    and (not (TargeTBaseObject.m_btRaceServer in [RC_PLAYMOSTER { 人形怪 }, RC_ARCHERGUARD { 弓箭手 }, RC_MOVE_ARCHERGUARD
    { 巡回弓箭手 }, RC_TRUCKOBJECT { 押镖车 }, 55 { 练功师 }, 110, 111 { 沙巴克城墙 } ])) //
    and (TargeTBaseObject.m_Master = nil) //
    and (not TargeTBaseObject.m_boGhost) and (not TargeTBaseObject.m_boDeath) then
  begin
    GamePetConfig := GetGamePetConfig(TargeTBaseObject.m_sCharName);

    if (GamePetConfig <> nil) and (GamePetConfig.CaptureRate > 0) and Player.IsEnoughBag then
    begin
      IsCanCapture := True;

      if GamePetConfig.EnabledLevelDifference then
      begin
        if GamePetConfig.LevelDifference > 0 then
          IsCanCapture := Int64(Player.m_Abil.Level) >= Int64(TargeTBaseObject.m_Abil.Level + GamePetConfig.LevelDifference)
        else
          IsCanCapture := Int64(Player.m_Abil.Level - GamePetConfig.LevelDifference) >= Int64(TargeTBaseObject.m_Abil.Level);
      end;

      if IsCanCapture then
        IsCanCapture := TargeTBaseObject.m_WAbil.HP <= Round(TargeTBaseObject.m_WAbil.MaxHP / 100 * GamePetConfig.HPScale);

      TargeTBaseObject.m_TargetCret := nil;

      if IsCanCapture then
      begin
        if Random(GamePetConfig.CaptureRate) = 0 then
        begin
          TargeTBaseObject.m_CurrTarget := nil;
          TargeTBaseObject.m_LastHiter := nil;
          TargeTBaseObject.m_ExpHitter := nil;
          TargeTBaseObject.m_PoisonHitter := nil;
          TargeTBaseObject.m_CurrTargetEx := nil;
          TargeTBaseObject.m_Master := nil;
          TargeTBaseObject.m_boGhost := True;
          TargeTBaseObject.m_dwGhostTick := MyGetTickCount();
          TargeTBaseObject.m_CurrTarget := nil;

          TargeTBaseObject.m_PEnvir.DeleteFromMap(TargeTBaseObject.m_nCurrX, TargeTBaseObject.m_nCurrY, TargeTBaseObject);
          TargeTBaseObject.SendRefMsg(RM_DISAPPEAR, 0, 0, 0, 0, '', 600);

          New(UserItem);
          if UserEngine.CopyToUserItemFromName('宠物蛋', UserItem) then
          begin
            UserItem.ItemFrom.ItemForm := ifCaptureMon;
            UserItem.ItemFrom.sMakerName := Player.m_sCharName;
            UserItem.ItemFrom.DateTime := Now();
            UserItem.btValue[13] := 1;
            UserItem.Name := TargeTBaseObject.m_sCharName;

            ObjectToUserItem(TargeTBaseObject, UserItem);

            // 还是重算一下宠物蛋的其他属性（攻击，魔法，道术等） 2019-05-22 15:34:03
            RecallGamePetAbilToUserItem(UserItem.Name, BaseObject.m_WAbil.Level, UserItem);

            Player.m_ItemList.Add(UserItem);
            Player.SendAddItem(UserItem);

            if g_Config.boCapturePetNeedItem and g_Config.boCaptureOKDecDura and (UseItemWhere <> -1) then
            begin
              Dec(Player.m_UseItems[UseItemWhere].Dura, g_Config.nMagicItemRate);
              if Player.m_UseItems[UseItemWhere].Dura <= 0 then
              begin
                Player.SendDelItem(@Player.m_UseItems[UseItemWhere]);
                Player.m_UseItems[UseItemWhere].wIndex := 0;
                Player.RecalcAbilitys();
              end
              else
                Player.SendUpdateItem(@Player.m_UseItems[UseItemWhere]);
            end;
          end
          else
            Dispose(UserItem);
        end;
      end;
      Result := True;
    end;
  end;
end;

end.

