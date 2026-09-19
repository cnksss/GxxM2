unit MySQLWrap;

interface

uses
  Classes, SysUtils, SyncObjs, MySQLCli, MySQLUtil;

type
  TMySQLLib = class(TMyLibrary)
  private
    FLock: TCriticalSection;
  protected
    procedure LoadEntries; override;
  public
    mysql_num_fields: TPrcmysql_num_fields;
    mysql_fetch_field_direct: TPrcmysql_fetch_field_direct;
    mysql_affected_rows: TPrcmysql_affected_rows;
    mysql_insert_id: TPrcmysql_insert_id;
    mysql_errno: TPrcmysql_errno;
    mysql_error: TPrcmysql_error;
    mysql_sqlstate: TPrcmysql_sqlstate;
    mysql_warning_count: TPrcmysql_warning_count;
    mysql_info: TPrcmysql_info;
    mysql_character_set_name: TPrcmysql_character_set_name;
    mysql_get_character_set_info: TPrcmysql_get_character_set_info;
    mysql_set_character_set: TPrcmysql_set_character_set;
    mysql_init: TPrcmysql_init;
    mysql_ssl_set: TPrcmysql_ssl_set;
    mysql_get_ssl_cipher: TPrcmysql_get_ssl_cipher;
    mysql_connect: TPrcmysql_connect;
    mysql_real_connect: TPrcmysql_real_connect;
    mysql_close: TPrcmysql_close;
    mysql_select_db: TPrcmysql_select_db;

    mysql_query: TPrcmysql_query;
    mysql_real_query: TPrcmysql_real_query;
    mysql_kill: TPrcmysql_kill;
    mysql_ping: TPrcmysql_ping;
    mysql_stat: TPrcmysql_stat;
    mysql_get_server_info: TPrcmysql_get_server_info;
    mysql_get_client_info: TPrcmysql_get_client_info;
    mysql_get_host_info: TPrcmysql_get_host_info;
    mysql_get_proto_info: TPrcmysql_get_proto_info;
    mysql_list_processes: TPrcmysql_list_processes;
    mysql_store_result: TPrcmysql_store_result;
    mysql_use_result: TPrcmysql_use_result;
    mysql_options_: TPrcmysql_options;
    mysql_free_result: TPrcmysql_free_result;
    mysql_fetch_row: TPrcmysql_fetch_row;
    mysql_fetch_lengths: TPrcmysql_fetch_lengths;
    mysql_escape_string: TPrcmysql_escape_string;
    mysql_real_escape_string: TPrcmysql_real_escape_string;
    mysql_thread_safe: TPrcmysql_thread_safe;
    mysql_more_results: TPrcmysql_more_results;
    mysql_next_result: TPrcmysql_next_result;
    mysql_server_init: TPrcmysql_server_init;
    mysql_server_end: TPrcmysql_server_end;
    mysql_thread_init: TPrcmysql_thread_init;
    mysql_thread_end: TPrcmysql_thread_end;
    mysql_thread_id: TPrcmysql_thread_id;

    mysql_stmt_init: TPrcmysql_stmt_init;
    mysql_stmt_prepare: TPrcmysql_stmt_prepare;
    mysql_stmt_execute: TPrcmysql_stmt_execute;
    mysql_stmt_fetch: TPrcmysql_stmt_fetch;
    mysql_stmt_fetch_column: TPrcmysql_stmt_fetch_column;
    mysql_stmt_store_result: TPrcmysql_stmt_store_result;
    mysql_stmt_param_count: TPrcmysql_stmt_param_count;
    mysql_stmt_attr_set: TPrcmysql_stmt_attr_set;
    mysql_stmt_attr_get: TPrcmysql_stmt_attr_get;
    mysql_stmt_bind_param: TPrcmysql_stmt_bind_param;
    mysql_stmt_bind_result: TPrcmysql_stmt_bind_result;
    mysql_stmt_close: TPrcmysql_stmt_close;
    mysql_stmt_reset: TPrcmysql_stmt_reset;
    mysql_stmt_free_result: TPrcmysql_stmt_free_result;
    mysql_stmt_send_long_data: TPrcmysql_stmt_send_long_data;
    mysql_stmt_result_metadata: TPrcmysql_stmt_result_metadata;
    mysql_stmt_errno: TPrcmysql_stmt_errno;
    mysql_stmt_error: TPrcmysql_stmt_error;
    mysql_stmt_sqlstate: TPrcmysql_stmt_sqlstate;
    mysql_stmt_row_seek: TPrcmysql_stmt_row_seek;
    mysql_stmt_row_tell: TPrcmysql_stmt_row_tell;
    mysql_stmt_data_seek: TPrcmysql_stmt_data_seek;
    mysql_stmt_num_rows: TPrcmysql_stmt_num_rows;
    mysql_stmt_affected_rows: TPrcmysql_stmt_affected_rows;
    mysql_stmt_insert_id: TPrcmysql_stmt_insert_id;
    mysql_stmt_field_count: TPrcmysql_stmt_field_count;
    mysql_stmt_next_result: TPrcmysql_stmt_next_result;
  public
    constructor Create(AOwningObj: TObject = nil);
    destructor Destroy; override;
    procedure Load(const AVendorHome, AVendorLib: string);
    procedure Unload; override;

    procedure Lock;
    procedure UnLock;
  end;

implementation

const
  smysql_get_client_info: string = 'mysql_get_client_info';
  smysql_num_fields: string = 'mysql_num_fields';
  smysql_fetch_field_direct: string = 'mysql_fetch_field_direct';
  smysql_affected_rows: string = 'mysql_affected_rows';
  smysql_insert_id: string = 'mysql_insert_id';
  smysql_errno: string = 'mysql_errno';
  smysql_error: string = 'mysql_error';
  smysql_sqlstate: string = 'mysql_sqlstate';
  smysql_warning_count: string = 'mysql_warning_count';
  smysql_info: string = 'mysql_info';
  smysql_character_set_name: string = 'mysql_character_set_name';
  smysql_get_character_set_info: string = 'mysql_get_character_set_info';
  smysql_set_character_set: string = 'mysql_set_character_set';
  smysql_init: string = 'mysql_init';
  smysql_connect: string = 'mysql_connect';
  smysql_ssl_set: string = 'mysql_ssl_set';
  smysql_get_ssl_cipher: string = 'mysql_get_ssl_cipher';
  smysql_real_connect: string = 'mysql_real_connect';
  smysql_close: string = 'mysql_close';
  smysql_select_db: string = 'mysql_select_db';

  smysql_query: string = 'mysql_query';
  smysql_real_query: string = 'mysql_real_query';
  smysql_kill: string = 'mysql_kill';
  smysql_ping: string = 'mysql_ping';
  smysql_stat: string = 'mysql_stat';
  smysql_get_server_info: string = 'mysql_get_server_info';
  smysql_get_host_info: string = 'mysql_get_host_info';
  smysql_get_proto_info: string = 'mysql_get_proto_info';
  smysql_list_processes: string = 'mysql_list_processes';
  smysql_store_result: string = 'mysql_store_result';
  smysql_use_result: string = 'mysql_use_result';
  smysql_options_: string = 'mysql_options';
  smysql_free_result: string = 'mysql_free_result';
  smysql_fetch_row: string = 'mysql_fetch_row';
  smysql_fetch_lengths: string = 'mysql_fetch_lengths';
  smysql_escape_string: string = 'mysql_escape_string';
  smysql_real_escape_string: string = 'mysql_real_escape_string';
  smysql_thread_safe: string = 'mysql_thread_safe';
  smysql_more_results: string = 'mysql_more_results';
  smysql_next_result: string = 'mysql_next_result';
  smysql_server_init: string = 'mysql_server_init';
  smysql_server_end: string = 'mysql_server_end';
  smysql_thread_init: string = 'mysql_thread_init';
  smysql_thread_end: string = 'mysql_thread_end';
  smysql_thread_id: string = 'mysql_thread_id';

  smysql_stmt_init: string = 'mysql_stmt_init';
  smysql_stmt_prepare: string = 'mysql_stmt_prepare';
  smysql_stmt_execute: string = 'mysql_stmt_execute';
  smysql_stmt_fetch: string = 'mysql_stmt_fetch';
  smysql_stmt_fetch_column: string = 'mysql_stmt_fetch_column';
  smysql_stmt_store_result: string = 'mysql_stmt_store_result';
  smysql_stmt_param_count: string = 'mysql_stmt_param_count';
  smysql_stmt_attr_set: string = 'mysql_stmt_attr_set';
  smysql_stmt_attr_get: string = 'mysql_stmt_attr_get';
  smysql_stmt_bind_param: string = 'mysql_stmt_bind_param';
  smysql_stmt_bind_result: string = 'mysql_stmt_bind_result';
  smysql_stmt_close: string = 'mysql_stmt_close';
  smysql_stmt_reset: string = 'mysql_stmt_reset';
  smysql_stmt_free_result: string = 'mysql_stmt_free_result';
  smysql_stmt_send_long_data: string = 'mysql_stmt_send_long_data';
  smysql_stmt_result_metadata: string = 'mysql_stmt_result_metadata';
  smysql_stmt_errno: string = 'mysql_stmt_errno';
  smysql_stmt_error: string = 'mysql_stmt_error';
  smysql_stmt_sqlstate: string = 'mysql_stmt_sqlstate';
  smysql_stmt_row_seek: string = 'mysql_stmt_row_seek';
  smysql_stmt_row_tell: string = 'mysql_stmt_row_tell';
  smysql_stmt_data_seek: string = 'mysql_stmt_data_seek';
  smysql_stmt_num_rows: string = 'mysql_stmt_num_rows';
  smysql_stmt_affected_rows: string = 'mysql_stmt_affected_rows';
  smysql_stmt_insert_id: string = 'mysql_stmt_insert_id';
  smysql_stmt_field_count: string = 'mysql_stmt_field_count';
  smysql_stmt_next_result: string = 'mysql_stmt_next_result';

  smariadb_connection: string = 'mariadb_connection';
  smysql_get_socket: string = 'mysql_get_socket';

{ TMySQLLib }

constructor TMySQLLib.Create(AOwningObj: TObject);
begin
  FLock := TCriticalSection.Create;
  inherited Create('MySQL', AOwningObj);
end;

destructor TMySQLLib.Destroy;
begin
  FLock.Free;
  inherited Destroy;
end;

procedure TMySQLLib.Load(const AVendorHome, AVendorLib: string);
const
  C_MysqlDll: string = {$IFDEF MSWINDOWS} 'libmysql' {$ENDIF}
                       {$IFDEF POSIX} 'libmysqlclient' {$ENDIF} + '.dll';
  C_MysqldDll: string = 'libmysqld' + '.dll';
  C_MysqlDllForlder: string = 'lib';
var
  sDLLName: string;
  aMySQLDllNames: array of string;
begin
  sDLLName := AVendorHome;
  if sDLLName <> '' then
    sDLLName := IncludeTrailingPathDelimiter(IncludeTrailingPathDelimiter(sDLLName) + C_MysqlDllForlder);

  if AVendorLib <> '' then
  begin
    SetLength(aMySQLDllNames, 1);
    aMySQLDllNames[0] := sDLLName + AVendorLib;
  end
  else
  begin
    SetLength(aMySQLDllNames, 2);
    aMySQLDllNames[0] := sDLLName + C_MysqlDll;
    aMySQLDllNames[1] := sDLLName + C_MysqldDll;
  end;

  inherited Load(aMySQLDllNames, True);
end;

procedure TMySQLLib.Unload;
begin
   inherited Unload;
end;

procedure TMySQLLib.LoadEntries;
begin
  @mysql_get_client_info := GetProc(smysql_get_client_info);
  FVersion := MyVerStr2Int(mysql_get_client_info());
  if FVersion < mvMySQL032000 then
    raise Exception.Create('MySql version to lower: ' + IntToStr(FVersion));

  @mysql_num_fields := GetProc(smysql_num_fields);
  @mysql_fetch_field_direct := GetProc(smysql_fetch_field_direct);
  @mysql_affected_rows := GetProc(smysql_affected_rows);
  @mysql_insert_id := GetProc(smysql_insert_id);
  @mysql_errno := GetProc(smysql_errno);
  @mysql_error := GetProc(smysql_error);
  if FVersion >= mvMySQL040101 then begin
    @mysql_sqlstate := GetProc(smysql_sqlstate);
    @mysql_warning_count := GetProc(smysql_warning_count);
  end;
  @mysql_info := GetProc(smysql_info);
  if FVersion >= mvMySQL032321 then
    @mysql_character_set_name := GetProc(smysql_character_set_name, False);
  if FVersion >= mvMySQL050010 then
    @mysql_get_character_set_info := GetProc(smysql_get_character_set_info);
  if FVersion >= mvMySQL050007 then
    @mysql_set_character_set := GetProc(smysql_set_character_set);
  @mysql_init := GetProc(smysql_init);

  //if not FMySQLEmbedded then
  begin
    if FVersion < mvMySQL040000 then
      @mysql_connect := GetProc(smysql_connect)
    else
      @mysql_ssl_set := GetProc(smysql_ssl_set);
    if FVersion >= mvMySQL050023 then
      @mysql_get_ssl_cipher := GetProc(smysql_get_ssl_cipher, False);
  end;

  @mysql_real_connect := GetProc(smysql_real_connect);
  @mysql_close := GetProc(smysql_close);
  @mysql_select_db := GetProc(smysql_select_db);
  @mysql_query := GetProc(smysql_query);
  @mysql_real_query := GetProc(smysql_real_query);
  @mysql_kill := GetProc(smysql_kill);
  @mysql_ping := GetProc(smysql_ping);
  @mysql_stat := GetProc(smysql_stat);
  @mysql_get_server_info := GetProc(smysql_get_server_info);
  @mysql_get_host_info := GetProc(smysql_get_host_info);
  @mysql_get_proto_info := GetProc(smysql_get_proto_info);
  @mysql_list_processes := GetProc(smysql_list_processes);
  @mysql_store_result := GetProc(smysql_store_result);
  @mysql_use_result := GetProc(smysql_use_result);
  @mysql_options_ := GetProc(smysql_options_);
  @mysql_free_result := GetProc(smysql_free_result);
  @mysql_fetch_row := GetProc(smysql_fetch_row);
  @mysql_fetch_lengths := GetProc(smysql_fetch_lengths);
  @mysql_escape_string := GetProc(smysql_escape_string);
  if FVersion >= mvMySQL032314 then begin
    @mysql_real_escape_string := GetProc(smysql_real_escape_string, False);
    @mysql_thread_safe := GetProc(smysql_thread_safe, False);
  end;
  if FVersion >= mvMySQL040101 then begin
    @mysql_more_results := GetProc(smysql_more_results);
    @mysql_next_result := GetProc(smysql_next_result);
  end;

  {
  if FMySQLEmbedded then begin
    @mysql_server_init := GetProc(smysql_server_init);
    @mysql_server_end := GetProc(smysql_server_end);
  end;
  }

  @mysql_thread_init := GetProc(smysql_thread_init, False);
  @mysql_thread_end := GetProc(smysql_thread_end, False);
  @mysql_thread_id := GetProc(smysql_thread_id);

  if FVersion >= mvMySQL050000 then begin
    @mysql_stmt_init := GetProc(smysql_stmt_init);
    @mysql_stmt_prepare := GetProc(smysql_stmt_prepare);
    @mysql_stmt_execute := GetProc(smysql_stmt_execute);
    @mysql_stmt_fetch := GetProc(smysql_stmt_fetch);
    @mysql_stmt_fetch_column := GetProc(smysql_stmt_fetch_column);
    @mysql_stmt_store_result := GetProc(smysql_stmt_store_result);
    @mysql_stmt_param_count := GetProc(smysql_stmt_param_count);
    @mysql_stmt_attr_set := GetProc(smysql_stmt_attr_set);
    @mysql_stmt_attr_get := GetProc(smysql_stmt_attr_get);
    @mysql_stmt_bind_param := GetProc(smysql_stmt_bind_param);
    @mysql_stmt_bind_result := GetProc(smysql_stmt_bind_result);
    @mysql_stmt_close := GetProc(smysql_stmt_close);
    @mysql_stmt_reset := GetProc(smysql_stmt_reset);
    @mysql_stmt_free_result := GetProc(smysql_stmt_free_result);
    @mysql_stmt_send_long_data := GetProc(smysql_stmt_send_long_data);
    @mysql_stmt_result_metadata := GetProc(smysql_stmt_result_metadata);
    @mysql_stmt_errno := GetProc(smysql_stmt_errno);
    @mysql_stmt_error := GetProc(smysql_stmt_error);
    @mysql_stmt_sqlstate := GetProc(smysql_stmt_sqlstate);
    @mysql_stmt_row_seek := GetProc(smysql_stmt_row_seek);
    @mysql_stmt_row_tell := GetProc(smysql_stmt_row_tell);
    @mysql_stmt_data_seek := GetProc(smysql_stmt_data_seek);
    @mysql_stmt_num_rows := GetProc(smysql_stmt_num_rows);
    @mysql_stmt_affected_rows := GetProc(smysql_stmt_affected_rows);
    @mysql_stmt_insert_id := GetProc(smysql_stmt_insert_id);
    @mysql_stmt_field_count := GetProc(smysql_stmt_field_count);
    if FVersion >= mvMySQL050503 then
      @mysql_stmt_next_result := GetProc(smysql_stmt_next_result);
  end;

  {
  if (GetProc(smariadb_connection, False) <> nil) or
     (GetProc(smysql_get_socket, False) <> nil) then
    FBrand := mbMariaDB
  else
    FBrand := mbMySQL;
  }
end;

procedure TMySQLLib.Lock;
begin
  FLock.Enter;
end;

procedure TMySQLLib.UnLock;
begin
  FLock.Leave;
end;

end.
