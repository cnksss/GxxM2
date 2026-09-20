// ============================================================================
// uFrmCustomMagic.pas **方法名 1:1 覆盖率门禁**
// 逐条断言：原文抽取表里的每个方法在托管侧都有同名公开成员。
// 来源表见 CustomMagicMethodTable.g.cs（脚本从 .pas 抽取，非手工转录）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GXX.M2Server.Forms.CustomMagic;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("CustomMagicFormLane")]
public sealed class CustomMagicMethodParityTests
{
    /// <summary>抽取表条数（232 = 212 窗体 + 10 + 10 两个 EditLink）。</summary>
    [Fact]
    public void Table_Has232Rows()
        => Assert.Equal(232, CustomMagicMethodTable.Count);

    public static IEnumerable<object[]> Rows()
    {
        foreach (var row in CustomMagicMethodTable.Rows)
            yield return new object[] { row };
    }

    [Theory]
    [MemberData(nameof(Rows))]
    public void EveryDelphiMethod_HasManagedCounterpart(CustomMagicMethodRow row)
    {
        Type target = row.Owner switch
        {
            "TFrmCustomMagic" => typeof(TFrmCustomMagic),
            "TDecAttribPropertyEditLink" => typeof(TDecAttribPropertyEditLink),
            "TElementPropertyEditLink" => typeof(TElementPropertyEditLink),
            _ => throw new InvalidOperationException($"未知 Owner {row.Owner}"),
        };

        var members = target.GetMember(row.Method,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        Assert.True(members.Length > 0,
            $"缺少成员 {target.Name}.{row.Method}（原文 {row.DelphiName} :{row.StartLine}-{row.EndLine}）");
    }

    /// <summary>
    /// 差异断言：两个 EditLink 的 10 个成员名**完全相同**（原文两份实现除
    /// PrepareEdit/EndEdit 的字段名外逐字相同），因此必须分别落在各自的类上。
    /// </summary>
    [Fact]
    public void BothEditLinks_HaveIdenticalMemberNames()
    {
        var dec = new List<string>();
        var elem = new List<string>();
        foreach (var row in CustomMagicMethodTable.Rows)
        {
            if (row.Owner == "TDecAttribPropertyEditLink") dec.Add(row.Method);
            if (row.Owner == "TElementPropertyEditLink") elem.Add(row.Method);
        }
        dec.Sort();
        elem.Sort();
        Assert.Equal(dec, elem);
        Assert.Equal(10, dec.Count);
        Assert.Contains("PrepareEdit", dec);
        Assert.Contains("EndEdit", dec);
        Assert.Contains("EditKeyDown", dec);
        Assert.Contains("EditKeyUp", dec);
    }

    /// <summary>原文 <c>interface</c> 段声明过 <c>ShowCustomMagic</c>，实现段又定义一次（:1625）。</summary>
    [Fact]
    public void ShowCustomMagic_ExistsOnceWithExpectedSignature()
    {
        var mi = typeof(TFrmCustomMagic).GetMethod("ShowCustomMagic",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        Assert.NotNull(mi);
        Assert.Equal(typeof(bool), mi!.ReturnType);
        Assert.Empty(mi.GetParameters());
    }

    [Fact]
    public void SetControlEnabled_IsStaticVoid()
    {
        var mi = typeof(TFrmCustomMagic).GetMethod("SetControlEnabled",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        Assert.NotNull(mi);
        Assert.Equal(typeof(void), mi!.ReturnType);
        Assert.Equal(2, mi.GetParameters().Length);
    }

    /// <summary>原文 :845-846 的 public 面：DoOpen 与 SetConfigChanged(IsChanged 默认 True)。</summary>
    [Fact]
    public void PublicDeclarations_MatchOriginal()
    {
        var doOpen = typeof(TFrmCustomMagic).GetMethod("DoOpen",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
        Assert.NotNull(doOpen);

        var setChanged = typeof(TFrmCustomMagic).GetMethod("SetConfigChanged",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(setChanged);
        var ps = setChanged!.GetParameters();
        Assert.Single(ps);
        Assert.True(ps[0].HasDefaultValue);
        Assert.Equal(true, ps[0].DefaultValue);
    }

    /// <summary>窗体私有状态的 4 个字段名 1:1（原文 :832-836）。</summary>
    [Fact]
    public void PrivateStateFields_MatchOriginal()
    {
        foreach (string name in new[]
                 {
                     "FCurrentCustomConfig", "FCurrentClientConfig", "FCurrentServerConfig", "FIsConfigChanged",
                 })
        {
            var fi = typeof(TFrmCustomMagic).GetField(name, BindingFlags.Public | BindingFlags.Instance);
            Assert.True(fi != null, $"缺少字段 {name}");
        }
    }

    /// <summary>两个 EditLink 的 4 个私有字段名 1:1（原文 :890-893 / :911-914）。</summary>
    [Fact]
    public void EditLinkFields_MatchOriginal()
    {
        foreach (var type in new[] { typeof(TDecAttribPropertyEditLink), typeof(TElementPropertyEditLink) })
        {
            foreach (string name in new[] { "FEdit", "FTree", "FNode", "FColumn" })
            {
                var fi = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
                         ?? type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
                Assert.True(fi != null, $"缺少字段 {type.Name}.{name}");
            }
        }
    }

    /// <summary>组件字段总数 600（原文 :17-616）。</summary>
    [Fact]
    public void ComponentFields_Are600()
    {
        int count = 0;
        foreach (var fi in typeof(TFrmCustomMagic).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (fi.Name.StartsWith("F", StringComparison.Ordinal) && fi.Name.Length > 1
                && char.IsUpper(fi.Name[1]))
                continue;   // FCurrent*/FIsConfigChanged 等状态字段
            if (fi.Name == "MainTreeHost" || fi.Name == "TreeHosts")
                continue;   // 接缝宿主的索引器（非原文组件）
            count++;
        }
        Assert.Equal(600, count);
    }
}
