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

    FOnRequest: TNotifyEvent;

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
  protected
    procedure DoRequest;
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

    property OnRequest: TNotifyEvent read FOnRequest write FOnRequest;

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

  TMySqlStatementBindType = (bt_Unknow, bt_Short, bt_UShort, bt_Int, bt_UInt, bt_Int64, bt_UInt64, bt_Bool, bt_Double, bt_DateTime, bt_Text);
  PMySqlStatementBindValue = ^TMySqlStatementBindValue;
  TMySqlStatementBindValue = record
    BufType: TMySqlStatementBindType;
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

    FStatementRecordSize: Integer;

    // Param 是指输入参数
    FAutoParamIndex: Integer;

    FParamNullValue: Byte;
    FParamBinds: PMYSQL_BIND;
    FParamBindSize: Cardinal;
    FParamCount: Integer;
    FParamValues: TList;
    FParamLength: array of Cardinal;

    // Field 是指输出字段
    FIsBindResultFields: Boolean;
    FFieldBinds: PMYSQL_BIND;
    FFieldBindSize: Cardinal;
    FFieldCount: Integer;
    FFieldValues: TList;
    FFieldLength: array of Cardinal;
    FFieldNullValues: array of Boolean;

    FResult: PMYSQL_RES;
    FRowCount: Integer;

    procedure GetParamBindInfo(ParamIndex: Integer;
      var BufType: Pmy_long; var Buf: PPByte; var BufLen: Pmy_ulong;
      var Len: PPmy_ulong; var IsNull: PPmy_bool; var IsUnsigned: PByte;
      var BindValue: PMySqlStatementBindValue);

    procedure GetFieldBindInfo(FieldIndex: Integer;
      var BufType: Pmy_long; var Buf: PPByte; var BufLen: Pmy_ulong;
      var Len: PPmy_ulong; var IsNull: PPmy_bool; var IsUnsigned: PByte;
      var BindValue: PMySqlStatementBindValue);

    procedure GetResultFieldInfo(Field: PMYSQL_FIELD; var name, srcname, table, db: my_pchar;
      var type_: Byte; var length, flags, decimals, charsetnr: LongWord);

    function BindFieldShort(const Index: Integer): Boolean;
    function BindFieldUShort(const Index: Integer): Boolean;
    function BindFieldInt(const Index: Integer): Boolean;
    function BindFieldUInt(const Index: Integer): Boolean;
    function BindFieldInt64(const Index: Integer): Boolean;
    function BindFieldUInt64(const Index: Integer): Boolean;
    function BindFieldBool(const Index: Integer): Boolean;
    function BindFieldDouble(const Index: Integer): Boolean;
    function BindFieldDateTime(const Index: Integer): Boolean;
    function BindFieldText(const Index: Integer; const StrLen: Integer): Boolean;

    procedure BindAllResultFields;
  public
    constructor Create(AOwner: TMySqlStatements; AOwnerDB: TMySQLDataBase);
    destructor Destroy; override;

    property Owner: TMySqlStatements read FOwner;
    property Name: string read FName;
    property Sql: string read FSql write FSql;
    property Handle: PMYSQL_STMT read FHandle;
    property ParamCount: Integer read FParamCount;        // 输入参数数量
    property FieldCount: Integer read FFieldCount;        // 字段数量
    property RowCount: Integer read FRowCount;            // 结果行数


    function Prepare: Boolean;
    function Reset: Boolean;
    function Step: Boolean;
    function Query: Boolean;
    function Fetch: Boolean;
    function Finalize: Boolean;
    function GetAffectedRows: Integer;

    procedure Close;

    function BindParamNull(const Index: Integer): Boolean;
    function BindParamInt(const Index: Integer; const Value: Integer): Boolean;
    function BindParamUInt(const Index: Integer; const Value: DWORD): Boolean;
    function BindParamInt64(const Index: Integer; const Value: Int64): Boolean;
    function BindParamBool(const Index: Integer; const Value: Boolean): Boolean;
    function BindParamDouble(const Index: Integer; const Value: Double): Boolean;
    function BindParamDateTime(const Index: Integer; const Value: TDateTime): Boolean;
    function BindParamText(const Index: Integer; const Value: AnsiString): Boolean;


    // BindParamValue Index 是从1开始，先增加再使用
    function OrderBindParamNull(): Boolean;
    function OrderBindParamInt(const Value: Integer): Boolean;
    function OrderBindParamUInt(const Value: DWORD): Boolean;
    function OrderBindParamInt64(const Value: Int64): Boolean;
    function OrderBindParamBool(const Value: Boolean): Boolean;
    function OrderBindParamDouble(const Value: Double): Boolean;
    function OrderBindParamDateTime(const Value: TDateTime): Boolean;
    function OrderBindParamText(const Value: AnsiString): Boolean;

    function GetColumnValueInt(const Index: Integer): Integer;
    function GetColumnValueUInt(const Index: Integer): DWORD;
    function GetColumnValueInt64(const Index: Integer): Int64;
    function GetColumnValueBool(const Index: Integer): Boolean;
    function GetColumnValueDouble(const Index: Integer): Double;
    function GetColumnValueDateTime(const Index: Integer): TDateTime;
    function GetColumnValueText(const Index: Integer): AnsiString;

    function OrderGetColumnValueInt: Integer;
    function OrderGetColumnValueUInt: DWORD;
    function OrderGetColumnValueInt64: Int64;
    function OrderGetColumnValueBool: Boolean;
    function OrderGetColumnValueDouble: Double;
    function OrderGetColumnValueDateTime: TDateTime;
    function OrderGetColumnValueText: AnsiString;
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
//    Disconnect;
//    Init;
//    Connect(FHost, FUser, FPwd, FCurrDB, FPort, FFlags);
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

  {
  if FPMySQL <> nil then
  begin
    OutputDebugstring(PChar(CharacterSetName));
  end;
  }
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
    Result := AnsiChar(AChar) in ASet
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

  ClearResult;

  DoRequest;
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

  DoRequest;
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

procedure TMySQLDataBase.DoRequest;
begin
  if Assigned(FOnRequest) then
    FOnRequest(Self);
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
  ALen := Pmy_ulong(IntPtr(FpLengths) + IntPtr(AIndex) * SizeOf(my_ulong))^;
  ApData := PPByte(IntPtr(FpRow) + IntPtr(AIndex) * SizeOf(PByte))^;
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
  if FOwnerDB <> nil then
  begin
    FHandle := FOwnerDB.FLib.mysql_stmt_init(AOwnerDB.FPMySQL);
  end;

  FAutoParamIndex := 0;
  FParamBinds := nil;
  FParamBindSize := 0;
  FParamCount := 0;
  FParamValues := TList.Create;
  FParamValues.Capacity := 50;

  FIsBindResultFields := False;
  FFieldBinds := nil;
  FFieldBindSize := 0;
  FFieldCount := 0;
  FFieldValues := TList.Create;
  FFieldValues.Capacity := 50;

  if FOwnerDB.FLib.Version >= mvMySQL050100 then
    FStatementRecordSize := SizeOf(MYSQL_BIND0510)
  else if FOwnerDB.FLib.Version >= mvMySQL050006 then
    FStatementRecordSize := SizeOf(MYSQL_STMT0506)
  else if FOwnerDB.FLib.Version >= mvMySQL041100 then
    FStatementRecordSize := SizeOf(MYSQL_STMT0411)
  else
    FStatementRecordSize := SizeOf(MYSQL_STMT0410);

  FParamNullValue := 1;

  FResult := nil;

  FRowCount := 0;
end;

procedure TMySqlStatement.Close;
begin
  FOwnerDB.FLib.mysql_stmt_free_result(FHandle);
  FRowCount := 0;
  FIsBindResultFields := False;
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

    Dispose(BindValue);
  end;
  FParamValues.Free;

  if FParamBinds <> nil then
  begin
    FreeMem(FParamBinds);
    FParamBinds := nil;
    FParamBindSize := 0;
  end;

  for I := 0 to FFieldValues.Count - 1 do
  begin
    BindValue := FFieldValues.Items[I];
    if BindValue.Buf <> nil then
    begin
      FreeMem(BindValue.Buf, BindValue.BufLen);
      BindValue.Buf := nil;
      BindValue.BufLen := 0;
    end;
    Dispose(BindValue);
  end;
  FFieldValues.Free;

  FIsBindResultFields := False;
  if FFieldBinds <> nil then
  begin
    FreeMem(FFieldBinds);
    FFieldBinds := nil;
    FFieldBindSize := 0;
  end;

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
  IsUnsigned: PByte;
begin
  FAutoParamIndex := 0;
  FIsBindResultFields := False;

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
        if FParamBinds <> nil then
        begin
          FreeMem(FParamBinds);
          FParamBinds := nil;
          FParamBindSize := 0;
        end;
      end
      else
      begin
        _BindSize := FStatementRecordSize * FParamCount;
        if FParamBindSize <> _BindSize then
        begin
          FParamBindSize := _BindSize;

          if FParamBinds = nil then
            GetMem(FParamBinds, FParamBindSize)
          else
            ReallocMem(FParamBinds, FParamBindSize);

          FillChar(FParamBinds^, FParamBindSize, 0);
        end;

        for I := 0 to FParamCount - 1 do
        begin
          GetParamBindInfo(I, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

          BufType^ := MYSQL_TYPE_DECIMAL;
          Len^ := @FParamLength[I];
          BufLen^ := 0;
          Buf^ := BindValue.Buf;
          IsUnsigned^ := 0;
        end;
      end;

      FFieldCount := FOwnerDB.FLib.mysql_stmt_field_count(FHandle);

      if FFieldValues.Count < FFieldCount then
      begin
        Count := FFieldCount - FFieldValues.Count;
        for I := 0 to Count - 1 do
        begin
          New(BindValue);
          BindValue.BufLen := 4;
          GetMem(BindValue.Buf, BindValue.BufLen);
          FFieldValues.Add(BindValue);
        end;
      end;

      SetLength(FFieldLength, FFieldCount);
      SetLength(FFieldNullValues, FFieldCount);

      if FFieldCount = 0 then
      begin
        if FFieldBinds <> nil then
        begin
          FreeMem(FFieldBinds);
          FFieldBinds := nil;
          FFieldBindSize := 0;
        end;
      end
      else
      begin
        _BindSize := FStatementRecordSize * FFieldCount;
        if FFieldBindSize <> _BindSize then
        begin
          FFieldBindSize := _BindSize;

          if FFieldBinds = nil then
            GetMem(FFieldBinds, FFieldBindSize)
          else
            ReallocMem(FFieldBinds, FFieldBindSize);

          FillChar(FFieldBinds^, FFieldBindSize, 0);
        end;

        for I := 0 to FFieldCount - 1 do
        begin
          GetFieldBindInfo(I, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

          BindValue.BufType := bt_Unknow;

          BufType^ := MYSQL_TYPE_DECIMAL;
          Buf^ := BindValue.Buf;
          BufLen^ := 0;
          Len^ := @FFieldLength[I];
          IsNull^ := @FFieldNullValues[I];

          if IsUnsigned <> nil then
            IsUnsigned^ := 0;
        end;
      end;
    end;
  end;

  FRowCount := 0;
end;

function TMySqlStatement.Reset: Boolean;
begin
  FAutoParamIndex := 0;
  Result := True;

//  OutputDebugString(FOwnerDB.FLib.mysql_error(FHandle));

  if (FOwnerDB <> nil) and (FResult <> nil) then
  begin
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_free_result(FHandle));
    FResult := nil;
  end;

  FRowCount := 0;
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

function TMySqlStatement.Finalize: Boolean;
begin
  FAutoParamIndex := 0;
  Result := True;

  if (FOwnerDB <> nil) and (FResult <> nil) then
  begin
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_free_result(FHandle));
    FResult := nil;
  end;

  FRowCount := 0;
end;

procedure TMySqlStatement.GetParamBindInfo(ParamIndex: Integer;
  var BufType: Pmy_long; var Buf: PPByte; var BufLen: Pmy_ulong;
  var Len: PPmy_ulong; var IsNull: PPmy_bool; var IsUnsigned: PByte;
  var BindValue: PMySqlStatementBindValue);
var
  BindPtr: Pointer;

  BIND0510: PMYSQL_BIND0510;
  BIND0506: PMYSQL_BIND0506;
  BIND0411: PMYSQL_BIND0411;
  BIND0410: PMYSQL_BIND0410;
begin
  BindValue := FParamValues.Items[ParamIndex];

  BindPtr := FParamBinds;
  BindPtr := Pointer(IntPtr(BindPtr) + ParamIndex * FStatementRecordSize); //IntPtr(BindPtr) := IntPtr(BindPtr) + ParamIndex * FStatementRecordSize;

  if FOwnerDB.FLib.Version >= mvMySQL050100 then
  begin
    BIND0510 := PMYSQL_BIND0510(BindPtr);

    BufType := @BIND0510^.buffer_type;
    Buf := @BIND0510^.buffer;
    BufLen := @BIND0510^.buffer_length;
    Len := @BIND0510^.length;
    IsNull := @BIND0510^.is_null;
    IsUnsigned := @BIND0510^.is_unsigned;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL050006 then
  begin
    BIND0506 := PMYSQL_BIND0506(BindPtr);

    BufType := @BIND0506^.buffer_type;
    Buf := @BIND0506^.buffer;
    BufLen := @BIND0506^.buffer_length;
    Len := @BIND0506^.length;
    IsNull := @BIND0506^.is_null;
    IsUnsigned := @BIND0506^.is_unsigned;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL041100 then
  begin
    BIND0411 := PMYSQL_BIND0411(BindPtr);

    BufType := @BIND0411^.buffer_type;
    Buf := @BIND0411^.buffer;
    BufLen := @BIND0411^.buffer_length;
    Len := @BIND0411^.length;
    IsNull := @BIND0411^.is_null;
    IsUnsigned := @BIND0411^.is_unsigned;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL041000 then
  begin
    BIND0410 := PMYSQL_BIND0410(BindPtr);

    BufType := @BIND0410^.buffer_type;
    Buf := @BIND0410^.buffer;
    BufLen := @BIND0410^.buffer_length;
    Len := @BIND0410^.length;
    IsNull := @BIND0410^.is_null;
    IsUnsigned := nil;
  end
  else
    ASSERT(False);
end;

procedure TMySqlStatement.GetFieldBindInfo(FieldIndex: Integer;
  var BufType: Pmy_long; var Buf: PPByte; var BufLen: Pmy_ulong;
  var Len: PPmy_ulong; var IsNull: PPmy_bool; var IsUnsigned: PByte;
  var BindValue: PMySqlStatementBindValue);
var
  BindPtr: Pointer;

  BIND0510: PMYSQL_BIND0510;
  BIND0506: PMYSQL_BIND0506;
  BIND0411: PMYSQL_BIND0411;
  BIND0410: PMYSQL_BIND0410;
begin
  BindValue := FFieldValues.Items[FieldIndex];

  BindPtr := FFieldBinds;
  BindPtr := Pointer(IntPtr(BindPtr) + FieldIndex * FStatementRecordSize); //IntPtr(BindPtr) := IntPtr(BindPtr) + FieldIndex * FStatementRecordSize;

  if FOwnerDB.FLib.Version >= mvMySQL050100 then
  begin
    BIND0510 := PMYSQL_BIND0510(BindPtr);

    BufType := @BIND0510^.buffer_type;
    Buf := @BIND0510^.buffer;
    BufLen := @BIND0510^.buffer_length;
    Len := @BIND0510^.length;
    IsNull := @BIND0510^.is_null;
    IsUnsigned := @BIND0510^.is_unsigned;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL050006 then
  begin
    BIND0506 := PMYSQL_BIND0506(BindPtr);

    BufType := @BIND0506^.buffer_type;
    Buf := @BIND0506^.buffer;
    BufLen := @BIND0506^.buffer_length;
    Len := @BIND0506^.length;
    IsNull := @BIND0506^.is_null;
    IsUnsigned := @BIND0506^.is_unsigned;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL041100 then
  begin
    BIND0411 := PMYSQL_BIND0411(BindPtr);

    BufType := @BIND0411^.buffer_type;
    Buf := @BIND0411^.buffer;
    BufLen := @BIND0411^.buffer_length;
    Len := @BIND0411^.length;
    IsNull := @BIND0411^.is_null;
    IsUnsigned := @BIND0411^.is_unsigned;
  end
  else if FOwnerDB.FLib.Version >= mvMySQL041000 then
  begin
    BIND0410 := PMYSQL_BIND0410(BindPtr);

    BufType := @BIND0410^.buffer_type;
    Buf := @BIND0410^.buffer;
    BufLen := @BIND0410^.buffer_length;
    Len := @BIND0410^.length;
    IsNull := @BIND0410^.is_null;
    IsUnsigned := nil;
  end
  else
    ASSERT(False);
end;

procedure TMySqlStatement.GetResultFieldInfo(Field: PMYSQL_FIELD; var name, srcname, table, db: my_pchar;
  var type_: Byte; var length, flags, decimals, charsetnr: LongWord);
var
  pFld0510: PMYSQL_FIELD0510;
  pFld0410: PMYSQL_FIELD0410;
  pFld0401: PMYSQL_FIELD0401;
  pFld0400: PMYSQL_FIELD0400;
  pFld0320: PMYSQL_FIELD0320;
begin
  if FOwnerDB.FLib.Version >= mvMySQL050100 then
  begin
    pFld0510 := PMYSQL_FIELD0510(Field);
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
  else if FOwnerDB.FLib.Version >= mvMySQL040101 then
  begin
    pFld0410 := PMYSQL_FIELD0410(Field);
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
  else if FOwnerDB.FLib.Version >= mvMySQL040100 then
  begin
    pFld0401 := PMYSQL_FIELD0401(Field);
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
  else if FOwnerDB.FLib.Version >= mvMySQL040000 then
  begin
    pFld0400 := PMYSQL_FIELD0400(Field);
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
    pFld0320 := PMYSQL_FIELD0320(Field);
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


function TMySqlStatement.BindParamBool(const Index: Integer;
  const Value: Boolean): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_TINY;

  BindValue.BufType := bt_Bool;

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

  IsUnsigned^ := 0;

  Result := True;
end;

function TMySqlStatement.BindParamDouble(const Index: Integer;
  const Value: Double): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_DOUBLE;

  BindValue.BufType := bt_Double;

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

  IsUnsigned^ := 0;

  Result := True;
end;

function TMySqlStatement.BindParamInt(const Index, Value: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  BindValue.BufType := bt_Int;

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

  IsUnsigned^ := 0;

  Result := True;
end;

function TMySqlStatement.BindParamUInt(const Index: Integer; const Value: DWORD): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  BindValue.BufType := bt_Int;

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

  IsUnsigned^ := 1;

  Result := True;
end;

function TMySqlStatement.BindParamInt64(const Index: Integer;
  const Value: Int64): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONGLONG;

  BindValue.BufType := bt_Int64;

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

  IsUnsigned^ := 0;

  Result := True;
end;

function TMySqlStatement.BindParamNull(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  BindValue.BufType := bt_Unknow;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := 0;    // Len^^ = FParamLength[Index]

  IsNull^ := @FParamNullValue;
  IsUnsigned^ := 0;

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

function TMySqlStatement.BindParamDateTime(const Index: Integer;
  const Value: TDateTime): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
  TimeStamp: TSQLTimeStamp;
  TIME0506: MYSQL_TIME0506;
begin
  if Index >= FParamCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

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

  BindValue.BufType := bt_DateTime;

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

  IsUnsigned^ := 0;

  Result := True;
end;

function TMySqlStatement.BindParamText(const Index: Integer;
  const Value: AnsiString): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

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

  GetParamBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_STRING;

  BindValue.BufType := bt_DateTime;

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

  IsUnsigned^ := 0;

  Result := True;
end;

function TMySqlStatement.OrderBindParamBool(const Value: Boolean): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamBool(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamDouble(const Value: Double): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamDouble(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamDateTime(const Value: TDateTime): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamDateTime(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamInt(const Value: Integer): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamInt(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamUInt(const Value: DWORD): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamUInt(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamInt64(const Value: Int64): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamInt64(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamNull: Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamNull(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderBindParamText(const Value: AnsiString): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := BindParamText(FAutoParamIndex, Value);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.Step: Boolean;
begin
  FRowCount := 0;
  Result := False;
  if (FHandle <> nil) and (FOwnerDB <> nil) then
  begin
    FOwnerDB.ClearResult;

    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_bind_param(FHandle, FParamBinds));
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_execute(FHandle));

    Result := True;
  end;

  if FOwnerDB <> nil then
  begin
    FOwnerDB.FLib.mysql_stmt_free_result(FHandle);
    FOwnerDB.DoRequest;
  end;
end;

function TMySqlStatement.GetAffectedRows: Integer;
begin
  // 注意，当使用 UPDATE 查询，MySQL 不会将原值与新值一样的列更新，当查询有结果而无任何字段更新时，返回的是0
  Result := FOwnerDB.FLib.mysql_stmt_affected_rows(Handle);
end;

procedure TMySqlStatement.BindAllResultFields;
var
  I: Integer;
  Field: PMYSQL_FIELD;

  name, srcname, table, db: my_pchar;
  type_: Byte;
  len, flags, decimals, charsetnr: LongWord;
begin
  if FResult = nil then Exit;

  if FIsBindResultFields then Exit;

  for I := 0 to FFieldCount - 1 do
  begin
    Field := FOwnerDB.FLib.mysql_fetch_field_direct(FResult, I);
    if Field <> nil then
    begin
      GetResultFieldInfo(Field, name, srcName, table, db, type_, len, flags, decimals, charsetnr);

      if ((srcname = nil) or (srcname^ = AnsiChar(#0))) and
         (type_ = MYSQL_TYPE_VAR_STRING) and (flags and BINARY_FLAG <> 0) then begin
        flags := flags and not BINARY_FLAG;
        if FOwnerDB.FEncoding = ecUTF8 then
          len := len * 3;
      end;
      {
      pInfo^.FAttrs := [];
      if (flags and NOT_NULL_FLAG) = 0 then
        Include(pInfo^.FAttrs, caAllowNull);
      if (flags and AUTO_INCREMENT_FLAG) <> 0 then begin
        Include(pInfo^.FAttrs, caAutoInc);
        Include(pInfo^.FAttrs, caAllowNull);
      end;
      if (MyConnection.FServerVersion >= mvMySQL050200) and
         not (caAutoInc in pInfo^.FAttrs) and
         ((flags and NO_DEFAULT_VALUE_FLAG) = 0) then
        Include(pInfo^.FAttrs, caDefault);
      }

      case type_ of
        MYSQL_TYPE_BIT:
          BindFieldBool(I);

        MYSQL_TYPE_TINY:
          begin
            BindFieldBool(I);
          end;

        MYSQL_TYPE_SHORT:
          begin
            if (flags and UNSIGNED_FLAG) <> 0 then
              BindFieldUShort(I)
            else
              BindFieldShort(I)
          end;

        MYSQL_TYPE_LONG,
        MYSQL_TYPE_INT24:
          begin
            if (flags and UNSIGNED_FLAG) <> 0 then
              BindFieldUInt(I)
            else
              BindFieldInt(I)
          end;

        MYSQL_TYPE_LONGLONG:
          begin
            if (flags and UNSIGNED_FLAG) <> 0 then
              BindFieldUInt64(I)
            else
              BindFieldInt64(I)
          end;

        MYSQL_TYPE_FLOAT,
        MYSQL_TYPE_DOUBLE:
          begin
            BindFieldDouble(I);
          end;

        MYSQL_TYPE_DECIMAL,
        MYSQL_TYPE_NEWDECIMAL:
          begin
            raise Exception.Create('Unknow FileType ' + IntToStr(type_));
          end;

        MYSQL_TYPE_DATE,
        MYSQL_TYPE_NEWDATE:
          BindFieldDateTime(I);

        MYSQL_TYPE_TIME:
          BindFieldDateTime(I);

        MYSQL_TYPE_DATETIME:
          BindFieldDateTime(I);

        MYSQL_TYPE_YEAR:
          BindFieldInt(I);

        MYSQL_TYPE_TIMESTAMP:
          BindFieldDateTime(I);

        MYSQL_TYPE_ENUM,
        MYSQL_TYPE_SET,
        MYSQL_TYPE_VAR_STRING,
        MYSQL_TYPE_STRING,
        MYSQL_TYPE_VARCHAR:
          BindFieldText(I, Len);

        MYSQL_TYPE_TINY_BLOB,
        MYSQL_TYPE_MEDIUM_BLOB,
        MYSQL_TYPE_LONG_BLOB,
        MYSQL_TYPE_BLOB:
          raise Exception.Create('Unknow FieldType ' + IntToStr(type_));
        MYSQL_TYPE_JSON:
          raise Exception.Create('Unknow FieldType ' + IntToStr(type_));
        MYSQL_TYPE_NULL:
          raise Exception.Create('Unknow FieldType ' + IntToStr(type_));
      end;
    end;
  end;

  FIsBindResultFields := True;
end;

function TMySqlStatement.Query: Boolean;
begin
  Result := False;
  FAutoParamIndex := 0;

  if (FHandle <> nil) and (FOwnerDB <> nil) then
  begin
    if FResult <> nil then
    begin
      FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_free_result(FHandle));
      FResult := nil;
    end;

    //FOwnerDB.ClearResult;

    // 绑定输入参数
    if FParamBindSize > 0 then
    begin
      FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_bind_param(FHandle, FParamBinds));
    end;

    // 执行sql语名
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_execute(FHandle));

    // 得到输出结果
    FResult := FOwnerDB.FLib.mysql_stmt_result_metadata(FHandle);

    BindAllResultFields;

    // 绑定输出字段
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_bind_result(FHandle, FFieldBinds));

    // 存储输出结果
    FOwnerDB.Check(FOwnerDB.FLib.mysql_stmt_store_result(FHandle));

    // 得到结果行数
    FRowCount := FOwnerDB.FLib.mysql_stmt_num_rows(FHandle);

    Result := FResult <> nil;

    FOwnerDB.DoRequest;
  end;
end;

function TMySqlStatement.BindFieldBool(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_TINY;

  BindValue.BufType := bt_Bool;

  // Set buffer
  if BindValue.BufLen < SizeOf(Boolean) then
  begin
    BindValue.BufLen := Max(SizeOf(Boolean), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Boolean);    // Len^^ = FFieldLength[Index]

  Result := True;
end;

function TMySqlStatement.BindFieldDouble(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_DOUBLE;

  BindValue.BufType := bt_Double;

  // Set buffer
  if BindValue.BufLen < SizeOf(Double) then
  begin
    BindValue.BufLen := Max(SizeOf(Double), 8);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;
  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Double);    // Len^^ = FFieldLength[Index]

  Result := True;
end;

function TMySqlStatement.BindFieldShort(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_SHORT;

  BindValue.BufType := bt_Short;

  // Set buffer
  if BindValue.BufLen < SizeOf(Word) then
  begin
    BindValue.BufLen := SizeOf(Word);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Word);    // Len^^ = FFieldLength[Index]

  Result := True;
end;

function TMySqlStatement.BindFieldUShort(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_SHORT;

  BindValue.BufType := bt_UShort;

  // Set buffer
  if BindValue.BufLen < SizeOf(Word) then
  begin
    BindValue.BufLen := SizeOf(Word);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Word);    // Len^^ = FFieldLength[Index]

  if IsUnsigned <> nil then
    IsUnsigned^ := 1;

  Result := True;
end;

function TMySqlStatement.BindFieldInt(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  BindValue.BufType := bt_Int;

  // Set buffer
  if BindValue.BufLen < SizeOf(Integer) then
  begin
    BindValue.BufLen := SizeOf(Integer);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Integer);    // Len^^ = FFieldLength[Index]

  Result := True;
end;

function TMySqlStatement.BindFieldUInt(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONG;

  BindValue.BufType := bt_UInt;

  // Set buffer
  if BindValue.BufLen < SizeOf(Integer) then
  begin
    BindValue.BufLen := SizeOf(Integer);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Integer);    // Len^^ = FFieldLength[Index]

  if IsUnsigned <> nil then
    IsUnsigned^ := 1;

  Result := True;
end;

function TMySqlStatement.BindFieldInt64(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONGLONG;

  BindValue.BufType := bt_Int64;

  // Set buffer
  if BindValue.BufLen < SizeOf(Int64) then
  begin
    BindValue.BufLen := SizeOf(Int64);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Int64);    // Len^^ = FFieldLength[Index]

  Result := True;
end;

function TMySqlStatement.BindFieldUInt64(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_LONGLONG;

  BindValue.BufType := bt_UInt64;

  // Set buffer
  if BindValue.BufLen < SizeOf(Int64) then
  begin
    BindValue.BufLen := SizeOf(Int64);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(Int64);    // Len^^ = FFieldLength[Index]

  if IsUnsigned <> nil then
    IsUnsigned^ := 1;

  Result := True;
end;

function TMySqlStatement.BindFieldDateTime(const Index: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  Len: PPmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_DATETIME;

  BindValue.BufType := bt_DateTime;

  // Set buffer
  if BindValue.BufLen < SizeOf(MYSQL_TIME0506) then
  begin
    BindValue.BufLen := SizeOf(MYSQL_TIME0506);
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := 0;

  //Set length
  Len^^ := SizeOf(MYSQL_TIME0506);    // Len^^ = FFieldLength[Index]

  Result := True;
end;

function TMySqlStatement.BindFieldText(const Index: Integer;
  const StrLen: Integer): Boolean;
var
  BufType: Pmy_long;
  Buf: PPByte;
  BufLen: Pmy_ulong;
  IsNull: PPmy_bool;
  IsUnsigned: PByte;

  Len: PPmy_ulong;

  BindValue: PMySqlStatementBindValue;
begin
  if Index >= FFieldCount then
  begin
    raise Exception.Create('Index out of range');
  end;

  GetFieldBindInfo(Index, BufType, Buf, BufLen, Len, IsNull, IsUnsigned, BindValue);

  // Set buffer_type
  BufType^ := MYSQL_TYPE_STRING;

  BindValue.BufType := bt_Text;

  // Set buffer
  if BindValue.BufLen < StrLen then
  begin
    BindValue.BufLen := StrLen;
    ReallocMem(BindValue.Buf, BindValue.BufLen);

    // 重新分配内存后，指针位置发生了改变，重新绑定位置
    Buf^ := BindValue.Buf;
  end;

  // set buffer_length
  BufLen^ := StrLen;

  //Set length
  Len^^ := StrLen;

  Result := True;
end;


function TMySqlStatement.Fetch: Boolean;
begin
  Result := FOwnerDB.FLib.mysql_stmt_fetch(FHandle) = 0;
  FAutoParamIndex := 0;
end;

function TMySqlStatement.GetColumnValueBool(const Index: Integer): Boolean;
var
  BindValue: PMySqlStatementBindValue;
begin
  Result := False;
  if (not FFieldNullValues[Index]) and (FResult <> nil) then
  begin
    BindValue := FFieldValues.Items[Index];
    Result := PBoolean(BindValue.Buf)^;
  end;
end;

function TMySqlStatement.GetColumnValueDateTime(const Index: Integer): TDateTime;
var
  BindValue: PMySqlStatementBindValue;
  Time0506: MYSQL_TIME0506;
begin
  Result := 0;
  if (not FFieldNullValues[Index]) and (FResult <> nil) then
  begin
    BindValue := FFieldValues.Items[Index];
    Time0506 := PMYSQL_TIME0506(BindValue.Buf)^;
    Result := EncodeDate(Time0506.year, Time0506.month, Time0506.day) +
      EncodeTime(Time0506.hour, Time0506.minute, Time0506.second, 0);
  end;
end;

function TMySqlStatement.GetColumnValueDouble(const Index: Integer): Double;
var
  BindValue: PMySqlStatementBindValue;
begin
  Result := 0;
  if (not FFieldNullValues[Index]) and (FResult <> nil) then
  begin
    BindValue := FFieldValues.Items[Index];
    Result := PDouble(BindValue.Buf)^;
  end;
end;

function TMySqlStatement.GetColumnValueInt(const Index: Integer): Integer;
var
  BindValue: PMySqlStatementBindValue;
begin
  Result := 0;

  if (FResult <> nil) and (not FFieldNullValues[Index]) then
  begin
    BindValue := FFieldValues.Items[Index];
    Result := PInteger(BindValue.Buf)^;
  end;
end;

function TMySqlStatement.GetColumnValueUInt(const Index: Integer): DWORD;
var
  BindValue: PMySqlStatementBindValue;
begin
  Result := 0;

  if (FResult <> nil) and (not FFieldNullValues[Index]) then
  begin
    BindValue := FFieldValues.Items[Index];
    Result := PDWORD(BindValue.Buf)^;
  end;
end;

function TMySqlStatement.GetColumnValueInt64(const Index: Integer): Int64;
var
  BindValue: PMySqlStatementBindValue;
begin
  Result := 0;

  if (FResult <> nil) and (not FFieldNullValues[Index]) then
  begin
    BindValue := FFieldValues.Items[Index];
    Result := PInt64(BindValue.Buf)^;
  end;
end;

function TMySqlStatement.GetColumnValueText(const Index: Integer): AnsiString;
var
  BindValue: PMySqlStatementBindValue;
  S: AnsiString;
  _Time: TDateTime;
  Time0506: MYSQL_TIME0506;
begin
  Result := '';

  if (FResult <> nil) and (not FFieldNullValues[Index]) and (FFieldLength[Index] > 0) then
  begin
    BindValue := FFieldValues.Items[Index];
    if BindValue.BufType = bt_Short then
      Result := IntToStr(PSmallint(BindValue.Buf)^)
    else if BindValue.BufType = bt_UShort then
      Result := IntToStr(PWord(BindValue.Buf)^)
    else if BindValue.BufType = bt_Int then
      Result := IntToStr(PInteger(BindValue.Buf)^)
    else if BindValue.BufType = bt_UInt then
      Result := IntToStr(PDWORD(BindValue.Buf)^)
    else if BindValue.BufType = bt_Int64 then
      Result := IntToStr(PInt64(BindValue.Buf)^)
    else if BindValue.BufType = bt_UInt64 then
      Result := IntToStr(PUInt64(BindValue.Buf)^)
    else if BindValue.BufType = bt_Bool then
      Result := IntToStr(PByte(BindValue.Buf)^)
    else if BindValue.BufType = bt_Double then
      Result := FloatToStr(PDouble(BindValue.Buf)^)
    else if BindValue.BufType = bt_DateTime then
    begin
      Time0506 := PMYSQL_TIME0506(BindValue.Buf)^;
      _Time := EncodeDate(Time0506.year, Time0506.month, Time0506.day) +
        EncodeTime(Time0506.hour, Time0506.minute, Time0506.second, 0);
      Result := FormatDateTime('yyyy-mm-dd hh:nn:ss', _Time);
    end
    else if BindValue.BufType = bt_Text then
    begin
      SetLength(S, FFieldLength[Index]);
      Move(BindValue.Buf^, S[1], FFieldLength[Index]);

      // iLen := MultiByteToWideChar(CP_UTF8, 0, pData, iLen, PWideChar(@s2[1]), Length(s2));
      if FOwnerDB.FEncoding = ecUTF8 then
        Result := Utf8Decode(S)
      else
        Result := S;
    end;
  end;
end;

function TMySqlStatement.OrderGetColumnValueBool: Boolean;
begin
  Result := False;
  if FResult <> nil then
  begin
    Result := GetColumnValueBool(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderGetColumnValueDateTime: TDateTime;
begin
  Result := 0;
  if FResult <> nil then
  begin
    Result := GetColumnValueDateTime(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderGetColumnValueDouble: Double;
begin
  Result := 0;
  if FResult <> nil then
  begin
    Result := GetColumnValueDouble(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderGetColumnValueInt: Integer;
begin
  Result := 0;
  if FResult <> nil then
  begin
    Result := GetColumnValueInt(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderGetColumnValueUInt: DWORD;
begin
  Result := 0;
  if FResult <> nil then
  begin
    Result := GetColumnValueUInt(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderGetColumnValueInt64: Int64;
begin
  Result := 0;
  if FResult <> nil then
  begin
    Result := GetColumnValueInt64(FAutoParamIndex);
    Inc(FAutoParamIndex);
  end;
end;

function TMySqlStatement.OrderGetColumnValueText: AnsiString;
begin
  Result := '';
  if FResult <> nil then
  begin
    Result := GetColumnValueText(FAutoParamIndex);
    Inc(FAutoParamIndex);
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
