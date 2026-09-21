using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// 车道 `p17-client-actor` 切片 3d：`THumActor` 的**中等方法族**
/// —— <c>DrawDressEffect</c>(11260) / <c>DefaultMotion</c>(13136) / <c>GetDefaultFrame</c>(13217) /
/// <c>RunFrameAction</c>(13310)，以及它们需要的实例字段与两条**结构化效果接缝**。
///
/// <para><b>★ 不造第三份实现（台帐 §14.2）</b>：<c>DrawDressEffect</c> 的两个版本
/// （<c>TActor</c> 5974-5987 / <c>THumActor</c> 11260-11304）**已有** 1:1 规划层
/// <see cref="DressEffectRender.DrawBaseDressEffect"/> 与
/// <see cref="DressEffectRender.DrawHumDressEffect"/>（含 11303 的 <c>inherited</c> 落点与
/// 11332 的原文笔误说明）。本文件只加**薄落点**（规划 → 绘制出口），逻辑一行不重写。
/// <c>GetDefaultFrame</c> 的自定义怪帧公式同理复用
/// <see cref="CustomMonsterFrameCalc.BaseFrame"/>（原文的 <c>TempDir</c> 模式）。</para>
/// </summary>
public partial class THumActor
{
    // ══════════════════════════════════════════════════════════════════════
    // 0/5  DrawDressEffect 的**基类槽位**（TActor 5974-5987）
    // ══════════════════════════════════════════════════════════════════════

    // 说明：基类版 `TActor.DrawDressEffect`（原文 1756 声明为 virtual、实体 5974-5987）
    //       此前托管侧**完全没有这个名字**，故 THumActor 的 override 无处可落。
    //       实体见本文件末尾的 `TActor` partial（薄落点 → DressEffectRender.DrawBaseDressEffect）。

    // ══════════════════════════════════════════════════════════════════════
    // 1/5  DrawDressEffect  ——  Actor.pas 11260-11304（45 行）+ inherited 11303
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.DrawDressEffect`（**11260-11304**）1:1 落点（覆写 <c>TActor.DrawDressEffect</c> 5974）。
    ///
    /// <para><b>四个块各自独立</b>（**不是 else 链**）⇒ 可能同时产出多张图，顺序即原文顺序；
    /// 11303 的 <c>inherited</c> 让**基类块（<c>m_DressEffectSurface</c>）排在四条之后**。</para>
    ///
    /// <para><b>★ 混合极性在原文里不一致（逐字保留）</b>：</para>
    /// <list type="bullet">
    /// <item>①② 用 <c>not m_boEffectNormalDraw</c> / <c>not m_boEffect_30NormalDraw</c></item>
    /// <item>③ 用 <c>m_boHeroM2DressNoBlend</c>、④ 用 <c>m_boMedalEffectDrawNoBlend</c></item>
    /// </list>
    /// 三者的**最终行为一致**（标志为真 ⇒ <c>Draw</c>），但字段名与判据写法不同 ——
    /// 由既有规划层分别保留两个入口承载。</para>
    ///
    /// <para><b>★ 两个形参 <c>blend</c>/<c>ceff</c> 在 <c>THumActor</c> 版本中完全未被使用</b>
    /// （原文如此）：写成"用 blend 决定混合"会改变行为。</para>
    /// </summary>
    public override void DrawDressEffect(int ddx, int ddy, bool blend, TColorEffect ceff)
    {
        var ops = DressEffectRender.DrawHumDressEffect(
            ddx, ddy, m_nShiftX, m_nShiftY,
            m_HumWinSurface != null, m_nSpX, m_nSpY, m_boEffectNormalDraw,                        // 11264-11271
            m_HumWinSurface_30 != null, m_nSpX_30, m_nSpY_30, m_boEffect_30NormalDraw,           // 11273-11281
            m_HeroM2DressEffect != null, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY,
            m_boHeroM2DressNoBlend,                                                              // 11283-11291
            m_MedalEffectSurface != null, m_nMedalEffectX, m_nMedalEffectY,
            m_boMedalEffectDrawNoBlend,                                                          // 11293-11301
            m_DressEffectSurface != null, m_nDressEffectX, m_nDressEffectY,
            m_boDressEffectDrawNoBlend);                                                         // 11303 inherited

        foreach (var op in ops)
        {
            ActorNpcEnv.NpcDrawFn(new NpcDrawOp(
                op.Blend ? NpcDrawKind.DrawBlend : NpcDrawKind.Draw,
                op.X, op.Y,
                SurfaceOf(op.Kind),
                op.Kind,
                11260));
        }
    }

    /// <summary>
    /// 规划产物 <c>Kind</c> → 本类对应的纹理槽（**只做映射，不做判断**）。
    /// 规划层已判过"是否存在"，故此处不会再产生"拿了 nil 去画"的情况。
    /// </summary>
    private object? SurfaceOf(string kind) => kind switch
    {
        "HumWin" => m_HumWinSurface,
        "HumWin30" => m_HumWinSurface_30,
        "HeroM2Dress" => m_HeroM2DressEffect,
        "MedalEffect" => m_MedalEffectSurface,
        "DressEffect" => m_DressEffectSurface,
        _ => null,
    };

    // ══════════════════════════════════════════════════════════════════════
    // 2/5  DefaultMotion  ——  Actor.pas 13136-13215（80 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.DefaultMotion`（**13136-13215，80 行**）1:1（覆写 <c>TActor.DefaultMotion</c> 6217）。
    ///
    /// <para><b>结构：两段**完全同形**的块</b>（13142-13173 用 <c>m_wEffect</c>；
    /// 13175-13206 用 <c>m_wEffect_30</c>），中间**没有 else 关联** ⇒ 两段都会执行，
    /// 且共用**同一个** <c>nFrame</c> 局部变量与**同一个</c> <c>m_dwFrameTick</c>
    /// （第二段会看到第一段刚更新的 tick）。</para>
    ///
    /// <para><b>段内三分支</b>：</para>
    /// <list type="bullet">
    /// <item><c>effect = 50</c> 且 <c>m_nCurrentFrame &lt;= 536</c>：节流 100ms，
    ///   <c>nFrame &lt; 19</c> 自增，否则**翻转 <c>m_bo2D0</c>** 并 <c>nFrame := 0</c>；</item>
    /// <item>否则 <c>effect &lt;&gt; 0</c> 且 <c>m_nCurrentFrame &lt; 64</c>：节流 <c>m_dwFrameTime</c>，
    ///   <c>nFrame &lt; 7</c> 自增，否则 <c>nFrame := 0</c>；</item>
    /// <item>13170-13171 / 13203-13204 是**空 <c>else begin end</c>**（原文留白，逐字保留为空分支）。</item>
    /// </list>
    ///
    /// <para><b>★ 返回语义</b>：<c>Result := inherited DefaultMotion</c> 先取基类结果；
    /// 末尾"<b>仅当</b> <c>m_nFrame</c> 与局部 <c>nFrame</c> 不同"才写回并置 <c>True</c>
    /// —— 即基类已返回 True、而帧号未变时，**不是**简单地把 True 传下去，而是被覆盖为
    /// 帧号比较的结果。这是最容易被"优化"掉的一处。</para>
    /// </summary>
    public override bool DefaultMotion()
    {
        bool result = base.DefaultMotion();          // 13140
        int nFrame = m_nFrame;                       // 13141

        if (m_wEffect == 50)                         // 13142
        {
            if (m_nCurrentFrame <= 536)              // 13143
            {
                if (ActorNpcEnv.TimeGetTimeFn() - m_dwFrameTick > 100)   // 13144
                {
                    if (nFrame < 19)
                        nFrame++;                                        // 13146
                    else
                    {
                        m_bo2D0 = !m_bo2D0;                              // 13148-13151
                        nFrame = 0;                                      // 13152
                    }
                    m_dwFrameTick = ActorNpcEnv.TimeGetTimeFn();          // 13154
                }
            }
        }
        else if (m_wEffect != 0)                     // 13158-13159
        {
            if (m_nCurrentFrame < 64)                // 13160
            {
                if (ActorNpcEnv.TimeGetTimeFn() - m_dwFrameTick > m_dwFrameTime)   // 13161
                {
                    if (nFrame < 7)
                        nFrame++;                                        // 13163
                    else
                        nFrame = 0;                                      // 13165
                    m_dwFrameTick = ActorNpcEnv.TimeGetTimeFn();          // 13166
                }
            }
            else
            {
                // 13170-13171：原文的空 else begin end —— 逐字保留为空分支
            }
        }

        if (m_wEffect_30 == 50)                      // 13175（★ 与上一段同形，共用 nFrame / m_dwFrameTick）
        {
            if (m_nCurrentFrame <= 536)              // 13176
            {
                if (ActorNpcEnv.TimeGetTimeFn() - m_dwFrameTick > 100)   // 13177
                {
                    if (nFrame < 19)
                        nFrame++;                                        // 13179
                    else
                    {
                        m_bo2D0 = !m_bo2D0;                              // 13181-13184
                        nFrame = 0;                                      // 13185
                    }
                    m_dwFrameTick = ActorNpcEnv.TimeGetTimeFn();          // 13187
                }
            }
        }
        else if (m_wEffect_30 != 0)                  // 13191-13192
        {
            if (m_nCurrentFrame < 64)                // 13193
            {
                if (ActorNpcEnv.TimeGetTimeFn() - m_dwFrameTick > m_dwFrameTime)   // 13194
                {
                    if (nFrame < 7)
                        nFrame++;                                        // 13196
                    else
                        nFrame = 0;                                      // 13198
                    m_dwFrameTick = ActorNpcEnv.TimeGetTimeFn();          // 13199
                }
            }
            else
            {
                // 13203-13204：原文的空 else begin end
            }
        }

        if (m_nFrame != nFrame)                      // 13208
        {
            m_nFrame = nFrame;                       // 13209
            result = true;                           // 13210（★ 覆盖基类结果）
        }

        // 13211 `// PlayScene.LoadSurface(LoadSurface);` 与 13213-13214 的调试段均被原文注释，逐字不移植
        return result;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3/5  GetDefaultFrame  ——  Actor.pas 13217-13308（91 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.GetDefaultFrame`（**13217-13308，91 行**）1:1（覆写 <c>TActor.GetDefaultFrame</c> 6136）。
    ///
    /// <para><b>★ 13222 的 <c>m_nChangeAppr &gt;= 0</c> 是**整段前置门 + 无条件 13276 <c>Exit</c>**</b>
    /// —— 只要 <c>m_nChangeAppr &gt;= 0</c>，下面 13279-13307 的**骑马修正与 <c>HA.Act*</c> 三支全部不执行**。
    /// 门内又分三段：</para>
    /// <list type="number">
    /// <item><b>13223 <c>&gt;= 100000</c></b>：<c>Result := 0</c> 初值 → 线性查 <c>g_CustomMonsterConfig</c>
    ///   首命中 <c>wMonsterAppr = m_nChangeAppr - 100000</c> → **配置为 nil 则 <c>Result</c> 保持 0**；
    ///   否则四支：骨架死 ⇒ <c>Actions[matDie].StartIndex</c>（**原样，不加 TempDir**）；
    ///   普通死 ⇒ <c>BaseFrame(matDie) + PlayCount - 1</c>（落末帧）；
    ///   石化 ⇒ <c>BaseFrame(matStoneRevive)</c>；否则 ⇒ <c>BaseFrame(matStand) + cf</c>
    ///   （<c>cf</c> 由 <c>Actions[matStand].PlayCount</c> 钳制）。</item>
    /// <item><b>13273-13274</b>：<c>m_nChangeAppr</c> 在 <c>[0, 100000)</c> ⇒ <b>转调基类</b>
    ///   （<c>TActor.GetDefaultFrame</c> 6136）。</item>
    /// <item><b>13279-13307</b>（仅当 <c>m_nChangeAppr &lt; 0</c>）：骑马修正
    ///   <c>if wmode and (m_btHorse &lt;&gt; 0) then wmode := False</c>；再三支 —— 死亡
    ///   （骑马双载时用**魔数 <c>256 + m_btDir*8 + 8 - 1</c>**）、<c>wmode</c>、站立（唯一写
    ///   <c>m_nDefFrameCount</c> 的一支）。</item>
    /// </list>
    ///
    /// <para><b>★ 原文易错点</b>：① 13287 <c>m_btHorse in [1..5]</c>（**不含 0**）
    /// 且要求 <c>m_btDoubleHumHorse &lt;&gt; 0</c>；② 13293 的 <c>m_boCustomMagicNoAction</c>
    /// 只影响 <c>wmode</c> 支；③ 13299 <c>m_nDefFrameCount</c> **只在站立支写**。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        int cf;
        int result = 0;

        if (m_nChangeAppr >= 0)                                   // 13222
        {
            if (m_nChangeAppr >= 100000)                          // 13223
            {
                result = 0;                                        // 13224
                var cfg = ActorFamilyEnv.CustomMonsterConfigLookupFn?.Invoke(m_nChangeAppr);   // 13227-13233

                if (cfg != null)                                   // 13235
                {
                    var c = cfg.Value;
                    if (m_boDeath)                                 // 13236
                    {
                        if (m_boSkeleton)
                            result = c.Actions[(int)TMonsterClientActionType.matDie].StartIndex;   // 13238
                        else
                        {
                            // 13240-13244：TempDir 模式下 + (PlayCount - 1) 落末帧
                            result = CustomMonsterFrameCalc.BaseFrame(
                                c.Actions[(int)TMonsterClientActionType.matDie], m_btDir)
                                + (c.Actions[(int)TMonsterClientActionType.matDie].PlayCount - 1);
                        }
                    }
                    else if (StateStoneMode)                       // 13248
                    {
                        result = CustomMonsterFrameCalc.BaseFrame(
                            c.Actions[(int)TMonsterClientActionType.matStoneRevive], m_btDir);      // 13254
                    }
                    else
                    {
                        var stand = c.Actions[(int)TMonsterClientActionType.matStand];
                        if (m_nCurrentDefFrame < 0)                // 13257
                            cf = 0;
                        else if (m_nCurrentDefFrame >= stand.PlayCount)
                            cf = 0;
                        else
                            cf = m_nCurrentDefFrame;
                        result = CustomMonsterFrameCalc.BaseFrame(stand, m_btDir) + cf;            // 13268
                    }
                }

                return result;                                     // 13276 Exit（★ 跳过下面全部）
            }
            else
            {
                return base.GetDefaultFrame(wmode);                // 13274
                // 13276 Exit
            }
        }

        // 修复攻击时骑马在人攻击归位前不显示 chongchong 2013-11-08
        if (wmode && m_btHorse != 0)                               // 13280
            wmode = false;

        var ha = ActorActionTables.HA;                             // HA（原文 188-204 的人物动作表）

        if (m_boDeath)                                             // 13285
        {
            // 骑马 - 修复官方马骑马死亡后躺下状态 chongchong 2013-10-17
            if (m_btHorse is >= 1 and <= 5 && m_btDoubleHumHorse != 0)   // 13287
                result = 256 + m_btDir * (7 + 1) + 8 - 1;                 // 13288（原文魔数）
            else
                result = ha.ActDie.start + m_btDir * (ha.ActDie.frame + ha.ActDie.skip)
                    + (ha.ActDie.frame - 1);                              // 13290
        }
        else if (wmode)                                            // 13292
        {
            if (m_boCustomMagicNoAction)                           // 13293
                result = ha.ActStand.start + m_btDir * (ha.ActStand.frame + ha.ActStand.skip);
            else
                result = ha.ActWarMode.start + m_btDir * (ha.ActWarMode.frame + ha.ActWarMode.skip);
        }
        else
        {
            m_nDefFrameCount = ha.ActStand.frame;                  // 13299（★ 只在这一支写）
            if (m_nCurrentDefFrame < 0)                            // 13300
                cf = 0;
            else if (m_nCurrentDefFrame >= ha.ActStand.frame)
                cf = 0;                                            // 13303 `// HA.ActStand.frame-1`
            else
                cf = m_nCurrentDefFrame;
            result = ha.ActStand.start + m_btDir * (ha.ActStand.frame + ha.ActStand.skip) + cf;   // 13306
        }

        return result;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4/5  RunFrameAction  ——  Actor.pas 13310-13353（43 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.RunFrameAction`（**13310-13353，43 行**）1:1（覆写 <c>TActor.RunFrameAction</c> 7097，
    /// 后者是**空体**）。
    ///
    /// <para><b>流程</b>：</para>
    /// <list type="number">
    /// <item><b>13316</b>：<c>m_boHideWeapon := False</c>（**无条件**首句）。</item>
    /// <item><b>13317-13332</b>：<c>SM_HEAVYHIT</c> 且 <c>frame = 5</c> 且 <c>m_boDigFragment</c> ⇒
    ///   翻标志 → 建 <c>TMapEffect(8 * m_btDir, 3, x, y)</c>、<c>ImgLib := g_WEffectImg</c>、
    ///   <c>NextFrameTime := 80</c>、播 <c>s_strike_stone</c>、<c>AddEffectList</c>、
    ///   再 <c>EventMan.GetEvent(x, y, ET_PILESTONES)</c> 命中则 <c>m_nEventParam + 1</c>。</item>
    /// <item><b>13333-13352</b>：<c>SM_THROW</c> 且 <c>frame = 3</c> 且 <c>m_boThrow</c> ⇒
    ///   <c>NewFlyObject(Self, x, y, m_nTargetX, m_nTargetY, m_nTargetRecog, mtFlyAxe)</c>；
    ///   非 nil 时置 <c>ReadyFrame := 40</c> / <c>ImgLib := g_WMonImages.Indexs[3]</c> /
    ///   <c>FlyImageBase := FLYOMAAXEBASE</c>。<b>随后独立一句</b>
    ///   <c>if frame &gt;= 3 then m_boHideWeapon := True</c> —— **注意它不在上面的 <c>if</c> 里**。</item>
    /// </list>
    ///
    /// <para><b>★ 原文易错点</b>：13350 的 <c>frame &gt;= 3</c> 与 13334 的 <c>frame = 3</c> 是
    /// **两个独立判据**（前者覆盖 frame &gt; 3 的所有帧）；把它并进上面的 if 会让"帧 4..N 隐藏武器"失效。</para>
    ///
    /// <para><b>接缝</b>：效果实例化（<c>TMapEffect</c>/<c>TFlyingAxe</c>/<c>EventMan</c>）属
    /// 特效与地图事件批次，故经两条**结构化请求**接缝
    /// （<see cref="ActorNpcEnv.SpawnDigFragmentEffectFn"/> /
    /// <see cref="ActorNpcEnv.SpawnThrowAxeFn"/>）携带原文全部实参出去；
    /// 标志位与帧判据留在本方法内。两条接缝默认 <c>null</c> = **未接线 ⇒ 不请求**
    /// （而非"请求了但没效果"）。</para>
    /// </summary>
    public override void RunFrameAction(int frame)
    {
        m_boHideWeapon = false;                                    // 13316

        if (m_nCurrentAction == SM_HEAVYHIT)                       // 13317
        {
            if (frame == 5 && m_boDigFragment)                     // 13318
            {
                m_boDigFragment = false;                           // 13319
                ActorNpcEnv.SpawnDigFragmentEffectFn?.Invoke(new HumDigFragmentEffect(
                    Dir8: 8 * m_btDir,                             // 13320
                    Kind: 3,
                    X: m_nCurrX,
                    Y: m_nCurrY,
                    ImageLib: "WEffectImg",                        // 13321 g_WEffectImg
                    NextFrameTime: 80,                             // 13322
                    SoundName: "s_strike_stone",                   // 13323
                    EventType: "ET_PILESTONES"));                   // 13328（命中则 m_nEventParam + 1，见 13330）
            }
        }

        if (m_nCurrentAction == SM_THROW)                          // 13333
        {
            if (frame == 3 && m_boThrow)                           // 13334
            {
                m_boThrow = false;                                 // 13335
                ActorNpcEnv.SpawnThrowAxeFn?.Invoke(new HumThrowAxeEffect(
                    X: m_nCurrX,                                   // 13337
                    Y: m_nCurrY,                                   // 13338
                    TargetX: m_nTargetX,                           // 13339
                    TargetY: m_nTargetY,                           // 13340
                    TargetRecog: m_nTargetRecog,                   // 13341
                    ReadyFrame: 40,                                // 13344
                    ImgLibIndex: 3,                                // 13345 g_WMonImages.Indexs[3]
                    FlyImageBase: MagicEffConsts.FLYOMAAXEBASE));   // 13346
            }

            if (frame >= 3)                                        // 13350（★ 独立判据，不在上面的 if 内）
                m_boHideWeapon = true;                             // 13351
        }
    }
}

/// <summary>
/// 切片 3d 需要的实例字段（partial 扩展 <c>TActorCore</c>）。
/// <para><b>不重复声明</b>：<c>m_HumWinSurface</c>/<c>m_HumWinSurface_30</c>/<c>m_HeroM2DressEffect</c>
/// /<c>m_nHeroM2DressEffectX</c>/<c>m_nHeroM2DressEffectY</c>/<c>m_boHeroM2DressNoBlend</c>/
/// <c>m_boMedalEffectDrawNoBlend</c>/<c>m_boDressEffectDrawNoBlend</c>
/// 已在 <c>ActorHumActor.cs</c> 的 <c>TActorCore</c> 扩展里声明；
/// <c>m_boCustomMagicNoAction</c> 在 <c>ActorMotion.cs:70</c>；
/// <c>m_nTargetRecog</c> 在 <c>ActorFamilyHerbEnv.cs:271</c>；
/// <c>m_nCurrX</c>/<c>m_nCurrY</c> 在 <c>ActorCore.cs:36</c>。</para>
/// </summary>
public partial class TActorCore
{
    // ---- DrawDressEffect（11260-11304）与基类版（5974-5987） ----

    /// <summary>`m_nSpX` / `m_nSpY`（11265-11266：人物窗特效偏移，**不是** <c>m_nPx/m_nPy</c>）。</summary>
    public int m_nSpX;
    public int m_nSpY;

    /// <summary>`m_nSpX_30` / `m_nSpY_30`（11274-11275：30 号人物窗特效偏移）。</summary>
    public int m_nSpX_30;
    public int m_nSpY_30;

    /// <summary>`m_MedalEffectSurface`（11293 判；勋章发光效外观图）。</summary>
    public object? m_MedalEffectSurface;

    /// <summary>`m_nMedalEffectX` / `m_nMedalEffectY`（11294-11295）。</summary>
    public int m_nMedalEffectX;
    public int m_nMedalEffectY;

    /// <summary>`m_DressEffectSurface`（5974-5987 基类版的图；11303 的 <c>inherited</c> 画它）。</summary>
    public object? m_DressEffectSurface;

    /// <summary>`m_nDressEffectX` / `m_nDressEffectY`（基类版偏移）。</summary>
    public int m_nDressEffectX;
    public int m_nDressEffectY;

    // ---- DefaultMotion（13136-13215） ----

    /// <summary>`m_bo2D0`（13148-13151 / 13181-13184：20 帧循环到顶时**翻转**的开关）。</summary>
    public bool m_bo2D0;

    // ---- RunFrameAction（13310-13353） ----

    /// <summary>`m_boHideWeapon`（13316 无条件清；13351 在 <c>SM_THROW</c> 且 <c>frame &gt;= 3</c> 时置真）。</summary>
    public bool m_boHideWeapon;

    /// <summary>`m_boDigFragment`（13318-13319：<c>SM_HEAVYHIT</c> 帧 5 的一次性碎石化开关）。</summary>
    public bool m_boDigFragment;

    /// <summary>`m_boThrow`（13334-13335：<c>SM_THROW</c> 帧 3 的一次性投掷开关）。</summary>
    public bool m_boThrow;

    // 注意：`m_nTargetX` / `m_nTargetY`（13339-13340）**已在 `ActorFamilyHerbEnv.cs:261-262`
    //       声明** —— 此处不重复声明（避免造出第二份存储）。
}

/// <summary>
/// `SM_HEAVYHIT` 帧 5 的**碎石化效果请求**（原文 13320-13330：<c>TMapEffect</c> + <c>g_PlaySound</c>
/// + <c>PlayScene.AddEffectList</c> + <c>EventMan.GetEvent(ET_PILESTONES)</c>）。
/// <para>字段与原文实参一一对应；<c>ImageLib</c>/<c>SoundName</c>/<c>EventType</c> 用**符号名**而非
/// 托管侧对象，以免本车道持有特效/事件/音频子系统的句柄（它们的批次另属）。</para>
/// </summary>
public sealed record HumDigFragmentEffect(
    int Dir8, int Kind, int X, int Y, string ImageLib, int NextFrameTime, string SoundName, string EventType);

/// <summary>
/// `SM_THROW` 帧 3 的**飞斧请求**（原文 13336-13347：<c>PlayScene.NewFlyObject</c> + 三个字段写入）。
/// </summary>
public sealed record HumThrowAxeEffect(
    int X, int Y, int TargetX, int TargetY, long TargetRecog,
    int ReadyFrame, int ImgLibIndex, int FlyImageBase);

/// <summary>
/// 车道 `p17-client-actor` 切片 3d：<c>TActor.DrawDressEffect</c> 的**基类槽位与落点**
/// （原文声明 1756 <c>virtual</c>；实体 **5974-5987**，18 行）。
///
/// <para><b>★ 此前托管侧没有这个名字</b>：所以 <c>THumActor.DrawDressEffect</c>（原文 2018
/// <c>override</c>）无处可落。本切片补出虚拟基槽 + 1:1 落点。</para>
///
/// <para><b>实体流程（5974-5987）</b>：只处理 <c>m_DressEffectSurface</c>；
/// 非 nil 时按 <c>m_boDressEffectDrawNoBlend</c>（**为真 ⇒ <c>Draw</c>**）画在
/// <c>(ddx + m_nDressEffectX + m_nShiftX, ddy + m_nDressEffectY + m_nShiftY)</c>。
/// 逻辑由既有规划层 <see cref="DressEffectRender.DrawBaseDressEffect"/> 承载（不重写）。</para>
/// </summary>
public partial class TActor
{
    /// <summary>`TActor.DrawDressEffect`（5974-5987）1:1 落点；<c>THumActor</c> 覆写它并在末尾转调。</summary>
    public virtual void DrawDressEffect(int ddx, int ddy, bool blend, TColorEffect ceff)
    {
        // ★ 两个形参 blend / ceff 在原文的两个版本里**都不参与判断**（逐字保留为未使用）
        var op = DressEffectRender.DrawBaseDressEffect(
            ddx, ddy,
            m_DressEffectSurface != null, m_boDressEffectDrawNoBlend,
            m_nDressEffectX, m_nDressEffectY, m_nShiftX, m_nShiftY);

        if (op == null)
            return;

        ActorNpcEnv.NpcDrawFn(new NpcDrawOp(
            op.Blend ? NpcDrawKind.DrawBlend : NpcDrawKind.Draw,
            op.X, op.Y, m_DressEffectSurface, op.Kind, 5974));
    }
}
