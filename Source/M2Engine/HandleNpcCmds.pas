unit HandleNpcCmds;

interface

uses
  Windows, SysUtils, Classes, Grobal2, ObjNpc, ObjBase, ObjPlayer, M2Definition, Math;

const
  MAXNPCCMDCODE = 1000;

type
  TConditionCmd = function(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject;
    QuestConditionInfo: pTQuestConditionInfo): Boolean;

  TActionCmd = procedure(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestActionInfo: pTQuestActionInfo;
    var boSendNpcSay, boBreak: Boolean);

  TConditionCmdArray = array [1 .. MAXNPCCMDCODE - 1] of TConditionCmd;

  TActionCmdArray = array [1 .. MAXNPCCMDCODE - 1] of TActionCmd;

var
  ConditionCmdArray: TConditionCmdArray;
  ActionCmdArray: TActionCmdArray;
  g_sNpcParam0: string;
  g_sNpcParam1: string;
  g_sNpcParam2: string;
  g_sNpcParam3: string;
  g_sNpcParam4: string;
  g_sNpcParam5: string;
  g_sNpcParam6: string;
  g_sNpcParam7: string;
  g_sNpcParam8: string;
  g_sNpcParam9: string;
  g_nNpcParam0: Integer;
  g_nNpcParam1: Integer;
  g_nNpcParam2: Integer;
  g_nNpcParam3: Integer;
  g_nNpcParam4: Integer;
  g_nNpcParam5: Integer;
  g_nNpcParam6: Integer;
  g_nNpcParam7: Integer;
  g_nNpcParam8: Integer;
  g_nNpcParam9: Integer;
  g_BatchList: TList;

type
  PBatchMoveInfo = ^TBatchMoveInfo;

  TBatchMoveInfo = record
    MapName: string;
    MoveX: Integer;
    MoveY: Integer;
    MoveDelay: LongWord;
  end;

function QuestCheckCondition(Npc: TNormNpc; UserObject: TPlayObject; ConditionList: TConditionList): Boolean;

function QuestActionProcess(Npc: TNormNpc; sLabel: string; UserObject: TPlayObject; ActionList: TList;
  var boSendNpcSay: Boolean): Boolean;

implementation

uses
  M2Share, NpcCommon;

procedure ClearBatchList;
var
  I: Integer;
begin
  for I := 0 to g_BatchList.Count - 1 do
  begin
    Dispose(PBatchMoveInfo(g_BatchList.Items[I]));
  end;
  g_BatchList.Clear;
end;

function QuestCheckConditionOr(Npc: TNormNpc; UserObject: TPlayObject; ConditionList: TList): Boolean;
resourcestring
  HookError = '[Exception] HookQuestConditionProcess';
var
  I: Integer;
  QuestConditionInfo: pTQuestConditionInfo;
  BaseObject: TBaseObject;
  PlayObject: TPlayObject;
  sVar, sRawParam: string;
  IsBreakParseVar: Boolean;
begin
  Result := False;
  for I := 0 to ConditionList.Count - 1 do
  begin
    QuestConditionInfo := ConditionList.Items[I];
    BaseObject := GetLevelBaseObjectCondition(Npc, UserObject, QuestConditionInfo); // 转换运行对象

    if BaseObject = nil then
    begin
      Result := QuestConditionInfo.boNot;
      Break;
    end
    else
    begin
      // 下面代码会有问题，N1设置到目标上面，而非自身变量 2020-09-29 21:54:02
      // 2020-09-29 21:54:08
      {
        [@Attack]
        #IF
        CHECKCURRTARGETRACE = 0
        #ACT
        M.GetObjectAbilityEx 0 N1
        SendMsg 6 目标人物的血量为<$STR(N1)>
        SENDMSG 6 你攻击了人物【<$CURRRTARGETNAME>】。使用魔法ID=<$CURRRUSEMAGICID> 255 249
        BREAK
      }

      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        PlayObject := TPlayObject(BaseObject)
      else
        PlayObject := UserObject;
    end;

    if QuestConditionInfo.VarInfo1.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam1, QuestConditionInfo.sParam1, QuestConditionInfo.nParam1,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam1, sVar, QuestConditionInfo.sParam1, QuestConditionInfo.nParam1);
    end;

    if QuestConditionInfo.VarInfo2.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam2, QuestConditionInfo.sParam2, QuestConditionInfo.nParam2,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam2, sVar, QuestConditionInfo.sParam2, QuestConditionInfo.nParam2);
    end;

    if QuestConditionInfo.VarInfo3.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam3, QuestConditionInfo.sParam3, QuestConditionInfo.nParam3,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam3, sVar, QuestConditionInfo.sParam3, QuestConditionInfo.nParam3);
    end;

    if QuestConditionInfo.VarInfo4.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam4, QuestConditionInfo.sParam4, QuestConditionInfo.nParam4,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam4, sVar, QuestConditionInfo.sParam4, QuestConditionInfo.nParam4);
    end;

    if QuestConditionInfo.VarInfo5.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam5, QuestConditionInfo.sParam5, QuestConditionInfo.nParam5,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam5, sVar, QuestConditionInfo.sParam5, QuestConditionInfo.nParam5);
    end;

    if QuestConditionInfo.VarInfo6.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam6, QuestConditionInfo.sParam6, QuestConditionInfo.nParam6,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam6, sVar, QuestConditionInfo.sParam6, QuestConditionInfo.nParam6);
    end;

    if QuestConditionInfo.VarInfo7.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam7, QuestConditionInfo.sParam7, QuestConditionInfo.nParam7,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam7, sVar, QuestConditionInfo.sParam7, QuestConditionInfo.nParam7);
    end;

    if QuestConditionInfo.VarInfo8.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam8, QuestConditionInfo.sParam8, QuestConditionInfo.nParam8,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam8, sVar, QuestConditionInfo.sParam8, QuestConditionInfo.nParam8);
    end;

    if QuestConditionInfo.VarInfo9.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam9, QuestConditionInfo.sParam9, QuestConditionInfo.nParam9,
        IsBreakParseVar);

      sRawParam := QuestConditionInfo.sRawParam9;
      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam9, sVar, QuestConditionInfo.sParam9, QuestConditionInfo.nParam9);
    end;

    if QuestConditionInfo.VarInfo10.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam10, QuestConditionInfo.sParam10, QuestConditionInfo.nParam10,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam10, sVar, QuestConditionInfo.sParam10, QuestConditionInfo.nParam10);
    end;

    if (QuestConditionInfo.nCMDCode >= 1) and (QuestConditionInfo.nCMDCode < MAXNPCCMDCODE) then
    begin
      if Assigned(ConditionCmdArray[QuestConditionInfo.nCMDCode]) then
      begin
        try
          Result := ConditionCmdArray[QuestConditionInfo.nCMDCode](Npc, BaseObject, PlayObject, QuestConditionInfo);
        except
          on E: Exception do
          begin
            MainOutMessage(Format('Condition脚本异常 %s(NPC: %s; Label: %s; Code: %s', [E.Message, Npc.m_sCharName, Npc.m_sLastLabel,
              QuestConditionInfo.sCmdLine]));
            raise;
          end;
        end;
      end;
    end
    else if (g_PluginManager <> nil) then
    begin
      Result := g_PluginManager.HookNpcConditionProcess(Npc, BaseObject, PlayObject, QuestConditionInfo);
    end;

    if QuestConditionInfo.boNot then // 检测命令取反
      Result := not Result;
    if Result then
      Break;
  end;
end;

function QuestCheckCondition(Npc: TNormNpc; UserObject: TPlayObject; ConditionList: TConditionList): Boolean;
resourcestring
  HookError = '[Exception] HookQuestConditionProcess';
var
  I, tmpTrueCount: Integer;
  QuestConditionInfo: pTQuestConditionInfo;
  BaseObject: TBaseObject;
  PlayObject: TPlayObject;
  sVar: string;
  IsBreakParseVar: Boolean;
begin
  if ConditionList.ConditionType = ct_or then
  begin
    Result := QuestCheckConditionOr(Npc, UserObject, ConditionList);
    Exit;
  end;

  if ConditionList.Count <= ConditionList.TrueCount then
    ConditionList.TrueCount := 0;

  Result := True;
  tmpTrueCount := 0;

  for I := 0 to ConditionList.Count - 1 do
  begin
    QuestConditionInfo := ConditionList.Items[I];

    BaseObject := GetLevelBaseObjectCondition(Npc, UserObject, QuestConditionInfo); // 转换运行对象
    if BaseObject = nil then
    begin
      Result := QuestConditionInfo.boNot;
      Break;
    end
    else
    begin
      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        PlayObject := TPlayObject(BaseObject)
      else
        PlayObject := UserObject;
    end;

    if QuestConditionInfo.VarInfo1.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam1, QuestConditionInfo.sParam1, QuestConditionInfo.nParam1,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam1, sVar, QuestConditionInfo.sParam1, QuestConditionInfo.nParam1);
    end;

    if QuestConditionInfo.VarInfo2.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam2, QuestConditionInfo.sParam2, QuestConditionInfo.nParam2,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam2, sVar, QuestConditionInfo.sParam2, QuestConditionInfo.nParam2);
    end;

    if QuestConditionInfo.VarInfo3.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam3, QuestConditionInfo.sParam3, QuestConditionInfo.nParam3,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam3, sVar, QuestConditionInfo.sParam3, QuestConditionInfo.nParam3);
    end;

    if QuestConditionInfo.VarInfo4.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam4, QuestConditionInfo.sParam4, QuestConditionInfo.nParam4,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam4, sVar, QuestConditionInfo.sParam4, QuestConditionInfo.nParam4);
    end;

    if QuestConditionInfo.VarInfo5.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam5, QuestConditionInfo.sParam5, QuestConditionInfo.nParam5,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam5, sVar, QuestConditionInfo.sParam5, QuestConditionInfo.nParam5);
    end;

    if QuestConditionInfo.VarInfo6.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam6, QuestConditionInfo.sParam6, QuestConditionInfo.nParam6,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam6, sVar, QuestConditionInfo.sParam6, QuestConditionInfo.nParam6);
    end;

    if QuestConditionInfo.VarInfo7.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam7, QuestConditionInfo.sParam7, QuestConditionInfo.nParam7,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam7, sVar, QuestConditionInfo.sParam7, QuestConditionInfo.nParam7);
    end;

    if QuestConditionInfo.VarInfo8.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam8, QuestConditionInfo.sParam8, QuestConditionInfo.nParam8,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam8, sVar, QuestConditionInfo.sParam8, QuestConditionInfo.nParam8);
    end;

    if QuestConditionInfo.VarInfo9.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam9, QuestConditionInfo.sParam9, QuestConditionInfo.nParam9,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam9, sVar, QuestConditionInfo.sParam9, QuestConditionInfo.nParam9);
    end;

    if QuestConditionInfo.VarInfo10.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam10, QuestConditionInfo.sParam10, QuestConditionInfo.nParam10,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestConditionInfo.sParam10, sVar, QuestConditionInfo.sParam10, QuestConditionInfo.nParam10);
    end;

    if (QuestConditionInfo.nCMDCode >= 1) //
      and (QuestConditionInfo.nCMDCode < MAXNPCCMDCODE) //
      and Assigned(ConditionCmdArray[QuestConditionInfo.nCMDCode]) then
    begin
      try
        Result := ConditionCmdArray[QuestConditionInfo.nCMDCode](Npc, BaseObject, PlayObject, QuestConditionInfo);
      except
        on E: Exception do
        begin
          MainOutMessage(Format('Condition脚本异常 %s(NPC: %s; Label: %s; Code: %s', [E.Message, Npc.m_sCharName, Npc.m_sLastLabel,
            QuestConditionInfo.sCmdLine]));
          raise;
        end;
      end;
    end
    else if g_PluginManager <> nil then
      Result := g_PluginManager.HookNpcConditionProcess(Npc, BaseObject, PlayObject, QuestConditionInfo);

    if QuestConditionInfo.boNot then // 检测命令取反
      Result := not Result;

    if Result then
      Inc(tmpTrueCount);

    if not Result and (ConditionList.TrueCount <= 0) then
      Exit;
  end;

  if ConditionList.TrueCount > 0 then
    Result := tmpTrueCount >= ConditionList.TrueCount;
end;

function QuestActionProcess(Npc: TNormNpc; sLabel: string; UserObject: TPlayObject; ActionList: TList;
  var boSendNpcSay: Boolean): Boolean;
resourcestring
  HookError = '[Exception] HookQuestActionProcess';
var
  I, nStartWhile, nEndWhile, nRunWhileCount: Integer;
  QuestActionInfo: pTQuestActionInfo;
  BaseObject: TBaseObject;
  PlayObject: TPlayObject;
  sVar: string;
  boBreak, boBreakWhile: Boolean;
  _boSendNpcSay: BOOL;
  _boBreak: BOOL;
  IsBreakParseVar: Boolean;

  procedure ProcessParams;
  begin
    // if (QuestActionInfo.nCMDCode <> nMOV) and (QuestActionInfo.nCMDCode <> nINC) and (QuestActionInfo.nCMDCode <> nDEC) then begin
    if QuestActionInfo.VarInfo1.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam1, QuestActionInfo.sParam1, QuestActionInfo.nParam1, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam1, sVar, QuestActionInfo.sParam1, QuestActionInfo.nParam1);
    end;

    if QuestActionInfo.VarInfo2.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam2, QuestActionInfo.sParam2, QuestActionInfo.nParam2, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam2, sVar, QuestActionInfo.sParam2, QuestActionInfo.nParam2);
    end;

    if QuestActionInfo.VarInfo3.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam3, QuestActionInfo.sParam3, QuestActionInfo.nParam3, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam3, sVar, QuestActionInfo.sParam3, QuestActionInfo.nParam3);
    end;

    if QuestActionInfo.VarInfo4.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam4, QuestActionInfo.sParam4, QuestActionInfo.nParam4, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam4, sVar, QuestActionInfo.sParam4, QuestActionInfo.nParam4);
    end;

    if QuestActionInfo.VarInfo5.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam5, QuestActionInfo.sParam5, QuestActionInfo.nParam5, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam5, sVar, QuestActionInfo.sParam5, QuestActionInfo.nParam5);
    end;

    if QuestActionInfo.VarInfo6.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam6, QuestActionInfo.sParam6, QuestActionInfo.nParam6, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam6, sVar, QuestActionInfo.sParam6, QuestActionInfo.nParam6);
    end;

    if QuestActionInfo.VarInfo7.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam7, QuestActionInfo.sParam7, QuestActionInfo.nParam7, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam7, sVar, QuestActionInfo.sParam7, QuestActionInfo.nParam7);
    end;

    if QuestActionInfo.VarInfo8.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam8, QuestActionInfo.sParam8, QuestActionInfo.nParam8, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam8, sVar, QuestActionInfo.sParam8, QuestActionInfo.nParam8);
    end;

    if QuestActionInfo.VarInfo9.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam9, QuestActionInfo.sParam9, QuestActionInfo.nParam9, IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam9, sVar, QuestActionInfo.sParam9, QuestActionInfo.nParam9);
    end;

    if QuestActionInfo.VarInfo10.VarAttr > aNone then
    begin
      Npc.GetVarValue(PlayObject, QuestActionInfo.sRawParam10, QuestActionInfo.sParam10, QuestActionInfo.nParam10,
        IsBreakParseVar);

      if not IsBreakParseVar then
        Npc.GetVarValue(PlayObject, QuestActionInfo.sParam10, sVar, QuestActionInfo.sParam10, QuestActionInfo.nParam10);
    end;
  end;

begin
  Result := True;
  boBreak := False;

  ClearBatchList;

  g_sNpcParam0 := '';
  g_sNpcParam1 := '';
  g_sNpcParam2 := '';
  g_sNpcParam3 := '';
  g_sNpcParam4 := '';
  g_sNpcParam5 := '';
  g_sNpcParam6 := '';
  g_sNpcParam7 := '';
  g_sNpcParam8 := '';
  g_sNpcParam9 := '';

  g_nNpcParam0 := 0;
  g_nNpcParam1 := 0;
  g_nNpcParam2 := 0;
  g_nNpcParam3 := 0;
  g_nNpcParam4 := 0;
  g_nNpcParam5 := 0;
  g_nNpcParam6 := 0;
  g_nNpcParam7 := 0;
  g_nNpcParam8 := 0;
  g_nNpcParam9 := 0;

  PlayObject := UserObject;
  I := 0;
  while I <= ActionList.Count - 1 do
  begin
    QuestActionInfo := ActionList.Items[I];

    // 相办法在这里支持循环 2021-03-20......
    if QuestActionInfo.nCMDCode = nNA_WHILE then
    begin
      nStartWhile := I;
      nEndWhile := I + 1;
      nRunWhileCount := 0;

      while nEndWhile <= ActionList.Count - 1 do
      begin
        if pTQuestActionInfo(ActionList.Items[nEndWhile]).nCMDCode = nNA_ENDWHILE then
        begin
          Inc(nEndWhile);
          Break;
        end
        else
        begin
          Inc(nEndWhile);
        end;
      end;

      while True do
      begin
        I := nStartWhile;

        QuestActionInfo := ActionList.Items[I];
        BaseObject := GetLevelBaseObjectAction(Npc, UserObject, QuestActionInfo); // 转换运行对象
        if BaseObject = nil then
        begin
          Result := False;
          Inc(I);
          Continue;
        end
        else
        begin
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
            PlayObject := TPlayObject(BaseObject)
          else
            PlayObject := UserObject;
        end;
        ProcessParams;
        boBreakWhile := False;
        ActionCmdArray[QuestActionInfo.nCMDCode](Npc, BaseObject, PlayObject, QuestActionInfo, boSendNpcSay, boBreakWhile);
        if boBreakWhile then
        begin
          I := nEndWhile;
          Break;
        end;

        Inc(nRunWhileCount);
        if nRunWhileCount > Max(g_Config.nLimitScriptGotoCount * 5, 1000) then
        begin
          MainOutMessage('[脚本死循环] NPC:' + Npc.m_sCharName + ' 命令: ' + QuestActionInfo.sCmdLine);
          I := nEndWhile;
          Break;
        end;

        Inc(I);
        while I <= ActionList.Count - 1 do
        begin
          QuestActionInfo := ActionList.Items[I];
          if QuestActionInfo.nCMDCode <> nNA_ENDWHILE then
          begin
            ProcessParams;

            if (QuestActionInfo.nCMDCode >= 1) //
              and (QuestActionInfo.nCMDCode < MAXNPCCMDCODE) //
              and Assigned(ActionCmdArray[QuestActionInfo.nCMDCode]) then
            begin
              try
                ActionCmdArray[QuestActionInfo.nCMDCode](Npc, BaseObject, PlayObject, QuestActionInfo, boSendNpcSay,
                  boBreakWhile);

                if boBreakWhile then
                  Break;
              except
                on E: Exception do
                begin
                  MainOutMessage(Format('Action脚本异常 %s(NPC: %s; Label: %s; Code: %s', [E.Message, Npc.m_sCharName, Npc.m_sLastLabel,
                    QuestActionInfo.sCmdLine]));
                  raise;
                end;
              end;
            end
            else if g_PluginManager <> nil then
            begin
              _boSendNpcSay := boSendNpcSay;
              _boBreak := boBreakWhile;
              g_PluginManager.HookNpcActionProcess(Npc, BaseObject, PlayObject, QuestActionInfo, _boSendNpcSay, _boBreak);
              boSendNpcSay := _boSendNpcSay;
              boBreakWhile := _boBreak;

              if boBreakWhile then
                Break;
            end;
            Inc(I);
          end
          else
          begin
            Inc(I);
            Break;
          end;
        end;
      end;

      Continue;
    end;

    QuestActionInfo := ActionList.Items[I];
    BaseObject := GetLevelBaseObjectAction(Npc, UserObject, QuestActionInfo); // 转换运行对象
    if BaseObject = nil then
    begin
      Result := False;

      // Break 改为 Inc(I) Continue;
      // 修正如下脚本当英雄不在时，人物也无法调整 chongchong 2015-12-02
      {
        H.CHANGEEXP + 1000
        CHANGEEXP + 5000
      }

      Inc(I);
      Continue;
    end
    else
    begin
      // 下面代码会有问题，N1设置到目标上面，而非自身变量 2020-09-29 21:54:02
      // 2020-09-29 21:54:08
      {
        [@Attack]
        #IF
        CHECKCURRTARGETRACE = 0
        #ACT
        M.GetObjectAbilityEx 0 N1
        SendMsg 6 目标人物的血量为<$STR(N1)>
        SENDMSG 6 你攻击了人物【<$CURRRTARGETNAME>】。使用魔法ID=<$CURRRUSEMAGICID> 255 249
        BREAK
      }

      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        PlayObject := TPlayObject(BaseObject)
      else
        PlayObject := UserObject;
    end;

    ProcessParams;

    if (QuestActionInfo.nCMDCode >= 1) //
      and (QuestActionInfo.nCMDCode < MAXNPCCMDCODE) //
      and Assigned(ActionCmdArray[QuestActionInfo.nCMDCode]) then
    begin
      try
        ActionCmdArray[QuestActionInfo.nCMDCode](Npc, BaseObject, PlayObject, QuestActionInfo, boSendNpcSay, boBreak);
        if boBreak then
        begin
          Result := False;
          Break;
        end;
      except
        on E: Exception do
        begin
          MainOutMessage(Format('Action脚本异常 %s(NPC: %s; Label: %s; Code: %s', [E.Message, Npc.m_sCharName, Npc.m_sLastLabel,
            QuestActionInfo.sCmdLine]));
          raise;
        end;
      end;
    end
    else if (g_PluginManager <> nil) then
    begin
      _boSendNpcSay := boSendNpcSay;
      _boBreak := boBreak;
      g_PluginManager.HookNpcActionProcess(Npc, BaseObject, PlayObject, QuestActionInfo, _boSendNpcSay, _boBreak);
      boSendNpcSay := _boSendNpcSay;
      boBreak := _boBreak;
      if boBreak then
      begin
        Result := False;
        Break;
      end;
    end;
    Inc(I);
  end;
end;

initialization

g_BatchList := TList.Create;

finalization

begin
  ClearBatchList;
  g_BatchList.Free;
end;

end.
