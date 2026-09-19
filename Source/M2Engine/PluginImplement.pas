unit PluginImplement;

interface

// ------------------------------------------------------------------------------
// M2Server插件接口实现
//
// 版 本 号: 1.1
// 发布日期: 2018-03-23
// 更新记录:
// 2018-03-23: 初次整理
// ------------------------------------------------------------------------------

uses
  Windows,
  Classes,
  SysUtils,
  Menus,
  PluginInterface,
  Envir,
  EncryptUnit_LF,
  EDcode,
  ObjBase,
  ObjPlayer,
  ObjHero,
  ObjDummy,
  ObjNpc,
  Guild,
  uMagicACUtils,
  Graphics,

{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  IniFiles,
  Grobal2,
  M2Definition,
  System.Types;

type
  PNotifyEventMethod = ^TNotifyEventMethod;

  TNotifyEventMethod = record
    Click: TNotifyEventEx;
    Sender: TObject;
  end;

  // -----------------------------------------------------------------------------
function _TMemory_Alloc(Size: Integer): Pointer; stdcall;
procedure _TMemory_Free(P: Pointer); stdcall;
procedure _TMemory_Realloc(P: Pointer; Size: Integer); stdcall;

// -----------------------------------------------------------------------------
function _TList_Create(): TList; stdcall;
procedure _TList_Free(List: TList); stdcall;
function _TList_Count(List: TList): Integer; stdcall;
procedure _TList_Clear(List: TList); stdcall;
procedure _TList_Add(List: TList; Item: Pointer); stdcall;
procedure _TList_Insert(List: TList; Index: Integer; Item: Pointer); stdcall;
procedure _TList_Remove(List: TList; Item: Pointer); stdcall;
procedure _TList_Delete(List: TList; Index: Integer); stdcall;
function _TList_GetItem(List: TList; Index: Integer): Pointer; stdcall;
procedure _TList_SetItem(List: TList; Index: Integer; Item: Pointer); stdcall;
function _TList_IndexOf(List: TList; Item: Pointer): Integer; stdcall;
procedure _TList_Exchange(List: TList; Index1, Index2: Integer); stdcall;
procedure _TList_CopyTo(Source, Dest: TList); stdcall;

// -----------------------------------------------------------------------------
function _TStrList_Create(): TStringList; stdcall;
procedure _TStrList_Free(Strings: TStringList); stdcall;
function _TStrList_GetCaseSensitive(Strings: TStringList): BOOL; stdcall;
procedure _TStrList_SetCaseSensitive(Strings: TStringList; IsCaseSensitive: BOOL); stdcall;
function _TStrList_GetSorted(Strings: TStringList): BOOL; stdcall;
procedure _TStrList_SetSorted(Strings: TStringList; Sorted: BOOL); stdcall;
function _TStrList_GetDuplicates(Strings: TStringList): BOOL; stdcall;
procedure _TStrList_SetDuplicates(Strings: TStringList; Duplicates: BOOL); stdcall;
function _TStrList_Count(Strings: TStringList): Integer; stdcall;
function _TStrList_GetText(Strings: _TStringList; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TStrList_SetText(Strings: _TStringList; Src: PAnsiChar; SrcLen: DWORD); stdcall;
procedure _TStrList_Add(Strings: TStringList; S: PAnsiChar); stdcall;
procedure _TStrList_AddObject(Strings: TStringList; S: PAnsiChar; AObject: TObject); stdcall;
procedure _TStrList_Insert(Strings: TStringList; Index: Integer; S: PAnsiChar); stdcall;
procedure _TStrList_InsertObject(Strings: TStringList; Index: Integer; S: PAnsiChar; AObject: TObject); stdcall;
procedure _TStrList_Remove(Strings: TStringList; S: PAnsiChar); stdcall;
procedure _TStrList_Delete(Strings: TStringList; Index: Integer); stdcall;
function _TStrList_GetItem(Strings: TStringList; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TStrList_SetItem(Strings: TStringList; Index: Integer; S: PAnsiChar); stdcall;
function _TStrList_GetObject(Strings: TStringList; Index: Integer): TObject; stdcall;
procedure _TStrList_SetObject(Strings: TStringList; Index: Integer; AObject: TObject); stdcall;
function _TStrList_IndexOf(Strings: TStringList; S: PAnsiChar): Integer; stdcall;
function _TStrList_IndexOfObject(Strings: TStringList; AObject: TObject): Integer; stdcall;
function _TStrList_Find(Strings: TStringList; S: PAnsiChar; var Index: Integer): BOOL; stdcall;
procedure _TStrList_Exchange(Strings: TStringList; Index1, Index2: Integer); stdcall;
procedure _TStrList_LoadFromFile(Strings: _TStringList; FileName: PAnsiChar); stdcall;
procedure _TStrList_SaveToFile(Strings: _TStringList; FileName: PAnsiChar); stdcall;
procedure _TStrList_CopyTo(Source, Dest: TStringList); stdcall;

// -----------------------------------------------------------------------------
function _TMemStream_Create(): TMemoryStream; stdcall;
procedure _TMemStream_Free(Stream: TMemoryStream); stdcall;
function _TMemStream_GetSize(Stream: TMemoryStream): Int64; stdcall;
procedure _TMemStream_SetSize(Stream: TMemoryStream; NewSize: Integer); stdcall;
procedure _TMemStream_Clear(Stream: TMemoryStream); stdcall;
function _TMemStream_Read(Stream: TMemoryStream; Buffer: PAnsiChar; Count: Integer): Integer; stdcall;
function _TMemStream_Write(Stream: TMemoryStream; Buffer: PAnsiChar; Count: Integer): Integer; stdcall;
function _TMemStream_Seek(Stream: TMemoryStream; Offset: Integer; Origin: Word): Integer; stdcall;
function _TMemStream_Memory(Stream: TMemoryStream): Pointer; stdcall;
function _TMemStream_GetPosition(Stream: TMemoryStream): Int64; stdcall;
procedure _TMemStream_SetPosition(Stream: TMemoryStream; Position: Int64); stdcall;
procedure _TMemStream_LoadFromFile(Stream: TMemoryStream; FileName: PAnsiChar); stdcall;
procedure _TMemStream_SaveToFile(Stream: TMemoryStream; FileName: PAnsiChar); stdcall;

// -----------------------------------------------------------------------------
function _TMenu_GetMainMenu(): TMenuItem; stdcall;
function _TMenu_GetControlMenu(): TMenuItem; stdcall;
function _TMenu_GetViewMenu(): TMenuItem; stdcall;
function _TMenu_GetOptionMenu(): TMenuItem; stdcall;
function _TMenu_GetManagerMenu(): TMenuItem; stdcall;
function _TMenu_GetToolsMenu(): TMenuItem; stdcall;
function _TMenu_GetHelpMenu(): TMenuItem; stdcall;
function _TMenu_GetPluginMenu(): TMenuItem; stdcall;
function _TMenu_Count(MenuItem: TMenuItem): Integer; stdcall;
function _TMenu_GetItems(MenuItem: TMenuItem; Index: Integer): TMenuItem; stdcall;
function _TMenu_Add(PlugID: NativeInt; MenuItem: TMenuItem; Caption: PAnsiChar; Tag: Integer; OnClick: TNotifyEventEx)
  : TMenuItem; stdcall;
function _TMenu_Insert(PlugID: NativeInt; MenuItem: TMenuItem; Index: Integer; Caption: PAnsiChar; Tag: Integer;
  OnClick: TNotifyEventEx): TMenuItem; stdcall;
function _TMenu_GetCaption(MenuItem: TMenuItem; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TMenu_SetCaption(MenuItem: TMenuItem; Caption: PAnsiChar); stdcall;
function _TMenu_GetEnabled(MenuItem: TMenuItem): BOOL; stdcall;
procedure _TMenu_SetEnabled(MenuItem: TMenuItem; Enabled: BOOL); stdcall;
function _TMenu_GetVisable(MenuItem: TMenuItem): BOOL; stdcall;
procedure _TMenu_SetVisable(MenuItem: TMenuItem; Visible: BOOL); stdcall;
function _TMenu_GetChecked(MenuItem: TMenuItem): BOOL; stdcall;
procedure _TMenu_SetChecked(MenuItem: TMenuItem; Checked: BOOL); stdcall;
function _TMenu_GetRadioItem(MenuItem: TMenuItem): BOOL; stdcall;
procedure _TMenu_SetRadioItem(MenuItem: TMenuItem; IsRadioItem: BOOL); stdcall;
function _TMenu_GetGroupIndex(MenuItem: TMenuItem): Integer; stdcall;
procedure _TMenu_SetGroupIndex(MenuItem: TMenuItem; Value: Integer); stdcall;

function _TMenu_GetTag(MenuItem: TMenuItem): Integer; stdcall;
procedure _TMenu_SetTag(MenuItem: TMenuItem; Value: Integer); stdcall;

// -----------------------------------------------------------------------------
function _TIniFile_Create(sFileName: PAnsiChar): TIniFile; stdcall;
procedure _TIniFile_Free(IniFile: TIniFile); stdcall;
function _TIniFile_SectionExists(IniFile: TIniFile; Section: PAnsiChar): BOOL; stdcall;
function _TIniFile_ValueExists(IniFile: TIniFile; Section, Ident: PAnsiChar): BOOL; stdcall;
function _TIniFile_ReadString(IniFile: TIniFile; Section, Ident, Default: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD)
  : BOOL; stdcall;
procedure _TIniFile_WriteString(IniFile: TIniFile; Section, Ident, Value: PAnsiChar); stdcall;
function _TIniFile_ReadInteger(IniFile: TIniFile; Section, Ident: PAnsiChar; Default: Integer): Integer; stdcall;
procedure _TIniFile_WriteInteger(IniFile: TIniFile; Section, Ident: PAnsiChar; Value: Integer); stdcall;
function _TIniFile_ReadBool(IniFile: TIniFile; Section, Ident: PAnsiChar; Default: BOOL): BOOL; stdcall;
procedure _TIniFile_WriteBool(IniFile: TIniFile; Section, Ident: PAnsiChar; Value: BOOL); stdcall;

// -----------------------------------------------------------------------------
function _TMapManager_FindMap(MapName: PAnsiChar): TEnvirnoment; stdcall;
function _TMapManager_GetMapList(): TList; stdcall;

// -----------------------------------------------------------------------------
function _TEnvir_GetMapName(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TEnvir_GetMapDesc(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TEnvir_GetWidth(Envir: TEnvirnoment): Integer; stdcall;
function _TEnvir_GetHeight(Envir: TEnvirnoment): Integer; stdcall;
function _TEnvir_GetMinMap(Envir: TEnvirnoment): Integer; stdcall;
function _TEnvir_IsMainMap(Envir: TEnvirnoment): BOOL; stdcall;
function _TEnvir_GetMainMapName(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TEnvir_IsMirrMap(Envir: TEnvirnoment): BOOL; stdcall;
function _TEnvir_GetMirrMapCreateTick(Envir: TEnvirnoment): DWORD; stdcall;
function _TEnvir_GetMirrMapSurvivalTime(Envir: TEnvirnoment): DWORD; stdcall;
function _TEnvir_GetMirrMapExitToMap(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TEnvir_GetMirrMapMinMap(Envir: TEnvirnoment): Integer; stdcall;
function _TEnvir_GetAlwaysShowTime(Envir: TEnvirnoment): BOOL; stdcall;
function _TEnvir_IsFBMap(Envir: TEnvirnoment): BOOL; stdcall;
function _TEnvir_GetFBMapName(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TEnvir_GetFBEnterLimit(Envir: TEnvirnoment): Integer; stdcall;
function _TEnvir_GetFBCreated(Envir: TEnvirnoment): BOOL; stdcall;
function _TEnvir_GetFBCreateTime(Envir: TEnvirnoment): DWORD; stdcall;
function _TEnvir_GetMapParam(Envir: TEnvirnoment; Param: PAnsiChar): BOOL; stdcall;
function _TEnvir_GetMapParamValue(Envir: TEnvirnoment; Param: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TEnvir_CheckCanMove(Envir: TEnvirnoment; nX, nY: Integer; boFlag: BOOL): BOOL; stdcall;
function _TEnvir_IsValidObject(Envir: TEnvirnoment; nX, nY, nRange: Integer; AObject: TObject): BOOL; stdcall;
function _TEnvir_GetItemObjects(Envir: TEnvirnoment; nX, nY: Integer; ObjectList: TList): Integer; stdcall;
function _TEnvir_GetBaseObjects(Envir: TEnvirnoment; nX, nY: Integer; IncDeathObject: BOOL; ObjectList: TList): Integer; stdcall;
function _TEnvir_GetPlayObjects(Envir: TEnvirnoment; nX, nY: Integer; IncDeathObject: BOOL; ObjectList: TList): Integer; stdcall;

// -----------------------------------------------------------------------------
function _TM2Engine_GetVersion(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_GetVersionInt(): Integer; stdcall;
function _TM2Engine_GetMainFormHandle(): THandle; stdcall;
procedure _TM2Engine_SetMainFormCaption(Caption: PAnsiChar); stdcall;
function _TM2Engine_GetAppDir(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_GetGlobalIniFile(M2IniType: Integer): TIniFile; stdcall;
function _TM2Engine_GetOtherFileDir(M2FileType: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TM2Engine_MainOutMessage(Msg: PAnsiChar; IsAddTime: BOOL); stdcall;
function _TM2Engine_GetGlobalVarI(Index: Integer): Integer; stdcall;
function _TM2Engine_SetGlobalVarI(Index: Integer; Value: Integer): BOOL; stdcall;
function _TM2Engine_GetGlobalVarG(Index: Integer): Integer; stdcall;
function _TM2Engine_SetGlobalVarG(Index: Integer; Value: Integer): BOOL; stdcall;
function _TM2Engine_GetGlobalVarA(Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_SetGlobalVarA(Index: Integer; Value: PAnsiChar): BOOL; stdcall;
function _TM2Engine_EncodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_DecodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_ZLibEncodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_ZLibDecodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_EncryptBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_DecryptBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TM2Engine_EncryptPassword(InData: PAnsiChar; OutData: PAnsiChar; var OutSize: DWORD): BOOL; stdcall;
function _TM2Engine_DecryptPassword(InData: PAnsiChar; OutData: PAnsiChar; var OutSize: DWORD): BOOL; stdcall;
function _TM2Engine_GetTakeOnPosition(StdMode: Integer): Integer; stdcall;
function _TM2Engine_CheckBindType(BindValue: Byte; BindType: Byte): BOOL; stdcall;
procedure _TM2Engine_SetBindValue(var BindValue: Byte; BindType: Byte; Value: BOOL); stdcall;
function _TM2Engine_GetRGB(Color: Byte): DWORD; stdcall;

// -----------------------------------------------------------------------------
function _TBaseObject_GetChrName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TBaseObject_SetChrName(BaseObject: TBaseObject; NewName: PAnsiChar): BOOL; stdcall;
procedure _TBaseObject_RefShowName(BaseObject: TBaseObject); stdcall;
procedure _TBaseObject_RefNameColor(BaseObject: TBaseObject); stdcall;
function _TBaseObject_GetGender(BaseObject: TBaseObject): Byte; stdcall;
function _TBaseObject_SetGender(BaseObject: TBaseObject; Gender: Byte): BOOL; stdcall;
function _TBaseObject_GetJob(BaseObject: TBaseObject): Byte; stdcall;
function _TBaseObject_SetJob(BaseObject: TBaseObject; Job: Byte): BOOL; stdcall;
function _TBaseObject_GetHair(BaseObject: TBaseObject): Byte; stdcall;
procedure _TBaseObject_SetHair(BaseObject: TBaseObject; Hair: Byte); stdcall;
function _TBaseObject_GetEnvir(BaseObject: TBaseObject): TEnvirnoment; stdcall;
function _TBaseObject_GetMapName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TBaseObject_GetCurrX(BaseObject: TBaseObject): Integer; stdcall;
function _TBaseObject_GetCurrY(BaseObject: TBaseObject): Integer; stdcall;
function _TBaseObject_GetDirection(BaseObject: TBaseObject): Byte; stdcall;
function _TBaseObject_GetHomeMap(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TBaseObject_GetHomeX(BaseObject: TBaseObject): Integer; stdcall;
function _TBaseObject_GetHomeY(BaseObject: TBaseObject): Integer; stdcall;
function _TBaseObject_GetPermission(BaseObject: TBaseObject): Byte; stdcall;
procedure _TBaseObject_SetPermission(BaseObject: TBaseObject; Value: Byte); stdcall;
function _TBaseObject_GetDeath(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetDeathTick(BaseObject: TBaseObject): DWORD; stdcall;
function _TBaseObject_GetGhost(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetGhostTick(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_MakeGhost(BaseObject: TBaseObject); stdcall;
procedure _TBaseObject_ReAlive(BaseObject: TBaseObject); stdcall;
function _TBaseObject_GetRaceServer(BaseObject: TBaseObject): Byte; stdcall;
function _TBaseObject_GetAppr(BaseObject: TBaseObject): Word; stdcall;
function _TBaseObject_GetRaceImg(BaseObject: TBaseObject): Byte; stdcall;
function _TBaseObject_GetCharStatus(BaseObject: TBaseObject): Integer; stdcall;
procedure _TBaseObject_SetCharStatus(BaseObject: TBaseObject; Value: Integer); stdcall;
procedure _TBaseObject_StatusChanged(BaseObject: TBaseObject); stdcall;
function _TBaseObject_GetHungerPoint(BaseObject: TBaseObject): Integer; stdcall;
procedure _TBaseObject_SetHungerPoint(BaseObject: TBaseObject; Value: Integer); stdcall;
function _TBaseobject_IsNGMonster(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_IsDummyObject(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetViewRange(BaseObject: TBaseObject): Integer; stdcall;
procedure _TBaseObject_SetViewRange(BaseObject: TBaseObject; Value: Integer); stdcall;
function _TBaseObject_GetAbility(BaseObject: _TBaseObject; Dest: pTAbility): BOOL; stdcall;
function _TBaseObject_GetWAbility(BaseObject: _TBaseObject; Dest: pTAbility): BOOL; stdcall;
procedure _TBaseObject_SetWAbility(BaseObject: TBaseObject; Value: pTAbility); stdcall;
function _TBaseObject_GetSlaveList(BaseObject: TBaseObject): TList; stdcall;
function _TBaseObject_GetMaster(BaseObject: TBaseObject): TBaseObject; stdcall;
function _TBaseObject_GetMasterEx(BaseObject: TBaseObject): TBaseObject; stdcall;
function _TBaseObject_GetSuperManMode(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetSuperManMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetAdminMode(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetAdminMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetTransparent(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetTransparent(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetObMode(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetObMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetStoneMode(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetStoneMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetStickMode(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetStickMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetIsAnimal(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetIsAnimal(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetIsNoItem(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetIsNoItem(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetCoolEye(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetCoolEye(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetHitPoint(BaseObject: TBaseObject): Word; stdcall;
procedure _TBaseObject_SetHitPoint(BaseObject: TBaseObject; Value: Word); stdcall;
function _TBaseObject_GetSpeedPoint(BaseObject: TBaseObject): Word; stdcall;
procedure _TBaseObject_SetSpeedPoint(BaseObject: TBaseObject; Value: Word); stdcall;
function _TBaseObject_GetHitSpeed(BaseObject: TBaseObject): ShortInt; stdcall;
procedure _TBaseObject_SetHitSpeed(BaseObject: TBaseObject; Value: ShortInt); stdcall;
function _TBaseObject_GetWalkSpeed(BaseObject: TBaseObject): Integer; stdcall;
procedure _TBaseObject_SetWalkSpeed(BaseObject: TBaseObject; Value: Integer); stdcall;
function _TBaseObject_GetHPRecover(BaseObject: TBaseObject): ShortInt; stdcall;
procedure _TBaseObject_SetHPRecover(BaseObject: TBaseObject; Value: ShortInt); stdcall;
function _TBaseObject_GetMPRecover(BaseObject: TBaseObject): ShortInt; stdcall;
procedure _TBaseObject_SetMPRecover(BaseObject: TBaseObject; Value: ShortInt); stdcall;
function _TBaseObject_GetPoisonRecover(BaseObject: TBaseObject): ShortInt; stdcall;
procedure _TBaseObject_SetPoisonRecover(BaseObject: TBaseObject; Value: ShortInt); stdcall;
function _TBaseObject_GetAntiPoison(BaseObject: TBaseObject): Byte; stdcall;
procedure _TBaseObject_SetAntiPoison(BaseObject: TBaseObject; Value: Byte); stdcall;
function _TBaseObject_GetAntiMagic(BaseObject: TBaseObject): ShortInt; stdcall;
procedure _TBaseObject_SetAntiMagic(BaseObject: TBaseObject; Value: ShortInt); stdcall;
function _TBaseObject_GetLuck(BaseObject: TBaseObject): Integer; stdcall;
procedure _TBaseObject_SetLuck(BaseObject: TBaseObject; Value: Integer); stdcall;
function _TBaseObject_GetAttatckMode(BaseObject: TBaseObject): Byte; stdcall;
procedure _TBaseObject_SetAttatckMode(BaseObject: TBaseObject; Value: Byte); stdcall;
function _TBaseObject_GetNation(BaseObject: TBaseObject): Byte; stdcall;
function _TBaseObject_SetNation(BaseObject: TBaseObject; Nation: Byte): BOOL; stdcall;
function _TBaseObject_GetNationaName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TBaseObject_GetGuild(BaseObject: TBaseObject): TGUild; stdcall;
function _TBaseobject_GetGuildRankNo(BaseObject: TBaseObject): Integer; stdcall;
function _TBaseobject_GetGuildRankName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TBaseObject_IsGuildMaster(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetHideMode(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetHideMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetIsParalysis(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetIsParalysis(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetParalysisRate(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetParalysisRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsMDParalysis(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetIsMDParalysis(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetMDParalysisRate(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetMDParalysisRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsFrozen(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetIsFrozen(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetFrozenRate(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetFrozenRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsCobwebWinding(BaseObject: TBaseObject): BOOL; stdcall;
procedure _TBaseObject_SetIsCobwebWinding(BaseObject: TBaseObject; Value: BOOL); stdcall;
function _TBaseObject_GetCobwebWindingRate(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetCobwebWindingRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetUnParalysisValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnParalysisValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnParalysis(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnMagicShieldValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnMagicShieldValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnMagicShield(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnRevivalValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnRevivalValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnRevival(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnPosionValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnPosionValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnPosion(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnTammingValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnTammingValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnTamming(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnFireCrossValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnFireCrossValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnFireCross(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnFrozenValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnFrozenValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnFrozen(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetUnCobwebWindingValue(BaseObject: TBaseObject): DWORD; stdcall;
procedure _TBaseObject_SetUnCobwebWindingValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
function _TBaseObject_GetIsUnCobwebWinding(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_GetTargetCret(BaseObject: TBaseObject): TBaseObject; stdcall;
procedure _TBaseObject_SetTargetCret(BaseObject: TBaseObject; TargetCret: TBaseObject); stdcall;
procedure _TBaseObject_DelTargetCreat(BaseObject: TBaseObject); stdcall;
function _TBaseObject_GetLastHiter(BaseObject: TBaseObject): TBaseObject; stdcall;
function _TBaseObject_GetExpHitter(BaseObject: TBaseObject): TBaseObject; stdcall;
function _TBaseObject_GetPoisonHitter(BaseObject: TBaseObject): TBaseObject; stdcall;
function _TBaseObject_GetPoseCreate(BaseObject: TBaseObject): TBaseObject; stdcall;
// function  _TBaseObject_IsProtectTarget(AObject, BaseObject: TBaseObject): BOOL; stdcall;
// function  _TBaseObject_IsAttackTarget(AObject, BaseObject: TBaseObject): BOOL; stdcall;       // ???????
function _TBaseObject_IsProperTarget(BaseObject, Target: TBaseObject): BOOL; stdcall;
function _TBaseObject_IsProperFriend(BaseObject, Target: TBaseObject): BOOL; stdcall;
function _TBaseObject_TargetInRange(BaseObject, Target: TBaseObject; nX, nY, nRange: Integer): BOOL; stdcall;
procedure _TBaseObject_SendMsg(BaseObject, Target: TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt;
  sMsg: PAnsiChar); stdcall;
procedure _TBaseObject_SendDelayMsg(BaseObject, Target: TBaseObject; wIdent, wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar; dwDelay: DWORD); stdcall;
procedure _TBaseObject_SendRefMsg(BaseObject: TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt;
  sMsg: PAnsiChar; dwDelay: DWORD); stdcall;
procedure _TBaseObject_SendUpdateMsg(BaseObject, Target: TBaseObject; wIdent, wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar); stdcall;
function _TBaseObject_SysMsg(BaseObject: TBaseObject; sMsg: PAnsiChar; FColor, BColor: Byte; MsgType: Integer): BOOL; stdcall;
function _TBaseObject_GetBagItemList(BaseObject: TBaseObject): TList; stdcall;
function _TBaseObject_IsEnoughBag(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_IsEnoughBagEx(BaseObject: TBaseObject; AddCount: Integer): BOOL; stdcall;
function _TBaseObject_AddItemToBag(BaseObject: TBaseObject; UserItem: pTUserItem): BOOL; stdcall;
function _TBaseObject_DelBagItemByIndex(BaseObject: TBaseObject; Index: Integer): BOOL; stdcall;
function _TBaseObject_DelBagItemByMakeIdx(BaseObject: TBaseObject; MakeIndex: Integer; ItemName: PAnsiChar): BOOL; stdcall;
function _TBaseObject_DelBagItemByUserItem(BaseObject: TBaseObject; UserItem: pTUserItem): BOOL; stdcall;
function _TBaseObject_IsInSafeZone(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_IsPtInSafeZone(BaseObject: TBaseObject; Envir: TEnvirnoment; nX, nY: Integer): BOOL; stdcall;
procedure _TBaseObject_RecalcLevelAbil(BaseObject: TBaseObject; IsSysDef: BOOL); stdcall;
procedure _TBaseObject_RecalcAbil(BaseObject: TBaseObject); stdcall;
function _TBaseObject_RecalcBagWeight(BaseObject: TBaseObject): Integer; stdcall;
function _TBaseObject_GetLevelExp(BaseObject: TBaseObject; nLevel: Integer): DWORD; stdcall;
procedure _TBaseObject_HasLevelUp(BaseObject: TBaseObject; nLevel: Integer); stdcall;
function _TBaseObject_TrainSkill(BaseObject: TBaseObject; UserMagic: pTUserMagic; nTranPoint: Integer; IsDoCheck: BOOL)
  : BOOL; stdcall;
function _TBaseObject_CheckMagicLevelup(BaseObject: TBaseObject; UserMagic: pTUserMagic): BOOL; stdcall;
procedure _TBaseObject_MagicTranPointChanged(BaseObject: TBaseObject; UserMagic: pTUserMagic); stdcall;
procedure _TBaseObject_DamageHealth(BaseObject: TBaseObject; nDamage: Integer; StruckFrom: TBaseObject); stdcall;
procedure _TBaseObject_DamageSpell(BaseObject: TBaseObject; nSpellPoint: Integer); stdcall;
procedure _TBaseObject_IncHealthSpell(BaseObject: TBaseObject; nHP, nMP: Integer; SendChangedToClient: BOOL); stdcall;
procedure _TBaseObject_HealthSpellChanged(BaseObject: TBaseObject; dwDelay: DWORD); stdcall;
procedure _TBaseObject_FeatureChanged(BaseObject: TBaseObject); stdcall;
procedure _TBaseObject_WeightChanged(BaseObject: TBaseObject); stdcall;
function _TBaseObject_GetHitStruckDamage(BaseObject: TBaseObject; Target: TBaseObject; nDamage: Integer;
  MagicACInfo: PMagicACInfo; nType: Integer): Integer; stdcall;
function _TBaseObject_GetMagStruckDamage(BaseObject: TBaseObject; Target: TBaseObject; nDamage: Integer): Integer; stdcall;
function _TBaseObject_GetActorIcon(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;
function _TBaseObject_SetActorIcon(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;
procedure _TBaseObject_RefUseIcons(BaseObject: TBaseObject); stdcall;
procedure _TBaseObject_RefUseEffects(BaseObject: TBaseObject); stdcall;
procedure _TBaseObject_SpaceMove(BaseObject: TBaseObject; sMapName: PAnsiChar; nX, nY: Integer; nInt: Integer); stdcall;
procedure _TBaseObject_MapRandomMove(BaseObject: TBaseObject; sMapName: PAnsiChar; nInt: Integer); stdcall;
function _TBaseObject_CanMove(BaseObject: TBaseObject): BOOL; stdcall;
function _TBaseObject_CanRun(BaseObject: TBaseObject; nCurrX, nCurrY, nX, nY: Integer): BOOL; stdcall;
procedure _TBaseObject_TurnTo(BaseObject: TBaseObject; btDir: Byte); stdcall;
function _TBaseObject_WalkTo(BaseObject: TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;
function _TBaseObject_RunTo(BaseObject: TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;
function _TBaseObject_PluginList(BaseObject: TBaseObject): TList; stdcall;

// -----------------------------------------------------------------------------
function _TSmartObject_GetMagicList(SmartObject: TSmartObject): TList; stdcall;
function _TSmartObject_GetUseItem(SmartObject: TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;
function _TSmartObject_GetJewelryBoxStatus(SmartObject: TSmartObject): Integer; stdcall;
procedure _TSmartObject_SetJewelryBoxStatus(SmartObject: TSmartObject; Value: Integer); stdcall;
function _TSmartObject_GetJewelryItem(SmartObject: TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;
function _TSmartObject_GetIsShowGodBless(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsShowGodBless(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetGodBlessItemsState(SmartObject: TSmartObject; Index: Integer): BOOL; stdcall;
procedure _TSmartObject_SetGodBlessItemsState(SmartObject: TSmartObject; Index: Integer; Value: BOOL); stdcall;
function _TSmartObject_GetGodBlessItem(SmartObject: TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;
function _TSmartObject_GetFengHaoItems(SmartObject: TSmartObject): TList; stdcall;
function _TSmartObject_GetActiveFengHao(SmartObject: TSmartObject): Integer; stdcall;
procedure _TSmartObject_SetActiveFengHao(SmartObject: TSmartObject; FengHaoIndex: Integer); stdcall;
procedure _TSmartObject_ActiveFengHaoChanged(SmartObject: TSmartObject); stdcall;
procedure _TSmartObject_DeleteFengHao(SmartObject: TSmartObject; Index: Integer); stdcall;
procedure _TSmartObject_ClearFengHao(SmartObject: TSmartObject); stdcall;
function _TSmartObject_GetMoveSpeed(SmartObject: TSmartObject): SmallInt; stdcall;
procedure _TSmartObject_SetMoveSpeed(SmartObject: TSmartObject; Value: SmallInt); stdcall;
function _TSmartObject_GetAttackSpeed(SmartObject: TSmartObject): SmallInt; stdcall;
procedure _TSmartObject_SetAttackSpeed(SmartObject: TSmartObject; Value: SmallInt); stdcall;
function _TSmartObject_GetSpellSpeed(SmartObject: TSmartObject): SmallInt; stdcall;
procedure _TSmartObject_SetSpellSpeed(SmartObject: TSmartObject; Value: SmallInt); stdcall;
procedure _TSmartObject_RefGameSpeed(SmartObject: TSmartObject); stdcall;
function _TSmartObject_GetIsButch(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsButch(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsTrainingNG(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsTrainingNG(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsTrainingXF(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsTrainingXF(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsOpenLastContinuous(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsOpenLastContinuous(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetContinuousMagicOrder(SmartObject: TSmartObject; Index: Integer): Byte; stdcall;
procedure _TSmartObject_SetContinuousMagicOrder(SmartObject: TSmartObject; Index: Integer; Value: Byte); stdcall;
function _TSmartObject_GetPKDieLostExp(SmartObject: TSmartObject): DWORD; stdcall;
procedure _TSmartObject_SetPKDieLostExp(SmartObject: TSmartObject; Value: DWORD); stdcall;
function _TSmartObject_GetPKDieLostLevel(SmartObject: TSmartObject): Integer; stdcall;
procedure _TSmartObject_SetPKDieLostLevel(SmartObject: TSmartObject; Value: Integer); stdcall;
function _TSmartObject_GetPKPoint(SmartObject: TSmartObject): Integer; stdcall;
procedure _TSmartObject_SetPKPoint(SmartObject: TSmartObject; Value: Integer); stdcall;
procedure _TSmartObject_IncPKPoint(SmartObject: TSmartObject; Value: Integer); stdcall;
procedure _TSmartObject_DecPKPoint(SmartObject: TSmartObject; Value: Integer); stdcall;
function _TSmartObject_GetPKLevel(SmartObject: TSmartObject): Integer; stdcall;
procedure _TSmartObject_SetPKLevel(SmartObject: TSmartObject; Value: Integer); stdcall;
function _TSmartObject_GetIsTeleport(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsTeleport(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsRevival(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsRevival(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetRevivalTime(SmartObject: TSmartObject): Integer; stdcall;
procedure _TSmartObject_SetRevivalTime(SmartObject: TSmartObject; Value: Integer); stdcall;
function _TSmartObject_GetIsFlameRing(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsFlameRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsRecoveryRing(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsRecoveryRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsMagicShield(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsMagicShield(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsMuscleRing(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsMuscleRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsFastTrain(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsFastTrain(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsProbeNecklace(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsProbeNecklace(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsRecallSuite(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsRecallSuite(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsPirit(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsPirit(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsSupermanItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsSupermanItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsExpItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsExpItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetExpItemValue(SmartObject: TSmartObject): Real; stdcall;
procedure _TSmartObject_SetExpItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
function _TSmartObject_GetExpItemRate(SmartObject: TSmartObject): Integer; stdcall;
function _TSmartObject_GetIsPowerItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsPowerItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetPowerItemValue(SmartObject: TSmartObject): Real; stdcall;
procedure _TSmartObject_SetPowerItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
function _TSmartObject_GetPowerItemRate(SmartObject: TSmartObject): Integer; stdcall;
function _TSmartObject_GetIsGuildMove(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsGuildMove(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsAngryRing(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsAngryRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsStarRing(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsStarRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsACItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsACItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetACItemValue(SmartObject: TSmartObject): Real; stdcall;
procedure _TSmartObject_SetACItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
function _TSmartObject_GetIsMACItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsMACItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetMACItemValue(SmartObject: TSmartObject): Real; stdcall;
procedure _TSmartObject_SetMACItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
function _TSmartObject_GetIsNoDropItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsNoDropItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetIsNoDropUseItem(SmartObject: TSmartObject): BOOL; stdcall;
procedure _TSmartObject_SetIsNoDropUseItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
function _TSmartObject_GetNGAbility(SmartObject: _TSmartObject; AbilityNG: pTAbilityNG): BOOL; stdcall;
procedure _TSmartObject_SetNGAbility(SmartObject: TSmartObject; Value: pTAbilityNG); stdcall;
function _TSmartObject_GetAlcohol(SmartObject: _TSmartObject; AbilityAlcohol: pTAbilityAlcohol): BOOL; stdcall;
procedure _TSmartObject_SetAlcohol(SmartObject: TSmartObject; Value: pTAbilityAlcohol); stdcall;
procedure _TSmartObject_RepairAllItem(SmartObject: TSmartObject); stdcall;
function _TSmartObject_IsAllowUseMagic(SmartObject: TSmartObject; MagicID: Word): BOOL; stdcall;
function _TSmartObject_SelectMagic(SmartObject: TSmartObject): Integer; stdcall;
function _TSmartObject_AttackTarget(SmartObject: TSmartObject; MagicID: Word; AttackTime: DWORD): BOOL; stdcall;

// -----------------------------------------------------------------------------
function _TPlayObject_GetUserID(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetIPAddr(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetIPLocal(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetMachineID(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetIsReadyRun(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetLogonTime(Player: TPlayObject; LogonTime: PSystemTime): BOOL; stdcall;
function _TPlayObject_GetSoftVerDate(Player: TPlayObject): Integer; stdcall;
function _TPlayObject_GetClientType(Player: TPlayObject): Integer; stdcall;
function _TPlayObject_IsOldClient(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetScreenWidth(Player: TPlayObject): Word; stdcall;
function _TPlayObject_GetScreenHeight(Player: TPlayObject): Word; stdcall;
function _TPlayObject_GetClientViewRange(Player: TPlayObject): Word; stdcall;
function _TPlayObject_GetRelevel(Player: TPlayObject): Byte; stdcall;
procedure _TPlayObject_SetRelevel(Player: TPlayObject; Value: Byte); stdcall;
function _TPlayObject_GetBonusPoint(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetBonusPoint(Player: TPlayObject; Value: Integer); stdcall;
procedure _TPlayObject_SendAdjustBonus(Player: TPlayObject); stdcall;
function _TPlayObject_GetHeroName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetDeputyHeroName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetDeputyHeroJob(Player: TPlayObject): Byte; stdcall;
function _TPlayObject_GetMyHero(Player: TPlayObject): THeroObject; stdcall;
function _TPlayObject_GetFixedHero(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_ClientHeroLogOn(Player: TPlayObject; IsDeputyHero: BOOL); stdcall; // ????
function _TPlayObject_GetStorageHero(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetStorageDeputyHero(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetIsStorageOpen(Player: TPlayObject; Index: Integer): BOOL; stdcall;
procedure _TPlayObject_SetIsStorageOpen(Player: TPlayObject; Index: Integer; Value: BOOL); stdcall;
function _TPlayObject_GetGold(Player: TPlayObject): DWORD; stdcall;
procedure _TPlayObject_SetGold(Player: TPlayObject; Value: DWORD); stdcall;
function _TPlayObject_GetGoldMax(Player: TPlayObject): DWORD; stdcall;
function _TPlayObject_IncGold(Player: TPlayObject; Value: DWORD): BOOL; stdcall;
function _TPlayObject_DecGold(Player: TPlayObject; Value: DWORD): BOOL; stdcall;
procedure _TPlayObject_GoldChanged(Player: TPlayObject); stdcall;
function _TPlayObject_GetGameGold(Player: TPlayObject): DWORD; stdcall;
procedure _TPlayObject_SetGameGold(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_IncGameGold(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_DecGameGold(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_GameGoldChanged(Player: TPlayObject); stdcall;
function _TPlayObject_GetGamePoint(Player: TPlayObject): DWORD; stdcall;
procedure _TPlayObject_SetGamePoint(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_IncGamePoint(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_DecGamePoint(Player: TPlayObject; Value: DWORD); stdcall;
function _TPlayObject_GetGameDiamond(Player: TPlayObject): DWORD; stdcall;
procedure _TPlayObject_SetGameDiamond(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_IncGameDiamond(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_DecGameDiamond(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_NewGamePointChanged(Player: TPlayObject); stdcall;
function _TPlayObject_GetGameGird(Player: TPlayObject): DWORD; stdcall;
procedure _TPlayObject_SetGameGird(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_IncGameGird(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_DecGameGird(Player: TPlayObject; Value: DWORD); stdcall;
function _TPlayObject_GetGameGoldEx(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetGameGoldEx(Player: TPlayObject; Value: Integer); stdcall;
function _TPlayObject_GetGameGlory(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetGameGlory(Player: TPlayObject; Value: Integer); stdcall;
procedure _TPlayObject_IncGameGlory(Player: TPlayObject; Value: Integer); stdcall;
procedure _TPlayObject_DecGameGlory(Player: TPlayObject; Value: Integer); stdcall;
procedure _TPlayObject_GameGloryChanged(Player: TPlayObject); stdcall;
function _TPlayObject_GetPayMentPoint(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetPayMentPoint(Player: TPlayObject; Value: Integer); stdcall;
function _TPlayObject_GetMemberType(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetMemberType(Player: TPlayObject; Value: Integer); stdcall;
function _TPlayObject_GetMemberLevel(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetMemberLevel(Player: TPlayObject; Value: Integer); stdcall;
function _TPlayObject_GetContribution(Player: TPlayObject): Word; stdcall;
procedure _TPlayObject_SetContribution(Player: TPlayObject; Value: Word); stdcall;
procedure _TPlayObejct_IncExp(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_SendExpChanged(Player: TPlayObject); stdcall;
procedure _TPlayObject_IncExpNG(Player: TPlayObject; Value: DWORD); stdcall;
procedure _TPlayObject_SendExpNGChanged(Player: TPlayObject); stdcall;
procedure _TPlayObject_IncBeadExp(Player: TPlayObject; Value: DWORD; IsFromNPC: BOOL); stdcall;
function _TPlayObject_GetVarP(Player: TPlayObject; Index: Integer): Integer; stdcall;
procedure _TPlayObject_SetVarP(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
function _TPlayObject_GetVarM(Player: TPlayObject; Index: Integer): Integer; stdcall;
procedure _TPlayObject_SetVarM(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
function _TPlayObject_GetVarD(Player: TPlayObject; Index: Integer): Integer; stdcall;
procedure _TPlayObject_SetVarD(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
function _TPlayObject_GetVarU(Player: TPlayObject; Index: Integer): Integer; stdcall;
procedure _TPlayObject_SetVarU(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
function _TPlayObject_GetVarT(Player: TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TPlayObject_SetVarT(Player: TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;
function _TPlayObject_GetVarN(Player: TPlayObject; Index: Integer): Integer; stdcall;
procedure _TPlayObject_SetVarN(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
function _TPlayObject_GetVarS(Player: TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TPlayObject_SetVarS(Player: TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;
function _TPlayObject_GetDynamicVarList(Player: TPlayObject): TList; stdcall;
function _TPlayObject_GetQuestFlagStatus(Player: TPlayObject; nFlag: Integer): Integer; stdcall;
procedure _TPlayObject_SetQuestFlagStatus(Player: TPlayObject; nFlag: Integer; Value: Integer); stdcall;
function _TPlayObject_IsOffLine(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_IsMaster(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetMasterName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetMasterHuman(Player: TPlayObject): TPlayObject; stdcall;
function _TPlayObject_GetApprenticeNO(Player: TPlayObject): Integer; stdcall;
function _TPlayObject_GetOnlineApprenticeList(Player: TPlayObject): TList; stdcall;
function _TPlayObject_GetAllApprenticeList(Player: TPlayObject): TList; stdcall;
function _TPlayObject_GetDearName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TPlayObject_GetDearHuman(Player: TPlayObject): TPlayObject; stdcall;
function _TPlayObject_GetMarryCount(Player: TPlayObject): Byte; stdcall;
function _TPlayObject_GetGroupOwner(Player: TPlayObject): TPlayObject; stdcall;
function _TPlayObject_GetGroupMembers(Player: TPlayObject): TStringList; stdcall;
function _TPlayObject_GetIsLockLogin(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsLockLogin(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsAllowGroup(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsAllowGroup(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsAllowGroupReCall(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsAllowGroupReCall(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsAllowGuildReCall(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsAllowGuildReCall(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsAllowTrading(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsAllowTrading(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsDisableInviteHorseRiding(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsDisableInviteHorseRiding(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsGameGoldTrading(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsGameGoldTrading(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsNewServer(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetIsFilterGlobalDropItemMsg(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsFilterGlobalDropItemMsg(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsFilterGlobalCenterMsg(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsFilterGlobalCenterMsg(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsFilterGolbalSendMsg(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsFilterGolbalSendMsg(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetIsPleaseDrink(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_GetIsDrinkWineQuality(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetIsDrinkWineQuality(Player: TPlayObject; Value: Integer); stdcall;
function _TPlayObject_GetIsDrinkWineAlcohol(Player: TPlayObject): Integer; stdcall;
procedure _TPlayObject_SetIsDrinkWineAlcohol(Player: TPlayObject; Value: Integer); stdcall;
function _TPlayObject_GetIsDrinkWineDrunk(Player: TPlayObject): BOOL; stdcall;
procedure _TPlayObject_SetIsDrinkWineDrunk(Player: TPlayObject; Value: BOOL); stdcall;
function _TPlayObject_GetAlcohol(Player: TPlayObject): pTAbilityAlcohol; stdcall;
procedure _TPlayObject_MoveToHome(Player: TPlayObject); stdcall;
procedure _TPlayObject_MoveRandomToHome(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendSocket(Player: TPlayObject; DefMsg: pTDefaultMessage; sMsg: PAnsiChar); stdcall;

// 2021-01-05 changed
procedure _TPlayObject_SendDefMessage(Player: TPlayObject; wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word;
  sMsg: PAnsiChar); stdcall;
procedure _TPlayObject_SendMoveMsg(Player: TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nY: Word; nMoveCount: Integer;
  nFontSize: Integer; nMarqueeTime: Integer); stdcall;
procedure _TPlayObject_SendCenterMsg(Player: TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;
function _TPlayObject_SendTopBroadCastMsg(Player: TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer;
  MsgType: Integer): BOOL; stdcall;

function _TPlayObject_CheckTakeOnItems(Player: TPlayObject; Where: Integer; StdItem: PTStdItem): BOOL; stdcall;
procedure _TPlayObject_ProcessUseItemSkill(Player: TPlayObject; Where: Integer; StdItem: PTStdItem; IsTakeOn: BOOL); stdcall;

procedure _TPlayObject_SendUseItems(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendAddItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
procedure _TPlayObject_SendDelItemList(Player: TPlayObject; Items: PAnsiChar; ItemsCount: Integer); stdcall;
procedure _TPlayObject_SendDelItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
procedure _TPlayObject_SendUpdateItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
procedure _TPlayObject_SendItemDuraChange(Player: TPlayObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;
procedure _TPlayObject_SendBagItems(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendJewelryBoxItems(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendGodBlessItems(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendOpenGodBlessItem(Player: TPlayObject; Index: Integer); stdcall;
procedure _TPlayObject_SendCloseGodBlessItem(Player: TPlayObject; Index: Integer); stdcall;
procedure _TPlayObject_SendUseMagics(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendAddMagic(Player: TPlayObject; UserMagic: pTUserMagic); stdcall;
procedure _TPlayObject_SendDelMagic(Player: TPlayObject; UserMagic: pTUserMagic); stdcall;
procedure _TPlayObject_SendFengHaoItems(Player: TPlayObject); stdcall;
procedure _TPlayObject_SendAddFengHaoItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
procedure _TPlayObject_SendDelFengHaoItem(Player: TPlayObject; Index: Integer); stdcall;
procedure _TPlayObject_SendSocketStatusFail(Player: TPlayObject); stdcall;
procedure _TPlayObject_PlayEffect(Player: TPlayObject; nFileIndex, nImageOffset, nImageCount, nLoopCount, nSpeedTime: Integer;
  btDrawOrder: Byte; nOffsetX: Integer; nOffsetY: Integer); stdcall;
function _TPlayObject_IsAutoPlayGame(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_StartAutoPlayGame(Player: TPlayObject): BOOL; stdcall;
function _TPlayObject_StopAutoPlayGame(Player: TPlayObject): BOOL; stdcall;

function _TPlayObject_GetHeroM2ShopList(Player: TPlayObject): TList; stdcall;
function _TPlayObject_GetHeroM2ShopOpenList(Player: TPlayObject): TList; stdcall;
// -----------------------------------------------------------------------------
function _TDummyObject_IsStart(Dummyer: TDummyObject): BOOL; stdcall;
procedure _TDummyObject_Start(Dummyer: TDummyObject); stdcall;
procedure _TDummyObject_Stop(Dummyer: TDummyObject); stdcall;

// -----------------------------------------------------------------------------
function _THeroObject_GetAttackMode(Hero: THeroObject): Byte; stdcall;
function _THeroObject_SetAttackMode(Hero: THeroObject; Value: Byte; ShowSysMsg: BOOL): BOOL; stdcall;
procedure _THeroObject_SetNextAttackMode(Hero: THeroObject); stdcall;
function _THeroObject_GetBagCount(Hero: THeroObject): Integer; stdcall;
function _THeroObject_GetAngryValue(Hero: THeroObject): Integer; stdcall;
function _THeroObject_GetLoyalPoint(Hero: THeroObject): Real; stdcall;
procedure _THeroObject_SetLoyalPoint(Hero: THeroObject; Value: Real); stdcall;
procedure _THeroObject_SendLoyalPointChanged(Hero: THeroObject); stdcall;
function _THeroObject_IsDeputy(Hero: THeroObject): BOOL; stdcall;
function _THeroObject_GetMasterName(Hero: THeroObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _THeroObject_GetQuestFlagStatus(Hero: THeroObject; nFlag: Integer): Integer; stdcall;
procedure _THeroObject_SetQuestFlagStatus(Hero: THeroObject; nFlag: Integer; Value: Integer); stdcall;
procedure _THeroObject_SendUseItems(Hero: THeroObject); stdcall;
procedure _THeroObject_SendBagItems(Hero: THeroObject); stdcall;
procedure _THeroObject_SendJewelryBoxItems(Hero: THeroObject); stdcall;
procedure _THeroObject_SendGodBlessItems(Hero: THeroObject); stdcall;
procedure _THeroObject_SendOpenGodBlessItem(Hero: THeroObject; Index: Integer); stdcall;
procedure _THeroObject_SendCloseGodBlessItem(Hero: THeroObject; Index: Integer); stdcall;
procedure _THeroObject_SendAddItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
procedure _THeroObject_SendDelItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
procedure _THeroObject_SendUpdateItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
procedure _THeroObject_SendItemDuraChange(Hero: THeroObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;
procedure _THeroObject_SendUseMagics(Hero: THeroObject); stdcall;
procedure _THeroObject_SendAddMagic(Hero: THeroObject; UserMagic: pTUserMagic); stdcall;
procedure _THeroObject_SendDelMagic(Hero: THeroObject; UserMagic: pTUserMagic); stdcall;
function _THeroObject_FindGroupMagic(Hero: THeroObject; UserMagic: pTUserMagic): BOOL; stdcall;
function _THeroObject_GetGroupMagicId(Hero: THeroObject): Integer; stdcall;
procedure _THeroObject_SendFengHaoItems(Hero: THeroObject); stdcall;
procedure _THeroObject_SendAddFengHaoItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
procedure _THeroObject_SendDelFengHaoItem(Hero: THeroObject; Index: Integer); stdcall; // ??????
procedure _THeroObject_IncExp(Hero: THeroObject; dwExp: DWORD); stdcall;
procedure _THeroObject_IncExpNG(Hero: THeroObject; dwExp: DWORD); stdcall;
function _THeroObject_IsOldClient(Hero: THeroObject): BOOL; stdcall;

// -----------------------------------------------------------------------------
function _TNormNpc_Create(sCharName, sMapName, sScript: PAnsiChar; X, Y: Integer; wAppr: Word; boIsHide: BOOL): TNormNpc; stdcall;
procedure _TNormNpc_LoadNpcScript(NormNpc: TNormNpc); stdcall;
procedure _TNormNpc_ClearScript(NormNpc: TNormNpc); stdcall;
function _TNormNpc_GetFilePath(NormNpc: TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TNormNpc_SetFilePath(NormNpc: TNormNpc; Value: PAnsiChar); stdcall;
function _TNormNpc_GetPath(NormNpc: TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
procedure _TNormNpc_SetPath(NormNpc: TNormNpc; Value: PAnsiChar); stdcall;
function _TNormNpc_GetIsHide(NormNpc: TNormNpc): BOOL; stdcall;
procedure _TNormNpc_SetIsHide(NormNpc: TNormNpc; Value: BOOL); stdcall;
function _TNormNpc_GetIsQuest(NormNpc: TNormNpc): BOOL; stdcall;
function _TNormNpc_GetLineVariableText(NormNpc: TNormNpc; Player: TPlayObject; sMsg: PAnsiChar; Dest: PAnsiChar;
  var DestLen: DWORD): BOOL; stdcall;
procedure _TNormNpc_GotoLable(NormNpc: TNormNpc; Player: TPlayObject; sLabel: PAnsiChar; boExtJmp: BOOL); stdcall;
procedure _TNormNpc_SendMsgToUser(NormNpc: TNormNpc; Player: TPlayObject; sMsg: PAnsiChar); stdcall;
procedure _TNormNpc_MessageBox(NormNpc: TNormNpc; Player: TPlayObject; sMsg: PAnsiChar); stdcall;
function _TNormNpc_GetVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar;
  var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;
function _TNormNpc_SetVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; nValue: Integer)
  : BOOL; stdcall;
function _TNormNpc_GetDynamicVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar;
  var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;
function _TNormNpc_SetDynamicVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar;
  nValue: Integer): BOOL; stdcall;

// -----------------------------------------------------------------------------
function _TMagicACList_Count(List: TMagicACList): Integer; stdcall;
function _TMagicACList_GetItem(List: TMagicACList; Index: Integer): PMagicACInfo; stdcall;
function _TMagicACList_FindByMagIdx(List: TMagicACList; MagIdx: Integer): PMagicACInfo; stdcall;

// -----------------------------------------------------------------------------
function _TUserEngine_GetPlayerList(): TStringList; stdcall;
function _TUserEngine_GetPlayerByName(ChrName: PAnsiChar): TPlayObject; stdcall;
function _TUserEngine_GetPlayerByUserID(UserID: PAnsiChar): TPlayObject; stdcall;
function _TUserEngine_GetPlayerByObject(AObject: TObject): TPlayObject; stdcall;
function _TUserEngine_GetOfflinePlayer(UserID: PAnsiChar): TPlayObject; stdcall;
procedure _TUserEngine_KickPlayer(ChrName: PAnsiChar); stdcall;
function _TUserEngine_GetHeroList(): TStringList; stdcall;
function _TUserEngine_GetHeroByName(ChrName: PAnsiChar): THeroObject; stdcall;
function _TUserEngine_KickHero(ChrName: PAnsiChar): BOOL; stdcall;
function _TUserEngine_GetMerchantList(): TList; stdcall;
function _TUserEngine_GetCustomNpcConfigList(): TList; stdcall;
function _TUserEngine_GetQuestNPCList(): TStringList; stdcall;
function _TUserEngine_GetManageNPC(): TNormNpc; stdcall;
function _TUserEngine_GetFunctionNPC(): TNormNpc; stdcall;
function _TUserEngine_GetRobotNPC(): TNormNpc; stdcall;
function _TUserEngine_MissionNPC(): TNormNpc; stdcall;
function _TUserEngine_FindMerchant(AObject: TObject): TNormNpc; stdcall;
function _TUserEngine_FindMerchantByPos(MapName: PAnsiChar; nX, nY: Integer): TNormNpc; stdcall;
function _TUserEngine_FindQuestNPC(AObject: TObject): TNormNpc; stdcall;
function _TUserEngine_GetMagicList(): TList; stdcall;
function _TUserEngine_GetCustomMagicConfigList(): TList; stdcall;
function _TUserEngine_GetMagicACList(): TMagicACList; stdcall;
function _TUserEngine_FindMagicByName(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindMagicByIndex(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindMagicByNameEx(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindMagicByIndexEx(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindHeroMagicByName(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindHeroMagicByIndex(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindHeroMagicByNameEx(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_FindHeroMagicByIndexEx(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
function _TUserEngine_GetStdItemList(): TList; stdcall;
function _TUserEngine_GetStdItemByName(ItemName: PAnsiChar; StdItem: PTStdItem): BOOL; stdcall;
function _TUserEngine_GetStdItemByIndex(ItemIdx: Integer; StdItem: PTStdItem): BOOL; stdcall;
function _TUserEngine_GetStdItemName(ItemIdx: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TUserEngine_GetStdItemIndex(ItemName: PAnsiChar): Integer; stdcall;
function _TUserEngine_MonsterList(): TList; stdcall;
function _TUserEngine_SendBroadCastMsg(sMsg: PAnsiChar; FColor, BColor: Integer; MsgType: Integer): BOOL; stdcall;
function _TUserEngine_SendBroadCastMsgExt(sMsg: PAnsiChar; MsgType: Integer): BOOL; stdcall;
function _TUserEngine_SendTopBroadCastMsg(sMsg: PAnsiChar; FColor, BColor: Integer; nTime: Integer; MsgType: Integer)
  : BOOL; stdcall;
procedure _TUserEngine_SendMoveMsg(sMsg: PAnsiChar; btFColor, btBColor: Byte; nY, nMoveCount: Integer; nFontSize: Integer;
  nMarqueeTime: Integer); stdcall;
procedure _TUserEngine_SendCenterMsg(sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;
procedure _TUserEngine_SendNewLineMsg(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte;
  nY, nShowMsgTime, nDrawType: Integer); stdcall;
procedure _TUserEngine_SendSuperMoveMsg(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte; nY, nMoveCount: Integer); stdcall;
procedure _TUserEngine_SendSceneShake(Count: Integer); stdcall;
function _TUserEngine_CopyToUserItemFromName(ItemName: PAnsiChar; UserItem: pTUserItem): BOOL; stdcall;
function _TUserEngine_CopyToUserItemFromItem(StdItem: PTStdItem; ItemIndex: Integer; UserItem: pTUserItem): BOOL; stdcall;
procedure _TUserEngine_RandomUpgradeItem(UserItem: pTUserItem); stdcall;
procedure _TUserEngine_RandomItemNewAbil(UserItem: pTUserItem); stdcall;
procedure _TUserEngine_GetUnknowItemValue(UserItem: pTUserItem); stdcall;
function _TUserEngine_GetAllDummyCount(): Integer; stdcall;
function _TUserEngine_GetMapDummyCount(Envir: TEnvirnoment): Integer; stdcall;
function _TUserEngine_GetOfflineCount(): Integer; stdcall;
function _TUserEngine_GetRealPlayerCount(): Integer; stdcall;

// -----------------------------------------------------------------------------
function _TGuildManager_FindGuild(GuildName: PAnsiChar): TGUild; stdcall;
function _TGuildManager_GetPlayerGuild(CharName: PAnsiChar): TGUild; stdcall;
function _TGuildManager_AddGuild(GuildName, GuildMaster: PAnsiChar): BOOL; stdcall;
function _TGuildManager_DelGuild(GuildName: PAnsiChar; var IsFoundGuild: BOOL): BOOL; stdcall;

// -----------------------------------------------------------------------------
function _TGuild_GetGuildName(Guild: TGUild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TGuild_GetJoinJob(Guild: TGUild): Integer; stdcall;
function _TGuild_GetJoinLevel(Guild: TGUild): DWORD; stdcall;
function _TGuild_GetJoinMsg(Guild: TGUild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
function _TGuild_GetBuildPoint(Guild: TGUild): Integer; stdcall;
function _TGuild_GetAurae(Guild: TGUild): Integer; stdcall;
function _TGuild_GetStability(Guild: TGUild): Integer; stdcall;
function _TGuild_GetFlourishing(Guild: TGUild): Integer; stdcall;
function _TGuild_GetChiefItemCount(Guild: TGUild): Integer; stdcall;
function _TGuild_GetMemberCount(Guild: TGUild): Integer; stdcall;
function _TGuild_GetOnlineMemeberCount(Guild: TGUild): Integer; stdcall;
function _TGuild_GetMasterCount(Guild: TGUild): Integer; stdcall;
procedure _TGuild_GetMaster(Guild: TGUild; var Master1, Master2: TPlayObject); stdcall;
function _TGuild_GetMasterName(Guild: TGUild; Master1: PAnsiChar; var Master1Size: DWORD; Master2: PAnsiChar;
  var Master2Size: DWORD): BOOL; stdcall; // 得到行会正副掌门名称
function _TGuild_CheckMemberIsFull(Guild: TGUild): BOOL; stdcall;
function _TGuild_IsMemeber(Guild: TGUild; CharName: PAnsiChar): BOOL; stdcall;
function _TGuild_AddMember(Guild: TGUild; Player: TPlayObject): BOOL; stdcall;
function _TGuild_AddMemberEx(Guild: TGUild; CharName: PAnsiChar): BOOL; stdcall;
function _TGuild_DelMemeber(Guild: TGUild; Player: TPlayObject): BOOL; stdcall;
function _TGuild_DelMemeberEx(Guild: TGUild; CharName: PAnsiChar): BOOL; stdcall;
function _TGuild_IsAllianceGuild(Guild: TGUild; CheckGuild: TGUild): BOOL; stdcall;
function _TGuild_IsWarGuild(Guild: TGUild; CheckGuild: TGUild): BOOL; stdcall;
function _TGuild_IsAttentionGuild(Guild: TGUild; CheckGuild: TGUild): BOOL; stdcall;
function _TGuild_AddAlliance(Guild: TGUild; AddGuild: TGUild): BOOL; stdcall;
function _TGuild_AddWarGuild(Guild: TGUild; AddGuild: TGUild): BOOL; stdcall;
function _TGuild_AddAttentionGuild(Guild: TGUild; AddGuild: TGUild): BOOL; stdcall;
function _TGuild_DelAllianceGuild(Guild: TGUild; DelGuild: TGUild): BOOL; stdcall;
function _TGuild_DelAttentionGuild(Guild: TGUild; DelGuild: TGUild): BOOL; stdcall;
function _TGuild_GetRandNameByName(Guild: TGUild; CharName: PAnsiChar; var nRankNo: Integer; Dest: PAnsiChar; var DestLen: DWORD)
  : BOOL; stdcall;
function _TGuild_GetRandNameByPlayer(Guild: TGUild; Player: TPlayObject; var nRankNo: Integer; Dest: PAnsiChar;
  var DestLen: DWORD): BOOL; stdcall;
procedure _TGuild_SendGuildMsg(Guild: TGUild; Msg: PAnsiChar); stdcall;

implementation

uses
  svMain,
  M2Share,
  DesUtils,
  PluginManager;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

function _TMemory_Alloc(Size: Integer): Pointer; stdcall;
begin
  Result := nil;

  try
    Result := AllocMem(Size);
  except
    MainOutMessage('[Exception] Plugin AllocMem');
  end;
end;

procedure _TMemory_Free(P: Pointer); stdcall;
begin
  try
    FreeMem(P);
  except
    MainOutMessage('[Exception] Plugin FreeMem');
  end;
end;

procedure _TMemory_Realloc(P: Pointer; Size: Integer); stdcall;
begin
  try
    ReallocMem(P, Size);
  except
    MainOutMessage('[Exception] Plugin ReallocMem');
  end;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

function _TList_Create(): TList; stdcall;
begin
  Result := TList.Create;
end;

procedure _TList_Free(List: TList); stdcall;
begin
  List.Free;
end;

function _TList_Count(List: TList): Integer; stdcall;
begin
  Result := List.Count;
end;

procedure _TList_Clear(List: TList); stdcall;
begin
  List.Clear;
end;

procedure _TList_Add(List: TList; Item: Pointer); stdcall;
begin
  List.Add(Item);
end;

procedure _TList_Insert(List: TList; Index: Integer; Item: Pointer); stdcall;
begin
  List.Insert(Index, Item);
end;

procedure _TList_Remove(List: TList; Item: Pointer); stdcall;
begin
  List.Remove(Item);
end;

procedure _TList_Delete(List: TList; Index: Integer); stdcall;
begin
  List.Delete(Index);
end;

function _TList_GetItem(List: TList; Index: Integer): Pointer; stdcall;
begin
  Result := List.Items[Index];
end;

procedure _TList_SetItem(List: TList; Index: Integer; Item: Pointer); stdcall;
begin
  List.Items[Index] := Item;
end;

function _TList_IndexOf(List: TList; Item: Pointer): Integer; stdcall;
begin
  Result := List.IndexOf(Item);
end;

procedure _TList_Exchange(List: TList; Index1, Index2: Integer); stdcall;
begin
  List.Exchange(Index1, Index2);
end;

procedure _TList_CopyTo(Source, Dest: TList); stdcall;
begin
  Dest.Assign(Source);
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

function _TStrList_Create(): TStringList; stdcall;
begin
  Result := TStringList.Create;
end;

procedure _TStrList_Free(Strings: TStringList); stdcall;
begin
  Strings.Free;
end;

function _TStrList_GetCaseSensitive(Strings: TStringList): BOOL; stdcall;
begin
  Result := Strings.CaseSensitive;
end;

procedure _TStrList_SetCaseSensitive(Strings: TStringList; IsCaseSensitive: BOOL); stdcall;
begin
  Strings.CaseSensitive := IsCaseSensitive;
end;

function _TStrList_GetSorted(Strings: TStringList): BOOL; stdcall;
begin
  Result := Strings.Sorted;
end;

procedure _TStrList_SetSorted(Strings: TStringList; Sorted: BOOL); stdcall;
begin
  Strings.Sorted := Sorted;
end;

function _TStrList_GetDuplicates(Strings: TStringList): BOOL; stdcall; // 是否重复，排序后生效
begin
  Result := Strings.Duplicates = dupAccept;
end;

procedure _TStrList_SetDuplicates(Strings: TStringList; Duplicates: BOOL); stdcall;
begin
  if Duplicates then
    Strings.Duplicates := dupAccept
  else
    Strings.Duplicates := dupIgnore;
end;

function _TStrList_Count(Strings: TStringList): Integer; stdcall;
begin
  Result := Strings.Count;
end;

function _TStrList_GetText(Strings: _TStringList; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Strings.Text;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TStrList_SetText(Strings: _TStringList; Src: PAnsiChar; SrcLen: DWORD); stdcall;
var
  S: AnsiString;
begin
  SetLength(S, SrcLen);
  Move(Src^, S[1], SrcLen);
  Strings.Text := S;
end;

procedure _TStrList_Add(Strings: TStringList; S: PAnsiChar); stdcall;
begin
  Strings.Add(S);
end;

procedure _TStrList_AddObject(Strings: TStringList; S: PAnsiChar; AObject: TObject); stdcall;
begin
  Strings.AddObject(S, AObject);
end;

procedure _TStrList_Insert(Strings: TStringList; Index: Integer; S: PAnsiChar); stdcall;
begin
  Strings.Insert(Index, S);
end;

procedure _TStrList_InsertObject(Strings: TStringList; Index: Integer; S: PAnsiChar; AObject: TObject); stdcall;
begin
  Strings.InsertObject(Index, S, AObject);
end;

procedure _TStrList_Remove(Strings: TStringList; S: PAnsiChar); stdcall;
var
  Index: Integer;
begin
  Index := Strings.IndexOf(S);
  if Index >= 0 then
    Strings.Delete(Index);
end;

procedure _TStrList_Delete(Strings: TStringList; Index: Integer); stdcall;
begin
  Strings.Delete(Index);
end;

function _TStrList_GetItem(Strings: TStringList; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Strings.Strings[Index];
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TStrList_SetItem(Strings: TStringList; Index: Integer; S: PAnsiChar); stdcall;
begin
  Strings.Strings[Index] := S;
end;

function _TStrList_GetObject(Strings: TStringList; Index: Integer): TObject; stdcall;
begin
  Result := Strings.Objects[Index];
end;

procedure _TStrList_SetObject(Strings: TStringList; Index: Integer; AObject: TObject); stdcall;
begin
  Strings.Objects[Index] := AObject;
end;

function _TStrList_IndexOf(Strings: TStringList; S: PAnsiChar): Integer; stdcall;
begin
  Result := Strings.IndexOf(S);
end;

function _TStrList_IndexOfObject(Strings: TStringList; AObject: TObject): Integer; stdcall;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to Strings.Count - 1 do
  begin
    if Strings.Objects[I] = AObject then
    begin
      Result := I;
      Break;
    end;
  end;
end;

function _TStrList_Find(Strings: TStringList; S: PAnsiChar; var Index: Integer): BOOL; stdcall;
begin
  Result := Strings.Find(S, Index);
end;

procedure _TStrList_Exchange(Strings: TStringList; Index1, Index2: Integer); stdcall;
begin
  Strings.Exchange(Index1, Index2);
end;

{ 从文件载入 }
procedure _TStrList_LoadFromFile(Strings: _TStringList; FileName: PAnsiChar); stdcall;
begin
  Strings.LoadFromFile(FileName);
end;

{ 存到文件 }
procedure _TStrList_SaveToFile(Strings: _TStringList; FileName: PAnsiChar); stdcall;
begin
  Strings.SaveToFile(FileName);
end;

procedure _TStrList_CopyTo(Source, Dest: TStringList); stdcall;
begin
  Dest.Assign(Source);
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

function _TMemStream_Create(): TMemoryStream; stdcall;
begin
  Result := TMemoryStream.Create;
end;

procedure _TMemStream_Free(Stream: TMemoryStream); stdcall;
begin
  Stream.Free;
end;

function _TMemStream_GetSize(Stream: TMemoryStream): Int64; stdcall;
begin
  Result := Stream.Size;
end;

procedure _TMemStream_SetSize(Stream: TMemoryStream; NewSize: Integer); stdcall;
begin
  Stream.SetSize(NewSize);
end;

procedure _TMemStream_Clear(Stream: TMemoryStream); stdcall;
begin
  Stream.Clear;
end;

function _TMemStream_Read(Stream: TMemoryStream; Buffer: PAnsiChar; Count: Integer): Integer; stdcall;
begin
  Result := Stream.Read(Buffer^, Count);
end;

function _TMemStream_Write(Stream: TMemoryStream; Buffer: PAnsiChar; Count: Integer): Integer; stdcall;
begin
  Result := Stream.Write(Buffer^, Count);
end;

function _TMemStream_Seek(Stream: TMemoryStream; Offset: Integer; Origin: Word): Integer; stdcall;
begin
  Result := Stream.Seek(Offset, Origin);
end;

function _TMemStream_Memory(Stream: TMemoryStream): Pointer; stdcall;
begin
  Result := Stream.Memory;
end;

function _TMemStream_GetPosition(Stream: TMemoryStream): Int64; stdcall;
begin
  Result := Stream.Position;
end;

procedure _TMemStream_SetPosition(Stream: TMemoryStream; Position: Int64); stdcall;
begin
  Stream.Position := Position;
end;

procedure _TMemStream_LoadFromFile(Stream: TMemoryStream; FileName: PAnsiChar); stdcall;
begin
  Stream.LoadFromFile(FileName);
end;

procedure _TMemStream_SaveToFile(Stream: TMemoryStream; FileName: PAnsiChar); stdcall;
begin
  Stream.SaveToFile(FileName);
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 获取主菜单
function _TMenu_GetMainMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.MainMenu.Items;
end;

// 控制菜单
function _TMenu_GetControlMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniControl;
end;

// 查看菜单
function _TMenu_GetViewMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniView;
end;

// 选项菜单
function _TMenu_GetOptionMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniOption;
end;

// 管理菜单
function _TMenu_GetManagerMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniManger;
end;

// 工具菜单
function _TMenu_GetToolsMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniTools;
end;

// 帮助菜单
function _TMenu_GetHelpMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniHelp;
end;

// 插件菜单
function _TMenu_GetPluginMenu(): TMenuItem; stdcall;
begin
  Result := FrmMain.mniPlugin;
end;

// 子菜单数量
function _TMenu_Count(MenuItem: TMenuItem): Integer; stdcall;
begin
  Result := MenuItem.Count;
end;

// 获取某个子菜单
function _TMenu_GetItems(MenuItem: TMenuItem; Index: Integer): TMenuItem; stdcall;
begin
  Result := MenuItem.Items[Index];
end;

procedure NotifyEventEx(Sender: TObject);
begin
  if Sender = nil then
    Exit;
  PNotifyEventMethod(Sender).Click(PNotifyEventMethod(Sender).Sender);
end;

// 添加菜单
function _TMenu_Add(PlugID: NativeInt; MenuItem: TMenuItem; Caption: PAnsiChar; Tag: Integer; OnClick: TNotifyEventEx)
  : TMenuItem; stdcall;
var
  I: Integer;
  Plug, TempPlug: TPlugin;
  Item: TMenuItem;
  Method: TMethod;
  NotifyEventMethod: PNotifyEventMethod;
begin
  Plug := nil;
  for I := 0 to g_PluginManager.Count - 1 do
  begin
    TempPlug := g_PluginManager.Items[I];
    if NativeInt(TempPlug) = PlugID then
    begin
      Plug := TempPlug;
      Break;
    end;
  end;

  if Plug = nil then
  begin
    raise Exception.Create('PlugID error');
  end;

  Item := TMenuItem.Create(MenuItem);

  NotifyEventMethod := nil;
  if Assigned(OnClick) then
  begin
    New(NotifyEventMethod);
    NotifyEventMethod.Click := OnClick;
    NotifyEventMethod.Sender := Item;

    Method.Code := @NotifyEventEx;
    Method.Data := NotifyEventMethod;

    Item.OnClick := TNotifyEvent(Method);
  end;

  Item.Caption := Caption;
  Item.Tag := Tag;

  if MenuItem = nil then
  begin
    FrmMain.MainMenu.Items.Add(Item);
  end
  else
  begin
    MenuItem.Add(Item);
  end;

  Result := Item;

  // 插件把菜单记下
  Plug.AddMenu(Item, NotifyEventMethod);
end;

// 插入菜单
function _TMenu_Insert(PlugID: NativeInt; MenuItem: TMenuItem; Index: Integer; Caption: PAnsiChar; Tag: Integer;
  OnClick: TNotifyEventEx): TMenuItem; stdcall;
var
  I: Integer;
  Plug, TempPlug: TPlugin;
  Item: TMenuItem;
  Method: TMethod;
  NotifyEventMethod: PNotifyEventMethod;
begin
  Plug := nil;
  for I := 0 to g_PluginManager.Count - 1 do
  begin
    TempPlug := g_PluginManager.Items[I];
    if NativeInt(TempPlug) = PlugID then
    begin
      Plug := TempPlug;
      Break;
    end;
  end;

  if Plug = nil then
  begin
    raise Exception.Create('PlugID error');
  end;

  Item := TMenuItem.Create(FrmMain.MainMenu);

  NotifyEventMethod := nil;
  if Assigned(OnClick) then
  begin
    New(NotifyEventMethod);
    NotifyEventMethod.Click := OnClick;
    NotifyEventMethod.Sender := Item;

    Method.Code := @NotifyEventEx;
    Method.Data := NotifyEventMethod;

    Item.OnClick := TNotifyEvent(Method);
  end;

  Item.Caption := Caption;
  Item.Tag := Tag;

  if MenuItem = nil then
    FrmMain.MainMenu.Items.Insert(Index, Item)
  else
    MenuItem.Insert(Index, Item);

  Result := Item;

  // 插件把菜单记下
  Plug.AddMenu(Item, NotifyEventMethod);
end;

// 获取菜单标题
function _TMenu_GetCaption(MenuItem: TMenuItem; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := MenuItem.Caption;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 设置菜单标题
procedure _TMenu_SetCaption(MenuItem: TMenuItem; Caption: PAnsiChar); stdcall;
begin
  MenuItem.Caption := Caption;
end;

// 获取菜单可用
function _TMenu_GetEnabled(MenuItem: TMenuItem): BOOL; stdcall;
begin
  Result := MenuItem.Enabled;
end;

// 设置菜单可用
procedure _TMenu_SetEnabled(MenuItem: TMenuItem; Enabled: BOOL); stdcall;
begin
  MenuItem.Enabled := Enabled;
end;

// 获取菜单可见
function _TMenu_GetVisable(MenuItem: TMenuItem): BOOL; stdcall;
begin
  Result := MenuItem.Visible;
end;

// 设置菜单可见
procedure _TMenu_SetVisable(MenuItem: TMenuItem; Visible: BOOL); stdcall;
begin
  MenuItem.Visible := Visible;
end;

// 获取菜单选中状态
function _TMenu_GetChecked(MenuItem: TMenuItem): BOOL; stdcall;
begin
  Result := MenuItem.Checked;
end;

// 设置菜单选中状态
procedure _TMenu_SetChecked(MenuItem: TMenuItem; Checked: BOOL); stdcall;
begin
  MenuItem.Checked := Checked;
end;

// 获取菜单是否为单选
function _TMenu_GetRadioItem(MenuItem: TMenuItem): BOOL; stdcall;
begin
  Result := MenuItem.RadioItem;
end;

// 设置菜单是否为单选
procedure _TMenu_SetRadioItem(MenuItem: TMenuItem; IsRadioItem: BOOL); stdcall;
begin
  MenuItem.RadioItem := IsRadioItem;
end;

// 获取菜单单选分组
function _TMenu_GetGroupIndex(MenuItem: TMenuItem): Integer; stdcall;
begin
  Result := MenuItem.GroupIndex;
end;

// 设置菜单单选分组
procedure _TMenu_SetGroupIndex(MenuItem: TMenuItem; Value: Integer); stdcall;
begin
  MenuItem.GroupIndex := Value;
end;

// 获取附加值
function _TMenu_GetTag(MenuItem: TMenuItem): Integer; stdcall;
begin
  Result := MenuItem.Tag;
end;

// 设置附加值
procedure _TMenu_SetTag(MenuItem: TMenuItem; Value: Integer); stdcall;
begin
  MenuItem.Tag := Value;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
function _TIniFile_Create(sFileName: PAnsiChar): TIniFile; stdcall;
begin
  Result := TIniFile.Create(sFileName);
end;

procedure _TIniFile_Free(IniFile: TIniFile); stdcall;
begin
  IniFile.Free;
end;

function _TIniFile_SectionExists(IniFile: TIniFile; Section: PAnsiChar): BOOL; stdcall;
begin
  Result := IniFile.SectionExists(Section);
end;

function _TIniFile_ValueExists(IniFile: TIniFile; Section, Ident: PAnsiChar): BOOL; stdcall;
begin
  Result := IniFile.ValueExists(Section, Ident);
end;

function _TIniFile_ReadString(IniFile: TIniFile; Section, Ident, Default: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD)
  : BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := IniFile.ReadString(Section, Ident, Default);
  if (Dest <> nil) and (DestLen > Length(S)) and (Length(S) > 0) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TIniFile_WriteString(IniFile: TIniFile; Section, Ident, Value: PAnsiChar); stdcall;
begin
  IniFile.WriteString(Section, Ident, Value);
end;

function _TIniFile_ReadInteger(IniFile: TIniFile; Section, Ident: PAnsiChar; Default: Integer): Integer; stdcall;
begin
  Result := IniFile.ReadInteger(Section, Ident, Default);
end;

procedure _TIniFile_WriteInteger(IniFile: TIniFile; Section, Ident: PAnsiChar; Value: Integer); stdcall;
begin
  IniFile.WriteInteger(Section, Ident, Value);
end;

function _TIniFile_ReadBool(IniFile: TIniFile; Section, Ident: PAnsiChar; Default: BOOL): BOOL; stdcall;
begin
  Result := IniFile.ReadBool(Section, Ident, Default);
end;

procedure _TIniFile_WriteBool(IniFile: TIniFile; Section, Ident: PAnsiChar; Value: BOOL); stdcall;
begin
  IniFile.WriteBool(Section, Ident, Value);
end;


// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 根据地图名得到地图对象
function _TMapManager_FindMap(MapName: PAnsiChar): TEnvirnoment; stdcall;
begin
  Result := g_MapManager.FindMap(MapName);
end;

// 获取地图对象列表 (GetItem -> TEnvirnoment);
function _TMapManager_GetMapList(): TList; stdcall;
begin
  Result := g_MapManager;
end;


// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 地图名称
function _TEnvir_GetMapName(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Envir.sMapName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 地图描述
function _TEnvir_GetMapDesc(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Envir.sMapDesc;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 地图宽度
function _TEnvir_GetWidth(Envir: TEnvirnoment): Integer; stdcall;
begin
  Result := Envir.m_nWidth;
end;

// 地图高度
function _TEnvir_GetHeight(Envir: TEnvirnoment): Integer; stdcall;
begin
  Result := Envir.m_nHeight;
end;

// 小地图
function _TEnvir_GetMinMap(Envir: TEnvirnoment): Integer; stdcall;
begin
  Result := Envir.nMinMap;
end;

// 是否主地图
function _TEnvir_IsMainMap(Envir: TEnvirnoment): BOOL; stdcall;
begin
  Result := Envir.m_boMainMap;
end;

// 主地图名
function _TEnvir_GetMainMapName(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Envir.sMainMapName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 是否动态镜像地图
function _TEnvir_IsMirrMap(Envir: TEnvirnoment): BOOL; stdcall;
begin
  Result := Envir.m_boMirror;
end;

// 动态镜像地图创建时间
function _TEnvir_GetMirrMapCreateTick(Envir: TEnvirnoment): DWORD; stdcall;
begin
  Result := Envir.m_dwMirrorCreateTick;
end;

// 动态镜像地图存活时间
function _TEnvir_GetMirrMapSurvivalTime(Envir: TEnvirnoment): DWORD; stdcall;
begin
  Result := Envir.m_dwMirrorSurvivalTime;
end;

// 动态地图退到哪个地图
function _TEnvir_GetMirrMapExitToMap(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Envir.m_sMirrorExitToMap;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 动态镜像地图小地图编号
function _TEnvir_GetMirrMapMinMap(Envir: TEnvirnoment): Integer; stdcall;
begin
  Result := Envir.m_nMirrorMinMap;
end;

// 动态镜像地图是否一直显示时间
function _TEnvir_GetAlwaysShowTime(Envir: TEnvirnoment): BOOL; stdcall;
begin
  Result := Envir.m_boAlwaysShowTime;
end;

// 是否副本地图
function _TEnvir_IsFBMap(Envir: TEnvirnoment): BOOL; stdcall;
begin
  Result := Envir.m_boFB;
end;

// 副本地图名
function _TEnvir_GetFBMapName(Envir: TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Envir.m_sFBName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 副本进入限制
function _TEnvir_GetFBEnterLimit(Envir: TEnvirnoment): Integer; stdcall;
begin
  Result := Integer(Envir.m_FBEnterLimit);
end;

// 副本地图是否创建
function _TEnvir_GetFBCreated(Envir: TEnvirnoment): BOOL; stdcall;
begin
  Result := Envir.m_boFBCreate;
end;

// 副本地图创建时间
function _TEnvir_GetFBCreateTime(Envir: TEnvirnoment): DWORD; stdcall;
begin
  Result := Envir.m_dwFBCreateTime;
end;

// 获取地图是否设置某参数
// 如地图有参数: FIGHT4，调用 GetMapParam(Envir, 'FIGHT4')，则返回True
// 如地图有参数：INCGAMEGOLD(1/10)，调用 GetMapParam(Envir, 'INCGAMEGOLD')，则返回True
function _TEnvir_GetMapParam(Envir: TEnvirnoment; Param: PAnsiChar): BOOL; stdcall;
begin
  Result := False;
end;

// 获取地图设置某参数值
// 如地图有参数：INCGAMEGOLD(1/10)，调用 GetMapParam(Envir, 'INCGAMEGOLD')，则返回1/10
function _TEnvir_GetMapParamValue(Envir: TEnvirnoment; Param: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
begin
  Result := False;
end;

// 地图点是否可达，boFlag = False时，会判断该坐标点是否有角色占据
function _TEnvir_CheckCanMove(Envir: TEnvirnoment; nX, nY: Integer; boFlag: BOOL): BOOL; stdcall;
begin
  Result := Envir.CanWalk(nX, nY, boFlag);
end;

// 判断地图上以坐标(nX, nY)为空中，以nRange为半径的矩形范围内，是否有Obj对象
function _TEnvir_IsValidObject(Envir: TEnvirnoment; nX, nY, nRange: Integer; AObject: TObject): BOOL; stdcall;
begin
  Result := Envir.IsValidObject(nX, nY, nRange, AObject);
end;

// 获取地图上某坐标的物品列表
function _TEnvir_GetItemObjects(Envir: TEnvirnoment; nX, nY: Integer; ObjectList: TList): Integer; stdcall;
begin
  Result := Envir.GeTItemObjects(nX, nY, ObjectList);
end;

// 获取地图上某坐标的角色列表
function _TEnvir_GetBaseObjects(Envir: TEnvirnoment; nX, nY: Integer; IncDeathObject: BOOL; ObjectList: TList): Integer; stdcall;
begin
  Result := Envir.GeTBaseObjects(nX, nY, IncDeathObject, ObjectList);
end;

// 获取地图上某坐标的人物列表
function _TEnvir_GetPlayObjects(Envir: TEnvirnoment; nX, nY: Integer; IncDeathObject: BOOL; ObjectList: TList): Integer; stdcall;
begin
  Result := Envir.GetPlayObjects(nX, nY, IncDeathObject, ObjectList);
end;

function _TM2Engine_GetVersion(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := g_version;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

function _TM2Engine_GetVersionInt(): Integer; stdcall;
var
  Y, M, D: Word;
begin
  DecodeDate(g_buildtime, Y, M, D);
  Result := Y * 10000 + M * 100 + D;
end;

function _TM2Engine_GetMainFormHandle(): THandle; stdcall;
begin
  Result := FrmMain.Handle;
end;

procedure _TM2Engine_SetMainFormCaption(Caption: PAnsiChar); stdcall;
begin
  sCaptionExtText := Caption;
end;

// 主程序目录
function _TM2Engine_GetAppDir(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := ExtractFilePath(ParamStr(0));
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

function _TM2Engine_GetGlobalIniFile(M2IniType: Integer): TIniFile; stdcall;
begin
  Result := nil;
  case M2IniType of
    0:
      Result := Config; // !Setup.txt
    1:
      Result := StringConf; // AnsiString.ini
  end;
end;

function _TM2Engine_GetOtherFileDir(M2FileType: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;

  case M2FileType of
    00:
      S := g_Config.sEnvirDir; // Envir目录
    01:
      S := g_Config.sPlugDir; // 插件目录
    02:
      S := g_Config.sGuildDir; // 行会目录
    03:
      S := g_Config.sGuildFile; // 行会文件
    04:
      S := g_Config.sCastleDir; // 城堡目录
    05:
      S := g_Config.sCastleFile; // 城堡文件
    06:
      S := g_Config.sMapDir; // 地图目录
    07:
      S := g_Config.sNoticeDir; // 公告目录
    08:
      S := g_Config.sBoxsDir; // 宝箱目录
    09:
      S := g_Config.sBoxsFile; // 宝箱文件
    10:
      S := g_Config.sSmartMonsterDir; // 自定义怪物目录
    11:
      S := g_Config.sCustomMagicDir; // 自定义技能目录
    12:
      S := g_Config.sSmartNpcDir; // 自定义NPC目录
    13:
      S := g_Config.sItemDropLimit; // 物品掉落规则目录
    14:
      S := g_Config.sItemDropLogDir; // 物品掉落规则日志目录
    15:
      S := g_Config.sConLogDir; // 登录日志目录
  end;

  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TM2Engine_MainOutMessage(Msg: PAnsiChar; IsAddTime: BOOL); stdcall;
begin
  MainOutMessage(Msg, True {IsAddTime});
end;

// 读取全局I变量
function _TM2Engine_GetGlobalVarI(Index: Integer): Integer; stdcall;
begin
  Result := g_Config.GlobaDyMval[Index];
end;

// 设置全局I变量
function _TM2Engine_SetGlobalVarI(Index: Integer; Value: Integer): BOOL; stdcall;
begin
  g_Config.GlobaDyMval[Index] := Value;
  Result := True;
end;

// 读取全局G变量
function _TM2Engine_GetGlobalVarG(Index: Integer): Integer; stdcall;
begin
  Result := g_Config.GlobalVal[Index];
end;

// 设置全局G变量
function _TM2Engine_SetGlobalVarG(Index: Integer; Value: Integer): BOOL; stdcall;
begin
  g_Config.GlobalVal[Index] := Value;
  Result := True;
end;

// 读取全局A变量
function _TM2Engine_GetGlobalVarA(Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := g_Config.GlobalAVal[Index];
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 设置全局A变量
function _TM2Engine_SetGlobalVarA(Index: Integer; Value: PAnsiChar): BOOL; stdcall;
begin
  g_Config.GlobalAVal[Index] := Value;
  Result := True;
end;

function _TM2Engine_EncodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  Len: DWORD;
begin
  Result := False;
  Len := GetEncodeSize(SrcLen);
  if (Dest <> nil) and (DestLen > Len) then
  begin
    Len := Encode6BitBuf(Src, Dest, SrcLen, DestLen);
    FillChar(PAnsiChar(NativeInt(Dest) + Len)^, 1, 0);
    Result := True;
  end;

  DestLen := Len;
end;

function _TM2Engine_DecodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  Len: DWORD;
begin
  Result := False;
  Len := GetDecodeSize(SrcLen);
  if (Dest <> nil) and (DestLen >= Len) then
  begin
    Len := Decode6BitBuf(Src, Dest, SrcLen, DestLen);
    Result := True;
  end;
  DestLen := Len;
end;

function _TM2Engine_ZLibEncodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
resourcestring
  OutMem = 'TM2Engine_ZLibEncodeBuffer out of Memory';
var
  pDest: PAnsiChar;
  nMemLen, nDestLen: DWORD;
begin
  Result := False;
  nMemLen := SrcLen * 2;
  try
    GetMem(pDest, nMemLen);
    nDestLen := zLibEncodeBuffer(Src, SrcLen, pDest, nMemLen);
    if nDestLen > 0 then
    begin
      if (Dest <> nil) and (nDestLen > DestLen) then
      begin
        Move(pDest^, Dest^, nDestLen);
        FillChar(PAnsiChar(NativeInt(Dest) + nDestLen)^, 1, 0);
        Result := True;
      end;

      DestLen := nDestLen;
    end;

    FreeMem(pDest, nMemLen);
  except
    MainOutMessage(OutMem, True);
  end;
end;

function _TM2Engine_ZLibDecodeBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
resourcestring
  OutMem = 'TM2Engine_ZLibDecodeBuffer out of Memory';
var
  pDest: PAnsiChar;
  nMemLen, nDestLen: DWORD;
begin
  Result := False;
  nMemLen := SrcLen * 3;
  try
    GetMem(pDest, nMemLen);
    nDestLen := zLibDecodeBuffer(Src, SrcLen, pDest, nMemLen);
    if nDestLen > 0 then
    begin
      if (Dest <> nil) and (nDestLen >= DestLen) then
      begin
        Move(pDest^, Dest^, nDestLen);
        Result := True;
      end;

      DestLen := nDestLen;
    end;

    FreeMem(pDest, nMemLen);
  except
    MainOutMessage(OutMem, True);
  end;
end;

function _TM2Engine_EncryptBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := EncryBufferA_LF(Src, SrcLen);
  if (Dest <> nil) and (DestLen >= Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    Result := True;
  end;
  DestLen := Length(S);
end;

function _TM2Engine_DecryptBuffer(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := DecryBufferA_LF(Src, SrcLen);
  if (Dest <> nil) and (DestLen >= Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    Result := True;
  end;
  DestLen := Length(S);
end;

const
  FIXED_PASSWORD: AnsiString = 'MIR_MIR_MIR_MIR';

function _TM2Engine_EncryptPassword(InData: PAnsiChar; OutData: PAnsiChar; var OutSize: DWORD): BOOL; stdcall;
var
  S1: AnsiString;
begin
  Result := False;
  S1 := AnsiString(InData);
  EncryptDes_New(S1[1], S1[1], Length(S1), FIXED_PASSWORD);
  S1 := EncodeString(S1);

  if (OutData <> nil) and (OutSize >= Length(S1)) then
  begin
    Move(S1[1], OutData^, Length(S1));
    Result := True;
    OutSize := Length(S1);
  end
  else
  begin
    OutSize := 0;
  end;
end;

function _TM2Engine_DecryptPassword(InData: PAnsiChar; OutData: PAnsiChar; var OutSize: DWORD): BOOL; stdcall;
var
  S1: AnsiString;
begin
  Result := False;
  S1 := DecodeString(AnsiString(InData));
  DecryptDes_New(S1[1], S1[1], Length(S1), FIXED_PASSWORD);
  if (OutData <> nil) and (OutSize >= Length(S1)) then
  begin
    Move(S1[1], OutData^, Length(S1));
    Result := True;
    OutSize := Length(S1);
  end
  else
  begin
    OutSize := 0;
  end;
end;

// 根据物品StdMode得到物品装备位置
function _TM2Engine_GetTakeOnPosition(StdMode: Integer): Integer; stdcall;
begin
  Result := GetTakeOnPosition(StdMode);
end;

function _TM2Engine_CheckBindType(BindValue: Byte; BindType: Byte): BOOL; stdcall;
begin
  Result := GetUserItemBindValue(BindValue, TUserItemBindValueType(BindType));
end;

procedure _TM2Engine_SetBindValue(var BindValue: Byte; BindType: Byte; Value: BOOL); stdcall;
begin
  SetUserItemBindValue(BindValue, TUserItemBindValueType(BindType), Value);
end;

function _TM2Engine_GetRGB(Color: Byte): DWORD; stdcall;
begin
  Result := GetRGB(Color);
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 名称
function _TBaseObject_GetChrName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := BaseObject.m_sCharName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 设置名称(不能设置人物、英雄)
function _TBaseObject_SetChrName(BaseObject: TBaseObject; NewName: PAnsiChar): BOOL; stdcall;
begin
  Result := False;
  if not(BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
  begin
    BaseObject.m_sCharName := NewName;
    Result := True;
  end;
end;

// 刷新名称到客户端
procedure _TBaseObject_RefShowName(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.RefShowName;
end;

// 刷新名称颜色 PKPoint等改变时
procedure _TBaseObject_RefNameColor(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.RefNameColor;
end;

// 获取性别
function _TBaseObject_GetGender(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btGender;
end;

// 设置性别
function _TBaseObject_SetGender(BaseObject: TBaseObject; Gender: Byte): BOOL; stdcall;
begin
  Result := False;
  if Gender in [0, 1] then
  begin
    BaseObject.m_btGender := Gender;
    Result := True;
  end;
end;

// 获取职业
function _TBaseObject_GetJob(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btJob;
end;

// 设置职业
function _TBaseObject_SetJob(BaseObject: TBaseObject; Job: Byte): BOOL; stdcall;
begin
  Result := False;
  if Job in [0 .. 2] then
  begin
    BaseObject.m_btJob := Job;
    Result := True;
  end;
end;

// 获取发型
function _TBaseObject_GetHair(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btHair;
end;

// 设置发型
procedure _TBaseObject_SetHair(BaseObject: TBaseObject; Hair: Byte); stdcall;
begin
  BaseObject.m_btHair := Hair;
end;

// 所在地图
function _TBaseObject_GetEnvir(BaseObject: TBaseObject): TEnvirnoment; stdcall;
begin
  Result := BaseObject.m_PEnvir;
end;

// 所在地图
function _TBaseObject_GetMapName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := BaseObject.m_sMapName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 坐标X
function _TBaseObject_GetCurrX(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nCurrX;
end;

// 坐标Y
function _TBaseObject_GetCurrY(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nCurrY;
end;

// 当前方向
function _TBaseObject_GetDirection(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btDirection;
end;

// 回城地图
function _TBaseObject_GetHomeMap(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := BaseObject.m_sHomeMap;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 回城坐标X
function _TBaseObject_GetHomeX(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nHomeX;
end;

// 回城坐标Y
function _TBaseObject_GetHomeY(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nHomeY;
end;

// 权限等级
function _TBaseObject_GetPermission(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btPermission;
end;

procedure _TBaseObject_SetPermission(BaseObject: TBaseObject; Value: Byte); stdcall;
resourcestring
  sOutFormatMsg = '[权限调整-插件] %s (%d -> %d)';
begin
  if g_Config.boPermissionChangeLog then
    MainOutMessage(Format(sOutFormatMsg, [BaseObject.m_sCharName, BaseObject.m_btPermission, Value]));

  BaseObject.m_btPermission := Value;
end;

// 是否死亡
function _TBaseObject_GetDeath(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boDeath;
end;

// 死亡时间
function _TBaseObject_GetDeathTick(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_dwDeathTick;
end;

// 是否死亡并清理
function _TBaseObject_GetGhost(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boGhost;
end;

// 清理时间
function _TBaseObject_GetGhostTick(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_dwGhostTick;
end;

// 杀死
procedure _TBaseObject_MakeGhost(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.MakeGhost;
end;

// 复活
procedure _TBaseObject_ReAlive(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.ReAlive(False);
end;

// 类型
function _TBaseObject_GetRaceServer(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btRaceServer;
end;

// Appr
function _TBaseObject_GetAppr(BaseObject: TBaseObject): Word; stdcall;
begin
  Result := BaseObject.m_wAppr;
end;

// RaceImg
function _TBaseObject_GetRaceImg(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btRaceImg;
end;

// 状态
function _TBaseObject_GetCharStatus(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nCharStatus;
end;

procedure _TBaseObject_SetCharStatus(BaseObject: TBaseObject; Value: Integer); stdcall;
begin
  BaseObject.m_nCharStatus := Value;
end;

// 状态改变
procedure _TBaseObject_StatusChanged(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.StatusChanged;
end;

// 饥饿点
function _TBaseObject_GetHungerPoint(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nHungerStatus;
end;

procedure _TBaseObject_SetHungerPoint(BaseObject: TBaseObject; Value: Integer); stdcall;
begin
  BaseObject.m_nHungerStatus := Value;
end;

// 是否为内功怪
function _TBaseobject_IsNGMonster(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boISNGMonster;
end;

// 是否假人
function _TBaseObject_IsDummyObject(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boDummyObject;
end;

// 获取视觉范围
function _TBaseObject_GetViewRange(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nViewRange;
end;

// 设置视觉范围
procedure _TBaseObject_SetViewRange(BaseObject: TBaseObject; Value: Integer); stdcall;
begin
  BaseObject.m_nViewRange := Value;
end;

// 原始属性
function _TBaseObject_GetAbility(BaseObject: _TBaseObject; Dest: pTAbility): BOOL; stdcall;
begin
  Dest^ := BaseObject.m_Abil;
  Result := True;
end;

// 最终属性
function _TBaseObject_GetWAbility(BaseObject: _TBaseObject; Dest: pTAbility): BOOL; stdcall;
begin
  Dest^ := BaseObject.m_WAbil;
  Result := True;
end;

procedure _TBaseObject_SetWAbility(BaseObject: TBaseObject; Value: pTAbility); stdcall;
begin
  BaseObject.m_WAbil := Value^;
end;

// 宝宝列表
function _TBaseObject_GetSlaveList(BaseObject: TBaseObject): TList; stdcall;
begin
  Result := BaseObject.m_SlaveList;
end;

// 主人
function _TBaseObject_GetMaster(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.m_Master;
end;

// 最上层主人
function _TBaseObject_GetMasterEx(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.Master;
end;

// 无敌模式
function _TBaseObject_GetSuperManMode(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boSuperMan;
end;

procedure _TBaseObject_SetSuperManMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boSuperMan := Value;
end;

// 管理模式
function _TBaseObject_GetAdminMode(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boAdminMode;
end;

procedure _TBaseObject_SetAdminMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boAdminMode := Value;
end;

// 魔法隐身
function _TBaseObject_GetTransparent(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boTransparent;
end;

procedure _TBaseObject_SetTransparent(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boTransparent := Value;
end;

// 隐身模式
function _TBaseObject_GetObMode(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boObMode;
end;

procedure _TBaseObject_SetObMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boObMode := Value;
end;

// 石像化模式
function _TBaseObject_GetStoneMode(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boStoneMode;
end;

procedure _TBaseObject_SetStoneMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boStoneMode := Value;
end;

// 不能推动
function _TBaseObject_GetStickMode(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boStickMode;
end;

procedure _TBaseObject_SetStickMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boStickMode := Value;
end;

// 怪物是否可挖
function _TBaseObject_GetIsAnimal(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boAnimal;
end;

procedure _TBaseObject_SetIsAnimal(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boAnimal := Value;
end;

// 死亡是否不掉装备(True: 不掉; False:掉)
function _TBaseObject_GetIsNoItem(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boNoItem;
end;

procedure _TBaseObject_SetIsNoItem(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boNoItem := Value;
end;

// 隐身免疫
function _TBaseObject_GetCoolEye(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boCoolEye;
end;

procedure _TBaseObject_SetCoolEye(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boCoolEye := Value;
end;

// 命中
function _TBaseObject_GetHitPoint(BaseObject: TBaseObject): Word; stdcall;
begin
  Result := BaseObject.m_btHitPoint;
end;

procedure _TBaseObject_SetHitPoint(BaseObject: TBaseObject; Value: Word); stdcall;
begin
  BaseObject.m_btHitPoint := Value;
end;

// 敏捷
function _TBaseObject_GetSpeedPoint(BaseObject: TBaseObject): Word; stdcall;
begin
  Result := BaseObject.m_btSpeedPoint;
end;

procedure _TBaseObject_SetSpeedPoint(BaseObject: TBaseObject; Value: Word); stdcall;
begin
  BaseObject.m_btSpeedPoint := Value;
end;

// 攻击速度
function _TBaseObject_GetHitSpeed(BaseObject: TBaseObject): ShortInt; stdcall;
begin
  Result := BaseObject.m_nHitSpeed;
end;

procedure _TBaseObject_SetHitSpeed(BaseObject: TBaseObject; Value: ShortInt); stdcall;
begin
  BaseObject.m_nHitSpeed := Value;
end;

// 移动速度
function _TBaseObject_GetWalkSpeed(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nWalkSpeed;
end;

procedure _TBaseObject_SetWalkSpeed(BaseObject: TBaseObject; Value: Integer); stdcall;
begin
  BaseObject.m_nWalkSpeed := Value;
end;

// HP恢复速度
function _TBaseObject_GetHPRecover(BaseObject: TBaseObject): ShortInt; stdcall;
begin
  Result := BaseObject.m_nHealthRecover;
end;

procedure _TBaseObject_SetHPRecover(BaseObject: TBaseObject; Value: ShortInt); stdcall;
begin
  BaseObject.m_nHealthRecover := Value;
end;

// MP恢复速度
function _TBaseObject_GetMPRecover(BaseObject: TBaseObject): ShortInt; stdcall;
begin
  Result := BaseObject.m_nSpellRecover;
end;

procedure _TBaseObject_SetMPRecover(BaseObject: TBaseObject; Value: ShortInt); stdcall;
begin
  BaseObject.m_nSpellRecover := Value;
end;

// 中毒恢复
function _TBaseObject_GetPoisonRecover(BaseObject: TBaseObject): ShortInt; stdcall;
begin
  Result := BaseObject.m_nPoisonRecover;
end;

procedure _TBaseObject_SetPoisonRecover(BaseObject: TBaseObject; Value: ShortInt); stdcall;
begin
  BaseObject.m_nPoisonRecover := Value;
end;

// 毒躲避
function _TBaseObject_GetAntiPoison(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btAntiPoison;
end;

procedure _TBaseObject_SetAntiPoison(BaseObject: TBaseObject; Value: Byte); stdcall;
begin
  BaseObject.m_btAntiPoison := Value;
end;

// 魔法躲避
function _TBaseObject_GetAntiMagic(BaseObject: TBaseObject): ShortInt; stdcall;
begin
  Result := BaseObject.m_nAntiMagic;
end;

procedure _TBaseObject_SetAntiMagic(BaseObject: TBaseObject; Value: ShortInt); stdcall;
begin
  BaseObject.m_nAntiMagic := Value;
end;

// 幸运
function _TBaseObject_GetLuck(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nLuck;
end;

procedure _TBaseObject_SetLuck(BaseObject: TBaseObject; Value: Integer); stdcall;
begin
  BaseObject.m_nLuck := Value;
end;

// 攻击模式
function _TBaseObject_GetAttatckMode(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btAttatckMode;
end;

procedure _TBaseObject_SetAttatckMode(BaseObject: TBaseObject; Value: Byte); stdcall;
begin
  BaseObject.m_btAttatckMode := Value;
end;

// 获取所属国家
function _TBaseObject_GetNation(BaseObject: TBaseObject): Byte; stdcall;
begin
  Result := BaseObject.m_btNation;
end;

// 设置所属国家
function _TBaseObject_SetNation(BaseObject: TBaseObject; Nation: Byte): BOOL; stdcall;
var
  NationInfo: pTNationInfo;
begin
  Result := False;

  if BaseObject.m_btNation = Nation then
  begin
    Result := True;
    Exit;
  end;

  NationInfo := g_NationManage.Items[Nation];
  if NationInfo <> nil then
  begin
    if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
    begin
      g_NationManage.DeleteMember(TPlayObject(BaseObject));

      Inc(NationInfo.nPeoples);
      BaseObject.m_btNation := Nation;
      BaseObject.m_sNationaName := NationInfo.sName;
      g_NationManage.AddMember(TPlayObject(BaseObject));
      g_NationManage.SaveConfig(BaseObject.m_btNation);
    end
    else
    begin
      BaseObject.m_btNation := Nation;
      BaseObject.m_sNationaName := NationInfo.sName;
    end;

    Result := True;
  end;
end;

// 国家名字
function _TBaseObject_GetNationaName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := BaseObject.m_sNationaName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 行会
function _TBaseObject_GetGuild(BaseObject: TBaseObject): TGUild; stdcall;
begin
  if BaseObject.m_MyGuild = nil then
    Result := nil
  else
    Result := TGUild(BaseObject.m_MyGuild);
end;

function _TBaseobject_GetGuildRankNo(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.m_nGuildRankNo;
end;

function _TBaseobject_GetGuildRankName(BaseObject: TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := BaseObject.m_sGuildRankName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 是否为行会老大
function _TBaseObject_IsGuildMaster(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.IsGuildMaster;
end;

// 隐身戒指 特殊物品:111
function _TBaseObject_GetHideMode(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boHideMode;
end;

procedure _TBaseObject_SetHideMode(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boHideMode := Value;
end;

// 麻痹戒指  特殊物品:113
function _TBaseObject_GetIsParalysis(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boParalysis;
end;

procedure _TBaseObject_SetIsParalysis(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boParalysis := Value;
end;

// 麻痹几率
function _TBaseObject_GetParalysisRate(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_dwParalysisRate;
end;

procedure _TBaseObject_SetParalysisRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_dwParalysisRate := Value;
end;

// 魔道麻痹戒指  特殊物品:204
function _TBaseObject_GetIsMDParalysis(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boMDParalysis;
end;

procedure _TBaseObject_SetIsMDParalysis(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boMDParalysis := Value;
end;

// 魔道麻痹几率
function _TBaseObject_GetMDParalysisRate(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_dwMDParalysisRate;
end;

procedure _TBaseObject_SetMDParalysisRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_dwMDParalysisRate := Value;
end;

// 冰冻戒指  特殊物品:205
function _TBaseObject_GetIsFrozen(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boFrozen;
end;

procedure _TBaseObject_SetIsFrozen(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boFrozen := Value;
end;

// 冰冻几率
function _TBaseObject_GetFrozenRate(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_dwFrozenRate;
end;

procedure _TBaseObject_SetFrozenRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_dwFrozenRate := Value;
end;

// 蛛网戒指  特殊物品:206
function _TBaseObject_GetIsCobwebWinding(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.m_boCobwebWinding;
end;

procedure _TBaseObject_SetIsCobwebWinding(BaseObject: TBaseObject; Value: BOOL); stdcall;
begin
  BaseObject.m_boCobwebWinding := Value;
end;

// 蛛网几率
function _TBaseObject_GetCobwebWindingRate(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_dwCobwebWindingRate;
end;

procedure _TBaseObject_SetCobwebWindingRate(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_dwCobwebWindingRate := Value;
end;

// 防麻几率
function _TBaseObject_GetUnParalysisValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[13];
end;

procedure _TBaseObject_SetUnParalysisValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[13] := Value;
end;

// 根据几率取当次是否防麻
function _TBaseObject_GetIsUnParalysis(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[13];
end;

// 防护身几率
function _TBaseObject_GetUnMagicShieldValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[14];
end;

procedure _TBaseObject_SetUnMagicShieldValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[14] := Value;
end;

// 根据几率取当次是否防护身
function _TBaseObject_GetIsUnMagicShield(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[14];
end;

// 防复活几率
function _TBaseObject_GetUnRevivalValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[15];
end;

procedure _TBaseObject_SetUnRevivalValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[15] := Value;
end;

// 根据几率取当次是否防复活
function _TBaseObject_GetIsUnRevival(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[15];
end;

// 防毒几率
function _TBaseObject_GetUnPosionValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[16];
end;

procedure _TBaseObject_SetUnPosionValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[16] := Value;
end;

// 根据几率取当次是否防毒
function _TBaseObject_GetIsUnPosion(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[16];
end;

// 防诱惑几率
function _TBaseObject_GetUnTammingValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[17];
end;

procedure _TBaseObject_SetUnTammingValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[17] := Value;
end;

// 根据几率取当次是否防诱惑
function _TBaseObject_GetIsUnTamming(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[17];
end;

// 防火墙几率
function _TBaseObject_GetUnFireCrossValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[18];
end;

procedure _TBaseObject_SetUnFireCrossValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[18] := Value;
end;

// 根据几率取当次是否防火墙
function _TBaseObject_GetIsUnFireCross(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[18];
end;

// 防冰冻几率
function _TBaseObject_GetUnFrozenValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[19];
end;

procedure _TBaseObject_SetUnFrozenValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[19] := Value;
end;

// 根据几率取当次是否防冰冻
function _TBaseObject_GetIsUnFrozen(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[19];
end;

// 防蛛网几率
function _TBaseObject_GetUnCobwebWindingValue(BaseObject: TBaseObject): DWORD; stdcall;
begin
  Result := BaseObject.m_WAbil.NewValue[20];
end;

procedure _TBaseObject_SetUnCobwebWindingValue(BaseObject: TBaseObject; Value: DWORD); stdcall;
begin
  BaseObject.m_WAbil.NewValue[20] := Value;
end;

// 根据几率取当次是否防蛛网
function _TBaseObject_GetIsUnCobwebWinding(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := Random(100) < BaseObject.m_WAbil.NewValue[20];
end;

// 获取当前攻击目标
function _TBaseObject_GetTargetCret(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.m_TargetCret;
end;

// 设置当前攻击目标
procedure _TBaseObject_SetTargetCret(BaseObject: TBaseObject; TargetCret: TBaseObject); stdcall;
begin
  BaseObject.SetTargetCreat(TargetCret);
end;

// 删除当前攻击目标
procedure _TBaseObject_DelTargetCreat(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.DelTargetCreat;
end;

// 被谁攻击
function _TBaseObject_GetLastHiter(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.m_LastHiter;
end;

// 谁得经验
function _TBaseObject_GetExpHitter(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.m_ExpHitter;
end;

// 施毒者
function _TBaseObject_GetPoisonHitter(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.m_PoisonHitter;
end;

// 面前的对象是谁
function _TBaseObject_GetPoseCreate(BaseObject: TBaseObject): TBaseObject; stdcall;
begin
  Result := BaseObject.GetPoseCreate;
end;

{
  function _TBaseObject_IsProtectTarget(AObject, BaseObject: TBaseObject): BOOL; stdcall;
  begin

  end;

  function _TBaseObject_IsAttackTarget(AObject, BaseObject: TBaseObject): BOOL; stdcall;       // 是否为攻击目标
  begin

  end;
}

// 是否为攻击目标(建议用这个)
function _TBaseObject_IsProperTarget(BaseObject, Target: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.IsProtectTarget(BaseObject);
end;

// 是否为朋友
function _TBaseObject_IsProperFriend(BaseObject, Target: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.IsProperFriend(Target);
end;

// 判断对象在指定范围内
function _TBaseObject_TargetInRange(BaseObject, Target: TBaseObject; nX, nY, nRange: Integer): BOOL; stdcall;
begin
  Result := BaseObject.CretInNearXY(Target, nX, nY, nRange);
end;

// 发消息
procedure _TBaseObject_SendMsg(BaseObject, Target: TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt;
  sMsg: PAnsiChar); stdcall;
begin
  BaseObject.SendMsg(Target, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
end;

// 发延时消息
procedure _TBaseObject_SendDelayMsg(BaseObject, Target: TBaseObject; wIdent, wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar; dwDelay: DWORD); stdcall;
begin
  BaseObject.SendDelayMsg(Target, wIdent, wParam, nParam1, nParam2, nParam3, sMsg, dwDelay);
end;

// 向全屏玩家发消息
procedure _TBaseObject_SendRefMsg(BaseObject: TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt;
  sMsg: PAnsiChar; dwDelay: DWORD); stdcall;
begin
  BaseObject.SendRefMsg(wIdent, wParam, nParam1, nParam2, nParam3, sMsg, dwDelay);
end;

// 更新发消息
procedure _TBaseObject_SendUpdateMsg(BaseObject, Target: TBaseObject; wIdent, wParam: Integer;
  nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar); stdcall;
begin
  BaseObject.SendUpdateMsg(Target, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
end;

// 发聊天信息
function _TBaseObject_SysMsg(BaseObject: TBaseObject; sMsg: PAnsiChar; FColor, BColor: Byte; MsgType: Integer): BOOL; stdcall;
begin
  Result := False;
  if (MsgType >= Integer(Low(TMsgType))) and (MsgType <= Integer(High(TMsgType))) then
  begin
    BaseObject.SysMsg(sMsg, FColor, BColor, TMsgType(MsgType));
    Result := True;
  end;
end;

// 背包物品
function _TBaseObject_GetBagItemList(BaseObject: TBaseObject): TList; stdcall;
begin
  Result := BaseObject.m_ItemList;
end;

// 检测背包是否满
function _TBaseObject_IsEnoughBag(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.IsEnoughBag;
end;

// 背包增加AddCount数量后，是否能放得下
function _TBaseObject_IsEnoughBagEx(BaseObject: TBaseObject; AddCount: Integer): BOOL; stdcall;
begin
  Result := BaseObject.IsEnoughBagEx(AddCount);
end;

// 加物品到背包
function _TBaseObject_AddItemToBag(BaseObject: TBaseObject; UserItem: pTUserItem): BOOL; stdcall;
begin
  Result := BaseObject.AddItemToBag(UserItem);
end;

// 删除背包第几个物品
function _TBaseObject_DelBagItemByIndex(BaseObject: TBaseObject; Index: Integer): BOOL; stdcall;
begin
  Result := BaseObject.DelBagItem(Index);
end;

// 根据makeIndex删除背包物品
function _TBaseObject_DelBagItemByMakeIdx(BaseObject: TBaseObject; MakeIndex: Integer; ItemName: PAnsiChar): BOOL; stdcall;
begin
  Result := BaseObject.DelBagItem(MakeIndex, ItemName);
end;

// 根据UserItem删除背包物品
function _TBaseObject_DelBagItemByUserItem(BaseObject: TBaseObject; UserItem: pTUserItem): BOOL; stdcall;
begin
  Result := BaseObject.DelBagItem(UserItem);
end;

// 检查角色是否在安全区
function _TBaseObject_IsInSafeZone(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.InSafeZone;
end;

// 检查坐标点是否在安全区内
function _TBaseObject_IsPtInSafeZone(BaseObject: TBaseObject; Envir: TEnvirnoment; nX, nY: Integer): BOOL; stdcall;
begin
  Result := BaseObject.InSafeZone(Envir, nX, nY);
end;

// 重算等级属性(IsSysDef=True: 使用系统默认等级属性; IsSysDef=False: 使用自定义等级属性)
procedure _TBaseObject_RecalcLevelAbil(BaseObject: TBaseObject; IsSysDef: BOOL); stdcall;
begin
  BaseObject.RecalcLevelAbilitys(IsSysDef);
end;

// 重算属性
procedure _TBaseObject_RecalcAbil(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.RecalcAbilitys;
end;

// 重算背包重量
function _TBaseObject_RecalcBagWeight(BaseObject: TBaseObject): Integer; stdcall;
begin
  Result := BaseObject.RecalcBagWeight;
end;

// 指定等级升到下级所要经验值
function _TBaseObject_GetLevelExp(BaseObject: TBaseObject; nLevel: Integer): DWORD; stdcall;
begin
  Result := BaseObject.GetLevelExp(nLevel);
end;

// 升级
procedure _TBaseObject_HasLevelUp(BaseObject: TBaseObject; nLevel: Integer); stdcall;
begin
  BaseObject.HasLevelUp(nLevel);
end;

// 加技能熟练点
function _TBaseObject_TrainSkill(BaseObject: TBaseObject; UserMagic: pTUserMagic; nTranPoint: Integer; IsDoCheck: BOOL)
  : BOOL; stdcall;
begin
  Result := False;
  if IsDoCheck then
  begin
    if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) and (UserMagic <> nil) and
      (UserMagic.btLevel < UserMagic.MagicInfo.btTrainLv) and
      (UserMagic.MagicInfo.TrainLevel[UserMagic.btLevel] <= BaseObject.m_Abil.Level) then
    begin
      BaseObject.TrainSkill(UserMagic, nTranPoint);
      Result := True;
    end;
  end
  else
  begin
    BaseObject.TrainSkill(UserMagic, nTranPoint);
    Result := True;
  end;
end;

// 检查技能是否升级
function _TBaseObject_CheckMagicLevelup(BaseObject: TBaseObject; UserMagic: pTUserMagic): BOOL; stdcall;
begin
  Result := BaseObject.CheckMagicLevelup(UserMagic);
end;

// 技能点改点
procedure _TBaseObject_MagicTranPointChanged(BaseObject: TBaseObject; UserMagic: pTUserMagic); stdcall;
begin
  BaseObject.SendDelayMsg(BaseObject, RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicId,
    MakeWord(UserMagic.btLevel, UserMagic.btNewLevel), UserMagic.nTranPoint, IntToStr(Integer(UserMagic.MagicAttr)), 3000);
end;

// 掉血
procedure _TBaseObject_DamageHealth(BaseObject: TBaseObject; nDamage: Integer; StruckFrom: TBaseObject); stdcall;
begin
  BaseObject.DamageHealth(nDamage, StruckFrom);
end;

// 消耗MP
procedure _TBaseObject_DamageSpell(BaseObject: TBaseObject; nSpellPoint: Integer); stdcall;
begin
  BaseObject.DamageSpell(nSpellPoint);
end;

// 增加HP/MP
procedure _TBaseObject_IncHealthSpell(BaseObject: TBaseObject; nHP, nMP: Integer; SendChangedToClient: BOOL); stdcall;
begin
  BaseObject.IncHealthSpell(nHP, nMP, SendChangedToClient);
end;

// 通知客户端HP/MP改变
procedure _TBaseObject_HealthSpellChanged(BaseObject: TBaseObject; dwDelay: DWORD); stdcall;
begin
  BaseObject.HealthSpellChanged(dwDelay);
end;

// 通知客户端外观改变
procedure _TBaseObject_FeatureChanged(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.FeatureChanged;
end;

// 通知客户端负重改变
procedure _TBaseObject_WeightChanged(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.WeightChanged;
end;

// 减防后物理伤害, nType = 1:减物理防御；nType = 2:减魔法盾防御
// MagicACInfo: 功能设置-->技能魔法-->技能破防百分比中的信息
function _TBaseObject_GetHitStruckDamage(BaseObject: TBaseObject; Target: TBaseObject; nDamage: Integer;
  MagicACInfo: PMagicACInfo; nType: Integer): Integer; stdcall;
begin
  Result := BaseObject.GetHitStruckDamage(Target, nDamage, MagicACInfo, nType);
end;

// 减防后魔法伤害
function _TBaseObject_GetMagStruckDamage(BaseObject: TBaseObject; Target: TBaseObject; nDamage: Integer): Integer; stdcall;
begin
  Result := BaseObject.GetMagStruckDamage(Target, nDamage, nil);
end;

// 顶戴花花翎
function _TBaseObject_GetActorIcon(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;
begin
  Result := False;
  if (Index < Low(BaseObject.m_ActorIcons)) or (Index > High(BaseObject.m_ActorIcons)) then
  begin
    Exit;
  end;

  ActorIcon^ := BaseObject.m_ActorIcons[Index];
  Result := True;
end;

// 设置顶戴花花翎
function _TBaseObject_SetActorIcon(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;
begin
  Result := False;
  if (Index < Low(BaseObject.m_ActorIcons)) or (Index > High(BaseObject.m_ActorIcons)) then
  begin
    Exit;
  end;

  BaseObject.m_ActorIcons[Index] := ActorIcon^;
  Result := True;
end;

// 刷新顶戴花花翎
procedure _TBaseObject_RefUseIcons(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.RefUseIcons;
end;

// 刷新效果
procedure _TBaseObject_RefUseEffects(BaseObject: TBaseObject); stdcall;
begin
  BaseObject.RefUseEffects;
end;

// 飞到指定地图及坐标
procedure _TBaseObject_SpaceMove(BaseObject: TBaseObject; sMapName: PAnsiChar; nX, nY: Integer; nInt: Integer); stdcall;
begin
  BaseObject.SpaceMove(sMapName, nX, nY, nInt);
end;

// 飞到指定地图随机坐标
procedure _TBaseObject_MapRandomMove(BaseObject: TBaseObject; sMapName: PAnsiChar; nInt: Integer); stdcall;
begin
  BaseObject.MapRandomMove(sMapName, nInt);
end;

// 对象是否可移动(当麻痹，冰结等状态时，不能移动)
function _TBaseObject_CanMove(BaseObject: TBaseObject): BOOL; stdcall;
begin
  Result := BaseObject.CanMove;
end;

// 是否可以从一个点跑往另一个点
function _TBaseObject_CanRun(BaseObject: TBaseObject; nCurrX, nCurrY, nX, nY: Integer): BOOL; stdcall;
begin
  Result := BaseObject.CanRun(nCurrX, nCurrY, nX, nY);
end;

// 转向
procedure _TBaseObject_TurnTo(BaseObject: TBaseObject; btDir: Byte); stdcall;
begin
  BaseObject.TurnTo(btDir);
end;

// 指定方向走一步
function _TBaseObject_WalkTo(BaseObject: TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;
begin
  Result := BaseObject.WalkTo(btDir, boFlag);
end;

// 指定方向跑一步
function _TBaseObject_RunTo(BaseObject: TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;
begin
  Result := BaseObject.RunTo(btDir, boFlag);
end;

// 预留给插件用，其他地方未使用
function _TBaseObject_PluginList(BaseObject: TBaseObject): TList; stdcall;
begin
  Result := BaseObject.m_Pointer;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 获取技能列表
function _TSmartObject_GetMagicList(SmartObject: TSmartObject): TList; stdcall;
begin
  Result := SmartObject.m_MagicList;
end;

// 取某个位置的装备; Index: 0-29
function _TSmartObject_GetUseItem(SmartObject: TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;
begin
  UserItem^ := SmartObject.m_UseItems[Index];
  Result := True;
end;

// 首饰盒状态 0:未激活; 1:激活; 2:开启
function _TSmartObject_GetJewelryBoxStatus(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := Integer(SmartObject.m_nJewelryBoxStatus);
end;

procedure _TSmartObject_SetJewelryBoxStatus(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  if (Value >= Integer(Low(TJewelryBoxStatus))) and (Value <= Integer(High(TJewelryBoxStatus))) then
  begin
    SmartObject.m_nJewelryBoxStatus := TJewelryBoxStatus(Value);
  end;
end;

// 取首饰盒物品; Index: 0-5
function _TSmartObject_GetJewelryItem(SmartObject: TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;
begin
  UserItem^ := SmartObject.m_JewelryBoxItems[Index];
  Result := True;
end;

// 是否显示神佑袋
function _TSmartObject_GetIsShowGodBless(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boShowGodBless;
end;

procedure _TSmartObject_SetIsShowGodBless(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boShowGodBless := Value;
end;

// 取某个神佑的开关状态
function _TSmartObject_GetGodBlessItemsState(SmartObject: TSmartObject; Index: Integer): BOOL; stdcall;
begin
  Result := SmartObject.m_GodBlessItemsState[Index] <> 0;
end;

procedure _TSmartObject_SetGodBlessItemsState(SmartObject: TSmartObject; Index: Integer; Value: BOOL); stdcall;
begin
  SmartObject.m_GodBlessItemsState[Index] := Integer(Value);
end;

// 取神佑袋物品; Index: 0-11
function _TSmartObject_GetGodBlessItem(SmartObject: TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;
begin
  UserItem^ := SmartObject.m_GodBlessItems[Index];
  Result := True;
end;

// 封号列表
function _TSmartObject_GetFengHaoItems(SmartObject: TSmartObject): TList; stdcall;
begin
  Result := SmartObject.m_FengHaoItems;
end;

// 激活的封号
function _TSmartObject_GetActiveFengHao(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := SmartObject.m_ActiveFengHao;
end;

// 设置当前激活封号
procedure _TSmartObject_SetActiveFengHao(SmartObject: TSmartObject; FengHaoIndex: Integer); stdcall;
begin
  if FengHaoIndex < 0 then
  begin
    SmartObject.m_ActiveFengHao := -1;
  end
  else if (FengHaoIndex >= 0) and (FengHaoIndex < SmartObject.m_FengHaoItems.Count) then
  begin
    SmartObject.m_ActiveFengHao := FengHaoIndex;
  end;
end;

procedure _TSmartObject_ActiveFengHaoChanged(SmartObject: TSmartObject); stdcall;
var
  DefMsg: TDefaultMessage;
begin
  if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
    DefMsg := MakeDefaultMsg(SM_ACTIVEFENGHAOITEM, SmartObject.m_ActiveFengHao, 0, 0, 0);
    TPlayObject(SmartObject).SendSocket(@DefMsg, '');
  end
  else if SmartObject.m_btRaceServer = RC_HEROOBJECT then
  begin
    DefMsg := MakeDefaultMsg(SM_ACTIVEFENGHAOITEM, SmartObject.m_ActiveFengHao, 0, 1, 0);
    THeroObject(SmartObject).SendSocket(@DefMsg, '');
  end;
end;

// 删除封号
procedure _TSmartObject_DeleteFengHao(SmartObject: TSmartObject; Index: Integer); stdcall;
begin
  if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
    TPlayObject(SmartObject).SendDelFengHaoItem(Index, False);
  end
  else if (SmartObject.m_btRaceServer = RC_HEROOBJECT) and (SmartObject.m_Master <> nil) and
    (SmartObject.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(SmartObject.m_Master).SendDelFengHaoItem(Index, True);
  end;
end;

// 清空封号
procedure _TSmartObject_ClearFengHao(SmartObject: TSmartObject); stdcall;
begin
  if SmartObject.m_btRaceServer = RC_PLAYOBJECT then
  begin
    TPlayObject(SmartObject).SendDelFengHaoItem(-1, False);
  end
  else if (SmartObject.m_btRaceServer = RC_HEROOBJECT) and (SmartObject.m_Master <> nil) and
    (SmartObject.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(SmartObject.m_Master).SendDelFengHaoItem(-1, True);
  end;
end;

function _TSmartObject_GetMoveSpeed(SmartObject: TSmartObject): SmallInt; stdcall;
begin
  Result := SmartObject.m_nMoveSpeed;
end;

procedure _TSmartObject_SetMoveSpeed(SmartObject: TSmartObject; Value: SmallInt); stdcall;
begin
  SmartObject.m_nMoveSpeed := Value;
end;

function _TSmartObject_GetAttackSpeed(SmartObject: TSmartObject): SmallInt; stdcall;
begin
  Result := SmartObject.m_nAttackSpeed;
end;

procedure _TSmartObject_SetAttackSpeed(SmartObject: TSmartObject; Value: SmallInt); stdcall;
begin
  SmartObject.m_nAttackSpeed := Value;
end;

function _TSmartObject_GetSpellSpeed(SmartObject: TSmartObject): SmallInt; stdcall;
begin
  Result := SmartObject.m_nSpellSpeed;
end;

procedure _TSmartObject_SetSpellSpeed(SmartObject: TSmartObject; Value: SmallInt); stdcall;
begin
  SmartObject.m_nSpellSpeed := Value;
end;

// 游戏速度
procedure _TSmartObject_RefGameSpeed(SmartObject: TSmartObject); stdcall;
begin
  SmartObject.RefGameSpeed;
end;

// 是否可挖
function _TSmartObject_GetIsButch(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boButch;
end;

procedure _TSmartObject_SetIsButch(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boButch := Value;
end;

// 是否学内功
function _TSmartObject_GetIsTrainingNG(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boTrainingNG;
end;

procedure _TSmartObject_SetIsTrainingNG(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boTrainingNG := Value;
end;

// 是否学心法
function _TSmartObject_GetIsTrainingXF(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boTrainingXF;
end;

procedure _TSmartObject_SetIsTrainingXF(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boTrainingXF := Value;
end;

// 第4个连击是否开启
function _TSmartObject_GetIsOpenLastContinuous(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boOpenLastContinuous;
end;

procedure _TSmartObject_SetIsOpenLastContinuous(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boOpenLastContinuous := Value;
end;

// 连击顺序 Index:0-3
function _TSmartObject_GetContinuousMagicOrder(SmartObject: TSmartObject; Index: Integer): Byte; stdcall;
begin
  Result := SmartObject.m_ContinuousMagicOrder[Index];
end;

procedure _TSmartObject_SetContinuousMagicOrder(SmartObject: TSmartObject; Index: Integer; Value: Byte); stdcall;
begin
  SmartObject.m_ContinuousMagicOrder[Index] := Value;
end;

// PK 死亡掉经验，不够经验就掉等级
function _TSmartObject_GetPKDieLostExp(SmartObject: TSmartObject): DWORD; stdcall;
begin
  Result := SmartObject.m_dwPKDieLostExp;
end;

procedure _TSmartObject_SetPKDieLostExp(SmartObject: TSmartObject; Value: DWORD); stdcall;
begin
  SmartObject.m_dwPKDieLostExp := Value;
end;

// PK 死亡掉等级
function _TSmartObject_GetPKDieLostLevel(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := SmartObject.m_nPKDieLostLevel;
end;

procedure _TSmartObject_SetPKDieLostLevel(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  SmartObject.m_nPKDieLostLevel := Value;
end;

// PK点数
function _TSmartObject_GetPKPoint(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := SmartObject.m_nPkPoint;
end;

procedure _TSmartObject_SetPKPoint(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  SmartObject.m_nPkPoint := Value;
end;

// 增加PK值
procedure _TSmartObject_IncPKPoint(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  SmartObject.IncPkPoint(Value);
end;

// 减少PK值
procedure _TSmartObject_DecPKPoint(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  SmartObject.DecPKPoint(Value);
end;

// PK等级
function _TSmartObject_GetPKLevel(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := SmartObject.m_nPkPoint div 100;
end;

procedure _TSmartObject_SetPKLevel(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  SmartObject.m_nPkPoint := Value * 100;
end;

// 传送戒指 特殊物品:112
function _TSmartObject_GetIsTeleport(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boTeleport;
end;

procedure _TSmartObject_SetIsTeleport(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boTeleport := Value;
end;

// 复活戒指 特殊物品:114
function _TSmartObject_GetIsRevival(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boRevival;
end;

procedure _TSmartObject_SetIsRevival(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boRevival := Value;
end;

// 复活戒指: 复活间隔
function _TSmartObject_GetRevivalTime(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := SmartObject.m_nRevivalTime;
end;

procedure _TSmartObject_SetRevivalTime(SmartObject: TSmartObject; Value: Integer); stdcall;
begin
  SmartObject.m_nRevivalTime := Value;
end;

// 火焰戒指 特殊物品:115
function _TSmartObject_GetIsFlameRing(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boFlameRing;
end;

procedure _TSmartObject_SetIsFlameRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boFlameRing := Value;
end;

// 治愈戒指 特殊物品:116
function _TSmartObject_GetIsRecoveryRing(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boRecoveryRing;
end;

procedure _TSmartObject_SetIsRecoveryRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boRecoveryRing := Value;
end;

// 护身戒指 特殊物品:118
function _TSmartObject_GetIsMagicShield(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boMagicShield;
end;

procedure _TSmartObject_SetIsMagicShield(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boMagicShield := Value;
end;

// 活力戒指(超负载) 特殊物品:119
function _TSmartObject_GetIsMuscleRing(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boMuscleRing;
end;

procedure _TSmartObject_SetIsMuscleRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boMuscleRing := Value;
end;

// 技巧项链 特殊物品:120
function _TSmartObject_GetIsFastTrain(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boFastTrain;
end;

procedure _TSmartObject_SetIsFastTrain(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boFastTrain := Value;
end;

// 探测项链 特殊物品:121
function _TSmartObject_GetIsProbeNecklace(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boProbeNecklace;
end;

procedure _TSmartObject_SetIsProbeNecklace(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boProbeNecklace := Value;
end;

// 记忆物品 特殊物品:122, 124, 125
function _TSmartObject_GetIsRecallSuite(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boRecallSuite;
end;

procedure _TSmartObject_SetIsRecallSuite(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boRecallSuite := Value;
end;

// 祈祷装备 特殊物品:126 - 129
function _TSmartObject_GetIsPirit(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_bopirit;
end;

procedure _TSmartObject_SetIsPirit(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_bopirit := Value;
end;

// 不死戒指 特殊物品:140
function _TSmartObject_GetIsSupermanItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boSupermanItem;
end;

procedure _TSmartObject_SetIsSupermanItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boSupermanItem := Value;
end;

// 经验物品 特殊物品:141
function _TSmartObject_GetIsExpItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boExpItem;
end;

procedure _TSmartObject_SetIsExpItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boExpItem := Value;
end;

// 经验物品值 特殊物品:141
function _TSmartObject_GetExpItemValue(SmartObject: TSmartObject): Real; stdcall;
begin
  Result := SmartObject.m_rExpItem;
end;

procedure _TSmartObject_SetExpItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
begin
  SmartObject.m_rExpItem := Value;
end;

// 经验物品经验倍率(物品装备->特殊属性->经验翻倍->倍率)
function _TSmartObject_GetExpItemRate(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := g_Config.nItemExpRate;
end;

// 经验物品 特殊物品:142
function _TSmartObject_GetIsPowerItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boPowerItem;
end;

procedure _TSmartObject_SetIsPowerItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boPowerItem := Value;
end;

// 经验物品值 特殊物品:142
function _TSmartObject_GetPowerItemValue(SmartObject: TSmartObject): Real; stdcall;
begin
  Result := SmartObject.m_rPowerItem;
end;

procedure _TSmartObject_SetPowerItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
begin
  SmartObject.m_rPowerItem := Value;
end;

function _TSmartObject_GetPowerItemRate(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := g_Config.nItemPowerRate;
end;

// 行会传送装备 特殊物品:145
function _TSmartObject_GetIsGuildMove(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boGuildMove;
end;

procedure _TSmartObject_SetIsGuildMove(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boGuildMove := Value;
end;

// 幸运戒指 特殊物品 170
function _TSmartObject_GetIsAngryRing(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boAngryRing;
end;

procedure _TSmartObject_SetIsAngryRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boAngryRing := Value;
end;

// 流星戒指
function _TSmartObject_GetIsStarRing(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boStarRing;
end;

procedure _TSmartObject_SetIsStarRing(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boStarRing := Value;
end;

// 防御物品
function _TSmartObject_GetIsACItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boACItem;
end;

procedure _TSmartObject_SetIsACItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boACItem := Value;
end;

// 防御值
function _TSmartObject_GetACItemValue(SmartObject: TSmartObject): Real; stdcall;
begin
  Result := SmartObject.m_rACItem;
end;

procedure _TSmartObject_SetACItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
begin
  SmartObject.m_rACItem := Value;
end;

// 魔御物品
function _TSmartObject_GetIsMACItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boMACItem;
end;

procedure _TSmartObject_SetIsMACItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boMACItem := Value;
end;

// 魔御值
function _TSmartObject_GetMACItemValue(SmartObject: TSmartObject): Real; stdcall;
begin
  Result := SmartObject.m_rMACItem;
end;

procedure _TSmartObject_SetMACItemValue(SmartObject: TSmartObject; Value: Real); stdcall;
begin
  SmartObject.m_rMACItem := Value;
end;

// 171不掉背包物品装备
function _TSmartObject_GetIsNoDropItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boNoDropItem;
end;

procedure _TSmartObject_SetIsNoDropItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boNoDropItem := Value;
end;

// 172不掉身上物品装备
function _TSmartObject_GetIsNoDropUseItem(SmartObject: TSmartObject): BOOL; stdcall;
begin
  Result := SmartObject.m_boNoDropUseItem;
end;

procedure _TSmartObject_SetIsNoDropUseItem(SmartObject: TSmartObject; Value: BOOL); stdcall;
begin
  SmartObject.m_boNoDropUseItem := Value;
end;

// 内功属性
function _TSmartObject_GetNGAbility(SmartObject: _TSmartObject; AbilityNG: pTAbilityNG): BOOL; stdcall;
begin
  AbilityNG^ := SmartObject.m_AbilNG;
  Result := True;
end;

procedure _TSmartObject_SetNGAbility(SmartObject: TSmartObject; Value: pTAbilityNG); stdcall;
begin
  SmartObject.m_AbilNG := Value^;
end;

// 酒属性
function _TSmartObject_GetAlcohol(SmartObject: _TSmartObject; AbilityAlcohol: pTAbilityAlcohol): BOOL; stdcall;
begin
  AbilityAlcohol^ := SmartObject.m_Alcohol;
  Result := True;
end;

procedure _TSmartObject_SetAlcohol(SmartObject: TSmartObject; Value: pTAbilityAlcohol); stdcall;
begin
  SmartObject.m_Alcohol := Value^;
end;

// 修复所有装备
procedure _TSmartObject_RepairAllItem(SmartObject: TSmartObject); stdcall;
begin
  SmartObject.RepairAllItem(False);
end;

// 是否满足技能使用条件
function _TSmartObject_IsAllowUseMagic(SmartObject: TSmartObject; MagicID: Word): BOOL; stdcall;
begin
  if (SmartObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) then
    Result := SmartObject.AllowUseMagic(MagicID)
  else
    Result := False;
end;

// 选择技能
function _TSmartObject_SelectMagic(SmartObject: TSmartObject): Integer; stdcall;
begin
  Result := SmartObject.SelectMagic;
end;

// 攻击目标
function _TSmartObject_AttackTarget(SmartObject: TSmartObject; MagicID: Word; AttackTime: DWORD): BOOL; stdcall;
begin
  Result := SmartObject.AttackTarget(MagicID, AttackTime)
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 帐户名
function _TPlayObject_GetUserID(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sUserID;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// IP
function _TPlayObject_GetIPAddr(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sIPaddr;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// IP归属地
function _TPlayObject_GetIPLocal(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sIPLocal;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// MAC
function _TPlayObject_GetMachineID(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sMachineID;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 是否进入游戏完成
function _TPlayObject_GetIsReadyRun(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boReadyRun;
end;

// 登录时间
function _TPlayObject_GetLogonTime(Player: TPlayObject; LogonTime: PSystemTime): BOOL; stdcall;
var
  wYear, wMonth, wDay: Word;
  wHour, wMin, wSec, wMSec: Word;
begin
  DecodeDate(Player.m_dLogonTime, wYear, wMonth, wDay);
  DecodeTime(Player.m_dLogonTime, wHour, wMin, wSec, wMSec);
  LogonTime.wYear := wYear;
  LogonTime.wMonth := wMonth;
  LogonTime.wDayOfWeek := DayOfWeek(Player.m_dLogonTime);
  LogonTime.wDay := wDay;
  LogonTime.wHour := wHour;
  LogonTime.wMinute := wMin;
  LogonTime.wSecond := wSec;
  LogonTime.wMilliseconds := wMSec;
  Result := True;
end;

// 客户端版本号
function _TPlayObject_GetSoftVerDate(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nSoftVersionDate;
end;

// 客户端类型(0:176; 1:185; 2:英雄版; 3:连击版; 4:传奇续章; 5:外传; 6:归来)
function _TPlayObject_GetClientType(Player: TPlayObject): Integer; stdcall;
begin
  Result := Integer(Player.m_ClientUIType);
end;

// 是否为老客户端(185兼容客户端)
function _TPlayObject_IsOldClient(Player: TPlayObject): BOOL; stdcall;
begin
  Result := False;
end;

// 客户端分辨率 宽
function _TPlayObject_GetScreenWidth(Player: TPlayObject): Word; stdcall;
begin
  Result := Player.m_wScreenWidth;
end;

// 客户端分辨率 高
function _TPlayObject_GetScreenHeight(Player: TPlayObject): Word; stdcall;
begin
  Result := Player.m_wScreenHeight;
end;

// 客户端视觉范围大小
function _TPlayObject_GetClientViewRange(Player: TPlayObject): Word; stdcall;
begin
  Result := Player.m_wClientViewRange;
end;

// 转生等级
function _TPlayObject_GetRelevel(Player: TPlayObject): Byte; stdcall;
begin
  Result := Player.m_btReLevel;
end;

procedure _TPlayObject_SetRelevel(Player: TPlayObject; Value: Byte); stdcall;
begin
  Player.m_btReLevel := Value;
end;

// 未分配属性点
function _TPlayObject_GetBonusPoint(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nBonusPoint;
end;

procedure _TPlayObject_SetBonusPoint(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nBonusPoint := Value;
end;

// 发送属性点
procedure _TPlayObject_SendAdjustBonus(Player: TPlayObject); stdcall;
begin
  Player.SendAdjustBonus;
end;

// 主将英雄名
function _TPlayObject_GetHeroName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sHeroName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 副将英雄名
function _TPlayObject_GetDeputyHeroName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sDeputyHeroName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 副将英雄职业
function _TPlayObject_GetDeputyHeroJob(Player: TPlayObject): Byte; stdcall;
begin
  Result := Player.m_btDeputyHeroJob
end;

// 英雄对象
function _TPlayObject_GetMyHero(Player: TPlayObject): THeroObject; stdcall;
begin
  if Player.m_MyHero = nil then
    Result := nil
  else
    Result := THeroObject(Player.m_MyHero);
end;

// 是否评定主副英雄
function _TPlayObject_GetFixedHero(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boFixedHero;
end;

procedure _TPlayObject_ClientHeroLogOn(Player: TPlayObject; IsDeputyHero: BOOL); stdcall; // 召唤英雄
begin
  if Player.m_boOffLine or Player.m_boWaitHeroDate or Player.m_boGhost or Player.m_boDeath or Player.m_PEnvir.m_boNoRecallHero
  then
  begin
    Exit;
  end;

  if Player.m_MyHero = nil then
  begin
    if not IsDeputyHero then
    begin
      if Length(Player.m_sHeroName) > 0 then
      begin
        DataEngine.LoadHeroData(Player, nil, Player.m_sHeroName, False, 0);
      end;
    end
    else
    begin
      if Length(Player.m_sDeputyHeroName) > 0 then
      begin
        DataEngine.LoadHeroData(Player, nil, Player.m_sDeputyHeroName, True, Player.m_btDeputyHeroJob);
      end;
    end;
  end;
end;

// 英雄是否寄存
function _TPlayObject_GetStorageHero(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boStorageHero;
end;

// 副将英雄是否寄存
function _TPlayObject_GetStorageDeputyHero(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boStorageDeputyHero;
end;

// 仓库是否开启 Index:仓库序号(0-3);
function _TPlayObject_GetIsStorageOpen(Player: TPlayObject; Index: Integer): BOOL; stdcall;
begin
  Result := Player.m_boStorageOpen[Index];
end;

procedure _TPlayObject_SetIsStorageOpen(Player: TPlayObject; Index: Integer; Value: BOOL); stdcall;
begin
  Player.m_boStorageOpen[Index] := Value;
end;

// 金币数量
function _TPlayObject_GetGold(Player: TPlayObject): DWORD; stdcall;
begin
  Result := Player.m_nGold;
end;

procedure _TPlayObject_SetGold(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.m_nGold := Value;
end;

function _TPlayObject_GetGoldMax(Player: TPlayObject): DWORD; stdcall;
begin
  Result := Player.m_nGoldMax;
end;

// 加金币
function _TPlayObject_IncGold(Player: TPlayObject; Value: DWORD): BOOL; stdcall;
begin
  Result := Player.IncGold(Value);
end;

// 减金币
function _TPlayObject_DecGold(Player: TPlayObject; Value: DWORD): BOOL; stdcall;
begin
  Result := Player.DecGold(Value);
end;

// 通知客户端刷新(金币，元宝)
procedure _TPlayObject_GoldChanged(Player: TPlayObject); stdcall;
begin
  Player.GoldChanged;
end;

// 元宝数量
function _TPlayObject_GetGameGold(Player: TPlayObject): DWORD; stdcall;
begin
  Result := Player.m_nGameGold;
end;

procedure _TPlayObject_SetGameGold(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.m_nGameGold := Value;
end;

// 加元宝
procedure _TPlayObject_IncGameGold(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.IncGameGold(Value);
end;

// 减元宝
procedure _TPlayObject_DecGameGold(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.DecGameGold(Value);
end;

// 通知客户端刷新(元宝，游戏点)
procedure _TPlayObject_GameGoldChanged(Player: TPlayObject); stdcall;
begin
  Player.GameGoldChanged;
end;

// 游戏点
function _TPlayObject_GetGamePoint(Player: TPlayObject): DWORD; stdcall;
begin
  Result := Player.m_nGamePoint;
end;

procedure _TPlayObject_SetGamePoint(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.m_nGamePoint := Value;
end;

// 加游戏点
procedure _TPlayObject_IncGamePoint(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.IncGamePoint(Value);
end;

// 减游戏点
procedure _TPlayObject_DecGamePoint(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.DecGamePoint(Value);
end;

// 金刚石
function _TPlayObject_GetGameDiamond(Player: TPlayObject): DWORD; stdcall;
begin
  Result := Player.m_nGameDiamond;
end;

procedure _TPlayObject_SetGameDiamond(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.m_nGameDiamond := Value;
end;

// 加金刚石
procedure _TPlayObject_IncGameDiamond(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.IncGameDiamond(Value);
end;

// 减金刚石
procedure _TPlayObject_DecGameDiamond(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.DecGameDiamond(Value);
end;

// 通知客户端刷新(金刚石，灵符)
procedure _TPlayObject_NewGamePointChanged(Player: TPlayObject); stdcall;
begin
  Player.NewGamePointChanged;
end;

// 灵符
function _TPlayObject_GetGameGird(Player: TPlayObject): DWORD; stdcall;
begin
  Result := Player.m_nGameGird;
end;

procedure _TPlayObject_SetGameGird(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.m_nGameGird := Value;
end;

// 加灵符
procedure _TPlayObject_IncGameGird(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.IncGameGird(Value);
end;

// 减灵符
procedure _TPlayObject_DecGameGird(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.DecGameGird(Value);
end;

// 新游戏点
function _TPlayObject_GetGameGoldEx(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nGameGoldEx;
end;

procedure _TPlayObject_SetGameGoldEx(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nGameGoldEx := Value;
end;

// 荣誉
function _TPlayObject_GetGameGlory(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nGameGlory;
end;

procedure _TPlayObject_SetGameGlory(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nGameGlory := Value;
end;

// 加荣誉
procedure _TPlayObject_IncGameGlory(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.IncGameGlory(Value);
end;

// 减荣誉
procedure _TPlayObject_DecGameGlory(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.DecGameGlory(Value);
end;

// 通知客户端刷新荣誉
procedure _TPlayObject_GameGloryChanged(Player: TPlayObject); stdcall;
begin
  Player.GameGloryChanged;
end;

// 充值点
function _TPlayObject_GetPayMentPoint(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nPayMentPoint;
end;

procedure _TPlayObject_SetPayMentPoint(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nPayMentPoint := Value;
end;

// 会员类型
function _TPlayObject_GetMemberType(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nMemberType;
end;

procedure _TPlayObject_SetMemberType(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nMemberType := Value;
end;

// 会员等级
function _TPlayObject_GetMemberLevel(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nMemberLevel;
end;

procedure _TPlayObject_SetMemberLevel(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nMemberLevel := Value;
end;

// 贡献度
function _TPlayObject_GetContribution(Player: TPlayObject): Word; stdcall;
begin
  Result := Player.m_wContribution;
end;

procedure _TPlayObject_SetContribution(Player: TPlayObject; Value: Word); stdcall;
begin
  Player.m_wContribution := Value;
end;

// 加经验
procedure _TPlayObejct_IncExp(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.IncExp(Value);
end;

procedure _TPlayObject_SendExpChanged(Player: TPlayObject); stdcall;
begin
  Player.SendMsg(Player, RM_WINEXP, 0, Player.m_WAbil.Exp, 0, 0, '');
end;

// 加内功经验
procedure _TPlayObject_IncExpNG(Player: TPlayObject; Value: DWORD); stdcall;
begin
  Player.IncExpNG(Value);
end;

procedure _TPlayObject_SendExpNGChanged(Player: TPlayObject); stdcall;
begin
  Player.SendMsg(Player, RM_WINEXP, 1, Player.m_WAbil.Exp, 0, 0, '');
end;

// 增加聚灵珠经验
procedure _TPlayObject_IncBeadExp(Player: TPlayObject; Value: DWORD; IsFromNPC: BOOL); stdcall;
begin
  Player.IncBeadExp(Value, IsFromNPC);
end;

// P变量 m_nVal  [0..999]
function _TPlayObject_GetVarP(Player: TPlayObject; Index: Integer): Integer; stdcall;
begin
  Result := Player.m_nVal[Index];
end;

procedure _TPlayObject_SetVarP(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
begin
  Player.m_nVal[Index] := Value;
end;

// M变量 m_nMval [0..999]
function _TPlayObject_GetVarM(Player: TPlayObject; Index: Integer): Integer; stdcall;
begin
  Result := Player.m_nMVal[Index];
end;

procedure _TPlayObject_SetVarM(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
begin
  Player.m_nMVal[Index] := Value;
end;

// D变量 m_DyVal [0..999]
function _TPlayObject_GetVarD(Player: TPlayObject; Index: Integer): Integer; stdcall;
begin
  Result := Player.m_DyVal[Index];
end;

procedure _TPlayObject_SetVarD(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
begin
  Player.m_DyVal[Index] := Value;
end;

// U变量 m_UVal [0..254]
function _TPlayObject_GetVarU(Player: TPlayObject; Index: Integer): Integer; stdcall;
begin
  Result := Player.m_UVal[Index];
end;

procedure _TPlayObject_SetVarU(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
begin
  Player.m_UVal[Index] := Value;
end;

// T变量 m_UVal [0..254]
function _TPlayObject_GetVarT(Player: TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_TVal[Index];
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TPlayObject_SetVarT(Player: TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;
begin
  Player.m_TVal[Index] := Value;
end;

// N变量 m_nInteger [0..999]
function _TPlayObject_GetVarN(Player: TPlayObject; Index: Integer): Integer; stdcall;
begin
  Result := Player.m_nInteger[Index];
end;

procedure _TPlayObject_SetVarN(Player: TPlayObject; Index: Integer; Value: Integer); stdcall;
begin
  Player.m_nInteger[Index] := Value;
end;

// S变量 m_sString [0..999]
function _TPlayObject_GetVarS(Player: TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sString[Index];
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TPlayObject_SetVarS(Player: TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;
begin
  Player.m_sString[Index] := Value;
end;

// 自定义动态变量列表(返回结果.GetItem = pTDynamicVar);
function _TPlayObject_GetDynamicVarList(Player: TPlayObject): TList; stdcall;
begin
  Result := Player.m_DynamicVarList;
end;


// m_IntegerList: TQuickList; // N
// m_StringList: TValueList; // S

function _TPlayObject_GetQuestFlagStatus(Player: TPlayObject; nFlag: Integer): Integer; stdcall;
begin
  Result := Player.GetQuestFlagStatus(nFlag);
end;

procedure _TPlayObject_SetQuestFlagStatus(Player: TPlayObject; nFlag: Integer; Value: Integer); stdcall;
begin
  Player.SetQuestFlagStatus(nFlag, Value);
end;

// 是否离线挂机
function _TPlayObject_IsOffLine(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boOffLine;
end;

// 是否是师傅
function _TPlayObject_IsMaster(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boMaster;
end;

// 师傅名字
function _TPlayObject_GetMasterName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sMasterName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 师傅
function _TPlayObject_GetMasterHuman(Player: TPlayObject): TPlayObject; stdcall;
begin
  Result := Player.m_MasterHuman;
end;

// 当角色为徒弟时，徒弟排名
function _TPlayObject_GetApprenticeNO(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nMasterNo;
end;

// 在线徒弟列表
function _TPlayObject_GetOnlineApprenticeList(Player: TPlayObject): TList; stdcall;
begin
  Result := Player.m_MasterList;
end;

// 所有徒弟列表(返回结果.GetItem = pTMasterRankInfo);
function _TPlayObject_GetAllApprenticeList(Player: TPlayObject): TList; stdcall;
begin
  Result := Player.m_MasterNoList;
end;

// 爱人名字
function _TPlayObject_GetDearName(Player: TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Player.m_sDearName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 爱人
function _TPlayObject_GetDearHuman(Player: TPlayObject): TPlayObject; stdcall;
begin
  Result := Player.m_DearHuman;
end;

// 离婚次数
function _TPlayObject_GetMarryCount(Player: TPlayObject): Byte; stdcall;
begin
  Result := Player.m_btMarryCount;
end;

// 队长
function _TPlayObject_GetGroupOwner(Player: TPlayObject): TPlayObject; stdcall;
begin
  Result := Player.m_GroupOwner;
end;

// 队员列表
function _TPlayObject_GetGroupMembers(Player: TPlayObject): TStringList; stdcall;
begin
  Result := Player.m_GroupMembers;
end;

// 锁定登录
function _TPlayObject_GetIsLockLogin(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boLockLogon;
end;

procedure _TPlayObject_SetIsLockLogin(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boLockLogon := Value;
end;

// 是否允许组队
function _TPlayObject_GetIsAllowGroup(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boAllowGroup;
end;

procedure _TPlayObject_SetIsAllowGroup(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boAllowGroup := Value;
end;

// 是否允许天地合一
function _TPlayObject_GetIsAllowGroupReCall(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boAllowGroupReCall;
end;

procedure _TPlayObject_SetIsAllowGroupReCall(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boAllowGroupReCall := Value;
end;

// 是否允许行会合一
function _TPlayObject_GetIsAllowGuildReCall(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boAllowGuildReCall;
end;

procedure _TPlayObject_SetIsAllowGuildReCall(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boAllowGuildReCall := Value;
end;

// 禁止交易
function _TPlayObject_GetIsAllowTrading(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boAllowDeal;
end;

procedure _TPlayObject_SetIsAllowTrading(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boAllowDeal := Value;
end;

// 禁止邀请上马
function _TPlayObject_GetIsDisableInviteHorseRiding(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boDisableHorseInvite;
end;

procedure _TPlayObject_SetIsDisableInviteHorseRiding(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boDisableHorseInvite := False;
end;

// 是否开启元宝交易
function _TPlayObject_GetIsGameGoldTrading(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boGameGoldDeal;
end;

procedure _TPlayObject_SetIsGameGoldTrading(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boGameGoldDeal := Value;
end;

// 合过区没有登录过的
function _TPlayObject_GetIsNewServer(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_btNewServer <> 0;
end;

// 过滤掉落提示信息
function _TPlayObject_GetIsFilterGlobalDropItemMsg(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boFilterGlobalDropItemMsg;
end;

procedure _TPlayObject_SetIsFilterGlobalDropItemMsg(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boFilterGlobalDropItemMsg := Value;
end;

// 过滤SendCenterMsg
function _TPlayObject_GetIsFilterGlobalCenterMsg(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boFilterGlobalCenterMsg;
end;

procedure _TPlayObject_SetIsFilterGlobalCenterMsg(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boFilterGlobalCenterMsg := Value;
end;

// 过滤SendMsg全局信息
function _TPlayObject_GetIsFilterGolbalSendMsg(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boFilterGolbalSendMsg;
end;

procedure _TPlayObject_SetIsFilterGolbalSendMsg(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boFilterGolbalSendMsg := Value;
end;

// 是否请过酒
function _TPlayObject_GetIsPleaseDrink(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boPleaseDrink;
end;

// 饮酒时酒的品质
function _TPlayObject_GetIsDrinkWineQuality(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nDrinkWineQuality;
end;

procedure _TPlayObject_SetIsDrinkWineQuality(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nDrinkWineQuality := Value;
end;

// 饮酒时酒的度数
function _TPlayObject_GetIsDrinkWineAlcohol(Player: TPlayObject): Integer; stdcall;
begin
  Result := Player.m_nDrinkWineAlcohol;
end;

procedure _TPlayObject_SetIsDrinkWineAlcohol(Player: TPlayObject; Value: Integer); stdcall;
begin
  Player.m_nDrinkWineAlcohol := Value;
end;

// 人是否喝酒醉了
function _TPlayObject_GetIsDrinkWineDrunk(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boDrinkWineDrunk;
end;

procedure _TPlayObject_SetIsDrinkWineDrunk(Player: TPlayObject; Value: BOOL); stdcall;
begin
  Player.m_boDrinkWineDrunk := Value;
end;

// 酒属性
function _TPlayObject_GetAlcohol(Player: TPlayObject): pTAbilityAlcohol; stdcall;
begin
  Result := @Player.m_Alcohol;
end;

// 回城
procedure _TPlayObject_MoveToHome(Player: TPlayObject); stdcall;
begin
  Player.MoveToHome;
end;

// 随机传送到回城地图
procedure _TPlayObject_MoveRandomToHome(Player: TPlayObject); stdcall;
begin
  Player.MoveRandomToHome;
end;

// 发送数据
procedure _TPlayObject_SendSocket(Player: TPlayObject; DefMsg: pTDefaultMessage; sMsg: PAnsiChar); stdcall;
begin
  Player.SendSocket(DefMsg, sMsg);
end;

// 发送数据  2021-01-05 changed
procedure _TPlayObject_SendDefMessage(Player: TPlayObject; wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word;
  sMsg: PAnsiChar); stdcall;
begin
  Player.SendDefMessage(wIdent, nRecog, nParam, nTag, nSeries, sMsg);
end;

procedure _TPlayObject_SendMoveMsg(Player: TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nY: Word; nMoveCount: Integer;
  nFontSize: Integer; nMarqueeTime: Integer); stdcall;
begin
  Player.SendMoveMsg(sMsg, btFColor, btBColor, nY, nMoveCount, nFontSize, nMarqueeTime);
end;

procedure _TPlayObject_SendCenterMsg(Player: TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;
begin
  Player.SendCenterMsg(sMsg, btFColor, btBColor, nTime);
end;

function _TPlayObject_SendTopBroadCastMsg(Player: TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer;
  MsgType: Integer): BOOL; stdcall;
begin
  Result := False;
  if (MsgType >= Integer(Low(TMsgType))) and (MsgType <= Integer(High(TMsgType))) then
  begin
    Player.SendTopBroadCastMsg(sMsg, btFColor, btBColor, nTime, TMsgType(MsgType));
    Result := True;
  end;
end;

// 检测装备是否可穿戴
function _TPlayObject_CheckTakeOnItems(Player: TPlayObject; Where: Integer; StdItem: PTStdItem): BOOL; stdcall;
begin
  Result := Player.CheckTakeOnItems(Where, StdItem);
end;

// 处理装备穿脱时对应的技能
procedure _TPlayObject_ProcessUseItemSkill(Player: TPlayObject; Where: Integer; StdItem: PTStdItem; IsTakeOn: BOOL); stdcall;
begin
  Player.ProcessUseItemSkill(Where, StdItem, IsTakeOn);
end;

// 发送身上装备列表
procedure _TPlayObject_SendUseItems(Player: TPlayObject); stdcall;
begin
  Player.SendUseItems;
end;

// 发送增加物品
procedure _TPlayObject_SendAddItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
begin
  Player.SendAddItem(UserItem);
end;

// 客户端删除多个物品 ItemList.AddObject(物品名称, MakeIndex)
procedure _TPlayObject_SendDelItemList(Player: TPlayObject; Items: PAnsiChar; ItemsCount: Integer); stdcall;
var
  sItems: AnsiString;
begin
  sItems := Items;
  Player.SendDelItemList(sItems, ItemsCount);
end;

// 客户端删除物品
procedure _TPlayObject_SendDelItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
begin
  Player.SendDelItem(UserItem);
end;

// 客户端刷新物品
procedure _TPlayObject_SendUpdateItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
begin
  Player.SendUpdateItem(UserItem);
end;

// 客户端刷新装备持久改变
procedure _TPlayObject_SendItemDuraChange(Player: TPlayObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;
begin
  Player.SendMsg(Player, RM_DURACHANGE, ItemWhere, UserItem.Dura, UserItem.DuraMax, 0, '');
end;

// 刷新客户端包裹
procedure _TPlayObject_SendBagItems(Player: TPlayObject); stdcall;
begin
  Player.DoQueryBagItems;
end;

// 发送首饰盒物品
procedure _TPlayObject_SendJewelryBoxItems(Player: TPlayObject); stdcall;
begin
  Player.SendJewelryBox(False);
end;

// 发送神佑袋物品
procedure _TPlayObject_SendGodBlessItems(Player: TPlayObject); stdcall;
begin
  Player.SendUpdateGodBless(False);
end;

// 神佑格开启
procedure _TPlayObject_SendOpenGodBlessItem(Player: TPlayObject; Index: Integer); stdcall;
begin
  Player.SendOpenGodBlessItem(False, Index);
end;

// 神佑格关闭
procedure _TPlayObject_SendCloseGodBlessItem(Player: TPlayObject; Index: Integer); stdcall;
begin
  Player.SendCloseGodBlessItem(False, Index);
end;

// 发送技能列表
procedure _TPlayObject_SendUseMagics(Player: TPlayObject); stdcall;
begin
  Player.SendUseMagic;
end;

// 发送技能添加
procedure _TPlayObject_SendAddMagic(Player: TPlayObject; UserMagic: pTUserMagic); stdcall;
begin
  Player.SendAddMagic(UserMagic);
end;

// 发送技能删除
procedure _TPlayObject_SendDelMagic(Player: TPlayObject; UserMagic: pTUserMagic); stdcall;
begin
  Player.SendDelMagic(UserMagic);
end;

// 发送封号物品
procedure _TPlayObject_SendFengHaoItems(Player: TPlayObject); stdcall;
begin
  Player.SendUpdateFengHao(False);
end;

// 发送封号增加
procedure _TPlayObject_SendAddFengHaoItem(Player: TPlayObject; UserItem: pTUserItem); stdcall;
begin
  Player.SendAddFengHaoItem(UserItem, False);
end;

// 发送封号删除
procedure _TPlayObject_SendDelFengHaoItem(Player: TPlayObject; Index: Integer); stdcall;
begin
  Player.SendDelFengHaoItem(Index, False);
end;

// 发送走路/跑步失败
procedure _TPlayObject_SendSocketStatusFail(Player: TPlayObject); stdcall;
begin
  Player.SendSocketStatusFail;
end;

procedure _TPlayObject_PlayEffect(Player: TPlayObject; nFileIndex, nImageOffset, nImageCount, nLoopCount, nSpeedTime: Integer;
  btDrawOrder: Byte; nOffsetX: Integer; nOffsetY: Integer); stdcall;
var
  Effect: pTBaseObjectEffect;
begin
  New(Effect);
  Effect.ActorEffect.nEffectFileIndex := nFileIndex;
  Effect.ActorEffect.nEffectImageOffSet := nImageOffset;
  Effect.ActorEffect.wEffectImageCount := nImageCount;
  Effect.ActorEffect.wEffectFrameTime := nSpeedTime;
  Effect.ActorEffect.nLoopCount := nLoopCount;
  Effect.ActorEffect.btDrawOrder := btDrawOrder;
  Effect.ActorEffect.nOffsetX := nOffsetX;
  Effect.ActorEffect.nOffsetY := nOffsetY;
  Effect.dwEffectTick := MyGetTickCount;

  Player.m_BaseObjectEffects.LockW(6);
  try
    Player.m_BaseObjectEffects.Add(Effect);
  finally
    Player.m_BaseObjectEffects.UnLockW;
  end;
end;

// 是否正在内挂挂机
function _TPlayObject_IsAutoPlayGame(Player: TPlayObject): BOOL; stdcall;
begin
  Result := Player.m_boAutoOnline;
end;

// 开始内挂挂机
function _TPlayObject_StartAutoPlayGame(Player: TPlayObject): BOOL; stdcall;
begin
  Result := False;
  if not Player.m_boAutoOnline then
  begin
    Player.SendDefMessage(SM_AUTOPLAYGAME_STATE, 1, 0, 0, 0, '');
    Player.m_boAutoOnline := True;
    Result := True;
  end;
end;

// 停止内挂挂机
function _TPlayObject_StopAutoPlayGame(Player: TPlayObject): BOOL; stdcall;
begin
  Result := False;
  if Player.m_boAutoOnline then
  begin
    Player.SendDefMessage(SM_AUTOPLAYGAME_STATE, 0, 0, 0, 0, '');
    Player.m_boAutoOnline := False;
    Result := True;
  end;
end;

function _TPlayObject_GetHeroM2ShopList(Player: TPlayObject): TList; stdcall;
begin
  Result := Player.m_HeroM2ShopList;
end;

function _TPlayObject_GetHeroM2ShopOpenList(Player: TPlayObject): TList; stdcall;
begin
  Result := Player.m_HeroM2ShopOpenList;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 假人是否开始挂机
function _TDummyObject_IsStart(Dummyer: TDummyObject): BOOL; stdcall;
begin
  Result := Dummyer.m_boStart;
end;

// 假人开始挂机
procedure _TDummyObject_Start(Dummyer: TDummyObject); stdcall;
begin
  Dummyer.Start;
end;

// 假人停止挂机
procedure _TDummyObject_Stop(Dummyer: TDummyObject); stdcall;
begin
  Dummyer.Stop;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 攻击模式
function _THeroObject_GetAttackMode(Hero: THeroObject): Byte; stdcall;
begin
  Result := Hero.m_btAttackMode;
end;

function _THeroObject_SetAttackMode(Hero: THeroObject; Value: Byte; ShowSysMsg: BOOL): BOOL; stdcall;
var
  I: Integer;
  Obj: TBaseObject;
begin
  Result := False;

  if g_nKey_HeroExt = 1 then
  begin
    if Hero.m_btAttackMode >= 3 then
      Exit;

    if g_Config.boHeroStatus[Value] then
    begin
      Hero.m_btAttackMode := Value;
      Hero.m_boSlaveRelax := Hero.m_btAttackMode = 2;
      Hero.m_boProtectStatus := False;
      Result := True;
    end;
  end
  else
  begin
    if Hero.m_btAttackMode >= 2 then
      Exit;

    Hero.m_btAttackMode := Value;
    Hero.m_boSlaveRelax := Hero.m_btAttackMode = 2;
    Hero.m_boProtectStatus := False;

    Result := True;
  end;

  if Result then
  begin
    case Hero.m_btAttackMode of
      0:
        begin
          Hero.SysMsg(g_sHeroAttack, clWhite, 252, t_Hint, False);
        end;
      1:
        begin
          // 英雄跟随时宝宝不打怪 chongchong 2015-05-17
          for I := 0 to Hero.m_SlaveList.Count - 1 do
          begin
            Obj := Hero.m_SlaveList[I];
            if Obj.m_TargetCret <> nil then
              Obj.DelTargetCreat;
          end;
          Hero.SysMsg(g_sHeroFollow, clWhite, 252, t_Hint, False);
        end;
      2:
        begin
          // 英雄休息时宝宝不打怪 chongchong 2015-05-17
          for I := 0 to Hero.m_SlaveList.Count - 1 do
          begin
            Obj := Hero.m_SlaveList[I];
            if Obj.m_TargetCret <> nil then
              Obj.DelTargetCreat;
          end;

          Hero.SysMsg(g_sHeroRest, clWhite, 252, t_Hint, False);
        end;
      3:
        begin
          Hero.SysMsg(g_sHeroFollowAttack, clWhite, 252, t_Hint, False);
        end;
    end;
  end;
end;

// 切换下一个攻击模式
procedure _THeroObject_SetNextAttackMode(Hero: THeroObject); stdcall;
begin
  Hero.RestHero;
end;

// 获取背包数量
function _THeroObject_GetBagCount(Hero: THeroObject): Integer; stdcall;
begin
  Result := Hero.m_nBagCount;
end;

// 当前怒气值
function _THeroObject_GetAngryValue(Hero: THeroObject): Integer; stdcall;
begin
  Result := Hero.m_btAngryValue;
end;

// 忠诚度
function _THeroObject_GetLoyalPoint(Hero: THeroObject): Real; stdcall;
begin
  Result := Hero.m_rLoyalPoint;
end;

procedure _THeroObject_SetLoyalPoint(Hero: THeroObject; Value: Real); stdcall;
begin
  Hero.m_rLoyalPoint := Value;
end;

procedure _THeroObject_SendLoyalPointChanged(Hero: THeroObject); stdcall;
begin
  Hero.SendLoyalPoint;
end;

// 是否副将英雄
function _THeroObject_IsDeputy(Hero: THeroObject): BOOL; stdcall;
begin
  Result := Hero.m_boIsDeputy;
end;

// 主人名称
function _THeroObject_GetMasterName(Hero: THeroObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Hero.m_sMasterName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

function _THeroObject_GetQuestFlagStatus(Hero: THeroObject; nFlag: Integer): Integer; stdcall;
begin
  Result := Hero.GetQuestFlagStatus(nFlag);
end;

procedure _THeroObject_SetQuestFlagStatus(Hero: THeroObject; nFlag: Integer; Value: Integer); stdcall;
begin
  Hero.SetQuestFlagStatus(nFlag, Value);
end;

// 发送身上装备
procedure _THeroObject_SendUseItems(Hero: THeroObject); stdcall;
begin
  Hero.SendUseItems;
end;

// 刷新英雄背包
procedure _THeroObject_SendBagItems(Hero: THeroObject); stdcall;
begin
  Hero.DoQueryBagItems;
end;

// 发送首饰盒物品
procedure _THeroObject_SendJewelryBoxItems(Hero: THeroObject); stdcall;
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendJewelryBox(True);
  end;
end;

// 发送神佑袋物品
procedure _THeroObject_SendGodBlessItems(Hero: THeroObject); stdcall;
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendUpdateGodBless(True);
  end;
end;

// 神佑格开启
procedure _THeroObject_SendOpenGodBlessItem(Hero: THeroObject; Index: Integer); stdcall;
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendOpenGodBlessItem(True, Index);
  end;
end;

// 神佑格关闭
procedure _THeroObject_SendCloseGodBlessItem(Hero: THeroObject; Index: Integer); stdcall;
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendCloseGodBlessItem(True, Index);
  end;
end;

// 发送增加物品
procedure _THeroObject_SendAddItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
begin
  Hero.SendAddItem(UserItem);
end;

// 客户端删除物品
procedure _THeroObject_SendDelItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
begin
  Hero.SendDelItem(UserItem);
end;

// 客户端刷新物品
procedure _THeroObject_SendUpdateItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
begin
  Hero.SendUpdateItem(UserItem);
end;

// 客户端刷新装备持久改变
procedure _THeroObject_SendItemDuraChange(Hero: THeroObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;
begin
  Hero.SendMsg(Hero, RM_DURACHANGE, ItemWhere, UserItem.Dura, UserItem.DuraMax, 0, '');
end;

// 发送技能列表
procedure _THeroObject_SendUseMagics(Hero: THeroObject); stdcall;
begin
  Hero.SendUseMagic;
end;

// 发送技能添加
procedure _THeroObject_SendAddMagic(Hero: THeroObject; UserMagic: pTUserMagic); stdcall;
begin
  Hero.SendAddMagic(UserMagic);
end;

// 发送技能删除
procedure _THeroObject_SendDelMagic(Hero: THeroObject; UserMagic: pTUserMagic); stdcall;
begin
  Hero.SendDelMagic(UserMagic);
end;

// 取得合击技能
function _THeroObject_FindGroupMagic(Hero: THeroObject; UserMagic: pTUserMagic): BOOL; stdcall;
var
  Temp: pTUserMagic;
begin
  Result := False;
  Temp := Hero.FindGroupMagic;
  if Temp <> nil then
  begin
    UserMagic^ := Temp^;
    Result := True;
  end;
end;

// 取得合击技能ID
function _THeroObject_GetGroupMagicId(Hero: THeroObject): Integer; stdcall;
begin
  Result := Hero.GetGroupMagicId;
end;

// 发送封号物品
procedure _THeroObject_SendFengHaoItems(Hero: THeroObject); stdcall;
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendUpdateFengHao(True);
  end;
end;

// 发送封号增加
procedure _THeroObject_SendAddFengHaoItem(Hero: THeroObject; UserItem: pTUserItem); stdcall;
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendAddFengHaoItem(UserItem, True);
  end;
end;

procedure _THeroObject_SendDelFengHaoItem(Hero: THeroObject; Index: Integer); stdcall; // 发送封号删除
begin
  if (Hero.m_Master <> nil) and (Hero.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    TPlayObject(Hero.m_Master).SendDelFengHaoItem(Index, True);
  end;
end;

procedure _THeroObject_IncExp(Hero: THeroObject; dwExp: DWORD); stdcall;
begin
  Hero.IncExp(dwExp);
end;

procedure _THeroObject_IncExpNG(Hero: THeroObject; dwExp: DWORD); stdcall;
begin
  Hero.IncExpNG(dwExp);
end;

function _THeroObject_IsOldClient(Hero: THeroObject): BOOL; stdcall;
begin
  Result := False;
end;


// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 创建NPC，该NPC由引擎自动释放
function _TNormNpc_Create(sCharName, sMapName, sScript: PAnsiChar; X, Y: Integer; wAppr: Word; boIsHide: BOOL): TNormNpc; stdcall;
var
  Merchant: TMerchant;
begin
  Merchant := TMerchant.Create;
  UserEngine.AddMerchant(Merchant);
  Merchant.m_sMapName := sMapName;
  Merchant.m_PEnvir := g_MapManager.FindMap(Merchant.m_sMapName);
  Merchant.m_nCurrX := X;
  Merchant.m_nCurrY := Y;
  Merchant.m_sCharName := sCharName;
  Merchant.m_nFlag := 0;
  Merchant.m_wAppr := wAppr;
  Merchant.m_sFilePath := sMarket_Def;
  Merchant.m_sScript := sScript;
  Merchant.m_boIsHide := boIsHide;
  Merchant.m_boIsQuest := False;
  Merchant.m_boPlug := True;
  if not Merchant.m_boIsHide then
    Merchant.Initialize;
  Result := Merchant;
end;

// 载入脚本
procedure _TNormNpc_LoadNpcScript(NormNpc: TNormNpc); stdcall;
begin
  NormNpc.LoadNpcScript;
end;

// 清脚本
procedure _TNormNpc_ClearScript(NormNpc: TNormNpc); stdcall;
begin
  NormNpc.ClearScript;
end;

function _TNormNpc_GetFilePath(NormNpc: TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := NormNpc.m_sFilePath;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TNormNpc_SetFilePath(NormNpc: TNormNpc; Value: PAnsiChar); stdcall;
begin
  NormNpc.m_sFilePath := Value;
end;

function _TNormNpc_GetPath(NormNpc: TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := NormNpc.m_sPath;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TNormNpc_SetPath(NormNpc: TNormNpc; Value: PAnsiChar); stdcall;
begin
  NormNpc.m_sPath := Value;
end;

function _TNormNpc_GetIsHide(NormNpc: TNormNpc): BOOL; stdcall;
begin
  Result := NormNpc.m_boIsHide;
end;

procedure _TNormNpc_SetIsHide(NormNpc: TNormNpc; Value: BOOL); stdcall;
begin
  NormNpc.m_boIsHide := Value;
end;

function _TNormNpc_GetIsQuest(NormNpc: TNormNpc): BOOL; stdcall;
begin
  Result := NormNpc.m_boIsQuest;
end;

function _TNormNpc_GetLineVariableText(NormNpc: TNormNpc; Player: TPlayObject; sMsg: PAnsiChar; Dest: PAnsiChar;
  var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
  IsBreakParseVar: Boolean;
begin
  Result := False;
  IsBreakParseVar := False;
  S := NormNpc.GetLineVariableText(Player, sMsg, IsBreakParseVar);
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

procedure _TNormNpc_GotoLable(NormNpc: TNormNpc; Player: TPlayObject; sLabel: PAnsiChar; boExtJmp: BOOL); stdcall;
begin
  NormNpc.GotoLable(Player, sLabel, boExtJmp, False);
end;

procedure _TNormNpc_SendMsgToUser(NormNpc: TNormNpc; Player: TPlayObject; sMsg: PAnsiChar); stdcall;
begin
  NormNpc.SendMsgToUser(Player, sMsg, False);
end;

procedure _TNormNpc_MessageBox(NormNpc: TNormNpc; Player: TPlayObject; sMsg: PAnsiChar); stdcall;
begin
  NormNpc.MessageBox(Player, sMsg);
end;

function _TNormNpc_GetVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar;
  var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;
var
  S: string;
  AnsiS: AnsiString;
begin
  Result := False;
  if NormNpc.GetValNameValue(Player, sVarName, S, nValue) then
  begin
    AnsiS := S;
    if sValueSize >= Length(AnsiS) then
    begin
      Move(S[1], sValue^, Length(AnsiS));
      sValueSize := Length(AnsiS);
      Result := True;
    end;
  end;
end;

function _TNormNpc_SetVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; nValue: Integer)
  : BOOL; stdcall;
begin
  Result := NormNpc.SetValNameValue(Player, sVarName, sValue, nValue);
end;

function _TNormNpc_GetDynamicVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar;
  var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;
var
  S: string;
  AnsiS: AnsiString;
begin
  Result := False;
  if NormNpc.GetDynamicValue(Player, sVarName, S, nValue) then
  begin
    AnsiS := S;
    if sValueSize >= Length(AnsiS) then
    begin
      Move(S[1], sValue^, Length(AnsiS));
      sValueSize := Length(AnsiS);
      Result := True;
    end;
  end;
end;

function _TNormNpc_SetDynamicVarValue(NormNpc: TNormNpc; Player: TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar;
  nValue: Integer): BOOL; stdcall;
begin
  Result := NormNpc.SetDynamicValue(Player, sVarName, sValue, nValue);
end;

// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

function _TMagicACList_Count(List: TMagicACList): Integer; stdcall;
begin
  Result := List.Count;
end;

function _TMagicACList_GetItem(List: TMagicACList; Index: Integer): PMagicACInfo; stdcall;
begin
  Result := List.Items[Index];
end;

function _TMagicACList_FindByMagIdx(List: TMagicACList; MagIdx: Integer): PMagicACInfo; stdcall;
begin
  Result := List.Get(MagIdx);
end;


// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------

// 获取所有在线人物列表(含假人)
function _TUserEngine_GetPlayerList(): TStringList; stdcall;
begin
  Result := UserEngine.m_PlayObjectList;
end;

// 根据在线人物名称获取对象
function _TUserEngine_GetPlayerByName(ChrName: PAnsiChar): TPlayObject; stdcall;
begin
  Result := UserEngine.GetPlayObject(ChrName);
end;

// 根据在线帐户获取对象
function _TUserEngine_GetPlayerByUserID(UserID: PAnsiChar): TPlayObject; stdcall;
begin
  Result := UserEngine.GetPlayObjectOfAccount(UserID);
end;

// 判断对象是否是一个合法的在线人物
function _TUserEngine_GetPlayerByObject(AObject: TObject): TPlayObject; stdcall;
begin
  Result := UserEngine.GetPlayObject(AObject);
end;

// 根据帐户获取一个离线挂机对象
function _TUserEngine_GetOfflinePlayer(UserID: PAnsiChar): TPlayObject; stdcall;
begin
  Result := UserEngine.GetPlayObjectExOfOffLine(UserID);
end;

// 踢人
procedure _TUserEngine_KickPlayer(ChrName: PAnsiChar); stdcall;
begin
  UserEngine.KickOnlineUser(ChrName);
end;

// 获取英雄列表
function _TUserEngine_GetHeroList(): TStringList; stdcall;
begin
  Result := UserEngine.m_HeroObjectList;
end;

// 根据名称获取英雄对象
function _TUserEngine_GetHeroByName(ChrName: PAnsiChar): THeroObject; stdcall;
begin
  Result := UserEngine.GetHeroObject(ChrName);
end;

// 踢英雄
function _TUserEngine_KickHero(ChrName: PAnsiChar): BOOL; stdcall;
begin
  UserEngine.KickOnlineHero(ChrName);
  Result := True;
end;

// 获取交易NPC列表
function _TUserEngine_GetMerchantList(): TList; stdcall;
begin
  Result := UserEngine.m_MerchantList;
end;

// 获取自定义NPC配置列表
function _TUserEngine_GetCustomNpcConfigList(): TList; stdcall;
begin
  Result := UserEngine.m_CustomMonsterList;
end;

// 获取MapQuest.txt中定义的NPC列表
function _TUserEngine_GetQuestNPCList(): TStringList; stdcall;
begin
  Result := UserEngine.QuestNPCList;
end;

function _TUserEngine_GetManageNPC(): TNormNpc; stdcall;
begin
  Result := g_ManageNPC;
end;

function _TUserEngine_GetFunctionNPC(): TNormNpc; stdcall;
begin
  Result := g_FunctionNPC;
end;

function _TUserEngine_GetRobotNPC(): TNormNpc; stdcall;
begin
  Result := g_RobotNPC;
end;

function _TUserEngine_MissionNPC(): TNormNpc; stdcall;
begin
  Result := g_MissionNPC;
end;

// 判断NPC对象是否合法
function _TUserEngine_FindMerchant(AObject: TObject): TNormNpc; stdcall;
begin
  Result := UserEngine.FindMerchant(AObject);
end;

// 根据地图坐标得到NPC
function _TUserEngine_FindMerchantByPos(MapName: PAnsiChar; nX, nY: Integer): TNormNpc; stdcall;
begin
  Result := UserEngine.FindMerchantByPosition(MapName, nX, nY);
end;

// 判断NPC对象是否合法
function _TUserEngine_FindQuestNPC(AObject: TObject): TNormNpc; stdcall;
begin
  Result := UserEngine.FindNPC(AObject);
end;

// Magic.DB
function _TUserEngine_GetMagicList(): TList; stdcall;
begin
  Result := UserEngine.m_MagicList;
end;

// 自定义技能配置列表
function _TUserEngine_GetCustomMagicConfigList(): TList; stdcall;
begin
  Result := UserEngine.m_CustomMagicList;
end;

// M2 -> 功能设置 ->技能魔法 -> 技能破防百分比
function _TUserEngine_GetMagicACList(): TMagicACList; stdcall;
begin
  Result := UserEngine.m_MagicACList;
end;

function _TUserEngine_FindMagicByName(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  Temp := UserEngine.FindMagicEx(MagName);
  if Temp <> nil then
  begin
    Magic^ := Temp^;
    Result := True;
  end;
end;

function _TUserEngine_FindMagicByIndex(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  Temp := UserEngine.FindMagic(MagIdx);
  if Temp <> nil then
  begin
    Magic^ := Temp^;
    Result := True;
  end;
end;

function _TUserEngine_FindMagicByNameEx(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;

  if (MagAttr >= Integer(Low(TMagicAttr))) and (MagAttr <= Integer(High(TMagicAttr))) then
  begin
    Temp := UserEngine.FindMagic(MagName, TMagicAttr(MagAttr));
    if Temp <> nil then
    begin
      Magic^ := Temp^;
      Result := True;
    end;
  end;
end;

function _TUserEngine_FindMagicByIndexEx(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  if (MagAttr >= Integer(Low(TMagicAttr))) and (MagAttr <= Integer(High(TMagicAttr))) then
  begin
    Temp := UserEngine.FindMagic(MagIdx, TMagicAttr(MagAttr));
    if Temp <> nil then
    begin
      Magic^ := Temp^;
      Result := True;
    end;
  end;
end;

function _TUserEngine_FindHeroMagicByName(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  Temp := UserEngine.FindHeroMagic(MagName);
  if Temp <> nil then
  begin
    Magic^ := Temp^;
    Result := True;
  end;
end;

function _TUserEngine_FindHeroMagicByIndex(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  Temp := UserEngine.FindHeroMagic(MagIdx);
  if Temp <> nil then
  begin
    Magic^ := Temp^;
    Result := True;
  end;
end;

function _TUserEngine_FindHeroMagicByNameEx(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  if (MagAttr >= Integer(Low(TMagicAttr))) and (MagAttr <= Integer(High(TMagicAttr))) then
  begin
    Temp := UserEngine.FindHeroMagic(MagName, TMagicAttr(MagAttr));
    if Temp <> nil then
    begin
      Magic^ := Temp^;
      Result := True;
    end;
  end;
end;

function _TUserEngine_FindHeroMagicByIndexEx(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;
var
  Temp: pTMagic;
begin
  Result := False;
  if (MagAttr >= Integer(Low(TMagicAttr))) and (MagAttr <= Integer(High(TMagicAttr))) then
  begin
    Temp := UserEngine.FindHeroMagic(MagIdx, TMagicAttr(MagAttr));
    if Temp <> nil then
    begin
      Magic^ := Temp^;
      Result := True;
    end;
  end;
end;

// StdItem.DB
function _TUserEngine_GetStdItemList(): TList; stdcall;
begin
  Result := UserEngine.StdItemList;
end;

function _TUserEngine_GetStdItemByName(ItemName: PAnsiChar; StdItem: PTStdItem): BOOL; stdcall;
var
  Temp: PTStdItem;
begin
  Result := False;
  Temp := UserEngine.GetStdItem(ItemName);
  if Temp <> nil then
  begin
    StdItem^ := Temp^;
    Result := True;
  end;
end;

function _TUserEngine_GetStdItemByIndex(ItemIdx: Integer; StdItem: PTStdItem): BOOL; stdcall;
var
  Temp: PTStdItem;
begin
  Result := False;
  Temp := UserEngine.GetStdItem(ItemIdx);
  if Temp <> nil then
  begin
    StdItem^ := Temp^;
    Result := True;
  end;
end;

function _TUserEngine_GetStdItemName(ItemIdx: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := UserEngine.GetStdItemName(ItemIdx);
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

function _TUserEngine_GetStdItemIndex(ItemName: PAnsiChar): Integer; stdcall;
begin
  Result := UserEngine.GetStdItemIdx(ItemName);
end;

// Monster.DB
function _TUserEngine_MonsterList(): TList; stdcall;
begin
  Result := UserEngine.MonsterList;
end;

function _TUserEngine_SendBroadCastMsg(sMsg: PAnsiChar; FColor, BColor: Integer; MsgType: Integer): BOOL; stdcall;
begin
  Result := False;
  if (MsgType >= Integer(Low(TMsgType))) and (MsgType <= Integer(High(TMsgType))) then
  begin
    UserEngine.SendBroadCastMsg(sMsg, FColor, BColor, TMsgType(MsgType));
    Result := True;
  end;
end;

function _TUserEngine_SendBroadCastMsgExt(sMsg: PAnsiChar; MsgType: Integer): BOOL; stdcall;
begin
  Result := False;
  if (MsgType >= Integer(Low(TMsgType))) and (MsgType <= Integer(High(TMsgType))) then
  begin
    UserEngine.SendBroadCastMsgExt(sMsg, TMsgType(MsgType));
    Result := True;
  end;
end;

function _TUserEngine_SendTopBroadCastMsg(sMsg: PAnsiChar; FColor, BColor: Integer; nTime: Integer; MsgType: Integer)
  : BOOL; stdcall;
begin
  Result := False;
  if (MsgType >= Integer(Low(TMsgType))) and (MsgType <= Integer(High(TMsgType))) then
  begin
    UserEngine.SendTopBroadCastMsg(sMsg, FColor, BColor, nTime, TMsgType(MsgType));
    Result := True;
  end;
end;

procedure _TUserEngine_SendMoveMsg(sMsg: PAnsiChar; btFColor, btBColor: Byte; nY, nMoveCount: Integer; nFontSize: Integer;
  nMarqueeTime: Integer); stdcall;
begin
  UserEngine.SendMoveMsg(sMsg, btFColor, btBColor, nY, nMoveCount, nFontSize, nMarqueeTime);
end;

procedure _TUserEngine_SendCenterMsg(sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;
begin
  UserEngine.SendCenterMsg(sMsg, btFColor, btBColor, nTime);
end;

// 换行消息 piaoyun 2013-08-03
procedure _TUserEngine_SendNewLineMsg(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte;
  nY, nShowMsgTime, nDrawType: Integer); stdcall;
begin
  UserEngine.SendNewLineMsg(sMsg, btFColor, btBColor, btFontSize, 0, nY, nShowMsgTime, nDrawType);
end;

// 仿盛大顶部渐隐消息 piaoyun 2013-08-01
procedure _TUserEngine_SendSuperMoveMsg(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte; nY, nMoveCount: Integer); stdcall;
begin
  UserEngine.SendSuperMoveMsg(sMsg, btFColor, btBColor, btFontSize, 0, nY, nMoveCount);
end;

// 发送屏幕震动消息 piaoyun 2013-09-14
procedure _TUserEngine_SendSceneShake(Count: Integer); stdcall;
begin
  UserEngine.SendSceneShake(Count);
end;

function _TUserEngine_CopyToUserItemFromName(ItemName: PAnsiChar; UserItem: pTUserItem): BOOL; stdcall;
begin
  Result := UserEngine.CopyToUserItemFromName(ItemName, UserItem);
end;

function _TUserEngine_CopyToUserItemFromItem(StdItem: PTStdItem; ItemIndex: Integer; UserItem: pTUserItem): BOOL; stdcall;
begin
  Result := UserEngine.CopyToUserItemFromItem(StdItem, ItemIndex, UserItem);
end;

procedure _TUserEngine_RandomUpgradeItem(UserItem: pTUserItem); stdcall;
begin
  UserEngine.RandomUpgradeItem(UserItem);
end;

// 元素
procedure _TUserEngine_RandomItemNewAbil(UserItem: pTUserItem); stdcall;
begin
  UserEngine.RandomItemNewAbil(0, UserItem);
end;

procedure _TUserEngine_GetUnknowItemValue(UserItem: pTUserItem); stdcall;
begin
  UserEngine.GetUnknowItemValue(UserItem);
end;

function _TUserEngine_GetAllDummyCount(): Integer; stdcall;
begin
  Result := UserEngine.GetDummyObjectCount;
end;

function _TUserEngine_GetMapDummyCount(Envir: TEnvirnoment): Integer; stdcall;
begin
  Result := UserEngine.GetDummyObjectCount(Envir);
end;

function _TUserEngine_GetOfflineCount(): Integer; stdcall;
begin
  Result := UserEngine.GetOfflineCount;
end;

function _TUserEngine_GetRealPlayerCount(): Integer; stdcall;
begin
  Result := UserEngine.GetReallyCount;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 根据行会名得到行会对象
function _TGuildManager_FindGuild(GuildName: PAnsiChar): TGUild; stdcall;
begin
  Result := g_GuildManager.FindGuild(GuildName);
end;

// 根据用户名得到行会对象
function _TGuildManager_GetPlayerGuild(CharName: PAnsiChar): TGUild; stdcall;
begin
  Result := g_GuildManager.MemberOfGuild(CharName);
end;

// 创建新行会
function _TGuildManager_AddGuild(GuildName, GuildMaster: PAnsiChar): BOOL; stdcall;
begin
  Result := g_GuildManager.AddGuild(GuildName, GuildMaster)
end;

// 删除行会
function _TGuildManager_DelGuild(GuildName: PAnsiChar; var IsFoundGuild: BOOL): BOOL; stdcall;
var
  _IsFoundGuild: Boolean;
begin
  _IsFoundGuild := IsFoundGuild;
  Result := g_GuildManager.DelGuild(GuildName, _IsFoundGuild);
  IsFoundGuild := _IsFoundGuild;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// 行会名称
function _TGuild_GetGuildName(Guild: TGUild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Guild.sGuildName;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 加入行会职业(1:战士; 2:法师; 3:道士，可组合)
function _TGuild_GetJoinJob(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.JoinJob;
end;

// 加入行会最低等级
function _TGuild_GetJoinLevel(Guild: TGUild): DWORD; stdcall;
begin
  Result := Guild.JoinLevel;
end;

// 招贤消息
function _TGuild_GetJoinMsg(Guild: TGUild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Guild.JoinMsg;
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 建筑度
function _TGuild_GetBuildPoint(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.nBuildPoint;
end;

// 人气值/关注度
function _TGuild_GetAurae(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.nAurae;
end;

// 安定度
function _TGuild_GetStability(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.nStability;
end;

// 繁荣度
function _TGuild_GetFlourishing(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.nFlourishing;
end;

// 领取装备数量
function _TGuild_GetChiefItemCount(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.nChiefItemCount;
end;

// 行会成员数量
function _TGuild_GetMemberCount(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.Count;
end;

// 行会在线成员数量
function _TGuild_GetOnlineMemeberCount(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.OnlineCount;
end;

// 掌门数量
function _TGuild_GetMasterCount(Guild: TGUild): Integer; stdcall;
begin
  Result := Guild.GetGuildMasterCount;
end;

// 得到行会正副掌门
procedure _TGuild_GetMaster(Guild: TGUild; var Master1, Master2: TPlayObject); stdcall;
begin
  Guild.GetGuildMaster(Master1, Master2);
end;

// 得到行会正副掌门名称
function _TGuild_GetMasterName(Guild: TGUild; Master1: PAnsiChar; var Master1Size: DWORD; Master2: PAnsiChar;
  var Master2Size: DWORD): BOOL; stdcall;
var
  S1, S2: string;
  AnsiS1, AnsiS2: AnsiString;
begin
  Result := False;

  Guild.GetGuildMasterName(S1, S2);
  if (Master1Size >= Length(S1)) and (Master2Size >= Length(S2)) then
  begin
    AnsiS1 := S1;
    AnsiS2 := S2;
    Master1Size := Length(AnsiS1);
    Master2Size := Length(AnsiS2);

    if Length(AnsiS1) > 0 then
    begin
      Move(AnsiS1[1], Master1^, Length(AnsiS1));
    end;

    if Length(AnsiS2) > 0 then
    begin
      Move(AnsiS2[1], Master2^, Length(AnsiS2));
    end;

    Result := True;
  end;
end;

// 检查行会是否满员
function _TGuild_CheckMemberIsFull(Guild: TGUild): BOOL; stdcall;
begin
  Result := Guild.IsFull;
end;

// 检查人员是否为行会成员
function _TGuild_IsMemeber(Guild: TGUild; CharName: PAnsiChar): BOOL; stdcall;
begin
  Result := Guild.IsMember(CharName);
end;

// 人员加入行会
function _TGuild_AddMember(Guild: TGUild; Player: TPlayObject): BOOL; stdcall;
begin
  Result := Guild.AddMember(Player);
end;

function _TGuild_AddMemberEx(Guild: TGUild; CharName: PAnsiChar): BOOL; stdcall;
begin
  Result := Guild.AddMember2(CharName);
end;

// 行会删除人员
function _TGuild_DelMemeber(Guild: TGUild; Player: TPlayObject): BOOL; stdcall;
begin
  Result := Guild.DelHumanObj(Player);
end;

function _TGuild_DelMemeberEx(Guild: TGUild; CharName: PAnsiChar): BOOL; stdcall;
begin
  Result := Guild.DelMember(CharName);
end;

// 判断 CheckGuild是否是Guild的联盟行会
function _TGuild_IsAllianceGuild(Guild: TGUild; CheckGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.IsAllyGuild(CheckGuild);
end;

// 判断是否为战争行会
function _TGuild_IsWarGuild(Guild: TGUild; CheckGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.IsWarGuild(CheckGuild);
end;

// 判断是否为关注行会
function _TGuild_IsAttentionGuild(Guild: TGUild; CheckGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.IsAttentionGuild(CheckGuild);
end;

// 添加联盟行会
function _TGuild_AddAlliance(Guild: TGUild; AddGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.AddAttentionGuild(AddGuild);
end;

// 添加战争行会
function _TGuild_AddWarGuild(Guild: TGUild; AddGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.AddWarGuild(AddGuild) <> nil;
end;

// 添加关注行会
function _TGuild_AddAttentionGuild(Guild: TGUild; AddGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.AddAttentionGuild(AddGuild);
end;

// 删除联盟行会
function _TGuild_DelAllianceGuild(Guild: TGUild; DelGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.DelAllyGuild(DelGuild);
end;

// 删除关注行会
function _TGuild_DelAttentionGuild(Guild: TGUild; DelGuild: TGUild): BOOL; stdcall;
begin
  Result := Guild.DelAttentionGuild(DelGuild);
end;

function _TGuild_GetRandNameByName(Guild: TGUild; CharName: PAnsiChar; var nRankNo: Integer; Dest: PAnsiChar; var DestLen: DWORD)
  : BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Guild.GetRankName(CharName, nRankNo);
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

function _TGuild_GetRandNameByPlayer(Guild: TGUild; Player: TPlayObject; var nRankNo: Integer; Dest: PAnsiChar;
  var DestLen: DWORD): BOOL; stdcall;
var
  S: AnsiString;
begin
  Result := False;
  S := Guild.GetRankName2(Player, nRankNo);
  if (Dest <> nil) and (DestLen > Length(S)) then
  begin
    Move(S[1], Dest^, Length(S));
    FillChar(PAnsiChar(NativeInt(Dest) + Length(S))^, 1, 0);
    Result := True;
  end;
  DestLen := Length(S);
end;

// 发送行会消息
procedure _TGuild_SendGuildMsg(Guild: TGUild; Msg: PAnsiChar); stdcall;
begin
  Guild.SendGuildMsg(Msg);
end;

end.
