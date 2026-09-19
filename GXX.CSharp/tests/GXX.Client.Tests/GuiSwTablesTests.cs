using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using GXX.Client.GUI.NewStateWin;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P1 切片 A：StateWindows.pas 声明段（49-892 / 1282-2131 / 48-1238 行）的静态表校验。
/// 逐字核对由脚本从原文抽取的三张表：字段名、控件注册名、方法声明清单。
/// </summary>
public sealed class TStateWindowsTablesTests
{
    private static string Sha256OfLines(IEnumerable<string> lines)
    {
        string joined = string.Join("\n", lines);
        byte[] bytes = Encoding.UTF8.GetBytes(joined);
        byte[] hash = SHA256.HashData(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (byte b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    // ===================== 字段声明表（原文 49-892） =====================

    [Fact]
    public void FieldTableHas845Entries()
    {
        // 原文 48 行 `TStateWindows = class`，49-892 行为字段声明，893 行起为 private 段。
        Assert.Equal(845, TStateWindowsFieldTable.Count);
        Assert.Equal(845, TStateWindowsFieldTable.Names.Length);
    }

    [Fact]
    public void FieldNamesSha256MatchesOriginal()
    {
        // 校验和锁定：字段名按声明顺序以 '\n' 连接后的 UTF-8 SHA-256。
        Assert.Equal("b3e7debb71ae24b8311b25cf2e42a40a3dcce1b5eb8b026166a7c9c33223071e",
            Sha256OfLines(TStateWindowsFieldTable.Names));
        Assert.Equal(TStateWindowsFieldTable.NamesSha256, Sha256OfLines(TStateWindowsFieldTable.Names));
    }

    [Fact]
    public void FieldNamesAreUniqueAndNonEmpty()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (string name in TStateWindowsFieldTable.Names)
        {
            Assert.False(string.IsNullOrEmpty(name));
            Assert.True(seen.Add(name), "重名字段：" + name);
        }
    }

    [Fact]
    public void FieldTableFirstAndLastEntriesMatchSource()
    {
        // 原文 49 行 = ContinuousMagicMenu，892 行 = DUserState1LabelDStateWinCharName。
        Assert.Equal("ContinuousMagicMenu", TStateWindowsFieldTable.Names[0]);
        Assert.Equal("DUserState1LabelDStateWinCharName", TStateWindowsFieldTable.Names[844]);
    }

    // ===================== 控件注册名表（原文 1282-2131） =====================

    [Fact]
    public void ControlNameTableHas845Entries()
    {
        // 原文 1285-2130 行共 845 条 Result.AddObject(...)。
        Assert.Equal(845, TStateWindowsControlNameTable.Count);
        Assert.Equal(845, TStateWindowsControlNameTable.Names.Length);
    }

    [Fact]
    public void ControlNameTableIsIdenticalToFieldTable()
    {
        // 原文 MakeControlAddressList 的键序与字段声明顺序**完全一致**，一条不多一条不少。
        Assert.Equal(TStateWindowsFieldTable.Names, TStateWindowsControlNameTable.Names);
    }

    [Fact]
    public void ControlNameSha256MatchesFieldSha256()
    {
        // 差异断言：两张表若任一处不同，下面两个校验和就会分叉。
        Assert.Equal(
            Sha256OfLines(TStateWindowsFieldTable.Names),
            Sha256OfLines(TStateWindowsControlNameTable.Names));
    }

    [Fact]
    public void AddressListBuildKeepsOrderAndCount()
    {
        var list = TStateWindowsControlAddressList.BuildNames();

        Assert.Equal(845, list.Count);
        Assert.Equal("ContinuousMagicMenu", list[0]);
        Assert.Equal("DSWGodBlessDlg", list[1]);
        Assert.Equal("DUserState1LabelDStateWinCharName", list[844]);
        Assert.Empty(TStateWindowsControlAddressList.Create());
    }

    [Fact]
    public void AddressListIndexOfIsCaseInsensitiveLikeTStringList()
    {
        var list = TStateWindowsControlAddressList.BuildNames();

        Assert.Equal(0, TStateWindowsControlAddressList.IndexOf(list, "ContinuousMagicMenu"));
        // Delphi TStringList.IndexOf 走 AnsiCompareText：大小写不敏感。
        Assert.Equal(0, TStateWindowsControlAddressList.IndexOf(list, "continuousmagicmenu"));
        Assert.Equal(1, TStateWindowsControlAddressList.IndexOf(list, "DSWGODBLESSDLG"));
        Assert.Equal(-1, TStateWindowsControlAddressList.IndexOf(list, "NoSuchControl"));
        Assert.Equal(-1, TStateWindowsControlAddressList.IndexOf(null, "x"));
        Assert.Equal(-1, TStateWindowsControlAddressList.IndexOf(list, null));
    }

    [Fact]
    public void AddressListOrderMatchesBruteForceSearch()
    {
        // 「看起来一样实则不同」：AddObject 保序 ⇒ 顺序 IndexOf 与表内序号必须一致。
        var list = TStateWindowsControlAddressList.BuildNames();
        for (int i = 0; i < TStateWindowsControlNameTable.Names.Length; i++)
            Assert.Equal(i, TStateWindowsControlAddressList.IndexOf(list, TStateWindowsControlNameTable.Names[i]));
    }

    // ===================== 方法声明清单（原文 48-1238 interface 段） =====================

    [Fact]
    public void SurfaceDeclCountIs182()
    {
        Assert.Equal(182, TStateWindowsSurface.DeclCount);
        Assert.Equal(182, TStateWindowsSurface.Decls.Length);
    }

    [Fact]
    public void SurfaceDeclSha256MatchesOriginal()
    {
        var lines = new List<string>();
        foreach (var d in TStateWindowsSurface.Decls)
            lines.Add(VisibilityText(d.Visibility) + "|" + d.Name);

        Assert.Equal("89e459061a4ab9e4e55c76803b25c157b69ad07abe15338ec1c90aa46d32ba0a", Sha256OfLines(lines));
        Assert.Equal(TStateWindowsSurface.DeclSha256, Sha256OfLines(lines));
    }

    private static string VisibilityText(TStateWindowsVisibility v) => v switch
    {
        TStateWindowsVisibility.Private => "private",
        TStateWindowsVisibility.Protected => "protected",
        TStateWindowsVisibility.Public => "public",
        _ => "published",
    };

    [Fact]
    public void SurfaceSectionCountsMatchOriginal()
    {
        // 原文 894 行 private、1179 行 protected、1181 行 public。
        Assert.Equal(142, TStateWindowsSurface.PrivateNames.Length);
        Assert.Equal(1, TStateWindowsSurface.ProtectedNames.Length);
        Assert.Equal(39, TStateWindowsSurface.PublicNames.Length);
        Assert.Equal(182, TStateWindowsSurface.PrivateNames.Length
            + TStateWindowsSurface.ProtectedNames.Length
            + TStateWindowsSurface.PublicNames.Length);
    }

    [Fact]
    public void ProtectedSectionHasExactlyDSWPetsDlgMouseMove()
    {
        // 原文 1180 行：protected 段只有一个方法。
        Assert.Equal("DSWPetsDlgMouseMove", TStateWindowsSurface.ProtectedNames[0]);
    }

    [Fact]
    public void PublicMethodSurfaceMatchesOriginal()
    {
        // 原文 1182-1237 行的 public 段，逐字（含 UpDate / LoadFromStream / OpenUserState 等写法）。
        string[] expected =
        {
            "Create", "Destroy", "Initialize", "Close", "UpDate", "LoadFromStream",
            "MakeControlAddressList", "RefreshGamePetList",
            "OpenStateWinDlg", "CloseStateWinDlg", "OpenUserState1Dlg", "CloseUserState1Dlg",
            "OpenMyStatus", "OpenMyMagic", "OpenUserState", "OpenGamePetDlg",
            "MySelfAbilChange", "MyHeroAbilChange", "OpenHeroStateWinDlg", "CloseHeroStateWinDlg",
            "SetMeridiansLevel", "SetHeroMeridiansLevel",
            "RefreshMySelfMagicList", "RefreshMyHeroMagicList",
            "SetDeputyHeroJob",
            "SetMySelfActivePageIndexCount", "SetMyHeroActivePageIndexCount",
            "ChangeMySelfMeridiansctivePage", "ChangeMyHeroMeridiansctivePage",
            "SetMySelfLastContinuousButton", "SetMyHeroLastContinuousButton",
            "SetMySelfJewelryBoxButton", "SetMyHeroJewelryBoxButton",
            "RefreshUpgradeButtons", "RefreshHeroUpgradeButtons",
            "RefreshFashionJewelryInfo", "RefreshMyFashionJewelryInfo",
            "RefreshHeroFashionJewelryInfo", "RefreshUserFashionJewelryInfo",
        };

        Assert.Equal(expected, TStateWindowsSurface.PublicNames);
    }

    [Fact]
    public void SurfaceLineRangesAreMonotonic()
    {
        for (int i = 1; i < TStateWindowsSurface.Decls.Length; i++)
            Assert.True(TStateWindowsSurface.Decls[i].Line > TStateWindowsSurface.Decls[i - 1].Line,
                "声明行号必须严格递增：" + TStateWindowsSurface.Decls[i].Name);
    }

    [Fact]
    public void SurfaceStartsAtPrivateSectionAndEndsAtPublicTail()
    {
        // 原文 965 = InitSelf（private 段第一个），1237 = RefreshUserFashionJewelryInfo（public 段最后一个）。
        Assert.Equal(965, TStateWindowsSurface.Decls[0].Line);
        Assert.Equal("InitSelf", TStateWindowsSurface.Decls[0].Name);
        Assert.Equal(TStateWindowsVisibility.Private, TStateWindowsSurface.Decls[0].Visibility);

        var last = TStateWindowsSurface.Decls[181];
        Assert.Equal(1237, last.Line);
        Assert.Equal("RefreshUserFashionJewelryInfo", last.Name);
        Assert.Equal(TStateWindowsVisibility.Public, last.Visibility);
    }

    [Fact]
    public void OriginalTyposArePreservedInSurface()
    {
        // 「原文如此」：ChangeMySelfMeridiansctivePage / DUSTitleAcitveMouseMove 都是原文拼写。
        Assert.Contains("ChangeMySelfMeridiansctivePage", TStateWindowsSurface.PublicNames);
        Assert.Contains("ChangeMyHeroMeridiansctivePage", TStateWindowsSurface.PublicNames);
        Assert.Contains("DUSTitleAcitveMouseMove", TStateWindowsSurface.PrivateNames);
        Assert.Contains("UpDate", TStateWindowsSurface.PublicNames); // 不是 Update
    }
}
