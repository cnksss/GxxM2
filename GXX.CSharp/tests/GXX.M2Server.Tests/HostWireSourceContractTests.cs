// ============================================================================
// 车道 p17-m2-hostwire：宿主接线层的**结构性契约**用例（源码级回归守卫）
//
// 为什么需要它（台账 §59.7 的教训）：
//   上一次真实回归的形态是 —— "**把默认路径的语句搬进了 opt-in 分支**"
//   （`LoginGateService.OnClientReceive` 默认分支的两条透传语句被搬进 Rest11 分支
//   ⇒ 关闭 Rest11 时网关什么都不转发）。单元测试各自都绿，只有集成用例抓到。
//   而**注释还写着"默认路径：与接线前逐字节一致"** ⇒ 注释与现实相反。
//   ⇒ 结论：**"默认路径没被搬空"必须机器可证，不能靠注释自证。**
//
// 本文件用**读源码文本**的方式，把本车道在 `src/GXX.M2Server/Program.cs` 上
// 对宿主接线层做的改动锁成可执行契约：
//   * 两处接缝都必须是 **null 第一行短路**，且**在默认分派之前**；
//   * `data.Length < 22` 长度门控与 `EDcode.DecodeMessage` 必须**留在默认路径**
//     （即出现在接缝判断之**前**）；
//   * 默认 `CM_*` 四个 case 标签必须**仍然存在**（没被搬进分支而消失）；
//   * `MainLoopTickHook` 的 null 守卫必须在 `UserEngine.Process()` 之**前**；
//   * **D-P17-01 回归守卫**：`(byte)msg.Recog` 作为**方向**的读法必须**一个都不剩**，
//     方向一律走 `(byte)msg.Param`。
//
// ⚠ 源码定位口径与既有先例一致（`ObjMonRealSourceHarness.CandidateRepoRoots` /
//   `PluginsAssemblyLoaderTests`）：从 `AppContext.BaseDirectory` 与
//   `Environment.CurrentDirectory` 逐级上溯，找**同时含 `Source` 与 `GXX.CSharp`** 的目录。
//   取不到就**抛**（§48.1：取证设施取不到证据必须失败，不许静默通过）。
//
// ⚠ 本文件只做**文本结构**断言，不做行为断言 —— 行为断言在同目录
//   `HostWireE2ETests.cs`（真宿主 + 真 socket）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// `src/GXX.M2Server/Program.cs`（宿主接线层）的结构性契约：默认路径不得被搬空（§59.7）。
/// </summary>
public class HostWireSourceContractTests
{
    /// <summary>被守卫的宿主文件（相对仓库根）。</summary>
    private const string HostFileRelative = @"GXX.CSharp\src\GXX.M2Server\Program.cs";

    private static readonly Lazy<string> Source = new(LoadHostSource);

    private static string LoadHostSource()
    {
        foreach (string root in CandidateRepoRoots())
        {
            string path = Path.Combine(root, HostFileRelative);
            if (File.Exists(path)) return File.ReadAllText(path);
        }
        throw new FileNotFoundException(
            "找不到宿主文件 " + HostFileRelative + "（从当前目录与测试程序集目录都上溯不到仓库根）。"
            + " 本类的断言全部以该文件为据，取不到必须失败。");
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

    /// <summary>`needle` 在源码里的下标；找不到即抛（断言的前提必须成立）。</summary>
    private static int IndexOf(string needle)
    {
        int i = Source.Value.IndexOf(needle, StringComparison.Ordinal);
        Assert.True(i >= 0, "宿主源码里找不到契约锚点：" + needle);
        return i;
    }

    private static int CountOf(string needle)
        => Source.Value.Split(new[] { needle }, StringSplitOptions.None).Length - 1;

    // ---------------------------------------------------------------- 接缝本身

    /// <summary>
    /// 两处宿主接缝必须是**实例**字段（不是 static）：避免污染进程级静态全局，
    /// 也免于 `M2ConfigIsolationCoverage`（本车道改不到的那个文件）是否覆盖本类型的
    /// 不确定性 ⇒ 由**用例自己**在退出前置回 null，而不是"靠机制自觉"。
    /// </summary>
    [Fact]
    public void HostSeams_AreInstanceFields_NotStaticGlobals()
    {
        string src = Source.Value;
        Assert.Contains("public Action<uint>? MainLoopTickHook;", src);
        Assert.Contains("public Func<int, TDefaultMessage, bool>? GateMessageDispatchHook;", src);
        Assert.DoesNotContain("static Action<uint>? MainLoopTickHook", src);
        Assert.DoesNotContain("static Func<int, TDefaultMessage, bool>? GateMessageDispatchHook", src);
    }

    /// <summary>
    /// **§59.7 核心契约**：接缝判断在解码之后、默认 `CM_*` 分派之前，且是第一行短路；
    /// 长度门控与解码**必须留在前面**（不得被搬进接缝分支）。
    /// </summary>
    [Fact]
    public void DispatchSeam_IsNullShortCircuit_BeforeDefaultSwitch_DecodeStaysOnDefaultPath()
    {
        string src = Source.Value;

        const string hook = "if (dispatchHook != null && dispatchHook(sockId, msg)) return;";
        Assert.Equal(1, CountOf(hook));                            // 只此一处

        int iLen = IndexOf("if (data.Length < 22) return;");       // 长度门控
        int iDecode = IndexOf("TDefaultMessage msg = EDcode.DecodeMessage(data);");
        int iHookRead = IndexOf("var dispatchHook = GateMessageDispatchHook;");
        int iHook = IndexOf(hook);
        int iSwitch = IndexOf("switch (msg.Ident)");

        Assert.True(iLen < iHook, "长度门控被搬到了接缝之后（§59.7 回归形态）");
        Assert.True(iDecode < iHook, "解码被搬到了接缝之后（§59.7 回归形态）");
        Assert.True(iHookRead < iHook, "接缝取值应在接缝判断之前（第一行短路的前提）");
        Assert.True(iHook < iSwitch, "接缝判断必须在默认 switch 之前");
    }

    /// <summary>
    /// 默认 `CM_*` 四个 case 标签必须**仍然存在**：证明默认分派没有被"搬进 opt-in 分支"后消失。
    /// </summary>
    [Fact]
    public void DefaultCmCases_StillPresent_NotMovedIntoOptInBranch()
    {
        string src = Source.Value;
        foreach (string label in new[]
                 {
                     "case Grobal2Const.CM_WALK:",
                     "case Grobal2Const.CM_RUN:",
                     "case Grobal2Const.CM_TURN:",
                     "case Grobal2Const.CM_QUERYUSERNAME:"
                 })
        {
            Assert.True(src.Contains(label, StringComparison.Ordinal), "默认分派的 case 标签丢失：" + label);
        }

        // 四个 case 都必须排在接缝判断之后（即仍属"默认路径"）
        int iHook = IndexOf("if (dispatchHook != null && dispatchHook(sockId, msg)) return;");
        Assert.True(IndexOf("case Grobal2Const.CM_WALK:") > iHook);
        Assert.True(IndexOf("case Grobal2Const.CM_TURN:") > iHook);
    }

    /// <summary>
    /// 主循环接缝：null 守卫必须在 `UserEngine.Process()` **之前**，
    /// 且默认（null）时**连 `GetTickCount()` 都不取** —— 默认路径无额外副作用。
    /// </summary>
    [Fact]
    public void MainLoopSeam_IsNullShortCircuit_BeforeProcess()
    {
        int iRead = IndexOf("var tickHook = MainLoopTickHook;");
        int iGuard = IndexOf("if (tickHook != null) tickHook(DelphiRTL.GetTickCount());");
        int iProcess = IndexOf("UserEngine.Process();");

        Assert.True(iRead < iGuard, "接缝取值应在守卫之前");
        Assert.True(iGuard < iProcess, "主循环接缝必须在 UserEngine.Process() 之前");
        Assert.Equal(1, CountOf("if (tickHook != null) tickHook(DelphiRTL.GetTickCount());"));
    }

    // ---------------------------------------------------------------- D-P17-01

    /// <summary>
    /// **D-P17-01 回归守卫（源码级）**：移动/转向的**方向一律取 `msg.Param`**，
    /// 源码里**不得再出现把 `Recog` 当方向**的读法。
    ///
    /// <para>
    /// 缺陷原貌与证据见 `HostWireE2ETests` 文件头注释与本车道报告 D-P17-01：
    /// `Recog` 是**对象标识**（原文 `ObjPlayer.pas:20196`；本文件自己的出站帧也这么用），
    /// 而 `TCreature.WalkTo` 用 `s_DirX[Math.Min(dir, 7)]` **静默夹取**越界方向
    /// ⇒ 走错方向既不抛也不打日志。
    /// </para>
    /// </summary>
    [Fact]
    public void DirectionCarrier_IsParam_NoRecogAsDirection_D_P17_01()
    {
        string src = Source.Value;

        Assert.Equal(0, CountOf("(byte)msg.Recog"));          // 一个都不许剩
        Assert.Equal(2, CountOf("byte dir = (byte)msg.Param;")); // CM_WALK/CM_RUN 一处 + CM_TURN 一处
    }

    /// <summary>
    /// 宿主入口 `M2EngineService` 的**可达性证据**：`Program.Main` 里真的 `new` 了它并
    /// `Application.Run` —— 即"宿主不是死代码"。
    /// <para>
    /// 反向证据（本车道报告 §端到端要用）：`tests/**` 在修本车道之前对 `M2EngineService`
    /// **零命中** —— 这条由 <see cref="HostWireE2ETests"/> 实际启动宿主来补足。
    /// </para>
    /// </summary>
    [Fact]
    public void HostEntryPoint_IsReachableFromMain()
    {
        string src = Source.Value;
        int iNew = IndexOf("var engine = new M2EngineService();");
        int iRun = IndexOf("Application.Run(new FrmMain(engine));");
        Assert.True(iNew < iRun, "宿主实例应在 Application.Run 之前构造");
    }
}
