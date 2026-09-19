unit SndaShop;

interface

uses
  Windows, Classes, SysUtils, Grobal2, StrUtils;

type
  TShopItem = record
    ShopType: Integer;
    StdItem: TStdItem;
    GameMoney: Integer;
    ImageIndex: Integer;
    ImageCount: Integer;
    Memo1: string[18];
    Memo2: string[150];
    ItemCount: Integer;
    boBulkBuy: Boolean;
    nBulkBuyCount: Integer;
  end;

  pTShopItem = ^TShopItem;

  TSndaShopList = class
  private
    ItemList: TList;
    FRecordCount: Integer;
    function GetList(Index: Integer): TList;
  public
    constructor Create();
    destructor Destroy; override;
    function Get(ItemName: string): pTShopItem;
    function GetEx(ItemName: string; MoneyType: Integer): pTShopItem;
    function Add(ShopItem: pTShopItem): Boolean;
    function Delete(ShopItem: pTShopItem): Boolean;
    procedure UpdateStdItem(StdItem: pTStdItem);
    procedure LoadFromFile;
    procedure SaveToFile;
    procedure Up(ShopItem: pTShopItem);
    procedure Down(ShopItem: pTShopItem);
    property Items[Index: Integer]: TList read GetList;
    property RecordCount: Integer read FRecordCount;
  end;

implementation

uses HUtil32, M2Share;

constructor TSndaShopList.Create();
var
  I: Integer;
begin
  FRecordCount := 0;
  ItemList := TList.Create;
  for I := 0 to 5 do
  begin
    ItemList.Add(TList.Create);
  end;
end;

destructor TSndaShopList.Destroy;
var
  I, II: Integer;
begin
  for I := 0 to ItemList.Count - 1 do
  begin
    for II := 0 to TList(ItemList.Items[I]).Count - 1 do
    begin
      Dispose(pTShopItem(TList(ItemList.Items[I]).Items[II]));
    end;
    TList(ItemList.Items[I]).Free;
  end;
  ItemList.Free;
  inherited Destroy;
end;

function TSndaShopList.GetList(Index: Integer): TList;
begin
  if not(Index in [0 .. 5]) then
  begin
    Result := nil;
    Exit;
  end;
  Result := TList(ItemList.Items[Index]);
end;

procedure TSndaShopList.Up(ShopItem: pTShopItem);
var
  nIndex: Integer;
  List: TList;
begin
  List := Items[ShopItem.ShopType];
  if List = nil then
    Exit;
  if List.Count <= 1 then
    Exit;
  nIndex := List.IndexOf(ShopItem);
  if nIndex >= 0 then
  begin
    if (nIndex - 1 >= 0) and (nIndex - 1 < List.Count) then
    begin
      List.Delete(nIndex);
      Dec(nIndex);
      List.Insert(nIndex, ShopItem);
    end;
  end;
end;

procedure TSndaShopList.Down(ShopItem: pTShopItem);
var
  nIndex: Integer;
  List: TList;
begin
  List := Items[ShopItem.ShopType];
  if List = nil then
    Exit;
  nIndex := List.IndexOf(ShopItem);
  if nIndex >= 0 then
  begin
    if (nIndex + 1 >= 0) and (nIndex + 1 < List.Count) then
    begin
      List.Delete(nIndex);
      Inc(nIndex);
      List.Insert(nIndex, ShopItem);
    end;
  end;
end;

procedure TSndaShopList.UpdateStdItem(StdItem: pTStdItem);
var
  I, Index, Price: Integer;
  List: TList;
  ShopItem: pTShopItem;
begin
  for Index := 0 to 5 do
  begin
    List := TList(ItemList.Items[Index]);
    for I := 0 to List.Count - 1 do
    begin
      ShopItem := List.Items[I];
      if CompareText(ShopItem.StdItem.Name, StdItem.Name) = 0 then
      begin
        Price := ShopItem.StdItem.Price;
        ShopItem.StdItem := StdItem^;
        ShopItem.StdItem.Price := Price;
        break;
      end;
    end;
  end;
end;

procedure TSndaShopList.SaveToFile;
var
  I, II: Integer;
  SaveList: TStringList;
  sFileName: string;
  sMemo2: string;
  ShopItem: pTShopItem;
  sLineText: string;
  List: TList;
const
  WideCRLF: string = #13#10;
begin
  SaveList := TStringList.Create;
  for I := 0 to 5 do
  begin
    List := TList(ItemList.Items[I]);
    for II := 0 to List.Count - 1 do
    begin
      ShopItem := pTShopItem(List.Items[II]);
      sMemo2 := ShopItem.Memo2;
      while Pos(WideCRLF, sMemo2) > 0 do
      begin
        sMemo2 := AnsiReplaceText(sMemo2, WideCRLF, '|');
      end;
      sLineText := IntToStr(ShopItem.ShopType) + #9 + ShopItem.StdItem.Name + #9 + IntToStr(ShopItem.StdItem.Looks) + #9 +
        IntToStr(ShopItem.StdItem.Price) + '|' + IntToStr(ShopItem.GameMoney) + #9 + IntToStr(ShopItem.ImageIndex) + #9 +
        IntToStr(ShopItem.ImageCount) + #9 + ShopItem.Memo1 + '|' + sMemo2 + #9 + IntToStr(ShopItem.ItemCount) + #9 +
        IntToStr(Integer(ShopItem.boBulkBuy)) + #9 + IntToStr(ShopItem.nBulkBuyCount);
      SaveList.Add(sLineText);
    end;
  end;

  sFileName := g_Config.sEnvirDir + 'ShopItemList.txt';
  try
    SaveList.SaveToFile(sFileName);
  except
  end;
  SaveList.Free;
end;

procedure TSndaShopList.LoadFromFile;
var
  I, II: Integer;
  LoadList: TStringList;
  sFileName: string;

  ShopItem: pTShopItem;
  StdItem: pTStdItem;
  nShopType, nPrice, nGameMoney, nImageIndex, nImageCount, nItemCount: Integer;
  tStr, sShopType, sItemName, s01, sPrice, sGameMoney, sImageIndex, sImageCount, sMemo1, sMemo2, sItemCount, sBulkBuy,
    sBulkBuyCount: string;
  List: TList;
const
  WideCRLF: string = #13#10;
begin
  for I := 0 to 5 do
  begin
    List := TList(ItemList.Items[I]);
    for II := 0 to List.Count - 1 do
    begin
      Dispose(pTShopItem(List.Items[II]));
    end;
    List.Clear;
  end;
  FRecordCount := 0;
  sFileName := g_Config.sEnvirDir + 'ShopItemList.txt';
  if not FileExists(sFileName) then
    Exit;

  LoadList := TStringList.Create;
  try
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      tStr := LoadList.Strings[I];
      if (tStr <> '') and (tStr[1] <> ';') then
      begin
        tStr := GetValidStr3(tStr, sShopType, [' ', #9]);
        tStr := GetValidStr3(tStr, sItemName, [' ', #9]);
        tStr := GetValidStr3(tStr, s01, [' ', #9]);
        tStr := GetValidStr3(tStr, sGameMoney, [' ', #9]);
        sGameMoney := GetValidStr3(sGameMoney, sPrice, ['|', #9]);

        tStr := GetValidStr3(tStr, sImageIndex, [' ', #9]);
        tStr := GetValidStr3(tStr, sImageCount, [' ', #9]);

        // 修复商铺物品描述不支持空格 {' ',} chongchong 2014-01-14
        tStr := GetValidStr3(tStr, sMemo2, [ { ' ', } #9]);
        sMemo2 := GetValidStr3(sMemo2, sMemo1, ['|', #9]);

        tStr := GetValidStr3(tStr, sItemCount, [' ', #9]);
        tStr := GetValidStr3(tStr, sBulkBuy, [' ', #9]);
        sBulkBuyCount := tStr;

        if Length(sMemo2) > 0 then
        begin
          while Pos('|', sMemo2) > 0 do
          begin
            sMemo2 := AnsiReplaceText(sMemo2, '|', WideCRLF);
          end;
          { for II := 1 to Length(sMemo2) do begin
            if sMemo2[II] = '|' then
            sMemo2[II] := WideCRLF[1];
            end; }
        end;
        nShopType := StrToIntDef(sShopType, -1);
        nPrice := StrToIntDef(sPrice, -1);
        nGameMoney := StrToIntDef(sGameMoney, 0);
        nImageIndex := StrToIntDef(sImageIndex, -1);
        nImageCount := StrToIntDef(sImageCount, -1);
        nItemCount := StrToIntDef(sItemCount, 1);
        if nItemCount < 1 then
          nItemCount := 1;

        if (nImageCount = 0) then
          nImageCount := 1;
        if (nImageIndex = 0) then
          nImageIndex := 380;

        // if nGameMoney = 1 then nGameMoney := 2;

        if (nShopType in [0 .. 5]) and (nPrice >= 0) and (nGameMoney in [0 .. 4]) and (sItemName <> '') and (sMemo1 <> '') then
        begin
          StdItem := UserEngine.GetStdItem(sItemName);
          if StdItem <> nil then
          begin
            Inc(FRecordCount);
            New(ShopItem);
            FillChar(ShopItem^, SizeOf(TShopItem), #0);
            ShopItem.ShopType := nShopType;
            ShopItem.StdItem := StdItem^;
            ShopItem.StdItem.Price := nPrice;
            ShopItem.GameMoney := nGameMoney;
            ShopItem.ImageIndex := nImageIndex;
            ShopItem.ImageCount := nImageCount;
            ShopItem.Memo1 := sMemo1;
            ShopItem.Memo2 := sMemo2;
            ShopItem.ItemCount := nItemCount;
            ShopItem.boBulkBuy := StrToIntDef(sBulkBuy, 0) <> 0;
            ShopItem.nBulkBuyCount := StrToIntDef(sBulkBuyCount, 99);
            if ShopItem.nBulkBuyCount < 0 then
              ShopItem.nBulkBuyCount := 1;
            Items[nShopType].Add(ShopItem);
          end;
        end;
      end;
    end;
  finally
    LoadList.Free;
  end;
end;

function TSndaShopList.Add(ShopItem: pTShopItem): Boolean;
var
  I, II: Integer;
  List: TList;
begin
  Result := False;
  if (ShopItem.ShopType in [0 .. 5]) then
  begin
    for I := 0 to ItemList.Count - 1 do
    begin
      List := TList(ItemList.Items[I]);
      for II := 0 to List.Count - 1 do
      begin
        if CompareText(ShopItem.StdItem.Name, pTShopItem(List.Items[II]).StdItem.Name) = 0 then
          Exit;
      end;
    end;
    List := TList(ItemList.Items[ShopItem.ShopType]);
    Inc(FRecordCount);
    List.Add(ShopItem);
    Result := True;
  end;
end;

function TSndaShopList.Delete(ShopItem: pTShopItem): Boolean;
var
  I, II: Integer;
  List: TList;
begin
  Result := False;
  for I := 0 to ItemList.Count - 1 do
  begin
    List := TList(ItemList.Items[I]);
    for II := 0 to List.Count - 1 do
    begin
      if List.Items[II] = ShopItem then
      begin
        Dec(FRecordCount);
        List.Delete(II);
        Dispose(ShopItem);
        Result := True;
        Exit;
      end;
    end;
  end;
end;

function TSndaShopList.Get(ItemName: string): pTShopItem;
var
  I, II: Integer;
  List: TList;
  ShopItem: pTShopItem;
begin
  Result := nil;
  for I := 0 to ItemList.Count - 1 do
  begin
    List := TList(ItemList.Items[I]);
    for II := 0 to List.Count - 1 do
    begin
      ShopItem := pTShopItem(List.Items[II]);
      if SameText(ShopItem.StdItem.Name, ItemName) then
      begin
        Result := ShopItem;
        Exit;
      end;
    end;
  end;
end;

function TSndaShopList.GetEx(ItemName: string; MoneyType: Integer): pTShopItem;
var
  I, II: Integer;
  List: TList;
  ShopItem: pTShopItem;
begin
  Result := nil;
  for I := 0 to ItemList.Count - 1 do
  begin
    List := TList(ItemList.Items[I]);
    for II := 0 to List.Count - 1 do
    begin
      ShopItem := pTShopItem(List.Items[II]);
      if SameText(ShopItem.StdItem.Name, ItemName) and (ShopItem.GameMoney = MoneyType) then
      begin
        Result := ShopItem;
        Exit;
      end;
    end;
  end;
end;

end.
