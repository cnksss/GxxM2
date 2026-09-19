unit UserShopDB;

interface
uses
  Windows, Classes, SysUtils, Math, Grobal2, MudUtil, Forms;
type
  TQuickNameList = class(TStringList)
  private
  public
    procedure SortString(nMin, nMax: Integer);
    function GetIndex(sName: string): Integer;
    procedure AddRecord(sName: string; P: Pointer);
    function GetChrList(sName: string; var List: TList): Integer;
  end;

  TRecordCount = Integer;

  TUserShop = packed record                                                                         // size =  116
    boDelete: Boolean;
    boBusiness: Boolean;                                                                            // 是否营业
    sAccount: string[ACCOUNTLEN];                                                                   // 30
    sShopName: string[ACTORNAMELEN];                                                                // 店铺名称 14
    sMasterName: string[ACTORNAMELEN];                                                              // 店主名称 14
    dCreateDate: TDateTime;                                                                         // 创建时间      8
    nMaxShopItemCount: Integer;                                                                     // 店铺最高物品数量 4
    nMaxStorageItemCount: Integer;                                                                  // 仓库最高物品数量 4
    nSellItemCount: Integer;                                                                        // 出售物品数量 4
    nCareValue: Integer;                                                                            // 关注度          4
    dLastDate: TDateTime;
    Param: array[0..6] of Integer;                                                                  // 10*4
    nOffset: Integer;
  end;
  pTUserShop = ^TUserShop;

  TUserShopItem = packed record
    boDelete: Boolean;
    boAllowSell: Boolean;                                                                           // 允许出售
    boGetMoney: Boolean;                                                                            // 是否已取款
    btItemType: Byte;                                                                               // 物品类型
    btMoneyType: Byte;                                                                              // 货币类型
    nPrice: Integer;                                                                                // 出售价格
    dCreateDate: TDateTime;                                                                         // 创建时间      8
    UserItem: TUserItem;
    sAccount: string[ACCOUNTLEN];
    sShopName: string[ACTORNAMELEN];                                                                // 店铺名称
    sMasterName: string[ACTORNAMELEN];                                                              // 店主名称
    sBuyName: string[ACTORNAMELEN];                                                                 // 购买人名称   已经出售
    nOffset: Integer;
    UserShop: pTUserShop;
  end;
  pTUserShopItem = ^TUserShopItem;

  TUserShopFile = class
    m_MasterNameList: TQuickList;
    m_UserShopList: TQuickList;
  private
    FRecordCount: TRecordCount;
    FFileName: string;
    FLoadFile: Boolean;
    FileStream: TFileStream;

    DeleteList: TList;

    function GetCount: Integer;
    function GetItem(Index: Integer): Pointer;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadShopList(const AFileName: string);
    function Add(const ShopName, Account, MasterName: string): Boolean;
    function DeleteShop(const ShopName: string): Boolean;
    function ResetUserShopName(const OldShopName, NewShopName: string): Integer;
    function UpDate(UserShop: pTUserShop): Boolean; overload;
    function UpDate(UserShop: pTUserShop; Size: Integer): Boolean; overload;

    function QueryCharName(const MasterName: string): Boolean;
    function QueryShopName(const ShopName: string): Boolean;
    function GetShop(const MasterName: string): pTUserShop;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: Pointer read GetItem;
    property RecordCount: Integer read FRecordCount;
  end;

  TUserShopItemFile = class
    m_ShopItemList: TQuickList;
    m_SellItemList: TQuickNameList;
    m_SelledItemList: TQuickNameList;
    m_ShopNameList: TQuickNameList;
    m_StorageList: TQuickNameList;
    m_CharNameList: TQuickNameList;
  private
    FRecordCount: TRecordCount;
    FFileName: string;
    FLoadFile: Boolean;
    FileStream: TFileStream;

    DeleteList: TList;
    function GetCount: Integer;
    function GetItem(Index: Integer): Pointer;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadShopItemList(const AFileName: string);

    function AddSell(Item: pTUserShopItem): Boolean;
    function Add(Item: pTUserShopItem): Boolean;
    function UpDate(UserShopItem: pTUserShopItem): Boolean; overload;
    function UpDate(UserShopItem: pTUserShopItem; Size: Integer): Boolean; overload;
    function DeleteShop(const CharName: string; const MakeIndex: Integer; UserItem: pTUserItem): Boolean;
    function DeleteStorage(const CharName: string; const MakeIndex: Integer; UserItem: pTUserItem): Boolean;
    function ShopToStorage(const CharName: string; const MakeIndex: Integer): Boolean;
    function StorageToShop(const CharName: string; const MakeIndex: Integer): Boolean;
    function SellItemCount(const CharName: string): Integer;
    function SellIedtemCount(const CharName: string): Integer;
    function StorageItemCount(const CharName: string): Integer;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: Pointer read GetItem;
    property RecordCount: Integer read FRecordCount;
  end;

implementation
uses M2Share;

procedure TQuickNameList.AddRecord(sName: string; P: Pointer);
var
  ChrList: TList;
  nLow, nHigh, nMed, n1C, n20: Integer;
begin
  if Count = 0 then
  begin
    ChrList := TList.Create;
    ChrList.Add(P);
    AddObject(sName, ChrList);
  end
  else
  begin                                                                                             // 0x0045B839
    if Count = 1 then
    begin
      nMed := CompareStr(sName, Self.Strings[0]);
      if nMed > 0 then
      begin
        ChrList := TList.Create;
        ChrList.Add(P);
        AddObject(sName, ChrList);
      end
      else
      begin                                                                                         // 0x0045B89C
        if nMed < 0 then
        begin
          ChrList := TList.Create;
          ChrList.Add(P);
          InsertObject(0, sName, ChrList);
        end
        else
        begin
          ChrList := TList(Self.Objects[0]);
          ChrList.Add(P);
        end;
      end;
    end
    else
    begin                                                                                           // 0x0045B8EF
      nLow := 0;
      nHigh := Self.Count - 1;
      nMed := (nHigh - nLow) div 2 + nLow;
      while (true) do
      begin
        if (nHigh - nLow) = 1 then
        begin
          n20 := CompareStr(sName, Self.Strings[nHigh]);
          if n20 > 0 then
          begin
            ChrList := TList.Create;
            ChrList.Add(P);
            InsertObject(nHigh + 1, sName, ChrList);
            break;
          end
          else
          begin
            if CompareStr(sName, Self.Strings[nHigh]) = 0 then
            begin
              ChrList := TList(Self.Objects[nHigh]);
              ChrList.Add(P);
              break;
            end
            else
            begin                                                                                   // 0x0045B9BB
              n20 := CompareStr(sName, Self.Strings[nLow]);
              if n20 > 0 then
              begin
                ChrList := TList.Create;
                ChrList.Add(P);
                InsertObject(nLow + 1, sName, ChrList);
                break;
              end
              else
              begin
                if n20 < 0 then
                begin
                  ChrList := TList.Create;
                  ChrList.Add(P);
                  InsertObject(nLow, sName, ChrList);
                  break;
                end
                else
                begin
                  ChrList := TList(Self.Objects[n20]);
                  ChrList.Add(P);
                  break;
                end;
              end;
            end;
          end;

        end
        else
        begin                                                                                       // 0x0045BA6A
          n1C := CompareStr(sName, Self.Strings[nMed]);
          if n1C > 0 then
          begin
            nLow := nMed;
            nMed := (nHigh - nLow) div 2 + nLow;
            Continue;
          end;
          if n1C < 0 then
          begin
            nHigh := nMed;
            nMed := (nHigh - nLow) div 2 + nLow;
            Continue;
          end;
          ChrList := TList(Self.Objects[nMed]);
          ChrList.Add(P);
          break;
        end;
      end;
    end;
  end;
end;

procedure TQuickNameList.SortString(nMin, nMax: Integer);
var
  ntMin, ntMax: Integer;
  s18: string;
begin
  if Self.Count > 0 then
    while (True) do
    begin
      ntMin := nMin;
      ntMax := nMax;
      s18 := Self.Strings[(nMin + nMax) shr 1];
      while (True) do
      begin
        while (CompareText(Self.Strings[ntMin], s18) < 0) do
          Inc(ntMin);
        while (CompareText(Self.Strings[ntMax], s18) > 0) do
          Dec(ntMax);
        if ntMin <= ntMax then
        begin
          Self.Exchange(ntMin, ntMax);
          Inc(ntMin);
          Dec(ntMax);
        end;
        if ntMin > ntMax then
          break
      end;
      if nMin < ntMax then SortString(nMin, ntMax);
      nMin := ntMin;
      if ntMin >= nMax then break;
    end;
end;

function TQuickNameList.GetIndex(sName: string): Integer;
var
  nHigh, nLow, nMed, n20, n24: Integer;
begin
  Result := -1;
  if Self.Count = 0 then exit;
  if Self.Count = 1 then
  begin
    if CompareStr(sName, Self.Strings[0]) = 0 then
    begin
      Result := 0;
    end;
  end
  else
  begin                                                                                             // 0x0045BBB7
    nLow := 0;
    nHigh := Self.Count - 1;
    nMed := (nHigh - nLow) div 2 + nLow;
    n24 := -1;
    while (True) do
    begin
      if (nHigh - nLow) = 1 then
      begin
        if CompareStr(sName, Self.Strings[nHigh]) = 0 then n24 := nHigh;
        if CompareStr(sName, Self.Strings[nLow]) = 0 then n24 := nLow;
        break;
      end
      else
      begin
        n20 := CompareStr(sName, Self.Strings[nMed]);
        if n20 > 0 then
        begin
          nLow := nMed;
          nMed := (nHigh - nLow) div 2 + nLow;
          Continue;
        end;
        if n20 < 0 then
        begin
          nHigh := nMed;
          nMed := (nHigh - nLow) div 2 + nLow;
          Continue;
        end;
        n24 := nMed;
        break;
      end;
    end;
    // if n24 <> -1 then List := TList(Self.Objects[n24]);
    Result := n24;
  end;
end;

function TQuickNameList.GetChrList(sName: string;
  var List: TList): Integer;
var
  nHigh, nLow, nMed, n20, n24: Integer;
begin
  Result := -1;
  if Self.Count = 0 then exit;
  if Self.Count = 1 then
  begin
    if CompareStr(sName, Self.Strings[0]) = 0 then
    begin
      List := TList(Self.Objects[0]);
      Result := 0;
    end;
  end
  else
  begin                                                                                             // 0x0045BBB7
    nLow := 0;
    nHigh := Self.Count - 1;
    nMed := (nHigh - nLow) div 2 + nLow;
    n24 := -1;
    while (True) do
    begin
      if (nHigh - nLow) = 1 then
      begin
        if CompareStr(sName, Self.Strings[nHigh]) = 0 then n24 := nHigh;
        if CompareStr(sName, Self.Strings[nLow]) = 0 then n24 := nLow;
        break;
      end
      else
      begin
        n20 := CompareStr(sName, Self.Strings[nMed]);
        if n20 > 0 then
        begin
          nLow := nMed;
          nMed := (nHigh - nLow) div 2 + nLow;
          Continue;
        end;
        if n20 < 0 then
        begin
          nHigh := nMed;
          nMed := (nHigh - nLow) div 2 + nLow;
          Continue;
        end;
        n24 := nMed;
        break;
      end;
    end;
    if n24 <> -1 then List := TList(Self.Objects[n24]);
    Result := n24;
  end;
end;
// ------------------------------------------------------------------------------

constructor TUserShopFile.Create();
begin
  FRecordCount := 0;
  FLoadFile := False;
  FileStream := nil;
  DeleteList := TList.Create;
  m_MasterNameList := TQuickList.Create;
  m_UserShopList := TQuickList.Create;
end;

destructor TUserShopFile.Destroy;
var
  I: Integer;
begin
  for I := 0 to m_UserShopList.Count - 1 do
  begin
    Dispose(pTUserShop(m_UserShopList.Objects[I]));
  end;
  m_UserShopList.Free;
  m_MasterNameList.Free;
  DeleteList.Free;
  if FileStream <> nil then
    FileStream.Free;
end;

function TUserShopFile.GetCount: Integer;
begin
  Result := m_UserShopList.Count;
end;

function TUserShopFile.GetItem(Index: Integer): Pointer;
begin
  Result := pTUserShop(m_UserShopList.Objects[Index]);
end;

function TUserShopFile.QueryCharName(const MasterName: string): Boolean;
var
  nIndex: Integer;
begin
  Result := False;
  nIndex := m_MasterNameList.GetIndex(MasterName);
  if nIndex >= 0 then
    Result := not pTUserShop(m_MasterNameList.Objects[nIndex]).boDelete;
end;

function TUserShopFile.GetShop(const MasterName: string): pTUserShop;
var
  nIndex: Integer;
begin
  Result := nil;
  nIndex := m_MasterNameList.GetIndex(MasterName);
  if (nIndex >= 0) and (not pTUserShop(m_MasterNameList.Objects[nIndex]).boDelete) then
    Result := pTUserShop(m_MasterNameList.Objects[nIndex]);
end;

function TUserShopFile.QueryShopName(const ShopName: string): Boolean;
var
  nIndex: Integer;
begin
  Result := False;
  nIndex := m_UserShopList.GetIndex(ShopName);
  if nIndex >= 0 then
    Result := not pTUserShop(m_UserShopList.Objects[nIndex]).boDelete;
end;

procedure TUserShopFile.LoadShopList(const AFileName: string);
var
  I: Integer;
  UserShop: pTUserShop;
begin
  FLoadFile := True;
  FFileName := AFileName;
  if FileExists(FFileName) then
  begin
    if FileStream = nil then
      FileStream := TFileStream.Create(FFileName, fmOpenReadWrite or fmShareDenyNone);
  end
  else
  begin
    if FileStream = nil then
      FileStream := TFileStream.Create(FFileName, fmOpenReadWrite or fmShareDenyNone or fmCreate);
    FileStream.Write(FRecordCount, SizeOf(TRecordCount));
  end;
  FileStream.Seek(0, soBeginning);
  FileStream.Read(FRecordCount, SizeOf(TRecordCount));

  for I := 0 to FRecordCount - 1 do
  begin
    New(UserShop);
    FillChar(UserShop^, SizeOf(TUserShop), #0);
    UserShop.nOffset := FileStream.Position;
    if FileStream.Read(UserShop^, SizeOf(TUserShop) - SizeOf(Integer)) <> SizeOf(TUserShop) - SizeOf(Integer) then
    begin
      Dispose(UserShop);
      break;
    end;
    if UserShop.boDelete or (UserShop.sShopName = '') then
    begin
      DeleteList.Add(Pointer(UserShop.nOffset));
      Dispose(UserShop);
    end
    else
    begin
      if UserShop.dLastDate = 0.0 then
        UserShop.dLastDate := Now;
      m_MasterNameList.AddObject(UserShop.sMasterName, TObject(UserShop));
      m_UserShopList.AddObject(UserShop.sShopName, TObject(UserShop));
    end;
    if I mod 100 = 0 then
      Application.ProcessMessages;
    if Application.Terminated then
    begin
      Exit;
    end;
  end;
  m_MasterNameList.SortString(0, m_MasterNameList.Count - 1);
  m_UserShopList.SortString(0, m_UserShopList.Count - 1);
end;

function TUserShopFile.DeleteShop(const ShopName: string): Boolean;
var
  sShopMaster: string;
  nIndex1: Integer;
  UserShop: pTUserShop;
begin
  Result := False;
  sShopMaster := '';
  if ShopName = '' then Exit;
  nIndex1 := m_UserShopList.GetIndex(ShopName);
  if nIndex1 >= 0 then
  begin
    UserShop := pTUserShop(m_UserShopList.Objects[nIndex1]);
    if not UserShop.boDelete then
    begin
      sShopMaster := UserShop.sMasterName;
      DeleteList.Add(Pointer(UserShop.nOffset));
      UserShop.boDelete := True;
      FileStream.Seek(UserShop.nOffset, soBeginning);
      FileStream.Write(UserShop^, SizeOf(Boolean));
      Result := True;
    end;
  end;

  if Result then
  begin
    nIndex1 := m_MasterNameList.GetIndex(sShopMaster);
    if nIndex1 >= 0 then
      m_MasterNameList.Delete(nIndex1);

    nIndex1 := m_UserShopList.GetIndex(ShopName);
    if nIndex1 >= 0 then
    begin
      UserShop := pTUserShop(m_UserShopList.Objects[nIndex1]);
      Dispose(UserShop);
      m_UserShopList.Delete(nIndex1);
    end;

    m_UserShopList.SortString(0, m_UserShopList.Count - 1);
    m_MasterNameList.SortString(0, m_UserShopList.Count - 1);
  end;
end;

function TUserShopFile.ResetUserShopName(const OldShopName, NewShopName: string): Integer;
var
  nIndex1: Integer;
  UserShop: pTUserShop;
begin
  if OldShopName = '' then
  begin
    Result := 1;
    Exit;
  end;

  if NewShopName = '' then
  begin
    Result := 2;
    Exit;
  end;

  if m_UserShopList.GetIndex(NewShopName) >= 0 then
  begin
    Result := 3;
  end;

  nIndex1 := m_UserShopList.GetIndex(OldShopName);
  if nIndex1 >= 0 then
  begin
    UserShop := pTUserShop(m_UserShopList.Objects[nIndex1]);
    if not UserShop.boDelete then
    begin
      m_UserShopList.Strings[nIndex1] := NewShopName;
      m_UserShopList.SortString(0, m_UserShopList.Count - 1);

      UserShop.sShopName := NewShopName;
      FileStream.Seek(UserShop.nOffset, soBeginning);
      FileStream.Write(UserShop^, SizeOf(Boolean));
      Result := 0;
    end;
  end
  else
  begin
    Result := 4;
  end;
end;

function TUserShopFile.Add(const ShopName, Account, MasterName: string): Boolean;
var
  nIndex1: Integer;
  nIndex2: Integer;
  UserShop: pTUserShop;
begin
  Result := False;
  if (Length(ShopName) > 0) and (Length(MasterName) > 0) then
  begin
    nIndex1 := m_MasterNameList.GetIndex(MasterName);
    if nIndex1 < 0 then
    begin
      nIndex2 := m_UserShopList.GetIndex(ShopName);
      if nIndex2 < 0 then
      begin
        New(UserShop);
        FillChar(UserShop^, SizeOf(TUserShop), #0);
        UserShop.boDelete := False;
        UserShop.sAccount := Account;
        UserShop.sShopName := ShopName;
        UserShop.sMasterName := MasterName;
        UserShop.boBusiness := True;
        UserShop.dCreateDate := Now;
        UserShop.dLastDate := Now;
        UserShop.nMaxShopItemCount := 10;                                                           // 店铺最高物品数量
        UserShop.nMaxStorageItemCount := 10;                                                        // 仓库最高物品数量

        if DeleteList.Count > 0 then
        begin
          UserShop.nOffset := Integer(DeleteList.Items[0]);
          DeleteList.Delete(0);
        end
        else
        begin
          FileStream.Seek(0, soEnd);
          UserShop.nOffset := FileStream.Position;
          FileStream.Seek(0, soBeginning);
          Inc(FRecordCount);
          FileStream.Write(FRecordCount, SizeOf(TRecordCount));
        end;
        FileStream.Seek(UserShop.nOffset, soBeginning);
        FileStream.Write(UserShop^, SizeOf(TUserShop) - SizeOf(Integer));

        m_MasterNameList.AddRecord(MasterName, NativeInt(UserShop));
        m_UserShopList.AddRecord(ShopName, NativeInt(UserShop));
        Result := True;
      end;
    end;
  end;
end;

function TUserShopFile.UpDate(UserShop: pTUserShop): Boolean;
begin
  UserShop.dLastDate := Now;
  FileStream.Seek(UserShop.nOffset, soBeginning);
  FileStream.Write(UserShop^, SizeOf(TUserShop) - SizeOf(Integer));
  Result := True;
end;

function TUserShopFile.UpDate(UserShop: pTUserShop; Size: Integer): Boolean;
begin
  UserShop.dLastDate := Now;
  FileStream.Seek(UserShop.nOffset, soBeginning);
  FileStream.Write(UserShop^, Min(SizeOf(TUserShop) - SizeOf(Integer), Size));
  Result := True;
end;
// ------------------------------------------------------------------------------

constructor TUserShopItemFile.Create();
begin
  DeleteList := TList.Create;
  m_ShopItemList := TQuickList.Create;
  m_SellItemList := TQuickNameList.Create;
  m_ShopNameList := TQuickNameList.Create;
  m_StorageList := TQuickNameList.Create;
  m_CharNameList := TQuickNameList.Create;
  m_SelledItemList := TQuickNameList.Create;

  { TODO -ochongchong -c内存泄露 : ++去内存泄露【2013-07-17】 }
  FileStream := nil;
end;

destructor TUserShopItemFile.Destroy;
var
  I: Integer;
  ShopItem: pTUserShopItem;
begin
  for I := 0 to m_ShopItemList.Count - 1 do
  begin
    ShopItem := pTUserShopItem(m_ShopItemList.Objects[I]);
    //Dispose(ShopItem.UserShop);
    Dispose(ShopItem);
  end;

  for I := 0 to m_ShopNameList.Count - 1 do
  begin
    m_ShopNameList.Objects[I].Free;
  end;
  for I := 0 to m_CharNameList.Count - 1 do
  begin
    m_CharNameList.Objects[I].Free;
  end;
  for I := 0 to m_SellItemList.Count - 1 do
  begin
    m_SellItemList.Objects[I].Free;
  end;
  for I := 0 to m_StorageList.Count - 1 do
  begin
    m_StorageList.Objects[I].Free;
  end;
  for I := 0 to m_SelledItemList.Count - 1 do
  begin
    m_SelledItemList.Objects[I].Free;
  end;

  DeleteList.Free;
  m_ShopItemList.Free;
  m_ShopNameList.Free;
  m_StorageList.Free;
  m_SellItemList.Free;
  m_CharNameList.Free;
  m_SelledItemList.Free;

  { TODO -ochongchong -c内存泄露 : ++去内存泄露【2013-07-17】 }
  if Assigned(FileStream) then
    FileStream.Free;
end;

function TUserShopItemFile.GetCount: Integer;
begin
  Result := m_ShopItemList.Count;
end;

function TUserShopItemFile.GetItem(Index: Integer): Pointer;
begin
  Result := m_ShopItemList.Objects[Index];
end;

procedure TUserShopItemFile.LoadShopItemList(const AFileName: string);
var
  I: Integer;
  UserShopItem: pTUserShopItem;
begin
  FLoadFile := True;
  FFileName := AFileName;
  if FileExists(FFileName) then
  begin
    if FileStream = nil then
      FileStream := TFileStream.Create(FFileName, fmOpenReadWrite or fmShareDenyNone);
  end
  else
  begin
    if FileStream = nil then
      FileStream := TFileStream.Create(FFileName, fmOpenReadWrite or fmShareDenyNone or fmCreate);
    FileStream.Write(FRecordCount, SizeOf(TRecordCount));
  end;
  FileStream.Seek(0, soBeginning);
  FileStream.Read(FRecordCount, SizeOf(TRecordCount));

  for I := 0 to FRecordCount - 1 do
  begin
    New(UserShopItem);
    FillChar(UserShopItem^, SizeOf(TUserShopItem), #0);
    UserShopItem.nOffset := FileStream.Position;
    if FileStream.Read(UserShopItem^, SizeOf(TUserShopItem) - SizeOf(Integer) * 2) <> SizeOf(TUserShopItem) - SizeOf(Integer) * 2 then
    begin
      Dispose(UserShopItem);
      break;
    end;

    if UserShopItem.boDelete then
    begin
      DeleteList.Add(Pointer(UserShopItem.nOffset));
      Dispose(UserShopItem);
    end
    else
    begin
      UserShopItem.UserShop := g_UserShopDB.GetShop(UserShopItem.sMasterName);
      if UserShopItem.sBuyName <> '' then
      begin
        m_SelledItemList.AddRecord(UserShopItem.sMasterName, UserShopItem);
      end
      else
      begin
        if UserShopItem.boAllowSell then
          m_SellItemList.AddRecord(UserShopItem.sMasterName, UserShopItem)
        else
          m_StorageList.AddRecord(UserShopItem.sMasterName, UserShopItem);
        m_ShopItemList.AddObject(IntToStr(UserShopItem.UserItem.MakeIndex), TObject(UserShopItem));
        m_ShopNameList.AddRecord(UserShopItem.sShopName, UserShopItem);
        m_CharNameList.AddRecord(UserShopItem.sMasterName, UserShopItem);
      end;
    end;

    if I mod 100 = 0 then
      Application.ProcessMessages;

    if Application.Terminated then
    begin
      Exit;
    end;
  end;

  m_ShopItemList.SortString(0, m_ShopItemList.Count - 1);
end;

function TUserShopItemFile.UpDate(UserShopItem: pTUserShopItem): Boolean;
begin
  FileStream.Seek(UserShopItem.nOffset, soBeginning);
  FileStream.Write(UserShopItem^, SizeOf(TUserShopItem) - SizeOf(Integer) * 2);
  Result := True;
end;

function TUserShopItemFile.UpDate(UserShopItem: pTUserShopItem; Size: Integer): Boolean;
begin
  FileStream.Seek(UserShopItem.nOffset, soBeginning);
  FileStream.Write(UserShopItem^, Min(SizeOf(TUserShopItem) - SizeOf(Integer) * 2, Size));
  Result := True;
end;

function TUserShopItemFile.Add(Item: pTUserShopItem): Boolean;
var
  UserShopItem: pTUserShopItem;
begin
  New(UserShopItem);
  FillChar(UserShopItem^, SizeOf(TUserShopItem), #0);
  UserShopItem^ := Item^;
  UserShopItem.dCreateDate := Now;
  UserShopItem.UserShop := g_UserShopDB.GetShop(UserShopItem.sMasterName);
  m_CharNameList.AddRecord(UserShopItem.sMasterName, UserShopItem);
  m_ShopItemList.AddRecord(IntToStr(UserShopItem.UserItem.MakeIndex), Integer(UserShopItem));
  m_ShopNameList.AddRecord(UserShopItem.sShopName, UserShopItem);

  if UserShopItem.boAllowSell then
  begin
    m_SellItemList.AddRecord(UserShopItem.sMasterName, UserShopItem);
  end
  else
  begin
    m_StorageList.AddRecord(UserShopItem.sMasterName, UserShopItem);
  end;

  if DeleteList.Count > 0 then
  begin
    UserShopItem.nOffset := Integer(DeleteList.Items[0]);
    DeleteList.Delete(0);
  end
  else
  begin
    FileStream.Seek(0, soEnd);
    UserShopItem.nOffset := FileStream.Position;
    FileStream.Seek(0, soBeginning);
    Inc(FRecordCount);
    FileStream.Write(FRecordCount, SizeOf(TRecordCount));
  end;

  FileStream.Seek(UserShopItem.nOffset, soBeginning);
  FileStream.Write(UserShopItem^, SizeOf(TUserShopItem) - SizeOf(Integer) * 2);
  Result := True;
end;

function TUserShopItemFile.SellItemCount(const CharName: string): Integer;
var
  nIndex: Integer;
  List: TList;
begin
  Result := 0;
  nIndex := m_SellItemList.GetChrList(CharName, List);
  if (nIndex >= 0) then
    Result := List.Count;
end;

function TUserShopItemFile.SellIedtemCount(const CharName: string): Integer;
var
  nIndex: Integer;
  List: TList;
begin
  Result := 0;
  nIndex := m_SelledItemList.GetChrList(CharName, List);
  if (nIndex >= 0) then
    Result := List.Count;
end;

function TUserShopItemFile.StorageItemCount(const CharName: string): Integer;
var
  nIndex: Integer;
  List: TList;
begin
  Result := 0;
  nIndex := m_StorageList.GetChrList(CharName, List);
  if (nIndex >= 0) then
    Result := List.Count;
end;

function TUserShopItemFile.ShopToStorage(const CharName: string; const MakeIndex: Integer): Boolean;
var
  nIndex, nSel: Integer;
  I: Integer;
  List: TList;
  UserShopItem: pTUserShopItem;
begin
  Result := False;
  nIndex := m_SellItemList.GetChrList(CharName, List);
  if (nIndex >= 0) then
  begin
    nSel := -1;
    UserShopItem := nil;
    for I := 0 to List.Count - 1 do
    begin
      UserShopItem := List.Items[I];
      if UserShopItem.UserItem.MakeIndex = MakeIndex then
      begin
        nSel := I;
        break;
      end;
      UserShopItem := nil;
    end;

    if UserShopItem <> nil then
    begin
      UserShopItem.boAllowSell := False;
      FileStream.Seek(UserShopItem.nOffset, soBeginning);

      FileStream.Write(UserShopItem^, SizeOf(TUserShopItem) - SizeOf(Integer) * 2);

      List.Delete(nSel);
      if List.Count <= 0 then
      begin
        m_SellItemList.Delete(nIndex);
        List.Free;
        // m_SellItemList.SortString(0, m_SellItemList.Count - 1);
      end;

      m_StorageList.AddRecord(CharName, UserShopItem);

      Result := True;
    end;
  end;
end;

function TUserShopItemFile.StorageToShop(const CharName: string; const MakeIndex: Integer): Boolean;
var
  nIndex, nSel: Integer;
  I: Integer;
  List: TList;
  UserShopItem: pTUserShopItem;
begin
  Result := False;
  nIndex := m_StorageList.GetChrList(CharName, List);
  if (nIndex >= 0) then
  begin
    nSel := -1;
    UserShopItem := nil;
    for I := 0 to List.Count - 1 do
    begin
      UserShopItem := List.Items[I];
      if UserShopItem.UserItem.MakeIndex = MakeIndex then
      begin
        nSel := I;
        break;
      end;
      UserShopItem := nil;
    end;

    if UserShopItem <> nil then
    begin
      UserShopItem.boAllowSell := True;
      FileStream.Seek(UserShopItem.nOffset, soBeginning);
      FileStream.Write(UserShopItem^, SizeOf(TUserShopItem) - SizeOf(Integer) * 2);

      List.Delete(nSel);
      if List.Count <= 0 then
      begin
        m_StorageList.Delete(nIndex);
        List.Free;
        // m_StorageList.SortString(0, m_StorageList.Count - 1);
      end;
      m_SellItemList.AddRecord(CharName, UserShopItem);
      Result := True;
    end;
  end;
end;

function TUserShopItemFile.DeleteShop(const CharName: string; const MakeIndex: Integer; UserItem: pTUserItem): Boolean;
var
  nIndex, nSel: Integer;
  I: Integer;
  List: TList;
  UserShopItem: pTUserShopItem;
begin
  Result := False;
  nIndex := m_SellItemList.GetChrList(CharName, List);
  if (nIndex >= 0) then
  begin
    nSel := -1;
    UserShopItem := nil;
    for I := 0 to List.Count - 1 do
    begin
      UserShopItem := List.Items[I];
      if UserShopItem.UserItem.MakeIndex = MakeIndex then
      begin
        nSel := I;
        break;
      end;
      UserShopItem := nil;
    end;

    if UserShopItem <> nil then
    begin
      UserShopItem.boDelete := True;
      DeleteList.Add(Pointer(UserShopItem.nOffset));

      FileStream.Seek(UserShopItem.nOffset, soBeginning);
      FileStream.Write(UserShopItem^, SizeOf(Boolean));
      if UserItem <> nil then
        UserItem^ := UserShopItem.UserItem;

      List.Delete(nSel);
      if List.Count <= 0 then
      begin
        m_SellItemList.Delete(nIndex);
        List.Free;
        // m_SellItemList.SortString(0, m_SellItemList.Count - 1);
      end;

      Result := True;
    end;
  end;
end;

function TUserShopItemFile.DeleteStorage(const CharName: string; const MakeIndex: Integer; UserItem: pTUserItem): Boolean;
var
  nIndex, nSel: Integer;
  I: Integer;
  List: TList;
  UserShopItem: pTUserShopItem;
begin
  Result := False;
  nIndex := m_StorageList.GetChrList(CharName, List);
  if (nIndex >= 0) then
  begin
    nSel := -1;
    UserShopItem := nil;
    for I := 0 to List.Count - 1 do
    begin
      UserShopItem := List.Items[I];
      if UserShopItem.UserItem.MakeIndex = MakeIndex then
      begin
        nSel := I;
        break;
      end;
      UserShopItem := nil;
    end;

    if UserShopItem <> nil then
    begin
      UserShopItem.boDelete := True;
      DeleteList.Add(Pointer(UserShopItem.nOffset));

      FileStream.Seek(UserShopItem.nOffset, soBeginning);
      FileStream.Write(UserShopItem^, SizeOf(Boolean));
      if UserItem <> nil then
        UserItem^ := UserShopItem.UserItem;

      List.Delete(nSel);
      if List.Count <= 0 then
      begin
        List.Free;
        m_StorageList.Delete(nIndex);
        // m_StorageList.SortString(0, m_StorageList.Count - 1);
      end;

      Result := True;
    end;
  end;
end;

function TUserShopItemFile.AddSell(Item: pTUserShopItem): Boolean;
var
  UserShopItem: pTUserShopItem;
begin
  New(UserShopItem);
  UserShopItem^ := Item^;
  UserShopItem.dCreateDate := Now;

  FileStream.Seek(UserShopItem.nOffset, soBeginning);
  FileStream.Write(UserShopItem^, SizeOf(TUserShopItem) - SizeOf(Integer) * 2);
  m_SelledItemList.AddRecord(UserShopItem.sMasterName, UserShopItem);
  Result := True;
end;

end.
