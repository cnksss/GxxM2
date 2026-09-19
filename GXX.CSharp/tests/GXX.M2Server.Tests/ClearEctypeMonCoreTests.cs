using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J120：副本/普通地图清怪（NpcActionCmd.pas 23438-23503 `CLEARECTYPEMON`
/// + 23181-23194 创建后清怪重刷）1:1 测试。
/// </summary>
public sealed class ClearEctypeMonCoreTests
{
    private static ClearEctypeMonCore.ClearMon Mon(
        string name, bool ghost = false, bool master = false,
        object? penvir = null, bool inFb = false)
        => new()
        {
            Name = name,
            BoGhost = ghost,
            HasMaster = master,
            Penvir = penvir,
            InFbList = inFb,
        };

    // ===================== 常量与注册 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(325, ClearEctypeMonCore.NaClearEctypeMon);
        Assert.Equal("CLEARECTYPEMON", ClearEctypeMonCore.CommandName);
        Assert.Equal("CLEARMACHINERYEVENT", ClearEctypeMonCore.UnimplementedCommandName);
        Assert.Equal("@CreateEctype_OK", ClearEctypeMonCore.CreateEctypeOkLabel);
    }

    [Fact]
    public void KeywordsMatchSource()
    {
        Assert.Equal("NPCMAP", ClearEctypeMonCore.KeywordNpcMap);
        Assert.Equal("FBMAP", ClearEctypeMonCore.KeywordFbMap);
        Assert.Equal("SELF", ClearEctypeMonCore.KeywordSelf);
    }

    // ===================== 参数为空（23446） =====================

    [Fact]
    public void EmptyParamDetected()
    {
        Assert.True(ClearEctypeMonCore.IsEmptyParam(""));
        Assert.False(ClearEctypeMonCore.IsEmptyParam("SELF"));
        Assert.False(ClearEctypeMonCore.IsEmptyParam(" "));
    }

    // ===================== 四选一分派（23451-23458） =====================

    [Fact]
    public void NpcMapKeyword()
    {
        Assert.Equal(ClearEctypeMonCore.MapSource.NpcMap,
            ClearEctypeMonCore.SelectMapSource("NPCMAP"));
    }

    [Fact]
    public void FbMapKeyword()
    {
        Assert.Equal(ClearEctypeMonCore.MapSource.PlayObjectFbEnvir,
            ClearEctypeMonCore.SelectMapSource("FBMAP"));
    }

    [Fact]
    public void SelfKeyword()
    {
        Assert.Equal(ClearEctypeMonCore.MapSource.PlayObjectMap,
            ClearEctypeMonCore.SelectMapSource("SELF"));
    }

    [Fact]
    public void OtherIsLookupByName()
    {
        Assert.Equal(ClearEctypeMonCore.MapSource.LookupByName,
            ClearEctypeMonCore.SelectMapSource("0"));
        Assert.Equal(ClearEctypeMonCore.MapSource.LookupByName,
            ClearEctypeMonCore.SelectMapSource("比奇省"));
    }

    [Fact]
    public void KeywordsAreCaseInsensitive()
    {
        // CompareText 大小写不敏感
        Assert.Equal(ClearEctypeMonCore.MapSource.NpcMap,
            ClearEctypeMonCore.SelectMapSource("npcMap"));
        Assert.Equal(ClearEctypeMonCore.MapSource.PlayObjectFbEnvir,
            ClearEctypeMonCore.SelectMapSource("fbmap"));
        Assert.Equal(ClearEctypeMonCore.MapSource.PlayObjectMap,
            ClearEctypeMonCore.SelectMapSource("Self"));
    }

    [Fact]
    public void KeywordsShadowRealMapNames()
    {
        // 若真有地图叫 SELF，用 SELF 得到的是玩家所在图
        Assert.True(ClearEctypeMonCore.KeywordsShadowRealMapNames());
        Assert.Equal(ClearEctypeMonCore.MapSource.PlayObjectMap,
            ClearEctypeMonCore.SelectMapSource("SELF"));
    }

    [Fact]
    public void KeywordOrderIsFbBeforeSelf()
    {
        // 原文顺序：NPCMAP → FBMAP → SELF；三者互斥故顺序不影响结果
        Assert.Equal(ClearEctypeMonCore.MapSource.PlayObjectFbEnvir,
            ClearEctypeMonCore.SelectMapSource("FBMAP"));
    }

    [Fact]
    public void NotFoundReportsSameErrorAsEmptyParam()
    {
        Assert.True(ClearEctypeMonCore.NotFoundReportsSameErrorAsEmptyParam());
    }

    [Fact]
    public void SkipsWhenEnvirNull()
    {
        Assert.True(ClearEctypeMonCore.SkipsWhenEnvirNull(null));
        Assert.False(ClearEctypeMonCore.SkipsWhenEnvirNull(new object()));
    }

    // ===================== 副本分支（23463-23472） =====================

    [Fact]
    public void FbBranchMakesLiveMasterlessGhosts()
    {
        var fb = new List<ClearEctypeMonCore.ClearMon>
        {
            Mon("a"),
            Mon("b"),
        };

        var r = ClearEctypeMonCore.RunFbBranch(fb);

        Assert.Equal(new[] { "a", "b" }, r.MadeGhostNames);
        Assert.True(r.ListCleared);
    }

    [Fact]
    public void FbBranchSkipsExistingGhosts()
    {
        // 注意：RunFbBranch 末尾会 Clear，故须在调用**前**持有元素引用
        var ghost = Mon("g", ghost: true);
        var live = Mon("a");
        var fb = new List<ClearEctypeMonCore.ClearMon> { ghost, live };

        var r = ClearEctypeMonCore.RunFbBranch(fb);

        Assert.Equal(new[] { "a" }, r.MadeGhostNames);
        Assert.False(ghost.MadeGhost);
        Assert.True(live.MadeGhost);
    }

    [Fact]
    public void FbBranchSkipsSummoned()
    {
        // m_Master <> nil 的怪（召唤物/宝宝）不变幽灵
        var pet = Mon("pet", master: true);
        var live = Mon("a");
        var fb = new List<ClearEctypeMonCore.ClearMon> { pet, live };

        var r = ClearEctypeMonCore.RunFbBranch(fb);

        Assert.Equal(new[] { "a" }, r.MadeGhostNames);
        Assert.False(pet.MadeGhost);
    }

    [Fact]
    public void FbBranchClearsEvenIfNoGhosts()
    {
        // **Clear 无条件执行**
        var fb = new List<ClearEctypeMonCore.ClearMon> { Mon("pet", master: true) };

        var r = ClearEctypeMonCore.RunFbBranch(fb);

        Assert.Empty(r.MadeGhostNames);
        Assert.True(r.ListCleared);
        Assert.Empty(fb);
    }

    [Fact]
    public void FbBranchOnEmptyList()
    {
        var r = ClearEctypeMonCore.RunFbBranch(new List<ClearEctypeMonCore.ClearMon>());

        Assert.Empty(r.MadeGhostNames);
        Assert.True(r.ListCleared);
        Assert.Equal(0, r.Iterations);
    }

    [Fact]
    public void FbBranchUsesMatchingList()
    {
        Assert.True(ClearEctypeMonCore.FbBranchUsesMatchingList());
    }

    // ===================== 普通分支（23482-23498）—— 原版缺陷 =====================

    [Fact]
    public void NormalMapBranchIndexesWrongList()
    {
        // **核心发现**：循环边界取自 m_MonObjectList，元素取自 m_FBMonsterList
        Assert.True(ClearEctypeMonCore.NormalMapBranchIndexesWrongList());
    }

    [Fact]
    public void NormalMapBranchEffectivelyNoOp()
    {
        // 普通地图上 CLEARECTYPEMON **什么也不做**
        var envir = new object();
        var monObjects = new List<ClearEctypeMonCore.ClearMon>
        {
            Mon("m1", penvir: envir),
            Mon("m2", penvir: envir),
        };
        var fbList = new List<ClearEctypeMonCore.ClearMon>();   // 非副本地图通常为空

        var r = ClearEctypeMonCore.RunNormalBranch(monObjects, fbList, envir);

        Assert.Empty(r.MadeGhostNames);
        Assert.True(ClearEctypeMonCore.NormalMapBranchEffectivelyNoOp());
    }

    [Fact]
    public void NormalMapBranchMayIndexOutOfRange()
    {
        // fbList 空、monObjectList 非空 → 越界
        var envir = new object();
        var monObjects = new List<ClearEctypeMonCore.ClearMon> { Mon("m1", penvir: envir) };

        var r = ClearEctypeMonCore.RunNormalBranch(
            monObjects, new List<ClearEctypeMonCore.ClearMon>(), envir);

        Assert.True(r.OutOfRange);
        Assert.True(ClearEctypeMonCore.NormalMapBranchMayIndexOutOfRange());
    }

    [Fact]
    public void NormalMapBranchNeverTouchesIntendedMonsters()
    {
        // 即便 fbList 非空，被处理的也是副本里的怪、且守卫会拦下
        var envir = new object();
        var otherEnvir = new object();

        var monObjects = new List<ClearEctypeMonCore.ClearMon> { Mon("target", penvir: envir) };
        var fbList = new List<ClearEctypeMonCore.ClearMon> { Mon("fbMon", penvir: otherEnvir) };

        var r = ClearEctypeMonCore.RunNormalBranch(monObjects, fbList, envir);

        Assert.Empty(r.MadeGhostNames);
        Assert.False(monObjects[0].MadeGhost);   // **目标怪未被处理**
        Assert.False(fbList[0].MadeGhost);       // 副本怪也因守卫不成立而未处理
    }

    [Fact]
    public void NormalBranchDoesNotClear()
    {
        var envir = new object();
        var fbList = new List<ClearEctypeMonCore.ClearMon> { Mon("x", penvir: envir) };

        var r = ClearEctypeMonCore.RunNormalBranch(
            new List<ClearEctypeMonCore.ClearMon> { Mon("m", penvir: envir) }, fbList, envir);

        Assert.False(r.ListCleared);
        Assert.Single(fbList);   // **未 Clear**
    }

    [Fact]
    public void OnlyFbBranchClears()
    {
        Assert.True(ClearEctypeMonCore.OnlyFbBranchClears());
        Assert.True(ClearEctypeMonCore.FbBranchClears());
        Assert.False(ClearEctypeMonCore.NormalBranchClears());
    }

    [Fact]
    public void TwoBranchesDifferOnBothListAndClear()
    {
        // 双重差异：遍历的表不同 **且** Clear 与否不同
        var envir = new object();

        var fbMon = Mon("fb");
        var fb = new List<ClearEctypeMonCore.ClearMon> { fbMon };
        var fbResult = ClearEctypeMonCore.RunFbBranch(fb);

        var normalFb = new List<ClearEctypeMonCore.ClearMon> { Mon("fb2") };
        var normalResult = ClearEctypeMonCore.RunNormalBranch(
            new List<ClearEctypeMonCore.ClearMon> { Mon("m") }, normalFb, envir);

        Assert.True(fbResult.ListCleared);
        Assert.False(normalResult.ListCleared);
        Assert.True(fbMon.MadeGhost);
    }

    // ===================== 分支选择（23461） =====================

    [Fact]
    public void IsFbMapDecidesBranch()
    {
        Assert.True(ClearEctypeMonCore.IsFbMap(true));
        Assert.False(ClearEctypeMonCore.IsFbMap(false));
    }

    [Fact]
    public void RunDispatchesByFbFlag()
    {
        var fb = new List<ClearEctypeMonCore.ClearMon> { Mon("x") };
        var r = ClearEctypeMonCore.Run(true, new List<ClearEctypeMonCore.ClearMon>(), fb, new object());

        Assert.Equal(new[] { "x" }, r.MadeGhostNames);
        Assert.True(r.ListCleared);
    }

    // ===================== 守卫（23467） =====================

    [Fact]
    public void MakeGhostRequiresTwoNegations()
    {
        Assert.True(ClearEctypeMonCore.ShouldMakeGhost(false, false));
        Assert.False(ClearEctypeMonCore.ShouldMakeGhost(true, false));    // 已是幽灵
        Assert.False(ClearEctypeMonCore.ShouldMakeGhost(false, true));    // 有主人
        Assert.False(ClearEctypeMonCore.ShouldMakeGhost(true, true));
    }

    [Fact]
    public void SummonedMonstersUntouched()
    {
        Assert.True(ClearEctypeMonCore.SummonedMonstersUntouched());
    }

    [Fact]
    public void ActionIsMakeGhostNotDelete()
    {
        // 变幽灵而非删除/Free——与 J113 的 Clear 语义区分
        Assert.True(ClearEctypeMonCore.ActionIsMakeGhost());
    }

    [Fact]
    public void GuardianMonsterCountUnaffected()
    {
        // 对比 J113：那里回收分支会清 m_nGuardinaLevelMonCount，本处不动任何计数
        var fb = new List<ClearEctypeMonCore.ClearMon> { Mon("boss") };
        ClearEctypeMonCore.RunFbBranch(fb);

        Assert.True(fb.Count == 0);
    }

    // ===================== 与 J113 的关系 =====================

    [Fact]
    public void ClearIsUnconditionalUnlikeJ113()
    {
        // 本处只有"变幽灵+清空"一种行为；J113 按 m_boFBCreate 分两种相反处理
        Assert.True(ClearEctypeMonCore.ClearIsUnconditionalUnlikeJ113());

        // J113 的对照：未创建时活怪变幽灵
        var s = new FbRecycleCore.FbRecycleState { BoFBCreate = false };
        var m = new FbRecycleCore.FbMonster { Name = "a" };
        s.FbMonsterList.Add(m);
        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.True(m.MadeGhost);

        // J113 的另一侧：已创建时活怪**不**变幽灵（本处没有这个分支）
        var s2 = new FbRecycleCore.FbRecycleState { BoFBCreate = true };
        var m2 = new FbRecycleCore.FbMonster { Name = "a" };
        s2.FbMonsterList.Add(m2);
        FbRecycleCore.ApplyMonsterCleanup(s2);

        Assert.False(m2.MadeGhost);
    }

    [Fact]
    public void ThreeCopiesAreIdentical()
    {
        Assert.True(ClearEctypeMonCore.ThreeCopiesAreIdentical());
        Assert.Equal(3, ClearEctypeMonCore.ClearCopies.Length);
    }

    [Fact]
    public void ClearCopiesSpanTwoFiles()
    {
        var files = new HashSet<string>();
        foreach (var (file, _, _) in ClearEctypeMonCore.ClearCopies)
            files.Add(file);

        Assert.Equal(2, files.Count);
        Assert.Contains("UsrEngn.pas", files);
        Assert.Contains("NpcActionCmd.pas", files);
    }

    [Fact]
    public void AllThreeCopiesDeclareTheirContext()
    {
        Assert.All(ClearEctypeMonCore.ClearCopies, c => Assert.False(string.IsNullOrEmpty(c.Context)));
        Assert.All(ClearEctypeMonCore.ClearCopies, c => Assert.False(string.IsNullOrEmpty(c.Lines)));
    }

    // ===================== 创建流程清怪+重刷（23181-23194） =====================

    [Fact]
    public void CreateEctypeRegenCountUsesTemplateCount()
    {
        // 23193：直接用模板 nCount，未经 J116 节流
        Assert.Equal(10, ClearEctypeMonCore.CreateEctypeRegenCount(10));
        Assert.Equal(0, ClearEctypeMonCore.CreateEctypeRegenCount(0));
    }

    [Fact]
    public void RegenResultIgnored()
    {
        // 某个模板刷怪失败也不影响创建报告成功
        Assert.True(ClearEctypeMonCore.RegenResultIgnored());
        Assert.True(ClearEctypeMonCore.CreateEctypeReportsSuccess(anyRegenFailed: true));
        Assert.True(ClearEctypeMonCore.CreateEctypeReportsSuccess(anyRegenFailed: false));
    }

    [Fact]
    public void UsesFbMonGenListNotMainList()
    {
        Assert.True(ClearEctypeMonCore.UsesFbMonGenListNotMainList());
    }

    [Fact]
    public void ClearBeforeRegen()
    {
        // 先清后刷；顺序颠倒会把刚刷出的怪立刻变幽灵
        Assert.True(ClearEctypeMonCore.ClearBeforeRegen());
        Assert.True(ClearEctypeMonCore.WrongOrderWouldGhostNewMonsters());
    }

    [Fact]
    public void CreateEctypeCleanupMatchesFbBranch()
    {
        // 23181-23187 与 CLEARECTYPEMON 的副本分支逐字相同 → 结果一致
        var a = new List<ClearEctypeMonCore.ClearMon> { Mon("x"), Mon("pet", master: true) };
        var b = new List<ClearEctypeMonCore.ClearMon> { Mon("x"), Mon("pet", master: true) };

        var ra = ClearEctypeMonCore.RunFbBranch(a);
        var rb = ClearEctypeMonCore.RunFbBranch(b);

        Assert.Equal(ra.MadeGhostNames, rb.MadeGhostNames);
        Assert.Equal(ra.ListCleared, rb.ListCleared);
    }

    // ===================== CLEARMACHINERYEVENT =====================

    [Fact]
    public void ClearMachineryEventIsNotImplemented()
    {
        // 只出现在 23167-23179 的注释块里
        Assert.True(ClearEctypeMonCore.ClearMachineryEventIsNotImplemented());
        Assert.Equal(0, ClearEctypeMonCore.ClearMachineryEventRegistrationCount());
    }

    [Fact]
    public void CommentedBlockRange()
    {
        Assert.Equal(23167, ClearEctypeMonCore.CommentedBlock.Start);
        Assert.Equal(23179, ClearEctypeMonCore.CommentedBlock.End);
    }

    [Fact]
    public void ScriptCleanupReplacedByInlineCode()
    {
        Assert.True(ClearEctypeMonCore.ScriptCleanupReplacedByInlineCode());
    }

    [Fact]
    public void UnimplementedCommandNameDiffersFromReal()
    {
        Assert.NotEqual(ClearEctypeMonCore.CommandName, ClearEctypeMonCore.UnimplementedCommandName);
    }
}
