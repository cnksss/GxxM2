using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// 车道 `p7-client-actor-family`：`TActor`（<c>Actor.pas</c>）那 4 个基类本体
/// （<c>LoadSurface</c> / <c>DrawChr</c> / <c>RunSound</c> / <c>RunActSound</c>）落地所需的
/// **运行时环境接缝**与**缺失字段**。
///
/// <para><b>设计原则（台账 §25.2「接缝不得静默返回中性值」）</b>：本文件里的接缝**没有一个是**
/// "静默返回中性值"的伪装层 —— 每个接缝要么返回**结构化结果**
/// （<see cref="ActorBodyImage"/> 规划 / <c>SurfaceDrawOp</c> 绘制操作 / <c>SoundCue</c> 声音队列），
/// 要么带**存在性标志**（<see cref="GetEffectBaseFn"/> 的 <c>null</c>、<see cref="CustomMagicConfigLookupFn"/>
/// 的 <c>null</c>）—— 且每一个 <c>null</c>/默认值都在注释里**指明它对应原文的哪一个分支**，
/// 使"未接线"与"分支没命中"在类型层面就可区分。</para>
/// </summary>
public static partial class ActorFamilyEnv
{
    // ══════════════════════════════════════════════════════════════════════
    // 一、时钟与画布（LoadSurface 5491 / 5492 的两处前置守卫）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `GameCanvas.Active and GameCanvas.Initialized`（原文 5491 的双重前置守卫）。
    /// <para>原文用 `(not Active) or (not Initialized) then Exit` 表达，即**两者都为真**才继续。</para>
    /// </summary>
    public static Func<bool> CanvasReadyFn = () => false;

    /// <summary>`MyGetTickCount`（原文 5492 刷新 `m_dwLoadSurfaceTime`）。</summary>
    public static Func<uint> MyGetTickCountFn = () => SceneTime.TickNow();

    // ══════════════════════════════════════════════════════════════════════
    // 二、LoadSurface 的三处依赖（自定义怪配置 / 图集偏移 / 取图）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `g_CustomMonsterConfig` 的线性查找（原文 5500-5506：按 `wMonsterAppr = m_nChangeAppr`
    /// **首命中即 Break**）。
    /// <para><c>null</c> = 未找到配置（原文 `MonsterConfig = nil`）⇒ 原文**跳过整个 case**、
    /// `ClientAction` 保持 nil ⇒ 落到 5551 的 else。**此 null 是原文语义**，不是"未接线"。</para>
    /// </summary>
    public static Func<int, TClientCustomMonsterConfig?>? CustomMonsterConfigLookupFn;

    /// <summary>
    /// `g_EffectImageList.Count`（原文 5545 的 `ActionFile &lt; g_EffectImageList.Count`）。
    /// <para>默认 0 ⇒ 任何 `ActionFile &gt;= 0` 都判越界、回退 `g_WMonImages[m_nChangeAppr - 100000]`
    /// —— 即原文 5548 的**回退分支**，与"未接线"无关。</para>
    /// </summary>
    public static Func<int> EffectImageListCountFn = () => 0;

    /// <summary>
    /// 取图接缝（原文 5546/5548/5559-5562/5571-5587 的
    /// <c>GetCachedImage</c> / <c>GetCachedGrayImage</c> / <c>GetCachedBrightImage</c>）。
    /// <para><c>kind</c> ∈ <c>"image" | "gray" | "bright"</c>。返回 <c>null</c> = 取图失败
    /// （原文 `m_BodySurface` 保持 nil ⇒ DrawChr 的 `Assigned(m_BodySurface)` 门为假 ⇒ 不画主体）。</para>
    /// </summary>
    public static Func<ActorBodyImage, string, object?> FetchBodySurfaceFn = (_, _) => null;
    /// <summary>
    /// `PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]`
    /// （原文 5567 的三重与；第 4/5 项 `m_boDeath` 与外观区间**由本体自己判**）。
    /// </summary>
    public static Func<bool> GhostHideEnabledFn = () => false;

    /// <summary>
    /// 5569 的 <c>Finalize</c> 请求。
    /// <para><b>接缝：待 `TActor.Finalize`（Actor.pas:1848 声明、实现另在）移植后接入</b>
    /// —— 托管侧基类**没有** <c>Finalize</c> 成员，故此处是**显式请求**而非空调用。</para>
    /// </summary>
    public static Action<TActorCore> FinalizeRequestedFn = _ => { };

    /// <summary>
    /// 5592 的 <c>ActionChanged</c> 请求。
    /// <para><b>接缝：待 `TActor.ActionChanged`（Actor.pas:7101-7104，**函数体为空**）接入</b>
    /// —— 托管侧基类只有 <c>ComputeActionChanged</c>（ActorMotion.cs:304，同样是空实现）。</para>
    /// </summary>
    public static Action<TActorCore> ActionChangedRequestedFn = _ => { };

    // ══════════════════════════════════════════════════════════════════════
    // 三、DrawChr 的绘制与施法特效
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `GameCanvas.DrawBlend(x, y, d)`（原文 6110-6113 的施法特效层）。
    /// <para>只接收**已规划好的** <c>SurfaceDrawOp</c>；与原文"`d = nil` 则不调 DrawBlend"
    /// （6109 的存在性门）在结构上等价。</para>
    /// </summary>
    public static Action<SurfaceDrawOp> CanvasDrawBlendFn = _ => { };

    /// <summary>
    /// `DrawStateEffSurface` 的三层状态绘制出口（原文 5669/5684/5699 的三处
    /// `GameCanvas.DrawBlend(...)`）。
    /// <para>形参是 <c>SurfaceDrawOp?</c> —— 与 `ActorDrawDispatch.DrawStateEffSurface` 的返回形状
    /// 一致（<c>null</c> 表示该层**没画**，对应原文 `d = nil` 的存在性门）。</para>
    /// </summary>
    public static Action<SurfaceDrawOp?> CanvasDrawOpFn = _ => { };

    /// <summary>`DrawEffSurface(m_BodySurface, x, y, blend, m_ColorEffect)`（原文 6076-6081）。</summary>
    public static Action<SurfaceDrawOp> DrawEffSurfaceOpFn = _ => { };

    /// <summary>`g_WNewopUIImages.GetCachedImage(idx)` / `g_cboEffect.GetCachedImage(idx)` 的存在性接缝。</summary>
    public static Func<string, int, bool> StateFxImageExistsFn = (_, _) => false;

    /// <summary>
    /// `GetEffectBase(effectNumber - 1, 0, wimg, idx, m_CurMagic.NewLevel)`（原文 6101）。
    /// <para><c>null</c> = 取不到特效图集（原文 `wimg = nil`）⇒ 6104 的门为假、不取图也不画
    /// —— 有明确的原文对应分支。</para>
    /// </summary>
    public static Func<int, int, int, ActorDrawDispatch.EffectBaseRef?> GetEffectBaseFn = (_, _, _) => null;

    // ══════════════════════════════════════════════════════════════════════
    // 四、RunSound / RunActSound 的播音与随机
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`g_PlaySound.PlaySound(id)`（原文 6798-6815 / 6914 / 6941-7018 / 7053-7060 等）。</summary>
    public static Action<int> PlaySoundByIdFn = _ => { };

    /// <summary>`PlaySound(FileName)`（原文 6804 / 6833 / 6933 / 6937 / 6986 / 6992-6993 的**命名音**）。</summary>
    public static Action<string> PlaySoundByNameFn = _ => { };

    /// <summary>`frmMain.SendDelayMsg(g_MySelf, RCM_PlayBGMgameover, …, 500)`（原文 6807）。</summary>
    public static Action? SendGameOverBgmDelayFn;

    /// <summary>`PlayScene.SceneShake()`（原文 7012，仅 `SM_102HIT` 且勾选场景震动）。</summary>
    public static Action? SceneShakeFn;

    /// <summary>`Random(8)`（原文 7052）。默认 <c>_ =&gt; 0</c>，与原工程其它随机接缝同形。</summary>
    public static Func<int, int> RandomFn = _ => 0;

    /// <summary>`g_ConfigDlg.ConfigCheckeds[ckSceneShake]`（原文 7011）。</summary>
    public static bool ConfigDlgCkSceneShake;

    /// <summary>`g_MySelf.m_boDeath`（原文 6105：施法层按**主角**是否死亡决定取灰度图）。</summary>
    public static Func<bool> ViewerDeadFn = () => false;

    /// <summary>
    /// `g_SoundList` 的命名音常量（`s_yedo_man` / `s_longhit` / … 全是 `g_SoundList.Count + N` 的
    /// **运行时值**，不是编译期常量）。返回 -1 = 未接线（与原工程其它命名音接缝同形）。
    /// </summary>
    public static Func<string, int> SoundIdFn = _ => -1;

    /// <summary>
    /// `GetCustomMagicConfig(serial)`（原文 6818 / 6919）。
    /// <para><c>null</c> = 无配置（原文 `CustomMagicConfig = nil` 走 else 分支，属**原文语义**）。</para>
    /// </summary>
    public static Func<int, CustomMagicSoundView?>? CustomMagicConfigLookupFn;

    /// <summary>
    /// `CustomMagicConfig.MagicConfigs[MagicPlusLevel]` 的**只读视图**：
    /// `Sounds[cmstUseMagic]` / `[cmstManWarr]` / `[cmstWomanWarr]` 三条命名音。
    /// <para>原文用 `Length(...) &gt; 0` 判"是否配置"（6832 / 6932 / 6936），故此处用**串**表达；
    /// 空串 = 该条未配置 —— 与 <c>null</c> 的"整块配置不存在"是**两个不同层级**的判据。</para>
    /// </summary>
    public sealed class CustomMagicSoundView
    {
        /// <summary>`Sounds[cmstUseMagic]`（6832）</summary>
        public string UseMagic = "";
        /// <summary>`Sounds[cmstManWarr]`（6932）</summary>
        public string ManWarr = "";
        /// <summary>`Sounds[cmstWomanWarr]`（6936）</summary>
        public string WomanWarr = "";
    }

    // ══════════════════════════════════════════════════════════════════════
    // 复位（台账 §29.4 第 1 条：复位前后必须能数清）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>把本类全部接缝复位为**构造期默认值**（测试用；不含各实例的字段）。</summary>
    public static void Reset()
    {
        CanvasReadyFn = () => false;
        MyGetTickCountFn = () => SceneTime.TickNow();
        CustomMonsterConfigLookupFn = null;
        EffectImageListCountFn = () => 0;
        FetchBodySurfaceFn = (_, _) => null;
        GhostHideEnabledFn = () => false;
        FinalizeRequestedFn = _ => { };
        ActionChangedRequestedFn = _ => { };
        CanvasDrawBlendFn = _ => { };
        CanvasDrawOpFn = _ => { };
        DrawEffSurfaceOpFn = _ => { };
        StateFxImageExistsFn = (_, _) => false;
        GetEffectBaseFn = (_, _, _) => null;
        PlaySoundByIdFn = _ => { };
        PlaySoundByNameFn = _ => { };
        SendGameOverBgmDelayFn = null;
        SceneShakeFn = null;
        RandomFn = _ => 0;
        ConfigDlgCkSceneShake = false;
        ViewerDeadFn = () => false;
        SoundIdFn = _ => -1;
        CustomMagicConfigLookupFn = null;
    }
}

/// <summary>
/// `Actor.pas` 里 `TActor` 本体用到、而托管侧 <c>TActorCore</c> 尚未持有的字段
/// （**新增**，属车道 `p7-client-actor-family` 的 <c>ActorFamily*</c> 自有文件）。
///
/// <para><b>为什么用 partial 而不是改 <c>ActorCore.cs</c></b>：本工程已确立"消除两个写者"的手法 ——
/// 在自己车道的独立文件里以 <c>partial</c> 扩展，**零改既有文件**。
/// <c>TActorCore</c> 自 <c>ActorCore.cs:11</c> 起就是 <c>partial</c>，故本扩展合法。</para>
///
/// <para><b>一处有意避让的字段名</b>：<c>TCustomActor</c>（<c>CustomActor.cs:1628</c>）**已经有**
/// 它自己的 <c>m_ColorEffect</c>。若此处也叫 <c>m_ColorEffect</c>，会形成字段隐藏（CS0108），
/// 并使 <c>TCustomActor</c> 的方法体**静默读到基类那一份**（两份状态不同步 = 静默缺陷）。
/// 故基类那一份命名为 <see cref="m_BaseColorEffect"/>，语义 1:1 对应原文 <c>m_ColorEffect</c>。</para>
/// </summary>
public partial class TActorCore
{
    // ---- LoadSurface 本体（5491-5593）直接读写 ----

    /// <summary>`m_boLoadSurface`（原文 5494 清；`CheckLoadSurface` 7355-7360 读）。</summary>
    public bool m_boLoadSurface;

    /// <summary>`m_dwLoadSurfaceTime`（原文 5492 打点；`CheckLoadSurface` 的 2 秒节流基准）。</summary>
    public uint m_dwLoadSurfaceTime;

    /// <summary>
    /// `m_BodySurface`（5480-5593 的主体纹理槽）。
    /// <para>以 <c>object?</c> 承载（headless 无 TTexture）。**取到的对象原样存放、不做解释** ——
    /// 以免把"取到图但类型不对"伪装成"没取到图"。</para>
    /// </summary>
    public object? m_BodySurface;

    /// <summary>基类那一份 `m_ColorEffect`（原文 5558/5574/5582 的三分支依据）。见类注释的避让说明。</summary>
    public TColorEffect m_BaseColorEffect;

    /// <summary>
    /// `m_ColorEffect` 的**非虚读取**（供“虚转调层”与静态实现共用）。
    /// <para><b>接缝：待 <c>TCustomActor</c> 把它的 <c>m_ColorEffect</c> 接到此处（报告 §8 请求 3）。</b></para>
    /// </summary>
    public TColorEffect ActorColorEffectEffective => m_BaseColorEffect;

    /// <summary>`m_nState` 的 `STATE_STONE_MODE` 位（原文 5512 的石化门）。</summary>
    public bool StateStoneMode => (m_nState & (int)ActorStates.STATE_STONE_MODE) != 0;

    // ---- DrawChr / DrawStateEffSurface（6067-6129 / 5654-5702）----
    // 注意：`m_boGhost` 已在 PlaySceneActors.cs:123、`m_boDeath` 在 ActorMessages.cs:38、
    //       `m_boReverseFrame` 在 ActorMotion.cs:68 —— 此处**均不重复声明**。

    /// <summary>`m_boCobweb`（蜘蛛网罩住，原文 5659）。</summary>
    public bool m_boCobweb;

    /// <summary>`m_boDuanJin`（断筋，原文 5659：有它则**不**画蛛网）。</summary>
    public bool m_boDuanJin;

    /// <summary>`m_boToxicSmoke`（毒烟，原文 5674）。</summary>
    public bool m_boToxicSmoke;

    /// <summary>`m_boForeverFrozen`（永恒冰冻，原文 5689）。</summary>
    public bool m_boForeverFrozen;

    /// <summary>`m_nEffectFrame` / `m_nEffectStart` / `m_nEffectEnd` / `m_dwEffectStartTime` /
    /// `m_dwEffectFrameTime` —— 施法层与 `TCentipedeKingMon.Run`（1244-1284）**共用**的动画游标。</summary>
    public int m_nEffectFrame;
    public int m_nEffectStart;
    public int m_nEffectEnd;
    public uint m_dwEffectStartTime;
    public uint m_dwEffectFrameTime;

    // ---- 施法层（6099-6115）----
    // 注意：`m_CurMagicEffectNumber` 已在 ActorMotion.cs:58 声明 —— 不重复声明。

    /// <summary>`m_CurMagic.MagicSerial`（原文 6672/6818/6919 的自定义魔法序号）。</summary>
    public int m_CurMagicMagicSerial;
    /// <summary>`m_CurMagic.NewLevel`（原文 6821-6828 / 6921-6928 的等级归一化输入）。</summary>
    public int m_CurMagicNewLevel;
    /// <summary>`m_nCurEffFrame`（原文 6100 的施法帧游标）。</summary>
    public int m_nCurEffFrame;
    /// <summary>`m_nSpellFrame`（原文 6100 的施法帧总数）。</summary>
    public int m_nSpellFrame;

    // ---- RunSound / RunActSound（6788-7095）----
    // 注意：`m_btSex` 已在 PlaySceneActors.cs:139 声明 —— 此处**不重复声明**。

    /// <summary>`m_boRunSound`（原文 1561；6794 置真、6915-7090 各分支置假）。</summary>
    public bool m_boRunSound;

    /// <summary>`SetSound` 计算出的音效槽族（原文 1561 附近）；初值 −1 与
    /// `ActorSoundDispatch` 的 `&gt;= 0` 门约定一致。</summary>
    public int m_nStruckWeaponSound = -1;
    public int m_nStruckSound = -1;
    public int m_nScreamSound = -1;
    public int m_nDieSound = -1;
    public int m_nDie2Sound = -1;
    public int m_nNormalSound = -1;
    public int m_nAttackSound = -1;
    public int m_nWeaponSound = -1;
    public int m_nAppearSound = -1;
    public int m_nMagicStartSound = -1;

    // ---- 三层状态特效的六个游标字段（原文 5660-5700）----

    /// <summary>`m_dwCobwebTick`（5660）</summary>
    public uint m_dwCobwebTick;
    /// <summary>`m_nCobwebIndex`（5662/5664-5665，0..9 回卷）</summary>
    public int m_nCobwebIndex;
    /// <summary>`m_dwToxicSmokeTick`（5675）</summary>
    public uint m_dwToxicSmokeTick;
    /// <summary>`m_nToxicSmokeIndex`（5677/5679-5680，0..9 回卷）</summary>
    public int m_nToxicSmokeIndex;
    /// <summary>`m_dwForeverFrozenTick`（5690）</summary>
    public uint m_dwForeverFrozenTick;
    /// <summary>`m_nForeverFrozenIndex`（5692/5694-5695，0..3 回卷）</summary>
    public int m_nForeverFrozenIndex;
}
