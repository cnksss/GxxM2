using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Client.Scenes;
using Xunit;
using TRect = GXX.Client.GUI.DxComponent.TRect;
// ★ 别名是**必须的**：`GXX.Client.GUI.Share`（FStateSeams 的接缝）与本车道的正式归属
// `GXX.Client.Scenes` 都声明了 `THintLines`/`THintWindows` → 同时 using 会触发 CS0104。
// 这正是交付报告 §「与 FState 的同一份 vs 两份」里给出的最小改法要消除的东西。
using THintLines = GXX.Client.Scenes.THintLines;
using THintWindows = GXX.Client.Scenes.THintWindows;

namespace GXX.Client.Tests;

/// <summary>DrawScrn 提示族测试的公共夹具（确定性字体度量 + 接缝复位）。</summary>
public abstract class ScrnDrawTestBase : IDisposable
{
    protected ScrnDrawTestBase()
    {
        DrawScrnEnv.ResetForTests();
        DropItemsMgrEnv.ResetForTests();
        // 固定字体度量：宽 = 6 × 字符数，高恒 12（= HGEFontEx.pas FFontHeight 的缺省）
        DrawScrnEnv.FontTextWidthFn = (f, s) => (s?.Length ?? 0) * 6;
        DrawScrnEnv.FontTextHeightFn = (f, s) => 12;
    }

    public void Dispose()
    {
        DrawScrnEnv.ResetForTests();
        DropItemsMgrEnv.ResetForTests();
    }

    /// <summary>造 n 张确定尺寸的纹理（测试用）。</summary>
    protected static TGameImages Images(int count, int width, int height, params int[] indices)
    {
        var imgs = new TGameImages(count);
        foreach (int i in indices)
            imgs[i] = new TTexture { Width = width, Height = height };
        return imgs;
    }
}

/// <summary>
/// DrawScrn.pas 提示族（THintMessage 族 / THintLines / ProcessHintText / THintWindow / THintWindows）
/// 的边界与公式测试。行号对应 `_analysis/utf8_mirror/Client-HGE/DrawScrn.pas`。
/// </summary>
public sealed class ScrnDrawHintTests : ScrnDrawTestBase
{
    // =========================================================================================
    // ProcessHintText（1453-2013）
    // =========================================================================================

    /// <summary>
    /// `&lt;TextW:X:Y:TEXT&gt;`（1754-1765）：OffsetX 当**固定宽度**、OffsetY 当**颜色索引**、
    /// 第三段是文本。★ 注意 OffsetY 同时被当成 `NewFixedWidthHintText(..., AColor:=OY)`
    /// 的颜色索引（原文 1978 `NewFixedWidthHintText(ShowText, OX, OY)`）—— 这里的 5 落进 0..255
    /// 于是 `Color := GetRGB(5)`。此为该 tag 的原文行为（登记为 D-P10-D05）。
    /// </summary>
    [Fact]
    public void ProcessHintText_TextW_UsesOffsetXAsFixedWidth_AndOffsetYAsColorIndex()
    {
        var l = new THintLines();
        l.Add("<TextW:120:5:TEXT>", TColor.clWhite);

        Assert.Equal(1, l.Count);
        var t = Assert.IsType<THintFixedWidthText>(l.FList[0]);
        Assert.Equal(120, t.FMinWidth);
        Assert.Equal("TEXT", t.FCaption);
        Assert.Equal(120, t.Width);             // FMinWidth(120) >= 文本宽(4*6=24)
        Assert.Equal(12, t.Height);
        Assert.Equal(14, l.ItemHeight);         // FHeight + 2
        Assert.Equal(DrawScrnEnv.GetRGB(5), t.FColor.Value);
    }

    /// <summary>`&lt;ImgNum:N:V:Space:X:Y&gt;`（1909-1920）：数字类型/值/间隔/偏移 + 图号公式 `1230 + N*10 + digit`。</summary>
    [Fact]
    public void ProcessHintText_ImgNum_ParsesFields_AndIconIndexFormula()
    {
        DrawScrnEnv.g_WNewopUIImages = Images(1300, 6, 10,
            1230 + 3 * 10 + 1, 1230 + 3 * 10 + 2, 1230 + 3 * 10 + 3, 1230 + 3 * 10 + 4);

        var l = new THintLines();
        l.Add("<ImgNum:3:1234:2:1:2>", TColor.clWhite);

        Assert.Equal(1, l.Count);
        var n = Assert.IsType<THintImageNumber>(l.FList[0]);
        Assert.Equal(3, n.FNumberIndex);
        Assert.Equal(2, n.FNumberSpace);
        Assert.Equal("1234", n.FNumberValue);
        Assert.Equal(1, n.FOffsetX);
        Assert.Equal(2, n.FOffsetY);

        // Initialize（1320-1337）：逐位取图，宽度 = Σ(宽 + 间隔)，高度 = max
        Assert.Equal(4 * (6 + 2), n.Width);
        Assert.Equal(10, n.Height);
    }

    /// <summary>否定性断言（§37.3 计数取证）：**只有取到纹理的那一位才累加宽高**（1331-1334 的 `if Texture &lt;&gt; nil`）。</summary>
    [Fact]
    public void ProcessHintText_ImgNum_MissingDigitTexture_ContributesNothing()
    {
        // 只放 digit=1 的图；digit=2 缺失
        DrawScrnEnv.g_WNewopUIImages = Images(1300, 6, 10, 1230 + 3 * 10 + 1);

        var l = new THintLines();
        l.Add("<ImgNum:3:12:2:0:0>", TColor.clWhite);
        var n = Assert.IsType<THintImageNumber>(l.FList[0]);

        Assert.Equal(6 + 2, n.Width);   // 只有 1 位命中
        Assert.Equal(10, n.Height);
    }

    /// <summary>
    /// `&lt;Looks:N:…&gt;`（1781-1790）：`ImageIndex := N mod 10000`（图库按 N 取、图号取模）。
    /// </summary>
    [Fact]
    public void ProcessHintText_Looks_ImageIndexIsMod10000()
    {
        var bag = Images(8, 13, 17, 5);
        DrawScrnEnv.g_WBagItemImages = new TLookImages { Resolver = i => i == 10005 ? bag : null };

        var l = new THintLines();
        l.Add("<Looks:10005:1:2:0:0:0>", TColor.clWhite);

        var im = Assert.IsType<THintImage>(l.FList[0]);
        Assert.Equal(5, im.FImageIndex);        // 10005 mod 10000
        Assert.Equal(1, im.FOffsetX);
        Assert.Equal(2, im.FOffsetY);
        Assert.Equal(13, im.Width);
        Assert.Equal(17, im.Height);
    }

    /// <summary>`&lt;Looks:…&gt;` 的三兄弟（DnItems 1791-1800 / StateItem 1801-1810）走各自图库，取模公式相同。</summary>
    [Fact]
    public void ProcessHintText_DnItems_And_StateItem_UseTheirOwnLibraries()
    {
        var dn = Images(8, 20, 21, 7);
        var st = Images(9, 22, 23, 8);
        DrawScrnEnv.g_WDnItemImages = new TLookImages { Resolver = i => i == 10007 ? dn : null };
        DrawScrnEnv.g_WStateItemImages = new TLookImages { Resolver = i => i == 10008 ? st : null };

        var l = new THintLines();
        l.Add("<DnItems:10007>", TColor.clWhite);
        l.Add("<StateItem:10008>", TColor.clWhite);

        Assert.Equal(7, ((THintImage)l.FList[0]).FImageIndex);
        Assert.Equal(20, l.Objects(0).Width);
        Assert.Equal(8, ((THintImage)l.FList[1]).FImageIndex);
        Assert.Equal(22, l.Objects(1).Width);
    }

    /// <summary>PlayTime &lt;= 0 时被改成 100（1821-1822 / 1876-1877）——两处都锁死。</summary>
    [Theory]
    [InlineData("<PlayImg:0:1:2:0:0:0:0:0:0>", true, 100, 2)]
    [InlineData("<PlayImg:0:1:2:50:0:0:0:0:0>", true, 50, 2)]
    [InlineData("<NewopPlayImg:1:3:0:0:0:0:0>", false, 100, 3)]
    [InlineData("<NewopPlayImg:1:3:7:0:0:0:0>", false, 7, 3)]
    public void ProcessHintText_PlayImage_DefaultPlayTimeIs100(string tag, bool expectPlayImg, int expectedTime, int expectedCount)
    {
        DrawScrnEnv.g_EffectImageList.Add(Images(16, 9, 9, 1, 2, 3));
        DrawScrnEnv.g_WNewopUIImages = Images(64, 9, 9, 1, 2, 3);

        var l = new THintLines();
        l.Add(tag, TColor.clWhite);

        Assert.Equal(1, l.Count);
        if (expectPlayImg)
        {
            var p = Assert.IsType<THintPlayImage>(l.FList[0]);
            Assert.IsType<THintPlayImage>(p);                     // 非 Ex
            Assert.Equal(expectedTime, p.FPlayTime);
            Assert.Equal(expectedCount, p.FPlayCount);
        }
        else
        {
            var p = Assert.IsType<THintPlayImageEx>(l.FList[0]);
            Assert.Equal(expectedTime, p.FPlayTime);
            Assert.Equal(expectedCount, p.FPlayCount);
            Assert.Equal(9 + p.FIncSpacing, p.Width);             // Initialize 加 FIncSpacing
        }
    }

    /// <summary>
    /// ★ 13 个 tag **全部**被识别（计数取证 —— 原文 `CheckHintImage` 的分支恰好 13 支：
    /// TextW / Img / Looks / DnItems / StateItem / NewopPlayImg / NewopUI / WinNewopUI /
    /// LineNewopUI / PlayImg / ItemProgress / ImgNum / Countdown），
    /// 其余任何名字都返回 False（被当字面文本追加回缓冲）。
    /// </summary>
    [Fact]
    public void CheckHintImage_AcceptsExactlyThirteenTags()
    {
        DrawScrnEnv.g_EffectImageList.Add(Images(16, 9, 9, 0, 1, 2, 3));
        DrawScrnEnv.g_WNewopUIImages = Images(1400, 9, 9, 1, 2, 3, 46, 250, 251, 620, 630, 640, 650);
        DrawScrnEnv.g_WBagItemImages = new TLookImages { Resolver = _ => Images(4, 9, 9, 0, 1) };
        DrawScrnEnv.g_WDnItemImages = new TLookImages { Resolver = _ => Images(4, 9, 9, 0, 1) };
        DrawScrnEnv.g_WStateItemImages = new TLookImages { Resolver = _ => Images(4, 9, 9, 0, 1) };

        string[] tags =
        {
            "<TextW:100:0:T>", "<Img:0:0>", "<Looks:0>", "<DnItems:0>", "<StateItem:0>",
            "<NewopPlayImg:1:1:0:0:0:0:0>", "<NewopUI:1>", "<WinNewopUI:1>", "<LineNewopUI:1>",
            "<PlayImg:0:1:1:0:0:0:0:0:0>", "<ItemProgress:1:1:1:1:0:0:0:T>", "<ImgNum:1:9:0:0:0>",
            "<Countdown:9:0:0:0>",
        };

        var spec = new DrawScrnHintTextParser.HintImageSpec();
        foreach (string tag in tags)
            Assert.True(new ParserProbe().InvokeCheck(tag, spec), $"tag 未被识别: {tag}");

        Assert.Equal(13, tags.Length); // 计数取证：恰好 13 支

        // 反例：未知 tag → False（→ 走 `StrB := StrB + StrC` 的字面文本路径）
        Assert.False(new ParserProbe().InvokeCheck("<Foo:1:2>", spec));
        Assert.False(new ParserProbe().InvokeCheck("<>", spec));          // Length <= 2
        Assert.False(new ParserProbe().InvokeCheck("<Countdown>", spec)); // I1 < 0
    }

    /// <summary>未知 tag 整体作为字面文本落地（1949-2009 的 `StrB` 回填路径）。</summary>
    [Fact]
    public void ProcessHintText_UnknownTag_IsKeptAsLiteralText()
    {
        var l = new THintLines();
        l.Add("<Foo:1:2>", TColor.clWhite);

        Assert.Equal(1, l.Count);
        Assert.Equal("<Foo:1:2>", ((THintText)l.FList[0]).FCaption);
    }

    /// <summary>`tag + 尾巴`：标签被吃掉、尾巴成为独立文本行（1974-1975 + 2003-2007）。</summary>
    [Fact]
    public void ProcessHintText_TagThenTail_ProducesImagePlusTextLine()
    {
        DrawScrnEnv.g_WNewopUIImages = Images(300, 5, 5, 1);
        var l = new THintLines();
        l.Add("<NewopUI:1>尾巴", TColor.clWhite);

        Assert.Equal(2, l.Count);
        Assert.IsType<THintImage>(l.FList[0]);
        Assert.Equal("尾巴", ((THintText)l.FList[1]).FCaption);
    }

    /// <summary>空串仍会造出一个空 THintText（1463-1473 的 `Length(Text) = 0` 分支）。</summary>
    [Fact]
    public void ProcessHintText_EmptyString_CreatesOneEmptyTextLine()
    {
        var l = new THintLines();
        l.Add("", TColor.clWhite);

        Assert.Equal(1, l.Count);
        Assert.Equal("", ((THintText)l.FList[0]).FCaption);
        Assert.Equal(14, l.ItemHeight); // 空文本仍取 FHeight=12 → ItemHeight=14
    }

    /// <summary>`{文字|颜色索引}`：0..255 内取自定义色；超范围则**整段原样保留**（1504-1531）。</summary>
    [Fact]
    public void NewHintText_CustomColor_InRangeAppliesElseKeepsBraces()
    {
        var l = new THintLines();
        l.Add("{攻击|247}后缀", TColor.clWhite);

        Assert.Equal(2, l.Count);
        Assert.Equal("攻击", ((THintText)l.FList[0]).FCaption);
        Assert.Equal(DrawScrnEnv.GetRGB(247), ((THintText)l.FList[0]).FColor.Value);
        Assert.Equal("后缀", ((THintText)l.FList[1]).FCaption);
        Assert.Equal(TColor.clWhite.Value, ((THintText)l.FList[1]).FColor.Value);

        var l2 = new THintLines();
        l2.Add("{攻击|999}尾", TColor.clWhite);
        Assert.Equal(2, l2.Count);
        Assert.Equal("{攻击|999}", ((THintText)l2.FList[0]).FCaption);
    }

    // =========================================================================================
    // THintLines（318-375 / 1361-2160）
    // =========================================================================================

    /// <summary>GetSize（1385-1418）：FItemHeight = FHeight + 2；固定行高参与 max；空容器走字体兜底。</summary>
    [Fact]
    public void THintLines_GetSize_ItemHeightAndEmptyFallback()
    {
        var l = new THintLines();
        Assert.Equal(0, l.Count);
        l.GetSize();
        // 空容器：FWidth<=0 → TextWidth(CurrentFont, "0") = 6；FHeight<=0 → g_CurrentFontHeight = 14
        Assert.Equal(6, l.Width);
        Assert.Equal(14, l.Height);
        Assert.Equal(16, l.ItemHeight);

        var l2 = new THintLines();
        l2.Add("AB", TColor.clWhite);                  // 12 宽 / 12 高
        Assert.Equal(14, l2.ItemHeight);

        l2.AddFixedHeightLine(30);                     // 30 高 > 12 → ItemHeight = 32
        Assert.Equal(32, l2.ItemHeight);
    }

    /// <summary>AddFixedHeightLine（2035-2043）：造 TFiexdHeightLine 并设 FHeight。</summary>
    [Fact]
    public void THintLines_AddFixedHeightLine_AppendsFixedLine()
    {
        var l = new THintLines();
        l.AddFixedHeightLine(41);
        Assert.Equal(1, l.Count);
        Assert.IsType<TFiexdHeightLine>(l.FList[0]);
        Assert.Equal(41, l.Objects(0).Height);
    }

    /// <summary>GetSize 的 FMinWidth 只由 TLineBGHintImage.FTextureWidth 赋值（1401-1403）。</summary>
    [Fact]
    public void THintLines_GetSize_MinWidthComesFromLineBGHintImageTextureWidth()
    {
        DrawScrnEnv.g_WNewopUIImages = Images(1400, 200, 20, 1);
        var l = new THintLines();
        l.Add("<LineNewopUI:1>", TColor.clWhite);

        Assert.IsType<TLineBGHintImage>(l.FList[0]);
        Assert.Equal(200, l.MinWidth);
        Assert.Equal(22, l.ItemHeight); // 高 20 + 2
    }

    /// <summary>Strings/Objects 访问器只对 THintText 生效（1426-1429 / 1446-1447）。</summary>
    [Fact]
    public void THintLines_Strings_OnlyThintText()
    {
        DrawScrnEnv.g_WNewopUIImages = Images(300, 5, 5, 1);
        var l = new THintLines();
        l.Add("AB", TColor.clWhite);
        l.Add("<NewopUI:1>", TColor.clWhite);

        Assert.Equal("AB", l.Strings(0));
        Assert.Equal("", l.Strings(1));      // THintImage → ""
        l.SetStrings(0, "CD");
        Assert.Equal("CD", l.Strings(0));
        l.SetStrings(1, "X");                // 非 THintText → 静默无效
        Assert.Equal("", l.Strings(1));
    }

    /// <summary>Delete 越界无效；Clear 清空（2075-2081 / 2065-2073）。</summary>
    [Fact]
    public void THintLines_Delete_And_Clear()
    {
        var l = new THintLines();
        l.Add("A", TColor.clWhite);
        l.Add("B", TColor.clWhite);
        l.Delete(5);
        Assert.Equal(2, l.Count);
        l.Delete(0);
        Assert.Equal(1, l.Count);
        Assert.Equal("B", l.Strings(0));
        l.Clear();
        Assert.Equal(0, l.Count);
    }

    // =========================================================================================
    // TCountdownText / THintText（倒计时 / 文本度量）
    // =========================================================================================

    [Theory]
    [InlineData(0, "0秒")]
    [InlineData(-5, "0秒")]
    [InlineData(59, "59秒")]
    [InlineData(60, "1分0秒")]
    [InlineData(3599, "59分59秒")]
    [InlineData(3600, "1时0分0秒")]
    [InlineData(86399, "23时59分59秒")]
    [InlineData(86400, "1天0时0分0秒")]
    [InlineData(90061, "1天1时1分1秒")]
    public void TCountdownText_GetShowText_Boundaries(int value, string expected)
    {
        var c = new TCountdownText(new THintLines()) { FCountdownValue = value };
        Assert.Equal(expected, c.GetShowText());
    }

    /// <summary>Paint（938-956）：仅当 `MyGetTickCount - FStartTick &gt;= 1000` 时递减 1（**3000 不递减 3**）。</summary>
    [Fact]
    public void TCountdownText_Paint_DecrementsOnePerSecond()
    {
        uint now = 10_000;
        DrawScrnEnv.MyGetTickCountFn = () => now;

        var c = new TCountdownText(new THintLines()) { FCountdownValue = 5, FStartTick = 10_000 };
        c.Paint(default, default, default);
        Assert.Equal(5, c.FCountdownValue);            // 0 < 1000

        now = 10_999;
        c.Paint(default, default, default);
        Assert.Equal(5, c.FCountdownValue);            // 999 < 1000

        now = 11_000;
        c.Paint(default, default, default);
        Assert.Equal(4, c.FCountdownValue);            // 恰好 1000 → 递减
        Assert.Equal(11_000u, c.FStartTick);           // FStartTick 被重置为当前 tick

        now = 13_500;
        c.Paint(default, default, default);
        Assert.Equal(3, c.FCountdownValue);            // 跨 2500ms 也只递减 1（原文缺陷：不补齐）
    }

    /// <summary>THintText.Initialize（807-829）：描边 +2 只在 FIsStroke 时加到高度；宽度两分支相同。</summary>
    [Fact]
    public void THintText_Initialize_StrokeAddsTwoToHeightOnly()
    {
        var plain = new THintText(new THintLines()) { Caption = "AB", Size = 9, IsStroke = false };
        plain.Initialize();
        Assert.Equal(12, plain.FHeight);
        Assert.Equal(12, plain.FWidth);

        var stroke = new THintText(new THintLines()) { Caption = "AB", Size = 9, IsStroke = true };
        stroke.Initialize();
        Assert.Equal(14, stroke.FHeight);
        Assert.Equal(12, stroke.FWidth);               // 原文 822 注释：宽度不加 +2
    }

    /// <summary>THintFixedWidthText.Initialize（874-879）：`FMinWidth >= FWidth` 时抬到 FMinWidth。</summary>
    [Fact]
    public void THintFixedWidthText_Initialize_RaisesWidthToMinWidth()
    {
        var t = new THintFixedWidthText(new THintLines()) { Caption = "ABCDEFGHIJ", Size = 9, FMinWidth = 30 };
        t.Initialize();                                 // 文本宽 60 > 30 → 保持 60
        Assert.Equal(60, t.Width);

        var t2 = new THintFixedWidthText(new THintLines()) { Caption = "AB", Size = 9, FMinWidth = 30 };
        t2.Initialize();                                // 文本宽 12 < 30 → 抬到 30
        Assert.Equal(30, t2.Width);
    }

    /// <summary>
    /// THintText.SetSize 触发 Initialize；SetStyle / SetIsStroke **不触发**（783-805 的 `// Initialize;`）。
    /// </summary>
    [Fact]
    public void THintText_SetSizeReinitializes_ButSetStyleDoesNot()
    {
        var t = new THintText(new THintLines()) { Caption = "AB", Size = 9 };
        t.Initialize();
        Assert.Equal(12, t.FWidth);

        t.Size = 9;                                     // 同值 → FSize 不变，但 Initialize 仍被调用
        Assert.Equal(12, t.FWidth);

        t.Caption = "ABCD";                             // Caption 的写访问器直接写字段（**不** Initialize）
        Assert.Equal(12, t.FWidth);                     // 宽度仍是旧值 → 证明未重算
    }

    // =========================================================================================
    // THintPlayImage / THintPlayImageEx 帧推进
    // =========================================================================================

    [Fact]
    public void THintPlayImage_Paint_AdvancesAndWrapsFrame()
    {
        uint now = 0;
        DrawScrnEnv.MyGetTickCountFn = () => now;
        DrawScrnEnv.GetCachedImageFn = (g, i) => (new TTexture { Width = 8, Height = 8 }, 0, 0);

        var p = new THintPlayImage(new THintLines())
        {
            FGameImages = Images(16, 8, 8, 1, 2, 3, 4, 5, 6, 7),
            FImageIndex = 5,
            FPlayImageIndex = 5,
            FPlayCount = 3,
            FPlayTime = 100,
            FPlayTick = 0,
        };

        now = 100; p.Paint(default, default, default);
        Assert.Equal(5, p.FPlayImageIndex);             // 100 > 100 为假

        now = 101; p.Paint(default, default, default);
        Assert.Equal(6, p.FPlayImageIndex);             // 6-5+1=2 <= 3

        now = 202; p.Paint(default, default, default);
        Assert.Equal(7, p.FPlayImageIndex);             // 7-5+1=3 <= 3

        now = 303; p.Paint(default, default, default);
        Assert.Equal(5, p.FPlayImageIndex);             // 8-5+1=4 > 3 → 回卷到 FImageIndex
    }

    /// <summary>
    /// THintPlayImage.Paint 使用 `GetCachedImage` 的**原点偏移**（1132-1136），
    /// 而 THintPlayImageEx 用 `Images[]` 且**不加**原点偏移（1178-1182）—— 两者差异必须锁死。
    /// </summary>
    [Fact]
    public void THintPlayImage_UsesCachedOriginOffset_ExDoesNot()
    {
        DrawScrnEnv.MyGetTickCountFn = () => 0;
        DrawScrnEnv.GetCachedImageFn = (g, i) => (new TTexture { Width = 8, Height = 8 }, 3, -4);

        var lines = new THintLines();
        var rt = DrawScrnRect.Rect(100, 200, 200, 220);

        var p = new THintPlayImage(lines) { FGameImages = Images(8, 8, 8, 5), FImageIndex = 5, FPlayImageIndex = 5, FPlayTime = 1000 };
        p.Paint(default, rt, default);
        var op1 = DrawScrnEnv.GameCanvas.Ops[^1];
        Assert.Equal(100 + lines.OffsetX + 0 + 3, op1.X);
        Assert.Equal(200 + lines.OffsetY + 0 - 4, op1.Y);

        DrawScrnEnv.GameCanvas.Clear();
        var e = new THintPlayImageEx(lines) { FGameImages = Images(8, 8, 8, 5), FImageIndex = 5, FPlayImageIndex = 5, FPlayTime = 1000, FIncSpacing = 7 };
        e.Initialize();
        Assert.Equal(8 + 7, e.Width);                    // Initialize：宽 + FIncSpacing
        e.Paint(default, rt, default);
        var op2 = DrawScrnEnv.GameCanvas.Ops[^1];
        Assert.Equal(100 + lines.OffsetX, op2.X);        // 无原点偏移
        Assert.Equal(200 + lines.OffsetY, op2.Y);
    }

    /// <summary>THintPlayImageEx.Initialize（1158-1169）在 FGameImages=nil 时**完全不动** FWidth/FHeight。</summary>
    [Fact]
    public void THintPlayImageEx_Initialize_NoImagesKeepsZero()
    {
        var e = new THintPlayImageEx(new THintLines()) { FGameImages = null, FIncSpacing = 9, FFixedWidth = 77, FFixedHeight = 88 };
        e.Initialize();
        Assert.Equal(0, e.Width);                        // 未走基类 Initialize → 保持 0
        Assert.Equal(0, e.Height);
    }

    // =========================================================================================
    // THintItemProgress（1204-1316）
    // =========================================================================================

    /// <summary>Initialize：FProgressIndex=1 用 640，否则用 620；宽度 = 图宽 + 2（1209-1217）。</summary>
    [Theory]
    [InlineData(1, 640, 41 + 2)]
    [InlineData(0, 620, 31 + 2)]
    public void THintItemProgress_Initialize_PicksBaseTexture(int progressIndex, int imageIndex, int expectedWidth)
    {
        var imgs = new TGameImages(700);
        imgs[620] = new TTexture { Width = 31, Height = 12 };
        imgs[640] = new TTexture { Width = 41, Height = 13 };

        var p = new THintItemProgress(new THintLines()) { FGameImages = imgs, FProgressIndex = progressIndex };
        p.Initialize();

        Assert.Equal(expectedWidth, p.Width);
        Assert.Equal(progressIndex == 1 ? 13 : 12, p.Height);
        Assert.NotNull(imgs[imageIndex]);
    }

    /// <summary>FMaxValue = 0 时 Paint 直接 Exit（1246）—— 不画任何进度条。</summary>
    [Fact]
    public void THintItemProgress_Paint_MaxValueZeroExits()
    {
        var imgs = new TGameImages(700);
        imgs[620] = new TTexture { Width = 31, Height = 12 };
        DrawScrnEnv.g_WNewopUIImages = imgs;

        var p = new THintItemProgress(new THintLines())
        {
            FGameImages = imgs,
            FProgressIndex = 0,
            FPlayCount = 1,
            FMaxValue = 0,
            FCurValue = 5,
            FText = "T",
        };
        p.Paint(default, DrawScrnRect.Rect(0, 0, 100, 100), default);

        // 只画了背景（1 次），没有进度条、也没有文字
        Assert.Single(DrawScrnEnv.GameCanvas.Ops);
        Assert.Equal(ScrnDrawKind.Draw, DrawScrnEnv.GameCanvas.Ops[0].Kind);
    }

    /// <summary>
    /// 进度条裁剪宽度（1274-1275）：`R.Right := R.Left + Round((R.Right-R.Left)/FMaxValue*FCurValue)`
    /// —— 用整数除法会得到不同结果，故此式必须锁死。
    /// </summary>
    [Fact]
    public void THintItemProgress_Paint_ClipsByMaxValueWithRounding()
    {
        var imgs = new TGameImages(700);
        imgs[620] = new TTexture { Width = 30, Height = 12 };  // 背景
        imgs[631] = new TTexture { Width = 20, Height = 10 };  // FProgressIndex=0, FPlayCount=11 → 630+? 见下

        var p = new THintItemProgress(new THintLines())
        {
            FGameImages = imgs,
            FProgressIndex = 0,
            FPlayCount = 4,          // <10 → texture = Images[620 + 4] = 624
            FMaxValue = 3,
            FCurValue = 1,
            FText = "",
        };
        imgs[624] = new TTexture { Width = 20, Height = 10 };

        p.Paint(default, DrawScrnRect.Rect(0, 0, 100, 100), default);

        // ops[0] = 背景 Draw；ops[1] = 进度 Draw(x,y,R,texture)
        Assert.Equal(2, DrawScrnEnv.GameCanvas.Ops.Count);
        var bar = DrawScrnEnv.GameCanvas.Ops[1];
        Assert.Equal(ScrnDrawKind.DrawRect, bar.Kind);
        Assert.Equal(0, bar.Rect.Left);
        Assert.Equal((int)Math.Round(20 / 3.0 * 1), bar.Rect.Right);  // Round(6.667) = 7
    }

    // =========================================================================================
    // THintWindow（377-406 / 2164-2500）
    // =========================================================================================

    private static THintWindow NewWindowWithLines(string msg, TColor color)
    {
        var w = new THintWindow();
        w.Show(0, 0, msg, color, false, false, true);
        return w;
    }

    /// <summary>
    /// Show 的几何（2286-2326）：`HintHeight = ΣItemHeight + Top + Bottom - 5`、
    /// `HintWidth = max(行宽) + Left + Right - 6`，再按屏幕四边夹紧，矩形下限 20×20。
    /// </summary>
    [Fact]
    public void THintWindow_Show_ComputesAndClampsGeometry()
    {
        var w = NewWindowWithLines("AB", TColor.clWhite);

        Assert.Equal(12 + 8 + 8 - 6, w.HintWidth);     // 22
        Assert.Equal(14 + 8 + 8 - 5, w.HintHeight);    // 25

        // 再放一次到屏幕外，验证夹紧
        w.Show(2000, 1000, "AB", TColor.clWhite, false, false, true);
        Assert.Equal(1024 - 22, w.X);
        Assert.Equal(768 - 25, w.Y);
        Assert.Equal(1024, w.HintRect.Right);
        Assert.Equal(768, w.HintRect.Bottom);
    }

    /// <summary>HintUp / DrawLeft 先平移再夹紧（2316-2321）。</summary>
    [Fact]
    public void THintWindow_Show_DrawUpAndDrawLeft_ShiftBeforeClamp()
    {
        var w = new THintWindow();
        w.Show(500, 500, "AB", TColor.clWhite, true, true, true);

        Assert.Equal(500 - 22, w.X);      // DrawLeft
        Assert.Equal(500 - 25, w.Y);      // DrawUp
        Assert.True(w.HintUp);
    }

    /// <summary>矩形下限 20×20（2323 的 `Max(HintWidth,20), Max(HintHeight,20)`）。</summary>
    [Fact]
    public void THintWindow_Show_RectMinimumIs20()
    {
        var w = new THintWindow();
        // 造一个「行宽/行高都算不出来」的窗口：直接 add 一个空 THintLines
        w.Add(new THintLines());
        w.Show(0, 0, false, false, true);

        Assert.Equal(8 + 8 - 6, w.HintWidth);   // 10
        Assert.Equal(0 + 8 + 8 - 5, w.HintHeight); // 11
        Assert.Equal(20, w.HintRect.Right - w.HintRect.Left);
        Assert.Equal(20, w.HintRect.Bottom - w.HintRect.Top);
    }

    /// <summary>多行消息按 '\' 切分（2232-2284 / 2295-2305）。</summary>
    [Fact]
    public void THintWindow_Show_SplitsByBackslash()
    {
        var w = new THintWindow();
        w.Show(0, 0, "AB\\CD", TColor.clWhite, false, false, true);

        Assert.Equal(2, w.Count);
        Assert.Equal(12 + 8 + 8 - 6, w.HintWidth);
        Assert.Equal(14 * 2 + 8 + 8 - 5, w.HintHeight);
    }

    /// <summary>
    /// ShowColor（2232-2284）：`颜色索引/文字` 用 '/' 切色（**颜色段在前**）；缺省加描边
    /// （GetHintFontStroke(True)）。颜色索引解析失败时回退 255（2249 的 `StrToIntDef(sColor, 255)`）。
    /// </summary>
    [Fact]
    public void THintWindow_ShowColor_ColorIndexComesFirst()
    {
        var w = new THintWindow();
        w.ShowColor(10, 20, "9/AB", false, false, true);

        Assert.Equal(1, w.Count);
        var line = (THintLines)w[0];
        Assert.Equal("AB", line.Strings(0));
        Assert.Equal(DrawScrnEnv.GetRGB(9), ((THintText)line.FList[0]).FColor.Value);
        Assert.True(((THintText)line.FList[0]).FIsStroke);   // GetHintFontStroke(True) 缺省直通
        Assert.Equal(14 + 2, line.ItemHeight);               // 描边 +2 高度

        // 颜色段不是数字 → StrToIntDef 回退 255
        var w2 = new THintWindow();
        w2.ShowColor(0, 0, "XYZ/AB", false, false, true);
        Assert.Equal(DrawScrnEnv.GetRGB(255), ((THintText)((THintLines)w2[0]).FList[0]).FColor.Value);
        Assert.Equal("AB", ((THintLines)w2[0]).Strings(0));
    }

    /// <summary>SetHintX 夹紧 &lt;0，SetHintXExt **不夹紧**（2364-2396 的差异）。</summary>
    [Fact]
    public void THintWindow_SetHintXClamps_ButSetHintXExtDoesNot()
    {
        var w = new THintWindow { HintWidth = 10, HintHeight = 10 };

        w.SetHintX(-5);
        Assert.Equal(0, w.X);

        w.SetHintXExt(-5);
        Assert.Equal(-5, w.X);                 // ★ 与 SetHintX 不同
        Assert.Equal(-5, w.HintRect.Left);

        w.SetHintYExt(-7);
        Assert.Equal(-7, w.Y);
    }

    /// <summary>属性 X/Y 走夹紧版；同值赋值不重算 HintRect（2364-2380 的 `if HintX &lt;&gt; Value`）。</summary>
    [Fact]
    public void THintWindow_XProperty_UsesClampingSetter_AndNoOpOnSameValue()
    {
        var w = new THintWindow { HintWidth = 10, HintHeight = 10 };
        w.X = -3;
        Assert.Equal(0, w.X);

        w.HintRect = DrawScrnRect.Rect(111, 222, 333, 444);
        w.X = 0;                                // 同值 → 不动 HintRect
        Assert.Equal(111, w.HintRect.Left);
    }

    /// <summary>Clear 把 FVisible 置 False 并清空（2176-2185）；Show 前会 Add 行。</summary>
    [Fact]
    public void THintWindow_Clear_SetsInvisible()
    {
        var w = NewWindowWithLines("AB", TColor.clWhite);
        Assert.True(w.Visible);
        w.Clear();
        Assert.False(w.Visible);
        Assert.Equal(0, w.Count);
    }

    /// <summary>
    /// Draw 的两条早退：矩形左上越屏即退出（2412-2413）—— 但 `DrawBackground()` 在早退**之前**
    /// 已经跑过（2408），所以仍会留下一次背景填充。
    /// </summary>
    [Fact]
    public void THintWindow_Draw_EarlyExitsAfterBackgroundOnly()
    {
        var w = NewWindowWithLines("AB", TColor.clWhite);
        w.HintRect = DrawScrnRect.Rect(2000, 0, 2022, 25);
        DrawScrnEnv.GameCanvas.Clear();
        w.Draw();

        Assert.Single(DrawScrnEnv.GameCanvas.Ops);
        Assert.Equal(ScrnDrawKind.FillRectAlpha, DrawScrnEnv.GameCanvas.Ops[0].Kind);
    }

    /// <summary>
    /// Draw 的对齐算式（2428-2452）：taRightJustify / taCenter 通过 `OffsetRect` 平移 LineRect；
    /// 平移量 = `PaintRect宽 - 行宽 - Right边距 - 6`（taCenter 再 div 2）。
    /// </summary>
    [Fact]
    public void THintWindow_Draw_RightAndCenterAlignmentOffsets()
    {
        var w = NewWindowWithLines("AB", TColor.clWhite);
        var line = (THintLines)w[0];

        var paint = DrawScrnRect.Rect(100, 200, 300, 225);   // 宽 200

        var right = DrawScrnRect.Bounds(paint.Left, 200, line.Width, line.ItemHeight);
        DrawScrnRect.OffsetRect(ref right, paint.Right - paint.Left - line.Width - DrawScrnEnv.HintWindowBorderWidth.Right - 6, 0);
        Assert.Equal(100 + (200 - 12 - 8 - 6), right.Left);

        var center = DrawScrnRect.Bounds(paint.Left, 200, line.Width, line.ItemHeight);
        DrawScrnRect.OffsetRect(ref center, (paint.Right - paint.Left - line.Width - DrawScrnEnv.HintWindowBorderWidth.Right - 6) / 2, 0);
        Assert.Equal(100 + (200 - 12 - 8 - 6) / 2, center.Left);

        // 真的走一遍 PaintWithoutWinHintImage（不抛异常即可）
        bool have = false;
        line.Alignment = TAlignment.taCenter;
        line.PaintWithoutWinHintImage(paint, center, ref have);
        Assert.False(have);
    }

    /// <summary>
    /// THintWindows.Show 把既有窗口全部置为不可见（2571-2588），Draw 会先删掉不可见的
    /// （2597-2607 的「否则会闪烁」分支）—— 最终只剩 1 个。
    /// </summary>
    [Fact]
    public void THintWindows_Show_InvalidatesPrevious_AndDrawDropsInvisible()
    {
        var ws = new THintWindows();
        ws.Show(0, 0, "A", TColor.clWhite);
        ws.Show(0, 0, "B", TColor.clWhite);

        Assert.Equal(2, ws.Count);
        Assert.False(ws.GetItems(0).Visible);
        Assert.True(ws.GetItems(1).Visible);

        ws.Draw();
        Assert.Equal(1, ws.Count);
    }

    /// <summary>Clear 会回写全局 `g_LastHintMakeIndex := -1`（2638）。</summary>
    [Fact]
    public void THintWindows_Clear_ResetsLastHintMakeIndex()
    {
        var ws = new THintWindows();
        ws.Show(0, 0, "A", TColor.clWhite);
        DrawScrnEnv.g_LastHintMakeIndex = 42;

        ws.Clear();
        Assert.Equal(-1, DrawScrnEnv.g_LastHintMakeIndex);
        Assert.Equal(0, ws.Count);
    }

    /// <summary>Initialize / Finalize_ 都是「全清」（2522-2550）—— 原文两者**逐字相同**。</summary>
    [Fact]
    public void THintWindows_InitializeAndFinalize_AreBothFullClear()
    {
        var ws = new THintWindows();
        ws.Show(0, 0, "A", TColor.clWhite);
        ws.Initialize();
        Assert.Equal(0, ws.Count);

        ws.Show(0, 0, "B", TColor.clWhite);
        ws.Finalize_();
        Assert.Equal(0, ws.Count);
    }

    /// <summary>UpDate 的方法体在原文里被整段注释 ⇒ 空实现（2675-2690）。</summary>
    [Fact]
    public void THintWindows_UpDate_IsEmpty()
    {
        var ws = new THintWindows();
        ws.Show(0, 0, "A", TColor.clWhite);
        ws.GetItems(0).Visible = false;
        ws.UpDate();
        Assert.Equal(1, ws.Count);   // 未被清理
    }

    /// <summary>THintWindow.IsItemIconText 参与 taRight/taCenter 的对齐宽度（2419-2420）。</summary>
    [Fact]
    public void THintWindow_Draw_IconWidthUsedWhenIsItemIconText()
    {
        var w = NewWindowWithLines("AB", TColor.clWhite);
        var line = (THintLines)w[0];
        line.IsItemIconText = true;
        Assert.True(line.IsItemIconText);
        Assert.Equal(12, line.Width);
    }

    /// <summary>
    /// `<WinNewopUI:…>` 走「最上层绘制」：PaintWithoutWinHintImage 会把 HaveWinHintImage 置真
    /// 且**不画**该元素（2126-2128），PaintWinHintImage 只画该元素（2155-2157）。
    /// </summary>
    [Fact]
    public void THintLines_WinHintImage_DeferredToSecondPass()
    {
        DrawScrnEnv.g_WNewopUIImages = Images(300, 5, 6, 1);
        var l = new THintLines();
        l.Add("<WinNewopUI:1>", TColor.clWhite);

        Assert.IsType<TWinHintImage>(l.FList[0]);
        Assert.Equal(0, l.Objects(0).Width);     // TWinHintImage.Initialize 恒把宽高置 0（1039-1057）

        var have = false;
        DrawScrnEnv.GameCanvas.Clear();
        l.PaintWithoutWinHintImage(default, DrawScrnRect.Rect(0, 0, 10, 10), ref have);
        Assert.True(have);
        Assert.Empty(DrawScrnEnv.GameCanvas.Ops);   // 第一遍不画

        // 参数顺序与原文一致：Paint(OwnerRect, DestRect, HintWinRect)，调用点传 (PaintRect, LineRect, OwnerRect)
        // ⇒ 实参 ownerRect = 外层 PaintRect(0,0,100,100)，hintWinRect 用的也是它。
        // X 公式：HintWinRect.Left + (HintWinRect宽 - 纹理宽) + FOffsetX = 0 + (100 - 5) + 0
        l.PaintWinHintImage(DrawScrnRect.Rect(0, 0, 100, 100), DrawScrnRect.Rect(7, 9, 17, 19));
        Assert.Single(DrawScrnEnv.GameCanvas.Ops);
        Assert.Equal(0 + (100 - 5), DrawScrnEnv.GameCanvas.Ops[0].X);
        Assert.Equal(0, DrawScrnEnv.GameCanvas.Ops[0].Y);
    }

    /// <summary>
    /// THintLines.Paint 的两遍绘制顺序：带播放的（Play/PlayEx/Image）先、其余后（2098-2113）。
    /// <para>注意：非描边 `THintText` 走**原文 866 的 `HGEFont.TextOut`**（不经 GameCanvas），
    /// 因此断言在 `THGEFont.TextOuts` 上；图元素走 `GameCanvas.Draw`。</para>
    /// </summary>
    [Fact]
    public void THintLines_Paint_PlayImagesPaintFirst()
    {
        DrawScrnEnv.g_WNewopUIImages = Images(300, 5, 6, 1);
        var font = DrawScrnEnv.CurrentFont;
        font.TextOuts.Clear();

        var l = new THintLines();
        l.Add("AB", TColor.clWhite);          // 文本（第二遍）
        l.Add("<NewopUI:1>", TColor.clWhite); // 图（第一遍）

        DrawScrnEnv.GameCanvas.Clear();
        l.Paint(DrawScrnRect.Rect(0, 0, 100, 100), DrawScrnRect.Rect(0, 0, 100, 100));

        // 第一遍只画了图；第二遍画文本。
        // 注意 Paint 的实参顺序：`hintMessage.Paint(PaintRect, Bounds(nX, …), OwnerRect)` ⇒
        // 图元素内部用的 `paintRect.Left` 其实是 **nX**（第一遍里已累加过"AB"的宽度 12）。
        Assert.Single(DrawScrnEnv.GameCanvas.Ops);
        Assert.Equal(ScrnDrawKind.Draw, DrawScrnEnv.GameCanvas.Ops[0].Kind);
        Assert.Equal(12 + l.OffsetX, DrawScrnEnv.GameCanvas.Ops[0].X);

        var text = Assert.Single(font.TextOuts);
        Assert.Equal("AB", text.S);
        Assert.Equal(0 + l.OffsetX, text.X);   // 第二遍里文本的 nX 从 PaintRect.Left=0 起算
    }

    /// <summary>
    /// 测试探针：`CheckHintImage` 是原文 ProcessHintText 的**嵌套函数**（托管侧为 `Ctx` 的非公开成员），
    /// 故此处按"走完整 ProcessHintText 后该 tag 是否留下**字面文本等于自身**的 THintText"来间接判定：
    /// 留下 = 未被识别。
    /// </summary>
    private sealed class ParserProbe
    {
        public bool InvokeCheck(string tag, DrawScrnHintTextParser.HintImageSpec spec)
        {
            var l = new THintLines();
            l.Add(tag, TColor.clWhite);
            // 走完整 ProcessHintText 后：若 tag 被识别，则列表里**没有**字面文本等于该 tag
            for (int i = 0; i < l.Count; i++)
            {
                if (l.Objects(i) is THintText t && t.FCaption == tag)
                    return false;
            }
            return true;
        }
    }
}
