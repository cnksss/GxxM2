using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GXX.RunGate.Rest9;
using Xunit;
using Xunit.Abstractions;

namespace GXX.RunGate.Tests;

/// <summary>
/// `Rest9NotPortedEvidence.cs` 的取证测试。
///
/// 与其它平行车道不同，本车道交付的是**「不移植 + 证据」**（台账 §18.5 第 6 条口径），
/// 因此测试对象不是"新算法"，而是**裁定本身**：
///   * 每个用例都**直接读仓库里的 Delphi 原文 / C# 既有源码**重新抽取、重新计数，
///     再与 `Rest9NotPortedEvidence` 里的数据双向比对 → 证据可复现，不是 C# 自证；
///   * 否定性断言（"C# 侧 0 命中"、"不是任何 .dpr/.dproj 的成员"）一律**先数总数再断言**
///     （台账 §37.3），避免"空集合恒真"。
/// </summary>
public class Rest9NotPortedEvidenceTests
{
    private readonly ITestOutputHelper _out;
    public Rest9NotPortedEvidenceTests(ITestOutputHelper output) => _out = output;

    // ================= 仓库定位 & 原文读取 =================

    /// <summary>从测试输出目录逐级上溯找仓库根（含 `GXX.CSharp` 与 `Source` 的那一级）。</summary>
    internal static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "GXX.CSharp")) &&
                Directory.Exists(Path.Combine(dir.FullName, "Source")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException(
            $"找不到仓库根（从 {AppContext.BaseDirectory} 上溯）。");
    }

    private static string RepoPath(string relative) =>
        Path.Combine(FindRepoRoot(), relative.Replace('/', Path.DirectorySeparatorChar));

    private static string ReadSource(string relative) => File.ReadAllText(RepoPath(relative));

    private static int PhysicalLineCount(string relative) =>
        File.ReadAllLines(RepoPath(relative)).Length;

    private static int CountMatches(string haystack, string pattern) =>
        Regex.Matches(haystack, pattern).Count;

    /// <summary>
    /// 数「整行内容恰为 <paramref name="token"/>」的行数。
    /// 不能用 <c>Regex.Matches(s, "^type\s*$", Multiline)</c>：<c>\s</c> 会跨行吞掉空行，
    /// 使 <c>$</c> 永远无法定位（本车道实测踩到，见并行报告 §4 踩坑）。
    /// </summary>
    private static int CountExactLines(string text, string token) =>
        text.Split('\n').Count(l => l.Trim() == token);

    /// <summary>
    /// 把仓库里所有 `*.cs`（src + tests）拼起来——用于"C# 侧 0 命中"的定量取证。
    /// <paramref name="excludeRest9"/> 为真时排除本车道自己的取证文件
    /// （它们**故意**写下这些名字）。
    /// </summary>
    private static string ReadAllCSharpSources(bool excludeRest9 = false)
    {
        var root = FindRepoRoot();
        var dirs = new[]
        {
            Path.Combine(root, "GXX.CSharp", "src"),
            Path.Combine(root, "GXX.CSharp", "tests"),
        };
        var sb = new System.Text.StringBuilder();
        foreach (var d in dirs.Where(Directory.Exists))
            foreach (var f in Directory.EnumerateFiles(d, "*.cs", SearchOption.AllDirectories))
            {
                var name = Path.GetFileName(f);
                if (excludeRest9 && name.StartsWith("Rest9", StringComparison.Ordinal)) continue;
                sb.AppendLine(File.ReadAllText(f));
            }
        return sb.ToString();
    }

    // ================= 0) 仓库自检（避免"找不到就静默通过"）=================

    [Fact]
    public void RepoRoot_IsDiscoverable_AndAllFourUnitsExist()
    {
        var root = FindRepoRoot();
        Assert.True(Directory.Exists(Path.Combine(root, "Source", "RunGate")));
        foreach (var unit in Rest9NotPortedEvidence.SourceUnits)
            Assert.True(File.Exists(RepoPath(unit)), $"原文缺失：{unit}");
    }

    [Fact]
    public void AllFourUnits_PhysicalLineCounts_MatchMeasuredReality()
    {
        var total = 0;
        foreach (var unit in Rest9NotPortedEvidence.SourceUnits)
        {
            var actual = PhysicalLineCount(unit);
            total += actual;
            Assert.Equal(Rest9NotPortedEvidence.PhysicalLineCounts[unit], actual);
        }
        Assert.Equal(1229, total);   // 586 + 340 + 293 + 10
        _out.WriteLine($"四单元物理 LF 合计 = {total}");
    }

    // ================= 1) IODataPool.pas =================

    private static string IODataPoolSource =>
        Rest9NotPortedEvidence.StripPascalComments(ReadSource("Source/RunGate/Common/IODataPool.pas"));

    [Fact]
    public void IODataPool_MemberCount_Is11_AndTwoOfThemAreInsideBlockComments()
    {
        var raw = ReadSource("Source/RunGate/Common/IODataPool.pas");
        var live = Rest9NotPortedEvidence.StripPascalComments(raw);

        // 原文（含注释内的两个 SendBuffer 成员）共 11 个 TIODataPool 实现体
        var inRaw = Regex.Matches(
            raw, @"^(?:constructor|destructor|procedure|function|class function)\s+TIODataPool",
            RegexOptions.Multiline).Count;
        // 剥掉 { } 注释后只剩 9 个
        var inLive = Regex.Matches(
            live, @"^(?:constructor|destructor|procedure|function|class function)\s+TIODataPool",
            RegexOptions.Multiline).Count;

        _out.WriteLine($"含注释 {inRaw} / 活代码 {inLive}");
        Assert.Equal(Rest9NotPortedEvidence.IODataPoolMembers.Count, inRaw);
        Assert.Equal(11, inRaw);
        Assert.Equal(9, inLive);
        Assert.Equal(Rest9NotPortedEvidence.IODataPoolCommentedOutMembers, inRaw - inLive);

        // 被注释掉的那两个成员必须真的只在注释里出现
        Assert.DoesNotContain("TIODataPool.GetNewSendBufer", live);
        Assert.DoesNotContain("TIODataPool.GiveBackSendBuffer", live);
        Assert.Contains("TIODataPool.GetNewSendBufer", raw);
        Assert.Contains("TIODataPool.GiveBackSendBuffer", raw);
    }

    [Fact]
    public void IODataPool_MemberNamesAndLineNumbers_MatchExtraction()
    {
        var raw = ReadSource("Source/RunGate/Common/IODataPool.pas");
        var lines = File.ReadAllLines(RepoPath("Source/RunGate/Common/IODataPool.pas"));
        var expected = new (string Sig, int Line)[]
        {
            ("constructor TIODataPool.Create", 82),
            ("destructor TIODataPool.Destroy", 174),
            ("procedure TIODataPool.Clear", 200),
            ("function TIODataPool.GetNewIOData", 266),
            ("function TIODataPool.GetCount", 368),
            ("function TIODataPool.GetUseCount", 373),
            ("function TIODataPool.GetNoUseCount", 378),
            ("function TIODataPool.GiveBackIOData", 383),
            ("function TIODataPool.GetNewSendBufer", 489),
            ("function TIODataPool.GiveBackSendBuffer", 536),
            ("class function TIODataPool.Instance", 567),
        };

        Assert.Equal(Rest9NotPortedEvidence.IODataPoolMembers.Count, expected.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Sig, Rest9NotPortedEvidence.IODataPoolMembers[i]);
            Assert.Contains(expected[i].Sig, lines[expected[i].Line - 1]);
        }
        Assert.NotEmpty(raw);
    }

    [Fact]
    public void IODataPool_IsAnUnmanagedPointerPool_NotAValueContainer()
    {
        var raw = ReadSource("Source/RunGate/Common/IODataPool.pas");
        // §37.3：先数总数，再断言阈值——而不是"搜不到就算过"
        var allocSites = Regex.Matches(raw, @"\b(?:GetMem|FreeMem)\s*\(").Count;
        _out.WriteLine($"IODataPool 内 GetMem/FreeMem 站点 = {allocSites}");
        Assert.Equal(Rest9NotPortedEvidence.IODataPoolUnmanagedAllocSites, allocSites);
        Assert.True(allocSites >= 10);

        // 池里装的是 POVERLAPPEDEx（原始指针），不是托管对象
        Assert.Contains(Rest9NotPortedEvidence.IODataPoolPayloadType, raw);
        Assert.Contains("POVERLAPPEDEx", raw);

        // 41 个尺寸档 × 1024 预分配：常数逐字保留在证据里
        Assert.Contains("MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10 shr 7 + 1", raw);
        Assert.Equal(41, (5 << 10 >> 7) + 1);
        Assert.Equal(Rest9NotPortedEvidence.IODataPoolBucketCount, (5 << 10 >> 7) + 1);
        Assert.Contains("MAX_PREALLOCATED_MEMORY_SIZE - 1", raw);
        Assert.Equal(Rest9NotPortedEvidence.IODataPoolPreallocPerBucket, ReadIocpCommonInt("MAX_PREALLOCATED_MEMORY_SIZE"));
    }

    private static int ReadIocpCommonInt(string constName)
    {
        var src = ReadSource("Source/RunGate/Common/IocpCommon.pas");
        var m = Regex.Match(src, constName + @"\s*:\s*Integer\s*=\s*([0-9]+)");
        Assert.True(m.Success, $"IocpCommon.pas 里找不到 {constName}");
        return int.Parse(m.Groups[1].Value);
    }

    [Fact]
    public void IODataPool_AllCallers_AreTheFourUnits_AndEveryOneOfThemExists()
    {
        var root = FindRepoRoot();
        var hits = new List<string>();
        foreach (var f in Directory.EnumerateFiles(
                     Path.Combine(root, "Source", "RunGate"), "*.pas", SearchOption.AllDirectories))
        {
            if (Regex.IsMatch(File.ReadAllText(f), @"\bIODataPool\b"))
                hits.Add(Path.GetRelativePath(root, f).Replace('\\', '/'));
        }
        hits.Sort(StringComparer.Ordinal);

        // §37.3 计数取证：先把"命中总数"打出来
        _out.WriteLine($"IODataPool 引用文件 = {hits.Count}：{string.Join(", ", hits)}");
        Assert.Equal(Rest9NotPortedEvidence.IODataPoolCalledFrom.Count, hits.Count);
        Assert.Equal(
            Rest9NotPortedEvidence.IODataPoolCalledFrom.OrderBy(x => x, StringComparer.Ordinal).ToArray(),
            hits.ToArray());

        // 4 个引用者里除 uFrmMain.pas 外全是 IOCP 管道自身；
        Assert.Equal(6, hits.Count);
        Assert.Contains("Source/RunGate/MirClientContext.pas", hits);
        Assert.Contains("Source/RunGate/uFrmMain.pas", hits);
        Assert.Contains("Source/RunGate/Common/IODataPool.pas", hits);
        Assert.DoesNotContain("Source/RunGate/RunGateUtils.pas", hits);

        // uFrmMain 是唯一"非管道"使用，且只用来显示 6 个统计标签
        var ufrm = File.ReadAllLines(RepoPath("Source/RunGate/uFrmMain.pas"));
        Assert.Equal(5, Rest9NotPortedEvidence.IODataPoolUiStatLabels.Count);
        foreach (var entry in Rest9NotPortedEvidence.IODataPoolUiStatLabels)
        {
            var m = Regex.Match(entry, @"^[^:]+:(?<line>\d+)\s+(?<code>.+)$");
            Assert.True(m.Success, entry);
            var line = int.Parse(m.Groups["line"].Value);
            Assert.Contains(m.Groups["code"].Value.Trim(), ufrm[line - 1]);
        }
    }

    [Fact]
    public void IODataPool_ManagedReplacement_HasNoPoolApi_SoThereIsNothingToReuse()
    {
        // GatewayKit 是被裁定"取代 IOCP 管道"的既有设施
        var gw = ReadSource("GXX.CSharp/src/GXX.GatewayKit/GatewayProtocol.cs");
        var link = ReadSource("GXX.CSharp/src/GXX.GatewayKit/TcpLink.cs");
        var all = gw + link;

        // §37.3：先证明我们真的读到了内容（否则"0 命中"是因为读空文件）
        Assert.True(all.Length > 5000, $"GatewayKit 源码读取异常，长度 = {all.Length}");
        Assert.Contains("class IocpManager", gw);
        Assert.Contains("class GateSession", gw);
        Assert.Contains("class TcpLink", link);

        // 公开 API 清单里的每一项都确实存在（证明清单不是编的）
        foreach (var api in Rest9NotPortedEvidence.ManagedGatewayKitPublicApi)
        {
            var name = api.Split('.')[1];
            Assert.True(all.Contains(name, StringComparison.Ordinal), $"API 清单里的 {api} 在 GatewayKit 里找不到");
        }

        // 而 GatewayKit **没有**任何尺寸档内存池 API ⇒ 没有可复用的等价物，
        // 也正因为如此，本车道不新建"第三份实现"（台账 §14.2）
        foreach (var poolApi in new[]
                 {
                     "GetNewIOData", "GiveBackIOData", "TIODataPool", "AllocMemSize",
                     "MaxAllocMemSize", "MaxUseCount", "NoUseCount", "POVERLAPPEDEx",
                 })
            Assert.DoesNotContain(poolApi, all);
    }

    [Fact]
    public void IODataPool_NoCSharpCounterpartWasCreatedByThisLane()
    {
        // §37.3 计数取证：本车道只允许出现"证据"文件，不允许出现池实现。
        // 排除本车道自己的 Rest9* 取证文件（它们**故意**写下这些名字）。
        var all = ReadAllCSharpSources(excludeRest9: true);
        Assert.True(all.Length > 100_000, $"C# 源码读取异常，长度 = {all.Length}");

        foreach (var forbidden in new[]
                 {
                     "class TIODataPool", "class TIocpTcpClient", "GetNewIOData", "GiveBackIOData",
                     "POVERLAPPEDEx",
                 })
        {
            var hits = CountMatches(all, Regex.Escape(forbidden));
            _out.WriteLine($"C# 侧 \"{forbidden}\" 命中 = {hits}");
            Assert.Equal(0, hits);
        }

        // 唯一一处提到 TIODataPool 的地方是 uFrmMainLogic.cs 的文件头注释
        // （说明主窗体的缺失接缝）——**不计**为实现，故只断言"恰好 1 处且是注释行"。
        var mentions = new List<string>();
        var root = FindRepoRoot();
        foreach (var f in Directory.EnumerateFiles(
                     Path.Combine(root, "GXX.CSharp", "src"), "*.cs", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(f);
            if (!text.Contains("TIODataPool", StringComparison.Ordinal)) continue;
            mentions.Add(Path.GetRelativePath(root, f).Replace('\\', '/'));
            Assert.All(
                File.ReadAllLines(f).Where(l => l.Contains("TIODataPool", StringComparison.Ordinal)),
                l => Assert.StartsWith("///", l.TrimStart()));
        }
        _out.WriteLine($"TIODataPool 在 C# src 的提及 = {mentions.Count}：{string.Join(", ", mentions)}");
        Assert.Single(mentions);
        Assert.Equal("GXX.CSharp/src/GXX.RunGate/uFrmMainLogic.cs", mentions[0]);
    }

    // ================= 2) IocpTcpClient.pas =================

    private static string IocpTcpClientSource =>
        ReadSource("Source/RunGate/Common/IocpTcpClient.pas");

    [Fact]
    public void IocpTcpClient_MemberCount_Is19()
    {
        var raw = IocpTcpClientSource;
        var live = Rest9NotPortedEvidence.StripPascalComments(raw);
        var inRaw = Regex.Matches(
            raw, @"^(?:constructor|destructor|procedure|function)\s+T",
            RegexOptions.Multiline).Count;
        var inLive = Regex.Matches(
            live, @"^(?:constructor|destructor|procedure|function)\s+T",
            RegexOptions.Multiline).Count;

        _out.WriteLine($"含注释 {inRaw} / 活代码 {inLive}");
        Assert.Equal(Rest9NotPortedEvidence.IocpTcpClientMembers.Count, inRaw);
        Assert.Equal(19, inRaw);
        // 原文唯一的"注释内代码"是 :146-153 那段 if（不是方法签名）⇒ 方法数不变
        Assert.Equal(inRaw, inLive);
    }

    [Fact]
    public void IocpTcpClient_MemberNamesAndLineNumbers_MatchExtraction()
    {
        var lines = File.ReadAllLines(RepoPath("Source/RunGate/Common/IocpTcpClient.pas"));
        var expected = new (string Sig, int Line)[]
        {
            ("constructor TIocpRemoteContext.Create", 88),
            ("destructor TIocpRemoteContext.Destroy", 94),
            ("procedure TIocpRemoteContext.CloseContextSocket", 100),
            ("procedure TIocpRemoteContext.DoConnect", 105),
            ("procedure TIocpRemoteContext.DoDisconnect", 111),
            ("procedure TIocpRemoteContext.DoReset", 117),
            ("procedure TIocpRemoteContext.SetActive", 123),
            ("constructor TIocpTcpClient.Create", 241),
            ("destructor TIocpTcpClient.Destroy", 254),
            ("procedure TIocpTcpClient.ClearContexts", 265),
            ("function TIocpTcpClient.Add", 276),
            ("function TIocpTcpClient.GetCount", 291),
            ("function TIocpTcpClient.GetItems", 296),
            ("procedure TIocpTcpClient.SetOnContextConnect", 304),
            ("procedure TIocpTcpClient.SetOnContextDisconnect", 310),
            ("procedure TIocpTcpClient.SetOnError", 316),
            ("procedure TIocpTcpClient.SetOnRecvDataBuffer", 322),
            ("procedure TIocpTcpClient.SetOnSendDataBuffer", 328),
            ("procedure TIocpTcpClient.RegisterContextClass", 334),
        };
        Assert.Equal(Rest9NotPortedEvidence.IocpTcpClientMembers.Count, expected.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Sig, Rest9NotPortedEvidence.IocpTcpClientMembers[i]);
            Assert.Contains(expected[i].Sig, lines[expected[i].Line - 1]);
        }
    }

    [Fact]
    public void IocpTcpClient_AllCallers_AreRunGateUtilsAlone()
    {
        // 声明：TIocpTcpClient/TIocpRemoteContext 只被 RunGateUtils.pas 使用
        var withType = new List<string>();
        foreach (var f in new[]
                 {
                     "Source/RunGate/RunGateUtils.pas",
                     "Source/RunGate/MirClientContext.pas",
                     "Source/RunGate/GateShare.pas",
                     "Source/RunGate/uFrmMain.pas",
                     "Source/RunGate/Common/IocpUtils.pas",
                     "Source/RunGate/Common/IocpTcpServer.pas",
                 })
        {
            var src = ReadSource(f);
            if (Regex.IsMatch(src, @"\bTIocpTcpClient\b") ||
                Regex.IsMatch(src, @"\bTIocpRemoteContext\b"))
                withType.Add(f);
        }
        _out.WriteLine($"TIocpTcpClient/TIocpRemoteContext 引用文件 = {withType.Count}：{string.Join(", ", withType)}");
        Assert.Single(withType);
        Assert.Equal("Source/RunGate/RunGateUtils.pas", withType[0]);

        // 且原文 uses 是**活分支**（UseIocpClient = 1）—— 佐证它不是"编译不进去的死单元"
        var utils = ReadSource("Source/RunGate/RunGateUtils.pas");
        Assert.Contains("{$IF UseIocpClient <> 0} IocpTcpClient, {$IFEND}", utils);
        Assert.Contains("UseIocpClient = 1", ReadSource("Source/RunGate/Common/IocpCommon.pas"));
    }

    [Fact]
    public void IocpTcpClient_EveryRegisteredCallSiteInEvidenceExistsVerbatim()
    {
        var utils = File.ReadAllLines(RepoPath("Source/RunGate/RunGateUtils.pas"));
        foreach (var entry in Rest9NotPortedEvidence.IocpTcpClientCalledFrom)
        {
            // 形如 "Source/RunGate/RunGateUtils.pas:3509  代码"
            var m = Regex.Match(entry, @"^(?<file>[^:]+):(?<line>\d+)\s+(?<code>.+)$");
            Assert.True(m.Success, $"证据条目格式不对：{entry}");
            var line = int.Parse(m.Groups["line"].Value);
            var code = m.Groups["code"].Value.Split("//")[0].Trim();
            if (m.Groups["file"].Value != "Source/RunGate/RunGateUtils.pas") continue;
            Assert.True(line <= utils.Length, $"行号越界：{entry}");
            Assert.Contains(code, utils[line - 1]);
        }
    }

    [Fact]
    public void IocpTcpClient_SetActive_IsTheOnlyPlaceWithTheWholeWin32IocpClientMechanics()
    {
        var raw = IocpTcpClientSource;
        // §37.3 计数取证：这些 Win32 调用在**整个单元**里的出现次数
        var counts = Rest9NotPortedEvidence.SetActiveWin32Mechanics;
        var actual = counts.Keys.ToDictionary(
            s => s, s => CountMatches(raw, Regex.Escape(s)));
        foreach (var kv in actual) _out.WriteLine($"{kv.Key} = {kv.Value}");

        // 逐条比对实测出现次数（不是"至少一次"——那会掩盖重复定义/笔误）
        foreach (var kv in counts)
            Assert.Equal(kv.Value, actual[kv.Key]);

        Assert.Equal(2, actual["WSASocket"]);
        Assert.Equal(2, actual["Bind"]);
        Assert.Equal(1, actual["CreateIoCompletionPort"]);
        Assert.Equal(1, actual["SIO_GET_EXTENSION_FUNCTION_POINTER"]);
        Assert.Equal(1, actual["inet_addr"]);
        Assert.Equal(20, actual.Values.Sum());
        _out.WriteLine($"Win32 机械出现次数合计 = {actual.Values.Sum()}");

        // 这些调用全部落在 SetActive 的方法体内（:123 起、:237 止）
        var body = string.Join('\n',
            File.ReadAllLines(RepoPath("Source/RunGate/Common/IocpTcpClient.pas")).Skip(122).Take(115));
        Assert.Contains("WSASocket", body);
        Assert.Contains("CreateIoCompletionPort", body);
        Assert.Contains("SIO_GET_EXTENSION_FUNCTION_POINTER", body);
    }

    [Fact]
    public void IocpTcpClient_ManagedReplacement_IsTcpLink_AndItIsARealActiveFacility()
    {
        var link = ReadSource("GXX.CSharp/src/GXX.GatewayKit/TcpLink.cs");
        Assert.Contains("class TcpLink", link);
        // TcpLink 覆盖的正是本单元的两件事：主动连接 + 收发
        Assert.Contains("public bool Connect()", link);
        Assert.Contains("public void Send(byte[] data)", link);
        Assert.Contains("OnDisconnected", link);

        // 且 TcpLink 是**活设施**（有生产调用方），不是孤儿——所以不需要第二份
        var root = FindRepoRoot();
        var callers = new List<string>();
        foreach (var f in Directory.EnumerateFiles(
                     Path.Combine(root, "GXX.CSharp", "src"), "*.cs", SearchOption.AllDirectories))
        {
            var p = Path.GetRelativePath(root, f).Replace('\\', '/');
            if (p.EndsWith("GatewayKit/TcpLink.cs", StringComparison.Ordinal)) continue;
            if (Regex.IsMatch(File.ReadAllText(f), @"\bTcpLink\b")) callers.Add(p);
        }
        callers.Sort(StringComparer.Ordinal);
        _out.WriteLine($"TcpLink 生产调用文件 = {callers.Count}：{string.Join(", ", callers)}");
        Assert.True(callers.Count >= 3, $"TcpLink 调用方只有 {callers.Count} 个，请复核裁定");
        Assert.Contains("GXX.CSharp/src/GXX.DBServer/IDSocCli.Adapter.cs", callers);
    }

    // ================= 3) Qos.pas =================

    private static string QosSource => ReadSource("Source/RunGate/Common/Qos.pas");

    [Fact]
    public void Qos_HasZeroExecutableCode_OnlyTypesAndConstants()
    {
        var live = Rest9NotPortedEvidence.StripPascalComments(QosSource);
        // implementation 段必须为空（否则就有可移植算法）
        var impl = live.Substring(live.IndexOf("implementation", StringComparison.Ordinal));
        Assert.DoesNotContain("procedure", impl, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("function", impl, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("class", impl, StringComparison.OrdinalIgnoreCase);

        // 只有 5 个 type 段 + 5 个 const 段（原文 :75/81/145/175/199/215/248/261/278）
        var typeAnchors = CountExactLines(live, "type");
        var constAnchors = CountExactLines(live, "const");
        _out.WriteLine($"Qos.pas 剥注释后 type 段 = {typeAnchors}，const 段 = {constAnchors}");
        Assert.Equal(5, typeAnchors);
        Assert.Equal(5, constAnchors);
    }

    [Fact]
    public void Qos_ExternalsymCount_Is40_AndEqualsConstants27PlusTypes13()
    {
        var raw = QosSource;
        var externalsym = CountMatches(raw, @"\{\$EXTERNALSYM");
        _out.WriteLine("{$EXTERNALSYM} 条数 = " + externalsym);

        // 常量 27 条（QosConstantNames 26 + TC_NONCONF_BORROW_PLUS）
        var constants = Rest9NotPortedEvidence.QosConstantNames.Count + 1;
        Assert.Equal(Rest9NotPortedEvidence.QosConstantCount, constants);
        Assert.Equal(27, constants);

        // 带 {$EXTERNALSYM} 的是「全部 27 个常量」+「13 个类型/记录」，共 40
        const int typesWithExternalsym = 13;
        Assert.Equal(Rest9NotPortedEvidence.QosExternalsymCount, constants + typesWithExternalsym);
        Assert.Equal(40, externalsym);

        // 无 {$EXTERNALSYM} 的 9 个类型别名/指针别名（总数 22 − 13）
        Assert.Equal(22 - typesWithExternalsym, Rest9NotPortedEvidence.QosTypeNames.Count - typesWithExternalsym);
    }

    [Fact]
    public void Qos_ConstantNamesAndValues_MatchExtraction()
    {
        var lines = File.ReadAllLines(RepoPath("Source/RunGate/Common/Qos.pas"));
        var expected = new (string Name, int Line, uint Value)[]
        {
            ("SERVICETYPE_NOTRAFFIC", 82, 0x00000000),
            ("SERVICETYPE_BESTEFFORT", 84, 0x00000001),
            ("SERVICETYPE_CONTROLLEDLOAD", 86, 0x00000002),
            ("SERVICETYPE_GUARANTEED", 88, 0x00000003),
            ("SERVICETYPE_NETWORK_UNAVAILABLE", 90, 0x00000004),
            ("SERVICETYPE_GENERAL_INFORMATION", 92, 0x00000005),
            ("SERVICETYPE_NOCHANGE", 94, 0x00000006),
            ("SERVICETYPE_NONCONFORMING", 96, 0x00000009),
            ("SERVICETYPE_NETWORK_CONTROL", 98, 0x0000000A),
            ("SERVICETYPE_QUALITATIVE", 100, 0x0000000D),
            ("SERVICE_BESTEFFORT", 105, 0x80010000),
            ("SERVICE_CONTROLLEDLOAD", 107, 0x80020000),
            ("SERVICE_GUARANTEED", 109, 0x80040000),
            ("SERVICE_QUALITATIVE", 111, 0x80200000),
            ("SERVICE_NO_TRAFFIC_CONTROL", 125, 0x81000000),
            ("SERVICE_NO_QOS_SIGNALING", 138, 0x40000000),
            ("QOS_NOT_SPECIFIED", 176, 0xFFFFFFFF),
            ("POSITIVE_INFINITY_RATE", 186, 0xFFFFFFFE),
            ("QOS_GENERAL_ID_BASE", 216, 2000),
            ("QOS_OBJECT_END_OF_LIST", 219, 2001),
            ("QOS_OBJECT_SD_MODE", 222, 2002),
            ("QOS_OBJECT_SHAPING_RATE", 225, 2003),
            ("QOS_OBJECT_DESTADDR", 228, 2004),
            ("TC_NONCONF_BORROW", 262, 0),
            ("TC_NONCONF_SHAPE", 264, 1),
            ("TC_NONCONF_DISCARD", 266, 2),
            ("TC_NONCONF_BORROW_PLUS", 268, 3),
        };

        Assert.Equal(Rest9NotPortedEvidence.QosConstantCount, expected.Length);
        for (int i = 0; i < expected.Length - 1; i++)   // 证据数组不含 BORROW_PLUS（另有常量登记）
            Assert.Equal(expected[i].Name, Rest9NotPortedEvidence.QosConstantNames[i]);

        foreach (var e in expected)
        {
            var line = lines[e.Line - 1];
            Assert.StartsWith(e.Name, line.TrimStart());
            if (e.Name.StartsWith("QOS_OBJECT_", StringComparison.Ordinal))
            {
                // QOS_OBJECT_* = $0000000N + QOS_GENERAL_ID_BASE ⇒ 2001..2004
                Assert.Contains("+ QOS_GENERAL_ID_BASE", line);
                var hm = Regex.Match(line, @"\$(?<hex>[0-9A-Fa-f]+)\s*\+");
                Assert.True(hm.Success, $"取不到 QOS_OBJECT_* 的十六进制加数：{line}");
                var addend = Convert.ToUInt32(hm.Groups["hex"].Value, 16);
                Assert.Equal(e.Value - 2000u, addend);
            }
            else
            {
                Assert.Contains(e.Value.ToString("X"), line.ToUpperInvariant());
            }
        }

        // 常量值本身（与原文表达式分离的第二真源）
        Assert.Equal(2001u, 1u + 2000u);   // QOS_OBJECT_END_OF_LIST
        Assert.Equal(2002u, 2u + 2000u);   // QOS_OBJECT_SD_MODE
        Assert.Equal(2003u, 3u + 2000u);   // QOS_OBJECT_SHAPING_RATE
        Assert.Equal(2004u, 4u + 2000u);   // QOS_OBJECT_DESTADDR
        Assert.Equal(2000u, 2000u);        // QOS_GENERAL_ID_BASE
    }

    [Fact]
    public void Qos_TypeNames_MatchExtraction_22Declarations()
    {
        var lines = File.ReadAllLines(RepoPath("Source/RunGate/Common/Qos.pas"));
        var expected = new (string Name, int Line)[]
        {
            ("SERVICETYPE", 76), ("TServiceType", 78), ("PServiceType", 79),
            ("_flowspec", 146), ("FLOWSPEC", 157), ("PFLOWSPEC", 159), ("LPFLOWSPEC", 161),
            ("TFlowSpec", 163),
            ("QOS_OBJECT_HDR", 200), ("LPQOS_OBJECT_HDR", 205), ("TQOSObjectHdr", 207),
            ("PQOSObjectHdr", 208),
            ("_QOS_SD_MODE", 249), ("QOS_SD_MODE", 254), ("LPQOS_SD_MODE", 256),
            ("TQOSSDMode", 258), ("PQOSSDMode", 259),
            ("_QOS_SHAPING_RATE", 279), ("QOS_SHAPING_RATE", 284), ("LPQOS_SHAPING_RATE", 286),
            ("TQOSShapingRate", 288), ("PQOSShapingRate", 289),
        };
        Assert.Equal(Rest9NotPortedEvidence.QosTypeNames.Count, expected.Length);
        Assert.Equal(22, expected.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Name, Rest9NotPortedEvidence.QosTypeNames[i]);
            Assert.Contains(expected[i].Name, lines[expected[i].Line - 1]);
        }
    }

    [Fact]
    public void Qos_IsReferencedByOneUnitOnly_AndThatUnitIsItselfNotPorted()
    {
        var root = FindRepoRoot();
        var hits = new List<string>();
        foreach (var f in Directory.EnumerateFiles(
                     Path.Combine(root, "Source"), "*.pas", SearchOption.AllDirectories))
        {
            var p = Path.GetRelativePath(root, f).Replace('\\', '/');
            if (p.EndsWith("Common/Qos.pas", StringComparison.Ordinal)) continue;
            if (Regex.IsMatch(File.ReadAllText(f), @"^unit\s+Qos\s*;", RegexOptions.Multiline)) continue;
            if (File.ReadAllText(f).Contains("Source/RunGate/Common/Qos.pas", StringComparison.Ordinal)) continue;
            hits.Add(p);
        }
        // 只数 uses 子句里的 "Qos"
        var usesHits = new List<string>();
        foreach (var f in hits)
        {
            var lines = File.ReadAllLines(Path.Combine(root, f.Replace('/', Path.DirectorySeparatorChar)));
            if (lines.Any(l => Regex.IsMatch(l, @"^\s*[^/]*\bQos\s*(,|;|$)")))
                usesHits.Add(f);
        }
        usesHits.Sort(StringComparer.Ordinal);
        _out.WriteLine($"uses Qos 的文件 = {usesHits.Count}：{string.Join(", ", usesHits)}");
        Assert.Single(usesHits);
        Assert.Equal("Source/RunGate/Common/IocpWinsock2.pas", usesHits[0]);
    }

    [Fact]
    public void Qos_OnlyFlowSpecAndServiceTypeReachIocpWinsock2_AndNothingElse()
    {
        var src = ReadSource("Source/RunGate/Common/IocpWinsock2.pas");
        Assert.True(src.Length > 50_000, $"IocpWinsock2.pas 读取异常，长度 = {src.Length}");

        // ★ 计数取证：引用方**真正**动用的 Qos 符号（逐条把实测值打出来）
        foreach (var kv in Rest9NotPortedEvidence.QosSymbolUseCountsInIocpWinsock2)
        {
            var n = CountMatches(src, Regex.Escape(kv.Key));
            _out.WriteLine($"IocpWinsock2.pas 内 \"{kv.Key}\" = {n}（期望 {kv.Value}）");
            Assert.Equal(kv.Value, n);
        }
        Assert.Equal(
            Rest9NotPortedEvidence.QosSymbolsActuallyUsedByIocpWinsock2.OrderBy(x => x, StringComparer.Ordinal),
            Rest9NotPortedEvidence.QosSymbolUseCountsInIocpWinsock2.Keys.OrderBy(x => x, StringComparer.Ordinal));

        // FLOWSPEC 只出现在 _QualityOfService 记录的 Sending/ReceivingFlowspec 两个字段上
        Assert.Contains("SendingFlowspec: FLOWSPEC;", src);
        Assert.Contains("ReceivingFlowspec: FLOWSPEC;", src);
        Assert.Contains("QOS = _QualityOfService;", src);

        // 其余整族（QOS 对象族 / 整形族 / 别名族）在该单元里 0 命中
        foreach (var sym in Rest9NotPortedEvidence.QosSymbolsAbsentFromIocpWinsock2)
        {
            var n = CountMatches(src, Regex.Escape(sym));
            _out.WriteLine($"IocpWinsock2.pas 内 \"{sym}\" = {n}");
            Assert.Equal(0, n);
        }
    }

    [Fact]
    public void Qos_AlreadyPortedPart_InClientLane_IsExactlyTheSevenServicetypeConstants()
    {
        var csharp = ReadSource(Rest9NotPortedEvidence.QosAlreadyPortedInCSharpWhere);
        var declarations = Regex.Matches(csharp, @"public const (?:int|uint) (SERVICETYPE_[A-Z_]+)")
            .Select(m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        _out.WriteLine($"WinSock2Constants.cs 里 SERVICETYPE_* = {declarations.Length}：{string.Join(", ", declarations)}");
        Assert.Equal(
            new[]
            {
                "SERVICETYPE_BESTEFFORT", "SERVICETYPE_CONTROLLEDLOAD", "SERVICETYPE_GENERAL_INFORMATION",
                "SERVICETYPE_GUARANTEED", "SERVICETYPE_NETWORK_UNAVAILABLE", "SERVICETYPE_NOCHANGE",
                "SERVICETYPE_NOTRAFFIC",
            },
            declarations);

        Assert.Equal(Rest9NotPortedEvidence.QosAlreadyPortedInCSharp.Count, declarations.Length);
    }

    [Fact]
    public void Qos_ThirtyTwoSymbols_HaveLiteralZeroCSharpHits()
    {
        // §37.3：先证明 C# 语料非空，再断言"0 命中"。排除本车道自己的取证文件
        // （它们**故意**写下这些名字——初版忘了排除，被这条用例当场否掉）。
        var all = ReadAllCSharpSources(excludeRest9: true);
        Assert.True(all.Length > 100_000, $"C# 语料读取异常，长度 = {all.Length}");

        var already = new HashSet<string>(
            Rest9NotPortedEvidence.QosAlreadyPortedInCSharp, StringComparer.Ordinal);

        var zeroHit = new List<string>();
        foreach (var sym in Rest9NotPortedEvidence.QosConstantNames
                     .Concat(Rest9NotPortedEvidence.QosTypeNames)
                     .Concat(new[] { Rest9NotPortedEvidence.QosTCNonconfBorrowPlus })
                     .Distinct(StringComparer.Ordinal))
        {
            if (already.Contains(sym)) continue;
            var n = CountMatches(all, @"\b" + Regex.Escape(sym) + @"\b");
            if (n == 0) zeroHit.Add(sym);
            else _out.WriteLine($"非零命中：{sym} = {n}");
        }

        _out.WriteLine($"Qos 独有且 C# 0 命中的符号 = {zeroHit.Count} / 40");
        Assert.Equal(Rest9NotPortedEvidence.QosSymbolsWithZeroCSharpHits, zeroHit.Count);
        // 40 个 EXTERNALSYM 中 8 条已在 Client 车道落地 ⇒ 32 条确实无落点
        Assert.Equal(40 - 8, zeroHit.Count);
    }

    [Fact]
    public void Qos_Differential_CSharpHasNoQosRecord_ButClientConstantsKeepTheSameNames()
    {
        // 排除本车道自己的取证文件（它们**故意**写下这些名字）
        var all = ReadAllCSharpSources(excludeRest9: true);
        Assert.True(all.Length > 100_000, $"C# 语料读取异常，长度 = {all.Length}");

        // Qos.pas 的**记录/结构体**一律没有落点（SERVICETYPE_* 常量是另一回事）
        foreach (var rec in new[]
                 {
                     "QOS_OBJECT_HDR", "QOS_SD_MODE", "QOS_SHAPING_RATE", "TQOSObjectHdr",
                     "TQOSSDMode", "TQOSShapingRate", "TFlowSpec", "PFLOWSPEC", "LPFLOWSPEC",
                     "TC_NONCONF_BORROW", "TC_NONCONF_SHAPE", "TC_NONCONF_DISCARD",
                 })
        {
            var n = CountMatches(all, @"\b" + Regex.Escape(rec) + @"\b");
            _out.WriteLine($"C# 侧 \"{rec}\" = {n}");
            Assert.Equal(0, n);
        }

        // `FLOWSPEC` 在 C# 里唯一的出现是 WSA_QOS_EFLOWSPEC/EPSFLOWSPEC 这两个**别的**常量（子串命中）
        foreach (var hit in Regex.Matches(all, @"[A-Za-z_]*FLOWSPEC[A-Za-z_]*")
                     .Select(x => x.Value).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal))
            _out.WriteLine($"C# 侧 FLOWSPEC 子串命中：{hit}");
        Assert.All(
            Regex.Matches(all, @"[A-Za-z_]*FLOWSPEC[A-Za-z_]*").Select(x => x.Value),
            v => Assert.StartsWith("WSA_QOS_E", v, StringComparison.Ordinal));

        // 但已落地的 7 条常量名与 Qos.pas **逐字同名** ⇒ 未来若需要，直接引用即可，不必重译
        var client = ReadSource(Rest9NotPortedEvidence.QosAlreadyPortedInCSharpWhere);
        foreach (var name in Rest9NotPortedEvidence.QosAlreadyPortedInCSharp)
            Assert.Contains(name, client);
    }

    // ================= 4) DllUpdateCommon.pas =================

    private static string DllUpdateCommonSource => ReadSource("Source/RunGate/DllUpdateCommon.pas");

    [Fact]
    public void DllUpdateCommon_IsAnEmptyShell_ZeroDeclarations()
    {
        var src = DllUpdateCommonSource;
        Assert.Equal(10, PhysicalLineCount("Source/RunGate/DllUpdateCommon.pas"));

        // interface 与 implementation 之间必须**只有空白**
        var m = Regex.Match(src, @"interface(?<gap>.*?)implementation", RegexOptions.Singleline);
        Assert.True(m.Success);
        var gap = m.Groups["gap"].Value;
        _out.WriteLine($"interface..implementation 间隔 = {gap.Length} 字符，内容 = \"{gap.Replace("\r", "\\r").Replace("\n", "\\n")}\"");
        Assert.True(string.IsNullOrWhiteSpace(gap), "interface 段必须是空的");
        Assert.Equal(0, gap.Trim().Length);
        Assert.Equal(0, Rest9NotPortedEvidence.DllUpdateCommonInterfaceSectionLength);

        // 全文只有 unit/interface/uses Windows/implementation/end. 五行有效内容
        var declarations = CountMatches(src,
            @"^\s*(type|const|var|procedure|function|class|record|property)\b");
        _out.WriteLine($"声明关键字命中 = {declarations}（其中 1 个是 uses Windows 后的 implementation 不计）");
        Assert.Equal(Rest9NotPortedEvidence.DllUpdateCommonDeclarationCount, declarations);
        Assert.Equal(0, declarations);

        // 唯一的 uses 是 Windows，且没有任何 interface 段承载它 ⇒ 纯空壳
        Assert.Contains("uses", src);
        Assert.Contains("Windows", src);
    }

    [Fact]
    public void DllUpdateCommon_IsNotReferencedByAnyProjectFile()
    {
        var root = FindRepoRoot();
        var hits = new List<string>();
        foreach (var ext in new[] { "*.dpr", "*.dproj", "*.dpk" })
            foreach (var f in Directory.EnumerateFiles(root, ext, SearchOption.AllDirectories))
            {
                if (f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")) continue;
                if (File.ReadAllText(f).Contains("DllUpdateCommon", StringComparison.OrdinalIgnoreCase))
                    hits.Add(Path.GetRelativePath(root, f).Replace('\\', '/'));
            }
        hits.Sort(StringComparer.Ordinal);
        _out.WriteLine($".dpr/.dproj/.dpk 命中 = {hits.Count}：{string.Join(", ", hits)}");
        Assert.Empty(hits);

        // §37.3：先把"扫过的工程文件总数"数出来，证明不是"目录扫空导致的 0"
        var scanned = new[] { "*.dpr", "*.dproj", "*.dpk" }
            .Sum(e => Directory.EnumerateFiles(root, e, SearchOption.AllDirectories).Count());
        _out.WriteLine($"扫过的工程文件总数 = {scanned}");
        Assert.True(scanned >= 20, $"只扫到 {scanned} 个工程文件，请复核");
    }

    [Fact]
    public void DllUpdateCommon_IsNotInAnyPascalUsesClause()
    {
        var root = FindRepoRoot();
        var hits = new List<string>();
        foreach (var f in Directory.EnumerateFiles(
                     Path.Combine(root, "Source", "RunGate"), "*.pas", SearchOption.AllDirectories))
        {
            var p = Path.GetRelativePath(root, f).Replace('\\', '/');
            if (p.EndsWith("DllUpdateCommon.pas", StringComparison.Ordinal)) continue;
            if (Rest9NotPortedEvidence.UsesClauseContainsUnit(File.ReadAllText(f), "DllUpdateCommon"))
                hits.Add(p);
        }
        _out.WriteLine($"uses DllUpdateCommon 的 .pas = {hits.Count}");

        // §37.3：先把 RunGate 的 .pas 总数数出来（防"目录扫空"）
        var total = Directory.EnumerateFiles(
            Path.Combine(root, "Source", "RunGate"), "*.pas", SearchOption.AllDirectories).Count();
        _out.WriteLine($"RunGate .pas 总数 = {total}");
        Assert.True(total >= 40, $"只扫到 {total} 个 .pas，请复核");
        Assert.Empty(hits);
    }

    [Fact]
    public void DllUpdateCommon_AppearsOnlyInDocumentation_NotInCode()
    {
        var root = FindRepoRoot();
        var hits = new List<string>();
        foreach (var dir in new[]
                 {
                     Path.Combine(root, "GXX.CSharp"),
                     Path.Combine(root, "Source"),
                     Path.Combine(root, "Common"),
                 })
        {
            if (!Directory.Exists(dir)) continue;
            foreach (var f in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            {
                var name = Path.GetFileName(f);
                if (!name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".pas", StringComparison.OrdinalIgnoreCase)) continue;
                var text = File.ReadAllText(f);
                if (text.Contains("DllUpdateCommon", StringComparison.Ordinal))
                    hits.Add(Path.GetRelativePath(root, f).Replace('\\', '/'));
            }
        }
        hits.Sort(StringComparer.Ordinal);
        _out.WriteLine($"DllUpdateCommon 命中文件（md/tsv/pas）= {hits.Count}：{string.Join(", ", hits)}");
        Assert.Contains("Source/RunGate/DllUpdateCommon.pas", hits);
        // 除原文自身外，只允许出现在文档/映射表里
        Assert.All(hits, h => Assert.True(
            h == "Source/RunGate/DllUpdateCommon.pas" ||
            h.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
            h.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase),
            $"意外的代码引用：{h}"));
    }

    // ================= 5) 工具函数自检 =================

    [Fact]
    public void StripPascalComments_HandlesAllThreeCommentForms()
    {
        var src = "a{b}1(*c*)2//d" + "\n" + "e";
        Assert.Equal("a12\ne", Rest9NotPortedEvidence.StripPascalComments(src));
        // 未闭合注释吞到末尾（与 Delphi 编译器一致的错误情形，不抛异常）
        Assert.Equal("a", Rest9NotPortedEvidence.StripPascalComments("a{never closed"));
        Assert.Equal("a", Rest9NotPortedEvidence.StripPascalComments("a(*never closed"));
        // 被注释掉的方法签名必须消失——这正是 IODataPool 两个 SendBuffer 成员的判据
        Assert.DoesNotContain("TIODataPool.GetNewSendBufer",
            Rest9NotPortedEvidence.StripPascalComments("function TIODataPool.GetNewSendBufer(x: Integer): PSendBuffer;"));
        Assert.Contains("TIODataPool.Create",
            Rest9NotPortedEvidence.StripPascalComments("constructor TIODataPool.Create;"));
    }

    [Fact]
    public void UsesClauseContainsUnit_IsExactTokenMatch_NotSubstring()
    {
        var uses = "Classes, SysUtils, Windows, IocpWinsock2, IocpCommon,\r\n  IODataPool, IocpUtils;";
        Assert.True(Rest9NotPortedEvidence.UsesClauseContainsUnit(uses, "IODataPool"));
        Assert.True(Rest9NotPortedEvidence.UsesClauseContainsUnit(uses, "iodatapool"));
        // 子串不算：DllUpdateCommon 与 DllUpdateCommonEx 必须区分开
        Assert.False(Rest9NotPortedEvidence.UsesClauseContainsUnit(uses, "Data"));
        Assert.False(Rest9NotPortedEvidence.UsesClauseContainsUnit(uses, "Iocp"));
        Assert.False(Rest9NotPortedEvidence.UsesClauseContainsUnit(uses, "DllUpdateCommon"));
        Assert.True(Rest9NotPortedEvidence.UsesClauseContainsUnit("Classes, DllUpdateCommon;", "DllUpdateCommon"));
        Assert.False(Rest9NotPortedEvidence.UsesClauseContainsUnit("", "IODataPool"));
        Assert.False(Rest9NotPortedEvidence.UsesClauseContainsUnit(uses, ""));
    }

    [Fact]
    public void FourUnitVerdict_TotalsAreConsistent()
    {
        // 4 个单元：0 个移植、4 个「不移植 + 证据」
        Assert.Equal(4, Rest9NotPortedEvidence.SourceUnits.Count);
        Assert.Equal(4, Rest9NotPortedEvidence.SourceUnits.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(1229, Rest9NotPortedEvidence.PhysicalLineCounts.Values.Sum());
        // 11 + 19 + 27 + 0 个成员/常量全部登记在案
        Assert.Equal(11, Rest9NotPortedEvidence.IODataPoolMembers.Count);
        Assert.Equal(19, Rest9NotPortedEvidence.IocpTcpClientMembers.Count);
        Assert.Equal(27, Rest9NotPortedEvidence.QosConstantCount);
        Assert.Equal(0, Rest9NotPortedEvidence.DllUpdateCommonDeclarationCount);
        // 且全部都是**新建在 Rest9/** 下的"证据"，没有第二份实现
        Assert.All(Rest9NotPortedEvidence.SourceUnits, u => Assert.EndsWith(".pas", u));
    }
}
