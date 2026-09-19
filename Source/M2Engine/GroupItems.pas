unit GroupItems;

interface

uses
  Windows, Classes, SysUtils, Grobal2, IniFiles, Forms, IniFilesEx;

type
  TGroupItem = record
    FLD_INDEX: Integer;
    FLD_COUNT: Integer;
    FLD_DESC: string;
    FLD_ITEMNAMES: TStringList;
    FLD_FLAG: array [0 .. 40 - 1] of Boolean;
    FLD_RATE: array [0 .. 40 - 1] of Integer;
    FLD_VALUE: array [0 .. 40 - 1] of Integer;

    AttackSkillPercent: array [1 .. 114] of Byte; // 技能威力攻击百分比
    DefenseSkillPercent: array [1 .. 114] of Byte; // 技能威力防御百分比
    FLD_HINTMSG: string;
  end;

  pTGroupItem = ^TGroupItem;

  TGroupItems = class
  private
    FRecordCount: Integer;
    FIndex: Integer;
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): pTGroupItem;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadFromFile;
    procedure SaveToFile;

    function Get(UseItems: THumanUseItems; JewelryBoxItems: THumanJewelryBoxItems; GodBlessItems: THumanGodBlessItems;
      FengHaoItems: TList; ActiveFengHao: Integer; GroupList: TList): Integer;
    function FindIndex(Index: Integer): Boolean;
    function Find(GroupItem: pTGroupItem): Boolean;
    function Add(Item: pTGroupItem): Boolean;
    function RateValue(Rate, Value: Integer): Integer;
    function RateValue2(Rate: Integer; Value: LongWord): Int64;
    function Delete(Item: pTGroupItem): Boolean;
    property Items[Index: Integer]: pTGroupItem read GetItems;
    property Count: Integer read GetCount;
    property RecordCount: Integer read FRecordCount;
  end;

implementation

uses
  Math, M2Share, HUtil32;

constructor TGroupItems.Create();
begin
  FList := TList.Create;
  FRecordCount := 0;
  FIndex := 0;
end;

destructor TGroupItems.Destroy;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    pTGroupItem(FList.Items[I]).FLD_ITEMNAMES.Free;
    Dispose(pTGroupItem(FList.Items[I]));
  end;
  FList.Free;
  inherited;
end;

function TGroupItems.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TGroupItems.GetItems(Index: Integer): pTGroupItem;
begin
  Result := FList.Items[Index];
end;

procedure TGroupItems.LoadFromFile;
var
  I, II, nC: Integer;
  sFileName: string;
  sLineText: string;
  sIndex: string;
  nIndex: Integer;
  sCount: string;
  nCount: Integer;
  sItemDesc: string;
  sItemNames: string;
  sBooleans: string;
  sRateValues: string;
  sValues: string;
  sHintMsg: string;
  LoadList: TStringList;
  GroupItem: pTGroupItem;
  TempList: TStringList;
  IniFile: TIniFileEx;
begin
  FRecordCount := 0;
  for I := 0 to FList.Count - 1 do
  begin
    pTGroupItem(FList.Items[I]).FLD_ITEMNAMES.Free;
    Dispose(pTGroupItem(FList.Items[I]));
  end;
  FList.Clear;

  sFileName := g_Config.sEnvirDir + 'GroupItemList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sIndex, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sCount, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sItemDesc, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sItemNames, [' ', #9]);
        nIndex := StrToIntDef(sIndex, -1);
        nCount := StrToIntDef(sCount, -1);
        if (sItemDesc <> '') and (sItemNames <> '') and (nIndex >= 0) and (nCount > 0) then
        begin
          TempList := TStringList.Create;
          ExtractStrings(['|'], [], PChar(Trim(sItemNames)), TempList);
          TrimStringList(TempList);

          New(GroupItem);
          FillChar(GroupItem^, SizeOf(TGroupItem), #0);
          GroupItem.FLD_ITEMNAMES := TStringList.Create;
          GroupItem.FLD_ITEMNAMES.AddStrings(TempList);
          GroupItem.FLD_INDEX := nIndex;
          GroupItem.FLD_COUNT := nCount;
          GroupItem.FLD_DESC := sItemDesc;
          FillChar(GroupItem.DefenseSkillPercent, SizeOf(GroupItem.DefenseSkillPercent), 0); // 增加技能防御百分百
          FillChar(GroupItem.AttackSkillPercent, SizeOf(GroupItem.AttackSkillPercent), 0); // 增加技能攻击百分百

          sLineText := GetValidStr3(sLineText, sBooleans, [' ', #9]);
          sLineText := GetValidStr3(sLineText, sRateValues, [' ', #9]);
          sLineText := GetValidStr3(sLineText, sValues, [' ', #9]);
          sLineText := GetValidStr3(sLineText, sHintMsg, [' ', #9]);
          GroupItem.FLD_HINTMSG := sHintMsg;

          TempList.Clear;
          ExtractStrings(['|'], [], PChar(Trim(sBooleans)), TempList);
          nC := Min(40, TempList.Count);
          for II := 0 to nC - 1 do
            GroupItem.FLD_FLAG[II] := TempList.Strings[II] = '1';

          TempList.Clear;
          ExtractStrings(['|'], [], PChar(Trim(sRateValues)), TempList);
          nC := Min(40, TempList.Count);
          for II := 0 to nC - 1 do
            GroupItem.FLD_RATE[II] := StrToIntDef(TempList.Strings[II], 0);

          TempList.Clear;
          ExtractStrings(['|'], [], PChar(Trim(sValues)), TempList);
          nC := Min(40, TempList.Count);
          for II := 0 to nC - 1 do
            GroupItem.FLD_VALUE[II] := StrToIntDef(TempList.Strings[II], 0);

          FList.Add(GroupItem);
          Inc(FRecordCount);
          TempList.Free;
        end;
      end;
    end;
    LoadList.Free;
  end;

  sFileName := g_Config.sEnvirDir + 'GroupItemSkillPowerList.txt';
  if FileExists(sFileName) then
  begin
    IniFile := TIniFileEx.Create(sFileName);
    for I := 0 to FList.Count - 1 do
    begin
      GroupItem := pTGroupItem(FList.Items[I]);
      for II := 1 to High(GroupItem.AttackSkillPercent) do
      begin
        if not IsValidMagicInSkillPowerItem(II) then
          Continue;
        GroupItem.AttackSkillPercent[II] := IniFile.ReadInteger(IntToStr(GroupItem.FLD_INDEX), 'Attack' + IntToStr(II), 0);
        GroupItem.DefenseSkillPercent[II] := IniFile.ReadInteger(IntToStr(GroupItem.FLD_INDEX), 'Defense' + IntToStr(II), 0);
      end;
    end;
    IniFile.Free;
  end;
end;

procedure TGroupItems.SaveToFile;
var
  I, II: Integer;
  sFileName: string;
  sLineText: string;
  sItemName: string;
  sFlag: string;
  sRate: string;
  sValue: string;
  GroupItem: pTGroupItem;
  SaveList: TStringList;
  IniFile: TIniFileEx;
begin
  sFileName := g_Config.sEnvirDir + 'GroupItemList.txt';
  SaveList := TStringList.Create;
  try
    for I := 0 to FList.Count - 1 do
    begin
      GroupItem := FList.Items[I];

      sItemName := '';
      for II := 0 to GroupItem.FLD_ITEMNAMES.Count - 1 do
        sItemName := sItemName + GroupItem.FLD_ITEMNAMES.Strings[II] + '|';
      if sItemName[Length(sItemName)] = '|' then
        sItemName := Copy(sItemName, 1, Length(sItemName) - 1);

      sFlag := '';
      for II := 0 to High(GroupItem.FLD_FLAG) do
        sFlag := sFlag + IntToStr(BoolToInt(GroupItem.FLD_FLAG[II])) + '|';
      if sFlag[Length(sFlag)] = '|' then
        sFlag := Copy(sFlag, 1, Length(sFlag) - 1);

      sRate := '';
      for II := 0 to High(GroupItem.FLD_RATE) do
        sRate := sRate + IntToStr(GroupItem.FLD_RATE[II]) + '|';
      if sRate[Length(sRate)] = '|' then
        sRate := Copy(sRate, 1, Length(sRate) - 1);

      sValue := '';
      for II := 0 to High(GroupItem.FLD_VALUE) do
        sValue := sValue + IntToStr(GroupItem.FLD_VALUE[II]) + '|';
      if sValue[Length(sValue)] = '|' then
        sValue := Copy(sValue, 1, Length(sValue) - 1);

      sLineText := IntToStr(GroupItem.FLD_INDEX) + #9 + IntToStr(GroupItem.FLD_COUNT) + #9 + GroupItem.FLD_DESC + #9 + sItemName +
        #9 + sFlag + #9 + sRate + #9 + sValue + #9 + GroupItem.FLD_HINTMSG;

      SaveList.Add(sLineText);
    end;

    SaveList.SaveToFile(sFileName);
  finally
    SaveList.Free;
  end;

  sFileName := g_Config.sEnvirDir + 'GroupItemSkillPowerList.txt';
  // if FileExists(sFileName) then begin
  IniFile := TIniFileEx.Create(sFileName);
  for I := 0 to FList.Count - 1 do
  begin
    GroupItem := pTGroupItem(FList.Items[I]);
    for II := 1 to High(GroupItem.AttackSkillPercent) do
    begin
      if not IsValidMagicInSkillPowerItem(II) then
        Continue;
      if GroupItem.AttackSkillPercent[II] <> 0 then
        IniFile.WriteInteger(IntToStr(GroupItem.FLD_INDEX), 'Attack' + IntToStr(II), GroupItem.AttackSkillPercent[II]);
      if GroupItem.DefenseSkillPercent[II] <> 0 then
        IniFile.WriteInteger(IntToStr(GroupItem.FLD_INDEX), 'Defense' + IntToStr(II), GroupItem.DefenseSkillPercent[II]);
    end;
  end;
  IniFile.Free;
  // end;
end;

function TGroupItems.Get(UseItems: THumanUseItems; JewelryBoxItems: THumanJewelryBoxItems; GodBlessItems: THumanGodBlessItems;
  FengHaoItems: TList; ActiveFengHao: Integer; GroupList: TList): Integer;
var
  UseItemArray: array [Low(THumanUseItems) .. High(THumanUseItems)] of Boolean;
  JewelryBoxItemsArray: array [Low(THumanJewelryBoxItems) .. High(THumanJewelryBoxItems)] of Boolean;
  GodBlessItemsArray: array [Low(THumanGodBlessItems) .. High(THumanGodBlessItems)] of Boolean;
  FengHaoItemsArray: array [Low(THumanFengHaoItems) .. High(THumanFengHaoItems)] of Boolean;

  function FindUseItems(sItemName: string): Boolean;
  var
    I: Integer;
    StdItem: pTStdItem;
    sUserItemName: string;
  begin
    Result := False;
    if not g_Config.boTZSupportRenameItem then
    begin
      for I := Low(THumanUseItems) to High(THumanUseItems) do
      begin
        if (UseItems[I].wIndex <= 0) or UseItemArray[I] then
          Continue;

        StdItem := UserEngine.GetStdItem(UseItems[I].wIndex);
        if StdItem = nil then
          Continue;

        if (Comparetext(sItemName, StdItem.Name) = 0) then
        begin
          UseItemArray[I] := True;
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      for I := Low(THumanUseItems) to High(THumanUseItems) do
      begin
        if (UseItems[I].wIndex <= 0) or UseItemArray[I] then
          Continue;

        StdItem := UserEngine.GetStdItem(UseItems[I].wIndex);
        if StdItem = nil then
          Continue;

        sUserItemName := '';
        if UseItems[I].btValue[13] = 1 then
          sUserItemName := UseItems[I].Name
        else
          sUserItemName := StdItem.Name;

        if (Comparetext(sItemName, sUserItemName) = 0) then
        begin
          UseItemArray[I] := True;
          Result := True;
          Break;
        end;
      end;
    end;
  end;

  function FindJewelryBoxItems(sItemName: string): Boolean;
  var
    I: Integer;
    StdItem: pTStdItem;
    sUserItemName: string;
  begin
    Result := False;
    if not g_Config.boTZSupportRenameItem then
    begin
      for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
      begin
        if (JewelryBoxItems[I].wIndex <= 0) or JewelryBoxItemsArray[I] then
          Continue;

        StdItem := UserEngine.GetStdItem(JewelryBoxItems[I].wIndex);
        if StdItem = nil then
          Continue;

        if (Comparetext(sItemName, StdItem.Name) = 0) then
        begin
          JewelryBoxItemsArray[I] := True;
          Result := True;
          Break;
        end;
      end;
    end
    else
    begin
      for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
      begin
        if (JewelryBoxItems[I].wIndex <= 0) or JewelryBoxItemsArray[I] then
          Continue;

        StdItem := UserEngine.GetStdItem(JewelryBoxItems[I].wIndex);
        if StdItem = nil then
          Continue;

        sUserItemName := '';
        if JewelryBoxItems[I].btValue[13] = 1 then
          sUserItemName := JewelryBoxItems[I].Name
        else
          sUserItemName := StdItem.Name;

        if (Comparetext(sItemName, sUserItemName) = 0) then
        begin
          JewelryBoxItemsArray[I] := True;
          Result := True;
          Break;
        end;
      end;
    end;
  end;

  function FindGodBlessItems(sItemName: string): Boolean;
  var
    I: Integer;
    StdItem: pTStdItem;
    sUserItemName: string;
  begin
    Result := False;
    if not g_Config.boTZSupportRenameItem then
    begin
      I := Low(THumanGodBlessItems);
      while I <= High(THumanGodBlessItems) do
      begin
        if (GodBlessItems[I].wIndex <= 0) or GodBlessItemsArray[I] then
        begin
          Inc(I);
          Continue;
        end;

        StdItem := UserEngine.GetStdItem(GodBlessItems[I].wIndex);
        if StdItem = nil then
        begin
          Inc(I);
          Continue;
        end;

        if (Comparetext(sItemName, StdItem.Name) = 0) then
        begin
          GodBlessItemsArray[I] := True;
          Result := True;
          Break;
        end;

        Inc(I);
      end;
    end
    else
    begin
      I := Low(THumanGodBlessItems);
      while I <= High(THumanGodBlessItems) do
      begin
        if (GodBlessItems[I].wIndex <= 0) or GodBlessItemsArray[I] then
        begin
          Inc(I);
          Continue;
        end;

        StdItem := UserEngine.GetStdItem(GodBlessItems[I].wIndex);
        if StdItem = nil then
        begin
          Inc(I);
          Continue;
        end;

        sUserItemName := '';
        if GodBlessItems[I].btValue[13] = 1 then
          sUserItemName := GodBlessItems[I].Name
        else
          sUserItemName := StdItem.Name;

        if (Comparetext(sItemName, sUserItemName) = 0) then
        begin
          GodBlessItemsArray[I] := True;
          Result := True;
          Break;
        end;

        Inc(I);
      end;
    end;
  end;

  function FindFengHaoItems(sItemName: string): Boolean;
  var
    I: Integer;
    StdItem: pTStdItem;
    UserItem: PTUserItem;
    sUserItemName: string;
  begin
    Result := False;
    if not g_Config.boTZSupportRenameItem then
    begin
      I := 0;
      while I <= FengHaoItems.Count - 1 do
      begin
        if I >= High(THumanFengHaoItems) then
          Exit;

        if FengHaoItemsArray[I] then
        begin
          Inc(I);
          Continue;
        end;

        UserItem := FengHaoItems.Items[I];

        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if StdItem = nil then
        begin
          Inc(I);
          Continue;
        end;

        if (StdItem.AniCount = 0) and (ActiveFengHao <> I) then
        begin
          Inc(I);
          Continue;
        end;

        if (Comparetext(sItemName, StdItem.Name) = 0) then
        begin
          FengHaoItemsArray[I] := True;
          Result := True;
          Break;
        end;

        Inc(I);
      end;
    end
    else
    begin
      I := 0;
      while I <= FengHaoItems.Count - 1 do
      begin
        if I >= High(THumanFengHaoItems) then
          Exit;

        if FengHaoItemsArray[I] then
        begin
          Inc(I);
          Continue;
        end;

        UserItem := FengHaoItems.Items[I];

        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if StdItem = nil then
        begin
          Inc(I);
          Continue;
        end;

        if (StdItem.AniCount = 0) and (ActiveFengHao <> I) then
        begin
          Inc(I);
          Continue;
        end;

        sUserItemName := '';
        if UserItem.btValue[13] = 1 then
          sUserItemName := UserItem.Name
        else
          sUserItemName := StdItem.Name;

        if (Comparetext(sItemName, sUserItemName) = 0) then
        begin
          FengHaoItemsArray[I] := True;
          Result := True;
          Break;
        end;

        Inc(I);
      end;
    end;
  end;

var
  I, II, nCount: Integer;
  GroupItem: pTGroupItem;
begin
  for I := 0 to FList.Count - 1 do
  begin
    GroupItem := FList.Items[I];
    nCount := GroupItem.FLD_COUNT;

    for II := Low(THumanUseItems) to High(THumanUseItems) do
      UseItemArray[II] := False;

    for II := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
      JewelryBoxItemsArray[II] := False;

    for II := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
      GodBlessItemsArray[II] := False;

    for II := Low(THumanFengHaoItems) to High(THumanFengHaoItems) do
      FengHaoItemsArray[II] := False;

    for II := 0 to GroupItem.FLD_ITEMNAMES.Count - 1 do
    begin
      if FindUseItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
        Dec(nCount)

        // 神佑盒计算套装属性 chongchong 2014-04-20
      else if FindGodBlessItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
        Dec(nCount)

        // 首饰盒计算套装属性chongchong 2014-04-03
      else if g_Config.boJewelryCalcGroupAbilitys and FindJewelryBoxItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
        Dec(nCount)

        // 封号计算套装属性 chongchong 2014-05-28
      else if FindFengHaoItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
        Dec(nCount);

      if nCount <= 0 then
      begin
        GroupList.Add(GroupItem);
        Break;
      end;
    end;

    (*
      // 单个物品“套装触发”，如果戴2个相同装备，第2个触发无效。 一个龙之戒指，单个触发+5%血量。戴上第一个龙之戒指时，+5%生效。 但是戴第二个龙之戒指时，是无效的。 2018-07-10 17:55:20
      nCount := GroupItem.FLD_COUNT;
      for II := 0 to GroupItem.FLD_ITEMNAMES.Count - 1 do
      begin
      if FindUseItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
      Dec(nCount)

      // 神佑盒计算套装属性 chongchong 2014-04-20
      else if FindGodBlessItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
      Dec(nCount)

      // 首饰盒计算套装属性chongchong 2014-04-03
      else if g_Config.boJewelryCalcGroupAbilitys and FindJewelryBoxItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
      Dec(nCount)

      // 封号计算套装属性 chongchong 2014-05-28
      else if FindFengHaoItems(GroupItem.FLD_ITEMNAMES.Strings[II]) then
      Dec(nCount);

      if nCount <= 0 then
      begin
      GroupList.Add(GroupItem);
      break;
      end;
      end;
    *)

  end;
  Result := GroupList.Count;
end;

function TGroupItems.RateValue(Rate, Value: Integer): Integer;
var
  Int64Value: Int64;
begin
  if Rate > 0 then
  begin
    Int64Value := Value + Round(Value * (Rate / 100));
    Result := Max(Min(Int64Value, High(Integer)), 0) // Value + Value * Rate div 100
  end
  else
    Result := Value;
end;

function TGroupItems.RateValue2(Rate: Integer; Value: LongWord): Int64;
begin
  if Rate > 0 then
  begin
    Result := Max(0, Value + Round(Value * (Rate / 100)));
  end
  else
    Result := Value;
end;

function TGroupItems.FindIndex(Index: Integer): Boolean;
var
  I: Integer;
  GroupItem: pTGroupItem;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    GroupItem := FList.Items[I];
    if GroupItem.FLD_INDEX = Index then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function TGroupItems.Find(GroupItem: pTGroupItem): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    if GroupItem = FList.Items[I] then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function TGroupItems.Add(Item: pTGroupItem): Boolean;
begin
  FList.Add(Item);
  Inc(FRecordCount);
  Result := True;
end;

function TGroupItems.Delete(Item: pTGroupItem): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    if FList.Items[I] = Item then
    begin
      Item.FLD_ITEMNAMES.Free;
      Dispose(pTGroupItem(FList.Items[I]));
      FList.Delete(I);
      Dec(FRecordCount);
      Result := True;
      Break;
    end;
  end;
end;

end.
