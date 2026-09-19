// =====================================================================================
// PluginInterface.pas 1:1 转换：托管接口层（19 个 I*Func 接口 + 各记录的保留槽位属性）。
//
// 与 PluginInterfaceTables.g.cs 中的 T*Func 记录一一对应：接口的每个成员即该记录的一个
// 函数指针字段，参数/返回值语义完全相同（PAnsiChar→byte[]、var DestLen→ref uint、
// pT*→ref T*、BOOL→bool）；`Reserved: array[0..N-1] of Pointer` 以 IntPtr 属性表达。
// 原生插件按 ABI 委托实现，托管插件按本接口实现，两者由 PluginAssemblyLoader 装载。
// =====================================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Plugins;

/// <summary>原文 TMemoryFunc 对应的托管接口 IMemoryFunc（4 个成员）。</summary>
public interface IMemoryFunc
{
    IntPtr Allow(int pSize);        // 原 2344  Allow: TMemory_Alloc
    void Free(IntPtr P);        // 原 2345  Free: TMemory_Free
    void Realloc(IntPtr P, int pSize);        // 原 2346  Realloc: TMemory_Realloc
}

/// <summary>原文 TListFunc 对应的托管接口 IListFunc（14 个成员）。</summary>
public interface IListFunc
{
    IListHandle Create();        // 原 2351  Create: TList_Create
    void Free(IListHandle pList);        // 原 2352  Free: TList_Free
    int Count(IListHandle pList);        // 原 2353  Count: TList_Count
    void Clear(IListHandle pList);        // 原 2354  Clear: TList_Clear
    void Add(IListHandle pList, IntPtr pItem);        // 原 2355  Add: TList_Add
    void Insert(IListHandle pList, int pIndex, IntPtr pItem);        // 原 2356  Insert: TList_Insert
    void Remove(IListHandle pList, IntPtr pItem);        // 原 2357  Remove: TList_Remove
    void Delete(IListHandle pList, int pIndex);        // 原 2358  Delete: TList_Delete
    IntPtr GetItem(IListHandle pList, int pIndex);        // 原 2359  GetItem: TList_GetItem
    void SetItem(IListHandle pList, int pIndex, IntPtr pItem);        // 原 2360  SetItem: TList_SetItem
    int IndexOf(IListHandle pList, IntPtr pItem);        // 原 2361  IndexOf: TList_IndexOf
    void Exchange(IListHandle pList, int Index1, int Index2);        // 原 2362  Exchange: TList_Exchange
    void CopyTo(IListHandle Source, IListHandle Dest);        // 原 2363  CopyTo: TList_CopyTo
}

/// <summary>原文 TStringListFunc 对应的托管接口 IStringListFunc（29 个成员）。</summary>
public interface IStringListFunc
{
    IStringListHandle Create();        // 原 2368  Create: TStrList_Create
    void Free(IStringListHandle Strings);        // 原 2369  Free: TStrList_Free
    bool GetCaseSensitive(IStringListHandle Strings);        // 原 2370  GetCaseSensitive: TStrList_GetCaseSensitive
    void SetCaseSensitive(IStringListHandle Strings, bool IsCaseSensitive);        // 原 2371  SetCaseSensitive: TStrList_SetCaseSensitive
    bool GetSorted(IStringListHandle Strings);        // 原 2372  GetSorted: TStrList_GetSorted
    void SetSorted(IStringListHandle Strings, bool Sorted);        // 原 2373  SetSorted: TStrList_SetSorted
    bool GetDuplicates(IStringListHandle Strings);        // 原 2374  GetDuplicates: TStrList_GetDuplicates
    void SetDuplicates(IStringListHandle Strings, bool Duplicates);        // 原 2375  SetDuplicates: TStrList_SetDuplicates
    int Count(IStringListHandle Strings);        // 原 2376  Count: TStrList_Count
    bool GetText(IStringListHandle Strings, byte[] Dest, uint DestLen);        // 原 2377  GetText: TStrList_GetText
    void SetText(IStringListHandle Strings, byte[] Src, uint SrcLen);        // 原 2378  SetText: TStrList_SetText
    void Add(IStringListHandle Strings, byte[] S);        // 原 2379  Add: TStrList_Add
    void AddObject(IStringListHandle Strings, byte[] S, object AObject);        // 原 2380  AddObject: TStrList_AddObject
    void Insert(IStringListHandle Strings, int pIndex, byte[] S);        // 原 2381  Insert: TStrList_Insert
    void InsertObject(IStringListHandle Strings, int pIndex, byte[] S, object AObject);        // 原 2382  InsertObject: TStrList_InsertObject
    void Remove(IStringListHandle Strings, byte[] S);        // 原 2383  Remove: TStrList_Remove
    void Delete(IStringListHandle Strings, int pIndex);        // 原 2384  Delete: TStrList_Delete
    bool GetItem(IStringListHandle Strings, int pIndex, byte[] Dest, uint DestLen);        // 原 2385  GetItem: TStrList_GetItem
    void SetItem(IStringListHandle Strings, int pIndex, byte[] S);        // 原 2386  SetItem: TStrList_SetItem
    object GetObject(IStringListHandle Strings, int pIndex);        // 原 2387  GetObject: TStrList_GetObject
    void SetObject(IStringListHandle Strings, int pIndex, object AObject);        // 原 2388  SetObject: TStrList_SetObject
    int IndexOf(IStringListHandle Strings, byte[] S);        // 原 2389  IndexOf: TStrList_IndexOf
    int IndexOfObject(IStringListHandle Strings, object AObject);        // 原 2390  IndexOfObject: TStrList_IndexOfObject
    bool Find(IStringListHandle Strings, byte[] S, int pIndex);        // 原 2391  Find: TStrList_Find
    void Exchange(IStringListHandle Strings, int Index1, int Index2);        // 原 2392  Exchange: TStrList_Exchange
    void LoadFromFile(IStringListHandle Strings, byte[] FileName);        // 原 2393  LoadFromFile: TStrLit_LoadFromFile
    void SaveToFile(IStringListHandle Strings, byte[] FileName);        // 原 2394  SaveToFile: TStrLit_SaveToFile
    void CopyTo(IStringListHandle Source, IStringListHandle Dest);        // 原 2395  CopyTo: TStrList_CopyTo
}

/// <summary>原文 TMemoryStreamFunc 对应的托管接口 IMemoryStreamFunc（14 个成员）。</summary>
public interface IMemoryStreamFunc
{
    IMemoryStreamHandle Create();        // 原 2400  Create: TMemStream_Create
    void Free(IMemoryStreamHandle pStream);        // 原 2401  Free: TMemStream_Free
    long GetSize(IMemoryStreamHandle pStream);        // 原 2402  GetSize: TMemStream_GetSize
    void SetSize(IMemoryStreamHandle pStream, int NewSize);        // 原 2403  SetSize: TMemStream_SetSize
    void Clear(IMemoryStreamHandle pStream);        // 原 2404  Clear: TMemStream_Clear
    int Read(IMemoryStreamHandle pStream, byte[] Buffer, int pCount);        // 原 2405  Read: TMemStream_Read
    int Write(IMemoryStreamHandle pStream, byte[] Buffer, int pCount);        // 原 2406  Write: TMemStream_Write
    int Seek(IMemoryStreamHandle pStream, int Offset, ushort Origin);        // 原 2407  Seek: TMemStream_Seek
    IntPtr Memory(IMemoryStreamHandle pStream);        // 原 2408  Memory: TMemStream_Memory
    long GetPosition(IMemoryStreamHandle pStream);        // 原 2409  GetPosition: TMemStream_GetPosition
    void SetPosition(IMemoryStreamHandle pStream, long Position);        // 原 2410  SetPosition: TMemStream_SetPosition
    void LoadFromFile(IMemoryStreamHandle pStream, byte[] FileName);        // 原 2411  LoadFromFile: TMemStream_LoadFromFile
    void SaveToFile(IMemoryStreamHandle pStream, byte[] FileName);        // 原 2412  SaveToFile: TMemStream_SaveToFile
}

/// <summary>原文 TMemuFunc 对应的托管接口 IMemuFunc（27 个成员）。</summary>
public interface IMemuFunc
{
    IMenuItem GetMainMenu();        // 原 2417  GetMainMenu: TMenu_GetMainMenu
    IMenuItem GetControlMenu();        // 原 2418  GetControlMenu: TMenu_GetControlMenu
    IMenuItem GetViewMenu();        // 原 2419  GetViewMenu: TMenu_GetViewMenu
    IMenuItem GetOptionMenu();        // 原 2420  GetOptionMenu: TMenu_GetOptionMenu
    IMenuItem GetManagerMenu();        // 原 2421  GetManagerMenu: TMenu_GetManagerMenu
    IMenuItem GetToolsMenu();        // 原 2422  GetToolsMenu: TMenu_GetToolsMenu
    IMenuItem GetHelpMenu();        // 原 2423  GetHelpMenu: TMenu_GetHelpMenu
    IMenuItem GetPluginMenu();        // 原 2424  GetPluginMenu: TMenu_GetPluginMenu
    int Count(IMenuItem MenuItem);        // 原 2425  Count: TMenu_Count
    IMenuItem GetItems(IMenuItem MenuItem, int pIndex);        // 原 2426  GetItems: TMenu_GetItems
    IMenuItem Add(IntPtr PlugID, IMenuItem MenuItem, byte[] Caption, int Tag, Action<object> OnClick);        // 原 2427  Add: TMenu_Add
    IMenuItem Insert(IntPtr PlugID, IMenuItem MenuItem, int pIndex, byte[] Caption, int Tag, Action<object> OnClick);        // 原 2428  Insert: TMenu_Insert
    bool GetCaption(IMenuItem MenuItem, byte[] Dest, uint DestLen);        // 原 2429  GetCaption: TMenu_GetCaption
    void SetCaption(IMenuItem MenuItem, byte[] Caption);        // 原 2430  SetCaption: TMenu_SetCaption
    bool GetEnabled(IMenuItem MenuItem);        // 原 2431  GetEnabled: TMenu_GetEnabled
    void SetEnabled(IMenuItem MenuItem, bool Enabled);        // 原 2432  SetEnabled: TMenu_SetEnabled
    bool GetVisable(IMenuItem MenuItem);        // 原 2433  GetVisable: TMenu_GetVisable
    void SetVisable(IMenuItem MenuItem, bool Visible);        // 原 2434  SetVisable: TMenu_SetVisable
    bool GetChecked(IMenuItem MenuItem);        // 原 2435  GetChecked: TMenu_GetChecked
    void SetChecked(IMenuItem MenuItem, bool pChecked);        // 原 2436  SetChecked: TMenu_SetChecked
    bool GetRadioItem(IMenuItem MenuItem);        // 原 2437  GetRadioItem: TMenu_GetRadioItem
    void SetRadioItem(IMenuItem MenuItem, bool IsRadioItem);        // 原 2438  SetRadioItem: TMenu_SetRadioItem
    int GetGroupIndex(IMenuItem MenuItem);        // 原 2439  GetGroupIndex: TMenu_GetGroupIndex
    void SetGroupIndex(IMenuItem MenuItem, int pValue);        // 原 2440  SetGroupIndex: TMenu_SetGroupIndex
    int GetTag(IMenuItem MenuItem);        // 原 2441  GetTag: TMenu_GetTag
    void SetTag(IMenuItem MenuItem, int pValue);        // 原 2442  SetTag: TMenu_SetTag
}

/// <summary>原文 TIniFileFunc 对应的托管接口 IIniFileFunc（11 个成员）。</summary>
public interface IIniFileFunc
{
    IIniFileHandle Create(byte[] sFileName);        // 原 2447  Create: TIniFile_Create
    void Free(IIniFileHandle IniFile);        // 原 2448  Free: TIniFile_Free
    bool SectionExists(IIniFileHandle IniFile, byte[] Section);        // 原 2449  SectionExists: TIniFile_SectionExists
    bool ValueExists(IIniFileHandle IniFile, byte[] Section, byte[] Ident);        // 原 2450  ValueExists: TIniFile_ValueExists
    bool ReadString(IIniFileHandle IniFile, byte[] Section, byte[] Ident, byte[] Default, byte[] Dest, uint DestLen);        // 原 2451  ReadString: TIniFile_ReadString
    void WriteString(IIniFileHandle IniFile, byte[] Section, byte[] Ident, byte[] pValue);        // 原 2452  WriteString: TIniFile_WriteString
    int ReadInteger(IIniFileHandle IniFile, byte[] Section, byte[] Ident, int Default);        // 原 2453  ReadInteger: TIniFile_ReadInteger
    void WriteInteger(IIniFileHandle IniFile, byte[] Section, byte[] Ident, int pValue);        // 原 2454  WriteInteger: TIniFile_WriteInteger
    bool ReadBool(IIniFileHandle IniFile, byte[] Section, byte[] Ident, bool Default);        // 原 2455  ReadBool: TIniFile_ReadBool
    void WriteBool(IIniFileHandle IniFile, byte[] Section, byte[] Ident, bool pValue);        // 原 2456  WriteBool: TIniFile_WriteBool
}

/// <summary>原文 TMagicACListFunc 对应的托管接口 IMagicACListFunc（4 个成员）。</summary>
public interface IMagicACListFunc
{
    int Count(IMagicACListHandle pList);        // 原 2461  Count: TMagicACList_Count
    IntPtr GetItem(IMagicACListHandle pList, int pIndex);        // 原 2462  GetItem: TMagicACList_GetItem
    IntPtr FindByMagIdx(IMagicACListHandle pList, int MagIdx);        // 原 2463  FindByMagIdx: TMagicACList_FindByMagIdx
}

/// <summary>原文 TMapManagerFunc 对应的托管接口 IMapManagerFunc（3 个成员）。</summary>
public interface IMapManagerFunc
{
    IEnvirnoment FindMap(byte[] MapName);        // 原 2468  FindMap: TMapManager_FindMap
    IListHandle GetMapList();        // 原 2469  GetMapList: TMapManager_GetMapList
}

/// <summary>原文 TEnvirnomentFunc 对应的托管接口 IEnvirnomentFunc（26 个成员）。</summary>
public interface IEnvirnomentFunc
{
    bool GetMapName(IEnvirnoment Envir, byte[] Dest, uint DestLen);        // 原 2474  GetMapName: TEnvir_GetMapName
    bool GetMapDesc(IEnvirnoment Envir, byte[] Dest, uint DestLen);        // 原 2475  GetMapDesc: TEnvir_GetMapDesc
    int GetWidth(IEnvirnoment Envir);        // 原 2476  GetWidth: TEnvir_GetWidth
    int GetHeight(IEnvirnoment Envir);        // 原 2477  GetHeight: TEnvir_GetHeight
    int GetMinMap(IEnvirnoment Envir);        // 原 2478  GetMinMap: TEnvir_GetMinMap
    bool IsMainMap(IEnvirnoment Envir);        // 原 2479  IsMainMap: TEnvir_IsMainMap
    bool GetMainMapName(IEnvirnoment Envir, byte[] Dest, uint DestLen);        // 原 2480  GetMainMapName: TEnvir_GetMainMapName
    bool IsMirrMap(IEnvirnoment Envir);        // 原 2481  IsMirrMap: TEnvir_IsMirrMap
    uint GetMirrMapCreateTick(IEnvirnoment Envir);        // 原 2482  GetMirrMapCreateTick: TEnvir_GetMirrMapCreateTick
    uint GetMirrMapSurvivalTime(IEnvirnoment Envir);        // 原 2483  GetMirrMapSurvivalTime: TEnvir_GetMirrMapSurvivalTime
    bool GetMirrMapExitToMap(IEnvirnoment Envir, byte[] Dest, uint DestLen);        // 原 2484  GetMirrMapExitToMap: TEnvir_GetMirrMapExitToMap
    int GetMirrMapMinMap(IEnvirnoment Envir);        // 原 2485  GetMirrMapMinMap: TEnvir_GetMirrMapMinMap
    bool GetAlwaysShowTime(IEnvirnoment Envir);        // 原 2486  GetAlwaysShowTime: TEnvir_GetAlwaysShowTime
    bool IsFBMap(IEnvirnoment Envir);        // 原 2487  IsFBMap: TEnvir_IsFBMap
    bool GetFBMapName(IEnvirnoment Envir, byte[] Dest, uint DestLen);        // 原 2488  GetFBMapName: TEnvir_GetFBMapName
    int GetFBEnterLimit(IEnvirnoment Envir);        // 原 2489  GetFBEnterLimit: TEnvir_GetFBEnterLimit
    bool GetFBCreated(IEnvirnoment Envir);        // 原 2491  GetFBCreated: TEnvir_GetFBCreated
    uint GetFBCreateTime(IEnvirnoment Envir);        // 原 2492  GetFBCreateTime: TEnvir_GetFBCreateTime
    bool GetMapParam(IEnvirnoment Envir, byte[] Param);        // 原 2493  GetMapParam: TEnvir_GetMapParam
    bool GetMapParamValue(IEnvirnoment Envir, byte[] Param, byte[] Dest, uint DestLen);        // 原 2494  GetMapParamValue: TEnvir_GetMapParamValue
    bool CheckCanMove(IEnvirnoment Envir, int nX, int nY, bool boFlag);        // 原 2495  CheckCanMove: TEnvir_CheckCanMove
    bool IsValidObject(IEnvirnoment Envir, int nX, int nY, int nRange, object AObject);        // 原 2496  IsValidObject: TEnvir_IsValidObject
    int GetItemObjects(IEnvirnoment Envir, int nX, int nY, IListHandle ObjectList);        // 原 2497  GetItemObjects: TEnvir_GetItemObjects
    int GetBaseObjects(IEnvirnoment Envir, int nX, int nY, bool IncDeathObject, IListHandle ObjectList);        // 原 2498  GetBaseObjects: TEnvir_GetBaseObjects
    int GetPlayObjects(IEnvirnoment Envir, int nX, int nY, bool IncDeathObject, IListHandle ObjectList);        // 原 2499  GetPlayObjects: TEnvir_GetPlayObjects
}

/// <summary>原文 TM2EngineFunc 对应的托管接口 IM2EngineFunc（27 个成员）。</summary>
public interface IM2EngineFunc
{
    bool GetVersion(byte[] Dest, uint DestLen);        // 原 2504  GetVersion: TM2Engine_GetVersion
    int GetVersionInt();        // 原 2505  GetVersionInt: TM2Engine_GetVersionInt
    IntPtr GetMainFormHandle();        // 原 2506  GetMainFormHandle: TM2Engine_GetMainFormHandle
    void SetMainFormCaption(byte[] Caption);        // 原 2507  SetMainFormCaption: TM2Engine_SetMainFormCaption
    bool GetAppDir(byte[] Dest, uint DestLen);        // 原 2508  GetAppDir: TM2Engine_GetAppDir
    IIniFileHandle GetGlobalIniFile(int M2IniType);        // 原 2509  GetGlobalIniFile: TM2Engine_GetGlobalIniFile
    bool GetOtherFileDir(int M2FileType, byte[] Dest, uint DestLen);        // 原 2510  GetOtherFileDir: TM2Engine_GetOtherFileDir
    void MainOutMessage(byte[] Msg, bool IsAddTime);        // 原 2511  MainOutMessage: TM2Engine_MainOutMessage
    int GetGlobalVarI(int pIndex);        // 原 2512  GetGlobalVarI: TM2Engine_GetGlobalVarI
    bool SetGlobalVarI(int pIndex, int pValue);        // 原 2513  SetGlobalVarI: TM2Engine_SetGlobalVarI
    int GetGlobalVarG(int pIndex);        // 原 2514  GetGlobalVarG: TM2Engine_GetGlobalVarG
    bool SetGlobalVarG(int pIndex, int pValue);        // 原 2515  SetGlobalVarG: TM2Engine_SetGlobalVarG
    bool GetGlobalVarA(int pIndex, byte[] Dest, uint DestLen);        // 原 2516  GetGlobalVarA: TM2Engine_GetGlobalVarA
    bool SetGlobalVarA(int pIndex, byte[] pValue);        // 原 2517  SetGlobalVarA: TM2Engine_SetGlobalVarA
    bool EncodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, uint DestLen);        // 原 2518  EncodeBuffer: TM2Engine_EncodeBuffer
    bool DecodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, uint DestLen);        // 原 2519  DecodeBuffer: TM2Engine_DecodeBuffer
    bool ZLibEncodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, uint DestLen);        // 原 2520  ZLibEncodeBuffer: TM2Engine_ZLibEncodeBuffer
    bool ZLibDecodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, uint DestLen);        // 原 2521  ZLibDecodeBuffer: TM2Engine_ZLibDecodeBuffer
    bool EncryptBuffer(byte[] Src, uint SrcLen, byte[] Dest, uint DestLen);        // 原 2522  EncryptBuffer: TM2Engine_EncryptBuffer
    bool DecryptBuffer(byte[] Src, uint SrcLen, byte[] Dest, uint DestLen);        // 原 2523  DecryptBuffer: TM2Engine_DecryptBuffer
    bool EncryptPassword(byte[] InData, byte[] OutData, uint OutSize);        // 原 2524  EncryptPassword: TM2Engine_EncryptPassword
    bool DecryptPassword(byte[] InData, byte[] OutData, uint OutSize);        // 原 2525  DecryptPassword: TM2Engine_DecryptPassword
    int GetTakeOnPosition(int StdMode);        // 原 2526  GetTakeOnPosition: TM2Engine_GetTakeOnPosition
    bool CheckBindType(byte BindValue, byte BindType);        // 原 2527  CheckBindType: TM2Engine_CheckBindType
    void SetBindValue(byte BindValue, byte BindType, bool pValue);        // 原 2528  SetBindValue: TM2Engine_SetBindValue
    uint GetRGB(byte Color);        // 原 2529  GetRGB: TM2Engine_GetRGB
}

/// <summary>原文 TBaseObjectFunc 对应的托管接口 IBaseObjectFunc（186 个成员）。</summary>
public interface IBaseObjectFunc
{
    bool GetChrName(IBaseObjectHandle BaseObject, byte[] Dest, uint DestLen);        // 原 2534  GetChrName: TBaseObject_GetChrName
    bool SetChrName(IBaseObjectHandle BaseObject, byte[] NewName);        // 原 2535  SetChrName: TBaseObject_SetChrName
    void RefShowName(IBaseObjectHandle BaseObject);        // 原 2536  RefShowName: TBaseObject_RefShowName
    void RefNameColor(IBaseObjectHandle BaseObject);        // 原 2537  RefNameColor: TBaseObject_RefNameColor
    byte GetGender(IBaseObjectHandle BaseObject);        // 原 2538  GetGender: TBaseObject_GetGender
    bool SetGender(IBaseObjectHandle BaseObject, byte Gender);        // 原 2539  SetGender: TBaseObject_SetGender
    byte GetJob(IBaseObjectHandle BaseObject);        // 原 2540  GetJob: TBaseObject_GetJob
    bool SetJob(IBaseObjectHandle BaseObject, byte Job);        // 原 2541  SetJob: TBaseObject_SetJob
    byte GetHair(IBaseObjectHandle BaseObject);        // 原 2542  GetHair: TBaseObject_GetHair
    void SetHair(IBaseObjectHandle BaseObject, byte Hair);        // 原 2543  SetHair: TBaseObject_SetHair
    IEnvirnoment GetEnvir(IBaseObjectHandle BaseObject);        // 原 2544  GetEnvir: TBaseObject_GetEnvir
    bool GetMapName(IBaseObjectHandle BaseObject, byte[] Dest, uint DestLen);        // 原 2545  GetMapName: TBaseObject_GetMapName
    int GetCurrX(IBaseObjectHandle BaseObject);        // 原 2546  GetCurrX: TBaseObject_GetCurrX
    int GetCurrY(IBaseObjectHandle BaseObject);        // 原 2547  GetCurrY: TBaseObject_GetCurrY
    byte GetDirection(IBaseObjectHandle BaseObject);        // 原 2548  GetDirection: TBaseObject_GetDirection
    bool GetHomeMap(IBaseObjectHandle BaseObject, byte[] Dest, uint DestLen);        // 原 2549  GetHomeMap: TBaseObject_GetHomeMap
    int GetHomeX(IBaseObjectHandle BaseObject);        // 原 2550  GetHomeX: TBaseObject_GetHomeX
    int GetHomeY(IBaseObjectHandle BaseObject);        // 原 2551  GetHomeY: TBaseObject_GetHomeY
    byte GetPermission(IBaseObjectHandle BaseObject);        // 原 2552  GetPermission: TBaseObject_GetPermission
    void SetPermission(IBaseObjectHandle BaseObject, byte pValue);        // 原 2553  SetPermission: TBaseObject_SetPermission
    bool GetDeath(IBaseObjectHandle BaseObject);        // 原 2554  GetDeath: TBaseObject_GetDeath
    uint GetDeathTick(IBaseObjectHandle BaseObject);        // 原 2555  GetDeathTick: TBaseObject_GetDeathTick
    bool GetGhost(IBaseObjectHandle BaseObject);        // 原 2556  GetGhost: TBaseObject_GetGhost
    uint GetGhostTick(IBaseObjectHandle BaseObject);        // 原 2557  GetGhostTick: TBaseObject_GetGhostTick
    void MakeGhost(IBaseObjectHandle BaseObject);        // 原 2558  MakeGhost: TBaseObject_MakeGhost
    void ReAlive(IBaseObjectHandle BaseObject);        // 原 2559  ReAlive: TBaseObject_ReAlive
    byte GetRaceServer(IBaseObjectHandle BaseObject);        // 原 2560  GetRaceServer: TBaseObject_GetRaceServer
    ushort GetAppr(IBaseObjectHandle BaseObject);        // 原 2561  GetAppr: TBaseObject_GetAppr
    byte GetRaceImg(IBaseObjectHandle BaseObject);        // 原 2562  GetRaceImg: TBaseObject_GetRaceImg
    int GetCharStatus(IBaseObjectHandle BaseObject);        // 原 2563  GetCharStatus: TBaseObject_GetCharStatus
    void SetCharStatus(IBaseObjectHandle BaseObject, int pValue);        // 原 2564  SetCharStatus: TBaseObject_SetCharStatus
    void StatusChanged(IBaseObjectHandle BaseObject);        // 原 2565  StatusChanged: TBaseObject_StatusChanged
    int GetHungerPoint(IBaseObjectHandle BaseObject);        // 原 2566  GetHungerPoint: TBaseObject_GetHungerPoint
    void SetHungerPoint(IBaseObjectHandle BaseObject, int pValue);        // 原 2567  SetHungerPoint: TBaseObject_SetHungerPoint
    bool IsNGMonster(IBaseObjectHandle BaseObject);        // 原 2568  IsNGMonster: TBaseobject_IsNGMonster
    bool IsDummyObject(IBaseObjectHandle BaseObject);        // 原 2569  IsDummyObject: TBaseObject_IsDummyObject
    int GetViewRange(IBaseObjectHandle BaseObject);        // 原 2570  GetViewRange: TBaseObject_GetViewRange
    void SetViewRange(IBaseObjectHandle BaseObject, int pValue);        // 原 2571  SetViewRange: TBaseObject_SetViewRange
    bool GetAbility(IBaseObjectHandle BaseObject, TAbility Dest);        // 原 2572  GetAbility: TBaseObject_GetAbility
    bool GetWAbility(IBaseObjectHandle BaseObject, TAbility Dest);        // 原 2573  GetWAbility: TBaseObject_GetWAbility
    void SetWAbility(IBaseObjectHandle BaseObject, TAbility pValue);        // 原 2574  SetWAbility: TBaseObject_SetWAbility
    IListHandle GetSlaveList(IBaseObjectHandle BaseObject);        // 原 2575  GetSlaveList: TBaseObject_GetSlaveList
    IBaseObjectHandle GetMaster(IBaseObjectHandle BaseObject);        // 原 2576  GetMaster: TBaseObject_GetMaster
    IBaseObjectHandle GetMasterEx(IBaseObjectHandle BaseObject);        // 原 2577  GetMasterEx: TBaseObject_GetMasterEx
    bool GetSuperManMode(IBaseObjectHandle BaseObject);        // 原 2578  GetSuperManMode: TBaseObject_GetSuperManMode
    void SetSuperManMode(IBaseObjectHandle BaseObject, bool pValue);        // 原 2579  SetSuperManMode: TBaseObject_SetSuperManMode
    bool GetAdminMode(IBaseObjectHandle BaseObject);        // 原 2580  GetAdminMode: TBaseObject_GetAdminMode
    void SetAdminMode(IBaseObjectHandle BaseObject, bool pValue);        // 原 2581  SetAdminMode: TBaseObject_SetAdminMode
    bool GetTransparent(IBaseObjectHandle BaseObject);        // 原 2582  GetTransparent: TBaseObject_GetTransparent
    void SetTransparent(IBaseObjectHandle BaseObject, bool pValue);        // 原 2583  SetTransparent: TBaseObject_SetTransparent
    bool GetObMode(IBaseObjectHandle BaseObject);        // 原 2584  GetObMode: TBaseObject_GetObMode
    void SetObMode(IBaseObjectHandle BaseObject, bool pValue);        // 原 2585  SetObMode: TBaseObject_SetObMode
    bool GetStoneMode(IBaseObjectHandle BaseObject);        // 原 2586  GetStoneMode: TBaseObject_GetStoneMode
    void SetStoneMode(IBaseObjectHandle BaseObject, bool pValue);        // 原 2587  SetStoneMode: TBaseObject_SetStoneMode
    bool GetStickMode(IBaseObjectHandle BaseObject);        // 原 2588  GetStickMode: TBaseObject_GetStickMode
    void SetStickMode(IBaseObjectHandle BaseObject, bool pValue);        // 原 2589  SetStickMode: TBaseObject_SetStickMode
    bool GetIsAnimal(IBaseObjectHandle BaseObject);        // 原 2590  GetIsAnimal: TBaseObject_GetIsAnimal
    void SetIsAnimal(IBaseObjectHandle BaseObject, bool pValue);        // 原 2591  SetIsAnimal: TBaseObject_SetIsAnimal
    bool GetIsNoItem(IBaseObjectHandle BaseObject);        // 原 2592  GetIsNoItem: TBaseObject_GetIsNoItem
    void SetIsNoItem(IBaseObjectHandle BaseObject, bool pValue);        // 原 2593  SetIsNoItem: TBaseObject_SetIsNoItem
    bool GetCoolEye(IBaseObjectHandle BaseObject);        // 原 2594  GetCoolEye: TBaseObject_GetCoolEye
    void SetCoolEye(IBaseObjectHandle BaseObject, bool pValue);        // 原 2595  SetCoolEye: TBaseObject_SetCoolEye
    ushort GetHitPoint(IBaseObjectHandle BaseObject);        // 原 2596  GetHitPoint: TBaseObject_GetHitPoint
    void SetHitPoint(IBaseObjectHandle BaseObject, ushort pValue);        // 原 2597  SetHitPoint: TBaseObject_SetHitPoint
    ushort GetSpeedPoint(IBaseObjectHandle BaseObject);        // 原 2598  GetSpeedPoint: TBaseObject_GetSpeedPoint
    void SetSpeedPoint(IBaseObjectHandle BaseObject, ushort pValue);        // 原 2599  SetSpeedPoint: TBaseObject_SetSpeedPoint
    sbyte GetHitSpeed(IBaseObjectHandle BaseObject);        // 原 2600  GetHitSpeed: TBaseObject_GetHitSpeed
    void SetHitSpeed(IBaseObjectHandle BaseObject, sbyte pValue);        // 原 2601  SetHitSpeed: TBaseObject_SetHitSpeed
    int GetWalkSpeed(IBaseObjectHandle BaseObject);        // 原 2602  GetWalkSpeed: TBaseObject_GetWalkSpeed
    void SetWalkSpeed(IBaseObjectHandle BaseObject, int pValue);        // 原 2603  SetWalkSpeed: TBaseObject_SetWalkSpeed
    sbyte GetHPRecover(IBaseObjectHandle BaseObject);        // 原 2604  GetHPRecover: TBaseObject_GetHPRecover
    void SetHPRecover(IBaseObjectHandle BaseObject, sbyte pValue);        // 原 2605  SetHPRecover: TBaseObject_SetHPRecover
    sbyte GetMPRecover(IBaseObjectHandle BaseObject);        // 原 2606  GetMPRecover: TBaseObject_GetMPRecover
    void SetMPRecover(IBaseObjectHandle BaseObject, sbyte pValue);        // 原 2607  SetMPRecover: TBaseObject_SetMPRecover
    sbyte GetPoisonRecover(IBaseObjectHandle BaseObject);        // 原 2608  GetPoisonRecover: TBaseObject_GetPoisonRecover
    void SetPoisonRecover(IBaseObjectHandle BaseObject, sbyte pValue);        // 原 2609  SetPoisonRecover: TBaseObject_SetPoisonRecover
    byte GetAntiPoison(IBaseObjectHandle BaseObject);        // 原 2610  GetAntiPoison: TBaseObject_GetAntiPoison
    void SetAntiPoison(IBaseObjectHandle BaseObject, byte pValue);        // 原 2611  SetAntiPoison: TBaseObject_SetAntiPoison
    sbyte GetAntiMagic(IBaseObjectHandle BaseObject);        // 原 2612  GetAntiMagic: TBaseObject_GetAntiMagic
    void SetAntiMagic(IBaseObjectHandle BaseObject, sbyte pValue);        // 原 2613  SetAntiMagic: TBaseObject_SetAntiMagic
    int GetLuck(IBaseObjectHandle BaseObject);        // 原 2614  GetLuck: TBaseObject_GetLuck
    void SetLuck(IBaseObjectHandle BaseObject, int pValue);        // 原 2615  SetLuck: TBaseObject_SetLuck
    byte GetAttatckMode(IBaseObjectHandle BaseObject);        // 原 2616  GetAttatckMode: TBaseObject_GetAttatckMode
    void SetAttatckMode(IBaseObjectHandle BaseObject, byte pValue);        // 原 2617  SetAttatckMode: TBaseObject_SetAttatckMode
    byte GetNation(IBaseObjectHandle BaseObject);        // 原 2618  GetNation: TBaseObject_GetNation
    bool SetNation(IBaseObjectHandle BaseObject, byte Nation);        // 原 2619  SetNation: TBaseObject_SetNation
    bool GetNationaName(IBaseObjectHandle BaseObject, byte[] Dest, uint DestLen);        // 原 2620  GetNationaName: TBaseObject_GetNationaName
    IGuildHandle GetGuild(IBaseObjectHandle BaseObject);        // 原 2621  GetGuild: TBaseObject_GetGuild
    int GetGuildRankNo(IBaseObjectHandle BaseObject);        // 原 2622  GetGuildRankNo: TBaseobject_GetGuildRankNo
    bool GetGuildRankName(IBaseObjectHandle BaseObject, byte[] Dest, uint DestLen);        // 原 2623  GetGuildRankName: TBaseobject_GetGuildRankName
    bool IsGuildMaster(IBaseObjectHandle BaseObject);        // 原 2624  IsGuildMaster: TBaseObject_IsGuildMaster
    bool GetHideMode(IBaseObjectHandle BaseObject);        // 原 2625  GetHideMode: TBaseObject_GetHideMode
    void SetHideMode(IBaseObjectHandle BaseObject, bool pValue);        // 原 2626  SetHideMode: TBaseObject_SetHideMode
    bool GetIsParalysis(IBaseObjectHandle BaseObject);        // 原 2627  GetIsParalysis: TBaseObject_GetIsParalysis
    void SetIsParalysis(IBaseObjectHandle BaseObject, bool pValue);        // 原 2628  SetIsParalysis: TBaseObject_SetIsParalysis
    uint GetParalysisRate(IBaseObjectHandle BaseObject);        // 原 2629  GetParalysisRate: TBaseObject_GetParalysisRate
    void SetParalysisRate(IBaseObjectHandle BaseObject, uint pValue);        // 原 2630  SetParalysisRate: TBaseObject_SetParalysisRate
    bool GetIsMDParalysis(IBaseObjectHandle BaseObject);        // 原 2631  GetIsMDParalysis: TBaseObject_GetIsMDParalysis
    void SetIsMDParalysis(IBaseObjectHandle BaseObject, bool pValue);        // 原 2632  SetIsMDParalysis: TBaseObject_SetIsMDParalysis
    uint GetMDParalysisRate(IBaseObjectHandle BaseObject);        // 原 2633  GetMDParalysisRate: TBaseObject_GetMDParalysisRate
    void SetMDParalysisRate(IBaseObjectHandle BaseObject, uint pValue);        // 原 2634  SetMDParalysisRate: TBaseObject_SetMDParalysisRate
    bool GetIsFrozen(IBaseObjectHandle BaseObject);        // 原 2635  GetIsFrozen: TBaseObject_GetIsFrozen
    void SetIsFrozen(IBaseObjectHandle BaseObject, bool pValue);        // 原 2636  SetIsFrozen: TBaseObject_SetIsFrozen
    uint GetFrozenRate(IBaseObjectHandle BaseObject);        // 原 2637  GetFrozenRate: TBaseObject_GetFrozenRate
    void SetFrozenRate(IBaseObjectHandle BaseObject, uint pValue);        // 原 2638  SetFrozenRate: TBaseObject_SetFrozenRate
    bool GetIsCobwebWinding(IBaseObjectHandle BaseObject);        // 原 2639  GetIsCobwebWinding: TBaseObject_GetIsCobwebWinding
    void SetIsCobwebWinding(IBaseObjectHandle BaseObject, bool pValue);        // 原 2640  SetIsCobwebWinding: TBaseObject_SetIsCobwebWinding
    uint GetCobwebWindingRate(IBaseObjectHandle BaseObject);        // 原 2641  GetCobwebWindingRate: TBaseObject_GetCobwebWindingRate
    void SetCobwebWindingRate(IBaseObjectHandle BaseObject, uint pValue);        // 原 2642  SetCobwebWindingRate: TBaseObject_SetCobwebWindingRate
    uint GetUnParalysisValue(IBaseObjectHandle BaseObject);        // 原 2643  GetUnParalysisValue: TBaseObject_GetUnParalysisValue
    void SetUnParalysisValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2644  SetUnParalysisValue: TBaseObject_SetUnParalysisValue
    bool GetIsUnParalysis(IBaseObjectHandle BaseObject);        // 原 2645  GetIsUnParalysis: TBaseObject_GetIsUnParalysis
    uint GetUnMagicShieldValue(IBaseObjectHandle BaseObject);        // 原 2646  GetUnMagicShieldValue: TBaseObject_GetUnMagicShieldValue
    void SetUnMagicShieldValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2647  SetUnMagicShieldValue: TBaseObject_SetUnMagicShieldValue
    bool GetIsUnMagicShield(IBaseObjectHandle BaseObject);        // 原 2648  GetIsUnMagicShield: TBaseObject_GetIsUnMagicShield
    uint GetUnRevivalValue(IBaseObjectHandle BaseObject);        // 原 2649  GetUnRevivalValue: TBaseObject_GetUnRevivalValue
    void SetUnRevivalValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2650  SetUnRevivalValue: TBaseObject_SetUnRevivalValue
    bool GetIsUnRevival(IBaseObjectHandle BaseObject);        // 原 2651  GetIsUnRevival: TBaseObject_GetIsUnRevival
    uint GetUnPosionValue(IBaseObjectHandle BaseObject);        // 原 2652  GetUnPosionValue: TBaseObject_GetUnPosionValue
    void SetUnPosionValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2653  SetUnPosionValue: TBaseObject_SetUnPosionValue
    bool GetIsUnPosion(IBaseObjectHandle BaseObject);        // 原 2654  GetIsUnPosion: TBaseObject_GetIsUnPosion
    uint GetUnTammingValue(IBaseObjectHandle BaseObject);        // 原 2655  GetUnTammingValue: TBaseObject_GetUnTammingValue
    void SetUnTammingValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2656  SetUnTammingValue: TBaseObject_SetUnTammingValue
    bool GetIsUnTamming(IBaseObjectHandle BaseObject);        // 原 2657  GetIsUnTamming: TBaseObject_GetIsUnTamming
    uint GetUnFireCrossValue(IBaseObjectHandle BaseObject);        // 原 2658  GetUnFireCrossValue: TBaseObject_GetUnFireCrossValue
    void SetUnFireCrossValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2659  SetUnFireCrossValue: TBaseObject_SetUnFireCrossValue
    bool GetIsUnFireCross(IBaseObjectHandle BaseObject);        // 原 2660  GetIsUnFireCross: TBaseObject_GetIsUnFireCross
    uint GetUnFrozenValue(IBaseObjectHandle BaseObject);        // 原 2661  GetUnFrozenValue: TBaseObject_GetUnFrozenValue
    void SetUnFrozenValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2662  SetUnFrozenValue: TBaseObject_SetUnFrozenValue
    bool GetIsUnFrozen(IBaseObjectHandle BaseObject);        // 原 2663  GetIsUnFrozen: TBaseObject_GetIsUnFrozen
    uint GetUnCobwebWindingValue(IBaseObjectHandle BaseObject);        // 原 2664  GetUnCobwebWindingValue: TBaseObject_GetUnCobwebWindingValue
    void SetUnCobwebWindingValue(IBaseObjectHandle BaseObject, uint pValue);        // 原 2665  SetUnCobwebWindingValue: TBaseObject_SetUnCobwebWindingValue
    bool GetIsUnCobwebWinding(IBaseObjectHandle BaseObject);        // 原 2666  GetIsUnCobwebWinding: TBaseObject_GetIsUnCobwebWinding
    IBaseObjectHandle GetTargetCret(IBaseObjectHandle BaseObject);        // 原 2667  GetTargetCret: TBaseObject_GetTargetCret
    void SetTargetCret(IBaseObjectHandle BaseObject, IBaseObjectHandle TargetCret);        // 原 2668  SetTargetCret: TBaseObject_SetTargetCret
    void DelTargetCreat(IBaseObjectHandle BaseObject);        // 原 2669  DelTargetCreat: TBaseObject_DelTargetCreat
    IBaseObjectHandle GetLastHiter(IBaseObjectHandle BaseObject);        // 原 2670  GetLastHiter: TBaseObject_GetLastHiter
    IBaseObjectHandle GetExpHitter(IBaseObjectHandle BaseObject);        // 原 2671  GetExpHitter: TBaseObject_GetExpHitter
    IBaseObjectHandle GetPoisonHitter(IBaseObjectHandle BaseObject);        // 原 2672  GetPoisonHitter: TBaseObject_GetPoisonHitter
    IBaseObjectHandle GetPoseCreate(IBaseObjectHandle BaseObject);        // 原 2673  GetPoseCreate: TBaseObject_GetPoseCreate
    bool IsProperTarget(IBaseObjectHandle BaseObject, IBaseObjectHandle Target);        // 原 2674  IsProperTarget: TBaseObject_IsProperTarget
    bool IsProperFriend(IBaseObjectHandle BaseObject, IBaseObjectHandle Target);        // 原 2675  IsProperFriend: TBaseObject_IsProperFriend
    bool TargetInRange(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nX, int nY, int nRange);        // 原 2676  TargetInRange: TBaseObject_TargetInRange
    void SendMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg);        // 原 2677  SendMsg: TBaseObject_SendMsg
    void SendDelayMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg, uint dwDelay);        // 原 2678  SendDelayMsg: TBaseObject_SendDelayMsg
    void SendRefMsg(IBaseObjectHandle BaseObject, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg, uint dwDelay);        // 原 2679  SendRefMsg: TBaseObject_SendRefMsg
    void SendUpdateMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg);        // 原 2680  SendUpdateMsg: TBaseObject_SendUpdateMsg
    bool SysMsg(IBaseObjectHandle BaseObject, byte[] sMsg, byte FColor, byte BColor, int MsgType);        // 原 2681  SysMsg: TBaseObject_SysMsg
    IListHandle GetBagItemList(IBaseObjectHandle BaseObject);        // 原 2682  GetBagItemList: TBaseObject_GetBagItemList
    bool IsEnoughBag(IBaseObjectHandle BaseObject);        // 原 2683  IsEnoughBag: TBaseObject_IsEnoughBag
    bool IsEnoughBagEx(IBaseObjectHandle BaseObject, int AddCount);        // 原 2684  IsEnoughBagEx: TBaseObject_IsEnoughBagEx
    bool AddItemToBag(IBaseObjectHandle BaseObject, TUserItem UserItem);        // 原 2685  AddItemToBag: TBaseObject_AddItemToBag
    bool DelBagItemByIndex(IBaseObjectHandle BaseObject, int pIndex);        // 原 2686  DelBagItemByIndex: TBaseObject_DelBagItemByIndex
    bool DelBagItemByMakeIdx(IBaseObjectHandle BaseObject, int MakeIndex, byte[] ItemName);        // 原 2687  DelBagItemByMakeIdx: TBaseObject_DelBagItemByMakeIdx
    bool DelBagItemByUserItem(IBaseObjectHandle BaseObject, TUserItem UserItem);        // 原 2688  DelBagItemByUserItem: TBaseObject_DelBagItemByUserItem
    bool IsInSafeZone(IBaseObjectHandle BaseObject);        // 原 2689  IsInSafeZone: TBaseObject_IsInSafeZone
    bool IsPtInSafeZone(IBaseObjectHandle BaseObject, IEnvirnoment Envir, int nX, int nY);        // 原 2690  IsPtInSafeZone: TBaseObject_IsPtInSafeZone
    void RecalcLevelAbil(IBaseObjectHandle BaseObject, bool IsSysDef);        // 原 2691  RecalcLevelAbil: TBaseObject_RecalcLevelAbil
    void RecalcAbil(IBaseObjectHandle BaseObject);        // 原 2693  RecalcAbil: TBaseObject_RecalcAbil
    int RecalcBagWeight(IBaseObjectHandle BaseObject);        // 原 2694  RecalcBagWeight: TBaseObject_RecalcBagWeight
    uint GetLevelExp(IBaseObjectHandle BaseObject, int nLevel);        // 原 2695  GetLevelExp: TBaseObject_GetLevelExp
    void HasLevelUp(IBaseObjectHandle BaseObject, int nLevel);        // 原 2696  HasLevelUp: TBaseObject_HasLevelUp
    bool TrainSkill(IBaseObjectHandle BaseObject, TUserMagic UserMagic, int nTranPoint, bool IsDoCheck);        // 原 2697  TrainSkill: TBaseObject_TrainSkill
    bool CheckMagicLevelup(IBaseObjectHandle BaseObject, TUserMagic UserMagic);        // 原 2698  CheckMagicLevelup: TBaseObject_CheckMagicLevelup
    void MagicTranPointChanged(IBaseObjectHandle BaseObject, TUserMagic UserMagic);        // 原 2699  MagicTranPointChanged: TBaseObject_MagicTranPointChanged
    void DamageHealth(IBaseObjectHandle BaseObject, int nDamage, IBaseObjectHandle StruckFrom);        // 原 2700  DamageHealth: TBaseObject_DamageHealth
    void DamageSpell(IBaseObjectHandle BaseObject, int nSpellPoint);        // 原 2701  DamageSpell: TBaseObject_DamageSpell
    void IncHealthSpell(IBaseObjectHandle BaseObject, int nHP, int nMP, bool SendChangedToClient);        // 原 2702  IncHealthSpell: TBaseObject_IncHealthSpell
    void HealthSpellChanged(IBaseObjectHandle BaseObject, uint dwDelay);        // 原 2703  HealthSpellChanged: TBaseObject_HealthSpellChanged
    void FeatureChanged(IBaseObjectHandle BaseObject);        // 原 2704  FeatureChanged: TBaseObject_FeatureChanged
    void WeightChanged(IBaseObjectHandle BaseObject);        // 原 2705  WeightChanged: TBaseObject_WeightChanged
    int GetHitStruckDamage(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nDamage, IntPtr MagicACInfo, int nType);        // 原 2706  GetHitStruckDamage: TBaseObject_GetHitStruckDamage
    int GetMagStruckDamage(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nDamage);        // 原 2707  GetMagStruckDamage: TBaseObject_GetMagStruckDamage
    bool GetActorIcon(IBaseObjectHandle BaseObject, int pIndex, TActorIcon ActorIcon);        // 原 2708  GetActorIcon: TBaseObject_GetActorIcon
    bool SetActorIcon(IBaseObjectHandle BaseObject, int pIndex, TActorIcon ActorIcon);        // 原 2709  SetActorIcon: TBaseObject_SetActorIcon
    void RefUseIcons(IBaseObjectHandle BaseObject);        // 原 2710  RefUseIcons: TBaseObject_RefUseIcons
    void RefUseEffects(IBaseObjectHandle BaseObject);        // 原 2711  RefUseEffects: TBaseObject_RefUseEffects
    void SpaceMove(IBaseObjectHandle BaseObject, byte[] sMapName, int nX, int nY, int nInt);        // 原 2712  SpaceMove: TBaseObject_SpaceMove
    void MapRandomMove(IBaseObjectHandle BaseObject, byte[] sMapName, int nInt);        // 原 2713  MapRandomMove: TBaseObject_MapRandomMove
    bool CanMove(IBaseObjectHandle BaseObject);        // 原 2714  CanMove: TBaseObject_CanMove
    bool CanRun(IBaseObjectHandle BaseObject, int nCurrX, int nCurrY, int nX, int nY);        // 原 2715  CanRun: TBaseObject_CanRun
    void TurnTo(IBaseObjectHandle BaseObject, byte btDir);        // 原 2716  TurnTo: TBaseObject_TurnTo
    bool WalkTo(IBaseObjectHandle BaseObject, byte btDir, bool boFlag);        // 原 2717  WalkTo: TBaseObject_WalkTo
    bool RunTo(IBaseObjectHandle BaseObject, byte btDir, bool boFlag);        // 原 2718  RunTo: TBaseObject_RunTo
    IListHandle PluginList(IBaseObjectHandle BaseObject);        // 原 2719  PluginList: TBaseObject_PluginList
}

/// <summary>原文 TSmartObjectFunc 对应的托管接口 ISmartObjectFunc（104 个成员）。</summary>
public interface ISmartObjectFunc
{
    IListHandle GetMagicList(ISmartObjectHandle SmartObject);        // 原 2724  GetMagicList: TSmartObject_GetMagicList
    bool GetUseItem(ISmartObjectHandle SmartObject, int pIndex, TUserItem UserItem);        // 原 2725  GetUseItem: TSmartObject_GetUseItem
    int GetJewelryBoxStatus(ISmartObjectHandle SmartObject);        // 原 2726  GetJewelryBoxStatus: TSmartObject_GetJewelryBoxStatus
    void SetJewelryBoxStatus(ISmartObjectHandle SmartObject, int pValue);        // 原 2727  SetJewelryBoxStatus: TSmartObject_SetJewelryBoxStatus
    bool GetJewelryItem(ISmartObjectHandle SmartObject, int pIndex, TUserItem UserItem);        // 原 2728  GetJewelryItem: TSmartObject_GetJewelryItem
    bool GetIsShowGodBless(ISmartObjectHandle SmartObject);        // 原 2729  GetIsShowGodBless: TSmartObject_GetIsShowGodBless
    void SetIsShowGodBless(ISmartObjectHandle SmartObject, bool pValue);        // 原 2730  SetIsShowGodBless: TSmartObject_SetIsShowGodBless
    bool GetGodBlessItemsState(ISmartObjectHandle SmartObject, int pIndex);        // 原 2731  GetGodBlessItemsState: TSmartObject_GetGodBlessItemsState
    void SetGodBlessItemsState(ISmartObjectHandle SmartObject, int pIndex, bool pValue);        // 原 2732  SetGodBlessItemsState: TSmartObject_SetGodBlessItemsState
    bool GetGodBlessItem(ISmartObjectHandle SmartObject, int pIndex, TUserItem UserItem);        // 原 2733  GetGodBlessItem: TSmartObject_GetGodBlessItem
    IListHandle GetFengHaoItems(ISmartObjectHandle SmartObject);        // 原 2734  GetFengHaoItems: TSmartObject_GetFengHaoItems
    int GetActiveFengHao(ISmartObjectHandle SmartObject);        // 原 2735  GetActiveFengHao: TSmartObject_GetActiveFengHao
    void SetActiveFengHao(ISmartObjectHandle SmartObject, int FengHaoIndex);        // 原 2736  SetActiveFengHao: TSmartObject_SetActiveFengHao
    void ActiveFengHaoChanged(ISmartObjectHandle SmartObject);        // 原 2737  ActiveFengHaoChanged: TSmartObject_ActiveFengHaoChanged
    void DeleteFengHao(ISmartObjectHandle SmartObject, int pIndex);        // 原 2738  DeleteFengHao: TSmartObject_DeleteFengHao
    void ClearFengHao(ISmartObjectHandle SmartObject);        // 原 2739  ClearFengHao: TSmartObject_ClearFengHao
    short GetMoveSpeed(ISmartObjectHandle SmartObject);        // 原 2740  GetMoveSpeed: TSmartObject_GetMoveSpeed
    void SetMoveSpeed(ISmartObjectHandle SmartObject, short pValue);        // 原 2741  SetMoveSpeed: TSmartObject_SetMoveSpeed
    short GetAttackSpeed(ISmartObjectHandle SmartObject);        // 原 2742  GetAttackSpeed: TSmartObject_GetAttackSpeed
    void SetAttackSpeed(ISmartObjectHandle SmartObject, short pValue);        // 原 2743  SetAttackSpeed: TSmartObject_SetAttackSpeed
    short GetSpellSpeed(ISmartObjectHandle SmartObject);        // 原 2744  GetSpellSpeed: TSmartObject_GetSpellSpeed
    void SetSpellSpeed(ISmartObjectHandle SmartObject, short pValue);        // 原 2745  SetSpellSpeed: TSmartObject_SetSpellSpeed
    void RefGameSpeed(ISmartObjectHandle SmartObject);        // 原 2746  RefGameSpeed: TSmartObject_RefGameSpeed
    bool GetIsButch(ISmartObjectHandle SmartObject);        // 原 2747  GetIsButch: TSmartObject_GetIsButch
    void SetIsButch(ISmartObjectHandle SmartObject, bool pValue);        // 原 2748  SetIsButch: TSmartObject_SetIsButch
    bool GetIsTrainingNG(ISmartObjectHandle SmartObject);        // 原 2749  GetIsTrainingNG: TSmartObject_GetIsTrainingNG
    void SetIsTrainingNG(ISmartObjectHandle SmartObject, bool pValue);        // 原 2750  SetIsTrainingNG: TSmartObject_SetIsTrainingNG
    bool GetIsTrainingXF(ISmartObjectHandle SmartObject);        // 原 2751  GetIsTrainingXF: TSmartObject_GetIsTrainingXF
    void SetIsTrainingXF(ISmartObjectHandle SmartObject, bool pValue);        // 原 2752  SetIsTrainingXF: TSmartObject_SetIsTrainingXF
    bool GetIsOpenLastContinuous(ISmartObjectHandle SmartObject);        // 原 2753  GetIsOpenLastContinuous: TSmartObject_GetIsOpenLastContinuous
    void SetIsOpenLastContinuous(ISmartObjectHandle SmartObject, bool pValue);        // 原 2754  SetIsOpenLastContinuous: TSmartObject_SetIsOpenLastContinuous
    byte GetContinuousMagicOrder(ISmartObjectHandle SmartObject, int pIndex);        // 原 2755  GetContinuousMagicOrder: TSmartObject_GetContinuousMagicOrder
    void SetContinuousMagicOrder(ISmartObjectHandle SmartObject, int pIndex, byte pValue);        // 原 2756  SetContinuousMagicOrder: TSmartObject_SetContinuousMagicOrder
    uint GetPKDieLostExp(ISmartObjectHandle SmartObject);        // 原 2757  GetPKDieLostExp: TSmartObject_GetPKDieLostExp
    void SetPKDieLostExp(ISmartObjectHandle SmartObject, uint pValue);        // 原 2758  SetPKDieLostExp: TSmartObject_SetPKDieLostExp
    int GetPKDieLostLevel(ISmartObjectHandle SmartObject);        // 原 2759  GetPKDieLostLevel: TSmartObject_GetPKDieLostLevel
    void SetPKDieLostLevel(ISmartObjectHandle SmartObject, int pValue);        // 原 2760  SetPKDieLostLevel: TSmartObject_SetPKDieLostLevel
    int GetPKPoint(ISmartObjectHandle SmartObject);        // 原 2761  GetPKPoint: TSmartObject_GetPKPoint
    void SetPKPoint(ISmartObjectHandle SmartObject, int pValue);        // 原 2762  SetPKPoint: TSmartObject_SetPKPoint
    void IncPKPoint(ISmartObjectHandle SmartObject, int pValue);        // 原 2763  IncPKPoint: TSmartObject_IncPKPoint
    void DecPKPoint(ISmartObjectHandle SmartObject, int pValue);        // 原 2764  DecPKPoint: TSmartObject_DecPKPoint
    int GetPKLevel(ISmartObjectHandle SmartObject);        // 原 2765  GetPKLevel: TSmartObject_GetPKLevel
    void SetPKLevel(ISmartObjectHandle SmartObject, int pValue);        // 原 2766  SetPKLevel: TSmartObject_SetPKLevel
    bool GetIsTeleport(ISmartObjectHandle SmartObject);        // 原 2767  GetIsTeleport: TSmartObject_GetIsTeleport
    void SetIsTeleport(ISmartObjectHandle SmartObject, bool pValue);        // 原 2768  SetIsTeleport: TSmartObject_SetIsTeleport
    bool GetIsRevival(ISmartObjectHandle SmartObject);        // 原 2769  GetIsRevival: TSmartObject_GetIsRevival
    void SetIsRevival(ISmartObjectHandle SmartObject, bool pValue);        // 原 2770  SetIsRevival: TSmartObject_SetIsRevival
    int GetRevivalTime(ISmartObjectHandle SmartObject);        // 原 2771  GetRevivalTime: TSmartObject_GetRevivalTime
    void SetRevivalTime(ISmartObjectHandle SmartObject, int pValue);        // 原 2772  SetRevivalTime: TSmartObject_SetRevivalTime
    bool GetIsFlameRing(ISmartObjectHandle SmartObject);        // 原 2773  GetIsFlameRing: TSmartObject_GetIsFlameRing
    void SetIsFlameRing(ISmartObjectHandle SmartObject, bool pValue);        // 原 2774  SetIsFlameRing: TSmartObject_SetIsFlameRing
    bool GetIsRecoveryRing(ISmartObjectHandle SmartObject);        // 原 2775  GetIsRecoveryRing: TSmartObject_GetIsRecoveryRing
    void SetIsRecoveryRing(ISmartObjectHandle SmartObject, bool pValue);        // 原 2776  SetIsRecoveryRing: TSmartObject_SetIsRecoveryRing
    bool GetIsMagicShield(ISmartObjectHandle SmartObject);        // 原 2777  GetIsMagicShield: TSmartObject_GetIsMagicShield
    void SetIsMagicShield(ISmartObjectHandle SmartObject, bool pValue);        // 原 2778  SetIsMagicShield: TSmartObject_SetIsMagicShield
    bool GetIsMuscleRing(ISmartObjectHandle SmartObject);        // 原 2779  GetIsMuscleRing: TSmartObject_GetIsMuscleRing
    void SetIsMuscleRing(ISmartObjectHandle SmartObject, bool pValue);        // 原 2780  SetIsMuscleRing: TSmartObject_SetIsMuscleRing
    bool GetIsFastTrain(ISmartObjectHandle SmartObject);        // 原 2781  GetIsFastTrain: TSmartObject_GetIsFastTrain
    void SetIsFastTrain(ISmartObjectHandle SmartObject, bool pValue);        // 原 2782  SetIsFastTrain: TSmartObject_SetIsFastTrain
    bool GetIsProbeNecklace(ISmartObjectHandle SmartObject);        // 原 2783  GetIsProbeNecklace: TSmartObject_GetIsProbeNecklace
    void SetIsProbeNecklace(ISmartObjectHandle SmartObject, bool pValue);        // 原 2784  SetIsProbeNecklace: TSmartObject_SetIsProbeNecklace
    bool GetIsRecallSuite(ISmartObjectHandle SmartObject);        // 原 2785  GetIsRecallSuite: TSmartObject_GetIsRecallSuite
    void SetIsRecallSuite(ISmartObjectHandle SmartObject, bool pValue);        // 原 2786  SetIsRecallSuite: TSmartObject_SetIsRecallSuite
    bool GetIsPirit(ISmartObjectHandle SmartObject);        // 原 2787  GetIsPirit: TSmartObject_GetIsPirit
    void SetIsPirit(ISmartObjectHandle SmartObject, bool pValue);        // 原 2788  SetIsPirit: TSmartObject_SetIsPirit
    bool GetIsSupermanItem(ISmartObjectHandle SmartObject);        // 原 2789  GetIsSupermanItem: TSmartObject_GetIsSupermanItem
    void SetIsSupermanItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2790  SetIsSupermanItem: TSmartObject_SetIsSupermanItem
    bool GetIsExpItem(ISmartObjectHandle SmartObject);        // 原 2791  GetIsExpItem: TSmartObject_GetIsExpItem
    void SetIsExpItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2792  SetIsExpItem: TSmartObject_SetIsExpItem
    double GetExpItemValue(ISmartObjectHandle SmartObject);        // 原 2793  GetExpItemValue: TSmartObject_GetExpItemValue
    void SetExpItemValue(ISmartObjectHandle SmartObject, double pValue);        // 原 2794  SetExpItemValue: TSmartObject_SetExpItemValue
    int GetExpItemRate(ISmartObjectHandle SmartObject);        // 原 2795  GetExpItemRate: TSmartObject_GetExpItemRate
    bool GetIsPowerItem(ISmartObjectHandle SmartObject);        // 原 2796  GetIsPowerItem: TSmartObject_GetIsPowerItem
    void SetIsPowerItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2797  SetIsPowerItem: TSmartObject_SetIsPowerItem
    double GetPowerItemValue(ISmartObjectHandle SmartObject);        // 原 2798  GetPowerItemValue: TSmartObject_GetPowerItemValue
    void SetPowerItemValue(ISmartObjectHandle SmartObject, double pValue);        // 原 2799  SetPowerItemValue: TSmartObject_SetPowerItemValue
    int GetPowerItemRate(ISmartObjectHandle SmartObject);        // 原 2800  GetPowerItemRate: TSmartObject_GetPowerItemRate
    bool GetIsGuildMove(ISmartObjectHandle SmartObject);        // 原 2801  GetIsGuildMove: TSmartObject_GetIsGuildMove
    void SetIsGuildMove(ISmartObjectHandle SmartObject, bool pValue);        // 原 2802  SetIsGuildMove: TSmartObject_SetIsGuildMove
    bool GetIsAngryRing(ISmartObjectHandle SmartObject);        // 原 2803  GetIsAngryRing: TSmartObject_GetIsAngryRing
    void SetIsAngryRing(ISmartObjectHandle SmartObject, bool pValue);        // 原 2804  SetIsAngryRing: TSmartObject_SetIsAngryRing
    bool GetIsStarRing(ISmartObjectHandle SmartObject);        // 原 2805  GetIsStarRing: TSmartObject_GetIsStarRing
    void SetIsStarRing(ISmartObjectHandle SmartObject, bool pValue);        // 原 2806  SetIsStarRing: TSmartObject_SetIsStarRing
    bool GetIsACItem(ISmartObjectHandle SmartObject);        // 原 2807  GetIsACItem: TSmartObject_GetIsACItem
    void SetIsACItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2808  SetIsACItem: TSmartObject_SetIsACItem
    double GetACItemValue(ISmartObjectHandle SmartObject);        // 原 2809  GetACItemValue: TSmartObject_GetACItemValue
    void SetACItemValue(ISmartObjectHandle SmartObject, double pValue);        // 原 2810  SetACItemValue: TSmartObject_SetACItemValue
    bool GetIsMACItem(ISmartObjectHandle SmartObject);        // 原 2811  GetIsMACItem: TSmartObject_GetIsMACItem
    void SetIsMACItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2812  SetIsMACItem: TSmartObject_SetIsMACItem
    double GetMACItemValue(ISmartObjectHandle SmartObject);        // 原 2813  GetMACItemValue: TSmartObject_GetMACItemValue
    void SetMACItemValue(ISmartObjectHandle SmartObject, double pValue);        // 原 2814  SetMACItemValue: TSmartObject_SetMACItemValue
    bool GetIsNoDropItem(ISmartObjectHandle SmartObject);        // 原 2815  GetIsNoDropItem: TSmartObject_GetIsNoDropItem
    void SetIsNoDropItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2816  SetIsNoDropItem: TSmartObject_SetIsNoDropItem
    bool GetIsNoDropUseItem(ISmartObjectHandle SmartObject);        // 原 2817  GetIsNoDropUseItem: TSmartObject_GetIsNoDropUseItem
    void SetIsNoDropUseItem(ISmartObjectHandle SmartObject, bool pValue);        // 原 2818  SetIsNoDropUseItem: TSmartObject_SetIsNoDropUseItem
    bool GetNGAbility(ISmartObjectHandle SmartObject, TAbilityNG AbilityNG);        // 原 2819  GetNGAbility: TSmartObject_GetNGAbility
    void SetNGAbility(ISmartObjectHandle SmartObject, TAbilityNG pValue);        // 原 2820  SetNGAbility: TSmartObject_SetNGAbility
    bool GetAlcohol(ISmartObjectHandle SmartObject, TAbilityAlcohol AbilityAlcohol);        // 原 2821  GetAlcohol: TSmartObject_GetAlcohol
    void SetAlcohol(ISmartObjectHandle SmartObject, TAbilityAlcohol pValue);        // 原 2822  SetAlcohol: TSmartObject_SetAlcohol
    void RepairAllItem(ISmartObjectHandle SmartObject);        // 原 2823  RepairAllItem: TSmartObject_RepairAllItem
    bool IsAllowUseMagic(ISmartObjectHandle SmartObject, ushort MagicID);        // 原 2824  IsAllowUseMagic: TSmartObject_IsAllowUseMagic
    int SelectMagic(ISmartObjectHandle SmartObject);        // 原 2825  SelectMagic: TSmartObject_SelectMagic
    bool AttackTarget(ISmartObjectHandle SmartObject, ushort MagicID, uint AttackTime);        // 原 2826  AttackTarget: TSmartObject_AttackTarget
}

/// <summary>原文 TPlayObjectFunc 对应的托管接口 IPlayObjectFunc（160 个成员）。</summary>
public interface IPlayObjectFunc
{
    bool GetUserID(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2831  GetUserID: TPlayObject_GetUserID
    bool GetIPAddr(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2832  GetIPAddr: TPlayObject_GetIPAddr
    bool GetIPLocal(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2833  GetIPLocal: TPlayObject_GetIPLocal
    bool GetMachineID(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2834  GetMachineID: TPlayObject_GetMachineID
    bool GetIsReadyRun(IPlayObjectHandle pPlayer);        // 原 2835  GetIsReadyRun: TPlayObject_GetIsReadyRun
    bool GetLogonTime(IPlayObjectHandle pPlayer, IntPtr LogonTime);        // 原 2836  GetLogonTime: TPlayObject_GetLogonTime
    int GetSoftVerDate(IPlayObjectHandle pPlayer);        // 原 2837  GetSoftVerDate: TPlayObject_GetSoftVerDate
    int GetClientType(IPlayObjectHandle pPlayer);        // 原 2838  GetClientType: TPlayObject_GetClientType
    bool IsOldClient(IPlayObjectHandle pPlayer);        // 原 2839  IsOldClient: TPlayObject_IsOldClient
    ushort GetScreenWidth(IPlayObjectHandle pPlayer);        // 原 2840  GetScreenWidth: TPlayObject_GetScreenWidth
    ushort GetScreenHeight(IPlayObjectHandle pPlayer);        // 原 2841  GetScreenHeight: TPlayObject_GetScreenHeight
    ushort GetClientViewRange(IPlayObjectHandle pPlayer);        // 原 2842  GetClientViewRange: TPlayObject_GetClientViewRange
    byte GetRelevel(IPlayObjectHandle pPlayer);        // 原 2843  GetRelevel: TPlayObject_GetRelevel
    void SetRelevel(IPlayObjectHandle pPlayer, byte pValue);        // 原 2844  SetRelevel: TPlayObject_SetRelevel
    int GetBonusPoint(IPlayObjectHandle pPlayer);        // 原 2845  GetBonusPoint: TPlayObject_GetBonusPoint
    void SetBonusPoint(IPlayObjectHandle pPlayer, int pValue);        // 原 2846  SetBonusPoint: TPlayObject_SetBonusPoint
    void SendAdjustBonus(IPlayObjectHandle pPlayer);        // 原 2847  SendAdjustBonus: TPlayObject_SendAdjustBonus
    bool GetHeroName(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2848  GetHeroName: TPlayObject_GetHeroName
    bool GetDeputyHeroName(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2849  GetDeputyHeroName: TPlayObject_GetDeputyHeroName
    byte GetDeputyHeroJob(IPlayObjectHandle pPlayer);        // 原 2850  GetDeputyHeroJob: TPlayObject_GetDeputyHeroJob
    IHeroObjectHandle GetMyHero(IPlayObjectHandle pPlayer);        // 原 2851  GetMyHero: TPlayObject_GetMyHero
    bool GetFixedHero(IPlayObjectHandle pPlayer);        // 原 2852  GetFixedHero: TPlayObject_GetFixedHero
    void ClientHeroLogOn(IPlayObjectHandle pPlayer, bool IsDeputyHero);        // 原 2853  ClientHeroLogOn: TPlayObject_ClientHeroLogOn
    bool GetStorageHero(IPlayObjectHandle pPlayer);        // 原 2854  GetStorageHero: TPlayObject_GetStorageHero
    bool GetStorageDeputyHero(IPlayObjectHandle pPlayer);        // 原 2855  GetStorageDeputyHero: TPlayObject_GetStorageDeputyHero
    bool GetIsStorageOpen(IPlayObjectHandle pPlayer, int pIndex);        // 原 2856  GetIsStorageOpen: TPlayObject_GetIsStorageOpen
    void SetIsStorageOpen(IPlayObjectHandle pPlayer, int pIndex, bool pValue);        // 原 2857  SetIsStorageOpen: TPlayObject_SetIsStorageOpen
    uint GetGold(IPlayObjectHandle pPlayer);        // 原 2858  GetGold: TPlayObject_GetGold
    void SetGold(IPlayObjectHandle pPlayer, uint pValue);        // 原 2859  SetGold: TPlayObject_SetGold
    uint GetGoldMax(IPlayObjectHandle pPlayer);        // 原 2860  GetGoldMax: TPlayObject_GetGoldMax
    bool IncGold(IPlayObjectHandle pPlayer, uint pValue);        // 原 2861  IncGold: TPlayObject_IncGold
    bool DecGold(IPlayObjectHandle pPlayer, uint pValue);        // 原 2862  DecGold: TPlayObject_DecGold
    void GoldChanged(IPlayObjectHandle pPlayer);        // 原 2863  GoldChanged: TPlayObject_GoldChanged
    uint GetGameGold(IPlayObjectHandle pPlayer);        // 原 2864  GetGameGold: TPlayObject_GetGameGold
    void SetGameGold(IPlayObjectHandle pPlayer, uint pValue);        // 原 2865  SetGameGold: TPlayObject_SetGameGold
    void IncGameGold(IPlayObjectHandle pPlayer, uint pValue);        // 原 2866  IncGameGold: TPlayObject_IncGameGold
    void DecGameGold(IPlayObjectHandle pPlayer, uint pValue);        // 原 2867  DecGameGold: TPlayObject_DecGameGold
    void GameGoldChanged(IPlayObjectHandle pPlayer);        // 原 2868  GameGoldChanged: TPlayObject_GameGoldChanged
    uint GetGamePoint(IPlayObjectHandle pPlayer);        // 原 2869  GetGamePoint: TPlayObject_GetGamePoint
    void SetGamePoint(IPlayObjectHandle pPlayer, uint pValue);        // 原 2870  SetGamePoint: TPlayObject_SetGamePoint
    void IncGamePoint(IPlayObjectHandle pPlayer, uint pValue);        // 原 2871  IncGamePoint: TPlayObject_IncGamePoint
    void DecGamePoint(IPlayObjectHandle pPlayer, uint pValue);        // 原 2872  DecGamePoint: TPlayObject_DecGamePoint
    uint GetGameDiamond(IPlayObjectHandle pPlayer);        // 原 2873  GetGameDiamond: TPlayObject_GetGameDiamond
    void SetGameDiamond(IPlayObjectHandle pPlayer, uint pValue);        // 原 2874  SetGameDiamond: TPlayObject_SetGameDiamond
    void IncGameDiamond(IPlayObjectHandle pPlayer, uint pValue);        // 原 2875  IncGameDiamond: TPlayObject_IncGameDiamond
    void DecGameDiamond(IPlayObjectHandle pPlayer, uint pValue);        // 原 2876  DecGameDiamond: TPlayObject_DecGameDiamond
    void NewGamePointChanged(IPlayObjectHandle pPlayer);        // 原 2877  NewGamePointChanged: TPlayObject_NewGamePointChanged
    uint GetGameGird(IPlayObjectHandle pPlayer);        // 原 2878  GetGameGird: TPlayObject_GetGameGird
    void SetGameGird(IPlayObjectHandle pPlayer, uint pValue);        // 原 2879  SetGameGird: TPlayObject_SetGameGird
    void IncGameGird(IPlayObjectHandle pPlayer, uint pValue);        // 原 2880  IncGameGird: TPlayObject_IncGameGird
    void DecGameGird(IPlayObjectHandle pPlayer, uint pValue);        // 原 2881  DecGameGird: TPlayObject_DecGameGird
    int GetGameGoldEx(IPlayObjectHandle pPlayer);        // 原 2882  GetGameGoldEx: TPlayObject_GetGameGoldEx
    void SetGameGoldEx(IPlayObjectHandle pPlayer, int pValue);        // 原 2883  SetGameGoldEx: TPlayObject_SetGameGoldEx
    int GetGameGlory(IPlayObjectHandle pPlayer);        // 原 2884  GetGameGlory: TPlayObject_GetGameGlory
    void SetGameGlory(IPlayObjectHandle pPlayer, int pValue);        // 原 2885  SetGameGlory: TPlayObject_SetGameGlory
    void IncGameGlory(IPlayObjectHandle pPlayer, int pValue);        // 原 2886  IncGameGlory: TPlayObject_IncGameGlory
    void DecGameGlory(IPlayObjectHandle pPlayer, int pValue);        // 原 2887  DecGameGlory: TPlayObject_DecGameGlory
    void GameGloryChanged(IPlayObjectHandle pPlayer);        // 原 2888  GameGloryChanged: TPlayObject_GameGloryChanged
    int GetPayMentPoint(IPlayObjectHandle pPlayer);        // 原 2889  GetPayMentPoint: TPlayObject_GetPayMentPoint
    void SetPayMentPoint(IPlayObjectHandle pPlayer, int pValue);        // 原 2890  SetPayMentPoint: TPlayObject_SetPayMentPoint
    int GetMemberType(IPlayObjectHandle pPlayer);        // 原 2891  GetMemberType: TPlayObject_GetMemberType
    void SetMemberType(IPlayObjectHandle pPlayer, int pValue);        // 原 2892  SetMemberType: TPlayObject_SetMemberType
    int GetMemberLevel(IPlayObjectHandle pPlayer);        // 原 2893  GetMemberLevel: TPlayObject_GetMemberLevel
    void SetMemberLevel(IPlayObjectHandle pPlayer, int pValue);        // 原 2894  SetMemberLevel: TPlayObject_SetMemberLevel
    ushort GetContribution(IPlayObjectHandle pPlayer);        // 原 2895  GetContribution: TPlayObject_GetContribution
    void SetContribution(IPlayObjectHandle pPlayer, ushort pValue);        // 原 2896  SetContribution: TPlayObject_SetContribution
    void IncExp(IPlayObjectHandle pPlayer, uint pValue);        // 原 2897  IncExp: TPlayObejct_IncExp
    void SendExpChanged(IPlayObjectHandle pPlayer);        // 原 2898  SendExpChanged: TPlayObject_SendExpChanged
    void IncExpNG(IPlayObjectHandle pPlayer, uint pValue);        // 原 2899  IncExpNG: TPlayObject_IncExpNG
    void SendExpNGChanged(IPlayObjectHandle pPlayer);        // 原 2900  SendExpNGChanged: TPlayObject_SendExpNGChanged
    void IncBeadExp(IPlayObjectHandle pPlayer, uint pValue, bool IsFromNPC);        // 原 2901  IncBeadExp: TPlayObject_IncBeadExp
    int GetVarP(IPlayObjectHandle pPlayer, int pIndex);        // 原 2902  GetVarP: TPlayObject_GetVarP
    void SetVarP(IPlayObjectHandle pPlayer, int pIndex, int pValue);        // 原 2903  SetVarP: TPlayObject_SetVarP
    int GetVarM(IPlayObjectHandle pPlayer, int pIndex);        // 原 2904  GetVarM: TPlayObject_GetVarM
    void SetVarM(IPlayObjectHandle pPlayer, int pIndex, int pValue);        // 原 2905  SetVarM: TPlayObject_SetVarM
    int GetVarD(IPlayObjectHandle pPlayer, int pIndex);        // 原 2906  GetVarD: TPlayObject_GetVarD
    void SetVarD(IPlayObjectHandle pPlayer, int pIndex, int pValue);        // 原 2907  SetVarD: TPlayObject_SetVarD
    int GetVarU(IPlayObjectHandle pPlayer, int pIndex);        // 原 2908  GetVarU: TPlayObject_GetVarU
    void SetVarU(IPlayObjectHandle pPlayer, int pIndex, int pValue);        // 原 2909  SetVarU: TPlayObject_SetVarU
    bool GetVarT(IPlayObjectHandle pPlayer, int pIndex, byte[] Dest, uint DestLen);        // 原 2910  GetVarT: TPlayObject_GetVarT
    void SetVarT(IPlayObjectHandle pPlayer, int pIndex, byte[] pValue);        // 原 2911  SetVarT: TPlayObject_SetVarT
    int GetVarN(IPlayObjectHandle pPlayer, int pIndex);        // 原 2912  GetVarN: TPlayObject_GetVarN
    void SetVarN(IPlayObjectHandle pPlayer, int pIndex, int pValue);        // 原 2913  SetVarN: TPlayObject_SetVarN
    bool GetVarS(IPlayObjectHandle pPlayer, int pIndex, byte[] Dest, uint DestLen);        // 原 2914  GetVarS: TPlayObject_GetVarS
    void SetVarS(IPlayObjectHandle pPlayer, int pIndex, byte[] pValue);        // 原 2915  SetVarS: TPlayObject_SetVarS
    IListHandle GetDynamicVarList(IPlayObjectHandle pPlayer);        // 原 2916  GetDynamicVarList: TPlayObject_GetDynamicVarList
    int GetQuestFlagStatus(IPlayObjectHandle pPlayer, int nFlag);        // 原 2917  GetQuestFlagStatus: TPlayObject_GetQuestFlagStatus
    void SetQuestFlagStatus(IPlayObjectHandle pPlayer, int nFlag, int pValue);        // 原 2918  SetQuestFlagStatus: TPlayObject_SetQuestFlagStatus
    bool IsOffLine(IPlayObjectHandle pPlayer);        // 原 2919  IsOffLine: TPlayObject_IsOffLine
    bool IsMaster(IPlayObjectHandle pPlayer);        // 原 2920  IsMaster: TPlayObject_IsMaster
    bool GetMasterName(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2921  GetMasterName: TPlayObject_GetMasterName
    IPlayObjectHandle GetMasterHuman(IPlayObjectHandle pPlayer);        // 原 2922  GetMasterHuman: TPlayObject_GetMasterHuman
    int GetApprenticeNO(IPlayObjectHandle pPlayer);        // 原 2923  GetApprenticeNO: TPlayObject_GetApprenticeNO
    IListHandle GetOnlineApprenticeList(IPlayObjectHandle pPlayer);        // 原 2924  GetOnlineApprenticeList: TPlayObject_GetOnlineApprenticeList
    IListHandle GetAllApprenticeList(IPlayObjectHandle pPlayer);        // 原 2925  GetAllApprenticeList: TPlayObject_GetAllApprenticeList
    bool GetDearName(IPlayObjectHandle pPlayer, byte[] Dest, uint DestLen);        // 原 2926  GetDearName: TPlayObject_GetDearName
    IPlayObjectHandle GetDearHuman(IPlayObjectHandle pPlayer);        // 原 2927  GetDearHuman: TPlayObject_GetDearHuman
    byte GetMarryCount(IPlayObjectHandle pPlayer);        // 原 2928  GetMarryCount: TPlayObject_GetMarryCount
    IPlayObjectHandle GetGroupOwner(IPlayObjectHandle pPlayer);        // 原 2929  GetGroupOwner: TPlayObject_GetGroupOwner
    IStringListHandle GetGroupMembers(IPlayObjectHandle pPlayer);        // 原 2930  GetGroupMembers: TPlayObject_GetGroupMembers
    bool GetIsLockLogin(IPlayObjectHandle pPlayer);        // 原 2931  GetIsLockLogin: TPlayObject_GetIsLockLogin
    void SetIsLockLogin(IPlayObjectHandle pPlayer, bool pValue);        // 原 2932  SetIsLockLogin: TPlayObject_SetIsLockLogin
    bool GetIsAllowGroup(IPlayObjectHandle pPlayer);        // 原 2933  GetIsAllowGroup: TPlayObject_GetIsAllowGroup
    void SetIsAllowGroup(IPlayObjectHandle pPlayer, bool pValue);        // 原 2934  SetIsAllowGroup: TPlayObject_SetIsAllowGroup
    bool GetIsAllowGroupReCall(IPlayObjectHandle pPlayer);        // 原 2935  GetIsAllowGroupReCall: TPlayObject_GetIsAllowGroupReCall
    void SetIsAllowGroupReCall(IPlayObjectHandle pPlayer, bool pValue);        // 原 2936  SetIsAllowGroupReCall: TPlayObject_SetIsAllowGroupReCall
    bool GetIsAllowGuildReCall(IPlayObjectHandle pPlayer);        // 原 2937  GetIsAllowGuildReCall: TPlayObject_GetIsAllowGuildReCall
    void SetIsAllowGuildReCall(IPlayObjectHandle pPlayer, bool pValue);        // 原 2938  SetIsAllowGuildReCall: TPlayObject_SetIsAllowGuildReCall
    bool GetIsAllowTrading(IPlayObjectHandle pPlayer);        // 原 2939  GetIsAllowTrading: TPlayObject_GetIsAllowTrading
    void SetIsAllowTrading(IPlayObjectHandle pPlayer, bool pValue);        // 原 2940  SetIsAllowTrading: TPlayObject_SetIsAllowTrading
    bool GetIsDisableInviteHorseRiding(IPlayObjectHandle pPlayer);        // 原 2941  GetIsDisableInviteHorseRiding: TPlayObject_GetIsDisableInviteHorseRiding
    void SetIsDisableInviteHorseRiding(IPlayObjectHandle pPlayer, bool pValue);        // 原 2942  SetIsDisableInviteHorseRiding: TPlayObject_SetIsDisableInviteHorseRiding
    bool GetIsGameGoldTrading(IPlayObjectHandle pPlayer);        // 原 2943  GetIsGameGoldTrading: TPlayObject_GetIsGameGoldTrading
    void SetIsGameGoldTrading(IPlayObjectHandle pPlayer, bool pValue);        // 原 2944  SetIsGameGoldTrading: TPlayObject_SetIsGameGoldTrading
    bool GetIsNewServer(IPlayObjectHandle pPlayer);        // 原 2945  GetIsNewServer: TPlayObject_GetIsNewServer
    bool GetIsFilterGlobalDropItemMsg(IPlayObjectHandle pPlayer);        // 原 2946  GetIsFilterGlobalDropItemMsg: TPlayObject_GetIsFilterGlobalDropItemMsg
    void SetIsFilterGlobalDropItemMsg(IPlayObjectHandle pPlayer, bool pValue);        // 原 2947  SetIsFilterGlobalDropItemMsg: TPlayObject_SetIsFilterGlobalDropItemMsg
    bool GetIsFilterGlobalCenterMsg(IPlayObjectHandle pPlayer);        // 原 2948  GetIsFilterGlobalCenterMsg: TPlayObject_GetIsFilterGlobalCenterMsg
    void SetIsFilterGlobalCenterMsg(IPlayObjectHandle pPlayer, bool pValue);        // 原 2949  SetIsFilterGlobalCenterMsg: TPlayObject_SetIsFilterGlobalCenterMsg
    bool GetIsFilterGolbalSendMsg(IPlayObjectHandle pPlayer);        // 原 2950  GetIsFilterGolbalSendMsg: TPlayObject_GetIsFilterGolbalSendMsg
    void SetIsFilterGolbalSendMsg(IPlayObjectHandle pPlayer, bool pValue);        // 原 2951  SetIsFilterGolbalSendMsg: TPlayObject_SetIsFilterGolbalSendMsg
    bool GetIsPleaseDrink(IPlayObjectHandle pPlayer);        // 原 2952  GetIsPleaseDrink: TPlayObject_GetIsPleaseDrink
    int GetIsDrinkWineQuality(IPlayObjectHandle pPlayer);        // 原 2953  GetIsDrinkWineQuality: TPlayObject_GetIsDrinkWineQuality
    void SetIsDrinkWineQuality(IPlayObjectHandle pPlayer, int pValue);        // 原 2954  SetIsDrinkWineQuality: TPlayObject_SetIsDrinkWineQuality
    int GetIsDrinkWineAlcohol(IPlayObjectHandle pPlayer);        // 原 2955  GetIsDrinkWineAlcohol: TPlayObject_GetIsDrinkWineAlcohol
    void SetIsDrinkWineAlcohol(IPlayObjectHandle pPlayer, int pValue);        // 原 2956  SetIsDrinkWineAlcohol: TPlayObject_SetIsDrinkWineAlcohol
    bool GetIsDrinkWineDrunk(IPlayObjectHandle pPlayer);        // 原 2957  GetIsDrinkWineDrunk: TPlayObject_GetIsDrinkWineDrunk
    void SetIsDrinkWineDrunk(IPlayObjectHandle pPlayer, bool pValue);        // 原 2958  SetIsDrinkWineDrunk: TPlayObject_SetIsDrinkWineDrunk
    void MoveToHome(IPlayObjectHandle pPlayer);        // 原 2959  MoveToHome: TPlayObject_MoveToHome
    void MoveRandomToHome(IPlayObjectHandle pPlayer);        // 原 2960  MoveRandomToHome: TPlayObject_MoveRandomToHome
    void SendSocket(IPlayObjectHandle pPlayer, TDefaultMessage DefMsg, byte[] sMsg);        // 原 2961  SendSocket: TPlayObject_SendSocket
    void SendDefMessage(IPlayObjectHandle pPlayer, ushort wIdent, long nRecog, ushort nParam, ushort nTag, ushort nSeries, byte[] sMsg);        // 原 2962  SendDefMessage: TPlayObject_SendDefMessage
    void SendMoveMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, ushort nY, int nMoveCount, int nFontSize, int nMarqueeTime);        // 原 2963  SendMoveMsg: TPlayObject_SendMoveMsg
    void SendCenterMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, int nTime);        // 原 2964  SendCenterMsg: TPlayObject_SendCenterMsg
    bool SendTopBroadCastMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, int nTime, int MsgType);        // 原 2965  SendTopBroadCastMsg: TPlayObject_SendTopBroadCastMsg
    bool CheckTakeOnItems(IPlayObjectHandle pPlayer, int Where, TStdItem StdItem);        // 原 2966  CheckTakeOnItems: TPlayObject_CheckTakeOnItems
    void ProcessUseItemSkill(IPlayObjectHandle pPlayer, int Where, TStdItem StdItem, bool IsTakeOn);        // 原 2967  ProcessUseItemSkill: TPlayObject_ProcessUseItemSkill
    void SendUseItems(IPlayObjectHandle pPlayer);        // 原 2968  SendUseItems: TPlayObject_SendUseItems
    void SendAddItem(IPlayObjectHandle pPlayer, TUserItem UserItem);        // 原 2969  SendAddItem: TPlayObject_SendAddItem
    void SendDelItemList(IPlayObjectHandle pPlayer, byte[] Items, int ItemsCount);        // 原 2970  SendDelItemList: TPlayObject_SendDelItemList
    void SendDelItem(IPlayObjectHandle pPlayer, TUserItem UserItem);        // 原 2971  SendDelItem: TPlayObject_SendDelItem
    void SendUpdateItem(IPlayObjectHandle pPlayer, TUserItem UserItem);        // 原 2972  SendUpdateItem: TPlayObject_SendUpdateItem
    void SendItemDuraChange(IPlayObjectHandle pPlayer, int ItemWhere, TUserItem UserItem);        // 原 2973  SendItemDuraChange: TPlayObject_SendItemDuraChange
    void SendBagItems(IPlayObjectHandle Plyaer);        // 原 2974  SendBagItems: TPlayObject_SendBagItems
    void SendJewelryBoxItems(IPlayObjectHandle pPlayer);        // 原 2975  SendJewelryBoxItems: TPlayObject_SendJewelryBoxItems
    void SendGodBlessItems(IPlayObjectHandle pPlayer);        // 原 2976  SendGodBlessItems: TPlayObject_SendGodBlessItems
    void SendOpenGodBlessItem(IPlayObjectHandle pPlayer, int pIndex);        // 原 2977  SendOpenGodBlessItem: TPlayObject_SendOpenGodBlessItem
    void SendCloseGodBlessItem(IPlayObjectHandle pPlayer, int pIndex);        // 原 2978  SendCloseGodBlessItem: TPlayObject_SendCloseGodBlessItem
    void SendUseMagics(IPlayObjectHandle pPlayer);        // 原 2979  SendUseMagics: TPlayObject_SendUseMagics
    void SendAddMagic(IPlayObjectHandle pPlayer, TUserMagic UserMagic);        // 原 2980  SendAddMagic: TPlayObject_SendAddMagic
    void SendDelMagic(IPlayObjectHandle pPlayer, TUserMagic UserMagic);        // 原 2981  SendDelMagic: TPlayObject_SendDelMagic
    void SendFengHaoItems(IPlayObjectHandle pPlayer);        // 原 2982  SendFengHaoItems: TPlayObject_SendFengHaoItems
    void SendAddFengHaoItem(IPlayObjectHandle pPlayer, TUserItem UserItem);        // 原 2983  SendAddFengHaoItem: TPlayObject_SendAddFengHaoItem
    void SendDelFengHaoItem(IPlayObjectHandle pPlayer, int pIndex);        // 原 2984  SendDelFengHaoItem: TPlayObject_SendDelFengHaoItem
    void SendSocketStatusFail(IPlayObjectHandle pPlayer);        // 原 2985  SendSocketStatusFail: TPlayObject_SendSocketStatusFail
    void PlayEffect(IPlayObjectHandle pPlayer, int nFileIndex, int nImageOffset, int nImageCount, int nLoopCount, int nSpeedTime, byte btDrawOrder, int nOffsetX, int nOffsetY);        // 原 2986  PlayEffect: TPlayObject_PlayEffect
    bool IsAutoPlayGame(IPlayObjectHandle pPlayer);        // 原 2987  IsAutoPlayGame: TPlayObject_IsAutoPlayGame
    bool StartAutoPlayGame(IPlayObjectHandle pPlayer);        // 原 2988  StartAutoPlayGame: TPlayObject_StartAutoPlayGame
    bool StopAutoPlayGame(IPlayObjectHandle pPlayer);        // 原 2989  StopAutoPlayGame: TPlayObject_StopAutoPlayGame
}

/// <summary>原文 TDummyObjectFunc 对应的托管接口 IDummyObjectFunc（4 个成员）。</summary>
public interface IDummyObjectFunc
{
    bool IsStart(IDummyObjectHandle Dummyer);        // 原 2994  IsStart: TDummyObject_IsStart
    void Start(IDummyObjectHandle Dummyer);        // 原 2995  Start: TDummyObject_Start
    void Stop(IDummyObjectHandle Dummyer);        // 原 2996  Stop: TDummyObject_Stop
}

/// <summary>原文 THeroObjectFunc 对应的托管接口 IHeroObjectFunc（34 个成员）。</summary>
public interface IHeroObjectFunc
{
    byte GetAttackMode(IHeroObjectHandle pHero);        // 原 3001  GetAttackMode: THeroObject_GetAttackMode
    bool SetAttackMode(IHeroObjectHandle pHero, byte pValue, bool ShowSysMsg);        // 原 3002  SetAttackMode: THeroObject_SetAttackMode
    void SetNextAttackMode(IHeroObjectHandle pHero);        // 原 3003  SetNextAttackMode: THeroObject_SetNextAttackMode
    int GetBagCount(IHeroObjectHandle pHero);        // 原 3004  GetBagCount: THeroObject_GetBagCount
    int GetAngryValue(IHeroObjectHandle pHero);        // 原 3005  GetAngryValue: THeroObject_GetAngryValue
    double GetLoyalPoint(IHeroObjectHandle pHero);        // 原 3006  GetLoyalPoint: THeroObject_GetLoyalPoint
    void SetLoyalPoint(IHeroObjectHandle pHero, double pValue);        // 原 3007  SetLoyalPoint: THeroObject_SetLoyalPoint
    void SendLoyalPointChanged(IHeroObjectHandle pHero);        // 原 3008  SendLoyalPointChanged: THeroObject_SendLoyalPointChanged
    bool IsDeputy(IHeroObjectHandle pHero);        // 原 3009  IsDeputy: THeroObject_IsDeputy
    bool GetMasterName(IHeroObjectHandle pHero, byte[] Dest, uint DestLen);        // 原 3010  GetMasterName: THeroObject_GetMasterName
    int GetQuestFlagStatus(IHeroObjectHandle pHero, int nFlag);        // 原 3011  GetQuestFlagStatus: THeroObject_GetQuestFlagStatus
    void SetQuestFlagStatus(IHeroObjectHandle pHero, int nFlag, int pValue);        // 原 3012  SetQuestFlagStatus: THeroObject_SetQuestFlagStatus
    void SendUseItems(IHeroObjectHandle pHero);        // 原 3013  SendUseItems: THeroObject_SendUseItems
    void SendBagItems(IHeroObjectHandle pHero);        // 原 3014  SendBagItems: THeroObject_SendBagItems
    void SendJewelryBoxItems(IHeroObjectHandle pHero);        // 原 3015  SendJewelryBoxItems: THeroObject_SendJewelryBoxItems
    void SendGodBlessItems(IHeroObjectHandle pHero);        // 原 3016  SendGodBlessItems: THeroObject_SendGodBlessItems
    void SendOpenGodBlessItem(IHeroObjectHandle pHero, int pIndex);        // 原 3017  SendOpenGodBlessItem: THeroObject_SendOpenGodBlessItem
    void SendCloseGodBlessItem(IHeroObjectHandle pHero, int pIndex);        // 原 3018  SendCloseGodBlessItem: THeroObject_SendCloseGodBlessItem
    void SendAddItem(IHeroObjectHandle pHero, TUserItem UserItem);        // 原 3019  SendAddItem: THeroObject_SendAddItem
    void SendDelItem(IHeroObjectHandle pHero, TUserItem UserItem);        // 原 3020  SendDelItem: THeroObject_SendDelItem
    void SendUpdateItem(IHeroObjectHandle pHero, TUserItem UserItem);        // 原 3021  SendUpdateItem: THeroObject_SendUpdateItem
    void SendItemDuraChange(IHeroObjectHandle pHero, int ItemWhere, TUserItem UserItem);        // 原 3022  SendItemDuraChange: THeroObject_SendItemDuraChange
    void SendUseMagics(IHeroObjectHandle pHero);        // 原 3023  SendUseMagics: THeroObject_SendUseMagics
    void SendAddMagic(IHeroObjectHandle pHero, TUserMagic UserMagic);        // 原 3024  SendAddMagic: THeroObject_SendAddMagic
    void SendDelMagic(IHeroObjectHandle pHero, TUserMagic UserMagic);        // 原 3025  SendDelMagic: THeroObject_SendDelMagic
    bool FindGroupMagic(IHeroObjectHandle pHero, TUserMagic UserMagic);        // 原 3026  FindGroupMagic: THeroObject_FindGroupMagic
    int GetGroupMagicId(IHeroObjectHandle pHero);        // 原 3027  GetGroupMagicId: THeroObject_GetGroupMagicId
    void SendFengHaoItems(IHeroObjectHandle pHero);        // 原 3028  SendFengHaoItems: THeroObject_SendFengHaoItems
    void SendAddFengHaoItem(IHeroObjectHandle pHero, TUserItem UserItem);        // 原 3029  SendAddFengHaoItem: THeroObject_SendAddFengHaoItem
    void SendDelFengHaoItem(IHeroObjectHandle pHero, int pIndex);        // 原 3030  SendDelFengHaoItem: THeroObject_SendDelFengHaoItem
    void IncExp(IHeroObjectHandle pHero, uint dwExp);        // 原 3031  IncExp: THeroObject_IncExp
    void IncExpNG(IHeroObjectHandle pHero, uint dwExp);        // 原 3032  IncExpNG: THeroObject_IncExpNG
    bool IsOldClient(IHeroObjectHandle pHero);        // 原 3033  IsOldClient: THeroObject_IsOldClient
}

/// <summary>原文 TNormNpcFunc 对应的托管接口 INormNpcFunc（19 个成员）。</summary>
public interface INormNpcFunc
{
    INormNpcHandle Create(byte[] CharName, byte[] sMapName, byte[] sScript, int X, int Y, ushort wAppr, bool boIsHide);        // 原 3038  Create: TNormNpc_Create
    void LoadNpcScript(INormNpcHandle NormNpc);        // 原 3039  LoadNpcScript: TNormNpc_LoadNpcScript
    void ClearScript(INormNpcHandle NormNpc);        // 原 3040  ClearScript: TNormNpc_ClearScript
    bool GetFilePath(INormNpcHandle NormNpc, byte[] Dest, uint DestLen);        // 原 3041  GetFilePath: TNormNpc_GetFilePath
    void SetFilePath(INormNpcHandle NormNpc, byte[] pValue);        // 原 3042  SetFilePath: TNormNpc_SetFilePath
    bool GetPath(INormNpcHandle NormNpc, byte[] Dest, uint DestLen);        // 原 3043  GetPath: TNormNpc_GetPath
    void SetPath(INormNpcHandle NormNpc, byte[] pValue);        // 原 3044  SetPath: TNormNpc_SetPath
    bool GetIsHide(INormNpcHandle NormNpc);        // 原 3045  GetIsHide: TNormNpc_GetIsHide
    void SetIsHide(INormNpcHandle NormNpc, bool pValue);        // 原 3046  SetIsHide: TNormNpc_SetIsHide
    bool GetIsQuest(INormNpcHandle NormNpc);        // 原 3047  GetIsQuest: TNormNpc_GetIsQuest
    bool GetLineVariableText(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg, byte[] Dest, uint DestLen);        // 原 3048  GetLineVariableText: TNormNpc_GetLineVariableText
    void GotoLable(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sLabel, bool boExtJmp);        // 原 3049  GotoLable: TNormNpc_GotoLable
    void SendMsgToUser(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg);        // 原 3050  SendMsgToUser: TNormNpc_SendMsgToUser
    void MessageBox(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg);        // 原 3051  MessageBox: TNormNpc_MessageBox
    bool GetVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, uint sValueSize, int nValue);        // 原 3052  GetVarValue: TNormNpc_GetVarValue
    bool SetVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, int nValue);        // 原 3053  SetVarValue: TNormNpc_SetVarValue
    bool GetDynamicVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, uint sValueSize, int nValue);        // 原 3054  GetDynamicVarValue: TNormNpc_GetDynamicVarValue
    bool SetDynamicVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, int nValue);        // 原 3055  SetDynamicVarValue: TNormNpc_SetDynamicVarValue
}

/// <summary>原文 TUserEngineFunc 对应的托管接口 IUserEngineFunc（54 个成员）。</summary>
public interface IUserEngineFunc
{
    IStringListHandle GetPlayerList();        // 原 3060  GetPlayerList: TUserEngine_GetPlayerList
    IPlayObjectHandle GetPlayerByName(byte[] ChrName);        // 原 3061  GetPlayerByName: TUserEngine_GetPlayerByName
    IPlayObjectHandle GetPlayerByUserID(byte[] UserID);        // 原 3062  GetPlayerByUserID: TUserEngine_GetPlayerByUserID
    IPlayObjectHandle GetPlayerByObject(object AObject);        // 原 3063  GetPlayerByObject: TUserEngine_GetPlayerByObject
    IPlayObjectHandle GetOfflinePlayer(byte[] UserID);        // 原 3064  GetOfflinePlayer: TUserEngine_GetOfflinePlayer
    void KickPlayer(byte[] ChrName);        // 原 3065  KickPlayer: TUserEngine_KickPlayer
    IStringListHandle GetHeroList();        // 原 3066  GetHeroList: TUserEngine_GetHeroList
    IHeroObjectHandle GetHeroByName(byte[] ChrName);        // 原 3067  GetHeroByName: TUserEngine_GetHeroByName
    bool KickHero(byte[] ChrName);        // 原 3068  KickHero: TUserEngine_KickHero
    IListHandle GetMerchantList();        // 原 3069  GetMerchantList: TUserEngine_GetMerchantList
    IListHandle GetCustomNpcConfigList();        // 原 3070  GetCustomNpcConfigList: TUserEngine_GetCustomNpcConfigList
    IStringListHandle GetQuestNPCList();        // 原 3071  GetQuestNPCList: TUserEngine_GetQuestNPCList
    INormNpcHandle GetManageNPC();        // 原 3072  GetManageNPC: TUserEngine_GetManageNPC
    INormNpcHandle GetFunctionNPC();        // 原 3073  GetFunctionNPC: TUserEngine_GetFunctionNPC
    INormNpcHandle GetRobotNPC();        // 原 3074  GetRobotNPC: TUserEngine_GetRobotNPC
    INormNpcHandle MissionNPC();        // 原 3075  MissionNPC: TUserEngine_MissionNPC
    INormNpcHandle FindMerchant(object AObject);        // 原 3076  FindMerchant: TUserEngine_FindMerchant
    INormNpcHandle FindMerchantByPos(byte[] MapName, int nX, int nY);        // 原 3077  FindMerchantByPos: TUserEngine_FindMerchantByPos
    INormNpcHandle FindQuestNPC(object AObject);        // 原 3078  FindQuestNPC: TUserEngine_FindQuestNPC
    IListHandle GetMagicList();        // 原 3079  GetMagicList: TUserEngine_GetMagicList
    IListHandle GetCustomMagicConfigList();        // 原 3080  GetCustomMagicConfigList: TUserEngine_GetCustomMagicConfigList
    IMagicACListHandle GetMagicACList();        // 原 3081  GetMagicACList: TUserEngine_GetMagicACList
    bool FindMagicByName(byte[] MagName, TMagic Magic);        // 原 3082  FindMagicByName: TUserEngine_FindMagicByName
    bool FindMagicByIndex(int MagIdx, TMagic Magic);        // 原 3083  FindMagicByIndex: TUserEngine_FindMagicByIndex
    bool FindMagicByNameEx(byte[] MagName, int MagAttr, TMagic Magic);        // 原 3084  FindMagicByNameEx: TUserEngine_FindMagicByNameEx
    bool FindMagicByIndexEx(int MagIdx, int MagAttr, TMagic Magic);        // 原 3085  FindMagicByIndexEx: TUserEngine_FindMagicByIndexEx
    bool FindHeroMagicByName(byte[] MagName, TMagic Magic);        // 原 3086  FindHeroMagicByName: TUserEngine_FindHeroMagicByName
    bool FindHeroMagicByIndex(int MagIdx, TMagic Magic);        // 原 3087  FindHeroMagicByIndex: TUserEngine_FindHeroMagicByIndex
    bool FindHeroMagicByNameEx(byte[] MagName, int MagAttr, TMagic Magic);        // 原 3088  FindHeroMagicByNameEx: TUserEngine_FindHeroMagicByNameEx
    bool FindHeroMagicByIndexEx(int MagIdx, int MagAttr, TMagic Magic);        // 原 3089  FindHeroMagicByIndexEx: TUserEngine_FindHeroMagicByIndexEx
    IListHandle GetStdItemList();        // 原 3090  GetStdItemList: TUserEngine_GetStdItemList
    bool GetStdItemByName(byte[] ItemName, TStdItem StdItem);        // 原 3091  GetStdItemByName: TUserEngine_GetStdItemByName
    bool GetStdItemByIndex(int ItemIdx, TStdItem StdItem);        // 原 3092  GetStdItemByIndex: TUserEngine_GetStdItemByIndex
    bool GetStdItemName(int ItemIdx, byte[] Dest, uint DestLen);        // 原 3093  GetStdItemName: TUserEngine_GetStdItemName
    int GetStdItemIndex(byte[] ItemName);        // 原 3094  GetStdItemIndex: TUserEngine_GetStdItemIndex
    IListHandle MonsterList();        // 原 3095  MonsterList: TUserEngine_MonsterList
    bool SendBroadCastMsg(byte[] sMsg, int FColor, int BColor, int MsgType);        // 原 3096  SendBroadCastMsg: TUserEngine_SendBroadCastMsg
    bool SendBroadCastMsgExt(byte[] sMsg, int MsgType);        // 原 3097  SendBroadCastMsgExt: TUserEngine_SendBroadCastMsgExt
    bool SendTopBroadCastMsg(byte[] sMsg, int FColor, int BColor, int nTime, int MsgType);        // 原 3098  SendTopBroadCastMsg: TUserEngine_SendTopBroadCastMsg
    void SendMoveMsg(byte[] sMsg, byte btFColor, byte btBColor, int nY, int nMoveCount, int nFontSize, int nMarqueeTime);        // 原 3099  SendMoveMsg: TUserEngine_SendMoveMsg
    void SendCenterMsg(byte[] sMsg, byte btFColor, byte btBColor, int nTime);        // 原 3100  SendCenterMsg: TUserEngine_SendCenterMsg
    void SendNewLineMsg(byte[] sMsg, byte btFColor, byte btBColor, byte btFontSize, int nY, int nShowMsgTime, int nDrawType);        // 原 3101  SendNewLineMsg: TUserEngine_SendNewLineMsg
    void SendSuperMoveMsg(byte[] sMsg, byte btFColor, byte btBColor, byte btFontSize, int nY, int nMoveCount);        // 原 3102  SendSuperMoveMsg: TUserEngine_SendSuperMoveMsg
    void SendSceneShake(int pCount);        // 原 3103  SendSceneShake: TUserEngine_SendSceneShake
    bool CopyToUserItemFromName(byte[] ItemName, TUserItem UserItem);        // 原 3104  CopyToUserItemFromName: TUserEngine_CopyToUserItemFromName
    bool CopyToUserItemFromItem(TStdItem StdItem, int ItemIndex, TUserItem UserItem);        // 原 3105  CopyToUserItemFromItem: TUserEngine_CopyToUserItemFromItem
    void RandomUpgradeItem(TUserItem UserItem);        // 原 3106  RandomUpgradeItem: TUserEngine_RandomUpgradeItem
    void RandomItemNewAbil(TUserItem UserItem);        // 原 3107  RandomItemNewAbil: TUserEngine_RandomItemNewAbil
    void GetUnknowItemValue(TUserItem UserItem);        // 原 3108  GetUnknowItemValue: TUserEngine_GetUnknowItemValue
    int GetAllDummyCount();        // 原 3109  GetAllDummyCount: TUserEngine_GetAllDummyCount
    int GetMapDummyCount(IEnvirnoment Envir);        // 原 3110  GetMapDummyCount: TUserEngine_GetMapDummyCount
    int GetOfflineCount();        // 原 3111  GetOfflineCount: TUserEngine_GetOfflineCount
    int GetRealPlayerCount();        // 原 3112  GetRealPlayerCount: TUserEngine_GetRealPlayerCount
}

/// <summary>原文 TGuildManagerFunc 对应的托管接口 IGuildManagerFunc（5 个成员）。</summary>
public interface IGuildManagerFunc
{
    IGuildHandle FindGuild(byte[] GuildName);        // 原 3117  FindGuild: TGuildManager_FindGuild
    IGuildHandle GetPlayerGuild(byte[] CharName);        // 原 3118  GetPlayerGuild: TGuildManager_GetPlayerGuild
    bool AddGuild(byte[] GuildName, byte[] GuildMaster);        // 原 3119  AddGuild: TGuildManager_AddGuild
    bool DelGuild(byte[] GuildName, bool IsFoundGuild);        // 原 3120  DelGuild: TGuildManager_DelGuild
}

/// <summary>原文 TGuildFunc 对应的托管接口 IGuildFunc（32 个成员）。</summary>
public interface IGuildFunc
{
    bool GetGuildName(IGuildHandle pGuild, byte[] Dest, uint DestLen);        // 原 3125  GetGuildName: TGuild_GetGuildName
    int GetJoinJob(IGuildHandle pGuild);        // 原 3126  GetJoinJob: TGuild_GetJoinJob
    uint GetJoinLevel(IGuildHandle pGuild);        // 原 3127  GetJoinLevel: TGuild_GetJoinLevel
    bool GetJoinMsg(IGuildHandle pGuild, byte[] Dest, uint DestLen);        // 原 3128  GetJoinMsg: TGuild_GetJoinMsg
    int GetBuildPoint(IGuildHandle pGuild);        // 原 3129  GetBuildPoint: TGuild_GetBuildPoint
    int GetAurae(IGuildHandle pGuild);        // 原 3130  GetAurae: TGuild_GetAurae
    int GetStability(IGuildHandle pGuild);        // 原 3131  GetStability: TGuild_GetStability
    int GetFlourishing(IGuildHandle pGuild);        // 原 3132  GetFlourishing: TGuild_GetFlourishing
    int GetChiefItemCount(IGuildHandle pGuild);        // 原 3133  GetChiefItemCount: TGuild_GetChiefItemCount
    int GetMemberCount(IGuildHandle pGuild);        // 原 3134  GetMemberCount: TGuild_GetMemberCount
    int GetOnlineMemeberCount(IGuildHandle pGuild);        // 原 3135  GetOnlineMemeberCount: TGuild_GetOnlineMemeberCount
    int GetMasterCount(IGuildHandle pGuild);        // 原 3136  GetMasterCount: TGuild_GetMasterCount
    void GetMaster(IGuildHandle pGuild, IPlayObjectHandle Master1, IPlayObjectHandle Master2);        // 原 3137  GetMaster: TGuild_GetMaster
    bool GetMasterName(IGuildHandle pGuild, byte[] Master1, uint Master1Size, byte[] Master2, uint Master2Size);        // 原 3138  GetMasterName: TGuild_GetMasterName
    bool CheckMemberIsFull(IGuildHandle pGuild);        // 原 3139  CheckMemberIsFull: TGuild_CheckMemberIsFull
    bool IsMemeber(IGuildHandle pGuild, byte[] CharName);        // 原 3140  IsMemeber: TGuild_IsMemeber
    bool AddMember(IGuildHandle pGuild, IPlayObjectHandle pPlayer);        // 原 3141  AddMember: TGuild_AddMember
    bool AddMemberEx(IGuildHandle pGuild, byte[] CharName);        // 原 3142  AddMemberEx: TGuild_AddMemberEx
    bool DelMemeber(IGuildHandle pGuild, IPlayObjectHandle pPlayer);        // 原 3143  DelMemeber: TGuild_DelMemeber
    bool DelMemeberEx(IGuildHandle pGuild, byte[] CharName);        // 原 3144  DelMemeberEx: TGuild_DelMemeberEx
    bool IsAllianceGuild(IGuildHandle pGuild, IGuildHandle CheckGuild);        // 原 3145  IsAllianceGuild: TGuild_IsAllianceGuild
    bool IsWarGuild(IGuildHandle pGuild, IGuildHandle CheckGuild);        // 原 3146  IsWarGuild: TGuild_IsWarGuild
    bool IsAttentionGuild(IGuildHandle pGuild, IGuildHandle CheckGuild);        // 原 3147  IsAttentionGuild: TGuild_IsAttentionGuild
    bool AddAlliance(IGuildHandle pGuild, IGuildHandle AddGuild);        // 原 3148  AddAlliance: TGuild_AddAlliance
    bool AddWarGuild(IGuildHandle pGuild, IGuildHandle AddGuild);        // 原 3149  AddWarGuild: TGuild_AddWarGuild
    bool AddAttentionGuild(IGuildHandle pGuild, IGuildHandle AddGuild);        // 原 3150  AddAttentionGuild: TGuild_AddAttentionGuild
    bool DelAllianceGuild(IGuildHandle pGuild, IGuildHandle DelGuild);        // 原 3151  DelAllianceGuild: TGuild_DelAllianceGuild
    bool DelAttentionGuild(IGuildHandle pGuild, IGuildHandle DelGuild);        // 原 3152  DelAttentionGuild: TGuild_DelAttentionGuild
    bool GetRandNameByName(IGuildHandle pGuild, byte[] CharName, int nRankNo, byte[] Dest, uint DestLen);        // 原 3153  GetRandNameByName: TGuild_GetRandNameByName
    bool GetRandNameByPlayer(IGuildHandle pGuild, IPlayObjectHandle pPlayer, int nRankNo, byte[] Dest, uint DestLen);        // 原 3154  GetRandNameByPlayer: TGuild_GetRandNameByPlayer
    void SendGuildMsg(IGuildHandle pGuild, byte[] Msg);        // 原 3155  SendGuildMsg: TGuild_SendGuildMsg
}

