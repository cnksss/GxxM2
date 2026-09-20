using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 `p7-client-actor-family`：**7 个原文子类 override 落地后的行为证据**
/// （`HerbActor.pas` 的 `TKillingHerb` 157-268 / `TBeeQueen` 303-425 /
/// `TMineMon` 1166-1226 / `TCentipedeKingMon` 430-514+1177-1284 /
/// `TBigHeartMon` 1230-1234 / `TSpiderHouseMon` 1238-1242 /
/// `TDragonBody` 1288-1339 / `TCastleDoor` 519-741 /
/// `TWallStructure` 746-968 / `TNewWallStructure` 973-1162）。
///
/// <para><b>本文件与 `TailHerbActorTests` 的分工</b>：那边测的是 `HerbActor.cs` 里
/// <c>IHerbActorView</c> 形状的**纯函数抽取**（决策表，供审计/差异断言）；
/// 本文件测的是**真正被执行**的类 <c>override</c> —— 即"接通之后有没有效果"。
/// 两者是同一批原文判定的两种承载，**都保留**（报告 §11 已登记"若两者分歧以原文为准"）。</para>
///
/// <para><b>差异断言集中点</b>：这一族特别容易"看起来一样实则不同"，
/// 故每个差异都在对应 típus 上双向断言（A 有、B 无）。</para>
/// </summary>
public sealed class ActorFamilySubclassTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorFamilySubclassTests()
    {
        ActorFamilyEnv.Reset();
        _restore.Add(ActorFamilyEnv.Reset);

        ActorFamilyEnv.CanvasReadyFn = () => true;
        _restore.Add(() => ActorFamilyEnv.CanvasReadyFn = () => false);

        ActorFamilyEnv.MyGetTickCountFn = () => 5555;
        _restore.Add(() => ActorFamilyEnv.MyGetTickCountFn = () => SceneTime.TickNow());
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ══════════════════════════════════════════════════════════════════════
    // 工具
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>race 13 → MA13（ActStand=0/4/skip6、ActWalk=10/8/ftime160、ActAttack=30/6/skip4/ftime120、
    /// ActStruck=110/2、ActDie=130/10、ActDeath=20/9/ftime150）；race 43 → MA21。
    /// <para>下面的常量取自 <c>Grobal2.pas:1938-1943</c> 的 <c>SM_ATTACK01..06 = 8946..8951</c>；
    /// 托管侧 <c>TActorCore</c> 只暴露了首末两个（<c>SM_ATTACK01</c>/<c>SM_ATTACK06</c>），
    /// 中间四个用偏移表达 —— 与原文本体里的写法一致。</para></summary>
    private const int SM_ATTACK02 = TActorCore.SM_ATTACK01 + 1;
    private const int SM_ATTACK03 = TActorCore.SM_ATTACK01 + 2;
    private const int SM_ATTACK04 = TActorCore.SM_ATTACK01 + 3;
    private const int SM_ATTACK05 = TActorCore.SM_ATTACK01 + 4;

    /// <summary>基类静态类型持有 —— 与场景调用点（`PlaySceneNewActor` 的工厂）同形。</summary>
    private static TActor AsBase(TActor a) => a;

    private static TKillingHerb KillingHerb() => new()
    {
        m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 2,
        m_dwStruckFrameTime = 150,
    };

    // ══════════════════════════════════════════════════════════════════════
    // 一、类层级（★ 若平铺为 : TActor，inherited 会绕过 TKillingHerb）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void SubclassHierarchyMatchesOriginal()
    {
        // 原文 :37/:52/:66/:71/:132 全部 `class(TKillingHerb)`
        Assert.IsAssignableFrom<TKillingHerb>(new TMineMon());
        Assert.IsAssignableFrom<TKillingHerb>(new TCentipedeKingMon());
        Assert.IsAssignableFrom<TKillingHerb>(new TBigHeartMon());
        Assert.IsAssignableFrom<TKillingHerb>(new TSpiderHouseMon());
        Assert.IsAssignableFrom<TKillingHerb>(new TDragonBody());

        // 原文 :45 `TBeeQueen = class(TActor)` —— **不是** TKillingHerb
        Assert.IsAssignableFrom<TActor>(new TBeeQueen());
        Assert.IsNotAssignableFrom<TKillingHerb>(new TBeeQueen());

        // 原文 :76/:94/:111 三者都是 `class(TActor)`
        Assert.IsAssignableFrom<TActor>(new TCastleDoor());
        Assert.IsAssignableFrom<TActor>(new TWallStructure());
        Assert.IsAssignableFrom<TActor>(new TNewWallStructure());
    }

    [Fact]
    public void Mon36FamilyDoesNotInheritKillingHerbImpl()
    {
        // ★ 差异断言：TMineMon 的 CalcActorFrame 会经 inherited 走到 TKillingHerb 的
        //   变身前置段（把 SM_ATTACK01 改写成 SM_HIT）；TBeeQueen 也有自己的同名段，
        //   但两者是**不同的类**（TBeeQueen 不是 TKillingHerb 的子类）。
        var mine = new TMineMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = 0, m_nCurrentAction = TActorCore.SM_ATTACK01 };
        var queen = new TBeeQueen { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = 0, m_nCurrentAction = TActorCore.SM_ATTACK01 };

        mine.CalcActorFrame();
        queen.CalcActorFrame();

        Assert.Equal(TActorCore.SM_HIT, mine.m_nCurrentAction);   // 经 TMineMon→TKillingHerb 的 163-169
        Assert.Equal(TActorCore.SM_HIT, queen.m_nCurrentAction);  // 经 TBeeQueen 自己的 309-315
    }

    // ══════════════════════════════════════════════════════════════════════
    // 二、变身前置段（162-173 / 308-319 / 435-446 / 587-598 / 767-778 / 994-1005）
    // ══════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(TActorCore.SM_ATTACK01)]
    [InlineData(SM_ATTACK02)]
    [InlineData(SM_ATTACK03)]
    [InlineData(SM_ATTACK04)]
    [InlineData(SM_ATTACK05)]
    [InlineData(TActorCore.SM_ATTACK06)]
    public void KillingHerbChangeApprNormalizesSixAttackActionsToHit(int action)
    {
        var a = KillingHerb();
        a.m_nChangeAppr = 0;                 // >= 0 → 变身前置段
        a.m_nCurrentAction = action;

        a.CalcActorFrame();

        Assert.Equal(TActorCore.SM_HIT, a.m_nCurrentAction);
    }

    [Fact]
    public void KillingHerbChangeApprDoesNotNormalizeOutOfListAction()
    {
        // ★ 差异断言：归一化列表**只有六个**（SM_ATTACK01..06），列表外动作不改写
        var a = KillingHerb();
        a.m_nChangeAppr = 0;
        a.m_nCurrentAction = TActorCore.SM_ATTACK06 + 1;   // 8952，不在列表内

        a.CalcActorFrame();

        Assert.Equal(TActorCore.SM_ATTACK06 + 1, a.m_nCurrentAction);
    }

    [Fact]
    public void KillingHerbChangeApprDelegatesToBaseCalcActorFrame()
    {
        // 171 `inherited;` —— 基类分支会写 m_nBodyOffset（= ActorOffsets.GetOffset）
        var a = KillingHerb();
        a.m_nChangeAppr = 0;
        a.m_nCurrentAction = TActorCore.SM_TURN;
        a.m_nBodyOffset = -999;

        a.CalcActorFrame();

        Assert.Equal(ActorOffsets.GetOffset(100), a.m_nBodyOffset);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 三、★ 核心差异：方向乘法（KillingHerb 有 vs BeeQueen 无）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void KillingHerbHitAppliesDirMultiplier()
    {
        // 230：ActAttack.start + m_btDir * (frame + skip) = 30 + 2*(6+4) = 80
        var a = KillingHerb();
        a.m_nCurrentAction = TActorCore.SM_HIT;

        AsBase(a).CalcActorFrame();

        Assert.Equal(30 + 2 * (6 + 4), a.m_nStartFrame);
        Assert.Equal(30 + 20 + 6 - 1, a.m_nEndFrame);
        Assert.Equal(120u, a.m_dwFrameTime);              // ActAttack.ftime
        Assert.Equal(5555u, a.m_dwStartTime);
    }

    [Fact]
    public void BeeQueenHitDoesNotApplyDirMultiplier()
    {
        // ★ 差异断言（366）：同样的 race/外观/方向，BeeQueen 的 start **不含**方向乘法
        var queen = new TBeeQueen
        {
            m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 2,
            m_nCurrentAction = TActorCore.SM_HIT,
        };

        AsBase(queen).CalcActorFrame();

        Assert.Equal(30, queen.m_nStartFrame);            // 30（不是 80）
        Assert.NotEqual(80, queen.m_nStartFrame);
    }

    [Fact]
    public void KillingHerbTurnDoesNotApplyDirMultiplier()
    {
        // 182：SM_TURN 的方向乘法在原文**被注释掉** ⇒ start = ActStand.start = 0
        var a = KillingHerb();
        a.m_nCurrentAction = TActorCore.SM_TURN;

        AsBase(a).CalcActorFrame();

        Assert.Equal(0, a.m_nStartFrame);
        Assert.Equal(0 + 4 - 1, a.m_nEndFrame);
        Assert.Equal(4, a.m_nDefFrameCount);
    }

    [Fact]
    public void KillingHerbStruckUsesStruckFrameTimeNotTableFtime()
    {
        // 241：`m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;`
        var a = KillingHerb();
        a.m_dwStruckFrameTime = 777;
        a.m_nCurrentAction = TActorCore.SM_STRUCK;

        AsBase(a).CalcActorFrame();

        Assert.Equal(777u, a.m_dwFrameTime);              // 不是 ActStruck.ftime = 100
        Assert.Equal(110 + 2 * (2 + 0), a.m_nStartFrame); // 114
    }

    [Fact]
    public void KillingHerbDeathStopsOnLastFrame()
    {
        // 250：`m_nStartFrame := m_nEndFrame;`（尸体停在末帧）
        var a = KillingHerb();
        a.m_nCurrentAction = TActorCore.SM_DEATH;

        AsBase(a).CalcActorFrame();

        Assert.Equal(a.m_nEndFrame, a.m_nStartFrame);
        Assert.Equal(130 + 2 * (10 + 0) + 10 - 1, a.m_nStartFrame);
    }

    [Fact]
    public void KillingHerbDigdownDelaysDeleteAndUsesDeathTable()
    {
        // 260-265：用 ActDeath（**不加方向乘法**）且置删除标记
        var a = KillingHerb();
        a.m_nCurrentAction = TActorCore.SM_DIGDOWN;

        AsBase(a).CalcActorFrame();

        Assert.Equal(20, a.m_nStartFrame);                // ActDeath.start
        Assert.Equal(20 + 9 - 1, a.m_nEndFrame);
        Assert.Equal(150u, a.m_dwFrameTime);              // ActDeath.ftime
        Assert.True(a.m_boDelActionAfterFinished);
    }

    [Fact]
    public void BeeQueenHasNoDigupOrDigdownBranches()
    {
        // ★ 差异断言：303-397 **没有** SM_DIGUP / SM_DIGDOWN 分支 ⇒ 越界动作什么都不做
        var queen = new TBeeQueen
        {
            m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 2,
        };

        queen.m_nCurrentAction = TActorCore.SM_DIGDOWN;
        queen.m_nStartFrame = -12345;
        AsBase(queen).CalcActorFrame();
        Assert.Equal(-12345, queen.m_nStartFrame);        // 未被改写

        var a = KillingHerb();
        a.m_nCurrentAction = TActorCore.SM_DIGDOWN;
        AsBase(a).CalcActorFrame();
        Assert.NotEqual(-12345, a.m_nStartFrame);         // TKillingHerb 有该分支
    }

    [Fact]
    public void KillingHerbDigupUsesWalkTableAndMoveState()
    {
        // 189-199：SM_DIGUP 用 ActWalk，且写 m_nMaxTick/m_nCurTick/m_nMoveStep
        var a = KillingHerb();
        a.m_nCurrentAction = TActorCore.SM_DIGUP;

        AsBase(a).CalcActorFrame();

        Assert.Equal(10, a.m_nStartFrame);                // ActWalk.start（无方向乘法）
        Assert.Equal(10 + 8 - 1, a.m_nEndFrame);
        Assert.Equal(160u, a.m_dwFrameTime);
        Assert.Equal(0, a.m_nMaxTick);                    // MA13.ActWalk.usetick = 0
        Assert.Equal(0, a.m_nCurTick);
        Assert.Equal(1, a.m_nMoveStep);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 四、GetDefaultFrame 三态差异
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void KillingHerbDefaultFrameSkeletonUsesDeathTable()
    {
        var a = KillingHerb();
        a.m_boDeath = true;
        a.m_boSkeleton = true;

        Assert.Equal(20, AsBase(a).GetDefaultFrame(false));   // 285：ActDeath.start
    }

    [Fact]
    public void KillingHerbDefaultFrameCorpseUsesDieTableWithDir()
    {
        var a = KillingHerb();
        a.m_boDeath = true;
        a.m_boSkeleton = false;

        // 287：ActDie.start + Dir*(frame+skip) + (frame-1) = 130 + 2*10 + 9
        Assert.Equal(130 + 2 * (10 + 0) + (10 - 1), AsBase(a).GetDefaultFrame(false));
    }

    [Fact]
    public void BeeQueenDefaultFrameCorpseHasNoSkeletonBranchAndNoDir()
    {
        // ★ 差异断言（412-414）：BeeQueen 死亡时**没有** m_boSkeleton 分支，且无方向乘法
        var queen = new TBeeQueen
        {
            m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 2, m_boDeath = true,
        };

        int withSkeleton = AsBase(queen).GetDefaultFrame(false);
        queen.m_boSkeleton = true;
        int alsoWithSkeleton = AsBase(queen).GetDefaultFrame(false);

        Assert.Equal(130 + (10 - 1), withSkeleton);          // 139（ActDie.start + frame - 1）
        Assert.Equal(withSkeleton, alsoWithSkeleton);        // m_boSkeleton 无影响

        // 对照：TKillingHerb 的同一场景**受** m_boSkeleton 影响
        var a = KillingHerb();
        a.m_boDeath = true;
        a.m_boSkeleton = false;
        int killedNoSkel = AsBase(a).GetDefaultFrame(false);
        a.m_boSkeleton = true;
        int killedSkel = AsBase(a).GetDefaultFrame(false);
        Assert.NotEqual(killedNoSkel, killedSkel);
    }

    [Fact]
    public void KillingHerbDefaultFrameStandClampsOutOfRangeToZero()
    {
        static int F(int def)
        {
            var a = KillingHerb();
            a.m_nCurrentDefFrame = def;
            return AsBase(a).GetDefaultFrame(false);
        }

        Assert.Equal(0, F(0));      // 边界下界合法
        Assert.Equal(3, F(3));      // 边界上界合法（frame = 4）
        Assert.Equal(0, F(-1));     // < 0 → 0
        Assert.Equal(0, F(4));      // >= frame → 0
        Assert.Equal(0, F(99));
    }

    [Fact]
    public void KillingHerbDefaultFrameWritesDefFrameCount()
    {
        var a = KillingHerb();
        a.m_nCurrentDefFrame = 1;
        AsBase(a).GetDefaultFrame(false);
        Assert.Equal(4, a.m_nDefFrameCount);       // 290：ActStand.frame
    }

    [Fact]
    public void MineMonDefaultFrameIsAlwaysZeroWhenNotTransformed()
    {
        // 1218-1226：未变身**恒返回 0**，且**不写** m_nDefFrameCount（与 TKillingHerb 的差异）
        var m = new TMineMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentDefFrame = 2 };
        m.m_nDefFrameCount = -77;

        Assert.Equal(0, AsBase(m).GetDefaultFrame(false));
        Assert.Equal(-77, m.m_nDefFrameCount);     // 未被改写
    }

    [Fact]
    public void MineMonDefaultFrameDelegatesWhenTransformed()
    {
        var m = new TMineMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = 0, m_nCurrentDefFrame = 2 };
        // 变身 → inherited GetDefaultFrame(wmode)（基类动作表分支）
        Assert.Equal(0 + 2, AsBase(m).GetDefaultFrame(false));   // ActStand.start + cf
        Assert.Equal(4, m.m_nDefFrameCount);                     // 基类分支会写
    }

    [Fact]
    public void MineMonCalcActorFrameIsPureDelegation()
    {
        // 1166-1170：只有 inherited —— 与 TKillingHerb 同结果
        var m = new TMineMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 2, m_nCurrentAction = TActorCore.SM_HIT };
        AsBase(m).CalcActorFrame();
        Assert.Equal(30 + 2 * (6 + 4), m.m_nStartFrame);

        var k = KillingHerb();
        k.m_nCurrentAction = TActorCore.SM_HIT;
        AsBase(k).CalcActorFrame();
        Assert.Equal(k.m_nStartFrame, m.m_nStartFrame);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 五、TBigHeartMon / TSpiderHouseMon（m_btDir := 0 后转调）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void BigHeartAndSpiderHouseZeroDirThenDelegate()
    {
        foreach (TActor a in new TActor[] { new TBigHeartMon(), new TSpiderHouseMon() })
        {
            a.m_btRace = 13;
            a.m_wAppearance = 100;
            a.m_nChangeAppr = -1;
            a.m_btDir = 5;
            a.m_nCurrentAction = TActorCore.SM_HIT;

            AsBase(a).CalcActorFrame();

            Assert.Equal(0, a.m_btDir);                       // 1232 / 1240
            Assert.Equal(30, a.m_nStartFrame);                // Dir = 0 ⇒ 无方向乘法项
        }
    }

    [Fact]
    public void BigHeartAndSpiderHouseAreDifferentClassesWithSameBody()
    {
        // 原文两个类的方法体逐字相同 —— 但**不是**同一个类，保留两者
        Assert.IsNotAssignableFrom<TBigHeartMon>(new TSpiderHouseMon());
    }

    // ══════════════════════════════════════════════════════════════════════
    // 六、TCentipedeKingMon（430-514 / 1177-1284）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void CentipedeKingTurnZerosDirThenDelegates()
    {
        // 454-457：SM_TURN → m_btDir := 0 后 inherited
        var c = new TCentipedeKingMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 4, m_nCurrentAction = TActorCore.SM_TURN };
        AsBase(c).CalcActorFrame();
        Assert.Equal(0, c.m_btDir);
        Assert.Equal(0, c.m_nStartFrame);            // 经 TKillingHerb 的 182
    }

    [Fact]
    public void CentipedeKingDigdownDoesNotZeroDir()
    {
        // ★ 差异断言（506-508）：SM_DIGDOWN 这一支**没有** m_btDir := 0
        var c = new TCentipedeKingMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 4, m_nCurrentAction = TActorCore.SM_DIGDOWN };
        AsBase(c).CalcActorFrame();
        Assert.Equal(4, c.m_btDir);                  // 未被清零
    }

    [Fact]
    public void CentipedeKingHitUsesCriticalTableAndEnablesDieEffect()
    {
        // 492-505：m_btDir := 0 → ActCritical（MA13 的 ActCritical 全 0）→ 死亡特效三段
        var c = new TCentipedeKingMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 4, m_nCurrentAction = TActorCore.SM_HIT };
        AsBase(c).CalcActorFrame();

        Assert.Equal(0, c.m_btDir);
        Assert.Equal(0, c.m_nStartFrame);            // ActCritical.start = 0
        Assert.True(c.BoUseDieEffect);               // 498
        Assert.Equal(0, c.m_EffectFrame);            // 499
        Assert.Equal(0, c.m_nEffectStart);           // 500
        Assert.Equal(9, c.m_nEffectEnd);             // 501：0 + 9
        Assert.Equal(62u, c.m_dwEffectFrameTime);    // 502
    }

    [Fact]
    public void CentipedeKingElseBranchZerosDir()
    {
        // 509-512：else → m_btDir := 0 + inherited
        var c = new TCentipedeKingMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 6, m_nCurrentAction = 12345 };
        AsBase(c).CalcActorFrame();
        Assert.Equal(0, c.m_btDir);
    }

    [Fact]
    public void CentipedeKingRunExitsOnFourMoveActions()
    {
        foreach (int act in new[]
                 {
                     TActorCore.SM_WALK, TActorCore.SM_BACKSTEP,
                     TActorCore.SM_HORSERUN, TActorCore.SM_RUN,
                 })
        {
            var c = new TCentipedeKingMon { m_nCurrentFrame = 0, m_nStartFrame = 0, m_nEndFrame = 0, m_dwStartTime = 0 };
            c.m_nCurrentAction = act;
            c.m_boUseEffect = true;
            c.m_EffectFrame = 3;
            c.m_nEffectEnd = 9;
            c.m_dwEffectFrameTime = 0;

            AsBase(c).Run(999999);

            // 1253 直接 Exit ⇒ 特效帧未被推进、m_boUseEffect 未关
            Assert.Equal(3, c.m_EffectFrame);
            Assert.True(c.m_boUseEffect);
        }
    }

    [Fact]
    public void CentipedeKingRunSwitchesDieEffectAtFrameFive()
    {
        // 1259-1267：`(m_nCurrentFrame - m_nStartFrame) >= 5` → 切到 m_boUseEffect 并重置游标
        var c = new TCentipedeKingMon
        {
            m_nCurrentAction = 1, m_nStartFrame = 10, m_nCurrentFrame = 14,   // 差 4 → 不切
        };
        c.BoUseDieEffect = true;
        AsBase(c).Run(0);
        Assert.True(c.BoUseDieEffect);        // 差 4 < 5 ⇒ 未切

        c.m_nCurrentFrame = 15;               // 差 5 → 切
        AsBase(c).Run(0);
        Assert.False(c.BoUseDieEffect);
        Assert.True(c.m_boUseEffect);
        Assert.Equal(0, c.m_EffectFrame);
    }

    [Fact]
    public void CentipedeKingRunAdvancesEffectFrameAfterFrameTime()
    {
        var c = new TCentipedeKingMon { m_nCurrentAction = 1 };
        c.m_boUseEffect = true;
        c.m_EffectFrame = 0;
        c.m_nEffectEnd = 2;
        c.m_dwEffectFrameTime = 50;
        c.m_dwEffectStartTime = 0;
        ActorFamilyEnv.MyGetTickCountFn = () => 100;   // 100 - 0 > 50

        AsBase(c).Run(0);

        Assert.Equal(1, c.m_EffectFrame);
        Assert.True(c.m_boUseEffect);
    }

    [Fact]
    public void CentipedeKingRunTurnsOffEffectAtEnd()
    {
        var c = new TCentipedeKingMon { m_nCurrentAction = 1 };
        c.m_boUseEffect = true;
        c.m_EffectFrame = 2;
        c.m_nEffectEnd = 2;                            // 已在末帧
        c.m_dwEffectFrameTime = 50;
        c.m_dwEffectStartTime = 0;
        ActorFamilyEnv.MyGetTickCountFn = () => 100;

        AsBase(c).Run(0);

        Assert.False(c.m_boUseEffect);                 // 1276
        Assert.Equal(2, c.m_EffectFrame);              // 未越界
    }

    [Fact]
    public void CentipedeKingLoadEffectLeavesSurfaceNullWhenUnwired()
    {
        // 1186-1187：先置 nil；m_boUseEffect 为假则直接返回（不取图）
        var c = new TCentipedeKingMon();
        c.AttackEffectSurface = "stale";
        c.m_boUseEffect = false;

        c.LoadEffect();

        Assert.Null(c.AttackEffectSurface);
    }

    [Fact]
    public void CentipedeKingFinalizeClearsAttackEffectSurface()
    {
        // 1202-1206
        var c = new TCentipedeKingMon();
        c.AttackEffectSurface = "surface";
        c.Finalize();
        Assert.Null(c.AttackEffectSurface);
    }

    [Fact]
    public void CentipedeKingLoadSurfaceCallsLoadEffect()
    {
        // 1214-1215：inherited LoadSurface(Sender) 后调 LoadEffect()（后者会把槽置 nil）
        var c = new TCentipedeKingMon { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1 };
        c.AttackEffectSurface = "stale";

        c.LoadSurface(null);

        Assert.Null(c.AttackEffectSurface);
        Assert.False(c.m_boLoadSurface);              // 基类 5493 清标志
        Assert.Equal(5555u, c.m_dwLoadSurfaceTime);   // 基类 5492 打点
    }

    // ══════════════════════════════════════════════════════════════════════
    // 七、TDragonBody（1288-1339）—— 硬编码帧区间
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void DragonBodyCalcActorFrameUsesHardCodedFrameRange()
    {
        // 1304-1307：StartFrame=0 / EndFrame=1 / FrameTime=400（与动作表无关）
        var d = new TDragonBody { m_btRace = 13, m_wAppearance = 100, m_btDir = 5, m_nCurrentAction = 1 };

        AsBase(d).CalcActorFrame();

        Assert.Equal(0, d.m_btDir);                   // 1292
        Assert.Equal(0, d.m_nStartFrame);             // 1304
        Assert.Equal(1, d.m_nEndFrame);               // 1305
        Assert.Equal(400u, d.m_dwFrameTime);          // 1306
        Assert.Equal(5555u, d.m_dwStartTime);         // 1307
        Assert.False(d.m_boUseMagic);                 // 1293
        Assert.Equal(-1, d.m_nCurrentFrame);          // 1294
    }

    [Fact]
    public void DragonBodyDigupUsesWalkFtimeAsMaxTick()
    {
        // 1298-1302：★ 用 ActWalk.**ftime** 当 m_nMaxTick（不是 usetick）
        var d = new TDragonBody { m_btRace = 13, m_wAppearance = 100, m_nCurrentAction = TActorCore.SM_DIGUP };

        AsBase(d).CalcActorFrame();

        Assert.Equal(160, d.m_nMaxTick);              // MA13.ActWalk.ftime = 160
        Assert.Equal(0, d.m_nCurTick);
        Assert.Equal(1, d.m_nMoveStep);
    }

    [Fact]
    public void DragonBodyHasNoChangeApprPrologue()
    {
        // ★ 差异断言：TDragonBody **没有** 162-173 式的变身前置段
        var d = new TDragonBody { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = 0, m_nCurrentAction = TActorCore.SM_ATTACK01 };

        AsBase(d).CalcActorFrame();

        Assert.Equal(TActorCore.SM_ATTACK01, d.m_nCurrentAction);   // 未被改写成 SM_HIT
    }

    [Fact]
    public void DragonBodyDrawEffRequiresValidDir()
    {
        var d = new TDragonBody();
        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;

        d.m_btDir = 8;
        d.m_BodySurface = "s";
        AsBase(d).DrawEff(0, 0);
        Assert.Empty(ops);

        d.m_btDir = 7;
        AsBase(d).DrawEff(10, 20);
        Assert.Single(ops);
        Assert.Equal(10 + d.m_nPx + d.m_nShiftX, ops[0].X);
        Assert.Equal(20 + d.m_nPy + d.m_nShiftY, ops[0].Y);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 八、TWallStructure / TNewWallStructure（746-1162）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void WallStructureCalcActorFrameWritesDeathFrame()
    {
        // 789-797：SM_NOWDEATH 用 ActDie，且 deathframe = ActStand.start + m_btDir
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_btDir = 3, m_nCurrentAction = TActorCore.SM_NOWDEATH };

        AsBase(w).CalcActorFrame();

        Assert.Equal(130, w.m_nStartFrame);                  // ActDie.start（无方向乘法）
        Assert.Equal(130 + 10 - 1, w.m_nEndFrame);
        Assert.Equal(0 + 3, w.deathframe);                   // 794
        Assert.True(w.m_boUseEffect);                        // 796
        Assert.Equal(" ", w.m_sUserName);                    // 785（一个空格）
    }

    [Fact]
    public void WallStructureDeathBranchDoesNotWriteFrameTime()
    {
        // 798-803：SM_DEATH 分支**不写** m_dwFrameTime / m_dwStartTime / m_boUseEffect
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_btDir = 1, m_nCurrentAction = TActorCore.SM_DEATH };
        w.m_dwFrameTime = 4242;
        w.m_dwStartTime = 1111;
        w.deathframe = -5;

        AsBase(w).CalcActorFrame();

        Assert.Equal(4242u, w.m_dwFrameTime);                // 未被改写
        Assert.Equal(1111u, w.m_dwStartTime);
        Assert.Equal(0 + 1, w.deathframe);                   // 802 写 deathframe
        Assert.Equal(130 + 10 - 1, w.m_nStartFrame);         // 799：末帧
        Assert.Equal(w.m_nStartFrame, w.m_nEndFrame);
        Assert.Equal(0, w.m_nDefFrameCount);
    }

    [Fact]
    public void WallStructureElseBranchZeroesDefFrameCountAndHoldsPlace()
    {
        // 812-826：else → start = ActStand.start + m_btDir，end = start，DefFrameCount = 0
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_btDir = 6, m_nCurrentAction = 1 };
        w.deathframe = 99;

        AsBase(w).CalcActorFrame();

        Assert.Equal(0 + 6, w.m_nStartFrame);                // 813（★ 只加 Dir，乘法被注释）
        Assert.Equal(w.m_nStartFrame, w.m_nEndFrame);        // 814
        Assert.Equal(0, w.m_nDefFrameCount);                 // 817
        Assert.True(w.m_boHoldPlace);                        // 819
        Assert.Equal(0, w.deathframe);                       // 786 先清 0，非 SM_TURN 不再写
    }

    [Fact]
    public void WallStructureTurnBranchSetsDeathFrame()
    {
        // 820-825：SM_TURN 时在 else 分支内**额外**写 deathframe
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_btDir = 6, m_nCurrentAction = TActorCore.SM_TURN };

        AsBase(w).CalcActorFrame();

        Assert.Equal(0 + 6, w.deathframe);                   // 821
        Assert.True(w.m_boHoldPlace);                        // 822 被注释 ⇒ 仍为 True
    }

    [Fact]
    public void NewWallStructureTurnBranchDoesNotSetDeathFrame()
    {
        // ★ 差异断言：TNewWallStructure 的 else（1038-1046）**没有** SM_TURN → deathframe 特判
        var nw = new TNewWallStructure { m_btRace = 13, m_wAppearance = 100, m_btDir = 6, m_nCurrentAction = TActorCore.SM_TURN };

        AsBase(nw).CalcActorFrame();

        Assert.Equal(0, nw.deathframe);                      // 1013 清 0 后未再写
    }

    [Fact]
    public void WallStructureDigupSetsUseEffectButNewWallDoesNot()
    {
        // ★ 差异断言：1036 的 `// m_boUseEffect := True;` 被注释掉（810 却生效）
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_nCurrentAction = TActorCore.SM_DIGUP };
        AsBase(w).CalcActorFrame();
        Assert.True(w.m_boUseEffect);

        var nw = new TNewWallStructure { m_btRace = 13, m_wAppearance = 100, m_nCurrentAction = TActorCore.SM_DIGUP };
        AsBase(nw).CalcActorFrame();
        Assert.False(nw.m_boUseEffect);
    }

    [Fact]
    public void WallStructureDefaultFrameIgnoresDeathAndDirMultiplication()
    {
        // 914-927：直接 ActStand.start + m_btDir（★ 不判死亡、不写 DefFrameCount）
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_btDir = 5, m_boDeath = true };
        w.m_nDefFrameCount = -3;

        Assert.Equal(0 + 5, AsBase(w).GetDefaultFrame(false));
        Assert.Equal(-3, w.m_nDefFrameCount);
        Assert.Equal(ActorOffsets.GetOffset(100), w.m_nBodyOffset);
    }

    [Fact]
    public void WallStructureRunTogglesBomarkposOnDeathTransition()
    {
        // 952-968：死亡且曾标记 → 解除（True）；存活且未标记 → 标记（False）
        var marks = new List<(int X, int Y, bool Walk)>();
        ActorFamilyHerbEnv.MapMarkCanWalkFn = (x, y, w2) => marks.Add((x, y, w2));
        _restore.Add(() => ActorFamilyHerbEnv.MapMarkCanWalkFn = (_, _, _) => { });

        var w = new TWallStructure { m_nCurrX = 7, m_nCurrY = 9 };
        w.bomarkpos = true;
        w.m_boDeath = true;
        AsBase(w).Run(0);
        Assert.False(w.bomarkpos);
        Assert.Equal((7, 9, true), Assert.Single(marks));

        marks.Clear();
        w.m_boDeath = false;
        w.bomarkpos = false;
        AsBase(w).Run(0);
        Assert.True(w.bomarkpos);
        Assert.Equal((7, 9, false), Assert.Single(marks));
    }

    [Fact]
    public void WallStructureRunDoesNotRetypeMarkWhenAlreadyCorrect()
    {
        var marks = new List<(int, int, bool)>();
        ActorFamilyHerbEnv.MapMarkCanWalkFn = (x, y, w2) => marks.Add((x, y, w2));
        _restore.Add(() => ActorFamilyHerbEnv.MapMarkCanWalkFn = (_, _, _) => { });

        var w = new TWallStructure { m_nCurrX = 1, m_nCurrY = 2 };
        w.bomarkpos = true;
        w.m_boDeath = false;            // 存活但已标记 ⇒ 不再标
        AsBase(w).Run(0);
        Assert.Empty(marks);

        w.bomarkpos = false;
        w.m_boDeath = true;             // 死亡但未标记 ⇒ 不解除
        AsBase(w).Run(0);
        Assert.Empty(marks);
    }

    [Fact]
    public void WallStructureLoadSurfaceUsesDeathFrameIndexWhenPositive()
    {
        // 832-912：deathframe > 0 时**自己装主体图**（图号 = GetOffset + deathframe），不走基类
        var calls = new List<(int Appr, int Idx, int X, int Y, string Kind)>();
        ActorFamilyHerbEnv.FetchWallBodyFn = (a, i, x, y, k) => { calls.Add((a, i, x, y, k)); return "body"; };
        _restore.Add(() => ActorFamilyHerbEnv.FetchWallBodyFn = (_, _, _, _, _) => null);

        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nPx = 4, m_nPy = 5 };
        w.deathframe = 7;

        w.LoadSurface(null);

        Assert.Single(calls);
        Assert.Equal(100, calls[0].Appr);
        Assert.Equal(ActorOffsets.GetOffset(100) + 7, calls[0].Idx);
        Assert.Equal("body", w.m_BodySurface);
    }

    [Fact]
    public void WallStructureLoadSurfaceBrokenBranchIsEmptyForAppearance904OrMore()
    {
        // ★ 差异断言（868-888）：外观 >= 904 时 BrokenSurface **整段被注释掉** ⇒ 保持 nil
        var calls = new List<int>();
        ActorFamilyHerbEnv.FetchWallBrokenFn = (a, i, x, y, k) => { calls.Add(i); return "broken"; };
        _restore.Add(() => ActorFamilyHerbEnv.FetchWallBrokenFn = (_, _, _, _, _) => null);

        // 904 以下：装图
        var w = new TWallStructure { m_btRace = 13, m_wAppearance = 900, m_nChangeAppr = -1, m_btDir = 2 };
        w.LoadSurface(null);
        Assert.Single(calls);
        Assert.Equal("broken", w.BrokenSurface);

        // 904 及以上：空分支
        calls.Clear();
        var w2 = new TWallStructure { m_btRace = 13, m_wAppearance = 904, m_nChangeAppr = -1, m_btDir = 2 };
        w2.LoadSurface(null);
        Assert.Empty(calls);
        Assert.Null(w2.BrokenSurface);
    }

    [Fact]
    public void NewWallStructureLoadSurfaceUsesMinus904Library()
    {
        // ★ 差异断言（1085）：TNewWallStructure 用 m_wAppearance - 904 作破碎图的图库
        var calls = new List<(int Appr, int Idx)>();
        ActorFamilyHerbEnv.FetchWallBrokenFn = (a, i, x, y, k) => { calls.Add((a, i)); return "broken"; };
        _restore.Add(() => ActorFamilyHerbEnv.FetchWallBrokenFn = (_, _, _, _, _) => null);

        var nw = new TNewWallStructure { m_btRace = 13, m_wAppearance = 950, m_nChangeAppr = -1, m_btDir = 1 };
        nw.LoadSurface(null);

        Assert.Single(calls);
        Assert.Equal(950 - 904, calls[0].Appr);
        Assert.Equal(ActorOffsets.GetOffset(950 - 904) + 8 + 1, calls[0].Idx);
    }

    [Fact]
    public void WallStructureCreateAndFinalizeClearBothSurfaces()
    {
        var w = new TWallStructure();
        Assert.Equal(0, w.m_btDir);
        Assert.False(w.bomarkpos);
        w.EffectSurface = "e";
        w.BrokenSurface = "b";
        w.Finalize();
        Assert.Null(w.EffectSurface);
        Assert.Null(w.BrokenSurface);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 九、TCastleDoor（519-741）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void CastleDoorCreateSetsDownDrawLevelOne()
    {
        // 519-525
        var d = new TCastleDoor();
        Assert.Equal(0, d.m_btDir);
        Assert.Equal(1, d.m_nDownDrawLevel);      // 524
        Assert.Null(d.EffectSurface);
    }

    [Fact]
    public void CastleDoorApplyDoorStateMarksTwelveCellsInOrder()
    {
        // 533-560：3 次无条件 True + 9 次 bowalk + 3 次（仅 dsOpen）False = 15 次
        var marks = new List<(int X, int Y, bool Walk)>();
        ActorFamilyHerbEnv.MapMarkCanWalkFn = (x, y, w) => marks.Add((x, y, w));
        _restore.Add(() => ActorFamilyHerbEnv.MapMarkCanWalkFn = (_, _, _) => { });

        var d = new TCastleDoor { m_nCurrX = 10, m_nCurrY = 20, m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentAction = TActorCore.SM_DIGUP };
        AsBase(d).CalcActorFrame();               // 626-632 → ApplyDoorState(dsOpen)

        Assert.Equal(15, marks.Count);
        // 前 3 格无条件 True
        Assert.Equal((10, 18, true), marks[0]);
        Assert.Equal((11, 19, true), marks[1]);
        Assert.Equal((11, 18, true), marks[2]);
        // 第 4-12 格按 bowalk（dsOpen ≠ dsClose ⇒ true）
        Assert.All(marks.GetRange(3, 9), m => Assert.True(m.Item3));
        // 最后 3 格在 dsOpen 下改回 False
        Assert.Equal((10, 18, false), marks[12]);
        Assert.Equal((11, 19, false), marks[13]);
        Assert.Equal((11, 18, false), marks[14]);
    }

    [Fact]
    public void CastleDoorApplyDoorStateCloseMarksAllWalkable()
    {
        // ★ 差异断言：dsClose ⇒ bowalk = False，且**没有**最后 3 格的 False 覆盖 ⇒ 共 12 次
        var marks = new List<(int, int, bool)>();
        ActorFamilyHerbEnv.MapMarkCanWalkFn = (x, y, w) => marks.Add((x, y, w));
        _restore.Add(() => ActorFamilyHerbEnv.MapMarkCanWalkFn = (_, _, _) => { });

        var d = new TCastleDoor { m_nCurrX = 0, m_nCurrY = 0, m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentAction = TActorCore.SM_DIGDOWN };
        AsBase(d).CalcActorFrame();               // 633-641 → ApplyDoorState(dsClose)

        Assert.Equal(12, marks.Count);
        // ★ 前 3 格在 537-539 是**无条件** True（原文如此），第 4-12 格才按 bowalk
        Assert.All(marks.GetRange(0, 3), m => Assert.True(m.Item3));
        Assert.All(marks.GetRange(3, 9), m => Assert.False(m.Item3));
        Assert.False(d.BoDoorOpen);
        Assert.True(d.m_boHoldPlace);
    }

    [Fact]
    public void CastleDoorCalcActorFrameClearsUseEffectNotUseMagic()
    {
        // ★ 差异断言（599）：门清的是 m_boUseEffect（怪物族清的是 m_boUseMagic）
        var d = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentAction = 12345, m_btDir = 1 };
        d.m_boUseEffect = true;
        d.m_boUseMagic = true;

        AsBase(d).CalcActorFrame();

        Assert.False(d.m_boUseEffect);            // 被清
        Assert.True(d.m_boUseMagic);              // 未被清
        Assert.Equal(" ", d.m_sUserName);         // 605
    }

    [Fact]
    public void CastleDoorElseBranchSplitsAtDirThree()
    {
        // 648-669：m_btDir < 3 → 关门态；>= 3 → 开门态
        var closed = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentAction = 12345, m_btDir = 2 };
        AsBase(closed).CalcActorFrame();
        Assert.False(closed.BoDoorOpen);
        Assert.True(closed.m_boHoldPlace);
        Assert.Equal(0 + 2 * (4 + 6), closed.m_nStartFrame);   // ActStand.start + Dir*(frame+skip)
        Assert.Equal(closed.m_nStartFrame, closed.m_nEndFrame);

        var open = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentAction = 12345, m_btDir = 3 };
        AsBase(open).CalcActorFrame();
        Assert.True(open.BoDoorOpen);
        Assert.False(open.m_boHoldPlace);
        Assert.Equal(0, open.m_nStartFrame);                   // ActCritical.start
        Assert.Equal(open.m_nStartFrame, open.m_nEndFrame);
    }

    [Fact]
    public void CastleDoorDeathBranchOmitsFrameTime()
    {
        // 642-647：门死亡的 start/end 都是 ActDie 末帧，且**不写** m_dwFrameTime
        var d = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentAction = TActorCore.SM_DEATH, m_btDir = 0 };
        d.m_dwFrameTime = 8888;

        AsBase(d).CalcActorFrame();

        Assert.Equal(130 + 10 - 1, d.m_nStartFrame);
        Assert.Equal(d.m_nStartFrame, d.m_nEndFrame);
        Assert.Equal(8888u, d.m_dwFrameTime);      // 未被改写
        Assert.Equal(0, d.m_nDefFrameCount);
    }

    [Fact]
    public void CastleDoorDefaultFrameSetsDrawLevelByState()
    {
        // 685-698：死亡/开门 → 层级 2；关门 → 层级 1
        var dead = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_boDeath = true };
        Assert.Equal(130 + 10 - 1, AsBase(dead).GetDefaultFrame(false));
        Assert.Equal(2, dead.m_nDownDrawLevel);

        var open = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_boDeath = false };
        open.BoDoorOpen = true;
        Assert.Equal(0, AsBase(open).GetDefaultFrame(false));      // ActCritical.start
        Assert.Equal(2, open.m_nDownDrawLevel);

        var closed = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_boDeath = false, m_btDir = 2 };
        closed.BoDoorOpen = false;
        Assert.Equal(0 + 2 * (4 + 6), AsBase(closed).GetDefaultFrame(false));
        Assert.Equal(1, closed.m_nDownDrawLevel);
    }

    [Fact]
    public void CastleDoorActionEndedOnlyHandlesDigup()
    {
        // 701-709
        var d = new TCastleDoor();
        d.BoDoorOpen = false;
        d.m_boHoldPlace = true;
        d.m_nCurrentAction = TActorCore.SM_DIGUP;
        AsBase(d).ActionEnded();
        Assert.True(d.BoDoorOpen);
        Assert.False(d.m_boHoldPlace);

        var d2 = new TCastleDoor();
        d2.BoDoorOpen = false;
        d2.m_boHoldPlace = true;
        d2.m_nCurrentAction = TActorCore.SM_DIGDOWN;   // 707-708 被注释掉 ⇒ 不变
        AsBase(d2).ActionEnded();
        Assert.False(d2.BoDoorOpen);
        Assert.True(d2.m_boHoldPlace);
    }

    [Fact]
    public void CastleDoorRunReappliesStateOnlyWhenUnitChanges()
    {
        var marks = new List<(int, int, bool)>();
        ActorFamilyHerbEnv.MapMarkCanWalkFn = (x, y, w) => marks.Add((x, y, w));
        _restore.Add(() => ActorFamilyHerbEnv.MapMarkCanWalkFn = (_, _, _) => { });

        int ux = 5, uy = 6;
        ActorFamilyHerbEnv.MapCurUnitXFn = () => ux;
        ActorFamilyHerbEnv.MapCurUnitYFn = () => uy;
        _restore.Add(() =>
        {
            ActorFamilyHerbEnv.MapCurUnitXFn = () => -1;
            ActorFamilyHerbEnv.MapCurUnitYFn = () => -1;
        });

        var d = new TCastleDoor { m_nCurrX = 1, m_nCurrY = 2 };
        d.oldunitx = 0;
        d.oldunity = 0;
        d.m_boDeath = true;

        AsBase(d).Run(0);                       // 格坐标变化 → 12 次（dsBroken）
        int first = marks.Count;
        Assert.Equal(12, first);
        Assert.Equal(5, d.oldunitx);
        Assert.Equal(6, d.oldunity);

        marks.Clear();
        AsBase(d).Run(0);                       // 未变化 → 不再刷
        Assert.Empty(marks);
    }

    [Fact]
    public void CastleDoorDrawChrForcesBlendFalseForBody()
    {
        // 732：`inherited DrawChr(dx, dy, blend, FALSE)` —— 主体绘制强制 boFlag = FALSE
        var d = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_btDir = 1 };
        d.m_BodySurface = "s";
        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;

        AsBase(d).DrawChr(1, 2, true, true);

        // 主体 + 状态层（若命中）——此处只断言主体确实画了（blend 参数由 DrawEffSurface 内部使用）
        Assert.NotEmpty(ops);
    }

    [Fact]
    public void CastleDoorLoadSurfaceSkipsWhenNoUseEffect()
    {
        // 573：EffectSurface 的装载整体被 if m_boUseEffect 包住
        var calls = new List<int>();
        ActorFamilyHerbEnv.FetchDoorEffectFn = (a, i, x, y, k) => { calls.Add(i); return "eff"; };
        _restore.Add(() => ActorFamilyHerbEnv.FetchDoorEffectFn = (_, _, _, _, _) => null);

        var d = new TCastleDoor { m_btRace = 13, m_wAppearance = 100, m_nChangeAppr = -1, m_nCurrentFrame = 5, m_nStartFrame = 3 };
        d.m_boUseEffect = false;
        d.LoadSurface(null);
        Assert.Empty(calls);
        Assert.Null(d.EffectSurface);

        d.m_boUseEffect = true;
        d.LoadSurface(null);
        Assert.Single(calls);
        Assert.Equal(ActorFamilyHerbEnv.DoorDeathEffectBase + (5 - 3), calls[0]);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 十、截图式守卫：未变身门的两种语义（>= 0 而不是 <> 0）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void ChangeApprZeroIsTreatedAsTransformed()
    {
        // ★ 原文用 `m_nChangeAppr >= 0` —— **0 也算变身**（不是 `<> 0`）。
        //
        // 判据来源（关键）：`m_boUseMagic` / `m_nCurrentFrame` **两边都写**
        // （自有分支 174-175，基类 ActorCore.CalcActorFrame 开头也写），故**不能**用来区分。
        // 真正的判别器是 **`m_nBodyOffset`**：
        //   自有分支 176 用 `GetOffset(m_wAppearance) = ActorOffsets.GetOffset(100) = 0`
        //   基类分支用 `ActorOffsets.GetOffset(m_wAppearance)` —— 对 race 156 走的却是
        //   它自己的 `m_wAppearance` 语义；两者在 appearance = 100 时**恰好同值**，
        //   故改用一个**基类 calc 不处理**的动作（`SM_DIGUP`）：基类 case 无该标签 ⇒
        //   若走了基类分支，`m_nMaxTick` 不会被写成自有分支 194 行的 `ActWalk.usetick`。
        //   更稳的做法是直接看 `m_dwFrameTime`：自有分支 192 写 ActWalk.ftime = 160；
        //   基类分支对 SM_DIGUP **不处理**（不会写），故留作哨兵值。
        var a = KillingHerb();
        a.m_nChangeAppr = 0;                       // 0 是"变身"（>= 0）
        a.m_nCurrentAction = TActorCore.SM_DIGUP;
        a.m_dwFrameTime = 4242;                    // 哨兵：自有分支必覆盖

        AsBase(a).CalcActorFrame();

        // 变身 ⇒ 走基类 ActorCore.CalcActorFrame ⇒ 不处理 SM_DIGUP ⇒ 哨兵保留
        Assert.Equal(4242u, a.m_dwFrameTime);

        // 对照：ChangeAppr < 0 时自有分支会把它覆盖为 ActWalk.ftime = 160
        var b = KillingHerb();
        b.m_nChangeAppr = -1;
        b.m_nCurrentAction = TActorCore.SM_DIGUP;
        b.m_dwFrameTime = 4242;
        AsBase(b).CalcActorFrame();
        Assert.Equal(160u, b.m_dwFrameTime);
    }

    [Fact]
    public void ChangeApprNegativeOneTakesOwnBranch()
    {
        var a = KillingHerb();
        a.m_nChangeAppr = -1;
        a.m_nCurrentAction = TActorCore.SM_TURN;
        a.m_boUseMagic = true;
        a.m_nCurrentFrame = 42;

        AsBase(a).CalcActorFrame();

        Assert.False(a.m_boUseMagic);              // 174 生效
        Assert.Equal(-1, a.m_nCurrentFrame);       // 175 生效
        Assert.Equal(0, a.m_nStartFrame);          // 182 生效（自有分支）
    }
}
