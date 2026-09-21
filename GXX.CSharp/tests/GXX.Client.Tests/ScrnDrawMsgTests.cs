using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Client.Scenes;
using Xunit;
using TRect = GXX.Client.GUI.DxComponent.TRect;
// 同 ScrnDrawHintTests：GUI.Share 的接缝与本车道正式归属同名 → 必须用别名消除 CS0104。
using THintWindows = GXX.Client.Scenes.THintWindows;

namespace GXX.Client.Tests;

/// <summary>
/// DrawScrn.pas 消息族（TDrawSysMsg / TDrawSysMsgEx / TDrawMoveHintMsg / TDrawScreenCenterMsg /
/// TDrawDelayMsg / TDrawScreenMoveMsg / TScreenMoveMsgList / TDrawScreenCenterNewlineMsg /
/// TDrawScreenNewMoveMsg / TScreenNewMoveMsgList / TMoveHintMsgList / TScreenNewLineMsgList /
/// TDrawScreen）的边界测试：**列表容量与淘汰顺序**、**tick 阈值**、**坐标/裁剪公式**。
/// </summary>
public sealed class ScrnDrawMsgTests : ScrnDrawTestBase
{
    private uint _now;

    public ScrnDrawMsgTests()
    {
        _now = 0;
        DrawScrnEnv.MyGetTickCountFn = () => _now;
    }

    // 场景构造器（Scenes.cs / PlaySceneCore.cs 的既有签名）
    private static TPlayScene NewPlayScene() => new TPlayScene(new SceneDialogs());
    private static TLoginScene NewLoginScene() => new TLoginScene(new SceneDialogs(), _ => { });
    private static TSelectChrScene NewSelectChrScene() => new TSelectChrScene(new SceneDialogs());

    // =========================================================================================
    // TDrawSysMsg（627-640 / 2694-2774）
    // =========================================================================================

    /// <summary>Create（2694-2704）的默认值：X=30 / Y=40 / FColor=GetRGB(2) / BColor=GetRGB(0)。</summary>
    [Fact]
    public void TDrawSysMsg_Create_Defaults()
    {
        var m = new TDrawSysMsg();
        Assert.Equal(30, m.m_nX);
        Assert.Equal(40, m.m_nY);
        Assert.Equal(DrawScrnEnv.GetRGB(2), m.m_FColor.Value);
        Assert.Equal(DrawScrnEnv.GetRGB(0), m.m_BColor.Value);
        Assert.False(m.m_boDownToUP);
        Assert.False(m.m_boDrawBottom);
    }

    /// <summary>Add（2716-2734）：容量 10，第 11 条淘汰**最旧**的一条（`Count &gt;= 10` 时 Delete(0)）。</summary>
    [Fact]
    public void TDrawSysMsg_Add_CapsAtTenAndEvictsOldest()
    {
        var m = new TDrawSysMsg();
        for (int i = 0; i < 12; i++)
            m.Add("M" + i, 2, 0);

        Assert.Equal(10, m.m_MsgList.Count);
        Assert.Equal("M2", m.m_MsgList[0]);   // M0/M1 被淘汰
        Assert.Equal("M11", m.m_MsgList[9]);
    }

    /// <summary>Draw（2736-2764）：DownToUP=False 时逐行 +16；=True 时逐行 -16。</summary>
    [Theory]
    [InlineData(false, new[] { 207, 223 })]
    [InlineData(true, new[] { 207, 191 })]
    public void TDrawSysMsg_Draw_LineStepDirection(bool downToUp, int[] expectedY)
    {
        var m = new TDrawSysMsg { m_nX = 7, m_nY = 207, m_boDownToUP = downToUp };
        DrawScrnEnv.g_MySelf = new TActor();
        m.Add("A", 2, 0);
        m.Add("B", 2, 0);

        DrawScrnEnv.GameCanvas.Clear();
        m.Draw();

        Assert.Equal(2, DrawScrnEnv.GameCanvas.Ops.Count);
        Assert.Equal(expectedY[0], DrawScrnEnv.GameCanvas.Ops[0].Y);
        Assert.Equal(expectedY[1], DrawScrnEnv.GameCanvas.Ops[1].Y);
        Assert.Equal(7, DrawScrnEnv.GameCanvas.Ops[0].X);
    }

    /// <summary>Draw 在 `g_MySelf = nil` 时直接退出（2741）。</summary>
    [Fact]
    public void TDrawSysMsg_Draw_RequiresMySelf()
    {
        DrawScrnEnv.g_MySelf = null;
        var m = new TDrawSysMsg();
        m.Add("A", 2, 0);
        DrawScrnEnv.GameCanvas.Clear();
        m.Draw();
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);
    }

    /// <summary>Draw 的淘汰阈值是 **3000ms**（2756），且只淘汰队首一条。</summary>
    [Fact]
    public void TDrawSysMsg_Draw_EvictsHeadAfter3000()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        var m = new TDrawSysMsg();
        m.Add("A", 2, 0);

        _now = 2999;
        m.Draw();
        Assert.Equal(1, m.m_MsgList.Count);

        _now = 3000;
        m.Draw();
        Assert.Equal(0, m.m_MsgList.Count);
    }

    /// <summary>Clear（2766-2774）**不 dispose 元素**、只清表。</summary>
    [Fact]
    public void TDrawSysMsg_Clear_EmptiesList()
    {
        var m = new TDrawSysMsg();
        m.Add("A", 2, 0);
        m.Clear();
        Assert.Equal(0, m.m_MsgList.Count);
    }

    // =========================================================================================
    // TDrawSysMsgEx（642-659 / 2778-3036）
    // =========================================================================================

    /// <summary>Create（2778-2791）：两个 tick 都被初始化成当前 tick。</summary>
    [Fact]
    public void TDrawSysMsgEx_Create_InitializesTicks()
    {
        _now = 1234;
        var m = new TDrawSysMsgEx();
        Assert.Equal(1234u, m.m_boWantDeleteTick);
        Assert.Equal(1234u, m.m_dwOffsetTick);
        Assert.Equal(0, m.m_nOffsetY);
        Assert.False(m.m_boWantDelete);
    }

    /// <summary>
    /// Draw（2916-3026）的三档参数：Count&gt;8 → (700,20)；Count&gt;5 → (1500,30)；否则 (2000,50)。
    /// 这里用 `Count &gt; 5` 一档验证「每 nStepTime 毫秒偏移 +1」。
    /// </summary>
    [Fact]
    public void TDrawSysMsgEx_Draw_CountGreaterThanFive_StepsEvery30ms()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        var m = new TDrawSysMsgEx();
        for (int i = 0; i < 6; i++) m.Add("M" + i, 2, 0);

        // 第一次 Draw：置 m_boWantDelete（因为 Count>5）并把 offset/tick 归零
        _now = 0;
        m.Draw();
        Assert.True(m.m_boWantDelete);
        Assert.Equal(0, m.m_nOffsetY);

        _now = 29;
        m.Draw();
        Assert.Equal(0, m.m_nOffsetY);     // 29 < 30

        _now = 30;
        m.Draw();
        Assert.Equal(1, m.m_nOffsetY);

        _now = 60;
        m.Draw();
        Assert.Equal(2, m.m_nOffsetY);
    }

    /// <summary>OffsetY 达到 16 时删除队首并把状态全部复位（2954-2973）。</summary>
    [Fact]
    public void TDrawSysMsgEx_Draw_OffsetReaching16_DeletesHeadAndResets()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        var m = new TDrawSysMsgEx();
        for (int i = 0; i < 6; i++) m.Add("M" + i, 2, 0);

        _now = 0;
        m.Draw();                     // 置 flag

        // 30ms 一步，推到 16
        for (int step = 1; step <= 16; step++)
        {
            _now = (uint)(step * 30);
            m.Draw();
        }
        Assert.Equal(16, m.m_nOffsetY);
        Assert.Equal(6, m.m_MsgList.Count);

        _now = 16 * 30 + 1;
        m.Draw();
        Assert.Equal(5, m.m_MsgList.Count);   // 删掉队首
        Assert.Equal(0, m.m_nOffsetY);
        Assert.False(m.m_boWantDelete);       // 剩下 5 条 ≤ 5 → 不再要求删除
    }

    /// <summary>Alpha（3005-3010）：队首且 OffsetY&gt;0 时 `180 - OffsetY*15`，负值夹到 0。</summary>
    [Fact]
    public void TDrawSysMsgEx_Draw_HeadAlphaFormula()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        var m = new TDrawSysMsgEx();
        for (int i = 0; i < 9; i++) m.Add("M" + i, 2, 0);

        // Count>8 → 700/20
        _now = 0;
        m.Draw();

        _now = 20;
        DrawScrnEnv.GameCanvas.Clear();
        m.Draw();                       // offset = 1
        Assert.Equal(1, m.m_nOffsetY);
        Assert.Equal(180 - 15, DrawScrnEnv.GameCanvas.Ops[0].Alpha);

        // 把 offset 推到 13 → 180-195 = -15 → 夹到 0
        for (int step = 2; step <= 13; step++)
        {
            _now = (uint)(step * 20);
            m.Draw();
        }
        Assert.Equal(13, m.m_nOffsetY);
        DrawScrnEnv.GameCanvas.Clear();
        m.Draw();
        Assert.Equal(0, DrawScrnEnv.GameCanvas.Ops[0].Alpha);
    }

    /// <summary>Draw 的每次循环最多画 6 行（3019-3020 的 `nShowCount &gt; 5 then Break`）。</summary>
    [Fact]
    public void TDrawSysMsgEx_Draw_ShowsAtMostSixLines()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        var m = new TDrawSysMsgEx();
        for (int i = 0; i < 10; i++) m.Add("M" + i, 2, 0);

        _now = 0;
        DrawScrnEnv.GameCanvas.Clear();
        m.Draw();

        Assert.Equal(6, DrawScrnEnv.GameCanvas.Ops.Count);
    }

    // =========================================================================================
    // TDrawMoveHintMsg（661-672 / 3040-3113）
    // =========================================================================================

    /// <summary>与 TDrawSysMsg 的差异：**没有 downToUP 分支**，恒 +16（3093）。</summary>
    [Fact]
    public void TDrawMoveHintMsg_Draw_AlwaysDownward()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        var m = new TDrawMoveHintMsg { m_nX = 1, m_nY = 100 };
        m.Add("A", 2, 0);
        m.Add("B", 2, 0);

        DrawScrnEnv.GameCanvas.Clear();
        m.Draw();

        Assert.Equal(100, DrawScrnEnv.GameCanvas.Ops[0].Y);
        Assert.Equal(116, DrawScrnEnv.GameCanvas.Ops[1].Y);
    }

    // =========================================================================================
    // TDrawScreenCenterMsg（430-442 / 3116-3264）
    // =========================================================================================

    /// <summary>Add（3149-3217）：把消息按 `SCREENWIDTH-20` 折行，并设定 `m_dwShowTime = now + nTime*1000`。</summary>
    [Fact]
    public void TDrawScreenCenterMsg_Add_WrapsAtScreenWidthMinus20()
    {
        _now = 1000;
        var c = new TDrawScreenCenterMsg();
        // 每字 6px，MaxWidth = 1024-20 = 1004 → 最多 167 字
        c.Add(new string('A', 200), 255, 0, 5);

        Assert.Equal(6000u, c.m_dwShowTime);                 // now(1000) + 5*1000
        Assert.True(c.m_TextList.Count >= 2);

        // 所有行拼起来等于原串（token 文本无损）
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < c.m_TextList.Count; i++)
            sb.Append(DrawScrnText.GetStrinLineExText((TStringLineEx)c.m_TextList.GetObject(i)));
        Assert.Equal(new string('A', 200), sb.ToString());
    }

    /// <summary>Draw（3219-3264）：超时后 `Clear(False)`；未超时时按 `(SCREENHEIGHT - 行数*行高)/2` 居中。</summary>
    [Fact]
    public void TDrawScreenCenterMsg_Draw_CentersAndExpires()
    {
        _now = 0;
        var c = new TDrawScreenCenterMsg();
        c.Add("AB", 255, 0, 5);      // 1 行
        Assert.Equal(1, c.m_TextList.Count);

        DrawScrnEnv.GameCanvas.Clear();
        _now = 4999;
        c.Draw();
        Assert.Equal(1, c.m_TextList.Count);
        // nY = (768 - 1*12)/2 = 378；nX = (1024 - 12)/2 = 506
        Assert.Equal(378, DrawScrnEnv.GameCanvas.Ops[0].Y);
        Assert.Equal(506, DrawScrnEnv.GameCanvas.Ops[0].X);

        _now = 5000;                 // `MyGetTickCount < m_dwShowTime` 为假 → 清空
        c.Draw();
        Assert.Equal(0, c.m_TextList.Count);
    }

    // =========================================================================================
    // TDrawDelayMsg（524-537 / 3268-3475）
    // =========================================================================================

    /// <summary>Add（3321-3361）：同 RecogId 原地更新（不新增）；新 id 追加并重置跑马灯状态。</summary>
    [Fact]
    public void TDrawDelayMsg_Add_SameRecogIdUpdatesInPlace()
    {
        _now = 0;
        var d = new TDrawDelayMsg();
        var f = new TColor(0x000000FF);
        var b = new TColor(0x00000000);

        d.Add(11, "A", 3, f, b, 5);
        Assert.Equal(1, d.m_MsgList.Count);
        var first = (TDelayMsg)d.m_MsgList[0];
        Assert.Equal(3000u, first.Time);
        Assert.True(d.m_MoveDraw);
        Assert.Equal(0, d.m_MoveOffset);

        // 第二次：m_MoveDraw 被上一次的 Draw 或这里手动置回，观察原地更新
        d.m_MoveDraw = false;
        _now = 100;
        d.Add(11, "B", 7, f, b, 9);

        Assert.Equal(1, d.m_MsgList.Count);
        Assert.Equal("B", first.Msg);
        Assert.Equal(9, first.X);
        Assert.Equal(100u + 7000u, first.Time);
        Assert.False(d.m_MoveDraw);          // 命中已有项 → **不**重置跑马灯（原文如此）
    }

    /// <summary>Delete（3301-3319）：按 RecogId 全删（倒序循环，可删多条）。</summary>
    [Fact]
    public void TDrawDelayMsg_Delete_RemovesAllMatching()
    {
        var d = new TDrawDelayMsg();
        var f = new TColor(0);
        var b = new TColor(0);
        d.Add(1, "A", 1, f, b, 0);
        d.Add(2, "B", 1, f, b, 0);
        d.Add(1, "C", 1, f, b, 0);

        d.Delete(1);
        Assert.Equal(1, d.m_MsgList.Count);
        Assert.Equal(2L, ((TDelayMsg)d.m_MsgList[0]).RecogId);
    }

    /// <summary>Draw（3363-3475）：`MyGetTickCount &gt; Time` 的项被淘汰；`Count &lt;= 5` 时关掉跑马灯。</summary>
    [Fact]
    public void TDrawDelayMsg_Draw_EvictsExpiredAndStopsMarquee()
    {
        _now = 0;
        var d = new TDrawDelayMsg();
        var f = new TColor(0);
        var b = new TColor(0);
        d.Add(1, "A", 1, f, b, 0);
        d.m_MoveDraw = true;

        _now = 1000;
        d.Draw();
        Assert.Equal(1, d.m_MsgList.Count);
        Assert.False(d.m_MoveDraw);          // Count(1) <= 5 → 关

        _now = 1001;                          // > Time(1000)
        d.Draw();
        Assert.Equal(0, d.m_MsgList.Count);
    }

    /// <summary>
    /// Draw 的 `%d` / `%s` 替换成剩余秒数（3434-3437），且 `X &lt;= 0` 时按屏宽居中并兜底 1（3440-3443）。
    /// </summary>
    [Fact]
    public void TDrawDelayMsg_Draw_ReplacesPlaceholders_AndCentersX()
    {
        _now = 0;
        var d = new TDrawDelayMsg();
        var f = new TColor(0);
        var b = new TColor(0);
        d.Add(1, "%d秒后", 3, f, b, 0);      // Time = 3000

        _now = 1000;                          // 剩余 2000ms → "2秒"
        DrawScrnEnv.GameCanvas.Clear();
        d.Draw();

        var texts = new List<string>();
        foreach (var op in DrawScrnEnv.GameCanvas.Ops)
            if (op.Kind == ScrnDrawKind.Text) texts.Add(op.Text);
        Assert.Contains("2秒", string.Concat(texts));

        // X 由 `(SCREENWIDTH - 文本宽) / 2` 得到（4 字 × 6 = 24 → 500）
        var msg = (TDelayMsg)d.m_MsgList[0];
        Assert.Equal((1024 - 24) / 2, msg.X);

        // `X <= 0` 时的兜底 1（3442）：超宽文本 → (1024 - 1200)/2 = -88 → 1
        var d2 = new TDrawDelayMsg();
        d2.Add(2, new string('A', 200), 3, f, b, 0);
        _now = 1000;
        d2.Draw();
        Assert.Equal(1, ((TDelayMsg)d2.m_MsgList[0]).X);
    }

    // =========================================================================================
    // TDrawScreenMoveMsg / TScreenMoveMsgList（539-578 / 3479-4000）
    // =========================================================================================

    /// <summary>
    /// Update（3706-3815）：拿到队首后按 Orientation 铺初始几何。
    /// 水平：FOffSetX = SCREENWIDTH-80、DestRect = (80, Y, 80+SCREENWIDTH-160, Y+BH)、SrcRect 宽 0。
    /// </summary>
    [Fact]
    public void TDrawScreenMoveMsg_Update_HorizontalInitialGeometry()
    {
        _now = 0;
        var mv = new TDrawScreenMoveMsg();
        mv.Add("AB", 255, 0, 300, 1, true, 190, 11, true, 60);

        mv.Update();

        Assert.NotNull(mv.m_nCurrMoveMsg);
        Assert.Equal(1024 - 80, mv.FOffSetX);
        Assert.Equal(300, mv.DestRect.Top);
        Assert.Equal(80, mv.DestRect.Left);
        Assert.Equal(80 + 1024 - 160, mv.DestRect.Right);
        Assert.Equal(12, mv.FTextSize);                 // 水平方向 = 文本宽（"AB" → 12）
        Assert.Equal(0, mv.SrcRect.Right);              // 水平方向的初始 SrcRect 宽为 0
        Assert.Equal(mv.m_nCurrMoveMsg.Height + mv.m_nCurrMoveMsg.Height / 4, mv.DestRect.Bottom - mv.DestRect.Top);
    }

    /// <summary>
    /// 每 `nMarqueeTime` 毫秒 FMoveSize += 2；未滚满一个 DestRect 宽时 SrcRect = (0,0,Min(文宽, FMoveSize), h)。
    /// </summary>
    [Fact]
    public void TDrawScreenMoveMsg_Update_ClipRectWhileScrolling()
    {
        _now = 0;
        var mv = new TDrawScreenMoveMsg();
        mv.Add("ABCDEF", 255, 0, 300, 1, true, 190, 11, true, 60);   // 文本宽 36
        mv.Update();                                                 // now=0 → 0 > 60 假

        _now = 61;
        mv.Update();
        Assert.Equal(2, mv.FMoveSize);
        Assert.Equal(0, mv.SrcRect.Left);
        Assert.Equal(DrawScrnRect.Min(mv.m_nCurrMoveMsg.Width, 2), mv.SrcRect.Right);
        Assert.Equal(1024 - 80 - 2, mv.FOffSetX);                    // 每步 -2（未到 80）
    }

    /// <summary>Count 递减到 0 且当前条用尽后，`MoveOver` 为真（3530-3535 / 3943-3948）。</summary>
    [Fact]
    public void TDrawScreenMoveMsg_MoveOver_AfterCountExhausted()
    {
        _now = 0;
        var mv = new TDrawScreenMoveMsg();
        mv.Add("AB", 255, 0, 300, 1, true, 190, 11, true, 10);
        Assert.False(mv.MoveOver());          // 列表里还有 1 条

        mv.Update();
        Assert.NotNull(mv.m_nCurrMoveMsg);
        Assert.False(mv.MoveOver());          // 当前条还在

        mv.m_nCurrMoveMsg.Count = 0;
        mv.Update();                          // 回收当前条
        Assert.True(mv.MoveOver());
    }

    /// <summary>TScreenMoveMsgList.Add 按 `Top`（= DestRect.Top）分组复用（3893-3920）。</summary>
    [Fact]
    public void TScreenMoveMsgList_Add_GroupsByTop()
    {
        _now = 0;
        var list = new TScreenMoveMsgList();
        list.Add("A", 255, 0, 100, 1);
        list.Add("B", 255, 0, 100, 1);
        Assert.Equal(1, list.m_MoveObjList.Count);

        list.Add("C", 255, 0, 200, 1);
        Assert.Equal(2, list.m_MoveObjList.Count);

        var first = (TDrawScreenMoveMsg)list.m_MoveObjList[0];
        Assert.Equal(100, first.Top);
        Assert.Equal(2, first.Count);
    }

    // =========================================================================================
    // TMoveHintMsgList（603-613 / 4102-4272）
    // =========================================================================================

    /// <summary>
    /// Add 的坐标换算（4123-4163）：nX=0 → `g_nMoveMouseX`（NPC 对话框打开时再减 VirtualRect.Left）；
    /// nY=0 → `g_nMoveMouseY - g_CurrentFontHeight - 5`；nY≠0 → `nY + 60`。
    /// </summary>
    [Fact]
    public void TMoveHintMsgList_Add_CoordinateFallbacks()
    {
        DrawScrnEnv.g_nMoveMouseX = 100;
        DrawScrnEnv.g_nMoveMouseY = 200;
        DrawScrnEnv.g_CurrentFontHeight = 14;
        DrawScrnEnv.DMerchantDlgVisibleFn = () => false;

        var l = new TMoveHintMsgList();
        l.Add("A", 2, 0, 0, 0);
        l.Add("B", 2, 0, 5, 7);

        var a = (TMoveHintMsgRecord)l.FItemList[0];
        Assert.Equal(100, a.nX);
        Assert.Equal(200 - 14 - 5, a.nY);
        Assert.Equal(0, a.nYOffset);
        Assert.Equal(_now, a.LastUpdateTick);

        var b = (TMoveHintMsgRecord)l.FItemList[1];
        Assert.Equal(5, b.nX);
        Assert.Equal(67, b.nY);

        // NPC 对话框打开 → 再减 VirtualRect
        DrawScrnEnv.DMerchantDlgVisibleFn = () => true;
        DrawScrnEnv.DMerchantDlgVirtualRectFn = () => DrawScrnRect.Rect(10, 20, 400, 500);
        l.Add("C", 2, 0, 0, 0);
        var c = (TMoveHintMsgRecord)l.FItemList[2];
        Assert.Equal(100 - 10, c.nX);
        Assert.Equal(200 - 14 - 5 - 20, c.nY);
    }

    /// <summary>
    /// Update（4231-4255）：`nYOffset &lt; 40` 时每 ≥10ms +2；`nYOffset &gt;= 40` 后还要再等
    /// **1500ms** 才删除。
    /// </summary>
    [Fact]
    public void TMoveHintMsgList_Update_SlowRiseThenHoldThenRemove()
    {
        var l = new TMoveHintMsgList();
        l.Add("A", 2, 0, 1, 1);

        _now = 9;
        l.Update();
        Assert.Equal(0, ((TMoveHintMsgRecord)l.FItemList[0]).nYOffset);

        _now = 10;
        l.Update();
        Assert.Equal(2, ((TMoveHintMsgRecord)l.FItemList[0]).nYOffset);

        // 推到 40（20 步）
        for (int step = 2; step <= 20; step++)
        {
            _now = (uint)(step * 10);
            l.Update();
        }
        Assert.Equal(40, ((TMoveHintMsgRecord)l.FItemList[0]).nYOffset);
        Assert.Equal(1, l.FItemList.Count);

        _now = 200 + 1499;
        l.Update();
        Assert.Equal(1, l.FItemList.Count);

        _now = 200 + 1500;
        l.Update();
        Assert.Equal(0, l.FItemList.Count);
    }

    /// <summary>Clear（4257-4272）清表；Free（4107-4121）的循环体在原文里**是空的**（内存泄漏，照抄）。</summary>
    [Fact]
    public void TMoveHintMsgList_Clear_And_FreeIsNoOpLoop()
    {
        var l = new TMoveHintMsgList();
        l.Add("A", 2, 0, 1, 1);
        l.Add("B", 2, 0, 2, 2);
        l.Clear();
        Assert.Equal(0, l.FItemList.Count);

        l.Add("C", 2, 0, 3, 3);
        l.Free();
        Assert.Null(l.FItemList);
    }

    // =========================================================================================
    // TDrawScreenCenterNewlineMsg（456-482 / 4903-5264）
    // =========================================================================================

    /// <summary>`m_nDrawType div 100 = 0 and not boDrawBack` 时直接退出（5051）。</summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_Draw_BackGate()
    {
        var c = new TDrawScreenCenterNewlineMsg();
        c.Add("AB", 255, 0, 20, 3, 100, 5, 0);   // nDrawType = 0 → 100 位为 0

        DrawScrnEnv.GameCanvas.Clear();
        c.Draw(false);
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);   // 直接 Exit
    }

    /// <summary>
    /// `nDrawType = 100`（= 0 + 100）时**无条件**绘制，且带透明矩形框
    /// `Rect(80, m_nY, SCREENWIDTH-80, m_nY + 行数*(行高+2) + 4)`（5103-5105）。
    /// </summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_Draw_Type100_DrawsTransparentBackRect()
    {
        _now = 0;
        var c = new TDrawScreenCenterNewlineMsg();
        c.Add("AB", 255, 0, 20, 3, 100, 5, 100);

        DrawScrnEnv.GameCanvas.Clear();
        c.Draw(false);

        var back = DrawScrnEnv.GameCanvas.Ops[0];
        Assert.Equal(ScrnDrawKind.FillRectAlpha, back.Kind);
        Assert.Equal(DrawScrnRect.Rect(80, 100, 1024 - 80, 100 + 1 * (12 + 2) + 4), back.Rect);
        Assert.Equal(100, back.Alpha);
        Assert.Equal(DrawScrnEnv.GetRGB(190), back.FColor.Value);
    }

    /// <summary>`nDrawType mod 100 = 1` → 走 DrawEx 淡入淡出（5055-5059）。</summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_Draw_Type101_UsesDrawEx()
    {
        _now = 0;
        var c = new TDrawScreenCenterNewlineMsg();
        c.Add("AB", 255, 0, 20, 0, 100, 5, 101);

        DrawScrnEnv.GameCanvas.Clear();
        c.Draw(false);

        // DrawEx 首帧：m_nState=0 且 m_nCurY=0 → m_nCurY = m_nY + nY*1 = 100 + 14
        Assert.Equal(114, c.m_nCurY);
        Assert.Equal(0, c.m_nState);
        // 首帧 alpha = 255 - Abs(Round(255)) = 0，且 m_nState != 1 → 走 BoldTextOutEx
        var op = Assert.Single(DrawScrnEnv.GameCanvas.Ops);
        Assert.Equal(ScrnDrawKind.Text, op.Kind);
        Assert.Equal(114, op.Y);
        Assert.Equal(0, op.Alpha);
    }

    /// <summary>停留超时后没有缓存 → `m_boShowOver = True`（5078-5089）。</summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_Draw_ExpiresWithoutCache()
    {
        _now = 0;
        var c = new TDrawScreenCenterNewlineMsg();
        c.Add("AB", 255, 0, 20, 0, 100, 5, 100);   // nStandTime = 5000

        c.Draw(false);                              // m_nState 0→1，m_dwStartStandTick = 0
        Assert.Equal(1, c.m_nState);

        _now = 5000;
        c.Draw(false);
        Assert.Equal(0, c.m_ShowLines.Count);
        Assert.True(c.m_boShowOver);
    }

    /// <summary>ClearTimeCache（4903-4915）：`tick_diff(nAddTick, now) &gt;= nTime + 8000` 才丢掉缓存项。</summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_ClearTimeCache_EvictsAtNTimePlus8000()
    {
        var c = new TDrawScreenCenterNewlineMsg();
        c.FCacheList.Add(new TDrawScreenNewMsgCacheText { nAddTick = 0, nTime = 100 });
        c.FCacheList.Add(new TDrawScreenNewMsgCacheText { nAddTick = 0, nTime = 200 });

        _now = 8099;
        c.ClearTimeCache();
        Assert.Equal(2, c.FCacheList.Count);

        _now = 8100;
        c.ClearTimeCache();
        Assert.Equal(1, c.FCacheList.Count);        // nTime=100 的被丢
        Assert.Equal(200, ((TDrawScreenNewMsgCacheText)c.FCacheList[0]).nTime);
    }

    /// <summary>`m_ShowLines.Count &gt; 0` 时新的 Add 只进缓存（4930-4945）。</summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_Add_CachesWhileShowing()
    {
        _now = 0;
        var c = new TDrawScreenCenterNewlineMsg();
        c.Add("A||B", 255, 0, 20, 0, 100, 5, 100);
        Assert.Equal(2, c.m_ShowLines.Count);       // "||" → 换行

        c.Add("C", 255, 0, 20, 0, 100, 5, 100);
        Assert.Equal(2, c.m_ShowLines.Count);
        Assert.Equal(1, c.FCacheList.Count);
        Assert.Equal(0u, ((TDrawScreenNewMsgCacheText)c.FCacheList[0]).nAddTick);
    }

    /// <summary>空串不进任何表（4928）。</summary>
    [Fact]
    public void TDrawScreenCenterNewlineMsg_Add_EmptyIsIgnored()
    {
        var c = new TDrawScreenCenterNewlineMsg();
        c.Add("", 255, 0, 20, 0, 100, 5, 100);
        Assert.Equal(0, c.m_ShowLines.Count);
        Assert.Equal(0, c.FCacheList.Count);
    }

    // =========================================================================================
    // TDrawScreenNewMoveMsg（484-522 / 5268-5535）
    // =========================================================================================

    /// <summary>
    /// Add 的行高/步长公式（5350-5355）：`LineHeight = TextHeight("文字") + 2`、
    /// `Y0/Y1/Y2 = nY ∓ LineHeight`、`StepMove = 2`、`StepAlphaChange = Round(250 / (LineHeight / 2))`。
    /// </summary>
    [Fact]
    public void TDrawScreenNewMoveMsg_Add_ComputesLineGeometry()
    {
        _now = 0;
        var m = new TDrawScreenNewMoveMsg();
        m.Add("A||B", 255, 0, 20, 0, 100, 1);

        Assert.Equal(2, m.m_ShowLines.Count);
        Assert.Equal(12 + 2, m.m_LineHeight);
        Assert.Equal(100 - 14, m.m_Y0);
        Assert.Equal(100, m.m_Y1);
        Assert.Equal(100 + 14, m.m_Y2);
        Assert.Equal(2, m.m_StepMove);
        Assert.Equal((int)Math.Round(250.0 / (14 / 2.0)), m.m_StepAlphaChange);
        Assert.True(m.m_IsStayShow);
        Assert.Equal(1, m.m_CurrentCount);
        Assert.Equal(1, m.m_TotalCount);
    }

    /// <summary>正在显示时新的 Add 进 FCacheList（5324-5336）。</summary>
    [Fact]
    public void TDrawScreenNewMoveMsg_Add_CachesWhileShowing()
    {
        var m = new TDrawScreenNewMoveMsg();
        m.Add("A", 255, 0, 20, 0, 100, 1);
        m.Add("B", 255, 0, 20, 0, 100, 1);

        Assert.Equal(1, m.m_ShowLines.Count);
        Assert.Equal(1, m.FCacheList.Count);
        Assert.Equal("B", ((TDrawScreenNewMsgCacheText)m.FCacheList[0]).sMsg);
    }

    /// <summary>空串 / 字体取不到 → 直接返回（5319 / 5339）。</summary>
    [Fact]
    public void TDrawScreenNewMoveMsg_Add_EmptyOrNoFontReturns()
    {
        var m = new TDrawScreenNewMoveMsg();
        m.Add("", 255, 0, 20, 0, 100, 1);
        Assert.Equal(0, m.m_ShowLines.Count);

        DrawScrnEnv.FindFontFn = (name, size, style) => null;
        var m2 = new TDrawScreenNewMoveMsg();
        m2.Add("A", 255, 0, 20, 0, 100, 1);
        Assert.Equal(0, m2.m_ShowLines.Count);
        Assert.Equal(0, m2.FCacheList.Count);
        Assert.Null(m2.m_HGEFont);
    }

    /// <summary>
    /// Draw（5382-5535）的停留→滚动切换：停留超 2000ms 后进入滚动分支，
    /// `LastTime = now - 1000`（5460，故意提前一秒以让下一帧立刻步进）。
    /// </summary>
    [Fact]
    public void TDrawScreenNewMoveMsg_Draw_StayThenScrollTransition()
    {
        _now = 0;
        var m = new TDrawScreenNewMoveMsg();
        m.Add("A||B", 255, 0, 20, 0, 100, 2);

        m.Draw();
        Assert.True(m.m_IsStayShow);
        Assert.Equal(0, m.m_CurLineIndex);

        _now = 2001;
        m.Draw();
        Assert.False(m.m_IsStayShow);
        Assert.Equal(m.m_Y1, m.m_OffsetY1);
        Assert.Equal(m.m_Y2, m.m_OffsetY2);
        Assert.Equal(250, m.m_Alpha1);
        Assert.Equal(0, m.m_Alpha2);
        Assert.Equal(2001u - 1000u, m.m_LastTime);
    }

    /// <summary>
    /// 滚动一步（5466-5487）：每 ≥100ms 走 `StepMove`、alpha 按 `StepAlphaChange` 变化；
    /// 到 `OffsetY1 &lt;= Y0` 或 `OffsetY2 &lt;= Y1` 时归位并把 CurLineIndex +1。
    /// </summary>
    [Fact]
    public void TDrawScreenNewMoveMsg_Draw_ScrollStepAndLineAdvance()
    {
        _now = 0;
        var m = new TDrawScreenNewMoveMsg();
        m.Add("A||B", 255, 0, 20, 0, 100, 2);

        m.Draw();                 // 停留
        _now = 2001;
        m.Draw();                 // 切到滚动，LastTime = 1001

        _now = 1101;              // 1101 - 1001 = 100 → 步进
        m.Draw();
        Assert.Equal(m.m_Y1 - 2, m.m_OffsetY1);
        Assert.Equal(unchecked((byte)(250 - m.m_StepAlphaChange)), m.m_Alpha1);
        Assert.Equal(m.m_Y2 - 2, m.m_OffsetY2);
        Assert.Equal(unchecked((byte)(0 + m.m_StepAlphaChange)), m.m_Alpha2);

        // 一直走到 OffsetY2 <= Y1（= 100）：从 114 起，每步 -2，7 步后到 100
        for (int step = 0; step < 8; step++)
        {
            _now += 100;
            m.Draw();
        }
        Assert.Equal(m.m_Y0, m.m_OffsetY1);
        Assert.Equal(0, m.m_Alpha1);
        Assert.Equal(m.m_Y1, m.m_OffsetY2);
        Assert.Equal(255, m.m_Alpha2);
        Assert.Equal(1, m.m_CurLineIndex);
        Assert.True(m.m_IsStayShow);
    }

    // =========================================================================================
    // TDrawScreen（674-733 / 4374-4899、5537-5540）
    // =========================================================================================

    /// <summary>Create（4374-4393）：Initialize 前 `m_boInitialize = False`（字段默认值）。</summary>
    [Fact]
    public void TDrawScreenScrn_Create_Defaults()
    {
        var d = new TDrawScreenScrn();
        Assert.Null(d.CurrentScene);
        Assert.False(d.m_boInitialize);
        Assert.False(d.m_boShowLoginSceneShowRandomCodeDlg);
        Assert.NotNull(d.DrawDelayMsg);
        Assert.NotNull(d.DrawScreenCenterMsg);
        Assert.NotNull(d.FScreenMoveMsgList);
        Assert.NotNull(d.FScreenNewMoveMsgList);
        Assert.NotNull(d.FScreenNewLineMsgList);
        Assert.NotNull(d.FMoveHintMsgList);
        Assert.NotNull(d.HintList);
    }

    /// <summary>AddSysMsg（4508-4536）：按 (X, Y, DownToUP, DrawBottom) 四元组分组建 TDrawSysMsg。</summary>
    [Fact]
    public void TDrawScreenScrn_AddSysMsg_GroupsByQuadruple()
    {
        var d = new TDrawScreenScrn();
        d.AddSysMsg("a", 1, 0, 10, 20);
        d.AddSysMsg("b", 1, 0, 10, 20);
        Assert.Equal(1, d.m_SysMsgList.Count);
        Assert.Equal(2, ((TDrawSysMsg)d.m_SysMsgList.GetObject(0)).m_MsgList.Count);

        d.AddSysMsg("c", 1, 0, 10, 21);
        Assert.Equal(2, d.m_SysMsgList.Count);

        d.AddSysMsg("d", 1, 0, 10, 20, false, true);
        Assert.Equal(3, d.m_SysMsgList.Count);
    }

    /// <summary>Initialize / Finalize_ 只翻 `m_boInitialize` 并转调 FScreenMoveMsgList（4429-4439）。</summary>
    [Fact]
    public void TDrawScreenScrn_InitializeAndFinalize_ToggleFlag()
    {
        var d = new TDrawScreenScrn();
        d.Initialize();
        Assert.True(d.m_boInitialize);
        d.Finalize_();
        Assert.False(d.m_boInitialize);
    }

    /// <summary>
    /// DrawMsg_TopLevel / DrawSysMsg_BottomLevel / DrawMove / DrawMoveBefor 的**双门**：
    /// `m_boInitialize` 且 `CurrentScene = PlayScene`（4827-4828 等）。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_DrawGates_RequireInitializeAndPlayScene()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        DrawScrnEnv.PlayScene = NewPlayScene();

        var d = new TDrawScreenScrn();
        d.AddSysMsg("a", 1, 0, 10, 20, false, false);
        d.AddSysMsg("b", 1, 0, 10, 20, false, true);

        // 未 Initialize → 全部不画
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawMsg_TopLevel();
        d.DrawSysMsg_BottomLevel();
        d.DrawMove();
        d.DrawMoveBefor();
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);

        // Initialize 了但场景不对 → 仍不画
        d.Initialize();
        d.CurrentScene = NewLoginScene();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawMsg_TopLevel();
        d.DrawSysMsg_BottomLevel();
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);

        // 场景对了 → TopLevel 画"非底部"的那条
        d.CurrentScene = DrawScrnEnv.PlayScene;
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawMsg_TopLevel();
        Assert.Single(DrawScrnEnv.GameCanvas.Ops);

        DrawScrnEnv.GameCanvas.Clear();
        d.DrawSysMsg_BottomLevel();
        Assert.Single(DrawScrnEnv.GameCanvas.Ops);   // 只画 boDrawBottom 的那条
    }

    /// <summary>ShowHint 直接转调全局 HintWindows（4632-4654）。</summary>
    [Fact]
    public void TDrawScreenScrn_ShowHint_ForwardsToHintWindows()
    {
        var d = new TDrawScreenScrn();
        d.ShowHint(1, 2, "AB", TColor.clWhite, false, false, true);
        Assert.Equal(1, DrawScrnEnv.HintWindows.Count);
    }

    /// <summary>ClearHint 只清 HintList（4656-4659）。</summary>
    [Fact]
    public void TDrawScreenScrn_ClearHint_ClearsList()
    {
        var d = new TDrawScreenScrn();
        d.HintList.Add("x");
        d.ClearHint();
        Assert.Equal(0, d.HintList.Count);
    }

    /// <summary>
    /// ChangeScene（4465-4506）：场景取自 **MShare.pas:1457-1461 的单元级全局**；
    /// `stSelectCountry`/`stNewChr`/`stLoading` 三支**不改 CurrentScene**（空臂）。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_ChangeScene_UsesMShareGlobals_AndKeepsCurrentOnEmptyArms()
    {
        DrawScrnEnv.WelcomeScene = new TWelcomeScene();
        DrawScrnEnv.LoginScene = NewLoginScene();
        DrawScrnEnv.SelectChrScene = NewSelectChrScene();
        DrawScrnEnv.LoginNoticeScene = new TLoginNotice();
        DrawScrnEnv.PlayScene = NewPlayScene();

        var d = new TDrawScreenScrn();
        d.ChangeScene(TSceneType.stWelcome);
        Assert.Same(DrawScrnEnv.WelcomeScene, d.CurrentScene);

        d.ChangeScene(TSceneType.stSelectCountry);   // 空臂：CurrentScene 保持 WelcomeScene
        Assert.Same(DrawScrnEnv.WelcomeScene, d.CurrentScene);

        d.ChangeScene(TSceneType.stNewChr);
        Assert.Same(DrawScrnEnv.WelcomeScene, d.CurrentScene);

        d.ChangeScene(TSceneType.stLoading);
        Assert.Same(DrawScrnEnv.WelcomeScene, d.CurrentScene);

        d.ChangeScene(TSceneType.stPlayGame);
        Assert.Same(DrawScrnEnv.PlayScene, d.CurrentScene);
    }

    /// <summary>
    /// `SetShowLoginSceneShowRandomCodeDlg` + `ChangeScene(stLogin)` 会触发 `FrmDlg.OpenDRandomCodeDlg`
    /// （4497-4499）。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_ChangeScene_RandomCodeHook()
    {
        DrawScrnEnv.LoginScene = NewLoginScene();
        int opened = 0;
        DrawScrnEnv.OpenDRandomCodeDlgFn = () => opened++;

        var d = new TDrawScreenScrn();
        d.ChangeScene(TSceneType.stLogin);
        Assert.Equal(0, opened);                          // 未置标志

        d.SetShowLoginSceneShowRandomCodeDlg();
        d.ChangeScene(TSceneType.stLogin);
        Assert.Equal(1, opened);
    }

    /// <summary>AddChatBoardString / AddTopChatBoardString 转发到 FrmDlg.DChatMemo（4602-4610）。</summary>
    [Fact]
    public void TDrawScreenScrn_ChatBoard_ForwardsToChatMemo()
    {
        var memo = new ChatMemoSeam();
        DrawScrnEnv.DChatMemo = memo;

        var d = new TDrawScreenScrn();
        d.AddChatBoardString("hello", 1, 2);
        d.AddTopChatBoardString("top", 3, 4, 5000);

        Assert.Single(memo.Adds);
        Assert.Equal(("hello", 1, 2), memo.Adds[0]);
        Assert.Single(memo.TopAdds);
        Assert.Equal(("top", 3, 4, 5000), memo.TopAdds[0]);
    }

    /// <summary>
    /// AddMoveMsg / AddNewMoveMsg / AddNewLineMsg / AddMoveHintMsg 的转发（4612-4630）——各落一条。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_AddMsgForwarders()
    {
        _now = 0;
        var d = new TDrawScreenScrn();

        d.AddMoveMsg("A", 255, 0, 100, 1);
        Assert.Equal(1, d.FScreenMoveMsgList.m_MoveObjList.Count);

        d.AddNewMoveMsg("B", 255, 0, 20, 0, 100, 1);
        Assert.Equal(1, d.FScreenNewMoveMsgList.FItemList.Count);

        d.AddNewLineMsg("C", 255, 0, 20, 0, 100, 5, 100);
        Assert.Equal(1, d.FScreenNewLineMsgList.FItemList.Count);

        d.AddMoveHintMsg("D", 2, 0, 1, 1);
        Assert.Equal(1, d.FMoveHintMsgList.FItemList.Count);
    }

    /// <summary>
    /// DrawScreen（4693-4818）：区域状态位 `$01 shl I` 逐位画右上角图标（总宽从右往左累加），
    /// `$04` 位在有绿色信息时另起一行画「攻城区域」。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_DrawScreen_AreaStateIconsAndSiegeText()
    {
        DrawScrnEnv.g_MySelf = new TActor();

        var main = new TGameImages(DrawScrnConst.AREASTATEICONBASE + 16);
        main[DrawScrnConst.AREASTATEICONBASE + 0] = new TTexture { Width = 10, Height = 10 };
        main[DrawScrnConst.AREASTATEICONBASE + 2] = new TTexture { Width = 20, Height = 10 };
        MShareGlobals.g_WMainImages = main;

        DrawScrnEnv.g_nAreaStateValue = 0x01 | 0x04;   // bit0 与 bit2
        DrawScrnEnv.PlugInEnabled = false;             // 绿色信息三重在插件关闭时不走

        var d = new TDrawScreenScrn();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawScreen();

        // bit0 宽 10 → 画在 x = 1024-10；bit2 宽 20 → 累计 30 → x = 1024-30
        Assert.Equal(1024 - 10, DrawScrnEnv.GameCanvas.Ops[0].X);
        Assert.Equal(1024 - 30, DrawScrnEnv.GameCanvas.Ops[1].X);

        // $04 且绿色信息为空 → 画在 (2, 0)
        Assert.Equal(3, DrawScrnEnv.GameCanvas.Ops.Count);
        Assert.Equal(2, DrawScrnEnv.GameCanvas.Ops[2].X);
        Assert.Equal(0, DrawScrnEnv.GameCanvas.Ops[2].Y);
        Assert.Equal("攻城区域", DrawScrnEnv.GameCanvas.Ops[2].Text);
    }

    /// <summary>
    /// 绿色信息（4739-4808）：非新式样式时包含等级/经验/负重/金币/元宝/鼠标坐标；且「目标」段超宽会换行。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_DrawScreen_GreenHintLegacyText()
    {
        var me = new TActor();
        me.m_sUserName = "ME";
        DrawScrnEnv.g_MySelf = me;
        DrawScrnEnv.g_FocusCret = null;
        DrawScrnEnv.g_MyHero = null;
        ActorUiFields.SetGold(me, 123);
        ActorUiFields.SetGameGold(me, 456);
        DrawScrnEnv.boGreenHintNewStyle = false;
        DrawScrnEnv.PlugInEnabled = true;
        DrawScrnEnv.boShowGreenHint = true;
        DrawScrnEnv.ConfigCheckedFn = _ => true;
        DrawScrnEnv.g_nMouseCurrX = 7;
        DrawScrnEnv.g_nMouseCurrY = 8;
        DrawScrnEnv.g_nMouseX = 9;
        DrawScrnEnv.g_nMouseY = 10;

        var d = new TDrawScreenScrn();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawScreen();

        var text = DrawScrnEnv.GameCanvas.Ops[0];
        Assert.Equal(ScrnDrawKind.Text, text.Kind);
        Assert.Equal(2, text.X);
        Assert.Contains("等级: ", text.Text);
        Assert.Contains("金币: 123", text.Text);
        Assert.Contains("元宝: 456", text.Text);
        Assert.Contains("鼠标: 7:8(9:10)", text.Text);
        Assert.Contains("目标: -/-", text.Text);
        Assert.Equal(DrawScrnEnv.clLime.Value, text.FColor.Value);
    }

    /// <summary>绿色信息的三重门：`PlugInEnabled and boShowGreenHint and ConfigCheckeds[ckShowGreenHint]`（4719）。</summary>
    [Theory]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void TDrawScreenScrn_DrawScreen_GreenHintTripleGate(bool plugin, bool show, bool checked_)
    {
        DrawScrnEnv.g_MySelf = new TActor();
        DrawScrnEnv.g_nAreaStateValue = 0;
        DrawScrnEnv.PlugInEnabled = plugin;
        DrawScrnEnv.boShowGreenHint = show;
        DrawScrnEnv.ConfigCheckedFn = _ => checked_;

        var d = new TDrawScreenScrn();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawScreen();
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);
    }

    /// <summary>
    /// 焦点目标血量显示门（4761-4781）：人物族用 `boHumStruckShowNumber`、其余用 `boMonStruckShowNumber`，
    /// 且 `RC_MERCHANT` 即使命中也不显示。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_DrawScreen_FocusHpGate()
    {
        var me = new TActor();
        DrawScrnEnv.g_MySelf = me;
        DrawScrnEnv.g_MyHero = null;
        DrawScrnEnv.PlugInEnabled = true;
        DrawScrnEnv.boShowGreenHint = true;
        DrawScrnEnv.ConfigCheckedFn = _ => true;
        DrawScrnEnv.boGreenHintNewStyle = false;
        DrawScrnEnv.boHumStruckShowNumber = true;
        DrawScrnEnv.boMonStruckShowNumber = true;

        // 人物族 + MaxHP>0 → 显示真实数字
        var hum = new TActor { m_btRace = ActorLabelConsts.RC_PLAYOBJECT, m_sUserName = "P", m_boStruckShowNumber = true };
        hum.m_Abil = new GXX.Core.Protocol.TAbility { HP = 3, MaxHP = 9 };
        DrawScrnEnv.g_FocusCret = hum;

        var d = new TDrawScreenScrn();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawScreen();
        Assert.Contains("目标: P(3/9)", DrawScrnEnv.GameCanvas.Ops[0].Text);

        // 商人 + MaxHP>0 → 仍显示 (0/0)
        var merchant = new TActor { m_btRace = ActorLabelConsts.RC_MERCHANT, m_sUserName = "M", m_boStruckShowNumber = true };
        merchant.m_Abil = new GXX.Core.Protocol.TAbility { HP = 3, MaxHP = 9 };
        DrawScrnEnv.g_FocusCret = merchant;
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawScreen();
        Assert.Contains("目标: M(0/0)", DrawScrnEnv.GameCanvas.Ops[0].Text);
    }

    /// <summary>英雄段用 `DelphiRTL.Format('我的英雄: %s(%d/%d)')`（4795）。</summary>
    [Fact]
    public void TDrawScreenScrn_DrawScreen_HeroSegment()
    {
        DrawScrnEnv.g_MySelf = new TActor();
        DrawScrnEnv.PlugInEnabled = true;
        DrawScrnEnv.boShowGreenHint = true;
        DrawScrnEnv.ConfigCheckedFn = _ => true;
        DrawScrnEnv.boGreenHintNewStyle = false;
        DrawScrnEnv.g_FocusCret = null;
        DrawScrnEnv.g_MyHero = new TActor { m_sUserName = "H", m_nCurrX = 11, m_nCurrY = 22 };

        var d = new TDrawScreenScrn();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawScreen();

        Assert.Contains("我的英雄: H(11/22)", DrawScrnEnv.GameCanvas.Ops[0].Text);
    }

    /// <summary>DrawHint（4896-4899）在原文里是空体。</summary>
    [Fact]
    public void TDrawScreenScrn_DrawHint_IsEmpty()
    {
        var d = new TDrawScreenScrn();
        DrawScrnEnv.GameCanvas.Clear();
        d.DrawHint();
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);
    }

    /// <summary>
    /// ClearChatBoard（4661-4691）清掉全部子集合（滚动/新滚动/换行/中心/延迟 + 两套 SysMsg）。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_ClearChatBoard_ClearsEverything()
    {
        _now = 0;
        var d = new TDrawScreenScrn();
        d.AddMoveMsg("A", 255, 0, 100, 1);
        d.AddNewMoveMsg("B", 255, 0, 20, 0, 100, 1);
        d.AddNewLineMsg("C", 255, 0, 20, 0, 100, 5, 100);
        d.AddSysMsg("D", 1, 0, 10, 20);

        d.ClearChatBoard();

        Assert.Equal(0, d.FScreenMoveMsgList.m_MoveObjList.Count);
        Assert.Equal(0, d.FScreenNewMoveMsgList.FItemList.Count);
        Assert.Equal(0, d.FScreenNewLineMsgList.FItemList.Count);
        Assert.Equal(0, d.DrawScreenCenterMsg.m_TextList.Count);
        Assert.Equal(0, d.DrawDelayMsg.m_MsgList.Count);
        Assert.Equal(0, d.m_SysMsgList.Count);
        Assert.Equal(0, d.m_SysMsgListEx.Count);
    }

    /// <summary>
    /// Update（4420-4427）转调四个集合 + **全局** `HintWindows.UpDate`（原文 4425 用的是
    /// MShare.pas:1464 的单元级全局，不是成员）。
    /// </summary>
    [Fact]
    public void TDrawScreenScrn_Update_TouchesHintWindowsGlobal()
    {
        _now = 0;
        DrawScrnEnv.HintWindows = new THintWindows();
        DrawScrnEnv.HintWindows.Show(0, 0, "A", TColor.clWhite);
        DrawScrnEnv.HintWindows.GetItems(0).Visible = false;

        var d = new TDrawScreenScrn();
        d.Update();

        // UpDate 是空实现 → 不可见窗口**不会**被清掉（原文如此）
        Assert.Equal(1, DrawScrnEnv.HintWindows.Count);
    }
}
