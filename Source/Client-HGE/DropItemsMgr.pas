unit DropItemsMgr;

interface

uses
  Windows,
  Classes,
  SysUtils,
  Graphics,
  HGEFontEx,
  Grobal2;

type
  TDropItem = record
    ID:Integer;

    X:Word;
    Y:Word;

    Visible:Boolean;
    ShowName:Boolean;
    ShowFlash:Boolean;
    FlashStep:Byte;

    Looks:Word;
    OverlapCount:Word;

    boValueItem:Boolean; // 是否极品装备
    ItemColor:TColor;
    Name:string[60];
    DBName:string[60];

    FlashTime:LongWord;
    FlashStepTime:LongWord;

    ItemTexture:TObject;
    TextureWidth:Integer;
    TextureHeight:Integer;

    ShowNameTime:LongWord;
    ShowItem:Pointer;

    NameImageInfo:TImageInfo;
    dwGhostTick:LongWord;

    // 排除到挂机捡物列表时间 chongchong 2014-12-04
    dwGJExcludeTick:LongWord;

    ItemEffect:TDropItemEffect;
    ItemEffectFrame:Integer;
    ItemEffectTick:LongWord;

    ValueItemEffectFrame:Integer;
    ValueItemEffectTick:LongWord;
  end;
  pTDropItem = ^TDropItem;

  TDropItemsMgr = class;

  // 某点掉落物品
  TPointDropItemList = class(TObject)
  private
    FList:TList;
    FDrawList:TList;
    FOwner:TDropItemsMgr;
    FPointX:Word;
    FPointY:Word;
    function GetCount:Integer;
    function GetItems(Index:Integer):PTDropItem;
    function GetDrawCount:Integer;
    function GetDrawItems(Index:Integer):PTDropItem;
  public
    constructor Create(AOwner:TDropItemsMgr; X, Y:Word);
    destructor Destroy; override;

    property X:Word read FPointX;
    property Y:Word read FPointY;

    procedure Clear;
    property Count:Integer read GetCount;
    property Items[Index:Integer]:PTDropItem read GetItems; default;

    procedure RefreshDrawList(ResetShowItem:Boolean = False);
    property DrawCount:Integer read GetDrawCount;
    property DrawItems[Index:Integer]:PTDropItem read GetDrawItems;

  end;

  TDropItemsMgr = class(TObject)
  private
    FCS:TRTLCriticalSection;
    FSortIDItemList:TList;
    FNoUseItemList:TList;
    FPointList:TList;
    function GetCount:Integer;
    function GetItems(Index:Integer):TPointDropItemList;

    procedure ClearAndFree;
  protected
    function IDCompare(ID1, ID2:Integer):Integer; virtual;
    function IDSearch(ID:Integer; var Index:Integer):Boolean; virtual;

    function PointCompare(X1, Y1, X2, Y2:Word):Integer; virtual;
    function PointSearch(X, Y:Word; var Index:Integer):Boolean; virtual;
  public
    constructor Create;
    destructor Destroy; override;

    procedure Clear;
    procedure Lock;
    procedure UnLock;

    procedure RefreshDrawList(ResetShowItem:Boolean = False);

    function GetItemByID(ID:Integer):pTDropItem;
    function GetItemListByPoint(X, Y:Word):TPointDropItemList;
    function GetItemListIndexByY(Y:Word):Integer;
    function AddDropItem(ID:Integer; nX, nY:Word; DBName:string; EffectIndex:Integer; var IsNew:Boolean):pTDropItem;

    function DelDropItem(ID:Integer; var X, Y:Word):pTDropItem;

    property Count:Integer read GetCount;
    property Items[Index:Integer]:TPointDropItemList read GetItems;
  end;

implementation

uses
  MShare,
  GameConfigDlg,
  FilterItems;

{ TPointDropItemList }

constructor TPointDropItemList.Create(AOwner:TDropItemsMgr; X, Y:Word);
begin
  FList := TList.Create;
  FList.Capacity := 4;

  FDrawList := TList.Create;
  FDrawList.Capacity := 3;

  FOwner := AOwner;
  FPointX := X;
  FPointY := Y;
end;

destructor TPointDropItemList.Destroy;
begin
  Clear;
  FList.Free;

  FDrawList.Free;
  inherited;
end;

procedure TPointDropItemList.Clear;
var
  I:Integer;
  DropItem:PTDropItem;
begin
  for I := 0 to FList.Count - 1 do begin
    DropItem := FList.Items[I];
    Dispose(DropItem);
  end;
  FList.Clear;
end;

function TPointDropItemList.GetCount:Integer;
begin
  Result := FList.Count;
end;

function TPointDropItemList.GetItems(Index:Integer):PTDropItem;
begin
  if (Index >= 0) and (Index <= FList.Count - 1) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

function TPointDropItemList.GetDrawCount:Integer;
begin
  Result := FDrawList.Count;
end;

function TPointDropItemList.GetDrawItems(Index:Integer):pTDropItem;
begin
  Result := FDrawList.Items[Index];
end;

procedure TPointDropItemList.RefreshDrawList(ResetShowItem:Boolean = False);
var
  I, II:Integer;
  DropItem:pTDropItem;
  boFind:Boolean;
begin
  FDrawList.Clear;

  for I := 0 to FList.Count - 1 do begin
    DropItem := FList.Items[I];

    if PlugInEnabled and ResetShowItem then begin
      DropItem.ShowItem := g_FileItemDB.Find(DropItem.DBName);
    end;

    if (DropItem.ShowItem <> nil) and DropItem.Visible and (pTShowItem(DropItem.ShowItem).boShowName) then begin
      if FDrawList.Count < 3 then
        FDrawList.Add(DropItem)
      else begin
        DropItem.NameImageInfo.ImageIndexs := nil;
        DropItem.NameImageInfo.Width := 0;
        DropItem.NameImageInfo.Height := 0;
      end;
    end
    else begin
      DropItem.NameImageInfo.ImageIndexs := nil;
      DropItem.NameImageInfo.Width := 0;
      DropItem.NameImageInfo.Height := 0;
    end;
  end;

  if FDrawList.Count >= 3 then Exit;

  if FList.Count > 0 then begin
    for I := 0 to FList.Count - 1 do begin
      DropItem := FList.Items[I];
      boFind := False;
      for II := 0 to FDrawList.Count - 1 do begin
        if FDrawList.Items[II] = DropItem then begin
          boFind := True;
          break;
        end;
      end;

      if (not boFind) and DropItem.Visible then
        FDrawList.Add(DropItem);

      if FDrawList.Count >= 3 then break;
    end;
  end;
end;

{ TDropItemsMgr }

constructor TDropItemsMgr.Create;
var
  I:Integer;
  DropItem:pTDropItem;
begin
  InitializeCriticalSection(FCS);
  FSortIDItemList := TList.Create;
  FSortIDItemList.Capacity := 256;

  FNoUseItemList := TList.Create;
  FNoUseItemList.Capacity := 512;

  for I := 0 to 512 - 1 do begin
    New(DropItem);
    FNoUseItemList.Add(DropItem);
  end;

  FPointList := TList.Create;
  FPointList.Capacity := 256;
end;

destructor TDropItemsMgr.Destroy;
begin
  ClearAndFree;
  FSortIDItemList.Free;
  FNoUseItemList.Free;
  FPointList.Free;
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TDropItemsMgr.ClearAndFree;
var
  I:Integer;
  PointItemList:TPointDropItemList;
  DropItem:pTDropItem;
begin
  for I := 0 to FPointList.Count - 1 do begin
    PointItemList := FPointList.Items[I];
    PointItemList.Free;
  end;
  FPointList.Clear;
  FSortIDItemList.Clear;

  for I := 0 to FNoUseItemList.Count - 1 do begin
    DropItem := FNoUseItemList.Items[I];
    Dispose(DropItem);
  end;
  FNoUseItemList.Clear;
end;

procedure TDropItemsMgr.Clear;
var
  I:Integer;
  PointItemList:TPointDropItemList;
  DropItem:pTDropItem;
begin
  for I := 0 to FSortIDItemList.Count - 1 do begin
    DropItem := FSortIDItemList.Items[I];
    if FNoUseItemList.Count >= 512 then
      Dispose(DropItem)
    else
      FNoUseItemList.Add(DropItem);
  end;
  FSortIDItemList.Clear;

  for I := 0 to FPointList.Count - 1 do begin
    PointItemList := FPointList.Items[I];
    PointItemList.FList.Clear;
    PointItemList.Free;
  end;
  FPointList.Clear;
end;

function TDropItemsMgr.GetCount:Integer;
begin
  Result := FPointList.Count;
end;

function TDropItemsMgr.GetItems(Index:Integer):TPointDropItemList;
begin
  if (Index >= 0) and (Index <= FPointList.Count - 1) then
    Result := FPointList.Items[Index]
  else
    Result := nil;
end;

function TDropItemsMgr.GetItemByID(ID:Integer):pTDropItem;
var
  Index:Integer;
begin
  if IDSearch(ID, Index) then
    Result := FSortIDItemList.Items[Index]
  else
    Result := nil;
end;

function TDropItemsMgr.GetItemListByPoint(X, Y:Word):TPointDropItemList;
var
  Index:Integer;
begin
  if PointSearch(X, Y, Index) then
    Result := FPointList.Items[Index]
  else
    Result := nil;
end;

function TDropItemsMgr.GetItemListIndexByY(Y:Word):Integer;
begin
  PointSearch(65535, Y, Result);
end;

function TDropItemsMgr.IDCompare(ID1, ID2:Integer):Integer;
begin
  Result := ID1 - ID2;
end;

function TDropItemsMgr.IDSearch(ID:Integer; var Index:Integer):Boolean;
var
  L, H, I, C:Integer;
begin
  Result := False;
  L := 0;
  H := FSortIDItemList.Count - 1;
  while L <= H do begin
    I := L + (H - L) shr 1;
    C := IDCompare(pTDropItem(FSortIDItemList[I]).id, ID);
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        Result := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

function TDropItemsMgr.PointCompare(X1, Y1, X2, Y2:Word):Integer;
begin
  Result := Integer((MakeLong(X1, Y1)) - MakeLong(X2, Y2));
end;

function TDropItemsMgr.PointSearch(X, Y:Word; var Index:Integer):Boolean;
var
  L, H, I, C:Integer;
  PointDropItemList:TPointDropItemList;
begin
  Result := False;
  L := 0;
  H := FPointList.Count - 1;
  while L <= H do begin
    I := L + (H - L) shr 1;
    PointDropItemList := TPointDropItemList(FPointList[I]);
    C := PointCompare(Cutecode_T(PointDropItemList.FPointX), Cutecode_T(PointDropItemList.FPointY), X, Y);
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        Result := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

function TDropItemsMgr.AddDropItem(ID:Integer; nX, nY:Word; DBName:string; EffectIndex:Integer; var IsNew:Boolean):pTDropItem;
var
  Index:Integer;
  PointDropItemList:TPointDropItemList;

  DropItemEffect:PDropItemEffect;
begin
  //Result := nil; //HZQ 20230525 以下每个路径均有对Result的赋值
  if IDSearch(ID, Index) then begin
    Result := FSortIDItemList.Items[Index];
    Result.DBName := DBName;
    IsNew := False;
  end else begin
    IsNew := True;
    if FNoUseItemList.Count > 0 then begin
      Result := FNoUseItemList.Items[0];
      FNoUseItemList.Delete(0);
    end else begin
      New(Result);
    end;

    FillChar(Result^, SizeOf(TDropItem), 0);

    Result.id := ID;
    Result.X := Makecode_T(nX);
    Result.Y := Makecode_T(nY);
    Result.DBName := DBName;
    Result.Visible := True;

    if PlugInEnabled then
      Result.ShowItem := g_ConfigDlg.GetShowItem(DBName);

    FSortIDItemList.Insert(Index, Result);

    if PointSearch(nX, nY, Index) then begin
      PointDropItemList := FPointList.Items[Index];

      DropItemEffect := nil;
      if EffectIndex > 0 then
        DropItemEffect := g_DropItemEffectList.Get(EffectIndex);

      if DropItemEffect <> nil then begin
        Result.ItemEffect := DropItemEffect^;
        Result.ItemEffectFrame := DropItemEffect.StartIndex;
        Result.ItemEffectTick := MyGetTickCount;

        Result.FlashTime := MyGetTickCount;
        Result.ShowFlash := False;
        Result.FlashStepTime := MyGetTickCount;
        Result.FlashStep := 0;
      end else begin
        Result.ItemEffect.FileIndex := -1;
      end;

      if (DropItemEffect <> nil) and (PointDropItemList.FList.Count > 0) then
        PointDropItemList.FList.Insert(0, Result)
      else
        PointDropItemList.FList.Add(Result);
    end else begin
      PointDropItemList := TPointDropItemList.Create(Self, Makecode_T(nX), Makecode_T(nY));
      FPointList.Insert(Index, PointDropItemList);

      DropItemEffect := nil;
      if EffectIndex > 0 then
        DropItemEffect := g_DropItemEffectList.Get(EffectIndex);

      if DropItemEffect <> nil then begin
        Result.ItemEffect := DropItemEffect^;
        Result.ItemEffectFrame := DropItemEffect.StartIndex;
        Result.ItemEffectTick := MyGetTickCount;

        Result.FlashTime := MyGetTickCount;
        Result.ShowFlash := False;
        Result.FlashStepTime := MyGetTickCount;
        Result.FlashStep := 0;
      end else begin
        Result.ItemEffect.FileIndex := -1;
      end;

      if (DropItemEffect <> nil) and (PointDropItemList.FList.Count > 0) then
        PointDropItemList.FList.Insert(0, Result)
      else
        PointDropItemList.FList.Add(Result);
    end;
  end;
end;

function TDropItemsMgr.DelDropItem(ID:Integer; var X, Y:Word):pTDropItem;
var
  I, Index:Integer;
  PointDropItemList:TPointDropItemList;
  //IsFound: Boolean;
begin
  Result := nil;
  if IDSearch(ID, Index) then begin
    Result := FSortIDItemList.Items[Index];
    FSortIDItemList.Delete(Index);
    Result.Visible := False;

    X := Cutecode_T(Result.X);
    Y := Cutecode_T(Result.Y);

    if PointSearch(Cutecode_T(Result.X), Cutecode_T(Result.Y), Index) then begin
      PointDropItemList := FPointList.Items[Index];

      //IsFound := False;
      for I := 0 to PointDropItemList.Count - 1 do begin
        if PointDropItemList.Items[I].ID = Result.ID then begin
          //IsFound := True;
          PointDropItemList.FList.Delete(I);
          Break;
        end;
      end;

      if PointDropItemList.Count = 0 then begin
        PointDropItemList.Free;
        FPointList.Delete(Index);
      end;

      {
      if not IsFound then
      begin
        OutputDebugString('aaa');
      end;
      }
    end else begin
      // OutputDebugString('aaa');
    end;

    FNoUseItemList.Add(Result);
  end;
end;

procedure TDropItemsMgr.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TDropItemsMgr.UnLock;
begin
  LeaveCriticalSection(FCS);
end;

procedure TDropItemsMgr.RefreshDrawList(ResetShowItem:Boolean = False);
var
  I:Integer;
  PointItemList:TPointDropItemList;
begin
  for I := 0 to FPointList.Count - 1 do begin
    PointItemList := FPointList.Items[I];
    PointItemList.RefreshDrawList(ResetShowItem);
  end;
end;

end.
