using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J46：Client 场景层第三片（TChrMsg 消息队列/ReadyAction/Shift 位移 + 地图分块渲染调度）1:1 测试。</summary>
public sealed class ActorMessageAndMapTests
{
    // ---- Shift 位移（Actor.pas 5000-5167） ----

    [Fact]
    public void Shift_Up_FirstFrame()
    {
        var actor = new TActorCore { m_nCurrX = 10, m_nCurrY = 10 };
        actor.Shift(0 /*DR_UP*/, 1, 0, 6);
        // ss = Round((6-0)/6)×1 = 1 → m_nRy = 10+1；ss=step → shiftY = -Round(32/6×0) = 0（偶）
        Assert.Equal(0, actor.m_nShiftX);
        Assert.Equal(0, actor.m_nShiftY);
        Assert.Equal(10, actor.m_nRx);
        Assert.Equal(11, actor.m_nRy);
    }

    [Fact]
    public void Shift_Up_MidFrame()
    {
        var actor = new TActorCore { m_nCurrX = 10, m_nCurrY = 10 };
        actor.Shift(0, 1, 3, 6); // cur=3：ss=Round(3/6)=0（银行家舍入）≠step → shiftY = +Round(32/6×3) = +16
        Assert.Equal(0, actor.m_nShiftX);
        Assert.Equal(16, actor.m_nShiftY);
        Assert.Equal(10, actor.m_nRy); // ss=0 → 不变
    }

    [Fact]
    public void Shift_Right_OddAdjust()
    {
        var actor = new TActorCore { m_nCurrX = 10, m_nCurrY = 10 };
        actor.Shift(2 /*DR_RIGHT*/, 1, 4, 6); // ss=Round(2/6)=0 → shiftX = -Round(48/6×2)=-16（偶）
        Assert.Equal(-16, actor.m_nShiftX);
        Assert.Equal(0, actor.m_nShiftY);
        // cur=1：ss=Round(5/6)=1=step → shiftX = +Round(48/6×1) = +8
        var odd = new TActorCore { m_nCurrX = 10, m_nCurrY = 10 };
        odd.Shift(2, 1, 1, 6);
        Assert.Equal(8, odd.m_nShiftX);
    }

    [Fact]
    public void Shift_DownRight_HorseQuirks()
    {
        // step=3 → v=1（骑马黑边修正）；unx=48×3=144、uny=32×3=96 随 step 缩放
        var actor = new TActorCore { m_nCurrX = 20, m_nCurrY = 20 };
        actor.Shift(3 /*DR_DOWNRIGHT*/, 3, 5, 6);
        // v=1：ss = Round((6-5-1)/6)×3 = 0 → shift = -Round(144/6×1)=-24、-Round(96/6×1)=-16（均偶）
        Assert.Equal(20, actor.m_nRx);
        Assert.Equal(20, actor.m_nRy);
        Assert.Equal(-24, actor.m_nShiftX);
        Assert.Equal(-16, actor.m_nShiftY);
    }

    [Fact]
    public void Shift_UpLeft_Step4_Quirk()
    {
        // step=4：cur=3 → v=0；否则 v=1；unx=48×4=192
        var actor = new TActorCore { m_nCurrX = 10, m_nCurrY = 10 };
        actor.Shift(7 /*DR_UPLEFT*/, 4, 3, 6);
        // v=0：ss = Round((6-3+0)/6)×4 = 0 → shiftX = +Round(192/6×3)=96（偶）、shiftY = +Round(32/6×3)=+16（偶）
        Assert.Equal(96, actor.m_nShiftX);
        Assert.Equal(64, actor.m_nShiftY); // uny=32×4=128 → 128/6×3=64
        Assert.Equal(10, actor.m_nRx);
        Assert.Equal(10, actor.m_nRy);
    }

    [Fact]
    public void Shift_FullStep_ReachesTarget()
    {
        var actor = new TActorCore { m_nCurrX = 7, m_nCurrY = 9 };
        actor.Shift(4 /*DR_DOWN*/, 1, 6, 6);
        // cur=Max：ss = Round(0/6)×1 = 0... Delphi DOWN 分支 v=1（Max>=6）→ ss = Round((6-6-1)/6)=0 → shiftY=-Round(0)=-0=0
        Assert.Equal(0, actor.m_nShiftY);
        Assert.Equal(9, actor.m_nRy);
    }

    // ---- 消息队列与 ReadyAction ----

    [Fact]
    public void SendMsg_Queue_FIFO_Dequeue()
    {
        var actor = new TActorCore();
        actor.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN, X = 1, Y = 2, Dir = 3 });
        actor.SendMsg(new TChrMsg { Ident = TActorCore.SM_HIT, X = 4, Y = 5, Dir = 6 });
        Assert.Equal(2, actor.MsgList.Count);

        Assert.True(actor.GetNextMsg(out var first));
        Assert.Equal(TActorCore.SM_TURN, first.Ident);
        Assert.Equal(1, first.X);
        Assert.Equal(1, actor.MsgList.Count);

        Assert.True(actor.GetNextMsg(out var second));
        Assert.Equal(TActorCore.SM_HIT, second.Ident);
        Assert.False(actor.GetNextMsg(out _)); // 空队列 → false
    }

    [Fact]
    public void ReadyAction_Turn_AppliesCoordAndFrame()
    {
        uint tick = 5000;
        var actor = new TActorCore { TimeGetTime = () => tick };
        actor.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN, X = 7, Y = 9, Dir = 5, State = 0 });
        Assert.True(actor.GetNextMsg(out var msg));

        actor.m_btRace = 9; // MA9：ActStand start=0 frame=1 skip=7 ftime=200
        actor.ReadyAction(msg);

        // else 分支坐标应用 + ReadyAction 内旧坐标记录
        Assert.Equal(7, actor.m_nCurrX);
        Assert.Equal(9, actor.m_nCurrY);
        Assert.Equal(5, actor.m_btDir);
        Assert.Equal(TActorCore.SM_TURN, actor.m_nCurrentAction);
        Assert.Equal(tick, actor.m_dwCurrentActionTick);
        // CalcActorFrame 已跑：start = 0 + 5×(1+7) = 40
        Assert.Equal(40, actor.m_nStartFrame);
        Assert.Equal(40, actor.m_nEndFrame);
        Assert.Equal(200u, actor.m_dwFrameTime);
    }

    [Fact]
    public void ReadyAction_Struck_FrameTimeFormula()
    {
        // 公式：n = Round(200 - Level×5)，n>80 取 n 否则 80，再加怪物弯腰延时
        var actor = new TActorCore { m_nLevel = 24 }; // 200-120=80 → 不大于 80 → 80
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_STRUCK, X = 1, Y = 1 });
        Assert.Equal(80, actor.m_dwStruckFrameTime);

        var low = new TActorCore { m_nLevel = 10 }; // 150 > 80 → 150
        low.ReadyAction(new TChrMsg { Ident = TActorCore.SM_STRUCK, X = 1, Y = 1 });
        Assert.Equal(150, low.m_dwStruckFrameTime);

        var delayed = new TActorCore { m_nLevel = 10, m_btMonStruckFrameDelayTime = 6 };
        delayed.ReadyAction(new TChrMsg { Ident = TActorCore.SM_STRUCK, X = 1, Y = 1 });
        Assert.Equal(156, delayed.m_dwStruckFrameTime);

        var veryLow = new TActorCore { m_nLevel = 30 }; // 200-150=50 → 钳 80
        veryLow.ReadyAction(new TChrMsg { Ident = TActorCore.SM_STRUCK, X = 1, Y = 1 });
        Assert.Equal(80, veryLow.m_dwStruckFrameTime);
    }

    [Fact]
    public void ReadyAction_BackStep_PackedDirAndStep()
    {
        // BACKSTEP/100HIT：dir 参数低字节方向、高字节步数（0 → 1）
        var actor = new TActorCore { m_btRace = 9 };
        int packedDir = 7 | (3 << 8); // 方向 7 + 步数 3
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_BACKSTEP, X = 5, Y = 6, Dir = packedDir });
        Assert.Equal(7, actor.m_btDir);
        Assert.Equal(3, actor.m_btStep);
        Assert.Equal(TActorCore.SM_BACKSTEP, actor.m_nCurrentAction);

        var zeroStep = new TActorCore { m_btRace = 9 };
        zeroStep.ReadyAction(new TChrMsg { Ident = TActorCore.SM_100HIT, X = 5, Y = 6, Dir = 4 });
        Assert.Equal(4, zeroStep.m_btDir);
        Assert.Equal(1, zeroStep.m_btStep); // 0 → 1
    }

    [Fact]
    public void ReadyAction_Death_MarksState()
    {
        uint tick = 777;
        var actor = new TActorCore { TimeGetTime = () => tick };
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_NOWDEATH, X = 2, Y = 3, Dir = 1 });
        Assert.True(actor.m_boDeath);
        Assert.Equal(tick, actor.m_dwDeathTick);
        Assert.False(actor.m_boStruckShowNumber);
        Assert.False(actor.m_boShowBigHPProgress);
        Assert.Equal(TActorCore.SM_NOWDEATH, actor.m_nCurrentAction);
    }

    [Fact]
    public void ReadyAction_Alive_ReversesDeath()
    {
        var actor = new TActorCore { m_boDeath = true };
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_ALIVE, X = 1, Y = 1 });
        Assert.False(actor.m_boDeath);
        Assert.False(actor.m_boSkeleton);
    }

    [Fact]
    public void ReadyAction_OpenHealthStateBit()
    {
        var actor = new TActorCore();
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_TURN, X = 1, Y = 1, State = ActorStates.STATE_OPENHEATH });
        Assert.True(actor.m_boOpenHealth);
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_TURN, X = 1, Y = 1, State = 0 });
        Assert.False(actor.m_boOpenHealth);
    }

    [Fact]
    public void ReadyAction_MagicFire_DoesNotSwitchAction()
    {
        var actor = new TActorCore();
        actor.m_nCurrentAction = TActorCore.SM_SPELL;
        actor.ReadyAction(new TChrMsg { Ident = TActorCore.SM_MAGICFIRE, X = 9, Y = 9 });
        Assert.Equal(TActorCore.SM_SPELL, actor.m_nCurrentAction); // MAGICFIRE 不切动作
        Assert.Equal(0, actor.m_nCurrX);                           // 坐标也不应用
    }

    [Fact]
    public void UpdateMsgMuch_MsgBacklog()
    {
        var actor = new TActorCore { IsSelf = true };
        actor.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN });
        actor.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN });
        Assert.False(actor.UpdateMsgMuch()); // 主角不加速
        var other = new TActorCore { IsSelf = false };
        other.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN });
        Assert.False(other.UpdateMsgMuch()); // 积压 1 不加速
        other.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN });
        Assert.True(other.UpdateMsgMuch());  // 积压 2 → 加速
    }

    // ---- 地图分块渲染调度（PlayScn.pas RenderTileMap） ----

    [Fact]
    public void MapSchedule_BackgroundPass_SteppingAndFiltering()
    {
        var scheduler = new MapRenderSchedule
        {
            ClientLeft = 10, ClientTop = 10, ClientRight = 13, ClientBottom = 13,
            BlockLeft = 10, BlockTop = 10,
        };
        scheduler.Cell = (i, j) => new MapRenderSchedule.MapCell
        {
            wBkImg = (ushort)((i % 2 == 0 && j % 2 == 0) ? 5 : 0),
            btUnitBkImg = 1,
        };
        scheduler.TextureProbe = (_, img) => (48, 32); // 全部纹理就绪

        var ops = scheduler.RenderTileMapSchedule();
        var bk = ops.FindAll(o => o.Layer == MapRenderSchedule.MapLayer.BkTile);
        // 可排行 J：-1..13（nY：-64 起步 +32/行）；可行列 I：-2..4（nX：-18 起步 +48/列）
        // 偶偶格有图：(0,0),(2,0),(4,0)×j∈{0,2,..,12} → 3 列 × 7 行 = 21 候选；
        // 但 j=0 行 nY=-32 满足 nY+h=0 不大于 0 → 被边界过滤 → 18 个绘制
        Assert.Equal(18, bk.Count);
        // 首个绘制 (0,2)：nX = -18 + (0-(-2))×48 = 78，nY = -64 + (2-(-1))×32 = 32；图号 5-1=4
        var first = bk[0];
        Assert.Equal(0, first.CellI);
        Assert.Equal(2, first.CellJ);
        Assert.Equal(78, first.PixelX);
        Assert.Equal(32, first.PixelY);
        Assert.Equal(4, first.ImgNumber);
        // (2,2)：nX = 78+96 = 174
        Assert.Equal(174, bk[1].PixelX);
        // (0,4)：nY = 32 + 64 = 96
        Assert.Equal(32, bk[2].PixelY); // 第三个 op 是 (4,2)，同行 nY=32
        // 边界过滤：nX<=1024 且 nY<768
        Assert.All(bk, o => { Assert.True(o.PixelX <= MapRenderSchedule.SCREENWIDTH); Assert.True(o.PixelY < MapRenderSchedule.MAPSURFACEHEIGHT); });
    }

    [Fact]
    public void MapSchedule_MidPass_NoBoundsClip_AndNewMapOffset()
    {
        var scheduler = new MapRenderSchedule
        {
            ClientLeft = 10, ClientTop = 10, ClientRight = 10, ClientBottom = 10,
            BlockLeft = 10, BlockTop = 10,
        };
        scheduler.Cell = (i, j) => new MapRenderSchedule.MapCell { wMidImg = 3 };
        scheduler.TextureProbe = (_, _) => (48, 32);

        // 旧图：中间层 nY = -UNITY 起步于首行（j=-1 被界外守卫跳过）；首个可行格 (i=0, j=0) → nX=78, nY=0
        var opsOld = scheduler.RenderTileMapSchedule();
        var midOld = opsOld.FindAll(o => o.Layer == MapRenderSchedule.MapLayer.MidTile);
        Assert.NotEmpty(midOld);
        Assert.Equal(78, midOld[0].PixelX);
        Assert.Equal(0, midOld[0].PixelY);
        Assert.Equal(2, midOld[0].ImgNumber); // 3-1
        // 每行 2 列（i=0..1），nY 换行 +32：第 3 个 op = 行 2 首列（j=2 → nY=64）
        Assert.Equal(32, midOld[2].PixelY);
        Assert.Equal(78, midOld[2].PixelX);

        // 新图：nY = -UNITY*2 起步；行 j=0 → nY=-32（为负仍产出，无边界过滤）
        scheduler.boNewMap = true;
        var opsNew = scheduler.RenderTileMapSchedule();
        var midNew = opsNew.FindAll(o => o.Layer == MapRenderSchedule.MapLayer.MidTile);
        Assert.NotEmpty(midNew);
        Assert.Equal(-32, midNew[0].PixelY);
        Assert.Equal(78, midNew[0].PixelX);
    }

    [Fact]
    public void MapSchedule_BkMask_And_MissingTexture()
    {
        var scheduler = new MapRenderSchedule
        {
            ClientLeft = 10, ClientTop = 10, ClientRight = 10, ClientBottom = 10,
            BlockLeft = 10, BlockTop = 10,
        };
        // wBkImg=0x8005：EN 地图不掩码 → 图号 0x8004；非 EN：& $7FFF = 5 → 图号 4
        scheduler.boENMap = true;
        scheduler.Cell = (i, j) => new MapRenderSchedule.MapCell { wBkImg = 0x8005 };
        var seenImages = new List<int>();
        scheduler.TextureProbe = (_, img) => { seenImages.Add(img); return null; }; // 纹理未就绪 → 不产出
        var opsEn = scheduler.RenderTileMapSchedule();
        Assert.DoesNotContain(opsEn, o => o.Layer == MapRenderSchedule.MapLayer.BkTile);
        Assert.Contains(0x8004, seenImages); // EN：0x8005 - 1

        scheduler.boENMap = false;
        var seenImages2 = new List<int>();
        scheduler.TextureProbe = (_, img) => { seenImages2.Add(img); return (48, 32); };
        var opsNotEn = scheduler.RenderTileMapSchedule();
        Assert.NotEmpty(opsNotEn);
        Assert.Contains(4, seenImages2); // 非 EN：&7FFF = 5 → -1 = 4
        Assert.Contains(opsNotEn, o => o.Layer == MapRenderSchedule.MapLayer.BkTile && o.ImgNumber == 4);
    }

    [Fact]
    public void MapSchedule_MapNotLoaded_ReturnsEmpty()
    {
        var scheduler = new MapRenderSchedule { MapLoadOk = false };
        Assert.Empty(scheduler.RenderTileMapSchedule());
    }
}
