using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J66：uFrmCustomNpc.pas 1:1（1496 行）测试。
/// FormCreate/Open 装载、左栏选中 → 16 行构建与全量回填、绘制顺序六分支与移动重算、
/// 右栏列文本与编辑门控与 EndEdit 八列写回、勾选写 Enabled、
/// 批量图库/间隔、四路等差数列、保存（逐节点落盘 + 压缩块 + 移动间隔）、另存 .dat。
/// </summary>
public sealed class CustomNpcFormTests : IDisposable
{
    private readonly string _dir;

    public CustomNpcFormTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "j66_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.sEnvirDir = _dir + Path.DirectorySeparatorChar;
        M2Config.sSmartNpcDir = _dir + Path.DirectorySeparatorChar + "CustomNPC" + Path.DirectorySeparatorChar;
        M2Config.ResetCustomNpcConfigDefaults();
        CustomNpcUtils.ResetForTests();
        ViewList2State.ResetForTests();
    }

    public void Dispose()
    {
        CustomNpcUtils.ResetForTests();
        ViewList2State.ResetForTests();
        M2Config.ResetCustomNpcConfigDefaults();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { }
    }

    private CustomNpcForm NewForm(IEnumerable<string>? effectImages = null)
    {
        return StaRunner.New(() =>
        {
            var f = new CustomNpcForm();
            ViewList2State.g_EffectImageList.Clear();
            if (effectImages != null)
                ViewList2State.g_EffectImageList.AddRange(effectImages);
            f.FormCreate();
            return f;
        });
    }

    // ================= FormCreate / Open =================

    [Fact]
    public void FormCreate_FillsCombosAndDisablesRightPane()
    {
        var form = NewForm(new[] { "a.wil", "b.wil", "c.wil" });
        try
        {
            Assert.Equal(3, form.cbbNpcHPFile.Items.Count);
            Assert.Equal(3, form.cbbNpcKeepPlayFile.Items.Count);
            Assert.Equal(3, form.cbbNpcBatchFile.Items.Count);
            Assert.Equal("a.wil", form.cbbNpcHPFile.Items[0]!.ToString());

            foreach (var cbb in new[] { form.cbbNpcStandDrawMode, form.cbbNpcStandEffectDrawMode,
                form.cbbNpcActionDrawMode, form.cbbNpcActionEffectDrawMode })
            {
                Assert.Equal(2, cbb.Items.Count);
                Assert.Equal("混合", cbb.Items[0]!.ToString());
                Assert.Equal("普通", cbb.Items[1]!.ToString());
            }

            Assert.False(form.pnlNpc.Enabled);          // SetControlEnabled(pnlNpc, False)
            Assert.Null(form.FCurrentNpcCustomConfig);
            Assert.False(form.btnSaveNpc.Enabled);
            Assert.Equal(8, form.vstNpcAction.Columns.Count);
            Assert.Equal("方向", form.vstNpcAction.Columns[0]!.Text);
            Assert.Equal("特效起始图", form.vstNpcAction.Columns[7]!.Text);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Open_LoadsNpcListAndConfig()
    {
        CustomNpcUtils.CustomNpcList.Add(new TCustomNpcConfig(101));
        CustomNpcUtils.CustomNpcList.Add(new TCustomNpcConfig(202));
        M2Config.boSendCustomNpcConfig = true;
        M2Config.dwCustomNpcMoveTime = 25;

        var form = NewForm();
        try
        {
            form.Open();
            Assert.Equal(2, form.vstCustomNpc.Items.Count);
            Assert.Equal("101", form.vstCustomNpc.Items[0].Text);
            Assert.Equal("202", form.vstCustomNpc.Items[1].Text);
            Assert.True(form.chkSendCustomNPCConfig.Checked);
            Assert.Equal(25m, form.seCustomNpcMoveTime.Value);
            Assert.False(form.FIsNpcChanged);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= 左栏选中 → 16 行 =================

    [Fact]
    public void NodeClick_BuildsSixteenRowsAndBackfills()
    {
        var cfg = new TCustomNpcConfig(7);
        var b = cfg.ClientBaseConfig;
        b.HPFile = 2;
        b.HPStartIndex = 15;
        b.HPBgOffsetX = -3;
        b.HPBgOffsetY = -4;
        b.HPOffsetX = 5;
        b.HPOffsetY = 6;
        b.HPTextOffsetX = 7;
        b.HPTextOffsetY = 8;
        b.StandDrawMode = TCustomDrawMode.mdmBlend;
        b.StandEffectDrawMode = TCustomDrawMode.mdmNormal;
        b.ActionDrawMode = TCustomDrawMode.mdmBlend;
        b.ActionEffectDrawMode = TCustomDrawMode.mdmNormal;
        b.KeepPlayFile = 1;
        b.KeepPlayIndex = 33;
        b.KeepPlayCount = 4;
        b.KeepPlayTime = 60;
        b.KeepPlayBlendDraw = 0;
        b.KeepPlayOffsetX = -9;
        b.KeepPlayOffsetY = -10;
        b.DrawOrder = TCustomNpcDrawOrder.ndoEff_Chr_Keep;
        cfg.ClientBaseConfig = b;
        CustomNpcUtils.CustomNpcList.Add(cfg);

        var form = NewForm(new[] { "a.wil", "b.wil", "c.wil" });
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);

            Assert.True(form.pnlNpc.Enabled);
            Assert.Same(cfg, form.FCurrentNpcCustomConfig);
            Assert.Equal(16, form.vstNpcAction.Items.Count);

            // 列 0/1 结构与站立行勾选
            Assert.Equal("方向1", form.vstNpcAction.Items[0].SubItems[0].Text);
            Assert.Equal("站立", form.vstNpcAction.Items[0].SubItems[1].Text);
            Assert.Equal("", form.vstNpcAction.Items[1].SubItems[0].Text);
            Assert.Equal("动作", form.vstNpcAction.Items[1].SubItems[1].Text);
            Assert.Equal("方向8", form.vstNpcAction.Items[14].SubItems[0].Text);
            Assert.True(form.vstNpcAction.Items[0].Checked);      // 默认 Enabled=True
            Assert.False(form.vstNpcAction.Items[1].Checked);     // 动作行不可勾选（未设）

            // 血条/绘制模式/常驻特效回填
            Assert.Equal(2, form.cbbNpcHPFile.SelectedIndex);
            Assert.Equal(15m, form.seNpcHPStartIndex.Value);
            Assert.Equal(-3m, form.seNpcHPBgOffsetX.Value);
            Assert.Equal(-10m, form.seKeepPlayOffsetY.Value);
            Assert.Equal(0, form.cbbNpcStandDrawMode.SelectedIndex);
            Assert.Equal(1, form.cbbNpcStandEffectDrawMode.SelectedIndex);
            Assert.Equal(1, form.cbbNpcKeepPlayFile.SelectedIndex);
            Assert.Equal(33m, form.seNpcKeepPlayIndex.Value);
            Assert.False(form.chkNpcKeepPlayBlendDraw.Checked);

            // 绘制顺序：ndoEff_Chr_Keep → 特效/角色/持久
            Assert.Equal(3, form.lstNpcDrawOrder.Items.Count);
            Assert.Equal("特效播放", form.lstNpcDrawOrder.Items[0]!.ToString());
            Assert.Equal("角色绘制", form.lstNpcDrawOrder.Items[1]!.ToString());
            Assert.Equal("持久播放", form.lstNpcDrawOrder.Items[2]!.ToString());
            Assert.Equal(2, form.DrawOrderObjectAt(0));
            Assert.Equal(1, form.DrawOrderObjectAt(1));

            Assert.False(form.btnSaveNpc.Enabled);   // 未变更 → FIsNpcChanged 还原
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void NodeClick_NoSelectionClearsRightPane()
    {
        CustomNpcUtils.CustomNpcList.Add(new TCustomNpcConfig(1));
        var form = NewForm();
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);
            Assert.Equal(16, form.vstNpcAction.Items.Count);

            form.SelectCustomNpcRow(-1);
            Assert.Equal(0, form.vstNpcAction.Items.Count);
            Assert.Null(form.FCurrentNpcCustomConfig);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void DrawOrderToSequence_AllSixBranches()
    {
        Assert.Equal(new[] { 0, 1, 2 }, CustomNpcForm.DrawOrderToSequence(TCustomNpcDrawOrder.ndoKeep_Chr_Eff));
        Assert.Equal(new[] { 0, 2, 1 }, CustomNpcForm.DrawOrderToSequence(TCustomNpcDrawOrder.ndoKeep_Eff_Chr));
        Assert.Equal(new[] { 1, 0, 2 }, CustomNpcForm.DrawOrderToSequence(TCustomNpcDrawOrder.ndoChr_Keep_Eff));
        Assert.Equal(new[] { 1, 2, 0 }, CustomNpcForm.DrawOrderToSequence(TCustomNpcDrawOrder.ndoChr_Eff_Keep));
        Assert.Equal(new[] { 2, 0, 1 }, CustomNpcForm.DrawOrderToSequence(TCustomNpcDrawOrder.ndoEff_Keep_Chr));
        Assert.Equal(new[] { 2, 1, 0 }, CustomNpcForm.DrawOrderToSequence(TCustomNpcDrawOrder.ndoEff_Chr_Keep));
    }

    [Fact]
    public void SequenceToDrawOrder_AllSixBranches()
    {
        Assert.Equal(TCustomNpcDrawOrder.ndoKeep_Chr_Eff, CustomNpcForm.SequenceToDrawOrder(0, 1));
        Assert.Equal(TCustomNpcDrawOrder.ndoKeep_Eff_Chr, CustomNpcForm.SequenceToDrawOrder(0, 2));
        Assert.Equal(TCustomNpcDrawOrder.ndoChr_Keep_Eff, CustomNpcForm.SequenceToDrawOrder(1, 0));
        Assert.Equal(TCustomNpcDrawOrder.ndoChr_Eff_Keep, CustomNpcForm.SequenceToDrawOrder(1, 2));
        Assert.Equal(TCustomNpcDrawOrder.ndoEff_Keep_Chr, CustomNpcForm.SequenceToDrawOrder(2, 0));
        Assert.Equal(TCustomNpcDrawOrder.ndoEff_Chr_Keep, CustomNpcForm.SequenceToDrawOrder(2, 1));
    }

    [Fact]
    public void MoveDrawOrder_RecalculatesDrawOrder()
    {
        var cfg = new TCustomNpcConfig(3);
        var b = cfg.ClientBaseConfig;
        b.DrawOrder = TCustomNpcDrawOrder.ndoKeep_Chr_Eff;   // 持久/角色/特效
        cfg.ClientBaseConfig = b;
        CustomNpcUtils.CustomNpcList.Add(cfg);

        var form = NewForm();
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);

            // 下移首项：持久→位置 1 → 顺序 角色/持久/特效 → ndoChr_Keep_Eff
            form.SelectDrawOrderRow(0);
            form.BtnNpcMoveClick(up: false);
            Assert.Equal("角色绘制", form.lstNpcDrawOrder.Items[0]!.ToString());
            Assert.Equal("持久播放", form.lstNpcDrawOrder.Items[1]!.ToString());
            Assert.Equal(TCustomNpcDrawOrder.ndoChr_Keep_Eff, cfg.ClientBaseConfig.DrawOrder);   // DIAG
            Assert.True(cfg.IsChanged);
            Assert.True(form.btnSaveNpc.Enabled);

            // 上移回去 → 持久/角色/特效 → ndoKeep_Chr_Eff
            form.BtnNpcMoveClick(up: true);
            Assert.Equal("持久播放", form.lstNpcDrawOrder.Items[0]!.ToString());
            Assert.Equal(TCustomNpcDrawOrder.ndoKeep_Chr_Eff, cfg.ClientBaseConfig.DrawOrder);

            // 边界：首项上移 / 末项下移 均早退
            form.SelectDrawOrderRow(0);
            form.BtnNpcMoveClick(up: true);
            Assert.Equal("持久播放", form.lstNpcDrawOrder.Items[0]!.ToString());
            form.SelectDrawOrderRow(2);
            form.BtnNpcMoveClick(up: false);
            Assert.Equal("特效播放", form.lstNpcDrawOrder.Items[2]!.ToString());
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= 右栏文本 / 编辑 =================

    [Fact]
    public void ActionCellText_AllColumnsBothActionTypes()
    {
        var cfg = new TCustomNpcConfig(9);
        var a = cfg.DirActions[2];
        a.Std_File = 1; a.Std_Index = 10; a.Std_Count = 3; a.Std_Time = 200;
        a.Std_EffFile = 2; a.Std_EffIndex = 20;
        a.Act_File = 0; a.Act_Index = 30; a.Act_Count = 5; a.Act_Time = 150;
        a.Act_EffFile = 1; a.Act_EffIndex = 40;
        cfg.DirActions[2] = a;

        var stand = new CustomNpcForm.TNpcNodeData { Owner = cfg, DirIndex = 2, ActionType = TNpcActionType.atStand };
        var act = new CustomNpcForm.TNpcNodeData { Owner = cfg, DirIndex = 2, ActionType = TNpcActionType.atAction };

        var form = NewForm(new[] { "f0.wil", "f1.wil", "f2.wil" });
        try
        {
            Assert.Equal("方向3", CustomNpcForm.ActionCellText(stand, 0));
            Assert.Equal("站立", CustomNpcForm.ActionCellText(stand, 1));
            Assert.Equal("f1.wil", CustomNpcForm.ActionCellText(stand, 2));
            Assert.Equal("10", CustomNpcForm.ActionCellText(stand, 3));
            Assert.Equal("3", CustomNpcForm.ActionCellText(stand, 4));
            Assert.Equal("200", CustomNpcForm.ActionCellText(stand, 5));
            Assert.Equal("f2.wil", CustomNpcForm.ActionCellText(stand, 6));
            Assert.Equal("20", CustomNpcForm.ActionCellText(stand, 7));

            Assert.Equal("", CustomNpcForm.ActionCellText(act, 0));
            Assert.Equal("动作", CustomNpcForm.ActionCellText(act, 1));
            Assert.Equal("f0.wil", CustomNpcForm.ActionCellText(act, 2));
            Assert.Equal("30", CustomNpcForm.ActionCellText(act, 3));
            Assert.Equal("5", CustomNpcForm.ActionCellText(act, 4));
            Assert.Equal("150", CustomNpcForm.ActionCellText(act, 5));
            Assert.Equal("f1.wil", CustomNpcForm.ActionCellText(act, 6));
            Assert.Equal("40", CustomNpcForm.ActionCellText(act, 7));

            // 越界图库下标 → 空串
            var b = cfg.DirActions[2];
            b.Std_File = 99;
            cfg.DirActions[2] = b;
            Assert.Equal("", CustomNpcForm.ActionCellText(stand, 2));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void ActionGate_HintAndBackColor()
    {
        CustomNpcUtils.CustomNpcList.Add(new TCustomNpcConfig(1));
        var form = NewForm();
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);
            Assert.Equal(16, form.vstNpcAction.Items.Count);

            Assert.False(form.ActionEditingAllowed(0, 0));
            Assert.False(form.ActionEditingAllowed(0, 1));
            Assert.True(form.ActionEditingAllowed(0, 2));
            Assert.True(form.ActionEditingAllowed(0, 7));
            Assert.False(form.ActionEditingAllowed(-1, 2));
            Assert.False(form.ActionEditingAllowed(9999, 2));

            Assert.False(form.ActionPrepareEdit(0));
            Assert.False(form.ActionPrepareEdit(1));
            Assert.True(form.ActionPrepareEdit(2));
            Assert.True(form.ActionPrepareEdit(7));
            Assert.False(form.ActionPrepareEdit(8));

            Assert.Equal("", CustomNpcForm.ActionHint(6));
            Assert.Equal("特效开始图片为-1表示不使用特效", CustomNpcForm.ActionHint(7));

            Assert.Equal(System.Drawing.Color.FromArgb(0xD8, 0xE4, 0xF2),
                CustomNpcForm.ActionCellBackColor(TNpcActionType.atStand, 3));
            Assert.Null(CustomNpcForm.ActionCellBackColor(TNpcActionType.atAction, 3));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void ActionEndEdit_WritesBackBothActionTypes()
    {
        var cfg = new TCustomNpcConfig(11);
        CustomNpcUtils.CustomNpcList.Add(cfg);
        var form = NewForm(new[] { "f0.wil", "f1.wil" });
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);

            // 站立行 = 下标 0（方向 0）；列 4 张数
            Assert.True(form.ActionEndEdit(0, 4, 0, 6));
            Assert.Equal(6, cfg.DirActions[0].Std_Count);
            Assert.True(cfg.IsChanged);

            Assert.True(form.ActionEndEdit(0, 3, 0, 12));
            Assert.Equal(12, cfg.DirActions[0].Std_Index);
            Assert.True(form.ActionEndEdit(0, 5, 0, 180));
            Assert.Equal(180, cfg.DirActions[0].Std_Time);
            Assert.True(form.ActionEndEdit(0, 7, 0, 99));
            Assert.Equal(99, cfg.DirActions[0].Std_EffIndex);
            Assert.True(form.ActionEndEdit(0, 2, 1, 0));
            Assert.Equal(1, cfg.DirActions[0].Std_File);
            Assert.Equal("f1.wil", form.vstNpcAction.Items[0].SubItems[2].Text);

            // 动作行 = 下标 1
            Assert.True(form.ActionEndEdit(1, 4, 0, 9));
            Assert.Equal(9, cfg.DirActions[0].Act_Count);
            Assert.True(form.ActionEndEdit(1, 3, 0, 20));
            Assert.Equal(20, cfg.DirActions[0].Act_Index);
            Assert.True(form.ActionEndEdit(1, 2, 1, 0));   // comboIndex = 1（0 为默认值，取 1 才能验证写回）
            Assert.Equal(1, cfg.DirActions[0].Act_File);

            // 同值与越界
            Assert.False(form.ActionEndEdit(0, 4, 0, 6));
            Assert.False(form.ActionEndEdit(9999, 4, 0, 1));
            Assert.False(form.ActionEndEdit(0, 0, 0, 1));

            // 方向 3 站立行 = 下标 6
            Assert.True(form.ActionEndEdit(6, 4, 0, 4));
            Assert.Equal(4, cfg.DirActions[3].Std_Count);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void ActionChecked_WritesEnabled()
    {
        var cfg = new TCustomNpcConfig(13);
        CustomNpcUtils.CustomNpcList.Add(cfg);
        var form = NewForm();
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);

            Assert.Equal(1, cfg.DirActions[0].Enabled);
            form.vstNpcAction.Items[0].Checked = false;
            form.VstNpcActionChecked(0);
            Assert.Equal(0, cfg.DirActions[0].Enabled);
            Assert.True(cfg.IsChanged);

            form.vstNpcAction.Items[0].Checked = true;
            form.VstNpcActionChecked(0);
            Assert.Equal(1, cfg.DirActions[0].Enabled);

            // 方向 1 站立行 = 下标 2
            form.vstNpcAction.Items[2].Checked = false;
            form.VstNpcActionChecked(2);
            Assert.Equal(0, cfg.DirActions[1].Enabled);

            // 越界
            form.VstNpcActionChecked(99);
            Assert.True(cfg.IsChanged);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void ActionNodeClick_RecordsEditingRequest()
    {
        var cfg = new TCustomNpcConfig(15);
        CustomNpcUtils.CustomNpcList.Add(cfg);
        var form = NewForm();
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);

            form.VstNpcActionNodeClick(3, 4);
            Assert.NotNull(form.LastEditingRequest);
            Assert.Equal((3, 4), form.LastEditingRequest!.Value);

            form.LastEditingRequest = null;
            form.VstNpcActionNodeClick(3, 1);   // 列 1 不进入编辑
            Assert.Null(form.LastEditingRequest);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= 批量 / 计算 =================

    [Fact]
    public void BatchFileAndTime_FillAllEightDirections()
    {
        var cfg = new TCustomNpcConfig(21);
        CustomNpcUtils.CustomNpcList.Add(cfg);
        var form = NewForm(new[] { "f0.wil", "f1.wil" });
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);
            form.cbbNpcBatchFile.SelectedIndex = 1;
            form.seNpcBatchTime.Value = 333;

            form.BtnNpcFileClick("stand");
            for (int i = 0; i < 8; i++)
                Assert.Equal(1, cfg.DirActions[i].Std_File);
            Assert.True(cfg.IsChanged);

            form.BtnNpcFileClick("standEffect");
            form.BtnNpcFileClick("action");
            form.BtnNpcFileClick("actionEffect");
            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(1, cfg.DirActions[i].Std_EffFile);
                Assert.Equal(1, cfg.DirActions[i].Act_File);
                Assert.Equal(1, cfg.DirActions[i].Act_EffFile);
            }

            form.BtnNpcTimeClick("stand");
            for (int i = 0; i < 8; i++)
                Assert.Equal(333, cfg.DirActions[i].Std_Time);
            form.BtnNpcTimeClick("action");
            for (int i = 0; i < 8; i++)
                Assert.Equal(333, cfg.DirActions[i].Act_Time);

            // 未选中配置 → 早退
            form.FCurrentNpcCustomConfig = null;
            form.BtnNpcFileClick("stand");
            form.BtnNpcTimeClick("stand");
            form.BtnNpcCalcClick("stand");
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Calc_FourPathsArithmeticSeries()
    {
        var cfg = new TCustomNpcConfig(31);
        CustomNpcUtils.CustomNpcList.Add(cfg);
        var form = NewForm();
        try
        {
            form.Open();
            form.SelectCustomNpcRow(0);

            form.seNpcCalcStartIndex.Value = 100;
            form.seNpcCalcPlayCount.Value = 4;
            form.seNpcCalcEmptyCount.Value = 2;
            form.seNpcCalcDirCount.Value = 8;

            // 站立：Index = 100 + 6*(i-1)，Count = 4
            form.BtnNpcCalcClick("stand");
            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(100 + 6 * i, cfg.DirActions[i].Std_Index);
                Assert.Equal(4, cfg.DirActions[i].Std_Count);
            }

            // 动作：同上写 Act_*
            form.BtnNpcCalcClick("action");
            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(100 + 6 * i, cfg.DirActions[i].Act_Index);
                Assert.Equal(4, cfg.DirActions[i].Act_Count);
            }

            // 两特效路只写 Index（张数槽位原文注释掉）
            form.BtnNpcCalcClick("standEffect");
            form.BtnNpcCalcClick("actionEffect");
            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(100 + 6 * i, cfg.DirActions[i].Std_EffIndex);
                Assert.Equal(100 + 6 * i, cfg.DirActions[i].Act_EffIndex);
            }

            // DirCount = 3 → 只写前 3 个方向
            form.seNpcCalcStartIndex.Value = 0;
            form.seNpcCalcPlayCount.Value = 2;
            form.seNpcCalcEmptyCount.Value = 0;
            form.seNpcCalcDirCount.Value = 3;
            form.BtnNpcCalcClick("stand");
            Assert.Equal(0, cfg.DirActions[0].Std_Index);
            Assert.Equal(2, cfg.DirActions[1].Std_Index);
            Assert.Equal(4, cfg.DirActions[2].Std_Index);
            Assert.Equal(100 + 6 * 3, cfg.DirActions[3].Std_Index);   // 未被覆盖

            Assert.True(cfg.IsChanged);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= 保存 =================

    [Fact]
    public void SaveNpc_SavesChangedNodesAndRebuildsText()
    {
        var changed = new TCustomNpcConfig(41);
        changed.SetChanged();
        var untouched = new TCustomNpcConfig(42);
        CustomNpcUtils.CustomNpcList.Add(changed);
        CustomNpcUtils.CustomNpcList.Add(untouched);

        var form = NewForm();
        var ints = new List<(string, int)>();
        form.WriteIntegerHandler = (k, v) => ints.Add((k, v));
        form.CompressHandler = data => data;   // 恒等（便于断言原始长度）
        try
        {
            form.Open();
            form.seCustomNpcMoveTime.Value = 42;
            form.FIsNpcChanged = true;
            form.btnSaveNpc.Enabled = true;

            form.BtnSaveNpcClick();

            Assert.True(File.Exists(changed.IniPath));            // 已变更节点落盘
            Assert.False(changed.IsChanged);
            Assert.False(File.Exists(untouched.IniPath));          // 未变更节点不落盘
            Assert.False(form.FIsNpcChanged);
            Assert.False(form.btnSaveNpc.Enabled);
            Assert.Equal(42, M2Config.dwCustomNpcMoveTime);
            Assert.Equal(("CustomNpcMoveTime", 42), ints[0]);

            // 压缩块：2 条 × 290
            Assert.Equal(2 * CustomNpcUtils.ClientRecordSize, form.CustomNpcListTextLen);
            Assert.Equal(form.CustomNpcListTextLen, form.CustomNpcListText.Length);
            Assert.NotEqual(0u, form.CustomNpcListTextCrc);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void RebuildCustomNpcListText_EnabledFirstPerConfig()
    {
        var cfg = new TCustomNpcConfig(51);
        var a0 = cfg.DirActions[0];
        a0.Enabled = 0;
        a0.Std_File = 900;
        cfg.DirActions[0] = a0;
        var a1 = cfg.DirActions[1];
        a1.Std_File = 901;
        cfg.DirActions[1] = a1;
        CustomNpcUtils.CustomNpcList.Add(cfg);

        var form = NewForm();
        try
        {
            form.RebuildCustomNpcListText();
            byte[] data = form.CustomNpcListText;

            Assert.Equal(CustomNpcUtils.ClientRecordSize, data.Length);
            Assert.Equal(7, BitConverter.ToUInt16(data, 2));     // wDirCount = 7（启用数）

            int actionsOffset = 4 + CustomNpcUtils.NpcBaseConfigSize;
            int actionSize = CustomNpcUtils.NpcDirActionSize;
            // 首位为启用的方向 1（Std_File 901）
            Assert.Equal(901, BitConverter.ToUInt16(data, actionsOffset + 4));
            // 末位为未启用的方向 0（Std_File 900）
            Assert.Equal(900, BitConverter.ToUInt16(data, actionsOffset + 7 * actionSize + 4));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void SaveNpcToFile_WritesDatAndRemembersPath()
    {
        CustomNpcUtils.CustomNpcList.Add(new TCustomNpcConfig(61));

        var form = NewForm();
        var strings = new List<(string, string)>();
        var messages = new List<string>();
        form.WriteStringHandler = (k, v) => strings.Add((k, v));
        form.ShowMessageHandler = m => messages.Add(m);
        try
        {
            string target = Path.Combine(_dir, "npc_out.txt");
            form.SaveFileDialogHandler = _ => target;
            form.BtnSaveNpcToFileClick();

            string expected = Path.ChangeExtension(target, ".dat");
            Assert.True(File.Exists(expected));
            Assert.Equal(expected, M2Config.sCustomNpcClientConfigFileName);
            Assert.Equal(("CustomNpcClientConfigFileName", expected), strings[0]);
            Assert.Equal("已经生成自定义NPC登录器配置文件", messages[0]);

            // 取消对话框 → 不写盘不改配置
            M2Config.sCustomNpcClientConfigFileName = "";
            strings.Clear();
            form.SaveFileDialogHandler = _ => null;
            form.BtnSaveNpcToFileClick();
            Assert.Equal("", M2Config.sCustomNpcClientConfigFileName);
            Assert.Empty(strings);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void SendConfigToggle_WritesBool()
    {
        var form = NewForm();
        var bools = new List<(string, bool)>();
        form.WriteBoolHandler = (k, v) => bools.Add((k, v));
        try
        {
            form.chkSendCustomNPCConfig.Checked = true;
            Assert.True(M2Config.boSendCustomNpcConfig);
            Assert.Equal(("SendCustomNpcConfig", true), bools[0]);

            form.chkSendCustomNPCConfig.Checked = false;
            Assert.False(M2Config.boSendCustomNpcConfig);
            Assert.Equal(2, bools.Count);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void SetNpcConfigChanged_NoSelectionIsNoop()
    {
        var form = NewForm();
        try
        {
            form.FCurrentNpcCustomConfig = null;
            form.SetNpcConfigChanged();
            Assert.False(form.FIsNpcChanged);
            Assert.False(form.btnSaveNpc.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}
