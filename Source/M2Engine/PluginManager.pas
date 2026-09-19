unit PluginManager;

// ----------------------------------------------------------------------------------------------------------------------
// M2Server插件管理
//
// 版 本 号: 1.1
// 发布日期: 2018-03-23
// 更新记录:
// 2018-03-23: 初次整理
// ----------------------------------------------------------------------------------------------------------------------
interface

uses
  Windows, Classes, SysUtils, PluginInterface, PluginImplement, MemoryModuleEx,
  // MemoryModule, MemoryModuleDef, //这两个国人改过的，时灵时不灵，还会崩溃，换成Github的原版 20230314 HZQ
{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
{$IF CompilerVersion >= 22}
  System.Types,
{$IFEND}
  IniFiles, ObjBase, ObjPlayer, ObjNpc, ObjDummy, ObjHero, Menus;

type
  // 插件初始化
  TPlugInit = function(AppFunc: PAppFuncDef; AppFuncCrc: DWORD; ExtParam: Integer; Desc: PAnsiChar; var DescLen: DWORD)
    : BOOL; stdcall;
  // 插件反初始化
  TPlugUnInit = procedure(); stdcall;
  // IP归属地查询
  TPlugHookGetIPLocal = function(sIPaddr: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
  // 引擎准备启动
  TPlugHookEngineReadyToStart = procedure(); stdcall;
  // 引擎启动完成
  TPlugHookEngineStartComplete = procedure(); stdcall;
  // 重新加载信息完成 控制-> 重新加载 ->... 加载完成
  TPlugHookEngineReloadComplete = procedure(ReloadType: Integer); stdcall;
  // 载入脚本文件, 数据需要使用ZIP压缩然后使用M2Server_EncryBuffer加密后才能写入Stream
  TPlugHookLoadScriptFile = function(FileName: PAnsiChar; MemStream: TMemoryStream): BOOL; stdcall;
  // 载入脚本文件, 数据需要使用ZIP压缩然后使用M2Server_EncryBuffer加密后才能写入Stream
  TPlugHookCloneCommonFile = function(FileName: PAnsiChar; MemStream: TMemoryStream): BOOL; stdcall;
  TPlugHookChangeCommonFile = function(FileName: PAnsiChar; btMode: Byte; ExtString: PAnsiChar): BOOL; stdcall;
  // 解密脚本文件
  TPlugHookDecryptScriptFile = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
  // 解密脚本单行
  TPlugHookDecryptScriptLine = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
  TPlugHookNpcCmdLoading = function(pCmd: PAnsiChar): Integer; stdcall;
  TPlugHookConditionProcess = function(ScriptParam: PScriptCmdParam): BOOL; stdcall;
  TPlugHookActionProcess = function(ScriptParam: PScriptCmdParam; var boSendNpcSay, boBreak: BOOL): BOOL; stdcall;
  TPlugHookUserSelect = function(Merchant: TMerchant; PlayObject: TPlayObject; sLabel, sMsg: PAnsiChar): BOOL; stdcall;
  TPlugHookUserCommand = function(PlayObject: TPlayObject; pCmd, pParam1, pParam2, pParam3, pParam4, pParam5, pParam6,
    pParam7: PAnsiChar): BOOL; stdcall;
  TPlugHookGetVariableText = function(NPC: TNormNpc; PlayObject: TPlayObject; sVariable: PAnsiChar; sValue: PAnsiChar;
    var nValueLen: DWORD): BOOL; stdcall;
  TPlugHookBaseObjectAction = procedure(BaseObject: TBaseObject); stdcall;
  // 2021-01-05 changed
  TPlugHookBaseObjectProcessMsg = procedure(BaseObject: TBaseObject; wIdent: Word; wParam: Integer;
    nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar;
    var boReturn: BOOL); stdcall;
  TPlugHookBaseObjectStruck = procedure(BaseObject: TBaseObject; AttackObject: TBaseObject); stdcall;
  TPlugHookBaseObjectMagicStruck = procedure(BaseObject: TBaseObject; AttackObject: TBaseObject; MagicIdx: Integer); stdcall;
  TPlugHookBaseObjectAttack = procedure(BaseObject: TBaseObject; Target: TBaseObject; MagicIdx: Integer;
    var nPower: Integer); stdcall;
  TPlugHookBaseObjectMagicAttack = procedure(BaseObject: TBaseObject; Target: TBaseObject; MagicIdx: Integer;
    var nPower: Integer); stdcall;
  TPlugHookPlayerAction = procedure(PlayObject: TPlayObject); stdcall;
  TPlugHookPlayerViewRangeNewObject = procedure(PlayeObject: TPlayObject; AObject: TObject; AObjectX, AObjectY: Integer); stdcall;
  // 2021-01-05 changed
  TPlugHookPlayerProcessMsg = procedure(PlayObject: TPlayObject; wIdent: Word; wParam: Integer;
    nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar;
    var boReturn: BOOL); stdcall;
  TPlugHookDummyObjectRunBegin = procedure(DummyObject: TDummyObject; var boReturn: BOOL); stdcall;
  TPlugHookDummyObjectAction = procedure(DummyObject: TDummyObject); stdcall;
  TPlugHookHeroObjectAction = procedure(Hero: THeroObject); stdcall;
  TPluginManager = class;

  TMenuItemEx = class(TObject)
  private
    FList: TList;
    FMenuItem: TMenuItem;
    FNotifyEventMethod: PNotifyEventMethod;
    function GetCount: Integer;
    function GetItems(Index: Integer): TMenuItemEx;
  public
    constructor Create;
    destructor Destroy; override;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: TMenuItemEx read GetItems;
    function IndexOf(MenuItem: TMenuItem): Integer;
    procedure Add(Item: TMenuItem; NotifyEventMethod: PNotifyEventMethod);
    procedure Clear;
  end;

  TPlugin = class(TObject)
  private
    FOwner: TPluginManager;
    FFileName: string;
    FPlugDesc: string;
    FIsMemLoad: Boolean;
    FModule: THandle;
    FIsInitOK: Boolean;
    FBaseCode: Integer;
    FMenuItems: TMenuItemEx;
    procedure UnLoad;
  public
    HookGetIPLocal: TPlugHookGetIPLocal;
    HookEngineReadyToStart: TPlugHookEngineReadyToStart;
    HookEngineStartComplete: TPlugHookEngineStartComplete;
    HookEngineReloadComplete: TPlugHookEngineReloadComplete;
    HookLoadScriptFile: TPlugHookLoadScriptFile;
    HookCloneCommonFile: TPlugHookCloneCommonFile;
    HookChangeCommonFile: TPlugHookChangeCommonFile;
    HookDecryptScriptFile: TPlugHookDecryptScriptFile;
    HookDecryptScriptLine: TPlugHookDecryptScriptLine;
    HookNpcLoadConditionCmd: TPlugHookNpcCmdLoading;
    HookNpcConditionProcess: TPlugHookConditionProcess;
    HookNpcLoadActionCmd: TPlugHookNpcCmdLoading;
    HookNpcActionProcess: TPlugHookActionProcess;
    HookUserSelect: TPlugHookUserSelect;
    HookUserCommand: TPlugHookUserCommand;
    HookGetVariableText: TPlugHookGetVariableText;
    HookBaseObjectCreate: TPlugHookBaseObjectAction;
    HookBaseObjectRecalAbilBegin: TPlugHookBaseObjectAction;
    HookBaseObjectRecalAbilEnd: TPlugHookBaseObjectAction;
    HookBaseObjectRun: TPlugHookBaseObjectAction;
    HookBaseObjectProcessMsg: TPlugHookBaseObjectProcessMsg;
    HookBaseObjectStruck: TPlugHookBaseObjectStruck;
    HookBaseObjectMagicStruck: TPlugHookBaseObjectMagicStruck;
    HookBaseObjectAttack: TPlugHookBaseObjectAttack;
    HookBaseObjectMagicAttack: TPlugHookBaseObjectMagicAttack;
    HookBaseObjectDie: TPlugHookBaseObjectAction;
    HookBaseObjectMakeGhost: TPlugHookBaseObjectAction;
    HookBaseObjectFree: TPlugHookBaseObjectAction;
    HookPlayerCreate: TPlugHookPlayerAction;
    HookPlayerLogin1: TPlugHookPlayerAction;
    HookPlayerLogin2: TPlugHookPlayerAction;
    HookPlayerLogin3: TPlugHookPlayerAction;
    HookPlayerLogin4: TPlugHookPlayerAction;
    HookPlayerRun: TPlugHookPlayerAction;
    HookPlayerViewRangeNewObject: TPlugHookPlayerViewRangeNewObject;
    HookPlayerProcessMsgBegin: TPlugHookPlayerProcessMsg;
    HookPlayerProcessMsgEnd: TPlugHookPlayerProcessMsg;
    HookPlayerFree: TPlugHookPlayerAction;
    HookDummyObjectRunBegin: TPlugHookDummyObjectRunBegin;
    HookDummyObjectRunEnd: TPlugHookDummyObjectAction;
    HookHeroObjectCreate: TPlugHookHeroObjectAction;
    HookHeroObjectFree: TPlugHookHeroObjectAction;
  public
    constructor Create(AOwner: TPluginManager);
    destructor Destroy; override;
    procedure AddMenu(MenuItem: TMenuItem; NotifyEventMethod: PNotifyEventMethod);
    property FileName: string read FFileName;
    property PlugDesc: string read FPlugDesc;
    function GetRegisterMenus: string;
    property IsSysDef: Boolean read FIsMemLoad;
  end;

  TPluginManager = class(TObject)
  private
    FLoadPlugList: TList; // 载入了哪些插件
    FPlugBaseCode: Integer;
    FHookGetIPLocalPlug: TPlugin;
    FHookLoadScriptFilePlug: TPlugin;
    FHookCommonFilePlug: TPlugin;
    FHookDecryptScriptLinePlug: TPlugin;
    FHookDecryptScriptFilePlug: TPlugin;
    function AddPlugin: TPlugin;
    procedure SetPluginInitOK(Plugin: TPlugin);
    procedure InitInnerPluginParams(pAF: PAppFuncDef; nFlag: Integer);
    function DoInnerPluginInit(Plugin: TPlugin; pfDoInit: TPlugInit; nFlag: Integer; out sErrMsg: string): Boolean;
    function PrepareInnerPlugin(Plugin: TPlugin; const sResName: string; const sPlugName: string; nResCrc: Cardinal): TPlugInit;
    procedure LoadSysInternalPlugin;
    function GetCount: Integer;
    function GetItems(Index: Integer): TPlugin;
  private
    FHookConditionCmdList: TStringList;
    FHookActionCmdList: TStringList;
  public
    PlugList: TStringList; // 配置了哪些插件
    constructor Create;
    destructor Destroy; override;
    procedure LoadPluginList;
    function LoadPlugin(Index: Integer): TPlugin;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: TPlugin read GetItems;
    procedure Clear;
    procedure Remove(Plugin: TPlugin);
    function GetHookConditionCmdList(Plugin: TPlugin): string;
    function GetHookActionCmdList(Plugin: TPlugin): string;
  public
    function IsCheckGetIPLocalHook: Boolean;
    function HookGetIPLocal(sIPaddr: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): Boolean;
    procedure HookEngineReadyToStart();
    procedure HookEngineStartComplete();
    procedure HookEngineReloadComplete(ReloadType: Integer);
    // function HookAddGameDataLog(sMsg: PAnsiChar; sMsgLen: Integer): Boolean;
    function IsCheckLoadScriptFileHook: Boolean;
    // 数据需要使用ZIP压缩然后使用M2Server_EncryBuffer加密后才能写入Stream
    function HookLoadScriptFile(FileName: PAnsiChar; MemStream: TMemoryStream): Boolean;
    function IsCheckCommonFileHook: Boolean;
    function HookCloneCommonFile(FileName: PAnsiChar; MemStream: TMemoryStream): Boolean;
    function HookChangeCommonFile(FileName: PAnsiChar; btMode: Byte; ExtString: PAnsiChar): Boolean;
    function IsCheckDecryptScriptFileHook: Boolean;
    function HookDecryptScriptFile(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): Boolean;
    function IsCheckDecryptScriptLineHook: Boolean;
    function HookDecryptScriptLine(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): Boolean;
    // Hook NPC检测命令
    function HookNpcLoadConditionCmd(pCmd: PAnsiChar): Integer;
    function HookNpcConditionProcess(NPC: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject;
      ConditionInfo: pTQuestConditionInfo): Boolean;
    // Hook NPC执行命令
    function HookNpcLoadActionCmd(pCmd: PAnsiChar): Integer;
    function HookNpcActionProcess(NPC: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject;
      QuestActionInfo: pTQuestActionInfo; var boSendNpcSay, boBreak: BOOL): Boolean;
    // NPC命令点击
    procedure HookUserSelect(Merchant: TMerchant; PlayObject: TPlayObject; pLabel, pMsg: PAnsiChar);
    // 聊天框命令
    function HookUserCommand(PlayObject: TPlayObject; pCmd, pParam1, pParam2, pParam3, pParam4, pParam5, pParam6,
      pParam7: PAnsiChar): Boolean;
    function HookGetVariableText(NPC: TNormNpc; PlayObject: TPlayObject; sVariable: PAnsiChar; sValue: PAnsiChar;
      var nValueLen: DWORD): Boolean;
    procedure HookBaseObjectCreate(BaseObject: TBaseObject);
    procedure HookBaseObjectRecalAbilBegin(BaseObject: TBaseObject);
    procedure HookBaseObjectRecalAbilEnd(BaseObject: TBaseObject);
    procedure HookBaseObjectRun(BaseObject: TBaseObject);
    // 2021-01-05 changed
    procedure HookBaseObjectProcessMsg(BaseObject: TBaseObject; wIdent: Word; wParam: Integer;
      nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar);
    procedure HookBaseObjectStruck(BaseObject: TBaseObject; AttackObject: TBaseObject);
    procedure HookBaseObjectMagicStruck(BaseObject: TBaseObject; AttackObject: TBaseObject; MagicIdx: Integer);
    procedure HookBaseObjectAttack(BaseObject: TBaseObject; Target: TBaseObject; MagicIdx: Integer; var nPower: Integer);
    procedure HookBaseObjectMagicAttack(BaseObject: TBaseObject; Target: TBaseObject; MagicIdx: Integer; var nPower: Integer);
    procedure HookBaseObjectDie(BaseObject: TBaseObject);
    procedure HookBaseObjectMakeGhost(BaseObject: TBaseObject);
    procedure HookBaseObjectFree(BaseObject: TBaseObject);
    procedure HookPlayerCreate(PlayObject: TPlayObject);
    procedure HookPlayerLogin1(PlayObject: TPlayObject);
    procedure HookPlayerLogin2(PlayObject: TPlayObject);
    procedure HookPlayerLogin3(PlayObject: TPlayObject);
    procedure HookPlayerLogin4(PlayObject: TPlayObject);
    procedure HookPlayerRun(PlayObject: TPlayObject);
    procedure HookPlayerViewRangeNewObject(PlayObject: TPlayObject; AObject: TObject; AObjectX, AObjectY: Integer);
    // 2021-01-05 changed
    procedure HookPlayerProcessMsgBegin(PlayObject: TPlayObject; wIdent: Word; wParam: Integer;
      nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar; var boReturn: BOOL);
    // 2021-01-05 changed
    procedure HookPlayerProcessMsgEnd(PlayObject: TPlayObject; wIdent: Word; wParam: Integer;
      nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar; var boReturn: BOOL);
    procedure HookPlayerFree(PlayObject: TPlayObject);
    procedure HookDummyObjectRunBegin(DummyObject: TDummyObject; var boReturn: BOOL);
    procedure HookDummyObjectRunEnd(DummyObject: TDummyObject);
    procedure HookHeroObjectCreate(Hero: THeroObject);
    procedure HookHeroObjectFree(Hero: THeroObject);
  end;

implementation

uses
  CheckUnit, M2Share;

const
  Max_NpcCommandHook_Count = 500;

  { TMenuItemEx }
constructor TMenuItemEx.Create;
begin
  FList := TList.Create;
  FMenuItem := nil;
  FNotifyEventMethod := nil;
end;

destructor TMenuItemEx.Destroy;
begin
  Clear;
  if FNotifyEventMethod <> nil then
  begin
    Dispose(FNotifyEventMethod);
    FNotifyEventMethod := nil;
  end;
  FList.Free;
  inherited;
end;

procedure TMenuItemEx.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    TMenuItemEx(FList.Items[I]).Free;
  end;
  FList.Clear;
  FMenuItem := nil;
  if FNotifyEventMethod <> nil then
  begin
    Dispose(FNotifyEventMethod);
    FNotifyEventMethod := nil;
  end;
end;

procedure TMenuItemEx.Add(Item: TMenuItem; NotifyEventMethod: PNotifyEventMethod);
var
  MenuItemEx: TMenuItemEx;
begin
  MenuItemEx := TMenuItemEx.Create;
  MenuItemEx.FMenuItem := Item;
  MenuItemEx.FNotifyEventMethod := NotifyEventMethod;
  FList.Add(MenuItemEx);
end;

function TMenuItemEx.GetItems(Index: Integer): TMenuItemEx;
begin
  Result := FList.Items[Index];
end;

function TMenuItemEx.GetCount: Integer;
begin
  Result := FList.Count
end;

function TMenuItemEx.IndexOf(MenuItem: TMenuItem): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to FList.Count - 1 do
  begin
    if TMenuItemEx(FList.Items[I]).FMenuItem = MenuItem then
    begin
      Result := I;
      Break;
    end;
  end;
end;

{ TPlugin }
constructor TPlugin.Create(AOwner: TPluginManager);
begin
  FOwner := AOwner;
  FModule := 0;
  FBaseCode := 0;
  FIsInitOK := False;
  FMenuItems := TMenuItemEx.Create();
  HookEngineReadyToStart := nil;
  HookEngineStartComplete := nil;
  HookEngineReloadComplete := nil;
  HookLoadScriptFile := nil;
  HookDecryptScriptFile := nil;
  HookDecryptScriptLine := nil;
  HookNpcLoadConditionCmd := nil;
  HookNpcConditionProcess := nil;
  HookNpcLoadActionCmd := nil;
  HookNpcActionProcess := nil;
  HookUserSelect := nil;
  HookUserCommand := nil;
  HookGetVariableText := nil;
  HookBaseObjectCreate := nil;
  HookBaseObjectRecalAbilBegin := nil;
  HookBaseObjectRecalAbilEnd := nil;
  HookBaseObjectRun := nil;
  HookBaseObjectProcessMsg := nil;
  HookBaseObjectStruck := nil;
  HookBaseObjectMagicStruck := nil;
  HookBaseObjectAttack := nil;
  HookBaseObjectMagicAttack := nil;
  HookBaseObjectDie := nil;
  HookBaseObjectMakeGhost := nil;
  HookBaseObjectFree := nil;
  HookPlayerCreate := nil;
  HookPlayerLogin1 := nil;
  HookPlayerLogin2 := nil;
  HookPlayerLogin3 := nil;
  HookPlayerLogin4 := nil;
  HookPlayerRun := nil;
  HookPlayerViewRangeNewObject := nil;
  HookPlayerProcessMsgBegin := nil;
  HookPlayerProcessMsgEnd := nil;
  HookPlayerFree := nil;
  HookDummyObjectRunBegin := nil;
  HookDummyObjectRunEnd := nil;
  HookHeroObjectCreate := nil;
  HookHeroObjectFree := nil;
end;

destructor TPlugin.Destroy;
begin
  if FOwner.FHookGetIPLocalPlug = Self then
  begin
    FOwner.FHookGetIPLocalPlug := nil;
  end;
  if FOwner.FHookLoadScriptFilePlug = Self then
  begin
    FOwner.FHookLoadScriptFilePlug := nil;
  end;
  if FOwner.FHookCommonFilePlug = Self then
  begin
    FOwner.FHookCommonFilePlug := nil;
  end;
  if FOwner.FHookDecryptScriptFilePlug = Self then
  begin
    FOwner.FHookDecryptScriptFilePlug := nil;
  end;
  if FOwner.FHookDecryptScriptLinePlug = Self then
  begin
    FOwner.FHookDecryptScriptLinePlug := nil;
  end;
  UnLoad;
  FMenuItems.Free;
  inherited;
end;

procedure TPlugin.UnLoad;
var
  I: Integer;
  UnInit: TPlugUnInit;
  MenuItem, ParentItem: TMenuItem;
begin
  if FModule = 0 then
    Exit;
  if FIsInitOK then
  begin
    if FOwner.FHookGetIPLocalPlug = Self then
    begin
      FOwner.FHookGetIPLocalPlug := nil;
    end;
    if FOwner.FHookLoadScriptFilePlug = Self then
    begin
      FOwner.FHookLoadScriptFilePlug := nil;
    end;
    if FOwner.FHookCommonFilePlug = Self then
    begin
      FOwner.FHookCommonFilePlug := nil;
    end;
    if FOwner.FHookDecryptScriptFilePlug = Self then
    begin
      FOwner.FHookDecryptScriptFilePlug := nil;
    end;
    if FOwner.FHookDecryptScriptLinePlug = Self then
    begin
      FOwner.FHookDecryptScriptLinePlug := nil;
    end;
    for I := FOwner.FHookConditionCmdList.Count - 1 downto 0 do
    begin
      if FOwner.FHookConditionCmdList.Objects[I] = Self then
      begin
        FOwner.FHookConditionCmdList.Delete(I);
      end;
    end;
    for I := FOwner.FHookActionCmdList.Count - 1 downto 0 do
    begin
      if FOwner.FHookActionCmdList.Objects[I] = Self then
      begin
        FOwner.FHookActionCmdList.Delete(I);
      end;
    end;
  end;
  if not FIsMemLoad then
  begin
    if FIsInitOK then
    begin
      UnInit := GetProcAddress(FModule, 'UnInit');
      if Assigned(UnInit) then
        UnInit;
    end;
    FreeLibrary(FModule);
    FModule := 0;
  end
  else
  begin
    if FIsInitOK then
    begin
      // UnInit := MemoryGetProcAddress(PMemoryModule(FModule), 'UnInit');
      UnInit := MemoryModuleEx.MemoryGetProcAddress(FModule, 'UnInit');
      if Assigned(UnInit) then
        UnInit;
    end;
    // MemoryFreeLibrary(PMemoryModule(FModule));
    MemoryFreeLibrary(FModule);
    FModule := 0;
  end;
  // 卸载插件时，卸载该插件的菜单
  if FIsInitOK then
  begin
    for I := 0 to FMenuItems.Count - 1 do
    begin
      MenuItem := FMenuItems.Items[I].FMenuItem;
      if MenuItem <> nil then
      begin
        ParentItem := MenuItem.Parent;
        if ParentItem <> nil then
        begin
          ParentItem.Remove(MenuItem);
        end;
        MenuItem.Free;
      end;
    end;
    FMenuItems.Clear;
  end;
end;

procedure SearchParentMenuItem(MainMenuItem: TMenuItemEx; SearchItem: TMenuItem; var SerarchRet: TMenuItemEx);
var
  Index: Integer;
begin
  if SerarchRet <> nil then
    Exit;
  Index := MainMenuItem.IndexOf(SearchItem);
  if Index >= 0 then
  begin
    SerarchRet := MainMenuItem.Items[Index];
    Exit;
  end
  else if MainMenuItem.Count > 0 then
  begin
    for Index := 0 to MainMenuItem.Count - 1 do
    begin
      SearchParentMenuItem(MainMenuItem.Items[Index], SearchItem, SerarchRet);
      if SerarchRet <> nil then
        Exit;
    end;
  end;
end;

procedure TPlugin.AddMenu(MenuItem: TMenuItem; NotifyEventMethod: PNotifyEventMethod);
var
  SerarchRet: TMenuItemEx;
begin
  SerarchRet := nil;
  SearchParentMenuItem(FMenuItems, MenuItem.Parent, SerarchRet);
  if SerarchRet = nil then
  begin
    FMenuItems.Add(MenuItem, NotifyEventMethod);
  end
  else
  begin
    SerarchRet.Add(MenuItem, NotifyEventMethod);
  end;
end;

function TPlugin.GetRegisterMenus: string;
  procedure GetMenuText(MenuItem: TMenuItemEx; Space: string; var Ret: string);
  var
    I: Integer;
  begin
    if MenuItem.FMenuItem <> nil then
    begin
      Ret := Ret + sLineBreak + Space + MenuItem.FMenuItem.Caption;
    end;
    for I := 0 to MenuItem.Count - 1 do
    begin
      GetMenuText(MenuItem.Items[I], Space + #9, Ret);
    end;
  end;

begin
  GetMenuText(FMenuItems, '', Result);
end;

{ TPluginManager }
constructor TPluginManager.Create;
begin
  PlugList := TStringList.Create;
  FLoadPlugList := TList.Create;
  FHookConditionCmdList := TStringList.Create;
  FHookActionCmdList := TStringList.Create;
  FHookConditionCmdList.CaseSensitive := False;
  FHookActionCmdList.CaseSensitive := False;
  FHookConditionCmdList.Sort;
  FHookActionCmdList.Sort;
  FPlugBaseCode := 0;
  FHookGetIPLocalPlug := nil;
  FHookLoadScriptFilePlug := nil;
  FHookCommonFilePlug := nil;
  FHookDecryptScriptFilePlug := nil;
  FHookDecryptScriptLinePlug := nil;
end;

destructor TPluginManager.Destroy;
begin
  PlugList.Free;
  Clear;
  FLoadPlugList.Free;
  FHookConditionCmdList.Free;
  FHookActionCmdList.Free;
  inherited;
end;

procedure TPluginManager.Clear;
var
  I: Integer;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    Plugin.Free;
  end;
  FLoadPlugList.Clear;
end;

procedure TPluginManager.Remove(Plugin: TPlugin);
begin
  FLoadPlugList.Remove(Plugin);
end;

function TPluginManager.GetCount: Integer;
begin
  Result := FLoadPlugList.Count;
end;

function TPluginManager.GetItems(Index: Integer): TPlugin;
begin
  Result := FLoadPlugList.Items[Index];
end;

function TPluginManager.AddPlugin: TPlugin;
begin
  Result := TPlugin.Create(Self);
  Result.FBaseCode := FPlugBaseCode;
  Inc(FPlugBaseCode, Max_NpcCommandHook_Count);
  FLoadPlugList.Add(Result);
end;

procedure TPluginManager.SetPluginInitOK(Plugin: TPlugin);
begin
  Plugin.FIsInitOK := True;
  if Assigned(Plugin.HookGetIPLocal) then
  begin
    FHookGetIPLocalPlug := Plugin;
  end;
  if Assigned(Plugin.HookLoadScriptFile) then
  begin
    FHookLoadScriptFilePlug := Plugin;
  end;
  if Assigned(Plugin.HookCloneCommonFile) then
  begin
    FHookCommonFilePlug := Plugin;
  end;
  if Assigned(Plugin.HookDecryptScriptFile) then
  begin
    FHookDecryptScriptFilePlug := Plugin;
  end;
  if Assigned(Plugin.HookDecryptScriptLine) then
  begin
    FHookDecryptScriptLinePlug := Plugin;
  end;
end;

function TPluginManager.IsCheckGetIPLocalHook: Boolean;
begin
  Result := Assigned(FHookGetIPLocalPlug) and Assigned(FHookGetIPLocalPlug.HookGetIPLocal);
end;

function TPluginManager.HookGetIPLocal(sIPaddr: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): Boolean;
var
  S: string;
begin
  Result := False;
  if Assigned(FHookGetIPLocalPlug) and Assigned(FHookGetIPLocalPlug.HookGetIPLocal) then
  begin
    try
      Result := FHookGetIPLocalPlug.HookGetIPLocal(sIPaddr, Dest, DestLen);
    except
      on E: Exception do
      begin
        S := Format('HookGetIPLocal error [%s]; %s', [FHookGetIPLocalPlug.FFileName, E.Message]);
        MainOutMessage(S);
      end;
    end;
  end;
end;

procedure TPluginManager.HookEngineReadyToStart();
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookEngineReadyToStart) then
    begin
      try
        Plugin.HookEngineReadyToStart;
      except
        on E: Exception do
        begin
          S := Format('HookEngineReadyToStart error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookEngineStartComplete();
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookEngineStartComplete) then
    begin
      try
        Plugin.HookEngineStartComplete;
      except
        on E: Exception do
        begin
          S := Format('HookEngineStartComplete error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookEngineReloadComplete(ReloadType: Integer);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookEngineReloadComplete) then
    begin
      try
        Plugin.HookEngineReloadComplete(ReloadType);
      except
        on E: Exception do
        begin
          S := Format('HookEngineReloadComplete error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

{
  function TPluginManager.HookAddGameDataLog(sMsg: PAnsiChar;
  sMsgLen: Integer): Boolean;
  begin
  end;
}
function TPluginManager.IsCheckLoadScriptFileHook: Boolean;
begin
  Result := Assigned(FHookLoadScriptFilePlug) and Assigned(FHookLoadScriptFilePlug.HookLoadScriptFile);
end;

// 数据需要使用ZIP压缩然后使用M2Server_EncryBuffer加密后才能写入Stream
function TPluginManager.HookLoadScriptFile(FileName: PAnsiChar; MemStream: TMemoryStream): Boolean;
var
  S: string;
begin
  Result := False;
  if Assigned(FHookLoadScriptFilePlug) and Assigned(FHookLoadScriptFilePlug.HookLoadScriptFile) then
  begin
    try
      Result := FHookLoadScriptFilePlug.HookLoadScriptFile(FileName, MemStream);
    except
      on E: Exception do
      begin
        S := Format('HookLoadScriptFile error [%s]; %s', [FHookLoadScriptFilePlug.FFileName, E.Message]);
        MainOutMessage(S);
      end;
    end;
  end;
end;

function TPluginManager.IsCheckCommonFileHook: Boolean;
begin
  Result := Assigned(FHookCommonFilePlug) and Assigned(FHookCommonFilePlug.HookCloneCommonFile);
end;

function TPluginManager.HookCloneCommonFile(FileName: PAnsiChar; MemStream: TMemoryStream): Boolean;
var
  S: string;
begin
  Result := False;
  if Assigned(FHookCommonFilePlug) and Assigned(FHookCommonFilePlug.HookCloneCommonFile) then
  begin
    try
      Result := FHookCommonFilePlug.HookCloneCommonFile(FileName, MemStream);
    except
      on E: Exception do
      begin
        S := Format('HookCloneCommonFile error [%s]; %s', [FHookCommonFilePlug.FFileName, E.Message]);
        MainOutMessage(S);
      end;
    end;
  end;
end;

function TPluginManager.HookChangeCommonFile(FileName: PAnsiChar; btMode: Byte; ExtString: PAnsiChar): Boolean;
var
  S: string;
begin
  Result := False;
  if Assigned(FHookCommonFilePlug) and Assigned(FHookCommonFilePlug.HookChangeCommonFile) then
  begin
    try
      Result := FHookCommonFilePlug.HookChangeCommonFile(FileName, btMode, ExtString);
    except
      on E: Exception do
      begin
        S := Format('HookChangeCommonFile error [%s]; %s', [FHookCommonFilePlug.FFileName, E.Message]);
        MainOutMessage(S);
      end;
    end;
  end;
end;

function TPluginManager.IsCheckDecryptScriptFileHook: Boolean;
begin
  Result := Assigned(FHookDecryptScriptFilePlug) and Assigned(FHookDecryptScriptFilePlug.HookDecryptScriptFile);
end;

function TPluginManager.HookDecryptScriptFile(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): Boolean;
var
  S: string;
begin
  Result := False;
  if Assigned(FHookDecryptScriptFilePlug) and Assigned(FHookDecryptScriptFilePlug.HookDecryptScriptFile) then
  begin
    try
      Result := FHookDecryptScriptFilePlug.HookDecryptScriptFile(Src, SrcLen, Dest, DestLen);
    except
      on E: Exception do
      begin
        S := Format('HookDecryptScriptFile error [%s]; %s', [FHookDecryptScriptFilePlug.FFileName, E.Message]);
        MainOutMessage(S);
      end;
    end;
  end;
end;

function TPluginManager.IsCheckDecryptScriptLineHook: Boolean;
begin
  Result := Assigned(FHookDecryptScriptLinePlug) and Assigned(FHookDecryptScriptLinePlug.HookDecryptScriptLine);
end;

function TPluginManager.HookDecryptScriptLine(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): Boolean;
var
  S: string;
begin
  Result := False;
  if Assigned(FHookDecryptScriptLinePlug) and Assigned(FHookDecryptScriptLinePlug.HookDecryptScriptLine) then
  begin
    try
      Result := FHookDecryptScriptLinePlug.HookDecryptScriptLine(Src, SrcLen, Dest, DestLen);
    except
      on E: Exception do
      begin
        S := Format('HookDecryptScriptLine error [%s]; %s', [FHookDecryptScriptLinePlug.FFileName, E.Message]);
        MainOutMessage(S);
      end;
    end;
  end;
end;

function TPluginManager.HookNpcLoadConditionCmd(pCmd: PAnsiChar): Integer;
var
  I, Ret, Index: Integer;
  S: string;
  Plugin: TPlugin;
begin
  Result := 0;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookNpcLoadConditionCmd) then
    begin
      try
        Ret := Plugin.HookNpcLoadConditionCmd(pCmd);
        if (Ret > 0) and (Ret <= Max_NpcCommandHook_Count) then
        begin
          if not FHookConditionCmdList.Find(pCmd, Index) then
          begin
            FHookConditionCmdList.InsertObject(Index, pCmd, Plugin);
          end;
          Result := 2000 + Plugin.FBaseCode + Ret;
          Exit;
        end;
      except
        on E: Exception do
        begin
          S := Format('HookNpcLoadConditionCmd error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

function TPluginManager.HookNpcConditionProcess(NPC: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject;
  ConditionInfo: pTQuestConditionInfo): Boolean;
var
  nCMDCode: Integer;
  I: Integer;
  S: string;
  Plugin: TPlugin;
  Param: TScriptCmdParam;
begin
  Result := False;
  nCMDCode := (ConditionInfo.nCMDCode - 2000);
  if nCMDCode < 0 then
    Exit;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if (nCMDCode > Plugin.FBaseCode) and (nCMDCode <= Plugin.FBaseCode + Max_NpcCommandHook_Count) then
    begin
      if Assigned(Plugin.HookNpcConditionProcess) then
      begin
        FillChar(Param, SizeOf(Param), 0);
        Param.NPC := NPC;
        Param.PlayObject := PlayObject;
        Param.BaseObject := BaseObject;
        Param.nCMDCode := nCMDCode - Plugin.FBaseCode;
        Param.sRawParam01 := PAnsiChar(AnsiString(ConditionInfo.sRawParam1));
        Param.sRawParam02 := PAnsiChar(AnsiString(ConditionInfo.sRawParam2));
        Param.sRawParam03 := PAnsiChar(AnsiString(ConditionInfo.sRawParam3));
        Param.sRawParam04 := PAnsiChar(AnsiString(ConditionInfo.sRawParam4));
        Param.sRawParam05 := PAnsiChar(AnsiString(ConditionInfo.sRawParam5));
        Param.sRawParam06 := PAnsiChar(AnsiString(ConditionInfo.sRawParam6));
        Param.sRawParam07 := PAnsiChar(AnsiString(ConditionInfo.sRawParam7));
        Param.sRawParam08 := PAnsiChar(AnsiString(ConditionInfo.sRawParam8));
        Param.sRawParam09 := PAnsiChar(AnsiString(ConditionInfo.sRawParam9));
        Param.sRawParam10 := PAnsiChar(AnsiString(ConditionInfo.sRawParam10));
        Param.sParam01 := PAnsiChar(AnsiString(ConditionInfo.sParam1));
        Param.nParam01 := ConditionInfo.nParam1;
        Param.sParam02 := PAnsiChar(AnsiString(ConditionInfo.sParam2));
        Param.nParam02 := ConditionInfo.nParam2;
        Param.sParam03 := PAnsiChar(AnsiString(ConditionInfo.sParam3));
        Param.nParam03 := ConditionInfo.nParam3;
        Param.sParam04 := PAnsiChar(AnsiString(ConditionInfo.sParam4));
        Param.nParam04 := ConditionInfo.nParam4;
        Param.sParam05 := PAnsiChar(AnsiString(ConditionInfo.sParam5));
        Param.nParam05 := ConditionInfo.nParam5;
        Param.sParam06 := PAnsiChar(AnsiString(ConditionInfo.sParam6));
        Param.nParam06 := ConditionInfo.nParam6;
        Param.sParam07 := PAnsiChar(AnsiString(ConditionInfo.sParam7));
        Param.nParam07 := ConditionInfo.nParam7;
        Param.sParam08 := PAnsiChar(AnsiString(ConditionInfo.sParam8));
        Param.nParam08 := ConditionInfo.nParam8;
        Param.sParam09 := PAnsiChar(AnsiString(ConditionInfo.sParam9));
        Param.nParam09 := ConditionInfo.nParam9;
        Param.sParam10 := PAnsiChar(AnsiString(ConditionInfo.sParam10));
        Param.nParam10 := ConditionInfo.nParam10;
        try
          Result := Plugin.HookNpcConditionProcess(@Param);
        except
          on E: Exception do
          begin
            S := Format('HookNpcConditionProcess error [%s]; %s', [Plugin.FFileName, E.Message]);
            MainOutMessage(S);
          end;
        end;
      end;
      Exit;
    end;
  end;
end;

function TPluginManager.HookNpcLoadActionCmd(pCmd: PAnsiChar): Integer;
var
  I, Ret, Index: Integer;
  S: string;
  Plugin: TPlugin;
begin
  Result := 0;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookNpcLoadActionCmd) then
    begin
      try
        Ret := Plugin.HookNpcLoadActionCmd(pCmd);
        if (Ret > 0) and (Ret <= Max_NpcCommandHook_Count) then
        begin
          if not FHookActionCmdList.Find(pCmd, Index) then
          begin
            FHookActionCmdList.InsertObject(Index, pCmd, Plugin);
          end;
          Result := 2000 + Plugin.FBaseCode + Ret;
          Exit;
        end;
      except
        on E: Exception do
        begin
          S := Format('HookNpcLoadActionCmd error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

function TPluginManager.HookNpcActionProcess(NPC: TNormNpc; BaseObject: TBaseObject; PlayObject: TPlayObject;
  QuestActionInfo: pTQuestActionInfo; var boSendNpcSay, boBreak: BOOL): Boolean;
var
  nCMDCode: Integer;
  I: Integer;
  S: string;
  Plugin: TPlugin;
  Param: TScriptCmdParam;
begin
  Result := False;
  nCMDCode := (QuestActionInfo.nCMDCode - 2000);
  if nCMDCode < 0 then
    Exit;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if (nCMDCode > Plugin.FBaseCode) and (nCMDCode <= Plugin.FBaseCode + Max_NpcCommandHook_Count) then
    begin
      if Assigned(Plugin.HookNpcActionProcess) then
      begin
        FillChar(Param, SizeOf(Param), 0);
        Param.NPC := NPC;
        Param.PlayObject := PlayObject;
        Param.BaseObject := BaseObject;
        Param.nCMDCode := nCMDCode - Plugin.FBaseCode;
        Param.sRawParam01 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam1));
        Param.sRawParam02 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam2));
        Param.sRawParam03 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam3));
        Param.sRawParam04 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam4));
        Param.sRawParam05 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam5));
        Param.sRawParam06 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam6));
        Param.sRawParam07 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam7));
        Param.sRawParam08 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam8));
        Param.sRawParam09 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam9));
        Param.sRawParam10 := PAnsiChar(AnsiString(QuestActionInfo.sRawParam10));
        Param.sParam01 := PAnsiChar(AnsiString(QuestActionInfo.sParam1));
        Param.nParam01 := QuestActionInfo.nParam1;
        Param.sParam02 := PAnsiChar(AnsiString(QuestActionInfo.sParam2));
        Param.nParam02 := QuestActionInfo.nParam2;
        Param.sParam03 := PAnsiChar(AnsiString(QuestActionInfo.sParam3));
        Param.nParam03 := QuestActionInfo.nParam3;
        Param.sParam04 := PAnsiChar(AnsiString(QuestActionInfo.sParam4));
        Param.nParam04 := QuestActionInfo.nParam4;
        Param.sParam05 := PAnsiChar(AnsiString(QuestActionInfo.sParam5));
        Param.nParam05 := QuestActionInfo.nParam5;
        Param.sParam06 := PAnsiChar(AnsiString(QuestActionInfo.sParam6));
        Param.nParam06 := QuestActionInfo.nParam6;
        Param.sParam07 := PAnsiChar(AnsiString(QuestActionInfo.sParam7));
        Param.nParam07 := QuestActionInfo.nParam7;
        Param.sParam08 := PAnsiChar(AnsiString(QuestActionInfo.sParam8));
        Param.nParam08 := QuestActionInfo.nParam8;
        Param.sParam09 := PAnsiChar(AnsiString(QuestActionInfo.sParam9));
        Param.nParam09 := QuestActionInfo.nParam9;
        Param.sParam10 := PAnsiChar(AnsiString(QuestActionInfo.sParam10));
        Param.nParam10 := QuestActionInfo.nParam10;
        try
          Result := Plugin.HookNpcActionProcess(@Param, boSendNpcSay, boBreak);
        except
          on E: Exception do
          begin
            S := Format('HookNpcActionProcess error [%s]; %s', [Plugin.FFileName, E.Message]);
            MainOutMessage(S);
          end;
        end;
      end;
      Exit;
    end;
  end;
end;

procedure TPluginManager.HookUserSelect(Merchant: TMerchant; PlayObject: TPlayObject; pLabel, pMsg: PAnsiChar);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookUserSelect) then
    begin
      try
        if Plugin.HookUserSelect(Merchant, PlayObject, pLabel, pMsg) then
        begin
          Break;
        end;
      except
        on E: Exception do
        begin
          S := Format('HookUserSelect error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

function TPluginManager.HookUserCommand(PlayObject: TPlayObject; pCmd: PAnsiChar;
  pParam1, pParam2, pParam3, pParam4, pParam5, pParam6, pParam7: PAnsiChar): Boolean;
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  Result := False;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookUserCommand) then
    begin
      try
        if Plugin.HookUserCommand(PlayObject, pCmd, pParam1, pParam2, pParam3, pParam4, pParam5, pParam6, pParam7) then
        begin
          Result := True;
          Break;
        end;
      except
        on E: Exception do
        begin
          S := Format('HookUserCommand error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

function TPluginManager.HookGetVariableText(NPC: TNormNpc; PlayObject: TPlayObject; sVariable: PAnsiChar; sValue: PAnsiChar;
  var nValueLen: DWORD): Boolean;
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  Result := False;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookGetVariableText) then
    begin
      try
        if Plugin.HookGetVariableText(NPC, PlayObject, sVariable, sValue, nValueLen) then
        begin
          Result := True;
          Break;
        end;
      except
        on E: Exception do
        begin
          S := Format('HookGetVariableText error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectCreate(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectCreate) then
    begin
      try
        Plugin.HookBaseObjectCreate(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectCreate error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectRecalAbilBegin(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectRecalAbilBegin) then
    begin
      try
        Plugin.HookBaseObjectRecalAbilBegin(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectRecalAbilBegin error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectRecalAbilEnd(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectRecalAbilEnd) then
    begin
      try
        Plugin.HookBaseObjectRecalAbilEnd(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectRecalAbilEnd error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectRun(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectRun) then
    begin
      try
        Plugin.HookBaseObjectRun(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectRun error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

// 2021-01-05 changed
procedure TPluginManager.HookBaseObjectProcessMsg(BaseObject: TBaseObject; wIdent: Word; wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
  boReturn: BOOL;
begin
  boReturn := False;
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectProcessMsg) then
    begin
      try
        Plugin.HookBaseObjectProcessMsg(BaseObject, wIdent, wParam, nParam1, nParam2, nParam3, MsgObject, dwDeliveryTime, pMsg,
          boReturn);
        if boReturn then
          Break;
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectProcessMsg error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectStruck(BaseObject, AttackObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectStruck) then
    begin
      try
        Plugin.HookBaseObjectStruck(BaseObject, AttackObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectStruck error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectMagicStruck(BaseObject, AttackObject: TBaseObject; MagicIdx: Integer);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectMagicStruck) then
    begin
      try
        Plugin.HookBaseObjectMagicStruck(BaseObject, AttackObject, MagicIdx);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectMagicStruck error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectAttack(BaseObject, Target: TBaseObject; MagicIdx: Integer; var nPower: Integer);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectAttack) then
    begin
      try
        Plugin.HookBaseObjectAttack(BaseObject, Target, MagicIdx, nPower);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectAttack error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectMagicAttack(BaseObject, Target: TBaseObject; MagicIdx: Integer; var nPower: Integer);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectMagicAttack) then
    begin
      try
        Plugin.HookBaseObjectMagicAttack(BaseObject, Target, MagicIdx, nPower);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectMagicAttack error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectDie(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectDie) then
    begin
      try
        Plugin.HookBaseObjectDie(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectDie error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectMakeGhost(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectMakeGhost) then
    begin
      try
        Plugin.HookBaseObjectMakeGhost(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectMakeGhost error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookBaseObjectFree(BaseObject: TBaseObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookBaseObjectFree) then
    begin
      try
        Plugin.HookBaseObjectFree(BaseObject);
      except
        on E: Exception do
        begin
          S := Format('HookBaseObjectFree error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerCreate(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerCreate) then
    begin
      try
        Plugin.HookPlayerCreate(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerCreate error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerLogin1(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerLogin1) then
    begin
      try
        Plugin.HookPlayerLogin1(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerLogin1 error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerLogin2(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerLogin2) then
    begin
      try
        Plugin.HookPlayerLogin2(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerLogin2 error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerLogin3(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerLogin3) then
    begin
      try
        Plugin.HookPlayerLogin3(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerLogin3 error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerLogin4(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerLogin4) then
    begin
      try
        Plugin.HookPlayerLogin4(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerLogin4 error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerRun(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerRun) then
    begin
      try
        Plugin.HookPlayerRun(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerRun error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerViewRangeNewObject(PlayObject: TPlayObject; AObject: TObject; AObjectX, AObjectY: Integer);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerViewRangeNewObject) then
    begin
      try
        Plugin.HookPlayerViewRangeNewObject(PlayObject, AObject, AObjectX, AObjectY);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerViewRangeNewObject error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

// 2021-01-05 changed
procedure TPluginManager.HookPlayerProcessMsgBegin(PlayObject: TPlayObject; wIdent: Word; wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar; var boReturn: BOOL);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerProcessMsgBegin) then
    begin
      try
        Plugin.HookPlayerProcessMsgBegin(PlayObject, wIdent, wParam, nParam1, nParam2, nParam3, MsgObject, dwDeliveryTime, pMsg,
          boReturn);
        if boReturn then
          Break;
      except
        on E: Exception do
        begin
          S := Format('HookPlayerProcessMsgBegin error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

// 2021-01-05 changed
procedure TPluginManager.HookPlayerProcessMsgEnd(PlayObject: TPlayObject; wIdent: Word; wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; MsgObject: TObject; dwDeliveryTime: LongWord; pMsg: PAnsiChar; var boReturn: BOOL);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerProcessMsgEnd) then
    begin
      try
        Plugin.HookPlayerProcessMsgEnd(PlayObject, wIdent, wParam, nParam1, nParam2, nParam3, MsgObject, dwDeliveryTime, pMsg,
          boReturn);
        if boReturn then
          Break;
      except
        on E: Exception do
        begin
          S := Format('HookPlayerProcessMsgEnd error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookPlayerFree(PlayObject: TPlayObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookPlayerFree) then
    begin
      try
        Plugin.HookPlayerFree(PlayObject);
      except
        on E: Exception do
        begin
          S := Format('HookPlayerFree error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookDummyObjectRunBegin(DummyObject: TDummyObject; var boReturn: BOOL);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookDummyObjectRunBegin) then
    begin
      try
        Plugin.HookDummyObjectRunBegin(DummyObject, boReturn);
        if boReturn then
          Break;
      except
        on E: Exception do
        begin
          S := Format('HookDummyObjectRunBegin error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookDummyObjectRunEnd(DummyObject: TDummyObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookDummyObjectRunEnd) then
    begin
      try
        Plugin.HookDummyObjectRunEnd(DummyObject);
      except
        on E: Exception do
        begin
          S := Format('HookDummyObjectRunEnd error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookHeroObjectCreate(Hero: THeroObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookHeroObjectCreate) then
    begin
      try
        Plugin.HookHeroObjectCreate(Hero);
      except
        on E: Exception do
        begin
          S := Format('HookHeroObjectCreate error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

procedure TPluginManager.HookHeroObjectFree(Hero: THeroObject);
var
  I: Integer;
  S: string;
  Plugin: TPlugin;
begin
  for I := 0 to FLoadPlugList.Count - 1 do
  begin
    Plugin := FLoadPlugList.Items[I];
    if Assigned(Plugin.HookHeroObjectFree) then
    begin
      try
        Plugin.HookHeroObjectFree(Hero);
      except
        on E: Exception do
        begin
          S := Format('HookHeroObjectFree error [%s]; %s', [Plugin.FFileName, E.Message]);
          MainOutMessage(S);
        end;
      end;
    end;
  end;
end;

function TPluginManager.GetHookConditionCmdList(Plugin: TPlugin): string;
var
  I: Integer;
  SBreak: string;
begin
  Result := '';
  SBreak := '';
  for I := 0 to FHookConditionCmdList.Count - 1 do
  begin
    if (Plugin = nil) or (FHookConditionCmdList.Objects[I] = Plugin) then
    begin
      Result := Result + SBreak + FHookConditionCmdList.Strings[I];
      SBreak := sLineBreak;
    end;
  end;
end;

function TPluginManager.GetHookActionCmdList(Plugin: TPlugin): string;
var
  I: Integer;
  SBreak: string;
begin
  Result := '';
  SBreak := '';
  for I := 0 to FHookActionCmdList.Count - 1 do
  begin
    if (Plugin = nil) or (FHookActionCmdList.Objects[I] = Plugin) then
    begin
      Result := Result + SBreak + FHookActionCmdList.Strings[I];
      SBreak := sLineBreak;
    end;
  end;
end;

function TPluginManager.PrepareInnerPlugin(Plugin: TPlugin; const sResName: string; const sPlugName: string; nResCrc: Cardinal)
  : TPlugInit;
var
  tmpCRC: Cardinal;
  RS: TResourceStream;
  MemMoudle: THandle;
begin
  Result := nil;
  RS := TResourceStream.Create(HInstance, sResName, 'dll');
  try
    RS.Seek(0, soFromBeginning);
    tmpCRC := BufferCrc(RS.Memory, RS.Size);
    if tmpCRC = nResCrc then
    begin
      MemMoudle := MemoryLoadLibrary(RS.Memory);
      if MemMoudle <> 0 then
      begin
        Result := MemoryGetProcAddress(MemMoudle, 'Init');
        if Assigned(Result) then
        begin
          Plugin.FFileName := sPlugName;
          Plugin.FIsMemLoad := True;
          Plugin.FModule := MemMoudle;
        end;
      end;
    end;
  finally
    RS.Free;
  end;
end;

procedure TPluginManager.InitInnerPluginParams(pAF: PAppFuncDef; nFlag: Integer);
begin
  ZeroMemory(pAF, SizeOf(pAF^));
  pAF.M2Engine.GetAppDir := _TM2Engine_GetAppDir;
  pAF.M2Engine.GetGlobalIniFile := _TM2Engine_GetGlobalIniFile;
  pAF.M2Engine.MainOutMessage := _TM2Engine_MainOutMessage;
  pAF.Menu.GetMainMenu := _TMenu_GetMainMenu;
  pAF.Menu.GetControlMenu := _TMenu_GetControlMenu;
  pAF.Menu.GetViewMenu := _TMenu_GetViewMenu;
  pAF.Menu.GetOptionMenu := _TMenu_GetOptionMenu;
  pAF.Menu.GetManagerMenu := _TMenu_GetManagerMenu;
  pAF.Menu.GetToolsMenu := _TMenu_GetToolsMenu;
  pAF.Menu.GetHelpMenu := _TMenu_GetHelpMenu;
  pAF.Menu.GetPluginMenu := _TMenu_GetPluginMenu;
  pAF.Menu.Add := _TMenu_Add;
  pAF.Menu.GetVisable := _TMenu_GetVisable;
  pAF.Menu.SetVisable := _TMenu_SetVisable;
  pAF.Menu.GetChecked := _TMenu_GetChecked;
  pAF.Menu.SetChecked := _TMenu_SetChecked;
  pAF.IniFile.ReadString := _TIniFile_ReadString;
  pAF.IniFile.WriteString := _TIniFile_WriteString;
  pAF.IniFile.ReadInteger := _TIniFile_ReadInteger;
  pAF.IniFile.WriteInteger := _TIniFile_WriteInteger;
  pAF.IniFile.ReadBool := _TIniFile_ReadBool;
  pAF.IniFile.WriteBool := _TIniFile_WriteBool;
  case nFlag of
    0:
      begin // IpLocal
      end;
    1:
      begin // RecvLog
        pAF.BaseObject.GetChrName := _TBaseObject_GetChrName;
      end;
    2:
      begin // RemoteScript
        pAF.M2Engine.EncodeBuffer := _TM2Engine_EncodeBuffer;
        pAF.M2Engine.DecodeBuffer := _TM2Engine_DecodeBuffer;
        pAF.MemStream.GetSize := _TMemStream_GetSize;
        pAF.MemStream.Write := _TMemStream_Write;
      end;
    3:
      begin // ScriptDecrypt
        pAF.M2Engine.EncryptPassword := _TM2Engine_EncryptPassword;
        pAF.M2Engine.DecryptPassword := _TM2Engine_DecryptPassword;
      end;
    4:
      begin
        pAF.M2Engine.EncodeBuffer := _TM2Engine_EncodeBuffer;
        pAF.M2Engine.DecodeBuffer := _TM2Engine_DecodeBuffer;
        pAF.MemStream.GetSize := _TMemStream_GetSize;
        pAF.MemStream.Write := _TMemStream_Write;
      end;
  end;
end;

function TPluginManager.DoInnerPluginInit(Plugin: TPlugin; pfDoInit: TPlugInit; nFlag: Integer; out sErrMsg: string): Boolean;
var
  af: TAppFuncDef;
  PlugDesc: array [0 .. 399] of AnsiChar;
  PlugDescLen: DWORD;
begin
  Result := False;
  InitInnerPluginParams(@af, nFlag); //
  af.PluginID := NativeInt(Plugin);
  try
    PlugDescLen := SizeOf(PlugDesc);
    FillChar(PlugDesc[0], PlugDescLen, 0);
    if pfDoInit(@af, BufferCrc(@af, SizeOf(af)), 0, @PlugDesc, PlugDescLen) then
    begin
      Plugin.FPlugDesc := PlugDesc;
      Result := True;
    end
    else
    begin
      sErrMsg := Format('插件初始化失败[%s]', [Plugin.FFileName]);
    end;
  except
    on E: Exception do
    begin
      sErrMsg := Format('插件初始化出错[%s]; %s', [Plugin.FFileName, E.Message]);
    end;
  end;
end;

procedure TPluginManager.LoadSysInternalPlugin;
var
  PlugDesc: array [0 .. 399] of AnsiChar;
  PlugDescLen: DWORD;
  RunCode: Integer;
  SL: TStringList;
  FileName, sErrMsg: string;
  IniFile: TIniFile;
  IsIPLocal, IsRemoteScript, IsScriptDecrypt, IsRecvLog, IsCommonScript: Boolean;
  DoInit: TPlugInit;
  Plugin: TPlugin;
  nCrc: Cardinal;
begin
  FileName := ExtractFilePath(ParamStr(0)) + '系统插件.ini';
  if not FileExists(FileName) then
  begin
    SL := TStringList.Create;
    try
      SL.Add(';需要启用插件的参数设置为1，关闭则为0');
      SL.Add('[setup]');
      SL.Add('IP插件=1');
      SL.Add('脚本解密=0');
      SL.Add('远程脚本=0');
      SL.Add('封包记录=0');
      // SL.Add('通区脚本=1');
      SL.SaveToFile(FileName);
    finally
      SL.Free;
    end;
  end;

  // VMProtectBeginUltra('VMProtect_LaodSysInternalPlugin');
  IniFile := TIniFile.Create(FileName);
  try
    IsIPLocal := IniFile.ReadBool('setup', 'IP插件', False);
    IsScriptDecrypt := IniFile.ReadBool('setup', '脚本解密', False);
    IsRemoteScript := IniFile.ReadBool('setup', '远程脚本', False);
    IsCommonScript := IniFile.ReadBool('setup', '通区脚本', False);
    IsRecvLog := IniFile.ReadBool('setup', '封包记录', False);
  finally
    IniFile.Free;
  end;
  {
    IsIPLocal := True;
    IsScriptDecrypt := True;
    IsRemoteScript := True;
    IsCommonScript := True;
    IsRecvLog := True;
  }

  if IsIPLocal then
  begin
{$IFDEF CPUX64}
    nCrc := 1604834858;
{$ELSE}
    nCrc := 320329455;
{$ENDIF}
    Plugin := AddPlugin;
    DoInit := PrepareInnerPlugin(Plugin, 'IPLocal', 'IP插件', nCrc);
    if Assigned(DoInit) then
    begin
      // 载入dll接口
      { .$I PluginFuncLoad_Mem.inc }
      Plugin.HookGetIPLocal := MemoryGetProcAddress(Plugin.FModule, 'HookGetIPLocal');
      Plugin.HookPlayerProcessMsgBegin := MemoryGetProcAddress(Plugin.FModule, 'HookPlayerProcessMsgBegin');
      if DoInnerPluginInit(Plugin, DoInit, 0, sErrMsg) then
      begin
        PlugList.AddObject(Plugin.FFileName, Plugin);
        SetPluginInitOK(Plugin);
      end
      else
      begin
        FLoadPlugList.Remove(Plugin);
        Plugin.Free;
        MainOutMessage(sErrMsg, False);
      end;
    end
    else
    begin
      FLoadPlugList.Remove(Plugin);
      Plugin.Free;
    end;
  end;

  if IsRecvLog then
  begin
{$IFDEF CPUX64}
    nCrc := 1398005562;
{$ELSE}
    nCrc := 1942413482;
{$ENDIF}
    Plugin := AddPlugin;
    DoInit := PrepareInnerPlugin(Plugin, 'RecvLog', '日志记录插件', nCrc);
    if Assigned(DoInit) then
    begin
      // 载入dll接口
      Plugin.HookPlayerProcessMsgBegin := MemoryGetProcAddress(Plugin.FModule, 'HookPlayerProcessMsgBegin');
      if DoInnerPluginInit(Plugin, DoInit, 1, sErrMsg) then
      begin
        PlugList.AddObject(Plugin.FFileName, Plugin);
        SetPluginInitOK(Plugin);
      end
      else
      begin
        FLoadPlugList.Remove(Plugin);
        Plugin.Free;
        MainOutMessage(sErrMsg, False);
      end;
    end
    else
    begin
      FLoadPlugList.Remove(Plugin);
      Plugin.Free;
    end;
  end;

  if IsRemoteScript then
  begin
{$IFDEF CPUX64}
    nCrc := 2373426267;
{$ELSE}
    nCrc := 863344940;
{$ENDIF}
    Plugin := AddPlugin;
    DoInit := PrepareInnerPlugin(Plugin, 'RemoteScript', '远程脚本插件', nCrc);
    if Assigned(DoInit) then
    begin
      // 载入dll接口
      Plugin.HookEngineStartComplete := MemoryGetProcAddress(Plugin.FModule, 'HookEngineStartComplete');
      Plugin.HookEngineReloadComplete := MemoryGetProcAddress(Plugin.FModule, 'HookEngineReloadComplete');
      Plugin.HookLoadScriptFile := MemoryGetProcAddress(Plugin.FModule, 'HookLoadScriptFile');
      if DoInnerPluginInit(Plugin, DoInit, 2, sErrMsg) then
      begin
        PlugList.AddObject(Plugin.FFileName, Plugin);
        SetPluginInitOK(Plugin);
      end
      else
      begin
        FLoadPlugList.Remove(Plugin);
        Plugin.Free;
        MainOutMessage(sErrMsg, False);
      end;
    end
    else
    begin
      FLoadPlugList.Remove(Plugin);
      Plugin.Free;
    end;
  end;

  if IsScriptDecrypt then
  begin
{$IFDEF CPUX64}
    nCrc := 3668527439;
{$ELSE}
    nCrc := 3056013488;
{$ENDIF}
    Plugin := AddPlugin;
    DoInit := PrepareInnerPlugin(Plugin, 'ScriptDecrypt', '脚本解密插件', nCrc);
    if Assigned(DoInit) then
    begin
      // 载入dll接口
      Plugin.HookEngineReadyToStart := MemoryGetProcAddress(Plugin.FModule, 'HookEngineReadyToStart');
      Plugin.HookDecryptScriptLine := MemoryGetProcAddress(Plugin.FModule, 'HookDecryptScriptLine');
      if DoInnerPluginInit(Plugin, DoInit, 3, sErrMsg) then
      begin
        PlugList.AddObject(Plugin.FFileName, Plugin);
        SetPluginInitOK(Plugin);
      end
      else
      begin
        FLoadPlugList.Remove(Plugin);
        Plugin.Free;
        MainOutMessage(sErrMsg, False);
      end;
    end
    else
    begin
      FLoadPlugList.Remove(Plugin);
      Plugin.Free;
    end;
  end;

  if IsCommonScript then
  begin
{$IFDEF CPUX64}
    nCrc := 2776185858;
{$ELSE}
    nCrc := 1269032756;
{$ENDIF}
    Plugin := AddPlugin;
    DoInit := PrepareInnerPlugin(Plugin, 'CommonScript', '通区脚本插件', nCrc);
    if Assigned(DoInit) then
    begin
      // 载入dll接口
      Plugin.HookEngineStartComplete := MemoryGetProcAddress(Plugin.FModule, 'HookEngineStartComplete');
      Plugin.HookEngineReloadComplete := MemoryGetProcAddress(Plugin.FModule, 'HookEngineReloadComplete');
      Plugin.HookCloneCommonFile := MemoryGetProcAddress(Plugin.FModule, 'HookCloneCommonFile');
      Plugin.HookChangeCommonFile := MemoryGetProcAddress(Plugin.FModule, 'HookChangeCommonFile');
      if DoInnerPluginInit(Plugin, DoInit, 4, sErrMsg) then
      begin
        PlugList.AddObject(Plugin.FFileName, Plugin);
        SetPluginInitOK(Plugin);
      end
      else
      begin
        FLoadPlugList.Remove(Plugin);
        Plugin.Free;
        MainOutMessage(sErrMsg, False);
      end;
    end
    else
    begin
      FLoadPlugList.Remove(Plugin);
      Plugin.Free;
    end;
  end;

  // if IsCommonScript then
  // begin
  // RS := TResourceStream.Create(HInstance, 'CommonScript', PChar('dll'));
  // try
  // RS.Seek(0, soFromBeginning);
  // nCrc := BufferCrc(RS.Memory, RS.Size);
  // MainOutMessage(IntToStr(nCrc));
  // {$IFDEF CPUX64}
  // if nCrc = 1692115693 then
  // {$ELSE}
  // if nCrc = 3998378797 then
  // {$ENDIF}
  // begin
  // MemMoudle := MemoryLoadLibary(RS.Memory, RS.Size, RunCode);
  //
  // if Assigned(MemMoudle) then
  // begin
  // DoInit := MemoryGetProcAddress(MemMoudle, 'Init');
  //
  // if Assigned(DoInit) then
  // begin
  // Plugin := AddPlugin;
  //
  // Plugin.FFileName := '通区脚本';
  // Plugin.FIsMemLoad := True;
  // Plugin.FModule := Cardinal(MemMoudle);
  //
  // // 结构体清0
  // FillChar(MF, SizeOf(MF), 0);
  // MF.PluginID := NativeInt(Plugin);
  //
  // // 载入dll接口
  // {.$I PluginFuncLoad_Mem.inc}
  //
  // // 只用了这几个，就只搞这几个，为了安全
  // Plugin.HookEngineStartComplete := MemoryGetProcAddress(MemMoudle, 'HookEngineStartComplete');
  // Plugin.HookEngineReloadComplete := MemoryGetProcAddress(MemMoudle, 'HookEngineReloadComplete');
  // Plugin.HookCloneCommonFile := MemoryGetProcAddress(MemMoudle, 'HookCloneCommonFile');
  // Plugin.HookChangeCommonFile := MemoryGetProcAddress(MemMoudle, 'HookChangeCommonFile');
  //
  // MF.M2Engine.GetGlobalIniFile := _TM2Engine_GetGlobalIniFile;
  // MF.M2Engine.MainOutMessage := _TM2Engine_MainOutMessage;
  // MF.M2Engine.EncodeBuffer := _TM2Engine_EncodeBuffer;
  // MF.M2Engine.DecodeBuffer := _TM2Engine_DecodeBuffer;
  //
  // MF.Menu.GetMainMenu := _TMenu_GetMainMenu;
  // MF.Menu.GetControlMenu := _TMenu_GetControlMenu;
  // MF.Menu.GetViewMenu := _TMenu_GetViewMenu;
  // MF.Menu.GetOptionMenu := _TMenu_GetOptionMenu;
  // MF.Menu.GetManagerMenu := _TMenu_GetManagerMenu;
  // MF.Menu.GetToolsMenu := _TMenu_GetToolsMenu;
  // MF.Menu.GetHelpMenu := _TMenu_GetHelpMenu;
  // MF.Menu.GetPluginMenu := _TMenu_GetPluginMenu;
  // MF.Menu.Add := _TMenu_Add;
  // MF.Menu.GetVisable := _TMenu_GetVisable;
  // MF.Menu.SetVisable := _TMenu_SetVisable;
  // MF.Menu.GetChecked := _TMenu_GetChecked;
  // MF.Menu.SetChecked := _TMenu_SetChecked;
  //
  // MF.MemStream.GetSize := _TMemStream_GetSize;
  // MF.MemStream.Write := _TMemStream_Write;
  //
  // MF.IniFile.ReadString := _TIniFile_ReadString;
  // MF.IniFile.WriteString := _TIniFile_WriteString;
  // MF.IniFile.ReadInteger := _TIniFile_ReadInteger;
  // MF.IniFile.WriteInteger := _TIniFile_WriteInteger;
  // MF.IniFile.ReadBool := _TIniFile_ReadBool;
  // MF.IniFile.WriteBool := _TIniFile_WriteBool;
  // // ----------------------------------------------------
  //
  // try
  // PlugDescLen := SizeOf(PlugDesc);
  // FillChar(PlugDesc[0], PlugDescLen, 0);
  //
  // if DoInit(@MF, BufferCrc(@MF, SizeOf(MF)), 0, @PlugDesc, PlugDescLen) then
  // begin
  // Plugin.FPlugDesc := PlugDesc;
  //
  // PlugList.AddObject(Plugin.FFileName, Plugin);
  //
  // SetPluginInitOK(Plugin);
  // end
  // else
  // begin
  // MainOutMessage(Format('插件初始化失败[%s]', [Plugin.FFileName]), False);
  //
  // FLoadPlugList.Remove(Plugin);
  // Plugin.Free;
  // end;
  // except
  // MainOutMessage(Format('插件初始化出错[%s]', [Plugin.FFileName]), False);
  //
  // FLoadPlugList.Remove(Plugin);
  // Plugin.Free;
  // end;
  // end;
  // end;
  // end;
  // finally
  // RS.Free;
  // end;
  // end;
  // VMProtectEnd();
end;

procedure TPluginManager.LoadPluginList;
var
  I: Integer;
  sFileName, sLine: string;
  SL: TStringList;
  Moudle: THandle;
  DoInit: TPlugInit;
  Plugin: TPlugin;
  MF: TAppFuncDef;
  PlugDesc: array [0 .. 399] of AnsiChar;
  PlugDescLen: DWORD;
begin
  LoadSysInternalPlugin;
  sFileName := g_Config.sPlugDir + 'PlugList.txt';
  if not DirectoryExists(g_Config.sPlugDir) then
    CreateDir(g_Config.sPlugDir);

  if FileExists(sFileName) then
  begin
    SL := TStringList.Create;
    SL.LoadFromFile(sFileName);

    for I := 0 to SL.Count - 1 do
    begin
      sLine := Trim(SL.Strings[I]);
      if (sLine = '') or (sLine[1] = ';') then
        Continue;

      sFileName := g_Config.sPlugDir + sLine; // 复用了sFileName
      PlugList.Add(sLine);

      if FileExists(sFileName) then
      begin
        Moudle := LoadLibrary(PChar(sFileName)); // FreeLibrary
        if Moudle > 32 then
        begin
          DoInit := GetProcAddress(Moudle, 'Init');

          if Assigned(DoInit) then
          begin
            Plugin := AddPlugin;
            Plugin.FFileName := sLine;
            Plugin.FIsMemLoad := False;
            Plugin.FModule := Moudle;
            // 结构体清0
            FillChar(MF, SizeOf(MF), 0);
            MF.PluginID := NativeInt(Plugin);
            // 载入dll接口
{$I PluginFuncLoad_Dll.inc}
            // 接口各种函数赋值，在这里弄的
{$I PluginFuncAssign.inc}
            try
{$IFDEF DEBUG}
              OutputDebugString('--------------------------------------------------------------------------------');
              OutputDebugString(PChar('插件ID为:' + IntToStr(MF.PluginID)));
              OutputDebugString(PChar('Memory.Allow:' + IntToStr(NativeInt(@MF.Memory.Allow))));
              OutputDebugString(PChar('Memory.Free:' + IntToStr(NativeInt(@MF.Memory.Free))));
              OutputDebugString(PChar('Memory.Realloc:' + IntToStr(NativeInt(@MF.Memory.Realloc))));
              OutputDebugString(PChar('List.Create:' + IntToStr(NativeInt(@MF.List.Create))));
              OutputDebugString(PChar('StringList.Create:' + IntToStr(NativeInt(@MF.StringList.Create))));
              OutputDebugString(PChar('MemStream.Create:' + IntToStr(NativeInt(@MF.MemStream.Create))));
              OutputDebugString(PChar('Menu.GetMainMenu:' + IntToStr(NativeInt(@MF.Menu.GetMainMenu))));
              OutputDebugString(PChar('IniFile.Create:' + IntToStr(NativeInt(@MF.IniFile.Create))));
              OutputDebugString(PChar('MagicACList.Count:' + IntToStr(NativeInt(@MF.MagicACList.Count))));
              OutputDebugString(PChar('MapManager.FindMap:' + IntToStr(NativeInt(@MF.MapManager.FindMap))));
              OutputDebugString(PChar('Envir.GetMapName' + IntToStr(NativeInt(@MF.Envir.GetMapName))));
              OutputDebugString(PChar('M2Engine.GetVersion:' + IntToStr(NativeInt(@MF.M2Engine.GetVersion))));
              OutputDebugString(PChar('Guild.SendGuildMsg:' + IntToStr(NativeInt(@MF.Guild.SendGuildMsg))));
              // OutputDebugString(PChar(':' + IntToStr(NativeInt(@))));
              OutputDebugString('--------------------------------------------------------------------------------');
{$ENDIF}
              // 这里是值传递，结构体
              PlugDescLen := SizeOf(PlugDesc);
              FillChar(PlugDesc[0], PlugDescLen, 0);

              if DoInit(@MF, BufferCrc(@MF, SizeOf(MF)), 0, @PlugDesc, PlugDescLen) then
              begin
                Plugin.FPlugDesc := PlugDesc;
                PlugList.Objects[PlugList.Count - 1] := Plugin;
                SetPluginInitOK(Plugin);
              end
              else
              begin
                MainOutMessage(Format('插件初始化失败[%s]', [Plugin.FFileName]), False);
                FLoadPlugList.Remove(Plugin);
                Plugin.Free;
              end;
            except
              MainOutMessage(Format('插件初始化出错[%s]', [Plugin.FFileName]), False);
              FLoadPlugList.Remove(Plugin);
              Plugin.Free;
            end;
          end
          else
            FreeLibrary(Moudle);
        end;
      end;
    end;
    SL.Free;
  end;
end;

function TPluginManager.LoadPlugin(Index: Integer): TPlugin;
var
  Moudle: THandle;
  DoInit: TPlugInit;
  Plugin: TPlugin;
  MF: TAppFuncDef;
  sFileName, sPlugFile: string;
  PlugDesc: array [0 .. 399] of AnsiChar;
  PlugDescLen: DWORD;
begin
  Result := nil;
  if (Index < 0) or (Index >= PlugList.Count) then
    Exit;
  if (PlugList.Objects[Index] <> nil) then
    Exit;
  sPlugFile := PlugList.Strings[Index];
  sFileName := g_Config.sPlugDir + sPlugFile;
  if not FileExists(sFileName) then
    Exit;
  Moudle := LoadLibrary(PChar(sFileName)); // FreeLibrary
  if Moudle > 32 then
  begin
    DoInit := GetProcAddress(Moudle, 'Init');
    if not Assigned(DoInit) then
    begin
      FreeLibrary(Moudle);
    end
    else
    begin
      Plugin := AddPlugin;
      Plugin.FFileName := sPlugFile;
      Plugin.FIsMemLoad := False;
      Plugin.FModule := Moudle;
      // 结构体清0
      FillChar(MF, SizeOf(MF), 0);
      MF.PluginID := NativeInt(Plugin);
      // OutputDebugString(PWideChar(Format('PlugName = %s PluginID = %.16X', [Plugin.FFileName, MF.PluginID])));
      // 载入dll接口
{$I PluginFuncLoad_Dll.inc}
      // 接口各种函数赋值，在这里弄的
{$I PluginFuncAssign.inc}
      try
        PlugDescLen := SizeOf(PlugDesc);
        FillChar(PlugDesc[0], PlugDescLen, 0);
        if DoInit(@MF, BufferCrc(@MF, SizeOf(MF)), 0, @PlugDesc, PlugDescLen) then
        begin
          Plugin.FPlugDesc := PlugDesc;
          PlugList.Objects[Index] := Plugin;
          SetPluginInitOK(Plugin);

          Result := Plugin;
        end
        else
        begin
          MainOutMessage(Format('插件初始化失败[%s]', [Plugin.FFileName]), False);

          FLoadPlugList.Remove(Plugin);
          Plugin.Free;
        end;
      except
        MainOutMessage(Format('插件初始化出错[%s]', [Plugin.FFileName]), False);

        FLoadPlugList.Remove(Plugin);
        Plugin.Free;
      end;
    end;
  end;
end;

end.
