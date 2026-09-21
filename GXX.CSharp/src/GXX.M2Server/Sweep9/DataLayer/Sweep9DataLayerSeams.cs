// ============================================================================
// 本文件 = p9-m2-datalayer 车道（M2Engine 三条「数据层」单元）的**最小接缝层**。
//
// 服务单元（源单元名一律写全路径，供 tools/audit-coverage.ps1 的 E2 证据规则识别）：
//   Source/M2Engine/ItemEvent.pas     —— 见同目录 ItemEvent.cs / ItemEventManager.cs
//   Source/M2Engine/DataManage.pas    —— 见同目录 DataManage.cs
//   Source/M2Engine/UserShopDB_Old.pas—— **死代码，不移植**（证据见
//                                        docs/并行报告-p9-m2-datalayer.md「不移植登记」节）
//
// 【为什么需要这一层】
//   ItemEvent.pas 的 `uses` 里有 6 个单元（ObjGame / ObjBase / Envir / M2Share / Grobal2 /
//   M2Definition），其中 **ObjBase / Envir / M2Share 都只被移植了「一部分」**：
//     * `TGameObject`（ObjGame.pas:9）在托管侧**根本不存在**（`git grep -n "class TGameObject"`
//       零命中；既有的 `GXX.M2Server.IGameObject`（MapObjectScanCore.cs:24）只是一个
//       `m_ObjGame { get; }` 只读接口，**没有任何实现类**，也不是类层次的一环）。
//       本车道按 1:1 需要该基类 ⇒ 在 ItemEvent.cs 里落 **ObjGame.pas 的 TGameObject 那 17 行**，
//       不重复实现已有的 TGateObject/TDoorObject（那两个至今无人移植，也不在本车道的三单元内）。
//     * `TEnvirnoment`（Engine/Envir.cs:80）已有 `sMapName`(85) 与
//       `DeleteFromMap(int,int,object)`(250)，但 **`m_boFB` / `m_boFBCreate` 两个字段尚未落地**
//       （`git grep -n "m_boFB" -- src/` 只命中注释）⇒ 用下面的接口 + 接缝取，默认返回 false。
//     * `g_Config.dwFloorItemCanPickUpTime` / `dwClearDropOnFloorItemTime` 已落地在
//       `Engine/M2Config.GameMsgTime.cs:74/73`，本层直接转调（**不复制**这两个字段）。
//     * `AddGameDataLog`（M2Share.pas:3124 的 14 参重载）尚未移植 ⇒ 委托接缝。
//
// 依赖原则（任务书第 4 条）：**依赖缺失时优先用接缝委托，不臆造替身**。
//   * 能转到既有实现的，一律转调（`M2Config` / `DelphiRTL`）；
//   * 转不了的，落成 `Action`/`Func` 委托，默认「无宿主」语义，测试可替换；
//   * 单测通过 <see cref="Sweep9DataLayerSeam"/> 注入，**绝不依赖真实计时/地图/日志**。
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Sweep9.DataLayer;

// ---------------------------------------------------------------------------
// ObjGame.pas:9-13 的 `TGameObject` 在托管侧的形态。
//
// 该基类只有 4 个字段 + 一对构造/析构，字段名逐字保留（m_ObjGame / m_dwAddTime /
// m_nMapX / m_nMapY）。`TObjGame` 枚举已由顺序会话落在
// `GXX.M2Server.TObjGame`（MapObjectScanCore.cs:9，`Obj_Item = 2`），此处**复用**它，
// 不另造一份（台账 §14.2「不造第三份实现」）。
// ---------------------------------------------------------------------------

/// <summary>
/// 地图上的物品（原文 `TItemObject = class(TGameObject)`，ItemEvent.pas:9）。
/// 只暴露 ItemEvent.pas 用得到的成员：`m_UserItem` / `m_nMapX` / `m_nMapY`。
/// </summary>
public interface IItemGameItem
{
    /// <summary>原文 `m_UserItem: TUserItem`（ItemEvent.pas:20）。</summary>
    TUserItem m_UserItem { get; }

    /// <summary>原文 `TGameObject.m_nMapX`（ObjGame.pas:12）。</summary>
    int m_nMapX { get; }

    /// <summary>原文 `TGameObject.m_nMapY`（ObjGame.pas:13）。</summary>
    int m_nMapY { get; }
}

/// <summary>
/// 原文 `TEnvirnoment`（Envir.pas）在 ItemEvent.pas 里用到的那一小面。
/// <para>ItemEvent.pas 只碰 `DeleteFromMap`（89/143）、`sMapName`（150）、
/// `m_boFB`（110）与 `m_boFBCreate`（110）。</para>
/// <para>`m_boFB`/`m_boFBCreate` 在托管侧**尚未落地**（`Engine/Envir.cs` 里零命中），
/// 故本接口把「取得到就取、取不到当 false」显式表达为可空实现；待 Envir.pas 补上这两个字段后，
/// 让 `TEnvirnoment` 实现本接口即可，**无需改本文件之外的任何代码**。</para>
/// </summary>
public interface IItemGameEnvir
{
    /// <summary>原文 `TEnvirnoment.DeleteFromMap(nX, nY: Integer; obj: TObject): Boolean`（Envir.pas:1772）。</summary>
    bool DeleteFromMap(int nX, int nY, object obj);

    /// <summary>原文 `TEnvirnoment.sMapName`（Envir.pas）。</summary>
    string sMapName { get; }

    /// <summary>原文 `TEnvirnoment.m_boFB`（副本地图）。</summary>
    bool m_boFB { get; }

    /// <summary>原文 `TEnvirnoment.m_boFBCreate`（副本创建者）。</summary>
    bool m_boFBCreate { get; }
}

// ---------------------------------------------------------------------------
// M2Share.pas 里 ItemEvent.pas 用到的常量与宿主能力（1:1 数值，逐条注明原文行号）。
// ---------------------------------------------------------------------------

/// <summary>
/// M2Share.pas 的日志常量 + `AddGameDataLog` 14 参重载的实参包。
/// 数值与既有 `GXX.M2Server.DbLayer.AuctionLogTypes` 一致（同一份原文常量）；
/// 跨命名空间重复声明的原因是 **DbLayer 那份是 AuctionDB 的接缝类**，
/// 而 M2Server 工程内并无「M2Share 常量」的单一来源（`git grep -n "class M2ShareConst"` 零命中）。
/// </summary>
public static class ItemEventLogTypes
{
    /// <summary>M2Share.pas:87 <c>LOG_ActionNone = 00</c>。</summary>
    public const int LOG_ActionNone = 0;

    /// <summary>M2Share.pas:96 <c>LOG_ItemDisappear = 09; // 物品消失</c>。</summary>
    public const int LOG_ItemDisappear = 9;

    /// <summary>M2Share.pas:136 <c>TLogActorType = (latNone, latHuman, ...)</c> → <c>latNone = 0</c>。</summary>
    public const int latNone = 0;
}

/// <summary>
/// 原文 `AddGameDataLog(LogAction1, LogAction2: Byte; LogActorType: TLogActorType; MapName: string;
/// PointX, PointY: Integer; ItemName: string; ItemMakeIndex: Integer; ActorName, TargetName: string;
/// Data1: Integer = 0; Data2: Integer = 0; LogDesc: string = '')`
/// （M2Share.pas:3124-3126）的 14 个实参，按原文顺序逐参对应。
/// <para>ItemEvent.pas:150-151 的实参序列是
/// `(LOG_ItemDisappear, LOG_ActionNone, latNone, sMapName, m_nMapX, m_nMapY, StdItem.Name,
///   m_UserItem.MakeIndex, '0', '0', 0, 0, '到时清理')` —— 只有 13 个实参，
///   **第 9/10 个（ActorName/TargetName）都传 `'0'`**，原文如此。</para>
/// </summary>
public sealed class ItemEventGameDataLogArgs
{
    /// <summary>`LOG_ItemDisappear`（= 9）。</summary>
    public int LogAction1;

    /// <summary>`LOG_ActionNone`（= 0）。</summary>
    public int LogAction2;

    /// <summary>`latNone`（= 0）。</summary>
    public int LogActorType;

    /// <summary>`TEnvirnoment(m_PEnvir).sMapName`。</summary>
    public string MapName = "";

    /// <summary>`m_nMapX`。</summary>
    public int PointX;

    /// <summary>`m_nMapY`。</summary>
    public int PointY;

    /// <summary>`StdItem.Name`。</summary>
    public string ItemName = "";

    /// <summary>`m_UserItem.MakeIndex`。</summary>
    public int ItemMakeIndex;

    /// <summary>字面量 `'0'`。</summary>
    public string ActorName = "";

    /// <summary>字面量 `'0'`。</summary>
    public string TargetName = "";

    /// <summary>`0`。</summary>
    public int Data1;

    /// <summary>`0`。</summary>
    public int Data2;

    /// <summary>`'到时清理'`。</summary>
    public string LogDesc = "";
}

/// <summary>
/// ItemEvent.pas 用到的宿主能力（`g_Config` 两个字段、`UserEngine.GetStdItem`、
/// `MyGetTickCount`、`TEnvirnoment` 的两个副本标志、`AddGameDataLog`）。
/// 全部可替换；默认实现即「原文语义」或「无宿主」。
/// </summary>
public static class Sweep9DataLayerSeam
{
    /// <summary>
    /// 原文 `M2Share.MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime'`
    /// （M2Share.pas:3589；**DWORD 语义 ⇒ uint，会回绕**）。
    /// 默认转调既有 <see cref="DelphiRTL.GetTickCount"/>（GetTickCount 语义，uint 回绕），**不另造**。
    /// </summary>
    public static Func<uint> MyGetTickCount { get; set; } = DelphiRTL.GetTickCount;

    /// <summary>原文 `g_Config.dwFloorItemCanPickUpTime`（ItemEvent.pas:82）。
    /// 默认转调既有 `Engine.M2Config.dwFloorItemCanPickUpTime`（M2Config.GameMsgTime.cs:74），**不另造**。</summary>
    public static Func<uint> DwFloorItemCanPickUpTime { get; set; } = () => M2Config.dwFloorItemCanPickUpTime;

    /// <summary>原文 `g_Config.dwClearDropOnFloorItemTime`（ItemEvent.pas:102）。
    /// 默认转调既有 `Engine.M2Config.dwClearDropOnFloorItemTime`（M2Config.GameMsgTime.cs:73），**不另造**。</summary>
    public static Func<uint> DwClearDropOnFloorItemTime { get; set; } = () => M2Config.dwClearDropOnFloorItemTime;

    /// <summary>原文 `UserEngine.GetStdItem(wIndex): pTStdItem`（ItemEvent.pas:147）。
    /// 接缝：待 UsrEngn.pas 移植后接入（与 `DbLayerRunSeam.GetStdItem` 同源，但那是 DbLayer 的接缝类，
    /// 本车道不跨车道改它）。默认返回 null（= 原文物品不存在）。</summary>
    public static Func<ushort, TStdItem?> GetStdItem { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TBaseObject(m_OfBaseObject).m_boGhost` / `TBaseObject(m_DropBaseObject).m_boGhost`
    /// （ItemEvent.pas:129/134）。
    /// <para>原文是**未检查硬转换**后读 `m_boGhost`；托管侧无法在不知道 `TBaseObject` 类的前提下
    /// 做等价硬转换，故落成 `Func&lt;object, bool&gt;`：默认实现**按既有 Engine 的幽灵判定语义**取，
    /// 取不到则 false（= 原文 `TBaseObject(nil)` 判空后的短路路径）。</para>
    /// 接缝：待 ObjBase.pas 的 `TBaseObject.m_boGhost` 补齐后改为 `((TBaseObject)o).m_boGhost`。
    /// </summary>
    public static Func<object, bool> IsBaseObjectGhost { get; set; } = _ => false;

    /// <summary>
    /// 原文 `TEnvirnoment(m_PEnvir).m_boFB`（ItemEvent.pas:110）。
    /// 默认实现：`m_PEnvir` 若实现了本车道的 <see cref="IItemGameEnvir"/> 则取该属性，否则 false
    /// （`Engine.TEnvirnoment` 尚未落地这两个字段 —— 见 <see cref="IItemGameEnvir"/> 的说明）。
    /// </summary>
    public static Func<object, bool> EnvirBoFB { get; set; } = envir => envir is IItemGameEnvir e && e.m_boFB;

    /// <summary>原文 `TEnvirnoment(m_PEnvir).m_boFBCreate`（ItemEvent.pas:110）。同 <see cref="EnvirBoFB"/>。</summary>
    public static Func<object, bool> EnvirBoFBCreate { get; set; } = envir => envir is IItemGameEnvir e && e.m_boFBCreate;

    /// <summary>
    /// 原文 `AddGameDataLog(...)` 的 14 参重载（M2Share.pas:3124-3126，ItemEvent.pas:150-151 调用）。
    /// 接缝：待 M2Share.pas 移植后接入。
    /// </summary>
    public static Action<ItemEventGameDataLogArgs> AddGameDataLog { get; set; } = _ => { };

    /// <summary>
    /// 原文 `TItemManager.Run` 的 `except MainOutMessage(sExceptionMsg)`（ItemEvent.pas:351，
    /// `sExceptionMsg = '[Exception] TItemManager.Run'`，:310）。
    /// 接缝：待 M2Share 的日志输出移植后接入。
    /// </summary>
    public static Action<string> MainOutMessage { get; set; } = msg => LoggedMessages.Add(msg ?? "");

    /// <summary>进程内日志观察列表（`MainOutMessage` 的默认可观测落点，便于断言异常/失败路径）。</summary>
    public static readonly System.Collections.Generic.List<string> LoggedMessages = new();

    /// <summary>恢复全部接缝默认值（测试隔离用；与 `DbLayerTestKit.Isolate()` 同构）。</summary>
    public static void ResetDefaults()
    {
        MyGetTickCount = DelphiRTL.GetTickCount;
        DwFloorItemCanPickUpTime = () => M2Config.dwFloorItemCanPickUpTime;
        DwClearDropOnFloorItemTime = () => M2Config.dwClearDropOnFloorItemTime;
        GetStdItem = _ => null;
        IsBaseObjectGhost = _ => false;
        EnvirBoFB = envir => envir is IItemGameEnvir e && e.m_boFB;
        EnvirBoFBCreate = envir => envir is IItemGameEnvir e && e.m_boFBCreate;
        AddGameDataLog = _ => { };
        MainOutMessage = msg => LoggedMessages.Add(msg ?? "");
        LoggedMessages.Clear();
        DataManageAccessSeam.ResetDefaults();
        DataManageGlobals.DBQry = null;
        DataManageGlobals.ADOConnection = null;
        DataManageGlobals.AccessEngine = null;
    }
}
