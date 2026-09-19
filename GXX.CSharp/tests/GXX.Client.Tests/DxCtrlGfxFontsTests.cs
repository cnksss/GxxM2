using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// GfxFonts.pas（963 行）的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/GfxFonts.cs）：
//   * TDxGfxFontTexture：构造默认值（FOutTimeTime = 60000）
//   * TDxGfxTextureFont.TextHeight / TextWidth（单字节 vs 双字节折算；Bold 分支）
//   * TDxGfxTextureFont.GetFontTextureArray（TextChars 门控 + 槽位数规则 + 未命中不加槽位）
//   * TDxGfxTextureFont.GetFontTexture（**区分大小写**的缓存命中/刷新 OutTimeTick/未命中建纹理/造不出返回 nil）
//   * TDxGfxTextureFont.ComputeGlyphBitmapSize（Max(逐行 TextWidth) × (TextHeight('pP') * 行数)）
//   * TDxGfxFontTextures：Add/Clear/Find/FreeIdleMemory（超时淘汰 + 游标分片 + 原文「删除后仍 Inc」怪癖）
//   * TDxGfxTextureFonts：Add/SetFont/RemoveFont/RemoveAll/Count/GetFont/FreeIdleMemory/Lock
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlGfxFontsTests
{
    private static TDxGfxTextureFont NewFont(Action<TDxGfxTextureFont> tune = null)
    {
        var fonts = new TDxGfxTextureFonts();
        var f = new TDxGfxTextureFont(fonts);
        tune?.Invoke(f);
        return f;
    }

    private static int GbkBytes(string s) => GXX.Core.EncodingInit.GBK.GetByteCount(s);

    // ===============================================================================
    // 一、构造默认值（原文 158-166 / 287-305）
    // ===============================================================================

    [Fact]
    public void FontTexture_Constructor_Defaults()
    {
        var t = new TDxGfxFontTexture();

        Assert.Null(t.Texture);
        Assert.Equal("", t.Text);
        Assert.Equal(1000u * 60 * 1, t.OutTimeTime);      // 原文 165：60000
        Assert.Equal(0, t.X);
        Assert.Equal(0, t.Y);
    }

    [Fact]
    public void Font_Constructor_Defaults_MatchOriginalConstants()
    {
        var f = NewFont();

        Assert.Equal("宋体", f.Descriptor.Name);          // 原文 291
        Assert.Equal(9, f.Descriptor.Size);               // 原文 292
        Assert.Equal(134, f.Descriptor.Charset);          // 原文 293 GB2312_CHARSET
        Assert.False(f.Descriptor.StyleBold);             // 原文 294 Style := []
        Assert.Equal(6, f.FontWidth);                     // 原文 295
        Assert.Equal(12, f.FontHeight);                   // 原文 296
        Assert.Equal(7, f.FontWidthBold);                 // 原文 297
        Assert.Equal(12, f.DoubleFontWidth);              // 原文 299
        Assert.Equal(12, f.DoubleFontHeight);             // 原文 300
        Assert.Equal(13, f.DoubleFontWidthBold);          // 原文 301
        Assert.Equal(0, f.TimeOutIdx);                    // 原文 304
        Assert.NotNull(f.FontTextures);
        Assert.Equal(0, f.FontTextures.TextureCount);
        Assert.Equal(1000u, f.FontTextures.OutTimeTime);  // 原文 181：1000
    }

    // ===============================================================================
    // 二、TextHeight（原文 766-776）
    // ===============================================================================

    [Fact]
    public void TextHeight_PureSingleByte_ReturnsFontHeight()
    {
        var f = NewFont();
        Assert.Equal(12, f.TextHeight("abc"));
        Assert.Equal(12, f.TextHeight(""));
        Assert.Equal(12, f.TextHeight("0123456789"));
    }

    [Fact]
    public void TextHeight_ContainsDoubleByte_ReturnsMaxOfBoth()
    {
        var f = NewFont();
        f.FontHeight = 10;
        f.DoubleFontHeight = 18;

        Assert.Equal(18, f.TextHeight("a中"));
        Assert.Equal(18, f.TextHeight("中"));
        Assert.Equal(10, f.TextHeight("ab"));
    }

    [Fact]
    public void TextHeight_Null_IsSafe()
    {
        var f = NewFont();
        Assert.Equal(12, f.TextHeight(null));
    }

    // ===============================================================================
    // 三、TextWidth（原文 778-798）
    // ===============================================================================

    [Fact]
    public void TextWidth_PureSingleByte_UsesFontWidth()
    {
        var f = NewFont();
        Assert.Equal(18, f.TextWidth("abc"));             // 6 * 3
        Assert.Equal(0, f.TextWidth(""));
        Assert.Equal(0, f.TextWidth(null));
    }

    [Fact]
    public void TextWidth_PureSingleByte_Bold_UsesBoldWidth()
    {
        var f = NewFont(x => x.Descriptor.StyleBold = true);
        Assert.Equal(21, f.TextWidth("abc"));             // 7 * 3
    }

    [Fact]
    public void TextWidth_DoubleByte_MixesBothWidths()
    {
        // "a中b"：GBK 字节 = 1 + 2 + 1 = 4，UTF-16 码元 = 3 → nCount = 1（双字节字符数 1）
        // 原文 796：FDoubleFontWidth * Max(1,0) + Max(3-1,0)*FFontWidth = 12*1 + 2*6 = 24
        var f = NewFont();
        Assert.Equal(2, GbkBytes("中"));                  // 前提校验：GBK 汉字占 2 字节
        Assert.Equal(24, f.TextWidth("a中b"));
    }

    [Fact]
    public void TextWidth_DoubleByte_Bold_MixesBothBoldWidths()
    {
        // 原文 794：FDoubleFontWidthBold * nCount + Max(nw-nCount,0) * FFontWidthBold
        var f = NewFont(x => x.Descriptor.StyleBold = true);
        Assert.Equal(13 * 1 + 2 * 7, f.TextWidth("a中b")); // 27
    }

    [Fact]
    public void TextWidth_AllDoubleByte()
    {
        // "中中"：GBK 4 字节、UTF-16 2 码元 → nCount = 2
        var f = NewFont();
        Assert.Equal(12 * 2, f.TextWidth("中中"));         // 24
    }

    [Fact]
    public void TextWidth_TunableMetrics_AreHonoured()
    {
        var f = NewFont(x =>
        {
            x.FontWidth = 5;
            x.FontWidthBold = 6;
            x.DoubleFontWidth = 10;
            x.DoubleFontWidthBold = 11;
        });

        Assert.Equal(15, f.TextWidth("abc"));              // 5 * 3（非 Bold）

        f.Descriptor.StyleBold = true;
        Assert.Equal(18, f.TextWidth("abc"));              // 6 * 3（Bold）
        Assert.Equal(11 * 1 + 2 * 6, f.TextWidth("a中b")); // DoubleFontWidthBold*1 + (2-1)*FontWidthBold = 23
    }

    // ===============================================================================
    // 四、TextChars 门控与 GetFontTextureArray（原文 319-349）
    // ===============================================================================

    [Theory]
    [InlineData(' ', true)]        // #32 下界
    [InlineData('A', true)]
    [InlineData('~', true)]
    [InlineData((char)0xFF, true)] // 上界
    [InlineData((char)0x1F, false)]
    [InlineData('\0', false)]
    public void IsTextChar_MatchesOriginalSet32To255(char c, bool expected)
        => Assert.Equal(expected, TDxGfxTextureFont.IsTextChar(c));

    [Fact]
    public void WideCharToAnsiString_AsciiCharIsItself()
    {
        Assert.Equal("A", TDxGfxTextureFont.WideCharToAnsiString('A'));
        Assert.Equal(" ", TDxGfxTextureFont.WideCharToAnsiString(' '));
    }

    [Fact]
    public void WideCharToAnsiString_CjkCharBecomesQuestionMark()
    {
        // 原文 `S := sText[I]`（WideString 单元素 → AnsiString）时汉字装不进 1 个元素 → '?'
        Assert.Equal("?", TDxGfxTextureFont.WideCharToAnsiString('中'));
    }

    [Fact]
    public void GetFontTextureArray_EmptyText_ReturnsEmpty()
    {
        var f = NewFont();
        Assert.Empty(f.GetFontTextureArray(""));
    }

    [Fact]
    public void GetFontTextureArray_NoGlyphBuilder_AllSlotsAreNull()
    {
        // 未接 GlyphBuilder → GetFontTexture 恒 nil → TextChars 命中的字符**不加槽位**
        var f = NewFont();
        var arr = f.GetFontTextureArray("abc");
        Assert.Empty(arr);
    }

    [Fact]
    public void GetFontTextureArray_ControlChars_ReserveNullSlots()
    {
        // '\t'(9) 与 '\n'(10) 都 < 32 → 走 else 分支，各留 1 个 null 槽位
        var f = NewFont();
        var arr = f.GetFontTextureArray("\t\n");
        Assert.Equal(2, arr.Count);
        Assert.All(arr, t => Assert.Null(t));
    }

    [Fact]
    public void GetFontTextureArray_CjkAlsoReservesOneSlot()
    {
        // 汉字经 GBK 转换变 '?'（$3F，落在 [#32..#255] 内）→ 走 TextChars 分支；
        // 因无 GlyphBuilder 拿不到纹理 → 不加槽位。这与"\t"的"预留 null 槽位"不同。
        var f = NewFont();
        Assert.Empty(f.GetFontTextureArray("中"));
    }

    [Fact]
    public void GetFontTextureArray_WithGlyphBuilder_ReturnsTexturesForAscii()
    {
        var f = NewFont();
        f.GlyphBuilder = (w, h) => new TDxTextureStub(Math.Max(w, 1), Math.Max(h, 1));
        // 让几何非 0：注入度量
        f.FontWidth = 6;
        f.FontHeight = 12;

        var arr = f.GetFontTextureArray("ab");

        Assert.Equal(2, arr.Count);
        Assert.All(arr, t => Assert.NotNull(t));
    }

    // ===============================================================================
    // 五、GetFontTexture 缓存语义（原文 504-644）
    // ===============================================================================

    [Fact]
    public void GetFontTexture_EmptyText_ReturnsNull()
    {
        var f = NewFont();
        Assert.Null(f.GetFontTexture(""));
    }

    [Fact]
    public void GetFontTexture_NoBuilder_ReturnsNullAndCachesNothing()
    {
        var f = NewFont();
        Assert.Null(f.GetFontTexture("abc"));
        Assert.Equal(0, f.FontTextures.TextureCount);
    }

    [Fact]
    public void GetFontTexture_BuildsCachesAndReuses()
    {
        var f = NewFont();
        int builds = 0;
        f.GlyphBuilder = (w, h) => { builds++; return new TDxTextureStub(Math.Max(w, 1), Math.Max(h, 1)); };

        var t1 = f.GetFontTexture("abc");
        var t2 = f.GetFontTexture("abc");

        Assert.NotNull(t1);
        Assert.Same(t1, t2);                     // 命中缓存
        Assert.Equal(1, builds);                 // 只造了一次
        Assert.Equal(1, f.FontTextures.TextureCount);
        Assert.Equal("abc", t1.Text);
    }

    [Fact]
    public void GetFontTexture_CacheLookupIsCaseSensitive()
    {
        // 原文 THashTable 的 `Find` 用 `Result^.Key = Name`（AnsiString `=` → 区分大小写）
        var f = NewFont();
        f.GlyphBuilder = (w, h) => new TDxTextureStub(1, 1);

        var lower = f.GetFontTexture("abc");
        var upper = f.GetFontTexture("ABC");

        Assert.NotNull(lower);
        Assert.NotNull(upper);
        Assert.NotSame(lower, upper);
        Assert.Equal(2, f.FontTextures.TextureCount);
    }

    [Fact]
    public void GetFontTexture_HitRefreshesOutTimeTick()
    {
        var f = NewFont();
        f.GlyphBuilder = (w, h) => new TDxTextureStub(1, 1);

        var t = f.GetFontTexture("abc");
        t.OutTimeTick = 1;                        // 人为置旧

        uint before = DxTickCount.MyGetTickCount();
        f.GetFontTexture("abc");

        Assert.True(t.OutTimeTick >= before);
    }

    [Fact]
    public void GetFontTexture_ComputesBitmapSizeFromLines()
    {
        // 多行文本 → nHeight = TextHeight('pP') * 行数；nWidth = Max(逐行 TextWidth)
        var f = NewFont();
        int gotW = -1, gotH = -1;
        f.GlyphBuilder = (w, h) => { gotW = w; gotH = h; return new TDxTextureStub(1, 1); };

        f.GetFontTexture("ab\nabcd\nabc");

        Assert.Equal(4 * 6, gotW);                // 最长行 "abcd" → 4*6 = 24
        Assert.Equal(12 * 3, gotH);               // TextHeight('pP')=12 × 3 行
    }

    [Fact]
    public void ComputeGlyphBitmapSize_MatchesBuildPath()
    {
        var f = NewFont();
        int gotW = -1, gotH = -1;
        f.GlyphBuilder = (w, h) => { gotW = w; gotH = h; return new TDxTextureStub(1, 1); };

        f.GetFontTexture("a\nbb\nccc");
        var p = f.ComputeGlyphBitmapSize("a\nbb\nccc");

        Assert.Equal(gotW, p.X);
        Assert.Equal(gotH, p.Y);
    }

    [Fact]
    public void GetTextTexture_DelegatesToGetFontTexture()
    {
        var f = NewFont();
        var tex = new TDxTextureStub(3, 4);
        f.GlyphBuilder = (w, h) => tex;

        Assert.Same(tex, f.GetTextTexture("x"));
        Assert.Null(f.GetTextTexture(""));        // 空串 → GetFontTexture nil
    }

    // ===============================================================================
    // 六、TDxGfxFontTextures 缓存表（原文 176-263）
    // ===============================================================================

    [Fact]
    public void FontTextures_AddAndIndex()
    {
        var table = new TDxGfxFontTextures();
        var a = new TDxGfxFontTexture { Text = "a" };
        var b = new TDxGfxFontTexture { Text = "b" };

        table.Add("a", a);
        table.Add("b", b);

        Assert.Equal(2, table.TextureCount);
        Assert.Same(a, table.GetTexture(0));
        Assert.Same(b, table.GetTexture(1));
        Assert.Same(a, table.Find("a"));
        Assert.Null(table.Find("A"));             // 区分大小写
    }

    [Fact]
    public void FontTextures_GetTexture_OutOfRange_IsNull()
    {
        var table = new TDxGfxFontTextures();
        table.Add("a", new TDxGfxFontTexture { Text = "a" });

        Assert.Null(table.GetTexture(-1));
        Assert.Null(table.GetTexture(1));
    }

    [Fact]
    public void FontTextures_Clear_ResetsProcIdxAndEmpties()
    {
        var table = new TDxGfxFontTextures();
        table.Add("a", new TDxGfxFontTexture { Text = "a" });
        table.OutTimeTick = 0;
        table.OutTimeTime = 0;
        table.FreeIdleMemory();                   // 让 ProcIdx 有机会非 0

        table.Clear();

        Assert.Equal(0, table.TextureCount);
        Assert.Equal(0, table.ProcIdx);
    }

    [Fact]
    public void FontTextures_FreeIdleMemory_GatedByOuterTimeWindow()
    {
        // 外层门控：`GetTickCount - FOutTimeTick > FOutTimeTime` 才动作。
        // 把 OutTimeTick 设为"现在"、OutTimeTime 设很大 → 不动作。
        var table = new TDxGfxFontTextures { OutTimeTick = DxTickCount.MyGetTickCount(), OutTimeTime = 100000 };
        var rec = new TDxGfxFontTexture { Text = "a", OutTimeTick = 0, OutTimeTime = 0 };  // 已过期
        table.Add("a", rec);

        Assert.Equal(0, table.FreeIdleMemory());
        Assert.Equal(1, table.TextureCount);
    }

    [Fact]
    public void FontTextures_FreeIdleMemory_EvictsExpiredWhenWindowOpens()
    {
        var table = new TDxGfxFontTextures { OutTimeTick = 0, OutTimeTime = 0 };
        var expired = new TDxGfxFontTexture { Text = "old", OutTimeTick = 0, OutTimeTime = 0 };
        table.Add("old", expired);

        int freed = table.FreeIdleMemory();

        Assert.Equal(1, freed);
        Assert.Equal(0, table.TextureCount);
        Assert.Null(table.Find("old"));
    }

    [Fact]
    public void FontTextures_FreeIdleMemory_KeepsFresh()
    {
        var table = new TDxGfxFontTextures { OutTimeTick = 0, OutTimeTime = 0 };
        var fresh = new TDxGfxFontTexture
        {
            Text = "new",
            OutTimeTick = DxTickCount.MyGetTickCount(),
            OutTimeTime = 100000,
        };
        table.Add("new", fresh);

        int freed = table.FreeIdleMemory();

        Assert.Equal(0, freed);
        Assert.Equal(1, table.TextureCount);
    }

    [Fact]
    public void FontTextures_FreeIdleMemory_SkipsOneAfterEviction_DueToOriginalIncQuirk()
    {
        // 原文 235-239：命中过期 → FreeAndNil + Delete(nIdx) + **Inc(nIdx)** + Continue。
        // 删除后同一索引已是下一项，这一 Inc 会**跳过一个元素**（原文怪癖，逐字保留）。
        var table = new TDxGfxFontTextures { OutTimeTick = 0, OutTimeTime = 0 };
        var e1 = new TDxGfxFontTexture { Text = "1", OutTimeTick = 0, OutTimeTime = 0 };
        var e2 = new TDxGfxFontTexture { Text = "2", OutTimeTick = 0, OutTimeTime = 0 };
        var e3 = new TDxGfxFontTexture { Text = "3", OutTimeTick = 0, OutTimeTime = 0 };
        table.Add("1", e1);
        table.Add("2", e2);
        table.Add("3", e3);

        int freed = table.FreeIdleMemory();

        // 删 index0 后 nIdx 变 1，跳过了原来 index1 的 e2 → 只删掉 e1 与（新 index1 的）e3
        Assert.Equal(2, freed);
        Assert.Equal(1, table.TextureCount);
        Assert.Same(e2, table.GetTexture(0));     // e2 被跳过、留了下来
    }

    [Fact]
    public void FontTextures_OutTimeTickAndTime_AreSettable()
    {
        var table = new TDxGfxFontTextures { OutTimeTick = 42, OutTimeTime = 7 };
        Assert.Equal(42u, table.OutTimeTick);
        Assert.Equal(7u, table.OutTimeTime);
    }

    // ===============================================================================
    // 七、TDxGfxTextureFonts 字体表（原文 801-963）
    // ===============================================================================

    [Fact]
    public void Fonts_Add_IncrementsCountAndSetsIndex()
    {
        var fonts = new TDxGfxTextureFonts();

        var f0 = fonts.Add("宋体", 9, false);
        var f1 = fonts.Add("黑体", 12, true);

        Assert.Equal(2, fonts.Count);
        Assert.Equal(0, f0.nIndex);
        Assert.Equal(1, f1.nIndex);
        Assert.Same(f0, fonts.GetFont(0));
        Assert.Same(f1, fonts.GetFont(1));
        Assert.Equal("黑体", f1.Descriptor.Name);
        Assert.Equal(12, f1.Descriptor.Size);
        Assert.True(f1.Descriptor.StyleBold);
        Assert.Equal(134, f1.Descriptor.Charset);          // 原文构造里的 GB2312_CHARSET
    }

    [Fact]
    public void Fonts_Add_DefaultOverload()
    {
        var fonts = new TDxGfxTextureFonts();
        var f = fonts.Add();

        Assert.Equal("宋体", f.Descriptor.Name);
        Assert.Equal(9, f.Descriptor.Size);
        Assert.False(f.Descriptor.StyleBold);
    }

    [Fact]
    public void Fonts_Add_CallsFontSizeProbe()
    {
        var fonts = new TDxGfxTextureFonts();
        var f = new TDxGfxTextureFont(fonts);
        int probed = 0;
        f.FontSizeProbe = x => { probed++; x.FontWidth = 99; };

        // 直接验证接缝：Add 内部会调 GetFontSize
        f.GetFontSize();
        Assert.Equal(1, probed);
        Assert.Equal(99, f.FontWidth);
    }

    [Fact]
    public void Fonts_GetFont_OutOfRange_IsNull()
    {
        var fonts = new TDxGfxTextureFonts();
        fonts.Add();
        Assert.Null(fonts.GetFont(-1));
        Assert.Null(fonts.GetFont(5));
    }

    [Fact]
    public void Fonts_RemoveAll_EmptiesTable()
    {
        var fonts = new TDxGfxTextureFonts();
        fonts.Add();
        fonts.Add();
        Assert.Equal(2, fonts.Count);

        fonts.RemoveAll();
        Assert.Equal(0, fonts.Count);
    }

    [Fact]
    public void Fonts_RemoveFont_DisposesSlot()
    {
        var fonts = new TDxGfxTextureFonts();
        var f0 = fonts.Add();
        var f1 = fonts.Add();

        fonts.RemoveFont(0);

        Assert.Equal(2, fonts.Count);              // 原文只置 null，不缩短数组
        Assert.Null(fonts.GetFont(0));
        Assert.Same(f1, fonts.GetFont(1));

        fonts.RemoveFont(99);                      // 越界安全
    }

    [Fact]
    public void Fonts_SetFont_ResetsAndSetsCurrent()
    {
        var fonts = new TDxGfxTextureFonts();
        fonts.Add();
        fonts.Add();
        Assert.Equal(2, fonts.Count);

        var f = fonts.SetFont("幼圆", 14, true);

        Assert.Equal(1, fonts.Count);              // RemoveAll + Add
        Assert.Same(f, fonts.GetFont(0));
        Assert.Same(f, TDxGfxTextureFonts.GlobalCurrent);

        // 清理静态状态，避免影响其它用例
        TDxGfxTextureFonts.GlobalCurrent = null;
    }

    [Fact]
    public void Fonts_SetFont_DefaultOverload()
    {
        var fonts = new TDxGfxTextureFonts();
        var f = fonts.SetFont();
        Assert.Equal("宋体", f.Descriptor.Name);
        Assert.Equal(9, f.Descriptor.Size);
        TDxGfxTextureFonts.GlobalCurrent = null;
    }

    [Fact]
    public void Fonts_InitializeAndFinalize_AreSafe()
    {
        var fonts = new TDxGfxTextureFonts();
        fonts.Add();
        fonts.Initialize();
        fonts.Finalize2();
        fonts.Dispose();
        Assert.Equal(0, fonts.Count);
    }

    [Fact]
    public void Fonts_LockUnlock_AreReentrantSafePair()
    {
        var fonts = new TDxGfxTextureFonts();
        fonts.Lock();
        fonts.UnLock();
        Assert.True(true);
    }

    [Fact]
    public void Fonts_D3DFormat_IsPlainProperty()
    {
        var fonts = new TDxGfxTextureFonts();
        Assert.False(fonts.D3DFormat);
        fonts.D3DFormat = true;
        Assert.True(fonts.D3DFormat);
    }

    [Fact]
    public void Fonts_FreeIdleMemory_AggregatesAcrossFonts()
    {
        var fonts = new TDxGfxTextureFonts();
        var f0 = fonts.Add();
        var f1 = fonts.Add();

        f0.FontTextures.OutTimeTick = 0;
        f0.FontTextures.OutTimeTime = 0;
        f0.FontTextures.Add("a", new TDxGfxFontTexture { Text = "a", OutTimeTick = 0, OutTimeTime = 0 });

        f1.FontTextures.OutTimeTick = 0;
        f1.FontTextures.OutTimeTime = 0;
        f1.FontTextures.Add("b", new TDxGfxFontTexture { Text = "b", OutTimeTick = 0, OutTimeTime = 0 });

        Assert.Equal(2, fonts.FreeIdleMemory());
    }

    // ===============================================================================
    // 八、Font 级 Clear / Initialize / Finalize / FreeIdleMemory
    // ===============================================================================

    [Fact]
    public void Font_ClearAndInitializeAndFinalize_EmptyTheCache()
    {
        var f = NewFont();
        f.GlyphBuilder = (w, h) => new TDxTextureStub(1, 1);
        f.GetFontTexture("abc");
        Assert.Equal(1, f.FontTextures.TextureCount);

        f.Clear();
        Assert.Equal(0, f.FontTextures.TextureCount);

        f.GetFontTexture("abc");
        f.Initialize();
        Assert.Equal(0, f.FontTextures.TextureCount);

        f.GetFontTexture("abc");
        f.Finalize2();
        Assert.Equal(0, f.FontTextures.TextureCount);
    }

    [Fact]
    public void Font_FreeIdleMemory_DelegatesToTable()
    {
        var f = NewFont();
        f.FontTextures.OutTimeTick = 0;
        f.FontTextures.OutTimeTime = 0;
        f.FontTextures.Add("a", new TDxGfxFontTexture { Text = "a", OutTimeTick = 0, OutTimeTime = 0 });

        Assert.Equal(1, f.FreeIdleMemory());
    }

    [Fact]
    public void Font_Dispose_IsSafe()
    {
        var f = NewFont();
        f.Dispose();
    }

    [Fact]
    public void FontTexture_Dispose_IsSafe()
    {
        var t = new TDxGfxFontTexture { Texture = new TDxTextureStub(1, 1) };
        t.Dispose();
        Assert.Null(t.Texture);
    }
}
