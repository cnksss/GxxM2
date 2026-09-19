unit ItemEvent;

interface

uses
  Windows, Classes, SysUtils, SyncObjs, ObjGame, Grobal2, SDK, M2Definition;

type
  TItemObject = class(TGameObject)
    m_wLooks: Word;
    m_wAniCount: Word;
    m_btReserved: Byte;
      // 队友能不能捡 0队友可以捡  1队友不能捡
    m_btColor: Byte;
    m_nCount: Integer;
    m_OfBaseObject: TObject;                                                                        // 哪个能捡
    m_DropBaseObject: TObject;                                                                      // 哪个掉的
    m_dwCanPickUpTick: LongWord;
    m_dwFloorItemCanPickUpTime: LongWord;
    m_UserItem: TUserItem;
    m_sName: string;
    m_sDBName: string;
    m_PEnvir: TObject;
    m_boGhost: Boolean;                                                                             // 0x2FC
    m_boDieDrop: Boolean;                                                                           // 是否死亡掉落
    m_boHumDrop: Boolean;                                                                           // 是否人物/英雄掉落
    m_boNpcThrowItem: Boolean;
      // Npc命令 ThrowItem搞的物品 2020-04-06 00:29:33
    m_dwGhostTick: LongWord;                                                                        // 0x300
    m_dwRunTick: LongWord;                                                                          // 0x300
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Run();
    procedure MakeGhost;
  end;

  TItemManager = class
    m_ItemList: TGList;
    m_FreeItemList: TGList;
    m_nProcItemIDx: Integer;
  private
    function GetItemCount: Integer;
  public
    constructor Create();
    destructor Destroy; override;

    procedure Run();
    procedure AddItem(ItemObject: TItemObject);
    function FindItem(Envir: TObject; ItemObject: TItemObject): TItemObject; overload;
    function FindItem(Envir: TObject; nX, nY: Integer): TItemObject; overload;
    function FindItem(Envir: TObject; nX, nY: Integer; ItemObject: TItemObject): TItemObject; overload;
    function FindItem(Envir: TObject; nX, nY, nRange: Integer; List: TList): Integer; overload;
    property ItemCount: Integer read GetItemCount;
  end;

implementation

uses
  ObjBase, Envir, M2Share;

constructor TItemObject.Create();
begin
  inherited;
  m_ObjGame := Obj_Item;
  m_sName := '';
  m_sDBName := '';
  m_wLooks := 0;
  m_wAniCount := 0;
  m_btReserved := 0;
  m_nCount := 0;
  m_OfBaseObject := nil;
  m_DropBaseObject := nil;
  m_dwCanPickUpTick := 0;

  m_PEnvir := nil;
  m_boGhost := False;
  m_dwGhostTick := 0;
  m_dwRunTick := MyGetTickCount;
  m_btColor := 255;
  FillChar(m_UserItem, SizeOf(TUserItem), #0);
  m_dwFloorItemCanPickUpTime := g_Config.dwFloorItemCanPickUpTime;
end;

destructor TItemObject.Destroy;
begin
  if (m_PEnvir <> nil) then
  begin
    TEnvirnoment(m_PEnvir).DeleteFromMap(m_nMapX, m_nMapY, Self);
    m_PEnvir := nil;
  end;
  inherited;
end;

procedure TItemObject.Run();
var
  Envir: TEnvirnoment;
  StdItem: pTStdItem;
begin
  if not m_boGhost then
  begin
    if ((MyGetTickCount - m_dwAddTime) > g_Config.dwClearDropOnFloorItemTime {60 * 60 * 1000}) then
    begin                                                                                           // 删除到期装备
      m_boGhost := True;
      m_dwGhostTick := MyGetTickCount;
    end;

    { 副本地图 -- 清除地面物品 chongchong 2013-09-14 }
    Envir := TEnvirnoment(m_PEnvir);
    if Envir.m_boFB and (not Envir.m_boFBCreate) then
    begin
      m_boGhost := True;
      m_dwGhostTick := MyGetTickCount;
    end;
  end;

  if not m_boGhost then
  begin
    if (m_OfBaseObject <> nil) or (m_DropBaseObject <> nil) then
    begin
      if (MyGetTickCount - m_dwCanPickUpTick) > m_dwFloorItemCanPickUpTime then
      begin
        // g_Config.dwFloorItemCanPickUpTime
        m_OfBaseObject := nil;
        m_DropBaseObject := nil;
      end;
      if TBaseObject(m_OfBaseObject) <> nil then
      begin
        if TBaseObject(m_OfBaseObject).m_boGhost then
          m_OfBaseObject := nil;
      end;
      if TBaseObject(m_DropBaseObject) <> nil then
      begin
        if TBaseObject(m_DropBaseObject).m_boGhost then
          m_DropBaseObject := nil;
      end;
    end;
  end
  else
  begin
    if (m_PEnvir <> nil) then
    begin
      if not TEnvirnoment(m_PEnvir).DeleteFromMap(m_nMapX, m_nMapY, Self) then
      begin
      end;

      StdItem := UserEngine.GetStdItem(m_UserItem.wIndex);
      if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
      begin
        AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, latNone, TEnvirnoment(m_PEnvir).sMapName, m_nMapX, m_nMapY, StdItem.Name,
          m_UserItem.MakeIndex, '0', '0', 0, 0, '到时清理');
      end;

      m_PEnvir := nil;
    end;
  end;
end;

procedure TItemObject.MakeGhost;
begin
  m_boGhost := True;
  m_dwGhostTick := MyGetTickCount;
  // m_PEnvir := nil;
end;

constructor TItemManager.Create();
begin
  m_ItemList := TGList.Create;
  m_FreeItemList := TGList.Create;
  m_nProcItemIDx := 0;
end;

destructor TItemManager.Destroy;
var
  I: Integer;
begin
  for I := 0 to m_ItemList.Count - 1 do
  begin
    TItemObject(m_ItemList.Items[I]).Free;
  end;
  m_ItemList.Free;

  for I := 0 to m_FreeItemList.Count - 1 do
  begin
    TItemObject(m_FreeItemList.Items[I]).Free;
  end;
  m_FreeItemList.Free;
  inherited;
end;

function TItemManager.GetItemCount: Integer;
begin
  // m_ItemList.Lock;
  // try
  Result := m_ItemList.Count;
  // finally
  // m_ItemList.UnLock;
  // end;
end;

procedure TItemManager.AddItem(ItemObject: TItemObject);
begin
  // m_ItemList.Lock;
  // try
  m_ItemList.Add(ItemObject);
  // finally
  // m_ItemList.UnLock;
  // end;
end;

function TItemManager.FindItem(Envir: TObject; ItemObject: TItemObject): TItemObject;
var
  I: Integer;
begin
  Result := nil;
  // m_ItemList.Lock;
  // try
  for I := 0 to m_ItemList.Count - 1 do
  begin
    if (not TItemObject(m_ItemList.Items[I]).m_boGhost) and (TItemObject(m_ItemList.Items[I]).m_PEnvir = Envir) and (TItemObject(m_ItemList.Items
      [I]) = ItemObject) then
    begin
      Result := TItemObject(m_ItemList.Items[I]);
      Break;
    end;
  end;
  // finally
  // m_ItemList.UnLock;
  // end;
end;

function TItemManager.FindItem(Envir: TObject; nX, nY: Integer): TItemObject;
var
  I: Integer;
  ItemObject: TItemObject;
begin
  Result := nil;
 // m_ItemList.Lock;
 // try
  for I := 0 to m_ItemList.Count - 1 do
  begin
    ItemObject := TItemObject(m_ItemList.Items[I]);
    if (not ItemObject.m_boGhost) and (ItemObject.m_PEnvir = Envir) and (ItemObject.m_nMapX = nX) and (ItemObject.m_nMapY = nY)
      then
    begin
      Result := ItemObject;
      Break;
    end;
  end;
 // finally
 // m_ItemList.UnLock;
 // end;
end;

function TItemManager.FindItem(Envir: TObject; nX, nY: Integer; ItemObject: TItemObject): TItemObject;
var
  I: Integer;
  AItemObject: TItemObject;
begin
  Result := nil;
 // m_ItemList.Lock;
 // try
  for I := 0 to m_ItemList.Count - 1 do
  begin
    AItemObject := TItemObject(m_ItemList.Items[I]);
    if (not AItemObject.m_boGhost) and (AItemObject.m_PEnvir = Envir) and (AItemObject = ItemObject) and (ItemObject.m_nMapX = nX)
      and (ItemObject.m_nMapY = nY) then
    begin
      Result := ItemObject;
      Break;
    end;
  end;
  {finally
    m_ItemList.UnLock;
  end;}
end;

function TItemManager.FindItem(Envir: TObject; nX, nY, nRange: Integer; List: TList): Integer;
var
  I, nCount: Integer;
  ItemObject: TItemObject;
begin
  nCount := 0;
  // m_ItemList.Lock;
 // try
  for I := 0 to m_ItemList.Count - 1 do
  begin
    ItemObject := TItemObject(m_ItemList.Items[I]);
    if (not ItemObject.m_boGhost) and (ItemObject.m_PEnvir = Envir) and (abs(ItemObject.m_nMapX - nX) <= nRange) and (abs(ItemObject.m_nMapY
      - nY) <= nRange) then
    begin
      Inc(nCount);
      if List <> nil then
        List.Add(ItemObject);
    end;
  end;
  Result := nCount;
  // finally
  // m_ItemList.UnLock;
  // end;
end;

procedure TItemManager.Run();
var
  I, nIdx: Integer;
  ItemObject: TItemObject;
  dwCheckTime: LongWord;
  boCheckTimeLimit: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TItemManager.Run';
begin
  boCheckTimeLimit := False;
  dwCheckTime := MyGetTickCount();
  nIdx := m_nProcItemIDx;
  try
    // m_ItemList.Lock;
    // try
    while True do
    begin
      if m_ItemList.Count <= nIdx then
        Break;

      ItemObject := TItemObject(m_ItemList.Items[nIdx]);
      if (not ItemObject.m_boGhost) and ((MyGetTickCount - ItemObject.m_dwRunTick) > 250) then
      begin
        ItemObject.m_dwRunTick := MyGetTickCount();
        ItemObject.Run();
      end;

      if ItemObject.m_boGhost then
      begin
        m_FreeItemList.Add(ItemObject);
        m_ItemList.Delete(nIdx);
        Continue;
      end;

      Inc(nIdx);
      if (MyGetTickCount - dwCheckTime) > 5 then
      begin
        boCheckTimeLimit := True;
        m_nProcItemIDx := nIdx;
        Break;
      end;
    end;                                                                                            // while True do begin
    {finally
      m_ItemList.UnLock;
    end;}
    if not boCheckTimeLimit then
      m_nProcItemIDx := 0;
  except
    MainOutMessage(sExceptionMsg);
  end;
  {
  m_ItemList.Lock;
  try
    for I := m_ItemList.Count - 1 downto 0 do begin
      ItemObject := TItemObject(m_ItemList.Items[I]);
      if (not ItemObject.m_boGhost) and ((MyGetTickCount - ItemObject.m_dwRunTick) > 250) then begin
        ItemObject.m_dwRunTick := MyGetTickCount();
        ItemObject.Run();
      end;
      if ItemObject.m_boGhost then begin
        m_FreeItemList.Add(ItemObject);
        m_ItemList.Delete(I);
      end;
    end;
  finally
    m_ItemList.UnLock;
  end; }

 { m_FreeItemList.Lock;
  try }
  for I := m_FreeItemList.Count - 1 downto 0 do
  begin
    ItemObject := TItemObject(m_FreeItemList.Items[I]);
    if (MyGetTickCount - ItemObject.m_dwGhostTick) > 5 * 60 * 1000 then
    begin
      m_FreeItemList.Delete(I);
      ItemObject.Free;
      // break;
    end;
  end;
  {finally
    m_FreeItemList.UnLock;
  end;  }
end;

end.

