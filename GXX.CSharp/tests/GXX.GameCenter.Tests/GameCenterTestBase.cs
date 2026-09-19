using System;
using System.Collections.Generic;
using System.IO;
using GXX.GameCenter;
using Xunit;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GameCenter 测试基座：每个测试类持有独立临时目录，并在构造/析构时复位
/// GShare.pas 全局状态（<see cref="GShareGlobals.ResetForTests"/>）、INI 单例、
/// 对话框接缝与全部宿主接线，保证测试互不干扰。
/// </summary>
public abstract class GameCenterTestBase : IDisposable
{
    /// <summary>本测试实例的临时根目录（对应一个 "D:\MirServer\"）。</summary>
    protected readonly string Dir;

    protected GameCenterTestBase()
    {
        Dir = Path.Combine(Path.GetTempPath(), "gxx_gc_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        Reset();
    }

    /// <summary>
    /// 复位全部全局状态并把 <c>g_sGameDirectory</c> 指向本测试临时目录
    /// （原文固定 'D:\MirServer\'，测试替换为临时目录以便断言真实落盘）。
    /// </summary>
    protected void Reset()
    {
        GShareGlobals.ResetForTests();
        GameCenterDialogs.ResetForTests();
        GMainConfig.DtpDateTimeSetter = null;
        GMainConfig.RefBackListToView = null;
        GMainConfig.BackListFileNameProvider = () => Path.Combine(Dir, "BackList.txt");
        GMainHelpers.ClearModValueHandler = null;
        GMainHelpers.ListCountProvider = null;
        GMainHelpers.ListItemProvider = null;
        GMainHelpers.ListClearHandler = null;
        GMainHelpers.ListAddHandler = null;
        GMainBackList.BackUpTaskFactory = null;
        GMainBackList.SetButtonEnabled = null;
        GMainBackList.ClearListView = null;
        GMainBackList.AddListViewRow = null;

        g_sGameDirectory = Dir + Path.DirectorySeparatorChar;
        g_sGameName = "BmM2";
        g_IniConf = new GameCenterIniFile(Path.Combine(Dir, "Config.ini"));
        GameCenterDialogs.MessageBoxHandler = (_, _, _) => GameCenterDialogs.IDOK;
    }

    public void Dispose()
    {
        g_IniConf?.Dispose();
        g_IniConf = null;
        GameCenterDialogs.ResetForTests();
        GShareGlobals.ResetForTests();
        try { Directory.Delete(Dir, true); } catch { /* best effort */ }
    }

    // ---------------- 断言辅助 ----------------

    /// <summary>以 GBK 读取全文（INI/清单文件落盘编码）。</summary>
    protected static string ReadGbk(string path)
        => File.Exists(path) ? File.ReadAllText(path, GXX.Core.EncodingInit.GBK) : "";

    /// <summary>以 GBK 读取为行数组（已去尾部空行）。</summary>
    protected static string[] ReadLinesGbk(string path)
    {
        string text = ReadGbk(path).Replace("\r\n", "\n").Replace("\r", "\n");
        var lines = text.Split('\n');
        if (lines.Length > 0 && lines[lines.Length - 1].Length == 0)
            Array.Resize(ref lines, lines.Length - 1);
        return lines;
    }

    /// <summary>
    /// 断言文本行序与内容逐行一致（键序保真的核心断言）。
    /// 首尾空元素会被忽略（尾部换行/节尾空行不构成键序证据）；
    /// expected 中为 null 的行只校验行数（用于包含临时路径或数值的行）。
    /// </summary>
    protected static void AssertLines(string path, params string?[] expected)
    {
        string[] actual = TrimTrailingActual(ReadLinesGbk(path));
        string?[] want = TrimTrailingExpected(expected);
        Assert.Equal(want.Length, actual.Length);
        for (int i = 0; i < want.Length; i++)
            if (want[i] != null)
                Assert.Equal(want[i], actual[i]);
    }

    private static string[] TrimTrailingActual(string[] lines)
    {
        int end = lines.Length;
        while (end > 0 && lines[end - 1].Length == 0) end--;
        if (end == lines.Length) return lines;
        var trimmed = new string[end];
        Array.Copy(lines, trimmed, end);
        return trimmed;
    }

    private static string?[] TrimTrailingExpected(string?[] lines)
    {
        int end = lines.Length;
        while (end > 0 && string.IsNullOrEmpty(lines[end - 1])) end--;
        if (end == lines.Length) return lines;
        var trimmed = new string?[end];
        Array.Copy(lines, trimmed, end);
        return trimmed;
    }

    /// <summary>诊断辅助：返回首个不一致的行描述（用于定位期望表偏差）。</summary>
    protected static string DescribeLineDiff(string path, params string?[] expected)
        => DescribeLineDiff(ReadLinesGbk(path), expected);

    /// <summary>诊断辅助：从已完成的内容数组比较。</summary>
    protected static string DescribeLineDiff(string[] actualLines, params string?[] expected)
    {
        string[] actual = TrimTrailingActual(actualLines);
        string?[] want = TrimTrailingExpected(expected);
        var sb = new System.Text.StringBuilder();
        sb.Append("want=").Append(want.Length).Append(" actual=").Append(actual.Length).Append(" ; ");
        int n = Math.Min(want.Length, actual.Length);
        for (int i = 0; i < n; i++)
        {
            if (want[i] == null) continue;
            if (want[i] != actual[i])
            {
                sb.Append("first diff at ").Append(i)
                  .Append(" want=[").Append(want[i]).Append("] actual=[").Append(actual[i]).Append(']');
                return sb.ToString();
            }
        }
        for (int i = n; i < actual.Length; i++)
        {
            sb.Append("extra actual[").Append(i).Append("]=[").Append(actual[i]).Append("] ");
        }
        for (int i = n; i < want.Length; i++)
        {
            sb.Append("missing wanted[").Append(i).Append("]=[").Append(want[i]).Append("] ");
        }
        return sb.ToString();
    }

    /// <summary>取路径在本测试临时目录下的绝对路径（自动创建父目录）。</summary>
    protected string Under(params string[] parts)
    {
        string p = Path.Combine(Dir, Path.Combine(parts));
        string? parent = Path.GetDirectoryName(p);
        if (!string.IsNullOrEmpty(parent)) Directory.CreateDirectory(parent);
        return p;
    }

    /// <summary>构造 [0..MAXRUNGATECOUNT-1] 的 RunGateInfo（前 count 个 boGetStart=True）。</summary>
    protected static void SetupRunGates(int count, int[]? gatePorts = null, int[]? dbPorts = null)
    {
        g_nRunGate_Count = count;
        g_RunGateInfo = new TRunGateInfo[GShareConst.MAXRUNGATECOUNT];
        for (int i = 0; i < GShareConst.MAXRUNGATECOUNT; i++)
        {
            g_RunGateInfo[i].boGetStart = i < count;
            g_RunGateInfo[i].nGatePort = gatePorts != null && i < gatePorts.Length
                ? gatePorts[i] : 7200 + i * 100;
            g_RunGateInfo[i].nDBPort = dbPorts != null && i < dbPorts.Length
                ? dbPorts[i] : 27201 + i * 100;
        }
    }

    /// <summary>内存备份管理器（DataBackUp.pas 接缝的最小实现）。</summary>
    protected sealed class MemoryBackUpManager : IBackUpManager
    {
        public IList<IBackUpTask> m_BackUpList { get; } = new List<IBackUpTask>();
        public void Add(IBackUpTask task) => m_BackUpList.Add(task);
    }

    /// <summary>内存备份任务（DataBackUp.pas TBackUpTask 接缝的最小实现）。</summary>
    protected sealed class MemoryBackUpTask : IBackUpTask
    {
        public string SourceDirectory { get; set; } = "";
        public string DestDirectory { get; set; } = "";
        public byte Mode { get; set; }
        public ushort Hour { get; set; }
        public ushort Min { get; set; }
        public bool Start { get; set; }
        public bool IsCompress { get; set; }
        public int BackUpCount { get; set; }
        public int FailCount { get; set; }
    }

    /// <summary>内存 ListBox（ListBoxAdd/ListBoxDel 接缝实现）。</summary>
    protected sealed class MemoryListBox : ListBoxTarget
    {
        public List<string> Items { get; } = new();
        public int SelectedIndex { get; set; } = -1;
        public int ItemsCount => Items.Count;
        public string ItemAt(int index) => Items[index];
        public void Add(string value) => Items.Add(value);
        public void DeleteSelected()
        {
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count) Items.RemoveAt(SelectedIndex);
        }
    }
}

/// <summary>禁止并行：GameCenter 全局状态（GShare.pas var 段）是进程级单例。</summary>
[CollectionDefinition("GameCenterSequential", DisableParallelization = true)]
public sealed class GameCenterSequentialCollection
{
}
