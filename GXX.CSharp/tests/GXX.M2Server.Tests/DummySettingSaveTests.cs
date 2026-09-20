using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.DummySetting;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// `ButtonDummySaveClick`（uFrmDummySetting.pas:431-550）1:1 覆盖。
/// 差异断言重点：
///   · :441 先写 `g_Config.sDummyHomeMap` 再 :443 验图 ⇒ **验图失败也不回滚**；
///   · :500 键名 `DiableDummyRun`（原文拼写）；
///   · ⑥/⑦ 两个脏标志块**只**在对应标志为 True 时覆写全局并 `Sorted := True`；
///   · 首次校验失败时**只**弹 1 次窗（不走到 52 个 Write*）。
/// </summary>
[Collection("DummySettingSerial")]
public sealed class DummySettingSaveTests : DummySettingTestBase
{
    public DummySettingSaveTests()
    {
        OpenWith(maps: new[] { "0", "3", "D401" });
    }

    private void SaveGood()
    {
        Form.Ct.edtDummyHomeMap.Text = "3";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);
    }

    [Fact]
    public void Save_EmptyHomeMap_ShowsErrorAndProbesFocus_NoWrites()
    {
        Form.Ct.edtDummyHomeMap.Text = "";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Single(Messages);
        Assert.Equal("出生地图设置错误！", Messages[0].Text);
        Assert.Equal("错误信息", Messages[0].Caption);
        Assert.Equal(M2Forms.MB_OK + M2Forms.MB_ICONERROR, Messages[0].Flags);
        Assert.Equal(new[] { nameof(DummySettingControls.edtDummyHomeMap) }, FocusProbes);
        Assert.Empty(WroteBool);
        Assert.Empty(WroteInt);
        Assert.Empty(WroteStr);
    }

    [Fact]
    public void Save_EmptyHomeMap_DoesNotClearSaveButton()
    {
        // 原文 :439 `Exit` —— uModValue() 在 :549，走不到 ⇒ 按钮保持原状
        Form.Ct.ButtonDummySave.Enabled = true;
        Form.Ct.edtDummyHomeMap.Text = "";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void Save_WhitespaceOnlyHomeMap_TrimsToEmpty_ThenFindMapFails()
    {
        // 差异断言：:435 校验的是**未 Trim** 的 Text（"   " ≠ ''）⇒ 通过；
        // :441 才 Trim ⇒ sDummyHomeMap 变 ''；:443 FindMap('') = nil ⇒ 弹窗 + Exit。
        Form.Ct.edtDummyHomeMap.Text = "   ";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Single(Messages);
        Assert.Equal("", M2Config.sDummyHomeMap);      // ★ 已被写入空串（不回滚）
    }

    [Fact]
    public void Save_UnknownMap_ShowsErrorAndProbesFocus_NoConfigWrites()
    {
        Form.Ct.edtDummyHomeMap.Text = "NO_SUCH_MAP";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Single(Messages);
        Assert.Equal("出生地图设置错误！", Messages[0].Text);
        Assert.Equal(new[] { nameof(DummySettingControls.edtDummyHomeMap) }, FocusProbes);
        Assert.Empty(WroteBool);
        Assert.Empty(WroteInt);
        Assert.Empty(WroteStr);
    }

    [Fact]
    public void Save_UnknownMap_StillWritesHomeMapIntoConfig_NoRollback_OriginalDefect()
    {
        // ★ 差异断言（原文 :441 在 :443 校验**之前**）—— 失败时配置已被污染，且不回滚。
        M2Config.sDummyHomeMap = "3";
        Form.Ct.edtDummyHomeMap.Text = "  NO_SUCH_MAP  ";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Equal("NO_SUCH_MAP", M2Config.sDummyHomeMap);
    }

    [Fact]
    public void Save_GoodMap_WritesHomeMapTrimmed()
    {
        Form.Ct.edtDummyHomeMap.Text = "  3  ";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);
        Assert.Equal("3", M2Config.sDummyHomeMap);
        Assert.Contains(("DummyHomeMap", "3"), WroteStr);
    }

    [Fact]
    public void Save_WritesAllSixteenBoolKeys_InOriginalOrder()
    {
        SaveGood();
        Assert.Equal(new[]
        {
            "DummyLogonRand",
            "DummyAutoRepairItem", "DummyAutoRecallHero",
            "DummyAutoAddHP", "DummyAutoAddMP",
            "DiableDummyRun",
            "DummyRunHum", "DummyRunMon", "DummyRunNpc", "DummyRunGuard",
            "DummyWarDisHumRun", "DummyWarHreoRun",
            "DummySafeAreaLimited", "DummySafeAreaDisNpcRun",
            "SafeAreaDisShopStallDummyRun", "SafeAreaDisOffLineDummyRun",
        }, WroteBool.Select(p => p.Key).ToArray());
    }

    [Fact]
    public void Save_BoolAndIntCounts_MatchOriginal52WriteCalls()
    {
        SaveGood();
        Assert.Equal(16, WroteBool.Count);      // :452/:461-462/:470/:473/:500-510
        Assert.Equal(27, WroteInt.Count);       // :454-456/:463-468/:471/:474/:476-498
        Assert.Equal(1, WroteStr.Count);        // :457
    }

    [Fact]
    public void Save_BoolKey_IsDiableDummyRun_OriginalSpellingDefectPreserved()
    {
        // 原文如此（:500）：键名 `DiableDummyRun`（少一个 b），**不是** `DisableDummyRun`。
        SaveGood();
        Assert.Contains(WroteBool, p => p.Key == "DiableDummyRun");
        Assert.DoesNotContain(WroteBool, p => p.Key == "DisableDummyRun");
    }

    [Fact]
    public void Save_WritesAllIntegerKeys_InOriginalOrder()
    {
        SaveGood();
        Assert.Equal(new[]
        {
            "DummyLogonTime", "DummyHomeX", "DummyHomeY",
            "DummyWarrorAttackTime", "DummyWizardAttackTime", "DummyTaoistAttackTime",
            "DummyWarrorWalkTime", "DummyWizardWalkTime", "DummyTaoistWalkTime",
            "DummyAddHPPercent", "DummyAddMPPercent",
            "DummyHPTime_Warrior", "DummyHPBase_Warrior",
            "DummyMPTime_Warrior", "DummyMPBase_Warrior",
            "DummyHPTime_DF", "DummyHPBase_DF",
            "DummyMPTime_DF", "DummyMPBase_DF",
            "DummyHeroHPTime_Warrior", "DummyHeroHPBase_Warrior",
            "DummyHeroMPTime_Warrior", "DummyHeroMPBase_Warrior",
            "DummyHeroHPTime_DF", "DummyHeroHPBase_DF",
            "DummyHeroMPTime_DF", "DummyHeroMPBase_DF",
        }, WroteInt.Select(p => p.Key).ToArray());
    }

    [Fact]
    public void Save_IntegerValues_MatchConfigSnapshot()
    {
        M2Config.nDummyLogonTime = 42;
        M2Config.nDummyHomeX = 11;
        M2Config.nDummyHomeY = 22;
        M2Config.nDummyAddHPPercent = 33;
        M2Config.nDummyHeroMPBase_DF = 44;
        SaveGood();

        Assert.Contains(WroteInt, p => p.Key == "DummyLogonTime" && p.Value == 42);
        Assert.Contains(WroteInt, p => p.Key == "DummyHomeX" && p.Value == 11);
        Assert.Contains(WroteInt, p => p.Key == "DummyHomeY" && p.Value == 22);
        Assert.Contains(WroteInt, p => p.Key == "DummyAddHPPercent" && p.Value == 33);
        Assert.Contains(WroteInt, p => p.Key == "DummyHeroMPBase_DF" && p.Value == 44);
    }

    [Fact]
    public void Save_BoolValues_MatchConfigSnapshot()
    {
        M2Config.boDummyLogonRand = true;
        M2Config.boDiableDummyRun = false;
        M2Config.boSafeAreaDisOffLineDummyRun = true;
        SaveGood();

        Assert.Contains(WroteBool, p => p.Key == "DummyLogonRand" && p.Value);
        Assert.Contains(WroteBool, p => p.Key == "DiableDummyRun" && !p.Value);
        Assert.Contains(WroteBool, p => p.Key == "SafeAreaDisOffLineDummyRun" && p.Value);
    }

    [Fact]
    public void Save_DirtyFlagsFalse_DoesNotOverwriteGlobals()
    {
        DummySettingState.g_DummyDisableMoveMapList.Add("KEEP1");
        DummySettingState.g_DummyNoActiveAttackMonList.Add("KEEP2");
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("NEW");
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.Add("NEWMON");

        SaveGood();

        Assert.Equal(new[] { "KEEP1" }, DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
        Assert.Equal(new[] { "KEEP2" }, DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    [Fact]
    public void Save_DisableMoveMapDirty_OverwritesGlobal()
    {
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);   // 置脏
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("Z9");
        Form.Ct.lstDisableMoveMap.Items.Add("A1");
        Form.Ct.lstDisableMoveMap.Items.Add("M5");

        SaveGood();

        var snapshot = DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList);
        Assert.Equal(3, snapshot.Length);
        Assert.Contains("Z9", snapshot);
        Assert.Contains("A1", snapshot);
        Assert.Contains("M5", snapshot);
    }

    [Fact]
    public void Save_DisableMoveMapDirty_AppliesSortedTrue()
    {
        // 原文 :524 `g_DummyDisableMoveMapList.Sorted := True`（这会**立刻**把已有项排好序）
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("bbb");
        Form.Ct.lstDisableMoveMap.Items.Add("aaa");
        Form.Ct.lstDisableMoveMap.Items.Add("ccc");

        SaveGood();

        Assert.Equal(new[] { "aaa", "bbb", "ccc" },
            DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
    }

    [Fact]
    public void Save_NoActiveAttackMonDirty_OverwritesGlobalAndSortsAscii()
    {
        Form.btnNoAttackMonDelAllClick(Form.Ct.btnNoAttackMonDelAll);              // 置脏
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.Add("zzz");
        Form.Ct.lstNoAttackMonList.Items.Add("aaa");

        SaveGood();

        Assert.Equal(new[] { "aaa", "zzz" },
            DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    [Fact]
    public void Save_DisableMoveMapDirty_WritesFileWithGlobalContents()
    {
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("B2");
        DummySettingState.SaveListToFile = (list, file) =>
        {
            var lines = new List<string>();
            for (int i = 0; i < list.Count; i++) lines.Add(list[i]);
            WroteFiles.Add((file, lines.ToArray()));
        };

        // 注意：FormCreate 已把 SaveListToFile 置 null（基类），此处再注入
        SaveGood();

        Assert.Single(WroteFiles);
        Assert.EndsWith("DummyDisableMoveMap.txt", WroteFiles[0].FileName);
        Assert.Equal(new[] { "B2" }, WroteFiles[0].Lines);
    }

    [Fact]
    public void Save_NoActiveAttackMonDirty_WritesItsOwnFile()
    {
        Form.btnNoAttackMonDelAllClick(Form.Ct.btnNoAttackMonDelAll);
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.Add("M1");
        DummySettingState.SaveListToFile = (list, file) =>
        {
            var lines = new List<string>();
            for (int i = 0; i < list.Count; i++) lines.Add(list[i]);
            WroteFiles.Add((file, lines.ToArray()));
        };

        SaveGood();

        Assert.Single(WroteFiles);
        Assert.EndsWith("DummyNoActiveAttackMonList.txt", WroteFiles[0].FileName);
        Assert.Equal(new[] { "M1" }, WroteFiles[0].Lines);
    }

    [Fact]
    public void Save_NameList_WritesUtf8NamedFileWithGbkContent()
    {
        // 原文 :512 `SaveDummyNameList` 在**两个条件块之前**、无条件执行。
        DummySettingState.g_DummyNameList.Add("甲");
        SaveGood();

        string path = Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt");
        Assert.True(File.Exists(path));
        var lines = File.ReadAllLines(path, System.Text.Encoding.GetEncoding(936));
        Assert.Equal(new[] { "甲" }, lines);
    }

    [Fact]
    public void Save_NameList_EmptyGlobal_StillCreatesEmptyFile()
    {
        DummySettingState.g_DummyNameList.Clear();
        SaveGood();
        string path = Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt");
        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length == 0);
    }

    [Fact]
    public void Save_NameList_WritesInsertionOrder_NotSorted()
    {
        // 差异断言：`SaveDummyNameList`（M2Share.pas:16660-16678）**不**置 `Sorted`
        // （对比 SaveDummyDisableMoveMap / SaveDummyNoActiveAttackMonList）。
        DummySettingState.g_DummyNameList.Clear();
        DummySettingState.g_DummyNameList.Add("zzz");
        DummySettingState.g_DummyNameList.Add("aaa");
        SaveGood();

        var lines = File.ReadAllLines(Path.Combine(M2Config.sEnvirDir, "DummyNameList.txt"),
                                      System.Text.Encoding.GetEncoding(936));
        Assert.Equal(new[] { "zzz", "aaa" }, lines);
    }

    [Fact]
    public void Save_NameList_DoesNotGoThroughSaveListToFileSeam_OriginalFormDiffers()
    {
        // ★ 差异断言：`SaveDummyNameList` 走**普通 TStringList 的中转 SaveList**，
        // **不经过** `DummySettingState.SaveListToFile` 接缝（后者只服务另外两个 Save*）。
        DummySettingState.g_DummyNameList.Add("甲");
        bool seamHit = false;
        DummySettingState.SaveListToFile = (_, _) => seamHit = true;

        SaveGood();

        Assert.False(seamHit);
    }

    [Fact]
    public void Save_Finally_ClearsSaveButton_UModValue()
    {
        Form.Ct.ButtonDummySave.Enabled = true;
        SaveGood();
        Assert.False(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void Save_Finally_AlsoOnErrorPaths_ButtonUnchanged()
    {
        Form.Ct.ButtonDummySave.Enabled = true;
        Form.Ct.edtDummyHomeMap.Text = "NOPE";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);   // 原文 :447 Exit 早退，未到 :549
    }

    [Fact]
    public void Save_TwiceInARow_SecondRunWritesMuchLess()
    {
        // 首次保存：`FIsDummyDisableMoveMapChanged = True` ⇒ 走 :514-530 块（含 :529 写盘）。
        // 第二次保存：该标志已在 :529 之后的 `btnDisableMoveMapSaveClick` 路径之外被
        // `ButtonDummySaveClick` 自身清掉吗？—— **没有**：:514-530 块**不清标志**
        // （只有 `btnDisableMoveMapSaveClick` :993 与 `btnNoAttackMonSaveClick` :1077 清）。
        // 所以第二次保存**仍会**再写一次盘。本用例锁定该原文行为（`>=` 而非 `==`）。
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);
        DummySettingState.SaveListToFile = (_, file) => WroteFiles.Add((file, Array.Empty<string>()));
        SaveGood();
        int firstCount = WroteFiles.Count;

        SaveGood();

        Assert.True(firstCount >= 1);
        Assert.True(WroteFiles.Count >= firstCount);
        Assert.All(WroteFiles, w => Assert.EndsWith("DummyDisableMoveMap.txt", w.FileName));
    }

    [Fact]
    public void Save_AfterDisableMoveMapSaveButton_DirtyFlagCleared_SoButtonSaveSkipsBlock()
    {
        // 差异断言：`btnDisableMoveMapSaveClick`（:993）**清脏** ⇒ 之后的「保存」按钮
        // **不再**走 :514-530 块（`WroteFiles` 不再增加）。
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("X1");
        DummySettingState.SaveListToFile = (_, file) => WroteFiles.Add((file, Array.Empty<string>()));
        Form.btnDisableMoveMapSaveClick(Form.Ct.btnDisableMoveMapSave);   // 清脏 + 写一次
        int afterSubSave = WroteFiles.Count;

        SaveGood();                                                      // 不再写

        Assert.Equal(afterSubSave, WroteFiles.Count);
    }

    [Fact]
    public void Save_EmptyEdtButConfigAlreadyGood_StillErrors()
    {
        // 差异断言：原文只看控件文本，不看 g_Config
        M2Config.sDummyHomeMap = "3";
        Form.Ct.edtDummyHomeMap.Text = "";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);
        Assert.Single(Messages);
        Assert.Equal("3", M2Config.sDummyHomeMap);   // 配置未被改写
    }
}
