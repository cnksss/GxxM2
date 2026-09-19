using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DxImageProgress.pas 1-402 的 1:1 测试。
/// 覆盖 TProgressSetting 的默认值与通知链（SetImageType/SetOnGetImage/Changed 顺序/Assign 不对称）、
/// TDxImageProgress 的进度几何（ComputeProgressSrcRect）、值文本（ComputeValueText）、
/// 三条绘制守卫与 RecallAutoSize 的面积阈值。
/// </summary>
public sealed class DxImageProgressTests
{
    private static TDxRect R(int l, int t, int r, int b) => TDxRect.Rect(l, t, r, b);

    private static IDxImageLibrary Lib(params (int w, int h)[] sizes)
    {
        var lib = new TDxImageLibraryStub();
        foreach (var (w, h) in sizes)
            lib.Add(new TDxTextureStub(w, h));
        return lib;
    }

    private static TDxImageProgress NewProgress(int width = 90, int height = 20)
    {
        var c = new TDxImageProgress { Left = 0, Top = 0, Width = width, Height = height, Designing = false };
        c.FontEnv.GetImageInfos = (f, s) => TDxFontEnv.BuildImageInfos(s);
        c.FontEnv.TextWidth = (f, s) => TDxFontEnv.MeasureTextWidth(s);
        c.FontEnv.TextHeight = (f, s) => TDxFontEnv.MeasureTextHeight(s);
        c.FontEnv.FindFont = (n, sz, st) => new object();
        return c;
    }

    // =====================================================================
    // TProgressSetting 构造（109-141）
    // =====================================================================

    /// <summary>用例 1：TProgressSetting 构造默认值逐项（109-141）。</summary>
    [Fact]
    public void ProgressSetting_ConstructorDefaults()
    {
        var c = NewProgress();
        var s = c.ProgressSetting;

        Assert.Same(c, s.Owner);                          // 112 / 121
        Assert.True(s.Font.Bold);                         // 115
        Assert.Equal(TDxColor.clWhite, s.Font.Color);     // 116
        Assert.Equal(TImageType.Prguse_wil, s.ImageType); // 119
        Assert.Equal(-1, s.ImageBG);                      // 122
        Assert.Equal(-1, s.ImageProgress);                // 123
        Assert.Equal(0, s.ImageProgressX);                // 125
        Assert.Equal(0, s.ImageProgressY);                // 126
        Assert.Equal(TProgressValueType.vtValue, s.ValueType);      // 128
        Assert.Equal("-", s.ValueSplite);                 // 129（原文拼写）
        Assert.Equal(TDxAlignment.taCenter, s.ValueAlignment);      // 130
        Assert.Equal("", s.ValuePrefix);                  // 131
        Assert.Equal("", s.ValueSuffix);                  // 132
        Assert.Equal(100u, s.Max);                        // 134
        Assert.Equal(0u, s.Min);                          // 135
        Assert.Equal(50u, s.Value);                       // 136 —— **不是 0**
        Assert.Null(s.Image);                             // 138
    }

    /// <summary>用例 2（差异断言）：Value 默认是 **50**（不是 0/Max 的一半之类的巧合巧合值）—— 照抄原文。</summary>
    [Fact]
    public void ProgressSetting_DefaultValueIsFiftyNotZero()
    {
        var s = NewProgress().ProgressSetting;
        Assert.Equal(50u, s.Value);
        Assert.NotEqual(0u, s.Value);
        Assert.Equal(50u, (s.Max + s.Min) / 2);   // 恰为中点，但原文是硬编码 50
    }

    /// <summary>用例 3：TDxImageProgress 构造（229-238）—— Width=90、Height=20。</summary>
    [Fact]
    public void Progress_ConstructorDefaults()
    {
        var c = new TDxImageProgress();
        Assert.Equal(90, c.Width);
        Assert.Equal(20, c.Height);
        Assert.NotNull(c.ProgressSetting);
        // 控件构造里把 OnGetImage 接到 FProgressSetting（234）
        Assert.NotNull(c.ProgressSetting.OnGetImage);
    }

    // =====================================================================
    // 通知链（149-200）
    // =====================================================================

    /// <summary>用例 4：SetImageType 值变时才回调查询 + Changed（149-157）。</summary>
    [Fact]
    public void SetImageType_OnlyOnChange()
    {
        var s = NewProgress().ProgressSetting;
        int queries = 0, changes = 0;
        s.OnGetImage = (ps, t, img) => queries++;
        s.OnChange = _ => changes++;

        s.ImageType = TImageType.Prguse_wil;    // 同值
        Assert.Equal(0, queries);
        Assert.Equal(0, changes);

        s.ImageType = TImageType.UI_wil;        // 变更
        Assert.Equal(1, queries);
        Assert.Equal(1, changes);
        Assert.Equal(TImageType.UI_wil, s.ImageType);
    }

    /// <summary>
    /// 用例 5（差异断言）：SetOnGetImage 赋值后**无条件**立即回调一次 + Changed（159-165），
    /// 即使 ImageType 没变。
    /// </summary>
    [Fact]
    public void SetOnGetImage_InvokesImmediatelyThenChanges()
    {
        var s = NewProgress().ProgressSetting;
        var seen = new List<TImageType>();
        int changes = 0;
        s.OnChange = _ => changes++;

        s.SetOnGetImage((ps, t, img) => seen.Add(t));

        Assert.Single(seen);
        Assert.Equal(TImageType.Prguse_wil, seen[0]);
        Assert.Equal(1, changes);
    }

    /// <summary>
    /// 用例 6（差异断言）：Changed 的顺序是**先** Owner.RecallAutoSize、**再** OnChange（194-200）。
    /// </summary>
    [Fact]
    public void Changed_CallsRecallAutoSizeBeforeOnChange()
    {
        var c = NewProgress();
        var order = new List<string>();
        c.ProgressSetting.OnChange = _ => order.Add("onchange");
        c.ProgressSetting.SetImage(Lib((40, 30)));       // 直接注入图库（接缝）
        c.ProgressSetting.ImageBG = 0;                 // 触发 Changed → RecallAutoSize
        order.Clear();

        c.ProgressSetting.ImageBG = 1;                 // 再次触发
        Assert.Equal(new[] { "onchange" }, order);
        Assert.Equal(40, c.Width);                     // RecallAutoSize 已把它定尺到 40×30
        Assert.Equal(30, c.Height);
    }

    /// <summary>用例 7：Font 走 Assign（拷贝而非换引用），且 FontChange 转发到 OnChange。</summary>
    [Fact]
    public void Font_AssignAndForwardChange()
    {
        var s = NewProgress().ProgressSetting;
        var original = s.Font;
        int changes = 0;
        s.OnChange = _ => changes++;

        s.Font = new TDxFont { Name = "SimSun", Size = 16, Bold = false, Color = 0x0A0B0C };

        Assert.Same(original, s.Font);                // 引用未变
        Assert.Equal("SimSun", s.Font.Name);
        Assert.Equal(16, s.Font.Size);
        Assert.False(s.Font.Bold);
        Assert.Equal(0x0A0B0C, s.Font.Color);
        Assert.True(changes > 0);                     // 逐属性赋值触发 FontChange → OnChange
    }

    /// <summary>用例 8：ImageBG / ImageProgress 值变才 Changed。</summary>
    [Fact]
    public void ImageBGAndProgress_OnlyChangedOnRealChange()
    {
        var s = NewProgress().ProgressSetting;
        int n = 0;
        s.OnChange = _ => n++;

        s.ImageBG = -1;                 // 同值
        s.ImageProgress = -1;           // 同值
        Assert.Equal(0, n);

        s.ImageBG = 3;
        s.ImageProgress = 4;
        Assert.Equal(2, n);
    }

    // =====================================================================
    // Assign（202-225）
    // =====================================================================

    /// <summary>
    /// 用例 9（差异断言）：Assign 走 `OnGetImage := ...`（会立即回调一次）而 ImageType 是**直写字段**
    /// （不触发 OnGetImage 查询）—— 这种不对称是原文如此；且末尾还会 Changed 一次。
    ///
    /// 注意：原文 205 是 `OnGetImage := TProgressSetting(Source).OnGetImage;`，
    /// 即把**源**的回调直接搬过来；若源回调未挂，这次赋值不会触发任何查询。
    /// </summary>
    [Fact]
    public void Assign_OnGetImageFiresButImageTypeDoesNot()
    {
        var src = NewProgress().ProgressSetting;
        src.SetImageType(TImageType.UI2_wil);
        src.Font.Name = "SimSun";
        src.ImageBG = 7;
        src.ImageProgress = 8;
        src.ValueType = TProgressValueType.vtPercentage;
        src.ValueSplite = "/";
        src.ValueAlignment = TDxAlignment.taRightJustify;
        src.ValuePrefix = "[";
        src.ValueSuffix = "]";
        src.Max = 200;
        src.Min = 10;
        src.Value = 120;

        var dst = NewProgress().ProgressSetting;
        int dstQueries = 0, dstChanges = 0;
        dst.OnGetImage = (ps, t, img) => dstQueries++;
        dst.OnChange = _ => dstChanges++;

        dst.Assign(src);

        // 源未挂 OnGetImage → 这次赋值不再触发查询（原文语义）
        Assert.Equal(0, dstQueries);
        // ImageType 是直写字段：不经过 SetImageType，故不会产生查询
        Assert.Equal(TImageType.UI2_wil, dst.ImageType);

        Assert.Equal("SimSun", dst.Font.Name);
        Assert.Equal(7, dst.ImageBG);
        Assert.Equal(8, dst.ImageProgress);
        Assert.Equal(TProgressValueType.vtPercentage, dst.ValueType);
        Assert.Equal("/", dst.ValueSplite);
        Assert.Equal(TDxAlignment.taRightJustify, dst.ValueAlignment);
        Assert.Equal("[", dst.ValuePrefix);
        Assert.Equal("]", dst.ValueSuffix);
        Assert.Equal(200u, dst.Max);
        Assert.Equal(10u, dst.Min);
        Assert.Equal(120u, dst.Value);
        Assert.True(dstChanges > 0);                      // 末尾 Changed
    }

    /// <summary>
    /// 用例 9b（差异断言）：源**挂了** OnGetImage 时，Assign 的 `OnGetImage := src.OnGetImage`
    /// 会立即回调一次（此时用的是目标当前的 ImageType）。
    /// 注意：Assign 之后目标挂的就是**源**的回调，故观测点应加在源上。
    /// </summary>
    [Fact]
    public void Assign_WithSourceOnGetImage_FiresOnceOnTarget()
    {
        var src = NewProgress().ProgressSetting;
        src.SetImageType(TImageType.UI2_wil);
        var seen = new List<TImageType>();
        src.OnGetImage = (ps, t, img) => seen.Add(t);

        var dst = NewProgress().ProgressSetting;
        dst.Assign(src);

        // SetOnGetImage(src.OnGetImage) 内部立即回调一次（此时 ImageType 仍是 dst 的旧值 Prguse_wil）
        Assert.Single(seen);
        Assert.Equal(TImageType.Prguse_wil, seen[0]);
        // 随后 FImageType 直写为源值
        Assert.Equal(TImageType.UI2_wil, dst.ImageType);
        // 目标此刻挂的就是源的回调（原文 205 的引用搬运语义）
        dst.SetImageType(TImageType.UI3_wil);
        Assert.Equal(2, seen.Count);
    }

    /// <summary>用例 10：Assign 的源类型不匹配时什么都不做。</summary>
    [Fact]
    public void Assign_IgnoresNonProgressSetting()
    {
        var dst = NewProgress().ProgressSetting;
        dst.Max = 999;

        dst.Assign(null);

        Assert.Equal(999u, dst.Max);
    }

    // =====================================================================
    // ComputeProgressSrcRect（275-277）
    // =====================================================================

    /// <summary>用例 11：进度源矩形 —— Right = Round(宽 / (Max-Min) * Min(Value, Max))。</summary>
    [Theory]
    [InlineData(100, 20, 100u, 0u, 50u, 50)]     // 一半
    [InlineData(100, 20, 100u, 0u, 0u, 0)]       // 最小值
    [InlineData(100, 20, 100u, 0u, 100u, 100)]   // 最大值
    [InlineData(100, 20, 100u, 0u, 200u, 100)]   // 超界 → Min(Value, Max) 夹到 100（不是夹到区间）
    [InlineData(100, 20, 200u, 0u, 50u, 25)]     // 区间 200
    [InlineData(100, 20, 100u, 50u, 75u, 150)]   // Min=50 → 区间 50，75/50 = 1.5 → 150（超出一倍）
    public void ComputeProgressSrcRect_Formula(int w, int h, uint max, uint min, uint value, int expectedRight)
    {
        var r = TDxImageProgress.ComputeProgressSrcRect(w, h, max, min, value);
        Assert.Equal(0, r.Left);
        Assert.Equal(0, r.Top);
        Assert.Equal(h, r.Bottom);
        Assert.Equal(expectedRight, r.Right);
    }

    /// <summary>用例 12：进度源矩形用银行家舍入（Round(0.5)=0、Round(1.5)=2）。</summary>
    [Fact]
    public void ComputeProgressSrcRect_UsesBankersRounding()
    {
        // 宽 1、区间 2、值 1 → 1/2*1 = 0.5 → Round = 0（偶数）
        Assert.Equal(0, TDxImageProgress.ComputeProgressSrcRect(1, 1, 2, 0, 1).Right);
        // 宽 3、区间 2、值 1 → 3/2*1 = 1.5 → Round = 2（偶数）
        Assert.Equal(2, TDxImageProgress.ComputeProgressSrcRect(3, 1, 2, 0, 1).Right);
    }

    /// <summary>
    /// 用例 13（差异断言）：Value 超 Max 时夹到 **Max**（不是夹到 (Max-Min)）。
    /// 例：Max=100/Min=50/Value=1000 → Min(Value,Max)=100 → 宽 × 100/50 = 2 倍宽（超出一倍）。
    /// </summary>
    [Fact]
    public void ComputeProgressSrcRect_ValueAboveMaxClampsToMaxNotToRange()
    {
        var r = TDxImageProgress.ComputeProgressSrcRect(100, 10, 100, 50, 1000);
        Assert.Equal(200, r.Right);      // 100 / 50 * 100 = 200（而非夹到 100）
    }

    // =====================================================================
    // ComputeValueText（284-291）
    // =====================================================================

    /// <summary>用例 14：四种 ValueType 的文本（含 vtNone → 空串）。</summary>
    [Theory]
    [InlineData(TProgressValueType.vtValueAndMax, "50-100")]
    [InlineData(TProgressValueType.vtValue, "50")]
    [InlineData(TProgressValueType.vtPercentage, "50%")]
    [InlineData(TProgressValueType.vtNone, "")]
    public void ComputeValueText_AllValueTypes(TProgressValueType type, string expected)
    {
        Assert.Equal(expected, TDxImageProgress.ComputeValueText(type, "", "", "-", 100, 0, 50));
    }

    /// <summary>用例 15：前缀/后缀/分隔符拼接（vtValueAndMax 的顺序是 Prefix+Value+Splite+Max+Suffix）。</summary>
    [Fact]
    public void ComputeValueText_AffixesAndSplite()
    {
        Assert.Equal("[50/100]",
            TDxImageProgress.ComputeValueText(TProgressValueType.vtValueAndMax, "[", "]", "/", 100, 0, 50));
        Assert.Equal("HP50",
            TDxImageProgress.ComputeValueText(TProgressValueType.vtValue, "HP", "", "-", 100, 0, 50));
        Assert.Equal("50%!",
            TDxImageProgress.ComputeValueText(TProgressValueType.vtPercentage, "", "!", "-", 100, 0, 50));
    }

    /// <summary>
    /// 用例 16（差异断言）：百分比的**分母是 (Max-Min)**，不是 Max ——
    /// Max=100/Min=50/Value=75 → 75/50*100 = 150%（超过 100%）。
    /// </summary>
    [Fact]
    public void ComputeValueText_PercentageUsesRangeNotMax()
    {
        Assert.Equal("150%",
            TDxImageProgress.ComputeValueText(TProgressValueType.vtPercentage, "", "", "-", 100, 50, 75));
        Assert.NotEqual("75%",
            TDxImageProgress.ComputeValueText(TProgressValueType.vtPercentage, "", "", "-", 100, 50, 75));
    }

    /// <summary>用例 17：百分比走银行家舍入。</summary>
    [Theory]
    [InlineData(2u, 0u, 1u, "50%")]      // 1/2*100 = 50
    [InlineData(8u, 0u, 1u, "12%")]      // 12.5 → 12（偶数，银行家舍入）
    [InlineData(8u, 0u, 3u, "38%")]      // 37.5 → 38（偶数）
    public void ComputeValueText_PercentageRounding(uint max, uint min, uint value, string expected)
    {
        Assert.Equal(expected, TDxImageProgress.ComputeValueText(
            TProgressValueType.vtPercentage, "", "", "-", max, min, value));
    }

    // =====================================================================
    // RecallAutoSize（350-366）
    // =====================================================================

    /// <summary>用例 18：AutoSize 为假时 RecallAutoSize 直接 Exit。</summary>
    [Fact]
    public void RecallAutoSize_ExitsWhenAutoSizeFalse()
    {
        var c = NewProgress(90, 20);
        c.AutoSize = false;
        c.ProgressSetting.SetImage(Lib((40, 30)));
        c.ProgressSetting.ImageBG = 0;

        Assert.Equal(90, c.Width);
        Assert.Equal(20, c.Height);
    }

    /// <summary>
    /// 用例 19（差异断言）：Area 必须 **&gt; 4**（严格大于）才定尺 —— 2×2=4 不触发，3×2=6 触发。
    /// </summary>
    [Theory]
    [InlineData(1, 1, false)]     // 1
    [InlineData(2, 2, false)]     // 4（等于 4，不触发）
    [InlineData(1, 4, false)]     // 4
    [InlineData(1, 5, true)]      // 5
    [InlineData(3, 2, true)]      // 6
    public void RecallAutoSize_RequiresAreaStrictlyGreaterThanFour(int w, int h, bool expectResize)
    {
        var c = NewProgress(90, 20);
        c.ProgressSetting.SetImage(Lib((w, h)));
        c.ProgressSetting.ImageBG = 0;

        if (expectResize)
        {
            Assert.Equal(w, c.Width);
            Assert.Equal(h, c.Height);
        }
        else
        {
            Assert.Equal(90, c.Width);
            Assert.Equal(20, c.Height);
        }
    }

    /// <summary>用例 20（差异断言）：ImageBG &lt; 0 或图库为 nil 时不定尺（350-365 的两重守卫）。</summary>
    [Fact]
    public void RecallAutoSize_RequiresImageAndImageBG()
    {
        var c1 = NewProgress(90, 20);
        c1.ProgressSetting.SetImage(Lib((40, 30)));
        c1.ProgressSetting.ImageBG = -1;          // 未设置背景图索引
        Assert.Equal(90, c1.Width);

        var c2 = NewProgress(90, 20);
        c2.ProgressSetting.ImageBG = 0;           // 无图库
        Assert.Equal(90, c2.Width);
    }

    // =====================================================================
    // PaintTo（246-337）
    // =====================================================================

    /// <summary>
    /// 用例 21：VisibleRect 退化 → 一次绘制都不发（256）。
    /// 必须关掉 AutoSize，否则 RecallAutoSize 会按背景图把控件定尺回 40×30。
    /// </summary>
    [Fact]
    public void Paint_DegenerateVisibleRectDrawsNothing()
    {
        var c = NewProgress(0, 0);
        c.AutoSize = false;
        c.ProgressSetting.SetImage(Lib((40, 30)));
        c.ProgressSetting.ImageBG = 0;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.Empty(p.Ops);
        Assert.False(r.DrewBackground);
    }

    /// <summary>
    /// 用例 22：背景块 SrcRect = Rect(0, 0, Min(D.Width, Width), Min(D.Height, Height))。
    /// 注意：SetImage 的接缝回调会立刻触发 RecallAutoSize（Changed → Owner.RecallAutoSize），
    /// 故控件已被定尺到图幅 40×30，Min 取到的就是控件尺寸。
    /// </summary>
    [Fact]
    public void Paint_BackgroundSrcRectIsClampedToControlSize()
    {
        var c = NewProgress(30, 10);
        c.ProgressSetting.SetImage(Lib((40, 30)));
        c.ProgressSetting.ImageBG = 0;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.True(r.DrewBackground);
        Assert.Equal(R(0, 0, 40, 30), r.BackgroundSrcRect);
        Assert.Equal(40, c.Width);                  // RecallAutoSize 已按图幅定尺
        Assert.Equal(30, c.Height);
        Assert.StartsWith("Draw(0,0,(0,0,40,30)", p.Ops[0]);
    }

    /// <summary>用例 22b（差异断言）：控件比图小（AutoSize=False 时）SrcRect 才真正被 Min 夹到控件尺寸。</summary>
    [Fact]
    public void Paint_BackgroundSrcRectClampedWhenAutoSizeDisabled()
    {
        var c = NewProgress(30, 10);
        c.AutoSize = false;
        c.ProgressSetting.SetImage(Lib((40, 30)));
        c.ProgressSetting.ImageBG = 0;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.True(r.DrewBackground);
        Assert.Equal(R(0, 0, 30, 10), r.BackgroundSrcRect);
        Assert.StartsWith("Draw(0,0,(0,0,30,10)", p.Ops[0]);
    }

    /// <summary>用例 23：进度块的四条守卫（ImageProgress&gt;=0、Max&gt;Min、Value&gt;=Min、取到图）。</summary>
    [Fact]
    public void Paint_ProgressBlockGuards()
    {
        TDxImageProgressPaintResult Run(Action<TProgressSetting> setup)
        {
            var c = NewProgress();
            c.ProgressSetting.SetImage(Lib((40, 30), (100, 20)));
            c.ProgressSetting.ImageBG = 0;
            c.ProgressSetting.ImageProgress = 1;
            setup(c.ProgressSetting);
            var p = new TDxRecordingPainter();
            return c.PaintTo(p);
        }

        Assert.True(Run(s => { }).DrewProgress);

        Assert.False(Run(s => s.ImageProgress = -1).DrewProgress);          // 索引 < 0
        Assert.False(Run(s => { s.Max = 0; s.Min = 0; }).DrewProgress);     // Max == Min
        Assert.False(Run(s => { s.Max = 10; s.Min = 10; }).DrewProgress);   // Max == Min
        Assert.False(Run(s => { s.Min = 60; s.Value = 50; }).DrewProgress); // Value < Min
        Assert.True(Run(s => { s.Min = 60; s.Value = 60; }).DrewProgress);  // Value == Min（闭边界）
    }

    /// <summary>
    /// 用例 24（差异断言）：值文本块的守卫用的是 **ImageBG**（不是 ImageProgress）——
    /// 只设 ImageProgress 时进度条会画，但值文本**不画**。
    /// </summary>
    [Fact]
    public void Paint_ValueTextGuardUsesImageBGNotImageProgress()
    {
        var c = NewProgress();
        c.ProgressSetting.SetImage(Lib((40, 30), (100, 20)));
        c.ProgressSetting.ImageProgress = 1;    // 只设进度图，ImageBG 保持 -1
        c.ProgressSetting.Max = 100;
        c.ProgressSetting.Min = 0;
        c.ProgressSetting.Value = 50;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.True(r.DrewProgress);
        Assert.False(r.DrewValueText);
        Assert.Equal("", r.ValueText);
    }

    /// <summary>用例 25：ImageBG &gt;= 0 时计算值文本并按 ValueAlignment 绘制。</summary>
    [Fact]
    public void Paint_ValueTextComputedWhenImageBGSet()
    {
        var c = NewProgress();
        c.ProgressSetting.SetImage(Lib((40, 30)));
        c.ProgressSetting.ImageBG = 0;
        c.ProgressSetting.Font.Color = 0x123456;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.True(r.DrewValueText);
        Assert.Equal("50", r.ValueText);        // vtValue 默认 + Value=50
        Assert.Contains("TextRect(", string.Join("|", p.Ops));
    }

    /// <summary>用例 26：vtNone → 空文本 → 不绘制值文本（293 的 Length(S) &gt; 0 守卫）。</summary>
    [Fact]
    public void Paint_VtNoneProducesNoText()
    {
        var c = NewProgress();
        c.ProgressSetting.SetImage(Lib((40, 30)));
        c.ProgressSetting.ImageBG = 0;
        c.ProgressSetting.ValueType = TProgressValueType.vtNone;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.Equal("", r.ValueText);
        Assert.False(r.DrewValueText);
    }

    /// <summary>用例 27：Designing=True 时最后画边框（334-336）。</summary>
    [Fact]
    public void Paint_DesigningDrawsFrame()
    {
        var c = NewProgress();
        c.Designing = true;
        c.BorderColor.Up.Color = 0x606080;

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.True(r.DrewDesignFrame);
        Assert.Single(p.Ops);
        Assert.StartsWith("FrameRect(", p.Ops[0]);
        Assert.Contains("606080", p.Ops[0]);
    }

    /// <summary>用例 28：无图库时（Image=nil）什么都不画，但 Designing 边框仍画。</summary>
    [Fact]
    public void Paint_WithoutImageOnlyDesignFrame()
    {
        var c = NewProgress();
        c.ProgressSetting.ImageBG = 0;      // 有索引但无图库

        var p = new TDxRecordingPainter();
        var r = c.PaintTo(p);

        Assert.False(r.DrewBackground);
        Assert.False(r.DrewProgress);
        Assert.False(r.DrewValueText);
        Assert.Empty(p.Ops);
    }

    // =====================================================================
    // Assign（368-400）
    // =====================================================================

    /// <summary>用例 29：TDxImageProgress.Assign 拷贝控件属性 + 设置对象。</summary>
    [Fact]
    public void Assign_CopiesControlAndProgressSetting()
    {
        var src = NewProgress();
        src.Left = 3; src.Top = 4; src.Width = 55; src.Height = 66;
        src.Transparent = false;
        src.EnableFocus = true;
        src.Floating = true;
        src.OwnerMove = true;
        src.ReferenceX = TReferenceX.rxRight;
        src.AdjustYByHeight = true;
        src.TopAlignment = true;
        src.Center = true;
        src.Align = TDxAlign.alTop;
        src.BackgroundColor = 0x0A0B0C;
        src.ProgressSetting.Max = 321;

        var dst = new TDxImageProgress();
        dst.Assign(src);

        Assert.Equal(3, dst.Left);
        Assert.Equal(4, dst.Top);
        Assert.Equal(55, dst.Width);
        Assert.Equal(66, dst.Height);
        Assert.False(dst.Transparent);
        Assert.True(dst.EnableFocus);
        Assert.True(dst.Floating);
        Assert.True(dst.OwnerMove);
        Assert.Equal(TReferenceX.rxRight, dst.ReferenceX);
        Assert.True(dst.AdjustYByHeight);
        Assert.True(dst.TopAlignment);
        Assert.True(dst.Center);
        Assert.Equal(TDxAlign.alTop, dst.Align);
        Assert.Equal(0x0A0B0C, dst.BackgroundColor);
        Assert.Equal(321u, dst.ProgressSetting.Max);
    }

    /// <summary>用例 30：Assign 的源类型不匹配时什么都不做。</summary>
    [Fact]
    public void Assign_IgnoresNonProgressSource()
    {
        var dst = NewProgress();
        dst.Width = 1234;

        dst.Assign(new TDxLine());

        Assert.Equal(1234, dst.Width);
    }
}
