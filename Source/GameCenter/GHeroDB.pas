unit GHeroDB;

interface
uses
  Windows, Messages, Classes, SysUtils, DBTables, DB, ActiveX;
type
  THeroDB = class
  private
    Session: TSession;
    SessionList: TStringList;
    function GetTables(const HeroDBName: string): TStrings;
    function GetFields(const HeroDBName, TableName: string): TStrings;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    function HeroDBExist(const HeroDBName: string): Boolean;
    function TableExist(const HeroDBName, TableName: string): Boolean;
    procedure SaveHeroDBConfigFile(const Name, Path: string);
    procedure DeleteAlias(const Name: string);
    function FieldExist(const HeroDBName, TableName, FieldName: string): Boolean;

    function CreateField(const HeroDBName, TableName, FieldName: string; Default: string; Len: Integer): Boolean; overload;
    function CreateField(const HeroDBName, TableName, FieldName: string; Default: Integer; Len: Byte): Boolean; overload;

    // ÐÞ¸Ä×Ö¶Î -- piaoyun 2013-08-17
    function ModifyField(const HeroDBName, TableName, FieldName: string;{ Default: Integer; }Len: Byte): Boolean;
  end;
implementation

constructor THeroDB.Create;
begin
  CoInitialize(nil);
  Session := TSession.Create(nil);
  SessionList := TStringList.Create;
  //Session.Active := True;
  Session.AutoSessionName := True;
  Session.GetAliasNames(SessionList);
end;

destructor THeroDB.Destroy;
var
  I, II: Integer;
  StringList: TStringList;
begin
  for I := 0 to SessionList.Count - 1 do begin
    if SessionList.Objects[I] <> nil then begin
      StringList := TStringList(SessionList.Objects[I]);
      for II := 0 to StringList.Count - 1 do begin
        if StringList.Objects[II] <> nil then
          StringList.Objects[II].Free;
      end;
      SessionList.Objects[I].Free;
    end;
  end;
  SessionList.Free;
  Session.Close;
  Session.Free;
  CoUnInitialize;
  inherited;
end;

procedure THeroDB.Clear;
var
  I, II: Integer;
  StringList: TStringList;
begin
  for I := 0 to SessionList.Count - 1 do begin
    if SessionList.Objects[I] <> nil then begin
      StringList := TStringList(SessionList.Objects[I]);
      for II := 0 to StringList.Count - 1 do begin
        if StringList.Objects[II] <> nil then
          StringList.Objects[II].Free;
      end;
      SessionList.Objects[I].Free;
    end;
  end;
  SessionList.Clear;
  Session.GetAliasNames(SessionList);
end;

function THeroDB.GetTables(const HeroDBName: string): TStrings;
var
  I: Integer;
  TableList: TStringList;
begin
  Result := nil;
  I := SessionList.IndexOf(HeroDBName);
  if I >= 0 then begin
    if SessionList.Objects[I] = nil then begin
      TableList := TStringList.Create;
      SessionList.Objects[I] := TableList;
      Session.GetTableNames(SessionList.Strings[I], '', False, False, TableList);
    end;
    Result := TStrings(SessionList.Objects[I]);
  end;
end;

function THeroDB.GetFields(const HeroDBName, TableName: string): TStrings;
var
  Index: Integer;
  FieldList: TStrings;
  TableList: TStrings;
begin
  Result := nil;
  TableList := GetTables(HeroDBName);
  if TableList <> nil then begin
    Index := TableList.IndexOf(TableName);
    if Index >= 0 then begin
      if TableList.Objects[Index] = nil then begin
        FieldList := TStringList.Create;
        TableList.Objects[Index] := FieldList;
        Session.GetFieldNames(HeroDBName, TableName, FieldList);
      end;
      Result := TStrings(TableList.Objects[Index]);
    end;
  end;
end;

function THeroDB.HeroDBExist(const HeroDBName: string): Boolean;
begin
  Result := SessionList.IndexOf(HeroDBName) >= 0;
end;

function THeroDB.TableExist(const HeroDBName, TableName: string): Boolean;
var
  TableList: TStrings;
begin
  Result := False;
  TableList := GetTables(HeroDBName);
  if TableList <> nil then
    Result := TableList.IndexOf(TableName) >= 0;
end;

function THeroDB.FieldExist(const HeroDBName, TableName, FieldName: string): Boolean;
var
  FieldList: TStrings;
begin
  Result := False;
  FieldList := GetFields(HeroDBName, TableName);
  if FieldList <> nil then
    Result := FieldList.IndexOf(FieldName) >= 0;
end;

function THeroDB.CreateField(const HeroDBName, TableName, FieldName: string; Default: string; Len: Integer): Boolean;
var
  Query: TQuery;
  sSQL: string;
begin
  Result := False;
  sSQL := 'Alter Table ' + TableName + ' Add ' + FieldName + ' VarChar(' + IntToStr(Len) + ')';

  Query := TQuery.Create(nil);
  Query.DatabaseName := HeroDBName;
  Query.SQL.Clear;
  Query.SQL.Add(sSQL);
  try
    Query.ExecSQL;
    if Default <> '' then begin
      sSQL := 'Update ' + TableName + ' Set ' + FieldName + '=' + Default;
      Query.Close;
      Query.SQL.Clear;
      Query.SQL.Add(sSQL);
      Query.ExecSQL;
      Result := True;
    end;
  except

  end;
  Query.Free;
end;

function THeroDB.ModifyField(const HeroDBName, TableName, FieldName: string;{ Default: Integer; }Len: Byte): Boolean;
var
  Query: TQuery;
  sSQL: string;
begin
  Result := False;
  //sSQL := 'ALTER TABLE  ' + TableName + ' ALTER ' + FieldName + ' SET DATA TYPE VarChar(' + IntToStr(Len) + ')';
  sSQL := 'ALTER TABLE  ' + TableName + ' ADD column Name1 ' + 'Char(' + IntToStr(Len) + ')';
  {
  alter table tab add column name1 char(30)
  update tab set name1=name
  alter table tab drop column name
  }

  Query := TQuery.Create(nil);
  Query.DatabaseName := HeroDBName;
  Query.SQL.Clear;
  Query.SQL.Text := sSQL;
  Query.ExecSQL;

  sSQL := 'update ' + TableName + ' set Name1 = ' + FieldName;
  Query.SQL.Clear;
  Query.SQL.Text := sSQL;
  Query.ExecSQL;

  try
    //Query.ExecSQL;
    {if Default <> '' then begin
      sSQL := 'Update ' + TableName + ' Set ' + FieldName + '=' + Default;
      Query.Close;
      Query.SQL.Clear;
      Query.SQL.Add(sSQL);
      Query.ExecSQL;
      Result := True;
    end;}
  except

  end;
  Query.Free;
end;

function THeroDB.CreateField(const HeroDBName, TableName, FieldName: string; Default: Integer; Len: Byte): Boolean;
var
  Query: TQuery;
  sSQL: string;
begin
  Result := False;
  sSQL := 'Alter Table ' + TableName + ' Add ' + FieldName;
  if Len > 2 then
    sSQL := sSQL + ' Integer'
  else
    sSQL := sSQL + ' SmallInt';

  Query := TQuery.Create(nil);
  Query.DatabaseName := HeroDBName;
  Query.SQL.Clear;
  Query.SQL.Add(sSQL);

  try
    Query.ExecSQL;
    sSQL := 'Update ' + TableName + ' Set ' + FieldName + '=' + IntToStr(Default);
    Query.Close;
    Query.SQL.Clear;
    Query.SQL.Add(sSQL);
    Query.ExecSQL;
    Result := True;
  except

  end;
  Query.Free;
end;

procedure THeroDB.DeleteAlias(const Name: string);
begin
  Session.DeleteAlias(Name);
  Session.SaveConfigFile;
end;

procedure THeroDB.SaveHeroDBConfigFile(const Name, Path: string);
var
  I, II: Integer;
  StringList: TStringList;
begin
  for I := 0 to SessionList.Count - 1 do begin
    if SessionList.Objects[I] <> nil then begin
      StringList := TStringList(SessionList.Objects[I]);
      for II := 0 to StringList.Count - 1 do begin
        if StringList.Objects[II] <> nil then
          StringList.Objects[II].Free;
      end;
      SessionList.Objects[I].Free;
    end;
  end;
  SessionList.Clear;

  Session.DeleteAlias(Name);
  Session.AddStandardAlias(Name, Path, 'Paradox');
  Session.SaveConfigFile;

  Session.GetAliasNames(SessionList);
end;

end.

