using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GXX.RunGate;

// =====================================================================================
// RunGatePluginInterface.pas 1:1 转换（Source\RunGate\RunGatePluginInterface.pas，337 行）
//
// 原文是给"原生 DLL 插件"用的 **C 风格函数指针表**（stdcall ABI）。托管侧保留两套表达：
//   1) ABI 层（保真、可 wire/指针兼容）：`[UnmanagedFunctionPointer(CallingConvention.StdCall)]`
//      委托 + `[StructLayout(LayoutKind.Sequential, Pack = 1)]` 记录，字段顺序与
//      `Package` 与原文逐字一致，`Reserved: array[0..N-1] of Pointer` 用 fixed 缓冲保留占位。
//   2) 托管层（可读、可继承）：`I*` 接口，成员与原文函数指针一一对应，供托管插件实现。
// 原文 `_TObject` / `_TMenuItem` / `_TList` / `_TClientContext` 是 Windows/VCL/IOCP 类型的
// 别名；托管侧 `_TObject → object`、其余 → 接缝接口（见文件末尾接缝说明）。
//
// 类型映射：
//   BOOL      → int（原文 Windows.BOOL 为 4 字节；托管侧用 int 保持 ABI 宽度，取值约定 0/1）
//   DWORD     → uint
//   Word      → ushort
//   Byte      → byte
//   Integer   → int
//   Int64     → long
//   PAnsiChar → byte[]（GBK 文本缓冲；`Dest: PAnsiChar; var DestLen: DWORD` 为出参缓冲）
//   PTDefaultMessage → 由 GXX.Core.Protocol.Grobal2 提供的结构（接缝，暂用 TDefaultMessage）
// =====================================================================================

/// <summary>原文 `TNotifyEventEx = procedure(Sender: _TObject); stdcall`（原 :14）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TNotifyEventEx(object sender);

// ---------------- TList 对象列表（原 :20-70） ----------------

/// <summary>原文 `TList_Create = function(): _TList; stdcall`（原 :21-22）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IListHandle TList_Create();

/// <summary>原文 `TList_Free = procedure(List: _TList); stdcall`（原 :25-26）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Free(IListHandle list);

/// <summary>原文 `TList_Count = function(List: _TList): Integer; stdcall`（原 :29-30）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TList_Count(IListHandle list);

/// <summary>原文 `TList_Clear = procedure(List: _TList); stdcall`（原 :33-34）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Clear(IListHandle list);

/// <summary>原文 `TList_Add = procedure(List: _TList; Item: Pointer); stdcall`（原 :37-38）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Add(IListHandle list, IntPtr item);

/// <summary>原文 `TList_Insert = procedure(List: _TList; Index: Integer; Item: Pointer); stdcall`（原 :41-42）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Insert(IListHandle list, int index, IntPtr item);

/// <summary>原文 `TList_Remove = procedure(List: _TList; Item: Pointer); stdcall`（原 :45-46）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Remove(IListHandle list, IntPtr item);

/// <summary>原文 `TList_Delete = procedure(List: _TList; Index: Integer); stdcall`（原 :49-50）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Delete(IListHandle list, int index);

/// <summary>原文 `TList_GetItem = function(List: _TList; Index: Integer): Pointer; stdcall`（原 :53-54）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IntPtr TList_GetItem(IListHandle list, int index);

/// <summary>原文 `TList_SetItem = procedure(List: _TList; Index: Integer; Item: Pointer); stdcall`（原 :57-58）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_SetItem(IListHandle list, int index, IntPtr item);

/// <summary>原文 `TList_IndexOf = function(List: _TList; Item: Pointer): Integer; stdcall`（原 :61-62）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TList_IndexOf(IListHandle list, IntPtr item);

/// <summary>原文 `TList_Exchange = procedure(List: _TList; Index1, Index2: Integer); stdcall`（原 :65-66）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_Exchange(IListHandle list, int index1, int index2);

/// <summary>原文 `TList_CopyTo = procedure(Source, Dest: _TList); stdcall`（原 :69-70）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TList_CopyTo(IListHandle source, IListHandle dest);

// ---------------- TMenu 菜单（原 :78-149） ----------------

/// <summary>原文 `TMenu_Count = function(MenuItem: _TMenuItem): Integer; stdcall`（原 :78-79）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_Count(IMenuItem menuItem);

/// <summary>原文 `TMenu_GetItems = function(MenuItem: _TMenuItem; Index: Integer): _TMenuItem; stdcall`（原 :82-83）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_GetItems(IMenuItem menuItem, int index);

/// <summary>原文 `TMenu_Add = function(PlugID: Integer; MenuItem: _TMenuItem; Caption: PAnsiChar;
///   Tag: Integer; OnClick: TNotifyEventEx): _TMenuItem; stdcall`（原 :86-88）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_Add(int plugId, IMenuItem menuItem, byte[] caption, int tag, TNotifyEventEx onClick);

/// <summary>原文 `TMenu_Insert = function(PlugID: Integer; MenuItem: _TMenuItem; Index: Integer;
///   Caption: PAnsiChar; Tag: Integer; OnClick: TNotifyEventEx): _TMenuItem; stdcall`（原 :91-93）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate IMenuItem TMenu_Insert(int plugId, IMenuItem menuItem, int index, byte[] caption, int tag, TNotifyEventEx onClick);

/// <summary>原文 `TMenu_GetCaption = function(MenuItem: _TMenuItem; Dest: PAnsiChar;
///   var DestLen: DWORD): BOOL; stdcall`（原 :96-97）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetCaption(IMenuItem menuItem, byte[] dest, ref uint destLen);

/// <summary>原文 `TMenu_SetCaption = procedure(MenuItem: _TMenuItem; Caption: PAnsiChar); stdcall`（原 :100-101）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetCaption(IMenuItem menuItem, byte[] caption);

/// <summary>原文 `TMenu_GetEnabled = function(MenuItem: _TMenuItem): BOOL; stdcall`（原 :104-105）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetEnabled(IMenuItem menuItem);

/// <summary>原文 `TMenu_SetEnabled = procedure(MenuItem: _TMenuItem; Enabled: BOOL); stdcall`（原 :108-109）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetEnabled(IMenuItem menuItem, int enabled);

/// <summary>原文 `TMenu_GetVisable = function(MenuItem: _TMenuItem): BOOL; stdcall`（原 :112-113）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetVisable(IMenuItem menuItem);

/// <summary>原文 `TMenu_SetVisable = procedure(MenuItem: _TMenuItem; Visible: BOOL); stdcall`（原 :116-117）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetVisable(IMenuItem menuItem, int visible);

/// <summary>原文 `TMenu_GetChecked = function(MenuItem: _TMenuItem): BOOL; stdcall`（原 :120-121）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetChecked(IMenuItem menuItem);

/// <summary>原文 `TMenu_SetChecked = procedure(MenuItem: _TMenuItem; Checked: BOOL); stdcall`（原 :124-125）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetChecked(IMenuItem menuItem, int isChecked);

/// <summary>原文 `TMenu_GetRadioItem = function(MenuItem: _TMenuItem): BOOL; stdcall`（原 :128-129）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetRadioItem(IMenuItem menuItem);

/// <summary>原文 `TMenu_SetRadioItem = procedure(MenuItem: _TMenuItem; IsRadioItem: BOOL); stdcall`（原 :132-133）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetRadioItem(IMenuItem menuItem, int isRadioItem);

/// <summary>原文 `TMenu_GetGroupIndex = function(MenuItem: _TMenuItem): Integer; stdcall`（原 :136-137）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetGroupIndex(IMenuItem menuItem);

/// <summary>原文 `TMenu_SetGroupIndex = procedure(MenuItem: _TMenuItem; Value: Integer); stdcall`（原 :140-141）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetGroupIndex(IMenuItem menuItem, int value);

/// <summary>原文 `TMenu_GetTag = function(MenuItem: _TMenuItem): Integer; stdcall`（原 :144-145）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TMenu_GetTag(IMenuItem menuItem);

/// <summary>原文 `TMenu_SetTag = procedure(MenuItem: _TMenuItem; Value: Integer); stdcall`（原 :148-149）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TMenu_SetTag(IMenuItem menuItem, int value);

// ---------------- TClientContext 客户端会话（原 :156-249） ----------------

/// <summary>原文 `TClientContext_GetContextID = function(ClientContext: _TClientContext): Integer; stdcall`（原 :156-157）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetContextID(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetIPValue = function(ClientContext: _TClientContext): DWORD; stdcall`（原 :160-161）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TClientContext_GetIPValue(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetIpAddr = function(ClientContext: _TClientContext; Dest: PAnsiChar;
///   var DestLen: DWORD): BOOL; stdcall`（原 :164-165）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetIpAddr(IIocpClientContext clientContext, byte[] dest, ref uint destLen);

/// <summary>原文 `TClientContext_GetPort = function(ClientContext: _TClientContext): Word; stdcall`（原 :168-169）。
/// 注意：Delphi `Word` 为 2 字节，C# `ushort` 对齐 —— 函数返回值在 stdcall 下按 32 位返回寄存器传递，
/// 但字段宽度语义保持 ushort。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate ushort TClientContext_GetPort(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_IsOldClient = function(ClientContext: _TClientContext): BOOL; stdcall`（原 :172-173）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_IsOldClient(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_IsLoginNotice = ...`（原 :176-177）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_IsLoginNotice(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_IsPlayGame = ...`（原 :180-181）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_IsPlayGame(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetRecogId = function(ClientContext: _TClientContext): Int64; stdcall`（原 :184-185）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate long TClientContext_GetRecogId(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetAccount = ...`（原 :188-189）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetAccount(IIocpClientContext clientContext, byte[] dest, ref uint destLen);

/// <summary>原文 `TClientContext_GetChrName = ...`（原 :192-193）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetChrName(IIocpClientContext clientContext, byte[] dest, ref uint destLen);

/// <summary>原文 `TClientContext_GetMachineID = ...`（原 :196-197）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetMachineID(IIocpClientContext clientContext, byte[] dest, ref uint destLen);

/// <summary>原文 `TClientContext_GetJob = function(ClientContext: _TClientContext): Byte; stdcall`（原 :200-201）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate byte TClientContext_GetJob(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetMoveSpeed = ...`（原 :204-205）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetMoveSpeed(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetAttackSpeed = ...`（原 :208-209）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetAttackSpeed(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetSpellSpeed = ...`（原 :212-213）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_GetSpellSpeed(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_SendToClientMsg = procedure(ClientContext: _TClientContext;
///   Msg: PTDefaultMessage; AddData: PAnsiChar; AddDataLen: Integer); stdcall`（原 :216-217）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TClientContext_SendToClientMsg(IIocpClientContext clientContext, IntPtr msg, byte[] addData, int addDataLen);

/// <summary>原文 `TClientContext_SendToM2ServerMsg = ...`（原 :220-221）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TClientContext_SendToM2ServerMsg(IIocpClientContext clientContext, IntPtr msg, byte[] addData, int addDataLen);

/// <summary>原文 `TClientContext_SendToClientMsg_Ex = procedure(ClientContext: _TClientContext;
///   SendData: PAnsiChar; SendLen: Integer); stdcall`（原 :224-225，2019-04-18 作废）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TClientContext_SendToClientMsg_Ex(IIocpClientContext clientContext, byte[] sendData, int sendLen);

/// <summary>原文 `TClientContext_SendToM2ServerMsg_Ex = ...`（原 :228-229，2019-04-18 作废）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TClientContext_SendToM2ServerMsg_Ex(IIocpClientContext clientContext, byte[] sendData, int sendLen);

/// <summary>原文 `TClientContext_Close = procedure(ClientContext: _TClientContext); stdcall`（原 :232-233）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TClientContext_Close(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_IsWaitClose = ...`（原 :236-237）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_IsWaitClose(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_LockUser = procedure(ClientContext: _TClientContext; LockTime: Integer); stdcall`（原 :240-241）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TClientContext_LockUser(IIocpClientContext clientContext, int lockTime);

/// <summary>原文 `TClientContext_IsSendClientDllComplete = ...`（原 :244-245）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TClientContext_IsSendClientDllComplete(IIocpClientContext clientContext);

/// <summary>原文 `TClientContext_GetSendClientDllTick = function(ClientContext: _TClientContext): DWORD; stdcall`（原 :248-249）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate uint TClientContext_GetSendClientDllTick(IIocpClientContext clientContext);

// ---------------- TRunGate 网关函数（原 :256-321） ----------------

/// <summary>原文 `TRunGate_GetAppPath = function(Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall`（原 :256-257）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_GetAppPath(byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_GetAppFileName = ...`（原 :260-261）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_GetAppFileName(byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_GetPluginPath = ...`（原 :264-265）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_GetPluginPath(byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_GetPluginFileName = ...`（原 :268-269）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_GetPluginFileName(byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_AddMainLogMsg = procedure(Msg: PAnsiChar; nLevel: Integer); stdcall`（原 :272-273）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TRunGate_AddMainLogMsg(byte[] msg, int nLevel);

/// <summary>原文 `TRunGate_EncodeMessage = function(Msg: PTDefaultMessage; Dest: PAnsiChar;
///   var DestLen: DWORD): BOOL; stdcall`（原 :276-277）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_EncodeMessage(IntPtr msg, byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_DecodeMessage = function(Src: PAnsiChar; SrcLen: DWORD;
///   Msg: PTDefaultMessage): BOOL; stdcall`（原 :280-281）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_DecodeMessage(byte[] src, uint srcLen, IntPtr msg);

/// <summary>原文 `TRunGate_EncodeBuffer = function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar;
///   var DestLen: DWORD): BOOL; stdcall`（原 :284-285）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_EncodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_DecodeBuffer = ...`（原 :288-289）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_DecodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_ZLibEncodeBuffer = ...`（原 :292-293）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_ZLibEncodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_ZLibDecodeBuffer = ...`（原 :296-297）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_ZLibDecodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);

/// <summary>原文 `TRunGate_GetOnlineContextList = function(List: _TList): Integer; stdcall`（原 :300-301）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TRunGate_GetOnlineContextList(IListHandle list);

/// <summary>原文 `TRunGate_AddTempBlockIP = procedure(IP: PAnsiChar); stdcall`（原 :304-305）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TRunGate_AddTempBlockIP(byte[] ip);

/// <summary>原文 `TRunGate_AddBlockIP = procedure(IP: PAnsiChar); stdcall`（原 :308-309）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TRunGate_AddBlockIP(byte[] ip);

/// <summary>原文 `TRunGate_AddTempBlockMac = procedure(Mac: PAnsiChar); stdcall`（原 :312-313）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TRunGate_AddTempBlockMac(byte[] mac);

/// <summary>原文 `TRunGate_AddBlockMac = procedure(Mac: PAnsiChar); stdcall`（原 :316-317）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TRunGate_AddBlockMac(byte[] mac);

/// <summary>原文 `TRunGate_NotifyClientDllReload = procedure(); stdcall`（原 :320-321）。</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TRunGate_NotifyClientDllReload();

// =====================================================================================
// 函数指针表（原文 record，全部 packed / 顺序敏感）
// =====================================================================================

/// <summary>原文 TListFunc（原 :324-340）。`Reserved: array[0..19] of Pointer`。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TListFunc
{
    public TList_Create Create;                       // 原 :325 列表创建
    public TList_Free Free;                           // 原 :326 列表释放
    public TList_Count Count;                         // 原 :327 取列表数量
    public TList_Clear Clear;                         // 原 :328 清空列表
    public TList_Add Add;                             // 原 :329 添加元素
    public TList_Insert Insert;                       // 原 :330 插入元素
    public TList_Remove Remove;                       // 原 :331 根据元素删除
    public TList_Delete Delete;                       // 原 :332 根据索引删除
    public TList_GetItem GetItem;                     // 原 :333 取得元素
    public TList_SetItem SetItem;                     // 原 :334 设置元素
    public TList_IndexOf IndexOf;                     // 原 :335 得到元素的索引
    public TList_Exchange Exchange;                   // 原 :336 交换元素
    public TList_CopyTo CopyTo;                       // 原 :337 复制到另一个列表
    public fixed long Reserved[20];                   // 原 :339 array[0..19] of Pointer
}

/// <summary>原文 TMemuFunc（原 :342-363，注意原文拼写 Memu 而非 Menu，照抄）。
/// `Reserved: array[0..19] of Pointer`。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMemuFunc
{
    public TMenu_Count Count;                         // 原 :343
    public TMenu_GetItems GetItems;                   // 原 :344
    public TMenu_Add Add;                             // 原 :345
    public TMenu_Insert Insert;                       // 原 :346
    public TMenu_GetCaption GetCaption;               // 原 :347
    public TMenu_SetCaption SetCaption;               // 原 :348
    public TMenu_GetEnabled GetEnabled;               // 原 :349
    public TMenu_SetEnabled SetEnabled;               // 原 :350
    public TMenu_GetVisable GetVisable;               // 原 :351（原文拼写 Visable，照抄）
    public TMenu_SetVisable SetVisable;               // 原 :352
    public TMenu_GetChecked GetChecked;               // 原 :353
    public TMenu_SetChecked SetChecked;               // 原 :354
    public TMenu_GetRadioItem GetRadioItem;           // 原 :355
    public TMenu_SetRadioItem SetRadioItem;           // 原 :356
    public TMenu_GetGroupIndex GetGroupIndex;         // 原 :357
    public TMenu_SetGroupIndex SetGroupIndex;         // 原 :358
    public TMenu_GetTag GetTag;                       // 原 :359
    public TMenu_SetTag SetTag;                       // 原 :360
    public fixed long Reserved[20];                   // 原 :362
}

/// <summary>原文 TClientContextFunc（原 :365-396）。`Reserved: array[0..39] of Pointer`。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientContextFunc
{
    public TClientContext_GetContextID GetContextID;               // 原 :366
    public TClientContext_GetIPValue GetIPValue;                   // 原 :367
    public TClientContext_GetIpAddr GetIpAddr;                     // 原 :368
    public TClientContext_GetPort GetPort;                         // 原 :369
    public TClientContext_IsOldClient IsOldClient;                 // 原 :370
    public TClientContext_IsLoginNotice IsLoginNotice;             // 原 :371
    public TClientContext_IsPlayGame IsPlayGame;                   // 原 :372
    public TClientContext_GetRecogId GetRecogId;                   // 原 :373
    public TClientContext_GetAccount GetAccount;                   // 原 :374
    public TClientContext_GetChrName GetChrName;                   // 原 :375
    public TClientContext_GetMachineID GetMachineID;               // 原 :376
    public TClientContext_GetJob GetJob;                           // 原 :377
    public TClientContext_GetMoveSpeed GetMoveSpeed;               // 原 :378
    public TClientContext_GetAttackSpeed GetAttackSpeed;           // 原 :379
    public TClientContext_GetSpellSpeed GetSpellSpeed;             // 原 :380
    public TClientContext_SendToClientMsg SendToClientMsg;         // 原 :382
    public TClientContext_SendToM2ServerMsg SendToM2ServerMsg;     // 原 :383
    public TClientContext_SendToClientMsg_Ex SendToClientMsg_Ex;         // 原 :385（2019-04-18 作废）
    public TClientContext_SendToM2ServerMsg_Ex SendToM2ServerMsg_Ex;     // 原 :386（2019-04-18 作废）
    public TClientContext_Close Close;                             // 原 :388
    public TClientContext_IsWaitClose IsWaitClose;                 // 原 :389
    public TClientContext_LockUser LockUser;                       // 原 :390
    public TClientContext_IsSendClientDllComplete IsSendClientDllComplete;   // 原 :392
    public TClientContext_GetSendClientDllTick GetSendClientDllTick;         // 原 :393
    public fixed long Reserved[40];                                // 原 :395
}

/// <summary>原文 TRunGateFunc（原 :398-422）。`Reserved: array[0..69] of Pointer`。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TRunGateFunc
{
    public TRunGate_GetAppPath GetAppPath;                         // 原 :399
    public TRunGate_GetAppFileName GetAppFileName;                 // 原 :400
    public TRunGate_GetPluginPath GetPluginPath;                   // 原 :401
    public TRunGate_GetPluginFileName GetPluginFileName;           // 原 :402
    public TRunGate_AddMainLogMsg AddMainLogMsg;                   // 原 :404
    public TRunGate_EncodeMessage EncodeMessage;                   // 原 :405
    public TRunGate_DecodeMessage DecodeMessage;                   // 原 :406
    public TRunGate_EncodeBuffer EncodeBuffer;                     // 原 :407
    public TRunGate_DecodeBuffer DecodeBuffer;                     // 原 :408
    public TRunGate_ZLibEncodeBuffer ZLibEncodeBuffer;             // 原 :409
    public TRunGate_ZLibDecodeBuffer ZLibDecodeBuffer;             // 原 :410
    public TRunGate_GetOnlineContextList GetOnlineContextList;     // 原 :412
    public TRunGate_AddTempBlockIP AddTempBlockIP;                 // 原 :414
    public TRunGate_AddBlockIP AddBlockIP;                         // 原 :415
    public TRunGate_AddTempBlockMac AddTempBlockMac;               // 原 :416
    public TRunGate_AddBlockMac AddBlockMac;                       // 原 :417
    public TRunGate_NotifyClientDllReload NotifyClientDllReload;   // 原 :419
    public fixed long Reserved[70];                                // 原 :421
}

/// <summary>原文 PAppFuncDef / TAppFuncDef（原 :424-435）。`Reserved: array[0..999] of Pointer`。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAppFuncDef
{
    public int PluginID;              // 原 :426 插件ID
    public TListFunc List;            // 原 :428 列表
    public TMemuFunc Menu;            // 原 :429 菜单
    public TClientContextFunc Context; // 原 :430 客户端
    public TRunGateFunc RunGate;      // 原 :432 网关函数
    public fixed long Reserved[1000]; // 原 :434
}

// =====================================================================================
// 托管接口层（与上面的函数指针表成员一一对应）
// =====================================================================================

/// <summary>
/// 托管侧 `_TList`（原文 `_TList = TList`，原 :11）。
/// 接缝：待 IocpTcpServer/MudUtil 的 TList 移植后接入（当前仅定义最小只读/可变面）。
/// </summary>
public interface IListHandle
{
    int Count { get; }
    void Clear();
    void Add(object item);
    void Insert(int index, object item);
    void Remove(object item);
    void Delete(int index);
    object GetItem(int index);
    void SetItem(int index, object item);
    int IndexOf(object item);
    void Exchange(int index1, int index2);
    void CopyTo(IListHandle dest);
}

/// <summary>
/// 托管侧 `_TMenuItem`（原文 `_TMenuItem = TMenuItem`，原 :10）。
/// 接缝：待 VCL TMenuItem 的 WinForms 对应物（System.Windows.Forms.ToolStripMenuItem）接入后继承。
/// </summary>
public interface IMenuItem
{
    int Count { get; }
    IMenuItem GetItems(int index);
    string Caption { get; set; }
    bool Enabled { get; set; }
    bool Visable { get; set; }
    bool Checked { get; set; }
    bool RadioItem { get; set; }
    int GroupIndex { get; set; }
    int Tag { get; set; }
}

/// <summary>
/// 托管侧 `_TClientContext`（原文 `_TClientContext = TIocpClientContext`，原 :12）。
/// 接缝：待 IocpTcpServer 的 TIocpClientContext 移植后实现此接口（当前接口成员即原文 24 个取值/动作）。
/// </summary>
public interface IIocpClientContext
{
    int GetContextID();
    uint GetIPValue();
    bool GetIpAddr(byte[] dest, ref uint destLen);
    ushort GetPort();
    bool IsOldClient();
    bool IsLoginNotice();
    bool IsPlayGame();
    long GetRecogId();
    bool GetAccount(byte[] dest, ref uint destLen);
    bool GetChrName(byte[] dest, ref uint destLen);
    bool GetMachineID(byte[] dest, ref uint destLen);
    byte GetJob();
    int GetMoveSpeed();
    int GetAttackSpeed();
    int GetSpellSpeed();
    void SendToClientMsg(IntPtr msg, byte[] addData, int addDataLen);
    void SendToM2ServerMsg(IntPtr msg, byte[] addData, int addDataLen);
    void SendToClientMsg_Ex(byte[] sendData, int sendLen);
    void SendToM2ServerMsg_Ex(byte[] sendData, int sendLen);
    void Close();
    bool IsWaitClose();
    void LockUser(int lockTime);
    bool IsSendClientDllComplete();
    uint GetSendClientDllTick();
}

/// <summary>
/// 托管侧 `TListFunc` 记录对应的接口（供托管插件实现；成员与 TListFunc 字段一一对应）。
/// </summary>
public interface IListFunc
{
    IListHandle Create();
    void Free(IListHandle list);
    int Count(IListHandle list);
    void Clear(IListHandle list);
    void Add(IListHandle list, IntPtr item);
    void Insert(IListHandle list, int index, IntPtr item);
    void Remove(IListHandle list, IntPtr item);
    void Delete(IListHandle list, int index);
    IntPtr GetItem(IListHandle list, int index);
    void SetItem(IListHandle list, int index, IntPtr item);
    int IndexOf(IListHandle list, IntPtr item);
    void Exchange(IListHandle list, int index1, int index2);
    void CopyTo(IListHandle source, IListHandle dest);
}

/// <summary>托管侧 `TMemuFunc` 记录对应的接口（原文拼写 Memu 照抄）。</summary>
public interface IMemuFunc
{
    int Count(IMenuItem menuItem);
    IMenuItem GetItems(IMenuItem menuItem, int index);
    IMenuItem Add(int plugId, IMenuItem menuItem, byte[] caption, int tag, Action<object> onClick);
    IMenuItem Insert(int plugId, IMenuItem menuItem, int index, byte[] caption, int tag, Action<object> onClick);
    bool GetCaption(IMenuItem menuItem, byte[] dest, ref uint destLen);
    void SetCaption(IMenuItem menuItem, byte[] caption);
    bool GetEnabled(IMenuItem menuItem);
    void SetEnabled(IMenuItem menuItem, bool enabled);
    bool GetVisable(IMenuItem menuItem);
    void SetVisable(IMenuItem menuItem, bool visible);
    bool GetChecked(IMenuItem menuItem);
    void SetChecked(IMenuItem menuItem, bool isChecked);
    bool GetRadioItem(IMenuItem menuItem);
    void SetRadioItem(IMenuItem menuItem, bool isRadioItem);
    int GetGroupIndex(IMenuItem menuItem);
    void SetGroupIndex(IMenuItem menuItem, int value);
    int GetTag(IMenuItem menuItem);
    void SetTag(IMenuItem menuItem, int value);
}

/// <summary>托管侧 `TClientContextFunc` 记录对应的接口。</summary>
public interface IClientContextFunc
{
    int GetContextID(IIocpClientContext clientContext);
    uint GetIPValue(IIocpClientContext clientContext);
    bool GetIpAddr(IIocpClientContext clientContext, byte[] dest, ref uint destLen);
    ushort GetPort(IIocpClientContext clientContext);
    bool IsOldClient(IIocpClientContext clientContext);
    bool IsLoginNotice(IIocpClientContext clientContext);
    bool IsPlayGame(IIocpClientContext clientContext);
    long GetRecogId(IIocpClientContext clientContext);
    bool GetAccount(IIocpClientContext clientContext, byte[] dest, ref uint destLen);
    bool GetChrName(IIocpClientContext clientContext, byte[] dest, ref uint destLen);
    bool GetMachineID(IIocpClientContext clientContext, byte[] dest, ref uint destLen);
    byte GetJob(IIocpClientContext clientContext);
    int GetMoveSpeed(IIocpClientContext clientContext);
    int GetAttackSpeed(IIocpClientContext clientContext);
    int GetSpellSpeed(IIocpClientContext clientContext);
    void SendToClientMsg(IIocpClientContext clientContext, IntPtr msg, byte[] addData, int addDataLen);
    void SendToM2ServerMsg(IIocpClientContext clientContext, IntPtr msg, byte[] addData, int addDataLen);
    void SendToClientMsg_Ex(IIocpClientContext clientContext, byte[] sendData, int sendLen);
    void SendToM2ServerMsg_Ex(IIocpClientContext clientContext, byte[] sendData, int sendLen);
    void Close(IIocpClientContext clientContext);
    bool IsWaitClose(IIocpClientContext clientContext);
    void LockUser(IIocpClientContext clientContext, int lockTime);
    bool IsSendClientDllComplete(IIocpClientContext clientContext);
    uint GetSendClientDllTick(IIocpClientContext clientContext);
}

/// <summary>托管侧 `TRunGateFunc` 记录对应的接口。</summary>
public interface IRunGateFunc
{
    bool GetAppPath(byte[] dest, ref uint destLen);
    bool GetAppFileName(byte[] dest, ref uint destLen);
    bool GetPluginPath(byte[] dest, ref uint destLen);
    bool GetPluginFileName(byte[] dest, ref uint destLen);
    void AddMainLogMsg(byte[] msg, int nLevel);
    bool EncodeMessage(IntPtr msg, byte[] dest, ref uint destLen);
    bool DecodeMessage(byte[] src, uint srcLen, IntPtr msg);
    bool EncodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);
    bool DecodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);
    bool ZLibEncodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);
    bool ZLibDecodeBuffer(byte[] src, uint srcLen, byte[] dest, ref uint destLen);
    int GetOnlineContextList(IListHandle list);
    void AddTempBlockIP(byte[] ip);
    void AddBlockIP(byte[] ip);
    void AddTempBlockMac(byte[] mac);
    void AddBlockMac(byte[] mac);
    void NotifyClientDllReload();
}

/// <summary>
/// 托管插件契约：对应原文"宿主把 TAppFuncDef 交给插件"的入口。
/// 原生插件原本从 `Package`/`RunGatePluginEntry` 导出函数获取该记录；
/// 托管侧以接口 + 返回 packed 结构表达（结构布局与原文 TAppFuncDef 完全一致）。
/// </summary>
public interface IRunGatePlugin
{
    /// <summary>原文插件入口（宿主填充 TAppFuncDef 后交给插件）。</summary>
    void GetAppFuncDef(ref TAppFuncDef appFuncDef);
}

/// <summary>原文 `PAppFuncDef = ^TAppFuncDef`（原 :424）的托管等价物：装箱引用载体。</summary>
public sealed class PAppFuncDef
{
    public TAppFuncDef Value;

    public PAppFuncDef() { }
    public PAppFuncDef(TAppFuncDef value) { Value = value; }
}

/// <summary>本单元 ABI 常量（原文未定义常量，仅类型；此处集中记录布局期望值以便测试断言）。</summary>
public static class RunGatePluginInterfaceLayout
{
    /// <summary>Pointer 宽度（32 位宿主为 4，64 位宿主为 8）。</summary>
    public static int PointerSize => IntPtr.Size;

    /// <summary>TAppFuncDef 大小 = 4(PluginID) + (13+20)*P + (18+20)*P + (24+40)*P + (17+70)*P + 1000*P。</summary>
    public static int SizeOfTAppFuncDef =>
        4
        + (13 + 20) * PointerSize
        + (18 + 20) * PointerSize
        + (24 + 40) * PointerSize
        + (17 + 70) * PointerSize
        + 1000 * PointerSize;
}
