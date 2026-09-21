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
    // 0/12  Create  ——  Actor.pas 11130-11210（81 行）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.Create`（**11130-11210，81 行**）1:1（C# 构造函数承载 Delphi 构造器）。
    ///
    /// <para><b>流程与顺序（顺序即语义）</b>：</para>
    /// <list type="number">
    /// <item><b>11132 <c>inherited Create;</c></b> —— 指向 <c>TActor.Create</c>（**2777-2944，168 行**），
    ///   而该本体**尚未移植**（<c>TActorCore</c> 上没有 <c>Create</c> 成员）⇒ 此处**没有可转调的目标**，
    ///   以 <c>NotPorted("…", 2777)</c> **显式留痕**（台帐 §48.1），使"跳过了 168 行基类初始化"
    ///   在运行期可观测，而不是静默省略。</item>
    /// <item><b>11133-11145</b>：10 个纹理槽置 nil（头发/武器/盾牌/坐骑四件/人物窗两件）。
    ///   ★ <c>m_ShopStallSurface</c>/<c>m_ShopHeadSurface</c>/<c>m_HeroM2DressEffect</c>
    ///   在 <c>Create</c> 里**不清**（只在 <c>Finalize</c> 11244-11246 清）—— 这一**不对称**逐字保留。</item>
    /// <item><b>11146-11150</b>：<c>m_boWeaponEffect := False</c>；<c>m_dwFrameTime := 150</c>；
    ///   <c>m_dwFrameTick := TimeGetTime()</c>；<c>m_nFrame := 0</c>；<c>m_nHumWinOffset := 0</c>。</item>
    /// <item><b>11151-11155</b>：三个 EffectIndex 置 <b>-1</b>、<c>m_wDBWeaponEffectOffSet := 0</c>
    ///   （★ **0 而非 -1**，与同段三个 -1 形成对照）、<c>m_OnShopStall := nil</c>。</item>
    /// <item><b>11157-11181</b>：NoBlend/NoSex 开关全 False；<c>m_nMedalEffectIndex := -1</c>；
    ///   两个 DIY 字节置 0；三个 <c>*DrawNoBlend</c> 置 False；
    ///   附加衣服特效六件套（Index <b>-1</b> / Order 0 / OffSet 0 / Count 0 / Time 0 / NoBlend False）。</item>
    /// <item><b>11183-11184</b>：<c>m_nDressAddEffectCurIndex := 0</c>；
    ///   ★ <c>m_nDressAddEffectLastTick := MyGetTickCount</c> —— **另一个时钟**，
    ///   与 11148 的 <c>TimeGetTime</c> **不是同一个接缝**（本工程两者可分设，已用测试夹住）。</item>
    /// <item><b>11186-11194</b>：内功/心法开关 False；首饰盒 <c>jbsNoActive</c>；神佑袋 False；
    ///   三段 <c>FillChar(..., SizeOf(...), 0)</c>（内功属性 / 酒属性 / 人物经络）⇒ 托管侧 <c>default</c>。</item>
    /// <item><b>11196-11206</b>：HeroM2 五件 + <c>m_boBrokenShield := False</c>；
    ///   ★ <b>11207 是一个孤立的 <c>;</c></b>（原文如此，无副作用）。</item>
    /// <item><b>11209</b>：<c>m_FriendHitList := TStringList.Create;</c> ⇒ 托管侧该字段是
    ///   <c>readonly List&lt;string&gt;</c>（<c>ActorMessages.cs:55</c>）**已在字段初始化器里建好**，
    ///   故此处不赋值（语义等价，且 <c>readonly</c> 不允许）。</item>
    /// </list>
    ///
    /// <para><b>对既有测试的影响（已核对，见 D-P17-02）</b>：本构造函数把 <c>m_dwFrameTime</c>
    /// 从 0 改成 150、把四个 EffectIndex 从 0 改成 -1。切片 3a 已把"把 <c>THumActor</c> 当
    /// <c>TActor</c> 替身"的 5 处测试（<c>ActorFamilyBaseTests.NewActor()</c> 与
    /// <c>GuiMirSequelAnd205Tests</c>×4）改成直接构造 <c>TActor</c>，故这些改值**不再影响**那些断言；
    /// 本切片另加用例**正向**锁住新初值。</para>
    /// </summary>
    public THumActor()
    {
        // 11132：inherited Create → TActor.Create（2777-2944，168 行）**未移植**，显式留痕
        NotPorted(SourceLine2777_InheritedCreate, 2777);

        // 11133-11145：纹理槽（headless 无 TTexture，以 object? 承载）
        m_HairSurface = null;                        // 11133
        m_WeaponSurface = null;                      // 11134
        m_ShieldSurface = null;                      // 11135 盾牌 chongchong 2013-09-16

        m_HorseSurface = null;                       // 11137 骑马 chongchong 2013-10-12
        m_HorseWingsEffectSurface = null;            // 11138 骑马 马特效 chongchong 2013-10-16
        m_HorseEffectSurface = null;                 // 11139

        m_HorseHairSurface = null;                   // 11141 骑马 马上人物的头发 chongchong 2013-10-17
        m_HorseHumSurface = null;                    // 11142 骑马 马上人物 chongchong 2013-10-17

        m_HumWinSurface = null;                      // 11144
        m_HumWinSurface_30 = null;                   // 11145

        m_boWeaponEffect = false;                    // 11146
        m_dwFrameTime = 150;                         // 11147
        m_dwFrameTick = ActorNpcEnv.TimeGetTimeFn(); // 11148 TimeGetTime()
        m_nFrame = 0;                                // 11149
        m_nHumWinOffset = 0;                         // 11150

        m_nDressEffectIndex = -1;                    // 11151 长发发光效外观wil 编号
        m_nWeaponEffectIndex = -1;                   // 11152
        m_wDBWeaponEffectOffSet = 0;                 // 11153 ★ 0 而非 -1（2020-11-11 00:51:03）
        m_nShieldEffectIndex = -1;                   // 11154
        m_OnShopStall = null;                        // 11155

        m_boDressEffectNoBlend = false;              // 11157
        m_boDressEffectNoSex = false;                // 11158
        m_boWeaponEffectNoBlend = false;             // 11159
        m_boWeaponEffectNoSex = false;               // 11160

        m_nMedalEffectIndex = -1;                    // 11162 勋章发光效外观wil 编号
        m_boMedalEffectNoBlend = false;              // 11163
        m_boMedalEffectNoSex = false;                // 11164

        m_btCboDressUseDiyImage = 0;                 // 11166
        m_btCboWeaponUseDiyImage = 0;                // 11167

        m_boShieldEffectNoBlend = false;             // 11169
        m_boShieldEffectNoSex = false;               // 11170

        m_boDressEffectDrawNoBlend = false;          // 11172
        m_boWeaponEffectDrawNoBlend = false;         // 11173
        m_boShieldEffectDrawNoBlend = false;         // 11174

        m_nDressAddEffectIndex = -1;                 // 11176 附加衣服特效
        m_bDressAddEffectOrder = 0;                  // 11177 附加衣服特效绘制顺序
        m_wDressAddEffectOffSet = 0;                 // 11178 附加衣服特效偏移
        m_wDressAddEffectCount = 0;                  // 11179 附加衣服特效数量
        m_wDressAddEffectTime = 0;                   // 11180 附加衣服特效时间
        m_boDressAddEffectNoBlend = false;           // 11181 附加衣服特效 - 普通绘制

        m_nDressAddEffectCurIndex = 0;               // 11183
        m_nDressAddEffectLastTick = ActorNpcEnv.MyGetTickCountFn();   // 11184（★ 另一个时钟）

        m_boTrainingNG = false;                      // 11186 是否学习过内功
        m_boTrainingXF = false;                      // 11187 是否学习过心法

        m_nJewelryBoxStatus = TJewelryBoxStatus.jbsNoActive;   // 11189 chongchong 2013-10-19
        m_boShowGodBless = false;                    // 11190 chongchong 2014-04-18

        m_AbilNG = default;                          // 11192 FillChar(m_AbilNG, SizeOf(TAbilityNG), 0)
        m_Alcohol = default;                         // 11193 FillChar(m_Alcohol, SizeOf(TAbilityAlcohol), 0)
        m_HumMeridians = default;                    // 11194 FillChar(m_HumMeridians, SizeOf(THumMeridians), 0)

        m_wHeroM2DressEffect = 0;                    // 11196 HeroM2 ChangeDressEffect
        m_boHeroM2DressNoBlend = false;              // 11197

        m_wOldHeroM2DressEffect = 0;                 // 11199
        m_boOldHeroM2DressNoBlend = false;           // 11200

        m_nHeroM2DressEffectX = 0;                   // 11202
        m_nHeroM2DressEffectY = 0;                   // 11203
        m_HeroM2DressEffect = null;                  // 11204

        m_boBrokenShield = false;                    // 11206
        // 11207：原文此处是一个**孤立的 `;`**（空语句）—— 逐字保留为注释，不加任何副作用。

        // 11209：m_FriendHitList := TStringList.Create
        //   托管侧 m_FriendHitList 是 readonly List<string>（ActorMessages.cs:55），
        //   已在字段初始化器里建好 ⇒ 此处不重复赋值（语义等价）。
    }

    /// <summary>
    /// 11132 的 <c>NotPorted</c> 留痕名：它留痕的是**未移植的 <c>TActor.Create</c>**
    /// （不是本类的 <c>Create</c> —— 本类 <c>Create</c> 已 1:1 落地）。
    /// 名字里带原文行号，以免与 <c>NotPortedLog</c> 既有三条例目混淆。
    /// </summary>
    private const string SourceLine2777_InheritedCreate = "TActor.Create(inherited@11132)";

    // ══════════════════════════════════════════════════════════════════════
    // 0b/12  Destroy / Initialize / Finalize  ——  Actor.pas 11212-11258
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `THumActor.Destroy`（**11212-11216**）1:1。
    /// <para>11214 <c>inherited Destroy</c> → <c>TActor.Destroy</c>（2945-2968，25 行）**未移植**
    /// （<c>TActorCore.Destroy()</c> 上是 <c>NotPorted(2945)</c> 槽位），故 <c>base.Destroy()</c>
    /// 会记一条留痕。</para>
    /// <para>11215 <c>m_FriendHitList.Free</c> ⇒ 托管侧该字段是 <c>readonly List&lt;string&gt;</c>，
    /// "释放"的等价动作是 <c>Clear()</c>（对象由 GC 回收，且 <c>readonly</c> 不允许置 nil）。</para>
    /// </summary>
    public override void Destroy()
    {
        base.Destroy();                    // 11214 inherited Destroy（→ NotPorted(2945) 留痕）
        m_FriendHitList.Clear();           // 11215 m_FriendHitList.Free（见 D-P17-10）
    }

    /// <summary>
    /// `THumActor.Initialize`（**11218-11221**）：**只有 <c>inherited Initialize</c> 一句**。
    /// <para>转调 <c>TActor.Initialize</c>（5420-5423，**空体**，原文如此）⇒ 本方法净效果是
    /// "什么都不做"，但**必须显式转调**：原文是虚方法链，若省略则一旦基类实体变化就会静默绕过。</para>
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();                 // 11220
    }

    /// <summary>
    /// `THumActor.Finalize`（**11223-11258，36 行**）1:1。
    ///
    /// <para><b>顺序即语义</b>：11228 先 <c>inherited Finalize</c>，**再**清纹理槽。
    /// 清理清单与 <c>Create</c> 11133-11145 **不对称** —— 这里**多了**
    /// <c>m_ShopStallSurface</c>(11244) / <c>m_ShopHeadSurface</c>(11245) / <c>m_HeroM2DressEffect</c>(11246)
    /// 三个，逐字保留。</para>
    ///
    /// <para><b>11248-11257 的队列段</b>：<c>m_ActorEffects</c> 加锁遍历，**只把每项
    /// <c>Texture := nil</c>**，既不置 <c>boWantDelete</c> 也不清空队列
    /// ⇒ 由 <see cref="ActorPlayEffectQueue.FinalizeEffects"/> 1:1 承载（已用测试夹住"队列长度不变"）。</para>
    /// </summary>
    public override void Finalize()
    {
        base.Finalize();                           // 11228

        m_HairSurface = null;                      // 11230
        m_WeaponSurface = null;                    // 11231
        m_ShieldSurface = null;                    // 11232 盾牌 chongchong 2013-09-16

        m_HorseSurface = null;                     // 11234 骑马 chongchong 2013-10-12
        m_HorseWingsEffectSurface = null;          // 11235 骑马 马特效 chongchong 2013-10-16
        m_HorseEffectSurface = null;               // 11236

        m_HorseHairSurface = null;                 // 11238 骑马 马上人物的头发 chongchong 2013-10-17
        m_HorseHumSurface = null;                  // 11239 骑马 马上人物 chongchong 2013-10-17

        m_HumWinSurface = null;                    // 11241
        m_HumWinSurface_30 = null;                 // 11242

        m_ShopStallSurface = null;                 // 11244（★ Create 不清这三个）
        m_ShopHeadSurface = null;                  // 11245
        m_HeroM2DressEffect = null;                // 11246

        // 11248-11257：脚本命令播放特效——逐项 Texture := nil（不置 boWantDelete、不清队列）
        m_ActorEffects.FinalizeEffects();
    }

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

    // ──────────────────────────────────────────────────────────────────────
    // THumActor.Create（11130-11210）/ Finalize（11223-11258）直接读写
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`m_HairSurface` / `m_WeaponSurface` / `m_ShieldSurface`（11133-11135；11230-11232 清）。</summary>
    public object? m_HairSurface;
    public object? m_WeaponSurface;
    public object? m_ShieldSurface;

    /// <summary>坐骑四件：`m_HorseSurface`(11137) / `m_HorseWingsEffectSurface`(11138) /
    /// `m_HorseEffectSurface`(11139) / `m_HorseHairSurface`(11141) / `m_HorseHumSurface`(11142)。</summary>
    public object? m_HorseSurface;
    public object? m_HorseWingsEffectSurface;
    public object? m_HorseEffectSurface;
    public object? m_HorseHairSurface;
    public object? m_HorseHumSurface;

    /// <summary>`m_HumWinSurface` / `m_HumWinSurface_30`（11144-11145；11241-11242 清；
    /// 11264/11273 的 <c>DrawDressEffect</c> 读）。</summary>
    public object? m_HumWinSurface;
    public object? m_HumWinSurface_30;

    /// <summary>`m_ShopStallSurface` / `m_ShopHeadSurface`（原文 **Create 不清**、只在 11244-11245 清）。</summary>
    public object? m_ShopStallSurface;
    public object? m_ShopHeadSurface;

    /// <summary>`m_HeroM2DressEffect`（11196-11204 一族；11246 清）。</summary>
    public object? m_HeroM2DressEffect;

    /// <summary>`m_dwFrameTick`（11148 初值 <c>TimeGetTime()</c>；<c>DefaultMotion</c> 13144 的节流基准）。</summary>
    public uint m_dwFrameTick;

    /// <summary>`m_nFrame`（11149 初值 0；<c>DefaultMotion</c> 的自增游标）。</summary>
    public int m_nFrame;

    /// <summary>`m_nHumWinOffset`（11150 初值 0）。</summary>
    public int m_nHumWinOffset;

    /// <summary>`m_OnShopStall:TNotifyEvent`（11155 置 nil；原文 2042 是属性
    /// <c>OnShopStall read m_OnShopStall write m_OnShopStall</c>）。</summary>
    public Action? m_OnShopStall;

    /// <summary>`m_boDressEffectDrawNoBlend` / `m_boWeaponEffectDrawNoBlend` / `m_boShieldEffectDrawNoBlend`
    /// （11172-11174）。<para>与既有的 <c>m_boDressEffectNoBlend</c> 等**是两组不同字段**
    /// （前者是"绘制时"开关，后者是"效果"开关），不可合并。</para></summary>
    public bool m_boDressEffectDrawNoBlend;
    public bool m_boWeaponEffectDrawNoBlend;
    public bool m_boShieldEffectDrawNoBlend;

    /// <summary>
    /// `m_boMedalEffectDrawNoBlend`（原文声明 **1400**；使用点 11297；
    /// 赋值点 **16307** `<c>m_boMedalEffectDrawNoBlend := m_boMedalEffectNoBlend;</c>`，在 <c>LoadSurface</c> 段内）。
    /// <para>★ 它与 <c>m_boMedalEffectNoBlend</c>（已存在于 <c>PlaySceneActors.cs:164</c>）**是两个字段**：
    /// 后者是配置来源，前者是绘制判据 —— 直接合并会让 16307 的赋值消失。</para>
    /// </summary>
    public bool m_boMedalEffectDrawNoBlend;

    /// <summary>`m_boTrainingXF`（11187 是否学习过心法）。<c>m_boTrainingNG</c> 已在 PlaySceneActors.cs:205。</summary>
    public bool m_boTrainingXF;

    /// <summary>`m_nJewelryBoxStatus`（11189；原文 <c>jbsNoActive</c>，枚举见 <c>Grobal2.Types1.cs:14</c>）。</summary>
    public TJewelryBoxStatus m_nJewelryBoxStatus;

    /// <summary>`m_boShowGodBless`（11190 显示神佑袋）。</summary>
    public bool m_boShowGodBless;

    /// <summary>`m_Alcohol:TAbilityAlcohol`（11193 的 <c>FillChar(...,0)</c>）。</summary>
    public TAbilityAlcohol m_Alcohol;

    /// <summary>
    /// `m_HumMeridians:THumMeridians`（11194 的 <c>FillChar(...,0)</c>）。
    /// <para>原文 <c>THumMeridians = array [0..4] of TMeridian</c>（Grobal2.pas:4185）；
    /// 托管侧协议层已有等价承载 <c>TMeridianArray5</c>（<c>Grobal2.Types4.cs:271</c>），
    /// 故**不另造类型**，直接用它。</para>
    /// </summary>
    public TMeridianArray5 m_HumMeridians;

    /// <summary>HeroM2 五件：`m_wHeroM2DressEffect`(11196) / `m_boHeroM2DressNoBlend`(11197) /
    /// `m_wOldHeroM2DressEffect`(11199，原文 <c>Byte</c>) / `m_boOldHeroM2DressNoBlend`(11200) /
    /// `m_nHeroM2DressEffectX`+`Y`(11202-11203)。</summary>
    public ushort m_wHeroM2DressEffect;
    public bool m_boHeroM2DressNoBlend;
    public byte m_wOldHeroM2DressEffect;
    public bool m_boOldHeroM2DressNoBlend;
    public int m_nHeroM2DressEffectX;
    public int m_nHeroM2DressEffectY;

    /// <summary>
    /// `m_ActorEffects`（原文 11248-11257 遍历的脚本命令特效表）。
    /// <para>托管侧由 <see cref="ActorPlayEffectQueue"/>（本车道 <c>Actor*.cs</c> 分区内）承载，
    /// 此前**没有**任何 <c>TActorCore</c> 字段指向它（只有类定义）—— 本切片补上这一字段，
    /// 使 <c>THumActor.Finalize</c> 的队列段可 1:1 落地。
    /// 实例在字段初始化器里建好（对应 Delphi 侧 <c>TActor.Create</c> 里的 <c>TGList.Create</c>）。</para>
    /// </summary>
    public readonly ActorPlayEffectQueue m_ActorEffects = new();
}
