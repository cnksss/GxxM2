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
    // H-1 ★ `m_nOldChrLight`：基类 byte 份 vs TCustomActor 派生 int 份
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// **H-1（最严重，且已确认可观测）**：`CustomActor.cs:1612` 声明
    /// <c>public int m_nOldChrLight;</c>，而基类 `TActorCore` 已在
    /// `ActorMessages.cs:53` 声明 <c>public byte m_nOldChrLight;</c>
    /// —— **同名、不同存储、甚至不同类型**（`int` vs `byte`）。
    ///
    /// <para><b>后果链（全部可用静态类型推出）</b>：</para>
    /// <list type="number">
    /// <item><c>PlaySceneCore.cs:153</c>：<c>ActorList</c> 是 <c>List&lt;TActorCore&gt;</c>；</item>
    /// <item><c>PlaySceneMessages.cs:672 / 677</c>：<c>actor.m_nOldChrLight = actor.m_nChrLight;</c>
    ///   —— <c>actor</c> 的静态类型是 <c>TActorCore</c> ⇒ 写的是**基类那一份（byte）**，
    ///   而且 <c>ProcessActors</c> **每次都写**；</item>
    /// <item><c>CustomActor.cs:1679 / 1806</c>：<c>TCustomActor</c> 自己的方法体读
    ///   <c>m_nOldChrLight</c> ⇒ 读的是**派生那一份（int）**，而**全工程没有任何一处写它**；</item>
    /// <item>于是自定义怪（`TCustomActor`）传给 <c>CustomActorLogic</c> 的
    ///   <c>OldChrLight</c> **恒为 0**，98 行的 <c>m_nChrLight := m_nOldChrLight</c>
    ///   也就把光照写成 0（原文语义是"沿用上一轮光照"）。</item>
    /// </list>
    ///
    /// <para><b>另注</b>：`Actor.pas` **根本没有** <c>m_nOldChrLight</c> —— 它只存在于
    /// <c>CustomActor.pas:33</c>。基类那份是托管侧为"镜像"它而新增的
    /// （`ActorMessages.cs:51` 的注释即如此写），却因为隐藏而**镜像不到**。</para>
    ///
    /// <para><b>建议修法</b>（三选一，推荐第 1 条）：① 删除基类的 <c>m_nOldChrLight</c>
    /// （原文没有它），把 <c>ProcessActors</c> 的两处改写指向派生字段所属的真实需要
    /// ——注意 <c>ProcessActors</c> 是基类静态类型，故需改成虚属性或接口；
    /// ② 删除 `CustomActor.cs:1612`，让 <c>TCustomActor</c> 用继承的 <c>byte</c> 那份
    /// （需同步把 1679/1806 的类型推导改为 <c>byte</c> 语义）；
    /// ③ 基类改为 <c>public virtual int OldChrLight { get; set; }</c>，派生 <c>override</c>。</para>
    ///
    /// <para><b>修复后本用例应改为</b>：断言 <c>custom</c> 能读到基类写入的 7
    /// （即两份合一），或直接删除本用例。</para>
    /// </summary>
    [Fact]
    public void H1_OldChrLight_HiddenFieldSplitsState_BetweenBaseByteAndDerivedInt()
    {
        var custom = new TCustomActor();
        TActorCore asBase = custom;

        // 步骤 1：模拟 PlaySceneMessages.cs:672/677 经**基类静态类型**写入（ProcessActors 每次都做）
        asBase.m_nChrLight = 5;
        asBase.m_nOldChrLight = 7;

        // 基类那一份确实写进去了
        Assert.Equal(7, asBase.m_nOldChrLight);

        // ★ 步骤 2：`TCustomActor` 自己的方法体读的是**派生 int 那一份** ⇒ 恒为 0（缺陷仍在）
        Assert.Equal(0, custom.m_nOldChrLight);

        // ★ 两份是**独立存储**：写派生不影响基类，反之亦然
        custom.m_nOldChrLight = 9;
        Assert.Equal(9, custom.m_nOldChrLight);
        Assert.Equal(7, asBase.m_nOldChrLight);   // 基类那份没被改
    }

    /// <summary>
    /// H-1 的类型面：基类 <c>byte</c> / 派生 <c>int</c> —— 同一语义字段两种宽度，
    /// 编译期**不报错**（C# 字段隐藏只给 CS0108 警告），运行期靠静态类型分流。
    /// </summary>
    [Fact]
    public void H1_OldChrLight_BaseAndDerivedHaveDifferentFieldTypes()
    {
        var baseField = typeof(TActorCore).GetField(nameof(TActorCore.m_nOldChrLight));
        var derivedField = typeof(TCustomActor).GetField(nameof(TCustomActor.m_nOldChrLight));

        Assert.NotNull(baseField);
        Assert.NotNull(derivedField);
        Assert.NotSame(baseField, derivedField);
        Assert.Equal(typeof(byte), baseField!.FieldType);              // ActorMessages.cs:53
        Assert.Equal(typeof(int), derivedField!.FieldType);            // CustomActor.cs:1612
        // ★ GetField（Public|Instance）返回的是**最派生**那一份 ⇒ 声明类型必须是 TCustomActor，
        //   即"确有隐藏"（若两份合一，这里会返回 DeclaringType == TActorCore）
        Assert.Equal(typeof(TCustomActor), derivedField.DeclaringType);
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
    ///   对应调用不再进 <c>NotPortedLog</c> ⇒ **本用例立刻红** ⇒
    ///   强制作者回来删条目并更新本报告的对账表；</item>
    /// <item>反过来，有人新增 <c>NotPorted</c> 却忘记登记，本用例也会红。</item>
    /// </list>
    ///
    /// <para><b>注意</b>：<c>NotPortedLog</c> 是**静态**累积表，故本用例先清空；
    /// 它记录的是"被调用过的留痕槽位"，不是"代码里存在多少处 NotPorted"。</para>
    /// </summary>
    [Fact]
    public void NotPortedLog_ExactEntriesAreLocked()
    {
        TActorCore.NotPortedLog.Clear();
        var actor = new TActor();          // 基类实例：派生类的真实现不参与

        var r1 = actor.CheckLoadUserName();   // 7120
        var r2 = actor.CheckLoadSurface();    // 7355
        actor.Destroy();                      // 2945

        Assert.False(r1);                     // ★ 留痕返回 false，**不是**裸 true（台帐 §48.1）
        Assert.False(r2);

        Assert.Equal(
            new[] { "CheckLoadUserName@7120", "CheckLoadSurface@7355", "Destroy@2945" },
            TActorCore.NotPortedLog.ToArray());
        Assert.Equal(3, TActorCore.NotPortedLog.Count);
    }

    /// <summary>
    /// `NotPorted` 的**可观测性**：它每次调用都留痕（不是静默中性值），
    /// 且恒返回 <c>false</c>；同一个槽位调用两次会留下**两条**记录（不静默去重）。
    /// </summary>
    [Fact]
    public void NotPorted_IsObservableEveryCallAndNeverReturnsTrue()
    {
        TActorCore.NotPortedLog.Clear();
        var actor = new TActor();

        actor.Destroy();
        actor.Destroy();

        Assert.Equal(2, TActorCore.NotPortedLog.Count);
        Assert.All(TActorCore.NotPortedLog, e => Assert.Equal("Destroy@2945", e));
    }
}
