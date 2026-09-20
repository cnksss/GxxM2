using System;
using GXX.Core.Protocol;
// `TDoorState`（原文 HerbActor.pas:26）的托管落点在 Tail/HerbActor.cs 的 GXX.Client.Tail 命名空间下。
using TDoorState = GXX.Client.Tail.TDoorState;

namespace GXX.Client.Scenes;

// ════════════════════════════════════════════════════════════════════════════
// 车道 p7-client-actor-family：TCastleDoor / TWallStructure / TNewWallStructure
//                        （HerbActor.pas:76 / :94 / :111）
// ════════════════════════════════════════════════════════════════════════════
//
// 这三者都是 **class(TActor)**（不是 TKillingHerb），且都覆写 5 个虚方法：
//   CalcActorFrame / LoadSurface(Sender) / GetDefaultFrame / DrawChr / Run
// 另各有 Create / Finalize / ActionEnded(TNewWallStructure 无)。
//
// 它们的 LoadSurface / DrawChr 依赖 TTexture 与 g_WMonImages 图集下标，
// 故取图与绘制走 ActorFamilyHerbEnv 的接缝；**帧决策与地图标记（Run/bomarkpos）**
// 是纯逻辑，逐字落地。

// ────────────────────────────────────────────────────────────────────────────
// TCastleDoor（:76）
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TCastleDoor`（原文 `HerbActor.pas:76`，`class(TActor)`）。
/// <para>额外字段：<c>EffectSurface</c> / <c>ax,ay</c> / <c>oldunitx,oldunity</c> / <c>BoDoorOpen</c>。</para>
/// </summary>
public partial class TCastleDoor
{
    /// <summary>
    /// `TCastleDoor.Create`（**519-525**）1:1。
    /// <para>原文 <c>m_nDownDrawLevel := 1</c>（524，注释"层级设为 1，让门画在人物之下"）。</para>
    /// </summary>
    public TCastleDoor()
    {
        m_btDir = 0;                             // 522
        EffectSurface = null;                    // 523
        m_nDownDrawLevel = 1;                    // 524
    }

    /// <summary>`TCastleDoor.Finalize`（**527-531**）1:1。</summary>
    public override void Finalize()
    {
        base.Finalize();                         // 529
        EffectSurface = null;                    // 530
    }

    /// <summary>
    /// `TCastleDoor.ApplyDoorState(dstate)`（**533-560**）1:1。
    ///
    /// <para><b>顺序即语义（12 次 MarkCanWalk）</b>：</para>
    /// <list type="number">
    /// <item>537-539：先把 3 格**无条件**标记可走：<c>(x, y-2)</c> / <c>(x+1, y-1)</c> / <c>(x+1, y-2)</c>；</item>
    /// <item>540-543：<c>bowalk := (dstate &lt;&gt; dsClose)</c>；</item>
    /// <item>545-553：按 <c>bowalk</c> 统一设置**另外 9 格**（注意 <c>(0,-2)/(+1,-1)</c> 与第 1 步重复）；</item>
    /// <item>555-559：若 <c>dstate = dsOpen</c>，再把第 1 步那 3 格**重新标记为不可走**。</item>
    /// </list>
    /// <para><b>净效果</b>：<c>dsOpen</c> 时那 3 格不可走；<c>dsClose</c>/<c>dsBroken</c> 时可走。</para>
    /// </summary>
    private void ApplyDoorState(TDoorState dstate)
    {
        // 537-539：无条件可走的 3 格
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY - 2, true);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX + 1, m_nCurrY - 1, true);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX + 1, m_nCurrY - 2, true);

        bool bowalk;
        if (dstate == TDoorState.dsClose)         // 540-543
            bowalk = false;
        else
            bowalk = true;

        // 545-553：9 格统一设置
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY - 1, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY - 2, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX + 1, m_nCurrY - 1, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX + 1, m_nCurrY - 2, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX - 1, m_nCurrY - 1, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX - 1, m_nCurrY, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX - 1, m_nCurrY + 1, bowalk);
        ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX - 2, m_nCurrY, bowalk);

        if (dstate == TDoorState.dsOpen)          // 555-559：前 3 格改回不可走
        {
            ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY - 2, false);
            ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX + 1, m_nCurrY - 1, false);
            ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX + 1, m_nCurrY - 2, false);
        }
    }

    /// <summary>
    /// `TCastleDoor.LoadSurface(Sender)`（**562-581**）1:1（原文签名带 <c>Sender</c>）。
    /// <para>门自己**不装主体图**：571 起只装 <c>EffectSurface</c>（死亡特效），
    /// 图号 = <c>DOORDEATHEFFECTBASE + (m_nCurrentFrame - m_nStartFrame)</c>，且**整体被
    /// <c>if m_boUseEffect then</c> 包住**（573）。</para>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        if (m_nChangeAppr >= 0)                       // 566-569
        {
            base.LoadSurface(sender);                 // 567 `inherited;`（无参形态）
            return;                                   // 568
        }

        base.LoadSurface(sender);                     // 570 `inherited LoadSurface(Self);`
        EffectSurface = null;                         // 571

        if (m_boUseEffect)                            // 573-579
        {
            EffectSurface = ActorFamilyHerbEnv.FetchDoorEffectFn(
                (int)m_wAppearance,
                ActorFamilyHerbEnv.DoorDeathEffectBase + (m_nCurrentFrame - m_nStartFrame),   // 575-578
                ax, ay,
                ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
        }
    }

    /// <summary>
    /// `TCastleDoor.CalcActorFrame`（**583-671**）1:1。
    ///
    /// <para><b>与怪物族的三处结构差异</b>：</para>
    /// <list type="bullet">
    /// <item>599 清的是 <c>m_boUseEffect</c>（**不是** <c>m_boUseMagic</c>）；</item>
    /// <item>605：<c>m_sUserName := ' ';</c>（**一个空格**）；</item>
    /// <item>它有 <c>else</c> 分支，按 <c>m_btDir &lt; 3</c> 分"关门/开门"两态，
    ///   各写 <c>BoDoorOpen</c> / <c>m_boHoldPlace</c> 并调 <c>ApplyDoorState</c>。</item>
    /// </list>
    /// </summary>
    public override void CalcActorFrame()
    {
        if (m_nChangeAppr >= 0)                       // 587-598
        {
            if (m_nCurrentAction == TActorCore.SM_ATTACK01
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 1
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 2
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 3
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 4
                || m_nCurrentAction == TActorCore.SM_ATTACK06)
            {
                m_nCurrentAction = TActorCore.SM_HIT;
            }

            base.CalcActorFrame();                    // 596
            return;                                   // 597
        }

        m_boUseEffect = false;                        // 599
        m_nCurrentFrame = -1;                         // 600
        m_nBodyOffset = GetOffset(m_wAppearance);     // 602

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 603
        if (pmOpt == null)
            return;                                   // 604
        var pm = pmOpt.Value;

        m_sUserName = " ";                            // 605（一个空格）

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_NOWDEATH:              // 608-616
                m_nStartFrame = pm.ActDie.start;      // 609（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                Shift(m_btDir, 0, 0, 1);
                m_boUseEffect = true;                 // 614
                ApplyDoorState(TDoorState.dsBroken);  // 615
                break;

            case TActorCore.SM_STRUCK:                // 617-625
                m_nStartFrame = pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
                m_nEndFrame = m_nStartFrame + pm.ActStruck.frame - 1;
                m_dwFrameTime = (uint)m_dwStruckFrameTime;   // 620
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                Shift(m_btDir, 0, 0, 1);
                m_CustomMagicStatusEffect_Struck = 0;  // 624
                break;

            case TActorCore.SM_DIGUP:                 // 626-632（开门）
                m_nStartFrame = pm.ActAttack.start;   // 627（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime = pm.ActAttack.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                ApplyDoorState(TDoorState.dsOpen);    // 631
                break;

            case TActorCore.SM_DIGDOWN:               // 633-641（关门）
                m_nStartFrame = pm.ActCritical.start; // 634（★ 用 ActCritical）
                m_nEndFrame = m_nStartFrame + pm.ActCritical.frame - 1;
                m_dwFrameTime = pm.ActCritical.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                BoDoorOpen = false;                   // 638
                m_boHoldPlace = true;                 // 639
                ApplyDoorState(TDoorState.dsClose);   // 640
                break;

            case TActorCore.SM_DEATH:                 // 642-647
                m_nStartFrame = pm.ActDie.start + pm.ActDie.frame - 1;   // 643：末帧
                m_nEndFrame = m_nStartFrame;          // 644
                m_nDefFrameCount = 0;                 // 645
                ApplyDoorState(TDoorState.dsBroken);  // 646
                // ★ 注意本分支**不写** m_dwFrameTime / m_dwStartTime（原文如此）
                break;

            default:                                  // 648-669：else，按 m_btDir < 3 分两态
                if (m_btDir < 3)
                {
                    // 649-659：关门态
                    m_nStartFrame = pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
                    m_nEndFrame = m_nStartFrame;      // 651（★ 原文把 `+ frame - 1` 注释掉了）
                    m_dwFrameTime = pm.ActStand.ftime;
                    m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                    m_nDefFrameCount = 0;             // 654（★ 原文把 pm.ActStand.frame 注释掉了）
                    Shift(m_btDir, 0, 0, 1);
                    BoDoorOpen = false;               // 656
                    m_boHoldPlace = true;             // 657
                    ApplyDoorState(TDoorState.dsClose);   // 658
                }
                else
                {
                    // 660-668：开门态（★ 用 ActCritical，且**不写** m_dwFrameTime/m_dwStartTime）
                    m_nStartFrame = pm.ActCritical.start;   // 661
                    m_nEndFrame = m_nStartFrame;            // 662
                    m_nDefFrameCount = 0;                   // 663
                    BoDoorOpen = true;                      // 665
                    m_boHoldPlace = false;                  // 666
                    ApplyDoorState(TDoorState.dsOpen);      // 667
                }
                break;
        }
    }

    /// <summary>
    /// `TCastleDoor.GetDefaultFrame`（**673-699**）1:1。
    /// <para>三态：死亡 → <c>ActDie</c> 末帧且 <c>m_nDownDrawLevel := 2</c>；
    /// 开门 → <c>ActCritical.start</c> 且层级 2；关门 → <c>ActStand.start + Dir*(frame+skip)</c> 且层级 1。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        if (m_nChangeAppr >= 0)                       // 677-680
            return base.GetDefaultFrame(wmode);       // 678

        int result = 0;                               // 681 `Result := 0; // jacky`
        m_nBodyOffset = GetOffset(m_wAppearance);     // 682（★ 本方法比 603 多写一次 m_nBodyOffset）

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 683
        if (pmOpt == null)
            return result;                            // 684
        var pm = pmOpt.Value;

        if (m_boDeath)                                // 685-688
        {
            result = pm.ActDie.start + pm.ActDie.frame - 1;   // 686
            m_nDownDrawLevel = 2;                              // 687
        }
        else
        {
            if (BoDoorOpen)                           // 690-693
            {
                m_nDownDrawLevel = 2;                 // 691
                result = pm.ActCritical.start;        // 692（★ 方向乘法原文被注释掉）
            }
            else                                      // 694-697
            {
                m_nDownDrawLevel = 1;                 // 695
                result = pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);   // 696
            }
        }

        return result;
    }

    /// <summary>
    /// `TCastleDoor.ActionEnded`（**701-709**）1:1。
    /// <para>仅 <c>SM_DIGUP</c> 时把 <c>BoDoorOpen := True</c> / <c>m_boHoldPlace := False</c>；
    /// 707-708 的 <c>SM_DIGDOWN → DefaultMotion</c> 在原文**被注释掉**。</para>
    /// </summary>
    public override void ActionEnded()
    {
        if (m_nCurrentAction == TActorCore.SM_DIGUP)  // 703
        {
            BoDoorOpen = true;                        // 704
            m_boHoldPlace = false;                    // 705
        }
        // 707-708 `// if CurrentAction = SM_DIGDOWN then // DefaultMotion;`（原文注释）
    }

    /// <summary>
    /// `TCastleDoor.Run`（**711-724**）1:1。
    /// <para>只在**格坐标变化**时重刷可走标记（三态选 <c>dsBroken</c>/<c>dsOpen</c>/<c>dsClose</c>），
    /// 随后无条件把当前格坐标记进 <c>oldunitx/oldunity</c>，最后 <c>inherited Run</c>。</para>
    /// </summary>
    public override void Run(uint now)
    {
        int curUnitX = ActorFamilyHerbEnv.MapCurUnitXFn();
        int curUnitY = ActorFamilyHerbEnv.MapCurUnitYFn();

        if (curUnitX != oldunitx || curUnitY != oldunity)   // 713
        {
            if (m_boDeath)
                ApplyDoorState(TDoorState.dsBroken);       // 715
            else if (BoDoorOpen)
                ApplyDoorState(TDoorState.dsOpen);         // 717
            else
                ApplyDoorState(TDoorState.dsClose);        // 719
        }

        oldunitx = curUnitX;                                // 721
        oldunity = curUnitY;                                // 722
        base.Run(now);                                      // 723
    }

    /// <summary>
    /// `TCastleDoor.DrawChr`（**726-741**）1:1。
    /// <para>★ 注意 732 行：<c>inherited DrawChr(dx, dy, blend, **FALSE**);</c> ——
    /// 主体绘制**强制 blend = False**（与传入的 <c>blend</c> 无关），
    /// 随后才是"<c>m_boUseEffect and not blend</c>"时的门特效混合绘制。</para>
    /// </summary>
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
    {
        if (m_nChangeAppr >= 0)                       // 728-731
        {
            base.DrawChr(dx, dy, blend, boFlag);      // 729
            return;                                   // 730
        }

        base.DrawChr(dx, dy, blend, false);           // 732（★ 第四实参强制 FALSE）

        if (m_boUseEffect && !blend)                  // 733-740
        {
            if (EffectSurface != null)
            {
                ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                    SurfaceDrawKind.DrawBlend,
                    dx + ax + m_nShiftX,
                    dy + ay + m_nShiftY));
            }
        }
    }
}

// ────────────────────────────────────────────────────────────────────────────
// TWallStructure（:94） / TNewWallStructure（:111） —— 两类的帧决策逐字相同
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TWallStructure`（原文 `HerbActor.pas:94`，`class(TActor)`，0x62）。
/// </summary>
public partial class TWallStructure
{
    /// <summary>`TWallStructure.Create`（**746-754**）1:1。
    /// <para>★ 753 的 <c>// DownDrawLevel := 1;</c> 在原文**被注释掉** ⇒ 不设层级。</para>
    /// </summary>
    public TWallStructure()
    {
        m_btDir = 0;                             // 749
        EffectSurface = null;                    // 750
        BrokenSurface = null;                    // 751
        bomarkpos = false;                       // 752
        // 753 `// DownDrawLevel := 1;`（原文注释）
    }

    /// <summary>`TWallStructure.Finalize`（**756-761**）1:1 —— 释放两个纹理槽。</summary>
    public override void Finalize()
    {
        base.Finalize();                         // 758
        EffectSurface = null;                    // 759
        BrokenSurface = null;                    // 760
    }

    /// <summary>
    /// `TWallStructure.CalcActorFrame`（**763-830**）1:1。
    /// <para><b>与门/怪物族的差异</b>：本方法多写一个 <c>deathframe</c> 字段
    /// （786 起手清 0，三个死亡类分支各写 <c>pm.ActStand.start + m_btDir</c>），
    /// 供 832-912 的 <c>LoadSurface</c> 决定"用死亡帧还是走基类"。</para>
    /// </summary>
    public override void CalcActorFrame()
    {
        if (m_nChangeAppr >= 0)                       // 767-778
        {
            if (m_nCurrentAction == TActorCore.SM_ATTACK01
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 1
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 2
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 3
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 4
                || m_nCurrentAction == TActorCore.SM_ATTACK06)
            {
                m_nCurrentAction = TActorCore.SM_HIT;
            }

            base.CalcActorFrame();                    // 776
            return;                                   // 777
        }

        m_boUseEffect = false;                        // 779
        m_nCurrentFrame = -1;                         // 780
        m_nBodyOffset = GetOffset(m_wAppearance);     // 782

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 783
        if (pmOpt == null)
            return;                                   // 784
        var pm = pmOpt.Value;

        m_sUserName = " ";                            // 785（一个空格）
        deathframe = 0;                               // 786

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_NOWDEATH:              // 789-797
                m_nStartFrame = pm.ActDie.start;      // 790（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                deathframe = pm.ActStand.start + m_btDir;   // 794（★ 只加 Dir，不乘 frame+skip）
                Shift(m_btDir, 0, 0, 1);
                m_boUseEffect = true;                 // 796
                break;

            case TActorCore.SM_DEATH:                 // 798-803
                m_nStartFrame = pm.ActDie.start + pm.ActDie.frame - 1;   // 799：末帧
                m_nEndFrame = m_nStartFrame;          // 800
                m_nDefFrameCount = 0;                 // 801
                deathframe = pm.ActStand.start + m_btDir;   // 802
                // ★ 本分支不写 m_dwFrameTime/m_dwStartTime/m_boUseEffect（原文如此）
                break;

            case TActorCore.SM_DIGUP:                 // 804-811
                m_nStartFrame = pm.ActDie.start;      // 805（★ 用 ActDie，与怪物的 ActWalk 不同）
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                deathframe = pm.ActStand.start + m_btDir;   // 809
                m_boUseEffect = true;                 // 810
                break;

            default:                                  // 812-826：else
                m_nStartFrame = pm.ActStand.start + m_btDir;   // 813（★ 只加 Dir，乘法被注释掉）
                m_nEndFrame = m_nStartFrame;          // 814（★ `+ frame - 1` 被注释掉）
                m_dwFrameTime = pm.ActStand.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_nDefFrameCount = 0;                 // 817（★ 原文把 pm.ActStand.frame 注释掉）
                Shift(m_btDir, 0, 0, 1);
                m_boHoldPlace = true;                 // 819
                if (m_nCurrentAction == TActorCore.SM_TURN)   // 820-825
                {
                    deathframe = pm.ActStand.start + m_btDir; // 821
                    // 822 `// m_boHoldPlace := False;`（原文注释）
                    // 823 的 AddChatBoardString 调试语句（原文注释）
                }
                break;
        }
    }

    /// <summary>
    /// `TWallStructure.LoadSurface(Sender)`（**832-912**）1:1（原文签名带 <c>Sender</c>）。
    ///
    /// <para><b>三段结构</b>：</para>
    /// <list type="number">
    /// <item>840-846：打点、清标志、**三个槽全清**（`m_BodySurface`/`BrokenSurface`/`EffectSurface`）；</item>
    /// <item>851-866：<c>deathframe &gt; 0</c> 时**自己装主体图**（图号 = <c>GetOffset + deathframe</c>，
    ///   **无反向帧分支**），否则 <c>inherited LoadSurface(Self)</c>；</item>
    /// <item>868-888：装 <c>BrokenSurface</c> —— ★ 外观 <c>&gt;= 904</c> 时整段**被注释掉**
    ///   （什么都不装），否则图号 = <c>GetOffset + 8 + m_btDir</c>；</item>
    /// <item>890-910：<c>m_boUseEffect</c> 时装 <c>EffectSurface</c>，图号 =
    ///   <c>WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame)</c>；
    ///   ★ 外观 <c>&gt;= 904</c> 时同样是**空分支**（894-898 被注释掉）；</item>
    /// <item>911：<c>ActionChanged</c>。</item>
    /// </list>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        if (m_nChangeAppr >= 0)                       // 836-839
        {
            base.LoadSurface(sender);                 // 837 `inherited;`（无参形态）
            return;                                   // 838
        }

        m_dwLoadSurfaceTime = ActorFamilyEnv.MyGetTickCountFn();   // 840
        m_boLoadSurface = false;                                   // 841
        // 842 `// LoadNameSurface;`（原文注释）
        m_BodySurface = null;                                      // 843
        BrokenSurface = null;                                      // 845
        EffectSurface = null;                                      // 846

        if (deathframe > 0)                                        // 851-862
        {
            m_BodySurface = ActorFamilyHerbEnv.FetchWallBodyFn(
                (int)m_wAppearance,
                GetOffset(m_wAppearance) + deathframe,             // 856-859
                m_nPx, m_nPy,
                ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
        }
        else
        {
            base.LoadSurface(sender);                              // 865 `inherited LoadSurface(Self);`
        }

        // 868-888：BrokenSurface
        if (m_wAppearance >= 904)
        {
            // 869-877：★ 原文整段被 { } 注释掉 ⇒ 什么都不装
        }
        else
        {
            BrokenSurface = ActorFamilyHerbEnv.FetchWallBrokenFn(
                (int)m_wAppearance,
                GetOffset(m_wAppearance) + 8 + m_btDir,            // 882-885
                bx, by,
                ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
        }

        // 890-910：EffectSurface
        if (m_boUseEffect)                                         // 892
        {
            if (m_wAppearance >= 904)
            {
                // 894-898：★ 原文整段被 { } 注释掉 ⇒ 什么都不装
            }
            else
            {
                EffectSurface = ActorFamilyHerbEnv.FetchWallEffectFn(
                    (int)m_wAppearance,
                    ActorFamilyHerbEnv.WallLeftBrokenEffectBase + (m_nCurrentFrame - m_nStartFrame),   // 903-906
                    ax, ay,
                    ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
            }
        }

        ActionChanged();                                           // 911
    }

    /// <summary>
    /// `TWallStructure.GetDefaultFrame`（**914-927**）1:1。
    /// <para>★ 全族最简单：**不判死亡**、不写 <c>m_nDefFrameCount</c>，
    /// 直接 <c>ActStand.start + m_btDir</c>（方向乘法被注释掉）。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        if (m_nChangeAppr >= 0)                       // 918-921
            return base.GetDefaultFrame(wmode);       // 919

        int result = 0;                               // 922 `Result := 0; // jacky`
        m_nBodyOffset = GetOffset(m_wAppearance);     // 923

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 924
        if (pmOpt == null)
            return result;                            // 925

        return pmOpt.Value.ActStand.start + m_btDir;  // 926（★ 无死亡分支、无 m_nDefFrameCount）
    }

    /// <summary>
    /// `TWallStructure.DrawChr`（**929-950**）1:1。
    /// <para>与 <c>TCastleDoor.DrawChr</c> 的差异：这里 935 行传的是**原样的 <c>boFlag</c>**
    /// （门传的是强制 FALSE），且多装一个 <c>BrokenSurface</c> 的普通绘制（936-941）。</para>
    /// </summary>
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
    {
        if (m_nChangeAppr >= 0)                       // 931-934
        {
            base.DrawChr(dx, dy, blend, boFlag);      // 932
            return;                                   // 933
        }

        base.DrawChr(dx, dy, blend, boFlag);          // 935（★ 原样传 boFlag）

        if (BrokenSurface != null && !blend)          // 936-941
        {
            ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                SurfaceDrawKind.Draw,
                dx + bx + m_nShiftX,
                dy + by + m_nShiftY));
        }

        if (m_boUseEffect && !blend)                  // 942-949
        {
            if (EffectSurface != null)
            {
                ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                    SurfaceDrawKind.DrawBlend,
                    dx + ax + m_nShiftX,
                    dy + ay + m_nShiftY));
            }
        }
    }

    /// <summary>
    /// `TWallStructure.Run`（**952-968**）1:1。
    /// <para><c>bomarkpos</c> 是"本格已被标记为不可走"的记忆位：
    /// 死亡时**解除**标记（若曾标记过），存活时**标记**为不可走（若尚未标记）；
    /// 然后 <c>SetActorDrawLevel(Self, 0)</c>，最后 <c>inherited Run</c>。</para>
    /// </summary>
    public override void Run(uint now)
    {
        if (m_boDeath)                                // 954-959
        {
            if (bomarkpos)
            {
                ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY, true);   // 956
                bomarkpos = false;                                              // 957
            }
        }
        else                                          // 960-965
        {
            if (!bomarkpos)
            {
                ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY, false);  // 962
                bomarkpos = true;                                               // 963
            }
        }

        ActorFamilyHerbEnv.SetActorDrawLevelFn(this, 0);   // 966
        base.Run(now);                                     // 967
    }
}

/// <summary>
/// `TNewWallStructure`（原文 `HerbActor.pas:111`，`class(TActor)`，0x62）。
///
/// <para><b>与 <see cref="TWallStructure"/> 的关系</b>：帧决策（<c>CalcActorFrame</c> 990-1048、
/// <c>GetDefaultFrame</c> 1108-1121）**逐字几乎相同**，只有两处**实质差异**：</para>
/// <list type="number">
/// <item>1015-1024 的 <c>SM_NOWDEATH</c> 分支**没有** <c>m_boUseEffect := True</c>（原文 1023 被注释）；
///   同理 1030-1037 的 <c>SM_DIGUP</c> 分支也没有（1036 被注释）；</item>
/// <item>1038-1046 的 <c>else</c> 分支**没有** <c>SM_TURN</c> 时的 <c>deathframe</c> 特判
///   （<c>TWallStructure</c> 820-825 有）。</item>
/// </list>
/// <para>另 <c>LoadSurface</c>（1050-1106）用的是 <c>m_wAppearance - 904</c> 这个**固定的偏移**
/// （而 <c>TWallStructure</c> 用 <c>m_wAppearance</c> 本身并按 <c>&gt;= 904</c> 分支）。</para>
/// </summary>
public partial class TNewWallStructure
{
    /// <summary>`TNewWallStructure.Create`（**973-981**）1:1（与 746-754 逐字相同）。</summary>
    public TNewWallStructure()
    {
        m_btDir = 0;                             // 976
        EffectSurface = null;                    // 977
        BrokenSurface = null;                    // 978
        bomarkpos = false;                       // 979
        // 980 `// DownDrawLevel := 1;`（原文注释）
    }

    /// <summary>`TNewWallStructure.Finalize`（**983-988**）1:1。</summary>
    public override void Finalize()
    {
        base.Finalize();                         // 985
        EffectSurface = null;                    // 986
        BrokenSurface = null;                    // 987
    }

    /// <summary>`TNewWallStructure.CalcActorFrame`（**990-1048**）1:1。</summary>
    public override void CalcActorFrame()
    {
        if (m_nChangeAppr >= 0)                       // 994-1005
        {
            if (m_nCurrentAction == TActorCore.SM_ATTACK01
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 1
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 2
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 3
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 4
                || m_nCurrentAction == TActorCore.SM_ATTACK06)
            {
                m_nCurrentAction = TActorCore.SM_HIT;
            }

            base.CalcActorFrame();                    // 1003
            return;                                   // 1004
        }

        m_boUseEffect = false;                        // 1006
        m_nCurrentFrame = -1;                         // 1007
        m_nBodyOffset = GetOffset(m_wAppearance);     // 1009

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 1010
        if (pmOpt == null)
            return;                                   // 1011
        var pm = pmOpt.Value;

        m_sUserName = " ";                            // 1012
        deathframe = 0;                               // 1013

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_NOWDEATH:              // 1016-1024
                m_nStartFrame = pm.ActDie.start;
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                deathframe = pm.ActStand.start + m_btDir;   // 1021
                Shift(m_btDir, 0, 0, 1);
                // ★ 1023 `// m_boUseEffect := True;` 被注释掉（与 TWallStructure 796 的实质差异）
                break;

            case TActorCore.SM_DEATH:                 // 1025-1029
                m_nStartFrame = pm.ActDie.start + pm.ActDie.frame - 1;   // 1026
                m_nEndFrame = m_nStartFrame;          // 1027
                m_nDefFrameCount = 0;                 // 1028
                // ★ 本分支**不写 deathframe**（与 TWallStructure 802 的实质差异）
                break;

            case TActorCore.SM_DIGUP:                 // 1030-1037
                m_nStartFrame = pm.ActDie.start;
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                deathframe = pm.ActStand.start + m_btDir;   // 1035
                // ★ 1036 `// m_boUseEffect := True;` 被注释掉（与 TWallStructure 810 的实质差异）
                break;

            default:                                  // 1038-1046
                m_nStartFrame = pm.ActStand.start + m_btDir;   // 1039
                m_nEndFrame = m_nStartFrame;          // 1040
                m_dwFrameTime = pm.ActStand.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_nDefFrameCount = 0;                 // 1043
                Shift(m_btDir, 0, 0, 1);
                m_boHoldPlace = true;                 // 1045
                // ★ 本分支**没有** TWallStructure 820-825 的 SM_TURN → deathframe 特判
                break;
        }
    }

    /// <summary>
    /// `TNewWallStructure.LoadSurface(Sender)`（**1050-1106**）1:1（原文签名带 <c>Sender</c>）。
    /// <para>★ 与 <c>TWallStructure</c> 的实质差异：<c>BrokenSurface</c> 与 <c>EffectSurface</c>
    /// 的图库都取 <c>g_WMonImages.Images[m_wAppearance **- 904**]</c>（1067 与 1085），
    /// 且**没有** <c>m_wAppearance &gt;= 904</c> 的空分支。</para>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        if (m_nChangeAppr >= 0)                       // 1054-1057
        {
            base.LoadSurface(sender);                 // 1055 `inherited;`
            return;                                   // 1056
        }

        m_dwLoadSurfaceTime = ActorFamilyEnv.MyGetTickCountFn();   // 1058
        m_boLoadSurface = false;                                   // 1059
        m_BodySurface = null;                                      // 1062
        BrokenSurface = null;                                      // 1064
        EffectSurface = null;                                      // 1065

        if (deathframe > 0)                                        // 1068-1078
        {
            m_BodySurface = ActorFamilyHerbEnv.FetchWallBodyFn(
                (int)m_wAppearance,
                GetOffset(m_wAppearance) + deathframe,             // 1073-1076
                m_nPx, m_nPy,
                ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
        }
        else
        {
            base.LoadSurface(sender);                              // 1082 `inherited LoadSurface(Self);`
        }

        // 1085-1104：BrokenSurface 与 EffectSurface（同用 m_wAppearance - 904 的图库）
        BrokenSurface = ActorFamilyHerbEnv.FetchWallBrokenFn(
            (int)m_wAppearance - 904,                              // 1085
            GetOffset(m_wAppearance - 904) + 8 + m_btDir,          // 1089-1092
            bx, by,
            ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));

        if (m_boUseEffect)                                         // 1095-1103
        {
            EffectSurface = ActorFamilyHerbEnv.FetchWallEffectFn(
                (int)m_wAppearance - 904,                          // 1085 的 mimg 沿用
                ActorFamilyHerbEnv.WallLeftBrokenEffectBase + (m_nCurrentFrame - m_nStartFrame),   // 1098-1101
                ax, ay,
                ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
        }

        ActionChanged();                                           // 1105
    }

    /// <summary>`TNewWallStructure.GetDefaultFrame`（**1108-1121**）1:1（与 914-927 逐字相同）。</summary>
    public override int GetDefaultFrame(bool wmode)
    {
        if (m_nChangeAppr >= 0)                       // 1112-1115
            return base.GetDefaultFrame(wmode);       // 1113

        int result = 0;                               // 1116
        m_nBodyOffset = GetOffset(m_wAppearance);     // 1117

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 1118
        if (pmOpt == null)
            return result;                            // 1119

        return pmOpt.Value.ActStand.start + m_btDir;  // 1120
    }

    /// <summary>`TNewWallStructure.DrawChr`（**1123-1144**）1:1（与 929-950 逐字相同）。</summary>
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
    {
        if (m_nChangeAppr >= 0)                       // 1125-1128
        {
            base.DrawChr(dx, dy, blend, boFlag);      // 1126
            return;                                   // 1127
        }

        base.DrawChr(dx, dy, blend, boFlag);          // 1129

        if (BrokenSurface != null && !blend)          // 1130-1135
        {
            ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                SurfaceDrawKind.Draw,
                dx + bx + m_nShiftX,
                dy + by + m_nShiftY));
        }

        if (m_boUseEffect && !blend)                  // 1136-1143
        {
            if (EffectSurface != null)
            {
                ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                    SurfaceDrawKind.DrawBlend,
                    dx + ax + m_nShiftX,
                    dy + ay + m_nShiftY));
            }
        }
    }

    /// <summary>`TNewWallStructure.Run`（**1146-1162**）1:1（与 952-968 逐字相同）。</summary>
    public override void Run(uint now)
    {
        if (m_boDeath)                                // 1148-1153
        {
            if (bomarkpos)
            {
                ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY, true);   // 1150
                bomarkpos = false;                                              // 1151
            }
        }
        else                                          // 1154-1159
        {
            if (!bomarkpos)
            {
                ActorFamilyHerbEnv.MapMarkCanWalkFn(m_nCurrX, m_nCurrY, false);  // 1156
                bomarkpos = true;                                               // 1157
            }
        }

        ActorFamilyHerbEnv.SetActorDrawLevelFn(this, 0);   // 1160
        base.Run(now);                                     // 1161
    }
}
