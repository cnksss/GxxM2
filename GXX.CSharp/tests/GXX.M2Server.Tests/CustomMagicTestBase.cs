// ============================================================================
// 车道 p5-m2-custommagic 的测试基础设施
//   * 串行化集合（多个测试类共享 TFrmCustomMagic 的静态接缝与窗体全局量）
//   * 无头开关复位（CustomMagicMessageBoxSeam.UiEnabled 必须为 false，否则挂死 testhost）
//   * 接缝静态量的逐测复位
// 注意：不加 AssemblyInfo、不改 csproj（照 p2-rungate-impl 车道的做法）。
// ============================================================================

using System;
using GXX.M2Server.Forms.CustomMagic;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>自定义技能窗体测试串行集合（共享静态接缝，禁并行）。</summary>
[CollectionDefinition("CustomMagicFormLane", DisableParallelization = true)]
public sealed class CustomMagicFormLane
{
}

/// <summary>自定义技能窗体测试基类：逐测复位接缝并把模态框关掉。</summary>
public abstract class CustomMagicTestBase : IDisposable
{
    /// <summary>复位全部接缝，禁用模态框（无头）。</summary>
    protected CustomMagicTestBase()
    {
        CustomMagicFormGlobals.Reset();
        CustomMagicMessageBoxSeam.UiEnabled = false;
        CustomMagicMessageBoxSeam.ShowModalHandler = null;
        TVtEditorFactory.Create = DefaultEditorFactory;
    }

    /// <summary>还原接缝。</summary>
    public virtual void Dispose()
    {
        CustomMagicFormGlobals.Reset();
        CustomMagicMessageBoxSeam.UiEnabled = true;
        TVtEditorFactory.Create = DefaultEditorFactory;
    }

    /// <summary>把 TVtEditorFactory.Create 复位为静态字段初始化时的默认实现。</summary>
    private static Func<TVtEditSpec, TWinControlSeam> DefaultEditorFactory { get; } = BuildDefaultFactory();

    private static Func<TVtEditSpec, TWinControlSeam> BuildDefaultFactory()
    {
        // 复制 CustomMagicSeams/CustomMagicEditLinkSeam 里字段初始化器的默认行为
        return spec => spec.Kind switch
        {
            TVtEditKind.SpinEdit => MakeSpin(spec),
            TVtEditKind.ComboBox => MakeCombo(spec),
            TVtEditKind.Edit => new TEditSeam { Visible = !spec.VisibleInitFalse, Text = spec.Text },
            _ => throw new InvalidOperationException("默认工厂不支持 None"),
        };
    }

    private static TSpinEditExSeam MakeSpin(TVtEditSpec spec)
    {
        var se = new TSpinEditExSeam { Value = spec.Value, Visible = !spec.VisibleInitFalse };
        if (spec.HasMinMax)
        {
            se.MinValue = spec.MinValue;
            se.MaxValue = spec.MaxValue;
        }
        return se;
    }

    private static TComboBoxSeam MakeCombo(TVtEditSpec spec)
    {
        var cbb = new TComboBoxSeam
        {
            Visible = !spec.VisibleInitFalse,
            Style = spec.DropDownList ? 3 : 0,
            ItemIndex = spec.ItemIndex,
        };
        cbb.Items.AddRange(spec.Items);
        return cbb;
    }

    /// <summary>造一个已装载的配置对象。</summary>
    protected static TCustomMagicConfig MakeConfig(string name = "技能A", ushort id = 5001, bool warr = false)
        => new(name, id, warr);

    /// <summary>造一个窗体并把当前配置指向给定对象（模拟 NodeClick 之后的稳定状态）。</summary>
    protected static TFrmCustomMagic MakeFormWith(TCustomMagicConfig cfg, int plusLevel = 0)
    {
        var form = new TFrmCustomMagic();
        form.FCurrentCustomConfig = cfg;
        form.FCurrentClientConfig = cfg.ClientConfigs[plusLevel];
        form.FCurrentServerConfig = cfg.ServerConfig;
        return form;
    }
}
