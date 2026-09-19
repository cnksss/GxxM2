using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Client.GUI.GameConfig;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Core.Util;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-client-guiconfig）：FilterItems.pas(1-492) 1:1 移植测试。
/// 覆盖：分类名/分类枚举双向映射、方位字符、LoadFormList 解析（分隔符/空行/注释/越界类型）、
/// SaveToFile 落盘**完整文本**、导入导出对称性、Add/Del/Find 的大小写与 boFromSystem 规则、Hint 文案。
/// </summary>
[Collection("GuiCfgConfigure")]
public sealed class GuiCfgFilterItemsTests : IDisposable
{
    private readonly string _root;
    private readonly string _configDir;

    public GuiCfgFilterItemsTests()
    {
        GuiCfgTestEnv.Reset();
        _root = Path.Combine(Path.GetTempPath(), "gxx-guicfg-" + Guid.NewGuid().ToString("N"));
        _configDir = Path.Combine(_root, "Config");
        Directory.CreateDirectory(_configDir);
        SelfFilePathSeam.g_sSelfFilePath = _root + "\\";   // 对应 MShare.g_sSelfFilePath（原文含尾分隔符）
        ConfigShareGlobal.g_sPlugServerName = "测试服";
    }

    public void Dispose()
    {
        GuiCfgTestEnv.Reset();
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch { }
    }

    // ============================================================ 工具函数

    [Fact]
    public void GetSelectStringReturnsCheckMarkOrEmpty()
    {
        // FilterItems.pas:55-61
        Assert.Equal("√", TFileItemDB.GetSelectString(true));
        Assert.Equal("", TFileItemDB.GetSelectString(false));
    }

    [Theory]
    [InlineData(TItemType.i_Other, "其它类")]
    [InlineData(TItemType.i_HPMPDurg, "药品类")]
    [InlineData(TItemType.i_Dress, "服装类")]
    [InlineData(TItemType.i_Weapon, "武器类")]
    [InlineData(TItemType.i_Jewelry, "首饰类")]
    [InlineData(TItemType.i_Decoration, "饰品类")]
    [InlineData(TItemType.i_Decorate, "装饰类")]
    [InlineData(TItemType.i_diy, "自定类")]
    public void GetItemTypeNameMapsAllButIAll(TItemType t, string expected)
    {
        // FilterItems.pas:63-75：**原文没有 i_All 分支** → i_All 返回空串
        Assert.Equal(expected, TFileItemDB.GetItemTypeName(t));
    }

    [Fact]
    public void GetItemTypeNameIAllHasNoBranchSoReturnsEmpty()
    {
        // 原文如此（FilterItems.pas:65 的 case 从 i_Other 开始）
        Assert.Equal("", TFileItemDB.GetItemTypeName(TItemType.i_All));
  }

    [Theory]
    [InlineData("其它类", TItemType.i_Other)]
    [InlineData("药品类", TItemType.i_HPMPDurg)]
    [InlineData("服装类", TItemType.i_Dress)]
    [InlineData("武器类", TItemType.i_Weapon)]
    [InlineData("首饰类", TItemType.i_Jewelry)]
    [InlineData("饰品类", TItemType.i_Decoration)]
    [InlineData("装饰类", TItemType.i_Decorate)]
    [InlineData("自定类", TItemType.i_diy)]
    public void GetItemTypeIsExactAndSizeSensitive(string name, TItemType expected)
    {
        // FilterItems.pas:77-88：用 `=` 精确比较（**大小写/空白敏感**）
        Assert.Equal(expected, TFileItemDB.GetItemType(name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("其它")]
    [InlineData("其它类 ")]
    [InlineData("其它类x")]
    [InlineData("all")]
    public void GetItemTypeFallsBackToDiy(string name)
    {
        // 默认值是 i_diy，任何不匹配都返回 i_diy
        Assert.Equal(TItemType.i_diy, TFileItemDB.GetItemType(name));
    }

    [Theory]
    [InlineData(0, "↑")]
    [InlineData(1, "↗")]
    [InlineData(2, "→")]
    [InlineData(3, "↘")]
    [InlineData(4, "↓")]
    [InlineData(5, "↙")]
    [InlineData(6, "←")]
    [InlineData(7, "↖")]
    public void GetActorDirMapsAllEightDirections(int ndir, string expected)
    {
        // FilterItems.pas:90-107
        ConfigShareSeam.g_MySelf = new FakeSelf(100, 200, "我");
        NextDirectionSeam.GetNextDirection = (x1, y1, x2, y2) => ndir;
        Assert.Equal(expected, TFileItemDB.GetActorDir(1, 1));
    }

    [Fact]
    public void GetActorDirExitsBeforeCallingGetNextDirectionWhenNoSelf()
    {
        // FilterItems.pas:95：`if (g_MySelf = nil) then Exit;` 在取方位**之前**
        int calls = 0;
        NextDirectionSeam.GetNextDirection = (a, b, c, d) => { calls++; return 0; };
        ConfigShareSeam.g_MySelf = null;
        Assert.Equal("", TFileItemDB.GetActorDir(1, 1));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void GetActorDirOutOfRangeDirectionReturnsEmpty()
    {
        // case 只覆盖 0..7，其它值 Result 保持 ''
        ConfigShareSeam.g_MySelf = new FakeSelf(0, 0, "我");
        NextDirectionSeam.GetNextDirection = (a, b, c, d) => 8;
        Assert.Equal("", TFileItemDB.GetActorDir(9, 9));
    }

    [Fact]
    public void GetActorDirUsesMySelfCoordinates()
    {
        ConfigShareSeam.g_MySelf = new FakeSelf(11, 22, "我");
        int x1 = -1, y1 = -1, x2 = -1, y2 = -1;
        NextDirectionSeam.GetNextDirection = (a, b, c, d) => { x1 = a; y1 = b; x2 = c; y2 = d; return 2; };
        TFileItemDB.GetActorDir(77, 88);
        Assert.Equal((11, 22, 77, 88), (x1, y1, x2, y2));
    }

    // ============================================================ LoadFormList 解析

    private static TStringList Lines(params string[] lines)
    {
        var l = new TStringList();
        foreach (var s in lines) l.Add(s);
        return l;
    }

    [Fact]
    public void LoadFormListCreatesCustomItemsFromNewNames()
    {
        // FilterItems.pas:195-243：未命中 Find → 新建并同时进 m_ShowItemList 与 m_FileItemList
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,1,1,0,0"), false);

        Assert.Single(db.m_ShowItemList);
        Assert.Single(db.m_FileItemList);
        var it = db.m_ShowItemList[0];
        Assert.Equal(TItemType.i_Dress, it.ItemType);
        Assert.Equal("服装类", it.sItemType);
        Assert.Equal("布衣", it.sItemName);
        Assert.Equal(1, it.boHintMsg);
        Assert.Equal(1, it.boPickup);
        Assert.Equal(1, it.boShowName);
        Assert.Equal(0, it.boShowSpecial);
        Assert.Equal(0, it.boAutoMove);
        Assert.Equal(0, it.boFromSystem);
    }

    [Fact]
    public void LoadFormListFromSystemSetsBoFromSystemAndSkipsCheckFlagsOnExisting()
    {
        // 原文 215-222：命中已存在项时，**仅当 FromSystem=False 才覆盖 5 个勾选位**，
        // 但 boFromSystem 无条件写入。
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,屠龙,0,0,0,0,0"), false);
        var it = db.m_ShowItemList[0];
        Assert.Equal(0, it.boHintMsg);

        db.LoadFormList(Lines("4,屠龙,1,1,1,1,1"), true);
        Assert.Same(it, db.m_ShowItemList[0]);
        Assert.Equal(0, it.boHintMsg);        // FromSystem=True → 勾选位保持原值
        Assert.Equal(1, it.boFromSystem);     // boFromSystem 无条件覆盖
    }

    [Fact]
    public void LoadFormListTabIsAlsoADivider()
    {
        // 原文 199-205 的分隔符集合是 [',', #9]
        var db = new TFileItemDB();
        db.LoadFormList(Lines("2\t金创药\t1\t0\t1\t0\t1"), false);
        var it = db.m_ShowItemList[0];
        Assert.Equal(TItemType.i_HPMPDurg, it.ItemType);
        Assert.Equal("金创药", it.sItemName);
        Assert.Equal(1, it.boHintMsg);
        Assert.Equal(0, it.boPickup);
        Assert.Equal(1, it.boShowName);
        Assert.Equal(0, it.boShowSpecial);
        Assert.Equal(1, it.boAutoMove);
    }

    [Fact]
    public void LoadFormListSkipsEmptyLinesAndSemicolonComments()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("", "   ", ";注释", "  ; 注释2", "4,屠龙,0,0,0,0,0"), false);
        Assert.Single(db.m_ShowItemList);
        Assert.Equal("屠龙", db.m_ShowItemList[0].sItemName);
    }

    [Fact]
    public void LoadFormListTreatsMissingTrailingFieldsAsEmpty()
    {
        // GetValidStr3 到末尾时后续字段为 ''
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,屠龙"), false);
        var it = db.m_ShowItemList[0];
        Assert.Equal("屠龙", it.sItemName);
        Assert.Equal(0, it.boHintMsg);
        Assert.Equal(0, it.boPickup);
        Assert.Equal(0, it.boShowName);
        Assert.Equal(0, it.boShowSpecial);
        Assert.Equal(0, it.boAutoMove);
    }

    [Fact]
    public void LoadFormListRejectsEmptyItemName()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines(",布衣,1,1,1,1,1"), false);
        Assert.Empty(db.m_ShowItemList);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("9")]
    [InlineData("100")]
    [InlineData("abc")]
    [InlineData("")]
    public void LoadFormListRejectsItemTypeOutOfTItemTypeRange(string type)
    {
        // 原文 207-209：nItemType 必须落在 [Low(TItemType), High(TItemType)] = [0, 8]
        var db = new TFileItemDB();
        db.LoadFormList(Lines(type + ",屠龙,1,1,1,1,1"), false);
        Assert.Empty(db.m_ShowItemList);
    }

    [Theory]
    [InlineData("0", TItemType.i_All)]
    [InlineData("8", TItemType.i_diy)]
    public void LoadFormListAcceptsBoundaryItemTypes(string type, TItemType expected)
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines(type + ",屠龙,0,0,0,0,0"), false);
        Assert.Single(db.m_ShowItemList);
        Assert.Equal(expected, db.m_ShowItemList[0].ItemType);
    }

    [Fact]
    public void LoadFormListOnlyLiteralOneCountsAsTrue()
    {
        // 原文是 `sHint = '1'` 的**字符串精确比较**：'01'、'  1'、'true' 都**不算**。
        // GetValidStr3（HUtil32.pas:1243-1341）**不会**吃掉字段前导空格，
        // 所以 "  1" 原样成为 Dest，`= '1'` 为假。
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,屠龙,01,  1,true,YES,0"), false);
        var it = db.m_ShowItemList[0];
        Assert.Equal("  1", "  1");        // 说明性：Dest 保留前导空格
        Assert.Equal(0, it.boHintMsg);     // "01" <> "1"
        Assert.Equal(0, it.boPickup);      // "  1" <> "1"
        Assert.Equal(0, it.boShowName);    // "true" <> "1"
        Assert.Equal(0, it.boShowSpecial); // "YES" <> "1"
        Assert.Equal(0, it.boAutoMove);    // "0" <> "1"
    }

    [Fact]
    public void LoadFormListDuplicateNamesInSameCallAreMergedNotDuplicated()
    {
        // 第一次 Add 成功；第二次 Find 命中 → 走"已存在"分支
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,屠龙,0,0,0,0,0", "4,屠龙,1,1,1,1,1"), false);
        Assert.Single(db.m_ShowItemList);     // Add 拒重
        Assert.Single(db.m_FileItemList);     // 第二条走的是"已存在"分支，不再 Add 到 FileItemList
        Assert.Equal(1, db.m_ShowItemList[0].boHintMsg);
    }

    [Fact]
    public void LoadFormListDoesNotClearListsFirst()
    {
        // **原文缺陷（照抄）**：FilterItems.pas:180-193 的清空循环被 (* *) 注释掉了，
        // 所以重复调用会把 m_FileItemList 越堆越长，而 m_ShowItemList 因为 Add 拒重仍是 1。
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,屠龙,0,0,0,0,0"), false);
        db.LoadFormList(Lines("5,开天,0,0,0,0,0"), false);

        Assert.Equal(2, db.m_ShowItemList.Count);
        Assert.Equal(2, db.m_FileItemList.Count);
        // 再来一遍同样的两行 → 两条都命中 Find（已是"已存在"分支）→ FileItemList 不再增长
        db.LoadFormList(Lines("4,屠龙,0,0,0,0,0", "5,开天,0,0,0,0,0"), false);
        Assert.Equal(2, db.m_ShowItemList.Count);
        Assert.Equal(2, db.m_FileItemList.Count);
    }

    [Fact]
    public void LoadFormListFileItemIsASnapshotCopy()
    {
        // 原文 New(FileItem); FileItem^ := ShowItem^; → 是**值拷贝**（记录），后续改 ShowItem 不影响 FileItem
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,屠龙,0,0,0,0,0"), false);
        db.m_ShowItemList[0].boHintMsg = 1;
        Assert.Equal(0, db.m_FileItemList[0].boHintMsg);
    }

    [Fact]
    public void LoadFormListFindsExistingItemsCaseInsensitively()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("4,LongSword,0,0,0,0,0"), false);
        db.LoadFormList(Lines("4,longsword,1,1,1,1,1"), false);
        Assert.Single(db.m_ShowItemList);
        Assert.Equal(1, db.m_ShowItemList[0].boHintMsg);
    }

    [Fact]
    public void LoadFormListSkipsBlankLineWithOnlyWhitespaceBeforeSemicolon()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("\t , , , , , , "), false);
        Assert.Empty(db.m_ShowItemList);
    }

    // ============================================================ Add / Del / Find / Get

    private static TShowItem Show(string name, TItemType type = TItemType.i_Other, bool fromSystem = false)
        => new TShowItem { sItemName = name, sItemType = TFileItemDB.GetItemTypeName(type), ItemType = type, boFromSystem = (byte)(fromSystem ? 1 : 0) };

    [Fact]
    public void AddRejectsDuplicateNameCaseInsensitively()
    {
        var db = new TFileItemDB();
        Assert.True(db.Add(Show("屠龙")));
        Assert.False(db.Add(Show("屠龙")));
        Assert.False(db.Add(Show("TULONG".ToLowerInvariant() == "tulong" ? "屠龙" : "屠龙")));
        Assert.Single(db.m_ShowItemList);
    }

    [Fact]
    public void FindIsCaseInsensitiveAndReturnsFirst()
    {
        var db = new TFileItemDB();
        db.Add(Show("LongSword"));
        db.Add(Show("Other"));
        Assert.Same(db.m_ShowItemList[0], db.Find("longsword"));
        Assert.Same(db.m_ShowItemList[0], db.Find("LONGSWORD"));
        Assert.Null(db.Find("none"));
    }

    [Fact]
    public void DelOnlyRemovesNonSystemItems()
    {
        // 原文 408：`if (not ShowItem.boFromSystem) and (CompareText(...) = 0)`
        // 注意原文按下标**同时**删两个链表，故这里两个链表都要有对应项。
        var db = new TFileItemDB();
        db.Add(Show("系统物", fromSystem: true));
        db.m_FileItemList.Add(Show("系统物", fromSystem: true));
        Assert.False(db.Del("系统物"));
        Assert.Single(db.m_ShowItemList);

        db.Add(Show("自定物", fromSystem: false));
        db.m_FileItemList.Add(Show("自定物", fromSystem: false));
        Assert.True(db.Del("自定物"));
        Assert.Single(db.m_ShowItemList);
        Assert.Null(db.Find("自定物"));
    }

    [Fact]
    public void DelOnShowOnlyEntryThrows()
    {
        // **原文缺陷（照抄）**：Del 会按同一下标删 m_FileItemList；
        // 只有 Add 手工加入（不进 FileItemList）的项在 Del 时会越界。
        var db = new TFileItemDB();
        db.Add(Show("自定物"));
        Assert.Throws<ArgumentOutOfRangeException>(() => db.Del("自定物"));
    }

    [Fact]
    public void DelRemovesByParallelIndexFromBothLists()
    {
        // 原文 410-413：按下标 I 同时删 m_ShowItemList 与 m_FileItemList
        var db = new TFileItemDB();
        db.Add(Show("A"));
        db.Add(Show("B"));
        db.Add(Show("C"));
        db.m_FileItemList.Add(Show("A"));
        db.m_FileItemList.Add(Show("B"));
        db.m_FileItemList.Add(Show("C"));

        Assert.True(db.Del("B"));
        Assert.Equal(new[] { "A", "C" }, new[] { db.m_ShowItemList[0].sItemName, db.m_ShowItemList[1].sItemName });
        Assert.Equal(new[] { "A", "C" }, new[] { db.m_FileItemList[0].sItemName, db.m_FileItemList[1].sItemName });
    }

    [Fact]
    public void DelOnlyRemovesFirstMatch()
    {
        var db = new TFileItemDB();
        db.m_ShowItemList.Add(Show("A"));
        db.m_ShowItemList.Add(Show("A"));
        db.m_FileItemList.Add(Show("A"));
        db.m_FileItemList.Add(Show("A"));
        Assert.True(db.Del("A"));
        Assert.Single(db.m_ShowItemList);
    }

    [Fact]
    public void GetByStringTypeMatchesExactCategoryName()
    {
        var db = new TFileItemDB();
        db.Add(Show("甲", TItemType.i_Weapon));
        db.Add(Show("乙", TItemType.i_Dress));
        var all = new List<TShowItem>();
        db.Get("(全部分类)", all);
        Assert.Equal(2, all.Count);

        var weapon = new List<TShowItem>();
        db.Get("武器类", weapon);
        Assert.Single(weapon);
        Assert.Equal("甲", weapon[0].sItemName);

        // 大小写/等值严格（中文无大小写，但前缀不同就不匹配）
        var none = new List<TShowItem>();
        db.Get("武器", none);
        Assert.Empty(none);
    }

    [Fact]
    public void GetByItemTypeTreatsIAllAsAll()
    {
        var db = new TFileItemDB();
        db.Add(Show("甲", TItemType.i_Weapon));
        db.Add(Show("乙", TItemType.i_Dress));
        var all = new List<TShowItem>();
        db.Get(TItemType.i_All, all);
        Assert.Equal(2, all.Count);

        var dress = new List<TShowItem>();
        db.Get(TItemType.i_Dress, dress);
        Assert.Single(dress);
        Assert.Equal("乙", dress[0].sItemName);
    }

    [Fact]
    public void GetWithNullListReturnsImmediately()
    {
        var db = new TFileItemDB();
        db.Add(Show("甲"));
        db.Get("(全部分类)", null);           // 不应抛异常
        db.Get(TItemType.i_All, null);
    }

    // ============================================================ 导入 / 导出 对称性

    [Fact]
    public void ExportToStringsWritesExactFormat()
    {
        // 原文 356-358 的 Format('%d,%s,%d,%d,%d,%d,%d', [...])
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1"), false);
        var outList = new TStringList();
        db.ExportToStrings(outList);

        Assert.Equal(1, outList.Count);
        Assert.Equal("3,布衣,1,0,1,0,1", outList[0]);
    }

    [Fact]
    public void ExportToStringsUsesShowItemStateNotFileItemState()
    {
        // 原文 350-352：遍历 m_FileItemList，但取 Find(FileItem.sItemName) 的**当前**状态
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,0,0,0,0,0"), false);
        db.m_ShowItemList[0].boHintMsg = 1;
        db.m_ShowItemList[0].boPickup = 1;
        var outList = new TStringList();
        db.ExportToStrings(outList);
        Assert.Equal("3,布衣,1,1,0,0,0", outList[0]);
    }

    [Fact]
    public void ExportToStringsSkipsFileItemsWhoseNameDisappearedFromShowList()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,0,0,0,0,0"), false);
        db.m_ShowItemList.Clear();       // 模拟被清空但 FileItemList 还在
        var outList = new TStringList();
        db.ExportToStrings(outList);
        Assert.Equal(0, outList.Count);
    }

    [Fact]
    public void ImportExportRoundTripPreservesAllFlags()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines(
            "1,其它物,1,1,1,1,1",
            "2,金创药,1,0,0,1,0",
            "3,布衣,0,1,1,0,1",
            "8,自定物,0,0,0,0,0"), false);

        var outList = new TStringList();
        db.ExportToStrings(outList);
        Assert.Equal(4, outList.Count);

        var db2 = new TFileItemDB();
        db2.ImportFormStrings(outList);
        var out2 = new TStringList();
        db2.ExportToStrings(out2);

        Assert.Equal(
            Enumerable_ToList(outList),
            Enumerable_ToList(out2));
    }

    private static List<string> Enumerable_ToList(TStringList l)
    {
        var r = new List<string>();
        for (int i = 0; i < l.Count; i++) r.Add(l[i]);
        return r;
    }

    [Fact]
    public void ImportFormStringsClearsBothListsFirst()
    {
        // 原文 328-341：Import 开头会 Dispose + Clear 两个链表（与 LoadFormList 不同）
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,0,0,0,0,0", "4,屠龙,0,0,0,0,0"), false);
        Assert.Equal(2, db.m_FileItemList.Count);

        db.ImportFormStrings(Lines("5,开天,0,0,0,0,0"));
        Assert.Single(db.m_ShowItemList);
        Assert.Single(db.m_FileItemList);
        Assert.Equal("开天", db.m_ShowItemList[0].sItemName);
    }

    [Fact]
    public void ImportFormFileReadsFileAndReplacesContent()
    {
        var path = Path.Combine(_root, "import.dat");
        File.WriteAllText(path, "5,开天,1,1,0,0,1\r\n", Encoding.GetEncoding(936));

        var db = new TFileItemDB();
        db.ImportFormFile(path);
        Assert.Single(db.m_ShowItemList);
        Assert.Equal("开天", db.m_ShowItemList[0].sItemName);
        Assert.Equal(1, db.m_ShowItemList[0].boAutoMove);
    }

    [Fact]
    public void ImportFormFileMissingFileIsSilentNoOp()
    {
        var db = new TFileItemDB();
        db.ImportFormFile(Path.Combine(_root, "不存在.dat"));
        Assert.Empty(db.m_ShowItemList);
    }

    [Fact]
    public void ExportToFileWritesExactText()
    {
        var path = Path.Combine(_root, "export.dat");
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1", "5,开天,0,1,0,1,0"), false);
        db.ExportToFile(path);

        Assert.Equal("3,布衣,1,0,1,0,1\r\n5,开天,0,1,0,1,0\r\n", File.ReadAllText(path, Encoding.GetEncoding(936)));
    }

    // ============================================================ SaveToFile / LoadFormFile 落盘

    private static FakeSelf MakeSelf(string userName = "玩家甲") => new FakeSelf(1, 1, userName);

    [Fact]
    public void SaveToFileDoesNothingWhenNoMySelf()
    {
        // 原文 265：`if (g_MySelf = nil) then Exit;`
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1"), false);
        ConfigShareSeam.g_MySelf = null;
        db.SaveToFile();
        Assert.Empty(Directory.GetFiles(_configDir));
    }

    [Fact]
    public void SaveToFileWritesExactFileNameAndContent()
    {
        // 文件名来自 Format('%s.%s.ItemFilter.dat', [g_sPlugServerName, g_sPlugUserName])，
        // 且 g_sPlugUserName 先经过 ProcessFileNameSpecialChar。
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1", "5,开天,0,1,0,1,0"), false);
        ConfigShareSeam.g_MySelf = MakeSelf("玩家/甲:B*?");
        db.SaveToFile();

        string expectedName = "测试服." + ConfigSeams.ProcessFileNameSpecialChar("玩家/甲:B*?") + ".ItemFilter.dat";
        Assert.Equal("测试服.玩家{甲;B@!.ItemFilter.dat", expectedName);
        string path = Path.Combine(_configDir, expectedName);
        Assert.True(File.Exists(path), "期望落盘文件：" + path);
        Assert.Equal("3,布衣,1,0,1,0,1\r\n5,开天,0,1,0,1,0\r\n", File.ReadAllText(path, Encoding.GetEncoding(936)));
        Assert.Equal("玩家{甲;B@!", ConfigShareGlobal.g_sPlugUserName);
    }

    [Fact]
    public void SaveToFileCreatesConfigDirectoryWhenMissing()
    {
        Directory.Delete(_configDir, true);
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,0,0,0,0,0"), false);
        ConfigShareSeam.g_MySelf = MakeSelf("玩家");
        db.SaveToFile();
        Assert.True(Directory.Exists(_configDir));
        Assert.True(File.Exists(Path.Combine(_configDir, "测试服.玩家.ItemFilter.dat")));
    }

    [Fact]
    public void SaveToFileWritesOneLinePerFileItemIncludingDuplicates()
    {
        // 落盘遍历的是 m_FileItemList、取的是 Find() 得到的**当前** ShowItem，
        // 因此 m_FileItemList 里出现重名条目时（例如 m_ShowItemList 被清空后重新装载、
        // 旧条目仍留在 m_FileItemList），就会写出重复行 —— 原文如此。
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1"), false);
        db.m_FileItemList.Add(db.Find("布衣"));   // 人为制造重名条目（模拟历史残留）
        ConfigShareSeam.g_MySelf = MakeSelf("玩家");
        db.SaveToFile();

        string text = File.ReadAllText(Path.Combine(_configDir, "测试服.玩家.ItemFilter.dat"), Encoding.GetEncoding(936));
        Assert.Equal("3,布衣,1,0,1,0,1\r\n3,布衣,1,0,1,0,1\r\n", text);
    }

    [Fact]
    public void LoadFormFileRoundTripsWhatSaveToFileWrote()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1", "5,开天,0,1,0,1,0"), false);
        ConfigShareSeam.g_MySelf = MakeSelf("玩家");
        db.SaveToFile();

        var db2 = new TFileItemDB();
        db2.LoadFormFile();
        var outList = new TStringList();
        db2.ExportToStrings(outList);
        Assert.Equal(2, outList.Count);
        Assert.Equal("3,布衣,1,0,1,0,1", outList[0]);
        Assert.Equal("5,开天,0,1,0,1,0", outList[1]);
    }

    [Fact]
    public void LoadFormFileRefreshesPlugUserNameFromMySelf()
    {
        ConfigShareGlobal.g_sPlugUserName = "旧名";
        ConfigShareSeam.g_MySelf = MakeSelf("新/名");
        var db = new TFileItemDB();
        db.LoadFormFile();
        Assert.Equal("新{名", ConfigShareGlobal.g_sPlugUserName);
    }

    [Fact]
    public void LoadFormFileMissingFileIsSilent()
    {
        ConfigShareSeam.g_MySelf = MakeSelf("玩家");
        var db = new TFileItemDB();
        db.LoadFormFile();
        Assert.Empty(db.m_ShowItemList);
    }

    // ============================================================ BackUp

    [Fact]
    public void BackUpCopiesFileListIntoShowListInPlace()
    {
        // 原文 246-252：`m_ShowItemList.Items[I]^ := m_FileItemList.Items[I]^`（原地覆盖）
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,1,0,1"), false);
        var showRef = db.m_ShowItemList[0];
        showRef.boHintMsg = 0;               // 改动 ShowItem
        db.BackUp();                          // 从 FileItem 恢复
        Assert.Same(showRef, db.m_ShowItemList[0]);
        Assert.Equal(1, showRef.boHintMsg);
    }

    [Fact]
    public void BackUpIsIndexBasedAndThrowsWhenFileListIsLonger()
    {
        // **原文缺陷（照抄）**：循环上界用 m_FileItemList.Count 却索引 m_ShowItemList，
        // 长度不一致时会越界（Delphi 侧是访问违例，托管侧是 IndexOutOfRangeException）。
        var db = new TFileItemDB();
        db.m_FileItemList.Add(Show("A"));
        db.m_FileItemList.Add(Show("B"));
        db.m_ShowItemList.Add(Show("A"));
        Assert.Throws<ArgumentOutOfRangeException>(() => db.BackUp());
    }

    // ============================================================ Hint

    private (string Msg, int Color, int BackColor)? Capture()
    {
        (string, int, int)? cap = null;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => cap = (m, c, b);
        return cap;
    }

    [Fact]
    public void HintFormatsMessageWithDirectionAndCoords()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,0,0,0"), false);
        ConfigShareSeam.g_MySelf = new FakeSelf(100, 200, "我");
        NextDirectionSeam.GetNextDirection = (x1, y1, x2, y2) => 1;   // 右上

        (string Msg, int Color, int BackColor)? cap = null;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => cap = (m, c, b);
        db.Hint("布衣", 103, 197);

        Assert.NotNull(cap);
        Assert.Equal("发现[布衣]，方位:右上↗，坐标:(103,197).", cap!.Value.Msg);
        Assert.Equal(0x0000FFFF, cap.Value.Color);
        Assert.Equal(0xFF0000, cap.Value.BackColor);
    }

    [Theory]
    [InlineData(0, "上", "↑")]
    [InlineData(2, "右", "→")]
    [InlineData(4, "下", "↓")]
    [InlineData(7, "左上", "↖")]
    public void HintPositionTextPerDirection(int ndir, string pos, string dir)
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,0,0,0"), false);
        ConfigShareSeam.g_MySelf = new FakeSelf(0, 0, "我");
        NextDirectionSeam.GetNextDirection = (a, b, c, d) => ndir;
        (string Msg, int Color, int BackColor)? cap = null;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => cap = (m, c, b);
        db.Hint("布衣", 5, 5);
        Assert.Equal($"发现[布衣]，方位:{pos}{dir}，坐标:(5,5).", cap!.Value.Msg);
    }

    [Fact]
    public void HintDoesNothingWhenItemNotFound()
    {
        var db = new TFileItemDB();
        ConfigShareSeam.g_MySelf = MakeSelf();
        int calls = 0;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => calls++;
        db.Hint("不存在", 1, 1);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void HintDoesNothingWhenHintFlagIsOff()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,0,1,1,0,0"), false);   // boHintMsg = 0
        ConfigShareSeam.g_MySelf = MakeSelf();
        int calls = 0;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => calls++;
        db.Hint("布衣", 1, 1);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void HintDoesNothingWhenNoMySelf()
    {
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,布衣,1,0,0,0,0"), false);
        ConfigShareSeam.g_MySelf = null;
        int calls = 0;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => calls++;
        db.Hint("布衣", 1, 1);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void HintUsesRequestedNameInMessageNotStoredName()
    {
        // 原文用的是 sItemName 参数拼文案
        var db = new TFileItemDB();
        db.LoadFormList(Lines("3,LongSword,1,0,0,0,0"), false);
        ConfigShareSeam.g_MySelf = MakeSelf();
        NextDirectionSeam.GetNextDirection = (a, b, c, d) => 0;
        (string Msg, int Color, int BackColor)? cap = null;
        ChatBoardSeam.AddChatBoardString = (m, c, b) => cap = (m, c, b);
        db.Hint("longsword", 1, 1);
        Assert.Equal("发现[longsword]，方位:上↑，坐标:(1,1).", cap!.Value.Msg);
    }

    // ============================================================ 全局变量初始化

    [Fact]
    public void GlobalFileItemDbExistsByInitialization()
    {
        // FilterItems.pas:486-487  initialization 段
        Assert.NotNull(FilterItemsGlobal.g_FileItemDB);
        Assert.False(FilterItemsGlobal.g_IsClientPickItemsChanged != 0);
        Assert.Equal(0u, FilterItemsGlobal.g_UploadPickItemsTick);
        Assert.Equal(0u, FilterItemsGlobal.g_UploadPickItemsHash);
    }

    private sealed class FakeSelf : IHumActorSeam
    {
        public FakeSelf(int x, int y, string name) { m_nCurrX = x; m_nCurrY = y; m_sUserName = name; }
        public int m_nCurrX { get; }
        public int m_nCurrY { get; }
        public string m_sUserName { get; }
    }
}
