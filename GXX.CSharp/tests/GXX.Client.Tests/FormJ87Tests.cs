using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J87：THumActor.DrawChr 的嵌套过程 DrawSelfMagicEffect（Actor.pas 16503-16630）1:1 测试。
/// </summary>
public sealed class SelfMagicEffectRenderTests
{
    private static FxImage Img(int idx, int ox = 0, int oy = 0)
        => new(idx, 16, 16, ox, oy, false);

    /// <summary>一份可用于绘制的默认配置。</summary>
    private static TMagicClientConfig Cfg(
        short selfFile = 1,
        ushort startIndex = 100,
        byte syncHumAction = 0,
        ushort playCount = 4,
        ushort emptyCount = 0,
        ushort playTime = 100,
        TCustomDrawOrder drawOrder = TCustomDrawOrder.mdoPriorSelf,
        TCustomDrawMode drawMode = TCustomDrawMode.mdmNormal,
        TCustomDirCalcType dirCalcType = TCustomDirCalcType.mdctNone,
        byte lightRange = 7,
        byte playFailNoDraw = 0)
        => new()
        {
            Self_File = selfFile,
            Self_StartIndex = startIndex,
            Self_SyncHumAction = syncHumAction,
            Self_PlayCount = playCount,
            Self_EmptyCount = emptyCount,
            Self_PlayTime = playTime,
            Self_DrawOrder = drawOrder,
            Self_DrawMode = drawMode,
            Self_DirCalcType = dirCalcType,
            Self_LightRange = lightRange,
            Self_PlayFailNoDraw = playFailNoDraw,
        };

    private static SelfMagicEffectState St() => new()
    {
        m_boUseMagic = true,
        m_nCurrentFrame = 10,
        m_nStartFrame = 0,
        m_nEndFrame = 20,
        m_nCurSelfEffFrame = 0,
        m_CurMagic_targx = -1,
        m_CurMagic_targy = -1,
    };

    private static List<SelfMagicEffectDrawOp> Run(
        ref SelfMagicEffectState st,
        TMagicClientConfig? cfg,
        bool isWarrDraw = false, bool beforeDraw = false, bool boFlag = false,
        bool isMySelf = true, bool selfInvisible = false,
        int effectImageListCount = 100,
        int dir = 0, int mrDir = 0,
        uint now = 0,
        Func<int, int, bool, FxImage?>? resolve = null,
        bool configFound = true)
    {
        return SelfMagicEffectRender.Apply(ref st, configFound, cfg, 0, 0,
            isWarrDraw, beforeDraw, boFlag, isMySelf, selfInvisible,
            effectImageListCount, dir, mrDir, now,
            resolve ?? ((i, f, b) => Img(i)));
    }

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(10, SelfMagicEffectRender.PlayTimeTolerance);
        Assert.Equal(0x00800000, SelfMagicEffectRender.StateInvisible);
    }

    // ===================== MagicPlusLevel 分档 =====================

    [Theory]
    [InlineData(0, TMagicPlusLevel.mplNone)]
    [InlineData(1, TMagicPlusLevel.mpl1_3)]
    [InlineData(2, TMagicPlusLevel.mpl1_3)]
    [InlineData(3, TMagicPlusLevel.mpl1_3)]
    [InlineData(4, TMagicPlusLevel.mpl4_6)]
    [InlineData(6, TMagicPlusLevel.mpl4_6)]
    [InlineData(7, TMagicPlusLevel.mpl7_9)]
    [InlineData(9, TMagicPlusLevel.mpl7_9)]
    [InlineData(10, TMagicPlusLevel.mpl7_9)]     // else → mpl7_9
    [InlineData(-1, TMagicPlusLevel.mpl7_9)]     // 负数亦落 else
    public void MagicPlusLevelMapping(int level, TMagicPlusLevel expected)
    {
        Assert.Equal(expected, SelfMagicEffectRender.MagicPlusLevelOf(level));
    }

    // ===================== 门禁 =====================

    [Fact]
    public void NonWarrRequiresUseMagic()
    {
        var st = St();
        st.m_boUseMagic = false;

        Assert.Empty(Run(ref st, Cfg(), isWarrDraw: false));
    }

    [Fact]
    public void WarrBypassesUseMagic()
    {
        var st = St();
        st.m_boUseMagic = false;

        Assert.NotEmpty(Run(ref st, Cfg(), isWarrDraw: true));
    }

    [Fact]
    public void MissingCustomMagicConfigExits()
    {
        var st = St();
        Assert.Empty(Run(ref st, Cfg(), configFound: false));
    }

    [Fact]
    public void NullClientConfigExits()
    {
        var st = St();
        Assert.Empty(Run(ref st, null));
    }

    [Fact]
    public void MissingConfigDoesNotAdvanceFrame()
    {
        // 16539 的 Exit 在帧推进之前
        var st = St();
        st.m_nCurSelfEffFrame = 2;

        Run(ref st, null, now: 99999);

        Assert.Equal(2, st.m_nCurSelfEffFrame);
    }

    // ===================== 帧推进：SyncHumAction =====================

    [Fact]
    public void SyncHumActionCopiesFrameDifferenceUnconditionally()
    {
        var st = St();
        st.m_nCurrentFrame = 15;
        st.m_nStartFrame = 5;
        st.m_nEndFrame = 25;

        int pfc = SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(syncHumAction: 1), false, 0);

        Assert.Equal(10, st.m_nCurSelfEffFrame);     // 15 - 5
        Assert.Equal(20, pfc);                       // 25 - 5
    }

    [Fact]
    public void SyncHumActionIgnoresTick()
    {
        // 同步路径不看 tick，即使 tick 未到也照样跟随
        var st = St();
        st.m_nCurSelfEffFrame = 99;
        st.m_dwCurSelfEffFrameTick = 5000;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(syncHumAction: 1, playTime: 100), false, 5000);

        Assert.Equal(10, st.m_nCurSelfEffFrame);     // 10 - 0
    }

    [Fact]
    public void SyncHumActionCanGoNegative()
    {
        // 该赋值无下限保护（原文如此）
        var st = St();
        st.m_nCurrentFrame = 3;
        st.m_nStartFrame = 10;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(syncHumAction: 1), false, 0);

        Assert.Equal(-7, st.m_nCurSelfEffFrame);
    }

    [Fact]
    public void SyncHumActionPlayFrameCountZeroExits()
    {
        var st = St();
        st.m_nStartFrame = 5;
        st.m_nEndFrame = 5;                          // 差为 0

        Assert.Empty(Run(ref st, Cfg(syncHumAction: 1)));
    }

    // ===================== 帧推进：自定义时间 =====================

    [Fact]
    public void PlayFrameCountIsSelfPlayCountWhenNotSync()
    {
        var st = St();
        int pfc = SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playCount: 9), false, 0);
        Assert.Equal(9, pfc);
    }

    [Fact]
    public void FrameAdvancesAtPlayTimeMinusTolerance()
    {
        // 判据是 `>= Self_PlayTime - 10`
        var st = St();
        st.m_dwCurSelfEffFrameTick = 0;
        st.m_nCurSelfEffFrame = 1;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playTime: 100), false, 90);

        Assert.Equal(2, st.m_nCurSelfEffFrame);
    }

    [Fact]
    public void FrameDoesNotAdvanceBelowThreshold()
    {
        var st = St();
        st.m_dwCurSelfEffFrameTick = 0;
        st.m_nCurSelfEffFrame = 1;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playTime: 100), false, 89);

        Assert.Equal(1, st.m_nCurSelfEffFrame);
    }

    [Fact]
    public void TickRefreshedOnlyWhenThresholdMet()
    {
        var st = St();
        st.m_dwCurSelfEffFrameTick = 0;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playTime: 100), false, 50);
        Assert.Equal(0u, st.m_dwCurSelfEffFrameTick);   // 未达闸值不刷新

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playTime: 100), false, 90);
        Assert.Equal(90u, st.m_dwCurSelfEffFrameTick);
    }

    [Fact]
    public void FrameStopsAtPlayCountBoundary()
    {
        // 16550：`m_nCurSelfEffFrame < Self_PlayCount - 0`，等于时不再递增
        var st = St();
        st.m_nCurSelfEffFrame = 4;                      // == PlayCount
        st.m_dwCurSelfEffFrameTick = 0;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playCount: 4, playTime: 100), false, 1000);

        Assert.Equal(4, st.m_nCurSelfEffFrame);
    }

    [Fact]
    public void TickStillRefreshesWhenFramePinned()
    {
        // 16552 在 if 之外，故帧被钉住时 tick 仍刷新
        var st = St();
        st.m_nCurSelfEffFrame = 4;
        st.m_dwCurSelfEffFrameTick = 0;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playCount: 4, playTime: 100), false, 1000);

        Assert.Equal(1000u, st.m_dwCurSelfEffFrameTick);
    }

    [Fact]
    public void WarrAndNonWarrTakeIdenticalAdvancePath()
    {
        // 16548 与 16556 两分支代码完全相同
        var a = St(); a.m_dwCurSelfEffFrameTick = 0; a.m_nCurSelfEffFrame = 1;
        var b = St(); b.m_dwCurSelfEffFrameTick = 0; b.m_nCurSelfEffFrame = 1;

        SelfMagicEffectRender.AdvanceFrame(ref a, Cfg(playTime: 100), false, 90);
        SelfMagicEffectRender.AdvanceFrame(ref b, Cfg(playTime: 100), true, 90);

        Assert.Equal(a.m_nCurSelfEffFrame, b.m_nCurSelfEffFrame);
        Assert.Equal(a.m_dwCurSelfEffFrameTick, b.m_dwCurSelfEffFrameTick);
    }

    [Fact]
    public void TickComparisonIsUnsignedWrapped()
    {
        // Cardinal 相减：tick 大于 now 时回绕成极大值 → 仍满足闸值
        var st = St();
        st.m_dwCurSelfEffFrameTick = 1000;
        st.m_nCurSelfEffFrame = 0;

        SelfMagicEffectRender.AdvanceFrame(ref st, Cfg(playCount: 10, playTime: 100), false, 100);

        Assert.Equal(1, st.m_nCurSelfEffFrame);
    }

    // ===================== IsDraw 判定 =====================

    [Fact]
    public void IsDrawRejectsNegativeSelfFile()
    {
        Assert.False(SelfMagicEffectRender.IsDraw(true, Cfg(selfFile: -1), 100, false, false, false));
    }

    [Fact]
    public void IsDrawRejectsSelfFileAtListCount()
    {
        Assert.False(SelfMagicEffectRender.IsDraw(true, Cfg(selfFile: 5), 5, false, false, false));
    }

    [Fact]
    public void IsDrawRejectsDirCalcTypeCenter()
    {
        Assert.False(SelfMagicEffectRender.IsDraw(true,
            Cfg(dirCalcType: TCustomDirCalcType.mdctCenter), 100, false, false, false));
    }

    [Fact]
    public void MySelfPriorMagicRequiresBeforeDrawAndBoFlag()
    {
        var cfg = Cfg(drawOrder: TCustomDrawOrder.mdoPriorMagic);

        Assert.True(SelfMagicEffectRender.IsDraw(true, cfg, 100, true, true, false));
        Assert.False(SelfMagicEffectRender.IsDraw(true, cfg, 100, true, false, false));   // boFlag 假
        Assert.False(SelfMagicEffectRender.IsDraw(true, cfg, 100, false, true, false));   // BeforeDraw 假
    }

    [Fact]
    public void MySelfPriorSelfRequiresNotBeforeDraw()
    {
        var cfg = Cfg(drawOrder: TCustomDrawOrder.mdoPriorSelf);

        Assert.True(SelfMagicEffectRender.IsDraw(true, cfg, 100, false, false, false));
        Assert.False(SelfMagicEffectRender.IsDraw(true, cfg, 100, true, false, false));
    }

    [Fact]
    public void MySelfPriorSelfAllowsBoFlagWhenInvisible()
    {
        // 16574：`(not boFlag) or (m_nState and $00800000 <> 0)`
        var cfg = Cfg(drawOrder: TCustomDrawOrder.mdoPriorSelf);

        Assert.True(SelfMagicEffectRender.IsDraw(true, cfg, 100, false, true, true));    // 隐身放行
        Assert.False(SelfMagicEffectRender.IsDraw(true, cfg, 100, false, true, false));  // 非隐身拒绝
    }

    [Fact]
    public void OtherActorPriorMagicIgnoresBoFlag()
    {
        // 旁人分支不看 boFlag
        var cfg = Cfg(drawOrder: TCustomDrawOrder.mdoPriorMagic);

        Assert.True(SelfMagicEffectRender.IsDraw(false, cfg, 100, true, false, false));
        Assert.True(SelfMagicEffectRender.IsDraw(false, cfg, 100, true, true, false));
    }

    [Fact]
    public void OtherActorPriorSelfIgnoresBoFlagAndInvisible()
    {
        var cfg = Cfg(drawOrder: TCustomDrawOrder.mdoPriorSelf);

        Assert.True(SelfMagicEffectRender.IsDraw(false, cfg, 100, false, true, false));
        Assert.False(SelfMagicEffectRender.IsDraw(false, cfg, 100, true, false, false));
    }

    // ===================== 图号计算 =====================

    [Fact]
    public void ImageIndexMdctNoneIgnoresDir()
    {
        var cfg = Cfg(startIndex: 100, dirCalcType: TCustomDirCalcType.mdctNone);
        Assert.Equal(103, SelfMagicEffectRender.ImageIndex(cfg, 5, 3));
    }

    [Fact]
    public void ImageIndexMdctNormalStridesByDir()
    {
        var cfg = Cfg(startIndex: 100, playCount: 4, emptyCount: 2,
            dirCalcType: TCustomDirCalcType.mdctNormal);

        // 100 + 2 * (4 + 2) + 1
        Assert.Equal(113, SelfMagicEffectRender.ImageIndex(cfg, 2, 1));
    }

    [Fact]
    public void ImageIndexMdctCenterReturnsMinusOne()
    {
        var cfg = Cfg(dirCalcType: TCustomDirCalcType.mdctCenter);
        Assert.Equal(-1, SelfMagicEffectRender.ImageIndex(cfg, 0, 0));
    }

    [Fact]
    public void MdctNormalUsesEmptyCountForStride()
    {
        var cfg = Cfg(startIndex: 0, playCount: 4, emptyCount: 6,
            dirCalcType: TCustomDirCalcType.mdctNormal);

        Assert.Equal(10, SelfMagicEffectRender.ImageIndex(cfg, 1, 0));   // 1 * (4 + 6)
    }

    // ===================== 方向计算 =====================

    [Fact]
    public void BothTargetsMinusOneUsesOwnDir()
    {
        var st = St();
        st.m_CurMagic_targx = -1;
        st.m_CurMagic_targy = -1;

        int asked = -1;
        Run(ref st, Cfg(dirCalcType: TCustomDirCalcType.mdctNormal), dir: 3, mrDir: 6,
            resolve: (i, f, b) => { asked = i; return Img(i); });

        // dir 3 → 100 + 3 * (4 + 0) + 0 = 112
        Assert.Equal(112, asked);
    }

    [Fact]
    public void OnlyOneTargetMinusOneDoesNotShortCircuit()
    {
        // 条件是 `targx = -1 and targy = -1` 二者同时成立
        var st = St();
        st.m_CurMagic_targx = -1;
        st.m_CurMagic_targy = 5;

        int asked = -1;
        Run(ref st, Cfg(dirCalcType: TCustomDirCalcType.mdctNormal), dir: 3, mrDir: 6,
            resolve: (i, f, b) => { asked = i; return Img(i); });

        // 走 mrDir 6 → 100 + 6 * 4 = 124
        Assert.Equal(124, asked);
    }

    [Fact]
    public void NonSelfUsesOwnDir()
    {
        var st = St();
        st.m_CurMagic_targx = 10;
        st.m_CurMagic_targy = 10;

        int asked = -1;
        Run(ref st, Cfg(dirCalcType: TCustomDirCalcType.mdctNormal), isMySelf: false,
            dir: 3, mrDir: 6, resolve: (i, f, b) => { asked = i; return Img(i); });

        Assert.Equal(112, asked);
    }

    [Fact]
    public void NoChangeDirUsesOwnDir()
    {
        var st = St();
        st.m_CurMagic_targx = 10;
        st.m_CurMagic_targy = 10;
        st.CustomMagicConfigNoChangeDir = true;

        int asked = -1;
        Run(ref st, Cfg(dirCalcType: TCustomDirCalcType.mdctNormal), dir: 3, mrDir: 6,
            resolve: (i, f, b) => { asked = i; return Img(i); });

        Assert.Equal(112, asked);
    }

    // ===================== 绘制 =====================

    [Fact]
    public void DrawUsesShiftAndOrigin()
    {
        var st = St();
        st.m_nShiftX = 5;
        st.m_nShiftY = 7;

        var ops = Run(ref st, Cfg(startIndex: 100),
            resolve: (i, f, b) => Img(i, 2, 3));

        var op = Assert.Single(ops);
        Assert.Equal(5 + 2, op.X);
        Assert.Equal(7 + 3, op.Y);
    }

    [Fact]
    public void BlendFollowsDrawMode()
    {
        var st = St();
        var ops = Run(ref st, Cfg(drawMode: TCustomDrawMode.mdmBlend));
        Assert.True(Assert.Single(ops).Blend);

        var st2 = St();
        var ops2 = Run(ref st2, Cfg(drawMode: TCustomDrawMode.mdmNormal));
        Assert.False(Assert.Single(ops2).Blend);
    }

    [Fact]
    public void MissingTextureNoOp()
    {
        var st = St();
        Assert.Empty(Run(ref st, Cfg(), resolve: (i, f, b) => null));
    }

    // ===================== m_nMagLight 写回 =====================

    [Fact]
    public void MagLightWrittenEvenWhenNotDrawn()
    {
        // 16583 在 `if IsDraw` 之外
        var st = St();
        var cfg = Cfg(lightRange: 9, drawOrder: TCustomDrawOrder.mdoPriorMagic);

        // beforeDraw = false → IsDraw 为假
        Run(ref st, cfg, beforeDraw: false, boFlag: false);

        Assert.Equal(9, st.m_nMagLight);
    }

    [Fact]
    public void MagLightWrittenWhenDrawn()
    {
        var st = St();
        Run(ref st, Cfg(lightRange: 12));
        Assert.Equal(12, st.m_nMagLight);
    }

    [Fact]
    public void MagLightNotWrittenWhenConfigMissing()
    {
        var st = St();
        st.m_nMagLight = 3;
        Run(ref st, null);
        Assert.Equal(3, st.m_nMagLight);             // 配置缺失时提前退出
    }

    // ===================== 16588-16590 的优先级陷阱 =====================

    [Fact]
    public void FrameRangeGuardBypassedWhenServerMagicCodePositive()
    {
        // `and` 优先于 `or`：ServerMagicCode > 0 时越过帧范围检查
        var st = St();
        st.m_nCurSelfEffFrame = 99;                  // 远超 [0..3]
        st.m_CurMagic_ServerMagicCode = 5;

        var ops = Run(ref st, Cfg(playCount: 4));

        Assert.NotEmpty(ops);                        // 仍绘制
    }

    [Fact]
    public void FrameRangeGuardBypassedWhenWarrDraw()
    {
        var st = St();
        st.m_nCurSelfEffFrame = 99;

        var ops = Run(ref st, Cfg(playCount: 4), isWarrDraw: true);

        Assert.NotEmpty(ops);
    }

    [Fact]
    public void FrameRangeGuardEnforcedWhenNeitherExemptionApplies()
    {
        var st = St();
        st.m_nCurSelfEffFrame = 99;
        st.m_CurMagic_ServerMagicCode = 0;

        Assert.Empty(Run(ref st, Cfg(playCount: 4), isWarrDraw: false));
    }

    [Fact]
    public void PlayFailNoDrawSuppressesWithinRange()
    {
        var st = St();
        var cfg = Cfg(playFailNoDraw: 1);

        Assert.Empty(Run(ref st, cfg, isWarrDraw: false));
    }

    [Fact]
    public void PlayFailNoDrawBypassedByServerMagicCode()
    {
        var st = St();
        st.m_CurMagic_ServerMagicCode = 1;

        Assert.NotEmpty(Run(ref st, Cfg(playFailNoDraw: 1)));
    }

    [Fact]
    public void PlayFailNoDrawBypassedByWarrDraw()
    {
        var st = St();
        Assert.NotEmpty(Run(ref st, Cfg(playFailNoDraw: 1), isWarrDraw: true));
    }

    [Fact]
    public void NegativeFrameFailsRangeGuard()
    {
        var st = St();
        st.m_nCurSelfEffFrame = -1;

        Assert.Empty(Run(ref st, Cfg(playCount: 4)));
    }

    // ===================== 端到端 =====================

    [Fact]
    public void EndToEndAdvanceThenDraw()
    {
        var st = St();
        st.m_dwCurSelfEffFrameTick = 0;

        var ops = Run(ref st, Cfg(startIndex: 200, playCount: 4, playTime: 100), now: 90);

        Assert.Equal(1, st.m_nCurSelfEffFrame);
        Assert.Equal(201, Assert.Single(ops).ImageIndex);
    }
}
