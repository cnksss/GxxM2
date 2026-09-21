using System;
using GXX.Client.GUI.Mir;
using Xunit;

namespace GXX.Client.Tests;

// ============================================================================================
// 【P17 切片C / p17-client-mshare】hint 字体族 7 条（原文 11703-11760）
//
// 全部只读 `g_ClientConfig`（原文写错的符号；2180 真正的声明是 `g_ConfigClient` ⇒ 见 P17-DEF-04）。
// 字段在切片A 已补进 `GUI/Mir/MirForms.cs::TConfigClient`。
//
// 行号 = `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号。
// ============================================================================================

public class MShareHintFontFamilyP17Tests
{
    /// <summary>Delphi Graphics.TFontStyles 的本地别名（与生产代码同源）。</summary>
    private const GXX.Client.GUI.Share.TFontStyles fsNone = GXX.Client.GUI.Share.TFontStyles.fsNone;
    private const GXX.Client.GUI.Share.TFontStyles fsBold = GXX.Client.GUI.Share.TFontStyles.fsBold;
    private const GXX.Client.GUI.Share.TFontStyles fsItalic = GXX.Client.GUI.Share.TFontStyles.fsItalic;
    private const GXX.Client.GUI.Share.TFontStyles fsUnderline = GXX.Client.GUI.Share.TFontStyles.fsUnderline;

    public MShareHintFontFamilyP17Tests() => MShareGlobalsReset.ResetForTests();

    // ------------------------------------------------------------------------------------
    // GetHintNameFontName（原文 11703-11706）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetHintNameFontName_ReadsShowHintFontName()
    {
        // 原文 11705：Result := g_ClientConfig.sShowHintFontName;
        Assert.Equal("", MShareFunctions.GetHintNameFontName());

        MShareGlobals.g_ConfigClient.ShowHintFontName = "宋体";
        Assert.Equal("宋体", MShareFunctions.GetHintNameFontName());

        MShareGlobals.g_ConfigClient.ShowHintFontName = "微软雅黑";
        Assert.Equal("微软雅黑", MShareFunctions.GetHintNameFontName());
    }

    [Fact]
    public void GetHintNameFontName_ShortStringIsLengthLimitedTo20Bytes()
    {
        // 原文 `sShowHintFontName:array[0..20] of AnsiChar`（string[20]）⇒ 超过 20 字节被截断
        MShareGlobals.g_ConfigClient.ShowHintFontName = new string('A', 30);
        Assert.Equal(20, MShareFunctions.GetHintNameFontName().Length);
        Assert.Equal(new string('A', 20), MShareFunctions.GetHintNameFontName());
    }

    // ------------------------------------------------------------------------------------
    // GetHintNameFontSize（原文 11708-11711）
    // ------------------------------------------------------------------------------------

    [Theory]
    [InlineData(0, 0)]
    [InlineData(9, 9)]
    [InlineData(12, 12)]
    [InlineData(255, 255)]
    public void GetHintNameFontSize_ReadsByteAsInt(byte raw, int expected)
    {
        // 原文 11710：Result := g_ClientConfig.btShowHintNameFontSize;（Integer := Byte 隐式提升）
        MShareGlobals.g_ConfigClient.btShowHintNameFontSize = raw;
        Assert.Equal(expected, MShareFunctions.GetHintNameFontSize());
    }

    // ------------------------------------------------------------------------------------
    // GetHintNameFontStyle（原文 11713-11723）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetHintNameFontStyle_Case0_PassesThroughInputStyles()
    {
        // 原文 11716-11717：0: Result := FontStyles;
        MShareGlobals.g_ConfigClient.btShowHintNameFontBold = 0;
        Assert.Equal(fsNone, MShareFunctions.GetHintNameFontStyle(fsNone));
        Assert.Equal(fsBold, MShareFunctions.GetHintNameFontStyle(fsBold));
        Assert.Equal(fsItalic, MShareFunctions.GetHintNameFontStyle(fsItalic));
        Assert.Equal(fsItalic | fsUnderline, MShareFunctions.GetHintNameFontStyle(fsItalic | fsUnderline));
    }

    [Fact]
    public void GetHintNameFontStyle_Case1_ForcesEmptySet()
    {
        // 原文 11718-11719：1: Result := [];
        MShareGlobals.g_ConfigClient.btShowHintNameFontBold = 1;
        Assert.Equal(fsNone, MShareFunctions.GetHintNameFontStyle(fsNone));
        Assert.Equal(fsNone, MShareFunctions.GetHintNameFontStyle(fsBold));
        Assert.Equal(fsNone, MShareFunctions.GetHintNameFontStyle(fsItalic | fsUnderline));
    }

    [Fact]
    public void GetHintNameFontStyle_Case2_ForcesBoldOnly()
    {
        // 原文 11720-11721：2: Result := [fsBold];
        MShareGlobals.g_ConfigClient.btShowHintNameFontBold = 2;
        Assert.Equal(fsBold, MShareFunctions.GetHintNameFontStyle(fsNone));
        Assert.Equal(fsBold, MShareFunctions.GetHintNameFontStyle(fsItalic));
        Assert.Equal(fsBold, MShareFunctions.GetHintNameFontStyle(fsItalic | fsUnderline)); // 仍然只 bold
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(255)]
    public void GetHintNameFontStyle_NoElseBranch_OutOfRangeYieldsEmptySet_NotTheInput(byte raw)
    {
        // ★ 原文 11715-11722 的 `case` **没有 else** ⇒ Delphi 下 Result 保持函数入口的默认值
        //   （TFontStyles 是 set ⇒ 默认 `[]`）。**不是**入参 FontStyles。
        //   传入 fsBold 时必须得到 fsNone —— 这条区分了"照抄"与"想当然"。
        MShareGlobals.g_ConfigClient.btShowHintNameFontBold = raw;
        Assert.Equal(fsNone, MShareFunctions.GetHintNameFontStyle(fsBold));
        Assert.Equal(fsNone, MShareFunctions.GetHintNameFontStyle(fsItalic));
    }

    // ------------------------------------------------------------------------------------
    // GetHintNameFontStroke（原文 11725-11733）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetHintNameFontStroke_Case0_PassesThroughInput()
    {
        // 原文 11728：0: Result := IsStroke;
        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = 0;
        Assert.False(MShareFunctions.GetHintNameFontStroke(false));
        Assert.True(MShareFunctions.GetHintNameFontStroke(true));
    }

    [Fact]
    public void GetHintNameFontStroke_Case1And2_ForceFalseAndTrue()
    {
        // 原文 11729-11730
        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = 1;
        Assert.False(MShareFunctions.GetHintNameFontStroke(false));
        Assert.False(MShareFunctions.GetHintNameFontStroke(true));   // 输入 true 也被压成 false

        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = 2;
        Assert.True(MShareFunctions.GetHintNameFontStroke(false));   // 输入 false 也被抬成 true
        Assert.True(MShareFunctions.GetHintNameFontStroke(true));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(255)]
    public void GetHintNameFontStroke_ElseBranchReturnsFalse(byte raw)
    {
        // 原文 11731：else Result := False; //HZQ 20230524
        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = raw;
        Assert.False(MShareFunctions.GetHintNameFontStroke(true));
        Assert.False(MShareFunctions.GetHintNameFontStroke(false));
    }

    [Fact]
    public void GetHintNameFontStroke_DefaultParameterIsFalse()
    {
        // 原文 `IsStroke:Boolean = False` ⇒ 省略实参时按 False
        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = 0;
        Assert.False(MShareFunctions.GetHintNameFontStroke());
    }

    // ------------------------------------------------------------------------------------
    // GetHintFontSize（原文 11735-11738）
    // ------------------------------------------------------------------------------------

    [Theory]
    [InlineData(0, 0)]
    [InlineData(9, 9)]
    [InlineData(14, 14)]
    public void GetHintFontSize_ReadsOtherFontSize(byte raw, int expected)
    {
        // 原文 11737：Result := g_ClientConfig.btShowHintOtherFontSize;
        MShareGlobals.g_ConfigClient.btShowHintOtherFontSize = raw;
        Assert.Equal(expected, MShareFunctions.GetHintFontSize());
    }

    [Fact]
    public void GetHintFontSize_And_NameFontSize_AreIndependentFields()
    {
        // 两条函数读的是**不同字段**（11710 vs 11737），不能互相顶替
        MShareGlobals.g_ConfigClient.btShowHintNameFontSize = 9;
        MShareGlobals.g_ConfigClient.btShowHintOtherFontSize = 14;
        Assert.Equal(9, MShareFunctions.GetHintNameFontSize());
        Assert.Equal(14, MShareFunctions.GetHintFontSize());
    }

    // ------------------------------------------------------------------------------------
    // GetHintFontStyle（原文 11740-11750）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetHintFontStyle_ThreeCases_MirrorTheNameVariant()
    {
        // 原文 11742-11749（读 btShowHintOtherFontBold）
        MShareGlobals.g_ConfigClient.btShowHintOtherFontBold = 0;
        Assert.Equal(fsItalic, MShareFunctions.GetHintFontStyle(fsItalic));

        MShareGlobals.g_ConfigClient.btShowHintOtherFontBold = 1;
        Assert.Equal(fsNone, MShareFunctions.GetHintFontStyle(fsItalic));

        MShareGlobals.g_ConfigClient.btShowHintOtherFontBold = 2;
        Assert.Equal(fsBold, MShareFunctions.GetHintFontStyle(fsItalic));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(200)]
    public void GetHintFontStyle_NoElseBranch_OutOfRangeYieldsEmptySet(byte raw)
    {
        // 与 name 版同理：无 else ⇒ 默认空集，不是入参
        MShareGlobals.g_ConfigClient.btShowHintOtherFontBold = raw;
        Assert.Equal(fsNone, MShareFunctions.GetHintFontStyle(fsBold));
    }

    // ------------------------------------------------------------------------------------
    // GetHintFontStroke（原文 11752-11760）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetHintFontStroke_ThreeCasesPlusElse()
    {
        // 原文 11754-11759
        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 0;
        Assert.True(MShareFunctions.GetHintFontStroke(true));
        Assert.False(MShareFunctions.GetHintFontStroke(false));

        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 1;
        Assert.False(MShareFunctions.GetHintFontStroke(true));

        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 2;
        Assert.True(MShareFunctions.GetHintFontStroke(false));

        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 7;
        Assert.False(MShareFunctions.GetHintFontStroke(true));   // else
    }

    [Fact]
    public void GetHintFontStroke_DefaultParameterIsFalse()
    {
        // 原文 `IsStroke:Boolean = False`
        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 0;
        Assert.False(MShareFunctions.GetHintFontStroke());
    }

    // ------------------------------------------------------------------------------------
    // 两族字段互不干扰（防止把 name/other 两组字段接混）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void NameAndOtherStroke_ReadDifferentFields()
    {
        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = 2;   // name ⇒ True
        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 1;  // other ⇒ False

        Assert.True(MShareFunctions.GetHintNameFontStroke(false));
        Assert.False(MShareFunctions.GetHintFontStroke(false));
    }

    [Fact]
    public void ResetForTests_ZeroesAllSevenHintFields()
    {
        MShareGlobals.g_ConfigClient.ShowHintFontName = "黑体";
        MShareGlobals.g_ConfigClient.btShowHintNameFontSize = 9;
        MShareGlobals.g_ConfigClient.btShowHintNameFontBold = 2;
        MShareGlobals.g_ConfigClient.btShowHintNameFontStroke = 2;
        MShareGlobals.g_ConfigClient.btShowHintOtherFontSize = 14;
        MShareGlobals.g_ConfigClient.btShowHintOtherFontBold = 2;
        MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke = 2;

        MShareGlobalsReset.ResetForTests();

        Assert.Equal("", MShareGlobals.g_ConfigClient.ShowHintFontName);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.btShowHintNameFontSize);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.btShowHintNameFontBold);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.btShowHintNameFontStroke);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.btShowHintOtherFontSize);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.btShowHintOtherFontBold);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke);

        // 复位后 0 分支 ⇒ 透传 / 透传 / false
        Assert.Equal(fsBold, MShareFunctions.GetHintFontStyle(fsBold));
        Assert.True(MShareFunctions.GetHintFontStroke(true));
        Assert.Equal(0, MShareFunctions.GetHintFontSize());
    }

    // ------------------------------------------------------------------------------------
    // 切片A：TConfigClient 新字段的类型口径（Core 对齐）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void NewConfigFields_UseCoreAlignedTypes()
    {
        // 三个连击字段按 Core 口径（Grobal2.Types5.cs:341-343）
        Assert.IsType<byte>(MShareGlobals.g_ConfigClient.boDisableWarrContinueHit);
        Assert.IsType<uint>(MShareGlobals.g_ConfigClient.nWarrContinueHitMinInterval);
        Assert.IsType<GXX.Core.Protocol.WordArray10>(MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs);

        // hint 字段按 Core 口径（Grobal2.Types5.cs:49-55）
        Assert.IsType<byte>(MShareGlobals.g_ConfigClient.btShowHintNameFontSize);
        Assert.IsType<byte>(MShareGlobals.g_ConfigClient.btShowHintOtherFontStroke);
        Assert.IsType<HintFontNameBuffer>(MShareGlobals.g_ConfigClient.sShowHintFontName);
    }

    [Fact]
    public void ArrDisableWarrContinueHitIDs_HasExactlyTenSlots()
    {
        // 原文 `array[0..9] of Word` ⇒ 托管 `[InlineArray(10)] WordArray10`。
        // 合法下标 0..9 全部可写：
        for (int i = 0; i < 10; i++)
            MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[i] = (ushort)(i + 1);
        Assert.Equal(1, MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[0]);
        Assert.Equal(10, MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[9]);

        // ⚠ 下标 10 的越界是 **编译期** 错误（CS9166「索引超出了内联数组的界限」），
        //   比运行时断言更强 —— 因此这里**不能**也不需要用 Assert.Throws 表达；
        //   本用例通过"0..9 全可写"正向锁死长度，越界由编译器守。
        //   （曾经写成 Assert.Throws<IndexOutOfRangeException> 会因为编译不过而被移除。）
    }
}
