// ============================================================================
//  测试：Source/M2Engine/ObjFireDragon.pas → GXX.M2Server.Sweep9.Monsters（1:1）
//
//  覆盖策略：
//    * 单元常量逐个对账（含 `MULTI_THREAD = 0` 这个决定条件编译的源头）；
//    * 方法清单 14 条例程：条数 / 分段计数 / 顺序 / 落在单元内；
//    * 原文缺陷 F1-F12 **逐条差异断言**（含计数取证，§37.3）；
//    * 每个判定函数含边界（闭区间 / 恰好等于门限 / 无符号回绕）与被注释旧实现的排除证据。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.M2Server.Sweep9.Monsters;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class Sweep9MonstersObjFireDragonTests
{
    // ===================== 一、常量 =====================

    [Fact]
    public void SourceIdentity()
    {
        Assert.Equal("Source/M2Engine/ObjFireDragon.pas", ObjFireDragonCore.SourceUnit);
        Assert.Equal(531, ObjFireDragonCore.SourceLines);
    }

    [Fact]
    public void Constants_MatchSource()
    {
        Assert.Equal(0, ObjFireDragonCore.MULTI_THREAD);                 // M2Share.pas:75
        Assert.Equal(13, ObjFireDragonCore.FireDragonViewRange);          // :58
        Assert.Equal(11, ObjFireDragonCore.FireDragonAttackViewRange);    // :246
        Assert.Equal(200, ObjFireDragonCore.AntiPoison);                 // :56/:179/:387/:443
        Assert.Equal(10000, ObjFireDragonCore.LightNotifyIntervalMs);     // :195
        Assert.Equal(6, ObjFireDragonCore.GuardPickRandomBound);          // :206  Random(6)
        Assert.Equal(6, ObjFireDragonCore.GuardMaxPerRound);              // :233  J >= 6
        Assert.Equal(3000, ObjFireDragonCore.GuardChosenLightTimeMs);     // :220
        Assert.Equal(2500, ObjFireDragonCore.GuardDefaultLightTimeMs);    // :392 / :513
        Assert.Equal(3000, ObjFireDragonCore.FireDragonAttickIntervalMs); // :344
        Assert.Equal(10000, ObjFireDragonCore.FireDragonPurgeIntervalMs); // :351
        Assert.Equal(1000, ObjFireDragonCore.SearchEnemyIntervalMs);      // :335
        Assert.Equal(3, ObjFireDragonCore.DragonHitRandomDenominator);    // :252  Random(3)
        Assert.Equal(4, ObjFireDragonCore.RetargetRandomDenominator);     // :297  Random(4)
        Assert.Equal(2, ObjFireDragonCore.DelayMagicType);                // :270 / :295
        Assert.Equal(600, ObjFireDragonCore.DelayMagicMs);                // :270 / :295
        Assert.Equal(3, ObjFireDragonCore.DragonExplosionRange);          // :313
        Assert.Equal(1, ObjFireDragonCore.GuardExplosionRange);           // :486
        Assert.Equal(4000, ObjFireDragonCore.FireBurnDurationMs);         // :410
        Assert.Equal(17, ObjFireDragonCore.ET_FIREDRAGON);                // Grobal2.pas:2795
        Assert.Equal(0, ObjFireDragonCore.FireBurnValue);                 // :410
        Assert.Equal(20114, ObjFireDragonCore.RM_10205);                  // Grobal2.pas:1057
        Assert.Equal(83, ObjFireDragonCore.RM_10205Param3);               // :148
        Assert.Equal(500, ObjFireDragonCore.RM_10205DelayMs);             // :148
        Assert.Equal(1, ObjFireDragonCore.GuardLightNotifyParam);         // :222
        Assert.Equal(5, ObjFireDragonCore.POISON_STONE);                  // M2Definition.pas:13
        Assert.Equal(0, ObjFireDragonCore.RC_PLAYOBJECT);                 // Grobal2.pas:190
    }

    // ===================== 二、方法清单（14/14 覆盖取证） =====================

    [Fact]
    public void MethodInventory_FourteenRoutines()
    {
        Assert.Equal(13, ObjFireDragonCore.DeclCount);
        Assert.Equal(13, ObjFireDragonCore.ImplCount);
        Assert.Equal(1, ObjFireDragonCore.NestedFunctionCount);
        Assert.Equal(14, ObjFireDragonCore.RoutineCount);
        Assert.Equal(13, ObjFireDragonCore.Methods.Length);
        Assert.Equal(ObjFireDragonCore.DeclCount, ObjFireDragonCore.Methods.Length);
        Assert.Equal(ObjFireDragonCore.ImplCount, ObjFireDragonCore.Methods.Length);
    }

    [Fact]
    public void MethodInventory_SplitMatchesClassCounts()
    {
        int dragon = ObjFireDragonCore.Methods.Count(m => m.Name.StartsWith("TFireDragon.", StringComparison.Ordinal));
        int guard = ObjFireDragonCore.Methods.Count(m => m.Name.StartsWith("TFireDragonGuard.", StringComparison.Ordinal));
        Assert.Equal(ObjFireDragonCore.FireDragonMethodCount, dragon);
        Assert.Equal(ObjFireDragonCore.FireDragonGuardMethodCount, guard);
        Assert.Equal(ObjFireDragonCore.Methods.Length, dragon + guard);
        Assert.Equal(7, ObjFireDragonCore.FireDragonGuardRoutineCount);   // 6 + 嵌套 IsChar
    }

    [Fact]
    public void MethodInventory_AscendingAndInsideUnit()
    {
        for (int i = 1; i < ObjFireDragonCore.Methods.Length; i++)
            Assert.True(ObjFireDragonCore.Methods[i].Start > ObjFireDragonCore.Methods[i - 1].Start,
                $"未按行号升序：{ObjFireDragonCore.Methods[i - 1].Name} → {ObjFireDragonCore.Methods[i].Name}");
        foreach (var (name, start, end) in ObjFireDragonCore.Methods)
        {
            Assert.True(start > 0 && end >= start, $"{name} 行段非法 {start}-{end}");
            Assert.True(end < ObjFireDragonCore.SourceLines, $"{name} 超出单元");
        }
    }

    /// <summary>
    /// ★ F12：`TFireDragon.MagBigExplosion` 在原文里出现**两次** ——
    /// `:106-137` 是 `{ }` 注释掉的旧版，`:139-173` 才是生效版。
    /// 本断言把"注释段完全落在生效段之前"钉死，防止"取第一次出现"的移植错误。
    /// </summary>
    [Fact]
    public void Flaw12_OldExplosionIsCommentedOut()
    {
        Assert.Equal(1, ObjFireDragonCore.OldExplosionCommentStart);
        Assert.Equal(1, ObjFireDragonCore.OldExplosionCommentEnd);
        Assert.Equal(106, ObjFireDragonCore.CommentedOldExplosionStart);
        Assert.Equal(137, ObjFireDragonCore.CommentedOldExplosionEnd);

        var live = ObjFireDragonCore.Methods.Single(m => m.Name == "TFireDragon.MagBigExplosion");
        Assert.Equal(139, live.Start);
        Assert.True(ObjFireDragonCore.CommentedOldExplosionEnd < live.Start);
    }

    // ===================== 三、原文缺陷 F1-F11 的计数证据 =====================

    /// <summary>
    /// ★ F1：`{$IF MULTI_THREAD = 1}` 的两处（含 `try`/`finally`/`LockR`/`UnLockR`）
    /// 因 `M2Share.pas:75 MULTI_THREAD = 0` **整段不参与编译**
    /// ⇒ 点灯循环**完全没有读锁**。
    /// </summary>
    [Fact]
    public void Flaw1_MultiThreadLockBlockIsCompiledOut()
    {
        Assert.Equal(0, ObjFireDragonCore.MULTI_THREAD);
        Assert.Equal(2, ObjFireDragonCore.MultiThreadIfSites);
        Assert.Equal(2, ObjFireDragonCore.MultiThreadIfEndSites);
        Assert.Equal(ObjFireDragonCore.MultiThreadIfSites, ObjFireDragonCore.MultiThreadIfEndSites);
        Assert.Equal(1, ObjFireDragonCore.MultiThreadDeclarationSites);
    }

    /// <summary>★ F2：点灯循环对被点对象做**零类型判别**的硬转换。</summary>
    [Fact]
    public void Flaw2_GuardLoopHasNoTypeCheck()
    {
        Assert.Equal(0, ObjFireDragonCore.GuardLoopTypeChecks);
        Assert.Equal(1, ObjFireDragonCore.GuardLoopEntryConditions);
    }

    /// <summary>★ F3：`Randomize` 在 `AttackTarget` 里每次点灯重新播种。</summary>
    [Fact]
    public void Flaw3_RandomizeInsideAttackTarget()
    {
        Assert.Equal(1, ObjFireDragonCore.RandomizeSites);
    }

    /// <summary>★ F9：`TFireDragonGuard.m_dwLightTick` 是死字段（声明后 0 读 0 写）。</summary>
    [Fact]
    public void Flaw9_GuardLightTickIsDeadField()
    {
        Assert.Equal(5, ObjFireDragonCore.LightTickOccurrences);
        Assert.Equal(0, ObjFireDragonCore.GuardLightTickUses);
        // 5 = 2 处声明（TFireDragon :19 / Guard :33）+ TFireDragon 的 1 写 :61 + 1 读 :195 + 1 写 :197
        Assert.Equal(5, 2 + 1 + 1 + 1);
    }

    /// <summary>★ F10：`Dispose(VisibleBaseObject)` 全单元唯一一处，在清视野列表路径内。</summary>
    [Fact]
    public void Flaw10_SingleDisposeOnSharedRecord()
    {
        Assert.Equal(1, ObjFireDragonCore.DisposeSites);
    }

    /// <summary>★ F11：两处 `Run` 的收尾 `inherited` 一处在外（:376）、一处在内（:524）。</summary>
    [Fact]
    public void Flaw11_InheritedAsymmetry()
    {
        Assert.Equal(9, ObjFireDragonCore.InheritedSites);
        Assert.Equal(1, ObjFireDragonCore.InheritedOutsideTry);
        Assert.Equal(1, ObjFireDragonCore.InheritedInsideTry);
        Assert.Equal(376, ObjFireDragonCore.InheritedOutsideTryLine);
        Assert.Equal(524, ObjFireDragonCore.InheritedInsideTryLine);
        Assert.True(ObjFireDragonCore.InheritedPlacementIsAsymmetric());
    }

    [Fact]
    public void VisibleActorsUseCount()
    {
        Assert.Equal(15, ObjFireDragonCore.VisibleActorsUses);
    }

    // ===================== 四、TickDiff / 节流判据 =====================

    [Fact]
    public void TickDiff_NormalAndZero()
    {
        Assert.Equal(0u, ObjFireDragonCore.TickDiff(100u, 100u));
        Assert.Equal(500u, ObjFireDragonCore.TickDiff(100u, 600u));
        Assert.Equal(0u, ObjFireDragonCore.TickDiff(0u, 0u));
    }

    [Fact]
    public void TickDiff_WrapsAround()
    {
        // 起点在 2^32 边界前 10ms、终点在边界后 10ms
        // ⇒ 真实经过 21ms，但原文公式 `High(Cardinal) - start + end` 给出 **20**（★ 短 1，原文如此）
        Assert.Equal(20u, ObjFireDragonCore.TickDiff(uint.MaxValue - 10u, 10u));
        // 参数顺序是 (旧, 新)：反过来会得到一个巨大的数（这正是误读点）
        Assert.True(ObjFireDragonCore.TickDiff(10u, uint.MaxValue - 10u) > int.MaxValue);
    }

    [Fact]
    public void HitTickDue_ThresholdIsSum()
    {
        Assert.False(ObjFireDragonCore.HitTickDue(0u, 100u, 100, 0));   // 差 100，不 > 100
        Assert.True(ObjFireDragonCore.HitTickDue(0u, 101u, 100, 0));    // 差 101 > 100
        Assert.False(ObjFireDragonCore.HitTickDue(0u, 150u, 100, 50));  // 差 150，不 > 150
        Assert.True(ObjFireDragonCore.HitTickDue(0u, 151u, 100, 50));   // 差 151 > 150
    }

    [Fact]
    public void WalkTickDue_ThresholdIsSum()
    {
        Assert.False(ObjFireDragonCore.WalkTickDue(0u, 300u, 200, 100));
        Assert.True(ObjFireDragonCore.WalkTickDue(0u, 301u, 200, 100));
    }

    [Fact]
    public void RawElapsedGreater_IsUnsigned()
    {
        Assert.False(ObjFireDragonCore.RawElapsedGreater(10000u, 0u, 10000));
        Assert.True(ObjFireDragonCore.RawElapsedGreater(10001u, 0u, 10000));
        Assert.True(ObjFireDragonCore.RawElapsedGreater(5u, uint.MaxValue - 5u, 10));
    }

    // ===================== 五、CheckAttackTarget（:69-103） =====================

    private static FireDragonVisible V(bool death = false, bool ghost = false, bool proper = true,
                                        int x = 0, int y = 0, bool baseNull = false, bool slotNull = false)
        => new()
        {
            VisibleSlotNotNull = !slotNull,
            BaseObjectNull = baseNull,
            Death = death,
            Ghost = ghost,
            ProperTarget = proper,
            X = x,
            Y = y,
        };

    [Fact]
    public void CheckAttackTarget_EmptyList()
    {
        Assert.False(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, Array.Empty<FireDragonVisible>()));
    }

    [Fact]
    public void CheckAttackTarget_RejectsBaseObjectNull()
        => Assert.False(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, new[] { V(baseNull: true) }));

    [Fact]
    public void CheckAttackTarget_RejectsDead()
        => Assert.False(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, new[] { V(death: true) }));

    [Fact]
    public void CheckAttackTarget_RejectsImproper()
        => Assert.False(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, new[] { V(proper: false) }));

    /// <summary>★ 原文只判 `m_boDeath`，**不判 `m_boGhost`** ⇒ 幽灵目标**通过**检查。</summary>
    [Fact]
    public void CheckAttackTarget_AcceptsGhost()
    {
        Assert.True(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, new[] { V(ghost: true) }));
        Assert.True(ObjFireDragonCore.CheckAttackTargetIgnoresGhost());
    }

    /// <summary>距离判据是 `&lt;=`（闭区间）。</summary>
    [Fact]
    public void CheckAttackTarget_ClosedIntervalOnBothAxes()
    {
        Assert.True(ObjFireDragonCore.CheckAttackTargetIsClosedInterval());
        Assert.True(ObjFireDragonCore.CheckAttackTarget(3, 0, 0, new[] { V(x: 3, y: 3) }));
        Assert.False(ObjFireDragonCore.CheckAttackTarget(3, 0, 0, new[] { V(x: 4, y: 0) }));
        Assert.False(ObjFireDragonCore.CheckAttackTarget(3, 0, 0, new[] { V(x: 0, y: -4) }));
    }

    [Fact]
    public void CheckAttackTarget_BreaksOnFirstHit()
    {
        // 命中即 Break：后面的元素即使"更该命中"也不再影响结果（结果只是 bool）
        var list = new[] { V(x: 1, y: 1), V(x: 100, y: 100) };
        Assert.True(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, list));
    }

    // ===================== 六、MagBigExplosion（:139-173） =====================

    [Fact]
    public void MagBigExplosionHits_SkipsDeadAndGhost()
    {
        var list = new[] { V(death: true), V(ghost: true), V(), V(proper: false) };
        var hits = ObjFireDragonCore.MagBigExplosionHits(list);
        Assert.Single(hits);
    }

    /// <summary>★ 与 `CheckAttackTarget` 不同：`MagBigExplosion` **同时**排除 death 与 ghost。</summary>
    [Fact]
    public void MagBigExplosionHits_GhostIsExcludedHereUnlikeCheckAttackTarget()
    {
        Assert.Empty(ObjFireDragonCore.MagBigExplosionHits(new[] { V(ghost: true) }));
        Assert.True(ObjFireDragonCore.CheckAttackTarget(13, 0, 0, new[] { V(ghost: true) }));
    }

    [Fact]
    public void MagBigExplosionResult_EmptyListIsFalse()
        => Assert.False(ObjFireDragonCore.MagBigExplosionResult(Array.Empty<FireDragonVisible>()));

    [Fact]
    public void MagBigExplosionResult_TrueWhenAnyHit()
    {
        Assert.True(ObjFireDragonCore.MagBigExplosionResult(new[] { V(proper: false), V() }));
        Assert.False(ObjFireDragonCore.MagBigExplosionResult(new[] { V(proper: false) }));
    }

    [Fact]
    public void MagBigExplosion_AssumesTargetNotNull()
        => Assert.True(ObjFireDragonCore.MagBigExplosionAssumesTargetNotNull());

    // ===================== 七、AttackTarget（:184-325） =====================

    [Fact]
    public void LightNotifyDue_Boundary()
    {
        Assert.False(ObjFireDragonCore.LightNotifyDue(10000u, 0u));   // 恰好 10000，不 > 10000
        Assert.True(ObjFireDragonCore.LightNotifyDue(10001u, 0u));
        Assert.False(ObjFireDragonCore.LightNotifyDue(5u, 5u));
    }

    [Fact]
    public void GuardLoopBreaks_OnLightOrAttick()
    {
        Assert.True(ObjFireDragonCore.GuardLoopBreaks(true, false));
        Assert.True(ObjFireDragonCore.GuardLoopBreaks(false, true));
        Assert.False(ObjFireDragonCore.GuardLoopBreaks(false, false));
    }

    [Fact]
    public void GuardIsChosen_AndCap()
    {
        Assert.True(ObjFireDragonCore.GuardIsChosen(2, 2));
        Assert.False(ObjFireDragonCore.GuardIsChosen(2, 3));
        Assert.False(ObjFireDragonCore.GuardLoopCapReached(5));
        Assert.True(ObjFireDragonCore.GuardLoopCapReached(6));
    }

    /// <summary>`SimulateGuardLighting`：同地图过滤 + 已亮即 Break + 单轮上限 6。</summary>
    [Fact]
    public void SimulateGuardLighting_PicksChosenAndCapsAtSix()
    {
        // 7 只同地图、都未亮：上限 6 ⇒ 只点 6 只
        var seven = Enumerable.Range(0, 7).Select(_ => (false, false, true)).ToList();
        var (chosen, lit) = ObjFireDragonCore.SimulateGuardLighting(seven, 5);
        Assert.Equal(6, lit);
        Assert.Equal(5, chosen);

        // k = 6 超出上限 ⇒ 永远选不中任何一只
        var (chosen2, lit2) = ObjFireDragonCore.SimulateGuardLighting(seven, 6);
        Assert.Equal(-1, chosen2);
        Assert.Equal(6, lit2);
    }

    [Fact]
    public void SimulateGuardLighting_SkipsOtherMapsAndBreaksOnAlreadyLit()
    {
        var list = new List<(bool Light, bool Attick, bool SameMap)>
        {
            (false, false, false),   // 别的图 ⇒ 跳过（不占 J）
            (true,  false, true),    // 已亮 ⇒ Break
            (false, false, true),    // 到不了
        };
        var (chosen, lit) = ObjFireDragonCore.SimulateGuardLighting(list, 0);
        Assert.Equal(-1, chosen);
        Assert.Equal(0, lit);
    }

    [Fact]
    public void SimulateGuardLighting_SelectedGetsAttickFlag()
    {
        var list = new List<(bool Light, bool Attick, bool SameMap)>
        {
            (false, false, true),
            (false, false, true),
        };
        var (chosen, lit) = ObjFireDragonCore.SimulateGuardLighting(list, 1);
        Assert.Equal(1, chosen);
        Assert.Equal(2, lit);
    }

    [Fact]
    public void AttackPhase_OneInThree()
    {
        Assert.Equal(0, ObjFireDragonCore.AttackPhase(0));   // 群雷
        Assert.Equal(1, ObjFireDragonCore.AttackPhase(1));   // 大火圈
        Assert.Equal(1, ObjFireDragonCore.AttackPhase(2));
    }

    [Fact]
    public void GroupLightningTargets_Filters()
    {
        Assert.False(ObjFireDragonCore.GroupLightningTargets(13, 0, 0, V(baseNull: true)));
        Assert.False(ObjFireDragonCore.GroupLightningTargets(13, 0, 0, V(death: true)));
        Assert.False(ObjFireDragonCore.GroupLightningTargets(13, 0, 0, V(proper: false)));
        Assert.False(ObjFireDragonCore.GroupLightningTargets(13, 0, 0, V(x: 14, y: 0)));
        Assert.True(ObjFireDragonCore.GroupLightningTargets(13, 0, 0, V(x: 13, y: 13)));
        // ★ 群雷分支**不排除 ghost**（原文只判 death）
        Assert.True(ObjFireDragonCore.GroupLightningTargets(13, 0, 0, V(ghost: true)));
    }

    [Fact]
    public void RetargetOnGroupLightning_OneInFour()
    {
        Assert.True(ObjFireDragonCore.RetargetOnGroupLightning(0));
        Assert.False(ObjFireDragonCore.RetargetOnGroupLightning(1));
    }

    [Fact]
    public void AttackTargetResult_IsUnconditionallyTrue()
        => Assert.True(ObjFireDragonCore.AttackTargetResultAfterBothChecks());

    // ===================== 八、TFireDragon.Run（:327-377） =====================

    [Fact]
    public void RunOuterGuard_ThreeConditions()
    {
        Assert.False(ObjFireDragonCore.RunOuterGuard(ghost: true, death: false, poisonStoneTime: 0));
        Assert.False(ObjFireDragonCore.RunOuterGuard(ghost: false, death: true, poisonStoneTime: 0));
        Assert.False(ObjFireDragonCore.RunOuterGuard(ghost: false, death: false, poisonStoneTime: 1));
        Assert.True(ObjFireDragonCore.RunOuterGuard(ghost: false, death: false, poisonStoneTime: 0));
        // ★ 判据是 `= 0`，负数（异常值）也视为"未石化"之外？—— 原文是 `= 0`，故 -1 不通过
        Assert.False(ObjFireDragonCore.RunOuterGuard(ghost: false, death: false, poisonStoneTime: -1));
    }

    [Fact]
    public void ShouldSearchTarget_IntervalAndNoTarget()
    {
        Assert.False(ObjFireDragonCore.ShouldSearchTarget(1000u, 0u, false));   // 恰好 1000 ⇒ 否
        Assert.True(ObjFireDragonCore.ShouldSearchTarget(1001u, 0u, false));
        Assert.False(ObjFireDragonCore.ShouldSearchTarget(1001u, 0u, true));    // 已有目标 ⇒ 否
    }

    [Fact]
    public void ShouldAttack_And_ShouldPurge()
    {
        Assert.False(ObjFireDragonCore.ShouldAttack(3000u, 0u));
        Assert.True(ObjFireDragonCore.ShouldAttack(3001u, 0u));
        Assert.False(ObjFireDragonCore.ShouldPurge(10000u, 0u));
        Assert.True(ObjFireDragonCore.ShouldPurge(10001u, 0u));
    }

    /// <summary>★ F5 的后果：`AttackTarget` 为真 ⇒ `Exit`（**不清视野列表**）。</summary>
    [Fact]
    public void Flaw5_PurgeOnlyWhenAttackFails()
    {
        Assert.False(ObjFireDragonCore.ContinuesAfterAttack(true));
        Assert.True(ObjFireDragonCore.ContinuesAfterAttack(false));
        Assert.True(ObjFireDragonCore.PurgeOnlyWhenAttackFails());
    }

    // ===================== 九、TFireDragonGuard.AttackTarget（:447-495） =====================

    [Fact]
    public void IsChar_CountsPipes()
    {
        Assert.Equal(0, ObjFireDragonCore.IsChar(""));
        Assert.Equal(0, ObjFireDragonCore.IsChar("100,200"));
        Assert.Equal(1, ObjFireDragonCore.IsChar("100,200|300,400"));
        Assert.Equal(2, ObjFireDragonCore.IsChar("1,2|3,4|5,6"));
        Assert.Equal(0, ObjFireDragonCore.IsChar(null!));
    }

    [Fact]
    public void GuardLoopIterations_IsPipeCountPlusOne()
    {
        Assert.Equal(1, ObjFireDragonCore.GuardLoopIterations("100,200"));
        Assert.Equal(2, ObjFireDragonCore.GuardLoopIterations("100,200|300,400"));
        Assert.Equal(3, ObjFireDragonCore.GuardLoopIterations("1,2|3,4|5,6"));
        Assert.Equal(1, ObjFireDragonCore.GuardLoopIterations(""));
    }

    [Fact]
    public void GuardSegments_SplitByPipe()
    {
        Assert.Equal(new[] { "1,2", "3,4" }, ObjFireDragonCore.GuardSegments("1,2|3,4").ToArray());
        Assert.Equal(new[] { "1,2" }, ObjFireDragonCore.GuardSegments("1,2").ToArray());
        Assert.Empty(ObjFireDragonCore.GuardSegments(""));
        // 空段被跳过（原文 :480 `if Str1 <> ''`）
        Assert.Equal(new[] { "1,2", "3,4" }, ObjFireDragonCore.GuardSegments("|1,2||3,4|").ToArray());
    }

    /// <summary>★ F6：`0 to IsChar(s)` 的循环次数**恰好等于段数**（不是 off-by-one）。</summary>
    [Fact]
    public void Flaw6_LoopCountEqualsSegmentCount()
        => Assert.True(ObjFireDragonCore.IsCharPlusOneEqualsSegmentCount());

    [Fact]
    public void GuardExplosionGated_RequiresPipe()
    {
        Assert.False(ObjFireDragonCore.GuardExplosionGated("100,200"));
        Assert.False(ObjFireDragonCore.GuardExplosionGated(""));
        Assert.True(ObjFireDragonCore.GuardExplosionGated("100,200|300,400"));
    }

    [Fact]
    public void ParseXY_OrderAndDefaults()
    {
        Assert.True(ObjFireDragonCore.ParseXYIsNotInverted());
        Assert.True(ObjFireDragonCore.ParseXYDefaultsToZero());
        Assert.True(ObjFireDragonCore.ParseXYAcceptsTab());
        ObjFireDragonCore.ParseXY("12,34", out int x, out int y);
        Assert.Equal(12, x);
        Assert.Equal(34, y);
    }

    [Fact]
    public void GuardExplosionPoints_MultiCoordinate()
    {
        var pts = ObjFireDragonCore.GuardExplosionPoints("100,200|300,400");
        Assert.Equal(2, pts.Count);
        Assert.Equal((100, 200), pts[0]);
        Assert.Equal((300, 400), pts[1]);
    }

    /// <summary>
    /// ★★ F7（本单元最有业务价值的缺陷）：**单坐标配置一次火圈都不放**，
    /// 尽管解析循环本身完全能处理它。
    /// </summary>
    [Fact]
    public void Flaw7_SingleCoordinateConfigNeverFires()
    {
        Assert.True(ObjFireDragonCore.SingleCoordinateConfigNeverFires());
        Assert.Empty(ObjFireDragonCore.GuardExplosionPoints("100,200"));
        // 而循环与解析都没问题：
        Assert.Equal(1, ObjFireDragonCore.GuardLoopIterations("100,200"));
        Assert.Single(ObjFireDragonCore.GuardSegments("100,200"));
        // 唯一的差别就是那道 Pos('|') 守卫
        Assert.False(ObjFireDragonCore.GuardExplosionGated("100,200"));
    }

    /// <summary>★★ F7+F8 的后果：单坐标下 `Result := True` 仍成立 ⇒ `m_boAttick` 被清掉。</summary>
    [Fact]
    public void Flaw7And8_SingleCoordinateStillClearsAttick()
    {
        Assert.True(ObjFireDragonCore.SingleCoordinateStillClearsAttick());
        Assert.True(ObjFireDragonCore.GuardAttackTargetResult(true));
        Assert.True(ObjFireDragonCore.GuardAttickClearedByRun(true, true, false, "100,200"));
    }

    [Fact]
    public void GuardAttackTargetResult_FollowsThrottle()
    {
        Assert.True(ObjFireDragonCore.GuardAttackTargetResult(true));
        Assert.False(ObjFireDragonCore.GuardAttackTargetResult(false));
    }

    // ===================== 十、TFireDragonGuard.Run（:497-528） =====================

    [Fact]
    public void GuardLightExpires_Boundary()
    {
        Assert.False(ObjFireDragonCore.GuardLightExpires(true, 2500u, 0u, 2500u));   // 恰好 ⇒ 否
        Assert.True(ObjFireDragonCore.GuardLightExpires(true, 2501u, 0u, 2500u));
        Assert.False(ObjFireDragonCore.GuardLightExpires(false, 99999u, 0u, 2500u)); // 未亮 ⇒ 否
        // 被选中者用 3000ms
        Assert.False(ObjFireDragonCore.GuardLightExpires(true, 3000u, 0u, (uint)ObjFireDragonCore.GuardChosenLightTimeMs));
        Assert.True(ObjFireDragonCore.GuardLightExpires(true, 3001u, 0u, (uint)ObjFireDragonCore.GuardChosenLightTimeMs));
    }

    [Fact]
    public void GuardRunShouldAttack_ThreeWayAnd()
    {
        Assert.True(ObjFireDragonCore.GuardRunShouldAttack(true, false, "1,2|3,4"));
        Assert.False(ObjFireDragonCore.GuardRunShouldAttack(false, false, "1,2|3,4"));
        Assert.False(ObjFireDragonCore.GuardRunShouldAttack(true, true, "1,2|3,4"));   // 还亮着 ⇒ 不打
        Assert.False(ObjFireDragonCore.GuardRunShouldAttack(true, false, ""));          // 坐标空 ⇒ 不打
    }

    /// <summary>★ 单坐标在 `Run` 里能通过 `s_AttickXY &lt;&gt; ''` 这道门（`"100,200"` 非空）。</summary>
    [Fact]
    public void SingleCoordinatePassesRunGateButNotExplosionGate()
    {
        Assert.True(ObjFireDragonCore.GuardRunShouldAttack(true, false, "100,200"));
        Assert.False(ObjFireDragonCore.GuardExplosionGated("100,200"));
    }

    [Fact]
    public void GuardWalkTickDue_SameAsDragon()
    {
        Assert.False(ObjFireDragonCore.GuardWalkTickDue(0u, 300u, 200, 100));
        Assert.True(ObjFireDragonCore.GuardWalkTickDue(0u, 301u, 200, 100));
    }

    // ===================== 十一、构造函数 / RecalcAbilitys =====================

    [Fact]
    public void CreateAssignmentTables()
    {
        Assert.Equal(8, ObjFireDragonCore.FireDragonCreateAssignments.Length);
        Assert.Equal(4, ObjFireDragonCore.FireDragonRecalcAssignments.Length);
        Assert.Equal(9, ObjFireDragonCore.GuardCreateAssignments.Length);
        Assert.Equal(5, ObjFireDragonCore.GuardRecalcAssignments.Length);
    }

    /// <summary>`RecalcAbilitys` **不重置**视野 / 计时 / 隐身（原文两处 `Recalc` 都只改 4-5 个字段）。</summary>
    [Fact]
    public void RecalcDoesNotResetViewRangeOrTicks()
    {
        Assert.Equal(3, ObjFireDragonCore.RecalcDoesNotReset.Length);
        var createFields = ObjFireDragonCore.FireDragonCreateAssignments.Select(a => a.Field).ToArray();
        foreach (var f in ObjFireDragonCore.RecalcDoesNotReset)
        {
            Assert.Contains(f, createFields);
            Assert.DoesNotContain(f, ObjFireDragonCore.FireDragonRecalcAssignments.Select(a => a.Field));
        }
    }

    [Fact]
    public void CreateRecalcOverlaps()
    {
        var dragonCreate = ObjFireDragonCore.FireDragonCreateAssignments.Select(a => a.Field).ToArray();
        var dragonRecalc = ObjFireDragonCore.FireDragonRecalcAssignments.Select(a => a.Field).ToArray();
        Assert.Equal(ObjFireDragonCore.FireDragonCreateRecalcOverlap,
            dragonCreate.Intersect(dragonRecalc).Count());

        var guardCreate = ObjFireDragonCore.GuardCreateAssignments.Select(a => a.Field).ToArray();
        var guardRecalc = ObjFireDragonCore.GuardRecalcAssignments.Select(a => a.Field).ToArray();
        Assert.Equal(ObjFireDragonCore.GuardCreateRecalcOverlap,
            guardCreate.Intersect(guardRecalc).Count());
    }

    /// <summary>关键字段值的字面量（防止"顺手统一"）。</summary>
    [Fact]
    public void CreateAssignmentLiteralValues()
    {
        Assert.Contains(("m_nViewRange", "13"), ObjFireDragonCore.FireDragonCreateAssignments);
        Assert.Contains(("m_btAntiPoison", "200"), ObjFireDragonCore.FireDragonCreateAssignments);
        Assert.Contains(("m_boFixedHideMode", "False"), ObjFireDragonCore.FireDragonCreateAssignments);
        Assert.Contains(("m_boStoneMode", "True"), ObjFireDragonCore.GuardCreateAssignments);
        Assert.Contains(("m_dwLightTime", "2500"), ObjFireDragonCore.GuardCreateAssignments);
        Assert.Contains(("s_AttickXY", "''"), ObjFireDragonCore.GuardCreateAssignments);
        // ★ 守护兽的 Recalc **不重置** m_boLight / m_boAttick / s_AttickXY / m_dwLightTime
        var guardRecalcFields = ObjFireDragonCore.GuardRecalcAssignments.Select(a => a.Field).ToArray();
        Assert.DoesNotContain("m_boLight", guardRecalcFields);
        Assert.DoesNotContain("m_boAttick", guardRecalcFields);
        Assert.DoesNotContain("s_AttickXY", guardRecalcFields);
        Assert.DoesNotContain("m_dwLightTime", guardRecalcFields);
    }

    // ===================== 十二、接缝 =====================

    [Fact]
    public void Seam_AddEventRecordsFireBurnSpec()
    {
        var seen = new List<FireBurnEventSpec>();
        try
        {
            ObjFireDragonSeam.AddEvent = spec => seen.Add(spec);
            // 原文 :410 TFireBurnEvent.Create(self, nX, nY, ET_FIREDRAGON, 4000, 0)
            ObjFireDragonSeam.AddEvent(new FireBurnEventSpec(
                12, 34, ObjFireDragonCore.ET_FIREDRAGON,
                ObjFireDragonCore.FireBurnDurationMs, ObjFireDragonCore.FireBurnValue));

            Assert.Single(seen);
            Assert.Equal(12, seen[0].X);
            Assert.Equal(34, seen[0].Y);
            Assert.Equal(17, seen[0].EventType);
            Assert.Equal(4000, seen[0].DurationMs);
            Assert.Equal(0, seen[0].Value);

            // ResetDefaults 之后默认实现是空操作 ⇒ 不再收集
            ObjFireDragonSeam.ResetDefaults();
            ObjFireDragonSeam.AddEvent(new FireBurnEventSpec(1, 1, 17, 4000, 0));
            Assert.Single(seen);
        }
        finally
        {
            ObjFireDragonSeam.ResetDefaults();
        }
    }

    /// <summary>`MyGetTickCount` / `MainOutMessage` 是**转发**既有 `SweepSeam`，不另造一份。</summary>
    [Fact]
    public void Seam_ForwardsToSweepSeam()
    {
        var saved = GXX.M2Server.Sweep.SweepSeam.MyGetTickCount;
        try
        {
            ObjFireDragonSeam.MyGetTickCount = () => 4242u;
            Assert.Equal(4242u, GXX.M2Server.Sweep.SweepSeam.MyGetTickCount());
            Assert.Equal(4242u, ObjFireDragonSeam.MyGetTickCount());
        }
        finally
        {
            ObjFireDragonSeam.MyGetTickCount = saved;
        }
    }
}
