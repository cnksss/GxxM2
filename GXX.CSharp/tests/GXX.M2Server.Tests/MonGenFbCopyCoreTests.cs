using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J110：副本 MonGenInfo 副本共享语义（LocalDB.pas 3600-3641 +
/// Envir.pas 3687-3691 + NpcActionCmd.pas 23189-23194）1:1 测试。
/// </summary>
public sealed class MonGenFbCopyCoreTests
{
    private static MonGenFbCopyCore.MonGenInfo Template(
        string mapName = "$FB1", string monName = "祖玛教主", int race = 100,
        List<object>? certList = null)
        => new()
        {
            MapName = mapName,
            MonName = monName,
            X = 300,
            Y = 400,
            Range = 10,
            Count = 5,
            ZenTimeMs = 600_000,
            MonRace = race,
            BoNoManNoMon = MonGenFbCopyCore.FbNoManNoMon,
            BoFB = MonGenFbCopyCore.FbBoFB,
            Envir = MonGenFbCopyCore.FbTemplateEnvir,
            CertList = certList ?? new List<object>(),
        };

    // ===================== 模板名判定（3600-3602） =====================

    [Fact]
    public void DollarPrefixIsFbTemplate()
    {
        Assert.True(MonGenFbCopyCore.IsFbTemplateMapName("$FB1"));
    }

    [Fact]
    public void NoPrefixIsNotFbTemplate()
    {
        Assert.False(MonGenFbCopyCore.IsFbTemplateMapName("0"));
        Assert.False(MonGenFbCopyCore.IsFbTemplateMapName("FB1"));
    }

    [Fact]
    public void EmptyNameIsNotFbTemplate()
    {
        // 3600 原文 sMapName[1] = '$'，空串在 Delphi 中会越界，
        // 但本移植按"空串非模板"处理（等价于原文不会出现空串到此）
        Assert.False(MonGenFbCopyCore.IsFbTemplateMapName(""));
    }

    [Fact]
    public void StripFbPrefixRemovesDollar()
    {
        // 3602：Copy(sMapName, 2, MaxInt)
        Assert.Equal("FB1", MonGenFbCopyCore.StripFbPrefix("$FB1"));
    }

    [Fact]
    public void StripFbPrefixNoOpWithoutPrefix()
    {
        Assert.Equal("FB1", MonGenFbCopyCore.StripFbPrefix("FB1"));
    }

    [Fact]
    public void StripFbPrefixOnlyFirstDollar()
    {
        Assert.Equal("$A", MonGenFbCopyCore.StripFbPrefix("$$A"));
    }

    [Fact]
    public void FbPrefixConstant()
    {
        Assert.Equal('$', MonGenFbCopyCore.FbMapNamePrefix);
    }

    // ===================== 处置决策（3600-3641） =====================

    [Fact]
    public void NonTemplateGoesNormalPath()
    {
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.NotFbTemplate,
            MonGenFbCopyCore.SelectFbTemplateAction("0", 5, 100));
    }

    [Fact]
    public void ManagerNotFoundIsSilentDrop()
    {
        // 3604：IndexOf = -1 → 原文**什么都不做**（无 else 分支）
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.ManagerNotFound,
            MonGenFbCopyCore.SelectFbTemplateAction("$FB1", MonGenFbCopyCore.IndexNotFound, 100));
    }

    [Fact]
    public void RaceNotFoundDisposes()
    {
        // 3610 + 3639
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.RaceNotFound,
            MonGenFbCopyCore.SelectFbTemplateAction("$FB1", 0, MonGenFbCopyCore.MonRaceNotFound));
    }

    [Fact]
    public void ValidTemplateRegisters()
    {
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.RegisterAndCreateCopies,
            MonGenFbCopyCore.SelectFbTemplateAction("$FB1", 0, 100));
    }

    [Fact]
    public void ManagerCheckPrecedesRaceCheck()
    {
        // **顺序**：3604 先于 3610——两者都失败时得到 ManagerNotFound
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.ManagerNotFound,
            MonGenFbCopyCore.SelectFbTemplateAction("$FB1", MonGenFbCopyCore.IndexNotFound, -1));
    }

    [Fact]
    public void TemplateCheckPrecedesAll()
    {
        Assert.Equal(MonGenFbCopyCore.FbTemplateAction.NotFbTemplate,
            MonGenFbCopyCore.SelectFbTemplateAction("0", MonGenFbCopyCore.IndexNotFound, -1));
    }

    [Fact]
    public void RaceMinusOneConstant()
    {
        Assert.Equal(-1, MonGenFbCopyCore.MonRaceNotFound);
        Assert.Equal(-1, MonGenFbCopyCore.IndexNotFound);
    }

    [Fact]
    public void ShouldRegisterFbTemplateMatchesRaceCheck()
    {
        Assert.True(MonGenFbCopyCore.ShouldRegisterFbTemplate(0));
        Assert.True(MonGenFbCopyCore.ShouldRegisterFbTemplate(100));
        Assert.False(MonGenFbCopyCore.ShouldRegisterFbTemplate(-1));
    }

    [Fact]
    public void CertListOnlyCreatedOnRegister()
    {
        // 3612：CertList 创建在 nRace <> -1 分支内
        Assert.True(MonGenFbCopyCore.CreatesCertList(
            MonGenFbCopyCore.FbTemplateAction.RegisterAndCreateCopies));
        Assert.False(MonGenFbCopyCore.CreatesCertList(MonGenFbCopyCore.FbTemplateAction.RaceNotFound));
        Assert.False(MonGenFbCopyCore.CreatesCertList(MonGenFbCopyCore.FbTemplateAction.ManagerNotFound));
        Assert.False(MonGenFbCopyCore.CreatesCertList(MonGenFbCopyCore.FbTemplateAction.NotFbTemplate));
    }

    // ===================== 副本路径 vs 普通路径的字段差异 =====================

    [Fact]
    public void FbPathConstants()
    {
        // 3606-3608
        Assert.False(MonGenFbCopyCore.FbNoManNoMon);
        Assert.True(MonGenFbCopyCore.FbBoFB);
        Assert.Null(MonGenFbCopyCore.FbTemplateEnvir);
    }

    [Fact]
    public void NormalPathConstants()
    {
        // 3650/3652
        Assert.True(MonGenFbCopyCore.NormalNoManNoMon);
        Assert.False(MonGenFbCopyCore.NormalBoFB);
    }

    [Fact]
    public void FbAndNormalPathsDifferOnBothFlags()
    {
        // **差异保护**：两条路径对同名字段取**相反**值，不可统一
        Assert.NotEqual(MonGenFbCopyCore.FbNoManNoMon, MonGenFbCopyCore.NormalNoManNoMon);
        Assert.NotEqual(MonGenFbCopyCore.FbBoFB, MonGenFbCopyCore.NormalBoFB);
    }

    // ===================== 副本构造（3630-3635） =====================

    [Fact]
    public void CopySharesCertListReference()
    {
        // **核心语义**：3630 注释"共 MonGenInfo.CertList，只引用指针，不管理对象"
        var template = Template();
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        Assert.True(MonGenFbCopyCore.SharesCertList(template, copy));
        Assert.Same(template.CertList, copy.CertList);
    }

    [Fact]
    public void CopyCertListIsNotANewList()
    {
        // 不是内容相同的新表，而是**同一实例**
        var certs = new List<object> { "c1", "c2" };
        var template = Template(certList: certs);
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        copy.CertList!.Add("c3");   // 通过副本改动

        Assert.Equal(3, template.CertList!.Count);   // 原件可见
        Assert.Same(certs, copy.CertList);
    }

    [Fact]
    public void AllCopiesShareSameCertList()
    {
        // 多个副本地图共享**同一份**凭证表
        var template = Template();
        var maps = new List<(string, object)>
        {
            ("FB1", new object()), ("FB2", new object()), ("FB3", new object()),
        };

        var copies = MonGenFbCopyCore.CreateFbCopies(template, maps);

        Assert.Equal(3, copies.Count);
        foreach (var c in copies)
            Assert.True(MonGenFbCopyCore.SharesCertList(template, c));
    }

    [Fact]
    public void CopyRewritesBoFBToTrue()
    {
        // 3632
        var template = Template();
        template.BoFB = false;   // 即便原件为假

        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());
        Assert.True(copy.BoFB);
    }

    [Fact]
    public void CopyRewritesMapName()
    {
        // 3633：改写为具体地图名
        var template = Template(mapName: "$FB1");
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "真实地图", new object());

        Assert.Equal("真实地图", copy.MapName);
        Assert.Equal("$FB1", template.MapName);   // 原件保留模板名
    }

    [Fact]
    public void CopyRewritesEnvir()
    {
        // 3634
        var template = Template();
        var envir = new object();
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", envir);

        Assert.Same(envir, copy.Envir);
        Assert.Null(template.Envir);
    }

    [Fact]
    public void CopyPreservesOtherFields()
    {
        // 3631 的整记录值拷贝
        var template = Template(monName: "赤月恶魔", race: 115);
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        Assert.Equal("赤月恶魔", copy.MonName);
        Assert.Equal(115, copy.MonRace);
        Assert.Equal(300, copy.X);
        Assert.Equal(400, copy.Y);
        Assert.Equal(10, copy.Range);
        Assert.Equal(5, copy.Count);
        Assert.Equal(600_000, copy.ZenTimeMs);
        Assert.False(copy.BoNoManNoMon);
    }

    [Fact]
    public void CopyIsMarkedAsCopy()
    {
        var template = Template();
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        Assert.True(copy.IsFbCopy);
        Assert.False(template.IsFbCopy);
    }

    [Fact]
    public void TemplateMapNameNotModifiedByCopyCreation()
    {
        // 副本构造**不得**改动原件（值拷贝语义）
        var template = Template(mapName: "$FB1", monName: "祖玛");
        var before = template.MapName;

        MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        Assert.Equal(before, template.MapName);
        Assert.Equal("祖玛", template.MonName);
    }

    [Fact]
    public void CopyOfCopyStillSharesCertList()
    {
        // 副本再派生副本仍共享同一表
        var template = Template();
        var copy1 = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());
        var copy2 = MonGenFbCopyCore.CreateFbCopy(copy1, "FB2", new object());

        Assert.True(MonGenFbCopyCore.SharesCertList(template, copy2));
        Assert.Same(template.CertList, copy2.CertList);
    }

    [Fact]
    public void CreateCopiesEmptyList()
    {
        var template = Template();
        var copies = MonGenFbCopyCore.CreateFbCopies(template, new List<(string, object)>());

        Assert.Empty(copies);
    }

    [Fact]
    public void CreateCopiesPreservesOrder()
    {
        var template = Template();
        var maps = new List<(string, object)>
        {
            ("A", new object()), ("B", new object()),
        };

        var copies = MonGenFbCopyCore.CreateFbCopies(template, maps);

        Assert.Equal("A", copies[0].MapName);
        Assert.Equal("B", copies[1].MapName);
    }

    [Fact]
    public void EachCopyGetsItsOwnEnvir()
    {
        var template = Template();
        var e1 = new object();
        var e2 = new object();

        var copies = MonGenFbCopyCore.CreateFbCopies(template,
            new List<(string, object)> { ("A", e1), ("B", e2) });

        Assert.Same(e1, copies[0].Envir);
        Assert.Same(e2, copies[1].Envir);
        Assert.NotSame(copies[0].Envir, copies[1].Envir);
    }

    // ===================== 释放语义（Envir.pas 3687-3691） =====================

    [Fact]
    public void FbCopyNeverDisposesCertList()
    {
        // **关键**：副本 Dispose 不得释放共享的 CertList，否则原件与其余副本悬空
        Assert.False(MonGenFbCopyCore.ShouldDisposeCertListOfFbCopy());
    }

    [Fact]
    public void DisposeFbListDisposesEachRecord()
    {
        var list = new List<string> { "a", "b", "c" };
        var disposed = new List<string>();

        MonGenFbCopyCore.DisposeFbMonGenList(list, s => disposed.Add(s));

        Assert.Equal(new[] { "a", "b", "c" }, disposed);
    }

    [Fact]
    public void DisposeFbListDoesNotTouchCertList()
    {
        // 释放后 CertList 内容仍然完整（因未释放）
        var template = Template();
        template.CertList!.Add("cert1");

        var copies = MonGenFbCopyCore.CreateFbCopies(template,
            new List<(string, object)> { ("A", new object()), ("B", new object()) });

        MonGenFbCopyCore.DisposeFbMonGenList(copies, _ => { /* Dispose 记录本身 */ });

        Assert.Single(template.CertList!);   // 凭证表未被清空
        Assert.Equal("cert1", template.CertList![0]);
    }

    [Fact]
    public void DisposeIsSafeWithManyCopies()
    {
        // 多副本释放后，共享表仍被原件持有且内容完好
        var template = Template();
        template.CertList!.AddRange(new object[] { "x", "y" });

        var copies = MonGenFbCopyCore.CreateFbCopies(template,
            new List<(string, object)> { ("A", new object()), ("B", new object()), ("C", new object()) });

        MonGenFbCopyCore.DisposeFbMonGenList(copies, _ => { });

        Assert.Equal(2, template.CertList!.Count);
    }

    [Fact]
    public void DisposeEmptyListIsNoOp()
    {
        int calls = 0;
        MonGenFbCopyCore.DisposeFbMonGenList(new List<string>(), _ => calls++);
        Assert.Equal(0, calls);
    }

    // ===================== 登记顺序与消费 =====================

    [Fact]
    public void TemplateRegisteredBeforeCopies()
    {
        // 3619 先于 3635
        Assert.Equal("m_MonGenList(原件)", MonGenFbCopyCore.RegistrationOrder[0]);
        Assert.Equal("m_FBMonGenList(副本)", MonGenFbCopyCore.RegistrationOrder[1]);
    }

    [Fact]
    public void RegistrationOrderHasTwoSteps()
    {
        Assert.Equal(2, MonGenFbCopyCore.RegistrationOrder.Length);
    }

    [Fact]
    public void ConsumerUsesCountForRegen()
    {
        // NpcActionCmd.pas 23193：RegenMonsters(MonGen, MonGen.nCount)
        // 故副本的 nCount 来自 3631 的值拷贝
        var template = Template();
        template.Count = 7;

        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        Assert.Equal(7, copy.Count);
    }

    [Fact]
    public void CopyCountIndependentOfTemplateAfterCreation()
    {
        // 值拷贝：之后改原件不影响已建副本
        var template = Template();
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        template.Count = 99;

        Assert.Equal(5, copy.Count);        // 副本保持原值
        Assert.Equal(99, template.Count);
    }

    [Fact]
    public void CopyMapNameIndependentOfTemplateAfterCreation()
    {
        var template = Template(mapName: "$FB1");
        var copy = MonGenFbCopyCore.CreateFbCopy(template, "FB1", new object());

        template.MapName = "$CHANGED";

        Assert.Equal("FB1", copy.MapName);
    }

    // ===================== 与 J109 排序查找的衔接 =====================

    [Fact]
    public void CopiesSortByRealMapNameNotTemplateName()
    {
        // 副本以**真实地图名**参与排序，模板名（带 $）只出现在原件上
        var template = Template(mapName: "$ZZZ");

        var copies = MonGenFbCopyCore.CreateFbCopies(template,
            new List<(string, object)> { ("AAA", new object()), ("MMM", new object()) });

        MonGenLoadCore.SortByMapName(copies, c => c.MapName);

        Assert.Equal("AAA", copies[0].MapName);
        Assert.Equal("MMM", copies[1].MapName);
    }

    [Fact]
    public void TemplateNameStillHasDollarPrefix()
    {
        // 原件保留带 $ 的模板名，故在按地图名的查找中不会被真实地图名命中
        var template = Template(mapName: "$FB1");

        var copies = MonGenFbCopyCore.CreateFbCopies(template,
            new List<(string, object)> { ("FB1", new object()) });

        Assert.True(MonGenFbCopyCore.IsFbTemplateMapName(template.MapName));
        Assert.False(MonGenFbCopyCore.IsFbTemplateMapName(copies[0].MapName));
        Assert.NotEqual(template.MapName, copies[0].MapName);
    }
}
