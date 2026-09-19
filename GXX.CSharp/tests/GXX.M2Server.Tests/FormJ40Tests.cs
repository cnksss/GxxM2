using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J40：MonsterConfig.pas 巨片余部第一片（掉落限制页：ItemDropLimit.pas 数据层 + vstDropLimitItems/vstItemRules 树）1:1 测试。</summary>
public sealed class MonsterConfigDropLimitTests : IDisposable
{
    private readonly string _dir;
    private readonly MonsterConfigForm _form;

    public MonsterConfigDropLimitTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j40_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetMonsterConfigSliceDefaults();
        M2Config.ResetMonsterSlice2Defaults();
        DropLimitGlobals.ResetForTests();
        M2Config.sItemDropLimit = Path.Combine(_dir, "ItemDropLimit") + "\\";
        M2Config.sItemDropLogDir = Path.Combine(_dir, "ItemDropLimit", "DropLog") + "\\";
        DropLimitGlobals.g_DropLimitMgr.LoadConfig(); // 服务器启动时 LoadConfig 建 INI（Save/Remove 依赖）
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new MonsterConfigForm();
            f.MagicListHandler = () => new List<string>();
            f.MonsterListHandler = () => new List<string>();
            f.CustomMonsterListHandler = () => new List<string>();
            f.AllItemsHandler = () => new List<string> { "木剑", "金创药", "裁决之杖" };
            f.InputQueryHandler = (_, _) => false;
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        DropLimitGlobals.ResetForTests();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void Manager_AddItem_Search_Sorted()
    {
        Assert.Equal(1, DropLimitGlobals.g_nKey_DropLimitExt); // M2Share 默认 1
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        Assert.NotNull(mgr.AddItem("裁决之杖"));
        Assert.NotNull(mgr.AddItem("木剑"));
        Assert.NotNull(mgr.AddItem("金创药"));
        Assert.Equal(3, mgr.Count);
        // 二分排序表：按名有序（木剑 < 金创药 < 裁决之杖），与 FItems 添加序无关
        Assert.True(mgr.Search("裁决之杖", out int idx1));
        Assert.True(mgr.Search("金创药", out int idx2));
        Assert.True(mgr.Search("木剑", out int idx3));
        Assert.True(idx3 < idx1 && idx1 < idx2, $"排序表序应为 木剑<裁决之杖<金创药，实际 {idx3},{idx1},{idx2}");
        // 重复添加返回既有项
        var again = mgr.AddItem("木剑");
        Assert.NotNull(again);
        Assert.Equal(3, mgr.Count);
        Assert.Equal("木剑", again!.Name);
        // 未收录 → false 且 Index=插入位置
        Assert.False(mgr.Search("未知物品", out int idx4));
        Assert.True(mgr.Search("裁决之杖", out int idx5));
        Assert.True(idx4 <= idx5);
    }

    [Fact]
    public void Item_Save_Load_RoundTrip()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        mgr.LoadConfig(); // 服务器启动流程：先 LoadConfig 建 INI，再录入
        var item = mgr.AddItem("木剑")!;
        item.IsRecordLog = true;
        var fixedTime = new DateTime(2026, 9, 17, 10, 30, 15);
        DropLimitGlobals.NowFn = () => fixedTime;
        var rule = item.Add(new TDropItemRule
        {
            MapName = "3",
            ClearInterval = 7,
            DropInterval = 30,
            IntervalType = TIntervalType.itHour,
            LimitCount = 5,
            DropedCount = 2,
            AllDropedCount = 11,
            LastClearDate = fixedTime.AddDays(-1).ToOADate(),
            LastDropTime = fixedTime.AddMinutes(-90).ToOADate(),
        });
        item.Save();

        var mgr2 = new TDropLimitManager();
        mgr2.LoadConfig();
        // LoadConfig 应按节名取回
        Assert.True(mgr2.Search("木剑", out int li));
        var loaded = mgr2.GetItems(li);
        Assert.NotNull(loaded);
        Assert.True(loaded!.IsRecordLog);
        Assert.Equal(1, loaded.Count);
        var back = loaded.GetItemRule(0)!;
        Assert.Equal("3", back.MapName);
        Assert.Equal(7, back.ClearInterval);
        Assert.Equal(30, back.DropInterval);
        Assert.Equal(TIntervalType.itHour, back.IntervalType);
        Assert.Equal(5, back.LimitCount);
        Assert.Equal(2, back.DropedCount);
        Assert.Equal(11, back.AllDropedCount);
        Assert.Equal(TDropLimitItem.Date2MyDate(fixedTime.AddDays(-1).ToOADate()), TDropLimitItem.Date2MyDate(back.LastClearDate));
        Assert.Equal(TDropLimitItem.Hour2MyHour(fixedTime.AddDays(-1).ToOADate()), TDropLimitItem.Hour2MyHour(back.LastClearDate));
        Assert.Equal(30, back.DropInterval);
    }

    [Fact]
    public void Manager_CanDropItem_And_DropItem()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        var item = mgr.AddItem("裁决之杖")!;
        item.Add(new TDropItemRule { MapName = "3", LimitCount = 2 });
        item.Add(new TDropItemRule { MapName = "*", LimitCount = 5, DropedCount = 4 });
        // 通用 '*' 规则未达上限 → 可掉
        Assert.True(mgr.CanDropItem("3", "裁决之杖"));
        // 地图规则 SameText 命中且已达上限 → 不可掉
        item.GetItemRule(0)!.DropedCount = 2;
        Assert.False(mgr.CanDropItem("3", "裁决之杖"));
        // 未收录物品恒可掉
        Assert.True(mgr.CanDropItem("0", "未知物品"));

        // DropItem：达到上限 → false；间隔未到 → false
        var item2 = mgr.AddItem("金创药")!;
        var r2 = item2.Add(new TDropItemRule { MapName = "*", LimitCount = 1 });
        var fixedTime = new DateTime(2026, 9, 17, 12, 0, 0);
        DropLimitGlobals.NowFn = () => fixedTime;
        Assert.True(mgr.DropItem("0", "金创药", "鸡", "金创药(1)", 10, 20));
        Assert.Equal(1, r2.DropedCount);
        Assert.Equal(1, r2.AllDropedCount);
        Assert.False(mgr.DropItem("0", "金创药", "鸡", "金创药(2)", 10, 20)); // LimitCount<=DropedCount
        r2.LimitCount = 5;
        r2.DropInterval = 60;
        r2.DropedCount = 0;
        Assert.False(mgr.DropItem("0", "金创药", "鸡", "金创药(3)", 10, 20)); // 30 分钟 < 60 分钟
        r2.LastDropTime = fixedTime.AddHours(-2).ToOADate();
        Assert.True(mgr.DropItem("0", "金创药", "鸡", "金创药(4)", 10, 20)); // 120 分钟 >= 60 分钟
        Assert.Equal(1, r2.DropedCount);
    }

    [Fact]
    public void Item_Remove_Saves()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        var item = mgr.AddItem("木剑")!;
        var r1 = item.Add(new TDropItemRule { MapName = "3" });
        var r2 = item.Add(new TDropItemRule { MapName = "0" });
        Assert.True(item.Remove(r1));
        Assert.Equal(1, item.Count);
        Assert.False(item.Remove(r1)); // 二次删除失败
        Assert.Equal("0", item.GetItemRule(0)!.MapName);
        Assert.Same(r2, item.GetItemRule(0));
    }

    [Fact]
    public void Manager_Remove_ErasesSection()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        mgr.LoadConfig();
        var item = mgr.AddItem("木剑")!;
        item.Save();
        string iniPath = M2Config.sItemDropLimit + "DropLimitConfig.ini";
        Assert.True(File.Exists(iniPath));
        using (var check = new TFastIniFile(iniPath))
            Assert.True(check.SectionExists("木剑"));
        Assert.True(mgr.Remove(item));
        Assert.Equal(0, mgr.Count);
        using (var check = new TFastIniFile(iniPath))
            Assert.False(check.SectionExists("木剑"));
        Assert.False(mgr.Remove(item));
    }

    [Fact]
    public void Gate_KeyZero_DisablesAll()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        mgr.LoadConfig();
        var item = mgr.AddItem("木剑")!;
        item.Add(new TDropItemRule { MapName = "*", LimitCount = 0 });
        try
        {
            DropLimitGlobals.g_nKey_DropLimitExt = 0;
            Assert.Null(mgr.AddItem("新物品"));
            Assert.False(mgr.Remove(item));
            // CanDropItem 原文无门控：'*' 规则 Limit 0 <= Droped 0 → 不可掉
            Assert.False(mgr.CanDropItem("3", "木剑"));
            Assert.True(mgr.DropItem("3", "木剑", "鸡", "x", 1, 1)); // DropItem 门控关 → 恒真
            item.Save(); // 门控关 → 直接返回
            item.Load();
        }
        finally
        {
            DropLimitGlobals.g_nKey_DropLimitExt = 1;
        }
    }

    [Fact]
    public void Form_Open_BuildsDropLimitPage()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        var item = mgr.AddItem("木剑")!;
        item.Add(new TDropItemRule { MapName = "3", LimitCount = 3 });
        Assert.True(mgr.AddItem("金创药") != null);

        _form.Open(showModal: false);
        Assert.True(_form.TsDropItemVisible);
        Assert.Equal(3, _form.lstAllItems.Items.Count); // FormCreate 全物品表
        Assert.Equal(2, _form.VstDropLimitItems.Items.Count); // Open 填充物品树
        // 无当前物品：按钮/复选禁用未勾选
        Assert.False(_form.btnClearItemRule.Enabled);
        Assert.False(_form.chkRecordLog.Enabled);
        Assert.False(_form.chkRecordLog.Checked);
        Assert.Null(_form.FCurrentLimitItem);

        // 点击物品节点 → 规则树回填 + 按钮使能
        _form.VstDropLimitItems.Items[0].Selected = true;
        _form.VstDropLimitItemsNodeClick();
        Assert.NotNull(_form.FCurrentLimitItem);
        Assert.Equal("木剑", _form.FCurrentLimitItem!.Name);
        Assert.Equal(1, _form.VstItemRules.Items.Count);
        Assert.True(_form.btnClearItemRule.Enabled);
        Assert.True(_form.chkRecordLog.Enabled);
        // 11 列文本
        Assert.Equal("3", _form.VstItemRules.Items[0].Text);
        Assert.Equal("3", _form.VstItemRules.Items[0].SubItems[0].Text);
        Assert.Equal(Math.Max(3 - 0, 0).ToString(), MonsterConfigDropLimitAsserts.ColText(_form, 0, 6));
        Assert.Equal("-", MonsterConfigDropLimitAsserts.ColText(_form, 0, 7));
        Assert.Equal("-", MonsterConfigDropLimitAsserts.ColText(_form, 0, 8));
        Assert.Equal("查看", MonsterConfigDropLimitAsserts.ColText(_form, 0, 10));
    }

    [Fact]
    public void Form_ItemRule_CellText_DerivedColumns()
    {
        var rule = new TDropItemRule
        {
            MapName = "0",
            ClearInterval = 1,
            IntervalType = TIntervalType.itDay,
            DropInterval = 5,
            LimitCount = 10,
            DropedCount = 3,
            AllDropedCount = 33,
            LastClearDate = new DateTime(2026, 9, 10, 8, 30, 0).ToOADate(),
        };
        Assert.Equal("0", MonsterConfigForm.ItemRuleCellText(rule, 0));
        Assert.Equal("1", MonsterConfigForm.ItemRuleCellText(rule, 1));
        Assert.Equal("天", MonsterConfigForm.ItemRuleCellText(rule, 2));
        Assert.Equal("5", MonsterConfigForm.ItemRuleCellText(rule, 3));
        Assert.Equal("10", MonsterConfigForm.ItemRuleCellText(rule, 4));
        Assert.Equal("3", MonsterConfigForm.ItemRuleCellText(rule, 5));
        Assert.Equal("7", MonsterConfigForm.ItemRuleCellText(rule, 6)); // 剩余
        // itDay 为天数加法：2026/09/10 08:30 + 1 天 = 2026/09/11 08:30
        Assert.Equal("2026/09/11", MonsterConfigForm.ItemRuleCellText(rule, 7));
        Assert.Equal("08:30:00", MonsterConfigForm.ItemRuleCellText(rule, 8));
        Assert.Equal("33", MonsterConfigForm.ItemRuleCellText(rule, 9));
        Assert.Equal("查看", MonsterConfigForm.ItemRuleCellText(rule, 10));
        // 无清理间隔 → '-'；限制 0 → 剩余 '-'
        rule.ClearInterval = 0;
        rule.LimitCount = 0;
        Assert.Equal("-", MonsterConfigForm.ItemRuleCellText(rule, 6));
        Assert.Equal("-", MonsterConfigForm.ItemRuleCellText(rule, 7));
        Assert.Equal("-", MonsterConfigForm.ItemRuleCellText(rule, 8));
        // 剩余下限 0
        rule.LimitCount = 2;
        rule.DropedCount = 5;
        rule.ClearInterval = 1;
        Assert.Equal("0", MonsterConfigForm.ItemRuleCellText(rule, 6));
        // 时/分类型名
        rule.IntervalType = TIntervalType.itHour;
        Assert.Equal("时", MonsterConfigForm.ItemRuleCellText(rule, 2));
        rule.IntervalType = TIntervalType.itMinute;
        Assert.Equal("分", MonsterConfigForm.ItemRuleCellText(rule, 2));
    }

    [Fact]
    public void Form_LstAllItemsDblClick_Add_And_Locate()
    {
        _form.Open(showModal: false);
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        // 录入未收录物品
        _form.lstAllItems.SelectedIndex = 0; // 木剑
        _form.LstAllItemsDblClick(_form);
        Assert.NotNull(_form.FCurrentLimitItem);
        Assert.Equal("木剑", _form.FCurrentLimitItem!.Name);
        Assert.Equal(1, _form.VstDropLimitItems.Items.Count);
        Assert.True(_form.FCurrentLimitItem.IsChanged);
        Assert.True(_form.btnClearItemRule.Enabled);
        // INI 落盘（节已建）
        mgr.LoadConfig();
        Assert.True(mgr.Search("木剑", out _));
        // 再次双击 → 定位既有节点，不重复
        _form.LstAllItemsDblClick(_form);
        Assert.Equal(1, _form.VstDropLimitItems.Items.Count);
        Assert.Equal(1, mgr.Count);
        // 未选中时双击（ItemIndex<0）→ 仅清当前
        _form.lstAllItems.SelectedIndex = -1;
        _form.LstAllItemsDblClick(_form);
        Assert.Null(_form.FCurrentLimitItem);
    }

    [Fact]
    public void Form_ItemRule_Editors_And_EndEdit()
    {
        _form.Open(showModal: false);
        _form.lstAllItems.SelectedIndex = 0;
        _form.LstAllItemsDblClick(_form);
        var item = _form.FCurrentLimitItem!;
        var rule = item.Add(new TDropItemRule { MapName = "3", ClearInterval = 7, LimitCount = 5, DropedCount = 5 });
        _form.VstDropLimitItemsNodeClick();
        Assert.Equal(1, _form.VstItemRules.Items.Count);

        // 编辑列许可：0..5 与 8
        Assert.True(MonsterConfigForm.IsItemRuleColumnEditable(0));
        Assert.True(MonsterConfigForm.IsItemRuleColumnEditable(5));
        Assert.True(MonsterConfigForm.IsItemRuleColumnEditable(8));
        Assert.False(MonsterConfigForm.IsItemRuleColumnEditable(6));
        Assert.False(MonsterConfigForm.IsItemRuleColumnEditable(9));
        Assert.False(MonsterConfigForm.IsItemRuleColumnEditable(10));

        // 列 0 地图下拉：Mirror 跳过 / FB 取主名 / 去重排序 / '*' 首位
        _form.MapListHandler = () => new List<MonsterConfigForm.DropLimitMapEntry>
        {
            new() { MapName = "0", MainMapName = "0" },
            new() { MapName = "3", MainMapName = "3", FB = true },
            new() { MapName = "3b", MainMapName = "3", Mirror = true },
            new() { MapName = "0b", MainMapName = "0", FB = true },
            new() { MapName = "1", MainMapName = "1" },
        };
        var ed0 = _form.PrepareItemRuleEditor(0, rule)!;
        Assert.Equal(MonsterConfigForm.ItemRuleEditKind.MapCombo, ed0.Kind);
        Assert.Equal(new List<string> { "*", "0", "1", "3" }, ed0.Items);
        Assert.Equal("3", ed0.Text);

        // 列 1 清理间隔：Min1/Max60；列 3/4/5 Min0/Max0（原文 Max=0 形态）
        var ed1 = _form.PrepareItemRuleEditor(1, rule)!;
        Assert.Equal((1, 60, 7), (ed1.MinValue, ed1.MaxValue, ed1.Value));
        for (int col = 3; col <= 5; col++)
        {
            var ed = _form.PrepareItemRuleEditor(col, rule)!;
            Assert.Equal(0, ed.MinValue);
            Assert.Equal(0, ed.MaxValue);
        }

        // 列 2 类型下拉：天/时/分 + 当前项
        var ed2 = _form.PrepareItemRuleEditor(2, rule)!;
        Assert.Equal(new List<string> { "天", "时", "分" }, ed2.Items);
        Assert.Equal(0, ed2.ItemIndex);

        // EndEdit 列 4：Limit 写回 + DropedCount 双向钳制 + 落盘
        var ed4 = new MonsterConfigForm.ItemRuleEditor { Kind = MonsterConfigForm.ItemRuleEditKind.Spin, Value = 3 };
        _form.EndItemRuleEdit(4, rule, ed4);
        Assert.Equal(3, rule.LimitCount);
        Assert.Equal(3, rule.DropedCount); // 5 → 3 钳制
        // EndEdit 列 5：Droped 写回 + 钳制
        var ed5 = new MonsterConfigForm.ItemRuleEditor { Kind = MonsterConfigForm.ItemRuleEditKind.Spin, Value = 2 };
        _form.EndItemRuleEdit(5, rule, ed5);
        Assert.Equal(2, rule.DropedCount);
        var ed5b = new MonsterConfigForm.ItemRuleEditor { Kind = MonsterConfigForm.ItemRuleEditKind.Spin, Value = 9 };
        _form.EndItemRuleEdit(5, rule, ed5b);
        Assert.Equal(3, rule.DropedCount); // 9 → Limit 3 钳制
        // EndEdit 列 2：类型切换
        var ed2b = new MonsterConfigForm.ItemRuleEditor { Kind = MonsterConfigForm.ItemRuleEditKind.IntervalTypeCombo, ItemIndex = 1 };
        _form.EndItemRuleEdit(2, rule, ed2b);
        Assert.Equal(TIntervalType.itHour, rule.IntervalType);
        // EndEdit 列 0：SameText 变更
        var ed0b = new MonsterConfigForm.ItemRuleEditor { Kind = MonsterConfigForm.ItemRuleEditKind.MapCombo, Text = "0" };
        _form.EndItemRuleEdit(0, rule, ed0b);
        Assert.Equal("0", rule.MapName);
        // 落盘一致性
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        mgr.LoadConfig();
        Assert.True(mgr.Search("木剑", out _));
    }

    [Fact]
    public void Form_ItemRule_CtrlInsert_CtrlDelete()
    {
        _form.Open(showModal: false);
        _form.lstAllItems.SelectedIndex = 0;
        _form.LstAllItemsDblClick(_form);
        var item = _form.FCurrentLimitItem!;

        // 无当前物品时 Ctrl+Insert 早退
        _form.FCurrentLimitItem = null;
        _form.VstItemRulesKeyUp(0x2D, ctrl: true);
        Assert.Null(_form.FCurrentLimitItem);
        _form.FCurrentLimitItem = item;

        // Ctrl+Insert：默认规则 ('*',1,天,0,1,0,累计0) + 落盘 + 进入列 0 编辑
        var now = new DateTime(2026, 9, 17, 9, 0, 0);
        DropLimitGlobals.NowFn = () => now;
        _form.VstItemRulesKeyUp(0x2D, ctrl: true);
        Assert.Equal(1, item.Count);
        var rule = item.GetItemRule(0)!;
        Assert.Equal("*", rule.MapName);
        Assert.Equal(1, rule.ClearInterval);
        Assert.Equal(TIntervalType.itDay, rule.IntervalType);
        Assert.Equal(1, rule.LimitCount);
        Assert.Equal(0, rule.DropedCount);
        Assert.Equal(0, rule.AllDropedCount);
        Assert.Equal(now.ToOADate(), rule.LastClearDate, 6);
        Assert.Equal(0, rule.LastDropTime);
        Assert.Equal(1, _form.VstItemRules.Items.Count);
        Assert.Equal(0, _form.PendingItemRuleEditColumn);

        // Ctrl+Delete：删除聚焦规则 → 无邻行 → FCurrentItemRule 置空 + 二次落盘
        _form.VstItemRules.Items[0].Selected = true;
        _form.VstItemRules.FocusedItem = _form.VstItemRules.Items[0];
        _form.FCurrentItemRule = rule;
        _form.VstItemRulesKeyUp(0x2E, ctrl: true);
        Assert.Equal(0, item.Count);
        Assert.Equal(0, _form.VstItemRules.Items.Count);
        Assert.Null(_form.FCurrentItemRule);

        // 多行删除聚焦邻行
        var r1 = item.Add(new TDropItemRule { MapName = "3" });
        var r2 = item.Add(new TDropItemRule { MapName = "0" });
        _form.VstDropLimitItemsNodeClick();
        Assert.Equal(2, _form.VstItemRules.Items.Count);
        _form.VstItemRules.Items[0].Selected = true;
        _form.VstItemRules.FocusedItem = _form.VstItemRules.Items[0];
        _form.VstItemRulesKeyUp(0x2E, ctrl: true);
        Assert.Equal(1, item.Count);
        Assert.Equal("0", item.GetItemRule(0)!.MapName);
        Assert.Equal("0", _form.FCurrentItemRule!.MapName); // 邻行接替
    }

    [Fact]
    public void Form_ItemSearch_And_CtrlF()
    {
        var mgr = DropLimitGlobals.g_DropLimitMgr;
        mgr.AddItem("木剑");
        mgr.AddItem("金创药");
        _form.Open(showModal: false);
        Assert.Equal(2, _form.VstDropLimitItems.Items.Count);

        // 关键字过滤（大小写敏感子串）
        _form.edtItemSearch.Text = "木剑";
        _form.EdtItemSearchChange();
        Assert.Equal(1, _form.VstDropLimitItems.Items.Count);
        Assert.Equal("木剑", _form.VstDropLimitItems.Items[0].Text);
        // 清空恢复全部
        _form.edtItemSearch.Text = "";
        _form.EdtItemSearchChange();
        Assert.Equal(2, _form.VstDropLimitItems.Items.Count);

        // Ctrl+F 精确查找（大小写敏感等值）；未收录名 → 保持原选中
        _form.LastInputQueryText = "不存在的物品";
        bool called = false;
        _form.InputQueryHandler = (caption, prompt) =>
        {
            called = caption == "物品查找" && prompt == "输入物品名称:";
            return true;
        };
        _form.LstAllItemsKeyDown(0x46, ctrl: true);
        Assert.True(called);
        Assert.Equal(-1, _form.lstAllItems.SelectedIndex); // 未命中保持
        _form.LastInputQueryText = "金创药";
        _form.LstAllItemsKeyDown(0x46, ctrl: true);
        Assert.Equal(1, _form.lstAllItems.SelectedIndex); // 命中定位
        // 取消对话框 → 不动
        _form.InputQueryHandler = (_, _) => false;
        _form.LstAllItemsKeyDown(0x46, ctrl: true);
        Assert.Equal(1, _form.lstAllItems.SelectedIndex);
        // 非 Ctrl+F 忽略
        _form.LstAllItemsKeyDown(0x46, ctrl: false);
        Assert.Equal(1, _form.lstAllItems.SelectedIndex);
    }

    [Fact]
    public void Form_RecordLog_Clear_Delete_Popup_ViewLog()
    {
        _form.Open(showModal: false);
        _form.lstAllItems.SelectedIndex = 0;
        _form.LstAllItemsDblClick(_form);
        var item = _form.FCurrentLimitItem!;
        item.Add(new TDropItemRule { MapName = "3" });
        _form.VstDropLimitItemsNodeClick();

        // chkRecordLogClick：写回并落盘
        _form.chkRecordLog.Checked = true;
        _form.ChkRecordLogClick();
        Assert.True(item.IsRecordLog);
        // RefreshDropLimitItemButtons 回显
        _form.FCurrentLimitItem = null;
        _form.RefreshDropLimitItemButtons();
        Assert.False(_form.chkRecordLog.Checked);
        _form.FCurrentLimitItem = item;
        _form.RefreshDropLimitItemButtons();
        Assert.True(_form.chkRecordLog.Checked);

        // 规则行点击：FCurrentItemRule + 编辑许可 + 右键菜单
        _form.VstItemRules.Items[0].Selected = true;
        _form.VstItemRulesNodeClick(0);
        Assert.NotNull(_form.FCurrentItemRule);
        _form.PmItemRulesPopup();
        Assert.True(_form.MiItemRulesLogVisible);
        // 列 10 '查看' → ShowFrmItemDropLog 接缝（物品名, 地图名）
        string? logItem = null, logMap = null;
        _form.ShowFrmItemDropLogHandler = (a, b) => { logItem = a; logMap = b; };
        _form.VstItemRulesNodeClick(10);
        Assert.Equal("木剑", logItem);
        Assert.Equal("3", logMap);
        // 无当前物品时列 10 走编辑路径
        _form.FCurrentLimitItem = null;
        _form.FCurrentItemRule = null;
        _form.VstItemRulesNodeClick(10);
        Assert.NotNull(_form.FCurrentItemRule);
        Assert.Equal(10, _form.PendingItemRuleEditColumn);

        // btnAddItemRuleClick：原文空动作（含判空早退）
        _form.FCurrentLimitItem = null;
        _form.BtnAddItemRuleClick();
        _form.FCurrentLimitItem = item;
        _form.BtnAddItemRuleClick();

        // btnClearItemRuleClick：清规则树 + 数据 + 落盘
        _form.BtnClearItemRuleClick();
        Assert.Equal(0, item.Count);
        Assert.Equal(0, _form.VstItemRules.Items.Count);

        // Button1Click：删除物品节点 + 管理器移除 + 复位
        _form.VstDropLimitItems.Items[0].Selected = true;
        _form.VstDropLimitItems.FocusedItem = _form.VstDropLimitItems.Items[0];
        _form.FCurrentLimitItem = item;
        _form.BtnDelLimitItemClick();
        Assert.Equal(0, DropLimitGlobals.g_DropLimitMgr.Count);
        Assert.Equal(0, _form.VstDropLimitItems.Items.Count);
        Assert.Null(_form.FCurrentLimitItem);
        Assert.Null(_form.FCurrentItemRule);
        Assert.False(_form.btnClearItemRule.Enabled);
    }

    [Fact]
    public void Gate_HidesDropLimitTab_WhenKeyNotOne()
    {
        // FormCreate：g_nKey_DropLimitExt<>1 → 页签隐藏（仍构建其余页）
        DropLimitGlobals.g_nKey_DropLimitExt = 0;
        var f2 = StaRunner.New(() =>
        {
            var f = new MonsterConfigForm();
            f.MagicListHandler = () => new List<string>();
            f.MonsterListHandler = () => new List<string>();
            f.CustomMonsterListHandler = () => new List<string>();
            f.AllItemsHandler = () => new List<string> { "木剑" };
            return f;
        });
        try
        {
            Assert.False(f2.TsDropItemVisible);
            Assert.Equal(0, f2.lstAllItems.Items.Count); // 不填充
            f2.Open(showModal: false);
            Assert.Equal(0, f2.VstDropLimitItems.Items.Count); // Open 也不填充
        }
        finally
        {
            StaRunner.New(() => f2.Dispose());
            DropLimitGlobals.g_nKey_DropLimitExt = 1;
        }
    }
}

/// <summary>规则树派生列断言辅助（读取 ListView 子项文本）。</summary>
internal static class MonsterConfigDropLimitAsserts
{
    public static string ColText(MonsterConfigForm form, int row, int col)
        => form.VstItemRules.Items[row].SubItems[col].Text;
}
