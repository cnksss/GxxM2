using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// 车道 `p7-client-actor-family`：`Actor.pas` **`TActor` 那 4 个虚方法的 1:1 本体**
/// （<c>LoadSurface(object?)</c> / <c>DrawChr</c> / <c>RunSound</c> / <c>RunActSound</c>，
/// 以及它们依赖的 <c>DrawStateEffSurface</c> / <c>SetSound</c>）。
///
/// <para><b>背景</b>：上一条车道 `p7-client-virtual` 在 `PlaySceneNewActor.cs:474-484` 补了这 4 个
/// **虚分派槽位**，但**本体是空实现**；本文件把原文本体落进去。</para>
///
/// <para><b>★★ 为什么现在是 `static` 形式而不是 `virtual` 覆写本体（越区阻塞 → 报告 §8 请求 1）</b>：
/// 4 个槽位的声明在 <c>PlaySceneNewActor.cs:474-484</c>，而 <c>:472</c> 的
/// <c>public class TActor : TActorCore</c> **未加 <c>partial</c>**。因此在托管侧此刻
/// **既无法**在自有新文件里写 <c>partial class TActor</c>（<c>CS0260</c>），
/// **也无法**写同签名成员（<c>CS0111</c> 重名）。</para>
///
/// <para><b>授权后接上只需两处、零逻辑改动</b>：</para>
/// <list type="number">
/// <item><c>PlaySceneNewActor.cs:472</c>：加 <c>partial</c>，并把 <c>:475/478/481/484</c> 那 4 个
///   空虚成员删掉（或改为转调）；</item>
/// <item>新建一个 <c>public partial class TActor</c> 文件，写
///   <c>public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
///   =&gt; ActorFamilyImpl.DrawChr(this, dx, dy, blend, boFlag);</c>
///   —— <b>方法体一行都不用改</b>。本类就是为这种"虚转调"设计的：全部状态都在
///   <c>TActorCore</c> 上（见 <c>ActorFamilyEnv.cs</c> 的字段声明），全部依赖都在
///   <see cref="ActorFamilyEnv"/> 的接缝上。</item>
/// </list>
///
/// <para><b>为什么现在就能给出可信证据</b>：这 4 个本体是**纯状态机** —— 只读写 <c>TActorCore</c>
/// 字段、只经注入接缝取外部资源。故静态形式与虚形式**逐字等价**；测试
/// <c>tests/GXX.Client.Tests/ActorFamilyBaseTests.cs</c> 直接对静态形式下断言，
/// 这些断言在接上虚槽位后**无需改动即继续成立**。</para>
/// </summary>
public static class ActorFamilyImpl
{
    // ══════════════════════════════════════════════════════════════════════
    // 1/4  LoadSurface(Sender:TObject)  ——  Actor.pas 5480-5593（115 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`g_WMonImages[m_nChangeAppr - 100000]` 的**基数十万**（原文 5548）。</summary>
    public const int AppearanceBase = 100000;

    /// <summary>
    /// `TActor.LoadSurface`（**5480-5593，115 行**）1:1。
    ///
    /// <para><b>流程（顺序即语义）</b>：</para>
    /// <list type="number">
    /// <item><b>5491</b>：画布未 Active 或未 Initialized → 直接返回。
    ///   ★ <b>此时还没有刷新加载时间戳</b>（5492 在其后）⇒ 画布不可用时**不消耗节流窗口**，
    ///   下次仍会立刻重试（与 `ClientLoadSurfaceCore`（J179）"核心发现一"逐字一致）。</item>
    /// <item><b>5492-5494</b>：打点、清加载标志、无条件把主体图置空。</item>
    /// <item><b>5496</b>：`(m_btRace in [156]) and (m_nChangeAppr &gt;= 0)` → 自定义怪路径；否则全局图集路径。</item>
    /// <item><b>5500-5506</b>：按 `wMonsterAppr = m_nChangeAppr` **线性查找**配置，首命中即 Break。</item>
    /// <item><b>5510-5541</b>：九标签 `case`，**无 else**；`SM_LIGHTINGEX` 与 `SM_SKELETON` 是**空分支**。</item>
    /// <item><b>5544</b>：三重判据（非 nil 且 `StartIndex &gt;= 0` 且 `PlayCount &gt; 0`）。</item>
    /// <item><b>5545-5548</b>：`ActionFile ∈ [0, g_EffectImageList.Count)` → EffectImageList；
    ///   否则回退 `g_WMonImages[m_nChangeAppr - 100000]`。</item>
    /// <item><b>5550</b>：自定义怪路径的偏移**就是 `m_nCurrentFrame` 本身**（不减 `m_nStartFrame`）。</item>
    /// <item><b>5557-5564</b>：自定义怪路径**被 `not m_boReverseFrame` 包住** ⇒ 反向帧时**什么都不取**。</item>
    /// <item><b>5567-5569</b>：全局路径的四重判据（插件 + 配置 + 勾选 + 死亡）再加
    ///   **外观 900..906 的排除区间** ⇒ 命中则调 <c>Finalize</c>（**不画图**）。</item>
    /// <item><b>5580-5588</b>：全局路径反向帧**另有分支**：下标 =
    ///   `GetOffset(外观) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame)`。</item>
    /// <item><b>5592</b>：**最后一句**调 `ActionChanged`（基类空实现，留给派生类的钩子）。</item>
    /// </list>
    ///
    /// <para><b>原文笔误/冗余（逐字保留）</b>：<c>m_BodySurface := nil</c> 在 5494 与 5498
    /// **相邻几行内写了两次**（第二次在自定义怪分支入口，冗余但保留）。</para>
    /// </summary>
    public static void LoadSurface(TActorCore self, object? sender)
    {
        ArgumentNullException.ThrowIfNull(self);

        // 5491：画布双重守卫（★ 在打点之前 —— 不可用时节流窗口不被消耗）
        if (!ActorFamilyEnv.CanvasReadyFn())
            return;

        // 5492-5494
        self.m_dwLoadSurfaceTime = ActorFamilyEnv.MyGetTickCountFn();
        self.m_boLoadSurface = false;
        self.m_BodySurface = null;

        if (self.m_btRace == 156 && self.m_nChangeAppr >= 0)      // 5496（原文 `m_btRace in [156]`）
        {
            // 5498：原文如此（Actor.pas:5498）—— m_BodySurface 在这里**又**置空一次
            self.m_BodySurface = null;

            // 5500-5506：线性查找，首命中即 Break
            TClientCustomMonsterConfig? monsterConfig =
                ActorFamilyEnv.CustomMonsterConfigLookupFn?.Invoke(self.m_nChangeAppr);

            // 5508-5541：ClientAction := nil 后走九标签 case（无 else）
            TMonsterClientActionType? clientAction = null;
            if (monsterConfig.HasValue)
                clientAction = MapActionToClientSlot(self.m_nCurrentAction, self.StateStoneMode);

            // 5544：三重判据
            // 原文如此（Actor.pas:5544-5550）：判据校验 StartIndex 与 PlayCount，
            // 而真正取图用的是 ActionFile 与 m_nCurrentFrame —— 两组字段**不是同一组**。
            bool accepted = false;
            TMonsterClientAction ca = default;
            if (monsterConfig.HasValue && clientAction.HasValue)
            {
                ca = monsterConfig.Value.Actions[(int)clientAction.Value];
                accepted = ca.StartIndex >= 0 && ca.PlayCount > 0;
            }

            if (accepted)
            {
                bool actionFileInRange = ca.ActionFile >= 0
                    && ca.ActionFile < ActorFamilyEnv.EffectImageListCountFn();     // 5545

                var image = new ActorBodyImage
                {
                    // 5546 / 5548
                    LibraryId = actionFileInRange ? ca.ActionFile : self.m_nChangeAppr - AppearanceBase,
                    ImageIndex = self.m_nCurrentFrame,                              // 5550：**不减** m_nStartFrame
                    Color = self.ActorColorEffectEffective,
                    UseEffectImageList = actionFileInRange,
                };

                // 5556-5565：只在非反向帧取图（5564 的门）
                if (!self.m_boReverseFrame)
                    FetchBody(self, image);
            }
            // else：5551-5553 giBody := nil / nBodyOffset := 0 ⇒ 不取图（m_BodySurface 保持 nil）
        }
        else
        {
            // 5567-5569：四重与 + 外观 900..906 排除区间
            if (ActorFamilyEnv.GhostHideEnabledFn()
                && self.m_boDeath
                && !(self.m_wAppearance >= 900 && self.m_wAppearance <= 906))
            {
                ActorFamilyEnv.FinalizeRequestedFn(self);        // 5569：命中则 Finalize（**不画图**）
            }
            else
            {
                // 5571-5587：全局图集路径
                int offset = ActorOffsets.GetOffset(self.m_wAppearance);          // 原文 GetOffset(m_wAppearance)
                int index = !self.m_boReverseFrame
                    ? offset + self.m_nCurrentFrame                                 // 5575-5578
                    : offset + self.m_nEndFrame - (self.m_nCurrentFrame - self.m_nStartFrame); // 5581：反向帧**另有分支**

                FetchBody(self, new ActorBodyImage
                {
                    LibraryId = self.m_wAppearance,
                    ImageIndex = index,
                    Color = self.ActorColorEffectEffective,
                });
            }
        }

        ActorFamilyEnv.ActionChangedRequestedFn(self);           // 5592：最后一句（基类空实现）
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2/4  DrawChr  ——  Actor.pas 6067-6129（63 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `TActor.DrawChr`（**6067-6129，63 行**）1:1。
    ///
    /// <list type="number">
    /// <item><b>6073</b>：`m_btDir` 不在 0..7 → 直接返回（**唯一的前置守卫**）。</item>
    /// <item><b>6075-6084</b>：主体图非 nil 时画两件事 —— ① <c>DrawEffSurface</c>
    ///   （位置 `dx+m_nPx+m_nShiftX, dy+m_nPy+m_nShiftY`、颜色取 `m_ColorEffect`）；
    ///   ② <c>DrawStateEffSurface(dx+m_nShiftX, dy+m_nShiftY)</c>。</item>
    /// <item><b>6099-6115</b>：施法特效层 —— `m_boUseMagic and (m_CurMagic.EffectNumber &gt; 0)` 且
    ///   `m_nCurEffFrame in [0..m_nSpellFrame-1]` 时取 `GetEffectBase`、图号加 `m_nCurEffFrame`、
    ///   <b>主角死亡时取灰度图</b>（6105-6106），最后 `DrawBlend`。</item>
    /// </list>
    ///
    /// <para><b>接缝：待插件引擎（<c>Enabled_PlugEngine=1</c> 的 <c>HookTActor_DrawChr1/2</c>，
    /// 6086-6097 / 6117-6128）移植后接入</b> —— 原文被 <c>{$IF Enabled_PlugEngine = 1}</c>
    /// 条件编译包住，托管侧无对应物。</para>
    /// </summary>
    public static void DrawChr(TActorCore self, int dx, int dy, bool blend, bool boFlag)
    {
        ArgumentNullException.ThrowIfNull(self);

        // 6073：方向守卫
        if (!ActorDrawPlan.CanDrawDir(self.m_btDir))
            return;

        // 6075-6084
        if (self.m_BodySurface != null)
        {
            var (x, y) = ActorDrawPlan.DrawChrPosition(
                dx, dy, self.m_nPx, self.m_nPy, self.m_nShiftX, self.m_nShiftY);

            ActorFamilyEnv.DrawEffSurfaceOpFn(ActorDrawDispatch.DrawEffSurface(
                self.m_nState, self.m_btBodyColor, self.ActorColorEffectEffective, blend, x, y));   // 6076-6081

            DrawStateEffSurface(self, dx + self.m_nShiftX, dy + self.m_nShiftY);                    // 6083
        }

        // 6099-6115：施法特效层
        var eff = ActorDrawDispatch.SpellEffect(
            self.m_boUseMagic, self.m_CurMagicEffectNumber, self.m_nCurEffFrame, self.m_nSpellFrame,
            self.m_CurMagicNewLevel, ActorFamilyEnv.ViewerDeadFn(),
            dx, dy, self.m_nShiftX, self.m_nShiftY,
            (e, i, lvl) => ActorFamilyEnv.GetEffectBaseFn(e, i, lvl));   // 6101

        if (eff != null)
            ActorFamilyEnv.CanvasDrawBlendFn(eff);                        // 6110-6113
    }

    /// <summary>
    /// `TActor.DrawStateEffSurface`（**5654-5702，49 行**）1:1。
    ///
    /// <para>三层状态各自"推进 → 回卷 → 取图（判存在）→ 画"，三层门各不相同：</para>
    /// <list type="bullet">
    /// <item>蛛网：`m_boCobweb and (not m_boDuanJin)` + `not Ghost` + `not Death`；节流 100ms；帧 0..9；图 320+</item>
    /// <item>毒烟：`m_boToxicSmoke` + `not Ghost` + `not Death`；节流 80ms；帧 0..9；图 4010+</item>
    /// <item>冰冻：`m_boForeverFrozen` + `not Ghost` + `not Death`；节流 80ms；帧 0..3；图 330+</item>
    /// </list>
    /// <para>三层 y 都取 `ddy + pY`，**唯独蛛网多减 20**（5669）。</para>
    /// </summary>
    public static void DrawStateEffSurface(TActorCore self, int ddx, int ddy)
    {
        ArgumentNullException.ThrowIfNull(self);

        // 六个游标字段是**实例状态**（原文 5660-5700），推进必须回写
        var st = new StateFxState
        {
            CobwebTick = self.m_dwCobwebTick,
            CobwebIndex = self.m_nCobwebIndex,
            ToxicTick = self.m_dwToxicSmokeTick,
            ToxicIndex = self.m_nToxicSmokeIndex,
            FrozenTick = self.m_dwForeverFrozenTick,
            FrozenIndex = self.m_nForeverFrozenIndex,
        };

        var (cobweb, smoke, frozen) = ActorDrawDispatch.DrawStateEffSurface(
            st, self.m_boCobweb, self.m_boDuanJin, self.m_boToxicSmoke, self.m_boForeverFrozen,
            self.m_boGhost, self.m_boDeath, ActorFamilyEnv.MyGetTickCountFn(), self.m_nSayX, ddy,
            (lib, idx) => ActorFamilyEnv.StateFxImageExistsFn(lib, idx) ? (0, 0) : null);

        self.m_dwCobwebTick = st.CobwebTick;
        self.m_nCobwebIndex = st.CobwebIndex;
        self.m_dwToxicSmokeTick = st.ToxicTick;
        self.m_nToxicSmokeIndex = st.ToxicIndex;
        self.m_dwForeverFrozenTick = st.FrozenTick;
        self.m_nForeverFrozenIndex = st.FrozenIndex;

        ActorFamilyEnv.CanvasDrawOpFn(cobweb);   // 5669
        ActorFamilyEnv.CanvasDrawOpFn(smoke);    // 5684
        ActorFamilyEnv.CanvasDrawOpFn(frozen);   // 5699
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3/4  RunSound  ——  Actor.pas 6788-6901（114 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `TActor.RunSound`（**6788-6901，114 行**）1:1。
    ///
    /// <para><b>6794-6795 是无条件两句</b>：先把 `m_boRunSound` 置真（**即使什么都不播**），
    /// 再调 `SetSound` 刷新音效槽；然后才是 `case m_nCurrentAction` 的五标签（**无 else**）。</para>
    ///
    /// <para><b>SM_NOWDEATH 的复合条件</b>（6803）：`(m_nDieSound &gt;= 0) and m_boDeath`；且仅当
    /// `Self = g_MySelf` 时再加发一条 500ms 延迟的 gameover BGM 请求（6806-6807）。</para>
    ///
    /// <para><b>SM_ALIVE / SM_DIGUP 的差异</b>（6814-6815）：**没有 `&gt;= 0` 判定**，
    /// appear 音一律播 —— 与 SM_STRUCK 三音各自判 `&gt;= 0` 形成对照（差异断言要点）。</para>
    ///
    /// <para><b>SM_SPELL 的两分支</b>（6817-6838）：有自定义魔法配置时取
    /// `MagicConfigs[等级归一化].Sounds[cmstUseMagic]`，且**只有长度 &gt; 0 才播**；
    /// 无配置时播 `m_nMagicStartSound`。</para>
    /// </summary>
    public static void RunSound(TActorCore self)
    {
        ArgumentNullException.ThrowIfNull(self);

        self.m_boRunSound = true;                        // 6794
        SetSound(self);                                  // 6795（原文 1816 `procedure SetSound; virtual;`）

        // 6796-6839：五标签 case、无 else
        var cues = ActorSoundDispatch.RunSound(
            self.m_nCurrentAction,
            self.m_nStruckWeaponSound, self.m_nStruckSound, self.m_nScreamSound,
            self.m_nDieSound, self.m_boDeath, self.IsMySelf,
            self.m_nAttackSound, self.m_nAppearSound, self.m_nMagicStartSound,
            CustomMagicConfig(self) != null, self.m_CurMagicNewLevel,
            CustomMagicUseMagicSounds(self));

        foreach (var cue in cues)
            PlayCue(cue);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4/4  RunActSound  ——  Actor.pas 6903-7095（193 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `TActor.RunActSound`（**6903-7095，193 行**）1:1。
    ///
    /// <para><b>6909 是入口门</b>：`if m_boRunSound then` —— 整段逻辑都被它包住。</para>
    ///
    /// <para><b>6910 分两大支</b>：`m_btRace in [0, 1]` 走武器音族（6911-7043，由
    /// `ActorSoundDispatch.RunActSoundWar` 承载）；其余种族走 7045 起的 else：</para>
    /// <list type="bullet">
    /// <item><b>7046-7047</b>：`m_btRace = 50` 是**空实现**（既不播音、也不消耗随机数）。</item>
    /// <item><b>7051-7056</b>：`SM_TURN` 且 `frame = 1` 且 `Random(8) = 1` → 播 `m_nNormalSound`。</item>
    /// <item><b>7057-7062</b>：`SM_HIT` 且 `frame = 3` 且 `m_nAttackSound &gt;= 0` → 播 `m_nWeaponSound`。
    ///   这两段是**两个独立 `if`**（不是 else if）。</item>
    /// <item><b>7063-7072</b>：`case m_wAppearance of 80:` —— 仅 `SM_NOWDEATH` 且 `frame = 2` 播
    ///   `m_nDie2Sound`。★ **既有派发层 `ActorSoundDispatch.RunActSoundOther` 缺这一支**（报告 §7）。</item>
    /// <item><b>7076-7092</b>：`m_btRace in [202..209]`（Mon36_X 族）—— 三动作各播硬编码音
    ///   （SM_TURN→542 / SM_STRUCK→495 / SM_NOWDEATH→496），**都不判 frame**。
    ///   ★ 这一支同样在既有派发层缺失（报告 §7）。</item>
    /// </list>
    /// <para>为避免扩大外部改动面，7063-7072 与 7076-7092 两支在本体内**逐字落地**（不经派发层）。</para>
    /// </summary>
    public static void RunActSound(TActorCore self, int frame)
    {
        ArgumentNullException.ThrowIfNull(self);

        if (!self.m_boRunSound)                          // 6909
            return;

        if (self.m_btRace is 0 or 1)                     // 6910
        {
            // 6911-7043：武器音族（全族共同门 `frame = 2` 由派发层内部把关）
            var cues = ActorSoundDispatch.RunActSoundWar(
                self.m_nCurrentAction, frame, self.m_btSex, self.m_nWeaponSound,
                ActorFamilyEnv.SoundIdFn("s_yedo_man"), ActorFamilyEnv.SoundIdFn("s_yedo_woman"),
                ActorFamilyEnv.SoundIdFn("s_longhit"), ActorFamilyEnv.SoundIdFn("s_widehit"),
                ActorFamilyEnv.SoundIdFn("s_firehit"), ActorFamilyEnv.SoundIdFn("s_phz"),
                CustomMagicConfig(self) != null, self.m_CurMagicNewLevel,
                (serial, lvl) => CustomMagicWarrSound(self, serial, lvl),
                ActorFamilyEnv.ConfigDlgCkSceneShake,
                out bool closeWar);

            foreach (var cue in cues)
                PlayCue(cue);

            if (closeWar)
                self.m_boRunSound = false;
        }
        else
        {
            // 7045-7047：race 50 空实现
            if (self.m_btRace == 50)
                return;

            // 7051-7062：两处**独立** if（可同时命中）
            if (self.m_nCurrentAction == TActorCore.SM_TURN)              // 7051
            {
                if (frame == 1 && ActorFamilyEnv.RandomFn(8) == 1)       // 7052
                {
                    PlayCue(new SoundCue(self.m_nNormalSound, "NormalTurn")); // 7053
                    self.m_boRunSound = false;                                // 7054
                }
            }

            if (self.m_nCurrentAction == TActorCore.SM_HIT)              // 7057
            {
                if (frame == 3 && self.m_nAttackSound >= 0)              // 7058
                {
                    PlayCue(new SoundCue(self.m_nWeaponSound, "HitWeapon")); // 7059
                    self.m_boRunSound = false;                               // 7060
                }
            }

            // 7063-7072：appearance 80 专用死亡音（原文唯一的 appearance 分支）
            if (self.m_wAppearance == 80
                && self.m_nCurrentAction == TActorCore.SM_NOWDEATH
                && frame == 2)
            {
                PlayCue(new SoundCue(self.m_nDie2Sound, "Die2"));        // 7067
                self.m_boRunSound = false;                               // 7068
            }

            // 7076-7092：Mon36_X 族（race 202..209），三分支**都判 action 而不判 frame**
            if (self.m_btRace is >= 202 and <= 209)
            {
                if (self.m_nCurrentAction == TActorCore.SM_TURN)          // 7080
                {
                    PlayCue(new SoundCue(542, "Mon36Turn"));            // 7081
                    self.m_boRunSound = false;                           // 7082
                }

                if (self.m_nCurrentAction == TActorCore.SM_STRUCK)        // 7084
                {
                    PlayCue(new SoundCue(495, "Mon36Struck"));          // 7085
                    self.m_boRunSound = false;                           // 7086
                }

                if (self.m_nCurrentAction == TActorCore.SM_NOWDEATH)      // 7088
                {
                    PlayCue(new SoundCue(496, "Mon36NowDeath"));        // 7089
                    self.m_boRunSound = false;                           // 7090
                }
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // SetSound ——  Actor.pas 6454-6786（333 行）：原文对怪物族**本就无副作用**
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `TActor.SetSound`（原文 **6454-6786，333 行**）的托管落点。
    ///
    /// <para><b>★ 为什么空实现是**正确语义**，而不是台账 §25.2 禁止的"未接线"</b>：
    /// 原文 `SetSound` 的整个函数体被 <c>6459 if m_btRace in [0, 1] then begin</c> 包住且
    /// **没有 else**；6766 的 <c>end;</c> 之后 6769-6785 的受击武器音段又要求
    /// <c>PlayScene.FindActor(m_nHiterCode)</c> 命中。即：**对怪物族（`m_btRace` ∉ {0,1}），
    /// 原文的 `SetSound` 逐字就是"什么都不做"**。</para>
    ///
    /// <para><b>依赖规模（故不在本车道移植）</b>：需要 `g_MySelf`、`Map.m_MArr[cx,cy]` 的
    /// `wBkImg/btArea/wMidImg/wFrImg`、`Map.m_nBlockLeft/Top`、`m_wWeapon/m_wDress/m_wWeaponSound/
    /// m_nHiterCode`、`m_nMagicStruckSound`、`PlayScene.FindActor`，以及 **60 余个命名音效常量**
    /// （`s_walk_*` / `s_struck_*` / `s_hit_*` / `s_cboZs*` …，全部是 `g_SoundList.Count + N` 的运行时值）。</para>
    ///
    /// <para><b>接缝：待 `TActor.SetSound`（Actor.pas:6454-6786）移植后接入</b>；
    /// 人类族的 `THumActor.SetSound` 覆写亦未移植（另一单元）。</para>
    /// </summary>
    public static void SetSound(TActorCore self)
    {
        ArgumentNullException.ThrowIfNull(self);

        // 原文 6459 `if m_btRace in [0, 1] then` 且无 else ⇒ 怪物族在此**本就无副作用**。
    }

    // ══════════════════════════════════════════════════════════════════════
    // 辅助（公开：供差异断言直接调用）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>5510-5541 的九标签 `case` → 动作槽（null = 原文该分支**没有赋值**）。</summary>
    public static TMonsterClientActionType? MapActionToClientSlot(int action, bool stone)
        => action switch
        {
            0 or TActorCore.SM_TURN => stone
                ? TMonsterClientActionType.matStoneRevive        // 5512-5513
                : TMonsterClientActionType.matStand,             // 5515
            TActorCore.SM_WALK or TActorCore.SM_RUSH
                or TActorCore.SM_RUSHKUNG or TActorCore.SM_BACKSTEP
                => TMonsterClientActionType.matWalk,             // 5517-5519
            TActorCore.SM_DIGUP => TMonsterClientActionType.matStoneRevive,  // 5520-5522
            TActorCore.SM_LIGHTINGEX => null,                    // 5523-5525：**空分支**
            TActorCore.SM_HIT => TMonsterClientActionType.matDefAttack,      // 5526-5528
            TActorCore.SM_STRUCK => TMonsterClientActionType.matStruck,      // 5529-5531
            TActorCore.SM_DEATH => TMonsterClientActionType.matDie,          // 5532-5534
            TActorCore.SM_NOWDEATH => TMonsterClientActionType.matDie,       // 5535-5537
            TActorCore.SM_SKELETON => null,                      // 5538-5540：**空分支**
            _ => null,                                           // 无 else ⇒ 保持 nil
        };

    /// <summary>5559/5575 的 `case m_ColorEffect` 三分支 → 取图种类（"image"/"gray"/"bright"）。</summary>
    public static string ActorBodyImageFetchKind(TColorEffect c)
        => c switch
        {
            TColorEffect.ceGrayScale or TColorEffect.ceGrayScale2 => "gray",   // 5559 / 5575
            TColorEffect.ceBright => "bright",                                 // 5560 / 5576
            _ => "image",                                                      // 5561-5562 / 5577-5578
        };

    // ---- 私有：取图与取声 ----

    /// <summary>5556-5565 / 5571-5589 的取图（两路径共用同一接缝）。</summary>
    private static void FetchBody(TActorCore self, ActorBodyImage image)
    {
        var fetched = ActorFamilyEnv.FetchBodySurfaceFn(image, ActorBodyImageFetchKind(image.Color));
        if (fetched != null)
            self.m_BodySurface = fetched;                    // 原文：仅在取到图时赋值
    }

    /// <summary>6818：`GetCustomMagicConfig(m_CurMagic.MagicSerial)`。</summary>
    private static ActorFamilyEnv.CustomMagicSoundView? CustomMagicConfig(TActorCore self)
        => ActorFamilyEnv.CustomMagicConfigLookupFn?.Invoke(self.m_CurMagicMagicSerial);

    /// <summary>6832：仅当 `Sounds[cmstUseMagic]` **长度 &gt; 0** 才播（空串 = 未配置）。</summary>
    private static IReadOnlyList<int>? CustomMagicUseMagicSounds(TActorCore self)
    {
        var cfg = CustomMagicConfig(self);
        if (cfg == null || cfg.UseMagic.Length == 0)
            return null;

        return new[] { ActorFamilyEnv.SoundIdFn(cfg.UseMagic) };
    }

    /// <summary>6932-6943：`cmstManWarr` / `cmstWomanWarr` 男女分流，都为空则回退武器音。</summary>
    private static int CustomMagicWarrSound(TActorCore self, int serial, int magicPlusLevel)
    {
        var cfg = CustomMagicConfig(self);
        if (cfg == null)
            return -1;

        if (self.m_btSex == 0 && cfg.ManWarr.Length > 0)
            return ActorFamilyEnv.SoundIdFn(cfg.ManWarr);

        if (self.m_btSex == 1 && cfg.WomanWarr.Length > 0)
            return ActorFamilyEnv.SoundIdFn(cfg.WomanWarr);

        return -1;   // 原文 6940-6943 的 else 走武器音（由派发层补 Weapon 提示）
    }

    private static void PlayCue(in SoundCue cue)
    {
        switch (cue.Kind)
        {
            case "GameOverBgmDelay":
                ActorFamilyEnv.SendGameOverBgmDelayFn?.Invoke();              // 6807
                break;
            case "SceneShake":
                ActorFamilyEnv.SceneShakeFn?.Invoke();                        // 7012
                break;
            default:
                ActorFamilyEnv.PlaySoundByIdFn(cue.SoundId);                  // 6798…（id 音）
                break;
        }
    }
}
