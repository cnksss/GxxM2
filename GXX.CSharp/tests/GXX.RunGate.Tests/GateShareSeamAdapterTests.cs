// 测试：GateShareSeamAdapters.cs —— 把 IProcessBlacklistSink 接到 GateShare 的真实落盘实现
//   覆盖 uFrmProcessBlacklist.pas:155-156 / :189-190 的"添加/删除后 Save + Rebuild"两步。
using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

[Collection("RunGateFormLane")]
public sealed class GateShareSeamAdapterTests : IDisposable
{
    private readonly string _dir;

    public GateShareSeamAdapterTests()
    {
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        _dir = Path.Combine(Path.GetTempPath(), "p2rg-seam-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        GateSharePaths.SetExeDirForTest(_dir);
    }

    public void Dispose()
    {
        GateSharePaths.ResetForTest();
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { }
    }

    [Fact]
    public void Sink_真的把进程黑名单落盘()
    {
        var sink = new GateShareProcessBlacklistSink();
        FormGlobals.g_ProcessBlackList.Add("cheat.exe", new string('a', 32));

        sink.SaveProcessBlacklist();

        string path = GateShareProcessBlacklistSink.FilePath;
        Assert.True(File.Exists(path));                                       // 上一轮的"只计数"实现不会建文件
        string text = File.ReadAllText(path, System.Text.Encoding.GetEncoding(936));
        Assert.Equal("cheat.exe|" + new string('A', 32) + "\r\n", text);        // MD5 已大写（原 :3301）
    }

    [Fact]
    public void Sink_Rebuild产出压缩负载与MD5()
    {
        var sink = new GateShareProcessBlacklistSink();
        FormGlobals.g_ProcessBlackList.Add("cheat.exe", new string('1', 32));

        sink.RebuildProcessBlacklist();

        Assert.NotEmpty(GateShareLists.ProcessBlacklistBytes);
        Assert.NotEqual("", FormGlobals.g_ProcessBlacklistStr);
        Assert.Equal(GateShareMd5.MD5Bytes(GateShareLists.ProcessBlacklistBytes),
                     FormGlobals.g_ProcessBlacklistMD5);
    }

    [Fact]
    public void Sink_空名单时Rebuild清空()
    {
        var sink = new GateShareProcessBlacklistSink();
        FormGlobals.g_ProcessBlackList.Add("a.exe", new string('2', 32));
        sink.RebuildProcessBlacklist();
        Assert.NotEmpty(GateShareLists.ProcessBlacklistBytes);

        FormGlobals.g_ProcessBlackList.Clear();
        sink.RebuildProcessBlacklist();
        Assert.Empty(GateShareLists.ProcessBlacklistBytes);
        Assert.Equal("", FormGlobals.g_ProcessBlacklistStr);
        Assert.Equal(new byte[16], FormGlobals.g_ProcessBlacklistMD5);
    }

    [Fact]
    public void Sink_可赋给ProcessBlacklistUnit并触发()
    {
        // 端到端：赋给 Sink 后，窗体路径上的两个触发器会真的落盘
        var original = ProcessBlacklistUnit.Sink;
        try
        {
            ProcessBlacklistUnit.Sink = new GateShareProcessBlacklistSink();
            FormGlobals.g_ProcessBlackList.Add("x.exe", new string('3', 32));

            ProcessBlacklistUnit.Sink.SaveProcessBlacklist();       // 原 :155 / :189
            ProcessBlacklistUnit.Sink.RebuildProcessBlacklist();    // 原 :156 / :190

            Assert.True(File.Exists(GateShareProcessBlacklistSink.FilePath));
            Assert.NotEmpty(GateShareLists.ProcessBlacklistBytes);
        }
        finally { ProcessBlacklistUnit.Sink = original; }
    }

    [Fact]
    public void Sink_FilePath跟随GateSharePaths()
    {
        Assert.Equal(_dir + Path.DirectorySeparatorChar + "ProcessBlacklist.txt",
                     GateShareProcessBlacklistSink.FilePath);
    }
}
