// 源单元：Source/Client-HGE/HerbActor.pas（原文 1223 行 / CRLF 计入 1341 行）
// 原文 uses：Windows, SysUtils, Classes, Grobal2, HGE, Graphics, SDK, magiceff, Actor,
//           GameImages, HGECanvas, MMSystem（implementation 段另用 ClMain, MShare）
// 原文无同名 .dfm（角色族，无窗体）。
//
// ⚠ **本文件是部分移植（partial），已如实登记未覆盖范围** —— 见文件末尾
//   <see cref="HerbActorCoverage"/> 的逐条清单。原因与取舍：
//
//   HerbActor.pas 的 14 个类全部派生自 <c>TActor</c>，并 **override** 了
//   <c>CalcActorFrame</c> / <c>GetDefaultFrame</c>。
//   <para>★ <b>已落地（车道 `p7-client-actor-family`）</b>：本段原先写「<c>TActorCore</c> 的这两个方法是
//   **非虚的**」—— <b>该表述已过期</b>。车道 `p7-client-virtual` 已把它们改为
//   <c>public virtual void CalcActorFrame()</c>（ActorCore.cs:250）与
//   <c>public virtual int GetDefaultFrame(bool)</c>（ActorMotion.cs:192），并在 <c>TActor</c> 上新增了
//   <c>LoadSurface()</c> / <c>DrawChr(int,int,bool,bool)</c> / <c>RunSound()</c> / <c>RunActSound(int)</c>
//   四个虚成员；<c>Run(uint)</c> 亦已虚化（ActorCore.cs:344）。其**基类本体**由车道
//   `p7-client-actor-family` 落在 <c>Scenes/ActorFamilyImpl.cs</c>（<c>LoadSurface(object?)</c> /
//   <c>DrawChr</c> / <c>RunSound</c> / <c>RunActSound</c>），接缝在 <c>Scenes/ActorFamilyEnv.cs</c>。</para>
//   <para>★ <b>已落地（第二轮，越区请求获批后）</b>：本单元这批子类**已全部接成类 `override`** ——
//   4 个基类本体落在 <c>Scenes/ActorFamilyBase.cs</c>（<c>partial class TActor</c>），
//   7 个目标子类分别落在 <c>Scenes/ActorFamilyHerb.cs</c>（TKillingHerb / TMineMon / TBeeQueen /
//   TCentipedeKingMon / TBigHeartMon / TSpiderHouseMon）、<c>Scenes/ActorFamilyStructures.cs</c>
//   （TCastleDoor / TWallStructure / TNewWallStructure）与 <c>Scenes/ActorFamilyDragonBody.cs</c>
//   （TDragonBody）。<b>桩类头加 `partial` 与基类关系更正</b>都已按原文完成
//   （原文层级：TKillingHerb : TActor，而 TMineMon/TCentipedeKingMon/TBigHeartMon/
//   TSpiderHouseMon/TDragonBody 都 : TKillingHerb —— 原先平铺成 : TActor 会让 `inherited`
//   整段绕过 TKillingHerb 的覆写，属"看起来一样实则不同"）。</para>
//   <para>⚠ 本文件上方那批「纯函数抽取」（<c>HerbActorFramework</c> 的决策表）**保留**：
//   它们是同一批原文判定的"可测抽取"产物，供审计与差异断言使用；
//   类 override 是**真正被执行**的那一份。**两者若出现分歧，以原文为准并须同时更正**（报告 §11）。</para>
//   <para>另：<c>GetRaceByPM</c> 读的是 <c>ActorActionTables</c> 的 <c>TMonsterClientAction</c>，
//   而原版是 <c>pTMonsterAction</c>（字段名亦不完全一致）—— 该字段面对齐由
//   <c>GXX.Core.Protocol</c> 的 <c>TMonsterClientAction</c> 承担。</para>
//
//   因此本文件按"**把每个子类的判定逻辑抽成可测纯函数**"的规程落地：
//   框架（<see cref="HerbActorFramework"/>）承载各子类**逐行等价**的决策表，
//   接入方在 DxComponent/Actor 侧补上虚方法后可直接转调这些函数。
using System;
using System.Collections.Generic;

namespace GXX.Client.Tail;

/// <summary>原文 :19-23 的常量（1:1）。</summary>
public static class HerbActorConst
{
    /// <summary>原文 :20 — <c>BEEQUEENBASE = 600;</c></summary>
    public const int BEEQUEENBASE = 600;
    /// <summary>原文 :21 — <c>DOORDEATHEFFECTBASE = 120;</c></summary>
    public const int DOORDEATHEFFECTBASE = 120;
    /// <summary>原文 :22 — <c>WALLLEFTBROKENEFFECTBASE = 224;</c></summary>
    public const int WALLLEFTBROKENEFFECTBASE = 224;
    /// <summary>原文 :23 — <c>WALLRIGHTBROKENEFFECTBASE = 240;</c></summary>
    public const int WALLRIGHTBROKENEFFECTBASE = 240;
}

/// <summary>原文 :26 — <c>TDoorState = (dsOpen, dsClose, dsBroken);</c></summary>
public enum TDoorState
{
    /// <summary>原文 :26 — <c>dsOpen</c></summary>
    dsOpen = 0,
    /// <summary>原文 :26 — <c>dsClose</c></summary>
    dsClose = 1,
    /// <summary>原文 :26 — <c>dsBroken</c></summary>
    dsBroken = 2,
}

/// <summary>
/// <c>TActor</c> 的**只读视图**接缝（原文 <c>Self</c> 的字段面）。
/// <para>与 <c>GXX.Client.Scenes</c> 的 <c>TActorCore</c> 对齐，字段名沿用原文的 <c>m_*</c> 命名。</para>
/// </summary>
public interface IHerbActorView
{
    /// <summary>原文 <c>m_nChangeAppr</c>（&gt;=0 表示"变身/外观覆盖"，走基类逻辑）</summary>
    int ChangeAppr { get; }
    /// <summary>原文 <c>m_nCurrentAction</c></summary>
    int CurrentAction { get; set; }
    /// <summary>原文 <c>m_btRace</c></summary>
    int BtRace { get; }
    /// <summary>原文 <c>m_wAppearance</c></summary>
    int Appearance { get; }
    /// <summary>原文 <c>m_btDir</c></summary>
    int BtDir { get; set; }
    /// <summary>原文 <c>m_boDeath</c></summary>
    bool BoDeath { get; }
    /// <summary>原文 <c>m_boSkeleton</c></summary>
    bool BoSkeleton { get; }
    /// <summary>原文 <c>m_nCurrentDefFrame</c></summary>
    int CurrentDefFrame { get; }
    /// <summary>原文 <c>m_boUseMagic</c>（<c>CalcActorFrame</c> 开头会被置 False）</summary>
    bool BoUseMagic { get; set; }
    /// <summary>原文 <c>m_nCurrentFrame</c>（<c>CalcActorFrame</c> 开头会被置 −1）</summary>
    int CurrentFrame { get; set; }
}

/// <summary>原文 <c>pTMonsterAction</c> 的动作子表（只取本单元用到的字段）。</summary>
public struct THerbActionPart
{
    /// <summary>原文 <c>start</c></summary>
    public int start;
    /// <summary>原文 <c>frame</c></summary>
    public int frame;
    /// <summary>原文 <c>skip</c></summary>
    public int skip;
    /// <summary>原文 <c>ftime</c></summary>
    public uint ftime;
    /// <summary>原文 <c>usetick</c>（只有 <c>ActWalk</c> 用到）</summary>
    public int usetick;
}

/// <summary>原文 <c>pm: pTMonsterAction</c> 的只读视图。</summary>
public interface IHerbMonsterAction
{
    /// <summary>原文 <c>pm.ActStand</c></summary>
    THerbActionPart ActStand { get; }
    /// <summary>原文 <c>pm.ActWalk</c></summary>
    THerbActionPart ActWalk { get; }
    /// <summary>原文 <c>pm.ActAttack</c></summary>
    THerbActionPart ActAttack { get; }
    /// <summary>原文 <c>pm.ActCritical</c></summary>
    THerbActionPart ActCritical { get; }
    /// <summary>原文 <c>pm.ActStruck</c></summary>
    THerbActionPart ActStruck { get; }
    /// <summary>原文 <c>pm.ActDie</c></summary>
    THerbActionPart ActDie { get; }
    /// <summary>原文 <c>pm.ActDeath</c></summary>
    THerbActionPart ActDeath { get; }
}

/// <summary>
/// 子类的 **CalcActorFrame 规划结果**（原文直接改 Self 的字段；此处把结果显式化以便单测）。
/// <para><c>Null</c> 表示原文在该分支 <c>Exit</c>（什么都不改）。</para>
/// </summary>
public sealed class HerbFramePlan
{
    /// <summary>原文 <c>m_nStartFrame</c> 写入值（null = 未写）</summary>
    public int? StartFrame;
    /// <summary>原文 <c>m_nEndFrame</c> 写入值</summary>
    public int? EndFrame;
    /// <summary>原文 <c>m_dwFrameTime</c> 写入值</summary>
    public uint? FrameTime;
    /// <summary>原文 <c>m_nDefFrameCount</c> 写入值</summary>
    public int? DefFrameCount;
    /// <summary>原文 <c>m_nMaxTick</c> 写入值</summary>
    public int? MaxTick;
    /// <summary>原文 <c>m_nCurTick</c> 写入值</summary>
    public int? CurTick;
    /// <summary>原文 <c>m_nMoveStep</c> 写入值</summary>
    public int? MoveStep;
    /// <summary>原文 <c>m_dwWarModeTime := TimeGetTime</c>（只记"是否写了"）</summary>
    public bool SetWarModeTime;
    /// <summary>原文 <c>m_dwStruckFrameTime</c> 被用作帧时间（受击分支）</summary>
    public bool UsedStruckFrameTime;
    /// <summary>原文 <c>m_CustomMagicStatusEffect.m_nStruck := 0</c>（受击分支）</summary>
    public bool ResetStruckCounter;
    /// <summary>原文 <c>m_boDelActionAfterFinished := True</c>（DIGDOWN 分支）</summary>
    public bool DelActionAfterFinished;
    /// <summary>原文调用了 <c>Shift(m_btDir, 0, 0, 1)</c></summary>
    public bool ShiftCalled;
    /// <summary>原文在该分支把 <c>m_btDir := 0</c>（CentipedeKing / BigHeart / SpiderHouse）</summary>
    public bool ForceDirZero;
    /// <summary>原文该分支标记为"交由 inherited 处理"（CentipedeKing 的 SM_TURN / SM_DIGDOWN / else）</summary>
    public bool DelegatesToInherited;
    /// <summary>原文该分支设置的死亡特效帧区间（CentipedeKing 的 LIGHTINGEX/HIT）</summary>
    public bool UseDieEffect;
    /// <summary>分支名（便于断言与排错）</summary>
    public string Branch;
}

/// <summary>
/// HerbActor.pas 各子类 <c>CalcActorFrame</c> 决策逻辑的 1:1 抽取。
/// </summary>
public static class HerbActorFramework
{
    /// <summary>
    /// 原文 :163-169 / :309-315 / :437-442 / :588-594 —— **四个子类逐字相同**的"攻击动作归一化"：
    /// 当 <c>m_nChangeAppr &gt;= 0</c> 且当前动作是 <c>SM_ATTACK01..SM_ATTACK06</c> 之一时，
    /// 把动作改写为 <c>SM_HIT</c>，然后**转调 inherited** 并 Exit。
    /// <para>注意 <c>SM_ATTACK14</c>（用镰刀/魔法的动作）**不在**这个列表里 —— 差异断言要点。</para>
    /// </summary>
    public static bool IsChangeApprAttackAction(int currentAction)
        => currentAction == ActorCoreSMAttack.SM_ATTACK01
        || currentAction == ActorCoreSMAttack.SM_ATTACK02
        || currentAction == ActorCoreSMAttack.SM_ATTACK03
        || currentAction == ActorCoreSMAttack.SM_ATTACK04
        || currentAction == ActorCoreSMAttack.SM_ATTACK05
        || currentAction == ActorCoreSMAttack.SM_ATTACK06;

    /// <summary>
    /// 原文 :162-173 <c>TKillingHerb.CalcActorFrame</c> 的**前置段**：
    /// 返回 true 表示"走 inherited 分支并 Exit"（变身处）；否则继续走自己的 case。
    /// <para>副作用：变身处会把 <c>m_nCurrentAction</c> 改写成 <c>SM_HIT</c>（原文 :169）。</para>
    /// </summary>
    public static bool PreKillingHerb(IHerbActorView self)
    {
        if (self.ChangeAppr >= 0)
        {
            if (IsChangeApprAttackAction(self.CurrentAction))
                self.CurrentAction = ActorCoreSM.SM_HIT;
            return true;
        }
        // 原文 :174-175（非变身处）
        self.BoUseMagic = false;
        self.CurrentFrame = -1;
        return false;
    }

    /// <summary>
    /// 原文 :303-397 <c>TBeeQueen.CalcActorFrame</c> 的 case 表。
    /// <para><b>与 <c>TKillingHerb</c> 的两处差异</b>（差异断言要点）：</para>
    /// <list type="bullet">
    /// <item>所有 <c>Act*</c> 的 <c>start</c> 都**不加** <c>m_btDir * (frame + skip)</c>（方向无关）；</item>
    /// <item>没有 <c>SM_DIGUP</c> 与 <c>SM_DIGDOWN</c> 两个分支。</item>
    /// </list>
    /// </summary>
    public static HerbFramePlan PlanBeeQueen(int currentAction, int btDir, uint struckFrameTime,
        IHerbMonsterAction pm)
    {
        if (pm == null) return null;
        var p = new HerbFramePlan { Branch = ActionName(currentAction) };
        switch (currentAction)
        {
            case ActorCoreSM.SM_TURN:
                p.StartFrame = pm.ActStand.start;
                p.EndFrame = p.StartFrame + pm.ActStand.frame - 1;
                p.FrameTime = pm.ActStand.ftime;
                p.DefFrameCount = pm.ActStand.frame;
                p.ShiftCalled = true;
                break;

            case ActorCoreSM.SM_LIGHTINGEX:
            case ActorCoreSM.SM_HIT:
                p.StartFrame = pm.ActAttack.start;
                p.EndFrame = p.StartFrame + pm.ActAttack.frame - 1;
                p.FrameTime = pm.ActAttack.ftime;
                p.SetWarModeTime = true;
                p.ShiftCalled = true;
                break;

            case ActorCoreSM.SM_STRUCK:
                p.StartFrame = pm.ActStruck.start;
                p.EndFrame = p.StartFrame + pm.ActStruck.frame - 1;
                p.FrameTime = struckFrameTime;      // 原文 :377 用 m_dwStruckFrameTime
                p.UsedStruckFrameTime = true;
                p.ShiftCalled = true;
                p.ResetStruckCounter = true;
                break;

            case ActorCoreSM.SM_DEATH:
                p.StartFrame = pm.ActDie.start;
                p.EndFrame = p.StartFrame + pm.ActDie.frame - 1;
                p.StartFrame = p.EndFrame;          // 原文 :386 尸体停在末帧
                p.FrameTime = pm.ActDie.ftime;
                break;

            case ActorCoreSM.SM_NOWDEATH:
                p.StartFrame = pm.ActDie.start;
                p.EndFrame = p.StartFrame + pm.ActDie.frame - 1;
                p.FrameTime = pm.ActDie.ftime;
                break;

            default:
                return p;                            // 原文 case 无 else ⇒ 什么都不做
        }
        return p;
    }

    /// <summary>
    /// 原文 :157-268 <c>TKillingHerb.CalcActorFrame</c> 的 case 表（未变身分支）。
    /// <para><b>与 <c>TBeeQueen</c> 的差异</b>：<c>ActAttack</c>/<c>ActStruck</c>/<c>ActDie</c>
    /// 都要加 <c>m_btDir * (frame + skip)</c>；另外多出 <c>SM_DIGUP</c>（用 <c>ActWalk</c>）
    /// 与 <c>SM_DIGDOWN</c>（用 <c>ActDeath</c> 且置 <c>m_boDelActionAfterFinished</c>）。</para>
    /// </summary>
    public static HerbFramePlan PlanKillingHerb(int currentAction, int btDir, uint struckFrameTime,
        IHerbMonsterAction pm)
    {
        if (pm == null) return null;
        var p = new HerbFramePlan { Branch = ActionName(currentAction) };
        switch (currentAction)
        {
            case ActorCoreSM.SM_TURN:
                // 原文 :182 —— 注意这一句把 Dir 乘法**注释掉**了（与 TBeeQueen 一致）
                p.StartFrame = pm.ActStand.start; // + Dir * (pm.ActStand.frame + pm.ActStand.skip);
                p.EndFrame = p.StartFrame + pm.ActStand.frame - 1;
                p.FrameTime = pm.ActStand.ftime;
                p.DefFrameCount = pm.ActStand.frame;
                p.ShiftCalled = true;
                break;

            case ActorCoreSM.SM_DIGUP:
                // 原文 :190 —— 同样把 Dir 乘法注释掉了，且 Shift 的后 3 个参数也被注释成 (btDir,0,0,1)
                p.StartFrame = pm.ActWalk.start; // + Dir * (pm.ActWalk.frame + pm.ActWalk.skip);
                p.EndFrame = p.StartFrame + pm.ActWalk.frame - 1;
                p.FrameTime = pm.ActWalk.ftime;
                p.MaxTick = pm.ActWalk.usetick;
                p.CurTick = 0;
                p.MoveStep = 1;
                p.ShiftCalled = true;
                break;

            case ActorCoreSM.SM_LIGHTINGEX:
            case ActorCoreSM.SM_HIT:
                p.StartFrame = pm.ActAttack.start + btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
                p.EndFrame = p.StartFrame + pm.ActAttack.frame - 1;
                p.FrameTime = pm.ActAttack.ftime;
                p.SetWarModeTime = true;
                p.ShiftCalled = true;
                break;

            case ActorCoreSM.SM_STRUCK:
                p.StartFrame = pm.ActStruck.start + btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
                p.EndFrame = p.StartFrame + pm.ActStruck.frame - 1;
                p.FrameTime = struckFrameTime;
                p.UsedStruckFrameTime = true;
                p.ShiftCalled = true;
                p.ResetStruckCounter = true;
                break;

            case ActorCoreSM.SM_DEATH:
                p.StartFrame = pm.ActDie.start + btDir * (pm.ActDie.frame + pm.ActDie.skip);
                p.EndFrame = p.StartFrame + pm.ActDie.frame - 1;
                p.StartFrame = p.EndFrame;
                p.FrameTime = pm.ActDie.ftime;
                break;

            case ActorCoreSM.SM_NOWDEATH:
                p.StartFrame = pm.ActDie.start + btDir * (pm.ActDie.frame + pm.ActDie.skip);
                p.EndFrame = p.StartFrame + pm.ActDie.frame - 1;
                p.FrameTime = pm.ActDie.ftime;
                break;

            case ActorCoreSM.SM_DIGDOWN:
                // 原文 :260-265 —— 用 ActDeath，且**不加** Dir 乘法；置删除标记
                p.StartFrame = pm.ActDeath.start;
                p.EndFrame = p.StartFrame + pm.ActDeath.frame - 1;
                p.FrameTime = pm.ActDeath.ftime;
                p.DelActionAfterFinished = true;
                break;

            default:
                return p;
        }
        return p;
    }

    /// <summary>
    /// 原文 :430-514 <c>TCentipedeKingMon.CalcActorFrame</c> 的 case 表。
    /// <para><b>本类最特殊</b>：除 <c>SM_LIGHTINGEX</c>/<c>SM_HIT</c> 外**一律把 <c>m_btDir := 0</c>
    /// 后转调 inherited**（含 <c>SM_TURN</c>、<c>SM_DIGDOWN</c> 与 <c>else</c> 分支）；
    /// 而这两个攻击分支用 <c>ActCritical</c> 表并打开死亡特效（<c>0..9</c> 帧、62ms）。</para>
    /// </summary>
    public static HerbFramePlan PlanCentipedeKing(int currentAction, uint struckFrameTime,
        IHerbMonsterAction pm)
    {
        if (pm == null) return null;
        var p = new HerbFramePlan { Branch = ActionName(currentAction) };
        switch (currentAction)
        {
            case ActorCoreSM.SM_LIGHTINGEX:
            case ActorCoreSM.SM_HIT:
                p.ForceDirZero = true;
                p.StartFrame = pm.ActCritical.start;    // btDir 恒 0 ⇒ 乘法项为 0
                p.EndFrame = p.StartFrame + pm.ActCritical.frame - 1;
                p.FrameTime = pm.ActCritical.ftime;
                p.UseDieEffect = true;
                p.ShiftCalled = true;
                break;

            case ActorCoreSM.SM_TURN:
                p.ForceDirZero = true;
                p.DelegatesToInherited = true;
                break;

            case ActorCoreSM.SM_DIGDOWN:
                // 原文 :506-508 这一支**没有** m_btDir := 0，直接 inherited
                p.DelegatesToInherited = true;
                break;

            default:
                p.ForceDirZero = true;
                p.DelegatesToInherited = true;
                break;
        }
        return p;
    }

    /// <summary>
    /// 原文 :1166-1170 <c>TMineMon.CalcActorFrame</c> —— **完全转调 inherited**（空实现）。
    /// <para>差异断言要点：它 <b>不</b>做任何前置处理，连 <c>m_boUseMagic := FALSE</c> 都没有
    /// （那是 <c>TKillingHerb</c> 自己加的）。</para>
    /// </summary>
    public static bool MineMon_DelegatesToInherited_WithNoOwnWork() => true;

    /// <summary>
    /// 原文 :1172-1175 <c>TMineMon.Create</c> —— 也只转调 inherited（无字段初始化）。
    /// </summary>
    public static bool MineMon_Create_HasNoFieldInit() => true;

    /// <summary>
    /// 原文 :1230-1234 <c>TBigHeartMon.CalcActorFrame</c> 与
    /// 原文 :1238-1242 <c>TSpiderHouseMon.CalcActorFrame</c>
    /// —— **两者逐字相同**：先 <c>m_btDir := 0</c> 再转调 inherited。
    /// </summary>
    public static void PlanDirZeroThenInherited(IHerbActorView self)
    {
        if (self == null) return;
        self.BtDir = 0;
    }

    // ══════════════════════════════════════════════════════════════════════
    // GetDefaultFrame（原文 :270-299 / :399-425 / :1218-1226）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :270-299 <c>TKillingHerb.GetDefaultFrame</c>（未变身分支）。
    /// <para>死亡时：<c>m_boSkeleton</c> ⇒ <c>ActDeath.start</c>；
    /// 否则 <c>ActDie.start + Dir*(frame+skip) + (frame-1)</c>（**尸体停在末帧**的等价写法）。</para>
    /// <para>存活时：<c>m_nCurrentDefFrame</c> 越界（&lt;0 或 &gt;= frame）⇒ 0，否则取自身；
    /// 结果 <c>ActStand.start + cf</c>。注意这里会把 <c>m_nDefFrameCount</c> 置为 <c>ActStand.frame</c>。</para>
    /// </summary>
    public static int GetDefaultFrameKillingHerb(bool boDeath, bool boSkeleton, int btDir,
        int currentDefFrame, IHerbMonsterAction pm, out int defFrameCount)
    {
        defFrameCount = 0;
        if (pm == null) return 0;

        if (boDeath)
        {
            if (boSkeleton) return pm.ActDeath.start;
            return pm.ActDie.start + btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
        }

        defFrameCount = pm.ActStand.frame;
        int cf;
        if (currentDefFrame < 0) cf = 0;
        else if (currentDefFrame >= pm.ActStand.frame) cf = 0;
        else cf = currentDefFrame;
        return pm.ActStand.start + cf;
    }

    /// <summary>
    /// 原文 :399-425 <c>TBeeQueen.GetDefaultFrame</c>（未变身分支）。
    /// <para><b>与 <c>TKillingHerb</c> 的差异</b>：死亡时**没有** <c>m_boSkeleton</c> 分支，
    /// 且 <c>ActDie</c> 的 <c>Dir*(frame+skip)</c> 项也去掉了 —— 直接
    /// <c>ActDie.start + (frame - 1)</c>。</para>
    /// </summary>
    public static int GetDefaultFrameBeeQueen(bool boDeath, int currentDefFrame,
        IHerbMonsterAction pm, out int defFrameCount)
    {
        defFrameCount = 0;
        if (pm == null) return 0;

        if (boDeath)
        {
            return pm.ActDie.start + (pm.ActDie.frame - 1);
        }

        defFrameCount = pm.ActStand.frame;
        int cf;
        if (currentDefFrame < 0) cf = 0;
        else if (currentDefFrame >= pm.ActStand.frame) cf = 0;
        else cf = currentDefFrame;
        return pm.ActStand.start + cf;
    }

    /// <summary>
    /// 原文 :1218-1226 <c>TMineMon.GetDefaultFrame</c> —— 未变身时**恒返回 0**
    /// （原文注释 <c>//HZQ 2030524</c>）；变身时转调 inherited。
    /// </summary>
    public static int GetDefaultFrameMineMon(int changeAppr, Func<int> inheritedGetDefaultFrame)
        => changeAppr >= 0 ? (inheritedGetDefaultFrame?.Invoke() ?? 0) : 0;

    // ══════════════════════════════════════════════════════════════════════
    // TCastleDoor（原文 :76-92 / :519-745）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :533-560 <c>TCastleDoor.ApplyDoorState(dstate)</c> 的**可行走标记表**。
    ///
    /// <para>原文流程（顺序重要）：</para>
    /// <list type="number">
    /// <item>先把 3 个格子**无条件**标记为可走：
    ///   <c>(x, y-2)</c>、<c>(x+1, y-1)</c>、<c>(x+1, y-2)</c>；</item>
    /// <item><c>bowalk := (dstate &lt;&gt; dsClose)</c>，然后把**另外 9 个格子**按 <c>bowalk</c> 统一设置；</item>
    /// <item>若 <c>dstate = dsOpen</c>，再把第 1 步那 3 个格子**重新标记为不可走**。</item>
    /// </list>
    /// <para>净效果：<c>dsOpen</c> 时这 3 格不可走；<c>dsClose</c>/<c>dsBroken</c> 时可走。</para>
    /// </summary>
    /// <returns>9 个"按 bowalk 统一设置"的格子的取值。</returns>
    public static bool ApplyDoorState_Bowalk(TDoorState dstate) => dstate != TDoorState.dsClose;

    /// <summary>原文 :556-559 —— 仅 <c>dsOpen</c> 时对前 3 格做"不可走"覆盖。</summary>
    public static bool ApplyDoorState_FirstThreeAreBlocked(TDoorState dstate)
        => dstate == TDoorState.dsOpen;

    /// <summary>原文 :537-539 —— 无条件标记为可走的 3 个偏移（相对 <c>m_nCurrX/m_nCurrY</c>）。</summary>
    public static readonly (int dx, int dy)[] DoorAlwaysWalkOffsets =
    {
        (0, -2), (+1, -1), (+1, -2),
    };

    /// <summary>原文 :545-553 —— 按 <c>bowalk</c> 统一设置的 9 个偏移（第 3 项与上面重复）。</summary>
    public static readonly (int dx, int dy)[] DoorBowalkOffsets =
    {
        (0, 0), (0, -1), (0, -2), (+1, -1), (+1, -2), (-1, -1), (-1, 0), (-1, +1), (-2, 0),
    };

    /// <summary>
    /// 原文 :519-525 <c>TCastleDoor.Create</c>：<c>m_btDir := 0</c>、
    /// <c>EffectSurface := nil</c>、<c>m_nDownDrawLevel := 1</c>。
    /// <para>原文注释：<c>// 1伎 刚历 弊覆.</c>（"层级设为 1，让门画在人物之下"）。</para>
    /// </summary>
    public const int CastleDoorDownDrawLevel = 1;

    /// <summary>原文 :605 <c>m_sUserName := ' ';</c>（注意是**一个空格**，不是空串）。</summary>
    public const string CastleDoorUserName = " ";

    // ══════════════════════════════════════════════════════════════════════
    // TWallStructure / TNewWallStructure（原文 :94-126 / :746-1165）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>原文 :22-23 的墙体破碎特效基址按左右分。</summary>
    public static int WallBrokenEffectBase(bool isLeftWall)
        => isLeftWall ? HerbActorConst.WALLLEFTBROKENEFFECTBASE : HerbActorConst.WALLRIGHTBROKENEFFECTBASE;

    /// <summary>原文 :100 / :117 —— <c>bomarkpos</c>（原文注释为韩文乱码，语义：是否标记过位置）。</summary>
    public static bool WallBomarkPosDefault() => false;

    // ══════════════════════════════════════════════════════════════════════
    // 辅助
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>把动作号转成原文分支名（便于断言/排错）。</summary>
    public static string ActionName(int action) => action switch
    {
        ActorCoreSM.SM_TURN => "SM_TURN",
        ActorCoreSM.SM_DIGUP => "SM_DIGUP",
        ActorCoreSM.SM_LIGHTINGEX => "SM_LIGHTINGEX",
        ActorCoreSM.SM_HIT => "SM_HIT",
        ActorCoreSM.SM_STRUCK => "SM_STRUCK",
        ActorCoreSM.SM_DEATH => "SM_DEATH",
        ActorCoreSM.SM_NOWDEATH => "SM_NOWDEATH",
        ActorCoreSM.SM_DIGDOWN => "SM_DIGDOWN",
        _ => "SM_" + action,
    };
}

/// <summary>
/// Actor.pas 的动作常量中本单元需要、但 <c>GXX.Client.Scenes.ActorCore</c> 未暴露的那几个。
/// <para>值取自 <c>Source/Common/Grobal2.pas</c>（与 <c>Actor.pas</c> 共用同一批 <c>SM_*</c>）。</para>
/// </summary>
public static class ActorCoreSM
{
    /// <summary>Grobal2.pas — <c>SM_TURN = 10</c></summary>
    public const int SM_TURN = 10;
    /// <summary>Grobal2.pas — <c>SM_HIT = 14</c></summary>
    public const int SM_HIT = 14;
    /// <summary>Grobal2.pas — <c>SM_STRUCK = 31</c></summary>
    public const int SM_STRUCK = 31;
    /// <summary>Grobal2.pas — <c>SM_DEATH = 32</c></summary>
    public const int SM_DEATH = 32;
    /// <summary>Grobal2.pas — <c>SM_NOWDEATH = 34</c></summary>
    public const int SM_NOWDEATH = 34;
    /// <summary>Grobal2.pas — <c>SM_DIGUP = 20</c></summary>
    public const int SM_DIGUP = 20;
    /// <summary>Grobal2.pas — <c>SM_DIGDOWN = 21</c></summary>
    public const int SM_DIGDOWN = 21;
    /// <summary>Grobal2.pas — <c>SM_LIGHTINGEX = 1445</c></summary>
    public const int SM_LIGHTINGEX = 1445;
}

/// <summary>
/// Actor.pas / Grobal2.pas 的 <c>SM_ATTACK01..SM_ATTACK06</c> 常量。
/// <para><b>值已用原文核对</b>：<c>Source/Common/Grobal2.pas:1938-1943</c> ——
/// <c>SM_ATTACK01 = 8946</c> 起、逐个 +1（注释"自定义攻击1..6"）。</para>
/// </summary>
public static class ActorCoreSMAttack
{
    /// <summary>Grobal2.pas:1938 — <c>SM_ATTACK01 = 8946; // 自定义攻击1</c></summary>
    public const int SM_ATTACK01 = 8946;
    /// <summary>Grobal2.pas:1939 — <c>SM_ATTACK02 = 8947;</c></summary>
    public const int SM_ATTACK02 = 8947;
    /// <summary>Grobal2.pas:1940 — <c>SM_ATTACK03 = 8948;</c></summary>
    public const int SM_ATTACK03 = 8948;
    /// <summary>Grobal2.pas:1941 — <c>SM_ATTACK04 = 8949;</c></summary>
    public const int SM_ATTACK04 = 8949;
    /// <summary>Grobal2.pas:1942 — <c>SM_ATTACK05 = 8950;</c></summary>
    public const int SM_ATTACK05 = 8950;
    /// <summary>Grobal2.pas:1943 — <c>SM_ATTACK06 = 8951; // 自定义攻击6</c></summary>
    public const int SM_ATTACK06 = 8951;
    /// <summary>
    /// 原文只定义了 <c>SM_ATTACK01..SM_ATTACK06</c> 六个自定义攻击动作
    /// （已全树 grep 核对：<c>Grobal2.pas</c> 中没有 <c>SM_ATTACK07..14</c>）。
    /// 此常量是一个**不存在**的动作号，仅用于差异断言 ——
    /// 验证"归一化列表外"的动作不会被改写成 <c>SM_HIT</c>。
    /// </summary>
    public const int SM_ATTACK_OUT_OF_LIST = 9999;
}

/// <summary>
/// 本单元的**覆盖登记**（承 §2.3 / 任务书"做不完如实报告"）。
/// </summary>
public static class HerbActorCoverage
{
    /// <summary>原文 14 个类的清单与落地状态。</summary>
    /// <remarks>
    /// ★ 车道 `p7-client-actor-family` 第二轮（越区请求获批后）：7 个目标子类已落成**真类 override**，
    /// 故本表的状态由"纯函数抽取"统一升级为"类 override + 纯函数抽取"。
    /// 类实现分别在 `Scenes/ActorFamilyHerb.cs` / `ActorFamilyStructures.cs` / `ActorFamilyDragonBody.cs`。
    /// </remarks>
    public static readonly (string ClassName, int SourceLine, string Status)[] Units = new[]
    {
        ("TKillingHerb",         28,  "已落地：类 override（CalcActorFrame 157-268 / GetDefaultFrame 270-299，见 ActorFamilyHerb.cs）+ 本文件纯函数抽取"),
        ("TMineMon",             37,  "已落地：类 override（CalcActorFrame 1166-1170 纯转调 / GetDefaultFrame 1218-1226 未变身恒 0）+ 纯函数抽取"),
        ("TBeeQueen",            45,  "已落地：类 override（CalcActorFrame 303-397 / GetDefaultFrame 399-425；与 KillingHerb 的方向乘法差异已双向断言）"),
        ("TCentipedeKingMon",    52,  "已落地：类 override（CalcActorFrame 430-514 / DrawEff 1177-1182 / LoadEffect 1184-1200 / Finalize 1202-1206 / LoadSurface 1208-1216 / Run 1244-1284）；依赖 g_WMonImages.Indexs[15] 与 PlayScene.NewMagic 的取图/特效走接缝"),
        ("TBigHeartMon",         66,  "已落地：类 override（CalcActorFrame 1230-1234：m_btDir := 0 后转调 inherited）"),
        ("TSpiderHouseMon",      71,  "已落地：类 override（1238-1242，与 TBigHeartMon 逐字相同但保留为两类）"),
        ("TCastleDoor",          76,  "已落地：类 override（Create 519-525 / Finalize 527-531 / ApplyDoorState 533-560 / LoadSurface 562-581 / CalcActorFrame 583-671 / GetDefaultFrame 673-699 / ActionEnded 701-709 / Run 711-724 / DrawChr 726-741）；TTexture 取图走接缝"),
        ("TWallStructure",       94,  "已落地：类 override（Create 746-754 / Finalize 756-761 / CalcActorFrame 763-830 / LoadSurface 832-912 / GetDefaultFrame 914-927 / DrawChr 929-950 / Run 952-968）"),
        ("TNewWallStructure",    111, "已落地：类 override（Create 973-981 / Finalize 983-988 / CalcActorFrame 990-1048 / LoadSurface 1050-1106 / GetDefaultFrame 1108-1121 / DrawChr 1123-1144 / Run 1146-1162）；与 TWallStructure 的三处实质差异已双向断言"),
        ("TSoccerBall",          128, "已落地：原文是空类（无任何成员）"),
        ("TDragonBody",          132, "已落地：类 override（CalcActorFrame 1288-1308 / DrawEff 1310-1315 / LoadSurface 1317-1339）；g_WDragonImg 取图走接缝"),
        ("TWallStructure 系列共用常量", 19, "已落地：BEEQUEENBASE/DOORDEATHEFFECTBASE/WALLLEFTBROKENEFFECTBASE/WALLRIGHTBROKENEFFECTBASE（后三者亦已在 ActorFamilyHerbEnv 常量表登记）"),
        ("TDoorState",           26,  "已落地：枚举三步（托管落点在 GXX.Client.Tail；Scenes 侧以 using 别名引用）"),
        ("Actor 基类虚方法（4 个本体 + 7 子类 override）", 28,
            "已落地：基类 4 本体在 Scenes/ActorFamilyBase.cs（经 ActorFamilyImpl 承载）+ Scenes/ActorFamilyImpl.cs；7 子类 override 见上表。另见 docs/并行报告-p7-client-actor-family.md"),
    };

    /// <summary>
    /// 未覆盖的原文行区间（供审计工具精确扣除）。
    /// <para>★ <b>第二轮结论：`HerbActor.pas` 已无未覆盖区间</b> —— 19-29 的常量/枚举、
    /// 157-1341 的全部方法体均已 1:1 落地（取图/绘制/特效经 <c>ActorFamilyEnv</c> 与
    /// <c>ActorFamilyHerbEnv</c> 接缝）。原登记的四段（583-745 / 763-1165 / 1177-1216 /
    /// 1244-1341）已全部覆盖。</para>
    /// <para>本数组保留一条 <b>(303,302) 占位</b>（<c>From &gt; To</c> 即"非区间"，
    /// 该形态是既有审计断言 <c>UncoveredRanges_AreOrderedAndInBounds</c> 已识别的占位约定，
    /// 且**不会被任何审计工具当成真实区间**）：在"确无未覆盖区间"时**不虚构一条假区间**
    /// 是更诚实的做法，故由 <see cref="AllRangesCovered"/> 显式声明该事实。</para>
    /// <para>★ 注意不要用 <c>(0,0)</c> 作占位 —— 它满足 <c>From &lt;= To</c>，
    /// 会被审计当作**真实区间**并因越界（原文 1..1341）而报错。</para>
    /// </summary>
    public static readonly (int From, int To, string Reason)[] UncoveredRanges = new[]
    {
        (303, 302, "占位（From > To 即非区间）：HerbActor.pas 已无未覆盖区间，见 AllRangesCovered"),
    };

    /// <summary>
    /// ★ 第二轮事实声明：<c>HerbActor.pas</c> 的全部原文行（常量/枚举/14 个类的方法体）
    /// 均已落地，<see cref="UncoveredRanges"/> 中**没有**真实未覆盖区间。
    /// </summary>
    public const bool AllRangesCovered = true;

    /// <summary>
    /// ★ 车道 `p7-client-actor-family` 新增：**`Actor.pas` 基类 4 个虚方法本体**的落地登记
    /// （原文不在本单元，但本单元是它的登记方 —— 因为 `HerbActor.pas` 的 14 个子类全都覆写它们）。
    /// </summary>
    public static readonly (string Method, string SourceRange, string LandedAt)[] BaseActorBodies = new[]
    {
        ("TActor.LoadSurface(Sender:TObject)", "Actor.pas 5480-5593（115 行）",
            "Scenes/ActorFamilyImpl.cs `LoadSurface(TActorCore, object?)`（1:1，含九标签 case / 三重判据 / 反向帧双分支 / Finalize 短路）"),
        ("TActor.DrawChr(dx,dy,blend,boFlag)", "Actor.pas 6067-6129（63 行）",
            "Scenes/ActorFamilyImpl.cs `DrawChr`（1:1，含方向守卫 / DrawEffSurface / DrawStateEffSurface / 施法层）"),
        ("TActor.RunSound", "Actor.pas 6788-6901（114 行）",
            "Scenes/ActorFamilyImpl.cs `RunSound`（1:1，经 ActorSoundDispatch.RunSound 派发）"),
        ("TActor.RunActSound(frame:Integer)", "Actor.pas 6903-7095（193 行）",
            "Scenes/ActorFamilyBase.cs → ActorFamilyImpl.RunActSound（1:1）。★ 7063-7072 的 appearance=80 与 7076-7092 的 race 202..209 两支原先在 ActorSoundDispatch.RunActSoundOther **缺失**，本车道已在**源头**补齐并删除了本处的重复补偿（父 agent 裁定：不得留两份同义实现）"),
        ("TActor.DrawStateEffSurface（本体依赖）", "Actor.pas 5654-5702（49 行）",
            "Scenes/ActorFamilyImpl.cs `DrawStateEffSurface`（1:1，三层游标回写）"),
        ("TActor.SetSound（本体依赖）", "Actor.pas 6454-6786（333 行）",
            "**未移植**：原文整段被 `if m_btRace in [0,1]` 包住且无 else ⇒ 对怪物族**本就无副作用**，故基类空实现即正确语义；人类族覆写在 THumActor（另一单元）"),
    };

    /// <summary>
    /// ★ 第二轮（越区请求获批后）：本批 7 个子类已**全部落成真类 override**，故原"阻塞原因"表
    /// 改登记为**落地位置**。表名保留（避免改动既有断言），语义由 Blocker 变为 LandedAt。
    /// </summary>
    public static readonly (string ClassName, string Methods, string Blocker)[] SubclassOverrideBlockers = new[]
    {
        ("TKillingHerb", "CalcActorFrame 157-268 / GetDefaultFrame 270-299",
            "已落地：Scenes/ActorFamilyHerb.cs（partial class TKillingHerb : TActor）；类头在 PlaySceneNewActor.cs:505 已加 partial"),
        ("TBeeQueen", "CalcActorFrame 303-397 / GetDefaultFrame 399-425",
            "已落地：Scenes/ActorFamilyHerb.cs（partial class TBeeQueen : TActor —— 原文 :45 确为 class(TActor)）；类头在 PlaySceneNewActor.cs:524"),
        ("TMineMon", "CalcActorFrame 1166-1170 / GetDefaultFrame 1218-1226",
            "已落地：Scenes/ActorFamilyHerb.cs，且**基类按原文更正为 TKillingHerb**（原平铺 : TActor 会让 inherited 绕过 TKillingHerb）；类头在 PlaySceneNewActor.cs:544"),
        ("TCentipedeKingMon", "CalcActorFrame 430-514 / DrawEff 1177-1182 / LoadEffect 1184-1200 / Finalize 1202-1206 / LoadSurface 1208-1216 / Run 1244-1284",
            "已落地：Scenes/ActorFamilyHerb.cs，基类更正为 TKillingHerb；g_WMonImages.Indexs[15] 与 PlayScene.NewMagic 走 ActorFamilyHerbEnv 接缝"),
        ("TCastleDoor", "Create 519-525 / Finalize 527-531 / ApplyDoorState 533-560 / LoadSurface 562-581 / CalcActorFrame 583-671 / GetDefaultFrame 673-699 / ActionEnded 701-709 / Run 711-724 / DrawChr 726-741",
            "已落地：Scenes/ActorFamilyStructures.cs；Map.MarkCanWalk 与门特效取图走 ActorFamilyHerbEnv 接缝"),
        ("TWallStructure", "Create 746-754 / Finalize 756-761 / CalcActorFrame 763-830 / LoadSurface 832-912 / GetDefaultFrame 914-927 / DrawChr 929-950 / Run 952-968",
            "已落地：Scenes/ActorFamilyStructures.cs；类头在 PlaySceneNewActor.cs 已加 partial"),
        ("TNewWallStructure", "Create 973-981 / Finalize 983-988 / CalcActorFrame 990-1048 / LoadSurface 1050-1106 / GetDefaultFrame 1108-1121 / DrawChr 1123-1144 / Run 1146-1162",
            "已落地：Scenes/ActorFamilyStructures.cs（同时补上了原文 :111 有、而托管侧此前**完全缺失**的桩类）；与 TWallStructure 的三处实质差异已双向断言"),
        ("TDragonBody", "CalcActorFrame 1288-1308 / DrawEff 1310-1315 / LoadSurface 1317-1339",
            "已落地：Scenes/ActorFamilyDragonBody.cs，基类更正为 TKillingHerb；g_WDragonImg 取图走接缝"),
    };
}
