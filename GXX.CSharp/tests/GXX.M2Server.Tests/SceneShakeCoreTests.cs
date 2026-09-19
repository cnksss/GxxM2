using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J121：屏幕震动与地图标识脚本命令
/// （NpcActionCmd.pas 23507-23530 SETMAPQUEST / 23536-23596 SCENESHAKE /
/// LocalDB.pas 6451-6458 / Envir.pas 4253-4345, 5527-5560 /
/// ObjBase.pas 33406-33414 / UsrEngn.pas 10037-10055）1:1 测试。
/// </summary>
public sealed class SceneShakeCoreTests
{
    private static SceneShakeCore.ShakeEntry Entry(
        int count = 1, int curCount = 0, uint lastTick = 0,
        string name = "", bool enable = false)
        => new()
        {
            Count = count,
            CurCount = curCount,
            LastTick = lastTick,
            PlayerName = name,
            EnableClientOption = enable,
        };

    private static SceneShakeCore.ShakePlayer P(
        string name, object? envir = null, bool ghost = false, bool death = false,
        bool offline = false, bool dummy = false, int range = 9)
        => new()
        {
            Name = name,
            Penvir = envir,
            BoGhost = ghost,
            BoDeath = death,
            BoOffLine = offline,
            BoDummyObject = dummy,
            ViewRange = range,
        };

    private static Func<SceneShakeCore.ShakePlayer, List<SceneShakeCore.ShakePlayer>> RangeOf(
        Dictionary<string, List<SceneShakeCore.ShakePlayer>> map)
        => p => map.TryGetValue(p.Name, out var l) ? l : new List<SceneShakeCore.ShakePlayer>();

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(326, SceneShakeCore.NaSetMapQuest);
        Assert.Equal(328, SceneShakeCore.NaSceneShake);
        Assert.Equal("SETMAPQUEST", SceneShakeCore.SetMapQuestCommand);
        Assert.Equal("SCENESHAKE", SceneShakeCore.SceneShakeCommand);
        Assert.Equal(20220, SceneShakeCore.RmSceneShake);
        Assert.Equal(8901, SceneShakeCore.SmSceneShake);
        Assert.Equal(-1, SceneShakeCore.ShakeTypeDefault);
        Assert.Equal(320u, SceneShakeCore.ShakeThrottleMs);
        Assert.Equal(1, SceneShakeCore.MinShakeCount);
    }

    [Fact]
    public void ExceptionMessageMatches()
    {
        Assert.Equal("{异常} TNormNpc.ActionOfSceneShake", SceneShakeCore.ExceptionMessage);
    }

    [Fact]
    public void SceneShakeInfoFieldsInOrder()
    {
        Assert.Equal(new[]
        {
            "LastTick", "Count", "CurCount", "PlayerName", "EnableClientOption",
        }, SceneShakeCore.SceneShakeInfoFields);
    }

    // ===================== SETMAPQUEST：空体 =====================

    [Fact]
    public void SetMapQuestBodyIsEmpty()
    {
        Assert.True(SceneShakeCore.SetMapQuestBodyIsEmpty());
        Assert.Contains("SetQuestFlagStatus", SceneShakeCore.CommentedSetQuestFlagStatus);
        Assert.StartsWith("//", SceneShakeCore.CommentedSetQuestFlagStatus);
    }

    [Fact]
    public void SetMapQuestHasNoEffect()
    {
        Assert.True(SceneShakeCore.SetMapQuestHasNoEffect(new object(), 1, 2));
        Assert.True(SceneShakeCore.SetMapQuestHasNoEffect(null, 0, 0));
    }

    [Fact]
    public void SetMapQuestEmptyParamErrors()
    {
        Assert.True(SceneShakeCore.SetMapQuestEmptyParamErrors(""));
        Assert.False(SceneShakeCore.SetMapQuestEmptyParamErrors("SELF"));
    }

    // ===================== SETMAPQUEST：参数预解析 =====================

    [Fact]
    public void IsStringNumberBehaviour()
    {
        Assert.True(SceneShakeCore.IsStringNumber("0"));
        Assert.True(SceneShakeCore.IsStringNumber("12345"));
        Assert.False(SceneShakeCore.IsStringNumber(""));
        Assert.False(SceneShakeCore.IsStringNumber("1a"));
        Assert.False(SceneShakeCore.IsStringNumber("-1"));
        Assert.False(SceneShakeCore.IsStringNumber("[1]"));
    }

    [Fact]
    public void ArrestBracketExtractsInner()
    {
        Assert.Equal("1", SceneShakeCore.ArrestBracket("[1]"));
        Assert.Equal("123", SceneShakeCore.ArrestBracket("xx[123]yy"));
        Assert.Equal("", SceneShakeCore.ArrestBracket("no bracket"));
        Assert.Equal("", SceneShakeCore.ArrestBracket("[unclosed"));
    }

    [Fact]
    public void OnlyParam2IsArrested()
    {
        Assert.True(SceneShakeCore.OnlyParam2IsArrested());
    }

    [Fact]
    public void SetMapQuestParamValidation()
    {
        // sParam2 带括号（会被 arrest）、sParam3 纯数字 → 通过
        Assert.Equal(326, SceneShakeCore.ParseSetMapQuestParams("[1]", "2"));
    }

    [Fact]
    public void Param2MustBeBracketed()
    {
        // **6453 无条件对 sParam2 做 arrest**：不带方括号 → 取到空串 → 校验失败 → 丢弃。
        // 即 sParam2 **必须**写成 [数字]，否则整条命令被丢。
        Assert.Equal(0, SceneShakeCore.ParseSetMapQuestParams("1", "2"));
        Assert.Equal(326, SceneShakeCore.ParseSetMapQuestParams("[1]", "2"));
    }

    [Fact]
    public void Param3WithBracketsIsRejected()
    {
        // **sParam3 未被 arrest，故带方括号会被拒**
        Assert.True(SceneShakeCore.Param3WithBracketsIsRejected());
        Assert.Equal(0, SceneShakeCore.ParseSetMapQuestParams("[1]", "[2]"));
    }

    [Fact]
    public void Param3PlainNumberAccepted()
    {
        Assert.True(SceneShakeCore.Param3PlainNumberAccepted());
    }

    [Fact]
    public void SetMapQuestDroppedWhenParamNotNumber()
    {
        Assert.Equal(0, SceneShakeCore.ParseSetMapQuestParams("abc", "2"));
        Assert.Equal(0, SceneShakeCore.ParseSetMapQuestParams("[1]", "xyz"));
        Assert.Equal(0, SceneShakeCore.ParseSetMapQuestParams("", "2"));
    }

    [Fact]
    public void SetMapQuestSilentlyDroppedInBothCases()
    {
        Assert.True(SceneShakeCore.SetMapQuestSilentlyDroppedInBothCases());
    }

    [Fact]
    public void ZeroCodeMeansUnknownNotError()
    {
        Assert.True(SceneShakeCore.ZeroCodeMeansUnknownNotError());
    }

    [Fact]
    public void HasSpecialParamParsing()
    {
        Assert.True(SceneShakeCore.HasSpecialParamParsing(326));
        Assert.False(SceneShakeCore.HasSpecialParamParsing(328));
    }

    // ===================== SCENESHAKE：分派 =====================

    [Fact]
    public void ShakeTypeDispatch()
    {
        Assert.Equal(SceneShakeCore.ShakeTarget.Self, SceneShakeCore.SelectShakeTarget(0));
        Assert.Equal(SceneShakeCore.ShakeTarget.AllMaps, SceneShakeCore.SelectShakeTarget(1));
        Assert.Equal(SceneShakeCore.ShakeTarget.ViewRange, SceneShakeCore.SelectShakeTarget(2));
        Assert.Equal(SceneShakeCore.ShakeTarget.CurrentMap, SceneShakeCore.SelectShakeTarget(3));
        Assert.Equal(SceneShakeCore.ShakeTarget.NamedMap, SceneShakeCore.SelectShakeTarget(4));
    }

    [Fact]
    public void InvalidShakeTypeDoesNothing()
    {
        // case 无 else
        Assert.True(SceneShakeCore.InvalidShakeTypeDoesNothing(-1));
        Assert.True(SceneShakeCore.InvalidShakeTypeDoesNothing(5));
        Assert.True(SceneShakeCore.InvalidShakeTypeDoesNothing(100));
    }

    [Fact]
    public void ShakeTypeDefaultIsMinusOne()
    {
        Assert.Equal(-1, SceneShakeCore.ParseShakeType("abc"));
        Assert.Equal(-1, SceneShakeCore.ParseShakeType(""));
        Assert.Equal(2, SceneShakeCore.ParseShakeType("2"));
    }

    [Fact]
    public void CountClamping()
    {
        Assert.Equal(1, SceneShakeCore.ClampShakeCount(0));
        Assert.Equal(1, SceneShakeCore.ClampShakeCount(-1));
        Assert.Equal(5, SceneShakeCore.ClampShakeCount(5));
    }

    [Fact]
    public void NegativeCountClampedToOne()
    {
        Assert.True(SceneShakeCore.NegativeCountClampedToOne());
        Assert.Equal(1, SceneShakeCore.ClampShakeCount(-100));
    }

    [Fact]
    public void CountClampedInAllFiveBranches()
    {
        Assert.True(SceneShakeCore.CountClampedInAllFiveBranches());
    }

    // ===================== SCENESHAKE：参数位置 =====================

    [Fact]
    public void Branch4CountComesFromParam3()
    {
        Assert.True(SceneShakeCore.Branch4CountComesFromParam3());
        Assert.Equal(3, SceneShakeCore.CountParamIndex(SceneShakeCore.ShakeTarget.NamedMap));
    }

    [Fact]
    public void Branch4UsesParam4ForClientOption()
    {
        Assert.True(SceneShakeCore.Branch4UsesParam4ForClientOption());
        Assert.Equal(4, SceneShakeCore.ClientOptionParamIndex(SceneShakeCore.ShakeTarget.NamedMap));
    }

    [Fact]
    public void OtherBranchesUseParam2And3()
    {
        Assert.True(SceneShakeCore.OtherBranchesUseParam2And3());
    }

    [Fact]
    public void Branch4DiffersFromOthers()
    {
        // 分支 4 与前四分支的参数下标**都**不同
        Assert.NotEqual(
            SceneShakeCore.CountParamIndex(SceneShakeCore.ShakeTarget.Self),
            SceneShakeCore.CountParamIndex(SceneShakeCore.ShakeTarget.NamedMap));
        Assert.NotEqual(
            SceneShakeCore.ClientOptionParamIndex(SceneShakeCore.ShakeTarget.Self),
            SceneShakeCore.ClientOptionParamIndex(SceneShakeCore.ShakeTarget.NamedMap));
    }

    [Fact]
    public void ParamEqualsOneIsExactEquality()
    {
        Assert.True(SceneShakeCore.ParamEqualsOne(1));
        Assert.False(SceneShakeCore.ParamEqualsOne(0));
        Assert.False(SceneShakeCore.ParamEqualsOne(2));   // **非 0 即真的等值判断**
    }

    // ===================== SCENESHAKE：不对称与防御 =====================

    [Fact]
    public void Branch0ImmediateVsBranch3Queued()
    {
        Assert.True(SceneShakeCore.Branch0ImmediateVsBranch3Queued());
    }

    [Fact]
    public void Branch2And3LookupDifferences()
    {
        Assert.True(SceneShakeCore.Branch2LooksUpByNameWithNilCheck());
        Assert.True(SceneShakeCore.Branch3DoesNotCheckNil());
        Assert.True(SceneShakeCore.Branch2ChecksNilVsBranch3DoesNot());
    }

    [Fact]
    public void Branch3NilEnvirRaises()
    {
        Assert.True(SceneShakeCore.Branch3NilEnvirRaises(null));
        Assert.False(SceneShakeCore.Branch3NilEnvirRaises(new object()));
    }

    [Fact]
    public void Branch2NilMapSafe()
    {
        Assert.True(SceneShakeCore.Branch2NilMapSafe(null));
        Assert.True(SceneShakeCore.Branch2NilMapSafe(new object()));
    }

    [Fact]
    public void ExceptionSwallowedWithoutBreak()
    {
        Assert.True(SceneShakeCore.ExceptionSwallowedWithoutBreak());
        Assert.True(SceneShakeCore.ScriptContinuesAfterException());
    }

    // ===================== 队列：AddSceneShake =====================

    [Fact]
    public void AddSceneShakeSetsFields()
    {
        var list = new List<SceneShakeCore.ShakeEntry>();

        var e = SceneShakeCore.AddSceneShake(list, 3, "hero", true);

        Assert.NotNull(e);
        Assert.Equal(0u, e!.LastTick);
        Assert.Equal(3, e.Count);
        Assert.Equal(0, e.CurCount);
        Assert.Equal("hero", e.PlayerName);
        Assert.True(e.EnableClientOption);
        Assert.Single(list);
    }

    [Fact]
    public void ZeroCountRejected()
    {
        var list = new List<SceneShakeCore.ShakeEntry>();

        Assert.Null(SceneShakeCore.AddSceneShake(list, 0, "", false));
        Assert.Empty(list);
    }

    [Fact]
    public void ZeroCountRejectedButNegativeAccepted()
    {
        Assert.True(SceneShakeCore.ZeroCountRejectedButNegativeAccepted());
    }

    [Fact]
    public void NewEntryLastTickIsZero()
    {
        Assert.True(SceneShakeCore.NewEntryLastTickIsZero());
    }

    [Fact]
    public void ClearSceneShakeListDisposesAll()
    {
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(), Entry() };

        int n = SceneShakeCore.ClearSceneShakeList(list);

        Assert.Equal(2, n);
        Assert.Empty(list);
    }

    // ===================== 队列：处理循环 =====================

    [Fact]
    public void IsThrottledAt320ms()
    {
        Assert.True(SceneShakeCore.IsThrottled(100, 100));      // 0 < 320
        Assert.True(SceneShakeCore.IsThrottled(419, 100));      // 319 < 320
        Assert.False(SceneShakeCore.IsThrottled(420, 100));     // 320 不 < 320
        Assert.False(SceneShakeCore.IsThrottled(1000, 100));
    }

    [Fact]
    public void IsCompletedWhenCurReachesCount()
    {
        Assert.False(SceneShakeCore.IsCompleted(Entry(count: 3, curCount: 2)));
        Assert.True(SceneShakeCore.IsCompleted(Entry(count: 3, curCount: 3)));
        Assert.True(SceneShakeCore.IsCompleted(Entry(count: 3, curCount: 4)));
    }

    [Fact]
    public void CompletedEntryRemovedBeforeThrottle()
    {
        // LastTick 设为刚刷新 → 若先判节流会被跳过；因先判完成故仍被删除
        var list = new List<SceneShakeCore.ShakeEntry>
        {
            Entry(count: 1, curCount: 1, lastTick: 500),
        };

        var round = SceneShakeCore.ProcessRound(
            list, 500, new object(), _ => null, _ => new(), Array.Empty<SceneShakeCore.ShakePlayer>());

        Assert.Equal(new[] { "completed" }, round.DeletedReasons);
        Assert.Empty(list);
    }

    [Fact]
    public void ThrottledEntryNotAdvancedNotDeleted()
    {
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(count: 3, lastTick: 500) };

        var round = SceneShakeCore.ProcessRound(
            list, 600, new object(), _ => null, _ => new(), Array.Empty<SceneShakeCore.ShakePlayer>());

        Assert.Empty(round.DeletedReasons);
        Assert.Equal(0, round.Advanced);
        Assert.Single(list);
        Assert.Equal(0, list[0].CurCount);
    }

    [Fact]
    public void EntryAdvancesWhenThrottlePassed()
    {
        var envir = new object();
        var hero = P("hero", envir);
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(count: 3, lastTick: 0) };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir,
            n => n == "hero" ? hero : null,
            _ => new List<SceneShakeCore.ShakePlayer>(),
            new[] { hero });

        Assert.Equal(1, round.Advanced);
        Assert.Equal(1, list[0].CurCount);
        Assert.Equal(500u, list[0].LastTick);
    }

    [Fact]
    public void NamedEntryDroppedWhenPlayerGone()
    {
        var envir = new object();
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(name: "gone", lastTick: 0) };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir, _ => null, _ => new(), Array.Empty<SceneShakeCore.ShakePlayer>());

        Assert.Equal(new[] { "playerGone" }, round.DeletedReasons);
        Assert.Empty(list);
    }

    [Fact]
    public void NamedEntryDroppedWhenPlayerChangedMap()
    {
        var envir = new object();
        var other = new object();
        var hero = P("hero", other);
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(name: "hero", lastTick: 0) };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir, _ => hero, _ => new(), new[] { hero });

        Assert.Equal(new[] { "playerGone" }, round.DeletedReasons);
        Assert.Empty(list);
    }

    [Fact]
    public void EmptyNameMeansAllShake()
    {
        Assert.True(SceneShakeCore.EmptyNameMeansAllShake(""));
        Assert.False(SceneShakeCore.EmptyNameMeansAllShake("hero"));
    }

    [Fact]
    public void AllShakePathTaken()
    {
        var envir = new object();
        var a = P("a", envir);
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(lastTick: 0) };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir, _ => null, _ => new(), new[] { a });

        Assert.True(round.AllShakePath);
        Assert.False(round.NamedPath);
        Assert.Equal(new[] { "a" }, round.Sent.ConvertAll(s => s.Name));
    }

    [Fact]
    public void NamedPathTaken()
    {
        var envir = new object();
        var hero = P("hero", envir);
        var near = P("near", envir);

        var list = new List<SceneShakeCore.ShakeEntry> { Entry(name: "hero", lastTick: 0) };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir,
            n => n == "hero" ? hero : null,
            RangeOf(new Dictionary<string, List<SceneShakeCore.ShakePlayer>>() { ["hero"] = new List<SceneShakeCore.ShakePlayer> { hero, near } }),
            new[] { hero, near });

        Assert.False(round.AllShakePath);
        Assert.True(round.NamedPath);
        Assert.Equal(new[] { "hero", "near" }, round.Sent.ConvertAll(s => s.Name));
    }

    [Fact]
    public void AllShakeSwallowsNamedOnesInSameRound()
    {
        // 本轮同时有"全图项"与"指名项" → 只走全图路径
        Assert.True(SceneShakeCore.AllShakeSwallowsNamedOnesInSameRound());

        var envir = new object();
        var hero = P("hero", envir);
        var far = P("far", envir);

        var list = new List<SceneShakeCore.ShakeEntry>
        {
            Entry(name: "", lastTick: 0),      // 全图
            Entry(name: "hero", lastTick: 0),  // 指名
        };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir,
            n => n == "hero" ? hero : null,
            RangeOf(new Dictionary<string, List<SceneShakeCore.ShakePlayer>>() { ["hero"] = new List<SceneShakeCore.ShakePlayer> { hero } }),
            new[] { hero, far });

        Assert.True(round.AllShakePath);
        Assert.False(round.NamedPath);
        // 全图路径发给**所有**合格玩家（含 far），指名项仍在视野内
        Assert.Contains("far", round.Sent.ConvertAll(s => s.Name));
        // 两个项都已递增（即使指名发送被跳过）
        Assert.Equal(2, round.Advanced);
    }

    [Fact]
    public void NeitherPathWhenTempListEmpty()
    {
        Assert.True(SceneShakeCore.NeitherPathWhenTempListEmpty());

        // 全部项都被删除 → 两条路径都不走
        var envir = new object();
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(name: "gone", lastTick: 0) };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir, _ => null, _ => new(), Array.Empty<SceneShakeCore.ShakePlayer>());

        Assert.False(round.AllShakePath);
        Assert.False(round.NamedPath);
        Assert.Empty(round.Sent);
    }

    [Fact]
    public void NamedPathUsesViewRange()
    {
        Assert.True(SceneShakeCore.NamedPathUsesViewRange());

        var envir = new object();
        var hero = P("hero", envir);
        var outside = P("outside", envir);

        var list = new List<SceneShakeCore.ShakeEntry> { Entry(name: "hero", lastTick: 0) };

        // getRange 只返回 hero（outside 不在视野）
        var round = SceneShakeCore.ProcessRound(
            list, 500, envir,
            n => hero,
            RangeOf(new Dictionary<string, List<SceneShakeCore.ShakePlayer>>() { ["hero"] = new List<SceneShakeCore.ShakePlayer> { hero } }),
            new[] { hero, outside });

        Assert.Equal(new[] { "hero" }, round.Sent.ConvertAll(s => s.Name));
        Assert.DoesNotContain("outside", round.Sent.ConvertAll(s => s.Name));
    }

    [Fact]
    public void TempList2ClearedPerPlayer()
    {
        Assert.True(SceneShakeCore.TempList2ClearedPerPlayer());
    }

    [Fact]
    public void EachRoundSendsCountOne()
    {
        Assert.True(SceneShakeCore.EachRoundSendsCountOne());
        Assert.Equal(1, SceneShakeCore.SendCountPerRound());

        // Count = 3 的项：每次到期各发 1，共发 3 次；第 4 轮才因"已完成"被删除（该轮不发）
        var envir = new object();
        var a = P("a", envir);
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(count: 3, lastTick: 0) };

        uint tick = 1000;
        int rounds = 0;
        int sends = 0;
        while (list.Count > 0 && rounds < 10)
        {
            var r = SceneShakeCore.ProcessRound(list, tick, envir, _ => null, _ => new(), new[] { a });
            rounds++;
            tick += 400;
            sends += r.Sent.Count;
            Assert.True(r.Sent.Count <= 1);   // **每轮最多发 1**
        }

        Assert.Equal(3, sends);    // Count=3 → 共 3 次发送
        Assert.Equal(4, rounds);   // 第 4 轮只做删除、不发
    }

    // ===================== 循环级 EnableClientOption =====================

    [Fact]
    public void EnableClientOptionIsLoopLevelLastWins()
    {
        Assert.True(SceneShakeCore.EnableClientOptionIsLoopLevelLastWins());
        // 参数按处理顺序给出，**最后一个胜出**
        Assert.False(SceneShakeCore.LastProcessedWinsEnableOption(true, false));
        Assert.True(SceneShakeCore.LastProcessedWinsEnableOption(false, true));
        Assert.True(SceneShakeCore.LastProcessedWinsEnableOption(false, false, true));
    }

    [Fact]
    public void SharedEnableOptionOverridesPerEntry()
    {
        // 两个全图项，开关不同 → 倒序处理，下标 0 的胜出
        var envir = new object();
        var a = P("a", envir);

        var list = new List<SceneShakeCore.ShakeEntry>
        {
            Entry(lastTick: 0, enable: true),    // 下标 0
            Entry(lastTick: 0, enable: false),   // 下标 1
        };

        var round = SceneShakeCore.ProcessRound(
            list, 500, envir, _ => null, _ => new(), new[] { a });

        // 倒序：先处理下标 1（false），再处理下标 0（true）→ 共享值为 True
        Assert.True(round.SharedEnableClientOption);
        Assert.All(round.Sent, s => Assert.True(s.Enable));
    }

    // ===================== 过滤集合差异 =====================

    [Fact]
    public void MapPathFilterHasFiveConditions()
    {
        var envir = new object();

        Assert.True(SceneShakeCore.PassesMapPathFilter(P("a", envir), envir));
        Assert.False(SceneShakeCore.PassesMapPathFilter(P("a", new object()), envir));
        Assert.False(SceneShakeCore.PassesMapPathFilter(P("a", envir, ghost: true), envir));
        Assert.False(SceneShakeCore.PassesMapPathFilter(P("a", envir, offline: true), envir));
        Assert.False(SceneShakeCore.PassesMapPathFilter(P("a", envir, dummy: true), envir));
    }

    [Fact]
    public void MapPathLacksDeathCheckUnlikeGlobalSend()
    {
        Assert.True(SceneShakeCore.MapPathLacksDeathCheckUnlikeGlobalSend());
    }

    [Fact]
    public void DeadPlayersStillShakenOnMap()
    {
        // **地图路径无死判** → 死了也震
        Assert.True(SceneShakeCore.DeadPlayersStillShakenOnMap());

        var envir = new object();
        var dead = P("dead", envir, death: true);

        Assert.True(SceneShakeCore.PassesMapPathFilter(dead, envir));
        Assert.False(SceneShakeCore.PassesGlobalFilter(dead));
    }

    [Fact]
    public void GlobalSendFiltersDifferFromMapPath()
    {
        Assert.True(SceneShakeCore.GlobalSendFiltersDifferFromMapPath());
    }

    [Fact]
    public void GlobalFilterHasFiveConditions()
    {
        Assert.True(SceneShakeCore.PassesGlobalFilter(P("a")));
        Assert.False(SceneShakeCore.PassesGlobalFilter(P("a", ghost: true)));
        Assert.False(SceneShakeCore.PassesGlobalFilter(P("a", death: true)));
        Assert.False(SceneShakeCore.PassesGlobalFilter(P("a", offline: true)));
        Assert.False(SceneShakeCore.PassesGlobalFilter(P("a", dummy: true)));
    }

    // ===================== 发送端 =====================

    [Fact]
    public void ClampAtSend()
    {
        Assert.Equal(1, SceneShakeCore.ClampAtSend(0));
        Assert.Equal(1, SceneShakeCore.ClampAtSend(-5));
        Assert.Equal(7, SceneShakeCore.ClampAtSend(7));
    }

    [Fact]
    public void SendSkipsOfflineOrDummy()
    {
        Assert.True(SceneShakeCore.SendSkipsOfflineOrDummy(true, false));
        Assert.True(SceneShakeCore.SendSkipsOfflineOrDummy(false, true));
        Assert.False(SceneShakeCore.SendSkipsOfflineOrDummy(false, false));

        // **不含 ghost/death 判断**
        Assert.False(SceneShakeCore.SendSkipsOfflineOrDummy(false, false));
    }

    [Fact]
    public void EnableClientOptionEncodedAsInteger()
    {
        Assert.Equal(1, SceneShakeCore.EncodeEnableClientOption(true));
        Assert.Equal(0, SceneShakeCore.EncodeEnableClientOption(false));
    }

    [Fact]
    public void ClampAppearsAtThreeLayers()
    {
        Assert.True(SceneShakeCore.ClampAppearsAtThreeLayers());
    }

    // ===================== 客户端 =====================

    [Fact]
    public void ClientShakeRule()
    {
        Assert.True(SceneShakeCore.ClientShakes(1, true));
        Assert.False(SceneShakeCore.ClientShakes(1, false));
        Assert.True(SceneShakeCore.ClientShakes(0, false));
        Assert.True(SceneShakeCore.ClientShakes(0, true));
    }

    [Fact]
    public void ClientOptionOnlyAffectsParamOne()
    {
        Assert.True(SceneShakeCore.ClientOptionOnlyAffectsParamOne());
    }

    // ===================== 多轮稳定性 =====================

    [Fact]
    public void EntryRemovedAfterCountReached()
    {
        var envir = new object();
        var a = P("a", envir);
        var list = new List<SceneShakeCore.ShakeEntry> { Entry(count: 2, lastTick: 0) };

        uint tick = 1000;
        var r1 = SceneShakeCore.ProcessRound(list, tick, envir, _ => null, _ => new(), new[] { a });
        Assert.Equal(1, r1.Advanced);

        tick += 400;
        var r2 = SceneShakeCore.ProcessRound(list, tick, envir, _ => null, _ => new(), new[] { a });
        Assert.Equal(1, r2.Advanced);

        tick += 400;
        var r3 = SceneShakeCore.ProcessRound(list, tick, envir, _ => null, _ => new(), new[] { a });
        Assert.Equal(0, r3.Advanced);
        Assert.Equal(new[] { "completed" }, r3.DeletedReasons);
        Assert.Empty(list);
    }

    [Fact]
    public void MultipleEntriesAdvanceIndependently()
    {
        var envir = new object();
        var a = P("a", envir);

        var list = new List<SceneShakeCore.ShakeEntry>
        {
            Entry(count: 1, lastTick: 0),
            Entry(count: 1, lastTick: 0),
            Entry(count: 1, lastTick: 0),
        };

        var round = SceneShakeCore.ProcessRound(
            list, 1000, envir, _ => null, _ => new(), new[] { a });

        // 三个项**各自**递增（互不影响）
        Assert.Equal(3, round.Advanced);
        // 但发送条数与"项数"无关：三项都是全图项 → 走全图路径 → 只按玩家数发（此处仅 1 人）
        Assert.Single(round.Sent);
    }

    [Fact]
    public void SendCountTracksPlayersNotQueueEntries()
    {
        // 项数与发送数解耦：3 个全图项 + 2 名玩家 → 只发 2 次（而非 3 或 6）
        var envir = new object();
        var a = P("a", envir);
        var b = P("b", envir);

        var list = new List<SceneShakeCore.ShakeEntry>
        {
            Entry(count: 1, lastTick: 0),
            Entry(count: 1, lastTick: 0),
            Entry(count: 1, lastTick: 0),
        };

        var round = SceneShakeCore.ProcessRound(
            list, 1000, envir, _ => null, _ => new(), new[] { a, b });

        Assert.Equal(3, round.Advanced);
        Assert.Equal(2, round.Sent.Count);
    }

    [Fact]
    public void EmptyQueueDoesNothing()
    {
        var round = SceneShakeCore.ProcessRound(
            new List<SceneShakeCore.ShakeEntry>(), 1000, new object(),
            _ => null, _ => new(), Array.Empty<SceneShakeCore.ShakePlayer>());

        Assert.False(round.AllShakePath);
        Assert.False(round.NamedPath);
        Assert.Empty(round.Sent);
        Assert.Equal(0, round.Advanced);
    }
}
