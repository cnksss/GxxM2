using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.LogDataServer;
using Xunit;

namespace GXX.LogDataServer.Tests;

/// <summary>LogManage.pas TFrmLogManage 1:1 测试。</summary>
public sealed class LogManageTests : IDisposable
{
    private readonly string _dir;

    public LogManageTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_lane6_logm_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        LogDataShare.ResetForTests();
        LogDataShare.sBaseDir = _dir;
        LogDataForms.MessageBoxHandler = (_, _, _) => LogDataForms.IDOK;
        LogDataForms.LastMessage = null;
        LogDataForms.LastCaption = null;
        LogDataForms.NextAnswer = null;
        TFrmLogManage.SaveFileProvider = null;
        TFrmLogManage.ClipboardHandler = null;
    }

    public void Dispose()
    {
        LogDataForms.MessageBoxHandler = null;
        LogDataForms.LastMessage = null;
        TFrmLogManage.SaveFileProvider = null;
        TFrmLogManage.ClipboardHandler = null;
        LogDataShare.ResetForTests();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private TFrmLogManage NewForm() => StaRunner.New(() =>
    {
        var f = new TFrmLogManage();
        f.Timer.Enabled = false;   // FormCreate 会打开计时器，测试内关掉
        return f;
    });

    // ------------------------------------------------------------------
    // 常量表 / 静态函数
    // ------------------------------------------------------------------

    /// <summary>LogManage.pas:150 ActionNames 表（42 项，顺序与取值 1:1）。</summary>
    [Fact]
    public void ActionNames_MatchDelphiTableExactly()
    {
        var expected = new (int Action, string Text)[]
        {            (01, "炼制物品"), (02, "制造物品"), (03, "NPC给予"), (04, "NPC回收"), (05, "捡取物品"),
            (06, "丢弃物品"), (07, "挖到物品"), (08, "掉落物品"), (09, "物品消失"), (10, "卖出物品"),
            (11, "买入物品"), (22, "卖出费用"), (23, "买入费用"), (12, "镶嵌物品"), (13, "拆开镶嵌"),
            (14, "物品升级"), (15, "交易物品"), (16, "挑战物品"), (17, "放入物品"), (18, "取回物品"),
            (19, "移动物品"), (20, "拆分物品"), (21, "叠加物品"), (24, "物品更新"), (25, "穿戴装备"),
            (26, "脱下装备"), (27, "命令爆出"),
            (60, "城堡存钱"), (61, "城堡取钱"), (50, "金币改变"), (51, "元宝改变"), (52, "游戏点改变"),
            (53, "金刚石改变"), (54, "灵符改变"), (55, "声望改变"), (56, "荣誉值改变"), (57, "等级改变"),
            (58, "属性点改变"),
            (70, "人物死亡"), (71, "人物上线"), (72, "人物下线"), (73, "角色交易"),
        };
        Assert.Equal(42, TFrmLogManage.ActionNames.Length);
        Assert.Equal(expected, TFrmLogManage.ActionNames);
    }

    [Fact]
    public void LogActionConstants_MatchDelphi()
    {
        Assert.Equal(0, TFrmLogManage.LOG_ActionNone);
        Assert.Equal(27, TFrmLogManage.LOG_ItemNpcMonDrop);
        Assert.Equal(50, TFrmLogManage.LOG_GoldChange);
        Assert.Equal(61, TFrmLogManage.LOG_CastleGetMoney);
        Assert.Equal(73, TFrmLogManage.LOG_PlayerTrading);
    }

    /// <summary>GetActString 只取 LoWord→LoByte（高 16 位与高字节被忽略）。</summary>
    [Theory]
    [InlineData(1u, "炼制物品")]
    [InlineData(27u, "命令爆出")]
    [InlineData(50u, "金币改变")]
    [InlineData(73u, "角色交易")]
    [InlineData(257u, "炼制物品")]        // LoWord=257, LoByte=1
    [InlineData(0x00010001u, "炼制物品")] // 高 16 位被丢掉
    [InlineData(0x1234u, "游戏点改变")]   // LoByte(0x1234) = 0x34 = 52
    public void GetActString_MapsByLoByte(uint nAct, string expected)
        => Assert.Equal(expected, TFrmLogManage.GetActString(nAct));

    [Theory]
    [InlineData(0u)]
    [InlineData(28u)]
    [InlineData(49u)]
    [InlineData(59u)]
    [InlineData(60000u)]
    public void GetActString_UnknownAction_ReturnsFailsafe(uint nAct)
        => Assert.Equal("无法分析", TFrmLogManage.GetActString(nAct));

    [Theory]
    [InlineData(@"C:\a\b", "b")]
    [InlineData(@"C:\a\b\", "b")]
    [InlineData("abc", "")]
    [InlineData(@"C:\", "")]
    [InlineData("", "")]
    [InlineData(@"\x", "x")]
    public void LastDirectoryName_MatchesDelphi(string input, string expected)
        => Assert.Equal(expected, TFrmLogManage.LastDirectoryName(input));

    // ------------------------------------------------------------------
    // DoSearchFile
    // ------------------------------------------------------------------

    [Fact]
    public void DoSearchFile_OnlyLogPrefixNlfFiles_SkipsDirectories()
    {
        StaRunner.New(() =>
        {
            string day = Path.Combine(_dir, "2024-01-01");
            Directory.CreateDirectory(day);
            File.WriteAllText(Path.Combine(day, "log-1.nlf"), "a");
            File.WriteAllText(Path.Combine(day, "LOG-2.NLF"), "b");      // SameText 大小写不敏感
            File.WriteAllText(Path.Combine(day, "other.nlf"), "c");     // 前缀不符
            File.WriteAllText(Path.Combine(day, "log-x.txt"), "d");     // 后缀不符
            Directory.CreateDirectory(Path.Combine(day, "log-dir.nlf")); // 目录 → 跳过

            using var f = NewForm();
            var list = new TStringList();
            f.DoSearchFile(day, list);

            var names = new List<string>();
            foreach (string s in list.AsEnumerable()) names.Add(Path.GetFileName(s));
            names.Sort(StringComparer.OrdinalIgnoreCase);
            Assert.Equal(new[] { "log-1.nlf", "LOG-2.NLF" }, names);
        });
    }

    [Fact]
    public void DoSearchFile_MissingDirectory_ReturnsEmpty()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            var list = new TStringList();
            f.DoSearchFile(Path.Combine(_dir, "nope"), list);
            Assert.Equal(0, list.Count);
        });
    }

    [Fact]
    public void DoSearchFile_EmptyDirectory_ReturnsEmpty()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            var list = new TStringList();
            f.DoSearchFile(_dir, list);
            Assert.Equal(0, list.Count);
        });
    }

    /// <summary>路径末尾反斜杠被规范化（IncludeTrailingBackslash）。</summary>
    [Fact]
    public void DoSearchFile_AppendsTrailingBackslashOnce()
    {
        StaRunner.New(() =>
        {
            File.WriteAllText(Path.Combine(_dir, "log-9.nlf"), "x");
            using var f = NewForm();
            var list = new TStringList();
            f.DoSearchFile(_dir + "\\", list);
            Assert.Equal(1, list.Count);
            Assert.Equal(Path.Combine(_dir, "log-9.nlf"), list[0]);
        });
    }

    // ------------------------------------------------------------------
    // FormCreate / 动作树
    // ------------------------------------------------------------------

    [Fact]
    public void FormCreate_BuildsActionTree_AndComboAndTimer()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmLogManage();
            try
            {
                // 4 个顶层节点：查询所有 / 物品相关 / 普通数据 / 其他动作
                Assert.Equal(4, f.vstLogType.RootNodeCount);
                var root0 = (TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[0].Data!;
                Assert.Equal(TFrmLogManage.TActionCategory.acAll, root0.Category);
                Assert.Equal("查询所有", root0.Text);
                Assert.Equal(26, f.vstLogType.Roots[1].Children.Count);
                Assert.Equal(12, f.vstLogType.Roots[2].Children.Count);
                Assert.Equal(4, f.vstLogType.Roots[3].Children.Count);
                Assert.Equal("物品相关", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[1].Data!).Text);
                Assert.Equal("普通数据", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[2].Data!).Text);
                Assert.Equal("其他动作", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[3].Data!).Text);

                var child0 = (TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[1].Children[0].Data!;
                Assert.Equal(TFrmLogManage.TActionCategory.acSpecify, child0.Category);
                Assert.Equal(1, child0.Action);
                Assert.Equal("炼制物品", child0.Text);
                Assert.Equal(TCheckType.ctTriStateCheckBox, f.vstLogType.Roots[0].CheckType);
                Assert.Equal(TCheckState.csCheckedNormal, f.vstLogType.Roots[0].CheckState);

                // cbbActionType ← LogActorTypeNames（7 项）
                Assert.Equal(7, f.cbbActionType.Items.Count);
                Assert.Equal("-", f.cbbActionType.Items[0]);
                Assert.Equal("人物", f.cbbActionType.Items[1]);
                Assert.Equal("怪物", f.cbbActionType.Items[6]);
                Assert.Equal(0, f.cbbActionType.SelectedIndex);

                Assert.True(f.Timer.Enabled);   // FormCreate 打开
                Assert.Equal(100, f.Timer.Interval);
                Assert.Equal(DateTime.Today, f.DateTimeEditBegin.Value.Date);
                Assert.Equal(DateTime.Today, f.DateTimeEditEnd.Value.Date);
                Assert.NotNull(TSearchManagerHost.g_SearchManager);
            }
            finally
            {
                f.Timer.Enabled = false;
            }
        });
    }

    [StaFact]
    public void DateTimeEditHandlers_KeepEndNotBeforeBegin()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.DateTimeEditBegin.Value = new DateTime(2024, 5, 1);
                f.DateTimeEditEnd.Value = new DateTime(2024, 5, 10);
                Assert.Equal(new DateTime(2024, 5, 1), f.DateTimeEditBegin.Value.Date);
                Assert.Equal(new DateTime(2024, 5, 10), f.DateTimeEditEnd.Value.Date);

                // Begin 晚于 End → End 被抬到 Begin（VCL 的 OnDateTimeChange 由控件赋值触发）
                f.DateTimeEditBegin.Value = new DateTime(2024, 6, 1);
                Assert.Equal(new DateTime(2024, 6, 1), f.DateTimeEditEnd.Value.Date);

                // Begin 早于 End → End 不变
                f.DateTimeEditBegin.Value = new DateTime(2024, 4, 1);
                Assert.Equal(new DateTime(2024, 6, 1), f.DateTimeEditEnd.Value.Date);

                // End 早于 Begin → End 被设为 DateTimeEditBegin.Date
                f.DateTimeEditEnd.Value = new DateTime(2024, 3, 1);
                Assert.Equal(new DateTime(2024, 4, 1), f.DateTimeEditEnd.Value.Date);
                Assert.Equal(new DateTime(2024, 4, 1), f.DateTimeEditBegin.Value.Date);

                // 直接调用处理器且参数不早于 Begin → 不变
                f.DateTimeEditEndDateTimeChange(f, new DateTime(2024, 7, 1));
                Assert.Equal(new DateTime(2024, 4, 1), f.DateTimeEditEnd.Value.Date);

                f.DateTimeEditBeginDateTimeChange(f, new DateTime(2024, 4, 1));
                Assert.Equal(new DateTime(2024, 4, 1), f.DateTimeEditEnd.Value.Date);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    // ------------------------------------------------------------------
    // btnStartClick 校验分支
    // ------------------------------------------------------------------

    public static TheoryData<string, string> ValidationCases => new()
    {
        { "ObjName", "请输入查询的人物名称 ！！！" },
        { "ActObjName", "请输入查询的交易对象 ！！！" },
        { "ItemName", "请输入查询的物品名称 ！！！" },
        { "ItemID", "请输入查询的物品ID ！！！" },
    };

    [StaTheory]
    [MemberData(nameof(ValidationCases))]
    public void btnStartClick_EmptyRequiredField_ShowsMessageAndAborts(string which, string expectedMessage)
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                switch (which)
                {
                    case "ObjName": f.chkObjName.Checked = true; f.edtObjName.Text = "  "; break;
                    case "ActObjName": f.chkActObjName.Checked = true; f.edtActObjName.Text = ""; break;
                    case "ItemName": f.chkItemName.Checked = true; f.edtItemName.Text = ""; break;
                    case "ItemID": f.chkItemID.Checked = true; f.edtItemID.Text = ""; break;
                }

                f.btnStartClick(f);

                Assert.Equal(expectedMessage, LogDataForms.LastMessage);
                Assert.Equal("提示信息", LogDataForms.LastCaption);
                Assert.Null(f.btnStart.Tag);   // Tag 未被置 1
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Theory]
    [InlineData("0", "物品ID必须是正整数 ！！！")]
    [InlineData("-3", "物品ID必须是正整数 ！！！")]
    [InlineData("abc", "物品ID必须是正整数 ！！！")]
    public void btnStartClick_NonPositiveItemId_ShowsMessage(string text, string expectedMessage)
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.chkItemID.Checked = true;
                f.edtItemID.Text = text;
                f.btnStartClick(f);
                Assert.Equal(expectedMessage, LogDataForms.LastMessage);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void btnStartClick_NoFiles_SetsStatusAndResets()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.btnStartClick(f);

                Assert.Equal("", f.StatusPanel(3).Text);
                Assert.Equal("查询已完成", f.StatusPanel(2).Text);
                Assert.Equal(0, f.btnStart.Tag);
                Assert.Equal("开始查询", f.btnStart.Text);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void btnStartClick_StopMode_CancelsTasksAndResetsCaption()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.btnStart.Tag = 1;
                f.btnStart.Text = "停止查询";
                TSearchManagerHost.g_SearchManager.Tasks.Add(new TSearchTask());

                f.btnStartClick(f);

                Assert.Equal(0, f.btnStart.Tag);
                Assert.Equal("开始查询", f.btnStart.Text);
                Assert.Empty(TSearchManagerHost.g_SearchManager.Tasks);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Theory]
    [InlineData(false, false, false, false, false, 0)]
    [InlineData(true, false, false, false, false, 1)]
    [InlineData(true, true, true, true, true, 31)]
    [InlineData(false, true, false, false, false, 2)]
    [InlineData(false, false, true, false, false, 4)]
    [InlineData(false, false, false, true, false, 8)]
    [InlineData(false, false, false, false, true, 16)]
    public void btnStartClick_ComputesSearchWhereBitmask(bool objName, bool objType, bool actObjName,
        bool itemName, bool itemId, int expected)
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.chkObjName.Checked = objName;
                f.chkObjType.Checked = objType;
                f.chkActObjName.Checked = actObjName;
                f.chkItemName.Checked = itemName;
                f.chkItemID.Checked = itemId;
                if (objName) f.edtObjName.Text = "n";
                if (actObjName) f.edtActObjName.Text = "a";
                if (itemName) f.edtItemName.Text = "i";
                if (itemId) f.edtItemID.Text = "5";

                f.btnStartClick(f);
                Assert.Equal(expected, TSearchManagerHost.g_SearchManager.SearchWhere);

                // 复原 Tag 以便 finally 清理
                if (Equals(f.btnStart.Tag, 1)) f.btnStartClick(f);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>找到日志文件 → 按文件建 TSearchTask（TaskID/FileName/ShowPanel）。</summary>
    [Fact]
    public void btnStartClick_FindsFilesAndAddsOneTaskPerFile()
    {
        StaRunner.New(() =>
        {
            var day = DateTime.Today;
            string dirName = day.Year + "-" + LogDataShare.IntToString(day.Month) + "-" + LogDataShare.IntToString(day.Day);
            string dayDir = Path.Combine(_dir, dirName);
            Directory.CreateDirectory(dayDir);
            File.WriteAllText(Path.Combine(dayDir, "log-a.nlf"), "x");
            File.WriteAllText(Path.Combine(dayDir, "log-b.nlf"), "y");

            using var f = NewForm();
            try
            {
                f.chkObjName.Checked = true;
                f.edtObjName.Text = "hero";
                f.btnStartClick(f);

                Assert.Equal(1, f.btnStart.Tag);
                Assert.Equal("停止查询", f.btnStart.Text);
                Assert.Equal(2, TSearchManagerHost.g_SearchManager.Tasks.Count);
                Assert.Equal(0, ((TSearchTask)TSearchManagerHost.g_SearchManager.Tasks[0]).TaskID);
                Assert.Equal(1, ((TSearchTask)TSearchManagerHost.g_SearchManager.Tasks[1]).TaskID);
                Assert.EndsWith("log-a.nlf", ((TSearchTask)TSearchManagerHost.g_SearchManager.Tasks[0]).FileName, StringComparison.Ordinal);
                Assert.Equal("hero", TSearchManagerHost.g_SearchManager.SearchObjName);
                Assert.NotNull(((TSearchTask)TSearchManagerHost.g_SearchManager.Tasks[0]).ShowPanel);
                Assert.Same(f.StatusPanel(2), ((TSearchTask)TSearchManagerHost.g_SearchManager.Tasks[0]).ShowPanel);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>顶层"查询所有"勾选 → 256 个 SearchActions 全 True。</summary>
    [Fact]
    public void btnStartClick_AllNodeChecked_SetsAllSearchActions()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.vstLogType.Roots[0].CheckState = TCheckState.csCheckedNormal;
                f.btnStartClick(f);
                Assert.All(TSearchManagerHost.g_SearchManager.SearchActions, b => Assert.True(b));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>
    /// 顶层未勾选 → 先全 False，再只打开 acSpecify 且勾选的节点。
    /// ★ 原文分组错位：第一组循环 0..25（26 项），第二组从 26 起，
    ///   于是 ActionNames[26]（"命令爆出"=27）被挂到"普通数据"父节点下 —— 1:1 保留。
    /// </summary>
    [Fact]
    public void btnStartClick_SelectiveNodes_SetOnlyCheckedSpecifyActions()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.vstLogType.Roots[0].CheckState = TCheckState.csUncheckedNormal;
                foreach (var n in f.vstLogType.AllNodes()) n.CheckState = TCheckState.csUncheckedNormal;

                var item = f.vstLogType.Roots[1].Children[0];   // ActionNames[0] → Action = 1
                item.CheckState = TCheckState.csCheckedNormal;
                var data = f.vstLogType.Roots[2].Children[0];   // ActionNames[26] → Action = 27（分组错位）
                data.CheckState = TCheckState.csCheckedNormal;
                Assert.Equal(27, ((TFrmLogManage.TLogTypeNodeData)data.Data!).Action);

                f.btnStartClick(f);

                var trues = new List<int>();
                for (int i = 0; i < TSearchManagerHost.g_SearchManager.SearchActions.Length; i++)
                    if (TSearchManagerHost.g_SearchManager.SearchActions[i]) trues.Add(i);
                string diag = "true actions: " + string.Join(",", trues);

                Assert.True(TSearchManagerHost.g_SearchManager.SearchActions[1], diag);
                Assert.True(TSearchManagerHost.g_SearchManager.SearchActions[27], diag);
                Assert.False(TSearchManagerHost.g_SearchManager.SearchActions[2], diag);
                Assert.False(TSearchManagerHost.g_SearchManager.SearchActions[60], diag);
                Assert.Equal(2, CountTrue(TSearchManagerHost.g_SearchManager.SearchActions));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>★ 分组边界断言：ActionNames[26] = ('命令爆出') 落在"普通数据"组。</summary>
    [Fact]
    public void FormCreate_GroupBoundary_IsOffByOneInSource()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                // 第一组循环 0..25 → "物品相关" 只到 ActionNames[25]（脱下装备）
                Assert.Equal("脱下装备", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[1].Children[25].Data!).Text);
                // ActionNames[26]（命令爆出）被排到"普通数据"组首位
                Assert.Equal("命令爆出", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[2].Children[0].Data!).Text);
                Assert.Equal("城堡存钱", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[2].Children[1].Data!).Text);
                Assert.Equal("人物死亡", ((TFrmLogManage.TLogTypeNodeData)f.vstLogType.Roots[3].Children[0].Data!).Text);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    private static int CountTrue(bool[] a)
    {
        int n = 0;
        foreach (bool b in a) if (b) n++;
        return n;
    }

    // ------------------------------------------------------------------
    // 排序 / 完成回调
    // ------------------------------------------------------------------

    private static TLogData Log(int idx, uint act = 1, string map = "m", int x = 1, int y = 2, string obj = "o",
        TLogActorType type = TLogActorType.latHuman, string item = "i", int itemIdx = 3, string actObj = "ao",
        int d1 = 4, int d2 = 5, string desc = "d", DateTime? date = null)
        => new()
        {
            nIndx = idx,
            nAct = act,
            sMapName = map,
            nX = x,
            nY = y,
            sObjectName = obj,
            ObjectType = type,
            sItemName = item,
            nItemIndex = itemIdx,
            sActObjectName = actObj,
            nData1 = d1,
            nData2 = d2,
            LogDesc = desc,
            Date = date ?? new DateTime(2024, 1, 2, 3, 4, 5),
        };

    [Fact]
    public void QuickSortLogData_SortsByIndexAscending()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var list = new List<object> { Log(5), Log(1), Log(4), Log(0), Log(3), Log(2) };
                f.QuickSortLogData(list, 0, list.Count - 1);
                for (int i = 0; i < list.Count; i++)
                    Assert.Equal(i, ((TLogData)list[i]).nIndx);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void QuickSortLogData_SingleElement_IsNoOp()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var list = new List<object> { Log(9) };
                f.QuickSortLogData(list, 0, 0);
                Assert.Equal(9, ((TLogData)list[0]).nIndx);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void OnTaskComplete_WhenAllDone_SortsRenumbersAndFillsGrid()
    {
        StaRunner.New(() =>
        {
            // 造 1 个日志文件 → btnStartClick 令 FTaskCount = 1，并接通 SearchDataList
            var day = DateTime.Today;
            string dirName = day.Year + "-" + LogDataShare.IntToString(day.Month) + "-" + LogDataShare.IntToString(day.Day);
            string dayDir = Path.Combine(_dir, dirName);
            Directory.CreateDirectory(dayDir);
            File.WriteAllText(Path.Combine(dayDir, "log-a.nlf"), "x");

            using var f = NewForm();
            try
            {
                f.btnStartClick(f);
                Assert.Equal(1, TSearchManagerHost.g_SearchManager.Tasks.Count);

                var dataList = TSearchManagerHost.g_SearchManager.SearchDataList!;
                var list = dataList.LockList();
                list.Add(Log(7));
                list.Add(Log(3));
                dataList.UnlockList();

                f.OnTaskComplete(new TPoolTask());

                Assert.Equal(2, f.vstLog.RootNodeCount);
                Assert.Equal(0, ((TLogData)f.vstLog.Roots[0].Data!).nIndx);
                Assert.Equal(1, ((TLogData)f.vstLog.Roots[1].Data!).nIndx);
                Assert.Equal("查询已完成", f.StatusPanel(2).Text);
                Assert.Equal("", f.StatusPanel(3).Text);
                Assert.Equal(0, f.btnStart.Tag);
                Assert.Equal("开始查询", f.btnStart.Text);
                Assert.Equal(0, f.vstLog.UpdateDepth);   // Begin/EndUpdate 配对

                // 再完成一次 → FTaskCount 变 -1，不再刷新（原文无保护）
                f.OnTaskComplete(new TPoolTask());
                Assert.Equal(2, f.vstLog.RootNodeCount);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void OnTaskComplete_WhenNotLastTask_OnlyDecrements()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                // FTaskCount 由 btnStartClick 设定；未开始时为 0 → Dec 后 = -1（原文无保护）
                f.OnTaskComplete(new TPoolTask());
                Assert.Equal(0, f.vstLog.RootNodeCount);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void ClearLogDataList_EmptiesTheSharedList()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.btnStartClick(f);   // 接通 SearchDataList → FLogDataList
                var shared = TSearchManagerHost.g_SearchManager.SearchDataList!;
                var list = shared.LockList();
                list.Add(Log(1));
                shared.UnlockList();

                f.ClearLogDataList();

                var after = shared.LockList();
                Assert.Empty(after);
                shared.UnlockList();
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    // ------------------------------------------------------------------
    // vstLog 列 / 比较 / 排序
    // ------------------------------------------------------------------

    [Fact]
    public void LogColumnTitles_MatchDfm()
    {
        Assert.Equal(new[] { "序号", "动作", "地图", "坐标X", "坐标Y", "角色名称", "角色类型", "物品名称",
            "物品ID", "目标对象", "新数据", "参考数据", "描述", "时间" }, TFrmLogManage.LogColumnTitles);
        Assert.Equal(new[] { 0, 90, 60, 46, 46, 90, 65, 90, 80, 120, 80, 69, 168, 130 }, TFrmLogManage.LogColumnWidths);
    }

    [Fact]
    public void vstLogGetText_MapsAllFourteenColumns()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var node = f.vstLog.AddChild(null);
                node.Data = Log(11, act: 1, map: "0", x: 100, y: 200, obj: "hero",
                    type: TLogActorType.latMonster, item: "剑", itemIdx: 42, actObj: "other",
                    d1: 7, d2: 8, desc: "desc", date: new DateTime(2024, 3, 4, 5, 6, 7));

                Assert.Equal("11", Cell(f, node, 0));
                Assert.Equal("炼制物品", Cell(f, node, 1));
                Assert.Equal("0", Cell(f, node, 2));
                Assert.Equal("100", Cell(f, node, 3));
                Assert.Equal("200", Cell(f, node, 4));
                Assert.Equal("hero", Cell(f, node, 5));
                Assert.Equal("怪物", Cell(f, node, 6));
                Assert.Equal("剑", Cell(f, node, 7));
                Assert.Equal("42", Cell(f, node, 8));
                Assert.Equal("other", Cell(f, node, 9));
                Assert.Equal("7", Cell(f, node, 10));
                Assert.Equal("8", Cell(f, node, 11));
                Assert.Equal("desc", Cell(f, node, 12));
                Assert.Equal("2024-03-04 05:06:07", Cell(f, node, 13));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    private static string Cell(TFrmLogManage f, TVirtualNode node, int col)
    {
        string s = "";
        f.vstLogGetText(f.vstLog, node, col, ref s);
        return s;
    }

    [Fact]
    public void vstLogGetText_UnknownAct_UsesFailsafeText()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var node = f.vstLog.AddChild(null);
                node.Data = Log(1, act: 999);
                Assert.Equal("无法分析", Cell(f, node, 1));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void vstLogGetText_NullData_LeavesCell()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var node = f.vstLog.AddChild(null);
                string s = "keep";
                f.vstLogGetText(f.vstLog, node, 0, ref s);
                Assert.Equal("keep", s);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Theory]
    [InlineData(0, 1, 2, -1)]
    [InlineData(1, 1, 2, -1)]
    [InlineData(3, 1, 2, -1)]
    [InlineData(8, 1, 2, -1)]
    [InlineData(10, 4, 5, -1)]
    [InlineData(11, 4, 5, -1)]
    public void vstLogCompareNodes_NumericColumns(int column, int v1, int v2, int sign)
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var n1 = f.vstLog.AddChild(null);
                var n2 = f.vstLog.AddChild(null);
                n1.Data = Log(v1, act: (uint)v1, x: v1, y: v1, itemIdx: v1, d1: v1, d2: v1);
                n2.Data = Log(v2, act: (uint)v2, x: v2, y: v2, itemIdx: v2, d1: v2, d2: v2);

                int r = 0;
                f.vstLogCompareNodes(f.vstLog, n1, n2, column, ref r);
                Assert.Equal(sign, Math.Sign(r));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void vstLogCompareNodes_StringAndDateColumns()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var a = f.vstLog.AddChild(null);
                var b = f.vstLog.AddChild(null);
                a.Data = Log(1, map: "aaa", obj: "aaa", item: "aaa", actObj: "aaa", desc: "aaa",
                    date: new DateTime(2024, 1, 1));
                b.Data = Log(2, map: "bbb", obj: "bbb", item: "bbb", actObj: "bbb", desc: "bbb",
                    date: new DateTime(2024, 1, 3));

                foreach (int col in new[] { 2, 5, 7, 9, 12, 13 })
                {
                    int r = 0;
                    f.vstLogCompareNodes(f.vstLog, a, b, col, ref r);
                    Assert.True(r < 0, $"column {col} 应为负");
                }

                int t = 0;
                f.vstLogCompareNodes(f.vstLog, a, b, 6, ref t);
                Assert.Equal(0, t);   // 同 ObjectType
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>列 0 → NoColumn + 升序；同列再点切换升降。</summary>
    [Fact]
    public void vstLogHeaderClick_TogglesSortColumnAndDirection()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var sorts = new List<string>();
                f.vstLog.Header.SortTreeHandler = (col, dir, rec) => sorts.Add($"{col}:{dir}:{rec}");

                f.vstLogHeaderClick(f.vstLog.Header, new TVTHeaderHitInfo { Column = 2, Button = TMouseButton.mbLeft });
                Assert.Equal(2, f.vstLog.Header.SortColumn);
                Assert.Equal(TSortDirection.sdAscending, f.vstLog.Header.SortDirection);
                Assert.Equal("2:sdAscending:False", sorts[^1]);

                f.vstLogHeaderClick(f.vstLog.Header, new TVTHeaderHitInfo { Column = 2, Button = TMouseButton.mbLeft });
                Assert.Equal(TSortDirection.sdDescending, f.vstLog.Header.SortDirection);
                Assert.Equal("2:sdDescending:False", sorts[^1]);

                f.vstLogHeaderClick(f.vstLog.Header, new TVTHeaderHitInfo { Column = 2, Button = TMouseButton.mbLeft });
                Assert.Equal(TSortDirection.sdAscending, f.vstLog.Header.SortDirection);

                // 列 0 → NoColumn，且强制升序
                f.vstLogHeaderClick(f.vstLog.Header, new TVTHeaderHitInfo { Column = 0, Button = TMouseButton.mbLeft });
                Assert.Equal(TVTHeader.NoColumn, f.vstLog.Header.SortColumn);
                Assert.Equal("0:sdAscending:False", sorts[^1]);

                // 非左键 → 什么都不做
                int before = sorts.Count;
                f.vstLogHeaderClick(f.vstLog.Header, new TVTHeaderHitInfo { Column = 5, Button = TMouseButton.mbRight });
                Assert.Equal(before, sorts.Count);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>Handler 接缝真的会按 vstLogCompareNodes 重排根节点。</summary>
    [Fact]
    public void SortLogTree_OrdersRowsByColumn()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.vstLog.AddChild(null).Data = Log(1, desc: "c");
                f.vstLog.AddChild(null).Data = Log(2, desc: "a");
                f.vstLog.AddChild(null).Data = Log(3, desc: "b");

                f.vstLog.Header.SortTree(12, TSortDirection.sdAscending, false);
                Assert.Equal("a", ((TLogData)f.vstLog.Roots[0].Data!).LogDesc);
                Assert.Equal("b", ((TLogData)f.vstLog.Roots[1].Data!).LogDesc);
                Assert.Equal("c", ((TLogData)f.vstLog.Roots[2].Data!).LogDesc);

                f.vstLog.Header.SortTree(12, TSortDirection.sdDescending, false);
                Assert.Equal("c", ((TLogData)f.vstLog.Roots[0].Data!).LogDesc);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    // ------------------------------------------------------------------
    // 选中行 / 剪贴板 / 导出
    // ------------------------------------------------------------------

    [Fact]
    public void pmiCopyClick_CopiesFocusedCellText()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string copied = "";
                TFrmLogManage.ClipboardHandler = s => copied = s;
                var node = f.vstLog.AddChild(null);
                node.Data = Log(9);
                f.vstLog.FocusedNode = node;
                f.vstLog.FocusedColumn = 0;

                f.pmiCopyClick(f);

                Assert.Equal("9", copied);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    /// <summary>复制选中行：14 列 Tab 分隔 + sLineBreak，末尾被 Copy(S,1,Length-2) 去掉 CRLF。</summary>
    [Fact]
    public void pmiCopyLineClick_BuildsTabSeparatedBlock_WithoutTrailingCrLf()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string copied = "";
                TFrmLogManage.ClipboardHandler = s => copied = s;
                var n1 = f.vstLog.AddChild(null);
                n1.Data = Log(1, desc: "d1", date: new DateTime(2024, 1, 2, 3, 4, 5));
                n1.Selected = true;
                var n2 = f.vstLog.AddChild(null);
                n2.Data = Log(2, desc: "d2", date: new DateTime(2024, 1, 2, 3, 4, 6));
                n2.Selected = true;

                f.pmiCopyLineClick(f);

                string expected = "1\t炼制物品\tm\t1\t2\to\t人物\ti\t3\tao\t4\t5\td1\t2024-01-02 03:04:05\r\n" +
                                  "2\t炼制物品\tm\t1\t2\to\t人物\ti\t3\tao\t4\t5\td2\t2024-01-02 03:04:06\r\n";
                expected = DelphiRTL.Copy(expected, 1, expected.Length - 2);
                Assert.Equal(expected, copied);
                Assert.False(copied.EndsWith("\r\n", StringComparison.Ordinal));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void pmiCopyLineClick_NoSelection_DoesNotTouchClipboard()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                int calls = 0;
                TFrmLogManage.ClipboardHandler = _ => calls++;
                f.pmiCopyLineClick(f);
                Assert.Equal(0, calls);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void pmiExportLineClick_WritesSelectedRowsToTxt()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string target = Path.Combine(_dir, "out.bin");
                TFrmLogManage.SaveFileProvider = () => target;
                var n1 = f.vstLog.AddChild(null);
                n1.Data = Log(1, desc: "one", date: new DateTime(2024, 1, 2, 3, 4, 5));
                n1.Selected = true;

                f.pmiExportLineClick(f);

                string file = Path.ChangeExtension(target, ".txt");
                Assert.True(File.Exists(file));
                Assert.Equal("1\t炼制物品\tm\t1\t2\to\t人物\ti\t3\tao\t4\t5\tone\t2024-01-02 03:04:05\r\n",
                    File.ReadAllText(file, GXX.Core.EncodingInit.GBK));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void pmiExportAllClick_WritesAllRows()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string target = Path.Combine(_dir, "all.bin");
                TFrmLogManage.SaveFileProvider = () => target;
                f.vstLog.AddChild(null).Data = Log(1, desc: "one");
                f.vstLog.AddChild(null).Data = Log(2, desc: "two");

                f.pmiExportAllClick(f);

                string file = Path.ChangeExtension(target, ".txt");
                string text = File.ReadAllText(file, GXX.Core.EncodingInit.GBK);
                Assert.Contains("one", text, StringComparison.Ordinal);
                Assert.Contains("two", text, StringComparison.Ordinal);
                Assert.Equal(2, text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Length);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void pmiExportLineClick_NoSelection_NoFile()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                int calls = 0;
                TFrmLogManage.SaveFileProvider = () => { calls++; return null; };
                f.pmiExportLineClick(f);
                Assert.Equal(0, calls);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void pmiExportAllClick_SaveCancelled_NoFile()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                TFrmLogManage.SaveFileProvider = () => null;
                f.vstLog.AddChild(null).Data = Log(1);
                f.pmiExportAllClick(f);   // 不应抛异常、不应写文件
                Assert.Empty(Directory.GetFiles(_dir, "*.txt"));
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void PopupMenuPopup_TogglesItemsAndCaption()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                // 无行 → 两项都禁用
                f.PopupMenuPopup(f);
                Assert.False(f.pmiCopy.Enabled);
                Assert.False(f.pmiCopyLine.Enabled);

                var node = f.vstLog.AddChild(null);
                node.Data = Log(3);
                f.vstLog.FocusedNode = node;
                f.vstLog.FocusedColumn = 0;
                f.PopupMenuPopup(f);

                Assert.True(f.pmiCopy.Enabled);
                Assert.False(f.pmiCopyLine.Enabled);
                Assert.Equal("复制 \"3\"", f.pmiCopy.Text);

                node.Selected = true;
                f.PopupMenuPopup(f);
                Assert.True(f.pmiCopyLine.Enabled);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void vstLogKeyAction_CtrlC_CopiesFocusedCell()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string copied = "";
                TFrmLogManage.ClipboardHandler = s => copied = s;
                var node = f.vstLog.AddChild(null);
                node.Data = Log(5);
                f.vstLog.FocusedNode = node;
                f.vstLog.FocusedColumn = 0;

                bool doDefault = true;
                f.vstLogKeyAction(f.vstLog, (ushort)'C', TShiftState.ssCtrl, ref doDefault);
                Assert.Equal("5", copied);

                copied = "";
                f.vstLogKeyAction(f.vstLog, (ushort)'C', TShiftState.ssNone, ref doDefault);
                Assert.Equal("", copied);   // 非 Ctrl 不复制
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    // ------------------------------------------------------------------
    // 绘制 / 擦除 / 折叠 / 计时器 / 系统消息
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void vstLogBeforeItemErase_Zebra(int index, bool expectColor)
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var node = new TVirtualNode { Index = index };
                int color = 0;
                var action = TItemEraseAction.eaDefault;
                f.vstLogBeforeItemErase(f.vstLog, node, ref color, ref action);
                Assert.Equal(expectColor ? 0x00FFFBF7 : 0, color);
                Assert.Equal(expectColor ? TItemEraseAction.eaColor : TItemEraseAction.eaDefault, action);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void vstLogDrawText_BoldsFocusedSelectedCell()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var node = f.vstLog.AddChild(null);
                node.Data = Log(1);
                node.Selected = true;
                f.vstLog.FocusedNode = node;
                f.vstLog.FocusedColumn = 3;

                var color = new[] { System.Drawing.Color.Black };
                var bold = new[] { false };
                bool def = true;
                f.vstLogDrawText(f.vstLog, color, bold, node, 3, ref def);
                Assert.Equal(System.Drawing.Color.Yellow, color[0]);
                Assert.True(bold[0]);

                color[0] = System.Drawing.Color.Black; bold[0] = false;
                f.vstLogDrawText(f.vstLog, color, bold, node, 4, ref def);
                Assert.Equal(System.Drawing.Color.Black, color[0]);
                Assert.False(bold[0]);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void vstLogTypeGetText_And_DrawText_And_Collapsing()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string s = "";
                f.vstLogTypeGetText(f.vstLogType, f.vstLogType.Roots[1], 0, ref s);
                Assert.Equal("物品相关", s);

                var color = new[] { System.Drawing.Color.Black };
                var bold = new[] { false };
                f.vstLogTypeDrawText(f.vstLogType, f.vstLogType.Roots[0], color, bold);
                Assert.True(bold[0]);
                Assert.Equal(System.Drawing.Color.Blue, color[0]);

                color[0] = System.Drawing.Color.Black; bold[0] = false;
                f.vstLogTypeDrawText(f.vstLogType, f.vstLogType.Roots[1], color, bold);
                Assert.True(bold[0]);
                Assert.Equal(System.Drawing.Color.Black, color[0]);   // acParent 只加粗

                bool allowed = true;
                f.vstLogTypeCollapsing(f.vstLogType, f.vstLogType.Roots[0], ref allowed);
                Assert.False(allowed);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void TimerTimer_DisablesTimer()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.Timer.Enabled = true;
                f.TimerTimer(f);
                Assert.False(f.Timer.Enabled);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void WMSYSCommand_MinimizeHidesForm_OtherDelegates()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                var minimize = new TFrmLogManage.TWMSYSCommand { CmdType = TFrmLogManage.SC_MINIMIZE };
                f.WMSYSCommand(ref minimize);
                Assert.False(f.Visible);

                var other = new TFrmLogManage.TWMSYSCommand { CmdType = 0xF120 /* SC_RESTORE */ };
                f.WMSYSCommand(ref other);   // 不抛异常即可
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void CheckListBoxClickCheck_IsCommentedOutNoOp()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.CheckListBoxClickCheck(f);   // 原文方法体整体被 (* *) 注释
                Assert.Null(LogDataForms.LastMessage);
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void SetClipboardText_WithoutHandler_UsesNoThrowPath()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                string viaHandler = "";
                TFrmLogManage.ClipboardHandler = s => viaHandler = s;
                TFrmLogManage.SetClipboardText("abc");
                Assert.Equal("abc", viaHandler);

                TFrmLogManage.ClipboardHandler = null;
                TFrmLogManage.SetClipboardText("xyz");   // 无剪贴板/无消息泵时静默
            }
            finally { f.Timer.Enabled = false; }
        });
    }

    [Fact]
    public void FormDestroy_ClearsLogDataList()
    {
        StaRunner.New(() =>
        {
            using var f = NewForm();
            try
            {
                f.btnStartClick(f);
                var shared = TSearchManagerHost.g_SearchManager.SearchDataList!;
                var list = shared.LockList();
                list.Add(Log(1));
                shared.UnlockList();

                f.FormDestroy(f);

                // 原文 FLogDataList.Free → 后续访问为 NRE（1:1 保留）
                Assert.Throws<NullReferenceException>(() => f.ClearLogDataList());
            }
            finally { f.Timer.Enabled = false; }
        });
    }
}
