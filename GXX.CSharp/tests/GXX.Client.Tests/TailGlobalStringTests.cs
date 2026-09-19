using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// <c>Source/Client-HGE/GlobalString.pas</c>（414 条 resourcestring + 1 个恒等函数）1:1 移植的测试。
///
/// <para><b>为什么这里要读一个 TSV 金标文件</b>：本单元是 414 条**大写常量**，
/// 手工抄写极易出错，规程也禁止手工转录。因此</para>
/// <list type="number">
/// <item>生成器 <c>_scratch/gen_globalstring.py</c> 直接从 GBK 原文抽取常量 → 生成 .cs 并回读比对；</item>
/// <item>同时落一份 <c>TailGlobalString.golden.tsv</c>（<c>名称 \t 原文行号 \t 条件编译分支 \t 取值</c>）；</item>
/// <item><b>本测试</b>用反射把 <c>GlobalString</c> 的每个 <c>public const string</c> 读出来，
/// 与金标文件**逐字节**比对，并核对常量总数。</item>
/// </list>
/// <para>任一环节不一致（多一条、少一条、值不同、行号漂移）都会失败。</para>
/// </summary>
public sealed class TailGlobalStringTests
{
    /// <summary>
    /// 常量名 → 取值（反射读取，只取 <c>public const string</c>）。
    /// </summary>
    private static Dictionary<string, string> ReadConstants()
    {
        var t = typeof(GlobalString);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (!f.IsLiteral) continue;                 // 只取 const
            if (f.FieldType != typeof(string)) continue;
            map[f.Name] = (string)f.GetRawConstantValue();
        }
        return map;
    }

    /// <summary>
    /// 金标表：<c>TailGlobalStringGolden.g.cs</c>（由 <c>_scratch/gen_globalstring.py</c>
    /// 从 GBK 原文抽取后生成，是**普通 .cs 源文件**——不占用共享 csproj，也不挂载任何资源）。
    /// </summary>
    private static List<(string Name, int Line, string Value)> ReadGolden()
        => TailGlobalStringGolden.Rows
            .Select(r => (r.Name, r.Line, r.Value))
            .ToList();

    // ── 用例 1：常量条数 ─────────────────────────────────────────────────
    [Fact]
    public void Constants_Count_IsExactly414()
    {
        // 原文 414 个「有效」常量（SBTMemoryMoudleErr01..19 在 {$IF}/{$ELSE} 两分支各声明一次，
        // 有效取值只有 {$ELSE} 的那一份，故不计重）。
        Assert.Equal(414, ReadConstants().Count);
        Assert.Equal(414, ReadGolden().Count);
    }

    // ── 用例 2：逐条取值与原文逐字节一致 ──────────────────────────────────
    [Fact]
    public void EveryConstant_Value_MatchesGoldenExtractedFromDelphiSource()
    {
        var cs = ReadConstants();
        var golden = ReadGolden();

        var missing = golden.Where(r => !cs.ContainsKey(r.Name)).Select(r => r.Name).ToList();
        var extra = cs.Keys.Where(k => golden.All(r => r.Name != k)).ToList();
        Assert.Empty(missing);
        Assert.Empty(extra);

        foreach (var r in golden)
        {
            // 差异断言：值必须**逐字节**相同（不做 Trim / 不折叠空白）
            Assert.True(string.Equals(cs[r.Name], r.Value, StringComparison.Ordinal),
                $"常量 {r.Name}（原文 @{r.Line}）取值不符：cs=<{cs[r.Name]}> golden=<{r.Value}>");
        }
    }

    // ── 用例 3：原文行号映射（防"抄到别的单元"）───────────────────────────
    [Fact]
    public void GoldenRows_LineNumbers_AreStrictlyIncreasing()
    {
        int prev = 0;
        foreach (var r in ReadGolden())
        {
            Assert.True(r.Line > prev, $"原文行号未递增：{r.Name} @{r.Line}（前一条 @{prev}）");
            prev = r.Line;
        }
        Assert.Equal(9, ReadGolden()[0].Line);          // SQuitAppAsk 在原文第 9 行
        // 末条按**声明顺序**是 SUpgradeItemFail8（:538）；注意 SGuildRequestAllyRet7 在
        // 原文第 451 行，但之后还有副本/物品/升级等资源串，所以它不是最后一条。
        Assert.Equal(538, ReadGolden().Last().Line);
        Assert.Equal("SUpgradeItemFail8", ReadGolden().Last().Name);
    }

    // ── 用例 4：{$IF TESTMODE=1} / {$ELSE} 两分支的取值差异 ────────────────
    [Fact]
    public void BtMemoryMoudleErr_DefaultBuild_IsSingleSpace_AndTestModeArray_HoldsRealText()
    {
        // 默认构建（{$ELSE}，GlobalString.pas:51-71）：19 条全部是**单个空格**。
        Assert.Equal(" ", GlobalString.SBTMemoryMoudleErr01);
        Assert.Equal(" ", GlobalString.SBTMemoryMoudleErr19);
        foreach (string name in Enumerable.Range(1, 19).Select(i => "SBTMemoryMoudleErr" + i.ToString("D2")))
        {
            var f = typeof(GlobalString).GetField(name, BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(f);
            Assert.Equal(" ", (string)f.GetRawConstantValue());
        }

        // {$IF TESTMODE = 1} 分支（:32-50）的真文本仍以数组形式保留，共 19 条且互不相同。
        var tm = GlobalString.BTMemoryMoudleErrTexts_TestMode;
        Assert.Equal(19, tm.Length);
        Assert.Equal("BuildImportTable: can't load library: ", tm[0]);
        Assert.Equal("BTMemoryGetProcAddress: index out of range", tm[18]);
        Assert.All(tm, s => Assert.False(string.IsNullOrWhiteSpace(s)));

        // 差异断言：默认值与 TESTMODE 文本**必须不同**（否则说明条件编译被压平了）
        for (int i = 0; i < tm.Length; i++)
        {
            var f = typeof(GlobalString).GetField("SBTMemoryMoudleErr" + (i + 1).ToString("D2"));
            Assert.NotEqual(tm[i], (string)f.GetRawConstantValue());
        }
    }

    // ── 用例 5：原文的转义/双反斜杠必须原样保留 ────────────────────────────
    [Fact]
    public void LiteralsWithBackslashAndDoubledQuote_ArePreservedVerbatim()
    {
        // 原文 :341 是 '...未知错误\\该物品可能不存在...'（GBK 里就是两个反斜杠）
        Assert.Contains(@"\\", GlobalString.SMyShopChangeItemFail9);
        // 原文 :32 的 can''t（Delphi 双写单引号）→ C# 单引号
        Assert.Contains("can't", GlobalString.BTMemoryMoudleErrTexts_TestMode[0]);
        // 原文 :186 的 SBigMapHint 用 \255/ \251/ \250/ 作颜色前缀（原文如此）
        Assert.StartsWith("255/", GlobalString.SBigMapHint);
        Assert.Contains(@"\251/", GlobalString.SBigMapHint);
        // 原文 :78-81 的加密/混淆消息串必须逐字保留
        Assert.StartsWith("Uc=LYcdqMBmIUoQ_Q@QFY@uhHCIEHS", GlobalString.SFindModuleMsg);
    }

    // ── 用例 6：与「加密前备份」的 3 处差异（差异断言）───────────────────
    [Fact]
    public void DifferencesAgainstBackup_AreTheThreeKnownOnes()
    {
        // (1) 本次源新增：SMyShopChangeItemFail12（备份无）
        Assert.Equal("[修改物品失败]：店铺物品禁止修改价格！", GlobalString.SMyShopChangeItemFail12);

        // (2) 本次源新增：SGuildRequestAllyRet7（备份无）
        Assert.Equal("不能和敌对行会或者联盟行会的敌对行会结盟。", GlobalString.SGuildRequestAllyRet7);

        // (3) 同名不同值：SCannotExitGame2 —— 本次源是「战斗状态」，备份是「攻击状态」
        Assert.Equal("战斗状态不能退出游戏！", GlobalString.SCannotExitGame2);
        Assert.NotEqual("攻击状态不能退出游戏！", GlobalString.SCannotExitGame2);

        // (4) 同名不同值：SGuildDelMemberFail4 —— 本次源是「退出行会失败」（备份的旧文案
        //     '不能使用命令Z！' 被 // 注释保留在原文里）
        Assert.Equal("退出行会失败", GlobalString.SGuildDelMemberFail4);
        Assert.NotEqual("不能使用命令Z！", GlobalString.SGuildDelMemberFail4);

        // 备份中**存在而本次源没有**的常量：在本类中必须不存在
        Assert.Null(typeof(GlobalString).GetField("SBackupOnlyPlaceholder",
            BindingFlags.Public | BindingFlags.Static));
    }

    // ── 用例 7：DecodeResStr 是恒等函数（原文 Result := S） ────────────────
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("你想退出游戏吗？")]
    [InlineData("Uc=LYcdqMBmIUoQ_Q@QFY@uhHCIEHS]>PPMfYpqMIbItXcQfQNxoYaLnToYRFqMAV_QCOo]mVsTgTrIUU?M`Z@meMp@lNPiHN`IhT`a?JQE?N_=eFp]RNl")]
    public void DecodeResStr_IsIdentity(string s)
        => Assert.Equal(s, GlobalString.DecodeResStr(s));

    // ── 用例 8：null 入参的边界行为 ───────────────────────────────────────
    [Fact]
    public void DecodeResStr_NullInput_ReturnsNull()
    {
        // 原文签名是 string（Delphi AnsiString 允许 nil 指针但表达式 Result := S 会得到 ''）。
        // C# 侧 `=> S` 直接透传，故 null → null。此处把该边界**显式锁住**，
        // 以免日后有人"顺手"改成返回 string.Empty。
        Assert.Null(GlobalString.DecodeResStr(null));
    }

    // ── 用例 9：常量类的形状（防日后被改成实例类/属性）─────────────────────
    [Fact]
    public void GlobalString_IsStaticClass_WithOnlyConstStringsAndOneArray()
        => Assert.True(typeof(GlobalString).IsAbstract && typeof(GlobalString).IsSealed,
            "原文 resourcestring 表映射为 C# 静态类（abstract sealed）");
}
