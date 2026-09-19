using System;
using System.IO;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>
/// 每条用例一个独立临时目录 + 独立 INI 文件（Delphi SelGate 的落盘文件均在工作目录，见 Protocol.pas:20-23）。
/// </summary>
public sealed class TempDir : IDisposable
{
    public string Path { get; }

    public TempDir()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "selgate-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public string File(string name) => System.IO.Path.Combine(Path, name);

    public void Dispose()
    {
        try { Directory.Delete(Path, true); } catch { /* 测试清理失败不影响结论 */ }
    }
}

/// <summary>
/// SelGateMisc/CloseIPConnect 用到的可控会话桩。
/// 对应 ClientSession.pas:11-40 TSessionObj 中参与判定的字段。
/// </summary>
public sealed class FakeSessionHost : ISelSessionHost
{
    public int IPAddr { get; set; }
    public string IPText { get; set; } = "";
    public bool Active { get; set; } = true;
    public int HandleLogin { get; set; }
    public int SvrObject { get; set; }
    public bool KickFlag { get; set; }
    public int FreeSocketCalls { get; set; }
    public int SendOutOfConnectionCalls { get; set; }
}

/// <summary>ClientSession.pas:17 m_tLastGameSvr: TClientThread 的测试桩。</summary>
public sealed class FakeClientThread : ISelClientThread
{
    public bool Active { get; set; } = true;
    public System.Collections.Generic.List<byte[]> Sent { get; } = new();

    public void SendBuffer(byte[] buf, int len)
    {
        byte[] copy = new byte[len];
        Array.Copy(buf, 0, copy, 0, Math.Min(len, buf.Length));
        Sent.Add(copy);
    }
}

/// <summary>
/// 独立类型里的 GetValidStr3(' ') 探针：同一逻辑放在带多个局部串的测试类方法里会被运行时串值
/// （见 src/GXX.SelGate/SelGatePacketRuleActive.cs 注释）。
/// </summary>
public sealed class SelPacketRuleActiveProbe
{
    public string First { get; }
    public string Rest { get; }
    public string IpText { get; }

    public SelPacketRuleActiveProbe(string listBoxText)
    {
        string[] tokens = listBoxText.Split(' ', 2);
        First = tokens[0];
        Rest = tokens.Length > 1 ? GXX.Core.Rtl.DelphiRTL.Trim(tokens[1]) : "";
        // 用 if 而不是三元（本构建下条件表达式的分支选择会被读到反转，见 SelGatePacketRuleActive 注释）
        if (Rest.Length == 0 || Rest[0] == '\u000F')
            IpText = First;
        else
            IpText = Rest;
    }
}
