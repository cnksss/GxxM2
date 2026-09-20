using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Misc;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：SellPlayer.pas（TSellPlayerList）1:1 测试。
/// 覆盖：ShortString 字节截断、二分 Search 的插入位语义、增删排序、INI 往返（含原文缺陷）、
/// AutoLoadSellPlayer 的加锁/打包/日志。
/// </summary>
[Collection("SellPlayerLane")]
public sealed class SellPlayerTests : SellPlayerTestBase
{
    // ------------------------------------------------------------------
    // SellPlayerShortStr / TSellPlayerInfo（原文 :11-21）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData("abc", 10, "abc")]                       // 未超容量 → 原样
    [InlineData("0123456789", 10, "0123456789")]         // 正好等于容量
    [InlineData("0123456789ABC", 10, "0123456789")]      // 超容量 → 截断
    [InlineData("", 10, "")]
    public void ShortStr_Trunc_AsciiByByteCapacity(string input, int capacity, string expected)
        => Assert.Equal(expected, SellPlayerShortStr.Trunc(input, capacity));

    [Fact]
    public void ShortStr_Trunc_ChineseNameTruncatesByGbkBytesNotChars()
    {
        // 中文在 GBK 下 2 字节/字：14 字节 = 7 个汉字；"十二个汉字测试名" 共 8 字 → 截 14 字节
        string name = "一二三四五六七八";                 // 8 字 = 16 字节
        string truncated = SellPlayerShortStr.Trunc(name, Grobal2Const.ACTOR_NAME_LEN);
        Assert.Equal("一二三四五六七", truncated);        // 7 字 = 14 字节
        Assert.Equal(14, System.Text.Encoding.GetEncoding(936).GetByteCount(truncated));
    }

    [Fact]
    public void ShortStr_Trunc_OddByteBoundarySplitsDoubleByteChar()
    {
        // 容量 13（奇数）会把第 7 个汉字切成半个 → GBK 解码出替换符，与 Delphi 存半字节一致（不可读）
        string name = "一二三四五六七";                   // 14 字节
        string truncated = SellPlayerShortStr.Trunc(name, 13);
        Assert.NotEqual(name, truncated);
        Assert.True(truncated.Length <= 7);
    }

    [Fact]
    public void ShortStr_Trunc_NonPositiveCapacityOrNullYieldsEmpty()
    {
        Assert.Equal("", SellPlayerShortStr.Trunc("abc", 0));
        Assert.Equal("", SellPlayerShortStr.Trunc("abc", -1));
        Assert.Equal("", SellPlayerShortStr.Trunc(null!, 10));
    }

    [Fact]
    public void SellPlayerInfo_DefaultsMatchFillCharZero()
    {
        var info = new TSellPlayerInfo();                 // 原文 :102 FillChar(Info^, ..., 0)
        Assert.Equal("", info.Account);
        Assert.Equal("", info.Player);
        Assert.Equal("", info.Delegater);
        Assert.Equal("", info.SetUserName);
        Assert.Equal(0, info.SellPricesType);
        Assert.Equal(0, info.SellPrices);
        Assert.Equal(0.0, info.SellTime);
        Assert.False(info.SetUser);
        Assert.False(info.IsBuying);
    }

    [Fact]
    public void SellPlayerInfo_AccountSetterTruncatesToAccountLen()
    {
        var info = new TSellPlayerInfo();
        info.Account = "0123456789ABCDEF";                 // ACCOUNT_LEN = 10
        Assert.Equal("0123456789", info.Account);
    }

    [Fact]
    public void SellPlayerInfo_ActorNameFieldsTruncateToActorNameLen()
    {
        var info = new TSellPlayerInfo();
        info.Player = "0123456789ABCDEF";
        info.Delegater = "0123456789ABCDEF";
        info.SetUserName = "0123456789ABCDEF";
        Assert.Equal("0123456789ABCD", info.Player);       // ACTOR_NAME_LEN = 14
        Assert.Equal("0123456789ABCD", info.Delegater);
        Assert.Equal("0123456789ABCD", info.SetUserName);
    }

    // ------------------------------------------------------------------
    // 构造 / Count / Items（原文 :54-59、:81-89）
    // ------------------------------------------------------------------

    [Fact]
    public void Ctor_StartsEmpty()
    {
        var list = new TSellPlayerList();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Ctor_IniFileNameIsEnvirDirPlusSellPlayerIni()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acc", "hero1", "deleg", 0, 100, 0, false, "");
        list.SaveConfig();
        Assert.True(File.Exists(Path.Combine(Dir, "SellPlayer.ini")));
    }

    [Fact]
    public void Ctor_EnvirDirWithoutTrailingSeparator_ConcatenatesLiterally_OriginalTrap()
    {
        // 原文 :58 `g_Config.sEnvirDir + 'SellPlayer.ini'` 是**裸字符串相加**（不是 Path.Combine）：
        // sEnvirDir 少一个尾部分隔符时，文件名会拼到目录名**后面**（Dir\sub + "SellPlayer.ini"
        // = Dir\subSellPlayer.ini），而不是落到 Dir\sub\ 里面。
        string sub = Path.Combine(Dir, "sub");
        Directory.CreateDirectory(sub);
        M2Config.sEnvirDir = sub;                          // 无尾部分隔符
        var list = new TSellPlayerList();
        list.AddSellPlayer("acc", "hero1", "deleg", 0, 100, 0, false, "");
        list.SaveConfig();
        Assert.True(File.Exists(sub + "SellPlayer.ini"));
        Assert.False(File.Exists(Path.Combine(sub, "SellPlayer.ini")));
    }

    [Fact]
    public void GetItems_AndIndexer_ReturnSameInstance()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acc", "hero1", "deleg", 1, 500, 0, false, "");
        Assert.Same(list[0], list.GetItems(0));
        Assert.Equal("hero1", list[0].Player);
    }

    [Fact]
    public void Indexer_OutOfRange_ThrowsManagedException()
    {
        // 偏离 D-P8-2：Delphi 未开范围检查时 TList.Items[] 越界读垃圾指针（不抛），托管必抛
        var list = new TSellPlayerList();
        Assert.Throws<ArgumentOutOfRangeException>(() => list[0]);
    }

    [Fact]
    public void Indexer_NegativeIndex_ThrowsManagedException()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acc", "hero1", "deleg", 1, 500, 0, false, "");
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
    }

    // ------------------------------------------------------------------
    // Clear（原文 :68-79）
    // ------------------------------------------------------------------

    [Fact]
    public void Clear_RemovesAllEntries()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "n1", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("b", "n2", "d", 0, 1, 0, false, "");
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Clear_OnEmptyList_IsNoOp()
    {
        var list = new TSellPlayerList();
        list.Clear();
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Clear_ThenAdd_WorksAgain()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "n1", "d", 0, 1, 0, false, "");
        list.Clear();
        Assert.True(list.AddSellPlayer("b", "n2", "d", 0, 1, 0, false, ""));
        Assert.Equal(1, list.Count);
        Assert.Equal("n2", list[0].Player);
    }

    // ------------------------------------------------------------------
    // Search（原文 :118-145）—— 二分 + 插入位语义
    // ------------------------------------------------------------------

    [Fact]
    public void Search_EmptyList_ReturnsFalseAndInsertPositionZero()
    {
        var list = new TSellPlayerList();
        Assert.False(list.Search("anyone", out int index));
        Assert.Equal(0, index);
    }

    [Fact]
    public void Search_Hit_ReturnsTrueAndHitIndex()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "aaa", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("b", "bbb", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("c", "ccc", "d", 0, 1, 0, false, "");
        Assert.True(list.Search("bbb", out int index));
        Assert.Equal(1, index);
    }

    [Fact]
    public void Search_CaseInsensitiveLikeAnsiCompareText()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "HeroOne", "d", 0, 1, 0, false, "");
        Assert.True(list.Search("heroone", out int index));
        Assert.Equal(0, index);
        Assert.True(list.Search("HEROONE", out _));
    }

    [Fact]
    public void Search_Miss_ReturnsInsertionPointKeepingSortOrder()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "bbb", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("b", "ddd", "d", 0, 1, 0, false, "");
        Assert.False(list.Search("ccc", out int index));
        Assert.Equal(1, index);                            // 应插在 bbb 与 ddd 之间
    }

    [Fact]
    public void Search_MissBelowFirstElement_InsertionPointZero()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "mmm", "d", 0, 1, 0, false, "");
        Assert.False(list.Search("aaa", out int index));
        Assert.Equal(0, index);
    }

    [Fact]
    public void Search_MissAboveLastElement_InsertionPointEqualsCount()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "mmm", "d", 0, 1, 0, false, "");
        Assert.False(list.Search("zzz", out int index));
        Assert.Equal(1, index);
    }

    [Fact]
    public void Search_EvenCountMidpointFormula_MatchesDelphiPrecedence()
    {
        // 覆盖原文 :129 `L + (H - L) shr 1`：Delphi 里 shr 先于 +，等价 L + ((H-L) shr 1)；
        // C# 里 >> 后于 +，必须显式括号。用 6 项（偶数）走遍两侧分支。
        var list = new TSellPlayerList();
        foreach (string n in new[] { "a", "b", "c", "d", "e", "f" })
            list.AddSellPlayer("acc", n, "deleg", 0, 1, 0, false, "");
        Assert.Equal(6, list.Count);
        for (int i = 0; i < 6; i++)
        {
            Assert.True(list.Search(((char)('a' + i)).ToString(), out int hit));
            Assert.Equal(i, hit);
        }
    }

    // ------------------------------------------------------------------
    // AddSellPlayer（原文 :91-116）
    // ------------------------------------------------------------------

    [Fact]
    public void AddSellPlayer_NewName_ReturnsTrueAndStoresAllFields()
    {
        var list = new TSellPlayerList();
        Assert.True(list.AddSellPlayer("acct", "hero", "deleg", 3, 8888, 45000.5, true, "buyer"));
        Assert.Equal(1, list.Count);
        var info = list[0];
        Assert.Equal("acct", info.Account);
        Assert.Equal("hero", info.Player);
        Assert.Equal("deleg", info.Delegater);
        Assert.Equal(3, info.SellPricesType);
        Assert.Equal(8888, info.SellPrices);
        Assert.Equal(45000.5, info.SellTime);
        Assert.True(info.SetUser);
        Assert.Equal("buyer", info.SetUserName);
        Assert.False(info.IsBuying);                       // :112 恒 False
    }

    [Fact]
    public void AddSellPlayer_DuplicateName_ReturnsFalseAndDoesNotInsert()
    {
        var list = new TSellPlayerList();
        Assert.True(list.AddSellPlayer("a", "hero", "d1", 0, 1, 0, false, ""));
        Assert.False(list.AddSellPlayer("b", "HERO", "d2", 0, 2, 0, false, ""));
        Assert.Equal(1, list.Count);
        Assert.Equal("a", list[0].Account);                // 先到者保留
    }

    [Fact]
    public void AddSellPlayer_KeepsListSortedByPlayerName()
    {
        var list = new TSellPlayerList();
        foreach (string n in new[] { "ddd", "aaa", "ccc", "bbb" })
            list.AddSellPlayer("acc", n, "deleg", 0, 1, 0, false, "");
        Assert.Equal(new[] { "aaa", "bbb", "ccc", "ddd" },
            new[] { list[0].Player, list[1].Player, list[2].Player, list[3].Player });
    }

    [Fact]
    public void AddSellPlayer_TruncatesShortStringFields()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("0123456789XYZ", "0123456789ABCDEF", "0123456789ABCDEF", 0, 1, 0, false, "0123456789ABCDEF");
        Assert.Equal("0123456789", list[0].Account);
        Assert.Equal("0123456789ABCD", list[0].Player);
        Assert.Equal("0123456789ABCD", list[0].Delegater);
        Assert.Equal("0123456789ABCD", list[0].SetUserName);
    }

    // ------------------------------------------------------------------
    // DeleteByIndex（原文 :147-154）
    // ------------------------------------------------------------------

    [Fact]
    public void DeleteByIndex_RemovesTheRightEntry()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "aaa", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("b", "bbb", "d", 0, 1, 0, false, "");
        list.DeleteByIndex(1);
        Assert.Equal(1, list.Count);
        Assert.Equal("aaa", list[0].Player);
    }

    [Fact]
    public void DeleteByIndex_FirstEntry_KeepsOrder()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "aaa", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("b", "bbb", "d", 0, 1, 0, false, "");
        list.DeleteByIndex(0);
        Assert.Equal("bbb", list[0].Player);
    }

    [Fact]
    public void DeleteByIndex_OutOfRange_Throws()
    {
        var list = new TSellPlayerList();
        Assert.Throws<ArgumentOutOfRangeException>(() => list.DeleteByIndex(0));
    }

    // ------------------------------------------------------------------
    // DeletePlayer / DeletePlayerEx（原文 :156-191）
    // ------------------------------------------------------------------

    [Fact]
    public void DeletePlayer_MatchingDelegater_RemovesAndSaves()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "Deleg", 0, 100, 0, false, "");
        Assert.True(list.DeletePlayer("hero", "deleg"));   // SameText：大小写不敏感
        Assert.Equal(0, list.Count);
        var ini = new TFastIniFile(Path.Combine(Dir, "SellPlayer.ini"));
        Assert.Equal(0, ini.ReadInteger("setup", "count", -1));   // SaveConfig 已被调用
    }

    [Fact]
    public void DeletePlayer_NonMatchingDelegater_KeepsEntryAndDoesNotSave()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "deleg", 0, 100, 0, false, "");
        Assert.False(list.DeletePlayer("hero", "other"));
        Assert.Equal(1, list.Count);
        Assert.False(File.Exists(Path.Combine(Dir, "SellPlayer.ini")));
    }

    [Fact]
    public void DeletePlayer_UnknownPlayer_ReturnsFalse()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "deleg", 0, 100, 0, false, "");
        Assert.False(list.DeletePlayer("nobody", "deleg"));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void DeletePlayerEx_RemovesRegardlessOfDelegater()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "delegA", 0, 100, 0, false, "");
        Assert.True(list.DeletePlayerEx("hero"));
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void DeletePlayerEx_UnknownPlayer_ReturnsFalse()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "delegA", 0, 100, 0, false, "");
        Assert.False(list.DeletePlayerEx("nobody"));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void DeletePlayerEx_CaseInsensitiveAndSaves()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "Hero", "delegA", 0, 100, 0, false, "");
        Assert.True(list.DeletePlayerEx("hERO"));
        var ini = new TFastIniFile(Path.Combine(Dir, "SellPlayer.ini"));
        Assert.Equal(0, ini.ReadInteger("setup", "count", -1));
    }

    // ------------------------------------------------------------------
    // SaveConfig / LoadConfig（原文 :193-264）
    // ------------------------------------------------------------------

    [Fact]
    public void SaveConfig_WritesCountAndAllKeysInOrder()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct", "hero", "deleg", 2, 777, 0, true, "buyer");
        list.SaveConfig();

        var ini = new TFastIniFile(Path.Combine(Dir, "SellPlayer.ini"));
        Assert.Equal(1, ini.ReadInteger("setup", "count", -1));
        Assert.Equal("acct", ini.ReadString("1", "Account", ""));
        Assert.Equal("hero", ini.ReadString("1", "Player", ""));
        Assert.Equal("deleg", ini.ReadString("1", "Delegater", ""));
        Assert.Equal(2, ini.ReadInteger("1", "MoneyType", -1));
        Assert.Equal(777, ini.ReadInteger("1", "SellPrices", -1));
        Assert.True(ini.ReadBool("1", "SetUser", false));
        Assert.Equal("buyer", ini.ReadString("1", "SetUserName", ""));
    }

    [Fact]
    public void SaveConfig_WritesFixedDateTimeFormat()
    {
        var list = new TSellPlayerList();
        // 2023-12-25 13:45:07 的 OLE 日期
        double dt = new DateTime(2023, 12, 25, 13, 45, 7).ToOADate();
        list.AddSellPlayer("a", "hero", "deleg", 0, 1, dt, false, "");
        list.SaveConfig();
        var ini = new TFastIniFile(Path.Combine(Dir, "SellPlayer.ini"));
        Assert.Equal("25-12-2023 13:45:07", ini.ReadString("1", "SellTime", ""));
    }

    [Fact]
    public void SaveConfig_EmptyList_WritesZeroCountOnly()
    {
        var list = new TSellPlayerList();
        list.SaveConfig();
        var ini = new TFastIniFile(Path.Combine(Dir, "SellPlayer.ini"));
        Assert.Equal(0, ini.ReadInteger("setup", "count", -1));
        Assert.False(ini.SectionExists("1"));
    }

    [Fact]
    public void SaveConfig_DoesNotClearOldSections_OriginalFlawPreserved()
    {
        // 原文 :243 `// IniFile.Clear;` 被注释掉 → 列表收缩后陈旧节仍在文件里（原文瑕疵，逐字保留）
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "aaa", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("b", "bbb", "d", 0, 1, 0, false, "");
        list.SaveConfig();

        list.DeleteByIndex(1);
        list.SaveConfig();

        var ini = new TFastIniFile(Path.Combine(Dir, "SellPlayer.ini"));
        Assert.Equal(1, ini.ReadInteger("setup", "count", -1));
        Assert.True(ini.SectionExists("2"));               // 陈旧节保留（原文如此）
    }

    [Fact]
    public void SaveConfig_SwallowsWriteErrors()
    {
        var blocker = Path.Combine(Dir, "blocker");
        File.WriteAllText(blocker, "x");
        M2Config.sEnvirDir = blocker + Path.DirectorySeparatorChar;   // 把它当目录用 → 建目录/写文件必失败
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "d", 0, 1, 0, false, "");
        list.SaveConfig();                                  // 原文 :262-263 裸 except → 不抛
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void LoadConfig_RoundTripsSavedEntries()
    {
        var list = new TSellPlayerList();
        double dt = new DateTime(2021, 6, 1, 8, 30, 0).ToOADate();
        list.AddSellPlayer("acct", "hero", "deleg", 1, 300, dt, true, "buyer");
        list.SaveConfig();

        var loaded = new TSellPlayerList();
        loaded.LoadConfig();
        Assert.Equal(1, loaded.Count);
        Assert.Equal("acct", loaded[0].Account);
        Assert.Equal("hero", loaded[0].Player);
        Assert.Equal("deleg", loaded[0].Delegater);
        Assert.Equal(1, loaded[0].SellPricesType);
        Assert.Equal(300, loaded[0].SellPrices);
        Assert.Equal(dt, loaded[0].SellTime);
        Assert.True(loaded[0].SetUser);
        Assert.Equal("buyer", loaded[0].SetUserName);
    }

    [Fact]
    public void LoadConfig_MissingFile_ClearsAndStaysEmpty()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("a", "hero", "d", 0, 1, 0, false, "");
        list.LoadConfig();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void LoadConfig_SkipsRowsFailingValidation()
    {
        string file = Path.Combine(Dir, "SellPlayer.ini");
        var ini = new TFastIniFile(file);
        ini.WriteInteger("setup", "count", 5);
        // 1: 合法
        ini.WriteString("1", "Player", "good1"); ini.WriteString("1", "Delegater", "d");
        ini.WriteInteger("1", "MoneyType", 0); ini.WriteInteger("1", "SellPrices", 10);
        // 2: Player 为空 → 跳过
        ini.WriteString("2", "Player", ""); ini.WriteString("2", "Delegater", "d");
        ini.WriteInteger("2", "MoneyType", 0); ini.WriteInteger("2", "SellPrices", 10);
        // 3: Delegater 为空 → 跳过
        ini.WriteString("3", "Player", "p3"); ini.WriteString("3", "Delegater", "");
        ini.WriteInteger("3", "MoneyType", 0); ini.WriteInteger("3", "SellPrices", 10);
        // 4: MoneyType=5 越界 → 跳过
        ini.WriteString("4", "Player", "p4"); ini.WriteString("4", "Delegater", "d");
        ini.WriteInteger("4", "MoneyType", 5); ini.WriteInteger("4", "SellPrices", 10);
        // 5: SellPrices=0 → 跳过
        ini.WriteString("5", "Player", "p5"); ini.WriteString("5", "Delegater", "d");
        ini.WriteInteger("5", "MoneyType", 0); ini.WriteInteger("5", "SellPrices", 0);
        ini.UpdateFile();

        var list = new TSellPlayerList();
        list.LoadConfig();
        Assert.Equal(1, list.Count);
        Assert.Equal("good1", list[0].Player);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(4, true)]                                  // 边界：MoneyType <= 4 含 4
    [InlineData(5, false)]
    [InlineData(-1, false)]
    public void LoadConfig_MoneyTypeValidationBoundary(int moneyType, bool accepted)
    {
        string file = Path.Combine(Dir, "SellPlayer.ini");
        var ini = new TFastIniFile(file);
        ini.WriteInteger("setup", "count", 1);
        ini.WriteString("1", "Player", "p");
        ini.WriteString("1", "Delegater", "d");
        ini.WriteInteger("1", "MoneyType", moneyType);
        ini.WriteInteger("1", "SellPrices", 10);
        ini.UpdateFile();

        var list = new TSellPlayerList();
        list.LoadConfig();
        Assert.Equal(accepted ? 1 : 0, list.Count);
    }

    [Fact]
    public void LoadConfig_EmptySetUserName_ForcesSetUserFalse()
    {
        string file = Path.Combine(Dir, "SellPlayer.ini");
        var ini = new TFastIniFile(file);
        ini.WriteInteger("setup", "count", 1);
        ini.WriteString("1", "Player", "p");
        ini.WriteString("1", "Delegater", "d");
        ini.WriteInteger("1", "MoneyType", 0);
        ini.WriteInteger("1", "SellPrices", 10);
        ini.WriteBool("1", "SetUser", true);
        ini.WriteString("1", "SetUserName", "");            // 空 → :222-223 强制 false
        ini.UpdateFile();

        var list = new TSellPlayerList();
        list.LoadConfig();
        Assert.Equal(1, list.Count);
        Assert.False(list[0].SetUser);
    }

    [Fact]
    public void LoadConfig_CountLargerThanPresentSections_UsesDefaults()
    {
        string file = Path.Combine(Dir, "SellPlayer.ini");
        var ini = new TFastIniFile(file);
        ini.WriteInteger("setup", "count", 3);              // 只提供 1 节
        ini.WriteString("1", "Player", "p1");
        ini.WriteString("1", "Delegater", "d");
        ini.WriteInteger("1", "SellPrices", 10);
        ini.UpdateFile();

        var list = new TSellPlayerList();
        list.LoadConfig();
        Assert.Equal(1, list.Count);                        // 缺失节读成 '' → 校验不过 → 跳过
    }

    [Fact]
    public void LoadConfig_NegativeCount_LoadsNothing()
    {
        string file = Path.Combine(Dir, "SellPlayer.ini");
        var ini = new TFastIniFile(file);
        ini.WriteInteger("setup", "count", -5);             // for I := 0 to Count-1 → 不执行
        ini.WriteString("1", "Player", "p1");
        ini.WriteString("1", "Delegater", "d");
        ini.WriteInteger("1", "SellPrices", 10);
        ini.UpdateFile();

        var list = new TSellPlayerList();
        list.LoadConfig();
        Assert.Equal(0, list.Count);
    }

    // ------------------------------------------------------------------
    // AutoLoadSellPlayer（原文 :266-305）
    // ------------------------------------------------------------------

    [Fact]
    public void AutoLoad_EmptyList_DoesNothing()
    {
        SellPlayerGlobals.MyGetTickCount = () => 123u;
        var list = new TSellPlayerList();
        list.AutoLoadSellPlayer();
        Assert.Empty(SellPlayerGlobals.m_AutoLoadSellPlayerList);
        Assert.False(SellPlayerGlobals.m_boStartAutoLoadSellPlayer);
        Assert.Empty(SellPlayerGlobals.ListLockCalls);
        Assert.Empty(M2ServerLog.Messages);
    }

    [Fact]
    public void AutoLoad_PacksEntriesIntoAutoLoadList()
    {
        SellPlayerGlobals.MyGetTickCount = () => 4242u;
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct1", "hero1", "d", 0, 1, 0, false, "");
        list.AddSellPlayer("acct2", "hero2", "d", 0, 1, 0, false, "");
        list.AutoLoadSellPlayer();

        Assert.Equal(2, SellPlayerGlobals.m_AutoLoadSellPlayerList.Count);
        var first = SellPlayerGlobals.m_AutoLoadSellPlayerList[0];
        Assert.Equal("acct1", first.sAccount);
        Assert.Equal("hero1", first.sCharName);
        Assert.False(first.boStartLogin);                   // :288
        Assert.Equal(4242u, first.dwStartLoginTick);        // :289 MyGetTickCount
        Assert.Null(first.SessInfo);                        // :290
        Assert.False(first.boPassWordSuccess);              // 原文 New 未清零该字段（FillChar 只在 AddSellPlayer 里用）
    }

    [Fact]
    public void AutoLoad_SetsStartFlagAndLogsMessage()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct", "hero", "d", 0, 1, 0, false, "");
        list.AutoLoadSellPlayer();
        Assert.True(SellPlayerGlobals.m_boStartAutoLoadSellPlayer);   // :300
        Assert.Contains("正在登录出售角色...", M2ServerLog.Messages);  // :303
    }

    [Fact]
    public void AutoLoad_MultiThreadRun_LocksWithArgTwoAndUnlocks()
    {
        SellPlayerGlobals.g_MultiThreadRun = true;
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct", "hero", "d", 0, 1, 0, false, "");
        list.AutoLoadSellPlayer();

        Assert.Equal(2, SellPlayerGlobals.ListLockCalls.Count);
        Assert.Equal(("LockW", 2), SellPlayerGlobals.ListLockCalls[0]);      // :278 LockW(2)
        Assert.Equal(("UnLockW", 0), SellPlayerGlobals.ListLockCalls[1]);    // :297
    }

    [Fact]
    public void AutoLoad_SingleThread_DoesNotLock()
    {
        SellPlayerGlobals.g_MultiThreadRun = false;
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct", "hero", "d", 0, 1, 0, false, "");
        list.AutoLoadSellPlayer();
        Assert.Empty(SellPlayerGlobals.ListLockCalls);
    }

    [Fact]
    public void AutoLoad_CalledTwice_AppendsAgainWithoutClearing()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct", "hero", "d", 0, 1, 0, false, "");
        list.AutoLoadSellPlayer();
        list.AutoLoadSellPlayer();
        Assert.Equal(2, SellPlayerGlobals.m_AutoLoadSellPlayerList.Count);   // 原文不清空接缝列表
    }

    [Fact]
    public void AutoLoad_FlagFalseWhenSeamListEmptiedExternally()
    {
        var list = new TSellPlayerList();
        list.AddSellPlayer("acct", "hero", "d", 0, 1, 0, false, "");
        list.AutoLoadSellPlayer();
        Assert.True(SellPlayerGlobals.m_boStartAutoLoadSellPlayer);
        SellPlayerGlobals.m_AutoLoadSellPlayerList.Clear();
        Assert.True(SellPlayerGlobals.m_boStartAutoLoadSellPlayer);          // 标志只在方法内刷新
    }

    // ------------------------------------------------------------------
    // g_SellPlayerList 接线：**接缝臆造修正**（M2ShareFuncs.cs，车道 p8-m2-itemprop-misc 请求 #1）
    //   原文 M2Share.pas:8416 `g_SellPlayerList: TSellPlayerList;`
    //   原托管实现是 List<string> + SearchSellPlayer（IndexOf 语义）—— 类型不符，已改为本类。
    // ------------------------------------------------------------------

    [Fact]
    public void M2ShareGlobals_SellPlayerList_IsTheRealTSellPlayerList()
    {
        M2ShareGlobals.g_SellPlayerList.Clear();
        Assert.IsType<TSellPlayerList>(M2ShareGlobals.g_SellPlayerList);
    }

    [Fact]
    public void M2ShareGlobals_SellPlayerList_SupportsRecordAccessAndDeletion()
    {
        var list = M2ShareGlobals.g_SellPlayerList;
        list.Clear();
        Assert.True(list.AddSellPlayer("acc", "寄售乙", "deleg", 2, 500, 0, false, ""));
        Assert.True(list.Search("寄售乙", out int index));       // 原文二分查找（= ViewOnlineHuman.pas:481 用法）
        Assert.Equal("acc", list[index].Account);                // Items[I] 记录访问：List<string> 表达不了
        list.DeleteByIndex(index);                               // UsrEngn.pas:2331/2840 用法
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void M2ShareGlobals_SellPlayerList_NoLongerExposesStringListShim()
    {
        // 防复发：SearchSellPlayer（IndexOf 语义的替身）必须不存在，字段类型必须是 TSellPlayerList
        var t = typeof(M2ShareGlobals);
        Assert.Null(t.GetMethod("SearchSellPlayer"));
        var field = t.GetField("g_SellPlayerList");
        Assert.NotNull(field);
        Assert.Equal(typeof(TSellPlayerList), field!.FieldType);
    }
}

/// <summary>
/// 车道 p8-m2-itemprop-misc：SellPlayer.pas 的 FastIniFile 固定日期时间缺口测试
/// （SellPlayerIni：ReadFixedDateTime / WriteFixedDateTime / StrToDateDef / StrToTimeDef）。
/// </summary>
[Collection("SellPlayerLane")]
public sealed class SellPlayerIniTests : SellPlayerTestBase
{
    private TFastIniFile NewIni() => new(Path.Combine(Dir, "dt.ini"));

    // ------------------------------------------------------------------
    // 常量（FastIniFile.pas:189-194）
    // ------------------------------------------------------------------

    [Fact]
    public void FixedFormatConstants_MatchFastIniFile()
    {
        Assert.Equal('-', SellPlayerIni.FIXED_DS);
        Assert.Equal(':', SellPlayerIni.FIXED_TS);
        Assert.Equal("dd-mm-yyyy", SellPlayerIni.FIXED_DATE);
        Assert.Equal("hh:nn:ss", SellPlayerIni.FIXED_TIME);
        Assert.Equal("dd-mm-yyyy hh:nn:ss", SellPlayerIni.FIXED_DATETIME);
        Assert.Equal(10, SellPlayerIni.FIXED_DATE.Length);
        Assert.Equal(8, SellPlayerIni.FIXED_TIME.Length);
    }

    // ------------------------------------------------------------------
    // StrToDateDef / StrToTimeDef（FastIniFile.pas:988-1024）
    // ------------------------------------------------------------------

    [Fact]
    public void StrToDateDef_ValidFixedFormat_ParsesWithDashSeparator()
        => Assert.Equal(new DateTime(2023, 12, 25).ToOADate(), SellPlayerIni.StrToDateDef("25-12-2023", -1));

    [Fact]
    public void StrToDateDef_WrongSeparator_ReturnsDefault()
        => Assert.Equal(-1.0, SellPlayerIni.StrToDateDef("25/12/2023", -1));

    [Fact]
    public void StrToDateDef_Garbage_ReturnsDefault()
        => Assert.Equal(-1.0, SellPlayerIni.StrToDateDef("not-a-date", -1));

    [Fact]
    public void StrToTimeDef_ValidFixedFormat_ParsesWithColonSeparator()
        => Assert.Equal(new TimeSpan(13, 45, 7).TotalDays, SellPlayerIni.StrToTimeDef("13:45:07", 0));

    [Fact]
    public void StrToTimeDef_Garbage_ReturnsDefaultZero()
        => Assert.Equal(0.0, SellPlayerIni.StrToTimeDef("xx:yy:zz", 0));

    [Fact]
    public void StrToTimeDef_OutOfRangeHour_ReturnsDefault()
        => Assert.Equal(0.0, SellPlayerIni.StrToTimeDef("99:00:00", 0));

    // ------------------------------------------------------------------
    // WriteFixedDateTime（FastIniFile.pas:2985-2989）
    // ------------------------------------------------------------------

    [Fact]
    public void FormatFixedDateTime_ZeroIsDelphiEpoch()
        => Assert.Equal("30-12-1899 00:00:00", SellPlayerIni.FormatFixedDateTime(0));

    [Fact]
    public void FormatFixedDateTime_KnownValue()
        => Assert.Equal("25-12-2023 13:45:07",
            SellPlayerIni.FormatFixedDateTime(new DateTime(2023, 12, 25, 13, 45, 7).ToOADate()));

    [Fact]
    public void FormatFixedDateTime_OutOfOleRange_YieldsEmpty()
    {
        Assert.Equal("", SellPlayerIni.FormatFixedDateTime(double.NaN));
        Assert.Equal("", SellPlayerIni.FormatFixedDateTime(1e12));
        Assert.Equal("", SellPlayerIni.FormatFixedDateTime(-1e12));
    }

    // ------------------------------------------------------------------
    // ReadFixedDateTime（FastIniFile.pas:2953-2971）—— 逐分支
    // ------------------------------------------------------------------

    [Fact]
    public void ReadFixedDateTime_ValidValue_ReturnsDatePlusTime()
    {
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023 13:45:07");
        Assert.Equal(new DateTime(2023, 12, 25, 13, 45, 7).ToOADate(),
            SellPlayerIni.ReadFixedDateTime(ini, "S", "V", 0));
    }

    [Fact]
    public void ReadFixedDateTime_NoSpace_ReturnsDefaultWithoutParsingDate()
    {
        // 分支 1：Pos(' ', S) = 0 → 连日期都不解析（原文 :2964）
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023");
        Assert.Equal(-7.0, SellPlayerIni.ReadFixedDateTime(ini, "S", "V", -7));
    }

    [Fact]
    public void ReadFixedDateTime_MissingKey_ReturnsDefault()
    {
        var ini = NewIni();
        Assert.Equal(-3.0, SellPlayerIni.ReadFixedDateTime(ini, "S", "absent", -3));
    }

    [Fact]
    public void ReadFixedDateTime_BadDate_ReturnsDefault()
    {
        // 分支 2：日期段 StrToDateDef 失败 → -1 → 整体返回 Default
        var ini = NewIni();
        ini.WriteString("S", "V", "99-99-9999 13:45:07");
        Assert.Equal(-5.0, SellPlayerIni.ReadFixedDateTime(ini, "S", "V", -5));
    }

    [Fact]
    public void ReadFixedDateTime_BadTime_IsSilentlyAcceptedAsMidnight_OriginalFlaw()
    {
        // 分支 3（原文瑕疵）：StrToTimeDef 的 Default 是 0 而判据是 `T <> -1` → 时间解析失败仍被接受
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023 garbage!");
        Assert.Equal(new DateTime(2023, 12, 25).ToOADate(),
            SellPlayerIni.ReadFixedDateTime(ini, "S", "V", -1));
    }

    [Fact]
    public void ReadFixedDateTime_WritesThenReads_RoundTrips()
    {
        var ini = NewIni();
        double v = new DateTime(2019, 1, 2, 3, 4, 5).ToOADate();
        SellPlayerIni.WriteFixedDateTime(ini, "S", "V", v);
        Assert.Equal("02-01-2019 03:04:05", ini.ReadString("S", "V", ""));
        Assert.Equal(v, SellPlayerIni.ReadFixedDateTime(ini, "S", "V", 0));
    }

    [Fact]
    public void ReadFixedDateTime_DefaultValueUsedWhenEntryEmpty()
    {
        var ini = NewIni();
        ini.WriteString("S", "V", "");
        Assert.Equal(12.5, SellPlayerIni.ReadFixedDateTime(ini, "S", "V", 12.5));
    }
}

/// <summary>
/// 车道 p8-m2-itemprop-misc：SellPlayer.pas 测试基类（隔离 M2Config.sEnvirDir 与静态接缝）。
/// </summary>
[CollectionDefinition("SellPlayerLane", DisableParallelization = true)]
public sealed class SellPlayerLaneCollection
{
}

/// <summary>SellPlayer 测试基类：临时 Envir 目录 + 接缝复位。</summary>
public abstract class SellPlayerTestBase : IDisposable
{
    /// <summary>本次用例的临时 Envir 目录（SellPlayer.ini 落盘处）。</summary>
    protected readonly string Dir;

    private readonly string _savedEnvirDir;

    protected SellPlayerTestBase()
    {
        Dir = Path.Combine(Path.GetTempPath(), "p8sellplayer_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        _savedEnvirDir = M2Config.sEnvirDir;
        M2Config.sEnvirDir = Dir + Path.DirectorySeparatorChar;              // 原文 g_Config.sEnvirDir + 'SellPlayer.ini'
        SellPlayerGlobals.Reset();
    }

    public void Dispose()
    {
        M2Config.sEnvirDir = _savedEnvirDir;
        SellPlayerGlobals.Reset();
        try { Directory.Delete(Dir, true); } catch { /* 临时目录清理失败不影响门禁 */ }
    }
}
