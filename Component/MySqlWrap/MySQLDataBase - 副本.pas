unit MySQLDataBase;

interface

uses
  Windows, Classes, SysUtils, MySQLCli, MySQLUtil, MySQLWrap, Math;

type
  TMySQLResult = class;
  TMySQLField = class;

  TMySQLStatements = class;

  TMySQLDataBase = class(TObject)
  private
    FLib: TMySQLLib;
    FPMySQL: PMYSQL;

    FCurrDB: string;

    FCharsetName: string;
    FEncoding: TMyEncoding;

    FServerVersion: TMyVersion;
    FClientVersion: TMyVersion;
    FPwd: string;
    FPort: Cardinal;
    FHost: string;
    FUser: string;
    FFlags: my_ulong;

    FSQLStatements: TMySQLStatements;

    function GetClientInfo: string;
    function GetServerInfo: string;

    function GetSSLCipher: string;
    function GetClientVersion: TMyVersion;
    function GetServerVersion: TMyVersion;
    function GetServerStatus: Cardinal;

    function GetDB: string;
    procedure SetDB(const AValue: string);

    function GetHostInfo: string;
    function GetInsert_ID: my_ulonglong;

    function GetCharacterSetName: string;
    procedure SetCharacterSetName(const AValue: string);
  private
    procedure ProcessError(AErrNo: Cardinal; const AMsg, ASQLState: string);
  public
    constructor Create(ALib: TMySQLLib);
    destructor Destroy; override;

    procedure Init;
    procedure SSLInit(const Akey, Acert, Aca, Acapath, Acipher: string);
    procedure Connect(const host, user, passwd, db: string;
      port: Cardinal; clientflag: my_ulong);
    procedure Disconnect;
    procedure KillQuery;
    procedure Ping;

    procedure Exec(const ACmd: string);     // 可执行多行Sql
    procedure Query(const ACmd: string);    // 只能执行一行且不为;结束的sql
    procedure Check(ACode: Integer = -1);
    function  CheckDBExists(const DBName: string): Boolean;
    procedure CreateDB(const DBName: string; const DefCharset: string = 'utf8'; const Collate: string = 'utf8_general_ci');
    procedure DropDB(const DBName: string);

    procedure ClearResult;
    function StoreResult: TMySQLResult;
    function UseResult: TMySQLResult;
    function MoreResults: Boolean;
    function NextResult: Boolean;

    property ServerInfo: string read GetServerInfo;
    property ServerVersion: TMyVersion read GetServerVersion;
    property ServerStatus: Cardinal read GetServerStatus;

    property ClientInfo: string read GetClientInfo;
    property ClientVersion: TMyVersion read GetClientVersion;

    property DB: string read GetDB write SetDB;
    property Lib: TMySQLLib read FLib;

    property CharacterSetName: string read GetCharacterSetName write SetCharacterSetName;
    property HostInfo: string read GetHostInfo;
    property Insert_ID: my_ulonglong read GetInsert_ID;
    property MySQL: PMYSQL read FPMySQL;
    property Host: string read FHost;
    property User: string read FUser;
    property Pwd: string read FPwd;
    property Port: Cardinal read FPort;
    property Flags: my_ulong read FFlags;
    property SSLCipher: string read GetSSLCipher;

    procedure GetInfo;

    procedure StartTransaction;
    procedure Commit;
    procedure Rollback;

    property Statements: TMySQLStatements read FSQLStatements;
  end;

  TMySQLResult = class(TObject)
  private
    FOwner: TMySQLDataBase;
    FCursor: PMYSQL_RES;
    FpRow: MYSQL_ROW;
    FpLengths: Pmy_ulong;
    FField: TMySQLField;
    function GetFieldCount: Cardinal;
    function GetData(AIndex: Integer; var ApData: Pointer; var ALen: LongWord): Boolean;

    function GetFields(AIndex: Integer): TMySQLField;
    function GetValues(AIndex: Integer): string;
  public
    constructor Create(AOwner: TMySQLDataBase; AResult: PMYSQL_RES);
    destructor Destroy; override;
    function Fetch: Boolean;
    property FieldCount: Cardinal read GetFieldCount;
    property Fields[AIndex: Integer]: TMySQLField read GetFields;
    property Values[AIndex: Integer]: string read GetValues;
  end;

  TMySQLField = class(TObject)
  private
    FResult: TMySQLResult;
    FpFld: PMYSQL_FIELD;
  public
    procedure GetInfo(var name, srcname, table, db: my_pchar; var type_: Byte;
      var length, flags, decimals, charsetnr: LongWord);
    constructor Create(AResult: TMySQLResult);
  end;

  PMySqlStatementBindValue = ^TMySqlStatementBindValue;
  TMySqlStatementBindValue = record
    Buf: PByte;
    BufLen: Cardinal;
  end;

  TMySqlStatement = class(TObject)
  private
    FOwner: TMySqlStatements;
    FOwnerDB: TMySQLDataBase;

    FName: string;
    FSql: string;
    FHandle: PMYSQL_STMT;

    FAutoOrderIndex: Integer;

    FBinds: PMYSQL_BIND;
    FBindSize: Cardinal;

    FStatementRecordSize: Integer;

    FParamCount: Integer;
    FParamValues: TList;
    FParamLength: array of Cardinal;

    FNullValue: Byte;

    FResult: PMYSQL_RES;
    FResultFieldCount: Integer;
    FRow: MYSQL_ROW;
    FFetchLengths: Pmy_ulong;

    procedure GetParamBindInfo(ParamIndex: Integer;
      var BufType: Pmy_long; var Buf: PPByte; var BufLen: Pmy_ulong;
      var Len: PPmy_ulong; var IsNull: PPmy_bool;
      var BindValue: PMySqlStatementBindValue);

    function GetData(AIndex: Integer; var ApData: Pointer; var ALen: LongWord): Boolean;
  public
    constructor Create(AOwner: TMySqlStatements; AOwnerDB: TMySQLDataBase);
    destructor Destroy; override;

    property Owner: TMySqlStatements read FOwner;
    property Name: string read FName;
    property Sql: string read FSql write FSql;
    property Handle: PMYSQL_STMT read FHandle;
    property ResultFieldCount: Integer read FResultFieldCount;

    function Prepare: Boolean;
    function Reset: Boolean;
    function Step: Boolean;
    function Query: Boolean;
    function Fetch: Boolean;
    function Finalize: Boolean;

    procedure Close;

    function GetBindParamCount: Integer;
    function GetValues(AIndex: Integer): string;

    function BindNull(const Index: Integer): Boolean;
    function BindInt(const Index: Integer; const Value: Integer): Boolean;
    function BindInt64(const Index: Integer; const Value: Int64): Boolean;
    function BindBool(const Index: Integer; const Value: Boolean): Boolean;
    function BindDouble(const Index: Integer; const Value: Double): Boolean;
    function BindDateTime(const Index: Integer; const Value: TDateTime): Boolean;
    function BindText(const Index: Integer; const Value: AnsiString): Boolean;


    // BindValue Index 是从1开始，先增加再使用
    function OrderBindNull(): Boolean;
    function OrderBindInt(const Value: Integer): Boolean;
    function OrderBindInt64(const Value: Int64): Boolean;
    function OrderBindBool(const Value: Boolean): Boolean;
    function OrderBindDouble(const Value: Double): Boolean;
    function OrderBindDateTime(const Value: TDateTime): Boolean;
    function OrderBindText(const Value: AnsiString): Boolean;
  end;

  TMySqlStatements = class(TObject)
  private
    FOwner: TMySQLDataBase;
    FList: TList;
    function Search(Name: string; var Index: Integer): Boolean; virtual;
    function GetCount: Integer;
    function GetItems(Index: Integer): TMySqlStatement;
  public
    constructor Create(AOwner: TMySQLDataBase);
    destructor Destroy; override;

    function AddSQLStatement(Name: string): TMySqlStatement;
    procedure Delete(Index: Integer);
    procedure Remove(Name: string);
    procedure Clear;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: TMySqlStatement read GetItems;
    function Find(Name: string): TMySqlStatement;
  end;


implementation

type
  PSQLTimeStamp = ^TSQLTimeStamp;
  TSQLTimeStamp = record
    Year: Word;
    Month: Word;
    Day: Word;
    Hour: Word;
    Minute: Word;
    Second: Word;
    Fractions: Cardinal;
  end;

{ TMySQLDataBase }

constructor TMySQLDataBase.Create(ALib: TMySQLLib);
begin
  inherited Create;
  FLib := ALib;
  FPMySQL := nil;
  FEncoding := ecANSI;

  FSQLStatements := TMySQLStatements.Create(Self);
end;

destructor TMySQLDataBase.Destroy;
begin
  FSQLStatements.Free;

  if FPMySQL <> nil then
    Disconnect;
  inherited;
end;

procedure TMySQLDataBase.Check(ACode: Integer);
var
  iErrNo: Cardinal;
  sMsg, sSQLState: string;
begin
  if ACode <> 0 then
  begin
    iErrNo := FLib.mysql_errno(FPMySQL);
    if (ACode <> -1) or (iErrNo <> 0) then
    begin
      sMsg := FLib.mysql_error(FPMySQL);
      if Assigned(FLib.mysql_sqlstate) then
        sSQLState := FLib.mysql_sqlstate(FPMySQL);
      ProcessError(iErrNo, sMsg, sSQLState);
    end;
  end;
end;

procedure TMySQLDataBase.ProcessError(AErrNo: Cardinal; const AMsg, ASQLState: string);
begin
  raise Exception.CreateFmt('SQLSTATE[%s][%d] %s', [ASQLState, AErrNo, AMsg]);
end;

procedure TMySQLDataBase.Connect(const host, user, passwd, db: string;
  port: Cardinal; clientflag: my_ulong);
var
  sHost, sUser, sPwd, sDb: TMyAnsistring;
begin
  sHost := host;
  sUser := user;
  sPwd := passwd;
  sDb := db;
  if Assigned(FLib.mysql_real_connect) then
  begin
    if FLib.mysql_real_connect(FPMySQL, my_pchar(PByte(sHost)),
        my_pchar(PByte(sUser)), my_pchar(PByte(sPwd)),
        my_pchar(PByte(sDb)), port, nil, clientflag) = nil then
      Check;
  end
  else
  begin
    if (port <> 0) and (port <> MYSQL_PORT) then
      raise Exception.Create('MySql不支持设置端口号');

    if FLib.mysql_connect(FPMySQL, my_pchar(PByte(sHost)),
        my_pchar(PByte(sUser)), my_pchar(PByte(sPwd))) = nil then
      Check;

    if db <> '' then
      Query('USE ' + db);
  end;
  FCurrDB := db;
  FHost := host;
  FUser := user;
  FPwd := passwd;
  FPort := port;
  FFlags := clientflag;

  if FPMySQL <> nil then
  begin
    OutputDebugstring(PChar(CharacterSetName));
  end;
end;

procedure TMySQLDataBase.Disconnect;
begin
  if FPMySQL = nil then Exit;
  FLib.mysql_close(FPMySQL);
  FPMySQL := nil;
end;

procedure TMySQLDataBase.Init;
begin
  FLib.Lock;
  try
    FPMySQL := FLib.mysql_init(nil);
  finally
    FLib.UnLock;
  end;
end;

function TMySQLDataBase.GetDB: string;
var
  pDb: my_pchar;
begin
  Result := '';
  if (Lib.Version >= mvMySQL050700) then
    pDb := PMYSQL0570(FPMySQL)^.db
  else if Lib.Version >= mvMySQL050100 then
    pDb := PMYSQL0510(FPMySQL)^.db
  else if Lib.Version >= mvMySQL050006 then
    pDb := PMYSQL0506(FPMySQL)^.db
  else if Lib.Version >= mvMySQL050000 then
    pDb := PMYSQL0500(FPMySQL)^.db
  else if Lib.Version >= mvMySQL041000 then
    pDb := PMYSQL0410(FPMySQL)^.db
  else if Lib.Version >= mvMySQL040000 then
    pDb := PMYSQL0400(FPMySQL)^.db
  else if Lib.Version >= mvMySQL032300 then
    pDb := PMYSQL0323(FPMySQL)^.db
  else if Lib.Version >= mvMySQL032000 then
    pDb := PMYSQL0320(FPMySQL)^.db
  else
    pDb := nil;
  if pDb <> nil then
    Result := pDb;
  if Result = '' then
    Result := FCurrDB;
end;

procedure TMySQLDataBase.SetDB(const AValue: string);
var
  sDb: TMyAnsistring;
begin
  sDb := AValue;
  Check(FLib.mysql_select_db(FPMySQL, my_pchar(PByte(sDb))));
  FCurrDB := AValue;
end;

function TMySQLDataBase.CheckDBExists(const DBName: string): Boolean;
var
  sDb: TMyAnsistring;
begin
  sDb := DBName;
  Result := FLib.mysql_select_db(FPMySQL, my_pchar(PByte(sDb))) = MYSQL_SUCCESS;
end;

procedure TMySQLDataBase.CreateDB(const DBName, DefCharset, Collate: string);
var
  S: string;
begin
  S := 'CREATE DATABASE ' + DBName;

  if Length(DefCharset) >0  then
  begin
    S := S + ' DEFAULT CHARSET ' + DefCharset;

    if Length(Collate) > 0 then
    begin
      S := S + ' COLLATE ' + Collate;
    end;
  end;

  Query(S);
end;

procedure TMySQLDataBase.DropDB(const DBName: string);
begin
  Query('DROP DATABASE ' + DBName);
end;

function TMySQLDataBase.GetCharacterSetName: string;
var
  CharSetInfo: MY_CHARSET_INFO;
begin
  if Assigned(FLib.mysql_character_set_name) then
  begin
    FLib.mysql_character_set_name(FPMySQL);
  end
  else if Assigned(FLib.mysql_get_character_set_info) then
  begin
    FLib.mysql_get_character_set_info(FPMySQL, CharSetInfo);
    Result := CharSetInfo.name;
  end;
end;

function MyInSet(AChar: Char; ASet: TMyCharSet): Boolean;
begin
  if Ord(AChar) <= 255 then
    Result := AChar in ASet
  else
    Result := False;
end;

procedure TMySQLDataBase.SetCharacterSetName(const AValue: string);
var
  iRes: Integer;
  sName: TMyAnsistring;
begin
  iRes := -1;
  if Assigned(FLib.mysql_set_character_set) then
  begin
    sName := AValue;
    iRes := FLib.mysql_set_character_set(FPMySQL, my_pchar(PByte(sName)));
  end;
  if iRes <> 0 then
  begin
    try
      if ServerVersion >= mvMySQL040100 then
        Query('SET NAMES ''' + AValue + '''')
      else
        Query('SET CHARACTER SET ' + AValue);
      iRes := 0;
    except
      // hide exception
    end;
  end;

  //MultiByteToWideChar(CP_UTF8, 0, pData, iLen, PWideChar(@s2[1]), Length(s2));

  if iRes = 0 then
  begin
    FCharsetName := AValue;
    if (Length(AValue) >= 4) and (StrLIComp(PChar(AValue), PChar('UTF8'), 4) = 0) then
    begin
      FEncoding := ecUTF8;
      //FUtf8mb4 := StrLIComp(PChar(AValue), PChar('UTF8MB4'), 7) = 0;
    end
    else
      FEncoding := ecANSI;
  end;
end;

procedure TMySQLDataBase.SSLInit(const Akey, Acert, Aca, Acapath,
  Acipher: string);
var
  sKey, sCert, sCa, sCAPath, sCipher: TMyAnsistring;
begin
  if Assigned(FLib.mysql_ssl_set) then
  begin
    sKey := Akey;
    sCert := Acert;
    sCa := Aca;
    sCAPath := Acapath;
    sCipher := Acipher;

    FLib.mysql_ssl_set(FPMySQL, PAnsiChar(sKey), PAnsiChar(sCert), PAnsiChar(sCa), PAnsiChar(sCAPath), PAnsiChar(sCipher));
  end;
end;

procedure TMySQLDataBase.StartTransaction;
begin
  Query('BEGIN');
end;

procedure TMySQLDataBase.Commit;
begin
  Query('COMMIT');
end;

procedure TMySQLDataBase.Rollback;
begin
  Query('ROLLBACK');
end;

function TMySQLDataBase.StoreResult: TMySQLResult;
var
  pRes: PMYSQL_RES;
begin
  pRes := FLib.mysql_store_result(FPMySQL);
  Check;
  if pRes = nil then
    Result := nil
  else
    Result := TMySQLResult.Create(Self, pRes);
end;

procedure TMySqlDataBase.ClearResult;
var
  pRes: PMYSQL_RES;
begin
  repeat
    pRes := FLib.mysql_use_result(FPMySQL);
    FLib.mysql_free_result(pRes);
  until FLib.mysql_next_result(FPMySQL) <> 0;
end;

function TMySQLDataBase.UseResult: TMySQLResult;
var
  pRes: PMYSQL_RES;
begin
  pRes := FLib.mysql_use_result(FPMySQL);
  Check;
  if pRes = nil then
    Result := nil
  else
    Result := TMySQLResult.Create(Self, pRes);
end;

function TMySQLDataBase.MoreResults: Boolean;
begin
  if not Assigned(FLib.mysql_more_results) then
    Result := False
  else
    Result := FLib.mysql_more_results(FPMySQL) = 1;
end;

function TMySQLDataBase.NextResult: Boolean;
begin
  if not Assigned(FLib.mysql_next_result) then
    Result := False
  else
  begin
    case FLib.mysql_next_result(FPMySQL) of
      my_bool(-1): Result := False;
      0:           Result := True;
    else
      begin
        Result := False;
        Check;
      end;
    end;
  end;
end;

procedure TMySQLDataBase.Ping;
begin
  Check(FLib.mysql_ping(FPMySQL));
end;

procedure TMySQLDataBase.Exec(const ACmd: string);
var
  iRes: Integer;
  sCmd: TMyAnsistring;
begin
  if FEncoding = ecUTF8 then
    sCmd := Utf8Encode(ACmd)
  else
    sCmd := ACmd;

  iRes := FLib.mysql_query(FPMySQL, my_pchar(PByte(sCmd)));
  Check(iRes);
  GetInfo;
end;

procedure TMySQLDataBase.Query(const ACmd: string);
var
  iRes: Integer;
  sCmd: TMyAnsistring;
begin
  if FEncoding = ecUTF8 then
    sCmd := Utf8Encode(ACmd)
  else
    sCmd := ACmd;

  iRes := FLib.mysql_real_query(FPMySQL, my_pchar(PByte(sCmd)), Length(sCmd));
  Check(iRes);
  GetInfo;
end;

procedure TMySQLDataBase.KillQuery;
var
  oSess: TMySQLDataBase;
begin
  oSess := TMySQLDataBase.Create(FLib);
  try
    oSess.Init;
    oSess.Connect(FHost, FUser, FPwd, FCurrDB, FPort, FFlags);
    oSess.Query(Format('KILL QUERY %u', [FLib.mysql_thread_id(FPMySQL)]));
  finally
    oSess.Free;
  end;
end;

procedure TMySQLDataBase.GetInfo;
var
  pInfo: my_pchar;
begin
  pInfo := FLib.mysql_info(FPMySQL);
  if pInfo <> nil then
  begin
    //SetInfo(pInfo, my_pchar(Encoder.Encode('Info', ecANSI)), MYSQL_SUCCESS);
  end;
end;

function TMySQLDataBase.GetClientInfo: string;
begin
  Result := FLib.mysql_get_client_info();
end;

function TMySQLDataBase.GetClientVersion: TMyVersion;
begin
  if FClientVersion = 0 then
    FClientVersion := MyVerStr2Int(ClientInfo);
  Result := FClientVersion;
end;

function TMySQLDataBase.GetHostInfo: string;
var
  pInfo: my_pchar;
begin
  pInfo := FLib.mysql_get_host_info(FPMySQL);
  Result := pInfo;
end;

function TMySQLDataBase.GetInsert_ID: my_ulonglong;
begin
  Result := FLib.mysql_insert_id(FPMySQL);
end;

function TMySQLDataBase.GetServerInfo: string;
begin
  Result := FLib.mysql_get_server_info(FPMySQL);
end;

function TMySQLDataBase.GetServerStatus: Cardinal;
begin
  if (Lib.Version >= mvMySQL050700) then
    Result := PMYSQL0570(FPMySQL)^.server_status
  else if Lib.Version >= mvMySQL050100 then
    Result := PMYSQL0510(FPMySQL)^.server_status
  else if Lib.Version >= mvMySQL050006 then
    Result := PMYSQL0506(FPMySQL)^.server_status
  else if Lib.Version >= mvMySQL050000 then
    Result := PMYSQL0500(FPMySQL)^.server_status
  else if Lib.Version >= mvMySQL041000 then
    Result := PMYSQL0410(FPMySQL)^.server_status
  else if Lib.Version >= mvMySQL040000 then
    Result := PMYSQL0400(FPMySQL)^.server_status
  else if Lib.Version >= mvMySQL032300 then
    Result := PMYSQL0323(FPMySQL)^.server_status
  else
    Result := 0;
end;

function TMySQLDataBase.GetServerVersion: TMyVersion;
begin
  if FServerVersion = 0 then
    FServerVersion := MyVerStr2Int(ServerInfo);
  Result := FServerVersion;
end;

function TMySQLDataBase.GetSSLCipher: string;
begin
  if Assigned(FLib.mysql_get_ssl_cipher) then
    Result := FLib.mysql_get_ssl_cipher(FPMySQL)
  else
    Result := '';
end;

{ TMySQLResult }

constructor TMySQLResult.Create(AOwner: TMySQLDataBase; AResult: PMYSQL_RES);
begin
  inherited Create;
  FOwner := AOwner;
  FCursor := AResult;
end;

destructor TMySQLResult.Destroy;
begin
  FOwner.FLib.mysql_free_result(FCursor);
  FCursor := nil;

  if FField <> nil then
  begin
    FreeAndNil(FField);
  end;

  inherited;
end;

function TMySQLResult.Fetch: Boolean;
begin
  FpRow := FOwner.FLib.mysql_fetch_row(FCursor);
  Result := (FpRow <> nil);
  if Result then
  begin
    FpLengths := FOwner.FLib.mysql_fetch_lengths(FCursor);
    if FpLengths = nil then FOwner.Check;
  end
  else
  begin
    FOwner.Check;
  end;
end;

function TMySQLResult.GetData(AIndex: Integer; var ApData: Pointer;
  var ALen: LongWord): Boolean;
begin
  ALen := Pmy_ulong(NativeUInt(FpLengths) + NativeUInt(AIndex) * SizeOf(my_ulong))^;
  ApData := PPByte(NativeUInt(FpRow) + NativeUInt(AIndex) * SizeOf(PByte))^;
  Result := ApData <> nil;
end;

function TMySQLResult.GetFieldCount: Cardinal;
begin
  Result := FOwner.FLib.mysql_num_fields(FCursor);
end;

function TMySQLResult.GetFields(AIndex: Integer): TMySQLField;
begin
  if FField = nil then
    FField := TMySQLField.Create(Self);

  FField.FpFld := FOwner.FLib.mysql_fetch_field_direct(FCursor, AIndex);
  Result := FField;
end;

function TMySQLResult.GetValues(AIndex: Integer): string;
var
  pData: Pointer;
  DataLen: Cardinal;
  S: AnsiString;
begin
  Result := '';
  if GetData(AIndex, pData, DataLen) then
  begin
    SetLength(S, DataLen);
    Move(pData^, S[1], DataLen);

    // iLen := MultiByteToWideChar(CP_UTF8, 0, pData, iLen, PWideChar(@s2[1]), Length(s2));
    if FOwner.FEncoding = ecUTF8 then
      Result := Utf8Decode(S)
    else
      Result := S;
  end;
end;

{ TMySQLField }

constructor TMySQLField.Create(AResult: TMySQLResult);
begin
  inherited Create;
  FResult := AResult;
end;

procedure TMySQLField.GetInfo(var name, srcname, table, db: my_pchar;
  var type_: Byte; var length, flags, decimals, charsetnr: LongWord);
var
  pFld0510: PMYSQL_FIELD0510;
  pFld0410: PMYSQL_FIELD0410;
  pFld0401: PMYSQL_FIELD0401;
  pFld0400: PMYSQL_FIELD0400;
  pFld0320: PMYSQL_FIELD0320;
begin
  if FResult.FOwner.FLib.Version >= mvMySQL050100 then
  begin
    pFld0510 := PMYSQL_FIELD0510(FpFld);
    name := pFld0510^.name;
    srcname := pFld0510^.org_name;
    table := pFld0510^.org_table;
    db := pFld0510^.db;
    type_ := pFld0510^.type_;
    length := pFld0510^.length;
    if pFld0510^.max_length > length then
      length := pFld0510^.max_length;
    flags := pFld0510^.flags;
    decimals := pFld0510^.decimals;
    charsetnr := pFld0510^.charsetnr;
  end
  else if FResult.FOwner.FLib.Version >= mvMySQL040101 then
  begin
    pFld0410 := PMYSQL_FIELD0410(FpFld);
    name := pFld0410^.name;
    srcname := pFld0410^.org_name;
    table := pFld0410^.org_table;
    db := pFld0410^.db;
    type_ := pFld0410^.type_;
    length := pFld0410^.length;
    if pFld0410^.max_length > length then
      length := pFld0410^.max_length;
    flags := pFld0410^.flags;
    decimals := pFld0410^.decimals;
    charsetnr := pFld0410^.charsetnr;
  end
  else if FResult.FOwner.FLib.Version >= mvMySQL040100 then
  begin
    pFld0401 := PMYSQL_FIELD0401(FpFld);
    name := pFld0401^.name;
    srcname := pFld0401^.org_name;
    table := pFld0401^.org_table;
    db := pFld0401^.db;
    type_ := pFld0401^.type_;
    length := pFld0401^.length;
    if pFld0401^.max_length > length then
      length := pFld0401^.max_length;
    flags := pFld0401^.flags;
    decimals := pFld0401^.decimals;
    charsetnr := pFld0401^.charsetnr;
  end
  else if FResult.FOwner.FLib.Version >= mvMySQL040000 then
  begin
    pFld0400 := PMYSQL_FIELD0400(FpFld);
    name := pFld0400^.name;
    srcname := name;
    table := pFld0400^.org_table;
    db := pFld0400^.db;
    type_ := pFld0400^.type_;
    length := pFld0400^.length;
    if pFld0400^.max_length > length then
      length := pFld0400^.max_length;
    flags := pFld0400^.flags;
    decimals := pFld0400^.decimals;
    charsetnr := 0;
  end
  else begin
    pFld0320 := PMYSQL_FIELD0320(FpFld);
    name := pFld0320^.name;
    srcname := name;
    table := pFld0320^.table;
    db := nil;
    type_ := pFld0320^.type_;
    length := pFld0320^.length;
    if pFld0320^.max_length > length then
      length := pFld0320^.max_length;
    flags := pFld0320^.flags;
    decimals := pFld0320^.decimals;
    charsetnr := 0;
  end;
end;

{ TMySqlStatement }

constructor TMySqlStatement.Create(AOwner: TMySqlStatements; AOwnerDB: TMySQLDataBase);
begin
  FOwner := AOwner;
  FOwnerDB := AOwnerDB;

  FHandle := nil;
  FAutoOrderIndex := 0;

  if FOwnerDB <> nil then
  begin
    FHandle := FOwnerDB.FLib.mysql_stmt_init(AOwnerDB.FPMySQL);
  end;

  FParamCount := 0;
  FParamValues := TList.Create;
  FParamValues.Capacity := 50;

  FBinds := nil;
  FBindSize := 0;

  if FOwnerDB.FLib.Version >= mvMySQL050100 then
    FStatementRecordSize := SizeOf(MYSQL_BIND0510)
  else if FOwnerDB.FLib.Version >= mvMySQL050006 then
    FStatementRecordSize := SizeOf(MYSQL_STMT0506)
  else if FOwnerDB.FLib.Version >= mvMySQL041100 then
    FStatementRecordSize := SizeOf(MYSQL_STMT0411)
  else
    FStatementRecordSize := SizeOf(MYSQL_STMT0410);

  FNullValue := 1;

  FResult := nil;
  FResultFieldCount := 0;
  FRow := nil;
  FFetchLengths := 0;
end;

procedure TMySqlStatement.Close;
begin
  FOwnerDB.FLib.mysql_stmt_free_result(FHandle);
end;

destructor TMySqlStatement.Destroy;
var
  I: Integer;
  BindValue: PMySqlStatementBindValue;
begin
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    FOwnerDB.FLib.mysql_stmt_close(FHandle);
    FHandle := nil;
  end;

  for I := 0 to FParamValues.Count - 1 do
  begin
    BindValue := FParamValues.Items[I];
    if BindValue.Buf <> nil then
    begin
      FreeMem(BindValue.Buf, BindValue.BufLen);
      BindValue.Buf := nil;
      BindValue.BufLen := 0;
    end;
  end;

  FParamValues.Free;

  inherited;
end;

function TMySqlStatement.Prepare: Boolean;
var
  PF: PAnsiChar;
  Ret, I, Count, _BindSize: Integer;
  S: AnsiString;

  BindValue: PMySqlStatementBindValue;

  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
begin
  FAutoOrderIndex := 0;
  if Length(FSql) = 0 then
  begin
    raise Exception.CreateFmt('TSQLStatement %s sql can not be empty.', [FName]);
  end;

  Result := False;
  if FHandle <> nil then
  begin
    S := FSql;
    if (FOwnerDB.FEncoding = ecUTF8) then
    begin
      S := AnsiToUtf8(FSql);
    end;
    PF := PAnsiChar(@S[1]);

    Ret := FOwnerDB.FLib.mysql_stmt_prepare(FHandle, PF, Length(S));
    Result := Ret = MYSQL_SUCCESS;
    if not Result then
      FOwnerDB.Check(Ret)
    else
    begin
      FParamCount := FOwnerDB.FLib.mysql_stmt_param_count(FHandle);
      if FParamValues.Count < FParamCount then
      begin
        Count := FParamCount - FParamValues.Count;
        for I := 0 to Count - 1 do
        begin
          New(BindValue);
          BindValue.BufLen := 4;
          GetMem(BindValue.Buf, BindValue.BufLen);
          FParamValues.Add(BindValue);
        end;
      end;

      SetLength(FParamLength, FParamCount);

      if FParamCount = 0 then
      begin
        if FBinds <> nil then
        begin
          FreeMem(FBinds);
          FBinds := nil;
          FBindSize := 0;
        end;

        Exit;
      end;

      _BindSize := FStatementRecordSize * FParamCount;
      if FBindSize <> _BindSize then
      begin
        FBindSize := _BindSize;

        if FBinds = nil then
          GetMem(FBinds, FBindSize)
        else
          ReallocMem(FBinds, FBindSize);

        FillChar(FBinds^, FBindSize, 0);
      end;

      for I := 0 to FParamCount - 1 do
      begin
        GetParamBindInfo(I, BufType, Buf, BufLen, Len, IsNull, BindValue);

        BufType^ := MYSQL_TYPE_DECIMAL;
        Len^ := @FParamLength[I];
        BufLen^ := 0;
        Buf^ := BindValue.Buf;
      end;
    end;
  end;

end;

function TMySqlStatement.Reset: Boolean;
begin
  FAutoOrderIndex := 0;
  Result := True;
  {
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := FOwnerDB.FLib.mysql_stmt_reset(FHandle);
    Result := Ret = MYSQL_SUCCESS;
    if not Result then FOwnerDB.Check(Ret);
  end;
  }
end;

function TMySqlStatement.BindBool(const Index: Integer;
  const Value: Boolean): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_TINY;

  // Set buffer
  if BindValue.BufLen < SizeOf(Value) then
  begin
    BindValue.BufLen := Max(SizeOf(Value), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  Move(Value, BindValue.Buf^, SizeOf(Value));

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Value);    // Len^^ = FParamLength[Index]

  IsNull^ := nil;

  Result := True;
end;

function TMySqlStatement.BindDouble(const Index: Integer;
  const Value: Double): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_DOUBLE;

  // Set buffer
  if BindValue.BufLen < SizeOf(Value) then
  begin
    BindValue.BufLen := Max(SizeOf(Value), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  Move(Value, BindValue.Buf^, SizeOf(Value));

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Value);    // Len^^ = FParamLength[Index]

  IsNull^ := nil;

  Result := True;
end;

function TMySqlStatement.BindInt(const Index, Value: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  // Set buffer
  if BindValue.BufLen < SizeOf(Value) then
  begin
    BindValue.BufLen := Max(SizeOf(Value), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  Move(Value, BindValue.Buf^, SizeOf(Value));

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Value);    // Len^^ = FParamLength[Index]

  IsNull^ := nil;

  Result := True;
end;

function TMySqlStatement.BindInt64(const Index: Integer;
  const Value: Int64): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONGLONG;

  // Set buffer
  if BindValue.BufLen < SizeOf(Value) then
  begin
    BindValue.BufLen := Max(SizeOf(Value), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  Move(Value, BindValue.Buf^, SizeOf(Value));

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Value);    // Len^^ = FParamLength[Index]

  IsNull^ := nil;

  Result := True;
end;

function TMySqlStatement.BindNull(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := 0;    // Len^^ = FParamLength[Index]

  IsNull^ := @FNullValue;

  Result := True;
end;

function DateTimeToSQLTimeStamp(const DateTime: TDateTime): TSQLTimeStamp;
var
  F: Word;
begin
  DecodeDate(DateTime, Result.Year, Result.Month, Result.Day);
  DecodeTime(DateTime, Result.Hour, Result.Minute, Result.Second, F);
  Result.Fractions := F;
end;

function TMySqlStatement.BindDateTime(const Index: Integer;
  const Value: TDateTime): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;
  TimeStamp: TSQLTimeStamp;
  TIME0506: MYSQL_TIME0506;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  TimeStamp := DateTimeToSQLTimeStamp(Value);

  TIME0506.year := TimeStamp.Year;
  TIME0506.month := TimeStamp.Month;
  TIME0506.day := TimeStamp.day;
  TIME0506.hour := TimeStamp.hour;
  TIME0506.minute := TimeStamp.minute;
  TIME0506.second := TimeStamp.second;
  TIME0506.second_part := TimeStamp.Fractions * 1000;
  TIME0506.neg := 0;
  TIME0506.time_type := MYSQL_TIMESTAMP_DATETIME;

  // Set buffer_type
  BufType^ := MYSQL_TYPE_DATETIME;

  // Set buffer
  if BindValue.BufLen < SizeOf(MYSQL_TIME0506) then
  begin
    BindValue.BufLen := Max(SizeOf(MYSQL_TIME0506), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  Move(TIME0506, BindValue.Buf^, SizeOf(TIME0506));

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(TIME0506);    // Len^^ = FParamLength[Index]

  IsNull^ := nil;

  Result := True;
end;

function TMySqlStatement.BindText(const Index: Integer;
  const Value: AnsiString): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;

  BindValue: PMySqlStatementBindValue;

  S: AnsiString;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  if (FOwnerDB.FEncoding = ecUTF8) then
    S := AnsiToUtf8(Value)
  else
    S := Value;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_STRING;

  // Set buffer
  if BindValue.BufLen < Length(S) then
  begin
    BindValue.BufLen := Max(Length(S), 80);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  Move(S[1], BindValue.Buf^, Length(S));

  // set buffer_length
  BufLen^ := Length(S);

  //Set length
  Len^^ := Length(S);

  IsNull^ := nil;

  Result := True;
end;

function TMySqlStatement.Finalize: Boolean;
var
  I: Integer;
  BindValue: PMySqlStatementBindValue;
begin
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    FOwnerDB.FLib.mysql_stmt_close(FHandle);
    FHandle := nil;

    Result := True;
  end;

  {
  for I := 0 to FParamValues.Count - 1 do
  begin
    BindValue := FParamValues.Items[I];
    if BindValue.Buf <> nil then
    begin
      FreeMem(BindValue.Buf, BindValue.BufLen);
      BindValue.Buf := nil;
      BindValue.BufLen := 0;
    end;
  end;

  FParamValues.Free;
  }
end;

function TMySqlStatement.GetBindParamCount: Integer;
begin
  Result := FParamCount;
end;

procedure TMySqlStatement.GetParamBindInfo(ParamIndex: Integer;
  var BufType: Pmy_long; var Buf: PPByte; var BufLen: Pmy_ulong;
  var Len: PPmy_ulong; var IsNull: PPmy_bool;
  var BindValue: PMySqlStatementBindValue);
var
  BindPtr: Pointer;

  BIND0510: PMYSQL_BIND0510;
  BIND0506: PMYSQL_BIND0506;
  BIND0411: PMYSQL_BIND0411;
  BIND0410: PMYSQL_BIND0410;
begin
  BindValue := FParamValues.Items[ParamIndex];

  BindPtr := FBinds;
  NativeUInt(BindPtr) := NativeUInt(BindPtr) + ParamIndex * FStatementRecordSize;

  if FOwnerDB.FLib.Version >= mvMySQL050100 then
  begin
    BIND0510 := PMYSQL_BIND0510(BindPtr);

    BufType := @BIND0510^.buffer_type;
    Buf := @BIND0510^.buffer;
    BufLen := @BIND0510^.buffer_length;
    Len := @BIND0510^.length;
    IsNull := @BIND0510^.is_null;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL050006 then
  begin
    BIND0506 := PMYSQL_BIND0506(BindPtr);

    BufType := @BIND0506^.buffer_type;
    Buf := @BIND0506^.buffer;
    BufLen := @BIND0506^.buffer_length;
    Len := @BIND0506^.length;
    IsNull := @BIND0506^.is_null;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL041100 then
  begin
    BIND0411 := PMYSQL_BIND0411(BindPtr);

    BufType := @BIND0411^.buffer_type;
    Buf := @BIND0411^.buffer;
    BufLen := @BIND0411^.buffer_length;
    Len := @BIND0411^.length;
    IsNull := @BIND0411^.is_null;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL041000 then
  begin
    BIND0410 := PMYSQL_BIND0410(BindPtr);

    BufType := @BIND0410^.buffer_type;
    Buf := @BIND0410^.buffer;
    BufLen := @BIND0410^.buffer_length;
    Len := @BIND0410^.length;
    IsNull := @BIND0410^.is_null;
  end
  else
    ASSERT(False);
end;

function TMySqlStatement.GetData(AIndex: Integer; var ApData: Pointer;
  var ALen: LongWord): Boolean;
begin
  ALen := Pmy_ulong(NativeUInt(FFetchLengths) + NativeUInt(AIndex) * SizeOf(my_ulong))^;
  ApData := PPByte(NativeUInt(FRow) + NativeUInt(AIndex) * SizeOf(PByte))^;
  Result := ApData <> nil;
end;

function TMySqlStatement.GetValues(AIndex: Integer): string;
var
  pData: Pointer;
  DataLen: Cardinal;
  S: AnsiString;
begin
  Result := '';
  if GetData(AIndex, pData, DataLen) then
  begin
    SetLength(S, DataLen);
    Move(pData^, S[1], DataLen);

    // iLen := MultiByteToWideChar(CP_UTF8, 0, pData, iLen, PWideChar(@s2[1]), Length(s2));
    if FOwnerDB.FEncoding = ecUTF8 then
      Result := Utf8Decode(S)
    else
      Result := S;
  end;
end;

function TMySqlStatement.OrderBindBool(const Value: Boolean): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindBool(FAutoOrderIndex, Value);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.OrderBindDouble(const Value: Double): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindDouble(FAutoOrderIndex, Value);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.OrderBindDateTime(const Value: TDateTime): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindDateTime(FAutoOrderIndex, Value);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.OrderBindInt(const Value: Integer): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindInt(FAutoOrderIndex, Value);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.OrderBindInt64(const Value: Int64): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindInt64(FAutoOrderIndex, Value);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.OrderBindNull: Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindNull(FAutoOrderIndex);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.OrderBindText(const Value: AnsiString): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindText(FAutoOrderIndex, Value);
    Inc(FAutoOrderIndex);
  end;
end;

function TMySqlStatement.Step: Boolean;
begin
  Result := False;
  if (FHandle <> nil) and (FOwnerDB <> nil) then
  begin
    FOwnerDB.ClearResult;

    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_bind_param(FHandle, FBinds));
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_execute(FHandle));

    Result := True;
  end;
end;

function TMySqlStatement.Query: Boolean;
begin
  Result := False;
  if (FHandle <> nil) and (FOwnerDB <> nil) then
  begin
    FResult := nil;
    FResultFieldCount := 0;
    FRow := nil;
    FFetchLengths := 0;
    FOwnerDB.ClearResult;

    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_bind_param(FHandle, FBinds));
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_execute(FHandle));

    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_store_result(FHandle));
    FResult := FOwnerDB.FLib.mysql_stmt_result_metadata(FHandle);

    Result := FResult <> nil;
    if Result then
      FResultFieldCount := FOwnerDB.FLib.mysql_num_fields(FResult);
  end;
end;

function TMySqlStatement.Fetch: Boolean;
begin
  FRow := FOwnerDB.FLib.mysql_fetch_row(FResult);
  Result := (FRow <> nil);
  if Result then
  begin
    FFetchLengths := FOwnerDB.FLib.mysql_fetch_lengths(FResult);
    if FFetchLengths = nil then FOwnerDB.Check;
  end
  else
  begin
    FOwnerDB.Check;
  end;
end;

{ TMySqlStatements }

constructor TMySqlStatements.Create(AOwner: TMySQLDataBase);
begin
  inherited Create;
  FOwner := AOwner;
  FList := TList.Create;
end;

destructor TMySqlStatements.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TMySqlStatements.AddSQLStatement(Name: string): TMySqlStatement;
var
  Index: Integer;
begin
  if not Search(Name, Index) then
  begin
    Result := TMySqlStatement.Create(self, FOwner);
    Result.FName := Name;
    Result.FOwner := Self;
    Result.FOwnerDB := FOwner;
    FList.Insert(Index, Result);
  end
  else
  begin
    Result := FList.Items[Index];
  end;
end;

function TMySqlStatements.Find(Name: string): TMySqlStatement;
var
  Index: Integer;
begin
  if Search(Name, Index) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

function TMySqlStatements.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TMySqlStatements.GetItems(Index: Integer): TMySqlStatement;
begin
  Result := nil;
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index];
end;

procedure TMySqlStatements.Delete(Index: Integer);
begin
  if (Index >= 0) and (Index < FList.Count) then
  begin
    TMySqlStatement(FList.Items[Index]).Free;
    FList.Delete(Index);
  end;
end;

procedure TMySqlStatements.Remove(Name: string);
var
  Index: Integer;
begin
  if Search(Name, Index) then
  begin
    TMySqlStatement(FList.Items[Index]).Free;
    FList.Delete(Index);
  end;
end;

procedure TMySqlStatements.Clear;
var
  I: Integer;
  Statement: TMySqlStatement;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Statement := FList.Items[I];
    Statement.Free;
  end;
  FList.Clear;
end;

function TMySqlStatements.Search(Name: string; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Search := False;
  L := 0;
  H := Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := AnsiCompareText(TMySqlStatement(FList.Items[I]).FName, Name);
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
