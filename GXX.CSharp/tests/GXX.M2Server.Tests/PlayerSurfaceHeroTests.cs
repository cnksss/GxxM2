// ============================================================================
// 测试：本车道 **英雄/副将片**（切片 4 / 4）+ **TCreature 基类成员片**（切片 3 的基类部分）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Hero.cs
//             GXX.M2Server/Engine/PlayerSurface/TCreature.PlayerSurface.Base.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:275 / 372 / 374 / 375 / 380 / 1698-1702
//           Source/M2Engine/ObjBase.pas:162 / 293 / 357 / 362 / 645-647 / 679-680 /
//             768 / 778 / 681 / 586 / 11473-11478 / 26888-26902 / 32881-32906 /
//             32929-32932 / 33382-33392
//           Source/Common/Grobal2.pas:50 / 3284
// 用例 ≥3/方法：0 / 边界 / 差异断言（含 Delphi 隐式窄化与 1-based→0-based）。
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class PlayerSurfaceHeroTests : IDisposable
{
    public PlayerSurfaceHeroTests() => PlayerSurfaceBaseSeams.ResetDefaults();
    public void Dispose() => PlayerSurfaceBaseSeams.ResetDefaults();

    [Fact]
    public void HeroFields_DefaultsMatchOriginalClearData()
    {
        // 原文 ObjPlayer.pas:1698-1702 ClearData
        var p = new TPlayObject();
        Assert.Null(p.m_MyHero);
        Assert.Equal("", p.m_sHeroName);
        Assert.Equal("", p.m_sTempHeroName);
        Assert.Equal("", p.m_sDeputyHeroName);
        Assert.False(p.m_boWaitHeroDate);
    }

    [Fact]
    public void HeroFields_AreAssignable()
    {
        var hero = new TPlayObject { m_sCharName = "英雄A" };
        var p = new TPlayObject
        {
            m_MyHero = hero,
            m_sHeroName = "英雄A",
            m_sTempHeroName = "临时",
            m_sDeputyHeroName = "副将B",
            m_boWaitHeroDate = true,
        };

        Assert.Same(hero, p.m_MyHero);
        Assert.Equal("英雄A", p.m_sHeroName);
        Assert.Equal("临时", p.m_sTempHeroName);
        Assert.Equal("副将B", p.m_sDeputyHeroName);
        Assert.True(p.m_boWaitHeroDate);
    }

    [Fact]
    public void MyHero_TypeIsCreatureLikeOriginalTBaseObject()
    {
        // 原文 ObjPlayer.pas:372 `m_MyHero: TBaseObject;` —— 托管用最薄的 TCreature
        var f = typeof(TPlayObject).GetField("m_MyHero");
        Assert.NotNull(f);
        Assert.Equal(typeof(TCreature), f!.FieldType);
    }

    [Fact]
    public void MyGamePet_ExistingFieldIsNotRedeclared()
    {
        // 既有成员：Engine/RecalcBonus.cs:248 `public TPlayObject? m_MyGamePet;`
        // 本车道**未重复声明**（若重复会 CS0102）
        var fields = typeof(TPlayObject).GetFields(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        int count = 0;
        foreach (var f in fields) if (f.Name == "m_MyGamePet") count++;
        Assert.Equal(1, count);
    }
}

public class PlayerSurfaceCreatureBaseTests : IDisposable
{
    public PlayerSurfaceCreatureBaseTests() => PlayerSurfaceBaseSeams.ResetDefaults();
    public void Dispose() => PlayerSurfaceBaseSeams.ResetDefaults();

    // ---------------------------------------------------------------
    // 字段形状
    // ---------------------------------------------------------------

    [Fact]
    public void BaseFields_Exist()
    {
        // 原文 ObjBase.pas:162 m_wAppr / :293 m_LastHiter / :357 m_CurrTarget
        var p = new TPlayObject();
        Assert.Equal((ushort)0, p.m_wAppr);
        Assert.Null(p.m_LastHiter);
        Assert.Null(p.m_CurrTarget);
    }

    [Fact]
    public void ActorIcons_HasTenEntries_WithOriginalDefaultsMinusOneAndOne()
    {
        // 原文 ObjBase.pas:362 + Grobal2.pas:50/3284 + 11473-11478
        var p = new TPlayObject();
        Assert.Equal(10, p.m_ActorIcons.Length);
        Assert.Equal(10, TActorIconArrayConst.MAX_ICON_COUNT);
        foreach (var icon in p.m_ActorIcons)
        {
            Assert.Equal((short)-1, icon.nFileIndex);   // 原文 11476
            Assert.Equal((byte)1, icon.nIconCount);     // 原文 11477
        }
    }

    [Fact]
    public void ActorIcons_NewDefaultActorIcons_MatchesClearObject()
    {
        var icons = TCreature.NewDefaultActorIcons();
        Assert.Equal(10, icons.Length);
        Assert.All(icons, i => Assert.Equal((short)-1, i.nFileIndex));
        Assert.All(icons, i => Assert.Equal((byte)1, i.nIconCount));
    }

    // ---------------------------------------------------------------
    // 虚分派（★ 台账 §18.8）
    // ---------------------------------------------------------------

    [Fact]
    public void Initialize_And_RecalcAbilitys_AreVirtual()
    {
        // 原文 ObjBase.pas:768 `procedure Initialize(); virtual;`
        // 原文 ObjBase.pas:778 `procedure RecalcAbilitys(); virtual;`
        Assert.True(typeof(TCreature).GetMethod("Initialize", Type.EmptyTypes)!.IsVirtual);
        Assert.True(typeof(TCreature).GetMethod("RecalcAbilitys", Type.EmptyTypes)!.IsVirtual);
    }

    [Fact]
    public void FeatureChanged_IsNotVirtual_MatchingOriginal()
    {
        // ⚠ 原文 ObjBase.pas:681 `procedure FeatureChanged();` **没有** virtual —— 照抄不升级
        var m = typeof(TCreature).GetMethod("FeatureChanged", Type.EmptyTypes);
        Assert.NotNull(m);
        Assert.False(m!.IsVirtual);
    }

    [Fact]
    public void GetPoseCreate_TurnTo_SendRefMsg_AreNotVirtual_MatchingOriginal()
    {
        // 原文 ObjBase.pas:645-647 / 679-680 / 586 均无 virtual
        Assert.False(typeof(TCreature).GetMethod("GetPoseCreate", Type.EmptyTypes)!.IsVirtual);
        Assert.False(typeof(TCreature).GetMethod("TurnTo", new[] { typeof(int) })!.IsVirtual);
        Assert.False(typeof(TCreature).GetMethod("TurnToEx", new[] { typeof(int) })!.IsVirtual);
        Assert.False(typeof(TCreature).GetMethod("SendRefMsg")!.IsVirtual);
    }

    [Fact]
    public void Initialize_VirtualDispatch_ReachesSubclassOverride()
    {
        // ★ 虚分派回归：下游 TNormNpc/TBoxMonster 的 `inherited Initialize` 依赖它
        var p = new ProbePlayer();
        TCreature asBase = p;
        asBase.Initialize();
        Assert.Equal(1, p.InitializeCalls);
    }

    [Fact]
    public void RecalcAbilitys_VirtualDispatch_AndSeam()
    {
        int seam = 0;
        PlayerSurfaceBaseSeams.RecalcAbilitys = _ => seam++;

        var p = new ProbePlayer();
        TCreature asBase = p;
        asBase.RecalcAbilitys();

        Assert.Equal(1, p.RecalcCalls);
        Assert.Equal(1, seam);
    }

    // ---------------------------------------------------------------
    // GetPoseCreate（原文 26888-26902）
    // ---------------------------------------------------------------

    [Fact]
    public void GetPoseCreate_NoEnvir_ReturnsNull_DoesNotThrow()
    {
        var p = new TPlayObject { m_PEnvir = null, m_btDirection = 0 };
        Assert.Null(p.GetPoseCreate());
    }

    [Fact]
    public void GetPoseCreate_LooksAtFrontCell_DirectionUp()
    {
        // 原文 26892 GetFrontPosition(nX,nY) → 用方向表（托管 ObjBase.cs:122-127）
        var env = new TEnvirnoment();
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 10, m_nCurrY = 10, m_btDirection = 0 };
        var target = new TPlayObject { m_nCurrX = 10, m_nCurrY = 9 };

        TCreature? seen = null;
        int seenX = -1, seenY = -1;
        PlayerSurfaceBaseSeams.GetMovingObject = (_, x, y, _, _) =>
        {
            seenX = x; seenY = y; return target;
        };

        var got = p.GetPoseCreate();
        Assert.Same(target, got);
        Assert.Equal(10, seenX);
        Assert.Equal(9, seenY);      // DR_UP → dy = -1
        Assert.Null(seen);
    }

    [Fact]
    public void GetPoseCreate_WithBaseObject_PassesExclusion()
    {
        var env = new TEnvirnoment();
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 5, m_nCurrY = 5, m_btDirection = 2 };
        var exclude = new TPlayObject();

        TCreature? passed = null;
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, ex, _) => { passed = ex; return null; };

        p.GetPoseCreate(exclude);
        Assert.Same(exclude, passed);
    }

    [Fact]
    public void GetPoseCreate_StepZero_ReturnsNull_NoProbe()
    {
        // ★ 差异断言：原文 `for I := 0 to AStep - 1` 在 AStep = 0 时**一次都不执行**
        var env = new TEnvirnoment();
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 5, m_nCurrY = 5, m_btDirection = 2 };

        int probes = 0;
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, _, _) => { probes++; return new TPlayObject(); };

        Assert.Null(p.GetPoseCreate(0u));
        Assert.Equal(0, probes);
    }

    [Fact]
    public void GetPoseCreate_StepTwo_ProbesUntilHit()
    {
        var env = new TEnvirnoment();
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 5, m_nCurrY = 5, m_btDirection = 2 };
        var hit = new TPlayObject();

        int probes = 0;
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, _, _) =>
        {
            probes++;
            return probes == 2 ? hit : null;
        };

        Assert.Same(hit, p.GetPoseCreate(5u));
        Assert.Equal(2, probes);
    }

    [Fact]
    public void GetFrontPosition_ComputesDeltasByDirection()
    {
        var p = new TPlayObject { m_nCurrX = 20, m_nCurrY = 20, m_btDirection = 4 };  // DR_DOWN
        p.GetFrontPosition(out int x, out int y);
        Assert.Equal(20, x);
        Assert.Equal(21, y);
    }

    // ---------------------------------------------------------------
    // TurnTo / TurnToEx（原文 33382-33392）—— 含 Delphi 隐式窄化差异
    // ---------------------------------------------------------------

    [Fact]
    public void TurnTo_SetsDirectionAndSendsRmTurn()
    {
        var p = new TPlayObject { m_nCurrX = 3, m_nCurrY = 4 };
        int ident = -1; long wp = -1;
        PlayerSurfaceBaseSeams.SendRefMsg = (_, i, a, _, _, _, _, _) => { ident = i; wp = a; };

        p.TurnTo(5);

        Assert.Equal(5, p.m_btDirection);
        Assert.Equal(Grobal2Const.RM_TURN, ident);
        Assert.Equal(5, wp);
    }

    [Fact]
    public void TurnToEx_SendsRmTurnEx()
    {
        var p = new TPlayObject();
        int ident = -1;
        PlayerSurfaceBaseSeams.SendRefMsg = (_, i, _, _, _, _, _, _) => ident = i;

        p.TurnToEx(6);

        Assert.Equal(6, p.m_btDirection);
        Assert.Equal(Grobal2Const.RM_TURN_EX, ident);
    }

    [Fact]
    public void TurnTo_OutOfByteRange_TruncatesLikeDelphiImplicitNarrowing()
    {
        // ★ 差异断言：原文 `m_btDirection := nDir`（Integer → Byte）**静默截断**。
        //   256 → 0，257 → 1，-1 → 255。
        var p = new TPlayObject();
        p.TurnTo(256);
        Assert.Equal(0, p.m_btDirection);

        p.TurnTo(257);
        Assert.Equal(1, p.m_btDirection);

        p.TurnTo(-1);
        Assert.Equal(255, p.m_btDirection);
    }

    [Fact]
    public void TurnTo_Zero_SetsZero()
    {
        var p = new TPlayObject { m_btDirection = 7 };
        p.TurnTo(0);
        Assert.Equal(0, p.m_btDirection);
    }

    // ---------------------------------------------------------------
    // FeatureChanged / SendRefMsg（原文 32929 / 30980）
    // ---------------------------------------------------------------

    [Fact]
    public void FeatureChanged_SendsRmFeatureChangedWithZeroParams()
    {
        // 原文 32931：SendRefMsg(RM_FEATURECHANGED, 0, 0, 0, 0, '');
        var p = new TPlayObject();
        int ident = -1; long a = -1, b = -1, c = -1, d = -1; string msg = "x"; uint delay = 99;
        PlayerSurfaceBaseSeams.SendRefMsg = (_, i, w, p1, p2, p3, s, dl) =>
        { ident = i; a = w; b = p1; c = p2; d = p3; msg = s; delay = dl; };

        p.FeatureChanged();

        Assert.Equal(Grobal2Const.RM_FEATURECHANGED, ident);
        Assert.Equal(0, a); Assert.Equal(0, b); Assert.Equal(0, c); Assert.Equal(0, d);
        Assert.Equal("", msg);
        Assert.Equal(0u, delay);   // 原文默认参数 dwDelay = 0
    }

    [Fact]
    public void SendRefMsg_DefaultDelayIsZero()
    {
        var p = new TPlayObject();
        uint delay = 99;
        PlayerSurfaceBaseSeams.SendRefMsg = (_, _, _, _, _, _, _, dl) => delay = dl;
        p.SendRefMsg(1, 2, 3, 4, 5, "s");
        Assert.Equal(0u, delay);
    }

    [Fact]
    public void SendRefMsg_ExplicitDelayIsPassedThrough()
    {
        var p = new TPlayObject();
        uint delay = 0;
        PlayerSurfaceBaseSeams.SendRefMsg = (_, _, _, _, _, _, _, dl) => delay = dl;
        p.SendRefMsg(1, 2, 3, 4, 5, "s", 200u);
        Assert.Equal(200u, delay);
    }

    // ---------------------------------------------------------------
    // Initialize（原文 32881-32906）
    // ---------------------------------------------------------------

    [Fact]
    public void Initialize_NoEnvir_SetsAddToMapFailTrue()
    {
        // 原文 32898 `m_boAddtoMapFail := True;`，32899 的 CanWalk 因 m_PEnvir = nil 跳过
        var p = new TPlayObject { m_PEnvir = null };
        p.Initialize();
        Assert.True(p.m_boAddtoMapFail);
    }

    /// <summary>构造一个尺寸已知、格子默认可行走的空地图（同 MagicBatchHTests.MakeEnv）。</summary>
    private static TEnvirnoment MakeEnv(int w = 8, int h = 8)
    {
        var env = new TEnvirnoment
        {
            nWidth = w,
            nHeight = h,
            MapCellArray = new TMapCellinfo[w, h],
            MapData = new TMapUnitInfo[w, h]
        };
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                env.MapCellArray[x, y] = new TMapCellinfo();
        return env;
    }

    [Fact]
    public void Initialize_AddableCell_ClearsAddToMapFail()
    {
        var env = MakeEnv();
        // 空地：CanWalk 真、AddToMap 真 → m_boAddtoMapFail := False（原文 32899-32900）
        int addCalls = 0;
        PlayerSurfaceBaseSeams.AddToMap = (_, _, _, _) => { addCalls++; return true; };
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 1, m_nCurrY = 1 };

        p.Initialize();

        Assert.False(p.m_boAddtoMapFail);
        Assert.Equal(1, addCalls);
    }

    [Fact]
    public void Initialize_NoEnvir_DoesNotCallAddToMap()
    {
        int addCalls = 0;
        PlayerSurfaceBaseSeams.AddToMap = (_, _, _, _) => { addCalls++; return true; };
        var p = new TPlayObject { m_PEnvir = null };
        p.Initialize();
        Assert.Equal(0, addCalls);
        Assert.True(p.m_boAddtoMapFail);
    }

    [Fact]
    public void Initialize_OutOfMapBounds_ShortCircuitsAndKeepsFailTrue()
    {
        // ★ 原文 32899 是 `CanWalk(...) and AddToMap()` 的**短路与**：
        //   坐标越界时 CanWalk 为假 → AddToMap **根本不会被调用**。
        var env = MakeEnv(4, 4);
        int addCalls = 0;
        PlayerSurfaceBaseSeams.AddToMap = (_, _, _, _) => { addCalls++; return true; };
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 99, m_nCurrY = 99 };

        p.Initialize();

        Assert.True(p.m_boAddtoMapFail);
        Assert.Equal(0, addCalls);
    }

    [Fact]
    public void Initialize_AddToMapFails_KeepsAddToMapFailTrue()
    {
        var env = new TEnvirnoment();
        PlayerSurfaceBaseSeams.AddToMap = (_, _, _, _) => false;
        var p = new TPlayObject { m_PEnvir = env, m_nCurrX = 1, m_nCurrY = 1 };

        p.Initialize();

        Assert.True(p.m_boAddtoMapFail);
    }

    [Fact]
    public void Initialize_CallsCharStatusBodyLuckAndLoadSayMsg_InOrder()
    {
        // 原文 32901-32903 的调用顺序
        var order = new System.Collections.Generic.List<string>();
        PlayerSurfaceBaseSeams.GetCharStatus = _ => { order.Add("status"); return 77; };
        PlayerSurfaceBaseSeams.AddBodyLuck = (_, n) => order.Add("luck" + n);
        PlayerSurfaceBaseSeams.LoadSayMsg = _ => order.Add("say");

        var p = new TPlayObject { m_PEnvir = null };
        p.Initialize();

        Assert.Equal(new[] { "status", "luck0", "say" }, order);
        Assert.Equal(77, p.m_nCharStatus);
    }

    [Fact]
    public void Initialize_MonsterSayMsgOnlyWhenConfigEnabled()
    {
        // 原文 32904：if g_Config.boMonSayMsg then MonsterSayMsg(nil, s_MonGen);
        bool saved = M2Config.boMonSayMsg;
        try
        {
            int calls = 0;
            PlayerSurfaceBaseSeams.MonsterSayMsg = _ => calls++;

            M2Config.boMonSayMsg = false;
            new TPlayObject { m_PEnvir = null }.Initialize();
            Assert.Equal(0, calls);

            M2Config.boMonSayMsg = true;
            new TPlayObject { m_PEnvir = null }.Initialize();
            Assert.Equal(1, calls);
        }
        finally
        {
            M2Config.boMonSayMsg = saved;
        }
    }

    [Fact]
    public void Initialize_AddToMapNotCalled_WhenEnvirNull()
    {
        int calls = 0;
        PlayerSurfaceBaseSeams.AddToMap = (_, _, _, _) => { calls++; return true; };
        new TPlayObject { m_PEnvir = null }.Initialize();
        Assert.Equal(0, calls);
    }

    // ---------------------------------------------------------------
    // 常量
    // ---------------------------------------------------------------

    [Fact]
    public void Consts_MatchOriginalGrobal2()
    {
        Assert.Equal(1, PlayerSurfaceConst.U_WEAPON);          // Grobal2.pas:102
        Assert.Equal(30, PlayerSurfaceConst.MAX_USE_ITEM_COUNT); // Grobal2.pas:51
        Assert.Equal(10, TActorIconArrayConst.MAX_ICON_COUNT);   // Grobal2.pas:50
    }

    /// <summary>虚分派探针：覆写 Initialize / RecalcAbilitys 并记录调用。</summary>
    private sealed class ProbePlayer : TPlayObject
    {
        public int InitializeCalls;
        public int RecalcCalls;

        public override void Initialize()
        {
            InitializeCalls++;
            base.Initialize();
        }

        public override void RecalcAbilitys()
        {
            RecalcCalls++;
            base.RecalcAbilitys();
        }
    }
}
