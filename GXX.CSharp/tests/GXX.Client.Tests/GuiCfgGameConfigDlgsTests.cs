using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Core.Rtl;
using GXX.Core.Util;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-client-guiconfig）：GameConfigDlgs.pas(1-174) 1:1 移植测试。
/// 覆盖：TConfigDlgManage 的插件装载/卸载、Initialize 参数拼装、
/// Finalize 的"只处理当前没在用的那一类"反直觉分支、GetConfigDlg 的**回退到第 0 项**语义、
/// 以及 LoadControlFromStream 的调用次序与 finally 释放。
/// </summary>
[Collection("GuiCfgConfigure")]
public sealed class GuiCfgGameConfigDlgsTests : IDisposable
{
    public GuiCfgGameConfigDlgsTests() => GuiCfgTestEnv.Reset();

    public void Dispose() => GuiCfgTestEnv.Reset();

    private static bool[] Configs(int n, params int[] trueAt)
    {
        var a = new bool[n];
        foreach (int i in trueAt) a[i] = true;
        return a;
    }

    // ============================================================ 构造/析构

    [Fact]
    public void CreateMakesEmptyList()
    {
        var m = new TConfigDlgManage();
        Assert.NotNull(m.ConfigDlgList);
        Assert.Equal(0, m.ConfigDlgList.Count);
    }

    [Fact]
    public void DestroyUnloadsPluginList()
    {
        // 69-73：UnLoadPlugIn 后 Free
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", new TStubGameConfigObject(TConfigDlgType.ptDefault));
        m.Destroy();
        Assert.Equal(0, m.ConfigDlgList.Count);
    }

    // ============================================================ LoadPlugIn

    [Fact]
    public void LoadPlugInAlwaysAddsAYsConfigDlgFirst()
    {
        // 75-83：**无条件**先建一个 TJSYConfigDlg
        ClientGlobalSeam.ConfigClientConfigs = Configs(60);
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvMirs;   // 不在 case 列表内

        var m = new TConfigDlgManage();
        m.LoadPlugIn();

        Assert.Equal(1, m.ConfigDlgList.Count);
        Assert.IsType<TJSYConfigDlg>(m.ConfigDlgList.GetObject(0));
        Assert.Equal("", m.ConfigDlgList[0]);
    }

    [Theory]
    [InlineData(TClientVersion.cv176)]
    [InlineData(TClientVersion.cv185)]
    [InlineData(TClientVersion.cvHero)]
    [InlineData(TClientVersion.cvSerial)]
    [InlineData(TClientVersion.cvMirSequel)]
    [InlineData(TClientVersion.cvMirNewUI205)]
    public void LoadPlugInAddsMirConfigDlgForSixVersions(TClientVersion version)
    {
        // 91-106 的 case 覆盖 cv176/cv185/cvHero/cvSerial/cvMirSequel/cvMirNewUI205
        ClientGlobalSeam.ConfigClientConfigs = Configs(60);
        ClientGlobalSeam.g_ClientVersion = version;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();

        Assert.Equal(2, m.ConfigDlgList.Count);
        Assert.IsType<TJSYConfigDlg>(m.ConfigDlgList.GetObject(0));
        // 集成方修正（台账 §47）：原断言写的是**桩**类型 `TStubGameConfigObject`，它描述的是"接缝尚未接线"的
        // 旧状态；车道 p10-client-mirconfig 已把接缝接到真实现，且桩污染源（GuiCfgConfigShareTests:1011）
        // 也已修掉 ⇒ 该断言若不改，会因"恰好被污染"而假绿（实测：整轮跑时它通过，单独跑时才暴露）。
        Assert.IsType<GXX.Client.GUI.GameConfig.Mir.TMirConfigDlg>(m.ConfigDlgList.GetObject(1));
        Assert.Equal(TConfigDlgType.ptDefault, ((TGameConfigObject)m.ConfigDlgList.GetObject(1)).ConfigDlgType);
    }

    [Theory]
    [InlineData(TClientVersion.cvMirs)]        // 检查：cvMirs 不在 case 列表内
    [InlineData(TClientVersion.cvMirReturn)]
    [InlineData(TClientVersion.cvMirReturn2)]
    public void LoadPlugInSkipsMirConfigDlgForOtherVersions(TClientVersion version)
    {
        ClientGlobalSeam.ConfigClientConfigs = Configs(60);
        ClientGlobalSeam.g_ClientVersion = version;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();

        Assert.Equal(1, m.ConfigDlgList.Count);
    }

    [Fact]
    public void LoadPlugInCopiesClientConfigsThenAddsSceneShakeFromIndex51()
    {
        // 85-88（TESTMODE=0 段）：
        //   循环 0..Length-1 复制，然后**额外**用下标 51 覆盖 ckSceneShake
        var cfg = new bool[60];
        cfg[3] = true;
        cfg[51] = false;                 // 先让 ckSceneShake 为 False
        ClientGlobalSeam.ConfigClientConfigs = cfg;
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvMirs;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();
        var jsy = (TGameConfigObject)m.ConfigDlgList.GetObject(0);
        // cfg[51] = false → ckSceneShake = False，而 cfg[53]（ckSceneShake 自身下标）并未被读
        Assert.False(jsy.GetConfigChecked(TConfigChecked.ckSceneShake));

        // 循环把下标 3 直接当成 TConfigChecked(3) = ckFilterExp
        Assert.True(jsy.GetConfigChecked(TConfigChecked.ckFilterExp));
    }

    [Fact]
    public void LoadPlugInSceneShakeComesFromSlot51NotFromItsOwnIndex()
    {
        // 原文 88：`ConfigObject.ConfigCheckeds[ckSceneShake] := g_ConfigClient.ClientConfigs[51];`
        // ckSceneShake 的枚举值是 53，与下标 51 不同 —— 这是一处**故意的错位**
        Assert.Equal(53, (int)TConfigChecked.ckSceneShake);

        var cfg = new bool[60];
        cfg[51] = true;      // 供给源
        cfg[53] = false;     // 枚举自身下标：应被忽略
        ClientGlobalSeam.ConfigClientConfigs = cfg;
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvMirs;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();
        var jsy = (TGameConfigObject)m.ConfigDlgList.GetObject(0);
        Assert.True(jsy.GetConfigChecked(TConfigChecked.ckSceneShake));
    }

    [Fact]
    public void LoadPlugInMirConfigDlgUsesLowToHighLoop()
    {
        // 101-103：`for I := Low(...) to High(...)` —— 与 JSY 侧的 `0..Length-1` 写法不同但等价
        // 数组长度须 >= 52，否则原文第 88 行的写死下标 51 会越界
        var cfg = new bool[52];
        cfg[0] = true;
        cfg[19] = true;
        ClientGlobalSeam.ConfigClientConfigs = cfg;
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cv176;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();
        var mir = (TGameConfigObject)m.ConfigDlgList.GetObject(1);
        Assert.True(mir.GetConfigChecked((TConfigChecked)0));
        Assert.True(mir.GetConfigChecked((TConfigChecked)19));
    }

    [Fact]
    public void LoadPlugInWithEmptyClientConfigsStillAddsDlgs()
    {
        // 数组长度为 0：两个循环都不执行（原文第 88 行会越界，此处按边界检查跳过）
        ClientGlobalSeam.ConfigClientConfigs = Array.Empty<bool>();
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cv176;
        var m = new TConfigDlgManage();
        m.LoadPlugIn();
        Assert.Equal(2, m.ConfigDlgList.Count);
    }

    [Fact]
    public void LoadPlugInIsAdditiveOnRepeatedCalls()
    {
        // 原文每次调用都 AddObject，不先清空 → 重复调用会累积（照抄）
        ClientGlobalSeam.ConfigClientConfigs = Configs(60);
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvMirs;
        var m = new TConfigDlgManage();
        m.LoadPlugIn();
        m.LoadPlugIn();
        Assert.Equal(2, m.ConfigDlgList.Count);
    }

    // ============================================================ UnLoadPlugIn

    [Fact]
    public void UnLoadPlugInClearsEverything()
    {
        ClientGlobalSeam.ConfigClientConfigs = Configs(60);
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cv176;
        var m = new TConfigDlgManage();
        m.LoadPlugIn();
        Assert.Equal(2, m.ConfigDlgList.Count);

        m.UnLoadPlugIn();
        Assert.Equal(0, m.ConfigDlgList.Count);
    }

    // ============================================================ Initialize

    [Fact]
    public void InitializePassesHandleMakeLongScreenAndVersion()
    {
        // 119-127：Initialize(frmMain.Handle, MakeLong(g_nScreenWidth, g_nScreenHeight),
        //                     g_ClientVersion, g_boWindowMode)
        var rec = new RecordingConfigObject(TConfigDlgType.ptDefault);
        ClientGlobalSeam.frmMainHandle = new IntPtr(0x4321);
        ClientGlobalSeam.g_nScreenWidth = 1024;
        ClientGlobalSeam.g_nScreenHeight = 768;
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvHero;
        ClientGlobalSeam.g_boWindowMode = true;

        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", rec);
        m.Initialize();

        Assert.Equal(1, rec.InitCalls);
        Assert.Equal(new IntPtr(0x4321), rec.LastHandle);
        // MakeLong(1024, 768) = (768 << 16) | 1024 = 0x03000400，再按形参类型（Byte）截断 → 低字节 0x00
        Assert.Equal(0x03000400u, (uint)DelphiRTL.MakeLong(1024, 768));
        Assert.Equal(0x00, rec.LastScreenMode);
        Assert.Equal(TClientVersion.cvHero, rec.LastVersion);
        Assert.True(rec.LastWindowMode);
    }

    [Fact]
    public void InitializeScreenModeKeepsLowByteOfMakeLong()
    {
        // 换一组能让 MakeLong 低字节非 0 的分辨率（宽 1024 的低字节是 0x00，看不出问题）
        var rec = new RecordingConfigObject(TConfigDlgType.ptDefault);
        ClientGlobalSeam.g_nScreenWidth = 0x2345;
        ClientGlobalSeam.g_nScreenHeight = 0x6789;

        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", rec);
        m.Initialize();

        Assert.Equal(0x2345u, (uint)(DelphiRTL.MakeLong(0x2345, 0x6789) & 0xFFFF));
        Assert.Equal(0x45, rec.LastScreenMode);
    }

    [Fact]
    public void InitializeOnEmptyListIsNoOp()
    {
        var m = new TConfigDlgManage();
        m.Initialize();     // 不应抛异常
        Assert.Equal(0, m.ConfigDlgList.Count);
    }

    [Fact]
    public void InitializeCallsEveryEntry()
    {
        var a = new RecordingConfigObject(TConfigDlgType.ptDefault);
        var b = new RecordingConfigObject(TConfigDlgType.ptJSY);
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", a);
        m.ConfigDlgList.AddObject("", b);
        m.Initialize();
        Assert.Equal(1, a.InitCalls);
        Assert.Equal(1, b.InitCalls);
    }

    // ============================================================ Finalize

    [Fact]
    public void FinalizeWithConfigDlgTypeZeroFinalizesNonJsyOnly()
    {
        // 129-142：btConfigDlgType = 0 时，`if not (X is TJSYConfigDlg) then X.Finalize`
        ClientGlobalSeam.g_ClientConfig = new TClientConfig { btConfigDlgType = 0 };
        var jsy = new TJSYConfigDlg();
        var mir = new TStubGameConfigObject(TConfigDlgType.ptDefault);
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", jsy);
        m.ConfigDlgList.AddObject("", mir);

        m.Finalize();

        Assert.Equal(0, jsy.FinalizeCalls);
        Assert.Equal(1, mir.FinalizeCalls);
    }

    [Fact]
    public void FinalizeWithNonZeroConfigDlgTypeFinalizesJsyOnly()
    {
        // 138-141：否则只 Finalize TJSYConfigDlg
        ClientGlobalSeam.g_ClientConfig = new TClientConfig { btConfigDlgType = 1 };
        var jsy = new TJSYConfigDlg();
        var mir = new TStubGameConfigObject(TConfigDlgType.ptDefault);
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", jsy);
        m.ConfigDlgList.AddObject("", mir);

        m.Finalize();

        Assert.Equal(1, jsy.FinalizeCalls);
        Assert.Equal(0, mir.FinalizeCalls);
    }

    [Fact]
    public void FinalizeWithOnlyJsyAndTypeZeroDoesNothing()
    {
        ClientGlobalSeam.g_ClientConfig = new TClientConfig { btConfigDlgType = 0 };
        var jsy = new TJSYConfigDlg();
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", jsy);
        m.Finalize();
        Assert.Equal(0, jsy.FinalizeCalls);
    }

    [Fact]
    public void FinalizeOnEmptyListIsNoOp()
    {
        var m = new TConfigDlgManage();
        m.Finalize();
        Assert.Equal(0, m.ConfigDlgList.Count);
    }

    // ============================================================ GetConfigDlg

    [Fact]
    public void GetConfigDlgFindsByType()
    {
        // 152-164
        var jsy = new TJSYConfigDlg();
        var mir = new TStubGameConfigObject(TConfigDlgType.ptDefault);
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", jsy);
        m.ConfigDlgList.AddObject("", mir);

        Assert.Same(jsy, m.GetConfigDlg(TConfigDlgType.ptJSY));
        Assert.Same(mir, m.GetConfigDlg(TConfigDlgType.ptDefault));
    }

    [Fact]
    public void GetConfigDlgFallsBackToFirstWhenTypeNotFound()
    {
        // **原文反直觉之处（照抄）**：165-167 —— 找不到时**不是返回 nil**，
        // 而是返回列表第 0 项。
        var jsy = new TJSYConfigDlg();
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", jsy);

        // 列表里没有 ptDefault，但返回了第 0 项（ptJSY）
        Assert.Same(jsy, m.GetConfigDlg(TConfigDlgType.ptDefault));
        Assert.Equal(TConfigDlgType.ptJSY, m.GetConfigDlg(TConfigDlgType.ptDefault).ConfigDlgType);
    }

    [Fact]
    public void GetConfigDlgReturnsNullOnEmptyList()
    {
        var m = new TConfigDlgManage();
        Assert.Null(m.GetConfigDlg(TConfigDlgType.ptDefault));
        Assert.Null(m.GetConfigDlg(TConfigDlgType.ptJSY));
    }

    [Fact]
    public void GetConfigDlgFirstMatchWins()
    {
        var a = new TStubGameConfigObject(TConfigDlgType.ptDefault);
        var b = new TStubGameConfigObject(TConfigDlgType.ptDefault);
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", a);
        m.ConfigDlgList.AddObject("", b);
        Assert.Same(a, m.GetConfigDlg(TConfigDlgType.ptDefault));
    }

    // ============================================================ LoadControlFromStream

    [Fact]
    public void LoadControlFromStreamCallOrderAndReturnValue()
    {
        // 51-62：先 LoadCompressedUIData，再 LoadControlFromStream，再 Patch，
        //          最后 finally 释放 msDefaultUI；返回值是 LoadControlFromStream 的返回值。
        var order = new List<string>();
        object ms = new object();
        LoadControlFromStreamSeam.LoadCompressedUIData = (name, ext) =>
        {
            order.Add($"compressed:{name}:{ext}");
            return ms;
        };
        LoadControlFromStreamSeam.LoadControlFromStream = (streamUI, addrList, uiName) =>
        {
            order.Add("load");
            return 7;
        };
        LoadControlFromStreamSeam.PatchLoadControlFromStream = (memory, addrList, uiName) =>
        {
            order.Add(ReferenceEquals(memory, ms) ? "patch:same" : "patch:different");
        };
        LoadControlFromStreamSeam.FreeMemoryStream = m =>
        {
            order.Add(ReferenceEquals(m, ms) ? "free:same" : "free:different");
        };

        int result = GameConfigDlgs.LoadControlFromStream("ADDR", "STREAM", "UI");

        Assert.Equal(7, result);
        Assert.Equal(new[] { "compressed:MIR_CONFIG_DLG_UI:ZDAT", "load", "patch:same", "free:same" }, order);
    }

    [Fact]
    public void LoadControlFromStreamFreesDefaultUiEvenWhenLoadThrows()
    {
        // 原文 try..finally：Patch/装载抛异常也必须释放 msDefaultUI
        object ms = new object();
        bool freed = false;
        LoadControlFromStreamSeam.LoadCompressedUIData = (n, e) => ms;
        LoadControlFromStreamSeam.LoadControlFromStream = (s, a, u) => throw new InvalidOperationException("boom");
        LoadControlFromStreamSeam.FreeMemoryStream = m => freed = true;

        Assert.Throws<InvalidOperationException>(() => GameConfigDlgs.LoadControlFromStream("A", "S"));
        Assert.True(freed);
    }

    [Fact]
    public void LoadControlFromStreamPatchThrowsStillFrees()
    {
        object ms = new object();
        bool freed = false;
        LoadControlFromStreamSeam.LoadCompressedUIData = (n, e) => ms;
        LoadControlFromStreamSeam.LoadControlFromStream = (s, a, u) => 1;
        LoadControlFromStreamSeam.PatchLoadControlFromStream = (m, a, u) => throw new InvalidOperationException("patch");
        LoadControlFromStreamSeam.FreeMemoryStream = m => freed = true;

        Assert.Throws<InvalidOperationException>(() => GameConfigDlgs.LoadControlFromStream("A", "S"));
        Assert.True(freed);
    }

    [Fact]
    public void LoadControlFromStreamDefaultUiNameIsEmpty()
    {
        // 原文签名 `sUiName:string = ''`
        string seen = "unset";
        LoadControlFromStreamSeam.LoadCompressedUIData = (n, e) => null;
        LoadControlFromStreamSeam.LoadControlFromStream = (s, a, u) => { seen = u; return 0; };
        GameConfigDlgs.LoadControlFromStream("A", "S");
        Assert.Equal("", seen);
    }

    [Fact]
    public void LoadControlFromStreamForwardsUiNameWhenGiven()
    {
        string seen = "unset";
        LoadControlFromStreamSeam.LoadControlFromStream = (s, a, u) => { seen = u; return 0; };
        GameConfigDlgs.LoadControlFromStream("A", "S", "MyUI");
        Assert.Equal("MyUI", seen);
    }

    [Fact]
    public void LoadControlFromStreamUsesFixedCompressedUiKey()
    {
        // 原文写死 'MIR_CONFIG_DLG_UI', 'ZDAT'（注释："必须存在，否则报错"）
        string name = "", ext = "";
        LoadControlFromStreamSeam.LoadCompressedUIData = (n, e) => { name = n; ext = e; return null; };
        LoadControlFromStreamSeam.LoadControlFromStream = (s, a, u) => 0;
        GameConfigDlgs.LoadControlFromStream("A", "S");
        Assert.Equal("MIR_CONFIG_DLG_UI", name);
        Assert.Equal("ZDAT", ext);
    }

    // ============================================================ 单元级全局

    [Fact]
    public void GlobalConfigDlgManageExistsByInitialization()
    {
        // 170-173：initialization 段 ConfigDlgManage := TConfigDlgManage.Create
        Assert.NotNull(GameConfigDlgsGlobal.ConfigDlgManage);
        Assert.NotNull(GameConfigDlgsGlobal.ConfigDlgManage.ConfigDlgList);
    }

    // ============================================================ 桩类型

    [Fact]
    public void StubGameConfigObjectImplementsWholeAbstractSurface()
    {
        // 桩只用于"JSYConfigDlg 未移植时占位"，把整套抽象面都实现为默认值
        var o = new TStubGameConfigObject(TConfigDlgType.ptDefault);
        Assert.Equal(TConfigDlgType.ptDefault, o.GetType());
        Assert.Equal(TConfigDlgType.ptDefault, o.ConfigDlgType);
        Assert.False(o.Visible);
        o.Visible = true;
        Assert.True(o.Visible);
        Assert.False(o.Enabled);
        o.Enabled = true;
        Assert.True(o.Enabled);
        Assert.False(o.ProtectEnabled);
        o.ProtectEnabled = true;
        Assert.True(o.ProtectEnabled);

        Assert.False(o.GetConfigChecked(TConfigChecked.ckShowHPLabel));
        o.SetConfigChecked(TConfigChecked.ckShowHPLabel, true);
        Assert.True(o.GetConfigChecked(TConfigChecked.ckShowHPLabel));
        Assert.True(o[(TConfigChecked)0]);
        o[(TConfigChecked)0] = false;
        Assert.False(o[(TConfigChecked)0]);

        Assert.Null(o.GetShowItem("x"));
        Assert.False(o.FindShowItem("x"));
        Assert.False(o.FindHintItem("x"));
        Assert.False(o.FindPickItem("x"));
        Assert.False(o.CanFilterExp(1));
        ushort k = 0;
        Assert.False(o.FormKeyDown(ref k, DelphiShiftState.None));
        char c = 'z';
        Assert.False(o.FormKeyPress(ref c));
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 1, o.ConfigCheckeds.Length);

        // 其余 void 方法不应抛异常
        o.Open(); o.Close(); o.LoadConfig("a"); o.Finalize(); o.Logon("s"); o.Logout();
        o.RefreshMySelfAbil(); o.RefreshMyHeroAbil(); o.RefreshMySelfMagicList(); o.RefreshMyHeroMagicList();
        o.RefreshUnBindItemList(); o.RefKeyboardConfig(); o.Struck(null, 0, 0); o.HealthChange(null, 0, 0, 0);
        o.LoadClientConfig(new TClientConfig()); o.Run(); o.RefActorList();
        o.HintItem("a", 0, 0); o.ClearShowItem(); o.RefShowItem();
        o.AddToBossList("a"); o.RemoveFromBossList("a"); o.AddOrRemoveBossList("a");
        o.Initialize(IntPtr.Zero, 0, TClientVersion.cvMirs, false);
    }

    [Fact]
    public void JsyStubReportsPtJsyType()
    {
        var o = new TJSYConfigDlg();
        Assert.Equal(TConfigDlgType.ptJSY, o.GetType());
        Assert.IsAssignableFrom<TGameConfigObject>(o);
    }

    /// <summary>记录 Initialize 调用参数/次数的配置对象，用于验证管理器的分派是否正确。</summary>
    private sealed class RecordingConfigObject : TStubGameConfigObject
    {
        public RecordingConfigObject(TConfigDlgType type) : base(type) { }

        public int InitCalls;
        public IntPtr LastHandle;
        public byte LastScreenMode;
        public TClientVersion LastVersion;
        public bool LastWindowMode;

        public override void Initialize(IntPtr Handle, byte ScreenMode, TClientVersion ClientVersion, bool WindowMode)
        {
            InitCalls++;
            LastHandle = Handle;
            LastScreenMode = ScreenMode;
            LastVersion = ClientVersion;
            LastWindowMode = WindowMode;
        }
    }
}
