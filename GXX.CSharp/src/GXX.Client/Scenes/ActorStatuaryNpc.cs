using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// **车道 `p17-client-actor`**：`Actor.pas` 的 <c>TStatuaryNpcActor</c>（**17537-18008，9 条实现**）1:1。
///
/// <para><b>★ 空壳迁移</b>：原先只是 <c>PlaySceneNewActor.cs:537</c> 的一行空壳
/// （<c>public class TStatuaryNpcActor : TNpcActor { public override string ActorClass =&gt; "TStatuaryNpcActor"; }</c>）。
/// 本车道把类声明移出并在本文件写真实现。</para>
///
/// <para><b>虚分派</b>：原文 9 条全部 <c>override</c>（声明 1921-1929）+
/// <c>SetEffigyState</c>（1930，非虚）；<c>boScaleShow</c> 只读属性（1932）。
/// 托管侧槽位：<c>TActorCore.CalcActorFrame()</c> / <c>Run(uint)</c>（本类不覆写 Run）/
/// <c>GetDefaultFrame(bool)</c> / <c>Finalize()</c> / <c>TActor.LoadSurface(object?)</c> /
/// <c>TActor.DrawChr(int,int,bool,bool)</c>，以及本车道新增的 <c>CheckLoadSurface()</c> 虚槽位。</para>
///
/// <para><b>一处必须说清的结构事实</b>：<c>TStatuaryNpcActor.Create</c>（17558）调用的是
/// <c>inherited</c>（即 <c>TNpcActor.Create</c>，10231 —— 它会按 <c>m_wAppearance</c> 查自定义 NPC 配置
/// 决定 <c>m_nKeepFrame</c>），**不是** <c>TActor.Create</c>。原文层级是
/// <c>TStatuaryNpcActor : TNpcActor</c>（1903），本类据此声明为 <c>: TNpcActor</c>
/// —— 若误声明为 <c>: TActor</c>，<c>base</c> 转调会**整段绕过** <c>TNpcActor</c> 的初始化。</para>
/// </summary>
public partial class TStatuaryNpcActor : TNpcActor
{
    /// <summary>类名（原文无此成员；迁移前 <c>PlaySceneNewActor.cs:537</c> 空壳上的原值，逐字保留）。</summary>
    public override string ActorClass => "TStatuaryNpcActor";

    /// <summary>
    /// `TStatuaryNpcActor.CalcActorFrame`（**17537-17551**）1:1。
    ///
    /// <para><b>★ 本方法完全不用 <c>pm</c>/<c>GetRaceByPM</c></b> —— 它是"雕像"专用的常量帧：
    /// <c>m_nBodyOffset := 1200</c>、<c>m_btDir := 0</c>、帧 0..0、<c>m_dwFrameTime := 100</c>、
    /// <c>m_nDefFrameCount := 1</c>。也**不 <c>inherited</c>**（改写 <c>TNpcActor.CalcActorFrame</c> 的全部行为）。</para>
    ///
    /// <para><b>17549-17550 是两句并列的打点</b>：<c>m_dwStartTime</c> 与 <c>m_StartCounter</c>
    /// 都取 <c>TimeGetTime</c>，与本族其它方法一致。</para>
    /// </summary>
    public override void CalcActorFrame()
    {
        m_boUseMagic = false;                                             // 17539
        m_nCurrentFrame = -1;                                             // 17540
        m_nBodyOffset = 1200;                                             // 17541

        m_btDir = 0;                                                      // 17543

        m_nStartFrame = 0;                                                // 17545
        m_nEndFrame = 0;                                                  // 17546
        m_dwFrameTime = 100;                                              // 17547
        m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();                      // 17548
        m_StartCounter = ActorNpcEnv.TimeGetTimeFn();                     // 17549
        m_nDefFrameCount = 1;                                             // 17550
    }

    /// <summary>
    /// `TStatuaryNpcActor.CheckLoadSurface`（**17553-17556**）：**只有 <c>inherited</c> 一句**。
    /// <para>原文 17555 转调 <c>TNpcActor.CheckLoadSurface</c>（该本体在 <c>TActor</c> 上，
    /// 原文 7355-7366）。托管侧基类此前**没有** <c>CheckLoadSurface</c> 这个名字
    /// （见报告「新增虚槽位」），本车道在 <c>TActorCore</c> 上补出同名虚成员，故此处逐字 <c>base</c> 转调。</para>
    /// </summary>
    public override bool CheckLoadSurface()
    {
        return base.CheckLoadSurface();                                   // 17555
    }

    /// <summary>
    /// `TStatuaryNpcActor.Create`（**17558-17579**）1:1（C# 构造函数承载 Delphi 构造器）。
    ///
    /// <para><b>顺序即语义</b>：17661-17678 先把"形象五件套"清零
    /// （<c>m_wDress</c>/<c>m_wWeapon</c>/<c>m_wWeaponSound</c>/<c>m_wShield</c> 与 <c>m_btHair</c>），
    /// 再置 <c>m_boScaleShow := True</c>、<c>m_IsGrayShow := False</c>、<c>FIsFinalized := False</c>。</para>
    ///
    /// <para><b>17652-17653 写的是 <c>m_nEffigyState.Value1 := 0; Value2 := 0;</c></b>
    /// —— **没有**动 <c>wDressEffType</c>（第三个字段）；逐字保留，见测试
    /// <c>StatuaryCreate_LeavesEffigyDressEffTypeUntouched</c>。</para>
    /// </summary>
    public TStatuaryNpcActor()
    {
        m_boShowStatuary = false;                                         // 17561
        m_nEffigyState.Value1 = 0;                                        // 17562
        m_nEffigyState.Value2 = 0;                                        // 17563

        m_nEffigyOffset = 0;                                              // 17565
        m_nOldEffigyOffset = 0;                                           // 17566

        m_wDress = 0;                                                     // 17568
        m_wWeapon = 0;                                                    // 17569
        m_wWeaponSound = 0;                                               // 17570
        m_wShield = 0;                                                    // 17571

        m_btHair = 0;                                                     // 17573

        m_boScaleShow = true;                                             // 17575
        m_IsGrayShow = false;                                             // 17576

        FIsFinalized = false;                                             // 17578
    }

    /// <summary>
    /// `TStatuaryNpcActor.Destroy`（**17581-17593**）1:1。
    ///
    /// <para><b>顺序即语义</b>：三张 800×800 离屏纹理**先各自判 nil 再释放**，
    /// 最后才 <c>inherited</c>（17592）—— 反过来会让基类析构先跑。</para>
    ///
    /// <para>托管侧无 <c>TTexture</c>，<c>FreeAndNil</c> 的等价见
    /// <see cref="TActorCore.FreeAndNil"/>（非 <see cref="IDisposable"/> 的只置 nil）。</para>
    /// </summary>
    public override void Destroy()
    {
        if (m_HumTexture != null)                                         // 17583
            FreeAndNil(ref m_HumTexture);                                 // 17584

        if (m_HumEffTexture != null)                                      // 17586
            FreeAndNil(ref m_HumEffTexture);                              // 17587

        if (m_WeaponEffTexture != null)                                   // 17589
            FreeAndNil(ref m_WeaponEffTexture);                           // 17590

        base.Destroy();                                                   // 17592
    }

    /// <summary>
    /// `TStatuaryNpcActor.DrawChr`（**17595-17642，47 行**）1:1。
    ///
    /// <para><b>流程</b>：</para>
    /// <list type="number">
    /// <item><b>17600</b>：先 <c>inherited;</c>（<c>TNpcActor.DrawChr</c> 10297）—— **必须在前**。</item>
    /// <item><b>17603-17606</b>：<c>FIsFinalized</c> 的**一次性重放**：
    ///   置回 False 后**重新</c> <c>SetEffigyState(m_IsGrayShow, m_boScaleShow, m_nEffigyState, m_nEffigyOffset)</c>
    ///   （原文注释：修复全屏模式切到桌面再切回游戏时天下第一有黑块 chongchong 2016-04-07）。
    ///   ★ 注意重放用的 <c>IsGrayShow</c>/<c>IsScaleShow</c> 是**当前字段值**（不是初次调用时的实参）——
    ///   因为 17700/17698 会把实参写回字段，二者在正常路径下相等；但若中途被改过，重放会用它。
    ///   逐字保留这一细节。</item>
    /// <item><b>17608-17641</b>：<c>m_boShowStatuary</c> 时按 <c>m_boScaleShow</c> 二分支画三层
    ///   （<c>m_WeaponEffTexture</c> → <c>m_HumEffTexture</c> → <c>m_HumTexture</c>，**顺序即语义**）；
    ///   放大支用 <c>StretchDraw(DR, SR, ...)</c>，非放大支用 <c>DrawBlend(DR.Left, DR.Top, SR, ...)</c>
    ///   画前两层、<c>Draw(DR.Left, DR.Top, SR, m_HumTexture)</c> 画主体
    ///   —— **主体那一次用的是 <c>Draw</c> 而非 <c>DrawBlend</c>**，逐字保留。</item>
    /// </list>
    ///
    /// <para><b>几何（17609-17611）</b>：<c>SR := Rect(0, 0, m_HumTexture.Width, m_HumTexture.Height)</c>；
    /// <c>DR := SR</c> 后 <c>OffsetRect(DR, dx + m_nShiftX - 200, dy + m_nShiftY - 237)</c>；
    /// 放大支再 <c>InflateRect(DR, 20, 20)</c>（**在宽高两侧各扩 20**，故目标比源大 40）。</para>
    ///
    /// <para><b>17609 直接解引用 <c>m_HumTexture</c></b> —— 原文此处**不判 nil**（
    /// 与 17624/17637 的两处判 nil 不同）。headless 下若纹理未创建则尺寸取 (0,0)，
    /// 绘制操作仍按几何公式产生，不静默跳过（见 <see cref="ActorNpcEnv.TextureSizeFn"/>）。</para>
    /// </summary>
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
    {
        base.DrawChr(dx, dy, blend, boFlag);                              // 17600

        if (FIsFinalized)                                                 // 17603
        {
            FIsFinalized = false;                                         // 17604
            SetEffigyState(m_IsGrayShow, m_boScaleShow, m_nEffigyState, m_nEffigyOffset);   // 17605
        }

        if (m_boShowStatuary)                                             // 17608
        {
            var (texW, texH) = ActorNpcEnv.TextureSizeFn(m_HumTexture);   // 17609：原文不判 nil
            int srcLeft = 0, srcTop = 0, srcRight = texW, srcBottom = texH;
            int destLeft = srcLeft + dx + m_nShiftX - 200;                // 17611
            int destTop = srcTop + dy + m_nShiftY - 237;

            if (m_boScaleShow)                                            // 17613
            {
                destLeft -= 20;                                           // InflateRect(DR, 20, 20)
                destTop -= 20;
                int destW = (srcRight - srcLeft) + 40;
                int destH = (srcBottom - srcTop) + 40;

                if (m_WeaponEffTexture != null)                           // 17616
                    EmitStretch(destLeft, destTop, destW, destH, m_WeaponEffTexture, "WeaponEff", 17617);

                if (m_HumEffTexture != null)                              // 17620
                    EmitStretch(destLeft, destTop, destW, destH, m_HumEffTexture, "HumEff", 17621);

                if (m_HumTexture != null)                                 // 17624
                    EmitStretch(destLeft, destTop, destW, destH, m_HumTexture, "Hum", 17625);
            }
            else
            {
                if (m_WeaponEffTexture != null)                           // 17629
                    EmitBlend(destLeft, destTop, m_WeaponEffTexture, "WeaponEff", 17630);

                if (m_HumEffTexture != null)                              // 17633
                    EmitBlend(destLeft, destTop, m_HumEffTexture, "HumEff", 17634);

                if (m_HumTexture != null)                                 // 17637
                    EmitDraw(destLeft, destTop, m_HumTexture, "Hum", 17638);
            }
        }
    }

    private static void EmitStretch(int x, int y, int w, int h, object? surface, string layer, int line)
        => ActorNpcEnv.NpcDrawFn(new NpcDrawOp(NpcDrawKind.StretchDraw, x, y, surface, layer, line, w, h));

    private static void EmitBlend(int x, int y, object? surface, string layer, int line)
        => ActorNpcEnv.NpcDrawFn(new NpcDrawOp(NpcDrawKind.DrawBlend, x, y, surface, layer, line));

    private static void EmitDraw(int x, int y, object? surface, string layer, int line)
        => ActorNpcEnv.NpcDrawFn(new NpcDrawOp(NpcDrawKind.Draw, x, y, surface, layer, line));

    /// <summary>
    /// `TStatuaryNpcActor.Finalize`（**17644-17648**）1:1：先 <c>inherited</c>，
    /// **再**置 <c>FIsFinalized := True</c>（顺序即语义 —— 标记必须最后立，
    /// 否则基类 <c>Finalize</c> 里的释放逻辑若触发 <c>DrawChr</c> 会误判为"已终结"而重放）。
    /// </summary>
    public override void Finalize()
    {
        base.Finalize();                                                  // 17646
        FIsFinalized = true;                                              // 17647
    }

    /// <summary>
    /// `TStatuaryNpcActor.GetDefaultFrame`（**17650-17653**）1:1：**无视 <c>wmode</c> 恒返回 0**
    /// （也不像 <c>TNpcActor.GetDefaultFrame</c> 那样写 <c>m_dwFrameTime</c>）。
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        return 0;                                                         // 17652
    }

    /// <summary>
    /// `TStatuaryNpcActor.LoadSurface`（**17655-17665**）1:1。
    ///
    /// <para><b>★ 与 <c>TNpcActor.LoadSurface</c> 的三处结构差异（照抄）</b>：</para>
    /// <list type="bullet">
    /// <item>17657-17659：打点、清 <c>m_boLoadSurface</c>、清 <c>m_EffSurface</c>，
    ///   但**不清 <c>m_KeepSurface</c>**（<c>TNpcActor</c> 的 10496 会清）；</item>
    /// <item>清完立刻取图，**不转调 <c>inherited LoadSurface</c>**；</item>
    /// <item>图库是 <c>g_WNewopUIImages</c>（不是 <c>g_WNpcImgImages</c>），
    ///   且图号是 <c>m_nBodyOffset + m_nCurrentFrame</c>，**没有 <c>Indexs[]</c> 层**。</item>
    /// </list>
    ///
    /// <para><b>17660-17663 的取值分支是 <c>m_IsGrayShow</c> 而不是 <c>m_ColorEffect</c></b>
    /// —— 与 <c>TNpcActor.LoadSurface</c> 全篇按 <c>ceGrayScale/ceBright</c> 分流**完全不同**。
    /// 写成 <c>m_ColorEffect</c> 会让雕像永远取原图。</para>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        m_dwLoadSurfaceTime = ActorNpcEnv.MyGetTickCountFn();             // 17657
        m_boLoadSurface = false;                                          // 17658
        m_EffSurface = null;                                              // 17659

        var fetch = ActorNpcEnv.FetchNewopUiImageFn(m_nBodyOffset + m_nCurrentFrame, m_IsGrayShow);   // 17660-17663
        m_BodySurface = fetch.Texture;
        m_nPx = fetch.OffsetX;
        m_nPy = fetch.OffsetY;
    }

    /// <summary>
    /// `TStatuaryNpcActor.SetEffigyState`（**17667-18008，341 行**）1:1。
    ///
    /// <para><b>17669-17677 是两个内嵌函数</b> <c>HiLong</c>/<c>LoLong</c>
    /// （<c>Large_Integer(N).HighPart/LowPart</c>）—— 即 Int64 的高/低 32 位，逐字对应
    /// <c>(int)(v &gt;&gt; 32)</c> / <c>(int)v</c>。</para>
    ///
    /// <para><b>流程</b>：</para>
    /// <list type="number">
    /// <item><b>17690-17699</b>：写 <c>m_nEffigyState</c>、把旧偏移转存 <c>m_nOldEffigyOffset</c>、
    ///   更新 <c>m_nEffigyOffset</c>、<c>m_nCurrentFrame &lt; 0</c> 时归零、
    ///   写回 <c>m_boScaleShow</c>/<c>m_IsGrayShow</c>。</item>
    /// <item><b>17701-17711</b>（<c>Value1 = 0</c>）：<c>m_boShowStatuary := False</c> 并释放三张纹理
    ///   （**注意此处用的是 <c>FreeAndNil</c>，与 17703 的 nil 判定**）；
    ///   17711 的 <c>end</c> 之后直接跳到 17917 的翅膀段 —— 即 **Value1=0 时仍会走 17917-18006 的
    ///   "翅膀/衣服/武器特效"段**（原文结构如此，不是提前 Exit）。</item>
    /// <item><b>17712-17915</b>（<c>Value1 &lt;&gt; 0</c>）：位段解包
    ///   （<c>Value1</c> 低 32 = Dress|Weapon，高 32 = Effect|Hair|Shield；
    ///   <c>Value2</c> 低 32 = DressEffIndex|WeaponEffIndex，高 32 = DressEffOffSet|WeaponEffOffSet）
    ///   → 打点、取主体图 → 释放并重建三张 800×800 纹理 → <c>nHairOffset</c> 大表
    ///   → 武器/身体/头发/盾牌依次 <c>CopyTexture</c>。</item>
    /// <item><b>17917-18006</b>：翅膀（<c>m_wEffect &gt;= 1000</c>）/ <c>m_wEffect = 50</c>（**空分支**，
    ///   原文整段被注释）/ <c>m_wEffect &lt;&gt; 0</c> 三分支，与衣服/武器特效的两段
    ///   <c>g_EffectImageList</c> 回退。</item>
    /// </list>
    ///
    /// <para><b>原文缺陷（照抄 + 锁死）</b>：</para>
    /// <list type="bullet">
    /// <item><b>17695</b>：<c>if m_nCurrentFrame &lt; 0 then m_nCurrentFrame := 0;</c>
    ///   —— 这会**改写实例帧号**，而 <c>CalcActorFrame</c> 刚把它设成 -1，故顺序敏感。</item>
    /// <item><b>17721</b>：<c>m_btSex := m_wDress mod 2;</c> —— 用**衣服**的奇偶定性别，
    ///   不是读 <c>m_btSex</c> 原值。</item>
    /// <item><b>17848-17897 的 <c>case m_btHair</c> 与 17765-17818 的 <c>case m_btHair</c> 区间相同，
    ///   但用的是**不同的表**</b>（后者算偏移，前者选出图集）—— 两处不可合并。</item>
    /// <item><b>17919 的 <c>m_wEffect &gt;= 1000</c> 用 <c>&gt;=</c></b>，而
    ///   <b>17937 的 <c>m_wEffect &lt;&gt; 0</c></b> 是宽松分支 —— 故 <c>m_wEffect in 1..999</c> 且 <c>&lt;&gt; 50</c>
    ///   会落到 <c>GetCachedImage((m_wEffect-1)*HUMANFRAME + idx)</c>。</item>
    /// <item><b>17904-17911</b>：<c>m_wShield &gt; 0</c> 才取盾牌，否则**显式 <c>D := nil</c>**
    ///   （把上一步的 <c>D</c> 清掉，防止复用）——逐字保留。</item>
    /// <item><b>17982-17999</b>：武器特效的 <c>else</c> 分支（<c>m_nWeaponEffectIndex</c> 越界时）
    ///   按 <c>m_wDBWeaponEffectOffSet</c> 的两段区间取图 —— 是**回退**而不是"不画"。</item>
    /// <item><b>18004</b>：武器特效 <c>CopyTexture</c> 带 <c>m_boScaleShow</c>（缩放），
    ///   而 17827/17842/17901/17913 的武器/身体/头发/盾牌**不带**缩放。</item>
    /// </list>
    /// </summary>
    public virtual void SetEffigyState(bool isGrayShow, bool isScaleShow, TFeature_New nEffigyState, int nOffset)
    {
        // 17690-17699
        m_nEffigyState = nEffigyState;

        m_nOldEffigyOffset = m_nEffigyOffset;
        m_nEffigyOffset = nOffset;

        if (m_nCurrentFrame < 0)
            m_nCurrentFrame = 0;                                          // 17696

        m_boScaleShow = isScaleShow;                                      // 17698
        m_IsGrayShow = isGrayShow;                                        // 17699

        if (m_nEffigyState.Value1 == 0)                                   // 17701
        {
            m_boShowStatuary = false;                                     // 17702
            if (m_HumTexture != null)
                FreeAndNil(ref m_HumTexture);                             // 17704

            if (m_HumEffTexture != null)
                FreeAndNil(ref m_HumEffTexture);                          // 17707

            if (m_WeaponEffTexture != null)
                FreeAndNil(ref m_WeaponEffTexture);                       // 17710
        }
        else
        {
            m_boShowStatuary = true;                                      // 17713

            // 17715-17735：Int64 位段解包
            int lInt = LoLong(nEffigyState.Value1);                       // 17715
            int hInt = HiLong(nEffigyState.Value1);                       // 17716

            m_wDress = LoWord(lInt);                                      // 17718
            m_wWeapon = HiWord(lInt);                                     // 17719

            m_btSex = (byte)(m_wDress % 2);                               // 17721

            m_wEffect = LoWord(hInt);                                     // 17723
            ushort w = HiWord(hInt);                                      // 17724
            m_btHair = LoByte(w);                                         // 17725
            m_wShield = HiByte(w);                                        // 17726

            lInt = LoLong(nEffigyState.Value2);                           // 17728
            hInt = HiLong(nEffigyState.Value2);                           // 17729

            m_nDressEffectIndex = (short)LoWord(lInt);                    // 17731
            m_nWeaponEffectIndex = (short)HiWord(lInt);                   // 17732

            m_wDressEffectOffSet = LoWord(hInt);                          // 17734
            m_wWeaponEffectOffSet = HiWord(hInt);                         // 17735

            m_dwLoadSurfaceTime = ActorNpcEnv.MyGetTickCountFn();         // 17737
            m_boLoadSurface = false;                                      // 17738
            m_BodySurface = null;                                         // 17739
            m_EffSurface = null;                                          // 17740

            // 17742-17745：主体图（**按 IsGrayShow 而非 m_ColorEffect**）
            var body = ActorNpcEnv.FetchNewopUiImageFn(m_nBodyOffset + m_nCurrentFrame, isGrayShow);
            m_BodySurface = body.Texture;
            m_nPx = body.OffsetX;
            m_nPy = body.OffsetY;

            // 17747-17757：三张纹理**先各自判 nil 再释放**
            if (m_HumTexture != null)
                FreeAndNil(ref m_HumTexture);                             // 17748

            if (m_HumEffTexture != null)
                FreeAndNil(ref m_HumEffTexture);                          // 17752

            if (m_WeaponEffTexture != null)
                FreeAndNil(ref m_WeaponEffTexture);                       // 17756

            // 17759-17761：重建三张 800×800
            m_HumTexture = ActorNpcEnv.CreateTexture800Fn();
            m_HumEffTexture = ActorNpcEnv.CreateTexture800Fn();
            m_WeaponEffTexture = ActorNpcEnv.CreateTexture800Fn();

            int nHairOffset = -1;                                         // 17763（HZQ 20230525）

            switch (m_btHair)                                             // 17765-17818
            {
                case >= 0 and <= 5:
                    if (m_btHair < 4)
                    {
                        switch (m_btSex)
                        {
                            case 0:
                                nHairOffset = m_btHair * 2 * ActorNpcEnv.HumanFrame;
                                break;
                            case 1:
                                nHairOffset = m_btHair > 0
                                    ? (m_btHair + 2) * ActorNpcEnv.HumanFrame
                                    : 0;
                                break;
                        }
                    }
                    else
                    {
                        switch (m_btHair)                                     // 17804：头盔
                        {
                            case 4: nHairOffset = 3600; break;
                            case 5: nHairOffset = 4800; break;
                            default: nHairOffset = -1; break;
                        }
                    }
                    break;

                // 修正发型扩展外观错误 chongchong 2013-11-21
                case >= 50 and <= 59:
                    nHairOffset = (m_btHair - 50) * ActorNpcEnv.HumanFrame;
                    break;
                case >= 60 and <= 69:
                    nHairOffset = (m_btHair - 60) * ActorNpcEnv.HumanFrame;
                    break;

                // 斗笠 1 - 8
                case >= 100 and <= 107:
                    nHairOffset = 3600 + (m_btHair - 100) * ActorNpcEnv.HumanFrame * 2
                        + m_btSex * ActorNpcEnv.HumanFrame;
                    break;

                // 斗笠 9, 10 hair3
                case >= 108 and <= 119:
                    nHairOffset = 0 + (m_btHair - 108) * ActorNpcEnv.HumanFrame * 2
                        + m_btSex * ActorNpcEnv.HumanFrame;
                    break;

                // 扩展斗笠 hair4
                case >= 120 and <= 129:
                    nHairOffset = 0 + (m_btHair - 120) * ActorNpcEnv.HumanFrame * 2
                        + m_btSex * ActorNpcEnv.HumanFrame;
                    break;

                // 扩展斗笠 hair5
                case >= 130 and <= 139:
                    nHairOffset = 0 + (m_btHair - 130) * ActorNpcEnv.HumanFrame * 2
                        + m_btSex * ActorNpcEnv.HumanFrame;
                    break;

                // 扩展斗笠 hair6
                case >= 140 and <= 149:
                    nHairOffset = 0 + (m_btHair - 140) * ActorNpcEnv.HumanFrame * 2
                        + m_btSex * ActorNpcEnv.HumanFrame;
                    break;
            }

            // 17820-17828：武器
            var fetch = ActorNpcEnv.WeaponImageFn(
                m_wWeapon, m_btSex, m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
            var d = fetch.Texture;
            if (d != null)
                ActorNpcEnv.CopyTextureFn(d, m_HumTexture, 200 + fetch.OffsetX, 210 + fetch.OffsetY, false);

            // 17830-17843：人物身体（**取不到或面积 <= 16 时回退到 wDress = 0**）
            fetch = ActorNpcEnv.HumImageFn(m_wDress, m_btSex, m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
            d = fetch.Texture;
            var (dw, dh) = ActorNpcEnv.TextureSizeFn(d);
            if (d == null || dw * dh <= 16)                               // 17835
            {
                fetch = ActorNpcEnv.HumImageFn(0, m_btSex, m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
                d = fetch.Texture;
            }
            if (d != null)
                ActorNpcEnv.CopyTextureFn(d, m_HumTexture, 200 + fetch.OffsetX, 210 + fetch.OffsetY, false);

            // 17845-17898：发型（case m_btHair 选表；17846 的 nHairOffset >= 0 门是**唯一**入口）
            if (nHairOffset >= 0)
            {
                int hairLib;
                switch (m_btHair)
                {
                    case >= 0 and <= 5: hairLib = 0; break;               // g_WHairImgImages
                    case >= 50 and <= 59: hairLib = 1; break;             // g_WHair10ImgImages
                    case >= 60 and <= 69: hairLib = 2; break;             // g_WHair11ImgImages
                    case >= 100 and <= 107: hairLib = 3; break;           // g_WHair2ImgImages
                    case >= 108 and <= 119: hairLib = 4; break;           // g_WHair3ImgImages
                    case >= 120 and <= 129: hairLib = 5; break;           // g_WHair4ImgImages
                    case >= 130 and <= 139: hairLib = 6; break;           // g_WHair5ImgImages
                    case >= 140 and <= 149: hairLib = 7; break;           // g_WHair6ImgImages
                    default: hairLib = -1; break;
                }

                if (hairLib >= 0)
                {
                    fetch = ActorNpcEnv.HairImageFn(hairLib,
                        nHairOffset + m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
                    d = fetch.Texture;
                    if (d != null)
                        ActorNpcEnv.CopyTextureFn(d, m_HumTexture,
                            200 + fetch.OffsetX, 210 + fetch.OffsetY, false);
                }
            }

            // 17904-17914：盾牌（m_wShield > 0 才取，否则**显式** D := nil）
            if (m_wShield > 0)
            {
                fetch = ActorNpcEnv.ShieldImageFn(
                    ActorNpcEnv.HumanFrame * (m_wShield - 1) + m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
                d = fetch.Texture;
            }
            else
            {
                d = null;                                                 // 17911
                fetch = default;
            }
            if (d != null)
                ActorNpcEnv.CopyTextureFn(d, m_HumTexture, 200 + fetch.OffsetX, 210 + fetch.OffsetY, false);
        }

        // ══════════════ 17917-18006：翅膀 / 衣服 / 武器特效（**两分支共用**） ══════════════
        object? d2 = null;
        NpcImageFetch fetch2 = default;

        if (m_wEffect >= 1000)                                            // 17919
        {
            fetch2 = ActorNpcEnv.HumEffectImageFn(
                m_wEffect, m_btSex, m_nCurrentFrame + m_nEffigyOffset, false, isGrayShow);
            d2 = fetch2.Texture;
        }
        else if (m_wEffect == 50)                                         // 17926
        {
            // 原文如此：整段（17927-17935）被注释，故此处**什么都不做**
        }
        else if (m_wEffect != 0)                                          // 17937
        {
            fetch2 = ActorNpcEnv.HumEffectCachedImageFn(
                (m_wEffect - 1) * ActorNpcEnv.HumanFrame + m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
            d2 = fetch2.Texture;
        }

        if (d2 != null)                                                   // 17944
        {
            ActorNpcEnv.CopyTextureFn(d2, m_HumEffTexture,
                200 + fetch2.OffsetX, 210 + fetch2.OffsetY, m_boScaleShow);
        }
        else
        {
            // 17947-17968：衣服特效回退
            int effectCount = ActorNpcEnv.EffectImageListCountFn();
            if (m_nDressEffectIndex >= 0 && m_nDressEffectIndex < effectCount)
            {
                fetch2 = ActorNpcEnv.FetchEffectListImageFn(m_nDressEffectIndex,
                    m_wDressEffectOffSet + m_nCurrentFrame + m_nEffigyOffset, TColorEffect.ceNone);
                d2 = fetch2.Texture;

                if (d2 != null)
                    ActorNpcEnv.CopyTextureFn(d2, m_HumEffTexture,
                        200 + fetch2.OffsetX, 210 + fetch2.OffsetY, m_boScaleShow);
            }
        }

        // 17970-18006：武器特效
        d2 = null;
        {
            int effectCount = ActorNpcEnv.EffectImageListCountFn();
            if (m_nWeaponEffectIndex >= 0 && m_nWeaponEffectIndex < effectCount)
            {
                fetch2 = ActorNpcEnv.FetchEffectListImageFn(m_nWeaponEffectIndex,
                    m_wWeaponEffectOffSet + m_nCurrentFrame + m_nEffigyOffset, TColorEffect.ceNone);
                d2 = fetch2.Texture;
            }
            else
            {
                // 17982-17999：武器特效扩展 piaoyun 2013-07-28
                switch (m_wDBWeaponEffectOffSet)
                {
                    case >= 1000 and <= 1015:                             // 绘制 DB 库扩展特效
                        fetch2 = ActorNpcEnv.HumEffectImageFn(m_wDBWeaponEffectOffSet, m_btSex,
                            m_nCurrentFrame + m_nEffigyOffset, true, isGrayShow);
                        d2 = fetch2.Texture;
                        break;
                    case >= 1025 and <= 2000:                             // 绘制扩展特效 WeaponEffect.wzl -- WeaponEffect5.wzl
                        fetch2 = ActorNpcEnv.WeaponEffectImageFn(m_wDBWeaponEffectOffSet, m_btSex,
                            m_nCurrentFrame + m_nEffigyOffset, isGrayShow);
                        d2 = fetch2.Texture;
                        break;
                }
            }
        }

        if (d2 != null)                                                   // 18004
            ActorNpcEnv.CopyTextureFn(d2, m_WeaponEffTexture,
                200 + fetch2.OffsetX, 210 + fetch2.OffsetY, m_boScaleShow);
    }

    // ──────────────────────────────────────────────────────────────────────
    // 17669-17677：HiLong / LoLong（Large_Integer 的高/低 32 位）
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`HiLong`（17669-17672）：<c>Large_Integer(N).HighPart</c> = 高 32 位。</summary>
    private static int HiLong(long n) => (int)(n >> 32);

    /// <summary>`LoLong`（17674-17677）：<c>Large_Integer(N).LowPart</c> = 低 32 位。</summary>
    private static int LoLong(long n) => (int)n;

    /// <summary>`LoWord`（Delphi Windows 单元）：低 16 位。</summary>
    private static ushort LoWord(int v) => (ushort)(v & 0xFFFF);

    /// <summary>`HiWord`（Delphi Windows 单元）：高 16 位。</summary>
    private static ushort HiWord(int v) => (ushort)((v >> 16) & 0xFFFF);

    /// <summary>`LoByte`（Delphi Windows 单元）：低 8 位。</summary>
    private static byte LoByte(ushort v) => (byte)(v & 0xFF);

    /// <summary>`HiByte`（Delphi Windows 单元）：高 8 位。</summary>
    private static byte HiByte(ushort v) => (byte)((v >> 8) & 0xFF);
}
