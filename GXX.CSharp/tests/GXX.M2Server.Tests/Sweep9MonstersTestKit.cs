// ============================================================================
//  车道 p9-m2-monsters 测试工具：**内存文件系统 + 可控时钟 + 记录型接缝**
//
//  设计依据：
//    * 复用 p3-m2-sweep 已并入 main 的 `SweepTestFs`（内存文件）与 `SweepSeam`（不另造一套）；
//    * 本文件只**安装/恢复**接缝，不复制任何实现；
//    * 与 `SweepTestEnv` 同一原则：**单测不碰磁盘**。
//
//  ⚠ 跨区事项（不在本车道分区，**未改**，已登记到报告）：
//    `tests/GXX.M2Server.Tests/M2ConfigIsolationCoverage.cs` 的静态全局清单里
//    **没有** `ObjRobotSeam`（本车道新增的静态接缝）。本车道的每个用例自己用
//    `using var env = new MonstersTestEnv();` 安装/还原；建议集成方把
//    `typeof(ObjRobotSeam)` 一并纳入该清单（一行），以取得"不靠自觉"的双保险。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;
using GXX.M2Server.Sweep9.Monsters;

namespace GXX.M2Server.Tests;

/// <summary>记录型 <see cref="IRobotNpcSeam"/>（原文 `g_RobotNPC`）。</summary>
internal sealed class FakeRobotNpc : IRobotNpcSeam
{
    /// <summary>收到的 (PlayObject, sLabel, boExtJmp) 列表。</summary>
    public readonly List<(TPlayObject? Player, string Label, bool ExtJmp)> Calls = new();

    /// <summary>非 null 时 <see cref="GotoLable"/> 抛出该异常（用于覆盖 <c>TRobotManage.Run</c> 的 except 路径）。</summary>
    public Exception? ThrowOnGoto;

    public void GotoLable(TPlayObject playObject, string sLabel, bool boExtJmp)
    {
        if (ThrowOnGoto != null) throw ThrowOnGoto;
        Calls.Add((playObject, sLabel, boExtJmp));
    }
}

/// <summary>
/// 安装/恢复本车道接缝的测试环境（内存文件 + 可控时钟 + 可控日历）。
/// </summary>
internal sealed class MonstersTestEnv : IDisposable
{
    /// <summary>内存文件系统（复用 p3-m2-sweep 的 <see cref="SweepTestFs"/>）。</summary>
    public readonly SweepTestFs Fs = new();

    /// <summary>内存文本文件：路径 → 行（原文 `TStringList.LoadFromFile` 的替身落点）。</summary>
    public readonly Dictionary<string, string[]> TextFiles = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>`MainOutMessage` 收到的日志。</summary>
    public readonly List<string> Log = new();

    /// <summary>记录型 `g_RobotNPC`。</summary>
    public readonly FakeRobotNpc RobotNpc = new();

    /// <summary>可控 `MyGetTickCount` 的当前值。</summary>
    public uint Now;

    /// <summary>可控 `DecodeTime(Time, ...)` 的结果。</summary>
    public ushort DecodeHour;
    public ushort DecodeMin;
    public ushort DecodeSec;
    public ushort DecodeMSec;

    /// <summary>可控 `DayOfTheWeek(Now)`。</summary>
    public ushort Week;

    /// <summary>`g_MapManager.FindMap` 的返回值（非 null 即"地图存在"）。</summary>
    public TEnvirnoment? Map = new TEnvirnoment();

    /// <summary>本环境钉死的 `M2Config.sEnvirDir`（与 `SweepTestEnv.CanonicalEnvirDir` 一致）。</summary>
    public const string CanonicalEnvirDir = ".\\Envir\\";

    private readonly string _savedEnvirDir;

    public MonstersTestEnv()
    {
        _savedEnvirDir = M2Config.sEnvirDir;
        M2Config.sEnvirDir = CanonicalEnvirDir;

        ObjRobotSeam.ResetDefaults();
        SweepSeam.ResetDefaults();

        SweepSeam.FileExists = path => Fs.FileExists(path) || TextFiles.ContainsKey(path);
        SweepSeam.MainOutMessage = msg => Log.Add(msg);
        SweepSeam.MyGetTickCount = () => Now;

        ObjRobotSeam.g_RobotNPC = RobotNpc;
        ObjRobotSeam.FindMap = _ => Map;
        ObjRobotSeam.LoadFromFile = (TStringList list, string fileName) =>
        {
            if (TextFiles.TryGetValue(fileName, out var lines))
                foreach (var line in lines) list.Add(line);
        };
        ObjRobotSeam.Time = () => new DateTime(2026, 9, 21, 0, 0, 0);
        ObjRobotSeam.Now = () => new DateTime(2026, 9, 21, 0, 0, 0);
        ObjRobotSeam.DecodeTime = (DateTime dt, out ushort h, out ushort m, out ushort s, out ushort ms) =>
        {
            h = DecodeHour; m = DecodeMin; s = DecodeSec; ms = DecodeMSec;
        };
        ObjRobotSeam.DayOfTheWeek = _ => Week;
    }

    /// <summary>预置一个内存文本文件。</summary>
    public void SeedText(string path, params string[] lines) => TextFiles[path] = lines;

    /// <summary>造一个"已连接地图"的机器人对象。</summary>
    public TRobotObject MakeRobot(string scriptFileName = "test")
        => new()
        {
            m_sCharName = "机器人",
            m_sMapName = "0",
            m_PEnvir = Map,
            m_sScriptFileName = scriptFileName,
        };

    public void Dispose()
    {
        ObjRobotSeam.ResetDefaults();
        SweepSeam.ResetDefaults();
        M2Config.sEnvirDir = _savedEnvirDir;
    }
}
