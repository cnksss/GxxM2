using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// **车道 `p17-client-actor`**：`Actor.pas` 的 <c>TNpcActor</c>（**9936-11122，10 条实现**）1:1。
///
/// <para><b>★ 本文件是"空壳 → 1:1"的一处迁移</b>：<c>TNpcActor</c> 原先只是
/// <c>PlaySceneNewActor.cs:536</c> 的一行空壳
/// （<c>public class TNpcActor : TActor { public override string ActorClass =&gt; "TNpcActor"; }</c>）。
/// 该壳的来源是 <c>PlayScn.pas</c> 的 <c>wRaceImg</c> 分派表**照抄类名**，
/// 与 <c>Actor.pas</c> 的 <c>TNpcActor</c> 只是**同名**（详见 <c>docs/并行报告-p12-e2only-review.md</c> §5.2/§5.3）。
/// 本车道把类声明**移出** <c>PlaySceneNewActor.cs</c>，1:1 实现落在此处（见报告「空壳迁移清单」）。</para>
///
/// <para><b>虚分派</b>：原文 10 条**全部**是 <c>override</c>（声明 1884-1901）。
/// 托管侧对应槽位：<c>TActorCore.CalcActorFrame()</c>（virtual，ActorCore.cs:250）、
/// <c>TActorCore.Run(uint)</c>（virtual，ActorCore.cs:344）、
/// <c>TActorCore.GetDefaultFrame(bool)</c>（virtual，ActorMotion.cs:192）、
/// <c>TActor.LoadSurface(object?)</c>（virtual，ActorFamilyBase.cs:64）、
/// <c>TActor.DrawChr(int,int,bool,bool)</c>（virtual，ActorFamilyBase.cs:94）、
/// <c>TActorCore.DrawEff(int,int)</c>（virtual，ActorFamilyHerbEnv.cs:238）、
/// <c>TActorCore.Finalize()</c>（virtual，ActorFamilyHerbEnv.cs:195）。
/// 另两个本车道**新增**的虚槽位：<c>CheckLoadUserName()</c> / <c>Initialize()</c>（见 <c>ActorNpcActor</c> 尾部）。</para>
///
/// <para><b>已登记的接缝（非偷工，全部有原文对应分支）</b>：取图、配置查找、<c>GameCanvas</c> 绘制、
/// <c>PlayScene.LoadSurface</c> 请求、<c>g_PlaySound</c> —— 见 <see cref="ActorNpcEnv"/> 逐条说明。</para>
/// </summary>
public partial class TNpcActor : TActor
{
    /// <summary>类名（原文无此成员；迁移前 <c>PlaySceneNewActor.cs:536</c> 空壳上的原值，逐字保留）。</summary>
    public override string ActorClass => "TNpcActor";

    /// <summary>原文 1898：`m_EffSurface := nil;`</summary>
    // 字段 m_EffSurface 已在 ActorNpcEnv.cs 的 TActorCore 扩展里声明。

    /// <summary>
    /// `TNpcActor.CalcActorFrame`（**9936-10229，294 行**）1:1。
    ///
    /// <para><b>顺序即语义</b>：</para>
    /// <list type="number">
    /// <item><b>9943-9946</b>：清 <c>m_boUseMagic</c>、<c>m_nCurrentFrame := -1</c>；<c>NpcDirAction := nil</c>。</item>
    /// <item><b>9948-9962</b>（<c>m_wAppearance &gt;= 10000</c> 自定义 NPC）：
    ///   <c>m_nBodyOffset := 0</c> → <c>pm := GetRaceByPM(...)</c> → **<c>pm = nil 则 Exit</c>** →
    ///   <c>NpcConfig := GetCustomNpcConfig(...)</c>；非 nil 时 <c>wDirCount &gt; 1</c> 才取模，
    ///   否则 <c>m_btDir := 0</c>；取 <c>@NpcConfig.Actions[m_btDir]</c>。</item>
    /// <item><b>9963-9981</b>（普通外观）：<c>GetNpcOffset</c> → <c>GetRaceByPM</c>（**同样 nil 则 Exit**）→
    ///   外观 244/245 置 <c>m_boUseEffect</c> → 外观不在 246..272 时 <c>m_btDir mod 3</c>，
    ///   且命中 54..59/70..75/81..84/90..92/94..101/211..225/245 ⇒ <c>m_btDir := 0</c>。</item>
    /// <item><b>9983-10228</b>：四标签 <c>case</c>（`SM_TURN` / `SM_HIT` / `SM_DIGUP` / `SM_WALK`），
    ///   **无 else**。前三个分支在 <c>NpcDirAction &lt;&gt; nil</c> 时**各自 <c>Exit</c>**（不是 Break）——
    ///   即自定义 NPC 的 TURN/HIT/WALK 走动作表后就**不再执行**后面的全局表逻辑。</item>
    /// </list>
    ///
    /// <para><b>原文缺陷（照抄 + 锁死）</b>：</para>
    /// <list type="bullet">
    /// <item><b>10031-10032</b>：外观 42..47 的 <c>SM_TURN</c> 分支把
    ///   <c>m_nStartFrame := 20; m_nEndFrame := 10;</c> —— **StartFrame &gt; EndFrame**（倒置）。
    ///   照抄，见测试 `CalcActorFrame_Appr42to47TurnHasInvertedFrameRange`。</item>
    /// <item><b>10160-10162</b>：<c>SM_HIT</c> 默认支里
    ///   `if (pm.ActAttack.frame = 0) and (m_wAppearance &gt;= 226) and (m_wAppearance &lt;= 272)` 的
    ///   **then 体是空的**（原文留白），走 else 才设帧。照抄为空分支。</item>
    /// <item><b>10130</b>：外观 84 的判据是
    ///   `(m_nStartFrame &lt;= 0) or (m_nStartFrame &gt;= pm.ActCritical.start + ...)` ——
    ///   是 `&lt;=0` 而**非** `<0`，故 <c>m_nStartFrame = 0</c> 也算"未开始"走攻击帧。
    ///   另注意该 if 的 **else** 才是 ActCritical，即"已开始"反而走暴击帧。</item>
    /// <item><b>10020-10027</b>：外观 33/34 的 <c>SM_TURN</c> 在**设完帧之后**才置特效 30..39。</item>
    /// </list>
    /// </summary>
    public override void CalcActorFrame()
    {
        // 9943-9946
        m_boUseMagic = false;
        m_nCurrentFrame = -1;

        TNpcDirAction? npcDirAction = null;
        TClientCustomNpcConfig? npcConfig = null;
        TMonsterAction? pm;

        if (m_wAppearance >= 10000)
        {
            m_nBodyOffset = 0;                                            // 9949

            // 9952：原文自带 {$MESSAGE HINT}：这两行是为 m_WAppearance>=10000 补的 pm 赋值与空值判断
            pm = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);
            if (pm == null)
                return;                                                   // 9953

            npcConfig = ActorNpcEnv.CustomNpcConfigLookupFn(m_wAppearance);   // 9955
            if (npcConfig != null)
            {
                if (npcConfig.Value.wDirCount > 1)                        // 9957
                    m_btDir = (byte)(m_btDir % npcConfig.Value.wDirCount);
                else
                    m_btDir = 0;                                          // 9960
                npcDirAction = npcConfig.Value.Actions[m_btDir];           // 9961
            }
        }
        else
        {
            m_nBodyOffset = ActorOffsets.GetNpcOffset(m_wAppearance);      // 9964
            pm = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);
            if (pm == null)
                return;                                                   // 9966

            if (m_wAppearance == 244)
                m_boUseEffect = true;                                     // 9969
            else if (m_wAppearance == 245)
                m_boUseEffect = true;                                     // 9971

            // 9976-9980：npc4 全是 8 方向 chongchong 2015-04-18
            if (m_wAppearance < 246 || m_wAppearance > 272)
            {
                m_btDir = (byte)(m_btDir % 3);
                if (m_wAppearance is >= 54 and <= 59 or >= 70 and <= 75 or >= 81 and <= 84
                    or >= 90 and <= 92 or >= 94 and <= 101 or >= 211 and <= 225 or 245)
                    m_btDir = 0;
            }
        }

        var pa = pm.Value;

        switch (m_nCurrentAction)
        {
            case SM_TURN:                                                 // 9984-10091
            {
                if (npcDirAction != null)
                {
                    var nd = npcDirAction.Value;
                    m_nStartFrame = nd.Std_Index;
                    m_nEndFrame = nd.Std_Index + nd.Std_Count - 1;
                    m_dwFrameTime = nd.Std_Time;
                    m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                    m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                    m_nDefFrameCount = nd.Std_Count;
                    Shift(m_btDir, 0, 0, 1);
                    return;                                               // 10010：Exit（非 Break）
                }

                m_nStartFrame = pa.ActStand.start + m_btDir * (pa.ActStand.frame + pa.ActStand.skip);
                m_nEndFrame = m_nStartFrame + pa.ActStand.frame - 1;
                m_dwFrameTime = pa.ActStand.ftime;
                m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                m_nDefFrameCount = pa.ActStand.frame;
                Shift(m_btDir, 0, 0, 1);

                if (m_wAppearance == 33 || m_wAppearance == 34)           // 10020-10027
                {
                    m_boUseEffect = true;
                    m_nEffectStart = 30;
                    m_nEffectFrame = 30;
                    m_nEffectEnd = 39;
                    m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                    m_dwEffectFrameTime = 300;
                }
                else
                {
                    switch (m_wAppearance)                                // 10029-10088
                    {
                        case >= 42 and <= 47:
                            // 原文如此（10031-10032）：StartFrame=20 > EndFrame=10，**倒置**
                            m_nStartFrame = 20;
                            m_nEndFrame = 10;
                            m_boUseEffect = true;
                            m_nEffectStart = 0;
                            m_nEffectFrame = 0;
                            m_nEffectEnd = 19;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 100;
                            break;
                        case 51:
                            m_boUseEffect = true;
                            m_nEffectStart = 60;
                            m_nEffectFrame = m_nEffectStart;
                            m_nEffectEnd = m_nEffectStart + 7;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 500;
                            break;
                        case 100:
                            m_boUseEffect = true;
                            m_nEffectStart = 10;
                            m_nEffectFrame = m_nEffectStart;
                            m_nEffectEnd = m_nEffectStart + 11;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 100;
                            break;
                        case >= 217 and <= 219:
                            m_boUseEffect = true;
                            m_nEffectStart = 10;
                            m_nEffectFrame = m_nEffectStart;
                            m_nEffectEnd = m_nEffectStart + 16 - 1;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 150;
                            break;
                        case 221:
                            m_boUseEffect = true;
                            m_nEffectStart = 20;
                            m_nEffectFrame = m_nEffectStart;
                            m_nEffectEnd = m_nEffectStart + 9 - 1;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 250;
                            break;
                        case 222:
                            m_boUseEffect = true;
                            m_nEffectStart = 10;
                            m_nEffectFrame = m_nEffectStart;
                            m_nEffectEnd = m_nEffectStart + 9 - 1;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 250;
                            break;
                        case 224:
                            m_boUseEffect = true;
                            m_nEffectStart = 10;
                            m_nEffectFrame = m_nEffectStart;
                            m_nEffectEnd = m_nEffectStart + 16 - 1;
                            m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_dwEffectFrameTime = 150;
                            break;
                    }
                }
                break;
            }

            case SM_HIT:                                                  // 10092-10180
            {
                if (npcDirAction != null)
                {
                    var nd = npcDirAction.Value;
                    m_nStartFrame = nd.Act_Index;
                    m_nEndFrame = nd.Act_Index + nd.Act_Count - 1;
                    m_dwFrameTime = nd.Act_Time;
                    m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                    m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                    m_nDefFrameCount = nd.Act_Count;
                    Shift(m_btDir, 0, 0, 1);
                    return;                                               // 10118：Exit（非 Break）
                }

                switch (m_wAppearance)                                    // 10121-10179
                {
                    case 33:
                    case 34:
                    case 52:
                        // 10122-10128：注意**不设 m_dwFrameTime**（保留上一动作的值）——原文如此
                        m_nStartFrame = pa.ActStand.start + m_btDir * (pa.ActStand.frame + pa.ActStand.skip);
                        m_nEndFrame = m_nStartFrame + pa.ActStand.frame - 1;
                        m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                        m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                        m_nDefFrameCount = pa.ActStand.frame;
                        break;

                    case 84:
                        // 10130：判据是 `<= 0`（含 0）—— 已开始则走 ActCritical
                        if (m_nStartFrame <= 0
                            || m_nStartFrame >= pa.ActCritical.start + m_btDir * (pa.ActCritical.frame + pa.ActCritical.skip))
                        {
                            m_nStartFrame = pa.ActAttack.start + m_btDir * (pa.ActAttack.frame + pa.ActAttack.skip);
                            m_nEndFrame = m_nStartFrame + pa.ActAttack.frame - 1;
                            m_dwFrameTime = pa.ActAttack.ftime;
                            m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                        }
                        else
                        {
                            m_nStartFrame = pa.ActCritical.start + m_btDir * (pa.ActCritical.frame + pa.ActCritical.skip);
                            m_nEndFrame = m_nStartFrame + pa.ActCritical.frame - 1;
                            m_dwFrameTime = pa.ActCritical.ftime;
                            m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                        }
                        break;

                    case 210:
                        // 10147：用 **(frame+skip) × 1** 而非 `m_btDir × (frame+skip)` —— 原文如此
                        m_nStartFrame = pa.ActAttack.start + (pa.ActAttack.frame + pa.ActAttack.skip);
                        m_nEndFrame = m_nStartFrame + pa.ActAttack.frame - 1;
                        m_dwFrameTime = pa.ActAttack.ftime;
                        m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                        m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                        break;

                    default:
                        // 10160-10162：then 体**空**（原文留白）
                        if (pa.ActAttack.frame == 0 && m_wAppearance >= 226 && m_wAppearance <= 272)
                        {
                            // 原文如此：空分支
                        }
                        else
                        {
                            m_nStartFrame = pa.ActAttack.start + m_btDir * (pa.ActAttack.frame + pa.ActAttack.skip);
                            m_nEndFrame = m_nStartFrame + pa.ActAttack.frame - 1;
                            m_dwFrameTime = pa.ActAttack.ftime;
                            m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                            m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                            if (m_wAppearance == 51)
                            {
                                m_boUseEffect = true;
                                m_nEffectStart = 60;
                                m_nEffectFrame = m_nEffectStart;
                                m_nEffectEnd = m_nEffectStart + 7;
                                m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                                m_dwEffectFrameTime = 500;
                            }
                        }
                        break;
                }
                break;
            }

            case SM_DIGUP:                                                // 10181-10195
                if (m_wAppearance == 52)
                {
                    m_bo248 = true;
                    m_dwUseEffectTick = ActorNpcEnv.TimeGetTimeFn() + 23000;
                    // 10186 `Randomize` 对 headless 无对象语义（无全局种子状态需要重置），逐字略
                    ActorNpcEnv.PlaySoundByIdFn(ActorNpcEnv.RandomFn(7) + 146);
                    m_boUseEffect = true;
                    m_nEffectStart = 60;
                    m_nEffectFrame = m_nEffectStart;
                    m_nEffectEnd = m_nEffectStart + 11;
                    m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();
                    m_dwEffectFrameTime = 100;
                }
                break;

            case SM_WALK:                                                 // 10196-10227
                if (npcDirAction != null)
                {
                    var nd = npcDirAction.Value;
                    m_nStartFrame = nd.Act_Index;
                    m_nEndFrame = nd.Act_Index + nd.Act_Count - 1;
                    m_dwFrameTime = nd.Act_Time;
                    m_dwStartTime = ActorNpcEnv.TimeGetTimeFn();
                    m_StartCounter = ActorNpcEnv.TimeGetTimeFn();
                    m_nDefFrameCount = nd.Act_Count;
                    m_nMaxTick = 1;
                    m_nCurTick = 0;
                    m_nMoveStep = 1;
                    Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
                    return;                                               // 10225：Exit（非 Break）
                }
                break;
        }
    }

    /// <summary>
    /// `TNpcActor.Create`（**10231-10252**）1:1。
    ///
    /// <para><b>注意 10237-10239 是无条件三句</b>：<c>m_KeepSurface := nil</c>、
    /// <c>m_boHitEffect := False</c>、<c>m_bo248 := False</c> —— 与 10236 的 <c>m_EffSurface := nil</c>
    /// 一起，**不论是否是自定义 NPC** 都会执行。</para>
    ///
    /// <para><b>10248 写的是 <c>TimeGetTime</c>（无括号）</b>，而 11111 写的是
    /// <c>TimeGetTime</c>（同样无括号）—— Delphi 里两者等价，此处统一取
    /// <see cref="ActorNpcEnv.TimeGetTimeFn"/>。</para>
    /// </summary>
    public TNpcActor()
    {
        m_EffSurface = null;                                              // 10236
        m_KeepSurface = null;                                             // 10237
        m_boHitEffect = false;                                            // 10238
        m_bo248 = false;                                                  // 10239

        if (m_wAppearance >= 10000)                                       // 10241
        {
            var npcConfig = ActorNpcEnv.CustomNpcConfigLookupFn(m_wAppearance);
            if (npcConfig != null)
            {
                var bc = npcConfig.Value.BaseConfig;
                if (bc.KeepPlayFile >= 0 && bc.KeepPlayFile < ActorNpcEnv.EffectImageListCountFn()
                    && bc.KeepPlayIndex >= 0 && bc.KeepPlayCount > 0 && bc.KeepPlayTime > 0)
                {
                    m_nKeepFrame = bc.KeepPlayIndex;                      // 10247
                    m_LastKeepPlayTick = ActorNpcEnv.TimeGetTimeFn();     // 10248
                }
            }
        }
    }

    /// <summary>
    /// `TNpcActor.Initialize`（**10254-10257**）：**只有 <c>inherited Initialize</c> 一句**。
    /// <para>原文 10256 的 <c>inherited Initialize;</c> 转调 <c>TActor.Initialize</c>（5420-5423，
    /// 实体为空 <c>begin end</c>）。托管侧基类此前**没有** <c>Initialize</c> 这个名字
    /// （见报告「新增虚槽位」），本车道在 <c>TActorCore</c> 上补出同名虚成员，故此处逐字写 <c>base</c> 转调。</para>
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();                                                // 10256
    }

    /// <summary>
    /// `TNpcActor.Finalize`（**10259-10264**）1:1：先 <c>inherited Finalize</c>，
    /// **再**清两个表面槽（顺序即语义：基类 Finalize 里若用到它们，此处必须在其后清）。
    /// </summary>
    public override void Finalize()
    {
        base.Finalize();                                                  // 10261
        m_EffSurface = null;                                              // 10262
        m_KeepSurface = null;                                             // 10263
    }

    /// <summary>
    /// `TNpcActor.CheckLoadUserName`（**10266-10295，30 行**）1:1。
    ///
    /// <para><b>流程</b>：</para>
    /// <list type="number">
    /// <item><b>10268-10269</b>：<c>Result := False</c>，画布不 Active/Initialized 则 **Exit**（返回 False）。</item>
    /// <item><b>10270</b>：<c>m_sNameText := '';</c>（**先清空**）。</item>
    /// <item><b>10271-10291</b>：<c>PlugInEnabled</c> 两分支 —— 开：需过
    ///   "名字不是 '不显名'" 且 "非（幽灵隐藏且已死）"，再看
    ///   (显示 NPC 名 且 未死) → <c>Desc\User</c>，否则 <c>g_FocusCret = Self</c> → <c>Desc\User</c>；
    ///   关：只需 "名字不是 '不显名'" 且 <c>g_FocusCret = Self</c>。</item>
    /// <item><b>10294</b>：<c>Result := CompareText(m_sCurNameText, m_sNameText) &lt;&gt; 0</c>
    ///   —— **大小写不敏感**比较；10294 行后半段的时间戳判据被原文注释掉，逐字略。</item>
    /// </list>
    ///
    /// <para><b>原文易错点（锁死）</b>：<c>CompareText</c>（大小写不敏感）而**非** <c>CompareStr</c>；
    /// 且 10273 的幽灵隐藏门是 `g_ClientConfig.boHideGhost **and** ConfigCheckeds[ckHideGhost]`
    /// 与 <c>m_boDeath</c> 的**三重与**，写成"或"会改变可见性。</para>
    /// </summary>
    public override bool CheckLoadUserName()
    {
        bool result = false;                                              // 10268
        if (!ActorNpcEnv.CanvasReadyFn())
            return result;                                                // 10269

        m_sNameText = "";                                                 // 10270

        if (ActorNpcEnv.PlugInEnabled)                                    // 10271
        {
            if (!CompareText(m_sUserName, "不显名"))                       // 10272
            {
                if (!((ActorNpcEnv.ClientConfigBoHideGhost && ActorNpcEnv.ConfigDlgCkHideGhost) && m_boDeath))
                {
                    if (ActorNpcEnv.ClientConfigBoShowNpcName && ActorNpcEnv.ConfigDlgCkShowNpcName && !m_boDeath)
                        m_sNameText = m_sDescUserName + "\\" + m_sUserName;      // 10277
                    else if (ActorNpcEnv.IsFocusActorFn(this))
                        m_sNameText = m_sDescUserName + "\\" + m_sUserName;      // 10280
                }
            }
        }
        else
        {
            if (!CompareText(m_sUserName, "不显名"))                       // 10286
            {
                if (ActorNpcEnv.IsFocusActorFn(this))
                    m_sNameText = m_sDescUserName + "\\" + m_sUserName;          // 10288
            }
        }

        // 10294：`{ or (MyGetTickCount - m_dwShowNameTimeTick > 1000 * 2)}` 整段被原文注释
        result = !CompareText(m_sCurNameText, m_sNameText);
        return result;
    }

    /// <summary>Delphi <c>CompareText</c>：**大小写不敏感**判等（返回是否相等，反转 <c>&lt;&gt; 0</c>）。</summary>
    private static bool CompareText(string a, string b)
        => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// `TNpcActor.DrawChr`（**10297-10422，126 行**）1:1。
    ///
    /// <para><b>10422 是全部出口</b>：本方法**不转调** <c>inherited DrawChr</c>（原文没有 <c>inherited</c>）；
    /// 它按外观分成"自定义 NPC（&gt;=10000）"与"全局表"两大支，互斥。</para>
    ///
    /// <para><b>自定义 NPC 支（10351-10387）</b>：先 <c>GetCustomNpcConfig</c>，**配置为 nil 则整段不画**
    /// （原文的 if 没有 else —— 注意外层 else 属于 `if m_wAppearance &gt;= 10000`，**不是**属于
    /// `if NpcConfig &lt;&gt; nil`）。六种 <c>DrawOrder</c> 是**六种不同的绘制顺序**，
    /// 不是同一顺序的等价写法 —— 表见测试 `DrawChr_CustomNpcSixDrawOrders`。</para>
    ///
    /// <para><b>全局表支（10388-10421）</b>：</para>
    /// <list type="bullet">
    /// <item><b>10389-10390</b>：外观不在 246..272 ⇒ <c>m_btDir := m_btDir mod 3</c>
    ///   （**注意：这是对实例字段的写回**，与 <c>CalcActorFrame</c> 同理）。</item>
    /// <item><b>10392-10405</b>：外观 54..58 / 94..98 ⇒ <c>DrawBlend</c>（**不看 blend 实参**）；
    ///   外观 51 ⇒ <c>DrawEffSurface(..., True, ...)</c>（**强制混合**）；否则用实参 <c>blend</c>。</item>
    /// <item><b>10415-10420</b>：特效层的外观白名单是
    ///   `64..68, 70..75, 84, 90, 91, 100, 101, 209, 217..219, 221, 222, 224`，走 <c>DrawBlend</c>。</item>
    /// </list>
    /// </summary>
    public override void DrawChr(int dx, int dy, bool blend, bool boFlag)
    {
        if (m_wAppearance >= 10000)                                       // 10351
        {
            var cfg = ActorNpcEnv.CustomNpcConfigLookupFn(m_wAppearance);
            if (cfg != null)
            {
                switch (cfg.Value.BaseConfig.DrawOrder)                   // 10354-10385
                {
                    case TCustomNpcDrawOrder.ndoKeep_Chr_Eff:
                        DrawEffect(cfg.Value, dx, dy);
                        DrawBody(cfg.Value, dx, dy, blend);
                        DrawKeep(cfg.Value, dx, dy);
                        break;
                    case TCustomNpcDrawOrder.ndoKeep_Eff_Chr:
                        DrawBody(cfg.Value, dx, dy, blend);
                        DrawEffect(cfg.Value, dx, dy);
                        DrawKeep(cfg.Value, dx, dy);
                        break;
                    case TCustomNpcDrawOrder.ndoChr_Keep_Eff:
                        DrawEffect(cfg.Value, dx, dy);
                        DrawKeep(cfg.Value, dx, dy);
                        DrawBody(cfg.Value, dx, dy, blend);
                        break;
                    case TCustomNpcDrawOrder.ndoChr_Eff_Keep:
                        DrawKeep(cfg.Value, dx, dy);
                        DrawEffect(cfg.Value, dx, dy);
                        DrawBody(cfg.Value, dx, dy, blend);
                        break;
                    case TCustomNpcDrawOrder.ndoEff_Keep_Chr:
                        DrawBody(cfg.Value, dx, dy, blend);
                        DrawKeep(cfg.Value, dx, dy);
                        DrawEffect(cfg.Value, dx, dy);
                        break;
                    case TCustomNpcDrawOrder.ndoEff_Chr_Keep:
                        DrawKeep(cfg.Value, dx, dy);
                        DrawBody(cfg.Value, dx, dy, blend);
                        DrawEffect(cfg.Value, dx, dy);
                        break;
                }
            }
        }
        else
        {
            if (m_wAppearance < 246 || m_wAppearance > 272)               // 10389
                m_btDir = (byte)(m_btDir % 3);

            if (m_BodySurface != null)                                    // 10391
            {
                if (m_wAppearance is >= 54 and <= 58 or >= 94 and <= 98)  // 10392
                {
                    ActorNpcEnv.NpcDrawFn(new NpcDrawOp(NpcDrawKind.DrawBlend,
                        dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY,
                        m_BodySurface, "Body", 10393));                   // 原文不看 blend 实参
                }
                else if (m_wAppearance == 51)                             // 10398
                {
                    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY,
                        true, ActorColorEffect);                          // 10402：强制 True
                }
                else
                {
                    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY,
                        blend, ActorColorEffect);                         // 10407-10412
                }
            }

            if (m_wAppearance is (>= 64 and <= 68) or (>= 70 and <= 75) or 84 or 90 or 91 or 100 or 101
                or 209 or (>= 217 and <= 219) or 221 or 222 or 224)       // 10415
            {
                if (m_EffSurface != null)
                {
                    ActorNpcEnv.NpcDrawFn(new NpcDrawOp(NpcDrawKind.DrawBlend,
                        dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY,
                        m_EffSurface, "Eff", 10416));
                }
            }
        }
    }

    /// <summary>`DrawChr` 内嵌过程 `DrawEffect`（10301-10317）1:1。</summary>
    private void DrawEffect(TClientCustomNpcConfig npcConfig, int dx, int dy)
    {
        if (m_EffSurface == null)
            return;                                                       // 10303

        bool useBlend;
        if (m_nCurrentAction is SM_HIT or SM_WALK)                        // 10304
            useBlend = npcConfig.BaseConfig.ActionEffectDrawMode != TCustomDrawMode.mdmNormal;
        else
            useBlend = npcConfig.BaseConfig.StandEffectDrawMode != TCustomDrawMode.mdmNormal;

        ActorNpcEnv.NpcDrawFn(new NpcDrawOp(
            useBlend ? NpcDrawKind.DrawBlend : NpcDrawKind.Draw,
            dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY,
            m_EffSurface, "Effect", 10306));
    }

    /// <summary>`DrawChr` 内嵌过程 `DrawKeep`（10319-10327）1:1。</summary>
    private void DrawKeep(TClientCustomNpcConfig npcConfig, int dx, int dy)
    {
        if (m_KeepSurface == null)
            return;                                                       // 10321

        bool useBlend = npcConfig.BaseConfig.KeepPlayBlendDraw != 0;       // 10322
        ActorNpcEnv.NpcDrawFn(new NpcDrawOp(
            useBlend ? NpcDrawKind.DrawBlend : NpcDrawKind.Draw,
            dx + m_nKeepX + m_nShiftX + npcConfig.BaseConfig.KeepPlayOffsetX,
            dy + m_nKeepY + m_nShiftY + npcConfig.BaseConfig.KeepPlayOffsetY,
            m_KeepSurface, "Keep", 10323));
    }

    /// <summary>`DrawChr` 内嵌过程 `DrawBody`（10329-10349）1:1。</summary>
    private void DrawBody(TClientCustomNpcConfig npcConfig, int dx, int dy, bool blend)
    {
        if (m_BodySurface == null)
            return;                                                       // 10331

        bool forceBlend;
        if (m_nCurrentAction is SM_HIT or SM_WALK)                        // 10332
            forceBlend = npcConfig.BaseConfig.ActionDrawMode != TCustomDrawMode.mdmNormal;
        else
            forceBlend = npcConfig.BaseConfig.StandDrawMode != TCustomDrawMode.mdmNormal;

        DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY,
            forceBlend ? true : blend, ActorColorEffect);                 // 10334 / 10342

        DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);              // 10338 / 10346
    }

    /// <summary>
    /// `TNpcActor.DrawEff`（**10424-10433**）1:1。
    /// <para><b>10426 的 <c>// inherited;</c> 是注释</b> —— 原文**不转调基类**（基类
    /// <c>TActor.DrawEff</c> 6131-6134 是空实现，故等价，但注释必须保留以免被"补回"）。</para>
    /// </summary>
    public override void DrawEff(int dx, int dy)
    {
        // inherited;   ← 原文如此：10426 该行被注释掉
        if (m_boUseEffect && m_EffSurface != null)
        {
            ActorNpcEnv.NpcDrawFn(new NpcDrawOp(NpcDrawKind.DrawBlend,
                dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY, m_EffSurface, "Eff", 10428));
        }
    }

    /// <summary>
    /// `TNpcActor.GetDefaultFrame`（**10435-10480，45 行**）1:1。
    ///
    /// <para><b>10441 <c>Result := 0</c> 是无条件初值</b>（Jacky）；
    /// 自定义 NPC 支与全局支都可能**提前 <c>Exit</c> 而不改它**（10463）。</para>
    ///
    /// <para><b>★ 顺序差异必须照抄</b>：自定义 NPC 支在算 <c>cf</c> **之前**取模方向（10445-10448）；
    /// 而全局支是在算完 <c>cf</c> 之后（10475-10476）才把方向归零，
    /// 且归零的外观集合与 <c>CalcActorFrame</c> 的 9978 行**不同**（这里**不含 245**）。</para>
    ///
    /// <para><b>10475-10476 的顺序</b>：先算 <c>cf</c>（用未归零的 <c>m_btDir</c> 只影响 10478 的乘法），
    /// 再把 <c>m_btDir</c> 置 0 —— 所以 10478 用的是**归零后**的方向。逐字保留。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        int result = 0;                                                   // 10441
        int cf;

        if (m_wAppearance >= 10000)
        {
            var cfg = ActorNpcEnv.CustomNpcConfigLookupFn(m_wAppearance);  // 10443
            if (cfg != null)
            {
                var c = cfg.Value;
                if (c.wDirCount > 1)                                      // 10445
                    m_btDir = (byte)(m_btDir % c.wDirCount);
                else
                    m_btDir = 0;                                          // 10448

                var act = c.Actions[m_btDir];
                if (m_nCurrentDefFrame < 0)                               // 10450
                    cf = 0;
                else if (m_nCurrentDefFrame >= act.Std_Count)
                    cf = 0;
                else
                    cf = m_nCurrentDefFrame;

                result = act.Std_Index + cf;                              // 10457
                m_dwFrameTime = act.Std_Time;                             // 10458
            }
        }
        else
        {
            var pm = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 10462
            if (pm == null)
                return result;                                            // 10463：Exit

            var p = pm.Value;
            if (m_wAppearance < 246 || m_wAppearance > 272)               // 10465
                m_btDir = (byte)(m_btDir % 3);

            if (m_nCurrentDefFrame < 0)                                   // 10468
                cf = 0;
            else if (m_nCurrentDefFrame >= p.ActStand.frame)
                cf = 0;
            else
                cf = m_nCurrentDefFrame;

            // 10475：**不含 245**（与 CalcActorFrame 的 9978 行不同）—— 原文如此
            if (m_wAppearance is (>= 54 and <= 59) or (>= 70 and <= 75) or (>= 81 and <= 84)
                or (>= 90 and <= 92) or (>= 94 and <= 101) or (>= 211 and <= 225))
                m_btDir = 0;

            result = p.ActStand.start + m_btDir * (p.ActStand.frame + p.ActStand.skip) + cf;   // 10478
        }

        return result;
    }

    /// <summary>
    /// `TNpcActor.LoadSurface`（**10482-11058，577 行**）1:1。
    ///
    /// <para><b>四大段（顺序即语义、每段各自 <c>Exit</c>）</b>：</para>
    /// <list type="number">
    /// <item><b>10491-10496</b>：无条件打点 + 清 <c>m_boLoadSurface</c> + 清 <b>三个</b>表面槽
    ///   （<c>m_BodySurface</c>/<c>m_EffSurface</c>/<c>m_KeepSurface</c>）。</item>
    /// <item><b>10500-10578</b>（外观 &gt;= 10000 自定义 NPC）：<c>m_nBodyOffset := 0</c>；
    ///   配置非 nil 才取 <c>NpcDirAction</c>；Keep 特效越界门是**五重与**
    ///   （KeepPlayFile 双界 + Index&gt;=0 + Count&gt;0 + Time&gt;0——注意 10516 取图**不判断 Time**）；
    ///   HIT/WALK 走 <c>Act_*</c>，其余走 <c>Std_*</c>；特效段判的是
    ///   **<c>Act_Count</c>/<c>Std_Count</c> 与 <c>Act_Time</c>/<c>Std_Time</c>**（原文笔误：特效段复用主体的 Count/Time），
    ///   逐字保留。末尾 <c>LoadActorIcons</c> 后 <c>Exit</c>。</item>
    /// <item><b>10580-10778</b>（外观 &gt;= 2000）：<c>wAppearance := m_wAppearance - 2000</c>；
    ///   <c>m_btRace = 50</c> 时按 <c>wAppearance &lt; 200</c> 选 <c>Indexs[10]/[11]</c>；
    ///   随后**六段 if/else 特效**（64..67 / 68 / 70..75 / 84 / 90,91 / 101 / 209），
    ///   其中 84 的分支含 `m_nStartFrame &gt;= 22 / &gt;= 4` 三层与 `m_EffSurface := nil`。</item>
    /// <item><b>10780-11057</b>（普通 NPC）：<c>m_btRace = 50</c> 时五段 Indexs 选择
    ///   （0 / 2（含 226..245，且 761、891 两处**坐标修正**）/ 3 / 9 / 1）；
    ///   之后特效段一长串 if/else（含外观 42 的**不带 Indexs** 取图 —— 10939-10942 的原文差异）。</item>
    /// </list>
    ///
    /// <para><b>★ 10526/10537/10550/10561 的 <c>{Prefix &gt;= 0}</c> 都带注释括号</b>：
    /// 原文把这四个"下界判定"注释掉了（只留上界 <c>&lt; g_EffectImageList.Count</c>），
    /// 托管侧逐字保留"只判上界"。</para>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        // 10491-10496
        m_dwLoadSurfaceTime = ActorNpcEnv.MyGetTickCountFn();
        m_boLoadSurface = false;
        m_BodySurface = null;
        m_EffSurface = null;
        m_KeepSurface = null;

        int effectCount = ActorNpcEnv.EffectImageListCountFn();

        // ───────────────────── 段 1：自定义 NPC（>= 10000） ─────────────────────
        if (m_wAppearance >= 10000)
        {
            m_nBodyOffset = 0;                                            // 10501

            var cfgOpt = ActorNpcEnv.CustomNpcConfigLookupFn(m_wAppearance);
            TNpcDirAction? npcDirAction = null;
            if (cfgOpt != null)
            {
                var c = cfgOpt.Value;
                if (c.wDirCount > 1)                                      // 10505
                    m_btDir = (byte)(m_btDir % c.wDirCount);
                else
                    m_btDir = 0;
                npcDirAction = c.Actions[m_btDir];                        // 10509
            }

            if (npcDirAction != null && cfgOpt != null)
            {
                var bc = cfgOpt.Value.BaseConfig;
                var nd = npcDirAction.Value;

                // 10513-10523：Keep 特效（五重门；**取图本身不判 KeepPlayTime**）
                if (bc.KeepPlayFile >= 0 && bc.KeepPlayFile < effectCount
                    && bc.KeepPlayIndex >= 0 && bc.KeepPlayCount > 0 && bc.KeepPlayTime > 0)
                {
                    var fetch = ActorNpcEnv.FetchEffectListImageFn(bc.KeepPlayFile, m_nKeepFrame, ActorColorEffect);
                    m_KeepSurface = fetch.Texture;                        // 10518-10521
                    m_nKeepX = fetch.OffsetX;
                    m_nKeepY = fetch.OffsetY;
                }

                if (m_nCurrentAction is SM_HIT or SM_WALK)
                {
                    // 10526：下界判定被原文注释掉，只判上界
                    if (nd.Act_File < effectCount && nd.Act_Index >= 0 && nd.Act_Count > 0 && nd.Act_Time > 0)
                    {
                        // 10530：ceGrayScale **或 ceGrayScale2** 都取灰度
                        var fetch = ActorNpcEnv.FetchEffectListImageFn(nd.Act_File, m_nCurrentFrame, ActorColorEffect);
                        m_BodySurface = fetch.Texture;
                        m_nPx = fetch.OffsetX;
                        m_nPy = fetch.OffsetY;
                    }

                    // 10537：判的是 Act_Count/Act_Time（原文笔误，不是 *_EffCount/*_EffTime）
                    if (nd.Act_EffFile < effectCount && nd.Act_EffIndex >= 0 && nd.Act_Count > 0 && nd.Act_Time > 0)
                    {
                        int index = nd.Act_EffIndex + m_nCurrentFrame - nd.Act_Index;   // 10540
                        var fetch = ActorNpcEnv.FetchEffectListImageFn(nd.Act_EffFile, index, ActorColorEffect);
                        m_EffSurface = fetch.Texture;
                        m_nEffX = fetch.OffsetX;
                        m_nEffY = fetch.OffsetY;
                    }
                }
                else
                {
                    // 10550：下界判定同样被注释
                    if (nd.Std_File < effectCount && nd.Std_Index >= 0 && nd.Std_Count > 0 && nd.Std_Time > 0)
                    {
                        var fetch = ActorNpcEnv.FetchEffectListImageFn(nd.Std_File, m_nCurrentFrame, ActorColorEffect);
                        m_BodySurface = fetch.Texture;
                        m_nPx = fetch.OffsetX;
                        m_nPy = fetch.OffsetY;
                    }

                    // 10561：判的是 Std_Count/Std_Time
                    if (nd.Std_EffFile < effectCount && nd.Std_EffIndex >= 0 && nd.Std_Count > 0 && nd.Std_Time > 0)
                    {
                        int index = nd.Std_EffIndex + m_nCurrentFrame - nd.Std_Index;   // 10564
                        var fetch = ActorNpcEnv.FetchEffectListImageFn(nd.Std_EffFile, index, ActorColorEffect);
                        m_EffSurface = fetch.Texture;
                        m_nEffX = fetch.OffsetX;
                        m_nEffY = fetch.OffsetY;
                    }
                }
            }

            LoadActorIcons();                                             // 10576
            return;                                                       // 10577
        }

        // ───────────────────── 段 2：外观 >= 2000 ─────────────────────
        if (m_wAppearance >= 2000)
        {
            int wAppearance = m_wAppearance - 2000;                       // 10581

            if (m_btRace == 50)                                           // 10582
            {
                int store = wAppearance < 200 ? 10 : 11;                  // 10583-10599
                FetchNpcBody(store, m_nBodyOffset + m_nCurrentFrame);

                if (wAppearance is >= 64 and <= 67)                       // 10600-10607
                    FetchNpcEff(store, 3540 + m_nCurrentFrame);
                else if (wAppearance == 68)                               // 10608-10615
                    FetchNpcEff(store, 3660 + m_nCurrentFrame);
                else if (wAppearance is >= 70 and <= 75)                  // 10616-10623
                    FetchNpcEff(store, m_nBodyOffset + 4 + m_nCurrentFrame);
                else if (wAppearance == 84)                               // 10624-10652
                {
                    if (m_nStartFrame >= 22)
                    {
                        if (m_nCurrentFrame <= 4)
                            FetchNpcEff(store, m_nBodyOffset - m_nCurrentFrame);
                        else
                            m_EffSurface = null;                          // 10635
                    }
                    else if (m_nStartFrame >= 4)
                    {
                        if (m_nCurrentFrame >= 4)
                            FetchNpcEff(store, m_nBodyOffset + m_nCurrentFrame + 12);
                        else
                            m_EffSurface = null;                          // 10647
                    }
                    else
                    {
                        m_EffSurface = null;                              // 10650
                    }
                }
                else if (wAppearance is 90 or 91)                         // 10653-10660
                    FetchNpcEff(store, m_nBodyOffset + 4 + m_nCurrentFrame);
                else if (wAppearance == 101)                              // 10661-10668
                    FetchNpcEff(store, m_nBodyOffset + 20 + m_nCurrentFrame);
                else if (wAppearance == 209)                              // 10669-10676
                    FetchNpcEff(11, m_nBodyOffset + 4 + m_nCurrentFrame);
            }

            if (wAppearance is >= 42 and <= 47)                           // 10679-10680
                m_BodySurface = null;

            if (m_boUseEffect)                                            // 10681-10774
            {
                if (wAppearance is >= 33 and <= 34)                       // 10682-10689
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                else if (wAppearance == 42)                               // 10690-10699
                {
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                    // 10697-10698 两行坐标修正被原文注释
                }
                else if (wAppearance == 43)                               // 10700-10709
                {
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                    // 10707-10708 同上被注释
                }
                else if (wAppearance == 44)                               // 10710-10719
                {
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                    m_nEffX += 7;                                         // 10717
                    m_nEffY += 12;                                        // 10718
                }
                else if (wAppearance == 45)                               // 10720-10729
                {
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                    m_nEffX += 6;                                         // 10727
                    m_nEffY += 12;                                        // 10728
                }
                else if (wAppearance == 46)                               // 10730-10739
                {
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                    m_nEffX += 7;                                         // 10737
                    m_nEffY += 12;                                        // 10738
                }
                else if (wAppearance == 47)                               // 10740-10749
                {
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                    m_nEffX += 8;                                         // 10747
                    m_nEffY += 12;                                        // 10748
                }
                else if (wAppearance == 51)                               // 10750-10757
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                else if (wAppearance == 52)                               // 10758-10765
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
                else if (wAppearance == 100)                              // 10766-10773
                    FetchNpcEff(10, m_nBodyOffset + m_nEffectFrame);
            }

            LoadActorIcons();                                             // 10776
            return;                                                       // 10777
        }

        // ───────────────────── 段 3：普通 NPC ─────────────────────
        if (m_btRace == 50)                                               // 10780
        {
            if (m_wAppearance < 200)                                      // 10781
                FetchNpcBody(0, m_nBodyOffset + m_nCurrentFrame);
            else if (m_wAppearance >= 226 && m_wAppearance <= 245)        // 10789
            {
                FetchNpcBody(2, m_nBodyOffset + m_nCurrentFrame);

                // 10796-10802：修正 npc3.wzl 中有些资源错误 chongchong 2015-04-20
                // 原文这两处坐标修正只在 **else（非 ceGrayScale/ceGrayScale2）** 分支里执行
                if (ActorColorEffect is not (TColorEffect.ceGrayScale or TColorEffect.ceGrayScale2))
                {
                    if (m_nBodyOffset + m_nCurrentFrame == 761)
                        m_nPy = -42;
                    else if (m_nBodyOffset + m_nCurrentFrame == 891)
                    {
                        m_nPx = 10;
                        m_nPy = -45;
                    }
                }

                if (m_wAppearance == 244)                                 // 10805-10812
                    FetchNpcEff(2, m_nBodyOffset + m_nCurrentFrame + 30);
                else if (m_wAppearance == 245)                            // 10813-10820
                    FetchNpcEff(2, m_nBodyOffset + m_nCurrentFrame + 10);
            }
            else if (m_wAppearance >= 246 && m_wAppearance <= 272)        // 10822
                FetchNpcBody(3, m_nBodyOffset + m_nCurrentFrame);
            else if (m_wAppearance >= 1000)                               // 10830
                FetchNpcBody(9, m_nBodyOffset + m_nCurrentFrame);
            else                                                          // 10838
                FetchNpcBody(1, m_nBodyOffset + m_nCurrentFrame);

            if (m_wAppearance is >= 64 and <= 67)                         // 10847-10854
                FetchNpcEff(0, 3540 + m_nCurrentFrame);
            else if (m_wAppearance == 68)                                 // 10855-10862
                FetchNpcEff(0, 3660 + m_nCurrentFrame);
            else if (m_wAppearance is >= 70 and <= 75)                    // 10863-10870
                FetchNpcEff(0, m_nBodyOffset + 4 + m_nCurrentFrame);
            else if (m_wAppearance == 84)                                 // 10871-10899
            {
                if (m_nStartFrame >= 22)
                {
                    if (m_nCurrentFrame <= 4)
                        FetchNpcEff(0, m_nBodyOffset - m_nCurrentFrame);
                    else
                        m_EffSurface = null;                              // 10882
                }
                else if (m_nStartFrame >= 4)
                {
                    if (m_nCurrentFrame >= 4)
                        FetchNpcEff(0, m_nBodyOffset + m_nCurrentFrame + 12);
                    else
                        m_EffSurface = null;                              // 10894
                }
                else
                {
                    m_EffSurface = null;                                  // 10897
                }
            }
            else if (m_wAppearance is 90 or 91)                           // 10900-10907
                FetchNpcEff(0, m_nBodyOffset + 4 + m_nCurrentFrame);
            else if (m_wAppearance == 101)                                // 10908-10915
                FetchNpcEff(0, m_nBodyOffset + 20 + m_nCurrentFrame);
            else if (m_wAppearance == 209)                                // 10916-10923
                FetchNpcEff(1, m_nBodyOffset + 4 + m_nCurrentFrame);
        }

        if (m_wAppearance is >= 42 and <= 47)                             // 10926-10927
            m_BodySurface = null;

        if (m_boUseEffect)                                                // 10928-11054
        {
            if (m_wAppearance is >= 33 and <= 34)                         // 10929-10936
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance == 42)                                 // 10937-10946
            {
                // ★ 10939-10942：**唯一**一处不带 `Indexs[]` 的取图（原文差异，逐字保留）
                var fetch = ActorNpcEnv.FetchNpcRootImageFn(m_nBodyOffset + m_nEffectFrame, ActorColorEffect);
                m_EffSurface = fetch.Texture;
                m_nEffX = fetch.OffsetX;
                m_nEffY = fetch.OffsetY;
                // 10944-10945 两行坐标修正被原文注释
            }
            else if (m_wAppearance == 43)                                 // 10947-10956
            {
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
                // 10954-10955 同上被注释
            }
            else if (m_wAppearance == 44)                                 // 10957-10966
            {
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
                m_nEffX += 7;                                             // 10964
                m_nEffY += 12;                                            // 10965
            }
            else if (m_wAppearance == 45)                                 // 10967-10976
            {
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
                m_nEffX += 6;                                             // 10974
                m_nEffY += 12;                                            // 10975
            }
            else if (m_wAppearance == 46)                                 // 10977-10986
            {
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
                m_nEffX += 7;                                             // 10984
                m_nEffY += 12;                                            // 10985
            }
            else if (m_wAppearance == 47)                                 // 10987-10996
            {
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
                m_nEffX += 8;                                             // 10994
                m_nEffY += 12;                                            // 10995
            }
            else if (m_wAppearance == 51)                                 // 10997-11004
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance == 52)                                 // 11005-11012
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance == 100)                                // 11013-11020
                FetchNpcEff(0, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance is >= 217 and <= 219)                  // 11021-11028
                FetchNpcEff(1, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance == 221)                                // 11029-11036
                FetchNpcEff(1, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance == 222)                                // 11037-11044
                FetchNpcEff(1, m_nBodyOffset + m_nEffectFrame);
            else if (m_wAppearance == 224)                                // 11045-11052
                FetchNpcEff(1, m_nBodyOffset + m_nEffectFrame);
        }

        LoadActorIcons();                                                 // 11057
    }

    /// <summary>
    /// `LoadSurface` 里 ~40 处同形的"主体图"取图（<c>g_WNpcImgImages.Indexs[store].GetCached*(idx, m_nPx, m_nPy)</c>）。
    /// <para>三分支语义（<c>ceGrayScale,ceGrayScale2</c> → 灰度；<c>ceBright</c> → 高亮；否则原图）
    /// **由 <see cref="ActorNpcEnv.FetchNpcImageFn"/> 按 <c>m_ColorEffect</c> 承载** —— 逐调用点的
    /// 原文行号写在调用处。</para>
    /// </summary>
    private void FetchNpcBody(int store, int index)
    {
        var fetch = ActorNpcEnv.FetchNpcImageFn(store, index, ActorColorEffect);
        m_BodySurface = fetch.Texture;
        m_nPx = fetch.OffsetX;
        m_nPy = fetch.OffsetY;
    }

    /// <summary>见 <see cref="FetchNpcBody"/>；此处写的是 <c>m_EffSurface</c>/<c>m_nEffX</c>/<c>m_nEffY</c>。</summary>
    private void FetchNpcEff(int store, int index)
    {
        var fetch = ActorNpcEnv.FetchNpcImageFn(store, index, ActorColorEffect);
        m_EffSurface = fetch.Texture;
        m_nEffX = fetch.OffsetX;
        m_nEffY = fetch.OffsetY;
    }

    /// <summary>
    /// `TNpcActor.Run`（**11060-11122，62 行**）1:1。
    ///
    /// <para><b>流程</b>：</para>
    /// <list type="number">
    /// <item><b>11067</b>：先 <c>inherited Run</c>（基类帧推进）。</item>
    /// <item><b>11068-11069</b>：**在推进前**快照 <c>nEffectFrame</c> / <c>nKeepFrame</c>。</item>
    /// <item><b>11070-11096</b>：<c>m_boUseEffect</c> 时按
    ///   `m_boUseMagic ? m_dwEffectFrameTime / 3 : m_dwEffectFrameTime` 节流推进特效帧；
    ///   到尾时若 <c>m_bo248</c> 且已过 <c>m_dwUseEffectTick</c> 则**熄灭**（11085-11087），
    ///   否则回卷 <c>m_nEffectFrame := m_nEffectStart</c>（**两种情况都回卷**，11089/11092）。</item>
    /// <item><b>11098-11115</b>：自定义 NPC 的 Keep 帧推进（五重门 + 时间门）。</item>
    /// <item><b>11117-11120</b>：**只有**两个帧号之一变了，才打点并请求
    ///   <c>PlayScene.LoadSurface(LoadSurface)</c>。</item>
    /// </list>
    ///
    /// <para><b>原文易错点（锁死）</b>：</para>
    /// <list type="bullet">
    /// <item>11104 的比较是 <c>&gt;=</c>（**闭区间**）；</item>
    /// <item>11106 是 <c>if m_nKeepFrame &lt; KeepPlayIndex</c>（**先加后夹**，可能的写法误读为
    ///   `&lt; KeepPlayIndex + Count`）—— 原文如此；</item>
    /// <item>11117 用 <c>or</c>（任一变化都重载），不是 <c>and</c>；</item>
    /// <item>11084 是 <c>TimeGetTime &gt; m_dwUseEffectTick</c>（**严格大于**）。</item>
    /// </list>
    /// </summary>
    public override void Run(uint now)
    {
        base.Run(now);                                                    // 11067

        int nEffectFrame = m_nEffectFrame;                                // 11068
        int nKeepFrame = m_nKeepFrame;                                    // 11069

        if (m_boUseEffect)                                                // 11070
        {
            uint dwEffectFrameTime = m_boUseMagic
                ? (uint)DelphiRound(m_dwEffectFrameTime / 3.0)            // 11072
                : m_dwEffectFrameTime;                                    // 11075

            uint nowTick = ActorNpcEnv.TimeGetTimeFn();
            if (nowTick - m_dwEffectStartTime > dwEffectFrameTime)        // 11077
            {
                m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();        // 11078
                if (m_nEffectFrame < m_nEffectEnd)                        // 11079
                {
                    m_nEffectFrame++;                                     // 11080
                }
                else
                {
                    if (m_bo248)                                          // 11083
                    {
                        if (nowTick > m_dwUseEffectTick)                  // 11084：严格大于
                        {
                            m_boUseEffect = false;                        // 11085
                            m_bo248 = false;                              // 11086
                            m_dwUseEffectTick = ActorNpcEnv.TimeGetTimeFn();   // 11087
                        }
                        m_nEffectFrame = m_nEffectStart;                  // 11089
                    }
                    else
                    {
                        m_nEffectFrame = m_nEffectStart;                  // 11092
                    }
                    m_dwEffectStartTime = ActorNpcEnv.TimeGetTimeFn();    // 11093
                }
            }
        }

        if (m_wAppearance >= 10000)                                       // 11098
        {
            var cfg = ActorNpcEnv.CustomNpcConfigLookupFn(m_wAppearance);
            if (cfg != null)
            {
                var bc = cfg.Value.BaseConfig;
                if (bc.KeepPlayFile >= 0 && bc.KeepPlayFile < ActorNpcEnv.EffectImageListCountFn()
                    && bc.KeepPlayIndex >= 0 && bc.KeepPlayCount > 0 && bc.KeepPlayTime > 0)   // 11101-11103
                {
                    if (ActorNpcEnv.TimeGetTimeFn() - m_LastKeepPlayTick >= (uint)bc.KeepPlayTime)   // 11104
                    {
                        m_nKeepFrame++;                                   // 11105
                        if (m_nKeepFrame < bc.KeepPlayIndex)              // 11106
                            m_nKeepFrame = bc.KeepPlayIndex;              // 11107
                        else if (m_nKeepFrame >= bc.KeepPlayIndex + bc.KeepPlayCount)   // 11108
                            m_nKeepFrame = bc.KeepPlayIndex;              // 11109

                        m_LastKeepPlayTick = ActorNpcEnv.TimeGetTimeFn(); // 11111
                    }
                }
            }
        }

        if (nEffectFrame != m_nEffectFrame || nKeepFrame != m_nKeepFrame)  // 11117：or
        {
            m_dwLoadSurfaceTime = ActorNpcEnv.MyGetTickCountFn();          // 11118
            ActorNpcEnv.RequestLoadSurfaceFn?.Invoke(this);                // 11119
        }
    }
}

/// <summary>
/// 车道 `p17-client-actor`：本族需要的两个**原文虚槽位**，此前托管侧**没有**同名成员
/// （<c>TNpcActor.Initialize</c> 10254 / <c>CheckLoadUserName</c> 10266，
/// 以及 <c>THumActor</c>/<c>TNpcActor</c> 各自的覆写）。
///
/// <para><b>为什么必须补成 <c>virtual</c></b>：若不做成虚成员，子类的 <c>override</c> 无处可落，
/// 只能写 <c>new</c> 隐藏 —— 而调用点全部是**基类静态类型**（<c>LoadSurface</c> 尾部、
/// <c>ShowName</c> 族），于是多态永远落不到子类（台帐 §18.8 的隐蔽缺陷形态）。</para>
/// </summary>
public partial class TActorCore
{
    /// <summary>
    /// `TActor.Initialize`（原文 5420-5423）：**函数体为空**（<c>begin end</c>）。
    /// <para>空实现是**原文语义**，不是未接线 —— <c>TNpcActor.Initialize</c>（10254）与
    /// <c>THumActor.Initialize</c>（11218）都只写 <c>inherited Initialize;</c>。</para>
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// `TActor.CheckLoadUserName`（原文 7120-7173，53 行；主体尚未移植 → 显式留痕）。
    /// <para><b>NotPorted 留痕（台帐 §48.1）</b>：本槽位**只**为
    /// <c>TNpcActor.CheckLoadUserName</c>（10266）与 <c>THumActor.CheckLoadUserName</c>（13367）
    /// 提供可覆写目标。<c>TActor</c> 自己的 7120-7173 主体（名库装载 + 名字纹理规划）尚未落地，
    /// 故基类体保持"返回 false"这一**原文未做任何事时的等价结果**，并在报告「未完成/阻塞」中登记。</para>
    /// </summary>
    public virtual bool CheckLoadUserName() => NotPorted(nameof(CheckLoadUserName), 7120);

    /// <summary>
    /// `TActor.CheckLoadSurface`（原文 7355-7366，11 行；主体尚未移植 → 显式留痕）。
    /// <para><b>NotPorted 留痕</b>：本槽位为 <c>TStatuaryNpcActor.CheckLoadSurface</c>（17553）
    /// 与 <c>THumActor.CheckLoadSurface</c>（13465）提供可覆写目标。<c>TActor</c> 自己的
    /// 7355-7366（<c>m_boLoadSurface</c> + 2 秒节流的 <c>GetRaceByPM</c> 守卫）尚未落地，
    /// 在报告「未完成/阻塞」中登记。</para>
    /// </summary>
    public virtual bool CheckLoadSurface() => NotPorted(nameof(CheckLoadSurface), 7355);

    /// <summary>
    /// `TActor.Destroy`（原文 2945-2968，23 行；主体尚未移植 → 显式留痕）。
    /// <para>本槽位为 <c>TStatuaryNpcActor.Destroy</c>（17581）与 <c>THumActor.Destroy</c>（11212）
    /// 提供 <c>inherited</c> 目标。Delphi 的 <c>inherited</c> 在 C# 侧即 <c>base.Destroy()</c>。</para>
    /// </summary>
    public virtual void Destroy() => NotPorted(nameof(Destroy), 2945);

    /// <summary>
    /// `TActor.light`（原文 **5415-5418，4 行**）1:1：<c>Result := m_nChrLight;</c>。
    /// <para>本虚槽位是 <c>THumActor.light</c>（14520）的覆写目标。基类体是**原文的实体**
    /// （不是留痕），故 <c>TActor</c> 派的角色取到的就是自己的 <c>m_nChrLight</c>。</para>
    /// </summary>
    public virtual int light() => m_nChrLight;

    /// <summary>
    /// 台帐 §48.1 的显式留痕：暂时做不了的成员**必须**以本方法标出，禁止裸 <c>=&gt; true;</c>。
    /// <para>返回值恒 <c>false</c> 且**每次调用都记录**，使"未移植"在运行期可观测（而非静默中性值）。</para>
    /// <para><b>线程安全</b>：<see cref="NotPortedLog"/> 是**进程级静态**表，而 xUnit 默认**并行**跑
    /// 测试类 ⇒ 用 <see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/> 承载，
    /// 避免 <c>List&lt;T&gt;</c> 并发写损坏。</para>
    /// <para><b>精确断言用本线程采集</b>：<see cref="NotPortedCapture"/>（<c>[ThreadStatic]</c>）
    /// 让单个用例锁死"**我这次调用**产生的条目集合与条数"，不受其它并行用例污染
    /// （借用 `p16` 车道"留痕条目数要用例锁死"的教训）。</para>
    /// </summary>
    public static bool NotPorted(string member, int sourceLine)
    {
        string entry = $"{member}@{sourceLine}";
        NotPortedLog.Enqueue(entry);
        NotPortedCapture?.Add(entry);
        return false;
    }

    /// <summary>已有 <c>NotPorted</c> 调用的成员清单（进程级、按调用序、线程安全）。</summary>
    public static readonly System.Collections.Concurrent.ConcurrentQueue<string> NotPortedLog = new();

    /// <summary>
    /// **本线程**的留痕采集器：非 <c>null</c> 时，<see cref="NotPorted"/> 会把条目同时追加进来。
    /// <para>用例用法：置为新 <c>List</c> → 调用被测成员 → 断言内容与条数 → 复位为 <c>null</c>。
    /// 因是 <c>[ThreadStatic]</c>，并行测试类互不影响。</para>
    /// </summary>
    [ThreadStatic]
    public static List<string>? NotPortedCapture;

    /// <summary>已有 <c>NotPorted</c> 调用的成员清单（进程级、按调用序、线程安全）。</summary>
    // 说明：NotPortedLog 的类型与声明见上方（ConcurrentQueue）。
}
