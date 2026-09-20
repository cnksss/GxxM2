using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

// ════════════════════════════════════════════════════════════════════════════
// 车道 p7-client-actor-family：HerbActor.pas 族子类的 1:1 override
// ════════════════════════════════════════════════════════════════════════════
//
// 本文件覆盖 6 个类（第 7 个 TDragonBody 在 ActorFamilyDragonBody.cs）：
//   TKillingHerb(28) / TMineMon(37) / TBeeQueen(45) / TCentipedeKingMon(52)
//   TBigHeartMon(66) / TSpiderHouseMon(71)
//
// 原文类层级（**必须逐字保留，否则 inherited 会走错**）：
//   TKillingHerb : TActor
//   TMineMon / TCentipedeKingMon / TBigHeartMon / TSpiderHouseMon / TDragonBody : TKillingHerb
//   TBeeQueen : TActor
//
// ⚠ 本文件**不重复实现**已由 HerbActor.cs 落地的纯函数决策表（PlanKillingHerb 等）——
//   那是同一批判定的"可测抽取"产物，供审计与差异断言使用；本文件是**真正被执行**的
//   类 override。两者若出现分歧，以**原文**为准并须同时更正（报告 §11 已登记该风险）。
//
// 每个方法都逐行标注原文行号；LoadSurface / DrawChr / Run 中依赖纹理与图库的部分
// 走 ActorFamilyEnv 接缝（见各方法注释与 ActorFamilyHerbEnv.cs 的接缝登记）。

// ────────────────────────────────────────────────────────────────────────────
// TKillingHerb（原文 :28）
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TKillingHerb`（原文 `HerbActor.pas:28`，`class(TActor)`）。
/// <para>覆写 <c>CalcActorFrame</c>（157-268）与 <c>GetDefaultFrame</c>（270-299）。</para>
/// </summary>
public partial class TKillingHerb
{
    /// <summary>
    /// `TKillingHerb.CalcActorFrame`（**157-268**）1:1。
    ///
    /// <para>两段结构：**162-173 的变身前置段**（`m_nChangeAppr &gt;= 0` 时把
    /// `SM_ATTACK01..06` 归一化为 `SM_HIT`，然后 `inherited` 并 Exit）+ **174-267 的本体 case 表**。</para>
    ///
    /// <para><b>★ 差异要点（对照 <see cref="TBeeQueen"/>）</b>：</para>
    /// <list type="bullet">
    /// <item>182 / 190：<c>SM_TURN</c> 与 <c>SM_DIGUP</c> 的 <c>start</c> **不加**方向乘法
    ///   （原文该处被注释掉）；</item>
    /// <item>201 / 230 / 239 / 248 / 255：<c>ActAttack/ActStruck/ActDie</c> **都加**
    ///   <c>m_btDir * (frame + skip)</c>；</item>
    /// <item>比 <c>TBeeQueen</c> 多出 <c>SM_DIGUP</c>（用 <c>ActWalk</c>）与 <c>SM_DIGDOWN</c>
    ///   （用 <c>ActDeath</c> 且置 <c>m_boDelActionAfterFinished</c>）。</item>
    /// </list>
    /// </summary>
    public override void CalcActorFrame()
    {
        // 162-173：变身前置段
        if (m_nChangeAppr >= 0)
        {
            // 163-169：六个自定义攻击动作 → SM_HIT（**SM_ATTACK06 是末项**，见 8946..8951）
            if (m_nCurrentAction == TActorCore.SM_ATTACK01
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 1
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 2
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 3
                || m_nCurrentAction == TActorCore.SM_ATTACK01 + 4
                || m_nCurrentAction == TActorCore.SM_ATTACK06)
            {
                m_nCurrentAction = TActorCore.SM_HIT;
            }

            base.CalcActorFrame();          // 171 `inherited;`
            return;                          // 172 `Exit;`
        }

        // 174-178
        m_boUseMagic = false;
        m_nCurrentFrame = -1;
        m_nBodyOffset = GetOffset(m_wAppearance);        // 176

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 177
        if (pmOpt == null)
            return;                                          // 178 `if pm = nil then Exit;`
        var pm = pmOpt.Value;

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_TURN:                          // 181-188
                m_nStartFrame = pm.ActStand.start;            // 182（★ 方向乘法在原文被注释掉）
                m_nEndFrame = m_nStartFrame + pm.ActStand.frame - 1;
                m_dwFrameTime = pm.ActStand.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();   // 185 TimeGetTime
                m_nDefFrameCount = pm.ActStand.frame;
                Shift(m_btDir, 0, 0, 1);
                break;

            case TActorCore.SM_DIGUP:                         // 189-199
                m_nStartFrame = pm.ActWalk.start;             // 190（★ 同上，方向乘法注释掉）
                m_nEndFrame = m_nStartFrame + pm.ActWalk.frame - 1;
                m_dwFrameTime = pm.ActWalk.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_nMaxTick = pm.ActWalk.usetick;              // 194
                m_nCurTick = 0;                               // 195
                // 196 `// WarMode := FALSE;`（原文注释）
                m_nMoveStep = 1;                              // 197
                Shift(m_btDir, 0, 0, 1);                      // 198（★ 后三个实参原文被注释掉）
                break;

            case TActorCore.SM_LIGHTINGEX:                    // 200-228
                m_nStartFrame = pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
                m_nEndFrame = m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime = pm.ActAttack.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                // 205 `// WarMode := TRUE;`（原文注释）
                m_dwWarModeTime = ActorFamilyEnv.MyGetTickCountFn();   // 206
                Shift(m_btDir, 0, 0, 1);

                if (m_nMagicNum > 0)                          // 209-227：施法特效 + 两态取声
                {
                    SetMagicSound();                          // 210
                    PlayNewMagicAndSound();                   // 211-226（走接缝）
                }
                break;

            case TActorCore.SM_HIT:                           // 229-237
                m_nStartFrame = pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
                m_nEndFrame = m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime = pm.ActAttack.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_dwWarModeTime = ActorFamilyEnv.MyGetTickCountFn();   // 235
                Shift(m_btDir, 0, 0, 1);
                break;

            case TActorCore.SM_STRUCK:                        // 238-246
                m_nStartFrame = pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
                m_nEndFrame = m_nStartFrame + pm.ActStruck.frame - 1;
                // 241：★ 用 m_dwStruckFrameTime 而**不是** pm.ActStruck.ftime（原文行尾即此注释）
                m_dwFrameTime = (uint)m_dwStruckFrameTime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                Shift(m_btDir, 0, 0, 1);

                m_CustomMagicStatusEffect_Struck = 0;         // 245
                break;

            case TActorCore.SM_DEATH:                         // 247-253
                m_nStartFrame = pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_nStartFrame = m_nEndFrame;                  // 250：尸体停在末帧
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                break;

            case TActorCore.SM_NOWDEATH:                      // 254-259
                m_nStartFrame = pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                break;

            case TActorCore.SM_DIGDOWN:                       // 260-266
                m_nStartFrame = pm.ActDeath.start;            // 261（★ 不加方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActDeath.frame - 1;
                m_dwFrameTime = pm.ActDeath.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_boDelActionAfterFinished = true;            // 265
                break;

            // 原文 case 无 else ⇒ 其余动作什么都不做
        }
    }

    /// <summary>
    /// `TKillingHerb.GetDefaultFrame`（**270-299**）1:1。
    /// <para>死亡时：<c>m_boSkeleton</c> ⇒ <c>ActDeath.start</c>；否则
    /// <c>ActDie.start + Dir*(frame+skip) + (frame-1)</c>（= 尸体停在末帧）。
    /// 存活时：<c>m_nCurrentDefFrame</c> 越界（&lt;0 或 &gt;= frame）⇒ 0，否则取自身；
    /// 结果 <c>ActStand.start + cf</c>（★ 与 182 不同，**这里没有方向乘法**）。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        if (m_nChangeAppr >= 0)                       // 275-278
            return base.GetDefaultFrame(wmode);       // 276 `inherited GetDefaultFrame(wmode);`

        int result = 0;                               // 279 `Result := 0; // jacky`
        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 280
        if (pmOpt == null)
            return result;                            // 281
        var pm = pmOpt.Value;

        if (m_boDeath)                                // 283-288
        {
            if (m_boSkeleton)
                result = pm.ActDeath.start;                                  // 285
            else
                result = pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip)
                         + (pm.ActDie.frame - 1);                            // 287
        }
        else                                          // 289-298
        {
            m_nDefFrameCount = pm.ActStand.frame;     // 290

            int cf;
            if (m_nCurrentDefFrame < 0)
                cf = 0;                               // 292
            else if (m_nCurrentDefFrame >= pm.ActStand.frame)
                cf = 0;                               // 294
            else
                cf = m_nCurrentDefFrame;              // 296

            result = pm.ActStand.start + cf;          // 297
        }

        return result;
    }

    /// <summary>
    /// 原文 211-226（`SM_LIGHTINGEX` 分支内的施法段）：
    /// <c>PlayScene.NewMagic(...)</c> → 按 <c>bofly</c> 两态取声。
    /// <para><b>接缝：待 <c>PlayScene.NewMagic</c> 移植后接入</b>（见
    /// <c>ActorFamilyHerbEnv.PlaySceneNewMagicFn</c>）。</para>
    /// </summary>
    private void PlayNewMagicAndSound()
    {
        bool? bofly = ActorFamilyHerbEnv.PlaySceneNewMagicFn?.Invoke(this);
        if (bofly == null)
            return;                               // 未接线 ⇒ 与"原文 m_nMagicNum <= 0 走不到这里"同形

        ActorFamilyEnv.PlaySoundByIdFn(bofly.Value
            ? m_nMagicFireSound                       // 224
            : m_nMagicExplosionSound);                // 226
    }
}

// ────────────────────────────────────────────────────────────────────────────
// TMineMon（原文 :37，class(TKillingHerb)）
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TMineMon`（原文 `HerbActor.pas:37`，`class(TKillingHerb)`）。
/// <para><b>全族最短</b>：<c>CalcActorFrame</c>（1166-1170）**完全转调 inherited、无任何自有动作**；
/// <c>GetDefaultFrame</c>（1218-1226）未变身时**恒返回 0**。</para>
/// <para><b>差异断言要点</b>：它**不做任何前置处理** —— 连 <c>m_boUseMagic := FALSE</c> 都没有
/// （那是 <c>TKillingHerb</c> 本体在 174 行做的，而 <c>TMineMon</c> 直接 `inherited` 就进去了，
/// 等于**同样会执行**，但**它自己一行都没写**）。</para>
/// </summary>
public partial class TMineMon
{
    /// <summary>`TMineMon.CalcActorFrame`（**1166-1170**）1:1 —— 只有一句 `inherited;`。</summary>
    public override void CalcActorFrame() => base.CalcActorFrame();

    /// <summary>
    /// `TMineMon.GetDefaultFrame`（**1218-1226**）1:1。
    /// <para>★ 注意与 <c>TKillingHerb.GetDefaultFrame</c> 的**结构差异**：这里是
    /// <c>if ... then begin inherited ... Exit end **else** begin Result := 0 end</c>
    /// —— 未变身分支**没有任何其它副作用**（不读 <c>GetRaceByPM</c>、不写 <c>m_nDefFrameCount</c>）。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        if (m_nChangeAppr >= 0)                       // 1220-1222
            return base.GetDefaultFrame(wmode);       // 1221 `inherited GetDefaultFrame(wmode);`

        return 0;                                     // 1224 `Result := 0; //HZQ 2030524`
    }
}

// ────────────────────────────────────────────────────────────────────────────
// TBeeQueen（原文 :45，class(TActor)）
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TBeeQueen`（原文 `HerbActor.pas:45`，**`class(TActor)`** —— 不是 `TKillingHerb`！）。
/// <para>覆写 <c>CalcActorFrame</c>（303-397）与 <c>GetDefaultFrame</c>（399-425）。</para>
/// <para><b>★ 与 <c>TKillingHerb</c> 的两处差异</b>：① 所有 <c>Act*</c> 的 <c>start</c> 都
/// **不加** <c>m_btDir * (frame + skip)</c>（方向无关）；② **没有** <c>SM_DIGUP</c> 与
/// <c>SM_DIGDOWN</c> 两个分支。</para>
/// </summary>
public partial class TBeeQueen
{
    /// <summary>`TBeeQueen.CalcActorFrame`（**303-397**）1:1。</summary>
    public override void CalcActorFrame()
    {
        // 308-319：变身前置段（与 TKillingHerb 162-173 **逐字相同**）
        if (m_nChangeAppr >= 0)
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

            base.CalcActorFrame();          // 317 `inherited;`
            return;                          // 318 `Exit;`
        }

        // 320-325
        m_boUseMagic = false;
        m_nCurrentFrame = -1;
        m_nBodyOffset = GetOffset(m_wAppearance);        // 323

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 324
        if (pmOpt == null)
            return;                                          // 325
        var pm = pmOpt.Value;

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_TURN:                          // 328-335
                m_nStartFrame = pm.ActStand.start;            // 329（★ 方向乘法注释掉）
                m_nEndFrame = m_nStartFrame + pm.ActStand.frame - 1;
                m_dwFrameTime = pm.ActStand.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_nDefFrameCount = pm.ActStand.frame;
                Shift(m_btDir, 0, 0, 1);
                break;

            case TActorCore.SM_LIGHTINGEX:                    // 336-364
                m_nStartFrame = pm.ActAttack.start;           // 337（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime = pm.ActAttack.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_dwWarModeTime = ActorFamilyEnv.MyGetTickCountFn();   // 342
                Shift(m_btDir, 0, 0, 1);

                if (m_nMagicNum > 0)                          // 345-363
                {
                    SetMagicSound();                          // 346
                    var bofly = ActorFamilyHerbEnv.PlaySceneNewMagicFn?.Invoke(this);
                    if (bofly != null)
                        ActorFamilyEnv.PlaySoundByIdFn(bofly.Value
                            ? m_nMagicFireSound : m_nMagicExplosionSound);
                }
                break;

            case TActorCore.SM_HIT:                           // 365-373
                m_nStartFrame = pm.ActAttack.start;           // 366（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime = pm.ActAttack.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                m_dwWarModeTime = ActorFamilyEnv.MyGetTickCountFn();   // 371
                Shift(m_btDir, 0, 0, 1);
                break;

            case TActorCore.SM_STRUCK:                        // 374-382
                m_nStartFrame = pm.ActStruck.start;           // 375（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActStruck.frame - 1;
                m_dwFrameTime = (uint)m_dwStruckFrameTime;    // 377
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                Shift(m_btDir, 0, 0, 1);

                m_CustomMagicStatusEffect_Struck = 0;         // 381
                break;

            case TActorCore.SM_DEATH:                         // 383-389
                m_nStartFrame = pm.ActDie.start;              // 384（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_nStartFrame = m_nEndFrame;                  // 386
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                break;

            case TActorCore.SM_NOWDEATH:                      // 390-395
                m_nStartFrame = pm.ActDie.start;              // 391（★ 无方向乘法）
                m_nEndFrame = m_nStartFrame + pm.ActDie.frame - 1;
                m_dwFrameTime = pm.ActDie.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                break;

            // ★ 303-397 **没有** SM_DIGUP / SM_DIGDOWN 分支（与 TKillingHerb 的差异点）
        }
    }

    /// <summary>
    /// `TBeeQueen.GetDefaultFrame`（**399-425**）1:1。
    /// <para><b>★ 与 <c>TKillingHerb</c> 的差异</b>：死亡时**没有** <c>m_boSkeleton</c> 分支，
    /// 且 <c>ActDie</c> 的方向乘法项也去掉了 —— 直接 <c>ActDie.start + (frame - 1)</c>。</para>
    /// </summary>
    public override int GetDefaultFrame(bool wmode)
    {
        if (m_nChangeAppr >= 0)                       // 404-407
            return base.GetDefaultFrame(wmode);       // 405

        int result = 0;                               // 408 `Result := 0; // jacky`
        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 409
        if (pmOpt == null)
            return result;                            // 410
        var pm = pmOpt.Value;

        if (m_boDeath)                                // 412-414
        {
            result = pm.ActDie.start + (pm.ActDie.frame - 1);   // 413（★ 无 Dir、无 skip）
        }
        else                                          // 415-424
        {
            m_nDefFrameCount = pm.ActStand.frame;     // 416

            int cf;
            if (m_nCurrentDefFrame < 0)
                cf = 0;                               // 418
            else if (m_nCurrentDefFrame >= pm.ActStand.frame)
                cf = 0;                               // 420
            else
                cf = m_nCurrentDefFrame;              // 422

            result = pm.ActStand.start + cf;          // 423
        }

        return result;
    }
}

// ────────────────────────────────────────────────────────────────────────────
// TBigHeartMon（:66） / TSpiderHouseMon（:71） —— 两者逐字相同
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TBigHeartMon`（原文 `HerbActor.pas:66`，`class(TKillingHerb)`）。
/// <para>`CalcActorFrame`（1230-1234）：**先 `m_btDir := 0` 再 `inherited`**。</para>
/// </summary>
public partial class TBigHeartMon
{
    /// <summary>`TBigHeartMon.CalcActorFrame`（**1230-1234**）1:1。</summary>
    public override void CalcActorFrame()
    {
        m_btDir = 0;                     // 1232
        base.CalcActorFrame();           // 1233
    }
}

/// <summary>
/// `TSpiderHouseMon`（原文 `HerbActor.pas:71`，`class(TKillingHerb)`）。
/// <para>`CalcActorFrame`（1238-1242）与 <see cref="TBigHeartMon"/> **逐字相同** ——
/// 保留为两个类（原文如此），不合并。</para>
/// </summary>
public partial class TSpiderHouseMon
{
    /// <summary>`TSpiderHouseMon.CalcActorFrame`（**1238-1242**）1:1。</summary>
    public override void CalcActorFrame()
    {
        m_btDir = 0;                     // 1240
        base.CalcActorFrame();           // 1241
    }
}

// ────────────────────────────────────────────────────────────────────────────
// TCentipedeKingMon（原文 :52，class(TKillingHerb)）
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// `TCentipedeKingMon`（原文 `HerbActor.pas:52`，`class(TKillingHerb)`）。
/// <para><b>本族最特殊</b>：<c>CalcActorFrame</c>（430-514）除 <c>SM_LIGHTINGEX</c>/<c>SM_HIT</c>
/// 外**一律先 `m_btDir := 0` 再 `inherited`**（含 <c>SM_TURN</c>、<c>SM_DIGDOWN</c> 与 <c>else</c>）；
/// 而这两个攻击分支用 <c>ActCritical</c> 表并打开死亡特效（<c>0..9</c> 帧、62ms）。</para>
/// </summary>
public partial class TCentipedeKingMon
{
    /// <summary>`TCentipedeKingMon.CalcActorFrame`（**430-514**）1:1。</summary>
    public override void CalcActorFrame()
    {
        // 435-446：变身前置段（与 TKillingHerb 162-173 逐字相同）
        if (m_nChangeAppr >= 0)
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

            base.CalcActorFrame();          // 444
            return;                          // 445
        }

        // 447-451
        m_boUseMagic = false;
        m_nCurrentFrame = -1;
        m_nBodyOffset = GetOffset(m_wAppearance);        // 449

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 450
        if (pmOpt == null)
            return;                                          // 451
        var pm = pmOpt.Value;

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_TURN:                          // 454-457
                m_btDir = 0;                                  // 455
                base.CalcActorFrame();                        // 456 `inherited CalcActorFrame;`
                break;

            case TActorCore.SM_LIGHTINGEX:                    // 458-491
                m_btDir = 0;                                  // 459
                m_nStartFrame = pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
                m_nEndFrame = m_nStartFrame + pm.ActCritical.frame - 1;
                m_dwFrameTime = pm.ActCritical.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                // 464-468：死亡特效三段（★ BoUseDieEffect / 0..9 帧 / 62ms）
                BoUseDieEffect = true;
                m_EffectFrame = 0;
                m_nEffectStart = 0;
                m_nEffectEnd = m_nEffectStart + 9;
                m_dwEffectFrameTime = 62;
                // 469 `// BoUseEffect:=True;`（原文注释）
                Shift(m_btDir, 0, 0, 1);

                if (m_nMagicNum > 0)                          // 472-490
                {
                    SetMagicSound();                          // 473
                    var bofly = ActorFamilyHerbEnv.PlaySceneNewMagicFn?.Invoke(this);
                    if (bofly != null)
                        ActorFamilyEnv.PlaySoundByIdFn(bofly.Value
                            ? m_nMagicFireSound : m_nMagicExplosionSound);
                }
                break;

            case TActorCore.SM_HIT:                           // 492-505
                m_btDir = 0;                                  // 493
                m_nStartFrame = pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
                m_nEndFrame = m_nStartFrame + pm.ActCritical.frame - 1;
                m_dwFrameTime = pm.ActCritical.ftime;
                m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();
                BoUseDieEffect = true;                        // 498
                m_EffectFrame = 0;                            // 499
                m_nEffectStart = 0;                           // 500
                m_nEffectEnd = m_nEffectStart + 9;            // 501
                m_dwEffectFrameTime = 62;                     // 502
                // 503 `// BoUseEffect:=True;`（原文注释）
                Shift(m_btDir, 0, 0, 1);
                break;

            case TActorCore.SM_DIGDOWN:                       // 506-508
                // ★ 这一支**没有** m_btDir := 0（与上面两个分支不同）
                base.CalcActorFrame();                        // 507
                break;

            default:                                          // 509-512：else
                m_btDir = 0;                                  // 510
                base.CalcActorFrame();                        // 511
                break;
        }
    }

    /// <summary>`TCentipedeKingMon.DrawEff`（**1177-1182**）1:1 —— 攻击特效图非 nil 时混合绘制。</summary>
    public override void DrawEff(int dx, int dy)
    {
        if (m_boUseEffect)
        {
            if (AttackEffectSurface != null)
            {
                ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                    SurfaceDrawKind.DrawBlend,
                    dx + ax + m_nShiftX,
                    dy + ay + m_nShiftY));
            }
        }
    }

    /// <summary>
    /// `TCentipedeKingMon.LoadEffect`（**1184-1200**）1:1 —— 图号 =
    /// <c>100 + m_EffectFrame - m_nEffectStart</c>，图库 <c>g_WMonImages.Indexs[15]</c>。
    /// <para><b>接缝：待 <c>g_WMonImages.Indexs[15]</c> 图库接缝接入</b>
    /// （既有 `ActorFamilyEnv.FetchBodySurfaceFn` 只按 `ActorBodyImage` 取图，
    /// 不表达"图集下标"维度）。未接线时 <c>AttackEffectSurface</c> 保持 nil ⇒ 1179 的门为假。</para>
    /// </summary>
    public override void LoadEffect()
    {
        AttackEffectSurface = null;                      // 1186
        if (!m_boUseEffect)
            return;                                      // 1187

        int imageIndex = 100 + m_EffectFrame - m_nEffectStart;   // 1190/1193/1197
        AttackEffectSurface = ActorFamilyHerbEnv.FetchMonImagesIndex15Fn(
            imageIndex, ax, ay, ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect));
    }

    /// <summary>`TCentipedeKingMon.Finalize`（**1202-1206**）1:1。</summary>
    public override void Finalize()
    {
        base.Finalize();                                 // 1204
        AttackEffectSurface = null;                      // 1205
    }

    /// <summary>
    /// `TCentipedeKingMon.LoadSurface`（**1208-1216**）1:1。
    /// <para>★ 注意它**没有**自己清 <c>m_dwLoadSurfaceTime</c>/<c>m_boLoadSurface</c> ——
    /// 那是 <c>inherited LoadSurface(Sender)</c>（原文 1214）做的。</para>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        if (m_nChangeAppr >= 0)                          // 1210-1213
        {
            base.LoadSurface(sender);                    // 1211 `inherited;`（无参形态，转调基类）
            return;                                      // 1212
        }

        base.LoadSurface(sender);                        // 1214 `inherited LoadSurface(Sender);`
        LoadEffect();                                    // 1215
    }

    /// <summary>
    /// `TCentipedeKingMon.Run`（**1244-1284**）1:1。
    /// <para>结构：① 四个移动动作直接 Exit（1248-1254）；② <c>CheckLoadSurface</c>（1256，
    /// 本托管版走既有接缝）；③ <c>BoUseDieEffect</c> 到第 5 帧时切到 <c>m_boUseEffect</c>
    /// 并重置特效游标（1259-1267）；④ 特效帧推进（1268-1278）；⑤ 特效帧变了就重载 Surface
    /// （1280-1281）；⑥ `inherited`（1283）。</para>
    /// <para><b>接缝：<c>CheckLoadSurface</c>（原文 7355-7360）与 <c>Map</c> 未移植</b> ——
    /// 见 `ActorFamilyEnv` 的登记；此处不调用（原文该调用的效果是"到点后经
    /// <c>PlayScene.LoadSurface</c> 延迟装载"，托管侧由 <c>RequestLoadSurfaceFn</c> 承载）。</para>
    /// </summary>
    public override void Run(uint now)
    {
        // 1248-1254：移动族直接 Exit（注意原文列的四个动作与基类 IsMoveAction 不同）
        if (m_nCurrentAction == TActorCore.SM_WALK
            || m_nCurrentAction == TActorCore.SM_BACKSTEP
            || m_nCurrentAction == TActorCore.SM_HORSERUN
            || m_nCurrentAction == TActorCore.SM_RUN)
        {
            return;
        }

        // 1256 `CheckLoadSurface;` —— 接缝：见方法注释（未接线时不产生副作用）

        int nEffectFrame = m_EffectFrame;                 // 1258

        if (BoUseDieEffect)                               // 1259-1267
        {
            if ((m_nCurrentFrame - m_nStartFrame) >= 5)   // 1260（★ >= 5，不是 > 5）
            {
                BoUseDieEffect = false;                   // 1261
                m_boUseEffect = true;                     // 1262
                m_dwEffectStartTime = ActorFamilyEnv.MyGetTickCountFn();   // 1263
                m_EffectFrame = 0;                        // 1264
                LoadEffect();                             // 1265
            }
        }

        if (m_boUseEffect)                                // 1268-1278
        {
            if ((ActorFamilyEnv.MyGetTickCountFn() - m_dwEffectStartTime) > m_dwEffectFrameTime)
            {
                m_dwEffectStartTime = ActorFamilyEnv.MyGetTickCountFn();   // 1270
                if (m_EffectFrame < m_nEffectEnd)         // 1271
                {
                    m_EffectFrame++;                      // 1272 `Inc(m_nEffectFrame)`
                    LoadEffect();                         // 1273
                }
                else
                {
                    m_boUseEffect = false;                // 1276
                }
            }
        }

        if (nEffectFrame != m_EffectFrame)                // 1280-1281
            LoadSurface(this);                            // 1281 `LoadSurface(self)`

        base.Run(now);                                    // 1283 `inherited;`
    }
}
