using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// DxImageButtonEx.pas（889 行）—— token 模型 + 标记解析器部分（原文 24-140 / 160-741）的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/DxImageButtonEx.cs）：
//   * TTokenBase / TTokenText / TTokenImage / TTokenPlayImage / TTokenLine / TLineList
//   * ProcessButtonText（`<Img:>` / `<Looks:>` / `<DnItems:>` / `<StateItem:>` / `<NewopUI:>`
//     / `<PlayImg:>` 标记 + `{文字|调色板索引}` 颜色标记）
//   * DxImageButtonExEnv 接缝与 IDxImageList / IDxImageLibraryCached
//
// 全部经**公开 API** 构造与断言（本工程无 InternalsVisibleTo，故不直接触碰原文
// private/internal 字段 —— token 一律由 ProcessButtonText 或公开属性构造）。
// 全部接缝注入，不触碰任何 UI/资源（无头安全）。
//
// 所有 `原文 NNN` 行号指 Source\Client-HGE\DxImageButtonEx.pas。
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlImageButtonExTests
{
    // ===============================================================================
    // 夹具
    // ===============================================================================

    private sealed class FakeFont { }

    private sealed class CachedLib : IDxImageLibraryCached, IDxImageLibrary
    {
        private readonly List<IDxTexture> _images = new();
        private readonly Dictionary<int, (int X, int Y)> _offsets = new();

        public CachedLib Add(int w, int h, int offX = 0, int offY = 0)
        {
            int index = _images.Count;
            _images.Add(new TDxTextureStub(w, h));
            _offsets[index] = (offX, offY);
            return this;
        }

        public IDxTexture GetImage(int index) => index >= 0 && index < _images.Count ? _images[index] : null;

        public IDxTexture GetGray(int index) => GetImage(index);

        public IDxTexture GetCachedImage(int index, out int offsetX, out int offsetY)
        {
            if (_offsets.TryGetValue(index, out var o)) { offsetX = o.X; offsetY = o.Y; }
            else { offsetX = 0; offsetY = 0; }
            return GetImage(index);
        }
    }

    /// <summary>替换全部接缝并在作用域结束时还原。</summary>
    private sealed class Scope : IDisposable
    {
        public readonly TDxRecordingPainter Painter = new();
        public readonly FakeFont Font = new();
        public int FindFontCalls;

        public Scope()
        {
            DxImageButtonExEnv.Reset();
            DxImageButtonExEnv.Painter = Painter;
            DxImageButtonExEnv.FindFont = (name, size, style) =>
            {
                FindFontCalls++;
                return name == "" ? null : Font;
            };
            DxImageButtonExEnv.TextWidth = (f, t) => t.Length * 6;
            DxImageButtonExEnv.TextHeight = (f, t) => 12;
        }

        public void Dispose() => DxImageButtonExEnv.Reset();
    }

    private static TDxFont MakeFont(string name = "宋体", int size = 9, int color = 0x123456, bool bold = false)
        => new TDxFont { Name = name, Size = size, Color = color, Bold = bold };

    /// <summary>TTokenLine 只暴露原文的只读索引属性与 Count（无 IEnumerable），故用索引取全部 token。</summary>
    private static List<TTokenBase> TokenList(TTokenLine line)
    {
        var list = new List<TTokenBase>();
        for (int i = 0; i < line.Count; i++) list.Add(line[i]);
        return list;
    }

    /// <summary>用 tokenizer 造一个纯文本 token（也是本文档覆盖 ProcessButtonText 的主入口）。</summary>
    private static TTokenText MakeTextToken(string caption, TDxFont font)
    {
        var line = new TTokenLine();
        DxImageButtonExUnit.ProcessButtonText(caption, line, font);
        return (TTokenText)line[0];
    }

    // ===============================================================================
    // 一、TTokenBase（原文 26-43 / 165-171）
    // ===============================================================================

    [Fact]
    public void TokenBase_Constructor_SetsOnlyOffsetsAndOwner()
    {
        using var _ = new Scope();
        var line = new TTokenLine();
        var token = new TTokenImage(line);

        Assert.Same(line, token.Owner);
        Assert.Equal(0, token.OffsetX);
        Assert.Equal(0, token.OffsetY);
        Assert.Equal(0, token.Width);       // 原文 Create 不置 FWidth
        Assert.Equal(0, token.Height);
    }

    [Fact]
    public void TokenBase_OffsetAndSizeAreReadWrite()
    {
        using var _ = new Scope();
        var token = new TTokenImage(new TTokenLine());

        token.OffsetX = -3;
        token.OffsetY = 7;
        token.Width = 11;
        token.Height = 13;

        Assert.Equal(-3, token.OffsetX);
        Assert.Equal(7, token.OffsetY);
        Assert.Equal(11, token.Width);
        Assert.Equal(13, token.Height);
    }

    // ===============================================================================
    // 二、TTokenText（原文 45-64 / 175-230）
    // ===============================================================================

    [Fact]
    public void TokenText_DefaultsAndFontCopy_MatchOriginal()
    {
        using var _ = new Scope();
        var raw = new TTokenText(new TTokenLine());

        Assert.Equal("", raw.Caption);                    // 原文 178
        Assert.Equal("", raw.FontName);                   // 原文 179
        Assert.Equal(TDxColor.clWhite, raw.FontColor);    // 原文 180
        Assert.Equal(0, raw.FontSize);                    // 原文 181
        Assert.Empty(raw.FontStyle);                      // 原文 182 FFontStyle := []
        Assert.False(raw.FontStroke);                     // 原文 183

        // tokenizer 逐项拷贝 Font 的 5 个属性（原文 466-472 等 5 处同形）
        var font = MakeFont("隶书", 14, 0xAABBCC, bold: true);
        var t = MakeTextToken("x", font);

        Assert.Equal("x", t.Caption);
        Assert.Equal("隶书", t.FontName);
        Assert.Equal(14, t.FontSize);
        Assert.Equal(0xAABBCC, t.FontColor);
        Assert.True(t.FontStroke);
    }

    [Fact]
    public void TokenText_Initialize_UsesFontNameFirst()
    {
        using var scope = new Scope();
        var t = MakeTextToken("abcd", MakeFont("宋体"));

        t.Initialize();

        Assert.Equal(24, t.Width);                        // TextWidth("abcd") = 4*6
        Assert.Equal(12, t.Height);                       // TextHeight("Pp") = 12
        Assert.Equal(1, scope.FindFontCalls);             // 首次命中，不再走 g_sCurFontName
    }

    [Fact]
    public void TokenText_Initialize_FallsBackToCurFontName()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.CurFontName = "默认字体";
        var t = MakeTextToken("ab", MakeFont(""));

        t.Initialize();

        Assert.Equal(12, t.Width);                        // 2*6
        Assert.Equal(12, t.Height);
        Assert.Equal(1, scope.FindFontCalls);             // FFontName='' → 只查一次兜底名
    }

    [Fact]
    public void TokenText_Initialize_BothNamesMiss_KeepsZeroSize()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.FindFont = (n, s, st) => null;
        var t = MakeTextToken("abc", MakeFont("F"));

        t.Initialize();

        Assert.Equal(0, t.Width);                         // 从 0 开始就没找到 → 保持 0
        Assert.Equal(0, t.Height);
    }

    [Fact]
    public void TokenText_Initialize_StrokeAddsTwoToBothDimensions()
    {
        using var _ = new Scope();
        var t = MakeTextToken("ab", MakeFont("F", bold: true));

        t.Initialize();

        Assert.Equal(14, t.Width);                        // 12 + 2（原文 203）
        Assert.Equal(14, t.Height);                       // 12 + 2（原文 204）
    }

    [Fact]
    public void TokenText_Initialize_NoFontFoundSecondTime_KeepsPreviousSize_Differential()
    {
        // 原文 186-207 **不先清零**：第二次找不到字体时尺寸保持第一次的值（差异断言：清零则为 0）
        using var scope = new Scope();
        var t = MakeTextToken("abc", MakeFont("F"));
        t.Initialize();
        Assert.Equal(18, t.Width);
        Assert.Equal(12, t.Height);

        DxImageButtonExEnv.FindFont = (n, s, st) => null;
        t.Initialize();

        Assert.Equal(18, t.Width);
        Assert.Equal(12, t.Height);
    }

    [Fact]
    public void TokenText_Paint_EmptyCaption_DrawsNothing()
    {
        using var scope = new Scope();
        var t = MakeTextToken("", MakeFont("F"));

        t.Paint(10, 20);

        Assert.Empty(scope.Painter.Ops);                  // 原文 214 `if FCaption = '' then Exit`
    }

    [Fact]
    public void TokenText_Paint_NoFont_DrawsNothing()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.FindFont = (n, s, st) => null;
        var t = MakeTextToken("abc", MakeFont("F"));

        t.Paint(0, 0);

        Assert.Empty(scope.Painter.Ops);
    }

    [Fact]
    public void TokenText_Paint_PlainText_SingleTextRectAtPointPlusOffset()
    {
        using var scope = new Scope();
        var t = MakeTextToken("ab", MakeFont("F"));
        t.Initialize();
        t.OffsetX = 3;
        t.OffsetY = 4;

        t.Paint(100, 200);

        Assert.Equal("TextRect(103,204,(103,204,115,216),n=1,123456,2,255,0)",
            Assert.Single(scope.Painter.Ops));
    }

    [Fact]
    public void TokenText_Paint_Stroke_DrawsFourOutlinePassesThenText()
    {
        // 原文 226：BoldTextOut(HGEFont, X, Y, FCaption, FFontColor, clBlack)
        using var scope = new Scope();
        var t = MakeTextToken("a", MakeFont("F", bold: true));
        t.Initialize();                                   // 6x12 → 描边后 8x14

        t.Paint(10, 20);

        Assert.Equal(5, scope.Painter.Ops.Count);
        Assert.Equal("TextRect(9,20,(10,20,18,34),n=1,000000,2,255,0)", scope.Painter.Ops[0]);
        Assert.Equal("TextRect(11,20,(10,20,18,34),n=1,000000,2,255,0)", scope.Painter.Ops[1]);
        Assert.Equal("TextRect(10,19,(10,20,18,34),n=1,000000,2,255,0)", scope.Painter.Ops[2]);
        Assert.Equal("TextRect(10,21,(10,20,18,34),n=1,000000,2,255,0)", scope.Painter.Ops[3]);
        Assert.Equal("TextRect(10,20,(10,20,18,34),n=1,123456,2,255,0)", scope.Painter.Ops[4]);
    }

    // ===============================================================================
    // 三、TTokenImage（原文 66-78 / 234-269）
    // ===============================================================================

    [Fact]
    public void TokenImage_Defaults_MatchOriginal()
    {
        using var _ = new Scope();
        var t = new TTokenImage(new TTokenLine());

        Assert.Null(t.GameImages);        // 原文 237
        Assert.Equal(0, t.ImageIndex);    // 原文 238
        Assert.False(t.DrawBlend);        // 原文 239
    }

    [Fact]
    public void TokenImage_Initialize_TakesTextureSize()
    {
        using var _ = new Scope();
        var t = new TTokenImage(new TTokenLine()) { GameImages = new CachedLib().Add(32, 48) };

        t.Initialize();

        Assert.Equal(32, t.Width);
        Assert.Equal(48, t.Height);
    }

    [Fact]
    public void TokenImage_Initialize_NullLibrary_KeepsPreviousSize()
    {
        using var _ = new Scope();
        var t = new TTokenImage(new TTokenLine()) { GameImages = new CachedLib().Add(5, 6) };
        t.Initialize();

        t.GameImages = null;
        t.Initialize();

        Assert.Equal(5, t.Width);         // 原文 246 的 if 不成立 → 不清零
        Assert.Equal(6, t.Height);
    }

    [Fact]
    public void TokenImage_Initialize_OutOfRangeIndex_KeepsPreviousSize()
    {
        using var _ = new Scope();
        var t = new TTokenImage(new TTokenLine()) { GameImages = new CachedLib().Add(5, 6) };
        t.Initialize();

        t.ImageIndex = 99;
        t.Initialize();

        Assert.Equal(5, t.Width);
        Assert.Equal(6, t.Height);
    }

    [Fact]
    public void TokenImage_Paint_DrawsAtPointPlusOffsetWithoutBlend()
    {
        using var scope = new Scope();
        var t = new TTokenImage(new TTokenLine())
        {
            GameImages = new CachedLib().Add(20, 10),
            OffsetX = 2,
            OffsetY = -3,
        };

        t.Paint(100, 200);

        Assert.Equal("Draw(102,197,tex20x10,2)", Assert.Single(scope.Painter.Ops));
    }

    [Fact]
    public void TokenImage_Paint_IgnoresDrawBlend_Differential()
    {
        // 原文怪癖：TTokenImage.Paint（256-269）只走 GameCanvas.Draw，**从不读 FDrawBlend**
        using var scope = new Scope();
        var t = new TTokenImage(new TTokenLine())
        {
            GameImages = new CachedLib().Add(20, 10),
            DrawBlend = true,
        };

        t.Paint(0, 0);

        Assert.Equal("Draw(0,0,tex20x10,2)", Assert.Single(scope.Painter.Ops));
    }

    [Fact]
    public void TokenImage_Paint_NoLibraryOrNoTexture_DrawsNothing()
    {
        using var scope = new Scope();
        var t = new TTokenImage(new TTokenLine());
        t.Paint(0, 0);
        Assert.Empty(scope.Painter.Ops);

        t.GameImages = new CachedLib();
        t.ImageIndex = 5;
        t.Paint(0, 0);
        Assert.Empty(scope.Painter.Ops);
    }

    // ===============================================================================
    // 四、TTokenPlayImage（原文 80-98 / 273-320）
    // ===============================================================================

    [Fact]
    public void TokenPlayImage_Defaults_MatchOriginal()
    {
        using var _ = new Scope();
        var t = new TTokenPlayImage(new TTokenLine());

        Assert.Null(t.GameImages);        // 原文 276
        Assert.False(t.DrawBlend);        // 原文 277
        Assert.Equal(0, t.StartIndex);    // 原文 278
        Assert.Equal(0, t.DrawCount);     // 原文 279
        Assert.Equal(100, t.DrawTime);    // 原文 280
        Assert.Equal(0, t.DrawIndex);     // 原文 282
    }

    [Fact]
    public void TokenPlayImage_Initialize_ZeroesSize()
    {
        using var _ = new Scope();
        var t = new TTokenPlayImage(new TTokenLine());
        t.Width = 9;
        t.Height = 9;

        t.Initialize();                   // 原文 287 的 inherited 指向抽象方法，见源文件头第 8 条

        Assert.Equal(0, t.Width);
        Assert.Equal(0, t.Height);
    }

    /// <summary>经 tokenizer 造一个 PlayImg token（_drawTick 取注入时钟的当前值）。</summary>
    private static TTokenPlayImage MakePlayToken(string tag, CachedLib lib, uint tickAtCreation = 0)
    {
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(lib);
        uint now = tickAtCreation;
        DxTickCount.Provider = () => now;
        var line = new TTokenLine();
        DxImageButtonExUnit.ProcessButtonText(tag, line, MakeFont("F"));
        return (TTokenPlayImage)line[0];
    }

    [Fact]
    public void TokenPlayImage_Paint_DrawsCurrentFrameWithCachedOffset()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(7, 8, offX: 4, offY: -6);
        var t = MakePlayToken("<PlayImg:0:0:3:100000:1:2:0>", lib);
        try
        {
            t.Paint(100, 200);

            Assert.Equal("Draw(105,196,tex7x8,2)", Assert.Single(scope.Painter.Ops));
        }
        finally { DxTickCount.Provider = null; }
    }

    [Fact]
    public void TokenPlayImage_Paint_BlendBranch_UsesDrawBlend()
    {
        using var scope = new Scope();
        var t = MakePlayToken("<PlayImg:0:0:3:100000:0:0:1>", new CachedLib().Add(7, 8));
        try
        {
            Assert.True(t.DrawBlend);

            t.Paint(0, 0);

            // TDxRecordingPainter 未实现 IDxSurfacePainterExt → DxPainterExt 退化为 4 参 Draw
            Assert.Equal("Draw(0,0,(0,0,7,8),tex7x8)", Assert.Single(scope.Painter.Ops));
        }
        finally { DxTickCount.Provider = null; }
    }

    [Fact]
    public void TokenPlayImage_Paint_NoLibrary_DrawsNothing()
    {
        using var scope = new Scope();
        new TTokenPlayImage(new TTokenLine()).Paint(0, 0);
        Assert.Empty(scope.Painter.Ops);
    }

    [Fact]
    public void TokenPlayImage_FrameAdvancesOnlyWhenStrictlyGreater()
    {
        // 原文 314 是 **严格大于**：Δ == DrawTime 不推进
        using var scope = new Scope();
        uint tick = 0;
        var t = MakePlayToken("<PlayImg:0:0:5:100:0:0:0>", new CachedLib().Add(1, 1));
        try
        {
            DxTickCount.Provider = () => tick;

            tick = 100;
            t.Paint(0, 0);
            Assert.Equal(0, t.DrawIndex);        // Δ == 100 → 不推进

            tick = 101;
            t.Paint(0, 0);
            Assert.Equal(1, t.DrawIndex);        // Δ == 101 > 100 → 推进
        }
        finally { DxTickCount.Provider = null; }
    }

    [Fact]
    public void TokenPlayImage_FrameWrapsAtStartIndexPlusDrawCount()
    {
        using var scope = new Scope();
        uint tick = 0;
        var lib = new CachedLib().Add(1, 1).Add(1, 1).Add(1, 1);
        var t = MakePlayToken("<PlayImg:0:1:2:10:0:0:0>", lib);
        try
        {
            DxTickCount.Provider = () => tick;
            Assert.Equal(1, t.StartIndex);
            Assert.Equal(2, t.DrawCount);
            Assert.Equal(1, t.DrawIndex);

            tick = 11; t.Paint(0, 0);
            Assert.Equal(2, t.DrawIndex);
            tick = 22; t.Paint(0, 0);
            Assert.Equal(1, t.DrawIndex);        // 3-1+1=3 > 2 → 回绕到 StartIndex
            tick = 33; t.Paint(0, 0);
            Assert.Equal(2, t.DrawIndex);
        }
        finally { DxTickCount.Provider = null; }
    }

    [Fact]
    public void TokenPlayImage_FrameAdvancesEvenWhenTextureMissing()
    {
        // 原文 313-319 的帧推进在 `if Texture <> nil` **之外**
        using var scope = new Scope();
        uint tick = 0;
        var t = MakePlayToken("<PlayImg:0:0:5:10:0:0:0>", new CachedLib());   // 空库
        try
        {
            DxTickCount.Provider = () => tick;
            tick = 20;

            t.Paint(0, 0);

            Assert.Empty(scope.Painter.Ops);
            Assert.Equal(1, t.DrawIndex);        // 仍然推进
        }
        finally { DxTickCount.Provider = null; }
    }

    [Fact]
    public void TokenPlayImage_CardinalWrapOfTickDifference()
    {
        // `CurTick - FDrawTick` 是 Cardinal：时钟回绕时差值变小但仍是正数
        using var scope = new Scope();
        uint tick = uint.MaxValue - 5;
        var t = MakePlayToken("<PlayImg:0:0:5:10:0:0:0>", new CachedLib().Add(1, 1), tick);
        try
        {
            tick = 10;                            // (uint)(10 - (MaxValue-5)) = 16
            DxTickCount.Provider = () => tick;

            t.Paint(0, 0);

            Assert.Equal(1, t.DrawIndex);         // 16 > 10
        }
        finally { DxTickCount.Provider = null; }
    }

    // ===============================================================================
    // 五、TTokenLine（原文 100-119 / 324-388）
    // ===============================================================================

    [Fact]
    public void TokenLine_DefaultsAndAddToken()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        Assert.Equal(0, line.Count);
        Assert.Equal(0, line.Width);
        Assert.Equal(0, line.Height);

        var t = new TTokenImage(line);
        line.AddToken(t);

        Assert.Equal(1, line.Count);
        Assert.Same(t, line[0]);
    }

    [Fact]
    public void TokenLine_ClearRemovesTokens()
    {
        using var _ = new Scope();
        var line = new TTokenLine();
        line.AddToken(new TTokenImage(line));
        line.AddToken(new TTokenImage(line));

        line.Clear();

        Assert.Equal(0, line.Count);
    }

    [Fact]
    public void TokenLine_RecalSize_SumsTextWidthAndTakesMaxTextHeight()
    {
        using var _ = new Scope();
        var line = new TTokenLine();
        DxImageButtonExUnit.ProcessButtonText("abcd", line, MakeFont("F"));   // 24 x 12
        DxImageButtonExUnit.ProcessButtonText("ab", line, MakeFont("F"));     // 12 x 12

        line.RecalSize();

        Assert.Equal(36, line.Width);               // 累加
        Assert.Equal(12, line.Height);              // Max
    }

    [Fact]
    public void TokenLine_RecalSize_NonTextTokensDoNotContributeSize()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(99, 99));
        var line = new TTokenLine();
        DxImageButtonExUnit.ProcessButtonText("<Img:0:0>", line, MakeFont("F"));
        DxImageButtonExUnit.ProcessButtonText("ab", line, MakeFont("F"));

        line.RecalSize();

        Assert.Equal(12, line.Width);               // 图片的 99 不计入（原文 375 的 is TTokenText 门控）
        Assert.Equal(12, line.Height);
        Assert.Equal(99, line[0].Width);            // 但 Initialize 仍被调用
    }

    [Fact]
    public void TokenLine_RecalSize_CurrentFontFallbackForZeroSize()
    {
        // 原文 381-387：CurrentFont <> nil 时，宽 ≤ 0 → TextWidth('0')，高 ≤ 0 → g_CurrentFontHeight
        using var _ = new Scope();
        DxImageButtonExEnv.CurrentFont = new FakeFont();
        DxImageButtonExEnv.CurrentFontHeight = 21;
        var line = new TTokenLine();

        line.RecalSize();

        Assert.Equal(6, line.Width);                // TextWidth("0") = 1*6
        Assert.Equal(21, line.Height);
    }

    [Fact]
    public void TokenLine_RecalSize_NoCurrentFont_LeavesZero()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.CurrentFont = null;
        var line = new TTokenLine();

        line.RecalSize();

        Assert.Equal(0, line.Width);
        Assert.Equal(0, line.Height);
    }

    [Fact]
    public void TokenLine_RecalSize_DoesNotOverrideNonZeroSize()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.CurrentFont = new FakeFont();
        DxImageButtonExEnv.CurrentFontHeight = 21;
        var line = new TTokenLine();
        DxImageButtonExUnit.ProcessButtonText("ab", line, MakeFont("F"));

        line.RecalSize();

        Assert.Equal(12, line.Width);               // 非 0 → 不兜底
        Assert.Equal(12, line.Height);
    }

    // ===============================================================================
    // 六、TLineList（原文 121-140 / 392-451）
    // ===============================================================================

    [Fact]
    public void LineList_AddLineAndIndexer()
    {
        using var _ = new Scope();
        var list = new TLineList();

        var l0 = list.AddLine();
        var l1 = list.AddLine();

        Assert.Equal(2, list.Count);
        Assert.Same(l0, list[0]);
        Assert.Same(l1, list[1]);
        Assert.Equal(0, list.Width);
        Assert.Equal(0, list.Height);
    }

    [Fact]
    public void LineList_ClearRemovesLines()
    {
        using var _ = new Scope();
        var list = new TLineList();
        list.AddLine();
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void LineList_RecalSize_SingleLine_NoExpandHeight()
    {
        using var _ = new Scope();
        var list = new TLineList();

        var line = list.AddLine();
        DxImageButtonExUnit.ProcessButtonText("ab", line, MakeFont("F"));
        list.RecalSize();

        Assert.Equal(12, list.Width);
        Assert.Equal(12, list.Height);              // 首行不加 ExpandLineHeight
    }

    [Fact]
    public void LineList_RecalSize_HeightAddsExpandOnlyBetweenLines()
    {
        using var _ = new Scope();
        var list = new TLineList();
        foreach (var caption in new[] { "ab", "abcd", "a" })
        {
            var line = list.AddLine();
            DxImageButtonExUnit.ProcessButtonText(caption, line, MakeFont("F"));
        }

        list.RecalSize();

        // 12 + (12+2) + (12+2) = 40（原文 448-449：AddHeight 首轮 0，其后恒为 ExpandLineHeight）
        Assert.Equal(40, list.Height);
        Assert.Equal(24, list.Width);               // 取各行最大值
    }

    [Fact]
    public void LineList_RecalSize_EmptyList_StaysZero()
    {
        using var _ = new Scope();
        new TLineList().RecalSize();
        Assert.Equal(0, new TLineList().Height);
    }

    // ===============================================================================
    // 七、ProcessButtonText —— 纯文本 / 颜色标记（原文 455-560）
    // ===============================================================================

    [Fact]
    public void ProcessButtonText_NoTag_OneTokenAndReturnsText()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("hello", line, MakeFont());

        Assert.Equal("hello", result);
        Assert.Equal(1, line.Count);
        Assert.Equal("hello", ((TTokenText)line[0]).Caption);
    }

    [Fact]
    public void ProcessButtonText_EmptyString_CreatesOneEmptyToken()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("", line, MakeFont());

        Assert.Equal("", result);
        Assert.Equal(1, line.Count);                 // 原文 465 的空串分支也会 AddToken
        Assert.Equal("", ((TTokenText)line[0]).Caption);
    }

    [Fact]
    public void ProcessButtonText_CustomColor_SplitsIntoThreeTokens()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.GetRGB = n => 0x1000 + n;
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("aa{bb|100}cc", line, MakeFont());

        Assert.Equal("aabbcc", result);              // 颜色标记的文字保留、花括号消失
        Assert.Equal(3, line.Count);
        Assert.Equal("aa", ((TTokenText)line[0]).Caption);
        Assert.Equal("bb", ((TTokenText)line[1]).Caption);
        Assert.Equal(0x1064, ((TTokenText)line[1]).FontColor);   // GetRGB(100)
        Assert.Equal("cc", ((TTokenText)line[2]).Caption);
    }

    [Theory]
    [InlineData("a{x|0}b", true)]
    [InlineData("a{x|255}b", true)]
    [InlineData("a{x|256}b", false)]
    [InlineData("a{x|-1}b", false)]
    public void ProcessButtonText_ColorIndexBoundaries(string input, bool expectCustom)
    {
        // 原文 511：仅 0..255 视为自定义颜色
        using var _ = new Scope();
        DxImageButtonExEnv.GetRGB = n => 0x2000 + n;
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText(input, line, MakeFont());

        if (expectCustom)
        {
            Assert.Equal("axb", result);
            Assert.Equal(3, line.Count);
        }
        else
        {
            Assert.Equal(input, result);
            // 非自定义色：中间的 `{...}` 原样成为一个 token，其后的文字再成一个 token → 共 3 个
            Assert.Equal(3, line.Count);
            Assert.Contains("}", ((TTokenText)line[1]).Caption);
        }
    }

    [Fact]
    public void ProcessButtonText_CustomColorUsesGetRGB()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.GetRGB = n => n * 3;
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("{x|7}", line, MakeFont());

        Assert.Equal(21, ((TTokenText)line[0]).FontColor);
    }

    [Fact]
    public void ProcessButtonText_BracesWithoutPipe_KeptLiterally()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a{x}b", line, MakeFont());

        Assert.Equal("a{x}b", result);
        Assert.Equal(3, line.Count);                 // "a" / "{x}" / "b"
        Assert.Equal("a", ((TTokenText)line[0]).Caption);
        Assert.Equal("{x}", ((TTokenText)line[1]).Caption);
        Assert.Equal("b", ((TTokenText)line[2]).Caption);
    }

    [Fact]
    public void ProcessButtonText_PipeValueNotANumber_KeptLiterally()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a{x|zz}b", line, MakeFont());

        Assert.Equal("a{x|zz}b", result);
        Assert.Equal(3, line.Count);
        Assert.Equal("{x|zz}", ((TTokenText)line[1]).Caption);
    }

    [Fact]
    public void ProcessButtonText_EmptyColorText_EmitsNoMiddleToken()
    {
        // `{|100}` → S3 为空 → boCustomColorText 为 True 但不建 token
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a{|100}b", line, MakeFont());

        Assert.Equal("ab", result);
        Assert.Equal(2, line.Count);
    }

    [Fact]
    public void ProcessButtonText_UnclosedBrace_TreatedAsPlainText()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a{b", line, MakeFont());

        Assert.Equal("a{b", result);
        Assert.Equal(1, line.Count);
        Assert.Equal("a{b", ((TTokenText)line[0]).Caption);
    }

    [Fact]
    public void ProcessButtonText_CloseBraceBeforeOpenBrace_ReproducesOriginalParser()
    {
        // 差异断言：原文 480-489 只检查 Index1/Index2 > 0，**不检查 Index2 > Index1**
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a}b{c", line, MakeFont());

        Assert.Equal("a}b{}b{c", result);
        Assert.Equal(3, line.Count);
        Assert.Equal("a}b", ((TTokenText)line[0]).Caption);
        Assert.Equal("{}", ((TTokenText)line[1]).Caption);
        Assert.Equal("b{c", ((TTokenText)line[2]).Caption);
    }

    [Fact]
    public void ProcessButtonText_MultipleColorSegments()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.GetRGB = n => n;
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a{1|5}b{2|6}c", line, MakeFont());

        Assert.Equal("a1b2c", result);
        Assert.Equal(5, line.Count);
        Assert.Equal(5, ((TTokenText)line[1]).FontColor);
        Assert.Equal(6, ((TTokenText)line[3]).FontColor);
    }

    [Fact]
    public void ProcessButtonText_LeadingAndTrailingColorTags()
    {
        using var _ = new Scope();
        DxImageButtonExEnv.GetRGB = n => n;
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("{a|1}{b|2}", line, MakeFont());

        Assert.Equal("ab", result);
        Assert.Equal(2, line.Count);
        Assert.Equal("a", ((TTokenText)line[0]).Caption);
        Assert.Equal("b", ((TTokenText)line[1]).Caption);
    }

    // ===============================================================================
    // 八、ProcessButtonText —— 图像标记（原文 562-741）
    // ===============================================================================

    [Fact]
    public void ProcessButtonText_ImgTag_CreatesTokenImageWithOffsets()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(10, 10).Add(20, 20);
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(lib);
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("<Img:3:0:5:6>", line, MakeFont());

        Assert.Equal("", result);
        var img = Assert.IsType<TTokenImage>(Assert.Single(TokenList(line)));
        Assert.Same(lib, img.GameImages);
        Assert.Equal(3, img.ImageIndex);
        Assert.Equal(5, img.OffsetX);
        Assert.Equal(6, img.OffsetY);
    }

    [Fact]
    public void ProcessButtonText_ImgTag_TextAroundItIsKept()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(10, 10));
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("ab<Img:1:0>cd", line, MakeFont());

        Assert.Equal("abcd", result);
        Assert.Equal(3, line.Count);
        Assert.IsType<TTokenText>(line[0]);
        Assert.IsType<TTokenImage>(line[1]);
        Assert.IsType<TTokenText>(line[2]);
    }

    [Fact]
    public void ProcessButtonText_ImgTag_IndexOutOfEffectList_NoLibrary()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        // I2=5 需 < Count(1) 才取图库 → 越界，图库为 nil → NewTokenImage 不加 token
        string result = DxImageButtonExUnit.ProcessButtonText("<Img:1:5>", line, MakeFont());

        Assert.Equal("", result);
        Assert.Equal(0, line.Count);
    }

    [Fact]
    public void ProcessButtonText_ImgTag_NoEffectListInjected_TagTextIsDropped()
    {
        // 未注入 g_EffectImageList 时 Count 取 -1 → 不取图库；CheckTokenImage 仍返回 True
        // → 调用方走 else：不建 token 且 StrC 不计入文本（原文在 nil 列表处会 AV，托管侧退化）
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = null;
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("ab<Img:1:0>cd", line, MakeFont());

        Assert.Equal("abcd", result);
        Assert.Equal(2, line.Count);
        Assert.All(TokenList(line), t => Assert.IsType<TTokenText>(t));
    }

    [Fact]
    public void ProcessButtonText_ImgTag_NegativeIndex_NotRecognised()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a<Img:-1:0>", line, MakeFont());

        Assert.Equal("a<Img:-1:0>", result);          // I1 < 0 → 标记不识别，原样保留
        Assert.Equal(1, line.Count);
    }

    [Fact]
    public void ProcessButtonText_LooksTag_UsesBagItemLooksAndMod10000()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(1, 1);
        DxImageButtonExEnv.BagItemLooks = i => i == 10005 ? lib : null;
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("<Looks:10005:2:3>", line, MakeFont());

        Assert.Equal("", result);
        var img = Assert.IsType<TTokenImage>(Assert.Single(TokenList(line)));
        Assert.Same(lib, img.GameImages);
        Assert.Equal(5, img.ImageIndex);             // 10005 mod 10000
        Assert.Equal(2, img.OffsetX);
        Assert.Equal(3, img.OffsetY);
    }

    [Fact]
    public void ProcessButtonText_DnItemsTag_UsesDnItemLooks()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(1, 1);
        DxImageButtonExEnv.DnItemLooks = i => lib;
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<DnItems:20001:7:8>", line, MakeFont());

        var img = Assert.IsType<TTokenImage>(Assert.Single(TokenList(line)));
        Assert.Same(lib, img.GameImages);
        Assert.Equal(1, img.ImageIndex);             // 20001 mod 10000
        Assert.Equal(7, img.OffsetX);
        Assert.Equal(8, img.OffsetY);
    }

    [Fact]
    public void ProcessButtonText_StateItemTag_UsesStateItemLooks()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(1, 1);
        DxImageButtonExEnv.StateItemLooks = i => lib;
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<StateItem:3:1:2>", line, MakeFont());

        var img = Assert.IsType<TTokenImage>(Assert.Single(TokenList(line)));
        Assert.Same(lib, img.GameImages);
        Assert.Equal(3, img.ImageIndex);
    }

    [Fact]
    public void ProcessButtonText_NewopUITag_UsesGlobalLibraryWithoutMod()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(1, 1);
        DxImageButtonExEnv.NewopUIImages = lib;
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<NewopUI:12345:4:5>", line, MakeFont());

        var img = Assert.IsType<TTokenImage>(Assert.Single(TokenList(line)));
        Assert.Same(lib, img.GameImages);
        Assert.Equal(12345, img.ImageIndex);         // NewopUI 不做 mod
        Assert.Equal(4, img.OffsetX);
        Assert.Equal(5, img.OffsetY);
    }

    [Fact]
    public void ProcessButtonText_TagNameIsCaseInsensitive()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<img:1:0>", line, MakeFont());

        Assert.IsType<TTokenImage>(Assert.Single(TokenList(line)));   // SameText（原文 624）
    }

    [Fact]
    public void ProcessButtonText_PlayImgTag_CreatesTokenPlayImage()
    {
        using var scope = new Scope();
        var lib = new CachedLib().Add(1, 1);
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(lib);
        DxTickCount.Provider = () => 777;
        try
        {
            var line = new TTokenLine();
            string result = DxImageButtonExUnit.ProcessButtonText("<PlayImg:0:7:3:50:11:12:1>", line, MakeFont());

            Assert.Equal("", result);
            var img = Assert.IsType<TTokenPlayImage>(Assert.Single(TokenList(line)));
            Assert.Same(lib, img.GameImages);
            Assert.Equal(7, img.StartIndex);             // N → 播放起始图片
            Assert.Equal(7, img.DrawIndex);              // 原文 585 FDrawIndex := AImageIndex
            Assert.Equal(3, img.DrawCount);
            Assert.Equal(50, img.DrawTime);
            Assert.Equal(11, img.OffsetX);
            Assert.Equal(12, img.OffsetY);
            Assert.True(img.DrawBlend);                  // S7 != 0
        }
        finally { DxTickCount.Provider = null; }
    }

    [Fact]
    public void ProcessButtonText_PlayImgTag_NonPositiveTimeBecomes100()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<PlayImg:0:1:3:0>", line, MakeFont());

        Assert.Equal(100, ((TTokenPlayImage)line[0]).DrawTime);   // 原文 668-669
    }

    [Fact]
    public void ProcessButtonText_PlayImgTag_ZeroPlayCount_NoLibrary()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<PlayImg:0:1:0:50>", line, MakeFont());

        Assert.Equal(0, line.Count);                 // PlayCount=0 → 不取图库 → 不加 token（原文 671）
    }

    [Fact]
    public void ProcessButtonText_UnknownTag_KeptLiterally()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("ab<Foo:1:2>cd", line, MakeFont());

        Assert.Equal("ab<Foo:1:2>cd", result);
        Assert.Equal(1, line.Count);
        Assert.Equal("ab<Foo:1:2>cd", ((TTokenText)line[0]).Caption);
    }

    [Fact]
    public void ProcessButtonText_OpenTagWithoutClose_TreatedAsText()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("ab<Img:1", line, MakeFont());

        Assert.Equal("ab<Img:1", result);             // 原文 703-707：Index2 = 0 → 作为文本
        Assert.Equal(1, line.Count);
    }

    [Fact]
    public void ProcessButtonText_ShortTag_NotRecognised()
    {
        using var _ = new Scope();
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("a<>b", line, MakeFont());

        Assert.Equal("a<>b", result);                 // 原文 607 要求 Length(Text) > 2
        Assert.Equal(1, line.Count);
    }

    [Fact]
    public void ProcessButtonText_ImageTagFollowedByColorTag_BothHandled()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.GetRGB = n => n;
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("x<Img:1:0>y{z|7}w", line, MakeFont());

        Assert.Equal("xyzw", result);
        Assert.Equal(5, line.Count);
        Assert.IsType<TTokenText>(line[0]);
        Assert.IsType<TTokenImage>(line[1]);
        Assert.IsType<TTokenText>(line[2]);
        Assert.Equal(7, ((TTokenText)line[3]).FontColor);
        Assert.IsType<TTokenText>(line[4]);
    }

    [Fact]
    public void ProcessButtonText_TagAtStartAndEnd()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        string result = DxImageButtonExUnit.ProcessButtonText("<Img:1:0>mid<Img:2:0>", line, MakeFont());

        Assert.Equal("mid", result);
        Assert.Equal(3, line.Count);
        Assert.IsType<TTokenImage>(line[0]);
        Assert.IsType<TTokenText>(line[1]);
        Assert.IsType<TTokenImage>(line[2]);
    }

    [Fact]
    public void ProcessButtonText_OnlyTag_ReturnsEmpty()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        Assert.Equal("", DxImageButtonExUnit.ProcessButtonText("<Img:1:0>", line, MakeFont()));
        Assert.Equal(1, line.Count);
    }

    [Fact]
    public void ProcessButtonText_TwoPlayImgTags_EachGetsOwnToken()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();

        DxImageButtonExUnit.ProcessButtonText("<PlayImg:0:0:2:10:0:0:0><PlayImg:0:5:2:10:0:0:0>", line, MakeFont());

        Assert.Equal(2, line.Count);
        Assert.Equal(0, ((TTokenPlayImage)line[0]).StartIndex);
        Assert.Equal(5, ((TTokenPlayImage)line[1]).StartIndex);
    }

    // ===============================================================================
    // 九、接缝与辅助类型
    // ===============================================================================

    [Fact]
    public void ImageListStub_IndexerAndCount()
    {
        var lib = new CachedLib().Add(1, 1);
        var list = new TDxImageListStub().Add(lib);

        Assert.Equal(1, list.Count);
        Assert.Same(lib, list[0]);
        Assert.Null(list[1]);
        Assert.Null(list[-1]);
    }

    [Fact]
    public void DxImageLibraryExt_WithCachedInterface_ReturnsOffsets()
    {
        var lib = new CachedLib().Add(4, 5, offX: 9, offY: -9);

        var tex = DxImageLibraryExt.GetCachedImage(lib, 0, out int x, out int y);

        Assert.NotNull(tex);
        Assert.Equal(9, x);
        Assert.Equal(-9, y);
    }

    [Fact]
    public void DxImageLibraryExt_WithoutCachedInterface_DegradesToZeroOffsets()
    {
        var lib = new TDxImageLibraryStub().Add(new TDxTextureStub(3, 4));

        var tex = DxImageLibraryExt.GetCachedImage(lib, 0, out int x, out int y);

        Assert.NotNull(tex);
        Assert.Equal(3, tex.Width);
        Assert.Equal(0, x);
        Assert.Equal(0, y);
        Assert.Null(DxImageLibraryExt.GetCachedImage(lib, 9, out _, out _));
    }

    [Fact]
    public void Env_Defaults_AreHeadlessSafe()
    {
        DxImageButtonExEnv.Reset();

        Assert.IsType<TDxNullPainter>(DxImageButtonExEnv.Painter);
        Assert.Null(DxImageButtonExEnv.FindFont);
        Assert.Null(DxImageButtonExEnv.CurrentFont);
        Assert.Null(DxImageButtonExEnv.EffectImageList);
        Assert.Null(DxImageButtonExEnv.NewopUIImages);
        Assert.Null(DxImageButtonExEnv.BagItemLooks);
        Assert.Null(DxImageButtonExEnv.DnItemLooks);
        Assert.Null(DxImageButtonExEnv.StateItemLooks);
        Assert.Equal("", DxImageButtonExEnv.CurFontName);
        Assert.Equal(0, DxImageButtonExEnv.CurrentFontHeight);
        Assert.Equal(42, DxImageButtonExEnv.GetRGB(42));
        Assert.Equal(12, DxImageButtonExEnv.TextWidth(null, "ab"));
        Assert.Equal(12, DxImageButtonExEnv.TextHeight(null, "ab"));
    }

    [Fact]
    public void Env_TextOut_And_BoldTextOut_GoThroughPainter()
    {
        using var scope = new Scope();
        var font = new FakeFont();

        DxImageButtonExEnv.TextOut(font, 5, 6, "a", 0x11, 6, 12);
        Assert.Single(scope.Painter.Ops);
        Assert.Contains(",000011,", scope.Painter.Ops[0]);

        scope.Painter.Clear();
        DxImageButtonExEnv.BoldTextOut(font, 5, 6, "a", 0x11, 0x22, 6, 12);
        Assert.Equal(5, scope.Painter.Ops.Count);       // 4 次描边 + 1 次正文
        Assert.Contains(",000022,", scope.Painter.Ops[0]);
        Assert.Contains(",000022,", scope.Painter.Ops[3]);
        Assert.Contains(",000011,", scope.Painter.Ops[4]);
    }

    [Fact]
    public void Env_TextOut_NullFontOrEmptyText_NoOps()
    {
        using var scope = new Scope();
        DxImageButtonExEnv.TextOut(null, 0, 0, "a", 0, 1, 1);
        DxImageButtonExEnv.TextOut(new FakeFont(), 0, 0, "", 0, 1, 1);
        DxImageButtonExEnv.BoldTextOut(null, 0, 0, "a", 0, 0, 1, 1);
        DxImageButtonExEnv.BoldTextOut(new FakeFont(), 0, 0, "", 0, 0, 1, 1);
        Assert.Empty(scope.Painter.Ops);
    }

    [Fact]
    public void UnitConstants_MatchOriginal()
    {
        Assert.Equal(2, DxImageButtonExUnit.ExpandLineHeight);   // 原文 161
    }

    [Fact]
    public void RecalSize_WithPlayImageToken_DoesNotThrow()
    {
        // 原文 TTokenPlayImage.Initialize（287）里的 inherited 指向抽象方法，见源文件头第 8 条
        using var _ = new Scope();
        DxImageButtonExEnv.EffectImageList = new TDxImageListStub().Add(new CachedLib().Add(1, 1));
        var line = new TTokenLine();
        DxImageButtonExUnit.ProcessButtonText("<PlayImg:0:0:1:10:0:0:0>", line, MakeFont("F"));

        line.RecalSize();                        // 不得抛

        Assert.Equal(0, line.Width);             // PlayImg 是 TTokenPlayImage → 不计入行尺寸
        Assert.Equal(0, line.Height);
    }
}
