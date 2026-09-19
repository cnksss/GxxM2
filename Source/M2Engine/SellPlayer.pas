unit SellPlayer;

interface

uses
  SysUtils, Classes, Grobal2, FastIniFile;

type
  PSellPlayerInfo = ^TSellPlayerInfo;

  TSellPlayerInfo = record
    Account: string[ACCOUNT_LEN]; // 出售帐号
    Player: string[ACTOR_NAME_LEN]; // 出售角色
    Delegater: string[ACTOR_NAME_LEN]; // 委托角色
    SellPricesType: Integer; // 货币类型
    SellPrices: Integer; // 出售价格
    SellTime: TDateTime; // 出售时间
    SetUser: Boolean; // 是否指定购买人
    SetUserName: string[ACTOR_NAME_LEN]; // 指定购买人
    IsBuying: Boolean; // 正在被购买中
  end;

  TSellPlayerList = class(TObject)
  private
    FList: TList;
    FIniFileName: string;
    function GetCount: Integer;
    function GetItems(Index: Integer): PSellPlayerInfo;
  public
    constructor Create;
    destructor Destroy; override;

    procedure LoadConfig;
    procedure SaveConfig;
    procedure AutoLoadSellPlayer;

    procedure Clear;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: PSellPlayerInfo read GetItems;

    function Search(Player: string; var Index: Integer): Boolean;
    function AddSellPlayer(Account, Player, Delegater: string; SellPricesType, SellPrices: Integer; SellTime: TDateTime;
      IsSetUser: Boolean; SetUserName: string): Boolean;
    procedure DeleteByIndex(Index: Integer);
    function DeletePlayer(Player, Delegater: string): Boolean;
    function DeletePlayerEx(Player: string): Boolean;
  end;

implementation

uses
  M2Share, UsrEngn{$IF MULTI_THREAD = 1}, M2Threads{$IFEND};

constructor TSellPlayerList.Create;
begin
  FList := TList.Create;
  FList.Capacity := 300;
  FIniFileName := g_Config.sEnvirDir + 'SellPlayer.ini';
end;

destructor TSellPlayerList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

procedure TSellPlayerList.Clear;
var
  I: Integer;
  Info: PSellPlayerInfo;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Info := FList.Items[I];
    Dispose(Info);
  end;
  FList.Clear;
end;

function TSellPlayerList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TSellPlayerList.GetItems(Index: Integer): PSellPlayerInfo;
begin
  Result := FList.Items[Index];
end;

function TSellPlayerList.AddSellPlayer(Account, Player, Delegater: string; SellPricesType, SellPrices: Integer;
  SellTime: TDateTime; IsSetUser: Boolean; SetUserName: string): Boolean;
var
  Index: Integer;
  Info: PSellPlayerInfo;
begin
  Result := not Search(Player, Index);

  if Result then
  begin
    New(Info);
    FillChar(Info^, SizeOf(TSellPlayerInfo), 0);

    Info.Account := Account;
    Info.Player := Player;
    Info.Delegater := Delegater;
    Info.SellPrices := SellPrices;
    Info.SellPricesType := SellPricesType;
    Info.SellTime := SellTime;
    Info.SetUser := IsSetUser;
    Info.SetUserName := SetUserName;
    Info.IsBuying := False;

    FList.Insert(Index, Info);
  end;
end;

function TSellPlayerList.Search(Player: string; var Index: Integer): Boolean;
var
  L, H, C, I: Integer;
  Info: PSellPlayerInfo;
begin
  Result := False;

  L := 0;
  H := FList.Count - 1;
  while L <= H do
  begin
    I := L + (H - L) shr 1;
    Info := FList.Items[I];
    C := AnsiCompareText(Info.Player, Player);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

procedure TSellPlayerList.DeleteByIndex(Index: Integer);
var
  Info: PSellPlayerInfo;
begin
  Info := FList.Items[Index];
  Dispose(Info);
  FList.Delete(Index);
end;

function TSellPlayerList.DeletePlayer(Player, Delegater: string): Boolean;
var
  Index: Integer;
  Info: PSellPlayerInfo;
begin
  Result := False;
  if Search(Player, Index) then
  begin
    Info := FList.Items[Index];
    if SameText(Info.Delegater, Delegater) then
    begin
      Dispose(Info);
      FList.Delete(Index);
      Result := True;

      SaveConfig;
    end;
  end;
end;

function TSellPlayerList.DeletePlayerEx(Player: string): Boolean;
var
  Index: Integer;
  Info: PSellPlayerInfo;
begin
  Result := False;
  if Search(Player, Index) then
  begin
    Info := FList.Items[Index];
    Dispose(Info);
    FList.Delete(Index);
    Result := True;

    SaveConfig;
  end;
end;

procedure TSellPlayerList.LoadConfig;
var
  I, Count: Integer;
  Section, Account, Player, Delegater, SetUserName: string;
  MoneyType, SellPrices: Integer;
  SellTime: TDateTime;
  boSetUser: Boolean;
  IniFile: TFastIniFile;
begin
  Clear;
  if FileExists(FIniFileName) then
  begin
    IniFile := TFastIniFile.Create(FIniFileName);
    try
      Count := IniFile.ReadInteger('setup', 'count', 0);
      for I := 0 to Count - 1 do
      begin
        Section := IntToStr(I + 1);
        Account := IniFile.ReadString(Section, 'Account', '');
        Player := IniFile.ReadString(Section, 'Player', '');
        Delegater := IniFile.ReadString(Section, 'Delegater', '');
        MoneyType := IniFile.ReadInteger(Section, 'MoneyType', 0);
        SellPrices := IniFile.ReadInteger(Section, 'SellPrices', 0);
        SellTime := IniFile.ReadFixedDateTime(Section, 'SellTime', 0);
        boSetUser := IniFile.ReadBoolean(Section, 'SetUser', False);
        SetUserName := IniFile.ReadString(Section, 'SetUserName', '');

        if (Player <> '') and (Delegater <> '') and (MoneyType >= 0) and (MoneyType <= 4) and (SellPrices > 0) then
        begin
          if SetUserName = '' then
            boSetUser := False;
          AddSellPlayer(Account, Player, Delegater, MoneyType, SellPrices, SellTime, boSetUser, SetUserName);
        end;
      end;
    finally
      IniFile.Free;
    end;
  end;
end;

procedure TSellPlayerList.SaveConfig;
var
  I: Integer;
  Section: string;
  Info: PSellPlayerInfo;
  IniFile: TFastIniFile;
begin
  IniFile := TFastIniFile.Create(FIniFileName);
  try
    try
      // IniFile.Clear;
      IniFile.WriteInteger('setup', 'count', FList.Count);
      for I := 0 to FList.Count - 1 do
      begin
        Info := FList.Items[I];

        Section := IntToStr(I + 1);
        IniFile.WriteString(Section, 'Account', Info.Account);
        IniFile.WriteString(Section, 'Player', Info.Player);
        IniFile.WriteString(Section, 'Delegater', Info.Delegater);
        IniFile.WriteInteger(Section, 'MoneyType', Info.SellPricesType);
        IniFile.WriteInteger(Section, 'SellPrices', Info.SellPrices);
        IniFile.WriteFixedDateTime(Section, 'SellTime', Info.SellTime);
        IniFile.WriteBoolean(Section, 'SetUser', Info.SetUser);
        IniFile.WriteString(Section, 'SetUserName', Info.SetUserName);
      end;
    finally
      IniFile.Free;
    end;
  except
  end;
end;

procedure TSellPlayerList.AutoLoadSellPlayer;
var
  I: Integer;
  OffLineData: pTOffLineData;
  Info: PSellPlayerInfo;
begin
  if FList.Count > 0 then
  begin
    UserEngine.m_boStartAutoLoadSellPlayer := False;

{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      UserEngine.m_AutoLoadSellPlayerList.LockW(2);
    try
{$IFEND}
      for I := 0 to FList.Count - 1 do
      begin
        Info := FList.Items[I];

        New(OffLineData);
        OffLineData.sAccount := Info.Account;
        OffLineData.sCharName := Info.Player;
        OffLineData.boStartLogin := False;
        OffLineData.dwStartLoginTick := MyGetTickCount;
        OffLineData.SessInfo := nil;

        UserEngine.m_AutoLoadSellPlayerList.Add(OffLineData);
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UserEngine.m_AutoLoadSellPlayerList.UnLockW;
    end;
{$IFEND}
    UserEngine.m_boStartAutoLoadSellPlayer := UserEngine.m_AutoLoadSellPlayerList.Count > 0;

    if UserEngine.m_boStartAutoLoadSellPlayer then
      MainOutMessage('正在登录出售角色...');
  end;
end;

end.
