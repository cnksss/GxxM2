unit DataManage;

interface
uses
  Windows, Classes, SysUtils, DB, ComObj, ADODB, ActiveX, Access, Grobal2, SDK;
type
  TAccessTable = class;
  TAccessEngine = class
    m_UserCriticalSection: TRTLCriticalSection;
    m_sFileName: string;
  private
    { Private declarations }
    AccessTableList: array of TAccessTable;
    TableList: TStringList;

    function GetTable(Table: string): TAccessTable;
    procedure SetTable(Table: string; Value: TAccessTable);
  protected
  public
    constructor Create(const FileName: string);
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    function LoadTable(Table: string): Boolean;
    procedure CreateTable(Table, SQL: string);
    function Connect: Boolean;
    procedure DisConnect;
    // property Count: Integer read GetCount;
    property Tables[Table: string]: TAccessTable read GetTable write SetTable;
  end;

  TAccessTable = class
    m_sTable: string;
  private
    function GetCount: Integer;
    function GetField(Field: string): TField;
    function GetParameters: TParameters;
    function GetADOQuery:TADOQuery;
  public
    constructor Create(const Table: string);
    destructor Destroy; override;
    procedure ClearSQL;
    procedure AddSQL(SQL: string);
    procedure OpenSQL;
    procedure NextSQL;
    procedure CloseSQL;
    procedure ExecSQL;
    procedure Lock;
    procedure UnLock;
    property Count: Integer read GetCount;
    property Fields[Field: string]: TField read GetField;
    property Parameters: TParameters read GetParameters;
    property ADOQuery:TADOQuery read GetADOQuery;
  end;
const
  g_sShopItemTable = 'TBL_SHOPITEM';
implementation
uses M2Share;
var
  DBQry: TADOQuery;
  ADOConnection: TADOConnection;
const
  g_sSQLString = 'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%s;Persist Security Info=False';

  g_sShopItemField =
    'FLD_ITEMTYPE		 TINYINT      NOT NULL,' +
    'FLD_ITEMNAME		 CHAR(14)	    NOT NULL,' +
    'FLD_ITEMPRICE   INT          NOT NULL,' +
    'FLD_IMAGEINDEX  INT          NOT NULL,' +
    'FLD_IMAGECOUNT  INT          NOT NULL,' +
    'FLD_ITEMMEMO1	 CHAR(18)	    NULL,' +
    'FLD_ITEMMEMO2   BINARY(150)  NULL';

constructor TAccessEngine.Create(const FileName: string);
var
  I: Integer;
begin
  inherited Create;
  CoInitialize(nil);

  InitializeCriticalSection(m_UserCriticalSection);
  m_sFileName := FileName;
  TableList := TStringList.Create;
  ADOConnection := TADOConnection.Create(nil);
  DBQry := TADOQuery.Create(nil);

  if CreateAccessDB(FileName, False) then begin

  end;

  if not TableExists(m_sFileName, g_sShopItemTable) then begin
    AccessCreateTable(FileName, g_sShopItemTable, g_sShopItemField);
    CreateAccessIndex(FileName, g_sShopItemTable, 'iFLD_ITEMNAME', 'FLD_ITEMNAME', True, True);
  end;


  GetTableList(FileName, TableList);
  for I := 0 to TableList.Count - 1 do begin
    TableList.Objects[I] := TAccessTable.Create(TableList.Strings[I]);
  end;
  ADOConnection.ConnectionString := Format(g_sSQLString, [m_sFileName]);
  ADOConnection.LoginPrompt := False;
  ADOConnection.KeepConnection := True;

  DBQry.Connection := ADOConnection;
  DBQry.Prepared := True;

  try
    ADOConnection.Connected := True;
  except
    MainOutMessage('[Exception] TAccessEngine ADOConnection:Connected');
  end;
end;

destructor TAccessEngine.Destroy;
var
  I: Integer;
begin
  for I := 0 to TableList.Count - 1 do begin
    TAccessTable(TableList.Objects[I]).Free;
  end;
  TableList.Free;
  DBQry.Free;
  ADOConnection.Free;
  DeleteCriticalSection(m_UserCriticalSection);
  CoUnInitialize;
  inherited Destroy;
end;

procedure TAccessEngine.Lock;
begin
  EnterCriticalSection(m_UserCriticalSection);
end;

procedure TAccessEngine.UnLock;
begin
  LeaveCriticalSection(m_UserCriticalSection);
end;

function TAccessEngine.LoadTable(Table: string): Boolean;
begin
  Result := False;
  if TableExists(m_sFileName, Table) then begin

    Result := True;
  end;
end;

procedure TAccessEngine.CreateTable(Table, SQL: string);
begin
  if not TableExists(m_sFileName, Table) then
    AccessCreateTable(m_sFileName, Table, SQL);
end;

function TAccessEngine.Connect: Boolean;
begin
  Result := ADOConnection.Connected;
  if not ADOConnection.Connected then begin
    ADOConnection.ConnectionString := Format(g_sSQLString, [m_sFileName]);
    ADOConnection.LoginPrompt := False;
    ADOConnection.KeepConnection := True;

    DBQry.Connection := ADOConnection;
    DBQry.Prepared := True;

    try
      ADOConnection.Connected := True;
      Result := True;
    except
    end;
  end;
end;

procedure TAccessEngine.DisConnect;
begin
  ADOConnection.Connected := False;
  ADOConnection.KeepConnection := False;
  DBQry.Prepared := False;
end;

function TAccessEngine.GetTable(Table: string): TAccessTable;
var
  I: Integer;
begin
  Result := nil;
  for I := 0 to TableList.Count - 1 do begin
    if Table = TableList.Strings[I] then begin
      Result := TAccessTable(TableList.Objects[I]);
      Break;
    end;
  end;
end;

procedure TAccessEngine.SetTable(Table: string; Value: TAccessTable);
var
  I: Integer;
begin
  for I := 0 to TableList.Count - 1 do begin
    if Table = TableList.Strings[I] then begin
      TAccessTable(TableList.Objects[I]).Free;
      TableList.Objects[I] := Value;
      Break;
    end;
  end;
end;
{------------------------------------------------------------------------------}

constructor TAccessTable.Create(const Table: string);
begin
  inherited Create;
  m_sTable := Table;
end;

destructor TAccessTable.Destroy;
begin
  inherited Destroy;
end;


function TAccessTable.GetCount: Integer;
begin
  Result := DBQry.RecordCount;
end;
function TAccessTable.GetADOQuery:TADOQuery;
begin
  Result := DBQry;
end;
procedure TAccessTable.Lock;
begin
  AccessEngine.Lock;
end;

procedure TAccessTable.UnLock;
begin
  AccessEngine.UnLock;
end;

procedure TAccessTable.ClearSQL;
begin
  DBQry.SQL.Clear;
end;

procedure TAccessTable.AddSQL(SQL: string);
begin
  DBQry.SQL.Add(SQL)
end;

procedure TAccessTable.OpenSQL;
begin
  DBQry.Open;
end;

procedure TAccessTable.NextSQL;
begin
  DBQry.Next;
end;

procedure TAccessTable.CloseSQL;
begin
  DBQry.Close;
end;

procedure TAccessTable.ExecSQL;
begin
  DBQry.ExecSQL;
end;

function TAccessTable.GetField(Field: string): TField;
begin
  Result := DBQry.FieldByName(Field);
end;
function TAccessTable.GetParameters: TParameters;
begin
  Result := DBQry.Parameters;
end;

end.

