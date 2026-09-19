unit HandleCommands;

interface

uses
  Windows, SysUtils, Classes, Grobal2, ObjBase, RunSock, ObjPlayer, M2Share,

{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  M2Threads, M2DataCommon, M2Definition;

type
  TAddOnCheck = function(AUserItem: pTUserItem): Boolean;

procedure ProcessUserLineMsg(PlayObject: TPlayObject; sData: string);

procedure CmdChangeAdminMode(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string; boFlag: Boolean);

procedure CmdChangeAttackMode(PlayObject: TPlayObject; nMode: Integer; sParam1, sParam2, sParam3, sParam4, sParam5, sParam6,
  sParam7: string);

procedure CmdChangeSuperManMode(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string; boFlag: Boolean);

procedure CmdChangeObMode(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string; boFlag: Boolean);

procedure CmdMakeItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sItemName: string; nCount: Integer; sParam1, sParam2: string);

procedure CmdDeleteItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sItemName: string; nCount: Integer;
  AddOnCheck: TAddOnCheck = nil);

implementation

uses
  HUtil32, LocalDB, SDK, Math, GameEvent, Envir, ObjNpc, ObjHero, Guild, Castle, uCustomMagicUtils;

procedure CmdTrainingMagic(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sSkillName: string; nLevel, nNewLevel: Integer);
var
  Magic: pTMagic;
  UserMagic: pTUserMagic;
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
  btOldHitPoint: Byte;
  CustomMagicConfig: TCustomMagicConfig;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sHumanName <> '') and (sHumanName[1] = '?')) //
      or (sHumanName = '') //
      or (sSkillName = '') //
      or (nLevel < 0) //
      or not(nLevel in [0 .. 3]) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称  技能名称 修炼等级(0-3)', c_Red, t_Hint);
      Exit;
    end;

    Magic := nil;
    HeroObject := nil;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject = nil then
      begin
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
        Exit;
      end;
    end;

    if OnlineObject <> nil then
    begin
      Magic := UserEngine.FindMagic(sSkillName);
    end
    else if HeroObject <> nil then
    begin
      Magic := UserEngine.FindHeroMagic(sSkillName);
    end;

    if Magic = nil then
    begin
      SysMsg(Format('%s 技能名称不正确！', [sSkillName]), c_Red, t_Hint);
      Exit;
    end;

    if (OnlineObject <> nil) and OnlineObject.IsTrainingSkill(Magic.wMagicId, Magic.MagicAttr) then
    begin
      SysMsg(Format('%s 技能已修炼过了！', [sSkillName]), c_Red, t_Hint);
      Exit;
    end
    else if (HeroObject <> nil) and HeroObject.IsTrainingSkill(Magic.wMagicId, Magic.MagicAttr) then
    begin
      SysMsg(Format('%s 技能已修炼过了！', [sSkillName]), c_Red, t_Hint);
      Exit;
    end;

    New(UserMagic);
    UserMagic.MagicInfo := Magic;
    UserMagic.MagicAttr := Magic.MagicAttr;
    UserMagic.wMagIdx := Magic.wMagicId;
    UserMagic.btLevel := nLevel;

    if (Magic.MagicAttr in [mtDefense, mtAttack]) or ((OnlineObject <> nil) and OnlineObject.m_boDummyObject) or
      (HeroObject <> nil) then
      UserMagic.btKey := VK_F1
    else
      UserMagic.btKey := 0;

    UserMagic.btNewLevel := Min(100, nNewLevel);
    UserMagic.nTranPoint := 0;
    UserMagic.boUsesItemAdd := False;
    if OnlineObject <> nil then
    begin
      OnlineObject.m_MagicList.Add(UserMagic);
      OnlineObject.SendAddMagic(UserMagic);

      if (UserMagic.wMagIdx = SKILL_ERGUM) and (OnlineObject.m_MagicErgumSkill = nil) then
      begin
        OnlineObject.m_MagicErgumSkill := UserMagic;
        if (not OnlineObject.m_boUseThrusting) then
          OnlineObject.ThrustingOnOff(True);

        // OnlineObject.SendSocket(nil, '+LNG');
        OnlineObject.SendOpenMagic(SKILL_ERGUM, True, 0);

        if g_FunctionNPC <> nil then
        begin
          OnlineObject.m_nLearnMagicID := Magic.wMagicId;
          g_FunctionNPC.GotoLable(OnlineObject, '@LearnMagic', False);
          OnlineObject.m_nLearnMagicID := 0;
        end;
      end
      else if CheckIsCustomMagic(UserMagic.wMagIdx) then
      begin
        CustomMagicConfig := GetCustomMagicConfig(UserMagic.wMagIdx);
        if (CustomMagicConfig <> nil) and (CustomMagicConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch) and
          (not OnlineObject.m_boCustomSkill[UserMagic.wMagIdx - CUSTOM_MAGIC_START_ID]) then
        begin
          if (CustomMagicConfig.ClientBaseConfig.MagicWarrNGOption = mngoLongHit) or CustomMagicConfig.ClientBaseConfig.MagicAutoOpen
          then
          begin
            OnlineObject.m_boCustomSkill[UserMagic.wMagIdx - CUSTOM_MAGIC_START_ID] := True;
            OnlineObject.SendOpenMagic(UserMagic.wMagIdx, True, 0);
          end;
        end;
      end;

      // 脚本自动开启刺杀剑术 -- piaoyun 2013-07-20
      if (UserMagic.wMagIdx = SKILL_ERGUM) //
        and (OnlineObject.m_MagicErgumSkill <> nil) //
        and (not OnlineObject.m_boUseThrusting) then
      begin
        OnlineObject.ThrustingOnOff(True);
        // OnlineObject.SendSocket(nil, '+LNG');
        OnlineObject.SendOpenMagic(SKILL_ERGUM, True, 0);
      end;

      btOldHitPoint := OnlineObject.m_btHitPoint;
      OnlineObject.RecalcAbilitys;

      if btOldHitPoint <> OnlineObject.m_btHitPoint then
        OnlineObject.SendMsg(OnlineObject, RM_SUBABILITY, 0, 0, 0, 0, '');
    end
    else if HeroObject <> nil then
    begin
      HeroObject.m_MagicList.Add(UserMagic);
      HeroObject.SendAddMagic(UserMagic);

      btOldHitPoint := HeroObject.m_btHitPoint;
      HeroObject.RecalcAbilitys;
      if btOldHitPoint <> HeroObject.m_btHitPoint then
      begin
        HeroObject.SendMsg(HeroObject, RM_SUBABILITY, 0, 0, 0, 0, '');
      end;
    end;

    SysMsg(Format('%s 的 %s 技能修炼成功！', [sHumanName, sSkillName]), c_Green, t_Hint);
  end;
end;

procedure CmdTrainingSkill(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sSkillName: string; nLevel: Integer);
var
  I: Integer;
  UserMagic: pTUserMagic;
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
begin
  HeroObject := nil;
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sHumanName = '') or (sSkillName = '') or (nLevel <= 0) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称  技能名称 修炼等级(0-3)', c_Red, t_Hint);
      Exit;
    end;
    nLevel := Min(3, nLevel);
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject = nil then
      begin
        SysMsg(Format('%s不在线，或在其它服务器上！！', [sHumanName]), c_Red, t_Hint);
        Exit;
      end;
    end;
    if OnlineObject <> nil then
    begin
      for I := 0 to OnlineObject.m_MagicList.Count - 1 do
      begin
        UserMagic := OnlineObject.m_MagicList.Items[I];
        if CompareText(UserMagic.MagicInfo.sMagicName, sSkillName) = 0 then
        begin
          UserMagic.btLevel := nLevel;
          OnlineObject.SendMsg(OnlineObject, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
            MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
          OnlineObject.SysMsg(Format('%s的修改炼等级为%d', [sSkillName, nLevel]), c_Green, t_Hint);
          SysMsg(Format('%s的技能%s修炼等级为%d', [sHumanName, sSkillName, nLevel]), c_Green, t_Hint);
          Break;
        end;
      end;
    end
    else if HeroObject <> nil then
    begin
      for I := 0 to HeroObject.m_MagicList.Count - 1 do
      begin
        UserMagic := HeroObject.m_MagicList.Items[I];
        if CompareText(UserMagic.MagicInfo.sMagicName, sSkillName) = 0 then
        begin
          UserMagic.btLevel := nLevel;
          HeroObject.SendMsg(HeroObject, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
            MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)));
          HeroObject.SysMsg(Format('%s的修改炼等级为%d', [sSkillName, nLevel]), c_Green, t_Hint);
          SysMsg(Format('%s的技能%s修炼等级为%d', [sHumanName, sSkillName, nLevel]), c_Green, t_Hint);
          Break;
        end;
      end;
    end;
  end;
end;

procedure CmdAddGameGold(PlayObject: TPlayObject; sCmd, sHumName: string; nPoint: Integer);
var
  OnlineObject: TPlayObject;
  nOldValue: LongWord;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;
    if (sHumName = '') or (nPoint <= 0) then
    begin
      SysMsg('命令格式: @' + sCmd + ' 人物名称  金币数量', c_Red, t_Hint);
      Exit;
    end;
    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      nOldValue := OnlineObject.m_nGameGold;

      if (OnlineObject.m_nGameGold + nPoint) < 2000000 then
      begin
        Inc(OnlineObject.m_nGameGold, nPoint);
      end
      else
      begin
        nPoint := 2000000 - OnlineObject.m_nGameGold;
        OnlineObject.m_nGameGold := 2000000;
      end;
      OnlineObject.GoldChanged();

      if g_boGameLogGameGold then
      begin
        AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, OnlineObject, g_Config.sGameGoldName, 0, PlayObject.m_sCharName,
          OnlineObject.m_nGameGold, nOldValue, '@' + sCmd + ' ' + sHumName + ' ' + IntToStr(nPoint));
      end;

      SysMsg(sHumName + '的游戏点已增加' + IntToStr(nPoint) + '.', c_Green, t_Hint);
      OnlineObject.SysMsg('游戏点已增加' + IntToStr(nPoint) + '.', c_Green, t_Hint);
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdDelGameGold(PlayObject: TPlayObject; sCmd, sHumName: string; nPoint: Integer);
var
  OnlineObject: TPlayObject;
  nOldValue: LongWord;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;
    if (sHumName = '') or (nPoint <= 0) then
      Exit;
    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      nOldValue := OnlineObject.m_nGameGold;

      if OnlineObject.m_nGameGold > nPoint then
      begin
        Dec(OnlineObject.m_nGameGold, nPoint);
      end
      else
      begin
        nPoint := OnlineObject.m_nGameGold;
        OnlineObject.m_nGameGold := 0;
      end;
      OnlineObject.GoldChanged();

      if g_boGameLogGameGold then
      begin
        AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, OnlineObject, g_Config.sGameGoldName, 0, PlayObject.m_sCharName,
          OnlineObject.m_nGameGold, nOldValue, '@' + sCmd + ' ' + sHumName + ' ' + IntToStr(nPoint));
      end;

      SysMsg(sHumName + '的游戏点已减少' + IntToStr(nPoint) + '.', c_Green, t_Hint);
      OnlineObject.SysMsg('游戏点已减少' + IntToStr(nPoint) + '.', c_Green, t_Hint);
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdGameGold(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string; sCtr: string; nGold: Int64);
var
  Ctr: Char;
  OnlineObject: TPlayObject;
  nChangeValue: Int64;
  nNewGold, OldGameGold: LongWord;
begin
  if nGold > High(LongWord) then
    nGold := High(LongWord);

  nNewGold := nGold;

  Ctr := '1';
  if (PlayObject.m_btPermission < Cmd.nPermissionMin) then
  begin
    PlayObject.SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
    Exit;
  end;
  if (sCtr <> '') then
  begin
    Ctr := sCtr[1];
  end;

{$IF CompilerVersion >= 22}
  if (sHumanName = '') or not CharInSet(Ctr, ['=', '+', '-']) or (nGold < 0) or ((sHumanName <> '') and (sHumanName[1] = '?'))
  then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandGameGoldHelpMsg]), c_Red, t_Hint);
    Exit;
  end;
{$ELSE}
  if (sHumanName = '') or not(Ctr in ['=', '+', '-']) or (nGold < 0) or ((sHumanName <> '') and (sHumanName[1] = '?')) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandGameGoldHelpMsg]), c_Red, t_Hint);
    Exit;
  end;
{$IFEND}
  OnlineObject := UserEngine.GetPlayObject(sHumanName);
  if OnlineObject = nil then
  begin
    PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    Exit;
  end;

  OldGameGold := OnlineObject.m_nGameGold;
  nChangeValue := 0;

  case sCtr[1] of
    '=':
      begin
        nChangeValue := nGold - OnlineObject.m_nGameGold;
        OnlineObject.m_nGameGold := nGold;
      end;
    '+':
      begin
        nGold := nGold + OnlineObject.m_nGameGold;
        if nGold > High(LongWord) then
          nGold := High(LongWord);
        nChangeValue := nGold - OnlineObject.m_nGameGold;
        OnlineObject.m_nGameGold := nGold;
      end;
    '-':
      begin
        nGold := OnlineObject.m_nGameGold - nGold;
        if nGold < 0 then
          nGold := 0;
        nChangeValue := nGold - OnlineObject.m_nGameGold;
        OnlineObject.m_nGameGold := nGold;
      end;
  end;

  if g_boGameLogGameGold then
  begin
    AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, OnlineObject, g_Config.sGameGoldName, 0, PlayObject.m_sCharName,
      OnlineObject.m_nGameGold, OldGameGold, '@' + Cmd.sCmd + ' ' + sHumanName + ' ' + sCtr[1] + ' ' + IntToStr(nNewGold));
  end;

  OnlineObject.GameGoldChanged();
  OnlineObject.SysMsg(Format(g_sGameCommandGameGoldHumanMsg, [g_Config.sGameGoldName, nChangeValue, OnlineObject.m_nGameGold,
    g_Config.sGameGoldName]), c_Green, t_Hint);
  PlayObject.SysMsg(Format(g_sGameCommandGameGoldGMMsg, [sHumanName, g_Config.sGameGoldName, nChangeValue,
    OnlineObject.m_nGameGold, g_Config.sGameGoldName]), c_Green, t_Hint);
end;

procedure CmdGamePoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sCtr: string; nPoint: Int64);
var
  Ctr: Char;
  OnlineObject: TPlayObject;
  nChangeValue: Int64;
  nOldGamePoint: LongWord;
begin
  if nPoint > High(LongWord) then
    nPoint := High(LongWord);

  with PlayObject do
  begin
    Ctr := '1';
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sCtr <> '') then
    begin
      Ctr := sCtr[1];
    end;
{$IF CompilerVersion >= 22}
    if (sHumanName = '') or not CharInSet(Ctr, ['=', '+', '-']) or (nPoint < 0) or ((sHumanName <> '') and (sHumanName[1] = '?'))
    then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandGamePointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
{$ELSE}
    if (sHumanName = '') or not(Ctr in ['=', '+', '-']) or (nPoint < 0) or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandGamePointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
{$IFEND}
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    nChangeValue := 0;
    nOldGamePoint := OnlineObject.m_nGamePoint;
    case sCtr[1] of
      '=':
        begin
          nChangeValue := nPoint - OnlineObject.m_nGamePoint;
          OnlineObject.m_nGamePoint := nPoint;
        end;
      '+':
        begin
          nPoint := nPoint + OnlineObject.m_nGamePoint;
          if nPoint > High(LongWord) then
            nPoint := High(LongWord);

          nChangeValue := nPoint - OnlineObject.m_nGamePoint;
          OnlineObject.m_nGamePoint := nPoint;
        end;
      '-':
        begin
          nPoint := OnlineObject.m_nGamePoint - nPoint;
          if nPoint < 0 then
            nPoint := 0;

          nChangeValue := nPoint - OnlineObject.m_nGamePoint;
          OnlineObject.m_nGamePoint := nPoint;
        end;
    end;

    if g_boGameLogGamePoint then
    begin
      AddGameDataLog(LOG_GamePointChange, LOG_ActionNone, OnlineObject, g_Config.sGamePointName, 0, PlayObject.m_sCharName,
        OnlineObject.m_nGamePoint, nOldGamePoint, '@' + Cmd.sCmd + ' ' + sCtr[1] + ' ' + IntToStr(nPoint));
    end;

    OnlineObject.GameGoldChanged();
    OnlineObject.SysMsg(Format(g_sGameCommandGamePointHumanMsg, [g_Config.sGamePointName, nChangeValue, OnlineObject.m_nGamePoint]
      ), c_Green, t_Hint);
    SysMsg(Format(g_sGameCommandGamePointGMMsg, [sHumanName, g_Config.sGamePointName, nChangeValue, OnlineObject.m_nGamePoint]),
      c_Green, t_Hint);
  end;
end;

procedure CmdCreditPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sCtr: string; nPoint: Integer);
var
  OnlineObject: TPlayObject;
  Ctr: Char;
  nCreditPoint: Int64;
  nOldCreditPoint: Integer;
begin
  Ctr := '1';
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sCtr <> '') then
    begin
      Ctr := sCtr[1];
    end;
{$IF CompilerVersion >= 22}
    if (sHumanName = '') or not CharInSet(Ctr, ['=', '+', '-']) or (nPoint < 0) or ((sHumanName <> '') and (sHumanName[1] = '?'))
    then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandCreditPointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
{$ELSE}
    if (sHumanName = '') or not(Ctr in ['=', '+', '-']) or (nPoint < 0) or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandCreditPointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
{$IFEND}
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    nOldCreditPoint := OnlineObject.m_WAbil.CreditPoint;
    case sCtr[1] of
      '=':
        begin
          OnlineObject.m_WAbil.CreditPoint := nPoint;
        end;
      '+':
        begin
          nCreditPoint := Int64(OnlineObject.m_WAbil.CreditPoint) + nPoint;
          if nCreditPoint > High(Integer) then
            nCreditPoint := High(Integer);
          OnlineObject.m_WAbil.CreditPoint := nCreditPoint;
        end;
      '-':
        begin
          nCreditPoint := Int64(PlayObject.m_WAbil.CreditPoint) - nPoint;
          if nCreditPoint < 0 then
            nCreditPoint := 0;
          PlayObject.m_WAbil.CreditPoint := nCreditPoint;
        end;
    end;

    // 声望日志
    AddGameDataLog(LOG_CreditPointChange, LOG_ActionNone, latHuman, '0', 0, 0, g_Config.sCreditPointName, 0, sHumanName,
      OnlineObject.m_sCharName, PlayObject.m_WAbil.CreditPoint, nOldCreditPoint, '@' + Cmd.sCmd + ' ' + sHumanName + ' ' + sCtr +
      ' ' + IntToStr(nPoint));

    OnlineObject.SysMsg(Format(g_sGameCommandCreditPointHumanMsg, [nPoint, OnlineObject.m_WAbil.CreditPoint - nOldCreditPoint]),
      c_Green, t_Hint);
    SysMsg(Format(g_sGameCommandCreditPointGMMsg, [sHumanName, nPoint, OnlineObject.m_WAbil.CreditPoint - nOldCreditPoint]),
      c_Green, t_Hint);
    SendMsg(PlayObject, RM_ABILITY, 0, 0, 0, 0, '');
  end;
end;

procedure CmdAddGold(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumName: string; nCount: Integer);
var
  OnlineObject: TPlayObject;
  Int64Value: Int64;
  OldGold: LongWord;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;
    if (sHumName = '') or (nCount <= 0) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称  金币数量', c_Red, t_Hint);
      Exit;
    end;
    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      OldGold := OnlineObject.m_nGold;
      Int64Value := Int64(OnlineObject.m_nGold) + nCount;

      if Int64Value > g_Config.nHumanMaxGold then
        Int64Value := g_Config.nHumanMaxGold;

      nCount := Int64Value - OnlineObject.m_nGold;

      OnlineObject.m_nGold := Int64Value;
      OnlineObject.GoldChanged();
      SysMsg(sHumName + '的金币已增加' + IntToStr(nCount) + '.', c_Green, t_Hint);

      if g_boGameLogGold then
        AddGameDataLog(LOG_GoldChange, LOG_ActionNone, OnlineObject, sSTRING_GOLDNAME, 0, m_sCharName, OnlineObject.m_nGold,
          OldGold, '@' + Cmd.sCmd + ' ' + sHumName + ' ' + IntToStr(nCount));
    end
    else
    begin
      DataEngine.HumanChangeGold(nil, nil, cgtGold, m_sCharName, sHumName, nCount);
      SysMsg(sHumName + ' 现在不在线，等其上线时金币将自动增加', c_Green, t_Hint);

      if g_boGameLogGold then
        AddGameDataLog(LOG_GoldChange, LOG_ActionNone, latHuman, '0', 0, 0, sSTRING_GOLDNAME, 0, sHumName, m_sCharName, 0, nCount,
          '@' + Cmd.sCmd + ' ' + sHumName + ' ' + IntToStr(nCount));
    end;
  end;
end;

procedure CmdAddGuild(PlayObject: TPlayObject; Cmd: pTGameCmd; sGuildName, sGuildChief: string); // 004CEBA0
var
  Human: TPlayObject;
  boAddState: Boolean;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if nServerIndex <> 0 then
    begin
      SysMsg('这个命令只能使用在主服务器上', c_Red, t_Hint);
      Exit;
    end;
    if (sGuildName = '') or (sGuildChief = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 行会名称 掌门人名称', c_Red, t_Hint);
      Exit;
    end;

    boAddState := False;
    Human := UserEngine.GetPlayObject(sGuildChief);
    if Human = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sGuildChief]), c_Red, t_Hint);
      Exit;
    end;
    if g_GuildManager.MemberOfGuild(sGuildChief) = nil then
    begin
      if g_GuildManager.AddGuild(sGuildName, sGuildChief) then
      begin
        SysMsg('行会名称: ' + sGuildName + ' 掌门人: ' + sGuildChief, c_Green, t_Hint);
        boAddState := True;
      end;
    end;
    if boAddState then
    begin
      Human.m_MyGuild := TObject(g_GuildManager.MemberOfGuild(Human.m_sCharName));
      if Human.m_MyGuild <> nil then
      begin
        Human.m_sGuildRankName := TGUild(Human.m_MyGuild).GetRankName2(PlayObject, Human.m_nGuildRankNo);
        Human.RefShowName();

        // 创建行会触发 chongchong 2013-10-28
        if (g_FunctionNPC <> nil) then
        begin
          Human.m_nScriptGotoCount := 0;
          g_FunctionNPC.GotoLable(Human, '@CreateGuild', False);
        end;
      end;
    end;
  end;
end;

procedure CmdAdjuestExp(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sExp: string);
var
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
  dwExp: LongWord;
  dwOExp: LongWord;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sHumanName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 经验值', c_Red, t_Hint);
      Exit;
    end;
    dwExp := StrToIntDef(sExp, 0);
    // if dwExp < 0 then dwExp := 0;

    HeroObject := nil;
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
    end;

    if OnlineObject <> nil then
    begin
      dwOExp := OnlineObject.m_Abil.Exp;
      OnlineObject.m_Abil.Exp := dwExp;
      OnlineObject.HasLevelUp(1);
      SysMsg(sHumanName + ' 经验调整完成。', c_Green, t_Hint);
      if g_Config.boShowMakeItemMsg then
        MainOutMessage('[经验调整] ' + m_sCharName + '(' + OnlineObject.m_sCharName + ' ' + IntToStr(dwOExp) + ' -> ' +
          IntToStr(OnlineObject.m_Abil.Exp) + ')');
    end
    else if HeroObject <> nil then
    begin
      dwOExp := HeroObject.m_Abil.Exp;
      HeroObject.m_Abil.Exp := dwExp;
      HeroObject.HasLevelUp(1);
      SysMsg(sHumanName + ' 经验调整完成。', c_Green, t_Hint);
      if g_Config.boShowMakeItemMsg then
        MainOutMessage('[经验调整] ' + m_sCharName + '(' + HeroObject.m_sCharName + ' ' + IntToStr(dwOExp) + ' -> ' +
          IntToStr(HeroObject.m_Abil.Exp) + ')');
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdAdjuestLevel(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string; nLevel: Int64);
var
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
  nOLevel: LongWord;
begin
  if nLevel > High(LongWord) then
    nLevel := High(LongWord);
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if sHumanName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 等级', c_Red, t_Hint);
      Exit;
    end;
    HeroObject := nil;
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
    end;

    if OnlineObject <> nil then
    begin
      nOLevel := OnlineObject.m_Abil.Level;

      if g_Config.btMaxLevel = 0 then
        OnlineObject.m_Abil.Level := Max(1, Min(MAXUPLEVEL, nLevel))
      else if g_Config.btMaxLevel = 1 then
        OnlineObject.m_Abil.Level := Max(1, Min(High(LongInt), nLevel))
      else
        OnlineObject.m_Abil.Level := Max(1, Min(High(LongWord), nLevel));

      OnlineObject.HasLevelUp(1);
      SysMsg(sHumanName + ' 等级调整完成。', c_Green, t_Hint);
      if g_Config.boShowMakeItemMsg then
        MainOutMessage('[等级调整] ' + m_sCharName + '(' + OnlineObject.m_sCharName + ' ' + IntToStr(nOLevel) + ' -> ' +
          IntToStr(OnlineObject.m_Abil.Level) + ')');

      AddGameDataLog(LOG_LevelChange, LOG_ActionNone, OnlineObject, '等级', 0, m_sCharName, OnlineObject.m_Abil.Level, nOLevel,
        Format('@%s %s %d', [Cmd.sCmd, sHumanName, nLevel]));

    end
    else if HeroObject <> nil then
    begin
      nOLevel := HeroObject.m_Abil.Level;

      if g_Config.btMaxLevel = 0 then
        HeroObject.m_Abil.Level := Max(1, Min(MAXUPLEVEL, nLevel))
      else if g_Config.btMaxLevel = 1 then
        HeroObject.m_Abil.Level := Max(1, Min(High(Integer), nLevel))
      else
        HeroObject.m_Abil.Level := Max(1, Min(High(LongWord), nLevel));

      HeroObject.HasLevelUp(1);
      SysMsg(sHumanName + ' 等级调整完成。', c_Green, t_Hint);
      if g_Config.boShowMakeItemMsg then
        MainOutMessage('[等级调整] ' + m_sCharName + '(' + HeroObject.m_sCharName + ' ' + IntToStr(nOLevel) + ' -> ' +
          IntToStr(HeroObject.m_Abil.Level) + ')');

      AddGameDataLog(LOG_LevelChange, LOG_ActionNone, HeroObject, '等级', 0, m_sCharName, HeroObject.m_Abil.Level, nOLevel,
        Format('@%s %s %d', [Cmd.sCmd, sHumanName, nLevel]));
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdAdjustExp(PlayObject: TPlayObject; Human: TPlayObject; nExp: Integer);
begin

end;

procedure CmdBackStep(PlayObject: TPlayObject; sCmd: string; nType, nCount: Integer);
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;
    nType := Min(nType, 8);
    if nType = 0 then
    begin
      CharPushed(GetBackDir(m_btDirection), nCount);
    end
    else
    begin
      CharPushed(Random(nType), nCount);
    end;
  end;
end;

procedure CmdBonuPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumName: string; nCount: Integer);
var
  OnlineObject: TPlayObject;
  sMsg: string;
  OldPoint: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sHumName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 属性点数(不输入为查看点数)', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumName]), c_Red, t_Hint);
      Exit;
    end;
    if (nCount > 0) then
    begin
      OldPoint := OnlineObject.m_nBonusPoint;
      OnlineObject.m_nBonusPoint := nCount;
      OnlineObject.SendMsg(PlayObject, RM_ADJUST_BONUS, 0, 0, 0, 0, '');

      AddGameDataLog(LOG_AbilPointChange, LOG_ActionNone, PlayObject, '未分配置属性点', 0, '0', OnlineObject.m_nBonusPoint, OldPoint,
        Format('@%s %s %d', [Cmd.sCmd, sHumName, nCount]));
      Exit;
    end;
    sMsg := Format('未分配点数:%d 已分配点数:(DC:%d MC:%d SC:%d AC:%d MAC:%d HP:%d MP:%d HIT:%d SPEED:%d)',
      [OnlineObject.m_nBonusPoint, OnlineObject.m_BonusAbil.DC, OnlineObject.m_BonusAbil.MC, OnlineObject.m_BonusAbil.SC,
      OnlineObject.m_BonusAbil.AC, OnlineObject.m_BonusAbil.MAC, OnlineObject.m_BonusAbil.HP, OnlineObject.m_BonusAbil.MP,
      OnlineObject.m_BonusAbil.Hit, OnlineObject.m_BonusAbil.Speed]);
    SysMsg(Format('%s的属性点数为:%s', [sHumName, sMsg]), c_Red, t_Hint);
  end;
end;

procedure CmdChangeAdminMode(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string; boFlag: Boolean);
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;
    m_boAdminMode := boFlag;

    if m_boAdminMode then
      SysMsg(sGameMasterMode, c_Green, t_Hint)
    else
      SysMsg(sReleaseGameMasterMode, c_Green, t_Hint);
  end;
end;

procedure CmdChangeAttackMode(PlayObject: TPlayObject; nMode: Integer; sParam1, sParam2, sParam3, sParam4, sParam5, sParam6,
  sParam7: string);
var
  btAttatckMode: Byte;
  nC: Integer;
  boChange: Boolean;
  HeroObj: THeroObject;
begin
  if PlayObject.m_PEnvir.m_boNoSwitchAttackMode then
    Exit;

  with PlayObject do
  begin
    if (not m_boChangeAttatckMode) then
    begin
      btAttatckMode := m_btAttatckMode;

      if (nMode >= 0) and (nMode <= 4) then
      begin
        if g_Config.AttatckModes[nMode] then
          m_btAttatckMode := nMode;
      end
      else
      begin
        nC := 0;
        boChange := False;
        while nC < 8 do
        begin
          if m_btAttatckMode < HAM_NATION then
            Inc(m_btAttatckMode)
          else
            m_btAttatckMode := HAM_ALL;
          if g_Config.AttatckModes[m_btAttatckMode] then
          begin
            boChange := True;
            Break;
          end
          else
            Inc(nC);
        end;
        if not boChange then
          m_btAttatckMode := btAttatckMode
        else
        begin
          { TODO -ochongchong -c新增 : 英雄攻击模式随人物调整 【2013-08-15】 }
          if m_btRaceServer = RC_PLAYOBJECT then
          begin
            HeroObj := THeroObject(TPlayObject(PlayObject).m_MyHero);

            if HeroObj <> nil then
            begin
              if g_Config.boHeroForcePeaceMode then
                HeroObj.m_btAttatckMode := HAM_PEACE
              else
                HeroObj.m_btAttatckMode := PlayObject.m_btAttatckMode;

              if (HeroObj.m_TargetCret <> nil) and (not HeroObj.IsAttackTarget(HeroObj.m_TargetCret)) then
                HeroObj.DelTargetCreat;
            end;
          end;
        end;
      end;
      SendDefMessage(SM_ATTATCKMODE, m_btAttatckMode, 0, 0, 0, ''); // 攻击模式
    end;

    g_FunctionNPC.GotoLable(TPlayObject(PlayObject), '@AttatckModeChange', False);
    case m_btAttatckMode of
      HAM_ALL:
        SysMsg(sAttackModeOfAll, c_Green, t_Hint); // [攻击模式: 全体攻击]
      HAM_PEACE:
        SysMsg(sAttackModeOfPeaceful, c_Green, t_Hint); // [攻击模式: 和平攻击]
      HAM_DEAR:
        SysMsg(sAttackModeOfDear, c_Green, t_Hint); // [攻击模式: 和平攻击]
      HAM_MASTER:
        SysMsg(sAttackModeOfMaster, c_Green, t_Hint); // [攻击模式: 和平攻击]
      HAM_GROUP:
        SysMsg(sAttackModeOfGroup, c_Green, t_Hint); // [攻击模式: 编组攻击]
      HAM_GUILD:
        SysMsg(sAttackModeOfGuild, c_Green, t_Hint); // [攻击模式: 行会攻击]
      HAM_PKATTACK:
        SysMsg(sAttackModeOfRedWhite, c_Green, t_Hint); // [攻击模式: 红名攻击]
      HAM_NATION:
        SysMsg(sAttackModeOfNation, c_Green, t_Hint); // [攻击模式: 国家攻击]
    end;
  end;
end;

procedure CmdChangeDearName(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sDearName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sHumanName = '') or (sDearName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 配偶名称(如果为 无 则清除)', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      if CompareText(sDearName, '无') = 0 then
      begin
        OnlineObject.m_sDearName := '';
        OnlineObject.RefShowName;
        SysMsg(sHumanName + ' 的配偶名清除成功。', c_Green, t_Hint);
      end
      else
      begin
        OnlineObject.m_sDearName := sDearName;
        OnlineObject.RefShowName;
        SysMsg(sHumanName + ' 的配偶名更改成功。', c_Green, t_Hint);
      end;
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdChangeGender(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sSex: string);
var
  OnlineObject: TPlayObject;
  nSex: Integer;
  HeroObject: THeroObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    nSex := -1;
    if (sSex = 'Man') or (sSex = '男') or (sSex = '0') then
    begin
      nSex := 0;
    end;
    if (sSex = 'WoMan') or (sSex = '女') or (sSex = '1') then
    begin
      nSex := 1;
    end;
    if (sHumanName = '') or (nSex = -1) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 性别(男、女)', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      if OnlineObject.m_btGender <> nSex then
      begin
        OnlineObject.m_btGender := nSex;
        OnlineObject.FeatureChanged();
        SysMsg(OnlineObject.m_sCharName + ' 的性别已改变。', c_Green, t_Hint);
      end
      else
      begin
        SysMsg(OnlineObject.m_sCharName + ' 的性别未改变！', c_Red, t_Hint);
      end;
    end
    else
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject <> nil then
      begin
        if HeroObject.m_btGender <> nSex then
        begin
          HeroObject.m_btGender := nSex;
          HeroObject.FeatureChanged();
          SysMsg(HeroObject.m_sCharName + ' 的性别已改变。', c_Green, t_Hint);
        end
        else
        begin
          SysMsg(HeroObject.m_sCharName + ' 的性别未改变！', c_Red, t_Hint);
        end;
      end
      else
      begin
        SysMsg(sHumanName + '没有在线！', c_Red, t_Hint);
      end;
    end;
  end;
end;

procedure CmdChangeJob(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sJobName: string);
var
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sHumanName = '') or (sJobName = '') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandChangeJobHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      if (CompareText(sJobName, 'Warr') = 0) or (CompareText(sJobName, '战士') = 0) then
        OnlineObject.m_btJob := 0;
      if (CompareText(sJobName, 'Wizard') = 0) or (CompareText(sJobName, '法师') = 0) then
        OnlineObject.m_btJob := 1;
      if (CompareText(sJobName, 'Taos') = 0) or (CompareText(sJobName, '道士') = 0) then
        OnlineObject.m_btJob := 2;
      OnlineObject.HasLevelUp(1, False);
      OnlineObject.SysMsg(g_sGameCommandChangeJobHumanMsg, c_Green, t_Hint);
      SysMsg(Format(g_sGameCommandChangeJobMsg, [sHumanName]), c_Green, t_Hint);
    end
    else
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject <> nil then
      begin
        if (CompareText(sJobName, 'Warr') = 0) or (CompareText(sJobName, '战士') = 0) then
          HeroObject.m_btJob := 0;
        if (CompareText(sJobName, 'Wizard') = 0) or (CompareText(sJobName, '法师') = 0) then
          HeroObject.m_btJob := 1;
        if (CompareText(sJobName, 'Taos') = 0) or (CompareText(sJobName, '道士') = 0) then
          HeroObject.m_btJob := 2;
        HeroObject.HasLevelUp(1, False);
        HeroObject.SysMsg(g_sGameCommandChangeJobHumanMsg, c_Green, t_Hint);
        SysMsg(Format(g_sGameCommandChangeJobMsg, [sHumanName]), c_Green, t_Hint);
      end
      else
      begin
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      end;
    end;
  end;
end;

procedure CmdChangeLevel(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam1: string);
var
  nOLevel: Integer;
  nLevel: Int64;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '']), c_Red, t_Hint);
      Exit;
    end;
    nLevel := StrToInt64Def(sParam1, 1);
    nOLevel := m_Abil.Level;

    // 负数处理 piaoyun 2013-07-26
    nLevel := Max(1, nLevel);

    if g_Config.btMaxLevel = 0 then
      m_Abil.Level := Min(MAXUPLEVEL, nLevel)
    else if g_Config.btMaxLevel = 1 then
      m_Abil.Level := Min(High(Integer), nLevel)
    else
      m_Abil.Level := Min(High(LongWord), nLevel);

    HasLevelUp(1);
    if g_Config.boShowMakeItemMsg then
    begin
      MainOutMessage(Format(g_sGameCommandLevelConsoleMsg, [m_sCharName, nOLevel, m_Abil.Level]));
    end;

    AddGameDataLog(LOG_LevelChange, LOG_ActionNone, PlayObject, '等级', 0, '0', m_Abil.Level, nOLevel,
      Format('@%s %d', [Cmd.sCmd, nLevel]));
  end;
end;

procedure CmdChangeLoyaltyPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHeroName, sCtr, sLoyalPoint: string);
var
  oldLoyalPoint: Real;
  changeLoyalPoint: Integer;
  Ctr: Char;
  HeroObject: THeroObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sHeroName <> '') and (sHeroName[1] = '?')) or (sCtr = '') or (sLoyalPoint = '') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '英雄名称 控制符(+,-,=) 忠诚度点数(0-10000)']), c_Red, t_Hint);
      Exit;
    end;
  end;

  HeroObject := UserEngine.GetHeroObject(sHeroName);
  if HeroObject = nil then
  begin
    PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHeroName]), c_Red, t_Hint);
    Exit;
  end;

  Ctr := sCtr[1];
  changeLoyalPoint := StrToIntDef(sLoyalPoint, -1);
{$IF CompilerVersion >= 22}
  if not CharInSet(Ctr, ['=', '-', '+']) or (changeLoyalPoint < 0) or (changeLoyalPoint > 100000000) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '英雄名称 控制符(+,-,=) 忠诚度点数(0-10000)']), c_Red, t_Hint);
    Exit;
  end;
{$ELSE}
  if not(Ctr in ['=', '-', '+']) or (changeLoyalPoint < 0) or (changeLoyalPoint > 100000000) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '英雄名称 控制符(+,-,=) 忠诚度点数(0-10000)']), c_Red, t_Hint);
    Exit;
  end;
{$IFEND}
  oldLoyalPoint := HeroObject.m_rLoyalPoint;
  case Ctr of
    '=':
      HeroObject.m_rLoyalPoint := changeLoyalPoint / 100;
    '-':
      HeroObject.m_rLoyalPoint := HeroObject.m_rLoyalPoint - changeLoyalPoint / 100;
    '+':
      HeroObject.m_rLoyalPoint := HeroObject.m_rLoyalPoint + changeLoyalPoint / 100;
  end;
  if HeroObject.m_rLoyalPoint < 0 then
    HeroObject.m_rLoyalPoint := 0;
  if HeroObject.m_rLoyalPoint > 100 then
    HeroObject.m_rLoyalPoint := 100;

  if oldLoyalPoint <> HeroObject.m_rLoyalPoint then
    HeroObject.SendLoyalPoint;
end;

procedure CmdGetMachineID(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称']), c_Red, t_Hint);
      Exit;
    end;
  end;

  OnlineObject := UserEngine.GetPlayObject(sHumanName);
  if OnlineObject <> nil then
  begin
    PlayObject.SysMsg(Format(g_sUserMachineIDMsg, [sHumanName, OnlineObject.m_sMachineID]), c_Green, t_Say);
  end
  else
  begin
    PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
  end;
end;

procedure CmdChangeGameDiamond(PlayObject: TPlayObject; Cmd: pTGameCmd; sPlayerName, sCtr, sChangeValue: string);
var
  nChangeValue: Int64;
  OldChagneValue: LongWord;
  Ctr: Char;
  Player: TPlayObject;
  Int64Value: Int64;
  OldGameDiamond: LongWord;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sPlayerName <> '') and (sPlayerName[1] = '?')) or (sCtr = '') or (sChangeValue = '') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
      Exit;
    end;
  end;

  Player := UserEngine.GetPlayObject(sPlayerName);
  if Player = nil then
  begin
    PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sPlayerName]), c_Red, t_Hint);
    Exit;
  end;

  OldGameDiamond := Player.m_nGameDiamond;
  Ctr := sCtr[1];
  nChangeValue := StrToInt64Def(sChangeValue, -1);
{$IF CompilerVersion >= 22}
  if not CharInSet(Ctr, ['=', '-', '+']) or (nChangeValue < 0) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
    Exit;
  end;
{$ELSE}
  if not(Ctr in ['=', '-', '+']) or (nChangeValue < 0) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
    Exit;
  end;
{$IFEND}
  if nChangeValue > High(LongWord) then
    nChangeValue := High(LongWord);

  OldChagneValue := nChangeValue;

  case Ctr of
    '=':
      begin
        Int64Value := nChangeValue;
        nChangeValue := Int64Value - Player.m_nGameDiamond;
        Player.m_nGameDiamond := Int64Value;
      end;
    '-':
      begin
        Int64Value := Int64(Player.m_nGameDiamond) - nChangeValue;
        if Int64Value < 0 then
          Int64Value := 0;
        nChangeValue := Int64Value - Player.m_nGameDiamond;
        Player.m_nGameDiamond := Int64Value;
      end;
    '+':
      begin
        Int64Value := Int64(Player.m_nGameDiamond) + nChangeValue;
        if Int64Value > High(LongWord) then
          Int64Value := High(LongWord);
        nChangeValue := Int64Value - Player.m_nGameDiamond;
        Player.m_nGameDiamond := Max(0, Int64Value);
      end;
  end;

  AddGameDataLog(LOG_GameDiamondChange, LOG_ActionNone, Player, g_Config.sGameDiamondName, 0, PlayObject.m_sCharName,
    Player.m_nGameDiamond, OldGameDiamond, '@' + Cmd.sCmd + ' ' + sPlayerName + ' ' + Ctr + ' ' + IntToStr(OldChagneValue));

  Player.NewGamePointChanged;
  Player.SysMsg(Format(g_sGameCommandGameDiamondHumanMsg, [g_Config.sGameDiamondName, nChangeValue, Player.m_nGameDiamond]),
    c_Green, t_Hint);
  PlayObject.SysMsg(Format(g_sGameCommandGameDiamondGMMsg, [sPlayerName, g_Config.sGameDiamondName, nChangeValue,
    Player.m_nGameDiamond]), c_Green, t_Hint);
end;

procedure CmdChangeGameGird(PlayObject: TPlayObject; Cmd: pTGameCmd; sPlayerName, sCtr, sChangeValue: string);
var
  nChangeValue, Int64Value: Int64;
  OldChangeValue: LongWord;
  OldGameGird: LongWord;
  Ctr: Char;
  Player: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sPlayerName <> '') and (sPlayerName[1] = '?')) or (sCtr = '') or (sChangeValue = '') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
      Exit;
    end;
  end;

  Player := UserEngine.GetPlayObject(sPlayerName);
  if Player = nil then
  begin
    PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sPlayerName]), c_Red, t_Hint);
    Exit;
  end;

  OldGameGird := Player.m_nGameGird;
  Ctr := sCtr[1];
  nChangeValue := StrToInt64Def(sChangeValue, -1);

{$IF CompilerVersion >= 22}
  if not CharInSet(Ctr, ['=', '-', '+']) or (nChangeValue < 0) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
    Exit;
  end;
{$ELSE}
  if not(Ctr in ['=', '-', '+']) or (nChangeValue < 0) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
    Exit;
  end;
{$IFEND}
  if nChangeValue > High(LongWord) then
    nChangeValue := High(LongWord);

  OldChangeValue := nChangeValue;

  case Ctr of
    '=':
      begin
        Int64Value := nChangeValue;
        nChangeValue := Int64Value - Player.m_nGameGird;
        Player.m_nGameGird := Int64Value;
      end;
    '-':
      begin
        Int64Value := Int64(Player.m_nGameGird) - nChangeValue;
        if Int64Value < 0 then
          Int64Value := 0;
        nChangeValue := Int64Value - Player.m_nGameGird;
        Player.m_nGameGird := Int64Value;
      end;
    '+':
      begin
        Int64Value := Int64(Player.m_nGameGird) + nChangeValue;
        if Int64Value > High(LongWord) then
          Int64Value := High(LongWord);
        nChangeValue := Int64Value - Player.m_nGameGird;
        Player.m_nGameGird := Max(0, Int64Value);
      end;
  end;
  Player.NewGamePointChanged;
  Player.SysMsg(Format(g_sGameCommandGameGirdHumanMsg, [g_Config.sGameGirdName, nChangeValue, Player.m_nGameGird]),
    c_Green, t_Hint);
  PlayObject.SysMsg(Format(g_sGameCommandGameGirdGMMsg, [sPlayerName, g_Config.sGameGirdName, nChangeValue, Player.m_nGameGird]),
    c_Green, t_Hint);

  AddGameDataLog(LOG_GameGirdChange, LOG_ActionNone, Player, g_Config.sGameGirdName, 0, PlayObject.m_sCharName,
    Player.m_nGameGird, OldGameGird, '@' + Cmd.sCmd + ' ' + sPlayerName + ' ' + Ctr + ' ' + IntToStr(OldChangeValue));
end;

procedure CmdChangeGameGlory(PlayObject: TPlayObject; Cmd: pTGameCmd; sPlayerName, sCtr, sChangeValue: string);
var
  nOldValue, nChangeValue: Integer;
  Ctr: Char;
  Player: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sPlayerName <> '') and (sPlayerName[1] = '?')) or (sCtr = '') or (sChangeValue = '') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
      Exit;
    end;
  end;

  Player := UserEngine.GetPlayObject(sPlayerName);
  if Player = nil then
  begin
    PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sPlayerName]), c_Red, t_Hint);
    Exit;
  end;

  Ctr := sCtr[1];
  nChangeValue := StrToIntDef(sChangeValue, -1);

{$IF CompilerVersion >= 22}
  if not CharInSet(Ctr, ['=', '-', '+']) or (nChangeValue < 0) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
    Exit;
  end;
{$ELSE}
  if not(Ctr in ['=', '-', '+']) or (nChangeValue < 0) then
  begin
    PlayObject.SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, ' 人物名称 控制符(+,-,=) 数量']), c_Red, t_Hint);
    Exit;
  end;
{$IFEND}
  nOldValue := Player.m_nGameGlory;
  case Ctr of
    '=':
      Player.m_nGameGlory := nChangeValue;
    '-':
      Player.m_nGameGlory := Max(0, Player.m_nGameGlory - nChangeValue);
    '+':
      Player.m_nGameGlory := Max(0, Player.m_nGameGlory + nChangeValue);
  end;

  AddGameDataLog(LOG_GamegLoryChange, LOG_ActionNone, Player, '', 0, PlayObject.m_sCharName, Player.m_nGameGlory, nOldValue,
    Format('@%s %s %s %d', [Cmd.sCmd, sPlayerName, Ctr, nChangeValue]));

  Player.SendGameGlory;
end;

procedure CmdChangeNGLevel(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sChangeValue: string);
var
  SmartObject: TSmartObject;
  nOldLevel, nNewLevel: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if sHumanName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 内功等级', c_Red, t_Hint);
      Exit;
    end;
    SmartObject := UserEngine.GetPlayObject(sHumanName);
    if SmartObject = nil then
      SmartObject := UserEngine.GetHeroObject(sHumanName);

    if SmartObject <> nil then
    begin
      if not SmartObject.m_boTrainingNG then
      begin
        SysMsg(sHumanName + '未学习内功', c_Red, t_Hint);
        Exit;
      end;

      nOldLevel := SmartObject.m_AbilNG.Level;
      nNewLevel := StrToIntDef(sChangeValue, -1);
      if (sChangeValue = '') or (nNewLevel < 0) or (nNewLevel > MAXNG_LEVEL) then
      begin
        SysMsg('内功等级错误', c_Red, t_Hint);
        Exit;
      end;

      if nOldLevel <> nNewLevel then
      begin
        SmartObject.m_AbilNG.Level := nNewLevel;
        SmartObject.HasLevelUpNG(nOldLevel);
        SysMsg(sHumanName + ' 内功等级调整完成。', c_Green, t_Hint);
      end;

      AddGameDataLog(LOG_LevelChange, LOG_ActionNone, SmartObject, '内功等级', 0, PlayObject.m_sCharName, nNewLevel, nOldLevel,
        Format('@%s %s %d', [Cmd.sCmd, sHumanName, nNewLevel]));
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdDelUserShop(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  Player: TPlayObject;
  UserShop: TUserShop;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if sHumanName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
      Exit;
    end;
    Player := UserEngine.GetPlayObject(sHumanName);

    if Player <> nil then
    begin
      if g_M2DataDB.UserShopDB.GetUserShopInfo(sHumanName, UserShop) then
      begin
        g_M2DataDB.UserShopDB.ShopDelete(UserShop.ShopID);
        SysMsg('已成功删除 ' + sHumanName + ' 的个人商店', c_Green, t_Hint);
      end
      else
      begin
        SysMsg(sHumanName + '无个人商店或个人商店已被删除', c_Red, t_Hint);
        Exit;
      end;
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdSetUserShopName(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, NewShopName: string);
var
  Player: TPlayObject;
  UserShop: TUserShop;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if sHumanName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 新商店名称', c_Red, t_Hint);
      Exit;
    end;
    Player := UserEngine.GetPlayObject(sHumanName);

    if Player <> nil then
    begin
      if g_M2DataDB.UserShopDB.GetUserShopInfo(sHumanName, UserShop) then
      begin
        if g_M2DataDB.UserShopDB.ShopNameExists(NewShopName) then
        begin
          SysMsg(sHumanName + '新商店名已被占用', c_Red, t_Hint);
          Exit;
        end
        else
        begin
          if GetNameInFilterList(NewShopName) then
          begin
            // 检测英雄名称是否有非法字符
            SysMsg('商店名称包含禁止的字符' + NewShopName, c_Red, t_Hint);
            Exit;
          end;

          g_M2DataDB.UserShopDB.ShopRename(UserShop.ShopID, NewShopName);
          SysMsg('已成功更改 ' + sHumanName + ' 的个人商店名称为 ' + NewShopName, c_Green, t_Hint);
        end;
      end
      else
      begin
        SysMsg(sHumanName + '无个人商店或个人商店已被删除', c_Red, t_Hint);
        Exit;
      end;
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdDelSellPlayer(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  Player: TPlayObject;
begin
  if (PlayObject.m_btPermission < Cmd.nPermissionMin) then
  begin
    PlayObject.SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
    Exit;
  end;

  if sHumanName = '' then
  begin
    PlayObject.SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
    Exit;
  end;

  if g_SellPlayerList.DeletePlayerEx(sHumanName) then
  begin
    Player := UserEngine.GetPlayObject(sHumanName);
    if Player <> nil then
    begin
      Player.MakeGhost;
    end;

    PlayObject.SysMsg('已经成功下架出售的角色 ' + sHumanName, c_Green, t_Hint);
  end
  else
  begin
    PlayObject.SysMsg('角色出售列表中未找到 ' + sHumanName, c_Red, t_Hint);
    Exit;
  end;
end;

procedure CmdChangeMasterName(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sMasterName, sIsMaster: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if (sHumanName = '') or (sMasterName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 师徒名称(如果为 无 则清除)', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      if CompareText(sMasterName, '无') = 0 then
      begin
        OnlineObject.m_sMasterName := '';
        OnlineObject.RefShowName;
        OnlineObject.m_boMaster := False;
        SysMsg(sHumanName + ' 的师徒名清除成功。', c_Green, t_Hint);
      end
      else
      begin
        OnlineObject.m_sMasterName := sMasterName;
        if (sIsMaster <> '') and (sIsMaster[1] = '1') then
          OnlineObject.m_boMaster := True
        else
          OnlineObject.m_boMaster := False;
        OnlineObject.RefShowName;
        SysMsg(sHumanName + ' 的师徒名更改成功。', c_Green, t_Hint);
      end;
    end
    else
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdChangeObMode(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string; boFlag: Boolean);
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;
    if ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;
    if boFlag then
    begin
      SendRefMsg(RM_DISAPPEAR, 0, 0, 0, 0, '');
      // 01/21 强行发送刷新数据到客户端，解决GM登录隐身有影子问题
    end;
    m_boObMode := boFlag;
    if m_boObMode then
    begin
      SysMsg(sObserverMode, c_Green, t_Hint);
    end
    else
      SysMsg(g_sReleaseObserverMode, c_Green, t_Hint);
  end;
end;

procedure CmdChangeSabukLord(PlayObject: TPlayObject; Cmd: pTGameCmd; sCASTLENAME, sGuildName: string; boFlag: Boolean);
// 004CFE1C
var
  Guild: TGUild;
  Castle: TUserCastle;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sCASTLENAME = '') or (sGuildName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 城堡名称 行会名称', c_Red, t_Hint);
      Exit;
    end;
    Castle := g_CastleManager.Find(sCASTLENAME);
    if Castle = nil then
    begin
      SysMsg(Format(g_sGameCommandSbkGoldCastleNotFoundMsg, [sCASTLENAME]), c_Red, t_Hint);
      Exit;
    end;

    Guild := g_GuildManager.FindGuild(sGuildName);
    if Guild <> nil then
    begin
      Castle.GetCastle(Guild);

      SysMsg(Castle.m_sName + ' 所属行会已经更改为 ' + sGuildName, c_Green, t_Hint);
    end
    else
      SysMsg('行会 ' + sGuildName + '还没建立！', c_Red, t_Hint);
  end;
end;

procedure CmdChangeSalveStatus(PlayObject: TPlayObject);
var
  I: Integer;
  Hero: THeroObject;
  BaseObject: TBaseObject;
begin
  if PlayObject = nil then
    Exit;

  if (PlayObject.m_SlaveList <> nil) //
    and (PlayObject.m_SlaveList.Count > 0) //
    or ((PlayObject.m_MyGamePet <> nil) // 不能控制宠物休息 2019-05-07 11:38:26
    and (not PlayObject.m_MyGamePet.m_boDeath) //
    and (not PlayObject.m_MyGamePet.m_boGhost)) then
  begin
    PlayObject.m_boSlaveRelax := not PlayObject.m_boSlaveRelax;

    if PlayObject.m_boSlaveRelax then
    begin
      for I := PlayObject.m_SlaveList.Count - 1 downto 0 do
      begin
        BaseObject := PlayObject.m_SlaveList.Items[I];
        if (not BaseObject.m_boGhost) and (not BaseObject.m_boDeath) then
          BaseObject.DelTargetCreat;
      end;

      // 不能控制宠物休息 2019-05-07 11:38:26
      if (g_Config.boPetSleepControlBySlave) //
        and (PlayObject.m_MyGamePet <> nil) //
        and (not PlayObject.m_MyGamePet.m_boDeath) //
        and (not PlayObject.m_MyGamePet.m_boGhost) then
        PlayObject.m_MyGamePet.DelTargetCreat;

      PlayObject.SysMsg(sPetRest, c_Green, t_Hint);
    end
    else
      PlayObject.SysMsg(sPetAttack, c_Green, t_Hint)
  end;

  if { g_Config.boControlHeroBBSleep and } (PlayObject.m_MyHero <> nil) then
  begin
    Hero := THeroObject(PlayObject.m_MyHero);
    if (Hero.m_SlaveList <> nil) and (Hero.m_SlaveList.Count > 0) then
    begin
      if PlayObject.m_SlaveList.Count > 0 then
        Hero.m_boSlaveRelax := PlayObject.m_boSlaveRelax
      else
        Hero.m_boSlaveRelax := not Hero.m_boSlaveRelax;

      if Hero.m_boSlaveRelax then
      begin
        for I := Hero.m_SlaveList.Count - 1 downto 0 do
        begin
          BaseObject := Hero.m_SlaveList.Items[I];
          if (not BaseObject.m_boGhost) and (not BaseObject.m_boDeath) then
            BaseObject.DelTargetCreat;
        end;

        if PlayObject.m_SlaveList.Count = 0 then
          PlayObject.SysMsg(sPetRest, c_Green, t_Hint);
      end
      else if PlayObject.m_SlaveList.Count = 0 then
        PlayObject.SysMsg(sPetAttack, c_Green, t_Hint);
    end;
  end;
end;

procedure CmdChangeSuperManMode(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string; boFlag: Boolean);
begin
  with PlayObject do
  begin
    if m_btPermission < nPermission then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    m_boSuperMan := boFlag;
    if m_boSuperMan then
      SysMsg(sSupermanMode, c_Green, t_Hint)
    else
      SysMsg(sReleaseSupermanMode, c_Green, t_Hint);
  end;
end;

procedure CmdChangeUserFull(PlayObject: TPlayObject; sCmd, sUserCount: string);
var
  nCount: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    nCount := StrToIntDef(sUserCount, -1);
    if (sUserCount = '') or (nCount < 1) or ((sUserCount <> '') and (sUserCount[1] = '?')) then
    begin
      SysMsg('设置服务器最高上线人数。', c_Red, t_Hint);
      SysMsg('命令格式: @' + sCmd + ' 人数', c_Red, t_Hint);
      Exit;
    end;

    g_Config.nUserFull := nCount;
    SysMsg(Format('服务器上线人数限制: %d', [nCount]), c_Green, t_Hint);
  end;
end;

procedure CmdChangeZenFastStep(PlayObject: TPlayObject; sCmd, sFastStep: string);
var
  nFastStep: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    nFastStep := StrToIntDef(sFastStep, -1);
    if (sFastStep = '') or (nFastStep < 1) or ((sFastStep <> '') and (sFastStep[1] = '?')) then
    begin
      SysMsg('设置怪物行动速度。', c_Red, t_Hint);
      SysMsg('命令格式: @' + sCmd + ' 速度', c_Red, t_Hint);
      Exit;
    end;

    g_Config.nZenFastStep := nFastStep;
    SysMsg(Format('怪物行动速度: %d', [nFastStep]), c_Green, t_Hint);
  end;
end;

procedure CmdClearBagItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  I: Integer;
  OnlineObject: TBaseObject;
  UserItem: pTUserItem;
begin
  if PlayObject.m_btPermission < Cmd.nPermissionMin then
  begin
    PlayObject.SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
    Exit;
  end;

  if not Trim(sHumanName).IsEmpty then
  begin
    OnlineObject := UserEngine.GetPlayObject(sHumanName); // 玩家
    if OnlineObject = nil then
    begin
      OnlineObject := UserEngine.GetHeroObject(sHumanName); // 英雄
      if OnlineObject = nil then
      begin
        PlayObject.SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
        Exit;
      end;
    end;
  end
  else
    OnlineObject := PlayObject;

  for I := OnlineObject.m_ItemList.Count - 1 downto 0 do
  begin
    UserItem := OnlineObject.m_ItemList.Items[I];
    Dispose(UserItem);
    OnlineObject.m_ItemList.Delete(I);
  end;

  if OnlineObject.m_btRaceServer = RC_PLAYOBJECT then
    TPlayObject(OnlineObject).SendDefMessage(SM_CLEAR_BAG_ITEMS, 0, 0, 0, 0, '')
  else if OnlineObject.m_btRaceServer = RC_HEROOBJECT then
    THeroObject(OnlineObject).SendDefMessage(SM_CLEAR_BAG_ITEMS, 1, 0, 0, 0, '')
end;

procedure CmdClearHumanPassword(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
      Exit;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg('清除玩家的仓库密码！', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s 人物名称', [sCmd]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
      Exit;

    OnlineObject.m_boPasswordLocked := False;
    OnlineObject.m_boUnLockStoragePwd := False;
    OnlineObject.m_sStoragePwd := '';
    OnlineObject.SysMsg('你的保护密码已被清除！', c_Green, t_Hint);
    SysMsg(Format('%s的保护密码已被清除！', [sHumanName]), c_Green, t_Hint);
  end;
end;

procedure CmdClearMapMonster(PlayObject: TPlayObject; Cmd: pTGameCmd; sMapName, sMonName, sItems: string);
var
  I, II: Integer;
  MonList: TList;
  Envir: TEnvirnoment;
  nMonCount: Integer;
  boKillAll: Boolean;
  boKillAllMap: Boolean;
  boNotItem: Boolean;
  BaseObject: TBaseObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMapName = '') or (sMonName = '') or (sItems = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 地图号(* 为所有) 怪物名称(* 为所有) 掉物品(0,1)', c_Red, t_Hint);
      Exit;
    end;

    boKillAll := False;
    boKillAllMap := False;
    boNotItem := True;
    nMonCount := 0;
    Envir := nil;
    if sMonName = '*' then
      boKillAll := True;
    if sMapName = '*' then
      boKillAllMap := True;
    if sItems = '1' then
      boNotItem := False;

    MonList := TList.Create;
    for I := 0 to g_MapManager.Count - 1 do
    begin
      Envir := TEnvirnoment(g_MapManager.Items[I]);
      if (Envir <> nil) and (boKillAllMap or (CompareText(Envir.sMapName, sMapName) = 0)) then
      begin
        UserEngine.GetMapMonster(Envir, MonList);
        for II := 0 to MonList.Count - 1 do
        begin
          BaseObject := TBaseObject(MonList.Items[II]);
          if boKillAll or (CompareText(sMonName, BaseObject.m_sCharName) = 0) then
          begin
            BaseObject.m_boNoItem := boNotItem;
            BaseObject.m_WAbil.HP := 0;
            Inc(nMonCount);
          end;
        end;
      end;
    end;
    MonList.Free;

    if Envir = nil then
    begin
      SysMsg('输入的地图不存在！', c_Red, t_Hint);
      Exit;
    end;

    SysMsg('已清除怪物数: ' + IntToStr(nMonCount), c_Red, t_Hint);
  end;
end;

procedure CmdClearMission(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称)', c_Red, t_Hint);
      Exit;
    end;

    if sHumanName[1] = '?' then
    begin
      SysMsg('此命令用于清除人物的任务标志。', c_Blue, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format('%s不在线，或在其它服务器上！！', [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    FillChar(OnlineObject.m_QuestFlag, SizeOf(TQuestFlag), #0);
    SysMsg(Format('%s的任务标志已经全部清零。', [sHumanName]), c_Green, t_Hint);
  end;
end;

procedure CmdContestPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sGuildName: string);
var
  Guild: TGUild;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sGuildName = '') or ((sGuildName <> '') and (sGuildName[1] = '?')) then
    begin
      SysMsg('查看行会战的得分数。', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s 行会名称', [Cmd.sCmd]), c_Red, t_Hint);
      Exit;
    end;

    Guild := g_GuildManager.FindGuild(sGuildName);
    if Guild <> nil then
      SysMsg(Format('%s 的得分为: %d', [sGuildName, Guild.nContestPoint]), c_Green, t_Hint)
    else
      SysMsg(Format('行会: %s 不存在！', [sGuildName]), c_Green, t_Hint);
  end;
end;

procedure CmdStartContest(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam1: string);
var
  I, II: Integer;
  List10, List14: TList;
  OnlineObject, PlayObjectA: TPlayObject;
  bo19: Boolean;
  s20: string;
  Guild: TGUild;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg('开始行会争霸赛。', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s', [Cmd.sCmd]), c_Red, t_Hint);
      Exit;
    end;

    if not m_PEnvir.m_boFight3Zone then
    begin
      SysMsg('此命令不能在当前地图中使用！', c_Red, t_Hint);
      Exit;
    end;

    List10 := TList.Create;
    List14 := TList.Create;
    UserEngine.GetMapRageHuman(m_PEnvir, m_nCurrX, m_nCurrY, 1000, List10);
    for I := 0 to List10.Count - 1 do
    begin
      OnlineObject := TPlayObject(List10.Items[I]);
      if not OnlineObject.m_boObMode or not OnlineObject.m_boAdminMode then
      begin
        OnlineObject.m_nFightZoneDieCount := 0;
        if OnlineObject.m_MyGuild = nil then
          Continue;
        bo19 := False;
        for II := 0 to List14.Count - 1 do
        begin
          PlayObjectA := TPlayObject(List14.Items[II]);
          if OnlineObject.m_MyGuild = PlayObjectA.m_MyGuild then
            bo19 := True;
        end;

        if not bo19 then
          List14.Add(OnlineObject.m_MyGuild);
      end;
    end;

    SysMsg('行会争霸赛已经开始。', c_Green, t_Hint);
    UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor, '- 行会战争已爆发。');
    s20 := '';
    for I := 0 to List14.Count - 1 do
    begin
      Guild := TGUild(List14.Items[I]);
      Guild.StartTeamFight();
      for II := 0 to List10.Count - 1 do
      begin
        OnlineObject := TPlayObject(List10.Items[I]);
        if OnlineObject.m_MyGuild = Guild then
        begin
          Guild.AddTeamFightMember(OnlineObject.m_sCharName);
        end;
      end;
      s20 := s20 + Guild.sGuildName + ' ';
    end;

    UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor,
      ' -参加的门派:' + s20);
    List10.Free;
    List14.Free;
  end;
end;

procedure CmdEndContest(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam1: string);
var
  I, II: Integer;
  List10, List14: TList;
  OnlineObject, PlayObjectA: TPlayObject;
  bo19: Boolean;
  Guild: TGUild;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg('结束行会争霸赛。', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s', [Cmd.sCmd]), c_Red, t_Hint);
      Exit;
    end;

    if not m_PEnvir.m_boFight3Zone then
    begin
      SysMsg('此命令不能在当前地图中使用！', c_Red, t_Hint);
      Exit;
    end;

    List10 := TList.Create;
    List14 := TList.Create;
    UserEngine.GetMapRageHuman(m_PEnvir, m_nCurrX, m_nCurrY, 1000, List10);
    for I := 0 to List10.Count - 1 do
    begin
      OnlineObject := TPlayObject(List10.Items[I]);
      if not OnlineObject.m_boObMode or not OnlineObject.m_boAdminMode then
      begin
        if OnlineObject.m_MyGuild = nil then
          Continue;

        bo19 := False;
        for II := 0 to List14.Count - 1 do
        begin
          PlayObjectA := TPlayObject(List14.Items[II]);
          if OnlineObject.m_MyGuild = PlayObjectA.m_MyGuild then
            bo19 := True;
        end;

        if not bo19 then
          List14.Add(OnlineObject.m_MyGuild);
      end;
    end;

    for I := 0 to List14.Count - 1 do
    begin
      Guild := TGUild(List14.Items[I]);
      Guild.EndTeamFight();
      UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor,
        Format(' - %s 行会争霸赛已结束。', [Guild.sGuildName]));
    end;
    List10.Free;
    List14.Free;
  end;
end;

procedure CmdAllowGroupReCall(PlayObject: TPlayObject; sCmd, sParam: string);
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('此命令用于允许或禁止编组传送功能。', c_Red, t_Hint);
      Exit;
    end;

    m_boAllowGroupReCall := not m_boAllowGroupReCall;
    if m_boAllowGroupReCall then
      SysMsg(g_sEnableGroupRecall { '[允许天地合一]' } , c_Green, t_Hint)
    else
      SysMsg(g_sDisableGroupRecall { '[禁止天地合一]' } , c_Green, t_Hint);
  end;
end;

procedure CmdAnnouncement(PlayObject: TPlayObject; Cmd: pTGameCmd; sGuildName: string);
var
  I: Integer;
  Guild: TGUild;
  sHumanName: string;
  nPoint: Integer;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sGuildName = '') or ((sGuildName <> '') and (sGuildName[1] = '?')) then
    begin
      SysMsg('查看行会争霸赛结果。', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s 行会名称', [Cmd.sCmd]), c_Red, t_Hint);
      Exit;
    end;

    if not m_PEnvir.m_boFight3Zone then
    begin
      SysMsg('此命令不能在当前地图中使用！', c_Red, t_Hint);
      Exit;
    end;

    Guild := g_GuildManager.FindGuild(sGuildName);
    if Guild <> nil then
    begin
      UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor,
        Format(' - %s 行会争霸赛结果: ', [Guild.sGuildName]));
      for I := 0 to Guild.TeamFightDeadList.Count - 1 do
      begin
        nPoint := Integer(Guild.TeamFightDeadList.Objects[I]);
        sHumanName := Guild.TeamFightDeadList.Strings[I];
        UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor,
          Format(' - %s  : %d 分/死亡%d次。 ', [sHumanName, HiWord(nPoint), LoWord(nPoint)]));
      end;
    end;
    UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor,
      Format(' - [%s] : %d 分。', [Guild.sGuildName, Guild.nContestPoint]));
    UserEngine.CryCry(RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 1000, g_Config.btCryMsgFColor, g_Config.btCryMsgBColor,
      '------------------------------------');
  end;
end;

procedure CmdDearRecall(PlayObject: TPlayObject; sCmd, sParam: string);
var
  dwValue: Integer;
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('命令格式: @' + sCmd + ' (夫妻传送，将对方传送到自己身边，对方必须允许传送。)', c_Green, t_Hint);
      Exit;
    end;

    if m_sDearName = '' then
    begin
      SysMsg('你没有结婚！', c_Red, t_Hint);
      Exit;
    end;

    if m_PEnvir.m_boNODEARRECALL then
    begin
      SysMsg('本地图禁止夫妻传送！', c_Red, t_Hint);
      Exit;
    end;

    if m_DearHuman = nil then
    begin
      if m_btGender = 0 then
        SysMsg('你老婆不在线！', c_Red, t_Hint)
      else
        SysMsg('你老公不在线！', c_Red, t_Hint);
      Exit;
    end;

    { TODO -ochongchong -c新增 : 夫妻传送间隔通过参数来控制（单位:秒) }
    dwValue := (MyGetTickCount - m_dwDearRecallTick) div 1000; // 时差 秒
    if dwValue < g_Config.dwDearRecallTime then
    begin
      SysMsg(Format('%d 秒之后才可以再使用此功能！', [g_Config.dwDearRecallTime - dwValue]), c_Red, t_Hint);
      Exit;
    end;

    m_dwDearRecallTick := MyGetTickCount();
    if m_DearHuman.m_boCanDearRecall then
    begin
      if m_DearHuman.InSafeZone and g_Config.boGroupReCallNotInSafeZone then
      begin
        SysMsg(Format('%s 安全区禁止夫妻传送！', [m_DearHuman.m_sCharName]), g_Config.btGreenMsgFColor, g_Config.btGreenMsgBColor, t_Hint);
      end
      else
        RecallHuman(m_DearHuman.m_sCharName);
    end
    else
    begin
      SysMsg(m_DearHuman.m_sCharName + ' 不允许传送！', c_Red, t_Hint);
      Exit;
    end;
  end;
end;

procedure CmdMasterRecall(PlayObject: TPlayObject; sCmd, sParam: string);
var
  I, dwValue: Integer;
  MasterHuman: TPlayObject;
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('命令格式: @' + sCmd + ' (师徒传送，师父可以将徒弟传送到自己身边，徒弟必须允许传送。)', c_Green, t_Hint);
      Exit;
    end;

    if not m_boMaster then
    begin
      SysMsg('只能师父才能使用此功能！', c_Red, t_Hint);
      Exit;
    end;

    if m_MasterList.Count = 0 then
    begin
      SysMsg('你的徒弟一个都不在线！', c_Red, t_Hint);
      Exit;
    end;

    if m_PEnvir.m_boNOMASTERRECALL then
    begin
      SysMsg('本地图禁止师徒传送！', c_Red, t_Hint);
      Exit;
    end;

    { TODO -ochongchong -c新增 : 师徒传送间隔通过参数来控制（单位:秒) }
    dwValue := (MyGetTickCount - m_dwMasterRecallTick) div 1000; // 时差 秒
    if dwValue < g_Config.dwMasterRecallTime then
    begin
      SysMsg(Format('%d 秒之后才可以再使用此功能！', [g_Config.dwMasterRecallTime - dwValue]), c_Red, t_Hint);
      Exit;
    end;

    m_dwMasterRecallTick := MyGetTickCount();
    for I := 0 to m_MasterList.Count - 1 do
    begin
      MasterHuman := TPlayObject(m_MasterList.Items[I]);
      if MasterHuman.m_boCanMasterRecall then
      begin
        if MasterHuman.InSafeZone and g_Config.boGroupReCallNotInSafeZone then
        begin
          SysMsg(Format('%s 安全区禁止师徒传送！', [MasterHuman.m_sCharName]), g_Config.btGreenMsgFColor,
            g_Config.btGreenMsgBColor, t_Hint);
        end
        else
          RecallHuman(MasterHuman.m_sCharName);
      end
      else
        SysMsg(MasterHuman.m_sCharName + ' 不允许传送！', c_Red, t_Hint);
    end;
  end;
end;

procedure CmdDelBonuPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumName: string);
var
  OnlineObject: TPlayObject;
  nOldBonusPoint: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sHumName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      FillChar(OnlineObject.m_BonusAbil, SizeOf(TNakedAbility), #0);
      nOldBonusPoint := OnlineObject.m_nBonusPoint;
      OnlineObject.m_nBonusPoint := 0;
      OnlineObject.SendMsg(OnlineObject, RM_ADJUST_BONUS, 0, 0, 0, 0, '');
      OnlineObject.HasLevelUp(0, False);
      OnlineObject.SysMsg('分配点数已清除！', c_Red, t_Hint);
      SysMsg(sHumName + ' 的分配点数已清除.', c_Green, t_Hint);

      AddGameDataLog(LOG_AbilPointChange, LOG_ActionNone, OnlineObject, '清未分配置属性点', 0, '0', OnlineObject.m_nBonusPoint,
        nOldBonusPoint, '@' + Cmd.sCmd + ' ' + sHumName);
    end
    else
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumName]), c_Red, t_Hint);
  end;
end;

procedure CmdReNewLevel(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sLevel: string);
var
  OnlineObject: TPlayObject;
  nLevel: Integer;
begin
  with PlayObject do
  begin
    if m_btPermission < 6 then
      Exit;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 点数(为空则查看)', c_Red, t_Hint);
      Exit;
    end;

    nLevel := StrToIntDef(sLevel, -1);
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      if (nLevel >= 0) and (nLevel <= 255) then
      begin
        OnlineObject.m_btReLevel := nLevel;
        OnlineObject.RefShowName();
      end;
      // 修复转生等级命令错误 piaoyun 2013-07-27
      SysMsg(sHumanName + ' 的转生等级为 ' + IntToStr(OnlineObject.m_btReLevel), c_Green, t_Hint);
    end
    else
      SysMsg(sHumanName + ' 没在线上！', c_Red, t_Hint);
  end;
end;

procedure CmdRestBonuPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumName: string);
var
  OnlineObject: TPlayObject;
  nOldBonusPoint, nTotleUsePoint: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sHumName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      nTotleUsePoint := OnlineObject.m_BonusAbil.DC + OnlineObject.m_BonusAbil.MC + OnlineObject.m_BonusAbil.SC +
        OnlineObject.m_BonusAbil.AC + OnlineObject.m_BonusAbil.MAC + OnlineObject.m_BonusAbil.HP + OnlineObject.m_BonusAbil.MP +
        OnlineObject.m_BonusAbil.Hit + OnlineObject.m_BonusAbil.Speed + OnlineObject.m_BonusAbil.X2;
      FillChar(OnlineObject.m_BonusAbil, SizeOf(TNakedAbility), #0);

      nOldBonusPoint := OnlineObject.m_nBonusPoint;
      Inc(OnlineObject.m_nBonusPoint, nTotleUsePoint);
      OnlineObject.SendMsg(PlayObject, RM_ADJUST_BONUS, 0, 0, 0, 0, '');
      OnlineObject.HasLevelUp(0, False);
      OnlineObject.SysMsg('分配点数已复位！', c_Red, t_Hint);
      SysMsg(sHumName + ' 的分配点数已复位.', c_Green, t_Hint);

      AddGameDataLog(LOG_AbilPointChange, LOG_ActionNone, OnlineObject, '复位未分配置属性点', 0, '0', OnlineObject.m_nBonusPoint,
        nOldBonusPoint, '@' + Cmd.sCmd + ' ' + sHumName);
    end
    else
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumName]), c_Red, t_Hint);
  end;
end;

procedure CmdSbkDoorControl(PlayObject: TPlayObject; sCmd, sParam: string);
begin

end;

procedure CmdSearchDear(PlayObject: TPlayObject; sCmd, sParam: string);
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('此命令用于查询配偶当前所在位置。', c_Red, t_Hint);
      Exit;
    end;

    if m_sDearName = '' then
    begin
      SysMsg(g_sYouAreNotMarryedMsg { '你都没结婚查什么？' } , c_Red, t_Hint);
      Exit;
    end;

    if m_DearHuman = nil then
    begin
      if m_btGender = 0 then
        SysMsg(g_sYourWifeNotOnlineMsg { '你的老婆还没有上线！' } , c_Red, t_Hint)
      else
        SysMsg(g_sYourHusbandNotOnlineMsg { '你的老公还没有上线！' } , c_Red, t_Hint);
      Exit;
    end;

    if m_btGender = 0 then
    begin
      SysMsg(g_sYourWifeNowLocateMsg { '你的老婆现在位于:' } , c_Green, t_Hint);
      SysMsg(m_DearHuman.m_sCharName + ' ' + m_DearHuman.m_PEnvir.sMapDesc + '(' + IntToStr(m_DearHuman.m_nCurrX) + ':' +
        IntToStr(m_DearHuman.m_nCurrY) + ')', c_Green, t_Hint);
      m_DearHuman.SysMsg(g_sYourHusbandSearchLocateMsg { '你的老公正在找你，他现在位于:' } , c_Green, t_Hint);
      m_DearHuman.SysMsg(m_sCharName + ' ' + m_PEnvir.sMapDesc + '(' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) + ')',
        c_Green, t_Hint);
    end
    else
    begin
      SysMsg(g_sYourHusbandNowLocateMsg { '你的老公现在位于:' } , c_Red, t_Hint);
      SysMsg(m_DearHuman.m_sCharName + ' ' + m_DearHuman.m_PEnvir.sMapDesc + '(' + IntToStr(m_DearHuman.m_nCurrX) + ':' +
        IntToStr(m_DearHuman.m_nCurrY) + ')', c_Green, t_Hint);
      m_DearHuman.SysMsg(g_sYourWifeSearchLocateMsg { '你的老婆正在找你，她现在位于:' } , c_Green, t_Hint);
      m_DearHuman.SysMsg(m_sCharName + ' ' + m_PEnvir.sMapDesc + '(' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) + ')',
        c_Green, t_Hint);
    end;
  end;
end;

procedure CmdSearchMaster(PlayObject: TPlayObject; sCmd, sParam: string);
var
  I: Integer;
  Human: TPlayObject;
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('此命令用于查询师徒当前所在位置。', c_Red, t_Hint);
      Exit;
    end;

    if m_sMasterName = '' then
    begin
      SysMsg(g_sYouAreNotMasterMsg, c_Red, t_Hint);
      Exit;
    end;

    if m_boMaster then
    begin
      if m_MasterList.Count <= 0 then
      begin
        SysMsg(g_sYourMasterListNotOnlineMsg, c_Red, t_Hint);
        Exit;
      end;

      SysMsg(g_sYourMasterListNowLocateMsg, c_Green, t_Hint);
      for I := 0 to m_MasterList.Count - 1 do
      begin
        Human := TPlayObject(m_MasterList.Items[I]);
        SysMsg(Human.m_sCharName + ' ' + Human.m_PEnvir.sMapDesc + '(' + IntToStr(Human.m_nCurrX) + ':' + IntToStr(Human.m_nCurrY)
          + ')', c_Green, t_Hint);
        Human.SysMsg(g_sYourMasterSearchLocateMsg, c_Green, t_Hint);
        Human.SysMsg(m_sCharName + ' ' + m_PEnvir.sMapDesc + '(' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) + ')',
          c_Green, t_Hint);
      end;
    end
    else
    begin
      if m_MasterHuman = nil then
      begin
        SysMsg(g_sYourMasterNotOnlineMsg, c_Red, t_Hint);
        Exit;
      end;

      SysMsg(g_sYourMasterNowLocateMsg, c_Red, t_Hint);
      SysMsg(m_MasterHuman.m_sCharName + ' ' + m_MasterHuman.m_PEnvir.sMapDesc + '(' + IntToStr(m_MasterHuman.m_nCurrX) + ':' +
        IntToStr(m_MasterHuman.m_nCurrY) + ')', c_Green, t_Hint);
      m_MasterHuman.SysMsg(g_sYourMasterListSearchLocateMsg, c_Green, t_Hint);
      m_MasterHuman.SysMsg(m_sCharName + ' ' + m_PEnvir.sMapDesc + '(' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) + ')',
        c_Green, t_Hint);
    end;
  end;
end;

procedure CmdSetPermission(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sPermission: string);
var
  nPerission: Integer;
  OnlineObject: TPlayObject;
resourcestring
  sOutFormatMsg = '[权限调整-GM命令] %s (%s %d -> %d)';
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    nPerission := StrToIntDef(sPermission, 0);
    if (sHumanName = '') or not(nPerission in [0 .. 10]) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 权限等级(0 - 10)', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    if g_Config.boPermissionChangeLog then
      MainOutMessage(Format(sOutFormatMsg, [m_sCharName, OnlineObject.m_sCharName, OnlineObject.m_btPermission, nPerission]));

    OnlineObject.m_btPermission := nPerission;
    SysMsg(sHumanName + ' 当前权限为: ' + IntToStr(OnlineObject.m_btPermission), c_Red, t_Hint);
  end;
end;

procedure CmdShowHumanFlag(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sHumanName, sFlag: string);
var
  OnlineObject: TPlayObject;
  nFlag: Integer;
begin
  with PlayObject do
  begin
    if m_btPermission < nPermission then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, g_sGameCommandShowHumanFlagHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    nFlag := StrToIntDef(sFlag, 0);
    if OnlineObject.GetQuestFlagStatus(nFlag) = 1 then
      SysMsg(Format(g_sGameCommandShowHumanFlagONMsg, [OnlineObject.m_sCharName, nFlag]), c_Green, t_Hint)
    else
      SysMsg(Format(g_sGameCommandShowHumanFlagOFFMsg, [OnlineObject.m_sCharName, nFlag]), c_Green, t_Hint);
  end;
end;

procedure CmdShowHumanUnit(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sHumanName, sUnit: string);
begin

end;

procedure CmdShowHumanUnitOpen(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sHumanName, sUnit: string);
begin

end;

procedure CmdShowMapInfo(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam1: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    SysMsg(Format(g_sGameCommandMapInfoMsg, [m_PEnvir.sMapName, m_PEnvir.sMapDesc]), c_Green, t_Hint);
    SysMsg(Format(g_sGameCommandMapInfoSizeMsg, [m_PEnvir.m_nWidth, m_PEnvir.m_nHeight]), c_Green, t_Hint);
  end;
end;

procedure CmdShowMapMode(PlayObject: TPlayObject; sCmd, sMapName: string);
var
  Envir: TEnvirnoment;
  sMsg: string;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if (sMapName = '') then
    begin
      SysMsg('命令格式: @' + sCmd + ' 地图号', c_Red, t_Hint);
      Exit;
    end;

    Envir := g_MapManager.FindMap(sMapName);
    if (Envir = nil) then
    begin
      SysMsg(sMapName + ' 不存在！', c_Red, t_Hint);
      Exit;
    end;

    sMsg := '地图模式: ' + Envir.GetEnvirInfo;
    SysMsg(sMsg, c_Blue, t_Hint);
  end;
end;

procedure CmdSetMapMode(PlayObject: TPlayObject; sCmd, sMapName, sMapMode, sParam1, sParam2: string);
var
  Envir: TEnvirnoment;
  sMsg: string;
begin
  if PlayObject.m_btPermission < 6 then
    Exit;

  if (sMapName = '') or (sMapMode = '') then
  begin
    PlayObject.SysMsg('命令格式: @' + sCmd + ' 地图号 模式', c_Red, t_Hint);
    Exit;
  end;

  Envir := g_MapManager.FindMap(sMapName);
  if Envir = nil then
  begin
    PlayObject.SysMsg(sMapName + ' 不存在！', c_Red, t_Hint);
    Exit;
  end;

  if CompareText(sMapMode, 'SAFE') = 0 then
    Envir.m_boSAFE := sParam1 <> ''
  else if CompareText(sMapMode, 'DARK') = 0 then
    Envir.m_boDARK := sParam1 <> ''
  else if CompareText(sMapMode, 'DARK') = 0 then
    Envir.m_boDARK := sParam1 <> ''
  else if CompareText(sMapMode, 'FIGHT') = 0 then
    Envir.m_boFightZone := sParam1 <> ''
  else if CompareText(sMapMode, 'FIGHT2') = 0 then
    Envir.m_boFight2Zone := sParam1 <> ''
  else if CompareText(sMapMode, 'FIGHT3') = 0 then
    Envir.m_boFight3Zone := sParam1 <> ''
  else if CompareText(sMapMode, 'FIGHT4') = 0 then
    Envir.m_boFight4Zone := sParam1 <> ''
  else if CompareText(sMapMode, 'DAY') = 0 then
    Envir.m_boDAY := sParam1 <> ''
  else if CompareText(sMapMode, 'QUIZ') = 0 then
    Envir.m_boQUIZ := sParam1 <> ''
  else if CompareText(sMapMode, 'NORECONNECT') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boNORECONNECT := True;
      Envir.sNoReconnectMap := sParam1;
    end
    else
      Envir.m_boNORECONNECT := False;
  end
  else if CompareText(sMapMode, 'MUSIC') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boMUSIC := True;
      Envir.m_sMusicFileName := sParam1;
    end
    else
      Envir.m_boMUSIC := False;
  end
  else if CompareText(sMapMode, 'EXPRATE') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boEXPRATE := True;
      Envir.m_nEXPRATE := StrToIntDef(sParam1, -1);
    end
    else
      Envir.m_boEXPRATE := False;
  end
  else if CompareText(sMapMode, 'PKWINLEVEL') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boPKWINLEVEL := True;
      Envir.m_nPKWINLEVEL := StrToIntDef(sParam1, -1);
    end
    else
      Envir.m_boPKWINLEVEL := False;
  end
  else if CompareText(sMapMode, 'PKWINEXP') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boPKWINEXP := True;
      Envir.m_nPKWINEXP := StrToIntDef(sParam1, -1);
    end
    else
      Envir.m_boPKWINEXP := False;
  end
  else if CompareText(sMapMode, 'PKLOSTLEVEL') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boPKLOSTLEVEL := True;
      Envir.m_nPKLOSTLEVEL := StrToIntDef(sParam1, -1);
    end
    else
      Envir.m_boPKLOSTLEVEL := False;
  end
  else if CompareText(sMapMode, 'PKLOSTEXP') = 0 then
  begin
    if (sParam1 <> '') then
    begin
      Envir.m_boPKLOSTEXP := True;
      Envir.m_nPKLOSTEXP := StrToIntDef(sParam1, -1);
    end
    else
      Envir.m_boPKLOSTEXP := False;
  end
  else if CompareText(sMapMode, 'DECHP') = 0 then
  begin
    if (sParam1 <> '') and (sParam2 <> '') then
    begin
      Envir.m_boDECHP := True;
      Envir.m_nDECHPTIME := StrToIntDef(sParam1, -1);
      Envir.m_nDECHPPOINT := StrToIntDef(sParam2, -1);
    end
    else
      Envir.m_boDECHP := False;
  end
  else if CompareText(sMapMode, 'DECGAMEGOLD') = 0 then
  begin
    if (sParam1 <> '') and (sParam2 <> '') then
    begin
      Envir.m_boDecGameGold := True;
      Envir.m_nDECGAMEGOLDTIME := StrToIntDef(sParam1, -1);
      Envir.m_nDecGameGold := StrToIntDef(sParam2, -1);
    end
    else
      Envir.m_boDecGameGold := False;
  end
  else if CompareText(sMapMode, 'INCGAMEGOLD') = 0 then
  begin
    if (sParam1 <> '') and (sParam2 <> '') then
    begin
      Envir.m_boIncGameGold := True;
      Envir.m_nINCGAMEGOLDTIME := StrToIntDef(sParam1, -1);
      Envir.m_nIncGameGold := StrToIntDef(sParam2, -1);
    end
    else
      Envir.m_boIncGameGold := False;
  end
  else if CompareText(sMapMode, 'INCGAMEPOINT') = 0 then
  begin
    if (sParam1 <> '') and (sParam2 <> '') then
    begin
      Envir.m_boINCGAMEPOINT := True;
      Envir.m_nINCGAMEPOINTTIME := StrToIntDef(sParam1, -1);
      Envir.m_nINCGAMEPOINT := StrToIntDef(sParam2, -1);
    end
    else
      Envir.m_boIncGameGold := False;
  end
  else if CompareText(sMapMode, 'RUNHUMAN') = 0 then
    Envir.m_boRUNHUMAN := sParam1 <> ''
  else if CompareText(sMapMode, 'RUNMON') = 0 then
    Envir.m_boRUNMON := sParam1 <> ''
  else if CompareText(sMapMode, 'NEEDHOLE') = 0 then
    Envir.m_boNEEDHOLE := sParam1 <> ''
  else if CompareText(sMapMode, 'NORECALL') = 0 then
    Envir.m_boNORECALL := sParam1 <> ''
  else if CompareText(sMapMode, 'NOGUILDRECALL') = 0 then
    Envir.m_boNOGUILDRECALL := sParam1 <> ''
  else if CompareText(sMapMode, 'NODEARRECALL') = 0 then
    Envir.m_boNODEARRECALL := sParam1 <> ''
  else if CompareText(sMapMode, 'NOMASTERRECALL') = 0 then
    Envir.m_boNOMASTERRECALL := sParam1 <> ''
  else if CompareText(sMapMode, 'NORANDOMMOVE') = 0 then
    Envir.m_boNORANDOMMOVE := sParam1 <> ''
  else if CompareText(sMapMode, 'NODRUG') = 0 then
    Envir.m_boNODRUG := sParam1 <> ''
  else if CompareText(sMapMode, 'MINE') = 0 then
    Envir.m_boMINE := sParam1 <> ''
  else if CompareText(sMapMode, 'NOPOSITIONMOVE') = 0 then
    Envir.m_boNOPOSITIONMOVE := sParam1 <> '';

  sMsg := '地图模式: ' + Envir.GetEnvirInfo;
  PlayObject.SysMsg(sMsg, c_Blue, t_Hint);
end;

procedure CmdDeleteItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sItemName: string; nCount: Integer;
  AddOnCheck: TAddOnCheck);
var
  I: Integer;
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
  nItemCount: Integer;
  StdItem: pTStdItem;
  UserItem: pTUserItem;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or (sItemName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 物品名称 数量)', c_Red, t_Hint);
      Exit;
    end;

    HeroObject := nil;
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject = nil then
      begin
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
        Exit;
      end;
    end;

    nItemCount := 0;
    if OnlineObject <> nil then
    begin
      for I := OnlineObject.m_ItemList.Count - 1 downto 0 do
      begin
        if OnlineObject.m_ItemList.Count <= 0 then
          Break;

        UserItem := OnlineObject.m_ItemList.Items[I];
        if UserItem = nil then
          Continue;

        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and SameText(sItemName, StdItem.Name) and (not Assigned(AddOnCheck) or AddOnCheck(UserItem)) then
        begin
          OnlineObject.SendDelItem(UserItem);
          OnlineObject.m_ItemList.Delete(I);
          Dispose(UserItem);
          Inc(nItemCount);

          if nItemCount >= nCount then
            Break;
        end;
      end;
    end
    else if HeroObject <> nil then
    begin
      for I := HeroObject.m_ItemList.Count - 1 downto 0 do
      begin
        if HeroObject.m_ItemList.Count <= 0 then
          Break;

        UserItem := HeroObject.m_ItemList.Items[I];
        if UserItem = nil then
          Continue;

        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and SameText(sItemName, StdItem.Name) and (not Assigned(AddOnCheck) or AddOnCheck(UserItem)) then
        begin
          HeroObject.SendDelItem(UserItem);
          HeroObject.m_ItemList.Delete(I);
          Dispose(UserItem);
          Inc(nItemCount);
          if nItemCount >= nCount then
            Break;
        end;
      end;
    end;
  end;
end;

procedure CmdDelGold(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumName: string; nCount: Integer);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumName = '') or (nCount <= 0) then
      Exit;

    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      if OnlineObject.m_nGold > nCount then
      begin
        Dec(OnlineObject.m_nGold, nCount);
      end
      else
      begin
        nCount := OnlineObject.m_nGold;
        OnlineObject.m_nGold := 0;
      end;

      OnlineObject.GoldChanged();
      SysMsg(sHumName + '的金币已减少' + IntToStr(nCount) + '.', c_Green, t_Hint);

      if g_boGameLogGold then
        AddGameDataLog(LOG_GoldChange, LOG_ActionNone, OnlineObject, sSTRING_GOLDNAME, 0, m_sCharName, OnlineObject.m_nGold,
          -nCount, '@' + Cmd.sCmd + ' ' + sHumName + ' ' + IntToStr(nCount));
    end
    else
    begin
      DataEngine.HumanChangeGold(nil, nil, cgtGold, m_sCharName, sHumName, -nCount);
      SysMsg(sHumName + '现在不在线，等其上线时金币将自动减少', c_Green, t_Hint);
    end;
  end;
end;

procedure CmdDelGuild(PlayObject: TPlayObject; Cmd: pTGameCmd; sGuildName: string);
var
  IsFound: Boolean;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if nServerIndex <> 0 then
    begin
      SysMsg('只能在主服务器上才可以使用此命令删除行会！', c_Red, t_Hint);
      Exit;
    end;

    if sGuildName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 行会名称', c_Red, t_Hint);
      Exit;
    end;

    if g_GuildManager.DELGUILD(sGuildName, IsFound) then
    begin
      if (g_FunctionNPC <> nil) then
      begin
        PlayObject.m_nScriptGotoCount := 0;
        g_FunctionNPC.GotoLable(PlayObject, '@ExitGuild', False);
      end;
    end
    else
    begin
      if IsFound then
        SysMsg('删除行会' + sGuildName + '失败！行会成员数量 > 1', c_Red, t_Hint)
      else
        SysMsg('没找到' + sGuildName + '这个行会！', c_Red, t_Hint);
    end;
  end;
end;

procedure CmdDelNpc(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string);
var
  BaseObject: TBaseObject;
  I: Integer;
  List: TList;
resourcestring
  sDelOK = '删除NPC成功...';
begin
  with PlayObject do
  begin
    if m_btPermission < nPermission then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    List := TList.Create;
    try
      BaseObject := GetPoseCreate();
      if BaseObject <> nil then
      begin
        UserEngine.m_MerchantList.LockR(14);
        try
          for I := 0 to UserEngine.m_MerchantList.Count - 1 do
            List.Add(UserEngine.m_MerchantList.Items[I]);
        finally
          UserEngine.m_MerchantList.UnLockR;
        end;

        for I := 0 to List.Count - 1 do
        begin
          if TBaseObject(List.Items[I]) = BaseObject then
          begin
            BaseObject.MakeGhost();
            if not TMerchant(BaseObject).m_boIsHide then
              BaseObject.SendRefMsg(RM_DISAPPEAR, 0, 0, 0, 0, '');

            SysMsg(sDelOK, c_Red, t_Hint);
            Exit;
          end;
        end;

        List.Clear;
        UserEngine.QuestNPCList.LockR(7);
        try
          for I := 0 to UserEngine.QuestNPCList.Count - 1 do
          begin
            List.Add(UserEngine.QuestNPCList.Objects[I]);
          end;
        finally
          UserEngine.QuestNPCList.UnLockR;
        end;

        for I := 0 to List.Count - 1 do
        begin
          if TBaseObject(List.Items[I]) = BaseObject then
          begin
            BaseObject.MakeGhost();
            // BaseObject.m_boGhost := True;
            // BaseObject.m_dwGhostTick := MyGetTickCount();
            if not TNormNpc(BaseObject).m_boIsHide then
              BaseObject.SendRefMsg(RM_DISAPPEAR, 0, 0, 0, 0, '');
            SysMsg(sDelOK, c_Red, t_Hint);
            Exit;
          end;
        end;
      end;

      SysMsg(g_sGameCommandDelNpcMsg, c_Red, t_Hint);
    finally
      List.Free;
    end;
  end;
end;

procedure CmdDelSkill(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sSkillName: string);
var
  I: Integer;
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
  boDelAll: Boolean;
  UserMagic: pTUserMagic;
  IsDeleted: Boolean;
  btOldHitPoint: Byte;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or (sSkillName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 技能名称)', c_Red, t_Hint);
      Exit;
    end;

    if CompareText(sSkillName, 'All') = 0 then
      boDelAll := True
    else
      boDelAll := False;

    HeroObject := nil;
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject = nil then
      begin
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
        Exit;
      end;
    end;

    if OnlineObject <> nil then
    begin
      IsDeleted := False;

      for I := OnlineObject.m_MagicList.Count - 1 downto 0 do
      begin
        if OnlineObject.m_MagicList.Count <= 0 then
          Break;

        UserMagic := OnlineObject.m_MagicList.Items[I];
        if UserMagic <> nil then
        begin
          if boDelAll then
          begin
            OnlineObject.m_MagicList.Delete(I);
            OnlineObject.SendDelMagic(UserMagic);

            Dispose(UserMagic);
            IsDeleted := True;
          end
          else
          begin
            if CompareText(UserMagic.MagicInfo.sMagicName, sSkillName) = 0 then
            begin
              OnlineObject.SendDelMagic(UserMagic);
              OnlineObject.m_MagicList.Delete(I);

              Dispose(UserMagic);
              OnlineObject.SysMsg(Format('技能%s已删除。', [sSkillName]), c_Green, t_Hint);
              SysMsg(Format('%s的技能%s已删除。', [sHumanName, sSkillName]), c_Green, t_Hint);
              IsDeleted := True;
              Break;
            end;
          end;
        end;
      end;

      if IsDeleted then
      begin
        btOldHitPoint := OnlineObject.m_btHitPoint;
        OnlineObject.RecalcAbilitys;
        if btOldHitPoint <> OnlineObject.m_btHitPoint then
          OnlineObject.SendMsg(OnlineObject, RM_SUBABILITY, 0, 0, 0, 0, '');
      end;
    end
    else if HeroObject <> nil then
    begin
      IsDeleted := False;
      for I := HeroObject.m_MagicList.Count - 1 downto 0 do
      begin
        if HeroObject.m_MagicList.Count <= 0 then
          Break;

        UserMagic := HeroObject.m_MagicList.Items[I];
        if UserMagic <> nil then
        begin
          if boDelAll then
          begin
            HeroObject.m_MagicList.Delete(I);
            HeroObject.SendDelMagic(UserMagic);

            Dispose(UserMagic);
            IsDeleted := True;
          end
          else
          begin
            if CompareText(UserMagic.MagicInfo.sMagicName, sSkillName) = 0 then
            begin
              HeroObject.SendDelMagic(UserMagic);
              HeroObject.m_MagicList.Delete(I);

              Dispose(UserMagic);
              HeroObject.SysMsg(Format('技能%s已删除。', [sSkillName]), c_Green, t_Hint);
              SysMsg(Format('%s的技能%s已删除。', [sHumanName, sSkillName]), c_Green, t_Hint);
              IsDeleted := True;
              Break;
            end;
          end;
        end;
      end;

      if IsDeleted then
      begin
        btOldHitPoint := HeroObject.m_btHitPoint;
        HeroObject.RecalcAbilitys;
        if btOldHitPoint <> HeroObject.m_btHitPoint then
          HeroObject.SendMsg(HeroObject, RM_SUBABILITY, 0, 0, 0, 0, '');
      end;
    end;
  end;
end;

procedure CmdDenyAccountLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sAccount, sFixDeny: string);
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sAccount = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 登录帐号 是否永久封(0,1)', c_Red, t_Hint);
      Exit;
    end;

    g_DenyAccountList.Lock;
    try
      if (sFixDeny <> '') and (sFixDeny[1] = '1') then
      begin
        g_DenyAccountList.AddObject(sAccount, TObject(1));
        SaveDenyAccountList();
        SysMsg(sAccount + '已加入禁止登录帐号列表', c_Green, t_Hint);
      end
      else
      begin
        g_DenyAccountList.AddObject(sAccount, TObject(0));
        SysMsg(sAccount + '已加入临时禁止登录帐号列表', c_Green, t_Hint);
      end;
    finally
      g_DenyAccountList.UnLock;
    end;
  end;
end;

procedure CmdDenyCharNameLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sCharName, sFixDeny: string);
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sCharName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 是否永久封(0,1)', c_Red, t_Hint);
      Exit;
    end;

    g_DenyChrNameList.Lock;
    try
      if (sFixDeny <> '') and (sFixDeny[1] = '1') then
      begin
        g_DenyChrNameList.AddObject(sCharName, TObject(1));
        SaveDenyChrNameList();
        SysMsg(sCharName + '已加入禁止人物列表', c_Green, t_Hint);
      end
      else
      begin
        g_DenyChrNameList.AddObject(sCharName, TObject(0));
        SysMsg(sCharName + '已加入临时禁止人物列表', c_Green, t_Hint);
      end;
    finally
      g_DenyChrNameList.UnLock;
    end;
  end;
end;

procedure CmdDenyIPaddrLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sIPaddr, sFixDeny: string);
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sIPaddr = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' IP地址 是否永久封(0,1)', c_Red, t_Hint);
      Exit;
    end;

    g_DenyIPAddrList.Lock;
    try
      if (sFixDeny <> '') and (sFixDeny[1] = '1') then
      begin
        g_DenyIPAddrList.AddObject(sIPaddr, TObject(1));
        SaveDenyIPAddrList();
        SysMsg(sIPaddr + '已加入禁止登录IP列表', c_Green, t_Hint);
      end
      else
      begin
        g_DenyIPAddrList.AddObject(sIPaddr, TObject(0));
        SysMsg(sIPaddr + '已加入临时禁止登录IP列表', c_Green, t_Hint);
      end;
    finally
      g_DenyIPAddrList.UnLock;
    end;
  end;
end;

procedure CmdDenyMachineIDLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sMID, sFixDeny: string);
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sMID = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 机器码 是否永久封(0,1)', c_Red, t_Hint);
      Exit;
    end;

    g_DenyMachineIDList.Lock;
    try
      if (sFixDeny <> '') and (sFixDeny[1] = '1') then
      begin
        g_DenyMachineIDList.AddObject(sMID, TObject(1));
        SaveDenyMachineIDList();
        SysMsg(sMID + '已加入禁止登录机器码列表', c_Green, t_Hint);
      end
      else
      begin
        g_DenyMachineIDList.AddObject(sMID, TObject(0));
        SysMsg(sMID + '已加入临时禁止登录机器码列表', c_Green, t_Hint);
      end;
    finally
      g_DenyMachineIDList.UnLock;
    end;
  end;
end;

procedure CmdDelMachineIDLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sMID, sFixDeny: string);
var
  I: Integer;
  boDelete: Boolean;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sMID = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 机器码', c_Red, t_Hint);
      Exit;
    end;

    boDelete := False;
    g_DenyMachineIDList.Lock;
    try
      for I := g_DenyMachineIDList.Count - 1 downto 0 do
      begin
        if g_DenyMachineIDList.Count <= 0 then
          Break;

        if CompareText(sMID, g_DenyMachineIDList.Strings[I]) = 0 then
        begin
          g_DenyMachineIDList.Delete(I);
          SaveDenyMachineIDList;
          SysMsg(sMID + '已从禁止登录机器码列表中删除。', c_Green, t_Hint);
          boDelete := True;
          Break;
        end;
      end;
    finally
      g_DenyMachineIDList.UnLock;
    end;

    if not boDelete then
      SysMsg(sMID + '没有被禁止登录。', c_Green, t_Hint);
  end;
end;

procedure CmdShowMachineIDLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sMID, sFixDeny: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    g_DenyMachineIDList.Lock;
    try
      if g_DenyMachineIDList.Count <= 0 then
      begin
        SysMsg('禁止登录机器码列表为空。', c_Green, t_Hint);
        Exit;
      end;

      for I := 0 to g_DenyMachineIDList.Count - 1 do
        SysMsg(g_DenyMachineIDList.Strings[I], c_Green, t_Hint);
    finally
      g_DenyMachineIDList.UnLock;
    end;
  end;
end;

procedure CmdDenyIPLocalLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sIPaddr, sFixDeny: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sIPaddr = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' IP所在地 是否永久封(0,1)', c_Red, t_Hint);
      Exit;
    end;

    g_DenyIPLocalList.Lock;
    try
      if (sFixDeny <> '') and (sFixDeny[1] = '1') then
      begin
        g_DenyIPLocalList.AddObject(sIPaddr, TObject(1));
        SaveDenyIPLocalList();
        SysMsg(sIPaddr + '已加入禁止登录IP所在地列表', c_Green, t_Hint);
      end
      else
      begin
        g_DenyIPLocalList.AddObject(sIPaddr, TObject(0));
        SysMsg(sIPaddr + '已加入临时禁止登录IP所在地列表', c_Green, t_Hint);
      end;
    finally
      g_DenyIPLocalList.UnLock;
    end;
  end;
end;

procedure CmdDisableFilter(PlayObject: TPlayObject; sCmd, sParam1: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg('启用/禁止文字过滤功能。', c_Red, t_Hint);
      Exit;
    end;

    boFilterWord := not boFilterWord;
    if boFilterWord then
      SysMsg('已启用文字过滤。', c_Green, t_Hint)
    else
      SysMsg('已禁止文字过滤。', c_Green, t_Hint);
  end;
end;

procedure CmdDelDenyAccountLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sAccount, sFixDeny: string);
var
  I: Integer;
  boDelete: Boolean;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sAccount = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 登录帐号', c_Red, t_Hint);
      Exit;
    end;

    boDelete := False;
    g_DenyAccountList.Lock;
    try
      for I := 0 to g_DenyAccountList.Count - 1 do
      begin
        if CompareText(sAccount, g_DenyAccountList.Strings[I]) = 0 then
        begin
          if Integer(g_DenyAccountList.Objects[I]) <> 0 then
            SaveDenyAccountList;

          g_DenyAccountList.Delete(I);
          SysMsg(sAccount + '已从禁止登录帐号列表中删除。', c_Green, t_Hint);
          boDelete := True;
          Break;
        end;
      end;
    finally
      g_DenyAccountList.UnLock;
    end;

    if not boDelete then
      SysMsg(sAccount + '没有被禁止登录。', c_Green, t_Hint);
  end;
end;

procedure CmdDelDenyCharNameLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sCharName, sFixDeny: string);
var
  I: Integer;
  boDelete: Boolean;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sCharName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
      Exit;
    end;

    boDelete := False;
    g_DenyChrNameList.Lock;
    try
      for I := 0 to g_DenyChrNameList.Count - 1 do
      begin
        if CompareText(sCharName, g_DenyChrNameList.Strings[I]) = 0 then
        begin
          if Integer(g_DenyChrNameList.Objects[I]) <> 0 then
            SaveDenyChrNameList;

          g_DenyChrNameList.Delete(I);
          SysMsg(sCharName + '已从禁止登录人物列表中删除。', c_Green, t_Hint);
          boDelete := True;
          Break;
        end;
      end;
    finally
      g_DenyChrNameList.UnLock;
    end;
    if not boDelete then
      SysMsg(sCharName + '没有被禁止登录。', c_Green, t_Hint);
  end;
end;

procedure CmdDelDenyIPaddrLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sIPaddr, sFixDeny: string);
var
  I: Integer;
  boDelete: Boolean;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sIPaddr = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' IP地址', c_Red, t_Hint);
      Exit;
    end;

    boDelete := False;
    g_DenyIPAddrList.Lock;
    try
      for I := g_DenyIPAddrList.Count - 1 downto 0 do
      begin
        if g_DenyIPAddrList.Count <= 0 then
          Break;

        if CompareText(sIPaddr, g_DenyIPAddrList.Strings[I]) = 0 then
        begin
          g_DenyIPAddrList.Delete(I);
          SaveDenyIPAddrList;
          SysMsg(sIPaddr + '已从禁止登录IP列表中删除。', c_Green, t_Hint);
          boDelete := True;
          Break;
        end;
      end;
    finally
      g_DenyIPAddrList.UnLock;
    end;

    if not boDelete then
      SysMsg(sIPaddr + '没有被禁止登录。', c_Green, t_Hint);
  end;
end;

procedure CmdDelDenyIPLocalLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sIPaddr, sFixDeny: string);
var
  I: Integer;
  boDelete: Boolean;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sIPaddr = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' IP所在地', c_Red, t_Hint);
      Exit;
    end;

    boDelete := False;
    g_DenyIPAddrList.Lock;
    try
      for I := g_DenyIPLocalList.Count - 1 downto 0 do
      begin
        if g_DenyIPLocalList.Count <= 0 then
          Break;

        if CompareText(sIPaddr, g_DenyIPLocalList.Strings[I]) = 0 then
        begin
          if Integer(g_DenyIPLocalList.Objects[I]) <> 0 then
            SaveDenyIPLocalList;
          g_DenyIPLocalList.Delete(I);
          SysMsg(sIPaddr + '已从禁止登录IP所在地列表中删除。', c_Green, t_Hint);
          boDelete := True;
          Break;
        end;
      end;
    finally
      g_DenyIPLocalList.UnLock;
    end;

    if not boDelete then
      SysMsg(sIPaddr + '没有被禁止登录。', c_Green, t_Hint);
  end;
end;

procedure CmdShowDenyAccountLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sAccount, sFixDeny: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    g_DenyAccountList.Lock;
    try
      if g_DenyAccountList.Count <= 0 then
      begin
        SysMsg('禁止登录帐号列表为空。', c_Green, t_Hint);
        Exit;
      end;

      for I := 0 to g_DenyAccountList.Count - 1 do
        SysMsg(g_DenyAccountList.Strings[I], c_Green, t_Hint);
    finally
      g_DenyAccountList.UnLock;
    end;
  end;
end;

procedure CmdShowDenyCharNameLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sCharName, sFixDeny: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    g_DenyChrNameList.Lock;
    try
      if g_DenyChrNameList.Count <= 0 then
      begin
        SysMsg('禁止登录角色列表为空。', c_Green, t_Hint);
        Exit;
      end;

      for I := 0 to g_DenyChrNameList.Count - 1 do
        SysMsg(g_DenyChrNameList.Strings[I], c_Green, t_Hint);
    finally
      g_DenyChrNameList.UnLock;
    end;
  end;
end;

procedure CmdShowDenyIPaddrLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sIPaddr, sFixDeny: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    g_DenyIPAddrList.Lock;
    try
      if g_DenyIPAddrList.Count <= 0 then
      begin
        SysMsg('禁止登录IP列表为空。', c_Green, t_Hint);
        Exit;
      end;

      for I := 0 to g_DenyIPAddrList.Count - 1 do
        SysMsg(g_DenyIPAddrList.Strings[I], c_Green, t_Hint);
    finally
      g_DenyIPAddrList.UnLock;
    end;
  end;
end;

procedure CmdShowDenyIPLocalLogon(PlayObject: TPlayObject; Cmd: pTGameCmd; sIPaddr, sFixDeny: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    g_DenyIPLocalList.Lock;
    try
      if g_DenyIPLocalList.Count <= 0 then
      begin
        SysMsg('禁止登录IP所在地列表为空。', c_Green, t_Hint);
        Exit;
      end;

      for I := 0 to g_DenyIPLocalList.Count - 1 do
      begin
        SysMsg(g_DenyIPLocalList.Strings[I], c_Green, t_Hint);
      end;
    finally
      g_DenyIPLocalList.UnLock;
    end;
  end;
end;

procedure CmdDisableSendMsg(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sHumanName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      OnlineObject.m_boFilterSendMsg := True;
    end;

    if g_DisableSendMsgList.IndexOf(sHumanName) >= 0 then
    begin
      SysMsg(sHumanName + ' 已经存在禁言列表中。', c_Green, t_Hint)
    end
    else
    begin
      g_DisableSendMsgList.Add(sHumanName);
      SaveDisableSendMsgList();
      SysMsg(sHumanName + ' 已加入禁言列表。', c_Green, t_Hint);
    end;
  end;
end;

procedure CmdDisableSendMsgList(PlayObject: TPlayObject; Cmd: pTGameCmd);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if g_DisableSendMsgList.Count <= 0 then
    begin
      SysMsg('禁言列表为空！', c_Red, t_Hint);
      Exit;
    end;

    SysMsg('禁言列表:', c_Blue, t_Hint);
    for I := 0 to g_DisableSendMsgList.Count - 1 do
      SysMsg(g_DisableSendMsgList.Strings[I], c_Green, t_Hint);
  end;
end;

procedure CmdEnableSendMsg(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  I: Integer;
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sHumanName = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称', c_Red, t_Hint);
      Exit;
    end;

    for I := g_DisableSendMsgList.Count - 1 downto 0 do
    begin
      if g_DisableSendMsgList.Count <= 0 then
        Break;

      if CompareText(sHumanName, g_DisableSendMsgList.Strings[I]) = 0 then
      begin
        OnlineObject := UserEngine.GetPlayObject(sHumanName);
        if OnlineObject <> nil then
        begin
          OnlineObject.m_boFilterSendMsg := False;
        end;
        g_DisableSendMsgList.Delete(I);
        SaveDisableSendMsgList();
        SysMsg(sHumanName + ' 已从禁言列表中删除。', c_Green, t_Hint);
        Exit;
      end;
    end;
    SysMsg(sHumanName + ' 没有被禁言！', c_Red, t_Hint);
  end;
end;

procedure CmdEndGuild(PlayObject: TPlayObject);
begin
  with PlayObject do
  begin
    if (m_MyGuild <> nil) then
    begin
      if (m_nGuildRankNo > 1) then
      begin
        if TGUild(m_MyGuild).IsMember(m_sCharName) and TGUild(m_MyGuild).DelMember(m_sCharName) then
        begin
          m_MyGuild := nil;
          RefRankInfo(0, '');
          RefShowName(); // 10/31
          SysMsg('你已经退出行会。', c_Green, t_Hint);

          if (g_FunctionNPC <> nil) then
          begin
            PlayObject.m_nScriptGotoCount := 0;
            g_FunctionNPC.GotoLable(PlayObject, '@ExitGuild', False);
          end;
        end;
      end
      else
        SysMsg('行会掌门人不能这样退出行会！', c_Red, t_Hint);
    end
    else
      SysMsg('你未加入行会，无需退出！', c_Red, t_Hint);
  end;
end;

procedure CmdFireBurn(PlayObject: TPlayObject; nInt, nTime, nN: Integer);
var
  FireBurnEvent: TFireBurnEvent;
begin
  with PlayObject do
  begin
    if m_btPermission < 6 then
      Exit;

    if (nInt = 0) or (nTime = 0) or (nN = 0) then
    begin
      SysMsg('命令格式: @' + g_GameCommand.FIREBURN.sCmd + ' nInt nTime nN', c_Red, t_Hint);
      Exit;
    end;

    FireBurnEvent := TFireBurnEvent.Create(PlayObject, m_nCurrX, m_nCurrY, nInt, nTime, nN);
    g_EventManager.AddEvent(FireBurnEvent);
  end;
end;

procedure CmdForcedWallconquestWar(PlayObject: TPlayObject; Cmd: pTGameCmd; sCASTLENAME: string);
var
  I: Integer;
  Castle: TUserCastle;
  Guild: TGUild;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sCASTLENAME = '' then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 城堡名称', c_Red, t_Hint);
      Exit;
    end;

    Castle := g_CastleManager.Find(sCASTLENAME);
    if Castle <> nil then
    begin
      if not Castle.m_boUnderWar then
      begin
        for I := 0 to g_GuildManager.GuildList.Count - 1 do
        begin
          Guild := TGUild(g_GuildManager.GuildList.Items[I]);
          Castle.AddAttackerInfo(Guild, 0);
        end;
        Castle.Save;
        Castle.m_boStartWar := False;
        Castle.StartWar;
      end
      else
        Castle.StopWar();
    end
    else
      SysMsg(Format(g_sGameCommandSbkGoldCastleNotFoundMsg, [sCASTLENAME]), c_Red, t_Hint);
  end;
end;

procedure CmdFreePenalty(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName <> '') and (sHumanName[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandFreePKHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject.m_nPkPoint := 0;
    OnlineObject.RefNameColor();

    { TODO -ochongchong -c新增 : 清除人物PK值时，也清除英雄PK值 【2013-08-15】 }
    if OnlineObject.m_MyHero <> nil then
    begin
      TSmartObject(OnlineObject.m_MyHero).m_nPkPoint := 0;
    end;

    OnlineObject.SysMsg(g_sGameCommandFreePKHumanMsg, c_Green, t_Hint);
    SysMsg(Format(g_sGameCommandFreePKMsg, [sHumanName]), c_Green, t_Hint);
  end;
end;

procedure CmdGroupRecall(PlayObject: TPlayObject; sCmd: string);
var
  I: Integer;
  dwValue: LongWord;
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if not(m_boRecallSuite or (m_btPermission >= 6)) then
    begin
      SysMsg('您现在还无法使用此功能！', g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
      Exit;
    end;

    if m_PEnvir.m_boNORECALL then
    begin
      SysMsg('此地图禁止使用此命令！', g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
      Exit;
    end;

    dwValue := (MyGetTickCount - m_dwGroupRcallTick) div 1000; // 时差 秒

    if dwValue < g_Config.dwGroupRecallTime then
    begin
      SysMsg(Format('%d 秒之后才可以再使用此功能！', [g_Config.dwGroupRecallTime - dwValue]), g_Config.btRedMsgFColor,
        g_Config.btRedMsgBColor, t_Hint);
      Exit;
    end;

    if m_GroupOwner = PlayObject then
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        m_GroupMembers.LockR(8);
      try
{$IFEND}
        for I := 1 to m_GroupMembers.Count - 1 do
        begin
          OnlineObject := TPlayObject(m_GroupMembers.Objects[I]);
          { TODO -opiaoyun -c新增: 如果对象允许天地合一、且在安全区，且勾选安全区禁止传送处理 【2013-07-24】 }
          if OnlineObject.m_boAllowGroupReCall then
          begin
            if OnlineObject.InSafeZone and g_Config.boGroupReCallNotInSafeZone then
            begin
              SysMsg(Format('%s 安全区禁止天地合一！', [OnlineObject.m_sCharName]), g_Config.btGreenMsgFColor,
                g_Config.btGreenMsgBColor, t_Hint);
            end
            else if OnlineObject.m_PEnvir.m_boNORECALL then
            begin
              SysMsg(Format('%s 所在的地图不允许传送。', [OnlineObject.m_sCharName]), g_Config.btRedMsgFColor,
                g_Config.btRedMsgBColor, t_Hint);
            end
            else
              RecallHuman(OnlineObject.m_sCharName);
          end
          else
            SysMsg(Format('%s 不允许天地合一！', [OnlineObject.m_sCharName]), g_Config.btGreenMsgFColor,
              g_Config.btGreenMsgBColor, t_Hint);
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          m_GroupMembers.UnLockR;
      end;
{$IFEND}
      m_dwGroupRcallTick := MyGetTickCount();
      m_wGroupRcallTime := g_Config.dwGroupRecallTime;
    end;
  end;
end;

procedure CmdGuildRecall(PlayObject: TPlayObject; sCmd, sParam: string);
var
  I, II: Integer;
  dwValue: LongWord;
  OnlineObject: TPlayObject;
  GuildRank: pTGuildRank;
  nRecallCount, nNoRecallCount: Integer;
  Castle: TUserCastle;
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('命令功能: 行会传送，行会掌门人可以将整个行会成员全部集中。', c_Red, t_Hint);
      Exit;
    end;

    if (not m_boGuildMove) and (m_btPermission < 6) then
    begin
      SysMsg('您现在还无法使用此功能！', c_Red, t_Hint);
      Exit;
    end;

    if not IsGuildMaster then
    begin
      SysMsg('行会掌门人才可以使用此功能！', c_Red, t_Hint);
      Exit;
    end;

    if m_PEnvir.m_boNOGUILDRECALL then
    begin
      SysMsg('本地图不允许使用此功能！', c_Red, t_Hint);
      Exit;
    end;

    Castle := g_CastleManager.InCastleWarArea(PlayObject);
    // if UserCastle.m_boUnderWar and UserCastle.InCastleWarArea(m_PEnvir,m_nCurrX,m_nCurrY) then begin
    if (Castle <> nil) and Castle.m_boUnderWar then
    begin
      SysMsg('攻城区域不允许使用此功能！', c_Red, t_Hint);
      Exit;
    end;

    nRecallCount := 0;
    nNoRecallCount := 0;
    dwValue := (MyGetTickCount - m_dwGuildRcallTick) div 1000;
    m_dwGuildRcallTick := m_dwGuildRcallTick + dwValue * 1000;
    if m_btPermission >= 6 then
      m_wGuildRcallTime := 0;

    if m_wGuildRcallTime > dwValue then
    begin
      Dec(m_wGuildRcallTime, dwValue);
    end
    else
      m_wGuildRcallTime := 0;

    if m_wGuildRcallTime > 0 then
    begin
      SysMsg(Format('%d 秒之后才可以再使用此功能！', [m_wGuildRcallTime]), c_Red, t_Hint);
      Exit;
    end;

    for I := 0 to TGUild(m_MyGuild).m_RankList.Count - 1 do
    begin
      GuildRank := TGUild(m_MyGuild).m_RankList.Items[I];
      if GuildRank = nil then
        Continue;

      for II := 0 to GuildRank.MemberList.Count - 1 do
      begin
        OnlineObject := TPlayObject(GuildRank.MemberList.Objects[II]);
        if OnlineObject <> nil then
        begin
          if OnlineObject = PlayObject then
            Continue;

          if OnlineObject.m_boAllowGuildReCall then
          begin
            if OnlineObject.InSafeZone and g_Config.boGroupReCallNotInSafeZone then
            begin
              SysMsg(Format('%s 安全区禁止行会合一！', [OnlineObject.m_sCharName]), g_Config.btGreenMsgFColor,
                g_Config.btGreenMsgBColor, t_Hint);
            end
            else if OnlineObject.m_PEnvir.m_boNORECALL then
            begin
              SysMsg(Format('%s 所在的地图不允许传送。', [OnlineObject.m_sCharName]), c_Red, t_Hint);
            end
            else
            begin
              RecallHuman(OnlineObject.m_sCharName);
              Inc(nRecallCount);
            end;
          end
          else
          begin
            Inc(nNoRecallCount);
            SysMsg(Format('%s 不允许行会合一！', [OnlineObject.m_sCharName]), c_Red, t_Hint);
          end;
        end;
      end;
    end;
    // SysMsg('已传送' + IntToStr(nRecallCount) + '个成员，' + IntToStr(nNoRecallCount) + '个成员未被传送。',c_Green,t_Hint);
    SysMsg(Format('已传送%d个成员，%d个成员未被传送。', [nRecallCount, nNoRecallCount]), c_Green, t_Hint);
    m_dwGuildRcallTick := MyGetTickCount();
    m_wGuildRcallTime := g_Config.nGuildRecallTime;
  end;
end;

procedure CmdGuildWar(PlayObject: TPlayObject; sCmd, sGuildName: string);
begin

end;

procedure CmdHair(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string; nHair: Integer);
var
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or (nHair < 0) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 人物名称 类型值', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      OnlineObject.m_btHair := nHair;
      OnlineObject.FeatureChanged();
      SysMsg(sHumanName + ' 的头发已改变。', c_Green, t_Hint);
    end
    else
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject <> nil then
      begin
        HeroObject.m_btHair := nHair;
        HeroObject.FeatureChanged();
        SysMsg(sHumanName + ' 的头发已改变。', c_Green, t_Hint);
      end
      else
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdHumanInfo(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandInfoHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    SysMsg(OnlineObject.GeTBaseObjectInfo(), c_Green, t_Hint);
  end;
end;

procedure CmdHumanLocal(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandHumanLocalHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    SysMsg(Format(g_sGameCommandHumanLocalMsg, [sHumanName, m_sIPLocal { GetIPLocal(PlayObject.m_sIPaddr) } ]), c_Green, t_Hint);
  end;
end;

procedure CmdHunger(PlayObject: TPlayObject; sCmd, sHumanName: string; nHungerPoint: Integer);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if (sHumanName = '') or (nHungerPoint < 0) then
    begin
      SysMsg('命令格式: @' + sCmd + ' 人物名称 能量值', c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      OnlineObject.m_nHungerStatus := nHungerPoint;
      OnlineObject.SendMsg(PlayObject, RM_MYSTATUS, 0, 0, 0, 0, '');
      OnlineObject.RefMyStatus();
      SysMsg(sHumanName + ' 的能量值已改变。', c_Green, t_Hint);
    end
    else
      SysMsg(sHumanName + '没有在线！', c_Red, t_Hint);
  end;
end;

procedure CmdIncPkPoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string; nPoint: Integer);
var
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandIncPkPointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    HeroObject := nil;
    OnlineObject := UserEngine.GetPlayObject(sHumanName);

    if OnlineObject = nil then
    begin
      HeroObject := UserEngine.GetHeroObject(sHumanName);
      if HeroObject = nil then
      begin
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
        Exit;
      end;
    end;

    if OnlineObject <> nil then
    begin
      Inc(OnlineObject.m_nPkPoint, nPoint);
      OnlineObject.RefNameColor();
    end
    else if HeroObject <> nil then
    begin
      Inc(HeroObject.m_nPkPoint, nPoint);
      HeroObject.RefNameColor();
    end;

    if nPoint > 0 then
      SysMsg(Format(g_sGameCommandIncPkPointAddPointMsg, [sHumanName, nPoint]), c_Green, t_Hint)
    else
      SysMsg(Format(g_sGameCommandIncPkPointDecPointMsg, [sHumanName, -nPoint]), c_Green, t_Hint);
  end;
end;

procedure CmdKickHuman(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumName: string);
var
  OnlineObject: TPlayObject;
  HeroObject: THeroObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumName = '') or ((sHumName <> '') and (sHumName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandKickHumanHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumName);
    if OnlineObject <> nil then
    begin
      OnlineObject.m_boOffLine := False;
      OnlineObject.m_boPlayOffLine := False;
      OnlineObject.m_boEmergencyClose := True;
      OnlineObject.m_boKickFlag := True;
    end
    else
    begin
      HeroObject := UserEngine.GetHeroObject(sHumName);
      if HeroObject <> nil then
        HeroObject.LogOut
      else
        SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumName]), c_Red, t_Hint);
    end;
  end;
end;

procedure CmdKill(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  BaseObject: TBaseObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if sHumanName <> '' then
    begin
      BaseObject := UserEngine.GetPlayObject(sHumanName);
      if BaseObject = nil then
      begin
        BaseObject := UserEngine.GetHeroObject(sHumanName);
        if BaseObject = nil then
          SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);

        Exit;
      end;
    end
    else
    begin
      BaseObject := GetPoseCreate();
      if BaseObject = nil then
      begin
        SysMsg('命令使用方法不正确，必须与角色面对面站好！', c_Red, t_Hint);
        Exit;
      end;
    end;

    if BaseObject <> nil then
      BaseObject.Die;
  end;
end;

procedure CmdLockLogin(PlayObject: TPlayObject; Cmd: pTGameCmd);
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if not g_Config.boLockHumanLogin then
    begin
      SysMsg('本服务器还没有启用登录锁功能！', c_Red, t_Hint);
      Exit;
    end;

    if m_boLockLogon and not m_boLockLogoned then
    begin
      SysMsg('您还没有打开登录锁或还没有设置锁密码！', c_Red, t_Hint);
      Exit;
    end;

    m_boLockLogon := not m_boLockLogon;
    if m_boLockLogon then
      SysMsg('已开启登录锁', c_Green, t_Hint)
    else
      SysMsg('已关闭登录锁', c_Green, t_Hint);
  end;
end;

procedure CmdLotteryTicket(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 = '') or ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    SysMsg(Format(g_sGameCommandLotteryTicketMsg, [g_Config.nWinLotteryCount, g_Config.nNoWinLotteryCount,
      g_Config.nWinLotteryLevel1, g_Config.nWinLotteryLevel2, g_Config.nWinLotteryLevel3, g_Config.nWinLotteryLevel4,
      g_Config.nWinLotteryLevel5, g_Config.nWinLotteryLevel6]), c_Green, t_Hint);
  end;
end;

procedure CmdLuckPoint(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sHumanName, sCtr, sPoint: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, g_sGameCommandLuckPointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    if sCtr = '' then
    begin
      SysMsg(Format(g_sGameCommandLuckPointMsg, [sHumanName, PlayObject.m_nBodyLuckLevel, OnlineObject.m_dBodyLuck,
        OnlineObject.m_nLuck]), c_Green, t_Hint);
      Exit;
    end;
  end;
end;

procedure CmdMakeItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sItemName: string; nCount: Integer; sParam1, sParam2: string);
var
  nParam2, nItemCount: Integer;
  dLastDate: PDouble;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  OverLapItem: pTUserItem;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sItemName = '') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGamecommandMakeHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    if (nCount <= 0) then
      nCount := 1;

    if (m_btPermission < Cmd.nPermissionMax) then
    begin
      if not CanMakeItem(sItemName) then
      begin
        SysMsg(g_sGamecommandMakeItemNameOrPerMissionNot, c_Red, t_Hint);
        Exit;
      end;

      if g_CastleManager.InCastleWarArea(PlayObject) <> nil then
      begin
        SysMsg(g_sGamecommandMakeInCastleWarRange, c_Red, t_Hint);
        Exit;
      end;

      if not InSafeZone then
      begin
        SysMsg(g_sGamecommandMakeInSafeZoneRange, c_Red, t_Hint);
        Exit;
      end;
      nCount := 1;
    end;

    while nCount > 0 do
    begin
      if m_ItemList.Count >= PlayObject.GetMaxBagCount then
        Exit;

      New(UserItem);
      if UserEngine.CopyToUserItemFromName(sItemName, UserItem) then
      begin
        if PlayObject.m_boFromGmExecute then
        begin
          UserItem.ItemFrom.ItemForm := ifScript;
          UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName;
          UserItem.ItemFrom.DateTime := Now();
        end
        else
        begin
          UserItem.ItemFrom.ItemForm := ifGM;
          UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName;
          UserItem.ItemFrom.DateTime := Now();
        end;

        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem.Price >= 15000) and not g_Config.boTestServer and (m_btPermission < 5) then
        begin
          Dispose(UserItem);
          Exit;
        end
        else if (g_Config.nMakeRandomAddValue > 0) and (Random(g_Config.nMakeRandomAddValue { 10 } ) = 0) then
          UserEngine.RandomUpgradeItem(UserItem);
        // 神秘装备
        if (StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26]) and (StdItem.Shape in [130, 131, 132]) then
          UserEngine.GetUnknowItemValue(UserItem);

        UserEngine.RandomItemNewAbil(u_Make, UserItem);
        // 制造的物品另行取得物品ID
        if m_btPermission >= Cmd.nPermissionMax then
          UserItem.MakeIndex := GetItemNumberEx();

        // 聚灵珠
        if StdItem.StdMode = 49 then
        begin
          UserItem.Dura := Min(StrToIntDef(sParam1, 0), UserItem.DuraMax);
          nParam2 := StrToIntDef(sParam2, -1);
          if nParam2 > 0 then
          begin
            dLastDate := @UserItem.btValue[4];
            dLastDate^ := Date + nParam2;
          end;
        end;

        nItemCount := OverLapBagItemCount(PlayObject, StdItem, OverLapItem);
        if (nItemCount > 0) and (OverLapItem <> nil) then
        begin
          nItemCount := Min(nItemCount, nCount);
          OverLapItem.Dura := OverLapItem.Dura + nItemCount;
          // Min(OverLapItem.Dura + nItemCount, OverLapItem.DuraMax - 1);
          Dispose(UserItem);
          UserItem := OverLapItem;
          Dec(nCount, nItemCount);
          SendUpDateItemDura(-1, UserItem.MakeIndex, False, UserItem.Dura);
        end
        else
        begin
          // 修正Make指令制造叠加物品时，数据多于最大叠加数量制造数量不对 chongchong 2014-04-15
          if CheckOverLapItem(StdItem) then
          begin
            if UserItem.DuraMax > nCount then
            begin
              UserItem.Dura := nCount - 1;
              nCount := 0;
            end
            else
            begin
              UserItem.Dura := UserItem.DuraMax - 1;
              nCount := nCount - UserItem.DuraMax;
            end;
            m_ItemList.Add(UserItem);
            SendAddItem(UserItem);
          end
          else
          begin
            m_ItemList.Add(UserItem);
            SendAddItem(UserItem);

            if (UserItem.boStartTime) then
            begin
              if StdItem.Need = 103 then
                SysMsg(Format('您的限时物品[%s]开始计时，有效时间%d分钟。', [StdItem.Name, StdItem.NeedLevel]), c_Red, t_System)
              else
              begin
                SysMsg(Format('您的限时物品[%s]开始计时，到期时间%s', [StdItem.Name, GetIncMinuteTime(UserItem.ItemFrom.DateTime,
                  StdItem.NeedLevel)]), c_Red, t_System);
              end;
            end;

            Dec(nCount);
          end;
        end;

        if g_Config.boShowMakeItemMsg and (m_btPermission >= 6) then
          MainOutMessage('[制造物品] ' + m_sCharName + ' ' + sItemName + '(' + IntToStr(UserItem.MakeIndex) + ')');

        if StdItem.NeedIdentify = 1 then
          AddGameDataLog(LOG_ItemMake, LOG_ActionNone, PlayObject, StdItem.Name, UserItem.MakeIndex, '0', 0, 0,
            '@' + Cmd.sCmd + ' ' + sItemName);
      end
      else
      begin
        Dispose(UserItem);
        SysMsg(Format(g_sGamecommandMakeItemNameNotFound, [sItemName]), c_Red, t_Hint);
        Break;
      end;
    end;
  end;

  PlayObject.WeightChanged;
end;

procedure CmdMapMove(PlayObject: TPlayObject; Cmd: pTGameCmd; sMapName: string);
var
  Envir: TEnvirnoment;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMapName = '') or ((sMapName <> '') and (sMapName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandMoveHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    if m_boOnHorse and (m_HorseOtherHum <> nil) then
    begin
      SysMsg(g_sDoubleHorseDisableMapMove, c_Red, t_Hint);
      Exit;
    end;

    Envir := g_MapManager.FindMap(sMapName);
    if (Envir = nil) then
    begin
      SysMsg(Format(g_sTheMapNotFound, [sMapName]) { + ' 此地图号不存在！' } , c_Red, t_Hint);
      Exit;
    end;

    if (m_btPermission >= Cmd.nPermissionMax) or CanMoveMap(sMapName) then
    begin
      SendRefMsg(RM_SPACEMOVE_FIRE, 0, 0, 0, 0, '');
      MapRandomMove(sMapName, 0);
    end
    else
      SysMsg(Format(g_sTheMapDisableMove, [sMapName, Envir.sMapDesc]) { '地图 ' + sParam1 + ' 不允许传送！' } , c_Red, t_Hint);
  end;
end;

procedure CmdPositionMove(PlayObject: TPlayObject; Cmd: pTGameCmd; sMapName, sX, sY: string);
var
  Envir: TEnvirnoment;
  nX, nY: Integer;
begin
  Envir := nil;
  try
    with PlayObject do
    begin
      if (m_btPermission < Cmd.nPermissionMin) then
      begin
        SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
        Exit;
      end;

      if m_boImprison then
      begin
        SysMsg('禁止使用此命令！', c_Red, t_Hint);
        Exit;
      end;

      if (sMapName = '') or (sX = '') or (sY = '') or ((sMapName <> '') and (sMapName[1] = '?')) then
      begin
        SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandPositionMoveHelpMsg]), c_Red, t_Hint);
        Exit;
      end;

      if (m_btPermission >= Cmd.nPermissionMax) or CanMoveMap(sMapName) then
      begin
        Envir := g_MapManager.FindMap(sMapName);
        if Envir <> nil then
        begin
          nX := StrToIntDef(sX, 0);
          nY := StrToIntDef(sY, 0);
          if Envir.CanWalk(nX, nY, True) then
            SpaceMove(sMapName, nX, nY, 0)
          else
            SysMsg(Format(g_sGameCommandPositionMoveCanotMoveToMap, [sMapName, sX, sY]), c_Green, t_Hint);
        end;
      end
      else if Envir <> nil then
        SysMsg(Format(g_sTheMapDisableMove, [sMapName, Envir.sMapDesc]), c_Red, t_Hint);
    end;
  except
    on E: Exception do
    begin
      MainOutMessage('[Exceptioin] CmdPositionMove');
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure CmdMapMoveHuman(PlayObject: TPlayObject; Cmd: pTGameCmd; sSrcMap, sDenMap: string);
var
  SrcEnvir, DenEnvir: TEnvirnoment;
  HumanList: TList;
  I: Integer;
  MoveHuman: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if m_boOnHorse and (m_HorseOtherHum <> nil) then
    begin
      SysMsg(g_sDoubleHorseDisableMapMove, c_Red, t_Hint);
      Exit;
    end;

    if (sDenMap = '') or (sSrcMap = '') or ((sSrcMap <> '') and (sSrcMap[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandMapMoveHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    SrcEnvir := g_MapManager.FindMap(sSrcMap);
    DenEnvir := g_MapManager.FindMap(sDenMap);
    if (SrcEnvir = nil) then
    begin
      SysMsg(Format(g_sGameCommandMapMoveMapNotFound, [sSrcMap]), c_Red, t_Hint);
      Exit;
    end;

    if (DenEnvir = nil) then
    begin
      SysMsg(Format(g_sGameCommandMapMoveMapNotFound, [sDenMap]), c_Red, t_Hint);
      Exit;
    end;

    HumanList := TList.Create;
    UserEngine.GetMapRageHuman(SrcEnvir, SrcEnvir.m_nWidth div 2, SrcEnvir.m_nHeight div 2, 1000, HumanList);
    for I := 0 to HumanList.Count - 1 do
    begin
      MoveHuman := TPlayObject(HumanList.Items[I]);
      if MoveHuman <> PlayObject then
        MoveHuman.MapRandomMove(sDenMap, 0);
    end;
    HumanList.Free;
  end;
end;

{
  procedure CmdUserCmd(PlayObject: TPlayObject; sLable: string);
  begin
  if g_FunctionNPC <> nil then
  begin
  PlayObject.m_nScriptGotoCount := 0;
  g_FunctionNPC.GotoLable(PlayObject, sLable, False);
  end;

  // 执行经络脚本 piaoyun 2013-08-16
  if g_BatterNPC <> nil then
  begin
  PlayObject.m_nScriptGotoCount := 0;
  g_BatterNPC.GotoLable(PlayObject, sLable, False);
  end;
  end;
}

procedure CmdMemberFunction(PlayObject: TPlayObject; sCmd, sParam: string);
begin
  if (sParam <> '') and (sParam[1] = '?') then
  begin
    PlayObject.SysMsg('打开会员功能窗口.', c_Red, t_Hint);
    Exit;
  end;

  if g_ManageNPC <> nil then
  begin
    PlayObject.m_nScriptGotoCount := 0;
    g_ManageNPC.GotoLable(PlayObject, '@Member', False);
  end;
end;

procedure CmdMemberFunctionEx(PlayObject: TPlayObject; sCmd, sParam: string);
begin
  if (sParam <> '') and (sParam[1] = '?') then
  begin
    PlayObject.SysMsg('打开会员功能窗口.', c_Red, t_Hint);
    Exit;
  end;

  if g_FunctionNPC <> nil then
  begin
    PlayObject.m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(PlayObject, '@Member', False);
  end;
end;

procedure CmdMission(PlayObject: TPlayObject; Cmd: pTGameCmd; sX, sY: string); // 004CCA08
var
  I, nXCount, nYCount: Integer;
  SL: TStringList;
  S: string;
begin
  if PlayObject.m_btPermission < Cmd.nPermissionMin then
  begin
    PlayObject.SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
    Exit;
  end;

  if (sX = '') or (sY = '') then
  begin
    PlayObject.SysMsg('命令格式: @' + Cmd.sCmd + ' X  Y', c_Red, t_Hint);
    Exit;
  end;

  SL := TStringList.Create;
  try
    SL.Delimiter := ';';

    SL.DelimitedText := sX;
    nXCount := SL.Count;

    SL.DelimitedText := sY;
    nYCount := SL.Count;

    if nXCount <> nYCount then
    begin
      PlayObject.SysMsg('命令格式: @' + Cmd.sCmd + ' X1;X2...XN  Y1;Y2...YN', c_Red, t_Hint);
      Exit;
    end;

    SetLength(g_nMissionPoints, nXCount);

    SL.DelimitedText := sX;
    for I := 0 to SL.Count - 1 do
    begin
      g_nMissionPoints[I].X := StrToIntDef(Trim(SL.Strings[I]), 0);
    end;

    SL.DelimitedText := sY;
    for I := 0 to SL.Count - 1 do
    begin
      g_nMissionPoints[I].Y := StrToIntDef(Trim(SL.Strings[I]), 0);
    end;

    g_boMission := True;
    g_sMissionMap := PlayObject.m_sMapName;

    for I := 0 to Length(g_nMissionPoints) - 1 do
    begin
      S := S + Format('%d:%d', [g_nMissionPoints[I].X, g_nMissionPoints[I].Y]);
      if I < Length(g_nMissionPoints) - 1 then
        S := S + ', ';
    end;
    PlayObject.SysMsg('怪物集中目标已设定为: ' + PlayObject.m_sMapName + '(' + S + ')', c_Green, t_Hint);
  finally
    SL.Free;
  end;
end;

procedure CmdMob(PlayObject: TPlayObject; Cmd: pTGameCmd; sMonName: string; nCount, nLevel: Integer; sNationaName: string;
  boCanAttackSameNationPlayer, boSameNationMonPK: Boolean; sFixedColor: string); // 004CC7F4
var
  I: Integer;
  nX, nY: Integer;
  Monster: TBaseObject;
  nColor: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMonName = '') or ((sMonName <> '') and (sMonName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandMobHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    if nCount <= 0 then
      nCount := 1;
    if not(nLevel in [0 .. 10]) then
      nLevel := 0;

    nCount := Min(64, nCount);
    PlayObject.GetFrontPosition(nX, nY);
    for I := 0 to nCount - 1 do
    begin
      Monster := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, nX, nY, sMonName);
      if Monster <> nil then
      begin
        Monster.m_btNation := StrToIntDef(sNationaName, 0); // GetNationIndex(
        Monster.m_boCanAttackSameNationPlayer := boCanAttackSameNationPlayer;
        Monster.m_boNoSameNationMonPK := boSameNationMonPK;

        nColor := StrToIntDef(sFixedColor, -1);
        if (nColor >= 0) and (nColor <= 255) then
        begin
          Monster.m_boFixedColor := True;
          Monster.m_btFixedColor := nColor;
        end;

        Monster.m_btSlaveMakeLevel := nLevel;
        Monster.m_btSlaveExpLevel := nLevel;
        Monster.RecalcAbilitys;

        if (nLevel <= SLAVEMAXLEVEL) then
        begin
          Monster.m_btNameColor := g_Config.SlaveColor[nLevel];
        end
        else
          Monster.m_btNameColor := g_Config.SlaveColor[High(g_Config.SlaveColor)];
        Monster.RefNameColor;
      end
      else
      begin
        SysMsg(g_sGameCommandMobMsg, c_Red, t_Hint);
        Break;
      end;
    end;
  end;
end;

procedure CmdMobCount(PlayObject: TPlayObject; Cmd: pTGameCmd; sMapName: string);
var
  Envir: TEnvirnoment;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMapName = '') or ((sMapName <> '') and (sMapName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandMobCountHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    Envir := g_MapManager.FindMap(sMapName);
    if Envir = nil then
    begin
      SysMsg(g_sGameCommandMobCountMapNotFound, c_Red, t_Hint);
      Exit;
    end;
    SysMsg(Format(g_sGameCommandMobCountMonsterCount, [UserEngine.GetMapMonster(Envir, nil)]), c_Green, t_Hint);
  end;
end;

procedure CmdHumanCount(PlayObject: TPlayObject; Cmd: pTGameCmd; sMapName: string);
var
  Envir: TEnvirnoment;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMapName = '') or ((sMapName <> '') and (sMapName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandHumanCountHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    Envir := g_MapManager.FindMap(sMapName);
    if Envir = nil then
    begin
      SysMsg(g_sGameCommandMobCountMapNotFound, c_Red, t_Hint);
      Exit;
    end;
    SysMsg(Format(g_sGameCommandMobCountMonsterCount, [UserEngine.GetMapHuman(sMapName)]), c_Green, t_Hint);
  end;
end;

procedure CmdMobFireBurn(PlayObject: TPlayObject; Cmd: pTGameCmd; sMAP, sX, sY, sType, sTime, sPoint: string);
var
  nX, nY, nType, nTime, nPoint: Integer;
  FireBurnEvent: TFireBurnEvent;
  Envir: TEnvirnoment;
  OldEnvir: TEnvirnoment;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMAP = '') or ((sMAP <> '') and (sMAP[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandMobFireBurnHelpMsg, [Cmd.sCmd, sMAP, sX, sY, sType, sTime, sPoint]), c_Red, t_Hint);
      Exit;
    end;

    nX := StrToIntDef(sX, -1);
    nY := StrToIntDef(sY, -1);
    nType := StrToIntDef(sType, -1);
    nTime := StrToIntDef(sTime, -1);
    nPoint := StrToIntDef(sPoint, -1);
    if nPoint < 0 then
      nPoint := 1;

    if (sMAP = '') or (nX < 0) or (nY < 0) or (nType < 0) or (nTime < 0) or (nPoint < 0) then
    begin
      SysMsg(Format(g_sGameCommandMobFireBurnHelpMsg, [Cmd.sCmd, sMAP, sX, sY, sType, sTime, sPoint]), c_Red, t_Hint);
      Exit;
    end;

    Envir := g_MapManager.FindMap(sMAP);
    if Envir <> nil then
    begin
      OldEnvir := m_PEnvir;
      try
        m_PEnvir := Envir;
        FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX, nY, nType, nTime * 1000, nPoint);
        g_EventManager.AddEvent(FireBurnEvent);
      finally
        m_PEnvir := OldEnvir;
      end;
      Exit;
    end;
    SysMsg(Format(g_sGameCommandMobFireBurnMapNotFountMsg, [Cmd.sCmd, sMAP]), c_Red, t_Hint);
  end;
end;

procedure CmdMobLevel(PlayObject: TPlayObject; Cmd: pTGameCmd; Param: string); // 004CFD5C
var
  I: Integer;
  BaseObjectList: TList;
  BaseObject: TBaseObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((Param <> '') and (Param[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    BaseObjectList := TList.Create;
    m_PEnvir.GetRangeBaseObject(m_nCurrX, m_nCurrY, 2, True, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      BaseObject := TBaseObject(BaseObjectList.Items[I]);
      SysMsg(BaseObject.GeTBaseObjectInfo(), c_Green, t_Hint);
    end;
    BaseObjectList.Free;
  end;
end;

procedure CmdMobNpc(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1, sParam2, sParam3, sParam4: string);
var
  nAppr: Integer;
  boIsCastle: Boolean;
  Merchant: TMerchant;
  nX, nY: Integer;
begin
  with PlayObject do
  begin
    if m_btPermission < nPermission then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 = '') or (sParam2 = '') or ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, g_sGameCommandMobNpcHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    nAppr := StrToIntDef(sParam3, 0);
    boIsCastle := (StrToIntDef(sParam4, 0) = 1);
    if sParam1 = '' then
    begin
      SysMsg('命令格式: @' + sCmd + ' NPC名称 脚本文件名 外形(数字) 属沙城(0,1)', c_Red, t_Hint);
      Exit;
    end;

    Merchant := TMerchant.Create;
    Merchant.m_sCharName := sParam1;
    Merchant.m_sMapName := m_sMapName;
    Merchant.m_PEnvir := m_PEnvir;
    Merchant.m_wAppr := nAppr;
    Merchant.m_nFlag := 0;
    Merchant.m_boCastle := boIsCastle;
    Merchant.m_sScript := sParam2;
    PlayObject.GetFrontPosition(nX, nY);
    Merchant.m_nCurrX := nX;
    Merchant.m_nCurrY := nY;
    Merchant.Initialize();
    UserEngine.AddMerchant(Merchant);
  end;
end;

procedure CmdMobPlace(PlayObject: TPlayObject; Cmd: pTGameCmd; sX, sY, sMonName, sCount: string);
var
  I: Integer;
  nCount, nX, nY: Integer;
  MEnvir: TEnvirnoment;
  mon: TBaseObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    nCount := Min(500, StrToIntDef(sCount, 0));
    nX := StrToIntDef(sX, 0);
    nY := StrToIntDef(sY, 0);
    MEnvir := g_MapManager.FindMap(g_sMissionMap);
    if (nX <= 0) or (nY <= 0) or (sMonName = '') or (nCount <= 0) then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' X  Y 怪物名称 怪物数量', c_Red, t_Hint);
      Exit;
    end;

    if not g_boMission or (MEnvir = nil) then
    begin
      SysMsg('还没有设定怪物集中点！', c_Red, t_Hint);
      SysMsg('请先用命令' + g_GameCommand.Mission.sCmd + '设置怪物的集中点。', c_Red, t_Hint);
      Exit;
    end;

    for I := 0 to nCount - 1 do
    begin
      mon := UserEngine.RegenMonsterByName(g_sMissionMap, nX, nY, sMonName);
      if mon <> nil then
      begin
        if Length(g_nMissionPoints) > 0 then
        begin
          mon.m_boMission := True;
          mon.m_nMissionPointIndex := -1;
          SetLength(mon.m_nMissionPoints, Length(g_nMissionPoints));
          Move(g_nMissionPoints[0], mon.m_nMissionPoints[0], Length(g_nMissionPoints) * SizeOf(TPoint));
        end;
      end
      else
        Break;
    end;

    if Length(g_nMissionPoints) > 0 then
    begin
      SysMsg(IntToStr(nCount) + ' 只 ' + sMonName + ' 已正在往地图 ' + g_sMissionMap + ' ' + IntToStr(g_nMissionPoints[0].X) + ':' +
        IntToStr(g_nMissionPoints[0].Y) + ' 集中。', c_Green, t_Hint);
    end;
  end;
end;

procedure CmdNpcScript(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1, sParam2, sParam3: string);
var
  BaseObject: TBaseObject;
  nNPCType: Integer;
  I: Integer;
  sScriptFileName: string;
  Merchant: TMerchant;
  NormNpc: TNormNpc;
  LoadList: TStringList;
  sScriptLine: string;
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 = '') or ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, g_sGameCommandNpcScriptHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    nNPCType := -1;
    BaseObject := GetPoseCreate();
    if BaseObject <> nil then
    begin
      for I := 0 to UserEngine.m_MerchantList.Count - 1 do
      begin
        if TBaseObject(UserEngine.m_MerchantList.Items[I]) = BaseObject then
        begin
          nNPCType := 0;
          Break;
        end;
      end;

      for I := 0 to UserEngine.QuestNPCList.Count - 1 do
      begin
        if TBaseObject(UserEngine.QuestNPCList.Objects[I]) = BaseObject then
        begin
          nNPCType := 1;
          Break;
        end;
      end;
    end;

    if nNPCType < 0 then
    begin
      SysMsg('命令使用方法不正确，必须与NPC面对面，才能使用此命令！', c_Red, t_Hint);
      Exit;
    end;

    if sParam1 = '' then
    begin
      if nNPCType = 0 then
      begin
        Merchant := TMerchant(BaseObject);
        sScriptFileName := g_Config.sEnvirDir + sMarket_Def + Merchant.m_sScript + '-' + Merchant.m_sMapName + '.txt';
      end;

      if nNPCType = 1 then
      begin
        NormNpc := TNormNpc(BaseObject);
        sScriptFileName := g_Config.sEnvirDir + sNpc_def + NormNpc.m_sCharName + '-' + NormNpc.m_sMapName + '.txt';
      end;

      if FileExists(sScriptFileName) then
      begin
        LoadList := TStringList.Create;
        try
          LoadList.LoadFromFile(sScriptFileName);
        except
          SysMsg('读取脚本文件错误: ' + sScriptFileName, c_Red, t_Hint);
        end;

        for I := 0 to LoadList.Count - 1 do
        begin
          sScriptLine := Trim(LoadList.Strings[I]);
          sScriptLine := ReplaceChar(sScriptLine, ' ', ',');
          SysMsg(IntToStr(I) + ',' + sScriptLine, c_Blue, t_Hint);
        end;
        LoadList.Free;
      end;
    end;
  end;
end;

procedure CmdOPDeleteSkill(PlayObject: TPlayObject; sHumanName, sSkillName: string); // 004CE938
begin

end;

procedure CmdOPTraining(sHumanName, sSkillName: string; nLevel: Integer);
begin

end;

procedure CmdPKpoint(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string); // 004CC61C
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandPKPointHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;
    SysMsg(Format(g_sGameCommandPKPointMsg, [sHumanName, OnlineObject.m_nPkPoint]), c_Green, t_Hint);
  end;
end;

procedure CmdPrvMsg(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sHumanName: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, g_sGameCommandPrvMsgHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    for I := m_BlockWhisperList.Count - 1 downto 0 do
    begin
      if m_BlockWhisperList.Count <= 0 then
        Break;

      if CompareText(m_BlockWhisperList.Strings[I], sHumanName) = 0 then
      begin
        m_BlockWhisperList.Delete(I);
        SysMsg(Format(g_sGameCommandPrvMsgUnLimitMsg, [sHumanName]), c_Green, t_Hint);
        Exit;
      end;
    end;
    m_BlockWhisperList.Add(sHumanName);
    SysMsg(Format(g_sGameCommandPrvMsgLimitMsg, [sHumanName]), c_Green, t_Hint);
  end;
end;

procedure CmdReAlive(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandReAliveHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    if (OnlineObject.m_PEnvir <> nil) and ((OnlineObject.m_PEnvir.m_nRevivalMaxCount < 0) or
      (OnlineObject.m_nRevivalCount < OnlineObject.m_PEnvir.m_nRevivalMaxCount)) then
    begin
      if OnlineObject.m_PEnvir.m_nRevivalMaxCount >= 0 then
        Inc(OnlineObject.m_nRevivalCount);
      OnlineObject.ReAlive;

      OnlineObject.SendMsg(OnlineObject, RM_ABILITY, 0, 0, 0, 0, '');

      SysMsg(Format(g_sGameCommandReAliveMsg, [sHumanName]), c_Green, t_Hint);
    end;
  end;
end;

procedure CmdRecallHuman(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string); // 004CE250
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandRecallHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
    RecallHuman(sHumanName);
  end;
end;

procedure CmdRecallMob(PlayObject: TPlayObject; Cmd: pTGameCmd; sMonName: string;
  nCount, nLevel, nAutoChangeColor, nBodyColor: Integer); // 004CC8C4
var
  I: Integer;
  mon: TBaseObject;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sMonName = '') or ((sMonName <> '') and (sMonName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandRecallMobHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    if nLevel >= 10 then
      nLevel := 9;
    if nCount <= 0 then
      nCount := 1;

    for I := 0 to nCount - 1 do
    begin
      if m_SlaveList.Count >= 20 then
        Break;
      // chongchong 2014-05-07
      mon := MakeSlave(sMonName, 3, nLevel, 100, 10 * 24 * 60 * 60, bb_Other, 0, not g_Config.boBBAttrPlusAddOnlyMagic);
      if mon <> nil then
      begin
        mon.m_boAutoChangeColor := nAutoChangeColor = 1;
        mon.m_nAutoChangeIdx := 0;
        mon.m_dwChangeBodyColorTime := 0;
        mon.m_dwStartChangeBodyColor := MyGetTickCount;

        if not mon.m_boAutoChangeColor then
        begin
          mon.m_btBodyColor := nBodyColor;
        end;

        mon.RecalcAbilitys();
        mon.RefNameColor();
      end;

      {
        PlayObject.GetFrontPosition(n10, n14);
        mon := UserEngine.RegenMonsterByName(m_PEnvir.sMapName, n10, n14, sMonName);
        if mon <> nil then
        begin
        mon.m_Master := PlayObject;
        mon.m_dwMasterRoyaltyTick := MyGetTickCount + 24 * 60 * 60 * 1000;
        mon.m_boNextCycleMasterRoyalty := mon.m_dwMasterRoyaltyTick < MyGetTickCount;
        mon.m_btSlaveMakeLevel := 3;
        mon.m_btSlaveExpLevel := nLevel;
        if nAutoChangeColor = 1 then
        begin
        mon.m_boAutoChangeColor := True;
        end
        else if nFixColor > 0 then
        begin
        mon.m_boFixColor := True;
        mon.m_nFixColorIdx := nFixColor - 1;
        end;

        mon.RecalcAbilitys();
        mon.RefNameColor();
        m_SlaveList.Add(mon);
        if mon.m_PEnvir <> nil then
        mon.m_PEnvir.AddObject(mon);
        end;
      }
    end;
  end;
end;

procedure CmdReconnection(PlayObject: TPlayObject; sCmd, sUserName, sIPaddr, sPort: string);
var
  Player: TPlayObject;
begin
  Player := UserEngine.GetPlayObject(sUserName);
  if Player = nil then
  begin
    PlayObject.SysMsg('命令格式: @' + sCmd + ' 人物名称 IP地址 端口', c_Red, t_Hint);
    Exit;
  end;

  if (PlayObject.m_btPermission < 10) then
    Exit;

  if (sIPaddr <> '') and (sIPaddr[1] = '?') then
  begin
    PlayObject.SysMsg('此命令用于改变客户端连接网关的IP及端口。', c_Blue, t_Hint);
    Exit;
  end;

  if (sIPaddr = '') or (sPort = '') then
  begin
    PlayObject.SysMsg('命令格式: @' + sCmd + ' 人物名称 IP地址 端口', c_Red, t_Hint);
    Exit;
  end;

  if (sIPaddr <> '') and (sPort <> '') then
    Player.SendMsg(Player, RM_RECONNECTION, 0, 0, 0, 0, sIPaddr + '/' + sPort);
end;

procedure CmdChangeGate(PlayObject: TPlayObject; sCmd, sOldGateIndex, sNewIPAddr, sNewPort: string);
var
  I: Integer;
  Player: TPlayObject;
  Gate: pTGateInfo;
  nOldGateIndex, nNewPort: Integer;
  UserList: TList;
  GateUser: pTGateUserInfo;
begin
  if (sOldGateIndex <> '') and (sOldGateIndex[1] = '?') then
  begin
    PlayObject.SysMsg('此命令用于切换网络连接。', c_Blue, t_Hint);
    Exit;
  end;

  if (sOldGateIndex = '') or (sNewIPAddr = '') or (sNewPort = '') then
  begin
    PlayObject.SysMsg('命令格式: @' + sCmd + ' 网关序号 新网关地址 新网关端口', c_Red, t_Hint);
    Exit;
  end;

  if (PlayObject.m_btPermission < 10) then
    Exit;

  nOldGateIndex := StrToIntDef(sOldGateIndex, -1);
  nNewPort := StrToIntDef(sNewPort, 0);
  if (nOldGateIndex < Low(g_GateArr)) or (nOldGateIndex > High(g_GateArr)) or (nNewPort <= 0) or (nNewPort > 65535) then
    Exit;

  Gate := @g_GateArr[nOldGateIndex];
  UserList := Gate.UserList;
  for I := UserList.Count - 1 downto 0 do
  begin
    GateUser := UserList.Items[I];
    if GateUser <> nil then
    begin
      Player := GateUser.PlayObject;
      if (Player <> nil) and (not Player.m_boDummyObject) then
        Player.SendMsg(Player, RM_RECONNECTION, 0, 0, 0, 0, sNewIPAddr + '/' + sNewPort);
    end;
  end;
end;

procedure CmdRefineWeapon(PlayObject: TPlayObject; Cmd: pTGameCmd; nDc, nMc, nSc, nHit: Integer); // 004CD1C4
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (nDc + nMc + nSc) > 10 then
      Exit;

    if m_UseItems[U_WEAPON].wIndex <= 0 then
      Exit;

    m_UseItems[U_WEAPON].btValue[0] := nDc;
    m_UseItems[U_WEAPON].btValue[1] := nMc;
    m_UseItems[U_WEAPON].btValue[2] := nSc;
    m_UseItems[U_WEAPON].btValue[5] := nHit;
    SendUpdateItem(@m_UseItems[U_WEAPON]);
    RecalcAbilitys();
    SendMsg(PlayObject, RM_ABILITY, 0, 0, 0, 0, '');
    SendMsg(PlayObject, RM_SUBABILITY, 0, 0, 0, 0, '');
    MainOutMessage('[武器调整]' + m_sCharName + ' DC:' + IntToStr(nDc) + ' MC' + IntToStr(nMc) + ' SC' + IntToStr(nSc) + ' HIT:' +
      IntToStr(nHit));
  end;
end;

procedure CmdReGotoHuman(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandReGotoHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;
    SpaceMove(OnlineObject.m_PEnvir.sMapName, OnlineObject.m_nCurrX, OnlineObject.m_nCurrY, 0);
  end;
end;

procedure CmdReloadAbuse(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;
  end;
end;

procedure CmdReLoadAdmin(PlayObject: TPlayObject; sCmd: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    FrmDB.LoadAdminList();
    SysMsg('管理员列表重新加载成功...', c_Green, t_Hint);
  end;
end;

procedure CmdReloadGuild(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string);
var
  Guild: TGUild;
begin
  with PlayObject do
  begin
    if (m_btPermission < nPermission) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 = '') or ((sParam1 <> '') and (sParam1[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, g_sGameCommandReloadGuildHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    if nServerIndex <> 0 then
    begin
      SysMsg(g_sGameCommandReloadGuildOnMasterserver, c_Red, t_Hint);
      Exit;
    end;

    Guild := g_GuildManager.FindGuild(sParam1);
    if Guild = nil then
    begin
      SysMsg(Format(g_sGameCommandReloadGuildNotFoundGuildMsg, [sParam1]), c_Red, t_Hint);
      Exit;
    end;

    Guild.LoadGuild();
    SysMsg(Format(g_sGameCommandReloadGuildSuccessMsg, [sParam1]), c_Red, t_Hint);
  end;
end;

procedure CmdReloadGuildAll(PlayObject: TPlayObject); // 004CE530
begin

end;

procedure CmdReloadLineNotice(PlayObject: TPlayObject; sCmd: string; nPermission: Integer; sParam1: string);
begin
  with PlayObject do
  begin
    if m_btPermission < nPermission then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    if LoadLineNotice(g_Config.sNoticeDir + 'LineNotice.txt') then
      SysMsg(g_sGameCommandReloadLineNoticeSuccessMsg, c_Green, t_Hint)
    else
      SysMsg(g_sGameCommandReloadLineNoticeFailMsg, c_Red, t_Hint);
  end;
end;

procedure CmdReloadManage(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam: string);
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    if sParam = '' then
    begin
      if g_ManageNPC <> nil then
      begin
        g_ManageNPC.ClearScript();
        g_ManageNPC.LoadNpcScript();
        SysMsg('重新加载登录脚本成功.', c_Green, t_Hint);
      end
      else
      begin
        SysMsg('重新加载登录脚本失败.', c_Green, t_Hint);
      end;
    end
    else
    begin
      if g_FunctionNPC <> nil then
      begin
        g_FunctionNPC.ClearScript();
        g_FunctionNPC.LoadNpcScript();
        SysMsg('重新加载功能脚本成功.', c_Green, t_Hint);
      end
      else
        SysMsg('重新加载功能脚本失败.', c_Green, t_Hint);
    end;
  end;
end;

procedure CmdReloadRobot(PlayObject: TPlayObject);
begin
  with PlayObject do
  begin
    RobotManage.RELOADROBOT();
    SysMsg('重新加载机器人配置成功.', c_Green, t_Hint);
  end;
end;

procedure CmdReloadRobotManage(PlayObject: TPlayObject);
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if g_RobotNPC <> nil then
    begin
      g_RobotNPC.ClearScript();
      g_RobotNPC.LoadNpcScript();
      SysMsg('重新加载机器人专用脚本成功.', c_Green, t_Hint);
    end
    else
      SysMsg('重新加载机器人专用脚本失败.', c_Green, t_Hint);
  end;
end;

procedure CmdReloadMonItems(PlayObject: TPlayObject); //
var
  I: Integer;
  Monster: pTMonInfo;
begin
  with PlayObject do
  begin
    if m_btPermission < 6 then
      Exit;

    try
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        UserEngine.MonsterList.LockR(8);
      try
{$IFEND}
        for I := 0 to UserEngine.MonsterList.Count - 1 do
        begin
          Monster := UserEngine.MonsterList.Items[I];
          FrmDB.LoadMonitems(Monster.sName, Monster.ItemList);
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UserEngine.MonsterList.UnLockR;
      end;
{$IFEND}
      SysMsg('怪物爆物品列表重加载成功.', c_Green, t_Hint);
    except
      SysMsg('怪物爆物品列表重加载失败！', c_Green, t_Hint);
    end;
  end;
end;

procedure CmdReloadNpc(PlayObject: TPlayObject; sParam: string); // 004CFFF8
var
  I: Integer;
  TmpList: TList;
  Merchant: TMerchant;
  NPC: TNormNpc;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if CompareText('all', sParam) = 0 then
    begin
      FrmDB.ReLoadMerchants();
      UserEngine.ReloadMerchantList();
      SysMsg('交易NPC重新加载成功.', c_Red, t_Hint);
      UserEngine.ReloadNpcList();
      SysMsg('管理NPC重新加载成功.', c_Red, t_Hint);
      Exit;
    end;

    TmpList := TList.Create;
    if UserEngine.GetMerchantList(m_PEnvir, m_nCurrX, m_nCurrY, 9, TmpList) > 0 then
    begin
      for I := 0 to TmpList.Count - 1 do
      begin
        Merchant := TMerchant(TmpList.Items[I]);
        Merchant.ClearScript;
        Merchant.LoadNpcScript;
        SysMsg(Merchant.m_sCharName + '重新加载成功.', c_Green, t_Hint);
      end;
    end
    else
      SysMsg('附近未发现任何交易NPC！', c_Red, t_Hint);

    TmpList.Clear;
    if UserEngine.GetNpcList(m_PEnvir, m_nCurrX, m_nCurrY, 9, TmpList) > 0 then
    begin
      for I := 0 to TmpList.Count - 1 do
      begin
        NPC := TNormNpc(TmpList.Items[I]);
        NPC.ClearScript;
        NPC.LoadNpcScript;
        SysMsg(NPC.m_sCharName + '重新加载成功.', c_Green, t_Hint);
      end; // for
    end
    else
      SysMsg('附近未发现任何管理NPC！', c_Red, t_Hint);

    TmpList.Free;
  end;
end;

procedure CmdSearchHuman(PlayObject: TPlayObject; sCmd, sHumanName: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if m_boProbeNecklace or (m_btPermission >= 6) then
    begin
      if (sHumanName = '') then
      begin
        SysMsg('命令格式: @' + sCmd + ' 人物名称', c_Red, t_Hint);
        Exit;
      end;

      if ((MyGetTickCount - m_dwProbeTick) > 10000) or (m_btPermission >= 3) then
      begin
        m_dwProbeTick := MyGetTickCount();
        OnlineObject := UserEngine.GetPlayObject(sHumanName);
        if OnlineObject <> nil then
        begin
          SysMsg(sHumanName + ' 现在位于 ' + OnlineObject.m_PEnvir.sMapDesc + ' ' + IntToStr(OnlineObject.m_nCurrX) + ':' +
            IntToStr(OnlineObject.m_nCurrY), c_Blue, t_Hint);
        end
        else
          SysMsg(sHumanName + ' 现在不在线，或位于其它服务器上！', c_Red, t_Hint);
      end
      else
        SysMsg(IntToStr((MyGetTickCount - m_dwProbeTick) div 1000 - 10) + ' 秒之后才可以再使用此功能！', c_Red, t_Hint);
    end
    else
      SysMsg('您现在还无法使用此功能！', c_Red, t_Hint);
  end;
end;

procedure CmdShowSbkGold(PlayObject: TPlayObject; Cmd: pTGameCmd; sCASTLENAME, sCtr, sGold: string);
var
  I: Integer;
  Ctr: Char;
  nGold: Integer;
  Castle: TUserCastle;
  List: TStringList;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sCASTLENAME <> '') and (sCASTLENAME[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    if sCASTLENAME = '' then
    begin
      List := TStringList.Create;
      g_CastleManager.GetCastleGoldInfo(List);
      for I := 0 to List.Count - 1 do
      begin
        SysMsg(List.Strings[I], c_Green, t_Hint);
      end;
      List.Free;
      Exit;
    end;

    Castle := g_CastleManager.Find(sCASTLENAME);
    if Castle = nil then
    begin
      SysMsg(Format(g_sGameCommandSbkGoldCastleNotFoundMsg, [sCASTLENAME]), c_Red, t_Hint);
      Exit;
    end;

    Ctr := sCtr[1];
    nGold := StrToIntDef(sGold, -1);

{$IF CompilerVersion >= 22}
    if not CharInSet(Ctr, ['=', '-', '+']) or (nGold < 0) or (nGold > 100000000) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandSbkGoldHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
{$ELSE}
    if not(Ctr in ['=', '-', '+']) or (nGold < 0) or (nGold > 100000000) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandSbkGoldHelpMsg]), c_Red, t_Hint);
      Exit;
    end;
{$IFEND}
    case Ctr of
      '=':
        Castle.m_nTotalGold := nGold;
      '-':
        Dec(Castle.m_nTotalGold);
      '+':
        Inc(Castle.m_nTotalGold, nGold);
    end;
    if Castle.m_nTotalGold < 0 then
      Castle.m_nTotalGold := 0;
  end;
end;

procedure CmdShowUseItemInfo(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  I: Integer;
  OnlineObject: TPlayObject;
  UserItem: pTUserItem;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandShowUseItemInfoHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    for I := Low(OnlineObject.m_UseItems) to High(OnlineObject.m_UseItems) do
    begin
      UserItem := @OnlineObject.m_UseItems[I];
      if UserItem.wIndex = 0 then
        Continue;

      SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]', [GetUseItemName(I), UserEngine.GetStdItemName(UserItem.wIndex),
        UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax]), c_Blue, t_Hint);
    end;
  end;
end;

procedure CmdBindUseItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sItem, sType: string);
var
  I: Integer;
  OnlineObject: TPlayObject;
  UserItem: pTUserItem;
  nItem, nBind: Integer;
  ItemBind: pTItemBind;
  nItemIdx, nMakeIdex: Integer;
  sBindName: string;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    nBind := -1;
    nItem := GetUseItemIdx(sItem);
    if CompareText(sType, '帐号') = 0 then
      nBind := 0;
    if CompareText(sType, '人物') = 0 then
      nBind := 1;
    if CompareText(sType, 'IP') = 0 then
      nBind := 2;

    if (nItem < 0) or (nBind < 0) or (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandBindUseItemHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    UserItem := @OnlineObject.m_UseItems[nItem];
    if UserItem.wIndex = 0 then
    begin
      SysMsg(Format(g_sGameCommandBindUseItemNoItemMsg, [sHumanName, sItem]), c_Red, t_Hint);
      Exit;
    end;

    nItemIdx := UserItem.wIndex;
    nMakeIdex := UserItem.MakeIndex;
    case nBind of
      0:
        begin
          sBindName := OnlineObject.m_sUserID;
          g_ItemBindAccount.Lock;
          try
            for I := 0 to g_ItemBindAccount.Count - 1 do
            begin
              ItemBind := g_ItemBindAccount.Items[I];
              if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
              begin
                SysMsg(Format(g_sGameCommandBindUseItemAlreadBindMsg, [sHumanName, sItem]), c_Red, t_Hint);
                Exit;
              end;
            end;
            New(ItemBind);
            ItemBind.nItemIdx := nItemIdx;
            ItemBind.nMakeIdex := nMakeIdex;
            ItemBind.sBindName := sBindName;
            g_ItemBindAccount.Insert(0, ItemBind);
          finally
            g_ItemBindAccount.UnLock;
          end;

          SaveItemBindAccount();
          SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。', [GetUseItemName(nItem),
            UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax,
            sBindName]), c_Blue, t_Hint);
          OnlineObject.SysMsg(Format('你的%s[%s]已经绑定到%s[%s]上了。', [GetUseItemName(nItem), UserEngine.GetStdItemName(UserItem.wIndex),
            sType, sBindName]), c_Blue, t_Hint);
        end;
      1:
        begin
          sBindName := OnlineObject.m_sCharName;
          g_ItemBindCharName.Lock;
          try
            for I := 0 to g_ItemBindCharName.Count - 1 do
            begin
              ItemBind := g_ItemBindCharName.Items[I];
              if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
              begin
                SysMsg(Format(g_sGameCommandBindUseItemAlreadBindMsg, [sHumanName, sItem]), c_Red, t_Hint);
                Exit;
              end;
            end;
            New(ItemBind);
            ItemBind.nItemIdx := nItemIdx;
            ItemBind.nMakeIdex := nMakeIdex;
            ItemBind.sBindName := sBindName;
            g_ItemBindCharName.Insert(0, ItemBind);
          finally
            g_ItemBindCharName.UnLock;
          end;

          SaveItemBindCharName();
          SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。', [GetUseItemName(nItem),
            UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax,
            sBindName]), c_Blue, t_Hint);
          OnlineObject.SysMsg(Format('你的%s[%s]已经绑定到%s[%s]上了。', [GetUseItemName(nItem), UserEngine.GetStdItemName(UserItem.wIndex),
            sType, sBindName]), c_Blue, t_Hint);
        end;
      2:
        begin
          sBindName := OnlineObject.m_sIPaddr;
          g_ItemBindIPaddr.Lock;
          try
            for I := 0 to g_ItemBindIPaddr.Count - 1 do
            begin
              ItemBind := g_ItemBindIPaddr.Items[I];
              if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
              begin
                SysMsg(Format(g_sGameCommandBindUseItemAlreadBindMsg, [sHumanName, sItem]), c_Red, t_Hint);
                Exit;
              end;
            end;
            New(ItemBind);
            ItemBind.nItemIdx := nItemIdx;
            ItemBind.nMakeIdex := nMakeIdex;
            ItemBind.sBindName := sBindName;
            g_ItemBindIPaddr.Insert(0, ItemBind);
          finally
            g_ItemBindIPaddr.UnLock;
          end;

          SaveItemBindIPaddr();
          SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。', [GetUseItemName(nItem),
            UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax,
            sBindName]), c_Blue, t_Hint);
          OnlineObject.SysMsg(Format('你的%s[%s]已经绑定到%s[%s]上了。', [GetUseItemName(nItem), UserEngine.GetStdItemName(UserItem.wIndex),
            sType, sBindName]), c_Blue, t_Hint);
        end;
    end;
  end;
end;

procedure CmdUnBindUseItem(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sItem, sType: string);
var
  I: Integer;
  OnlineObject: TPlayObject;
  UserItem: pTUserItem;
  nItem, nBind: Integer;
  ItemBind: pTItemBind;
  nItemIdx, nMakeIdex: Integer;
  sBindName: string;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    nBind := -1;
    nItem := GetUseItemIdx(sItem);
    if CompareText(sType, '帐号') = 0 then
      nBind := 0;
    if CompareText(sType, '人物') = 0 then
      nBind := 1;
    if CompareText(sType, 'IP') = 0 then
      nBind := 2;

    if (nItem < 0) or (nBind < 0) or (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandBindUseItemHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject = nil then
    begin
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
      Exit;
    end;

    UserItem := @OnlineObject.m_UseItems[nItem];
    if UserItem.wIndex = 0 then
    begin
      SysMsg(Format(g_sGameCommandBindUseItemNoItemMsg, [sHumanName, sItem]), c_Red, t_Hint);
      Exit;
    end;

    nItemIdx := UserItem.wIndex;
    nMakeIdex := UserItem.MakeIndex;
    case nBind of //
      0:
        begin
          sBindName := OnlineObject.m_sUserID;
          g_ItemBindAccount.Lock;
          try
            for I := 0 to g_ItemBindAccount.Count - 1 do
            begin
              ItemBind := g_ItemBindAccount.Items[I];
              if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
              begin
                SysMsg(Format(g_sGameCommandBindUseItemAlreadBindMsg, [sHumanName, sItem]), c_Red, t_Hint);
                Exit;
              end;
            end;
            New(ItemBind);
            ItemBind.nItemIdx := nItemIdx;
            ItemBind.nMakeIdex := nMakeIdex;
            ItemBind.sBindName := sBindName;
            g_ItemBindAccount.Insert(0, ItemBind);
          finally
            g_ItemBindAccount.UnLock;
          end;

          SaveItemBindAccount();
          SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。', [GetUseItemName(nItem),
            UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax,
            sBindName]), c_Blue, t_Hint);
          OnlineObject.SysMsg(Format('你的%s[%s]已经绑定到%s[%s]上了。', [GetUseItemName(nItem), UserEngine.GetStdItemName(UserItem.wIndex),
            sType, sBindName]), c_Blue, t_Hint);
        end;
      1:
        begin
          sBindName := OnlineObject.m_sCharName;
          g_ItemBindCharName.Lock;
          try
            for I := 0 to g_ItemBindCharName.Count - 1 do
            begin
              ItemBind := g_ItemBindCharName.Items[I];
              if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
              begin
                SysMsg(Format(g_sGameCommandBindUseItemAlreadBindMsg, [sHumanName, sItem]), c_Red, t_Hint);
                Exit;
              end;
            end;
            New(ItemBind);
            ItemBind.nItemIdx := nItemIdx;
            ItemBind.nMakeIdex := nMakeIdex;
            ItemBind.sBindName := sBindName;
            g_ItemBindCharName.Insert(0, ItemBind);
          finally
            g_ItemBindCharName.UnLock;
          end;

          SaveItemBindCharName();
          SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。', [GetUseItemName(nItem),
            UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax,
            sBindName]), c_Blue, t_Hint);
          OnlineObject.SysMsg(Format('你的%s[%s]已经绑定到%s[%s]上了。', [GetUseItemName(nItem), UserEngine.GetStdItemName(UserItem.wIndex),
            sType, sBindName]), c_Blue, t_Hint);
        end;
      2:
        begin
          sBindName := OnlineObject.m_sIPaddr;
          g_ItemBindIPaddr.Lock;
          try
            for I := 0 to g_ItemBindIPaddr.Count - 1 do
            begin
              ItemBind := g_ItemBindIPaddr.Items[I];
              if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
              begin
                SysMsg(Format(g_sGameCommandBindUseItemAlreadBindMsg, [sHumanName, sItem]), c_Red, t_Hint);
                Exit;
              end;
            end;
            New(ItemBind);
            ItemBind.nItemIdx := nItemIdx;
            ItemBind.nMakeIdex := nMakeIdex;
            ItemBind.sBindName := sBindName;
            g_ItemBindIPaddr.Insert(0, ItemBind);
          finally
            g_ItemBindIPaddr.UnLock;
          end;

          SaveItemBindIPaddr();
          SysMsg(Format('%s[%s]IDX[%d]系列号[%d]持久[%d-%d]，绑定到%s成功。', [GetUseItemName(nItem),
            UserEngine.GetStdItemName(UserItem.wIndex), UserItem.wIndex, UserItem.MakeIndex, UserItem.Dura, UserItem.DuraMax,
            sBindName]), c_Blue, t_Hint);
          OnlineObject.SysMsg(Format('你的%s[%s]已经绑定到%s[%s]上了。', [GetUseItemName(nItem), UserEngine.GetStdItemName(UserItem.wIndex),
            sType, sBindName]), c_Blue, t_Hint);
        end;
    end;
  end;
end;

procedure CmdShutup(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sTime: string);
var
  dwTime: LongWord;
  nIndex: Integer;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sTime = '') or (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandShutupHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    dwTime := StrToIntDef(sTime, 5);
    g_DenySayMsgList.Lock;
    try
      nIndex := g_DenySayMsgList.GetIndex(sHumanName);
      if nIndex >= 0 then
        g_DenySayMsgList.Objects[nIndex] := TObject(MyGetTickCount + dwTime * 60 * 1000)
      else
        g_DenySayMsgList.AddRecord(sHumanName, MyGetTickCount + dwTime * 60 * 1000);
    finally
      g_DenySayMsgList.UnLock;
    end;
    SysMsg(Format(g_sGameCommandShutupHumanMsg, [sHumanName, dwTime]), c_Red, t_Hint);
  end;
end;

procedure CmdShutupList(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam1: string);
var
  I: Integer;
begin
  with PlayObject do
  begin
    if m_btPermission < Cmd.nPermissionMin then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, '']), c_Red, t_Hint);
      Exit;
    end;

    if m_btPermission < 6 then
      Exit;

    g_DenySayMsgList.Lock;
    try
      if g_DenySayMsgList.Count <= 0 then
      begin
        SysMsg(g_sGameCommandShutupListIsNullMsg, c_Green, t_Hint);
        Exit;
      end;

      for I := 0 to g_DenySayMsgList.Count - 1 do
      begin
        SysMsg(g_DenySayMsgList.Strings[I] + ' ' + IntToStr((LongWord(g_DenySayMsgList.Objects[I]) - MyGetTickCount) div 60000),
          c_Green, t_Hint);
      end;
    finally
      g_DenySayMsgList.UnLock;
    end;
  end;
end;

procedure CmdShutupRelease(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string; boAll: Boolean);
var
  I: Integer;
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandShutupReleaseHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    g_DenySayMsgList.Lock;
    try
      I := g_DenySayMsgList.GetIndex(sHumanName);
      if I >= 0 then
      begin
        g_DenySayMsgList.Delete(I);
        OnlineObject := UserEngine.GetPlayObject(sHumanName);
        if OnlineObject <> nil then
          OnlineObject.SysMsg(g_sGameCommandShutupReleaseCanSendMsg, c_Red, t_Hint);

        SysMsg(Format(g_sGameCommandShutupReleaseHumanCanSendMsg, [sHumanName]), c_Green, t_Hint);
      end;
    finally
      g_DenySayMsgList.UnLock;
    end;
  end;
end;

procedure CmdSmakeItem(PlayObject: TPlayObject; Cmd: pTGameCmd; nWhere, nValueType, nValue: Integer);
var
  sShowMsg: string;
  StdItem: pTStdItem;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (nWhere in [Low(THumanUseItems) .. High(THumanUseItems)]) and (nValueType in [0 .. 15]) and (nValue in [0 .. 255]) then
    begin
      if (m_UseItems[nWhere].wIndex > 0) then
      begin
        StdItem := UserEngine.GetStdItem(m_UseItems[nWhere].wIndex);
        if StdItem = nil then
          Exit;

        if nValueType > 13 then
        begin
          nValue := Min(65, nValue);
          if nValueType = 14 then
            m_UseItems[nWhere].Dura := nValue * 1000;
          if nValueType = 15 then
            m_UseItems[nWhere].DuraMax := nValue * 1000;
        end
        else
          m_UseItems[nWhere].btValue[nValueType] := nValue;

        RecalcAbilitys();
        SendUpdateItem(@m_UseItems[nWhere]);
        sShowMsg := IntToStr(m_UseItems[nWhere].wIndex) + '-' + IntToStr(m_UseItems[nWhere].MakeIndex) + ' ' +
          IntToStr(m_UseItems[nWhere].Dura) + '/' + IntToStr(m_UseItems[nWhere].DuraMax) + ' ' +
          IntToStr(m_UseItems[nWhere].btValue[0]) + '/' + IntToStr(m_UseItems[nWhere].btValue[1]) + '/' +
          IntToStr(m_UseItems[nWhere].btValue[2]) + '/' + IntToStr(m_UseItems[nWhere].btValue[3]) + '/' +
          IntToStr(m_UseItems[nWhere].btValue[4]) + '/' + IntToStr(m_UseItems[nWhere].btValue[5]) + '/' +
          IntToStr(m_UseItems[nWhere].btValue[6]) + '/' + IntToStr(m_UseItems[nWhere].btValue[7]) + '/' +
          IntToStr(m_UseItems[nWhere].btValue[8]) + '/' + IntToStr(m_UseItems[nWhere].btValue[9]) + '/' +
          IntToStr(m_UseItems[nWhere].btValue[10]) + '/' + IntToStr(m_UseItems[nWhere].btValue[11]) + '/' +
          IntToStr(m_UseItems[nWhere].btValue[12]) + '/' + IntToStr(m_UseItems[nWhere].btValue[13]);
        SysMsg(sShowMsg, c_Blue, t_Hint);
        if g_Config.boShowMakeItemMsg then
          MainOutMessage('[物品调整] ' + m_sCharName + '(' + StdItem.Name + ' -> ' + sShowMsg + ')');
      end
      else
        SysMsg(g_sGamecommandSuperMakeHelpMsg, c_Red, t_Hint);
    end;
  end;
end;

procedure CmdSpirtStart(PlayObject: TPlayObject; sCmd, sParam1: string);
var
  nTime: Integer;
  dwTime: LongWord;
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg('此命令用于开始祈祷生效宝宝叛变。', c_Red, t_Hint);
      Exit;
    end;

    nTime := StrToIntDef(sParam1, -1);
    if nTime > 0 then
      dwTime := LongWord(nTime) * 1000
    else
      dwTime := g_Config.dwSpiritMutinyTime;

    g_dwSpiritMutinyTick := MyGetTickCount + dwTime;
    SysMsg('祈祷叛变已开始。持续时长 ' + IntToStr(dwTime div 1000) + ' 秒。', c_Green, t_Hint);
  end;
end;

procedure CmdSpirtStop(PlayObject: TPlayObject; sCmd, sParam1: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < 6) then
      Exit;

    if (sParam1 <> '') and (sParam1[1] = '?') then
    begin
      SysMsg('此命令用于停止祈祷生效导致宝宝叛变。', c_Red, t_Hint);
      Exit;
    end;

    g_dwSpiritMutinyTick := 0;
    SysMsg('祈祷叛变已停止。', c_Green, t_Hint);
  end;
end;

procedure CmdStartQuest(PlayObject: TPlayObject; Cmd: pTGameCmd; sQuestName: string);
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sQuestName = '') then
    begin
      SysMsg('命令格式: @' + Cmd.sCmd + ' 问答名称', c_Red, t_Hint);
      Exit;
    end;

    UserEngine.SendQuestMsg(sQuestName);
  end;
end;

procedure CmdSuperTing(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName, sRange: string);
var
  I: Integer;
  OnlineObject: TPlayObject;
  MoveHuman: TPlayObject;
  nRange: Integer;
  HumanList: TList;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sRange = '') or (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandSuperTingHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    nRange := Max(10, StrToIntDef(sRange, 2));
    OnlineObject := UserEngine.GetPlayObject(sHumanName);
    if OnlineObject <> nil then
    begin
      HumanList := TList.Create;
      UserEngine.GetMapRageHuman(OnlineObject.m_PEnvir, OnlineObject.m_nCurrX, OnlineObject.m_nCurrY, nRange, HumanList);
      for I := 0 to HumanList.Count - 1 do
      begin
        MoveHuman := TPlayObject(HumanList.Items[I]);
        if MoveHuman <> PlayObject then
          MoveHuman.MapRandomMove(MoveHuman.m_sHomeMap, 0);
      end;
      HumanList.Free;
    end
    else
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
  end;
end;

procedure CmdTakeOffHorse(PlayObject: TPlayObject; sCmd, sParam: string);
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('下马命令，在骑马状态输入此命令下马。', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s', [sCmd]), c_Red, t_Hint);
      Exit;
    end;

    if m_boOnHorse then
    begin
      if m_HorseOtherHum <> nil then
      begin
        { 骑马发起人下马，被邀请人也下马 chongchong 2013-10-14 }
        if m_boHorseMaster then
        begin
          m_HorseOtherHum.m_boOnHorse := False;
          m_HorseOtherHum.m_dwDownHorseTick := MyGetTickCount;
          m_HorseOtherHum.m_dwClientTakeHorseTick := MyGetTickCount;
        end;

        m_HorseOtherHum.m_HorseOtherHum := nil;
        m_HorseOtherHum.DoTalkStatusChanged;
        m_HorseOtherHum.TriggerHorseScript;
        if m_boHorseMaster then
          m_HorseOtherHum.SendTakeOffHorse;
      end;

      StopCollect;

      m_boOnHorse := False;
      m_dwClientTakeHorseTick := MyGetTickCount;
      m_dwDownHorseTick := MyGetTickCount;
      m_HorseOtherHum := nil;
      DoTalkStatusChanged;
      TriggerHorseScript;
      if not m_boHorseMaster then
        SendTakeOffHorse;
    end;
  end;
end;

procedure CmdTakeOnHorse(PlayObject: TPlayObject; sCmd, sParam: string);
begin
  with PlayObject do
  begin
    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg('上马命令，在戴好马牌后输入此命令就可以骑上马。', c_Red, t_Hint);
      SysMsg(Format('命令格式: @%s', [sCmd]), c_Red, t_Hint);
      Exit;
    end;

    if m_boShopStall then
    begin
      SysMsg(g_sHorseStopShop, c_Red, t_Hint);
      Exit;
    end;

    if m_boOnHorse then
      Exit;

    if (m_btHorseType = 0) then
    begin
      SysMsg(g_sHorseBrand, c_Red, t_Hint);
      Exit;
    end;

    m_nChangeAppr := -1;
    m_boOnHorse := True;
    m_boHorseMaster := True;
    m_HorseOtherHum := nil;
    FeatureChanged();
  end;
end;

procedure CmdTestFire(PlayObject: TPlayObject; sCmd: string; nRange, nType, nTime, nPoint: Integer);
var
  nX, nY: Integer;
  FireBurnEvent: TFireBurnEvent;
  nMinX, nMaxX, nMinY, nMaxY: Integer;
begin
  with PlayObject do
  begin
    nMinX := m_nCurrX - nRange;
    nMaxX := m_nCurrX + nRange;
    nMinY := m_nCurrY - nRange;
    nMaxY := m_nCurrY + nRange;
    for nX := nMinX to nMaxX do
    begin
      for nY := nMinY to nMaxY do
      begin
        if ((nX < nMaxX) and (nY = nMinY)) or ((nY < nMaxY) and (nX = nMinX)) or (nX = nMaxX) or (nY = nMaxY) then
        begin
          FireBurnEvent := TFireBurnEvent.Create(PlayObject, nX, nY, nType, nTime * 1000, nPoint);
          g_EventManager.AddEvent(FireBurnEvent);
        end;
      end;
    end;
  end;
end;

procedure CmdTestGetBagItems(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam: string);
var
  btDc, btSc, btMc, btDura: Byte;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sParam <> '') and (sParam[1] = '?') then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandTestGetBagItemsHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    btDc := 0;
    btSc := 0;
    btMc := 0;
    btDura := 0;
    GetBagUseItems(btDc, btSc, btMc, btDura);
    SysMsg(Format('DC:%d SC:%d MC:%d DURA:%d', [btDc, btSc, btMc, btDura]), c_Blue, t_Hint);
  end;
end;

procedure CmdGiveMine(PlayObject: TPlayObject; Cmd: pTGameCmd; sItemName: string; nCount, nDura: Integer);
var
  I: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if m_ItemList.Count >= GetMaxBagCount then
      Exit;

    if (sItemName <> '') and (nCount > 0) and (nDura > 0) then
    begin
      StdItem := UserEngine.GetStdItem(sItemName);
      if StdItem <> nil then
      begin
        for I := 0 to nCount - 1 do
        begin
          New(UserItem);
          if UserEngine.CopyToUserItemFromName(sItemName, UserItem) then
          begin

            if PlayObject.m_boFromGmExecute then
            begin
              UserItem.ItemFrom.ItemForm := ifScript;
              UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName;
              UserItem.ItemFrom.DateTime := Now();
            end
            else
            begin
              UserItem.ItemFrom.ItemForm := ifGM;
              UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName;
              UserItem.ItemFrom.DateTime := Now();
            end;

            UserItem.Dura := nDura * 1000;
            m_ItemList.Add(UserItem);
            WeightChanged();
            SendAddItem(UserItem);

            if (UserItem.boStartTime) then
            begin
              if StdItem.Need = 103 then
                SysMsg(Format('您的限时物品[%s]开始计时，有效时间%d分钟。', [StdItem.Name, StdItem.NeedLevel]), c_Red, t_System)
              else
              begin
                SysMsg(Format('您的限时物品[%s]开始计时，到期时间%s', [StdItem.Name, GetIncMinuteTime(UserItem.ItemFrom.DateTime,
                  StdItem.NeedLevel)]), c_Red, t_System);
              end;
            end;

            if g_Config.boShowMakeItemMsg and (m_btPermission >= 6) then
              MainOutMessage('[制造物品] ' + m_sCharName + ' ' + sItemName + '(' + IntToStr(UserItem.MakeIndex) + ')');

            if StdItem.NeedIdentify = 1 then
              AddGameDataLog(LOG_ItemMake, LOG_ActionNone, PlayObject, StdItem.Name, UserItem.MakeIndex, '0', 0, 0,
                '@' + Cmd.sCmd + ' ' + sItemName);

            if m_ItemList.Count >= GetMaxBagCount then
              Break;
          end
          else
            Dispose(UserItem);
        end;
      end;
    end;
  end;
end;

procedure CmdSendTopChatBoardMsg(PlayObject: TPlayObject; Cmd: pTGameCmd; sMsg: string); // 传音筒
var
  StdItem: pTStdItem;
  SC: string;
begin
  with PlayObject do
  begin
    if (g_FilterTexts <> nil) and (sMsg <> '') then
    begin
      // 检测用户输入是否有非法字符
      if g_FilterTexts.Filter(sMsg, SC) then
        sMsg := SC;

      if sMsg = '' then
        Exit;
    end;

    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if MyGetTickCount - m_dwUserItemSayMsgTick > g_Config.nUserItemSayMsgTime * 1000 then
    begin
      m_dwUserItemSayMsgTick := MyGetTickCount();

      if (m_UseItems[U_CHARM].wIndex > 0) and (m_UseItems[U_CHARM].Dura > 0) then
      begin
        StdItem := UserEngine.GetStdItem(m_UseItems[U_CHARM].wIndex);
        if (StdItem <> nil) and (StdItem.StdMode = 7) and (StdItem.Shape = 0) and (StdItem.AniCount > 0) then
        begin
          case StdItem.AniCount of
            1:
              UserEngine.SendBroadCastMsg(m_sCharName + ': ' + sMsg, g_Config.btUserSayMsgFColor,
                g_Config.btUserSayMsgBColor, t_Char);
            2:
              UserEngine.SendTopBroadCastMsg(m_sCharName + ': ' + sMsg, g_Config.btTopUserSayMsgFColor,
                g_Config.btTopUserSayMsgBColor, 30, t_Char);
          end;

          if m_UseItems[U_CHARM].Dura >= 1000 then
            Dec(m_UseItems[U_CHARM].Dura, 1000)
          else
            m_UseItems[U_CHARM].Dura := 0;

          if m_UseItems[U_CHARM].Dura <= 0 then
          begin
            SendDelItem(@m_UseItems[U_CHARM]);
            m_UseItems[U_CHARM].wIndex := 0;
            RecalcAbilitys();
          end
          else
            SendUpDateItemDura(U_CHARM, m_UseItems[U_CHARM].MakeIndex, False, m_UseItems[U_CHARM].Dura);
          Exit;
        end;
      end;

      if (m_UseItems[U_ARMRINGL].wIndex > 0) and (m_UseItems[U_ARMRINGL].Dura > 0) then
      begin // 1.76版
        StdItem := UserEngine.GetStdItem(m_UseItems[U_ARMRINGL].wIndex);
        if (StdItem <> nil) and (StdItem.StdMode = 7) and (StdItem.Shape = 0) and (StdItem.AniCount > 0) then
        begin
          case StdItem.AniCount of
            1:
              UserEngine.SendBroadCastMsg(m_sCharName + ': ' + sMsg, g_Config.btUserSayMsgFColor,
                g_Config.btUserSayMsgBColor, t_Char);
            2:
              UserEngine.SendTopBroadCastMsg(m_sCharName + ': ' + sMsg, g_Config.btTopUserSayMsgFColor,
                g_Config.btTopUserSayMsgBColor, 30, t_Char);
          end;

          if m_UseItems[U_ARMRINGL].Dura >= 1000 then
            Dec(m_UseItems[U_ARMRINGL].Dura, 1000)
          else
            m_UseItems[U_ARMRINGL].Dura := 0;

          if m_UseItems[U_ARMRINGL].Dura <= 0 then
          begin
            SendDelItem(@m_UseItems[U_ARMRINGL]);
            m_UseItems[U_ARMRINGL].wIndex := 0;
            RecalcAbilitys();
          end
          else
            SendUpDateItemDura(U_ARMRINGL, m_UseItems[U_ARMRINGL].MakeIndex, False, m_UseItems[U_ARMRINGL].Dura);
        end;
      end;
    end
    else
      SysMsg(Format('请在%d秒后使用', [(g_Config.nUserItemSayMsgTime * 1000 - (MyGetTickCount - m_dwUserItemSayMsgTick)) div 1000]),
        c_Red, t_Hint);
  end;
end;

procedure CmdShowEffect(PlayObject: TPlayObject; Cmd: pTGameCmd; sParam: string); // 烟花
var
  nEffectType: Integer;
  nTime: Integer;
  FlowerEvent: TFlowerEvent;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    nEffectType := StrToIntDef(sParam, -1);
    nTime := 10; // 烟花时间控制
    if nEffectType in [ET_FIREFLOWER_1 .. ET_FIREFLOWER_8] then
    begin
      FlowerEvent := TFlowerEvent.Create(m_PEnvir, m_nCurrX, m_nCurrY, nEffectType, nTime * 1000);
      g_EventManager.AddEvent(FlowerEvent);
    end;
  end;
end;

procedure CmdRestHero(PlayObject: TPlayObject);
begin
  if (PlayObject.m_MyHero <> nil) then
    THeroObject(PlayObject.m_MyHero).RestHero;
end;

procedure CmdTestSpeedMode(PlayObject: TPlayObject; Cmd: pTGameCmd);
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    m_boTestSpeedMode := not m_boTestSpeedMode;
    if m_boTestSpeedMode then
      SysMsg('开启速度测试模式', c_Red, t_Hint)
    else
      SysMsg('关闭速度测试模式', c_Red, t_Hint);
  end;
end;

procedure CmdTestStatus(PlayObject: TPlayObject; sCmd: string; nType, nTime: Integer; boShowMsg: Boolean);
begin
  with PlayObject do
  begin
    if m_btPermission < 6 then
      Exit;

    if (not(nType in [Low(TStatusTime) .. High(TStatusTime)])) or (nTime < 0) then
    begin
      SysMsg('命令格式: @' + sCmd + ' 类型(0..11) 时长', c_Red, t_Hint);
      Exit;
    end;

    if (nType = STATE_BUBBLEDEFENCEUP) and (nTime = 0) then
      m_boAbilMagBubbleDefence := False;

    m_wStatusTimeArr[nType] := nTime;
    m_dwStatusArrTick[nType] := MyGetTickCount();
    m_nCharStatus := GetCharStatus();
    StatusChanged();
    if boShowMsg then
      SysMsg(Format('状态编号:%d 时间长度: %d 秒', [nType, nTime]), c_Green, t_Hint);
  end;
end;

procedure CmdTing(PlayObject: TPlayObject; Cmd: pTGameCmd; sHumanName: string);
var
  OnlieObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sHumanName = '') or ((sHumanName <> '') and (sHumanName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandTingHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlieObject := UserEngine.GetPlayObject(sHumanName);
    if OnlieObject <> nil then
      OnlieObject.MapRandomMove(m_sHomeMap, 0)
    else
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sHumanName]), c_Red, t_Hint);
  end;
end;

procedure CmdTraining(PlayObject: TPlayObject; sSkillName: string; nLevel: Integer); // 004CC414
begin

end;

procedure CmdUserMoveXY(PlayObject: TPlayObject; sCmd, sX, sY: string);
var
  nX, nY: Integer;
  Castle: TUserCastle;
begin
  with PlayObject do
  begin
    if m_boTeleport then
    begin
      { TODO -opiaoyun -c修改 : 攻城区域禁止传送戒指 【2013-07-19】 }
      // if m_PEnvir.m_boNOSAFEPOSITIONMOVE and InSafeZone then
      Castle := g_CastleManager.InCastleWarArea(PlayObject);
      if m_PEnvir.m_boNOSAFEPOSITIONMOVE and InSafeZone or
        ((Castle <> nil) and (Castle.m_boUnderWar) and (g_Config.boWarDisTeleport)) then
      begin
        SysMsg('此区域禁止使用此命令！', c_Red, t_Hint);
        Exit;
      end;
      nX := StrToIntDef(sX, -1);
      nY := StrToIntDef(sY, -1);
      {
        if (nX < 0) or (nY < 0) then begin
        SysMsg('命令格式: @' + sCMD + ' 座标X 座标Y',c_Red,t_Hint);
        exit;
        end;
      }

      if m_boImprison then
      begin
        SysMsg('禁止使用此命令！', c_Red, t_Hint);
        Exit;
      end;

      if g_Config.boDisableMoveParalysisHuman and (m_wStatusTimeArr[POISON_STONE] <> 0) then
      begin
        SysMsg('麻痹状态不允许随机传送。', g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
        Exit;
      end;

      if not m_PEnvir.m_boNOPOSITIONMOVE then
      begin
        if m_PEnvir.CanWalkOfItem(nX, nY, g_Config.boUserMoveCanDupObj, g_Config.boUserMoveCanOnItem) then
        begin
          Castle := g_CastleManager.InCastleWarArea(m_PEnvir, nX, nY);
          if ((Castle <> nil) and (Castle.m_boUnderWar) and (g_Config.boWarDisTeleport)) then
          begin
            SysMsg('此区域禁止使用此命令！', c_Red, t_Hint);
            Exit;
          end;

          if (MyGetTickCount - m_dwTeleportTick) > g_Config.dwUserMoveTime * 1000 { 10000 } then
          begin
            m_boStopTeleport := False;

            if g_FunctionNPC <> nil then
            begin
              m_nScriptGotoCount := 0;
              g_FunctionNPC.GotoLable(PlayObject, '@BeginTeleport', False);
            end;

            if not m_boStopTeleport then
            begin
              m_dwTeleportTick := MyGetTickCount();
              if m_TeleportUserItem <> nil then
              begin // 传送符
                if m_TeleportUserItem.Dura >= 100 then
                  Dec(m_TeleportUserItem.Dura, 100)
                else
                  m_TeleportUserItem.Dura := 0;

                if m_TeleportUserItem.Dura > 0 then
                begin
                  SendUpdateItem(m_TeleportUserItem);
                end
                else
                begin
                  SendDelItem(m_TeleportUserItem);
                  m_TeleportUserItem.wIndex := 0;
                  m_TeleportUserItem := nil;
                  RecalcAbilitys;
                end;
              end;
              SendRefMsg(RM_SPACEMOVE_FIRE, 0, 0, 0, 0, '');

              if (Length(sX) = 0) and (Length(sY) = 0) then
                MapRandomMove(m_sMapName, 0)
              else
                SpaceMove(m_sMapName, nX, nY, 0);
            end;
          end
          else
            SysMsg(IntToStr(g_Config.dwUserMoveTime - (MyGetTickCount - m_dwTeleportTick) div 1000) + '秒之后才可以再使用此功能！',
              c_Red, t_Hint);
        end
        else
          SysMsg(Format(g_sGameCommandPositionMoveCanotMoveToMap, [m_sMapName, sX, sY]), c_Green, t_Hint);
      end
      else
        SysMsg('此地图禁止使用此命令！', c_Red, t_Hint);
    end
    else
      SysMsg('您现在还无法使用此功能！', c_Red, t_Hint);
  end;
end;

procedure CmdViewDiary(PlayObject: TPlayObject; sCmd: string; nFlag: Integer);
begin
end;

procedure CmdViewWhisper(PlayObject: TPlayObject; Cmd: pTGameCmd; sCharName, sParam2: string);
var
  OnlineObject: TPlayObject;
begin
  with PlayObject do
  begin
    if (m_btPermission < Cmd.nPermissionMin) then
    begin
      SysMsg(g_sGameCommandPermissionTooLow, c_Red, t_Hint);
      Exit;
    end;

    if (sCharName = '') or ((sCharName <> '') and (sCharName[1] = '?')) then
    begin
      SysMsg(Format(g_sGameCommandParamUnKnow, [Cmd.sCmd, g_sGameCommandViewWhisperHelpMsg]), c_Red, t_Hint);
      Exit;
    end;

    OnlineObject := UserEngine.GetPlayObject(sCharName);
    if OnlineObject <> nil then
    begin
      if OnlineObject.m_GetWhisperHuman = PlayObject then
      begin
        OnlineObject.m_GetWhisperHuman := nil;
        SysMsg(Format(g_sGameCommandViewWhisperMsg1, [sCharName]), c_Green, t_Hint);
      end
      else
      begin
        OnlineObject.m_GetWhisperHuman := PlayObject;
        SysMsg(Format(g_sGameCommandViewWhisperMsg2, [sCharName]), c_Green, t_Hint);
      end;
    end
    else
      SysMsg(Format(g_sNowNotOnLineOrOnOtherServer, [sCharName]), c_Red, t_Hint);
  end;
end;

procedure CmdShowMapInfoEx(PlayObject: TPlayObject; sMAP, sX, sY: string);
var
  Map: TEnvirnoment;
  nX, nY: Integer;
  MapCellInfo: pTMapCellinfo;
begin
  with PlayObject do
  begin
    nX := StrToIntDef(sX, 0);
    nY := StrToIntDef(sY, 0);

    if (sMAP <> '') and (nX >= 0) and (nY >= 0) then
    begin
      Map := g_MapManager.FindMap(sMAP);
      if Map <> nil then
      begin
        if Map.GetMapCellInfo(nX, nY, MapCellInfo) then
        begin
          SysMsg('标志: ' + IntToStr(MapCellInfo.chFlag), c_Green, t_Hint);
{$IF USEOBJLIST = 1}
          SysMsg('对象数: ' + IntToStr(Length(MapCellInfo.ObjList)), c_Green, t_Hint);
{$ELSE}
          if MapCellInfo.ObjList <> nil then
            SysMsg('对象数: ' + IntToStr(MapCellInfo.ObjList.Count), c_Green, t_Hint);
{$IFEND}
        end
        else
          SysMsg('取地图单元信息失败: ' + sMAP, c_Red, t_Hint);
      end;
    end
    else
      SysMsg('请按正确格式输入: ' + g_GameCommand.MAPINFO.sCmd + ' 地图号 X Y', c_Green, t_Hint);
  end;
end;

procedure ProcessUserLineMsg(PlayObject: TPlayObject; sData: string);
var
  SC, sCmd, sParam1, sParam2, sParam3, sParam4, sParam5, sParam6, sParam7: string;
  OnlineObject: TPlayObject;
  nFlag: Integer;
  nValue: Integer;
  nLen: Integer;
  sTemp: string;
  // 千里传音空格被截断，特殊处理 piaoyun 2013-08-17
resourcestring
  sExceptionMsg = '[Exception] TPlayObject.ProcessUserLineMsg Msg = %s';
begin
  { PlayObject.m_sInputParam0 := '';
    PlayObject.m_sInputParam1 := '';
    PlayObject.m_sInputParam2 := '';
    PlayObject.m_sInputParam3 := '';
    PlayObject.m_sInputParam4 := '';
    PlayObject.m_sInputParam5 := '';
    PlayObject.m_sInputParam6 := ''; }
  try
    nLen := Length(sData);
    // if nLen > 255 then sData := Copy(sData, 1, 255);
    if sData = '' then
      Exit;

    with PlayObject do
    begin
      if m_boSetStoragePwd then
      begin
        m_boSetStoragePwd := False;
        if (nLen > 3) and (nLen < 8) then
        begin
          m_sTempPwd := sData;
          m_boReConfigPwd := True;
          SysMsg(g_sReSetPasswordMsg, c_Green, t_Hint);
          { '请重复输入一次仓库密码：' }
          SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
        end
        else
        begin
          SysMsg(g_sPasswordOverLongMsg, c_Red, t_Hint);
          { '输入的密码长度不正确！，密码长度必须在 4 - 7 的范围内，请重新设置密码。' }
        end;
        Exit;
      end;

      if m_boReConfigPwd then
      begin
        m_boReConfigPwd := False;
        if CompareStr(m_sTempPwd, sData) = 0 then
        begin
          m_sStoragePwd := sData;
          m_boPasswordLocked := True;
          m_boCanGetBackItem := False;
          m_sTempPwd := '';
          SysMsg(g_sReSetPasswordOKMsg, c_Blue, t_Hint);
          { '密码设置成功！！，仓库已经自动上锁，请记好您的仓库密码，在取仓库时需要使用此密码开锁。' }
        end
        else
        begin
          m_sTempPwd := '';
          SysMsg(g_sReSetPasswordNotMatchMsg, c_Red, t_Hint);
        end;
        Exit;
      end;

      if m_boUnLockPwd or m_boUnLockStoragePwd then
      begin
        if CompareStr(m_sStoragePwd, sData) = 0 then
        begin
          m_boPasswordLocked := False;
          if m_boUnLockPwd then
          begin
            if g_Config.boLockDealAction then
              m_boCanDeal := True;
            if g_Config.boLockDropAction then
              m_boCanDrop := True;
            if g_Config.boLockWalkAction then
              m_boCanWalk := True;
            if g_Config.boLockRunAction then
              m_boCanRun := True;
            if g_Config.boLockHitAction then
              m_boCanHit := True;
            if g_Config.boLockSpellAction then
              m_boCanSpell := True;
            if g_Config.boLockSendMsgAction then
              m_boCanSendMsg := True;
            if g_Config.boLockUserItemAction then
              m_boCanUseItem := True;
            if g_Config.boLockInObModeAction then
            begin
              m_boObMode := False;
              m_boAdminMode := False;
            end;
            m_boLockLogoned := True;
            SysMsg(g_sPasswordUnLockOKMsg, c_Blue, t_Hint);
          end;
          if m_boUnLockStoragePwd then
          begin
            if g_Config.boLockGetBackItemAction then
              m_boCanGetBackItem := True;
            SysMsg(g_sStorageUnLockOKMsg, c_Blue, t_Hint);
          end;
        end
        else
        begin
          Inc(m_btPwdFailCount);
          SysMsg(g_sUnLockPasswordFailMsg, c_Red, t_Hint);
          if m_btPwdFailCount > 3 then
          begin
            SysMsg(g_sStoragePasswordLockedMsg, c_Red, t_Hint);
          end;
        end;
        m_boUnLockPwd := False;
        m_boUnLockStoragePwd := False;
        Exit;
      end;

      if m_boCheckOldPwd then
      begin
        m_boCheckOldPwd := False;
        if m_sStoragePwd = sData then
        begin
          SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
          SysMsg(g_sSetPasswordMsg, c_Green, t_Hint);
          m_boSetStoragePwd := True;
        end
        else
        begin
          Inc(m_btPwdFailCount);
          SysMsg(g_sOldPasswordIncorrectMsg, c_Red, t_Hint);
          if m_btPwdFailCount > 3 then
          begin
            SysMsg(g_sStoragePasswordLockedMsg, c_Red, t_Hint);
            m_boPasswordLocked := True;
          end;
        end;
        Exit;
      end;

      if sData[1] <> '@' then
      begin
        ProcessSayMsg(sData);
        Exit;
      end;
      SC := Copy(sData, 2, Length(sData) - 1);
      SC := GetValidStr3(SC, sCmd, [' ', ':', ',', #9]);
      sTemp := SC;
      // 处理千里传音空格被截断问题 piaoyun 2013-08-17
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam1, [' ', ':', ',', #9]);
      end;
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam2, [' ', ':', ',', #9]);
      end;
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam3, [' ', ':', ',', #9]);
      end;
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam4, [' ', ':', ',', #9]);
      end;
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam5, [' ', ':', ',', #9]);
      end;
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam6, [' ', ':', ',', #9]);
      end;
      if SC <> '' then
      begin
        SC := GetValidStr3(SC, sParam7, [' ', ':', ',', #9]);
      end;

      g_sInputParam0 := sCmd;
      g_sInputParam1 := sParam1;
      g_sInputParam2 := sParam2;
      g_sInputParam3 := sParam3;
      g_sInputParam4 := sParam4;
      g_sInputParam5 := sParam5;
      g_sInputParam6 := sParam6;
      g_sInputParams := sTemp;

      if CompareText(sCmd, 'TestMode') = 0 then
      begin
        m_boTestMode := not m_boTestMode;
        SysMsg('TestMode=' + BoolToStr(m_boTestMode), c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, 'ShowMapObject') = 0 then
      begin
        m_PEnvir.ShowMapObject(StrToIntDef(sParam1, 0), StrToIntDef(sParam2, 0), PlayObject);
        Exit;
      end;

      // 新密码命令 BEGIN
      if CompareText(sCmd, g_GameCommand.PASSWORDLOCK.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;
        if m_sStoragePwd = '' then
        begin
          SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
          m_boSetStoragePwd := True;
          SysMsg(g_sSetPasswordMsg, c_Green, t_Hint);
          Exit;
        end;
        if m_btPwdFailCount > 3 then
        begin
          SysMsg(g_sStoragePasswordLockedMsg, c_Red, t_Hint);
          m_boPasswordLocked := True;
          Exit;
        end;
        if m_sStoragePwd <> '' then
        begin
          SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
          m_boCheckOldPwd := True;
          SysMsg(g_sPleaseInputOldPasswordMsg, c_Green, t_Hint);
          Exit;
        end;
        Exit;
      end;
      // 新密码命令 END

      if CompareText(sCmd, g_GameCommand.SETPASSWORD.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;

        if m_sStoragePwd = '' then
        begin
          SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
          m_boSetStoragePwd := True;
          SysMsg(g_sSetPasswordMsg, c_Green, t_Hint);
        end
        else
          SysMsg(g_sAlreadySetPasswordMsg, c_Red, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.UNPASSWORD.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;

        if not m_boPasswordLocked then
        begin
          m_sStoragePwd := '';
          SysMsg(g_sOldPasswordIsClearMsg, c_Green, t_Hint);
        end
        else
          SysMsg(g_sPleaseUnLockPasswordMsg, c_Red, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHGPASSWORD.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;

        if m_btPwdFailCount > 3 then
        begin
          SysMsg(g_sStoragePasswordLockedMsg, c_Red, t_Hint);
          m_boPasswordLocked := True;
          Exit;
        end;

        if m_sStoragePwd <> '' then
        begin
          SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
          m_boCheckOldPwd := True;
          SysMsg(g_sPleaseInputOldPasswordMsg, c_Green, t_Hint);
        end
        else
          SysMsg(g_sNoPasswordSetMsg, c_Red, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.UNLOCKSTORAGE.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;

        if m_btPwdFailCount > g_Config.nPasswordErrorCountLock { 3 } then
        begin
          SysMsg(g_sStoragePasswordLockedMsg, c_Red, t_Hint);
          m_boPasswordLocked := True;
          Exit;
        end;

        if m_sStoragePwd <> '' then
        begin
          if not m_boUnLockStoragePwd then
          begin
            SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
            SysMsg(g_sPleaseInputUnLockPasswordMsg, c_Green, t_Hint);
            m_boUnLockStoragePwd := True;
          end
          else
            SysMsg(g_sStorageAlreadyUnLockMsg, c_Red, t_Hint);
        end
        else
          SysMsg(g_sStorageNoPasswordMsg, c_Red, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.UnLock.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;

        if m_btPwdFailCount > g_Config.nPasswordErrorCountLock { 3 } then
        begin
          SysMsg(g_sStoragePasswordLockedMsg, c_Red, t_Hint);
          m_boPasswordLocked := True;
          Exit;
        end;

        if m_sStoragePwd <> '' then
        begin
          if not m_boUnLockPwd then
          begin
            SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
            SysMsg(g_sPleaseInputUnLockPasswordMsg, c_Green, t_Hint);
            m_boUnLockPwd := True;
          end
          else
            SysMsg(g_sStorageAlreadyUnLockMsg, c_Red, t_Hint);
        end
        else
          SysMsg(g_sStorageNoPasswordMsg, c_Red, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Lock.sCmd) = 0 then
      begin
        if not g_Config.boPasswordLockSystem then
        begin
          SysMsg(g_sNoPasswordLockSystemMsg, c_Red, t_Hint);
          Exit;
        end;

        if not m_boPasswordLocked then
        begin
          if m_sStoragePwd <> '' then
          begin
            m_boPasswordLocked := True;
            m_boCanGetBackItem := False;
            SysMsg(g_sLockStorageSuccessMsg, c_Green, t_Hint);
          end
          else
            SysMsg(g_sStorageNoPasswordMsg, c_Green, t_Hint);
        end
        else
          SysMsg(g_sStorageAlreadyLockMsg, c_Red, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MEMBERFUNCTION.sCmd) = 0 then
      begin
        CmdMemberFunction(PlayObject, g_GameCommand.MEMBERFUNCTION.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MEMBERFUNCTIONEX.sCmd) = 0 then
      begin
        CmdMemberFunctionEx(PlayObject, g_GameCommand.MEMBERFUNCTIONEX.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.REMTEMSG.sCmd) = 0 then
      begin
        m_boRemoteMsg := True;
        SysMsg('允许接受消息。', c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DEAR.sCmd) = 0 then
      begin
        CmdSearchDear(PlayObject, g_GameCommand.DEAR.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MASTER.sCmd) = 0 then
      begin
        CmdSearchMaster(PlayObject, g_GameCommand.MASTER.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MASTERECALL.sCmd) = 0 then
      begin
        CmdMasterRecall(PlayObject, g_GameCommand.MASTERECALL.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DEARRECALL.sCmd) = 0 then
      begin
        CmdDearRecall(PlayObject, g_GameCommand.DEARRECALL.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ALLOWDEARRCALL.sCmd) = 0 then
      begin
        m_boCanDearRecall := not m_boCanDearRecall;
        if m_boCanDearRecall then
          SysMsg(g_sEnableDearRecall { '允许夫妻传送！' } , c_Blue, t_Hint)
        else
          SysMsg(g_sDisableDearRecall { '禁止夫妻传送！' } , c_Blue, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ALLOWMASTERRECALL.sCmd) = 0 then
      begin
        m_boCanMasterRecall := not m_boCanMasterRecall;
        if m_boCanMasterRecall then
          SysMsg(g_sEnableMasterRecall { '允许师徒传送！' } , c_Blue, t_Hint)
        else
          SysMsg(g_sDisableMasterRecall { '禁止师徒传送！' } , c_Blue, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Data.sCmd) = 0 then
      begin
        SysMsg(g_sNowCurrDateTime { '当前日期时间: ' } + FormatDateTime('dddddd,dddd,hh:mm:nn', Now), c_Blue, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.PRVMSG.sCmd) = 0 then
      begin
        CmdPrvMsg(PlayObject, g_GameCommand.PRVMSG.sCmd, g_GameCommand.PRVMSG.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ALLOWMSG.sCmd) = 0 then
      begin
        m_boHearWhisper := not m_boHearWhisper;
        if m_boHearWhisper then
          SysMsg(g_sEnableHearWhisper { '[允许私聊]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableHearWhisper { '[禁止私聊]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LETSHOUT.sCmd) = 0 then
      begin
        m_boBanShout := not m_boBanShout;
        if m_boBanShout then
          SysMsg(g_sEnableShoutMsg { '[允许组队聊天]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableShoutMsg { '[拒绝组队聊天]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LETTRADE.sCmd) = 0 then
      begin
        m_boAllowDeal := not m_boAllowDeal;
        if m_boAllowDeal then
          SysMsg(g_sEnableDealMsg { '[允许交易]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableDealMsg { '[禁止交易]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LETGUILD.sCmd) = 0 then
      begin
        m_boAllowGuild := not m_boAllowGuild;
        if m_boAllowGuild then
          SysMsg(g_sEnableJoinGuild { '[允许加入行会]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableJoinGuild { '[禁止加入行会]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ENDGUILD.sCmd) = 0 then
      begin
        CmdEndGuild(PlayObject);
        Exit;
      end;

      // 允许/禁止挑战 piaoyun 2013-07-22
      if CompareText(sCmd, g_GameCommand.LETCHALLENGE.sCmd) = 0 then
      begin
        m_boAllowChallenge := not m_boAllowChallenge;
        if m_boAllowChallenge then
          SysMsg(g_sEnableAllowChallenge { '[允许挑战]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisablAllowChallenge { '[禁止挑战]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ENDGUILD.sCmd) = 0 then
      begin
        CmdEndGuild(PlayObject);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.BANGUILDCHAT.sCmd) = 0 then
      begin
        m_boBanGuildChat := not m_boBanGuildChat;
        if m_boBanGuildChat then
          SysMsg(g_sEnableGuildChat { '[允许行会聊天]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableGuildChat { '[禁止行会聊天]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.AUTHALLY.sCmd) = 0 then
      begin
        if IsGuildMaster then
        begin
          TGUild(m_MyGuild).m_boEnableAuthAlly := not TGUild(m_MyGuild).m_boEnableAuthAlly;
          if TGUild(m_MyGuild).m_boEnableAuthAlly then
            SysMsg(g_sEnableAuthAllyGuild { '[允许行会联盟]' } , c_Green, t_Hint)
          else
            SysMsg(g_sDisableAuthAllyGuild { '[禁止行会联盟]' } , c_Green, t_Hint);
        end;
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ALLOWGROUPCALL.sCmd) = 0 then
      begin
        CmdAllowGroupReCall(PlayObject, sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.GROUPRECALLL.sCmd) = 0 then
      begin
        CmdGroupRecall(PlayObject, g_GameCommand.GROUPRECALLL.sCmd);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ALLOWGUILDRECALL.sCmd) = 0 then
      begin
        m_boAllowGuildReCall := not m_boAllowGuildReCall;
        if m_boAllowGuildReCall then
          SysMsg(g_sEnableGuildRecall { '[允许行会合一]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableGuildRecall { '[禁止行会合一]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.GUILDRECALLL.sCmd) = 0 then
      begin
        CmdGuildRecall(PlayObject, g_GameCommand.GUILDRECALLL.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.AUTH.sCmd) = 0 then
      begin
        if IsGuildMaster then
          ClientGuildAlly();
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.AUTHCANCEL.sCmd) = 0 then
      begin
        if IsGuildMaster then
          ClientGuildBreakAlly(sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DIARY.sCmd) = 0 then
      begin
        CmdViewDiary(PlayObject, g_GameCommand.DIARY.sCmd, StrToIntDef(sParam1, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ATTACKMODE.sCmd) = 0 then
      begin
        CmdChangeAttackMode(PlayObject, StrToIntDef(sParam1, -1), sParam1, sParam2, sParam3, sParam4, sParam5, sParam6, sParam7);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.REST.sCmd) = 0 then
      begin
        CmdChangeSalveStatus(PlayObject);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.TAKEONHORSE.sCmd) = 0 then
      begin
        CmdTakeOnHorse(PlayObject, sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.TAKEOFHORSE.sCmd) = 0 then
      begin
        CmdTakeOffHorse(PlayObject, sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DISABLEHORSEINVITE.sCmd) = 0 then
      begin
        m_boDisableHorseInvite := not m_boDisableHorseInvite;
        if m_boDisableHorseInvite then
          SysMsg(g_sDisableHorseInviteMsg { '[禁止邀请上马]' } , c_Green, t_Hint)
        else
          SysMsg(g_sEnableHorseInviteMsg { '[允许邀请上马]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.PublicChat.sCmd) = 0 then
      begin
        SendDefMessage(SM_DISABLE_PUBLICCHAT, 0, 0, 0, 0, '');
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CryChat.sCmd) = 0 then
      begin
        SendDefMessage(SM_DISABLE_CRYCHAT, 0, 0, 0, 0, '');
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.OpenSellPlayer.sCmd) = 0 then
      begin
        m_boEnableSellPlayerRequest := not m_boEnableSellPlayerRequest;
        if m_boEnableSellPlayerRequest then
          SysMsg(g_sSellPlayerEnabled, c_Green, t_Hint)
        else
          SysMsg(g_sSellPlayerDisable, c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.TESTGA.sCmd) = 0 then
      begin
        Exit;
        SendMsg(PlayObject, RM_PASSWORD, 0, 0, 0, 0, '');
        m_boTestGa := True;
        SysMsg(g_sPleaseInputPassword { '请输入密码:' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MAPINFO.sCmd) = 0 then
      begin
        CmdShowMapInfoEx(PlayObject, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CLEARBAG.sCmd) = 0 then
      begin
        CmdClearBagItem(PlayObject, @g_GameCommand.CLEARBAG, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWUSEITEMINFO.sCmd) = 0 then
      begin
        CmdShowUseItemInfo(PlayObject, @g_GameCommand.SHOWUSEITEMINFO, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.BINDUSEITEM.sCmd) = 0 then
      begin
        CmdBindUseItem(PlayObject, @g_GameCommand.BINDUSEITEM, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SBKDOOR.sCmd) = 0 then
      begin
        CmdSbkDoorControl(PlayObject, g_GameCommand.SBKDOOR.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.USERMOVE.sCmd) = 0 then
      begin
        CmdUserMoveXY(PlayObject, g_GameCommand.USERMOVE.sCmd, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SEARCHING.sCmd) = 0 then
      begin
        CmdSearchHuman(PlayObject, g_GameCommand.SEARCHING.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LOCKLOGON.sCmd) = 0 then
      begin
        CmdLockLogin(PlayObject, @g_GameCommand.LOCKLOGON);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.BANNATIONCHAT.sCmd) = 0 then
      begin
        m_boBanNationChat := not m_boBanNationChat;
        if m_boBanNationChat then
          SysMsg(g_sEnableNationChat { '[允许国家聊天]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableNationChat { '[禁止国家聊天]' } , c_Green, t_Hint);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LETNATION.sCmd) = 0 then
      begin
        m_boAllowNation := not m_boAllowNation;
        if m_boAllowNation then
          SysMsg(g_sEnableJoinNation { '[允许加入国家]' } , c_Green, t_Hint)
        else
          SysMsg(g_sDisableJoinNation { '[禁止加入国家]' } , c_Green, t_Hint);
        Exit;
      end;

      if (m_btPermission >= 2) and (Length(sData) > 2) then
      begin
        // if sData[2] = '!' then begin
        if (m_btPermission >= 6) and (sData[2] = g_GMRedMsgCmd) then
        begin
          if MyGetTickCount - m_dwSayMsgTick > 2000 then
          begin
            m_dwSayMsgTick := MyGetTickCount();
            sData := Copy(sData, 3, Length(sData) - 2);
            if Length(sData) > g_Config.nSayRedMsgMaxLen then
              sData := Copy(sData, 1, g_Config.nSayRedMsgMaxLen);

            if g_Config.boShutRedMsgShowGMName then
              SC := m_sCharName + ': ' + sData
            else
              SC := sData;
            UserEngine.SendBroadCastMsg(SC, t_GM);
          end;
          Exit;
        end;
      end;

      if CompareText(sCmd, g_GameCommand.HUMANLOCAL.sCmd) = 0 then
      begin
        CmdHumanLocal(PlayObject, @g_GameCommand.HUMANLOCAL, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Move.sCmd) = 0 then
      begin
        CmdMapMove(PlayObject, @g_GameCommand.Move, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.POSITIONMOVE.sCmd) = 0 then
      begin
        CmdPositionMove(PlayObject, @g_GameCommand.POSITIONMOVE, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.INFO.sCmd) = 0 then
      begin
        CmdHumanInfo(PlayObject, @g_GameCommand.INFO, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MOBLEVEL.sCmd) = 0 then
      begin
        CmdMobLevel(PlayObject, @g_GameCommand.MOBLEVEL, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MOBCOUNT.sCmd) = 0 then
      begin
        CmdMobCount(PlayObject, @g_GameCommand.MOBCOUNT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.HUMANCOUNT.sCmd) = 0 then
      begin
        CmdHumanCount(PlayObject, @g_GameCommand.HUMANCOUNT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.KICK.sCmd) = 0 then
      begin
        CmdKickHuman(PlayObject, @g_GameCommand.KICK, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.TING.sCmd) = 0 then
      begin
        CmdTing(PlayObject, @g_GameCommand.TING, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SUPERTING.sCmd) = 0 then
      begin
        CmdSuperTing(PlayObject, @g_GameCommand.SUPERTING, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MAPMOVE.sCmd) = 0 then
      begin
        CmdMapMoveHuman(PlayObject, @g_GameCommand.MAPMOVE, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHUTUP.sCmd) = 0 then
      begin
        CmdShutup(PlayObject, @g_GameCommand.SHUTUP, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Map.sCmd) = 0 then
      begin
        CmdShowMapInfo(PlayObject, @g_GameCommand.Map, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RELEASESHUTUP.sCmd) = 0 then
      begin
        CmdShutupRelease(PlayObject, @g_GameCommand.RELEASESHUTUP, sParam1, True);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHUTUPLIST.sCmd) = 0 then
      begin
        CmdShutupList(PlayObject, @g_GameCommand.SHUTUPLIST, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.GAMEMASTER.sCmd) = 0 then
      begin
        CmdChangeAdminMode(PlayObject, g_GameCommand.GAMEMASTER.sCmd, g_GameCommand.GAMEMASTER.nPermissionMin, sParam1,
          not m_boAdminMode);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.OBSERVER.sCmd) = 0 then
      begin
        CmdChangeObMode(PlayObject, g_GameCommand.OBSERVER.sCmd, g_GameCommand.OBSERVER.nPermissionMin, sParam1, not m_boObMode);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SUEPRMAN.sCmd) = 0 then
      begin
        CmdChangeSuperManMode(PlayObject, g_GameCommand.OBSERVER.sCmd, g_GameCommand.OBSERVER.nPermissionMin, sParam1,
          not m_boSuperMan);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Level.sCmd) = 0 then
      begin
        CmdChangeLevel(PlayObject, @g_GameCommand.Level, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SABUKWALLGOLD.sCmd) = 0 then
      begin
        CmdShowSbkGold(PlayObject, @g_GameCommand.SABUKWALLGOLD, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RECALL.sCmd) = 0 then
      begin
        CmdRecallHuman(PlayObject, @g_GameCommand.RECALL, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.REGOTO.sCmd) = 0 then
      begin
        CmdReGotoHuman(PlayObject, @g_GameCommand.REGOTO, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWFLAG.sCmd) = 0 then
      begin
        CmdShowHumanFlag(PlayObject, g_GameCommand.SHOWFLAG.sCmd, g_GameCommand.SHOWFLAG.nPermissionMin, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWOPEN.sCmd) = 0 then
      begin
        CmdShowHumanUnitOpen(PlayObject, g_GameCommand.SHOWOPEN.sCmd, g_GameCommand.SHOWOPEN.nPermissionMin, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWUNIT.sCmd) = 0 then
      begin
        CmdShowHumanUnit(PlayObject, g_GameCommand.SHOWUNIT.sCmd, g_GameCommand.SHOWUNIT.nPermissionMin, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Attack.sCmd) = 0 then
      begin
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MOB.sCmd) = 0 then
      begin
        CmdMob(PlayObject, @g_GameCommand.MOB, sParam1, StrToIntDef(sParam2, 0), StrToIntDef(sParam3, 0), sParam4,
          StrToIntDef(sParam5, 0) <> 0, StrToIntDef(sParam6, 0) <> 0, sParam7);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MOBNPC.sCmd) = 0 then
      begin
        CmdMobNpc(PlayObject, g_GameCommand.MOBNPC.sCmd, g_GameCommand.MOBNPC.nPermissionMin, sParam1, sParam2, sParam3, sParam4);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.NPCSCRIPT.sCmd) = 0 then
      begin
        CmdNpcScript(PlayObject, g_GameCommand.NPCSCRIPT.sCmd, g_GameCommand.NPCSCRIPT.nPermissionMin, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELNPC.sCmd) = 0 then
      begin
        CmdDelNpc(PlayObject, g_GameCommand.DELNPC.sCmd, g_GameCommand.DELNPC.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RECALLMOB.sCmd) = 0 then
      begin
        CmdRecallMob(PlayObject, @g_GameCommand.RECALLMOB, sParam1, StrToIntDef(sParam2, 0), StrToIntDef(sParam3, 0),
          StrToIntDef(sParam4, 0), StrToIntDef(sParam5, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LUCKYPOINT.sCmd) = 0 then
      begin
        CmdLuckPoint(PlayObject, g_GameCommand.LUCKYPOINT.sCmd, g_GameCommand.LUCKYPOINT.nPermissionMin, sParam1,
          sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.LOTTERYTICKET.sCmd) = 0 then
      begin
        CmdLotteryTicket(PlayObject, g_GameCommand.LOTTERYTICKET.sCmd, g_GameCommand.LOTTERYTICKET.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RELOADGUILD.sCmd) = 0 then
      begin
        CmdReloadGuild(PlayObject, g_GameCommand.RELOADGUILD.sCmd, g_GameCommand.RELOADGUILD.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RELOADLINENOTICE.sCmd) = 0 then
      begin
        CmdReloadLineNotice(PlayObject, g_GameCommand.RELOADLINENOTICE.sCmd,
          g_GameCommand.RELOADLINENOTICE.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RELOADABUSE.sCmd) = 0 then
      begin
        CmdReloadAbuse(PlayObject, g_GameCommand.RELOADABUSE.sCmd, g_GameCommand.RELOADABUSE.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.FREEPENALTY.sCmd) = 0 then
      begin
        CmdFreePenalty(PlayObject, @g_GameCommand.FREEPENALTY, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.PKPOINT.sCmd) = 0 then
      begin
        CmdPKpoint(PlayObject, @g_GameCommand.PKPOINT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.IncPkPoint.sCmd) = 0 then
      begin
        CmdIncPkPoint(PlayObject, @g_GameCommand.IncPkPoint, sParam1, StrToIntDef(sParam2, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MAKE.sCmd) = 0 then
      begin
        CmdMakeItem(PlayObject, @g_GameCommand.MAKE, sParam1, StrToIntDef(sParam2, 0), sParam3, sParam4);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.VIEWWHISPER.sCmd) = 0 then
      begin
        CmdViewWhisper(PlayObject, @g_GameCommand.VIEWWHISPER, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ReAlive.sCmd) = 0 then
      begin
        CmdReAlive(PlayObject, @g_GameCommand.ReAlive, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.KILL.sCmd) = 0 then
      begin
        CmdKill(PlayObject, @g_GameCommand.KILL, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SMAKE.sCmd) = 0 then
      begin
        CmdSmakeItem(PlayObject, @g_GameCommand.SMAKE, StrToIntDef(sParam1, 0), StrToIntDef(sParam2, 0), StrToIntDef(sParam3, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEJOB.sCmd) = 0 then
      begin
        CmdChangeJob(PlayObject, @g_GameCommand.CHANGEJOB, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEGENDER.sCmd) = 0 then
      begin
        CmdChangeGender(PlayObject, @g_GameCommand.CHANGEGENDER, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.HAIR.sCmd) = 0 then
      begin
        CmdHair(PlayObject, @g_GameCommand.HAIR, sParam1, StrToIntDef(sParam2, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.BonusPoint.sCmd) = 0 then
      begin
        CmdBonuPoint(PlayObject, @g_GameCommand.BonusPoint, sParam1, StrToIntDef(sParam2, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELBONUSPOINT.sCmd) = 0 then
      begin
        CmdDelBonuPoint(PlayObject, @g_GameCommand.DELBONUSPOINT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RESTBONUSPOINT.sCmd) = 0 then
      begin
        CmdRestBonuPoint(PlayObject, @g_GameCommand.RESTBONUSPOINT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SETPERMISSION.sCmd) = 0 then
      begin
        CmdSetPermission(PlayObject, @g_GameCommand.SETPERMISSION, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RENEWLEVEL.sCmd) = 0 then
      begin
        CmdReNewLevel(PlayObject, @g_GameCommand.RENEWLEVEL, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELGOLD.sCmd) = 0 then
      begin
        CmdDelGold(PlayObject, @g_GameCommand.DELGOLD, sParam1, StrToIntDef(sParam2, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ADDGOLD.sCmd) = 0 then
      begin
        CmdAddGold(PlayObject, @g_GameCommand.ADDGOLD, sParam1, StrToIntDef(sParam2, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.GAMEGOLD.sCmd) = 0 then
      begin
        CmdGameGold(PlayObject, @g_GameCommand.GAMEGOLD, sParam1, sParam2, StrToInt64Def(sParam3, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.GAMEPOINT.sCmd) = 0 then
      begin
        CmdGamePoint(PlayObject, @g_GameCommand.GAMEPOINT, sParam1, sParam2, StrToInt64Def(sParam3, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CreditPoint.sCmd) = 0 then
      begin
        CmdCreditPoint(PlayObject, @g_GameCommand.CreditPoint, sParam1, sParam2, StrToIntDef(sParam3, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.TRAINING.sCmd) = 0 then
      begin
        CmdTrainingSkill(PlayObject, @g_GameCommand.TRAINING, sParam1, sParam2, StrToIntDef(sParam3, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELETEITEM.sCmd) = 0 then
      begin
        CmdDeleteItem(PlayObject, @g_GameCommand.DELETEITEM, sParam1, sParam2, StrToIntDef(sParam3, 1));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELETESKILL.sCmd) = 0 then
      begin
        CmdDelSkill(PlayObject, @g_GameCommand.DELETESKILL, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.TRAININGSKILL.sCmd) = 0 then
      begin
        CmdTrainingMagic(PlayObject, @g_GameCommand.TRAININGSKILL, sParam1, sParam2, StrToIntDef(sParam3, 0),
          StrToIntDef(sParam4, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CLEARMISSION.sCmd) = 0 then
      begin
        CmdClearMission(PlayObject, @g_GameCommand.CLEARMISSION, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.STARTQUEST.sCmd) = 0 then
      begin
        CmdStartQuest(PlayObject, @g_GameCommand.STARTQUEST, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DENYIPLOGON.sCmd) = 0 then
      begin
        CmdDenyIPaddrLogon(PlayObject, @g_GameCommand.DENYIPLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DENYIPLOCALLOGON.sCmd) = 0 then
      begin
        CmdDenyIPLocalLogon(PlayObject, @g_GameCommand.DENYIPLOCALLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEDEARNAME.sCmd) = 0 then
      begin
        CmdChangeDearName(PlayObject, @g_GameCommand.CHANGEDEARNAME, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEMASTERNAME.sCmd) = 0 then
      begin
        CmdChangeMasterName(PlayObject, @g_GameCommand.CHANGEMASTERNAME, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CLEARMON.sCmd) = 0 then
      begin
        CmdClearMapMonster(PlayObject, @g_GameCommand.CLEARMON, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DENYACCOUNTLOGON.sCmd) = 0 then
      begin
        CmdDenyAccountLogon(PlayObject, @g_GameCommand.DENYACCOUNTLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DENYCHARNAMELOGON.sCmd) = 0 then
      begin
        CmdDenyCharNameLogon(PlayObject, @g_GameCommand.DENYCHARNAMELOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELDENYIPLOGON.sCmd) = 0 then
      begin
        CmdDelDenyIPaddrLogon(PlayObject, @g_GameCommand.DELDENYIPLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELDENYIPLOCALLOGON.sCmd) = 0 then
      begin
        CmdDelDenyIPLocalLogon(PlayObject, @g_GameCommand.DELDENYIPLOCALLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELDENYACCOUNTLOGON.sCmd) = 0 then
      begin
        CmdDelDenyAccountLogon(PlayObject, @g_GameCommand.DELDENYACCOUNTLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELDENYCHARNAMELOGON.sCmd) = 0 then
      begin
        CmdDelDenyCharNameLogon(PlayObject, @g_GameCommand.DELDENYCHARNAMELOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWDENYIPLOGON.sCmd) = 0 then
      begin
        CmdShowDenyIPaddrLogon(PlayObject, @g_GameCommand.SHOWDENYIPLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWDENYIPLOCALLOGON.sCmd) = 0 then
      begin
        CmdShowDenyIPLocalLogon(PlayObject, @g_GameCommand.SHOWDENYIPLOCALLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWDENYACCOUNTLOGON.sCmd) = 0 then
      begin
        CmdShowDenyAccountLogon(PlayObject, @g_GameCommand.SHOWDENYACCOUNTLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWDENYCHARNAMELOGON.sCmd) = 0 then
      begin
        CmdShowDenyCharNameLogon(PlayObject, @g_GameCommand.SHOWDENYCHARNAMELOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.Mission.sCmd) = 0 then
      begin
        CmdMission(PlayObject, @g_GameCommand.Mission, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.MobPlace.sCmd) = 0 then
      begin
        CmdMobPlace(PlayObject, @g_GameCommand.MobPlace, sParam1, sParam2, sParam3, sParam4);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SetMapMode.sCmd) = 0 then
      begin
        CmdSetMapMode(PlayObject, g_GameCommand.SetMapMode.sCmd, sParam1, sParam2, sParam3, sParam4);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWMAPMODE.sCmd) = 0 then
      begin
        CmdShowMapMode(PlayObject, g_GameCommand.SHOWMAPMODE.sCmd, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CLRPASSWORD.sCmd) = 0 then
      begin
        CmdClearHumanPassword(PlayObject, g_GameCommand.CLRPASSWORD.sCmd, g_GameCommand.CLRPASSWORD.nPermissionMin, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CONTESTPOINT.sCmd) = 0 then
      begin
        CmdContestPoint(PlayObject, @g_GameCommand.CONTESTPOINT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.STARTCONTEST.sCmd) = 0 then
      begin
        CmdStartContest(PlayObject, @g_GameCommand.STARTCONTEST, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ENDCONTEST.sCmd) = 0 then
      begin
        CmdEndContest(PlayObject, @g_GameCommand.ENDCONTEST, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ANNOUNCEMENT.sCmd) = 0 then
      begin
        CmdAnnouncement(PlayObject, @g_GameCommand.ANNOUNCEMENT, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DISABLESENDMSG.sCmd) = 0 then
      begin
        CmdDisableSendMsg(PlayObject, @g_GameCommand.DISABLESENDMSG, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ENABLESENDMSG.sCmd) = 0 then
      begin
        CmdEnableSendMsg(PlayObject, @g_GameCommand.ENABLESENDMSG, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.REFINEWEAPON.sCmd) = 0 then
      begin
        CmdRefineWeapon(PlayObject, @g_GameCommand.REFINEWEAPON, StrToIntDef(sParam1, 0), StrToIntDef(sParam2, 0),
          StrToIntDef(sParam3, 0), StrToIntDef(sParam4, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DISABLESENDMSGLIST.sCmd) = 0 then
      begin
        CmdDisableSendMsgList(PlayObject, @g_GameCommand.DISABLESENDMSGLIST);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.RestHero.sCmd) = 0 then
      begin
        CmdRestHero(PlayObject);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DENYMACHINEIDLOGON.sCmd) = 0 then
      begin
        CmdDenyMachineIDLogon(PlayObject, @g_GameCommand.DENYMACHINEIDLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DELMACHINEIDLOGON.sCmd) = 0 then
      begin
        CmdDelMachineIDLogon(PlayObject, @g_GameCommand.DELMACHINEIDLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SHOWMACHINEIDLOGON.sCmd) = 0 then
      begin
        CmdShowMachineIDLogon(PlayObject, @g_GameCommand.SHOWMACHINEIDLOGON, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.ShowEffect.sCmd) = 0 then
      begin
        CmdShowEffect(PlayObject, @g_GameCommand.ShowEffect, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.GiveMine.sCmd) = 0 then
      begin
        CmdGiveMine(PlayObject, @g_GameCommand.GiveMine, sParam1, StrToIntDef(sParam2, 0), StrToIntDef(sParam3, 0));
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SendTopChatBoardMsg.sCmd) = 0 then
      begin
        // sText := sParam1;
        sParam1 := sTemp;
        sParam1 := CopyEx(sParam1, 1, 100);
        CmdSendTopChatBoardMsg(PlayObject, @g_GameCommand.SendTopChatBoardMsg, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEGAMEDIAMOND.sCmd) = 0 then
      begin
        CmdChangeGameDiamond(PlayObject, @g_GameCommand.CHANGEGAMEDIAMOND, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEGAMEGIRD.sCmd) = 0 then
      begin
        CmdChangeGameGird(PlayObject, @g_GameCommand.CHANGEGAMEGIRD, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.CHANGEGAMEGLORY.sCmd) = 0 then
      begin
        CmdChangeGameGlory(PlayObject, @g_GameCommand.CHANGEGAMEGLORY, sParam1, sParam2, sParam3);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.NGLevel.sCmd) = 0 then
      begin
        CmdChangeNGLevel(PlayObject, @g_GameCommand.NGLevel, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DelUserShop.sCmd) = 0 then
      begin
        CmdDelUserShop(PlayObject, @g_GameCommand.DelUserShop, sParam1);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.SetUserShopName.sCmd) = 0 then
      begin
        CmdSetUserShopName(PlayObject, @g_GameCommand.SetUserShopName, sParam1, sParam2);
        Exit;
      end;

      if CompareText(sCmd, g_GameCommand.DelSellPlayer.sCmd) = 0 then
      begin
        CmdDelSellPlayer(PlayObject, @g_GameCommand.DelSellPlayer, sParam1);
        Exit;
      end;

      if m_btPermission > 4 then
      begin
        if CompareText(sCmd, g_GameCommand.BACKSTEP.sCmd) = 0 then
        begin
          CmdBackStep(PlayObject, sCmd, StrToIntDef(sParam1, 0), StrToIntDef(sParam2, 1));
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.BALL.sCmd) = 0 then
        begin // 精神波
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.CHANGELUCK.sCmd) = 0 then
        begin
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.HUNGER.sCmd) = 0 then
        begin
          CmdHunger(PlayObject, g_GameCommand.HUNGER.sCmd, sParam1, StrToIntDef(sParam2, 0));
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.NAMECOLOR.sCmd) = 0 then
        begin
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.TRANSPARECY.sCmd) = 0 then
        begin
          Exit;
        end;
        if CompareText(sCmd, g_GameCommand.LEVEL0.sCmd) = 0 then
        begin
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.SETFLAG.sCmd) = 0 then
        begin // 004D3BDD
          OnlineObject := UserEngine.GetPlayObject(sParam1);
          if OnlineObject <> nil then
          begin
            nFlag := StrToIntDef(sParam2, 0);
            nValue := StrToIntDef(sParam3, 0);
            OnlineObject.SetQuestFlagStatus(nFlag, nValue);
            if OnlineObject.GetQuestFlagStatus(nFlag) = 1 then
            begin
              SysMsg(OnlineObject.m_sCharName + ': [' + IntToStr(nFlag) + '] = ON', c_Green, t_Hint);
            end
            else
            begin
              SysMsg(OnlineObject.m_sCharName + ': [' + IntToStr(nFlag) + '] = OFF', c_Green, t_Hint);
            end;
          end
          else
            SysMsg('@' + g_GameCommand.SETFLAG.sCmd + ' 人物名称 标志号 数字(0 - 1)', c_Red, t_Hint);
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.RECONNECTION.sCmd) = 0 then
        begin
          CmdReconnection(PlayObject, sCmd, sParam1, sParam2, sParam3);
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.CHANGEGATE.sCmd) = 0 then
        begin
          CmdChangeGate(PlayObject, sCmd, sParam1, sParam2, sParam3);
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.DISABLEFILTER.sCmd) = 0 then
        begin
          CmdDisableFilter(PlayObject, sCmd, sParam1);
          Exit;
        end;
        if CompareText(sCmd, g_GameCommand.CHGUSERFULL.sCmd) = 0 then
        begin
          CmdChangeUserFull(PlayObject, sCmd, sParam1);
          Exit;
        end;
        if CompareText(sCmd, g_GameCommand.CHGZENFASTSTEP.sCmd) = 0 then
        begin
          CmdChangeZenFastStep(PlayObject, sCmd, sParam1);
          Exit;
        end;

        if CompareText(sCmd, g_GameCommand.OXQUIZROOM.sCmd) = 0 then
        begin
          Exit;
        end;
        if CompareText(sCmd, g_GameCommand.GSA.sCmd) = 0 then
        begin
          Exit;
        end;

        { TODO -ochongchong -c新增 : 加管理员命令：调整英雄忠诚度  【2013-07-28】 }
        if CompareText(sCmd, g_GameCommand.LOYALTYPOINT.sCmd) = 0 then
        begin
          CmdChangeLoyaltyPoint(PlayObject, @g_GameCommand.LOYALTYPOINT, sParam1, sParam2, sParam3);
          Exit;
        end;

        { TODO -ochongchong -c新增 : 加管理员命令: 取用户机器码 【2013-09-05】 }
        if CompareText(sCmd, g_GameCommand.MACHINEID.sCmd) = 0 then
        begin
          CmdGetMachineID(PlayObject, @g_GameCommand.MACHINEID, sParam1);
          Exit;
        end;

        if (m_btPermission >= 5) or (g_Config.boTestServer) then
        begin
          if CompareText(sCmd, g_GameCommand.FIREBURN.sCmd) = 0 then
          begin
            CmdFireBurn(PlayObject, StrToIntDef(sParam1, 0), StrToIntDef(sParam2, 0), StrToIntDef(sParam3, 0));
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.TESTFIRE.sCmd) = 0 then
          begin
            CmdTestFire(PlayObject, sCmd, StrToIntDef(sParam1, 0), StrToIntDef(sParam2, 0), StrToIntDef(sParam3, 0),
              StrToIntDef(sParam4, 0));
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.TESTSTATUS.sCmd) = 0 then
          begin
            CmdTestStatus(PlayObject, sCmd, StrToIntDef(sParam1, -1), StrToIntDef(sParam2, 0), sParam3 = '1');
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.DELGAMEGOLD.sCmd) = 0 then
          begin
            CmdDelGameGold(PlayObject, g_GameCommand.DELGAMEGOLD.sCmd, sParam1, StrToIntDef(sParam2, 0));
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.ADDGAMEGOLD.sCmd) = 0 then
          begin
            CmdAddGameGold(PlayObject, g_GameCommand.ADDGAMEGOLD.sCmd, sParam1, StrToIntDef(sParam2, 0));
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.TESTGOLDCHANGE.sCmd) = 0 then
          begin
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.RELOADADMIN.sCmd) = 0 then
          begin
            CmdReLoadAdmin(PlayObject, g_GameCommand.RELOADADMIN.sCmd);
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.ReLoadNpc.sCmd) = 0 then
          begin
            CmdReloadNpc(PlayObject, sParam1);
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.RELOADMANAGE.sCmd) = 0 then
          begin
            CmdReloadManage(PlayObject, @g_GameCommand.RELOADMANAGE, sParam1);
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.RELOADROBOTMANAGE.sCmd) = 0 then
          begin
            CmdReloadRobotManage(PlayObject);
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.RELOADROBOT.sCmd) = 0 then
          begin
            CmdReloadRobot(PlayObject);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.RELOADMONITEMS.sCmd) = 0 then
          begin
            CmdReloadMonItems(PlayObject);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.RELOADDIARY.sCmd) = 0 then
          begin
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.RELOADITEMDB.sCmd) = 0 then
          begin
            FrmDB.LoadItemsDB();
            SysMsg('物品数据库重新加载完成。', c_Green, t_Hint);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.RELOADMAGICDB.sCmd) = 0 then
          begin
            FrmDB.LoadMagicDB();
            UserEngine.ReloadMagicList();
            UserEngine.ReloadHeroMagicList();
            SysMsg('魔法数据库重新加载完成。', c_Green, t_Hint);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.RELOADMONSTERDB.sCmd) = 0 then
          begin
            FrmDB.LoadMonsterDB();
            SysMsg('怪物数据库重新加载完成。', c_Green, t_Hint);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.RELOADMINMAP.sCmd) = 0 then
          begin
            FrmDB.LoadMinMap();
            g_MapManager.ReSetMinMap();
            SysMsg('小地图配置重新加载完成。', c_Green, t_Hint);
            Exit;
          end;

          if CompareText(sCmd, g_GameCommand.ADJUESTLEVEL.sCmd) = 0 then
          begin
            CmdAdjuestLevel(PlayObject, @g_GameCommand.ADJUESTLEVEL, sParam1, StrToInt64Def(sParam2, 1));
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.ADJUESTEXP.sCmd) = 0 then
          begin
            CmdAdjuestExp(PlayObject, @g_GameCommand.ADJUESTEXP, sParam1, sParam2);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.AddGuild.sCmd) = 0 then
          begin
            CmdAddGuild(PlayObject, @g_GameCommand.AddGuild, sParam1, sParam2);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.DELGUILD.sCmd) = 0 then
          begin
            CmdDelGuild(PlayObject, @g_GameCommand.DELGUILD, sParam1);
            Exit;
          end;
          if (CompareText(sCmd, g_GameCommand.CHANGESABUKLORD.sCmd) = 0) then
          begin
            CmdChangeSabukLord(PlayObject, @g_GameCommand.CHANGESABUKLORD, sParam1, sParam2, True);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.FORCEDWALLCONQUESTWAR.sCmd) = 0 then
          begin
            CmdForcedWallconquestWar(PlayObject, @g_GameCommand.FORCEDWALLCONQUESTWAR, sParam1);
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.ADDTOITEMEVENT.sCmd) = 0 then
          begin
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.ADDTOITEMEVENTASPIECES.sCmd) = 0 then
          begin
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.ItemEventList.sCmd) = 0 then
          begin
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.STARTINGGIFTNO.sCmd) = 0 then
          begin
            Exit;
          end;
          if CompareText(sCmd, g_GameCommand.DELETEALLITEMEVENT.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.STARTITEMEVENT.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.ITEMEVENTTERM.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.ADJUESTTESTLEVEL.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.OPDELETESKILL.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.CHANGEWEAPONDURA.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.RELOADGUILDALL.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.SPIRIT.sCmd) = 0 then
          begin
            CmdSpirtStart(PlayObject, g_GameCommand.SPIRIT.sCmd, sParam1);
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.SPIRITSTOP.sCmd) = 0 then
          begin
            CmdSpirtStop(PlayObject, g_GameCommand.SPIRITSTOP.sCmd, sParam1);
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.TESTSERVERCONFIG.sCmd) = 0 then
          begin
            SendServerConfig();
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.SERVERSTATUS.sCmd) = 0 then
          begin
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.TESTGETBAGITEM.sCmd) = 0 then
          begin
            CmdTestGetBagItems(PlayObject, @g_GameCommand.TESTGETBAGITEM, sParam1);
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.MOBFIREBURN.sCmd) = 0 then
          begin
            CmdMobFireBurn(PlayObject, @g_GameCommand.MOBFIREBURN, sParam1, sParam2, sParam3, sParam4, sParam5, sParam6);
            Exit;
          end
          else if CompareText(sCmd, g_GameCommand.TESTSPEEDMODE.sCmd) = 0 then
          begin
            CmdTestSpeedMode(PlayObject, @g_GameCommand.TESTSPEEDMODE);
            Exit;
          end
          else if SameText(sCmd, 'GetVersion') then
          begin
            PlayObject.SysMsg('M2版本：' + g_version, c_Red, t_Hint);
            Exit;
          end
        end;
      end;

      if (g_UserCmds <> nil) and g_UserCmds.GotoLable(PlayObject, sCmd) then
      begin
        g_sInputParam0 := '';
        g_sInputParam1 := '';
        g_sInputParam2 := '';
        g_sInputParam3 := '';
        g_sInputParam4 := '';
        g_sInputParam5 := '';
        g_sInputParam6 := '';
        g_sInputParams := '';
        Exit;
      end;
    end;

    if g_PluginManager <> nil then
    begin
      if g_PluginManager.HookUserCommand(PlayObject, PAnsiChar(AnsiString(sCmd)), PAnsiChar(AnsiString(sParam1)),
        PAnsiChar(AnsiString(sParam2)), PAnsiChar(AnsiString(sParam3)), PAnsiChar(AnsiString(sParam4)),
        PAnsiChar(AnsiString(sParam5)), PAnsiChar(AnsiString(sParam6)), PAnsiChar(AnsiString(sParam7))) then
        Exit;
    end;

    PlayObject.SysMsg('@' + sCmd + g_sGameCommandPermissionTooLow, c_Red, t_Hint);
  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg, [sData]));
      MainOutMessage(E.Message);
    end;
  end;
end;

end.
