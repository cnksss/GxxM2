using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J117：造怪主体 `RegenMonsters`（UsrEngn.pas 6352-6478）1:1 测试。
/// </summary>
public sealed class RegenMonstersCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(10, RegenMonstersCore.MissionJitter);
        Assert.Equal(20, RegenMonstersCore.MissionJitterModulus);
        Assert.Equal(100, RegenMonstersCore.MissionDiceMax);
        Assert.Equal("[Exception] TUserEngine.RegenMonsters", RegenMonstersCore.ExceptionMessage);
    }

    // ===================== 入口条件（6367） =====================

    [Fact]
    public void EntryRequiresThreeConditions()
    {
        Assert.True(RegenMonstersCore.ShouldCreateMonsters(false, 100, 5));

        Assert.False(RegenMonstersCore.ShouldCreateMonsters(true, 100, 5));    // MonGen 为 nil
        Assert.False(RegenMonstersCore.ShouldCreateMonsters(false, 0, 5));     // **nRace 必须 > 0**
        Assert.False(RegenMonstersCore.ShouldCreateMonsters(false, -1, 5));    // 负种族号
        Assert.False(RegenMonstersCore.ShouldCreateMonsters(false, 100, 0));   // nCount 必须 > 0
        Assert.False(RegenMonstersCore.ShouldCreateMonsters(false, 100, -3));
    }

    [Fact]
    public void InvalidParamsStillReturnsTrue()
    {
        // 6364：Result := True 是初值且在 try 之外 → "没造怪"也报告成功
        Assert.True(RegenMonstersCore.InvalidParamsStillReturnsTrue());
        Assert.True(RegenMonstersCore.InitialTrueIsOutsideTry());
    }

    [Fact]
    public void InvalidParamsRoundIsNoOp()
    {
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            MonGenIsNull = false,
            NRace = 0,
            NCount = 10,
        });

        Assert.False(b.Entered);
        Assert.Equal(0, b.SpawnedCount);
        Assert.True(b.Result);   // **仍为 True**
    }

    [Fact]
    public void ZeroRaceBlocksSpawn()
    {
        // nRace 为 0 是 J108 解析失败的行
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 0,
            NCount = 3,
            MonGenEnvir = new object(),
        });

        Assert.False(b.Entered);
        Assert.True(b.Result);
    }

    // ===================== 地图解析（6369-6373） =====================

    [Fact]
    public void UsesEnvirWhenPresent()
    {
        bool lookupCalled = false;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 1,
            MonGenEnvir = new object(),
            MapLookup = () => { lookupCalled = true; return new object(); },
        });

        Assert.True(b.Entered);
        Assert.False(lookupCalled);        // Envir 非 nil → 不查图
        Assert.False(b.UsedMapLookup);
    }

    [Fact]
    public void FallsBackToMapLookupWhenEnvirNull()
    {
        // 6369-6372：**又一条容错路径**（空挂靠点之外的）
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 1,
            MonGenEnvir = null,
            MapLookup = () => new object(),
        });

        Assert.True(b.Entered);
        Assert.True(b.UsedMapLookup);
    }

    [Fact]
    public void MapLookupFailureSkipsEverything()
    {
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 5,
            MonGenEnvir = null,
            MapLookup = () => null,
        });

        Assert.False(b.Entered);
        Assert.Equal(0, b.SpawnedCount);
        Assert.True(b.Result);   // **仍为 True**
    }

    // ===================== 分支选择（6375） =====================

    [Fact]
    public void MissionBranchRequiresPositiveRateAndDiceUnder()
    {
        Assert.True(RegenMonstersCore.IsMissionBatch(50, 0));
        Assert.True(RegenMonstersCore.IsMissionBatch(50, 49));
        Assert.False(RegenMonstersCore.IsMissionBatch(50, 50));   // 严格 <
        Assert.False(RegenMonstersCore.IsMissionBatch(50, 99));
    }

    [Fact]
    public void ZeroRateNeverMission()
    {
        Assert.False(RegenMonstersCore.IsMissionBatchWithoutRoll(0));
        Assert.False(RegenMonstersCore.IsMissionBatchWithoutRoll(-5));
        Assert.True(RegenMonstersCore.IsMissionBatchWithoutRoll(1));
    }

    [Fact]
    public void DiceRolledOncePerBatchNotPerMonster()
    {
        // **整批只掷一次**：3 个怪 → 骰子只被调用 1 次
        int diceCalls = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 3,
            MonGenEnvir = new object(),
            MissionGenRate = 50,
            RollDice = () => { diceCalls++; return 0; },
        });

        Assert.Equal(1, diceCalls);
        Assert.True(b.MissionBranch);
    }

    [Fact]
    public void MissionBatchSharesOneCoord()
    {
        // 任务分支坐标在循环外算一次 → 整批同坐标
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 3,
            MonGenEnvir = new object(),
            MissionGenRate = 50,
            RollDice = () => 0,
            BaseX = 100,
            BaseY = 200,
            RollRange = _ => 0,
            RollJitter = () => 0,
        });

        Assert.Equal(3, b.Coords.Count);
        Assert.Equal(b.Coords[0], b.Coords[1]);
        Assert.Equal(b.Coords[1], b.Coords[2]);
    }

    [Fact]
    public void NormalBatchRecoordsEachMonster()
    {
        // 常规分支每次迭代重掷
        int roll = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 3,
            MonGenEnvir = new object(),
            MissionGenRate = 0,
            BaseX = 100,
            BaseY = 100,
            Range = 0,
            MapWidth = 1000,
            MapHeight = 1000,
            RollRange = _ => (roll++) % 5,
        });

        var distinct = new HashSet<(int, int)>(b.Coords);
        Assert.True(distinct.Count > 1);
    }

    // ===================== 坐标生成 =====================

    [Fact]
    public void RangeModulusIsTwoRangePlusOne()
    {
        Assert.Equal(1, RegenMonstersCore.RangeModulus(0));
        Assert.Equal(5, RegenMonstersCore.RangeModulus(2));
        Assert.Equal(21, RegenMonstersCore.RangeModulus(10));
    }

    [Fact]
    public void MissionRangeIncludesBothEndpoints()
    {
        // Random(range*2+1) ∈ [0, 2*range] → 坐标 ∈ [base-range, base+range] **闭区间**
        int baseC = 100, range = 3;

        Assert.Equal(97, RegenMonstersCore.MissionTempCoord(baseC, range, 0));
        Assert.Equal(103, RegenMonstersCore.MissionTempCoord(baseC, range, 6));
    }

    [Fact]
    public void MissionJitterRange()
    {
        // (temp - 10) + Random(20) → [temp-10, temp+9]
        Assert.Equal(90, RegenMonstersCore.MissionFinalCoord(100, 0));
        Assert.Equal(109, RegenMonstersCore.MissionFinalCoord(100, 19));
    }

    [Fact]
    public void MissionBranchDoesNotClamp()
    {
        // 原文任务分支**无边界钳制**：可能为负
        int x = RegenMonstersCore.MissionFinalCoord(
            RegenMonstersCore.MissionTempCoord(0, 0, 0), 0);

        Assert.Equal(-10, x);
        Assert.True(RegenMonstersCore.MissionBranchDoesNotClamp());
    }

    [Fact]
    public void NormalClampLow()
    {
        Assert.Equal(0, RegenMonstersCore.ClampCoord(-5, 1000));
    }

    [Fact]
    public void NormalClampHigh()
    {
        Assert.Equal(999, RegenMonstersCore.ClampCoord(1000, 1000));
        Assert.Equal(999, RegenMonstersCore.ClampCoord(50_000, 1000));
    }

    [Fact]
    public void NormalClampInRangeUntouched()
    {
        Assert.Equal(500, RegenMonstersCore.ClampCoord(500, 1000));
        Assert.Equal(0, RegenMonstersCore.ClampCoord(0, 1000));
        Assert.Equal(999, RegenMonstersCore.ClampCoord(999, 1000));
    }

    [Fact]
    public void NormalCoordClampsBothAxes()
    {
        var (x, y) = RegenMonstersCore.NormalCoord(0, 0, 100, 0, 0, 50, 60);
        Assert.Equal(0, x);
        Assert.Equal(0, y);

        var (x2, y2) = RegenMonstersCore.NormalCoord(0, 0, 100, 0, 0, 50, 60);
        Assert.True(x2 <= 49);
        Assert.True(y2 <= 59);
    }

    [Fact]
    public void NormalBranchClamps()
    {
        Assert.True(RegenMonstersCore.NormalBranchClamps());
    }

    [Fact]
    public void TwoBranchesDifferOnClamping()
    {
        // **差异保护**：同一输入下任务分支出负值，常规分支被钳到 0
        int mission = RegenMonstersCore.MissionFinalCoord(
            RegenMonstersCore.MissionTempCoord(0, 0, 0), 0);
        var (normalX, _) = RegenMonstersCore.NormalCoord(0, 0, 0, 0, 0, 100, 100);

        Assert.True(mission < 0);
        Assert.Equal(0, normalX);
    }

    // ===================== 六个字段（6386-6391） =====================

    [Fact]
    public void SpawnFieldsCopiedVerbatim()
    {
        var src = new RegenMonstersCore.SpawnSource
        {
            BoIsNGMon = true,
            BtNameColor = 200,
            NationaID = "3",
            BoCanAttackSameNationPlayer = true,
            BoAllowSameNationPlayerAttack = false,
            BoNoSameNationMonPK = true,
        };

        var f = RegenMonstersCore.MakeSpawnFields(src);

        Assert.True(f.BoISNGMonster);
        Assert.Equal(200, f.BtNameColor);
        Assert.Equal(3, f.BtNation);
        Assert.True(f.BoCanAttackSameNationPlayer);
        Assert.False(f.BoAllowSameNationPlayerAttack);
        Assert.True(f.BoNoSameNationMonPK);
    }

    [Fact]
    public void NationParseDefaultIsZero()
    {
        // 6388：`StrToIntDef(sNationaID, 0)` —— **默认 0，不是 -1**
        Assert.Equal(0, RegenMonstersCore.ParseNation(""));
        Assert.Equal(0, RegenMonstersCore.ParseNation("abc"));
        Assert.Equal(5, RegenMonstersCore.ParseNation("5"));
        Assert.Equal(-2, RegenMonstersCore.ParseNation("-2"));
    }

    [Fact]
    public void BothBranchesSetSameSixFields()
    {
        // 两分支的字段设置**完全相同**：用同一 source 走两分支，结果一致
        var src = new RegenMonstersCore.SpawnSource
        {
            BoIsNGMon = true,
            BtNameColor = 77,
            NationaID = "9",
            BoCanAttackSameNationPlayer = true,
            BoAllowSameNationPlayerAttack = true,
            BoNoSameNationMonPK = false,
        };

        var a = RegenMonstersCore.MakeSpawnFields(src);
        var b = RegenMonstersCore.MakeSpawnFields(src);

        Assert.Equal(a.BoISNGMonster, b.BoISNGMonster);
        Assert.Equal(a.BtNameColor, b.BtNameColor);
        Assert.Equal(a.BtNation, b.BtNation);
        Assert.Equal(a.BoCanAttackSameNationPlayer, b.BoCanAttackSameNationPlayer);
        Assert.Equal(a.BoAllowSameNationPlayerAttack, b.BoAllowSameNationPlayerAttack);
        Assert.Equal(a.BoNoSameNationMonPK, b.BoNoSameNationMonPK);
    }

    [Fact]
    public void AddsToCertListOnlyWhenCertNotNull()
    {
        Assert.True(RegenMonstersCore.AddsToCertList(false));
        Assert.False(RegenMonstersCore.AddsToCertList(true));
    }

    [Fact]
    public void NullCertContinuesLoop()
    {
        // 某个坐标造怪失败不影响其余
        int calls = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 4,
            MonGenEnvir = new object(),
            AddBaseObject = (_, _) => { calls++; return calls != 2; },   // 第二个失败
        });

        Assert.Equal(4, calls);
        Assert.Equal(3, b.SpawnedCount);
        Assert.True(RegenMonstersCore.NullCertContinuesLoop());
    }

    [Fact]
    public void NullCertSkipsFieldsButContinuesLoop()
    {
        int ctxCalls = 0;
        var ctx = new RegenMonstersCore.ScriptContext();

        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 3,
            MonGenEnvir = new object(),
            AddBaseObject = (_, _) => false,      // 全部失败
            TriggerScript = "@test",
            ScriptCtx = ctx,
        });

        Assert.Equal(3, b.Iterations);
        Assert.Equal(0, b.SpawnedCount);
        Assert.Equal(0, ctxCalls);
        Assert.Equal(0, b.ScriptTriggers);
    }

    // ===================== 触发脚本（6393-6407） =====================

    [Fact]
    public void TriggerScriptRequiresThreeConditions()
    {
        Assert.True(RegenMonstersCore.ShouldRunTriggerScript("@a", false, false));

        Assert.False(RegenMonstersCore.ShouldRunTriggerScript("", false, false));
        Assert.False(RegenMonstersCore.ShouldRunTriggerScript("@a", true, false));
        Assert.False(RegenMonstersCore.ShouldRunTriggerScript("@a", false, true));
    }

    [Fact]
    public void ScriptContextSetSixItems()
    {
        var ctx = new RegenMonstersCore.ScriptContext { ScriptGotoCount = 99 };
        RegenMonstersCore.SetScriptContext(ctx, "鸡", "0", "比奇", 10, 20);

        Assert.Equal("鸡", ctx.RegMonName);
        Assert.Equal("0", ctx.RegMonMap);
        Assert.Equal("比奇", ctx.RegMonMapDesc);
        Assert.Equal(10, ctx.RegMonX);
        Assert.Equal(20, ctx.RegMonY);
        Assert.Equal(0, ctx.ScriptGotoCount);   // **归零**
    }

    [Fact]
    public void ScriptContextClearRemovesFiveNotGotoCount()
    {
        var ctx = new RegenMonstersCore.ScriptContext();
        RegenMonstersCore.SetScriptContext(ctx, "鸡", "0", "比奇", 10, 20);
        RegenMonstersCore.ClearScriptContext(ctx);

        Assert.Equal("", ctx.RegMonName);
        Assert.Equal("", ctx.RegMonMap);
        Assert.Equal("", ctx.RegMonMapDesc);
        Assert.Equal(0, ctx.RegMonX);
        Assert.Equal(0, ctx.RegMonY);
        Assert.Equal(0, ctx.ScriptGotoCount);   // **未在清除之列**（保留归零后的 0）
        Assert.True(RegenMonstersCore.ScriptGotoCountSurvivesCleanup());
    }

    [Fact]
    public void ScriptGotoCountNotResetToPreviousValue()
    {
        // 清空**不会**把 ScriptGotoCount 还原为旧值
        var ctx = new RegenMonstersCore.ScriptContext { ScriptGotoCount = 42 };
        RegenMonstersCore.SetScriptContext(ctx, "鸡", "0", "d", 1, 1);
        RegenMonstersCore.ClearScriptContext(ctx);

        Assert.Equal(0, ctx.ScriptGotoCount);   // 仍是归零后的 0，不是 42
    }

    [Fact]
    public void ScriptTriggeredPerMonster()
    {
        var ctx = new RegenMonstersCore.ScriptContext();
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 3,
            MonGenEnvir = new object(),
            TriggerScript = "@regen",
            ScriptCtx = ctx,
        });

        Assert.Equal(3, b.ScriptTriggers);
    }

    [Fact]
    public void ScriptNotTriggeredWhenCertNull()
    {
        var ctx = new RegenMonstersCore.ScriptContext();
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 2,
            MonGenEnvir = new object(),
            TriggerScript = "@regen",
            ScriptCtx = ctx,
            AddBaseObject = (_, _) => false,
        });

        Assert.Equal(0, b.ScriptTriggers);
    }

    [Fact]
    public void MissionScriptUsesFinalCoords()
    {
        // 6398-6399：传的是抖动后的 nX/nY，与造怪坐标一致
        Assert.True(RegenMonstersCore.MissionScriptUsesFinalCoords());
    }

    // ===================== 超时中断（6414-6418） =====================

    [Fact]
    public void ZenLimitComparisonIsStrict()
    {
        Assert.False(RegenMonstersCore.IsZenLimitExceeded(1000, 0, 1000));   // 相等不算超
        Assert.True(RegenMonstersCore.IsZenLimitExceeded(1001, 0, 1000));
    }

    [Fact]
    public void TimeoutSetsFalseAndBreaks()
    {
        Assert.True(RegenMonstersCore.TimeoutSetsResultFalse());
        Assert.True(RegenMonstersCore.TimeoutBreaksLoop());
    }

    [Fact]
    public void TimeoutBreaksAfterFirstMonster()
    {
        int spawned = 0;
        int tick = 0;

        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 10,
            MonGenEnvir = new object(),
            ZenLimit = 100,
            Clock = () => tick,
            AddBaseObject = (_, _) => { spawned++; tick += 200; return true; },
        });

        Assert.True(b.TimedOut);
        Assert.False(b.Result);
        Assert.Equal(1, b.Iterations);
        Assert.Equal(1, spawned);
    }

    [Fact]
    public void TimeoutKeepsAlreadySpawnedMonsters()
    {
        // 已造出的怪**不回收**：CertList 里仍有它们
        int spawned = 0;
        int tick = 0;

        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 10,
            MonGenEnvir = new object(),
            ZenLimit = 100,
            Clock = () => tick,
            AddBaseObject = (_, _) => { spawned++; tick += 200; return true; },
        });

        Assert.Equal(1, b.CertListCount);   // **保留**
        Assert.True(RegenMonstersCore.TimeoutKeepsAlreadySpawnedMonsters());
    }

    [Fact]
    public void LastMonsterHasNoTimeoutExemption()
    {
        // 即便这是最后一个怪，超时照样置 False
        int tick = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 1,
            MonGenEnvir = new object(),
            ZenLimit = 100,
            Clock = () => tick,
            AddBaseObject = (_, _) => { tick += 200; return true; },
        });

        Assert.False(b.Result);   // 唯一一个怪也已造出，但报告失败
        Assert.True(b.TimedOut);
        Assert.Equal(1, b.SpawnedCount);
        Assert.True(RegenMonstersCore.LastMonsterHasNoTimeoutExemption());
    }

    [Fact]
    public void TimingStartsAtFunctionEntry()
    {
        Assert.True(RegenMonstersCore.TimingStartsAtFunctionEntry());
    }

    [Fact]
    public void NoTimeoutWhenWithinLimit()
    {
        int tick = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 3,
            MonGenEnvir = new object(),
            ZenLimit = 1000,
            Clock = () => tick,
            AddBaseObject = (_, _) => { tick += 10; return true; },
        });

        Assert.False(b.TimedOut);
        Assert.True(b.Result);
        Assert.Equal(3, b.Iterations);
    }

    [Fact]
    public void TimeoutCheckRunsEvenWhenCertNull()
    {
        // 6414 在 if Cert 块之外 → 造怪失败也检查超时
        int tick = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 5,
            MonGenEnvir = new object(),
            ZenLimit = 100,
            Clock = () => { tick += 150; return tick; },
            AddBaseObject = (_, _) => false,
        });

        Assert.True(b.TimedOut);
        Assert.Equal(0, b.SpawnedCount);
    }

    // ===================== 异常（6475-6477） =====================

    [Fact]
    public void ExceptionKeepsCurrentResult()
    {
        Assert.True(RegenMonstersCore.ExceptionKeepsCurrentResult());
        Assert.True(RegenMonstersCore.ExceptionBeforeTimeoutReturnsTrue());
    }

    // ===================== 端到端 =====================

    [Fact]
    public void EndToEndNormalBatch()
    {
        var ctx = new RegenMonstersCore.ScriptContext();
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 5,
            MonGenEnvir = new object(),
            BaseX = 300,
            BaseY = 300,
            Range = 10,
            MapWidth = 1000,
            MapHeight = 1000,
            RollRange = r => r / 2,
            TriggerScript = "@regen",
            ScriptCtx = ctx,
        });

        Assert.True(b.Entered);
        Assert.False(b.MissionBranch);
        Assert.Equal(5, b.Iterations);
        Assert.Equal(5, b.SpawnedCount);
        Assert.Equal(5, b.CertListCount);
        Assert.Equal(5, b.ScriptTriggers);
        Assert.True(b.Result);
    }

    [Fact]
    public void EndToEndMissionBatch()
    {
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 4,
            MonGenEnvir = new object(),
            MissionGenRate = 100,      // 必中
            RollDice = () => 0,
            BaseX = 300,
            BaseY = 300,
            Range = 0,
            RollRange = _ => 0,
            RollJitter = () => 10,
        });

        Assert.True(b.MissionBranch);
        Assert.Equal(4, b.SpawnedCount);
        Assert.All(b.Coords, c => Assert.Equal((300, 300), c));
    }

    [Fact]
    public void EndToEndReturnValueFeedsJ116()
    {
        // 超时 → Result False → J116 的 boRegened 被覆盖为 False → **不刷新 dwStartTick**
        int tick = 0;
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 100,
            NCount = 5,
            MonGenEnvir = new object(),
            ZenLimit = 50,
            Clock = () => tick,
            AddBaseObject = (_, _) => { tick += 100; return true; },
        });

        Assert.False(b.Result);
        Assert.False(RegenMonstersLoopCore.ShouldRefreshStartTick(b.Result));
    }

    [Fact]
    public void EndToEndNoSpawnStillRefreshesInJ116()
    {
        // 参数不合法 → 本函数返回 True → J116 仍刷新
        var b = RegenMonstersCore.RunBatch(new RegenMonstersCore.SpawnRequest
        {
            NRace = 0,
            NCount = 5,
        });

        Assert.True(b.Result);
        Assert.True(RegenMonstersLoopCore.ShouldRefreshStartTick(b.Result));
    }
}
