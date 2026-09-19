// =====================================================================================
// PluginInterface.pas 1:1 转换（Source/M2Engine/PluginInterface.pas，3304 行；本文件含 736 个 procedural type）。
//
// 原文是给"原生 DLL 插件"用的 C 风格 stdcall 函数指针表。托管侧保留两套表达：
//   1) ABI 层（本文件 + PluginInterfaceTables.g.cs + PluginInterfaceHost.cs）：
//      [UnmanagedFunctionPointer(StdCall)] 委托 + Pack=1 结构，字段顺序与 Reserved
//      占位与原文逐字一致，可与原版原生 DLL 对照/互操作。
//   2) 托管层（PluginInterfaceManaged.cs）：I* 接口，成员与原文函数指针一一对应。
//
// 类型映射：BOOL→int（Windows.BOOL 为 4 字节，约定 0/1）、DWORD→uint、Word→ushort、
//   Byte→byte、ShortInt→sbyte、SmallInt→short、Integer→int、Int64→long、
//   NativeInt/Pointer/THandle→IntPtr、Real→double、PAnsiChar→byte[]（GBK 文本缓冲，
//   `Dest: PAnsiChar; var DestLen: DWORD` 为出参缓冲）、pT*→ref T*（packed 指针）、
//   `_T*` 是原文暴露给外部插件的别名（_TList = TList …），托管侧映射为接缝接口。
//
// 逐条来源与行号见 PluginInterface.manifest.tsv（脚本抽取 + 测试回读比对）。
// =====================================================================================

using System;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;

namespace GXX.M2Server.Plugins;

/// <summary>原文 `TNotifyEventEx = procedure(Sender: _TObject); stdcall`（PluginInterface.pas:114）；该类型不是 `X = function(...)` 形式，故不在上面 736 条之列，手工补齐。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNotifyEventEx(object? Sender);

/// <summary>原文 `TMemory_Alloc = function(Size: Integer): Pointer; stdcall;`（PluginInterface.pas:122）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TMemory_Alloc(int pSize);

/// <summary>原文 `TMemory_Free = procedure(P: Pointer); stdcall;`（PluginInterface.pas:125）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemory_Free(IntPtr P);

/// <summary>原文 `TMemory_Realloc = procedure(P: Pointer; Size: Integer); stdcall;`（PluginInterface.pas:128）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemory_Realloc(IntPtr P, int pSize);

/// <summary>原文 `TList_Create = function(): _TList; stdcall;`（PluginInterface.pas:136）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TList_Create();

/// <summary>原文 `TList_Free = procedure(List: _TList); stdcall;`（PluginInterface.pas:139）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Free(IListHandle pList);

/// <summary>原文 `TList_Count = function(List: _TList): Integer; stdcall;`（PluginInterface.pas:142）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TList_Count(IListHandle pList);

/// <summary>原文 `TList_Clear = procedure(List: _TList); stdcall;`（PluginInterface.pas:145）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Clear(IListHandle pList);

/// <summary>原文 `TList_Add = procedure(List: _TList; Item: Pointer); stdcall;`（PluginInterface.pas:148）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Add(IListHandle pList, IntPtr pItem);

/// <summary>原文 `TList_Insert = procedure(List: _TList; Index: Integer; Item: Pointer); stdcall;`（PluginInterface.pas:151）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Insert(IListHandle pList, int pIndex, IntPtr pItem);

/// <summary>原文 `TList_Remove = procedure(List: _TList; Item: Pointer); stdcall;`（PluginInterface.pas:154）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Remove(IListHandle pList, IntPtr pItem);

/// <summary>原文 `TList_Delete = procedure(List: _TList; Index: Integer); stdcall;`（PluginInterface.pas:157）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Delete(IListHandle pList, int pIndex);

/// <summary>原文 `TList_GetItem = function(List: _TList; Index: Integer): Pointer; stdcall;`（PluginInterface.pas:160）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TList_GetItem(IListHandle pList, int pIndex);

/// <summary>原文 `TList_SetItem = procedure(List: _TList; Index: Integer; Item: Pointer); stdcall;`（PluginInterface.pas:163）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_SetItem(IListHandle pList, int pIndex, IntPtr pItem);

/// <summary>原文 `TList_IndexOf = function(List: _TList; Item: Pointer): Integer; stdcall;`（PluginInterface.pas:166）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TList_IndexOf(IListHandle pList, IntPtr pItem);

/// <summary>原文 `TList_Exchange = procedure(List: _TList; Index1, Index2: Integer); stdcall;`（PluginInterface.pas:169）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Exchange(IListHandle pList, int Index1, int Index2);

/// <summary>原文 `TList_CopyTo = procedure(Source, Dest: _TList); stdcall;`（PluginInterface.pas:172）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_CopyTo(IListHandle Source, IListHandle Dest);

/// <summary>原文 `TStrList_Create = function(): _TStringList; stdcall;`（PluginInterface.pas:180）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IStringListHandle TStrList_Create();

/// <summary>原文 `TStrList_Free = procedure(Strings: _TStringList); stdcall;`（PluginInterface.pas:183）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_Free(IStringListHandle Strings);

/// <summary>原文 `TStrList_GetCaseSensitive = function(Strings: _TStringList): BOOL; stdcall;`（PluginInterface.pas:186）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_GetCaseSensitive(IStringListHandle Strings);

/// <summary>原文 `TStrList_SetCaseSensitive = procedure(Strings: _TStringList; IsCaseSensitive: BOOL); stdcall;`（PluginInterface.pas:189）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_SetCaseSensitive(IStringListHandle Strings, int IsCaseSensitive);

/// <summary>原文 `TStrList_GetSorted = function(Strings: _TStringList): BOOL; stdcall;`（PluginInterface.pas:192）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_GetSorted(IStringListHandle Strings);

/// <summary>原文 `TStrList_SetSorted = procedure(Strings: _TStringList; Sorted: BOOL); stdcall;`（PluginInterface.pas:195）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_SetSorted(IStringListHandle Strings, int Sorted);

/// <summary>原文 `TStrList_GetDuplicates = function(Strings: _TStringList): BOOL; stdcall;`（PluginInterface.pas:198）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_GetDuplicates(IStringListHandle Strings);

/// <summary>原文 `TStrList_SetDuplicates = procedure(Strings: _TStringList; Duplicates: BOOL); stdcall;`（PluginInterface.pas:201）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_SetDuplicates(IStringListHandle Strings, int Duplicates);

/// <summary>原文 `TStrList_Count = function(Strings: _TStringList): Integer; stdcall;`（PluginInterface.pas:204）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_Count(IStringListHandle Strings);

/// <summary>原文 `TStrList_GetText = function(Strings: _TStringList; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:207）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_GetText(IStringListHandle Strings, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TStrList_SetText = procedure(Strings: _TStringList; Src: PAnsiChar; SrcLen: DWORD); stdcall;`（PluginInterface.pas:210）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_SetText(IStringListHandle Strings, byte[] Src, uint SrcLen);

/// <summary>原文 `TStrList_Add = procedure(Strings: _TStringList; S: PAnsiChar); stdcall;`（PluginInterface.pas:213）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_Add(IStringListHandle Strings, byte[] S);

/// <summary>原文 `TStrList_AddObject = procedure(Strings: _TStringList; S: PAnsiChar; AObject: _TObject); stdcall;`（PluginInterface.pas:216）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_AddObject(IStringListHandle Strings, byte[] S, object AObject);

/// <summary>原文 `TStrList_Insert = procedure(Strings: _TStringList; Index: Integer; S: PAnsiChar); stdcall;`（PluginInterface.pas:219）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_Insert(IStringListHandle Strings, int pIndex, byte[] S);

/// <summary>原文 `TStrList_InsertObject = procedure(Strings: _TStringList; Index: Integer; S: PAnsiChar; AObject: _TObject); stdcall;`（PluginInterface.pas:222）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_InsertObject(IStringListHandle Strings, int pIndex, byte[] S, object AObject);

/// <summary>原文 `TStrList_Remove = procedure(Strings: _TStringList; S: PAnsiChar); stdcall;`（PluginInterface.pas:225）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_Remove(IStringListHandle Strings, byte[] S);

/// <summary>原文 `TStrList_Delete = procedure(Strings: _TStringList; Index: Integer); stdcall;`（PluginInterface.pas:228）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_Delete(IStringListHandle Strings, int pIndex);

/// <summary>原文 `TStrList_GetItem = function(Strings: _TStringList; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:231）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_GetItem(IStringListHandle Strings, int pIndex, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TStrList_SetItem = procedure(Strings: _TStringList; Index: Integer; S: PAnsiChar); stdcall;`（PluginInterface.pas:234）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_SetItem(IStringListHandle Strings, int pIndex, byte[] S);

/// <summary>原文 `TStrList_GetObject = function(Strings: _TStringList; Index: Integer): _TObject; stdcall;`（PluginInterface.pas:237）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate object TStrList_GetObject(IStringListHandle Strings, int pIndex);

/// <summary>原文 `TStrList_SetObject = procedure(Strings: _TStringList; Index: Integer; AObject: _TObject); stdcall;`（PluginInterface.pas:240）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_SetObject(IStringListHandle Strings, int pIndex, object AObject);

/// <summary>原文 `TStrList_IndexOf = function(Strings: _TStringList; S: PAnsiChar): Integer; stdcall;`（PluginInterface.pas:243）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_IndexOf(IStringListHandle Strings, byte[] S);

/// <summary>原文 `TStrList_IndexOfObject = function(Strings: _TStringList; AObject: _TObject): Integer; stdcall;`（PluginInterface.pas:246）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_IndexOfObject(IStringListHandle Strings, object AObject);

/// <summary>原文 `TStrList_Find = function(Strings: _TStringList; S: PAnsiChar; var Index: Integer): BOOL; stdcall;`（PluginInterface.pas:249）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TStrList_Find(IStringListHandle Strings, byte[] S, ref int pIndex);

/// <summary>原文 `TStrList_Exchange = procedure(Strings: _TStringList; Index1, Index2: Integer); stdcall;`（PluginInterface.pas:252）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_Exchange(IStringListHandle Strings, int Index1, int Index2);

/// <summary>原文 `TStrLit_LoadFromFile = procedure(Strings: _TStringList; FileName: PAnsiChar); stdcall;`（PluginInterface.pas:255）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrLit_LoadFromFile(IStringListHandle Strings, byte[] FileName);

/// <summary>原文 `TStrLit_SaveToFile = procedure(Strings: _TStringList; FileName: PAnsiChar); stdcall;`（PluginInterface.pas:258）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrLit_SaveToFile(IStringListHandle Strings, byte[] FileName);

/// <summary>原文 `TStrList_CopyTo = procedure(Source, Dest: _TStringList); stdcall;`（PluginInterface.pas:261）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TStrList_CopyTo(IStringListHandle Source, IStringListHandle Dest);

/// <summary>原文 `TMemStream_Create = function(): _TMemoryStream; stdcall;`（PluginInterface.pas:269）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMemoryStreamHandle TMemStream_Create();

/// <summary>原文 `TMemStream_Free = procedure(Stream: _TMemoryStream); stdcall;`（PluginInterface.pas:272）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemStream_Free(IMemoryStreamHandle pStream);

/// <summary>原文 `TMemStream_GetSize = function(Stream: _TMemoryStream): Int64; stdcall;`（PluginInterface.pas:275）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate long TMemStream_GetSize(IMemoryStreamHandle pStream);

/// <summary>原文 `TMemStream_SetSize = procedure(Stream: _TMemoryStream; NewSize: Integer); stdcall;`（PluginInterface.pas:278）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemStream_SetSize(IMemoryStreamHandle pStream, int NewSize);

/// <summary>原文 `TMemStream_Clear = procedure(Stream: _TMemoryStream); stdcall;`（PluginInterface.pas:281）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemStream_Clear(IMemoryStreamHandle pStream);

/// <summary>原文 `TMemStream_Read = function(Stream: _TMemoryStream; Buffer: PAnsiChar; Count: Integer): Integer; stdcall;`（PluginInterface.pas:284）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMemStream_Read(IMemoryStreamHandle pStream, byte[] Buffer, int pCount);

/// <summary>原文 `TMemStream_Write = function(Stream: _TMemoryStream; Buffer: PAnsiChar; Count: Integer): Integer; stdcall;`（PluginInterface.pas:287）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMemStream_Write(IMemoryStreamHandle pStream, byte[] Buffer, int pCount);

/// <summary>原文 `TMemStream_Seek = function(Stream: _TMemoryStream; Offset: Integer; Origin: Word): Integer; stdcall;`（PluginInterface.pas:290）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMemStream_Seek(IMemoryStreamHandle pStream, int Offset, ushort Origin);

/// <summary>原文 `TMemStream_Memory = function(Stream: _TMemoryStream): Pointer; stdcall;`（PluginInterface.pas:293）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TMemStream_Memory(IMemoryStreamHandle pStream);

/// <summary>原文 `TMemStream_GetPosition = function(Stream: _TMemoryStream): Int64; stdcall;`（PluginInterface.pas:296）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate long TMemStream_GetPosition(IMemoryStreamHandle pStream);

/// <summary>原文 `TMemStream_SetPosition = procedure(Stream: _TMemoryStream; Position: Int64); stdcall;`（PluginInterface.pas:299）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemStream_SetPosition(IMemoryStreamHandle pStream, long Position);

/// <summary>原文 `TMemStream_LoadFromFile = procedure(Stream: _TMemoryStream; FileName: PAnsiChar); stdcall;`（PluginInterface.pas:302）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemStream_LoadFromFile(IMemoryStreamHandle pStream, byte[] FileName);

/// <summary>原文 `TMemStream_SaveToFile = procedure(Stream: _TMemoryStream; FileName: PAnsiChar); stdcall;`（PluginInterface.pas:305）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMemStream_SaveToFile(IMemoryStreamHandle pStream, byte[] FileName);

/// <summary>原文 `TMenu_GetMainMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:313）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetMainMenu();

/// <summary>原文 `TMenu_GetControlMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:316）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetControlMenu();

/// <summary>原文 `TMenu_GetViewMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:319）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetViewMenu();

/// <summary>原文 `TMenu_GetOptionMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:322）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetOptionMenu();

/// <summary>原文 `TMenu_GetManagerMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:325）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetManagerMenu();

/// <summary>原文 `TMenu_GetToolsMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:328）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetToolsMenu();

/// <summary>原文 `TMenu_GetHelpMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:331）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetHelpMenu();

/// <summary>原文 `TMenu_GetPluginMenu = function(): _TMenuItem; stdcall;`（PluginInterface.pas:334）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetPluginMenu();

/// <summary>原文 `TMenu_Count = function(MenuItem: _TMenuItem): Integer; stdcall;`（PluginInterface.pas:337）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_Count(IMenuItem MenuItem);

/// <summary>原文 `TMenu_GetItems = function(MenuItem: _TMenuItem; Index: Integer): _TMenuItem; stdcall;`（PluginInterface.pas:340）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetItems(IMenuItem MenuItem, int pIndex);

/// <summary>原文 `TMenu_Add = function(PlugID: NativeInt; MenuItem: _TMenuItem; Caption: PAnsiChar; Tag: Integer; OnClick: TNotifyEventEx) : _TMenuItem; stdcall;`（PluginInterface.pas:343）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_Add(IntPtr PlugID, IMenuItem MenuItem, byte[] Caption, int Tag, TNotifyEventEx OnClick);

/// <summary>原文 `TMenu_Insert = function(PlugID: NativeInt; MenuItem: _TMenuItem; Index: Integer; Caption: PAnsiChar; Tag: Integer; OnClick: TNotifyEventEx): _TMenuItem; stdcall;`（PluginInterface.pas:347）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_Insert(IntPtr PlugID, IMenuItem MenuItem, int pIndex, byte[] Caption, int Tag, TNotifyEventEx OnClick);

/// <summary>原文 `TMenu_GetCaption = function(MenuItem: _TMenuItem; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:351）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetCaption(IMenuItem MenuItem, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TMenu_SetCaption = procedure(MenuItem: _TMenuItem; Caption: PAnsiChar); stdcall;`（PluginInterface.pas:354）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetCaption(IMenuItem MenuItem, byte[] Caption);

/// <summary>原文 `TMenu_GetEnabled = function(MenuItem: _TMenuItem): BOOL; stdcall;`（PluginInterface.pas:357）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetEnabled(IMenuItem MenuItem);

/// <summary>原文 `TMenu_SetEnabled = procedure(MenuItem: _TMenuItem; Enabled: BOOL); stdcall;`（PluginInterface.pas:360）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetEnabled(IMenuItem MenuItem, int Enabled);

/// <summary>原文 `TMenu_GetVisable = function(MenuItem: _TMenuItem): BOOL; stdcall;`（PluginInterface.pas:363）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetVisable(IMenuItem MenuItem);

/// <summary>原文 `TMenu_SetVisable = procedure(MenuItem: _TMenuItem; Visible: BOOL); stdcall;`（PluginInterface.pas:366）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetVisable(IMenuItem MenuItem, int Visible);

/// <summary>原文 `TMenu_GetChecked = function(MenuItem: _TMenuItem): BOOL; stdcall;`（PluginInterface.pas:369）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetChecked(IMenuItem MenuItem);

/// <summary>原文 `TMenu_SetChecked = procedure(MenuItem: _TMenuItem; Checked: BOOL); stdcall;`（PluginInterface.pas:372）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetChecked(IMenuItem MenuItem, int pChecked);

/// <summary>原文 `TMenu_GetRadioItem = function(MenuItem: _TMenuItem): BOOL; stdcall;`（PluginInterface.pas:375）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetRadioItem(IMenuItem MenuItem);

/// <summary>原文 `TMenu_SetRadioItem = procedure(MenuItem: _TMenuItem; IsRadioItem: BOOL); stdcall;`（PluginInterface.pas:378）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetRadioItem(IMenuItem MenuItem, int IsRadioItem);

/// <summary>原文 `TMenu_GetGroupIndex = function(MenuItem: _TMenuItem): Integer; stdcall;`（PluginInterface.pas:381）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetGroupIndex(IMenuItem MenuItem);

/// <summary>原文 `TMenu_SetGroupIndex = procedure(MenuItem: _TMenuItem; Value: Integer); stdcall;`（PluginInterface.pas:384）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetGroupIndex(IMenuItem MenuItem, int pValue);

/// <summary>原文 `TMenu_GetTag = function(MenuItem: _TMenuItem): Integer; stdcall;`（PluginInterface.pas:387）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetTag(IMenuItem MenuItem);

/// <summary>原文 `TMenu_SetTag = procedure(MenuItem: _TMenuItem; Value: Integer); stdcall;`（PluginInterface.pas:390）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetTag(IMenuItem MenuItem, int pValue);

/// <summary>原文 `TIniFile_Create = function(sFileName: PAnsiChar): _TIniFile; stdcall;`（PluginInterface.pas:398）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IIniFileHandle TIniFile_Create(byte[] sFileName);

/// <summary>原文 `TIniFile_Free = procedure(IniFile: _TIniFile); stdcall;`（PluginInterface.pas:401）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TIniFile_Free(IIniFileHandle IniFile);

/// <summary>原文 `TIniFile_SectionExists = function(IniFile: _TIniFile; Section: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:404）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TIniFile_SectionExists(IIniFileHandle IniFile, byte[] Section);

/// <summary>原文 `TIniFile_ValueExists = function(IniFile: _TIniFile; Section, Ident: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:407）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TIniFile_ValueExists(IIniFileHandle IniFile, byte[] Section, byte[] Ident);

/// <summary>原文 `TIniFile_ReadString = function(IniFile: _TIniFile; Section, Ident, Default: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD) : BOOL; stdcall;`（PluginInterface.pas:410）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TIniFile_ReadString(IIniFileHandle IniFile, byte[] Section, byte[] Ident, byte[] Default, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TIniFile_WriteString = procedure(IniFile: _TIniFile; Section, Ident, Value: PAnsiChar); stdcall;`（PluginInterface.pas:414）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TIniFile_WriteString(IIniFileHandle IniFile, byte[] Section, byte[] Ident, byte[] pValue);

/// <summary>原文 `TIniFile_ReadInteger = function(IniFile: _TIniFile; Section, Ident: PAnsiChar; Default: Integer): Integer; stdcall;`（PluginInterface.pas:417）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TIniFile_ReadInteger(IIniFileHandle IniFile, byte[] Section, byte[] Ident, int Default);

/// <summary>原文 `TIniFile_WriteInteger = procedure(IniFile: _TIniFile; Section, Ident: PAnsiChar; Value: Integer); stdcall;`（PluginInterface.pas:420）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TIniFile_WriteInteger(IIniFileHandle IniFile, byte[] Section, byte[] Ident, int pValue);

/// <summary>原文 `TIniFile_ReadBool = function(IniFile: _TIniFile; Section, Ident: PAnsiChar; Default: BOOL): BOOL; stdcall;`（PluginInterface.pas:423）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TIniFile_ReadBool(IIniFileHandle IniFile, byte[] Section, byte[] Ident, int Default);

/// <summary>原文 `TIniFile_WriteBool = procedure(IniFile: _TIniFile; Section, Ident: PAnsiChar; Value: BOOL); stdcall;`（PluginInterface.pas:426）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TIniFile_WriteBool(IIniFileHandle IniFile, byte[] Section, byte[] Ident, int pValue);

/// <summary>原文 `TMagicACList_Count = function(List: _TMagicACList): Integer; stdcall;`（PluginInterface.pas:434）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMagicACList_Count(IMagicACListHandle pList);

/// <summary>原文 `TMagicACList_GetItem = function(List: _TMagicACList; Index: Integer): PMagicACInfo; stdcall;`（PluginInterface.pas:437）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TMagicACList_GetItem(IMagicACListHandle pList, int pIndex);

/// <summary>原文 `TMagicACList_FindByMagIdx = function(List: _TMagicACList; MagIdx: Integer): PMagicACInfo; stdcall;`（PluginInterface.pas:440）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TMagicACList_FindByMagIdx(IMagicACListHandle pList, int MagIdx);

/// <summary>原文 `TMapManager_FindMap = function(MapName: PAnsiChar): _TEnvirnoment; stdcall;`（PluginInterface.pas:448）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IEnvirnoment TMapManager_FindMap(byte[] MapName);

/// <summary>原文 `TMapManager_GetMapList = function(): _TList; stdcall;`（PluginInterface.pas:451）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TMapManager_GetMapList();

/// <summary>原文 `TEnvir_GetMapName = function(Envir: _TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:459）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMapName(IEnvirnoment Envir, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TEnvir_GetMapDesc = function(Envir: _TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:462）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMapDesc(IEnvirnoment Envir, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TEnvir_GetWidth = function(Envir: _TEnvirnoment): Integer; stdcall;`（PluginInterface.pas:465）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetWidth(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetHeight = function(Envir: _TEnvirnoment): Integer; stdcall;`（PluginInterface.pas:468）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetHeight(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetMinMap = function(Envir: _TEnvirnoment): Integer; stdcall;`（PluginInterface.pas:471）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMinMap(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_IsMainMap = function(Envir: _TEnvirnoment): BOOL; stdcall;`（PluginInterface.pas:474）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_IsMainMap(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetMainMapName = function(Envir: _TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:477）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMainMapName(IEnvirnoment Envir, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TEnvir_IsMirrMap = function(Envir: _TEnvirnoment): BOOL; stdcall;`（PluginInterface.pas:480）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_IsMirrMap(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetMirrMapCreateTick = function(Envir: _TEnvirnoment): DWORD; stdcall;`（PluginInterface.pas:483）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TEnvir_GetMirrMapCreateTick(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetMirrMapSurvivalTime = function(Envir: _TEnvirnoment): DWORD; stdcall;`（PluginInterface.pas:486）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TEnvir_GetMirrMapSurvivalTime(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetMirrMapExitToMap = function(Envir: _TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:489）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMirrMapExitToMap(IEnvirnoment Envir, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TEnvir_GetMirrMapMinMap = function(Envir: _TEnvirnoment): Integer; stdcall;`（PluginInterface.pas:492）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMirrMapMinMap(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetAlwaysShowTime = function(Envir: _TEnvirnoment): BOOL; stdcall;`（PluginInterface.pas:495）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetAlwaysShowTime(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_IsFBMap = function(Envir: _TEnvirnoment): BOOL; stdcall;`（PluginInterface.pas:498）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_IsFBMap(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetFBMapName = function(Envir: _TEnvirnoment; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:501）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetFBMapName(IEnvirnoment Envir, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TEnvir_GetFBEnterLimit = function(Envir: _TEnvirnoment): Integer; stdcall;`（PluginInterface.pas:504）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetFBEnterLimit(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetFBCreated = function(Envir: _TEnvirnoment): BOOL; stdcall;`（PluginInterface.pas:507）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetFBCreated(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetFBCreateTime = function(Envir: _TEnvirnoment): DWORD; stdcall;`（PluginInterface.pas:510）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TEnvir_GetFBCreateTime(IEnvirnoment Envir);

/// <summary>原文 `TEnvir_GetMapParam = function(Envir: _TEnvirnoment; Param: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:515）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMapParam(IEnvirnoment Envir, byte[] Param);

/// <summary>原文 `TEnvir_GetMapParamValue = function(Envir: _TEnvirnoment; Param: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD) : BOOL; stdcall;`（PluginInterface.pas:519）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetMapParamValue(IEnvirnoment Envir, byte[] Param, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TEnvir_CheckCanMove = function(Envir: _TEnvirnoment; nX, nY: Integer; boFlag: BOOL): BOOL; stdcall;`（PluginInterface.pas:523）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_CheckCanMove(IEnvirnoment Envir, int nX, int nY, int boFlag);

/// <summary>原文 `TEnvir_IsValidObject = function(Envir: _TEnvirnoment; nX, nY, nRange: Integer; AObject: _TObject): BOOL; stdcall;`（PluginInterface.pas:526）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_IsValidObject(IEnvirnoment Envir, int nX, int nY, int nRange, object AObject);

/// <summary>原文 `TEnvir_GetItemObjects = function(Envir: _TEnvirnoment; nX, nY: Integer; ObjectList: _TList): Integer; stdcall;`（PluginInterface.pas:529）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetItemObjects(IEnvirnoment Envir, int nX, int nY, IListHandle ObjectList);

/// <summary>原文 `TEnvir_GetBaseObjects = function(Envir: _TEnvirnoment; nX, nY: Integer; IncDeathObject: BOOL; ObjectList: _TList) : Integer; stdcall;`（PluginInterface.pas:532）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetBaseObjects(IEnvirnoment Envir, int nX, int nY, int IncDeathObject, IListHandle ObjectList);

/// <summary>原文 `TEnvir_GetPlayObjects = function(Envir: _TEnvirnoment; nX, nY: Integer; IncDeathObject: BOOL; ObjectList: _TList) : Integer; stdcall;`（PluginInterface.pas:536）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TEnvir_GetPlayObjects(IEnvirnoment Envir, int nX, int nY, int IncDeathObject, IListHandle ObjectList);

/// <summary>原文 `TM2Engine_GetVersion = function(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:545）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetVersion(byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_GetVersionInt = function(): Integer; stdcall;`（PluginInterface.pas:548）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetVersionInt();

/// <summary>原文 `TM2Engine_GetMainFormHandle = function(): THandle; stdcall;`（PluginInterface.pas:551）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TM2Engine_GetMainFormHandle();

/// <summary>原文 `TM2Engine_SetMainFormCaption = procedure(Caption: PAnsiChar); stdcall;`（PluginInterface.pas:554）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TM2Engine_SetMainFormCaption(byte[] Caption);

/// <summary>原文 `TM2Engine_GetAppDir = function(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:557）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetAppDir(byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_GetGlobalIniFile = function(M2IniType: Integer): _TIniFile; stdcall;`（PluginInterface.pas:560）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IIniFileHandle TM2Engine_GetGlobalIniFile(int M2IniType);

/// <summary>原文 `TM2Engine_MainOutMessage = procedure(Msg: PAnsiChar; IsAddTime: BOOL); stdcall;`（PluginInterface.pas:571）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TM2Engine_MainOutMessage(byte[] Msg, int IsAddTime);

/// <summary>原文 `TM2Engine_GetGlobalVarI = function(Index: Integer): Integer; stdcall;`（PluginInterface.pas:574）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetGlobalVarI(int pIndex);

/// <summary>原文 `TM2Engine_SetGlobalVarI = function(Index: Integer; Value: Integer): BOOL; stdcall;`（PluginInterface.pas:577）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_SetGlobalVarI(int pIndex, int pValue);

/// <summary>原文 `TM2Engine_GetGlobalVarG = function(Index: Integer): Integer; stdcall;`（PluginInterface.pas:580）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetGlobalVarG(int pIndex);

/// <summary>原文 `TM2Engine_SetGlobalVarG = function(Index: Integer; Value: Integer): BOOL; stdcall;`（PluginInterface.pas:583）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_SetGlobalVarG(int pIndex, int pValue);

/// <summary>原文 `TM2Engine_GetGlobalVarA = function(Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:586）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetGlobalVarA(int pIndex, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_SetGlobalVarA = function(Index: Integer; Value: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:589）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_SetGlobalVarA(int pIndex, byte[] pValue);

/// <summary>原文 `TM2Engine_EncodeBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:592）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_EncodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_DecodeBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:595）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_DecodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_ZLibEncodeBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:598）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_ZLibEncodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_ZLibDecodeBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:601）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_ZLibDecodeBuffer(byte[] Src, uint SrcLen, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_EncryptBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:604）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_EncryptBuffer(byte[] Src, uint SrcLen, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_DecryptBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:607）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_DecryptBuffer(byte[] Src, uint SrcLen, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TM2Engine_EncryptPassword = function(InData: PAnsiChar; OutData: PAnsiChar; var OutSize: DWORD): BOOL; stdcall;`（PluginInterface.pas:610）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_EncryptPassword(byte[] InData, byte[] OutData, ref uint OutSize);

/// <summary>原文 `TM2Engine_DecryptPassword = function(InData: PAnsiChar; OutData: PAnsiChar; var OutSize: DWORD): BOOL; stdcall;`（PluginInterface.pas:613）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_DecryptPassword(byte[] InData, byte[] OutData, ref uint OutSize);

/// <summary>原文 `TM2Engine_GetTakeOnPosition = function(StdMode: Integer): Integer; stdcall;`（PluginInterface.pas:616）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetTakeOnPosition(int StdMode);

/// <summary>原文 `TM2Engine_CheckBindType = function(BindValue: Byte; BindType: Byte): BOOL; stdcall;`（PluginInterface.pas:619）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_CheckBindType(byte BindValue, byte BindType);

/// <summary>原文 `TM2Engine_SetBindValue = procedure(var BindValue: Byte; BindType: Byte; Value: BOOL); stdcall;`（PluginInterface.pas:622）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TM2Engine_SetBindValue(ref byte BindValue, byte BindType, int pValue);

/// <summary>原文 `TM2Engine_GetRGB = function(Color: Byte): DWORD; stdcall;`（PluginInterface.pas:625）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TM2Engine_GetRGB(byte Color);

/// <summary>原文 `TBaseObject_GetChrName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:633）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetChrName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TBaseObject_SetChrName = function(BaseObject: _TBaseObject; NewName: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:636）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_SetChrName(IBaseObjectHandle BaseObject, byte[] NewName);

/// <summary>原文 `TBaseObject_RefShowName = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:639）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_RefShowName(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_RefNameColor = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:642）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_RefNameColor(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetGender = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:645）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetGender(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetGender = function(BaseObject: _TBaseObject; Gender: Byte): BOOL; stdcall;`（PluginInterface.pas:648）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_SetGender(IBaseObjectHandle BaseObject, byte Gender);

/// <summary>原文 `TBaseObject_GetJob = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:651）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetJob(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetJob = function(BaseObject: _TBaseObject; Job: Byte): BOOL; stdcall;`（PluginInterface.pas:654）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_SetJob(IBaseObjectHandle BaseObject, byte Job);

/// <summary>原文 `TBaseObject_GetHair = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:657）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetHair(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetHair = procedure(BaseObject: _TBaseObject; Hair: Byte); stdcall;`（PluginInterface.pas:660）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetHair(IBaseObjectHandle BaseObject, byte Hair);

/// <summary>原文 `TBaseObject_GetEnvir = function(BaseObject: _TBaseObject): _TEnvirnoment; stdcall;`（PluginInterface.pas:663）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IEnvirnoment TBaseObject_GetEnvir(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetMapName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:666）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetMapName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TBaseObject_GetCurrX = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:669）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetCurrX(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetCurrY = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:672）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetCurrY(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetDirection = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:675）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetDirection(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetHomeMap = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:678）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetHomeMap(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TBaseObject_GetHomeX = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:681）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetHomeX(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetHomeY = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:684）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetHomeY(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetPermission = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:687）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetPermission(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetPermission = procedure(BaseObject: _TBaseObject; Value: Byte); stdcall;`（PluginInterface.pas:690）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetPermission(IBaseObjectHandle BaseObject, byte pValue);

/// <summary>原文 `TBaseObject_GetDeath = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:693）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetDeath(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetDeathTick = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:696）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetDeathTick(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetGhost = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:699）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetGhost(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetGhostTick = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:702）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetGhostTick(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_MakeGhost = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:705）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_MakeGhost(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_ReAlive = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:708）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_ReAlive(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetRaceServer = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:711）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetRaceServer(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetAppr = function(BaseObject: _TBaseObject): Word; stdcall;`（PluginInterface.pas:714）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TBaseObject_GetAppr(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetRaceImg = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:717）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetRaceImg(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetCharStatus = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:720）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetCharStatus(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetCharStatus = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`（PluginInterface.pas:723）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetCharStatus(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_StatusChanged = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:726）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_StatusChanged(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetHungerPoint = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:729）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetHungerPoint(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetHungerPoint = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`（PluginInterface.pas:732）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetHungerPoint(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseobject_IsNGMonster = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:735）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseobject_IsNGMonster(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_IsDummyObject = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:738）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsDummyObject(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetViewRange = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:741）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetViewRange(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetViewRange = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`（PluginInterface.pas:744）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetViewRange(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetAbility = function(BaseObject: _TBaseObject; Dest: pTAbility): BOOL; stdcall;`（PluginInterface.pas:747）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetAbility(IBaseObjectHandle BaseObject, ref TAbility Dest);

/// <summary>原文 `TBaseObject_GetWAbility = function(BaseObject: _TBaseObject; Dest: pTAbility): BOOL; stdcall;`（PluginInterface.pas:750）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetWAbility(IBaseObjectHandle BaseObject, ref TAbility Dest);

/// <summary>原文 `TBaseObject_SetWAbility = procedure(BaseObject: _TBaseObject; Value: pTAbility); stdcall;`（PluginInterface.pas:753）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetWAbility(IBaseObjectHandle BaseObject, ref TAbility pValue);

/// <summary>原文 `TBaseObject_GetSlaveList = function(BaseObject: _TBaseObject): _TList; stdcall;`（PluginInterface.pas:756）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TBaseObject_GetSlaveList(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetMaster = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:759）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetMaster(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetMasterEx = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:762）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetMasterEx(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetSuperManMode = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:765）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetSuperManMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetSuperManMode = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:768）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetSuperManMode(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetAdminMode = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:771）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetAdminMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetAdminMode = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:774）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetAdminMode(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetTransparent = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:777）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetTransparent(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetTransparent = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:780）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetTransparent(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetObMode = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:783）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetObMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetObMode = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:786）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetObMode(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetStoneMode = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:789）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetStoneMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetStoneMode = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:792）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetStoneMode(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetStickMode = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:795）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetStickMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetStickMode = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:798）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetStickMode(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetIsAnimal = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:801）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsAnimal(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetIsAnimal = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:804）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetIsAnimal(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetIsNoItem = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:807）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsNoItem(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetIsNoItem = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:810）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetIsNoItem(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetCoolEye = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:813）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetCoolEye(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetCoolEye = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:816）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetCoolEye(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetHitPoint = function(BaseObject: _TBaseObject): Word; stdcall;`（PluginInterface.pas:819）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TBaseObject_GetHitPoint(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetHitPoint = procedure(BaseObject: _TBaseObject; Value: Word); stdcall;`（PluginInterface.pas:822）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetHitPoint(IBaseObjectHandle BaseObject, ushort pValue);

/// <summary>原文 `TBaseObject_GetSpeedPoint = function(BaseObject: _TBaseObject): Word; stdcall;`（PluginInterface.pas:825）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TBaseObject_GetSpeedPoint(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetSpeedPoint = procedure(BaseObject: _TBaseObject; Value: Word); stdcall;`（PluginInterface.pas:828）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetSpeedPoint(IBaseObjectHandle BaseObject, ushort pValue);

/// <summary>原文 `TBaseObject_GetHitSpeed = function(BaseObject: _TBaseObject): ShortInt; stdcall;`（PluginInterface.pas:831）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate sbyte TBaseObject_GetHitSpeed(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetHitSpeed = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`（PluginInterface.pas:834）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetHitSpeed(IBaseObjectHandle BaseObject, sbyte pValue);

/// <summary>原文 `TBaseObject_GetWalkSpeed = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:837）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetWalkSpeed(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetWalkSpeed = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`（PluginInterface.pas:840）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetWalkSpeed(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetHPRecover = function(BaseObject: _TBaseObject): ShortInt; stdcall;`（PluginInterface.pas:843）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate sbyte TBaseObject_GetHPRecover(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetHPRecover = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`（PluginInterface.pas:846）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetHPRecover(IBaseObjectHandle BaseObject, sbyte pValue);

/// <summary>原文 `TBaseObject_GetMPRecover = function(BaseObject: _TBaseObject): ShortInt; stdcall;`（PluginInterface.pas:849）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate sbyte TBaseObject_GetMPRecover(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetMPRecover = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`（PluginInterface.pas:852）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetMPRecover(IBaseObjectHandle BaseObject, sbyte pValue);

/// <summary>原文 `TBaseObject_GetPoisonRecover = function(BaseObject: _TBaseObject): ShortInt; stdcall;`（PluginInterface.pas:855）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate sbyte TBaseObject_GetPoisonRecover(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetPoisonRecover = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`（PluginInterface.pas:858）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetPoisonRecover(IBaseObjectHandle BaseObject, sbyte pValue);

/// <summary>原文 `TBaseObject_GetAntiPoison = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:861）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetAntiPoison(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetAntiPoison = procedure(BaseObject: _TBaseObject; Value: Byte); stdcall;`（PluginInterface.pas:864）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetAntiPoison(IBaseObjectHandle BaseObject, byte pValue);

/// <summary>原文 `TBaseObject_GetAntiMagic = function(BaseObject: _TBaseObject): ShortInt; stdcall;`（PluginInterface.pas:867）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate sbyte TBaseObject_GetAntiMagic(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetAntiMagic = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`（PluginInterface.pas:870）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetAntiMagic(IBaseObjectHandle BaseObject, sbyte pValue);

/// <summary>原文 `TBaseObject_GetLuck = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:873）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetLuck(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetLuck = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`（PluginInterface.pas:876）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetLuck(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetAttatckMode = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:879）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetAttatckMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetAttatckMode = procedure(BaseObject: _TBaseObject; Value: Byte); stdcall;`（PluginInterface.pas:882）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetAttatckMode(IBaseObjectHandle BaseObject, byte pValue);

/// <summary>原文 `TBaseObject_GetNation = function(BaseObject: _TBaseObject): Byte; stdcall;`（PluginInterface.pas:885）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TBaseObject_GetNation(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetNation = function(BaseObject: _TBaseObject; Nation: Byte): BOOL; stdcall;`（PluginInterface.pas:888）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_SetNation(IBaseObjectHandle BaseObject, byte Nation);

/// <summary>原文 `TBaseObject_GetNationaName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:891）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetNationaName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TBaseObject_GetGuild = function(BaseObject: _TBaseObject): _TGuild; stdcall;`（PluginInterface.pas:894）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IGuildHandle TBaseObject_GetGuild(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseobject_GetGuildRankNo = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:897）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseobject_GetGuildRankNo(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseobject_GetGuildRankName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:900）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseobject_GetGuildRankName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TBaseObject_IsGuildMaster = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:903）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsGuildMaster(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetHideMode = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:906）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetHideMode(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetHideMode = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:908）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetHideMode(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetIsParalysis = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:911）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsParalysis(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetIsParalysis = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:913）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetIsParalysis(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetParalysisRate = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:916）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetParalysisRate(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetParalysisRate = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:918）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetParalysisRate(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsMDParalysis = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:921）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsMDParalysis(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetIsMDParalysis = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:923）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetIsMDParalysis(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetMDParalysisRate = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:926）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetMDParalysisRate(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetMDParalysisRate = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:928）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetMDParalysisRate(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsFrozen = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:931）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsFrozen(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetIsFrozen = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:933）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetIsFrozen(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetFrozenRate = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:936）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetFrozenRate(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetFrozenRate = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:938）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetFrozenRate(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsCobwebWinding = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:941）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsCobwebWinding(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetIsCobwebWinding = procedure(BaseObject: _TBaseObject; Value: BOOL); stdcall;`（PluginInterface.pas:943）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetIsCobwebWinding(IBaseObjectHandle BaseObject, int pValue);

/// <summary>原文 `TBaseObject_GetCobwebWindingRate = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:946）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetCobwebWindingRate(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetCobwebWindingRate = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:948）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetCobwebWindingRate(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetUnParalysisValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:951）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnParalysisValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnParalysisValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:953）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnParalysisValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnParalysis = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:956）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnParalysis(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnMagicShieldValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:959）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnMagicShieldValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnMagicShieldValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:961）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnMagicShieldValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnMagicShield = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:964）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnMagicShield(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnRevivalValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:967）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnRevivalValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnRevivalValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:969）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnRevivalValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnRevival = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:972）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnRevival(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnPosionValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:975）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnPosionValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnPosionValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:977）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnPosionValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnPosion = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:980）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnPosion(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnTammingValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:983）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnTammingValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnTammingValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:985）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnTammingValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnTamming = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:988）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnTamming(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnFireCrossValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:991）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnFireCrossValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnFireCrossValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:993）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnFireCrossValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnFireCross = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:996）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnFireCross(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnFrozenValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:999）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnFrozenValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnFrozenValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:1001）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnFrozenValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnFrozen = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1004）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnFrozen(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetUnCobwebWindingValue = function(BaseObject: _TBaseObject): DWORD; stdcall;`（PluginInterface.pas:1007）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetUnCobwebWindingValue(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetUnCobwebWindingValue = procedure(BaseObject: _TBaseObject; Value: DWORD); stdcall;`（PluginInterface.pas:1009）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetUnCobwebWindingValue(IBaseObjectHandle BaseObject, uint pValue);

/// <summary>原文 `TBaseObject_GetIsUnCobwebWinding = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1012）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetIsUnCobwebWinding(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetTargetCret = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:1015）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetTargetCret(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SetTargetCret = procedure(BaseObject: _TBaseObject; TargetCret: _TBaseObject); stdcall;`（PluginInterface.pas:1018）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SetTargetCret(IBaseObjectHandle BaseObject, IBaseObjectHandle TargetCret);

/// <summary>原文 `TBaseObject_DelTargetCreat = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:1021）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_DelTargetCreat(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetLastHiter = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:1024）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetLastHiter(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetExpHitter = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:1027）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetExpHitter(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetPoisonHitter = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:1030）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetPoisonHitter(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetPoseCreate = function(BaseObject: _TBaseObject): _TBaseObject; stdcall;`（PluginInterface.pas:1033）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IBaseObjectHandle TBaseObject_GetPoseCreate(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_IsProperTarget = function(BaseObject, Target: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1036）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsProperTarget(IBaseObjectHandle BaseObject, IBaseObjectHandle Target);

/// <summary>原文 `TBaseObject_IsProperFriend = function(BaseObject, Target: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1039）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsProperFriend(IBaseObjectHandle BaseObject, IBaseObjectHandle Target);

/// <summary>原文 `TBaseObject_TargetInRange = function(BaseObject, Target: _TBaseObject; nX, nY, nRange: Integer): BOOL; stdcall;`（PluginInterface.pas:1042）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_TargetInRange(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nX, int nY, int nRange);

/// <summary>原文 `TBaseObject_SendMsg = procedure(BaseObject, Target: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar); stdcall;`（PluginInterface.pas:1045）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SendMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg);

/// <summary>原文 `TBaseObject_SendDelayMsg = procedure(BaseObject, Target: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar; dwDelay: DWORD); stdcall;`（PluginInterface.pas:1049）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SendDelayMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg, uint dwDelay);

/// <summary>原文 `TBaseObject_SendRefMsg = procedure(BaseObject: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar; dwDelay: DWORD); stdcall;`（PluginInterface.pas:1053）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SendRefMsg(IBaseObjectHandle BaseObject, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg, uint dwDelay);

/// <summary>原文 `TBaseObject_SendUpdateMsg = procedure(BaseObject, Target: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar); stdcall;`（PluginInterface.pas:1057）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SendUpdateMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg);

/// <summary>原文 `TBaseObject_SysMsg = function(BaseObject: _TBaseObject; sMsg: PAnsiChar; FColor, BColor: Byte; MsgType: Integer) : BOOL; stdcall;`（PluginInterface.pas:1061）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_SysMsg(IBaseObjectHandle BaseObject, byte[] sMsg, byte FColor, byte BColor, int MsgType);

/// <summary>原文 `TBaseObject_GetBagItemList = function(BaseObject: _TBaseObject): _TList; stdcall;`（PluginInterface.pas:1065）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TBaseObject_GetBagItemList(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_IsEnoughBag = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1068）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsEnoughBag(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_IsEnoughBagEx = function(BaseObject: _TBaseObject; AddCount: Integer): BOOL; stdcall;`（PluginInterface.pas:1071）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsEnoughBagEx(IBaseObjectHandle BaseObject, int AddCount);

/// <summary>原文 `TBaseObject_AddItemToBag = function(BaseObject: _TBaseObject; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:1074）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_AddItemToBag(IBaseObjectHandle BaseObject, ref TUserItem UserItem);

/// <summary>原文 `TBaseObject_DelBagItemByIndex = function(BaseObject: _TBaseObject; Index: Integer): BOOL; stdcall;`（PluginInterface.pas:1077）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_DelBagItemByIndex(IBaseObjectHandle BaseObject, int pIndex);

/// <summary>原文 `TBaseObject_DelBagItemByMakeIdx = function(BaseObject: _TBaseObject; MakeIndex: Integer; ItemName: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:1080）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_DelBagItemByMakeIdx(IBaseObjectHandle BaseObject, int MakeIndex, byte[] ItemName);

/// <summary>原文 `TBaseObject_DelBagItemByUserItem = function(BaseObject: _TBaseObject; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:1083）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_DelBagItemByUserItem(IBaseObjectHandle BaseObject, ref TUserItem UserItem);

/// <summary>原文 `TBaseObject_IsInSafeZone = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1086）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsInSafeZone(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_IsPtInSafeZone = function(BaseObject: _TBaseObject; Envir: _TEnvirnoment; nX, nY: Integer): BOOL; stdcall;`（PluginInterface.pas:1089）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_IsPtInSafeZone(IBaseObjectHandle BaseObject, IEnvirnoment Envir, int nX, int nY);

/// <summary>原文 `TBaseObject_RecalcLevelAbil = procedure(BaseObject: _TBaseObject; IsSysDef: BOOL); stdcall;`（PluginInterface.pas:1092）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_RecalcLevelAbil(IBaseObjectHandle BaseObject, int IsSysDef);

/// <summary>原文 `TBaseObject_RecalcAbil = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:1095）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_RecalcAbil(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_RecalcBagWeight = function(BaseObject: _TBaseObject): Integer; stdcall;`（PluginInterface.pas:1098）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_RecalcBagWeight(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetLevelExp = function(BaseObject: _TBaseObject; nLevel: Integer): DWORD; stdcall;`（PluginInterface.pas:1101）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TBaseObject_GetLevelExp(IBaseObjectHandle BaseObject, int nLevel);

/// <summary>原文 `TBaseObject_HasLevelUp = procedure(BaseObject: _TBaseObject; nLevel: Integer); stdcall;`（PluginInterface.pas:1104）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_HasLevelUp(IBaseObjectHandle BaseObject, int nLevel);

/// <summary>原文 `TBaseObject_CheckMagicLevelup = function(BaseObject: _TBaseObject; UserMagic: pTUserMagic): BOOL; stdcall;`（PluginInterface.pas:1119）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_CheckMagicLevelup(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic);

/// <summary>原文 `TBaseObject_MagicTranPointChanged = procedure(BaseObject: _TBaseObject; UserMagic: pTUserMagic); stdcall;`（PluginInterface.pas:1122）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_MagicTranPointChanged(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic);

/// <summary>原文 `TBaseObject_DamageHealth = procedure(BaseObject: _TBaseObject; nDamage: Integer; StruckFrom: _TBaseObject); stdcall;`（PluginInterface.pas:1125）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_DamageHealth(IBaseObjectHandle BaseObject, int nDamage, IBaseObjectHandle StruckFrom);

/// <summary>原文 `TBaseObject_DamageSpell = procedure(BaseObject: _TBaseObject; nSpellPoint: Integer); stdcall;`（PluginInterface.pas:1128）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_DamageSpell(IBaseObjectHandle BaseObject, int nSpellPoint);

/// <summary>原文 `TBaseObject_IncHealthSpell = procedure(BaseObject: _TBaseObject; nHP, nMP: Integer; SendChangedToClient: BOOL); stdcall;`（PluginInterface.pas:1131）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_IncHealthSpell(IBaseObjectHandle BaseObject, int nHP, int nMP, int SendChangedToClient);

/// <summary>原文 `TBaseObject_HealthSpellChanged = procedure(BaseObject: _TBaseObject; dwDelay: DWORD); stdcall;`（PluginInterface.pas:1134）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_HealthSpellChanged(IBaseObjectHandle BaseObject, uint dwDelay);

/// <summary>原文 `TBaseObject_FeatureChanged = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:1137）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_FeatureChanged(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_WeightChanged = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:1140）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_WeightChanged(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_GetHitStruckDamage = function(BaseObject: _TBaseObject; Target: _TBaseObject; nDamage: Integer; MagicACInfo: PMagicACInfo; nType: Integer): Integer; stdcall;`（PluginInterface.pas:1144）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetHitStruckDamage(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nDamage, IntPtr MagicACInfo, int nType);

/// <summary>原文 `TBaseObject_GetMagStruckDamage = function(BaseObject: _TBaseObject; Target: _TBaseObject; nDamage: Integer): Integer; stdcall;`（PluginInterface.pas:1148）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetMagStruckDamage(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nDamage);

/// <summary>原文 `TBaseObject_GetActorIcon = function(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;`（PluginInterface.pas:1151）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_GetActorIcon(IBaseObjectHandle BaseObject, int pIndex, ref TActorIcon ActorIcon);

/// <summary>原文 `TBaseObject_SetActorIcon = function(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;`（PluginInterface.pas:1154）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_SetActorIcon(IBaseObjectHandle BaseObject, int pIndex, ref TActorIcon ActorIcon);

/// <summary>原文 `TBaseObject_RefUseIcons = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:1157）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_RefUseIcons(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_RefUseEffects = procedure(BaseObject: _TBaseObject); stdcall;`（PluginInterface.pas:1160）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_RefUseEffects(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_SpaceMove = procedure(BaseObject: _TBaseObject; sMapName: PAnsiChar; nX, nY: Integer; nInt: Integer); stdcall;`（PluginInterface.pas:1163）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_SpaceMove(IBaseObjectHandle BaseObject, byte[] sMapName, int nX, int nY, int nInt);

/// <summary>原文 `TBaseObject_MapRandomMove = procedure(BaseObject: _TBaseObject; sMapName: PAnsiChar; nInt: Integer); stdcall;`（PluginInterface.pas:1166）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_MapRandomMove(IBaseObjectHandle BaseObject, byte[] sMapName, int nInt);

/// <summary>原文 `TBaseObject_CanMove = function(BaseObject: _TBaseObject): BOOL; stdcall;`（PluginInterface.pas:1169）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_CanMove(IBaseObjectHandle BaseObject);

/// <summary>原文 `TBaseObject_CanRun = function(BaseObject: _TBaseObject; nCurrX, nCurrY, nX, nY: Integer): BOOL; stdcall;`（PluginInterface.pas:1172）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_CanRun(IBaseObjectHandle BaseObject, int nCurrX, int nCurrY, int nX, int nY);

/// <summary>原文 `TBaseObject_TurnTo = procedure(BaseObject: _TBaseObject; btDir: Byte); stdcall;`（PluginInterface.pas:1175）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TBaseObject_TurnTo(IBaseObjectHandle BaseObject, byte btDir);

/// <summary>原文 `TBaseObject_WalkTo = function(BaseObject: _TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;`（PluginInterface.pas:1178）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_WalkTo(IBaseObjectHandle BaseObject, byte btDir, int boFlag);

/// <summary>原文 `TBaseObject_RunTo = function(BaseObject: _TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;`（PluginInterface.pas:1181）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_RunTo(IBaseObjectHandle BaseObject, byte btDir, int boFlag);

/// <summary>原文 `TBaseObject_PluginList = function(BaseObject: _TBaseObject): _TList; stdcall;`（PluginInterface.pas:1184）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TBaseObject_PluginList(IBaseObjectHandle BaseObject);

/// <summary>原文 `TSmartObject_GetMagicList = function(SmartObject: _TSmartObject): _TList; stdcall;`（PluginInterface.pas:1192）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TSmartObject_GetMagicList(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_GetUseItem = function(SmartObject: _TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:1195）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetUseItem(ISmartObjectHandle SmartObject, int pIndex, ref TUserItem UserItem);

/// <summary>原文 `TSmartObject_GetJewelryBoxStatus = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1198）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetJewelryBoxStatus(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetJewelryBoxStatus = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1201）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetJewelryBoxStatus(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetJewelryItem = function(SmartObject: _TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:1204）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetJewelryItem(ISmartObjectHandle SmartObject, int pIndex, ref TUserItem UserItem);

/// <summary>原文 `TSmartObject_GetIsShowGodBless = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1207）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsShowGodBless(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsShowGodBless = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1210）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsShowGodBless(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetGodBlessItemsState = function(SmartObject: _TSmartObject; Index: Integer): BOOL; stdcall;`（PluginInterface.pas:1213）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetGodBlessItemsState(ISmartObjectHandle SmartObject, int pIndex);

/// <summary>原文 `TSmartObject_SetGodBlessItemsState = procedure(SmartObject: _TSmartObject; Index: Integer; Value: BOOL); stdcall;`（PluginInterface.pas:1216）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetGodBlessItemsState(ISmartObjectHandle SmartObject, int pIndex, int pValue);

/// <summary>原文 `TSmartObject_GetGodBlessItem = function(SmartObject: _TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:1219）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetGodBlessItem(ISmartObjectHandle SmartObject, int pIndex, ref TUserItem UserItem);

/// <summary>原文 `TSmartObject_GetFengHaoItems = function(SmartObject: _TSmartObject): _TList; stdcall;`（PluginInterface.pas:1222）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TSmartObject_GetFengHaoItems(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_GetActiveFengHao = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1225）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetActiveFengHao(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetActiveFengHao = procedure(SmartObject: _TSmartObject; FengHaoIndex: Integer); stdcall;`（PluginInterface.pas:1228）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetActiveFengHao(ISmartObjectHandle SmartObject, int FengHaoIndex);

/// <summary>原文 `TSmartObject_ActiveFengHaoChanged = procedure(SmartObject: _TSmartObject); stdcall;`（PluginInterface.pas:1231）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_ActiveFengHaoChanged(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_DeleteFengHao = procedure(SmartObject: _TSmartObject; Index: Integer); stdcall;`（PluginInterface.pas:1234）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_DeleteFengHao(ISmartObjectHandle SmartObject, int pIndex);

/// <summary>原文 `TSmartObject_ClearFengHao = procedure(SmartObject: _TSmartObject); stdcall;`（PluginInterface.pas:1237）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_ClearFengHao(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_GetMoveSpeed = function(SmartObject: _TSmartObject): SmallInt; stdcall;`（PluginInterface.pas:1239）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate short TSmartObject_GetMoveSpeed(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetMoveSpeed = procedure(SmartObject: _TSmartObject; Value: SmallInt); stdcall;`（PluginInterface.pas:1241）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetMoveSpeed(ISmartObjectHandle SmartObject, short pValue);

/// <summary>原文 `TSmartObject_GetAttackSpeed = function(SmartObject: _TSmartObject): SmallInt; stdcall;`（PluginInterface.pas:1243）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate short TSmartObject_GetAttackSpeed(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetAttackSpeed = procedure(SmartObject: _TSmartObject; Value: SmallInt); stdcall;`（PluginInterface.pas:1245）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetAttackSpeed(ISmartObjectHandle SmartObject, short pValue);

/// <summary>原文 `TSmartObject_GetSpellSpeed = function(SmartObject: _TSmartObject): SmallInt; stdcall;`（PluginInterface.pas:1247）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate short TSmartObject_GetSpellSpeed(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetSpellSpeed = procedure(SmartObject: _TSmartObject; Value: SmallInt); stdcall;`（PluginInterface.pas:1249）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetSpellSpeed(ISmartObjectHandle SmartObject, short pValue);

/// <summary>原文 `TSmartObject_RefGameSpeed = procedure(SmartObject: _TSmartObject); stdcall;`（PluginInterface.pas:1252）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_RefGameSpeed(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_GetIsButch = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1255）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsButch(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsButch = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1258）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsButch(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsTrainingNG = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1261）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsTrainingNG(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsTrainingNG = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1263）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsTrainingNG(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsTrainingXF = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1266）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsTrainingXF(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsTrainingXF = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1268）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsTrainingXF(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsOpenLastContinuous = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1271）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsOpenLastContinuous(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsOpenLastContinuous = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1274）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsOpenLastContinuous(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetContinuousMagicOrder = function(SmartObject: _TSmartObject; Index: Integer): Byte; stdcall;`（PluginInterface.pas:1277）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TSmartObject_GetContinuousMagicOrder(ISmartObjectHandle SmartObject, int pIndex);

/// <summary>原文 `TSmartObject_SetContinuousMagicOrder = procedure(SmartObject: _TSmartObject; Index: Integer; Value: Byte); stdcall;`（PluginInterface.pas:1280）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetContinuousMagicOrder(ISmartObjectHandle SmartObject, int pIndex, byte pValue);

/// <summary>原文 `TSmartObject_GetPKDieLostExp = function(SmartObject: _TSmartObject): DWORD; stdcall;`（PluginInterface.pas:1283）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TSmartObject_GetPKDieLostExp(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetPKDieLostExp = procedure(SmartObject: _TSmartObject; Value: DWORD); stdcall;`（PluginInterface.pas:1285）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetPKDieLostExp(ISmartObjectHandle SmartObject, uint pValue);

/// <summary>原文 `TSmartObject_GetPKDieLostLevel = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1288）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetPKDieLostLevel(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetPKDieLostLevel = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1290）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetPKDieLostLevel(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetPKPoint = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1293）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetPKPoint(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetPKPoint = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1295）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetPKPoint(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_IncPKPoint = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1298）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_IncPKPoint(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_DecPKPoint = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1301）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_DecPKPoint(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetPKLevel = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1304）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetPKLevel(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetPKLevel = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1306）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetPKLevel(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsTeleport = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1309）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsTeleport(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsTeleport = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1311）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsTeleport(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsRevival = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1314）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsRevival(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsRevival = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1316）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsRevival(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetRevivalTime = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1319）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetRevivalTime(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetRevivalTime = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`（PluginInterface.pas:1321）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetRevivalTime(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsFlameRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1324）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsFlameRing(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsFlameRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1326）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsFlameRing(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsRecoveryRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1329）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsRecoveryRing(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsRecoveryRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1331）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsRecoveryRing(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsMagicShield = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1334）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsMagicShield(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsMagicShield = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1336）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsMagicShield(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsMuscleRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1339）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsMuscleRing(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsMuscleRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1341）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsMuscleRing(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsFastTrain = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1344）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsFastTrain(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsFastTrain = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1346）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsFastTrain(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsProbeNecklace = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1349）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsProbeNecklace(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsProbeNecklace = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1351）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsProbeNecklace(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsRecallSuite = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1354）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsRecallSuite(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsRecallSuite = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1356）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsRecallSuite(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsPirit = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1359）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsPirit(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsPirit = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1361）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsPirit(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsSupermanItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1364）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsSupermanItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsSupermanItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1366）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsSupermanItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsExpItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1369）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsExpItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsExpItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1371）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsExpItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetExpItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`（PluginInterface.pas:1374）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate double TSmartObject_GetExpItemValue(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetExpItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`（PluginInterface.pas:1376）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetExpItemValue(ISmartObjectHandle SmartObject, double pValue);

/// <summary>原文 `TSmartObject_GetExpItemRate = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1379）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetExpItemRate(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_GetIsPowerItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1382）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsPowerItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsPowerItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1384）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsPowerItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetPowerItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`（PluginInterface.pas:1387）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate double TSmartObject_GetPowerItemValue(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetPowerItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`（PluginInterface.pas:1390）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetPowerItemValue(ISmartObjectHandle SmartObject, double pValue);

/// <summary>原文 `TSmartObject_GetPowerItemRate = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1393）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetPowerItemRate(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_GetIsGuildMove = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1396）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsGuildMove(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsGuildMove = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1398）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsGuildMove(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsAngryRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1401）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsAngryRing(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsAngryRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1403）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsAngryRing(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsStarRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1406）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsStarRing(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsStarRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1408）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsStarRing(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsACItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1411）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsACItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsACItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1413）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsACItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetACItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`（PluginInterface.pas:1416）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate double TSmartObject_GetACItemValue(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetACItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`（PluginInterface.pas:1418）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetACItemValue(ISmartObjectHandle SmartObject, double pValue);

/// <summary>原文 `TSmartObject_GetIsMACItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1421）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsMACItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsMACItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1423）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsMACItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetMACItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`（PluginInterface.pas:1426）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate double TSmartObject_GetMACItemValue(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetMACItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`（PluginInterface.pas:1428）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetMACItemValue(ISmartObjectHandle SmartObject, double pValue);

/// <summary>原文 `TSmartObject_GetIsNoDropItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1431）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsNoDropItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsNoDropItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1433）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsNoDropItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetIsNoDropUseItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`（PluginInterface.pas:1436）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetIsNoDropUseItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_SetIsNoDropUseItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`（PluginInterface.pas:1438）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetIsNoDropUseItem(ISmartObjectHandle SmartObject, int pValue);

/// <summary>原文 `TSmartObject_GetNGAbility = function(SmartObject: _TSmartObject; AbilityNG: pTAbilityNG): BOOL; stdcall;`（PluginInterface.pas:1441）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetNGAbility(ISmartObjectHandle SmartObject, ref TAbilityNG AbilityNG);

/// <summary>原文 `TSmartObject_SetNGAbility = procedure(SmartObject: _TSmartObject; Value: pTAbilityNG); stdcall;`（PluginInterface.pas:1443）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetNGAbility(ISmartObjectHandle SmartObject, ref TAbilityNG pValue);

/// <summary>原文 `TSmartObject_GetAlcohol = function(SmartObject: _TSmartObject; AbilityAlcohol: pTAbilityAlcohol): BOOL; stdcall;`（PluginInterface.pas:1446）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_GetAlcohol(ISmartObjectHandle SmartObject, ref TAbilityAlcohol AbilityAlcohol);

/// <summary>原文 `TSmartObject_SetAlcohol = procedure(SmartObject: _TSmartObject; Value: pTAbilityAlcohol); stdcall;`（PluginInterface.pas:1448）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_SetAlcohol(ISmartObjectHandle SmartObject, ref TAbilityAlcohol pValue);

/// <summary>原文 `TSmartObject_RepairAllItem = procedure(SmartObject: _TSmartObject); stdcall;`（PluginInterface.pas:1451）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TSmartObject_RepairAllItem(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_IsAllowUseMagic = function(SmartObject: _TSmartObject; MagicID: Word): BOOL; stdcall;`（PluginInterface.pas:1454）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_IsAllowUseMagic(ISmartObjectHandle SmartObject, ushort MagicID);

/// <summary>原文 `TSmartObject_SelectMagic = function(SmartObject: _TSmartObject): Integer; stdcall;`（PluginInterface.pas:1457）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_SelectMagic(ISmartObjectHandle SmartObject);

/// <summary>原文 `TSmartObject_AttackTarget = function(SmartObject: _TSmartObject; MagicID: Word; AttackTime: DWORD): BOOL; stdcall;`（PluginInterface.pas:1460）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TSmartObject_AttackTarget(ISmartObjectHandle SmartObject, ushort MagicID, uint AttackTime);

/// <summary>原文 `TPlayObject_GetUserID = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1468）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetUserID(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetIPAddr = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1471）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIPAddr(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetIPLocal = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1474）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIPLocal(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetMachineID = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1477）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetMachineID(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetIsReadyRun = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1480）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsReadyRun(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetLogonTime = function(Player: _TPlayObject; LogonTime: PSystemTime): BOOL; stdcall;`（PluginInterface.pas:1483）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetLogonTime(IPlayObjectHandle pPlayer, IntPtr LogonTime);

/// <summary>原文 `TPlayObject_GetSoftVerDate = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1486）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetSoftVerDate(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetClientType = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1489）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetClientType(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_IsOldClient = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1492）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_IsOldClient(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetScreenWidth = function(Player: _TPlayObject): Word; stdcall;`（PluginInterface.pas:1495）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TPlayObject_GetScreenWidth(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetScreenHeight = function(Player: _TPlayObject): Word; stdcall;`（PluginInterface.pas:1498）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TPlayObject_GetScreenHeight(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetClientViewRange = function(Player: _TPlayObject): Word; stdcall;`（PluginInterface.pas:1501）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TPlayObject_GetClientViewRange(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetRelevel = function(Player: _TPlayObject): Byte; stdcall;`（PluginInterface.pas:1504）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TPlayObject_GetRelevel(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetRelevel = procedure(Player: _TPlayObject; Value: Byte); stdcall;`（PluginInterface.pas:1506）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetRelevel(IPlayObjectHandle pPlayer, byte pValue);

/// <summary>原文 `TPlayObject_GetBonusPoint = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1509）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetBonusPoint(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetBonusPoint = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1511）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetBonusPoint(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_SendAdjustBonus = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1514）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendAdjustBonus(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetHeroName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1517）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetHeroName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetDeputyHeroName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1520）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetDeputyHeroName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetDeputyHeroJob = function(Player: _TPlayObject): Byte; stdcall;`（PluginInterface.pas:1523）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TPlayObject_GetDeputyHeroJob(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetMyHero = function(Player: _TPlayObject): _THeroObject; stdcall;`（PluginInterface.pas:1526）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IHeroObjectHandle TPlayObject_GetMyHero(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetFixedHero = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1529）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetFixedHero(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_ClientHeroLogOn = procedure(Player: _TPlayObject; IsDeputyHero: BOOL); stdcall;`（PluginInterface.pas:1532）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_ClientHeroLogOn(IPlayObjectHandle pPlayer, int IsDeputyHero);

/// <summary>原文 `TPlayObject_GetStorageHero = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1535）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetStorageHero(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetStorageDeputyHero = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1538）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetStorageDeputyHero(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetIsStorageOpen = function(Player: _TPlayObject; Index: Integer): BOOL; stdcall;`（PluginInterface.pas:1541）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsStorageOpen(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SetIsStorageOpen = procedure(Player: _TPlayObject; Index: Integer; Value: BOOL); stdcall;`（PluginInterface.pas:1543）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsStorageOpen(IPlayObjectHandle pPlayer, int pIndex, int pValue);

/// <summary>原文 `TPlayObject_GetGold = function(Player: _TPlayObject): DWORD; stdcall;`（PluginInterface.pas:1546）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TPlayObject_GetGold(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1548）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGold(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_GetGoldMax = function(Player: _TPlayObject): DWORD; stdcall;`（PluginInterface.pas:1551）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TPlayObject_GetGoldMax(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_IncGold = function(Player: _TPlayObject; Value: DWORD): BOOL; stdcall;`（PluginInterface.pas:1554）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_IncGold(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_DecGold = function(Player: _TPlayObject; Value: DWORD): BOOL; stdcall;`（PluginInterface.pas:1557）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_DecGold(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_GoldChanged = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1560）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_GoldChanged(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetGameGold = function(Player: _TPlayObject): DWORD; stdcall;`（PluginInterface.pas:1563）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TPlayObject_GetGameGold(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGameGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1565）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGameGold(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_IncGameGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1568）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncGameGold(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_DecGameGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1571）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_DecGameGold(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_GameGoldChanged = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1574）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_GameGoldChanged(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetGamePoint = function(Player: _TPlayObject): DWORD; stdcall;`（PluginInterface.pas:1577）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TPlayObject_GetGamePoint(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGamePoint = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1580）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGamePoint(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_IncGamePoint = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1583）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncGamePoint(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_DecGamePoint = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1586）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_DecGamePoint(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_GetGameDiamond = function(Player: _TPlayObject): DWORD; stdcall;`（PluginInterface.pas:1589）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TPlayObject_GetGameDiamond(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGameDiamond = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1591）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGameDiamond(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_IncGameDiamond = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1594）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncGameDiamond(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_DecGameDiamond = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1597）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_DecGameDiamond(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_NewGamePointChanged = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1600）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_NewGamePointChanged(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetGameGird = function(Player: _TPlayObject): DWORD; stdcall;`（PluginInterface.pas:1603）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TPlayObject_GetGameGird(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGameGird = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1605）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGameGird(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_IncGameGird = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1608）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncGameGird(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_DecGameGird = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1611）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_DecGameGird(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_GetGameGoldEx = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1614）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetGameGoldEx(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGameGoldEx = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1616）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGameGoldEx(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetGameGlory = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1619）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetGameGlory(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetGameGlory = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1621）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetGameGlory(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_IncGameGlory = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1624）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncGameGlory(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_DecGameGlory = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1627）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_DecGameGlory(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GameGloryChanged = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1630）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_GameGloryChanged(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetPayMentPoint = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1633）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetPayMentPoint(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetPayMentPoint = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1635）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetPayMentPoint(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetMemberType = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1638）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetMemberType(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetMemberType = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1640）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetMemberType(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetMemberLevel = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1643）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetMemberLevel(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetMemberLevel = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1645）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetMemberLevel(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetContribution = function(Player: _TPlayObject): Word; stdcall;`（PluginInterface.pas:1648）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TPlayObject_GetContribution(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetContribution = procedure(Player: _TPlayObject; Value: Word); stdcall;`（PluginInterface.pas:1650）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetContribution(IPlayObjectHandle pPlayer, ushort pValue);

/// <summary>原文 `TPlayObejct_IncExp = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1653）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObejct_IncExp(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_SendExpChanged = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1656）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendExpChanged(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_IncExpNG = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`（PluginInterface.pas:1659）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncExpNG(IPlayObjectHandle pPlayer, uint pValue);

/// <summary>原文 `TPlayObject_SendExpNGChanged = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1662）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendExpNGChanged(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_IncBeadExp = procedure(Player: _TPlayObject; Value: DWORD; IsFromNPC: BOOL); stdcall;`（PluginInterface.pas:1665）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_IncBeadExp(IPlayObjectHandle pPlayer, uint pValue, int IsFromNPC);

/// <summary>原文 `TPlayObject_GetVarP = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`（PluginInterface.pas:1668）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarP(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SetVarP = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1670）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarP(IPlayObjectHandle pPlayer, int pIndex, int pValue);

/// <summary>原文 `TPlayObject_GetVarM = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`（PluginInterface.pas:1673）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarM(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SetVarM = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1675）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarM(IPlayObjectHandle pPlayer, int pIndex, int pValue);

/// <summary>原文 `TPlayObject_GetVarD = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`（PluginInterface.pas:1678）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarD(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SetVarD = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1680）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarD(IPlayObjectHandle pPlayer, int pIndex, int pValue);

/// <summary>原文 `TPlayObject_GetVarU = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`（PluginInterface.pas:1683）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarU(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SetVarU = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1685）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarU(IPlayObjectHandle pPlayer, int pIndex, int pValue);

/// <summary>原文 `TPlayObject_GetVarT = function(Player: _TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1688）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarT(IPlayObjectHandle pPlayer, int pIndex, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_SetVarT = procedure(Player: _TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;`（PluginInterface.pas:1690）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarT(IPlayObjectHandle pPlayer, int pIndex, byte[] pValue);

/// <summary>原文 `TPlayObject_GetVarN = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`（PluginInterface.pas:1693）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarN(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SetVarN = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1695）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarN(IPlayObjectHandle pPlayer, int pIndex, int pValue);

/// <summary>原文 `TPlayObject_GetVarS = function(Player: _TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1698）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetVarS(IPlayObjectHandle pPlayer, int pIndex, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_SetVarS = procedure(Player: _TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;`（PluginInterface.pas:1700）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetVarS(IPlayObjectHandle pPlayer, int pIndex, byte[] pValue);

/// <summary>原文 `TPlayObject_GetDynamicVarList = function(Player: _TPlayObject): _TList; stdcall;`（PluginInterface.pas:1703）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TPlayObject_GetDynamicVarList(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetQuestFlagStatus = function(Player: _TPlayObject; nFlag: Integer): Integer; stdcall;`（PluginInterface.pas:1707）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetQuestFlagStatus(IPlayObjectHandle pPlayer, int nFlag);

/// <summary>原文 `TPlayObject_SetQuestFlagStatus = procedure(Player: _TPlayObject; nFlag: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1709）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetQuestFlagStatus(IPlayObjectHandle pPlayer, int nFlag, int pValue);

/// <summary>原文 `TPlayObject_IsOffLine = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1712）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_IsOffLine(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_IsMaster = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1715）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_IsMaster(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetMasterName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1718）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetMasterName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetMasterHuman = function(Player: _TPlayObject): _TPlayObject; stdcall;`（PluginInterface.pas:1721）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TPlayObject_GetMasterHuman(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetApprenticeNO = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1724）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetApprenticeNO(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetOnlineApprenticeList = function(Player: _TPlayObject): _TList; stdcall;`（PluginInterface.pas:1727）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TPlayObject_GetOnlineApprenticeList(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetAllApprenticeList = function(Player: _TPlayObject): _TList; stdcall;`（PluginInterface.pas:1730）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TPlayObject_GetAllApprenticeList(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetDearName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1733）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetDearName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TPlayObject_GetDearHuman = function(Player: _TPlayObject): _TPlayObject; stdcall;`（PluginInterface.pas:1736）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TPlayObject_GetDearHuman(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetMarryCount = function(Player: _TPlayObject): Byte; stdcall;`（PluginInterface.pas:1739）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TPlayObject_GetMarryCount(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetGroupOwner = function(Player: _TPlayObject): _TPlayObject; stdcall;`（PluginInterface.pas:1742）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TPlayObject_GetGroupOwner(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetGroupMembers = function(Player: _TPlayObject): _TStringList; stdcall;`（PluginInterface.pas:1745）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IStringListHandle TPlayObject_GetGroupMembers(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetIsLockLogin = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1748）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsLockLogin(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsLockLogin = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1750）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsLockLogin(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsAllowGroup = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1753）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsAllowGroup(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsAllowGroup = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1755）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsAllowGroup(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsAllowGroupReCall = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1758）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsAllowGroupReCall(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsAllowGroupReCall = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1760）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsAllowGroupReCall(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsAllowGuildReCall = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1763）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsAllowGuildReCall(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsAllowGuildReCall = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1765）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsAllowGuildReCall(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsAllowTrading = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1768）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsAllowTrading(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsAllowTrading = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1770）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsAllowTrading(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsDisableInviteHorseRiding = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1773）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsDisableInviteHorseRiding(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsDisableInviteHorseRiding = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1775）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsDisableInviteHorseRiding(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsGameGoldTrading = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1778）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsGameGoldTrading(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsGameGoldTrading = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1780）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsGameGoldTrading(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsNewServer = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1783）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsNewServer(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetIsFilterGlobalDropItemMsg = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1786）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsFilterGlobalDropItemMsg(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsFilterGlobalDropItemMsg = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1788）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsFilterGlobalDropItemMsg(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsFilterGlobalCenterMsg = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1791）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsFilterGlobalCenterMsg(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsFilterGlobalCenterMsg = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1793）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsFilterGlobalCenterMsg(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsFilterGolbalSendMsg = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1796）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsFilterGolbalSendMsg(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsFilterGolbalSendMsg = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1798）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsFilterGolbalSendMsg(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsPleaseDrink = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1801）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsPleaseDrink(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_GetIsDrinkWineQuality = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1804）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsDrinkWineQuality(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsDrinkWineQuality = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1806）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsDrinkWineQuality(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsDrinkWineAlcohol = function(Player: _TPlayObject): Integer; stdcall;`（PluginInterface.pas:1809）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsDrinkWineAlcohol(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsDrinkWineAlcohol = procedure(Player: _TPlayObject; Value: Integer); stdcall;`（PluginInterface.pas:1811）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsDrinkWineAlcohol(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_GetIsDrinkWineDrunk = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1814）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_GetIsDrinkWineDrunk(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SetIsDrinkWineDrunk = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`（PluginInterface.pas:1816）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SetIsDrinkWineDrunk(IPlayObjectHandle pPlayer, int pValue);

/// <summary>原文 `TPlayObject_MoveToHome = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1819）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_MoveToHome(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_MoveRandomToHome = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1822）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_MoveRandomToHome(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SendSocket = procedure(Player: _TPlayObject; DefMsg: pTDefaultMessage; sMsg: PAnsiChar); stdcall;`（PluginInterface.pas:1825）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendSocket(IPlayObjectHandle pPlayer, ref TDefaultMessage DefMsg, byte[] sMsg);

/// <summary>原文 `TPlayObject_SendDefMessage = procedure(Player: _TPlayObject; wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word; sMsg: PAnsiChar); stdcall;`（PluginInterface.pas:1828）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendDefMessage(IPlayObjectHandle pPlayer, ushort wIdent, long nRecog, ushort nParam, ushort nTag, ushort nSeries, byte[] sMsg);

/// <summary>原文 `TPlayObject_SendMoveMsg = procedure(Player: _TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nY: Word; nMoveCount: Integer; nFontSize: Integer; nMarqueeTime: Integer); stdcall;`（PluginInterface.pas:1831）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendMoveMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, ushort nY, int nMoveCount, int nFontSize, int nMarqueeTime);

/// <summary>原文 `TPlayObject_SendCenterMsg = procedure(Player: _TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;`（PluginInterface.pas:1834）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendCenterMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, int nTime);

/// <summary>原文 `TPlayObject_SendTopBroadCastMsg = function(Player: _TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer; MsgType: Integer): BOOL; stdcall;`（PluginInterface.pas:1837）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_SendTopBroadCastMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, int nTime, int MsgType);

/// <summary>原文 `TPlayObject_CheckTakeOnItems = function(Player: _TPlayObject; Where: Integer; StdItem: pTStdItem): BOOL; stdcall;`（PluginInterface.pas:1841）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_CheckTakeOnItems(IPlayObjectHandle pPlayer, int Where, ref TStdItem StdItem);

/// <summary>原文 `TPlayObject_ProcessUseItemSkill = procedure(Player: _TPlayObject; Where: Integer; StdItem: pTStdItem; IsTakeOn: BOOL); stdcall;`（PluginInterface.pas:1844）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_ProcessUseItemSkill(IPlayObjectHandle pPlayer, int Where, ref TStdItem StdItem, int IsTakeOn);

/// <summary>原文 `TPlayObject_SendUseItems = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1848）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendUseItems(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SendAddItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1851）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendAddItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem);

/// <summary>原文 `TPlayObject_SendDelItemList = procedure(Player: _TPlayObject; Items: PAnsiChar; ItemsCount: Integer); stdcall;`（PluginInterface.pas:1854）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendDelItemList(IPlayObjectHandle pPlayer, byte[] Items, int ItemsCount);

/// <summary>原文 `TPlayObject_SendDelItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1857）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendDelItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem);

/// <summary>原文 `TPlayObject_SendUpdateItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1860）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendUpdateItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem);

/// <summary>原文 `TPlayObject_SendItemDuraChange = procedure(Player: _TPlayObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1863）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendItemDuraChange(IPlayObjectHandle pPlayer, int ItemWhere, ref TUserItem UserItem);

/// <summary>原文 `TPlayObject_SendBagItems = procedure(Plyaer: _TPlayObject); stdcall;`（PluginInterface.pas:1866）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendBagItems(IPlayObjectHandle Plyaer);

/// <summary>原文 `TPlayObject_SendJewelryBoxItems = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1869）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendJewelryBoxItems(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SendGodBlessItems = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1872）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendGodBlessItems(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SendOpenGodBlessItem = procedure(Player: _TPlayObject; Index: Integer); stdcall;`（PluginInterface.pas:1875）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendOpenGodBlessItem(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SendCloseGodBlessItem = procedure(Player: _TPlayObject; Index: Integer); stdcall;`（PluginInterface.pas:1878）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendCloseGodBlessItem(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SendUseMagics = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1881）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendUseMagics(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SendAddMagic = procedure(Player: _TPlayObject; UserMagic: pTUserMagic); stdcall;`（PluginInterface.pas:1884）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendAddMagic(IPlayObjectHandle pPlayer, ref TUserMagic UserMagic);

/// <summary>原文 `TPlayObject_SendDelMagic = procedure(Player: _TPlayObject; UserMagic: pTUserMagic); stdcall;`（PluginInterface.pas:1887）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendDelMagic(IPlayObjectHandle pPlayer, ref TUserMagic UserMagic);

/// <summary>原文 `TPlayObject_SendFengHaoItems = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1890）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendFengHaoItems(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_SendAddFengHaoItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1893）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendAddFengHaoItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem);

/// <summary>原文 `TPlayObject_SendDelFengHaoItem = procedure(Player: _TPlayObject; Index: Integer); stdcall;`（PluginInterface.pas:1896）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendDelFengHaoItem(IPlayObjectHandle pPlayer, int pIndex);

/// <summary>原文 `TPlayObject_SendSocketStatusFail = procedure(Player: _TPlayObject); stdcall;`（PluginInterface.pas:1899）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_SendSocketStatusFail(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_PlayEffect = procedure(Player: _TPlayObject; nFileIndex, nImageOffset, nImageCount, nLoopCount, nSpeedTime: Integer; btDrawOrder: Byte; nOffsetX: Integer; nOffsetY: Integer); stdcall;`（PluginInterface.pas:1901）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlayObject_PlayEffect(IPlayObjectHandle pPlayer, int nFileIndex, int nImageOffset, int nImageCount, int nLoopCount, int nSpeedTime, byte btDrawOrder, int nOffsetX, int nOffsetY);

/// <summary>原文 `TPlayObject_IsAutoPlayGame = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1905）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_IsAutoPlayGame(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_StartAutoPlayGame = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1908）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_StartAutoPlayGame(IPlayObjectHandle pPlayer);

/// <summary>原文 `TPlayObject_StopAutoPlayGame = function(Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:1911）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlayObject_StopAutoPlayGame(IPlayObjectHandle pPlayer);

/// <summary>原文 `TDummyObject_IsStart = function(Dummyer: _TDummyObject): BOOL; stdcall;`（PluginInterface.pas:1919）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TDummyObject_IsStart(IDummyObjectHandle Dummyer);

/// <summary>原文 `TDummyObject_Start = procedure(Dummyer: _TDummyObject); stdcall;`（PluginInterface.pas:1922）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TDummyObject_Start(IDummyObjectHandle Dummyer);

/// <summary>原文 `TDummyObject_Stop = procedure(Dummyer: _TDummyObject); stdcall;`（PluginInterface.pas:1925）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TDummyObject_Stop(IDummyObjectHandle Dummyer);

/// <summary>原文 `THeroObject_GetAttackMode = function(Hero: _THeroObject): Byte; stdcall;`（PluginInterface.pas:1933）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte THeroObject_GetAttackMode(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SetAttackMode = function(Hero: _THeroObject; Value: Byte; ShowSysMsg: BOOL): BOOL; stdcall;`（PluginInterface.pas:1935）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_SetAttackMode(IHeroObjectHandle pHero, byte pValue, int ShowSysMsg);

/// <summary>原文 `THeroObject_SetNextAttackMode = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1938）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SetNextAttackMode(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_GetBagCount = function(Hero: _THeroObject): Integer; stdcall;`（PluginInterface.pas:1941）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_GetBagCount(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_GetAngryValue = function(Hero: _THeroObject): Integer; stdcall;`（PluginInterface.pas:1944）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_GetAngryValue(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_GetLoyalPoint = function(Hero: _THeroObject): Real; stdcall;`（PluginInterface.pas:1947）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate double THeroObject_GetLoyalPoint(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SetLoyalPoint = procedure(Hero: _THeroObject; Value: Real); stdcall;`（PluginInterface.pas:1949）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SetLoyalPoint(IHeroObjectHandle pHero, double pValue);

/// <summary>原文 `THeroObject_SendLoyalPointChanged = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1951）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendLoyalPointChanged(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_IsDeputy = function(Hero: _THeroObject): BOOL; stdcall;`（PluginInterface.pas:1954）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_IsDeputy(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_GetMasterName = function(Hero: _THeroObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:1957）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_GetMasterName(IHeroObjectHandle pHero, byte[] Dest, ref uint DestLen);

/// <summary>原文 `THeroObject_GetQuestFlagStatus = function(Hero: _THeroObject; nFlag: Integer): Integer; stdcall;`（PluginInterface.pas:1959）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_GetQuestFlagStatus(IHeroObjectHandle pHero, int nFlag);

/// <summary>原文 `THeroObject_SetQuestFlagStatus = procedure(Hero: _THeroObject; nFlag: Integer; Value: Integer); stdcall;`（PluginInterface.pas:1961）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SetQuestFlagStatus(IHeroObjectHandle pHero, int nFlag, int pValue);

/// <summary>原文 `THeroObject_SendUseItems = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1964）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendUseItems(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendBagItems = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1967）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendBagItems(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendJewelryBoxItems = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1970）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendJewelryBoxItems(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendGodBlessItems = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1973）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendGodBlessItems(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendOpenGodBlessItem = procedure(Hero: _THeroObject; Index: Integer); stdcall;`（PluginInterface.pas:1976）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendOpenGodBlessItem(IHeroObjectHandle pHero, int pIndex);

/// <summary>原文 `THeroObject_SendCloseGodBlessItem = procedure(Hero: _THeroObject; Index: Integer); stdcall;`（PluginInterface.pas:1979）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendCloseGodBlessItem(IHeroObjectHandle pHero, int pIndex);

/// <summary>原文 `THeroObject_SendAddItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1982）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendAddItem(IHeroObjectHandle pHero, ref TUserItem UserItem);

/// <summary>原文 `THeroObject_SendDelItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1985）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendDelItem(IHeroObjectHandle pHero, ref TUserItem UserItem);

/// <summary>原文 `THeroObject_SendUpdateItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1988）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendUpdateItem(IHeroObjectHandle pHero, ref TUserItem UserItem);

/// <summary>原文 `THeroObject_SendItemDuraChange = procedure(Hero: _THeroObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:1991）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendItemDuraChange(IHeroObjectHandle pHero, int ItemWhere, ref TUserItem UserItem);

/// <summary>原文 `THeroObject_SendUseMagics = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:1994）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendUseMagics(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendAddMagic = procedure(Hero: _THeroObject; UserMagic: pTUserMagic); stdcall;`（PluginInterface.pas:1997）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendAddMagic(IHeroObjectHandle pHero, ref TUserMagic UserMagic);

/// <summary>原文 `THeroObject_SendDelMagic = procedure(Hero: _THeroObject; UserMagic: pTUserMagic); stdcall;`（PluginInterface.pas:2000）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendDelMagic(IHeroObjectHandle pHero, ref TUserMagic UserMagic);

/// <summary>原文 `THeroObject_FindGroupMagic = function(Hero: _THeroObject; UserMagic: pTUserMagic): BOOL; stdcall;`（PluginInterface.pas:2003）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_FindGroupMagic(IHeroObjectHandle pHero, ref TUserMagic UserMagic);

/// <summary>原文 `THeroObject_GetGroupMagicId = function(Hero: _THeroObject): Integer; stdcall;`（PluginInterface.pas:2006）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_GetGroupMagicId(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendFengHaoItems = procedure(Hero: _THeroObject); stdcall;`（PluginInterface.pas:2009）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendFengHaoItems(IHeroObjectHandle pHero);

/// <summary>原文 `THeroObject_SendAddFengHaoItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`（PluginInterface.pas:2012）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendAddFengHaoItem(IHeroObjectHandle pHero, ref TUserItem UserItem);

/// <summary>原文 `THeroObject_SendDelFengHaoItem = procedure(Hero: _THeroObject; Index: Integer); stdcall;`（PluginInterface.pas:2015）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_SendDelFengHaoItem(IHeroObjectHandle pHero, int pIndex);

/// <summary>原文 `THeroObject_IncExp = procedure(Hero: _THeroObject; dwExp: DWORD); stdcall;`（PluginInterface.pas:2017）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_IncExp(IHeroObjectHandle pHero, uint dwExp);

/// <summary>原文 `THeroObject_IncExpNG = procedure(Hero: _THeroObject; dwExp: DWORD); stdcall;`（PluginInterface.pas:2019）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void THeroObject_IncExpNG(IHeroObjectHandle pHero, uint dwExp);

/// <summary>原文 `THeroObject_IsOldClient = function(Hero: _THeroObject): BOOL; stdcall;`（PluginInterface.pas:2021）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int THeroObject_IsOldClient(IHeroObjectHandle pHero);

/// <summary>原文 `TNormNpc_Create = function(CharName, sMapName, sScript: PAnsiChar; X, Y: Integer; wAppr: Word; boIsHide: BOOL) : _TNormNpc; stdcall;`（PluginInterface.pas:2029）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TNormNpc_Create(byte[] CharName, byte[] sMapName, byte[] sScript, int X, int Y, ushort wAppr, int boIsHide);

/// <summary>原文 `TNormNpc_LoadNpcScript = procedure(NormNpc: _TNormNpc); stdcall;`（PluginInterface.pas:2033）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_LoadNpcScript(INormNpcHandle NormNpc);

/// <summary>原文 `TNormNpc_ClearScript = procedure(NormNpc: _TNormNpc); stdcall;`（PluginInterface.pas:2036）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_ClearScript(INormNpcHandle NormNpc);

/// <summary>原文 `TNormNpc_GetFilePath = function(NormNpc: _TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2038）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetFilePath(INormNpcHandle NormNpc, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TNormNpc_SetFilePath = procedure(NormNpc: _TNormNpc; Value: PAnsiChar); stdcall;`（PluginInterface.pas:2040）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_SetFilePath(INormNpcHandle NormNpc, byte[] pValue);

/// <summary>原文 `TNormNpc_GetPath = function(NormNpc: _TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2042）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetPath(INormNpcHandle NormNpc, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TNormNpc_SetPath = procedure(NormNpc: _TNormNpc; Value: PAnsiChar); stdcall;`（PluginInterface.pas:2044）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_SetPath(INormNpcHandle NormNpc, byte[] pValue);

/// <summary>原文 `TNormNpc_GetIsHide = function(NormNpc: _TNormNpc): BOOL; stdcall;`（PluginInterface.pas:2046）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetIsHide(INormNpcHandle NormNpc);

/// <summary>原文 `TNormNpc_SetIsHide = procedure(NormNpc: _TNormNpc; Value: BOOL); stdcall;`（PluginInterface.pas:2048）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_SetIsHide(INormNpcHandle NormNpc, int pValue);

/// <summary>原文 `TNormNpc_GetIsQuest = function(NormNpc: _TNormNpc): BOOL; stdcall;`（PluginInterface.pas:2050）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetIsQuest(INormNpcHandle NormNpc);

/// <summary>原文 `TNormNpc_GetLineVariableText = function(NormNpc: _TNormNpc; Player: _TPlayObject; sMsg: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2052）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetLineVariableText(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TNormNpc_GotoLable = procedure(NormNpc: _TNormNpc; Player: _TPlayObject; sLabel: PAnsiChar; boExtJmp: BOOL); stdcall;`（PluginInterface.pas:2055）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_GotoLable(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sLabel, int boExtJmp);

/// <summary>原文 `TNormNpc_SendMsgToUser = procedure(NormNpc: _TNormNpc; Player: _TPlayObject; sMsg: PAnsiChar); stdcall;`（PluginInterface.pas:2057）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_SendMsgToUser(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg);

/// <summary>原文 `TNormNpc_MessageBox = procedure(NormNpc: _TNormNpc; Player: _TPlayObject; sMsg: PAnsiChar); stdcall;`（PluginInterface.pas:2059）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNormNpc_MessageBox(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg);

/// <summary>原文 `TNormNpc_GetVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;`（PluginInterface.pas:2061）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, ref uint sValueSize, ref int nValue);

/// <summary>原文 `TNormNpc_SetVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; nValue: Integer): BOOL; stdcall;`（PluginInterface.pas:2064）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_SetVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, int nValue);

/// <summary>原文 `TNormNpc_GetDynamicVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;`（PluginInterface.pas:2067）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_GetDynamicVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, ref uint sValueSize, ref int nValue);

/// <summary>原文 `TNormNpc_SetDynamicVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; nValue: Integer): BOOL; stdcall;`（PluginInterface.pas:2070）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TNormNpc_SetDynamicVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, int nValue);

/// <summary>原文 `TUserEngine_GetPlayerList = function(): _TStringList; stdcall;`（PluginInterface.pas:2079）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IStringListHandle TUserEngine_GetPlayerList();

/// <summary>原文 `TUserEngine_GetPlayerByName = function(ChrName: PAnsiChar): _TPlayObject; stdcall;`（PluginInterface.pas:2082）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TUserEngine_GetPlayerByName(byte[] ChrName);

/// <summary>原文 `TUserEngine_GetPlayerByUserID = function(UserID: PAnsiChar): _TPlayObject; stdcall;`（PluginInterface.pas:2085）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TUserEngine_GetPlayerByUserID(byte[] UserID);

/// <summary>原文 `TUserEngine_GetPlayerByObject = function(AObject: _TObject): _TPlayObject; stdcall;`（PluginInterface.pas:2088）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TUserEngine_GetPlayerByObject(object AObject);

/// <summary>原文 `TUserEngine_GetOfflinePlayer = function(UserID: PAnsiChar): _TPlayObject; stdcall;`（PluginInterface.pas:2091）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IPlayObjectHandle TUserEngine_GetOfflinePlayer(byte[] UserID);

/// <summary>原文 `TUserEngine_KickPlayer = procedure(ChrName: PAnsiChar); stdcall;`（PluginInterface.pas:2094）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_KickPlayer(byte[] ChrName);

/// <summary>原文 `TUserEngine_GetHeroList = function(): _TStringList; stdcall;`（PluginInterface.pas:2097）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IStringListHandle TUserEngine_GetHeroList();

/// <summary>原文 `TUserEngine_GetHeroByName = function(ChrName: PAnsiChar): _THeroObject; stdcall;`（PluginInterface.pas:2100）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IHeroObjectHandle TUserEngine_GetHeroByName(byte[] ChrName);

/// <summary>原文 `TUserEngine_KickHero = function(ChrName: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:2103）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_KickHero(byte[] ChrName);

/// <summary>原文 `TUserEngine_GetMerchantList = function(): _TList; stdcall;`（PluginInterface.pas:2106）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TUserEngine_GetMerchantList();

/// <summary>原文 `TUserEngine_GetCustomNpcConfigList = function(): _TList; stdcall;`（PluginInterface.pas:2109）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TUserEngine_GetCustomNpcConfigList();

/// <summary>原文 `TUserEngine_GetQuestNPCList = function(): _TStringList; stdcall;`（PluginInterface.pas:2112）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IStringListHandle TUserEngine_GetQuestNPCList();

/// <summary>原文 `TUserEngine_GetManageNPC = function(): _TNormNpc; stdcall;`（PluginInterface.pas:2114）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_GetManageNPC();

/// <summary>原文 `TUserEngine_GetFunctionNPC = function(): _TNormNpc; stdcall;`（PluginInterface.pas:2116）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_GetFunctionNPC();

/// <summary>原文 `TUserEngine_GetRobotNPC = function(): _TNormNpc; stdcall;`（PluginInterface.pas:2118）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_GetRobotNPC();

/// <summary>原文 `TUserEngine_MissionNPC = function(): _TNormNpc; stdcall;`（PluginInterface.pas:2120）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_MissionNPC();

/// <summary>原文 `TUserEngine_FindMerchant = function(AObject: _TObject): _TNormNpc; stdcall;`（PluginInterface.pas:2123）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_FindMerchant(object AObject);

/// <summary>原文 `TUserEngine_FindMerchantByPos = function(MapName: PAnsiChar; nX, nY: Integer): _TNormNpc; stdcall;`（PluginInterface.pas:2126）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_FindMerchantByPos(byte[] MapName, int nX, int nY);

/// <summary>原文 `TUserEngine_FindQuestNPC = function(AObject: _TObject): _TNormNpc; stdcall;`（PluginInterface.pas:2129）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate INormNpcHandle TUserEngine_FindQuestNPC(object AObject);

/// <summary>原文 `TUserEngine_GetMagicList = function(): _TList; stdcall;`（PluginInterface.pas:2132）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TUserEngine_GetMagicList();

/// <summary>原文 `TUserEngine_GetCustomMagicConfigList = function(): _TList; stdcall;`（PluginInterface.pas:2135）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TUserEngine_GetCustomMagicConfigList();

/// <summary>原文 `TUserEngine_GetMagicACList = function(): _TMagicACList; stdcall;`（PluginInterface.pas:2138）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMagicACListHandle TUserEngine_GetMagicACList();

/// <summary>原文 `TUserEngine_FindMagicByName = function(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2141）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindMagicByName(byte[] MagName, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindMagicByIndex = function(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2144）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindMagicByIndex(int MagIdx, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindMagicByNameEx = function(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2147）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindMagicByNameEx(byte[] MagName, int MagAttr, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindMagicByIndexEx = function(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2150）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindMagicByIndexEx(int MagIdx, int MagAttr, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindHeroMagicByName = function(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2153）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindHeroMagicByName(byte[] MagName, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindHeroMagicByIndex = function(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2156）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindHeroMagicByIndex(int MagIdx, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindHeroMagicByNameEx = function(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2159）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindHeroMagicByNameEx(byte[] MagName, int MagAttr, ref TMagic Magic);

/// <summary>原文 `TUserEngine_FindHeroMagicByIndexEx = function(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`（PluginInterface.pas:2162）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_FindHeroMagicByIndexEx(int MagIdx, int MagAttr, ref TMagic Magic);

/// <summary>原文 `TUserEngine_GetStdItemList = function(): _TList; stdcall;`（PluginInterface.pas:2165）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TUserEngine_GetStdItemList();

/// <summary>原文 `TUserEngine_GetStdItemByName = function(ItemName: PAnsiChar; StdItem: pTStdItem): BOOL; stdcall;`（PluginInterface.pas:2168）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetStdItemByName(byte[] ItemName, ref TStdItem StdItem);

/// <summary>原文 `TUserEngine_GetStdItemByIndex = function(ItemIdx: Integer; StdItem: pTStdItem): BOOL; stdcall;`（PluginInterface.pas:2171）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetStdItemByIndex(int ItemIdx, ref TStdItem StdItem);

/// <summary>原文 `TUserEngine_GetStdItemName = function(ItemIdx: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2174）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetStdItemName(int ItemIdx, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TUserEngine_GetStdItemIndex = function(ItemName: PAnsiChar): Integer; stdcall;`（PluginInterface.pas:2177）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetStdItemIndex(byte[] ItemName);

/// <summary>原文 `TUserEngine_MonsterList = function(): _TList; stdcall;`（PluginInterface.pas:2180）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TUserEngine_MonsterList();

/// <summary>原文 `TUserEngine_SendBroadCastMsg = function(sMsg: PAnsiChar; FColor, BColor: Integer; MsgType: Integer): BOOL; stdcall;`（PluginInterface.pas:2182）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_SendBroadCastMsg(byte[] sMsg, int FColor, int BColor, int MsgType);

/// <summary>原文 `TUserEngine_SendBroadCastMsgExt = function(sMsg: PAnsiChar; MsgType: Integer): BOOL; stdcall;`（PluginInterface.pas:2184）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_SendBroadCastMsgExt(byte[] sMsg, int MsgType);

/// <summary>原文 `TUserEngine_SendTopBroadCastMsg = function(sMsg: PAnsiChar; FColor, BColor: Integer; nTime: Integer; MsgType: Integer) : BOOL; stdcall;`（PluginInterface.pas:2186）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_SendTopBroadCastMsg(byte[] sMsg, int FColor, int BColor, int nTime, int MsgType);

/// <summary>原文 `TUserEngine_SendMoveMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor: Byte; nY, nMoveCount: Integer; nFontSize: Integer; nMarqueeTime: Integer); stdcall;`（PluginInterface.pas:2189）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_SendMoveMsg(byte[] sMsg, byte btFColor, byte btBColor, int nY, int nMoveCount, int nFontSize, int nMarqueeTime);

/// <summary>原文 `TUserEngine_SendCenterMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;`（PluginInterface.pas:2192）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_SendCenterMsg(byte[] sMsg, byte btFColor, byte btBColor, int nTime);

/// <summary>原文 `TUserEngine_SendNewLineMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte; nY, nShowMsgTime, nDrawType: Integer); stdcall;`（PluginInterface.pas:2195）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_SendNewLineMsg(byte[] sMsg, byte btFColor, byte btBColor, byte btFontSize, int nY, int nShowMsgTime, int nDrawType);

/// <summary>原文 `TUserEngine_SendSuperMoveMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte; nY, nMoveCount: Integer); stdcall;`（PluginInterface.pas:2199）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_SendSuperMoveMsg(byte[] sMsg, byte btFColor, byte btBColor, byte btFontSize, int nY, int nMoveCount);

/// <summary>原文 `TUserEngine_SendSceneShake = procedure(Count: Integer); stdcall;`（PluginInterface.pas:2203）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_SendSceneShake(int pCount);

/// <summary>原文 `TUserEngine_CopyToUserItemFromName = function(ItemName: PAnsiChar; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:2205）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_CopyToUserItemFromName(byte[] ItemName, ref TUserItem UserItem);

/// <summary>原文 `TUserEngine_CopyToUserItemFromItem = function(StdItem: pTStdItem; ItemIndex: Integer; UserItem: pTUserItem): BOOL; stdcall;`（PluginInterface.pas:2207）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_CopyToUserItemFromItem(ref TStdItem StdItem, int ItemIndex, ref TUserItem UserItem);

/// <summary>原文 `TUserEngine_RandomUpgradeItem = procedure(UserItem: pTUserItem); stdcall;`（PluginInterface.pas:2209）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_RandomUpgradeItem(ref TUserItem UserItem);

/// <summary>原文 `TUserEngine_RandomItemNewAbil = procedure(UserItem: pTUserItem); stdcall;`（PluginInterface.pas:2212）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_RandomItemNewAbil(ref TUserItem UserItem);

/// <summary>原文 `TUserEngine_GetUnknowItemValue = procedure(UserItem: pTUserItem); stdcall;`（PluginInterface.pas:2214）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TUserEngine_GetUnknowItemValue(ref TUserItem UserItem);

/// <summary>原文 `TUserEngine_GetAllDummyCount = function(): Integer; stdcall;`（PluginInterface.pas:2217）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetAllDummyCount();

/// <summary>原文 `TUserEngine_GetMapDummyCount = function(Envir: _TEnvirnoment): Integer; stdcall;`（PluginInterface.pas:2220）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetMapDummyCount(IEnvirnoment Envir);

/// <summary>原文 `TUserEngine_GetOfflineCount = function(): Integer; stdcall;`（PluginInterface.pas:2223）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetOfflineCount();

/// <summary>原文 `TUserEngine_GetRealPlayerCount = function(): Integer; stdcall;`（PluginInterface.pas:2226）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TUserEngine_GetRealPlayerCount();

/// <summary>原文 `TGuild_GetGuildName = function(Guild: _TGuild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2234）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetGuildName(IGuildHandle pGuild, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TGuild_GetJoinJob = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2237）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetJoinJob(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetJoinLevel = function(Guild: _TGuild): DWORD; stdcall;`（PluginInterface.pas:2240）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TGuild_GetJoinLevel(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetJoinMsg = function(Guild: _TGuild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2243）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetJoinMsg(IGuildHandle pGuild, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TGuild_GetBuildPoint = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2246）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetBuildPoint(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetAurae = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2249）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetAurae(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetStability = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2252）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetStability(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetFlourishing = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2255）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetFlourishing(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetChiefItemCount = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2258）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetChiefItemCount(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetMemberCount = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2261）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetMemberCount(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetOnlineMemeberCount = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2264）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetOnlineMemeberCount(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetMasterCount = function(Guild: _TGuild): Integer; stdcall;`（PluginInterface.pas:2267）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetMasterCount(IGuildHandle pGuild);

/// <summary>原文 `TGuild_GetMaster = procedure(Guild: _TGuild; var Master1, Master2: _TPlayObject); stdcall;`（PluginInterface.pas:2270）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TGuild_GetMaster(IGuildHandle pGuild, ref IPlayObjectHandle Master1, ref IPlayObjectHandle Master2);

/// <summary>原文 `TGuild_GetMasterName = function(Guild: _TGuild; Master1: PAnsiChar; var Master1Size: DWORD; Master2: PAnsiChar; var Master2Size: DWORD): BOOL; stdcall;`（PluginInterface.pas:2273）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetMasterName(IGuildHandle pGuild, byte[] Master1, ref uint Master1Size, byte[] Master2, ref uint Master2Size);

/// <summary>原文 `TGuild_CheckMemberIsFull = function(Guild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2277）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_CheckMemberIsFull(IGuildHandle pGuild);

/// <summary>原文 `TGuild_IsMemeber = function(Guild: _TGuild; CharName: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:2280）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_IsMemeber(IGuildHandle pGuild, byte[] CharName);

/// <summary>原文 `TGuild_AddMember = function(Guild: _TGuild; Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:2283）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_AddMember(IGuildHandle pGuild, IPlayObjectHandle pPlayer);

/// <summary>原文 `TGuild_AddMemberEx = function(Guild: _TGuild; CharName: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:2285）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_AddMemberEx(IGuildHandle pGuild, byte[] CharName);

/// <summary>原文 `TGuild_DelMemeber = function(Guild: _TGuild; Player: _TPlayObject): BOOL; stdcall;`（PluginInterface.pas:2288）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_DelMemeber(IGuildHandle pGuild, IPlayObjectHandle pPlayer);

/// <summary>原文 `TGuild_DelMemeberEx = function(Guild: _TGuild; CharName: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:2290）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_DelMemeberEx(IGuildHandle pGuild, byte[] CharName);

/// <summary>原文 `TGuild_IsAllianceGuild = function(Guild: _TGuild; CheckGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2293）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_IsAllianceGuild(IGuildHandle pGuild, IGuildHandle CheckGuild);

/// <summary>原文 `TGuild_IsWarGuild = function(Guild: _TGuild; CheckGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2296）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_IsWarGuild(IGuildHandle pGuild, IGuildHandle CheckGuild);

/// <summary>原文 `TGuild_IsAttentionGuild = function(Guild: _TGuild; CheckGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2299）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_IsAttentionGuild(IGuildHandle pGuild, IGuildHandle CheckGuild);

/// <summary>原文 `TGuild_AddAlliance = function(Guild: _TGuild; AddGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2302）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_AddAlliance(IGuildHandle pGuild, IGuildHandle AddGuild);

/// <summary>原文 `TGuild_AddWarGuild = function(Guild: _TGuild; AddGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2305）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_AddWarGuild(IGuildHandle pGuild, IGuildHandle AddGuild);

/// <summary>原文 `TGuild_AddAttentionGuild = function(Guild: _TGuild; AddGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2308）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_AddAttentionGuild(IGuildHandle pGuild, IGuildHandle AddGuild);

/// <summary>原文 `TGuild_DelAllianceGuild = function(Guild: _TGuild; DelGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2311）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_DelAllianceGuild(IGuildHandle pGuild, IGuildHandle DelGuild);

/// <summary>原文 `TGuild_DelAttentionGuild = function(Guild: _TGuild; DelGuild: _TGuild): BOOL; stdcall;`（PluginInterface.pas:2314）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_DelAttentionGuild(IGuildHandle pGuild, IGuildHandle DelGuild);

/// <summary>原文 `TGuild_GetRandNameByName = function(Guild: _TGuild; CharName: PAnsiChar; var nRankNo: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2316）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetRandNameByName(IGuildHandle pGuild, byte[] CharName, ref int nRankNo, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TGuild_GetRandNameByPlayer = function(Guild: _TGuild; Player: _TPlayObject; var nRankNo: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`（PluginInterface.pas:2319）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuild_GetRandNameByPlayer(IGuildHandle pGuild, IPlayObjectHandle pPlayer, ref int nRankNo, byte[] Dest, ref uint DestLen);

/// <summary>原文 `TGuild_SendGuildMsg = procedure(Guild: _TGuild; Msg: PAnsiChar); stdcall;`（PluginInterface.pas:2323）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TGuild_SendGuildMsg(IGuildHandle pGuild, byte[] Msg);

/// <summary>原文 `TGuildManager_FindGuild = function(GuildName: PAnsiChar): _TGuild; stdcall;`（PluginInterface.pas:2331）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IGuildHandle TGuildManager_FindGuild(byte[] GuildName);

/// <summary>原文 `TGuildManager_GetPlayerGuild = function(CharName: PAnsiChar): _TGuild; stdcall;`（PluginInterface.pas:2334）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IGuildHandle TGuildManager_GetPlayerGuild(byte[] CharName);

/// <summary>原文 `TGuildManager_AddGuild = function(GuildName, GuildMaster: PAnsiChar): BOOL; stdcall;`（PluginInterface.pas:2337）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuildManager_AddGuild(byte[] GuildName, byte[] GuildMaster);

/// <summary>原文 `TGuildManager_DelGuild = function(GuildName: PAnsiChar; var IsFoundGuild: BOOL): BOOL; stdcall;`（PluginInterface.pas:2340）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TGuildManager_DelGuild(byte[] GuildName, ref int IsFoundGuild);

/// <summary>原文缺失类型声明：PluginImplement.pas:1852 实现体；被 PluginInterface.pas:2510 引用。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TM2Engine_GetOtherFileDir(int M2FileType, byte[] Dest, ref uint DestLen);

/// <summary>原文缺失类型声明：PluginImplement.pas:3197 实现体；被 PluginInterface.pas:2697 引用。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TBaseObject_TrainSkill(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic, int nTranPoint, int IsDoCheck);

