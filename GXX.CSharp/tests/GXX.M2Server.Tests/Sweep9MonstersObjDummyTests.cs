// ============================================================================
//  测试：Source/M2Engine/ObjDummy.pas → GXX.M2Server.Sweep9.Monsters（1:1）
//
//  覆盖策略：
//    * 单元常量与标签字面量逐个对账；
//    * 例程清单 19 条：条数 / 分段计数 / 顺序 / 落在单元内；
//    * 原文缺陷 F1-F15 **逐条差异断言**（含计数取证，§37.3）；
//    * 纯逻辑（方向族 / GetPoint / CanAutoUseMagic / IsProperTarget /
//      StartPickUpItem / Wondering / Run 判定）含分支、边界、越界与可执行证据。
//    * 全部为纯函数调用，不碰磁盘、不需要对象模型。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep9.Monsters;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class Sweep9MonstersObjDummyTests
{
    // ===================== 一、常量与标签 =====================

    [Fact]
    public void SourceIdentity()
    {
        Assert.Equal("Source/M2Engine/ObjDummy.pas", ObjDummyCore.SourceUnit);
        Assert.Equal(1245, ObjDummyCore.SourceLines);
    }

    [Fact]
    public void DirectionConstants_MatchGrobal2()
    {
        Assert.Equal(0, ObjDummyCore.DR_UP);
        Assert.Equal(1, ObjDummyCore.DR_UPRIGHT);
        Assert.Equal(2, ObjDummyCore.DR_RIGHT);
        Assert.Equal(3, ObjDummyCore.DR_DOWNRIGHT);
        Assert.Equal(4, ObjDummyCore.DR_DOWN);
        Assert.Equal(5, ObjDummyCore.DR_DOWNLEFT);
        Assert.Equal(6, ObjDummyCore.DR_LEFT);
        Assert.Equal(7, ObjDummyCore.DR_UPLEFT);
        Assert.Equal(new byte[] { 2, 4, 6 }, ObjDummyCore.CheckSteps);
        Assert.Equal(0, ObjDummyCore.CheckStepsLow);
        Assert.Equal(2, ObjDummyCore.CheckStepsHigh);
    }

    [Fact]
    public void OtherConstants_MatchSource()
    {
        Assert.Equal(5, ObjDummyCore.HAM_GUILD);                     // Grobal2.pas:163
        Assert.Equal(0, ObjDummyCore.RC_PLAYOBJECT);
        Assert.Equal(10, ObjDummyCore.RC_NPC);
        Assert.Equal(50, ObjDummyCore.RC_ANIMAL);
        Assert.Equal(112, ObjDummyCore.RC_ARCHERGUARD);
        Assert.Equal(110, ObjDummyCore.SiegeCityDoorRace);          // :297 字面量
        Assert.Equal(111, ObjDummyCore.SiegeCityWallRace);          // :297 字面量
        Assert.Equal(120040918, ObjDummyCore.CLIENT_VERSION_NUMBER);
        Assert.Equal(9, ObjDummyCore.U_BUJUK);
        Assert.Equal(5, ObjDummyCore.ET_FIRE);
        Assert.Equal(500, ObjDummyCore.DefaultWalkTimeMs);
        Assert.Equal(500, ObjDummyCore.RunDefaultWalkTimeMs);
        Assert.Equal(1, ObjDummyCore.WarriorRunAttackRange);
        Assert.Equal(5, ObjDummyCore.OtherRunAttackRange);
        Assert.Equal(20, ObjDummyCore.FireWallHuntRandomBound);
        Assert.Equal(3, ObjDummyCore.FireWallScanStepFrom);
        Assert.Equal(6, ObjDummyCore.FireWallScanStepTo);
        Assert.Equal(8, ObjDummyCore.DirectionCount);
        Assert.Equal(1, ObjDummyCore.MagicRageStepOffset);
        Assert.Equal(10, ObjDummyCore.HeroJointMaxDistance);
        Assert.Equal(8, ObjDummyCore.HeroJointRandomBound);
        Assert.Equal(2, ObjDummyCore.HeroJointFlyThreshold);
        Assert.Equal(4, ObjDummyCore.TaoistIdleRunRandomBound);
        Assert.Equal(10000, ObjDummyCore.MooteboCooldownMs);
        Assert.Equal(1000, ObjDummyCore.AutoMagicSecondToMs);
        Assert.Equal(1000, ObjDummyCore.SkillUseTickHigh);
    }

    [Fact]
    public void SkillConstants_MatchM2Share()
    {
        Assert.Equal(26, ObjDummyCore.SKILL_FIRESWORD);
        Assert.Equal(27, ObjDummyCore.SKILL_MOOTEBO);
        Assert.Equal(39, ObjDummyCore.SKILL_GROUPDEDING);
        Assert.Equal(42, ObjDummyCore.SKILL_42);
        Assert.Equal(56, ObjDummyCore.SKILL_56);
        Assert.Equal(66, ObjDummyCore.SKILL_66);
        Assert.Equal(75, ObjDummyCore.SKILL_75);
        Assert.Equal(113, ObjDummyCore.SKILL_113);
        Assert.Equal(114, ObjDummyCore.SKILL_114);
        Assert.Equal(115, ObjDummyCore.SKILL_115);
        Assert.Equal(1000, ObjDummyCore.CUSTOM_MAGIC_START_ID);
        Assert.Equal(300, ObjDummyCore.CUSTOM_MAGIC_COUNT);
    }

    [Fact]
    public void ScriptLabels_MatchSource()
    {
        Assert.Equal("@DummyStart", ObjDummyCore.DummyStartLabel);
        Assert.Equal("@DummyStop", ObjDummyCore.DummyStopLabel);
        Assert.Equal("@Run", ObjDummyCore.RunLabel);
        Assert.Equal("@Walk", ObjDummyCore.WalkLabel);
    }

    // ===================== 二、例程清单（19/19 覆盖取证） =====================

    [Fact]
    public void RoutineInventory_Nineteen()
    {
        Assert.Equal(16, ObjDummyCore.DeclCount);
        Assert.Equal(16, ObjDummyCore.ImplCount);
        Assert.Equal(2, ObjDummyCore.UnitFunctionCount);
        Assert.Equal(1, ObjDummyCore.NestedFunctionCount);
        Assert.Equal(19, ObjDummyCore.RoutineCount);
        Assert.Equal(19, ObjDummyCore.Methods.Length);
        // 19 = 16 方法 + 2 单元级函数 + 1 嵌套
        Assert.Equal(ObjDummyCore.RoutineCount,
            ObjDummyCore.DeclCount + ObjDummyCore.UnitFunctionCount + ObjDummyCore.NestedFunctionCount);
    }

    [Fact]
    public void RoutineInventory_AscendingAndInsideUnit()
    {
        for (int i = 1; i < ObjDummyCore.Methods.Length; i++)
            Assert.True(ObjDummyCore.Methods[i].Start > ObjDummyCore.Methods[i - 1].Start,
                $"未按行号升序：{ObjDummyCore.Methods[i - 1].Name} → {ObjDummyCore.Methods[i].Name}");
        foreach (var (name, start, end) in ObjDummyCore.Methods)
        {
            Assert.True(start > 0 && end >= start, $"{name} 行段非法 {start}-{end}");
            Assert.True(end < ObjDummyCore.SourceLines, $"{name} 超出单元");
        }
    }

    /// <summary>★ F3：两个方向函数是**单元级函数**（清单里没有 `TDummyObject.` 前缀）。</summary>
    [Fact]
    public void DirectionFunctionsAreUnitLevel()
    {
        Assert.Contains(ObjDummyCore.Methods, m => m.Name == "NextDirClockwise");
        Assert.Contains(ObjDummyCore.Methods, m => m.Name == "NextDirAntiClockwise");
        Assert.DoesNotContain(ObjDummyCore.Methods, m => m.Name.StartsWith("TDummyObject.NextDir", StringComparison.Ordinal));
    }

    /// <summary>★ F4：`GetPoint` 是 `Wondering` 的**嵌套**函数（行段包含在 `Wondering` 内）。</summary>
    [Fact]
    public void GetPointIsNestedInWondering()
    {
        var wondering = ObjDummyCore.Methods.Single(m => m.Name == "TDummyObject.Wondering");
        var getPoint = ObjDummyCore.Methods.Single(m => m.Name == "GetPoint");
        Assert.True(wondering.Start <= getPoint.Start && getPoint.End <= wondering.End);
        Assert.Contains("GetPoint", ObjDummyCore.Methods.Select(m => m.Name));
    }

    // ===================== 三、原文缺陷 F1-F15 的计数证据 =====================

    /// <summary>★ F1：`GotoPath` 的整个函数体被 `(* *)` 注释掉 ⇒ 只剩一行 `Result := False`。</summary>
    [Fact]
    public void Flaw1_GotoPathIsEntirelyCommentedOut()
    {
        Assert.Equal(1, ObjDummyCore.GotoPathCommentOpen);
        Assert.Equal(1, ObjDummyCore.GotoPathCommentClose);
        Assert.Equal(1, ObjDummyCore.GotoPathLiveStatements);
        Assert.Equal(2, ObjDummyCore.GotoPathTrueInsideComment);
        // 唯一的 GotoPath() 调用点也在 { } 注释内
        Assert.Equal(1, ObjDummyCore.GotoPathCallSites);
        // 旧版块的行段
        Assert.Equal(520, ObjDummyCore.WonderingOldBlockOpenLine);
        Assert.Equal(541, ObjDummyCore.WonderingOldBlockCloseLine);
        var gotoPath = ObjDummyCore.Methods.Single(m => m.Name == "TDummyObject.GotoPath");
        Assert.True(ObjDummyCore.WonderingOldBlockCloseLine > gotoPath.End);
    }

    [Fact]
    public void Flaw2_DeadAndWriteOnlyFields()
    {
        Assert.Equal(1, ObjDummyCore.NotCanPickItemListOccurrences);      // 仅声明
        Assert.Equal(1, ObjDummyCore.StartPickItemTickOccurrences);       // 仅声明
        Assert.Equal(2, ObjDummyCore.AskTickOccurrences);                 // 声明 + 写
        Assert.Equal(0, ObjDummyCore.AskTickReadSites);                   // 读 0
    }

    [Fact]
    public void Flaw3_DirectionCaseHasNoElse()
    {
        Assert.Equal(8, ObjDummyCore.DirectionCaseLabels);
        Assert.Equal(0, ObjDummyCore.DirectionCaseElseBranches);
        Assert.True(ObjDummyCore.OutOfRangeDirectionBecomeUp());
    }

    [Fact]
    public void Flaw4_CommentedOldForLoop()
    {
        Assert.Equal(1, ObjDummyCore.GetPointCommentedForLoop);
        Assert.True(ObjDummyCore.CheckIndexClampsToZero());
    }

    [Fact]
    public void Flaw5_TwoGetNextDirAssignments()
    {
        Assert.Equal(2, ObjDummyCore.GetNextDirAssignSites);
    }

    [Fact]
    public void Flaw6_MinusOneDirectionSites()
    {
        Assert.Equal(1, ObjDummyCore.MinusOneDirectionSites);
        Assert.Equal(1, ObjDummyCore.PlusOneDirectionSites);
        Assert.Equal(1, ObjDummyCore.PlusCountDirectionSites);
    }

    [Fact]
    public void Flaw7_AttackTimeClampSites()
    {
        Assert.Equal(3, ObjDummyCore.AttackTimeClampSites);
    }

    [Fact]
    public void Flaw8_AutoMagicDuplication()
    {
        Assert.Equal(2, ObjDummyCore.AutoMagicConditionSites);
        Assert.Equal(2, ObjDummyCore.SecondGuardMissingConditions);
        Assert.True(ObjDummyCore.SecondGuardOmitsStartAndCanMove());
        Assert.True(ObjDummyCore.AutoMagicBlockIsDuplicated());
    }

    [Fact]
    public void Flaw9_HeroTargetDerefSites()
    {
        Assert.Equal(6, ObjDummyCore.HeroTargetDerefSites);
        Assert.Equal(0, ObjDummyCore.HeroTargetNilGuards);
        Assert.True(ObjDummyCore.HeroJointAttackDerefsTargetWithoutNilCheck());
    }

    [Fact]
    public void Flaw10_HeroObjectHardCasts()
    {
        Assert.Equal(21, ObjDummyCore.HeroObjectCastSites);
        Assert.Equal(0, ObjDummyCore.HeroObjectTypeChecks);
        Assert.True(ObjDummyCore.HeroObjectCastHasNoTypeCheck());
    }

    [Fact]
    public void Flaw11_MovementConditionStyles()
    {
        Assert.Equal(1, ObjDummyCore.FireWallHuntBranches);
        Assert.Equal(2, ObjDummyCore.MovementConditionStyles);
        Assert.True(ObjDummyCore.FireWallHuntOnlyForWizard());
        Assert.True(ObjDummyCore.MovementConditionStylesDiffer());
    }

    [Fact]
    public void Flaw13_TwoMasterReadForms()
    {
        Assert.Equal(2, ObjDummyCore.MasterReadForms);
        Assert.True(ObjDummyCore.ProperTargetVetoConditions == 6);
    }

    [Fact]
    public void Flaw15_SkillUseTickReadsHaveNoGuard()
    {
        Assert.Equal(8, ObjDummyCore.SkillUseTickReadsInCanAuto);
        Assert.Equal(0, ObjDummyCore.SkillUseTickGuardsInCanAuto);
        Assert.True(ObjDummyCore.SkillTickIndexStylesAreMixed());
        // 而 Run 里的写入**有**守卫
        Assert.True(ObjDummyCore.SkillUseTickWriteAllowed(1000));
        Assert.False(ObjDummyCore.SkillUseTickWriteAllowed(1001));
    }

    [Fact]
    public void MiscellaneousCounters()
    {
        Assert.Equal(1, ObjDummyCore.RandomizeSites);
        Assert.Equal(4, ObjDummyCore.GotoLableCallSites);
        Assert.True(ObjDummyCore.MoveTimeTickRefreshCount());
    }

    // ===================== 四、F3：方向函数（1:1） =====================

    [Theory]
    [InlineData(0, 1)] [InlineData(1, 2)] [InlineData(2, 3)] [InlineData(3, 4)]
    [InlineData(4, 5)] [InlineData(5, 6)] [InlineData(6, 7)] [InlineData(7, 0)]
    public void NextDirClockwise_Mapping(byte input, byte expected)
        => Assert.Equal(expected, ObjDummyCore.NextDirClockwise(input));

    [Theory]
    [InlineData(0, 7)] [InlineData(1, 0)] [InlineData(2, 1)] [InlineData(3, 2)]
    [InlineData(4, 3)] [InlineData(5, 4)] [InlineData(6, 5)] [InlineData(7, 6)]
    public void NextDirAntiClockwise_Mapping(byte input, byte expected)
        => Assert.Equal(expected, ObjDummyCore.NextDirAntiClockwise(input));

    [Fact]
    public void DirectionFunctions_OutOfRangeFallBackToUp()
    {
        Assert.Equal(ObjDummyCore.DR_UP, ObjDummyCore.NextDirClockwise(8));
        Assert.Equal(ObjDummyCore.DR_UP, ObjDummyCore.NextDirAntiClockwise(255));
        Assert.True(ObjDummyCore.OutOfRangeDirectionBecomeUp());
    }

    [Fact]
    public void DirectionFunctions_AreInversesAndCycle()
    {
        Assert.True(ObjDummyCore.ClockwiseAntiClockwiseAreInverses());
        Assert.True(ObjDummyCore.ClockwiseCycleIsEightSteps());
    }

    // ===================== 五、F4/F5：GetPoint =====================

    [Fact]
    public void ClampCheckStepIndex_OnlyLowerBoundIsHonored()
    {
        Assert.Equal(0, ObjDummyCore.ClampCheckStepIndex(0));
        Assert.Equal(1, ObjDummyCore.ClampCheckStepIndex(1));
        Assert.Equal(2, ObjDummyCore.ClampCheckStepIndex(2));
        Assert.Equal(0, ObjDummyCore.ClampCheckStepIndex(-1));
        Assert.Equal(0, ObjDummyCore.ClampCheckStepIndex(3));    // ★ 上界越界也退化为 0
        Assert.Equal(0, ObjDummyCore.ClampCheckStepIndex(int.MaxValue));
    }

    [Fact]
    public void GetPointRange_And_Steps()
    {
        Assert.Equal(2, ObjDummyCore.GetPointRange(0));
        Assert.Equal(4, ObjDummyCore.GetPointRange(1));
        Assert.Equal(6, ObjDummyCore.GetPointRange(2));
        Assert.Equal((2, 1), ObjDummyCore.GetPointSteps(0));
        Assert.Equal((4, 3), ObjDummyCore.GetPointSteps(1));
        Assert.Equal((6, 5), ObjDummyCore.GetPointSteps(2));
    }

    [Fact]
    public void ChooseNextDirStrategy_FollowsRandomTwo()
    {
        // `Random(2) = 0` 走顺时针、否则走逆时针；两者在 DR_UP 上给不同结果
        Assert.Equal(ObjDummyCore.NextDirClockwise(ObjDummyCore.DR_UP),
            ObjDummyCore.ChooseNextDirStrategy(0)(ObjDummyCore.DR_UP));
        Assert.Equal(ObjDummyCore.NextDirAntiClockwise(ObjDummyCore.DR_UP),
            ObjDummyCore.ChooseNextDirStrategy(1)(ObjDummyCore.DR_UP));
        Assert.NotEqual(ObjDummyCore.ChooseNextDirStrategy(0)(ObjDummyCore.DR_UP),
            ObjDummyCore.ChooseNextDirStrategy(1)(ObjDummyCore.DR_UP));
    }

    /// <summary>当前朝向的两个步长里就找到可行点 ⇒ 不做换向枚举。</summary>
    [Fact]
    public void GetPoint_FirstPhaseSucceeds()
    {
        int calls = 0;
        bool ok = ObjDummyCore.GetPoint(
            0, 10, 10, ObjDummyCore.DR_UP, 0,
            (x, y, dir, step) => { calls++; return (true, x, y - step); },
            (x, y) => true,
            out int nX, out int nY);
        Assert.True(ok);
        Assert.Equal(10, nX);
        Assert.Equal(8, nY);          // nRange = 2（step 先试 2）
        Assert.Equal(1, calls);       // 第一次就命中
    }

    /// <summary>第一个步长不可行 ⇒ 退到第二个步长（`nRange - 1`）。</summary>
    [Fact]
    public void GetPoint_FirstPhaseFallsBackToSecondStep()
    {
        var tried = new List<int>();
        bool ok = ObjDummyCore.GetPoint(
            0, 0, 0, ObjDummyCore.DR_UP, 0,
            (x, y, dir, step) => { tried.Add(step); return (true, 0, -step); },
            (x, y) => y == -1,            // 只有 step=1 可行
            out int nX, out int nY);
        Assert.True(ok);
        Assert.Equal(-1, nY);
        Assert.Equal(new[] { 2, 1 }, tried.ToArray());
    }

    /// <summary>当前朝向两段都不可行 ⇒ 进入换向枚举（顺时针，最多 8 轮）。</summary>
    [Fact]
    public void GetPoint_SecondPhaseTurnsAround()
    {
        int phases = 0;
        int maxTurns = 0;
        bool ok = ObjDummyCore.GetPoint(
            0, 0, 0, ObjDummyCore.DR_UP, 0,
            (x, y, dir, step) =>
            {
                if (dir == ObjDummyCore.DR_UP) { phases++; return (false, 0, 0); }
                return (true, dir, step);
            },
            (x, y) => true,
            out _, out _);
        Assert.True(ok);
        maxTurns = phases;
        Assert.Equal(2, maxTurns);      // 第一段试了 2 个步长
    }

    /// <summary>全部不可行 ⇒ 返回 false（`Result` 初值）。</summary>
    [Fact]
    public void GetPoint_AllBlockedReturnsFalse()
    {
        bool ok = ObjDummyCore.GetPoint(
            2, 0, 0, ObjDummyCore.DR_UP, 1,
            (x, y, dir, step) => (false, 0, 0),
            (x, y) => true,
            out _, out _);
        Assert.False(ok);
    }

    /// <summary>换向枚举只试 8 轮（`Inc(nC); if nC >= 8 then Break;`）。</summary>
    [Fact]
    public void GetPoint_TurnLoopCapsAtEight()
    {
        int getNextDirCalls = 0;
        bool ok = ObjDummyCore.GetPoint(
            0, 0, 0, ObjDummyCore.DR_UP, 0,
            (x, y, dir, step) => { getNextDirCalls++; return (false, 0, 0); },
            (x, y) => true,
            out _, out _);
        Assert.False(ok);
        // 第一段 2 次 + 换向 8 轮 × 2 步长 = 18
        Assert.Equal(2 + ObjDummyCore.GetPointMaxTurns * 2, getNextDirCalls);
    }

    // ===================== 六、CanAutoUseMagic =====================

    private static readonly Func<int, bool> AllowAll = static _ => true;
    private static readonly Func<int, uint> CdZero = static _ => 0u;

    private static AutoUseMagicState St(int id, uint now = 0u, uint lastShield = 0u,
        bool hasShield = false, bool fire = false, bool s42 = false, bool s66 = false,
        bool s113 = false, bool s115 = false, bool crs = false, bool half = false,
        Dictionary<int, uint>? ticks = null)
        => new(id, now, lastShield, hasShield, fire, s42, s66, s113, s115, crs, half, ticks);

    [Fact]
    public void CanAutoUseMagic_AllowUseMagicGateWins()
    {
        Assert.True(ObjDummyCore.AllowUseMagicGateWins());
        var d = ObjDummyCore.CanAutoUseMagic(St(40), static _ => false, CdZero);
        Assert.False(d.Result);
        Assert.Equal(AutoUseMagicEffect.None, d.Effect);
    }

    /// <summary>★ F-原：`SKILL_75` 分支永远返回 False（唯一赋值是 False）。</summary>
    [Fact]
    public void CanAutoUseMagic_Skill75AlwaysFalse()
    {
        Assert.True(ObjDummyCore.Skill75AlwaysReturnsFalse());
        var d = ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_75, now: 99999u, hasShield: true),
            AllowAll, CdZero);
        Assert.False(d.Result);
        Assert.Equal(AutoUseMagicEffect.OpenSuperShiled, d.Effect);
    }

    [Fact]
    public void CanAutoUseMagic_Skill75WithoutShieldHasNoEffect()
    {
        var d = ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_75, now: 99999u, hasShield: false),
            AllowAll, CdZero);
        Assert.False(d.Result);
        Assert.Equal(AutoUseMagicEffect.None, d.Effect);
    }

    [Fact]
    public void CanAutoUseMagic_Skill56()
    {
        var cd = static (int _) => 100u;
        Assert.False(ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_56, now: 99u), AllowAll, cd).Result);
        var d = ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_56, now: 100u,
            ticks: new Dictionary<int, uint> { [ObjDummyCore.SKILL_56] = 0u }), AllowAll, cd);
        Assert.True(d.Result);
        Assert.Equal(AutoUseMagicEffect.AllowSWordHit, d.Effect);
    }

    [Fact]
    public void CanAutoUseMagic_FireSword_EffectGatedButResultIsNot()
    {
        Assert.True(ObjDummyCore.EffectGatedButResultIsNot());
        var cd = static (int _) => 0u;
        // 未开 ⇒ 有效果
        var d1 = ObjDummyCore.CanAutoUseMagic(St(26, now: 1u, fire: false), AllowAll, cd);
        Assert.True(d1.Result);
        Assert.Equal(AutoUseMagicEffect.AllowFireHit, d1.Effect);
        // 已开 ⇒ 无效果但结果仍 True
        var d2 = ObjDummyCore.CanAutoUseMagic(St(26, now: 1u, fire: true), AllowAll, cd);
        Assert.True(d2.Result);
        Assert.Equal(AutoUseMagicEffect.None, d2.Effect);
    }

    [Theory]
    [InlineData(42, AutoUseMagicEffect.Allow42Hit)]
    [InlineData(66, AutoUseMagicEffect.Allow66Hit)]
    [InlineData(113, AutoUseMagicEffect.Allow113Hit)]
    [InlineData(115, AutoUseMagicEffect.Allow115Hit)]
    public void CanAutoUseMagic_GatedEffects(int id, AutoUseMagicEffect expected)
    {
        var d = ObjDummyCore.CanAutoUseMagic(St(id, now: 1u), AllowAll, CdZero);
        Assert.True(d.Result);
        Assert.Equal(expected, d.Effect);
    }

    [Fact]
    public void CanAutoUseMagic_Mootebo_UsesLiteralTenSeconds()
    {
        var cd = static (int _) => 0u;   // 即便 CD 为 0，MOOTEBO 仍走自己的 10 秒
        Assert.False(ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_MOOTEBO, now: 10000u), AllowAll, cd).Result);
        Assert.True(ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_MOOTEBO, now: 10001u), AllowAll, cd).Result);
    }

    [Fact]
    public void CanAutoUseMagic_Skill114And39UseGreaterThan()
    {
        var cd = static (int _) => 500u;
        // 114：恰好等于 ⇒ False
        Assert.False(ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_114, now: 500u), AllowAll, cd).Result);
        Assert.True(ObjDummyCore.CanAutoUseMagic(St(ObjDummyCore.SKILL_114, now: 501u), AllowAll, cd).Result);
        // 39：恰好等于 ⇒ False
        Assert.False(ObjDummyCore.CanAutoUseMagic(St(39, now: 500u, ticks: new Dictionary<int, uint> { [39] = 0u }),
            AllowAll, cd).Result);
        Assert.True(ObjDummyCore.CanAutoUseMagic(St(39, now: 501u, ticks: new Dictionary<int, uint> { [39] = 0u }),
            AllowAll, cd).Result);
    }

    /// <summary>★ 比较运算符不统一：`&gt;=`（6 个）与 `&gt;`（3 个）在"恰好等于 CD"时结果不同。</summary>
    [Fact]
    public void CanAutoUseMagic_ComparisonOperatorsAreInconsistent()
    {
        Assert.True(ObjDummyCore.ComparisonOperatorsAreInconsistent());
        Assert.Equal(6, ObjDummyCore.CanAutoUseMagicGeBranches);
        Assert.Equal(3, ObjDummyCore.CanAutoUseMagicGtBranches);
        Assert.True(ObjDummyCore.ExactCooldownDiffersBetweenBranches());
    }

    [Fact]
    public void CanAutoUseMagic_CrsAndHalfMoonHaveNoTimeCheck()
    {
        var d1 = ObjDummyCore.CanAutoUseMagic(St(40, now: 0u, crs: false), AllowAll, static _ => uint.MaxValue);
        Assert.True(d1.Result);
        Assert.Equal(AutoUseMagicEffect.SkillCrsOn, d1.Effect);

        var d2 = ObjDummyCore.CanAutoUseMagic(St(25, now: 0u, half: false), AllowAll, static _ => uint.MaxValue);
        Assert.True(d2.Result);
        Assert.Equal(AutoUseMagicEffect.HalfMoonOn, d2.Effect);

        // 已开 ⇒ 无副作用
        Assert.Equal(AutoUseMagicEffect.None,
            ObjDummyCore.CanAutoUseMagic(St(40, crs: true), AllowAll, CdZero).Effect);
        Assert.Equal(AutoUseMagicEffect.None,
            ObjDummyCore.CanAutoUseMagic(St(25, half: true), AllowAll, CdZero).Effect);
    }

    /// <summary>★ 四个分支完全不做时间判定；对照分支会做。</summary>
    [Fact]
    public void CanAutoUseMagic_FourBranchesHaveNoTimeCheck()
    {
        Assert.Equal(4, ObjDummyCore.CanAutoUseMagicNoTimeBranches);
        Assert.True(ObjDummyCore.FourBranchesHaveNoTimeCheck());
    }

    /// <summary>★ 未列举 ID 一律放行（`:1240` 兜底恒真）。</summary>
    [Fact]
    public void CanAutoUseMagic_UnknownSkillAlwaysAllowed()
    {
        Assert.True(ObjDummyCore.UnknownSkillAlwaysAllowed());
        Assert.Equal(14, ObjDummyCore.CanAutoUseMagicBranches);
    }

    [Fact]
    public void CanAutoUseMagic_CustomMagicRangeIsClosed()
    {
        Assert.True(ObjDummyCore.CustomMagicRangeIsClosed());
        Assert.True(ObjDummyCore.CanAutoUseMagic(
            St(ObjDummyCore.CUSTOM_MAGIC_START_ID + ObjDummyCore.CUSTOM_MAGIC_COUNT), AllowAll, CdZero).Result);
        Assert.True(ObjDummyCore.CanAutoUseMagic(
            St(ObjDummyCore.CUSTOM_MAGIC_START_ID + ObjDummyCore.CUSTOM_MAGIC_COUNT + 1), AllowAll, CdZero).Result);
    }

    [Fact]
    public void CanAutoUseMagic_NineGetMagicCdCallSites()
    {
        var cds = new List<int>();
        ObjDummyCore.CanAutoUseMagic(St(26, now: 5u), AllowAll, id => { cds.Add(id); return 0u; });
        Assert.Equal(new[] { ObjDummyCore.SKILL_FIRESWORD }, cds.ToArray());
        Assert.Equal(9, ObjDummyCore.GetMagicCdCallSites);
    }

    // ===================== 七、IsProperTarget =====================

    private static ProperTargetState Pt(bool inherited = true, bool isSelf = false, int race = 0,
        bool safe = false, bool masterField = false, bool masterProp = false,
        bool lastHiterSelf = false, int attackMode = ObjDummyCore.HAM_GUILD)
        => new(inherited, isSelf, race, safe, masterField, masterProp, 0, lastHiterSelf, attackMode);

    [Fact]
    public void IsProperTarget_InheritedGate()
    {
        Assert.False(ObjDummyCore.IsProperTarget(Pt(inherited: false)));
        Assert.True(ObjDummyCore.IsProperTarget(Pt(inherited: true)));
        Assert.True(ObjDummyCore.InheritedFalseShortCircuits());
    }

    [Fact]
    public void IsProperTarget_SelfIsVetoed()
    {
        Assert.False(ObjDummyCore.IsProperTarget(Pt(isSelf: true)));
        Assert.True(ObjDummyCore.IsProperTarget(Pt(isSelf: false)));
    }

    [Fact]
    public void IsProperTarget_PlayerInSafeZoneIsVetoed()
    {
        Assert.False(ObjDummyCore.IsProperTarget(Pt(race: ObjDummyCore.RC_PLAYOBJECT, safe: true)));
        Assert.True(ObjDummyCore.IsProperTarget(Pt(race: ObjDummyCore.RC_PLAYOBJECT, safe: false)));
        // 非玩家种族即使在安全区也不否决
        Assert.True(ObjDummyCore.IsProperTarget(Pt(race: 80, safe: true)));
    }

    /// <summary>★ F13：`m_Master = Self` **或** `Master = Self` 任一成立都否决（两种读法）。</summary>
    [Fact]
    public void IsProperTarget_MasterVetoedByEitherReadForm()
    {
        Assert.False(ObjDummyCore.IsProperTarget(Pt(masterField: true)));
        Assert.False(ObjDummyCore.IsProperTarget(Pt(masterProp: true)));
        Assert.True(ObjDummyCore.IsProperTarget(Pt(masterField: false, masterProp: false)));
    }

    [Fact]
    public void IsProperTarget_NpcAnimalRange()
        => Assert.True(ObjDummyCore.NpcAnimalRangeIsTenToFifty());

    [Fact]
    public void IsProperTarget_ArcherGuardDependsOnLastHiter()
        => Assert.True(ObjDummyCore.ArcherGuardVetoDependsOnLastHiter());

    [Fact]
    public void IsProperTarget_SiegeDoorAndWallVeto()
        => Assert.True(ObjDummyCore.SiegeVetoRequiresNonGuildMode());

    // ===================== 八、StartPickUpItem =====================

    private static PickUpItemState Pick(bool death = false, bool ghost = false, bool bag = true,
        bool hasSelected = false, bool onMap = true, bool selGhost = false,
        bool sameX = true, bool sameY = true, bool blocked = false, bool walkElapsed = false,
        bool gotoOk = false, bool pickOk = false, bool priority = false, bool fallback = false, int job = 0)
        => new()
        {
            Death = death, Ghost = ghost, EnoughBag = bag,
            HasSelected = hasSelected, SelectedStillOnMap = onMap, SelectedGhost = selGhost,
            SameX = sameX, SameY = sameY, BlockedByObject = blocked, WalkIntervalElapsed = walkElapsed,
            GotoNextOneOk = gotoOk, DoPickUpItemOk = pickOk,
            FoundPriority = priority, FoundFallback = fallback, Job = job,
        };

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void StartPickUpItem_GuardRejects(bool death, bool ghost, bool bag)
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(death: death, ghost: ghost, bag: bag, priority: true));
        Assert.False(d.Result);
        Assert.Equal(PickUpItemOutcome.RejectedByGuard, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_NoSelection_SearchesPriorityThenFallback()
    {
        var d1 = ObjDummyCore.StartPickUpItem(Pick(priority: true));
        Assert.True(d1.Result);
        Assert.Equal(PickUpItemOutcome.SearchNewTarget, d1.Outcome);

        var d2 = ObjDummyCore.StartPickUpItem(Pick(priority: false, fallback: true));
        Assert.True(d2.Result);
        Assert.Equal(PickUpItemOutcome.SearchNewTarget, d2.Outcome);

        var d3 = ObjDummyCore.StartPickUpItem(Pick(priority: false, fallback: false));
        Assert.False(d3.Result);
        Assert.Equal(PickUpItemOutcome.SearchNewTarget, d3.Outcome);
    }

    [Fact]
    public void StartPickUpItem_SelectionGoneFromMap_Searches()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, onMap: false, priority: true));
        Assert.True(d.Result);
        Assert.Equal(PickUpItemOutcome.SearchNewTarget, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_SelectionIsGhost_Searches()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, selGhost: true, priority: false, fallback: true));
        Assert.True(d.Result);
        Assert.Equal(PickUpItemOutcome.SearchNewTarget, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_DifferentPosition_RetargetsWhenBlocked()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, sameX: false, blocked: true));
        Assert.False(d.Result);
        Assert.Equal(PickUpItemOutcome.RetargetedToBlocker, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_DifferentPosition_GotoNextOneFailed()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, sameY: false,
            blocked: false, walkElapsed: true, gotoOk: false));
        Assert.False(d.Result);
        Assert.Equal(PickUpItemOutcome.GotoNextOneFailed, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_DifferentPosition_Walking()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, sameY: false,
            walkElapsed: true, gotoOk: true));
        Assert.True(d.Result);
        Assert.Equal(PickUpItemOutcome.Walking, d.Outcome);
    }

    /// <summary>★ 坐标不同 + 走位间隔**未到** ⇒ 落到搜索段（原文没有 else，:161）。</summary>
    [Fact]
    public void StartPickUpItem_DifferentPosition_ThrottledFallsThroughToSearch()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, sameY: false,
            blocked: false, walkElapsed: false, priority: true));
        Assert.True(d.Result);
        Assert.Equal(PickUpItemOutcome.SearchNewTarget, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_SamePosition_PickedUp()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, pickOk: true));
        Assert.True(d.Result);
        Assert.Equal(PickUpItemOutcome.PickedUp, d.Outcome);
    }

    [Fact]
    public void StartPickUpItem_SamePosition_PickFailed()
    {
        var d = ObjDummyCore.StartPickUpItem(Pick(hasSelected: true, pickOk: false, priority: true));
        Assert.False(d.Result);
        Assert.Equal(PickUpItemOutcome.PickUpFailed, d.Outcome);
    }

    /// <summary>★ 被注释掉的 `if` 之后紧跟 `begin`（配对缺失）—— 原文如此。</summary>
    [Fact]
    public void StartPickUpItem_UnpairedBeginAfterCommentedIf()
        => Assert.True(ObjDummyCore.UnpairedBeginAfterCommentedIf());

    [Fact]
    public void WalkTimeByJob_UsesConfigWithFallback()
    {
        int saveW = M2Config.dwDummyWarrorWalkTime, saveZ = M2Config.dwDummyWizardWalkTime,
            saveT = M2Config.dwDummyTaoistWalkTime;
        try
        {
            M2Config.dwDummyWarrorWalkTime = 111;
            M2Config.dwDummyWizardWalkTime = 222;
            M2Config.dwDummyTaoistWalkTime = 333;
            Assert.Equal(111, ObjDummyCore.WalkTimeByJob(0));
            Assert.Equal(222, ObjDummyCore.WalkTimeByJob(1));
            Assert.Equal(333, ObjDummyCore.WalkTimeByJob(2));
            Assert.Equal(500, ObjDummyCore.WalkTimeByJob(3));    // 兜底
            Assert.Equal(500, ObjDummyCore.WalkTimeByJob(-1));
        }
        finally
        {
            M2Config.dwDummyWarrorWalkTime = saveW;
            M2Config.dwDummyWizardWalkTime = saveZ;
            M2Config.dwDummyTaoistWalkTime = saveT;
        }
    }

    [Fact]
    public void AttackTimeByJob_UsesConfig()
    {
        int saveW = M2Config.dwDummyWarrorAttackTime, saveZ = M2Config.dwDummyWizardAttackTime,
            saveT = M2Config.dwDummyTaoistAttackTime;
        try
        {
            M2Config.dwDummyWarrorAttackTime = 1201;
            M2Config.dwDummyWizardAttackTime = 1202;
            M2Config.dwDummyTaoistAttackTime = 1203;
            Assert.Equal(1201, ObjDummyCore.AttackTimeByJob(0));
            Assert.Equal(1202, ObjDummyCore.AttackTimeByJob(1));
            Assert.Equal(1203, ObjDummyCore.AttackTimeByJob(2));
            Assert.Equal(0, ObjDummyCore.AttackTimeByJob(3));      // 原文 case 无 else
        }
        finally
        {
            M2Config.dwDummyWarrorAttackTime = saveW;
            M2Config.dwDummyWizardAttackTime = saveZ;
            M2Config.dwDummyTaoistAttackTime = saveT;
        }
    }

    // ===================== 九、F7 / D-P9-03：AttackTime 的两种读法 =====================

    [Fact]
    public void AttackTime_SignedReadingIsClamped()
    {
        Assert.True(ObjDummyCore.AttackTimeIsClampedNotWrapped());
        Assert.Equal(900, ObjDummyCore.DummyAttackTimeSigned(1200, 30, 10));
        Assert.Equal(0, ObjDummyCore.DummyAttackTimeSigned(1200, 30, 100));
        Assert.Equal(0, ObjDummyCore.DummyAttackTimeSigned(1200, 30, 40));   // 恰好相等 ⇒ 0
    }

    [Fact]
    public void AttackTime_UnsignedReadingWouldWrap()
    {
        Assert.True(ObjDummyCore.UnsignedReadingWouldNeverAttack());
        long wrapped = ObjDummyCore.DummyAttackTimeIfUnsigned(1200, 30, 100);
        Assert.Equal(4294965496L, wrapped);
        // 两读法在此处差距是"0 vs ~4.29e9"
        Assert.NotEqual((long)ObjDummyCore.DummyAttackTimeSigned(1200, 30, 100), wrapped);
    }

    // ===================== 十、Wondering =====================

    private static WonderingState Wo(bool start = true, bool hasTarget = false, bool canMove = true,
        bool autoPick = false, bool walkElapsed = false, int job = 0, int currX = 0, int currY = 0)
        => new()
        {
            Start = start, HasTarget = hasTarget, CanMove = canMove,
            AutoPickUpSucceeded = autoPick, WalkIntervalElapsed = walkElapsed,
            Job = job, CurrX = currX, CurrY = currY,
        };

    [Fact]
    public void Wondering_GuardNeedsAllSeven()
    {
        Assert.True(ObjDummyCore.WonderingGuard(Wo()));
        Assert.False(ObjDummyCore.WonderingGuard(Wo(start: false)));
        Assert.False(ObjDummyCore.WonderingGuard(Wo(hasTarget: true)));
        Assert.False(ObjDummyCore.WonderingGuard(Wo(canMove: false)));

        foreach (var mutate in new Action<WonderingState>[]
                 {
                     s => s.Ghost = true,
                     s => s.Death = true,
                     s => s.FixedHideMode = true,
                     s => s.StoneMode = true,
                     s => s.ShopStall = true,
                 })
        {
            var s = Wo();
            mutate(s);
            Assert.False(ObjDummyCore.WonderingGuard(s));
        }
    }

    [Fact]
    public void Wondering_AutoPickUpShortCircuits()
    {
        var d = ObjDummyCore.Wondering(Wo(autoPick: true, walkElapsed: true));
        Assert.Equal(WonderingOutcome.AutoPickUp, d.Outcome);
    }

    [Fact]
    public void Wondering_Throttled()
    {
        var d = ObjDummyCore.Wondering(Wo(walkElapsed: false));
        Assert.Equal(WonderingOutcome.Throttled, d.Outcome);
    }

    [Fact]
    public void Wondering_FarPointTriggersRunToTarget()
    {
        var s = Wo(walkElapsed: true, currX: 10, currY: 10);
        s.Points[0] = (true, 10, 10);      // 同点
        s.Points[1] = (true, 13, 10);      // dx = 3 > 2 ⇒ 命中
        s.Points[2] = (true, 20, 20);
        var d = ObjDummyCore.Wondering(s);
        Assert.Equal(WonderingOutcome.RunToTarget, d.Outcome);
        Assert.Equal(13, d.TargetX);
        Assert.Equal(1, d.Index);
    }

    [Fact]
    public void Wondering_AllNearPointsDoNothing()
    {
        var s = Wo(walkElapsed: true, currX: 10, currY: 10);
        s.Points[0] = (true, 11, 10);
        s.Points[1] = (true, 10, 12);
        s.Points[2] = (true, 12, 12);
        var d = ObjDummyCore.Wondering(s);
        Assert.Equal(WonderingOutcome.NoFarPoint, d.Outcome);
    }

    [Fact]
    public void Wondering_PointsNotFoundAreSkipped()
    {
        var s = Wo(walkElapsed: true, currX: 10, currY: 10);
        s.Points[0] = (false, 99, 99);
        s.Points[1] = (false, 99, 99);
        s.Points[2] = (true, 10, 10);
        var d = ObjDummyCore.Wondering(s);
        Assert.Equal(WonderingOutcome.NoFarPoint, d.Outcome);
    }

    [Fact]
    public void FarPointThresholdIsLiteralTwo()
        => Assert.True(ObjDummyCore.FarPointThresholdIsLiteralTwo());

    // ===================== 十一、Run 的关键判定 =====================

    [Fact]
    public void RunMainGuard_And_ElseGuard()
    {
        Assert.True(ObjDummyCore.RunMainGuard(true, false, false, false, false, false, true));
        Assert.False(ObjDummyCore.RunMainGuard(false, false, false, false, false, false, true));
        Assert.False(ObjDummyCore.RunMainGuard(true, false, false, false, false, false, false));

        Assert.True(ObjDummyCore.RunElseGuard(false, false, false, false, false));
        Assert.False(ObjDummyCore.RunElseGuard(true, false, false, false, false));
    }

    /// <summary>★★ F8：`m_boStart = False` 或 `CanMove = False` 时，自动练功**照样跑**。</summary>
    [Fact]
    public void Flaw8_AutoMagicRunsEvenWhenStoppedOrCannotMove()
        => Assert.True(ObjDummyCore.AutoMagicRunsEvenWhenStoppedOrCannotMove());

    [Fact]
    public void AutoGotoGuard_And_Phases()
    {
        Assert.True(ObjDummyCore.AutoGotoGuard(false, false, false, false, false, true));
        Assert.False(ObjDummyCore.AutoGotoGuard(false, false, false, false, false, false));

        Assert.True(ObjDummyCore.AutoGotoWalkPhase(0, 100, 100));
        Assert.True(ObjDummyCore.AutoGotoWalkPhase(1, 1, 1));
        Assert.False(ObjDummyCore.AutoGotoWalkPhase(1, 2, 1));
        Assert.False(ObjDummyCore.AutoGotoWalkPhase(1, 1, 2));
    }

    [Fact]
    public void IsBackAndForth_ComparesOppositeToNextDir()
    {
        Assert.True(ObjDummyCore.IsBackAndForth(4, 4));
        Assert.False(ObjDummyCore.IsBackAndForth(4, 5));
    }

    [Fact]
    public void AutoGotoRunsInsteadOfWalks_ThresholdIsOne()
    {
        Assert.False(ObjDummyCore.AutoGotoRunsInsteadOfWalks(1, 1));
        Assert.True(ObjDummyCore.AutoGotoRunsInsteadOfWalks(2, 0));
        Assert.True(ObjDummyCore.AutoGotoRunsInsteadOfWalks(0, -2));
    }

    [Fact]
    public void RunAttackGuard_And_Range()
    {
        Assert.False(ObjDummyCore.RunAttackGuard(0, 0, true));       // rate = 0 ⇒ Random(0) 会抛 ⇒ 必须挡住
        Assert.False(ObjDummyCore.RunAttackGuard(5, 1, true));
        Assert.False(ObjDummyCore.RunAttackGuard(5, 0, false));      // 走位间隔未到
        Assert.True(ObjDummyCore.RunAttackGuard(5, 0, true));
        Assert.Equal(1, ObjDummyCore.RunAttackRange(0));
        Assert.Equal(5, ObjDummyCore.RunAttackRange(1));
        Assert.Equal(5, ObjDummyCore.RunAttackRange(2));
    }

    /// <summary>★★ F6：`(btDir - 1) mod 8` 在 `btDir = 0` 时给出 **-1**（Delphi `mod` 取被除数符号）。</summary>
    [Fact]
    public void Flaw6_MinusOneDirectionIsReachable()
    {
        Assert.True(ObjDummyCore.MinusOneDirectionIsReachable());
        Assert.Equal(-1, ObjDummyCore.JitterDirection(0, plusOne: false));
        Assert.Equal(1, ObjDummyCore.JitterDirection(0, plusOne: true));
        Assert.Equal(6, ObjDummyCore.JitterDirection(7, plusOne: false));
        Assert.Equal(0, ObjDummyCore.JitterDirection(7, plusOne: true));
    }

    [Fact]
    public void Flaw6_PlusOneAndPlusCountAreAlwaysValid()
    {
        Assert.True(ObjDummyCore.PlusOneDirectionIsAlwaysValid());
        Assert.True(ObjDummyCore.PlusCountDirectionIsAlwaysValid());
    }

    [Fact]
    public void WizardMovePhase_And_Range()
    {
        Assert.True(ObjDummyCore.WizardMovePhase(true, true));
        Assert.False(ObjDummyCore.WizardMovePhase(true, false));
        Assert.False(ObjDummyCore.WizardMovePhase(false, true));

        Assert.True(ObjDummyCore.WizardOutOfRange(13, 0, 12));
        Assert.False(ObjDummyCore.WizardOutOfRange(12, 12, 12));
        Assert.Equal(11, ObjDummyCore.WizardStep(12));
    }

    [Fact]
    public void WizardFireWallHunt()
    {
        Assert.False(ObjDummyCore.WizardFireWallHuntEntry(ObjDummyCore.RC_PLAYOBJECT, 0));
        Assert.False(ObjDummyCore.WizardFireWallHuntEntry(80, 1));
        Assert.True(ObjDummyCore.WizardFireWallHuntEntry(80, 0));
        Assert.Equal(20, ObjDummyCore.FireWallHuntRandomBound);
    }

    [Fact]
    public void IsOwnFireEvent()
    {
        Assert.True(ObjDummyCore.IsOwnFireEvent(false, ObjDummyCore.ET_FIRE, true));
        Assert.False(ObjDummyCore.IsOwnFireEvent(true, ObjDummyCore.ET_FIRE, true));   // GameEvent = nil
        Assert.False(ObjDummyCore.IsOwnFireEvent(false, 6, true));
        Assert.False(ObjDummyCore.IsOwnFireEvent(false, ObjDummyCore.ET_FIRE, false));
    }

    [Fact]
    public void TaoistMovePhase_HasNoFireWallBranch()
    {
        Assert.True(ObjDummyCore.TaoistMovePhase(true, true, 13, 0, 12));
        Assert.False(ObjDummyCore.TaoistMovePhase(true, true, 12, 12, 12));
        Assert.False(ObjDummyCore.TaoistMovePhase(true, false, 13, 0, 12));
        Assert.False(ObjDummyCore.TaoistMovePhase(false, true, 13, 0, 12));
    }

    /// <summary>★ F12：道士"没事找事跑"的抑制在 `Random(4) = 0` 时**不成立** ⇒ 1/4 概率仍然跑。</summary>
    [Fact]
    public void Flaw12_TaoistStillRunsOneInFour()
    {
        Assert.True(ObjDummyCore.TaoistStillRunsOneInFour());
        Assert.False(ObjDummyCore.TaoistIdleRunSuppressed(2, 0, 0, 12, 0));
        Assert.True(ObjDummyCore.TaoistIdleRunSuppressed(2, 0, 0, 12, 1));
        Assert.False(ObjDummyCore.TaoistIdleRunSuppressed(1, 0, 0, 12, 1));   // 非法师
        Assert.False(ObjDummyCore.TaoistIdleRunSuppressed(2, 13, 0, 12, 1));  // 距离超了
    }

    // ===================== 十二、英雄合击 =====================

    [Fact]
    public void HeroJointAttackCondition_AllEleven()
    {
        bool Ok(bool joint = true, bool hero = true, int angry = 100, int max = 100,
                bool wear = true, bool bujuk = true, bool group = true,
                int tdx = 1, int tdy = 1, int hdx = 1, int hdy = 1, int rnd = 0)
            => ObjDummyCore.HeroJointAttackCondition(joint, hero, angry, max, wear, bujuk, group, tdx, tdy, hdx, hdy, rnd);

        Assert.True(Ok());
        Assert.False(Ok(joint: false));
        Assert.False(Ok(hero: false));
        Assert.False(Ok(angry: 99));
        Assert.False(Ok(wear: false));
        Assert.False(Ok(bujuk: false));
        Assert.False(Ok(group: false));
        Assert.False(Ok(tdx: 11));
        Assert.False(Ok(tdy: -11));
        Assert.False(Ok(hdx: 11));
        Assert.False(Ok(hdy: 11));
        Assert.False(Ok(rnd: 1));
        Assert.True(Ok(tdx: 10, tdy: -10, hdx: 10, hdy: -10));   // 闭区间
    }

    [Fact]
    public void HeroJointFlyNeeded()
    {
        Assert.True(ObjDummyCore.HeroJointFlyNeeded(true, 3, 0));
        Assert.False(ObjDummyCore.HeroJointFlyNeeded(true, 2, 2));
        Assert.False(ObjDummyCore.HeroJointFlyNeeded(false, 99, 99));
    }

    // ===================== 十三、自动练功的到期判定 =====================

    [Fact]
    public void AutoUseMagicDue_Boundary()
    {
        // m_dwAutoUseMagicTime = 0 ⇒ 门限 0 ⇒ 只要走过就满足（>=）
        Assert.True(ObjDummyCore.AutoUseMagicDue(true, 26, 100u, 100u, 0u));
        // 3 秒
        Assert.False(ObjDummyCore.AutoUseMagicDue(true, 26, 2999u, 0u, 3u));
        Assert.True(ObjDummyCore.AutoUseMagicDue(true, 26, 3000u, 0u, 3u));
        // 开关/ID 守卫
        Assert.False(ObjDummyCore.AutoUseMagicDue(false, 26, 99999u, 0u, 3u));
        Assert.False(ObjDummyCore.AutoUseMagicDue(true, 0, 99999u, 0u, 3u));
    }

    // ===================== 十四、Create / Start / Stop / Walk / Initialize / Ask =====================

    [Fact]
    public void CreateAssignments_CountAndKeyValues()
    {
        Assert.True(ObjDummyCore.CreateHasFifteenAssignments());
        Assert.Contains(("m_nSocket", "0"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_nGSocketIdx", "-1"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_nGateIdx", "-1"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_boDummyObject", "True"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_boLoginNoticeOK", "True"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_nSoftVersionDate", "CLIENT_VERSION_NUMBER"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_nAutoGotoX", "-1"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_nAutoGotoY", "-1"), ObjDummyCore.CreateAssignments);
        Assert.Contains(("m_boStart", "False"), ObjDummyCore.CreateAssignments);
    }

    [Fact]
    public void OnSpaceMove_OnlyResetsAutoGoto()
    {
        Assert.Equal((-1, -1), ObjDummyCore.OnSpaceMoveResult());
        Assert.True(ObjDummyCore.OnSpaceMoveOnlyResetsAutoGoto());
    }

    [Fact]
    public void Start_And_Stop()
    {
        // 已启动 ⇒ 什么都不做
        Assert.Equal((true, (string?)null), ObjDummyCore.Start(true, true));
        // 未启动 + 有 FunctionNPC ⇒ 置真并跳标签
        Assert.Equal((true, "@DummyStart"), ObjDummyCore.Start(false, true));
        // 未启动 + 无 FunctionNPC ⇒ 只置真
        Assert.Equal((true, (string?)null), ObjDummyCore.Start(false, false));

        Assert.Equal((false, (string?)null), ObjDummyCore.Stop(false, true));
        Assert.Equal((false, "@DummyStop"), ObjDummyCore.Stop(true, true));
        Assert.Equal((false, (string?)null), ObjDummyCore.Stop(true, false));
    }

    [Fact]
    public void WalkScriptGoto_MatchesRunWalk()
    {
        Assert.Equal(("@Run", true, false), ObjDummyCore.WalkScriptGoto(ObjDummyCore.RM_RUN, true));
        Assert.Equal(("@Walk", false, true), ObjDummyCore.WalkScriptGoto(ObjDummyCore.RM_WALK, true));
        Assert.Equal(((string?)null, false, false), ObjDummyCore.WalkScriptGoto(12345, true));
        // 无 FunctionNPC ⇒ 不跳标签，但依旧走 inherited Walk
        Assert.Equal(((string?)null, true, false), ObjDummyCore.WalkScriptGoto(ObjDummyCore.RM_RUN, false));
        Assert.True(ObjDummyCore.WalkNestsSecondCheckInsideElse());
        // RM_RUN / RM_WALK 不能是同一个值
        Assert.NotEqual(ObjDummyCore.RM_RUN, ObjDummyCore.RM_WALK);
    }

    [Fact]
    public void RunToNext_DegradesToWalk()
    {
        Assert.True(ObjDummyCore.RunToNextDegradesToWalk(true, false));
        Assert.True(ObjDummyCore.RunToNextDegradesToWalk(false, true));
        Assert.False(ObjDummyCore.RunToNextDegradesToWalk(false, false));
        Assert.True(ObjDummyCore.MoveTickRefreshedEvenOnFailure());
    }

    [Fact]
    public void Initialize_StepsAndMagicLevelClamp()
    {
        Assert.True(ObjDummyCore.InitializeHasSixSteps());
        Assert.True(ObjDummyCore.OverLevelIsZeroedNotClamped());
        Assert.Equal((byte)0, ObjDummyCore.ClampUserMagicLevel(9, 3));
        Assert.Equal((byte)3, ObjDummyCore.ClampUserMagicLevel(3, 3));
        Assert.Equal((byte)1, ObjDummyCore.ClampUserMagicLevel(1, 3));
    }

    [Fact]
    public void Ask_OverwritesFirstObjectWithTick()
        => Assert.True(ObjDummyCore.AskOverwritesFirstObjectWithTick());

    // ===================== 十五、接缝 =====================

    [Fact]
    public void Seam_FunctionNpcAndPluginDefaultsAreNull()
    {
        try
        {
            ObjDummySeam.ResetDefaults();
            Assert.Null(ObjDummySeam.g_FunctionNPC);
            Assert.Null(ObjDummySeam.g_PluginManager);
            Assert.NotNull(ObjDummySeam.Runtime);
            Assert.False(ObjDummySeam.Runtime.boHeroJointAttack);
            Assert.Equal(0, ObjDummySeam.Runtime.m_nRunAttackRate);
            Assert.False(ObjDummySeam.Runtime.m_boAutoPickUpItem);
        }
        finally
        {
            ObjDummySeam.ResetDefaults();
        }
    }

    [Fact]
    public void Seam_ForwardsTickToSweepSeam()
    {
        var saved = GXX.M2Server.Sweep.SweepSeam.MyGetTickCount;
        try
        {
            ObjDummySeam.MyGetTickCount = () => 777u;
            Assert.Equal(777u, GXX.M2Server.Sweep.SweepSeam.MyGetTickCount());
            Assert.Equal(777u, ObjDummySeam.MyGetTickCount());
        }
        finally
        {
            ObjDummySeam.MyGetTickCount = saved;
        }
    }

    [Fact]
    public void Seam_SortDefaultsToNoOp()
    {
        try
        {
            ObjDummySortSeam.ResetDefaults();
            var list = new GXX.Core.Util.TStringList();
            list.Add("b");
            list.Add("a");
            ObjDummySortSeam.CustomSort(list);
            Assert.Equal("b", list[0]);      // 默认实现不动顺序
        }
        finally
        {
            ObjDummySortSeam.ResetDefaults();
        }
    }
}
