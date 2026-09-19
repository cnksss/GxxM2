unit LuaScript;

interface

uses
  Lua, pLua, LuaWrapper, LuaObject, System.SysUtils, ObjBase, ObjPlayer;

//const
//  LINEFEED = #13#10;
//  BASELUASCRIPT = 's = {event = {}, share = {}, obj = {}, ini = {}}' + LINEFEED + 's.event = require "event";' + LINEFEED + 's.share = require "share";' + LINEFEED + 's.obj = require "obj";' + LINEFEED + 's.ini = require "ini";' + LINEFEED + 'package.path = package.path .. '';.\\Envir\\LuaScript\\?.lua'';';
//  LUAEVENTCOUNT = 60;

type
  TLuaScript = class
  public
    FLua: TLua;
    constructor Create;
    destructor Destroy;
//    procedure Open; override;
    procedure DebugOut();
  end;

  {TLuaServerEvent 服务端事件管理}
var
  LuaSystem: TLuaScript; //lua脚本支持 By 一支笔 at:2021-06-30 18:05:42

function print(L: PLua_State): Integer; cdecl;

procedure LoadLuaScript();

procedure UnLoadLuaScript();

implementation

uses
  M2Share, M2Definition, LuaEvent, LuaActor;

function print(L: PLua_State): Integer;
var
  N, I: Integer;
  S: MarshaledAString;
  Sz: size_t;
  Msg: String;
begin
  Msg := '';

  N := lua_gettop(L);  //* number of arguments */
  lua_getglobal(L, 'tostring');
  for I := 1 to N do
  begin
    lua_pushvalue(L, -1);  //* function to be called */
    lua_pushvalue(L, i);   //* value to print */
    lua_call(L, 1, 1);
    S := lua_tolstring(L, -1, @Sz);  //* get result */
    if S = NIL then
    begin
      Result := luaL_error(L, '"tostring" must return a string to "print"',[]);
      Exit;
    end;

    if I > 1 then
      Msg := Msg + #9;
    Msg := Msg + String(S);
    lua_pop(L, 1);  //* pop result */
  end;
  Result := 0;
  MainOutMessage(Msg);
end;

procedure LoadLuaScript();
begin
  LuaSystem := TLuaScript.Create;
end;

procedure UnLoadLuaScript();
begin
  LuaSystem.Free;
end;

procedure TLuaScript.DebugOut();
var
  I: Integer;
  nType: Integer;
begin
  for I := 0 to lua_gettop(FLua.LuaState) do
  begin
    nType := lua_type(FLua.LuaState, I);
    MainOutMessage(format('   (%d)  %s         %s\n', [I, lua_typename(FLua.LuaState, nType), lua_tostring(FLua.LuaState, I)]));
  end;
end;

constructor TLuaScript.Create;
var
  p : TLuaObjectNewCallback;
begin
  inherited;
  FLua := TLua.Create;
  FLua.RegisterLuaMethod('print', @Print);
  FLua.LuaPath := g_Config.sEnvirDir + 'LuaScript\?.lua';
  RegisterLuaEvent(FLua.LuaState);
  RegisterLuaActor(FLua.LuaState);
//  RegisterTLuaObject(FLua.LuaState, 'TEvent', @new_event, @methods_event);

  FLua.ExecuteFile(g_Config.sEnvirDir + 'LuaScript\main.lua');
end;

destructor TLuaScript.Destroy;
begin
  FLua.Free;
  inherited;
end;

end.

