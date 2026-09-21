using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// 守卫（防回归，车道 <c>p15-cp936-sweep</c>）：
/// GBK 的注册只发生在 <c>GXX.Core.EncodingInit</c> 的 <c>[ModuleInitializer]</c> 里 ——
/// **只有 GXX.Core 模块被触碰时才会跑**。因此任何**直接**写 <c>Encoding.GetEncoding(936)</c>
/// 的调用点都潜藏 <c>No data is available for encoding 936</c>：只要调用方先于任何 GXX.Core
/// 类型被触碰，<c>CodePagesEncodingProvider</c> 就还没注册。
/// <para>
/// 本用例扫描 <c>GXX.CSharp/src</c> 下全部 <c>*.cs</c>（排除 <c>bin</c>/<c>obj</c>），
/// 断言除 <c>EncodingInit.cs</c> 自身外**零**命中；命中即把「文件:行号: 行内容」全部列出。
/// </para>
/// <para>
/// 判据 = 「违规清单为空」**且**「全仓命中总数恰为 1（即 EncodingInit.cs 里那唯一合法的一处）」——
/// 后者是反向断言，防止 EncodingInit 被改名/删除后本守卫退化成恒真空转。
/// </para>
/// </summary>
public class Cp936DirectLookupGuardTests
{
    /// <summary>直取 CP936 的字面形态（容忍空白变体：<c>GetEncoding(936)</c> / <c>GetEncoding( 936 )</c>）。</summary>
    private const string DirectLookupPattern = @"GetEncoding\s*\(\s*936\s*\)";

    /// <summary>唯一允许出现直取的文件（定义本身：<c>EncodingInit.GBK</c> 的真身）。</summary>
    private const string AllowedFileName = "EncodingInit.cs";

    /// <summary>从测试输出目录逐级上溯，找到含 `GXX.CSharp\src` 的那一级（不写死盘符、不写死绝对路径）。</summary>
    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "GXX.CSharp", "src")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException(
            $"找不到仓库根（含 GXX.CSharp\\src 的那一级；从 {AppContext.BaseDirectory} 上溯）。");
    }

    private static bool IsBuildArtifact(string fullPath) =>
        fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(seg => seg == "bin" || seg == "obj");

    private static string RepoRelative(string srcRoot, string fullPath) =>
        "GXX.CSharp/src/" + Path.GetRelativePath(srcRoot, fullPath).Replace('\\', '/');

    [Fact]
    public void NoDirectCp936LookupOutsideEncodingInit()
    {
        string srcRoot = Path.Combine(FindRepoRoot(), "GXX.CSharp", "src");
        var files = Directory.EnumerateFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
                             .Where(f => !IsBuildArtifact(f))
                             .OrderBy(f => f, StringComparer.Ordinal)
                             .ToList();

        // 非空扫描面取证：若路径定位失效导致 0 个文件，本用例会"恒真通过"，必须当场失败。
        Assert.True(files.Count > 100,
            $"扫描面异常：{srcRoot} 下只找到 {files.Count} 个 *.cs（路径定位可能失效）。");

        var rx = new Regex(DirectLookupPattern, RegexOptions.Compiled);
        var violations = new List<string>();
        int total = 0;
        foreach (var file in files)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (!rx.IsMatch(lines[i])) continue;
                total++;
                if (Path.GetFileName(file) == AllowedFileName) continue;
                violations.Add($"{RepoRelative(srcRoot, file)}:{i + 1}: {lines[i].Trim()}");
            }
        }

        Assert.True(violations.Count == 0,
            "GXX.CSharp/src 下除 EncodingInit.cs 外不得再直取 CP936（加载顺序依赖雷）：\n"
            + string.Join("\n", violations));

        // 反向断言：唯一合法的一处必须仍在（否则本守卫因"定义被删/改名"而空转通过）。
        Assert.Equal(1, total);
    }

    /// <summary>
    /// 语义前置：<c>EncodingInit.GBK</c> 必须真的返回 CP936（本守卫所要求的替代物不能是别的东西）。
    /// </summary>
    [Fact]
    public void EncodingInitGbkIsCp936()
    {
        var gbk = GXX.Core.EncodingInit.GBK;
        Assert.Equal(936, gbk.CodePage);
    }
}
