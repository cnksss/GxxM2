using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// **车道 `p17-client-actor`**：`Actor.pas` 的 <c>THumActor</c>（**11130-17436，25 条实现，
/// 约 5,000 行**）的**类声明迁移点 + 已落地的确定性方法体**。
///
/// <para><b>★ 本文件是"空壳 → 归属归位"的迁移点</b>：原先把 <c>THumActor</c> 声明在
/// <c>PlaySceneNewActor.cs:488-491</c>（源注释写的是 <c>PlayScn.pas</c> 的 <c>NewActor</c> 分派表），
/// 与 <c>Actor.pas</c> 的 <c>THumActor</c> 只是**同名**。本车道把声明迁到本文件。</para>
///
/// <para><b>★ 基类关系</b>：原文 <c>THumActor = class(TActor)</c>（1935），故托管侧
/// <c>: TActor</c> 是**正确**的（与 <c>THeroActor : THumActor</c> 那处错位不同）。</para>
///
/// <para><b>⚠ 尚未落地的 17 条（已登记的近似物 D-P17-02）</b>：
/// <c>Create</c>(11130, 82 行) / <c>Destroy</c>(11212) / <c>Initialize</c>(11218) /
/// <c>Finalize</c>(11223) / <c>DrawDressEffect</c>(11260) / <c>DrawDressEffectEx</c>(11307) /
/// <c>CalcActorFrame</c>(11338, **1783 行**) / <c>DefaultMotion</c>(13136) /
/// <c>GetDefaultFrame</c>(13217) / <c>RunFrameAction</c>(13310) / <c>CheckLoadUserName</c>(13367) /
/// <c>CheckLoadSurface</c>(13465, **172 行**) / <c>OnTargetFinished</c>(13650) /
/// <c>PlayMagicEffect</c>(13758, **326 行**) / <c>Run</c>(14084, **436 行**) /
/// <c>LoadSurface</c>(14532, **1969 行**) / <c>DrawChr</c>(16501, **925 行**)。
///
/// <para>在本类上**故意不加 <c>override</c> 空体**：既有测试以 <c>new THumActor()</c> 作
/// <c>TActor</c> 的替身断言 <c>TActor</c> 本体行为（<c>ActorFamilyBaseTests.NewActor()</c>），
/// 一旦在此覆盖成 <c>NotPorted</c>，多态调用点会**静默变成空操作**，把"未移植"伪装成
/// "已移植但没效果"（台帐 §25.2 禁止的形态）。故此处保留"继承 <c>TActor</c> 的通用实现"
/// 这一**已知近似**，并在报告「未完成/阻塞」与「偏离登记 D-P17-02」中逐条列出。
/// 正确做法（留给后续切片）：先把那些测试改为直接构造 <c>TActor</c>，再逐条补 <c>override</c>。</para>
///
/// <para><b>本切片已落地的 8 条</b>：<c>UseMagicDelayTime</c>(13121) / <c>light</c>(14520) /
/// <c>DoWeaponBreakEffect</c>(13355) / <c>DoBrokenShieldEffect</c>(13361) /
/// <c>CheckLoadDressAddEffect</c>(13419) / <c>LoadDressAddEffect</c>(13439) /
/// <c>OnTargetExplosion</c>(13637) / <c>TakeHorse</c>(17426)。
/// 全部为**纯函数或纯字段写**，不依赖本类构造期字段初始化，故不会改变既有
/// <c>new THumActor()</c> 的初始状态（这一点是刻意的：构造期初始化属未完成项）。</para>
/// </summary>
public partial class THumActor : TActor
{
    /// <summary>
    /// 类名（原文无此成员；<c>TActorCore.ActorClass</c> 为托管侧诊断用虚属性，
    /// <c>PlaySceneNewActor</c> 分派与既有测试依赖它）。迁移前的原值逐字保留。
    /// </summary>
    public override string ActorClass => "THumActor";

    // ══════════════════════════════════════════════════════════════════════
    // 1/8  UseMagicDelayTime  ——  Actor.pas 13121-13134（14 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.UseMagicDelayTime`（**13121-13134**）1:1。
    ///
    /// <para><b>★ 13125-13130 整段（按 <c>nSpellSpeed</c> 加速）被原文 <c>{...}</c> 注释掉</b>，
    /// 13132 的注释说明了原因：「技能延时时间由数据库配置，这里不用加速了 chongchong 2018-07-04 23:54:18」。
    /// 生效的只有 13133：<c>Result := 200 + dwDelayTime</c>。</para>
    ///
    /// <para>托管侧逐字不移植注释块（原文如此）；若"补回"被注释的加速公式，
    /// 会让所有人物技能的施法帧时长变短。</para>
    /// </summary>
    public int UseMagicDelayTime(int dwDelayTime)
    {
        // {nSpellSpeed := g_ClientConfig.nSpellSpeed + m_nSpellSpeed;
        //  if nSpellSpeed <> 0 then
        //    Result := Max((300 + dwDelayTime) - Round((300 + dwDelayTime) * nSpellSpeed * 10 / 100), 0)
        //  else
        //    Result := (300 + dwDelayTime);}     ← 原文 13125-13130 整段注释，逐字不移植

        // 技能延时时间由数据库配置，这里不用加速了 chongchong 2018-07-04 23:54:18
        return 200 + dwDelayTime;                                          // 13133
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2/8  light  ——  Actor.pas 14520-14530（11 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.light`（**14520-14530**）1:1（覆写 <c>TActor.light</c> 5415-5418）。
    ///
    /// <para><b>★ 只有"魔法光比角色光更亮"时才提升</b>：<c>L := m_nChrLight</c>；
    /// 仅当 <c>L &lt; m_nMagLight</c> **且**（<c>m_boUseMagic</c> 或 <c>m_boHitEffect</c>）时
    /// 才 <c>L := m_nMagLight</c>。故"正放魔法但魔法光更暗" ⇒ 保持角色光（不是取 max 的直觉写法，
    /// 因为 <c>m_boUseMagic</c> 为真但 <c>m_nMagLight</c> 更小时不取）。</para>
    ///
    /// <para><c>m_boHitEffect</c> 是**与**条件里的或项（14084 的 <c>Run</c> 会置它）——
    /// 写成"只看 m_boUseMagic"会漏掉受击特效的提亮。</para>
    /// </summary>
    public override int light()
    {
        int l = m_nChrLight;                                              // 14524
        if (l < m_nMagLight)                                              // 14525
        {
            if (m_boUseMagic || m_boHitEffect)                            // 14526
                l = m_nMagLight;                                          // 14527
        }
        return l;                                                         // 14529
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3-4/8  DoWeaponBreakEffect / DoBrokenShieldEffect（13355-13365）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.DoWeaponBreakEffect`（**13355-13359**）1:1：置 <c>m_boWeaponEffect := True</c>
    /// 并把 <c>m_nCurWeaponEffect</c> 归零（**归零即"从头播"**，不在于它当前是几）。
    /// </summary>
    public void DoWeaponBreakEffect()
    {
        m_boWeaponEffect = true;                                          // 13357
        m_nCurWeaponEffect = 0;                                          // 13358
    }

    /// <summary>
    /// `THumActor.DoBrokenShieldEffect`（**13361-13365**）1:1：置 <c>m_boBrokenShield := True</c>
    /// 并把 <c>m_nBrokenShieldEffect</c> 归零。
    ///
    /// <para><b>注意与 13355 的两处不同</b>：① 二者写的是**两组不同字段**
    /// （武器破裂 vs 盾牌破碎）；② <c>m_boBrokenShield</c> 在 <c>THumActor.Create</c> 11206 被初始化，
    /// 而 <c>m_boWeaponEffect</c> 在 11146 被初始化 —— 均为 False。</para>
    /// </summary>
    public void DoBrokenShieldEffect()
    {
        m_boBrokenShield = true;                                         // 13363
        m_nBrokenShieldEffect = 0;                                       // 13364
    }

    // ══════════════════════════════════════════════════════════════════════
    // 5/8  CheckLoadDressAddEffect  ——  Actor.pas 13419-13437（19 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.CheckLoadDressAddEffect`（**13419-13437**）1:1。
    ///
    /// <para><b>流程</b>：三重门前置（<c>Index &gt;= 0</c>、<c>Count &gt; 0</c>、<c>Time &gt; 0</c>；
    /// 13423 的 <c>m_wDressAddEffectOffSet &gt;= 0</c> 因该字段是 Word 被原文注释掉 ——
    /// 逐字保留"不判它"）；门内快照 <c>prv</c>，按**严格大于** <c>&gt;</c> 的节流推进帧并回卷；
    /// 结果 = 「帧号变了」。</para>
    ///
    /// <para><b>★ 门外分支会清 <c>m_DressAddEffectSurface</c>（13434）</b>
    /// —— 即"配置被撤掉时主动丢弃已取的图"，不是单纯返回 False。</para>
    ///
    /// <para><b>13426 是 <c>&gt;</c> 而不是 <c>&gt;=</c></b>（与 <c>TNpcActor.Run</c> 11104 的
    /// <c>&gt;=</c> 恰好相反）—— 这类"相邻函数一个闭一个开"的差异必须逐字照抄。</para>
    /// </summary>
    public bool CheckLoadDressAddEffect()
    {
        if (m_nDressAddEffectIndex >= 0 && m_wDressAddEffectCount > 0 && m_wDressAddEffectTime > 0)   // 13423
        {
            int prv = m_nDressAddEffectCurIndex;                          // 13425
            uint now = ActorNpcEnv.MyGetTickCountFn();
            if (now - m_nDressAddEffectLastTick > m_wDressAddEffectTime)   // 13426：严格大于
            {
                m_nDressAddEffectLastTick = now;                          // 13427
                m_nDressAddEffectCurIndex++;                              // 13428
                if (m_nDressAddEffectCurIndex >= m_wDressAddEffectCount)   // 13429
                    m_nDressAddEffectCurIndex = 0;                        // 13430
            }
            return prv != m_nDressAddEffectCurIndex;                      // 13432
        }
        else
        {
            m_DressAddEffectSurface = null;                               // 13434
            return false;                                                 // 13435（HZQ 20230525）
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // 6/8  LoadDressAddEffect  ——  Actor.pas 13439-13463（25 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.LoadDressAddEffect`（**13439-13463**）1:1。
    ///
    /// <para><b>★ 13443 先无条件清空</b>，再判**四重门**（与 <c>CheckLoadDressAddEffect</c> 的
    /// 三重门不同：这里**多一个</b> <c>m_nDressAddEffectCurIndex &gt;= 0</c>，13445）。
    /// 即"帧游标为负 ⇒ 不取图（且保持 nil）"。</para>
    ///
    /// <para>取图三分支用 <c>m_ColorEffect</c>，与 <c>TNpcActor</c> 一致，经
    /// <see cref="ActorNpcEnv.FetchEffectListImageFn"/> 承载；注意
    /// <c>ceGrayScale</c> **不含** <c>ceGrayScale2</c>（13452 只列了前者）——
    /// 与 <c>TNpcActor.LoadSurface</c> 的若干段"灰度+灰度2"不同，逐字保留。</para>
    /// </summary>
    public void LoadDressAddEffect()
    {
        m_DressAddEffectSurface = null;                                   // 13443

        if (m_nDressAddEffectIndex >= 0 && m_wDressAddEffectCount > 0 && m_wDressAddEffectTime > 0
            && m_nDressAddEffectCurIndex >= 0)                            // 13444-13445
        {
            if (m_nDressAddEffectIndex < ActorNpcEnv.EffectImageListCountFn())   // 13448
            {
                var fetch = ActorNpcEnv.FetchEffectListImageFn(
                    m_nDressAddEffectIndex,
                    m_wDressAddEffectOffSet + m_nDressAddEffectCurIndex,  // 13452-13455
                    ActorColorEffect);
                m_DressAddEffectSurface = fetch.Texture;
                m_nDressAddEffectX = fetch.OffsetX;
                m_nDressAddEffectY = fetch.OffsetY;
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // 7/8  OnTargetExplosion  ——  Actor.pas 13637-13648（12 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.OnTargetExplosion`（**13637-13648**）1:1。
    ///
    /// <para><b>双重门</b>：① <c>Sender is TCustomMonFlyEffect</c>（类型门，避免把别的 Sender 当飞行特效解引用）；
    /// ② <c>Length(FlyEffect.ClientConfig.Sounds[custMagicExplosion]) &gt; 0</c>
    /// （**空串 = 未配置该条音效**，与"配置存在但音效为空"是同一层判据）。</para>
    ///
    /// <para><b>接缝</b>：飞行特效载体与 <c>ClientConfig.Sounds[...]</c> 属魔法批次
    /// （<c>MagicEffectsCustomMon.cs</c>）。本车道用
    /// <see cref="ActorNpcEnv.MonFlyEffectExplosionSoundFn"/> 取"该 Sender 的爆炸音命名"，
    /// 返回 <c>null</c> = 不是飞行特效（对应门①为假），返回**空串** = 门②为假
    /// —— 两级 <c>null</c>/空串在类型层面可区分，不是静默中性值。</para>
    /// </summary>
    public void OnTargetExplosion(object? sender)
    {
        string? sound = ActorNpcEnv.MonFlyEffectExplosionSoundFn(sender);   // 13641-13644
        if (!string.IsNullOrEmpty(sound))                                   // 13644
            ActorFamilyEnv.PlaySoundByNameFn(sound!);                       // 13645
    }

    // ══════════════════════════════════════════════════════════════════════
    // 8/8  TakeHorse  ——  Actor.pas 17426-17433（8 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.TakeHorse`（**17426-17433**）1:1。
    ///
    /// <para><b>★ 条件是 <c>m_btHorse = 0</c>（还没有马）⇒ 传 <c>1</c>；否则传 <c>0</c></b>
    /// —— 即"请求召唤"与"请求收回"共用同一个 <c>CM_TAKEHORSE</c>，靠第二个参数区分。
    /// 两个分支的坐标/参数都是 0。</para>
    ///
    /// <para>17432 的 <c>LegendMap.Stop</c> 是**后置无条件副作用**（不随分支变化）。</para>
    /// </summary>
    public void TakeHorse()
    {
        if (m_btHorse == 0)                                               // 17428
            ActorNpcEnv.SendClientMessageFn(CM_TAKEHORSE, 1, 0, 0, 0);    // 17429
        else
            ActorNpcEnv.SendClientMessageFn(CM_TAKEHORSE, 0, 0, 0, 0);    // 17431

        ActorNpcEnv.LegendMapStopFn();                                    // 17432
    }

    /// <summary>`CM_TAKEHORSE = 5002`（Common/Grobal2.pas:446，注释：「(骑马) 召唤/收回坐骑 chongchong 2013-10-12」）。</summary>
    public const int CM_TAKEHORSE = 5002;
}

/// <summary>
/// 车道 `p17-client-actor`：`THumActor` 已落地方法所需的**实例字段**（partial 扩展 <c>TActorCore</c>）。
///
/// <para><b>不重复声明</b>：<c>m_nDressAddEffectIndex</c>/<c>m_bDressAddEffectOrder</c>/
/// <c>m_wDressAddEffectOffSet</c>/<c>m_wDressAddEffectCount</c>/<c>m_wDressAddEffectTime</c>/
/// <c>m_boDressAddEffectNoBlend</c>/<c>m_boDressAddEffectDrawCenter</c>
/// 已在 <c>PlaySceneActors.cs:178-184</c>；<c>m_boUseMagic</c>（ActorCore.cs:60）、
/// <c>m_boHitEffect</c>（本车道 ActorNpcEnv.cs）亦已存在。</para>
/// </summary>
public partial class TActorCore
{
    /// <summary>
    /// `m_nChrLight`（原文 5420 附近 / 5492；<c>TActor.light</c> 5417 的读取源）。
    /// <para><b>注意</b>：<c>m_nChrLight</c> 已在 <c>ActorMessages.cs:52</c> 以 <c>byte</c> 声明，
    /// 而原文的 <c>TActor.light</c> 返回 <c>Integer</c>、<c>THumActor.light</c> 与
    /// <c>m_nMagLight</c> 比较。故此处**不重复声明** <c>m_nChrLight</c>，只补
    /// <see cref="m_nMagLight"/>。</para>
    /// </summary>
    public int m_nMagLight;

    /// <summary>`m_boWeaponEffect`（原文 11146 初值 False；13357 置真）。</summary>
    public bool m_boWeaponEffect;

    /// <summary>`m_nCurWeaponEffect`（原文 13358 归零；14532 起的 <c>LoadSurface</c> 推进）。</summary>
    public int m_nCurWeaponEffect;

    /// <summary>`m_boBrokenShield`（原文 11206 初值 False；13363 置真）。</summary>
    public bool m_boBrokenShield;

    /// <summary>`m_nBrokenShieldEffect`（原文 13364 归零）。</summary>
    public int m_nBrokenShieldEffect;

    /// <summary>`m_nDressAddEffectCurIndex`（原文 11183 初值 0；13425-13430 推进/回卷）。</summary>
    public int m_nDressAddEffectCurIndex;

    /// <summary>`m_nDressAddEffectLastTick`（原文 11184 初值 <c>MyGetTickCount</c>；13426-13427）。</summary>
    public uint m_nDressAddEffectLastTick;

    /// <summary>`m_DressAddEffectSurface`（原文 13434/13443 清、13452-13455 取）。</summary>
    public object? m_DressAddEffectSurface;

    /// <summary>`m_nDressAddEffectX` / `m_nDressAddEffectY`（原文 13452-13455 的 <c>out</c> 偏移）。</summary>
    public int m_nDressAddEffectX;
    public int m_nDressAddEffectY;
}
