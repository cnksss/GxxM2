using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// <c>Source/Client-HGE/NPCFormDeBug.pas</c>（451 行）1:1 移植的测试。
///
/// <para>用 <see cref="FakeNpcControl"/> 充当 <see cref="INpcControlView"/> 接缝
/// （DxComponent 控件族由另一条车道负责），从而把**脚本生成逻辑**与控件实现解耦测试。</para>
/// </summary>
public sealed class TailNpcFormDebugTests : IDisposable
{
    public TailNpcFormDebugTests() => NPCFormDeBug.ResetColorTableForTest();
    public void Dispose() => NPCFormDeBug.ResetColorTableForTest();

    /// <summary>INpcControlView 的可注入假实现。</summary>
    private class FakeNpcControl : INpcControlView
    {
        public int ControlID { get; set; }
        public string Caption { get; set; } = "";
        public string Hint { get; set; } = "";
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ImageWilId { get; set; }
        public int ImageIndexUp { get; set; }
        public int ImageIndexHot { get; set; }
        public int ImageIndexDown { get; set; }
        public int CaptionColorUpColor { get; set; } = NPCFormDeBug.clWhite;
        public int CaptionColorDownColor { get; set; } = NPCFormDeBug.clWhite;
        public bool CaptionBold { get; set; }
        public int CaptionSize { get; set; } = 9;
        public string CaptionFontName { get; set; } = "宋体";
        public IReadOnlyList<int> AutoColors { get; set; } = Array.Empty<int>();
        public string Cmd { get; set; } = "";

        public bool IsNpcLabel { get; set; }
        public bool IsNpcScrollBox { get; set; }
        public bool IsNpcItemButton { get; set; }
        public bool IsNpcInputEdit { get; set; }
        public bool IsNpcItemBoxButton { get; set; }
        public bool IsNpcProgressBoxButton { get; set; }
        public bool IsNpcUserItemButton { get; set; }
        public bool IsCountDownLabel { get; set; }
        public bool IsImgCountDownButton { get; set; }
        public bool IsNpcButton { get; set; }

        public string ScrollBoxDelimitedText { get; set; } = "";
        public int ScrollBoxListCount { get; set; }
        public bool ScrollBoxMouseHorizontal { get; set; }

        public int ItemFaceIndex { get; set; }
        public int ItemCount { get; set; }
        public bool ItemShowBorder { get; set; }
        public int ItemLight { get; set; }

        public bool EditIsNumber { get; set; }
        public int EditId { get; set; }
        public bool EditTransparent { get; set; }
        public int EditBackgroundColor { get; set; }
        public bool EditDrawBorder { get; set; }
        public int EditBorderColor { get; set; }
        public int EditFontColor { get; set; }
        public int EditMinValue { get; set; }
        public int EditMaxValue { get; set; }
        public string EditValidityTips { get; set; } = "";
        public string EditHintText { get; set; } = "";
        public int EditHintTextFontColor { get; set; }

        public int ItemBoxIndex { get; set; }
        public string ItemBoxStdModes { get; set; } = "";

        public int ProgressBgIndex { get; set; }
        public int ProgressStartIndex { get; set; }
        public int ProgressCount { get; set; }
        public int ProgressRefreshTime { get; set; }
        public int ProgressOffsetX { get; set; }
        public int ProgressOffsetY { get; set; }
        public int ProgressMinValue { get; set; }
        public int ProgressMaxValue { get; set; }
        public int ProgressValue { get; set; }
        public int ProgressDist { get; set; }
        public int ProgressTextColor { get; set; }
        public int ProgressTextOffsetX { get; set; }
        public int ProgressTextOffsetY { get; set; }
        public string ProgressText { get; set; } = "";

        public int UserItemIndex { get; set; }
        public int UserItemLight { get; set; }

        public int CountDownOldValue { get; set; }
        public int CountDownLoopCount { get; set; }
        public int ImgCountDownStartIndex { get; set; }
        public int ImgCountDownSpace { get; set; }

        public string ButtonACaption { get; set; } = "";
        public string ButtonPostText { get; set; } = "";
        public int ButtonStartImageIndex { get; set; }
        public int ButtonStopImageCount { get; set; }
        public int ButtonPlayImageTime { get; set; }
        public int ButtonAddData1 { get; set; }
        public int ButtonBlendMode { get; set; }
        public bool ButtonShowBG { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════
    // CalcEuclidDistance / ColorTo256
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：CalcEuclidDistance 的 +0.5 四舍五入与三轴勾股。</summary>
    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 0)]
    [InlineData(0, 3, 0, 4, 0, 0, 5)]      // 3-4-5
    [InlineData(0, 1, 0, 0, 0, 0, 1)]
    [InlineData(0, 2, 0, 0, 0, 0, 2)]
    [InlineData(0, 0, 0, 1, 0, 1, 1)]      // sqrt(2)=1.414 ⇒ Trunc(1.914)=1
    [InlineData(0, 1, 0, 1, 0, 0, 1)]      // sqrt(2) 同上
    public void CalcEuclidDistance_ComputesPythagorasWithRoundedTruncate(
        int r1, int r2, int g1, int g2, int b1, int b2, int expected)
        => Assert.Equal(expected, NPCFormDeBug.CalcEuclidDistance(r1, r2, g1, g2, b1, b2));

    /// <summary>用例 2：CalcEuclidDistance 的对称性（交换两端结果相同）。</summary>
    [Fact]
    public void CalcEuclidDistance_IsSymmetric()
    {
        int a = NPCFormDeBug.CalcEuclidDistance(10, 20, 30, 40, 50, 60);
        int b = NPCFormDeBug.CalcEuclidDistance(20, 10, 40, 30, 60, 50);
        Assert.Equal(a, b);
        Assert.Equal(0, NPCFormDeBug.CalcEuclidDistance(200, 200, 1, 1, 2, 2));
        // 三轴等价：任一轴差 100 都得 100
        Assert.Equal(100, NPCFormDeBug.CalcEuclidDistance(0, 100, 0, 0, 0, 0));
        Assert.Equal(100, NPCFormDeBug.CalcEuclidDistance(0, 0, 0, 100, 0, 0));
        Assert.Equal(100, NPCFormDeBug.CalcEuclidDistance(0, 0, 0, 0, 0, 100));
    }

    /// <summary>用例 3：RGB 打包与 TColor 的 0x00BBGGRR 布局。</summary>
    [Fact]
    public void Rgb_PacksInDelphiTColorOrder()
    {
        Assert.Equal(0x000000FF, NPCFormDeBug.RGB(255, 0, 0));      // 红在低字节
        Assert.Equal(0x0000FF00, NPCFormDeBug.RGB(0, 255, 0));
        Assert.Equal(0x00FF0000, NPCFormDeBug.RGB(0, 0, 255));
        Assert.Equal(NPCFormDeBug.clWhite, NPCFormDeBug.RGB(255, 255, 255));
        Assert.Equal(0, NPCFormDeBug.RGB(0, 0, 0));
    }

    /// <summary>用例 4：ColorTo256 精确命中（默认调色板里存在的颜色）。</summary>
    [Fact]
    public void ColorTo256_ExactPaletteColours_ReturnTheirIndex()
    {
        // 默认调色板前 16 项是 EGA 标准色
        Assert.Equal(0, NPCFormDeBug.ColorTo256(NPCFormDeBug.RGB(0, 0, 0)));
        Assert.Equal(15, NPCFormDeBug.ColorTo256(NPCFormDeBug.RGB(255, 255, 255)));
        Assert.Equal(9, NPCFormDeBug.ColorTo256(NPCFormDeBug.RGB(255, 0, 0)));
        Assert.Equal(10, NPCFormDeBug.ColorTo256(NPCFormDeBug.RGB(0, 255, 0)));
        Assert.Equal(12, NPCFormDeBug.ColorTo256(NPCFormDeBug.RGB(0, 0, 255)));
    }

    /// <summary>用例 5：ColorTo256 精确命中时取**第一个**匹配索引（32..35 是重复的灰阶）。</summary>
    [Fact]
    public void ColorTo256_ExactHitTakesFirstMatch()
    {
        // 6×6×6 立方里 (0,0,0) 在索引 16，但索引 0 就是 (0,0,0) ⇒ 应返回 0
        Assert.Equal(0, NPCFormDeBug.ColorTo256(NPCFormDeBug.RGB(0, 0, 0)));
    }

    /// <summary>用例 6：ColorTo256 的 TColor 位域抽取（红/绿/蓝各自独立）。</summary>
    [Fact]
    public void ColorTo256_ExtractsRgbFromLowThreeBytesOnly()
    {
        // 高字节（$FF000000 以上）必须被忽略：原文只取 $FF/$FF00/$FF0000
        int withHighByte = unchecked((int)0xFF123456);
        int withoutHigh = 0x00123456;
        Assert.Equal(NPCFormDeBug.ColorTo256(withoutHigh), NPCFormDeBug.ColorTo256(withHighByte));
    }

    /// <summary>
    /// 用例 7：<b>原文缺陷保真断言</b> —— 非精确命中时，原文的
    /// <c>if nCurED &lt; nMinEd then Result := I;</c> **忘记更新 nMinEd**（原文 :127-129），
    /// 于是 <c>nMinEd</c> 恒为"与索引 0 的距离"，<c>Result</c> 最后停在
    /// "自索引 1 起第一个比 d(0) 更近的索引"（若不存在则为 0），而**不是**真正最近的索引。
    /// <para>用例先把原文算法就地复算一遍（含缺陷），再断言 C# 实现与之一致；
    /// 最后用一个**构造出来的反例**证明它与"真正最小距离索引"不同。</para>
    /// </summary>
    [Fact]
    public void ColorTo256_NonExactMatch_UsesStaleMinDistance_OriginalBugFaithfullyPreserved()
    {
        var table = NPCFormDeBug.g_DefColorTable;

        // 就地复算原文算法
        byte Oracle(int c)
        {
            int R = c & 0xFF, G = (c & 0xFF00) >> 8, B = (c & 0xFF0000) >> 16;
            int nRlt = -1;
            for (int i = 0; i < 256; i++)
            {
                if (table[i].rgbBlue == B && table[i].rgbRed == R && table[i].rgbGreen == G) { nRlt = i; break; }
            }
            if (nRlt >= 0) return (byte)nRlt;
            int res = 0;
            int nMinEd = NPCFormDeBug.CalcEuclidDistance(R, table[0].rgbRed, G, table[0].rgbGreen, B, table[0].rgbBlue);
            for (int i = 1; i < 256; i++)
            {
                int cur = NPCFormDeBug.CalcEuclidDistance(R, table[i].rgbRed, G, table[i].rgbGreen, B, table[i].rgbBlue);
                if (cur < nMinEd) res = i;      // nMinEd **不**更新 —— 原文如此
            }
            return (byte)res;
        }

        var rnd = new Random(20260920);
        int mismatch = 0, sampled = 0;
        for (int n = 0; n < 4000 && mismatch == 0; n++)
        {
            int c = rnd.Next(0, 0x1000000);
            byte actual = NPCFormDeBug.ColorTo256(c);
            Assert.Equal(Oracle(c), actual);       // 与原文算法逐字一致
            sampled++;

            // 真正的最小距离索引
            int R = c & 0xFF, G = (c & 0xFF00) >> 8, B = (c & 0xFF0000) >> 16;
            int best = 0, bestD = int.MaxValue;
            for (int i = 0; i < 256; i++)
            {
                int cur = NPCFormDeBug.CalcEuclidDistance(R, table[i].rgbRed, G, table[i].rgbGreen, B, table[i].rgbBlue);
                if (cur < bestD) { bestD = cur; best = i; }
            }
            if ((byte)best != actual) mismatch++;
        }
        Assert.True(sampled > 0);
        // 至少抽到一个"有缺陷的返回值与真正最近索引不同"的颜色 ⇒ 缺陷确实存在
        Assert.True(mismatch > 0, $"抽了 {sampled} 个颜色都没能体现出该缺陷，缺陷断言失效");
    }

    /// <summary>用例 8：ColorTo256 的返回值恒 &lt; 256（byte 语义）。</summary>
    [Fact]
    public void ColorTo256_AlwaysReturnsByteRange()
    {
        var rnd = new Random(20260920);
        for (int i = 0; i < 200; i++)
        {
            int c = rnd.Next(0, 0x1000000);
            Assert.InRange(NPCFormDeBug.ColorTo256(c), (byte)0, (byte)255);
        }
    }

    /// <summary>用例 9：BuildDefaultColorTable 的形状（256 项、EGA 前 16 项、232..255 灰阶）。</summary>
    [Fact]
    public void BuildDefaultColorTable_HasExpectedShape()
    {
        var t = NPCFormDeBug.BuildDefaultColorTable();
        Assert.Equal(256, t.Length);
        Assert.Equal(0, t[0].rgbRed + t[0].rgbGreen + t[0].rgbBlue);
        Assert.Equal((255, 255, 255), (t[15].rgbRed, t[15].rgbGreen, t[15].rgbBlue));
        // 232..255 是灰阶：rgb 三通道相等
        for (int i = 232; i < 256; i++)
        {
            Assert.Equal(t[i].rgbRed, t[i].rgbGreen);
            Assert.Equal(t[i].rgbGreen, t[i].rgbBlue);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // GetFontStr
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：空 Caption 直接返回空串（原文 :147 Exit）。</summary>
    [Fact]
    public void GetFontStr_EmptyCaption_ReturnsEmpty()
    {
        var d = new FakeNpcControl { Caption = "" };
        Assert.Equal("", NPCFormDeBug.GetFontStr(d));
        Assert.Equal("", NPCFormDeBug.GetFontStr(null));
    }

    /// <summary>用例 2：默认字体（宋体/9/非粗/白底）⇒ 空串（无任何装饰）。</summary>
    [Fact]
    public void GetFontStr_DefaultFont_ProducesNoDecoration()
    {
        var d = new FakeNpcControl
        {
            Caption = "x",
            CaptionColorDownColor = NPCFormDeBug.clWhite,
            CaptionBold = false,
            CaptionSize = 9,
            CaptionFontName = "宋体",
        };
        Assert.Equal("", NPCFormDeBug.GetFontStr(d));
    }

    /// <summary>用例 3：AUTOCOLOR 分支（TNpcLabel 且有 AutoColors）。</summary>
    [Fact]
    public void GetFontStr_AutoColors_ProducesAutoColorList()
    {
        var d = new FakeNpcControl
        {
            Caption = "x",
            IsNpcLabel = true,
            AutoColors = new[] { NPCFormDeBug.RGB(255, 255, 255), NPCFormDeBug.RGB(0, 0, 0) },
        };
        // 白⇒15，黑⇒0（默认调色板精确命中）
        Assert.Equal("AUTOCOLOR=15,0,:", NPCFormDeBug.GetFontStr(d));
    }

    /// <summary>用例 4：FCOLOR 分支（非 TNpcLabel 且 Down.Color ≠ clWhite）。</summary>
    [Fact]
    public void GetFontStr_NonLabelWithNonWhiteDown_ProducesFColor()
    {
        var d = new FakeNpcControl
        {
            Caption = "x",
            IsNpcLabel = false,
            CaptionColorUpColor = NPCFormDeBug.RGB(255, 255, 255),
            CaptionColorDownColor = NPCFormDeBug.RGB(255, 0, 0),      // ≠ clWhite
        };
        Assert.Equal("FCOLOR=15:", NPCFormDeBug.GetFontStr(d));
    }

    /// <summary>用例 5：差异断言 —— 判据用的是 <c>Down.Color</c>，输出用的是 <c>Up.Color</c>。</summary>
    [Fact]
    public void GetFontStr_FColorConditionUsesDownButEmitsUp()
    {
        var d = new FakeNpcControl
        {
            Caption = "x",
            IsNpcLabel = false,
            CaptionColorUpColor = NPCFormDeBug.RGB(0, 0, 255),        // 蓝 ⇒ 索引 12
            CaptionColorDownColor = NPCFormDeBug.RGB(0, 255, 0),      // 绿 ⇒ 仅用于"非白"判据
        };
        Assert.Equal("FCOLOR=12:", NPCFormDeBug.GetFontStr(d));
    }

    /// <summary>用例 6：TNpcLabel 但 AutoColors 为空 ⇒ 走 else-if 的 FCOLOR 分支。</summary>
    [Fact]
    public void GetFontStr_LabelWithoutAutoColors_FallsBackToFColor()
    {
        var d = new FakeNpcControl
        {
            Caption = "x",
            IsNpcLabel = true,
            AutoColors = Array.Empty<int>(),
            CaptionColorUpColor = NPCFormDeBug.RGB(255, 0, 0),
            CaptionColorDownColor = NPCFormDeBug.RGB(255, 0, 0),
        };
        Assert.Equal("FCOLOR=9:", NPCFormDeBug.GetFontStr(d));
    }

    /// <summary>用例 7：FBOLD / FSIZE / FNAME 三个后缀的拼接顺序。</summary>
    [Fact]
    public void GetFontStr_AppendsBoldSizeNameInOrder()
    {
        var d = new FakeNpcControl
        {
            Caption = "x",
            CaptionColorDownColor = NPCFormDeBug.clWhite,   // 不产生 FCOLOR
            CaptionBold = true,
            CaptionSize = 12,
            CaptionFontName = "黑体",
        };
        Assert.Equal("FBOLD:FSIZE=12:FNAME=黑体:", NPCFormDeBug.GetFontStr(d));
    }

    /// <summary>用例 8：边界 —— Size=9 与 Name='宋体' 都不产生字段（硬编码默认值）。</summary>
    [Fact]
    public void GetFontStr_Size9AndSimSunAreTreatedAsDefaults()
    {
        var d = new FakeNpcControl { Caption = "x", CaptionSize = 9, CaptionFontName = "宋体" };
        Assert.Equal("", NPCFormDeBug.GetFontStr(d));
        d.CaptionSize = 10;
        Assert.Equal("FSIZE=10:", NPCFormDeBug.GetFontStr(d));
        d.CaptionSize = 9;
        d.CaptionFontName = "宋体x";
        Assert.Equal("FNAME=宋体x:", NPCFormDeBug.GetFontStr(d));
    }

    // ══════════════════════════════════════════════════════════════════════
    // DxControlToString
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：TNpcLabel 的 TEXT 标签（含 Hint / 坐标 / Cmd）。</summary>
    [Fact]
    public void DxControlToString_NpcLabel()
    {
        var d = new FakeNpcControl
        {
            IsNpcLabel = true, ControlID = 7, Caption = "你好", Hint = "提示",
            Left = 11, Top = 22, Cmd = "@test",
        };
        Assert.Equal("<7&TEXT:你好|提示:11:22/@test>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 2：ControlID=0 时改用 99999（原文 :204-207）。</summary>
    [Fact]
    public void DxControlToString_ZeroControlIdBecomes99999()
    {
        var d = new FakeNpcControl { IsNpcLabel = true, ControlID = 0, Caption = "x" };
        Assert.StartsWith("<99999&TEXT:", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 3：TNpcScrollBox（含 m_List.Count&gt;0 才输出 DelimitedText）。</summary>
    [Fact]
    public void DxControlToString_NpcScrollBox()
    {
        var d = new FakeNpcControl
        {
            IsNpcScrollBox = true, ControlID = 1,
            ScrollBoxListCount = 2, ScrollBoxDelimitedText = "a,b",
            ImageWilId = 3, ImageIndexUp = 4,
            Left = 5, Top = 6, Width = 7, Height = 8,
            ScrollBoxMouseHorizontal = true,
        };
        Assert.Equal("<1&SCROLLBOX:a,b:3:4:5:6:7:8:1>", NPCFormDeBug.DxControlToString(d));

        d.ScrollBoxListCount = 0;
        Assert.Equal("<1&SCROLLBOX:3:4:5:6:7:8:1>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 4：TNpcItemButton 的 ITEMSHOW 标签。</summary>
    [Fact]
    public void DxControlToString_NpcItemButton()
    {
        var d = new FakeNpcControl
        {
            IsNpcItemButton = true, ControlID = 2,
            ItemFaceIndex = 100, ItemCount = 1,
            Left = 3, Top = 4, ItemShowBorder = true, ItemLight = 5,
            Cmd = "@buy",
        };
        Assert.Equal("<2&ITEMSHOW:100:1:3:4:1:5/@buy>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 5：TNpcInputEdit 的数字/文本两分支与透明、边框开关。</summary>
    [Fact]
    public void DxControlToString_NpcInputEdit_BothVariants()
    {
        var d = new FakeNpcControl
        {
            IsNpcInputEdit = true, ControlID = 3,
            EditIsNumber = true, EditId = 9, Left = 1, Top = 2, Width = 30, Height = 10,
            EditTransparent = true, EditDrawBorder = false,
            EditFontColor = NPCFormDeBug.RGB(255, 255, 255),
            EditMinValue = 0, EditMaxValue = 99,
            EditValidityTips = "范围0-99", EditHintText = "输入",
            EditHintTextFontColor = NPCFormDeBug.RGB(0, 0, 0),
        };
        Assert.Equal("<3&INPUTNUM:9:1:2:30:10:-1:-1:15:0:99:范围0-99:输入:0>",
            NPCFormDeBug.DxControlToString(d));

        d.EditIsNumber = false;
        d.EditTransparent = false;
        d.EditDrawBorder = true;
        d.EditBackgroundColor = NPCFormDeBug.RGB(255, 255, 255);
        d.EditBorderColor = NPCFormDeBug.RGB(0, 0, 0);
        Assert.Equal("<3&INPUTTEXT:9:1:2:30:10:15:0:15:0:99:范围0-99:输入:0>",
            NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 6：TNpcItemBoxButton 的 ITEMBOX 标签。</summary>
    [Fact]
    public void DxControlToString_NpcItemBoxButton()
    {
        var d = new FakeNpcControl
        {
            IsNpcItemBoxButton = true, ControlID = 4,
            ItemBoxIndex = 5, ImageWilId = 6, ImageIndexUp = 7,
            Left = 8, Top = 9, Width = 40, Height = 41, ItemBoxStdModes = "M",
        };
        Assert.Equal("<4&ITEMBOX:5:6:7:8:9:40:41:M>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 7：TNpcProgressBoxButton 的 PROGRESSBAR 标签（18 个字段）。</summary>
    [Fact]
    public void DxControlToString_NpcProgressBoxButton()
    {
        var d = new FakeNpcControl
        {
            IsNpcProgressBoxButton = true, ControlID = 5,
            Left = 1, Top = 2, ImageWilId = 3,
            ProgressBgIndex = 4, ProgressStartIndex = 5, ProgressCount = 6, ProgressRefreshTime = 7,
            ProgressOffsetX = 8, ProgressOffsetY = 9, ProgressMinValue = 10, ProgressMaxValue = 11,
            ProgressValue = 12, ProgressDist = 13,
            ProgressTextColor = NPCFormDeBug.RGB(255, 255, 255),
            ProgressTextOffsetX = 14, ProgressTextOffsetY = 15, ProgressText = "T",
        };
        Assert.Equal("<5&PROGRESSBAR:1:2:3:4:5:6:7:8:9:10:11:12:13:15:14:15:T>",
            NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>
    /// 用例 8：<b>差异断言</b> —— TNpcUserItemButton 的标签头**也是 <c>PROGRESSBAR</c>**
    /// （原文 :323 的复制粘贴缺陷），而不是 <c>USERITEM</c>。
    /// </summary>
    [Fact]
    public void DxControlToString_NpcUserItemButton_UsesProgressBarTag_OriginalQuirk()
    {
        var d = new FakeNpcControl
        {
            IsNpcUserItemButton = true, ControlID = 6,
            UserItemIndex = 1, Left = 2, Top = 3,
            ItemShowBorder = true, UserItemLight = 4,
            Cmd = "@c",
        };
        string s = NPCFormDeBug.DxControlToString(d);
        Assert.Equal("<6&PROGRESSBAR:1:2:3:1:4/@c>", s);
        Assert.DoesNotContain("USERITEM", s);
    }

    /// <summary>用例 9：TCountDownLabel 与 TImgCountDownButton。</summary>
    [Fact]
    public void DxControlToString_CountDownVariants()
    {
        var a = new FakeNpcControl
        {
            IsCountDownLabel = true, ControlID = 8,
            CountDownOldValue = 10, CountDownLoopCount = 3,
            CaptionColorUpColor = NPCFormDeBug.RGB(255, 255, 255),
            Left = 4, Top = 5, Cmd = "@d",
        };
        Assert.Equal("<8&COUNTDOWN:10:3:15:4:5/@d>", NPCFormDeBug.DxControlToString(a));

        var b = new FakeNpcControl
        {
            IsImgCountDownButton = true, ControlID = 9,
            CountDownOldValue = 1, CountDownLoopCount = 2,
            ImgCountDownStartIndex = 3, ImgCountDownSpace = 4,
            Left = 5, Top = 6,
        };
        Assert.Equal("<9&IMGCOUNTDOWN:1:2:3:4:5:6>", NPCFormDeBug.DxControlToString(b));
    }

    /// <summary>用例 10：TNpcButton 的 IMG 分支。</summary>
    [Fact]
    public void DxControlToString_NpcButton_Img()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 10, ButtonACaption = NPCFormDeBug.C_IMG,
            ImageIndexUp = 1, ImageWilId = 2, Left = 3, Top = 4,
            ButtonPostText = "P", Cmd = "@e",
        };
        Assert.Equal("<10&IMG:1:2:3:4:P/@e>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 11：TNpcButton 的 PLAYIMG 分支（帧数 = Stop - Start + 1）。</summary>
    [Fact]
    public void DxControlToString_NpcButton_PlayImg_UsesStopMinusStartPlusOne()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 11, ButtonACaption = NPCFormDeBug.C_PLAYIMG,
            ImageWilId = 1, ButtonStartImageIndex = 10, ButtonStopImageCount = 19,
            ButtonPlayImageTime = 50, Left = 2, Top = 3, ButtonBlendMode = 2,
            Hint = "", ButtonPostText = "",
        };
        // BlendMode == 2 ⇒ 第 7 个字段是 0
        Assert.Equal("<11&PLAYIMG:1:10:10:50:2:3:0>", NPCFormDeBug.DxControlToString(d));

        d.ButtonBlendMode = 0;   // ≠ 2 ⇒ 1
        Assert.Equal("<11&PLAYIMG:1:10:10:50:2:3:1>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 12：差异断言 —— PLAYIMG 不输出 AddData1，PLAYIMGEX 输出。</summary>
    [Fact]
    public void DxControlToString_PlayImgVsPlayImgEx_AddData1Difference()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 12,
            ImageWilId = 1, ButtonStartImageIndex = 1, ButtonStopImageCount = 3,
            ButtonPlayImageTime = 9, ButtonAddData1 = 77,
            Left = 4, Top = 5, ButtonBlendMode = 2,
        };
        d.ButtonACaption = NPCFormDeBug.C_PLAYIMG;
        string playImg = NPCFormDeBug.DxControlToString(d);
        d.ButtonACaption = NPCFormDeBug.C_PLAYIMGEX;
        string playImgEx = NPCFormDeBug.DxControlToString(d);

        Assert.Equal("<12&PLAYIMG:1:1:3:9:4:5:0>", playImg);
        Assert.Equal("<12&PLAYIMGEX:1:1:3:9:77:4:5:0>", playImgEx);
        Assert.DoesNotContain(":77:", playImg);
    }

    /// <summary>用例 13：IMGEX / IMGNUM 分支的字段差异（IMGEX 有 Hot/Down，IMGNUM 也有但无 WilId）。</summary>
    [Fact]
    public void DxControlToString_ImgExVsImgNum()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 13,
            ImageWilId = 9, ImageIndexUp = 1, ImageIndexHot = 2, ImageIndexDown = 3,
            Left = 4, Top = 5, ButtonPostText = "P",
        };
        d.ButtonACaption = NPCFormDeBug.C_IMGEX;
        Assert.Equal("<13&IMGEX:9:1:2:3:4:5:P>", NPCFormDeBug.DxControlToString(d));
        d.ButtonACaption = NPCFormDeBug.C_IMGNUM;
        Assert.Equal("<13&IMGNUM:1:2:3:4:5>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 14：IMGPAY 分支的三处硬编码 '请重新输入'。</summary>
    [Fact]
    public void DxControlToString_ImgPay_HasHardCodedReinputText()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 14, ButtonACaption = NPCFormDeBug.C_IMGPAY,
            Left = 7, Top = 8,
        };
        Assert.Equal("<14&IMGPAY:请重新输入:请重新输入:请重新输入:7:8:请重新输入>",
            NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>
    /// 用例 15：<b>差异断言</b> —— LOOKS / DNITEMS / STATEITEM 三个分支的字段序列**完全相同**，
    /// 只有 NEWOPUI 少了 <c>m_nShowBG</c>。
    /// </summary>
    [Fact]
    public void DxControlToString_LooksDnItemsStateItemAreIdentical_NewOpUiLacksShowBg()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 15,
            ImageIndexUp = 1, Left = 2, Top = 3, ButtonShowBG = true, ButtonPostText = "P",
        };

        d.ButtonACaption = NPCFormDeBug.C_LOOKS;
        string looks = NPCFormDeBug.DxControlToString(d);
        d.ButtonACaption = NPCFormDeBug.C_DNITEMS;
        string dnItems = NPCFormDeBug.DxControlToString(d);
        d.ButtonACaption = NPCFormDeBug.C_STATEITEM;
        string stateItem = NPCFormDeBug.DxControlToString(d);
        d.ButtonACaption = NPCFormDeBug.C_NEWOPUI;
        string newOpUi = NPCFormDeBug.DxControlToString(d);

        // 前三个只有标签名不同 ⇒ 把标签名 replace 掉后应完全相同
        Assert.Equal(looks.Replace("LOOKS", "X"), dnItems.Replace("DNITEMS", "X"));
        Assert.Equal(looks.Replace("LOOKS", "X"), stateItem.Replace("STATEITEM", "X"));
        // NEWOPUI **少**一个 ShowBG 字段
        Assert.Equal("<15&LOOKS:1:2:3:1:P>", looks);
        Assert.Equal("<15&NEWOPUI:1:2:3:P>", newOpUi);
    }

    /// <summary>用例 16：TNpcButton 的未知 m_ACaption ⇒ 只有标签头 + FontStr + Cmd。</summary>
    [Fact]
    public void DxControlToString_NpcButton_UnknownCaption_YieldsBareTag()
    {
        var d = new FakeNpcControl
        {
            IsNpcButton = true, ControlID = 16, ButtonACaption = "UNKNOWN",
            Cmd = "@x",
        };
        Assert.Equal("<16&UNKNOWN/@x>", NPCFormDeBug.DxControlToString(d));
    }

    /// <summary>用例 17：未匹配任何类型 ⇒ 空串；null ⇒ 空串。</summary>
    [Fact]
    public void DxControlToString_UnmatchedOrNull_YieldsEmpty()
    {
        Assert.Equal("", NPCFormDeBug.DxControlToString(new FakeNpcControl()));
        Assert.Equal("", NPCFormDeBug.DxControlToString(null));
    }

    /// <summary>用例 18：类型判定的**优先级顺序**（Label 先于其他）。</summary>
    [Fact]
    public void DxControlToString_TypeBranchOrderMatchesSource()
    {
        // 同时置多个类型标志时，按原文 if/else-if 顺序取第一个
        var d = new FakeNpcControl
        {
            IsNpcLabel = true, IsNpcScrollBox = true, IsNpcButton = true,
            ControlID = 1, Caption = "L",
        };
        Assert.StartsWith("<1&TEXT:", NPCFormDeBug.DxControlToString(d));
    }

    // ══════════════════════════════════════════════════════════════════════
    // Start / BuildScriptLines / StripCrLf / Open
    // ══════════════════════════════════════════════════════════════════════

    private sealed class FakeNode : INpcControlNode
    {
        public List<INpcControlView> Children = new List<INpcControlView>();
        public int ComponentCount => Children.Count;
        public INpcControlView GetControl(int index) => Children[index];
    }

    /// <summary>既能当"控件视图"又能当"控件节点"的假控件（用于递归遍历测试）。</summary>
    private sealed class FakeControlNode : FakeNpcControl, INpcControlNode
    {
        public List<INpcControlView> Children = new List<INpcControlView>();
        public int ComponentCount => Children.Count;
        public INpcControlView GetControl(int index) => Children[index];
    }

    /// <summary>用例 1：Start 的倒序遍历（原文 for I := Count-1 downto 0）。</summary>
    [Fact]
    public void Start_IteratesChildrenInReverseOrder()
    {
        var root = new FakeNode();
        root.Children.Add(new FakeNpcControl { IsNpcLabel = true, ControlID = 1, Caption = "A" });
        root.Children.Add(new FakeNpcControl { IsNpcLabel = true, ControlID = 2, Caption = "B" });
        root.Children.Add(new FakeNpcControl { IsNpcLabel = true, ControlID = 3, Caption = "C" });

        var lines = NPCFormDeBug.BuildScriptLines(root);
        Assert.Equal(3, lines.Count);
        Assert.StartsWith("<3&TEXT:C:", lines[0]);
        Assert.StartsWith("<2&TEXT:B:", lines[1]);
        Assert.StartsWith("<1&TEXT:A:", lines[2]);
    }

    /// <summary>用例 2：ComponentCount=0 ⇒ 什么都不产出（原文 :481 Exit）。</summary>
    [Fact]
    public void Start_EmptyNode_ProducesNothing()
    {
        Assert.Empty(NPCFormDeBug.BuildScriptLines(new FakeNode()));
        Assert.Empty(NPCFormDeBug.BuildScriptLines(null));
    }

    /// <summary>用例 3：递归进入同时实现 INpcControlNode 的子控件。</summary>
    [Fact]
    public void Start_RecursesIntoChildNodes()
    {
        var grandChild = new FakeNpcControl { IsNpcLabel = true, ControlID = 99, Caption = "G" };
        var childNode = new FakeControlNode();
        childNode.Children.Add(grandChild);

        var root = new FakeNode();
        root.Children.Add(childNode);

        var lines = NPCFormDeBug.BuildScriptLines(root);
        // 第一行是子节点自身的脚本（未匹配类型 ⇒ 空串），第二行是孙节点
        Assert.Equal(2, lines.Count);
        Assert.Equal("", lines[0]);
        Assert.StartsWith("<99&TEXT:G:", lines[1]);
    }

    /// <summary>用例 4：StripCrLf 去掉全部 CR/LF（原文 Button1Click 的预处理）。</summary>
    [Fact]
    public void StripCrLf_RemovesAllCrAndLf()
    {
        Assert.Equal("abc", NPCFormDeBug.StripCrLf("a\r\nb\nc"));
        Assert.Equal("abc", NPCFormDeBug.StripCrLf("a\rb\rc"));
        Assert.Equal("", NPCFormDeBug.StripCrLf("\r\n\r\n"));
        Assert.Equal("", NPCFormDeBug.StripCrLf(""));
        Assert.Equal("", NPCFormDeBug.StripCrLf(null));
    }

    /// <summary>用例 5：StripCrLf 不动其它空白（Tab 保留）。</summary>
    [Fact]
    public void StripCrLf_KeepsTabsAndSpaces()
    {
        Assert.Equal("a\tb c", NPCFormDeBug.StripCrLf("a\tb c\r\n"));
    }

    /// <summary>用例 6：Open 填充下拉框并选中第一项（原文 :51-62）。</summary>
    [Fact]
    public void Open_FillsComboBoxAndSelectsFirst()
    {
        using var cb = new System.Windows.Forms.ComboBox();
        NPCFormDeBug.Open(cb, new[]
        {
            new KeyValuePair<string, object>("wil1", 1),
            new KeyValuePair<string, object>("wil2", 2),
        });
        Assert.Equal(2, cb.Items.Count);
        Assert.Equal(0, cb.SelectedIndex);
        Assert.Equal("wil1", cb.Items[0]);
    }

    /// <summary>用例 7：Open 的空列表与 null 入参边界。</summary>
    [Fact]
    public void Open_EmptyOrNull_LeavesComboBoxEmpty()
    {
        using var cb = new System.Windows.Forms.ComboBox();
        NPCFormDeBug.Open(cb, Array.Empty<KeyValuePair<string, object>>());
        Assert.Equal(0, cb.Items.Count);
        Assert.Equal(-1, cb.SelectedIndex);

        NPCFormDeBug.Open(cb, null);
        Assert.Equal(0, cb.Items.Count);

        NPCFormDeBug.Open(null, null);   // 不应抛
    }

    // ══════════════════════════════════════════════════════════════════════
    // DFM 逐条对照（NPCFormDeBug.dfm 是**文本格式**）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：窗体级属性与 DFM 一致。</summary>
    [Fact]
    public void Dfm_FormProperties_MatchTextDfm()
    {
        Assert.Equal(-118, NPCFormDeBug.DfmLeft);
        Assert.Equal(182, NPCFormDeBug.DfmTop);
        Assert.Equal("NPC界面调试", NPCFormDeBug.DfmCaption);   // DFM: Caption='NPC'#30028#38754#35843#35797
        Assert.Equal(400, NPCFormDeBug.DfmClientHeight);
        Assert.Equal(822, NPCFormDeBug.DfmClientWidth);
    }

    /// <summary>用例 2：三个顶层控件的几何与 DFM 一致，且不重叠。</summary>
    [Fact]
    public void Dfm_TopLevelControls_GeometryMatchesAndDoesNotOverlap()
    {
        Assert.Equal(561, NPCFormDeBug.DfmMemo1Width);
        Assert.Equal(305, NPCFormDeBug.DfmMemo1Height);
        Assert.Equal(576, NPCFormDeBug.DfmMemo2Left);
        Assert.Equal(241, NPCFormDeBug.DfmMemo2Width);
        Assert.Equal(305, NPCFormDeBug.DfmMemo2Height);

        Assert.Equal(3, NPCFormDeBug.DfmGroupBoxLeft);
        Assert.Equal(312, NPCFormDeBug.DfmGroupBoxTop);
        Assert.Equal(814, NPCFormDeBug.DfmGroupBoxWidth);
        Assert.Equal(73, NPCFormDeBug.DfmGroupBoxHeight);

        // Memo1 占 0..560，Memo2 从 576 起 ⇒ 不重叠
        Assert.True(561 <= NPCFormDeBug.DfmMemo2Left);
        // GroupBox 在 Memo 之下（312 >= 305）
        Assert.True(NPCFormDeBug.DfmGroupBoxTop >= NPCFormDeBug.DfmMemo1Height);
        // GroupBox 右边界不超出客户区
        Assert.True(NPCFormDeBug.DfmGroupBoxLeft + NPCFormDeBug.DfmGroupBoxWidth <= NPCFormDeBug.DfmClientWidth);
    }

    /// <summary>用例 3：GroupBox 内控件的几何与文案与 DFM 一致。</summary>
    [Fact]
    public void Dfm_GroupBoxChildren_MatchDfm()
    {
        Assert.Equal("图片资源:", NPCFormDeBug.DfmLabel1Caption);   // DFM: #22270#29255#36164#28304':'
        Assert.Equal("图片序号:", NPCFormDeBug.DfmLabel2Caption);   // DFM: #22270#29255#24207#21495':'
        Assert.Equal("NPC对话框背景设置", NPCFormDeBug.DfmGroupBoxCaption);

        Assert.Equal(76, NPCFormDeBug.DfmComboBox1Left);
        Assert.Equal(23, NPCFormDeBug.DfmComboBox1Top);
        Assert.Equal(145, NPCFormDeBug.DfmComboBox1Width);
        Assert.Equal(22, NPCFormDeBug.DfmComboBox1Height);

        Assert.Equal(288, NPCFormDeBug.DfmSpinEdit1Left);
        Assert.Equal(23, NPCFormDeBug.DfmSpinEdit1Top);
        Assert.Equal(73, NPCFormDeBug.DfmSpinEdit1Width);
        Assert.Equal(22, NPCFormDeBug.DfmSpinEdit1Height);

        Assert.Equal("应用(&A)", NPCFormDeBug.DfmButton1Caption);
        Assert.Equal(480, NPCFormDeBug.DfmButton1Left);
        Assert.Equal(24, NPCFormDeBug.DfmButton1Top);
        Assert.Equal(75, NPCFormDeBug.DfmButton1Width);
        Assert.Equal(25, NPCFormDeBug.DfmButton1Height);

        Assert.Equal("保存(&S)", NPCFormDeBug.DfmButton2Caption);
        Assert.Equal(576, NPCFormDeBug.DfmButton2Left);
        Assert.Equal(24, NPCFormDeBug.DfmButton2Top);
        Assert.Equal(75, NPCFormDeBug.DfmButton2Width);
        Assert.Equal(25, NPCFormDeBug.DfmButton2Height);
    }

    /// <summary>用例 4：按 DFM 构造的控件树自洽（需要 WinForms）。</summary>
    [Fact]
    public void Dfm_ConstructedForm_HasMatchingTree()
    {
        using var frm = new NPCFormDeBug.TFrmNPCDeBug();
        Assert.Equal(NPCFormDeBug.DfmCaption, frm.Text);
        Assert.Equal(NPCFormDeBug.DfmClientWidth, frm.ClientSize.Width);
        Assert.Equal(NPCFormDeBug.DfmClientHeight, frm.ClientSize.Height);
        Assert.Equal(3, frm.Controls.Count);

        Assert.Equal(NPCFormDeBug.DfmMemo1Width, frm.Memo1.Width);
        Assert.Equal(NPCFormDeBug.DfmMemo2Left, frm.Memo2.Left);
        Assert.Equal(6, frm.GroupBox1.Controls.Count);
        Assert.Equal(NPCFormDeBug.DfmGroupBoxCaption, frm.GroupBox1.Text);
        Assert.Equal(NPCFormDeBug.DfmComboBox1Width, frm.ComboBox1.Width);
        Assert.Equal(NPCFormDeBug.DfmSpinEdit1Width, frm.SpinEdit1.Width);
        Assert.Equal(0, frm.SpinEdit1.Maximum);       // DFM: MaxValue=0
        Assert.Equal(0, frm.SpinEdit1.Minimum);       // DFM: MinValue=0
        Assert.Equal(NPCFormDeBug.DfmButton1Caption, frm.Button1.Text);
        Assert.Equal(NPCFormDeBug.DfmButton2Caption, frm.Button2.Text);
    }

    /// <summary>用例 5：脚本标签常量与原文一致（10 个）。</summary>
    [Fact]
    public void ScriptTagConstants_MatchSource()
    {
        Assert.Equal("IMG", NPCFormDeBug.C_IMG);
        Assert.Equal("PLAYIMG", NPCFormDeBug.C_PLAYIMG);
        Assert.Equal("IMGEX", NPCFormDeBug.C_IMGEX);
        Assert.Equal("IMGNUM", NPCFormDeBug.C_IMGNUM);
        Assert.Equal("IMGPAY", NPCFormDeBug.C_IMGPAY);
        Assert.Equal("PLAYIMGEX", NPCFormDeBug.C_PLAYIMGEX);
        Assert.Equal("LOOKS", NPCFormDeBug.C_LOOKS);
        Assert.Equal("DNITEMS", NPCFormDeBug.C_DNITEMS);
        Assert.Equal("STATEITEM", NPCFormDeBug.C_STATEITEM);
        Assert.Equal("NEWOPUI", NPCFormDeBug.C_NEWOPUI);
    }
}
