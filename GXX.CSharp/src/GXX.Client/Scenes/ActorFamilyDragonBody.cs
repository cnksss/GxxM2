using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

// ════════════════════════════════════════════════════════════════════════════
// 车道 p7-client-actor-family：TDragonBody（HerbActor.pas:132，class(TKillingHerb)）
// ════════════════════════════════════════════════════════════════════════════

/// <summary>
/// `TDragonBody`（原文 `HerbActor.pas:132`，`class(TKillingHerb)`，Size 0x5a）。
/// <para>覆写 `DrawEff`（1310-1315）、`CalcActorFrame`（1288-1308）、`LoadSurface`（1317-1339，带参）。</para>
/// </summary>
public partial class TDragonBody
{
    /// <summary>
    /// `TDragonBody.CalcActorFrame`（**1288-1308**）1:1。
    ///
    /// <para><b>★ 本族最简单的 CalcActorFrame</b>：帧区间与帧时长全是**硬编码数值常量**
    /// （`m_nStartFrame := 0` / `m_nEndFrame := 1` / `m_dwFrameTime := 400`），
    /// 完全不用 <c>pm</c> 的动作表字段 —— 只把 <c>pm</c> 当"外观有效"的门。</para>
    ///
    /// <para><b>注意它没有变身前置段</b>（不像 TKillingHerb/TBeeQueen/TCentipedeKingMon/墙系/门）——
    /// 直接 `m_btDir := 0` 起手。</para>
    /// </summary>
    public override void CalcActorFrame()
    {
        m_btDir = 0;                                     // 1292
        m_boUseMagic = false;                            // 1293
        m_nCurrentFrame = -1;                            // 1294
        m_nBodyOffset = GetOffset(m_wAppearance);        // 1295

        var pmOpt = ActorActionTables.GetRaceByPM(m_btRace, m_wAppearance);   // 1296
        if (pmOpt == null)
            return;                                      // 1297
        var pm = pmOpt.Value;

        if (m_nCurrentAction == TActorCore.SM_DIGUP)     // 1298-1303
        {
            m_nMaxTick = pm.ActWalk.ftime;               // 1299（★ 用 ftime 而非 usetick，原文如此）
            m_nCurTick = 0;                              // 1300
            m_nMoveStep = 1;                             // 1301
            Shift(m_btDir, 0, 0, 1);                     // 1302
        }

        // 1304-1307：★ 硬编码，与动作表无关
        m_nStartFrame = 0;                               // 1304
        m_nEndFrame = 1;                                 // 1305
        m_dwFrameTime = 400;                             // 1306
        m_dwStartTime = ActorFamilyEnv.MyGetTickCountFn();   // 1307 `TimeGetTime()`
    }

    /// <summary>`TDragonBody.DrawEff`（**1310-1315**）1:1 —— 方向守卫 + 主体图混合绘制。</summary>
    public override void DrawEff(int dx, int dy)
    {
        if (!(m_btDir is >= 0 and <= 7))                 // 1312 `if not (m_btDir in [0..7]) then Exit;`
            return;

        if (m_BodySurface != null)                       // 1313-1314
        {
            ActorFamilyEnv.DrawEffSurfaceOpFn(new SurfaceDrawOp(
                SurfaceDrawKind.DrawBlend,
                dx + m_nPx + m_nShiftX,
                dy + m_nPy + m_nShiftY));
        }
    }

    /// <summary>
    /// `TDragonBody.LoadSurface`（**1317-1339**）1:1（**原文签名带 <c>Sender</c>**）。
    ///
    /// <para><b>★ 与基类 <c>TActor.LoadSurface</c> 的三处差异</b>：</para>
    /// <list type="number">
    /// <item>它**没有** <c>GameCanvas.Active/Initialized</c> 前置守卫（直接进 1323 打点）；</item>
    /// <item>图库是 <c>g_WDragonImg</c>（**不是** <c>g_WMonImages</c>），
    ///   且图号是 <c>GetOffset(m_wAppearance)</c> —— **不加 <c>m_nCurrentFrame</c>**；</item>
    /// <item>取图 y 用 <c>m_nHpy</c>（**不是** <c>m_nPy</c>），且**没有**反向帧分支；</item>
    /// <item>末尾除 <c>ActionChanged</c> 外还调 <c>LoadActorIcons</c>（1337）。</item>
    /// </list>
    /// </summary>
    public override void LoadSurface(object? sender)
    {
        if (m_nChangeAppr >= 0)                          // 1319-1322
        {
            base.LoadSurface(sender);                    // 1320 `inherited;`（无参形态）
            return;                                      // 1321
        }

        m_dwLoadSurfaceTime = ActorFamilyEnv.MyGetTickCountFn();   // 1323
        m_boLoadSurface = false;                                   // 1324
        m_BodySurface = null;                                      // 1325

        // 1330-1335：g_WDragonImg 取图（图号 = GetOffset(外观)，**不含当前帧**；y 偏移用 m_nHpy）
        string kind = ActorFamilyImpl.ActorBodyImageFetchKind(ActorColorEffect);
        m_BodySurface = ActorFamilyHerbEnv.FetchDragonImgFn(
            GetOffset(m_wAppearance), m_nPx, m_nHpy, kind);

        LoadActorIcons();                                // 1337
        ActionChanged();                                 // 1338
    }
}
