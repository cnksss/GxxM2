unit NpcConditionCmd;

interface

uses
  Windows, SysUtils, StrUtils, Classes, Grobal2, ObjNpc, ObjBase, ObjMon2,
  ObjHero, Envir, ObjPlayer, M2Threads, NpcCommon, M2Definition, SellPlayer,
  StringListHelper, Generics.Collections;

implementation

uses
  HandleNpcCmds, M2Share, HUtil32, Castle, Guild, DateUtils, Math, ObjSmartMon,
  SDK;

var
  TxtFileCache: TDictionary<string, TStringList>;

function CheckOrCreateTxtFile(const AFilePath: string): Boolean;
var
  tmpSL: TStrings;
begin
  Result := False;
  if not FileExists(AFilePath) then
  begin
    tmpSL := TStringList.Create;
    try
      try
        tmpSL.SaveToFile(AFilePath); // 文件路径可能是错的，比如没有此盘符。
        Result := True;
      except
      end;
    finally
      tmpSL.Free;
    end;
  end
  else
    Result := True;
end;

function GetTxtFileObject(AFilePath: string): TStringList;
begin
  AFilePath := UpperCase(AFilePath); // TDictionary的Key用字符串时，区分大小写。

  if TxtFileCache.TryGetValue(AFilePath, Result) then
    Exit;

  if CheckOrCreateTxtFile(AFilePath) then
  begin
    Result := TStringList.Create();
    Result.LoadFromFile(AFilePath);
    if not TxtFileCache.TryAdd(AFilePath, Result) then
      FreeAndNil(Result);
  end
  else
    Result := nil;
end;

procedure CloseTxtFileObjects;
var
  tmpValue: TStringList;
begin
  for tmpValue in TxtFileCache.values do
    tmpValue.Free;
end;

function ConditionOfCheck(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  S, S1, S2: string;
  IsError: Boolean;
  n10, I, Index: Integer;
  List: TList;
  SL: TStringList;
  N1, N2, II: Integer;
begin
  Result := False;
  if BaseObject = nil then
    Exit;

  if not BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    Exit;

  if (QuestConditionInfo.nParam1 < 0) or (QuestConditionInfo.nParam2 < 0) then
    Exit;

  if QuestConditionInfo.nParam2 <> 0 then
    QuestConditionInfo.nParam2 := 1;

  S := QuestConditionInfo.sParam1;
  if (Pos(',', S) = 0) and (Pos('-', S) = 0) then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
      n10 := TPlayObject(BaseObject).GetQuestFlagStatus(QuestConditionInfo.nParam1)
    else
      n10 := THeroObject(BaseObject).GetQuestFlagStatus(QuestConditionInfo.nParam1);

    Result := n10 = QuestConditionInfo.nParam2;
    Exit;
  end;

  // S := Copy(S, 2, Length(S) - 2);

  IsError := False;
  List := TList.Create;
  SL := TStringList.Create;
  try
    List.Capacity := 100;

    ExtractStrings([','], [' '], PChar(S), SL);

    for I := 0 to SL.Count - 1 do
    begin
      S := SL[I];
      if Length(S) = 0 then
      begin
        IsError := True;
        Break;
      end;

      Index := Pos('-', S);
      if Index > 0 then
      begin
        S1 := Copy(S, 1, Index - 1);
        S2 := Copy(S, Index + 1, MaxInt);

        if TryStrToInt(S1, N1) and TryStrToInt(S2, N2) then
        begin
          if N1 >= N2 then
          begin
            for II := N1 downto N2 do
            begin
              List.Add(Pointer(II));
            end;
          end
          else
          begin
            for II := N1 to N2 do
            begin
              List.Add(Pointer(II));
            end;
          end;
        end
        else
        begin
          IsError := True;
          Break;
        end;
      end
      else
      begin
        if TryStrToInt(S, N1) then
        begin
          List.Add(Pointer(N1))
        end
        else
        begin
          IsError := True;
          Break;
        end;
      end;
    end;

    if IsError then
    begin
      Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
      Exit;
    end;

    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to List.Count - 1 do
      begin
        n10 := TPlayObject(BaseObject).GetQuestFlagStatus(Integer(List.Items[I]));
        Result := n10 = QuestConditionInfo.nParam2;
        if not Result then
          Exit;
      end;
    end
    else
    begin
      for I := 0 to List.Count - 1 do
      begin
        n10 := THeroObject(BaseObject).GetQuestFlagStatus(Integer(List.Items[I]));
        Result := n10 = QuestConditionInfo.nParam2;
        if not Result then
          Exit;
      end;
    end;
  finally
    SL.Free;
    List.Free;
  end;
end;

function ConditionOfRandom(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := Random(QuestConditionInfo.nParam1) = 0;
end;

function ConditionOfRandomEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := Random(QuestConditionInfo.nParam2) < QuestConditionInfo.nParam1;
end;

function ConditionOfGender(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if CompareText(QuestConditionInfo.sParam1, sMAN) = 0 then
  begin
    Result := BaseObject.m_btGender = 0;
  end
  else
  begin
    Result := BaseObject.m_btGender = 1;
  end;
end;

function ConditionOfDatTime(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if CompareText(QuestConditionInfo.sParam1, sSUNRAISE) = 0 then
  begin // 太阳出来
    Result := g_nGameTime = 0;
  end;
  if CompareText(QuestConditionInfo.sParam1, sDAY) = 0 then
  begin // 白天
    Result := g_nGameTime = 1;
  end;
  if CompareText(QuestConditionInfo.sParam1, sSUNSET) = 0 then
  begin // 日落，傍晚
    Result := g_nGameTime = 2;
  end;
  if CompareText(QuestConditionInfo.sParam1, sNIGHT) = 0 then
  begin // 晚上
    Result := g_nGameTime = 3;
  end;
end;

function ConditionOfCheckLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := BaseObject.m_Abil.Level >= QuestConditionInfo.nParam1;
end;

function ConditionOfCheckJob(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;

  if BaseObject = nil then
  begin
    Exit;
  end;

  if CompareLStr(QuestConditionInfo.sParam1, sWARRIOR, 3) or (CompareText(QuestConditionInfo.sParam1, '战士') = 0) then
  begin
    Result := BaseObject.m_btJob = 0;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sWIZARD, 3) or (CompareText(QuestConditionInfo.sParam1, '法师') = 0) then
  begin
    Result := BaseObject.m_btJob = 1;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sTAOS, 3) or (CompareText(QuestConditionInfo.sParam1, '道士') = 0) then
  begin
    Result := BaseObject.m_btJob = 2;
  end;
end;

function ConditionOfCheckHeroJob(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (PlayObject.m_MyHero <> nil) then
  begin
    if CompareLStr(QuestConditionInfo.sParam1, sWARRIOR, 3) or (CompareText(QuestConditionInfo.sParam1, '战士') = 0) then
    begin
      Result := PlayObject.m_MyHero.m_btJob = 0;
    end;
    if CompareLStr(QuestConditionInfo.sParam1, sWIZARD, 3) or (CompareText(QuestConditionInfo.sParam1, '法师') = 0) then
    begin
      Result := PlayObject.m_MyHero.m_btJob = 1;
    end;
    if CompareLStr(QuestConditionInfo.sParam1, sTAOS, 3) or (CompareText(QuestConditionInfo.sParam1, '道士') = 0) then
    begin
      Result := PlayObject.m_MyHero.m_btJob = 2;
    end;
  end
  else
    Result := False;
end;

function ConditionOfCheckBBCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := BaseObject.m_SlaveList.Count >= QuestConditionInfo.nParam1;
end;

function ConditionOfCheckItem(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount: Integer;
  nParam: Integer;
  nDura: Integer;
begin
  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if QuestConditionInfo.nParam2 <= 0 then
    QuestConditionInfo.nParam2 := 1;
  BaseObject.QuestCheckItemEx(QuestConditionInfo.sParam1, nCount, nParam, nDura, QuestConditionInfo.nParam3 = 0, QuestConditionInfo.nParam4 > 0);
  Result := nCount >= QuestConditionInfo.nParam2;
end;

function ConditionOfCheckItemwLooks(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sCheckName: string;
  I: Integer;
  UserItem: pTUserItem;
  StdItem, StdItem2: pTStdItem;
  SmartObject: TSmartObject;
begin
  Result := False;

  sCheckName := QuestConditionInfo.sParam1;
  if sCheckName = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  StdItem2 := nil;
  for I := Low(SmartObject.m_UseItems) to High(SmartObject.m_UseItems) do
  begin
    UserItem := @SmartObject.m_UseItems[I];
    if UserItem.wIndex > 0 then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) and SameText(sCheckName, StdItem.Name) then
      begin
        StdItem2 := StdItem;
        Break;
      end;
    end;
  end;

  if StdItem2 = nil then
  begin
    for I := Low(SmartObject.m_JewelryBoxItems) to High(SmartObject.m_JewelryBoxItems) do
    begin
      UserItem := @SmartObject.m_JewelryBoxItems[I];
      if UserItem.wIndex > 0 then
      begin
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and SameText(sCheckName, StdItem.Name) then
        begin
          StdItem2 := StdItem;
          Break;
        end;
      end;
    end;
  end;

  if StdItem2 = nil then
  begin
    for I := Low(SmartObject.m_GodBlessItems) to High(SmartObject.m_GodBlessItems) do
    begin
      UserItem := @SmartObject.m_GodBlessItems[I];
      if UserItem.wIndex > 0 then
      begin
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and SameText(sCheckName, StdItem.Name) then
        begin
          StdItem2 := StdItem;
          Break;
        end;
      end;
    end;
  end;

  Result := StdItem2 <> nil;
  if Result then
  begin
    if QuestConditionInfo.sRawParam2 <> '' then
    begin
      if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam2, IntToStr(StdItem2.Looks), StdItem2.Looks) then
      begin
        Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
      end;
    end;
  end;
end;

function ConditionOfCheckItems(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sCheckName: string;
  I, nCount, nCheckCount: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  SmartObject: TSmartObject;
begin
  Result := False;

  sCheckName := QuestConditionInfo.sParam1;
  nCheckCount := QuestConditionInfo.nParam2;
  if nCheckCount = 0 then
    nCheckCount := 1;
  if sCheckName = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nCount := 0;
  for I := Low(SmartObject.m_UseItems) to High(SmartObject.m_UseItems) do
  begin
    UserItem := @SmartObject.m_UseItems[I];
    if UserItem.wIndex > 0 then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) and SameText(sCheckName, StdItem.Name) then
      begin
        Inc(nCount);
      end;
    end;
  end;

  for I := Low(SmartObject.m_JewelryBoxItems) to High(SmartObject.m_JewelryBoxItems) do
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[I];
    if UserItem.wIndex > 0 then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) and SameText(sCheckName, StdItem.Name) then
      begin
        Inc(nCount);
      end;
    end;
  end;

  for I := Low(SmartObject.m_GodBlessItems) to High(SmartObject.m_GodBlessItems) do
  begin
    UserItem := @SmartObject.m_GodBlessItems[I];
    if UserItem.wIndex > 0 then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) and SameText(sCheckName, StdItem.Name) then
      begin
        Inc(nCount);
      end;
    end;
  end;

  Result := nCount >= nCheckCount;
end;

function ConditionOfCheckItemW(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;

  function CheckItemW(sItemType: string; nParam: Integer; FullNameCheck: Boolean; CheckChangedName: Boolean): pTUserItem;
  // 0049BA7C
  var
    nCount: Integer;
    SmartObject: TSmartObject;
  begin
    Result := nil;
    if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
      SmartObject := TSmartObject(BaseObject)
    else
      Exit;

    if CompareLStr(sItemType, '[NECKLACE]', 4) then
    begin
      if SmartObject.m_UseItems[U_NECKLACE].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_NECKLACE];
      end;
      Exit;
    end;
    if CompareLStr(sItemType, '[RING]', 4) then
    begin
      if SmartObject.m_UseItems[U_RINGL].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_RINGL];
      end;
      if SmartObject.m_UseItems[U_RINGR].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_RINGR];
      end;
      Exit;
    end;
    if CompareLStr(sItemType, '[ARMRING]', 4) then
    begin
      if SmartObject.m_UseItems[U_ARMRINGL].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_ARMRINGL];
      end;
      if SmartObject.m_UseItems[U_ARMRINGR].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_ARMRINGR];
      end;
      Exit;
    end;
    if CompareLStr(sItemType, '[WEAPON]', 4) then
    begin
      if SmartObject.m_UseItems[U_WEAPON].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_WEAPON];
      end;
      Exit;
    end;
    if CompareLStr(sItemType, '[HELMET]', 4) then
    begin
      if SmartObject.m_UseItems[U_HELMET].wIndex > 0 then
      begin
        Result := @SmartObject.m_UseItems[U_HELMET];
      end;
      Exit;
    end;
    Result := SmartObject.sub_4C4CD4(sItemType, nCount, FullNameCheck, CheckChangedName);
    if nCount < nParam then
      Result := nil;
  end;

var
  UserItem: pTUserItem;
begin
  Result := False;
  if BaseObject = nil then
    Exit;

  UserItem := CheckItemW(QuestConditionInfo.sParam1, QuestConditionInfo.nParam2, QuestConditionInfo.nParam3 = 0, False);
  Result := UserItem <> nil;
end;

function ConditionOfCheckGold(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (BaseObject <> nil) and (BaseObject.m_nGold >= QuestConditionInfo.nParam1);
end;

function ConditionOfIsTakeItem(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
end;

function ConditionOfCheckDura(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount, nParam: Integer;
  nDura: Integer;
begin
  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  BaseObject.QuestCheckItem(QuestConditionInfo.sParam1, nCount, nParam, nDura);
  Result := Round(nDura / 1000) >= QuestConditionInfo.nParam2;
end;

function ConditionOfCheckDuraEva(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount, nParam: Integer;
  nDura: Integer;
begin
  Result := False;
  if BaseObject = nil then
    Exit;

  BaseObject.QuestCheckItem(QuestConditionInfo.sParam1, nCount, nParam, nDura);
  if nCount > 0 then
    Result := Round(nParam / nCount / 1000) >= QuestConditionInfo.nParam2;
end;

function ConditionOfDayOfWeek(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if CompareLStr(QuestConditionInfo.sParam1, sSUN, Length(sSUN)) then
  begin
    Result := DayOfWeek(Now) = 1;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sMON, Length(sMON)) then
  begin
    Result := DayOfWeek(Now) = 2;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sTUE, Length(sTUE)) then
  begin
    Result := DayOfWeek(Now) = 3;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sWED, Length(sWED)) then
  begin
    Result := DayOfWeek(Now) = 4;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sTHU, Length(sTHU)) then
  begin
    Result := DayOfWeek(Now) = 5;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sFRI, Length(sFRI)) then
  begin
    Result := DayOfWeek(Now) = 6;
  end;
  if CompareLStr(QuestConditionInfo.sParam1, sSAT, Length(sSAT)) then
  begin
    Result := DayOfWeek(Now) = 7;
  end;
end;

function ConditionOfHour(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  Hour, Min, Sec, MSec: Word;
begin
  if (QuestConditionInfo.nParam1 <> 0) and (QuestConditionInfo.nParam2 = 0) then
    QuestConditionInfo.nParam2 := QuestConditionInfo.nParam1;
  DecodeTime(Time, Hour, Min, Sec, MSec);
  if (Hour < QuestConditionInfo.nParam1) or (Hour > QuestConditionInfo.nParam2) then
    Result := False
  else
    Result := True;
end;

function ConditionOfMin(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  Hour, Min, Sec, MSec: Word;
begin
  if (QuestConditionInfo.nParam1 <> 0) and (QuestConditionInfo.nParam2 = 0) then
    QuestConditionInfo.nParam2 := QuestConditionInfo.nParam1;
  DecodeTime(Time, Hour, Min, Sec, MSec);
  if (Min < QuestConditionInfo.nParam1) or (Min > QuestConditionInfo.nParam2) then
    Result := False
  else
    Result := True;
end;

function ConditionOfCheckPKPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and (TSmartObject(BaseObject).PKLevel >= QuestConditionInfo.nParam1);
end;

function ConditionOfCheckLuckPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nLuck, nLuckyPoint: Integer;
  {
    UserItem: pTUserItem;
    pStdItem: pTStdItem;
    StdItem: TStdItem;
  }
begin
  Result := False;
  nLuckyPoint := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nLuckyPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  {
    nLuck := 0;
    UserItem := @PlayObject.m_UseItems[U_WEAPON];
    if UserItem.wIndex > 0 then
    begin
    pStdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if pStdItem <> nil then
    begin
    StdItem := pStdItem^;
    ItemUnit.GetItemAddValue(UserItem, StdItem);
    if Loword(StdItem.AC) > 0 then
    nLuck := Loword(StdItem.AC);
    end;
    end;

    UserItem := @PlayObject.m_UseItems[U_NECKLACE];
    if UserItem.wIndex > 0 then
    begin
    pStdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if pStdItem <> nil then
    begin
    StdItem := pStdItem^;
    ItemUnit.GetItemAddValue(UserItem, StdItem);
    if Hiword(StdItem.MAC) > 0 then
    nLuck := nLuck + Hiword(StdItem.MAC);
    end;
    end;
  }

  nLuck := BaseObject.m_nLuck;
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nLuck = nLuckyPoint then
        Result := True;
    '>':
      if nLuck > nLuckyPoint then
        Result := True;
    '<':
      if nLuck < nLuckyPoint then
        Result := True;
  else
    if nLuck >= nLuckyPoint then
      Result := True;
  end;
end;

function ConditionOfCheckMonMapCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  nCount: Integer;
  nMapRangeCount: Integer;
  Envir: TEnvirnoment;
  MonList: TList;
  AObject: TBaseObject;
  IsExclueBB: Boolean;
begin
  Result := False;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  nCount := QuestConditionInfo.nParam2;

  // 排除BB chongchong 2014-03-17
  IsExclueBB := QuestConditionInfo.nParam3 <> 0;

  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if (Envir = nil) or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  MonList := TList.Create;
  UserEngine.GetMapMonster(Envir, MonList);
  for I := MonList.Count - 1 downto 0 do
  begin
    if MonList.Count <= 0 then
      Break;
    AObject := TBaseObject(MonList.Items[I]);
    if (AObject.m_btRaceServer < RC_ANIMAL) or (AObject.m_btRaceServer = RC_ARCHERGUARD) or (AObject.m_Master <> nil) or (AObject.m_btRaceServer = RC_NPC) or (BaseObject.m_btRaceServer = RC_PEACENPC) then
      MonList.Delete(I)
    else if IsExclueBB and (AObject.Master <> nil) then
      MonList.Delete(I);
  end;
  nMapRangeCount := MonList.Count;
  MonList.Free;

  if nMapRangeCount >= nCount then
    Result := True;
end;

function ConditionOfCheckMapHuman(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  Envir: TEnvirnoment;
begin
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  Result := UserEngine.GetMapHuman(Envir) >= QuestConditionInfo.nParam2;
end;

function ConditionOfCheckBagGage(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  StdItem: pTStdItem;
begin
  Result := False;

  if BaseObject = nil then
  begin
    Exit;
  end;

  if BaseObject.IsEnoughBag then
  begin
    if QuestConditionInfo.sParam1 <> '' then
    begin
      StdItem := UserEngine.GetStdItem(QuestConditionInfo.sParam1);
      if StdItem <> nil then
      begin
        if PlayObject.IsAddWeightAvailable(StdItem.Weight) then
          Result := True;
      end;
    end;
  end;
end;

function CheckAnsiContainsTextList(sTest, sListFileName: string): Boolean;
var
  I: Integer;
  S: string;
  LoadList: TStringList;
begin
  Result := False;
  sListFileName := g_Config.sEnvirDir + sListFileName;
  if FileExists(sListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;
    sTest := LowerCase(sTest);
    for I := 0 to LoadList.Count - 1 do
    begin
      S := LowerCase(Trim(LoadList[I]));
      if Pos(sTest, S) <> 0 then
      begin
        Result := True;
        Break;
      end;
    end;
    LoadList.Free;
  end
  else
    MainOutMessage('file not found => ' + sListFileName);
end;

function CheckStringList(sHumName, sListFileName: string; IsAbsolutePath, CaseSensitive: Boolean): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sText: string;
begin
  Result := False;
  if not IsAbsolutePath then
    sListFileName := g_Config.sEnvirDir + sListFileName;
  if FileExists(sListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;

    if not CaseSensitive then
    begin
      for I := 0 to LoadList.Count - 1 do
      begin
        sText := Trim(LoadList[I]);
        if CompareText(sText, sHumName) = 0 then
        begin
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      for I := 0 to LoadList.Count - 1 do
      begin
        sText := Trim(LoadList[I]);
        if CompareStr(sText, sHumName) = 0 then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
    LoadList.Free;
  end
  else
    MainOutMessage('file not found => ' + sListFileName);
end;

function CheckStringListEx(sValue1, sValue2, sListFileName: string; IsAbsolutePath, CaseSensitive: Boolean): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sLineText: string;
  sText1, sText2: string;
begin
  Result := False;
  if not IsAbsolutePath then
    sListFileName := g_Config.sEnvirDir + sListFileName;
  if FileExists(sListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;

    if not CaseSensitive then
    begin
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList[I]);
        if sLineText = '' then
          Continue;
        sLineText := GetValidStr3_Ex(sLineText, sText1, ' ');
        sLineText := GetValidStr3_Ex(sLineText, sText2, ' ');
        if (sText1 = '') or (sText1 = '') then
          Continue;
        if (CompareText(sText1, sValue1) = 0) and (CompareText(sText2, sValue2) = 0) then
        begin
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList[I]);
        if sLineText = '' then
          Continue;
        sLineText := GetValidStr3_Ex(sLineText, sText1, ' ');
        sLineText := GetValidStr3_Ex(sLineText, sText2, ' ');
        if (sText1 = '') or (sText1 = '') then
          Continue;
        if (CompareStr(sText1, sValue1) = 0) and (CompareStr(sText2, sValue2) = 0) then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
    LoadList.Free;
  end
  else
    MainOutMessage('file not found => ' + sListFileName);
end;

function CheckCacheStringList(sHumName, sListFileName: string; IsAbsolutePath, CaseSensitive: Boolean): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sText: string;
begin
  Result := False;
  if not IsAbsolutePath then
    sListFileName := g_Config.sEnvirDir + sListFileName;

  LoadList := GetTxtFileObject(sListFileName);
  if Assigned(LoadList) then
  begin
    for I := 0 to LoadList.Count - 1 do
    begin
      sText := Trim(LoadList[I]);

      if not CaseSensitive then
      begin
        if CompareText(sText, sHumName) = 0 then
        begin
          Result := True;
          Break;
        end;
      end
      else
      begin
        if CompareStr(sText, sHumName) = 0 then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
  end
  else
    MainOutMessage('file not found => ' + sListFileName);
end;

function CheckCacheStringListEx(sValue1, sValue2, sListFileName: string; IsAbsolutePath, CaseSensitive: Boolean): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sLineText: string;
  sText1, sText2: string;
begin
  Result := False;
  if not IsAbsolutePath then
    sListFileName := g_Config.sEnvirDir + sListFileName;

  LoadList := GetTxtFileObject(sListFileName);
  if Assigned(LoadList) then
  begin
    if not CaseSensitive then
    begin
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList[I]);
        if sLineText = '' then
          Continue;
        sLineText := GetValidStr3_Ex(sLineText, sText1, ' ');
        sLineText := GetValidStr3_Ex(sLineText, sText2, ' ');
        if (sText1 = '') or (sText1 = '') then
          Continue;
        if (CompareText(sText1, sValue1) = 0) and (CompareText(sText2, sValue2) = 0) then
        begin
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList[I]);
        if sLineText = '' then
          Continue;
        sLineText := GetValidStr3_Ex(sLineText, sText1, ' ');
        sLineText := GetValidStr3_Ex(sLineText, sText2, ' ');
        if (sText1 = '') or (sText1 = '') then
          Continue;
        if (CompareStr(sText1, sValue1) = 0) and (CompareStr(sText2, sValue2) = 0) then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
  end
  else
    MainOutMessage('file not found => ' + sListFileName);
end;

function ConditionOfCheckNameList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if QuestConditionInfo.nParam2 = 0 then
    Result := CheckStringList(BaseObject.m_sCharName, Npc.m_sPath + QuestConditionInfo.sParam1, False, False)
  else
    Result := CheckStringList(BaseObject.m_sCharName, QuestConditionInfo.sParam1, True, False);
end;

function ConditionOfCheckAccountList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := CheckStringList(PlayObject.m_sUserID, Npc.m_sPath + QuestConditionInfo.sParam1, False, False);
end;

function ConditionOfCheckIPList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := CheckStringList(PlayObject.m_sIPaddr, Npc.m_sPath + QuestConditionInfo.sParam1, False, False);
end;

function ConditionOfEqual(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  IsBreakParseVar: Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  // Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam1, QuestConditionInfo.sParam1, QuestConditionInfo.nParam1);
  // Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam2, QuestConditionInfo.sParam2, QuestConditionInfo.nParam2);

  // MainOutMessage(Format('%s %s %d %d',[QuestConditionInfo.sParam1, QuestConditionInfo.sParam2,QuestConditionInfo.nParam1, QuestConditionInfo.nParam2]));
  Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam2, QuestConditionInfo.sParam2, QuestConditionInfo.nParam2, IsBreakParseVar);
  Result := (CompareText(QuestConditionInfo.sParam1, QuestConditionInfo.sParam2) = 0) and (QuestConditionInfo.nParam1 = QuestConditionInfo.nParam2);
end;

function ConditionOfLarge(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := QuestConditionInfo.nParam1 > QuestConditionInfo.nParam2;
end;

function ConditionOfSmall(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := QuestConditionInfo.nParam1 < QuestConditionInfo.nParam2;
end;

function ConditionOfIsSysop(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_btPermission >= 4);
end;

function ConditionOfIsAdmin(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := (PlayObject.m_btPermission >= 6);
end;

function ConditionOfCheckGroupCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if (PlayObject.m_GroupOwner = nil) then
    Exit;

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.nParam2 < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    PlayObject.m_GroupOwner.m_GroupMembers.LockR(9);
  try
{$IFEND}
    cMethod := QuestConditionInfo.sParam1[1];
    case cMethod of
      '=':
        if PlayObject.m_GroupOwner.m_GroupMembers.Count = QuestConditionInfo.nParam2 then
          Result := True;
      '>':
        if PlayObject.m_GroupOwner.m_GroupMembers.Count > QuestConditionInfo.nParam2 then
          Result := True;
      '<':
        if PlayObject.m_GroupOwner.m_GroupMembers.Count < QuestConditionInfo.nParam2 then
          Result := True;
    else
      if PlayObject.m_GroupOwner.m_GroupMembers.Count >= QuestConditionInfo.nParam2 then
        Result := True;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      PlayObject.m_GroupOwner.m_GroupMembers.UnLockR;
  end;
{$IFEND}
end;

function ConditionOfCheckPoseDir(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
begin
  Result := False;

  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  PoseHuman := BaseObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.GetPoseCreate = BaseObject) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    case QuestConditionInfo.nParam1 of
      1:
        if PoseHuman.m_btGender = PlayObject.m_btGender then
          Result := True; // 要求相同性别
      2:
        if PoseHuman.m_btGender <> PlayObject.m_btGender then
          Result := True; // 要求不同性别
    else
      Result := True; // 无参数时不判别性别
    end;
  end;
end;

function ConditionOfCheckPoseLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
  cMethod: Char;
begin
  Result := False;

  if BaseObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.nParam2 < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  PoseHuman := BaseObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    case cMethod of
      '=':
        if PoseHuman.m_Abil.Level = QuestConditionInfo.nParam2 then
          Result := True;
      '>':
        if PoseHuman.m_Abil.Level > QuestConditionInfo.nParam2 then
          Result := True;
      '<':
        if PoseHuman.m_Abil.Level < QuestConditionInfo.nParam2 then
          Result := True;
    else
      if PoseHuman.m_Abil.Level >= QuestConditionInfo.nParam2 then
        Result := True;
    end;
  end;
end;

function ConditionOfCheckPoseGender(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
  btSex: Byte;
begin
  Result := False;

  if BaseObject = nil then
  begin
    Exit;
  end;

  btSex := 0;
  if CompareText(QuestConditionInfo.sParam1, 'MAN') = 0 then
  begin
    btSex := 0;
  end
  else if CompareText(QuestConditionInfo.sParam1, '男') = 0 then
  begin
    btSex := 0;
  end
  else if CompareText(QuestConditionInfo.sParam1, 'WOMAN') = 0 then
  begin
    btSex := 1;
  end
  else if CompareText(QuestConditionInfo.sParam1, '女') = 0 then
  begin
    btSex := 1;
  end;
  PoseHuman := BaseObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.m_btGender = btSex) then
    Result := True;
end;

function ConditionOfCheckBonusPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nTotlePoint, nCount: Integer;
  cMethod: Char;
begin
  Result := False;

  if PlayObject = nil then
  begin
    Exit;
  end;

  nTotlePoint := PlayObject.m_BonusAbil.DC + PlayObject.m_BonusAbil.MC + PlayObject.m_BonusAbil.SC + PlayObject.m_BonusAbil.AC + PlayObject.m_BonusAbil.MAC + PlayObject.m_BonusAbil.HP + PlayObject.m_BonusAbil.MP + PlayObject.m_BonusAbil.Hit + PlayObject.m_BonusAbil.Speed + PlayObject.m_BonusAbil.X2;
  nTotlePoint := nTotlePoint + PlayObject.m_nBonusPoint;
  cMethod := QuestConditionInfo.sParam1[1];
  nCount := QuestConditionInfo.nParam2;
  case cMethod of
    '=':
      if nTotlePoint = nCount then
        Result := True;
    '>':
      if nTotlePoint > nCount then
        Result := True;
    '<':
      if nTotlePoint < nCount then
        Result := True;
  else
    if nTotlePoint >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckMarry(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if PlayObject = nil then
    Exit;
  Result := PlayObject.m_sDearName <> '';
end;

function ConditionOfCheckPoseMarry(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
begin
  Result := False;
  if PlayObject = nil then
  begin
    Exit;
  end;

  PoseHuman := PlayObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    if TPlayObject(PoseHuman).m_sDearName <> '' then
      Result := True;
  end;
end;

function ConditionOfCheckMarryCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount: Integer;
  cMethod: Char;
begin
  Result := False;

  if PlayObject = nil then
  begin
    Exit;
  end;

  nCount := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_btMarryCount = nCount then
        Result := True;
    '>':
      if PlayObject.m_btMarryCount > nCount then
        Result := True;
    '<':
      if PlayObject.m_btMarryCount < nCount then
        Result := True;
  else
    if PlayObject.m_btMarryCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if PlayObject = nil then
    Exit;
  Result := (Length(PlayObject.m_sMasterName) = 0) and (PlayObject.m_boMaster);
end;

function ConditionOfHaveMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;

  if PlayObject = nil then
  begin
    Exit;
  end;

  if PlayObject.m_sMasterName <> '' then
    Result := True;
end;

function ConditionOfCheckPoseMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
begin
  Result := False;

  if PlayObject = nil then
  begin
    Exit;
  end;

  PoseHuman := PlayObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    if (TPlayObject(PoseHuman).m_sMasterName <> '') and not (TPlayObject(PoseHuman).m_boMaster) then
      Result := True;
  end;
end;

{
  function ConditionOfPoseHaveMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
  var
  PoseHuman: TBaseObject;
  begin
  Result := False;
  PoseHuman := PlayObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
  begin
  if (TPlayObject(PoseHuman).m_sMasterName <> '') then
  Result := True;
  end;
  end;
}

function ConditionOfCheckLevelEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nLevel: Int64;
  cMethod: Char;
begin
  Result := False;

  if BaseObject = nil then
  begin
    Exit;
  end;

  nLevel := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if BaseObject.m_Abil.Level = nLevel then
        Result := True;
    '>':
      if BaseObject.m_Abil.Level > nLevel then
        Result := True;
    '<':
      if BaseObject.m_Abil.Level < nLevel then
        Result := True;
  else
    if BaseObject.m_Abil.Level >= nLevel then
      Result := True;
  end;
end;

function ConditionOfCheckHaveGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := PlayObject.m_MyGuild <> nil;
end;

function ConditionOfCheckIsGuildMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := PlayObject.IsGuildMaster;
end;

function ConditionOfCheckGuildMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  AObject: TPlayObject;
begin
  Result := False;
  if PlayObject = nil then
  begin
    Exit;
  end;

  if QuestConditionInfo.sRawParam2 <> '' then
  begin
    AObject := UserEngine.GetPlayObject(QuestConditionInfo.sParam2);
    if AObject <> nil then
    begin
      Result := AObject.IsGuildMaster and (CompareText(TGUild(AObject.m_MyGuild).sGuildName, QuestConditionInfo.sParam1) = 0);
    end;
  end
  else
  begin
    Result := PlayObject.IsGuildMaster and (CompareText(TGUild(PlayObject.m_MyGuild).sGuildName, QuestConditionInfo.sParam1) = 0);
  end;
end;

function ConditionOfCheckIsCastleMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := PlayObject.IsGuildMaster and (g_CastleManager.IsCastleMember(PlayObject) <> nil);
end;

function ConditionOfCheckIsCastleaGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := g_CastleManager.IsCastleMember(PlayObject) <> nil;
end;

function ConditionOfCheckIsAttackGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if Npc.m_Castle = nil then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if PlayObject.m_MyGuild = nil then
    Exit;
  Result := TUserCastle(Npc.m_Castle).IsAttackGuild(TGUild(PlayObject.m_MyGuild));
end;

function ConditionOfCheckIsDefenseGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if PlayObject = nil then
  begin
    Result := False;
    Exit;
  end;

  if Npc.m_Castle = nil then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_MyGuild = nil then
    Exit;
  Result := TUserCastle(Npc.m_Castle).IsDefenseGuild(TGUild(PlayObject.m_MyGuild));
end;

function ConditionOfCheckCastleDoorStatus(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nDay: Integer;
  nDoorStatus: Integer;
  CastleDoor: TCastleDoor;
begin
  Result := False;
  nDay := QuestConditionInfo.nParam2;

  nDoorStatus := -1;
  if CompareText(QuestConditionInfo.sParam1, '损坏') = 0 then
    nDoorStatus := 0;
  if CompareText(QuestConditionInfo.sParam1, '开启') = 0 then
    nDoorStatus := 1;
  if CompareText(QuestConditionInfo.sParam1, '关闭') = 0 then
    nDoorStatus := 2;

  if (nDay < 0) or (Npc.m_Castle = nil) or (nDoorStatus < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  CastleDoor := TCastleDoor(TUserCastle(Npc.m_Castle).m_MainDoor.BaseObj);

  case nDoorStatus of
    0:
      if CastleDoor.m_boDeath then
        Result := True;
    1:
      if CastleDoor.m_boOpened then
        Result := True;
    2:
      if not CastleDoor.m_boOpened then
        Result := True;
  end;
end;

function ConditionOfCheckIsAttackAllyGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if Npc.m_Castle = nil then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if PlayObject.m_MyGuild = nil then
    Exit;
  Result := TUserCastle(Npc.m_Castle).IsAttackAllyGuild(TGUild(PlayObject.m_MyGuild));
end;

function ConditionOfCheckIsDefenseAllyGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if Npc.m_Castle = nil then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if PlayObject.m_MyGuild = nil then
    Exit;
  Result := TUserCastle(Npc.m_Castle).IsDefenseAllyGuild(TGUild(PlayObject.m_MyGuild));
end;

{
  function ConditionOfCheckPoseIsMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
  var
  PoseHuman: TBaseObject;
  begin
  Result := False;
  PoseHuman := PlayObject.GetPoseCreate();
  if (PoseHuman <> nil) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
  begin
  if (TPlayObject(PoseHuman).m_sMasterName <> '') and (TPlayObject(PoseHuman).m_boMaster) then
  Result := True;
  end;
  end;
}

function ConditionOfCheckNameIPList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sCharName: string;
  sFileName: string;
  sCharAccount: string;
  sCharIPaddr: string;
  sLine: string;
  sName: string;
  sIPaddr: string;
begin
  Result := False;
  LoadList := TStringList.Create;
  try
    sFileName := QuestConditionInfo.sParam1;
    sCharName := PlayObject.m_sCharName;
    sCharAccount := PlayObject.m_sUserID;
    sCharIPaddr := PlayObject.m_sIPaddr;
    if FileExists(g_Config.sEnvirDir + sFileName) then
    begin
      LoadList.LoadFromFile(g_Config.sEnvirDir + sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLine := LoadList[I];
        if sLine[1] = ';' then
          Continue;
        sIPaddr := GetValidStr3(sLine, sName, [' ', '/', #9]);
        sIPaddr := Trim(sIPaddr);
        if (sName = sCharName) and (sIPaddr = sCharIPaddr) then
        begin
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    end;
  finally
    LoadList.Free
  end;
end;

function ConditionOfCheckAccountIPList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sCharName: string;
  sCharAccount: string;
  sCharIPaddr: string;
  sLine: string;
  sName: string;
  sIPaddr: string;
  sFileName: string;
begin
  Result := False;
  LoadList := TStringList.Create;
  try
    sCharName := PlayObject.m_sCharName;
    sCharAccount := PlayObject.m_sUserID;
    sCharIPaddr := PlayObject.m_sIPaddr;
    sFileName := QuestConditionInfo.sParam1;
    if FileExists(g_Config.sEnvirDir + sFileName) then
    begin
      LoadList.LoadFromFile(g_Config.sEnvirDir + sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLine := LoadList[I];
        if sLine[1] = ';' then
          Continue;
        sIPaddr := GetValidStr3(sLine, sName, [' ', '/', #9]);
        sIPaddr := Trim(sIPaddr);
        if (sName = sCharAccount) and (sIPaddr = sCharIPaddr) then
        begin
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    end;
  finally
    LoadList.Free
  end;
end;

function ConditionOfCheckSlaveCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nCount: Integer;
  cMethod: Char;
  SlaveObject: TBaseObject;
  sCharName, sSlaveName: string;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.nParam2 < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  sSlaveName := QuestConditionInfo.sParam3;
  if Length(sSlaveName) = 0 then
  begin
    cMethod := QuestConditionInfo.sParam1[1];
    case cMethod of
      '=':
        if BaseObject.m_SlaveList.Count = QuestConditionInfo.nParam2 then
          Result := True;
      '>':
        if BaseObject.m_SlaveList.Count > QuestConditionInfo.nParam2 then
          Result := True;
      '<':
        if BaseObject.m_SlaveList.Count < QuestConditionInfo.nParam2 then
          Result := True;
    else
      if BaseObject.m_SlaveList.Count >= QuestConditionInfo.nParam2 then
        Result := True;
    end;
  end
  else
  begin
    nCount := 0;
    for I := 0 to BaseObject.m_SlaveList.Count - 1 do
    begin
      SlaveObject := TBaseObject(BaseObject.m_SlaveList.Items[I]);
      sCharName := SlaveObject.m_sCharName;

      if QuestConditionInfo.nParam4 = 0 then
      begin
        sCharName := DelNumber(sCharName);
      end;

      if SameText(sCharName, sSlaveName) then
      begin
        Inc(nCount);
      end;
    end;

    cMethod := QuestConditionInfo.sParam1[1];
    case cMethod of
      '=':
        if nCount = QuestConditionInfo.nParam2 then
          Result := True;
      '>':
        if nCount > QuestConditionInfo.nParam2 then
          Result := True;
      '<':
        if nCount < QuestConditionInfo.nParam2 then
          Result := True;
    else
      if nCount >= QuestConditionInfo.nParam2 then
        Result := True;
    end;
  end;
end;

function ConditionOfIsNewHuman(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if BaseObject.m_btRaceServer = RC_HEROOBJECT then
  begin
    Result := THeroObject(BaseObject).m_boNewHero;
  end
  else
  begin
    Result := PlayObject.m_boNewHuman;
  end;
end;

function ConditionOfCheckMemberType(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nType: Integer;
  cMethod: Char;
begin
  Result := False;
  nType := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nType < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nMemberType = nType then
        Result := True;
    '>':
      if PlayObject.m_nMemberType > nType then
        Result := True;
    '<':
      if PlayObject.m_nMemberType < nType then
        Result := True;
  else
    if PlayObject.m_nMemberType >= nType then
      Result := True;
  end;
end;

function ConditionOfCheckMemBerLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nLevel: Integer;
  cMethod: Char;
begin
  Result := False;
  nLevel := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nMemberLevel = nLevel then
        Result := True;
    '>':
      if PlayObject.m_nMemberLevel > nLevel then
        Result := True;
    '<':
      if PlayObject.m_nMemberLevel < nLevel then
        Result := True;
  else
    if PlayObject.m_nMemberLevel >= nLevel then
      Result := True;
  end;
end;

function ConditionOfCheckGameGold(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nGameGold: Int64;
begin
  Result := False;
  nGameGold := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  if (QuestConditionInfo.sParam1 = '') or (nGameGold < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nGameGold = nGameGold then
        Result := True;
    '>':
      if PlayObject.m_nGameGold > nGameGold then
        Result := True;
    '<':
      if PlayObject.m_nGameGold < nGameGold then
        Result := True;
  else
    if PlayObject.m_nGameGold >= nGameGold then
      Result := True;
  end;
end;

function ConditionOfCheckGamePoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nGamePoint: Int64;
begin
  Result := False;
  nGamePoint := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  if (QuestConditionInfo.sParam1 = '') or (nGamePoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nGamePoint = nGamePoint then
        Result := True;
    '>':
      if PlayObject.m_nGamePoint > nGamePoint then
        Result := True;
    '<':
      if PlayObject.m_nGamePoint < nGamePoint then
        Result := True;
  else
    if PlayObject.m_nGamePoint >= nGamePoint then
      Result := True;
  end;
end;

function ConditionOfCheckNameListPostion(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sCharName: string;
  sFileName: string;
  nNamePostion, nPostion: Integer;
  sLine: string;
begin
  Result := False;
  sFileName := QuestConditionInfo.sParam1;
  nNamePostion := -1;
  LoadList := TStringList.Create;
  try
    sCharName := BaseObject.m_sCharName;
    if FileExists(g_Config.sEnvirDir + sFileName) then
    begin
      LoadList.LoadFromFile(g_Config.sEnvirDir + sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLine := Trim(LoadList[I]);
        if sLine[1] = ';' then
          Continue;
        if CompareText(sLine, sCharName) = 0 then
        begin
          nNamePostion := I;
          Break;
        end;
      end;
    end
    else
    begin
      Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    end;
  finally
    LoadList.Free
  end;
  nPostion := QuestConditionInfo.nParam2;

  if nPostion < 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if nNamePostion >= nPostion then
    Result := True;
end;

function ConditionOfCheckGuildList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (PlayObject = nil) or (PlayObject.m_MyGuild = nil) then
    Exit;
  Result := CheckStringList(TGUild(PlayObject.m_MyGuild).sGuildName, Npc.m_sPath + QuestConditionInfo.sParam1, False, False);
end;

function ConditionOfCheckReNewLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nLevel: Integer;
  cMethod: Char;
  PlayObj: TPlayObject;
  HeroObj: THeroObject;
begin
  Result := False;

  if (PlayObject = nil) then
    Exit;

  nLevel := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  cMethod := QuestConditionInfo.sParam1[1];

  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
    PlayObj := BaseObject as TPlayObject;
    case cMethod of
      '=':
        if PlayObj.m_btReLevel = nLevel then
          Result := True;
      '>':
        if PlayObj.m_btReLevel > nLevel then
          Result := True;
      '<':
        if PlayObj.m_btReLevel < nLevel then
          Result := True;
    else
      if PlayObj.m_btReLevel >= nLevel then
        Result := True;
    end;
  end
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
  begin
    HeroObj := BaseObject as THeroObject;
    case cMethod of
      '=':
        if HeroObj.m_btReLevel = nLevel then
          Result := True;
      '>':
        if HeroObj.m_btReLevel > nLevel then
          Result := True;
      '<':
        if HeroObj.m_btReLevel < nLevel then
          Result := True;
    else
      if HeroObj.m_btReLevel >= nLevel then
        Result := True;
    end;
  end;
end;

function ConditionOfCheckSlaveLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  nLevel: Integer;
  cMethod: Char;
  SlaveObject: TBaseObject;
  nSlaveLevel: Integer;
begin
  Result := False;
  nLevel := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  nSlaveLevel := -1;
  for I := 0 to BaseObject.m_SlaveList.Count - 1 do
  begin
    SlaveObject := TBaseObject(BaseObject.m_SlaveList.Items[I]);
    if SlaveObject.m_btSlaveExpLevel > nSlaveLevel then
      nSlaveLevel := SlaveObject.m_btSlaveExpLevel;
  end;
  if nSlaveLevel < 0 then
    Exit;
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nSlaveLevel = nLevel then
        Result := True;
    '>':
      if nSlaveLevel > nLevel then
        Result := True;
    '<':
      if nSlaveLevel < nLevel then
        Result := True;
  else
    if nSlaveLevel >= nLevel then
      Result := True;
  end;
end;

function ConditionOfCheckSlaveName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  sSlaveName, sTempName: string;
  SlaveObject: TBaseObject;
begin
  Result := False;
  if QuestConditionInfo.sParam1 = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  sSlaveName := DelNumber(QuestConditionInfo.sParam1);

  for I := 0 to BaseObject.m_SlaveList.Count - 1 do
  begin
    SlaveObject := TBaseObject(BaseObject.m_SlaveList.Items[I]);
    sTempName := DelNumber(SlaveObject.m_sCharName);
    if CompareText(sSlaveName, sTempName) = 0 then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function ConditionOfCheckCreditPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCreditPoint: Int64;
  cMethod: Char;
begin
  Result := False;
  nCreditPoint := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  if (QuestConditionInfo.sParam1 = '') or (nCreditPoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if BaseObject.m_WAbil.CreditPoint = nCreditPoint then
        Result := True;
    '>':
      if BaseObject.m_WAbil.CreditPoint > nCreditPoint then
        Result := True;
    '<':
      if BaseObject.m_WAbil.CreditPoint < nCreditPoint then
        Result := True;
  else
    if BaseObject.m_WAbil.CreditPoint >= nCreditPoint then
      Result := True;
  end;
end;

function ConditionOfCheckOfGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if QuestConditionInfo.sParam1 = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if (PlayObject.m_MyGuild <> nil) then
  begin
    if CompareText(TGUild(PlayObject.m_MyGuild).sGuildName, QuestConditionInfo.sParam1) = 0 then
    begin
      Result := True;
    end;
  end;
end;

function ConditionOfCheckPayMent(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPayMent: Integer;
begin
  Result := False;
  nPayMent := QuestConditionInfo.nParam1;
  if nPayMent < 1 then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_nPayMent = nPayMent then
    Result := True;
end;

function ConditionOfCheckUseItem(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nWhere: Integer;
  StdItem: pTStdItem;
  sItemName: string;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;
  sItemName := QuestConditionInfo.sParam2;
  nWhere := QuestConditionInfo.nParam1;

  if (nWhere < 0) or (nWhere > U_GODBLESSITEM12) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    if sItemName <> '' then
    begin
      StdItem := UserEngine.GetStdItem(SmartObject.m_UseItems[nWhere].wIndex);
      if (StdItem <> nil) and (CompareText(StdItem.Name, sItemName) = 0) then
        Result := True;
    end
    else
      Result := SmartObject.m_UseItems[nWhere].wIndex > 0;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    Result := SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1].wIndex > 0;

    if Result and (sItemName <> '') then
    begin
      StdItem := UserEngine.GetStdItem(SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1].wIndex);
      Result := (StdItem <> nil) and (CompareText(StdItem.Name, sItemName) = 0);
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    Result := SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1].wIndex > 0;

    if Result and (sItemName <> '') then
    begin
      StdItem := UserEngine.GetStdItem(SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1].wIndex);
      Result := (StdItem <> nil) and (CompareText(StdItem.Name, sItemName) = 0);
    end;
  end;
end;

function ConditionOfCheckBagSize(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nSize, nMaxSize: Integer;
begin
  Result := False;
  nMaxSize := 0;
  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
    nMaxSize := BaseObject.GetMaxBagCount;
  end
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
  begin
    nMaxSize := THeroObject(BaseObject).m_nBagCount;
  end;

  nSize := QuestConditionInfo.nParam1;
  if (nSize <= 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if BaseObject.m_ItemList.Count + nSize <= nMaxSize then
    Result := True;
end;

function ConditionOfCheckListCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
end;

function ConditionOfCheckDC(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethodMin, cMethodMax: Char;
  nMIN, nMax: Integer;

  function CheckHigh(): Boolean;
  begin
    Result := False;
    case cMethodMax of
      '=':
        begin
          if BaseObject.m_WAbil.DC2 = nMax then
          begin
            Result := True;
          end;
        end;
      '>':
        begin
          if BaseObject.m_WAbil.DC2 > nMax then
          begin
            Result := True;
          end;
        end;
      '<':
        begin
          if BaseObject.m_WAbil.DC2 < nMax then
          begin
            Result := True;
          end;
        end;
    else
      begin
        if BaseObject.m_WAbil.DC2 >= nMax then
        begin
          Result := True;
        end;
      end;
    end;
  end;

begin
  Result := False;
  nMIN := QuestConditionInfo.nParam2;
  nMax := QuestConditionInfo.nParam4;

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.sParam3 = '') or (nMIN < 0) or (nMax < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethodMin := QuestConditionInfo.sParam1[1];
  cMethodMax := QuestConditionInfo.sParam3[1];

  case cMethodMin of
    '=':
      begin
        if (BaseObject.m_WAbil.DC1 = nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '>':
      begin
        if (BaseObject.m_WAbil.DC1 > nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '<':
      begin
        if (BaseObject.m_WAbil.DC1 < nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
  else
    begin
      if (BaseObject.m_WAbil.DC1 >= nMIN) then
      begin
        Result := CheckHigh;
      end;
    end;
  end;
end;

function ConditionOfCheckMC(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethodMin, cMethodMax: Char;
  nMIN, nMax: Integer;

  function CheckHigh(): Boolean;
  begin
    Result := False;
    case cMethodMax of
      '=':
        begin
          if BaseObject.m_WAbil.MC2 = nMax then
          begin
            Result := True;
          end;
        end;
      '>':
        begin
          if BaseObject.m_WAbil.MC2 > nMax then
          begin
            Result := True;
          end;
        end;
      '<':
        begin
          if BaseObject.m_WAbil.MC2 < nMax then
          begin
            Result := True;
          end;
        end;
    else
      begin
        if BaseObject.m_WAbil.MC2 >= nMax then
        begin
          Result := True;
        end;
      end;
    end;
  end;

begin
  Result := False;
  nMIN := QuestConditionInfo.nParam2;
  nMax := QuestConditionInfo.nParam4;

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.sParam3 = '') or (nMIN < 0) or (nMax < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethodMin := QuestConditionInfo.sParam1[1];
  cMethodMax := QuestConditionInfo.sParam3[1];
  case cMethodMin of
    '=':
      begin
        if (BaseObject.m_WAbil.MC1 = nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '>':
      begin
        if (BaseObject.m_WAbil.MC1 > nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '<':
      begin
        if (BaseObject.m_WAbil.MC1 < nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
  else
    begin
      if (BaseObject.m_WAbil.MC1 >= nMIN) then
      begin
        Result := CheckHigh;
      end;
    end;
  end;
end;

function ConditionOfCheckSC(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethodMin, cMethodMax: Char;
  nMIN, nMax: Integer;

  function CheckHigh(): Boolean;
  begin
    Result := False;
    case cMethodMax of
      '=':
        begin
          if BaseObject.m_WAbil.SC2 = nMax then
          begin
            Result := True;
          end;
        end;
      '>':
        begin
          if BaseObject.m_WAbil.SC2 > nMax then
          begin
            Result := True;
          end;
        end;
      '<':
        begin
          if BaseObject.m_WAbil.SC2 < nMax then
          begin
            Result := True;
          end;
        end;
    else
      begin
        if BaseObject.m_WAbil.SC2 >= nMax then
        begin
          Result := True;
        end;
      end;
    end;
  end;

begin
  Result := False;
  nMIN := QuestConditionInfo.nParam2;
  nMax := QuestConditionInfo.nParam4;

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.sParam3 = '') or (nMIN < 0) or (nMax < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethodMin := QuestConditionInfo.sParam1[1];
  cMethodMax := QuestConditionInfo.sParam3[1];
  case cMethodMin of
    '=':
      begin
        if (BaseObject.m_WAbil.SC1 = nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '>':
      begin
        if (BaseObject.m_WAbil.SC1 > nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '<':
      begin
        if (BaseObject.m_WAbil.SC1 < nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
  else
    begin
      if (BaseObject.m_WAbil.SC1 >= nMIN) then
      begin
        Result := CheckHigh;
      end;
    end;
  end;
end;

function ConditionOfCheckHP(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethodMin, cMethodMax: Char;
  nMIN, nMax: Int64;

  function CheckHigh(): Boolean;
  begin
    Result := False;
    case cMethodMax of
      '=':
        begin
          if BaseObject.m_WAbil.MaxHP = nMax then
          begin
            Result := True;
          end;
        end;
      '>':
        begin
          if BaseObject.m_WAbil.MaxHP > nMax then
          begin
            Result := True;
          end;
        end;
      '<':
        begin
          if BaseObject.m_WAbil.MaxHP < nMax then
          begin
            Result := True;
          end;
        end;
    else
      begin
        if BaseObject.m_WAbil.MaxHP >= nMax then
        begin
          Result := True;
        end;
      end;
    end;
  end;

begin
  Result := False;
  nMIN := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  nMax := StrToInt64Def(QuestConditionInfo.sParam4, 0);

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.sParam3 = '') or (nMIN < 0) or (nMax < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethodMin := QuestConditionInfo.sParam1[1];
  cMethodMax := QuestConditionInfo.sParam3[1];
  case cMethodMin of
    '=':
      begin
        if (BaseObject.m_WAbil.HP = nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '>':
      begin
        if (BaseObject.m_WAbil.HP > nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '<':
      begin
        if (BaseObject.m_WAbil.HP < nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
  else
    begin
      if (BaseObject.m_WAbil.HP >= nMIN) then
      begin
        Result := CheckHigh;
      end;
    end;
  end;
end;

function ConditionOfCheckMP(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethodMin, cMethodMax: Char;
  nMIN, nMax: Int64;

  function CheckHigh(): Boolean;
  begin
    Result := False;
    case cMethodMax of
      '=':
        begin
          if BaseObject.m_WAbil.MaxHP = nMax then
          begin
            Result := True;
          end;
        end;
      '>':
        begin
          if BaseObject.m_WAbil.MaxHP > nMax then
          begin
            Result := True;
          end;
        end;
      '<':
        begin
          if BaseObject.m_WAbil.MaxHP < nMax then
          begin
            Result := True;
          end;
        end;
    else
      begin
        if BaseObject.m_WAbil.MaxHP >= nMax then
        begin
          Result := True;
        end;
      end;
    end;
  end;

begin
  Result := False;
  nMIN := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  nMax := StrToInt64Def(QuestConditionInfo.sParam4, 0);

  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.sParam3 = '') or (nMIN < 0) or (nMax < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethodMin := QuestConditionInfo.sParam1[1];
  cMethodMax := QuestConditionInfo.sParam3[1];
  case cMethodMin of
    '=':
      begin
        if (BaseObject.m_WAbil.MP = nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '>':
      begin
        if (BaseObject.m_WAbil.MP > nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
    '<':
      begin
        if (BaseObject.m_WAbil.MP < nMIN) then
        begin
          Result := CheckHigh;
        end;
      end;
  else
    begin
      if (BaseObject.m_WAbil.MP >= nMIN) then
      begin
        Result := CheckHigh;
      end;
    end;
  end;
end;

function ConditionOfCheckItemType(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nWhere, nBoxItemIndex: Integer;
  nType: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  SmartObject: TSmartObject;
  I, nMakeIndex: Integer;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nType := QuestConditionInfo.nParam2;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
    for I := 0 to PlayObject.m_ItemList.Count - 1 do
    begin
      if PlayObject.m_ItemList.Items[I] = PlayObject.m_UpgradeItem then
      begin
        UserItem := PlayObject.m_UpgradeItem;
        Break;
      end;
    end;
  end;

  if (UserItem <> nil) and (UserItem.wIndex > 0) then
  begin
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    Result := (StdItem <> nil) and (StdItem.StdMode = nType);
  end;
end;

function ConditionOfCheckExp(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  dwExp: LongWord;
  nExp: Integer;
begin
  Result := False;
  nExp := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nExp = -1) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  dwExp := LongWord(nExp);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if BaseObject.m_Abil.Exp = dwExp then
        Result := True;
    '>':
      if BaseObject.m_Abil.Exp > dwExp then
        Result := True;
    '<':
      if BaseObject.m_Abil.Exp < dwExp then
        Result := True;
  else
    if BaseObject.m_Abil.Exp >= dwExp then
      Result := True;
  end;
end;

function ConditionOfCheckCastleGold(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nGold: Integer;
begin
  Result := False;
  nGold := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nGold < 0) or (Npc.m_Castle = nil) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if TUserCastle(Npc.m_Castle).m_nTotalGold = nGold then
        Result := True;
    '>':
      if TUserCastle(Npc.m_Castle).m_nTotalGold > nGold then
        Result := True;
    '<':
      if TUserCastle(Npc.m_Castle).m_nTotalGold < nGold then
        Result := True;
  else
    if TUserCastle(Npc.m_Castle).m_nTotalGold >= nGold then
      Result := True;
  end;
end;

function ConditionOfCheckPasswordErrorCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nErrorCount: Integer;
  cMethod: Char;
begin
  Result := False;

  nErrorCount := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nErrorCount < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_btPwdFailCount = nErrorCount then
        Result := True;
    '>':
      if PlayObject.m_btPwdFailCount > nErrorCount then
        Result := True;
    '<':
      if PlayObject.m_btPwdFailCount < nErrorCount then
        Result := True;
  else
    if PlayObject.m_btPwdFailCount >= nErrorCount then
      Result := True;
  end;
end;

function ConditionOfIsLockPassword(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject <> nil) and PlayObject.m_boPasswordLocked;
end;

function ConditionOfIsLockStorage(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject <> nil) and (not PlayObject.m_boCanGetBackItem);
end;

function ConditionOfCheckGuildBuildPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nPoint: Integer;
  Guild: TGUild;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_MyGuild = nil then
  begin
    Exit;
  end;
  Guild := TGUild(PlayObject.m_MyGuild);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Guild.nBuildPoint = nPoint then
        Result := True;
    '>':
      if Guild.nBuildPoint > nPoint then
        Result := True;
    '<':
      if Guild.nBuildPoint < nPoint then
        Result := True;
  else
    if Guild.nBuildPoint >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckGuildAuraePoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nPoint: Integer;
  Guild: TGUild;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_MyGuild = nil then
  begin
    Exit;
  end;
  Guild := TGUild(PlayObject.m_MyGuild);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Guild.nAurae = nPoint then
        Result := True;
    '>':
      if Guild.nAurae > nPoint then
        Result := True;
    '<':
      if Guild.nAurae < nPoint then
        Result := True;
  else
    if Guild.nAurae >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckStabilityPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nPoint: Integer;
  Guild: TGUild;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_MyGuild = nil then
  begin
    Exit;
  end;
  Guild := TGUild(PlayObject.m_MyGuild);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Guild.nStability = nPoint then
        Result := True;
    '>':
      if Guild.nStability > nPoint then
        Result := True;
    '<':
      if Guild.nStability < nPoint then
        Result := True;
  else
    if Guild.nStability >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckFlourishPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nPoint: Integer;
  Guild: TGUild;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_MyGuild = nil then
  begin
    Exit;
  end;
  Guild := TGUild(PlayObject.m_MyGuild);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Guild.nFlourishing = nPoint then
        Result := True;
    '>':
      if Guild.nFlourishing > nPoint then
        Result := True;
    '<':
      if Guild.nFlourishing < nPoint then
        Result := True;
  else
    if Guild.nFlourishing >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckContribution(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nContribution: Integer;
  cMethod: Char;
begin
  Result := False;
  nContribution := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nContribution < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_wContribution = nContribution then
        Result := True;
    '>':
      if PlayObject.m_wContribution > nContribution then
        Result := True;
    '<':
      if PlayObject.m_wContribution < nContribution then
        Result := True;
  else
    if PlayObject.m_wContribution >= nContribution then
      Result := True;
  end;
end;

function ConditionOfCheckRangeMonCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  sMapName: string;
  nX, nY, nRange, nCount: Integer;
  cMethod: Char;
  nMapRangeCount: Integer;
  Envir: TEnvirnoment;
  MonList: TList;
  AObject: TBaseObject;
begin
  Result := False;
  sMapName := QuestConditionInfo.sParam1;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);

  if Envir = nil then
    Envir := g_MapManager.FindMap(sMapName)
  else
    sMapName := Envir.sMapName;

  nX := QuestConditionInfo.nParam2;
  nY := QuestConditionInfo.nParam3;
  nRange := QuestConditionInfo.nParam4;
  nCount := QuestConditionInfo.nParam6;

  if CompareText(sMapName, 'Self') = 0 then
  begin
    sMapName := BaseObject.m_sMapName;
    Envir := g_MapManager.FindMap(sMapName);
    nX := BaseObject.m_nCurrX;
    nY := BaseObject.m_nCurrY;
  end;

  if (QuestConditionInfo.sParam5 = '') or (Envir = nil) or (nX < 0) or (nY < 0) or (nRange < 0) or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam5[1];

  MonList := TList.Create;
  Envir.GetRangeBaseObject(nX, nY, nRange, True, MonList);
  for I := MonList.Count - 1 downto 0 do
  begin
    if MonList.Count <= 0 then
      Break;
    AObject := TBaseObject(MonList.Items[I]);
    if (AObject.m_btRaceServer < RC_ANIMAL) or (AObject.m_btRaceServer = RC_ARCHERGUARD) or (AObject.m_Master <> nil) or (AObject.m_btRaceServer = RC_NPC) or (AObject.m_btRaceServer = RC_PEACENPC) then
      MonList.Delete(I);
  end;
  nMapRangeCount := MonList.Count;
  case cMethod of
    '=':
      if nMapRangeCount = nCount then
        Result := True;
    '>':
      if nMapRangeCount > nCount then
        Result := True;
    '<':
      if nMapRangeCount < nCount then
        Result := True;
  else
    if nMapRangeCount >= nCount then
      Result := True;
  end;
  MonList.Free;
end;

function ConditionOfCheckRangeMonCountEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  sMapName, sMonName: string;
  nX, nY, nRange, nCount: Integer;
  cMethod: Char;
  nMapRangeCount: Integer;
  Envir: TEnvirnoment;
  MonList: TList;
  AObject: TBaseObject;
begin
  Result := False;
  sMapName := QuestConditionInfo.sParam1;

  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1)
  else
    sMapName := Envir.sMapName;

  sMonName := QuestConditionInfo.sParam2;
  nX := QuestConditionInfo.nParam3;
  nY := QuestConditionInfo.nParam4;
  nRange := QuestConditionInfo.nParam5;
  nCount := QuestConditionInfo.nParam7;

  if CompareText(sMapName, 'Self') = 0 then
  begin
    sMapName := BaseObject.m_sMapName;
    Envir := g_MapManager.FindMap(sMapName);
    nX := BaseObject.m_nCurrX;
    nY := BaseObject.m_nCurrY;
  end;

  if (QuestConditionInfo.sParam6 = '') or (Envir = nil) or (nX < 0) or (nY < 0) or (nRange < 0) or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethod := QuestConditionInfo.sParam6[1];

  MonList := TList.Create;
  Envir.GetRangeBaseObject(nX, nY, nRange, True, MonList);

  if QuestConditionInfo.nParam8 = 0 then
  begin
    for I := MonList.Count - 1 downto 0 do
    begin
      if MonList.Count <= 0 then
        Break;
      AObject := TBaseObject(MonList.Items[I]);
      if (CompareText(AObject.m_sCharName, sMonName) <> 0) or (AObject.m_btRaceServer < RC_ANIMAL) or (AObject.m_btRaceServer = RC_ARCHERGUARD) or (AObject.m_Master <> nil) or (AObject.m_btRaceServer = RC_NPC) or (AObject.m_btRaceServer = RC_PEACENPC) then
        MonList.Delete(I);
    end;
  end
  else
  begin
    for I := MonList.Count - 1 downto 0 do
    begin
      if MonList.Count <= 0 then
        Break;
      AObject := TBaseObject(MonList.Items[I]);
      if (CompareText(AObject.m_sCharName, sMonName) <> 0) or (AObject.m_btRaceServer < RC_ANIMAL) or (AObject.m_btRaceServer = RC_ARCHERGUARD) or (AObject.m_btRaceServer = RC_NPC) or (AObject.m_btRaceServer = RC_PEACENPC) then
        MonList.Delete(I);
    end;
  end;

  nMapRangeCount := MonList.Count;
  case cMethod of
    '=':
      if nMapRangeCount = nCount then
        Result := True;
    '>':
      if nMapRangeCount > nCount then
        Result := True;
    '<':
      if nMapRangeCount < nCount then
        Result := True;
  else
    if nMapRangeCount >= nCount then
      Result := True;
  end;
  MonList.Free;
end;

function ConditionOfCheckItemAddValueEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  I, nBoxItemIndex, nMakeIndex: Integer;
  nWhere, nValue, nPoint: Integer;
  StdItem: pTStdItem;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;

  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nValue := QuestConditionInfo.nParam3;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (Length(QuestConditionInfo.sParam2) = 0) or (nValue < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    if (UserItem.wIndex <= 0) then
      Exit;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    if (UserItem.wIndex <= 0) then
      Exit;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    if (UserItem.wIndex <= 0) then
      Exit;
  end
  else
  begin
    for I := 0 to PlayObject.m_ItemList.Count - 1 do
    begin
      if PlayObject.m_ItemList.Items[I] = PlayObject.m_UpgradeItem then
      begin
        UserItem := PlayObject.m_UpgradeItem;
        Break;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    cMethod := QuestConditionInfo.sParam2[1];
    nPoint := 0;
    if QuestConditionInfo.nParam4 = 1 then
    begin
      for I := 0 to Length(UserItem.btNewValue) - 1 do
        nPoint := nPoint + UserItem.btNewValue[I];
    end
    else
    begin
      nPoint := 0;
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem <> nil then
      begin
        case StdItem.StdMode of
          5, 6, 68, 69:
            begin
              nPoint := UserItem.btValue[0] + UserItem.btValue[1] + UserItem.btValue[2] + UserItem.btValue[5] + UserItem.btValue[6];
            end;

          10, 11, 12, 15, 16, 19, 20, 21, 22, 23, 24, 26, 28, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 66, 67, 75, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90:
            begin
              nPoint := UserItem.btValue[0] + UserItem.btValue[1] + UserItem.btValue[2] + UserItem.btValue[3] + UserItem.btValue[4];
            end;
        end;
      end;
    end;

    case cMethod of
      '=':
        Result := nPoint = nValue;
      '>':
        Result := nPoint > nValue;
      '<':
        Result := nPoint < nValue;
    else
      Result := nPoint >= nValue;
    end;
  end;
end;

function ConditionOfCheckGroupMemberCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.nParam2 < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethod := QuestConditionInfo.sParam1[1];
  if PlayObject.m_GroupOwner <> nil then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      PlayObject.m_GroupOwner.m_GroupMembers.LockR(10);
    try
{$IFEND}
      case cMethod of
        '=':
          if PlayObject.m_GroupOwner.m_GroupMembers.Count = QuestConditionInfo.nParam2 then
            Result := True;
        '>':
          if PlayObject.m_GroupOwner.m_GroupMembers.Count > QuestConditionInfo.nParam2 then
            Result := True;
        '<':
          if PlayObject.m_GroupOwner.m_GroupMembers.Count < QuestConditionInfo.nParam2 then
            Result := True;
      else
        if PlayObject.m_GroupOwner.m_GroupMembers.Count >= QuestConditionInfo.nParam2 then
          Result := True;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        PlayObject.m_GroupOwner.m_GroupMembers.UnLockR;
    end;
{$IFEND}
  end;
end;

function ConditionOfCheckItemAddValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nBoxItemIndex, nMakeIndex: Integer;
  nWhere: Integer;
  nType: Integer;
  nValue: Integer;
  nPoint: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nType := QuestConditionInfo.nParam2;
  nValue := QuestConditionInfo.nParam4;

  // 扩展装备位置 chongchong 2014-04-26
  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (QuestConditionInfo.sParam3 = '') or (nValue < 0) or (not (nType in [0..14])) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else
  begin
    for I := 0 to PlayObject.m_ItemList.Count - 1 do
    begin
      if PlayObject.m_ItemList.Items[I] = PlayObject.m_UpgradeItem then
      begin
        UserItem := PlayObject.m_UpgradeItem;
        Break;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    cMethod := QuestConditionInfo.sParam3[1];

    if nType = 14 then
      nPoint := UserItem.DuraMax
    else
      nPoint := UserItem.btValue[nType];

    case cMethod of
      '=':
        if nPoint = nValue then
          Result := True;
      '>':
        if nPoint > nValue then
          Result := True;
      '<':
        if nPoint < nValue then
          Result := True;
    else
      if nPoint >= nValue then
        Result := True;
    end;
  end;
end;

(*
  function ConditionOfCheckInMapRange(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
  var
  sMapName: string;
  nX, nY, nRange: Integer;
  Envir: TEnvirnoment;
  begin
  Result := False;
  sMapName := QuestConditionInfo.sParam1;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
  Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
  Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1)
  else
  sMapName := Envir.sMapName;

  nX := QuestConditionInfo.nParam2;
  nY := QuestConditionInfo.nParam3;
  nRange := QuestConditionInfo.nParam4;

  if (Envir = nil) or (nX < 0) or (nY < 0) or (nRange < 0) then
  begin
  Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
  Exit;
  end;
  if CompareText(BaseObject.m_sMapName, sMapName) <> 0 then Exit;
  if (abs(BaseObject.m_nCurrX - nX) <= nRange) and (abs(BaseObject.m_nCurrY - nY) <= nRange) then
  Result := True;
  end;
*)

{ TODO -ochongchong -c修改 : 副本地图 -- 加入副本地图检测 【2013-09-10】 }

function ConditionOfCheckInMapRange(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  IsCheckFB: Boolean;
  sMapName: string;
  nX, nY, nRange: Integer;
  Envir: TEnvirnoment;
  Index: Integer;
  FBList: TList;
begin
  Result := False;
  IsCheckFB := (Length(QuestConditionInfo.sParam1) > 0) and (QuestConditionInfo.sParam1[1] = '$');
  if IsCheckFB then
    sMapName := Copy(QuestConditionInfo.sParam1, 2, MaxInt)
  else
    sMapName := QuestConditionInfo.sParam1;

  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
  begin
    if not IsCheckFB then
      Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1)
    else
    begin
      Index := g_FBMapManager.IndexOf(sMapName);
      if Index <> -1 then
      begin
        FBList := TList(g_FBMapManager.Objects[Index]);
        if FBList.Count > 0 then
          Envir := TEnvirnoment(FBList[0]);
      end;
    end;
  end;
  if Envir = nil then
  begin
    if not IsCheckFB then
      Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1)
    else
    begin
      Index := g_FBMapManager.IndexOf(sMapName);
      if Index <> -1 then
      begin
        FBList := TList(g_FBMapManager.Objects[Index]);
        if FBList.Count > 0 then
          Envir := TEnvirnoment(FBList[0]);
      end;
    end;
  end
  else
  begin
    if Envir.m_boFB then
      sMapName := Envir.m_sFBName
    else
      sMapName := Envir.sMapName;
  end;

  nX := QuestConditionInfo.nParam2;
  nY := QuestConditionInfo.nParam3;
  nRange := QuestConditionInfo.nParam4;

  if (Envir = nil) or (nX < 0) or (nY < 0) or (nRange < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not IsCheckFB then
  begin
    if CompareText(BaseObject.m_sMapName, sMapName) <> 0 then
      Exit;
  end
  else
  begin
    if BaseObject.m_PEnvir = nil then
      Exit;
    if CompareText(BaseObject.m_PEnvir.m_sFBName, sMapName) <> 0 then
      Exit;
  end;
  if (abs(BaseObject.m_nCurrX - nX) <= nRange) and (abs(BaseObject.m_nCurrY - nY) <= nRange) then
    Result := True;
end;

function ConditionOfCheckCastleChangeDay(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nDay: Integer;
  cMethod: Char;
  nChangeDay: Integer;
begin
  Result := False;
  nDay := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nDay < 0) or (Npc.m_Castle = nil) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  nChangeDay := GetDayCount(Now, TUserCastle(Npc.m_Castle).m_ChangeDate);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nChangeDay = nDay then
        Result := True;
    '>':
      if nChangeDay > nDay then
        Result := True;
    '<':
      if nChangeDay < nDay then
        Result := True;
  else
    if nChangeDay >= nDay then
      Result := True;
  end;
end;

function ConditionOfCheckCastleWarDay(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nDay: Integer;
  cMethod: Char;
  nWarDay: Integer;
begin
  Result := False;
  nDay := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nDay < 0) or (Npc.m_Castle = nil) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  nWarDay := GetDayCount(Now, TUserCastle(Npc.m_Castle).m_WarDate);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nWarDay = nDay then
        Result := True;
    '>':
      if nWarDay > nDay then
        Result := True;
    '<':
      if nWarDay < nDay then
        Result := True;
  else
    if nWarDay >= nDay then
      Result := True;
  end;
end;

function ConditionOfCheckOnlineLongMin(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nOnlineMin: Integer;
  nOnlineTime: Integer;
begin
  Result := False;
  nOnlineMin := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nOnlineMin < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    nOnlineTime := (MyGetTickCount - PlayObject.m_dwLogonTick) div 60000
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    nOnlineTime := (MyGetTickCount - THeroObject(BaseObject).m_dwLogonTick) div 60000
  else
    nOnlineTime := 0;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nOnlineTime = nOnlineMin then
        Result := True;
    '>':
      if nOnlineTime > nOnlineMin then
        Result := True;
    '<':
      if nOnlineTime < nOnlineMin then
        Result := True;
  else
    if nOnlineTime >= nOnlineMin then
      Result := True;
  end;
end;

function ConditionOfCheckOnline(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  Result := UserEngine.GetPlayObject(QuestConditionInfo.sParam1) <> nil;
end;

function ConditionOfCheckChiefItemCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nCount: Integer;
  Guild: TGUild;
begin
  Result := False;

  nCount := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if PlayObject.m_MyGuild = nil then
  begin
    Exit;
  end;
  Guild := TGUild(PlayObject.m_MyGuild);
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Guild.nChiefItemCount = nCount then
        Result := True;
    '>':
      if Guild.nChiefItemCount > nCount then
        Result := True;
    '<':
      if Guild.nChiefItemCount < nCount then
        Result := True;
  else
    if Guild.nChiefItemCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckNameDateList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sListFileName, sLineText, sHumName, sDate: string;
  boDeleteExprie, boNoCompareHumanName: Boolean;
  dOldDate: TDateTime;
  cMethod: Char;
  nDayCount, nDay: Integer;
begin
  Result := False;

  nDayCount := QuestConditionInfo.nParam3;
  boDeleteExprie := CompareText(QuestConditionInfo.sParam6, '清理') = 0;
  boNoCompareHumanName := QuestConditionInfo.sParam6 = '1';

  if (QuestConditionInfo.sParam2 = '') or (nDayCount < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  cMethod := QuestConditionInfo.sParam2[1];
  sListFileName := g_Config.sEnvirDir + Npc.m_sPath + QuestConditionInfo.sParam1;
  if FileExists(sListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := Trim(LoadList[I]);
      sLineText := GetValidStr3(sLineText, sHumName, [' ', #9]);
      sLineText := GetValidStr3(sLineText, sDate, [' ', #9]);
      if (CompareText(sHumName, PlayObject.m_sCharName) = 0) or boNoCompareHumanName then
      begin
        nDay := High(Integer);
        if TryStrToDateTime(sDate, dOldDate) then
          nDay := GetDayCount(Now, dOldDate);

        case cMethod of
          '=':
            if nDay = nDayCount then
              Result := True;
          '>':
            if nDay > nDayCount then
              Result := True;
          '<':
            if nDay < nDayCount then
              Result := True;
        else
          if nDay >= nDayCount then
            Result := True;
        end;

        if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam4, IntToStr(nDay), nDay) then
        begin
          Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
          Break;
        end;

        if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam5, IntToStr(nDayCount - nDay), nDayCount - nDay) then
        begin
          Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
          Break;
        end;

        if not Result then
        begin
          if boDeleteExprie then
          begin
            LoadList.Delete(I);
            try
              LoadList.SaveToFile(sListFileName);
            except
              MainOutMessage('Save fail.... => ' + sListFileName);
            end;
          end;
        end;
        Break;
      end;
    end;
    LoadList.Free;
  end
  else
  begin
    MainOutMessage('file not found => ' + sListFileName);
  end;
end;

function ConditionOfCheckMapHumanCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount, nHumanCount: Integer;
  cMethod: Char;
  Envir: TEnvirnoment;
begin
  Result := False;
  nCount := QuestConditionInfo.nParam3;

  if (QuestConditionInfo.sParam2 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  if Envir = nil then
    Exit;

  nHumanCount := UserEngine.GetMapHuman(Envir);
  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      if nHumanCount = nCount then
        Result := True;
    '>':
      if nHumanCount > nCount then
        Result := True;
    '<':
      if nHumanCount < nCount then
        Result := True;
  else
    if nHumanCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckMapMonCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount, nMonCount: Integer;
  cMethod: Char;
  Envir: TEnvirnoment;
  I: Integer;
  MonList: TList;
  AObject: TBaseObject;
  IsExclueBB: Boolean;
begin
  Result := False;
  nCount := QuestConditionInfo.nParam3;

  // 排除BB chongchong 2014-03-17
  IsExclueBB := QuestConditionInfo.nParam4 <> 0;

  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  if (QuestConditionInfo.sParam2 = '') or (nCount < 0) or (Envir = nil) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if Envir = nil then
    Exit;

  if not IsExclueBB then
    nMonCount := UserEngine.GetMapMonster(Envir, nil)
  else
  begin
    MonList := TList.Create;
    UserEngine.GetMapMonster(Envir, MonList);
    for I := MonList.Count - 1 downto 0 do
    begin
      if MonList.Count <= 0 then
        Break;
      AObject := TBaseObject(MonList.Items[I]);
      if (AObject.Master <> nil) then
        MonList.Delete(I);
    end;
    nMonCount := MonList.Count;
    MonList.Free;
  end;

  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      if nMonCount = nCount then
        Result := True;
    '>':
      if nMonCount > nCount then
        Result := True;
    '<':
      if nMonCount < nCount then
        Result := True;
  else
    if nMonCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckVar(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  sType: string;
  sVarName: string;
  sVarValue: string;
  nVarValue: Integer;
  sName: string;
  sMethod: string;
  cMethod: Char;
  DynamicVar: pTDynamicVar;
  boFoundVar: Boolean;
  DynamicVarList: TList;
  BasePlayer: TPlayObject;
resourcestring
  sVarFound = '变量%s已存在，变量类型:%s';
  sVarTypeError = '变量类型错误，错误类型:%s 当前支持类型(HUMAN、GUILD、GLOBAL)';
begin
  Result := False;

  // 修改函数以支持H. M. .... chongchong 2013-11-22

  sType := QuestConditionInfo.sParam1;
  sVarName := QuestConditionInfo.sParam2;
  sMethod := QuestConditionInfo.sParam3;
  nVarValue := QuestConditionInfo.nParam4;
  sVarValue := QuestConditionInfo.sParam4;

  if BaseObject.m_btRaceServer <> RC_PLAYOBJECT then
    Exit;

  BasePlayer := TPlayObject(BaseObject);

  // CHECKVAR GLOBAL 庄家姓名 = 暂无
  // MainOutMessage(Format('sParam1:%s sParam2:%s sParam3:%s sParam4:%s',
  // [QuestConditionInfo.sParam1, QuestConditionInfo.sParam2, QuestConditionInfo.sParam3, QuestConditionInfo.sParam4]));
  // [脚本参数不正确] Cmd:CHECKVAR NPC名称:QManage 地图:0 座标:0:0 参数1:HUMAN 参数2:RWSW 参数3:> 参数4:0 参数5:
  // sParam1:HUMAN sParam2:RWSW sParam3:> sParam4:0
  // [脚本参数不正确] Cmd:CHECKVAR 2 NPC名称:QManage 地图:0 座标:0:0 参数1:HUMAN 参数2:RWSW 参数3:> 参数4:0 参数5:
  if (sType = '') or (sVarName = '') or (sMethod = '') then
  begin
    Npc.ScriptConditionError(BasePlayer, QuestConditionInfo);
    Exit;
  end;

  if nVarValue < 0 then
    nVarValue := 0;

  cMethod := sMethod[1];
  DynamicVarList := Npc.GetDynamicVarList(BasePlayer, sType, sName);
  if DynamicVarList = nil then
  begin
    Npc.ScriptConditionError(BasePlayer, QuestConditionInfo);
    Exit;
  end
  else
  begin
    // if BasePlayer.m_DynamicVarList = DynamicVarList then
    // MainOutMessage('ConditionOfCheckVar BasePlayer.m_DynamicVarList = DynamicVarList');
    boFoundVar := False;
    for I := 0 to DynamicVarList.Count - 1 do
    begin
      DynamicVar := DynamicVarList.Items[I];
      if CompareText(DynamicVar.sName, sVarName) = 0 then
      begin
        boFoundVar := True;
        case DynamicVar.VarType of
          vInteger:
            begin
              case cMethod of
                '=':
                  if DynamicVar.nInternet = nVarValue then
                    Result := True;
                '>':
                  if DynamicVar.nInternet > nVarValue then
                    Result := True;
                '<':
                  if DynamicVar.nInternet < nVarValue then
                    Result := True;
              else
                if DynamicVar.nInternet >= nVarValue then
                  Result := True;
              end;
            end;
          vString:
            if DynamicVar.sString = sVarValue then
              Result := True;
        end;
        Break;
      end;
    end;
    if not boFoundVar then
      Npc.ScriptConditionError(BasePlayer, QuestConditionInfo);
  end;
end;

function ConditionOfCheckServerName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := QuestConditionInfo.sParam1 = g_Config.sServerName;
end;

function ConditionOfCheckMapName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sMapName: string;
  Envir: TEnvirnoment;
begin
  sMapName := QuestConditionInfo.sParam1;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  if Envir <> nil then
    sMapName := Envir.sMapName;

  Result := CompareText(sMapName, BaseObject.m_sMapName) = 0;
end;

function ConditionOfCheckSafeZone(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := BaseObject.InSafeZone;
end;

function ConditionOfCheckSkill(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nSkillLevel: Integer;
  cMethod: Char;
  UserMagic: pTUserMagic;
  boNewLevel: Boolean;
  sMagicName: string;
begin
  Result := False;

  nSkillLevel := QuestConditionInfo.nParam3;
  boNewLevel := QuestConditionInfo.sParam4 = '1';

  if (QuestConditionInfo.sParam2 = '') or (nSkillLevel < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  sMagicName := QuestConditionInfo.sParam1;
  UserMagic := nil;

  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    UserMagic := PlayObject.GetMagicInfo(sMagicName)
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    UserMagic := THeroObject(BaseObject).FindMagic(sMagicName);

  if UserMagic = nil then
    Exit;
  cMethod := QuestConditionInfo.sParam2[1];
  if boNewLevel then
  begin
    case cMethod of
      '=':
        if UserMagic.btNewLevel = nSkillLevel then
          Result := True;
      '>':
        if UserMagic.btNewLevel > nSkillLevel then
          Result := True;
      '<':
        if UserMagic.btNewLevel < nSkillLevel then
          Result := True;
    else
      if UserMagic.btNewLevel >= nSkillLevel then
        Result := True;
    end;
  end
  else
  begin
    case cMethod of
      '=':
        if UserMagic.btLevel = nSkillLevel then
          Result := True;
      '>':
        if UserMagic.btLevel > nSkillLevel then
          Result := True;
      '<':
        if UserMagic.btLevel < nSkillLevel then
          Result := True;
    else
      if UserMagic.btLevel >= nSkillLevel then
        Result := True;
    end;
  end;
end;

function ConditionOfAnsiContainsText(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := AnsiContainsText(QuestConditionInfo.sParam1, QuestConditionInfo.sParam2);
end;

function ConditionOfCompareText(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := CompareText(QuestConditionInfo.sParam1, QuestConditionInfo.sParam2) = 0;
end;

function ConditionOfCheckTextList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  S1, S2: string;
begin
  S1 := QuestConditionInfo.sParam2;
  S2 := QuestConditionInfo.sParam3;

  if (Length(S1) > 1) and (S1[1] = '''') and (S1[Length(S1)] = '''') then
  begin
    S1 := Copy(S1, 2, Length(S1) - 2);
  end;

  if (Length(S1) > 1) and (S1[1] = '"') and (S1[Length(S1)] = '"') then
  begin
    S1 := Copy(S1, 2, Length(S1) - 2);
  end;

  if (Length(S2) > 1) and (S2[1] = '''') and (S2[Length(S2)] = '''') then
  begin
    S2 := Copy(S2, 2, Length(S2) - 2);
  end;

  if (Length(S2) > 1) and (S2[1] = '"') and (S2[Length(S2)] = '"') then
  begin
    S2 := Copy(S2, 2, Length(S2) - 2);
  end;

  if S2 <> '' then
  begin
    if QuestConditionInfo.nParam4 = 0 then
      Result := CheckStringListEx(S1, S2, Npc.m_sPath + QuestConditionInfo.sParam1, False, QuestConditionInfo.nParam5 > 0)
    else
      Result := CheckStringListEx(S1, S2, QuestConditionInfo.sParam1, True, QuestConditionInfo.nParam5 > 0);
  end
  else
  begin
    if QuestConditionInfo.nParam4 = 0 then
      Result := CheckStringList(S1, Npc.m_sPath + QuestConditionInfo.sParam1, False, QuestConditionInfo.nParam5 > 0)
    else
      Result := CheckStringList(S1, QuestConditionInfo.sParam1, True, QuestConditionInfo.nParam5 > 0);
  end;
end;

function ConditionOfCheckCacheTextList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  S1, S2: string;
begin
  S1 := QuestConditionInfo.sParam2;
  S2 := QuestConditionInfo.sParam3;

  if (Length(S1) > 1) and (S1[1] = '''') and (S1[Length(S1)] = '''') then
  begin
    S1 := Copy(S1, 2, Length(S1) - 2);
  end;

  if (Length(S1) > 1) and (S1[1] = '"') and (S1[Length(S1)] = '"') then
  begin
    S1 := Copy(S1, 2, Length(S1) - 2);
  end;

  if (Length(S2) > 1) and (S2[1] = '''') and (S2[Length(S2)] = '''') then
  begin
    S2 := Copy(S2, 2, Length(S2) - 2);
  end;

  if (Length(S2) > 1) and (S2[1] = '"') and (S2[Length(S2)] = '"') then
  begin
    S2 := Copy(S2, 2, Length(S2) - 2);
  end;

  if S2 <> '' then
  begin
    if QuestConditionInfo.nParam4 = 0 then
      Result := CheckCacheStringListEx(S1, S2, Npc.m_sPath + QuestConditionInfo.sParam1, False, QuestConditionInfo.nParam5 > 0)
    else
      Result := CheckCacheStringListEx(S1, S2, QuestConditionInfo.sParam1, True, QuestConditionInfo.nParam5 > 0);
  end
  else
  begin
    if QuestConditionInfo.nParam4 = 0 then
      Result := CheckCacheStringList(S1, Npc.m_sPath + QuestConditionInfo.sParam1, False, QuestConditionInfo.nParam5 > 0)
    else
      Result := CheckCacheStringList(S1, QuestConditionInfo.sParam1, True, QuestConditionInfo.nParam5 > 0);
  end;
end;

function ConditionOfIsGroupMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_GroupOwner = PlayObject);
end;

function ConditionOfCheckAnsiContainsTextList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := CheckAnsiContainsTextList(QuestConditionInfo.sParam2, Npc.m_sPath + QuestConditionInfo.sParam1);
end;

function ConditionOfCheckCacheContainsTextList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, Index: Integer;
  sLine, sSearchText, sFileName: string;
  LoadList: TStringList;
begin
  Result := False;
  sSearchText := QuestConditionInfo.sParam2;
  sFileName := g_Config.sEnvirDir + Npc.m_sPath + QuestConditionInfo.sParam1;

  if FileExists(sFileName) then
  begin
    Index := g_NpcTextFilesCache.IndexOf(sFileName);

    if Index >= 0 then
      LoadList := TStringList(g_NpcTextFilesCache.Objects[Index])
    else
    begin
      LoadList := TStringList.Create;
      g_NpcTextFilesCache.AddObject(sFileName, LoadList);
      try
        LoadList.LoadFromFile(sFileName);
      except
        MainOutMessage('loading fail.... => ' + sFileName);
      end;
    end;

    sSearchText := LowerCase(sSearchText);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLine := LowerCase(Trim(LoadList[I]));
      if Pos(sSearchText, sLine) <> 0 then
      begin
        Result := True;
        Break;
      end;
    end;
  end
  else
  begin
    MainOutMessage('file not found => ' + sFileName);
  end;
end;

function ConditionOfCheckHeroOnline(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_MyHero <> nil);
end;

function ConditionOfCheckIsDupMode(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := BaseObject.m_PEnvir.GetXYObjCount(BaseObject.m_nCurrX, BaseObject.m_nCurrY) > 1;
end;

function ConditionOfCheckGameDiamond(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Int64;
  cMethod: Char;
begin
  Result := False;
  nPoint := StrToInt64Def(QuestConditionInfo.sParam2, 0);

  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nGameDiamond = nPoint then
        Result := True;
    '>':
      if PlayObject.m_nGameDiamond > nPoint then
        Result := True;
    '<':
      if PlayObject.m_nGameDiamond < nPoint then
        Result := True;
  else
    if PlayObject.m_nGameDiamond >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckGameGird(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Int64;
  cMethod: Char;
begin
  Result := False;
  nPoint := StrToInt64Def(QuestConditionInfo.sParam2, 0);

  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nGameGird = nPoint then
        Result := True;
    '>':
      if PlayObject.m_nGameGird > nPoint then
        Result := True;
    '<':
      if PlayObject.m_nGameGird < nPoint then
        Result := True;
  else
    if PlayObject.m_nGameGird >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckGameGlory(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Integer;
  cMethod: Char;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nGameGlory = nPoint then
        Result := True;
    '>':
      if PlayObject.m_nGameGlory > nPoint then
        Result := True;
    '<':
      if PlayObject.m_nGameGlory < nPoint then
        Result := True;
  else
    if PlayObject.m_nGameGlory >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckStringLength(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nLen: Integer;
  cMethod: Char;
  sValue: AnsiString;
begin
  Result := False;
  nLen := QuestConditionInfo.nParam3;
  if (QuestConditionInfo.sParam2 = '') or (nLen < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  sValue := QuestConditionInfo.sParam1;
  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      if Length(sValue) = nLen then
        Result := True;
    '>':
      if Length(sValue) > nLen then
        Result := True;
    '<':
      if Length(sValue) < nLen then
        Result := True;
  else
    if Length(sValue) >= nLen then
      Result := True;
  end;
end;

function ConditionOfHavHero(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if QuestConditionInfo.sParam1 = 'TRUE' then
  begin
    Result := (PlayObject.m_sDeputyHeroName <> '') { and (not PlayObject.m_boStorageDeputyHero) };
  end
  else
  begin
    Result := (PlayObject.m_sHeroName <> '') { and (not PlayObject.m_boStorageHero) };
  end;
end;

function ConditionOfCheckOnlinePlayCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Integer;
  cMethod: Char;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if UserEngine.PlayObjectCount = nPoint then
        Result := True;
    '>':
      if UserEngine.PlayObjectCount > nPoint then
        Result := True;
    '<':
      if UserEngine.PlayObjectCount < nPoint then
        Result := True;
  else
    if UserEngine.PlayObjectCount >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckMapMove(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nX, nY: Integer;
  Envir: TEnvirnoment;
begin
  Result := False;

  // 双人骑马不允许传送 chongchong 2013-10-15
  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(BaseObject).m_boOnHorse and (TPlayObject(BaseObject).m_HorseOtherHum <> nil) then
    Exit;

  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);
  nX := QuestConditionInfo.nParam2;
  nY := QuestConditionInfo.nParam3;

  if (nX < 0) or (nY < 0) or (Envir = nil) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  Result := Envir.CanWalk(nX, nY, True);
  {
    g_FindPath.BaseObject := BaseObject;
    if (BaseObject.m_PEnvir = Envir) then
    begin
    Result := g_FindPath.FindPath3(Envir, BaseObject.m_nCurrX, BaseObject.m_nCurrY, nX, nY, False, False);
    end;
  }
end;

function ConditionOfCheckNameDateTimeList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
{ function DeTwoDateTime(const St, Et: TDateTime): string;
  const
  Fmt = '相差 %d 年 %d 月 %d 日  %d 小时 %d 分 %d 秒';
  var
  Year, Month, Day: Integer;
  Hour, Min, Se: Integer;
  begin
  Year := YearsBetween(et, st); // 相差总年数

  Month := MonthsBetween(et, st) mod 12; // 相差月数（年后的零头）

  Day := Trunc(Et - IncMonth(IncYear(St, Year), Month)); // 取相隔XX年YY月后的天数零头

  // 下面的东东有点奇怪，你可以注释以后用2002-1-31与2002-6-1试试就明白了。
  if (Et - Day) < IncMonth(Et, -1) then // 如果间隔天数超过一个月
  begin
  Month := Month + 1;
  Day := Day mod Trunc(Et - IncMonth(Et, -1));
  end;


  Hour := HoursBetween(Et, St) mod 24; // 相差总小时
  Min := MinutesBetween(Et, St) mod 60; // 相差分钟
  Se := SecondsBetween(Et, st) mod 60; // 相差秒数

  Result := Format(Fmt, [Year, Month, Day, Hour, Min, Se]);
  end; }

  function BetweenDateTime(const St, Et: TDateTime; var Hour, Min, Se: Integer): Integer;
  begin
    Hour := HoursBetween(Et, St) mod 24; // 相差总小时
    Min := MinutesBetween(Et, St) mod 60; // 相差分钟
    Se := SecondsBetween(Et, St) mod 60; // 相差秒数
    Result := GetDayCount(Et, St);
  end;

var
  I: Integer;
  LoadList: TStringList;
  sListFileName, sLineText, sHumName, sDate, sTime: string;
  boDeleteExprie: Boolean;
  dOldDate: TDateTime;
  nDay: Integer;
  AYear, AMonth, ADay, AHour, AMinute: Integer;
  sYear, sMonth, sDAY, sHour, sMinute: string;
  nHour, nMIN, nSec: Integer;
begin
  Result := False;

  boDeleteExprie := QuestConditionInfo.sParam2 = '1';
  // nDayCount := StrToIntDef(QuestConditionInfo.sParam3, -1);
  // boDeleteExprie := CompareText(QuestConditionInfo.sParam2, '清理') = 0;
  // boNoCompareHumanName := CompareText(QuestConditionInfo.sParam6, '1') = 0;
  // cMethod := QuestConditionInfo.sParam2[1];
  // if nDayCount < 0 then begin
  // ScriptConditionError(PlayObject, QuestConditionInfo);
  // Exit;
  // end;
  sListFileName := g_Config.sEnvirDir + Npc.m_sPath + QuestConditionInfo.sParam1;

  if FileExists(sListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := Trim(LoadList[I]);
      sLineText := GetValidStr3(sLineText, sHumName, [' ', #9]);
      sLineText := GetValidStr3(sLineText, sDate, [' ', #9]);
      sLineText := GetValidStr3(sLineText, sTime, [' ', #9]);
      sDate := GetValidStr3_Ex(sDate, sYear, '-');
      sDate := GetValidStr3_Ex(sDate, sMonth, '-');
      sDate := GetValidStr3_Ex(sDate, sDAY, '-');
      sTime := GetValidStr3_Ex(sTime, sHour, ':');
      sTime := GetValidStr3_Ex(sTime, sMinute, ':');

      AYear := StrToIntDef(sYear, -1);
      AMonth := StrToIntDef(sMonth, -1);
      ADay := StrToIntDef(sDAY, -1);
      AHour := StrToIntDef(sHour, -1);
      AMinute := StrToIntDef(sMinute, -1);

      // sLineText := GetValidStr3(sLineText, sDateTime, [' ', #9]);
      // MainOutMessage('sDateTime ' + sDateTime);
      // MainOutMessage('sDateTime2 ' + DateTimeToStr(StrToDateTime(sDateTime)));
      if (AYear >= 0) and (AMonth >= 0) and (ADay >= 0) and (AHour >= 0) and (AMinute >= 0) and (CompareText(sHumName, BaseObject.m_sCharName) = 0) and TryEncodeDateTime(AYear, AMonth, ADay, AHour, AMinute, 0, 0, dOldDate) then
      begin
        if Now >= dOldDate then
        begin
          if boDeleteExprie then
          begin
            LoadList.Delete(I);
            try
              LoadList.SaveToFile(sListFileName);
            except
              MainOutMessage('Save fail.... => ' + sListFileName);
            end;
          end;
          Break;
        end;

        nDay := BetweenDateTime(Now, dOldDate, nHour, nMIN, nSec);

        if (nDay <= 0) and (nHour <= 0) and (nMIN <= 0) then
        begin
          if boDeleteExprie then
          begin
            LoadList.Delete(I);
            try
              LoadList.SaveToFile(sListFileName);
            except
              MainOutMessage('Save fail.... => ' + sListFileName);
            end;
          end;
          Break;
        end;

        { if nDay < 0 then begin
          if boDeleteExprie then begin
          LoadList.Delete(I);
          try
          LoadList.SaveToFile(sListFileName);
          except
          MainOutMessage('Save fail.... => ' + sListFileName);
          end;
          end;
          break;
          end;

          if (nDay = 0) and (nHour < 0) then begin
          if boDeleteExprie then begin
          LoadList.Delete(I);
          try
          LoadList.SaveToFile(sListFileName);
          except
          MainOutMessage('Save fail.... => ' + sListFileName);
          end;
          end;
          break;
          end;

          if (nDay = 0) and (nHour = 0) and (nMin < 0) then begin
          if boDeleteExprie then begin
          LoadList.Delete(I);
          try
          LoadList.SaveToFile(sListFileName);
          except
          MainOutMessage('Save fail.... => ' + sListFileName);
          end;
          end;
          break;
          end; }

        Result := True;

        if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam3, FormatDateTime('yyyy/mm/dd HH:MM', dOldDate), nDay) then
        begin
          Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
          Break;
        end;

        // 修正日期差比实际多一天; 如 2015-9-23 10:00:00  至 2015-9-24 8:00:00 剩余时间还有1天多 chongchong 2015-09-23
        nDay := Max(nDay - 1, 0);
        if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam4, IntToStr(nDay), nDay) then
        begin
          Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
          Break;
        end;

        if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam5, IntToStr(nHour), nHour) then
        begin
          Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
          Break;
        end;

        if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam6, IntToStr(nMIN), nMIN) then
        begin
          Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
          Break;
        end;

        Break;
      end;
    end;
    LoadList.Free;
  end
  else
  begin
    MainOutMessage('file not found => ' + sListFileName);
  end;
end;

function ConditionOfCheckKillerRace(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nKillerRace: Integer;
  cMethod: Char;
begin
  Result := False;
  nKillerRace := QuestConditionInfo.nParam2;

  if BaseObject = nil then
    Exit;

  if (QuestConditionInfo.sParam1 = '') or (nKillerRace < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if BaseObject.m_btKillerRace = nKillerRace then
        Result := True;
    '>':
      if BaseObject.m_btKillerRace > nKillerRace then
        Result := True;
    '<':
      if BaseObject.m_btKillerRace < nKillerRace then
        Result := True;
  else
    if BaseObject.m_btKillerRace >= nKillerRace then
      Result := True;
  end;
end;

function ConditionOfCheckCurrTargetRace(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nTargetRaceServer, nRaceServer: Integer;
  cMethod: Char;
begin
  Result := False;
  if (BaseObject = nil) or (BaseObject.m_CurrTarget = nil) then
    Exit;

  nRaceServer := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nRaceServer < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if BaseObject.m_CurrTarget is TCopyMon then // 分身RaceServer检测改为151
    nTargetRaceServer := 151
  else
    nTargetRaceServer := BaseObject.m_CurrTarget.m_btRaceServer;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nTargetRaceServer = nRaceServer then
        Result := True;
    '>':
      if nTargetRaceServer > nRaceServer then
        Result := True;
    '<':
      if nTargetRaceServer < nRaceServer then
        Result := True;
  else
    if nTargetRaceServer >= nRaceServer then
      Result := True;
  end;
end;

function ConditionOfCheckCurrTargetSlave(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  tmpCurrTarget: TBaseObject;
begin
  Result := False;
  if (BaseObject = nil) or (BaseObject.m_CurrTarget = nil) then
    Exit;

  tmpCurrTarget := BaseObject.m_CurrTarget;
  Result := (tmpCurrTarget.m_Master <> nil) and (tmpCurrTarget.m_Master.m_SlaveList.IndexOf(Pointer(tmpCurrTarget)) >= 0);
end;

function ConditionOfCheckCastleWarArea(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  Castle: TUserCastle;
begin
  // Result := False;
  g_CastleManager.Lock;
  try
    Castle := g_CastleManager.InCastleWarArea(BaseObject.m_PEnvir, BaseObject.m_nCurrX, BaseObject.m_nCurrY);
    Result := (Castle <> nil); // and Castle.m_boUnderWar;
  finally
    g_CastleManager.UnLock;
  end;
end;

function ConditionOfCheckUnderWar(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  UserCastle: TUserCastle;
  sCastleName: string;
begin
  Result := False;
  sCastleName := QuestConditionInfo.sParam1;
  if sCastleName <> '' then
  begin
    g_CastleManager.Lock;
    try
      for I := 0 to g_CastleManager.m_CastleList.Count - 1 do
      begin
        UserCastle := TUserCastle(g_CastleManager.m_CastleList.Items[I]);
        if CompareText(UserCastle.m_sName, sCastleName) = 0 then
        begin
          Result := UserCastle.m_boUnderWar;
          Break;
        end;
      end;
    finally
      g_CastleManager.UnLock;
    end;
  end;
end;

function ConditionOfCheckMapSameMonCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nX, nY: Integer;
  sMapName, sMonName: string;
  nCount: Integer;
  cMethod: Char;
  nMapRangeCount: Integer;
  Envir: TEnvirnoment;
  MonList: TList;
  AObject: TBaseObject;

  function DeleteLastNumber(MonName: string): string;
  var
    n: Integer;
  begin
    Result := MonName;
    if QuestConditionInfo.nParam5 <> 1 then
      Exit;
    for n := Length(MonName) downto 1 do
    begin
{$IF CompilerVersion >= 22}
      if not CharInSet(MonName[n], ['0'..'9']) then
        Break;
{$ELSE}
      if not (MonName[n] in ['0'..'9']) then
        Break;
{$IFEND}
    end;

    if n < Length(MonName) then
      Result := Copy(MonName, 1, n);
  end;

begin
  Result := False;
  sMapName := QuestConditionInfo.sParam1;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1)
  else
    sMapName := Envir.sMapName;

  sMonName := QuestConditionInfo.sParam2;
  nCount := QuestConditionInfo.nParam4;

  if CompareText(sMapName, 'Self') = 0 then
  begin
    sMapName := BaseObject.m_sMapName;
    Envir := g_MapManager.FindMap(sMapName);
  end;

  if (QuestConditionInfo.sParam3 = '') or (Envir = nil) or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam3[1];

  MonList := TList.Create;
  nX := Envir.m_nWidth div 2;
  nY := Envir.m_nHeight div 2;
  Envir.GetRangeBaseObject(nX, nY, Max(Envir.m_nWidth div 2, Envir.m_nHeight div 2), True, MonList);
  for I := MonList.Count - 1 downto 0 do
  begin
    if MonList.Count <= 0 then
      Break;
    AObject := TBaseObject(MonList.Items[I]);
    if (CompareText(DeleteLastNumber(AObject.m_sCharName), sMonName) <> 0) or (AObject.m_btRaceServer < RC_ANIMAL) or (AObject.m_btRaceServer = RC_ARCHERGUARD) or (AObject.m_Master <> nil) or (AObject.m_btRaceServer = RC_NPC) or (AObject.m_btRaceServer = RC_PEACENPC) then
      MonList.Delete(I);
  end;
  nMapRangeCount := MonList.Count;
  case cMethod of
    '=':
      if nMapRangeCount = nCount then
        Result := True;
    '>':
      if nMapRangeCount > nCount then
        Result := True;
    '<':
      if nMapRangeCount < nCount then
        Result := True;
  else
    if nMapRangeCount >= nCount then
      Result := True;
  end;
  MonList.Free;
end;

function ConditionOfCheckMyShop(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := g_M2DataDB.UserShopDB.HumanNameExists(PlayObject.m_sCharName);
end;

function ConditionOfCheckShopName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := g_M2DataDB.UserShopDB.ShopNameExists(QuestConditionInfo.sParam1);
end;

function ConditionOfCheckSlaveInRange(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sSlaveName: string;
  sCharName: string;
  I, nRange: Integer;
  AObject: TBaseObject;
begin
  Result := False;
  sSlaveName := QuestConditionInfo.sParam1;
  if (sSlaveName = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  sSlaveName := DelNumber(sSlaveName);
  nRange := QuestConditionInfo.nParam2;

  if nRange < 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  for I := 0 to BaseObject.m_SlaveList.Count - 1 do
  begin
    AObject := TBaseObject(BaseObject.m_SlaveList.Items[I]);
    sCharName := DelNumber(AObject.m_sCharName);
    if (AObject.m_PEnvir = BaseObject.m_PEnvir) and (CompareText(sCharName, sSlaveName) = 0) and (not AObject.m_boDeath) then
    begin
      if (abs(BaseObject.m_nCurrX - AObject.m_nCurrX) <= nRange) and (abs(BaseObject.m_nCurrY - AObject.m_nCurrY) <= nRange) then
      begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

function ConditionOfCheckRangeHumanCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount, nHumanCount, nX, nY, nRange: Integer;
  cMethod: Char;
  Envir: TEnvirnoment;
begin
  Result := False;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  nX := QuestConditionInfo.nParam2;
  nY := QuestConditionInfo.nParam3;
  nRange := QuestConditionInfo.nParam4;
  nCount := QuestConditionInfo.nParam6;

  if (QuestConditionInfo.sParam5 = '') or (Envir = nil) or (nX < 0) or (nY < 0) or (nRange < 0) or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  nHumanCount := UserEngine.GetMapRageHuman(Envir, nX, nY, nRange, nil);

  cMethod := QuestConditionInfo.sParam5[1];
  case cMethod of
    '=':
      if nHumanCount = nCount then
        Result := True;
    '>':
      if nHumanCount > nCount then
        Result := True;
    '<':
      if nHumanCount < nCount then
        Result := True;
  else
    if nHumanCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckHumanInRange(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nX, nY, nRange: Integer;
  Envir: TEnvirnoment;
begin
  Result := False;

  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  nX := QuestConditionInfo.nParam2;
  nY := QuestConditionInfo.nParam3;
  nRange := QuestConditionInfo.nParam4;

  if (Envir = nil) or (nX < 0) or (nY < 0) or (nRange < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (BaseObject.m_PEnvir = Envir) and (abs(BaseObject.m_nCurrX - nX) <= nRange) and (abs(BaseObject.m_nCurrY - nY) <= nRange) then
  begin
    Result := True;
  end;
end;

function ConditionOfCheckGuildMemberMaxLimitCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sGuildName: string;
  Guild: TGUild;
  cMethod: Char;
  nCount: Integer;
begin
  Result := False;

  if UpperCase(QuestConditionInfo.sParam1) = 'SELF' then
  begin
    if PlayObject.m_MyGuild = nil then
    begin
      Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
      Exit;
    end;
    Guild := TGUild(PlayObject.m_MyGuild);
  end
  else
  begin
    sGuildName := QuestConditionInfo.sParam1;
    Guild := g_GuildManager.FindGuild(sGuildName);
    if Guild = nil then
    begin
      Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
      Exit;
    end;
  end;
  nCount := QuestConditionInfo.nParam3;
  if (QuestConditionInfo.sParam2 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      if Guild.m_nMemberMaxLimit = nCount then
        Result := True;
    '>':
      if Guild.m_nMemberMaxLimit > nCount then
        Result := True;
    '<':
      if Guild.m_nMemberMaxLimit < nCount then
        Result := True;
  else
    if Guild.m_nMemberMaxLimit >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckNewItemValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nValType, nPoint: Integer;
  nBoxItemIndex, nMakeIndex: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  StdItem: pTStdItem;
  SmartObject: TSmartObject;
begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  // 检测当前装备是否附加了新属性支持OK框位置 chongchong 2014-07-25
  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nValType := QuestConditionInfo.nParam2;
  nPoint := QuestConditionInfo.nParam4;

  if (QuestConditionInfo.sParam3 = '') or (nValType < 0) or (nValType > 24) or (nPoint < 0) or (nPoint > 255) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam3[1];
  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      SmartObject.SysMsg('你首饰盒中没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      SmartObject.SysMsg('你神佑袋中没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if StdItem <> nil then
    begin
      case cMethod of
        '=':
          if UserItem.btNewValue[nValType] = nPoint then
            Result := True;
        '>':
          if UserItem.btNewValue[nValType] > nPoint then
            Result := True;
        '<':
          if UserItem.btNewValue[nValType] < nPoint then
            Result := True;
      else
        if UserItem.btNewValue[nValType] >= nPoint then
          Result := True;
      end;
    end;
  end;
end;

function ConditionOfCheckShopStall(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := PlayObject.m_boShopStall;
end;

function ConditionOfCheckHeroLoyal(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Integer;
  cMethod: Char;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) or (nPoint > 100) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if PlayObject.m_MyHero = nil then
    Exit;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Trunc(THeroObject(PlayObject.m_MyHero).m_rLoyalPoint) = nPoint then
        Result := True;
    '>':
      if Trunc(THeroObject(PlayObject.m_MyHero).m_rLoyalPoint) > nPoint then
        Result := True;
    '<':
      if Trunc(THeroObject(PlayObject.m_MyHero).m_rLoyalPoint) < nPoint then
        Result := True;
  else
    if Trunc(THeroObject(PlayObject.m_MyHero).m_rLoyalPoint) >= nPoint then
      Result := True;
  end;
end;

function ConditionOfIsDummy(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) then
    Result := TSmartObject(BaseObject).m_boDummyObject
  else
    Result := False;
end;

function ConditionOfCheckDummyCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') or (QuestConditionInfo.nParam2 < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if UserEngine.GetDummyObjectCount = QuestConditionInfo.nParam2 then
        Result := True;
    '>':
      if UserEngine.GetDummyObjectCount > QuestConditionInfo.nParam2 then
        Result := True;
    '<':
      if UserEngine.GetDummyObjectCount < QuestConditionInfo.nParam2 then
        Result := True;
  else
    if UserEngine.GetDummyObjectCount >= QuestConditionInfo.nParam2 then
      Result := True;
  end;
end;

function ConditionOfMapHumIsSameGuild(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, II: Integer;
  Online: TPlayObject;
  Online1: TPlayObject;
begin
  if QuestConditionInfo.sParam1 = '1' then
  begin
    Result := True;

{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      UserEngine.m_PlayObjectList.LockR(61);
    try
{$IFEND}
      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
        Online := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
        if (not Online.m_boDeath) and (not Online.m_boGhost) and (Online.m_PEnvir = PlayObject.m_PEnvir) and (Online.m_MyGuild <> nil) then
        begin

          for II := I + 1 to UserEngine.m_PlayObjectList.Count - 1 do
          begin
            Online1 := TPlayObject(UserEngine.m_PlayObjectList.Objects[II]);
            if (not Online1.m_boDeath) and (not Online1.m_boGhost) and (Online1.m_PEnvir = PlayObject.m_PEnvir) and (Online1.m_MyGuild <> nil) and (Online1.m_MyGuild <> Online.m_MyGuild) then
            begin
              Result := False;
              Exit;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UserEngine.m_PlayObjectList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
    Result := False;
    if PlayObject.m_MyGuild = nil then
      Exit;
    Result := True;

{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      UserEngine.m_PlayObjectList.LockR(62);
    try
{$IFEND}
      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
        Online := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
        if (not Online.m_boDeath) and (not Online.m_boGhost) and (Online.m_PEnvir = PlayObject.m_PEnvir) and (Online.m_MyGuild <> PlayObject.m_MyGuild) then
        begin
          Result := False;
          Break;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UserEngine.m_PlayObjectList.UnLockR;
    end;
{$IFEND}
  end;
end;

function ConditionOfCheckKillMonName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if BaseObject.m_CurrTarget <> nil then
  begin
    Result := CompareText(BaseObject.m_CurrTarget.m_sCharName, QuestConditionInfo.sParam1) = 0;
    if QuestConditionInfo.nParam2 > 0 then
      BaseObject.m_CurrTarget := nil;
  end;
end;

function ConditionOfCheckHitMonName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if (BaseObject.m_CurrTarget <> nil) and (not (BaseObject.m_CurrTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) then
  begin
    Result := CompareText(BaseObject.m_CurrTarget.m_sCharName, QuestConditionInfo.sParam1) = 0;
  end;
end;

function ConditionOfCheckOffLine(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := PlayObject.m_boOffline;
end;

function ConditionOfCheckItemNameColor(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  nWhere: Integer;
  nColor: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  SmartObject: TSmartObject;
  nBoxItemIndex, nMakeIndex: Integer;
begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nColor := QuestConditionInfo.nParam2;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (not (nColor in [0..255])) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      Exit;
    end;
  end
  else
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    if UserItem.btColor > 0 then
      Result := UserItem.btColor = nColor
    else
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) then
      begin
        Result := StdItem.Color = nColor;
      end;
    end;
  end;
end;

function ConditionOfKillByHum(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if BaseObject.m_CurrTarget <> nil then
  begin
    Result := BaseObject.m_CurrTarget.m_btRaceServer = RC_PLAYOBJECT;
  end;
end;

function ConditionOfCheckRandomNo(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (PlayObject <> nil) and (PlayObject.m_sRandomString <> '') and (PlayObject.m_sInputData <> '') then
    Result := CompareText(PlayObject.m_sRandomString, PlayObject.m_sInputData) = 0;
end;

function ConditionOfCheckFoundryItem(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;

  function FindBagItemCount(const sItemName: string; var nItemCount: Integer): Boolean;
  var
    I: Integer;
    UserItem: pTUserItem;
    StdItem: pTStdItem;
  begin
    Result := False;
    for I := 0 to PlayObject.m_ItemList.Count - 1 do
    begin
      if nItemCount <= 0 then
      begin
        Result := True;
        Break;
      end;
      UserItem := PlayObject.m_ItemList.Items[I];
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) and (CompareText(StdItem.Name, sItemName) = 0) then
      begin
        if CheckOverLapItem(StdItem) then
        begin // 叠加物品
          if nItemCount > UserItem.Dura + 1 then
          begin
            nItemCount := nItemCount - (UserItem.Dura + 1);
          end
          else
          begin
            nItemCount := 0;
            Result := True;
            Break;
          end;
        end
        else
        begin
          Dec(nItemCount);
          if nItemCount <= 0 then
          begin
            Result := True;
            Break;
          end;
        end;
      end;
    end;
  end;

var
  I: Integer;
  sItemName: string;
  FoundryItem: pTFoundryItem;
  FoundryNeedItem: pTFoundryNeedItem;
  nItemCount: Integer;
  IsFoundAllItem: Boolean;
  StrSplite: string;
begin
  Result := False;

  PlayObject.m_sLackFoundryItem := '';
  sItemName := QuestConditionInfo.sParam1;
  if CompareText(sItemName, '%FoundryItem') = 0 then
  begin
    sItemName := PlayObject.m_sNpcSelectItemName;
  end;
  if sItemName = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  IsFoundAllItem := True;
  StrSplite := '';

  FoundryItem := GetFoundryItem(sItemName);
  if FoundryItem <> nil then
  begin
    for I := 0 to FoundryItem.ItemList.Count - 1 do
    begin
      FoundryNeedItem := FoundryItem.ItemList.Items[I];

      if CompareText(FoundryNeedItem.sItemName, sSTRING_GOLDNAME) = 0 then
      begin
        if PlayObject.m_nGold < FoundryNeedItem.nItemCount then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + sSTRING_GOLDNAME + ':' + IntToStr(FoundryNeedItem.nItemCount - PlayObject.m_nGold);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end
      else if CompareText(FoundryNeedItem.sItemName, g_Config.sGameGoldName) = 0 then
      begin
        if PlayObject.m_nGameGold < FoundryNeedItem.nItemCount then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + g_Config.sGameGoldName + ':' + IntToStr(FoundryNeedItem.nItemCount - PlayObject.m_nGameGold);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end
      else if CompareText(FoundryNeedItem.sItemName, g_Config.sCreditPointName) = 0 then
      begin
        if PlayObject.m_WAbil.CreditPoint < FoundryNeedItem.nItemCount then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + g_Config.sCreditPointName + ':' + IntToStr(FoundryNeedItem.nItemCount - PlayObject.m_WAbil.CreditPoint);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end
      else if CompareText(FoundryNeedItem.sItemName, g_Config.sGamePointName) = 0 then
      begin
        if PlayObject.m_nGamePoint < FoundryNeedItem.nItemCount then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + g_Config.sGamePointName + ':' + IntToStr(FoundryNeedItem.nItemCount - PlayObject.m_nGamePoint);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end
      else if CompareText(FoundryNeedItem.sItemName, g_Config.sGameGirdName) = 0 then
      begin
        if PlayObject.m_nGameGird < FoundryNeedItem.nItemCount then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + g_Config.sGameGirdName + ':' + IntToStr(FoundryNeedItem.nItemCount - PlayObject.m_nGameGird);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end
      else if CompareText(FoundryNeedItem.sItemName, g_Config.sGameDiamondName) = 0 then
      begin
        if PlayObject.m_nGameDiamond < FoundryNeedItem.nItemCount then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + g_Config.sGameDiamondName + ':' + IntToStr(FoundryNeedItem.nItemCount - PlayObject.m_nGameDiamond);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end
      else
      begin
        nItemCount := FoundryNeedItem.nItemCount;
        if not FindBagItemCount(FoundryNeedItem.sItemName, nItemCount) then
        begin
          PlayObject.m_sLackFoundryItem := PlayObject.m_sLackFoundryItem + StrSplite + FoundryNeedItem.sItemName + ':' + IntToStr(nItemCount);
          StrSplite := ', ';
          IsFoundAllItem := False;
        end;
      end;

    end;

    Result := IsFoundAllItem;
    Exit;
  end;
end;

function ConditionOfCheckGuildMemberCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount: Integer;
  cMethod: Char;
  Guild: TGUild;
begin
  Result := False;
  if (PlayObject = nil) or (PlayObject.m_MyGuild = nil) then
  begin
    Exit;
  end;
  nCount := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  Guild := TGUild(PlayObject.m_MyGuild);

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if Guild.Count = nCount then
        Result := True;
    '>':
      if Guild.Count > nCount then
        Result := True;
    '<':
      if Guild.Count < nCount then
        Result := True;
  else
    if Guild.Count >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckUpgradeItemName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nWhere: Integer;
  sItemName: string;
begin
  Result := False;

  nWhere := QuestConditionInfo.nParam1;

  if (nWhere < 0) or (nWhere > 1) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  sItemName := QuestConditionInfo.sParam2;

  if nWhere = 0 then
    Result := CompareText(sItemName, PlayObject.m_sUpgradeDialogItemName1) = 0
  else
    Result := CompareText(sItemName, PlayObject.m_sUpgradeDialogItemName2) = 0;
end;

function ConditionOfCheckMine(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  nCount: Integer;
  nDura, nPoint: Integer;
  sItemName: string;
  cMethod: Char;
begin
  Result := False;
  sItemName := QuestConditionInfo.sParam1;
  nCount := QuestConditionInfo.nParam2;
  nPoint := QuestConditionInfo.nParam4;
  if (nCount < 0) or (nPoint < 0) or (Length(QuestConditionInfo.sRawParam3) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sRawParam3[1];
  for I := 0 to BaseObject.m_ItemList.Count - 1 do
  begin
    if nCount <= 0 then
      Break;
    UserItem := BaseObject.m_ItemList.Items[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if StdItem = nil then
      Continue;
    if (CompareText(StdItem.Name, sItemName) = 0) then
    begin
      // chongchong 2013-11-22

      if (StdItem.StdMode = 43) or // 矿石 纯度
      // --------------------------------------------------------------------------------------------------------持久
        ((StdItem.StdMode = 7) and (StdItem.Shape in [0, 1, 2, 3])) or // 千里传音,气血石,幻魔石,魔血石
        (StdItem.StdMode = 53) or // 53类宝石
        ((StdItem.StdMode = 2) and (StdItem.Shape in [1, 2, 3])) or // 自定义记次物品,随机传送石,回城石
        ((StdItem.StdMode = 25) and (StdItem.Shape = 9)) or // 火龙之心
        (StdItem.StdMode = 5) or // 武器
        (StdItem.StdMode = 6) or // 武器
        (StdItem.StdMode = 10) or // 衣服 (男)
        (StdItem.StdMode = 11) or // 衣服 (女)
        (StdItem.StdMode = 12) or // 盾牌
        (StdItem.StdMode = 15) or // 头盔
        (StdItem.StdMode = 16) or // 斗笠
        (StdItem.StdMode = 19) or // 项链
        (StdItem.StdMode = 20) or // 项链
        (StdItem.StdMode = 21) or // 项链
        (StdItem.StdMode = 22) or // 戒指
        (StdItem.StdMode = 23) or // 戒指
        (StdItem.StdMode = 24) or // 手镯
        (StdItem.StdMode = 26) or // 手镯
        (StdItem.StdMode = 28) or // 勋章 马牌
        (StdItem.StdMode = 29) or // 天使翅膀
        (StdItem.StdMode = 30) or // 照明物体
        (StdItem.StdMode = 49) or // 聚灵珠
        (StdItem.StdMode = 66) or // 时装衣服 (男)
        (StdItem.StdMode = 67) or // 时装衣服 (女)
        (StdItem.StdMode = 68) or // 时装武器
        (StdItem.StdMode = 69) or // 时装武器
        ((StdItem.StdMode >= 75) and (StdItem.Shape <= 89)) or // 时装首饰
        (StdItem.StdMode = 90) or // 灵玉
        (StdItem.StdMode = 94) or // 捕兽网
        (StdItem.StdMode = 96) or // 祝福罐 chongchong 2017-07-08
        (StdItem.StdMode = 97) or // 火龙补品 chongchong 2017-07-10
      // --------------------------------------------------------------------------------------------------------品质
        (StdItem.StdMode = 40) // 肉
        then
      begin
        nDura := Round(UserItem.Dura / 1000);
        case cMethod of
          '=':
            if nDura = nPoint then
              Dec(nCount);
          '>':
            if nDura > nPoint then
              Dec(nCount);
          '<':
            if nDura < nPoint then
              Dec(nCount);
        else
          if nDura >= nPoint then
            Dec(nCount);
        end;
      end;
    end;
  end;
  Result := nCount <= 0;
end;

function ConditionOfCheckHeroCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nCount, nSlaveCount: Integer;
  cMethod: Char;
  SlaveObject: TBaseObject;
begin
  Result := False;
  nCount := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  nSlaveCount := 0;
  for I := 0 to BaseObject.m_SlaveList.Count - 1 do
  begin
    SlaveObject := TBaseObject(BaseObject.m_SlaveList.Items[I]);
    if (not SlaveObject.m_boDeath) and (SlaveObject is TCopyMon) then
      Inc(nSlaveCount);
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nSlaveCount = nCount then
        Result := True;
    '>':
      if nSlaveCount > nCount then
        Result := True;
    '<':
      if nSlaveCount < nCount then
        Result := True;
  else
    if nSlaveCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckMagicName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  UserMagic: pTUserMagic;
begin
  UserMagic := nil;
  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    UserMagic := TPlayObject(BaseObject).GetMagicInfo(QuestConditionInfo.sParam1)
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    UserMagic := THeroObject(BaseObject).FindMagic(QuestConditionInfo.sParam1);

  Result := UserMagic <> nil;
end;

function ConditionOfCheckKillMob(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if BaseObject.m_PEnvir.m_boOnKillMob then
  begin
    Result := (BaseObject.m_CurrTarget <> nil) and (CompareText(QuestConditionInfo.sParam1, BaseObject.m_CurrTarget.m_sCharName) = 0);
  end;
end;

function ConditionOfCheckKillSlaveName(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (BaseObject.m_CurrTarget <> nil) and (CompareText(QuestConditionInfo.sParam1, BaseObject.m_CurrTarget.m_sCharName) = 0);
end;

function ConditionOfCheckGuildMember(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sCharName: string;
  sGuildName: string;
  Guild: TGUild;
begin
  Result := False;
  sGuildName := QuestConditionInfo.sParam1;
  Guild := g_GuildManager.FindGuild(sGuildName);
  if (sGuildName = '') or (Guild = nil) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  sCharName := QuestConditionInfo.sParam2;

  if sCharName <> '' then
    Guild.IsMember(sCharName)
  else
    Result := Guild.IsMember(BaseObject.m_sCharName);
end;
(*
  function ConditionOfRepairAll(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
  var
  I: Integer;
  nPrice, nRepairPrice: Integer;
  UserItem: pTUserItem;
  begin
  Result := False;

  if Npc is TMerchant then
  begin
  nRepairPrice := 0;
  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
  UserItem := @PlayObject.m_UseItems[I];
  if (UserItem.wIndex > 0) and (UserItem.Dura < UserItem.DuraMax) and (not g_ItemRules.Get(UserEngine.GetStdItemName(UserItem.wIndex), 3)) then
  begin
  if (UserItem.boCanotUserRepairBindItem and UserItem.boIsBind) then Continue;
  nPrice := TMerchant(Npc).GetUserPrice(PlayObject, TMerchant(Npc).GetUserItemPrice(UserItem)) * g_Config.nSuperRepairPriceRate;
  if (nPrice > 0) and (UserItem.DuraMax > UserItem.Dura) then
  begin
  if UserItem.DuraMax > 0 then
  begin
  nRepairPrice := nRepairPrice + Round(nPrice div 3 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
  end else
  begin
  if nPrice > 0 then
  nRepairPrice := nRepairPrice + nPrice;
  end;
  end;
  end;
  end;

  if PlayObject.DecGold(nRepairPrice) then
  begin
  PlayObject.GoldChanged;
  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
  UserItem := @PlayObject.m_UseItems[I];
  if (UserItem.wIndex > 0) and (UserItem.Dura < UserItem.DuraMax) and (not g_ItemRules.Get(UserEngine.GetStdItemName(UserItem.wIndex), 3)) then
  begin
  if (UserItem.boCanotUserRepairBindItem and UserItem.boIsBind) then Continue;
  if (UserItem.DuraMax > UserItem.Dura) then
  begin
  UserItem.Dura := UserItem.DuraMax;
  if (PlayObject.m_btRaceServer = RC_PLAYOBJECT) or (PlayObject.m_btRaceServer = RC_HEROOBJECT) then
  begin
  PlayObject.SendMsg(PlayObject, RM_DURACHANGE, I, UserItem.Dura, UserItem.DuraMax, 0, '');
  end;
  end;
  end;
  end;
  Result := True;
  end;
  end;
  end;
*)

function ConditionOfCheckNationCredit(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Integer;
  cMethod: Char;
begin
  Result := False;

  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nNationCredit = nPoint then
        Result := True;
    '>':
      if PlayObject.m_nNationCredit > nPoint then
        Result := True;
    '<':
      if PlayObject.m_nNationCredit < nPoint then
        Result := True;
  else
    if PlayObject.m_nNationCredit >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckGameGoldEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPoint: Integer;
  cMethod: Char;
begin
  Result := False;
  nPoint := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nPoint < 0) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_nGameGoldEx = nPoint then
        Result := True;
    '>':
      if PlayObject.m_nGameGoldEx > nPoint then
        Result := True;
    '<':
      if PlayObject.m_nGameGoldEx < nPoint then
        Result := True;
  else
    if PlayObject.m_nGameGoldEx >= nPoint then
      Result := True;
  end;
end;

function ConditionOfCheckPulseLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nMeridian, nLevel: Integer;
  SmartObject: TSmartObject;
  cMethod: Char;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    SmartObject := TSmartObject(BaseObject);

    nMeridian := QuestConditionInfo.nParam1;
    nLevel := QuestConditionInfo.nParam3;

    if (QuestConditionInfo.sParam2 = '') or (not (nMeridian in [0..4])) then
    begin
      Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
      Exit;
    end;

    cMethod := QuestConditionInfo.sParam2[1];
    case cMethod of
      '>':
        Result := SmartObject.m_HumMeridians[nMeridian].Level > nLevel;
      '=':
        Result := SmartObject.m_HumMeridians[nMeridian].Level = nLevel;
      '<':
        Result := SmartObject.m_HumMeridians[nMeridian].Level < nLevel;
    else
      Result := SmartObject.m_HumMeridians[nMeridian].Level >= nLevel;
    end;
  end;
end;

function ConditionOfCheckHumanPulse(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nMeridian, nAcupoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    SmartObject := TSmartObject(BaseObject);

    nMeridian := QuestConditionInfo.nParam1;
    nAcupoint := QuestConditionInfo.nParam2;

    if (not (nMeridian in [0..4])) or (not (nAcupoint in [1..5])) then
    begin
      Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
      Exit;
    end;

    Dec(nAcupoint);
    if nAcupoint <= 0 then
    begin // 第一个穴位不检测
      Result := True;
      Exit;
    end;
    Dec(nAcupoint);

    Result := SmartObject.m_HumMeridians[nMeridian].Acupoints[nAcupoint] > 0;
  end;
end;

function ConditionOfCheckOpenPulseLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nMeridian, nAcupoint: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    SmartObject := TSmartObject(BaseObject);

    nMeridian := QuestConditionInfo.nParam1;
    nAcupoint := QuestConditionInfo.nParam2;

    if (not (nMeridian in [0..4])) or (not (nAcupoint in [1..5])) then
    begin
      Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
      Exit;
    end;

    Dec(nAcupoint);

    Result := SmartObject.m_AbilNG.Level >= g_Config.AcupointLevels[nMeridian, nAcupoint];
  end;
end;

function ConditionOfCheckHeroAutoPractice(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := PlayObject.m_boHeroAutoPractice;
end;

function ConditionOfCheckDeputyHero(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_MyHero <> nil) and (THeroObject(PlayObject.m_MyHero).m_boIsDeputy);
end;

function ConditionOfCheckHeroInStorage(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  if QuestConditionInfo.sParam1 = 'TRUE' then
  begin
    Result := PlayObject.m_boStorageDeputyHero or (PlayObject.m_sDeputyHeroName = '');
  end
  else
  begin
    Result := PlayObject.m_boStorageHero or (PlayObject.m_sHeroName = '');
  end;
end;

function ConditionOfCheckReadSkillNG(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(BaseObject).m_boTrainingNG;
end;

function ConditionOfCheckNGLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nLevel: Integer;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    SmartObject := TSmartObject(BaseObject);

    nLevel := QuestConditionInfo.nParam2;
    if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
    begin
      Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
      Exit;
    end;

    cMethod := QuestConditionInfo.sParam1[1];
    case cMethod of
      '=':
        if SmartObject.m_AbilNG.Level = nLevel then
          Result := True;
      '>':
        if SmartObject.m_AbilNG.Level > nLevel then
          Result := True;
      '<':
        if SmartObject.m_AbilNG.Level < nLevel then
          Result := True;
    else
      if SmartObject.m_AbilNG.Level >= nLevel then
        Result := True;
    end;
  end;
end;

function ConditionOfCheckOpenLastSkill(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) and TSmartObject(BaseObject).m_boTrainingNG and TSmartObject(BaseObject).m_boOpenLastContinuous;
end;

(*
  function ConditionOfCheckItemBind(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
  var
  I: Integer;
  nWhere: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
  nBoxItemIndex, nMakeIndex: Integer;
  begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  SmartObject := TSmartObject(BaseObject)
  else
  Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
  nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
  nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  if not (
  ((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or
  ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))
  ) then
  begin
  Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
  Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
  if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
  nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
  if nMakeIndex = 0 then Exit;

  for I := 0 to SmartObject.m_ItemList.Count - 1 do
  begin
  if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
  begin
  UserItem := SmartObject.m_ItemList[I];
  Break;
  end;
  end;
  end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
  UserItem := @SmartObject.m_UseItems[nWhere];
  // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if (UserItem.wIndex <= 0) {or (StdItem = nil)} then
  begin
  Exit;
  end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
  UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
  // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if (UserItem.wIndex <= 0) {or (StdItem = nil)} then
  begin
  Exit;
  end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
  UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
  // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if (UserItem.wIndex <= 0) {or (StdItem = nil)} then
  begin
  //SmartObject.SysMsg('你神佑袋中没有戴指定物品！', c_Red, t_Hint);
  Exit;
  end;
  end
  else
  begin
  if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
  for I := 0 to SmartObject.m_ItemList.Count - 1 do
  begin
  if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
  begin
  UserItem := TPlayObject(SmartObject).m_UpgradeItem;
  break;
  end;
  end;
  end;
  end;

  if UserItem <> nil then
  begin
  Result := UserItem.boIsBind;
  end;
  end;
*)

function ConditionOfCheckItemState(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  nWhere: Integer;
  nState: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
  nBoxItemIndex, nMakeIndex: Integer;
begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nState := QuestConditionInfo.nParam2;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (not (nState in [Integer(Low(TUserItemBindValueType))..Integer(High(TUserItemBindValueType))])) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    if (UserItem.wIndex <= 0) { or (StdItem = nil) } then
    begin
      Exit;
    end;
  end
  else
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    Result := GetUserItemBindValue(UserItem, TUserItemBindValueType(nState));
  end;
end;

function ConditionOfCheckPKPointEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPKPoint: Integer;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nPKPoint := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nPKPoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if SmartObject.m_nPkPoint = nPKPoint then
        Result := True;
    '>':
      if SmartObject.m_nPkPoint > nPKPoint then
        Result := True;
    '<':
      if SmartObject.m_nPkPoint < nPKPoint then
        Result := True;
  else
    if SmartObject.m_nPkPoint >= nPKPoint then
      Result := True;
  end;
end;

function ConditionOfCheckHeroPKPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nPKPoint: Integer;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if PlayObject.m_MyHero = nil then
    Exit;
  SmartObject := TSmartObject(PlayObject.m_MyHero);

  nPKPoint := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nPKPoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if SmartObject.m_nPkPoint = nPKPoint then
        Result := True;
    '>':
      if SmartObject.m_nPkPoint > nPKPoint then
        Result := True;
    '<':
      if SmartObject.m_nPkPoint < nPKPoint then
        Result := True;
  else
    if SmartObject.m_nPkPoint >= nPKPoint then
      Result := True;
  end;
end;

function ConditionOfIsNewServer(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if PlayObject <> nil then
  begin
    if PlayObject.m_btNewServer = QuestConditionInfo.nParam1 then
      Result := True;
  end
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
  begin
    if THeroObject(BaseObject).m_btNewServer = QuestConditionInfo.nParam1 then
      Result := True;
  end;
end;

function ConditionOfCheckSuckDamage(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
  cMethod: Char;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nSuckDamagePoint := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nSuckDamagePoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if SmartObject.m_nSuckDamagePoint = nSuckDamagePoint then
        Result := True;
    '>':
      if SmartObject.m_nSuckDamagePoint > nSuckDamagePoint then
        Result := True;
    '<':
      if SmartObject.m_nSuckDamagePoint < nSuckDamagePoint then
        Result := True;
  else
    if SmartObject.m_nSuckDamagePoint >= nSuckDamagePoint then
      Result := True;
  end;
end;

function ConditionOfCheckHeroSuckDamage(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nSuckDamagePoint: Integer;
  SmartObject: TSmartObject;
  cMethod: Char;
begin
  Result := False;
  if PlayObject.m_MyHero = nil then
    Exit;
  SmartObject := TSmartObject(PlayObject.m_MyHero);

  nSuckDamagePoint := QuestConditionInfo.nParam2;

  if (QuestConditionInfo.sParam1 = '') or (nSuckDamagePoint < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if SmartObject.m_nSuckDamagePoint = nSuckDamagePoint then
        Result := True;
    '>':
      if SmartObject.m_nSuckDamagePoint > nSuckDamagePoint then
        Result := True;
    '<':
      if SmartObject.m_nSuckDamagePoint < nSuckDamagePoint then
        Result := True;
  else
    if SmartObject.m_nSuckDamagePoint >= nSuckDamagePoint then
      Result := True;
  end;
end;

function ConditionOfCheckMapDummyCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCount: Integer;
  cMethod: Char;
  Envir: TEnvirnoment;
begin
  Result := False;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  nCount := QuestConditionInfo.nParam3;
  if (QuestConditionInfo.sParam2 = '') or (nCount < 0) or (Envir = nil) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      if UserEngine.GetDummyObjectCount(Envir) = nCount then
        Result := True;
    '>':
      if UserEngine.GetDummyObjectCount(Envir) > nCount then
        Result := True;
    '<':
      if UserEngine.GetDummyObjectCount(Envir) < nCount then
        Result := True;
  else
    if UserEngine.GetDummyObjectCount(Envir) >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckRecall(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  AObject: TPlayObject;
begin
  Result := False;
  AObject := UserEngine.GetPlayObject(QuestConditionInfo.sParam1);
  if AObject <> nil then
    Result := AObject.m_PEnvir.m_boNORECALL or AObject.m_PEnvir.m_boNODEARRECALL or AObject.m_PEnvir.m_boNOMASTERRECALL;
end;

function ConditionOfCheckCurrentItem(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    Result := SameText(QuestConditionInfo.sParam1, TSmartObject(BaseObject).m_sCurrentItemNewName);
  end;
end;

function ConditionOfIsHigh(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    'L':
      Result := BaseObject = g_HighLevelHuman;
    'P':
      Result := BaseObject = g_HighPKPointHuman;
    'D':
      Result := BaseObject = g_HighDCHuman;
    'M':
      Result := BaseObject = g_HighMCHuman;
    'S':
      Result := BaseObject = g_HighSCHuman;
  end;
end;

function ConditionOfCheckHumBag(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nBagCount, nCount: Integer;
  cMethod: Char;
  AObject: TPlayObject;
begin
  Result := False;
  nCount := QuestConditionInfo.nParam3;

  if (QuestConditionInfo.sParam2 = '') or (nCount < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  AObject := UserEngine.GetPlayObject(QuestConditionInfo.sParam1);
  if AObject <> nil then
  begin
    cMethod := QuestConditionInfo.sParam2[1];
    nBagCount := Max(AObject.GetMaxBagCount - AObject.m_ItemList.Count, 0);
    case cMethod of
      '=':
        if nBagCount = nCount then
          Result := True;
      '>':
        if nBagCount > nCount then
          Result := True;
      '<':
        if nBagCount < nCount then
          Result := True;
    else
      if nBagCount >= nCount then
        Result := True;
    end;
  end;
end;

function ConditionOfCheckHaveHero(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_sHeroName <> '') or (PlayObject.m_sDeputyHeroName <> '');
end;

function ConditionOfCheckHeroLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nLevel: Integer;
begin
  Result := False;
  if PlayObject.m_MyHero = nil then
    Exit;

  nLevel := QuestConditionInfo.nParam2;
  if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_MyHero.m_Abil.Level = nLevel then
        Result := True;
    '>':
      if PlayObject.m_MyHero.m_Abil.Level > nLevel then
        Result := True;
    '<':
      if PlayObject.m_MyHero.m_Abil.Level < nLevel then
        Result := True;
  else
    if PlayObject.m_MyHero.m_Abil.Level >= nLevel then
      Result := True;
  end;
end;

function ConditionOfCheckItemUpgradeCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nUpgradeCount: Integer;
  nCount: Integer;
begin
  Result := False;
  nCount := QuestConditionInfo.nParam4;
  if (not (QuestConditionInfo.nParam1 in [0, 1])) or (not (QuestConditionInfo.nParam2 in [Low(THumanUseItems)..High(THumanUseItems)])) or (QuestConditionInfo.sParam3 = '') or (not (nCount in [0..255])) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  nUpgradeCount := 0;
  case QuestConditionInfo.nParam1 of
    0:
      begin
        if PlayObject.m_UpgradeDialogUserItem.wIndex <= 0 then
        begin
          PlayObject.SysMsg('你没有在升级装备！', c_Red, t_Hint);
          Exit;
        end
        else
          nUpgradeCount := PlayObject.m_UpgradeDialogUserItem.btUpgradeCount;
      end;
    1:
      begin
        if PlayObject.m_UseItems[QuestConditionInfo.nParam2].wIndex <= 0 then
        begin
          // PlayObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
          Exit;
        end
        else
          nUpgradeCount := PlayObject.m_UseItems[QuestConditionInfo.nParam2].btUpgradeCount;
      end;
  end;
  cMethod := QuestConditionInfo.sParam3[1];
  case cMethod of
    '=':
      if nUpgradeCount = nCount then
        Result := True;
    '>':
      if nUpgradeCount > nCount then
        Result := True;
    '<':
      if nUpgradeCount < nCount then
        Result := True;
  else
    if nUpgradeCount >= nCount then
      Result := True;
  end;
end;

function ConditionOfCheckAttackMode(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (not (QuestConditionInfo.nParam1 in [HAM_ALL..HAM_NATION])) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  Result := PlayObject.m_btAttatckMode = QuestConditionInfo.nParam1;
end;

function ConditionOfCheckItemDura(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  sMethod: string;
  I, nCount: Integer;
  UserItem: pTUserItem;
  nWhere, nDura: Integer;
  SmartObject: TSmartObject;
  nBoxItemIndex, nMakeIndex: Integer;
begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  sMethod := QuestConditionInfo.sParam2;
  nCount := QuestConditionInfo.nParam3;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (sMethod = '') or (nCount < 0) or (nCount > 65000) or (not (QuestConditionInfo.nParam4 in [0, 1])) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    if QuestConditionInfo.nParam4 = 0 then
      nDura := UserItem.Dura
    else
      nDura := UserItem.DuraMax;

    cMethod := sMethod[1];
    case cMethod of
      '=':
        Result := nDura = nCount;
      '>':
        Result := nDura > nCount;
      '<':
        Result := nDura < nCount;
    else
      Result := nDura >= nCount;
    end;
  end;
end;

function ConditionOfMonthOfYear(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nMonth: Integer;
begin
  Result := False;
  if (QuestConditionInfo.nParam1 <= 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if QuestConditionInfo.nParam2 > 0 then
  begin
    nMonth := MonthOf(Now);
    Result := (nMonth >= QuestConditionInfo.nParam1) and (nMonth <= QuestConditionInfo.nParam2);
  end
  else
  begin
    Result := MonthOf(Now) = QuestConditionInfo.nParam1;
  end;
end;

function ConditionOfDayOfMonth(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nDay: Integer;
begin
  Result := False;
  if (QuestConditionInfo.nParam1 <= 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if QuestConditionInfo.nParam2 > 0 then
  begin
    nDay := DayOf(Now);
    Result := (nDay >= QuestConditionInfo.nParam1) and (nDay <= QuestConditionInfo.nParam2);
  end
  else
  begin
    Result := DayOf(Now) = QuestConditionInfo.nParam1;
  end;
end;

function ConditionOfCheckNumOfKick(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := PlayObject.m_nKickCount >= QuestConditionInfo.nParam1;
end;

function ConditionOfCheckNation(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if not (QuestConditionInfo.nParam1 in [0..100]) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  Result := PlayObject.m_btNation = QuestConditionInfo.nParam1;
end;

function ConditionOfCheckNationHumCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nCount: Integer;
  nPeoples: Integer;
  NationInfo: pTNationInfo;
begin
  Result := False;
  nCount := QuestConditionInfo.nParam2;
  if QuestConditionInfo.sParam1 = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  NationInfo := g_NationManage.Items[PlayObject.m_btNation];
  if NationInfo <> nil then
    nPeoples := NationInfo.nPeoples
  else
    nPeoples := 0;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if nPeoples = nCount then
        Result := True;
    '>':
      if nPeoples > nCount then
        Result := True;
    '<':
      if nPeoples < nCount then
        Result := True;
  else
    if nPeoples >= nCount then
      Result := True;
  end;
end;

function ConditionOfIsNationKing(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  NationInfo: pTNationInfo;
begin
  Result := False;
  if not (QuestConditionInfo.nParam1 in [0..100]) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.nParam1 > 0 then
  begin
    if PlayObject.m_btNation = QuestConditionInfo.nParam1 then
    begin
      NationInfo := g_NationManage.Items[PlayObject.m_btNation];
      if NationInfo <> nil then
      begin
        Result := SameText(NationInfo.sKingName, PlayObject.m_sCharName);
      end;
    end;
  end
  else if PlayObject.m_btNation > 0 then
  begin
    NationInfo := g_NationManage.Items[PlayObject.m_btNation];
    if NationInfo <> nil then
    begin
      Result := SameText(NationInfo.sKingName, PlayObject.m_sCharName);
    end;
  end;
end;

function ConditionOfCheckNationNameExists(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;

  if Length(QuestConditionInfo.sParam1) = 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  Result := g_NationManage.GetNationIndex(QuestConditionInfo.sParam1) > 0;
end;

// 检测技能点脚本命令 piaoyun 2013-07-27

function ConditionOfCheckTranPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nTranPoint: Integer;
  cMethod: Char;
  UserMagic: pTUserMagic;
  sMagicName: string;
begin
  Result := False;
  sMagicName := QuestConditionInfo.sParam1;
  nTranPoint := QuestConditionInfo.nParam3;
  UserMagic := nil;

  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    UserMagic := PlayObject.GetMagicInfo(sMagicName)
  else if BaseObject.m_btRaceServer = RC_HEROOBJECT then
    UserMagic := THeroObject(BaseObject).FindMagic(sMagicName);

  if UserMagic = nil then
    Exit;

  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      if UserMagic.nTranPoint = nTranPoint then
        Result := True;
    '>':
      if UserMagic.nTranPoint > nTranPoint then
        Result := True;
    '<':
      if UserMagic.nTranPoint < nTranPoint then
        Result := True;
  else
    if UserMagic.nTranPoint >= nTranPoint then
      Result := True;
  end;
end;

/// /////////////// 连击、经络相关 piaoyun 2013-08-16 ///////////////////////////
// function TNormNpc.ConditionOfCHECKKIMNEEDLE(PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;

function ConditionOfCheckKimneedle(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  ICount, iParam, iDura: Integer;
begin
  Result := False;
  try
    // ======================================================================
    { if QuestConditionInfo.sParam6 = '88' then
      begin                                                                                         //无忧加对非本人检测脚本 20100624
      PlayObject := UserEngine.GetPlayObject(GetLineVariableText(PlayObject, QuestConditionInfo.sParam7));
      if playobject = nil then exit;
      end; }
    // ======================================================================
    Result := False;
    if QuestConditionInfo.nParam2 > 0 then
    begin
      ICount := QuestConditionInfo.nParam2;
      PlayObject.QuestCheckItem(QuestConditionInfo.sParam1, ICount, iParam, iDura);
      if ICount >= QuestConditionInfo.nParam2 then
      begin
        Result := True;
      end; // 函数暂时使用 ChenkItem  以后加上  20091218 科技学院
    end;
  except
    MainOutMessage('{异常} TNormNpc.ConditionOfCHECKKIMNEEDLE');
  end;
end;

// 检测人物是否已经创建指定名称的副本 chongchong 2013-09-06

function ConditionOfCanMoveEctype(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sFBName: string;
begin
  Result := False;
  sFBName := QuestConditionInfo.sParam1;
  if (sFBName = '') then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if (PlayObject.m_GroupOwner <> nil) and (TPlayObject(PlayObject.m_GroupOwner).m_FBEnvir <> nil) and (CompareText(sFBName, TPlayObject(PlayObject.m_GroupOwner).m_FBEnvir.m_sFBName) = 0) then
    Result := True
  else if (PlayObject.m_FBEnvir <> nil) and (CompareText(sFBName, PlayObject.m_FBEnvir.m_sFBName) = 0) then
    Result := True;
end;

// 检测地图标识状态 chongchong 2013-09-06

function ConditionOfCheckMapQuest(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
end;

// 检测人物多长时间没有移动 chongchong 2013-10-28
function ConditionOfCheckStationTime(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nCount: Integer;
begin
  Result := False;
  if Length(QuestConditionInfo.sParam1) = 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  nCount := QuestConditionInfo.nParam2 * 1000 * 60;
  if QuestConditionInfo.nParam2 <= 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      Result := MyGetTickCount - PlayObject.m_dwMoveTick = nCount;
    '>':
      Result := MyGetTickCount - PlayObject.m_dwMoveTick > nCount;
    '<':
      Result := MyGetTickCount - PlayObject.m_dwMoveTick < nCount;
  else
    Result := MyGetTickCount - PlayObject.m_dwMoveTick >= nCount;
  end;
end;

// 检测自身血量百分比 chongchong 2013-10-29
function ConditionOfCheckHpper(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nBaseValue: Integer;
begin
  Result := False;
  if Length(QuestConditionInfo.sParam1) = 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];

  if QuestConditionInfo.nParam3 = 0 then
    nBaseValue := 100
  else if QuestConditionInfo.nParam3 = 1 then
    nBaseValue := 1000
  else if QuestConditionInfo.nParam3 = 2 then
    nBaseValue := 10000
  else
    nBaseValue := 100;

  case cMethod of
    '=':
      Result := BaseObject.m_WAbil.HP = Round(BaseObject.m_WAbil.MaxHP / nBaseValue * QuestConditionInfo.nParam2);
    '>':
      Result := BaseObject.m_WAbil.HP > Round(BaseObject.m_WAbil.MaxHP / nBaseValue * QuestConditionInfo.nParam2);
    '<':
      Result := BaseObject.m_WAbil.HP < Round(BaseObject.m_WAbil.MaxHP / nBaseValue * QuestConditionInfo.nParam2);
  else
    Result := BaseObject.m_WAbil.HP >= Round(BaseObject.m_WAbil.MaxHP / nBaseValue * QuestConditionInfo.nParam2);
  end;
end;

// 检测自身MP百分比 chongchong 2013-10-29
function ConditionOfCheckMpper(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
  nBaseValue: Integer;
begin
  Result := False;
  if Length(QuestConditionInfo.sParam1) = 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];

  if QuestConditionInfo.nParam3 = 0 then
    nBaseValue := 100
  else if QuestConditionInfo.nParam3 = 1 then
    nBaseValue := 1000
  else if QuestConditionInfo.nParam3 = 2 then
    nBaseValue := 10000
  else
    nBaseValue := 100;

  case cMethod of
    '=':
      Result := BaseObject.m_WAbil.MP = Round(BaseObject.m_WAbil.MaxMP / nBaseValue * QuestConditionInfo.nParam2);
    '>':
      Result := BaseObject.m_WAbil.MP > Round(BaseObject.m_WAbil.MaxMP / nBaseValue * QuestConditionInfo.nParam2);
    '<':
      Result := BaseObject.m_WAbil.MP < Round(BaseObject.m_WAbil.MaxMP / nBaseValue * QuestConditionInfo.nParam2);
  else
    Result := BaseObject.m_WAbil.MP >= Round(BaseObject.m_WAbil.MaxMP / nBaseValue * QuestConditionInfo.nParam2);
  end;
end;

// 检查夫妻另一半是否在线 / 在同一地图 chongchong 2013-12-17
// CheckDearOnline 人物名称(不填表示检查自己的配偶在不在线，填则表示检查指定人的配偶在不在线）
function ConditionOfCheckDearOnLine(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  DestName, DearName: string;
  DestObj, ADear: TPlayObject;
begin
  Result := False;
  DestName := QuestConditionInfo.sParam1;
  if Length(DestName) = 0 then
    DestObj := PlayObject
  else
  begin
    DestObj := UserEngine.GetPlayObject(DestName);
    if DestObj = nil then
      Exit;
  end;

  DearName := DestObj.m_sDearName;
  if Length(DearName) = 0 then
    Exit;

  ADear := UserEngine.GetPlayObject(DearName);
  if ADear = nil then
    Exit;

  Result := not ADear.m_boOffline;
end;

// 检查夫妻另一半是否在某个地图 chongchong 2013-12-17

// CheckDearOnMap 地图号(self或不填，自己所在的地图，否则为指定地图） 人物名称(不填表示检查自己的配偶在某个地图，否则检查指定人的配偶是否在某个地图）
function ConditionOfCheckDearOnMap(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  MapID, DestName, DearName: string;
  DestObj, ADear: TPlayObject;
  Envir: TEnvirnoment;
begin
  Result := False;
  MapID := QuestConditionInfo.sParam1;
  DestName := QuestConditionInfo.sParam2;

  if Length(MapID) = 0 then
  begin
    Envir := PlayObject.m_PEnvir;
    DestObj := PlayObject;
  end
  else
  begin
    if SameText(MapID, 'self') then
      Envir := PlayObject.m_PEnvir
    else
      Envir := g_MapManager.FindMap(MapID);
    if Envir = nil then
      Exit;

    if Length(DestName) = 0 then
      DestObj := PlayObject
    else
    begin
      DestObj := UserEngine.GetPlayObject(DestName);
      if DestObj = nil then
        Exit;
    end;
  end;

  DearName := DestObj.m_sDearName;
  if Length(DearName) = 0 then
    Exit;

  ADear := UserEngine.GetPlayObject(DearName);
  if ADear = nil then
    Exit;
  if ADear.m_boOffline then
    Exit;

  Result := ADear.m_PEnvir = Envir;
end;

function ConditionOfCheckRepairAllGold(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  NeedDura: Integer; // 修复装备所需持久点
  StdItem: pTStdItem;
  SmartObject: TSmartObject;
  TotalGold, nPrice, nRepairPrice: Integer;
  sVarName: string;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  TotalGold := 0;
  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
    StdItem := UserEngine.GetStdItem(SmartObject.m_UseItems[I].wIndex);

    if (SmartObject.m_UseItems[I].wIndex > 0) and (SmartObject.m_UseItems[I].Dura < SmartObject.m_UseItems[I].DuraMax) and (StdItem <> nil) and (not (StdItem.StdMode in [7, 2, 25])) then
    begin
      if g_ItemRules.Get(SmartObject.m_UseItems[I].wIndex, 3) then
        Continue;
      if (GetUserItemBindValue(@SmartObject.m_UseItems[I], ubNoRepair) and SmartObject.m_UseItems[I].boIsBind) then
        Continue;

      // 禁止叠加物品修理 chongchong 2014-09-26
      if CheckOverLapItem(StdItem) then
        Continue;

      NeedDura := Max(0, SmartObject.m_UseItems[I].DuraMax - SmartObject.m_UseItems[I].Dura);
      if NeedDura = 0 then
        Continue;

      {
        if Npc is TMerchant then
        nPrice := TMerchant(Npc).GetUserPrice(PlayObject, TMerchant(Npc).GetUserItemPrice(@SmartObject.m_UseItems[I]))
        else
      }
      nPrice := StdItem.Price;

      if SmartObject.m_UseItems[I].DuraMax > 0 then
        nRepairPrice := Round(nPrice div 3 / SmartObject.m_UseItems[I].DuraMax * NeedDura)
      else
        nRepairPrice := nPrice;

      TotalGold := TotalGold + nRepairPrice;
    end;
  end;

  TotalGold := Round(TotalGold * g_Config.nSuperRepairPriceRate);
  if SmartObject.m_nGold >= TotalGold then
    Result := True;

  sVarName := QuestConditionInfo.sRawParam1; // 变量名
  if Length(sVarName) <> 0 then
    Npc.SetValNameValue(PlayObject, sVarName, '', TotalGold);
end;

// 检测神佑袋某个是否开启 chongchong 2014-04-17
function ConditionOfCheckOpenGodBless(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  if (QuestConditionInfo.nParam1 < Low(THumanGodBlessItems)) or (QuestConditionInfo.nParam1 > High(THumanGodBlessItems)) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  Result := SmartObject.m_GodBlessItemsState[QuestConditionInfo.nParam1] = 1;
end;

function ConditionOfCheckShowGodBless(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  Result := SmartObject.m_boShowGodBless;
end;

function ConditionOfCheckClientWidth(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;

  if (Length(QuestConditionInfo.sParam1) = 0) or (Length(QuestConditionInfo.sParam1) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      Result := PlayObject.m_nClientWidth = QuestConditionInfo.nParam2;
    '>':
      Result := PlayObject.m_nClientWidth > QuestConditionInfo.nParam2;
    '<':
      Result := PlayObject.m_nClientWidth < QuestConditionInfo.nParam2;
  else
    Result := PlayObject.m_nClientWidth >= QuestConditionInfo.nParam2;
  end;
end;

function ConditionOfCheckClientHeight(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;

  if (Length(QuestConditionInfo.sParam1) = 0) or (Length(QuestConditionInfo.sParam2) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      Result := PlayObject.m_nClientHeight = QuestConditionInfo.nParam2;
    '>':
      Result := PlayObject.m_nClientHeight > QuestConditionInfo.nParam2;
    '<':
      Result := PlayObject.m_nClientHeight < QuestConditionInfo.nParam2;
  else
    Result := PlayObject.m_nClientHeight >= QuestConditionInfo.nParam2;
  end;
end;

function ConditionOfCheckFengHao(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
var
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  if (Length(QuestConditionInfo.sParam1) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  for I := 0 to SmartObject.m_FengHaoItems.Count - 1 do
  begin
    UserItem := SmartObject.m_FengHaoItems[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem <> nil) and SameText(StdItem.Name, QuestConditionInfo.sParam1) then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function ConditionOfCheckFengHaoCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  cMethod: Char;
begin
  Result := False;

  if (Length(QuestConditionInfo.sParam1) = 0) or (Length(QuestConditionInfo.sParam2) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      Result := PlayObject.m_FengHaoItems.Count = QuestConditionInfo.nParam2;
    '>':
      Result := PlayObject.m_FengHaoItems.Count > QuestConditionInfo.nParam2;
    '<':
      Result := PlayObject.m_FengHaoItems.Count < QuestConditionInfo.nParam2;
  else
    Result := PlayObject.m_FengHaoItems.Count >= QuestConditionInfo.nParam2;
  end;
end;

function ConditionOfCheckNewFengHaoValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sItemName: string;
  I, nValType, nPoint: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  StdItem: pTStdItem;
  SmartObject: TSmartObject;
begin
  Result := False;
  sItemName := QuestConditionInfo.sParam1;
  nValType := QuestConditionInfo.nParam2;
  nPoint := QuestConditionInfo.nParam4;

  if (QuestConditionInfo.sParam3 = '') or (nValType < 0) or (nValType > 24) or (Length(sItemName) = 0) or (nPoint < 0) or (nPoint > 100) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    Exit;

  SmartObject := TSmartObject(BaseObject);

  cMethod := QuestConditionInfo.sParam3[1];
  for I := 0 to SmartObject.m_FengHaoItems.Count - 1 do
  begin
    UserItem := SmartObject.m_FengHaoItems.Items[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);

    if (StdItem <> nil) and SameText(StdItem.Name, sItemName) then
    begin
      case cMethod of
        '=':
          if UserItem.btNewValue[nValType] = nPoint then
            Result := True;
        '>':
          if UserItem.btNewValue[nValType] > nPoint then
            Result := True;
        '<':
          if UserItem.btNewValue[nValType] < nPoint then
            Result := True;
      else
        if UserItem.btNewValue[nValType] >= nPoint then
          Result := True;
      end;

      Break;
    end;
  end;
end;

function ConditionOfCheckOnHorse(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    Result := (BaseObject as TPlayObject).m_boOnHorse;
end;

function ConditionOfCheckKillByHum(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if BaseObject.m_boDeath and (BaseObject.m_LastHiter <> nil) then
    Result := BaseObject.m_LastHiter.m_btRaceServer = RC_PLAYOBJECT;
end;

function ConditionOfCheckMasterOnline(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  MasterRankInfo: pTMasterRankInfo;
begin
  Result := False;
  // 如果是师傅，要指定徒弟编号
  if PlayObject.m_boMaster then
  begin
    if (QuestConditionInfo.nParam1 >= 0) or (QuestConditionInfo.nParam1 <= PlayObject.m_MasterNoList.Count) then
    begin
      MasterRankInfo := PlayObject.m_MasterNoList.Items[Max(QuestConditionInfo.nParam1 - 1, 0)];
      Result := UserEngine.GetPlayObject(MasterRankInfo.sChrName) <> nil;
    end;
  end
  else if Length(PlayObject.m_sMasterName) > 0 then
  begin
    Result := UserEngine.GetPlayObject(PlayObject.m_sMasterName) <> nil;
  end;
end;

function ConditionOfCheckMasterOnMap(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  MasterRankInfo: pTMasterRankInfo;
  Envir: TEnvirnoment;
  FindObj: TPlayObject;
begin
  Result := False;
  Envir := nil;

  if SameText(QuestConditionInfo.sParam1, 'self') then
    Envir := PlayObject.m_PEnvir
  else
  begin
    if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
      Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);

    if Envir = nil then
      Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);
  end;

  // 如果是师傅，要指定徒弟编号
  if PlayObject.m_boMaster then
  begin
    if (QuestConditionInfo.nParam2 >= 0) or (QuestConditionInfo.nParam2 <= PlayObject.m_MasterNoList.Count) then
    begin
      MasterRankInfo := PlayObject.m_MasterNoList.Items[Max(QuestConditionInfo.nParam2, 0)];
      FindObj := UserEngine.GetPlayObject(MasterRankInfo.sChrName);
      Result := (FindObj <> nil) and (FindObj.m_PEnvir = Envir);
    end;
  end
  else if Length(PlayObject.m_sMasterName) > 0 then
  begin
    FindObj := UserEngine.GetPlayObject(PlayObject.m_sMasterName);
    Result := (FindObj <> nil) and (FindObj.m_PEnvir = Envir);
  end;
end;

function ConditionOfCheckIsMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_boMaster) and (PlayObject.m_MasterNoList.Count > 0);
end;

function ConditionOfCheckIsPrentice(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (not PlayObject.m_boMaster) and (PlayObject.m_sMasterName <> '');
end;

function ConditionOfPoseHaveMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
  PosePlayer: TPlayObject;
begin
  Result := False;
  PoseHuman := BaseObject.GetPoseCreate();
  if PoseHuman.m_btRaceServer = RC_PLAYOBJECT then
  begin
    PosePlayer := TPlayObject(PoseHuman);
    Result := (not PosePlayer.m_boMaster) and (PosePlayer.m_sMasterName <> '');
  end;
end;

function ConditionOfPoseHavePrentice(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
  PosePlayer: TPlayObject;
begin
  Result := False;
  PoseHuman := BaseObject.GetPoseCreate();
  if PoseHuman.m_btRaceServer = RC_PLAYOBJECT then
  begin
    PosePlayer := TPlayObject(PoseHuman);
    Result := (PosePlayer.m_boMaster) and (PosePlayer.m_MasterNoList.Count > 0);
  end;
end;

function ConditionOfCheckPoseIsPrentice(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
  PosePlayer: TPlayObject;
begin
  Result := False;
  PoseHuman := BaseObject.GetPoseCreate();
  if PoseHuman.m_btRaceServer = RC_PLAYOBJECT then
  begin
    PosePlayer := TPlayObject(PoseHuman);
    Result := (not PosePlayer.m_boMaster) and (PosePlayer.m_sMasterName = PlayObject.m_sCharName);
  end;
end;

function ConditionOfCheckPoseIsMaster(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  PoseHuman: TBaseObject;
  PosePlayer: TPlayObject;
begin
  Result := False;
  PoseHuman := BaseObject.GetPoseCreate();
  if PoseHuman.m_btRaceServer = RC_PLAYOBJECT then
  begin
    PosePlayer := TPlayObject(PoseHuman);
    Result := (PosePlayer.m_boMaster) and (PlayObject.m_sMasterName = PosePlayer.m_sCharName);
  end;
end;

// 检测开槽数量 chongchong
function ConditionOfCheckFluteCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  nValue: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nValue := QuestConditionInfo.nParam3;

  if (QuestConditionInfo.sParam2 = '') or (nValue < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    cMethod := QuestConditionInfo.sParam2[1];

    case cMethod of
      '=':
        Result := UserItem.btFluteCount = nValue;
      '>':
        Result := UserItem.btFluteCount > nValue;
      '<':
        Result := UserItem.btFluteCount < nValue;
    else
      Result := UserItem.btFluteCount >= nValue;
    end;
  end;
end;

// 检测镶嵌宝石数量 chongchong 2015-02-17
function ConditionOfCheckItemStoneCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  nValue: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -2;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -2);

  nValue := QuestConditionInfo.nParam3;

  if (QuestConditionInfo.sParam2 = '') or (nValue < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else
  begin
    for I := 0 to PlayObject.m_ItemList.Count - 1 do
    begin
      if PlayObject.m_ItemList.Items[I] = PlayObject.m_UpgradeItem then
      begin
        UserItem := PlayObject.m_UpgradeItem;
        Break;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    cMethod := QuestConditionInfo.sParam2[1];

    case cMethod of
      '=':
        Result := GetItemStoneCount(UserItem) = nValue;
      '>':
        Result := GetItemStoneCount(UserItem) > nValue;
      '<':
        Result := GetItemStoneCount(UserItem) < nValue;
    else
      Result := GetItemStoneCount(UserItem) >= nValue;
    end;
  end;
end;

function ConditionOfCheckNpcSetImage(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := Npc.m_nEffigyState.Value1 <> 0;
end;

function ConditionOfCheckUpgradeCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  nValue: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  nCount: Integer;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  nValue := QuestConditionInfo.nParam3;

  if (QuestConditionInfo.sParam2 = '') or (nValue < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -2) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end
  end
  else if nWhere = -2 then
  begin
    nCount := 0;

    for nWhere := Low(THumanUseItems) to High(THumanUseItems) do
    begin
      UserItem := @SmartObject.m_UseItems[nWhere];
      if (UserItem.wIndex > 0) then
        Inc(nCount, UserItem.btUpgradeCount);
    end;

    for nWhere := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
    begin
      UserItem := @SmartObject.m_JewelryBoxItems[nWhere];
      if (UserItem.wIndex > 0) then
        Inc(nCount, UserItem.btUpgradeCount);
    end;

    for nWhere := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
    begin
      UserItem := @SmartObject.m_GodBlessItems[nWhere];
      if (UserItem.wIndex > 0) then
        Inc(nCount, UserItem.btUpgradeCount);
    end;

    cMethod := QuestConditionInfo.sParam2[1];

    case cMethod of
      '=':
        Result := nCount = nValue;
      '>':
        Result := nCount > nValue;
      '<':
        Result := nCount < nValue;
    else
      Result := nCount >= nValue;
    end;

    Exit;
  end;

  if UserItem <> nil then
  begin
    cMethod := QuestConditionInfo.sParam2[1];

    case cMethod of
      '=':
        Result := UserItem.btUpgradeCount = nValue;
      '>':
        Result := UserItem.btUpgradeCount > nValue;
      '<':
        Result := UserItem.btUpgradeCount < nValue;
    else
      Result := UserItem.btUpgradeCount >= nValue;
    end;
  end;
end;

function ConditionOfCheckBoxItemCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nIndex, nCount, nMakeIndex: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
begin
  Result := False;
  nIndex := QuestConditionInfo.nParam1;
  nCount := QuestConditionInfo.nParam2;

  if (nIndex < 0) or (nIndex > High(THumanItemBoxItems)) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  nMakeIndex := PlayObject.m_ItemBoxItems[nIndex];
  if nMakeIndex = 0 then
    Exit;

  for I := 0 to PlayObject.m_ItemList.Count - 1 do
  begin
    UserItem := pTUserItem(PlayObject.m_ItemList[I]);

    if UserItem.MakeIndex = nMakeIndex then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem <> nil then
      begin
        if CheckOverLapItem(StdItem) then
          Result := UserItem.Dura + 1 >= nCount
        else
          Result := True;
      end;

      Break;
    end;
  end;
end;

function ConditionOfCheckBagItemCountEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, J, nCheckCount, nCount: Integer;
  sItemName: string;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  FindInItemBox: Boolean;
begin
  Result := False;
  sItemName := QuestConditionInfo.sParam1;
  nCheckCount := QuestConditionInfo.nParam2;

  if (Length(sItemName) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  nCount := 0;
  for I := 0 to PlayObject.m_ItemList.Count - 1 do
  begin
    UserItem := pTUserItem(PlayObject.m_ItemList[I]);
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem <> nil) and SameText(StdItem.Name, sItemName) then
    begin
      FindInItemBox := False;
      for J := Low(PlayObject.m_ItemBoxItems) to High(PlayObject.m_ItemBoxItems) do
      begin
        if PlayObject.m_ItemBoxItems[J] = UserItem.MakeIndex then
        begin
          FindInItemBox := True;
          Break;
        end;
      end;

      if not FindInItemBox then
      begin
        if CheckOverLapItem(StdItem) then
          Inc(nCount, UserItem.Dura + 1)
        else
          Inc(nCount, 1);
      end;
    end;
  end;

  Result := nCount >= nCheckCount;
end;

function ConditionOfCheckItemHasStone(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
  StdItem: pTStdItem;
  StoneName: string;
  CheckCount, TotalCount: Integer;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  StoneName := QuestConditionInfo.sParam2;
  CheckCount := QuestConditionInfo.nParam3;

  if (StoneName = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end
  end;

  if UserItem <> nil then
  begin
    TotalCount := 0;
    for I := 0 to UserItem.btFluteCount - 1 do
    begin
      if (UserItem.Flutes[I].GemIndex > 0) then
      begin
        StdItem := UserEngine.GetStdItem(UserItem.Flutes[I].GemIndex);
        if (StdItem <> nil) and (StdItem.StdMode = 46) and (StdItem.Shape = 3) then
        begin
          if SameText(StdItem.Name, StoneName) then
          begin
            Inc(TotalCount, Max(1, UserItem.Flutes[I].GemCount));
          end;
        end;
      end;
    end;

    Result := TotalCount >= CheckCount;
  end;
end;

function ConditionOfCheckItemFluteIndexHasStone(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex, nStoneIndex: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  nStoneIndex := QuestConditionInfo.nParam2;
  if (nStoneIndex < 1) or (nStoneIndex > MAX_FLUTE_COUNT) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := TPlayObject(SmartObject).m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(SmartObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := SmartObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end
  end;

  if UserItem <> nil then
  begin
    Result := (UserItem.Flutes[nStoneIndex - 1].GemIndex > 0);
  end;
end;

// 检测全身镶嵌指定宝石数量 chongchong 2015-02-17
function ConditionOfCheckStoneCount(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, J: Integer;
  StoneName: string;
  nCheckValue, nAllStoneCount: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nAllStoneCount := 0;
  StoneName := QuestConditionInfo.sParam1;
  nCheckValue := QuestConditionInfo.nParam3;

  if (StoneName = '') or (QuestConditionInfo.sParam2 = '') or (nCheckValue < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  for I := Low(THumanUseItems) to High(THumanUseItems) do
  begin
    UserItem := @SmartObject.m_UseItems[I];
    if (UserItem.wIndex > 0) then
    begin
      for J := 0 to UserItem.btFluteCount - 1 do
      begin
        if (J in [0..MAX_FLUTE_COUNT - 1]) and (UserItem.Flutes[J].GemIndex > 0) then
        begin
          StdItem := UserEngine.GetStdItem(UserItem.Flutes[J].GemIndex);
          if (StdItem <> nil) and (StdItem.StdMode = 46) and (StdItem.Shape = 3) then
          begin
            if SameText(StdItem.Name, StoneName) then
            begin
              Inc(nAllStoneCount, Max(1, UserItem.Flutes[J].GemCount));
            end;
          end;
        end;
      end;
    end;
  end;

  for I := Low(TJewelryBoxItems) to High(TJewelryBoxItems) do
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[I];

    if (UserItem.wIndex > 0) then
    begin
      for J := 0 to UserItem.btFluteCount - 1 do
      begin
        if (J in [0..MAX_FLUTE_COUNT - 1]) and (UserItem.Flutes[J].GemIndex > 0) then
        begin
          StdItem := UserEngine.GetStdItem(UserItem.Flutes[J].GemIndex);
          if (StdItem <> nil) and (StdItem.StdMode = 46) and (StdItem.Shape = 3) then
          begin
            if SameText(StdItem.Name, StoneName) then
            begin
              Inc(nAllStoneCount, Max(1, UserItem.Flutes[J].GemCount));
            end;
          end;
        end;
      end;
    end;
  end;

  for I := Low(TGodBlessItems) to High(TGodBlessItems) do
  begin
    UserItem := @SmartObject.m_GodBlessItems[I];

    if (UserItem.wIndex > 0) then
    begin
      for J := 0 to UserItem.btFluteCount - 1 do
      begin
        if (J in [0..MAX_FLUTE_COUNT - 1]) and (UserItem.Flutes[J].GemIndex > 0) then
        begin
          StdItem := UserEngine.GetStdItem(UserItem.Flutes[J].GemIndex);
          if (StdItem <> nil) and (StdItem.StdMode = 46) and (StdItem.Shape = 3) then
          begin
            if SameText(StdItem.Name, StoneName) then
            begin
              Inc(nAllStoneCount, Max(1, UserItem.Flutes[J].GemCount));
            end;
          end;
        end;
      end;
    end;
  end;

  cMethod := QuestConditionInfo.sParam2[1];
  case cMethod of
    '=':
      Result := nAllStoneCount = nCheckValue;
    '>':
      Result := nAllStoneCount > nCheckValue;
    '<':
      Result := nAllStoneCount < nCheckValue;
  else
    Result := nAllStoneCount >= nCheckValue;
  end;
end;

function ConditionOfCheckActiveFengHao(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  SmartObject: TSmartObject;
  FengHaoName: string;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  FengHaoName := QuestConditionInfo.sParam1;
  if Length(FengHaoName) = 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (SmartObject.m_ActiveFengHao >= 0) and (SmartObject.m_ActiveFengHao < SmartObject.m_FengHaoItems.Count) then
  begin
    UserItem := SmartObject.m_FengHaoItems.Items[SmartObject.m_ActiveFengHao];
    if UserItem <> nil then
    begin
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if (StdItem <> nil) and SameText(FengHaoName, StdItem.Name) then
      begin
        Result := True;
      end;
    end
  end;
end;

function ConditionOfCheckCustomItemProgressbar(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  nProgressIndex: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  nProgressIndex := QuestConditionInfo.nParam2;

  if (nProgressIndex < 0) or (nProgressIndex > 1) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end;
  end;

  if UserItem <> nil then
  begin
    Result := UserItem.Progress[nProgressIndex].boOpen;
  end;
end;

function ConditionOfCheckCustomItemProgressbarValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  nProgressIndex, nValueType, nValue, nCheckValue: Integer;
  UserItem: pTUserItem;
  cMethod: Char;
  SmartObject: TSmartObject;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  nProgressIndex := QuestConditionInfo.nParam2;
  nValueType := QuestConditionInfo.nParam3;
  nValue := QuestConditionInfo.nParam5;

  if (QuestConditionInfo.sParam4 = '') or (nProgressIndex < 0) or (nProgressIndex > 1) or (nValueType < 0) or (nValueType > 3) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end
  end;

  if UserItem <> nil then
  begin
    cMethod := QuestConditionInfo.sParam4[1];

    if nValueType = 0 then
    begin
      case cMethod of
        '=':
          Result := UserItem.Progress[nProgressIndex].wMax = nValue;
        '>':
          Result := UserItem.Progress[nProgressIndex].wMax > nValue;
        '<':
          Result := UserItem.Progress[nProgressIndex].wMax < nValue;
      else
        Result := UserItem.Progress[nProgressIndex].wMax >= nValue;
      end;
    end
    else if nValueType = 1 then
    begin
      case cMethod of
        '=':
          Result := UserItem.Progress[nProgressIndex].wValue = nValue;
        '>':
          Result := UserItem.Progress[nProgressIndex].wValue > nValue;
        '<':
          Result := UserItem.Progress[nProgressIndex].wValue < nValue;
      else
        Result := UserItem.Progress[nProgressIndex].wValue >= nValue;
      end;
    end
    else if nValueType = 2 then
    begin
      if UserItem.Progress[nProgressIndex].wMax = 0 then
        nCheckValue := 0
      else
        nCheckValue := Trunc(UserItem.Progress[nProgressIndex].wValue / UserItem.Progress[nProgressIndex].wMax * 100);

      case cMethod of
        '=':
          Result := nCheckValue = nValue;
        '>':
          Result := nCheckValue > nValue;
        '<':
          Result := nCheckValue < nValue;
      else
        Result := nCheckValue >= nValue;
      end;
    end
    else if nValueType = 3 then
    begin
      case cMethod of
        '=':
          Result := UserItem.Progress[nProgressIndex].wLevel = nValue;
        '>':
          Result := UserItem.Progress[nProgressIndex].wLevel > nValue;
        '<':
          Result := UserItem.Progress[nProgressIndex].wLevel < nValue;
      else
        Result := UserItem.Progress[nProgressIndex].wLevel >= nValue;
      end;
    end;
  end;
end;

function ConditionOfCheckCustomItemValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
  nPropertyIndex, nValue, nValueIndex: Integer;
  cMethod: Char;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  nPropertyIndex := QuestConditionInfo.nParam2;
  nValue := QuestConditionInfo.nParam4;
  nValueIndex := QuestConditionInfo.nParam5;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (nPropertyIndex < 0) or (nPropertyIndex >= ITEM_PROP_COUNT) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.sParam3 = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (nValueIndex < 0) or (nValueIndex >= ITEM_PROP_VALUES_COUNT) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end
  end;

  cMethod := QuestConditionInfo.sParam3[1];
  if UserItem <> nil then
  begin
    case cMethod of
      '=':
        Result := UserItem.CustomProperty.Properties[nPropertyIndex].nValues[nValueIndex] = nValue;
      '>':
        Result := UserItem.CustomProperty.Properties[nPropertyIndex].nValues[nValueIndex] > nValue;
      '<':
        Result := UserItem.CustomProperty.Properties[nPropertyIndex].nValues[nValueIndex] < nValue;
    else
      Result := UserItem.CustomProperty.Properties[nPropertyIndex].nValues[nValueIndex] >= nValue;
    end;
  end;
end;

function ConditionOfCheckCustomItemBindType(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, nWhere, nBoxItemIndex, nMakeIndex: Integer;
  UserItem: pTUserItem;
  SmartObject: TSmartObject;
  nPropertyIndex, nValue: Integer;
  cMethod: Char;
begin
  Result := False;
  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  nBoxItemIndex := -1;
  nWhere := -3;
  I := Pos('boxitem', LowerCase(QuestConditionInfo.sParam1));
  if I = 1 then
    nBoxItemIndex := StrToIntDef(Copy(QuestConditionInfo.sParam1, Length('boxitem') + 1, MaxInt), -1)
  else
    nWhere := StrToIntDef(QuestConditionInfo.sParam1, -3);

  nPropertyIndex := QuestConditionInfo.nParam2;
  nValue := QuestConditionInfo.nParam4;

  if not (((nWhere >= -1) and (nWhere <= U_GODBLESSITEM12)) or ((nBoxItemIndex >= 0) and (nBoxItemIndex <= High(THumanItemBoxItems)))) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if (nPropertyIndex < 0) or (nPropertyIndex >= ITEM_PROP_COUNT) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.sParam3 = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  UserItem := nil;
  if nBoxItemIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)] then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      nMakeIndex := PlayObject.m_ItemBoxItems[nBoxItemIndex];
      if nMakeIndex = 0 then
        Exit;

      for I := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
        begin
          UserItem := PlayObject.m_ItemList[I];
          Break;
        end;
      end;
    end;
  end
  else if nWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @SmartObject.m_UseItems[nWhere];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_JEWELRYITEM1..U_JEWELRYITEM6] then
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[nWhere - U_JEWELRYITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere in [U_GODBLESSITEM1..U_GODBLESSITEM12] then
  begin
    UserItem := @SmartObject.m_GodBlessItems[nWhere - U_GODBLESSITEM1];
    // StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (UserItem.wIndex <= 0) then
    begin
      // SmartObject.SysMsg('你身上没有戴指定物品！', c_Red, t_Hint);
      Exit;
    end;
  end
  else if nWhere = -1 then
  begin
    if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      for I := 0 to SmartObject.m_ItemList.Count - 1 do
      begin
        if SmartObject.m_ItemList.Items[I] = TPlayObject(SmartObject).m_UpgradeItem then
        begin
          UserItem := TPlayObject(SmartObject).m_UpgradeItem;
          Break;
        end;
      end;
    end
  end;

  cMethod := QuestConditionInfo.sParam3[1];
  if UserItem <> nil then
  begin
    case cMethod of
      '=':
        Result := UserItem.CustomProperty.Properties[nPropertyIndex].btBindType = nValue;
      '>':
        Result := UserItem.CustomProperty.Properties[nPropertyIndex].btBindType > nValue;
      '<':
        Result := UserItem.CustomProperty.Properties[nPropertyIndex].btBindType < nValue;
    else
      Result := UserItem.CustomProperty.Properties[nPropertyIndex].btBindType >= nValue;
    end;
  end;
end;

function ConditionOfCanVerifyCode(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := Length(PlayObject.m_sVerifyCode) = 0;
end;

function ConditionOfCheckVerifyCode(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := PlayObject.m_boVerifyCodeCheckOK;
end;

function ConditionOfCheckMirrorMap(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  S1: string;
  N1: Integer;
  Envir: TEnvirnoment;
  IsBreakParseVar: Boolean;
begin
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
  begin
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
    if Envir = nil then
    begin
      S1 := '';
      Npc.GetVarValue(PlayObject, QuestConditionInfo.sRawParam1, S1, N1, IsBreakParseVar);
      if Length(S1) > 0 then
        Envir := g_MapManager.FindMap(S1);
    end;
  end;

  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  Result := (Envir <> nil) and (Envir.m_boMirror);
end;

function ConditionOfCheckMobileNumber(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := Length(PlayObject.m_sMobileNumber) > 0;
end;

function ConditionOfCheckMobileBind(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (Length(PlayObject.m_sMobileNumber) > 0) and PlayObject.m_boMobileBind;
end;

function ConditionOfCheckStorageOpen(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;
  if (QuestConditionInfo.nParam1 < 2) and (QuestConditionInfo.nParam1 > 4) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  Result := PlayObject.m_boStorageOpen[QuestConditionInfo.nParam1 - 1];
end;

function ConditionOfCheckScriptParam(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sTemp: WideString;
  I, II, Index: Integer;
  S: array[0..20] of string;
begin
  Result := False;

  if Length(QuestConditionInfo.sParam1) = 0 then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  sTemp := QuestConditionInfo.sParam1;
  Index := 0;
  for I := Low(PlayObject.m_sScriptParams) to High(PlayObject.m_sScriptParams) do
  begin
    II := Pos(WideString(','), sTemp);
    if II > 0 then
    begin
      S[Index] := Trim(Copy(sTemp, 1, II - 1));
      sTemp := Copy(sTemp, II + 1, MaxInt);
      Inc(Index);
    end
    else if Length(sTemp) > 0 then
    begin
      S[Index] := Trim(sTemp);
      sTemp := '';
      Inc(Index);
      Break;
    end;
  end;

  Result := True;
  for I := 0 to Index - 1 do
  begin
    if not (SameText(S[I], PlayObject.m_sScriptParams[I])) then
    begin
      Result := False;
      Exit;
    end;
  end;
end;

function ConditionOfCheckSelfRankNo(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  RankType: Integer;
  cMethod: Char;
  List: TGStringList;
  Index: Integer;
begin
  Result := False;
  RankType := QuestConditionInfo.nParam2;

  if (RankType < 1) or (RankType > 4) or (Length(QuestConditionInfo.sRawParam1) = 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if RankType = 1 then
    List := g_HumanRankList
  else if RankType = 2 then
    List := g_WarriorRankList
  else if RankType = 3 then
    List := g_WizardRankList
  else if RankType = 4 then
    List := g_TaoistRankList
  else
    Exit;

  Index := List.IndexOf(PlayObject.m_sCharName) + 1;
  cMethod := QuestConditionInfo.sRawParam1[1];
  case cMethod of
    '>':
      begin
        Result := Index > QuestConditionInfo.nParam3;
      end;
    '<':
      begin
        Result := Index < QuestConditionInfo.nParam3;
      end;
    '=':
      begin
        Result := Index = QuestConditionInfo.nParam3;
      end;
  else
    begin
      Result := Index >= QuestConditionInfo.nParam3;
    end;
  end;
end;

function ConditionOfCheckMapMonInfo(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  Envir: TEnvirnoment;
  MonList: TList;
  AObject: TBaseObject;
  sMonName: string;
  IsOK: Boolean;
begin
  Result := False;
  Envir := nil;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  if Envir = nil then
    Exit;

  sMonName := QuestConditionInfo.sParam2;
  if sMonName = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.sRawParam3 = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.sRawParam4 = '' then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  MonList := TList.Create;
  try
    UserEngine.GetMapMonster(Envir, MonList);
    for I := MonList.Count - 1 downto 0 do
    begin
      if MonList.Count <= 0 then
        Break;
      AObject := TBaseObject(MonList.Items[I]);
      if (not AObject.m_boGhost) and (not AObject.m_boDeath) and (AObject.Master = nil) then
      begin
        if QuestConditionInfo.nParam5 = 0 then
          IsOK := SameText(DelNumber(AObject.m_sCharName), sMonName) or SameText(AObject.m_sCharName, sMonName)
        else
          IsOK := Pos(sMonName, AObject.m_sCharName) > 0;

        if IsOK then
        begin
          if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam3, IntToStr(AObject.m_nCurrX), AObject.m_nCurrX) then
          begin
            Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
            Exit;
          end;

          if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam4, IntToStr(AObject.m_nCurrY), AObject.m_nCurrY) then
          begin
            Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
            Exit;
          end;

          Result := True;
          Break;
        end;
      end;
    end;
  finally
    MonList.Free;
  end;
end;

function ConditionOfCheckCallGamePet(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;

  if PlayObject = nil then
    Exit;

  Result := PlayObject.m_MyGamePet <> nil;
end;

function ConditionOfCheckGamePetLevel(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nLevel: Int64;
  cMethod: Char;
begin
  Result := False;

  if PlayObject = nil then
    Exit;

  if PlayObject.m_MyGamePet = nil then
    Exit;

  nLevel := StrToInt64Def(QuestConditionInfo.sParam2, 0);
  if (QuestConditionInfo.sParam1 = '') or (nLevel < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  case cMethod of
    '=':
      if PlayObject.m_MyGamePet.m_Abil.Level = nLevel then
        Result := True;
    '>':
      if PlayObject.m_MyGamePet.m_Abil.Level > nLevel then
        Result := True;
    '<':
      if PlayObject.m_MyGamePet.m_Abil.Level < nLevel then
        Result := True;
  else
    if PlayObject.m_MyGamePet.m_Abil.Level >= nLevel then
      Result := True;
  end;
end;

function ConditionOfCheckGamePetSkillMagic(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  StdItem: pTStdItem;
  I, ItemIndex: Integer;
  GamePetData: pTGamePetData;
begin
  Result := False;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  StdItem := UserEngine.GetStdItemEx(QuestConditionInfo.sParam1, ItemIndex);
  if StdItem = nil then
    Exit;

  if (PlayObject.m_MyGamePetIndex >= 0) and (PlayObject.m_MyGamePetIndex < PlayObject.m_GamePetList.Count) and (PlayObject.m_MyGamePet <> nil) then
  begin
    GamePetData := PlayObject.m_GamePetList.Items[PlayObject.m_MyGamePetIndex];

    for I := Low(GamePetData.wMagics) to High(GamePetData.wMagics) do
    begin
      if GamePetData.wMagics[I] = ItemIndex then
      begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

function ConditionOfCheckShowFashion(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  SmartObject: TSmartObject;
begin
  Result := False;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  Result := SmartObject.m_boShowFashion;
end;

function ConditionOfCheckIsSellPlayer(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
begin
  Result := g_SellPlayerList.Search(PlayObject.m_sCharName, I);
end;

function ConditionOfCheckIsSellPlayDelegator(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  Info: PSellPlayerInfo;
begin
  Result := False;
  for I := 0 to g_SellPlayerList.Count - 1 do
  begin
    Info := g_SellPlayerList.Items[I];

    if SameText(Info.Delegater, PlayObject.m_sCharName) then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function ConditionOfGetStringPosEx(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  LoadList: TStringList;
  sListFileName: string;
  sFindText, S: string;
  sVarLineNo, sVarText: string;
begin
  Result := False;

  if QuestConditionInfo.nParam5 = 0 then
    sListFileName := g_Config.sEnvirDir + Npc.m_sPath + QuestConditionInfo.sParam1
  else
    sListFileName := QuestConditionInfo.sParam1;

  sFindText := QuestConditionInfo.sParam2;

  sVarLineNo := QuestConditionInfo.sRawParam3;
  sVarText := QuestConditionInfo.sRawParam4;

  if (sVarLineNo = '') or (sVarText = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  sFindText := LowerCase(sFindText);

  if FileExists(sListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;

    for I := 0 to LoadList.Count - 1 do
    begin
      S := LowerCase(LoadList[I]);
      if Pos(sFindText, S) <> 0 then
      begin
        Result := True;

        if not Npc.SetVarValue(PlayObject, sVarLineNo, IntToStr(I), I) then
        begin
          Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
        end;

        S := LoadList[I];

        if not Npc.SetVarValue(PlayObject, sVarText, S, StrToIntDef(S, 0)) then
        begin
          Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
        end;

        Break;
      end;
    end;

    LoadList.Free;
  end
  else
  begin
    MainOutMessage('file not found => ' + sListFileName);
  end;
end;

function ConditionOfCheckGroupLeader(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (PlayObject.m_GroupOwner <> nil) and (PlayObject.m_GroupOwner = PlayObject);
end;

function ConditionOfIsSyncKillMonBurstRate(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and (TSmartObject(BaseObject).m_boSyncKillMonBurstRate);
end;

function ConditionOfCheckBagItems(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sVar, sVar2, sListFileName, sCheck: string;
  LoadList: TStringList;
  I, J, nCount: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  isFound: Boolean;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    Exit;

  sListFileName := QuestConditionInfo.sParam1;
  if not FileExists(sListFileName) then
    sListFileName := g_Config.sEnvirDir + Npc.m_sPath + QuestConditionInfo.sParam1;
  if not FileExists(sListFileName) then
  begin
    MainOutMessage('file not found => ' + sListFileName);
    Exit;
  end;

  sVar := QuestConditionInfo.sRawParam2;
  sVar2 := QuestConditionInfo.sRawParam3;

  LoadList := TStringList.Create;
  try
    try
      LoadList.LoadFromFile(sListFileName);
    except
      MainOutMessage('loading fail.... => ' + sListFileName);
    end;

    nCount := 0;
    isFound := False;
    for I := 0 to LoadList.Count - 1 do
    begin
      sCheck := LoadList[I];
      for J := BaseObject.m_ItemList.Count - 1 downto 0 do
      begin
        UserItem := pTUserItem(BaseObject.m_ItemList[J]);
        if UserItem <> nil then
        begin
          StdItem := UserEngine.GetStdItem(UserItem.wIndex);
          if (StdItem <> nil) and (StdItem.Name = sCheck) then
          begin
            isFound := True;
            Inc(nCount);
          end;
        end;
      end;

      if isFound then
        Break;
    end;
  finally
    LoadList.Free;
  end;

  Result := isFound;
  if not Npc.SetVarValue(PlayObject, sVar, sCheck, 0) then
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
  if not Npc.SetVarValue(PlayObject, sVar2, IntToStr(nCount), nCount) then
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
end;

function ConditionOfCacheCheckBagItems(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sVar, sVar2, sFileName, sCheck: string;
  LoadList: TStringList;
  I, J, Index, nCount: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  isFound: Boolean;
begin
  Result := False;
  if not (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    Exit;

  sFileName := QuestConditionInfo.sParam1;
  if not FileExists(sFileName) then
    sFileName := g_Config.sEnvirDir + Npc.m_sPath + QuestConditionInfo.sParam1;
  if not FileExists(sFileName) then
  begin
    MainOutMessage('file not found => ' + sFileName);
    Exit;
  end;

  sVar := QuestConditionInfo.sRawParam2;
  sVar2 := QuestConditionInfo.sRawParam3;

  Index := g_NpcTextFilesCache.IndexOf(sFileName);
  if Index >= 0 then
    LoadList := TStringList(g_NpcTextFilesCache.Objects[Index])
  else
  begin
    LoadList := TStringList.Create;
    g_NpcTextFilesCache.AddObject(sFileName, LoadList);
    try
      LoadList.LoadFromFile(sFileName);
    except
      MainOutMessage('loading fail.... => ' + sFileName);
    end;
  end;

  if LoadList = nil then
    Exit; // HZQ 20230410

  isFound := False;
  nCount := 0;
  for I := 0 to LoadList.Count - 1 do
  begin
    sCheck := LoadList[I];
    for J := BaseObject.m_ItemList.Count - 1 downto 0 do
    begin
      UserItem := pTUserItem(BaseObject.m_ItemList[J]);
      if UserItem <> nil then
      begin
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and (StdItem.Name = sCheck) then
        begin
          isFound := True;
          Inc(nCount);
        end;

      end;
    end;

    if isFound then
      Break;
  end;

  Result := isFound;
  if not Npc.SetVarValue(PlayObject, sVar, sCheck, 0) then
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);

  if not Npc.SetVarValue(PlayObject, sVar2, IntToStr(nCount), nCount) then
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
end;

function ConditionOfCheckGroupItem(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := QuestConditionInfo.nParam1 = PlayObject.m_wGroupItemIndex;
end;

function ConditionOfIsMobile(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := PlayObject.m_boIsMobile;
end;

function ConditionOfCheckMoney(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sMoneyName: string;
  nOldMoney, nMoney, n01: Integer;
  cMethod: Char;
begin

  Result := False; // HZQ 20230410
  if not BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    Exit;
  sMoneyName := QuestConditionInfo.sParam1;
  cMethod := QuestConditionInfo.sParam2[1];
  nMoney := QuestConditionInfo.nParam3;
  if FindCustomMoney(sMoneyName) = nil then
  begin
    MainOutMessage(Format('用户:[%s]使用不存在的自定义货币[%s]', [PlayObject.m_sCharName, sMoneyName]));
    Exit;
  end;
  n01 := PlayObject.m_MoneyList.GetIndex(UpperCase(sMoneyName));
  if n01 >= 0 then
  begin
    nOldMoney := Integer(PlayObject.m_MoneyList.Objects[n01]);
    case cMethod of
      '=':
        Result := nOldMoney = nMoney;
      '>':
        Result := nOldMoney > nMoney;
      '<':
        Result := nOldMoney < nMoney;
    else
      Result := nOldMoney >= nMoney;
    end;
    PlayObject.m_MoneyList.Objects[n01] := TObject(nOldMoney);
  end
  else
  begin
    case cMethod of
      '=':
        Result := 0 = nMoney;
      '>':
        Result := 0 > nMoney;
      '<':
        Result := 0 < nMoney;
    else
      Result := 0 >= nMoney;
    end;
    PlayObject.m_MoneyList.AddRecord(UpperCase(sMoneyName), 0);
  end;
end;

function ConditionOfCheckBindMoney(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  sMoneyName: string;
  I, nMoney, n01, nCount: Integer;
  nCheckMoney: Int64; // 防止多种货币加起来溢出  By 一支笔 at:2022-02-21 22:29:14
  cMethod: Char;
  CustomMoney: pTCustomMoney;
  MoneyArr: TDynamicCustomMoneyArray;
begin
  Result := False; // HZQ 20230410;
  if not BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    Exit;
  nCheckMoney := 0;
  sMoneyName := QuestConditionInfo.sParam1;
  cMethod := QuestConditionInfo.sParam2[1];
  nMoney := QuestConditionInfo.nParam3;
  CustomMoney := FindCustomMoney(sMoneyName);
  if CustomMoney = nil then
  begin
    MainOutMessage(Format('用户:[%s]使用不存在的自定义货币[%s]', [PlayObject.m_sCharName, sMoneyName]));
    Exit;
  end;
  nCount := GetCustomMoneyBindArr(CustomMoney, MoneyArr);
  for I := 0 to nCount - 1 do
  begin
    n01 := PlayObject.m_MoneyList.GetIndex(UpperCase(MoneyArr[I].sName));
    if n01 >= 0 then
    begin
      nCheckMoney := nCheckMoney + Integer(PlayObject.m_MoneyList.Objects[n01]);
    end;
  end;

  case cMethod of
    '=':
      Result := nCheckMoney = nMoney;
    '>':
      Result := nCheckMoney > nMoney;
    '<':
      Result := nCheckMoney < nMoney;
  else
    Result := nCheckMoney >= nMoney;
  end;
end;

function ConditionOfFindNpcPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  // I: Integer;
  nX, nY: Integer;
  Envir: TEnvirnoment;
  Merchant: TMerchant;
begin
  Result := False;
  Envir := nil;
  nX := -1;
  nY := -1;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  if Envir = nil then
    Exit;

  if QuestConditionInfo.sRawParam3 = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.sRawParam4 = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  Merchant := UserEngine.FindMerchantByName(Envir.sMapName, QuestConditionInfo.sParam2);

  if Merchant = nil then
  begin
    Exit;
  end;

  Result := True;

  if Merchant <> nil then
  begin
    nX := Merchant.m_nCurrX;
    nY := Merchant.m_nCurrY;
  end;

  if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam3, IntToStr(nX), nX) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
  end;
  if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam4, IntToStr(nY), nY) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
  end;
end;

function ConditionOfFindMonPoint(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  // I: Integer;
  nX, nY: Integer;
  Envir: TEnvirnoment;
  Monster: TBaseObject;
begin
  Result := False;
  Envir := nil;
  nX := -1;
  nY := -1;
  if (not QuestConditionInfo.boCompleteFormat1) and (QuestConditionInfo.VarInfo1.VarAttr = aFixVar) then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sRawParam1);
  if Envir = nil then
    Envir := g_MapManager.FindMap(QuestConditionInfo.sParam1);

  if Envir = nil then
    Exit;

  if QuestConditionInfo.sRawParam3 = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;

  if QuestConditionInfo.sRawParam4 = '' then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
    Exit;
  end;
  if Envir = PlayObject.m_PEnvir then
    Monster := UserEngine.FindMapMonster(Envir, QuestConditionInfo.sParam2, PlayObject.m_nCurrX, PlayObject.m_nCurrY)
  else
    Monster := UserEngine.FindMapMonster(Envir, QuestConditionInfo.sParam2);

  if Monster = nil then
  begin
    Exit;
  end;

  Result := True;

  if Monster <> nil then
  begin
    nX := Monster.m_nCurrX;
    nY := Monster.m_nCurrY;
  end;

  if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam3, IntToStr(nX), nX) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
  end;
  if not Npc.SetVarValue(PlayObject, QuestConditionInfo.sRawParam4, IntToStr(nY), nY) then
  begin
    Npc.ScriptConditionError(PlayObject, QuestConditionInfo);
  end;
end;

function ConditionOfCheckAngryValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  nAngryValue: Int64;
  cMethod: Char;
  BaseObj: TBaseObject;
  Hero: THeroObject;
  boPer: Boolean;
  nBaseValue: Integer;
begin
  Result := False;

  BaseObj := nil;
  if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    BaseObj := TPlayObject(BaseObject).m_MyHero
  else if (BaseObject.m_btRaceServer in [RC_HEROOBJECT]) then
    BaseObj := BaseObject
  else if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) then
    BaseObj := TPlayObject(BaseObject.Master).m_MyHero;

  if BaseObj = nil then
    Exit;

  Hero := THeroObject(BaseObj);

  boPer := QuestConditionInfo.nParam3 = 1;
  nBaseValue := 100;

  nAngryValue := StrToInt64Def(QuestConditionInfo.sParam2, 0);

  if (QuestConditionInfo.sParam1 = '') or (nAngryValue < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  cMethod := QuestConditionInfo.sParam1[1];
  if boPer then
  begin
    case cMethod of
      '=':
        Result := Hero.m_btAngryValue = Round(g_Config.btMaxAngryValue / nBaseValue * nAngryValue);
      '>':
        Result := Hero.m_btAngryValue > Round(g_Config.btMaxAngryValue / nBaseValue * nAngryValue);
      '<':
        Result := Hero.m_btAngryValue < Round(g_Config.btMaxAngryValue / nBaseValue * nAngryValue);
    else
      Result := Hero.m_btAngryValue >= Round(g_Config.btMaxAngryValue / nBaseValue * nAngryValue);
    end;
  end
  else
  begin
    case cMethod of
      '=':
        if Hero.m_btAngryValue = nAngryValue then
          Result := True;
      '>':
        if Hero.m_btAngryValue > nAngryValue then
          Result := True;
      '<':
        if Hero.m_btAngryValue < nAngryValue then
          Result := True;
    else
      if Hero.m_btAngryValue >= nAngryValue then
        Result := True;
    end;
  end;
end;

function ConditionOfCheckStopM2MakeMon(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := g_Config.boStopM2MakeMon;
end;

function ConditionOfCheckSelfStatus(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;

  if not QuestConditionInfo.nParam1 in [1..8] then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  case QuestConditionInfo.nParam1 of
    1:
      Result := BaseObject.m_wStatusTimeArr[POISON_STONE] <> 0;
    2:
      Result := BaseObject.m_wStatusTimeArr[STATE_FROZEN] <> 0;
    3:
      Result := BaseObject.m_boCobwebWindingStatus;
    4:
      Result := BaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] <> 0;
    5:
      Result := BaseObject.m_wStatusTimeArr[POISON_DECHEALTH] <> 0;
    6:
      Result := BaseObject.m_boDingShen;
    7:
      Result := BaseObject.m_boTanHuan;
    8:
      Result := BaseObject.m_boImprison;
  end;
end;

// CheckShieldStateOpen 参数 1(0:新武力盾;1:武力盾/魔法盾/道力盾;2:新道力盾)
function ConditionOfCheckShieldStateOpen(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
begin
  Result := False;

  if not QuestConditionInfo.nParam1 in [0..2] then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  case QuestConditionInfo.nParam1 of
    0:
      Result := BaseObject.m_boAbilNewHitBubbleDefence;
    1:
      Result := BaseObject.m_wStatusTimeArr[STATE_BUBBLEDEFENCEUP] <> 0;
    2:
      Result := BaseObject.m_boAbilNewMagBubbleDefence;
  end;
end;

function ConditionOfCheckFullBead(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I, tmpNum: Integer;
  tmpUserItem: pTUserItem;
  tmpStdItem: pTStdItem;
begin
  Result := False;

  if (QuestConditionInfo.sParam1.IsEmpty) or (QuestConditionInfo.sParam2.IsEmpty) or (QuestConditionInfo.nParam3 < 0) then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  tmpNum := 0;
  for I := 0 to BaseObject.m_ItemList.Count - 1 do
  begin
    tmpUserItem := BaseObject.m_ItemList.Items[I];
    tmpStdItem := UserEngine.GetStdItem(tmpUserItem.wIndex);

    if (tmpStdItem <> nil) //
      and (tmpStdItem.StdMode = 49) //
      and SameText(tmpStdItem.Name, QuestConditionInfo.sParam1) //
      and (tmpUserItem.Dura = tmpUserItem.DuraMax) then
      Inc(tmpNum);
  end;

  case Char(QuestConditionInfo.sParam2[1]) of
    '=':
      Result := tmpNum = QuestConditionInfo.nParam3;
    '>':
      Result := tmpNum > QuestConditionInfo.nParam3;
    '<':
      Result := tmpNum < QuestConditionInfo.nParam3;
  end;
end;

function ConditionOfCheckVarInList(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  sValue: string;
  IsBreakParseVar: Boolean;
  aryLValue: TArray<string>;
begin
  Result := False;
  sValue := QuestConditionInfo.sParam2;
  if (QuestConditionInfo.sParam1 = '') or (sValue = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  aryLValue := QuestConditionInfo.sParam1.Split([',']);
  for I := High(aryLValue) downto Low(aryLValue) do
  begin
    if aryLValue[I] = sValue then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function ConditionOfCheckListAllDigit(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  IsBreakParseVar: Boolean;
  aryLValue: TArray<string>;
  TempInt: Integer;
begin
  Result := True;
  if (QuestConditionInfo.sParam1 = '') then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;

  aryLValue := QuestConditionInfo.sParam1.Split([',']);
  for I := High(aryLValue) downto Low(aryLValue) do
  begin
    if not TryStrToInt(aryLValue[I], TempInt) then
    begin
      Result := False;
      Break;
    end;
  end;
end;

function ConditionOfCheckStateValue(Npc: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): Boolean;
var
  I: Integer;
  IsBreakParseVar: Boolean;
  aryLValue: TArray<string>;
  TempInt: Integer;
begin
  Result := False;
  if not QuestConditionInfo.nParam1 in [0..13, 16, 17] then
  begin
    Npc.ScriptConditionError(BaseObject, QuestConditionInfo);
    Exit;
  end;
  if PlayObject.m_wStatusArrValue[QuestConditionInfo.nParam1] <> 0 then
    Result := True;

end;

initialization
  FillChar(ConditionCmdArray, SizeOf(TConditionCmdArray), 0);
  ConditionCmdArray[nNC_CHECK] := ConditionOfCheck;
  ConditionCmdArray[nNC_RANDOM] := ConditionOfRandom;
  ConditionCmdArray[nNC_RANDOMEX] := ConditionOfRandomEx;
  ConditionCmdArray[nNC_GENDER] := ConditionOfGender;
  ConditionCmdArray[nNC_DAYTIME] := ConditionOfDatTime;
  ConditionCmdArray[nNC_CHECKLEVEL] := ConditionOfCheckLevel;
  ConditionCmdArray[nNC_CHECKJOB] := ConditionOfCheckJob;
  ConditionCmdArray[nNC_CHECKBBCOUNT] := ConditionOfCheckBBCount;
  ConditionCmdArray[nNC_CHECKITEM] := ConditionOfCheckItem;
  ConditionCmdArray[nNC_CHECKITEMW] := ConditionOfCheckItemW;
  ConditionCmdArray[nNC_CHECKGOLD] := ConditionOfCheckGold;
  ConditionCmdArray[nNC_ISTAKEITEM] := ConditionOfIsTakeItem;
  ConditionCmdArray[nNC_CHECKDURA] := ConditionOfCheckDura;
  ConditionCmdArray[nNC_CHECKDURAEVA] := ConditionOfCheckDuraEva;
  ConditionCmdArray[nNC_DAYOFWEEK] := ConditionOfDayOfWeek;
  ConditionCmdArray[nNC_HOUR] := ConditionOfHour;
  ConditionCmdArray[nNC_MIN] := ConditionOfMin;
  ConditionCmdArray[nNC_CHECKPKPOINT] := ConditionOfCheckPKPoint;
  ConditionCmdArray[nNC_CHECKMONMAP] := ConditionOfCheckMonMapCount;
  ConditionCmdArray[nNC_CHECKHUM] := ConditionOfCheckMapHuman;
  ConditionCmdArray[nNC_CHECKBAGGAGE] := ConditionOfCheckBagGage;
  ConditionCmdArray[nNC_EQUAL] := ConditionOfEqual;
  ConditionCmdArray[nNC_LARGE] := ConditionOfLarge;
  ConditionCmdArray[nNC_SMALL] := ConditionOfSmall;
  ConditionCmdArray[nNC_CHECKNAMELIST] := ConditionOfCheckNameList;
  ConditionCmdArray[nNC_ISGUILDMASTER] := ConditionOfCheckIsGuildMaster;
  ConditionCmdArray[nNC_ISCASTLEGUILD] := ConditionOfCheckIsCastleaGuild;
  ConditionCmdArray[nNC_ISATTACKGUILD] := ConditionOfCheckIsAttackGuild;
  ConditionCmdArray[nNC_ISDEFENSEGUILD] := ConditionOfCheckIsDefenseGuild;
  ConditionCmdArray[nNC_HASGUILD] := ConditionOfCheckHaveGuild;
  ConditionCmdArray[nNC_CHECKCASTLEDOOR] := ConditionOfCheckCastleDoorStatus;
  ConditionCmdArray[nNC_CHECKPOS] := nil;
  ConditionCmdArray[nNC_ISATTACKALLYGUILD] := ConditionOfCheckIsAttackAllyGuild;
  ConditionCmdArray[nNC_ISDEFENSEALLYGUILD] := ConditionOfCheckIsDefenseAllyGuild;
  ConditionCmdArray[nNC_ISSYSOP] := ConditionOfIsSysop;
  ConditionCmdArray[nNC_ISADMIN] := ConditionOfIsAdmin;
  ConditionCmdArray[nNC_CHECKGROUPCOUNT] := ConditionOfCheckGroupCount;
  ConditionCmdArray[nNC_CHECKACCOUNTLIST] := ConditionOfCheckAccountList;
  ConditionCmdArray[nNC_CHECKIPLIST] := ConditionOfCheckIPList;
  ConditionCmdArray[nNC_CHECKCREDITPOINT] := ConditionOfCheckCreditPoint;
  ConditionCmdArray[nNC_CHECKPOSEDIR] := ConditionOfCheckPoseDir;
  ConditionCmdArray[nNC_CHECKPOSELEVEL] := ConditionOfCheckPoseLevel;
  ConditionCmdArray[nNC_CHECKPOSEGENDER] := ConditionOfCheckPoseGender;
  ConditionCmdArray[nNC_CHECKLEVELEX] := ConditionOfCheckLevelEx;
  ConditionCmdArray[nNC_CHECKBONUSPOINT] := ConditionOfCheckBonusPoint;
  ConditionCmdArray[nNC_CHECKMARRY] := ConditionOfCheckMarry;
  ConditionCmdArray[nNC_CHECKPOSEMARRY] := ConditionOfCheckPoseMarry;
  ConditionCmdArray[nNC_CHECKMARRYCOUNT] := ConditionOfCheckMarryCount;
  ConditionCmdArray[nNC_CHECKMASTER] := ConditionOfCheckMaster;
  ConditionCmdArray[nNC_HAVEMASTER] := ConditionOfHaveMaster;
  ConditionCmdArray[nNC_CHECKPOSEMASTER] := ConditionOfCheckPoseMaster;
// ConditionCmdArray[nNC_POSEHAVEMASTER] := ConditionOfPoseHaveMaster;
// ConditionCmdArray[nNC_CHECKPOSEISMASTER] := ConditionOfCheckPoseIsMaster;
  ConditionCmdArray[nNC_CHECKNAMEIPLIST] := ConditionOfCheckNameIPList;
  ConditionCmdArray[nNC_CHECKACCOUNTIPLIST] := ConditionOfCheckAccountIPList;
  ConditionCmdArray[nNC_CHECKSLAVECOUNT] := ConditionOfCheckSlaveCount;
  ConditionCmdArray[nNC_CHECKCASTLEMASTER] := ConditionOfCheckIsCastleMaster;
  ConditionCmdArray[nNC_ISNEWHUMAN] := ConditionOfIsNewHuman;
  ConditionCmdArray[nNC_CHECKMEMBERTYPE] := ConditionOfCheckMemberType;
  ConditionCmdArray[nNC_CHECKMEMBERLEVEL] := ConditionOfCheckMemBerLevel;
  ConditionCmdArray[nNC_CHECKGAMEGOLD] := ConditionOfCheckGameGold;
  ConditionCmdArray[nNC_CHECKGAMEPOINT] := ConditionOfCheckGamePoint;
  ConditionCmdArray[nNC_CHECKNAMELISTPOSITION] := ConditionOfCheckNameListPostion;
  ConditionCmdArray[nNC_CHECKGUILDLIST] := ConditionOfCheckGuildList;
  ConditionCmdArray[nNC_CHECKRENEWLEVEL] := ConditionOfCheckReNewLevel;
  ConditionCmdArray[nNC_CHECKSLAVELEVEL] := ConditionOfCheckSlaveLevel;
  ConditionCmdArray[nNC_CHECKSLAVENAME] := ConditionOfCheckSlaveName;
  ConditionCmdArray[nNC_CHECKOFGUILD] := ConditionOfCheckOfGuild;
  ConditionCmdArray[nNC_CHECKPAYMENT] := ConditionOfCheckPayMent;
  ConditionCmdArray[nNC_CHECKUSEITEM] := ConditionOfCheckUseItem;
  ConditionCmdArray[nNC_CHECKBAGSIZE] := ConditionOfCheckBagSize;
  ConditionCmdArray[nNC_CHECKLISTCOUNT] := ConditionOfCheckListCount;
  ConditionCmdArray[nNC_CHECKDC] := ConditionOfCheckDC;
  ConditionCmdArray[nNC_CHECKMC] := ConditionOfCheckMC;
  ConditionCmdArray[nNC_CHECKSC] := ConditionOfCheckSC;
  ConditionCmdArray[nNC_CHECKHP] := ConditionOfCheckHP;
  ConditionCmdArray[nNC_CHECKMP] := ConditionOfCheckMP;
  ConditionCmdArray[nNC_CHECKITEMTYPE] := ConditionOfCheckItemType;
  ConditionCmdArray[nNC_CHECKEXP] := ConditionOfCheckExp;
  ConditionCmdArray[nNC_CHECKCASTLEGOLD] := ConditionOfCheckCastleGold;
  ConditionCmdArray[nNC_PASSWORDERRORCOUNT] := ConditionOfCheckPasswordErrorCount;
  ConditionCmdArray[nNC_ISLOCKPASSWORD] := ConditionOfIsLockPassword;
  ConditionCmdArray[nNC_ISLOCKSTORAGE] := ConditionOfIsLockStorage;
  ConditionCmdArray[nNC_CHECKBUILDPOINT] := ConditionOfCheckGuildBuildPoint;
  ConditionCmdArray[nNC_CHECKAURAEPOINT] := ConditionOfCheckGuildAuraePoint;
  ConditionCmdArray[nNC_CHECKSTABILITYPOINT] := ConditionOfCheckStabilityPoint;
  ConditionCmdArray[nNC_CHECKFLOURISHPOINT] := ConditionOfCheckFlourishPoint;
  ConditionCmdArray[nNC_CHECKCONTRIBUTION] := ConditionOfCheckContribution;
  ConditionCmdArray[nNC_CHECKRANGEMONCOUNT] := ConditionOfCheckRangeMonCount;
  ConditionCmdArray[nNC_CHECKITEMADDVALUE] := ConditionOfCheckItemAddValue;
  ConditionCmdArray[nNC_CHECKINMAPRANGE] := ConditionOfCheckInMapRange;
  ConditionCmdArray[nNC_CASTLECHANGEDAY] := ConditionOfCheckCastleChangeDay;
  ConditionCmdArray[nNC_CASTLEWARDAY] := ConditionOfCheckCastleWarDay;
  ConditionCmdArray[nNC_ONLINELONGMIN] := ConditionOfCheckOnlineLongMin;
  ConditionCmdArray[nNC_CHECKGUILDCHIEFITEMCOUNT] := ConditionOfCheckChiefItemCount;
  ConditionCmdArray[nNC_CHECKNAMEDATELIST] := ConditionOfCheckNameDateList;
  ConditionCmdArray[nNC_CHECKMAPHUMANCOUNT] := ConditionOfCheckMapHumanCount;
  ConditionCmdArray[nNC_CHECKMAPMONCOUNT] := ConditionOfCheckMapMonCount;
  ConditionCmdArray[nNC_CHECKVAR] := ConditionOfCheckVar;
  ConditionCmdArray[nNC_CHECKSERVERNAME] := ConditionOfCheckServerName;
  ConditionCmdArray[nNC_CHECKMAPNAME] := ConditionOfCheckMapName;
  ConditionCmdArray[nNC_INSAFEZONE] := ConditionOfCheckSafeZone;
  ConditionCmdArray[nNC_CHECKSKILL] := ConditionOfCheckSkill;
  ConditionCmdArray[nNC_CHECKUSERDATE] := ConditionOfCheckNameDateList;
  ConditionCmdArray[nNC_CHECKCONTAINSTEXT] := ConditionOfAnsiContainsText;
  ConditionCmdArray[nNC_COMPARETEXT] := ConditionOfCompareText;
  ConditionCmdArray[nNC_CHECKTEXTLIST] := ConditionOfCheckTextList;
  ConditionCmdArray[nNC_CHECKCACHETEXTLIST] := ConditionOfCheckCacheTextList;
  ConditionCmdArray[nNC_ISGROUPMASTER] := ConditionOfIsGroupMaster;
  ConditionCmdArray[nNC_CHECKCONTAINSTEXTLIST] := ConditionOfCheckAnsiContainsTextList;
  ConditionCmdArray[nNC_CheckCacheContainsTextList] := ConditionOfCheckCacheContainsTextList;
  ConditionCmdArray[nNC_CHECKONLINE] := ConditionOfCheckOnline;
  ConditionCmdArray[nNC_CHECKTEXTLENGTH] := ConditionOfCheckStringLength;
  ConditionCmdArray[nNC_ISDUPMODE] := ConditionOfCheckIsDupMode;
  ConditionCmdArray[nNC_CHECKGAMEDIAMOND] := ConditionOfCheckGameDiamond;
  ConditionCmdArray[nNC_CHECKGAMEGIRD] := ConditionOfCheckGameGird;
  ConditionCmdArray[nNC_CHECKGAMEGLORY] := ConditionOfCheckGameGlory;
  ConditionCmdArray[nNC_CHECKSTRINGLENGTH] := ConditionOfCheckStringLength;
  ConditionCmdArray[nNC_HAVHERO] := ConditionOfHavHero;
  ConditionCmdArray[nNC_CHECKONLINEPLAYCOUNT] := ConditionOfCheckOnlinePlayCount;
  ConditionCmdArray[nNC_CHECKRANGEMONCOUNTEX] := ConditionOfCheckRangeMonCountEx;
  ConditionCmdArray[nNC_CHECKMAPMOVE] := ConditionOfCheckMapMove;
  ConditionCmdArray[nNC_CHECKNAMEDATETIMELIST] := ConditionOfCheckNameDateTimeList;
  ConditionCmdArray[nNC_KILLERRACE] := ConditionOfCheckKillerRace;
  ConditionCmdArray[nNC_CHECKCASTLEWARAREA] := ConditionOfCheckCastleWarArea;
  ConditionCmdArray[nNC_CHECKUNDERWAR] := ConditionOfCheckUnderWar;
  ConditionCmdArray[nNC_CHECKCURRRTARGETRACE] := ConditionOfCheckCurrTargetRace;
  ConditionCmdArray[nNC_CHECKMAPSAMEMONCOUNT] := ConditionOfCheckMapSameMonCount;
  ConditionCmdArray[nNC_CHECKMYSHOP] := ConditionOfCheckMyShop;
  ConditionCmdArray[nNC_CHECKSHOPNAME] := ConditionOfCheckShopName;
  ConditionCmdArray[nNC_CHECKSLAVEINRANGE] := ConditionOfCheckSlaveInRange;
  ConditionCmdArray[nNC_CHECKRANGEHUMCOUNT] := ConditionOfCheckRangeHumanCount;
  ConditionCmdArray[nNC_CHECKHUMINRANGE] := ConditionOfCheckHumanInRange;
  ConditionCmdArray[nNC_CHECKGUILDMEMBERMAXLIMITCOUNT] := ConditionOfCheckGuildMemberMaxLimitCount;
  ConditionCmdArray[nNC_CHECKNEWITEMVALUE] := ConditionOfCheckNewItemValue;
  ConditionCmdArray[nNC_CHECKSHOPSTALLSTATUS] := ConditionOfCheckShopStall;
  ConditionCmdArray[nNC_CHECKHEROLOYAL] := ConditionOfCheckHeroLoyal;
  ConditionCmdArray[nNC_ISDUMMY] := ConditionOfIsDummy;
  ConditionCmdArray[nNC_CHECKDUMMYCOUNT] := ConditionOfCheckDummyCount;
  ConditionCmdArray[nNC_MAPHUMISSAMEGUILD] := ConditionOfMapHumIsSameGuild;
  ConditionCmdArray[nNC_CHECKKILLMONNAME] := ConditionOfCheckKillMonName;
  ConditionCmdArray[nNC_CHECKHITMONNAME] := ConditionOfCheckHitMonName;
  ConditionCmdArray[nNC_CHECKOFFLINE] := ConditionOfCheckOffLine;
  ConditionCmdArray[nNC_CHECKITEMNAMECOLOR] := ConditionOfCheckItemNameColor;
  ConditionCmdArray[nNC_KILLBYHUM] := ConditionOfKillByHum;
  ConditionCmdArray[nNC_CHECKRANDOMNO] := ConditionOfCheckRandomNo;
  ConditionCmdArray[nNC_CHECKFOUNDRYITEM] := ConditionOfCheckFoundryItem;
  ConditionCmdArray[nNC_CHECKGUILDMEMBERCOUNT] := ConditionOfCheckGuildMemberCount;
  ConditionCmdArray[nNC_CHECKUPGRADEITEMNAME] := ConditionOfCheckUpgradeItemName;
  ConditionCmdArray[nNC_CHECKLUCKPOINT] := ConditionOfCheckLuckPoint;
  ConditionCmdArray[nNC_CHECKMINE] := ConditionOfCheckMine;
  ConditionCmdArray[nNC_CHECKHEROCOUNT] := ConditionOfCheckHeroCount;
  ConditionCmdArray[nNC_CHECKMAGICNAME] := ConditionOfCheckMagicName;
  ConditionCmdArray[nNC_CHECKKILLMOBNAME] := ConditionOfCheckKillMob;
  ConditionCmdArray[nNC_CHECKKILLSLAVENAME] := ConditionOfCheckKillSlaveName;
  ConditionCmdArray[nNC_CHECKGUILDMEMBER] := ConditionOfCheckGuildMember;
// 修改REPAIRALL脚本报错 piaoyun 2013-08-24
// ConditionCmdArray[nNC_REPAIRALL] := ConditionOfRepairAll;
  ConditionCmdArray[nNC_CHECKNATIONCREDIT] := ConditionOfCheckNationCredit;
  ConditionCmdArray[nNC_CHECKGAMEGOLDEX] := ConditionOfCheckGameGoldEx;
  ConditionCmdArray[nNC_CHECKPULSELEVEL] := ConditionOfCheckPulseLevel;
  ConditionCmdArray[nNC_CHECKHUMANPULSE] := ConditionOfCheckHumanPulse;
  ConditionCmdArray[nNC_CHECKOPENPULSELEVEL] := ConditionOfCheckOpenPulseLevel;
  ConditionCmdArray[nNC_CHECKHEROAUTOPRACTICE] := ConditionOfCheckHeroAutoPractice;
  ConditionCmdArray[nNC_CHECKDEPUTYHERO] := ConditionOfCheckDeputyHero;
  ConditionCmdArray[nNC_CHECKHEROINSTORAGE] := ConditionOfCheckHeroInStorage;
  ConditionCmdArray[nNC_CHECKREADSKILLNG] := ConditionOfCheckReadSkillNG;
  ConditionCmdArray[nNC_CHECKNGLEVEL] := ConditionOfCheckNGLevel;
  ConditionCmdArray[nNC_CHECKOPENLASTSKILL] := ConditionOfCheckOpenLastSkill;
  ConditionCmdArray[nNC_CHECKHEROJOB] := ConditionOfCheckHeroJob;
  ConditionCmdArray[nNC_CHECKHEROONLINE] := ConditionOfCheckHeroOnline;
  ConditionCmdArray[nNC_CHECKINWARAREA] := ConditionOfCheckCastleWarArea;
// ConditionCmdArray[nNC_CHECKITEMBIND] := ConditionOfCheckItemBind;
  ConditionCmdArray[nNC_CHECKITEMSTATE] := ConditionOfCheckItemState;
  ConditionCmdArray[nNC_CHECKPKPOINTEX] := ConditionOfCheckPKPointEx;
  ConditionCmdArray[nNC_ISNEWSERVER] := ConditionOfIsNewServer;
  ConditionCmdArray[nNC_CHECKSUCKDAMAGE] := ConditionOfCheckSuckDamage;
  ConditionCmdArray[nNC_CHECKMAPDUMMYCOUNT] := ConditionOfCheckMapDummyCount;
  ConditionCmdArray[nNC_CHECKGUILDMASTER] := ConditionOfCheckGuildMaster;
  ConditionCmdArray[nNC_CHECKRECALL] := ConditionOfCheckRecall;
  ConditionCmdArray[nNC_CHECKTAKEOFFITEM] := ConditionOfCheckCurrentItem;
  ConditionCmdArray[nNC_CHECKTAKEONITEM] := ConditionOfCheckCurrentItem;
  ConditionCmdArray[nNC_CheckCurrentItem] := ConditionOfCheckCurrentItem;
  ConditionCmdArray[nNC_ISHIGH] := ConditionOfIsHigh;
  ConditionCmdArray[nNC_CHECKHUMBAG] := ConditionOfCheckHumBag;
  ConditionCmdArray[nNC_CHECKHAVEHERO] := ConditionOfCheckHaveHero;
  ConditionCmdArray[nNC_CHECKHEROLEVEL] := ConditionOfCheckHeroLevel;
  ConditionCmdArray[nNC_CHECKHEROPKPOINT] := ConditionOfCheckHeroPKPoint;
  ConditionCmdArray[nNC_CHECKHEROSUCKDAMAGE] := ConditionOfCheckHeroSuckDamage;
  ConditionCmdArray[nNC_CHECKITEMUPGRADECOUNT] := ConditionOfCheckItemUpgradeCount;
  ConditionCmdArray[nNC_CHECKATTACKMODE] := ConditionOfCheckAttackMode;
  ConditionCmdArray[nNC_CHECKITEMDURA] := ConditionOfCheckItemDura;
  ConditionCmdArray[nNC_CHECKNUMOFKICK] := ConditionOfCheckNumOfKick;
  ConditionCmdArray[nNC_MONTHOFYEAR] := ConditionOfMonthOfYear;
  ConditionCmdArray[nNC_DAYOFMONTH] := ConditionOfDayOfMonth;
  ConditionCmdArray[nNC_CHECKNation] := ConditionOfCheckNation;
  ConditionCmdArray[nNC_CHECKNATIONHUMCOUNT] := ConditionOfCheckNationHumCount;
  ConditionCmdArray[nNC_CHECKITEMADDVALUEEX] := ConditionOfCheckItemAddValueEx;
  ConditionCmdArray[nNC_CHECKGROUPMEMBERCOUNT] := ConditionOfCheckGroupMemberCount;
// 检测技能点脚本命令 piaoyun 2013-07-27
  ConditionCmdArray[nNC_CHECKTRANPOINT] := ConditionOfCheckTranPoint;
// 连击、经络相关 piaoyun 2013-08-16
  ConditionCmdArray[nNC_CHECKKIMNEEDLE] := ConditionOfCheckKimneedle;
// 检测人物是否已经创建指定名称的副本 chongchong 2013-09-06
  ConditionCmdArray[nNC_CheckCanMoveEctype] := ConditionOfCanMoveEctype;
// 检测地图标识状态 chongchong 2013-09-06
  ConditionCmdArray[nNC_CHECKMAPQUEST] := ConditionOfCheckMapQuest;
// 检测人物多长时间没有移动 chongchong 2013-10-28
  ConditionCmdArray[nNC_CHECKSTATIONTIME] := ConditionOfCheckStationTime;
// 检测自身血量百分比 chongchong 2013-10-29
  ConditionCmdArray[nNC_CHECKHPPER] := ConditionOfCheckHpper;
// 检测自身MP百分比 chongchong 2013-10-29
  ConditionCmdArray[nNC_CHECKMPPER] := ConditionOfCheckMpper;
// 检查夫妻另一半是否在线 chongchong 2013-12-17
  ConditionCmdArray[nNC_CHECKDEARONLINE] := ConditionOfCheckDearOnLine;
// 检查夫妻另一半是否在某个地图 chongchong 2013-12-17
  ConditionCmdArray[nNC_CHECKDEARONMAP] := ConditionOfCheckDearOnMap;
// 检查特修需要的金币 chongchong 2013-12-26
  ConditionCmdArray[nNC_CHECKREPAIRALLGOLD] := ConditionOfCheckRepairAllGold;
// 检测神佑袋某个是否开启 chongchong 2014-04-17
  ConditionCmdArray[nNC_CHECKOPENGODBLESS] := ConditionOfCheckOpenGodBless;
// 检测神佑袋是否显示 chongchong 2014-04-20
  ConditionCmdArray[nNC_CHECKSHOWGODBLESS] := ConditionOfCheckShowGodBless;
// 检测客户端宽度 chongchong 2014-05-17
  ConditionCmdArray[nNC_CHECKCLIENTWIDTH] := ConditionOfCheckClientWidth;
// 检测客户端宽度 chongchong 2014-05-17
  ConditionCmdArray[nNC_CHECKCLIENTHEIGHT] := ConditionOfCheckClientHeight;
// 检查玩家是否有指定称号 chongchong 2014-05-23
  ConditionCmdArray[nNC_CHECKFENGHAO] := ConditionOfCheckFengHao;
// 检查玩家所有称号的数量 chongchong 2014-05-23
  ConditionCmdArray[nNC_CHECKFENGHAOCOUNT] := ConditionOfCheckFengHaoCount;
// 检测封号元素属性 chongchong 2014-05-28
  ConditionCmdArray[nNC_CHECKNEWFENGHAOVALUE] := ConditionOfCheckNewFengHaoValue;
// 检测人物是否在骑马 chongchong 2014-08-12
  ConditionCmdArray[nNC_CHECKONHORSE] := ConditionOfCheckOnHorse;
// 检测是否被人物杀死 2014-08-12
  ConditionCmdArray[nNC_CHECKKILLBYHUM] := ConditionOfCheckKillByHum;
// 检查人物的师傅或者徒弟是否在线上 2014-10-17
  ConditionCmdArray[nNC_CheckMasterOnline] := ConditionOfCheckMasterOnline;
// 检查人物的师傅或者徒弟是否在指定地图 2014-10-17
  ConditionCmdArray[nNC_CheckMasterOnMap] := ConditionOfCheckMasterOnMap;
// 检查人物是不是是师傅 chongchong 2014-10-17
  ConditionCmdArray[nNC_CHECKISMASTER] := ConditionOfCheckIsMaster;
// 检查人物是不是是徒弟 chongchong 2014-10-17
  ConditionCmdArray[nNC_CHECKISPrentice] := ConditionOfCheckIsPrentice;
// 检测对面的有没有师傅 2014-10-17
  ConditionCmdArray[nNC_PoseHaveMaster] := ConditionOfPoseHaveMaster;
// 检测对面的有没有徒弟 2014-10-17
  ConditionCmdArray[nNC_CheckPoseHavePrentice] := ConditionOfPoseHavePrentice;
// 检测对面是否为自己的徒弟 2014-10-17
  ConditionCmdArray[nNC_CheckPoseIsPrentice] := ConditionOfCheckPoseIsPrentice;
// 检测对面是否为自己的师傅 2014-10-17
  ConditionCmdArray[nNC_CheckPoseIsMaster] := ConditionOfCheckPoseIsMaster;
// 检查装备凹槽数量 chongchong 2015-01-08
  ConditionCmdArray[nNC_CHECKFLUTECOUNT] := ConditionOfCheckFluteCount;
// 检查装备镶嵌宝石数量 chongchong 2015-02-17
  ConditionCmdArray[nNC_CHECKITEMSTONECOUNT] := ConditionOfCheckItemStoneCount;
// 检查全身镶嵌指定的宝石数量
  ConditionCmdArray[nNC_CheckStoneCount] := ConditionOfCheckStoneCount;
// 检查NPC是否改变了外观
  ConditionCmdArray[nNC_CHECKNPCSETIMAGE] := ConditionOfCheckNpcSetImage;
// 检测星星数量 chongchong 2015-02-16
  ConditionCmdArray[nNC_CHECKUPGRADECOUNT] := ConditionOfCheckUpgradeCount;
  ConditionCmdArray[nNC_CHECKITEMS] := ConditionOfCheckItems;
  ConditionCmdArray[nNC_CHECKITEMWLOOKS] := ConditionOfCheckItemwLooks;
// 检测OK框中物品数量 chongchong 2015-02-16
  ConditionCmdArray[nNC_CHECKBOXITEMCOUNT] := ConditionOfCheckBoxItemCount;
// 检测包裹中指定物品数量(不算OK框中放的物品) chongchong 2015-11-13
  ConditionCmdArray[nNC_CHECKBAGITEMCOUNTEX] := ConditionOfCheckBagItemCountEx;
// 检测装备是否镶嵌某宝石 chongchong 2015-10-07
  ConditionCmdArray[nNC_CHECKITEMHASSTONE] := ConditionOfCheckItemHasStone;
  ConditionCmdArray[nNC_CHECKActiveFengHao] := ConditionOfCheckActiveFengHao;
// 检测自定义装备进度条是否开启
  ConditionCmdArray[nNC_CHECKCUSTOMITEMPROGRESSBAR] := ConditionOfCheckCustomItemProgressbar;
// 检测自定义装备进度条值
  ConditionCmdArray[nNC_CHECKCUSTOMITEMPROGRESSBARVALUE] := ConditionOfCheckCustomItemProgressbarValue;
// 检查自定义装备属值
  ConditionCmdArray[nNC_CHECKCUSTOMITEMVALUE] := ConditionOfCheckCustomItemValue;
  ConditionCmdArray[nNC_CheckCustomItemBindType] := ConditionOfCheckCustomItemBindType;
  ConditionCmdArray[nNC_CHECKMAPDUMMYCOUNT] := ConditionOfCheckMapDummyCount;
// 检测孔位置是否有宝石
  ConditionCmdArray[nNC_CheckItemFluteIndexHasStone] := ConditionOfCheckItemFluteIndexHasStone;
  ConditionCmdArray[nNC_IsNationKing] := ConditionOfIsNationKing;
  ConditionCmdArray[nNC_CheckNationNameExists] := ConditionOfCheckNationNameExists;
  ConditionCmdArray[nNC_CanVerifyCode] := ConditionOfCanVerifyCode;
  ConditionCmdArray[nNC_CheckVerifyCode] := ConditionOfCheckVerifyCode;
  ConditionCmdArray[nNC_CheckMirrorMap] := ConditionOfCheckMirrorMap;
  ConditionCmdArray[nNC_CheckMobileNumber] := ConditionOfCheckMobileNumber;
  ConditionCmdArray[nNC_CheckMobileBind] := ConditionOfCheckMobileBind;
  ConditionCmdArray[nNC_CheckStorageOpen] := ConditionOfCheckStorageOpen;
  ConditionCmdArray[nNC_CheckScriptParam] := ConditionOfCheckScriptParam;
  ConditionCmdArray[nNC_CheckSelfRankNo] := ConditionOfCheckSelfRankNo;
  ConditionCmdArray[nNC_CheckMapMonInfo] := ConditionOfCheckMapMonInfo;
  ConditionCmdArray[nNC_CheckCallGamePet] := ConditionOfCheckCallGamePet;
  ConditionCmdArray[nNC_CheckGamePetLevel] := ConditionOfCheckGamePetLevel;
  ConditionCmdArray[nNC_CheckGamePetSkillMagic] := ConditionOfCheckGamePetSkillMagic;
  ConditionCmdArray[nNC_CheckShowFashion] := ConditionOfCheckShowFashion;
  ConditionCmdArray[nNC_CheckIsSellPlayer] := ConditionOfCheckIsSellPlayer;
  ConditionCmdArray[nNC_CheckIsSellPlayDelegator] := ConditionOfCheckIsSellPlayDelegator;
  ConditionCmdArray[nNC_GetStringPosEx] := ConditionOfGetStringPosEx;
  ConditionCmdArray[nNC_CheckGroupLeader] := ConditionOfCheckGroupLeader;
  ConditionCmdArray[nNC_CheckStopM2MakeMon] := ConditionOfCheckStopM2MakeMon;
  ConditionCmdArray[nNC_CheckAngryValue] := ConditionOfCheckAngryValue;
  ConditionCmdArray[nNC_ISSYNCKILLMONBURSTRATE] := ConditionOfIsSyncKillMonBurstRate;
  ConditionCmdArray[nNC_CHECKBAGITEMS] := ConditionOfCheckBagItems;
  ConditionCmdArray[nNC_CACHECHECKBAGITEMS] := ConditionOfCacheCheckBagItems;
  ConditionCmdArray[nNC_FindMonPoint] := ConditionOfFindMonPoint;
  ConditionCmdArray[nNC_FindNpcPoint] := ConditionOfFindNpcPoint;
  ConditionCmdArray[nNC_CHECKGROUPITEM] := ConditionOfCheckGroupItem;
  ConditionCmdArray[nNC_ISMOBILE] := ConditionOfIsMobile;
  ConditionCmdArray[nNC_CHECKMONEY] := ConditionOfCheckMoney;
  ConditionCmdArray[nNC_CHECKBINDMONEY] := ConditionOfCheckBindMoney;
  ConditionCmdArray[nNC_CHECKCURRTARGETSLAVE] := ConditionOfCheckCurrTargetSlave;
  ConditionCmdArray[nNC_CHECKSELFSTATUS] := ConditionOfCheckSelfStatus;
  ConditionCmdArray[nNC_CheckShieldStateOpen] := ConditionOfCheckShieldStateOpen;
  ConditionCmdArray[nNC_CheckFullBead] := ConditionOfCheckFullBead;
  ConditionCmdArray[nNA_CheckVarInList] := ConditionOfCheckVarInList;
  ConditionCmdArray[nNA_CheckListAllDigit] := ConditionOfCheckListAllDigit;
  ConditionCmdArray[nNA_CheckStateValue] := ConditionOfCheckStateValue;

  TxtFileCache := TDictionary<string, TStringList>.Create;


finalization
  CloseTxtFileObjects;

end.

