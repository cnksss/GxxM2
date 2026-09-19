using GXX.Core.Rtl;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// 核心收口：Delphi 布尔字符串化 —— **SysUtils 那一套是 <c>-1</c>/<c>0</c>**。
///
/// 本仓库里"布尔转字符串"一共有四套互不相同的语义（原文如此，禁止统一），本文件锁的是
/// SysUtils 的两处（<c>Format('%d',[Boolean])</c> 与 <c>SysUtils.BoolToStr</c>）；
/// HUtil32 单元的四套（'TRUE'/'FALSE'、'1'/'0'、'是'/'否'、1/0）与 TCustomIniFile.WriteBool
/// 的 '1'/'0' 由 <c>BoolStr*.cs</c> 锁定。
///
/// 原文依据（本仓库不含 RTL 源码，Source/ 下无 SysUtils.pas，故无行号可引）：
///   * GameCenter/GMain.pas:5-11 的 uses 里**没有** HUtil32 → GMain.pas:1991-1993 的
///     <c>IniGameConf.WriteString('Server','EnableMakingID', BoolToStr(...))</c> 解析到 SysUtils 版；
///   * M2Engine/Forms/GameConfig.pas:5-7 同样没有 HUtil32 → :2146-2147 的 BoolToStr 亦为 SysUtils 版；
///   * 项目内三处已按 -1/0 落地的桩：GXX.GameCenter/GShareTypes.cs:145、
///     GXX.M2Server/Forms/GeneralConfigForm.cs:475、GXX.SelGate/SelGateConfig.cs:363。
/// </summary>
public class CoreRtlBooleanStrTests
{
    // ============================ 1) Format('%d', [Boolean]) ============================

    [Fact]
    public void Format_d_WithBoolean_RendersMinusOneAndZero()
    {
        Assert.Equal("-1", DelphiRTL.Format("%d", true));
        Assert.Equal("0", DelphiRTL.Format("%d", false));
    }

    [Fact]
    public void Format_d_WithBoolean_DiffersFromOneZeroAndFromTextForms()
    {
        string t = DelphiRTL.Format("%d", true);
        string f = DelphiRTL.Format("%d", false);

        // 差异断言：True 不是 '1'（C# 的 Convert.ToInt64(Boolean) 会给出 1，旧实现即如此）
        Assert.NotEqual(Convert.ToInt64(true).ToString(), t);
        // 差异断言：也不是 'True'（SysUtils.BoolToStr(UseBoolStrs=True) 的形态）
        Assert.NotEqual("True", t);
        // 差异断言：与同为"Delphi 布尔字符串化"的 HUtil32.BoolToStr2（'1'/'0'）也不同
        Assert.NotEqual(GXX.Core.Util.HUtil32.BoolToStr2(true), t);

        // 原文如此：False 的四套语义里有三套都落成同一个字符 '0'
        //（SysUtils '-1'/'0'、BoolToStr2 '1'/'0'、BoolToInt 1/0、以及 C# 的 Convert.ToInt64），
        // 唯一不撞的是文本形态 'False' —— 差异只出现在 True 一侧，此处显式记录以免误判。
        Assert.Equal("0", f);
        Assert.Equal(Convert.ToInt64(false).ToString(), f);
        Assert.Equal(GXX.Core.Util.HUtil32.BoolToStr2(false), f);
        Assert.NotEqual("False", f);
    }

    [Fact]
    public void Format_d_WithBoolean_FollowsWidthAndPrecision()
    {
        // 宽度：与整数路径共用同一套填充/零填充规则（Delphi 对负号不计入精度位数）
        Assert.Equal(" -1", DelphiRTL.Format("%3d", true));
        Assert.Equal("  0", DelphiRTL.Format("%3d", false));
        Assert.Equal("-001", DelphiRTL.Format("%.3d", true));   // 精度 3 → 1 → "001"，再补负号
        Assert.Equal("000", DelphiRTL.Format("%.3d", false));
    }

    [Fact]
    public void Format_d_WithIntegers_Unchanged()
    {
        // 回归守卫：只有 Boolean 实参走 -1/0，整数实参一个字节都不变
        Assert.Equal("7", DelphiRTL.Format("%d", 7));
        Assert.Equal("-7", DelphiRTL.Format("%d", -7));
        Assert.Equal("0", DelphiRTL.Format("%d", 0));
        Assert.Equal("9223372036854775807", DelphiRTL.Format("%d", long.MaxValue));
        Assert.Equal("第 3 关", DelphiRTL.Format("第 %d 关", 3));
    }

    [Fact]
    public void Format_d_WithNullableIntLikeArgs_Unchanged()
    {
        // 回归守卫：byte/short/enum 等仍按整数渲染（它们是"整数"而不是 Boolean）
        Assert.Equal("1", DelphiRTL.Format("%d", (byte)1));
        Assert.Equal("1", DelphiRTL.Format("%d", (short)1));
        Assert.Equal("1", DelphiRTL.Format("%d", 1u));
        Assert.Equal("1", DelphiRTL.Format("%d", 1L));
    }

    // ============================ 2) SysUtils.BoolToStr ============================

    [Fact]
    public void SysUtils_BoolToStr_DefaultOverload_IsMinusOneAndZero()
    {
        Assert.Equal("-1", DelphiRTL.BoolToStr(true));
        Assert.Equal("0", DelphiRTL.BoolToStr(false));
        // 显式传 useBoolStrs: false 与默认重载等价
        Assert.Equal("-1", DelphiRTL.BoolToStr(true, false));
        Assert.Equal("0", DelphiRTL.BoolToStr(false, false));
    }

    [Fact]
    public void SysUtils_BoolToStr_UseBoolStrs_IsText()
    {
        Assert.Equal("True", DelphiRTL.BoolToStr(true, true));
        Assert.Equal("False", DelphiRTL.BoolToStr(false, true));
    }

    [Fact]
    public void SysUtils_BoolToStr_MatchesFormat_d_BooleanBranch()
    {
        // 同一语义的两条通路必须一致：Format 的 %d 布尔分支就是 SysUtils.BoolToStr 默认重载
        Assert.Equal(DelphiRTL.BoolToStr(true), DelphiRTL.Format("%d", true));
        Assert.Equal(DelphiRTL.BoolToStr(false), DelphiRTL.Format("%d", false));
    }

    [Fact]
    public void SysUtils_BoolToStr_DiffersFromEveryOtherFamily()
    {
        // 差异断言（四套语义互不相等）
        Assert.NotEqual("1", DelphiRTL.BoolToStr(true));
        Assert.NotEqual("True", DelphiRTL.BoolToStr(true));
        Assert.NotEqual("TRUE", DelphiRTL.BoolToStr(true));
        Assert.NotEqual("是", DelphiRTL.BoolToStr(true));

        Assert.NotEqual("1", DelphiRTL.BoolToStr(false));
        Assert.NotEqual("False", DelphiRTL.BoolToStr(false));
        Assert.NotEqual("FALSE", DelphiRTL.BoolToStr(false));
        Assert.NotEqual("否", DelphiRTL.BoolToStr(false));
    }
}
