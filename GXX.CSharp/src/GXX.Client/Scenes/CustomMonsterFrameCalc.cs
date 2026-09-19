using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// Actor.pas `TActor.CalcActorFrame`（3482-3774）的 **race 156 自定义怪分支**（3491-3642）
/// 1:1 移植（批次J92）。该分支在 `m_btRace = 156 and m_nChangeAppr >= 0` 时**取代**通用动作表分支，
/// 帧区间全部来自 `g_CustomMonsterConfig` 中按 `wMonsterAppr` 匹配到的配置项。
///
/// **贯穿全分支的 TempDir 模式（原文重复 10 次，逐字一致）**：
/// `ClientAction.CalcDir` 为真 → `TempDir := m_btDir`，否则 `TempDir := 0`；
/// 帧号一律为 `StartIndex + TempDir * (PlayCount + EmptyCount)`。
/// </summary>
public static class CustomMonsterFrameCalc
{
    /// <summary>自定义怪（race 156）的种族编号。</summary>
    public const int CustomMonsterRace = 156;

    /// <summary>
    /// 3491：进入自定义怪分支的条件——`m_btRace in [156] and m_nChangeAppr >= 0`。
    /// 注意 `m_nChangeAppr` 用 `>= 0` 而非 `<> 0`（0 也是有效外观号）。
    /// </summary>
    public static bool IsCustomMonsterBranch(int btRace, int changeAppr)
        => btRace == CustomMonsterRace && changeAppr >= 0;

    /// <summary>原文的 TempDir 计算：`CalcDir` 为真取自身方向，否则恒取 0。</summary>
    public static int TempDir(bool calcDir, int btDir) => calcDir ? btDir : 0;

    /// <summary>帧号基址 = `StartIndex + TempDir * (PlayCount + EmptyCount)`。</summary>
    public static int BaseFrame(in TMonsterClientAction a, int btDir)
        => a.StartIndex + TempDir(a.CalcDir != 0, btDir) * (a.PlayCount + a.EmptyCount);

    /// <summary>帧推进规划结果（对应原文本分支写入各字段的组合）。</summary>
    public sealed record FramePlan(
        int StartFrame, int EndFrame, int FrameTime,
        int DefFrameCount, int MaxTick, int MoveStep,
        bool WarModeTime, bool ResetState, bool ResetStruck,
        string Kind);

    /// <summary>
    /// 3491-3642 完整分支 1:1。
    /// 返回 null 表示"未匹配到配置"或"该动作在原文中为空实现"（SM_LIGHTINGEX / SM_SKELETON）。
    /// </summary>
    /// <param name="actions">`MonsterConfig.Actions[mat*]` 读取接缝。</param>
    public static FramePlan? Plan(
        int m_nCurrentAction,
        int m_btDir, int m_btStep,
        bool stateStoneMode,
        int dwStruckFrameTime,
        Func<TMonsterClientActionType, TMonsterClientAction> actions,
        out int moveStepOut)
    {
        moveStepOut = 1;

        if (stateStoneMode && m_nCurrentAction is 0 or TActorCore.SM_TURN)
        {
            // 3507-3519：站立/转身 + 石化态 → 复活动作，且 **StartFrame = EndFrame**（单帧定格）
            var a = actions(TMonsterClientActionType.matStoneRevive);
            int sf = BaseFrame(a, m_btDir);
            return new FramePlan(sf, sf, a.PlayTime, a.PlayCount, 0, 0,
                false, false, false, "StandStoneRevive");
        }

        switch (m_nCurrentAction)
        {
            case 0:
            case TActorCore.SM_TURN:
            {
                // 3522-3535：站立/转身（非石化）→ matStand
                var a = actions(TMonsterClientActionType.matStand);
                int sf = BaseFrame(a, m_btDir);
                return new FramePlan(sf, sf + a.PlayCount - 1, a.PlayTime, a.PlayCount, 0, 0,
                    false, false, false, "Stand");
            }

            case TActorCore.SM_WALK:
            case TActorCore.SM_RUSH:
            case TActorCore.SM_RUSHKUNG:
            case TActorCore.SM_BACKSTEP:
            {
                // 3537-3559：行走族 → matWalk
                // **注意 3549 用的是通用动作表 HA.ActWalk.usetick，而非自定义怪配置的字段**
                var a = actions(TMonsterClientActionType.matWalk);
                int sf = BaseFrame(a, m_btDir);
                int ef = sf + a.PlayCount - 1;

                int step = 1;
                if (m_nCurrentAction == TActorCore.SM_BACKSTEP)
                    step = m_btStep;
                moveStepOut = step;

                return new FramePlan(sf, ef, a.PlayTime, 0,
                    CustomMonsterWalkUseTick, step, false, false, false, "Walk");
            }

            case TActorCore.SM_DIGUP:
            {
                // 3560-3579：出土 → matStoneRevive，且**清零 m_nState**
                var a = actions(TMonsterClientActionType.matStoneRevive);
                int sf = BaseFrame(a, m_btDir);
                return new FramePlan(sf, sf + a.PlayCount - 1, a.PlayTime, a.PlayCount, 0, 0,
                    false, true, false, "DigUp");
            }

            case TActorCore.SM_LIGHTINGEX:
                // 3580-3582：空实现
                return null;

            case TActorCore.SM_HIT:
            {
                // 3583-3596：攻击 → matDefAttack（注意是 **Def**Attack）
                var a = actions(TMonsterClientActionType.matDefAttack);
                int sf = BaseFrame(a, m_btDir);
                return new FramePlan(sf, sf + a.PlayCount - 1, a.PlayTime, 0, 0, 0,
                    true, false, false, "Hit");
            }

            case TActorCore.SM_STRUCK:
            {
                // 3597-3611：受击 → matStruck，**帧时间用 m_dwStruckFrameTime**，且清零受击计数
                var a = actions(TMonsterClientActionType.matStruck);
                int sf = BaseFrame(a, m_btDir);
                return new FramePlan(sf, sf + a.PlayCount - 1, dwStruckFrameTime, 0, 0, 0,
                    false, false, true, "Struck");
            }

            case TActorCore.SM_DEATH:
            {
                // 3612-3623：死亡 → matDie，**StartFrame 直接落在末帧**（+ PlayCount - 1）
                var a = actions(TMonsterClientActionType.matDie);
                int sf = BaseFrame(a, m_btDir) + a.PlayCount - 1;
                return new FramePlan(sf, sf, a.PlayTime, 0, 0, 0,
                    false, false, false, "Death");
            }

            case TActorCore.SM_NOWDEATH:
            {
                // 3624-3635：就地死亡 → matDie，从首帧正常播放到末帧
                var a = actions(TMonsterClientActionType.matDie);
                int sf = BaseFrame(a, m_btDir);
                return new FramePlan(sf, sf + a.PlayCount - 1, a.PlayTime, 0, 0, 0,
                    false, false, false, "NowDeath");
            }

            case TActorCore.SM_SKELETON:
                // 3636-3638：空实现
                return null;

            default:
                return null;
        }
    }

    /// <summary>
    /// 3549：行走族写入 `m_nMaxTick := HA.ActWalk.usetick` —— 这里读的是**通用动作表**的
    /// `ActWalk.usetick`，而不是自定义怪配置的字段（原文如此；自定义怪配置无对应字段）。
    /// 本实现以常量 0 占位并由调用方从通用动作表取值，故此值仅表示"需要写 m_nMaxTick"。
    /// </summary>
    public const int CustomMonsterWalkUseTick = 0;
}
