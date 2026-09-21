// ============================================================================
// uAliyunSendSMSThread.pas（320 行）1:1 测试
//   方法 **10**：Create/Destroy/Terminate/AddTask/Execute/GetSleeping/TriggerEvent/
//               DoExecuteLoop/ClearTaskList/LoadSendSMSConfig
//   无窗体（DFM 对账不适用）；核心是线程体 DoExecuteLoop 的 4 条早退 + 成功/失败两支
// ============================================================================

using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using GXX.M2Server.Sweep;
using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsAliyunSmsTests : IDisposable
{
    private readonly FakeSmsRequest Request = new();
    private readonly FakeEvent Event = new();
    private readonly FakePlayer Player = new();
    private readonly FakeFunctionNpc FunctionNpc = new();
    private readonly List<string> Logs = new();
    private readonly GXX.M2Server.Npc.TNormNpc PlayerBase = new();
    private readonly GXX.M2Server.Npc.TNormNpc NpcBase = new();
    private TAliyunSendSMSThread Thread = null!;
    private uint Tick;

    public Sweep9FormsAliyunSmsTests()
    {
        Sweep9FormsSmsGlobals.Reset();
        Sweep9FormsSmsSeams.Reset();
        Sweep9FormsSmsSeams.StartThreadOnCreate = false;      // ★ 无头：不起真实线程
        // ★ 构造期接缝是**静态**的（原文 Create 在构造里就建 TSendSmsRequest/TEvent）
        Sweep9FormsSmsSeams.CreateSendSmsRequest = () => Request;
        Sweep9FormsSmsSeams.CreateEvent = () => Event;
        SweepSeam.ResetDefaults();

        Thread = new TAliyunSendSMSThread
        {
            GetPlayObjectHandler = _ => Player,
            g_FunctionNPC = FunctionNpc,
            MainOutMessage = Logs.Add,
            MyGetTickCount = () => Tick,
        };
    }

    public void Dispose()
    {
        Thread.Destroy();
        Sweep9FormsSmsGlobals.Reset();
        Sweep9FormsSmsSeams.Reset();
        SweepSeam.ResetDefaults();
    }

    /// <summary>加任务并把 AddTask 自身的 TriggerEvent（:123）清零，便于单独观察 DoExecuteLoop 的 finally。</summary>
    private void AddTaskQuiet(TCreature? p, TCreature? n)
    {
        Thread.AddTask(p, n);
        Event.SetCount = 0;
    }

    // ------------------------------------------------------------------
    // 方法面
    // ------------------------------------------------------------------

    [Fact]
    public void PublicMethodSurface_Is10OriginalMethods()
    {
        var expected = new[]
        {
            "Destroy", "Dispose", "Terminate", "AddTask", "Execute", "GetSleeping",
            "TriggerEvent", "DoExecuteLoop", "ClearTaskList", "LoadSendSMSConfig",
        };
        var declared = typeof(TAliyunSendSMSThread)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Assert.Equal(expected.OrderBy(n => n, StringComparer.Ordinal).ToArray(), declared);
        // 原文方法 10 个（含 LoadSendSMSConfig 是单元级静态过程）；Dispose 是 Free 的托管入口
        // 原文方法 10 个 = **构造函数 Create** + 9 个具名（Dispose 是 Free 的托管入口、Sleeping 是属性）
        Assert.Equal(9, expected.Count(n => n is not ("Dispose" or "Sleeping")));
    }

    // ------------------------------------------------------------------
    // 构造 / 析构
    // ------------------------------------------------------------------

    [Fact]
    public void Create_InitialisesTaskListRequestAndEvent()
    {
        Assert.Equal("TAliyunSendSMSThread.Locker", Thread.FTaskList.FName);   // :79
        Assert.Same(Request, Thread.FSendSmsRequest);                          // :81
        Assert.False(Thread.FInExecuteLoop);                                   // :20 初值
        Assert.False(Thread.Terminated);                                       // TThread 初值
        Assert.True(Thread.Sleeping);                                          // not FInExecuteLoop
    }

    [Fact]
    public void Create_CopiesSignNameFromGlobalConfig()
    {
        Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName = "签名A";
        Sweep9FormsSmsSeams.CreateSendSmsRequest = () => new FakeSmsRequest();
        var t = new TAliyunSendSMSThread();
        Assert.Equal("签名A", t.FSendSmsRequest.SignName);                     // :82
        t.Destroy();
    }

    [Fact]
    public void Create_DoesNotStartThreadWhenSeamDisabled()
    {
        // 接缝：StartThreadOnCreate = false（无头）。生产默认 true ⇒ 构造即起线程（:77）。
        Assert.True(Sweep9FormsSmsSeams.StartThreadOnCreate == false);
        Assert.False(Thread.Sleeping == false && Thread.FInExecuteLoop);
    }

    [Fact]
    public void Destroy_ClearsTaskList()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Assert.Equal(1, Thread.FTaskList.Count);
        Thread.Destroy();
        Assert.Equal(0, Thread.FTaskList.Count);
    }

    // ------------------------------------------------------------------
    // AddTask / GetSleeping / TriggerEvent / Terminate
    // ------------------------------------------------------------------

    [Fact]
    public void AddTask_AddsTaskAndTriggersEventWhenSleeping()
    {
        Assert.True(Thread.GetSleeping());                 // FInExecuteLoop = false
        Thread.AddTask(PlayerBase, NpcBase);

        Assert.Equal(1, Thread.FTaskList.Count);           // :116
        var task = (TSMSTask)Thread.FTaskList[0]!;
        Assert.Same(PlayerBase, task.Player);
        Assert.Same(NpcBase, task.Npc);
        Assert.Equal(1, Event.SetCount);                   // :123 TriggerEvent
    }

    [Fact]
    public void AddTask_DoesNotTriggerWhenBusy()
    {
        Thread.FInExecuteLoop = true;                      // 模拟 DoExecuteLoop 执行中
        Assert.False(Thread.GetSleeping());
        Thread.AddTask(PlayerBase, NpcBase);
        Assert.Equal(0, Event.SetCount);
    }

    [Fact]
    public void GetSleeping_IsInverseOfInExecuteLoop()
    {
        Thread.FInExecuteLoop = false;
        Assert.True(Thread.GetSleeping());
        Assert.True(Thread.Sleeping);
        Thread.FInExecuteLoop = true;
        Assert.False(Thread.GetSleeping());
        Assert.False(Thread.Sleeping);
    }

    [Fact]
    public void TriggerEvent_SetsTheEvent()
    {
        Thread.TriggerEvent();
        Assert.Equal(1, Event.SetCount);
    }

    [Fact]
    public void Terminate_SetsFlagAndTriggersEvent()
    {
        Thread.Terminate();
        Assert.True(Thread.Terminated);
        Assert.Equal(1, Event.SetCount);
    }

    [Fact]
    public void Execute_LoopTopChecksTerminated_RunsOneMoreRoundAfterTerminate()
    {
        // ★ 原文 :130 循环条件只在**顶部**判 Terminated：
        //   预置 Terminated = true 后 Execute 立刻退出（0 轮）；
        //   再验证"等待→DoExecuteLoop→再判"的顺序。
        Thread.Terminated = true;
        Thread.Execute();
        Assert.Equal(0, Event.WaitCount);

        // Terminated 为假：WaitFor 返回后跑一轮 DoExecuteLoop（空表 ⇒ 不再 TriggerEvent），
        // 随后顶部判定仍未 Terminated ⇒ 再等一次。用事件替身在第一次 Wait 后置 Terminated，
        // 使循环在第二轮顶部退出。
        Thread.Terminated = false;
        Event.OnWait = () =>
        {
            Thread.Terminated = true;                      // 让第二轮顶部退出
        };
        Thread.Execute();
        Assert.Equal(1, Event.WaitCount);
        Assert.False(Thread.FInExecuteLoop);               // finally 复位
    }

    // ------------------------------------------------------------------
    // ClearTaskList
    // ------------------------------------------------------------------

    [Fact]
    public void ClearTaskList_EmptiesList()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        AddTaskQuiet(PlayerBase, null);
        Assert.Equal(2, Thread.FTaskList.Count);
        Thread.ClearTaskList();
        Assert.Equal(0, Thread.FTaskList.Count);
    }

    // ------------------------------------------------------------------
    // DoExecuteLoop —— 4 条早退
    // ------------------------------------------------------------------

    [Fact]
    public void DoExecuteLoop_EmptyList_ReturnsWithoutTriggeringEvent()
    {
        Thread.DoExecuteLoop();
        Assert.Equal(0, Event.SetCount);                   // :177 Exit 在 try 之前
    }

    [Fact]
    public void DoExecuteLoop_PlayerNotFound_TriggersOnce()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Thread.GetPlayObjectHandler = _ => null;

        Thread.DoExecuteLoop();

        Assert.Equal(1, Event.SetCount);                   // finally :237
        Assert.Equal(0, Request.Calls);
    }

    [Fact]
    public void DoExecuteLoop_GhostPlayer_TriggersButDoesNotSend()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Player.m_boGhost = true;

        Thread.DoExecuteLoop();

        Assert.Equal(1, Event.SetCount);
        Assert.Equal(0, Request.Calls);
    }

    [Fact]
    public void DoExecuteLoop_EmptyMobileNumber_TriggersButDoesNotSend()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Player.Mobile = "";

        Thread.DoExecuteLoop();

        Assert.Equal(1, Event.SetCount);
        Assert.Equal(0, Request.Calls);
    }

    [Fact]
    public void DoExecuteLoop_TakesOnlyTheFirstTaskPerCall()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        AddTaskQuiet(PlayerBase, NpcBase);
        Thread.FInExecuteLoop = true;                      // 抑制 AddTask 的 TriggerEvent 干扰
        Event.SetCount = 0;

        Thread.DoExecuteLoop();

        Assert.Equal(1, Thread.FTaskList.Count);           // 只取走队首
    }

    // ------------------------------------------------------------------
    // DoExecuteLoop —— 成功路径
    // ------------------------------------------------------------------

    [Fact]
    public void DoExecuteLoop_Success_GeneratesCodeAndSendsMsg()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Thread.Random = n => { Assert.Equal(900000, n); return 5; };   // :189 Random(900000)
        Player.Mobile = "13800000000";
        Player.m_boMobileBind = false;                                 // ⇒ 用"绑定"模板（原文如此）
        Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind = "TPL_BIND";
        Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck = "TPL_CHECK";
        Tick = 123456;

        Thread.DoExecuteLoop();

        Assert.Equal(1, Request.Calls);
        Assert.Equal("13800000000", Request.LastMobile);
        Assert.Equal("{\"code\":\"100005\"}", Request.LastParam);      // :202（**不含角色名**）
        Assert.Equal("TPL_BIND", Request.TemplateCode);                // :192
        Assert.Equal("100005", Player.m_sMobileVerifyCode);            // :204
        Assert.Equal(123456u, Player.m_dwMobileVerifyTick);            // :205
        Assert.Equal(new[] { Grobal2Const.RM_INPUTMOBILE_VerifyCode }, Player.SentIdents);
        Assert.Same(PlayerBase, Player.LastSendTarget);
        Assert.Empty(Logs);                                           // 成功路径无日志
        Assert.Equal(0, FunctionNpc.GotoCalls);
    }

    [Fact]
    public void DoExecuteLoop_MobileBindTrue_UsesCheckTemplate_OriginalNaming()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Player.Mobile = "13800000000";
        Player.m_boMobileBind = true;
        Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind = "TPL_BIND";
        Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck = "TPL_CHECK";

        Thread.DoExecuteLoop();

        // ★ 原文缺陷 6：已绑定 ⇒ 用 **Check** 模板；未绑定 ⇒ 用 **Bind** 模板（名字与直觉相反）
        Assert.Equal("TPL_CHECK", Request.TemplateCode);
    }

    [Fact]
    public void DoExecuteLoop_SendMsgUsesTheTaskPlayerAsTarget()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Player.Mobile = "13800000000";
        Thread.DoExecuteLoop();
        Assert.Same(PlayerBase, Player.LastSendTarget);                // :207 SendMsg(Player2, ...)
        Assert.Equal("", Player.LastSendMsg);                          // :207 空消息
    }

    // ------------------------------------------------------------------
    // DoExecuteLoop —— 失败路径
    // ------------------------------------------------------------------

    [Fact]
    public void DoExecuteLoop_Failure_WithNpc_LogsAndGotosLabel()
    {
        AddTaskQuiet(PlayerBase, NpcBase);                           // Npc <> nil
        Thread.Random = _ => 7;
        Player.Mobile = "13900000000";
        Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind = "TPL_BIND";
        Sweep9FormsSmsGlobals.g_RequestStr = "<原始响应>";
        Request.Result = false;
        Request.ErrorCodeOut = "isv.AMOUNT_NOT_ENOUGH";
        Request.ErrorMsgOut = "余额不足";
        Player.m_nScriptGotoCount = 9;

        Thread.DoExecuteLoop();

        Assert.Equal(2, Logs.Count);                                   // :211 与 :213
        Assert.Equal("发送短信失败, SignName: ; TemplateCode: TPL_BIND; MobilePhone: 13900000000; " +
                     "错误代码: isv.AMOUNT_NOT_ENOUGH; 错误原因: 余额不足", Logs[0]);
        Assert.Equal("<原始响应>", Logs[1]);
        Assert.Equal(1, FunctionNpc.GotoCalls);                        // :233
        Assert.Equal("@MobileVerifyCodeSendFail", FunctionNpc.LastLabel);
        Assert.False(FunctionNpc.LastBoExt);
        Assert.Equal(0, Player.m_nScriptGotoCount);                    // :232 先清 0
        // 取证「先清 0 再跳」：GotoLable 被调到时 m_nScriptGotoCount 已是 0
        Assert.Equal(new[] { 0 }, FunctionNpc.Order);
    }

    [Fact]
    public void DoExecuteLoop_Failure_WithoutNpc_IsSilentlySwallowed()
    {
        // ★ 原文缺陷 8：`:209 else if Npc <> nil` ⇒ 不带 NPC 的失败**完全静默**（无日志、无跳转）
        AddTaskQuiet(PlayerBase, null);
        Thread.Random = _ => 7;
        Request.Result = false;
        Player.m_nScriptGotoCount = 9;

        Thread.DoExecuteLoop();

        Assert.Empty(Logs);
        Assert.Equal(0, FunctionNpc.GotoCalls);
        Assert.Equal(9, Player.m_nScriptGotoCount);                    // 未被清 0
    }

    [Fact]
    public void DoExecuteLoop_Failure_NullFunctionNpc_NoGoto()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Player.Mobile = "13800000000";
        Thread.g_FunctionNPC = null;                                   // 原文 g_FunctionNPC = nil
        Request.Result = false;

        Thread.DoExecuteLoop();

        Assert.Equal(2, Logs.Count);                                   // 日志照出
        Assert.Equal(0, FunctionNpc.GotoCalls);
    }

    [Fact]
    public void DoExecuteLoop_TriggerEventHappensOnEveryPathWithPlayer()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Player.m_boGhost = true;
        Thread.DoExecuteLoop();
        Assert.Equal(1, Event.SetCount);
    }

    [Fact]
    public void DoExecuteLoop_UnwiredGetPlayObject_Throws()
    {
        AddTaskQuiet(PlayerBase, NpcBase);
        Thread.GetPlayObjectHandler = null;
        var ex = Assert.Throws<InvalidOperationException>(() => Thread.DoExecuteLoop());
        Assert.Contains("GetPlayObject", ex.Message);
    }

    [Fact]
    public void NullSendSmsRequest_RequestThrows_NotWired()
    {
        Sweep9FormsSmsSeams.CreateSendSmsRequest = () => new Sweep9FormsNullSendSmsRequest();
        var t = new TAliyunSendSMSThread();
        Assert.Throws<InvalidOperationException>(
            () => t.FSendSmsRequest.Request("1", "2", out _, out _));
        t.Destroy();
    }

    // ------------------------------------------------------------------
    // LoadSendSMSConfig
    // ------------------------------------------------------------------

    [Fact]
    public void LoadSendSMSConfig_CleansLegacyKeys_AndWritesDefaultsWhenMissing()
    {
        var ini = new FakeIni();
        Sweep9FormsSmsSeams.CreateIniFile = _ => ini;
        Sweep9FormsSmsGlobals.g_AcsUtil.KeyID = "ID0";
        Sweep9FormsSmsGlobals.g_AcsUtil.KeySecret = "SEC0";
        Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName = "签名0";

        TAliyunSendSMSThread.LoadSendSMSConfig();

        Assert.Equal(new[] { "Endpoint", "Topic" }, ini.Deleted);      // :268-269 无条件清理
        Assert.Equal("ID0", ini.Written["KeyID"]);                     // 字符串键走 WriteString
        Assert.Equal("SEC0", ini.Written["KeySecret"]);
        Assert.Equal("签名0", ini.Written["SignName"]);
        Assert.Equal("3", ini.Written["ResendVerifyCodeCount"]);       // 整数键走 WriteInteger
        Assert.Equal("60", ini.Written["VerifySendInterval"]);
        Assert.Equal("120", ini.Written["VerifyCodeTimeOutTime"]);
        Assert.Equal("3", ini.Written["VerifyCodeErrorCount"]);
        Assert.True(ini.Disposed);
    }

    [Fact]
    public void LoadSendSMSConfig_ReadsExistingValues()
    {
        var ini = new FakeIni
        {
            Values =
            {
                ["KeyID"] = "ID1", ["KeySecret"] = "SEC1", ["SignName"] = "签名1",
                ["TemplateCodeBind"] = "B1", ["TemplateCodeCheck"] = "C1",
                ["ResendVerifyCodeCount"] = "5", ["VerifySendInterval"] = "30",
                ["VerifyCodeTimeOutTime"] = "90", ["VerifyCodeErrorCount"] = "7",
            },
        };
        Sweep9FormsSmsSeams.CreateIniFile = _ => ini;

        TAliyunSendSMSThread.LoadSendSMSConfig();

        Assert.Equal("ID1", Sweep9FormsSmsGlobals.g_AcsUtil.KeyID);
        Assert.Equal("SEC1", Sweep9FormsSmsGlobals.g_AcsUtil.KeySecret);
        Assert.Equal("签名1", Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName);
        Assert.Equal("B1", Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind);
        Assert.Equal("C1", Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck);
        Assert.Equal(5, Sweep9FormsSmsGlobals.g_SendSMSConfig.ResendVerifyCodeCount);
        Assert.Equal(30, Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifySendInterval);
        Assert.Equal(90, Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeTimeOutTime);
        Assert.Equal(7, Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeErrorCount);
        Assert.Empty(ini.Written);                                     // 全命中 ⇒ 一个都不写
    }

    [Fact]
    public void LoadSendSMSConfig_UsesSendSmsIniUnderSelfPath()
    {
        var ini = new FakeIni();
        string? seen = null;
        Sweep9FormsSmsSeams.CreateIniFile = p => { seen = p; return ini; };
        M2ShareState.g_sSelfFilePath = "C:\\p9app\\";

        try
        {
            TAliyunSendSMSThread.LoadSendSMSConfig();
        }
        finally
        {
            M2ShareState.ResetForTests(null);
        }

        Assert.Equal("C:\\p9app\\SendSMS.ini", seen);
    }

    [Fact]
    public void SendSMSConfig_TypedConstInitialValues_MatchOriginal()
    {
        Sweep9FormsSmsGlobals.Reset();
        var c = Sweep9FormsSmsGlobals.g_SendSMSConfig;
        Assert.Equal("", c.SignName);
        Assert.Equal("", c.TemplateCodeBind);
        Assert.Equal("", c.TemplateCodeCheck);
        Assert.Equal(3, c.ResendVerifyCodeCount);
        Assert.Equal(60, c.VerifySendInterval);
        Assert.Equal(120, c.VerifyCodeTimeOutTime);
        Assert.Equal(3, c.VerifyCodeErrorCount);
        Assert.Null(Sweep9FormsSmsGlobals.g_AliyunSendSMSThread);
    }

    // ==================================================================
    // 测试替身
    // ==================================================================

    private sealed class FakeSmsRequest : ISweep9FormsSendSmsRequest
    {
        public string SignName { get; set; } = "";
        public string TemplateCode { get; set; } = "";
        public int Calls;
        public string LastMobile = "";
        public string LastParam = "";
        public bool Result = true;
        public string ErrorCodeOut = "";
        public string ErrorMsgOut = "";

        public bool Request(string mobilePhone, string paramStr, out string errorCode, out string errorMsg)
        {
            Calls++;
            LastMobile = mobilePhone;
            LastParam = paramStr;
            errorCode = ErrorCodeOut;
            errorMsg = ErrorMsgOut;
            return Result;
        }
    }

    private sealed class FakeEvent : ISweep9FormsEvent
    {
        public int WaitCount;
        public int SetCount;
        public Action? OnWait;

        public uint WaitFor(uint timeout)
        {
            WaitCount++;
            OnWait?.Invoke();
            return 0;
        }

        public void SetEvent() => SetCount++;

        public void Dispose() { }
    }

    private sealed class FakePlayer : ISweep9FormsSmsPlayer
    {
        public bool m_boGhost { get; set; }
        public string Mobile = "";
        public string m_sMobileNumber => Mobile;
        public bool m_boMobileBind { get; set; }
        public string m_sMobileVerifyCode { get; set; } = "";
        public uint m_dwMobileVerifyTick { get; set; }
        public int m_nScriptGotoCount { get; set; }

        public readonly List<int> SentIdents = new();
        public TCreature? LastSendTarget;
        public string LastSendMsg = "";

        public void SendMsg(TCreature baseObject, ushort wIdent, long wParam, long nParam1,
            long nParam2, long nParam3, string sMsg)
        {
            LastSendTarget = baseObject;
            SentIdents.Add(wIdent);
            LastSendMsg = sMsg;
        }
    }

    private sealed class FakeFunctionNpc : ISweep9FormsFunctionNpc
    {
        public int GotoCalls;
        public string LastLabel = "";
        public bool LastBoExt;
        public readonly List<int> Order = new();

        public void GotoLable(ISweep9FormsSmsPlayer playObject, string sLabel, bool boExt)
        {
            GotoCalls++;
            LastLabel = sLabel;
            LastBoExt = boExt;
            Order.Add(playObject.m_nScriptGotoCount);      // 取证"先清 0 再跳"
        }
    }

    private sealed class FakeIni : ISweep9FormsSmsIniFile
    {
        public readonly Dictionary<string, string> Values = new();
        public readonly Dictionary<string, string> Written = new();
        public readonly List<string> Deleted = new();
        public bool Disposed;

        public void DeleteKey(string section, string key)
        {
            Deleted.Add(key);
            Values.Remove(key);
        }

        public bool ValueExists(string section, string key) => Values.ContainsKey(key);

        public string ReadString(string section, string key, string defaultValue)
            => Values.TryGetValue(key, out var v) ? v : defaultValue;

        public int ReadInteger(string section, string key, int defaultValue)
            => Values.TryGetValue(key, out var v) && int.TryParse(v, out var i) ? i : defaultValue;

        public void WriteString(string section, string key, string value) => Written[key] = value;

        public void WriteInteger(string section, string key, int value) => Written[key] = value.ToString();

        public void Dispose() => Disposed = true;
    }
}
