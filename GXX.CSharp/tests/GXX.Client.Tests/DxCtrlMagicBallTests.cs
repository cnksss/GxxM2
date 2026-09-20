using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
// DxComponents.pas 46-47 的两个枚举在解决方案内的唯一定义在 GXX.Client.LoadDx
// （见 src/GXX.Client/DxComponent/DxMagicBall.cs 文件头 using 段：跨车道重名接缝）。
using TMagicBallType = GXX.Client.LoadDx.TMagicBallType;
using TMagicBallValueAlignment = GXX.Client.LoadDx.TMagicBallValueAlignment;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// DxMagicBall.pas（808 行）的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/DxMagicBall.cs）：
//   * TMagicBallOverallSetting / TMagicBallAloneSetting：构造默认值、setter 的「值变化才回调 + Changed」、
//     SetOnGetImage 的双次立即回调、Assign 的字段搬移范围
//   * ComputeFillRect：四种对齐的「值 → 像素」映射（含 0/满/越界/负差 与银行家舍入）
//   * ComputeSplitZone / ComputeSecondHalfRect / ComputeSecondHalfFill：HPMP 组合的中间分隔区几何
//     （含原文「475 行 mbaRight 减 dwMaxHP」的怪癖断言）
//   * AdvanceEffectFrame：帧推进 + 间隔门控 + 回绕（含 negative frame 归零）
//   * QueryHumAbility：OnlyViewHP→20/30、四条默认值、回调覆盖、`<=0` 门控
//   * PaintMagicBall：三种 BallType 的落点序列、Empty/Full 的取图、分隔条放到最后、OnStopPaint
//
// 所有 `原文 NNN` 行号指 Source\Client-HGE\DxComponent\DxMagicBall.pas。
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlMagicBallTests
{
    private static TDxRect R(int l, int t, int r, int b) => TDxRect.Rect(l, t, r, b);

    /// <summary>5 张 100x80 的图（索引 0..4 全部有效），便于索引任意设置。</summary>
    private static TDxImageLibraryStub Lib5()
    {
        var lib = new TDxImageLibraryStub();
        for (int i = 0; i < 5; i++) lib.Add(new TDxTextureStub(100, 80));
        return lib;
    }

    // ===============================================================================
    // 一、构造默认值（原文 182-207 / 267-283）
    // ===============================================================================

    [Fact]
    public void OverallSetting_Defaults_MatchOriginal()
    {
        var s = new TMagicBallOverallSetting();

        Assert.Equal(TImageType.Prguse_wil, s.ImageType);      // 原文 186
        Assert.Equal(-1, s.EmptyHPMP);                          // 原文 187
        Assert.Equal(-1, s.FullHPMP);                           // 原文 188
        Assert.Equal(-1, s.EmptyHP);                            // 原文 189
        Assert.Equal(-1, s.FullHP);                             // 原文 190
        Assert.Equal(-1, s.Splite);                             // 原文 191
        Assert.Null(s.Image);                                   // 原文 192
        Assert.Equal(0, s.MiddleZoneWidth);                     // 原文 193
        Assert.False(s.OnlyViewHP);                             // 原文 194

        Assert.Equal(0, s.EffectCurrFrame);                     // 原文 196
        Assert.False(s.EffectDrawBlend);                        // 原文 198
        Assert.Equal(TImageType.Prguse_wil, s.EffectImageType); // 原文 199
        Assert.Equal(-1, s.EffectHPMPStart);                    // 原文 200
        Assert.Equal(-1, s.EffectHPStart);                      // 原文 201
        Assert.Equal(0, s.EffectImageCount);                    // 原文 202
        Assert.Equal(200, s.EffectPlayInterval);                // 原文 203

        Assert.Null(s.OnChange);                                // 原文 205
        Assert.Null(s.OnGetImage);                              // 原文 206
    }

    [Fact]
    public void AloneSetting_Defaults_MatchOriginal()
    {
        var s = new TMagicBallAloneSetting();

        Assert.Equal(TImageType.Prguse_wil, s.ImageType);       // 原文 267
        Assert.Equal(-1, s.Empty);                              // 原文 270
        Assert.Equal(-1, s.Full);                               // 原文 271

        Assert.Equal(0, s.EffectCurrFrame);                     // 原文 273
        Assert.False(s.EffectDrawBlend);                        // 原文 275
        Assert.Equal(TImageType.Prguse_wil, s.EffectImageType); // 原文 276
        Assert.Equal(-1, s.EffectStart);                        // 原文 277
        Assert.Equal(0, s.EffectImageCount);                    // 原文 278
        Assert.Equal(200, s.EffectPlayInterval);                // 原文 279

        Assert.Null(s.Image);                                   // 原文 281
        Assert.Null(s.OnChange);                                // 原文 282
        Assert.Null(s.OnGetImage);                              // 原文 283
    }

    // ===============================================================================
    // 二、设置类 setter / SetOnGetImage / Assign（原文 209-261 / 286-333）
    // ===============================================================================

    [Fact]
    public void OverallSetting_ImageType_OnlyFiresOnChange()
    {
        var s = new TMagicBallOverallSetting();
        var log = new List<string>();
        s.SetOnGetImage((o, t, img) => log.Add("gi:" + t));
        s.OnChange = _ => log.Add("changed");
        log.Clear();

        s.ImageType = TImageType.UI_wil;
        Assert.Equal(new List<string> { "gi:UI_wil", "changed" }, log);

        log.Clear();
        s.ImageType = TImageType.UI_wil;            // 同值 → 不触发
        Assert.Empty(log);
    }

    [Fact]
    public void OverallSetting_EffectImageType_OnlyFiresOnChange()
    {
        var s = new TMagicBallOverallSetting();
        var log = new List<string>();
        s.SetOnGetImage((o, t, img) => log.Add("gi:" + t));
        log.Clear();

        s.EffectImageType = TImageType.Prguse2_wil;
        Assert.Equal(new List<string> { "gi:Prguse2_wil" }, log);

        log.Clear();
        s.EffectImageType = TImageType.Prguse2_wil;
        Assert.Empty(log);
    }

    [Fact]
    public void OverallSetting_SetOnGetImage_FiresTwiceImmediately_ThenChanged()
    {
        // 原文 229-237：先主图库、再特效图库，各一次；最后 Changed
        var s = new TMagicBallOverallSetting();
        var log = new List<string>();
        s.OnChange = _ => log.Add("changed");

        s.SetOnGetImage((o, t, img) => log.Add("gi:" + t));

        Assert.Equal(new List<string> { "gi:Prguse_wil", "gi:Prguse_wil", "changed" }, log);
    }

    [Fact]
    public void OverallSetting_SetOnGetImage_NullStillFiresChanged()
    {
        // 原文 230 `if FOnGetImage <> nil` 才两条回调，但 239 的 Changed 无条件
        var s = new TMagicBallOverallSetting();
        var log = new List<string>();
        s.OnChange = _ => log.Add("changed");

        s.SetOnGetImage(null);

        Assert.Equal(new List<string> { "changed" }, log);
    }

    [Fact]
    public void AloneSetting_ImageType_OnlyFiresOnChange()
    {
        // 原文 286-294：与 Overall 版同形（值变化才回调 + Changed）
        var s = new TMagicBallAloneSetting();
        var log = new List<string>();
        s.SetOnGetImage((o, t, img) => log.Add("gi:" + t));
        s.OnChange = _ => log.Add("changed");
        log.Clear();

        s.ImageType = TImageType.UI_wil;
        Assert.Equal(new List<string> { "gi:UI_wil", "changed" }, log);

        log.Clear();
        s.ImageType = TImageType.UI_wil;            // 同值 → 不触发
        Assert.Empty(log);

        log.Clear();
        s.ImageType = TImageType.Prguse2_wil;       // 再变一次
        Assert.Equal(new List<string> { "gi:Prguse2_wil", "changed" }, log);
    }

    [Fact]
    public void AloneSetting_EffectImageType_OnlyFiresOnChange()
    {
        // 原文 296-304：回调传的是 FEffectImage（不是 FImage）
        var s = new TMagicBallAloneSetting();
        var log = new List<string>();
        s.SetOnGetImage((o, t, img) => log.Add("gi:" + t));
        log.Clear();

        s.EffectImageType = TImageType.Prguse2_wil;
        Assert.Equal(new List<string> { "gi:Prguse2_wil" }, log);

        log.Clear();
        s.EffectImageType = TImageType.Prguse2_wil;
        Assert.Empty(log);
    }

    [Fact]
    public void AloneSetting_OnChange_FiresOncePerSetter()
    {
        // 原文 316-320 Changed 仅在 setter 内触发；直接写字段（如 Assign 的字段直写）不触发
        var s = new TMagicBallAloneSetting();
        int changed = 0;
        s.OnChange = _ => changed++;

        s.Empty = 3;                                // published 字段直写，无 setter → 0
        Assert.Equal(0, changed);

        s.ImageType = TImageType.UI_wil;             // 走 SetImageType → 1
        Assert.Equal(1, changed);
    }

    [Fact]
    public void OverallSetting_OnChange_FiresOncePerSetter()
    {
        var s = new TMagicBallOverallSetting();
        int changed = 0;
        s.OnChange = _ => changed++;

        s.OnlyViewHP = true;                        // published 字段直写，无 setter → 0
        Assert.Equal(0, changed);

        s.EffectImageType = TImageType.UI_wil;       // 走 SetEffectImageType → 1
        Assert.Equal(1, changed);
    }

    [Fact]
    public void AloneSetting_SetOnGetImage_FiresTwiceImmediately()
    {
        var s = new TMagicBallAloneSetting();
        var log = new List<string>();
        s.OnChange = _ => log.Add("changed");

        s.SetOnGetImage((o, t, img) => log.Add("gi:" + t));

        Assert.Equal(new List<string> { "gi:Prguse_wil", "gi:Prguse_wil", "changed" }, log);
    }

    [Fact]
    public void OverallSetting_Assign_CopiesBusinessFieldsOnly()
    {
        var src = new TMagicBallOverallSetting
        {
            EmptyHPMP = 1,
            FullHPMP = 2,
            EmptyHP = 3,
            FullHP = 4,
            Splite = 5,
            MiddleZoneWidth = 6,
            OnlyViewHP = true,
            EffectCurrFrame = 9,          // 特效字段**不在** Assign 里
            EffectHPStart = 77,
        };

        var dst = new TMagicBallOverallSetting();
        dst.Assign(src);

        Assert.Equal(1, dst.EmptyHPMP);
        Assert.Equal(2, dst.FullHPMP);
        Assert.Equal(3, dst.EmptyHP);
        Assert.Equal(4, dst.FullHP);
        Assert.Equal(5, dst.Splite);
        Assert.Equal(6, dst.MiddleZoneWidth);
        Assert.True(dst.OnlyViewHP);
        Assert.Equal(0, dst.EffectCurrFrame);   // 未搬（原文 245-261 只搬 7 个业务字段）
        Assert.Equal(-1, dst.EffectHPStart);
        Assert.Null(dst.Image);                 // 原文也不搬 FImage
    }

    [Fact]
    public void OverallSetting_Assign_NullIsSafe()
    {
        var s = new TMagicBallOverallSetting { EmptyHPMP = 7 };
        s.Assign(null);
        Assert.Equal(7, s.EmptyHPMP);
    }

    [Fact]
    public void OverallSetting_Assign_CopiesImageTypeAndOnGetImageViaProperty()
    {
        // 原文 247-248：`FImageType := Source.FImageType` 与 `OnGetImage := Source.OnGetImage`
        //（**属性赋值** → 走 SetOnGetImage 的两次回调）
        var src = new TMagicBallOverallSetting { ImageType = TImageType.Prguse2_wil };
        var srcCalls = 0;
        src.OnGetImage = (o, t, i) => srcCalls++;

        var dst = new TMagicBallOverallSetting();
        dst.Assign(src);

        Assert.Equal(TImageType.Prguse2_wil, dst.ImageType);
        Assert.Equal(2, srcCalls);              // SetOnGetImage 立即回调两次
    }

    [Fact]
    public void OverallSetting_Assign_RaisesChangedOnce()
    {
        var src = new TMagicBallOverallSetting();
        var dst = new TMagicBallOverallSetting();
        int changed = 0;
        dst.OnChange = _ => changed++;

        dst.Assign(src);                        // 原文 259 末尾 Changed()

        // 加上 SetOnGetImage 内部的 Changed，共 2 次（源无回调时 SetOnGetImage 仍 Changed）
        Assert.Equal(2, changed);
    }

    [Fact]
    public void AloneSetting_Assign_CopiesEmptyFullAndImageType()
    {
        var src = new TMagicBallAloneSetting
        {
            Empty = 11, Full = 12, EffectStart = 99, ImageType = TImageType.UI_wil,
        };
        var dst = new TMagicBallAloneSetting();

        dst.Assign(src);

        Assert.Equal(11, dst.Empty);
        Assert.Equal(12, dst.Full);
        Assert.Equal(TImageType.UI_wil, dst.ImageType);
        Assert.Equal(-1, dst.EffectStart);      // 原文 322-333 不搬特效字段
    }

    [Fact]
    public void AloneSetting_Assign_NullIsSafe()
    {
        new TMagicBallAloneSetting().Assign(null);
    }

    // ===============================================================================
    // 三、ComputeFillRect（原文 406-412 / 448-454 / 528-534 / 651-657 / 689-695 / 728-734 / 766-772）
    // ===============================================================================

    [Fact]
    public void ComputeFillRect_MbaLeft_RightIsValueRounded()
    {
        // fullW=100, maxBase=100, value=50 → Right = Round(100/100*50) = 50
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 100),
            100, 100, 100, 50);

        Assert.Equal(50, r.Right);
        Assert.Equal(0, r.Left);        // 原文此行不改 Left
        Assert.Equal(100, r.Bottom);    // 其他边不动
    }

    [Fact]
    public void ComputeFillRect_MbaLeft_ZeroAndFull()
    {
        var zero = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 100),
            100, 100, 100, 0);
        Assert.Equal(0, zero.Right);

        var full = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 100),
            100, 100, 100, 100);
        Assert.Equal(100, full.Right);
    }

    [Fact]
    public void ComputeFillRect_MbaRight_LeftIsRemainder()
    {
        // 原文 457：Left = Max(Round(fullW / maxBase * Max(maxBase - value, 0)), 0)
        // value=30 → Max(100-30,0)=70 → Round(100/100*70) = 70
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaRight, R(0, 0, 100, 100),
            100, 100, 100, 30);
        Assert.Equal(70, r.Left);
    }

    [Fact]
    public void ComputeFillRect_MbaRight_ValueAboveMax_ClampsToZero()
    {
        // value > maxBase → Max(maxBase - value, 0) = 0 → Left = 0（满格）
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaRight, R(0, 0, 100, 100),
            100, 100, 100, 150);
        Assert.Equal(0, r.Left);
    }

    [Fact]
    public void ComputeFillRect_MbaTop_BottomIsValueRounded()
    {
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaTop, R(0, 0, 100, 100),
            100, 100, 200, 50);
        Assert.Equal(25, r.Bottom);     // Round(100 / 200 * 50)
    }

    [Fact]
    public void ComputeFillRect_MbaBottom_TopIsRemainder()
    {
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaBottom, R(0, 0, 100, 100),
            100, 100, 200, 50);
        Assert.Equal(75, r.Top);        // Round(100 / 200 * Max(200-50,0)=150)
    }

    [Fact]
    public void ComputeFillRect_MbaBottom_ValueAboveMax_ClampsToZero()
    {
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaBottom, R(0, 0, 100, 100),
            100, 100, 200, 300);
        Assert.Equal(0, r.Top);
    }

    [Fact]
    public void ComputeFillRect_UsesDoubleDivision_NotIntegerDivision()
    {
        // 关键：Delphi `/` 返回 Real。若误写整数除法，(int)(7/100)=0 → 结果会是 0
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 100),
            7, 100, 100, 50);
        // Round(7.0 / 100 * 50) = Round(3.5) = 4（银行家舍入到偶数）
        Assert.Equal(4, r.Right);
    }

    [Fact]
    public void ComputeFillRect_BankersRounding_MatchesDelphiRound()
    {
        // Delphi Round 是银行家舍入：.5 → 最近偶数
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 10, 10),
            10, 10, 4, 1);
        Assert.Equal(2, r.Right);       // 10/4*1 = 2.5 → 2

        var r2 = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 10, 10),
            10, 10, 4, 3);
        Assert.Equal(8, r2.Right);      // 10/4*3 = 7.5 → 8
    }

    [Fact]
    public void ComputeFillRect_WidthAndHeightUseSeparateBases()
    {
        // 左对齐看 fullW，上对齐看 fullH —— 传不同的 fullW/fullH 即可区分
        var left = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 100),
            40, 200, 100, 50);
        Assert.Equal(20, left.Right);   // Round(40/100*50)

        var top = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaTop, R(0, 0, 100, 100),
            40, 200, 100, 50);
        Assert.Equal(100, top.Bottom);  // Round(200/100*50)
    }

    [Fact]
    public void ComputeFillRect_DoesNotModifyOtherEdges()
    {
        var r = TDXMagicBall.ComputeFillRect(TMagicBallValueAlignment.mbaLeft, R(5, 6, 100, 80),
            100, 80, 100, 50);
        Assert.Equal(5, r.Left);
        Assert.Equal(6, r.Top);
        Assert.Equal(80, r.Bottom);     // 只改 Right
    }

    // ===============================================================================
    // 四、ComputeSplitZone（原文 437-446 / 556-565）
    // ===============================================================================

    [Fact]
    public void ComputeSplitZone_LeftAlignment_HalvesHeight()
    {
        // 原文：nWidth := d.Width; nHeight := d.Height div 2; PaintRect.Bottom := nHeight - MiddleZoneWidth div 2
        var n = TDXMagicBall.ComputeSplitZone(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 80),
            100, 80, 6, out var half);

        Assert.Equal(100, n.X);
        Assert.Equal(40, n.Y);                    // 80 div 2
        Assert.Equal(40 - 3, half.Bottom);        // nHeight - 6 div 2
    }

    [Fact]
    public void ComputeSplitZone_RightAlignment_AlsoHalvesHeight()
    {
        // 原文判据是 `in [mbaLeft, mbaRight]` —— 右对齐与左对齐同路
        var n = TDXMagicBall.ComputeSplitZone(TMagicBallValueAlignment.mbaRight, R(0, 0, 100, 80),
            100, 80, 0, out var half);
        Assert.Equal(100, n.X);
        Assert.Equal(40, n.Y);
        Assert.Equal(40, half.Bottom);
    }

    [Fact]
    public void ComputeSplitZone_TopAlignment_HalvesWidth()
    {
        var n = TDXMagicBall.ComputeSplitZone(TMagicBallValueAlignment.mbaTop, R(0, 0, 100, 80),
            100, 80, 6, out var half);

        Assert.Equal(50, n.X);                    // 100 div 2
        Assert.Equal(80, n.Y);
        Assert.Equal(50 - 3, half.Right);         // nWidth - 6 div 2
    }

    [Fact]
    public void ComputeSplitZone_BottomAlignment_AlsoHalvesWidth()
    {
        var n = TDXMagicBall.ComputeSplitZone(TMagicBallValueAlignment.mbaBottom, R(0, 0, 100, 80),
            100, 80, 0, out var half);
        Assert.Equal(50, n.X);
        Assert.Equal(80, n.Y);
        Assert.Equal(50, half.Right);
    }

    [Fact]
    public void ComputeSplitZone_OddSizeTruncates_IntegerDivision()
    {
        // div 是整数除法：81 div 2 = 40；MiddleZoneWidth=5 → 5 div 2 = 2
        var n = TDXMagicBall.ComputeSplitZone(TMagicBallValueAlignment.mbaLeft, R(0, 0, 101, 81),
            101, 81, 5, out var half);
        Assert.Equal(40, n.Y);
        Assert.Equal(40 - 2, half.Bottom);
    }

    // ===============================================================================
    // 五、ComputeSecondHalfRect（原文 467-471 / 589-593）
    // ===============================================================================

    [Fact]
    public void ComputeSecondHalfRect_LeftAlignment_UsesTop()
    {
        var r = TDXMagicBall.ComputeSecondHalfRect(TMagicBallValueAlignment.mbaLeft, R(0, 0, 100, 80),
            100, 40, 6);
        Assert.Equal(43, r.Top);        // 40 + 6 div 2
        Assert.Equal(0, r.Left);        // Left 不动
    }

    [Fact]
    public void ComputeSecondHalfRect_RightAlignment_UsesTop()
    {
        var r = TDXMagicBall.ComputeSecondHalfRect(TMagicBallValueAlignment.mbaRight, R(0, 0, 100, 80),
            100, 40, 6);
        Assert.Equal(43, r.Top);
    }

    [Fact]
    public void ComputeSecondHalfRect_TopAlignment_UsesLeft()
    {
        var r = TDXMagicBall.ComputeSecondHalfRect(TMagicBallValueAlignment.mbaTop, R(0, 0, 100, 80),
            50, 80, 6);
        Assert.Equal(53, r.Left);       // 50 + 3
        Assert.Equal(0, r.Top);
    }

    [Fact]
    public void ComputeSecondHalfRect_BottomAlignment_UsesLeft()
    {
        var r = TDXMagicBall.ComputeSecondHalfRect(TMagicBallValueAlignment.mbaBottom, R(0, 0, 100, 80),
            50, 80, 6);
        Assert.Equal(53, r.Left);
    }

    // ===============================================================================
    // 六、ComputeSecondHalfFill（原文 474-479 / 596-601 / 728-734）
    // ===============================================================================

    [Fact]
    public void ComputeSecondHalfFill_MbaRight_UsesHpBaseQuirk()
    {
        // 原文 475/597（HPMP 组合段）：`Left := PaintRect.Left + Max(Round(nWidth / dwMaxMP * Max(dwMaxHP - dwMP, 0)), 0)`
        // maxHp=2000, maxMp=1000, mp=400, nWidth=100
        //   → Max(2000-400,0)=1600 → Round(100/1000*1600) = 160
        var r = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaRight, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 400, useHpBaseForRight: true);
        Assert.Equal(0 + 160, r.Left);
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaRight_UsesMpBase_WhenNotFlagged()
    {
        // 原文 730（非组合段）：用 Max(dwMaxMP - dwMP, 0) = 600 → Round(100/1000*600) = 60
        var r = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaRight, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 400, useHpBaseForRight: false);
        Assert.Equal(0 + 60, r.Left);
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaRight_QuirkDiffersFromNonQuirk()
    {
        // 同参数下两种基准必须给出不同结果 —— 这是「原文 475 减 dwMaxHP」怪癖的可观测证据
        var quirk = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaRight, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 400, useHpBaseForRight: true);
        var normal = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaRight, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 400, useHpBaseForRight: false);
        Assert.NotEqual(quirk.Left, normal.Left);
        Assert.True(quirk.Left > normal.Left);   // maxHp > maxMp 时长条更靠右
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaLeft_RightIsFromHalfRectLeft()
    {
        // 原文 474：`Right := PaintRect.Left + Max(Round(nWidth / dwMaxMP * dwMP), 0)`
        var r = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaLeft, R(10, 43, 100, 80),
            100, 40, 2000, 1000, 500, useHpBaseForRight: true);
        Assert.Equal(10 + 50, r.Right);   // Round(100/1000*500) = 50
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaLeft_ZeroAndFullMp()
    {
        var zero = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaLeft, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 0, useHpBaseForRight: true);
        Assert.Equal(0, zero.Right);

        var full = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaLeft, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 1000, useHpBaseForRight: true);
        Assert.Equal(100, full.Right);
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaTop_BottomFromMpValue()
    {
        var r = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaTop, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 500, useHpBaseForRight: true);
        Assert.Equal(20, r.Bottom);       // Round(40 / 1000 * 500)
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaBottom_TopIsMpRemainder_UsingMpBase()
    {
        // mbaBottom 分支**不看** useHpBaseForRight，固定用 maxMp
        var r = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaBottom, R(0, 43, 100, 80),
            100, 40, 2000, 1000, 400, useHpBaseForRight: true);
        Assert.Equal(24, r.Top);          // Round(40 / 1000 * Max(1000-400,0)=600)
    }

    [Fact]
    public void ComputeSecondHalfFill_MbaRight_MpAboveMax_ClampsToZeroOffset()
    {
        var r = TDXMagicBall.ComputeSecondHalfFill(TMagicBallValueAlignment.mbaRight, R(0, 43, 100, 80),
            100, 40, 500, 1000, 1500, useHpBaseForRight: true);
        Assert.Equal(0, r.Left);          // Max(500-1500, 0) = 0
    }

    // ===============================================================================
    // 七、AdvanceEffectFrame（原文 513-520 / 675-682 / 752-759）
    // ===============================================================================

    [Fact]
    public void AdvanceEffectFrame_NotElapsed_DoesNotAdvance()
    {
        uint tick = DxTickCount.MyGetTickCount();
        int frame = 0;

        int got = TDXMagicBall.AdvanceEffectFrame(ref tick, ref frame, 5, 100000);

        Assert.Equal(0, got);
        Assert.Equal(0, frame);
    }

    [Fact]
    public void AdvanceEffectFrame_Elapsed_AdvancesAndRefreshesTick()
    {
        uint tick = 0;                  // 远古时刻 → 必然到期
        int frame = 0;
        uint before = DxTickCount.MyGetTickCount();

        int got = TDXMagicBall.AdvanceEffectFrame(ref tick, ref frame, 5, 1);

        Assert.Equal(1, got);
        Assert.True(tick >= before);    // EffectLastTick := MyGetTickCount
    }

    [Fact]
    public void AdvanceEffectFrame_WrapsAtImageCount()
    {
        uint tick = 0;
        int frame = 4;

        int got = TDXMagicBall.AdvanceEffectFrame(ref tick, ref frame, 5, 1);

        Assert.Equal(0, got);           // 4 + 1 = 5 >= ImageCount(5) → 归 0
    }

    [Fact]
    public void AdvanceEffectFrame_NegativeFrame_ResetsToZero()
    {
        uint tick = DxTickCount.MyGetTickCount();   // 未到期 → 不推进
        int frame = -3;

        int got = TDXMagicBall.AdvanceEffectFrame(ref tick, ref frame, 5, 100000);

        Assert.Equal(0, got);           // 原文无条件判 `< 0` → 归 0
    }

    [Fact]
    public void AdvanceEffectFrame_ZeroImageCount_ResetsToZero()
    {
        uint tick = DxTickCount.MyGetTickCount();
        int frame = 3;

        int got = TDXMagicBall.AdvanceEffectFrame(ref tick, ref frame, 0, 100000);

        Assert.Equal(0, got);           // frame >= ImageCount(0) → 归 0
    }

    [Fact]
    public void AdvanceEffectFrame_ImageCountOne_AlwaysZero()
    {
        uint tick = 0;
        int frame = 0;

        Assert.Equal(0, TDXMagicBall.AdvanceEffectFrame(ref tick, ref frame, 1, 1));
    }

    [Fact]
    public void AdvanceEffectFrameOverall_UsesOverallFields()
    {
        var ball = new TDXMagicBall();
        ball.OverallSetting.EffectImageCount = 3;
        ball.OverallSetting.EffectPlayInterval = 1;
        ball.OverallSetting.EffectLastTick = 0;

        Assert.Equal(1, ball.AdvanceEffectFrameOverall());
        Assert.Equal(1, ball.OverallSetting.EffectCurrFrame);
        Assert.Equal(0, ball.AloneSetting.EffectCurrFrame);   // 不串到 Alone
    }

    [Fact]
    public void AdvanceEffectFrameAlone_UsesAloneFields()
    {
        var ball = new TDXMagicBall();
        ball.AloneSetting.EffectImageCount = 3;
        ball.AloneSetting.EffectPlayInterval = 1;
        ball.AloneSetting.EffectLastTick = 0;

        Assert.Equal(1, ball.AdvanceEffectFrameAlone());
        Assert.Equal(1, ball.AloneSetting.EffectCurrFrame);
        Assert.Equal(0, ball.OverallSetting.EffectCurrFrame);
    }

    [Fact]
    public void AdvanceEffectFrameAlone_NotElapsed_StaysZero()
    {
        var ball = new TDXMagicBall();
        ball.AloneSetting.EffectImageCount = 3;
        ball.AloneSetting.EffectPlayInterval = 100000;
        ball.AloneSetting.EffectLastTick = DxTickCount.MyGetTickCount();

        Assert.Equal(0, ball.AdvanceEffectFrameAlone());
    }

    // ===============================================================================
    // 八、QueryHumAbility / UseHpOnlyPath（原文 372-393 / 522 / 624）
    // ===============================================================================

    [Fact]
    public void QueryHumAbility_DefaultsLevel20_WhenOnlyViewHp()
    {
        var ball = new TDXMagicBall();
        ball.OverallSetting.OnlyViewHP = true;

        Assert.True(ball.QueryHumAbility(out byte job, out uint level, out uint hp, out uint maxHp,
            out uint mp, out uint maxMp));

        Assert.Equal(0, job);            // 默认 btJob = 0
        Assert.Equal(20u, level);        // 原文 373
        Assert.Equal(1500u, hp);         // 原文 377
        Assert.Equal(2000u, maxHp);      // 原文 378
        Assert.Equal(1700u, mp);         // 原文 380
        Assert.Equal(2000u, maxMp);      // 原文 381
    }

    [Fact]
    public void QueryHumAbility_DefaultsLevel30_Otherwise()
    {
        var ball = new TDXMagicBall();
        Assert.True(ball.QueryHumAbility(out _, out uint level, out _, out _, out _, out _));
        Assert.Equal(30u, level);        // 原文 375
    }

    [Fact]
    public void QueryHumAbility_CallbackOverridesAll()
    {
        var ball = new TDXMagicBall();
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) =>
        {
            j.Value = 3;
            lv.Value = 42;
            hp.Value = 111;
            maxHp.Value = 222;
            mp.Value = 333;
            maxMp.Value = 444;
        };

        Assert.True(ball.QueryHumAbility(out byte job, out uint level, out uint hp, out uint maxHp,
            out uint mp, out uint maxMp));

        Assert.Equal(3, job);
        Assert.Equal(42u, level);
        Assert.Equal(111u, hp);
        Assert.Equal(222u, maxHp);
        Assert.Equal(333u, mp);
        Assert.Equal(444u, maxMp);
    }

    [Fact]
    public void QueryHumAbility_PartialCallback_KeepsUntouchedDefaults()
    {
        // 回调只改等级 → 其余保持 1500/2000/1700/2000
        var ball = new TDXMagicBall();
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => lv.Value = 55;

        Assert.True(ball.QueryHumAbility(out _, out uint level, out uint hp, out uint maxHp,
            out uint mp, out uint maxMp));
        Assert.Equal(55u, level);
        Assert.Equal(1500u, hp);
        Assert.Equal(2000u, maxHp);
        Assert.Equal(1700u, mp);
        Assert.Equal(2000u, maxMp);
    }

    [Fact]
    public void QueryHumAbility_ZeroMaxHp_ReturnsFalse()
    {
        var ball = new TDXMagicBall();
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => maxHp.Value = 0;
        Assert.False(ball.QueryHumAbility(out _, out _, out _, out _, out _, out _));
    }

    [Fact]
    public void QueryHumAbility_ZeroMaxMp_ReturnsFalse()
    {
        var ball = new TDXMagicBall();
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => maxMp.Value = 0;
        Assert.False(ball.QueryHumAbility(out _, out _, out _, out _, out _, out _));
    }

    [Fact]
    public void QueryHumAbility_NotCalled_NoCallback()
    {
        var ball = new TDXMagicBall();
        int calls = 0;
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => calls++;
        Assert.Equal(0, calls);
    }

    [Theory]
    [InlineData(0, 27u, true)]
    [InlineData(0, 0u, true)]
    [InlineData(0, 28u, false)]     // 原文 `< 28`，28 不算
    [InlineData(1, 10u, false)]     // 原文 `btJob = 0`
    [InlineData(2, 1u, false)]
    public void UseHpOnlyPath_JobZeroAndLevelBelow28(byte job, uint level, bool expected)
        => Assert.Equal(expected, TDXMagicBall.UseHpOnlyPath(job, level));

    // ===============================================================================
    // 九、控件骨架（原文 335-356 / 796-801）
    // ===============================================================================

    [Fact]
    public void MagicBall_Constructor_Defaults_MatchOriginal()
    {
        var ball = new TDXMagicBall();

        Assert.Equal(TMagicBallType.mbtHPMP, ball.BallType);                   // 原文 338
        Assert.Equal(TMagicBallValueAlignment.mbaBottom, ball.ValueAlignment); // 原文 339
        Assert.NotNull(ball.OverallSetting);                                   // 原文 341
        Assert.NotNull(ball.AloneSetting);                                     // 原文 342
        Assert.Same(ball, ball.OverallSetting.Owner);                          // FOwner := Self
        Assert.Same(ball, ball.AloneSetting.Owner);
        Assert.Equal(90, ball.Width);                                          // 原文 347
        Assert.Equal(90, ball.Height);                                         // 原文 348
    }

    [Fact]
    public void MagicBall_Constructor_SettingsOnGetImageGoesToControl()
    {
        // 原文 344-345：两个设置的 OnGetImage 指向控件自身的 OnGetImage 属性
        var ball = new TDXMagicBall();
        var seen = new List<TImageType>();
        ball.SetOnGetImageV2((idx, t) => seen.Add(t));
        seen.Clear();       // SetOnGetImageV2 自身会立即回调两次，这里只看手动 invoke

        ball.OverallSetting.OnGetImage(ball.OverallSetting, TImageType.UI_wil, null);

        Assert.Equal(new List<TImageType> { TImageType.UI_wil }, seen);
    }

    [Fact]
    public void MagicBall_Constructor_AloneSettingOnGetImageAlsoGoesToControl()
    {
        var ball = new TDXMagicBall();
        var seen = new List<TImageType>();
        ball.SetOnGetImageV2((idx, t) => seen.Add(t));
        seen.Clear();

        ball.AloneSetting.OnGetImage(ball.AloneSetting, TImageType.Prguse2_wil, null);

        Assert.Equal(new List<TImageType> { TImageType.Prguse2_wil }, seen);
    }

    [Fact]
    public void MagicBall_SetOnGetImageV2_ForwardsAndChangedFiresOnBothSettings()
    {
        var ball = new TDXMagicBall();
        var log = new List<string>();
        ball.OverallSetting.OnChange = _ => log.Add("O");
        ball.AloneSetting.OnChange = _ => log.Add("A");

        ball.SetOnGetImageV2((idx, t) => { });

        // 两个设置各自 SetOnGetImage → 各触发一次 Changed
        Assert.Equal(new List<string> { "O", "A" }, log);
    }

    [Fact]
    public void MagicBall_SetOnGetImageV2_NullIsSafe()
    {
        var ball = new TDXMagicBall();
        ball.SetOnGetImageV2(null);
        Assert.Null(DxControlHooks.GetOnGetImage(ball));
    }

    [Fact]
    public void MagicBall_DisposeMagicBall_ClearsSettings()
    {
        var ball = new TDXMagicBall();
        ball.DisposeMagicBall();
        Assert.Null(ball.OverallSetting);
        Assert.Null(ball.AloneSetting);
    }

    [Fact]
    public void MagicBall_DisposeMagicBall_NullSettingsIsSafe()
    {
        var ball = new TDXMagicBall();
        ball.DisposeMagicBall();
        ball.DisposeMagicBall();     // 第二次不得抛
    }

    // ===============================================================================
    // 十、PaintMagicBall 落点序列（原文 358-794）
    // ===============================================================================

    /// <summary>
    /// 造一个可绘制的球：图库 5 张 100x80、四路图库全接上、ClientRect = 90x90、
    /// 默认 Empty/Full/Split 索引都可用，`OnGetHumAbility` 给固定值。
    /// 保持默认 `ValueAlignment = mbaBottom`（原文构造默认），ClientRect 与 Width/Height 一致 → vtRect.Left=Top=0。
    /// </summary>
    private static TDXMagicBall MakeBall(TMagicBallType type, TDxRecordingPainter painter,
        byte job = 0, uint level = 40)
    {
        var ball = new TDXMagicBall { BallType = type };
        ball.ClientRect = TDxRect.Bounds(0, 0, 90, 90);
        ball.Designing = false;
        ball.Painter = painter;

        var lib = Lib5();
        ball.OverallSetting.Image = lib;
        ball.OverallSetting.EffectImage = lib;
        ball.AloneSetting.Image = lib;
        ball.AloneSetting.EffectImage = lib;

        // 让三种 BallType 的 Empty/Full/Splite 都有有效索引
        ball.OverallSetting.EmptyHPMP = 0;
        ball.OverallSetting.FullHPMP = 1;
        ball.OverallSetting.EmptyHP = 2;
        ball.OverallSetting.FullHP = 3;
        ball.OverallSetting.Splite = 4;
        ball.AloneSetting.Empty = 0;
        ball.AloneSetting.Full = 1;

        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) =>
        {
            j.Value = job;
            lv.Value = level;
            hp.Value = 1000;
            maxHp.Value = 2000;
            mp.Value = 500;
            maxMp.Value = 1000;
        };
        return ball;
    }

    [Fact]
    public void PaintMagicBall_AbortsOnDegenerateVisibleRect()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.ClientRect = TDxRect.Rect(0, 0, 0, 0);

        ball.PaintMagicBall();

        Assert.Empty(p.Ops);
    }

    [Fact]
    public void PaintMagicBall_AbortsWhenMaxHpOrMaxMpIsZero()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => maxHp.Value = 0;

        ball.PaintMagicBall();

        Assert.Empty(p.Ops);            // 原文 386 直接 Exit
    }

    [Fact]
    public void PaintMagicBall_AbortsOnMaxMpZero()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => maxMp.Value = 0;

        ball.PaintMagicBall();

        Assert.Empty(p.Ops);
    }

    [Fact]
    public void PaintMagicBall_NullImageLibrary_DrawsNothing()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Image = null;

        ball.PaintMagicBall();

        Assert.Empty(p.Ops);
    }

    [Fact]
    public void PaintMagicBall_NegativeIndices_DrawNothing()
    {
        var p = new TDxRecordingPainter();
        var ball = new TDXMagicBall();
        ball.ClientRect = TDxRect.Bounds(0, 0, 90, 90);
        ball.Designing = false;
        ball.Painter = p;
        // 索引保持构造默认 -1；图库为空 → 不取图、不绘制
        ball.PaintMagicBall();
        Assert.Empty(p.Ops);
    }

    [Fact]
    public void PaintMagicBall_Hpmp_DrawsEmptyThenHpThenMpThenSplite()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.MiddleZoneWidth = 0;

        ball.PaintMagicBall();

        // 期望顺序：空底（整图）→ HP 半区（裁剪）→ MP 半区（裁剪）→ **分隔条放最后**
        Assert.Equal(4, p.Ops.Count);

        // 基准落点：x = (90 - 100) div 2 = -5，y = (90 - 80) div 2 = 5
        Assert.Equal("Draw(-5,5,tex100x80,2)", p.Ops[0]);
        Assert.Equal("Draw(-5,45,(0,40,50,80),tex100x80)", p.Ops[1]);    // mbaBottom: Top = 40
        Assert.Equal("Draw(45,45,(50,40,100,80),tex100x80)", p.Ops[2]);  // MP 半区从 nWidth=50 起
        Assert.Equal("Draw(-5,5,tex100x80,2)", p.Ops[3]);                // 分隔条居中，与空底同点
    }

    [Fact]
    public void PaintMagicBall_Hpmp_NoSpliteIndex_OnlyThreeOps()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_Hpmp_HpOnlyPath_NoSpliteEvenWhenConfigured()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p, job: 0, level: 10);   // < 28 → hpOnly

        ball.PaintMagicBall();

        // 原文 621-635 的分隔条段在 `not (btJob=0 and dwLevel<28)` 里 → hpOnly 不画
        Assert.Equal(2, p.Ops.Count);    // EmptyHP + FullHP
    }

    [Fact]
    public void PaintMagicBall_Hpmp_HpOnlyPath_UsesEmptyHpAndFullHp()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p, job: 0, level: 10);
        // 只把 HP 索引留有效，HPMP 索引全部失效 —— 若走错分支就一条都不画
        ball.OverallSetting.EmptyHPMP = -1;
        ball.OverallSetting.FullHPMP = -1;

        ball.PaintMagicBall();

        Assert.Equal(2, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_Hpmp_MpPath_Level28_UsesHpmpIndices()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p, job: 0, level: 28);   // 边界：28 不算 hpOnly
        ball.OverallSetting.EmptyHP = -1;
        ball.OverallSetting.FullHP = -1;
        ball.OverallSetting.Splite = -1;

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);    // EmptyHPMP + HP 半区 + MP 半区
    }

    [Fact]
    public void PaintMagicBall_Hpmp_NonWarriorJob_UsesHpmpPathEvenAtLowLevel()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p, job: 1, level: 10);
        ball.OverallSetting.EmptyHP = -1;
        ball.OverallSetting.FullHP = -1;
        ball.OverallSetting.Splite = -1;

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_AloneHp_DrawsEmptyAndFullOnly()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHP, p);
        ball.AloneSetting.EffectStart = -1;

        ball.PaintMagicBall();

        Assert.Equal(2, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_AloneMp_DrawsEmptyAndFullOnly()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtMP, p);
        ball.AloneSetting.EffectStart = -1;

        ball.PaintMagicBall();

        Assert.Equal(2, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_AloneHp_IgnoresOverallIndices()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHP, p);
        ball.OverallSetting.EmptyHPMP = -1;
        ball.OverallSetting.FullHPMP = -1;
        ball.OverallSetting.EmptyHP = -1;
        ball.OverallSetting.FullHP = -1;
        ball.OverallSetting.Splite = -1;

        ball.PaintMagicBall();

        Assert.Equal(2, p.Ops.Count);    // 只走 AloneSetting
    }

    [Fact]
    public void PaintMagicBall_AloneHp_UsesMaxHpBase_MpUsesMaxMpBase()
    {
        // mbtHP 与 mbtMP 的唯一差别是被绘的值/基准（hp/maxHp vs mp/maxMp）。
        // 用非对称数值让两条路径产出不同的 PaintRect 宽度，从而可区分。
        var pHp = new TDxRecordingPainter();
        var hpBall = MakeBall(TMagicBallType.mbtHP, pHp);
        hpBall.AloneSetting.EffectStart = -1;
        hpBall.PaintMagicBall();

        var pMp = new TDxRecordingPainter();
        var mpBall = MakeBall(TMagicBallType.mbtMP, pMp);
        mpBall.AloneSetting.EffectStart = -1;
        mpBall.PaintMagicBall();

        // hp = 1000/2000 → 半满；mp = 500/1000 → 也半满 → 4 参 Draw 串相同
        Assert.Equal(pHp.Ops[1], pMp.Ops[1]);
    }

    [Fact]
    public void PaintMagicBall_Effect_DrawsHpmpPairWhenConfigured()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = 0;
        ball.OverallSetting.EffectImageCount = 2;
        ball.OverallSetting.EffectPlayInterval = 100000;   // 不推进

        ball.PaintMagicBall();

        // Empty + HP 半区 + MP 半区 + 特效 HP + 特效 MP = 5
        Assert.Equal(5, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_Effect_ZeroImageCount_Skipped()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = 0;
        ball.OverallSetting.EffectImageCount = 0;          // 原文 511 门控

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_Effect_ZeroInterval_Skipped()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = 0;
        ball.OverallSetting.EffectImageCount = 2;
        ball.OverallSetting.EffectPlayInterval = 0;        // 门控要求 > 0

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_Effect_NegativeStart_Skipped()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = -1;
        ball.OverallSetting.EffectImageCount = 2;
        ball.OverallSetting.EffectPlayInterval = 100000;

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_Effect_AdvancesFrameOnEachPaint()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = 0;
        ball.OverallSetting.EffectImageCount = 2;
        ball.OverallSetting.EffectPlayInterval = 1;
        ball.OverallSetting.EffectLastTick = 0;

        ball.PaintMagicBall();

        Assert.Equal(1, ball.OverallSetting.EffectCurrFrame);
    }

    [Fact]
    public void PaintMagicBall_Effect_AloneHpPath_DrawsHpEffect()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHP, p);
        ball.AloneSetting.EffectStart = 0;
        ball.AloneSetting.EffectImageCount = 2;
        ball.AloneSetting.EffectPlayInterval = 100000;

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);    // Empty + Full + 特效
    }

    [Fact]
    public void PaintMagicBall_OnStopPaint_InvokedOnceAtEnd()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        int stops = 0;
        ball.OnStopPaint = _ => { Assert.Equal(4, p.Ops.Count); stops++; };

        ball.PaintMagicBall();

        Assert.Equal(1, stops);          // 原文 792-793
    }

    [Fact]
    public void PaintMagicBall_OnStopPaint_NotInvokedWhenAborting()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OnGetHumAbility = (s, j, lv, hp, maxHp, mp, maxMp) => maxHp.Value = 0;
        int stops = 0;
        ball.OnStopPaint = _ => stops++;

        ball.PaintMagicBall();

        Assert.Equal(0, stops);          // 原文 Exit 早于 OnStopPaint
    }

    [Fact]
    public void PaintMagicBall_AreaCallback_FiresHpThenMp()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        var flags = new List<bool>();
        ball.OnAfterDrawMagicBallArea = (s, isHp, rect) => flags.Add(isHp);

        ball.PaintMagicBall();

        Assert.Equal(new List<bool> { true, false }, flags);   // 原文 464(true) / 489(false)
    }

    [Fact]
    public void PaintMagicBall_AreaCallback_RectIsCenteredPlusPaintRect()
    {
        // 原文 458-465 / 483-490：R := vtRect.TopLeft + ((Width-d.Width) div 2, (Height-d.Height) div 2) + PaintRect
        // 贴图 100x80、控件 90x90 → 居中偏移 (-5, +5)（`div 2` 对负数向零截断 = C# `/ 2`）
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        var all = new List<string>();
        ball.OnAfterDrawMagicBallArea = (s, isHp, rect) => all.Add($"{isHp}:{rect}");

        ball.PaintMagicBall();

        // HP 半区（原文 436-453）：竖切 nWidth=50 → mbaBottom: Top = Round(80/2000*1000) = 40 → PaintRect(0,40,50,80)
        // MP 半区（原文 467-478）：起点 (50,0,100,80) → Top = Round(80/1000*500) = 40 → PaintRect(50,40,100,80)
        Assert.Equal(new List<string> { "True:(-5,45,45,85)", "False:(45,45,95,85)" }, all);
    }

    [Fact]
    public void PaintMagicBall_HpmpEffect_HpHalfUsesWidthAsHeightQuirk_MpHalfDoesNot()
    {
        // 原文 576/578 vs 604/606 的**差异断言**：
        //   特效 HP 半区的 y 用 `(Height - d.Width) div 2`（把 Width 当 Height 用，原文怪癖）
        //   特效 MP 半区的 y 用 `(Height - d.Height) div 2`（正常）
        // 贴图 100x80、控件 90x90 → 前者 (90-100)/2 = -5，后者 (90-80)/2 = +5 —— 差 10 像素。
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = 0;
        ball.OverallSetting.EffectImageCount = 2;
        ball.OverallSetting.EffectPlayInterval = 100000;
        var effectRects = new List<string>();
        ball.OnAfterDrawMagicBallEffectArea = (s, isHp, rect) => effectRects.Add($"{isHp}:{rect}");

        ball.PaintMagicBall();

        // ops: 0 空底 / 1 HP 半区 / 2 MP 半区 / 3 特效 HP / 4 特效 MP
        Assert.Equal(5, p.Ops.Count);
        Assert.Equal("Draw(-5,35,(0,40,50,80),tex100x80)", p.Ops[3]);   // y = -5 + 40
        Assert.Equal("Draw(45,45,(50,40,100,80),tex100x80)", p.Ops[4]); // y = +5 + 40

        // 回调矩形两条都走 (Height - d.Height) div 2（原文 582 / 610）→ 均为 +5 偏移
        Assert.Equal(new List<string> { "True:(-5,45,45,85)", "False:(45,45,95,85)" }, effectRects);
    }

    [Fact]
    public void PaintMagicBall_EffectCallback_FiresHpThenMp()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OverallSetting.Splite = -1;
        ball.OverallSetting.EffectHPMPStart = 0;
        ball.OverallSetting.EffectImageCount = 2;
        ball.OverallSetting.EffectPlayInterval = 100000;
        var flags = new List<bool>();
        ball.OnAfterDrawMagicBallEffectArea = (s, isHp, rect) => flags.Add(isHp);

        ball.PaintMagicBall();

        Assert.Equal(new List<bool> { true, false }, flags);
    }

    [Fact]
    public void PaintMagicBall_AreaCallback_NotFiredWhenNull()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.OnAfterDrawMagicBallArea = null;

        ball.PaintMagicBall();          // 不得抛

        Assert.Equal(4, p.Ops.Count);
    }

    [Fact]
    public void PaintMagicBall_AloneAreaCallback_IsHpFlagMatchesPath()
    {
        var pHp = new TDxRecordingPainter();
        var hpBall = MakeBall(TMagicBallType.mbtHP, pHp);
        var hpFlags = new List<bool>();
        hpBall.OnAfterDrawMagicBallArea = (s, isHp, rect) => hpFlags.Add(isHp);
        hpBall.PaintMagicBall();
        Assert.Equal(new List<bool> { true }, hpFlags);

        var pMp = new TDxRecordingPainter();
        var mpBall = MakeBall(TMagicBallType.mbtMP, pMp);
        var mpFlags = new List<bool>();
        mpBall.OnAfterDrawMagicBallArea = (s, isHp, rect) => mpFlags.Add(isHp);
        mpBall.PaintMagicBall();
        Assert.Equal(new List<bool> { false }, mpFlags);
    }

    [Fact]
    public void PaintMagicBall_EffectDrawBlend_FallsBackWithoutExtPainter()
    {
        // TDxRecordingPainter 未实现 IDxSurfacePainterExt → DxPainterExt 退化为普通 Draw
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHP, p);
        ball.AloneSetting.EffectStart = 0;
        ball.AloneSetting.EffectImageCount = 2;
        ball.AloneSetting.EffectPlayInterval = 100000;
        ball.AloneSetting.EffectDrawBlend = true;

        ball.PaintMagicBall();

        Assert.Equal(3, p.Ops.Count);
        Assert.StartsWith("Draw(", p.Ops[2]);   // 退化后仍是 Draw
    }

    [Fact]
    public void PaintMagicBall_EmptyIndexPresentFullMissing_DrawsOnlyEmpty()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHP, p);
        ball.AloneSetting.Full = -1;
        ball.AloneSetting.EffectStart = -1;

        ball.PaintMagicBall();

        Assert.Single(p.Ops);
    }

    [Fact]
    public void PaintMagicBall_ImageIndexOutOfRange_DrawsNothing()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHP, p);
        ball.AloneSetting.Empty = 0;
        ball.AloneSetting.Full = 99;    // 图库只有 5 张 → GetImage 返回 null
        ball.AloneSetting.EffectStart = -1;

        ball.PaintMagicBall();

        Assert.Single(p.Ops);           // 只有空底
    }

    [Fact]
    public void PaintMagicBall_ZeroWidthOrHeight_Aborts()
    {
        var p = new TDxRecordingPainter();
        var ball = MakeBall(TMagicBallType.mbtHPMP, p);
        ball.ClientRect = TDxRect.Rect(0, 0, 10, 0);   // 高 0

        ball.PaintMagicBall();

        Assert.Empty(p.Ops);
    }
}
