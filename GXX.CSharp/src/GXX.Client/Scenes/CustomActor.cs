using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

// ============================================================================
// 源单元：Source\Client-HGE\CustomActor.pas（1,130 行，GBK）
//
// 本文件 1:1 移植 TCustomActor（class(TActor)）的全部 8 个成员：
//   Create        :61-70
//   CalcActorFrame:72-401   （330 行，本单元最大；虚覆写 TActor.CalcActorFrame）
//   LoadSurface   :402-584  （虚覆写 TActor.LoadSurface）
//   GetDefaultFrame:585-639 （虚覆写 TActor.GetDefaultFrame）
//   DrawChr       :640-787  （虚覆写 TActor.DrawChr，含内嵌过程 DrawSelfMagicEffect 648-717）
//   Run           :788-1038 （虚覆写 TActor.Run）
//   RunActSound   :1039-1107（虚覆写 TActor.RunActSound）
//   RunSound      :1108-1130（虚覆写 TActor.RunSound）
//
// ★ 贯穿全单元的"三态前置门"（原文出现 7 次，逐字一致）：
//     if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin inherited; Exit; end;
//   出现位置：CalcActorFrame 80-91、LoadSurface 409-412、GetDefaultFrame 590-593、
//   DrawChr 719-722、Run 804-807、RunActSound 1045-1048、RunSound 1112-1115。
//   即「自定义怪」身份 = (m_nChangeAppr < 0) or (m_btRace = 156)，此门为假才走本单元逻辑，
//   为真则整体退回基类 TActor 的同名实现。
//   （另注：RunActSound 的 `if not m_boRunSound then Exit;`（1043）**先于**该门，
//     是本单元唯一一处门序例外，已用测试固化。）
//
// ★ 虚分派：原文 CalcActorFrame / LoadSurface / GetDefaultFrame / DrawChr / Run /
//   RunActSound / RunSound 全部带 `override`。托管侧此前不具备对应的虚槽位：
//   ActorCore.cs 的 CalcActorFrame / Run(uint) 与 ActorMotion.cs 的 GetDefaultFrame 非虚，
//   TActor 更没有 LoadSurface / DrawChr / RunSound / RunActSound 四个成员 ——
//   于是**经基类静态类型调用时**（PlaySceneMessages.cs:729、ActorMessages.cs:277/287、
//   ActorMotion.cs:290）本单元实现被通用动作表/空实现静默接管。
//   车道 p7-client-virtual 已修复：基类三处补 `virtual` + TActor 新增 4 个空虚成员，
//   本类 7 个方法全部改为 `override`（原文形状）。
//     - 决策层仍全部抽为纯静态函数（可在无头环境完整单测，不依赖虚分派）；
//     - 多态落点的回归证据：tests/GXX.Client.Tests/VirtualDispatchCustomActorTests.cs
//       （以基类静态类型 TActor 持有本类实例，逐一断言落到本类实现）。
//
// ★ 未移植（接缝）：所有画布绘制调用（GameCanvas.Draw*/GetCachedImage）、
//   真实纹理/图库查找（g_WMonImages / g_EffectImageList）、音频解码播放、
//   TMagicEff 的入列副作用均已抽为返回"绘制操作/生成计划"的纯函数或注入点。
// ============================================================================

/// <summary>
/// CustomActor.pas 的依赖注入接缝（headless：无真实画布 / 图库 / 音频设备 / 场景单例）。
/// </summary>
public static class CustomActorEnv
{
    /// <summary>MyGetTickCount（原文 667/415 等用作秒级 tick）。</summary>
    public static Func<uint> MyGetTickCountFn = () => SceneTime.TickNow();

    /// <summary>TimeGetTime（原文 m_dwStartTime / m_dwCurSelfEffFrameTick 写值）。</summary>
    public static Func<uint> TimeGetTimeFn = () => SceneTime.TickNow();

    /// <summary>Random(8)（原文 1052 转身音随机门）。</summary>
    public static Func<int, int> RandomFn = n => 0;

    /// <summary>g_EffectImageList.Count（EffectIndex/EffectFile 越界判定）。</summary>
    public static Func<int> EffectImageListCountFn = () => 0;

    /// <summary>g_ClientConfig.boHideGhost（LoadSurface 421 幽灵隐藏门）。</summary>
    public static bool ClientConfigBoHideGhost;

    /// <summary>g_ConfigDlg.ConfigCheckeds[ckHideGhost]。</summary>
    public static bool ConfigDlgCkHideGhost;

    /// <summary>PlugInEnabled（LoadSurface 421）。</summary>
    public static bool PlugInEnabled;

    /// <summary>GameCanvas.Active / GameCanvas.Initialized（LoadSurface 414 双门）。</summary>
    public static bool GameCanvasActive;
    public static bool GameCanvasInitialized;

    /// <summary>PlaySound(FileName) 接缝（CustomActor.pas 唯一取声入口）。</summary>
    public static Action<string> PlaySoundFn = _ => { };

    /// <summary>g_PlaySound.PlaySound(id) 接缝（1122 武器受击音）。</summary>
    public static Action<int> PlaySoundByIdFn = _ => { };

    /// <summary>ScreenXYfromMCXY（351/876/945/946）。</summary>
    public static Action<int, int, RefInt, RefInt> ScreenXYfromMCXYFn =
        (cx, cy, sx, sy) => { sx.Value = cx * 48; sy.Value = cy * 32; };

    /// <summary>GetNextDirection（695：8 方向）。</summary>
    public static Func<int, int, int, int, int> GetNextDirectionFn = (sx, sy, ex, ey) => 0;

    /// <summary>GetFlyDirection16（697/953：16 方向）。</summary>
    public static Func<int, int, int, int, int> GetFlyDirection16Fn = (sx, sy, ex, ey) => 0;

    /// <summary>GetFlyDirection（951：8 方向飞行）。</summary>
    public static Func<int, int, int, int, int> GetFlyDirectionFn = (sx, sy, ex, ey) => 0;

    /// <summary>
    /// LoadSurface 548-579 的取图接缝（giBody/giBodyEffect/giBodyEffect2 的
    /// GetCachedImage / GetCachedGrayImage / GetCachedBrightImage）。
    /// kind ∈ {"image","gray","bright"}；返回 null 表示未取到图（各段自身的 nil 判定）。
    /// </summary>
    public static Func<int, int, int, int, string, object?> FetchSurfaceFn = (_, _, _, _, _) => null;

    /// <summary>DrawSelfMagicEffect 690/699 的 `GetCachedImage(idx, px, py)` 出参偏移接缝。</summary>
    public static Func<int, int, (int Px, int Py)> GetCachedImageOffsetFn = (_, _) => (0, 0);

    /// <summary>DrawSelfMagicEffect 706 的 `SelfEffectSurface <> nil` 存在性接缝。</summary>
    public static Func<int, int, bool> SurfaceExistsFn = (_, _) => false;

    /// <summary>GameCanvas.Draw / DrawBlend（708-711 / 729-782）接缝。</summary>
    public static Action<int, int, object?, string> DrawSurfaceFn = (_, _, _, _) => { };

    /// <summary>
    /// Run 目标/飞行特效的实例化 + AddEffectList 接缝（原文 899-908 / 923-931 / 980-1028）。
    /// 真正的 TCustomMonTargetEffect / TCustomMonFlyEffect 构造与入列由集成方实现。
    /// </summary>
    public static Action<TCustomActor, CustomActorRunPlan>? SpawnEffectFn;

    public static void Reset()
    {
        MyGetTickCountFn = () => SceneTime.TickNow();
        TimeGetTimeFn = () => SceneTime.TickNow();
        RandomFn = _ => 0;
        EffectImageListCountFn = () => 0;
        ClientConfigBoHideGhost = false;
        ConfigDlgCkHideGhost = false;
        PlugInEnabled = false;
        GameCanvasActive = false;
        GameCanvasInitialized = false;
        PlaySoundFn = _ => { };
        PlaySoundByIdFn = _ => { };
        ScreenXYfromMCXYFn = (cx, cy, sx, sy) => { sx.Value = cx * 48; sy.Value = cy * 32; };
        GetNextDirectionFn = (sx, sy, ex, ey) => 0;
        GetFlyDirection16Fn = (sx, sy, ex, ey) => 0;
        GetFlyDirectionFn = (sx, sy, ex, ey) => 0;
        FetchSurfaceFn = (_, _, _, _, _) => null;
        GetCachedImageOffsetFn = (_, _) => (0, 0);
        SurfaceExistsFn = (_, _) => false;
        DrawSurfaceFn = (_, _, _, _) => { };
        SpawnEffectFn = null;
    }
}

/// <summary>by-ref int 载体（headless 替代 Delphi 的 var 出参）。</summary>
public sealed class RefInt
{
    public int Value;
    public RefInt(int value = 0) => Value = value;
}

/// <summary>CustomActor.pas 帧计算输入（TActor 状态的显式快照，避免依赖基类字段布局）。</summary>
public readonly record struct CustomActorCalcInput(
    int ChangeAppr,          // m_nChangeAppr
    int BtRace,              // m_btRace
    int CurrentAction,       // m_nCurrentAction
    int BtDir,               // m_btDir
    int BtStep,              // m_btStep
    int State,               // m_nState（STATE_STONE_MODE 位）
    int OldChrLight,         // m_nOldChrLight
    int StruckFrameTime,     // m_dwStruckFrameTime
    in TClientCustomMonsterConfig Cfg);

/// <summary>
/// CustomActor.pas `TCustomActor.CalcActorFrame`（72-401）1:1 的纯逻辑结果。
/// 字段名对应原文被写入的 TActor 成员。
/// </summary>
public sealed record CustomActorFramePlan(
    bool UseMagic,                 // m_boUseMagic
    int BodyOffset,                // m_nBodyOffset := 0（96）
    int ChrLight,                  // m_nChrLight
    int StartFrame,                // m_nStartFrame
    int EndFrame,                  // m_nEndFrame
    uint FrameTime,                // m_dwFrameTime
    int MaxTick,                   // m_nMaxTick
    int CurTick,                   // m_nCurTick
    int MoveStep,                  // m_nMoveStep
    int DefFrameCount,             // m_nDefFrameCount（本单元不改的动作为 0，调用方需忽略）
    bool WritesDefFrameCount,      // 该动作是否写 m_nDefFrameCount
    int SpellFrame,                // m_nSpellFrame
    int CurEffFrame,               // m_nCurEffFrame := 0（393）
    int CurSelfEffFrame,           // m_nCurSelfEffFrame := 0（394）
    bool WritesState,              // SM_DIGUP 写 m_nState := 0
    int State,                     // m_nState 新值
    bool WritesWarModeTime,        // SM_HIT 写 m_dwWarModeTime
    bool ResetsCustomMagicStruck,  // SM_STRUCK 写 m_CustomMagicStatusEffect.m_nStruck := 0
    bool WritesCreateEffectFalse,  // Create 内 m_boCreateEffect := False
    int ShiftDir,                  // Shift(dir, step, cur, max)
    int ShiftStep,
    int ShiftCur,
    int ShiftMax,
    bool DoShift,
    /// <summary>FClientAction 指向的动作槽；-1 = nil（SM_LIGHTINGEX / SM_SKELETON 为空实现）。</summary>
    int ClientActionIndex,
    /// <summary>命中的 AttackConfigs 下标；-1 = ClientConfig 仍为 nil。</summary>
    int AttackConfigIndex)
{
    /// <summary>FClientAction 的强类型视图（-1 → null）。</summary>
    public TMonsterClientActionType? ClientActionType
        => ClientActionIndex < 0 ? null : (TMonsterClientActionType)ClientActionIndex;
}

/// <summary>
/// SM_ATTACK01..06 的实值（Grobal2.pas:1938-1943）。
///
/// 托管侧 <see cref="TActorCore"/> 只登记了 SM_ATTACK01 = 8946 与 SM_ATTACK06 = 8951
/// （ActorMessages.cs:324-325），**中间四个缺号**；本单元六个都要用，故在本文件按原文实值补齐
/// （Grobal2.Const.g.cs:1625-1629 有同名常量可交叉核对）。
/// 未改动基类，无重名冲突；若日后 TActorCore 补齐，本类应改为直接引用。
/// </summary>
public static class CustomActorAttackActions
{
    /// <summary>Grobal2.pas:1938 — SM_ATTACK01 = 8946（自定义攻击1）。</summary>
    public const int SM_ATTACK01 = 8946;
    /// <summary>Grobal2.pas:1939 — SM_ATTACK02 = 8947。</summary>
    public const int SM_ATTACK02 = 8947;
    /// <summary>Grobal2.pas:1940 — SM_ATTACK03 = 8948。</summary>
    public const int SM_ATTACK03 = 8948;
    /// <summary>Grobal2.pas:1941 — SM_ATTACK04 = 8949。</summary>
    public const int SM_ATTACK04 = 8949;
    /// <summary>Grobal2.pas:1942 — SM_ATTACK05 = 8950。</summary>
    public const int SM_ATTACK05 = 8950;
    /// <summary>Grobal2.pas:1943 — SM_ATTACK06 = 8951（自定义攻击6）。</summary>
    public const int SM_ATTACK06 = 8951;
}

/// <summary>SM_ATTACK01..06 对应的 AttackConfigs 槽（原文 283-324 顺序）。</summary>
public static class CustomActorAttackSlot
{
    /// <summary>SM_ATTACK01..SM_ATTACK06（原文 case 标签顺序）。</summary>
    public static readonly int[] AttackActions =
    {
        CustomActorAttackActions.SM_ATTACK01, CustomActorAttackActions.SM_ATTACK02, CustomActorAttackActions.SM_ATTACK03,
        CustomActorAttackActions.SM_ATTACK04, CustomActorAttackActions.SM_ATTACK05, CustomActorAttackActions.SM_ATTACK06,
    };

    /// <summary>SM_ATTACK01 → 0 … SM_ATTACK06 → 5；非攻击动作 → -1。</summary>
    public static int SlotOf(int currentAction)
    {
        for (int i = 0; i < AttackActions.Length; i++)
            if (AttackActions[i] == currentAction)
                return i;
        return -1;
    }

    /// <summary>DrawSelfMagicEffect / Run 里 `-1 * SM_ATTACKxx` 的效果号 → AttackConfigs 槽。</summary>
    public static int SlotOfEffectNumber(int effectNumber)
    {
        for (int i = 0; i < AttackActions.Length; i++)
            if (-1 * AttackActions[i] == effectNumber)
                return i;
        return -1;
    }
}

/// <summary>自定义怪身份判定（原文 6 处前置门，逐字一致）。</summary>
public static class CustomActorGate
{
    /// <summary>
    /// 原文形态：`(m_nChangeAppr &gt;= 0) and (m_btRace &lt;&gt; 156)`。
    /// 为真 → 退回基类；为假 → 走 TCustomActor 本单元逻辑。
    /// </summary>
    public static bool BypassToInherited(int changeAppr, int btRace)
        => changeAppr >= 0 && btRace != 156;

    /// <summary>自定义怪 race 号。</summary>
    public const int CustomMonsterRace = 156;
}

/// <summary>
/// CustomActor.pas `TCustomActor` 的纯决策层 1:1。
/// 全部函数无副作用（除通过 out/返回表达原文被写入的字段）。
/// </summary>
public static class CustomActorLogic
{
    /// <summary>Actions[type] 读取（TClientCustomMonsterConfig.Actions 为 InlineArray）。</summary>
    public static TMonsterClientAction Action(in TClientCustomMonsterConfig cfg, TMonsterClientActionType type)
        => cfg.Actions[(int)type];

    /// <summary>
    /// `TCustomActor.CalcActorFrame`（72-401）1:1。
    /// 返回 null = 命中 80-91 前置门，调用方须改调基类 TActor.CalcActorFrame
    /// （此时原文已把 SM_ATTACK01..06 改写为 SM_HIT，改写值经 <paramref name="newCurrentAction"/> 传出）。
    /// </summary>
    public static CustomActorFramePlan? CalcActorFrame(in CustomActorCalcInput input, out int newCurrentAction)
    {
        newCurrentAction = input.CurrentAction;

        // 80-91
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
        {
            if (input.CurrentAction == CustomActorAttackActions.SM_ATTACK01 ||
                input.CurrentAction == CustomActorAttackActions.SM_ATTACK02 ||
                input.CurrentAction == CustomActorAttackActions.SM_ATTACK03 ||
                input.CurrentAction == CustomActorAttackActions.SM_ATTACK04 ||
                input.CurrentAction == CustomActorAttackActions.SM_ATTACK05 ||
                input.CurrentAction == CustomActorAttackActions.SM_ATTACK06)
                newCurrentAction = TActorCore.SM_HIT;
            return null;
        }

        var cfg = input.Cfg;

        // 93-98：计算前的无条件重置
        bool useMagic = false;                  // 93：m_boUseMagic := FALSE
        int chrLight = input.OldChrLight;       // 98：m_nChrLight := m_nOldChrLight

        int shiftStep = 0;
        int shiftMax = 1;
        int moveStep = 1;

        switch (input.CurrentAction)
        {
            // 102-133：0, SM_TURN
            case 0:
            case TActorCore.SM_TURN:
                {
                    if ((input.State & ActorStates.STATE_STONE_MODE) != 0)
                    {
                        // 103-116：石化复活，StartFrame := EndFrame（单帧定格）
                        var a = Action(cfg, TMonsterClientActionType.matStoneRevive);
                        int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                        int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                        return new CustomActorFramePlan(
                            UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                            StartFrame: startFrame, EndFrame: startFrame, FrameTime: a.PlayTime,
                            MaxTick: 0, CurTick: 0, MoveStep: 1,
                            DefFrameCount: a.PlayCount, WritesDefFrameCount: true,
                            SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                            WritesState: false, State: 0, WritesWarModeTime: false,
                            ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                            ShiftDir: input.BtDir, ShiftStep: 0, ShiftCur: 0, ShiftMax: 1,
                            DoShift: true,
                            ClientActionIndex: (int)TMonsterClientActionType.matStoneRevive,
                            AttackConfigIndex: -1);
                    }

                    // 118-130：站立
                    var stand = Action(cfg, TMonsterClientActionType.matStand);
                    int standDir = stand.CalcDir != 0 ? input.BtDir : 0;
                    int standStart = stand.StartIndex + standDir * (stand.PlayCount + stand.EmptyCount);
                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: standStart, EndFrame: standStart + stand.PlayCount - 1,
                        FrameTime: stand.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: 1,
                        DefFrameCount: stand.PlayCount, WritesDefFrameCount: true,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: false, State: 0, WritesWarModeTime: false,
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: input.BtDir, ShiftStep: 0, ShiftCur: 0, ShiftMax: 1,
                        DoShift: true,
                        ClientActionIndex: (int)TMonsterClientActionType.matStand,
                        AttackConfigIndex: -1);
                }

            // 134-154：SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP
            case TActorCore.SM_WALK:
            case TActorCore.SM_RUSH:
            case TActorCore.SM_RUSHKUNG:
            case TActorCore.SM_BACKSTEP:
                {
                    var a = Action(cfg, TMonsterClientActionType.matWalk);
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                    int endFrame = startFrame + a.PlayCount - 1;

                    // 148-153：SM_BACKSTEP 走反方向且步数取 m_btStep
                    int dir;
                    int step;
                    if (input.CurrentAction == TActorCore.SM_BACKSTEP)
                    {
                        dir = TActorCore.GetBack(input.BtDir);
                        step = input.BtStep;
                    }
                    else
                    {
                        dir = input.BtDir;
                        step = 1;
                    }

                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: endFrame, FrameTime: a.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: step,
                        DefFrameCount: 0, WritesDefFrameCount: false,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: false, State: 0, WritesWarModeTime: false,
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: dir, ShiftStep: step, ShiftCur: 0,
                        ShiftMax: endFrame - startFrame + 1,
                        DoShift: true,
                        ClientActionIndex: (int)TMonsterClientActionType.matWalk,
                        AttackConfigIndex: -1);
                }

            // 155-173：SM_DIGUP（出土；写 m_nState := 0）
            case TActorCore.SM_DIGUP:
                {
                    var a = Action(cfg, TMonsterClientActionType.matStoneRevive);
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                    int endFrame = startFrame + a.PlayCount - 1;
                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: endFrame, FrameTime: a.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: 1,
                        DefFrameCount: a.PlayCount, WritesDefFrameCount: true,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: true, State: 0,               // 170：m_nState := 0
                        WritesWarModeTime: false,
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: input.BtDir, ShiftStep: 0, ShiftCur: 0, ShiftMax: 1,
                        DoShift: true,
                        ClientActionIndex: (int)TMonsterClientActionType.matStoneRevive,
                        AttackConfigIndex: -1);
                }

            // 174-205：SM_LIGHTINGEX —— 整段被原文注释掉，本体为空实现（不写任何字段、不 Shift）
            case TActorCore.SM_LIGHTINGEX:
                return new CustomActorFramePlan(
                    UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                    StartFrame: 0, EndFrame: 0, FrameTime: 0,
                    MaxTick: 0, CurTick: 0, MoveStep: 0,
                    DefFrameCount: 0, WritesDefFrameCount: false,
                    SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                    WritesState: false, State: 0, WritesWarModeTime: false,
                    ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                    ShiftDir: 0, ShiftStep: 0, ShiftCur: 0, ShiftMax: 0, DoShift: false,
                    ClientActionIndex: -1, AttackConfigIndex: -1);

            // 206-229：SM_HIT（写 m_dwWarModeTime；220-228 整段注释保留）
            case TActorCore.SM_HIT:
                {
                    var a = Action(cfg, TMonsterClientActionType.matDefAttack);
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                    int endFrame = startFrame + a.PlayCount - 1;
                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: endFrame, FrameTime: a.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: 1,
                        DefFrameCount: 0, WritesDefFrameCount: false,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: false, State: 0,
                        WritesWarModeTime: true,                    // 217
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: input.BtDir, ShiftStep: 0, ShiftCur: 0, ShiftMax: 1,
                        DoShift: true,
                        ClientActionIndex: (int)TMonsterClientActionType.matDefAttack,
                        AttackConfigIndex: -1);
                }

            // 230-251：SM_STRUCK（帧时间取 m_dwStruckFrameTime；清 CustomMagicStatusEffect.m_nStruck）
            case TActorCore.SM_STRUCK:
                {
                    var a = Action(cfg, TMonsterClientActionType.matStruck);
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                    int endFrame = startFrame + a.PlayCount - 1;
                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: endFrame,
                        FrameTime: (uint)input.StruckFrameTime,      // 239：m_dwStruckFrameTime
                        MaxTick: 0, CurTick: 0, MoveStep: 1,
                        DefFrameCount: 0, WritesDefFrameCount: false,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: false, State: 0, WritesWarModeTime: false,
                        ResetsCustomMagicStruck: true,                // 243
                        WritesCreateEffectFalse: false,
                        ShiftDir: input.BtDir, ShiftStep: 0, ShiftCur: 0, ShiftMax: 1,
                        DoShift: true,
                        ClientActionIndex: (int)TMonsterClientActionType.matStruck,
                        AttackConfigIndex: -1);
                }

            // 252-263：SM_DEATH（StartFrame 直接取末帧，EndFrame := StartFrame，无 Shift）
            case TActorCore.SM_DEATH:
                {
                    var a = Action(cfg, TMonsterClientActionType.matDie);
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount) + a.PlayCount - 1;
                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: startFrame, FrameTime: a.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: 1,
                        DefFrameCount: 0, WritesDefFrameCount: false,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: false, State: 0, WritesWarModeTime: false,
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: 0, ShiftStep: 0, ShiftCur: 0, ShiftMax: 0, DoShift: false,
                        ClientActionIndex: (int)TMonsterClientActionType.matDie,
                        AttackConfigIndex: -1);
                }

            // 264-275：SM_NOWDEATH（整段动画，无 Shift）
            case TActorCore.SM_NOWDEATH:
                {
                    var a = Action(cfg, TMonsterClientActionType.matDie);
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                    int endFrame = startFrame + a.PlayCount - 1;
                    return new CustomActorFramePlan(
                        UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: endFrame, FrameTime: a.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: 1,
                        DefFrameCount: 0, WritesDefFrameCount: false,
                        SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                        WritesState: false, State: 0, WritesWarModeTime: false,
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: 0, ShiftStep: 0, ShiftCur: 0, ShiftMax: 0, DoShift: false,
                        ClientActionIndex: (int)TMonsterClientActionType.matDie,
                        AttackConfigIndex: -1);
                }

            // 276-278：SM_SKELETON —— 空实现（ClientAction 保持 nil）
            case TActorCore.SM_SKELETON:
                return new CustomActorFramePlan(
                    UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                    StartFrame: 0, EndFrame: 0, FrameTime: 0,
                    MaxTick: 0, CurTick: 0, MoveStep: 0,
                    DefFrameCount: 0, WritesDefFrameCount: false,
                    SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                    WritesState: false, State: 0, WritesWarModeTime: false,
                    ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                    ShiftDir: 0, ShiftStep: 0, ShiftCur: 0, ShiftMax: 0, DoShift: false,
                    ClientActionIndex: -1, AttackConfigIndex: -1);

            // 279-396：SM_ATTACK01..06
            case CustomActorAttackActions.SM_ATTACK01:
            case CustomActorAttackActions.SM_ATTACK02:
            case CustomActorAttackActions.SM_ATTACK03:
            case CustomActorAttackActions.SM_ATTACK04:
            case CustomActorAttackActions.SM_ATTACK05:
            case CustomActorAttackActions.SM_ATTACK06:
                {
                    int slot = CustomActorAttackSlot.SlotOf(input.CurrentAction);

                    // 281-330：按动作选 Actions[matAttackN] 与 AttackConfigs[N]
                    TMonsterClientActionType actionType = slot switch
                    {
                        0 => TMonsterClientActionType.matAttack1,
                        1 => TMonsterClientActionType.matAttack2,
                        2 => TMonsterClientActionType.matAttack3,
                        3 => TMonsterClientActionType.matAttack4,
                        4 => TMonsterClientActionType.matAttack5,
                        _ => TMonsterClientActionType.matAttack6,
                    };

                    var a = Action(cfg, actionType);
                    var cc = cfg.AttackConfigs[slot];

                    // 286-288：Self_LightRange > 0 时覆盖 m_nChrLight
                    if (cc.Self_LightRange > 0)
                        chrLight = cc.Self_LightRange;

                    // 332-340
                    int tempDir = a.CalcDir != 0 ? input.BtDir : 0;
                    int startFrame = a.StartIndex + tempDir * (a.PlayCount + a.EmptyCount);
                    int endFrame = startFrame + a.PlayCount - 1;

                    // 342-343：进入魔法态
                    useMagic = true;
                    int spellFrame = a.PlayCount;

                    // 346-391：Self_StartIndex >= 0 且 Self_PlayCount > 0 时才生成环绕特效；
                    // 380-390 的 else if Self_PlayDelayAction 分支整段被注释，故不实现。
                    // 350-377 的 mdctCenter 分支生成 nDirCount 个 TMagicEff（副作用，走 Run/调用方接缝）。

                    return new CustomActorFramePlan(
                        UseMagic: true, BodyOffset: 0, ChrLight: chrLight,
                        StartFrame: startFrame, EndFrame: endFrame, FrameTime: a.PlayTime,
                        MaxTick: 0, CurTick: 0, MoveStep: 0,
                        DefFrameCount: 0, WritesDefFrameCount: false,
                        SpellFrame: spellFrame,
                        CurEffFrame: 0,                 // 393
                        CurSelfEffFrame: 0,             // 394
                        WritesState: false, State: 0, WritesWarModeTime: false,
                        ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                        ShiftDir: input.BtDir, ShiftStep: 0, ShiftCur: 0, ShiftMax: 1,
                        DoShift: true,                  // 395
                        ClientActionIndex: (int)actionType,
                        AttackConfigIndex: slot);
                }

            // 原文 case 无 else；未列入的动作 ClientAction 保持 nil、m_nMoveStep 保持 1（147 已置）
            default:
                return new CustomActorFramePlan(
                    UseMagic: false, BodyOffset: 0, ChrLight: chrLight,
                    StartFrame: 0, EndFrame: 0, FrameTime: 0,
                    MaxTick: 0, CurTick: 0, MoveStep: 0,
                    DefFrameCount: 0, WritesDefFrameCount: false,
                    SpellFrame: 0, CurEffFrame: 0, CurSelfEffFrame: 0,
                    WritesState: false, State: 0, WritesWarModeTime: false,
                    ResetsCustomMagicStruck: false, WritesCreateEffectFalse: false,
                    ShiftDir: 0, ShiftStep: 0, ShiftCur: 0, ShiftMax: 0, DoShift: false,
                    ClientActionIndex: -1, AttackConfigIndex: -1);
        }
    }

    /// <summary>
    /// SM_ATTACK01..06 的环绕特效生成参数（原文 350-377）。
    /// mdctCenter 时 nDirCount = 8（mdcDir8）或 16，且 I=1 那一发带 Light。
    /// 名字：`meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0)`。
    /// </summary>
    public sealed record SelfCenterEffectPlan(
        bool Applies,               // 命中 mdctCenter 且 Self_StartIndex>=0 且 Self_PlayCount>0
        int DirCount,               // 8 或 16
        int BaseStart,              // ClientConfig.Self_StartIndex
        int PlayCount,              // ClientConfig.Self_PlayCount
        int EmptyCount,             // ClientConfig.Self_EmptyCount
        int PlayTime,               // ClientConfig.Self_PlayTime
        int LightRange,             // ClientConfig.Self_LightRange
        int SelfFile,               // ClientConfig.Self_File
        bool LightOnSecond,         // I = 1 时 meff.Light := Self_LightRange
        bool TargetActorNil);       // meff.TargetActor := nil

    /// <summary>AttackConfigs 的 mdctCenter 环绕特效参数（350-377）。</summary>
    public static SelfCenterEffectPlan SelfCenterEffect(int currentAction, in TClientCustomMonsterConfig cfg)
    {
        int slot = CustomActorAttackSlot.SlotOf(currentAction);
        if (slot < 0)
            return new SelfCenterEffectPlan(false, 0, 0, 0, 0, 0, 0, 0, false, false);

        var cc = cfg.AttackConfigs[slot];

        // 346：两个前置条件 + ClientConfig <> nil
        if (!(cc.Self_StartIndex >= 0 && cc.Self_PlayCount > 0))
            return new SelfCenterEffectPlan(false, 0, 0, 0, 0, 0, 0, 0, false, false);

        // 350：仅 mdctCenter
        if (cc.Self_DirCalcType != TCustomDirCalcType.mdctCenter)
            return new SelfCenterEffectPlan(false, 0, 0, 0, 0, 0, 0, 0, false, false);

        // 353-357：mdcDir8 → 8，否则 16
        int dirCount = cc.Self_DirCount == TCustomDirCount.mdcDir8 ? 8 : 16;

        return new SelfCenterEffectPlan(
            Applies: true,
            DirCount: dirCount,
            BaseStart: cc.Self_StartIndex,
            PlayCount: cc.Self_PlayCount,
            EmptyCount: cc.Self_EmptyCount,
            PlayTime: cc.Self_PlayTime,
            LightRange: cc.Self_LightRange,
            SelfFile: cc.Self_File,
            LightOnSecond: true,
            TargetActorNil: true);
    }

    /// <summary>361：`Self_StartIndex + I * (Self_PlayCount + Self_EmptyCount)`。</summary>
    public static int SelfCenterEffectFrame(in SelfCenterEffectPlan p, int i)
        => p.BaseStart + i * (p.PlayCount + p.EmptyCount);
}

/// <summary>
/// CustomActor.pas `TCustomActor.GetDefaultFrame`（585-639）1:1 的纯逻辑。
/// </summary>
public readonly record struct CustomActorDefaultFrameInput(
    int ChangeAppr,
    int BtRace,
    int BtDir,
    bool Death,           // m_boDeath
    bool Skeleton,        // m_boSkeleton
    int State,            // m_nState
    int CurrentDefFrame,  // m_nCurrentDefFrame
    int OldChrLight,      // m_nOldChrLight
    in TClientCustomMonsterConfig Cfg);

/// <summary>GetDefaultFrame 结果（含原文写入的 FClientAction 与 m_nChrLight 副作用）。</summary>
public sealed record CustomActorDefaultFramePlan(
    int Frame,
    int ChrLight,
    int ClientActionIndex);

public static class CustomActorDefaultFrame
{
    /// <summary>
    /// `TCustomActor.GetDefaultFrame`（585-639）1:1。
    /// 返回 null = 命中 590-593 前置门，调用方须改调基类。
    /// </summary>
    public static CustomActorDefaultFramePlan? Compute(in CustomActorDefaultFrameInput input)
    {
        // 590-593
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
            return null;

        var cfg = input.Cfg;

        // 596：m_nChrLight := m_nOldChrLight
        int chrLight = input.OldChrLight;

        // 595 的 `Result := 0;` 已被原文注释掉（"下面没条路径均有赋值"），故无默认值。

        if (input.Death)
        {
            // 598-609
            if (input.Skeleton)
            {
                // 599-600：骨架取 die.StartIndex（**不**加方向）
                int frame = CustomActorLogic.Action(cfg, TMonsterClientActionType.matDie).StartIndex;
                return new CustomActorDefaultFramePlan(frame, chrLight,
                    (int)TMonsterClientActionType.matDie);
            }

            var die = CustomActorLogic.Action(cfg, TMonsterClientActionType.matDie);
            int tempDir = die.CalcDir != 0 ? input.BtDir : 0;
            int result = die.StartIndex
                + tempDir * (die.PlayCount + die.EmptyCount)
                + (die.PlayCount - 1);
            return new CustomActorDefaultFramePlan(result, chrLight,
                (int)TMonsterClientActionType.matDie);
        }

        if ((input.State & ActorStates.STATE_STONE_MODE) != 0)
        {
            // 611-619：石化复活取首帧
            var revive = CustomActorLogic.Action(cfg, TMonsterClientActionType.matStoneRevive);
            int tempDir = revive.CalcDir != 0 ? input.BtDir : 0;
            int result = revive.StartIndex + tempDir * (revive.PlayCount + revive.EmptyCount);
            return new CustomActorDefaultFramePlan(result, chrLight,
                (int)TMonsterClientActionType.matStoneRevive);
        }

        // 620-636：站立
        var stand = CustomActorLogic.Action(cfg, TMonsterClientActionType.matStand);

        // 621-626：cf 三重判定（< 0 → 0；>= PlayCount → 0；否则原值）
        int cf;
        if (input.CurrentDefFrame < 0)
            cf = 0;
        else if (input.CurrentDefFrame >= stand.PlayCount)
            cf = 0;
        else
            cf = input.CurrentDefFrame;

        int standDir = stand.CalcDir != 0 ? input.BtDir : 0;
        int standFrame = stand.StartIndex + standDir * (stand.PlayCount + stand.EmptyCount) + cf;
        return new CustomActorDefaultFramePlan(standFrame, chrLight,
            (int)TMonsterClientActionType.matStand);
    }
}

/// <summary>
/// CustomActor.pas `TCustomActor.RunSound`（1108-1130）与 `RunActSound`（1039-1107）1:1 的纯决策层。
/// </summary>
public static class CustomActorSound
{
    /// <summary>
    /// `RunActSound`（1039-1107）1:1。
    /// 返回 null = 命中 1045-1048 前置门或 1043 的 m_boRunSound 门（后者关闭 m_boRunSound 为 false）。
    /// 返回的 <c>CloseRunSound</c> 为 true 表示原文置了 `m_boRunSound := False`。
    /// </summary>
    public static (string Sound, bool CloseRunSound)? RunActSound(
        in CustomActorCalcInput input,
        bool boRunSound,
        int frame)
    {
        // 1043：`if not m_boRunSound then Exit;`（**先于**前置门）
        if (!boRunSound)
            return null;

        // 1045-1048
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
            return null;

        var sounds = input.Cfg.BaseConfig.Sounds;

        switch (input.CurrentAction)
        {
            // 1051-1056：转身，第 1 帧且 Random(8) = 1
            case TActorCore.SM_TURN:
                if (frame == 1 && CustomActorEnv.RandomFn(8) == 1)
                    return (sounds[(int)TMonsterSoundType.mstNormal].Value, true);
                return ("", false);

            // 1057-1062：受击，第 3 帧
            case TActorCore.SM_HIT:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack].Value, true);
                return ("", false);

            // 1063-1098：六个攻击动作，均为第 3 帧、各自 mstAttackN
            case CustomActorAttackActions.SM_ATTACK01:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack1].Value, true);
                return ("", false);
            case CustomActorAttackActions.SM_ATTACK02:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack2].Value, true);
                return ("", false);
            case CustomActorAttackActions.SM_ATTACK03:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack3].Value, true);
                return ("", false);
            case CustomActorAttackActions.SM_ATTACK04:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack4].Value, true);
                return ("", false);
            case CustomActorAttackActions.SM_ATTACK05:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack5].Value, true);
                return ("", false);
            case CustomActorAttackActions.SM_ATTACK06:
                if (frame == 3)
                    return (sounds[(int)TMonsterSoundType.mstAttack6].Value, true);
                return ("", false);

            // 1099-1104：现死，第 2 帧
            case TActorCore.SM_NOWDEATH:
                if (frame == 2)
                    return (sounds[(int)TMonsterSoundType.mstDie].Value, true);
                return ("", false);

            default:
                return ("", false);
        }
    }

    /// <summary>
    /// `RunSound`（1108-1130）1:1。
    /// 返回 null = 命中 1112-1115 前置门；否则返回要播的声音列表
    /// （`SM_STRUCK` 可能两发：mstStruck + m_nStruckWeaponSound &gt;= 0）。
    /// 副作用：无论动作如何，1117 恒置 `m_boRunSound := True`、1118 调 SetSound。
    /// </summary>
    public static (List<string> Sounds, int StruckWeaponSound)? RunSound(
        in CustomActorCalcInput input,
        int struckWeaponSound)
    {
        // 1112-1115
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
            return null;

        var sounds = input.Cfg.BaseConfig.Sounds;
        var result = new List<string>();

        switch (input.CurrentAction)
        {
            // 1120-1123：受击音 + 可选武器音（各自独立条件）
            case TActorCore.SM_STRUCK:
                result.Add(sounds[(int)TMonsterSoundType.mstStruck].Value);
                if (struckWeaponSound >= 0)
                    result.Add("<weapon:" + struckWeaponSound + ">");
                break;

            // 1124-1126：出土音
            case TActorCore.SM_DIGUP:
                result.Add(sounds[(int)TMonsterSoundType.mstDigUP].Value);
                break;
        }

        return (result, struckWeaponSound);
    }
}

// ============================================================================
// LoadSurface（402-584）/ DrawChr（640-787）/ Run（788-1038）的纯决策层
// ============================================================================

/// <summary>LoadSurface 的输入快照（TActor + TCustomActor 状态）。</summary>
public readonly record struct CustomActorSurfaceInput(
    int ChangeAppr,       // m_nChangeAppr
    int BtRace,           // m_btRace
    bool Death,           // m_boDeath
    bool ReverseFrame,    // m_boReverseFrame
    int CurrentFrame,     // m_nCurrentFrame（= nBodyOffset，502）
    int BtDir,            // m_btDir
    int Appearance,       // m_wAppearance
    in TClientCustomMonsterConfig Cfg);

/// <summary>
/// LoadSurface 结果（402-584）。
/// <see cref="Hidden"/> = 原文 421-423 的幽灵隐藏门命中（调用 Finalize 后**不再**装纹理）。
/// </summary>
public sealed record CustomActorSurfacePlan(
    bool Hidden,
    bool LoadBody,
    int BodyFileIndex,
    int BodyOffset,
    bool LoadEffect,
    int EffectFileIndex,
    int EffectOffset,
    bool LoadEffect2,
    int EffectFile2Index,
    int Effect2Offset)
{
    /// <summary>giBody 是否为 WMonImages.Images[m_wAppearance]（ActionFile 越界时）。</summary>
    public bool BodyFallsBackToWMon;
    /// <summary>giBodyEffect 是否回落 WMonImages。</summary>
    public bool EffectFallsBackToWMon;
    /// <summary>giBodyEffect2 是否回落 WMonImages。</summary>
    public bool Effect2FallsBackToWMon;
}

public static class CustomActorSurface
{
    /// <summary>
    /// 原文 421 的幽灵隐藏门（PlugInEnabled and g_ClientConfig.boHideGhost and
    /// g_ConfigDlg.ConfigCheckeds[ckHideGhost] and m_boDeath and
    /// (not ((m_wAppearance >= 900) and (m_wAppearance <= 906)))）。
    /// </summary>
    public static bool HideGhost(bool death, int appearance)
        => CustomActorEnv.PlugInEnabled
           && CustomActorEnv.ClientConfigBoHideGhost
           && CustomActorEnv.ConfigDlgCkHideGhost
           && death
           && !(appearance >= 900 && appearance <= 906);

    /// <summary>ActionFile / EffectFile / EffectFile2 的图库选择（497-500 / 505-508 / 527-530）。</summary>
    private static (int Index, bool Fallback) ResolveLib(int file, int appearance)
        => file >= 0 && file < CustomActorEnv.EffectImageListCountFn()
            ? (file, false)      // g_EffectImageList.Objects[file]
            : (appearance, true); // g_WMonImages.Images[m_wAppearance]

    /// <summary>
    /// `TCustomActor.LoadSurface`（402-584）1:1。
    /// 返回 null = 命中 409-412 前置门，调用方须改调基类。
    /// </summary>
    public static CustomActorSurfacePlan? Compute(in CustomActorSurfaceInput input)
    {
        // 409-412
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
            return null;

        // 414：画布未就绪 → 整体 Exit（不改任何状态）
        if (!CustomActorEnv.GameCanvasActive || !CustomActorEnv.GameCanvasInitialized)
            return null;

        // 415-419：无条件重置
        // m_dwLoadSurfaceTime := MyGetTickCount; m_boLoadSurface := False;
        // m_BodySurface / m_BodyEffectSurface / m_BodyEffect2Surface := nil

        // 421-423：幽灵隐藏（注意 423 的 Finalize 是**其它单元**的方法，此处只表达分支）
        if (HideGhost(input.Death, input.Appearance))
            return new CustomActorSurfacePlan(true, false, 0, 0, false, 0, 0, false, 0, 0);

        // 426-483：整段被原文注释掉的 case（按 m_nCurrentAction 选 ClientAction）——不实现。

        // 487：ClientAction := FClientAction（由 CalcActorFrame 记录）
        var actions = input.Cfg.Actions;

        bool loadBody = false, loadEffect = false, loadEffect2 = false;
        int bodyFile = 0, bodyOffset = 0;
        int effectFile = 0, effectOffset = 0;
        int effect2File = 0, effect2Offset = 0;
        bool bodyFallback = false, effectFallback = false, effect2Fallback = false;

        // 496-546：只有 ClientAction <> nil 且 StartIndex >= 0 且 PlayCount > 0 才装
        // （ClientAction 由 CalcActorFrame 落值；此处遍历所有动作槽以匹配 FClientAction）
        foreach (var ca in AllActions(input.Cfg))
        {
            if (!(ca.StartIndex >= 0 && ca.PlayCount > 0))
                continue;

            // 497-500：giBody
            (bodyFile, bodyFallback) = ResolveLib(ca.ActionFile, input.Appearance);
            loadBody = true;

            // 502：nBodyOffset := m_nCurrentFrame
            bodyOffset = input.CurrentFrame;

            // 504-524：EffectIndex >= 0
            if (ca.EffectIndex >= 0)
            {
                (effectFile, effectFallback) = ResolveLib(ca.EffectFile, input.Appearance);
                loadEffect = true;
                effectOffset = EffectOffsetOf(ca, input.BtDir, input.CurrentFrame,
                    input.Cfg.BaseConfig.DieNoCalcDir);
            }

            // 526-545：EffectIndex2 >= 0
            if (ca.EffectIndex2 >= 0)
            {
                (effect2File, effect2Fallback) = ResolveLib(ca.EffectFile2, input.Appearance);
                loadEffect2 = true;
                effect2Offset = EffectOffset2Of(ca, input.BtDir, input.CurrentFrame,
                    input.Cfg.BaseConfig.DieNoCalcDir);
            }

            break; // FClientAction 只有一个
        }

        return new CustomActorSurfacePlan(
            Hidden: false,
            LoadBody: loadBody, BodyFileIndex: bodyFile, BodyOffset: bodyOffset,
            LoadEffect: loadEffect, EffectFileIndex: effectFile, EffectOffset: effectOffset,
            LoadEffect2: loadEffect2, EffectFile2Index: effect2File, Effect2Offset: effect2Offset)
        {
            BodyFallsBackToWMon = bodyFallback,
            EffectFallsBackToWMon = effectFallback,
            Effect2FallsBackToWMon = effect2Fallback,
        };
    }

    /// <summary>
    /// 遍历 12 个动作槽（InlineArray，无法直接 foreach；原文按 FClientAction 单个指针使用）。
    /// 不用迭代器：C# 迭代器不允许 `in` 参数。
    /// </summary>
    private static TMonsterClientAction[] AllActions(in TClientCustomMonsterConfig cfg)
    {
        var all = new TMonsterClientAction[12];
        for (int i = 0; i < 12; i++)
            all[i] = cfg.Actions[i];
        return all;
    }

    /// <summary>
    /// 512-523：效果偏移。
    /// CalcDir 时 TempOffset = (m_nCurrentFrame - StartIndex) mod (PlayCount + EmptyCount)；
    /// 若该动作是 matDie 且 BaseConfig.DieNoCalcDir → EffectIndex + TempOffset（**不加方向步长**），
    /// 否则 EffectIndex + TempDir * (PlayCount + EmptyCount) + TempOffset；
    /// 非 CalcDir 时 TempOffset = m_nCurrentFrame - StartIndex 且 nBodyEffectOffset := EffectIndex + TempOffset。
    /// </summary>
    public static int EffectOffsetOf(in TMonsterClientAction ca, int btDir, int currentFrame, int dieNoCalcDir)
    {
        if (ca.CalcDir != 0)
        {
            int tempDir = btDir;
            int tempOffset = (currentFrame - ca.StartIndex) % (ca.PlayCount + ca.EmptyCount);
            if (ca.ActionType == TMonsterClientActionType.matDie && dieNoCalcDir != 0)
                return ca.EffectIndex + tempOffset;
            return ca.EffectIndex + tempDir * (ca.PlayCount + ca.EmptyCount) + tempOffset;
        }

        int plain = currentFrame - ca.StartIndex;
        return ca.EffectIndex + plain;
    }

    /// <summary>534-544：EffectIndex2 版，与 <see cref="EffectOffsetOf"/> 同构（读 EffectIndex2 / EffectFile2）。</summary>
    public static int EffectOffset2Of(in TMonsterClientAction ca, int btDir, int currentFrame, int dieNoCalcDir)
    {
        if (ca.CalcDir != 0)
        {
            int tempDir = btDir;
            int tempOffset = (currentFrame - ca.StartIndex) % (ca.PlayCount + ca.EmptyCount);
            if (ca.ActionType == TMonsterClientActionType.matDie && dieNoCalcDir != 0)
                return ca.EffectIndex2 + tempOffset;
            return ca.EffectIndex2 + tempDir * (ca.PlayCount + ca.EmptyCount) + tempOffset;
        }

        int plain = currentFrame - ca.StartIndex;
        return ca.EffectIndex2 + plain;
    }

    /// <summary>
    /// 548-579 的取图分派：ceGrayScale / ceGrayScale2 → GetCachedGrayImage；
    /// ceBright → GetCachedBrightImage；其余 → GetCachedImage。
    /// 注意 **Effect2 段（572-576）没有 ceGrayScale2**（原文如此：只列 ceGrayScale / ceBright），
    /// 故该段遇 ceGrayScale2 落 else 走普通 GetCachedImage。
    /// 全部包在 `if not m_boReverseFrame` 内。
    /// </summary>
    public static string SurfaceFetchKind(TColorEffect colorEffect, bool reverseFrame, bool isEffect2)
    {
        if (reverseFrame)
            return "none";      // 549/560/571：m_boReverseFrame 为真时整段不取图

        if (colorEffect == TColorEffect.ceGrayScale)
            return "gray";
        if (!isEffect2 && colorEffect == TColorEffect.ceGrayScale2)
            return "gray";      // 仅 Body(551) 与 Effect1(562) 段认 ceGrayScale2
        if (colorEffect == TColorEffect.ceBright)
            return "bright";
        return "image";
    }

    /// <summary>
    /// 548-579 逐段判定，返回 (Body, Effect1, Effect2) 三段各自的取图种类。
    /// Effect2 段**不认 ceGrayScale2**（原文 573 只写 ceGrayScale），故与另两段不同。
    /// </summary>
    public static (string Body, string Effect1, string Effect2) SurfaceFetches(
        TColorEffect colorEffect, bool reverseFrame)
        => (SurfaceFetchKind(colorEffect, reverseFrame, false),
            SurfaceFetchKind(colorEffect, reverseFrame, false),
            SurfaceFetchKind(colorEffect, reverseFrame, true));
}

/// <summary>DrawChr 的输入快照（640-787）。</summary>
public readonly record struct CustomActorDrawInput(
    int ChangeAppr,
    int BtRace,
    int DrawOrder,          // FConfig.BaseConfig.DrawOrder
    int DrawMode,           // FConfig.BaseConfig.DrawMode（mdmBlend=0）
    int DrawMode2,          // FConfig.BaseConfig.DrawMode2
    bool HasEffect,         // m_BodyEffectSurface <> nil
    bool HasEffect2);       // m_BodyEffect2Surface <> nil

/// <summary>
/// DrawChr 的绘制序列（640-787）。
/// 元素取值："self" = DrawSelfMagicEffect、"-self" = DrawSelfMagicEffect(False)、
/// "body" = inherited DrawChr、"eff1" / "eff2" = 两块特效（各自按 DrawMode/DrawMode2 选 Blend）。
/// </summary>
public static class CustomActorDraw
{
    /// <summary>DrawOrder 枚举值（Grobal2：mdoSelf_Eff1_Eff2=0, mdoEff1_Self_Eff2=1, mdoEff1_Eff2_Self=2）。</summary>
    public const int MdoSelfEff1Eff2 = 0;
    public const int MdoEff1SelfEff2 = 1;
    public const int MdoEff1Eff2Self = 2;

    /// <summary>
    /// 640-787 的顺序层。返回 null = 命中 719-722 前置门（调用方改调基类 DrawChr）。
    /// **DrawSelfMagicEffect(True) 恒在最前（724）**，且 `mdoSelf_Eff1_Eff2` 分支里
    /// 先 body 再 eff1 再 eff2（725-748）；`mdoEff1_Self_Eff2` 是 eff1 → body → eff2（749-766）；
    /// 其余（含 mdoEff1_Eff2_Self 与任何未知值）是 eff1 → eff2 → body（767-784）；
    /// 最后恒为 DrawSelfMagicEffect(False)（785）。
    /// **注意 mdoEff1_Eff2_Self 与"未知值"落同一 else**，故三值之外的任何 DrawOrder 也走身在后。
    /// </summary>
    public static List<string>? Sequence(in CustomActorDrawInput input)
    {
        // 719-722
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
            return null;

        var seq = new List<string> { "self" };     // 724：DrawSelfMagicEffect(True)

        if (input.DrawOrder == MdoSelfEff1Eff2)
        {
            seq.Add("body");                        // 726
            if (input.HasEffect) seq.Add("eff1");   // 728-740
            if (input.HasEffect2) seq.Add("eff2");  // 742-748
        }
        else if (input.DrawOrder == MdoEff1SelfEff2)
        {
            if (input.HasEffect) seq.Add("eff1");   // 750-756
            seq.Add("body");                        // 758
            if (input.HasEffect2) seq.Add("eff2");  // 760-766
        }
        else
        {
            if (input.HasEffect) seq.Add("eff1");   // 768-774
            if (input.HasEffect2) seq.Add("eff2");  // 776-782
            seq.Add("body");                        // 783
        }

        seq.Add("-self");                           // 785：DrawSelfMagicEffect(False)
        return seq;
    }

    /// <summary>724/785 内嵌过程 DrawSelfMagicEffect 的入口门（651）。</summary>
    public static bool SelfMagicEffectGate(bool useMagic, int curMagicEffectNumber)
        => useMagic && curMagicEffectNumber != 0;

    /// <summary>655-660：效果号 → AttackConfigs 槽（原文为 `-1 * SM_ATTACKxx`）。</summary>
    public static int SelfMagicEffectAttackSlot(int curMagicEffectNumber)
        => CustomActorAttackSlot.SlotOfEffectNumber(curMagicEffectNumber);

    /// <summary>
    /// 664-672 的完整版（Self_PlayCount 与 Self_PlayTime 是**两个不同字段**，勿混用）。
    /// </summary>
    public static (int Frame, uint Tick, bool Ticked) AdvanceSelfFrameEx(
        int curEffFrame, int curSelfEffFrame, uint tick,
        int selfPlayTime, int selfPlayCount, uint myGetTickCount, uint timeGetTime)
    {
        // 664-666
        if (curEffFrame == 0)
            return (0, timeGetTime, true);

        // 667：MyGetTickCount - Cardinal(m_dwCurSelfEffFrameTick) >= Self_PlayTime
        if ((uint)myGetTickCount - tick >= (uint)selfPlayTime)
        {
            int f = curSelfEffFrame;
            // 668-670：m_nCurSelfEffFrame <= Self_PlayCount → Inc
            if (f <= selfPlayCount)
                f++;
            return (f, timeGetTime, true);          // 671：刷新 tick
        }

        return (curSelfEffFrame, tick, false);
    }

    /// <summary>
    /// 675-677：非 8 方向攻击特效的绘制门。
    /// (Self_DrawOrder = mdoPriorMagic) and BeforeDraw
    /// or (Self_DrawOrder = mdoPriorSelf) and (not BeforeDraw)，
    /// 且 Self_DirCalcType &lt;&gt; mdctCenter。
    /// </summary>
    public static bool SelfEffectDrawGate(int selfDrawOrder, bool beforeDraw, int selfDirCalcType)
    {
        bool orderOk = (selfDrawOrder == 1 /*mdoPriorMagic*/ && beforeDraw)      // 675
                    || (selfDrawOrder == 0 /*mdoPriorSelf*/ && !beforeDraw);     // 676
        return orderOk && selfDirCalcType != 2 /*mdctCenter*/;                   // 677
    }

    /// <summary>
    /// 681：`{m_nCurEffFrame} m_nCurSelfEffFrame in [0..Self_PlayCount - 1]`
    /// （被注释的是 m_nCurEffFrame，实际判定用 m_nCurSelfEffFrame）。
    /// </summary>
    public static bool SelfEffectFrameInRange(int curSelfEffFrame, int selfPlayCount,
        int selfStartIndex)
        => selfStartIndex >= 0 && selfPlayCount > 0
           && curSelfEffFrame >= 0 && curSelfEffFrame <= selfPlayCount - 1;

    /// <summary>
    /// 689-702：取图号。
    /// mdctNone → Self_StartIndex + m_nCurSelfEffFrame（**不看方向**）；
    /// mdctNormal → 目标为 (-1,-1) 时用 m_btDir，否则按 mdcDir8 用 GetNextDirection、
    /// 否则用 GetFlyDirection16；图号 = Self_StartIndex + nDir * (PlayCount + EmptyCount) + m_nCurSelfEffFrame。
    /// 返回 null = Self_DirCalcType 既不是 mdctNone 也不是 mdctNormal（原文两分支均未命中，SelfEffectSurface 保持 nil）。
    /// </summary>
    public static int? SelfEffectImageIndex(
        int selfDirCalcType, int selfStartIndex, int selfPlayCount, int selfEmptyCount,
        int curSelfEffFrame, int btDir, int magicTargetX, int magicTargetY,
        int dirCount, int myX, int myY)
    {
        if (selfDirCalcType == 0 /*mdctNone*/)
            return selfStartIndex + curSelfEffFrame;

        if (selfDirCalcType == 1 /*mdctNormal*/)
        {
            int nDir;
            if (magicTargetX == -1 && magicTargetY == -1)
                nDir = btDir;                                            // 692-693
            else if (dirCount == 0 /*mdcDir8*/)
                nDir = CustomActorEnv.GetNextDirectionFn(myX, myY, magicTargetX, magicTargetY);  // 695
            else
                nDir = CustomActorEnv.GetFlyDirection16Fn(myX, myY, magicTargetX, magicTargetY); // 697

            return selfStartIndex + nDir * (selfPlayCount + selfEmptyCount) + curSelfEffFrame;
        }

        return null;    // 689/691 两分支均未命中
    }

    /// <summary>707-711：mdmBlend(0) → DrawBlend，否则 Draw。</summary>
    public static string SelfEffectDrawKind(int selfDrawMode)
        => selfDrawMode == 0 ? "DrawBlend" : "Draw";
}

/// <summary>Run 的输入快照（788-1038）。</summary>
public readonly record struct CustomActorRunInput(
    int ChangeAppr,
    int BtRace,
    bool UseEffect,          // m_boUseEffect
    uint EffectFrameTime,    // m_dwEffectFrameTime
    uint EffectStartTime,    // m_dwEffectStartTime
    int EffectFrame,         // m_nEffectFrame
    int EffectEnd,           // m_nEffectEnd
    bool UseMagic,           // m_boUseMagic
    int CurEffFrame,         // m_nCurEffFrame
    int SpellFrame,          // m_nSpellFrame
    bool CreateEffect,       // m_boCreateEffect
    int CurMagicEffectNumber,// m_CurMagic.EffectNumber
    int Appearance,          // m_wAppearance
    uint Now,                // TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTime
    in TClientCustomMonsterConfig Cfg);

/// <summary>Run 目标特效段（841-939）计划。</summary>
public sealed record CustomActorTargetEffectPlan(
    bool Applies,
    int StartIndex, int StartIndex2, int PlayCount,
    int PlayTime, int DrawMode, int DrawMode2,
    int LightRange, bool LockDraw, bool KeepPlay, int KeepTime,
    int TargetFile,
    bool PerTargetLockDraw);

/// <summary>Run 飞行特效段（941-1029）计划。</summary>
public sealed record CustomActorFlyEffectPlan(
    bool Applies,
    int FlyStartIndex, int FlyPlayCount, int FlyEmptyCount, int FlyPlayTime, int FlyDrawMode,
    int FlyLightRange, int FlyFile, bool FlyCalcDir, int FlyDirCount,
    int ExplosionStartIndex, int ExplosionStartIndex2, int ExplosionPlayCount,
    int ExplosionPlayTime, int ExplosionDrawMode, int ExplosionDrawMode2,
    bool ExplosionLockDraw, int ExplosionLightRange, bool ExplosionKeepPlay, int ExplosionKeepTime,
    int ExplosionFile,
    int FlyEffStartIndex, int FlyEffFile, int FlyEffDrawMode);

/// <summary>Run 的最终结果。</summary>
public sealed record CustomActorRunPlan(
    bool UseEffect,
    bool EffectAdvanced,
    int EffectFrame,
    bool CreateEffect,
    bool EarlyExit,             // 842 的 Exit（整个 Run 返回）
    bool UseMagicAfter,         // m_boUseMagic 是否仍为真（进入 824 分支）
    bool MagicBranchTaken,      // m_nCurEffFrame = m_nSpellFrame - 1
    bool ResetCreateEffect,     // 1033-1035 的 else 分支
    bool EffectSpawned,
    CustomActorTargetEffectPlan? Target,
    CustomActorFlyEffectPlan? Fly);

public static class CustomActorRun
{
    /// <summary>
    /// `TCustomActor.Run`（788-1038）1:1 的决策层（不含 TMagicEff 实例化与入列副作用）。
    /// 返回 null = 命中 804-807 前置门（调用方改调基类 Run）。
    /// </summary>
    public static CustomActorRunPlan? Compute(in CustomActorRunInput input)
    {
        // 804-807
        if (CustomActorGate.BypassToInherited(input.ChangeAppr, input.BtRace))
            return null;

        // 809-820：m_boUseEffect 帧推进
        bool useEffect = input.UseEffect;
        bool effectAdvanced = false;
        int effectFrame = input.EffectFrame;
        if (useEffect)
        {
            // 811：TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime（严格大于）
            if ((uint)input.Now - input.EffectStartTime > input.EffectFrameTime)
            {
                effectAdvanced = true;
                if (effectFrame < input.EffectEnd)
                    effectFrame++;              // 813-814
                else
                    useEffect = false;          // 816-817
            }
        }

        // 822：inherited Run（基类帧推进，由调用方负责）

        // 824：m_boUseMagic 门
        if (!input.UseMagic)
        {
            return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
                input.CreateEffect, EarlyExit: false, UseMagicAfter: false,
                MagicBranchTaken: false, ResetCreateEffect: false, EffectSpawned: false,
                Target: null, Fly: null);
        }

        // 827：m_nCurEffFrame = m_nSpellFrame - 1
        if (input.CurEffFrame != input.SpellFrame - 1)
        {
            // 1033-1035：else → m_boCreateEffect := False
            return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
                CreateEffect: false, EarlyExit: false, UseMagicAfter: true,
                MagicBranchTaken: false, ResetCreateEffect: true, EffectSpawned: false,
                Target: null, Fly: null);
        }

        // 828：not m_boCreateEffect
        if (input.CreateEffect)
        {
            // 内层 if 为假 → 整体什么都不做（m_boCreateEffect 保持 True）
            return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
                CreateEffect: true, EarlyExit: false, UseMagicAfter: true,
                MagicBranchTaken: true, ResetCreateEffect: false, EffectSpawned: false,
                Target: null, Fly: null);
        }

        // 829：m_boCreateEffect := True
        bool createEffect = true;

        // 832-839：效果号 → AttackConfigs 槽
        int slot = CustomActorAttackSlot.SlotOfEffectNumber(input.CurMagicEffectNumber);

        // 842：ClientConfig = nil → Exit（整个 Run 返回）
        if (slot < 0)
        {
            return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
                createEffect, EarlyExit: true, UseMagicAfter: true,
                MagicBranchTaken: true, ResetCreateEffect: false, EffectSpawned: false,
                Target: null, Fly: null);
        }

        var cc = input.Cfg.AttackConfigs[slot];

        // 844：`(Fly_StartIndex < 0) or (Fly_PlayCount <= 0)` —— **或**关系
        bool noFly = cc.Fly_StartIndex < 0 || cc.Fly_PlayCount <= 0;

        if (noFly)
        {
            // 845-847：只有目标播放效果的三门
            bool targetApplies =
                (cc.Target_StartIndex >= 0 || cc.Target_StartIndex2 >= 0)
                && cc.Target_PlayCount > 0
                && (cc.Target_KeepPlay == 0 || cc.Target_KeepTime == 0);

            var plan = new CustomActorTargetEffectPlan(
                Applies: targetApplies,
                StartIndex: cc.Target_StartIndex,
                StartIndex2: cc.Target_StartIndex2,
                PlayCount: cc.Target_PlayCount,
                PlayTime: cc.Target_PlayTime,
                DrawMode: (int)cc.Target_DrawMode,
                DrawMode2: (int)cc.Target_DrawMode2,
                LightRange: cc.Target_LightRange,
                LockDraw: cc.Target_LockDraw != 0,
                KeepPlay: cc.Target_KeepPlay != 0,
                KeepTime: cc.Target_KeepTime,
                TargetFile: cc.Target_File,
                PerTargetLockDraw: cc.Target_LockDraw != 0);

            return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
                createEffect, EarlyExit: false, UseMagicAfter: true,
                MagicBranchTaken: true, ResetCreateEffect: false,
                EffectSpawned: targetApplies, Target: plan, Fly: null);
        }

        // 941：`(Fly_StartIndex >= 0) or (Fly_PlayCount > 0)` —— **或**关系
        // （注意与 844 的否命题并不互补：见报告"原文缺陷/易错点"）
        if (cc.Fly_StartIndex >= 0 || cc.Fly_PlayCount > 0)
        {
            // 949-954：FlyDir
            // 950：mdcDir8(0) → GetFlyDirection(8 方向)，否则 GetFlyDirection16
            var flyPlan = new CustomActorFlyEffectPlan(
                Applies: true,
                FlyStartIndex: cc.Fly_StartIndex,
                FlyPlayCount: cc.Fly_PlayCount,
                FlyEmptyCount: cc.Fly_EmptyCount,
                FlyPlayTime: cc.Fly_PlayTime,
                FlyDrawMode: (int)cc.Fly_DrawMode,
                FlyLightRange: cc.Fly_LightRange,
                FlyFile: cc.Fly_File,
                FlyCalcDir: cc.Fly_CalcDir != 0,
                FlyDirCount: (int)cc.Fly_DirCount,
                ExplosionStartIndex: cc.Explosion_StartIndex,
                ExplosionStartIndex2: cc.Explosion_StartIndex2,
                ExplosionPlayCount: cc.Explosion_PlayCount,
                ExplosionPlayTime: cc.Explosion_PlayTime,
                ExplosionDrawMode: (int)cc.Explosion_DrawMode,
                ExplosionDrawMode2: (int)cc.Explosion_DrawMode2,
                ExplosionLockDraw: cc.Explosion_LockDraw != 0,
                ExplosionLightRange: cc.Explosion_LightRange,
                ExplosionKeepPlay: cc.Explosion_KeepPlay != 0,
                ExplosionKeepTime: cc.Explosion_KeepTime,
                ExplosionFile: cc.Explosion_File,
                FlyEffStartIndex: cc.FlyEff_StartIndex,
                FlyEffFile: cc.FlyEff_File,
                FlyEffDrawMode: (int)cc.FlyEff_DrawMode);

            return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
                createEffect, EarlyExit: false, UseMagicAfter: true,
                MagicBranchTaken: true, ResetCreateEffect: false,
                EffectSpawned: true, Target: null, Fly: flyPlan);
        }

        // 844 与 941 都为假时的落空路径（Fly_StartIndex < 0 且 Fly_PlayCount <= 0 且
        // Fly_StartIndex < 0 且 Fly_PlayCount <= 0 不成立是不可能的组合，见报告）
        return new CustomActorRunPlan(useEffect, effectAdvanced, effectFrame,
            createEffect, EarlyExit: false, UseMagicAfter: true,
            MagicBranchTaken: true, ResetCreateEffect: false, EffectSpawned: false,
            Target: null, Fly: null);
    }

    /// <summary>951-953：飞行方向。Fly_CalcDir 为假时 FlyDir 保持 0。</summary>
    public static int FlyDir(bool flyCalcDir, int flyDirCount,
        int curX, int curY, int targetX, int targetY)
    {
        if (!flyCalcDir)
            return 0;
        return flyDirCount == 0 /*mdcDir8*/
            ? CustomActorEnv.GetFlyDirectionFn(curX, curY, targetX, targetY)
            : CustomActorEnv.GetFlyDirection16Fn(curX, curY, targetX, targetY);
    }

    /// <summary>980：`Fly_StartIndex + FlyDir * (Fly_PlayCount + Fly_EmptyCount)`。</summary>
    public static int FlyBaseFrame(in CustomActorFlyEffectPlan p, int flyDir)
        => p.FlyStartIndex + flyDir * (p.FlyPlayCount + p.FlyEmptyCount);

    /// <summary>1008：`FlyEff_StartIndex + FlyDir * (Fly_PlayCount + Fly_EmptyCount)`（复用 Fly_* 步长）。</summary>
    public static int FlyEffStartFrame(in CustomActorFlyEffectPlan p, int flyDir)
        => p.FlyEffStartIndex + flyDir * (p.FlyPlayCount + p.FlyEmptyCount);

    /// <summary>
    /// 990-999：持续播放时恒置基址 -1 / 爆燃帧 0，否则写真实基址。
    /// </summary>
    public static (int ExplosionBase, int ExplosionBase2, int ExplosionFrame) ExplosionFrames(
        in CustomActorFlyEffectPlan p)
        => p.ExplosionKeepPlay && p.ExplosionKeepTime > 0
            ? (-1, -1, 0)
            : (p.ExplosionStartIndex, p.ExplosionStartIndex2, p.ExplosionPlayCount);

    /// <summary>1001-1002：爆炸混合标志为 `DrawMode = mdmBlend`。</summary>
    public static (bool Blend, bool Blend2) ExplosionBlends(in CustomActorFlyEffectPlan p)
        => (p.ExplosionDrawMode == 0, p.ExplosionDrawMode2 == 0);
}

/// <summary>
/// 目标/飞行特效的"落点"接缝：原文在 Run 内直接 new TMagicEff 并 AddEffectList，
/// 本单元只产出计划；真正的实例化与入列由集成方按此签名接入。
/// </summary>
public static class CustomActorEffectSink
{
    /// <summary>PlayScene.FindActor(m_nTargetRecog) 接缝。</summary>
    public static Func<long, IMagicTarget?> FindActorFn = _ => null;

    /// <summary>PlayScene.AddEffectList(meff) 接缝。</summary>
    public static Action<TMagicEff> AddEffectListFn = _ => { };

    /// <summary>PlayScene.m_EffectList 快照接缝（Target_LockDraw 去重扫描用）。</summary>
    public static Func<IReadOnlyList<TMagicEff>> EffectListFn = () => Array.Empty<TMagicEff>();

    /// <summary>895-910 / 917-933：两段**逐字相同**的"是否已有等价特效"判定（除 targetx/targety 一项）。</summary>
    public static bool ContainsEquivalentTargetEffect(
        in CustomActorTargetEffectPlan p,
        IMagicTarget actor,
        object magOwner,
        object imgLib,
        IReadOnlyList<TMagicEff> effectList,
        int targetX, int targetY, bool useLockDrawForm)
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            if (effectList[i] is not TCustomMonTargetEffect m)
                continue;

            bool eq = m.EffectBase == p.StartIndex
                   && m.MagExplosionBase2 == p.StartIndex2
                   && m.ExplosionFrame == p.PlayCount
                   && m.DrawMode == p.DrawMode
                   && m.DrawMode2 == p.DrawMode2
                   && ReferenceEquals(GetImgLib(m), imgLib)
                   && m.light == p.LightRange
                   && m.NextFrameTime == p.PlayTime
                   && ReferenceEquals(m.MagOwner, magOwner);

            if (!eq)
                continue;

            if (useLockDrawForm)
            {
                // 870：最后一据是 TargetActor = Actor
                if (ReferenceEquals(m.TargetActor, actor))
                    return true;
            }
            else
            {
                // 889-891：最后三据是 MagOwner / targetx / targety
                if (m.targetx == targetX && m.targety == targetY)
                    return true;
            }
        }

        return false;
    }

    private static object? GetImgLib(TMagicEff m) => m.ImgLibId;
}

/// <summary>PlayScene 侧 Run 的调用上下文（m_Saying 多目标广播段 912-938 的输入）。</summary>
public static class CustomActorRunContext
{
    /// <summary>m_Saying（多目标 recogid 逗号串）。</summary>
    public static Func<object, string> SayingFn = _ => "";

    /// <summary>StrToInt64Def(Trim(Targets[I]), 0) 的语义：解析失败返回 0。</summary>
    public static long StrToInt64Def(string s, long def)
    {
        s = GXX.Core.Rtl.DelphiRTL.Trim(s);
        return long.TryParse(s, out long v) ? v : def;
    }

    /// <summary>
    /// 912-938：把 m_Saying 按 ',' 切分，逐项 StrToInt64Def(...,0)，
    /// **非 0 才** FindActor；找到才生成一枚目标特效。返回顺序与去重后的 recogid 列表一致。
    /// 注意 `Targets.Delimiter := ','` 与 `DelimitedText := m_Saying` 是 Delphi TStringList 语义，
    /// 且 918 行对每项做了 Trim。
    /// </summary>
    public static List<long> SayingTargets(string saying)
    {
        var result = new List<long>();
        if (saying.Length == 0)
            return result;                       // 912：Length(m_Saying) > 0 门

        var parts = saying.Split(',');
        for (int i = 0; i < parts.Length; i++)
        {
            long recog = StrToInt64Def(parts[i], 0);
            if (recog != 0)                      // 919：TempTargetRecog <> 0
                result.Add(recog);
        }
        return result;
    }
}

// ============================================================================
// ★ TCustomActor 本体（CustomActor.pas:18-48 声明 / 61-1130 实现）
//
// 虚分派处置（车道 p7-client-virtual 已修完；逐方法）：
//   CalcActorFrame  → 基类 ActorCore.cs:247  已补 `virtual` → 本类 `override`
//   GetDefaultFrame → 基类 ActorMotion.cs:191 已补 `virtual` → 本类 `override`
//   Run(uint)       → 基类 ActorCore.cs:338  已补 `virtual` → 本类 `override`
//   LoadSurface     → 基类新增空虚成员（PlaySceneNewActor.cs TActor）→ 本类 `override`（原带 sender 形参已去掉）
//   DrawChr         → 基类新增空虚成员 → 本类 `override`
//   RunSound        → 基类新增空虚成员（ClEvent 的 LoadSurface/Run 是别的类，与本链无关）→ 本类 `override`
//   RunActSound     → 基类新增空虚成员 → 本类 `override`
// 因此经**基类静态类型**（PlaySceneMessages.cs:729 / ActorMessages.cs:277,287 / ActorMotion.cs:290）
// 调用时，多态**确实**落回本类实现（证据见 tests/GXX.Client.Tests/VirtualDispatchCustomActorTests.cs）。
// 决策逻辑仍全部在纯静态层（可完整单测），本类只做"状态搬运 + 接缝调用"。
//
// 基类缺失、故在本类补齐的 TActor 字段（均已注明；不新增同名基类成员，无重名冲突）：
//   m_nEffectFrame / m_nEffectEnd / m_nStruckWeaponSound / m_ColorEffect /
//   m_BodySurface / m_dwLoadSurfaceTime / m_boLoadSurface / m_boCreateEffect /
//   m_nSpellFrame / m_nCurEffFrame / m_nTargetRecog / m_Saying /
//   m_dwEffectFrameTime / m_dwEffectStartTime
// （注：这些是**字段**，与上面 7 个**虚方法**不同；字段重名冲突问题不在本轮范围内。）
// ============================================================================

/// <summary>
/// CustomActor.pas `TCustomActor`（18-48 / 61-1130）1:1 移植。
/// 自定义怪：帧计算/装载/绘制/推进/音效全部由 <see cref="Config"/>（TClientCustomMonsterConfig）
/// 驱动，而非通用动作表。
/// </summary>
public class TCustomActor : TActor
{
    // ---- CustomActor.pas:20-31 本类私有字段 ----
    private int m_nEffectPx, m_nEffectPy;
    private object? m_BodyEffectSurface;
    private int m_nEffect2Px, m_nEffect2Py;
    private object? m_BodyEffect2Surface;
    private int m_nCurSelfEffFrame;
    private uint m_dwCurSelfEffFrameTick;

    private TClientCustomMonsterConfig FConfig;

    /// <summary>
    /// FClientAction（原文 `PMonsterClientAction` 指针）。
    /// 托管侧 TMonsterClientAction 是 struct，故以**槽位下标**承载指针语义：
    /// -1 = nil（原文 SM_LIGHTINGEX / SM_SKELETON 及未列出动作均为 nil）。
    /// </summary>
    private int FClientActionIndex = -1;

    // ---- CustomActor.pas:33 ----
    /// <summary>
    /// `m_nOldChrLight`（原文 `public`，类型 <c>Integer</c>）—— **真身存储**。
    ///
    /// <para><b>★ 缺陷 H-1 的修复（D-P17-08）</b>：修复前本处是
    /// <c>public int m_nOldChrLight;</c> —— 一个**字段**，它隐藏了基类
    /// <c>TActorCore</c> 的同名 <c>byte</c> 字段。由于字段访问**不参与虚分派**，
    /// <c>PlaySceneMessages.cs:672/677</c>（<c>actor</c> 静态类型 <c>TActorCore</c>）
    /// 写的是基类那一份，而本类方法体（<c>1679</c> / <c>1806</c>）读的是自己这一份
    /// ⇒ 自定义怪的 <c>m_nOldChrLight</c> **恒为 0**（原文语义是"沿用上一轮光照"），
    /// 表现为**自定义怪掉光**，且编译与单测全部通过。</para>
    ///
    /// <para>现改为 <c>override</c> 自动属性：基类提供**虚访问点**（<c>ActorMessages.cs</c>），
    /// 存储留在本类（与原文"字段只属于 <c>TCustomActor</c>"一致），
    /// 写点经虚分派落到真身。</para>
    /// </summary>
    public override int m_nOldChrLight { get; set; }

    // ---- 基类未登记、但本单元需要的 TActor 成员（在本类补齐）----
    // ★ 车道 p7-client-actor-family「继承字段消重」：
    //   下列成员原文里都是 **TActor 的单个字段**，本类早期因基类尚未移植而自建了一份
    //   同名字段（形成字段隐藏 CS0108）—— 那会让"同一份状态"变成**两份不同步的存储**，
    //   一旦基类本体（ActorFamilyBase.cs）读写继承那一份，两处就再也对不上（台账 §25.2 的
    //   静默缺陷形态）。基类现已持有这些字段，故**本类的重复声明一律删除**，一律使用继承字段：
    //     m_nSpellFrame / m_nCurEffFrame / m_nEffectFrame / m_nEffectEnd /
    //     m_nStruckWeaponSound / m_BodySurface / m_dwLoadSurfaceTime / m_boLoadSurface /
    //     m_boRunSound / m_dwEffectFrameTime / m_dwEffectStartTime
    //   `m_ColorEffect` 例外：它改为覆写虚属性 `ActorColorEffect`（见下方），因为基类的存储
    //   历史上为避免字段隐藏而命名为 `m_BaseColorEffect`。

    /// <summary>m_boCreateEffect（原文 TActor 成员）。</summary>
    public bool m_boCreateEffect;
    /// <summary>m_nTargetRecog（原文 TActor 成员；Run 853/942）。</summary>
    public long m_nTargetRecog;
    /// <summary>m_Saying（原文 TActor 成员；Run 912-938 多目标广播）。</summary>
    public string m_Saying = "";
    /// <summary>m_CustomMagicStatusEffect.m_nStruck（原文 243；聚合基类未登记，先以标量承载）。</summary>
    public int m_CustomMagicStatusEffect_Struck;
    /// <summary>m_CurMagic.EffectNumber 接缝（651/832）。</summary>
    public int m_CurMagicEffectNumber;
    /// <summary>m_CurMagic.targx / targy 接缝（692/946）。</summary>
    public int m_CurMagicTargetX = -1;
    public int m_CurMagicTargetY = -1;

    /// <summary>
    /// `constructor TCustomActor.Create`（61-70）1:1。
    /// 注意 68 行是 `m_nChrLight := 1`（**不是** m_nOldChrLight）、69 行 `m_boCreateEffect := False`。
    /// </summary>
    public TCustomActor(TClientCustomMonsterConfig monsterConfig)
    {
        FConfig = monsterConfig;         // 64
        m_BodySurface = null;            // 65
        m_BodyEffectSurface = null;      // 66
        m_BodyEffect2Surface = null;     // 67
        m_nChrLight = 1;                 // 68
        m_boCreateEffect = false;        // 69
    }

    /// <summary>原文 35 行 `reintroduce` 的无参入口（配置随后经 Config 赋入）。</summary>
    public TCustomActor()
    {
        m_BodySurface = null;
        m_BodyEffectSurface = null;
        m_BodyEffect2Surface = null;
        m_nChrLight = 1;
        m_boCreateEffect = false;
    }

    /// <summary>CustomActor.pas:45 — property Config read FConfig write FConfig。</summary>
    public TClientCustomMonsterConfig Config
    {
        get => FConfig;
        set => FConfig = value;
    }

    /// <summary>类名（原 6 行桩上的 ActorClass；PlaySceneNewActor 分派与测试依赖）。</summary>
    public override string ActorClass => "TCustomActor";

    /// <summary>FClientAction 的托管视图（-1 → null）。</summary>
    public TMonsterClientActionType? ClientAction        => FClientActionIndex < 0 ? null : (TMonsterClientActionType)FClientActionIndex;

    private CustomActorCalcInput CalcInput()
        => new(m_nChangeAppr, m_btRace, m_nCurrentAction, m_btDir, m_btStep,
               m_nState, m_nOldChrLight, m_dwStruckFrameTime, in FConfig);

    /// <summary>
    /// `TCustomActor.CalcActorFrame`（72-401）1:1。
    /// ★ 车道 p7-client-virtual：原文带 `override`；基类同名成员已补 `virtual`，此处由 `new` 改为 `override`。
    /// </summary>
    public override void CalcActorFrame()
    {
        var plan = CustomActorLogic.CalcActorFrame(CalcInput(), out int newAction);

        // 80-91：前置门为真时原文已把 SM_ATTACK01..06 改写为 SM_HIT，随后 inherited
        if (plan == null)
        {
            m_nCurrentAction = newAction;
            base.CalcActorFrame();
            return;
        }

        // 93-98
        m_boUseMagic = plan.UseMagic;                    // 93
        m_nCurrentFrame = -1;                            // 94
        m_nBodyOffset = plan.BodyOffset;                 // 96
        m_nChrLight = (byte)plan.ChrLight;               // 98

        // 各分支写入
        m_nStartFrame = plan.StartFrame;
        m_nEndFrame = plan.EndFrame;
        m_dwFrameTime = plan.FrameTime;
        m_dwStartTime = CustomActorEnv.TimeGetTimeFn();  // 115/129/…
        m_nMaxTick = plan.MaxTick;
        m_nCurTick = plan.CurTick;
        m_nMoveStep = plan.MoveStep;
        if (plan.WritesDefFrameCount)
            m_nDefFrameCount = plan.DefFrameCount;
        if (plan.WritesWarModeTime)
            m_dwWarModeTime = CustomActorEnv.TimeGetTimeFn();   // 217
        if (plan.WritesState)
            m_nState = plan.State;                       // 170
        if (plan.ResetsCustomMagicStruck)
            m_CustomMagicStatusEffect_Struck = 0;        // 243

        // 342-343 / 393-394
        m_nSpellFrame = plan.SpellFrame;
        m_nCurEffFrame = plan.CurEffFrame;
        m_nCurSelfEffFrame = plan.CurSelfEffFrame;

        // 132/153/172/… Shift(...)
        if (plan.DoShift)
            Shift(plan.ShiftDir, plan.ShiftStep, plan.ShiftCur, plan.ShiftMax);

        // 399
        FClientActionIndex = plan.ClientActionIndex;
    }

    /// <summary>
    /// `TCustomActor.LoadSurface`（402-584）1:1；纹理抓取走接缝。
    /// ★ 车道 p7-client-virtual：原文是无参 `override`；此处去掉 `sender` 形参并改用 `override`。
    /// （全仓没有任何带参 `LoadSurface(...)` 调用点，方法体亦未引用 `sender`。）
    /// </summary>
    public override void LoadSurface()
    {
        var plan = CustomActorSurface.Compute(new CustomActorSurfaceInput(
            m_nChangeAppr, m_btRace, m_boDeath, m_boReverseFrame,
            m_nCurrentFrame, m_btDir, m_wAppearance, in FConfig));

        // 409-412：前置门 → inherited
        if (plan == null)
        {
            // 原文 `inherited;` —— ★ 车道 p7-client-actor-family：基类 TActor.LoadSurface(object?)
            // 现已**有本体**（ActorFamilyBase.cs），故这里必须**真发调用**；
            // 不再"只 return"（那会让前置门为真时什么都不发生 —— 台账 §24.2 的镜像形态）。
            base.LoadSurface(null);
            return;
        }

        // 414：画布未就绪 → Compute 已返回 null 之外还需判"非前置门但画布未就绪"
        //     （Compute 对这两种 Exit 都返回 null；此处以画布状态区分）
        if (!CustomActorEnv.GameCanvasActive || !CustomActorEnv.GameCanvasInitialized)
            return;

        // 415-419
        m_dwLoadSurfaceTime = CustomActorEnv.MyGetTickCountFn();
        m_boLoadSurface = false;
        m_BodySurface = null;
        m_BodyEffectSurface = null;
        m_BodyEffect2Surface = null;

        // 421-423：幽灵隐藏 → Finalize（不再装纹理），仍走 582 的 ActionChanged
        if (plan.Hidden)
        {
            OnFinalizeRequested?.Invoke(this);
            ComputeActionChanged();
            return;
        }

        // 548-579：三段取图（真正取图走接缝，此处只传图号与取图种类）
        // ★ 车道 p7-client-actor-family：`m_ColorEffect` 改读继承状态（见下方 ActorColorEffect 覆写）。
        if (plan.LoadBody)
            m_BodySurface = CustomActorEnv.FetchSurfaceFn(
                plan.BodyFileIndex, plan.BodyOffset, m_nPx, m_nPy,
                CustomActorSurface.SurfaceFetchKind(ActorColorEffect, m_boReverseFrame, false));

        if (plan.LoadEffect)
            m_BodyEffectSurface = CustomActorEnv.FetchSurfaceFn(
                plan.EffectFileIndex, plan.EffectOffset, m_nEffectPx, m_nEffectPy,
                CustomActorSurface.SurfaceFetchKind(ActorColorEffect, m_boReverseFrame, false));

        if (plan.LoadEffect2)
            m_BodyEffect2Surface = CustomActorEnv.FetchSurfaceFn(
                plan.EffectFile2Index, plan.Effect2Offset, m_nEffect2Px, m_nEffect2Py,
                CustomActorSurface.SurfaceFetchKind(ActorColorEffect, m_boReverseFrame, true));

        // 582
        ComputeActionChanged();
    }

    /// <summary>LoadSurface 421-423 命中时请求 Finalize 的接缝（原文直接调 Finalize）。</summary>
    public static Action<TCustomActor>? OnFinalizeRequested;

    /// <summary>
    /// `TCustomActor.GetDefaultFrame`（585-639）1:1。
    /// ★ 车道 p7-client-virtual：原文带 `override`；基类同名成员已补 `virtual`，此处由 `new` 改为 `override`。
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        var plan = CustomActorDefaultFrame.Compute(new CustomActorDefaultFrameInput(
            m_nChangeAppr, m_btRace, m_btDir, m_boDeath, m_boSkeleton,
            m_nState, m_nCurrentDefFrame, m_nOldChrLight, in FConfig));

        // 590-593
        if (plan == null)
            return base.GetDefaultFrame(wmode);

        m_nChrLight = (byte)plan.ChrLight;      // 596
        FClientActionIndex = plan.ClientActionIndex;
        return plan.Frame;
    }

    /// <summary>
    /// `TCustomActor.DrawChr`（640-787）1:1；绘制走接缝，顺序已被单测固化。
    /// ★ 车道 p7-client-virtual：原文带 `override`；基类同名成员本轮已补（空虚成员），此处改用 `override`。
    /// </summary>
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
    {
        var seq = CustomActorDraw.Sequence(new CustomActorDrawInput(
            m_nChangeAppr, m_btRace,
            (int)FConfig.BaseConfig.DrawOrder, (int)FConfig.BaseConfig.DrawMode,
            (int)FConfig.BaseConfig.DrawMode2,
            m_BodyEffectSurface != null, m_BodyEffect2Surface != null));

        // 719-722
        if (seq == null)
        {
            // 原文 `inherited DrawChr(dx, dy, blend, boFlag);` —— ★ 车道 p7-client-actor-family：
            // 基类 TActor.DrawChr 现有本体，必须真发调用（理由同上）。
            base.DrawChr(dx, dy, blend, boFlag);
            return;
        }

        foreach (var op in seq)
        {
            switch (op)
            {
                case "self": DrawSelfMagicEffect(dx, dy, true); break;    // 724
                case "-self": DrawSelfMagicEffect(dx, dy, false); break;  // 785
                case "body":
                    // 726/758/783：inherited DrawChr —— ★ 基类现有本体，真发调用。
                    base.DrawChr(dx, dy, blend, boFlag);
                    break;
                case "eff1":
                    CustomActorEnv.DrawSurfaceFn(
                        dx + m_nEffectPx + m_nShiftX, dy + m_nEffectPy + m_nShiftY,
                        m_BodyEffectSurface,
                        CustomActorDraw.SelfEffectDrawKind((int)FConfig.BaseConfig.DrawMode));
                    break;
                case "eff2":
                    CustomActorEnv.DrawSurfaceFn(
                        dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY,
                        m_BodyEffect2Surface,
                        CustomActorDraw.SelfEffectDrawKind((int)FConfig.BaseConfig.DrawMode2));
                    break;
            }
        }
    }

    /// <summary>
    /// 内嵌过程 `DrawSelfMagicEffect`（648-717）1:1。
    /// <paramref name="beforeDraw"/> 为 true 对应 724 的 `DrawSelfMagicEffect(True)`。
    /// </summary>
    private void DrawSelfMagicEffect(int dx, int dy, bool beforeDraw)
    {
        // 651
        if (!CustomActorDraw.SelfMagicEffectGate(m_boUseMagic, m_CurMagicEffectNumber))
            return;

        // 652-661：效果号 → AttackConfigs 槽（非六个攻击号时 ClientConfig 保持 nil，整段不绘）
        int slot = CustomActorDraw.SelfMagicEffectAttackSlot(m_CurMagicEffectNumber);
        if (slot < 0)
            return;

        var cc = FConfig.AttackConfigs[slot];

        // 664-672：自身帧推进（注意 668 比的是 Self_PlayCount、667 比的是 Self_PlayTime）
        var (frame, tick, _) = CustomActorDraw.AdvanceSelfFrameEx(
            m_nCurEffFrame, m_nCurSelfEffFrame, m_dwCurSelfEffFrameTick,
            cc.Self_PlayTime, cc.Self_PlayCount,
            CustomActorEnv.MyGetTickCountFn(), CustomActorEnv.TimeGetTimeFn());
        m_nCurSelfEffFrame = frame;
        m_dwCurSelfEffFrameTick = tick;

        // 675-677：非 8 方向 + 绘制顺序门
        if (!CustomActorDraw.SelfEffectDrawGate(
                (int)cc.Self_DrawOrder, beforeDraw, (int)cc.Self_DirCalcType))
            return;

        // 679-681：帧范围门
        if (!CustomActorDraw.SelfEffectFrameInRange(
                m_nCurSelfEffFrame, cc.Self_PlayCount, cc.Self_StartIndex))
            return;

        // 682-685：图库（Self_File 越界 → 回落 WMonImages.Images[m_wAppearance]）
        int lib = cc.Self_File >= 0 && cc.Self_File < CustomActorEnv.EffectImageListCountFn()
            ? cc.Self_File
            : m_wAppearance;

        // 689-702：图号
        int? idx = CustomActorDraw.SelfEffectImageIndex(
            (int)cc.Self_DirCalcType, cc.Self_StartIndex, cc.Self_PlayCount, cc.Self_EmptyCount,
            m_nCurSelfEffFrame, m_btDir, m_CurMagicTargetX, m_CurMagicTargetY,
            (int)cc.Self_DirCount, m_nCurrX, m_nCurrY);
        if (idx == null)
            return;

        // 690/699：GetCachedImage(idx, px, py) —— px/py 是出参（选图偏移）
        var (px, py) = CustomActorEnv.GetCachedImageOffsetFn(lib, idx.Value);
        if (!CustomActorEnv.SurfaceExistsFn(lib, idx.Value))
            return;                                     // 706：SelfEffectSurface <> nil

        // 707-711
        // 708/710：dx + px + m_nShiftX, dy + py + m_nShiftY
        CustomActorEnv.DrawSurfaceFn(
            dx + px + m_nShiftX,
            dy + py + m_nShiftY,
            null,
            CustomActorDraw.SelfEffectDrawKind((int)cc.Self_DrawMode));
    }

    /// <summary>
    /// `TCustomActor.Run`（788-1038）1:1；特效实例化与入列走接缝。
    /// ★ 车道 p7-client-virtual：原文带 `override`；基类同名成员已补 `virtual`，此处由 `new` 改为 `override`。
    /// </summary>
    public override void Run(uint now)
    {
        var plan = CustomActorRun.Compute(new CustomActorRunInput(
            m_nChangeAppr, m_btRace,
            m_boUseEffect, m_dwEffectFrameTime, m_dwEffectStartTime,
            m_nEffectFrame, m_nEffectEnd,
            m_boUseMagic, m_nCurEffFrame, m_nSpellFrame, m_boCreateEffect,
            m_CurMagicEffectNumber, m_wAppearance, now, in FConfig));

        // 804-807
        if (plan == null)
        {
            base.Run(now);
            return;
        }

        // 809-820：m_boUseEffect 帧推进
        m_boUseEffect = plan.UseEffect;
        if (plan.EffectAdvanced)
        {
            m_dwEffectStartTime = now;                  // 812
            m_nEffectFrame = plan.EffectFrame;          // 814
        }

        // 822：inherited Run（基类帧推进）
        base.Run(now);

        // 829/1034：m_boCreateEffect 落值
        m_boCreateEffect = plan.CreateEffect;

        // 842：ClientConfig = nil → Exit（整个 Run 返回）
        if (plan.EarlyExit)
            return;

        // 接缝：真正 new TCustomMonTargetEffect / TCustomMonFlyEffect 并 AddEffectList
        if (plan.EffectSpawned)
            CustomActorEnv.SpawnEffectFn?.Invoke(this, plan);
    }

    /// <summary>
    /// `TCustomActor.RunSound`（1108-1130）1:1。
    /// ★ 车道 p7-client-virtual：原文带 `override`；基类同名成员本轮已补（空虚成员），此处改用 `override`。
    /// </summary>
    public override void RunSound()
    {
        var r = CustomActorSound.RunSound(CalcInput(), m_nStruckWeaponSound);

        // 1112-1115：前置门 → inherited RunSound
        if (r == null)
        {
            // ★ 车道 p7-client-actor-family：基类 TActor.RunSound 现有本体，必须真发调用
            //（原先"只 return"会让前置门为真时不发任何声）。
            base.RunSound();
            return;
        }

        m_boRunSound = true;                            // 1117
        SetSound_();                                    // 1118

        foreach (var s in r.Value.Sounds)
            CustomActorEnv.PlaySoundFn(s);
    }

    /// <summary>
    /// `TCustomActor.RunActSound`（1039-1107）1:1。
    /// ★ 车道 p7-client-virtual：原文带 `override`；基类同名成员本轮已补（空虚成员），此处改用 `override`。
    /// </summary>
    public override void RunActSound(int frame)
    {
        var r = CustomActorSound.RunActSound(CalcInput(), m_boRunSound, frame);

        // 1043 / 1045-1048：两条 Exit 均退回 inherited RunActSound
        // ★ 车道 p7-client-actor-family：基类 TActor.RunActSound 现有本体，必须真发调用
        //（原先"只 return"会让前置门为真时不发任何声）。
        if (r == null)
        {
            base.RunActSound(frame);
            return;
        }

        if (r.Value.CloseRunSound)
            m_boRunSound = false;                       // 1054/1060/…
        if (r.Value.Sound.Length > 0)
            CustomActorEnv.PlaySoundFn(r.Value.Sound);
    }

    /// <summary>
    /// `m_ColorEffect`（原文 TActor 的单个字段；LoadSurface 550-576 / DrawChr 647-650 的三分支依据）。
    /// <para>★ 车道 p7-client-actor-family：本类**原先自建**了一份同名字段（字段隐藏 CS0108），
    /// 那会造成"同一外观颜色状态两份存储"（台账 §25.2 的静默缺陷形态）。本车道删除了那份重复声明，
    /// 改为**覆写基类虚属性**，直接读写**继承**的那一份。</para>
    /// </summary>
    public override TColorEffect ActorColorEffect
    {
        get => m_BaseColorEffect;
        set => m_BaseColorEffect = value;
    }

    /// <summary>`m_btSex`（原文 TActor 成员；RunActSound 6932-6936/6950 的男女分流）。
    /// 基类实现已读继承字段，此处无需覆写 —— 保留本注释仅作对照说明。</summary>

    /// <summary>1118：SetSound（原文 TActor 成员）。
    /// ★ 车道 p7-client-actor-family：基类已提供 <c>TActor.SetSound()</c>（对怪物族本就是
    /// "什么都不做"的无副作用体，见 ActorFamilyBase.cs 的论证），此处**如实转调基类**
    /// 而不留一个恒空的本地替身（后者会把"基类语义"伪装成"本类占位"）。</summary>
    private void SetSound_() => base.SetSound();
}
