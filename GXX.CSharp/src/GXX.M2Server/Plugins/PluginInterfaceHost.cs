using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Plugins;

// =====================================================================================
// PluginImplement.pas 1:1 转换（Source/M2Engine/PluginImplement.pas，6321 行）。
//
// 原文是**宿主侧**实现：741 个 `_T*` 例程，由 PluginManager 的
// `{$I PluginFuncAssign.inc}` 逐个写进 `TAppFuncDef`（宿主 → 插件的回调表），
// 因此本单元**没有 exports 段**（全文件 0 处 `exports`），不是原生 DLL 导出表。
//
// 本文件实现"不依赖引擎内部状态"的那部分（内存/列表/字符串列表/内存流/菜单/INI/
// 引擎级全局量/地图与地图对象只读属性/编解码薄封装/BaseObject 纯字段访问器…），
// 逐条对照原行号；其余走 PluginInterfaceHost.Stubs.g.cs 的显式未完成壳。
//
// 两条接缝：
//   1) 引擎对象（TBaseObject/TPlayObject/TEnvirnoment/TGuild/TUserEngine…）由
//      PluginInterfaceSeams.cs 的接口表达，待对应单元移植后由真实类实现；
//   2) 引擎全局（g_Config / g_MapManager / UserEngine / FrmMain / g_version…）由
//      <see cref="IPluginHostEnv"/> 表达，待 M2Share/UsvrEngn/svMain 移植后接入。
// =====================================================================================

/// <summary>
/// 宿主环境接缝：原文 PluginImplement.pas 直接引用 M2Share/svMain/UsrEngn/Envir 等单元的
/// 全局量（g_Config、g_MapManager、UserEngine、FrmMain、g_version、g_buildtime…）。
/// 接缝：待 M2Share/UsrEngn/svMain/Envir 移植后由真实实现接入。
/// </summary>
public interface IPluginHostEnv
{
    /// <summary>原文 `g_version`（M2Share）。</summary>
    string Version { get; }
    /// <summary>原文 `g_buildtime`（M2Share），用于 `TM2Engine_GetVersionInt`。</summary>
    DateTime BuildTime { get; }
    /// <summary>原文 `ParamStr(0)` 所在目录（`TM2Engine_GetAppDir`）。</summary>
    string AppDir { get; }
    /// <summary>原文 `FrmMain.Handle`（`TM2Engine_GetMainFormHandle`）。</summary>
    IntPtr MainFormHandle { get; }
    /// <summary>原文 `MainOutMessage(Msg, IsAddTime)`。</summary>
    void MainOutMessage(string msg, bool isAddTime);
    /// <summary>原文 16 个文件/目录配置（`TM2Engine_GetOtherFileDir` 的 0..15）。</summary>
    string GetOtherFileDir(int m2FileType);
    /// <summary>原文 `g_Config.GlobaDyMval[Index]`（全局 I 变量）。</summary>
    int GetGlobalVarI(int index);
    void SetGlobalVarI(int index, int value);
    /// <summary>原文 `g_Config.GlobalVal[Index]`（全局 G 变量）。</summary>
    int GetGlobalVarG(int index);
    void SetGlobalVarG(int index, int value);
    /// <summary>原文 `g_Config.GlobalAVal[Index]`（全局 A 变量）。</summary>
    string GetGlobalVarA(int index);
    void SetGlobalVarA(int index, string value);
    /// <summary>原文 `sCaptionExtText`。</summary>
    void SetMainFormCaptionExt(string caption);
    /// <summary>原文 `g_MapManager.FindMap(MapName)`。</summary>
    IEnvirnoment? FindMap(string mapName);
    /// <summary>原文 `g_MapManager`（`TMapManager_GetMapList` 直接返回它自身）。</summary>
    IListHandle MapList { get; }
    /// <summary>原文 `Config`（!Setup.txt 的 TIniFile）。</summary>
    IIniFileHandle Config { get; }
    /// <summary>原文 `StringConf`（AnsiString.ini 的 TIniFile）。</summary>
    IIniFileHandle StringConf { get; }
    /// <summary>原文 `FrmMain.MainMenu.Items`（`TMenu_GetMainMenu`）。</summary>
    IMenuItem MainMenuItems { get; }
    /// <summary>原文 `FrmMain.mniControl / mniView / mniOption / mniManger / mniTools / mniHelp / mniPlugin`。</summary>
    IMenuItem GetFrameMenuItem(string field);
    /// <summary>原文 `g_PluginManager.Items[I]` 的 `NativeInt(TempPlug) = PlugID` 反查。</summary>
    bool PluginExists(IntPtr plugId);
    /// <summary>原文 `Plug.AddMenu(Item, NotifyEventMethod)`。</summary>
    void PluginAddMenu(IntPtr plugId, IMenuItem item, object? notifyEventMethod);
    /// <summary>原文 `UserEngine`（UsrEngn 单元）。</summary>
    IUserEngineSeam UserEngine { get; }
    /// <summary>原文 `g_Config.nItemExpRate` 等 M2Share 全局配置读取的通用入口。</summary>
    int GetConfigInt(string field);
    /// <summary>原文 `uMagicACUtils.g_MagicACList`。</summary>
    IMagicACListHandle MagicACList { get; }
    /// <summary>`TM2Engine_GetTakeOnPosition(StdMode)`（M2Share 的 GetTakeOnPosition）。</summary>
    int GetTakeOnPosition(int stdMode);
    /// <summary>`TM2Engine_CheckBindType`（M2Share 的 GetUserItemBindValue）。</summary>
    bool GetUserItemBindValue(byte bindValue, int bindType);
    /// <summary>`TM2Engine_SetBindValue`（M2Share 的 SetUserItemBindValue）。</summary>
    void SetUserItemBindValue(ref byte bindValue, int bindType, bool value);
    /// <summary>`TM2Engine_GetRGB`（M2Share 的 GetRGB）。</summary>
    uint GetRGB(byte color);
}

/// <summary>接缝：UsrEngn 的 TUserEngine（原文 `UserEngine`）。</summary>
public interface IUserEngineSeam
{
    IStringListHandle PlayerList { get; }
    IPlayObjectHandle? GetPlayerByName(string chrName);
    IPlayObjectHandle? GetPlayerByUserID(string userId);
    IPlayObjectHandle? GetPlayerByObject(object aObject);
    IPlayObjectHandle? GetOfflinePlayer(string userId);
    void KickPlayer(string chrName);
    IStringListHandle HeroList { get; }
    IHeroObjectHandle? GetHeroByName(string chrName);
    bool KickHero(string chrName);
    IListHandle MerchantList { get; }
    IListHandle CustomNpcConfigList { get; }
    IStringListHandle QuestNPCList { get; }
    INormNpcHandle? ManageNPC { get; }
    INormNpcHandle? FunctionNPC { get; }
    INormNpcHandle? RobotNPC { get; }
    INormNpcHandle? MissionNPCObject { get; }
    INormNpcHandle? FindMerchant(object aObject);
    INormNpcHandle? FindMerchantByPos(string mapName, int x, int y);
    INormNpcHandle? FindQuestNPC(object aObject);
    IListHandle MagicList { get; }
    IListHandle CustomMagicConfigList { get; }
    IListHandle StdItemList { get; }
    IListHandle MonsterList { get; }
    int AllDummyCount { get; }
    int GetMapDummyCount(IEnvirnoment envir);
    int OfflineCount { get; }
    int RealPlayerCount { get; }
}

/// <summary>
/// PluginImplement.pas 的托管实现（原文 741 个 `_T*` 例程的载体）。
/// 原文没有 `class`，全是单元级 routine；托管侧统一挂到本类，句柄即 `this`。
/// </summary>
public sealed partial class PluginInterfaceHost
{
    private readonly IPluginHostEnv _env;
    private readonly Dictionary<IntPtr, TStringListHandle> _strLists = new();
    private readonly Dictionary<IntPtr, TMemoryStreamHandle> _memStreams = new();
    private readonly Dictionary<IntPtr, TIniFileHandle> _iniFiles = new();
    private IntPtr _nextHandle = 1;

    public PluginInterfaceHost(IPluginHostEnv env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    /// <summary>宿主环境接缝（供 PluginInterfaceHost.Stubs.g.cs 的未完成壳引用）。</summary>
    public IPluginHostEnv Env => _env;

    private IntPtr NewHandle() => (IntPtr)(_nextHandle++);

    private TStringListHandle GetStrList(IntPtr h) =>
        _strLists.TryGetValue(h, out var v) ? v : throw new InvalidOperationException($"invalid TStringList handle {h}");

    private TMemoryStreamHandle GetMemStream(IntPtr h) =>
        _memStreams.TryGetValue(h, out var v) ? v : throw new InvalidOperationException($"invalid TMemoryStream handle {h}");

    private TIniFileHandle GetIniFile(IntPtr h) =>
        _iniFiles.TryGetValue(h, out var v) ? v : throw new InvalidOperationException($"invalid TIniFile handle {h}");

    // =================================================================================
    // 原 :870-897  TMemory 内存管理
    // =================================================================================

    /// <summary>原文 `TMemory_Alloc`（原 :870）：`AllocMem(Size)`（Delphi 保证清零）。</summary>
    public IntPtr TMemory_Alloc(int pSize)
    {
        var result = IntPtr.Zero;
        try
        {
            // 原文如此（PluginImplement.pas:875）：AllocMem 等价于 AllocHGlobal + 清零
            result = System.Runtime.InteropServices.Marshal.AllocHGlobal(pSize);
            unsafe
            {
                var p = (byte*)result;
                for (var i = 0; i < pSize; i++) p[i] = 0;
            }
        }
        catch
        {
            // 原文如此（PluginImplement.pas:877）：except MainOutMessage('[Exception] Plugin AllocMem');
            _env.MainOutMessage("[Exception] Plugin AllocMem", true);
        }
        return result;
    }

    /// <summary>原文 `TMemory_Free`（原 :881）：`FreeMem(P)`。</summary>
    public void TMemory_Free(IntPtr P)
    {
        try
        {
            if (P != IntPtr.Zero) System.Runtime.InteropServices.Marshal.FreeHGlobal(P);
        }
        catch
        {
            _env.MainOutMessage("[Exception] Plugin FreeMem", true);
        }
    }

    /// <summary>
    /// 原文 `TMemory_Realloc`（原 :890）：`ReallocMem(P, Size)`。
    /// 原文如此（PluginImplement.pas:890）：该函数**只改大小、不会把新指针回写调用方**
    /// （参数不是 `var P`），因此原版对"搬家"情形本身就是有缺陷的；托管侧照抄该签名，
    /// 并按 ReAllocHGlobal 语义实现（新指针无法回写，属已知原生语义缺陷）。
    /// </summary>
    public void TMemory_Realloc(IntPtr P, int pSize)
    {
        try
        {
            _ = System.Runtime.InteropServices.Marshal.ReAllocHGlobal(P, pSize);
        }
        catch
        {
            _env.MainOutMessage("[Exception] Plugin ReallocMem", true);
        }
    }

    // =================================================================================
    // 原 :903-966  TList 对象列表
    // =================================================================================

    /// <summary>原文 `TList_Create`（原 :903）：`TList.Create`。</summary>
    public IListHandle TList_Create() => new TListHandle();

    /// <summary>原文 `TList_Free`（原 :908）：`List.Free`。托管侧由 GC 管理，此处仅清空引用。</summary>
    public void TList_Free(IListHandle pList) => pList.Clear();

    /// <summary>原文 `TList_Count`（原 :913）。</summary>
    public int TList_Count(IListHandle pList) => pList.Count;

    /// <summary>原文 `TList_Clear`（原 :918）。</summary>
    public void TList_Clear(IListHandle pList) => pList.Clear();

    /// <summary>原文 `TList_Add`（原 :923）。</summary>
    public void TList_Add(IListHandle pList, IntPtr pItem) => pList.Add(pItem);

    /// <summary>原文 `TList_Insert`（原 :928）。</summary>
    public void TList_Insert(IListHandle pList, int pIndex, IntPtr pItem) => pList.Insert(pIndex, pItem);

    /// <summary>原文 `TList_Remove`（原 :933）。</summary>
    public void TList_Remove(IListHandle pList, IntPtr pItem) => pList.Remove(pItem);

    /// <summary>原文 `TList_Delete`（原 :938）。</summary>
    public void TList_Delete(IListHandle pList, int pIndex) => pList.Delete(pIndex);

    /// <summary>原文 `TList_GetItem`（原 :943）。</summary>
    public IntPtr TList_GetItem(IListHandle pList, int pIndex) => pList.GetItem(pIndex);

    /// <summary>原文 `TList_SetItem`（原 :948）。</summary>
    public void TList_SetItem(IListHandle pList, int pIndex, IntPtr pItem) => pList.SetItem(pIndex, pItem);

    /// <summary>原文 `TList_IndexOf`（原 :953）。</summary>
    public int TList_IndexOf(IListHandle pList, IntPtr pItem) => pList.IndexOf(pItem);

    /// <summary>原文 `TList_Exchange`（原 :958）。</summary>
    public void TList_Exchange(IListHandle pList, int Index1, int Index2) => pList.Exchange(Index1, Index2);

    /// <summary>原文 `TList_CopyTo`（原 :963）：`Dest.Assign(Source)`。</summary>
    public void TList_CopyTo(IListHandle Source, IListHandle Dest) => Dest.CopyTo(Source);

    // =================================================================================
    // 原 :972-1153  TStringList 文本列表
    // =================================================================================

    /// <summary>原文 `TStrList_Create`（原 :972）。</summary>
    public IStringListHandle TStrList_Create() => new TStringListHandle();

    /// <summary>原文 `TStrList_Free`（原 :977）。</summary>
    public void TStrList_Free(IStringListHandle Strings) => Strings.Text = string.Empty;

    /// <summary>原文 `TStrList_GetCaseSensitive`（原 :982）。</summary>
    public int TStrList_GetCaseSensitive(IStringListHandle Strings) => Strings.CaseSensitive ? 1 : 0;

    /// <summary>原文 `TStrList_SetCaseSensitive`（原 :987）。</summary>
    public void TStrList_SetCaseSensitive(IStringListHandle Strings, int IsCaseSensitive) => Strings.CaseSensitive = IsCaseSensitive != 0;

    /// <summary>原文 `TStrList_GetSorted`（原 :992）。</summary>
    public int TStrList_GetSorted(IStringListHandle Strings) => Strings.Sorted ? 1 : 0;

    /// <summary>原文 `TStrList_SetSorted`（原 :997）。</summary>
    public void TStrList_SetSorted(IStringListHandle Strings, int Sorted) => Strings.Sorted = Sorted != 0;

    /// <summary>原文 `TStrList_GetDuplicates`（原 :1002）：`Result := Strings.Duplicates = dupAccept;`</summary>
    public int TStrList_GetDuplicates(IStringListHandle Strings) => Strings.Duplicates ? 1 : 0;

    /// <summary>原文 `TStrList_SetDuplicates`（原 :1007）：True→dupAccept，False→dupIgnore。</summary>
    public void TStrList_SetDuplicates(IStringListHandle Strings, int Duplicates) => Strings.Duplicates = Duplicates != 0;

    /// <summary>原文 `TStrList_Count`（原 :1015）。</summary>
    public int TStrList_Count(IStringListHandle Strings) => Strings.Count;

    /// <summary>
    /// 原文 `TStrList_GetText`（原 :1020-1033）。
    /// 原文如此（PluginImplement.pas:1026）：判定为 `DestLen &gt; Length(S)`（必须容纳结尾 0），
    /// 且**无论是否写入都执行 `DestLen := Length(S)`**。
    /// </summary>
    public int TStrList_GetText(IStringListHandle Strings, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Strings.Text, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TStrList_SetText`（原 :1035-1042）：按 SrcLen 定长取 GBK 文本。</summary>
    public void TStrList_SetText(IStringListHandle Strings, byte[]? Src, uint SrcLen)
        => Strings.Text = PluginHostText.ReadAnsiLen(Src, SrcLen);

    /// <summary>原文 `TList_Add`（原 :1044）。</summary>
    public void TStrList_Add(IStringListHandle Strings, byte[]? S) => Strings.Add(PluginHostText.ReadAnsi(S));

    /// <summary>原文 `TStrList_AddObject`（原 :1049）。</summary>
    public void TStrList_AddObject(IStringListHandle Strings, byte[]? S, object? AObject)
        => Strings.AddObject(PluginHostText.ReadAnsi(S), AObject);

    /// <summary>原文 `TList_Insert`（原 :1054）。</summary>
    public void TStrList_Insert(IStringListHandle Strings, int Index, byte[]? S)
        => Strings.Insert(Index, PluginHostText.ReadAnsi(S));

    /// <summary>原文 `TStrList_InsertObject`（原 :1059）。</summary>
    public void TStrList_InsertObject(IStringListHandle Strings, int Index, byte[]? S, object? AObject)
        => Strings.InsertObject(Index, PluginHostText.ReadAnsi(S), AObject);

    /// <summary>原文 `TList_Remove`（原 :1064）。</summary>
    public void TStrList_Remove(IStringListHandle Strings, byte[]? S) => Strings.Remove(PluginHostText.ReadAnsi(S));

    /// <summary>原文 `TStrList_Delete`（原 :1069）。</summary>
    public void TStrList_Delete(IStringListHandle Strings, int Index) => Strings.Delete(Index);

    /// <summary>原文 `TList_GetItem`（原 :1074-1081）：与 GetText 同样的缓冲判定。</summary>
    public int TStrList_GetItem(IStringListHandle Strings, int Index, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Strings.GetItem(Index), Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TList_SetItem`（原 :1084）。</summary>
    public void TStrList_SetItem(IStringListHandle Strings, int Index, byte[]? S)
        => Strings.SetItem(Index, PluginHostText.ReadAnsi(S));

    /// <summary>原文 `TStrList_GetObject`（原 :1087）。</summary>
    public object? TStrList_GetObject(IStringListHandle Strings, int Index) => Strings.GetObject(Index);

    /// <summary>原文 `TStrList_SetObject`（原 :1092）。</summary>
    public void TStrList_SetObject(IStringListHandle Strings, int Index, object? AObject) => Strings.SetObject(Index, AObject);

    /// <summary>原文 `TList_IndexOf`（原 :1095）。</summary>
    public int TStrList_IndexOf(IStringListHandle Strings, byte[]? S) => Strings.IndexOf(PluginHostText.ReadAnsi(S));

    /// <summary>原文 `TStrList_IndexOfObject`（原 :1100）。</summary>
    public int TStrList_IndexOfObject(IStringListHandle Strings, object? AObject) => Strings.IndexOfObject(AObject);

    /// <summary>原文 `TStrList_Find`（原 :1105）：二分查找（需已排序）。</summary>
    public int TStrList_Find(IStringListHandle Strings, byte[]? S, ref int Index) => Strings.Find(PluginHostText.ReadAnsi(S), ref Index) ? 1 : 0;

    /// <summary>原文 `TStrList_Exchange`（原 :1110）。</summary>
    public void TStrList_Exchange(IStringListHandle Strings, int Index1, int Index2) => Strings.Exchange(Index1, Index2);

    /// <summary>
    /// 原文 `TStrList_LoadFromFile`（原 :1139）。
    /// 原文如此（PluginInterface.pas:255）：procedural type 名拼作 `TStrLit_LoadFromFile`
    /// （Lit 而非 List），而实现体在 PluginImplement.pas:1139 写作 `TStrList_LoadFromFile`
    /// ——两处拼写不一致，本层按实现体命名，并在 PluginInterfaceTypes.g.cs 保留原文类型名。
    /// </summary>
    public void TStrList_LoadFromFile(IStringListHandle Strings, byte[]? FileName)
        => Strings.LoadFromFile(PluginHostText.ReadAnsi(FileName));

    /// <summary>原文 `TStrList_SaveToFile`（原 :1145）；类型名拼写同上（原文如此）。</summary>
    public void TStrList_SaveToFile(IStringListHandle Strings, byte[]? FileName)
        => Strings.SaveToFile(PluginHostText.ReadAnsi(FileName));

    /// <summary>原文 `TStrList_CopyTo`（原 :1150）：`Dest.Assign(Source)`。</summary>
    public void TStrList_CopyTo(IStringListHandle Source, IStringListHandle Dest) => Dest.CopyTo(Source);

    // =================================================================================
    // 原 :1159-1222  TMemoryStream 内存流
    // =================================================================================

    /// <summary>原文 `TMemStream_Create`（原 :1159）。</summary>
    public IMemoryStreamHandle TMemStream_Create()
    {
        var s = new TMemoryStreamHandle();
        _memStreams[NewHandle()] = s;
        return s;
    }

    /// <summary>
    /// 原文 `TMemStream_Free`（原 :1164）：`Stream.Free`。
    /// 托管侧由 GC 管理，这里只把内容清空（保留与原文"释放后不可再用"一致的可观察行为）。
    /// </summary>
    public void TMemStream_Free(IMemoryStreamHandle pStream) => pStream.Clear();

    /// <summary>原文 `TMemStream_GetSize`（原 :1169）。</summary>
    public long TMemStream_GetSize(IMemoryStreamHandle pStream) => pStream.Size;

    /// <summary>原文 `TMemStream_SetSize`（原 :1174）。</summary>
    public void TMemStream_SetSize(IMemoryStreamHandle pStream, int NewSize) => pStream.SetSize(NewSize);

    /// <summary>原文 `TMemStream_Clear`（原 :1179）。</summary>
    public void TMemStream_Clear(IMemoryStreamHandle pStream) => pStream.Clear();

    /// <summary>原文 `TMemStream_Read`（原 :1184）。</summary>
    public int TMemStream_Read(IMemoryStreamHandle pStream, byte[]? Buffer, int pCount)
        => Buffer is null ? 0 : pStream.Read(Buffer, pCount);

    /// <summary>原文 `TMemStream_Write`（原 :1189）。</summary>
    public int TMemStream_Write(IMemoryStreamHandle pStream, byte[]? Buffer, int pCount)
        => Buffer is null ? 0 : pStream.Write(Buffer, pCount);

    /// <summary>原文 `TMemStream_Seek`（原 :1194）。</summary>
    public int TMemStream_Seek(IMemoryStreamHandle pStream, int Offset, ushort Origin) => pStream.Seek(Offset, Origin);

    /// <summary>原文 `TMemStream_Memory`（原 :1199）。</summary>
    public IntPtr TMemStream_Memory(IMemoryStreamHandle pStream) => pStream.Memory;

    /// <summary>原文 `TMemStream_GetPosition`（原 :1204）。</summary>
    public long TMemStream_GetPosition(IMemoryStreamHandle pStream) => pStream.Position;

    /// <summary>原文 `TMemStream_SetPosition`（原 :1209）。</summary>
    public void TMemStream_SetPosition(IMemoryStreamHandle pStream, long Position) => pStream.Position = Position;

    /// <summary>原文 `TMemStream_LoadFromFile`（原 :1214）。</summary>
    public void TMemStream_LoadFromFile(IMemoryStreamHandle pStream, byte[]? FileName)
        => pStream.LoadFromFile(PluginHostText.ReadAnsi(FileName));

    /// <summary>原文 `TMemStream_SaveToFile`（原 :1219）。</summary>
    public void TMemStream_SaveToFile(IMemoryStreamHandle pStream, byte[]? FileName)
        => pStream.SaveToFile(PluginHostText.ReadAnsi(FileName));

    // =================================================================================
    // 原 :1229-1501  TMenu 菜单
    // =================================================================================

    /// <summary>原文 `TMenu_GetMainMenu`（原 :1229）：`FrmMain.MainMenu.Items`。</summary>
    public IMenuItem? TMenu_GetMainMenu() => _env.MainMenuItems;

    /// <summary>原文 `TMenu_GetControlMenu`（原 :1235）：`FrmMain.mniControl`。</summary>
    public IMenuItem? TMenu_GetControlMenu() => _env.GetFrameMenuItem("mniControl");

    /// <summary>原文 `TMenu_GetViewMenu`（原 :1241）：`FrmMain.mniView`。</summary>
    public IMenuItem? TMenu_GetViewMenu() => _env.GetFrameMenuItem("mniView");

    /// <summary>原文 `TMenu_GetOptionMenu`（原 :1247）：`FrmMain.mniOption`。</summary>
    public IMenuItem? TMenu_GetOptionMenu() => _env.GetFrameMenuItem("mniOption");

    /// <summary>原文 `TMenu_GetManagerMenu`（原 :1253）：`FrmMain.mniManger`（原文拼写如此）。</summary>
    public IMenuItem? TMenu_GetManagerMenu() => _env.GetFrameMenuItem("mniManger");

    /// <summary>原文 `TMenu_GetToolsMenu`（原 :1259）：`FrmMain.mniTools`。</summary>
    public IMenuItem? TMenu_GetToolsMenu() => _env.GetFrameMenuItem("mniTools");

    /// <summary>原文 `TMenu_GetHelpMenu`（原 :1265）：`FrmMain.mniHelp`。</summary>
    public IMenuItem? TMenu_GetHelpMenu() => _env.GetFrameMenuItem("mniHelp");

    /// <summary>原文 `TMenu_GetPluginMenu`（原 :1271）：`FrmMain.mniPlugin`。</summary>
    public IMenuItem? TMenu_GetPluginMenu() => _env.GetFrameMenuItem("mniPlugin");

    /// <summary>原文 `TMenu_Count`（原 :1277）。</summary>
    public int TMenu_Count(IMenuItem MenuItem) => MenuItem.Count;

    /// <summary>原文 `TMenu_GetItems`（原 :1283）：`MenuItem.Items[Index]`。</summary>
    public IMenuItem TMenu_GetItems(IMenuItem MenuItem, int pIndex) => MenuItem.GetItems(pIndex);

    /// <summary>
    /// 原文 `TNotifyEventMethod`（PluginImplement.pas:41-46）——`Click: TNotifyEventEx; Sender: TObject;`
    /// 的托管承载（见 <see cref="PluginMenuNotify"/>）。
    /// 原文 `NotifyEventEx`（原 :1288-1293）用 VCL `TMethod`（Code=@NotifyEventEx, Data=NotifyEventMethod）
    /// 把两者绑成一个 `TNotifyEvent`；托管侧没有"方法指针 + 数据指针"（属**无法等价的原生语义**），
    /// 改为直接持有 `PluginMenuNotify` 回调，触发时机与 Sender 参数一致。
    /// </summary>
    public static void NotifyEventEx(object? sender)
    {
        if (sender is null) return;
        if (sender is PluginMenuNotify notify) notify.Click(notify.Sender);
    }

    /// <summary>
    /// 原文 `TMenu_Add`（原 :1296-1352）。
    /// 原文按 `NativeInt(TempPlug) = PlugID` 反查插件，找不到就 `raise Exception.Create('PlugID error')`。
    /// </summary>
    public IMenuItem? TMenu_Add(IntPtr PlugID, IMenuItem? MenuItem, byte[]? Caption, int Tag, TNotifyEventEx? OnClick)
    {
        if (!_env.PluginExists(PlugID))
        {
            // 原文如此（PluginImplement.pas:1318）：raise Exception.Create('PlugID error');
            throw new InvalidOperationException("PlugID error");
        }

        // 原文如此（PluginImplement.pas:1321）：Item := TMenuItem.Create(MenuItem)
        // ——MenuItem 为 nil 时建在主菜单下，否则建在该菜单项下。
        var item = MenuItem is null ? _env.MainMenuItems : MenuItem.GetItems(0);
        item.Caption = PluginHostText.ReadAnsi(Caption);
        item.Tag = Tag;

        object? method = null;
        if (OnClick is not null) method = new PluginMenuNotify(OnClick, item);
        _env.PluginAddMenu(PlugID, item, method);
        return item;
    }

    /// <summary>原文 `TMenu_Insert`（原 :1355-1407）：与 Add 同构，差别是 `Insert(Index, Item)`。</summary>
    public IMenuItem? TMenu_Insert(IntPtr PlugID, IMenuItem? MenuItem, int Index, byte[]? Caption, int Tag, TNotifyEventEx? OnClick)
    {
        if (!_env.PluginExists(PlugID))
        {
            throw new InvalidOperationException("PlugID error");
        }

        // 原文如此（PluginImplement.pas:1380）：TMenuItem.Create(FrmMain.MainMenu)
        var item = MenuItem is null ? _env.MainMenuItems : MenuItem.GetItems(0);
        item.Caption = PluginHostText.ReadAnsi(Caption);
        item.Tag = Tag;

        object? method = null;
        if (OnClick is not null) method = new PluginMenuNotify(OnClick, item);
        _env.PluginAddMenu(PlugID, item, method);
        return item;
    }

    /// <summary>原文 `TMenu_GetCaption`（原 :1410-1423）。</summary>
    public int TMenu_GetCaption(IMenuItem MenuItem, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(MenuItem.Caption, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TMenu_SetCaption`（原 :1426）。</summary>
    public void TMenu_SetCaption(IMenuItem MenuItem, byte[]? Caption) => MenuItem.Caption = PluginHostText.ReadAnsi(Caption);

    /// <summary>原文 `TMenu_GetEnabled`（原 :1432）。</summary>
    public int TMenu_GetEnabled(IMenuItem MenuItem) => MenuItem.Enabled ? 1 : 0;

    /// <summary>原文 `TMenu_SetEnabled`（原 :1438）。</summary>
    public void TMenu_SetEnabled(IMenuItem MenuItem, int Enabled) => MenuItem.Enabled = Enabled != 0;

    /// <summary>原文 `TMenu_GetVisable`（原 :1444）：注意属性名是 VCL 的 `Visible`，接口成员按原文拼作 Visable。</summary>
    public int TMenu_GetVisable(IMenuItem MenuItem) => MenuItem.Visable ? 1 : 0;

    /// <summary>原文 `TMenu_SetVisable`（原 :1450）。</summary>
    public void TMenu_SetVisable(IMenuItem MenuItem, int Visible) => MenuItem.Visable = Visible != 0;

    /// <summary>原文 `TMenu_GetChecked`（原 :1456）。</summary>
    public int TMenu_GetChecked(IMenuItem MenuItem) => MenuItem.Checked ? 1 : 0;

    /// <summary>原文 `TMenu_SetChecked`（原 :1462）。</summary>
    public void TMenu_SetChecked(IMenuItem MenuItem, int Checked) => MenuItem.Checked = Checked != 0;

    /// <summary>原文 `TMenu_GetRadioItem`（原 :1468）。</summary>
    public int TMenu_GetRadioItem(IMenuItem MenuItem) => MenuItem.RadioItem ? 1 : 0;

    /// <summary>原文 `TMenu_SetRadioItem`（原 :1474）。</summary>
    public void TMenu_SetRadioItem(IMenuItem MenuItem, int IsRadioItem) => MenuItem.RadioItem = IsRadioItem != 0;

    /// <summary>原文 `TMenu_GetGroupIndex`（原 :1480）。</summary>
    public int TMenu_GetGroupIndex(IMenuItem MenuItem) => MenuItem.GroupIndex;

    /// <summary>原文 `TMenu_SetGroupIndex`（原 :1486）。</summary>
    public void TMenu_SetGroupIndex(IMenuItem MenuItem, int Value) => MenuItem.GroupIndex = Value;

    /// <summary>原文 `TMenu_GetTag`（原 :1492）。</summary>
    public int TMenu_GetTag(IMenuItem MenuItem) => MenuItem.Tag;

    /// <summary>原文 `TMenu_SetTag`（原 :1498）。</summary>
    public void TMenu_SetTag(IMenuItem MenuItem, int Value) => MenuItem.Tag = Value;

    // =================================================================================
    // 原 :1506-1565  TIniFile
    // =================================================================================

    /// <summary>原文 `TIniFile_Create`（原 :1506）：`TIniFile.Create(sFileName)`。</summary>
    public IIniFileHandle TIniFile_Create(byte[]? sFileName)
    {
        var ini = new TIniFileHandle(PluginHostText.ReadAnsi(sFileName));
        _iniFiles[NewHandle()] = ini;
        return ini;
    }

    /// <summary>原文 `TIniFile_Free`（原 :1511）：`IniFile.Free`。</summary>
    public void TIniFile_Free(IIniFileHandle IniFile)
    {
        // 托管侧：TIniFileHandle 每次写操作即落盘，无需释放
    }

    /// <summary>原文 `TIniFile_SectionExists`（原 :1516）。</summary>
    public int TIniFile_SectionExists(IIniFileHandle IniFile, byte[]? Section)
        => IniFile.SectionExists(PluginHostText.ReadAnsi(Section)) ? 1 : 0;

    /// <summary>原文 `TIniFile_ValueExists`（原 :1521）。</summary>
    public int TIniFile_ValueExists(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident)
        => IniFile.ValueExists(PluginHostText.ReadAnsi(Section), PluginHostText.ReadAnsi(Ident)) ? 1 : 0;

    /// <summary>
    /// 原文 `TIniFile_ReadString`（原 :1526-1540）。
    /// 原文如此（PluginImplement.pas:1533）：多一个 `Length(S) &gt; 0` 条件——**空串永不写、Result 恒 False**，
    /// 这与其他 Get* 系（如 :1026）不同，照抄。
    /// </summary>
    public int TIniFile_ReadString(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident, byte[]? Default, byte[]? Dest, ref uint DestLen)
    {
        var s = IniFile.ReadString(
            PluginHostText.ReadAnsi(Section),
            PluginHostText.ReadAnsi(Ident),
            PluginHostText.ReadAnsi(Default));
        var result = false;
        var bytes = GXX.Core.EncodingInit.GBK.GetBytes(s);
        if (Dest is not null && DestLen > (uint)bytes.Length && bytes.Length > 0)
        {
            var n = Math.Min(bytes.Length, Dest.Length);
            Array.Copy(bytes, 0, Dest, 0, n);
            if (n < Dest.Length) Dest[n] = 0;
            result = true;
        }
        DestLen = (uint)bytes.Length;
        return result ? 1 : 0;
    }

    /// <summary>原文 `TIniFile_WriteString`（原 :1542）。</summary>
    public void TIniFile_WriteString(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident, byte[]? Value)
        => IniFile.WriteString(PluginHostText.ReadAnsi(Section), PluginHostText.ReadAnsi(Ident), PluginHostText.ReadAnsi(Value));

    /// <summary>原文 `TIniFile_ReadInteger`（原 :1547）。</summary>
    public int TIniFile_ReadInteger(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident, int Default)
        => IniFile.ReadInteger(PluginHostText.ReadAnsi(Section), PluginHostText.ReadAnsi(Ident), Default);

    /// <summary>原文 `TIniFile_WriteInteger`（原 :1552）。</summary>
    public void TIniFile_WriteInteger(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident, int Value)
        => IniFile.WriteInteger(PluginHostText.ReadAnsi(Section), PluginHostText.ReadAnsi(Ident), Value);

    /// <summary>原文 `TIniFile_ReadBool`（原 :1557）。</summary>
    public int TIniFile_ReadBool(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident, int Default)
        => IniFile.ReadBool(PluginHostText.ReadAnsi(Section), PluginHostText.ReadAnsi(Ident), Default);

    /// <summary>原文 `TIniFile_WriteBool`（原 :1562）。</summary>
    public void TIniFile_WriteBool(IIniFileHandle IniFile, byte[]? Section, byte[]? Ident, int Value)
        => IniFile.WriteBool(PluginHostText.ReadAnsi(Section), PluginHostText.ReadAnsi(Ident), Value);

    // =================================================================================
    // 原 :1573-1790  TMapManager / TEnvirnoment
    // =================================================================================

    /// <summary>原文 `TMapManager_FindMap`（原 :1573）：`g_MapManager.FindMap(MapName)`。</summary>
    public IEnvirnoment? TMapManager_FindMap(byte[]? MapName) => _env.FindMap(PluginHostText.ReadAnsi(MapName));

    /// <summary>
    /// 原文 `TMapManager_GetMapList`（原 :1579）：`Result := g_MapManager;`
    /// 原文如此（PluginImplement.pas:1581）：直接把 TMapManager 自身当 TList 返回
    /// （TMapManager 继承自 TList），因此列表元素是 TEnvirnoment。
    /// </summary>
    public IListHandle TMapManager_GetMapList() => _env.MapList;

    /// <summary>原文 `TEnvir_GetMapName`（原 :1590）：`Envir.sMapName`。</summary>
    public int TEnvir_GetMapName(IEnvirnoment Envir, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Envir.sMapName, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TEnvir_GetMapDesc`（原 :1606）：`Envir.sMapDesc`。</summary>
    public int TEnvir_GetMapDesc(IEnvirnoment Envir, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Envir.sMapDesc, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TEnvir_GetWidth`（原 :1622）：`Envir.m_nWidth`。</summary>
    public int TEnvir_GetWidth(IEnvirnoment Envir) => Envir.m_nWidth;

    /// <summary>原文 `TEnvir_GetHeight`（原 :1628）：`Envir.m_nHeight`。</summary>
    public int TEnvir_GetHeight(IEnvirnoment Envir) => Envir.m_nHeight;

    /// <summary>原文 `TEnvir_GetMinMap`（原 :1634）：`Envir.nMinMap`。</summary>
    public int TEnvir_GetMinMap(IEnvirnoment Envir) => Envir.nMinMap;

    /// <summary>原文 `TEnvir_IsMainMap`（原 :1640）：`Envir.m_boMainMap`。</summary>
    public int TEnvir_IsMainMap(IEnvirnoment Envir) => Envir.m_boMainMap ? 1 : 0;

    /// <summary>原文 `TEnvir_GetMainMapName`（原 :1646）：`Envir.sMainMapName`。</summary>
    public int TEnvir_GetMainMapName(IEnvirnoment Envir, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Envir.sMainMapName, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TEnvir_IsMirrMap`（原 :1662）：`Envir.m_boMirror`。</summary>
    public int TEnvir_IsMirrMap(IEnvirnoment Envir) => Envir.m_boMirror ? 1 : 0;

    /// <summary>原文 `TEnvir_GetMirrMapCreateTick`（原 :1668）：`Envir.m_dwMirrorCreateTick`。</summary>
    public uint TEnvir_GetMirrMapCreateTick(IEnvirnoment Envir) => Envir.m_dwMirrorCreateTick;

    /// <summary>原文 `TEnvir_GetMirrMapSurvivalTime`（原 :1674）：`Envir.m_dwMirrorSurvivalTime`。</summary>
    public uint TEnvir_GetMirrMapSurvivalTime(IEnvirnoment Envir) => Envir.m_dwMirrorSurvivalTime;

    /// <summary>原文 `TEnvir_GetMirrMapExitToMap`（原 :1680）：`Envir.m_sMirrorExitToMap`。</summary>
    public int TEnvir_GetMirrMapExitToMap(IEnvirnoment Envir, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Envir.m_sMirrorExitToMap, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TEnvir_GetMirrMapMinMap`（原 :1696）：`Envir.m_nMirrorMinMap`。</summary>
    public int TEnvir_GetMirrMapMinMap(IEnvirnoment Envir) => Envir.m_nMirrorMinMap;

    /// <summary>原文 `TEnvir_GetAlwaysShowTime`（原 :1702）：`Envir.m_boAlwaysShowTime`。</summary>
    public int TEnvir_GetAlwaysShowTime(IEnvirnoment Envir) => Envir.m_boAlwaysShowTime ? 1 : 0;

    /// <summary>原文 `TEnvir_IsFBMap`（原 :1708）：`Envir.m_boFB`。</summary>
    public int TEnvir_IsFBMap(IEnvirnoment Envir) => Envir.m_boFB ? 1 : 0;

    /// <summary>原文 `TEnvir_GetFBMapName`（原 :1714）：`Envir.m_sFBName`。</summary>
    public int TEnvir_GetFBMapName(IEnvirnoment Envir, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(Envir.m_sFBName, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TEnvir_GetFBEnterLimit`（原 :1730）：`Integer(Envir.m_FBEnterLimit)`。</summary>
    public int TEnvir_GetFBEnterLimit(IEnvirnoment Envir) => Envir.m_FBEnterLimit;

    /// <summary>原文 `TEnvir_GetFBCreated`（原 :1736）：`Envir.m_boFBCreate`。</summary>
    public int TEnvir_GetFBCreated(IEnvirnoment Envir) => Envir.m_boFBCreate ? 1 : 0;

    /// <summary>原文 `TEnvir_GetFBCreateTime`（原 :1742）：`Envir.m_dwFBCreateTime`。</summary>
    public uint TEnvir_GetFBCreateTime(IEnvirnoment Envir) => Envir.m_dwFBCreateTime;

    /// <summary>
    /// 原文 `TEnvir_GetMapParam`（原 :1750-1753）。
    /// 原文如此（PluginImplement.pas:1752）：**函数体只有 `Result := False;`**——空实现，不补全。
    /// </summary>
    public int TEnvir_GetMapParam(IEnvirnoment Envir, byte[]? Param)
    {
        return 0;
    }

    /// <summary>
    /// 原文 `TEnvir_GetMapParamValue`（原 :1757-1760）。
    /// 原文如此（PluginImplement.pas:1759）：同样是**空实现**（只有 `Result := False;`），不补全。
    /// </summary>
    public int TEnvir_GetMapParamValue(IEnvirnoment Envir, byte[]? Param, byte[]? Dest, ref uint DestLen)
    {
        return 0;
    }

    /// <summary>原文 `TEnvir_CheckCanMove`（原 :1763）：`Envir.CanWalk(nX, nY, boFlag)`。</summary>
    public int TEnvir_CheckCanMove(IEnvirnoment Envir, int nX, int nY, int boFlag) => Envir.CanWalk(nX, nY, boFlag != 0) ? 1 : 0;

    /// <summary>原文 `TEnvir_IsValidObject`（原 :1769）：`Envir.IsValidObject(...)`。</summary>
    public int TEnvir_IsValidObject(IEnvirnoment Envir, int nX, int nY, int nRange, object? AObject)
        => Envir.IsValidObject(nX, nY, nRange, AObject!) ? 1 : 0;

    /// <summary>原文 `TEnvir_GetItemObjects`（原 :1775）：`Envir.GeTItemObjects(...)`（原文大小写如此）。</summary>
    public int TEnvir_GetItemObjects(IEnvirnoment Envir, int nX, int nY, IListHandle ObjectList)
        => Envir.GeTItemObjects(nX, nY, ObjectList);

    /// <summary>原文 `TEnvir_GetBaseObjects`（原 :1781）：`Envir.GeTBaseObjects(...)`（原文大小写如此）。</summary>
    public int TEnvir_GetBaseObjects(IEnvirnoment Envir, int nX, int nY, int IncDeathObject, IListHandle ObjectList)
        => Envir.GeTBaseObjects(nX, nY, IncDeathObject != 0, ObjectList);

    /// <summary>原文 `TEnvir_GetPlayObjects`（原 :1787）。</summary>
    public int TEnvir_GetPlayObjects(IEnvirnoment Envir, int nX, int nY, int IncDeathObject, IListHandle ObjectList)
        => Envir.GetPlayObjects(nX, nY, IncDeathObject != 0, ObjectList);

    // =================================================================================
    // 原 :1792-2135  M2 引擎相关
    // =================================================================================

    /// <summary>原文 `TM2Engine_GetVersion`（原 :1792）：`g_version`。</summary>
    public int TM2Engine_GetVersion(byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(_env.Version, Dest, ref DestLen) ? 1 : 0;

    /// <summary>
    /// 原文 `TM2Engine_GetVersionInt`（原 :1807）：
    /// `DecodeDate(g_buildtime, Y, M, D); Result := Y * 10000 + M * 100 + D;`
    /// </summary>
    public int TM2Engine_GetVersionInt()
    {
        var t = _env.BuildTime;
        return t.Year * 10000 + t.Month * 100 + t.Day;
    }

    /// <summary>原文 `TM2Engine_GetMainFormHandle`（原 :1815）：`FrmMain.Handle`。</summary>
    public IntPtr TM2Engine_GetMainFormHandle() => _env.MainFormHandle;

    /// <summary>原文 `TM2Engine_SetMainFormCaption`（原 :1820）：`sCaptionExtText := Caption;`</summary>
    public void TM2Engine_SetMainFormCaption(byte[]? Caption) => _env.SetMainFormCaptionExt(PluginHostText.ReadAnsi(Caption));

    /// <summary>原文 `TM2Engine_GetAppDir`（原 :1826）：`ExtractFilePath(ParamStr(0))`。</summary>
    public int TM2Engine_GetAppDir(byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(_env.AppDir, Dest, ref DestLen) ? 1 : 0;

    /// <summary>
    /// 原文 `TM2Engine_GetGlobalIniFile`（原 :1841）：
    /// `case M2IniType of 0: Config(!Setup.txt); 1: StringConf(AnsiString.ini); end;` 其余返回 nil。
    /// </summary>
    public IIniFileHandle? TM2Engine_GetGlobalIniFile(int M2IniType) => M2IniType switch
    {
        0 => _env.Config,
        1 => _env.StringConf,
        _ => null,
    };

    /// <summary>
    /// 原文 `TM2Engine_GetOtherFileDir`（原 :1852-1900）。
    /// 原文类型声明缺失（PluginInterface.pas 里没有 `TM2Engine_GetOtherFileDir`，
    /// 但 PluginInterface.pas:2510 的记录字段与 PluginFuncAssign.inc:165 都在用它），
    /// 签名取自本实现体。0..15 → 16 类目录（14/15 超出注释范围，原文如此）。
    /// </summary>
    public int TM2Engine_GetOtherFileDir(int M2FileType, byte[]? Dest, ref uint DestLen)
    {
        var s = _env.GetOtherFileDir(M2FileType);
        return PluginHostText.WriteText(s, Dest, ref DestLen) ? 1 : 0;
    }

    /// <summary>
    /// 原文 `TM2Engine_MainOutMessage`（原 :1902）。
    /// 原文如此（PluginImplement.pas:1904）：`MainOutMessage(Msg, True {IsAddTime})`
    /// ——**忽略传入的 IsAddTime 参数，恒传 True**。
    /// </summary>
    public void TM2Engine_MainOutMessage(byte[]? Msg, int IsAddTime)
        => _env.MainOutMessage(PluginHostText.ReadAnsi(Msg), true);

    /// <summary>原文 `TM2Engine_GetGlobalVarI`（原 :1908）：`g_Config.GlobaDyMval[Index]`。</summary>
    public int TM2Engine_GetGlobalVarI(int Index) => _env.GetGlobalVarI(Index);

    /// <summary>原文 `TM2Engine_SetGlobalVarI`（原 :1914）：赋值后 `Result := True`。</summary>
    public int TM2Engine_SetGlobalVarI(int Index, int Value)
    {
        _env.SetGlobalVarI(Index, Value);
        return 1;
    }

    /// <summary>原文 `TM2Engine_GetGlobalVarG`（原 :1921）：`g_Config.GlobalVal[Index]`。</summary>
    public int TM2Engine_GetGlobalVarG(int Index) => _env.GetGlobalVarG(Index);

    /// <summary>原文 `TM2Engine_SetGlobalVarG`（原 :1927）：赋值后 `Result := True`。</summary>
    public int TM2Engine_SetGlobalVarG(int Index, int Value)
    {
        _env.SetGlobalVarG(Index, Value);
        return 1;
    }

    /// <summary>原文 `TM2Engine_GetGlobalVarA`（原 :1934）：`g_Config.GlobalAVal[Index]`。</summary>
    public int TM2Engine_GetGlobalVarA(int Index, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(_env.GetGlobalVarA(Index), Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TM2Engine_SetGlobalVarA`（原 :1950）：赋值后 `Result := True`。</summary>
    public int TM2Engine_SetGlobalVarA(int Index, byte[]? Value)
    {
        _env.SetGlobalVarA(Index, PluginHostText.ReadAnsi(Value));
        return 1;
    }

    /// <summary>
    /// 原文 `TM2Engine_EncodeBuffer`（原 :1956-1970）。
    /// 原文如此（PluginImplement.pas:1962）：判定用 `DestLen &gt; Len`（不是 `&gt;=`），
    /// 且**只有写入成功才把 DestLen 设为实际长度**（`Len := Encode6BitBuf(..., DestLen)` 在分支内），
    /// 失败时 DestLen 保持 `GetEncodeSize(SrcLen)`。
    /// 接缝：6-Bit 编码用 GXX.Core.Protocol.EDcode（已移植）。
    /// </summary>
    public int TM2Engine_EncodeBuffer(byte[]? Src, uint SrcLen, byte[]? Dest, ref uint DestLen)
    {
        var result = false;
        var len = GXX.Core.Protocol.EDcode.GetEncodeSize((int)SrcLen);
        if (Dest is not null && Src is not null && DestLen > (uint)len)
        {
            len = GXX.Core.Protocol.EDcode.Encode6BitBuf(Src, 0, Dest, 0, (int)SrcLen, (int)DestLen);
            if (len < Dest.Length) Dest[len] = 0;
            result = true;
        }
        DestLen = (uint)len;
        return result ? 1 : 0;
    }

    /// <summary>
    /// 原文 `TM2Engine_DecodeBuffer`（原 :1972-1984）。
    /// 原文如此（PluginImplement.pas:1978）：这里判定用 `DestLen &gt;= Len`（与 Encode 的 `&gt;` 不同）。
    /// </summary>
    public int TM2Engine_DecodeBuffer(byte[]? Src, uint SrcLen, byte[]? Dest, ref uint DestLen)
    {
        var result = false;
        var len = GXX.Core.Protocol.EDcode.GetDecodeSize((int)SrcLen);
        if (Dest is not null && Src is not null && DestLen >= (uint)len)
        {
            len = GXX.Core.Protocol.EDcode.Decode6BitBuf(Src, 0, Dest, 0, (int)SrcLen, (int)DestLen);
            result = true;
        }
        DestLen = (uint)len;
        return result ? 1 : 0;
    }

    /// <summary>
    /// 原文 `TM2Engine_ZLibEncodeBuffer`（原 :1986-2014）：中间缓冲 `SrcLen * 2`，
    /// 压缩成功且 `nDestLen &gt; DestLen` 才写出（原文如此，同样不是 `&gt;=`）。
    /// 接缝：zLib 用 GXX.Core.Compress.ZlibEx（待接入其精确签名）。
    /// </summary>
    public int TM2Engine_ZLibEncodeBuffer(byte[]? Src, uint SrcLen, byte[]? Dest, ref uint DestLen)
        => throw new NotImplementedException("接缝：待 GXX.Core.Compress.ZlibEx 的 zLibEncodeBuffer 接入后实现（PluginImplement.pas:1986）");

    /// <summary>原文 `TM2Engine_ZLibDecodeBuffer`（原 :2016-2043）：中间缓冲 `SrcLen * 3`，判定为 `&gt;=`。</summary>
    public int TM2Engine_ZLibDecodeBuffer(byte[]? Src, uint SrcLen, byte[]? Dest, ref uint DestLen)
        => throw new NotImplementedException("接缝：待 GXX.Core.Compress.ZlibEx 的 zLibDecodeBuffer 接入后实现（PluginImplement.pas:2016）");

    /// <summary>
    /// 原文 `TM2Engine_EncryptBuffer`（原 :2045-2057）：`EncryBufferA_LF(Src, SrcLen)`，判定 `DestLen &gt;=`。
    /// 接缝：EncryptUnit_LF 已在 GXX.RunGate/EncryptUnitLf.cs 移植（跨车道，待上移 GXX.Core 后接入）。
    /// </summary>
    public int TM2Engine_EncryptBuffer(byte[]? Src, uint SrcLen, byte[]? Dest, ref uint DestLen)
        => throw new NotImplementedException("接缝：待 EncryptUnit_LF 的 EncryBufferA_LF 上移 GXX.Core 后接入（PluginImplement.pas:2045）");

    /// <summary>原文 `TM2Engine_DecryptBuffer`（原 :2059-2071）：`DecryBufferA_LF(Src, SrcLen)`。</summary>
    public int TM2Engine_DecryptBuffer(byte[]? Src, uint SrcLen, byte[]? Dest, ref uint DestLen)
        => throw new NotImplementedException("接缝：待 EncryptUnit_LF 的 DecryBufferA_LF 上移 GXX.Core 后接入（PluginImplement.pas:2059）");

    /// <summary>
    /// 原文 `TM2Engine_EncryptPassword`（原 :2076-2095）：固定密码 `MIR_MIR_MIR_MIR`，
    /// `EncryptDes_New(...)` + `EncodeString(...)`；容器不足时 `OutSize := 0`（原文如此）。
    /// </summary>
    public int TM2Engine_EncryptPassword(byte[]? InData, byte[]? OutData, ref uint OutSize)
        => throw new NotImplementedException("接缝：待 EncryptUnit_LF + DesUtils 接入后实现（PluginImplement.pas:2076）");

    /// <summary>原文 `TM2Engine_DecryptPassword`（原 :2097-2114）：`DecodeString` + `DecryptDes_New`。</summary>
    public int TM2Engine_DecryptPassword(byte[]? InData, byte[]? OutData, ref uint OutSize)
        => throw new NotImplementedException("接缝：待 EncryptUnit_LF + DesUtils 接入后实现（PluginImplement.pas:2097）");

    /// <summary>原文 `TM2Engine_GetTakeOnPosition`（原 :2117）：`GetTakeOnPosition(StdMode)`（M2Share）。</summary>
    public int TM2Engine_GetTakeOnPosition(int StdMode) => _env.GetTakeOnPosition(StdMode);

    /// <summary>原文 `TM2Engine_CheckBindType`（原 :2122）：`GetUserItemBindValue(BindValue, TUserItemBindValueType(BindType))`。</summary>
    public int TM2Engine_CheckBindType(byte BindValue, byte BindType) => _env.GetUserItemBindValue(BindValue, BindType) ? 1 : 0;

    /// <summary>原文 `TM2Engine_SetBindValue`（原 :2127）：`SetUserItemBindValue(...)`，注意签名是 `var BindValue`。</summary>
    public void TM2Engine_SetBindValue(ref byte BindValue, byte BindType, int pValue)
        => _env.SetUserItemBindValue(ref BindValue, BindType, pValue != 0);

    /// <summary>原文 `TM2Engine_GetRGB`（原 :2132）：`GetRGB(Color)`（M2Share）。</summary>
    public uint TM2Engine_GetRGB(byte Color) => _env.GetRGB(Color);

    // =================================================================================
    // 原 :2142-3320  TBaseObject 纯字段访问器（只取 m_* 字段的那些）
    // =================================================================================

    /// <summary>原文 `TBaseObject_GetChrName`（原 :2142）：`BaseObject.m_sCharName`。</summary>
    public int TBaseObject_GetChrName(IBaseObjectHandle BaseObject, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(BaseObject.m_sCharName, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TBaseObject_GetGender`（原 :2181）：`BaseObject.m_btGender`。</summary>
    public byte TBaseObject_GetGender(IBaseObjectHandle BaseObject) => BaseObject.m_btGender;

    /// <summary>原文 `TBaseObject_GetJob`（原 :2187）：`BaseObject.m_btJob`。</summary>
    public byte TBaseObject_GetJob(IBaseObjectHandle BaseObject) => BaseObject.m_btJob;

    /// <summary>原文 `TBaseObject_GetHair`（原 :2215）：`BaseObject.m_btHair`。</summary>
    public byte TBaseObject_GetHair(IBaseObjectHandle BaseObject) => BaseObject.m_btHair;

    /// <summary>原文 `TBaseObject_GetHomeMap`（原 :2267）：`BaseObject.m_sHomeMap`。</summary>
    public int TBaseObject_GetHomeMap(IBaseObjectHandle BaseObject, byte[]? Dest, ref uint DestLen)
        => PluginHostText.WriteText(BaseObject.m_sHomeMap, Dest, ref DestLen) ? 1 : 0;

    /// <summary>原文 `TBaseObject_GetHomeX`（原 :2283）：`BaseObject.m_nHomeX`。</summary>
    public int TBaseObject_GetHomeX(IBaseObjectHandle BaseObject) => BaseObject.m_nHomeX;

    /// <summary>原文 `TBaseObject_GetHomeY`（原 :2289）：`BaseObject.m_nHomeY`。</summary>
    public int TBaseObject_GetHomeY(IBaseObjectHandle BaseObject) => BaseObject.m_nHomeY;

    /// <summary>原文 `TBaseObject_GetPermission`（原 :2295）：`BaseObject.m_btPermission`。</summary>
    public byte TBaseObject_GetPermission(IBaseObjectHandle BaseObject) => BaseObject.m_btPermission;

    /// <summary>原文 `TBaseObject_SetPermission`（原 :2300）。</summary>
    public void TBaseObject_SetPermission(IBaseObjectHandle BaseObject, byte pValue) => BaseObject.m_btPermission = pValue;

    /// <summary>原文 `TBaseObject_GetDeath`（原 :2305）：`BaseObject.m_boDeath`。</summary>
    public int TBaseObject_GetDeath(IBaseObjectHandle BaseObject) => BaseObject.m_boDeath ? 1 : 0;

    /// <summary>原文 `TBaseObject_GetDeathTick`（原 :2310）：`BaseObject.m_dwDeathTick`。</summary>
    public uint TBaseObject_GetDeathTick(IBaseObjectHandle BaseObject) => BaseObject.m_dwDeathTick;

    /// <summary>原文 `TBaseObject_GetGhost`（原 :2315）：`BaseObject.m_boGhost`。</summary>
    public int TBaseObject_GetGhost(IBaseObjectHandle BaseObject) => BaseObject.m_boGhost ? 1 : 0;

    /// <summary>原文 `TBaseObject_GetGhostTick`（原 :2320）：`BaseObject.m_dwGhostTick`。</summary>
    public uint TBaseObject_GetGhostTick(IBaseObjectHandle BaseObject) => BaseObject.m_dwGhostTick;

    /// <summary>原文 `TBaseObject_GetRaceServer`（原 :2345）：`BaseObject.m_btRaceServer`。</summary>
    public byte TBaseObject_GetRaceServer(IBaseObjectHandle BaseObject) => BaseObject.m_btRaceServer;

    /// <summary>原文 `TBaseObject_GetAppr`（原 :2350）：`BaseObject.m_wAppr`。</summary>
    public ushort TBaseObject_GetAppr(IBaseObjectHandle BaseObject) => BaseObject.m_wAppr;

    /// <summary>原文 `TBaseObject_GetRaceImg`（原 :2355）：`BaseObject.m_btRaceImg`。</summary>
    public byte TBaseObject_GetRaceImg(IBaseObjectHandle BaseObject) => BaseObject.m_btRaceImg;

    /// <summary>原文 `TBaseObject_GetCharStatus`（原 :2365）：`BaseObject.m_nCharStatus`。</summary>
    public int TBaseObject_GetCharStatus(IBaseObjectHandle BaseObject) => BaseObject.m_nCharStatus;

    /// <summary>原文 `TBaseObject_SetCharStatus`（原 :2370）。</summary>
    public void TBaseObject_SetCharStatus(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_nCharStatus = Value;

    /// <summary>原文 `TBaseObject_GetHungerPoint`（原 :2382）：`BaseObject.m_nHungerStatus`。</summary>
    public int TBaseObject_GetHungerPoint(IBaseObjectHandle BaseObject) => BaseObject.m_nHungerStatus;

    /// <summary>原文 `TBaseObject_SetHungerPoint`（原 :2387）。</summary>
    public void TBaseObject_SetHungerPoint(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_nHungerStatus = Value;

    /// <summary>原文 `TBaseobject_IsNGMonster`（原 :2393，注意原文类型名 `TBaseobject_` 小写 o）：`BaseObject.m_boISNGMonster`。</summary>
    public int TBaseobject_IsNGMonster(IBaseObjectHandle BaseObject) => BaseObject.m_boISNGMonster ? 1 : 0;

    /// <summary>原文 `TBaseObject_IsDummyObject`（原 :2399）：`BaseObject.m_boDummyObject`。</summary>
    public int TBaseObject_IsDummyObject(IBaseObjectHandle BaseObject) => BaseObject.m_boDummyObject ? 1 : 0;

    /// <summary>原文 `TBaseObject_GetViewRange`（原 :2405）：`BaseObject.m_nViewRange`。</summary>
    public int TBaseObject_GetViewRange(IBaseObjectHandle BaseObject) => BaseObject.m_nViewRange;

    /// <summary>原文 `TBaseObject_SetViewRange`（原 :2411）。</summary>
    public void TBaseObject_SetViewRange(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_nViewRange = Value;

    /// <summary>原文 `TBaseObject_GetAbility`（原 :2417）：`Dest^ := BaseObject.m_Abil; Result := True;`</summary>
    public int TBaseObject_GetAbility(IBaseObjectHandle BaseObject, ref TAbility Dest)
    {
        // 原文如此（PluginImplement.pas:2418）：整体结构体赋值（TAbility 为 packed 值类型）
        Dest = BaseObject.M_Abil;
        return 1;
    }

    /// <summary>原文 `TBaseObject_GetWAbility`（原 :2423）：`Dest^ := BaseObject.m_WAbil; Result := True;`</summary>
    public int TBaseObject_GetWAbility(IBaseObjectHandle BaseObject, ref TAbility Dest)
    {
        Dest = BaseObject.M_WAbil;
        return 1;
    }

    /// <summary>原文 `TBaseObject_SetWAbility`（原 :2428）：`BaseObject.m_WAbil := Value^;`</summary>
    public void TBaseObject_SetWAbility(IBaseObjectHandle BaseObject, ref TAbility pValue) => BaseObject.M_WAbil = pValue;
    /// <summary>原文 `TBaseObject_GetMaster`（原 :2442）：`BaseObject.m_Master`。</summary>
    public IBaseObjectHandle? TBaseObject_GetMaster(IBaseObjectHandle BaseObject) => BaseObject.m_Master;

    /// <summary>原文 `TBaseObject_GetMasterEx`（原 :2447）：`BaseObject.GetMasterEx`（方法而非字段）。</summary>
    public IBaseObjectHandle? TBaseObject_GetMasterEx(IBaseObjectHandle BaseObject) => BaseObject.GetMasterEx();

    /// <summary>原文 `TBaseObject_GetSuperManMode`（原 :2452）：`BaseObject.m_boSuperMan`。</summary>
    public int TBaseObject_GetSuperManMode(IBaseObjectHandle BaseObject) => BaseObject.m_boSuperMan ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetSuperManMode`（原 :2457）。</summary>
    public void TBaseObject_SetSuperManMode(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boSuperMan = Value != 0;

    /// <summary>原文 `TBaseObject_GetAdminMode`（原 :2462）：`BaseObject.m_boAdminMode`。</summary>
    public int TBaseObject_GetAdminMode(IBaseObjectHandle BaseObject) => BaseObject.m_boAdminMode ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetAdminMode`（原 :2467）。</summary>
    public void TBaseObject_SetAdminMode(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boAdminMode = Value != 0;

    /// <summary>原文 `TBaseObject_GetTransparent`（原 :2472）：`BaseObject.m_boTransparent`。</summary>
    public int TBaseObject_GetTransparent(IBaseObjectHandle BaseObject) => BaseObject.m_boTransparent ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetTransparent`（原 :2477）。</summary>
    public void TBaseObject_SetTransparent(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boTransparent = Value != 0;

    /// <summary>原文 `TBaseObject_GetObMode`（原 :2482）：`BaseObject.m_boObMode`。</summary>
    public int TBaseObject_GetObMode(IBaseObjectHandle BaseObject) => BaseObject.m_boObMode ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetObMode`（原 :2487）。</summary>
    public void TBaseObject_SetObMode(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boObMode = Value != 0;

    /// <summary>原文 `TBaseObject_GetStoneMode`（原 :2492）：`BaseObject.m_boStoneMode`。</summary>
    public int TBaseObject_GetStoneMode(IBaseObjectHandle BaseObject) => BaseObject.m_boStoneMode ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetStoneMode`（原 :2497）。</summary>
    public void TBaseObject_SetStoneMode(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boStoneMode = Value != 0;

    /// <summary>原文 `TBaseObject_GetStickMode`（原 :2502）：`BaseObject.m_boStickMode`。</summary>
    public int TBaseObject_GetStickMode(IBaseObjectHandle BaseObject) => BaseObject.m_boStickMode ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetStickMode`（原 :2507）。</summary>
    public void TBaseObject_SetStickMode(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boStickMode = Value != 0;

    /// <summary>原文 `TBaseObject_GetIsAnimal`（原 :2515）：`BaseObject.m_boAnimal`。</summary>
    public int TBaseObject_GetIsAnimal(IBaseObjectHandle BaseObject) => BaseObject.m_boAnimal ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetIsAnimal`（原 :2520）。</summary>
    public void TBaseObject_SetIsAnimal(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boAnimal = Value != 0;

    /// <summary>原文 `TBaseObject_GetIsNoItem`（原 :2525）：`BaseObject.m_boNoItem`。</summary>
    public int TBaseObject_GetIsNoItem(IBaseObjectHandle BaseObject) => BaseObject.m_boNoItem ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetIsNoItem`（原 :2530）。</summary>
    public void TBaseObject_SetIsNoItem(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boNoItem = Value != 0;

    /// <summary>原文 `TBaseObject_GetCoolEye`（原 :2535）：`BaseObject.m_boCoolEye`。</summary>
    public int TBaseObject_GetCoolEye(IBaseObjectHandle BaseObject) => BaseObject.m_boCoolEye ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetCoolEye`（原 :2540）。</summary>
    public void TBaseObject_SetCoolEye(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boCoolEye = Value != 0;

    /// <summary>原文 `TBaseObject_GetHideMode`（原 :2676）：`BaseObject.m_boHideMode`。</summary>
    public int TBaseObject_GetHideMode(IBaseObjectHandle BaseObject) => BaseObject.m_boHideMode ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetHideMode`（原 :2681）。</summary>
    public void TBaseObject_SetHideMode(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boHideMode = Value != 0;

    /// <summary>原文 `TBaseObject_GetIsParalysis`（原 :2708）：`BaseObject.m_boParalysis`。</summary>
    public int TBaseObject_GetIsParalysis(IBaseObjectHandle BaseObject) => BaseObject.m_boParalysis ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetIsParalysis`（原 :2713）。</summary>
    public void TBaseObject_SetIsParalysis(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boParalysis = Value != 0;

    /// <summary>原文 `TBaseObject_GetParalysisRate`（原 :2718）：`BaseObject.m_dwParalysisRate`。</summary>
    public uint TBaseObject_GetParalysisRate(IBaseObjectHandle BaseObject) => BaseObject.m_dwParalysisRate;

    /// <summary>原文 `TBaseObject_SetParalysisRate`（原 :2723）。</summary>
    public void TBaseObject_SetParalysisRate(IBaseObjectHandle BaseObject, uint Value) => BaseObject.m_dwParalysisRate = Value;

    /// <summary>原文 `TBaseObject_GetIsMDParalysis`（原 :2760，原文如此拼写 MD）：`BaseObject.m_boMDParalysis`。</summary>
    public int TBaseObject_GetIsMDParalysis(IBaseObjectHandle BaseObject) => BaseObject.m_boMDParalysis ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetIsMDParalysis`（原 :2765）。</summary>
    public void TBaseObject_SetIsMDParalysis(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boMDParalysis = Value != 0;

    /// <summary>原文 `TBaseObject_GetMDParalysisRate`（原 :2770）：`BaseObject.m_dwMDParalysisRate`。</summary>
    public uint TBaseObject_GetMDParalysisRate(IBaseObjectHandle BaseObject) => BaseObject.m_dwMDParalysisRate;

    /// <summary>原文 `TBaseObject_SetMDParalysisRate`（原 :2775）。</summary>
    public void TBaseObject_SetMDParalysisRate(IBaseObjectHandle BaseObject, uint Value) => BaseObject.m_dwMDParalysisRate = Value;

    /// <summary>原文 `TBaseObject_GetIsFrozen`（原 :2780）：`BaseObject.m_boFrozen`。</summary>
    public int TBaseObject_GetIsFrozen(IBaseObjectHandle BaseObject) => BaseObject.m_boFrozen ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetIsFrozen`（原 :2785）。</summary>
    public void TBaseObject_SetIsFrozen(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boFrozen = Value != 0;

    /// <summary>原文 `TBaseObject_GetFrozenRate`（原 :2790）：`BaseObject.m_dwFrozenRate`。</summary>
    public uint TBaseObject_GetFrozenRate(IBaseObjectHandle BaseObject) => BaseObject.m_dwFrozenRate;

    /// <summary>原文 `TBaseObject_SetFrozenRate`（原 :2795）。</summary>
    public void TBaseObject_SetFrozenRate(IBaseObjectHandle BaseObject, uint Value) => BaseObject.m_dwFrozenRate = Value;

    /// <summary>原文 `TBaseObject_GetIsCobwebWinding`（原 :2800）：`BaseObject.m_boCobwebWinding`。</summary>
    public int TBaseObject_GetIsCobwebWinding(IBaseObjectHandle BaseObject) => BaseObject.m_boCobwebWinding ? 1 : 0;

    /// <summary>原文 `TBaseObject_SetIsCobwebWinding`（原 :2805）。</summary>
    public void TBaseObject_SetIsCobwebWinding(IBaseObjectHandle BaseObject, int Value) => BaseObject.m_boCobwebWinding = Value != 0;

    /// <summary>原文 `TBaseObject_GetCobwebWindingRate`（原 :2810）：`BaseObject.m_dwCobwebWindingRate`。</summary>
    public uint TBaseObject_GetCobwebWindingRate(IBaseObjectHandle BaseObject) => BaseObject.m_dwCobwebWindingRate;

    /// <summary>原文 `TBaseObject_SetCobwebWindingRate`（原 :2815）。</summary>
    public void TBaseObject_SetCobwebWindingRate(IBaseObjectHandle BaseObject, uint Value) => BaseObject.m_dwCobwebWindingRate = Value;

    /// <summary>原文 `TBaseObject_GetUnParalysisValue`（原 :2820）：`BaseObject.m_WAbil.NewValue[0]`。</summary>
    public uint TBaseObject_GetUnParalysisValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(0);

    /// <summary>原文 `TBaseObject_SetUnParalysisValue`（原 :2825）：`BaseObject.m_WAbil.NewValue[0] := Value;`</summary>
    public void TBaseObject_SetUnParalysisValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(0, Value);

    /// <summary>原文 `TBaseObject_GetUnMagicShieldValue`（原 :2830）：`m_WAbil.NewValue[1]`。</summary>
    public uint TBaseObject_GetUnMagicShieldValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(1);

    /// <summary>原文 `TBaseObject_SetUnMagicShieldValue`（原 :2835）。</summary>
    public void TBaseObject_SetUnMagicShieldValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(1, Value);

    /// <summary>原文 `TBaseObject_GetUnRevivalValue`（原 :2840）：`m_WAbil.NewValue[2]`。</summary>
    public uint TBaseObject_GetUnRevivalValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(2);

    /// <summary>原文 `TBaseObject_SetUnRevivalValue`（原 :2845）。</summary>
    public void TBaseObject_SetUnRevivalValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(2, Value);

    /// <summary>原文 `TBaseObject_GetUnPosionValue`（原 :2850，原文如此拼写 Posion）：`m_WAbil.NewValue[3]`。</summary>
    public uint TBaseObject_GetUnPosionValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(3);

    /// <summary>原文 `TBaseObject_SetUnPosionValue`（原 :2855）。</summary>
    public void TBaseObject_SetUnPosionValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(3, Value);

    /// <summary>原文 `TBaseObject_GetUnTammingValue`（原 :2860，原文如此拼写 Tamming）：`m_WAbil.NewValue[4]`。</summary>
    public uint TBaseObject_GetUnTammingValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(4);

    /// <summary>原文 `TBaseObject_SetUnTammingValue`（原 :2865）。</summary>
    public void TBaseObject_SetUnTammingValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(4, Value);

    /// <summary>原文 `TBaseObject_GetUnFireCrossValue`（原 :2870）：`m_WAbil.NewValue[5]`。</summary>
    public uint TBaseObject_GetUnFireCrossValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(5);

    /// <summary>原文 `TBaseObject_SetUnFireCrossValue`（原 :2875）。</summary>
    public void TBaseObject_SetUnFireCrossValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(5, Value);

    /// <summary>原文 `TBaseObject_GetUnFrozenValue`（原 :2880）：`m_WAbil.NewValue[19]`。</summary>
    public uint TBaseObject_GetUnFrozenValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(19);

    /// <summary>原文 `TBaseObject_SetUnFrozenValue`（原 :2885）：`BaseObject.m_WAbil.NewValue[19] := Value;`（原 :2973 在另一分支里同义）。</summary>
    public void TBaseObject_SetUnFrozenValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(19, Value);

    /// <summary>原文 `TBaseObject_GetUnCobwebWindingValue`（原 :2890）：`m_WAbil.NewValue[20]`。</summary>
    public uint TBaseObject_GetUnCobwebWindingValue(IBaseObjectHandle BaseObject) => BaseObject.M_WAbilNewValue(20);

    /// <summary>原文 `TBaseObject_SetUnCobwebWindingValue`（原 :2895）。</summary>
    public void TBaseObject_SetUnCobwebWindingValue(IBaseObjectHandle BaseObject, uint Value) => BaseObject.SetM_WAbilNewValue(20, Value);

    /// <summary>原文 `TBaseObject_GetTargetCret`（原 :2900）：`BaseObject.m_TargetCret`。</summary>
    public IBaseObjectHandle? TBaseObject_GetTargetCret(IBaseObjectHandle BaseObject) => BaseObject.m_TargetCret;

    /// <summary>原文 `TBaseObject_SetTargetCret`（原 :2905）：`BaseObject.m_TargetCret := TargetCret;`</summary>
    public void TBaseObject_SetTargetCret(IBaseObjectHandle BaseObject, IBaseObjectHandle? TargetCret) => BaseObject.m_TargetCret = TargetCret;

    /// <summary>原文 `TBaseObject_DelTargetCreat`（原 :2910）：`BaseObject.m_TargetCret := nil;`</summary>
    public void TBaseObject_DelTargetCreat(IBaseObjectHandle BaseObject) => BaseObject.m_TargetCret = null;

    /// <summary>原文 `TBaseObject_GetLastHiter`（原 :2915）：`BaseObject.m_LastHiter`。</summary>
    public IBaseObjectHandle? TBaseObject_GetLastHiter(IBaseObjectHandle BaseObject) => BaseObject.m_LastHiter;

    /// <summary>原文 `TBaseObject_GetExpHitter`（原 :2920）：`BaseObject.m_ExpHitter`。</summary>
    public IBaseObjectHandle? TBaseObject_GetExpHitter(IBaseObjectHandle BaseObject) => BaseObject.m_ExpHitter;

    /// <summary>原文 `TBaseObject_GetPoisonHitter`（原 :2925）：`BaseObject.m_PoisonHiter`（原文如此拼写 Hiter）。</summary>
    public IBaseObjectHandle? TBaseObject_GetPoisonHitter(IBaseObjectHandle BaseObject) => BaseObject.m_PoisonHiter;

    /// <summary>原文 `TBaseObject_GetPoseCreate`（原 :2930）：`BaseObject.m_PoseCreate`。</summary>
    public IBaseObjectHandle? TBaseObject_GetPoseCreate(IBaseObjectHandle BaseObject) => BaseObject.m_PoseCreate;

    /// <summary>原文 `TBaseObject_IsProperTarget`（原 :2935）：`BaseObject.IsProperTarget(Target)`。</summary>
    public int TBaseObject_IsProperTarget(IBaseObjectHandle BaseObject, IBaseObjectHandle? Target)
        => BaseObject.IsProperTarget(Target) ? 1 : 0;

    /// <summary>原文 `TBaseObject_IsProperFriend`（原 :2940）：`BaseObject.IsProperFriend(Target)`。</summary>
    public int TBaseObject_IsProperFriend(IBaseObjectHandle BaseObject, IBaseObjectHandle? Target)
        => BaseObject.IsProperFriend(Target) ? 1 : 0;

    /// <summary>原文 `TBaseObject_TargetInRange`（原 :2945）：`BaseObject.TargetInRange(Target, nX, nY, nRange)`。</summary>
    public int TBaseObject_TargetInRange(IBaseObjectHandle BaseObject, IBaseObjectHandle? Target, int nX, int nY, int nRange)
        => BaseObject.TargetInRange(Target, nX, nY, nRange) ? 1 : 0;

    /// <summary>原文 `TBaseObject_IsInSafeZone`（原 :3155 附近）：`BaseObject.InSafeZone`。</summary>
    public int TBaseObject_IsInSafeZone(IBaseObjectHandle BaseObject) => BaseObject.InSafeZone ? 1 : 0;

    /// <summary>原文 `TBaseObject_GetLevelExp`（原 :3200 附近）：`BaseObject.GetLevelExp(nLevel)` 之类，接缝。</summary>
    public uint TBaseObject_GetLevelExp(IBaseObjectHandle BaseObject, int nLevel) => BaseObject.GetLevelExp(nLevel);

    /// <summary>
    /// 原文 `TBaseObject_TrainSkill`（原 :3197-3209）。
    /// 原文类型声明缺失（PluginInterface.pas 里没有 `TBaseObject_TrainSkill`，但
    /// PluginInterface.pas:2697 的记录字段与 PluginFuncAssign.inc:409 都在用它），
    /// 签名取自本实现体；实现在 ObjBase 的 `TrainSkill/CheckMagicLevelup/MagicTranPointChanged` 组合里，
    /// 接缝：待 ObjBase 单元移植后接入。
    /// </summary>
    public int TBaseObject_TrainSkill(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic, int nTranPoint, int IsDoCheck)
        => throw new NotImplementedException("接缝：待 ObjBase 的 TrainSkill/CheckMagicLevelup 接入后实现（PluginImplement.pas:3197）");

    /// <summary>
    /// 原文 `_TPlayObject_GetAlcohol`（原 :4862-4865）：`Result := @Player.m_Alcohol;`
    /// 原文缺陷（PluginInterface.pas 里**没有** `TPlayObject_GetAlcohol` 类型声明；
    /// :1446 只有 `TSmartObject_GetAlcohol`，而 PluginFuncAssign.inc:585 赋的也是
    /// `_TSmartObject_GetAlcohol`，故本函数**未被任何字段引用**）。
    /// 返回 `m_Alcohol` 的**指针**；接缝：待 ObjPlayer.m_Alcohol 字段接入后返回其地址。
    /// </summary>
    public IntPtr TPlayObject_GetAlcohol(IPlayObjectHandle Player)
        => throw new NotImplementedException("接缝：待 ObjPlayer.m_Alcohol 字段接入后返回其地址（PluginImplement.pas:4862）");

    /// <summary>
    /// 原文 `_TPlayObject_GetHeroM2ShopList`（原 :5091-5094）：`Result := Player.m_HeroM2ShopList;`
    /// 原文缺陷（同上：无对应类型声明，PluginFuncAssign.inc 也未引用）。
    /// 接缝：待 ObjPlayer.m_HeroM2ShopList 接入后返回该列表。
    /// </summary>
    public IListHandle TPlayObject_GetHeroM2ShopList(IPlayObjectHandle Player)
        => throw new NotImplementedException("接缝：待 ObjPlayer.m_HeroM2ShopList 接入后返回（PluginImplement.pas:5091）");

    /// <summary>
    /// 原文 `_TPlayObject_GetHeroM2ShopOpenList`（原 :5096-5099）：`Result := Player.m_HeroM2ShopOpenList;`
    /// 原文缺陷（同上）。接缝：待 ObjPlayer.m_HeroM2ShopOpenList 接入后返回该列表。
    /// </summary>
    public IListHandle TPlayObject_GetHeroM2ShopOpenList(IPlayObjectHandle Player)
        => throw new NotImplementedException("接缝：待 ObjPlayer.m_HeroM2ShopOpenList 接入后返回（PluginImplement.pas:5096）");
}

/// <summary>
/// 原文 `TNotifyEventMethod`（PluginInterface.pas:41-46 / PluginImplement.pas:41-46）
/// ——`record Click: TNotifyEventEx; Sender: TObject; end;` 的托管承载。
/// </summary>
public sealed class PluginMenuNotify
{
    public PluginMenuNotify(TNotifyEventEx click, object? sender)
    {
        Click = click;
        Sender = sender;
    }

    /// <summary>原文 `Click: TNotifyEventEx`。</summary>
    public TNotifyEventEx Click { get; }

    /// <summary>原文 `Sender: TObject`（原版指向新建的 TMenuItem）。</summary>
    public object? Sender { get; }
}

/// <summary>
/// 原文 `TIniFile_ReadString` 需要同时拿到"文本 + 长度"，
/// 而 `IIniFileHandle.ReadString` 的 byte[] 出参在托管侧只够读回长度；
/// 该桥用于让宿主实现体读到上一次读出的文本（仅本单元内部使用）。
/// </summary>
internal static class PluginIniReadBridge
{
    [ThreadStatic]
    private static string? _lastReadValue;

    internal static string LastReadValue => _lastReadValue ?? string.Empty;

    internal static void Set(string? value) => _lastReadValue = value;
}
