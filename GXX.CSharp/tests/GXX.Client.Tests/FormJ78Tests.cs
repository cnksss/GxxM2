using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J78：Actor.pas TActor.LoadFengHaoSurface（9563-9607）封号纹理装载 1:1 测试。
/// 重点是各守卫的**先后次序**——尤其是 9596-9597 的图案索引守卫位于文字纹理装载之后。
/// </summary>
public sealed class ActorFengHaoLoadTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorFengHaoLoadTests()
    {
        var saved = (TActorCore.CanvasReadyFn, TActorCore.CurrentFontAvailableFn,
            TActorCore.GetImageInfoFn, TActorCore.MyGetTickCountFn,
            TActorCore.ClientConfig_nTitleFileIndex,
            TActorCore.EffectImageListGetter, TActorCore.EffectImageListCountFn);

        _restore.Add(() =>
        {
            TActorCore.CanvasReadyFn = saved.Item1;
            TActorCore.CurrentFontAvailableFn = saved.Item2;
            TActorCore.GetImageInfoFn = saved.Item3;
            TActorCore.MyGetTickCountFn = saved.Item4;
            TActorCore.ClientConfig_nTitleFileIndex = saved.Item5;
            TActorCore.EffectImageListGetter = saved.Item6;
            TActorCore.EffectImageListCountFn = saved.Item7;
        });

        TActorCore.CanvasReadyFn = () => true;
        TActorCore.CurrentFontAvailableFn = () => true;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);
        TActorCore.MyGetTickCountFn = () => 1000;
        TActorCore.ClientConfig_nTitleFileIndex = -1;
        TActorCore.EffectImageListGetter = _ => null;
        TActorCore.EffectImageListCountFn = () => 0;
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    private sealed class FakeImages : IFengHaoImageSource
    {
        public int RequestedLooks = -1;
        public bool UsedGray;
        public LabelSurface? Result;

        public LabelSurface? GetCachedImage(int looks)
        {
            RequestedLooks = looks;
            UsedGray = false;
            return Result;
        }

        public LabelSurface? GetCachedGrayImage(int looks)
        {
            RequestedLooks = looks;
            UsedGray = true;
            return Result;
        }
    }

    // ===================== 无条件前置重置 =====================

    [Fact]
    public void LoadFengHaoSurface_ResetsStateEvenWhenCanvasNotReady()
    {
        TActorCore.CanvasReadyFn = () => false;

        var a = new TActor();
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));
        a.m_FengHaoEffectSurface = LabelSurface.Of(30, 30);
        a.m_nActiveFengHaoID = 77;
        a.m_nOldFengHaoSurfaceID = 0;

        a.LoadFengHaoSurface();

        // 9568-9570 在守卫之前
        Assert.Empty(a.FengHaoImageInfo);
        Assert.Null(a.m_FengHaoEffectSurface);
        Assert.Equal(77, a.m_nOldFengHaoSurfaceID);
    }

    [Fact]
    public void LoadFengHaoSurface_ClearsStaleImageInfoFromPreviousCall()
    {
        var a = new TActor();
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));

        a.LoadFengHaoSurface();

        Assert.Empty(a.FengHaoImageInfo);   // SetLength(..., 0)
    }

    // ===================== 骑马守卫（9575） =====================

    [Fact]
    public void LoadFengHaoSurface_HorseWithoutDoubleRiderExits()
    {
        var a = new TActor();
        a.m_btHorse = 1;
        a.m_btDoubleHumHorse = 0;
        a.m_sActiveFengHaoName = "称号";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        // 9575 的 Exit 早于 9589 的文字装载 → 文字纹理为空
        Assert.Empty(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_HorseHorse2AlsoExits()
    {
        var a = new TActor();
        a.m_btHorse = 2;
        a.m_btDoubleHumHorse = 0;
        a.m_sActiveFengHaoName = "称号";
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Empty(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_DoubleRiderBypassesHorseGuard()
    {
        var a = new TActor();
        a.m_btHorse = 1;
        a.m_btDoubleHumHorse = 1;                 // 被邀请人 → 放行
        a.m_sActiveFengHaoName = "称号";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_Horse0NeverTriggersHorseGuard()
    {
        var a = new TActor();
        a.m_btHorse = 0;
        a.m_sActiveFengHaoName = "称号";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_Horse3IsNotGuarded()
    {
        var a = new TActor();
        a.m_btHorse = 3;                          // 不在 [1,2]
        a.m_btDoubleHumHorse = 0;
        a.m_sActiveFengHaoName = "称号";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);
    }

    // ===================== Reserved = 2 守卫（9578） =====================

    [Fact]
    public void LoadFengHaoSurface_ReservedTwoExitsAndClearsSurface()
    {
        var a = new TActor();
        a.m_btActiveFengHaoReserved = 2;
        a.m_sActiveFengHaoName = "称号";
        a.m_FengHaoEffectSurface = LabelSurface.Of(30, 30);
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Null(a.m_FengHaoEffectSurface);
        Assert.Empty(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_ReservedOneDoesNotExitButBlocksTextLoad()
    {
        var a = new TActor();
        a.m_btActiveFengHaoReserved = 1;          // ≠ 2 → 不 Exit；但 9590 要求 = 0
        a.m_sActiveFengHaoName = "称号";
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Empty(a.FengHaoImageInfo);         // 文字被 Reserved <> 0 拦住
    }

    // ===================== 摆摊守卫（9584） =====================

    [Fact]
    public void LoadFengHaoSurface_ShopStallExitsAndClearsSurface()
    {
        var a = new TActor();
        a.m_boShopStall = true;
        a.m_sActiveFengHaoName = "称号";
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));
        TActorCore.GetImageInfoFn = _ => LabelSurface.Of(20, 10);

        a.LoadFengHaoSurface();

        Assert.Null(a.m_FengHaoEffectSurface);
        Assert.Empty(a.FengHaoImageInfo);
    }

    // ===================== 文字纹理装载（9589-9594） =====================

    [Fact]
    public void LoadFengHaoSurface_LoadsNameTextureWhenAllConditionsMet()
    {
        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);

        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);
        Assert.Equal("称号名".Length * 6, a.FengHaoImageInfo[0].Width);
    }

    [Fact]
    public void LoadFengHaoSurface_EmptyNameSkipsTextLoad()
    {
        var a = new TActor();
        a.m_sActiveFengHaoName = "";
        a.m_btActiveFengHaoReserved = 0;

        a.LoadFengHaoSurface();

        Assert.Empty(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_NoFontSkipsTextLoad()
    {
        TActorCore.CurrentFontAvailableFn = () => false;

        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;

        a.LoadFengHaoSurface();

        Assert.Empty(a.FengHaoImageInfo);
    }

    [Fact]
    public void LoadFengHaoSurface_NullImageInfoIsNotAppended()
    {
        TActorCore.GetImageInfoFn = _ => null;

        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;

        a.LoadFengHaoSurface();

        Assert.Empty(a.FengHaoImageInfo);
    }

    // ===================== 图案索引守卫（9596-9597） =====================

    [Fact]
    public void LoadFengHaoSurface_NegativeTitleFileIndexExitsButKeepsTextTexture()
    {
        TActorCore.ClientConfig_nTitleFileIndex = -1;

        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);

        a.LoadFengHaoSurface();

        // 9596 的 Exit 在 9589 之后 → 文字纹理已被装入并保留
        Assert.Single(a.FengHaoImageInfo);
        Assert.Null(a.m_FengHaoEffectSurface);
    }

    [Fact]
    public void LoadFengHaoSurface_TitleFileIndexOutOfRangeExitsButKeepsTextTexture()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 5;
        TActorCore.EffectImageListCountFn = () => 3;

        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);

        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);
        Assert.Null(a.m_FengHaoEffectSurface);
    }

    [Fact]
    public void LoadFengHaoSurface_IndexEqualToCountIsOutOfRange()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 3;
        TActorCore.EffectImageListCountFn = () => 3;   // >= Count → Exit

        var a = new TActor();
        var images = new FakeImages { Result = LabelSurface.Of(40, 40) };
        TActorCore.EffectImageListGetter = _ => images;

        a.LoadFengHaoSurface();

        Assert.Null(a.m_FengHaoEffectSurface);
        Assert.Equal(-1, images.RequestedLooks);        // 从未取图
    }

    // ===================== 图案装载（9599-9606） =====================

    [Fact]
    public void LoadFengHaoSurface_LoadsColorImageWhenAlive()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 0;
        TActorCore.EffectImageListCountFn = () => 1;

        var a = new TActor();
        a.m_boDeath = false;
        a.m_dwActiveFengHaoLooks = 42;

        var images = new FakeImages { Result = LabelSurface.Of(40, 40) };
        TActorCore.EffectImageListGetter = idx => idx == 0 ? images : null;

        a.LoadFengHaoSurface();

        Assert.NotNull(a.m_FengHaoEffectSurface);
        Assert.Equal(40, a.m_FengHaoEffectSurface!.Width);
        Assert.False(images.UsedGray);
        Assert.Equal(42, images.RequestedLooks);
    }

    [Fact]
    public void LoadFengHaoSurface_LoadsGrayImageWhenDead()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 0;
        TActorCore.EffectImageListCountFn = () => 1;

        var a = new TActor();
        a.m_boDeath = true;
        a.m_dwActiveFengHaoLooks = 7;

        var images = new FakeImages { Result = LabelSurface.Of(40, 40) };
        TActorCore.EffectImageListGetter = _ => images;

        a.LoadFengHaoSurface();

        Assert.True(images.UsedGray);
        Assert.Equal(7, images.RequestedLooks);
    }

    [Fact]
    public void LoadFengHaoSurface_StampsLoadTimeOnSuccess()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 0;
        TActorCore.EffectImageListCountFn = () => 1;
        TActorCore.MyGetTickCountFn = () => 6543;

        var a = new TActor();
        var images = new FakeImages { Result = LabelSurface.Of(40, 40) };
        TActorCore.EffectImageListGetter = _ => images;

        a.LoadFengHaoSurface();

        Assert.Equal(6543u, a.m_dwLoadFengHaoSurfaceTime);
    }

    [Fact]
    public void LoadFengHaoSurface_NullImageSourceDoesNotStamp()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 0;
        TActorCore.EffectImageListCountFn = () => 1;
        TActorCore.EffectImageListGetter = _ => null;   // Objects[idx] 为 nil

        var a = new TActor();
        a.m_dwLoadFengHaoSurfaceTime = 111;

        a.LoadFengHaoSurface();

        Assert.Null(a.m_FengHaoEffectSurface);
        Assert.Equal(111u, a.m_dwLoadFengHaoSurfaceTime);   // 未打点
    }

    [Fact]
    public void LoadFengHaoSurface_BothTextAndImageCoexist()
    {
        TActorCore.ClientConfig_nTitleFileIndex = 0;
        TActorCore.EffectImageListCountFn = () => 1;

        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);

        var images = new FakeImages { Result = LabelSurface.Of(40, 40) };
        TActorCore.EffectImageListGetter = _ => images;

        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);
        Assert.NotNull(a.m_FengHaoEffectSurface);
    }

    [Fact]
    public void LoadFengHaoSurface_RepeatedCallsDoNotAccumulateTextTextures()
    {
        var a = new TActor();
        a.m_sActiveFengHaoName = "称号名";
        a.m_btActiveFengHaoReserved = 0;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);

        a.LoadFengHaoSurface();
        a.LoadFengHaoSurface();
        a.LoadFengHaoSurface();

        Assert.Single(a.FengHaoImageInfo);   // 每次先 Clear
    }

    [Fact]
    public void LoadFengHaoSurface_OldSurfaceIdTracksActiveIdAcrossCalls()
    {
        var a = new TActor();
        a.m_nActiveFengHaoID = 10;
        a.LoadFengHaoSurface();
        Assert.Equal(10, a.m_nOldFengHaoSurfaceID);

        a.m_nActiveFengHaoID = 25;
        a.LoadFengHaoSurface();
        Assert.Equal(25, a.m_nOldFengHaoSurfaceID);
    }
}
