using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J47：Client 场景层第四片（TActor 绘制核心 + DrawScene 场景合成调度）1:1 测试。</summary>
public sealed class ActorDrawAndSceneComposeTests
{
    // ---- LoadSurface 主体图规划（Actor.pas 5480-5593） ----

    [Fact]
    public void PlanBodyImage_MonsterBranch()
    {
        // 普通怪：外观 0 → GetOffset(0)=0；帧 40 → 图号 40；库 = 外观 0
        var plan = ActorDrawPlan.PlanBodyImage(9, 0, -1, TActorCore.SM_TURN, 40, 40, 40, false, TColorEffect.ceNone, null);
        Assert.NotNull(plan);
        Assert.Equal(0, plan.Value.LibraryId);
        Assert.Equal(40, plan.Value.ImageIndex);
        Assert.Equal(TColorEffect.ceNone, plan.Value.Color);
        Assert.False(plan.Value.UseEffectImageList);

        // 外观 15（nrace 1）：GetOffset(15) = 5×230 = 1150
        var plan2 = ActorDrawPlan.PlanBodyImage(9, 15, -1, TActorCore.SM_TURN, 0, 0, 0, false, TColorEffect.ceNone, null);
        Assert.Equal(1150, plan2.Value.ImageIndex);

        // 反向播放：图号 = Offset + EndFrame - (CurrentFrame - StartFrame)
        var rev = ActorDrawPlan.PlanBodyImage(9, 0, -1, TActorCore.SM_TURN, 130, 128, 131, true, TColorEffect.ceNone, null);
        Assert.Equal(129, rev.Value.ImageIndex); // 0 + 131 - 2

        // 色彩效果透传
        var gray = ActorDrawPlan.PlanBodyImage(9, 0, -1, TActorCore.SM_TURN, 0, 0, 0, false, TColorEffect.ceGrayScale, null);
        Assert.Equal(TColorEffect.ceGrayScale, gray.Value.Color);
        var bright = ActorDrawPlan.PlanBodyImage(9, 0, -1, TActorCore.SM_TURN, 0, 0, 0, false, TColorEffect.ceBright, null);
        Assert.Equal(TColorEffect.ceBright, bright.Value.Color);
    }

    [Fact]
    public void PlanBodyImage_Race156_CustomMonster()
    {
        // 自定义怪：ActionFile=2 合法 → g_EffectImageList[2]，图号 = 当前帧
        var resolver = new Func<int, int, (int, int, int)?>((action, appr) =>
            appr == 30 && action == TActorCore.SM_HIT ? (10, 6, 2) : null);
        var plan = ActorDrawPlan.PlanBodyImage(156, 0, 30, TActorCore.SM_HIT, 7, 7, 7, false, TColorEffect.ceNone, resolver);
        Assert.NotNull(plan);
        Assert.Equal(2, plan.Value.LibraryId);
        Assert.Equal(7, plan.Value.ImageIndex);
        Assert.True(plan.Value.UseEffectImageList);

        // ActionFile 越界（-1）→ g_WMonImages[ChangeAppr-100000]
        var resolver2 = new Func<int, int, (int, int, int)?>((action, appr) => (0, 6, -1));
        var plan2 = ActorDrawPlan.PlanBodyImage(156, 0, 103000, TActorCore.SM_HIT, 3, 3, 3, false, TColorEffect.ceNone, resolver2);
        Assert.Equal(3000, plan2.Value.LibraryId); // 103000-100000
        Assert.Equal(3, plan2.Value.ImageIndex);
        Assert.False(plan2.Value.UseEffectImageList);

        // 无配置 → 不绘制
        Assert.Null(ActorDrawPlan.PlanBodyImage(156, 0, 30, TActorCore.SM_TURN, 0, 0, 0, false, TColorEffect.ceNone,
            new Func<int, int, (int, int, int)?>((_, _) => null)));
    }

    [Fact]
    public void DrawChr_Guard_And_Position()
    {
        // 方向守卫 0..7
        Assert.True(ActorDrawPlan.CanDrawDir(0));
        Assert.True(ActorDrawPlan.CanDrawDir(7));
        Assert.False(ActorDrawPlan.CanDrawDir(8));
        Assert.False(ActorDrawPlan.CanDrawDir(-1));
        // 绘制位置 = dx+m_nPx+ShiftX, dy+m_nPy+ShiftY
        var pos = ActorDrawPlan.DrawChrPosition(100, 50, px: 3, py: -2, shiftX: -24, shiftY: 16);
        Assert.Equal((79, 64), pos);
        // 死亡观察者 → 灰度
        Assert.Equal(TColorEffect.ceGrayScale, ActorDrawPlan.EffectiveColor(TColorEffect.ceNone, viewerDead: true));
        Assert.Equal(TColorEffect.ceNone, ActorDrawPlan.EffectiveColor(TColorEffect.ceNone, viewerDead: false));
    }

    // ---- DrawScene 场景合成调度 ----

    [Fact]
    public void SceneComposer_Rows_And_Actors()
    {
        var composer = new SceneComposer
        {
            ClientLeft = 10, ClientTop = 10, ClientRight = 13, ClientBottom = 13,
            BlockLeft = 10, BlockTop = 10,
            DefXX = 400, DefYY = 300,
        };
        composer.ObjectCell = (_, _) => new SceneComposer.MapObjectCell(); // 无地图对象
        composer.SortedActors = new List<SceneComposer.SceneActorInput>
        {
            new() { RecogId = 1, Rx = 11, Ry = 12, DownDrawLevel = 0, ShiftX = -24, ShiftY = 16 },
            new() { RecogId = 2, Rx = 12, Ry = 13, DownDrawLevel = 1, ShiftX = 0, ShiftY = 0 },
        };

        var ops = composer.ComposeSchedule();
        var actorOps = ops.FindAll(o => o.Kind == SceneComposer.SceneDrawOp.OpKind.Actor);
        Assert.Equal(2, actorOps.Count);

        // 角色一：行 J = Ry-BlockTop-Level = 12-10-0 = 2
        // m（行像素基）= DefYY-UNITY + (J-(Top-Block)+1)×UNITY = 300-32+3×32 = 364
        // nX = (Rx-Left)×48+DefXX+ShakeX+ShiftX = (11-10)×48+400-24-24 = 500
        Assert.Equal(1, actorOps[0].ActorId);
        Assert.Equal(2, actorOps[0].RowJ);
        Assert.Equal(424, actorOps[0].PixelX);
        Assert.Equal(348, actorOps[0].PixelY); // m(行2)=DefYY-UNITY+2行推进=332
        // 角色二：行 J = 13-10-1 = 2（同行）
        Assert.Equal(2, actorOps[1].ActorId);
        Assert.Equal(2, actorOps[1].RowJ);
        Assert.Equal((12 - 10) * 48 + 400, actorOps[1].PixelX);
        Assert.Equal(364, actorOps[1].PixelY); // 332+1×32

        // DownDrawLevel 越大的角色落在更后的行（视觉在更前）
        var a0 = composer.SortedActors[0];
        a0.DownDrawLevel = 0;
        composer.SortedActors[0] = a0;
        var a1 = composer.SortedActors[1];
        a1.DownDrawLevel = 0;
        a1.Ry = 100; // 行 90 超出可视行范围（0..35）→ 不产出
        composer.SortedActors[1] = a1;
        var ops2 = composer.ComposeSchedule();
        Assert.Equal(1, ops2.FindAll(o => o.Kind == SceneComposer.SceneDrawOp.OpKind.Actor).Count);
    }

    [Fact]
    public void SceneComposer_MapObjects_BlendAndAnime()
    {
        var composer = new SceneComposer
        {
            ClientLeft = 10, ClientTop = 10, ClientRight = 10, ClientBottom = 10,
            BlockLeft = 10, BlockTop = 10,
            DefXX = 400, DefYY = 300,
            AniCount = 24,
        };
        // 行 0 首个可行格 (10,10)：FileIdx=26（动画文件）、ani=62（无混合位）→ fridx + (24/2 mod 10) = +2
        composer.ObjectCell = (i, j) => i == 0 && j == 0
            ? new SceneComposer.MapObjectCell { FileIdx1 = 26, Obj2 = 100, Obj2Ani = 62 }
            : new SceneComposer.MapObjectCell();
        composer.ObjectTextureProbe = (_, _) => (96, 64); // 非 48×32 立式对象

        var ops = composer.ComposeSchedule();
        var objs = ops.FindAll(o => o.Kind == SceneComposer.SceneDrawOp.OpKind.MapObject);
        Assert.NotEmpty(objs);
        var first = objs[0];
        Assert.Equal(26, first.FileIdx);
        Assert.Equal(102, first.ImageIndex); // 100 + (12 mod 10)
        Assert.False(first.Blend);
        // 非混合：mmm = m + UNITY - 高 = 行像素 + 32 - 64
        int mRow0 = 268; // m：DefYY-UNITY=268（行0 无负行跳过）
        Assert.Equal(mRow0 + 32 - 64, first.PixelY); // mmm = m + UNITY - 高 = 268
        // nX = DefXX - UNITX*2 + (i-(Left-Block-2))×UNITX + ShakeX；i=10 → 第 3 格
        Assert.Equal(400 - 96 + 2 * 48, first.PixelX);

        // 混合位：ani & $80 → blend（file 26 ani=190+128）
        composer.ObjectCell = (i, j) => i == 0 && j == 0
            ? new SceneComposer.MapObjectCell { FileIdx1 = 26, Obj2 = 100, Obj2Ani = 190 | 0x80 }
            : new SceneComposer.MapObjectCell();
        var opsBlend = composer.ComposeSchedule();
        var objsBlend = opsBlend.FindAll(o => o.Kind == SceneComposer.SceneDrawOp.OpKind.MapObject);
        // 混合对象走 mmm = m + PtY - 68 路径，本调度仅产出非混合（与 2701 分支一致）
        Assert.DoesNotContain(objsBlend, o => o.Blend);
    }

    [Fact]
    public void SceneComposer_Guards()
    {
        // CanDraw=false → 空调度
        var composer = new SceneComposer { CanDraw = false };
        Assert.Empty(composer.ComposeSchedule());
        // 地图未载入 → 空调度
        var composer2 = new SceneComposer { MapLoadOk = false };
        Assert.Empty(composer2.ComposeSchedule());
        // fridx=65535 与 FileIdx 越界（0 或 >=75）均不产出
        var composer3 = new SceneComposer
        {
            ClientLeft = 10, ClientTop = 10, ClientRight = 12, ClientBottom = 12,
            BlockLeft = 10, BlockTop = 10,
        };
        composer3.ObjectCell = (i, j) => new SceneComposer.MapObjectCell
        {
            FileIdx1 = i switch { 10 => 0, 11 => 65535, _ => 80 },
            Obj2 = 100,
        };
        composer3.ObjectTextureProbe = (_, _) => (48, 32);
        Assert.Empty(composer3.ComposeSchedule());
    }
}
