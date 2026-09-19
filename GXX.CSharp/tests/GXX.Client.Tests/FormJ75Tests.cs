using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J75：Actor.pas 名字/喊话/称号/数字显血排版层（ActorLabelRender.cs）1:1 行为锁定。
/// 覆盖 ShowSay(9400-9478) / ShowName(9609-9724) / ShowShopName(9726-9789) /
/// ShowNumberLable(9793-9845) 的几何、配色与全部守卫分支。
/// </summary>
public sealed class ActorLabelRenderTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorLabelRenderTests()
    {
        var saved = (TActorCore.LabelConfig, TActorCore.NewopUI170Texture0,
            TActorCore.ShopNameBackgroundImage, TActorCore.CurrentFontAvailableFn,
            TActorCore.GetRgbFn, new HashSet<string>(TActorCore.MyTargetList, StringComparer.Ordinal));

        _restore.Add(() =>
        {
            TActorCore.LabelConfig = saved.Item1;
            TActorCore.NewopUI170Texture0 = saved.Item2;
            TActorCore.ShopNameBackgroundImage = saved.Item3;
            TActorCore.CurrentFontAvailableFn = saved.Item4;
            TActorCore.GetRgbFn = saved.Item5;
            TActorCore.MyTargetList.Clear();
            foreach (var n in saved.Item6)
                TActorCore.MyTargetList.Add(n);
        });

        TActorCore.LabelConfig = new ActorLabelConfig();
        TActorCore.NewopUI170Texture0 = null;
        TActorCore.ShopNameBackgroundImage = null;
        TActorCore.CurrentFontAvailableFn = () => true;
        TActorCore.GetRgbFn = c => c;
        TActorCore.MyTargetList.Clear();
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    private static TActor MakeActor(int race = ActorLabelConsts.RC_PLAYOBJECT, int sayX = 100, int sayY = 200)
    {
        var a = new TActor { m_btRace = (byte)race };
        a.m_nSayX = sayX;
        a.m_nSayY = sayY;
        a.m_boCanDraw = true;
        return a;
    }

    // ===================== ShowSay =====================

    [Fact]
    public void PlanShowSay_EmptyTextProducesNothing()
    {
        var a = MakeActor();
        Assert.Empty(a.PlanShowSay(1000));

        // 有纹理行但首行文本为空 → 原文条件 `m_SayingArr[0].Text <> ''` 不成立
        a.SayingText.Add("");
        a.SayingArr.Add(LabelSurface.Of(20, 10));
        Assert.Empty(a.PlanShowSay(1000));
    }

    [Fact]
    public void PlanShowSay_WithinFourSecondsDrawsOutlinePlusColor()
    {
        var a = MakeActor(race: ActorLabelConsts.RC_PLAYOBJECT, sayX: 100, sayY: 200);
        a.SayingText.Add("你好");
        a.SayingArr.Add(LabelSurface.Of(40, 12));
        a.m_nSayLineCount = 1;
        a.m_HearMsgColor = 0x123456;
        a.m_dwSayTime = 1000;

        // NewopUI170Texture0 = null、无勾选 → HPOffsetY = 2
        var ops = a.PlanShowSay(2000);

        Assert.Equal(5, ops.Count);
        // x = 100 - 40/2 + 0 = 80 ; y = 200 - 2 + 0 - (1*16) + 0 = 182
        Assert.Equal(79, ops[0].X); Assert.Equal(182, ops[0].Y); Assert.Equal(TActorCore.clBlack, ops[0].Color);
        Assert.Equal(81, ops[1].X); Assert.Equal(182, ops[1].Y);
        Assert.Equal(80, ops[2].X); Assert.Equal(181, ops[2].Y);
        Assert.Equal(80, ops[3].X); Assert.Equal(183, ops[3].Y);
        Assert.Equal(80, ops[4].X); Assert.Equal(182, ops[4].Y);
        Assert.Equal(0x123456, ops[4].Color);
        Assert.Equal("SayText", ops[4].Kind);
    }

    [Fact]
    public void PlanShowSay_DeathUsesGrayColor()
    {
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_HearMsgColor = 0x123456;
        a.m_boDeath = true;
        a.m_dwSayTime = 1000;

        var ops = a.PlanShowSay(2000);
        Assert.Equal(TActorCore.clGray, ops[4].Color);
    }

    [Fact]
    public void PlanShowSay_ExpiredClearsOnlyFirstLine()
    {
        var a = MakeActor();
        a.SayingText.Add("第一行");
        a.SayingText.Add("第二行");
        a.SayingArr.Add(LabelSurface.Of(40, 12));
        a.SayingArr.Add(LabelSurface.Of(40, 12));
        a.m_nSayLineCount = 2;
        a.m_dwSayTime = 1000;

        var ops = a.PlanShowSay(1000 + 4 * 1000 + 1);   // 超出 4 秒

        Assert.Empty(ops);
        Assert.Equal("", a.SayingText[0]);
        Assert.Equal(0, a.SayingArr[0].Width);
        Assert.Equal(0, a.SayingArr[0].Height);
        Assert.False(a.SayingArr[0].HasImage);
        // 原文只清 [0]，第二行保留
        Assert.Equal("第二行", a.SayingText[1]);
        Assert.True(a.SayingArr[1].HasImage);
    }

    [Fact]
    public void PlanShowSay_ExactlyFourSecondsStillDraws()
    {
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;

        // 原文 `TimeGetTime - m_dwSayTime < 4 * 1000` → 恰好等于时不进 else
        var ops = a.PlanShowSay(1000 + 4 * 1000 - 1);
        Assert.Equal(5, ops.Count);
    }

    [Fact]
    public void PlanShowSay_NotCanDrawSkipsWithoutClearing()
    {
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;
        a.m_boCanDraw = false;

        var ops = a.PlanShowSay(2000);

        Assert.Empty(ops);
        Assert.Equal("x", a.SayingText[0]);             // 未超时 → 不清
    }

    [Fact]
    public void PlanShowSay_NewopUI170AffectsHpOffsetY()
    {
        TActorCore.NewopUI170Texture0 = LabelSurface.Of(32, 3);

        // ckShowNumberLable 或 ckShowJobAndLevel → 3*2+14 = 20
        TActorCore.LabelConfig.ckShowNumberLable = true;
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;
        var ops = a.PlanShowSay(2000);
        Assert.Equal(200 - 20 - 16, ops[4].Y);

        // 仅 ckShowHPLabel → 3*2+2 = 8
        TActorCore.LabelConfig.ckShowNumberLable = false;
        TActorCore.LabelConfig.ckShowJobAndLevel = false;
        TActorCore.LabelConfig.ckShowHPLabel = true;
        a.SayingArr[0].HasImage = true;
        a.SayingText[0] = "x";
        ops = a.PlanShowSay(2000);
        Assert.Equal(200 - 8 - 16, ops[4].Y);

        // 都未勾 → 2
        TActorCore.LabelConfig.ckShowHPLabel = false;
        a.SayingArr[0].HasImage = true;
        a.SayingText[0] = "x";
        ops = a.PlanShowSay(2000);
        Assert.Equal(200 - 2 - 16, ops[4].Y);
    }

    [Fact]
    public void PlanShowSay_NoTextureUsesLegacyOffsetsIgnoringJobAndLevel()
    {
        TActorCore.NewopUI170Texture0 = null;

        // 原文无纹理分支只判 ckShowNumberLable：仅 ckShowJobAndLevel 勾选时仍走 else → 2
        TActorCore.LabelConfig.ckShowJobAndLevel = true;
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;

        var ops = a.PlanShowSay(2000);
        Assert.Equal(200 - 2 - 16, ops[4].Y);

        // ckShowNumberLable → 18
        TActorCore.LabelConfig.ckShowNumberLable = true;
        a.SayingArr[0].HasImage = true;
        ops = a.PlanShowSay(2000);
        Assert.Equal(200 - 18 - 16, ops[4].Y);
    }

    [Fact]
    public void PlanShowSay_FengHaoPushesUpEighteen()
    {
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;
        a.m_FengHaoEffectSurface = LabelSurface.Of(20, 20);

        var ops = a.PlanShowSay(2000);
        Assert.Equal(200 - (2 + 18) - 16, ops[4].Y);
    }

    [Fact]
    public void PlanShowSay_FengHaoTextAlsoPushesUp()
    {
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));   // 仅文字封号

        var ops = a.PlanShowSay(2000);
        Assert.Equal(200 - (2 + 18) - 16, ops[4].Y);
    }

    [Fact]
    public void PlanShowSay_NoFontSkipsFengHaoExtra()
    {
        TActorCore.CurrentFontAvailableFn = () => false;
        var a = MakeActor();
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;
        a.m_FengHaoEffectSurface = LabelSurface.Of(20, 20);

        // 无字体 → 不加 18，且循环内 DrawSay 因 CurrentFont=nil 不产出
        var ops = a.PlanShowSay(2000);
        Assert.Empty(ops);
    }

    [Fact]
    public void PlanShowSay_MultipleLinesStackByFourteen()
    {
        var a = MakeActor(sayY: 200);
        a.SayingText.Add("A");
        a.SayingText.Add("B");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 2;
        a.m_dwSayTime = 1000;

        var ops = a.PlanShowSay(2000);

        Assert.Equal(10, ops.Count);
        int y0 = ops[4].Y;
        int y1 = ops[9].Y;
        Assert.Equal(14, y1 - y0);
        // 首行 = 200 - 2 - 2*16 + 0 = 166
        Assert.Equal(200 - 2 - 32, y0);
    }

    [Fact]
    public void PlanShowSay_SkipsLinesWithoutImage()
    {
        var a = MakeActor();
        a.SayingText.Add("A");
        a.SayingText.Add("B");
        a.SayingArr.Add(LabelSurface.Of(10, 10, hasImage: false));
        a.SayingArr.Add(LabelSurface.Of(10, 10, hasImage: true));
        a.m_nSayLineCount = 2;
        a.m_dwSayTime = 1000;

        var ops = a.PlanShowSay(2000);

        Assert.Equal(5, ops.Count);                     // 只有第二行绘制
    }

    [Fact]
    public void PlanShowSay_RaceOffsetsApplied()
    {
        TActorCore.LabelConfig.nHumHPBarOffsetX = 5;
        TActorCore.LabelConfig.nNpcHPBarOffsetX = 7;
        TActorCore.LabelConfig.nMonHPBarOffsetX = 9;

        var a = MakeActor(race: ActorLabelConsts.RC_MERCHANT);
        a.SayingText.Add("x");
        a.SayingArr.Add(LabelSurface.Of(10, 10));
        a.m_nSayLineCount = 1;
        a.m_dwSayTime = 1000;

        // x = 100 - 5 + 7 = 102
        var ops = a.PlanShowSay(2000);
        Assert.Equal(102 - 1, ops[0].X);
    }

    // ===================== HpBarOffset / NameOffsetPair =====================

    [Fact]
    public void HpBarAndNameOffsetsSelectByRace()
    {
        var cfg = TActorCore.LabelConfig;
        cfg.nHumHPBarOffsetX = 1; cfg.nHumHPBarOffsetY = 2;
        cfg.nNpcHPBarOffsetX = 3; cfg.nNpcHPBarOffsetY = 4;
        cfg.nMonHPBarOffsetX = 5; cfg.nMonHPBarOffsetY = 6;
        cfg.nHumNameOffsetX = 11; cfg.nHumNameOffsetY = 12;
        cfg.nNpcNameOffsetX = 13; cfg.nNpcNameOffsetY = 14;
        cfg.nMonNameOffsetX = 15; cfg.nMonNameOffsetY = 16;

        var hum = MakeActor(race: ActorLabelConsts.RC_PLAYOBJECT);
        Assert.Equal((1, 2), hum.HpBarOffset());
        Assert.Equal((11, 12), hum.NameOffsetPair());

        var hero = MakeActor(race: ActorLabelConsts.RC_HEROOBJECT);
        Assert.Equal((1, 2), hero.HpBarOffset());
        Assert.Equal((11, 12), hero.NameOffsetPair());

        var npc = MakeActor(race: ActorLabelConsts.RC_MERCHANT);
        Assert.Equal((3, 4), npc.HpBarOffset());
        Assert.Equal((13, 14), npc.NameOffsetPair());

        var mon = MakeActor(race: 80);
        Assert.Equal((5, 6), mon.HpBarOffset());
        Assert.Equal((15, 16), mon.NameOffsetPair());
    }

    // ===================== ShowName =====================

    [Fact]
    public void NameBaseY_WithTextureThreeBranches()
    {
        TActorCore.NewopUI170Texture0 = LabelSurface.Of(32, 3);
        var a = MakeActor(sayY: 200);
        var cfg = TActorCore.LabelConfig;

        cfg.ckShowNumberLable = true;
        Assert.Equal(200 - 3 * 2 - 32, a.NameBaseY());

        cfg.ckShowNumberLable = false;
        cfg.ckShowJobAndLevel = true;
        Assert.Equal(200 - 3 * 2 - 32, a.NameBaseY());

        cfg.ckShowJobAndLevel = false;
        cfg.ckShowHPLabel = true;
        Assert.Equal(200 - 3 * 2 - 20, a.NameBaseY());

        cfg.ckShowHPLabel = false;
        Assert.Equal(200 - 19, a.NameBaseY());
    }

    [Fact]
    public void NameBaseY_WithoutTextureThreeBranches()
    {
        TActorCore.NewopUI170Texture0 = null;
        var a = MakeActor(sayY: 200);
        var cfg = TActorCore.LabelConfig;

        cfg.ckShowNumberLable = true;
        Assert.Equal(200 - 38, a.NameBaseY());

        cfg.ckShowNumberLable = false;
        cfg.ckShowHPLabel = true;
        Assert.Equal(200 - 26, a.NameBaseY());

        cfg.ckShowHPLabel = false;
        Assert.Equal(200 - 19, a.NameBaseY());
    }

    [Fact]
    public void PlanShowName_NoNameSurfaceDrawsNothing()
    {
        var a = MakeActor();
        Assert.Empty(a.PlanShowName());

        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_boCanDraw = false;
        Assert.Empty(a.PlanShowName());                 // m_boCanDraw 守卫
    }

    [Fact]
    public void PlanShowName_HorseShiftsNameDownByFifty()
    {
        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_nNameColor = 0xABCDEF;
        a.m_btHorse = 1;                                // 骑乘 → +50

        var ops = a.PlanShowName();

        Assert.Equal(5, ops.Count);
        // nY = (200 + 50) + 0 = 250 ; nX = 100 ; half = 20
        Assert.Equal(99 - 20, ops[0].X); Assert.Equal(250, ops[0].Y);
        Assert.Equal(101 - 20, ops[1].X);
        Assert.Equal(100 - 20, ops[2].X); Assert.Equal(249, ops[2].Y);
        Assert.Equal(100 - 20, ops[3].X); Assert.Equal(251, ops[3].Y);
        Assert.Equal(80, ops[4].X); Assert.Equal(250, ops[4].Y);
        Assert.Equal(0xABCDEF, ops[4].Color);
    }

    [Fact]
    public void PlanShowName_NoHorseShiftsDownByThirtyOnly()
    {
        var a = MakeActor(sayY: 200);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_btHorse = 0;

        var ops = a.PlanShowName();
        Assert.Equal(230, ops[4].Y);
    }

    [Fact]
    public void PlanShowName_StatuaryNpcUsesMerchant273Color()
    {
        TActorCore.LabelConfig.btMerchant273NameColor = 273;
        TActorCore.GetRgbFn = c => c * 2;

        var a = MakeActor(race: ActorLabelConsts.RC_MERCHANT);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_nNameColor = 0x111111;
        a.IsStatuaryNpcActor = true;

        var ops = a.PlanShowName();
        Assert.Equal(546, ops[4].Color);
    }

    [Fact]
    public void PlanShowName_HumActorInTargetListUsesLime()
    {
        TActorCore.MyTargetList.Add("敌方甲");

        var a = MakeActor();
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_nNameColor = 0x111111;
        a.IsHumActor = true;
        a.m_sUserName = "敌方甲";

        var ops = a.PlanShowName();
        Assert.Equal(TActorCore.clLime, ops[4].Color);
    }

    [Fact]
    public void PlanShowName_HumActorNotInTargetListKeepsNameColor()
    {
        var a = MakeActor();
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_nNameColor = 0x111111;
        a.IsHumActor = true;
        a.m_sUserName = "路人";

        var ops = a.PlanShowName();
        Assert.Equal(0x111111, ops[4].Color);
    }

    [Fact]
    public void PlanShowName_FengHaoEffectOnlyDrawsEffectAtCenteredX()
    {
        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_FengHaoEffectSurface = LabelSurface.Of(30, 12);

        var ops = a.PlanShowName();

        // 封号特效：x = 100 - 30/2 + 0 = 85；y = (200-19) + 16 - 12 + 0 = 185
        var fh = ops[0];
        Assert.Equal("FengHaoEffect", fh.Kind);
        Assert.Equal(85, fh.X);
        Assert.Equal(185, fh.Y);

        // 人名紧随其后
        Assert.Equal("NameText", ops[^1].Kind);
    }

    [Fact]
    public void PlanShowName_FengHaoEffectPlusTextLaysOutSideBySide()
    {
        TActorCore.LabelConfig.CurrentFontHeight = 12;

        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_FengHaoEffectSurface = LabelSurface.Of(30, 12);
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));
        a.m_nActiveFengHaoColor = 0x998877;

        var ops = a.PlanShowName();

        // NameBaseY = 181；有特效 → nY = 181 + 16 - 12 + 0 = 185
        // nX = 100 - (20 + 2 + 30)/2 = 100 - 26 = 74 ；特效画在 nX + 0 = 74
        Assert.Equal("FengHaoEffect", ops[0].Kind);
        Assert.Equal(74, ops[0].X);
        Assert.Equal(185 - (12 - 12) / 2, ops[0].Y);

        // 文字起点 nX = 74 + 30 + 2 + 0 = 106
        var txt = ops.Find(o => o.Kind == "FengHaoTextText");
        Assert.NotNull(txt);
        Assert.Equal(106, txt!.X);
        Assert.Equal(0x998877, txt.Color);
    }

    [Fact]
    public void PlanShowName_FengHaoTextOnlyIsCentered()
    {
        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));
        a.m_nActiveFengHaoColor = 0x445566;

        var ops = a.PlanShowName();

        // NameBaseY = 200 - 19 = 181（无勾选、无 NewopUI170）
        // 仅文字封号分支不改 nY：x = 100 - 20/2 + 0 = 90
        var txt = ops.Find(o => o.Kind == "FengHaoTextText");
        Assert.NotNull(txt);
        Assert.Equal(90, txt!.X);
        Assert.Equal(181, txt.Y);
    }

    [Fact]
    public void PlanShowName_ShopStallSkipsFengHaoEntirely()
    {
        var a = MakeActor();
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_FengHaoEffectSurface = LabelSurface.Of(30, 12);
        a.FengHaoImageInfo.Add(LabelSurface.Of(20, 10));
        a.m_boShopStall = true;

        var ops = a.PlanShowName();

        Assert.DoesNotContain(ops, o => o.Kind.StartsWith("FengHao", StringComparison.Ordinal));
        Assert.Contains(ops, o => o.Kind == "NameText");   // 人名仍绘制
    }

    [Fact]
    public void PlanShowName_RaceNameOffsetsApplied()
    {
        TActorCore.LabelConfig.nMonNameOffsetX = 7;
        TActorCore.LabelConfig.nMonNameOffsetY = 9;

        var a = MakeActor(race: 80, sayX: 100, sayY: 200);
        a.m_NameTextSurface = LabelSurface.Of(40, 10);
        a.m_btHorse = 0;

        var ops = a.PlanShowName();
        // nX = 100 + 7 = 107 ; nY = 230 + 9 = 239 ; half = 20
        Assert.Equal(107 - 20, ops[4].X);
        Assert.Equal(239, ops[4].Y);
    }

    // ===================== ShowShopName =====================

    [Fact]
    public void PlanShowShopName_RequiresStallAndNameImage()
    {
        var a = MakeActor();
        Assert.Empty(a.PlanShowShopName(500));

        a.m_boShopStall = true;
        Assert.Empty(a.PlanShowShopName(500));          // 无店名纹理

        a.m_ShopNameImageInfo = LabelSurface.Of(0, 10);
        Assert.Empty(a.PlanShowShopName(500));          // 宽度 0
    }

    [Fact]
    public void PlanShowShopName_NoFontDrawsNothing()
    {
        TActorCore.CurrentFontAvailableFn = () => false;
        var a = MakeActor();
        a.m_boShopStall = true;
        a.m_ShopNameImageInfo = LabelSurface.Of(60, 12);

        Assert.Empty(a.PlanShowShopName(500));
    }

    [Fact]
    public void PlanShowShopName_StampsTickBeforeCanDrawGuard()
    {
        var a = MakeActor();
        a.m_boShopStall = true;
        a.m_ShopNameImageInfo = LabelSurface.Of(60, 12);
        a.m_boCanDraw = false;

        var ops = a.PlanShowShopName(777);

        Assert.Empty(ops);
        Assert.Equal(777u, a.m_dwShowShopNameTimeTick);   // 打点在 m_boCanDraw 之前
    }

    [Fact]
    public void PlanShowShopName_NoHeadPicUsesSayYMinus25()
    {
        TActorCore.LabelConfig.boShopHeadPic = false;
        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_boShopStall = true;
        a.m_ShopNameImageInfo = LabelSurface.Of(60, 12);
        a.m_nNameColor = 0x222222;

        var ops = a.PlanShowShopName(500);

        // nX = 100 - 30 = 70 ; nY = 200 - 25 = 175
        var txt = ops.Find(o => o.Kind == "ShopNameText");
        Assert.NotNull(txt);
        Assert.Equal(70, txt!.X);
        Assert.Equal(175, txt.Y);
        Assert.Equal(0x0086C2DF, txt.Color);            // 非自己/非焦点
    }

    [Fact]
    public void PlanShowShopName_HeadPicWithBackgroundUsesMinus43AndStretchRect()
    {
        TActorCore.LabelConfig.boShopHeadPic = true;
        TActorCore.ShopNameBackgroundImage = LabelSurface.Of(80, 20);

        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_boShopStall = true;
        a.m_ShopNameImageInfo = LabelSurface.Of(60, 12);

        var ops = a.PlanShowShopName(500);

        var bg = ops.Find(o => o.Kind.StartsWith("ShopBg", StringComparison.Ordinal));
        Assert.NotNull(bg);
        // tempN = (20-12)/2 = 4 ; left = 70 - 1 - 4 = 65 ; top = 157 - 1 - 4 = 152
        // bottom = top + 20 = 172
        Assert.Equal("ShopBg:172", bg!.Kind);
        Assert.Equal(65, bg.X);
        Assert.Equal(152, bg.Y);

        var txt = ops.Find(o => o.Kind == "ShopNameText");
        Assert.Equal(157, txt!.Y);                      // 200 - 43
    }

    [Fact]
    public void PlanShowShopName_HeadPicWithoutBackgroundUsesMinus40()
    {
        TActorCore.LabelConfig.boShopHeadPic = true;
        TActorCore.ShopNameBackgroundImage = null;

        var a = MakeActor(sayY: 200);
        a.m_boShopStall = true;
        a.m_ShopNameImageInfo = LabelSurface.Of(60, 12);

        var ops = a.PlanShowShopName(500);

        // 无底图 → 不产出 ShopBg，仅 5 条文字描边
        Assert.DoesNotContain(ops, o => o.Kind.StartsWith("ShopBg", StringComparison.Ordinal));
        Assert.Equal(5, ops.Count);
        var txt = ops.Find(o => o.Kind == "ShopNameText");
        Assert.Equal(160, txt!.Y);                      // 200 - 40
    }

    [Fact]
    public void PlanShowShopName_SelfOrFocusUsesBrightColor()
    {
        TActorCore.GetRgbFn = c => c + 1;

        var self = MakeActor(sayY: 200);
        TActorCore.MySelfRef = self;
        self.m_boShopStall = true;
        self.m_ShopNameImageInfo = LabelSurface.Of(60, 12);
        var ops = self.PlanShowShopName(500);
        Assert.Equal(251, ops.Find(o => o.Kind == "ShopNameText")!.Color);

        TActorCore.MySelfRef = null;
        var focus = MakeActor(sayY: 200);
        focus.m_boShopStall = true;
        focus.m_ShopNameImageInfo = LabelSurface.Of(60, 12);
        focus.IsFocused = true;
        ops = focus.PlanShowShopName(500);
        Assert.Equal(251, ops.Find(o => o.Kind == "ShopNameText")!.Color);
    }

    // ===================== ShowNumberLable =====================

    [Fact]
    public void PlanShowNumberLable_ShopStallExitsImmediately()
    {
        var a = MakeActor();
        a.m_boShopStall = true;
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);

        Assert.Empty(a.PlanShowNumberLable(123));
        Assert.Equal(0u, a.m_ShowNumberLableTimeTick);     // 打点也未执行
    }

    [Fact]
    public void PlanShowNumberLable_RequiresImageAndFont()
    {
        var a = MakeActor();
        Assert.Empty(a.PlanShowNumberLable(123));           // 无纹理

        a.m_NumberLableImageInfo = LabelSurface.Of(0, 10);
        Assert.Empty(a.PlanShowNumberLable(123));           // 宽 0

        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);
        TActorCore.CurrentFontAvailableFn = () => false;
        Assert.Empty(a.PlanShowNumberLable(123));           // 无字体
    }

    [Fact]
    public void PlanShowNumberLable_WithoutNewopTextureUsesMinus22()
    {
        TActorCore.NewopUI170Texture0 = null;

        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);

        var ops = a.PlanShowNumberLable(555);

        Assert.Equal(555u, a.m_ShowNumberLableTimeTick);
        Assert.Equal(5, ops.Count);
        // nX = 100 - 20 = 80 ; nY = 200 - 22 = 178 ; 本色 clWhite
        Assert.Equal(79, ops[0].X); Assert.Equal(178, ops[0].Y);
        Assert.Equal(80, ops[4].X);
        Assert.Equal(0xFFFFFF, ops[4].Color);
        Assert.Equal("NumberLableText", ops[4].Kind);
    }

    [Fact]
    public void PlanShowNumberLable_WithNewopTextureUsesHeightFormula()
    {
        TActorCore.NewopUI170Texture0 = LabelSurface.Of(32, 3);

        var a = MakeActor(sayY: 200);
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);

        var ops = a.PlanShowNumberLable(555);

        // nY = 200 - (3*2 + 4) - 12 = 178
        Assert.Equal(178, ops[4].Y);
    }

    [Fact]
    public void PlanShowNumberLable_CustomActorAddsHpOffsets()
    {
        TActorCore.NewopUI170Texture0 = null;

        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);
        a.IsCustomActor = true;
        a.m_nChangeAppr = -1;                              // < 0 才进入
        a.CustomActorHpOffsetX = 3;
        a.CustomActorHpTextOffsetX = 4;
        a.CustomActorHpOffsetY = 5;
        a.CustomActorHpTextOffsetY = 6;

        var ops = a.PlanShowNumberLable(555);

        // nX = 100 - 20 + 7 = 87 ; nY = 200 - 22 + 11 = 189
        Assert.Equal(86, ops[0].X);
        Assert.Equal(189, ops[4].Y);

        // m_nChangeAppr >= 0 → 不加偏移
        a.m_nChangeAppr = 0;
        ops = a.PlanShowNumberLable(555);
        Assert.Equal(80, ops[4].X);
        Assert.Equal(178, ops[4].Y);
    }

    [Fact]
    public void PlanShowNumberLable_NonCustomActorNeverAddsOffsets()
    {
        TActorCore.NewopUI170Texture0 = null;

        var a = MakeActor(sayX: 100, sayY: 200);
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);
        a.IsCustomActor = false;
        a.m_nChangeAppr = -1;
        a.CustomActorHpOffsetX = 100;                      // 不应生效

        var ops = a.PlanShowNumberLable(555);
        Assert.Equal(80, ops[4].X);
    }

    [Fact]
    public void PlanShowNumberLable_NotCanDrawStillStampsTick()
    {
        var a = MakeActor();
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);
        a.m_boCanDraw = false;

        var ops = a.PlanShowNumberLable(999);

        Assert.Empty(ops);
        Assert.Equal(999u, a.m_ShowNumberLableTimeTick);
    }

    [Fact]
    public void PlanShowNumberLable_RaceOffsetsApplied()
    {
        TActorCore.NewopUI170Texture0 = null;
        TActorCore.LabelConfig.nNpcHPBarOffsetX = 5;
        TActorCore.LabelConfig.nNpcHPBarOffsetY = 6;

        var a = MakeActor(race: ActorLabelConsts.RC_MERCHANT, sayX: 100, sayY: 200);
        a.m_NumberLableImageInfo = LabelSurface.Of(40, 10);

        var ops = a.PlanShowNumberLable(555);
        Assert.Equal(100 - 20 + 5, ops[4].X);
        Assert.Equal(200 - 22 + 6, ops[4].Y);
    }

    // ===================== 常量 =====================

    [Fact]
    public void LabelConstantsMatchOriginal()
    {
        Assert.Equal(0, ActorLabelConsts.RC_PLAYOBJECT);
        Assert.Equal(1, ActorLabelConsts.RC_HEROOBJECT);
        Assert.Equal(50, ActorLabelConsts.RC_MERCHANT);
        Assert.Equal(15, ActorLabelConsts.RC_PEACENPC);   // Grobal2.pas 196
        Assert.Equal(2, ActorLabelConsts.NameOffset);
        Assert.Equal(14, ActorLabelConsts.SayLineStep);
        Assert.Equal(16, ActorLabelConsts.SayBlockStep);
        Assert.Equal(18, ActorLabelConsts.SayFengHaoExtra);
    }
}
