using System;
using System.Collections.Generic;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p16-m2-tmonster-run 的测试夹具：`TMonster.Run`（`ObjMon.pas:1121-1379`）用到的
/// 三个接缝的**可观测替身**：
/// <list type="number">
///   <item><see cref="IMonsterRunWorld"/> —— `Think`/`AttackTarget`/`SpaceMove`/`PickRangeItem`/`StartPickUpItem`/`DelTargetCreat`/`CanMove`；</item>
///   <item><see cref="IMonsterRunConfig"/> —— `g_Config` 的四个宠物开关 + `Self is TCustomMonster` 一族；</item>
///   <item><see cref="IMonsterRunEnvirView"/> —— 地图侧的 `m_boGuardianLevel`/`m_boMirror`/`m_boNoAutoRangePickItem`/`GetMovingObject`。</item>
/// </list>
/// 每个成员都记录**调用次数与调用轨迹**，从而让"某分支到底进没进"变成可断言的硬事实
/// （而不是只看最终副作用）。
/// </summary>
public sealed class FakeMonsterRunWorld : IMonsterRunWorld
{
    /// <summary>调用轨迹（按发生顺序），诊断与"控制流穿过哪几段"的证据。</summary>
    public readonly List<string> Trace = new();

    public int CanMoveCalls;
    public int ThinkCalls;
    public int AttackTargetCalls;
    public int SpaceMoveCalls;
    public int PickRangeItemCalls;
    public int StartPickUpItemCalls;
    public int DelTargetCreatCalls;

    /// <summary>各方法的返回值（原文里都是 Boolean 结果）。</summary>
    public bool CanMoveResult = true;
    public bool ThinkResult;
    public bool AttackTargetResult;
    public bool PickRangeItemResult;
    public bool StartPickUpItemResult;

    /// <summary>最近一次 `SpaceMove` 的实参（原文 `(sMapName, nX, nY, nInt)`）。</summary>
    public (string Map, int X, int Y, int Int)? LastSpaceMove;

    /// <summary>最近一次 `PickRangeItem` 的四个实参（原文 1166/1242/1262 三处口径不同）。</summary>
    public (bool All, bool Drop, bool Scatter, uint Time)? LastPickRangeItem;

    /// <summary>最近一次 `StartPickUpItem` 的三个实参（三参形态）。</summary>
    public (bool Drop, bool Scatter, uint Time)? LastStartPickUpItem;

    /// <summary>标记 1169 的四参形态确实被走到（由测试在调用后设置）。</summary>
    public bool SawFourArgStartPickUpItem;

    public bool CanMove(TMonster self)
    {
        Trace.Add("CanMove");
        CanMoveCalls++;
        return CanMoveResult;
    }

    public bool Think(TMonster self)
    {
        Trace.Add("Think");
        ThinkCalls++;
        return ThinkResult;
    }

    public bool AttackTarget(TMonster self)
    {
        Trace.Add("AttackTarget");
        AttackTargetCalls++;
        return AttackTargetResult;
    }

    public void SpaceMove(TMonster self, string sMapName, int nX, int nY, int nInt)
    {
        Trace.Add("SpaceMove");
        SpaceMoveCalls++;
        LastSpaceMove = (sMapName, nX, nY, nInt);
    }

    public bool PickRangeItem(TMonster self, bool boSlaveAutoPickAll, bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime)
    {
        Trace.Add("PickRangeItem");
        PickRangeItemCalls++;
        LastPickRangeItem = (boSlaveAutoPickAll, boAutoPickPlayDropItem, boAutoPickPlayScatterItem, dwAutoPickScatterToPickTime);
        return PickRangeItemResult;
    }

    public bool StartPickUpItem(TMonster self, bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime)
    {
        // 三参形态（1245/1266）：返回值被调用方检查。
        Trace.Add("StartPickUpItem");
        StartPickUpItemCalls++;
        LastStartPickUpItem = (boAutoPickPlayDropItem, boAutoPickPlayScatterItem, dwAutoPickScatterToPickTime);
        return StartPickUpItemResult;
    }

    /// <summary>四参形态（1169）由 <c>TMonster.StartPickUpItem(..., bool)</c> 走过。</summary>
    public void NoteFourArgStartPickUpItem(TMonster self)
    {
        Trace.Add("StartPickUpItem4");
        SawFourArgStartPickUpItem = true;
    }

    public void DelTargetCreat(TMonster self)
    {
        Trace.Add("DelTargetCreat");
        DelTargetCreatCalls++;
    }

    public void NoteFourArgStartPickUpItem() => SawFourArgStartPickUpItem = true;

    public void Reset()
    {
        Trace.Clear();
        CanMoveCalls = ThinkCalls = AttackTargetCalls = SpaceMoveCalls = 0;
        PickRangeItemCalls = StartPickUpItemCalls = DelTargetCreatCalls = 0;
        LastSpaceMove = null;
        LastPickRangeItem = null;
        LastStartPickUpItem = null;
        SawFourArgStartPickUpItem = false;
    }
}

/// <summary>`g_Config` 替身：五个开关 + `Self is TCustomMonster` 一族。</summary>
public sealed class FakeMonsterRunConfig : IMonsterRunConfig
{
    public bool boPetQuickPickup { get; set; }
    public bool boEnabledPetPickup { get; set; }
    public bool boPetRangePickup { get; set; }
    public bool boPetSleepControlBySlave { get; set; }
    public int g_nKey_UseClientPickItems { get; set; } = 1;

    public bool IsCustomMonsterValue { get; set; }
    public bool IsNoMoveCustomMonsterValue { get; set; }
    public int MinAttackNearRangeValue { get; set; } = 1;

    public bool IsCustomMonster(TMonster self) => IsCustomMonsterValue;
    public bool IsNoMoveCustomMonster(TMonster self) => IsNoMoveCustomMonsterValue;
    public int GetMinAttackNearRange(TMonster self) => MinAttackNearRangeValue;
}

/// <summary>地图侧替身（`m_boGuardianLevel` / `m_boMirror` / `m_boNoAutoRangePickItem` / `GetMovingObject`）。</summary>
public sealed class FakeMonsterRunEnvirView : IMonsterRunEnvirView
{
    public bool m_boGuardianLevel { get; set; }
    public bool m_boMirror { get; set; }
    public bool m_boNoAutoRangePickItem { get; set; }

    /// <summary>被占住的格子集合；`GetMovingObject` 命中即返回一个非 null 对象。</summary>
    public readonly HashSet<(int X, int Y)> Occupied = new();

    public int GetMovingObjectCalls;

    public TCreature? GetMovingObject(int nX, int nY, bool boFlag)
    {
        GetMovingObjectCalls++;
        return Occupied.Contains((nX, nY)) ? Dummy : null;
    }

    private static readonly TCreature Dummy = new ProbeCreature();

    private sealed class ProbeCreature : TCreature
    {
        public ProbeCreature()
        {
            m_nCurrX = -999;
            m_nCurrY = -999;
        }
    }
}
