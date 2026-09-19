unit uSqliteDB;

interface

uses
  Windows, Classes, SysUtils, SQLite3DataBase, Dialogs, SQLiteCli;

  function ProcessSqliteDB(FileName: string): Boolean;

implementation

function ProcessSqliteDB(FileName: string): Boolean;
var
  DB: TSQLite3Database;
  sm: TSQLStatement;
  nRet: Integer;
  ColumnNames: TStringList;
  I, Count: Integer;
begin
  Result := False;

  if not FileExists(FileName) then
  begin
    ShowMessage('Sqlite数据库: ' + FileName + ' 不存在');
    Exit;
  end;

  DB := TSQLite3Database.Create();
  try
    DB.MustExist := True;
    DB.Database := FileName;
    DB.Connected := True;

    sm := DB.Statements.AddSQLStatement('check_table_StdItems');
    sm.Sql := 'select 1 from sqlite_master where type = "table" and lower(name) = "stditems";';
    sm.Prepare;
    nRet := sm.Step;
    sm.Reset;

    if nRet <> SQLITE_ROW then
    begin
      ShowMessage('数据库中的表: StdItems 不存在');
      Exit;
    end;

    sm := DB.Statements.AddSQLStatement('CheckFieldExists');
    sm.Sql := 'select * from StdItems LIMIT 0;';
    sm.Prepare;
    sm.Reset;
    if sm.Step = SQLITE_DONE then
    begin
      ColumnNames := TStringList.Create;
      try
        Count := sm.GetColumnCount;
        for I := 0 to Count - 1 do
        begin
          ColumnNames.Add(sm.GetColumnName(I));
        end;

        ColumnNames.Sorted := True;

        // 扩展三个字段 chongchong 2017-04-16
        if ColumnNames.IndexOf('Element21') < 0 then
        begin
          DB.Execute('ALTER TABLE StdItems ADD COLUMN Element21 INTEGER;');
        end;

        if ColumnNames.IndexOf('Element22') < 0 then
        begin
          DB.Execute('ALTER TABLE StdItems ADD COLUMN Element22 INTEGER;');
        end;

        if ColumnNames.IndexOf('Element23') < 0 then
        begin
          DB.Execute('ALTER TABLE StdItems ADD COLUMN Element23 INTEGER;');
        end;

        if ColumnNames.IndexOf('Element24') < 0 then
        begin
          DB.Execute('ALTER TABLE StdItems ADD COLUMN Element24 INTEGER;');
        end;

      finally
        ColumnNames.Free;
      end;
    end;

    sm.Reset;

    sm := DB.Statements.AddSQLStatement('check_table_Monster');
    sm.Sql := 'select 1 from sqlite_master where type = "table" and lower(name) = "monster";';
    sm.Prepare;
    nRet := sm.Step;
    sm.Reset;

    if nRet <> SQLITE_ROW then
    begin
      ShowMessage('数据库中的表: Monster 不存在');
      Exit;
    end;

    sm := DB.Statements.AddSQLStatement('CheckFieldExists_Monster');
    sm.Sql := 'select * from Monster LIMIT 0;';
    sm.Prepare;
    sm.Reset;
    if sm.Step = SQLITE_DONE then
    begin
      ColumnNames := TStringList.Create;
      try
        Count := sm.GetColumnCount;
        for I := 0 to Count - 1 do
        begin
          ColumnNames.Add(sm.GetColumnName(I));
        end;

        ColumnNames.Sorted := True;

        // 扩展三个字段 chongchong 2017-04-16
        if ColumnNames.IndexOf('ExploreItem') < 0 then
        begin
          DB.Execute('ALTER TABLE Monster ADD COLUMN ExploreItem INTEGER default 0;');
        end;

        if ColumnNames.IndexOf('DisableSimpleActor') < 0 then
        begin
          DB.Execute('ALTER TABLE Monster ADD COLUMN DisableSimpleActor INTEGER default 0;');
        end;

      finally
        ColumnNames.Free;
      end;
    end;
    sm.Reset;

    sm := DB.Statements.AddSQLStatement('check_table_Magic');
    sm.Sql := 'select 1 from sqlite_master where type = "table" and lower(name) = "magic";';
    sm.Prepare;
    nRet := sm.Step;
    sm.Reset;

    if nRet <> SQLITE_ROW then
    begin
      ShowMessage('数据库中的表: Magic 不存在');
      Exit;
    end;

    Result := True;
  finally
    DB.Free;
  end;
end;


end.
