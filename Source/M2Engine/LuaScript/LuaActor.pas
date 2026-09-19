unit LuaActor;

interface

uses
  Classes, SysUtils, lua, pLuaObject, plua, ObjBase, ObjPlayer, StdCtrls;

procedure RegisterLuaActor(L : Plua_State);
procedure RegisterExistingActor({L : Plua_State;} InstanceName : String; Instance : TBaseObject);

implementation

uses
  LuaScript;
type

  { TBaseObjectDelegate }

  TBaseObjectDelegate = class(TLuaObjectEventDelegate)
  public
    constructor Create(InstanceInfo : PLuaInstanceInfo; obj : TObject); override;
    destructor Destroy; override;

    procedure ClickHandler(Sender : TObject);
  end;

{ TActorDelegate }

constructor TBaseObjectDelegate.Create(InstanceInfo: PLuaInstanceInfo; obj : TObject);
begin
  inherited Create(InstanceInfo, obj);
//  TActor(obj).OnClick := ClickHandler;
end;

destructor TBaseObjectDelegate.Destroy;
begin
//  TActor(fobj).OnClick := nil;
  inherited Destroy;
end;

procedure TBaseObjectDelegate.ClickHandler(Sender: TObject);
begin
  CallEvent('OnClick');
end;

var
  ActorInfo : TLuaClassInfo;

//function newActor(l : PLua_State; paramidxstart, paramcount : Integer; InstanceInfo : PLuaInstanceInfo) : TObject;
//begin
//  result := TActor.Create(frmMain);
//  TActor(Result).Parent := frmMain;
//  TActor(Result).Visible := true;
//  TActorDelegate.Create(InstanceInfo, result);
//end;


function TActor_GetGold(target : TObject; l : Plua_State; paramidxstart, paramcount : integer) : Integer;
var
  BaseObject : TBaseObject;
begin
  BaseObject := TBaseObject(target);
  lua_pushinteger(l, BaseObject.m_nGold);
  result := 1;
end;

function TActor_SetGold(target : TObject; l : Plua_State; paramidxstart, paramcount : integer) : Integer;
var
  BaseObject : TBaseObject;
begin
  BaseObject := TBaseObject(target);
  BaseObject.m_nGold := lua_tointeger(l, paramidxstart);
  TPlayObject(BaseObject).GoldChanged;
//  btn.Caption := lua_tostring(l, paramidxstart);
  result := 0;
end;

function TActor_LevelUp(target : TObject; l : Plua_State; paramidxstart, paramcount : integer) : Integer;
var
  BaseObject : TBaseObject;
begin
  result := 0;
  BaseObject := TBaseObject(target);
  plua_CallObjectEvent(plua_GetObjectInfo(l, BaseObject), 'levelup', []);
end;

procedure RegisterLuaActor(L: Plua_State);
begin
  plua_registerclass(L, ActorInfo);
end;

procedure RegisterExistingActor({L: Plua_State;} InstanceName : String; Instance: TBaseObject);
begin
  TBaseObjectDelegate.Create(plua_registerExisting(LuaSystem.FLua.LuaState, InstanceName, Instance, @ActorInfo), Instance);
end;

function setActorInfo : TLuaClassInfo;
begin
  plua_initClassInfo(result);
  result.ClassName := 'TActor';
//  result.New := @newActor;
  plua_AddClassProperty(result, 'gold', @TActor_GetGold, @TActor_SetGold);



  {
  函数表

  RecalcAbilitys()                            重载Actor属性
  ReAlive(isDelay: Boolean)                   复活Actor

  }
  plua_AddClassMethod(result, 'levelup', @TActor_LevelUp);


//  plua_AddClassProperty(result, 'Left', @GetLeft, @SetLeft);
//  plua_AddClassProperty(result, 'Top', @GetTop, @SetTop);
//  plua_AddClassProperty(result, 'Width', @GetWidth, @SetWidth);
//  plua_AddClassProperty(result, 'Height', @GetHeight, @SetHeight);
//  plua_AddClassProperty(result, 'Visible', @GetVisible, @SetVisible);
//  plua_AddClassProperty(result, 'Enabled', @GetEnabled, @SetEnabled);
end;

initialization
  ActorInfo := setActorInfo;

finalization

end.

