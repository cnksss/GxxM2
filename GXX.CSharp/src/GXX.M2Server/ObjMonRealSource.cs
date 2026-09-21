using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GXX.M2Server;

/// <summary>
/// **`ObjMon.pas` 原文行访问器**（车道 `p13-m2-objmon-real` 新增）。
///
/// <para>
/// **为什么需要它**：本单元原有的 46 个 `ObjMon*Core.cs` 里，约 1,917 条谓词是裸
/// <c>=&gt; true;</c>（台账 §48.1「沉默桩」）。那些谓词陈述的是**关于原文的事实**
/// （"1456 行重复了同一个表达式"、"1455 行复位了延迟"…），
/// 但**没有任何代码去核对原文** —— 于是它们编译通过、测试通过、却证明不了任何事。
/// </para>
/// <para>
/// 修复口径（台账 §49.3 三选一）：把这类谓词从"恒真断言"改成
/// **真的去读原文、真的去数、真的去比对**的实现体。
/// 本类提供"按行号取原文行"的最小设施，`ObjMon*Core.cs` 的谓词据此计算，
/// 测试（`ObjMonReal*Tests.cs`）则把**同一个原文**喂进来并断言谓词为真 ——
/// 于是"谓词为真"等价于"原文确实如此"。
/// </para>
/// <para>
/// **原文路径**：优先 <c>_analysis/utf8_mirror/M2Engine/ObjMon.pas</c>（UTF-8 镜像，
/// 工程口径指定的对照体）；该镜像**不入版本库**，故回退到已入库的
/// <c>Source/M2Engine/ObjMon.pas</c>。两者只有编码差异（GBK ↔ UTF-8），
/// 本文件关心的判据全部落在 ASCII 字节上，故回退不影响任何断言的成立性。
/// </para>
/// <para>
/// **失败必须响**：取不到原文时**抛异常**，绝不静默返回默认值 ——
/// 静默正是本车道要消灭的缺陷形态。
/// </para>
/// </summary>
public static class ObjMonRealSource
{
    /// <summary>本单元在原文里的物理行数（脚本实测：`ObjMon.pas` 共 9,502 行）。</summary>
    public const int UnitLineCount = 9502;

    private static readonly object Gate = new();
    private static string[]? _cachedLines;
    private static string? _resolvedPath;

    /// <summary>实际解析到的原文路径（未解析时为 <c>null</c>）。仅供诊断输出使用。</summary>
    public static string? ResolvedPath
    {
        get { lock (Gate) { return _resolvedPath; } }
    }

    /// <summary>原文全部物理行（1-based 通过 <see cref="Line"/> 访问）。</summary>
    public static string[] Lines
    {
        get
        {
            lock (Gate)
            {
                if (_cachedLines != null) return _cachedLines;

                string? path = LocateUnitFile();
                if (path == null)
                {
                    throw new FileNotFoundException(
                        "找不到 ObjMon.pas 原文：既没有 _analysis/utf8_mirror/M2Engine/ObjMon.pas，" +
                        "也没有 Source/M2Engine/ObjMon.pas。" +
                        "本类的一切断言都必须以原文为据，故此处必须失败而不是返回默认值。");
                }

                _cachedLines = File.ReadAllLines(path, new UTF8Encoding(false));
                _resolvedPath = path;

                if (_cachedLines.Length != UnitLineCount)
                {
                    throw new InvalidDataException(
                        $"ObjMon.pas 行数不符：实测 {_cachedLines.Length}，期望 {UnitLineCount}。" +
                        $"原文若被改动，本单元全部行号引用都会失效，故此处必须失败。path={path}");
                }

                return _cachedLines;
            }
        }
    }

    /// <summary>取第 <paramref name="line"/> 行（**1-based**，与原文行号一致）。</summary>
    /// <exception cref="ArgumentOutOfRangeException">行号不在 1..9502 内。</exception>
    public static string Line(int line)
    {
        string[] lines = Lines;
        if (line < 1 || line > lines.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(line), line, $"ObjMon.pas 只有 {lines.Length} 行。");
        }

        return lines[line - 1];
    }

    /// <summary>把整份原文喂给调用方（测试用：一次性读盘、避免每条谓词各读一次）。</summary>
    public static string[] LoadAllLines()
    {
        string[] lines = Lines;
        var copy = new string[lines.Length];
        Array.Copy(lines, copy, lines.Length);
        return copy;
    }

    /// <summary>
    /// 统计 <paramref name="needle"/> 在 <c>[fromLine, toLine]</c>（含两端、1-based）里
    /// 出现的**次数**（按子串出现次数计，非按行计）—— 本车道"否定性断言必须计数取证"
    /// （台账 §37.3）的公共实现。
    /// </summary>
    public static int CountInRange(string needle, int fromLine, int toLine)
    {
        if (string.IsNullOrEmpty(needle)) return 0;
        int n = 0;
        foreach (string line in Range(fromLine, toLine))
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

        return n;
    }

    /// <summary>枚举 <c>[fromLine, toLine]</c>（含两端、1-based）的原文行。</summary>
    public static IEnumerable<string> Range(int fromLine, int toLine)
    {
        string[] lines = Lines;
        for (int i = fromLine; i <= toLine; i++)
        {
            if (i < 1 || i > lines.Length) break;
            yield return lines[i - 1];
        }
    }

    /// <summary>某行是否包含 <paramref name="needle"/>（区分大小写，与 Pascal 一致）。</summary>
    public static bool LineContains(int line, string needle)
        => Line(line).Contains(needle, StringComparison.Ordinal);

    /// <summary>
    /// 在原文里找 <c>procedure &lt;Class&gt;.&lt;Method&gt;</c> / <c>function ...</c>
    /// 的**实现行**（列 0、即不在类声明体内）；找不到时抛异常。
    /// </summary>
    public static int FindImplementationLine(string typeName, string methodName)
    {
        string[] lines = Lines;
        for (int i = 0; i < lines.Length; i++)
        {
            string t = lines[i].TrimStart();
            if (t.Length == 0 || char.IsWhiteSpace(lines[i][0])) continue;   // 类体内声明有缩进
            if (!(t.StartsWith("procedure ", StringComparison.Ordinal)
                  || t.StartsWith("function ", StringComparison.Ordinal)
                  || t.StartsWith("constructor ", StringComparison.Ordinal)
                  || t.StartsWith("destructor ", StringComparison.Ordinal)))
            {
                continue;
            }

            if (t.Contains(typeName + "." + methodName, StringComparison.Ordinal))
                return i + 1;
        }

        throw new InvalidOperationException(
            $"ObjMon.pas 里找不到实现行：{typeName}.{methodName}");
    }

    /// <summary>
    /// 定位原文文件。
    /// <para>
    /// ★ **两级回退必须分两轮**：先在所有候选根里找 **UTF-8 镜像**，
    /// 全都找不到才退而求其次用某一份 <c>Source/M2Engine/ObjMon.pas</c>。
    /// 本车道第一版把两者写在同一个循环里（"这个根没有镜像就用这个根的 Source"），
    /// 于是在 **git worktree** 里会误取 worktree 自己的 `Source`（GBK）而放弃真仓库的镜像 ——
    /// 被 `ProductSideSourceMatchesTestSideSource` 抓到（两侧逐行比对不一致）。
    /// </para>
    /// </summary>
    private static string? LocateUnitFile()
    {
        const string mirrorRelative = @"_analysis\utf8_mirror\M2Engine\ObjMon.pas";
        const string sourceRelative = @"Source\M2Engine\ObjMon.pas";

        foreach (string root in CandidateRepoRoots())
        {
            string mirror = Path.Combine(root, mirrorRelative);
            if (File.Exists(mirror)) return mirror;
        }

        foreach (string root in CandidateRepoRoots())
        {
            string src = Path.Combine(root, sourceRelative);
            if (File.Exists(src)) return src;
        }

        return null;
    }

    /// <summary>
    /// 候选仓库根：从**当前工作目录**与**测试程序集所在目录**两个起点各自向上找，
    /// 命中"同时含 <c>Source</c> 与 <c>GXX.CSharp</c> 两个目录"的那一层。
    /// </summary>
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
                if (seen.Add(candidate) && LooksLikeRepoRoot(candidate))
                    yield return candidate;
            }
        }
    }

    private static bool LooksLikeRepoRoot(string dir)
        => Directory.Exists(Path.Combine(dir, "Source"))
           && Directory.Exists(Path.Combine(dir, "GXX.CSharp"));
}
