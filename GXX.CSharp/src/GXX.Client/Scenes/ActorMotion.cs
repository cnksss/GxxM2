using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// Actor.pas 姿态/动作决策层（批次J74）——原文中「当前该显示哪一帧、能不能走、
/// 动作算不算结束」这一整族纯判定函数，全部与渲染句柄无关，可完整 1:1 移植：
/// IsIdle(4871-4880) / ActionFinished(4882-4889) / GetNextHitTime(4891-4912) /
/// CanWalk(4953-4963) / CanRun(4965-4980) / Strucked(4982-4993) /
/// GetDefaultFrame(6136-6215，自定义怪 + 动作表双分支) / DefaultMotion(6217-6232) /
/// RunFrameAction(7097-7099) / ActionChanged(7101-7104) / ActionEnded(7106-7118) /
/// CanCancelAction(8395-8401) / CancelAction(8403-8407) / CleanCharMapSetting(8409-8420) /
/// MoveFail(8366-8393)。
/// 依赖注入项（无真实窗口/配置对象）：g_ClientConfig 相关值、g_ChrAction、g_boMapMovingWait、
/// g_dwLatestSpellTick/g_dwMagicPKDelayTime、frmMain.StartContinuousMagicAttack。
/// </summary>
public partial class TActorCore
{
    // ===================== 依赖注入接缝 =====================

    /// <summary>g_ClientConfig.dwHitFrameTime（GetNextHitTime 基准）。</summary>
    public static int ClientConfig_dwHitFrameTime = 0;

    /// <summary>g_ClientConfig.dwIncSpeedDecInterval（攻速折算步长）。</summary>
    public static int ClientConfig_dwIncSpeedDecInterval = 0;

    /// <summary>g_boAttackSlow（腕力超限，仅主角 +1500）。</summary>
    public static bool ClientConfig_boAttackSlow;

    /// <summary>g_ChrAction（caNone 时禁止走/跑）。</summary>
    public static int ChrAction = 1;                 // caNone = 0

    /// <summary>g_boMapMovingWait（过图等待中禁止移动）。</summary>
    public static bool boMapMovingWait;

    /// <summary>MyGetTickCount - g_dwLatestSpellTick &lt; g_dwMagicPKDelayTime 时禁走。</summary>
    public static long LatestSpellTick;
    public static long MagicPKDelayTime;

    /// <summary>MyGetTickCount 提供者（GetNextHitTime/CanWalk 时间基准）。</summary>
    public static Func<long> MyGetTickCountFn = () => SceneTime.TickNow();

    /// <summary>frmMain.StartContinuousMagicAttack（ActionEnded 连击续招）。</summary>
    public static Action? StartContinuousMagicAttackFn;

    /// <summary>CanWalk/CanRun 的 caNone 常量。</summary>
    public const int caNone = 0;

    /// <summary>RUN_MINHEALTH（CanRun 血量下限）。</summary>
    public const int RUN_MINHEALTH = 10;

    /// <summary>STATE_WARMODE 位（战技锁定 $00010000）。</summary>
    public const int STATE_SKILL_LOCK = 0x00010000;

    /// <summary>ActionEnded 连击判定用的魔法效果号字段（m_CurMagic.EffectNumber）。</summary>
    public int m_CurMagicEffectNumber;

    /// <summary>m_dwActionEndTime（ActionEnded 打点）。</summary>
    public uint m_dwActionEndTime;

    // ---- GetDefaultFrame / DefaultMotion 引用字段 ----
    /// <summary>m_nCurrentDefFrame（站立帧游标；&lt;0 或越界一律回落 0）。</summary>
    public int m_nCurrentDefFrame = -1;

    /// <summary>m_boReverseFrame（DefaultMotion 起始清零）。</summary>
    public bool m_boReverseFrame;

    /// <summary>m_boCustomMagicNoAction（战斗模式 4 秒超时一并清除）。</summary>
    public bool m_boCustomMagicNoAction;

    /// <summary>m_Abil.HP（CanRun 血量下限判定）。</summary>
    public int m_nAbilHP;

    /// <summary>m_nAttackSpeed（GetNextHitTime 攻速折算）。</summary>
    public int m_nAttackSpeed;

    /// <summary>g_ActionCode（MoveFail 无恢复坐标时的回退判定）。</summary>
    public static int ActionCode;

    /// <summary>g_boCanDrawTileMap（MoveFail 恢复地图绘制）。</summary>
    public static Action? SetCanDrawTileMapFn;

    /// <summary>m_boUseEffect（CanCancelAction 门）。</summary>
    public bool m_boUseEffect;

    /// <summary>主角引用（ActionEnded / MoveFail / CleanCharMapSetting 的 g_MySelf 语义）。</summary>
    public static TActorCore? MySelfRef;

    /// <summary>是否为 g_MySelf。</summary>
    public bool IsMySelf => ReferenceEquals(this, MySelfRef);

    // ===================== IsIdle / ActionFinished =====================

    /// <summary>IsIdle 1:1（4871-4880）：无动作且消息队列为空。</summary>
    public bool ComputeIsIdle()
        => m_nCurrentAction == 0 && MsgList.Count == 0;

    /// <summary>ActionFinished 1:1（4882-4889）：无动作 或 已到倒数第二帧。</summary>
    public bool ActionFinished()
        => m_nCurrentAction == 0 || m_nCurrentFrame >= m_nEndFrame - 1;

    // ===================== GetNextHitTime =====================

    /// <summary>
    /// GetNextHitTime 1:1（4891-4912）：基准 dwHitFrameTime → 主角腕力超限 +1500 →
    /// 攻速按 dwIncSpeedDecInterval 递减（Int64 运算后 Max(0)）→ 下限 100。
    /// </summary>
    public int GetNextHitTime()
    {
        int nextHitTime = Math.Max(0, ClientConfig_dwHitFrameTime);

        if (ClientConfig_boAttackSlow && IsMySelf)
            nextHitTime += 1500;

        // 原文嵌套 if m_nAttackSpeed <> 0（外层判断冗余，保留双层结构）
        if (m_nAttackSpeed != 0)
        {
            if (m_nAttackSpeed != 0)
                nextHitTime = (int)Math.Max((long)nextHitTime - (long)m_nAttackSpeed * ClientConfig_dwIncSpeedDecInterval, 0);
        }

        nextHitTime = Math.Max(0, nextHitTime);
        if (nextHitTime < 100)
            nextHitTime = 100;

        return nextHitTime;
    }

    // ===================== CanWalk / CanRun / Strucked =====================

    /// <summary>
    /// CanWalk 1:1（4953-4963）：魔法 PK 延迟内 / caNone / 过图等待 → -1；
    /// 战技锁定状态位 → 强制 -1。
    /// </summary>
    public int CanWalk()
    {
        int result;
        if ((MyGetTickCountFn() - LatestSpellTick < MagicPKDelayTime)
            || (ChrAction == caNone)
            || boMapMovingWait)
            result = -1;
        else
            result = 1;

        if ((m_nState & STATE_SKILL_LOCK) != 0)
            result = -1;

        return result;
    }

    /// <summary>CanRun 1:1（4965-4980）：HP &lt; RUN_MINHEALTH / caNone / 过图等待 → -1；战技锁定 → -1。</summary>
    public int CanRun()
    {
        int result = 1;
        if ((m_nAbilHP < RUN_MINHEALTH) || (ChrAction == caNone) || boMapMovingWait)
            result = -1;

        if ((m_nState & STATE_SKILL_LOCK) != 0)
            result = -1;

        return result;
    }

    /// <summary>Strucked 1:1（4982-4993）：消息队列中存在 SM_STRUCK。</summary>
    public bool Strucked()
    {
        for (int i = 0; i < MsgList.Count; i++)
        {
            if (MsgList[i].Ident == SM_STRUCK)
                return true;
        }
        return false;
    }

    // ===================== GetDefaultFrame =====================

    /// <summary>g_CustomMonsterConfig 查找接缝（按 wMonsterAppr 线性查找，首命中即返回）。</summary>
    public static Func<int, TClientCustomMonsterConfig?>? CustomMonsterConfigResolver;

    /// <summary>自定义怪 Actions[索引] 读取（TMonsterClientActionType 下标）。</summary>
    private static TMonsterClientAction CustomAction(in TClientCustomMonsterConfig cfg, TMonsterClientActionType type)
        => cfg.Actions[(int)type];

    /// <summary>
    /// GetDefaultFrame 1:1（6136-6215）：race 156 且 ChangeAppr ≥ 0 走自定义怪配置分支
    /// （死亡 / 石化复活 / 站立三态），否则走 GetRaceByPM 动作表分支。
    /// 副作用：非自定义怪分支写 m_nDefFrameCount = ActStand.frame。
    /// ★ 车道 p7-client-virtual：原文 TActor.GetDefaultFrame 为虚方法（子类 override），此处补 `virtual`。
    /// </summary>
    public virtual int GetDefaultFrame(bool wmode)
    {
        if (m_btRace == 156 && m_nChangeAppr >= 0)
        {
            int result = 0;
            TClientCustomMonsterConfig? monsterConfig = null;

            var found = CustomMonsterConfigResolver?.Invoke(m_nChangeAppr);
            if (found.HasValue)
                monsterConfig = found;

            if (monsterConfig.HasValue)
            {
                var cfg = monsterConfig.Value;
                if (m_boDeath)
                {
                    if (m_boSkeleton)
                        result = CustomAction(cfg, TMonsterClientActionType.matDie).StartIndex;
                    else
                    {
                        var die = CustomAction(cfg, TMonsterClientActionType.matDie);
                        int tempDir = die.CalcDir != 0 ? m_btDir : 0;
                        result = die.StartIndex + tempDir * (die.PlayCount + die.EmptyCount) + (die.PlayCount - 1);
                    }
                }
                else
                {
                    if ((m_nState & (int)ActorStates.STATE_STONE_MODE) != 0)
                    {
                        var revive = CustomAction(cfg, TMonsterClientActionType.matStoneRevive);
                        int tempDir = revive.CalcDir != 0 ? m_btDir : 0;
                        result = revive.StartIndex + tempDir * (revive.PlayCount + revive.EmptyCount);
                    }
                    else
                    {
                        var stand = CustomAction(cfg, TMonsterClientActionType.matStand);
                        int cf;
                        if (m_nCurrentDefFrame < 0)
                            cf = 0;
                        else if (m_nCurrentDefFrame >= stand.PlayCount)
                            cf = 0;
                        else
                            cf = m_nCurrentDefFrame;

                        int tempDir = stand.CalcDir != 0 ? m_btDir : 0;
                        result = stand.StartIndex + tempDir * (stand.PlayCount + stand.EmptyCount) + cf;
                    }
                }
            }

            return result;
        }

        // ---- 动作表分支 ----
        int r = 0;
        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);
        if (pmOpt == null)
            return r;                                   // 原文 Exit（Result 保持 0）
        var pm = pmOpt.Value;

        if (m_boDeath)
        {
            if (m_boSkeleton)
                r = pm.ActDeath.start;
            else
                r = pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
        }
        else
        {
            m_nDefFrameCount = pm.ActStand.frame;
            int cf;
            if (m_nCurrentDefFrame < 0)
                cf = 0;
            else if (m_nCurrentDefFrame >= pm.ActStand.frame)
                cf = 0;
            else
                cf = m_nCurrentDefFrame;
            r = pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
        }

        return r;
    }

    /// <summary>
    /// DefaultMotion 1:1（6217-6232）：清反向帧 → 战斗模式 4 秒超时自动退出 →
    /// 取默认帧 → Shift(dir,0,1,1) → 返回帧号是否变化并写回 m_nCurrentFrame。
    /// ★ 车道 p17-client-actor 切片3d：原文 1750 声明为 <c>virtual</c>
    /// （<c>THumActor.DefaultMotion</c> 2016 <c>override</c>），此处补 <c>virtual</c>
    /// —— 否则子类覆写只能 <c>new</c> 隐藏，而调用点全是基类静态类型（台帐 §18.8）。
    /// </summary>
    public virtual bool DefaultMotion()
    {
        m_boReverseFrame = false;
        if (m_boWarMode)
        {
            if (SceneTime.TickNow() - m_dwWarModeTime > 4 * 1000)
            {
                m_boWarMode = false;
                m_boCustomMagicNoAction = false;
            }
        }

        int nCurrentFrame = GetDefaultFrame(m_boWarMode);
        Shift(m_btDir, 0, 1, 1);
        bool result = nCurrentFrame != m_nCurrentFrame;
        m_nCurrentFrame = nCurrentFrame;
        return result;
    }

    // ===================== RunFrameAction / ActionChanged / ActionEnded =====================

    /// <summary>
    /// RunFrameAction 1:1（7097-7099）：原文空函数体。
    /// ★ 车道 p17-client-actor 切片3d：原文 1821 声明为 <c>virtual</c>
    /// （<c>THumActor.RunFrameAction</c> 2025 <c>override</c>，13310-13353 有实体），此处补 <c>virtual</c>。
    /// </summary>
    public virtual void RunFrameAction(int frame) { }

    /// <summary>ActionChanged 1:1（7101-7104）：原文空函数体（虚方法，子类覆写）。</summary>
    public virtual void ComputeActionChanged() { }

    /// <summary>
    /// ActionEnded 1:1（7106-7118）：仅主角且（当前动作 SM_SPELL 且效果号 104..111）触发连击，
    /// 其余分支原文为空；末尾恒写 m_dwActionEndTime。
    /// </summary>
    public void RunActionEnded()
    {
        if (IsMySelf)
        {
            if (m_nCurrentAction == SM_SPELL && m_CurMagicEffectNumber is >= 104 and <= 111)
                StartContinuousMagicAttackFn?.Invoke();
            // else 分支原文仅注释
        }

        m_dwActionEndTime = SceneTime.TickNow();
    }

    // ===================== CanCancelAction / CancelAction =====================

    /// <summary>CanCancelAction 1:1（8395-8401）：仅 SM_HIT 且未用特效时可取消。</summary>
    public bool CanCancelAction()
    {
        bool result = false;
        if (m_nCurrentAction == SM_HIT)
        {
            if (!m_boUseEffect)
                result = true;
        }
        return result;
    }

    /// <summary>CancelAction 1:1（8403-8407）：动作清零并锁定末帧。</summary>
    public void RunCancelAction()
    {
        m_nCurrentAction = 0;
        m_boLockEndFrame = true;
    }

    // ===================== CleanCharMapSetting =====================

    /// <summary>
    /// CleanCharMapSetting 1:1（8409-8420）：主角与自身坐标对齐、清动作与帧、
    /// 清理用户消息队列。原文全部写 g_MySelf 且 this 为主角（调用方语义）。
    /// </summary>
    public void CleanCharMapSetting(int x, int y)
    {
        var self = MySelfRef ?? this;
        self.m_nCurrX = x;
        self.m_nCurrY = y;
        self.m_nRx = x;
        self.m_nRy = y;
        m_nOldx = x;
        m_nOldy = y;
        m_nCurrentAction = 0;
        m_nCurrentFrame = -1;
        CleanUserMsgs();
    }

    // ===================== MoveFail =====================

    /// <summary>
    /// MoveFail 1:1（8366-8393）：动作清零 + 锁末帧；有三参恢复值则写主角坐标/方向，
    /// 否则按 g_ActionCode 在行走族回退到 m_nOldx/Oldy/OldDir；
    /// 清用户消息 → 恢复地图绘制 → ActionChanged。
    /// </summary>
    public void MoveFail(int nRestoreX, int nRestoreY, int nDir)
    {
        m_nCurrentAction = 0;
        m_boLockEndFrame = true;

        var self = MySelfRef;
        if (nRestoreX > -1 && nRestoreY > -1 && nDir > -1)
        {
            if (self != null)
            {
                self.m_nCurrX = nRestoreX;
                self.m_nCurrY = nRestoreY;
                self.m_btDir = (byte)nDir;
            }
        }
        else
        {
            if (ActionCode is CM_WALK or CM_RUN or CM_HORSERUN)
            {
                if (self != null)
                {
                    self.m_nCurrX = m_nOldx;
                    self.m_nCurrY = m_nOldy;
                    self.m_btDir = (byte)m_nOldDir;
                }
            }
        }

        CleanUserMsgs();
        SetCanDrawTileMapFn?.Invoke();
        ComputeActionChanged();
    }
}

/// <summary>
/// TActor.CleanUserMsgs 1:1（3411-3480 双实现）：删除队列中的用户输入消息
/// （CM_WALK/CM_RUN/CM_HORSERUN/CM_TURN/CM_BACKSTEP/CM_ATTACK…），保留服务器下发的动作消息。
/// headless 版直接按 ident 判定，语义与原文两个重载一致。
/// </summary>
public partial class TActorCore
{
    /// <summary>CM_* 用户消息码（Common/Grobal2.pas 实值；原文 CleanUserMsgs 清除集）。</summary>
    public const int CM_HORSERUN = 3009;
    public const int CM_TURN = 3010;
    public const int CM_WALK = 3011;
    public const int CM_RUN = 3013;
    public const int CM_HIT = 3014;
    public const int CM_SPELL = 3017;
    public const int CM_POWERHIT = 3018;
    public const int CM_LONGHIT = 3019;
    public const int CM_WIDEHIT = 3024;
    public const int CM_FIREHIT = 3025;
    public const int CM_DROPITEM = 1000;
    public const int CM_PICKUP = 1001;
    public const int CM_EAT = 1006;

    /// <summary>CM_BACKSTEP（原文 CleanUserMsgs 集内，Grobal2 未定义同名常量 → 按客户端后撤码）。</summary>
    public const int CM_BACKSTEP = 3012;

    /// <summary>用户输入消息集合（CleanUserMsgs 清除对象）。</summary>
    public static readonly HashSet<int> UserMsgIdents = new()
    {
        CM_TURN, CM_WALK, CM_RUN, CM_HORSERUN, CM_BACKSTEP, CM_HIT, CM_POWERHIT,
        CM_LONGHIT, CM_WIDEHIT, CM_FIREHIT, CM_PICKUP, CM_DROPITEM, CM_EAT, CM_SPELL,
    };

    /// <summary>CleanUserMsgs 1:1（3411-3480）：原地过滤，删除全部用户输入消息。</summary>
    public void CleanUserMsgs()
    {
        for (int i = MsgList.Count - 1; i >= 0; i--)
        {
            if (UserMsgIdents.Contains(MsgList[i].Ident))
                MsgList.RemoveAt(i);
        }
    }
}
