using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>`M2Definition.pas` 的 `TObjGame = (Obj_None, Obj_Actor, ...)` 1:1。
/// 顺序即取值，`Obj_Actor = 1`。</summary>
public enum TObjGame
{
    Obj_None = 0,
    Obj_Actor = 1,
    Obj_Item = 2,
    Obj_Event = 3,
    Obj_Gate = 4,
    Obj_Switch = 5,
    Obj_MapEvent = 6,
    Obj_Door = 7,
    Obj_Roon = 8,
    Obj_MapEffect = 9,
}

/// <summary>地图格上的游戏对象（原文 `TGameObject` 基类，只用到 `m_ObjGame` 判别字段）。</summary>
public interface IGameObject
{
    TObjGame m_ObjGame { get; }
}

/// <summary>
/// ObjBase.pas 两个**地图范围扫描**函数 1:1 移植（批次J100）：
/// `TBaseObject.GetMapBaseObjects`(30866-30922) 与 `TBaseObject.GetMapBaseObjectCount`(30924-30978)。
///
/// **两函数几乎逐行相同，但有一处关键差异（极易在移植时被"对齐"掉）**：
/// - `GetMapBaseObjects`（30903）的过滤条件是 `(not m_boDeath) and (not m_boGhost)`；
/// - `GetMapBaseObjectCount`（30959-30960）的过滤条件是
///   `(not m_boDeath) and (not m_boGhost) and (IsProperTarget(BaseObject))`
///   ——**多一个 `IsProperTarget`**。
/// 也就是说"数一数周围有多少目标"会**排除非合法目标**（如同一阵营/不可攻击对象），
/// 而"列出周围对象"则**不会**——列表里会包含非法目标，由调用方自行再筛。
/// 若把两者"统一"成同一个过滤谓词，会让计数偏大（多算）或列表少掉本应返回的对象。
/// 本移植以显式区分两条路径表达该差异，并由
/// `CountExcludesNonProperTargets` / `ListExcludesNonProperTargets` 两个测试从两侧夹住。
///
/// **其余逐字保留的语义**：
/// - 扫描范围是**闭区间** `[nX-nRage, nX+nRage] × [nY-nRage, nY+nRage]`（30937-30940），
///   故实际覆盖 **(2*nRage+1)²** 个格子（`nRage = 0` 时仅中心 1 格）。
/// - 内层只统计 `GameObject &lt;&gt; nil` **且** `m_ObjGame = Obj_Actor` 的对象（30956）——
///   地上的物品（`Obj_Item`）等一律跳过。
/// - `GetMapBaseObjects`（30902-30906）**不判 `BaseObject &lt;&gt; nil`**，
///   而 `GetMapBaseObjectCount`（30959）**判了** `(BaseObject &lt;&gt; nil)`。由于
///   `BaseObject := TBaseObject(GameObject)` 是同一对象的类型转换，二者在语义上等价，
///   此处按各自原文逐字保留。
/// - `GetMapBaseObjects` 末尾**无条件** `Result := True`（30921），**即使发生异常也返回 True**
///   （异常在 30918-30920 被吞掉并 `MainOutMessage`）。
/// - `GetMapBaseObjectCount` 异常时返回已累计到的 `Result`（30935 的初值 0 或部分累加值）。
/// - 两者的 `resourcestring sExceptionMsg` **都是 `'[Exception] TBaseObject.GetMapBaseObjects'`**
///   （30933、30875）——**计数函数里也写着 GetMapBaseObjects**，属原文复制粘贴遗留，
///   本移植保留该字符串原样以便日志比对。
/// </summary>
public static class MapObjectScanCore
{
    /// <summary>30933/30875：两个函数共用的异常消息串（原文在计数函数中也写作 GetMapBaseObjects）。</summary>
    public const string ExceptionMsg = "[Exception] TBaseObject.GetMapBaseObjects";

    /// <summary>单格扫描的结果：满足条件的对象（`GetMapBaseObjects` 用）。</summary>
    public static List<object> GetMapBaseObjects(
        IMapScanEnvir envir, int nX, int nY, int nRage,
        Func<object, bool> isProperTarget,
        Func<object, bool> isDead,
        Func<object, bool> isGhost,
        Action<string>? onException = null)
    {
        var rList = new List<object>();

        try
        {
            int nStartX = nX - nRage;
            int nEndX = nX + nRage;
            int nStartY = nY - nRage;
            int nEndY = nY + nRage;

            for (int x = nStartX; x <= nEndX; x++)
            {
                for (int y = nStartY; y <= nEndY; y++)
                {
                    if (!envir.GetMapCellInfo(x, y, out var cell) || cell?.ObjList is null)
                        continue;

                    var objList = cell.ObjList;
                    for (int iii = 0; iii < objList.Count; iii++)
                    {
                        var gameObject = objList[iii];

                        if (gameObject is null)
                            continue;

                        // 30956：只取 obj_Actor
                        if (gameObject is not IGameObject go || go.m_ObjGame != TObjGame.Obj_Actor)
                            continue;

                        // 30903：**本函数不判 IsProperTarget**
                        if (!isDead(gameObject) && !isGhost(gameObject))
                            rList.Add(gameObject);
                    }
                }
            }
        }
        catch (Exception)
        {
            // 30918-30920
            onException?.Invoke(ExceptionMsg);
        }

        // 30921：**异常时也返回 True**
        return rList;
    }

    /// <summary>
    /// 30924-30978：与 <see cref="GetMapBaseObjects"/> 相同的扫描，但**只计数**，
    /// 且额外要求 `IsProperTarget`。异常时返回已累计的计数。
    /// </summary>
    public static int GetMapBaseObjectCount(
        IMapScanEnvir envir, int nX, int nY, int nRage,
        Func<object, bool> isProperTarget,
        Func<object, bool> isDead,
        Func<object, bool> isGhost,
        Action<string>? onException = null)
    {
        int result = 0;

        try
        {
            int nStartX = nX - nRage;
            int nEndX = nX + nRage;
            int nStartY = nY - nRage;
            int nEndY = nY + nRage;

            for (int x = nStartX; x <= nEndX; x++)
            {
                for (int y = nStartY; y <= nEndY; y++)
                {
                    if (!envir.GetMapCellInfo(x, y, out var cell) || cell?.ObjList is null)
                        continue;

                    var objList = cell.ObjList;
                    for (int iii = 0; iii < objList.Count; iii++)
                    {
                        var gameObject = objList[iii];

                        if (gameObject is null)
                            continue;

                        if (gameObject is not IGameObject go || go.m_ObjGame != TObjGame.Obj_Actor)
                            continue;

                        // 30959-30960：**比 GetMapBaseObjects 多一个 IsProperTarget**
                        if (!isDead(gameObject) && !isGhost(gameObject) && isProperTarget(gameObject))
                            result++;
                    }
                }
            }
        }
        catch (Exception)
        {
            onException?.Invoke(ExceptionMsg);
            return result;      // 异常时返回已累计值
        }

        return result;
    }

    /// <summary>扫描覆盖的格子数：(2*nRage+1)²（30941-30943 的闭区间）。</summary>
    public static int CellCountForRage(int nRage)
    {
        int side = 2 * nRage + 1;
        return side * side;
    }
}

/// <summary>扫描所需的地图环境接口（对应 `TEnvirnoment.GetMapCellInfo`）。</summary>
public interface IMapScanEnvir
{
    bool GetMapCellInfo(int nX, int nY, out IMapCell? cell);
}

/// <summary>地图格（对应 `TMapCellinfo`，仅用到 `ObjList`）。</summary>
public interface IMapCell
{
    List<object>? ObjList { get; }
}
