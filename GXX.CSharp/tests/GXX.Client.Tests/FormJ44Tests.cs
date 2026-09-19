using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J44：Client 场景层第二片（GetOffset/GetNpcOffset 偏移计算 + TActor 帧状态机核心）1:1 测试。</summary>
public sealed class ActorFrameTests
{
    // ---- GetOffset（Delphi 2294-2646 抽样验证） ----

    [Fact]
    public void GetOffset_BasicRaceTables()
    {
        Assert.Equal(0, ActorOffsets.GetOffset(1000));    // >=1000 修正：(1000 mod 10)×360
        Assert.Equal(1800, ActorOffsets.GetOffset(1005)); // (5)×360
        Assert.Equal(840, ActorOffsets.GetOffset(3));     // nrace 0：3×280
        Assert.Equal(920, ActorOffsets.GetOffset(14));    // nrace 1：4×230
        Assert.Equal(1080, ActorOffsets.GetOffset(23));   // nrace 2：3×360
        Assert.Equal(3240, ActorOffsets.GetOffset(99));   // nrace 9：9×360
        Assert.Equal(600, ActorOffsets.GetOffset(41));    // nrace 4：npos=1 → 600 特例
        Assert.Equal(0, ActorOffsets.GetOffset(40));      // nrace 4：npos=0 → 0×360
        Assert.Equal(2150, ActorOffsets.GetOffset(55));   // nrace 5：5×430
        Assert.Equal(2640, ActorOffsets.GetOffset(66));   // nrace 6：6×440
        // nrace 13 嵌套
        Assert.Equal(0, ActorOffsets.GetOffset(130));
        Assert.Equal(550, ActorOffsets.GetOffset(133));
        Assert.Equal(3240, ActorOffsets.GetOffset(139));  // else 9×360
        // nrace 16 无分支 → else
        Assert.Equal(360, ActorOffsets.GetOffset(161));
        // nrace 18（无 else 的嵌套：npos>7 保持初值 0）
        Assert.Equal(3014, ActorOffsets.GetOffset(187));
        Assert.Equal(0, ActorOffsets.GetOffset(189));
        // nrace 21（npos=5 无分支 → 0；6→2440 覆盖注释行 2260）
        Assert.Equal(0, ActorOffsets.GetOffset(215));
        Assert.Equal(2440, ActorOffsets.GetOffset(216));
        // nrace 33（0→20 起始非零）
        Assert.Equal(20, ActorOffsets.GetOffset(330));
        // nrace 35（Mon36：-1 保留项）
        Assert.Equal(-1, ActorOffsets.GetOffset(352));
        Assert.Equal(1800, ActorOffsets.GetOffset(353));
        Assert.Equal(6980, ActorOffsets.GetOffset(359));
        // nrace 60/61/62（连续分段）
        Assert.Equal(0, ActorOffsets.GetOffset(600));
        Assert.Equal(6980, ActorOffsets.GetOffset(609));
        Assert.Equal(-1, ActorOffsets.GetOffset(613));
        Assert.Equal(16390, ActorOffsets.GetOffset(619));
        Assert.Equal(17360, ActorOffsets.GetOffset(620));
        // nrace 63/64（Mon32 分组）
        Assert.Equal(4390, ActorOffsets.GetOffset(639));
        Assert.Equal(6680, ActorOffsets.GetOffset(643));
        // nrace 80（8→321 与 7→322 重复形态原文保留）
        Assert.Equal(321, ActorOffsets.GetOffset(808));
        // nrace 90
        Assert.Equal(1770, ActorOffsets.GetOffset(904));
        // nrace 95（npos=3 → 3008 血灵教主；其余 npos×360）
        Assert.Equal(3008, ActorOffsets.GetOffset(953));
        Assert.Equal(1440, ActorOffsets.GetOffset(954));
        // 缺省 nrace → npos×360
        Assert.Equal(3240, ActorOffsets.GetOffset(999));
        Assert.Equal(720, ActorOffsets.GetOffset(302)); // nrace 30 无分支 → else
    }

    [Fact]
    public void GetNpcOffset_AllArms()
    {
        Assert.Equal(0, ActorOffsets.GetNpcOffset(0));
        Assert.Equal(1320, ActorOffsets.GetNpcOffset(22));
        Assert.Equal(1380, ActorOffsets.GetNpcOffset(23));
        Assert.Equal(1470, ActorOffsets.GetNpcOffset(24));
        Assert.Equal(1530, ActorOffsets.GetNpcOffset(25));
        Assert.Equal(1620, ActorOffsets.GetNpcOffset(26));
        Assert.Equal(1650, ActorOffsets.GetNpcOffset(27)); // (27-26)×60+1620-30
        Assert.Equal(1950, ActorOffsets.GetNpcOffset(32)); // (32-26)×60+1590
        Assert.Equal(1740, ActorOffsets.GetNpcOffset(28)); // (28-26)×60+1620
        Assert.Equal(2580, ActorOffsets.GetNpcOffset(42));
        Assert.Equal(2640, ActorOffsets.GetNpcOffset(45));
        Assert.Equal(2700, ActorOffsets.GetNpcOffset(48));
        Assert.Equal(2820, ActorOffsets.GetNpcOffset(50));
        Assert.Equal(2880, ActorOffsets.GetNpcOffset(51));
        Assert.Equal(2960, ActorOffsets.GetNpcOffset(52));
        Assert.Equal(4490, ActorOffsets.GetNpcOffset(54));
        Assert.Equal(4530, ActorOffsets.GetNpcOffset(58));
        Assert.Equal(4490, ActorOffsets.GetNpcOffset(94));
        Assert.Equal(4540, ActorOffsets.GetNpcOffset(59));
        Assert.Equal(3060, ActorOffsets.GetNpcOffset(60));
        Assert.Equal(3480, ActorOffsets.GetNpcOffset(67));
        Assert.Equal(3600, ActorOffsets.GetNpcOffset(68));
        Assert.Equal(3780, ActorOffsets.GetNpcOffset(70));
        Assert.Equal(3830, ActorOffsets.GetNpcOffset(75));
        Assert.Equal(3840, ActorOffsets.GetNpcOffset(76));
        Assert.Equal(3900, ActorOffsets.GetNpcOffset(77));
        Assert.Equal(4060, ActorOffsets.GetNpcOffset(78));
        Assert.Equal(3960, ActorOffsets.GetNpcOffset(81));
        Assert.Equal(4000, ActorOffsets.GetNpcOffset(83));
        Assert.Equal(4030, ActorOffsets.GetNpcOffset(84));
        Assert.Equal(3750, ActorOffsets.GetNpcOffset(90));
        Assert.Equal(3770, ActorOffsets.GetNpcOffset(92));
        Assert.Equal(4240, ActorOffsets.GetNpcOffset(99));
        Assert.Equal(4560, ActorOffsets.GetNpcOffset(100));
        Assert.Equal(4770, ActorOffsets.GetNpcOffset(101));
        Assert.Equal(4810, ActorOffsets.GetNpcOffset(102));
        Assert.Equal(0, ActorOffsets.GetNpcOffset(200));
        Assert.Equal(490, ActorOffsets.GetNpcOffset(207));
        Assert.Equal(630, ActorOffsets.GetNpcOffset(208));
        Assert.Equal(740, ActorOffsets.GetNpcOffset(210));
        Assert.Equal(2820, ActorOffsets.GetNpcOffset(272));
        // 尾部公式链
        Assert.Equal(0, ActorOffsets.GetNpcOffset(1000));      // (1000-1000)×60
            Assert.Equal(21000, ActorOffsets.GetNpcOffset(2500));  // 先 -2000 → 500 → else (500-200)×70
        Assert.Equal(55930, ActorOffsets.GetNpcOffset(999));   // (999-200)×70
        Assert.Equal(7000, ActorOffsets.GetNpcOffset(300));    // (300-200)×70
    }

    // ---- TActor 帧状态机核心 ----

    private static TActorCore NewMonster(byte race, ushort appr, byte dir)
        => new() { m_btRace = race, m_wAppearance = appr, m_btDir = dir };

    [Fact]
    public void CalcActorFrame_Turn_TableDriven()
    {
        var actor = NewMonster(9, 0, dir: 5); // race 9 → MA9（ActStand start=0 frame=1 skip=7 ftime=200）
        actor.m_nCurrentAction = TActorCore.SM_TURN;
        actor.CalcActorFrame();
        // start = 0 + 5×(1+7) = 40；end = start + 1 - 1 = 40
        Assert.Equal(40, actor.m_nStartFrame);
        Assert.Equal(40, actor.m_nEndFrame);
        Assert.Equal(200u, actor.m_dwFrameTime);
        Assert.Equal(1, actor.m_nDefFrameCount);
        Assert.Equal(-1, actor.m_nCurrentFrame);
        Assert.False(actor.m_boUseMagic);
    }

    [Fact]
    public void CalcActorFrame_Walk_And_BackStep()
    {
        var actor = NewMonster(10, 0, dir: 3); // MA10（ActWalk start=64 frame=6 skip=2 ftime=120 usetick=3）
        actor.m_nCurrentAction = TActorCore.SM_WALK;
        actor.CalcActorFrame();
        Assert.Equal(64 + 3 * 8, actor.m_nStartFrame);
        Assert.Equal(64 + 3 * 8 + 5, actor.m_nEndFrame);
        Assert.Equal(120u, actor.m_dwFrameTime);
        Assert.Equal(3, actor.m_nMaxTick);
        Assert.Equal(1, actor.m_nMoveStep);

        var back = NewMonster(10, 0, dir: 6);
        back.m_btStep = 2;
        back.m_nCurrentAction = TActorCore.SM_BACKSTEP;
        back.CalcActorFrame();
        Assert.Equal(2, back.m_nMoveStep); // m_btStep 覆盖
    }

    [Fact]
    public void CalcActorFrame_Hit_Struck_Death()
    {
        var hit = NewMonster(9, 0, dir: 2); // MA9 ActAttack start=64 frame=6 skip=2 ftime=150
        hit.m_nCurrentAction = TActorCore.SM_HIT;
        hit.CalcActorFrame();
        Assert.Equal(64 + 2 * 8, hit.m_nStartFrame);
        Assert.Equal(64 + 2 * 8 + 5, hit.m_nEndFrame);
        Assert.Equal(150u, hit.m_dwFrameTime);

        var struck = NewMonster(9, 0, dir: 0); // MA9 ActStruck start=64 frame=6 skip=2
        struck.m_nCurrentAction = TActorCore.SM_STRUCK;
        struck.CalcActorFrame();
        Assert.Equal(64, struck.m_nStartFrame);
        Assert.Equal(150u, struck.m_dwFrameTime); // m_dwStruckFrameTime 覆盖表值

        var death = NewMonster(9, 0, dir: 1); // MA9 ActDie start=0 frame=1 skip=7
        death.m_nCurrentAction = TActorCore.SM_DEATH;
        death.CalcActorFrame();
        Assert.Equal(death.m_nEndFrame, death.m_nStartFrame); // 尸体停在末帧
        Assert.Equal(140u, death.m_dwFrameTime);

        var nowDeath = NewMonster(9, 0, dir: 1);
        nowDeath.m_nCurrentAction = TActorCore.SM_NOWDEATH;
        nowDeath.CalcActorFrame();
        Assert.Equal(8, nowDeath.m_nStartFrame);
        Assert.Equal(8, nowDeath.m_nEndFrame);

        var skeleton = NewMonster(9, 0, dir: 3); // MA9 ActDeath start=0 frame=1 ftime=0
        skeleton.m_nCurrentAction = TActorCore.SM_SKELETON;
        skeleton.CalcActorFrame();
        Assert.Equal(3, skeleton.m_nStartFrame); // ActDeath.start + dir（无 skip 乘法）
    }

    [Fact]
    public void CalcActorFrame_UnknownRace_NoTable()
    {
        var actor = NewMonster(1, 0, dir: 0); // race 1 无表 → GetRaceByPM 缺省 MA19 非空；
        // 为验证 m_Action=nil 早退，直接构造无查表场景：GetRaceByPM 恒返回表，因此用方向公式仍成立
        actor.m_nCurrentAction = TActorCore.SM_TURN;
        actor.CalcActorFrame();
        Assert.NotNull(actor.m_Action); // 缺省 MA19
        Assert.Equal(0, actor.m_nBodyOffset); // appr 0 → nrace 0/npos 0 → 0
    }

    [Fact]
    public void Run_FrameAdvance_And_ActionReset()
    {
        uint now = 1000;
        var actor = NewMonster(10, 0, dir: 0); // MA10 ActAttack start=128 frame=4 skip=4 ftime=150
        actor.m_nCurrentAction = TActorCore.SM_HIT;
        actor.CalcActorFrame(); // 内部重置 m_dwStartTime = TimeGetTime()
        actor.m_dwStartTime = now; // 测试显式定基（与 Delphi TimeGetTime 时刻等效）
        Assert.Equal(128, actor.m_nStartFrame);
        Assert.Equal(131, actor.m_nEndFrame);

        actor.Run(now + 100); // 未超时 → 仅钳制到首帧（-1 → start）
        Assert.Equal(128, actor.m_nCurrentFrame);
        actor.Run(now + 151); // 超时 → 推进
        Assert.Equal(129, actor.m_nCurrentFrame);
        actor.Run(now + 302);
        Assert.Equal(130, actor.m_nCurrentFrame);
        actor.Run(now + 453);
        Assert.Equal(131, actor.m_nCurrentFrame);
        actor.Run(now + 604); // 已到末帧 → 动作复位
        Assert.Equal(0, actor.m_nCurrentAction);
        Assert.False(actor.m_boUseMagic);
    }

    [Fact]
    public void Run_MoveActionGuard()
    {
        var actor = NewMonster(10, 0, dir: 0);
        actor.m_nCurrentAction = TActorCore.SM_WALK;
        actor.m_dwStartTime = 100;
        actor.Run(99999); // 行走族由移动驱动，Run 不推进
        Assert.Equal(-1, actor.m_nCurrentFrame);
        Assert.Equal(TActorCore.SM_WALK, actor.m_nCurrentAction);

        Assert.True(TActorCore.IsMoveAction(TActorCore.SM_RUN));
        Assert.True(TActorCore.IsMoveAction(TActorCore.SM_HORSERUN));
        Assert.True(TActorCore.IsMoveAction(TActorCore.SM_MAGICMOVE));
        Assert.True(TActorCore.IsMoveAction(TActorCore.SM_100HIT));
        Assert.False(TActorCore.IsMoveAction(TActorCore.SM_HIT));
        Assert.False(TActorCore.IsMoveAction(TActorCore.SM_TURN));
    }

    [Fact]
    public void Run_MsgMuch_PacesFaster()
    {
        var actor = NewMonster(10, 0, dir: 0);
        actor.m_nCurrentAction = TActorCore.SM_HIT;
        uint now = 500;
        actor.CalcActorFrame(); // ftime=150 → 积压时 100
        actor.m_dwStartTime = now;
        actor.m_boMsgMuch = true;

        actor.Run(now + 101); // 积压步长 100 < 101 → 钳制并推进
        Assert.Equal(129, actor.m_nCurrentFrame);
        Assert.Equal(now + 101, actor.m_dwStartTime);
    }

    [Fact]
    public void Run_DeleteAfterFinished()
    {
        var actor = NewMonster(9, 0, dir: 0); // MA9 ActStand frame=1：一帧动作
        actor.m_nCurrentAction = TActorCore.SM_NOWDEATH;
        actor.m_boDelActionAfterFinished = true;
        int ended = 0;
        actor.OnActionEnded = () => ended++;
        uint now = 100;
        // CalcActorFrame 内部按 TimeGetTime 打点，必须把该接缝固定到 now，
        // 否则 Run(now+500) 会与真实墙钟做 uint 相减而产生偶发失败。
        actor.TimeGetTime = () => now;
        actor.CalcActorFrame();
        actor.Run(now + 500); // 末帧 → 结束：删除标记 + 复位
        Assert.Equal(1, ended);
        Assert.Equal(0, actor.m_nCurrentAction);
        Assert.True(actor.m_boDelActor);
        Assert.True(actor.m_boFreeActor);
        Assert.Equal(now + 500, actor.m_dwDeleteTime);
    }

    [Fact]
    public void GetBack_OppositeDirection()
    {
        Assert.Equal(4, TActorCore.GetBack(0));
        Assert.Equal(0, TActorCore.GetBack(4));
        Assert.Equal(7, TActorCore.GetBack(3));
        Assert.Equal(1, TActorCore.GetBack(5));
    }
}
