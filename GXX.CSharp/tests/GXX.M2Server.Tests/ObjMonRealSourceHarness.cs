using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// **`ObjMon.pas` 原文取证设施**（车道 `p13-m2-objmon-real` 新增）。
///
/// <para>
/// 本车道的口径（台账 §48.1/§49.3）：`ObjMon*Core.cs` 里那些**关于原文的断言**
/// 必须真的读原文再判定，不许裸 `=&gt; true;`。
/// 本类给测试侧提供"独立地、不经被测代码"地读原文的能力 ——
/// 于是"谓词为真"与"原文确实如此"成为两件互相印证的事，
/// 而不是同一条恒真式自证。
/// </para>
/// <para>
/// 原文路径口径与产品侧 `ObjMonRealSource` 一致：UTF-8 镜像优先
/// （`_analysis/utf8_mirror/M2Engine/ObjMon.pas`，不入版本库），
/// 回退到已入库的 `Source/M2Engine/ObjMon.pas`（GBK）。
/// **两个来源都必须按 UTF-8 之外的编码正确处理**：GBK 副本用代码页 936 读，
/// 因为本工程禁止对含中文文件做 shell 文本往返（台账 §41.6）。
/// </para>
/// <para>
/// 所有正文判据都落在 ASCII 字节上（Pascal 标识符、运算符、数字），
/// 两种来源在这些字节上完全一致 —— 本类因此不受编码差异影响。
/// </para>
/// </summary>
internal static class ObjMonRealSourceHarness
{
    /// <summary>脚本实测：`ObjMon.pas` 共 9,502 个物理行。</summary>
    internal const int UnitLineCount = 9502;

    private static readonly Lazy<(string Path, string[] Lines)> Loaded = new(LoadCore);

    /// <summary>一个只用于诊断的 C# 文件路径（排查产品侧/测试侧读到的原文是否同一份）。</summary>
    internal static string Diagnostic => "harnessLines=" + Lines.Length
        + " productLines=" + GXX.M2Server.ObjMonRealSource.Lines.Length
        + " harnessPath=" + SourcePath
        + " productPath=" + GXX.M2Server.ObjMonRealSource.ResolvedPath;

    /// <summary>实际解析到的原文路径（报告与失败信息里要写出来）。</summary>
    internal static string SourcePath => Loaded.Value.Path;

    /// <summary>原文全部物理行（1-based 访问用 <see cref="Line"/>）。</summary>
    internal static string[] Lines => Loaded.Value.Lines;

    /// <summary>取原文第 <paramref name="line"/> 行（1-based）。</summary>
    internal static string Line(int line)
    {
        string[] lines = Lines;
        if (line < 1 || line > lines.Length)
            throw new ArgumentOutOfRangeException(nameof(line), line, "ObjMon.pas 行号越界");
        return lines[line - 1];
    }

    /// <summary>`[fromLine, toLine]`（含两端、1-based）内 <paramref name="needle"/> 的出现次数。</summary>
    internal static int Count(string needle, int fromLine, int toLine)
    {
        int n = 0;
        for (int i = fromLine; i <= toLine && i <= Lines.Length; i++)
        {
            if (i < 1) continue;
            string line = Lines[i - 1];
            int idx = 0;
            while (true)
            {
                idx = line.IndexOf(needle, idx, StringComparison.Ordinal);
                if (idx < 0) break;
                n++;
                idx += needle.Length;
            }
        }

        return n;
    }

    /// <summary>
    /// 本单元的**否定性断言取证**（台账 §37.3）：断言某个记号在**已入库的全部原文副本**
    /// （镜像 + `Source`）里出现 <paramref name="expected"/> 次，并返回实测值。
    /// </summary>
    internal static IReadOnlyList<(string Path, int Count)> CountAcrossAllCopies(
        string needle, int expected)
    {
        var results = new List<(string, int)>();
        foreach ((string path, string[] lines) in AllCopies())
        {
            int n = 0;
            foreach (string line in lines)
            {
                int idx = 0;
                while (true)
                {
                    idx = line.IndexOf(needle, idx, StringComparison.Ordinal);
                    if (idx < 0) break;
                    n++;
                    idx += needle.Length;
                }
            }

            results.Add((path, n));
        }

        Assert.NotEmpty(results);
        foreach ((string path, int n) in results)
            Assert.Equal(expected, n);

        return results;
    }

    /// <summary>已入库的原文副本路径（相对仓库根，用 '/' 分隔）。</summary>
    internal const string TrackedRelativePath = "Source/M2Engine/ObjMon.pas";

    private static IEnumerable<(string Path, string[] Lines)> AllCopies()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // ① 镜像（不入库；存在则用）
        string? mirror = TryResolveMirror();
        if (mirror != null && seen.Add(mirror))
            yield return (mirror, File.ReadAllLines(mirror, new UTF8Encoding(false)));

        // ② 已入库副本（GBK → 代码页 936）
        string tracked = Path.Combine(RepoRoot(), "Source", "M2Engine", "ObjMon.pas");
        if (File.Exists(tracked) && seen.Add(tracked))
            yield return (tracked, ReadWithCodePage(tracked, 936));
    }

    private static (string Path, string[] Lines) LoadCore()
    {
        // ★ 两级回退分两轮：先找镜像，找不到才退入库副本。
        //   同一个循环里回退会误取 git worktree 自己的 Source（GBK），见 ObjMonRealSource.LocateUnitFile 的注释。
        string? mirror = TryResolveMirror();
        if (mirror != null)
        {
            string[] lines = File.ReadAllLines(mirror, new UTF8Encoding(false));
            if (lines.Length == UnitLineCount) return (mirror, lines);
        }

        foreach (string root in CandidateRepoRoots())
        {
            string tracked = Path.Combine(root, "Source", "M2Engine", "ObjMon.pas");
            if (!File.Exists(tracked)) continue;

            string[] lines = ReadWithCodePage(tracked, 936);
            if (lines.Length == UnitLineCount) return (tracked, lines);
        }

        throw new FileNotFoundException(
            "找不到可用的 ObjMon.pas 原文（镜像与 Source/M2Engine/ObjMon.pas 都不可用，或行数不为 "
            + UnitLineCount + "）。本类的断言全部以原文为据，取不到原文必须失败。");
    }

    private static string? TryResolveMirror()
    {
        foreach (string root in CandidateRepoRoots())
        {
            string path = Path.Combine(root, "_analysis", "utf8_mirror", "M2Engine", "ObjMon.pas");
            if (File.Exists(path)) return path;
        }

        return null;
    }

    private static IEnumerable<string> CandidateRepoRoots()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            if (string.IsNullOrEmpty(start)) continue;

            var dir = new DirectoryInfo(start);
            for (int depth = 0; dir != null && depth < 12; depth++, dir = dir.Parent)
            {
                string candidate = dir.FullName;
                if (seen.Add(candidate)
                    && Directory.Exists(Path.Combine(candidate, "Source"))
                    && Directory.Exists(Path.Combine(candidate, "GXX.CSharp")))
                {
                    yield return candidate;
                }
            }
        }
    }

    /// <summary>
    /// 用指定代码页读文本。C# 侧用 <see cref="Encoding.GetEncoding(int)"/> 读取 GBK 原文，
    /// **没有**任何 shell 文本往返（台账 §41.6）。
    /// </summary>
    private static string[] ReadWithCodePage(string path, int codePage)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string[] lines = File.ReadAllLines(path, Encoding.GetEncoding(codePage));
        return lines;
    }

    private static string RepoRoot()
    {
        foreach (string root in CandidateRepoRoots()) return root;

        throw new DirectoryNotFoundException(
            "从当前目录与测试程序集目录都找不到仓库根（同时含 Source 与 GXX.CSharp 的目录）。");
    }
}
