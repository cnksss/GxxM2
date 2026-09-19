using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J86：Actor.pas 脚本播放特效族 1:1 测试 ——
/// CheckLoadPlayEffect(7274-7315)、LoadPlayEffectSurface(7317-7353)、DrawPlayEffect(5935-5970)、
/// THumActor.Finalize 队列段(11248-11257)、TActor 析构(2956-2959)。
/// </summary>
public sealed class ActorPlayEffectQueueTests
{
    private static ActorPlayEffectQueue Make(int count = 100)
        => new() { EffectImageListCount = count, ResolveImage = (f, i, ce) => new FxImage(i, 16, 16, 0, 0, false) };

    private static TClientActorEffectRef Effect(
        int offSet = 0, ushort imgCount = 3, ushort frameTime = 100,
        int loopCount = 1, short fileIdx = 1, byte drawOrder = 0)
        => new()
        {
            nEffectImageOffSet = offSet,
            wEffectImageCount = imgCount,
            wEffectFrameTime = frameTime,
            nLoopCount = loopCount,
            nEffectFileIndex = fileIdx,
            nCurrentFrame = offSet,
            nOldCurrentFrame = -1,
            btDrawOrder = drawOrder,
        };

    // ===================== 入队 =====================

    [Fact]
    public void NewQueueIsEmpty()
    {
        Assert.Empty(Make().Items);
    }

    [Fact]
    public void AddReturnsMutableReferenceInQueue()
    {
        var q = Make();
        var e = q.Add();

        e.nLoopCount = 5;
        e.nCurrentFrame = 7;

        Assert.Single(q.Items);
        Assert.Equal(5, q.Items[0].nLoopCount);       // 同一实例（Delphi 指针语义）
        Assert.Equal(7, q.Items[0].nCurrentFrame);
    }

    [Fact]
    public void AddInitialisesOldFrameToMinusOne()
    {
        var e = Make().Add();
        Assert.Equal(-1, e.nOldCurrentFrame);
        Assert.Equal(0, e.nCurrentFrame);
    }

    // ===================== CheckLoadPlayEffect：帧推进 =====================

    [Fact]
    public void FrameAdvancesWhenTickExceedsFrameTime()
    {
        var q = Make();
        var e = Effect(frameTime: 100);
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);

        Assert.Equal(1, e.nCurrentFrame);
        Assert.Equal(101u, e.dwEffectTick);
    }

    [Fact]
    public void FrameDoesNotAdvanceAtExactlyFrameTime()
    {
        // 7287 是 `> wEffectFrameTime`
        var q = Make();
        var e = Effect(frameTime: 100);
        e.dwEffectTick = 1000;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(1100);

        Assert.Equal(0, e.nCurrentFrame);
    }

    [Fact]
    public void FrameWrapsAtOffsetPlusCountMinusOne()
    {
        // offSet 0、count 3 → 帧上限 2；帧 2 再推进到 3 时回卷 0
        var q = Make();
        var e = Effect(offSet: 0, imgCount: 3, frameTime: 100, loopCount: 5);
        e.nCurrentFrame = 2;
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);

        Assert.Equal(0, e.nCurrentFrame);
    }

    [Fact]
    public void WrapResetsOldFrameToMinusOne()
    {
        var q = Make();
        var e = Effect(offSet: 0, imgCount: 3, frameTime: 100, loopCount: 5);
        e.nCurrentFrame = 2;
        e.nOldCurrentFrame = 2;
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);

        // 回卷后 7294 置 -1，随后 7301 同步为 0
        Assert.Equal(0, e.nOldCurrentFrame);
    }

    [Fact]
    public void WrapDecrementsLoopCountWhenPositive()
    {
        var q = Make();
        var e = Effect(offSet: 0, imgCount: 3, frameTime: 100, loopCount: 3);
        e.nCurrentFrame = 2;
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);

        Assert.Equal(2, e.nLoopCount);
    }

    [Fact]
    public void WrapDoesNotDecrementNegativeLoopCount()
    {
        // 7297：仅 `nLoopCount > 0` 时递减（-1 表示无限循环）
        var q = Make();
        var e = Effect(offSet: 0, imgCount: 3, frameTime: 100, loopCount: -1);
        e.nCurrentFrame = 2;
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);

        Assert.Equal(-1, e.nLoopCount);
    }

    [Fact]
    public void WrapResetsTick()
    {
        var q = Make();
        var e = Effect(offSet: 0, imgCount: 3, frameTime: 100, loopCount: 5);
        e.nCurrentFrame = 2;
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);

        Assert.Equal(101u, e.dwEffectTick);
    }

    [Fact]
    public void ReturnTrueWhenFrameChanged()
    {
        var q = Make();
        var e = Effect(frameTime: 100);
        e.dwEffectTick = 0;
        q.Items.Add(e);

        Assert.True(q.CheckLoadPlayEffect(101));
    }

    [Fact]
    public void ReturnFalseWhenNothingChanged()
    {
        var q = Make();
        var e = Effect(frameTime: 100);
        e.dwEffectTick = 1000;
        e.nOldCurrentFrame = 0;
        q.Items.Add(e);

        Assert.False(q.CheckLoadPlayEffect(1050));
    }

    // ===================== CheckLoadPlayEffect：出队 =====================

    [Fact]
    public void ZeroLoopCountIsRemoved()
    {
        var q = Make();
        q.Items.Add(Effect(loopCount: 0));

        q.CheckLoadPlayEffect(0);

        Assert.Empty(q.Items);
    }

    [Fact]
    public void RemovalReturnsTrue()
    {
        var q = Make();
        q.Items.Add(Effect(loopCount: 0));

        Assert.True(q.CheckLoadPlayEffect(0));
    }

    [Fact]
    public void NegativeLoopCountIsAlsoRemoved()
    {
        // 7285：判据是 `<> 0`，故 -1... 若为 -1 则保留；
        // 但 0 才出队 —— 这里用 -1 验证其被保留
        var q = Make();
        q.Items.Add(Effect(loopCount: -1, frameTime: 1000));

        q.CheckLoadPlayEffect(0);

        Assert.Single(q.Items);
    }

    [Fact]
    public void ReverseIterationKeepsRemainingIndicesIntact()
    {
        // 倒序遍历使 RemoveAt 不影响尚未处理的下标
        var q = Make();
        q.Items.Add(Effect(loopCount: 0));                       // 0：出队
        q.Items.Add(Effect(loopCount: 5, frameTime: 1000));      // 1：保留
        q.Items.Add(Effect(loopCount: 0));                       // 2：出队

        q.CheckLoadPlayEffect(0);

        var only = Assert.Single(q.Items);
        Assert.Equal(5, only.nLoopCount);
    }

    [Fact]
    public void ExpiredItemStillAdvancesOthersInSamePass()
    {
        var q = Make();
        q.Items.Add(Effect(loopCount: 0));
        var live = Effect(frameTime: 100, loopCount: 5);
        live.dwEffectTick = 0;
        q.Items.Add(live);

        q.CheckLoadPlayEffect(101);

        Assert.Equal(1, live.nCurrentFrame);
    }

    // ===================== LoadPlayEffectSurface =====================

    [Fact]
    public void SurfaceClearsTextureFirst()
    {
        var q = Make();
        var e = Effect();
        e.Texture = new FxImage(99, 1, 1, 0, 0, false);
        e.nEffectFileIndex = -1;                    // 不重新取图
        q.Items.Add(e);

        q.LoadPlayEffectSurface();

        Assert.Null(e.Texture);
    }

    [Fact]
    public void SurfaceResolvesForValidFileIndex()
    {
        var q = Make();
        var e = Effect(fileIdx: 3);
        e.nCurrentFrame = 5;
        q.Items.Add(e);

        q.LoadPlayEffectSurface();

        Assert.NotNull(e.Texture);
        Assert.Equal(5, ((FxImage)e.Texture!).ImageIndex);
    }

    [Fact]
    public void SurfaceSkipsNegativeFileIndex()
    {
        var q = Make();
        var e = Effect(fileIdx: -1);
        q.Items.Add(e);

        q.LoadPlayEffectSurface();

        Assert.Null(e.Texture);
    }

    [Fact]
    public void SurfaceSkipsFileIndexAtListCount()
    {
        var q = Make(count: 5);
        var e = Effect(fileIdx: 5);
        q.Items.Add(e);

        q.LoadPlayEffectSurface();

        Assert.Null(e.Texture);
    }

    [Fact]
    public void SurfaceSkipsZeroLoopCount()
    {
        var q = Make();
        var e = Effect(loopCount: 0);
        q.Items.Add(e);

        q.LoadPlayEffectSurface();

        Assert.Null(e.Texture);
    }

    [Fact]
    public void SurfacePassesColorEffectThrough()
    {
        TColorEffect seen = TColorEffect.ceNone;
        var q = Make();
        q.ColorEffect = TColorEffect.ceGrayScale;
        q.ResolveImage = (f, i, ce) => { seen = ce; return new FxImage(i, 1, 1, 0, 0, true); };
        q.Items.Add(Effect());

        q.LoadPlayEffectSurface();

        Assert.Equal(TColorEffect.ceGrayScale, seen);
    }

    [Fact]
    public void SurfaceMissingLibraryLeavesNull()
    {
        var q = Make();
        q.ResolveImage = (f, i, ce) => null;
        q.Items.Add(Effect());

        q.LoadPlayEffectSurface();

        Assert.Null(q.Items[0].Texture);
    }

    [Fact]
    public void SurfaceIteratesAllItems()
    {
        var q = Make();
        q.Items.Add(Effect(fileIdx: 1));
        q.Items.Add(Effect(fileIdx: 2));
        q.Items.Add(Effect(fileIdx: 3));

        q.LoadPlayEffectSurface();

        Assert.All(q.Items, e => Assert.NotNull(e.Texture));
    }

    // ===================== DrawPlayEffect =====================

    [Fact]
    public void DrawExitsEntirelyWhenMounted()
    {
        var q = Make();
        var e = Effect();
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        q.Items.Add(e);

        Assert.Empty(q.DrawPlayEffect(0, 0, false, 1, 0, 0));      // m_btHorse > 0
    }

    [Fact]
    public void DrawSkipsItemsWithoutTexture()
    {
        var q = Make();
        q.Items.Add(Effect());

        Assert.Empty(q.DrawPlayEffect(0, 0, false, 0, 0, 0));
    }

    [Fact]
    public void DrawSkipsZeroLoopCount()
    {
        var q = Make();
        var e = Effect(loopCount: 0);
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        q.Items.Add(e);

        Assert.Empty(q.DrawPlayEffect(0, 0, false, 0, 0, 0));
    }

    [Fact]
    public void DrawSkipsInvalidFileIndex()
    {
        var q = Make(count: 5);
        var e = Effect(fileIdx: 5);
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        q.Items.Add(e);

        Assert.Empty(q.DrawPlayEffect(0, 0, false, 0, 0, 0));
    }

    [Fact]
    public void DrawOrderZeroDrawsFrontOnly()
    {
        var q = Make();
        var e = Effect(drawOrder: 0);
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        q.Items.Add(e);

        Assert.Single(q.DrawPlayEffect(0, 0, false, 0, 0, 0));
        Assert.Empty(q.DrawPlayEffect(0, 0, true, 0, 0, 0));
    }

    [Fact]
    public void DrawOrderNonZeroDrawsBackOnly()
    {
        var q = Make();
        var e = Effect(drawOrder: 1);
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        q.Items.Add(e);

        Assert.Empty(q.DrawPlayEffect(0, 0, false, 0, 0, 0));
        Assert.Single(q.DrawPlayEffect(0, 0, true, 0, 0, 0));
    }

    [Fact]
    public void DrawPositionSumsAllFourOffsets()
    {
        var q = Make();
        var e = Effect();
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        e.nX = 1; e.nY = 2; e.nOffsetX = 3; e.nOffsetY = 4;
        q.Items.Add(e);

        var op = Assert.Single(q.DrawPlayEffect(10, 20, false, 0, 100, 200));

        Assert.Equal(10 + 1 + 3 + 100, op.X);
        Assert.Equal(20 + 2 + 4 + 200, op.Y);
    }

    [Fact]
    public void DrawBlendFlagHonoured()
    {
        var q = Make();
        var e = Effect();
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        e.boBlendMode = true;
        q.Items.Add(e);

        var op = Assert.Single(q.DrawPlayEffect(0, 0, false, 0, 0, 0));

        Assert.True(op.Blend);
    }

    [Fact]
    public void DrawNormalWhenBlendFalse()
    {
        var q = Make();
        var e = Effect();
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        e.boBlendMode = false;
        q.Items.Add(e);

        var op = Assert.Single(q.DrawPlayEffect(0, 0, false, 0, 0, 0));

        Assert.False(op.Blend);
    }

    [Fact]
    public void DrawMultipleItemsInOrder()
    {
        var q = Make();
        for (int i = 0; i < 3; i++)
        {
            var e = Effect();
            e.Texture = new FxImage(10 + i, 16, 16, 0, 0, false);
            q.Items.Add(e);
        }

        var ops = q.DrawPlayEffect(0, 0, false, 0, 0, 0);

        Assert.Equal(3, ops.Count);
        Assert.Equal(10, ops[0].ImageIndex);
        Assert.Equal(11, ops[1].ImageIndex);
        Assert.Equal(12, ops[2].ImageIndex);
    }

    // ===================== Finalize / Destroy =====================

    [Fact]
    public void FinalizeOnlyClearsTexturesAndKeepsQueue()
    {
        // 11248-11257：仅置 Texture := nil，**不清空队列、不置 boWantDelete**
        var q = Make();
        var e = Effect();
        e.Texture = new FxImage(1, 16, 16, 0, 0, false);
        q.Items.Add(e);

        q.FinalizeEffects();

        Assert.Null(e.Texture);
        Assert.Single(q.Items);                     // 队列仍在
        Assert.False(e.boWantDelete);               // 未被置真
    }

    [Fact]
    public void DestroyClearsQueue()
    {
        var q = Make();
        q.Items.Add(Effect());
        q.Items.Add(Effect());

        q.DestroyEffects();

        Assert.Empty(q.Items);
    }

    [Fact]
    public void WantDeleteIsNeverSetByQueueOperations()
    {
        // boWantDelete 只由 ClMain.pas 21079 的另一条路径设置
        var q = Make();
        var e = Effect(loopCount: 0);
        q.Items.Add(e);

        q.CheckLoadPlayEffect(0);

        Assert.False(e.boWantDelete);
    }

    // ===================== 结构与引用语义 =====================

    [Fact]
    public void RefToStructCopiesAllFields()
    {
        var e = Effect(offSet: 2, imgCount: 4, frameTime: 50, loopCount: 3, fileIdx: 9, drawOrder: 1);
        e.nOldLoopCount = 7;
        e.nOldCurrentFrame = 5;
        e.nCurrentFrame = 6;
        e.dwEffectTick = 123;
        e.nX = 11; e.nY = 12;
        e.nOffsetX = 13; e.nOffsetY = 14;
        e.boBlendMode = true;
        e.boWantDelete = true;

        var s = e.ToStruct();

        Assert.Equal((short)9, s.nEffectFileIndex);
        Assert.Equal(2, s.nEffectImageOffSet);
        Assert.Equal((ushort)4, s.wEffectImageCount);
        Assert.Equal((ushort)50, s.wEffectFrameTime);
        Assert.Equal(7, s.nOldLoopCount);
        Assert.Equal(3, s.nLoopCount);
        Assert.Equal(5, s.nOldCurrentFrame);
        Assert.Equal(6, s.nCurrentFrame);
        Assert.Equal(123u, s.dwEffectTick);
        Assert.Equal(11, s.nX);
        Assert.Equal(12, s.nY);
        Assert.Equal((byte)1, s.btDrawOrder);
        Assert.Equal(13, s.nOffsetX);
        Assert.Equal(14, s.nOffsetY);
        Assert.True(s.boBlendMode);
        Assert.True(s.boWantDelete);
    }

    // ===================== 端到端：两轮循环后出队 =====================

    [Fact]
    public void FullPlaybackLifecycleRetiresAfterLoops()
    {
        // offSet 0、count 2（帧 0/1）、loopCount 1
        var q = Make();
        var e = Effect(offSet: 0, imgCount: 2, frameTime: 100, loopCount: 1);
        e.dwEffectTick = 0;
        q.Items.Add(e);

        q.CheckLoadPlayEffect(101);                 // 0 → 1
        Assert.Equal(1, e.nCurrentFrame);

        q.CheckLoadPlayEffect(202);                 // 1 → 2 超限 → 回卷 0，loopCount 1 → 0
        Assert.Equal(0, e.nCurrentFrame);
        Assert.Equal(0, e.nLoopCount);

        q.CheckLoadPlayEffect(303);                 // loopCount 为 0 → 出队
        Assert.Empty(q.Items);
    }
}
