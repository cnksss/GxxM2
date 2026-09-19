using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J111：FB(...) 声明解析与副本地图创建注册（LocalDB.pas 1719-1760 / 2225-2251）
/// + FindMonster/GetMonRace（UsrEngn.pas 11142-11177 / 714-746）1:1 测试。
/// </summary>
public sealed class FbMapDeclareCoreTests
{
    private sealed class Mon
    {
        public string Name = "";
        public byte Race;
        public byte RaceImg;
        public ushort Appr;
    }

    private static List<Mon> Mons(params (string Name, byte Race, byte Img, ushort Appr)[] items)
    {
        var list = new List<Mon>();
        foreach (var (n, r, i, a) in items)
            list.Add(new Mon { Name = n, Race = r, RaceImg = i, Appr = a });
        return list;
    }

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal("FB(", FbMapDeclareCore.FbDeclPrefix);
        Assert.Equal(2, FbMapDeclareCore.FbCountMin);
        Assert.Equal(99, FbMapDeclareCore.FbCountMax);
        Assert.Equal(10, FbMapDeclareCore.NoHumClearFbMinDefault);
        Assert.Equal(10, FbMapDeclareCore.NoHumClearFbMinFloor);
        Assert.Equal(1, FbMapDeclareCore.FbIndexStart);
        Assert.Equal("$FB_", FbMapDeclareCore.FbMapNamePrefix);
    }

    [Fact]
    public void UnitConversionConstants()
    {
        // 2242 分→毫秒；2243 秒→毫秒 —— **系数不同**
        Assert.Equal(60_000, FbMapDeclareCore.MinuteToMs);
        Assert.Equal(1_000, FbMapDeclareCore.SecondToMs);
        Assert.NotEqual(FbMapDeclareCore.MinuteToMs, FbMapDeclareCore.SecondToMs);
    }

    [Fact]
    public void ErrorCodesMatchSource()
    {
        Assert.Equal(-12, FbMapDeclareCore.ErrEmptyFbName);
        Assert.Equal(-13, FbMapDeclareCore.ErrFbCountOutOfRange);
        Assert.Equal(-14, FbMapDeclareCore.ErrFbNameDuplicated);
    }

    [Fact]
    public void EnterLimitRangeIsZeroToThree()
    {
        Assert.Equal(0, FbMapDeclareCore.FbEnterLimitLow);
        Assert.Equal(3, FbMapDeclareCore.FbEnterLimitHigh);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, FbMapDeclareCore.FbEnterLimitFallback);
    }

    [Fact]
    public void EnterLimitOrdinalsMatchSourceDeclaration()
    {
        // Envir.pas 155：(fbel_JOB3, fbel_Group, fbel_OnlyCreater, fbel_Guild)
        // **注意 1 是 Group、2 是 OnlyCreater** —— 与 1721 注释的数值用词不同
        Assert.Equal(0, (int)FbMapDeclareCore.FbEnterLimit.Job3);
        Assert.Equal(1, (int)FbMapDeclareCore.FbEnterLimit.Group);
        Assert.Equal(2, (int)FbMapDeclareCore.FbEnterLimit.OnlyCreater);
        Assert.Equal(3, (int)FbMapDeclareCore.FbEnterLimit.Guild);
    }

    // ===================== 声明识别与括号提取 =====================

    [Fact]
    public void FbDeclPrefixIsCaseInsensitivePrefix()
    {
        Assert.True(FbMapDeclareCore.IsFbDecl("FB(40,祖玛副本,0,1)"));
        Assert.True(FbMapDeclareCore.IsFbDecl("fb(40,x,0,1)"));
    }

    [Fact]
    public void NonFbDeclRejected()
    {
        Assert.False(FbMapDeclareCore.IsFbDecl("SAFE"));
        Assert.False(FbMapDeclareCore.IsFbDecl(""));
    }

    [Fact]
    public void ArrestStringExReturnsInnerAndRemainder()
    {
        // HUtil32 1713-1759：ArrestStr = 括号内；**返回值 = ')' 之后**
        var (inner, rem) = FbMapDeclareCore.ArrestStringEx("FB(40,祖玛,0,1)TAIL");

        Assert.Equal("40,祖玛,0,1", inner);
        Assert.Equal("TAIL", rem);
    }

    [Fact]
    public void ArrestStringExNoParenLeavesArrestEmpty()
    {
        // 1720 无条件置空 → ArrestStr = ''；1719 → 返回原串
        var (inner, rem) = FbMapDeclareCore.ArrestStringEx("FB_NO_PAREN");

        Assert.Equal("", inner);
        Assert.Equal("FB_NO_PAREN", rem);
    }

    [Fact]
    public void ArrestStringExUnclosedParenKeepsArrestEmpty()
    {
        var (inner, rem) = FbMapDeclareCore.ArrestStringEx("FB(40,祖玛");

        Assert.Equal("", inner);
        Assert.Equal("FB(40,祖玛", rem);
    }

    [Fact]
    public void ArrestStringExEmptySource()
    {
        var (inner, rem) = FbMapDeclareCore.ArrestStringEx("");
        Assert.Equal("", inner);
        Assert.Equal("", rem);
    }

    [Fact]
    public void ArrestStringExEmptyParens()
    {
        var (inner, rem) = FbMapDeclareCore.ArrestStringEx("FB()");
        Assert.Equal("", inner);
        Assert.Equal("", rem);
    }

    [Fact]
    public void ExtractParenContentReturnsInner()
    {
        Assert.Equal("a,b", FbMapDeclareCore.ExtractParenContent("X(a,b)"));
    }

    // ===================== 参数解析（1725-1740） =====================

    [Fact]
    public void ParseFullDecl()
    {
        // 原文注释 1720：FB(40,祖玛副本,0,1)
        var d = FbMapDeclareCore.ParseFbDecl("FB(40,祖玛副本,0,1)");

        Assert.Equal(40, d.FbCount);
        Assert.Equal("祖玛副本", d.FbName);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Job3, d.EnterLimit);
        Assert.Equal(1, d.EnterDelayMin);
        Assert.Equal(10, d.NoHumClearFbMin);   // 第5参数缺失 → 缺省 10
    }

    [Fact]
    public void ParseAllFiveParams()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,赤月副本,1,5,30)");

        Assert.Equal(20, d.FbCount);
        Assert.Equal("赤月副本", d.FbName);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Group, d.EnterLimit);
        Assert.Equal(5, d.EnterDelayMin);
        Assert.Equal(30, d.NoHumClearFbMin);
    }

    [Fact]
    public void EnterLimitOutOfRangeFallsBackToOnlyCreater()
    {
        // 1731-1735：越界回退，**不是**取注释里的 0
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,名,7,0)");
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, d.EnterLimit);
    }

    [Fact]
    public void EnterLimitMissingFallsBackToOnlyCreater()
    {
        // 缺省 -1 → 越界 → 回退
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,名)");
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, d.EnterLimit);
    }

    [Fact]
    public void EnterLimitAllValidValues()
    {
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Job3,
            FbMapDeclareCore.ParseFbDecl("FB(20,n,0,0)").EnterLimit);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Group,
            FbMapDeclareCore.ParseFbDecl("FB(20,n,1,0)").EnterLimit);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater,
            FbMapDeclareCore.ParseFbDecl("FB(20,n,2,0)").EnterLimit);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Guild,
            FbMapDeclareCore.ParseFbDecl("FB(20,n,3,0)").EnterLimit);
    }

    [Fact]
    public void EnterLimitNegativeFallsBack()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,n,-1,0)");
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, d.EnterLimit);
    }

    [Fact]
    public void NoHumClearZeroResetsToTen()
    {
        // 1739-1740：**显式写 0 也变成 10**
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,n,0,0,0)");
        Assert.Equal(10, d.NoHumClearFbMin);
    }

    [Fact]
    public void NoHumClearNegativeResetsToTen()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,n,0,0,-5)");
        Assert.Equal(10, d.NoHumClearFbMin);
    }

    [Fact]
    public void NoHumClearPositiveKept()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,n,0,0,45)");
        Assert.Equal(45, d.NoHumClearFbMin);
    }

    [Fact]
    public void MissingCountDefaultsToZero()
    {
        // 1727 缺省 0（非 -1）——与 enterLimit 的缺省不同
        var d = FbMapDeclareCore.ParseFbDecl("FB()");
        Assert.Equal(0, d.FbCount);
    }

    [Fact]
    public void MissingEnterDelayDefaultsToZero()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,n,0)");
        Assert.Equal(0, d.EnterDelayMin);
    }

    [Fact]
    public void NonNumericFieldsFallBackToDefaults()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(abc,名,xyz,q)");

        Assert.Equal(0, d.FbCount);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, d.EnterLimit);
        Assert.Equal(0, d.EnterDelayMin);
        Assert.Equal(10, d.NoHumClearFbMin);
    }

    [Fact]
    public void RawFieldsPreserved()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,n,1,5,30)");

        Assert.Equal("1", d.RawEnterLimit);
        Assert.Equal("5", d.RawEnterDelay);
    }

    [Fact]
    public void SelectEnterLimitDirectly()
    {
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Guild, FbMapDeclareCore.SelectEnterLimit(3));
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, FbMapDeclareCore.SelectEnterLimit(4));
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.OnlyCreater, FbMapDeclareCore.SelectEnterLimit(-99));
    }

    // ===================== 三条校验（1741-1758） =====================

    [Fact]
    public void ValidDeclPasses()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,祖玛,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.Ok,
            FbMapDeclareCore.ValidateFbDecl(d, nameAlreadyExists: false));
    }

    [Fact]
    public void EmptyNameRejected()
    {
        // 1741-1745
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.EmptyFbName,
            FbMapDeclareCore.ValidateFbDecl(d, false));
    }

    [Fact]
    public void CountBelowTwoRejected()
    {
        // 1747-1751
        var d = FbMapDeclareCore.ParseFbDecl("FB(1,名,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.CountOutOfRange,
            FbMapDeclareCore.ValidateFbDecl(d, false));
    }

    [Fact]
    public void CountAbove99Rejected()
    {
        var d = FbMapDeclareCore.ParseFbDecl("FB(100,名,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.CountOutOfRange,
            FbMapDeclareCore.ValidateFbDecl(d, false));
    }

    [Fact]
    public void CountBoundariesAccepted()
    {
        // 闭区间 [2..99]
        Assert.True(FbMapDeclareCore.IsFbCountValid(2));
        Assert.True(FbMapDeclareCore.IsFbCountValid(99));
        Assert.False(FbMapDeclareCore.IsFbCountValid(1));
        Assert.False(FbMapDeclareCore.IsFbCountValid(100));
    }

    [Fact]
    public void DuplicateNameRejected()
    {
        // 1753-1757
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,祖玛,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.NameDuplicated,
            FbMapDeclareCore.ValidateFbDecl(d, nameAlreadyExists: true));
    }

    [Fact]
    public void ValidationOrderEmptyNameFirst()
    {
        // **顺序不可重排**：空名 + 数量越界 + 重名 → 空名
        var d = FbMapDeclareCore.ParseFbDecl("FB(1,,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.EmptyFbName,
            FbMapDeclareCore.ValidateFbDecl(d, true));
    }

    [Fact]
    public void ValidationOrderCountBeforeDuplicate()
    {
        // 数量越界 + 重名 → 数量
        var d = FbMapDeclareCore.ParseFbDecl("FB(1,名,0,1)");
        Assert.Equal(FbMapDeclareCore.FbValidateResult.CountOutOfRange,
            FbMapDeclareCore.ValidateFbDecl(d, true));
    }

    [Fact]
    public void ValidationCodesAndMessages()
    {
        Assert.Equal(-12, FbMapDeclareCore.ValidateResultToCode(FbMapDeclareCore.FbValidateResult.EmptyFbName));
        Assert.Equal(-13, FbMapDeclareCore.ValidateResultToCode(FbMapDeclareCore.FbValidateResult.CountOutOfRange));
        Assert.Equal(-14, FbMapDeclareCore.ValidateResultToCode(FbMapDeclareCore.FbValidateResult.NameDuplicated));
        Assert.Equal(0, FbMapDeclareCore.ValidateResultToCode(FbMapDeclareCore.FbValidateResult.Ok));
    }

    [Fact]
    public void ValidationMessageTexts()
    {
        Assert.Equal("0 副本名称不能为空.",
            FbMapDeclareCore.ValidateMessage(FbMapDeclareCore.FbValidateResult.EmptyFbName, "0", ""));
        Assert.Equal("0 副本数量为2~99.",
            FbMapDeclareCore.ValidateMessage(FbMapDeclareCore.FbValidateResult.CountOutOfRange, "0", ""));
        Assert.Equal("0 副本名称[祖玛]已经存在.",
            FbMapDeclareCore.ValidateMessage(FbMapDeclareCore.FbValidateResult.NameDuplicated, "0", "祖玛"));
        Assert.Equal("", FbMapDeclareCore.ValidateMessage(FbMapDeclareCore.FbValidateResult.Ok, "0", ""));
    }

    [Fact]
    public void BoFBOnlySetWhenValid()
    {
        // 1759
        Assert.True(FbMapDeclareCore.ShouldSetBoFB(FbMapDeclareCore.FbValidateResult.Ok));
        Assert.False(FbMapDeclareCore.ShouldSetBoFB(FbMapDeclareCore.FbValidateResult.EmptyFbName));
        Assert.False(FbMapDeclareCore.ShouldSetBoFB(FbMapDeclareCore.FbValidateResult.CountOutOfRange));
        Assert.False(FbMapDeclareCore.ShouldSetBoFB(FbMapDeclareCore.FbValidateResult.NameDuplicated));
    }

    // ===================== 副本地图创建与注册（2225-2251） =====================

    [Fact]
    public void FbMapNameFormat()
    {
        // 2235：'$FB_' + sMainMapName + '_' + k
        Assert.Equal("$FB_0_1", FbMapDeclareCore.MakeFbMapName("0", 1));
    }

    [Fact]
    public void FbMapNamesStartAtOne()
    {
        // 2233：for k := 1 to nFBCount —— **从 1 开始**
        var names = FbMapDeclareCore.MakeAllFbMapNames("祖玛", 3);

        Assert.Equal(new[] { "$FB_祖玛_1", "$FB_祖玛_2", "$FB_祖玛_3" }, names);
    }

    [Fact]
    public void FbMapNamesCountMatches()
    {
        Assert.Equal(5, FbMapDeclareCore.MakeAllFbMapNames("m", 5).Count);
    }

    [Fact]
    public void FbMapNamesEmptyWhenCountZero()
    {
        Assert.Empty(FbMapDeclareCore.MakeAllFbMapNames("m", 0));
    }

    [Fact]
    public void FbMapNamesNeverIncludeZeroSuffix()
    {
        var names = FbMapDeclareCore.MakeAllFbMapNames("m", 2);
        Assert.DoesNotContain(names, n => n.EndsWith("_0"));
    }

    [Fact]
    public void ResolveMainMapNameUsesOwnNameWhenEmpty()
    {
        // 2228-2229
        Assert.Equal("祖玛", FbMapDeclareCore.ResolveMainMapName("", "祖玛"));
    }

    [Fact]
    public void ResolveMainMapNameKeepsExplicitValue()
    {
        Assert.Equal("主图", FbMapDeclareCore.ResolveMainMapName("主图", "祖玛"));
    }

    [Fact]
    public void EnterDelayConvertedAsMinutes()
    {
        // 2242：*60000
        Assert.Equal(60_000, FbMapDeclareCore.EnterDelayMinToMs(1));
        Assert.Equal(300_000, FbMapDeclareCore.EnterDelayMinToMs(5));
    }

    [Fact]
    public void NoHumClearConvertedAsSeconds()
    {
        // 2243：*1000（**与上面系数不同**）
        Assert.Equal(10_000, FbMapDeclareCore.NoHumClearMinToMs(10));
        Assert.Equal(30_000, FbMapDeclareCore.NoHumClearMinToMs(30));
    }

    [Fact]
    public void TwoConversionsDifferForSameInput()
    {
        // **差异保护**：同一数字（1）下两个字段得到不同毫秒值
        Assert.NotEqual(FbMapDeclareCore.EnterDelayMinToMs(1), FbMapDeclareCore.NoHumClearMinToMs(1));
        Assert.Equal(60_000, FbMapDeclareCore.EnterDelayMinToMs(1));
        Assert.Equal(1_000, FbMapDeclareCore.NoHumClearMinToMs(1));
    }

    [Fact]
    public void CreatedMapGetsFbFields()
    {
        // 2239-2243
        var d = FbMapDeclareCore.ParseFbDecl("FB(20,祖玛,1,5,30)");
        var e = FbMapDeclareCore.MakeFbMapEntry("$FB_主_1", d);

        Assert.Equal("$FB_主_1", e.MapName);
        Assert.True(e.BoFB);
        Assert.Equal("祖玛", e.FbName);
        Assert.Equal(FbMapDeclareCore.FbEnterLimit.Group, e.EnterLimit);
        Assert.Equal(300_000, e.EnterDelayMs);   // 5 分
        Assert.Equal(30_000, e.NoHumClearMs);    // 30 秒
    }

    [Fact]
    public void CreateFailMessageText()
    {
        // 2247
        Assert.Equal("副本地图创建失败，地图名已经存在：$FB_主_1",
            FbMapDeclareCore.FbCreateFailMessage("$FB_主_1"));
    }

    [Fact]
    public void CreatedOnlyWhenEnvirNotNull()
    {
        // 2237
        Assert.True(FbMapDeclareCore.IsFbMapCreated(new object()));
        Assert.False(FbMapDeclareCore.IsFbMapCreated(null));
    }

    [Fact]
    public void AddToFbListOnlyWhenCreated()
    {
        // 2244：仅成功分支
        Assert.True(FbMapDeclareCore.ShouldAddToFbList(true));
        Assert.False(FbMapDeclareCore.ShouldAddToFbList(false));
    }

    // ===================== FindMonster（11142-11177） =====================

    [Fact]
    public void FindMonsterHitsFirstMatch()
    {
        var list = Mons(("a", 1, 0, 0), ("b", 2, 0, 0), ("b", 3, 0, 0), ("c", 4, 0, 0));

        var r = FbMapDeclareCore.FindMonster(list, m => m.Name, "b");

        Assert.True(r.Found);
        Assert.Equal(1, r.Index);   // 第一个匹配
    }

    [Fact]
    public void FindMonsterMissIndexIsInsertPositionNotMinusOne()
    {
        // **与 J109 的关键差异**：11170 无条件赋值 → 未命中时 Index 是插入位置
        var list = Mons(("a", 1, 0, 0), ("c", 3, 0, 0));

        var r = FbMapDeclareCore.FindMonster(list, m => m.Name, "b");

        Assert.False(r.Found);
        Assert.Equal(1, r.Index);       // 插入位置，**不是 -1**
        Assert.NotEqual(-1, r.Index);
    }

    [Fact]
    public void FindMonsterMissOnEmptyListGivesZero()
    {
        var r = FbMapDeclareCore.FindMonster(new List<Mon>(), m => m.Name, "b");

        Assert.False(r.Found);
        Assert.Equal(0, r.Index);
    }

    [Fact]
    public void FindMonsterMissAboveAllGivesCount()
    {
        var list = Mons(("a", 1, 0, 0), ("b", 2, 0, 0));
        var r = FbMapDeclareCore.FindMonster(list, m => m.Name, "z");

        Assert.False(r.Found);
        Assert.Equal(2, r.Index);
    }

    [Fact]
    public void FindMonsterCaseInsensitive()
    {
        var list = Mons(("Abc", 1, 0, 0));
        Assert.True(FbMapDeclareCore.FindMonster(list, m => m.Name, "abc").Found);
    }

    [Fact]
    public void FindMonsterAllSameGivesZero()
    {
        var list = Mons(("a", 1, 0, 0), ("a", 2, 0, 0), ("a", 3, 0, 0));
        var r = FbMapDeclareCore.FindMonster(list, m => m.Name, "a");

        Assert.True(r.Found);
        Assert.Equal(0, r.Index);
    }

    // ===================== GetMonRace（714-746） =====================

    [Fact]
    public void GetMonRaceReturnsRaceOnHit()
    {
        var list = Mons(("祖玛教主", 115, 0, 0), ("鹿", 20, 0, 0));

        Assert.Equal(115, FbMapDeclareCore.GetMonRace(list, m => m.Name, m => m.Race, "祖玛教主"));
        Assert.Equal(20, FbMapDeclareCore.GetMonRace(list, m => m.Name, m => m.Race, "鹿"));
    }

    [Fact]
    public void GetMonRaceReturnsMinusOneOnMiss()
    {
        // **关键**：未命中返回 -1（这正是 J110 的 3610 判定所依赖的）
        var list = Mons(("鹿", 20, 0, 0));
        Assert.Equal(-1, FbMapDeclareCore.GetMonRace(list, m => m.Name, m => m.Race, "不存在"));
    }

    [Fact]
    public void GetMonRaceOnEmptyListIsMinusOne()
    {
        Assert.Equal(-1, FbMapDeclareCore.GetMonRace(new List<Mon>(), m => m.Name, m => m.Race, "x"));
    }

    [Fact]
    public void GetMonRaceMissDoesNotReadNeighbour()
    {
        // 因取值靠布尔返回值而非 Index，未命中时**不会**误读相邻怪物
        var list = Mons(("a", 11, 0, 0), ("c", 33, 0, 0));

        // 'b' 未命中，插入位置为 1（指向 'c'）
        int race = FbMapDeclareCore.GetMonRace(list, m => m.Name, m => m.Race, "b");

        Assert.Equal(-1, race);
        Assert.NotEqual(33, race);
    }

    [Fact]
    public void GetMonRaceMinusOneIsTheNotFoundSentinel()
    {
        // 与 FbMapDeclareCore.MonRaceNotFound 常量一致（供 J110 的 SelectFbTemplateAction 使用）
        var list = Mons(("鹿", 20, 0, 0));
        int race = FbMapDeclareCore.GetMonRace(list, m => m.Name, m => m.Race, "无");

        Assert.Equal(MonGenFbCopyCore.MonRaceNotFound, race);
        Assert.False(MonGenFbCopyCore.ShouldRegisterFbTemplate(race));
    }

    [Fact]
    public void GetMonRaceImgReturnsImgOnHit()
    {
        var list = Mons(("鹿", 20, 7, 0));
        Assert.Equal(7, FbMapDeclareCore.GetMonRaceImg(list, m => m.Name, m => m.RaceImg, "鹿"));
        Assert.Equal(-1, FbMapDeclareCore.GetMonRaceImg(list, m => m.Name, m => m.RaceImg, "无"));
    }

    [Fact]
    public void GetMonRaceImgAndApprHit()
    {
        var list = Mons(("鹿", 20, 7, 555));

        bool ok = FbMapDeclareCore.GetMonRaceImgAndAppr(
            list, m => m.Name, m => m.RaceImg, m => m.Appr, m => m.Race, "鹿",
            out byte img, out ushort appr, out byte race);

        Assert.True(ok);
        Assert.Equal(7, img);
        Assert.Equal(555, appr);
        Assert.Equal(20, race);
    }

    [Fact]
    public void GetMonRaceImgAndApprMissReturnsFalseAndZeroes()
    {
        // 738：Result := False 且**不写出参**（本移植置 0 以保证确定性）
        var list = Mons(("鹿", 20, 7, 555));

        bool ok = FbMapDeclareCore.GetMonRaceImgAndAppr(
            list, m => m.Name, m => m.RaceImg, m => m.Appr, m => m.Race, "无",
            out byte img, out ushort appr, out byte race);

        Assert.False(ok);
        Assert.Equal(0, img);
        Assert.Equal(0, appr);
        Assert.Equal(0, race);
    }

    // ===================== 端到端 =====================

    [Fact]
    public void EndToEndDeclToMaps()
    {
        // 声明 → 校验 → 生成地图名 → 建条目
        const string line = "FB(3,祖玛副本,1,5,20)";

        Assert.True(FbMapDeclareCore.IsFbDecl(line));

        var d = FbMapDeclareCore.ParseFbDecl(line);
        Assert.Equal(FbMapDeclareCore.FbValidateResult.Ok,
            FbMapDeclareCore.ValidateFbDecl(d, false));

        string main = FbMapDeclareCore.ResolveMainMapName("", "祖玛7");
        var names = FbMapDeclareCore.MakeAllFbMapNames(main, d.FbCount);

        Assert.Equal(3, names.Count);
        Assert.Equal("$FB_祖玛7_1", names[0]);

        var e = FbMapDeclareCore.MakeFbMapEntry(names[0], d);
        Assert.True(e.BoFB);
        Assert.Equal("祖玛副本", e.FbName);
        Assert.Equal(300_000, e.EnterDelayMs);
        Assert.Equal(20_000, e.NoHumClearMs);
    }

    [Fact]
    public void EndToEndDeclFeedsJ110RaceCheck()
    {
        // FB 声明解析出的怪名 → GetMonRace → J110 的模板登记判定
        var mons = Mons(("祖玛教主", 115, 0, 0));

        int race = FbMapDeclareCore.GetMonRace(mons, m => m.Name, m => m.Race, "祖玛教主");
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.RegisterAndCreateCopies,
            MonGenFbCopyCore.SelectFbTemplateAction("$祖玛副本", 0, race));

        int missing = FbMapDeclareCore.GetMonRace(mons, m => m.Name, m => m.Race, "不存在的怪");
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.RaceNotFound,
            MonGenFbCopyCore.SelectFbTemplateAction("$祖玛副本", 0, missing));
    }
}
