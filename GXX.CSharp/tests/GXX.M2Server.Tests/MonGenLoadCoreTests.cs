using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J109：LoadMonGen 文件加载决策（LocalDB.pas 3359-3527）+
/// DoInitSortMapMonGenList/FindFirstSortMapMonGenListIndex（UsrEngn.pas 11179-11259）1:1 测试。
/// </summary>
public sealed class MonGenLoadCoreTests
{
    private sealed class Row
    {
        public string MapName = "";
        public override string ToString() => MapName;
    }

    private static List<Row> Rows(params string[] names)
    {
        var r = new List<Row>();
        foreach (string n in names)
            r.Add(new Row { MapName = n });
        return r;
    }

    // ===================== 常量与路径 =====================

    [Fact]
    public void MainFileNameIsMonGenTxt()
    {
        Assert.Equal("MonGen.txt", MonGenLoadCore.MainFileName);
    }

    [Fact]
    public void HookPathPrefixesMonGenDir()
    {
        // 3395/3517：'MonGen\' + 文件名
        Assert.Equal("MonGen\\sub.txt", MonGenLoadCore.HookPath("sub.txt"));
    }

    [Fact]
    public void MonGenSubDirMatchesSource()
    {
        // 3372：g_Config.sEnvirDir + 'MonGen\'
        Assert.Equal("MonGen\\", MonGenLoadCore.MonGenSubDir);
    }

    // ===================== 文本来源（3378/3468-3474） =====================

    [Fact]
    public void DiskFileWinsOverHook()
    {
        // 优先级：磁盘 > 钩子
        Assert.Equal(MonGenLoadCore.TextSource.DiskFile,
            MonGenLoadCore.SelectTextSource(fileExists: true, pluginManagerPresent: true, hookEnabled: true));
    }

    [Fact]
    public void HookUsedWhenNoDiskFile()
    {
        Assert.Equal(MonGenLoadCore.TextSource.PluginHook,
            MonGenLoadCore.SelectTextSource(false, true, true));
    }

    [Fact]
    public void NoSourceWhenHookDisabled()
    {
        // 3474 的 else → 3507
        Assert.Equal(MonGenLoadCore.TextSource.NoSource,
            MonGenLoadCore.SelectTextSource(false, true, false));
    }

    [Fact]
    public void NoSourceWhenNoPluginManager()
    {
        Assert.Equal(MonGenLoadCore.TextSource.NoSource,
            MonGenLoadCore.SelectTextSource(false, false, false));
    }

    [Fact]
    public void HookDisabledEvenIfManagerPresent()
    {
        // 3474 要求两者同时成立
        Assert.NotEqual(MonGenLoadCore.TextSource.PluginHook,
            MonGenLoadCore.SelectTextSource(false, true, hookEnabled: false));
    }

    // ===================== 兜底判定：主文件 vs 二级文件 =====================

    [Fact]
    public void DiskFileNeverFallsBack()
    {
        Assert.False(MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.DiskFile, false, false, false));
    }

    [Fact]
    public void MainFileFallsBackWhenHookReturnsFalse()
    {
        // **3490-3492：主文件在钩子返回 false 时**兜底**（与二级文件不同）
        Assert.True(MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.PluginHook, hookReturnedFalse: true, false, false));
    }

    [Fact]
    public void SubFileDoesNotFallBackWhenHookReturnsFalse()
    {
        // **3397-3399：二级文件仅 Exit，不兜底**——这是与主文件的关键差异
        Assert.True(MonGenLoadCore.SubFileExitsOnHookFalse(hookReturnedFalse: true));
    }

    [Fact]
    public void MainAndSubFileDifferOnHookFalse()
    {
        // **差异保护**：同一输入下"是否兜底"结论相反
        bool mainFallsBack = MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.PluginHook, true, false, false);
        bool subOnlyExits = MonGenLoadCore.SubFileExitsOnHookFalse(true);

        Assert.True(mainFallsBack);
        Assert.True(subOnlyExits);
        // 二者对同一情形的处理方式不同（一个补空表、一个什么都不做）
        Assert.NotEqual(mainFallsBack, !subOnlyExits);
    }

    [Fact]
    public void MainFileFallsBackWhenHookThrows()
    {
        // 3495-3501
        Assert.True(MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.PluginHook, false, hookThrew: true, false));
    }

    [Fact]
    public void MainFileDoesNotFallBackWhenStreamEmpty()
    {
        // 3481-3488：钩子成功但流为空 → LoadList 已创建（非 nil）→ **不兜底**，继续解析空表
        Assert.False(MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.PluginHook, false, false, streamEmpty: true));
    }

    [Fact]
    public void MainFileFallsBackWhenNoSource()
    {
        // 3505-3508：钩子未启用 → 兜底并 Exit
        Assert.True(MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.NoSource, false, false, false));
    }

    [Fact]
    public void EmptyStreamDiffersFromHookFalse()
    {
        // **差异保护**：两种 PluginHook 情形下兜底结论不同
        bool onFalse = MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.PluginHook, true, false, false);
        bool onEmpty = MonGenLoadCore.ShouldAddEmptyMonGenInfo(
            MonGenLoadCore.TextSource.PluginHook, false, false, true);

        Assert.NotEqual(onFalse, onEmpty);
    }

    // ===================== 放弃加载（3510-3511） =====================

    [Fact]
    public void AbortWhenListNull()
    {
        Assert.True(MonGenLoadCore.ShouldAbortLoad(loadListIsNull: true));
    }

    [Fact]
    public void ProceedWhenListPresent()
    {
        Assert.False(MonGenLoadCore.ShouldAbortLoad(false));
    }

    // ===================== loadgen 行解析（3517-3525） =====================

    [Fact]
    public void ParseLoadGenLineExtractsFileName()
    {
        var (name, load) = MonGenLoadCore.ParseLoadGenLine("loadgen sub.txt");
        Assert.Equal("sub.txt", name);
        Assert.True(load);
    }

    [Fact]
    public void ParseLoadGenLineEmptyNameSkipsLoad()
    {
        // 3521：文件名为空则不加载
        var (name, load) = MonGenLoadCore.ParseLoadGenLine("loadgen");
        Assert.Equal("", name);
        Assert.False(load);
    }

    [Fact]
    public void ParseLoadGenLineCaseInsensitivePrefix()
    {
        var (name, _) = MonGenLoadCore.ParseLoadGenLine("LoadGen sub.txt");
        Assert.Equal("sub.txt", name);
    }

    // ===================== ExpandLoadGen（3513-3527） =====================

    [Fact]
    public void ExpandRemovesLoadGenLine()
    {
        var main = new List<string> { "loadgen sub.txt" };
        var result = MonGenLoadCore.ExpandLoadGen(main, _ => new List<string> { "0 1 1 鸡 1 1 10" });

        Assert.Single(result);
        Assert.Equal("0 1 1 鸡 1 1 10", result[0]);
    }

    [Fact]
    public void ExpandAppendsSubFileLinesAtEnd()
    {
        // 3523：LoadMapGen 把二级行**追加到末尾**
        var main = new List<string>
        {
            "loadgen sub.txt",
            "1 2 2 鹿 1 1 10",
        };

        var result = MonGenLoadCore.ExpandLoadGen(main, _ => new List<string> { "9 9 9 猪 1 1 10" });

        Assert.Equal(2, result.Count);
        Assert.Equal("1 2 2 鹿 1 1 10", result[0]);
        Assert.Equal("9 9 9 猪 1 1 10", result[1]);
    }

    [Fact]
    public void ExpandKeepsOtherLines()
    {
        var main = new List<string>
        {
            "0 1 1 鸡 1 1 10",
            "loadgen sub.txt",
            "2 3 3 猪 1 1 10",
        };

        var result = MonGenLoadCore.ExpandLoadGen(main, _ => new List<string> { "9 9 9 龙 1 1 10" });

        Assert.Equal(3, result.Count);
        Assert.DoesNotContain(result, l => MonGenParseCore.IsLoadGenLine(l));
    }

    [Fact]
    public void ExpandNilSubFileJustRemovesLine()
    {
        // 二级文件取不到（磁盘无、钩子也拿不到）→ 只删掉 loadgen 行
        var main = new List<string> { "loadgen missing.txt", "0 1 1 鸡 1 1 10" };
        var result = MonGenLoadCore.ExpandLoadGen(main, _ => null);

        Assert.Single(result);
        Assert.Equal("0 1 1 鸡 1 1 10", result[0]);
    }

    [Fact]
    public void ExpandEmptySubFileJustRemovesLine()
    {
        var main = new List<string> { "loadgen empty.txt" };
        var result = MonGenLoadCore.ExpandLoadGen(main, _ => new List<string>());

        Assert.Empty(result);
    }

    [Fact]
    public void ExpandDoesNotRecurseIntoAppendedLines()
    {
        // **反直觉但正确**：二级行追加在**末尾**，而 `Delete(I)` + `Inc(I)` 使游标越过它们，
        // 故二级文件里的 `loadgen` **不会**被再次处理（它的行留在了结果里，
        // 之后由 J108 的解析器当普通数据行处理并多半丢弃）。
        var main = new List<string> { "loadgen a.txt" };

        var files = new Dictionary<string, IReadOnlyList<string>>
        {
            ["a.txt"] = new List<string> { "loadgen b.txt" },
            ["b.txt"] = new List<string> { "0 1 1 鸡 1 1 10" },
        };

        var result = MonGenLoadCore.ExpandLoadGen(main, f => files.TryGetValue(f, out var v) ? v : null);

        // 内部那行 loadgen 原样保留，**没有**展开成 b.txt 的内容
        Assert.Single(result);
        Assert.Equal("loadgen b.txt", result[0]);
        Assert.DoesNotContain(result, l => l == "0 1 1 鸡 1 1 10");
    }

    [Fact]
    public void ExpandSkipsAdjacentLoadGenLine()
    {
        // **连续的 loadgen 行会互相跳过**：处理第一条时 `Delete(0)` 使第二条落到下标 0，
        // 而 `Inc(I)` 把游标推到 1（已越过它），故第二条**不会被处理**，
        // 原样留在结果里（之后由 J108 解析器当普通数据行处理并丢弃）。
        var main = new List<string>
        {
            "loadgen a.txt",
            "loadgen b.txt",
        };

        var files = new Dictionary<string, IReadOnlyList<string>>
        {
            ["a.txt"] = new List<string> { "A-CONTENT" },
            ["b.txt"] = new List<string> { "B-CONTENT" },
        };

        var result = MonGenLoadCore.ExpandLoadGen(main, f => files.TryGetValue(f, out var v) ? v : null);

        // 只有第一条被展开
        Assert.Contains("A-CONTENT", result);
        Assert.DoesNotContain("B-CONTENT", result);
        Assert.Contains("loadgen b.txt", result);   // 第二条原样保留
    }

    [Fact]
    public void ExpandProcessesSecondLoadGenWhenSeparated()
    {
        // 若两条 loadgen 之间隔了普通行，则第二条能被正常处理
        // （因游标推进到普通行时，第二条仍在游标前方）
        var main = new List<string>
        {
            "loadgen a.txt",
            "0 1 1 鸡 1 1 10",
            "loadgen b.txt",
        };

        var files = new Dictionary<string, IReadOnlyList<string>>
        {
            ["a.txt"] = new List<string> { "A-CONTENT" },
            ["b.txt"] = new List<string> { "B-CONTENT" },
        };

        var result = MonGenLoadCore.ExpandLoadGen(main, f => files.TryGetValue(f, out var v) ? v : null);

        Assert.Contains("A-CONTENT", result);
        Assert.Contains("B-CONTENT", result);
        Assert.DoesNotContain(result, l => MonGenParseCore.IsLoadGenLine(l));
    }

    [Fact]
    public void ExpandSkipsLineAfterLoadGen()
    {
        // **游标语义**：Delete(I) 后原 I+1 落到 I，再 Inc(I) 使该行**被跳过**
        var main = new List<string>
        {
            "loadgen a.txt",
            "SECOND-LINE",
        };

        var result = MonGenLoadCore.ExpandLoadGen(main, _ => new List<string> { "APPENDED" });

        // 'SECOND-LINE' 被跳过（未被删除，留在结果里）
        Assert.Contains("SECOND-LINE", result);
        Assert.Contains("APPENDED", result);
        Assert.DoesNotContain(result, l => MonGenParseCore.IsLoadGenLine(l));
    }

    [Fact]
    public void ExpandHandlesEmptyMainList()
    {
        Assert.Empty(MonGenLoadCore.ExpandLoadGen(new List<string>(), _ => new List<string>()));
    }

    [Fact]
    public void ExpandDoesNotRemoveNonLoadGenLines()
    {
        var main = new List<string> { "0 1 1 鸡 1 1 10" };
        var result = MonGenLoadCore.ExpandLoadGen(main, _ => null);

        Assert.Single(result);
    }

    [Fact]
    public void ExpandWithEmptyFileNameKeepsNoLineAndLoadsNothing()
    {
        // 3521：文件名为空 → 不加载；3520 的 Delete 仍执行
        var main = new List<string> { "loadgen", "0 1 1 鸡 1 1 10" };
        bool called = false;

        var result = MonGenLoadCore.ExpandLoadGen(main, _ => { called = true; return null; });

        Assert.False(called);
        Assert.Single(result);
    }

    // ===================== AnsiCompareText =====================

    [Fact]
    public void AnsiCompareTextIsCaseInsensitive()
    {
        Assert.Equal(0, MonGenLoadCore.AnsiCompareText("abc", "ABC"));
    }

    [Fact]
    public void AnsiCompareTextOrders()
    {
        Assert.True(MonGenLoadCore.AnsiCompareText("a", "b") < 0);
        Assert.True(MonGenLoadCore.AnsiCompareText("b", "a") > 0);
    }

    // ===================== SortByMapName（11179-11227） =====================

    [Fact]
    public void SortOrdersByMapName()
    {
        var list = Rows("3", "1", "2");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        Assert.Equal(new[] { "1", "2", "3" }, new[] { list[0].MapName, list[1].MapName, list[2].MapName });
    }

    [Fact]
    public void SortEmptyListIsNoOp()
    {
        var list = new List<Row>();
        MonGenLoadCore.SortByMapName(list, r => r.MapName);
        Assert.Empty(list);
    }

    [Fact]
    public void SortSingleElement()
    {
        var list = Rows("only");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);
        Assert.Single(list);
    }

    [Fact]
    public void SortAlreadySortedIsStable()
    {
        var list = Rows("a", "b", "c");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        Assert.Equal("a", list[0].MapName);
        Assert.Equal("c", list[2].MapName);
    }

    [Fact]
    public void SortReverseOrder()
    {
        var list = Rows("e", "d", "c", "b", "a");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        for (int i = 1; i < list.Count; i++)
            Assert.True(MonGenLoadCore.AnsiCompareText(list[i - 1].MapName, list[i].MapName) <= 0);
    }

    [Fact]
    public void SortHandlesDuplicates()
    {
        var list = Rows("b", "a", "b", "a");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        Assert.Equal(4, list.Count);
        Assert.Equal("a", list[0].MapName);
        Assert.Equal("a", list[1].MapName);
        Assert.Equal("b", list[2].MapName);
        Assert.Equal("b", list[3].MapName);
    }

    [Fact]
    public void SortIsCaseInsensitive()
    {
        var list = Rows("B", "a");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        // 'a' < 'B'（不区分大小写）
        Assert.Equal("a", list[0].MapName);
    }

    [Fact]
    public void SortManyElementsIsCorrect()
    {
        var names = new List<string>();
        var rand = new Random(12345);
        for (int i = 0; i < 200; i++)
            names.Add(rand.Next(50).ToString());

        var list = Rows(names.ToArray());
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        for (int i = 1; i < list.Count; i++)
            Assert.True(MonGenLoadCore.AnsiCompareText(list[i - 1].MapName, list[i].MapName) <= 0,
                $"下标 {i} 处顺序错误");
    }

    [Fact]
    public void SortPreservesElementCount()
    {
        var list = Rows("c", "a", "b", "a");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);
        Assert.Equal(4, list.Count);
    }

    [Fact]
    public void SortPreservesDuplicateMultiplicity()
    {
        var list = Rows("x", "x", "x", "y");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        int xCount = 0;
        foreach (var r in list)
            if (r.MapName == "x") xCount++;

        Assert.Equal(3, xCount);
    }

    [Fact]
    public void SortEmptyMapNames()
    {
        var list = Rows("", "a", "");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        Assert.Equal("", list[0].MapName);   // 空串排最前
        Assert.Equal("a", list[2].MapName);
    }

    // ===================== FindFirstSortMapMonGenListIndex（11229-11259） =====================

    [Fact]
    public void FindReturnsFirstMatch()
    {
        var list = Rows("a", "b", "b", "b", "c");
        Assert.Equal(1, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "b"));
    }

    [Fact]
    public void FindReturnsMinusOneWhenAbsent()
    {
        var list = Rows("a", "b", "c");
        Assert.Equal(-1, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "z"));
    }

    [Fact]
    public void FindOnEmptyListIsMinusOne()
    {
        Assert.Equal(-1, MonGenLoadCore.FindFirstByMapName(new List<Row>(), r => r.MapName, "a"));
    }

    [Fact]
    public void FindFirstAndLastElement()
    {
        var list = Rows("a", "b", "c");

        Assert.Equal(0, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "a"));
        Assert.Equal(2, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "c"));
    }

    [Fact]
    public void FindAllSameReturnsZero()
    {
        var list = Rows("a", "a", "a");
        Assert.Equal(0, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "a"));
    }

    [Fact]
    public void FindIsCaseInsensitive()
    {
        // AnsiCompareText 不区分大小写
        var list = Rows("Abc", "def");
        Assert.Equal(0, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "abc"));
    }

    [Fact]
    public void FindOverManyDuplicates()
    {
        var list = new List<Row>();
        for (int i = 0; i < 10; i++) list.Add(new Row { MapName = "a" });
        for (int i = 0; i < 10; i++) list.Add(new Row { MapName = "b" });

        Assert.Equal(10, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "b"));
    }

    [Fact]
    public void FindAfterSortingMatchesSourceOrder()
    {
        // **整合**：先按原文排序，再按原文二分查找
        var list = Rows("3", "1", "2", "1");
        MonGenLoadCore.SortByMapName(list, r => r.MapName);

        Assert.Equal(0, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "1"));
        Assert.Equal(2, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "2"));
        Assert.Equal(3, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "3"));
    }

    [Fact]
    public void FindEmptyMapNameMatchesEmptyRows()
    {
        // 空挂靠点（AddEmptyMonGenInfo）的地图名为空，故可被空名查得
        var list = Rows("", "", "a");
        Assert.Equal(0, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, ""));
    }

    [Fact]
    public void FindSingleElementHitAndMiss()
    {
        var list = Rows("a");

        Assert.Equal(0, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "a"));
        Assert.Equal(-1, MonGenLoadCore.FindFirstByMapName(list, r => r.MapName, "b"));
    }
}
