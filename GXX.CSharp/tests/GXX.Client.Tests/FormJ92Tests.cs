using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J92：Actor.pas TActor.CalcActorFrame 的 **race 156 自定义怪分支**（3491-3642）1:1 测试。
/// </summary>
public sealed class CustomMonsterFrameCalcTests
{
    private const int SM_TURN = TActorCore.SM_TURN;
    private const int SM_WALK = TActorCore.SM_WALK;
    private const int SM_RUSH = TActorCore.SM_RUSH;
    private const int SM_RUSHKUNG = TActorCore.SM_RUSHKUNG;
    private const int SM_BACKSTEP = TActorCore.SM_BACKSTEP;
    private const int SM_HIT = TActorCore.SM_HIT;
    private const int SM_STRUCK = TActorCore.SM_STRUCK;
    private const int SM_DEATH = TActorCore.SM_DEATH;
    private const int SM_NOWDEATH = TActorCore.SM_NOWDEATH;
    private const int SM_SKELETON = TActorCore.SM_SKELETON;
    private const int SM_DIGUP = TActorCore.SM_DIGUP;
    private const int SM_LIGHTINGEX = TActorCore.SM_LIGHTINGEX;

    private static TMonsterClientAction Act(
        int startIndex = 100, ushort playCount = 4, ushort emptyCount = 1,
        ushort playTime = 120, byte calcDir = 1)
        => new()
        {
            StartIndex = (short)startIndex,
            PlayCount = playCount,
            EmptyCount = emptyCount,
            PlayTime = playTime,
            CalcDir = calcDir,
        };

    /// <summary>按类型返回不同动作，便于区分分支取错了哪个动作。</summary>
    private static Func<TMonsterClientActionType, TMonsterClientAction> Table(
        TMonsterClientAction? stand = null, TMonsterClientAction? walk = null,
        TMonsterClientAction? defAttack = null, TMonsterClientAction? struck = null,
        TMonsterClientAction? die = null, TMonsterClientAction? stoneRevive = null)
    {
        var s = stand ?? Act(100);
        var w = walk ?? Act(200);
        var da = defAttack ?? Act(300);
        var st = struck ?? Act(400);
        var d = die ?? Act(500);
        var r = stoneRevive ?? Act(600);

        return t => t switch
        {
            TMonsterClientActionType.matStand => s,
            TMonsterClientActionType.matWalk => w,
            TMonsterClientActionType.matDefAttack => da,
            TMonsterClientActionType.matStruck => st,
            TMonsterClientActionType.matDie => d,
            TMonsterClientActionType.matStoneRevive => r,
            _ => Act(900),
        };
    }

    private static CustomMonsterFrameCalc.FramePlan? Run(
        int action, int btDir = 0, int btStep = 1, bool stoneMode = false,
        int dwStruckFrameTime = 0,
        Func<TMonsterClientActionType, TMonsterClientAction>? table = null)
    {
        return CustomMonsterFrameCalc.Plan(
            action, btDir, btStep, stoneMode,
            dwStruckFrameTime, table ?? Table(), out _);
    }

    // ===================== 进入条件 =====================

    [Fact]
    public void BranchRequiresRace156()
    {
        Assert.True(CustomMonsterFrameCalc.IsCustomMonsterBranch(156, 0));
        Assert.False(CustomMonsterFrameCalc.IsCustomMonsterBranch(155, 0));
        Assert.False(CustomMonsterFrameCalc.IsCustomMonsterBranch(157, 0));
    }

    [Fact]
    public void BranchRequiresChangeApprNonNegative()
    {
        // `>= 0`：0 是有效外观号
        Assert.True(CustomMonsterFrameCalc.IsCustomMonsterBranch(156, 0));
        Assert.True(CustomMonsterFrameCalc.IsCustomMonsterBranch(156, 1));
        Assert.False(CustomMonsterFrameCalc.IsCustomMonsterBranch(156, -1));
    }

    [Fact]
    public void CustomMonsterRaceConstant()
    {
        Assert.Equal(156, CustomMonsterFrameCalc.CustomMonsterRace);
    }

    // ===================== TempDir 模式 =====================

    [Fact]
    public void TempDirUsesOwnDirWhenCalcDir()
    {
        Assert.Equal(3, CustomMonsterFrameCalc.TempDir(calcDir: true, btDir: 3));
    }

    [Fact]
    public void TempDirIsZeroWhenNotCalcDir()
    {
        Assert.Equal(0, CustomMonsterFrameCalc.TempDir(calcDir: false, btDir: 3));
    }

    [Fact]
    public void BaseFrameStridesByPlayCountPlusEmptyCount()
    {
        var a = Act(startIndex: 100, playCount: 4, emptyCount: 1);
        Assert.Equal(100 + 3 * (4 + 1), CustomMonsterFrameCalc.BaseFrame(a, 3));
    }

    [Fact]
    public void BaseFrameIgnoresDirWhenCalcDirFalse()
    {
        var a = Act(startIndex: 100, playCount: 4, emptyCount: 1, calcDir: 0);
        Assert.Equal(100, CustomMonsterFrameCalc.BaseFrame(a, 5));
    }

    // ===================== 站立 / 转身 =====================

    [Fact]
    public void StandUsesStandAction()
    {
        var p = Run(0, btDir: 0, table: Table(stand: Act(100)));

        Assert.NotNull(p);
        Assert.Equal("Stand", p!.Kind);
        Assert.Equal(100, p.StartFrame);
        Assert.Equal(103, p.EndFrame);                 // + PlayCount - 1
    }

    [Fact]
    public void TurnUsesStandActionToo()
    {
        // 3506：`0, SM_TURN` 共用一支
        var p = Run(SM_TURN, table: Table(stand: Act(100)));
        Assert.Equal("Stand", p!.Kind);
    }

    [Fact]
    public void StandWritesDefFrameCount()
    {
        var p = Run(0, table: Table(stand: Act(100, playCount: 7)));
        Assert.Equal(7, p!.DefFrameCount);
    }

    [Fact]
    public void StandStoneModeUsesReviveWithSingleFrame()
    {
        // 3507-3519：StartFrame = EndFrame
        var p = Run(0, btDir: 2, stoneMode: true, table: Table(stoneRevive: Act(600, playCount: 5)));

        Assert.NotNull(p);
        Assert.Equal("StandStoneRevive", p!.Kind);
        Assert.Equal(600 + 2 * (5 + 1), p.StartFrame);
        Assert.Equal(p.StartFrame, p.EndFrame);
    }

    [Fact]
    public void StoneModeAlsoAppliesToTurn()
    {
        var p = Run(SM_TURN, stoneMode: true, table: Table(stoneRevive: Act(600)));
        Assert.Equal("StandStoneRevive", p!.Kind);
    }

    [Fact]
    public void StoneModeIgnoredForNonStandActions()
    {
        var p = Run(SM_WALK, stoneMode: true, table: Table(walk: Act(200), stoneRevive: Act(600)));
        Assert.Equal("Walk", p!.Kind);
        Assert.Equal(200, p.StartFrame);
    }

    // ===================== 行走族 =====================

    [Theory]
    [InlineData(SM_WALK)]
    [InlineData(SM_RUSH)]
    [InlineData(SM_RUSHKUNG)]
    [InlineData(SM_BACKSTEP)]
    public void WalkFamilyUsesWalkAction(int action)
    {
        var p = Run(action, table: Table(walk: Act(200)));
        Assert.Equal("Walk", p!.Kind);
        Assert.Equal(200, p.StartFrame);
        Assert.Equal(203, p.EndFrame);
    }

    [Fact]
    public void WalkSetsMoveStepOneForNonBackstep()
    {
        CustomMonsterFrameCalc.Plan(SM_WALK, 0, 5, false, 0,
            Table(), out int step);

        Assert.Equal(1, step);
    }

    [Fact]
    public void BackstepUsesBtStep()
    {
        CustomMonsterFrameCalc.Plan(SM_BACKSTEP, 0, 5, false, 0,
            Table(), out int step);

        Assert.Equal(5, step);
    }

    [Fact]
    public void WalkDoesNotWriteDefFrameCount()
    {
        // 3537-3559 未写 m_nDefFrameCount
        var p = Run(SM_WALK, table: Table(walk: Act(200, playCount: 9)));
        Assert.Equal(0, p!.DefFrameCount);
    }

    [Fact]
    public void WalkMaxTickIsZeroPlaceholder()
    {
        // 3549 读的是通用动作表 HA.ActWalk.usetick（自定义怪配置无此字段）
        var p = Run(SM_WALK, table: Table(walk: Act(200)));
        Assert.Equal(CustomMonsterFrameCalc.CustomMonsterWalkUseTick, p!.MaxTick);
    }

    // ===================== 出土 =====================

    [Fact]
    public void DigUpUsesReviveAction()
    {
        var p = Run(SM_DIGUP, table: Table(stoneRevive: Act(600)));

        Assert.Equal("DigUp", p!.Kind);
        Assert.Equal(600, p.StartFrame);
        Assert.Equal(603, p.EndFrame);
    }

    [Fact]
    public void DigUpResetsState()
    {
        // 3576：m_nState := 0
        var p = Run(SM_DIGUP, table: Table(stoneRevive: Act(600)));
        Assert.True(p!.ResetState);
    }

    [Fact]
    public void DigUpWritesDefFrameCount()
    {
        var p = Run(SM_DIGUP, table: Table(stoneRevive: Act(600, playCount: 6)));
        Assert.Equal(6, p!.DefFrameCount);
    }

    [Fact]
    public void StandDoesNotResetState()
    {
        Assert.False(Run(0)!.ResetState);
    }

    // ===================== 空实现 =====================

    [Fact]
    public void LightingExIsEmptyInCustomBranch()
    {
        // 3580-3582 是空 begin/end
        Assert.Null(Run(SM_LIGHTINGEX));
    }

    [Fact]
    public void SkeletonIsEmptyInCustomBranch()
    {
        // 3636-3638 是空 begin/end
        Assert.Null(Run(SM_SKELETON));
    }

    // ===================== 攻击 / 受击 =====================

    [Fact]
    public void HitUsesDefAttackAction()
    {
        // 3584：matDefAttack（注意是 DefAttack 而非 matAttack1）
        var p = Run(SM_HIT, table: Table(defAttack: Act(300)));

        Assert.Equal("Hit", p!.Kind);
        Assert.Equal(300, p.StartFrame);
        Assert.Equal(303, p.EndFrame);
    }

    [Fact]
    public void HitSetsWarModeTime()
    {
        Assert.True(Run(SM_HIT)!.WarModeTime);
    }

    [Fact]
    public void WalkDoesNotSetWarModeTime()
    {
        Assert.False(Run(SM_WALK)!.WarModeTime);
    }

    [Fact]
    public void StruckUsesStruckAction()
    {
        var p = Run(SM_STRUCK, table: Table(struck: Act(400)));
        Assert.Equal(400, p!.StartFrame);
    }

    [Fact]
    public void StruckUsesDwStruckFrameTime()
    {
        // 3606：m_dwFrameTime := m_dwStruckFrameTime（覆盖动作自身的 PlayTime）
        var p = Run(SM_STRUCK, dwStruckFrameTime: 999, table: Table(struck: Act(400, playTime: 120)));

        Assert.Equal(999, p!.FrameTime);
        Assert.NotEqual(120, p.FrameTime);
    }

    [Fact]
    public void StruckResetsStruckCounter()
    {
        // 3610：m_CustomMagicStatusEffect.m_nStruck := 0
        Assert.True(Run(SM_STRUCK)!.ResetStruck);
    }

    [Fact]
    public void StruckDoesNotSetWarModeTime()
    {
        // 与 SM_HIT 不同，受击支路没有 m_dwWarModeTime
        Assert.False(Run(SM_STRUCK)!.WarModeTime);
    }

    // ===================== 死亡 =====================

    [Fact]
    public void DeathUsesDieActionAndPinsLastFrame()
    {
        // 3619-3620：+ PlayCount - 1 且 EndFrame = StartFrame
        var p = Run(SM_DEATH, btDir: 0, table: Table(die: Act(500, playCount: 8)));

        Assert.Equal("Death", p!.Kind);
        Assert.Equal(500 + 8 - 1, p.StartFrame);
        Assert.Equal(p.StartFrame, p.EndFrame);
    }

    [Fact]
    public void NowDeathPlaysFromFirstFrame()
    {
        var p = Run(SM_NOWDEATH, btDir: 0, table: Table(die: Act(500, playCount: 8)));

        Assert.Equal("NowDeath", p!.Kind);
        Assert.Equal(500, p.StartFrame);
        Assert.Equal(507, p.EndFrame);
    }

    [Fact]
    public void DeathAndNowDeathDifferInRange()
    {
        var death = Run(SM_DEATH, table: Table(die: Act(500, playCount: 8)));
        var now = Run(SM_NOWDEATH, table: Table(die: Act(500, playCount: 8)));

        Assert.NotEqual(death!.StartFrame, now!.StartFrame);
        Assert.Equal(death.EndFrame, now.EndFrame);    // 末帧相同
    }

    [Fact]
    public void DeathDoesNotSetWarModeTimeOrDefFrameCount()
    {
        var p = Run(SM_DEATH);
        Assert.False(p!.WarModeTime);
        Assert.Equal(0, p.DefFrameCount);
    }

    // ===================== 方向步进贯穿 =====================

    [Fact]
    public void AllBranchesStrideByDirWhenCalcDir()
    {
        foreach (int action in new[] { 0, SM_TURN, SM_WALK, SM_DIGUP, SM_HIT, SM_STRUCK, SM_DEATH, SM_NOWDEATH })
        {
            var d0 = Run(action, btDir: 0, table: Table());
            var d3 = Run(action, btDir: 3, table: Table());

            Assert.NotNull(d0);
            Assert.NotNull(d3);

            int stride = 3 * (4 + 1);              // 默认 Act：PlayCount 4、EmptyCount 1
            Assert.Equal(stride, d3!.StartFrame - d0!.StartFrame);
        }
    }

    [Fact]
    public void NonCalcDirCollapsesAllDirections()
    {
        var dir0 = Run(SM_WALK, btDir: 0, table: Table(walk: Act(200, calcDir: 0)));
        var dir3 = Run(SM_WALK, btDir: 3, table: Table(walk: Act(200, calcDir: 0)));

        Assert.Equal(dir0!.StartFrame, dir3!.StartFrame);
    }

    [Fact]
    public void CalcDirSeparatesDirections()
    {
        var dir0 = Run(SM_WALK, btDir: 0, table: Table(walk: Act(200, calcDir: 1)));
        var dir1 = Run(SM_WALK, btDir: 1, table: Table(walk: Act(200, calcDir: 1)));

        Assert.NotEqual(dir0!.StartFrame, dir1!.StartFrame);
    }

    // ===================== 未匹配动作 =====================

    [Fact]
    public void UnknownActionReturnsNull()
    {
        Assert.Null(Run(9999));
    }

    // ===================== 各分支取不同动作（防取错） =====================

    [Fact]
    public void EachBranchReadsItsOwnActionType()
    {
        var table = Table(
            stand: Act(100), walk: Act(200), defAttack: Act(300),
            struck: Act(400), die: Act(500), stoneRevive: Act(600));

        Assert.Equal(100, Run(0, table: table)!.StartFrame);
        Assert.Equal(200, Run(SM_WALK, table: table)!.StartFrame);
        Assert.Equal(300, Run(SM_HIT, table: table)!.StartFrame);
        Assert.Equal(400, Run(SM_STRUCK, table: table)!.StartFrame);
        Assert.Equal(500, Run(SM_NOWDEATH, table: table)!.StartFrame);
        Assert.Equal(600, Run(SM_DIGUP, table: table)!.StartFrame);
    }
}
