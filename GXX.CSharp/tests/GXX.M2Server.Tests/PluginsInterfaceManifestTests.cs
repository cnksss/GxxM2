using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;
using GXX.M2Server.Plugins;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 并行批次P2b：PluginInterface.pas / PluginImplement.pas 插件层 1:1 移植的**接口面齐全性**测试。
///
/// 核心断言（本车道最重要的测试）：
///   1. 用反射取出 <c>GXX.M2Server.Plugins</c> 里全部 stdcall 委托，与脚本从
///      PluginInterface.pas 抽取的 736 条 procedural type（PluginsData/PluginInterface.manifest.tsv）
///      **逐条一致**：名字集合相等、条数相等、每条的行号注释与原文行号一致。
///   2. 宿主实现面与 PluginImplement.pas 的 741 个 `_T*` 例程一一对应
///      （734 条有对应类型声明 + 7 条原文只在实现里存在的）。
///   3. 21 个 Pack=1 记录的字段名/顺序与抽取清单一致（`SizeOf` 断言）。
/// </summary>
public sealed class PluginsInterfaceManifestTests
{
    private static readonly Assembly M2Asm = typeof(PluginInterfaceHost).Assembly;

    // ---------------------------------------------------------------- 数据文件定位

    /// <summary>从测试程序集位置向上找到 GXX.CSharp 根（定位 PluginsData）。</summary>
    internal static string DataDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "tests", "GXX.M2Server.Tests", "PluginsData");
            if (Directory.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("找不到 tests/GXX.M2Server.Tests/PluginsData");
    }

    internal static List<string[]> ReadTsv(string name)
    {
        var path = Path.Combine(DataDir(), name);
        var rows = new List<string[]>();
        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Trim().Length == 0) continue;
            rows.Add(line.Split('\t'));
        }
        return rows;
    }

    private static List<Type> DelegateTypes() =>
        M2Asm.GetTypes()
             .Where(t => t.IsSubclassOf(typeof(Delegate)) && t.Namespace == "GXX.M2Server.Plugins")
             .OrderBy(t => t.Name, StringComparer.Ordinal)
             .ToList();

    // ---------------------------------------------------------------- 1) 736 条 procedural type

    /// <summary>
    /// 声明集合与抽取清单逐条一致：名字、条数、原文行号。
    /// </summary>
    [Fact]
    public void ProceduralTypes_MatchExtractedManifest_NameByLineByLine()
    {
        var manifest = ReadTsv("PluginInterface.manifest.tsv");
        Assert.Equal(736, manifest.Count);

        // manifest: Name <TAB> Line <TAB> OriginalSignature
        var expected = manifest.ToDictionary(r => r[0], r => int.Parse(r[1]), StringComparer.Ordinal);
        var actual = DelegateTypes()
            .Where(t => !t.Name.StartsWith("TSeam_", StringComparison.Ordinal))
            .ToDictionary(t => t.Name, t => t, StringComparer.Ordinal);

        // 手工补齐的 3 个：TNotifyEventEx（原文不是 X = function 形式）+ 2 个原文漏声明类型
        var manual = new[] { "TNotifyEventEx", "TM2Engine_GetOtherFileDir", "TBaseObject_TrainSkill", "TPlugInit", "TPlugUnInit" };

        var missing = expected.Keys.Where(k => !actual.ContainsKey(k)).OrderBy(k => k, StringComparer.Ordinal).ToList();
        Assert.True(missing.Count == 0, "缺少委托声明：" + string.Join(", ", missing));

        var unexpected = actual.Keys
            .Where(k => !expected.ContainsKey(k) && !manual.Contains(k))
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToList();
        Assert.True(unexpected.Count == 0, "多出的委托声明：" + string.Join(", ", unexpected));

        Assert.Equal(736 + manual.Length, actual.Count);

        // 行号校验：每个委托的 XML 注释里必须带 PluginInterface.pas:<原行号>
        var bad = new List<string>();
        foreach (var kv in expected)
        {
            var xml = GetSummary(actual[kv.Key]);
            if (xml is null || !xml.Contains("PluginInterface.pas:" + kv.Value))
                bad.Add(kv.Key + " expect " + kv.Value + " got " + (xml ?? "<null>"));
        }
        Assert.True(bad.Count == 0, "line-number comment mismatch (first 5): " + string.Join(" | ", bad.Take(5)));
    }

    /// <summary>全部 736 条必须是 `[UnmanagedFunctionPointer(CallingConvention.StdCall)]`。</summary>
    [Fact]
    public void ProceduralTypes_AreAllStdCall()
    {
        var manifest = ReadTsv("PluginInterface.manifest.tsv").Select(r => r[0]).ToHashSet(StringComparer.Ordinal);
        var checkedCount = 0;
        foreach (var t in DelegateTypes().Where(t => manifest.Contains(t.Name)))
        {
            var attr = t.GetCustomAttribute<UnmanagedFunctionPointerAttribute>();
            Assert.NotNull(attr);
            Assert.Equal(CallingConvention.StdCall, attr!.CallingConvention);
            checkedCount++;
        }
        Assert.Equal(736, checkedCount);
    }

    /// <summary>
    /// 抽查原文档里几个"关键签名"的 C# 形参类型映射（BOOL→int、PAnsiChar→byte[]、
    /// var DestLen→ref uint、pT*→ref T*、NativeInt→IntPtr）。
    /// </summary>
    [Theory]
    [InlineData("TStrList_GetText", new[] { "IStringListHandle", "Byte[]", "UInt32&" }, "Int32")]
    [InlineData("TMemory_Alloc", new[] { "Int32" }, "IntPtr")]
    [InlineData("TBaseObject_GetAbility", new[] { "IBaseObjectHandle", "TAbility&" }, "Int32")]
    [InlineData("TBaseObject_SendMsg", new[]
    {
        "IBaseObjectHandle", "IBaseObjectHandle", "Int32", "Int32", "IntPtr", "IntPtr", "IntPtr", "Byte[]"
    }, "Void")]
    [InlineData("TMenu_Add", new[] { "IntPtr", "IMenuItem", "Byte[]", "Int32", "TNotifyEventEx" }, "IMenuItem")]
    [InlineData("TGuild_GetMaster", new[] { "IGuildHandle", "IPlayObjectHandle&", "IPlayObjectHandle&" }, "Void")]
    public void ProceduralTypes_KeySignatures_MapPerConversionRules(string name, string[] paramTypes, string returnType)
    {
        var t = DelegateTypes().Single(x => x.Name == name);
        var invoke = t.GetMethod("Invoke")!;
        Assert.Equal(paramTypes, invoke.GetParameters().Select(p => p.ParameterType.Name).ToArray());
        Assert.Equal(returnType, invoke.ReturnType.Name);
    }

    /// <summary>
    /// 原文笔误照抄：PluginInterface.pas:255/258 的 procedural type 名拼作 `TStrLit_*`
    /// （Lit 而非 List），而实现体（PluginImplement.pas:1139/1145）写作 `_TStrList_*`。
    /// 本层保留原文类型名，且**不**为它们生成实现体。
    /// </summary>
    [Fact]
    public void OriginalTypos_TStrLit_TypesArePreservedVerbatim()
    {
        var manifest = ReadTsv("PluginInterface.manifest.tsv").ToDictionary(r => r[0], r => r[2], StringComparer.Ordinal);
        Assert.True(manifest.ContainsKey("TStrLit_LoadFromFile"));
        Assert.True(manifest.ContainsKey("TStrLit_SaveToFile"));
        Assert.Contains("TStrLit_LoadFromFile = procedure(Strings: _TStringList; FileName: PAnsiChar); stdcall;", manifest["TStrLit_LoadFromFile"]);
        Assert.Contains("TStrLit_SaveToFile = procedure(Strings: _TStringList; FileName: PAnsiChar); stdcall;", manifest["TStrLit_SaveToFile"]);

        // 声明存在
        Assert.NotNull(DelegateTypes().SingleOrDefault(t => t.Name == "TStrLit_LoadFromFile"));
        Assert.NotNull(DelegateTypes().SingleOrDefault(t => t.Name == "TStrLit_SaveToFile"));
        // 实现体名字不同（实现里写作 TStrList_*，原文如此，不"补全"成同一个名字）
        Assert.NotNull(typeof(PluginInterfaceHost).GetMethod("TStrList_LoadFromFile"));
        Assert.NotNull(typeof(PluginInterfaceHost).GetMethod("TStrList_SaveToFile"));
        // TStrLit_* 只作为"声明"存在；实现体是原文笔误的另一半，故托管侧为未完成壳（能取到，但方法体抛接缝）
        Assert.NotNull(typeof(PluginInterfaceHost).GetMethod("TStrLit_LoadFromFile"));
        Assert.NotNull(typeof(PluginInterfaceHost).GetMethod("TStrLit_SaveToFile"));
    }

    // ---------------------------------------------------------------- 2) PluginImplement 741 个例程

    /// <summary>
    /// 宿主实现面：PluginImplement.pas 的 734 个"有类型声明"的例程 +
    /// 7 个"只在实现里存在"的例程（原文缺陷），全部要有对应托管方法。
    /// </summary>
    [Fact]
    public void HostCallbacks_CoverAll741RoutinesInPluginImplement()
    {
        var bodies = ReadTsv("PluginImplement.bodies.tsv");
        var extras = ReadTsv("PluginImplement.extra.tsv");
        Assert.Equal(734, bodies.Count);
        Assert.Equal(7, extras.Count);

        var methods = typeof(PluginInterfaceHost)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        var missing = new List<string>();
        foreach (var row in bodies.Concat(extras))
        {
            var expected = ToManagedName(row[0]);
            if (!methods.Contains(expected)) missing.Add($"{row[0]} -> {expected}");
        }
        Assert.True(missing.Count == 0, "宿主实现缺失：" + string.Join(", ", missing));

        // 2 条原文类型声明笔误（TStrLit_*）没有实现体，必须显式登记
        var missingBodies = ReadTsv("PluginInterface.missing-bodies.tsv");
        Assert.Equal(2, missingBodies.Count);
        Assert.Equal(new[] { "TStrLit_LoadFromFile", "TStrLit_SaveToFile" }, missingBodies.Select(r => r[0]).OrderBy(x => x).ToArray());
    }

    /// <summary>原文 7 个"只在实现里存在"的 `_T*` 例程必须逐条照抄在册。</summary>
    [Fact]
    public void ExtraRoutines_AreTheSevenKnownOriginalDefects()
    {
        var extras = ReadTsv("PluginImplement.extra.tsv").Select(r => r[0]).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[]
        {
            "_TBaseObject_TrainSkill",
            "_TM2Engine_GetOtherFileDir",
            "_TPlayObject_GetAlcohol",
            "_TPlayObject_GetHeroM2ShopList",
            "_TPlayObject_GetHeroM2ShopOpenList",
            "_TStrList_LoadFromFile",
            "_TStrList_SaveToFile",
        }, extras);
    }

    // ---------------------------------------------------------------- 3) TAppFuncDef 布局

    /// <summary>
    /// TAppFuncDef（PluginInterface.pas:3162-3184）：21 个字段，尾随 `Reserved: array[0..999] of Pointer`。
    /// Pack=1 ⇒ 大小 = 20*PointerSize + 1000*PointerSize（第一字段 PluginID 也是 NativeInt）。
    /// </summary>
    [Fact]
    public void TAppFuncDef_LayoutMatchesOriginal()
    {
        var fields = typeof(TAppFuncDef).GetFields(BindingFlags.Public | BindingFlags.Instance);
        Assert.Equal(21, fields.Length);
        Assert.Equal(
            new[]
            {
                "PluginID", "Memory", "List", "StringList", "MemStream", "Menu", "IniFile", "MagicACList",
                "MapManager", "Envir", "M2Engine", "BaseObject", "Smarter", "Player", "Dummy", "Hero", "Npc",
                "UserEngine", "GuildManager", "Guild", "Reserved",
            },
            fields.Select(f => f.Name).ToArray());

        Assert.Equal(24024, Marshal.SizeOf<TAppFuncDef>());

        // 布局来源校验：反射看不到 [StructLayout]（.NET 运行时把它折算成类型布局），
        // 故直接读生成源码里的 Attributes / Pack，与原文 `packed record` 语义对齐。
        var srcAttr = TypeAttributeLines("TAppFuncDef");
        Assert.Contains("[StructLayout(LayoutKind.Sequential, Pack = 1)]", srcAttr);
    }

    /// <summary>从生成源码里取某个 struct 声明前的属性行。</summary>
    private static string TypeAttributeLines(string structName)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var file = Path.Combine(dir.FullName, "src", "GXX.M2Server", "Plugins", "PluginInterfaceTables.g.cs");
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("struct " + structName) && !lines[i].Contains("class"))
                    {
                        return (i > 0 ? lines[i - 1] : string.Empty) + "\n" + lines[i];
                    }
                }
            }
            dir = dir.Parent;
        }
        return string.Empty;
    }

    /// <summary>
    /// 21 个记录的字段清单必须与脚本抽取的 fields.tsv 完全一致（名称 + 顺序 + 类型）。
    /// </summary>
    [Fact]
    public void AllFunctionTables_FieldListsMatchExtractedInventory()
    {
        // 全部 21 个记录的字段清单必须与脚本抽取的 fields.tsv 一致（名称 + 顺序）
        var fields = ReadTsv("PluginInterface.fields.tsv");
        var byRecord = fields.GroupBy(r => r[0])
            .ToDictionary(g => g.Key, g => g.OrderBy(r => int.Parse(r[1])).Select(r => r[3]).ToArray());

        var recordTypes = new (string Record, Type Type)[]
        {
            ("TScriptCmdParam", typeof(TScriptCmdParam)),
            ("TMemoryFunc", typeof(TMemoryFunc)),
            ("TListFunc", typeof(TListFunc)),
            ("TStringListFunc", typeof(TStringListFunc)),
            ("TMemoryStreamFunc", typeof(TMemoryStreamFunc)),
            ("TMemuFunc", typeof(TMemuFunc)),
            ("TIniFileFunc", typeof(TIniFileFunc)),
            ("TMagicACListFunc", typeof(TMagicACListFunc)),
            ("TMapManagerFunc", typeof(TMapManagerFunc)),
            ("TEnvirnomentFunc", typeof(TEnvirnomentFunc)),
            ("TM2EngineFunc", typeof(TM2EngineFunc)),
            ("TBaseObjectFunc", typeof(TBaseObjectFunc)),
            ("TSmartObjectFunc", typeof(TSmartObjectFunc)),
            ("TPlayObjectFunc", typeof(TPlayObjectFunc)),
            ("TDummyObjectFunc", typeof(TDummyObjectFunc)),
            ("THeroObjectFunc", typeof(THeroObjectFunc)),
            ("TNormNpcFunc", typeof(TNormNpcFunc)),
            ("TUserEngineFunc", typeof(TUserEngineFunc)),
            ("TGuildManagerFunc", typeof(TGuildManagerFunc)),
            ("TGuildFunc", typeof(TGuildFunc)),
            ("TAppFuncDef", typeof(TAppFuncDef)),
        };

        Assert.Equal(21, recordTypes.Length);
        foreach (var (record, type) in recordTypes)
        {
            Assert.True(byRecord.ContainsKey(record), "抽取清单里没有记录 " + record);
            var actual = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(f => f.Name).ToArray();
            Assert.Equal(byRecord[record], actual);
        }
    }

    /// <summary>
    /// 宿主方法的命名规则：**就是原文例程名去掉前导下划线**（`_TMemory_Alloc` → `TMemory_Alloc`），
    /// 因此反射结果可与 Delphi 例程名做纯字符串比对，不会因"美化命名"而漏项。
    /// </summary>
    internal static string ToManagedName(string delphiRoutineName)
        => delphiRoutineName.StartsWith("_", StringComparison.Ordinal)
            ? delphiRoutineName.Substring(1)
            : delphiRoutineName;

    /// <summary>取类型的声明行（用于校验原文行号注释）。</summary>
    private static string GetSummary(MemberInfo member)
    {
        // 反射拿不到 XML doc；改从生成的源码文件里按"声明上一行的注释"检索。
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var file = Path.Combine(dir.FullName, "src", "GXX.M2Server", "Plugins", "PluginInterfaceTypes.g.cs");
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 1; i < lines.Length; i++)
                {
                    if (lines[i].Contains("public delegate ") && lines[i].Contains(" " + member.Name + "("))
                    {
                        // 声明行上面依次是 [UnmanagedFunctionPointer(...)] 与 <summary>，取注释那一行
                        var j = i - 1;
                        while (j >= 0 && lines[j].Contains("[UnmanagedFunctionPointer")) j--;
                        return j >= 0 ? lines[j] : lines[i - 1];
                    }
                }
            }
            dir = dir.Parent;
        }
        return null;
    }
}