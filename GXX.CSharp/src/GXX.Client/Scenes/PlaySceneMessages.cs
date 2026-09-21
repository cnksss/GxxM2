using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// PlayScn.pas 消息驱动与角色生命周期（批次J73）：
/// ClearDropItem（735-842）、CleanObjects（600-724）、ActorDied（7489-7525）、
/// SetActorDrawLevel（7527-7533）、ClearActors（7535-7659）、DeleteActor（7661-7696）、
/// DelActor（7698-7716）、AddEffectList（4374-4387）、ProcessActors（906-1336）、
/// SendMsg（7758-8271 结构性主体）。
///
/// 语义要点：Actor 删除只登记 m_boDelActor/m_boFreeActor，真正移出 ActorList 由
/// ProcessActors 的 DoDelActor 分支完成（原文 1035-1057 的注释明确要求）。
/// 位图族（GameCanvas/CurrentFont/frmMain/TSerialWindows）以接缝承载，见文末接口。
/// </summary>
public partial class TPlayScene
{
    // ---- 特效三表（PlayScn.pas 字段；ClearActors/ProcessActors 遍历） ----
    public readonly List<TMagicEff> GroundEffectList = new(); // m_GroundEffectList
    public readonly List<TMagicEff> EffectList = new();       // m_EffectList
    public readonly List<TMagicEff> FlyList = new();          // m_FlyList
    public readonly List<TMagicEff> FreeEffectList = new();   // m_FreeEffectList（AddFreeEffectList）

    public readonly List<TMagicEff> DrawGroundEffectList = new(); // m_DrawGroundEffectList
    public readonly List<TMagicEff> DrawFlyList = new();          // m_DrawFlyList

    // ---- 接缝 ----
    public FrmMainSeam? FrmMain;
    /// <summary>clEvent TClEventManager（CleanObjects/ClearActors 清事件）。</summary>
    public ClEventManager? EventMan;
    /// <summary>DelChangeFace（换角等待完成后的变脸表清理）。</summary>
    public Action<long>? DelChangeFaceFn;
    public void DelChangeFace(long recogId) => DelChangeFaceFn?.Invoke(recogId);

    // ---- ClearDropItem 接缝 ----
    /// <summary>g_ConfigDlg.ConfigCheckeds[ckSpecialQuickFlashing]。</summary>
    public bool ckSpecialQuickFlashing;
    /// <summary>g_dwDropItemFlashTime（缺省 5000）。</summary>
    public uint DropItemFlashTime = DropItemFx.DefaultFlashInterval;
    /// <summary>g_boShowAllItem（快捷显示全部物品名）。</summary>
    public bool ShowAllItem;
    /// <summary>内挂启用（PlugInEnabled）。</summary>
    public bool PlugInEnabled;
    /// <summary>GameCanvas.Active and Initialized 等效（true = 可取图/生成名字）。</summary>
    public bool CanvasActive = true;
    /// <summary>名字图生成接缝（CurrentFont.GetImageInfo；null = 不生成）。</summary>
    public Func<string, (int W, int H)?>? NameImageFn;
    /// <summary>物品图选择接缝（0=Images / 1=Brights / 2=Grays）。</summary>
    public Func<DropItem, int, SurfaceSize?>? DropItemTextureFn;
    /// <summary>g_FocusItem / g_OldFocusItem。</summary>
    public DropItem? FocusItem;
    public DropItem? OldFocusItem;
    /// <summary>g_ConfigClient.boOverLapItemNumOldShow（重叠数量旧格式 ' (n)' / 新格式 'xn'）。</summary>
    public bool OverlapItemNumOldShow;

    /// <summary>AddFreeEffectList（Delphi 释放链；headless 入回收表）。</summary>
    public void AddFreeEffectList(TMagicEff eff) => FreeEffectList.Add(eff);

    /// <summary>AddEffectList（PlayScn.pas 4374-4387 1:1）：地面符类入 m_GroundEffectList，其余入 m_EffectList。</summary>
    public void AddEffectList(TMagicEff magicEff)
    {
        if (magicEff.MagicType == TMagicType.mtBujaukGroundEffect)
            GroundEffectList.Add(magicEff);
        else
            EffectList.Add(magicEff);
    }

    // ================= ClearDropItem（735-842） =================

    /// <summary>
    /// PlayScn.pas 735-842 ClearDropItem 1:1：跳过空点表 → RefreshDrawList →
    /// 快闪窗口（特殊物品 300ms，否则 g_dwDropItemFlashTime）→ 20ms 步进闪烁（≥10 步关闪）→
    /// 图选择（主角死亡用灰图 / 焦点物品用亮图）→ 焦点物品新旧交替 → 名字图生成或清除。
    /// </summary>
    public void ClearDropItem()
    {
        if (G.MySelf == null)
            return;
        if (DropItems == null)
            return;

        uint now = SceneTime.TickNow();

        foreach (var list in DropItems.Points)
        {
            if (list.Items.Count == 0)
                continue;

            list.RefreshDrawList();

            foreach (var dropItem in list.DrawItems)
            {
                bool showSpecial = dropItem.ShowItem is { ShowSpecial: true };
                uint interval = DropItemFx.FlashInterval(ckSpecialQuickFlashing, showSpecial);
                DropItemFx.UpdateFlash(dropItem.Fx, now, interval);

                if (CanvasActive)
                {
                    if (G.MySelf.m_boDeath)
                        dropItem.ItemTexture = DropItemTextureFn?.Invoke(dropItem, 2); // Grays
                    else if (ReferenceEquals(dropItem, FocusItem))
                        dropItem.ItemTexture = DropItemTextureFn?.Invoke(dropItem, 1); // Brights
                    else
                        dropItem.ItemTexture = DropItemTextureFn?.Invoke(dropItem, 0); // Images
                }

                if (ReferenceEquals(dropItem, FocusItem))
                {
                    if (!ReferenceEquals(OldFocusItem, FocusItem))
                        OldFocusItem = FocusItem;
                }

                if (FocusItem == null && OldFocusItem != null)
                {
                    if (ReferenceEquals(OldFocusItem, dropItem))
                        OldFocusItem = null;
                }

                // 修复内挂 物品显示 无效BUG --- piaoyun 2013-09-07
                bool showItemName = PlugInEnabled && dropItem.ShowItem is { ShowName: true };
                dropItem.ShowName = showItemName || ShowAllItem;

                if (CanvasActive && dropItem.ShowName && NameImageFn != null)
                {
                    if (!dropItem.NameImageGenerated)
                    {
                        string s = dropItem.Name;
                        if (dropItem.OverlapCount > 1)
                        {
                            s = OverlapItemNumOldShow
                                ? s + " (" + dropItem.OverlapCount + ")"
                                : s + "x" + dropItem.OverlapCount;
                        }

                        var info = NameImageFn(s);
                        if (info != null)
                            dropItem.SetNameImage(info.Value.W, info.Value.H);
                    }
                }
                else
                {
                    ClearDropItemName(dropItem);
                }
            }
        }
    }

    /// <summary>ClearDropItemName（PlayScn.pas 728-733 1:1）：名字图索引/宽高清空。</summary>
    public static void ClearDropItemName(DropItem dropItem) => dropItem.ClearNameImage();

    // ================= CleanObjects（600-724） =================

    /// <summary>
    /// PlayScn.pas 600-724 CleanObjects 1:1：清绘制表 → 保留主角与英雄、回收其余角色 →
    /// 处理主角最后消息并清双方队列 → 清全局指向 → 回收三张特效表 → 清事件与掉落物 → 掉落索引归零。
    /// </summary>
    public void CleanObjects()
    {
        DrawActorList.Clear();

        for (int i = ActorList.Count - 1; i >= 0; i--)
        {
            var actor = ActorList[i];
            if (!ReferenceEquals(actor, G.MySelf))
            {
                if (!ReferenceEquals(actor, G.MyHero))
                {
                    actor.m_boGhost = true;
                    AddFreeActorList(actor);

                    // 原文 620：仅「非主角且非英雄」才移出角色表
                    ActorList.RemoveAt(i);
                }
            }
        }

        if (G.MySelf != null)
            DrawActorList.Add(G.MySelf);

        MsgList.Clear();

        if (G.MySelf != null)
        {
            G.MySelf.ProcLastMsg();
            G.MySelf.CleanMsgs();
        }

        if (G.MyHero != null)
            G.MyHero.CleanMsgs();

        G.TargetCret = null;
        G.FocusCret = null;
        G.FocusCretTick = SceneTime.TickNow();
        G.MagicTarget = 0;
        G.BrightActor = null;

        FreeEffectsOf(GroundEffectList);

        FreeEffectsOf(EffectList);
        FreeEffectsOf(FlyList);

        EventMan?.ClearEvents(SceneTime.TickNow());

        FrmMain?.ClearDropItems();

        m_nProcDropItemsIdx = 0;
    }

    private void FreeEffectsOf(List<TMagicEff> list)
    {
        foreach (var eff in list)
        {
            eff.m_dwGhostTick = (int)SceneTime.TickNow();
            AddFreeEffectList(eff);
        }
        list.Clear();
    }

    // ================= 角色生命周期（7489-7716） =================

    /// <summary>
    /// PlayScn.pas 7489-7525 ActorDied 1:1：函数体为空。原作者注释「DoDelActor(Actor) 加这个会有内存泄露」，
    /// 旧的重排 for 循环整体注释保留，此处同样不执行任何动作。
    /// </summary>
    public void ActorDied(TActorCore actor)
    {
        // 原文 7495-7524 全部为注释（含 m_ActorList.Delete/Insert 重排逻辑）
    }

    /// <summary>PlayScn.pas 7527-7533 SetActorDrawLevel 1:1（仅 Level=0：移到绘制表首位；其余等级原文无操作）。</summary>
    public void SetActorDrawLevel(TActorCore actor, int level)
    {
        if (level == 0)
        {
            DrawActorList.Remove(actor);
            DrawActorList.Insert(0, actor);
        }
    }

    /// <summary>PlayScn.pas 7661-7696 DeleteActor 1:1（先移出绘制表；英雄 boFreeActor=false 且清消息，其余 true）。</summary>
    public void DeleteActor(long id)
    {
        TActorCore? actor = DoSearchActor(id, out int index) ? ActorList[index] : null;

        if (actor != null)
        {
            DrawActorList.Remove(actor);

            if (ReferenceEquals(actor, G.MyHero))
            {
                actor.m_dwDeleteTime = SceneTime.TickNow();
                actor.m_boDelActor = true;
                actor.m_boFreeActor = false;
                actor.CleanMsgs();
            }
            else
            {
                actor.m_dwDeleteTime = SceneTime.TickNow();
                actor.m_boDelActor = true;
                actor.m_boFreeActor = true;
            }
        }
    }

    /// <summary>PlayScn.pas 7698-7716 DelActor 1:1（仅在角色表中命中时才登记删除）。</summary>
    public void DelActor(TActorCore actor)
    {
        if (DoSearchActor(actor.m_nRecogId, out _))
        {
            actor.m_dwDeleteTime = SceneTime.TickNow();
            actor.m_boDelActor = true;
            actor.m_boFreeActor = true;
        }
    }

    /// <summary>
    /// PlayScn.pas 7535-7659 ClearActors 1:1：全表回收（含英雄）→ 清绘制/消息/排序表 →
    /// 主角最后消息 + 双方清队列 → 清全部全局指向与锁定 → 回收三张特效表 → 清事件与掉落物。
    /// </summary>
    public void ClearActors()
    {
        for (int i = ActorList.Count - 1; i >= 0; i--)
        {
            var actor = ActorList[i];
            actor.m_boGhost = true;
            AddFreeActorList(actor);
            ActorList.RemoveAt(i);
        }

        if (G.MyHero != null)
        {
            G.MyHero.m_boGhost = true;
            AddFreeActorList(G.MyHero);
        }

        DrawActorList.Clear();
        MsgList.Clear();
        SortYDrawActorList.Clear();

        if (G.MySelf != null)
        {
            G.MySelf.ProcLastMsg();
            G.MySelf.CleanMsgs();
        }

        if (G.MyHero != null)
            G.MyHero.CleanMsgs();

        G.BrightActor = null;
        G.MySelf = null;
        G.MyHero = null;
        G.TargetCret = null;
        G.FocusCret = null;
        G.FocusCretTick = SceneTime.TickNow();
        G.MagicTarget = 0;

        G.LockTarget = null;
        G.dwLockTargetTick = SceneTime.TickNow();

        FreeEffectsOf(GroundEffectList);
        FreeEffectsOf(EffectList);
        FreeEffectsOf(FlyList);

        EventMan?.ClearEvents(SceneTime.TickNow());

        FrmMain?.ClearDropItems();

        m_nProcDropItemsIdx = 0;
    }

    // ================= ProcessActors（906-1336） =================

    /// <summary>g_boAppExit（ProcessActors 循环提前退出）。</summary>
    public bool boAppExit;

    /// <summary>g_nRenderCode（异常诊断用渲染阶段码）。</summary>
    public int m_nRenderCode;

    /// <summary>
    /// PlayScn.pas 906-1336 ProcessActors 1:1（结构性主体）：
    /// ① 清绘制表与全局标记 → ② 移动节拍 dwStepMoveTime/dwRunIntervalTime（含 m_nMoveSpeed 折算）
    /// → ③ 动画节拍 50ms/m_nAniCount 回卷 → ④ 角色表遍历（清 m_boCanDraw；movetick 解锁末帧；
    /// ProcMsg → movetick 时 DoMove 命中则 ++nIdx 重来 → Run → ProcHurryMsg；
    /// 换角等待（IsIdle → NewActor 接管名字/体色/名色，旧角登记回收，魔法目标改指）
    /// → 删除回收（DoDelActor 命中则原地重来，boFreeActor 入回收表））
    /// → ⑤ 地面特效表与飞行表推进。
    /// Actor 的 ProcMsg/Run/DoMove 等以接缝承载（headless 无位图与渲染）。
    /// </summary>
    public void ProcessActors()
    {
        boCanDrawTileMap = false;
        if (G.MySelf == null)
            return;

        DrawGroundEffectList.Clear();
        DrawFlyList.Clear();

        boDoFastFadeOut = false;
        bool movetick = false;
        m_nRenderCode = 0;

        // 修改移动帧间隔 chongchong 2018-07-04 23:14:16
        int dwRunIntervalTime = Math.Max(
            (int)clientConfig_dwMoveFrameTime - (int)clientConfig_dwIncMoveSpeedDecInterval * G.MySelf.m_nMoveSpeed,
            0);

        int dwStepMoveTime = dwRunIntervalTime / 6;
        int sub = (int)Math.Round(dwStepMoveTime * clientConfig_nMoveSpeed / 1000.0, MidpointRounding.AwayFromZero);
        dwStepMoveTime = Math.Max(dwStepMoveTime - sub, 1);

        m_nRenderCode = 1;

        if (SceneTime.TickNow() - m_dwMoveTime >= (uint)dwStepMoveTime)
        {
            m_dwMoveTime = SceneTime.TickNow();
            movetick = true;
            m_nMoveStepCount++;
            if (m_nMoveStepCount > 1)
                m_nMoveStepCount = 0;
        }

        m_nRenderCode = 2;
        if (SceneTime.TickNow() - m_dwAniTime >= 50)
        {
            m_dwAniTime = SceneTime.TickNow();
            m_nAniCount++;
            if (m_nAniCount > 100000)
                m_nAniCount = 0;
        }

        m_nRenderCode = 3;

        int nIdx = 0;
        while (true)
        {
            if (boAppExit)
                break;
            if (nIdx >= ActorList.Count)
                break;

            var actor = ActorList[nIdx];
            actor.m_boCanDraw = false;

            if (!actor.m_boDelActor)
            {
                // 处理角色消息移到这里，移动速度更均匀 chongchong 2018-07-07 18:27:07
                if (movetick)
                    actor.m_boLockEndFrame = false;

                if (!actor.m_boLockEndFrame)
                {
                    actor.ProcMsg();

                    if (movetick)
                    {
                        if (actor.DoMove(m_nMoveStepCount))
                        {
                            nIdx++;
                            continue;
                        }
                    }

                    actor.RunTick();

                    if (!ReferenceEquals(actor, G.MySelf))
                        actor.ProcHurryMsg();
                }

                if (ReferenceEquals(actor, G.MySelf))
                    actor.ProcHurryMsg();

                if (actor.m_nWaitForRecogId != 0 && actor.IsIdle)
                {
                    DelChangeFace(actor.m_nWaitForRecogId);
                    var waitActor = NewActor(actor.m_nWaitForRecogId, actor.m_nCurrX, actor.m_nCurrY,
                        actor.m_btDir, actor.m_WaitForFeature!, actor.m_nWaitForStatus, false);

                    // 修正神兽爬下后，名字不显示，非要移上去才显示  chongchong 2016-04-30
                    if (waitActor != null)
                    {
                        waitActor.m_sUserName = actor.m_sUserName;
                        waitActor.m_btBodyColor = actor.m_btBodyColor;
                        waitActor.m_nNameColor = actor.m_nNameColor;
                    }

                    actor.m_nWaitForRecogId = 0;
                    actor.m_dwDeleteTime = SceneTime.TickNow();
                    actor.m_boDelActor = true;
                    actor.m_boFreeActor = true;
                    if (G.MagicTarget == actor.m_nRecogId)
                        G.MagicTarget = waitActor?.m_nRecogId ?? 0;
                }
            }

            if (actor.m_boDelActor)
            {
                m_nRenderCode = 4;

                // 不能直接删除 nIdx 元素，会有问题 chongchong 2018-08-29 10:58:41
                if (!DoDelActor(actor))
                    nIdx++;

                m_nRenderCode = 5;
                if (actor.m_boFreeActor)
                {
                    actor.m_boGhost = true;
                    AddFreeActorList(actor);
                }
                m_nRenderCode = 6;
                m_nRenderCode = 7;
            }
            else
            {
                nIdx++;
            }
        }

        m_nRenderCode = 8;

        ProcessEffectList(GroundEffectList, DrawGroundEffectList, 9);
        ProcessEffectList(FlyList, DrawFlyList, 10);
    }

    /// <summary>特效表推进（ProcessActors 1074 起结构 1:1）：Run 未结束入绘制表；结束则记 ghost tick 并回收。</summary>
    private void ProcessEffectList(List<TMagicEff> src, List<TMagicEff> draw, int codeBase)
    {
        int nIdx = 0;
        while (true)
        {
            if (boAppExit)
                break;
            if (nIdx >= src.Count)
                break;

            var eff = src[nIdx];
            if (eff.m_boActive)
            {
                if (!eff.Run())
                {
                    m_nRenderCode = codeBase;
                    eff.m_dwGhostTick = (int)SceneTime.TickNow();
                    src.RemoveAt(nIdx);
                    AddFreeEffectList(eff);
                    continue;
                }

                draw.Add(eff);
            }

            nIdx++;
        }
    }

    /// <summary>g_boDoFastFadeOut（场景淡出加速标记）。</summary>
    public bool boDoFastFadeOut;

    // ---- 移动/动画节拍配置（g_ClientConfig 子集；缺省取自 ConfigClient 出厂值） ----
    public uint clientConfig_dwMoveFrameTime = 150;
    public uint clientConfig_dwIncMoveSpeedDecInterval = 50;
    public int clientConfig_nMoveSpeed;
    public uint m_dwMoveTime;
    public uint m_dwAniTime;
    public int m_nMoveStepCount;
    public int m_nAniCount;

    // ================= SendMsg（7758-8271） =================

    /// <summary>
    /// PlayScn.pas 7758-8271 SendMsg 1:1（结构性主体）：
    /// SM_CHANGEMAP/SM_NEWMAP（换图 + 新图时主角重登延后删除）、
    /// SM_HIDE/SM_DISAPPEARMYHERO（清英雄窗体与全局，再按英雄/普通两路登记回收）、
    /// 兜底分支（按需建角 + 死亡瞬间补标记 + 移动/死亡族方向与照明处理 + 骷髅标记 +
    /// 近身开盾 + 好友/敌人近身提示 + SM_FEATURECHANGED 简装替换 + SM_CHARSTATUSCHANGED 状态位
    /// + 落到 Actor.SendMsg 入队）。
    /// SM_TEST / SM_LOGON / SM_HEROLOGON 依赖登录窗体与配置族，见 §6 待深化。
    /// </summary>
    public TActorCore? SendMsg(int ident, long chrid, int x, int y, int cdir, TFeature? feature,
        long state, string str)
    {
        TActorCore? actor = null;

        switch (ident)
        {
            case SM_CHANGEMAP:
            case SM_NEWMAP:
            {
                g_sMapName = str;
                Map.LoadMap(str, x, y);
                Map.OldMapName = "";
                g_nDarkLevel = cdir;
                boViewFog = cdir != 0;

                if (boViewFog)
                {
                    switch (g_nDarkLevel)
                    {
                        case 1: g_nDarkValue = 10; break;
                        case 2: g_nDarkValue = 160; break;
                    }
                }

                if (boViewMiniMap || Map.MiniMapVisible)
                {
                    g_nMiniMapIndex = -1;
                    FrmMain?.SendWantMiniMap(false);
                }
                else
                {
                    g_nMiniMapIndex = -1;
                    if (clientConfig_boUseFindPath && Map.CheckDMinMapBigDlgVisible)
                        FrmMain?.SendWantMiniMap(false);
                    else if (Map.IsDGJPointsShow)
                        FrmMain?.SendWantMiniMap(true);
                }

                if (ident == SM_NEWMAP && G.MySelf != null)
                {
                    G.MySelf.m_nCurrX = x;
                    G.MySelf.m_nCurrY = y;
                    G.MySelf.m_nRx = x;
                    G.MySelf.m_nRy = y;
                    DelActor(G.MySelf);
                    G.MySelf = null;
                }

                boCanDrawTileMap = true;
                break;
            }

            case SM_HIDE:
            case SM_DISAPPEARMYHERO:
            {
                actor = FindActor(chrid);

                // 修复英雄尸体清理后偶尔图标不会消失 chongchong 2013-11-23
                if (ident == SM_DISAPPEARMYHERO)
                {
                    FrmMain?.CloseDHeroStateDlg();
                    FrmMain?.CloseDHeroItemBagDlg();
                    FrmMain?.CloseDHeroStateWinDlg();
                    FrmMain?.ClearWaitingHeroUseItem();
                    FrmMain?.ClearHeroEatingItem();
                    G.MyHero = null;
                }

                if (actor == null)
                    return null;

                // 神兽变身才用了这个东东（X 低位、Y 高位合成新 chrid）
                long newchrid = (uint)x | ((long)(uint)y << 32);
                if (newchrid != 0 && ReferenceEquals(G.g_MagicLockActor, actor))
                {
                    G.g_MagicLockActor = FindActor(newchrid);
                    if (G.g_MagicLockActor == null)
                        G.g_nMagicTargetRecogId = newchrid;
                }

                if (actor.m_boDelActionAfterFinished)
                    return actor;

                if (actor.m_nWaitForRecogId != 0)
                    return actor;

                if (ReferenceEquals(G.MyHero, actor))
                {
                    actor.m_dwDeleteTime = SceneTime.TickNow();
                    actor.m_boDelActor = true;
                    actor.m_boFreeActor = false;
                    actor.CleanMsgs();
                }
                else
                {
                    actor.m_dwDeleteTime = SceneTime.TickNow();
                    actor.m_boDelActor = true;
                    actor.m_boFreeActor = true;
                }
                break;
            }

            default:
            {
                actor = FindActor(chrid);

                if (actor == null && feature != null
                    && (ident == TActorCore.SM_TURN || ident == TActorCore.SM_DEATH || ident == TActorCore.SM_NOWDEATH
                        || ident == TActorCore.SM_SKELETON || ident == TActorCore.SM_DIGUP || ident == TActorCore.SM_ALIVE))
                {
                    actor = NewActor(chrid, x, y, LoByte(cdir), feature, state);

                    // 一个对象刚好死亡一瞬间发过来，后面可能没有动作，导致这个对象永不死亡 chongchong 2018-08-25 23:55:32
                    if (actor != null && (ident == TActorCore.SM_DEATH || ident == TActorCore.SM_NOWDEATH))
                    {
                        actor.m_boStruckShowNumber = false;
                        actor.m_boShowBigHPProgress = false;
                        actor.m_boDeath = true;
                        actor.m_dwDeathTick = SceneTime.TickNow();
                    }
                }

                if (ident == TActorCore.SM_TURN || ident == TActorCore.SM_RUN || ident == TActorCore.SM_HORSERUN || ident == TActorCore.SM_WALK
                    || ident == TActorCore.SM_BACKSTEP || ident == TActorCore.SM_MAGICMOVE
                    || ident == TActorCore.SM_DEATH || ident == TActorCore.SM_NOWDEATH || ident == TActorCore.SM_SKELETON
                    || ident == TActorCore.SM_DIGUP || ident == TActorCore.SM_ALIVE
                    || (ident >= SM_CUSTOM_MAGICMOVE001 && ident < SM_CUSTOM_MAGICMOVE001 + CustomMagicCount))
                {
                    if (actor != null)
                    {
                        if (ident != TActorCore.SM_BACKSTEP)
                        {
                            actor.m_nChrLight = (byte)HiByte(cdir);
                            // ★ 集成方补回原文守卫（台账 §64.8 / D-P17-09）：原文 PlayScn.pas:7852-7853 /
                            //   8019-8020 / 8023-8024 **三个写点都带 `if Actor is TCustomActor`**：
                            //     if Actor is TCustomActor then TCustomActor(Actor).m_nOldChrLight := Actor.m_nChrLight;
                            //   托管侧此前把它写成**无条件直写**（守卫被丢）⇒ 普通角色也被写。
                            //   当前不可观测（普通角色那份存储原文从不读），但它让"基类那份存储"看起来必需，
                            //   从而掩盖了 H-1（派生真身恒 0）。补回后该存储即可无损退役。
                            if (actor is TCustomActor customActor)
                                customActor.m_nOldChrLight = actor.m_nChrLight;
                            cdir = LoByte(cdir);
                        }
                        else
                        {
                            if (actor is TCustomActor customActor2)
                                customActor2.m_nOldChrLight = actor.m_nChrLight;
                        }

                        if (ident == TActorCore.SM_SKELETON)
                        {
                            actor.m_boDeath = true;
                            actor.m_dwDeathTick = SceneTime.TickNow();
                            actor.m_boSkeleton = true;
                            actor.m_boStruckShowNumber = false;
                            actor.m_boShowBigHPProgress = false;
                            actor.m_boSendQueryBigHPProgress = false;
                            if (ReferenceEquals(actor, G.MySelf))
                                boCanDrawTileMap = true;
                        }

                        if (ident == TActorCore.SM_RUN || ident == TActorCore.SM_WALK || ident == TActorCore.SM_TURN)
                        {
                            // 近身开盾 piaoyun 2013-09-10
                            TryNearStruckShield(x, y);
                            // 好友/敌人近身提示 piaoyun 2013-09-11
                            UpdateFriendHitList(actor, x, y);
                        }
                    }
                }

                if (actor == null)
                    return null;

                switch (ident)
                {
                    case SM_FEATURECHANGED:
                    {
                        if (feature == null)
                            break;

                        actor.m_Feature = feature;
                        // 开启内挂的怪物简装（原文对 Actor.m_Feature 再做一次替换）
                        ApplySimpleShowSubstitution(feature);
                        actor.FeatureChanged();
                        break;
                    }

                    case SM_CHARSTATUSCHANGED:
                    {
                        actor.m_nState = (int)state;

                        if (ReferenceEquals(actor, G.MySelf))
                        {
                            if ((G.MySelf.m_nState & 0x00080000) != 0 || (G.MySelf.m_nState & 0x04000000) != 0)
                            {
                                G.MySelf.CancelAction();
                                G.MySelf.m_boWarMode = false;
                                G.MySelf.CalcActorFrame();
                                G.MySelf.OnActionChanged?.Invoke();
                            }
                        }
                        break;
                    }

                    default:
                    {
                        if (ident == TActorCore.SM_TURN && str != "")
                            actor.m_sUserName = str;

                        actor.SendMsg(new TChrMsg
                        {
                            Ident = ident,
                            X = x,
                            Y = y,
                            Dir = cdir,
                            State = state,
                            Saying = "",
                            Sound = 0,
                        });
                        break;
                    }
                }

                break;
            }
        }

        return actor;
    }

    /// <summary>近身开盾（SendMsg 8042-8142）：≤2 格 + 600ms 节流 → 按职业挑盾（87/88、31、73/89）交 FrmMain.UseMagic。</summary>
    private void TryNearStruckShield(int x, int y)
    {
        if (G.MySelf == null)
            return;
        if (Math.Abs(G.MySelf.m_nCurrX - x) > 2 || Math.Abs(G.MySelf.m_nCurrY - y) > 2)
            return;
        if (G.MySelf.m_boDeath)
            return;
        if (!(clientConfig_boHumStruckShield && ckHumStruckShield))
            return;
        if (SceneTime.TickNow() - g_dwHumStruckShieldTick <= 600)
            return;

        g_dwHumStruckShieldTick = SceneTime.TickNow();

        // 战士 87（武力盾）→ 88（新武力盾）；法师 31（魔法盾）；道士 73（道力盾）→ 89（新道力盾）
        (int primary, int fallback, int primaryMask, int fallbackMask) = G.MySelf.m_btJob switch
        {
            0 => (87, 88, 0x00100000, 0x00040000),
            1 => (31, 0, 0x00100000, 0),
            2 => (73, 89, 0x00100000, 0x00020000),
            _ => (0, 0, 0, 0),
        };
        if (primary == 0)
            return;

        var magic = FindMagicById(primary);
        int mask = primaryMask;
        if (magic == null && fallback != 0)
        {
            magic = FindMagicById(fallback);
            mask = fallbackMask;
        }
        if (magic == null)
            return;

        if ((G.MySelf.m_nState & mask) != 0)
            return;

        if (magic.Value.Spell + magic.Value.DefSpell <= G.MySelf.m_nAbilMP)
            FrmMain?.UseMagic(g_nMouseX, g_nMouseY, magic.Value);
    }

    /// <summary>好友/敌人近身提示（SendMsg 8144-8185）：≤4 格记录 race 0 角色名，出圈移除。</summary>
    private void UpdateFriendHitList(TActorCore actor, int x, int y)
    {
        if (G.MySelf == null)
            return;

        if (Math.Abs(G.MySelf.m_nCurrX - x) <= 4 && Math.Abs(G.MySelf.m_nCurrY - y) <= 4)
        {
            if (actor.m_btRace == 0)
            {
                int dir = MapPath.GetNextDirection(G.MySelf.m_nCurrX, G.MySelf.m_nCurrY,
                    actor.m_nCurrX, actor.m_nCurrY);

                // 好友近身提示 piaoyun 2013-09-11
                if (ckFriendHit && (FrmMain?.IsInFriendMemo(actor.m_sUserName) ?? false))
                {
                    if (!G.MySelf.m_FriendHitList.Contains(actor.m_sUserName))
                    {
                        FrmMain?.AddChatBoardString(
                            $"你的好友[{actor.m_sUserName}]出现在坐标({actor.m_nCurrX}:{actor.m_nCurrY})，方向 {DirsStr[dir]}");
                    }
                }

                // 黑名单近身提示 piaoyun 2013-09-11
                if (ckBlacklistHit && (FrmMain?.IsInBlacklistMemo(actor.m_sUserName) ?? false))
                {
                    if (!G.MySelf.m_FriendHitList.Contains(actor.m_sUserName))
                    {
                        FrmMain?.AddChatBoardString(
                            $"你的敌人[{actor.m_sUserName}]出现在坐标({actor.m_nCurrX}:{actor.m_nCurrY})，方向 {DirsStr[dir]}");
                    }
                }

                if (!G.MySelf.m_FriendHitList.Contains(actor.m_sUserName))
                    G.MySelf.m_FriendHitList.Add(actor.m_sUserName);
            }
        }
        else
        {
            if (actor.m_btRace == 0)
            {
                int idx = G.MySelf.m_FriendHitList.IndexOf(actor.m_sUserName);
                if (idx != -1)
                    G.MySelf.m_FriendHitList.RemoveAt(idx);
            }
        }
    }

    /// <summary>
    /// 简装替换（SM_FEATURECHANGED 8197-8239 与 NewActor 7129-7170 同源逻辑）：
    /// 守卫族豁免（RaceImg ∈ {0,1,50}）时不换装；否则按 HumBBType 分流走稻草人 18/27/83 或自定义配置。
    /// </summary>
    public void ApplySimpleShowSubstitution(TFeature feature)
    {
        var mon = feature.MonFeature();
        if (mon == null)
            return;

        if (IsGuardExemptRace(mon.btRace, mon.wRaceImg))
            return;

        if (mon.HumBBType == THumBBType.bbNo)
        {
            if (mon.IsDisableSimpleActor)
                return;
            if (!(G.boSimpleShowActor && G.ckSimpleShowActor))
                return;

            if (!G.boCustomActorSimpleShow)
            {
                mon.wRaceImg = 18;
                mon.wAppr = 27;
                mon.btRace = 83;
            }
            else
            {
                mon.wRaceImg = (ushort)G.nSimpleActorRaceImg;
                mon.wAppr = (ushort)G.nSimpleActorAppr;
                mon.btRace = (byte)G.nSimpleActorRace;
            }
        }
        else
        {
            if (!(G.boSimpleShowBB && G.ckSimpleShowBB))
                return;

            if (!G.boCustomBBSimpleShow)
            {
                mon.wRaceImg = 18;
                mon.wAppr = 27;
                mon.btRace = 83;
            }
            else
            {
                mon.wRaceImg = (ushort)G.nSimpleBBSimpleRaceImg;
                mon.wAppr = (ushort)G.nSimpleBBAppr;
                mon.btRace = (byte)G.nSimpleBBRace;
            }
        }
    }

    /// <summary>DirsStr（八方向名；SendMsg 近身提示格式化用）。</summary>
    public static readonly string[] DirsStr =
    {
        "上", "右上", "右", "右下", "下", "左下", "左", "左上",
    };

    /// <summary>HiByte / LoByte（HUtil32 1:1）。</summary>
    public static int HiByte(int value) => (value >> 8) & 0xFF;
    public static int LoByte(int value) => value & 0xFF;

    // ---- SendMsg 依赖的全局与接缝 ----
    public string g_sMapName = "";
    public int g_nDarkLevel;
    public int g_nDarkValue;
    public bool boViewFog;
    public bool boViewMiniMap;
    public int g_nMiniMapIndex = -1;
    public bool clientConfig_boUseFindPath;
    public bool clientConfig_boHumStruckShield;
    public bool ckHumStruckShield;
    public bool ckFriendHit;
    public bool ckBlacklistHit;
    public uint g_dwHumStruckShieldTick;
    public int g_nMouseX;
    public int g_nMouseY;

    /// <summary>SM_CUSTOM_MAGICMOVE001（Grobal2.pas 2515）。</summary>
    public const int SM_CUSTOM_MAGICMOVE001 = 11500;

    /// <summary>CUSTOM_MAGIC_COUNT（Grobal2.pas 60）。</summary>
    public const int CustomMagicCount = 300;

    // Grobal2.pas 客户端动作码（SendMsg 分派用）
    public const int SM_TEST = 65037;
    public const int SM_CHANGEMAP = 634;
    public const int SM_NEWMAP = 51;
    public const int SM_HIDE = 29;
    public const int SM_DISAPPEARMYHERO = 8911;
    public const int SM_FEATURECHANGED = 41;
    public const int SM_CHARSTATUSCHANGED = 657;

    /// <summary>按 wMagicId 查技能（g_MagicList 接缝：返回 (MagicId, wSpell, wDefSpell)）。</summary>
    public Func<int, (int MagicId, int Spell, int DefSpell)?>? MagicLookupFn;

    private (int MagicId, int Spell, int DefSpell)? FindMagicById(int magicId) => MagicLookupFn?.Invoke(magicId);
}

/// <summary>frmMain 接缝（SendMsg/CleanObjects 依赖的主窗体调用）。</summary>
public interface FrmMainSeam
{
    void ClearDropItems();
    void SendWantMiniMap(bool big);
    void AddChatBoardString(string text);
    void ClearWaitingHeroUseItem();
    void ClearHeroEatingItem();
    void UseMagic(int x, int y, (int MagicId, int Spell, int DefSpell) magic);
    bool IsInFriendMemo(string name);
    bool IsInBlacklistMemo(string name);
    void CloseDHeroStateDlg();
    void CloseDHeroItemBagDlg();
    void CloseDHeroStateWinDlg();
}
