using System;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>UnitExpCalc.pas（数学表达式解析计算）测试 —— 向量来自原单元头部文档。</summary>
public class ExpCalcTests
{
    [Theory]
    [InlineData("1+2", 3.0)]
    [InlineData("2*3+4", 10.0)]
    [InlineData("(1+2)*4", 12.0)]
    [InlineData("10/4", 2.5)]
    [InlineData("-2+5", 3.0)]
    [InlineData("+3-8", -5.0)]
    [InlineData("2^10", 1024.0)]
    [InlineData("2^3^2", 512.0)]              // 幂右结合
    [InlineData("50%", 0.5)]                  // 百分数
    [InlineData("200*10%", 20.0)]
    [InlineData("5!", 120.0)]                 // 阶乘
    [InlineData("15!", 1307674368000.0)]
    [InlineData("1.5+2.5", 4.0)]
    public void CalcExp_BasicArithmetic(string exp, double expected)
    {
        Assert.True(ExpCalc.CalcExp(exp, out double res, out string info), info);
        Assert.Equal(expected, res, 9);
    }

    [Fact]
    public void CalcExp_DocumentedExample()
    {
        // CalcExp('1+max(0.5,sin(1))+sum(1,2^3,mod(5,3))')
        // = 1 + max(0.5, sin(1)) + (1 + 8 + 2) = 12.8414709848...
        Assert.True(ExpCalc.CalcExp("1+max(0.5,sin(1))+sum(1,2^3,mod(5,3))", out double res, out string info));
        Assert.Equal(1 + Math.Max(0.5, Math.Sin(1)) + 11, res, 9);
    }

    [Fact]
    public void CalcExp_DocumentedPercentExample()
    {
        // sin(1)+(-2+(3-4))*20% = sin(1) - 0.6
        Assert.True(ExpCalc.CalcExp("sin(1)+(-2+(3-4))*20%", out double res, out _));
        Assert.Equal(Math.Sin(1) - 0.6, res, 9);
    }

    [Fact]
    public void CalcExp_Constants()
    {
        Assert.True(ExpCalc.CalcExp("PI", out double r1, out _));
        Assert.Equal(Math.PI, r1, 12);
        Assert.True(ExpCalc.CalcExp("e", out double r2, out _));
        Assert.Equal(Math.E, r2, 12);
        Assert.True(ExpCalc.CalcExp("pi*2", out double r3, out _));
        Assert.Equal(Math.PI * 2, r3, 12);
    }

    [Fact]
    public void CalcExp_SignParams_StringForm()
    {
        // AddSignParam('a=1,s=0.5') 后：1+a+sin(s) = 1+1+sin(0.5)
        Assert.Equal(2, ExpCalc.AddSignParam("a=1,s=0.5"));
        Assert.True(ExpCalc.CalcExp("1+a+sin(s)", out double res, out string info));
        Assert.Equal(2 + Math.Sin(0.5), res, 9);
        ExpCalc.InitSignParam();
    }

    [Fact]
    public void CalcExp_SignParams_ArrayForm()
    {
        Assert.Equal(2, ExpCalc.AddSignParam(new[] { "x1", "y2" }, new[] { 3.0, 4.0 }));
        Assert.True(ExpCalc.CalcExp("x1*y2", out double res, out _));
        Assert.Equal(12.0, res, 9);
        // GetSignParam 查询
        Assert.Equal("x1=3", ExpCalc.GetSignParam("x1"));
        Assert.Equal("y2=4", ExpCalc.GetSignParam(1));
        // DelSignParam
        Assert.True(ExpCalc.DelSignParam("x1"));
        Assert.False(ExpCalc.CalcExp("x1*2", out _, out _)); // 未定义符号 → 失败
        ExpCalc.InitSignParam();
    }

    [Theory]
    [InlineData("max(3,7,2)", 7.0)]
    [InlineData("min(3,7,2)", 2.0)]
    [InlineData("sum(1,2,3)", 6.0)]
    [InlineData("avg(2,4,6)", 4.0)]
    [InlineData("stddev(2,4,6)", 1.6329931618554521)] // 样本标准差
    [InlineData("sqrt(16)", 4.0)]
    [InlineData("power(2,8)", 256.0)]
    [InlineData("abs(-9)", 9.0)]
    [InlineData("exp(0)", 1.0)]
    [InlineData("log2(8)", 3.0)]
    [InlineData("log10(1000)", 3.0)]
    [InlineData("logN(8,2)", 3.0)]
    [InlineData("ln(e)", 1.0)]
    [InlineData("int(3.99)", 3.0)]
    [InlineData("trunc(-3.99)", -3.0)]
    [InlineData("frac(3.75)", 0.75)]
    [InlineData("round(3.5)", 4.0)]
    [InlineData("round(3.4)", 3.0)]
    [InlineData("mod(5,3)", 2.0)]
    [InlineData("degrad(180)", 3.14159265358979)]
    [InlineData("raddeg(3.14159265358979)", 180.0)]
    public void CalcExp_Functions(string exp, double expected)
    {
        Assert.True(ExpCalc.CalcExp(exp, out double res, out string info), info);
        Assert.Equal(expected, res, 9);
    }

    [Fact]
    public void CalcExp_Geometry()
    {
        // s_rect(3,4)=12；s_circ(1)=pi；s_tria(3,4,5)=6（海伦）
        Assert.True(ExpCalc.CalcExp("s_rect(3,4)", out double r1, out _));
        Assert.Equal(12.0, r1, 9);
        Assert.True(ExpCalc.CalcExp("s_circ(1)", out double r2, out _));
        Assert.Equal(Math.PI, r2, 9);
        Assert.True(ExpCalc.CalcExp("s_tria(3,4,5)", out double r3, out _));
        Assert.Equal(6.0, r3, 9);
        // pdisplanes(0,0,3,4)=5
        Assert.True(ExpCalc.CalcExp("pdisplanes(0,0,3,4)", out double r4, out _));
        Assert.Equal(5.0, r4, 9);
        // sn(1,2,4) = 1+3+5+7 = 16 等差前 n 项和
        Assert.True(ExpCalc.CalcExp("sn(1,2,4)", out double r5, out _));
        Assert.Equal(16.0, r5, 9);
        // sqn(1,2,4) = 1+2+4+8 = 15 等比前 n 项和
        Assert.True(ExpCalc.CalcExp("sqn(1,2,4)", out double r6, out _));
        Assert.Equal(15.0, r6, 9);
    }

    [Fact]
    public void CheckCalcExp_Errors()
    {
        Assert.False(ExpCalc.CalcExp("1+", out _, out string info1));
        Assert.False(ExpCalc.CalcExp("max(2", out _, out string info2));
        Assert.False(ExpCalc.CalcExp("unknownfn(1)", out _, out string info3));
        Assert.False(ExpCalc.CalcExp("1/0", out _, out _));
        // 未定义符号
        Assert.False(ExpCalc.CalcExp("zz+1", out _, out _));
        // 括号不匹配
        Assert.False(ExpCalc.CalcExp("(1+2))", out _, out _));
        // CheckCalcExp 只查语法不求值
        Assert.True(ExpCalc.CheckCalcExp("1+2*3", out _));
        Assert.False(ExpCalc.CheckCalcExp("1++", out _));
    }

    [Fact]
    public void CalcExp_ComplexNested()
    {
        // 组合嵌套：((1+2)*(3+4))^2 = 21^2 = 441
        Assert.True(ExpCalc.CalcExp("((1+2)*(3+4))^2", out double r1, out _));
        Assert.Equal(441.0, r1, 9);
        // max 内含表达式与函数
        Assert.True(ExpCalc.CalcExp("max(1+1, sqrt(16), round(2.6))", out double r2, out _));
        Assert.Equal(4.0, r2, 9);
    }
}
