using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>Grobal2.pas 4718-4727 TChrMsg（角色动作消息；Feature 头像面以 State2 承载）。</summary>
public class TChrMsg
{
    public int Ident;
    public int X;
    public int Y;
    public int Dir;
    public long State;
    public long Feature;
    public string Saying = "";
    public int Sound;
}

/// <summary>状态位（Grobal2 181-182 实值）。</summary>
public static class ActorStates
{
    public const long STATE_STONE_MODE = 1;
    public const long STATE_OPENHEATH = 2;
}

/// <summary>
/// Actor.pas 消息驱动核心扩展（批次J46）：TChrMsg 队列（SendMsg 入队 2985-3013 /
/// GetNextMsg 出队 4330-4361）与 ReadyAction 分发（3776-4200 核心语义：坐标与方向应用、
/// 受击帧时长公式、动作切换 + CalcActorFrame、死亡标记）。
/// </summary>
public partial class TActorCore
{
    // ---- 消息队列（Delphi m_MsgList:TGList of pTChrMsg） ----
    public readonly List<TChrMsg> MsgList = new();

    /// <summary>STATE_OPENHEATH 位（ReadyAction 头部状态位应用）。</summary>
    public bool m_boOpenHealth;
    public bool m_boDeath;
    public bool m_boSkeleton;
    public bool m_boStruckShowNumber;
    public bool m_boShowBigHPProgress;
    public uint m_dwDeathTick;
    public uint m_dwLastStruckTime;
    public uint m_dwCurrentActionTick;
    public int m_nHitEffectLevel;
    public int m_nLevel = 1;                 // m_Abil.Level（受击帧时长公式）
    public int m_btMonStruckFrameDelayTime;  // g_ClientConfig.btMonStruckFrameDelayTime
    public int m_nChangeAppr = -1;           // 自定义怪变脸（>=0 生效）

    // ---- 批次J73：SendMsg / ProcessActors 引用字段 ----
    /// <summary>m_nChrLight（HiByte(cdir) 承载）。</summary>
    public byte m_nChrLight;

    /// <summary>
    /// `m_nOldChrLight` 的**虚访问点**（缺陷 **H-1** 的修复，见 <c>D-P17-08</c>）。
    ///
    /// <para><b>① 原文事实（已取证）</b>：该字段**只**声明在 <c>TCustomActor</c>
    /// （<c>CustomActor.pas:33</c>，类型 <c>Integer</c>）；<c>Actor.pas</c> 的 <c>TActor</c>
    /// **没有**它。四处写点**全部带类型守卫</b>：</para>
    /// <list type="bullet">
    /// <item><c>PlayScn.pas:7852-7853</c>：<c>if Actor is TCustomActor then TCustomActor(Actor).m_nOldChrLight := Actor.m_nChrLight;</c></item>
    /// <item><c>PlayScn.pas:8019-8020</c>：同上（<c>ident &lt;&gt; SM_BACKSTEP</c> 支）</item>
    /// <item><c>PlayScn.pas:8023-8024</c>：同上（<c>SM_BACKSTEP</c> 支）</item>
    /// <item><c>ClMain.pas:25644</c>：<c>TCustomActor(Actor).m_nOldChrLight := DefMsg.param;</c>（未移植）</item>
    /// </list>
    /// <para>两处读点都在 <c>TCustomActor</c> 内部（<c>CustomActor.pas:98</c> / <c>:596</c>）。</para>
    ///
    /// <para><b>② 托管改法</b>：<c>ProcessActors</c> 的 <c>actor</c> 静态类型是 <c>TActorCore</c>
    /// （<c>PlaySceneCore.cs:153</c> 的 <c>List&lt;TActorCore&gt;</c>），而**字段访问不参与虚分派**
    /// ⇒ 原先"基类一份 <c>byte</c> 字段 + <c>TCustomActor</c> 一份 <c>int</c> 字段"必然分裂
    /// （写点写基类那份，<c>TCustomActor</c> 方法体读自己那份 ⇒ 自定义怪光照恒 0）。
    /// 现改为：基类只提供**虚访问点**，**真身存储**在派生类覆写里
    /// （<c>CustomActor.cs</c> 的 <c>public override int m_nOldChrLight { get; set; }</c>）。</para>
    ///
    /// <para><b>③ 是否偏离 ⇒ 是（D-P17-08）</b>：原文是"类型守卫 + 字段直写"，托管侧改为
    /// "虚属性 + 派生覆写"。行为等价（非自定义角色上写点被基类吸收 = 原文守卫不匹配），
    /// 但**访问机制变了**。基类实现**可计数**（<see cref="NonCustomActorOldChrLightWrites"/>），
    /// 故它不是台帐 §25.2 禁止的"静默中性值"。</para>
    ///
    /// <para><b>★ 附带发现的第二处缺陷（已报，未在本车道修）</b>：
    /// 原文三个写点**都带** <c>if Actor is TCustomActor</c> 守卫，而托管侧
    /// <c>PlaySceneMessages.cs:672/677</c> **没有**该守卫（写成无条件 <c>actor.m_nOldChrLight = ...</c>）。
    /// 由于原文对**非** <c>TCustomActor</c> 的角色**没有任何读点**，该缺失在当前
    /// <b>不可观测</b>（基类这份存储不会被原文语义读到）——
    /// 但守卫本身仍未补回，见报告 D-P17-09 与 §8.2 阻塞项。</para>
    ///
    /// <para><b>为什么基类这份仍带存储</b>（而不是做成 no-op）：
    /// 既有 <c>FormJ73Tests.cs:744-745 / 760-761</c> 断言"普通角色也能读写
    /// <c>m_nOldChrLight</c>"（那是按**无守卫的托管写点**写的）。
    /// 原文对普通角色既不写也不读它，故保留存储是**不可观测**的超集；
    /// 而本缺陷的实质（自定义怪读不到写点的值）已由派生覆写消除。
    /// 若日后把写点的守卫补回，可再评估是否连基类这份一并删除。</para>
    /// </summary>
    public virtual int m_nOldChrLight { get; set; }

    /// <summary>近身好友名单（g_MySelf.m_FriendHitList：≤4 格内 race 0 角色名去重记录）。</summary>
    public readonly List<string> m_FriendHitList = new();

    /// <summary>m_nMoveSpeed（ProcessActors 移动帧间隔公式用）。</summary>
    public int m_nMoveSpeed;

    /// <summary>m_nNameColor（等变脸换名时随名字一起搬移）。</summary>
    public int m_nNameColor;
    public int m_nCurrNameColor;

    /// <summary>m_boSendQueryBigHPProgress（SM_SKELETON 清标记）。</summary>
    public bool m_boSendQueryBigHPProgress;

    /// <summary>m_boWarMode（SM_CHARSTATUSCHANGED 状态位命中时清除）。</summary>
    public bool m_boWarMode;

    /// <summary>m_nAbilMP（m_Abil.MP；近身开盾可用性判定）。</summary>
    public int m_nAbilMP;

    /// <summary>m_nWaitForRecogId / m_WaitForFeature / m_nWaitForStatus（神兽变身换角等待）。</summary>
    public long m_nWaitForRecogId;
    public TFeature? m_WaitForFeature;
    public long m_nWaitForStatus;

    /// <summary>m_boLockEndFrame（ProcessActors movetick 解锁末帧）。</summary>
    public bool m_boLockEndFrame;

    /// <summary>m_boCanDraw（ProcessActors 起始清零，渲染期置位）。</summary>
    public bool m_boCanDraw;

    /// <summary>TActor.FeatureChanged 钩子（SM_FEATURECHANGED 尾部）。</summary>
    public Action? OnFeatureChanged;

    /// <summary>CancelAction 钩子（SM_CHARSTATUSCHANGED 状态位）。</summary>
    public Action? OnCancelAction;

    /// <summary>m_Feature（TActor 持有的形象副本；SM_FEATURECHANGED 整体替换）。</summary>
    public TFeature? m_Feature;

    /// <summary>ProcLastMsg（清队列前处理最后一条；headless 以钩子承载）。</summary>
    public Action? OnProcLastMsg;

    /// <summary>ProcMsg（ProcessActors 处理本条消息；headless 以钩子承载）。</summary>
    public Action? OnProcMsg;

    /// <summary>ProcHurryMsg（魔法消息即时处理；headless 以钩子承载）。</summary>
    public Action? OnProcHurryMsg;

    /// <summary>DoMove（movetick 帧推进；返回 true = 本帧不 ++nIdx）。</summary>
    public Func<int, bool>? OnDoMove;

    /// <summary>Run（帧推进；headless 以钩子承载，缺省走既有 Run(now)）。</summary>
    public Action? OnRun;

    /// <summary>IsIdle（神兽变身换角等待判定）。</summary>
    public Func<bool>? OnIsIdle;

    public void FeatureChanged() => OnFeatureChanged?.Invoke();
    public void CancelAction() => OnCancelAction?.Invoke();
    public void ProcLastMsg() => OnProcLastMsg?.Invoke();
    public void ProcMsg() => OnProcMsg?.Invoke();
    public void ProcHurryMsg() => OnProcHurryMsg?.Invoke();
    public void RunTick()
    {
        if (OnRun != null) OnRun();
        else Run(SceneTime.TickNow());
    }
    public bool DoMove(int step) => OnDoMove?.Invoke(step) ?? false;
    public bool IsIdle => OnIsIdle?.Invoke() ?? true;

    /// <summary>CleanMsgs（清空消息队列）。</summary>
    public void CleanMsgs() => MsgList.Clear();

    /// <summary>ActionChanged 钩子（Delphi 虚方法）。</summary>
    public Action? OnActionChanged;

    /// <summary>TActor.SendMsg 1:1（消息入队尾）。</summary>
    public void SendMsg(TChrMsg msg) => MsgList.Add(msg);

    /// <summary>TActor.GetNextMsg 1:1（4330-4361：取队首并移除；空返回 false）。</summary>
    public bool GetNextMsg(out TChrMsg chrMsg)
    {
        if (MsgList.Count > 0)
        {
            chrMsg = MsgList[0];
            MsgList.RemoveAt(0);
            return true;
        }
        chrMsg = new TChrMsg();
        return false;
    }

    /// <summary>m_boMsgMuch 判定（7433：非主角且积压 ≥2 加速播放）。</summary>
    public bool IsSelf;
    public bool UpdateMsgMuch()
    {
        m_boMsgMuch = !IsSelf && MsgList.Count >= 2;
        return m_boMsgMuch;
    }

    /// <summary>
    /// ReadyAction 核心 1:1（3776-4200）：SM_ALIVE 复活、开血条状态位、旧坐标记录、
    /// 坐标/方向应用（race 95 方向保护、BACKSTEP/100HIT/PUSH 系 dir 打包 step）、
    /// SM_STRUCK 受击帧时长公式、m_nCurrentAction 切换 + CalcActorFrame、死亡标记。
    /// </summary>
    public void ReadyAction(TChrMsg msg)
    {
        m_nActBeforeX = m_nCurrX;
        m_nActBeforeY = m_nCurrY;

        if (msg.Ident == SM_ALIVE)
        {
            m_boDeath = false;
            m_boSkeleton = false;
        }

        if (!m_boDeath && msg.Ident != SM_MAGICFIRE && msg.Ident != SM_MAGICFIRE_FAIL)
        {
            // 移动/转身族携带的状态位（开血条）
            switch (msg.Ident)
            {
                case SM_TURN:
                case SM_WALK:
                case SM_BACKSTEP:
                case SM_RUSH:
                case SM_MAGICMOVE:
                case SM_RUSHKUNG:
                case SM_RUN:
                case SM_HORSERUN:
                case SM_DIGUP:
                case SM_ALIVE:
                case SM_100HIT:
                    if ((msg.State & ActorStates.STATE_OPENHEATH) != 0)
                        m_boOpenHealth = true;
                    else
                        m_boOpenHealth = false;
                    break;
            }

            m_nOldx = m_nCurrX;
            m_nOldy = m_nCurrY;
            m_nOldDir = m_btDir;

            switch (msg.Ident)
            {
                case SM_STRUCK:
                    // 受击帧时长：200 - 等级×5，下限 80，加怪物弯腰延时
                    int n = DelphiRound(200 - m_nLevel * 5.0);
                    m_dwStruckFrameTime = n > 80 ? n : 80;
                    m_dwStruckFrameTime += m_btMonStruckFrameDelayTime;
                    m_dwLastStruckTime = TimeGetTime();
                    break;
                default:
                {
                    if (msg.Ident != SM_MAGICFIRE && msg.Ident != SM_MAGICFIRE_FAIL)
                    {
                        if (msg.Ident != SM_60HIT)
                        {
                            m_nCurrX = msg.X;
                            m_nCurrY = msg.Y;
                        }

                        if (m_btRace != 95)
                        {
                            if (msg.Ident == SM_BACKSTEP || msg.Ident == SM_100HIT)
                            {
                                m_btDir = (byte)(msg.Dir & 0xFF);        // LoByte
                                m_btStep = (byte)((msg.Dir >> 8) & 0xFF); // HiByte
                                if (m_btStep == 0) m_btStep = 1;
                            }
                            else
                            {
                                m_btDir = (byte)msg.Dir;
                            }
                        }
                    }
                    break;
                }
            }

            // 攻击系特效等级
            switch (msg.Ident)
            {
                case SM_HIT:
                case SM_HEAVYHIT:
                case SM_POWERHIT:
                case SM_LONGHIT:
                case SM_WIDEHIT:
                case SM_BIGHIT:
                case SM_60HIT:
                case SM_61HIT:
                case SM_62HIT:
                case SM_66HIT:
                case SM_TWNHIT:
                case SM_CRSHIT:
                case SM_43HIT:
                case SM_SWORDHIT:
                case SM_101HIT:
                case SM_102HIT:
                case SM_103HIT:
                    m_nHitEffectLevel = (int)msg.State;
                    break;

                // 4124：`SM_CUSTOM_HIT001..(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1)` 范围 case
                // ——共 300 个自定义攻击动作，与原工具同样只写 m_nHitEffectLevel
                // （4125；4128-4129 的 m_nHitEndX/Y 赋值已被原文注释掉）
                case int hit when hit >= SM_CUSTOM_HIT001
                    && hit < SM_CUSTOM_HIT001 + CustomMagicCount:
                    m_nHitEffectLevel = (int)msg.State;
                    break;
            }

            // 自定义怪物变脸：攻击族动作强制 SM_HIT
            if (m_nChangeAppr >= 0 &&
                (msg.Ident is >= SM_ATTACK01 and <= SM_ATTACK06))
                m_nCurrentAction = SM_HIT;
            else
                m_nCurrentAction = msg.Ident;

            m_dwCurrentActionTick = TimeGetTime();

            if (msg.Ident != SM_MAGICFIRE && msg.Ident != SM_MAGICFIRE_FAIL)
                CalcActorFrame();

            OnActionChanged?.Invoke();
        }
        else
        {
            if (msg.Ident == SM_SKELETON)
            {
                m_nCurrentAction = msg.Ident;
                m_dwCurrentActionTick = TimeGetTime();
                CalcActorFrame();
                OnActionChanged?.Invoke();
                m_boSkeleton = true;
            }
        }

        if (msg.Ident == SM_DEATH || msg.Ident == SM_NOWDEATH)
        {
            m_boStruckShowNumber = false;
            m_boShowBigHPProgress = false;
            m_boDeath = true;
            m_dwDeathTick = TimeGetTime();
        }
    }

    // ReadyAction 引用的消息码（Grobal2.pas 实值）
    public const int SM_ALIVE = 27;
    public const int SM_SPELL = 17;
    public const int SM_MAGICFIRE = 638;
    public const int SM_MAGICFIRE_FAIL = 639;
    public const int SM_HEAVYHIT = 15;
    public const int SM_BIGHIT = 16;
    public const int SM_POWERHIT = 18;
    public const int SM_LONGHIT = 19;
    public const int SM_WIDEHIT = 24;
    public const int SM_FIREHIT = 8;
    public const int SM_60HIT = 1118;
    public const int SM_61HIT = 1119;
    public const int SM_62HIT = 1120;
    public const int SM_66HIT = 66;
    public const int SM_TWNHIT = 26;
    public const int SM_CRSHIT = 25;
    public const int SM_43HIT = 43;
    public const int SM_SWORDHIT = 56;
    public const int SM_101HIT = 9101;
    public const int SM_102HIT = 9102;
    public const int SM_103HIT = 9103;
    public const int SM_ATTACK01 = 8946;
    public const int SM_ATTACK06 = 8951;

    /// <summary>Grobal2.pas 2215：自定义攻击动作起始码（配合 `CustomMagicCount` 构成 300 个范围 case）。</summary>
    public const int SM_CUSTOM_HIT001 = 11000;

    /// <summary>Grobal2.pas 2815：自定义推挤动作起始码。</summary>
    public const int SM_CUSTOM_PUSH001 = 12000;

    // ---- RunSound / RunActSound 引用的消息码（Grobal2.pas 实值）----

    /// <summary>Grobal2.pas：投掷（`1314` 为旧值注释）。</summary>
    public const int SM_THROW = 65069;

    /// <summary>Grobal2.pas 22：飞斧（半兽统领攻击方式）。</summary>
    public const int SM_FLYAXE = 22;

    /// <summary>Grobal2.pas 23：闪电。</summary>
    public const int SM_LIGHTING = 23;

    /// <summary>Grobal2.pas 21：挖动作的"坐"。</summary>
    public const int SM_DIGDOWN = 21;

    /// <summary>Grobal2.pas 166：开天斩重击（与 `SM_66HIT` 同音）。</summary>
    public const int SM_66HIT1 = 166;

    /// <summary>Grobal2.pas 113：断空斩。</summary>
    public const int SM_113HIT = 113;

    /// <summary>Grobal2.pas 115：血魄一击（战）。</summary>
    public const int SM_115HIT = 115;

    /// <summary>Grobal2.pas 60：自定义动作数量（范围 case 的宽度）。</summary>
    public const int CustomMagicCount = 300;

    /// <summary>4124：`Msg.ident` 是否落在自定义攻击动作范围内。</summary>
    public static bool IsCustomHit(int ident)
        => ident >= SM_CUSTOM_HIT001 && ident < SM_CUSTOM_HIT001 + CustomMagicCount;

    /// <summary>4064：`Msg.ident` 是否落在自定义推挤动作范围内（该支路另取 `m_btDir`/`m_btStep`）。</summary>
    public static bool IsCustomPush(int ident)
        => ident >= SM_CUSTOM_PUSH001 && ident < SM_CUSTOM_PUSH001 + CustomMagicCount;
}
