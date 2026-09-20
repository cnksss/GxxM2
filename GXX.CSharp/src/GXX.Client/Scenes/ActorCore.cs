using System;

namespace GXX.Client.Scenes;

/// <summary>
/// Actor.pas TActor 动画帧状态机核心（批次J44：无渲染数据层）——
/// CalcActorFrame（3482-3774 通用表驱动分支：m_Action 动作表 + 方向×帧区间展开）
/// 与 Run（7391-7509 帧推进：超时步进、到尾复位动作、行走族由移动驱动不受 Run 推进）。
/// SM_* 动作常量取自 Grobal2.pas 1399-1783。
/// </summary>
public partial class TActorCore
{
    // Grobal2.pas 客户端动作消息码
    public const int SM_HORSERUN = 5;
    public const int SM_RUSH = 6;
    public const int SM_RUSHKUNG = 7;
    public const int SM_BACKSTEP = 9;
    public const int SM_TURN = 10;
    public const int SM_WALK = 11;
    public const int SM_RUN = 13;
    public const int SM_HIT = 14;
    public const int SM_DIGUP = 20;
    public const int SM_STRUCK = 31;
    public const int SM_DEATH = 32;
    public const int SM_SKELETON = 33;
    public const int SM_NOWDEATH = 34;
    public const int SM_LIGHTINGEX = 1445;
    public const int SM_MAGICMOVE = 5354;
    public const int SM_100HIT = 9100;

    public long m_nRecogId;
    public byte m_btDir;              // 当前站立方向 0..7
    public byte m_btRace;             // RaceImg
    public ushort m_wAppearance;      // Appr
    public byte m_btStep;             // 冲撞步数
    public int m_nCurrX, m_nCurrY;

    public int m_nCurrentAction;
    public int m_nCurrentFrame = -1;
    public int m_nStartFrame;
    public int m_nEndFrame;
    public uint m_dwFrameTime;
    public uint m_dwStartTime;
    public uint m_dwWarModeTime;
    public int m_nDefFrameCount;
    public int m_nMaxTick;
    public int m_nCurTick;
    public int m_nMoveStep;
    public int m_nBodyOffset;
    public int m_dwStruckFrameTime = 150;

    // ---- Shift 位移与坐标状态（批次J46） ----
    public int m_nShiftX;
    public int m_nShiftY;
    public int m_nRx;
    public int m_nRy;
    public int m_nOldx, m_nOldy, m_nOldDir;
    public int m_nActBeforeX, m_nActBeforeY;

    public bool m_boUseMagic;
    public bool m_boMsgMuch;
    public bool m_boDelActionAfterFinished;
    public bool m_boDelActor;
    public bool m_boFreeActor;
    public uint m_dwDeleteTime;

    public TMonsterAction? m_Action;

    /// <summary>TimeGetTime 接缝（复用 SceneTime.TickNow 保证测试确定性）。</summary>
    public Func<uint> TimeGetTime = () => SceneTime.TickNow();

    /// <summary>ActionEnded 钩子（Delphi 虚方法，具体子类覆写）。</summary>
    public Action? OnActionEnded;

    /// <summary>Delphi GetBack：背向 = (dir+4) mod 8。</summary>
    public static int GetBack(int dir) => (dir + 4) % 8;

    /// <summary>Delphi Round：银行家舍入（Shift 的像素插值依赖）。</summary>
    public static int DelphiRound(double value)
    {
        var rounded = Math.Round(value, MidpointRounding.ToEven);
        return (int)rounded;
    }

    /// <summary>Actor.pas 5000-5167 Shift 1:1（UNITX=48/UNITY=32；像素位移按 Max 比例推进，
    /// 网格补偿 ss 判定与奇数 +1 修正是原文黑边修正形态，逐分支保留）。</summary>
    public virtual void Shift(int dir, int step, int cur, int max)
    {
        const int UNITX = 48;
        const int UNITY = 32;
        const int DR_UP = 0, DR_UPRIGHT = 1, DR_RIGHT = 2, DR_DOWNRIGHT = 3;
        const int DR_DOWN = 4, DR_DOWNLEFT = 5, DR_LEFT = 6, DR_UPLEFT = 7;

        int unx = UNITX * step;
        int uny = UNITY * step;
        if (cur > max) cur = max;
        m_nRx = m_nCurrX;
        m_nRy = m_nCurrY;

        switch (dir)
        {
            case DR_UP:
                {
                    int ss = DelphiRound((double)(max - cur) / max) * step;
                    m_nShiftX = 0;
                    m_nRy = m_nCurrY + ss;
                    if (ss == step)
                        m_nShiftY = -DelphiRound((double)uny / max * cur);
                    else
                        m_nShiftY = DelphiRound((double)uny / max * (max - cur));
                    if (m_nShiftY % 2 != 0) m_nShiftY += 1;
                    break;
                }
            case DR_UPRIGHT:
                {
                    int v = max >= 6 ? 2 : 0;
                    int ss = DelphiRound((double)(max - cur + v) / max) * step;
                    m_nRx = m_nCurrX - ss;
                    m_nRy = m_nCurrY + ss;
                    if (ss == step)
                    {
                        m_nShiftX = DelphiRound((double)unx / max * cur);
                        m_nShiftY = -DelphiRound((double)uny / max * cur);
                    }
                    else
                    {
                        m_nShiftX = -DelphiRound((double)unx / max * (max - cur));
                        m_nShiftY = DelphiRound((double)uny / max * (max - cur));
                    }
                    if (m_nShiftX % 2 != 0) m_nShiftX += 1;
                    if (m_nShiftY % 2 != 0) m_nShiftY += 1;
                    break;
                }
            case DR_RIGHT:
                {
                    int ss = DelphiRound((double)(max - cur) / max) * step;
                    m_nRx = m_nCurrX - ss;
                    if (ss == step)
                        m_nShiftX = DelphiRound((double)unx / max * cur);
                    else
                        m_nShiftX = -DelphiRound((double)unx / max * (max - cur));
                    if (m_nShiftX % 2 != 0) m_nShiftX += 1;
                    m_nShiftY = 0;
                    break;
                }
            case DR_DOWNRIGHT:
                {
                    int v = max >= 6 ? 2 : 0;
                    if (step == 3)
                        v = 1;
                    else if (step == 4)
                        v = cur == 3 ? 0 : 1;
                    else if (step == 5) // 追心刺推动5格黑边修正
                        v = cur == 4 ? 1 : 0;

                    int ss = DelphiRound((double)(max - cur - v) / max) * step;
                    m_nRx = m_nCurrX - ss;
                    m_nRy = m_nCurrY - ss;
                    if (ss == step)
                    {
                        m_nShiftX = DelphiRound((double)unx / max * cur);
                        m_nShiftY = DelphiRound((double)uny / max * cur);
                    }
                    else
                    {
                        m_nShiftX = -DelphiRound((double)unx / max * (max - cur));
                        m_nShiftY = -DelphiRound((double)uny / max * (max - cur));
                    }
                    if (m_nShiftX % 2 != 0) m_nShiftX += 1;
                    if (m_nShiftY % 2 != 0) m_nShiftY += 1;
                    break;
                }
            case DR_DOWN:
                {
                    int v = max >= 6 ? 1 : 0;
                    int ss = DelphiRound((double)(max - cur - v) / max) * step;
                    m_nShiftX = 0;
                    m_nRy = m_nCurrY - ss;
                    if (ss == step)
                        m_nShiftY = DelphiRound((double)uny / max * cur);
                    else
                        m_nShiftY = -DelphiRound((double)uny / max * (max - cur));
                    if (m_nShiftY % 2 != 0) m_nShiftY += 1;
                    break;
                }
            case DR_DOWNLEFT:
                {
                    int v = max >= 6 ? 2 : 0;
                    int ss = DelphiRound((double)(max - cur - v) / max) * step;
                    m_nRx = m_nCurrX + ss;
                    m_nRy = m_nCurrY - ss;
                    if (ss == step)
                    {
                        m_nShiftX = -DelphiRound((double)unx / max * cur);
                        m_nShiftY = DelphiRound((double)uny / max * cur);
                    }
                    else
                    {
                        m_nShiftX = DelphiRound((double)unx / max * (max - cur));
                        m_nShiftY = -DelphiRound((double)uny / max * (max - cur));
                    }
                    if (m_nShiftX % 2 != 0) m_nShiftX += 1;
                    if (m_nShiftY % 2 != 0) m_nShiftY += 1;
                    break;
                }
            case DR_LEFT:
                {
                    int ss = DelphiRound((double)(max - cur) / max) * step;
                    m_nRx = m_nCurrX + ss;
                    if (ss == step)
                        m_nShiftX = -DelphiRound((double)unx / max * cur);
                    else
                        m_nShiftX = DelphiRound((double)unx / max * (max - cur));
                    if (m_nShiftX % 2 != 0) m_nShiftX += 1;
                    m_nShiftY = 0;
                    break;
                }
            case DR_UPLEFT:
                {
                    int v = max >= 6 ? 2 : 0;
                    if (step == 4)
                        v = cur == 3 ? 0 : 1;
                    else if (step == 5) // 追心刺推动5格黑边修正
                        v = cur == 4 ? 1 : 0;

                    int ss = DelphiRound((double)(max - cur + v) / max) * step;
                    m_nRx = m_nCurrX + ss;
                    m_nRy = m_nCurrY + ss;
                    if (ss == step)
                    {
                        m_nShiftX = -DelphiRound((double)unx / max * cur);
                        m_nShiftY = -DelphiRound((double)uny / max * cur);
                    }
                    else
                    {
                        m_nShiftX = DelphiRound((double)unx / max * (max - cur));
                        m_nShiftY = DelphiRound((double)uny / max * (max - cur));
                    }
                    if (m_nShiftX % 2 != 0) m_nShiftX += 1;
                    if (m_nShiftY % 2 != 0) m_nShiftY += 1;
                    break;
                }
        }
    }

    /// <summary>
    /// Actor.pas 3643-3760 CalcActorFrame 通用表驱动分支（race 156 自定义怪分支随后续批次）。
    /// ★ 车道 p7-client-virtual：原文 TActor.CalcActorFrame 为虚方法（子类 override），此处补 `virtual`。
    /// </summary>
    public virtual void CalcActorFrame()
    {
        m_boUseMagic = false;
        m_nCurrentFrame = -1;

        m_nBodyOffset = ActorOffsets.GetOffset(m_wAppearance);
        m_Action = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);
        if (m_Action == null)
            return;
        var action = m_Action.Value;

        switch (m_nCurrentAction)
        {
            case SM_TURN:
                m_nStartFrame = action.ActStand.start + m_btDir * (action.ActStand.frame + action.ActStand.skip);
                m_nEndFrame = m_nStartFrame + action.ActStand.frame - 1;
                m_dwFrameTime = action.ActStand.ftime;
                m_dwStartTime = TimeGetTime();
                m_nDefFrameCount = action.ActStand.frame;
                Shift(m_btDir, 0, 0, 1);
                break;
            case SM_WALK:
            case SM_RUSH:
            case SM_RUSHKUNG:
            case SM_BACKSTEP:
                m_nStartFrame = action.ActWalk.start + m_btDir * (action.ActWalk.frame + action.ActWalk.skip);
                m_nEndFrame = m_nStartFrame + action.ActWalk.frame - 1;
                m_dwFrameTime = action.ActWalk.ftime;
                m_dwStartTime = TimeGetTime();
                m_nMaxTick = action.ActWalk.usetick;
                m_nCurTick = 0;
                m_nMoveStep = 1;
                if (m_nCurrentAction == SM_BACKSTEP)
                {
                    m_nMoveStep = m_btStep;
                    Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
                }
                else
                    Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
                break;
            case SM_LIGHTINGEX:
                m_nStartFrame = action.ActAttack.start + m_btDir * (action.ActAttack.frame + action.ActAttack.skip);
                m_nEndFrame = m_nStartFrame + action.ActAttack.frame - 1;
                m_dwFrameTime = action.ActAttack.ftime;
                m_dwStartTime = TimeGetTime();
                m_dwWarModeTime = TimeGetTime();
                Shift(m_btDir, 0, 0, 1);
                break;
            case SM_HIT:
                m_nStartFrame = action.ActAttack.start + m_btDir * (action.ActAttack.frame + action.ActAttack.skip);
                m_nEndFrame = m_nStartFrame + action.ActAttack.frame - 1;
                m_dwFrameTime = action.ActAttack.ftime;
                m_dwStartTime = TimeGetTime();
                m_dwWarModeTime = TimeGetTime();
                Shift(m_btDir, 0, 0, 1);
                break;
            case SM_STRUCK:
                m_nStartFrame = action.ActStruck.start + m_btDir * (action.ActStruck.frame + action.ActStruck.skip);
                m_nEndFrame = m_nStartFrame + action.ActStruck.frame - 1;
                m_dwFrameTime = (uint)m_dwStruckFrameTime;
                m_dwStartTime = TimeGetTime();
                Shift(m_btDir, 0, 0, 1);
                break;
            case SM_DEATH:
                m_nStartFrame = action.ActDie.start + m_btDir * (action.ActDie.frame + action.ActDie.skip);
                m_nEndFrame = m_nStartFrame + action.ActDie.frame - 1;
                m_nStartFrame = m_nEndFrame; // 原文：尸体停在末帧
                m_dwFrameTime = action.ActDie.ftime;
                m_dwStartTime = TimeGetTime();
                break;
            case SM_NOWDEATH:
                m_nStartFrame = action.ActDie.start + m_btDir * (action.ActDie.frame + action.ActDie.skip);
                m_nEndFrame = m_nStartFrame + action.ActDie.frame - 1;
                m_dwFrameTime = action.ActDie.ftime;
                m_dwStartTime = TimeGetTime();
                break;
            case SM_SKELETON:
                m_nStartFrame = action.ActDeath.start + m_btDir;
                m_nEndFrame = m_nStartFrame + action.ActDeath.frame - 1;
                m_dwFrameTime = action.ActDeath.ftime;
                m_dwStartTime = TimeGetTime();
                break;
        }
    }

    /// <summary>Run 顶部行走族守卫（7416-7428）：移动动作帧由移动逻辑驱动，Run 不推进。</summary>
    public static bool IsMoveAction(int currentAction)
        => currentAction is SM_WALK or SM_BACKSTEP or SM_RUN or SM_HORSERUN or SM_MAGICMOVE
            or SM_RUSH or SM_RUSHKUNG or SM_100HIT;

    /// <summary>
    /// Actor.pas 7444-7509 Run 帧推进核心（非魔法路径）。
    /// ★ 车道 p7-client-virtual：原文 TActor.Run 为虚方法（子类 override），此处补 `virtual`。
    /// </summary>
    public virtual void Run(uint now)
    {
        if (IsMoveAction(m_nCurrentAction))
            return;

        if (m_nCurrentAction != 0)
        {
            if (m_nCurrentFrame < m_nStartFrame || m_nCurrentFrame > m_nEndFrame)
                m_nCurrentFrame = m_nStartFrame;

            // 消息积压加速播放（7454-7457）：×2/3；魔法路径 /1.8 随魔法批次接入
            uint frameTimetime = m_dwFrameTime;
            if (m_boMsgMuch && !(m_btRace == 50))
                frameTimetime = (uint)Math.Round(m_dwFrameTime * 2.0 / 3.0);

            if (now - m_dwStartTime > frameTimetime)
            {
                if (m_nCurrentFrame < m_nEndFrame)
                {
                    m_nCurrentFrame++;
                    m_dwStartTime = now;
                }
                else
                {
                    if (m_boDelActionAfterFinished)
                    {
                        m_dwDeleteTime = now;
                        m_boDelActor = true;
                        m_boFreeActor = true;
                    }
                    OnActionEnded?.Invoke();
                    m_nCurrentAction = 0;
                    m_boUseMagic = false;
                }
            }
        }
    }
}
