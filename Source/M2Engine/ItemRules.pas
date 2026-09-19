unit ItemRules;

interface

uses
  Windows, Classes, SysUtils, CheckUnit, Grobal2;

type
  {
    0   禁止丢弃
    1   禁止交易
    2   禁止存仓
    3   禁止修理
    4   禁止出售
    5   上线消失
    6   死亡必爆
    7   禁止英雄
    8   禁止寄售
    9   禁止存入个人商店
    10  禁止挑战
    11  禁止宝石升级
    12  怪物掉落提示
    13  永不掉落
    14  禁止商铺打折
    15  英雄包裹
    16  英雄物品
    17  禁止升级
    18  死亡消失
    19  挖取提示
    20  禁止捡起
    21  触发提示
    22  下线必掉
    23  宝箱提示
    24  丢弃消失
    25  触发ID
    26  人物掉落提示
    27  禁止拍卖
    28  掉落触发
    29  宝箱物品触发
    30  禁止透视
    31  禁止宠物背包
  }

  TFlagArray = array[0..40] of Boolean;

  pTFlagArray = ^TFlagArray;

  TItemRule = record
    ItemIdx: Integer;
    ItemName: string;
    FlagArray: TFlagArray;
    PricesLime: TAcutionItemPricesLime;
  end;

  pTItemRule = ^TItemRule;

  TItemRules = class
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): pTItemRule;
  protected
    function Compare(Key1, Key2: Integer): Integer; virtual;
    function Search(ItemIdx: Integer; var Index: Integer): Boolean; virtual;
  public
    constructor Create();
    destructor Destroy; override;

    procedure LoadFromFile;
    procedure SaveToFile;

    function Add(sItemName: string; FlagArray: TFlagArray; AcutionItemPrices: PTAcutionItemPricesLime): pTItemRule;
    function Delete(sItemName: string): Boolean;

    procedure Clear;

    function Find(sItemName: string): pTItemRule;
    function Get(ItemIdx: Integer; nFlag: Integer): Boolean;
    function GetEx(ItemIdx: Integer; nFlag: Integer; var AcutionItemPrices: TAcutionItemPricesLime): Boolean;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: pTItemRule read GetItems;
  end;

implementation

uses
  M2Share, HUtil32, Math, EDCode;

constructor TItemRules.Create();
begin
  FList := TList.Create;
end;

destructor TItemRules.Destroy;
begin
  Clear;
  FList.Free;

  inherited;
end;

procedure TItemRules.Clear;
var
  I: Integer;
  ItemRule: pTItemRule;
begin
  for I := 0 to FList.Count - 1 do
  begin
    ItemRule := FList.Items[I];
    Dispose(ItemRule);
  end;

  FList.Clear;
end;

function TItemRules.GetCount: Integer;
begin
  if FList <> nil then
  begin
    Result := FList.Count;
  end
  else
    Result := 0
end;

procedure TItemRules.LoadFromFile;
var
  I, II, nC, nItemIdx, Index: Integer;
  sFileName: string;
  sLineText: string;
  sItemName: string;
  ItemRule: pTItemRule;
  LoadList: TStringList;
  TempList: TStringList;
  SAuctionPrices, sValue: string;
  S, sTemp: AnsiString;
  RulesActionItem: TRulesActionItem;
begin
  S := '';

  Clear;
  FList.Capacity := UserEngine.StdItemList.Count + 256;

  sFileName := g_Config.sEnvirDir + 'ItemRuleList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      sLineText := GetValidStr3(sLineText, sItemName, [' ', #9]);
      SAuctionPrices := GetValidStr3_Ex(sLineText, sLineText, '|');
      nItemIdx := UserEngine.GetStdItemIdx(sItemName);

      if (sItemName <> '') and (nItemIdx >= 0) and (not Search(nItemIdx, Index)) then
      begin
        New(ItemRule);
        FillChar(ItemRule.FlagArray, SizeOf(TFlagArray), 0);
        FillChar(ItemRule.PricesLime.Min, SizeOf(ItemRule.PricesLime.Min), 0);
        FillChar(ItemRule.PricesLime.Max, SizeOf(ItemRule.PricesLime.Max), 0);

        ItemRule.ItemIdx := nItemIdx;
        ItemRule.ItemName := sItemName;
        TempList := TStringList.Create;
        try
          ExtractStrings([' '], [], PChar(Trim(sLineText)), TempList);
          nC := Min(High(ItemRule.FlagArray), TempList.Count - 1);
          for II := 0 to nC do
            ItemRule.FlagArray[II] := TempList.Strings[II] = '1';

          for II := 0 to Length(ItemRule.PricesLime.Min) - 1 do
          begin
            SAuctionPrices := GetValidStr3(SAuctionPrices, sValue, [' ', #9]);
            ItemRule.PricesLime.Min[II] := StrToIntDef(Trim(sValue), 0);

            SAuctionPrices := GetValidStr3(SAuctionPrices, sValue, [' ', #9]);
            ItemRule.PricesLime.Max[II] := StrToIntDef(Trim(sValue), 0);
          end;

          if ItemRule.FlagArray[27] then
          begin
            RulesActionItem.ItemName := ItemRule.ItemName;
            Move(ItemRule.PricesLime, RulesActionItem.Prices, SizeOf(ItemRule.PricesLime));

            SetLength(sTemp, SizeOf(RulesActionItem));
            Move(RulesActionItem, sTemp[1], SizeOf(RulesActionItem));

            S := S + sTemp;
          end;
        finally
          TempList.Free;
        end;

        FList.Insert(Index, ItemRule);
      end;
    end;
    LoadList.Free;
  end;

  g_EnabledAuctionItemListTextLen := Length(S);
  S := zLibCompressString(S);
  g_EnabledAuctionItemListTextCRC := BufferCrc(PAnsiChar(S), Length(S));
  g_EnabledAuctionItemListText := S;
end;

procedure TItemRules.SaveToFile;
var
  I, II: Integer;
  sFileName: string;
  sLineText: string;
  ItemRule: pTItemRule;
  SaveList: TStringList;
  nLen: Integer;
  S, sValue: AnsiString;
  CRC32: LongWord;
  RulesActionItem: TRulesActionItem;
begin
  if FList <> nil then
  begin
    sFileName := g_Config.sEnvirDir + 'ItemRuleList.txt';
    SaveList := TStringList.Create;

    S := '';
    for I := 0 to FList.Count - 1 do
    begin
      ItemRule := FList.Items[I];
      sLineText := ItemRule.ItemName + #9;
      for II := 0 to High(ItemRule.FlagArray) do
      begin
        sLineText := sLineText + IntToStr(BoolToInt(ItemRule.FlagArray[II])) + ' ';
      end;

      sLineText := sLineText + '|';
      for II := 0 to High(ItemRule.PricesLime.Min) do
      begin
        sLineText := sLineText + IntToStr(ItemRule.PricesLime.Min[II]) + ' ';
        sLineText := sLineText + IntToStr(ItemRule.PricesLime.Max[II]) + ' ';
      end;

      if ItemRule.FlagArray[27] then
      begin
        RulesActionItem.ItemName := ItemRule.ItemName;
        Move(ItemRule.PricesLime, RulesActionItem.Prices, SizeOf(ItemRule.PricesLime));

        SetLength(sValue, SizeOf(RulesActionItem));
        Move(RulesActionItem, sValue[1], SizeOf(RulesActionItem));

        S := S + sValue;
      end;

      sLineText := Trim(sLineText);
      SaveList.Add(sLineText);
    end;

    nLen := Length(S);
    S := zLibCompressString(S);
    CRC32 := BufferCrc(PAnsiChar(S), Length(S));
    if CRC32 <> g_EnabledAuctionItemListTextCRC then
    begin
      g_EnabledAuctionItemListTextCRC := CRC32;
      g_EnabledAuctionItemListTextLen := nLen;
      g_EnabledAuctionItemListText := S;

      UserEngine.SendEnabledAuctionItemList;
    end;

    try
      SaveList.SaveToFile(sFileName);
    except

    end;
    SaveList.Free;
  end;
end;

function TItemRules.Find(sItemName: string): pTItemRule;
var
  ItemIdx, Index: Integer;
begin
  Result := nil;

  ItemIdx := UserEngine.GetStdItemIdx(sItemName);
  if ItemIdx < 0 then
    Exit;

  if Search(ItemIdx, Index) then
  begin
    Result := FList.Items[Index];
  end;
end;

function TItemRules.Add(sItemName: string; FlagArray: TFlagArray; AcutionItemPrices: PTAcutionItemPricesLime): pTItemRule;
var
  ItemIdx, Index: Integer;
begin
  Result := nil;

  ItemIdx := UserEngine.GetStdItemIdx(sItemName);
  if ItemIdx < 0 then
    Exit;

  if not Search(ItemIdx, Index) then
  begin
    New(Result);
    Result.ItemIdx := ItemIdx;
    Result.ItemName := sItemName;
    Result.FlagArray := FlagArray;
    Result.PricesLime := AcutionItemPrices^;

    FList.Insert(Index, Result);
  end;
end;

function TItemRules.Delete(sItemName: string): Boolean;
var
  ItemIdx, Index: Integer;
  ItemRule: pTItemRule;
begin
  Result := False;

  ItemIdx := UserEngine.GetStdItemIdx(sItemName);
  if ItemIdx < 0 then
    Exit;

  if Search(ItemIdx, Index) then
  begin
    ItemRule := FList.Items[Index];

    Dispose(ItemRule);
    FList.Delete(Index);
    Result := True;
  end;
end;

function TItemRules.Get(ItemIdx: Integer; nFlag: Integer): Boolean;
var
  Index: Integer;
  ItemRule: pTItemRule;
begin
  Result := False;

  if nFlag in [0..40] then
  begin
    if Search(ItemIdx, Index) then
    begin
      ItemRule := FList.Items[Index];
      Result := ItemRule.FlagArray[nFlag];
    end;
  end;
end;

function TItemRules.GetEx(ItemIdx: Integer; nFlag: Integer; var AcutionItemPrices: TAcutionItemPricesLime): Boolean;
var
  Index: Integer;
  ItemRule: pTItemRule;
begin
  Result := False;
  if FList <> nil then
  begin
    if nFlag in [0..40] then
    begin
      if Search(ItemIdx, Index) then
      begin
        ItemRule := FList.Items[Index];
        Result := ItemRule.FlagArray[nFlag];
        AcutionItemPrices := ItemRule.PricesLime;
      end;
    end;
  end;
end;

function TItemRules.GetItems(Index: Integer): pTItemRule;
begin
  Result := FList.Items[Index];
end;

function TItemRules.Compare(Key1, Key2: Integer): Integer;
begin
  Result := Key1 - Key2;
end;

function TItemRules.Search(ItemIdx: Integer; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Search := False;
  L := 0;
  H := Count - 1;
  while L <= H do
  begin
    I := L + (H - L) shr 1;
    C := Compare(pTItemRule(Items[I]).ItemIdx, ItemIdx);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Search := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

end.

