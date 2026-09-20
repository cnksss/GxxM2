using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms.DummySetting;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// `M2Share.pas` 假人全局/函数子集 + `ShowFrmDummySetting`（:208/:217-227）
/// + `btnDummyLogonClick`（:345-374）1:1 覆盖。
/// <para>
/// 无头 UI 处置：`ShowModal` 走 `DummySettingMessageBoxSeam`（本套件全程 `UiEnabled = false`
/// 或注入 handler）—— **绝不让真实模态循环跑起来**。
/// </para>
/// </summary>
[Collection("DummySettingSerial")]
public sealed class DummySettingGlobalsTests : DummySettingTestBase
{
    // ---------------- GetDummyNameList（M2Share.pas:16640-16658） ----------------

    [Fact]
    public void GetDummyNameList_ExactHit_ReturnsTrue()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        Assert.True(DummySettingState.GetDummyNameList("甲"));
    }

    [Fact]
    public void GetDummyNameList_CaseInsensitive_ReturnsTrue()
    {
        DummySettingState.g_DummyNameList.Add("DummyA");
        Assert.True(DummySettingState.GetDummyNameList("dummyA"));
        Assert.True(DummySettingState.GetDummyNameList("DUMMYA"));
    }

    [Fact]
    public void GetDummyNameList_Miss_ReturnsFalse()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        Assert.False(DummySettingState.GetDummyNameList("乙"));
    }

    [Fact]
    public void GetDummyNameList_EmptyList_ReturnsFalse()
    {
        Assert.False(DummySettingState.GetDummyNameList("任意"));
    }

    [Fact]
    public void GetDummyNameList_EmptyQuery_ReturnsFalseWhenNoEmptyEntry()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        Assert.False(DummySettingState.GetDummyNameList(""));
    }

    [Fact]
    public void GetDummyNameList_EmptyQuery_ReturnsTrueWhenEmptyEntryPresent()
    {
        // 无守卫：列表里若有空串，空查询命中（原文 :16649 直接 CompareText）
        DummySettingState.g_DummyNameList.Add("");
        Assert.True(DummySettingState.GetDummyNameList(""));
    }

    [Fact]
    public void GetDummyNameList_FirstHitWins_NoThrowOnDuplicates()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        DummySettingState.g_DummyNameList.Add("甲");
        Assert.True(DummySettingState.GetDummyNameList("甲"));
    }

    // ---------------- SaveDummyNameList（M2Share.pas:16660-16678） ----------------

    [Fact]
    public void SaveDummyNameList_WritesOneLinePerEntry()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        DummySettingState.g_DummyNameList.Add("乙");
        Assert.True(DummySettingState.SaveDummyNameList());

        var lines = File.ReadAllLines(Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt"),
                                      System.Text.Encoding.GetEncoding(936));
        Assert.Equal(new[] { "甲", "乙" }, lines);
    }

    [Fact]
    public void SaveDummyNameList_EmptyList_CreatesEmptyFile()
    {
        Assert.True(DummySettingState.SaveDummyNameList());
        string path = Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt");
        Assert.True(File.Exists(path));
        Assert.Equal(0, new FileInfo(path).Length);
    }

    [Fact]
    public void SaveDummyNameList_OverwritesExistingFile()
    {
        File.WriteAllText(Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt"), "OLD\r\n");
        DummySettingState.g_DummyNameList.Add("NEW");
        DummySettingState.SaveDummyNameList();
        var lines = File.ReadAllLines(Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt"),
                                      System.Text.Encoding.GetEncoding(936));
        Assert.Equal(new[] { "NEW" }, lines);
    }

    // ---------------- SaveDummyDisableMoveMap / SaveDummyNoActiveAttackMonList ----------------

    [Fact]
    public void SaveDummyDisableMoveMap_GoesThroughSeam_WithCorrectFileName()
    {
        DummySettingState.g_DummyDisableMoveMapList.Add("Z9");
        DummySettingState.SaveListToFile = (list, file) =>
            WroteFiles.Add((file, Enumerable.Range(0, list.Count).Select(i => list[i]).ToArray()));

        Assert.True(DummySettingState.SaveDummyDisableMoveMap());

        Assert.Single(WroteFiles);
        Assert.EndsWith("DummyDisableMoveMap.txt", WroteFiles[0].FileName);
        Assert.Equal(new[] { "Z9" }, WroteFiles[0].Lines);
        Assert.StartsWith(M2Config.sEnvirDir, WroteFiles[0].FileName);
    }

    [Fact]
    public void SaveDummyDisableMoveMap_DefaultSeam_WritesGbkFile()
    {
        DummySettingState.g_DummyDisableMoveMapList.Add("地图甲");
        DummySettingState.SaveDummyDisableMoveMap();

        var lines = File.ReadAllLines(Path.Combine(M2Config.sEnvirDir, "DummyDisableMoveMap.txt"),
                                      System.Text.Encoding.GetEncoding(936));
        Assert.Equal(new[] { "地图甲" }, lines);
    }

    [Fact]
    public void SaveDummyDisableMoveMap_EmptyList_WritesEmptyFile()
    {
        DummySettingState.SaveDummyDisableMoveMap();
        string path = Path.Combine(M2Config.sEnvirDir, "DummyDisableMoveMap.txt");
        Assert.True(File.Exists(path));
        Assert.Equal(0, new FileInfo(path).Length);
    }

    [Fact]
    public void SaveDummyNoActiveAttackMonList_GoesThroughSeam_WithCorrectFileName()
    {
        DummySettingState.g_DummyNoActiveAttackMonList.Add("僵尸");
        DummySettingState.SaveListToFile = (list, file) =>
            WroteFiles.Add((file, Enumerable.Range(0, list.Count).Select(i => list[i]).ToArray()));

        Assert.True(DummySettingState.SaveDummyNoActiveAttackMonList());

        Assert.Single(WroteFiles);
        Assert.EndsWith("DummyNoActiveAttackMonList.txt", WroteFiles[0].FileName);
        Assert.Equal(new[] { "僵尸" }, WroteFiles[0].Lines);
    }

    [Fact]
    public void SaveDummyNoActiveAttackMonList_DefaultSeam_WritesGbkFile()
    {
        DummySettingState.g_DummyNoActiveAttackMonList.Add("怪甲");
        DummySettingState.SaveDummyNoActiveAttackMonList();

        var lines = File.ReadAllLines(Path.Combine(M2Config.sEnvirDir, "DummyNoActiveAttackMonList.txt"),
                                      System.Text.Encoding.GetEncoding(936));
        Assert.Equal(new[] { "怪甲" }, lines);
    }

    [Fact]
    public void SaveDummyNoActiveAttackMonList_EmptyList_WritesEmptyFile()
    {
        DummySettingState.SaveDummyNoActiveAttackMonList();
        string path = Path.Combine(M2Config.sEnvirDir, "DummyNoActiveAttackMonList.txt");
        Assert.True(File.Exists(path));
        Assert.Equal(0, new FileInfo(path).Length);
    }

    [Fact]
    public void Snapshot_ReturnsCopy_NotLiveView()
    {
        DummySettingState.g_DummyNameList.Add("A");
        var snap = DummySettingState.Snapshot(DummySettingState.g_DummyNameList);
        DummySettingState.g_DummyNameList.Add("B");
        Assert.Equal(new[] { "A" }, snap);
    }

    [Fact]
    public void Snapshot_EmptyList_ReturnsEmptyArray()
    {
        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    // ---------------- ShowFrmDummySetting（:208/:217-227） ----------------

    [Fact]
    public void ShowFrmDummySetting_Headless_DoesNotRunRealModalLoop()
    {
        DummySettingMessageBoxSeam.UiEnabled = false;
        DummySettingMessageBoxSeam.ShowModalCount = 0;

        TFrmDummySetting.ShowFrmDummySetting();

        Assert.Equal(1, DummySettingMessageBoxSeam.ShowModalCount);
        Assert.Equal(TModalResult.mrCancel, DummySettingMessageBoxSeam.LastModalResult);   // 无头默认值
    }

    [Fact]
    public void ShowFrmDummySetting_UsesInjectedShowModalHandler()
    {
        DummySettingMessageBoxSeam.UiEnabled = false;
        DummySettingMessageBoxSeam.ShowModalHandler = () => TModalResult.mrOk;

        TFrmDummySetting.ShowFrmDummySetting();

        Assert.Equal(TModalResult.mrOk, DummySettingMessageBoxSeam.LastModalResult);
    }

    [Fact]
    public void ShowFrmDummySetting_UsesFormFactory_AndDisposesIt()
    {
        DummySettingMessageBoxSeam.UiEnabled = false;
        TFrmDummySetting? created = null;

        TFrmDummySetting.ShowFrmDummySetting(() =>
        {
            created = StaRunner.New(() => new TFrmDummySetting());
            return created;
        });

        Assert.NotNull(created);
        // 原文 :225 `FrmDummySetting.Free` ⇒ 托管 Dispose；已释放窗体不可再访问句柄
        Assert.True(created!.IsDisposed);
    }

    [Fact]
    public void ShowFrmDummySetting_FactoryThrows_PropagatesAndDoesNotSwallow()
    {
        DummySettingMessageBoxSeam.UiEnabled = false;
        Assert.Throws<InvalidOperationException>(() =>
            TFrmDummySetting.ShowFrmDummySetting(() => throw new InvalidOperationException("boom")));
    }

    [Fact]
    public void ShowFrmDummySetting_ModalThrows_StillDisposesForm()
    {
        // 原文 :222-226 `try/finally Free` ⇒ 模态异常也要释放
        DummySettingMessageBoxSeam.UiEnabled = false;
        DummySettingMessageBoxSeam.ShowModalHandler = () => throw new InvalidOperationException("modal");

        TFrmDummySetting? created = null;
        Assert.Throws<InvalidOperationException>(() => TFrmDummySetting.ShowFrmDummySetting(() =>
        {
            created = StaRunner.New(() => new TFrmDummySetting());
            return created;
        }));
        Assert.True(created!.IsDisposed);
    }

    [Fact]
    public void ShowFrmDummySetting_ShowModalCountIncrementsPerCall()
    {
        DummySettingMessageBoxSeam.UiEnabled = false;
        DummySettingMessageBoxSeam.ShowModalCount = 0;
        TFrmDummySetting.ShowFrmDummySetting();
        TFrmDummySetting.ShowFrmDummySetting();
        Assert.Equal(2, DummySettingMessageBoxSeam.ShowModalCount);
    }

    // ---------------- btnDummyLogonClick（:345-374） ----------------

    private void SetupLogon()
    {
        MapTable = new List<TEnvirnoment> { Env("3"), Env("0") };
        Form.FindMapHandler = n => MapTable.Find(e => e.sMapName == n);
        M2Config.sDummyHomeMap = "3";
        M2Config.nDummyHomeX = 100;
        M2Config.nDummyHomeY = 200;
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.lstDummyList.Items.AddRange(new object[] { "甲", "乙" });
    }

    [Fact]
    public void Logon_UnknownHomeMap_ShowsErrorAndProbesFocus_NoLogonQueued()
    {
        SetupLogon();
        M2Config.sDummyHomeMap = "NOPE";
        SelectDummyList(0);

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Single(Messages);
        Assert.Equal("出生地图设置错误！", Messages[0].Text);
        Assert.Equal(new[] { nameof(DummySettingControls.edtDummyHomeMap) }, FocusProbes);
        Assert.Empty(LogonsAdded);
    }

    [Fact]
    public void Logon_UnknownHomeMap_DoesNotDisableLogonButton_EarlyReturn()
    {
        // ★ 差异断言：原文 :354 `Exit` ⇒ :373 `btnDummyLogon.Enabled := False` 走不到
        SetupLogon();
        M2Config.sDummyHomeMap = "NOPE";
        Form.Ct.btnDummyLogon.Enabled = true;

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.True(Form.Ct.btnDummyLogon.Enabled);
    }

    [Fact]
    public void Logon_KnownHomeMap_QueuesSelectedNamesWithConfigCoordinates()
    {
        SetupLogon();
        SelectDummyList(0, 1);

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Equal(2, LogonsAdded.Count);
        Assert.Equal("甲", LogonsAdded[0].sCharName);
        Assert.Equal("3", LogonsAdded[0].sMapName);
        Assert.Equal(100, LogonsAdded[0].nX);
        Assert.Equal(200, LogonsAdded[0].nY);
        Assert.Equal("乙", LogonsAdded[1].sCharName);
    }

    [Fact]
    public void Logon_NoSelection_QueuesNothing_ButDisablesButton()
    {
        SetupLogon();
        SelectDummyList();

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Empty(LogonsAdded);
        Assert.Empty(Messages);
        Assert.False(Form.Ct.btnDummyLogon.Enabled);      // :373
    }

    [Fact]
    public void Logon_SkipsAlreadyExistingPlayObject()
    {
        SetupLogon();
        ExistingPlayObjects.Add("甲");
        SelectDummyList(0, 1);

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Single(LogonsAdded);
        Assert.Equal("乙", LogonsAdded[0].sCharName);
    }

    [Fact]
    public void Logon_SkipsAlreadyQueuedLogon()
    {
        SetupLogon();
        AlreadyQueuedLogons.Add("乙");
        SelectDummyList(0, 1);

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Single(LogonsAdded);
        Assert.Equal("甲", LogonsAdded[0].sCharName);
    }

    [Fact]
    public void Logon_UsesConfigNotControls_DifferenceAssertion()
    {
        // ★ 差异断言：原文 :356-358 读 `g_Config.*`，**不读**控件当前值。
        // 但 WinForms `NumericUpDown.Value = X` **会**触发已绑定的 ValueChanged
        // ⇒ 在托管侧"改控件"必然会同步 `g_Config`（这是原文行为在托管 UI 下的自然结果）。
        // 因此本用例只构造**不会**触发 ValueChanged 的差异：直接改 `edtDummyHomeMap.Text`
        // （原文 :408-411 的 `edtDummyHomeMapChange` **未被 DFM 绑定** ⇒ 改文本不写 g_Config）。
        SetupLogon();
        Form.Ct.edtDummyHomeMap.Text = "0";        // 控件改成别的图（原文也不会同步 g_Config）
        SelectDummyList(0);

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Single(LogonsAdded);
        Assert.Equal("3", LogonsAdded[0].sMapName);   // 仍是 g_Config 里的 "3"
    }

    [Fact]
    public void Logon_SpinChangeDoesSyncConfig_BecauseValueChangedIsBound()
    {
        // 对照（托管侧的自然结果）：`seDummyHomeX.Value` 改动会经 :413-417
        // 的 OnChange 立刻写回 `g_Config.nDummyHomeX` ⇒ 登录用的是新值。
        SetupLogon();
        Form.Ct.seDummyHomeX.Value = 999;
        SelectDummyList(0);

        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.Single(LogonsAdded);
        Assert.Equal(999, LogonsAdded[0].nX);
    }

    [Fact]
    public void Logon_EmptyDummyList_NoThrowAndDisablesButton()
    {
        SetupLogon();
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.btnDummyLogon.Enabled = true;

        Assert.Null(Record.Exception(() => { Form.btnDummyLogonClick(Form.Ct.btnDummyLogon); }));
        Assert.False(Form.Ct.btnDummyLogon.Enabled);
    }

    [Fact]
    public void Logon_EachQueuedLogonIsDistinctInstance_ClassSemantics()
    {
        // 托管 `TDummyLogon` 是 `class`（GXX.Core.Protocol.Grobal2.Types6.cs:131）；
        // 原文是**栈上局部记录**逐轮覆写 + AddDummyLogon 内部拷贝。
        // 1:1 后每个入队对象必须是**独立实例**（否则后写会污染先入队的那条）。
        SetupLogon();
        SelectDummyList(0, 1);
        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);

        Assert.NotSame(LogonsAdded[0], LogonsAdded[1]);
        Assert.Equal("甲", LogonsAdded[0].sCharName);   // 未被第二次覆写
    }

    [Fact]
    public void Logon_MultiThreadFlagDoesNotAffectLogonPath()
    {
        SetupLogon();
        Form.g_MultiThreadRun = true;
        SelectDummyList(0);
        Form.btnDummyLogonClick(Form.Ct.btnDummyLogon);
        Assert.Single(LogonsAdded);
    }

    [Fact]
    public void LockUnlock_IsReentrant_DoesNotThrow()
    {
        // 托管侧用 `Monitor`（可重入）；原文 `TRTLCriticalSection` 同样可重入。
        Assert.Null(Record.Exception(() =>
        {
            DummySettingState.Lock();
            try
            {
                DummySettingState.Lock();
                try
                {
                    DummySettingState.g_DummyNameList.Add("A");
                }
                finally
                {
                    DummySettingState.UnLock();
                }
            }
            finally
            {
                DummySettingState.UnLock();
            }
        }));
    }

    [Fact]
    public void LockUnlock_BalancedUnlock_AllowsSubsequentAcquire()
    {
        DummySettingState.Lock();
        DummySettingState.UnLock();
        Assert.Null(Record.Exception(() =>
        {
            DummySettingState.Lock();
            DummySettingState.UnLock();
        }));
    }
}
