// 源单元（文件头证据，供 tools/audit-coverage.ps1 的 E2 规则识别）：
//   Source/M2Engine/ItemEvent.pas
//   Source/M2Engine/DataManage.pas
//
// Sweep9 DataLayer 测试基础设施（p9-m2-datalayer 车道）：
//   * FakeItemGameEnvir  —— IItemGameEnvir 的内存实现（记录每次 DeleteFromMap 的实参）
//   * FakeBaseObject     —— 只有"幽灵"标志的基对象替身（IsBaseObjectGhost 的载荷）
//   * FakeAdoConnection  —— IAdoConnection 的内存实现（可令 Connected 赋值抛异常）
//   * FakeAdoQuery       —— IAdoQuery 的内存实现（记录 SQL/Open/Next/Close/ExecSQL 序列）
//   * Sweep9DataLayerTestKit.Isolate() —— 一键把全部接缝换成内存实现（**绝不连真实数据库/COM**）

using System;
using System.Collections.Generic;
using GXX.Core.Util;
using GXX.M2Server.Sweep9.DataLayer;

namespace GXX.M2Server.Tests;

/// <summary>`IItemGameEnvir` 的内存实现：记录 `DeleteFromMap` 的每一次调用。</summary>
public sealed class FakeItemGameEnvir : IItemGameEnvir
{
    public string sMapName { get; set; } = "";
    public bool m_boFB { get; set; }
    public bool m_boFBCreate { get; set; }

    /// <summary>`DeleteFromMap` 的返回值（原文把返回值丢弃，这里用来断言"丢弃"确实是丢弃）。</summary>
    public bool DeleteResult { get; set; } = true;

    /// <summary>调用序列：`"nX/nY"`（`obj` 由 <see cref="DeletedObjects"/> 单独记录）。</summary>
    public readonly List<string> DeleteCalls = new();
    public readonly List<object> DeletedObjects = new();

    public bool DeleteFromMap(int nX, int nY, object obj)
    {
        DeleteCalls.Add($"{nX}/{nY}");
        DeletedObjects.Add(obj);
        return DeleteResult;
    }
}

/// <summary>只有"幽灵"标志的基对象替身（对应原文 `TBaseObject.m_boGhost`）。</summary>
public sealed class FakeBaseObject
{
    public bool m_boGhost;
}

/// <summary>记录 `IAdoConnection` 上的每一次属性写入。</summary>
public sealed class FakeAdoConnection : IAdoConnection
{
    private bool _connected;
    private bool _keepConnection;
    private string _connectionString = "";

    /// <summary>每次 `ConnectionString` 被写入时记录新值。</summary>
    public readonly List<string> ConnectionStringWrites = new();

    public string ConnectionString
    {
        get => _connectionString;
        set
        {
            ConnectionStringWrites.Add(value);
            if (ThrowOnConnectionString)
                throw new InvalidOperationException("ADO 连接串赋值失败（测试注入）");
            _connectionString = value;
        }
    }

    public bool LoginPrompt { get; set; }

    /// <summary>每次 `Connected` 被写入时记录新值。</summary>
    public readonly List<bool> ConnectedWrites = new();

    /// <summary>为真时，写入 `Connected = true` 抛异常（用于测原文的 try/except 路径）。</summary>
    public bool ThrowOnConnect { get; set; }

    /// <summary>为真时，任何 `ConnectionString` 赋值抛异常（原文 :101 在保护圈之外）。</summary>
    public bool ThrowOnConnectionString { get; set; }

    public bool Connected
    {
        get => _connected;
        set
        {
            ConnectedWrites.Add(value);
            if (value && ThrowOnConnect) throw new InvalidOperationException("ADO 连接失败（测试注入）");
            _connected = value;
        }
    }

    public bool KeepConnection
    {
        get => _keepConnection;
        set { KeepConnectionWrites.Add(value); _keepConnection = value; }
    }

    public readonly List<bool> KeepConnectionWrites = new();
}

/// <summary>记录 `IAdoQuery` 上的每一次调用（顺序敏感）。</summary>
public sealed class FakeAdoQuery : IAdoQuery
{
    public readonly List<string> Calls = new();
    public readonly List<string> SqlLines = new();

    private readonly TStringList _sql = new();
    private readonly List<string> _opens = new();

    public int RecordCount { get; set; }
    public IAdoConnection? Connection { get; set; }
    public bool Prepared { get; set; }
    public IAccessParameters? Parameters { get; set; }

    /// <summary>为真时 `SQL.Add` 抛异常（用于测 `TItemManager.Run` 的 except 分支的同类路径）。</summary>
    public bool ThrowOnAdd;

    public TStringList SQL => _sql;

    /// <summary>把当前 <see cref="SQL"/> 的行快照成一条字符串（`|` 分隔）。</summary>
    public string SqlSnapshot() => string.Join("|", SqlLines);

    public void Open() { Calls.Add("Open"); _opens.Add(SqlSnapshot()); }
    public void Next() => Calls.Add("Next");
    public void Close() => Calls.Add("Close");
    public void ExecSQL() => Calls.Add("ExecSQL");
    public IAccessField? FieldByName(string field) { Calls.Add($"FieldByName({field})"); return null; }
}

/// <summary>`IAccessField` 的最小替身。</summary>
public sealed class FakeAccessField : IAccessField
{
    public string FieldName { get; set; } = "";
}

/// <summary>Sweep9 DataLayer 测试的公共辅助。</summary>
public static class Sweep9DataLayerTestKit
{
    /// <summary>
    /// 一键隔离：把 ItemEvent / DataManage 两条链接缝全部换成默认/内存实现，并清空日志与全局。
    /// <para><b>本套件不触碰真实计时、真实地图、真实 COM/ADO/Access 与磁盘。</b></para>
    /// </summary>
    public static (Sweep9FakeClock Clock, List<string> Log) Isolate()
    {
        Sweep9DataLayerSeam.ResetDefaults();
        DataManageAccessSeam.ResetDefaults();
        DataManageGlobals.DBQry = null;
        DataManageGlobals.ADOConnection = null;
        DataManageGlobals.AccessEngine = null;

        var clock = new Sweep9FakeClock();
        Sweep9DataLayerSeam.MyGetTickCount = clock.Peek;
        return (clock, Sweep9DataLayerSeam.LoggedMessages);
    }

    /// <summary>默认的两条 `g_Config` 时间字段（原文 M2Config 默认值：180 s / 2 min）。</summary>
    public static void UseDefaultFloorTimes()
    {
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 180u * 1000u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 2u * 60u * 1000u;
    }
}

/// <summary>
/// 可编排假时钟：每次读 <see cref="Peek"/> **不**自增（默认），
/// 或（<see cref="AutoIncrement"/>）每次自增 1 —— 用来把"原文在同一表达式里
/// 调用 TickCount 两次"这类差异暴露成可断言的**读取次数**与**递增值**。
/// </summary>
public sealed class Sweep9FakeClock
{
    public uint Now { get; set; }

    /// <summary>为真时每次读 <see cref="Peek"/> 累加 1。</summary>
    public bool AutoIncrement { get; set; }

    /// <summary>自 <see cref="ResetCounters"/> 以来的读取次数（计数取证的载荷）。</summary>
    public int ReadCount { get; private set; }

    /// <summary>返回当前值；<see cref="AutoIncrement"/> 时先自增（读取次数照常累计）。</summary>
    public uint Peek()
    {
        ReadCount++;
        if (AutoIncrement) { _counter++; return Now + _counter; }
        return Now;
    }

    /// <summary>清零读取计数与自增偏移（便于在同一用例里分段取证）。</summary>
    public void ResetCounters()
    {
        ReadCount = 0;
        _counter = 0;
    }

    /// <summary>只把"自增偏移"归零（保留 <see cref="ReadCount"/> 累计），
    /// 用于"构造阶段先读了几次、被测动作再读几次"的分段场景。</summary>
    public void ResetCountersKeepingValue() => _counter = 0;

    private uint _counter;
}
