using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J112：副本地图实例池分配与生命周期（NpcActionCmd.pas 23055-23202 +
/// svMain.pas 1941/2135-2137）1:1 测试。
/// </summary>
public sealed class FbInstancePoolCoreTests
{
    private static FbInstancePoolCore.FbInstance Inst(
        string mapName = "$FB_0_1", int count = 0, bool created = false)
        => new()
        {
            MapName = mapName,
            PlayObjectCount = count,
            BoFBCreate = created,
            FbName = "祖玛副本",
        };

    private static List<FbInstancePoolCore.FbInstance> Pool(params FbInstancePoolCore.FbInstance[] items)
        => new(items);

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(60_000, FbInstancePoolCore.MinuteToMs);
        Assert.Equal(0, FbInstancePoolCore.InvalidDuration);
        Assert.Equal("副本不存在", FbInstancePoolCore.FbNotExistsError);
        Assert.Equal("@CreateEctype_NoExists", FbInstancePoolCore.LabelNoExists);
        Assert.Equal("@CreateEctype_Fail_GroupMaster", FbInstancePoolCore.LabelFailGroupMaster);
        Assert.Equal("@CreateEctype_Fail_GuildMaster", FbInstancePoolCore.LabelFailGuildMaster);
        Assert.Equal("@CreateEctype_OK", FbInstancePoolCore.LabelCreateOk);
        Assert.Equal("@CreateEctype_Fail", FbInstancePoolCore.LabelCreateFail);
    }

    [Fact]
    public void LabelsAreAllDistinct()
    {
        var labels = new[]
        {
            FbInstancePoolCore.LabelNoExists,
            FbInstancePoolCore.LabelFailGroupMaster,
            FbInstancePoolCore.LabelFailGuildMaster,
            FbInstancePoolCore.LabelCreateOk,
            FbInstancePoolCore.LabelCreateFail,
        };

        Assert.Equal(labels.Length, new HashSet<string>(labels).Count);
    }

    // ===================== 空闲判定（23154） =====================

    [Fact]
    public void FreeWhenCountZeroAndNotCreated()
    {
        Assert.True(FbInstancePoolCore.IsFreeInstance(Inst(count: 0, created: false)));
    }

    [Fact]
    public void NotFreeWhenCreated()
    {
        Assert.False(FbInstancePoolCore.IsFreeInstance(Inst(count: 0, created: true)));
    }

    [Fact]
    public void NotFreeWhenCountPositive()
    {
        Assert.False(FbInstancePoolCore.IsFreeInstance(Inst(count: 1, created: false)));
    }

    [Fact]
    public void NegativeCountStillCountsAsFree()
    {
        // **23154 用的是 `<= 0` 而非 `= 0`** —— 负数仍算空闲
        Assert.True(FbInstancePoolCore.IsFreeInstance(Inst(count: -1, created: false)));
        Assert.True(FbInstancePoolCore.IsFreeInstance(Inst(count: -99, created: false)));
    }

    [Fact]
    public void NegativeCountWithCreatedIsNotFree()
    {
        // 两个条件是**与**关系
        Assert.False(FbInstancePoolCore.IsFreeInstance(Inst(count: -1, created: true)));
    }

    // ===================== 参数校验（23062） =====================

    [Fact]
    public void EmptyNameIsBadParam()
    {
        Assert.Equal(FbInstancePoolCore.ParamCheck.BadParam, FbInstancePoolCore.CheckParams("", 10));
    }

    [Fact]
    public void ZeroDurationIsBadParam()
    {
        Assert.Equal(FbInstancePoolCore.ParamCheck.BadParam, FbInstancePoolCore.CheckParams("祖玛", 0));
    }

    [Fact]
    public void NegativeDurationIsAccepted()
    {
        // 23062 只判 `dwTime = 0`，**负数不拦** —— 会得到已过期的副本时间
        Assert.Equal(FbInstancePoolCore.ParamCheck.Ok, FbInstancePoolCore.CheckParams("祖玛", -5));
    }

    [Fact]
    public void ValidParamsPass()
    {
        Assert.Equal(FbInstancePoolCore.ParamCheck.Ok, FbInstancePoolCore.CheckParams("祖玛", 10));
    }

    // ===================== 池查找（23069-23075） =====================

    [Fact]
    public void LookupFindsPool()
    {
        var manager = new List<(string, List<FbInstancePoolCore.FbInstance>)>
        {
            ("祖玛副本", Pool(Inst())),
        };

        var r = FbInstancePoolCore.LookupPool(manager, "祖玛副本");

        Assert.Equal(0, r.Index);
        Assert.NotNull(r.FbList);
        Assert.False(r.NotFound);
    }

    [Fact]
    public void LookupIsCaseInsensitive()
    {
        // TStringList.IndexOf 语义
        var manager = new List<(string, List<FbInstancePoolCore.FbInstance>)>
        {
            ("FBName", Pool(Inst())),
        };

        Assert.Equal(0, FbInstancePoolCore.LookupPool(manager, "fbname").Index);
    }

    [Fact]
    public void LookupMissGivesMinusOne()
    {
        var r = FbInstancePoolCore.LookupPool(new List<(string, List<FbInstancePoolCore.FbInstance>)>(), "无");

        Assert.Equal(-1, r.Index);
        Assert.Null(r.FbList);
        Assert.True(r.NotFound);
    }

    [Fact]
    public void LookupEmptyPoolIsNotFound()
    {
        // 23073：池存在但 Count = 0 → Envir 仍为 nil → 副本不存在
        var manager = new List<(string, List<FbInstancePoolCore.FbInstance>)>
        {
            ("祖玛副本", Pool()),
        };

        var r = FbInstancePoolCore.LookupPool(manager, "祖玛副本");

        Assert.Equal(0, r.Index);      // 找到了池
        Assert.True(r.NotFound);       // 但池为空 → 副本不存在
    }

    [Fact]
    public void PeekFirstReturnsIndexZero()
    {
        var p = Pool(Inst("A"), Inst("B"));
        Assert.Equal("A", FbInstancePoolCore.PeekFirstForEnterLimit(p)!.MapName);
    }

    [Fact]
    public void PeekFirstOnEmptyPoolIsNull()
    {
        Assert.Null(FbInstancePoolCore.PeekFirstForEnterLimit(Pool()));
    }

    [Fact]
    public void PeekFirstIsNotTheAllocationTarget()
    {
        // 23074 只用于准入判断；真正分配在 23151 的循环里重找。
        // 第 0 个满员时，PeekFirst 返回它，但分配会选到第 1 个
        var busy = Inst("BUSY", count: 5, created: true);
        var free = Inst("FREE");
        var p = Pool(busy, free);

        Assert.Equal("BUSY", FbInstancePoolCore.PeekFirstForEnterLimit(p)!.MapName);

        var allocated = FbInstancePoolCore.AllocateInstance(p, new object(), 10, 1000);
        Assert.Equal("FREE", allocated!.MapName);
    }

    // ===================== 实例分配（23151-23165） =====================

    [Fact]
    public void AllocatePicksFirstFree()
    {
        var p = Pool(Inst("A", count: 1), Inst("B"), Inst("C"));
        var got = FbInstancePoolCore.AllocateInstance(p, new object(), 10, 1000);

        Assert.Equal("B", got!.MapName);
    }

    [Fact]
    public void AllocateReturnsNullWhenNoneFree()
    {
        var p = Pool(Inst("A", created: true), Inst("B", count: 3));
        Assert.Null(FbInstancePoolCore.AllocateInstance(p, new object(), 10, 1000));
    }

    [Fact]
    public void AllocateOnEmptyPoolIsNull()
    {
        Assert.Null(FbInstancePoolCore.AllocateInstance(Pool(), new object(), 10, 1000));
    }

    [Fact]
    public void AllocateSetsCreateFlag()
    {
        var e = Inst();
        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 10, 1000);

        Assert.True(e.BoFBCreate);
    }

    [Fact]
    public void AllocateResetsEnterFlag()
    {
        // 23158
        var e = Inst();
        e.BoFBPlayObjectEnter = true;

        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 10, 1000);
        Assert.False(e.BoFBPlayObjectEnter);
    }

    [Fact]
    public void AllocateSetsFbTimeFromDuration()
    {
        // 23159：MyGetTickCount + dwTime * 60 * 1000
        var e = Inst();
        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), dwTime: 10, nowTick: 1000);

        Assert.Equal(1000 + 600_000, e.FbTime);
    }

    [Fact]
    public void AllocateSetsCreateTimeToNow()
    {
        // 23161：基准点是 now，**不等于** FbTime
        var e = Inst();
        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), dwTime: 10, nowTick: 1000);

        Assert.Equal(1000, e.FbCreateTime);
        Assert.NotEqual(e.FbTime, e.FbCreateTime);
    }

    [Fact]
    public void OneMinuteDurationAddsSixtyThousand()
    {
        var e = Inst();
        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 1, 0);
        Assert.Equal(60_000, e.FbTime);
    }

    [Fact]
    public void AllocateClearsFailState()
    {
        // 23162-23163
        var e = Inst();
        e.FbFailTime = 12345;
        e.BoFBFail = true;

        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 10, 1000);

        Assert.Equal(0, e.FbFailTime);
        Assert.False(e.BoFBFail);
    }

    [Fact]
    public void AllocateRecordsOwner()
    {
        // 23160
        var e = Inst();
        var owner = new object();

        FbInstancePoolCore.AllocateInstance(Pool(e), owner, 10, 1000);
        Assert.Same(owner, e.FbMasterObject);
    }

    [Fact]
    public void AllocateIncrementsFbIndex()
    {
        // 23156
        var e = Inst();
        e.BtFBIndex = 5;

        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 10, 1000);
        Assert.Equal(6, e.BtFBIndex);
    }

    [Fact]
    public void FbIndexStartsAtZeroThenBecomesOne()
    {
        var e = Inst();
        Assert.Equal(0, e.BtFBIndex);

        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 10, 1000);
        Assert.Equal(1, e.BtFBIndex);
    }

    [Fact]
    public void FbIndexWrapsAt256()
    {
        // **byte 语义**：只增不减，255 + 1 回绕到 0
        var e = Inst();
        e.BtFBIndex = 255;

        FbInstancePoolCore.AllocateInstance(Pool(e), new object(), 10, 1000);
        Assert.Equal(0, e.BtFBIndex);
    }

    [Fact]
    public void FbIndexIsByteTyped()
    {
        Assert.Equal(typeof(byte), typeof(FbInstancePoolCore.FbInstance).GetField("BtFBIndex")!.FieldType);
    }

    [Fact]
    public void RepeatedAllocationOnlyTouchesOneInstance()
    {
        var a = Inst("A");
        var b = Inst("B");
        var p = Pool(a, b);

        FbInstancePoolCore.AllocateInstance(p, new object(), 10, 1000);

        Assert.True(a.BoFBCreate);
        Assert.False(b.BoFBCreate);   // 未被动过
        Assert.Equal(0, b.BtFBIndex);
    }

    [Fact]
    public void PreviouslyAllocatedIsSkippedNextTime()
    {
        var a = Inst("A");
        var b = Inst("B");
        var p = Pool(a, b);

        var first = FbInstancePoolCore.AllocateInstance(p, new object(), 10, 1000);
        var second = FbInstancePoolCore.AllocateInstance(p, new object(), 10, 2000);

        Assert.Equal("A", first!.MapName);
        Assert.Equal("B", second!.MapName);
    }

    [Fact]
    public void ReleasedInstanceCanBeReallocated()
    {
        // 释放（人数归 0 且 boFBCreate 归 False）后可再次分配
        var a = Inst("A");
        var p = Pool(a);

        FbInstancePoolCore.AllocateInstance(p, new object(), 10, 1000);
        Assert.Null(FbInstancePoolCore.AllocateInstance(p, new object(), 10, 2000));

        a.BoFBCreate = false;
        a.PlayObjectCount = 0;

        var again = FbInstancePoolCore.AllocateInstance(p, new object(), 10, 3000);
        Assert.Equal("A", again!.MapName);
        Assert.Equal(2, a.BtFBIndex);   // 序号累加，**不重置**
    }

    [Fact]
    public void DurationToMsMatchesAllocation()
    {
        Assert.Equal(600_000, FbInstancePoolCore.DurationToMs(10));
        Assert.Equal(60_000, FbInstancePoolCore.DurationToMs(1));
    }

    [Fact]
    public void DurationIsMinutesNotSeconds()
    {
        // 23159 的 `dwTime * 60 * 1000` —— 单位是**分**，不是秒
        Assert.NotEqual(FbInstancePoolCore.SecondsToMs(10), FbInstancePoolCore.DurationToMs(10));
        Assert.Equal(60, FbInstancePoolCore.MinuteToMs / 1_000);
    }

    // ===================== 准入判断 =====================

    [Fact]
    public void AlreadyInSameFbAllFourConditions()
    {
        var e = Inst();
        Assert.True(FbInstancePoolCore.IsAlreadyInSameFb(e, true, 500, 500, "祖玛", "祖玛"));
    }

    [Fact]
    public void AlreadyInSameFbFailsWhenNoEnvir()
    {
        Assert.False(FbInstancePoolCore.IsAlreadyInSameFb(null, true, 500, 500, "祖玛", "祖玛"));
    }

    [Fact]
    public void AlreadyInSameFbFailsWhenNotCreated()
    {
        var e = Inst();
        Assert.False(FbInstancePoolCore.IsAlreadyInSameFb(e, false, 500, 500, "祖玛", "祖玛"));
    }

    [Fact]
    public void AlreadyInSameFbFailsWhenCreateTimeDiffers()
    {
        // **时间戳不同 = 不是同一次创建**（实例被回收后重建）
        var e = Inst();
        Assert.False(FbInstancePoolCore.IsAlreadyInSameFb(e, true, 500, 600, "祖玛", "祖玛"));
    }

    [Fact]
    public void AlreadyInSameFbFailsWhenNameDiffers()
    {
        var e = Inst();
        Assert.False(FbInstancePoolCore.IsAlreadyInSameFb(e, true, 500, 500, "祖玛", "赤月"));
    }

    [Fact]
    public void AlreadyInSameFbNameIsCaseInsensitive()
    {
        // SameText
        var e = Inst();
        Assert.True(FbInstancePoolCore.IsAlreadyInSameFb(e, true, 500, 500, "FB", "fb"));
    }

    [Fact]
    public void GroupLikeLimitCoversZeroAndOne()
    {
        // 23090/23131：`in [fbel_JOB3, fbel_Group]` = 0 或 1
        Assert.True(FbInstancePoolCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.Job3));
        Assert.True(FbInstancePoolCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.Group));
        Assert.False(FbInstancePoolCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.OnlyCreater));
        Assert.False(FbInstancePoolCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.Guild));
    }

    [Fact]
    public void GroupLikeCoversExactlyTwoValues()
    {
        int n = 0;
        foreach (FbMapDeclareCore.FbEnterLimit v in Enum.GetValues<FbMapDeclareCore.FbEnterLimit>())
            if (FbInstancePoolCore.IsGroupLikeLimit(v)) n++;

        Assert.Equal(2, n);
    }

    [Fact]
    public void GroupMasterCheckIsReferenceEquality()
    {
        // 23134：`m_GroupOwner <> PlayObject`
        var player = new object();
        var other = new object();

        Assert.True(FbInstancePoolCore.IsGroupMasterSelf(player, player));
        Assert.False(FbInstancePoolCore.IsGroupMasterSelf(other, player));
        Assert.False(FbInstancePoolCore.IsGroupMasterSelf(null, player));
    }

    [Fact]
    public void GuildMasterCheck()
    {
        Assert.True(FbInstancePoolCore.IsGuildMaster(true));
        Assert.False(FbInstancePoolCore.IsGuildMaster(false));
    }

    [Fact]
    public void LeaderInSameFbThreeConditions()
    {
        var e = Inst();
        Assert.True(FbInstancePoolCore.IsLeaderInSameFb(e, true, "祖玛", "祖玛"));
        Assert.False(FbInstancePoolCore.IsLeaderInSameFb(null, true, "祖玛", "祖玛"));
        Assert.False(FbInstancePoolCore.IsLeaderInSameFb(e, false, "祖玛", "祖玛"));
        Assert.False(FbInstancePoolCore.IsLeaderInSameFb(e, true, "祖玛", "赤月"));
    }

    [Fact]
    public void LeaderCheckIsCaseInsensitive()
    {
        var e = Inst();
        Assert.True(FbInstancePoolCore.IsLeaderInSameFb(e, true, "FB", "fb"));
    }

    // ===================== 释放（svMain.pas 2135-2137） =====================

    [Fact]
    public void ShutdownOrderFreesListsBeforeManager()
    {
        Assert.Equal("Objects[i].Free(FBList)", FbInstancePoolCore.ShutdownOrder[0]);
        Assert.Equal("g_FBMapManager.Free", FbInstancePoolCore.ShutdownOrder[1]);
    }

    [Fact]
    public void DisposeAllFbListsVisitsEach()
    {
        var lists = new List<List<string>> { new() { "a" }, new() { "b" }, new() { "c" } };
        int n = 0;

        FbInstancePoolCore.DisposeAllFbLists(lists, _ => n++);

        Assert.Equal(3, n);
    }

    [Fact]
    public void DisposeAllFbListsEmptyIsNoOp()
    {
        int n = 0;
        FbInstancePoolCore.DisposeAllFbLists(new List<List<string>>(), _ => n++);
        Assert.Equal(0, n);
    }

    [Fact]
    public void EnvirIsNotFreedWithFbList()
    {
        // **关键所有权**：FBList 释放时不得连带释放 Envir（归 g_MapManager）
        Assert.False(FbInstancePoolCore.ShouldFreeEnvirWithFbList());
    }

    [Fact]
    public void MonsterListIsClearedNotFreed()
    {
        // 23187：m_FBMonsterList.Clear
        Assert.True(FbInstancePoolCore.ClearsMonsterListNotFrees());
    }

    [Fact]
    public void DisposeDoesNotFreeInstanceObjects()
    {
        // 用真实的 FbInstance 列表验证：释放"列表"后实例对象仍然可用
        var e = Inst("A");
        var list = Pool(e);

        FbInstancePoolCore.DisposeAllFbLists(new List<List<FbInstancePoolCore.FbInstance>> { list }, _ => { });

        Assert.Equal("A", e.MapName);   // 实例未被释放
    }

    // ===================== 端到端 =====================

    [Fact]
    public void EndToEndAllocateAndReportBusy()
    {
        // 池三个实例 → 依次分配 → 第三次满
        var manager = new List<(string, List<FbInstancePoolCore.FbInstance>)>
        {
            ("祖玛副本", Pool(Inst("1"), Inst("2"), Inst("3"))),
        };

        var lookup = FbInstancePoolCore.LookupPool(manager, "祖玛副本");
        Assert.False(lookup.NotFound);

        var owner = new object();
        var a = FbInstancePoolCore.AllocateInstance(lookup.FbList!, owner, 10, 1000);
        var b = FbInstancePoolCore.AllocateInstance(lookup.FbList!, owner, 10, 2000);
        var c = FbInstancePoolCore.AllocateInstance(lookup.FbList!, owner, 10, 3000);
        var d = FbInstancePoolCore.AllocateInstance(lookup.FbList!, owner, 10, 4000);

        Assert.NotNull(a);
        Assert.NotNull(b);
        Assert.NotNull(c);
        Assert.Null(d);   // 池已满
    }
}
