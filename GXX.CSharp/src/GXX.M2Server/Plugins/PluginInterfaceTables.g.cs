// =====================================================================================
// PluginInterface.pas 1:1 转换：21 个 Pack=1 记录（TAppFuncDef 及 20 个 T*Func 回调表）。
//
// 字段顺序、字段名与 Reserved 占位逐字对齐；Pack=1 保证与原版 wire 布局不变。
// 函数指针字段用 PluginInterfaceTypes.g.cs 中的 stdcall 委托类型。
//
// 逐条来源与行号见各字段行尾注释与 PluginInterface.manifest.tsv。
// =====================================================================================

using System;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;

namespace GXX.M2Server.Plugins;

/// <summary>原文 TScriptCmdParam（PluginInterface.pas:77-111,34 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TScriptCmdParam
{
    public INormNpcHandle Npc;        // 原 78  Npc: _TNormNpc
    public IPlayObjectHandle PlayObject;        // 原 79  PlayObject: _TPlayObject
    public IBaseObjectHandle BaseObject;        // 原 80  BaseObject: _TBaseObject
    public int nCMDCode;        // 原 81  nCMDCode: Integer
    public IntPtr sRawParam01;        // 原 82  sRawParam01: PAnsiChar
    public IntPtr sRawParam02;        // 原 83  sRawParam02: PAnsiChar
    public IntPtr sRawParam03;        // 原 84  sRawParam03: PAnsiChar
    public IntPtr sRawParam04;        // 原 85  sRawParam04: PAnsiChar
    public IntPtr sRawParam05;        // 原 86  sRawParam05: PAnsiChar
    public IntPtr sRawParam06;        // 原 87  sRawParam06: PAnsiChar
    public IntPtr sRawParam07;        // 原 88  sRawParam07: PAnsiChar
    public IntPtr sRawParam08;        // 原 89  sRawParam08: PAnsiChar
    public IntPtr sRawParam09;        // 原 90  sRawParam09: PAnsiChar
    public IntPtr sRawParam10;        // 原 91  sRawParam10: PAnsiChar
    public IntPtr sParam01;        // 原 92  sParam01: PAnsiChar
    public int nParam01;        // 原 93  nParam01: Integer
    public IntPtr sParam02;        // 原 94  sParam02: PAnsiChar
    public int nParam02;        // 原 95  nParam02: Integer
    public IntPtr sParam03;        // 原 96  sParam03: PAnsiChar
    public int nParam03;        // 原 97  nParam03: Integer
    public IntPtr sParam04;        // 原 98  sParam04: PAnsiChar
    public int nParam04;        // 原 99  nParam04: Integer
    public IntPtr sParam05;        // 原 100  sParam05: PAnsiChar
    public int nParam05;        // 原 101  nParam05: Integer
    public IntPtr sParam06;        // 原 102  sParam06: PAnsiChar
    public int nParam06;        // 原 103  nParam06: Integer
    public IntPtr sParam07;        // 原 104  sParam07: PAnsiChar
    public int nParam07;        // 原 105  nParam07: Integer
    public IntPtr sParam08;        // 原 106  sParam08: PAnsiChar
    public int nParam08;        // 原 107  nParam08: Integer
    public IntPtr sParam09;        // 原 108  sParam09: PAnsiChar
    public int nParam09;        // 原 109  nParam09: Integer
    public IntPtr sParam10;        // 原 110  sParam10: PAnsiChar
    public int nParam10;        // 原 111  nParam10: Integer
}

/// <summary>原文 TMemoryFunc（PluginInterface.pas:2343-2348,4 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMemoryFunc
{
    public TMemory_Alloc Allow;        // 原 2344  Allow: TMemory_Alloc
    public TMemory_Free Free;        // 原 2345  Free: TMemory_Free
    public TMemory_Realloc Realloc;        // 原 2346  Realloc: TMemory_Realloc
    public fixed long Reserved[4];        // 原 2347  Reserved: array of 3 x4
}

/// <summary>原文 TListFunc（PluginInterface.pas:2350-2365,14 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TListFunc
{
    public TList_Create Create;        // 原 2351  Create: TList_Create
    public TList_Free Free;        // 原 2352  Free: TList_Free
    public TList_Count Count;        // 原 2353  Count: TList_Count
    public TList_Clear Clear;        // 原 2354  Clear: TList_Clear
    public TList_Add Add;        // 原 2355  Add: TList_Add
    public TList_Insert Insert;        // 原 2356  Insert: TList_Insert
    public TList_Remove Remove;        // 原 2357  Remove: TList_Remove
    public TList_Delete Delete;        // 原 2358  Delete: TList_Delete
    public TList_GetItem GetItem;        // 原 2359  GetItem: TList_GetItem
    public TList_SetItem SetItem;        // 原 2360  SetItem: TList_SetItem
    public TList_IndexOf IndexOf;        // 原 2361  IndexOf: TList_IndexOf
    public TList_Exchange Exchange;        // 原 2362  Exchange: TList_Exchange
    public TList_CopyTo CopyTo;        // 原 2363  CopyTo: TList_CopyTo
    public fixed long Reserved[20];        // 原 2364  Reserved: array of 19 x20
}

/// <summary>原文 TStringListFunc（PluginInterface.pas:2367-2397,29 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TStringListFunc
{
    public TStrList_Create Create;        // 原 2368  Create: TStrList_Create
    public TStrList_Free Free;        // 原 2369  Free: TStrList_Free
    public TStrList_GetCaseSensitive GetCaseSensitive;        // 原 2370  GetCaseSensitive: TStrList_GetCaseSensitive
    public TStrList_SetCaseSensitive SetCaseSensitive;        // 原 2371  SetCaseSensitive: TStrList_SetCaseSensitive
    public TStrList_GetSorted GetSorted;        // 原 2372  GetSorted: TStrList_GetSorted
    public TStrList_SetSorted SetSorted;        // 原 2373  SetSorted: TStrList_SetSorted
    public TStrList_GetDuplicates GetDuplicates;        // 原 2374  GetDuplicates: TStrList_GetDuplicates
    public TStrList_SetDuplicates SetDuplicates;        // 原 2375  SetDuplicates: TStrList_SetDuplicates
    public TStrList_Count Count;        // 原 2376  Count: TStrList_Count
    public TStrList_GetText GetText;        // 原 2377  GetText: TStrList_GetText
    public TStrList_SetText SetText;        // 原 2378  SetText: TStrList_SetText
    public TStrList_Add Add;        // 原 2379  Add: TStrList_Add
    public TStrList_AddObject AddObject;        // 原 2380  AddObject: TStrList_AddObject
    public TStrList_Insert Insert;        // 原 2381  Insert: TStrList_Insert
    public TStrList_InsertObject InsertObject;        // 原 2382  InsertObject: TStrList_InsertObject
    public TStrList_Remove Remove;        // 原 2383  Remove: TStrList_Remove
    public TStrList_Delete Delete;        // 原 2384  Delete: TStrList_Delete
    public TStrList_GetItem GetItem;        // 原 2385  GetItem: TStrList_GetItem
    public TStrList_SetItem SetItem;        // 原 2386  SetItem: TStrList_SetItem
    public TStrList_GetObject GetObject;        // 原 2387  GetObject: TStrList_GetObject
    public TStrList_SetObject SetObject;        // 原 2388  SetObject: TStrList_SetObject
    public TStrList_IndexOf IndexOf;        // 原 2389  IndexOf: TStrList_IndexOf
    public TStrList_IndexOfObject IndexOfObject;        // 原 2390  IndexOfObject: TStrList_IndexOfObject
    public TStrList_Find Find;        // 原 2391  Find: TStrList_Find
    public TStrList_Exchange Exchange;        // 原 2392  Exchange: TStrList_Exchange
    public TStrLit_LoadFromFile LoadFromFile;        // 原 2393  LoadFromFile: TStrLit_LoadFromFile
    public TStrLit_SaveToFile SaveToFile;        // 原 2394  SaveToFile: TStrLit_SaveToFile
    public TStrList_CopyTo CopyTo;        // 原 2395  CopyTo: TStrList_CopyTo
    public fixed long Reserved[20];        // 原 2396  Reserved: array of 19 x20
}

/// <summary>原文 TMemoryStreamFunc（PluginInterface.pas:2399-2414,14 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMemoryStreamFunc
{
    public TMemStream_Create Create;        // 原 2400  Create: TMemStream_Create
    public TMemStream_Free Free;        // 原 2401  Free: TMemStream_Free
    public TMemStream_GetSize GetSize;        // 原 2402  GetSize: TMemStream_GetSize
    public TMemStream_SetSize SetSize;        // 原 2403  SetSize: TMemStream_SetSize
    public TMemStream_Clear Clear;        // 原 2404  Clear: TMemStream_Clear
    public TMemStream_Read Read;        // 原 2405  Read: TMemStream_Read
    public TMemStream_Write Write;        // 原 2406  Write: TMemStream_Write
    public TMemStream_Seek Seek;        // 原 2407  Seek: TMemStream_Seek
    public TMemStream_Memory Memory;        // 原 2408  Memory: TMemStream_Memory
    public TMemStream_GetPosition GetPosition;        // 原 2409  GetPosition: TMemStream_GetPosition
    public TMemStream_SetPosition SetPosition;        // 原 2410  SetPosition: TMemStream_SetPosition
    public TMemStream_LoadFromFile LoadFromFile;        // 原 2411  LoadFromFile: TMemStream_LoadFromFile
    public TMemStream_SaveToFile SaveToFile;        // 原 2412  SaveToFile: TMemStream_SaveToFile
    public fixed long Reserved[20];        // 原 2413  Reserved: array of 19 x20
}

/// <summary>原文 TMemuFunc（PluginInterface.pas:2416-2444,27 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMemuFunc
{
    public TMenu_GetMainMenu GetMainMenu;        // 原 2417  GetMainMenu: TMenu_GetMainMenu
    public TMenu_GetControlMenu GetControlMenu;        // 原 2418  GetControlMenu: TMenu_GetControlMenu
    public TMenu_GetViewMenu GetViewMenu;        // 原 2419  GetViewMenu: TMenu_GetViewMenu
    public TMenu_GetOptionMenu GetOptionMenu;        // 原 2420  GetOptionMenu: TMenu_GetOptionMenu
    public TMenu_GetManagerMenu GetManagerMenu;        // 原 2421  GetManagerMenu: TMenu_GetManagerMenu
    public TMenu_GetToolsMenu GetToolsMenu;        // 原 2422  GetToolsMenu: TMenu_GetToolsMenu
    public TMenu_GetHelpMenu GetHelpMenu;        // 原 2423  GetHelpMenu: TMenu_GetHelpMenu
    public TMenu_GetPluginMenu GetPluginMenu;        // 原 2424  GetPluginMenu: TMenu_GetPluginMenu
    public TMenu_Count Count;        // 原 2425  Count: TMenu_Count
    public TMenu_GetItems GetItems;        // 原 2426  GetItems: TMenu_GetItems
    public TMenu_Add Add;        // 原 2427  Add: TMenu_Add
    public TMenu_Insert Insert;        // 原 2428  Insert: TMenu_Insert
    public TMenu_GetCaption GetCaption;        // 原 2429  GetCaption: TMenu_GetCaption
    public TMenu_SetCaption SetCaption;        // 原 2430  SetCaption: TMenu_SetCaption
    public TMenu_GetEnabled GetEnabled;        // 原 2431  GetEnabled: TMenu_GetEnabled
    public TMenu_SetEnabled SetEnabled;        // 原 2432  SetEnabled: TMenu_SetEnabled
    public TMenu_GetVisable GetVisable;        // 原 2433  GetVisable: TMenu_GetVisable
    public TMenu_SetVisable SetVisable;        // 原 2434  SetVisable: TMenu_SetVisable
    public TMenu_GetChecked GetChecked;        // 原 2435  GetChecked: TMenu_GetChecked
    public TMenu_SetChecked SetChecked;        // 原 2436  SetChecked: TMenu_SetChecked
    public TMenu_GetRadioItem GetRadioItem;        // 原 2437  GetRadioItem: TMenu_GetRadioItem
    public TMenu_SetRadioItem SetRadioItem;        // 原 2438  SetRadioItem: TMenu_SetRadioItem
    public TMenu_GetGroupIndex GetGroupIndex;        // 原 2439  GetGroupIndex: TMenu_GetGroupIndex
    public TMenu_SetGroupIndex SetGroupIndex;        // 原 2440  SetGroupIndex: TMenu_SetGroupIndex
    public TMenu_GetTag GetTag;        // 原 2441  GetTag: TMenu_GetTag
    public TMenu_SetTag SetTag;        // 原 2442  SetTag: TMenu_SetTag
    public fixed long Reserved[20];        // 原 2443  Reserved: array of 19 x20
}

/// <summary>原文 TIniFileFunc（PluginInterface.pas:2446-2458,11 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TIniFileFunc
{
    public TIniFile_Create Create;        // 原 2447  Create: TIniFile_Create
    public TIniFile_Free Free;        // 原 2448  Free: TIniFile_Free
    public TIniFile_SectionExists SectionExists;        // 原 2449  SectionExists: TIniFile_SectionExists
    public TIniFile_ValueExists ValueExists;        // 原 2450  ValueExists: TIniFile_ValueExists
    public TIniFile_ReadString ReadString;        // 原 2451  ReadString: TIniFile_ReadString
    public TIniFile_WriteString WriteString;        // 原 2452  WriteString: TIniFile_WriteString
    public TIniFile_ReadInteger ReadInteger;        // 原 2453  ReadInteger: TIniFile_ReadInteger
    public TIniFile_WriteInteger WriteInteger;        // 原 2454  WriteInteger: TIniFile_WriteInteger
    public TIniFile_ReadBool ReadBool;        // 原 2455  ReadBool: TIniFile_ReadBool
    public TIniFile_WriteBool WriteBool;        // 原 2456  WriteBool: TIniFile_WriteBool
    public fixed long Reserved[30];        // 原 2457  Reserved: array of 29 x30
}

/// <summary>原文 TMagicACListFunc（PluginInterface.pas:2460-2465,4 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMagicACListFunc
{
    public TMagicACList_Count Count;        // 原 2461  Count: TMagicACList_Count
    public TMagicACList_GetItem GetItem;        // 原 2462  GetItem: TMagicACList_GetItem
    public TMagicACList_FindByMagIdx FindByMagIdx;        // 原 2463  FindByMagIdx: TMagicACList_FindByMagIdx
    public fixed long Reserved[10];        // 原 2464  Reserved: array of 9 x10
}

/// <summary>原文 TMapManagerFunc（PluginInterface.pas:2467-2471,3 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMapManagerFunc
{
    public TMapManager_FindMap FindMap;        // 原 2468  FindMap: TMapManager_FindMap
    public TMapManager_GetMapList GetMapList;        // 原 2469  GetMapList: TMapManager_GetMapList
    public fixed long Reserved[40];        // 原 2470  Reserved: array of 39 x40
}

/// <summary>原文 TEnvirnomentFunc（PluginInterface.pas:2473-2501,26 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TEnvirnomentFunc
{
    public TEnvir_GetMapName GetMapName;        // 原 2474  GetMapName: TEnvir_GetMapName
    public TEnvir_GetMapDesc GetMapDesc;        // 原 2475  GetMapDesc: TEnvir_GetMapDesc
    public TEnvir_GetWidth GetWidth;        // 原 2476  GetWidth: TEnvir_GetWidth
    public TEnvir_GetHeight GetHeight;        // 原 2477  GetHeight: TEnvir_GetHeight
    public TEnvir_GetMinMap GetMinMap;        // 原 2478  GetMinMap: TEnvir_GetMinMap
    public TEnvir_IsMainMap IsMainMap;        // 原 2479  IsMainMap: TEnvir_IsMainMap
    public TEnvir_GetMainMapName GetMainMapName;        // 原 2480  GetMainMapName: TEnvir_GetMainMapName
    public TEnvir_IsMirrMap IsMirrMap;        // 原 2481  IsMirrMap: TEnvir_IsMirrMap
    public TEnvir_GetMirrMapCreateTick GetMirrMapCreateTick;        // 原 2482  GetMirrMapCreateTick: TEnvir_GetMirrMapCreateTick
    public TEnvir_GetMirrMapSurvivalTime GetMirrMapSurvivalTime;        // 原 2483  GetMirrMapSurvivalTime: TEnvir_GetMirrMapSurvivalTime
    public TEnvir_GetMirrMapExitToMap GetMirrMapExitToMap;        // 原 2484  GetMirrMapExitToMap: TEnvir_GetMirrMapExitToMap
    public TEnvir_GetMirrMapMinMap GetMirrMapMinMap;        // 原 2485  GetMirrMapMinMap: TEnvir_GetMirrMapMinMap
    public TEnvir_GetAlwaysShowTime GetAlwaysShowTime;        // 原 2486  GetAlwaysShowTime: TEnvir_GetAlwaysShowTime
    public TEnvir_IsFBMap IsFBMap;        // 原 2487  IsFBMap: TEnvir_IsFBMap
    public TEnvir_GetFBMapName GetFBMapName;        // 原 2488  GetFBMapName: TEnvir_GetFBMapName
    public TEnvir_GetFBEnterLimit GetFBEnterLimit;        // 原 2489  GetFBEnterLimit: TEnvir_GetFBEnterLimit
    public TEnvir_GetFBCreated GetFBCreated;        // 原 2491  GetFBCreated: TEnvir_GetFBCreated
    public TEnvir_GetFBCreateTime GetFBCreateTime;        // 原 2492  GetFBCreateTime: TEnvir_GetFBCreateTime
    public TEnvir_GetMapParam GetMapParam;        // 原 2493  GetMapParam: TEnvir_GetMapParam
    public TEnvir_GetMapParamValue GetMapParamValue;        // 原 2494  GetMapParamValue: TEnvir_GetMapParamValue
    public TEnvir_CheckCanMove CheckCanMove;        // 原 2495  CheckCanMove: TEnvir_CheckCanMove
    public TEnvir_IsValidObject IsValidObject;        // 原 2496  IsValidObject: TEnvir_IsValidObject
    public TEnvir_GetItemObjects GetItemObjects;        // 原 2497  GetItemObjects: TEnvir_GetItemObjects
    public TEnvir_GetBaseObjects GetBaseObjects;        // 原 2498  GetBaseObjects: TEnvir_GetBaseObjects
    public TEnvir_GetPlayObjects GetPlayObjects;        // 原 2499  GetPlayObjects: TEnvir_GetPlayObjects
    public fixed long Reserved[100];        // 原 2500  Reserved: array of 99 x100
}

/// <summary>原文 TM2EngineFunc（PluginInterface.pas:2503-2531,27 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TM2EngineFunc
{
    public TM2Engine_GetVersion GetVersion;        // 原 2504  GetVersion: TM2Engine_GetVersion
    public TM2Engine_GetVersionInt GetVersionInt;        // 原 2505  GetVersionInt: TM2Engine_GetVersionInt
    public TM2Engine_GetMainFormHandle GetMainFormHandle;        // 原 2506  GetMainFormHandle: TM2Engine_GetMainFormHandle
    public TM2Engine_SetMainFormCaption SetMainFormCaption;        // 原 2507  SetMainFormCaption: TM2Engine_SetMainFormCaption
    public TM2Engine_GetAppDir GetAppDir;        // 原 2508  GetAppDir: TM2Engine_GetAppDir
    public TM2Engine_GetGlobalIniFile GetGlobalIniFile;        // 原 2509  GetGlobalIniFile: TM2Engine_GetGlobalIniFile
    public TM2Engine_GetOtherFileDir GetOtherFileDir;        // 原 2510  GetOtherFileDir: TM2Engine_GetOtherFileDir
    public TM2Engine_MainOutMessage MainOutMessage;        // 原 2511  MainOutMessage: TM2Engine_MainOutMessage
    public TM2Engine_GetGlobalVarI GetGlobalVarI;        // 原 2512  GetGlobalVarI: TM2Engine_GetGlobalVarI
    public TM2Engine_SetGlobalVarI SetGlobalVarI;        // 原 2513  SetGlobalVarI: TM2Engine_SetGlobalVarI
    public TM2Engine_GetGlobalVarG GetGlobalVarG;        // 原 2514  GetGlobalVarG: TM2Engine_GetGlobalVarG
    public TM2Engine_SetGlobalVarG SetGlobalVarG;        // 原 2515  SetGlobalVarG: TM2Engine_SetGlobalVarG
    public TM2Engine_GetGlobalVarA GetGlobalVarA;        // 原 2516  GetGlobalVarA: TM2Engine_GetGlobalVarA
    public TM2Engine_SetGlobalVarA SetGlobalVarA;        // 原 2517  SetGlobalVarA: TM2Engine_SetGlobalVarA
    public TM2Engine_EncodeBuffer EncodeBuffer;        // 原 2518  EncodeBuffer: TM2Engine_EncodeBuffer
    public TM2Engine_DecodeBuffer DecodeBuffer;        // 原 2519  DecodeBuffer: TM2Engine_DecodeBuffer
    public TM2Engine_ZLibEncodeBuffer ZLibEncodeBuffer;        // 原 2520  ZLibEncodeBuffer: TM2Engine_ZLibEncodeBuffer
    public TM2Engine_ZLibDecodeBuffer ZLibDecodeBuffer;        // 原 2521  ZLibDecodeBuffer: TM2Engine_ZLibDecodeBuffer
    public TM2Engine_EncryptBuffer EncryptBuffer;        // 原 2522  EncryptBuffer: TM2Engine_EncryptBuffer
    public TM2Engine_DecryptBuffer DecryptBuffer;        // 原 2523  DecryptBuffer: TM2Engine_DecryptBuffer
    public TM2Engine_EncryptPassword EncryptPassword;        // 原 2524  EncryptPassword: TM2Engine_EncryptPassword
    public TM2Engine_DecryptPassword DecryptPassword;        // 原 2525  DecryptPassword: TM2Engine_DecryptPassword
    public TM2Engine_GetTakeOnPosition GetTakeOnPosition;        // 原 2526  GetTakeOnPosition: TM2Engine_GetTakeOnPosition
    public TM2Engine_CheckBindType CheckBindType;        // 原 2527  CheckBindType: TM2Engine_CheckBindType
    public TM2Engine_SetBindValue SetBindValue;        // 原 2528  SetBindValue: TM2Engine_SetBindValue
    public TM2Engine_GetRGB GetRGB;        // 原 2529  GetRGB: TM2Engine_GetRGB
    public fixed long Reserved[100];        // 原 2530  Reserved: array of 99 x100
}

/// <summary>原文 TBaseObjectFunc（PluginInterface.pas:2533-2721,186 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TBaseObjectFunc
{
    public TBaseObject_GetChrName GetChrName;        // 原 2534  GetChrName: TBaseObject_GetChrName
    public TBaseObject_SetChrName SetChrName;        // 原 2535  SetChrName: TBaseObject_SetChrName
    public TBaseObject_RefShowName RefShowName;        // 原 2536  RefShowName: TBaseObject_RefShowName
    public TBaseObject_RefNameColor RefNameColor;        // 原 2537  RefNameColor: TBaseObject_RefNameColor
    public TBaseObject_GetGender GetGender;        // 原 2538  GetGender: TBaseObject_GetGender
    public TBaseObject_SetGender SetGender;        // 原 2539  SetGender: TBaseObject_SetGender
    public TBaseObject_GetJob GetJob;        // 原 2540  GetJob: TBaseObject_GetJob
    public TBaseObject_SetJob SetJob;        // 原 2541  SetJob: TBaseObject_SetJob
    public TBaseObject_GetHair GetHair;        // 原 2542  GetHair: TBaseObject_GetHair
    public TBaseObject_SetHair SetHair;        // 原 2543  SetHair: TBaseObject_SetHair
    public TBaseObject_GetEnvir GetEnvir;        // 原 2544  GetEnvir: TBaseObject_GetEnvir
    public TBaseObject_GetMapName GetMapName;        // 原 2545  GetMapName: TBaseObject_GetMapName
    public TBaseObject_GetCurrX GetCurrX;        // 原 2546  GetCurrX: TBaseObject_GetCurrX
    public TBaseObject_GetCurrY GetCurrY;        // 原 2547  GetCurrY: TBaseObject_GetCurrY
    public TBaseObject_GetDirection GetDirection;        // 原 2548  GetDirection: TBaseObject_GetDirection
    public TBaseObject_GetHomeMap GetHomeMap;        // 原 2549  GetHomeMap: TBaseObject_GetHomeMap
    public TBaseObject_GetHomeX GetHomeX;        // 原 2550  GetHomeX: TBaseObject_GetHomeX
    public TBaseObject_GetHomeY GetHomeY;        // 原 2551  GetHomeY: TBaseObject_GetHomeY
    public TBaseObject_GetPermission GetPermission;        // 原 2552  GetPermission: TBaseObject_GetPermission
    public TBaseObject_SetPermission SetPermission;        // 原 2553  SetPermission: TBaseObject_SetPermission
    public TBaseObject_GetDeath GetDeath;        // 原 2554  GetDeath: TBaseObject_GetDeath
    public TBaseObject_GetDeathTick GetDeathTick;        // 原 2555  GetDeathTick: TBaseObject_GetDeathTick
    public TBaseObject_GetGhost GetGhost;        // 原 2556  GetGhost: TBaseObject_GetGhost
    public TBaseObject_GetGhostTick GetGhostTick;        // 原 2557  GetGhostTick: TBaseObject_GetGhostTick
    public TBaseObject_MakeGhost MakeGhost;        // 原 2558  MakeGhost: TBaseObject_MakeGhost
    public TBaseObject_ReAlive ReAlive;        // 原 2559  ReAlive: TBaseObject_ReAlive
    public TBaseObject_GetRaceServer GetRaceServer;        // 原 2560  GetRaceServer: TBaseObject_GetRaceServer
    public TBaseObject_GetAppr GetAppr;        // 原 2561  GetAppr: TBaseObject_GetAppr
    public TBaseObject_GetRaceImg GetRaceImg;        // 原 2562  GetRaceImg: TBaseObject_GetRaceImg
    public TBaseObject_GetCharStatus GetCharStatus;        // 原 2563  GetCharStatus: TBaseObject_GetCharStatus
    public TBaseObject_SetCharStatus SetCharStatus;        // 原 2564  SetCharStatus: TBaseObject_SetCharStatus
    public TBaseObject_StatusChanged StatusChanged;        // 原 2565  StatusChanged: TBaseObject_StatusChanged
    public TBaseObject_GetHungerPoint GetHungerPoint;        // 原 2566  GetHungerPoint: TBaseObject_GetHungerPoint
    public TBaseObject_SetHungerPoint SetHungerPoint;        // 原 2567  SetHungerPoint: TBaseObject_SetHungerPoint
    public TBaseobject_IsNGMonster IsNGMonster;        // 原 2568  IsNGMonster: TBaseobject_IsNGMonster
    public TBaseObject_IsDummyObject IsDummyObject;        // 原 2569  IsDummyObject: TBaseObject_IsDummyObject
    public TBaseObject_GetViewRange GetViewRange;        // 原 2570  GetViewRange: TBaseObject_GetViewRange
    public TBaseObject_SetViewRange SetViewRange;        // 原 2571  SetViewRange: TBaseObject_SetViewRange
    public TBaseObject_GetAbility GetAbility;        // 原 2572  GetAbility: TBaseObject_GetAbility
    public TBaseObject_GetWAbility GetWAbility;        // 原 2573  GetWAbility: TBaseObject_GetWAbility
    public TBaseObject_SetWAbility SetWAbility;        // 原 2574  SetWAbility: TBaseObject_SetWAbility
    public TBaseObject_GetSlaveList GetSlaveList;        // 原 2575  GetSlaveList: TBaseObject_GetSlaveList
    public TBaseObject_GetMaster GetMaster;        // 原 2576  GetMaster: TBaseObject_GetMaster
    public TBaseObject_GetMasterEx GetMasterEx;        // 原 2577  GetMasterEx: TBaseObject_GetMasterEx
    public TBaseObject_GetSuperManMode GetSuperManMode;        // 原 2578  GetSuperManMode: TBaseObject_GetSuperManMode
    public TBaseObject_SetSuperManMode SetSuperManMode;        // 原 2579  SetSuperManMode: TBaseObject_SetSuperManMode
    public TBaseObject_GetAdminMode GetAdminMode;        // 原 2580  GetAdminMode: TBaseObject_GetAdminMode
    public TBaseObject_SetAdminMode SetAdminMode;        // 原 2581  SetAdminMode: TBaseObject_SetAdminMode
    public TBaseObject_GetTransparent GetTransparent;        // 原 2582  GetTransparent: TBaseObject_GetTransparent
    public TBaseObject_SetTransparent SetTransparent;        // 原 2583  SetTransparent: TBaseObject_SetTransparent
    public TBaseObject_GetObMode GetObMode;        // 原 2584  GetObMode: TBaseObject_GetObMode
    public TBaseObject_SetObMode SetObMode;        // 原 2585  SetObMode: TBaseObject_SetObMode
    public TBaseObject_GetStoneMode GetStoneMode;        // 原 2586  GetStoneMode: TBaseObject_GetStoneMode
    public TBaseObject_SetStoneMode SetStoneMode;        // 原 2587  SetStoneMode: TBaseObject_SetStoneMode
    public TBaseObject_GetStickMode GetStickMode;        // 原 2588  GetStickMode: TBaseObject_GetStickMode
    public TBaseObject_SetStickMode SetStickMode;        // 原 2589  SetStickMode: TBaseObject_SetStickMode
    public TBaseObject_GetIsAnimal GetIsAnimal;        // 原 2590  GetIsAnimal: TBaseObject_GetIsAnimal
    public TBaseObject_SetIsAnimal SetIsAnimal;        // 原 2591  SetIsAnimal: TBaseObject_SetIsAnimal
    public TBaseObject_GetIsNoItem GetIsNoItem;        // 原 2592  GetIsNoItem: TBaseObject_GetIsNoItem
    public TBaseObject_SetIsNoItem SetIsNoItem;        // 原 2593  SetIsNoItem: TBaseObject_SetIsNoItem
    public TBaseObject_GetCoolEye GetCoolEye;        // 原 2594  GetCoolEye: TBaseObject_GetCoolEye
    public TBaseObject_SetCoolEye SetCoolEye;        // 原 2595  SetCoolEye: TBaseObject_SetCoolEye
    public TBaseObject_GetHitPoint GetHitPoint;        // 原 2596  GetHitPoint: TBaseObject_GetHitPoint
    public TBaseObject_SetHitPoint SetHitPoint;        // 原 2597  SetHitPoint: TBaseObject_SetHitPoint
    public TBaseObject_GetSpeedPoint GetSpeedPoint;        // 原 2598  GetSpeedPoint: TBaseObject_GetSpeedPoint
    public TBaseObject_SetSpeedPoint SetSpeedPoint;        // 原 2599  SetSpeedPoint: TBaseObject_SetSpeedPoint
    public TBaseObject_GetHitSpeed GetHitSpeed;        // 原 2600  GetHitSpeed: TBaseObject_GetHitSpeed
    public TBaseObject_SetHitSpeed SetHitSpeed;        // 原 2601  SetHitSpeed: TBaseObject_SetHitSpeed
    public TBaseObject_GetWalkSpeed GetWalkSpeed;        // 原 2602  GetWalkSpeed: TBaseObject_GetWalkSpeed
    public TBaseObject_SetWalkSpeed SetWalkSpeed;        // 原 2603  SetWalkSpeed: TBaseObject_SetWalkSpeed
    public TBaseObject_GetHPRecover GetHPRecover;        // 原 2604  GetHPRecover: TBaseObject_GetHPRecover
    public TBaseObject_SetHPRecover SetHPRecover;        // 原 2605  SetHPRecover: TBaseObject_SetHPRecover
    public TBaseObject_GetMPRecover GetMPRecover;        // 原 2606  GetMPRecover: TBaseObject_GetMPRecover
    public TBaseObject_SetMPRecover SetMPRecover;        // 原 2607  SetMPRecover: TBaseObject_SetMPRecover
    public TBaseObject_GetPoisonRecover GetPoisonRecover;        // 原 2608  GetPoisonRecover: TBaseObject_GetPoisonRecover
    public TBaseObject_SetPoisonRecover SetPoisonRecover;        // 原 2609  SetPoisonRecover: TBaseObject_SetPoisonRecover
    public TBaseObject_GetAntiPoison GetAntiPoison;        // 原 2610  GetAntiPoison: TBaseObject_GetAntiPoison
    public TBaseObject_SetAntiPoison SetAntiPoison;        // 原 2611  SetAntiPoison: TBaseObject_SetAntiPoison
    public TBaseObject_GetAntiMagic GetAntiMagic;        // 原 2612  GetAntiMagic: TBaseObject_GetAntiMagic
    public TBaseObject_SetAntiMagic SetAntiMagic;        // 原 2613  SetAntiMagic: TBaseObject_SetAntiMagic
    public TBaseObject_GetLuck GetLuck;        // 原 2614  GetLuck: TBaseObject_GetLuck
    public TBaseObject_SetLuck SetLuck;        // 原 2615  SetLuck: TBaseObject_SetLuck
    public TBaseObject_GetAttatckMode GetAttatckMode;        // 原 2616  GetAttatckMode: TBaseObject_GetAttatckMode
    public TBaseObject_SetAttatckMode SetAttatckMode;        // 原 2617  SetAttatckMode: TBaseObject_SetAttatckMode
    public TBaseObject_GetNation GetNation;        // 原 2618  GetNation: TBaseObject_GetNation
    public TBaseObject_SetNation SetNation;        // 原 2619  SetNation: TBaseObject_SetNation
    public TBaseObject_GetNationaName GetNationaName;        // 原 2620  GetNationaName: TBaseObject_GetNationaName
    public TBaseObject_GetGuild GetGuild;        // 原 2621  GetGuild: TBaseObject_GetGuild
    public TBaseobject_GetGuildRankNo GetGuildRankNo;        // 原 2622  GetGuildRankNo: TBaseobject_GetGuildRankNo
    public TBaseobject_GetGuildRankName GetGuildRankName;        // 原 2623  GetGuildRankName: TBaseobject_GetGuildRankName
    public TBaseObject_IsGuildMaster IsGuildMaster;        // 原 2624  IsGuildMaster: TBaseObject_IsGuildMaster
    public TBaseObject_GetHideMode GetHideMode;        // 原 2625  GetHideMode: TBaseObject_GetHideMode
    public TBaseObject_SetHideMode SetHideMode;        // 原 2626  SetHideMode: TBaseObject_SetHideMode
    public TBaseObject_GetIsParalysis GetIsParalysis;        // 原 2627  GetIsParalysis: TBaseObject_GetIsParalysis
    public TBaseObject_SetIsParalysis SetIsParalysis;        // 原 2628  SetIsParalysis: TBaseObject_SetIsParalysis
    public TBaseObject_GetParalysisRate GetParalysisRate;        // 原 2629  GetParalysisRate: TBaseObject_GetParalysisRate
    public TBaseObject_SetParalysisRate SetParalysisRate;        // 原 2630  SetParalysisRate: TBaseObject_SetParalysisRate
    public TBaseObject_GetIsMDParalysis GetIsMDParalysis;        // 原 2631  GetIsMDParalysis: TBaseObject_GetIsMDParalysis
    public TBaseObject_SetIsMDParalysis SetIsMDParalysis;        // 原 2632  SetIsMDParalysis: TBaseObject_SetIsMDParalysis
    public TBaseObject_GetMDParalysisRate GetMDParalysisRate;        // 原 2633  GetMDParalysisRate: TBaseObject_GetMDParalysisRate
    public TBaseObject_SetMDParalysisRate SetMDParalysisRate;        // 原 2634  SetMDParalysisRate: TBaseObject_SetMDParalysisRate
    public TBaseObject_GetIsFrozen GetIsFrozen;        // 原 2635  GetIsFrozen: TBaseObject_GetIsFrozen
    public TBaseObject_SetIsFrozen SetIsFrozen;        // 原 2636  SetIsFrozen: TBaseObject_SetIsFrozen
    public TBaseObject_GetFrozenRate GetFrozenRate;        // 原 2637  GetFrozenRate: TBaseObject_GetFrozenRate
    public TBaseObject_SetFrozenRate SetFrozenRate;        // 原 2638  SetFrozenRate: TBaseObject_SetFrozenRate
    public TBaseObject_GetIsCobwebWinding GetIsCobwebWinding;        // 原 2639  GetIsCobwebWinding: TBaseObject_GetIsCobwebWinding
    public TBaseObject_SetIsCobwebWinding SetIsCobwebWinding;        // 原 2640  SetIsCobwebWinding: TBaseObject_SetIsCobwebWinding
    public TBaseObject_GetCobwebWindingRate GetCobwebWindingRate;        // 原 2641  GetCobwebWindingRate: TBaseObject_GetCobwebWindingRate
    public TBaseObject_SetCobwebWindingRate SetCobwebWindingRate;        // 原 2642  SetCobwebWindingRate: TBaseObject_SetCobwebWindingRate
    public TBaseObject_GetUnParalysisValue GetUnParalysisValue;        // 原 2643  GetUnParalysisValue: TBaseObject_GetUnParalysisValue
    public TBaseObject_SetUnParalysisValue SetUnParalysisValue;        // 原 2644  SetUnParalysisValue: TBaseObject_SetUnParalysisValue
    public TBaseObject_GetIsUnParalysis GetIsUnParalysis;        // 原 2645  GetIsUnParalysis: TBaseObject_GetIsUnParalysis
    public TBaseObject_GetUnMagicShieldValue GetUnMagicShieldValue;        // 原 2646  GetUnMagicShieldValue: TBaseObject_GetUnMagicShieldValue
    public TBaseObject_SetUnMagicShieldValue SetUnMagicShieldValue;        // 原 2647  SetUnMagicShieldValue: TBaseObject_SetUnMagicShieldValue
    public TBaseObject_GetIsUnMagicShield GetIsUnMagicShield;        // 原 2648  GetIsUnMagicShield: TBaseObject_GetIsUnMagicShield
    public TBaseObject_GetUnRevivalValue GetUnRevivalValue;        // 原 2649  GetUnRevivalValue: TBaseObject_GetUnRevivalValue
    public TBaseObject_SetUnRevivalValue SetUnRevivalValue;        // 原 2650  SetUnRevivalValue: TBaseObject_SetUnRevivalValue
    public TBaseObject_GetIsUnRevival GetIsUnRevival;        // 原 2651  GetIsUnRevival: TBaseObject_GetIsUnRevival
    public TBaseObject_GetUnPosionValue GetUnPosionValue;        // 原 2652  GetUnPosionValue: TBaseObject_GetUnPosionValue
    public TBaseObject_SetUnPosionValue SetUnPosionValue;        // 原 2653  SetUnPosionValue: TBaseObject_SetUnPosionValue
    public TBaseObject_GetIsUnPosion GetIsUnPosion;        // 原 2654  GetIsUnPosion: TBaseObject_GetIsUnPosion
    public TBaseObject_GetUnTammingValue GetUnTammingValue;        // 原 2655  GetUnTammingValue: TBaseObject_GetUnTammingValue
    public TBaseObject_SetUnTammingValue SetUnTammingValue;        // 原 2656  SetUnTammingValue: TBaseObject_SetUnTammingValue
    public TBaseObject_GetIsUnTamming GetIsUnTamming;        // 原 2657  GetIsUnTamming: TBaseObject_GetIsUnTamming
    public TBaseObject_GetUnFireCrossValue GetUnFireCrossValue;        // 原 2658  GetUnFireCrossValue: TBaseObject_GetUnFireCrossValue
    public TBaseObject_SetUnFireCrossValue SetUnFireCrossValue;        // 原 2659  SetUnFireCrossValue: TBaseObject_SetUnFireCrossValue
    public TBaseObject_GetIsUnFireCross GetIsUnFireCross;        // 原 2660  GetIsUnFireCross: TBaseObject_GetIsUnFireCross
    public TBaseObject_GetUnFrozenValue GetUnFrozenValue;        // 原 2661  GetUnFrozenValue: TBaseObject_GetUnFrozenValue
    public TBaseObject_SetUnFrozenValue SetUnFrozenValue;        // 原 2662  SetUnFrozenValue: TBaseObject_SetUnFrozenValue
    public TBaseObject_GetIsUnFrozen GetIsUnFrozen;        // 原 2663  GetIsUnFrozen: TBaseObject_GetIsUnFrozen
    public TBaseObject_GetUnCobwebWindingValue GetUnCobwebWindingValue;        // 原 2664  GetUnCobwebWindingValue: TBaseObject_GetUnCobwebWindingValue
    public TBaseObject_SetUnCobwebWindingValue SetUnCobwebWindingValue;        // 原 2665  SetUnCobwebWindingValue: TBaseObject_SetUnCobwebWindingValue
    public TBaseObject_GetIsUnCobwebWinding GetIsUnCobwebWinding;        // 原 2666  GetIsUnCobwebWinding: TBaseObject_GetIsUnCobwebWinding
    public TBaseObject_GetTargetCret GetTargetCret;        // 原 2667  GetTargetCret: TBaseObject_GetTargetCret
    public TBaseObject_SetTargetCret SetTargetCret;        // 原 2668  SetTargetCret: TBaseObject_SetTargetCret
    public TBaseObject_DelTargetCreat DelTargetCreat;        // 原 2669  DelTargetCreat: TBaseObject_DelTargetCreat
    public TBaseObject_GetLastHiter GetLastHiter;        // 原 2670  GetLastHiter: TBaseObject_GetLastHiter
    public TBaseObject_GetExpHitter GetExpHitter;        // 原 2671  GetExpHitter: TBaseObject_GetExpHitter
    public TBaseObject_GetPoisonHitter GetPoisonHitter;        // 原 2672  GetPoisonHitter: TBaseObject_GetPoisonHitter
    public TBaseObject_GetPoseCreate GetPoseCreate;        // 原 2673  GetPoseCreate: TBaseObject_GetPoseCreate
    public TBaseObject_IsProperTarget IsProperTarget;        // 原 2674  IsProperTarget: TBaseObject_IsProperTarget
    public TBaseObject_IsProperFriend IsProperFriend;        // 原 2675  IsProperFriend: TBaseObject_IsProperFriend
    public TBaseObject_TargetInRange TargetInRange;        // 原 2676  TargetInRange: TBaseObject_TargetInRange
    public TBaseObject_SendMsg SendMsg;        // 原 2677  SendMsg: TBaseObject_SendMsg
    public TBaseObject_SendDelayMsg SendDelayMsg;        // 原 2678  SendDelayMsg: TBaseObject_SendDelayMsg
    public TBaseObject_SendRefMsg SendRefMsg;        // 原 2679  SendRefMsg: TBaseObject_SendRefMsg
    public TBaseObject_SendUpdateMsg SendUpdateMsg;        // 原 2680  SendUpdateMsg: TBaseObject_SendUpdateMsg
    public TBaseObject_SysMsg SysMsg;        // 原 2681  SysMsg: TBaseObject_SysMsg
    public TBaseObject_GetBagItemList GetBagItemList;        // 原 2682  GetBagItemList: TBaseObject_GetBagItemList
    public TBaseObject_IsEnoughBag IsEnoughBag;        // 原 2683  IsEnoughBag: TBaseObject_IsEnoughBag
    public TBaseObject_IsEnoughBagEx IsEnoughBagEx;        // 原 2684  IsEnoughBagEx: TBaseObject_IsEnoughBagEx
    public TBaseObject_AddItemToBag AddItemToBag;        // 原 2685  AddItemToBag: TBaseObject_AddItemToBag
    public TBaseObject_DelBagItemByIndex DelBagItemByIndex;        // 原 2686  DelBagItemByIndex: TBaseObject_DelBagItemByIndex
    public TBaseObject_DelBagItemByMakeIdx DelBagItemByMakeIdx;        // 原 2687  DelBagItemByMakeIdx: TBaseObject_DelBagItemByMakeIdx
    public TBaseObject_DelBagItemByUserItem DelBagItemByUserItem;        // 原 2688  DelBagItemByUserItem: TBaseObject_DelBagItemByUserItem
    public TBaseObject_IsInSafeZone IsInSafeZone;        // 原 2689  IsInSafeZone: TBaseObject_IsInSafeZone
    public TBaseObject_IsPtInSafeZone IsPtInSafeZone;        // 原 2690  IsPtInSafeZone: TBaseObject_IsPtInSafeZone
    public TBaseObject_RecalcLevelAbil RecalcLevelAbil;        // 原 2691  RecalcLevelAbil: TBaseObject_RecalcLevelAbil
    public TBaseObject_RecalcAbil RecalcAbil;        // 原 2693  RecalcAbil: TBaseObject_RecalcAbil
    public TBaseObject_RecalcBagWeight RecalcBagWeight;        // 原 2694  RecalcBagWeight: TBaseObject_RecalcBagWeight
    public TBaseObject_GetLevelExp GetLevelExp;        // 原 2695  GetLevelExp: TBaseObject_GetLevelExp
    public TBaseObject_HasLevelUp HasLevelUp;        // 原 2696  HasLevelUp: TBaseObject_HasLevelUp
    public TBaseObject_TrainSkill TrainSkill;        // 原 2697  TrainSkill: TBaseObject_TrainSkill
    public TBaseObject_CheckMagicLevelup CheckMagicLevelup;        // 原 2698  CheckMagicLevelup: TBaseObject_CheckMagicLevelup
    public TBaseObject_MagicTranPointChanged MagicTranPointChanged;        // 原 2699  MagicTranPointChanged: TBaseObject_MagicTranPointChanged
    public TBaseObject_DamageHealth DamageHealth;        // 原 2700  DamageHealth: TBaseObject_DamageHealth
    public TBaseObject_DamageSpell DamageSpell;        // 原 2701  DamageSpell: TBaseObject_DamageSpell
    public TBaseObject_IncHealthSpell IncHealthSpell;        // 原 2702  IncHealthSpell: TBaseObject_IncHealthSpell
    public TBaseObject_HealthSpellChanged HealthSpellChanged;        // 原 2703  HealthSpellChanged: TBaseObject_HealthSpellChanged
    public TBaseObject_FeatureChanged FeatureChanged;        // 原 2704  FeatureChanged: TBaseObject_FeatureChanged
    public TBaseObject_WeightChanged WeightChanged;        // 原 2705  WeightChanged: TBaseObject_WeightChanged
    public TBaseObject_GetHitStruckDamage GetHitStruckDamage;        // 原 2706  GetHitStruckDamage: TBaseObject_GetHitStruckDamage
    public TBaseObject_GetMagStruckDamage GetMagStruckDamage;        // 原 2707  GetMagStruckDamage: TBaseObject_GetMagStruckDamage
    public TBaseObject_GetActorIcon GetActorIcon;        // 原 2708  GetActorIcon: TBaseObject_GetActorIcon
    public TBaseObject_SetActorIcon SetActorIcon;        // 原 2709  SetActorIcon: TBaseObject_SetActorIcon
    public TBaseObject_RefUseIcons RefUseIcons;        // 原 2710  RefUseIcons: TBaseObject_RefUseIcons
    public TBaseObject_RefUseEffects RefUseEffects;        // 原 2711  RefUseEffects: TBaseObject_RefUseEffects
    public TBaseObject_SpaceMove SpaceMove;        // 原 2712  SpaceMove: TBaseObject_SpaceMove
    public TBaseObject_MapRandomMove MapRandomMove;        // 原 2713  MapRandomMove: TBaseObject_MapRandomMove
    public TBaseObject_CanMove CanMove;        // 原 2714  CanMove: TBaseObject_CanMove
    public TBaseObject_CanRun CanRun;        // 原 2715  CanRun: TBaseObject_CanRun
    public TBaseObject_TurnTo TurnTo;        // 原 2716  TurnTo: TBaseObject_TurnTo
    public TBaseObject_WalkTo WalkTo;        // 原 2717  WalkTo: TBaseObject_WalkTo
    public TBaseObject_RunTo RunTo;        // 原 2718  RunTo: TBaseObject_RunTo
    public TBaseObject_PluginList PluginList;        // 原 2719  PluginList: TBaseObject_PluginList
    public fixed long Reserved[100];        // 原 2720  Reserved: array of 99 x100
}

/// <summary>原文 TSmartObjectFunc（PluginInterface.pas:2723-2828,104 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSmartObjectFunc
{
    public TSmartObject_GetMagicList GetMagicList;        // 原 2724  GetMagicList: TSmartObject_GetMagicList
    public TSmartObject_GetUseItem GetUseItem;        // 原 2725  GetUseItem: TSmartObject_GetUseItem
    public TSmartObject_GetJewelryBoxStatus GetJewelryBoxStatus;        // 原 2726  GetJewelryBoxStatus: TSmartObject_GetJewelryBoxStatus
    public TSmartObject_SetJewelryBoxStatus SetJewelryBoxStatus;        // 原 2727  SetJewelryBoxStatus: TSmartObject_SetJewelryBoxStatus
    public TSmartObject_GetJewelryItem GetJewelryItem;        // 原 2728  GetJewelryItem: TSmartObject_GetJewelryItem
    public TSmartObject_GetIsShowGodBless GetIsShowGodBless;        // 原 2729  GetIsShowGodBless: TSmartObject_GetIsShowGodBless
    public TSmartObject_SetIsShowGodBless SetIsShowGodBless;        // 原 2730  SetIsShowGodBless: TSmartObject_SetIsShowGodBless
    public TSmartObject_GetGodBlessItemsState GetGodBlessItemsState;        // 原 2731  GetGodBlessItemsState: TSmartObject_GetGodBlessItemsState
    public TSmartObject_SetGodBlessItemsState SetGodBlessItemsState;        // 原 2732  SetGodBlessItemsState: TSmartObject_SetGodBlessItemsState
    public TSmartObject_GetGodBlessItem GetGodBlessItem;        // 原 2733  GetGodBlessItem: TSmartObject_GetGodBlessItem
    public TSmartObject_GetFengHaoItems GetFengHaoItems;        // 原 2734  GetFengHaoItems: TSmartObject_GetFengHaoItems
    public TSmartObject_GetActiveFengHao GetActiveFengHao;        // 原 2735  GetActiveFengHao: TSmartObject_GetActiveFengHao
    public TSmartObject_SetActiveFengHao SetActiveFengHao;        // 原 2736  SetActiveFengHao: TSmartObject_SetActiveFengHao
    public TSmartObject_ActiveFengHaoChanged ActiveFengHaoChanged;        // 原 2737  ActiveFengHaoChanged: TSmartObject_ActiveFengHaoChanged
    public TSmartObject_DeleteFengHao DeleteFengHao;        // 原 2738  DeleteFengHao: TSmartObject_DeleteFengHao
    public TSmartObject_ClearFengHao ClearFengHao;        // 原 2739  ClearFengHao: TSmartObject_ClearFengHao
    public TSmartObject_GetMoveSpeed GetMoveSpeed;        // 原 2740  GetMoveSpeed: TSmartObject_GetMoveSpeed
    public TSmartObject_SetMoveSpeed SetMoveSpeed;        // 原 2741  SetMoveSpeed: TSmartObject_SetMoveSpeed
    public TSmartObject_GetAttackSpeed GetAttackSpeed;        // 原 2742  GetAttackSpeed: TSmartObject_GetAttackSpeed
    public TSmartObject_SetAttackSpeed SetAttackSpeed;        // 原 2743  SetAttackSpeed: TSmartObject_SetAttackSpeed
    public TSmartObject_GetSpellSpeed GetSpellSpeed;        // 原 2744  GetSpellSpeed: TSmartObject_GetSpellSpeed
    public TSmartObject_SetSpellSpeed SetSpellSpeed;        // 原 2745  SetSpellSpeed: TSmartObject_SetSpellSpeed
    public TSmartObject_RefGameSpeed RefGameSpeed;        // 原 2746  RefGameSpeed: TSmartObject_RefGameSpeed
    public TSmartObject_GetIsButch GetIsButch;        // 原 2747  GetIsButch: TSmartObject_GetIsButch
    public TSmartObject_SetIsButch SetIsButch;        // 原 2748  SetIsButch: TSmartObject_SetIsButch
    public TSmartObject_GetIsTrainingNG GetIsTrainingNG;        // 原 2749  GetIsTrainingNG: TSmartObject_GetIsTrainingNG
    public TSmartObject_SetIsTrainingNG SetIsTrainingNG;        // 原 2750  SetIsTrainingNG: TSmartObject_SetIsTrainingNG
    public TSmartObject_GetIsTrainingXF GetIsTrainingXF;        // 原 2751  GetIsTrainingXF: TSmartObject_GetIsTrainingXF
    public TSmartObject_SetIsTrainingXF SetIsTrainingXF;        // 原 2752  SetIsTrainingXF: TSmartObject_SetIsTrainingXF
    public TSmartObject_GetIsOpenLastContinuous GetIsOpenLastContinuous;        // 原 2753  GetIsOpenLastContinuous: TSmartObject_GetIsOpenLastContinuous
    public TSmartObject_SetIsOpenLastContinuous SetIsOpenLastContinuous;        // 原 2754  SetIsOpenLastContinuous: TSmartObject_SetIsOpenLastContinuous
    public TSmartObject_GetContinuousMagicOrder GetContinuousMagicOrder;        // 原 2755  GetContinuousMagicOrder: TSmartObject_GetContinuousMagicOrder
    public TSmartObject_SetContinuousMagicOrder SetContinuousMagicOrder;        // 原 2756  SetContinuousMagicOrder: TSmartObject_SetContinuousMagicOrder
    public TSmartObject_GetPKDieLostExp GetPKDieLostExp;        // 原 2757  GetPKDieLostExp: TSmartObject_GetPKDieLostExp
    public TSmartObject_SetPKDieLostExp SetPKDieLostExp;        // 原 2758  SetPKDieLostExp: TSmartObject_SetPKDieLostExp
    public TSmartObject_GetPKDieLostLevel GetPKDieLostLevel;        // 原 2759  GetPKDieLostLevel: TSmartObject_GetPKDieLostLevel
    public TSmartObject_SetPKDieLostLevel SetPKDieLostLevel;        // 原 2760  SetPKDieLostLevel: TSmartObject_SetPKDieLostLevel
    public TSmartObject_GetPKPoint GetPKPoint;        // 原 2761  GetPKPoint: TSmartObject_GetPKPoint
    public TSmartObject_SetPKPoint SetPKPoint;        // 原 2762  SetPKPoint: TSmartObject_SetPKPoint
    public TSmartObject_IncPKPoint IncPKPoint;        // 原 2763  IncPKPoint: TSmartObject_IncPKPoint
    public TSmartObject_DecPKPoint DecPKPoint;        // 原 2764  DecPKPoint: TSmartObject_DecPKPoint
    public TSmartObject_GetPKLevel GetPKLevel;        // 原 2765  GetPKLevel: TSmartObject_GetPKLevel
    public TSmartObject_SetPKLevel SetPKLevel;        // 原 2766  SetPKLevel: TSmartObject_SetPKLevel
    public TSmartObject_GetIsTeleport GetIsTeleport;        // 原 2767  GetIsTeleport: TSmartObject_GetIsTeleport
    public TSmartObject_SetIsTeleport SetIsTeleport;        // 原 2768  SetIsTeleport: TSmartObject_SetIsTeleport
    public TSmartObject_GetIsRevival GetIsRevival;        // 原 2769  GetIsRevival: TSmartObject_GetIsRevival
    public TSmartObject_SetIsRevival SetIsRevival;        // 原 2770  SetIsRevival: TSmartObject_SetIsRevival
    public TSmartObject_GetRevivalTime GetRevivalTime;        // 原 2771  GetRevivalTime: TSmartObject_GetRevivalTime
    public TSmartObject_SetRevivalTime SetRevivalTime;        // 原 2772  SetRevivalTime: TSmartObject_SetRevivalTime
    public TSmartObject_GetIsFlameRing GetIsFlameRing;        // 原 2773  GetIsFlameRing: TSmartObject_GetIsFlameRing
    public TSmartObject_SetIsFlameRing SetIsFlameRing;        // 原 2774  SetIsFlameRing: TSmartObject_SetIsFlameRing
    public TSmartObject_GetIsRecoveryRing GetIsRecoveryRing;        // 原 2775  GetIsRecoveryRing: TSmartObject_GetIsRecoveryRing
    public TSmartObject_SetIsRecoveryRing SetIsRecoveryRing;        // 原 2776  SetIsRecoveryRing: TSmartObject_SetIsRecoveryRing
    public TSmartObject_GetIsMagicShield GetIsMagicShield;        // 原 2777  GetIsMagicShield: TSmartObject_GetIsMagicShield
    public TSmartObject_SetIsMagicShield SetIsMagicShield;        // 原 2778  SetIsMagicShield: TSmartObject_SetIsMagicShield
    public TSmartObject_GetIsMuscleRing GetIsMuscleRing;        // 原 2779  GetIsMuscleRing: TSmartObject_GetIsMuscleRing
    public TSmartObject_SetIsMuscleRing SetIsMuscleRing;        // 原 2780  SetIsMuscleRing: TSmartObject_SetIsMuscleRing
    public TSmartObject_GetIsFastTrain GetIsFastTrain;        // 原 2781  GetIsFastTrain: TSmartObject_GetIsFastTrain
    public TSmartObject_SetIsFastTrain SetIsFastTrain;        // 原 2782  SetIsFastTrain: TSmartObject_SetIsFastTrain
    public TSmartObject_GetIsProbeNecklace GetIsProbeNecklace;        // 原 2783  GetIsProbeNecklace: TSmartObject_GetIsProbeNecklace
    public TSmartObject_SetIsProbeNecklace SetIsProbeNecklace;        // 原 2784  SetIsProbeNecklace: TSmartObject_SetIsProbeNecklace
    public TSmartObject_GetIsRecallSuite GetIsRecallSuite;        // 原 2785  GetIsRecallSuite: TSmartObject_GetIsRecallSuite
    public TSmartObject_SetIsRecallSuite SetIsRecallSuite;        // 原 2786  SetIsRecallSuite: TSmartObject_SetIsRecallSuite
    public TSmartObject_GetIsPirit GetIsPirit;        // 原 2787  GetIsPirit: TSmartObject_GetIsPirit
    public TSmartObject_SetIsPirit SetIsPirit;        // 原 2788  SetIsPirit: TSmartObject_SetIsPirit
    public TSmartObject_GetIsSupermanItem GetIsSupermanItem;        // 原 2789  GetIsSupermanItem: TSmartObject_GetIsSupermanItem
    public TSmartObject_SetIsSupermanItem SetIsSupermanItem;        // 原 2790  SetIsSupermanItem: TSmartObject_SetIsSupermanItem
    public TSmartObject_GetIsExpItem GetIsExpItem;        // 原 2791  GetIsExpItem: TSmartObject_GetIsExpItem
    public TSmartObject_SetIsExpItem SetIsExpItem;        // 原 2792  SetIsExpItem: TSmartObject_SetIsExpItem
    public TSmartObject_GetExpItemValue GetExpItemValue;        // 原 2793  GetExpItemValue: TSmartObject_GetExpItemValue
    public TSmartObject_SetExpItemValue SetExpItemValue;        // 原 2794  SetExpItemValue: TSmartObject_SetExpItemValue
    public TSmartObject_GetExpItemRate GetExpItemRate;        // 原 2795  GetExpItemRate: TSmartObject_GetExpItemRate
    public TSmartObject_GetIsPowerItem GetIsPowerItem;        // 原 2796  GetIsPowerItem: TSmartObject_GetIsPowerItem
    public TSmartObject_SetIsPowerItem SetIsPowerItem;        // 原 2797  SetIsPowerItem: TSmartObject_SetIsPowerItem
    public TSmartObject_GetPowerItemValue GetPowerItemValue;        // 原 2798  GetPowerItemValue: TSmartObject_GetPowerItemValue
    public TSmartObject_SetPowerItemValue SetPowerItemValue;        // 原 2799  SetPowerItemValue: TSmartObject_SetPowerItemValue
    public TSmartObject_GetPowerItemRate GetPowerItemRate;        // 原 2800  GetPowerItemRate: TSmartObject_GetPowerItemRate
    public TSmartObject_GetIsGuildMove GetIsGuildMove;        // 原 2801  GetIsGuildMove: TSmartObject_GetIsGuildMove
    public TSmartObject_SetIsGuildMove SetIsGuildMove;        // 原 2802  SetIsGuildMove: TSmartObject_SetIsGuildMove
    public TSmartObject_GetIsAngryRing GetIsAngryRing;        // 原 2803  GetIsAngryRing: TSmartObject_GetIsAngryRing
    public TSmartObject_SetIsAngryRing SetIsAngryRing;        // 原 2804  SetIsAngryRing: TSmartObject_SetIsAngryRing
    public TSmartObject_GetIsStarRing GetIsStarRing;        // 原 2805  GetIsStarRing: TSmartObject_GetIsStarRing
    public TSmartObject_SetIsStarRing SetIsStarRing;        // 原 2806  SetIsStarRing: TSmartObject_SetIsStarRing
    public TSmartObject_GetIsACItem GetIsACItem;        // 原 2807  GetIsACItem: TSmartObject_GetIsACItem
    public TSmartObject_SetIsACItem SetIsACItem;        // 原 2808  SetIsACItem: TSmartObject_SetIsACItem
    public TSmartObject_GetACItemValue GetACItemValue;        // 原 2809  GetACItemValue: TSmartObject_GetACItemValue
    public TSmartObject_SetACItemValue SetACItemValue;        // 原 2810  SetACItemValue: TSmartObject_SetACItemValue
    public TSmartObject_GetIsMACItem GetIsMACItem;        // 原 2811  GetIsMACItem: TSmartObject_GetIsMACItem
    public TSmartObject_SetIsMACItem SetIsMACItem;        // 原 2812  SetIsMACItem: TSmartObject_SetIsMACItem
    public TSmartObject_GetMACItemValue GetMACItemValue;        // 原 2813  GetMACItemValue: TSmartObject_GetMACItemValue
    public TSmartObject_SetMACItemValue SetMACItemValue;        // 原 2814  SetMACItemValue: TSmartObject_SetMACItemValue
    public TSmartObject_GetIsNoDropItem GetIsNoDropItem;        // 原 2815  GetIsNoDropItem: TSmartObject_GetIsNoDropItem
    public TSmartObject_SetIsNoDropItem SetIsNoDropItem;        // 原 2816  SetIsNoDropItem: TSmartObject_SetIsNoDropItem
    public TSmartObject_GetIsNoDropUseItem GetIsNoDropUseItem;        // 原 2817  GetIsNoDropUseItem: TSmartObject_GetIsNoDropUseItem
    public TSmartObject_SetIsNoDropUseItem SetIsNoDropUseItem;        // 原 2818  SetIsNoDropUseItem: TSmartObject_SetIsNoDropUseItem
    public TSmartObject_GetNGAbility GetNGAbility;        // 原 2819  GetNGAbility: TSmartObject_GetNGAbility
    public TSmartObject_SetNGAbility SetNGAbility;        // 原 2820  SetNGAbility: TSmartObject_SetNGAbility
    public TSmartObject_GetAlcohol GetAlcohol;        // 原 2821  GetAlcohol: TSmartObject_GetAlcohol
    public TSmartObject_SetAlcohol SetAlcohol;        // 原 2822  SetAlcohol: TSmartObject_SetAlcohol
    public TSmartObject_RepairAllItem RepairAllItem;        // 原 2823  RepairAllItem: TSmartObject_RepairAllItem
    public TSmartObject_IsAllowUseMagic IsAllowUseMagic;        // 原 2824  IsAllowUseMagic: TSmartObject_IsAllowUseMagic
    public TSmartObject_SelectMagic SelectMagic;        // 原 2825  SelectMagic: TSmartObject_SelectMagic
    public TSmartObject_AttackTarget AttackTarget;        // 原 2826  AttackTarget: TSmartObject_AttackTarget
    public fixed long Reserved[100];        // 原 2827  Reserved: array of 99 x100
}

/// <summary>原文 TPlayObjectFunc（PluginInterface.pas:2830-2991,160 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TPlayObjectFunc
{
    public TPlayObject_GetUserID GetUserID;        // 原 2831  GetUserID: TPlayObject_GetUserID
    public TPlayObject_GetIPAddr GetIPAddr;        // 原 2832  GetIPAddr: TPlayObject_GetIPAddr
    public TPlayObject_GetIPLocal GetIPLocal;        // 原 2833  GetIPLocal: TPlayObject_GetIPLocal
    public TPlayObject_GetMachineID GetMachineID;        // 原 2834  GetMachineID: TPlayObject_GetMachineID
    public TPlayObject_GetIsReadyRun GetIsReadyRun;        // 原 2835  GetIsReadyRun: TPlayObject_GetIsReadyRun
    public TPlayObject_GetLogonTime GetLogonTime;        // 原 2836  GetLogonTime: TPlayObject_GetLogonTime
    public TPlayObject_GetSoftVerDate GetSoftVerDate;        // 原 2837  GetSoftVerDate: TPlayObject_GetSoftVerDate
    public TPlayObject_GetClientType GetClientType;        // 原 2838  GetClientType: TPlayObject_GetClientType
    public TPlayObject_IsOldClient IsOldClient;        // 原 2839  IsOldClient: TPlayObject_IsOldClient
    public TPlayObject_GetScreenWidth GetScreenWidth;        // 原 2840  GetScreenWidth: TPlayObject_GetScreenWidth
    public TPlayObject_GetScreenHeight GetScreenHeight;        // 原 2841  GetScreenHeight: TPlayObject_GetScreenHeight
    public TPlayObject_GetClientViewRange GetClientViewRange;        // 原 2842  GetClientViewRange: TPlayObject_GetClientViewRange
    public TPlayObject_GetRelevel GetRelevel;        // 原 2843  GetRelevel: TPlayObject_GetRelevel
    public TPlayObject_SetRelevel SetRelevel;        // 原 2844  SetRelevel: TPlayObject_SetRelevel
    public TPlayObject_GetBonusPoint GetBonusPoint;        // 原 2845  GetBonusPoint: TPlayObject_GetBonusPoint
    public TPlayObject_SetBonusPoint SetBonusPoint;        // 原 2846  SetBonusPoint: TPlayObject_SetBonusPoint
    public TPlayObject_SendAdjustBonus SendAdjustBonus;        // 原 2847  SendAdjustBonus: TPlayObject_SendAdjustBonus
    public TPlayObject_GetHeroName GetHeroName;        // 原 2848  GetHeroName: TPlayObject_GetHeroName
    public TPlayObject_GetDeputyHeroName GetDeputyHeroName;        // 原 2849  GetDeputyHeroName: TPlayObject_GetDeputyHeroName
    public TPlayObject_GetDeputyHeroJob GetDeputyHeroJob;        // 原 2850  GetDeputyHeroJob: TPlayObject_GetDeputyHeroJob
    public TPlayObject_GetMyHero GetMyHero;        // 原 2851  GetMyHero: TPlayObject_GetMyHero
    public TPlayObject_GetFixedHero GetFixedHero;        // 原 2852  GetFixedHero: TPlayObject_GetFixedHero
    public TPlayObject_ClientHeroLogOn ClientHeroLogOn;        // 原 2853  ClientHeroLogOn: TPlayObject_ClientHeroLogOn
    public TPlayObject_GetStorageHero GetStorageHero;        // 原 2854  GetStorageHero: TPlayObject_GetStorageHero
    public TPlayObject_GetStorageDeputyHero GetStorageDeputyHero;        // 原 2855  GetStorageDeputyHero: TPlayObject_GetStorageDeputyHero
    public TPlayObject_GetIsStorageOpen GetIsStorageOpen;        // 原 2856  GetIsStorageOpen: TPlayObject_GetIsStorageOpen
    public TPlayObject_SetIsStorageOpen SetIsStorageOpen;        // 原 2857  SetIsStorageOpen: TPlayObject_SetIsStorageOpen
    public TPlayObject_GetGold GetGold;        // 原 2858  GetGold: TPlayObject_GetGold
    public TPlayObject_SetGold SetGold;        // 原 2859  SetGold: TPlayObject_SetGold
    public TPlayObject_GetGoldMax GetGoldMax;        // 原 2860  GetGoldMax: TPlayObject_GetGoldMax
    public TPlayObject_IncGold IncGold;        // 原 2861  IncGold: TPlayObject_IncGold
    public TPlayObject_DecGold DecGold;        // 原 2862  DecGold: TPlayObject_DecGold
    public TPlayObject_GoldChanged GoldChanged;        // 原 2863  GoldChanged: TPlayObject_GoldChanged
    public TPlayObject_GetGameGold GetGameGold;        // 原 2864  GetGameGold: TPlayObject_GetGameGold
    public TPlayObject_SetGameGold SetGameGold;        // 原 2865  SetGameGold: TPlayObject_SetGameGold
    public TPlayObject_IncGameGold IncGameGold;        // 原 2866  IncGameGold: TPlayObject_IncGameGold
    public TPlayObject_DecGameGold DecGameGold;        // 原 2867  DecGameGold: TPlayObject_DecGameGold
    public TPlayObject_GameGoldChanged GameGoldChanged;        // 原 2868  GameGoldChanged: TPlayObject_GameGoldChanged
    public TPlayObject_GetGamePoint GetGamePoint;        // 原 2869  GetGamePoint: TPlayObject_GetGamePoint
    public TPlayObject_SetGamePoint SetGamePoint;        // 原 2870  SetGamePoint: TPlayObject_SetGamePoint
    public TPlayObject_IncGamePoint IncGamePoint;        // 原 2871  IncGamePoint: TPlayObject_IncGamePoint
    public TPlayObject_DecGamePoint DecGamePoint;        // 原 2872  DecGamePoint: TPlayObject_DecGamePoint
    public TPlayObject_GetGameDiamond GetGameDiamond;        // 原 2873  GetGameDiamond: TPlayObject_GetGameDiamond
    public TPlayObject_SetGameDiamond SetGameDiamond;        // 原 2874  SetGameDiamond: TPlayObject_SetGameDiamond
    public TPlayObject_IncGameDiamond IncGameDiamond;        // 原 2875  IncGameDiamond: TPlayObject_IncGameDiamond
    public TPlayObject_DecGameDiamond DecGameDiamond;        // 原 2876  DecGameDiamond: TPlayObject_DecGameDiamond
    public TPlayObject_NewGamePointChanged NewGamePointChanged;        // 原 2877  NewGamePointChanged: TPlayObject_NewGamePointChanged
    public TPlayObject_GetGameGird GetGameGird;        // 原 2878  GetGameGird: TPlayObject_GetGameGird
    public TPlayObject_SetGameGird SetGameGird;        // 原 2879  SetGameGird: TPlayObject_SetGameGird
    public TPlayObject_IncGameGird IncGameGird;        // 原 2880  IncGameGird: TPlayObject_IncGameGird
    public TPlayObject_DecGameGird DecGameGird;        // 原 2881  DecGameGird: TPlayObject_DecGameGird
    public TPlayObject_GetGameGoldEx GetGameGoldEx;        // 原 2882  GetGameGoldEx: TPlayObject_GetGameGoldEx
    public TPlayObject_SetGameGoldEx SetGameGoldEx;        // 原 2883  SetGameGoldEx: TPlayObject_SetGameGoldEx
    public TPlayObject_GetGameGlory GetGameGlory;        // 原 2884  GetGameGlory: TPlayObject_GetGameGlory
    public TPlayObject_SetGameGlory SetGameGlory;        // 原 2885  SetGameGlory: TPlayObject_SetGameGlory
    public TPlayObject_IncGameGlory IncGameGlory;        // 原 2886  IncGameGlory: TPlayObject_IncGameGlory
    public TPlayObject_DecGameGlory DecGameGlory;        // 原 2887  DecGameGlory: TPlayObject_DecGameGlory
    public TPlayObject_GameGloryChanged GameGloryChanged;        // 原 2888  GameGloryChanged: TPlayObject_GameGloryChanged
    public TPlayObject_GetPayMentPoint GetPayMentPoint;        // 原 2889  GetPayMentPoint: TPlayObject_GetPayMentPoint
    public TPlayObject_SetPayMentPoint SetPayMentPoint;        // 原 2890  SetPayMentPoint: TPlayObject_SetPayMentPoint
    public TPlayObject_GetMemberType GetMemberType;        // 原 2891  GetMemberType: TPlayObject_GetMemberType
    public TPlayObject_SetMemberType SetMemberType;        // 原 2892  SetMemberType: TPlayObject_SetMemberType
    public TPlayObject_GetMemberLevel GetMemberLevel;        // 原 2893  GetMemberLevel: TPlayObject_GetMemberLevel
    public TPlayObject_SetMemberLevel SetMemberLevel;        // 原 2894  SetMemberLevel: TPlayObject_SetMemberLevel
    public TPlayObject_GetContribution GetContribution;        // 原 2895  GetContribution: TPlayObject_GetContribution
    public TPlayObject_SetContribution SetContribution;        // 原 2896  SetContribution: TPlayObject_SetContribution
    public TPlayObejct_IncExp IncExp;        // 原 2897  IncExp: TPlayObejct_IncExp
    public TPlayObject_SendExpChanged SendExpChanged;        // 原 2898  SendExpChanged: TPlayObject_SendExpChanged
    public TPlayObject_IncExpNG IncExpNG;        // 原 2899  IncExpNG: TPlayObject_IncExpNG
    public TPlayObject_SendExpNGChanged SendExpNGChanged;        // 原 2900  SendExpNGChanged: TPlayObject_SendExpNGChanged
    public TPlayObject_IncBeadExp IncBeadExp;        // 原 2901  IncBeadExp: TPlayObject_IncBeadExp
    public TPlayObject_GetVarP GetVarP;        // 原 2902  GetVarP: TPlayObject_GetVarP
    public TPlayObject_SetVarP SetVarP;        // 原 2903  SetVarP: TPlayObject_SetVarP
    public TPlayObject_GetVarM GetVarM;        // 原 2904  GetVarM: TPlayObject_GetVarM
    public TPlayObject_SetVarM SetVarM;        // 原 2905  SetVarM: TPlayObject_SetVarM
    public TPlayObject_GetVarD GetVarD;        // 原 2906  GetVarD: TPlayObject_GetVarD
    public TPlayObject_SetVarD SetVarD;        // 原 2907  SetVarD: TPlayObject_SetVarD
    public TPlayObject_GetVarU GetVarU;        // 原 2908  GetVarU: TPlayObject_GetVarU
    public TPlayObject_SetVarU SetVarU;        // 原 2909  SetVarU: TPlayObject_SetVarU
    public TPlayObject_GetVarT GetVarT;        // 原 2910  GetVarT: TPlayObject_GetVarT
    public TPlayObject_SetVarT SetVarT;        // 原 2911  SetVarT: TPlayObject_SetVarT
    public TPlayObject_GetVarN GetVarN;        // 原 2912  GetVarN: TPlayObject_GetVarN
    public TPlayObject_SetVarN SetVarN;        // 原 2913  SetVarN: TPlayObject_SetVarN
    public TPlayObject_GetVarS GetVarS;        // 原 2914  GetVarS: TPlayObject_GetVarS
    public TPlayObject_SetVarS SetVarS;        // 原 2915  SetVarS: TPlayObject_SetVarS
    public TPlayObject_GetDynamicVarList GetDynamicVarList;        // 原 2916  GetDynamicVarList: TPlayObject_GetDynamicVarList
    public TPlayObject_GetQuestFlagStatus GetQuestFlagStatus;        // 原 2917  GetQuestFlagStatus: TPlayObject_GetQuestFlagStatus
    public TPlayObject_SetQuestFlagStatus SetQuestFlagStatus;        // 原 2918  SetQuestFlagStatus: TPlayObject_SetQuestFlagStatus
    public TPlayObject_IsOffLine IsOffLine;        // 原 2919  IsOffLine: TPlayObject_IsOffLine
    public TPlayObject_IsMaster IsMaster;        // 原 2920  IsMaster: TPlayObject_IsMaster
    public TPlayObject_GetMasterName GetMasterName;        // 原 2921  GetMasterName: TPlayObject_GetMasterName
    public TPlayObject_GetMasterHuman GetMasterHuman;        // 原 2922  GetMasterHuman: TPlayObject_GetMasterHuman
    public TPlayObject_GetApprenticeNO GetApprenticeNO;        // 原 2923  GetApprenticeNO: TPlayObject_GetApprenticeNO
    public TPlayObject_GetOnlineApprenticeList GetOnlineApprenticeList;        // 原 2924  GetOnlineApprenticeList: TPlayObject_GetOnlineApprenticeList
    public TPlayObject_GetAllApprenticeList GetAllApprenticeList;        // 原 2925  GetAllApprenticeList: TPlayObject_GetAllApprenticeList
    public TPlayObject_GetDearName GetDearName;        // 原 2926  GetDearName: TPlayObject_GetDearName
    public TPlayObject_GetDearHuman GetDearHuman;        // 原 2927  GetDearHuman: TPlayObject_GetDearHuman
    public TPlayObject_GetMarryCount GetMarryCount;        // 原 2928  GetMarryCount: TPlayObject_GetMarryCount
    public TPlayObject_GetGroupOwner GetGroupOwner;        // 原 2929  GetGroupOwner: TPlayObject_GetGroupOwner
    public TPlayObject_GetGroupMembers GetGroupMembers;        // 原 2930  GetGroupMembers: TPlayObject_GetGroupMembers
    public TPlayObject_GetIsLockLogin GetIsLockLogin;        // 原 2931  GetIsLockLogin: TPlayObject_GetIsLockLogin
    public TPlayObject_SetIsLockLogin SetIsLockLogin;        // 原 2932  SetIsLockLogin: TPlayObject_SetIsLockLogin
    public TPlayObject_GetIsAllowGroup GetIsAllowGroup;        // 原 2933  GetIsAllowGroup: TPlayObject_GetIsAllowGroup
    public TPlayObject_SetIsAllowGroup SetIsAllowGroup;        // 原 2934  SetIsAllowGroup: TPlayObject_SetIsAllowGroup
    public TPlayObject_GetIsAllowGroupReCall GetIsAllowGroupReCall;        // 原 2935  GetIsAllowGroupReCall: TPlayObject_GetIsAllowGroupReCall
    public TPlayObject_SetIsAllowGroupReCall SetIsAllowGroupReCall;        // 原 2936  SetIsAllowGroupReCall: TPlayObject_SetIsAllowGroupReCall
    public TPlayObject_GetIsAllowGuildReCall GetIsAllowGuildReCall;        // 原 2937  GetIsAllowGuildReCall: TPlayObject_GetIsAllowGuildReCall
    public TPlayObject_SetIsAllowGuildReCall SetIsAllowGuildReCall;        // 原 2938  SetIsAllowGuildReCall: TPlayObject_SetIsAllowGuildReCall
    public TPlayObject_GetIsAllowTrading GetIsAllowTrading;        // 原 2939  GetIsAllowTrading: TPlayObject_GetIsAllowTrading
    public TPlayObject_SetIsAllowTrading SetIsAllowTrading;        // 原 2940  SetIsAllowTrading: TPlayObject_SetIsAllowTrading
    public TPlayObject_GetIsDisableInviteHorseRiding GetIsDisableInviteHorseRiding;        // 原 2941  GetIsDisableInviteHorseRiding: TPlayObject_GetIsDisableInviteHorseRiding
    public TPlayObject_SetIsDisableInviteHorseRiding SetIsDisableInviteHorseRiding;        // 原 2942  SetIsDisableInviteHorseRiding: TPlayObject_SetIsDisableInviteHorseRiding
    public TPlayObject_GetIsGameGoldTrading GetIsGameGoldTrading;        // 原 2943  GetIsGameGoldTrading: TPlayObject_GetIsGameGoldTrading
    public TPlayObject_SetIsGameGoldTrading SetIsGameGoldTrading;        // 原 2944  SetIsGameGoldTrading: TPlayObject_SetIsGameGoldTrading
    public TPlayObject_GetIsNewServer GetIsNewServer;        // 原 2945  GetIsNewServer: TPlayObject_GetIsNewServer
    public TPlayObject_GetIsFilterGlobalDropItemMsg GetIsFilterGlobalDropItemMsg;        // 原 2946  GetIsFilterGlobalDropItemMsg: TPlayObject_GetIsFilterGlobalDropItemMsg
    public TPlayObject_SetIsFilterGlobalDropItemMsg SetIsFilterGlobalDropItemMsg;        // 原 2947  SetIsFilterGlobalDropItemMsg: TPlayObject_SetIsFilterGlobalDropItemMsg
    public TPlayObject_GetIsFilterGlobalCenterMsg GetIsFilterGlobalCenterMsg;        // 原 2948  GetIsFilterGlobalCenterMsg: TPlayObject_GetIsFilterGlobalCenterMsg
    public TPlayObject_SetIsFilterGlobalCenterMsg SetIsFilterGlobalCenterMsg;        // 原 2949  SetIsFilterGlobalCenterMsg: TPlayObject_SetIsFilterGlobalCenterMsg
    public TPlayObject_GetIsFilterGolbalSendMsg GetIsFilterGolbalSendMsg;        // 原 2950  GetIsFilterGolbalSendMsg: TPlayObject_GetIsFilterGolbalSendMsg
    public TPlayObject_SetIsFilterGolbalSendMsg SetIsFilterGolbalSendMsg;        // 原 2951  SetIsFilterGolbalSendMsg: TPlayObject_SetIsFilterGolbalSendMsg
    public TPlayObject_GetIsPleaseDrink GetIsPleaseDrink;        // 原 2952  GetIsPleaseDrink: TPlayObject_GetIsPleaseDrink
    public TPlayObject_GetIsDrinkWineQuality GetIsDrinkWineQuality;        // 原 2953  GetIsDrinkWineQuality: TPlayObject_GetIsDrinkWineQuality
    public TPlayObject_SetIsDrinkWineQuality SetIsDrinkWineQuality;        // 原 2954  SetIsDrinkWineQuality: TPlayObject_SetIsDrinkWineQuality
    public TPlayObject_GetIsDrinkWineAlcohol GetIsDrinkWineAlcohol;        // 原 2955  GetIsDrinkWineAlcohol: TPlayObject_GetIsDrinkWineAlcohol
    public TPlayObject_SetIsDrinkWineAlcohol SetIsDrinkWineAlcohol;        // 原 2956  SetIsDrinkWineAlcohol: TPlayObject_SetIsDrinkWineAlcohol
    public TPlayObject_GetIsDrinkWineDrunk GetIsDrinkWineDrunk;        // 原 2957  GetIsDrinkWineDrunk: TPlayObject_GetIsDrinkWineDrunk
    public TPlayObject_SetIsDrinkWineDrunk SetIsDrinkWineDrunk;        // 原 2958  SetIsDrinkWineDrunk: TPlayObject_SetIsDrinkWineDrunk
    public TPlayObject_MoveToHome MoveToHome;        // 原 2959  MoveToHome: TPlayObject_MoveToHome
    public TPlayObject_MoveRandomToHome MoveRandomToHome;        // 原 2960  MoveRandomToHome: TPlayObject_MoveRandomToHome
    public TPlayObject_SendSocket SendSocket;        // 原 2961  SendSocket: TPlayObject_SendSocket
    public TPlayObject_SendDefMessage SendDefMessage;        // 原 2962  SendDefMessage: TPlayObject_SendDefMessage
    public TPlayObject_SendMoveMsg SendMoveMsg;        // 原 2963  SendMoveMsg: TPlayObject_SendMoveMsg
    public TPlayObject_SendCenterMsg SendCenterMsg;        // 原 2964  SendCenterMsg: TPlayObject_SendCenterMsg
    public TPlayObject_SendTopBroadCastMsg SendTopBroadCastMsg;        // 原 2965  SendTopBroadCastMsg: TPlayObject_SendTopBroadCastMsg
    public TPlayObject_CheckTakeOnItems CheckTakeOnItems;        // 原 2966  CheckTakeOnItems: TPlayObject_CheckTakeOnItems
    public TPlayObject_ProcessUseItemSkill ProcessUseItemSkill;        // 原 2967  ProcessUseItemSkill: TPlayObject_ProcessUseItemSkill
    public TPlayObject_SendUseItems SendUseItems;        // 原 2968  SendUseItems: TPlayObject_SendUseItems
    public TPlayObject_SendAddItem SendAddItem;        // 原 2969  SendAddItem: TPlayObject_SendAddItem
    public TPlayObject_SendDelItemList SendDelItemList;        // 原 2970  SendDelItemList: TPlayObject_SendDelItemList
    public TPlayObject_SendDelItem SendDelItem;        // 原 2971  SendDelItem: TPlayObject_SendDelItem
    public TPlayObject_SendUpdateItem SendUpdateItem;        // 原 2972  SendUpdateItem: TPlayObject_SendUpdateItem
    public TPlayObject_SendItemDuraChange SendItemDuraChange;        // 原 2973  SendItemDuraChange: TPlayObject_SendItemDuraChange
    public TPlayObject_SendBagItems SendBagItems;        // 原 2974  SendBagItems: TPlayObject_SendBagItems
    public TPlayObject_SendJewelryBoxItems SendJewelryBoxItems;        // 原 2975  SendJewelryBoxItems: TPlayObject_SendJewelryBoxItems
    public TPlayObject_SendGodBlessItems SendGodBlessItems;        // 原 2976  SendGodBlessItems: TPlayObject_SendGodBlessItems
    public TPlayObject_SendOpenGodBlessItem SendOpenGodBlessItem;        // 原 2977  SendOpenGodBlessItem: TPlayObject_SendOpenGodBlessItem
    public TPlayObject_SendCloseGodBlessItem SendCloseGodBlessItem;        // 原 2978  SendCloseGodBlessItem: TPlayObject_SendCloseGodBlessItem
    public TPlayObject_SendUseMagics SendUseMagics;        // 原 2979  SendUseMagics: TPlayObject_SendUseMagics
    public TPlayObject_SendAddMagic SendAddMagic;        // 原 2980  SendAddMagic: TPlayObject_SendAddMagic
    public TPlayObject_SendDelMagic SendDelMagic;        // 原 2981  SendDelMagic: TPlayObject_SendDelMagic
    public TPlayObject_SendFengHaoItems SendFengHaoItems;        // 原 2982  SendFengHaoItems: TPlayObject_SendFengHaoItems
    public TPlayObject_SendAddFengHaoItem SendAddFengHaoItem;        // 原 2983  SendAddFengHaoItem: TPlayObject_SendAddFengHaoItem
    public TPlayObject_SendDelFengHaoItem SendDelFengHaoItem;        // 原 2984  SendDelFengHaoItem: TPlayObject_SendDelFengHaoItem
    public TPlayObject_SendSocketStatusFail SendSocketStatusFail;        // 原 2985  SendSocketStatusFail: TPlayObject_SendSocketStatusFail
    public TPlayObject_PlayEffect PlayEffect;        // 原 2986  PlayEffect: TPlayObject_PlayEffect
    public TPlayObject_IsAutoPlayGame IsAutoPlayGame;        // 原 2987  IsAutoPlayGame: TPlayObject_IsAutoPlayGame
    public TPlayObject_StartAutoPlayGame StartAutoPlayGame;        // 原 2988  StartAutoPlayGame: TPlayObject_StartAutoPlayGame
    public TPlayObject_StopAutoPlayGame StopAutoPlayGame;        // 原 2989  StopAutoPlayGame: TPlayObject_StopAutoPlayGame
    public fixed long Reserved[100];        // 原 2990  Reserved: array of 99 x100
}

/// <summary>原文 TDummyObjectFunc（PluginInterface.pas:2993-2998,4 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDummyObjectFunc
{
    public TDummyObject_IsStart IsStart;        // 原 2994  IsStart: TDummyObject_IsStart
    public TDummyObject_Start Start;        // 原 2995  Start: TDummyObject_Start
    public TDummyObject_Stop Stop;        // 原 2996  Stop: TDummyObject_Stop
    public fixed long Reserved[100];        // 原 2997  Reserved: array of 99 x100
}

/// <summary>原文 THeroObjectFunc（PluginInterface.pas:3000-3035,34 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct THeroObjectFunc
{
    public THeroObject_GetAttackMode GetAttackMode;        // 原 3001  GetAttackMode: THeroObject_GetAttackMode
    public THeroObject_SetAttackMode SetAttackMode;        // 原 3002  SetAttackMode: THeroObject_SetAttackMode
    public THeroObject_SetNextAttackMode SetNextAttackMode;        // 原 3003  SetNextAttackMode: THeroObject_SetNextAttackMode
    public THeroObject_GetBagCount GetBagCount;        // 原 3004  GetBagCount: THeroObject_GetBagCount
    public THeroObject_GetAngryValue GetAngryValue;        // 原 3005  GetAngryValue: THeroObject_GetAngryValue
    public THeroObject_GetLoyalPoint GetLoyalPoint;        // 原 3006  GetLoyalPoint: THeroObject_GetLoyalPoint
    public THeroObject_SetLoyalPoint SetLoyalPoint;        // 原 3007  SetLoyalPoint: THeroObject_SetLoyalPoint
    public THeroObject_SendLoyalPointChanged SendLoyalPointChanged;        // 原 3008  SendLoyalPointChanged: THeroObject_SendLoyalPointChanged
    public THeroObject_IsDeputy IsDeputy;        // 原 3009  IsDeputy: THeroObject_IsDeputy
    public THeroObject_GetMasterName GetMasterName;        // 原 3010  GetMasterName: THeroObject_GetMasterName
    public THeroObject_GetQuestFlagStatus GetQuestFlagStatus;        // 原 3011  GetQuestFlagStatus: THeroObject_GetQuestFlagStatus
    public THeroObject_SetQuestFlagStatus SetQuestFlagStatus;        // 原 3012  SetQuestFlagStatus: THeroObject_SetQuestFlagStatus
    public THeroObject_SendUseItems SendUseItems;        // 原 3013  SendUseItems: THeroObject_SendUseItems
    public THeroObject_SendBagItems SendBagItems;        // 原 3014  SendBagItems: THeroObject_SendBagItems
    public THeroObject_SendJewelryBoxItems SendJewelryBoxItems;        // 原 3015  SendJewelryBoxItems: THeroObject_SendJewelryBoxItems
    public THeroObject_SendGodBlessItems SendGodBlessItems;        // 原 3016  SendGodBlessItems: THeroObject_SendGodBlessItems
    public THeroObject_SendOpenGodBlessItem SendOpenGodBlessItem;        // 原 3017  SendOpenGodBlessItem: THeroObject_SendOpenGodBlessItem
    public THeroObject_SendCloseGodBlessItem SendCloseGodBlessItem;        // 原 3018  SendCloseGodBlessItem: THeroObject_SendCloseGodBlessItem
    public THeroObject_SendAddItem SendAddItem;        // 原 3019  SendAddItem: THeroObject_SendAddItem
    public THeroObject_SendDelItem SendDelItem;        // 原 3020  SendDelItem: THeroObject_SendDelItem
    public THeroObject_SendUpdateItem SendUpdateItem;        // 原 3021  SendUpdateItem: THeroObject_SendUpdateItem
    public THeroObject_SendItemDuraChange SendItemDuraChange;        // 原 3022  SendItemDuraChange: THeroObject_SendItemDuraChange
    public THeroObject_SendUseMagics SendUseMagics;        // 原 3023  SendUseMagics: THeroObject_SendUseMagics
    public THeroObject_SendAddMagic SendAddMagic;        // 原 3024  SendAddMagic: THeroObject_SendAddMagic
    public THeroObject_SendDelMagic SendDelMagic;        // 原 3025  SendDelMagic: THeroObject_SendDelMagic
    public THeroObject_FindGroupMagic FindGroupMagic;        // 原 3026  FindGroupMagic: THeroObject_FindGroupMagic
    public THeroObject_GetGroupMagicId GetGroupMagicId;        // 原 3027  GetGroupMagicId: THeroObject_GetGroupMagicId
    public THeroObject_SendFengHaoItems SendFengHaoItems;        // 原 3028  SendFengHaoItems: THeroObject_SendFengHaoItems
    public THeroObject_SendAddFengHaoItem SendAddFengHaoItem;        // 原 3029  SendAddFengHaoItem: THeroObject_SendAddFengHaoItem
    public THeroObject_SendDelFengHaoItem SendDelFengHaoItem;        // 原 3030  SendDelFengHaoItem: THeroObject_SendDelFengHaoItem
    public THeroObject_IncExp IncExp;        // 原 3031  IncExp: THeroObject_IncExp
    public THeroObject_IncExpNG IncExpNG;        // 原 3032  IncExpNG: THeroObject_IncExpNG
    public THeroObject_IsOldClient IsOldClient;        // 原 3033  IsOldClient: THeroObject_IsOldClient
    public fixed long Reserved[100];        // 原 3034  Reserved: array of 99 x100
}

/// <summary>原文 TNormNpcFunc（PluginInterface.pas:3037-3057,19 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TNormNpcFunc
{
    public TNormNpc_Create Create;        // 原 3038  Create: TNormNpc_Create
    public TNormNpc_LoadNpcScript LoadNpcScript;        // 原 3039  LoadNpcScript: TNormNpc_LoadNpcScript
    public TNormNpc_ClearScript ClearScript;        // 原 3040  ClearScript: TNormNpc_ClearScript
    public TNormNpc_GetFilePath GetFilePath;        // 原 3041  GetFilePath: TNormNpc_GetFilePath
    public TNormNpc_SetFilePath SetFilePath;        // 原 3042  SetFilePath: TNormNpc_SetFilePath
    public TNormNpc_GetPath GetPath;        // 原 3043  GetPath: TNormNpc_GetPath
    public TNormNpc_SetPath SetPath;        // 原 3044  SetPath: TNormNpc_SetPath
    public TNormNpc_GetIsHide GetIsHide;        // 原 3045  GetIsHide: TNormNpc_GetIsHide
    public TNormNpc_SetIsHide SetIsHide;        // 原 3046  SetIsHide: TNormNpc_SetIsHide
    public TNormNpc_GetIsQuest GetIsQuest;        // 原 3047  GetIsQuest: TNormNpc_GetIsQuest
    public TNormNpc_GetLineVariableText GetLineVariableText;        // 原 3048  GetLineVariableText: TNormNpc_GetLineVariableText
    public TNormNpc_GotoLable GotoLable;        // 原 3049  GotoLable: TNormNpc_GotoLable
    public TNormNpc_SendMsgToUser SendMsgToUser;        // 原 3050  SendMsgToUser: TNormNpc_SendMsgToUser
    public TNormNpc_MessageBox MessageBox;        // 原 3051  MessageBox: TNormNpc_MessageBox
    public TNormNpc_GetVarValue GetVarValue;        // 原 3052  GetVarValue: TNormNpc_GetVarValue
    public TNormNpc_SetVarValue SetVarValue;        // 原 3053  SetVarValue: TNormNpc_SetVarValue
    public TNormNpc_GetDynamicVarValue GetDynamicVarValue;        // 原 3054  GetDynamicVarValue: TNormNpc_GetDynamicVarValue
    public TNormNpc_SetDynamicVarValue SetDynamicVarValue;        // 原 3055  SetDynamicVarValue: TNormNpc_SetDynamicVarValue
    public fixed long Reserved[100];        // 原 3056  Reserved: array of 99 x100
}

/// <summary>原文 TUserEngineFunc（PluginInterface.pas:3059-3114,54 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserEngineFunc
{
    public TUserEngine_GetPlayerList GetPlayerList;        // 原 3060  GetPlayerList: TUserEngine_GetPlayerList
    public TUserEngine_GetPlayerByName GetPlayerByName;        // 原 3061  GetPlayerByName: TUserEngine_GetPlayerByName
    public TUserEngine_GetPlayerByUserID GetPlayerByUserID;        // 原 3062  GetPlayerByUserID: TUserEngine_GetPlayerByUserID
    public TUserEngine_GetPlayerByObject GetPlayerByObject;        // 原 3063  GetPlayerByObject: TUserEngine_GetPlayerByObject
    public TUserEngine_GetOfflinePlayer GetOfflinePlayer;        // 原 3064  GetOfflinePlayer: TUserEngine_GetOfflinePlayer
    public TUserEngine_KickPlayer KickPlayer;        // 原 3065  KickPlayer: TUserEngine_KickPlayer
    public TUserEngine_GetHeroList GetHeroList;        // 原 3066  GetHeroList: TUserEngine_GetHeroList
    public TUserEngine_GetHeroByName GetHeroByName;        // 原 3067  GetHeroByName: TUserEngine_GetHeroByName
    public TUserEngine_KickHero KickHero;        // 原 3068  KickHero: TUserEngine_KickHero
    public TUserEngine_GetMerchantList GetMerchantList;        // 原 3069  GetMerchantList: TUserEngine_GetMerchantList
    public TUserEngine_GetCustomNpcConfigList GetCustomNpcConfigList;        // 原 3070  GetCustomNpcConfigList: TUserEngine_GetCustomNpcConfigList
    public TUserEngine_GetQuestNPCList GetQuestNPCList;        // 原 3071  GetQuestNPCList: TUserEngine_GetQuestNPCList
    public TUserEngine_GetManageNPC GetManageNPC;        // 原 3072  GetManageNPC: TUserEngine_GetManageNPC
    public TUserEngine_GetFunctionNPC GetFunctionNPC;        // 原 3073  GetFunctionNPC: TUserEngine_GetFunctionNPC
    public TUserEngine_GetRobotNPC GetRobotNPC;        // 原 3074  GetRobotNPC: TUserEngine_GetRobotNPC
    public TUserEngine_MissionNPC MissionNPC;        // 原 3075  MissionNPC: TUserEngine_MissionNPC
    public TUserEngine_FindMerchant FindMerchant;        // 原 3076  FindMerchant: TUserEngine_FindMerchant
    public TUserEngine_FindMerchantByPos FindMerchantByPos;        // 原 3077  FindMerchantByPos: TUserEngine_FindMerchantByPos
    public TUserEngine_FindQuestNPC FindQuestNPC;        // 原 3078  FindQuestNPC: TUserEngine_FindQuestNPC
    public TUserEngine_GetMagicList GetMagicList;        // 原 3079  GetMagicList: TUserEngine_GetMagicList
    public TUserEngine_GetCustomMagicConfigList GetCustomMagicConfigList;        // 原 3080  GetCustomMagicConfigList: TUserEngine_GetCustomMagicConfigList
    public TUserEngine_GetMagicACList GetMagicACList;        // 原 3081  GetMagicACList: TUserEngine_GetMagicACList
    public TUserEngine_FindMagicByName FindMagicByName;        // 原 3082  FindMagicByName: TUserEngine_FindMagicByName
    public TUserEngine_FindMagicByIndex FindMagicByIndex;        // 原 3083  FindMagicByIndex: TUserEngine_FindMagicByIndex
    public TUserEngine_FindMagicByNameEx FindMagicByNameEx;        // 原 3084  FindMagicByNameEx: TUserEngine_FindMagicByNameEx
    public TUserEngine_FindMagicByIndexEx FindMagicByIndexEx;        // 原 3085  FindMagicByIndexEx: TUserEngine_FindMagicByIndexEx
    public TUserEngine_FindHeroMagicByName FindHeroMagicByName;        // 原 3086  FindHeroMagicByName: TUserEngine_FindHeroMagicByName
    public TUserEngine_FindHeroMagicByIndex FindHeroMagicByIndex;        // 原 3087  FindHeroMagicByIndex: TUserEngine_FindHeroMagicByIndex
    public TUserEngine_FindHeroMagicByNameEx FindHeroMagicByNameEx;        // 原 3088  FindHeroMagicByNameEx: TUserEngine_FindHeroMagicByNameEx
    public TUserEngine_FindHeroMagicByIndexEx FindHeroMagicByIndexEx;        // 原 3089  FindHeroMagicByIndexEx: TUserEngine_FindHeroMagicByIndexEx
    public TUserEngine_GetStdItemList GetStdItemList;        // 原 3090  GetStdItemList: TUserEngine_GetStdItemList
    public TUserEngine_GetStdItemByName GetStdItemByName;        // 原 3091  GetStdItemByName: TUserEngine_GetStdItemByName
    public TUserEngine_GetStdItemByIndex GetStdItemByIndex;        // 原 3092  GetStdItemByIndex: TUserEngine_GetStdItemByIndex
    public TUserEngine_GetStdItemName GetStdItemName;        // 原 3093  GetStdItemName: TUserEngine_GetStdItemName
    public TUserEngine_GetStdItemIndex GetStdItemIndex;        // 原 3094  GetStdItemIndex: TUserEngine_GetStdItemIndex
    public TUserEngine_MonsterList MonsterList;        // 原 3095  MonsterList: TUserEngine_MonsterList
    public TUserEngine_SendBroadCastMsg SendBroadCastMsg;        // 原 3096  SendBroadCastMsg: TUserEngine_SendBroadCastMsg
    public TUserEngine_SendBroadCastMsgExt SendBroadCastMsgExt;        // 原 3097  SendBroadCastMsgExt: TUserEngine_SendBroadCastMsgExt
    public TUserEngine_SendTopBroadCastMsg SendTopBroadCastMsg;        // 原 3098  SendTopBroadCastMsg: TUserEngine_SendTopBroadCastMsg
    public TUserEngine_SendMoveMsg SendMoveMsg;        // 原 3099  SendMoveMsg: TUserEngine_SendMoveMsg
    public TUserEngine_SendCenterMsg SendCenterMsg;        // 原 3100  SendCenterMsg: TUserEngine_SendCenterMsg
    public TUserEngine_SendNewLineMsg SendNewLineMsg;        // 原 3101  SendNewLineMsg: TUserEngine_SendNewLineMsg
    public TUserEngine_SendSuperMoveMsg SendSuperMoveMsg;        // 原 3102  SendSuperMoveMsg: TUserEngine_SendSuperMoveMsg
    public TUserEngine_SendSceneShake SendSceneShake;        // 原 3103  SendSceneShake: TUserEngine_SendSceneShake
    public TUserEngine_CopyToUserItemFromName CopyToUserItemFromName;        // 原 3104  CopyToUserItemFromName: TUserEngine_CopyToUserItemFromName
    public TUserEngine_CopyToUserItemFromItem CopyToUserItemFromItem;        // 原 3105  CopyToUserItemFromItem: TUserEngine_CopyToUserItemFromItem
    public TUserEngine_RandomUpgradeItem RandomUpgradeItem;        // 原 3106  RandomUpgradeItem: TUserEngine_RandomUpgradeItem
    public TUserEngine_RandomItemNewAbil RandomItemNewAbil;        // 原 3107  RandomItemNewAbil: TUserEngine_RandomItemNewAbil
    public TUserEngine_GetUnknowItemValue GetUnknowItemValue;        // 原 3108  GetUnknowItemValue: TUserEngine_GetUnknowItemValue
    public TUserEngine_GetAllDummyCount GetAllDummyCount;        // 原 3109  GetAllDummyCount: TUserEngine_GetAllDummyCount
    public TUserEngine_GetMapDummyCount GetMapDummyCount;        // 原 3110  GetMapDummyCount: TUserEngine_GetMapDummyCount
    public TUserEngine_GetOfflineCount GetOfflineCount;        // 原 3111  GetOfflineCount: TUserEngine_GetOfflineCount
    public TUserEngine_GetRealPlayerCount GetRealPlayerCount;        // 原 3112  GetRealPlayerCount: TUserEngine_GetRealPlayerCount
    public fixed long Reserved[100];        // 原 3113  Reserved: array of 99 x100
}

/// <summary>原文 TGuildManagerFunc（PluginInterface.pas:3116-3122,5 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGuildManagerFunc
{
    public TGuildManager_FindGuild FindGuild;        // 原 3117  FindGuild: TGuildManager_FindGuild
    public TGuildManager_GetPlayerGuild GetPlayerGuild;        // 原 3118  GetPlayerGuild: TGuildManager_GetPlayerGuild
    public TGuildManager_AddGuild AddGuild;        // 原 3119  AddGuild: TGuildManager_AddGuild
    public TGuildManager_DelGuild DelGuild;        // 原 3120  DelGuild: TGuildManager_DelGuild
    public fixed long Reserved[100];        // 原 3121  Reserved: array of 99 x100
}

/// <summary>原文 TGuildFunc（PluginInterface.pas:3124-3157,32 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGuildFunc
{
    public TGuild_GetGuildName GetGuildName;        // 原 3125  GetGuildName: TGuild_GetGuildName
    public TGuild_GetJoinJob GetJoinJob;        // 原 3126  GetJoinJob: TGuild_GetJoinJob
    public TGuild_GetJoinLevel GetJoinLevel;        // 原 3127  GetJoinLevel: TGuild_GetJoinLevel
    public TGuild_GetJoinMsg GetJoinMsg;        // 原 3128  GetJoinMsg: TGuild_GetJoinMsg
    public TGuild_GetBuildPoint GetBuildPoint;        // 原 3129  GetBuildPoint: TGuild_GetBuildPoint
    public TGuild_GetAurae GetAurae;        // 原 3130  GetAurae: TGuild_GetAurae
    public TGuild_GetStability GetStability;        // 原 3131  GetStability: TGuild_GetStability
    public TGuild_GetFlourishing GetFlourishing;        // 原 3132  GetFlourishing: TGuild_GetFlourishing
    public TGuild_GetChiefItemCount GetChiefItemCount;        // 原 3133  GetChiefItemCount: TGuild_GetChiefItemCount
    public TGuild_GetMemberCount GetMemberCount;        // 原 3134  GetMemberCount: TGuild_GetMemberCount
    public TGuild_GetOnlineMemeberCount GetOnlineMemeberCount;        // 原 3135  GetOnlineMemeberCount: TGuild_GetOnlineMemeberCount
    public TGuild_GetMasterCount GetMasterCount;        // 原 3136  GetMasterCount: TGuild_GetMasterCount
    public TGuild_GetMaster GetMaster;        // 原 3137  GetMaster: TGuild_GetMaster
    public TGuild_GetMasterName GetMasterName;        // 原 3138  GetMasterName: TGuild_GetMasterName
    public TGuild_CheckMemberIsFull CheckMemberIsFull;        // 原 3139  CheckMemberIsFull: TGuild_CheckMemberIsFull
    public TGuild_IsMemeber IsMemeber;        // 原 3140  IsMemeber: TGuild_IsMemeber
    public TGuild_AddMember AddMember;        // 原 3141  AddMember: TGuild_AddMember
    public TGuild_AddMemberEx AddMemberEx;        // 原 3142  AddMemberEx: TGuild_AddMemberEx
    public TGuild_DelMemeber DelMemeber;        // 原 3143  DelMemeber: TGuild_DelMemeber
    public TGuild_DelMemeberEx DelMemeberEx;        // 原 3144  DelMemeberEx: TGuild_DelMemeberEx
    public TGuild_IsAllianceGuild IsAllianceGuild;        // 原 3145  IsAllianceGuild: TGuild_IsAllianceGuild
    public TGuild_IsWarGuild IsWarGuild;        // 原 3146  IsWarGuild: TGuild_IsWarGuild
    public TGuild_IsAttentionGuild IsAttentionGuild;        // 原 3147  IsAttentionGuild: TGuild_IsAttentionGuild
    public TGuild_AddAlliance AddAlliance;        // 原 3148  AddAlliance: TGuild_AddAlliance
    public TGuild_AddWarGuild AddWarGuild;        // 原 3149  AddWarGuild: TGuild_AddWarGuild
    public TGuild_AddAttentionGuild AddAttentionGuild;        // 原 3150  AddAttentionGuild: TGuild_AddAttentionGuild
    public TGuild_DelAllianceGuild DelAllianceGuild;        // 原 3151  DelAllianceGuild: TGuild_DelAllianceGuild
    public TGuild_DelAttentionGuild DelAttentionGuild;        // 原 3152  DelAttentionGuild: TGuild_DelAttentionGuild
    public TGuild_GetRandNameByName GetRandNameByName;        // 原 3153  GetRandNameByName: TGuild_GetRandNameByName
    public TGuild_GetRandNameByPlayer GetRandNameByPlayer;        // 原 3154  GetRandNameByPlayer: TGuild_GetRandNameByPlayer
    public TGuild_SendGuildMsg SendGuildMsg;        // 原 3155  SendGuildMsg: TGuild_SendGuildMsg
    public fixed long Reserved[100];        // 原 3156  Reserved: array of 99 x100
}

/// <summary>原文 TAppFuncDef（PluginInterface.pas:3162-3184,21 fields）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAppFuncDef
{
    public IntPtr PluginID;        // 原 3163  PluginID: NativeInt
    public TMemoryFunc Memory;        // 原 3164  Memory: TMemoryFunc
    public TListFunc List;        // 原 3165  List: TListFunc
    public TStringListFunc StringList;        // 原 3166  StringList: TStringListFunc
    public TMemoryStreamFunc MemStream;        // 原 3167  MemStream: TMemoryStreamFunc
    public TMemuFunc Menu;        // 原 3168  Menu: TMemuFunc
    public TIniFileFunc IniFile;        // 原 3169  IniFile: TIniFileFunc
    public TMagicACListFunc MagicACList;        // 原 3170  MagicACList: TMagicACListFunc
    public TMapManagerFunc MapManager;        // 原 3171  MapManager: TMapManagerFunc
    public TEnvirnomentFunc Envir;        // 原 3172  Envir: TEnvirnomentFunc
    public TM2EngineFunc M2Engine;        // 原 3173  M2Engine: TM2EngineFunc
    public TBaseObjectFunc BaseObject;        // 原 3174  BaseObject: TBaseObjectFunc
    public TSmartObjectFunc Smarter;        // 原 3175  Smarter: TSmartObjectFunc
    public TPlayObjectFunc Player;        // 原 3176  Player: TPlayObjectFunc
    public TDummyObjectFunc Dummy;        // 原 3177  Dummy: TDummyObjectFunc
    public THeroObjectFunc Hero;        // 原 3178  Hero: THeroObjectFunc
    public TNormNpcFunc Npc;        // 原 3179  Npc: TNormNpcFunc
    public TUserEngineFunc UserEngine;        // 原 3180  UserEngine: TUserEngineFunc
    public TGuildManagerFunc GuildManager;        // 原 3181  GuildManager: TGuildManagerFunc
    public TGuildFunc Guild;        // 原 3182  Guild: TGuildFunc
    public fixed long Reserved[1000];        // 原 3183  Reserved: array of 999 x1000
}

