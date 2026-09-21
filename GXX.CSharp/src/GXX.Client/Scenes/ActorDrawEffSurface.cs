using System;

namespace GXX.Client.Scenes;

/// <summary>
/// 车道 `p17-client-actor`：`Actor.pas` <c>TActor</c> 的**表面绘制落点**虚成员
/// （<c>DrawEffSurface</c> 5704-5812 / <c>StretchDrawEffSurface</c> 5814-5886）。
///
/// <para><b>为什么这两个成员在此处才补</b>：<c>p2c-client-tail</c> 批次已把 5704-5886 的
/// **决策**落成 <see cref="ActorDrawDispatch.DrawEffSurface"/> /
/// <see cref="ActorDrawDispatch.StretchDrawEffSurface"/>（纯规划，返回 <see cref="SurfaceDrawOp"/>），
/// 但 <c>TActor</c> 上**没有同名方法** —— 于是 <c>TNpcActor.DrawChr</c>（10334/10342）与
/// <c>TNpcActor.DrawChr</c> 的外观 51 分支（10399）**没有可调用的目标**。
/// 本文件只加**薄落点**（转调既有规划 + 既有出口接缝），<b>不重写</b>那两段逻辑（台帐 §14.2「不造第三份实现」）。</para>
///
/// <para><b>虚分派</b>：原文 5704 / 5814 都是 <c>procedure ...; virtual;</c>（声明 1810-1811），
/// <c>THumActor</c> / <c>TNpcActor</c> 等子类不覆写它们本体，但 <c>TCustomActor</c> 族会经
/// <c>ActorColorEffect</c> 影响行为 —— 故保留 <c>virtual</c> 以便后续批次按原文接入覆写。</para>
/// </summary>
public partial class TActor
{
    /// <summary>
    /// `TActor.DrawEffSurface`（**5704-5812，109 行**）1:1 落点。
    ///
    /// <para><b>原文流程</b>：5706-5708 先判 <c>m_nState and $00800000</c> 强制 <c>blend := True</c>；
    /// 5709 <c>Assigned(Source)</c> 为假则**整段不画**（5757-5811 的三层状态绘制块**被原文整段注释**，
    /// 逐字不移植）。<c>m_btBodyColor &lt;&gt; 0</c> 走 <c>DrawColor/DrawColorAlpha(GetRGB(...), 150)</c>；
    /// 否则走 13 色 <c>ceff</c> 分流。</para>
    ///
    /// <para><b>★ 原文差异（已由既有规划层锁死，此处只做落点）</b>：非混合分支的
    /// <c>ceGreen</c> 用 <c>clLime</c>，而混合分支用 <c>clGreen</c>
    /// （原文 5723 注释「更改绿色为草绿色 clGreen --&gt; clLime piaoyun 2013-06-28」）；
    /// 混合分支**恒 alpha 150</b>（5739-5751）。</para>
    ///
    /// <para><b>注意</b>：<c>Source</c> 为 <c>null</c> 时**不产生任何绘制操作** ——
    /// 与原文 <c>Assigned(Source)</c> 门等价（<see cref="SurfaceDrawOp"/> 本身没有"未画"表示，
    /// 故必须在这一层拦掉）。</para>
    /// </summary>
    public virtual void DrawEffSurface(object? source, int ddx, int ddy, bool blend, TColorEffect ceff)
    {
        if (source == null)
            return;                                                        // 5709：Assigned(Source)

        ActorFamilyEnv.DrawEffSurfaceOpFn(ActorDrawDispatch.DrawEffSurface(
            m_nState, m_btBodyColor, ceff, blend, ddx, ddy));              // 5706-5755
    }

    /// <summary>
    /// `TActor.StretchDrawEffSurface`（**5814-5886，73 行**）1:1 落点。
    ///
    /// <para><b>原文 5848-5884 的整段 <c>{...}</c> 是注释掉的第二套实现</b>（按 <c>blend</c>
    /// 再分两支、<c>m_btColor &lt; 255</c> 判据），逐字**不移植** —— 生效的只有 5820-5847。</para>
    ///
    /// <para><b>几何（5821-5826）</b>：<c>nWidth := Round(Source.Width * 1.5)</c>、
    /// <c>nHeight := Round(Source.Height * 1.5)</c>、<c>nX := 12</c>、<c>nY := UNITY(32)</c>、
    /// <c>DestRect := Bounds(ddx - nX, ddy - nY, nWidth, nHeight)</c>、
    /// <c>SrcRect := Source.ClientRect</c>。原文用 Delphi <c>Round</c>（**银行家舍入**）——
    /// 既有规划层用 <c>MidpointRounding.ToEven</c> 承载。</para>
    ///
    /// <para><b>尺寸来源</b>：原文从 <c>Source.Width/Height</c> 取，headless 无纹理本体，
    /// 故经 <see cref="ActorNpcEnv.TextureSizeFn"/> 取。该接缝默认 <c>(0,0)</c>
    /// ⇒ 目标矩形为 0×0 —— 这是"取不到尺寸"的**保守**结果，不是静默中性值：
    /// 原文若 <c>Source</c> 存在则尺寸必 &gt; 0。</para>
    /// </summary>
    public virtual void StretchDrawEffSurface(object? source, int ddx, int ddy, bool blend, TColorEffect ceff)
    {
        if (source == null)
            return;                                                        // 5820

        var (srcWidth, srcHeight) = ActorNpcEnv.TextureSizeFn(source);
        ActorFamilyEnv.DrawEffSurfaceOpFn(ActorDrawDispatch.StretchDrawEffSurface(
            m_btBodyColor, ceff, ddx, ddy, srcWidth, srcHeight, m_btPhantomAlpha));   // 5821-5846
    }
}
