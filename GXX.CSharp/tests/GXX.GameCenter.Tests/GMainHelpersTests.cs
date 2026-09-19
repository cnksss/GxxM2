using System;
using System.Collections.Generic;
using System.IO;
using GXX.GameCenter;
using Xunit;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GMain.pas 辅助过程段（581-864 + 5730-5845）与备份清单持久化段（1446-1540）测试。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class GMainHelpersTests : GameCenterTestBase
{
    // ==================================================================
    // ListBoxAdd / ListBoxDel（GMain.pas:581-606）
    // ==================================================================

    [Fact]
    public void ListBoxAdd_AppendsNewItem()
    {
        var box = new MemoryListBox();
        GMainHelpers.ListBoxAdd(box, @"D:\a.txt");
        Assert.Equal(new[] { @"D:\a.txt" }, box.Items);
        Assert.Null(GameCenterDialogs.LastMessage);
    }

    [Fact]
    public void ListBoxAdd_DuplicateShowsExactMessageAndSkips()
    {
        var box = new MemoryListBox();
        GMainHelpers.ListBoxAdd(box, @"D:\a.txt");
        GMainHelpers.ListBoxAdd(box, @"D:\a.txt");

        Assert.Single(box.Items);
        Assert.Equal("此文件路径已在列表中，请重新选择！！", GameCenterDialogs.LastMessage);
        Assert.Equal("提示信息", GameCenterDialogs.LastCaption);
    }

    [Fact]
    public void ListBoxAdd_EmptyStringIsAllowedOnce()
    {
        var box = new MemoryListBox();
        GMainHelpers.ListBoxAdd(box, "");
        GMainHelpers.ListBoxAdd(box, "");
        Assert.Single(box.Items);
        Assert.Equal("此文件路径已在列表中，请重新选择！！", GameCenterDialogs.LastMessage);
    }

    [Fact]
    public void ListBoxDel_RemovesSelectedItemOnly()
    {
        var box = new MemoryListBox();
        box.Add("a"); box.Add("b"); box.Add("c");
        box.SelectedIndex = 1;
        GMainHelpers.ListBoxDel(box);
        Assert.Equal(new[] { "a", "c" }, box.Items);
    }

    [Fact]
    public void ListBoxDel_NoSelectionKeepsItems()
    {
        var box = new MemoryListBox();
        box.Add("a");
        box.SelectedIndex = -1;
        GMainHelpers.ListBoxDel(box);
        Assert.Equal(new[] { "a" }, box.Items);
    }

    // ==================================================================
    // ClearModValue / ClearTxt（GMain.pas:608-622）
    // ==================================================================

    [Fact]
    public void ClearModValue_InvokesHostHandler()
    {
        int calls = 0;
        GMainHelpers.ClearModValueHandler = () => calls++;
        GMainHelpers.ClearModValue();
        Assert.Equal(1, calls);
    }

    [Fact]
    public void ClearModValue_WithoutHandler_IsNoOp()
    {
        GMainHelpers.ClearModValueHandler = null;
        GMainHelpers.ClearModValue();  // 不抛异常
    }

    [Fact]
    public void ClearTxt_TruncatesExistingFile()
    {
        string f = Under("x.txt");
        File.WriteAllText(f, "content");
        GMainHelpers.ClearTxt(f);
        Assert.Equal("", File.ReadAllText(f));
    }

    [Fact]
    public void ClearTxt_CreatesMissingFile()
    {
        string f = Under("new.txt");
        GMainHelpers.ClearTxt(f);
        Assert.True(File.Exists(f));
        Assert.Equal("", File.ReadAllText(f));
    }

    [Fact]
    public void ClearTxt_DirectoryWithSameName_ThrowsLikeDelphiRewrite()
    {
        string dir = Under("adir");
        Directory.CreateDirectory(dir);
        // Delphi rewrite(目录) 抛 I/O 错误；托管侧 File 写入目录名抛 UnauthorizedAccessException。
        Assert.ThrowsAny<Exception>(() => GMainHelpers.ClearTxt(dir));
    }

    // ==================================================================
    // Clear_SaveConfig / Clear_LoadConfig（GMain.pas:626-688）
    // ==================================================================

    private sealed class ListStore
    {
        public readonly Dictionary<GMainHelpers.ListSource, List<string>> Items = new();
        public ListStore()
        {
            foreach (GMainHelpers.ListSource s in Enum.GetValues<GMainHelpers.ListSource>())
                Items[s] = new List<string>();
        }
        public void Wire()
        {
            GMainHelpers.ListCountProvider = s => Items[s].Count;
            GMainHelpers.ListItemProvider = (s, i) => Items[s][i];
            GMainHelpers.ListClearHandler = s => Items[s].Clear();
            GMainHelpers.ListAddHandler = (s, v) => Items[s].Add(v);
        }
    }

    [Fact]
    public void Clear_SaveConfig_WritesThreeCountsAndIndexedKeys()
    {
        var store = new ListStore();
        store.Items[GMainHelpers.ListSource.MyGetTxt].AddRange(new[] { "t0", "t1" });
        store.Items[GMainHelpers.ListSource.MyGetFile].Add("f0");
        store.Items[GMainHelpers.ListSource.MyGetDir].AddRange(new[] { "d0", "d1", "d2" });
        store.Wire();

        Assert.True(GMainHelpers.Clear_SaveConfig());
        // 原文 g_IniConf 为 FastIniFile，内存写入后由应用退出/显式 UpdateFile 落盘；
        // 管理侧 g_IniConf 同为"内存缓冲 + UpdateFile"语义（见 GameCenterIniFile）。
        g_IniConf.UpdateFile();

        using var ini = new GameCenterIniFile(Path.Combine(Dir, "Config.ini"));
        Assert.Equal(2, ini.ReadInteger("ClearServer", "MyGetTxtNum", -1));
        Assert.Equal("t0", ini.ReadString("ClearServer", "MyGetTxt0", ""));
        Assert.Equal("t1", ini.ReadString("ClearServer", "MyGetTxt1", ""));
        Assert.Equal(1, ini.ReadInteger("ClearServer", "MyGetFileNum", -1));
        Assert.Equal("f0", ini.ReadString("ClearServer", "MyGetFile0", ""));
        Assert.Equal(3, ini.ReadInteger("ClearServer", "MyGetDirNum", -1));
        Assert.Equal("d2", ini.ReadString("ClearServer", "MyGetDir2", ""));
    }

    [Fact]
    public void Clear_SaveConfig_EmptyListsWriteZeroCountsAndNoIndexedKeys()
    {
        new ListStore().Wire();

        Assert.True(GMainHelpers.Clear_SaveConfig());
        // 原文 g_IniConf 为 FastIniFile，内存写入后由应用退出/显式 UpdateFile 落盘；
        // 管理侧 g_IniConf 同为"内存缓冲 + UpdateFile"语义（见 GameCenterIniFile）。
        g_IniConf.UpdateFile();

        using var ini = new GameCenterIniFile(Path.Combine(Dir, "Config.ini"));
        Assert.Equal(0, ini.ReadInteger("ClearServer", "MyGetTxtNum", -1));
        Assert.Equal(0, ini.ReadInteger("ClearServer", "MyGetFileNum", -1));
        Assert.Equal(0, ini.ReadInteger("ClearServer", "MyGetDirNum", -1));
        Assert.Equal("D", ini.ReadString("ClearServer", "MyGetTxt0", "D"));
    }

    [Fact]
    public void Clear_LoadConfig_ReadsBackIntoThreeLists()
    {
        var store = new ListStore();
        store.Wire();
        // 直接写 g_IniConf（原文 FastIniFile 是同一实例：写内存 → 读内存）。
        g_IniConf.WriteInteger("ClearServer", "MyGetTxtNum", 1);
        g_IniConf.WriteString("ClearServer", "MyGetTxt0", "T0");
        g_IniConf.WriteInteger("ClearServer", "MyGetFileNum", 2);
        g_IniConf.WriteString("ClearServer", "MyGetFile0", "F0");
        g_IniConf.WriteString("ClearServer", "MyGetFile1", "F1");
        g_IniConf.WriteInteger("ClearServer", "MyGetDirNum", 1);
        g_IniConf.WriteString("ClearServer", "MyGetDir0", "D0");

        GMainHelpers.Clear_LoadConfig();

        Assert.Equal(new[] { "T0" }, store.Items[GMainHelpers.ListSource.MyGetTxt]);
        Assert.Equal(new[] { "F0", "F1" }, store.Items[GMainHelpers.ListSource.MyGetFile]);
        Assert.Equal(new[] { "D0" }, store.Items[GMainHelpers.ListSource.MyGetDir]);
    }

    [Fact]
    public void Clear_LoadConfig_MissingIndexedKeyUsesPasDefaultText()
    {
        var store = new ListStore();
        store.Wire();
        g_IniConf.WriteInteger("ClearServer", "MyGetTxtNum", 2);
        g_IniConf.WriteString("ClearServer", "MyGetTxt0", "T0");
        // MyGetTxt1 缺失 → 默认 '读取配置文件错误'

        GMainHelpers.Clear_LoadConfig();

        Assert.Equal(new[] { "T0", "读取配置文件错误" }, store.Items[GMainHelpers.ListSource.MyGetTxt]);
    }

    [Fact]
    public void Clear_LoadConfig_AllZeroCounts_LeavesListsUntouched()
    {
        var store = new ListStore();
        store.Items[GMainHelpers.ListSource.MyGetTxt].Add("existing");
        store.Wire();

        GMainHelpers.Clear_LoadConfig();

        Assert.Equal(new[] { "existing" }, store.Items[GMainHelpers.ListSource.MyGetTxt]);
    }

    [Fact]
    public void Clear_LoadConfig_ClearsListOnlyWhenCountNonZero()
    {
        var store = new ListStore();
        store.Items[GMainHelpers.ListSource.MyGetTxt].Add("existing");
        store.Wire();
        g_IniConf.WriteInteger("ClearServer", "MyGetTxtNum", 1);
        g_IniConf.WriteString("ClearServer", "MyGetTxt0", "T0");

        GMainHelpers.Clear_LoadConfig();

        Assert.Equal(new[] { "T0" }, store.Items[GMainHelpers.ListSource.MyGetTxt]);
    }

    // ==================================================================
    // ClearSetupIni（GMain.pas:759-839）
    // ==================================================================

    private string SetupTxtPath => Under("Mir200", "!setup.txt");

    private void SeedSetupTxt()
    {
        Directory.CreateDirectory(Under("Mir200"));
        File.WriteAllText(SetupTxtPath,
            "[Setup]\r\n" +
            "HighLevel=1\r\n" +
            "HighLevelGetExp=2\r\n" +
            "MaxUpLevelCount=3\r\n" +
            "LimitChangeExp=4\r\n" +
            "IncAlcoholTime=5\r\n" +
            "DecDrinkTime=6\r\n" +
            "MaxAlcoholValue=7\r\n" +
            "IncAlcoholValue=8\r\n" +
            "DecMedicineValue=9\r\n" +
            "DecMedicineTime=10\r\n" +
            "GlobalVal0=1\r\n" +
            "GlobalStrVal0=x\r\n" +
            "ServerName=KeepMe\r\n" +
            "[Exp]\r\n" +
            "KillMonExpMultiple=1\r\n" +
            "BaseExp=2\r\n" +
            "AddExp=3\r\n" +
            "UseFixExp=1\r\n" +
            "HighLevelKillMonFixExp=1\r\n" +
            "HighLevelGroupFixExp=1\r\n" +
            "Level1=1\r\n" +
            "LevelExpRate1=1\r\n" +
            "SomeOtherKey=keep\r\n" +
            "[HeroExp]\r\n" +
            "Level1=1\r\n" +
            "[MedicineExp]\r\n" +
            "Level1=1\r\n" +
            "[WineExp]\r\n" +
            "Level1=1\r\n",
            GXX.Core.EncodingInit.GBK);
    }

    [Fact]
    public void ClearSetupIni_RemovesLevelKeysAndErasesEmptySections()
    {
        SeedSetupTxt();

        GMainHelpers.ClearSetupIni();

        using var ini = new GameCenterIniFile(SetupTxtPath);
        // Exp 节保留了非等级键
        Assert.Equal("keep", ini.ReadString("Exp", "SomeOtherKey", ""));
        Assert.Equal("", ini.ReadString("Exp", "Level1", ""));
        Assert.Equal("", ini.ReadString("Exp", "LevelExpRate1", ""));
        Assert.Equal("", ini.ReadString("Exp", "BaseExp", ""));
        // HeroExp/MedicineExp/WineExp 被删空后整节擦除
        Assert.False(ini.SectionExists("HeroExp"));
        Assert.False(ini.SectionExists("MedicineExp"));
        Assert.False(ini.SectionExists("WineExp"));
        // Setup 的等级/全局值与酒类键被删，ServerName 保留
        Assert.Equal("", ini.ReadString("Setup", "HighLevel", ""));
        Assert.Equal("", ini.ReadString("Setup", "GlobalVal0", ""));
        Assert.Equal("", ini.ReadString("Setup", "GlobalStrVal0", ""));
        Assert.Equal("", ini.ReadString("Setup", "IncAlcoholTime", ""));
        Assert.Equal("KeepMe", ini.ReadString("Setup", "ServerName", ""));
    }

    [Fact]
    public void ClearSetupIni_MissingFileIsNoOp()
    {
        Assert.False(File.Exists(SetupTxtPath));
        GMainHelpers.ClearSetupIni();   // 不抛异常、不建文件
        Assert.False(File.Exists(SetupTxtPath));
    }

    [Fact]
    public void ClearSetupIni_Level1000IsDeletedBut1001IsNot()
    {
        Directory.CreateDirectory(Under("Mir200"));
        File.WriteAllText(SetupTxtPath,
            "[Exp]\r\nLevel1000=1\r\nLevel1001=2\r\nLevelExpRate1000=3\r\nKeep=1\r\n",
            GXX.Core.EncodingInit.GBK);

        GMainHelpers.ClearSetupIni();

        using var ini = new GameCenterIniFile(SetupTxtPath);
        Assert.Equal("", ini.ReadString("Exp", "Level1000", ""));
        Assert.Equal("2", ini.ReadString("Exp", "Level1001", ""));
        Assert.Equal("", ini.ReadString("Exp", "LevelExpRate1000", ""));
        Assert.Equal(1000, GMainHelpers.MAXCHANGELEVEL);
    }

    [Fact]
    public void ClearSetupIni_GlobalVal499DeletedBut500Not()
    {
        Directory.CreateDirectory(Under("Mir200"));
        File.WriteAllText(SetupTxtPath,
            "[Setup]\r\nGlobalVal499=1\r\nGlobalVal500=2\r\nGlobalStrVal499=1\r\nGlobalStrVal500=2\r\nKeep=3\r\n",
            GXX.Core.EncodingInit.GBK);

        GMainHelpers.ClearSetupIni();

        using var ini = new GameCenterIniFile(SetupTxtPath);
        Assert.Equal("", ini.ReadString("Setup", "GlobalVal499", ""));
        Assert.Equal("2", ini.ReadString("Setup", "GlobalVal500", ""));
        Assert.Equal("", ini.ReadString("Setup", "GlobalStrVal499", ""));
        Assert.Equal("2", ini.ReadString("Setup", "GlobalStrVal500", ""));
    }

    // ==================================================================
    // ClearGlobal（GMain.pas:842-864）
    // ==================================================================

    [Fact]
    public void ClearGlobal_Writes1000IntAnd1000StringKeys()
    {
        string f = Under("g.txt");
        Assert.True(GMainHelpers.ClearGlobal(f));

        string[] lines = ReadLinesGbk(f);
        // [Setup] + 1000 GlobalVal + 1000 GlobalStrVal + 尾部空行
        Assert.Equal(1 + 1000 + 1000 + 1, lines.Length);
        Assert.Equal("[Setup]", lines[0]);
        Assert.Equal("GlobalVal0=0", lines[1]);
        Assert.Equal("GlobalVal999=0", lines[1000]);
        Assert.Equal("GlobalStrVal0=", lines[1001]);
        Assert.Equal("GlobalStrVal999=", lines[2000]);
    }

    [Fact]
    public void ClearGlobal_OverwritesExistingValuesByAppendingDuplicates()
    {
        string f = Under("g.txt");
        File.WriteAllText(f, "[Setup]\r\nGlobalVal0=9\r\nKeep=1\r\n", GXX.Core.EncodingInit.GBK);

        GMainHelpers.ClearGlobal(f);

        using var ini = new GameCenterIniFile(f);
        // 新写入的 0 在文件末尾，读回为最后值
        Assert.Equal(0, ini.ReadInteger("Setup", "GlobalVal0", -1));
        Assert.Equal("1", ini.ReadString("Setup", "Keep", ""));
    }

    // ==================================================================
    // 清库 SQL（GMain.pas:5730-5845）
    // ==================================================================

    [Fact]
    public void GetClearAccountDBSql_IsSingleDeleteFromAccount()
    {
        Assert.Equal("delete from Account;", GMainClearSql.GetClearAccountDBSql());
    }

    [Fact]
    public void GetClearRoleDataDBSql_HasFortyTwoStatementsInOrder()
    {
        string[] stmts = GMainClearSql.GetClearRoleDataDBSql()
            .Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(42, stmts.Length);
        Assert.Equal("delete from HeroItemElementAdd;", stmts[0]);
        Assert.Equal("delete from Hero;", stmts[18]);
        Assert.Equal("delete from HumanItemElementAdd;", stmts[19]);
        Assert.Equal("delete from Human;", stmts[41]);
    }

    [Fact]
    public void GetClearUserShopSql_TargetsItemType1AndUserShop()
    {
        string sql = GMainClearSql.GetClearUserShopSql();
        Assert.Contains("delete from Items where ItemType = 1;", sql);
        Assert.Contains("delete from UserShopItem;", sql);
        Assert.Contains("delete from UserShop;", sql);
        Assert.EndsWith("delete from UserShop;\r\n", sql);
    }

    [Fact]
    public void GetClearStorageExSql_TargetsItemType0AndStorageEx()
    {
        string sql = GMainClearSql.GetClearStorageExSql();
        Assert.Contains("delete from Items where ItemType = 0;", sql);
        Assert.EndsWith("delete from StorageEx;\r\n", sql);
    }

    [Fact]
    public void GetClearAuctionDataSql_TargetsItemType5AndAuctionTables()
    {
        string sql = GMainClearSql.GetClearAuctionDataSql();
        Assert.Contains("delete from Items where ItemType = 5;", sql);
        Assert.EndsWith("delete from AuctionAttention;\r\ndelete from AuctionData;\r\n", sql);
    }

    [Fact]
    public void ClearSql_DifferentTypesAreDistinct()
    {
        // 差异断言：三条按 ItemType 区分的语句必须不同（0 / 1 / 5）。
        Assert.NotEqual(GMainClearSql.GetClearUserShopSql(), GMainClearSql.GetClearStorageExSql());
        Assert.NotEqual(GMainClearSql.GetClearUserShopSql(), GMainClearSql.GetClearAuctionDataSql());
        Assert.NotEqual(GMainClearSql.GetClearStorageExSql(), GMainClearSql.GetClearAuctionDataSql());
    }

    // ==================================================================
    // SaveBackList / LoadBackList / RefBackListToView（GMain.pas:1446-1540）
    // ==================================================================

    [Fact]
    public void SaveBackList_WritesOneSectionPerTaskWithExactKeyOrder()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        manager.Add(new MemoryBackUpTask
        {
            SourceDirectory = @"D:\S1\", DestDirectory = @"D:\D1\",
            Hour = 1, Min = 2, Mode = 3, Start = true, IsCompress = false,
        });
        manager.Add(new MemoryBackUpTask
        {
            SourceDirectory = @"D:\S2\", DestDirectory = @"D:\D2\",
            Hour = 4, Min = 5, Mode = 6, Start = false, IsCompress = true,
        });

        GMainBackList.SaveBackList();

        AssertLines(GMainConfig.BackListFileName(),
            "[0]",
            @"Source=D:\S1\",
            @"Save=D:\D1\",
            "Hour=1",
            "Min=2",
            "BackMode=3",
            "GetBack=1",
            "IsCompress=0",
            "",
            "[1]",
            @"Source=D:\S2\",
            @"Save=D:\D2\",
            "Hour=4",
            "Min=5",
            "BackMode=6",
            "GetBack=0",
            "IsCompress=1",
            "");
    }

    [Fact]
    public void SaveBackList_EmptyList_WritesEmptyFile()
    {
        g_BackUpManager = new MemoryBackUpManager();
        GMainBackList.SaveBackList();
        Assert.Equal("", ReadGbk(GMainConfig.BackListFileName()));
    }

    [Fact]
    public void SaveBackList_DeletesPreviousFileFirst()
    {
        g_BackUpManager = new MemoryBackUpManager();
        File.WriteAllText(GMainConfig.BackListFileName(), "[9]\r\nSource=stale\r\n", GXX.Core.EncodingInit.GBK);
        GMainBackList.SaveBackList();
        Assert.Equal("", ReadGbk(GMainConfig.BackListFileName()));
    }

    [Fact]
    public void LoadBackList_CreatesTasksForCompleteSectionsOnly()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        GMainBackList.BackUpTaskFactory = () => new MemoryBackUpTask();
        File.WriteAllText(GMainConfig.BackListFileName(),
            "[0]\r\nSource=S0\r\nSave=D0\r\nHour=1\r\nMin=2\r\nBackMode=3\r\nGetBack=1\r\nIsCompress=1\r\n" +
            "\r\n" +
            "[1]\r\nSource=S1\r\nSave=\r\nHour=1\r\nMin=1\r\n" +
            "\r\n" +
            "[2]\r\nSource=\r\nSave=D2\r\nHour=1\r\nMin=1\r\n",
            GXX.Core.EncodingInit.GBK);

        GMainBackList.LoadBackList();

        Assert.Single(manager.m_BackUpList);
        var t = (MemoryBackUpTask)manager.m_BackUpList[0];
        Assert.Equal("S0", t.SourceDirectory);
        Assert.Equal("D0", t.DestDirectory);
        Assert.Equal((ushort)1, t.Hour);
        Assert.Equal((ushort)2, t.Min);
        Assert.Equal((byte)3, t.Mode);
        Assert.True(t.Start);
        Assert.True(t.IsCompress);
    }

    [Fact]
    public void LoadBackList_DefaultsAreGetBackTrueAndIsCompressTrue()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        GMainBackList.BackUpTaskFactory = () => new MemoryBackUpTask();
        File.WriteAllText(GMainConfig.BackListFileName(), "[0]\r\nSource=S\r\nSave=D\r\n",
            GXX.Core.EncodingInit.GBK);

        GMainBackList.LoadBackList();

        var t = (MemoryBackUpTask)manager.m_BackUpList[0];
        Assert.True(t.Start);
        Assert.True(t.IsCompress);
        Assert.Equal((ushort)0, t.Hour);
        Assert.Equal((byte)0, t.Mode);
    }

    [Fact]
    public void LoadBackList_MissingFile_AddsNothingAndDisablesButtons()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        GMainBackList.BackUpTaskFactory = () => new MemoryBackUpTask();
        var disabled = new List<GMainBackList.BackListButton>();
        GMainBackList.SetButtonEnabled = (b, enabled) => { if (!enabled) disabled.Add(b); };
        GMainConfig.DeleteBackListFile();

        GMainBackList.LoadBackList();

        Assert.Empty(manager.m_BackUpList);
        Assert.Equal(new[] { GMainBackList.BackListButton.ButtonBackDel, GMainBackList.BackListButton.ButtonBackChg }, disabled);
    }

    [Fact]
    public void LoadBackList_HourAboveWordRangeWrapsLikeDelphiTruncation()
    {
        // Delphi 中 wHour: Word := ReadInteger(...) 会按 16 位截断（65536 → 0）。
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        GMainBackList.BackUpTaskFactory = () => new MemoryBackUpTask();
        File.WriteAllText(GMainConfig.BackListFileName(), "[0]\r\nSource=S\r\nSave=D\r\nHour=65537\r\n",
            GXX.Core.EncodingInit.GBK);

        GMainBackList.LoadBackList();

        Assert.Equal((ushort)1, ((MemoryBackUpTask)manager.m_BackUpList[0]).Hour);
    }

    [Fact]
    public void RefBackListToView_EmitsOneRowPerTaskWithStartStopText()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        manager.Add(new MemoryBackUpTask { SourceDirectory = "S1", DestDirectory = "D1", Start = true, BackUpCount = 3, FailCount = 1 });
        manager.Add(new MemoryBackUpTask { SourceDirectory = "S2", DestDirectory = "D2", Start = false, BackUpCount = 0, FailCount = 0 });

        int cleared = 0;
        var rows = new List<string>();
        GMainBackList.ClearListView = () => cleared++;
        GMainBackList.AddListViewRow = (cap, sub, task, cnt, fail, state) =>
            rows.Add($"{cap}|{sub}|{cnt}|{fail}|{state}");

        GMainBackList.RefBackListToView();

        Assert.Equal(1, cleared);
        Assert.Equal(new[] { "S1|D1|3|1|启动", "S2|D2|0|0|停止" }, rows);
    }

    [Fact]
    public void RefBackListToView_EmptyList_ClearsOnly()
    {
        g_BackUpManager = new MemoryBackUpManager();
        int cleared = 0;
        GMainBackList.ClearListView = () => cleared++;
        GMainBackList.AddListViewRow = (_, _, _, _, _, _) => throw new InvalidOperationException("should not add");

        GMainBackList.RefBackListToView();

        Assert.Equal(1, cleared);
    }
}
