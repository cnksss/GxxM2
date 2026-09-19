using GXX.Core.Rtl;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// <c>Source/Common/HUtil32.pas</c> 的布尔字符串化族 —— 原文有**四套互不相同**的语义
/// （禁止"统一"，也禁止把 SysUtils 的 -1/0 倒灌进来）：
///
/// | 函数 | 原文行号 | True / False |
/// |---|---|---|
/// | <c>BoolToStr</c> | 2884-2890 | <c>'TRUE'</c> / <c>'FALSE'</c>（**大写**） |
/// | <c>BoolToStr2</c> | 2892-2898 | <c>'1'</c> / <c>'0'</c> |
/// | <c>BooleanToStr</c> | 2900-2906 | <c>'是'</c> / <c>'否'</c> |
/// | <c>BoolToIntStr</c> | 503-506 | <c>'1'</c> / <c>'0'</c>（<c>Integer(True)=1</c>，**不是** -1） |
/// | <c>BoolToCStr</c> | 508-516 | <c>'是'</c> / <c>'否'</c>（与 BooleanToStr 重复，原文即如此） |
/// | <c>BoolToInt</c> | 495-501 | <c>1</c> / <c>0</c> |
/// | <c>StrToBool</c> | 449-452 | <c>Str_ToInt(Str,0) &lt;&gt; 0</c> |
///
/// 另有非 HUtil32 的两套：SysUtils.BoolToStr（<c>'-1'/'0'</c>，由
/// <see cref="CoreRtlBooleanStrTests"/> 锁定）与 TCustomIniFile.WriteBool（<c>'1'/'0'</c>，
/// 由 <see cref="BoolStrFastIniFileTests"/> 锁定）。
/// </summary>
public class BoolStrSemanticsTests
{
    // ============================ BoolToStr（原文 'TRUE'/'FALSE'） ============================

    [Fact]
    public void HUtil32_BoolToStrDelphi_原文复刻_大写TRUE_FALSE()
    {
        Assert.Equal("TRUE", HUtil32.BoolToStrDelphi(true));
        Assert.Equal("FALSE", HUtil32.BoolToStrDelphi(false));
    }

    [Fact]
    public void HUtil32_BoolToStrDelphi_与SysUtils的不同_差异断言()
    {
        // SysUtils 默认重载是 '-1'/'0'；HUtil32 原文是 'TRUE'/'FALSE'
        Assert.NotEqual(DelphiRTL.BoolToStr(true), HUtil32.BoolToStrDelphi(true));
        Assert.NotEqual("-1", HUtil32.BoolToStrDelphi(true));
        Assert.NotEqual("0", HUtil32.BoolToStrDelphi(false));
        // 与 UseBoolStrs=True 的形态只差大小写（这也是旧实现唯一的偏差）
        Assert.Equal(DelphiRTL.BoolToStr(true, true), HUtil32.BoolToStrDelphi(true), ignoreCase: true);
        Assert.NotEqual(DelphiRTL.BoolToStr(true, true), HUtil32.BoolToStrDelphi(true));
    }

    [Fact]
    public void HUtil32_BoolToStr_历史实现保留_大小写偏差已登记()
    {
        // 旧实现返回混合大小写，与原文 HUtil32.pas:2884-2890（'TRUE'/'FALSE'）不符。
        // 由于 tests/GXX.Core.Tests/CoreTests.cs:448 与 tests/GXX.GameCenter.Tests/GShareDeclTests.cs:256
        // 依赖该文本、而这两个文件不在本车道白名单内，本车道只新增忠实成员 BoolToStrDelphi，
        // 不静默改语义 —— 这两行是本车道对该历史行为的显式登记（集成期修正后请一并删除）。
        Assert.Equal("True", HUtil32.BoolToStr(true));
        Assert.Equal("False", HUtil32.BoolToStr(false));
        Assert.NotEqual(HUtil32.BoolToStrDelphi(true), HUtil32.BoolToStr(true));
    }

    // ============================ 1/0 家族 ============================

    [Fact]
    public void HUtil32_BoolToStr2_原文复刻_1和0()
    {
        Assert.Equal("1", HUtil32.BoolToStr2(true));
        Assert.Equal("0", HUtil32.BoolToStr2(false));
    }

    [Fact]
    public void HUtil32_BoolToIntStr_原文复刻_1和0_不是负一()
    {
        // 原文 503-506：IntToStr(Integer(boBoolean))，Delphi 里 Integer(True) = 1
        Assert.Equal("1", HUtil32.BoolToIntStr(true));
        Assert.Equal("0", HUtil32.BoolToIntStr(false));
        // 差异断言：与 SysUtils 的 -1/0 不同；与 BoolToStr2 相同（原文即两套同形）
        Assert.NotEqual(DelphiRTL.BoolToStr(true), HUtil32.BoolToIntStr(true));
        Assert.Equal(HUtil32.BoolToStr2(true), HUtil32.BoolToIntStr(true));
    }

    [Fact]
    public void HUtil32_BoolToInt_原文复刻_1和0()
    {
        Assert.Equal(1, HUtil32.BoolToInt(true));
        Assert.Equal(0, HUtil32.BoolToInt(false));
    }

    // ============================ 是/否 家族 ============================

    [Fact]
    public void HUtil32_BooleanToStr_原文复刻_是否()
    {
        // 原文 2900-2906；旧实现返回 "True"/"False"（与原文完全不符），本车道已按原文修正
        Assert.Equal("是", HUtil32.BooleanToStr(true));
        Assert.Equal("否", HUtil32.BooleanToStr(false));
        Assert.NotEqual("True", HUtil32.BooleanToStr(true));
        Assert.NotEqual("False", HUtil32.BooleanToStr(false));
    }

    [Fact]
    public void HUtil32_BoolToCStr_原文复刻_是否_并与BooleanToStr同形()
    {
        Assert.Equal("是", HUtil32.BoolToCStr(true));
        Assert.Equal("否", HUtil32.BoolToCStr(false));
        // 原文里这两个函数体逐字相同（508-516 vs 2900-2906），不是"重复实现事故"
        Assert.Equal(HUtil32.BooleanToStr(true), HUtil32.BoolToCStr(true));
        Assert.Equal(HUtil32.BooleanToStr(false), HUtil32.BoolToCStr(false));
    }

    // ============================ StrToBool（整数非 0 即真） ============================

    [Fact]
    public void HUtil32_StrToBool_按整数非零判定()
    {
        Assert.True(HUtil32.StrToBool("1"));
        Assert.True(HUtil32.StrToBool("-1"));
        Assert.True(HUtil32.StrToBool("2"));
        Assert.False(HUtil32.StrToBool("0"));
        Assert.False(HUtil32.StrToBool(""));
        Assert.False(HUtil32.StrToBool("abc"));
    }

    [Fact]
    public void HUtil32_StrToBool_与BoolToStrDelphi不互逆_原文如此()
    {
        // 原文 StrToBool = Boolean(Str_ToInt(Str, 0))，首字符不是数字/± 时直接回落 0 → False。
        // 因此 'TRUE' 读回 False、'-1' 读回 True —— 这一对函数在原文里本来就不互逆，勿"修好"。
        Assert.False(HUtil32.StrToBool(HUtil32.BoolToStrDelphi(true)));
        Assert.True(HUtil32.StrToBool(DelphiRTL.BoolToStr(true)));   // '-1' → 非 0 → True
        Assert.False(HUtil32.StrToBool(DelphiRTL.BoolToStr(false))); // '0'  → 0   → False
    }

    // ============================ 对照表 ============================

    [Fact]
    public void 布尔字符串化_各家族真值形态对照表()
    {
        // True 侧共 4 种不同文本 + 若干整数；False 侧 4 种不同文本。
        Assert.Equal("-1", DelphiRTL.BoolToStr(true));               // SysUtils.BoolToStr 默认重载
        Assert.Equal("True", DelphiRTL.BoolToStr(true, true));       // SysUtils.BoolToStr(useBoolStrs: true)
        Assert.Equal("TRUE", HUtil32.BoolToStrDelphi(true));         // HUtil32.pas:2884
        Assert.Equal("1", HUtil32.BoolToStr2(true));                 // HUtil32.pas:2892
        Assert.Equal("1", HUtil32.BoolToIntStr(true));               // HUtil32.pas:503
        Assert.Equal("是", HUtil32.BooleanToStr(true));              // HUtil32.pas:2900
        Assert.Equal("是", HUtil32.BoolToCStr(true));                // HUtil32.pas:508
        Assert.Equal(1, HUtil32.BoolToInt(true));                    // HUtil32.pas:495
        Assert.Equal("1", DelphiRTL.Format("%d", 1));                // 整数实参不受影响
        Assert.Equal("-1", DelphiRTL.Format("%d", true));            // Boolean 实参 → -1

        Assert.Equal("0", DelphiRTL.BoolToStr(false));
        Assert.Equal("False", DelphiRTL.BoolToStr(false, true));
        Assert.Equal("FALSE", HUtil32.BoolToStrDelphi(false));
        Assert.Equal("0", HUtil32.BoolToStr2(false));
        Assert.Equal("0", HUtil32.BoolToIntStr(false));
        Assert.Equal("否", HUtil32.BooleanToStr(false));
        Assert.Equal("否", HUtil32.BoolToCStr(false));
        Assert.Equal(0, HUtil32.BoolToInt(false));
        Assert.Equal("0", DelphiRTL.Format("%d", false));
    }
}
