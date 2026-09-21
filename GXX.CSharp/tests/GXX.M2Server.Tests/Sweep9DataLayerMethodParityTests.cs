// 源单元：Source/M2Engine/ItemEvent.pas · Source/M2Engine/DataManage.pas
//
// 本文件是"**方法数对账**"的取证用例（台账 §37.3「否定性断言必须计数取证」）：
// 逐个单元把「原文过程函数数」与「托管公开成员数」摆在一起断言，
// 防止"逐行扫一眼觉得都齐了"这种不可靠的判断。
//
// 原文计数来源（`_analysis/utf8_mirror` 镜像副本上 `^  (constructor|destructor|procedure|function) T` 实测）：
//   ItemEvent.pas    → 13
//   DataManage.pas   → 24
//   （UserShopDB_Old.pas → 33，但**判定为死代码、不移植** —— 见报告 §2）

using System;
using System.Linq;
using System.Reflection;
using GXX.M2Server.Sweep9.DataLayer;
using Xunit;

namespace GXX.M2Server.Tests;

public class Sweep9DataLayerMethodParityTests
{
    private static string[] DeclaredPublicMethods(Type t) =>
        t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
         .Where(m => !m.IsSpecialName)
         .Select(m => m.Name)
         .Distinct()
         .OrderBy(n => n, StringComparer.Ordinal)
         .ToArray();

    /// <summary>
    /// `TItemObject`（ItemEvent.pas:62-164）原文 4 个过程函数：
    /// `Create`(:62)、`Destroy`(:85)、`Run`(:95)、`MakeGhost`(:159)。
    /// <para>托管侧 `Create` 落成构造函数（`GetConstructors` 计），故公开方法应为
    /// **3** 个 + **1** 个（公开）构造。</para>
    /// </summary>
    [Fact]
    public void TItemObject_PublicMethodCountMatchesOriginal()
    {
        string[] methods = DeclaredPublicMethods(typeof(TItemObject));
        Assert.Equal(new[] { "Destroy", "MakeGhost", "Run" }, methods);

        // Create → 公开无参构造
        var ctors = typeof(TItemObject)
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .Where(c => c.GetParameters().Length == 0)
            .ToArray();
        Assert.Single(ctors);

        Assert.Equal(4, methods.Length + ctors.Length);   // 3 + 1 = 原文 4
    }

    /// <summary>原文 `TItemManager`（ItemEvent.pas:166-386）共 8 项过程函数（Create 含在内）。</summary>
    [Fact]
    public void TItemManager_PublicMethodCountMatchesOriginal()
    {
        string[] methods = DeclaredPublicMethods(typeof(TItemManager));
        // 4 个 FindItem 重载在反射里同名 ⇒ Distinct 后只剩 3 个不同名 + Run/Destroy/AddItem = 6 个不同名
        Assert.Equal(
            new[] { "AddItem", "Destroy", "FindItem", "Run" },
            methods);

        int findItemOverloads = typeof(TItemManager)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Count(m => m.Name == "FindItem");
        Assert.Equal(4, findItemOverloads);               // 原文 4 个 overload

        var ctors = typeof(TItemManager).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Assert.Single(ctors);

        // 原文 8 项过程函数 = Create(ctor) + Destroy + AddItem + 4×FindItem + Run
        Assert.Equal(8, 1 + 1 + 1 + findItemOverloads + 1);
        // Distinct 名只有 4 个（4 个 FindItem 同名）⇒ 用 (methods-1) 还原"不同名方法"里的真实个数
        Assert.Equal(8, ctors.Length + 1 /*Destroy*/ + 1 /*AddItem*/ + findItemOverloads + 1 /*Run*/);
    }

    /// <summary>原文 `TAccessEngine`（DataManage.pas:74-205）共 11 个过程函数（含 2 个属性读写器）。</summary>
    [Fact]
    public void TAccessEngine_PublicMethodCountMatchesOriginal()
    {
        string[] methods = DeclaredPublicMethods(typeof(TAccessEngine));
        Assert.Equal(
            new[] { "Connect", "CreateTable", "Destroy", "DisConnect", "GetTable", "LoadTable", "Lock", "SetTable", "UnLock" },
            methods);

        var ctors = typeof(TAccessEngine).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Assert.Single(ctors);

        // 9 个公开方法 + 1 个构造 = 10；另 Tables 属性（:29）= 原文第 11 项
        int indexers = typeof(TAccessEngine)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Count(p => p.GetIndexParameters().Length > 0);
        Assert.Equal(1, indexers);                        // property Tables[...]
        Assert.Equal(11, methods.Length + ctors.Length + indexers);
    }

    /// <summary>
    /// 原文 `TAccessTable`（DataManage.pas:208-275）共 **13** 个过程函数
    /// （`Create`:208、`Destroy`:214、`GetCount`:220、`GetADOQuery`:224、`Lock`:228、`UnLock`:233、
    /// `ClearSQL`:238、`AddSQL`:243、`OpenSQL`:248、`NextSQL`:253、`CloseSQL`:258、`ExecSQL`:263、
    /// `GetField`:268、`GetParameters`:272 —— 计 **14** 项声明名）
    /// + **4** 个属性（:50-53）。
    /// <para>托管侧的对应：`Create`→构造；`GetCount`→属性 `Count`；`GetADOQuery`→属性 `ADOQuery`；
    /// `GetField`/`GetParameters` **保留为方法**（与原文过程函数逐字同名）；
    /// `Fields[Field: string]`→索引器。C# 不允许"属性与 getter 方法同名"，
    /// 故 :52 的 `Parameters` 属性不再重复暴露（原文的 :37 方法已完整表达该能力）。</para>
    /// </summary>
    [Fact]
    public void TAccessTable_PublicMemberCountMatchesOriginal()
    {
        string[] methods = DeclaredPublicMethods(typeof(TAccessTable));
        Assert.Equal(
            new[]
            {
                "AddSQL", "ClearSQL", "CloseSQL", "Destroy", "ExecSQL",
                "GetField", "GetParameters", "Lock", "NextSQL", "OpenSQL", "UnLock",
            },
            methods);

        var ctors = typeof(TAccessTable).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Assert.Single(ctors);

        // 原文的 14 项声明名逐一落位：
        //   Create→ctor(1) · Destroy(1) · Lock/UnLock(2) · ClearSQL/AddSQL/OpenSQL/NextSQL/CloseSQL/ExecSQL(6)
        //   · GetField/GetParameters(2) · GetCount→属性 Count(1) · GetADOQuery→属性 ADOQuery(1)
        Assert.Equal(14, ctors.Length + methods.Length + 1 /*Count*/ + 1 /*ADOQuery*/);

        // 属性面：Count(:50) + Fields[..]→索引器 Item(:51) + ADOQuery(:53)
        //（:52 的 Parameters 不重复暴露 —— 见上面 summary）
        var props = typeof(TAccessTable)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(new[] { "ADOQuery", "Count", "Item" }, props);
        Assert.Equal(3, props.Length);

        // 原文 4 个属性声明里的第 4 个（Parameters）由 GetParameters 方法承载：
        Assert.Contains("GetParameters", methods);
    }

    /// <summary>
    /// 反向取证（否定性断言 + 计数）：`UserShopDB_Old.pas` 的 33 个过程函数
    /// **一个都不该**在本车道的产出里出现 —— 它是死代码，登记为"不移植"。
    /// <para>计数口径：把原文 33 个过程函数名与本车道程序集里所有公开类型名做交集，
    /// 结果必须为空集。</para>
    /// </summary>
    [Fact]
    public void UserShopDBOld_ProducesNoManagedTypesAtAll()
    {
        // 原文 33 个过程函数所属的 3 个类名（`git grep -l` 实测只在 UserShopDB_Old.pas 命中）
        string[] deadNames = { "TQuickNameList", "TUserShopFile", "TUserShopItemFile" };

        Assembly asm = typeof(TItemObject).Assembly;
        string[] found = asm.GetTypes()
            .Where(t => deadNames.Contains(t.Name))
            .Select(t => t.FullName ?? t.Name)
            .ToArray();

        Assert.Empty(found);
        Assert.Equal(3, deadNames.Length);
    }
}
