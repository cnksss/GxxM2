unit SQLite3DataBase;

interface

uses
  Classes, Windows, SysUtils, SQLiteCli;

type
  ESQLite3Error = class(Exception);

  TSQLStatements = class;

  TSQLite3Database = class(TObject)
  private
    FDBHandle: psqlite3;
    FIsUTF8: Boolean;
    FTimeOut: Integer;

    FConnected: Boolean;
    FDatabase: string;

    FMustExist: Boolean;

    FIsTransactionOpen: Boolean;
    FVersion: string;

    FSQLStatements: TSQLStatements;

    FAfterConnect: TNotifyEvent;
    FAfterDisconnect: TNotifyEvent;
    FBeforeConnect: TNotifyEvent;
    FBeforeDisconnect: TNotifyEvent;

    FErrorCode: Integer;

    FIsReadOnly: Boolean;
    FUseThreadMode: Boolean;

    procedure SetTimeOut(const Value: Integer);
    procedure Close;
    procedure SetConnected(const Value: Boolean);
  private
    procedure CheckDBHandle;
    procedure ShowError(AErrorCode: Integer);
    procedure SetIsReadOnly(const Value: Boolean);
    procedure SetUseThreadMode(const Value: Boolean);
  public
    constructor Create;
    destructor Destroy; override;

    function Execute(const SQL: WideString): Boolean;
    function ExecSql(const SQL: WideString): Boolean;

    function GetLastInsertRowID: Int64;
    function GetRowsAffected: Integer;

    procedure BeginTransaction;
    procedure Commit;
    procedure Rollback;

    property IsReadOnly: Boolean read FIsReadOnly write SetIsReadOnly;
    property UseThreadMode: Boolean read FUseThreadMode write SetUseThreadMode;
    property Database: string read FDatabase write FDatabase;
    property MustExist: Boolean read FMustExist write FMustExist;
    property DBHandle: psqlite3 read FDBHandle;
    property IsUTF8: Boolean read FIsUTF8;
    property TimeOut: Integer read FTimeOut write SetTimeOut;
    property Connected: Boolean read FConnected write SetConnected;
    property IsTransactionOpen: Boolean read FIsTransactionOpen;
    property Version: string read FVersion;
    property Statements: TSQLStatements read FSQLStatements;
    property ErrorCode: Integer read FErrorCode;
  end;

  TSQLStatement = class(TObject)
  private
    FOwner: TSQLStatements;
    FOwnerDB: TSQLite3Database;

    FName: string;
    FSql: string;
    FHandle: psqlite3_stmt;

    FAutoOrderIndex: Integer;

    procedure ShowError(AErrorCode: Integer);
  public
    constructor Create;
    destructor Destroy; override;
    
    property Owner: TSQLStatements read FOwner;
    property Name: string read FName;
    property Sql: string read FSql write FSql;
    property Handle: psqlite3_stmt read FHandle;

    function Prepare: Boolean;
    function Reset: Boolean;
    function Step: Integer;
    function Finalize: Boolean;

    function GetBindParamCount: Integer;
    function ClearBindings: Boolean;
    function ParamIndexByName(const ParamName: AnsiString): Integer;

    function BindNull(const Index: Integer): Boolean; overload;
    function BindInt(const Index: Integer; const Value: Integer): Boolean; overload;
    function BindInt64(const Index: Integer; const Value: Int64): Boolean; overload;
    function BindBool(const Index: Integer; const Value: Boolean): Boolean; overload;
    function BindDouble(const Index: Integer; const Value: Double): Boolean; overload;
    function BindText(const Index: Integer; const Value: AnsiString): Boolean; overload;
    function BindBolb(const Index: Integer; const Value: PAnsiChar; const ValueLen: Integer): Boolean; overload;


    // BindValue Index 是从1开始，先增加再使用
    function OrderBindNull(): Boolean;
    function OrderBindInt(const Value: Integer): Boolean;
    function OrderBindInt64(const Value: Int64): Boolean;
    function OrderBindBool(const Value: Boolean): Boolean;
    function OrderBindDouble(const Value: Double): Boolean;
    function OrderBindText(const Value: AnsiString): Boolean;
    function OrderBindBolb(const Value: PAnsiChar; const ValueLen: Integer): Boolean;

    function BindNull(const ParamName: AnsiString): Boolean; overload;
    function BindInt(const ParamName: AnsiString; const Value: Integer): Boolean; overload;
    function BindInt64(const ParamName: AnsiString; const Value: Int64): Boolean; overload;
    function BindBool(const ParamName: AnsiString; const Value: Boolean): Boolean; overload;
    function BindDouble(const ParamName: AnsiString; const Value: Double): Boolean; overload;
    function BindText(const ParamName: AnsiString; const Value: AnsiString): Boolean; overload;
    function BindBolb(const ParamName: AnsiString; const Value: PAnsiChar; const ValueLen: Integer): Boolean; overload;

    function GetColumnCount: Integer;
    function GetColumnType(const Index: Integer): Integer;
    function GetColumnDeclType(const Index: Integer): AnsiString;
    function GetColumnName(const Index: Integer): AnsiString;
    function GetColumnBytes(const Index: Integer): Integer;


    function GetColumnValueInt(const Index: Integer): Integer;
    function GetColumnValueInt64(const Index: Integer): Int64;
    function GetColumnValueBool(const Index: Integer): Boolean;
    function GetColumnValueDouble(const Index: Integer): Double;
    function GetColumnValueText(const Index: Integer): AnsiString;

    //function GetColumnValueWideText(const Index: Integer): WideString;

    // GetColumnValue Index 是从0开始，先使用再增加
    function OrderGetColumnValueInt: Integer;
    function OrderGetColumnValueInt64: Int64;
    function OrderGetColumnValueBool: Boolean;
    function OrderGetColumnValueDouble: Double;
    function OrderGetColumnValueText: AnsiString;

    //function OrderGetColumnValueWideText: WideString;
  end;

  TSQLStatements = class(TObject)
  private
    FOwner: TSQLite3Database;
    FList: TList;
    function Search(Name: string; var Index: Integer): Boolean; virtual;
    function GetCount: Integer;
    function GetItems(Index: Integer): TSQLStatement;
  public
    constructor Create(AOwner: TSQLite3Database);
    destructor Destroy; override;
    
    function AddSQLStatement(Name: string): TSQLStatement;
    procedure Delete(Index: Integer);
    procedure Remove(Name: string);
    procedure Clear;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: TSQLStatement read GetItems;
    function Find(Name: string): TSQLStatement;
  end;

implementation

resourcestring
  SMissingDatabaseProperty = 'Missing database property';
  SDatabaseNotConnected = 'database is not connected.';
  STransactionAlreadyOpen = 'Transaction is already opened.';
  STransactionNotOpen = 'No transaction is open';

{ TSQLite3Database }

constructor TSQLite3Database.Create;
begin
  FIsUTF8 := True;
  FTimeOut := 0;
  FDBHandle := nil;
  FMustExist := False;
  FIsTransactionOpen := False;

  FIsReadOnly := False;
  FUseThreadMode := False;

  sqlite3_initialize;
  FVersion := SQLite3_LibVersion;
  FSQLStatements := TSQLStatements.Create(Self);
end;

destructor TSQLite3Database.Destroy;
begin
  if FConnected then Close;
  FSQLStatements.Free;
  sqlite3_finalize(FDBHandle);
  inherited;
end;

procedure TSQLite3Database.SetConnected(const Value: Boolean);
var
  nRet: Integer;
  sFileName: AnsiString;
begin
  if not Value then
  begin
    if not FConnected then Exit;

    Close;
  end
  else
  begin
    if Length(FDatabase) = 0 then
    begin
      raise ESQLite3Error.Create(SMissingDatabaseProperty);
      Exit;
    end;

    if FMustExist and (not FileExists(FDatabase)) then
    begin
      raise ESQLite3Error.Create('Database ' + FDatabase + ' does not exist');
    end;

    if Assigned(FBeforeConnect) then
      FBeforeConnect(Self);

    sFileName := FDatabase;
    if FIsUTF8 then
      sFileName := AnsiToUtf8(FDatabase);

    if FUseThreadMode then
    begin
      if not FIsReadOnly then
        nRet := sqlite3_open_v2(PAnsiChar(sFileName), FDBHandle, SQLITE_OPEN_READWRITE or SQLITE_OPEN_CREATE or SQLITE_OPEN_FULLMUTEX, nil)
      else
        nRet := sqlite3_open_v2(PAnsiChar(sFileName), FDBHandle, SQLITE_OPEN_READONLY or SQLITE_OPEN_FULLMUTEX, nil);
    end
    else
    begin
      if not FIsReadOnly then
        nRet := sqlite3_open_v2(PAnsiChar(sFileName), FDBHandle, SQLITE_OPEN_READWRITE or SQLITE_OPEN_CREATE, nil)
      else
        nRet := sqlite3_open_v2(PAnsiChar(sFileName), FDBHandle, SQLITE_OPEN_READONLY, nil);
    end;

    // 限定工作线程数量 chongchong 2018-10-13 01:36:18
    sqlite3_limit(FDBHandle, SQLITE_LIMIT_WORKER_THREADS, 4);

    if nRet <> SQLITE_OK then
    begin
      ShowError(nRet);
      Exit;
    end;

    FConnected := True;
    if Assigned(FAfterConnect) then
      FAfterConnect(Self);

    if (FTimeOut > 0) then
      TimeOut := FTimeOut;
  end;
end;

procedure TSQLite3Database.SetIsReadOnly(const Value: Boolean);
begin
  FIsReadOnly := Value;
end;

procedure TSQLite3Database.Close;
var
  I: Integer;
begin
  if Assigned(FBeforeDisconnect) then
    FBeforeDisconnect(Self);

  if FIsTransactionOpen then
    Rollback;

  for I := 0 to FSQLStatements.Count - 1 do
  begin
    FSQLStatements.Items[I].Finalize;
  end;

  SQLite3_Close(FDBHandle);

  if Assigned(FAfterDisconnect) then
    FAfterDisconnect(Self);

  FDBHandle := nil;
  FConnected := False;
end;

procedure TSQLite3Database.SetTimeOut(const Value: Integer);
begin
  FTimeOut := Value;
  if FConnected then
    sqlite3_busy_timeout(FDBHandle, FTimeOut);
end;

procedure TSQLite3Database.SetUseThreadMode(const Value: Boolean);
begin
  FUseThreadMode := Value;
end;

function TSQLite3Database.Execute(const SQL: WideString): Boolean;
var
  nRet: Integer;
  S: AnsiString;
begin
  CheckDBHandle;

  if FIsUTF8 then
  begin
    S := Utf8Encode(SQL);
    nRet := sqlite3_exec(FDBHandle, PAnsiChar(S), nil, nil, nil);
  end
  else
  begin
    S := SQL;
    nRet := sqlite3_exec(FDBHandle, PAnsiChar(S), nil, nil, nil);
  end;

  Result := nRet = SQLITE_OK;
  if not Result then ShowError(nRet);
end;

function TSQLite3Database.ExecSql(const SQL: WideString): Boolean;
var
  nRet: Integer;
  S: AnsiString;
  Smt: psqlite3_stmt;
  PF: PByte;
begin
  CheckDBHandle;

  if FIsUTF8 then
    S := Utf8Encode(SQL)
  else
    S := SQL;

  PF := PByte(@S[1]);

  Smt := nil;
  nRet := SQLite3_Prepare_v2(FDBHandle, PF, -1, Smt, PF);
  Result := nRet = SQLITE_OK;
  if not Result then ShowError(nRet);

  nRet := SQLite3_Step(Smt);
  Result := nRet in [SQLITE_OK, SQLITE_DONE, SQLITE_ROW];
  if not Result then ShowError(nRet);

  nRet := SQLite3_Finalize(Smt);
  Result := nRet = SQLITE_OK;
  if not Result then ShowError(nRet);
end;

procedure TSQLite3Database.BeginTransaction;
begin
  if not FIsTransactionOpen then
  begin
    Execute('BEGIN TRANSACTION;');
    FIsTransactionOpen := True;
  end
  else
    raise ESQLite3Error.Create(STransactionAlreadyOpen);
end;

procedure TSQLite3Database.Commit;
begin
  if FIsTransactionOpen then
  begin
    Execute('COMMIT;');
    FIsTransactionOpen := False;
  end
  else
    raise ESQLite3Error.Create(STransactionNotOpen);
end;

procedure TSQLite3Database.Rollback;
begin
  if FIsTransactionOpen then
  begin
    Execute('ROLLBACK;');
    FIsTransactionOpen := False;
  end
  else
    raise ESQLite3Error.Create(STransactionNotOpen);
end;

procedure TSQLite3Database.CheckDBHandle;
begin
  if FDBHandle = nil then
    raise ESQLite3Error.Create(SDatabaseNotConnected);
end;

procedure TSQLite3Database.ShowError(AErrorCode: Integer);
var
  S: AnsiString;
begin
  S := '';

  FErrorCode := AErrorCode;

  if IsUTF8 then
    S := Utf8ToAnsi(AnsiString(PAnsiChar(sqlite3_errstr(AErrorCode))))
  else
    S := AnsiString(PAnsiChar(sqlite3_errstr(AErrorCode)));

  raise ESQLite3Error.Create(S);
end;

function TSQLite3Database.GetLastInsertRowID: Int64;
begin
  CheckDBHandle;
  Result := sqlite3_last_insert_rowid(FDBHandle);
end;


function TSQLite3Database.GetRowsAffected: Integer;
begin
  CheckDBHandle;
  if not FConnected then
    Result := -1
  else
    Result := sqlite3_changes(DBHandle);
end;

{ TSQLStatement }

(*
function StrToUTF8(const S: WideString): AnsiString;
begin
  Result := UTF8Encode(S);
end;

function UTF8ToStr(const S: PAnsiChar; const Len: Integer): WideString;
var
  UTF8Str: AnsiString;
begin
  if Len < 0 then
  begin
    Result := UTF8Decode(S);
  end
  else if Len > 0 then
  begin
    SetLength(UTF8Str, Len);
    Move(S^, UTF8Str[1], Len);
    Result := UTF8Decode(UTF8Str);
  end
  else Result := '';
end;
*)

constructor TSQLStatement.Create;
begin
  FHandle := nil;
  FAutoOrderIndex := 0;
end;

destructor TSQLStatement.Destroy;
begin
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    SQLite3_Reset(FHandle);
    SQLite3_Finalize(FHandle);
    FHandle := nil;
  end;
  inherited;
end;

function TSQLStatement.Prepare: Boolean;
var
  PF: PByte;
  Ret: Integer;
  S: AnsiString;
begin
  FAutoOrderIndex := 0;
  if Length(FSql) = 0 then
  begin
    raise Exception.CreateFmt('TSQLStatement %s sql can not be empty.', [FName]);
  end;

  Result := False;
  if FOwnerDB <> nil then
  begin
    if (FHandle <> nil) then
    begin
      SQLite3_Finalize(FHandle);
      FHandle := nil;
    end;

    S := FSql;
    if (FOwnerDB.IsUTF8) then
    begin
      S := AnsiToUtf8(FSql);
    end;
    PF := PByte(@S[1]);

    FHandle := nil;
    Ret := SQLite3_Prepare_v2(FOwnerDB.DBHandle, PF, -1, FHandle, PF);
    Result := Ret in [SQLITE_OK];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.Reset: Boolean;
var
  Ret: Integer;
begin
  FAutoOrderIndex := 0;

  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Reset(FHandle);
    Result := Ret in [SQLITE_OK];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.Step: Integer;
begin
  FAutoOrderIndex := 0;
  Result := -1;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Step(FHandle);
    if not (Result in [SQLITE_OK, SQLITE_DONE, SQLITE_ROW]) then ShowError(Result);
  end;
end;

function TSQLStatement.Finalize: Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    SQLite3_Reset(FHandle);

    Ret := SQLite3_Finalize(FHandle);
    Result := Ret in [SQLITE_OK];
    if Result then
      FHandle := nil
    else
      ShowError(Ret);
  end;
end;

function TSQLStatement.GetBindParamCount: Integer;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := sqlite3_bind_parameter_count(FHandle);
  end;
end;

function TSQLStatement.ClearBindings: Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := sqlite3_clear_bindings(FHandle) in [SQLITE_OK];
  end;
end;

function TSQLStatement.ParamIndexByName(const ParamName: AnsiString): Integer;
var
  S: AnsiString;
begin
  Result := 0;

  if Length(ParamName) = 0 then
  begin
    raise Exception.Create('TSQLStatement ParamName can not be empty.');
  end;

  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if (FOwnerDB.FIsUtf8) then
      S := AnsiToUtf8(ParamName)
    else
      S := ParamName;

    Result := Sqlite3_Bind_Parameter_Index(FHandle, PAnsiChar(S));
  end;
end;

//------------------------------------------------------------------------------------------------------------------

function TSQLStatement.BindNull(const Index: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Null(FHandle, Index);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindInt(const Index, Value: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Int(FHandle, Index, Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindInt64(const Index: Integer;
  const Value: Int64): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Int64(FHandle, Index, Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindBool(const Index: Integer;
  const Value: Boolean): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Int(FHandle, Index, Integer(Value));
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindDouble(const Index: Integer;
  const Value: Double): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Double(FHandle, Index, Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindText(const Index: Integer;
  const Value: AnsiString): Boolean;
var
  S: AnsiString;
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if Length(Value) = 0 then
    begin
      Ret := SQLite3_Bind_Text(FHandle, Index, '', 0, SQLITE_TRANSIENT);
    end
    else
    begin
      if (FOwnerDB.FIsUtf8) then
        S := AnsiToUtf8(Value)
      else
        S := Value;

      Ret := SQLite3_Bind_Text(FHandle, Index, PAnsiChar(S), Length(S), SQLITE_TRANSIENT);
    end;

    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindBolb(const Index: Integer;
  const Value: PAnsiChar; const ValueLen: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Blob(FHandle, Index, Value, ValueLen, SQLITE_TRANSIENT);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

//---------------------------------------------------------------------------------------------------------

function TSQLStatement.OrderBindNull(): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Inc(FAutoOrderIndex);
    Ret := SQLite3_Bind_Null(FHandle, FAutoOrderIndex);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.OrderBindInt(const Value: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Inc(FAutoOrderIndex);
    Ret := SQLite3_Bind_Int(FHandle, FAutoOrderIndex, Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.OrderBindInt64(const Value: Int64): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Inc(FAutoOrderIndex);
    Ret := SQLite3_Bind_Int64(FHandle, FAutoOrderIndex, Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.OrderBindBool(const Value: Boolean): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Inc(FAutoOrderIndex);
    Ret := SQLite3_Bind_Int(FHandle, FAutoOrderIndex, Integer(Value));
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.OrderBindDouble(const Value: Double): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Inc(FAutoOrderIndex);
    Ret := SQLite3_Bind_Double(FHandle, FAutoOrderIndex, Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.OrderBindText(const Value: AnsiString): Boolean;
var
  S: AnsiString;
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if Length(Value) = 0 then
    begin
      Inc(FAutoOrderIndex);
      Ret := SQLite3_Bind_Text(FHandle, FAutoOrderIndex, '', 0, SQLITE_TRANSIENT);
    end
    else
    begin
      if (FOwnerDB.FIsUtf8) then
        S := AnsiToUtf8(Value)
      else
        S := Value;

      Inc(FAutoOrderIndex);
      Ret := SQLite3_Bind_Text(FHandle, FAutoOrderIndex, PAnsiChar(S), Length(S), SQLITE_TRANSIENT);
    end;

    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.OrderBindBolb(const Value: PAnsiChar; const ValueLen: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Inc(FAutoOrderIndex);
    Ret := SQLite3_Bind_Blob(FHandle, FAutoOrderIndex, Value, ValueLen, SQLITE_TRANSIENT);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

//---------------------------------------------------------------------------------------------

function TSQLStatement.BindNull(const ParamName: AnsiString): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Null(FHandle, ParamIndexByName(ParamName));
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindInt(const ParamName: AnsiString;
  const Value: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Int(FHandle, ParamIndexByName(ParamName), Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindInt64(const ParamName: AnsiString;
  const Value: Int64): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Int64(FHandle, ParamIndexByName(ParamName), Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindBool(const ParamName: AnsiString;
  const Value: Boolean): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Int(FHandle, ParamIndexByName(ParamName), Integer(Value));
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindDouble(const ParamName: AnsiString;
  const Value: Double): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Double(FHandle, ParamIndexByName(ParamName), Value);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindText(const ParamName,
  Value: AnsiString): Boolean;
var
  S: AnsiString;
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if Length(Value) = 0 then
    begin
      Ret := SQLite3_Bind_Text(FHandle, ParamIndexByName(ParamName), '', 0, SQLITE_TRANSIENT);
    end
    else
    begin
      if (FOwnerDB.FIsUtf8) then
        S := AnsiToUtf8(Value)
      else
        S := Value;

      Ret := SQLite3_Bind_Text(FHandle, ParamIndexByName(ParamName), PAnsiChar(S), Length(S), SQLITE_TRANSIENT);
    end;

    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

function TSQLStatement.BindBolb(const ParamName: AnsiString;
  const Value: PAnsiChar; const ValueLen: Integer): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Ret := SQLite3_Bind_Blob(FHandle, ParamIndexByName(ParamName), Value, ValueLen, SQLITE_TRANSIENT);
    Result := Ret in [SQLITE_OK, SQLITE_DONE];
    if not Result then ShowError(Ret);
  end;
end;

//---------------------------------------------------------------------------------------------

procedure TSQLStatement.ShowError(AErrorCode: Integer);
var
  S: AnsiString;
begin
  S := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    FOwnerDB.FErrorCode := AErrorCode;
    
    if FOwnerDB.IsUTF8 then
      S := Utf8ToAnsi(AnsiString(PAnsiChar(sqlite3_errstr(AErrorCode))))
    else
      S := AnsiString(PAnsiChar(sqlite3_errstr(AErrorCode)));
  end;

  raise ESQLite3Error.Create(FSql + sLineBreak + 'error code = ' + IntToStr(AErrorCode) + '; ' + S);
end;

function TSQLStatement.GetColumnCount: Integer;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_count(FHandle);
  end;
end;

function TSQLStatement.GetColumnName(const Index: Integer): AnsiString;
begin
  Result := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if FOwnerDB.IsUTF8 then
      Result := Utf8ToAnsi(AnsiString(PAnsiChar(sqlite3_column_name(FHandle, Index))))
    else
      Result := AnsiString(PAnsiChar(sqlite3_column_name(FHandle, Index)));
  end;
end;

function TSQLStatement.GetColumnBytes(const Index: Integer): Integer;
begin
  Result := sqlite3_column_bytes(FHandle, Index);
end;

function TSQLStatement.GetColumnType(const Index: Integer): Integer;
begin
  Result := -1;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_type(FHandle, Index);
  end;
end;

function TSQLStatement.GetColumnDeclType(const Index: Integer): AnsiString;
begin
  Result := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if FOwnerDB.IsUTF8 then
      Result := Utf8ToAnsi(AnsiString(PAnsiChar(sqlite3_column_decltype(FHandle, Index))))
    else
      Result := AnsiString(PAnsiChar(sqlite3_column_decltype(FHandle, Index)));
  end;
end;

function TSQLStatement.GetColumnValueInt(const Index: Integer): Integer;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_Int(FHandle, Index);
  end;
end;

function TSQLStatement.GetColumnValueInt64(const Index: Integer): Int64;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_Int64(FHandle, Index);
  end;
end;

function TSQLStatement.GetColumnValueBool(const Index: Integer): Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_int(FHandle, Index) <> 0;
  end;
end;

function TSQLStatement.GetColumnValueDouble(const Index: Integer): Double;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_Double(FHandle, Index);
  end;
end;

function TSQLStatement.GetColumnValueText(const Index: Integer): AnsiString;
begin
  Result := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if FOwnerDB.IsUTF8 then
      Result := Utf8ToAnsi(AnsiString(PAnsiChar(SQLite3_Column_Text(FHandle, Index))))
    else
      Result := AnsiString(PAnsiChar(SQLite3_Column_Text(FHandle, Index)));
  end;
end;

(*
function TSQLStatement.GetColumnValueWideText(const Index: Integer): WideString;
var
  Len: Integer;
begin
  Result := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Len := GetColumnBytes(Index);
    if FOwnerDB.IsUTF8 then
      Result := UTF8ToStr(PAnsiChar(sqlite3_column_text(FHandle, Index)), Len)
    else
      Result := PAnsiChar(sqlite3_column_text(FHandle, Index));
  end;
end;
*)

function TSQLStatement.OrderGetColumnValueInt: Integer;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_Int(FHandle, FAutoOrderIndex);
    Inc(FAutoOrderIndex);
  end;
end;

function TSQLStatement.OrderGetColumnValueInt64: Int64;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_Int64(FHandle, FAutoOrderIndex);
    Inc(FAutoOrderIndex);
  end;
end;


function TSQLStatement.OrderGetColumnValueBool: Boolean;
begin
  Result := False;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_int(FHandle, FAutoOrderIndex) <> 0;
    Inc(FAutoOrderIndex);
  end;
end;

function TSQLStatement.OrderGetColumnValueDouble: Double;
begin
  Result := 0;
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Result := SQLite3_Column_Double(FHandle, FAutoOrderIndex);
    Inc(FAutoOrderIndex);
  end;
end;

function TSQLStatement.OrderGetColumnValueText: AnsiString;
begin
  Result := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    if FOwnerDB.IsUTF8 then
      Result := Utf8ToAnsi(AnsiString(PAnsiChar(SQLite3_Column_Text(FHandle, FAutoOrderIndex))))
    else
      Result := AnsiString(PAnsiChar(SQLite3_Column_Text(FHandle, FAutoOrderIndex)));

    Inc(FAutoOrderIndex);
  end;
end;

(*
function TSQLStatement.OrderGetColumnValueWideText: WideString;
var
  Len: Integer;
begin
  Result := '';
  if (FOwnerDB <> nil) and (FHandle <> nil) then
  begin
    Len := GetColumnBytes(FAutoOrderIndex);
    if FOwnerDB.IsUTF8 then
      Result := UTF8ToStr(PAnsiChar(sqlite3_column_text(FHandle, FAutoOrderIndex)), Len)
    else
      Result := PAnsiChar(sqlite3_column_text(FHandle, FAutoOrderIndex));

    Inc(FAutoOrderIndex);
  end;
end;
*)


{ TSQLStatements }

constructor TSQLStatements.Create(AOwner: TSQLite3Database);
begin
  inherited Create;
  FOwner := AOwner;
  FList := TList.Create;
end;

destructor TSQLStatements.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TSQLStatements.AddSQLStatement(Name: string): TSQLStatement;
var
  Index: Integer;
begin
  if not Search(Name, Index) then
  begin
    Result := TSQLStatement.Create;
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

function TSQLStatements.Find(Name: string): TSQLStatement;
var
  Index: Integer;
begin
  if Search(Name, Index) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

function TSQLStatements.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TSQLStatements.GetItems(Index: Integer): TSQLStatement;
begin
  Result := nil;
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index];
end;

procedure TSQLStatements.Delete(Index: Integer);
begin
  if (Index >= 0) and (Index < FList.Count) then
  begin
    TSQLStatement(FList.Items[Index]).Free;
    FList.Delete(Index);
  end;
end;

procedure TSQLStatements.Remove(Name: string);
var
  Index: Integer;
begin
  if Search(Name, Index) then
  begin
    TSQLStatement(FList.Items[Index]).Free;
    FList.Delete(Index);
  end;
end;

procedure TSQLStatements.Clear;
var
  I: Integer;
  Statement: TSQLStatement;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Statement := FList.Items[I];
    Statement.Free;
  end;
  FList.Clear;
end;

function TSQLStatements.Search(Name: string; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Search := False;
  L := 0;
  H := Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := AnsiCompareText(TSQLStatement(FList.Items[I]).FName, Name);
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

