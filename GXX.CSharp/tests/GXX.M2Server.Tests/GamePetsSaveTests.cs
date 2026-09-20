using System.Linq;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.GamePets;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 保存族 1:1 覆盖：
///   :1149-1182 btnSaveExpClick（网格 → 经验表 + 5 个 Setup 键 + 1000 行 GamePetExp）
///   :830-912   btnSavePetParamsClick（29~30 个 Setup 键 + SendServerConfig）
///   M2Share.pas:32759-32833 SaveGamePetsConfig（先删后建 + 40 键/只）
///   M2Share.pas:32678-32757 LoadGamePetsConfig（count/name 门控 + 缺省值）
///
/// 差异断言重点：
///   · btnSaveExpClick 的 `dwExp > High(LongWord)` 是 **uint.MaxValue** 边界，不是 int
///   · 解析失败（空串/Garbage）走 `StrToInt64Def(..., 0)` → **0**，不抛
///   · SaveGamePetsConfig 是"先 DeleteFile 再重建"（不是覆盖写），且写盘后不留旧键
///   · LoadGamePetsConfig 用 `Name.Length > 0` 门控，空 name 的节**整节丢弃**
/// </summary>
[Collection("GamePetsSerial")]
public sealed class GamePetsSaveTests : GamePetsTestBase
{
    private string PetConfigFile => Path.Combine(Dir, "GamePetConfigs.txt");

    // ========================================================================
    //  :1149-1182 btnSaveExpClick
    // ========================================================================

    [Fact]
    public void SaveExp_ReadsWholeGridIntoPetNeedExps()
    {
        OpenWith();
        SetGridCell(1, 1, "111");
        SetGridCell(2, 1, "222");
        SetGridCell(1000, 1, "4294967295");   // uint.MaxValue 恰好允许

        Form.RaiseSaveExp();

        Assert.Equal(111u, M2Config.dwPetNeedExps[1]);
        Assert.Equal(222u, M2Config.dwPetNeedExps[2]);
        Assert.Equal(uint.MaxValue, M2Config.dwPetNeedExps[1000]);
        Assert.Empty(Messages);
    }

    [Fact]
    public void SaveExp_ValueAboveUintMax_ShowsErrorLocatesRowAndAbortsBeforeAnyWrite()
    {
        // :1158 `if dwExp > High(LongWord)` → 4294967296 越界。
        // ⚠ 部分写入是**原文语义**：:1165 在**校验之后**、循环内逐行写表，
        //   所以第 1 行（合法）已被写入 g_Config.dwPetNeedExps，第 2 行报错 Exit 时
        //   它**前面**的行已生效、**后面**的行未生效（不是事务性回滚）。
        OpenWith();
        SetGridCell(1, 1, "10");
        SetGridCell(2, 1, "4294967296");     // uint.MaxValue + 1
        SetGridCell(3, 1, "30");

        Form.RaiseSaveExp();

        Assert.Single(Messages);
        Assert.Equal("等级 2 升级经验设置错误！", Messages[0].Text);
        Assert.Equal("错误信息", Messages[0].Caption);
        Assert.Equal(M2Forms.MB_OK | M2Forms.MB_ICONERROR, Messages[0].Flags);
        Assert.Equal(new[] { 2 }, GridLocatedRows);          // :1161-1162
        Assert.Empty(WroteInt);                             // :1163 Exit → 未写任何 INI 键
        Assert.Empty(WroteExp);
        Assert.Empty(WroteBool);
        Assert.Equal(10u, M2Config.dwPetNeedExps[1]);       // 第 1 行已写（原文如此）
        Assert.NotEqual(30u, M2Config.dwPetNeedExps[3]);    // 第 3 行未写（Exit 在它之前）
    }

    [Fact]
    public void SaveExp_RowOneFails_LoopAbortsImmediately()
    {
        OpenWith();
        SetGridCell(1, 1, "9999999999");     // 第 1 行即越界

        Form.RaiseSaveExp();

        Assert.Single(Messages);
        Assert.Equal("等级 1 升级经验设置错误！", Messages[0].Text);
        Assert.Equal(new[] { 1 }, GridLocatedRows);
    }

    [Fact]
    public void SaveExp_UintMaxBoundaryIsInclusive()
    {
        // 差异断言：边界是 `> uint.MaxValue`（不是 `> int.MaxValue`）。
        // 2147483648（int.MaxValue+1）必须**通过**，2147483647 更必须通过。
        OpenWith();
        SetGridCell(1, 1, "2147483647");
        SetGridCell(2, 1, "2147483648");
        SetGridCell(3, 1, "4294967295");

        Form.RaiseSaveExp();

        Assert.Empty(Messages);
        Assert.Equal(2147483647u, M2Config.dwPetNeedExps[1]);
        Assert.Equal(2147483648u, M2Config.dwPetNeedExps[2]);
        Assert.Equal(4294967295u, M2Config.dwPetNeedExps[3]);
    }

    [Fact]
    public void SaveExp_UnparseableCell_BecomesZero_NoThrow()
    {
        // `StrToInt64Def(S, 0)`：空串 / 非数字 / 小数 / 前后空白的语义
        OpenWith();
        SetGridCell(1, 1, "");
        SetGridCell(2, 1, "abc");
        SetGridCell(3, 1, "  12  ");
        SetGridCell(4, 1, "1.5");

        Form.RaiseSaveExp();

        Assert.Empty(Messages);
        Assert.Equal(0u, M2Config.dwPetNeedExps[1]);    // 空 → 0
        Assert.Equal(0u, M2Config.dwPetNeedExps[2]);    // 非数字 → 0
        Assert.Equal(12u, M2Config.dwPetNeedExps[3]);   // Trim 后可解析
        Assert.Equal(0u, M2Config.dwPetNeedExps[4]);    // "1.5" 用 Integer 样式 → 失败 → 0
    }

    [Fact]
    public void SaveExp_NegativeCell_IsNotGreaterThanUintMax_SoStoredAsWrappedUint()
    {
        // ★ 差异断言（原文语义）：`dwExp` 是 Int64，`if dwExp > High(LongWord)` 对**负数**为 false，
        //   于是 `NeedExps[I] := dwExp` 把 -1 赋给 LongWord 字段 → 回绕成 4294967295。
        //   托管侧 `(uint)dwExp` 复刻同一回绕。
        OpenWith();
        SetGridCell(1, 1, "-1");

        Form.RaiseSaveExp();

        Assert.Empty(Messages);
        Assert.Equal(uint.MaxValue, M2Config.dwPetNeedExps[1]);
    }

    [Fact]
    public void SaveExp_WritesFiveSetupKeysInOriginalOrder()
    {
        M2Config.boPetUseFixExp = true;
        M2Config.nPetBaseExp = 11;
        M2Config.nPetAddExp = 22;
        M2Config.nPetHighLevel = 33;
        M2Config.nPetHighLevelGetExp = 44;
        OpenWith();

        Form.RaiseSaveExp();

        Assert.Equal(new[] { ("PetUseFixExp", true) }, WroteBool);
        Assert.Equal(new[]
        {
            ("PetBaseExp", 11), ("PetAddExp", 22), ("PetHighLevel", 33), ("PetHighLevelGetExp", 44),
        }, WroteInt);
    }

    [Fact]
    public void SaveExp_WritesExactlyOneThousandGamePetExpEntriesLevel1ToLevel1000()
    {
        OpenWith();

        Form.RaiseSaveExp();

        Assert.Equal(1000, WroteExp.Count);
        Assert.All(WroteExp, e => Assert.Equal("GamePetExp", e.Section));
        Assert.Equal("Level1", WroteExp[0].Key);
        Assert.Equal("Level1000", WroteExp[999].Key);
        // :1175 `for I := Low(dwPetNeedExps) to High(dwPetNeedExps)` —— Low=1，**不含索引 0**
        Assert.DoesNotContain(WroteExp, e => e.Key == "Level0");
        // 键序严格递增，无重复
        var levels = WroteExp.Select(e => int.Parse(e.Key.Substring(5))).ToList();
        Assert.Equal(Enumerable.Range(1, 1000), levels);
    }

    [Fact]
    public void SaveExp_GamePetExpValueMatchesTableViaDelphiIntToStr()
    {
        OpenWith();
        SetGridCell(7, 1, "123456");
        WroteExp.Clear();

        Form.RaiseSaveExp();

        Assert.Equal("123456", WroteExp.Single(e => e.Key == "Level7").Value);
    }

    [Fact]
    public void SaveExp_ClearsDirtyFlagAndDisablesButton()
    {
        OpenWith();
        Form.btnSaveExp.Enabled = true;
        Form.RaiseGridSetEditText(1, 1);
        Assert.True(Form.BoModValued);

        Form.RaiseSaveExp();

        Assert.False(Form.BoModValued);          // :1180
        Assert.False(Form.btnSaveExp.Enabled);   // :1181
    }

    [Fact]
    public void SaveExp_DoesNotCallSendServerConfig()
    {
        OpenWith();
        Form.RaiseSaveExp();
        Assert.Equal(0, SendServerConfigCount);
    }

    private void SetGridCell(int row, int col, string value)
    {
        while (Form.GridLevelExp.Rows.Count <= row)
            Form.GridLevelExp.Rows.Add();
        Form.GridLevelExp.Rows[row].Cells[col].Value = value;
    }

    // ========================================================================
    //  :830-912 btnSavePetParamsClick
    // ========================================================================

    [Fact]
    public void SavePetParams_WritesAllSetupKeysInOriginalOrder()
    {
        OpenWith();
        M2Config.boOpenGamePet = true;
        M2Config.boEnabledPetAttack = true;
        M2Config.boDisableMonAttackPet = true;
        M2Config.boDisableAllAttackPet = true;
        M2Config.boEnabledPetPickup = true;
        M2Config.boPetOnlyPickMonsterItem = true;
        M2Config.boPetPickupToMaster = true;
        M2Config.boPetPickupFullToMaster = true;
        M2Config.boPetQuickPickup = true;
        M2Config.boPetRangePickup = true;
        M2Config.btPetPickupRange = 7;
        M2Config.boPetNoEntity = true;
        M2Config.boPetNoShowHPProgress = true;
        M2Config.boPetSleepControlBySlave = true;
        M2Config.nPetAbilToMasterRate = 9;
        M2Config.boPetHPToMaster = true;
        M2Config.boPetDCToMaster = true;
        M2Config.boPetMCToMaster = true;
        M2Config.boPetSCToMaster = true;
        M2Config.boPetACToMaster = true;
        M2Config.boPetMACToMaster = true;
        M2Config.dwPetUseItemIntervalTime = 100;
        M2Config.boCapturePetNeedItem = true;
        M2Config.boCaptureOKDecDura = true;
        M2Config.boPetShowMasterName = true;
        M2Config.btPetNameColor = 5;
        M2Config.sPetSuffixName = "宝宝";
        M2Config.nGamePetMaxCount = 1;
        M2Config.nGamePetNameCount = 2;
        M2Config.nGamePetRecallTime = 3;
        M2Config.boGamePetKillMonTrigger = true;
        M2Config.boEnablePetUseClientPickItems = true;

        Form.RaiseSavePetParams();

        var boolKeys = WroteBool.Select(x => x.Key).ToList();
        Assert.Equal(new[]
        {
            "OpenGamePet", "EnabledPetAttack", "DisableMonAttackPet", "DisableAllAttackPet",
            "EnabledPetPickup", "PetOnlyPickMonsterItem", "PetPickupToMaster", "PetPickupFullToMaster",
            "PetQuickPickup", "PetRangePickup", "PetNoEntity", "PetNoShowHPProgress",
            "PetSleepControlBySlave", "PetHPToMaster", "PetDCToMaster", "PetMCToMaster",
            "PetSCToMaster", "PetACToMaster", "PetMACToMaster",
            "CapturePetNeedItem", "CaptureOKDecDura", "PetShowMasterName",
            "GamePetKillMonTrigger", "EnablePetUseClientPickItems",
        }, boolKeys);

        var intKeys = WroteInt.Select(x => x.Key).ToList();
        Assert.Equal(new[]
        {
            "PetPickupRange", "PetAbilToMasterRate", "PetUseItemIntervalTime",
            "PetNameColor", "GamePetMaxCount", "GamePetNameCount", "GamePetRecallTime",
        }, intKeys);

        Assert.Equal(new[] { ("PetSuffixName", "宝宝") }, WroteStr);
        Assert.Equal(1, SendServerConfigCount);
        Assert.False(Form.btnSavePetParams.Enabled);
    }

    [Fact]
    public void SavePetParams_KeyControlled_OmitsClientPickItemsWhenKeyIsNotOne()
    {
        // ★ 差异断言（:903-906）：`EnablePetUseClientPickItems` **只有** g_nKey == 1 才写。
        OpenWith();
        Form.g_nKey_UseClientPickItems = 0;

        Form.RaiseSavePetParams();

        Assert.DoesNotContain(WroteBool, x => x.Key == "EnablePetUseClientPickItems");
        Assert.Equal(1, SendServerConfigCount);          // 下发仍执行
    }

    [Fact]
    public void SavePetParams_KeyControlled_IncludesClientPickItemsWhenKeyIsOne()
    {
        OpenWith();
        Form.g_nKey_UseClientPickItems = 1;

        Form.RaiseSavePetParams();

        Assert.Contains(WroteBool, x => x.Key == "EnablePetUseClientPickItems");
        Assert.DoesNotContain(WroteBool, x => x.Key == "EnableClientPickItems");
    }

    [Fact]
    public void SavePetParams_NotOpened_StillWritesBecauseNoBoOpenedGate()
    {
        // ★ 差异断言：原文 btnSavePetParamsClick **没有** `if not boOpened` 闸门
        //   （对比其它 36 个处理器都有）。按钮虽在 DoOpen 尾部被禁用，但处理器本身无闸门。
        using var fresh = new FreshForm();
        var ints = new List<(string, int)>();
        fresh.Form.WriteIntegerHandler = (k, v) => ints.Add((k, v));

        fresh.Form.RaiseSavePetParams();

        Assert.NotEmpty(ints);
    }

    // ========================================================================
    //  SaveGamePetsConfig / LoadGamePetsConfig（M2Share.pas:32759-32833 / 32678-32757）
    // ========================================================================

    [Fact]
    public void SaveGamePetsConfig_WritesCountAndPerPetSections()
    {
        var a = Pet("狼");
        var b = Pet("鹿");
        OpenWith(a, b);

        Form.RaiseSavePet();

        Assert.True(File.Exists(PetConfigFile));
        var ini = new TFastIniFile(PetConfigFile);
        Assert.Equal(2, ini.ReadInteger("setup", "count", -1));
        Assert.Equal("狼", ini.ReadString("pet1", "name", ""));
        Assert.Equal("鹿", ini.ReadString("pet2", "name", ""));
        Assert.Equal(1, ini.ReadInteger("pet1", "capturerate", -1));
        Assert.Equal(50, ini.ReadInteger("pet1", "HPScale", -1));
        Assert.True(ini.ReadBool("pet1", "IsAddHPRate", false));
    }

    [Fact]
    public void SaveGamePetsConfig_DeletesExistingFileFirst_RemovingStaleSections()
    {
        // ★ 差异断言：原文 `if FileExists then DeleteFile` 再 `TIniFileEx.Create`
        //   （**先删后建**，不是"覆盖写"）→ 旧的多余节必须消失。
        var ini0 = new TFastIniFile(PetConfigFile);
        ini0.WriteInteger("setup", "count", 99);
        ini0.WriteString("pet99", "name", "陈旧");
        ini0.Save();
        Assert.Contains("pet99", File.ReadAllText(PetConfigFile));

        OpenWith(Pet("新的"));
        Form.RaiseSavePet();

        string text = File.ReadAllText(PetConfigFile);
        Assert.DoesNotContain("pet99", text);
        Assert.DoesNotContain("陈旧", text);
        Assert.Contains("count=1", text);
    }

    [Fact]
    public void SaveGamePetsConfig_EmptyList_WritesCountZeroAndNoPetSections()
    {
        OpenWith();
        Form.RaiseSavePet();

        string text = File.ReadAllText(PetConfigFile);
        Assert.Contains("count=0", text);
        Assert.DoesNotContain("[pet1]", text);
    }

    [Fact]
    public void SaveGamePetsConfig_WritesAllFortyPerPetKeys()
    {
        OpenWith(Pet("全字段"));
        Form.RaiseSavePet();

        var ini = new TFastIniFile(PetConfigFile);
        // 1 name + 1 capturerate + 3 (EnabledLevelDifference/LevelDifference/HPScale)
        // + 12 Show* + 2 AddHP/IsAddHPRate + 20 六属性×(值+Rate) = 39 键
        Assert.Equal("全字段", ini.ReadString("pet1", "name", ""));
        Assert.Equal(3, ini.ReadInteger("pet1", "ShowFile1", -1));
        Assert.Equal(4, ini.ReadInteger("pet1", "ShowStart1", -1));
        Assert.Equal(8, ini.ReadInteger("pet1", "ShowOffsetY1", -1));
        Assert.Equal(9, ini.ReadInteger("pet1", "ShowFile2", -1));
        Assert.Equal(14, ini.ReadInteger("pet1", "ShowOffsetY2", -1));
        Assert.Equal(15, ini.ReadInteger("pet1", "AddHP", -1));
        Assert.True(ini.ReadBool("pet1", "IsAddHPRate", false));
        Assert.Equal(16, ini.ReadInteger("pet1", "AddDC1", -1));
        Assert.Equal(25, ini.ReadInteger("pet1", "AddMAC2", -1));
        Assert.True(ini.ReadBool("pet1", "IsAddMAC2Rate", false));
        Assert.True(ini.ReadBool("pet1", "EnabledLevelDifference", false));
        Assert.Equal(2, ini.ReadInteger("pet1", "LevelDifference", -1));
    }

    [Fact]
    public void SaveThenLoad_RoundTripsAllFields()
    {
        var original = Pet("往返");
        OpenWith(original);

        Form.RaiseSavePet();

        GamePetsState.ClearGamePetsConfig();
        Assert.True(GamePetsState.LoadGamePetsConfig());

        var loaded = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Equal(original.Name, loaded.Name);
        Assert.Equal(original.CaptureRate, loaded.CaptureRate);
        Assert.Equal(original.EnabledLevelDifference, loaded.EnabledLevelDifference);
        Assert.Equal(original.LevelDifference, loaded.LevelDifference);
        Assert.Equal(original.HPScale, loaded.HPScale);
        Assert.Equal(original.ShowFile1, loaded.ShowFile1);
        Assert.Equal(original.ShowStart1, loaded.ShowStart1);
        Assert.Equal(original.ShowCount1, loaded.ShowCount1);
        Assert.Equal(original.ShowTime1, loaded.ShowTime1);
        Assert.Equal(original.ShowOffsetX1, loaded.ShowOffsetX1);
        Assert.Equal(original.ShowOffsetY1, loaded.ShowOffsetY1);
        Assert.Equal(original.ShowFile2, loaded.ShowFile2);
        Assert.Equal(original.ShowStart2, loaded.ShowStart2);
        Assert.Equal(original.ShowCount2, loaded.ShowCount2);
        Assert.Equal(original.ShowTime2, loaded.ShowTime2);
        Assert.Equal(original.ShowOffsetX2, loaded.ShowOffsetX2);
        Assert.Equal(original.ShowOffsetY2, loaded.ShowOffsetY2);
        Assert.Equal(original.AddHP, loaded.AddHP);
        Assert.Equal(original.IsAddHPRate, loaded.IsAddHPRate);
        Assert.Equal(original.AddDC1, loaded.AddDC1);
        Assert.Equal(original.IsAddDC1Rate, loaded.IsAddDC1Rate);
        Assert.Equal(original.AddDC2, loaded.AddDC2);
        Assert.Equal(original.IsAddDC2Rate, loaded.IsAddDC2Rate);
        Assert.Equal(original.AddMC1, loaded.AddMC1);
        Assert.Equal(original.IsAddMC1Rate, loaded.IsAddMC1Rate);
        Assert.Equal(original.AddMC2, loaded.AddMC2);
        Assert.Equal(original.IsAddMC2Rate, loaded.IsAddMC2Rate);
        Assert.Equal(original.AddSC1, loaded.AddSC1);
        Assert.Equal(original.IsAddSC1Rate, loaded.IsAddSC1Rate);
        Assert.Equal(original.AddSC2, loaded.AddSC2);
        Assert.Equal(original.IsAddSC2Rate, loaded.IsAddSC2Rate);
        Assert.Equal(original.AddAC1, loaded.AddAC1);
        Assert.Equal(original.IsAddAC1Rate, loaded.IsAddAC1Rate);
        Assert.Equal(original.AddAC2, loaded.AddAC2);
        Assert.Equal(original.IsAddAC2Rate, loaded.IsAddAC2Rate);
        Assert.Equal(original.AddMAC1, loaded.AddMAC1);
        Assert.Equal(original.IsAddMAC1Rate, loaded.IsAddMAC1Rate);
        Assert.Equal(original.AddMAC2, loaded.AddMAC2);
        Assert.Equal(original.IsAddMAC2Rate, loaded.IsAddMAC2Rate);
    }

    [Fact]
    public void LoadGamePetsConfig_MissingFile_ReturnsFalseAndLeavesListCleared()
    {
        GamePetsState.g_GamePetConfigList.Add(Pet("残留"));

        Assert.False(GamePetsState.LoadGamePetsConfig());
        Assert.Empty(GamePetsState.g_GamePetConfigList);   // ClearGamePetsConfig 先执行
    }

    [Fact]
    public void LoadGamePetsConfig_EmptyNameSection_IsDroppedEntirely()
    {
        // :32699 `if Length(GamePetConfig.Name) > 0` —— 空 name 的节**整节丢弃**
        var ini = new TFastIniFile(PetConfigFile);
        ini.WriteInteger("setup", "count", 2);
        ini.WriteString("pet1", "name", "");
        ini.WriteInteger("pet1", "capturerate", 5);
        ini.WriteString("pet2", "name", "有效");
        ini.WriteInteger("pet2", "capturerate", 6);
        ini.Save();

        Assert.True(GamePetsState.LoadGamePetsConfig());

        var only = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Equal("有效", only.Name);
        Assert.Equal(6, only.CaptureRate);
    }

    [Fact]
    public void LoadGamePetsConfig_CountLargerThanSections_MissingSectionsSkipped()
    {
        // count 说 5 个，实际只有 pet1 → 只有 1 个入表（其余 name 为空被丢）
        var ini = new TFastIniFile(PetConfigFile);
        ini.WriteInteger("setup", "count", 5);
        ini.WriteString("pet1", "name", "唯一");
        ini.Save();

        Assert.True(GamePetsState.LoadGamePetsConfig());
        Assert.Single(GamePetsState.g_GamePetConfigList);
    }

    [Fact]
    public void LoadGamePetsConfig_CountZero_LoadsNothingButReturnsTrue()
    {
        var ini = new TFastIniFile(PetConfigFile);
        ini.WriteInteger("setup", "count", 0);
        ini.Save();

        Assert.True(GamePetsState.LoadGamePetsConfig());
        Assert.Empty(GamePetsState.g_GamePetConfigList);
    }

    [Fact]
    public void LoadGamePetsConfig_DefaultsMatchOriginalLiterals()
    {
        // :32703-32705 缺省：EnabledLevelDifference=True / LevelDifference=2 / HPScale=50
        var ini = new TFastIniFile(PetConfigFile);
        ini.WriteInteger("setup", "count", 1);
        ini.WriteString("pet1", "name", "只有名字");
        ini.Save();

        Assert.True(GamePetsState.LoadGamePetsConfig());

        var c = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.True(c.EnabledLevelDifference);   // True
        Assert.Equal(2, c.LevelDifference);      // 2
        Assert.Equal(50, c.HPScale);             // 50
        Assert.Equal(0, c.CaptureRate);          // 0
        Assert.False(c.IsAddHPRate);             // False
    }

    [Fact]
    public void LoadGamePetsConfig_MultiByteName_RoundTripsThroughGbk()
    {
        OpenWith(Pet("中文宝宝·测试"));
        Form.RaiseSavePet();
        GamePetsState.ClearGamePetsConfig();

        Assert.True(GamePetsState.LoadGamePetsConfig());
        Assert.Equal("中文宝宝·测试", GamePetsState.g_GamePetConfigList[0].Name);
    }

    // ---------- GetGamePetConfig 大小写语义 ----------

    [Fact]
    public void GetGamePetConfig_CaseInsensitive_FirstMatchWins()
    {
        GamePetsState.ClearGamePetsConfig();
        var first = Pet("Alpha");
        var second = Pet("ALPHA");
        GamePetsState.g_GamePetConfigList.Add(first);
        GamePetsState.g_GamePetConfigList.Add(second);

        Assert.Same(first, GamePetsState.GetGamePetConfig("alpha"));
        Assert.Same(first, GamePetsState.GetGamePetConfig("ALPHA"));
        Assert.Same(first, GamePetsState.GetGamePetConfig("Alpha"));
        Assert.Null(GamePetsState.GetGamePetConfig("Beta"));
    }

    [Fact]
    public void GetGamePetConfig_EmptyName_MatchesOnlyEmptyEntry()
    {
        GamePetsState.ClearGamePetsConfig();
        GamePetsState.g_GamePetConfigList.Add(Pet(""));

        Assert.NotNull(GamePetsState.GetGamePetConfig(""));
        Assert.Null(GamePetsState.GetGamePetConfig("x"));
    }

    [Fact]
    public void GetGamePetConfig_EmptyList_ReturnsNull()
    {
        GamePetsState.ClearGamePetsConfig();
        Assert.Null(GamePetsState.GetGamePetConfig("任意"));
    }

    [Fact]
    public void ClearGamePetsConfig_EmptiesList()
    {
        GamePetsState.g_GamePetConfigList.Add(Pet("A"));
        GamePetsState.g_GamePetConfigList.Add(Pet("B"));

        GamePetsState.ClearGamePetsConfig();

        Assert.Empty(GamePetsState.g_GamePetConfigList);
    }
}
