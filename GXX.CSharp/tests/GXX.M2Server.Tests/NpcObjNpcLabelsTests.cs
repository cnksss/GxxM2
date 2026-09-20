// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：GXX.M2Server.Npc 切片 3（标签管理 / 脚本记录排序 / 脚本错误上报）
//   · TNormNpc.AllowSelect                 原文 5934-5951
//   · TNormNpc.AddSelectLable              原文 5953-5965
//   · TNormNpc.DeleteSelectLable           原文 5967-5979
//   · TNormNpc.ClearScript                 原文 4383-4429
//   · TNormNpc.QuickSortRecordList         原文 9955-9994
//   · TNormNpc.DoSort                      原文 9996-10017
//   · TNormNpc.GetSayingRecordFromRecordList 原文 10019-10046
//   · TNormNpc.ScriptActionError           原文 9745-9765
//   · TNormNpc.ScriptConditionError        原文 9767-9787
// ============================================================================

using System.Collections.Generic;
using GXX.M2Server.Npc;
using Xunit;

// 见 NpcObjNpcTests.cs 的同名说明：Engine.TNormNpc 与本车道 Npc.TNormNpc 同名，
// 文件级别名消歧（跨车道碰撞已上报调度方）。
using TNormNpc = GXX.M2Server.Npc.TNormNpc;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcLabelsTests : System.IDisposable
{
    private readonly List<string> _messages = new();

    public NpcObjNpcLabelsTests()
    {
        NpcSeams.ResetDefaults();
        NpcSeams.MainOutMessage = m => _messages.Add(m);
    }

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TSayingRecord Rec(string label)
        => new() { sLabel = label, ProcedureList = new List<object>() };

    private static List<object> SortedList(params string[] labels)
    {
        var list = new List<object>();
        foreach (string s in labels)
            list.Add(Rec(s));
        return list;
    }

    private static string[] LabelsOf(List<object> list)
    {
        var r = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
            r[i] = ((TSayingRecord)list[i]).sLabel;
        return r;
    }

    // -----------------------------------------------------------------------
    // AllowSelect / AddSelectLable / DeleteSelectLable
    // -----------------------------------------------------------------------

    [Fact]
    public void AllowSelect_EmptyBlacklist_ReturnsTrue()
    {
        var npc = new TNormNpc();
        Assert.True(npc.AllowSelect("@main"));
        Assert.Empty(_messages);
    }

    [Fact]
    public void AllowSelect_ExactEntry_ReturnsFalse()
    {
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@main");
        Assert.False(npc.AllowSelect("@main"));
    }

    [Fact]
    public void AllowSelect_PrefixEntry_MatchesLongerLabel()
    {
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@main");
        Assert.False(npc.AllowSelect("@main.sub"));
    }

    [Fact]
    public void AllowSelect_EntryLongerThanLabel_DoesNotMatch()
    {
        // 原文 5941 的比较长度取**条目长度** → sLabel 更短时 CompareLStr 直接返回 False。
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@main.sub");
        Assert.True(npc.AllowSelect("@main"));
    }

    [Fact]
    public void AllowSelect_IsCaseInsensitive()
    {
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@MAIN");
        Assert.False(npc.AllowSelect("@main"));
    }

    [Fact]
    public void AllowSelect_DiagnosticOnlyForFunctionOrMissionNpc()
    {
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@main");
        NpcSeams.IsFunctionOrMissionNpc = _ => false;
        npc.AllowSelect("@main");
        Assert.Empty(_messages);

        NpcSeams.IsFunctionOrMissionNpc = x => ReferenceEquals(x, npc);
        npc.AllowSelect("@main");
        Assert.Single(_messages);
        Assert.Equal("不可点字段:@main", _messages[0]);
    }

    [Fact]
    public void AddSelectLable_AppendsWhenAbsent()
    {
        var npc = new TNormNpc();
        npc.AddSelectLable("@a");
        npc.AddSelectLable("@b");
        Assert.Equal(2, npc.m_NoUserSelectList.Count);
        Assert.Equal("@a", npc.m_NoUserSelectList[0]);
        Assert.Equal("@b", npc.m_NoUserSelectList[1]);
    }

    [Fact]
    public void AddSelectLable_SkipsPrefixDuplicate()
    {
        var npc = new TNormNpc();
        npc.AddSelectLable("@a");
        npc.AddSelectLable("@a.child");
        Assert.Equal(1, npc.m_NoUserSelectList.Count);
    }

    [Fact]
    public void AddSelectLable_ShorterLabelIsNotADuplicate()
    {
        var npc = new TNormNpc();
        npc.AddSelectLable("@a.child");
        npc.AddSelectLable("@a");
        Assert.Equal(2, npc.m_NoUserSelectList.Count);
    }

    [Fact]
    public void DeleteSelectLable_RemovesFirstMatchOnly()
    {
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@a");
        npc.m_NoUserSelectList.Add("@a2");
        npc.m_NoUserSelectList.Add("@a3");
        // `@a` 命中第 0 项即 Break；`@a2`/`@a3` 因长度更长不参与匹配。
        npc.DeleteSelectLable("@a");
        Assert.Equal(2, npc.m_NoUserSelectList.Count);
        Assert.Equal("@a2", npc.m_NoUserSelectList[0]);
    }

    [Fact]
    public void DeleteSelectLable_NoMatch_LeavesListIntact()
    {
        var npc = new TNormNpc();
        npc.m_NoUserSelectList.Add("@zzz");
        npc.DeleteSelectLable("@a");
        Assert.Equal(1, npc.m_NoUserSelectList.Count);
    }

    // -----------------------------------------------------------------------
    // ClearScript（4383-4429）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClearScript_EmptyList_IsNoOp()
    {
        var npc = new TNormNpc();
        npc.ClearScript();
        Assert.Empty(npc.m_ScriptList);
    }

    [Fact]
    public void ClearScript_WalksAllFourLevelsThenClearsTopList()
    {
        var npc = new TNormNpc();
        var cond = new TQuestConditionInfo();
        var act = new TQuestActionInfo();
        var elseAct = new TQuestActionInfo();
        var proc = new TSayingProcedure
        {
            ConditionList = new TConditionList(),
            ActionList = new List<object>(),
            ElseActionList = new List<object>(),
        };
        proc.ConditionList.Items.Add(cond);
        proc.ActionList.Add(act);
        proc.ElseActionList.Add(elseAct);

        var record = new TSayingRecord { sLabel = "@main", ProcedureList = new List<object>() };
        record.ProcedureList.Add(proc);

        var script = new TScript();
        script.RecordList.Add(record);
        npc.m_ScriptList.Add(script);

        npc.ClearScript();

        Assert.Empty(npc.m_ScriptList);
        // 原文只做 Dispose（托管侧无动作），嵌套容器**不被清空** —— 差异断言。
        Assert.Single(script.RecordList);
        Assert.Single(record.ProcedureList);
        Assert.Single(proc.ConditionList.Items);
        Assert.Single(proc.ActionList);
        Assert.Single(proc.ElseActionList);
    }

    [Fact]
    public void ClearScript_MultipleScripts_AllVisitedThenTopCleared()
    {
        var npc = new TNormNpc();
        for (int i = 0; i < 3; i++)
        {
            var s = new TScript();
            s.RecordList.Add(new TSayingRecord { sLabel = $"@L{i}", ProcedureList = new List<object>() });
            npc.m_ScriptList.Add(s);
        }
        npc.ClearScript();
        Assert.Empty(npc.m_ScriptList);
    }

    // -----------------------------------------------------------------------
    // QuickSortRecordList / DoSort（9955-10017）
    // -----------------------------------------------------------------------

    [Fact]
    public void QuickSortRecordList_SortsAscending()
    {
        var npc = new TNormNpc();
        var list = SortedList("@c", "@a", "@d", "@b");
        npc.QuickSortRecordList(list, 0, list.Count - 1);
        Assert.Equal(new[] { "@a", "@b", "@c", "@d" }, LabelsOf(list));
    }

    [Fact]
    public void QuickSortRecordList_IsCaseInsensitive()
    {
        // 原文用 SysUtils.CompareText → 大小写不敏感。
        var npc = new TNormNpc();
        var list = SortedList("@b", "@A", "@c");
        npc.QuickSortRecordList(list, 0, list.Count - 1);
        Assert.Equal(new[] { "@A", "@b", "@c" }, LabelsOf(list));
    }

    [Fact]
    public void QuickSortRecordList_SingleElementSubRange()
    {
        var npc = new TNormNpc();
        var one = SortedList("@x");
        npc.QuickSortRecordList(one, 0, 0);
        Assert.Equal(new[] { "@x" }, LabelsOf(one));
    }

    [Fact]
    public void QuickSortRecordList_EmptyRange_HasNoGuardAndThrows()
    {
        // 原文缺陷（9955-9994）：函数体内**没有** `R < L` 的守卫，
        // `P := (L + R) shr 1` 在 (0, -1) 时得 -1，紧接着 `List.Items[P]` 越界。
        // Delphi 侧 `TList.Items[-1]` 无范围检查 → 读到数组外的垃圾指针（行为未定义）；
        // 托管侧 `List<T>` 索引器抛 ArgumentOutOfRangeException。
        // 正常路径不受影响：`DoSort`(10005) 已用 `Count > 0` 把空表挡在外面。
        var npc = new TNormNpc();
        var empty = new List<object>();
        Assert.Throws<System.ArgumentOutOfRangeException>(() => npc.QuickSortRecordList(empty, 0, -1));
    }

    [Fact]
    public void QuickSortRecordList_DuplicateLabels_KeptAll()
    {
        var npc = new TNormNpc();
        var list = SortedList("@a", "@a", "@a", "@b");
        npc.QuickSortRecordList(list, 0, list.Count - 1);
        Assert.Equal(new[] { "@a", "@a", "@a", "@b" }, LabelsOf(list));
    }

    [Fact]
    public void QuickSortRecordList_LargeRandomInput_MatchesSortedCopy()
    {
        var npc = new TNormNpc();
        var list = new List<object>();
        var expect = new List<string>();
        for (int i = 0; i < 200; i++)
        {
            string label = "@" + ((i * 37) % 200).ToString("D3");
            list.Add(Rec(label));
            expect.Add(label);
        }
        expect.Sort(System.StringComparer.OrdinalIgnoreCase);
        npc.QuickSortRecordList(list, 0, list.Count - 1);
        Assert.Equal(expect.ToArray(), LabelsOf(list));
    }

    [Fact]
    public void DoSort_SortsEachScriptRecordList()
    {
        var npc = new TNormNpc();
        var s1 = new TScript();
        s1.RecordList.Add(Rec("@z"));
        s1.RecordList.Add(Rec("@a"));
        var s2 = new TScript();
        s2.RecordList.Add(Rec("@y"));
        s2.RecordList.Add(Rec("@b"));
        npc.m_ScriptList.Add(s1);
        npc.m_ScriptList.Add(s2);

        npc.DoSort();

        Assert.Equal(new[] { "@a", "@z" }, LabelsOf(s1.RecordList));
        Assert.Equal(new[] { "@b", "@y" }, LabelsOf(s2.RecordList));
    }

    [Fact]
    public void DoSort_EmptyRecordList_IsSkipped()
    {
        var npc = new TNormNpc();
        var s = new TScript();
        npc.m_ScriptList.Add(s);
        npc.DoSort();   // 原文 10005 的 Count > 0 门槛 → 不调用 QuickSortRecordList(0, -1)
        Assert.Empty(s.RecordList);
    }

    // -----------------------------------------------------------------------
    // GetSayingRecordFromRecordList（10019-10046）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetSayingRecordFromRecordList_Found()
    {
        var npc = new TNormNpc();
        var list = SortedList("@a", "@b", "@c", "@d", "@e");
        Assert.Same(list[2], npc.GetSayingRecordFromRecordList(list, "@c"));
    }

    [Fact]
    public void GetSayingRecordFromRecordList_NotFound_ReturnsNull()
    {
        var npc = new TNormNpc();
        var list = SortedList("@a", "@b", "@c");
        Assert.Null(npc.GetSayingRecordFromRecordList(list, "@zz"));
        Assert.Null(npc.GetSayingRecordFromRecordList(list, ""));
    }

    [Fact]
    public void GetSayingRecordFromRecordList_NullListOrEmptyLabel_ReturnsNull()
    {
        var npc = new TNormNpc();
        Assert.Null(npc.GetSayingRecordFromRecordList(null, "@a"));
        Assert.Null(npc.GetSayingRecordFromRecordList(SortedList("@a"), ""));
    }

    [Fact]
    public void GetSayingRecordFromRecordList_IsCaseInsensitive()
    {
        var npc = new TNormNpc();
        var list = SortedList("@A", "@B");
        Assert.Same(list[0], npc.GetSayingRecordFromRecordList(list, "@a"));
    }

    [Fact]
    public void GetSayingRecordFromRecordList_SingleElement()
    {
        var npc = new TNormNpc();
        var list = SortedList("@only");
        Assert.Same(list[0], npc.GetSayingRecordFromRecordList(list, "@only"));
    }

    // -----------------------------------------------------------------------
    // ScriptActionError（9745-9765）/ ScriptConditionError（9767-9787）
    // -----------------------------------------------------------------------

    [Fact]
    public void ScriptActionError_EmitsFormattedMessage()
    {
        var npc = new TNormNpc
        {
            m_sCharName = "NPC1",
            m_sMapName = "0",
            m_nCurrX = 100,
            m_nCurrY = 200,
        };
        var act = new TQuestActionInfo
        {
            sCmd = "GIVE",
            sParam1 = "P1",
            sParam2 = "P2",
        };
        npc.ScriptActionError(null, "参数不足", act);

        Assert.Single(_messages);
        Assert.Equal(
            "[脚本错误] 参数不足 脚本命令:GIVE NPC名称:NPC1 地图:0(100:200) 参数1:P1 参数2:P2 参数3: 参数4: 参数5: 参数6: 参数7: 参数8: 参数9: 参数10:",
            _messages[0]);
    }

    [Fact]
    public void ScriptConditionError_NeverEmitsAnything()
    {
        // 原文 9774-9786：整段（含唯一的 MainOutMessage）被 `{ }` 注释掉 —— 本过程实际什么都不做。
        var npc = new TNormNpc { m_sCharName = "NPC1", m_sMapName = "0" };
        var con = new TQuestConditionInfo { sCmd = "CHECKITEM", sParam1 = "P1" };
        npc.ScriptConditionError(null, con);
        Assert.Empty(_messages);
    }

    [Fact]
    public void ErrorFormatConstants_MatchOriginalResourceStrings()
    {
        Assert.Equal(
            "[脚本错误] %s 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s 参数7:%s 参数8:%s 参数9:%s 参数10:%s",
            TNormNpc.ScriptActionErrorFormat);
        Assert.Equal(
            "[脚本错误] 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s 参数7:%s 参数8:%s 参数9:%s 参数10:%s",
            TNormNpc.ScriptConditionErrorFormat);
    }
}
