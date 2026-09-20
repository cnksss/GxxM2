// HUtil32.CompareLStr / CompareBackLStr 的原文语义回归测试
//
// 背景（本工程"看起来一样、语义不同"缺陷类，由车道 p4-m2-objnpc 报出）：
//   原文 HUtil32.pas:1981-1995 / :2010-2032 的两个函数都有两条**必须照抄**的语义，
//   旧托管实现两条都错：
//     1) `compn <= 0` 应直接返回 False。
//        旧实现缺这条守卫：compn==0 时 string.CompareOrdinal(...,0)==0 → 错判为"相等"；
//        compn<0 时更会抛 ArgumentOutOfRangeException。
//     2) 原文逐字符 `UpCase(Src[I]) <> UpCase(targ[I])` → **大小写不敏感**；
//        旧实现用 string.CompareOrdinal → **大小写敏感**。
//   这类缺陷不会被编译期发现，故以断言锁死。
//
// 原文字面（供核对）：
//   CompareLStr:     Result := False; if (compn<=0) or (Length(Src)<compn) or (Length(targ)<compn) then Exit;
//                    for I := 1 to compn do if UpCase(Src[I]) <> UpCase(targ[I]) then Result := False
//   CompareBackLStr: 同上三条守卫；slen/tLen 取长度；for I := 0 to compn-1 do
//                    if UpCase(Src[slen-I]) <> UpCase(targ[tLen-I]) then Result := False

using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>锁定 HUtil32 两个比较函数的原文语义（大小写不敏感 + compn&lt;=0 守卫）。</summary>
public class HUtil32CompareTests
{
    // ---------------- CompareLStr ----------------

    /// <summary>原文逐字符 UpCase → 大小写不敏感（旧实现 CompareOrdinal 会判 false，此为回归点）。</summary>
    [Theory]
    [InlineData("Hello", "hello", 5)]
    [InlineData("HELLO", "hello", 5)]
    [InlineData("hELLo", "Hello", 5)]
    [InlineData("AbCdE", "aBcDe", 5)]
    public void CompareLStr_大小写不敏感(string src, string targ, int compn)
        => Assert.True(HUtil32.CompareLStr(src, targ, compn));

    /// <summary>只比较前 compn 个字符，其后内容不参与。</summary>
    [Fact]
    public void CompareLStr_只比较前compn个字符()
    {
        Assert.True(HUtil32.CompareLStr("ABCxyz", "abcXYZ", 3));
        Assert.False(HUtil32.CompareLStr("ABCxyz", "abdXYZ", 3));
    }

    /// <summary>compn == 0 必须 False（旧实现返回 true —— 核心回归点）。</summary>
    [Fact]
    public void CompareLStr_compn为零返回false()
    {
        Assert.False(HUtil32.CompareLStr("ABC", "ABC", 0));
        Assert.False(HUtil32.CompareLStr("", "", 0));
    }

    /// <summary>compn &lt; 0 必须 False 且**不得抛异常**（旧实现抛 ArgumentOutOfRangeException）。</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CompareLStr_compn为负返回false且不抛(int compn)
    {
        var ex = Record.Exception(() => HUtil32.CompareLStr("ABC", "ABC", compn));
        Assert.Null(ex);
        Assert.False(HUtil32.CompareLStr("ABC", "ABC", compn));
    }

    /// <summary>任一侧长度不足 compn → False（原文两条 Length 守卫）。</summary>
    [Fact]
    public void CompareLStr_长度不足返回false()
    {
        Assert.False(HUtil32.CompareLStr("AB", "ABC", 3));
        Assert.False(HUtil32.CompareLStr("ABC", "AB", 3));
        Assert.False(HUtil32.CompareLStr("", "ABC", 1));
    }

    /// <summary>长度恰好等于 compn 的边界。</summary>
    [Fact]
    public void CompareLStr_长度恰好等于compn()
    {
        Assert.True(HUtil32.CompareLStr("ABC", "abc", 3));
        Assert.True(HUtil32.CompareLStr("A", "a", 1));
    }

    /// <summary>非字母字符按原样比较（UpCase 只影响字母）。</summary>
    [Fact]
    public void CompareLStr_非字母字符原样比较()
    {
        Assert.True(HUtil32.CompareLStr("1a-2", "1A-2", 4));
        Assert.False(HUtil32.CompareLStr("1a-2", "1a_2", 4));
    }

    // ---------------- CompareBackLStr ----------------

    /// <summary>从**末尾**向前逐字符比较，且大小写不敏感。</summary>
    [Fact]
    public void CompareBackLStr_从末尾比较且大小写不敏感()
    {
        Assert.True(HUtil32.CompareBackLStr("xxABC", "yyabc", 3));
        Assert.True(HUtil32.CompareBackLStr("ABC", "abc", 3));
        Assert.False(HUtil32.CompareBackLStr("xxABC", "yyABd", 3));
    }

    /// <summary>只比较**末尾** compn 个字符，前缀不同不影响（与 CompareLStr 的关键区别）。</summary>
    [Fact]
    public void CompareBackLStr_只比较末尾compn个字符()
    {
        Assert.True(HUtil32.CompareBackLStr("ZZZend", "qqqEND", 3));
        Assert.False(HUtil32.CompareBackLStr("ZZZend", "qqqENX", 3));
    }

    /// <summary>compn == 0 → False；compn &lt; 0 → False 且不抛（同 CompareLStr）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-7)]
    public void CompareBackLStr_compn非正返回false且不抛(int compn)
    {
        Assert.Null(Record.Exception(() => HUtil32.CompareBackLStr("ABC", "ABC", compn)));
        Assert.False(HUtil32.CompareBackLStr("ABC", "ABC", compn));
    }

    /// <summary>长度不足 compn → False。</summary>
    [Fact]
    public void CompareBackLStr_长度不足返回false()
    {
        Assert.False(HUtil32.CompareBackLStr("AB", "ABC", 3));
        Assert.False(HUtil32.CompareBackLStr("ABC", "AB", 3));
    }

    /// <summary>长度恰好等于 compn 的边界（整串比较）。</summary>
    [Fact]
    public void CompareBackLStr_长度恰好等于compn()
    {
        Assert.True(HUtil32.CompareBackLStr("abc", "ABC", 3));
        Assert.False(HUtil32.CompareBackLStr("abc", "ABD", 3));
    }
}
