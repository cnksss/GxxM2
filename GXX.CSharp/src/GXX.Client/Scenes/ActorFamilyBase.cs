using System;

namespace GXX.Client.Scenes;

/// <summary>
/// 车道 `p7-client-actor-family`：**`TActor` 那 4 个虚方法的本体**（`Actor.pas` 的
/// `LoadSurface` 5480-5593 / `DrawChr` 6067-6129 / `RunSound` 6788-6901 / `RunActSound` 6903-7095），
/// 以及同族依赖的 `DrawStateEffSurface`（5654-5702）。
///
/// <para><b>与 `PlaySceneNewActor.cs` 的关系</b>：<c>TActor</c> 是**同一个类的两半** ——
/// 那一半（<c>PlaySceneNewActor.cs:472</c>，已加 <c>partial</c>）只留类头与 <c>ActorClass</c> 系列；
/// 本文件承载原文本体。这与 <c>TCreature</c>/<c>TPlayObject</c> 的组织形态一致，
/// 也是本工程消除"两个写者"风险的既定手法。</para>
///
/// <para><b>★ `LoadSurface` 的两个槽位（原文签名 vs 派生槽位）</b></para>
/// <list type="bullet">
/// <item><see cref="LoadSurface(object?)"/> —— **原文签名**（`procedure LoadSurface(Sender:TObject); virtual;`，
///   `Actor.pas:1842`）。<c>HerbActor.pas</c> 族全部覆写的是**这一个**（如 `TCastleDoor.LoadSurface(Sender)` 562、
///   `TWallStructure.LoadSurface(Sender)` 832）。它的方法体是 1:1 原文（含 <c>5491-5593</c> 全文）。</item>
/// <item><see cref="LoadSurface()"/> —— `p7-client-virtual` 落的**无参派生槽位**
///   （`PlaySceneNewActor.cs:475` 原为 <c>public virtual void LoadSurface() { }</c>）。
///   `CustomActor.pas` 族的 `LoadSurface` **无参**，覆写的是这一个。
///   <para><b>裁定（父 agent 批准）</b>：保留两个槽位，**无参槽位转调带参重载**（传 <c>null</c>）。
///   原文只用 <c>5491-5593</c> 的几何/图集逻辑，**全文不引用 <c>Sender</c>** ——
///   故 <c>null</c> 与任何实参等价（`CustomActor.cs` 既有注释亦已核对过这一点）。</para></item>
/// </list>
///
/// <para><b>接缝</b>：全部外部依赖在 <see cref="ActorFamilyEnv"/>；本文件不含任何"静默返回中性值"
/// 的伪装层（台账 §25.2）。</para>
/// </summary>
public partial class TActor
{
    // ══════════════════════════════════════════════════════════════════════
    // 1/5  LoadSurface  ——  Actor.pas 5480-5593（115 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// **原文签名**：`procedure TActor.LoadSurface(Sender:TObject);`（`Actor.pas:5480-5593`，115 行）1:1。
    ///
    /// <para><b>流程（顺序即语义）</b>：</para>
    /// <list type="number">
    /// <item><b>5491</b>：画布未 Active 或未 Initialized → 直接返回。
    ///   ★ <b>此时还没有刷新加载时间戳</b>（5492 在其后）⇒ 画布不可用时**不消耗节流窗口**，下次仍会立刻重试。</item>
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
    /// **相邻几行内写了两次**（第二次在自定义怪分支入口，冗余但保留）。
    /// ★ <b>全文不引用 `Sender`</b> —— 由 <see cref="LoadSurface()"/> 转调时传 <c>null</c> 是安全的。</para>
    /// </summary>
    public virtual void LoadSurface(object? sender) => ActorFamilyImpl.LoadSurface(this, sender);

    /// <summary>
    /// **无参派生槽位**（`p7-client-virtual` 在 `PlaySceneNewActor.cs:475` 落的分派槽位；
    /// `CustomActor.pas` 族的 `LoadSurface` 是无参的，覆写这一个）。
    /// <para>如实转调**原文签名**的 <see cref="LoadSurface(object?)"/>（原文全文不用 `Sender`，故传 <c>null</c>）。</para>
    /// </summary>
    public virtual void LoadSurface() => LoadSurface(null);

    // ══════════════════════════════════════════════════════════════════════
    // 2/5  DrawChr  ——  Actor.pas 6067-6129（63 行）
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
    public virtual void DrawChr(int dx, int dy, bool blend, bool boFlag)
        => ActorFamilyImpl.DrawChr(this, dx, dy, blend, boFlag);

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
    public virtual void DrawStateEffSurface(int ddx, int ddy)
        => ActorFamilyImpl.DrawStateEffSurface(this, ddx, ddy);

    // ══════════════════════════════════════════════════════════════════════
    // 3/5  RunSound  ——  Actor.pas 6788-6901（114 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `TActor.RunSound`（**6788-6901，114 行**）1:1。
    ///
    /// <para><b>6794-6795 是无条件两句</b>：先把 `m_boRunSound` 置真（**即使什么都不播**），
    /// 再调 `SetSound` 刷新音效槽；然后才是 `case m_nCurrentAction` 的五标签（**无 else**）。</para>
    /// </summary>
    public virtual void RunSound() => ActorFamilyImpl.RunSound(this);

    /// <summary>
    /// `TActor.SetSound`（原文 **6454-6786，333 行**）。
    ///
    /// <para><b>★ 空实现是**正确语义**，不是台账 §25.2 禁止的"未接线"</b>：原文整段被
    /// <c>6459 if m_btRace in [0, 1] then begin</c> 包住且**没有 else**；6766 的 <c>end;</c> 之后
    /// 6769-6785 的受击武器音段又要求 <c>PlayScene.FindActor(m_nHiterCode)</c> 命中。
    /// 即：**对怪物族（`m_btRace` ∉ {0,1}），原文的 `SetSound` 逐字就是"什么都不做"**。</para>
    ///
    /// <para><b>接缝：待 `TActor.SetSound`（Actor.pas:6454-6786）移植后接入</b>；
    /// 人类族的 `THumActor.SetSound` 覆写亦未移植（另一单元）。</para>
    /// </summary>
    public virtual void SetSound() => ActorFamilyImpl.SetSound(this);

    // ══════════════════════════════════════════════════════════════════════
    // 4/5  RunActSound  ——  Actor.pas 6903-7095（193 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `TActor.RunActSound`（**6903-7095，193 行**）1:1。
    ///
    /// <para><b>6909 是入口门</b>：`if m_boRunSound then` —— 整段逻辑都被它包住。</para>
    /// <para><b>6910 分两大支</b>：`m_btRace in [0, 1]` 走武器音族（6911-7043）；其余种族走 7045 起的 else
    /// （<c>m_btRace = 50</c> 空实现 / `SM_TURN`+frame1+`Random(8)=1` / `SM_HIT`+frame3 /
    /// `m_wAppearance = 80` 的 `m_nDie2Sound` / `m_btRace in [202..209]` 的硬编码音）。</para>
    /// </summary>
    public virtual void RunActSound(int frame) => ActorFamilyImpl.RunActSound(this, frame);

    // ══════════════════════════════════════════════════════════════════════
    // 5/5  子类接入点
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `m_ColorEffect` 的**虚读取点**（基类本体的三分支依据：5559 / 5575 / 6076-6081）。
    ///
    /// <para><b>为什么不是直接读一个字段</b>：<c>TCustomActor</c> 在 <c>CustomActor.cs:1628</c>
    /// 声明了它**自己的** <c>m_ColorEffect</c>（原文里这是 <c>TActor</c> 的单个字段）。
    /// 本车道的处置：**删除 `TCustomActor` 的那份重复字段**，改由它覆写本属性
    /// （见 `CustomActor.cs`「继承字段消重」段）—— 这样"同一外观颜色状态"只有**一份存储**。</para>
    ///
    /// <para>基类那一份存储叫 <see cref="TActorCore.m_BaseColorEffect"/>（历史上为避免字段隐藏而取的名）。</para>
    /// </summary>
    public virtual TColorEffect ActorColorEffect
    {
        get => m_BaseColorEffect;
        set => m_BaseColorEffect = value;
    }

    /// <summary>`m_btSex`（原文 6914/6932-6936/6950/7025-7037 的音效分流依据）。</summary>
    public virtual int ActorSex => m_btSex;

    /// <summary>`g_MySelf.m_boDeath`（原文 6105：施法特效层按**主角**是否死亡决定取灰度图）。</summary>
    public virtual bool ViewerDead => ActorFamilyEnv.ViewerDeadFn();
}
