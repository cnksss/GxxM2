// ParadoxConv.pas 的 Delphi `Format` 语义回归测试（集成方修复的回填测试）
//
// 背景（本工程第 1 类高发缺陷："看起来一样、语义不同"）：
//   ParadoxConv 的 EConvException.Create 用 Delphi `Format` 拼消息，格式串是
//       "Can't convert from %S to %S symbol %S"      （原文 ParadoxConv.pas:71-80）
//   托管侧一度写成 `string.Format(fmt.Replace("%S", "{0}"), args)`。
//   因为该串有**三个** `%S`，全部替换成 `{0}` 后，三个位置都取第一个实参，
//   消息实际输出 source 三次、dest/sym 被静默丢弃（且 .NET 的 string.Format 根本不认 %S）。
//
//   这类缺陷**不会被编译期发现、也不会被普通断言发现**（它是"输出文本错"），
//   正是 DIB 车道发现的 `%d` 同类问题的另一例。故补此测试锁定 Delphi 按序替换语义。
//
// 修复：改为调用 `GXX.Core.Rtl.DelphiFormat.Format`（GXX.Core/Rtl/DelphiRTL.cs:193），
//       它按序消费实参，且 %S 与 %s 等价（char.ToLowerInvariant 分派）。

using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>锁定 `EConvException.Create` 的 Delphi Format 按序替换语义（原文 ParadoxConv.pas:71-80）。</summary>
public class ParadoxConvFormatTests
{
    /// <summary>
    /// 核心回归：三个占位符必须分别取 source / dest / sym，而不是全取第一个。
    /// 修复前该断言会得到 "Can't convert from A to A symbol A"。
    /// </summary>
    [Fact]
    public void Create_三个占位符各自取对应实参()
    {
        var ex = EConvException.Create("CP936", "UTF8", "0x81");

        Assert.Equal("Can't convert from CP936 to UTF8 symbol 0x81", ex.Message);
    }

    /// <summary>三个实参互不相同（防止"恰好相等"使上面的断言假通过）。</summary>
    [Fact]
    public void Create_实参互不相同时消息中三者均出现()
    {
        var ex = EConvException.Create("SRC", "DST", "SYM");

        Assert.Contains("SRC", ex.Message);
        Assert.Contains("DST", ex.Message);
        Assert.Contains("SYM", ex.Message);
        // dest 与 sym 不得被第一个实参顶替
        Assert.DoesNotContain("to SRC", ex.Message);
        Assert.DoesNotContain("symbol SRC", ex.Message);
    }

    /// <summary>
    /// 原文取大写 `%S`；Delphi 中 %S 与 %s 等价，托管实现按 char.ToLowerInvariant 分派。
    /// 这条同时钉住"大写占位符也要被识别"（旧的 Replace("%S",…) 只认大写，反而更脆弱）。
    /// </summary>
    [Fact]
    public void Create_属性按原文赋值()
    {
        var ex = EConvException.Create("CP936", "UTF8", "0x81");

        Assert.Equal("CP936", ex.SrcEncoding);
        Assert.Equal("UTF8", ex.DstEncoding);
        Assert.Equal("0x81", ex.Symbol);
    }

    /// <summary>空串参数（边界）：占位符仍应替换，不应残留 %S 字面量。</summary>
    [Fact]
    public void Create_空串实参不残留占位符()
    {
        var ex = EConvException.Create("", "", "");

        Assert.DoesNotContain("%S", ex.Message);
        Assert.Equal("Can't convert from  to  symbol ", ex.Message);
    }

    /// <summary>含 % 的实参不应被当作格式串再次解析（实参不参与格式解析）。</summary>
    [Fact]
    public void Create_实参含百分号不被二次解析()
    {
        var ex = EConvException.Create("a%b", "c%d", "e%S");

        Assert.Equal("Can't convert from a%b to c%d symbol e%S", ex.Message);
    }
}
