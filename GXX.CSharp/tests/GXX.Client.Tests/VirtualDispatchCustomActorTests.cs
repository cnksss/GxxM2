using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 p7-client-virtual：**虚分派**验证（改动后定稿版，7 个成员全覆盖）。
///
/// 核心命题：原文 CustomActor.pas 的 7 个方法全带 `override`，托管侧基类
/// （TActorCore / TActor）此前却把它们写成非虚（或根本没有该成员），于是**调用点全是基类静态类型**
/// （PlaySceneMessages.cs:729、ActorMessages.cs:277/287、ActorMotion.cs:290）时，
/// TCustomActor 的实现被通用动作表 / 空实现**静默接管**。
///
/// 本测试用基类静态类型 <see cref="TActor"/> 持有 <see cref="TCustomActor"/> 实例，
/// 逐一断言调用落到 TCustomActor 的实现而不是基类实现。判据全部是**结构性**的：
/// 基类实现不会写 TCustomActor 独有的状态（FClientActionIndex / m_boCreateEffect / m_boLoadSurface …），
/// 也不会走 CustomActorEnv 的接缝。改动前的结果见 docs/并行报告-p7-client-virtual.md。
/// </summary>
public sealed class VirtualDispatchCustomActorTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public VirtualDispatchCustomActorTests()
    {
        CustomActorEnv.Reset();
        _restore.Add(CustomActorEnv.Reset);

        CustomActorEnv.GameCanvasActive = true;
        CustomActorEnv.GameCanvasInitialized = true;
        _restore.Add(() =>
        {
            CustomActorEnv.GameCanvasActive = false;
            CustomActorEnv.GameCanvasInitialized = false;
        });
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ===================== 构造工具（与 CustomActorTests 同形） =====================

    private static TMonsterClientAction Act(TMonsterClientActionType t, int start, int play, int empty,
        bool calcDir, int playTime = 100)
        => new()
        {
            ActionType = t,
            StartIndex = (short)start,
            PlayCount = (ushort)play,
            EmptyCount = (ushort)empty,
            PlayTime = (ushort)playTime,
            CalcDir = (byte)(calcDir ? 1 : 0),
            EffectIndex = -1,
            EffectIndex2 = -1,
            EffectFile = -1,
            EffectFile2 = -1,
            ActionFile = -1,
        };

    private static TClientCustomMonsterConfig MakeCfg()
    {
        var cfg = new TClientCustomMonsterConfig();
        cfg.Actions[(int)TMonsterClientActionType.matStand] =
            Act(TMonsterClientActionType.matStand, 100, 4, 2, true);
        cfg.Actions[(int)TMonsterClientActionType.matWalk] =
            Act(TMonsterClientActionType.matWalk, 200, 6, 1, true);
        cfg.Actions[(int)TMonsterClientActionType.matDie] =
            Act(TMonsterClientActionType.matDie, 500, 8, 1, true);
        return cfg;
    }

    private static TClientCustomMonsterConfig MakeSoundCfg()
    {
        var cfg = MakeCfg();
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstAttack] = new ShortStr30 { Value = "s_attack" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstDigUP] = new ShortStr30 { Value = "s_digup" };
        return cfg;
    }

    /// <summary>**基类静态类型**持有子类实例——这正是原文调用点的形状。</summary>
    private static TActor AsBaseType(TCustomActor a) => a;

    // ===================== 虚分派断言 =====================

    [Fact]
    public void BaseTypedCalcActorFrame_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeCfg())
        {
            m_nChangeAppr = -1,      // < 0 → 自定义怪分支（非前置门）
            m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_TURN,
            m_btDir = 2,
        };
        CustomActorEnv.TimeGetTimeFn = () => 4242;

        AsBaseType(a).CalcActorFrame();

        // 站立案（100 + 2*(4+2)），只有 TCustomActor 实现写得出
        Assert.Equal(100 + 2 * (4 + 2), a.m_nStartFrame);
        Assert.Equal(100 + 2 * 6 + 4 - 1, a.m_nEndFrame);
        Assert.Equal(4242u, a.m_dwStartTime);
        // TCustomActor 实现独有的落值：FClientActionIndex（基类实现不写）
        Assert.Equal(TMonsterClientActionType.matStand, a.ClientAction);
    }

    [Fact]
    public void BaseTypedGetDefaultFrame_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeCfg())
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
            m_btDir = 3,
            m_nCurrentDefFrame = 2,
        };

        int frame = AsBaseType(a).GetDefaultFrame(false);

        Assert.Equal(100 + 3 * (4 + 2) + 2, frame);
        Assert.Equal(TMonsterClientActionType.matStand, a.ClientAction);
    }

    [Fact]
    public void BaseTypedRun_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeCfg())
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
            m_boUseMagic = true,
            m_nCurEffFrame = 0,      // != m_nSpellFrame - 1
            m_nSpellFrame = 5,
            m_boCreateEffect = true, // 1033-1035 分支会把它改回 False
        };

        AsBaseType(a).Run(1000);

        Assert.False(a.m_boCreateEffect);
    }

    // ---- 基类本轮**新增**的 4 个虚成员：改动前 TActor 上根本没有这 4 个成员
    //      （对基类静态类型的调用会直接 CS1061 编译失败），改动后必须落到 TCustomActor。----

    [Fact]
    public void BaseTypedLoadSurface_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeCfg())
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
        };
        a.m_boLoadSurface = true;        // 415-416 只有 TCustomActor 实现会清掉它
        a.m_BodySurface = "sentinel";

        int fetches = 0;
        CustomActorEnv.FetchSurfaceFn = (_, _, _, _, _) => { fetches++; return "s"; };

        AsBaseType(a).LoadSurface();

        Assert.False(a.m_boLoadSurface);
        Assert.Equal("s", a.m_BodySurface);   // 415-416 清空 + 548 主体取图：只有 TCustomActor 实现会做
        Assert.True(fetches > 0);        // 548-579 的三段取图只有 TCustomActor 实现会走
    }

    [Fact]
    public void BaseTypedDrawChr_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeCfg())
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
            m_boUseMagic = true,                                  // 651 门
            m_CurMagicEffectNumber = -CustomActorAttackSlot.AttackActions[0],  // 652-661 → 槽 0
        };

        // DrawChr 本体（DrawSelfMagicEffect 664-672）会**取时钟接缝**：
        // 基类空实现不会，故接缝被调用即证明落到了 TCustomActor 的实现。
        int tickCalls = 0;
        CustomActorEnv.TimeGetTimeFn = () => { tickCalls++; return 7; };

        AsBaseType(a).DrawChr(0, 0, false, false);

        Assert.True(tickCalls > 0);
    }

    [Fact]
    public void BaseTypedRunSound_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeSoundCfg())
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_DIGUP,   // 1112-1115 门为假
        };

        var played = new List<string>();
        CustomActorEnv.PlaySoundFn = s => played.Add(s);

        AsBaseType(a).RunSound();

        Assert.Equal(new[] { "s_digup" }, played);
        Assert.True(a.m_boRunSound);                 // 1117 只写在 TCustomActor 实现里
    }

    [Fact]
    public void BaseTypedRunActSound_LandsOnCustomActor()
    {
        var a = new TCustomActor(MakeSoundCfg())
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_HIT,
            m_boRunSound = true,                     // 1043 门
        };

        var played = new List<string>();
        CustomActorEnv.PlaySoundFn = s => played.Add(s);

        AsBaseType(a).RunActSound(3);

        Assert.Equal(new[] { "s_attack" }, played);
        Assert.False(a.m_boRunSound);                // 1060：只有 TCustomActor 实现会关掉它
    }
}
