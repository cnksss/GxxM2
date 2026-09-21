using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 `p17-client-actor` 切片 1：`Actor.pas` **NPC 族**
/// （<c>TNpcActor</c> 9936-11122 / <c>TStatuaryNpcActor</c> 17537-18008 / <c>THeroActor</c> 17437-17533）
/// 的 1:1 回归证据。
///
/// <para><b>被测对象</b>：<c>TNpcActor</c> / <c>TStatuaryNpcActor</c> / <c>THeroActor</c>
/// —— 三者此前在 <c>PlaySceneNewActor.cs</c> 里是**一行空壳**（只有 <c>ActorClass</c>），
/// 本切片把它们迁到 <c>Scenes/Actor*.cs</c> 并写真实现。</para>
///
/// <para><b>断言口径</b>：全部为**结构性**判据 —— 字段被改成什么、取图接缝收到什么参数、
/// 绘制操作以什么**顺序**产生、上行消息发出什么实参。三处最值得记住的"看起来一样实则不同"：</para>
/// <list type="number">
/// <item><c>CalcActorFrame</c> 在 <c>NpcDirAction &lt;&gt; nil</c> 时是 <b>Exit</b>（跳过后续全局表逻辑），
///   而非 <b>Break</b>；</item>
/// <item><c>DrawChr</c> 的六种 <c>DrawOrder</c> 是**六种不同顺序**，不是同一顺序的写法；</item>
/// <item><c>GetDefaultFrame</c> 的"方向归零外观集合"比 <c>CalcActorFrame</c> 的**少了 245**。</item>
/// </list>
/// </summary>
public sealed class ActorNpcActorTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorNpcActorTests()
    {
        ActorNpcEnv.Reset();
        _restore.Add(ActorNpcEnv.Reset);

        ActorNpcEnv.CanvasReadyFn = () => true;
        _restore.Add(() => ActorNpcEnv.CanvasReadyFn = () => false);

        ActorNpcEnv.TimeGetTimeFn = () => 1000;
        _restore.Add(() => ActorNpcEnv.TimeGetTimeFn = () => SceneTime.TickNow());

        ActorNpcEnv.MyGetTickCountFn = () => 500;
        _restore.Add(() => ActorNpcEnv.MyGetTickCountFn = () => SceneTime.TickNow());

        ActorFamilyEnv.Reset();
        _restore.Add(ActorFamilyEnv.Reset);
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ══════════════════════════════════════════════════════════════════════
    // 构造工具
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`m_btRace = 20` 命中 `MA19`（ActorActionTables.g.cs:170）：
    /// ActStand(start=0,frame=4,skip=6,ftime=200)、ActAttack(start=160,frame=6,skip=4,ftime=100)、
    /// ActCritical(start=0,frame=0,skip=0,ftime=0)。</summary>
    private static TNpcActor NewNpc(ushort appearance = 100, byte race = 20, byte dir = 0)
        => new() { m_wAppearance = appearance, m_btRace = race, m_btDir = dir };

    private static TNpcDirAction DirAction(
        int stdFile = -1, short stdIndex = 10, ushort stdCount = 4, ushort stdTime = 200, short stdEffIndex = -1, ushort stdEffFile = 0,
        int actFile = -1, short actIndex = 30, ushort actCount = 5, ushort actTime = 100, short actEffIndex = -1, ushort actEffFile = 0)
        => new()
        {
            Enabled = 1,
            Std_File = (ushort)stdFile,
            Std_Index = stdIndex,
            Std_Count = stdCount,
            Std_Time = stdTime,
            Std_EffFile = stdEffFile,
            Std_EffIndex = stdEffIndex,
            Act_File = (ushort)actFile,
            Act_Index = actIndex,
            Act_Count = actCount,
            Act_Time = actTime,
            Act_EffFile = actEffFile,
            Act_EffIndex = actEffIndex,
        };

    /// <summary>构造一份自定义 NPC 配置；<paramref name="dirs"/> 按方向下标填入 <c>Actions</c>。</summary>
    private static TClientCustomNpcConfig MakeConfig(
        ushort npcAppr, ushort dirCount, TNpcBaseConfig? baseConfig = null,
        params (int Dir, TNpcDirAction Action)[] dirs)
    {
        var cfg = new TClientCustomNpcConfig
        {
            wNpcAppr = npcAppr,
            wDirCount = dirCount,
            BaseConfig = baseConfig ?? new TNpcBaseConfig(),
        };

        foreach (var (dir, action) in dirs)
            cfg.Actions[dir] = action;

        return cfg;
    }

    private static void UseConfig(TClientCustomNpcConfig cfg)
        => ActorNpcEnv.CustomNpcConfigLookupFn = appearance => appearance == cfg.wNpcAppr ? cfg : null;

    // ══════════════════════════════════════════════════════════════════════
    // 1. TNpcActor.CalcActorFrame（9936-10229）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void CalcActorFrame_AlwaysClearsMagicFlagAndFrame()
    {
        var npc = NewNpc();
        npc.m_boUseMagic = true;
        npc.m_nCurrentFrame = 42;
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.False(npc.m_boUseMagic);     // 9943
        Assert.Equal(-1, npc.m_nCurrentFrame);   // 9944：SM_TURN 分支不再动它
    }

    [Fact]
    public void CalcActorFrame_GlobalPathUsesNpcOffsetAndActStand()
    {
        var npc = NewNpc(appearance: 300, dir: 2);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        // 9964：GetNpcOffset(300)
        Assert.Equal(ActorOffsets.GetNpcOffset(300), npc.m_nBodyOffset);
        // 9977：外观不在 246..272 ⇒ m_btDir mod 3（2 -> 2）
        Assert.Equal(2, npc.m_btDir);
        // 10013-10018：MA19.ActStand(0,4,6) ⇒ start = 0 + 2*10 = 20
        Assert.Equal(20, npc.m_nStartFrame);
        Assert.Equal(23, npc.m_nEndFrame);
        Assert.Equal(200u, npc.m_dwFrameTime);
        Assert.Equal(4, npc.m_nDefFrameCount);
        Assert.Equal(1000u, npc.m_dwStartTime);
        Assert.Equal(1000u, npc.m_StartCounter);    // 10017
    }

    [Fact]
    public void CalcActorFrame_AppearanceInWhiteListForcesDirectionZero()
    {
        var npc = NewNpc(appearance: 245, dir: 2);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.Equal(0, npc.m_btDir);       // 9978：245 在归零集合里（★ CalcActorFrame 含 245）
    }

    [Fact]
    public void CalcActorFrame_Appearance244And245EnableEffect()
    {
        foreach (var appr in new ushort[] { 244, 245 })
        {
            var npc = NewNpc(appearance: appr);
            npc.m_boUseEffect = false;
            npc.m_nCurrentAction = TNpcActor.SM_TURN;

            npc.CalcActorFrame();

            Assert.True(npc.m_boUseEffect);   // 9969 / 9971
        }
    }

    [Fact]
    public void CalcActorFrame_CustomNpcDirCountModsDirectionBeforeIndexing()
    {
        const ushort appr = 10001;
        var cfg = MakeConfig(appr, 3, null,
            (1, DirAction(stdIndex: 90, stdCount: 2, stdTime: 111)));
        UseConfig(cfg);

        var npc = NewNpc(appearance: appr, dir: 7);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.Equal(0, npc.m_nBodyOffset);      // 9949：自定义 NPC 路径**强制 0**
        Assert.Equal(1, npc.m_btDir);            // 9958：7 mod 3 = 1 → 命中 Actions[1]
        Assert.Equal(90, npc.m_nStartFrame);     // 9986
        Assert.Equal(91, npc.m_nEndFrame);       // 9987
        Assert.Equal(111u, npc.m_dwFrameTime);   // 9988
    }

    [Fact]
    public void CalcActorFrame_CustomNpcWithoutConfigLeavesFramesUnset()
    {
        // 外观 >= 10000 但配置查找返回 null ⇒ NpcDirAction 保持 nil ⇒ 落到全局表的 SM_TURN
        var npc = NewNpc(appearance: 10004, dir: 0);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.Equal(0, npc.m_nBodyOffset);     // 9949 已执行
        Assert.Equal(0, npc.m_nStartFrame);     // 10013：全局 MA19.ActStand.start
    }

    [Fact]
    public void CalcActorFrame_CustomNpcUsesDirActionAndDoesNotFallThroughToGlobalTable()
    {
        const ushort appr = 10002;
        var cfg = MakeConfig(appr, 0, null, (0, DirAction(stdIndex: 70, stdCount: 3, stdTime: 250)));
        UseConfig(cfg);

        var npc = NewNpc(appearance: appr, dir: 5);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.Equal(0, npc.m_btDir);                // 9960：wDirCount <= 1 ⇒ 0
        Assert.Equal(70, npc.m_nStartFrame);         // 9986
        Assert.Equal(72, npc.m_nEndFrame);           // 9987
        Assert.Equal(250u, npc.m_dwFrameTime);       // 9988
        Assert.Equal(3, npc.m_nDefFrameCount);       // 9991
        // ★ Exit（10010）而非 Break：全局表不会覆盖，故 frame 仍是动作表的值
    }

    [Fact]
    public void CalcActorFrame_CustomNpcWalkSetsMoveParamsThenExits()
    {
        const ushort appr = 10003;
        var cfg = MakeConfig(appr, 0, null, (0, DirAction(actIndex: 40, actCount: 6, actTime: 90)));
        UseConfig(cfg);

        var npc = NewNpc(appearance: appr);
        npc.m_nCurrentAction = TNpcActor.SM_WALK;

        npc.CalcActorFrame();

        Assert.Equal(40, npc.m_nStartFrame);
        Assert.Equal(45, npc.m_nEndFrame);
        Assert.Equal(90u, npc.m_dwFrameTime);
        Assert.Equal(1, npc.m_nMaxTick);      // 10221
        Assert.Equal(0, npc.m_nCurTick);      // 10222
        Assert.Equal(1, npc.m_nMoveStep);     // 10223
        Assert.Equal(0, npc.m_nShiftX);       // Shift(dir=0, step=1, cur=0, max=6)：DR_UP 且 ss==max-cur? 
    }

    [Fact]
    public void CalcActorFrame_Appearance33And34TurnSetsEffect30to39()
    {
        var npc = NewNpc(appearance: 33);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.True(npc.m_boUseEffect);       // 10021
        Assert.Equal(30, npc.m_nEffectStart); // 10022
        Assert.Equal(30, npc.m_nEffectFrame); // 10023
        Assert.Equal(39, npc.m_nEffectEnd);   // 10024
        Assert.Equal(300u, npc.m_dwEffectFrameTime);  // 10026
    }

    [Fact]
    public void CalcActorFrame_Appr42to47TurnHasInvertedFrameRange()
    {
        var npc = NewNpc(appearance: 45);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        // ★ 原文如此（10031-10032）：StartFrame=20 > EndFrame=10
        Assert.Equal(20, npc.m_nStartFrame);
        Assert.Equal(10, npc.m_nEndFrame);
        Assert.True(npc.m_nStartFrame > npc.m_nEndFrame);
        Assert.True(npc.m_boUseEffect);
        Assert.Equal(0, npc.m_nEffectStart);
        Assert.Equal(19, npc.m_nEffectEnd);
        Assert.Equal(100u, npc.m_dwEffectFrameTime);
    }

    [Theory]
    [InlineData((ushort)51, 60, 67, 500u)]
    [InlineData((ushort)100, 10, 21, 100u)]
    [InlineData((ushort)218, 10, 25, 150u)]
    [InlineData((ushort)221, 20, 28, 250u)]
    [InlineData((ushort)222, 10, 18, 250u)]
    [InlineData((ushort)224, 10, 25, 150u)]
    public void CalcActorFrame_TurnEffectTable(ushort appearance, int effStart, int effEnd, uint frameTime)
    {
        var npc = NewNpc(appearance: appearance);
        npc.m_nCurrentAction = TNpcActor.SM_TURN;

        npc.CalcActorFrame();

        Assert.True(npc.m_boUseEffect);
        Assert.Equal(effStart, npc.m_nEffectStart);
        Assert.Equal(effStart, npc.m_nEffectFrame);
        Assert.Equal(effEnd, npc.m_nEffectEnd);
        Assert.Equal(frameTime, npc.m_dwEffectFrameTime);
    }

    [Fact]
    public void CalcActorFrame_Appearance51HitAlsoSetsEffect()
    {
        var npc = NewNpc(appearance: 51);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;

        npc.CalcActorFrame();

        Assert.Equal(160, npc.m_nStartFrame);      // 10164：MA19.ActAttack.start
        Assert.Equal(165, npc.m_nEndFrame);        // 10165
        Assert.Equal(100u, npc.m_dwFrameTime);
        Assert.True(npc.m_boUseEffect);            // 10169-10176
        Assert.Equal(60, npc.m_nEffectStart);
        Assert.Equal(67, npc.m_nEffectEnd);
        Assert.Equal(500u, npc.m_dwEffectFrameTime);
    }

    [Fact]
    public void CalcActorFrame_Appearance33HitUsesActStandAndKeepsFrameTime()
    {
        var npc = NewNpc(appearance: 33);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;
        npc.m_dwFrameTime = 4242;      // 10122-10128 分支**不写** m_dwFrameTime

        npc.CalcActorFrame();

        Assert.Equal(0, npc.m_nStartFrame);        // MA19.ActStand.start + dir*skip
        Assert.Equal(3, npc.m_nEndFrame);
        Assert.Equal(4, npc.m_nDefFrameCount);     // 10127
        Assert.Equal(4242u, npc.m_dwFrameTime);    // ★ 原文不覆盖
    }

    [Fact]
    public void CalcActorFrame_Appearance84HitTakesActAttackWhenStartFrameIsZero()
    {
        var npc = NewNpc(appearance: 84);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;
        npc.m_nStartFrame = 0;

        npc.CalcActorFrame();

        // 10130：`m_nStartFrame <= 0` 为真 ⇒ ActAttack（160..165）
        Assert.Equal(160, npc.m_nStartFrame);
        Assert.Equal(165, npc.m_nEndFrame);
        Assert.Equal(100u, npc.m_dwFrameTime);
    }

    [Fact]
    public void CalcActorFrame_Appearance84HitAlwaysTakesActAttackForMA19()
    {
        // ★ 原文如此：MA19.ActCritical = (start=0, frame=0, skip=0)，
        //   故 `m_nStartFrame >= 0 + dir*(0+0)` 对任何 m_nStartFrame >= 0 都真；
        //   与 `m_nStartFrame <= 0` 的或关系 ⇒ 10137 的 else（ActCritical 帧）**永不执行**。
        var npc = NewNpc(appearance: 84);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;
        npc.m_nStartFrame = 999;

        npc.CalcActorFrame();

        Assert.Equal(160, npc.m_nStartFrame);   // 仍是 ActAttack，不是 ActCritical
        Assert.Equal(6, npc.m_nEndFrame - npc.m_nStartFrame + 1);   // ActAttack.frame = 6
    }

    [Fact]
    public void CalcActorFrame_Appearance210HitUsesSingleStepNotDirection()
    {
        var npc = NewNpc(appearance: 210, dir: 3);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;

        npc.CalcActorFrame();

        // 10147：`pm.ActAttack.start + (frame + skip)` —— **没有乘 dir**
        Assert.Equal(160 + (6 + 4), npc.m_nStartFrame);
        Assert.Equal(175, npc.m_nEndFrame);
    }

    [Fact]
    public void CalcActorFrame_Appearance226to272WithNonZeroAttackFramesTakesElseBranch()
    {
        // 10160 的判据要求 ActAttack.frame = 0；MA19.ActAttack.frame = 6 ⇒ 走 else 设帧。
        var npc = NewNpc(appearance: 230, dir: 0);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;
        npc.m_nStartFrame = 777;

        npc.CalcActorFrame();

        Assert.Equal(160, npc.m_nStartFrame);
    }

    [Fact]
    public void CalcActorFrame_Appearance52DigUpStarts23SecondLoop()
    {
        var npc = NewNpc(appearance: 52);
        npc.m_nCurrentAction = TNpcActor.SM_DIGUP;

        var sounds = new List<int>();
        ActorNpcEnv.PlaySoundByIdFn = sounds.Add;
        ActorNpcEnv.RandomFn = _ => 3;

        npc.CalcActorFrame();

        Assert.True(npc.m_bo248);                       // 10184
        Assert.Equal(1000u + 23000, npc.m_dwUseEffectTick);   // 10185
        Assert.Equal(new[] { 3 + 146 }, sounds);        // 10187：Random(7) + 146
        Assert.True(npc.m_boUseEffect);                 // 10188
        Assert.Equal(60, npc.m_nEffectStart);
        Assert.Equal(71, npc.m_nEffectEnd);             // 10191：+ 11（**不是 +11-1**）
        Assert.Equal(100u, npc.m_dwEffectFrameTime);
    }

    [Fact]
    public void CalcActorFrame_DigUpIsNoOpForOtherAppearances()
    {
        var npc = NewNpc(appearance: 99);
        npc.m_nCurrentAction = TNpcActor.SM_DIGUP;
        npc.m_bo248 = false;
        npc.m_nEffectFrame = 123;

        npc.CalcActorFrame();

        Assert.False(npc.m_bo248);
        Assert.Equal(123, npc.m_nEffectFrame);
    }

    [Fact]
    public void CalcActorFrame_WalkWithoutNpcDirActionIsNoOp()
    {
        var npc = NewNpc(appearance: 99);
        npc.m_nCurrentAction = TNpcActor.SM_WALK;
        npc.m_nMaxTick = 55;
        npc.m_nMoveStep = 9;

        npc.CalcActorFrame();

        Assert.Equal(55, npc.m_nMaxTick);   // 10196-10227 只在 NpcDirAction <> nil 时写
        Assert.Equal(9, npc.m_nMoveStep);
    }

    [Fact]
    public void CalcActorFrame_UnknownActionLeavesFramesAtMinusOne()
    {
        var npc = NewNpc(appearance: 99);
        npc.m_nCurrentAction = 987654;

        npc.CalcActorFrame();

        Assert.Equal(-1, npc.m_nCurrentFrame);   // 9944 之后四个 case 都不命中
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. TNpcActor.Create / Initialize / Finalize（10231-10264）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Create_FieldDefaultsAreClearedValuesForPlainNpc()
    {
        // 10236-10239 的四句写入与字段**默认值重合**，故单独不可观测；
        // 此处锁的是"新建 TNpcActor 的四元组就是 10236-10239 写下的值"。
        var npc = new TNpcActor();

        Assert.Null(npc.m_EffSurface);     // 10236
        Assert.Null(npc.m_KeepSurface);    // 10237
        Assert.False(npc.m_boHitEffect);   // 10238
        Assert.False(npc.m_bo248);         // 10239
    }

    [Fact]
    public void Create_CustomNpcKeepPlayBranchCannotObserveAppearanceAtConstructionTime()
    {
        // ★ 原文缺陷（照抄并锁死）：10241 的门是 `m_wAppearance >= 10000`，
        //   而 PlayScn.NewActor 的次序是 **先 Create、后写形象字段**（本工程
        //   PlaySceneNewActor.cs:174 构造 → :266 写 m_wAppearance；与原文一致）。
        //   因此在真实流程里构造期 m_wAppearance 恒为 0 ⇒ 10241 的分支**永不进入**，
        //   10247 的 m_nKeepFrame 播种也就永不发生。
        const ushort appr = 10010;
        var bc = new TNpcBaseConfig
        {
            KeepPlayFile = 2,
            KeepPlayIndex = 7,
            KeepPlayCount = 4,
            KeepPlayTime = 150,
        };
        UseConfig(MakeConfig(appr, 1, bc));
        ActorNpcEnv.EffectImageListCountFn = () => 10;

        var npc = new TNpcActor { m_wAppearance = appr };   // 形象字段在构造**之后**才写

        Assert.Equal(0, npc.m_nKeepFrame);            // 10247 未执行
        Assert.Equal(0u, npc.m_LastKeepPlayTick);     // 10248 未执行
        Assert.Equal(appr, npc.m_wAppearance);
    }

    [Fact]
    public void Finalize_ClearsBothSurfaceSlots()
    {
        var npc = new TNpcActor { m_EffSurface = "a", m_KeepSurface = "b" };

        npc.Finalize();

        Assert.Null(npc.m_EffSurface);    // 10262
        Assert.Null(npc.m_KeepSurface);   // 10263
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3. TNpcActor.CheckLoadUserName（10266-10295）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void CheckLoadUserName_CanvasNotReadyReturnsFalseAndKeepsPreviousName()
    {
        ActorNpcEnv.CanvasReadyFn = () => false;
        var npc = new TNpcActor { m_sNameText = "KEEP", m_sUserName = "npc" };

        Assert.False(npc.CheckLoadUserName());
        Assert.Equal("KEEP", npc.m_sNameText);   // 10269 在 10270 之前 Exit
    }

    [Fact]
    public void CheckLoadUserName_ShowsDescAndNameWhenNpcNameOptionOn()
    {
        ActorNpcEnv.ClientConfigBoShowNpcName = true;
        ActorNpcEnv.ConfigDlgCkShowNpcName = true;
        var npc = new TNpcActor { m_sUserName = "商人", m_sDescUserName = "高级" };

        Assert.True(npc.CheckLoadUserName());       // 上一轮是 '' ⇒ 有变化
        Assert.Equal("高级\\商人", npc.m_sNameText);   // 10277
    }

    [Fact]
    public void CheckLoadUserName_OptionOnButDeadFallsThroughToFocusCheck()
    {
        ActorNpcEnv.ClientConfigBoShowNpcName = true;
        ActorNpcEnv.ConfigDlgCkShowNpcName = true;
        ActorNpcEnv.IsFocusActorFn = a => true;
        var npc = new TNpcActor { m_sUserName = "商人", m_sDescUserName = "高级", m_boDeath = true };

        Assert.True(npc.CheckLoadUserName());
        Assert.Equal("高级\\商人", npc.m_sNameText);   // 10279-10280（焦点分支）
    }

    [Fact]
    public void CheckLoadUserName_HiddenNameIsNeverShown()
    {
        ActorNpcEnv.ClientConfigBoShowNpcName = true;
        ActorNpcEnv.ConfigDlgCkShowNpcName = true;
        ActorNpcEnv.IsFocusActorFn = a => true;
        var npc = new TNpcActor { m_sUserName = "不显名", m_sDescUserName = "X" };

        Assert.False(npc.CheckLoadUserName());     // 名字文本保持 '' ⇒ 无变化
        Assert.Equal("", npc.m_sNameText);
    }

    [Fact]
    public void CheckLoadUserName_PlugInOffOnlyUsesFocus()
    {
        ActorNpcEnv.PlugInEnabled = false;
        ActorNpcEnv.IsFocusActorFn = a => true;
        var npc = new TNpcActor { m_sUserName = "商人", m_sDescUserName = "高级" };

        Assert.True(npc.CheckLoadUserName());
        Assert.Equal("高级\\商人", npc.m_sNameText);   // 10288
    }

    [Fact]
    public void CheckLoadUserName_PlugInOffAndNotFocusedLeavesEmpty()
    {
        ActorNpcEnv.PlugInEnabled = false;
        var npc = new TNpcActor { m_sUserName = "商人", m_sDescUserName = "高级" };

        Assert.False(npc.CheckLoadUserName());
        Assert.Equal("", npc.m_sNameText);
    }

    [Fact]
    public void CheckLoadUserName_GhostHideGateSuppressesOnlyWhenBothSwitchesAndDead()
    {
        ActorNpcEnv.ClientConfigBoShowNpcName = true;
        ActorNpcEnv.ConfigDlgCkShowNpcName = true;
        ActorNpcEnv.IsFocusActorFn = a => true;         // 焦点分支可用，否则无法区分
        var npc = new TNpcActor { m_sUserName = "商人", m_sDescUserName = "高级", m_boDeath = true };

        // 只开一个开关 ⇒ 10273 的三重与为假 ⇒ 名字照常显示
        ActorNpcEnv.ClientConfigBoHideGhost = true;
        ActorNpcEnv.ConfigDlgCkHideGhost = false;
        Assert.True(npc.CheckLoadUserName());
        Assert.Equal("高级\\商人", npc.m_sNameText);

        // 两个开关都开 + 已死 ⇒ 10273 为真 ⇒ 整块跳过 ⇒ 名字保持空
        npc.m_sCurNameText = "";
        ActorNpcEnv.ConfigDlgCkHideGhost = true;
        Assert.False(npc.CheckLoadUserName());
        Assert.Equal("", npc.m_sNameText);
    }

    [Fact]
    public void CheckLoadUserName_ReturnsFalseWhenNameUnchanged()
    {
        ActorNpcEnv.ClientConfigBoShowNpcName = true;
        ActorNpcEnv.ConfigDlgCkShowNpcName = true;
        var npc = new TNpcActor { m_sUserName = "商人", m_sDescUserName = "高级" };

        Assert.True(npc.CheckLoadUserName());
        npc.m_sCurNameText = npc.m_sNameText;
        Assert.False(npc.CheckLoadUserName());     // 10294：与上一轮相同
    }

    [Fact]
    public void CheckLoadUserName_ComparisonIsCaseInsensitive()
    {
        // CompareText（大小写不敏感）而非 CompareStr
        ActorNpcEnv.ClientConfigBoShowNpcName = true;
        ActorNpcEnv.ConfigDlgCkShowNpcName = true;
        var npc = new TNpcActor { m_sUserName = "NPC", m_sDescUserName = "" };

        Assert.True(npc.CheckLoadUserName());
        npc.m_sCurNameText = "\\npc";              // 仅大小写不同
        Assert.False(npc.CheckLoadUserName());
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4. TNpcActor.DrawChr（10297-10422）
    // ══════════════════════════════════════════════════════════════════════

    private static List<NpcDrawOp> Record(TNpcActor npc, int dx = 100, int dy = 200, bool blend = false)
    {
        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;
        npc.DrawChr(dx, dy, blend, false);
        return ops;
    }

    /// <summary>
    /// 统一记录**绘制层序**：<c>NpcDrawFn</c>（Eff/Keep/外观 54..58 的主体）与
    /// <c>ActorFamilyEnv.DrawEffSurfaceOpFn</c>（走 <c>DrawEffSurface</c> 的主体）汇成一条序列。
    /// <para>背景：<c>TNpcActor.DrawChr</c> 的主体图有**两个不同出口** ——
    /// 外观 54..58 直接 <c>GameCanvas.DrawBlend</c>（走 NpcDrawFn），
    /// 其余走 <c>DrawEffSurface</c>（走既有 <c>DrawEffSurfaceOpFn</c> 接缝）。
    /// 只挂一个接缝会把顺序看错。</para>
    /// </summary>
    private static List<string> RecordLayerOrder(TNpcActor npc, int dx = 100, int dy = 200, bool blend = false)
    {
        var seq = new List<string>();
        ActorNpcEnv.NpcDrawFn = op => seq.Add(op.Layer);
        ActorFamilyEnv.DrawEffSurfaceOpFn = _ => seq.Add("Body");
        ActorFamilyEnv.CanvasDrawOpFn = _ => { };      // DrawStateEffSurface 不参与层序
        npc.DrawChr(dx, dy, blend, false);
        return seq;
    }

    [Fact]
    public void DrawChr_GlobalPathWithNothingLoadedDrawsNothing()
    {
        var npc = NewNpc(appearance: 300);
        Assert.Empty(Record(npc));
    }

    [Fact]
    public void DrawChr_Appearance54to58ForcesDrawBlendRegardlessOfBlendArgument()
    {
        var npc = NewNpc(appearance: 56);
        npc.m_BodySurface = "body";

        var ops = Record(npc, blend: false);

        var op = Assert.Single(ops);
        Assert.Equal(NpcDrawKind.DrawBlend, op.Kind);   // 10393
        Assert.Equal("Body", op.Layer);
        Assert.Equal(10393, op.ReferenceLine);
    }

    [Fact]
    public void DrawChr_Appearance51ForcesBlendOnDrawEffSurface()
    {
        var npc = NewNpc(appearance: 51);
        npc.m_BodySurface = "body";

        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;

        npc.DrawChr(10, 20, false, false);

        var op = Assert.Single(ops);
        // ★ 10399-10404 传的是字面 True ⇒ 强制混合 ⇒ 无 bodyColor 时落 DrawColorAlpha(White, 150)
        Assert.Equal(SurfaceDrawKind.DrawColorAlpha, op.Kind);
        Assert.Equal(150, op.Alpha);
        Assert.Equal(10, op.X);      // dx + m_nPx(0) + m_nShiftX(0)
        Assert.Equal(20, op.Y);
    }

    [Fact]
    public void DrawChr_NonForcedAppearanceHonoursBlendArgument()
    {
        var npc = NewNpc(appearance: 300);
        npc.m_BodySurface = "body";

        var ops = new List<SurfaceDrawOp>();
        ActorFamilyEnv.DrawEffSurfaceOpFn = ops.Add;

        npc.DrawChr(10, 20, false, false);
        Assert.Equal(SurfaceDrawKind.Draw, Assert.Single(ops).Kind);   // blend=false ⇒ Draw

        ops.Clear();
        npc.DrawChr(10, 20, true, false);
        Assert.Equal(SurfaceDrawKind.DrawColorAlpha, Assert.Single(ops).Kind);   // blend=true ⇒ 混合
    }

    [Fact]
    public void DrawChr_GlobalPathNormalizesDirectionMod3()
    {
        var npc = NewNpc(appearance: 300, dir: 7);
        npc.m_BodySurface = "b";

        Record(npc);

        Assert.Equal(1, npc.m_btDir);          // 10390：7 mod 3
    }

    [Fact]
    public void DrawChr_GlobalPathDoesNotNormalizeDirectionForAppearance246to272()
    {
        var npc = NewNpc(appearance: 250, dir: 7);
        npc.m_BodySurface = "b";

        Record(npc);

        Assert.Equal(7, npc.m_btDir);          // 10389：区间内不动
    }

    [Fact]
    public void DrawChr_EffWhitelistDrawsEffLayer()
    {
        var npc = NewNpc(appearance: 90);
        npc.m_BodySurface = "b";
        npc.m_EffSurface = "e";

        var ops = Record(npc);

        var op = Assert.Single(ops);                   // 主体走 DrawEffSurface，不在本接缝
        Assert.Equal("Eff", op.Layer);                 // 10415-10420
        Assert.Equal(NpcDrawKind.DrawBlend, op.Kind);
        Assert.Equal(10416, op.ReferenceLine);
    }

    [Fact]
    public void DrawChr_EffOutsideWhitelistIsNotDrawn()
    {
        var npc = NewNpc(appearance: 300);
        npc.m_BodySurface = "b";
        npc.m_EffSurface = "e";

        var ops = Record(npc);

        Assert.Empty(ops);                    // 外观 300 不在 10415 白名单
    }

    /// <summary>
    /// ★ 六种 <c>DrawOrder</c> 的**实际绘制顺序**（10355-10385）。
    ///
    /// <para><b>原文缺陷（锁死）</b>：枚举标识符的读序与**执行序恰好相反** ——
    /// 如 <c>ndoKeep_Chr_Eff</c> 的代码是 <c>DrawEffect; DrawBody; DrawKeep;</c>（Eff→Chr→Keep），
    /// 而不是标识符暗示的 Keep→Chr→Eff。六条**全部**成立（见
    /// <see cref="DrawChr_CustomNpcDrawOrderIdentifierIsReverseOfExecutionOrder"/>）。
    /// 「按名字直译」会画出完全相反的三层覆盖关系，而肉眼在正常情况下很难分辨。</para>
    /// </summary>
    [Theory]
    [InlineData((ushort)10001, "Effect", "Body", "Keep")]   // ndoKeep_Chr_Eff
    [InlineData((ushort)10002, "Body", "Effect", "Keep")]   // ndoKeep_Eff_Chr
    [InlineData((ushort)10003, "Effect", "Keep", "Body")]   // ndoChr_Keep_Eff
    [InlineData((ushort)10004, "Keep", "Effect", "Body")]   // ndoChr_Eff_Keep
    [InlineData((ushort)10005, "Body", "Keep", "Effect")]   // ndoEff_Keep_Chr
    [InlineData((ushort)10006, "Keep", "Body", "Effect")]   // ndoEff_Chr_Keep
    public void DrawChr_CustomNpcSixDrawOrders(ushort appr, string first, string second, string third)
    {
        var bc = new TNpcBaseConfig
        {
            DrawOrder = (TCustomNpcDrawOrder)(appr - 10001),
            KeepPlayBlendDraw = 1,
        };
        UseConfig(MakeConfig(appr, 1, bc));

        var npc = NewNpc(appearance: appr);
        npc.m_BodySurface = "body";
        npc.m_EffSurface = "eff";
        npc.m_KeepSurface = "keep";

        var seq = RecordLayerOrder(npc);

        Assert.Equal(3, seq.Count);
        Assert.Equal(first, seq[0]);
        Assert.Equal(second, seq[1]);
        Assert.Equal(third, seq[2]);
    }

    [Fact]
    public void DrawChr_CustomNpcDrawOrderIdentifierIsReverseOfExecutionOrder()
    {
        // ★ 把"枚举名字的读序"与"实际执行序"逐条对撞：六条**全部**互为倒序。
        var nameToLayer = new Dictionary<string, string>
        {
            ["Keep"] = "Keep",
            ["Chr"] = "Body",
            ["Eff"] = "Effect",
        };

        for (int i = 0; i < 6; i++)
        {
            ushort appr = (ushort)(10001 + i);
            var bc = new TNpcBaseConfig
            {
                DrawOrder = (TCustomNpcDrawOrder)i,
                KeepPlayBlendDraw = 0,
            };
            UseConfig(MakeConfig(appr, 1, bc));

            var npc = NewNpc(appearance: appr);
            npc.m_BodySurface = "body";
            npc.m_EffSurface = "eff";
            npc.m_KeepSurface = "keep";

            var seq = RecordLayerOrder(npc);

            var parts = ((TCustomNpcDrawOrder)i).ToString().Replace("ndo", "").Split('_');
            var nameOrder = parts.Select(p => nameToLayer[p]).ToArray();
            nameOrder = nameOrder.Reverse().ToArray();

            Assert.Equal(nameOrder, seq);
        }
    }

    [Fact]
    public void DrawChr_CustomNpcWithoutConfigDrawsNothing()
    {
        var npc = NewNpc(appearance: 10009);
        npc.m_BodySurface = "body";

        Assert.Empty(Record(npc));    // 10353：NpcConfig = nil ⇒ 整段不画（且**不**落 else）
    }

    [Fact]
    public void DrawChr_CustomNpcDoorsUseKeepOffsetAndBlend()
    {
        const ushort appr = 10020;
        var bc = new TNpcBaseConfig
        {
            DrawOrder = TCustomNpcDrawOrder.ndoKeep_Chr_Eff,
            KeepPlayBlendDraw = 1,
            KeepPlayOffsetX = 5,
            KeepPlayOffsetY = -7,
        };
        UseConfig(MakeConfig(appr, 1, bc));

        var npc = NewNpc(appearance: appr);
        npc.m_KeepSurface = "keep";
        npc.m_nKeepX = 3;
        npc.m_nKeepY = 4;
        npc.m_nShiftX = 11;
        npc.m_nShiftY = 12;

        var ops = Record(npc, dx: 100, dy: 200);

        var keep = ops[0];
        Assert.Equal("Keep", keep.Layer);
        Assert.Equal(NpcDrawKind.DrawBlend, keep.Kind);      // KeepPlayBlendDraw <> 0
        Assert.Equal(100 + 3 + 11 + 5, keep.X);              // 10323-10325
        Assert.Equal(200 + 4 + 12 - 7, keep.Y);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 5. TNpcActor.DrawEff（10424-10433）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void DrawEff_RequiresBothFlagAndSurface()
    {
        var npc = NewNpc();
        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;

        npc.m_boUseEffect = true;
        npc.m_EffSurface = null;
        npc.DrawEff(10, 20);
        Assert.Empty(ops);                     // 10427：m_EffSurface = nil ⇒ 不画

        npc.m_boUseEffect = false;
        npc.m_EffSurface = "e";
        npc.DrawEff(10, 20);
        Assert.Empty(ops);                     // 10427：m_boUseEffect 为假 ⇒ 不画

        npc.m_boUseEffect = true;
        npc.DrawEff(10, 20);
        var op = Assert.Single(ops);
        Assert.Equal(NpcDrawKind.DrawBlend, op.Kind);
        Assert.Equal(10428, op.ReferenceLine);
    }

    [Fact]
    public void DrawEff_DoesNotCallInherited()
    {
        // 原文 10426 的 `// inherited;` 是注释；基类 TActor.DrawEff 也是空实现，
        // 故此断言锁的是"不经基类后仍只产生一条 Eff 操作"。
        var npc = NewNpc();
        npc.m_boUseEffect = true;
        npc.m_EffSurface = "e";

        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;
        npc.DrawEff(0, 0);

        Assert.Single(ops);
        Assert.Equal("Eff", ops[0].Layer);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 6. TNpcActor.GetDefaultFrame（10435-10480）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void GetDefaultFrame_GlobalPathUsesActStandWithClampedCf()
    {
        var npc = NewNpc(appearance: 300, dir: 1);
        npc.m_nCurrentDefFrame = 2;

        Assert.Equal(0 + 1 * 10 + 2, npc.GetDefaultFrame(false));    // 10478
    }

    [Fact]
    public void GetDefaultFrame_ClampsNegativeAndOverflowCfToZero()
    {
        var npc = NewNpc(appearance: 300, dir: 0);

        npc.m_nCurrentDefFrame = -1;
        Assert.Equal(0, npc.GetDefaultFrame(false));   // 10469

        npc.m_nCurrentDefFrame = 4;                    // >= ActStand.frame(4)
        Assert.Equal(0, npc.GetDefaultFrame(false));   // 10471
    }

    [Fact]
    public void GetDefaultFrame_WhiteListExcludes245UnlikeCalcActorFrame()
    {
        // ★ 差异锁死：CalcActorFrame 的 9978 行白名单**含 245**，GetDefaultFrame 的 10475 **不含**。
        //   取 dir = 4：两者都先做 mod 3 ⇒ 1；随后 CalcActorFrame 因含 245 归零，
        //   而 GetDefaultFrame 保持 1。
        var calc = NewNpc(appearance: 245, dir: 4);
        calc.m_nCurrentAction = TNpcActor.SM_TURN;
        calc.CalcActorFrame();
        Assert.Equal(0, calc.m_btDir);            // 9978

        var def = NewNpc(appearance: 245, dir: 4);
        def.GetDefaultFrame(false);
        Assert.Equal(1, def.m_btDir);             // 10475 不含 245 ⇒ 保持 4 mod 3 = 1
    }

    [Fact]
    public void GetDefaultFrame_CustomNpcUsesStdIndexAndStdTime()
    {
        const ushort appr = 10030;
        var cfg = MakeConfig(appr, 2, null,
            (1, DirAction(stdIndex: 500, stdCount: 8, stdTime: 321)));
        UseConfig(cfg);

        var npc = NewNpc(appearance: appr, dir: 3);
        npc.m_nCurrentDefFrame = 5;

        int frame = npc.GetDefaultFrame(false);

        Assert.Equal(1, npc.m_btDir);        // 3 mod 2 = 1
        Assert.Equal(500 + 5, frame);        // 10457
        Assert.Equal(321u, npc.m_dwFrameTime);   // 10458
    }

    [Fact]
    public void GetDefaultFrame_CustomNpcZeroDirCountForcesDirectionZero()
    {
        const ushort appr = 10031;
        UseConfig(MakeConfig(appr, 0, null, (0, DirAction(stdIndex: 100, stdCount: 4))));

        var npc = NewNpc(appearance: appr, dir: 5);

        Assert.Equal(100, npc.GetDefaultFrame(false));
        Assert.Equal(0, npc.m_btDir);
    }

    [Fact]
    public void GetDefaultFrame_CustomNpcClampsOverflowStdCount()
    {
        const ushort appr = 10032;
        UseConfig(MakeConfig(appr, 1, null, (0, DirAction(stdIndex: 200, stdCount: 3))));

        var npc = NewNpc(appearance: appr, dir: 0);
        npc.m_nCurrentDefFrame = 3;          // >= Std_Count

        Assert.Equal(200, npc.GetDefaultFrame(false));   // 10453 ⇒ cf = 0
    }

    // ══════════════════════════════════════════════════════════════════════
    // 7. TNpcActor.LoadSurface（10482-11058）
    // ══════════════════════════════════════════════════════════════════════

    private sealed class FakeImages
    {
        public readonly List<(int Lib, int Index, TColorEffect Color)> NpcStore = new();
        public readonly List<(int Index, TColorEffect Color)> NpcRoot = new();
        public readonly List<(int File, int Index, TColorEffect Color)> EffectList = new();
        public readonly List<(int Index, bool Gray)> NewopUi = new();
        public object? NextTexture = "tex";
        public int NextPx = 11;
        public int NextPy = 22;
    }

    private FakeImages HookImages()
    {
        var f = new FakeImages();
        ActorNpcEnv.FetchNpcImageFn = (lib, idx, color) =>
        {
            f.NpcStore.Add((lib, idx, color));
            return new NpcImageFetch(f.NextTexture, f.NextPx, f.NextPy);
        };
        ActorNpcEnv.FetchNpcRootImageFn = (idx, color) =>
        {
            f.NpcRoot.Add((idx, color));
            return new NpcImageFetch(f.NextTexture, f.NextPx, f.NextPy);
        };
        ActorNpcEnv.FetchEffectListImageFn = (file, idx, color) =>
        {
            f.EffectList.Add((file, idx, color));
            return new NpcImageFetch(f.NextTexture, f.NextPx, f.NextPy);
        };
        ActorNpcEnv.FetchNewopUiImageFn = (idx, gray) =>
        {
            f.NewopUi.Add((idx, gray));
            return new NpcImageFetch(f.NextTexture, f.NextPx, f.NextPy);
        };
        return f;
    }

    [Fact]
    public void LoadSurface_ClearsThreeSlotsAndTimestamps()
    {
        var npc = NewNpc(appearance: 300);
        npc.m_BodySurface = "b";
        npc.m_EffSurface = "e";
        npc.m_KeepSurface = "k";
        npc.m_boLoadSurface = true;

        npc.LoadSurface(null);

        Assert.Equal(500u, npc.m_dwLoadSurfaceTime);   // 10491
        Assert.False(npc.m_boLoadSurface);             // 10492
    }

    [Fact]
    public void LoadSurface_GlobalPathAppearance42To47NullsBodySurface()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 43, race: 50);

        npc.LoadSurface(null);

        Assert.Null(npc.m_BodySurface);    // 直接命中 10926-10927（或在段 2 的 10679）
    }

    [Fact]
    public void LoadSurface_GlobalPathUsesNpcStoreZeroForRace50SmallAppearance()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 100, race: 50);
        npc.m_nBodyOffset = 5;
        npc.m_nCurrentFrame = 3;
        npc.ActorColorEffect = TColorEffect.ceGrayScale;

        npc.LoadSurface(null);

        Assert.Contains((0, 8, TColorEffect.ceGrayScale), f.NpcStore);   // 10783：Indexs[0]
        Assert.Equal("tex", npc.m_BodySurface);
        Assert.Equal(11, npc.m_nPx);
        Assert.Equal(22, npc.m_nPy);
    }

    [Fact]
    public void LoadSurface_Appearance226to245UsesStoreTwoAndAppliesResourceCorrections()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 230, race: 50);
        npc.m_nBodyOffset = 700;
        npc.m_nCurrentFrame = 61;                  // 700 + 61 = 761 → m_nPy := -42
        npc.ActorColorEffect = TColorEffect.ceNone;

        npc.LoadSurface(null);

        Assert.Contains((2, 761, TColorEffect.ceNone), f.NpcStore);   // 10791
        Assert.Equal(-42, npc.m_nPy);                                 // 10798（★ 注意不是取图返回的 22）

        npc.m_nBodyOffset = 800;
        npc.m_nCurrentFrame = 91;                  // 800 + 91 = 891 → m_nPx := 10; m_nPy := -45
        npc.LoadSurface(null);

        Assert.Equal(10, npc.m_nPx);
        Assert.Equal(-45, npc.m_nPy);
    }

    [Fact]
    public void LoadSurface_ResourceCorrectionIsSkippedForGrayScale()
    {
        HookImages();
        var npc = NewNpc(appearance: 230, race: 50);
        npc.m_nBodyOffset = 700;
        npc.m_nCurrentFrame = 61;
        npc.ActorColorEffect = TColorEffect.ceGrayScale;

        npc.LoadSurface(null);

        Assert.Equal(22, npc.m_nPy);   // 修正只在 else（非灰度）分支内
    }

    [Fact]
    public void LoadSurface_Appearance244And245TakeEffFromStoreTwoWithOffsets()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 244, race: 50);
        npc.m_nBodyOffset = 100;
        npc.m_nCurrentFrame = 7;
        npc.ActorColorEffect = TColorEffect.ceNone;

        npc.LoadSurface(null);

        Assert.Contains((2, 100 + 7 + 30), f.NpcStore.Select(x => (x.Lib, x.Index)));   // 10807

        npc.m_wAppearance = 245;
        npc.LoadSurface(null);
        Assert.Contains((2, 100 + 7 + 10), f.NpcStore.Select(x => (x.Lib, x.Index)));   // 10815
    }

    [Fact]
    public void LoadSurface_Appearance246to272UsesStoreThree()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 250, race: 50);
        npc.m_nCurrentFrame = 0;
        npc.ActorColorEffect = TColorEffect.ceNone;

        npc.LoadSurface(null);

        Assert.Contains((3, 0), f.NpcStore.Select(x => (x.Lib, x.Index)));   // 10824
    }

    [Fact]
    public void LoadSurface_AppearanceOver1000UsesStoreNine()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 1500, race: 50);
        npc.m_nCurrentFrame = 0;
        npc.ActorColorEffect = TColorEffect.ceNone;

        npc.LoadSurface(null);

        Assert.Contains((9, 0), f.NpcStore.Select(x => (x.Lib, x.Index)));   // 10832
    }

    [Fact]
    public void LoadSurface_Appearance200to225FallsToStoreOne()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 210, race: 50);
        npc.m_nCurrentFrame = 0;
        npc.ActorColorEffect = TColorEffect.ceNone;

        npc.LoadSurface(null);

        Assert.Contains((1, 0), f.NpcStore.Select(x => (x.Lib, x.Index)));   // 10840
    }

    [Fact]
    public void LoadSurface_EffOffsetsAreAddedForAppearance44to47()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 46, race: 50);
        npc.m_boUseEffect = true;
        npc.m_nBodyOffset = 10;
        npc.m_nEffectFrame = 2;

        npc.LoadSurface(null);

        // 10979：Indexs[0]，图号 10+2；随后 10984-10985 加 (7, 12)
        Assert.Contains((0, 12), f.NpcStore.Select(x => (x.Lib, x.Index)));
        Assert.Equal(11 + 7, npc.m_nEffX);
        Assert.Equal(22 + 12, npc.m_nEffY);
    }

    [Fact]
    public void LoadSurface_Appearance42EffUsesRootFetchWithoutIndexs()
    {
        var f = HookImages();
        var npc = NewNpc(appearance: 42, race: 50);
        npc.m_boUseEffect = true;
        npc.m_nBodyOffset = 10;
        npc.m_nEffectFrame = 4;

        npc.LoadSurface(null);

        // ★ 10939-10942：全篇唯一一处不带 Indexs[] 的取图
        Assert.Contains((14, TColorEffect.ceNone), f.NpcRoot);
        Assert.Equal(11, npc.m_nEffX);   // 42 的坐标修正被原文注释 ⇒ 不加
        Assert.Equal(22, npc.m_nEffY);
    }

    [Fact]
    public void LoadSurface_CustomNpcKeepEffectRequiresFiveGates()
    {
        const ushort appr = 10040;
        var bc = new TNpcBaseConfig
        {
            KeepPlayFile = 3, KeepPlayIndex = 9, KeepPlayCount = 5, KeepPlayTime = 100,
        };
        UseConfig(MakeConfig(appr, 1, bc, (0, DirAction())));
        ActorNpcEnv.EffectImageListCountFn = () => 10;

        var f = HookImages();
        var npc = NewNpc(appearance: appr);
        npc.m_nKeepFrame = 9;

        npc.LoadSurface(null);

        Assert.Contains((3, 9, TColorEffect.ceNone), f.EffectList);   // 10516-10521
        Assert.Equal("tex", npc.m_KeepSurface);
    }

    [Fact]
    public void LoadSurface_CustomNpcStdEffectUsesStdCountAndTimeGate()
    {
        const ushort appr = 10041;
        var action = DirAction(stdFile: 1, stdIndex: 100, stdCount: 4, stdTime: 200,
            stdEffFile: 2, stdEffIndex: 40);
        UseConfig(MakeConfig(appr, 1, null, (0, action)));
        ActorNpcEnv.EffectImageListCountFn = () => 5;

        var f = HookImages();
        var npc = NewNpc(appearance: appr);
        npc.m_nCurrentFrame = 102;    // 非 HIT/WALK ⇒ Std 段
        npc.m_nCurrentAction = 0;

        npc.LoadSurface(null);

        // 10561 的判据用 Std_Count/Std_Time ⇒ 通过
        Assert.Contains((2, 40 + 102 - 100, TColorEffect.ceNone), f.EffectList);   // 10564
    }

    [Fact]
    public void LoadSurface_CustomNpcActEffectGateUsesActCountAndTime()
    {
        const ushort appr = 10042;
        var action = DirAction(stdFile: 1, actFile: 1, actIndex: 30, actCount: 5, actTime: 100,
            actEffFile: 1, actEffIndex: 60);
        UseConfig(MakeConfig(appr, 1, null, (0, action)));
        ActorNpcEnv.EffectImageListCountFn = () => 5;

        var f = HookImages();
        var npc = NewNpc(appearance: appr);
        npc.m_nCurrentAction = TNpcActor.SM_HIT;
        npc.m_nCurrentFrame = 32;

        npc.LoadSurface(null);

        Assert.Contains((1, 60 + 32 - 30, TColorEffect.ceNone), f.EffectList);   // 10540
    }

    [Fact]
    public void LoadSurface_CustomNpcStdBodyRequiresTimeGate()
    {
        const ushort appr = 10043;
        // Std_Time = 0 ⇒ 10550 的门为假 ⇒ 不取主体图（原文如此）
        var action = DirAction(stdFile: 1, stdIndex: 100, stdCount: 4, stdTime: 0);
        UseConfig(MakeConfig(appr, 1, null, (0, action)));
        ActorNpcEnv.EffectImageListCountFn = () => 5;

        var f = HookImages();
        var npc = NewNpc(appearance: appr);
        npc.m_nCurrentAction = 0;

        npc.LoadSurface(null);

        Assert.DoesNotContain(f.EffectList, x => x.Index == 100);
    }

    [Fact]
    public void LoadSurface_CustomNpcWithoutConfigStillClearsAndReturns()
    {
        UseConfig(MakeConfig(10050, 1));
        var npc = NewNpc(appearance: 10051);      // 查找返回 null
        npc.m_BodySurface = "b";
        npc.m_EffSurface = "e";
        npc.m_KeepSurface = "k";

        npc.LoadSurface(null);

        Assert.Null(npc.m_BodySurface);   // 10493
        Assert.Null(npc.m_EffSurface);    // 10495
        Assert.Null(npc.m_KeepSurface);   // 10496
    }

    // ══════════════════════════════════════════════════════════════════════
    // 8. TNpcActor.Run（11060-11122）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Run_DoesNotAdvanceEffectWhenTimeNotReached()
    {
        var npc = NewNpc();
        npc.m_boUseEffect = true;
        npc.m_nEffectFrame = 5;
        npc.m_nEffectEnd = 10;
        npc.m_dwEffectFrameTime = 100;
        npc.m_dwEffectStartTime = 950;    // now(1000) - 950 = 50 <= 100

        npc.Run(1000);

        Assert.Equal(5, npc.m_nEffectFrame);
    }

    [Fact]
    public void Run_AdvancesEffectFrameWhenPastFrameTime()
    {
        var npc = NewNpc();
        npc.m_boUseEffect = true;
        npc.m_nEffectFrame = 5;
        npc.m_nEffectEnd = 10;
        npc.m_dwEffectFrameTime = 100;
        npc.m_dwEffectStartTime = 800;    // 1000 - 800 = 200 > 100

        npc.Run(1000);

        Assert.Equal(6, npc.m_nEffectFrame);
        Assert.Equal(1000u, npc.m_dwEffectStartTime);
    }

    [Fact]
    public void Run_UseMagicDividesEffectFrameTimeByThree()
    {
        var npc = NewNpc();
        npc.m_boUseEffect = true;
        npc.m_boUseMagic = true;
        npc.m_nEffectFrame = 5;
        npc.m_nEffectEnd = 10;
        npc.m_dwEffectFrameTime = 300;    // /3 = 100
        npc.m_dwEffectStartTime = 850;    // 150 > 100

        npc.Run(1000);

        Assert.Equal(6, npc.m_nEffectFrame);
    }

    [Fact]
    public void Run_WrapsEffectToStartAtEnd()
    {
        var npc = NewNpc();
        npc.m_boUseEffect = true;
        npc.m_nEffectFrame = 10;
        npc.m_nEffectEnd = 10;
        npc.m_nEffectStart = 4;
        npc.m_dwEffectFrameTime = 100;
        npc.m_dwEffectStartTime = 800;

        npc.Run(1000);

        Assert.Equal(4, npc.m_nEffectFrame);    // 11092
    }

    [Fact]
    public void Run_Bo248ExtinguishesOnlyAfterUseEffectTick()
    {
        var npc = NewNpc(appearance: 52);
        npc.m_boUseEffect = true;
        npc.m_bo248 = true;
        npc.m_nEffectFrame = 10;
        npc.m_nEffectEnd = 10;
        npc.m_nEffectStart = 4;
        npc.m_dwEffectFrameTime = 100;
        npc.m_dwEffectStartTime = 800;
        npc.m_dwUseEffectTick = 5000;          // now(1000) 未超过

        npc.Run(1000);

        Assert.True(npc.m_boUseEffect);        // 11084 为假 ⇒ 不熄灭
        Assert.True(npc.m_bo248);
        Assert.Equal(4, npc.m_nEffectFrame);
    }

    [Fact]
    public void Run_Bo248ExtinguishesWhenTickPassed()
    {
        var npc = NewNpc(appearance: 52);
        npc.m_boUseEffect = true;
        npc.m_bo248 = true;
        npc.m_nEffectFrame = 10;
        npc.m_nEffectEnd = 10;
        npc.m_nEffectStart = 4;
        npc.m_dwEffectFrameTime = 100;
        npc.m_dwEffectStartTime = 800;
        npc.m_dwUseEffectTick = 900;           // now(1000) > 900

        npc.Run(1000);

        Assert.False(npc.m_boUseEffect);       // 11085
        Assert.False(npc.m_bo248);             // 11086
        Assert.Equal(1000u, npc.m_dwUseEffectTick);   // 11087
        Assert.Equal(4, npc.m_nEffectFrame);   // 11089：两种情况都回卷
    }

    [Fact]
    public void Run_RequestsLoadSurfaceOnlyWhenFrameChanged()
    {
        var requested = new List<TActor>();
        ActorNpcEnv.RequestLoadSurfaceFn = requested.Add;

        var npc = NewNpc();
        npc.m_boUseEffect = true;
        npc.m_nEffectFrame = 5;
        npc.m_nEffectEnd = 10;
        npc.m_dwEffectFrameTime = 100;
        npc.m_dwEffectStartTime = 950;

        npc.Run(1000);                         // 帧未变
        Assert.Empty(requested);

        npc.m_dwEffectStartTime = 800;
        npc.Run(1000);                         // 帧 5 -> 6
        Assert.Single(requested);
        Assert.Same(npc, requested[0]);
    }

    [Fact]
    public void Run_KeepFrameAdvancesWithIndexClamp()
    {
        const ushort appr = 10060;
        var bc = new TNpcBaseConfig
        {
            KeepPlayFile = 1, KeepPlayIndex = 5, KeepPlayCount = 3, KeepPlayTime = 100,
        };
        UseConfig(MakeConfig(appr, 1, bc));
        ActorNpcEnv.EffectImageListCountFn = () => 5;

        var npc = NewNpc(appearance: appr);
        npc.m_nKeepFrame = 5;
        npc.m_LastKeepPlayTick = 800;          // 1000 - 800 = 200 >= 100

        npc.Run(1000);

        Assert.Equal(6, npc.m_nKeepFrame);     // 11105
        Assert.Equal(1000u, npc.m_LastKeepPlayTick);
    }

    [Fact]
    public void Run_KeepFrameWrapsAtIndexPlusCount()
    {
        const ushort appr = 10061;
        var bc = new TNpcBaseConfig
        {
            KeepPlayFile = 1, KeepPlayIndex = 5, KeepPlayCount = 3, KeepPlayTime = 100,
        };
        UseConfig(MakeConfig(appr, 1, bc));
        ActorNpcEnv.EffectImageListCountFn = () => 5;

        var npc = NewNpc(appearance: appr);
        npc.m_nKeepFrame = 7;                  // +1 = 8 >= 5 + 3 ⇒ 回卷 5
        npc.m_LastKeepPlayTick = 800;

        npc.Run(1000);

        Assert.Equal(5, npc.m_nKeepFrame);     // 11109
    }

    [Fact]
    public void Run_KeepFrameTimeComparisonIsInclusive()
    {
        const ushort appr = 10062;
        var bc = new TNpcBaseConfig
        {
            KeepPlayFile = 1, KeepPlayIndex = 5, KeepPlayCount = 3, KeepPlayTime = 100,
        };
        UseConfig(MakeConfig(appr, 1, bc));
        ActorNpcEnv.EffectImageListCountFn = () => 5;

        var npc = NewNpc(appearance: appr);
        npc.m_nKeepFrame = 5;
        npc.m_LastKeepPlayTick = 900;          // 恰好 100 ⇒ >= 成立

        npc.Run(1000);

        Assert.Equal(6, npc.m_nKeepFrame);     // 11104 是 >=（闭区间）
    }

    // ══════════════════════════════════════════════════════════════════════
    // 9. TStatuaryNpcActor（17537-18008）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void StatuaryCalcActorFrame_WritesFixedStatuaryFrame()
    {
        var s = new TStatuaryNpcActor { m_boUseMagic = true, m_nCurrentFrame = 9, m_btDir = 5 };

        s.CalcActorFrame();

        Assert.False(s.m_boUseMagic);
        Assert.Equal(-1, s.m_nCurrentFrame);   // 17540（本方法**不**回写 0）
        Assert.Equal(1200, s.m_nBodyOffset);   // 17541
        Assert.Equal(0, s.m_btDir);            // 17543
        Assert.Equal(0, s.m_nStartFrame);      // 17545
        Assert.Equal(0, s.m_nEndFrame);        // 17546
        Assert.Equal(100u, s.m_dwFrameTime);   // 17547
        Assert.Equal(1000u, s.m_dwStartTime);  // 17548
        Assert.Equal(1000u, s.m_StartCounter); // 17549
        Assert.Equal(1, s.m_nDefFrameCount);   // 17550
    }

    [Fact]
    public void StatuaryCreate_ZeroesEffigyValue1Value2AndStatuaryFlags()
    {
        var fresh = new TStatuaryNpcActor();

        Assert.False(fresh.m_boShowStatuary);              // 17561
        Assert.Equal(0, fresh.m_nEffigyState.Value1);      // 17562
        Assert.Equal(0, fresh.m_nEffigyState.Value2);      // 17563
        Assert.Equal(0, fresh.m_nEffigyOffset);            // 17565
        Assert.Equal(0, fresh.m_nOldEffigyOffset);         // 17566
        Assert.Equal(0, fresh.m_wDress);                   // 17568
        Assert.Equal(0, fresh.m_wWeapon);                  // 17569
        Assert.Equal(0, fresh.m_wWeaponSound);             // 17570
        Assert.Equal(0, fresh.m_wShield);                  // 17571
        Assert.Equal(0, fresh.m_btHair);                   // 17573
        Assert.True(fresh.m_boScaleShow);                  // 17575
        Assert.False(fresh.m_IsGrayShow);                  // 17576
        Assert.False(fresh.FIsFinalized);                  // 17578
    }

    [Fact]
    public void StatuaryCreate_DoesNotWriteEffigyDressEffType()
    {
        // ★ 17562-17563 只写 Value1/Value2（**没有**动第三个字段 wDressEffType）——
        //   本断言锁的是"构造后该字段仍为结构体默认 0"，即 17561-17578 全文不含对它的赋值。
        var s = new TStatuaryNpcActor();
        Assert.Equal(0, s.m_nEffigyState.wDressEffType);

        // 反证：赋值后不因任何构造期代码被清掉
        s.m_nEffigyState.wDressEffType = 1234;
        Assert.Equal(1234, s.m_nEffigyState.wDressEffType);
    }

    [Fact]
    public void StatuaryGetDefaultFrame_IgnoresWmodeAndReturnsZero()
    {
        var s = new TStatuaryNpcActor { m_nCurrentDefFrame = 9 };

        Assert.Equal(0, s.GetDefaultFrame(false));
        Assert.Equal(0, s.GetDefaultFrame(true));
    }

    [Fact]
    public void StatuaryFinalize_SetsFinalizedFlagAfterBase()
    {
        var s = new TStatuaryNpcActor();

        s.Finalize();

        Assert.True(s.FIsFinalized);    // 17647
    }

    [Fact]
    public void StatuaryLoadSurface_ClearsEffButNotKeep()
    {
        var f = HookImages();
        var s = new TStatuaryNpcActor
        {
            m_wAppearance = 100,
            m_nBodyOffset = 1200,
            m_nCurrentFrame = 3,
            m_EffSurface = "e",
            m_KeepSurface = "keep",
            m_boLoadSurface = true,
        };

        s.LoadSurface(null);

        Assert.Equal(500u, s.m_dwLoadSurfaceTime);   // 17657
        Assert.False(s.m_boLoadSurface);             // 17658
        Assert.Null(s.m_EffSurface);                 // 17659
        Assert.Equal("keep", s.m_KeepSurface);       // ★ 不清（与 TNpcActor 10496 不同）
        Assert.Contains((1203, false), f.NewopUi);   // 17663
        Assert.Equal("tex", s.m_BodySurface);
    }

    [Fact]
    public void StatuaryLoadSurface_UsesGrayWhenIsGrayShow()
    {
        var f = HookImages();
        var s = new TStatuaryNpcActor { m_IsGrayShow = true, m_nBodyOffset = 1200, m_nCurrentFrame = 0 };

        s.LoadSurface(null);

        Assert.Contains((1200, true), f.NewopUi);    // 17661
    }

    [Fact]
    public void StatuaryDrawChr_WhenNotShowingDrawsOnlyBase()
    {
        var s = new TStatuaryNpcActor { m_boShowStatuary = false };
        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;

        s.DrawChr(10, 20, false, false);

        Assert.Empty(ops);    // TNpcActor.DrawChr 全局支无图 ⇒ 也不画
    }

    [Fact]
    public void StatuaryDrawChr_NonScaledDrawsEffThenHumWithBlendAndBodyWithDraw()
    {
        var s = new TStatuaryNpcActor();
        s.m_boShowStatuary = true;
        s.m_boScaleShow = false;
        s.m_WeaponEffTexture = "w";
        s.m_HumEffTexture = "he";
        s.m_HumTexture = "h";
        s.m_nShiftX = 1;
        s.m_nShiftY = 2;
        ActorNpcEnv.TextureSizeFn = t => t is null ? (0, 0) : (64, 128);

        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;

        s.DrawChr(100, 200, false, false);

        Assert.Equal(3, ops.Count);
        Assert.Equal("WeaponEff", ops[0].Layer);
        Assert.Equal(NpcDrawKind.DrawBlend, ops[0].Kind);
        Assert.Equal("HumEff", ops[1].Layer);
        Assert.Equal(NpcDrawKind.DrawBlend, ops[1].Kind);
        Assert.Equal("Hum", ops[2].Layer);
        Assert.Equal(NpcDrawKind.Draw, ops[2].Kind);        // ★ 主体用 Draw 而非 DrawBlend
        Assert.Equal(100 + 1 - 200, ops[0].X);              // 17611
        Assert.Equal(200 + 2 - 237, ops[0].Y);
    }

    [Fact]
    public void StatuaryDrawChr_ScaledInflatesDestAndStretches()
    {
        var s = new TStatuaryNpcActor();
        s.m_boShowStatuary = true;
        s.m_boScaleShow = true;
        s.m_HumTexture = "h";
        s.m_nShiftX = 0;
        s.m_nShiftY = 0;
        ActorNpcEnv.TextureSizeFn = t => t is null ? (0, 0) : (64, 128);

        var ops = new List<NpcDrawOp>();
        ActorNpcEnv.NpcDrawFn = ops.Add;

        s.DrawChr(300, 400, false, false);

        var op = Assert.Single(ops);
        Assert.Equal(NpcDrawKind.StretchDraw, op.Kind);
        Assert.Equal(300 - 200 - 20, op.X);       // InflateRect(DR, 20, 20)
        Assert.Equal(400 - 237 - 20, op.Y);
        Assert.Equal(64 + 40, op.DestWidth);
        Assert.Equal(128 + 40, op.DestHeight);
    }

    [Fact]
    public void StatuaryDrawChr_FinalizedTriggersOneShotReplay()
    {
        var s = new TStatuaryNpcActor();
        s.FIsFinalized = true;
        s.m_boShowStatuary = false;
        s.m_IsGrayShow = false;
        s.m_boScaleShow = false;
        s.m_nBodyOffset = 1200;
        s.m_nCurrentFrame = 5;
        s.m_nEffigyState = new TFeature_New { Value1 = 1 };   // 重放用**当前字段值**

        var fetched = new List<(int Index, bool Gray)>();
        ActorNpcEnv.FetchNewopUiImageFn = (idx, gray) =>
        {
            fetched.Add((idx, gray));
            return new NpcImageFetch("t", 0, 0);
        };

        s.DrawChr(0, 0, false, false);

        Assert.False(s.FIsFinalized);            // 17604
        Assert.Contains((1205, false), fetched); // 17605 重放 SetEffigyState ⇒ 17743 取主体图
    }

    [Fact]
    public void StatuaryDrawChr_ReplayUsesCurrentFieldValuesNotOriginalArguments()
    {
        var s = new TStatuaryNpcActor();
        s.FIsFinalized = true;
        s.m_boShowStatuary = false;
        s.m_nEffigyState = new TFeature_New { Value1 = 1 };
        s.m_IsGrayShow = true;                    // 字段被改过
        s.m_nBodyOffset = 1200;
        s.m_nCurrentFrame = 5;

        var grays = new List<bool>();
        ActorNpcEnv.FetchNewopUiImageFn = (_, gray) => { grays.Add(gray); return new NpcImageFetch("t", 0, 0); };

        s.DrawChr(0, 0, false, false);

        Assert.Equal(new[] { true }, grays);      // 重放传的是 m_IsGrayShow 的**当前值**
    }

    // ══════════════════════════════════════════════════════════════════════
    // 10. TStatuaryNpcActor.SetEffigyState（17667-18008）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void SetEffigyState_Value1ZeroHidesStatuaryAndFreesTextures()
    {
        var s = new TStatuaryNpcActor
        {
            m_HumTexture = "h",
            m_HumEffTexture = "he",
            m_WeaponEffTexture = "w",
        };

        s.SetEffigyState(false, true, new TFeature_New { Value1 = 0 }, 0);

        Assert.False(s.m_boShowStatuary);     // 17702
        Assert.Null(s.m_HumTexture);          // 17704
        Assert.Null(s.m_HumEffTexture);       // 17707
        Assert.Null(s.m_WeaponEffTexture);    // 17710
    }

    [Fact]
    public void SetEffigyState_UnpacksBitFieldsFromValue1AndValue2()
    {
        // Value1 低 32 = Dress(低16) | Weapon(高16)；高 32 = Effect(低16) | Hair(低8) | Shield(高8)
        // Value2 低 32 = DressEffIndex(低16) | WeaponEffIndex(高16)；高 32 = DressEffOffSet | WeaponEffOffSet
        int l1 = 0x000A_0005;             // Dress=5, Weapon=10
        int h1 = 0x0703_000C;             // Effect=12, W=0x0703 ⇒ Hair=3, Shield=7
        int l2 = 0x0014_0009;             // DressEffIndex=9, WeaponEffIndex=20
        int h2 = 0x001E_0011;             // DressEffOffSet=17, WeaponEffOffSet=30

        long v1 = ((long)h1 << 32) | (uint)l1;
        long v2 = ((long)h2 << 32) | (uint)l2;

        var s = new TStatuaryNpcActor();
        s.SetEffigyState(false, true, new TFeature_New { Value1 = v1, Value2 = v2 }, 4);

        Assert.True(s.m_boShowStatuary);          // 17713
        Assert.Equal(5, s.m_wDress);              // 17718
        Assert.Equal(10, s.m_wWeapon);            // 17719
        Assert.Equal(1, s.m_btSex);               // 17721：5 mod 2
        Assert.Equal(12, s.m_wEffect);            // 17723
        Assert.Equal(3, s.m_btHair);              // 17725
        Assert.Equal(7, s.m_wShield);             // 17726
        Assert.Equal(9, s.m_nDressEffectIndex);   // 17731
        Assert.Equal(20, s.m_nWeaponEffectIndex); // 17732
        Assert.Equal(17, s.m_wDressEffectOffSet); // 17734
        Assert.Equal(30, s.m_wWeaponEffectOffSet);// 17735
        Assert.Equal(4, s.m_nEffigyOffset);       // 17693
        Assert.Equal(0, s.m_nOldEffigyOffset);    // 17692
    }

    [Fact]
    public void SetEffigyState_ClampsNegativeCurrentFrameToZero()
    {
        var s = new TStatuaryNpcActor { m_nCurrentFrame = -1 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 1 }, 0);

        Assert.Equal(0, s.m_nCurrentFrame);   // 17696
    }

    [Fact]
    public void SetEffigyState_CreatesThreeTexturesAndFetchesBody()
    {
        var created = 0;
        ActorNpcEnv.CreateTexture800Fn = () => { created++; return "t" + created; };
        var fetched = new List<(int Index, bool Gray)>();
        ActorNpcEnv.FetchNewopUiImageFn = (idx, gray) => { fetched.Add((idx, gray)); return new NpcImageFetch("body", 1, 2); };

        var s = new TStatuaryNpcActor { m_nBodyOffset = 1200, m_nCurrentFrame = 7 };
        s.SetEffigyState(true, false, new TFeature_New { Value1 = 1 }, 0);

        Assert.Equal(3, created);              // 17759-17761
        Assert.Equal("t1", s.m_HumTexture);
        Assert.Equal("t2", s.m_HumEffTexture);
        Assert.Equal("t3", s.m_WeaponEffTexture);
        Assert.Contains((1207, true), fetched);   // 17743
        Assert.Equal("body", s.m_BodySurface);
        Assert.Equal(1, s.m_nPx);
        Assert.Equal(2, s.m_nPy);
    }

    [Theory]
    // hair, sex, 期望 nHairOffset
    [InlineData((byte)0, (byte)0, 0)]
    [InlineData((byte)2, (byte)0, 2 * 2 * 600)]
    [InlineData((byte)0, (byte)1, 0)]
    [InlineData((byte)2, (byte)1, (2 + 2) * 600)]
    [InlineData((byte)4, (byte)0, 3600)]
    [InlineData((byte)5, (byte)1, 4800)]
    [InlineData((byte)52, (byte)1, 2 * 600)]
    [InlineData((byte)105, (byte)1, 3600 + 5 * 600 * 2 + 1 * 600)]
    [InlineData((byte)112, (byte)0, 0 + 4 * 600 * 2)]
    [InlineData((byte)125, (byte)1, 5 * 600 * 2 + 600)]
    [InlineData((byte)135, (byte)0, 5 * 600 * 2)]
    [InlineData((byte)145, (byte)0, 5 * 600 * 2)]
    public void SetEffigyState_HairOffsetTable(byte hair, byte sex, int expectedOffset)
    {
        // 性别由 wDress 奇偶决定 ⇒ 构造 dress = sex
        int l1 = (0 << 16) | sex;
        int h1 = (12 << 0) | (hair << 16);                // Effect=12, W=hair（shield=0）
        long v1 = ((long)h1 << 32) | (uint)l1;

        int weaponIndex = -1;
        var hairFetches = new List<(int Lib, int Index)>();
        ActorNpcEnv.WeaponImageFn = (_, _, idx, _) => { weaponIndex = idx; return default; };
        ActorNpcEnv.HairImageFn = (lib, idx, _) => { hairFetches.Add((lib, idx)); return default; };

        var s = new TStatuaryNpcActor { m_nCurrentFrame = 5 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = v1 }, 0);

        Assert.Equal(hair, s.m_btHair);                      // 17725
        Assert.Equal(sex, s.m_btSex);                        // 17721：wDress mod 2
        Assert.Equal(5, weaponIndex);                        // 17822-17824：currentFrame + offset

        // 17846 的 nHairOffset >= 0 门 + 17848 的选表 ⇒ 图号 = nHairOffset + frame + offset
        var expectedLib = hair switch
        {
            >= 0 and <= 5 => 0,
            >= 50 and <= 59 => 1,
            >= 60 and <= 69 => 2,
            >= 100 and <= 107 => 3,
            >= 108 and <= 119 => 4,
            >= 120 and <= 129 => 5,
            >= 130 and <= 139 => 6,
            _ => 7,
        };
        var fetch = Assert.Single(hairFetches);
        Assert.Equal(expectedLib, fetch.Lib);
        Assert.Equal(expectedOffset + 5, fetch.Index);
    }

    [Fact]
    public void SetEffigyState_HairOffsetGatesHairFetch()
    {
        // hair = 200（不在任何区间）⇒ nHairOffset 保持 -1 ⇒ **不取头发图**
        int l1 = (0 << 16) | 0;
        int h1 = (12 << 0) | (200 << 16);
        long v1 = ((long)h1 << 32) | (uint)l1;

        bool hairFetched = false;
        ActorNpcEnv.HairImageFn = (_, _, _) => { hairFetched = true; return default; };

        var s = new TStatuaryNpcActor();
        s.SetEffigyState(false, true, new TFeature_New { Value1 = v1 }, 0);

        Assert.False(hairFetched);    // 17846：nHairOffset >= 0 为假
    }

    [Fact]
    public void SetEffigyState_ShieldFetchedOnlyWhenShieldAboveZero()
    {
        // shield = 0
        long v1NoShield = ((long)((12) | (3 << 16)) << 32) | 0u;
        bool shieldFetched = false;
        ActorNpcEnv.ShieldImageFn = (_, _) => { shieldFetched = true; return default; };

        var s = new TStatuaryNpcActor();
        s.SetEffigyState(false, true, new TFeature_New { Value1 = v1NoShield }, 0);
        Assert.False(shieldFetched);   // 17911 显式 D := nil

        // shield = 2
        long v1Shield = ((long)((12) | (2 << 8) << 16) << 32) | 0u;
        s.SetEffigyState(false, true, new TFeature_New { Value1 = v1Shield }, 0);
        Assert.True(shieldFetched);    // 17906
    }

    [Fact]
    public void SetEffigyState_Value1ZeroStillRunsWingsSection()
    {
        // ★ 原文结构：17711 的 end 之后直接进入 17917 的翅膀段 —— 不是提前 Exit
        object? copied = null;
        ActorNpcEnv.HumEffectImageFn = (_, _, _, _, _) => new NpcImageFetch("wings", 0, 0);
        ActorNpcEnv.CopyTextureFn = (src, _, _, _, _) => copied = src;

        var s = new TStatuaryNpcActor { m_wEffect = 1000 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 0 }, 0);

        Assert.Equal("wings", copied);    // 17921-17923 + 17944-17945
    }

    [Fact]
    public void SetEffigyState_EffectFiftyBranchIsEmpty()
    {
        // 17926-17936：m_wEffect = 50 的整段被原文注释 ⇒ 什么都不取
        bool anyEffect = false;
        ActorNpcEnv.HumEffectImageFn = (_, _, _, _, _) => { anyEffect = true; return default; };
        ActorNpcEnv.HumEffectCachedImageFn = (_, _) => { anyEffect = true; return default; };

        var s = new TStatuaryNpcActor { m_wEffect = 50 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 0 }, 0);

        // 但 17947 的回退段会走 g_EffectImageList —— 索引未配置 ⇒ 不取
        Assert.False(anyEffect);
    }

    [Fact]
    public void SetEffigyState_EffectBelowThousandUsesCachedImageFormula()
    {
        var indices = new List<int>();
        ActorNpcEnv.HumEffectCachedImageFn = (idx, _) => { indices.Add(idx); return default; };

        var s = new TStatuaryNpcActor { m_wEffect = 3, m_nCurrentFrame = 4 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 0 }, 2);

        Assert.Equal((3 - 1) * ActorNpcEnv.HumanFrame + 4 + 2, Assert.Single(indices));   // 17939-17941
    }

    [Fact]
    public void SetEffigyState_DressEffectFallbackUsesEffectImageList()
    {
        var fetched = new List<(int File, int Index)>();
        ActorNpcEnv.EffectImageListCountFn = () => 10;
        ActorNpcEnv.FetchEffectListImageFn = (file, idx, _) => { fetched.Add((file, idx)); return default; };

        // 衣服特效索引 = 3，偏移 = 20；武器特效索引 = -1（越界）且 DB 偏移 = 0
        int l2 = (3 & 0xFFFF) | ((-1 & 0xFFFF) << 16);
        int h2 = 20;
        long v2 = ((long)h2 << 32) | (uint)l2;

        var s = new TStatuaryNpcActor { m_nCurrentFrame = 6 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 1, Value2 = v2 }, 1);

        Assert.Contains((3, 20 + 6 + 1), fetched);    // 17954-17957
    }

    [Fact]
    public void SetEffigyState_WeaponEffectDbfallbackBranches()
    {
        var humEffectWeaponFlags = new List<bool>();
        ActorNpcEnv.EffectImageListCountFn = () => 0;         // 武器索引判定必越界 ⇒ 走 DB 回退
        ActorNpcEnv.HumEffectImageFn = (_, _, _, weapon, _) => { humEffectWeaponFlags.Add(weapon); return default; };
        ActorNpcEnv.WeaponEffectImageFn = (_, _, _, _) => default;

        var s = new TStatuaryNpcActor { m_wDBWeaponEffectOffSet = 1010 };
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 1 }, 0);
        Assert.Contains(true, humEffectWeaponFlags);   // 17987：weapon = True

        humEffectWeaponFlags.Clear();
        s.m_wDBWeaponEffectOffSet = 1500;
        s.SetEffigyState(false, true, new TFeature_New { Value1 = 1 }, 0);
        Assert.Empty(humEffectWeaponFlags);            // 17993 走 WeaponEffectList
    }

    [Fact]
    public void SetEffigyState_Value2GrayFlagIsPassedToBodyFetch()
    {
        var grays = new List<bool>();
        ActorNpcEnv.FetchNewopUiImageFn = (_, gray) => { grays.Add(gray); return default; };

        var s = new TStatuaryNpcActor();
        s.SetEffigyState(true, false, new TFeature_New { Value1 = 1 }, 0);

        Assert.Equal(new[] { true }, grays);   // 17742-17745
    }

    // ══════════════════════════════════════════════════════════════════════
    // 11. THeroActor（17437-17533）
    // ══════════════════════════════════════════════════════════════════════

    [Theory]
    // 主角职业, 英雄职业, 期望技能号
    [InlineData(0, 0, 60)] [InlineData(0, 1, 62)] [InlineData(0, 2, 61)]
    [InlineData(1, 0, 62)] [InlineData(1, 1, 65)] [InlineData(1, 2, 64)]
    [InlineData(2, 0, 61)] [InlineData(2, 1, 64)] [InlineData(2, 2, 63)]
    public void GetGroupMagicId_FullTable(int myJob, byte heroJob, int expected)
    {
        ActorNpcEnv.MySelfJobFn = () => myJob;
        var hero = new THeroActor { m_btJob = heroJob };

        Assert.Equal(expected, hero.GetGroupMagicId());
    }

    [Theory]
    [InlineData(3, (byte)0)]
    [InlineData(0, (byte)3)]
    [InlineData(255, (byte)255)]
    public void GetGroupMagicId_OutOfRangeJobsStayZero(int myJob, byte heroJob)
    {
        ActorNpcEnv.MySelfJobFn = () => myJob;
        var hero = new THeroActor { m_btJob = heroJob };

        Assert.Equal(0, hero.GetGroupMagicId());   // 原文无 else
    }

    [Fact]
    public void FindGroupMagic_ReturnsFirstHit()
    {
        ActorNpcEnv.MySelfJobFn = () => 0;
        var hero = new THeroActor { m_btJob = 0 };      // 期望 60
        ActorNpcEnv.HeroMagicIdListFn = () => new[] { 11, 60, 60, 99 };

        Assert.Equal(60, hero.FindGroupMagic());
    }

    [Fact]
    public void FindGroupMagic_ReturnsNullWhenNotFound()
    {
        ActorNpcEnv.MySelfJobFn = () => 0;
        var hero = new THeroActor { m_btJob = 0 };
        ActorNpcEnv.HeroMagicIdListFn = () => new[] { 1, 2, 3 };

        Assert.Null(hero.FindGroupMagic());
    }

    [Fact]
    public void FindGroupMagic_EmptyListReturnsNull()
    {
        var hero = new THeroActor();
        ActorNpcEnv.HeroMagicIdListFn = () => Array.Empty<int>();

        Assert.Null(hero.FindGroupMagic());
    }

    [Fact]
    public void GroupAttack_SendsOnlyWhenAngryValueReachesMax()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        var ticks = new List<uint>();
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));
        ActorNpcEnv.SetLatestSpellTickFn = ticks.Add;

        var hero = new THeroActor { m_nAngryValue = 99, m_nMaxAngryValue = 100 };
        hero.GroupAttack();
        Assert.Empty(sends);
        Assert.Empty(ticks);

        hero.m_nAngryValue = 100;
        hero.GroupAttack();

        Assert.Equal((THeroActor.CM_HEROGROUPATTACK, 0L, 0, 0, 0), Assert.Single(sends));
        Assert.Equal(500u, Assert.Single(ticks));      // MyGetTickCountFn = 500
    }

    [Fact]
    public void GroupAttack_ZeroMaxNeverSends()
    {
        var sent = false;
        ActorNpcEnv.SendClientMessageFn = (_, _, _, _, _) => sent = true;

        var hero = new THeroActor { m_nAngryValue = 100, m_nMaxAngryValue = 0 };
        hero.GroupAttack();

        Assert.False(sent);    // 17489：m_nMaxAngryValue > 0 是第一个与项
    }

    [Fact]
    public void Rest_SendsRestHeroCommand()
    {
        var says = new List<string>();
        ActorNpcEnv.SendSayFn = says.Add;

        new THeroActor().Rest();

        Assert.Equal(new[] { "@RestHero" }, says);
    }

    [Fact]
    public void Protect_SendsMouseCoordinatesWithZeroRecog()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));
        ActorNpcEnv.MouseCurrXFn = () => 321;
        ActorNpcEnv.MouseCurrYFn = () => 654;

        new THeroActor().Protect();

        Assert.Equal((THeroActor.CM_HEROPROTECT, 0L, 321, 654, 0), Assert.Single(sends));
    }

    [Fact]
    public void Target_NoTargetAndNoFocusSendsMouseWithZeroRecog()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));
        ActorNpcEnv.MouseCurrXFn = () => 7;
        ActorNpcEnv.MouseCurrYFn = () => 8;

        new THeroActor().Target();

        Assert.Equal((THeroActor.CM_HEROTARGET, 0L, 7, 8, 0), Assert.Single(sends));
    }

    [Fact]
    public void Target_DeadTargetWithLiveFocusSendsFocus()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));

        var dead = new TActor { m_nRecogId = 1, m_boDeath = true };
        var focus = new TActor { m_nRecogId = 2, m_nCurrX = 30, m_nCurrY = 40 };
        ActorNpcEnv.TargetCretFn = () => dead;
        ActorNpcEnv.FocusCretFn = () => focus;
        ActorNpcEnv.IsValidActorFn = _ => true;

        new THeroActor().Target();

        Assert.Equal((THeroActor.CM_HEROTARGET, 2L, 30, 40, 0), Assert.Single(sends));   // 17523
    }

    [Fact]
    public void Target_DeadTargetWithDeadFocusSendsMouse()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));
        ActorNpcEnv.MouseCurrXFn = () => 5;
        ActorNpcEnv.MouseCurrYFn = () => 6;

        var dead = new TActor { m_boDeath = true };
        var deadFocus = new TActor { m_nRecogId = 2, m_boDeath = true };
        ActorNpcEnv.TargetCretFn = () => dead;
        ActorNpcEnv.FocusCretFn = () => deadFocus;
        ActorNpcEnv.IsValidActorFn = _ => true;

        new THeroActor().Target();

        Assert.Equal((THeroActor.CM_HEROTARGET, 0L, 5, 6, 0), Assert.Single(sends));    // 17526
    }

    [Fact]
    public void Target_InvalidTargetFallsToElseBranchWithoutDeathCheck()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));

        var invalidTarget = new TActor { m_nRecogId = 9 };
        var liveFocus = new TActor { m_nRecogId = 2, m_nCurrX = 1, m_nCurrY = 2, m_boDeath = true };
        ActorNpcEnv.TargetCretFn = () => invalidTarget;
        ActorNpcEnv.FocusCretFn = () => liveFocus;
        // IsValidActor 只对 focus 成立
        ActorNpcEnv.IsValidActorFn = a => ReferenceEquals(a, liveFocus);

        new THeroActor().Target();

        // ★ 17521 为假 ⇒ 落 17529 的 else；而 else **不判 m_boDeath** ⇒ 已死的焦点照样发
        Assert.Equal((THeroActor.CM_HEROTARGET, 2L, 1, 2, 0), Assert.Single(sends));
    }

    [Fact]
    public void Target_InvalidTargetAndInvalidFocusSendsNothing()
    {
        var sent = false;
        ActorNpcEnv.SendClientMessageFn = (_, _, _, _, _) => sent = true;
        ActorNpcEnv.TargetCretFn = () => new TActor();
        ActorNpcEnv.FocusCretFn = () => new TActor();
        ActorNpcEnv.IsValidActorFn = _ => false;

        new THeroActor().Target();

        Assert.False(sent);   // 17529 的 else 里没有"都不满足"的兜底
    }

    [Fact]
    public void HeroActorInheritsFromHumActorNotFromActorDirectly()
    {
        // ★ 基类层级必须与原文一致（Actor.pas 2049：THeroActor : THumActor）
        Assert.True(typeof(THumActor).IsAssignableFrom(typeof(THeroActor)));
        Assert.Equal("THeroActor", new THeroActor().ActorClass);
    }

    [Fact]
    public void NpcActorHierarchyMatchesSource()
    {
        // Actor.pas 1877 TNpcActor : TActor；1903 TStatuaryNpcActor : TNpcActor
        Assert.True(typeof(TActor).IsAssignableFrom(typeof(TNpcActor)));
        Assert.True(typeof(TNpcActor).IsAssignableFrom(typeof(TStatuaryNpcActor)));
        Assert.Equal("TNpcActor", new TNpcActor().ActorClass);
        Assert.Equal("TStatuaryNpcActor", new TStatuaryNpcActor().ActorClass);
    }
}
