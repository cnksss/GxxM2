unit LuaEvent;

interface

uses
  Lua, pLua, LuaWrapper, LuaObject, System.SysUtils, ObjBase, ObjPlayer;

const
  LUAEVENTCOUNT = 60;

//type
//  TLuaServerEvent = class(TLuaObject)
//  public
////  published
//  end;

var
  sLuaEvent: array[0..LUAEVENTCOUNT - 1] of string;


function DoEvent(nIndex: Integer): Boolean; overload;//执行lua事件命令
function DoEvent(nIndex: Integer; BaseObject: TPlayObject): Boolean; overload;
procedure RegisterLuaEvent(L : Plua_State);

implementation
uses
  LuaScript;

function bind(l: PLua_State): Integer; cdecl;
var
  event : TLuaObject;
  FuncType: Integer;
  FuncName: string;
begin
  result := 0;
  if (lua_gettop(l) < 1) then
    exit;
  event := TLuaObject(LuaToTLuaObject(l, 1));
  FuncType := Lua_ToInteger(l, 2);
  FuncName := lua_tostring(l, 3);
  sLuaEvent[FuncType] := FuncName;
end;

function event_new(L : PLua_State; AParent : TLuaObject=nil):TLuaObject;
begin
  result := TLuaObject.Create(L, AParent);
end;

function new_event(L : PLua_State) : Integer; cdecl;
var
  p : TLuaObjectNewCallback;
begin
  p := @event_new;
  result := new_LuaObject(L, 'TEvent', p);
end;

procedure methods_event(L : Plua_State; classTable : Integer);
begin
  RegisterMethod(L, 'bind', @bind, classTable);
end;

procedure RegisterLuaEvent(L: Plua_State);
begin
  RegisterTLuaObject(L, 'TEvent', @new_event, @methods_event);
end;

function DoEvent(nIndex: Integer): Boolean;//执行lua事件命令
var
  L: PLua_State;
  sName: string;
begin
  Result := False;
  sName := sLuaEvent[nIndex];
  if sName = '' then
    Exit;
  if LuaSystem.FLua.FunctionExists(sName) then
  begin
    LuaSystem.FLua.CallFunction(sName, [1, 2]);
  end;
end;

function DoEvent(nIndex: Integer; BaseObject: TPlayObject): Boolean;//执行lua事件命令
var
  L: PLua_State;
  sName: string;
begin
  Result := False;
  sName := sLuaEvent[nIndex];
  if sName = '' then
    Exit;
  if LuaSystem.FLua.FunctionExists(sName) then
  begin
    LuaSystem.FLua.CallFunction(sName, [1, 2]);
  end;
end;

end.
