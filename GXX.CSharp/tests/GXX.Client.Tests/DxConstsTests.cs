using System;
using System.Linq;
using System.Reflection;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-dxcomponent）：DXConsts.pas 1:1 移植的成员齐全性测试。
///
/// 依据 DXConsts.pas 6-99。原文含 63 条 resourcestring + 15 条 const（3 个 const 段）= 78 个常量；
/// 抽取脚本（按 `^名 = '串';$` 逐行匹配 GBK 原文）产出 78 条，逐条粘进
/// DXConsts.ExtractionBaseline，本测试再按名字反射回 const 成员做双向比对 —— 任一边漏项即失败。
/// </summary>
public sealed class DxConstsTests
{
    private static readonly FieldInfo[] StringFields = typeof(DXConsts)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.FieldType == typeof(string))
        .ToArray();

    /// <summary>用例 1：常量总数（原文 63 resourcestring + 15 const = 78）。</summary>
    [Fact]
    public void MemberCount_MatchesSource()
    {
        Assert.Equal(78, DXConsts.MemberCount);
    }

    /// <summary>用例 2：反射到的字符串常量个数 = MemberCount（一条不多一条不少）。</summary>
    [Fact]
    public void ReflectionSeesEveryMember()
    {
        Assert.Equal(DXConsts.MemberCount, StringFields.Length);
        Assert.Equal(DXConsts.MemberCount, StringFields.Select(f => f.Name).Distinct().Count());
    }

    /// <summary>用例 3：每个成员名都能在原文里找到（用抽取基线的名字集合做白名单校验）。</summary>
    [Fact]
    public void EveryMemberName_AppearsInExtractionBaseline()
    {
        var baselineNames = DXConsts.ExtractionBaseline
            .Select(l => l.Substring(0, l.IndexOf('|')))
            .ToHashSet(StringComparer.Ordinal);

        var missing = StringFields
            .Select(f => f.Name)
            .Where(n => !baselineNames.Contains(n))
            .ToList();

        Assert.True(missing.Count == 0, "以下常量的名字未出现在原文抽取基线里：" + string.Join(", ", missing));
    }

    /// <summary>
    /// 用例 4（回读比对）：ExtractionBaseline（脚本从原文抽取）逐条与同名 const 比对 ——
    /// 覆盖 78 条脚本可见常量；缺一条即失败。
    /// </summary>
    [Fact]
    public void ExtractionBaseline_MatchesEveryExtractedConstant()
    {
        Assert.Equal(78, DXConsts.ExtractionBaseline.Length);

        foreach (var line in DXConsts.ExtractionBaseline)
        {
            int bar = line.IndexOf('|');
            Assert.True(bar > 0, $"bad baseline line: {line}");
            string name = line.Substring(0, bar);
            string value = line.Substring(bar + 1);

            var field = typeof(DXConsts).GetField(name, BindingFlags.Public | BindingFlags.Static);
            Assert.True(field != null, $"DXConsts 缺少原文常量 {name}");
            Assert.Equal(value, (string)field.GetRawConstantValue());
        }
    }

    /// <summary>
    /// 用例 5（差异断言）：Delphi 的双写单引号 '' 要还原成单个撇号 ——
    /// 若把原文 `Image ''%s'' not found` 照字面抄成两个撇号，就与运行期实际串不符。
    /// </summary>
    [Fact]
    public void DelphiEscapedQuotes_AreUnescapedExactlyOnce()
    {
        Assert.Equal("Image '%s' not found", DXConsts.SImageNotFound);
        Assert.Equal("Wave '%s' not found", DXConsts.SWaveNotFound);
        Assert.Equal("Effect '%s' not found", DXConsts.SEffectNotFound);
        Assert.Equal("Provider '%s' not found", DXConsts.SDXPlayProviderNotFound);
        Assert.Equal("Session '%s' not found", DXConsts.SDXPlaySessionNotFound);
        Assert.Equal("Can't Load this Graphic", DXConsts.SCannotLoadGraphic);

        // 未转义的那条（原文 62 行漏了引号）保持 %s 裸写，不能"顺手修正"
        Assert.Equal("Session %s cannot be opened", DXConsts.SDXPlaySessionCannotOpened);
    }

    /// <summary>
    /// 用例 6（差异断言）：原文的拼写错误必须原样保留 —— 一旦"顺手修正"，
    /// 与原文（以及任何按原串比对的代码）就对不上了。
    /// </summary>
    [Fact]
    public void OriginalTypos_ArePreserved()
    {
        Assert.Equal("Stream not opend", DXConsts.SStreamNotOpend);       // opened
        Assert.Equal("This format graphic not suported", DXConsts.SNotSupportGraphicFile); // supported
        Assert.Equal("Not posible Overlay Surface", DXConsts.SOverlay);  // possible
        Assert.Equal("Bitcount in invalid (%d)", DXConsts.SInvalidDIBBitCount);  // is invalid
        Assert.Equal("PixelFormat in invalid", DXConsts.SInvalidDIBPixelFormat); // is invalid

        // 差异断言：SStreamNotOpend 与 SStreamOpend 只差前缀，"修正"拼写会破坏这对名字
        Assert.NotEqual(DXConsts.SStreamNotOpend, DXConsts.SStreamOpend);
        Assert.Equal("Stream has already been opened", DXConsts.SStreamOpend);
    }

    /// <summary>用例 7：三个 const 段（78-99）全部到位。</summary>
    [Fact]
    public void ConstSections_AreComplete()
    {
        Assert.Equal("(%dx%d)", DXConsts.SDIBSize);
        Assert.Equal("%d color", DXConsts.SDIBColor);
        Assert.Equal("%d bytes", DXConsts.SDIBBitSize);
        Assert.Equal("%d Kbytes", DXConsts.SDIBBitSize_K);

        Assert.Equal("%.4g sec", DXConsts.SWaveLength);
        Assert.Equal("%dHz", DXConsts.SWaveFrequency);
        Assert.Equal("%dbit", DXConsts.SWaveBitCount);
        Assert.Equal("Mono", DXConsts.SWaveMono);
        Assert.Equal("Stereo", DXConsts.SWaveStereo);
        Assert.Equal("%d bytes", DXConsts.SWaveSize);

        Assert.Equal("Left", DXConsts.SKeyLeft);
        Assert.Equal("Up", DXConsts.SKeyUp);
        Assert.Equal("Right", DXConsts.SKeyRight);
        Assert.Equal("Down", DXConsts.SKeyDown);

        Assert.Equal("%s Effect Editor", DXConsts.SFFBEffectEditor);
    }

    /// <summary>
    /// 用例 8（差异断言）：SDIBBitSize 与 SWaveSize 原文都是 "%d bytes"，两者是**同值不同名**的两条常量；
    /// 若合并去重就会丢掉一个成员（MemberCount 也就对不上了）。
    /// </summary>
    [Fact]
    public void SameValuedConstants_StayDistinct()
    {
        Assert.Equal(DXConsts.SDIBBitSize, DXConsts.SWaveSize);
        Assert.NotEqual(nameof(DXConsts.SDIBBitSize), nameof(DXConsts.SWaveSize));
        // 88 个字段名两两不同 —— 同值常量没有被去重合并
        Assert.Equal(DXConsts.MemberCount, StringFields.Select(f => f.Name).Distinct().Count());
    }

    /// <summary>用例 9：格式化占位符个数与原文一致（%d/%s 不能被吞）。</summary>
    [Theory]
    [InlineData(nameof(DXConsts.SUnknownError), 1)]
    [InlineData(nameof(DXConsts.SDisplayModeChange), 3)]
    [InlineData(nameof(DXConsts.SDIBSize), 2)]
    [InlineData(nameof(DXConsts.SNone), 0)]
    [InlineData(nameof(DXConsts.SNotSupportGraphicFile), 0)]
    public void FormatPlaceholderCounts_MatchSource(string fieldName, int expected)
    {
        var value = (string)typeof(DXConsts).GetField(fieldName).GetRawConstantValue();
        int count = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] != '%') continue;
            if (i + 1 < value.Length && value[i + 1] == '%') { i++; continue; }
            count++;
        }
        Assert.Equal(expected, count);
    }
}
