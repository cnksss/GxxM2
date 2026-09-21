using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 `p17-client-actor` 切片 3 的**缺陷锁**（父 agent 提示：主分支刚修了 `X-P17-01`
/// ——「基类属性被派生自动属性隐藏 ⇒ 三条判定永远不可达」，同类"**隐藏而非覆写**"在本工程
/// 已 6 例，发现即当缺陷报出）。
///
/// <para><b>本节用途</b>：本车道用"类体深度感知"的脚本扫过 `Actor*`/`CustomActor*` 全族，
/// 方法层面**没有**隐藏（全部同名方法要么 `override`、要么基类无同名），
/// 但**字段层面找到 4 处**——其中两处在源码注释里被当作"有意保留"，一处在注释里被描述为
/// "镜像"，实际效果都是**同一语义状态分裂成两份不同步的存储**（台帐 §25.2 的静默缺陷形态）。</para>
///
/// <para><b>为什么把缺陷写成用例</b>（借用 `p16` 车道的教训）：留痕条目必须被用例**锁死**，
/// 否则修好之后没人会删它。本节每个用例的断言都是**当前（缺陷仍在时）的可观测结果**，
/// 并在注释里写明"修复后应改成什么"——修复者一旦改动继承结构，这里就会**红**，
/// 从而强制同步更新，而不是让缺陷悄悄消失又悄悄回来。</para>
///
/// <para><b>归属说明</b>：`CustomActor.cs` **不在本车道独占分区**（`Scenes/Actor*`）内，
/// 故本车道**只报不改**；修法建议写在各用例注释里，交集成方或 `CustomActor` 归属车道处置。</para>
/// </summary>
public sealed class ActorHiddenFieldDefectTests
{
    // ══════════════════════════════════════════════════════════════════════
    // H-1 ★ `m_nOldChrLight` —— **已修复（D-P17-08）**，本组用例改为锁修复后的形态
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// **H-1 修复验证**：经**基类静态类型**（`TActorCore`，即 `PlaySceneMessages.cs:672/677`
    /// 的真实情形）写入，必须落到 `TCustomActor` 的**真身存储**上。
    ///
    /// <para><b>原文事实（取证）</b>：字段只声明在 <c>CustomActor.pas:33</c>（<c>Integer</c>）；
    /// <c>PlayScn.pas:7852/8019/8023</c> 三处写点都是
    /// <c>if Actor is TCustomActor then TCustomActor(Actor).m_nOldChrLight := Actor.m_nChrLight;</c>
    /// —— 即"**类型守卫 + 写派生字段**"。</para>
    ///
    /// <para><b>修复前</b>本用例断言的是"写基类 7 ⇒ 派生读到 0"（状态分裂）；
    /// <b>修复后</b>（基类虚访问点 + 派生 <c>override</c> 存储）应读到 7。
    /// 这正是当时注释里写明的"修复后应改成什么"。</para>
    /// </summary>
    [Fact]
    public void H1_OldChrLight_WriteThroughBaseReferenceReachesCustomActorRealStorage()
    {
        var custom = new TCustomActor();
        TActorCore asBase = custom;

        // 模拟 PlaySceneMessages.cs:672/677：经**基类静态类型**写入
        asBase.m_nChrLight = 5;
        asBase.m_nOldChrLight = 7;

        // ★ 修复后：写到真身，读得到（修复前恒为 0 ⇒ 自定义怪掉光）
        Assert.Equal(7, custom.m_nOldChrLight);
        Assert.Equal(7, asBase.m_nOldChrLight);      // 同一存储，两个视角一致

        // 反向亦然
        custom.m_nOldChrLight = 9;
        Assert.Equal(9, asBase.m_nOldChrLight);
    }

    /// <summary>
    /// H-1 修复的结构面：两份存储已合一 ——
    /// 两侧都**不再有**名为 <c>m_nOldChrLight</c> 的**字段**；
    /// 基类是 <c>virtual</c> 属性，派生是 <c>override</c> 属性（真身存储在此）。
    /// </summary>
    [Fact]
    public void H1_OldChrLight_IsVirtualPropertyOnBaseAndOverrideOnDerived_NoFieldHiding()
    {
        // ★ 字段层面：两侧都不再有同名字段（隐藏的载体消失）
        Assert.Null(typeof(TActorCore).GetField(nameof(TActorCore.m_nOldChrLight)));
        Assert.Null(typeof(TCustomActor).GetField(nameof(TCustomActor.m_nOldChrLight)));

        var baseProp = typeof(TActorCore).GetProperty(nameof(TActorCore.m_nOldChrLight));
        var derivedProp = typeof(TCustomActor).GetProperty(nameof(TCustomActor.m_nOldChrLight));

        Assert.NotNull(baseProp);
        Assert.NotNull(derivedProp);
        Assert.Equal(typeof(TActorCore), baseProp!.DeclaringType);
        Assert.Equal(typeof(TCustomActor), derivedProp!.DeclaringType);
        Assert.Equal(typeof(int), baseProp.PropertyType);      // 原文 Integer
        Assert.Equal(typeof(int), derivedProp.PropertyType);

        // 基类访问器是 virtual，派生访问器是 override（同一 get/set 槽位）
        Assert.True(baseProp.GetMethod!.IsVirtual);
        Assert.True(baseProp.SetMethod!.IsVirtual);
        Assert.True(derivedProp.GetMethod!.IsVirtual);
        Assert.True(derivedProp.SetMethod!.IsVirtual);
        Assert.NotSame(baseProp.GetMethod, derivedProp.GetMethod);
        Assert.Same(baseProp.GetMethod, derivedProp.GetMethod!.GetBaseDefinition());
    }

    /// <summary>
    /// 普通（非 <c>TCustomActor</c>）角色上基类那份存储仍可读写 ——
    /// 这是**既有测试**（<c>FormJ73Tests.cs:744-745 / 760-761</c>）锁定的行为，
    /// 原文对普通角色既不写也不读它（无读点）⇒ 保留存储是**不可观测**的超集。
    /// <para>本用例把这一点显式记录下来，避免日后误以为"基类那份该整段删掉"而破坏既有断言。</para>
    /// </summary>
    [Fact]
    public void H1_OldChrLight_BaseStorageStillRoundTripsForNonCustomActor()
    {
        var plain = new TActor();
        TActorCore asBase = plain;

        asBase.m_nOldChrLight = 123;
        Assert.Equal(123, asBase.m_nOldChrLight);
        Assert.Equal(123, plain.m_nOldChrLight);
    }

    // ══════════════════════════════════════════════════════════════════════
    // H-2 `m_CurMagicEffectNumber`
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// **H-2**：`CustomActor.cs:1635` 隐藏 `ActorMotion.cs:58`（两份都是 <c>int</c>）。
    ///
    /// <para><b>读到哪一份取决于静态类型</b>：</para>
    /// <list type="bullet">
    /// <item>基类那一份被 <c>ActorFamilyImpl.cs:204</c>（<c>DrawChr</c> 的施法层，
    ///   <c>self</c> 静态类型是 <c>TActor</c>）与 <c>ActorMotion.cs:314</c>
    ///   （<c>ActionEnded</c> 的连击判定，<c>TActorCore</c> 静态类型）读取；</item>
    /// <item>派生那一份被 <c>CustomActor.cs:1871 / 1875 / 1937</c> 读取。</item>
    /// </list>
    /// 全工程**没有任何一处写入**这两份中的任何一份（原文里它是
    /// <c>m_CurMagic</c> 的成员、随魔法状态更新）。故当前两者都恒 0；
    /// 一旦有人把自定义怪的魔法状态接到**派生**那份，
    /// `ActorFamilyImpl.DrawChr` 的施法层仍会从**基类**那份读到 0 —— 施法特效不出现，
    /// 且**编译通过、单测通过**（台兄 §18.8 的形态）。
    ///
    /// <para><b>修复后本用例应改为</b>：删除派生声明后断言两份引用同一存储，
    /// 或删除本用例。</para>
    /// </summary>
    [Fact]
    public void H2_CurMagicEffectNumber_HiddenFieldSplitsDrawChrSpellLayerFromCustomActor()
    {
        var custom = new TCustomActor();
        TActorCore asBase = custom;

        custom.m_CurMagicEffectNumber = 105;      // 派生那份（CustomActor 的绘制路径读它）

        Assert.Equal(105, custom.m_CurMagicEffectNumber);
        // ★ 基类那份（ActorFamilyImpl.DrawChr 6101 的施法层 / ActorMotion 连击判定读它）仍是 0
        Assert.Equal(0, asBase.m_CurMagicEffectNumber);
    }

    // ══════════════════════════════════════════════════════════════════════
    // H-3 `m_nTargetRecog`
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// **H-3**：`CustomActor.cs:1629` 隐藏 `ActorFamilyHerbEnv.cs:271`（两份都是 <c>long</c>）。
    /// <para>`Actor.pas:1475` 只有一个 <c>m_nTargetRecog:Int64</c>，且 **`Actor.pas:3706` 在一处
    /// `<c>TActor</c>` 本体方法里使用它** —— 那个方法一旦被 1:1 移植，它读的必然是**基类**那一份，
    /// 而 `TCustomActor.Run`（853/942）写/读的是**派生**那一份 ⇒ 分裂。</para>
    /// <para>基类源码注释（`ActorFamilyHerbEnv.cs:269-270`）写的是"两者无交集，故保留同名字段
    /// （无需消重）"—— 该判断在**当前**成立，但它依赖"没有基类本体读它"这一**未来会被打破**的前提；
    /// 属于"靠约定维持、没有编译期保障"的形态。</para>
    /// <para><b>修复后本用例应改为</b>：两份合一后本用例自然失效，删除即可。</para>
    /// </summary>
    [Fact]
    public void H3_TargetRecog_HiddenFieldSplitsActorBodyFromCustomActor()
    {
        var custom = new TCustomActor();
        TActorCore asBase = custom;

        custom.m_nTargetRecog = 12345L;           // CustomActor.Run 853/942 用的那份
        Assert.Equal(12345L, custom.m_nTargetRecog);
        Assert.Equal(0L, asBase.m_nTargetRecog);  // ★ Actor.pas:3706 若移植，读到的是这份（仍 0）
    }

    // ══════════════════════════════════════════════════════════════════════
    // H-4 `m_CustomMagicStatusEffect_Struck`
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// **H-4**：`CustomActor.cs:1633` 隐藏 `ActorFamilyHerbEnv.cs:252`。
    /// <para>基类那份由 HerbActor 族写（`ActorFamilyHerb.cs:139 / 351`、
    /// `ActorFamilyStructures.cs:178`），派生那份由 `CustomActor.cs:1718` 写；
    /// **两份都没有任何读点**。故当前只是"两份死存储"，但 `TActorCore` 静态类型的任何读取
    /// 都会拿到基类那份 —— 与 H-2 同型。</para>
    /// <para>另注：原文这里是**一个记录** `m_CustomMagicStatusEffect:TCustomMagicStatusEffect`
    /// （`Actor.pas:1645`），托管侧用标量替代（D-P17 已登记的偏离）。</para>
    /// </summary>
    [Fact]
    public void H4_CustomMagicStatusEffectStruck_HiddenFieldSplitsHerbWritesFromCustomActor()
    {
        var custom = new TCustomActor();
        TActorCore asBase = custom;

        custom.m_CustomMagicStatusEffect_Struck = 3;   // CustomActor.cs:1718 写它
        Assert.Equal(3, custom.m_CustomMagicStatusEffect_Struck);
        Assert.Equal(0, asBase.m_CustomMagicStatusEffect_Struck);   // HerbActor 族读/写的是这份
    }

    // ══════════════════════════════════════════════════════════════════════
    // 附：本族的**方法**层面没有隐藏（正面证据）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 与 H-1..H-4 相对照：本车道给 `TActor` 的 NPC/人类族补虚槽位时，
    /// **方法**层面全部是 <c>override</c>（对有基类同名成员的）或**新建虚成员**
    /// （对原文里本就独立声明的），没有一处用 <c>new</c> 隐藏。
    /// <para>判据：有基类同名成员 ⇒ <c>GetBaseDefinition().DeclaringType != 自己</c>（真 override）；
    /// 基类没有同名成员 ⇒ <c>GetBaseDefinition()</c> 指回自己（合法的新虚成员）。</para>
    /// </summary>
    [Fact]
    public void ActorFamily_MethodLevel_NoHidingForTheSlotsThisLaneAdded()
    {
        // 有基类同名成员 ⇒ 必须是真 override
        var overrides = new (Type Derived, string Method)[]
        {
            (typeof(THumActor), nameof(THumActor.light)),
            (typeof(TNpcActor), nameof(TNpcActor.CalcActorFrame)),
            (typeof(TNpcActor), nameof(TNpcActor.LoadSurface)),
            (typeof(TNpcActor), nameof(TNpcActor.DrawChr)),
            (typeof(TNpcActor), nameof(TNpcActor.Run)),
            (typeof(TNpcActor), nameof(TNpcActor.GetDefaultFrame)),
            (typeof(TNpcActor), nameof(TNpcActor.CheckLoadUserName)),
            (typeof(TNpcActor), nameof(TNpcActor.Initialize)),
            (typeof(TStatuaryNpcActor), nameof(TStatuaryNpcActor.CheckLoadSurface)),
            (typeof(TStatuaryNpcActor), nameof(TStatuaryNpcActor.Destroy)),
            (typeof(TStatuaryNpcActor), nameof(TStatuaryNpcActor.Finalize)),
            (typeof(TStatuaryNpcActor), nameof(TStatuaryNpcActor.CalcActorFrame)),
        };

        foreach (var (derived, methodName) in overrides)
        {
            var candidates = derived
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.DeclaredOnly)
                .Where(m => m.Name == methodName)
                .ToArray();

            Assert.NotEmpty(candidates);
            foreach (var m in candidates)
            {
                Assert.True(m.GetBaseDefinition().DeclaringType != m.DeclaringType,
                    $"{derived.Name}.{methodName} 看起来是**隐藏**（GetBaseDefinition 指回自己）");
                Assert.Equal(derived, m.DeclaringType);
            }
        }

        // 基类没有同名成员 ⇒ 合法的新虚成员（原文里本就独立声明，如 Actor.pas:1930）
        var newVirtuals = new (Type Declaring, string Method)[]
        {
            (typeof(TStatuaryNpcActor), nameof(TStatuaryNpcActor.SetEffigyState)),
            (typeof(TNpcActor), nameof(TNpcActor.CheckLoadUserName)),   // 槽位在 TActorCore 上，见下
        };
        // CheckLoadUserName 其实有基类槽位，故只在此处核对 SetEffigyState 的"新虚成员"性质
        var setEffigy = typeof(TStatuaryNpcActor)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.DeclaredOnly)
            .Single(m => m.Name == nameof(TStatuaryNpcActor.SetEffigyState));
        Assert.Equal(typeof(TStatuaryNpcActor), setEffigy.GetBaseDefinition().DeclaringType);
        Assert.True(setEffigy.IsVirtual);
        _ = newVirtuals;
    }

    // ══════════════════════════════════════════════════════════════════════
    // NotPorted 留痕条目数 —— 用例锁死（p16 车道教训）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 台帐 §48.1 的 <c>NotPorted(...)</c> 留痕：**当前恰好 3 条**，逐条锁死。
    ///
    /// <para><b>为什么这条用例必须存在</b>（`p16` 车道的教训）：留痕条目**接线后必须同步删除**，
    /// 否则要么永远留着（把"已实现"误记为"未实现"），要么被静默删掉而无人复核。
    /// 本用例把条目的**名称 + 原文行号**逐条写进断言：</para>
    /// <list type="bullet">
    /// <item>某人把 <c>TActor.Destroy</c>(2945) / <c>CheckLoadUserName</c>(7120) /
    ///   <c>CheckLoadSurface</c>(7355) 中任何一条**真正实现**之后，
    ///   对应调用不再进留痕 ⇒ **本用例立刻红** ⇒ 强制作者回来删条目并更新报告对账表；</item>
    /// <item>反过来，有人新增 <c>NotPorted</c> 却忘记登记，本用例也会红。</item>
    /// </list>
    ///
    /// <para><b>★ 为什么用 <c>NotPortedCapture</c> 而不是进程级 <c>NotPortedLog</c></b>：
    /// xUnit 默认**并行**跑测试类，而 <c>NotPortedLog</c> 是静态表 ——
    /// 别的用例构造 <c>THumActor</c>（其构造函数会记录 <c>TActor.Create…@2777</c>）就会污染计数。
    /// <c>NotPortedCapture</c> 是 <c>[ThreadStatic]</c>，只收**本线程本用例**产生的条目，
    /// 故"条目数锁死"才是真的锁得住。</para>
    /// </summary>
    [Fact]
    public void NotPortedLog_ExactEntriesAreLocked()
    {
        TActorCore.NotPortedCapture = new List<string>();
        try
        {
            var actor = new TActor();          // 基类实例：派生类的真实现不参与

            var r1 = actor.CheckLoadUserName();   // 7120
            var r2 = actor.CheckLoadSurface();    // 7355
            actor.Destroy();                      // 2945

            Assert.False(r1);                     // ★ 留痕返回 false，**不是**裸 true（台帐 §48.1）
            Assert.False(r2);

            Assert.Equal(
                new[] { "CheckLoadUserName@7120", "CheckLoadSurface@7355", "Destroy@2945" },
                TActorCore.NotPortedCapture.ToArray());
            Assert.Equal(3, TActorCore.NotPortedCapture.Count);
        }
        finally
        {
            TActorCore.NotPortedCapture = null;
        }
    }

    /// <summary>
    /// `NotPorted` 的**可观测性**：它每次调用都留痕（不是静默中性值），
    /// 且恒返回 <c>false</c>；同一个槽位调用两次会留下**两条**记录（不静默去重）。
    /// </summary>
    [Fact]
    public void NotPorted_IsObservableEveryCallAndNeverReturnsTrue()
    {
        TActorCore.NotPortedCapture = new List<string>();
        try
        {
            var actor = new TActor();

            actor.Destroy();
            actor.Destroy();

            Assert.Equal(2, TActorCore.NotPortedCapture.Count);
            Assert.All(TActorCore.NotPortedCapture, e => Assert.Equal("Destroy@2945", e));
        }
        finally
        {
            TActorCore.NotPortedCapture = null;
        }
    }

    /// <summary>
    /// 留痕表的**全量口径**（进程级，供审计）：至少包含当前 4 个槽位。
    /// <para>这里用 <c>Contains</c> 而非精确相等 —— 进程级表会被并行用例追加，
    /// 精确相等只适合上面的 <c>NotPortedCapture</c>。<c>NotPortedLog</c> 是**只追加**的
    /// （本车道不再清空它），故本用例**自己产生**所需条目后再断言，不依赖测试执行顺序。</para>
    /// </summary>
    [Fact]
    public void NotPortedLog_ProcessWideContainsCurrentSlots()
    {
        _ = new THumActor();                       // 产生 11132 的 inherited Create 留痕

        var actor = new TActor();
        actor.CheckLoadUserName();                 // 7120
        actor.CheckLoadSurface();                  // 7355
        actor.Destroy();                           // 2945

        var snapshot = TActorCore.NotPortedLog.ToArray();

        Assert.Contains("CheckLoadUserName@7120", snapshot);
        Assert.Contains("CheckLoadSurface@7355", snapshot);
        Assert.Contains("Destroy@2945", snapshot);
        Assert.Contains("TActor.Create(inherited@11132)@2777", snapshot);
    }
}
