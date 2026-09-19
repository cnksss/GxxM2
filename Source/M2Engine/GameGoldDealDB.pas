unit GameGoldDealDB;
{------------------------------------------------------------------------------}
{-----------------------------------元宝交易-----------------------------------}
{------------------------------------------------------------------------------}

interface

uses
  Windows, Classes, SysUtils, Forms, MudUtil, Grobal2;

type
  TDealItems = array[0..8] of TUserItem;

  pTDealItems = ^TDealItems;

  TGameGoldDealData = packed record
    Dealing: TGameGoldDeal;
    LastDeal: TGameGoldDeal;
    DealItems: TDealItems;
  end;

  pTGameGoldDealData = ^TGameGoldDealData;

  TDealingInfo = record
    OffSet: Integer;
    Dealing: TGameGoldDeal;
    DealItems: TDealItems;
  end;

  pTDealingInfo = ^TDealingInfo;

  TLastDealInfo = record
    OffSet: Integer;
    LastDeal: TGameGoldDeal;
  end;

  pTLastDealInfo = ^TLastDealInfo;

  TGameGoldDealDB = class
    m_Header: TItemCount;
    m_SellList: TQuickList;
    m_BuyList: TQuickList;

    m_LastSellList: TQuickList;
    m_LastBuyList: TQuickList;

    m_FileStream: TFileStream;

    m_DeleteList: TList;
  private
    CS: TRTLCriticalSection;
    procedure UnLoadQuickList;
    procedure LoadQuickList();
  public
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    procedure LoadFormFile(const FileName: string);

    function BuyerCancel(const CharName: string): Boolean;                                          // 买家取消
    function SellerCancel(const CharName: string; GameGold: Integer; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Boolean;
      // 卖家取消

    function Sell(Dealing: pTGameGoldDeal; DealItems: pTDealItems): Integer;
    function Buy(GameGold: Integer; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Integer;

    function QueryDeal(const CharName: string; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Boolean; // 查询购买物品
    function QuerySell(const CharName: string; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Boolean; // 查询出售物品
    function QueryDealLog(const CharName: string; Dealing: pTGameGoldDeal): Boolean;
  end;

implementation

constructor TGameGoldDealDB.Create();
begin
  InitializeCriticalSection(CS);
  m_SellList := TQuickList.Create;
  m_BuyList := TQuickList.Create;
  m_LastSellList := TQuickList.Create;
  m_LastBuyList := TQuickList.Create;
  m_DeleteList := TList.Create;
  m_FileStream := nil;
end;

destructor TGameGoldDealDB.Destroy;
begin
  UnLoadQuickList;
  m_SellList.Free;
  m_BuyList.Free;
  m_LastSellList.Free;
  m_LastBuyList.Free;
  m_DeleteList.Free;
  if m_FileStream <> nil then
    m_FileStream.Free;
  DeleteCriticalSection(CS);
  inherited;
end;

procedure TGameGoldDealDB.Lock();
begin
  EnterCriticalSection(CS);
end;

procedure TGameGoldDealDB.UnLock();
begin
  LeaveCriticalSection(CS);
end;

procedure TGameGoldDealDB.LoadFormFile(const FileName: string);
begin
  if FileExists(FileName) then
  begin
    if m_FileStream = nil then
      m_FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone);

    m_FileStream.Seek(0, soBeginning);
    m_FileStream.Read(m_Header, SizeOf(TItemCount));
    if m_Header = 0 then
    begin
      m_FileStream.Seek(0, soBeginning);
      m_FileStream.Write(m_Header, SizeOf(TItemCount));
      m_FileStream.Size := SizeOf(TItemCount);
    end;
  end
  else
  begin
    if m_FileStream = nil then
      m_FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone or fmCreate);
    m_Header := 0;
    m_FileStream.Write(m_Header, SizeOf(TItemCount));
  end;
  LoadQuickList();
end;

function TGameGoldDealDB.Sell(Dealing: pTGameGoldDeal; DealItems: pTDealItems): Integer;
var
  nIndex: Integer;
  DealData: TGameGoldDealData;
  DealingInfo: pTDealingInfo;
begin
  nIndex := m_SellList.GetIndex(Dealing.SellChrName);
  if (nIndex >= 0) then
  begin
    DealingInfo := pTDealingInfo(m_SellList.Objects[nIndex]);
    if DealingInfo.Dealing.DealState = s_None then
    begin
      if (m_BuyList.GetIndex(Dealing.BuyChrName) < 0) and (m_BuyList.GetIndex(Dealing.SellChrName) < 0) then
      begin
        DealingInfo.Dealing := Dealing^;
        DealingInfo.DealItems := DealItems^;

        m_FileStream.Position := DealingInfo.OffSet;
        m_FileStream.Write(DealingInfo.Dealing, SizeOf(TGameGoldDeal));
        m_FileStream.Seek(SizeOf(TGameGoldDeal), soCurrent);
        m_FileStream.Write(DealingInfo.DealItems, SizeOf(TDealItems));

        m_BuyList.AddRecord(DealingInfo.Dealing.BuyChrName, NativeInt(DealingInfo));
        Result := 1;
      end
      else
        Result := -2;
    end
    else if DealingInfo.Dealing.DealState = s_Expired then
    begin
      Result := -3;
    end
    else
    begin
      Result := -1;
    end;
  end
  else
  begin
    if (m_BuyList.GetIndex(Dealing.BuyChrName) < 0) and (m_BuyList.GetIndex(Dealing.SellChrName) < 0) then
    begin
      New(DealingInfo);
      DealingInfo.Dealing := Dealing^;
      DealingInfo.DealItems := DealItems^;
      if m_DeleteList.Count > 0 then
      begin
        DealingInfo.OffSet := NativeInt(m_DeleteList.Items[0]);
        m_DeleteList.Delete(0);
      end
      else
      begin
        m_Header := m_Header + 1;
        m_FileStream.Seek(0, soBeginning);
        m_FileStream.Write(m_Header, SizeOf(TItemCount));
        DealingInfo.OffSet := m_FileStream.Size;
      end;
      DealData.Dealing := Dealing^;
      DealData.DealItems := DealItems^;
      FillChar(DealData.LastDeal, SizeOf(TGameGoldDeal), #0);

      m_FileStream.Position := DealingInfo.OffSet;
      m_FileStream.Write(DealData, SizeOf(TGameGoldDealData));

      m_SellList.AddRecord(Dealing.SellChrName, NativeInt(DealingInfo));
      //nIndex := m_BuyList.GetIndex(Dealing.BuyChrName);
      m_BuyList.AddRecord(Dealing.BuyChrName, NativeInt(DealingInfo));

      Result := 1;
    end
    else
      Result := -2;
  end;
end;

function TGameGoldDealDB.Buy(GameGold: Integer; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Integer;
var
  nIndex: Integer;
  nIndex1: Integer;
  DealingInfo: pTDealingInfo;
  LastDealInfo: pTLastDealInfo;
begin
  nIndex := m_SellList.GetIndex(Dealing.SellChrName);
  if (nIndex >= 0) then
  begin
    DealingInfo := pTDealingInfo(m_SellList.Objects[nIndex]);

    if not (DealingInfo.Dealing.DealState in [s_None, s_Expired]) then
    begin
      if Round(Now - DealingInfo.Dealing.SellDateTime) > 3 then
      begin
        DealingInfo.Dealing.DealState := s_Expired;
        m_FileStream.Position := DealingInfo.OffSet;
        m_FileStream.Write(DealingInfo.Dealing, SizeOf(Byte));
      end;
    end;

    if DealingInfo.Dealing.DealState = s_Normal then
    begin                                                                                           // 检测是否过期或已交易完成
      nIndex1 := m_BuyList.GetIndex(Dealing.BuyChrName);
      if (nIndex1 >= 0) and (DealingInfo = pTDealingInfo(m_BuyList.Objects[nIndex1])) then
      begin
        if GameGold >= DealingInfo.Dealing.GameGold then
        begin                                                                                       // 检测元宝是否足够
          m_BuyList.Delete(nIndex1);
          // m_BuyList.SortString(0, m_BuyList.Count - 1);

          Dealing^ := DealingInfo.Dealing;
          DealItems^ := DealingInfo.DealItems;
          DealingInfo.Dealing.DealState := s_None;

          m_FileStream.Position := DealingInfo.OffSet;
          m_FileStream.Write(DealingInfo.Dealing, SizeOf(Byte));

          DealingInfo.Dealing.DealState := s_Succeed;
          m_FileStream.Position := DealingInfo.OffSet + SizeOf(TGameGoldDeal);
          m_FileStream.Write(DealingInfo.Dealing, SizeOf(TGameGoldDeal));

          nIndex := m_LastSellList.GetIndex(DealingInfo.Dealing.SellChrName);
          if nIndex >= 0 then
          begin
            LastDealInfo := pTLastDealInfo(m_LastSellList.Objects[nIndex]);
            LastDealInfo.OffSet := DealingInfo.OffSet;
            LastDealInfo.LastDeal := DealingInfo.Dealing;
          end
          else
          begin
            New(LastDealInfo);
            LastDealInfo.OffSet := DealingInfo.OffSet;
            LastDealInfo.LastDeal := DealingInfo.Dealing;
            m_LastSellList.AddRecord(DealingInfo.Dealing.SellChrName, NativeInt(LastDealInfo));
          end;

          nIndex := m_LastBuyList.GetIndex(DealingInfo.Dealing.BuyChrName);
          if nIndex >= 0 then
          begin
            LastDealInfo := pTLastDealInfo(m_LastBuyList.Objects[nIndex]);
            LastDealInfo.OffSet := DealingInfo.OffSet;
            LastDealInfo.LastDeal := DealingInfo.Dealing;
          end
          else
          begin
            New(LastDealInfo);
            LastDealInfo.OffSet := DealingInfo.OffSet;
            LastDealInfo.LastDeal := DealingInfo.Dealing;
            m_LastBuyList.AddRecord(DealingInfo.Dealing.BuyChrName, NativeInt(LastDealInfo));
          end;

          DealingInfo.Dealing.DealState := s_None;

          Result := 1;
        end
        else
          Result := -4;
      end
      else
        Result := -3;
    end
    else if DealingInfo.Dealing.DealState = s_Expired then
    begin
      Result := -2;
    end
    else
    begin
      Result := -1;
    end;
  end
  else
    Result := -1;
end;

function TGameGoldDealDB.QueryDeal(const CharName: string; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Boolean;
  // 查询购买物品
var
  nIndex: Integer;
  DealingInfo: pTDealingInfo;
begin
  Result := False;
  nIndex := m_BuyList.GetIndex(CharName);
  if (nIndex >= 0) then
  begin
    DealingInfo := pTDealingInfo(m_BuyList.Objects[nIndex]);
    if not (DealingInfo.Dealing.DealState in [s_None, s_Expired]) then
    begin
      if Round(Now - DealingInfo.Dealing.SellDateTime) > 3 then
      begin
        DealingInfo.Dealing.DealState := s_Expired;
        m_FileStream.Position := DealingInfo.OffSet;
        m_FileStream.Write(DealingInfo.Dealing, SizeOf(Byte));
      end;
    end;
    if DealingInfo.Dealing.DealState in [s_Normal, s_Expired] then
    begin
      Dealing^ := DealingInfo.Dealing;
      DealItems^ := DealingInfo.DealItems;
      Result := True;
    end;
  end;
end;

function TGameGoldDealDB.QuerySell(const CharName: string; Dealing: pTGameGoldDeal; DealItems: pTDealItems): Boolean;
  // 查询出售物品
var
  nIndex: Integer;
  DealingInfo: pTDealingInfo;
begin
  Result := False;
  nIndex := m_SellList.GetIndex(CharName);
  if (nIndex >= 0) then
  begin
    DealingInfo := pTDealingInfo(m_SellList.Objects[nIndex]);
    if not (DealingInfo.Dealing.DealState in [s_None, s_Expired]) then
    begin
      if Round(Now - DealingInfo.Dealing.SellDateTime) > 3 then
      begin
        DealingInfo.Dealing.DealState := s_Expired;
        m_FileStream.Position := DealingInfo.OffSet;
        m_FileStream.Write(DealingInfo.Dealing, SizeOf(Byte));
      end;
    end;
    if DealingInfo.Dealing.DealState in [s_Normal, s_Cancel, s_Expired] then
    begin
      Dealing^ := DealingInfo.Dealing;
      DealItems^ := DealingInfo.DealItems;
      Result := True;
    end;
  end;
end;

function TGameGoldDealDB.BuyerCancel(const CharName: string): Boolean;
var
  nIndex: Integer;
  DealingInfo: pTDealingInfo;
begin
  Result := False;
  nIndex := m_BuyList.GetIndex(CharName);
  if (nIndex >= 0) and (nIndex < m_BuyList.Count) then
  begin
    DealingInfo := pTDealingInfo(m_BuyList.Objects[nIndex]);
    if not (DealingInfo.Dealing.DealState in [s_None, s_Cancel, s_Expired]) then
    begin
      DealingInfo.Dealing.DealState := s_Cancel;
      m_FileStream.Position := DealingInfo.OffSet;
      m_FileStream.Write(DealingInfo.Dealing, SizeOf(Byte));
    end;
    m_BuyList.Delete(nIndex);
    Result := True;
  end;
end;

function TGameGoldDealDB.SellerCancel(const CharName: string; GameGold: Integer; Dealing: pTGameGoldDeal; DealItems: pTDealItems):
  Boolean;
var
  nIndex: Integer;
  DealingInfo: pTDealingInfo;
begin
  Result := False;
  nIndex := m_SellList.GetIndex(CharName);
  if (nIndex >= 0) and (nIndex < m_SellList.Count) then
  begin
    DealingInfo := pTDealingInfo(m_SellList.Objects[nIndex]);
    if DealingInfo.Dealing.DealState <> s_None then
    begin

      if ((DealingInfo.Dealing.DealState = s_Expired) and (GameGold > 0)) or (DealingInfo.Dealing.DealState <> s_Expired) then
      begin
        Dealing^ := DealingInfo.Dealing;
        DealItems^ := DealingInfo.DealItems;

        DealingInfo.Dealing.DealState := s_None;

        m_FileStream.Position := DealingInfo.OffSet;
        m_FileStream.Write(DealingInfo.Dealing, SizeOf(Byte));

        nIndex := m_BuyList.GetIndex(Dealing.BuyChrName);
        if (nIndex >= 0) then
        begin
          m_BuyList.Delete(nIndex);
        end;
        Result := True;
      end;
    end;
  end;
end;

function TGameGoldDealDB.QueryDealLog(const CharName: string; Dealing: pTGameGoldDeal): Boolean;
var
  nIndex: Integer;
  LastDealInfo: pTLastDealInfo;
begin
  Result := False;
  nIndex := m_LastBuyList.GetIndex(CharName);
  if nIndex >= 0 then
  begin
    LastDealInfo := pTLastDealInfo(m_LastBuyList.Objects[nIndex]);
    Dealing^ := LastDealInfo.LastDeal;
    Result := True;
  end
  else
  begin
    nIndex := m_LastSellList.GetIndex(CharName);
    if nIndex >= 0 then
    begin
      LastDealInfo := pTLastDealInfo(m_LastSellList.Objects[nIndex]);
      Dealing^ := LastDealInfo.LastDeal;
      Result := True;
    end;
  end;
end;

procedure TGameGoldDealDB.UnLoadQuickList;
var
  I: Integer;
begin
  for I := 0 to m_SellList.Count - 1 do
  begin
    Dispose(pTDealingInfo(m_SellList.Objects[I]));
  end;

  for I := 0 to m_LastSellList.Count - 1 do
  begin
    Dispose(pTLastDealInfo(m_LastSellList.Objects[I]));
  end;

  for I := 0 to m_LastBuyList.Count - 1 do
  begin
    Dispose(pTLastDealInfo(m_LastBuyList.Objects[I]));
  end;

  m_SellList.Clear;
  m_BuyList.Clear;
  m_LastSellList.Clear;
  m_LastBuyList.Clear;
  m_DeleteList.Clear;
end;

procedure TGameGoldDealDB.LoadQuickList();
var
  I, nIndex: Integer;
  DBHeader: TItemCount;
  DBRecord: TGameGoldDealData;
  DealingInfo: pTDealingInfo;
  LastDealInfo: pTLastDealInfo;
begin
  UnLoadQuickList;
  nIndex := 0;
  m_FileStream.Seek(0, soBeginning);
  if m_FileStream.Read(DBHeader, SizeOf(TItemCount)) = SizeOf(TItemCount) then
  begin
    for I := 0 to DBHeader - 1 do
    begin
      if m_FileStream.Read(DBRecord, SizeOf(TGameGoldDealData)) <> SizeOf(TGameGoldDealData) then
      begin
        break;
      end;

      if (DBRecord.Dealing.DealState = s_None) and (DBRecord.LastDeal.DealState = s_None) then
      begin
        m_DeleteList.Add(Pointer(m_FileStream.Position - SizeOf(TGameGoldDealData)));
      end
      else
      begin
        if (DBRecord.Dealing.DealState <> s_None) and (DBRecord.Dealing.DealState <> s_Succeed) and (DBRecord.Dealing.BuyChrName
          <> '') and (DBRecord.Dealing.SellChrName <> '') then
        begin
          New(DealingInfo);
          DealingInfo.OffSet := m_FileStream.Position - SizeOf(TGameGoldDealData);
          DealingInfo.Dealing := DBRecord.Dealing;
          Move(DBRecord.DealItems, DealingInfo.DealItems, SizeOf(TUserItem) * 9);
          m_SellList.AddObject(DBRecord.Dealing.SellChrName, TObject(DealingInfo));

          if DBRecord.Dealing.DealState <> s_Cancel then
            m_BuyList.AddObject(DBRecord.Dealing.BuyChrName, TObject(DealingInfo));
        end;

        if (DBRecord.LastDeal.DealState = s_Succeed) and (DBRecord.LastDeal.BuyChrName <> '') and (DBRecord.LastDeal.SellChrName
          <> '') then
        begin
          New(LastDealInfo);
          LastDealInfo.OffSet := m_FileStream.Position - SizeOf(TGameGoldDealData);
          LastDealInfo.LastDeal := DBRecord.LastDeal;
          m_LastSellList.AddObject(DBRecord.LastDeal.SellChrName, TObject(LastDealInfo));

          nIndex := m_LastBuyList.GetIndex(DBRecord.LastDeal.BuyChrName);
          if nIndex >= 0 then
          begin
            LastDealInfo := pTLastDealInfo(m_LastBuyList.Objects[nIndex]);
            if LastDealInfo.LastDeal.SellDateTime < DBRecord.LastDeal.SellDateTime then
            begin
              LastDealInfo.OffSet := m_FileStream.Position - SizeOf(TGameGoldDealData);
              LastDealInfo.LastDeal := DBRecord.LastDeal;
            end
            else
              Continue;
          end
          else
          begin
            New(LastDealInfo);
            LastDealInfo.OffSet := m_FileStream.Position - SizeOf(TGameGoldDealData);
            LastDealInfo.LastDeal := DBRecord.LastDeal;
            m_LastBuyList.AddRecord(DBRecord.LastDeal.BuyChrName, NativeInt(LastDealInfo));
          end;
        end;
      end;

      if nIndex mod 100 = 0 then
        Application.ProcessMessages;
      if Application.Terminated then
      begin
        Exit;
      end;
    end;
  end;
  m_SellList.SortString(0, m_SellList.Count - 1);
  m_BuyList.SortString(0, m_BuyList.Count - 1);

  m_LastSellList.SortString(0, m_LastSellList.Count - 1);
  m_LastBuyList.SortString(0, m_LastBuyList.Count - 1);
end;

end.

