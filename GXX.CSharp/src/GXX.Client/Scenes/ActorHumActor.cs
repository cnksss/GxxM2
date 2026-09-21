namespace GXX.Client.Scenes;

/// <summary>
/// **车道 `p17-client-actor`**：`Actor.pas` 的 <c>THumActor</c>（**11130-17436，25 条实现，
/// 约 5,000 行**）的**类声明迁移点**。
///
/// <para><b>★ 本文件是"空壳 → 归属归位"的第一步</b>：原先把 <c>THumActor</c> 声明在
/// <c>PlaySceneNewActor.cs:488-491</c>（源注释写的是 <c>PlayScn.pas</c> 的 <c>NewActor</c> 分派表），
/// 与 <c>Actor.pas</c> 的 <c>THumActor</c> 只是**同名**。本车道把声明迁到本文件；
/// 1:1 方法体按切片逐个补入（见 <c>docs/并行报告-p17-client-actor.md</c> 的逐例程对账表）。</para>
///
/// <para><b>★ 基类关系</b>：原文 <c>THumActor = class(TActor)</c>（1935），故托管侧
/// <c>: TActor</c> 是**正确**的（与 <c>THeroActor : THumActor</c> 那处错位不同）。</para>
///
/// <para><b>⚠ 当前未落地的 25 条</b>：本类此刻**只有** <c>ActorClass</c>。
/// 于是 <c>THumActor</c> 实例走的是 <c>TActor</c>/<c>TActorCore</c> 的通用实现 ——
/// 这**不是**"已移植"，而是**已登记的近似物（D-P17-02）**：
/// 原文 <c>THumActor</c> 覆写的 <c>CalcActorFrame</c>(11338) / <c>LoadSurface</c>(14532) /
/// <c>DrawChr</c>(16501) / <c>Run</c>(14084) / <c>PlayMagicEffect</c>(13758)
/// 等在本类上**尚不可达**。之所以先不加 <c>NotPorted</c> 覆写，是为了**不让既有多态行为退化**：
/// 既有测试以 <c>new THumActor()</c> 作 <c>TActor</c> 的替身断言 <c>TActor</c> 本体行为
/// （<c>ActorFamilyBaseTests.NewActor()</c>），一旦在此覆盖会连带改掉那些断言的语义。
/// 本车道的处置：**同一批次内**把那些测试改为直接构造 <c>TActor</c>，然后逐条补 <c>override</c>。</para>
/// </summary>
public partial class THumActor : TActor
{
    /// <summary>
    /// 类名（原文无此成员；<c>TActorCore.ActorClass</c> 为托管侧诊断用虚属性，
    /// <c>PlaySceneNewActor</c> 分派与既有测试依赖它）。迁移前的原值逐字保留。
    /// </summary>
    public override string ActorClass => "THumActor";
}
