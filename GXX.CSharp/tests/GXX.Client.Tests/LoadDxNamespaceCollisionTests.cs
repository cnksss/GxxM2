using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// 命名空间撞车守卫（2026-09-20 集成期 CS0104 事故的回归锁）。
//
// 事故：车道 p2-dxcontrols-rest 把 `DxControls.pas`/`DxImageForm.pas` 的全量移植并入
// `GXX.Client.DxComponent` 后，本车道 `GXX.Client.LoadDx` 里早先自造的接缝类型与它**同名**，
// 于是任何同时 `using` 两个命名空间的文件（本项目全部测试）都报 CS0104。
//
// 本测试断言：两个命名空间的**公开类型简单名**交集必须为空。
// 一个类型只有一个归属（台账 §9.3）。若此测试变红，说明又有类型被两处声明：
//   * 若 `DxComponent` 已经有正式实现 → 删除 `LoadDx` 侧那份，改引用（本车道的做法）；
//   * 若 `LoadDx` 的接缝是唯一实现 → 不要在 `DxComponent` 重新声明，改由接缝承载。
// =====================================================================================
public sealed class LoadDxNamespaceCollisionTests
{
    private static HashSet<string> PublicTypeNames(string ns)
        => typeof(TGuiHeader).Assembly.GetTypes()
            .Where(t => t.Namespace == ns && t.DeclaringType == null && t.IsPublic)
            .Select(t => t.Name)
            .ToHashSet(StringComparer.Ordinal);

    [Fact]
    public void No_Type_Name_Is_Declared_In_Both_LoadDx_And_DxComponent()
    {
        var loadDx = PublicTypeNames("GXX.Client.LoadDx");
        var dxComponent = PublicTypeNames("GXX.Client.DxComponent");

        var collisions = loadDx.Intersect(dxComponent, StringComparer.Ordinal).OrderBy(n => n, StringComparer.Ordinal).ToArray();

        Assert.True(collisions.Length == 0,
            "GXX.Client.LoadDx 与 GXX.Client.DxComponent 出现同名类型（CS0104 隐患）："
            + string.Join(", ", collisions)
            + "。请删除其中一份声明并改引用（一个类型只有一个归属）。");
    }

    [Fact]
    public void Both_Namespaces_Are_Non_Empty_Sanity_Check()
    {
        // 防止"反射取不到类型"把上面的守卫变成永真断言。
        Assert.Contains("TDxControl", PublicTypeNames("GXX.Client.DxComponent"));
        Assert.Contains("GuiComponentLoader", PublicTypeNames("GXX.Client.LoadDx"));
        Assert.Contains("TGuiHeader", PublicTypeNames("GXX.Client.LoadDx"));
    }

    [Fact]
    public void Deduped_Types_Resolve_To_DxComponent_Owner()
    {
        // 去重后的归属断言：这 6 个类型必须只由 DxComponent 声明，LoadDx 侧不再声明。
        // 用字符串 + 反射取类型（而不是 typeof），这样本文件在"对方车道尚未合并"的工作树里也能编译。
        var assembly = typeof(TGuiHeader).Assembly;
        var loadDx = PublicTypeNames("GXX.Client.LoadDx");
        foreach (var name in new[] { "TAlignEx", "TDrawAligment", "TDxControlRef", "TDxImageForm", "TDxImageFormShape", "TDxScrollControl" })
        {
            Assert.DoesNotContain(name, loadDx);
            Assert.NotNull(assembly.GetType("GXX.Client.DxComponent." + name));
        }
    }

    [Fact]
    public void LoadDx_Own_Seams_Are_Still_Declared_Locally()
    {
        // 反向守卫：真正还没有正式归属的接缝必须留在 LoadDx，避免"顺手删干净"。
        var loadDx = PublicTypeNames("GXX.Client.LoadDx");
        foreach (var name in new[]
        {
            "TDxEdit", "TDxImageEdit", "TDxImageGrid", "TDxScrollControlSeam", "TDxScrollBox", "TDxChatMemo",
            "TDxListView", "TDxTreeView", "TDxPopupMenu", "TDxComboBox", "TDxPageControl", "TDxTabSheet",
            "TDxMainBottomForm", "TDxMagicBall", "TDxSexPanel", "TDxGroupAttackProgress", "TDxSwitchButton",
            "THashedStringList", "TDxControlAddressList",
        })
        {
            Assert.Contains(name, loadDx);
        }
    }

    [Fact]
    public void Adopted_Scroll_Base_Is_The_Official_One()
    {
        // 本车道承载面直接派生自正式基类；4 个子类派生自承载面；
        // 整族都必须落在正式 GXX.Client.DxComponent.TDxScrollControl 之下。
        const string official = "GXX.Client.DxComponent.TDxScrollControl";
        Assert.Equal(official, typeof(TDxScrollControlSeam).BaseType?.FullName);

        var family = new[] { typeof(TDxScrollControlSeam), typeof(TDxListView), typeof(TDxTreeView), typeof(TDxChatMemo), typeof(TDxScrollBox) };
        foreach (var t in family)
        {
            bool found = false;
            for (var b = t.BaseType; b != null; b = b.BaseType)
            {
                if (b.FullName == official) { found = true; break; }
            }
            Assert.True(found, $"{t.Name} 不在正式 TDxScrollControl 之下");
        }

        Assert.Equal("GXX.Client.LoadDx.TDxScrollControlSeam", typeof(TDxListView).BaseType?.FullName);
    }
}
