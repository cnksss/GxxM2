// ============================================================================
// uFrmCustomMagic.pas 151 个"单字段写回"处理器 × 3 分支（表驱动）
// 源单元：Source\M2Engine\Forms\uFrmCustomMagic.pas
// 期望值不手工转录：来源表由脚本从该 .pas 抽取（见 CustomMagicHandlerTable.g.cs 头注释）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GXX.M2Server.Forms.CustomMagic;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("CustomMagicFormLane")]
public sealed class CustomMagicFormHandlerTests : CustomMagicTestBase
{
    /// <summary>表里必须恰好 151 行（原文形状匹配结果）。</summary>
    [Fact]
    public void Table_Has151Rows()
        => Assert.Equal(151, CustomMagicHandlerTable.Count);

    /// <summary>表里每一行都必须在 TFrmCustomMagic 上找到同名无参公开方法。</summary>
    [Fact]
    public void All151Handlers_ExistOnForm()
    {
        foreach (var row in CustomMagicHandlerTable.Rows)
        {
            var mi = typeof(TFrmCustomMagic).GetMethod(row.Method, BindingFlags.Public | BindingFlags.Instance,
                null, Type.EmptyTypes, null);
            Assert.True(mi != null, $"缺少方法 {row.Method}（原文 {row.DelphiName} :{row.StartLine}-{row.EndLine}）");
        }
    }

    /// <summary>表里每一行的来源控件字段必须存在且类型匹配。</summary>
    [Fact]
    public void All151Handlers_SourceControlsExist()
    {
        foreach (var row in CustomMagicHandlerTable.Rows)
        {
            var fi = typeof(TFrmCustomMagic).GetField(row.Control, BindingFlags.Public | BindingFlags.Instance);
            Assert.True(fi != null, $"缺少控件字段 {row.Control}（{row.Method}）");

            Type expected = row.ControlKind switch
            {
                "spin" => typeof(TSpinEditExSeam),
                "check" => typeof(TCheckBoxSeam),
                "edit" => typeof(TEditSeam),
                "combo" => typeof(TComboBoxSeam),
                _ => throw new InvalidOperationException($"未知控件种类 {row.ControlKind}"),
            };
            Assert.True(expected.IsAssignableFrom(fi!.FieldType),
                $"{row.Control} 类型应为 {expected.Name}，实际 {fi.FieldType.Name}");
        }
    }

    public static IEnumerable<object[]> Rows()
    {
        foreach (var row in CustomMagicHandlerTable.Rows)
            yield return new object[] { row };
    }

    /// <summary>
    /// 分支 1：守卫对象为 nil → 原文整段跳过：不写、不置 Changed、不抛异常。
    /// </summary>
    [Theory]
    [MemberData(nameof(Rows))]
    public void Handler_NullGuard_DoesNothing(CustomMagicHandlerRow row)
    {
        using var form = new TFrmCustomMagic();
        form.FCurrentCustomConfig = null;
        form.FCurrentClientConfig = null;
        form.FCurrentServerConfig = null;

        SetControl(form, row, 3);
        Invoke(form, row);

        Assert.Null(form.FCurrentCustomConfig);
        Assert.False(form.FIsConfigChanged);
        Assert.False(form.btnSave.Enabled);
    }

    /// <summary>
    /// 分支 2：守卫非 nil → 写回 + SetConfigChanged() 生效
    /// （IsChanged = True、FIsConfigChanged = True、btnSave.Enabled = True）。
    /// </summary>
    [Theory]
    [MemberData(nameof(Rows))]
    public void Handler_NonNullGuard_WritesBackAndMarksChanged(CustomMagicHandlerRow row)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        SetControl(form, row, 7);
        Invoke(form, row);

        Assert.True(form.FIsConfigChanged, $"{row.Method} 未置 FIsConfigChanged");
        Assert.True(cfg.IsChanged, $"{row.Method} 未置 Config.IsChanged");
        Assert.True(form.btnSave.Enabled, $"{row.Method} 未启用 btnSave");
    }

    /// <summary>
    /// 分支 3：边界值（0 / -1 / 该字段的最小值）→ 不抛异常，且仍然置 Changed。
    /// 覆盖 <c>cbb*.ItemIndex - 1</c> 在 ItemIndex=0 时得到 -1 的情形。
    /// </summary>
    [Theory]
    [MemberData(nameof(Rows))]
    public void Handler_BoundaryValues_DoNotThrow(CustomMagicHandlerRow row)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        foreach (int value in new[] { 0, -1, int.MinValue })
        {
            form.FIsConfigChanged = false;
            cfg.SetChanged(false);
            SetControl(form, row, value);
            Invoke(form, row);
            Assert.True(form.FIsConfigChanged, $"{row.Method} value={value}");
        }
    }

    /// <summary>
    /// 分支 4（差异断言）：<c>cbb*.ItemIndex - 1</c> 型 RHS 在 ItemIndex = 0 时写入 -1，
    /// 而 <c>cbb*.ItemIndex</c> 型直接写 0；<c>chk*.Checked</c> 型写 false。
    /// </summary>
    [Fact]
    public void RhsKinds_BehaveDifferently()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        // 文件下拉：ItemIndex - 1
        form.cbbClientIconFile.ItemIndex = 0;
        form.CbbClientIconFileChange();
        Assert.Equal(-1, cfg.ClientConfigs[0].Value.Icon_File);

        // 下拉写枚举：ItemIndex
        form.cbbClientFlyDrawMode.ItemIndex = 1;
        form.CbbClientFlyDrawModeChange();
        Assert.Equal(GXX.Core.Protocol.TCustomDrawMode.mdmNormal, cfg.ClientConfigs[0].Value.Fly_DrawMode);

        // 勾选写 Boolean 字段（Delphi Boolean → C# byte）
        form.chkClientFlyCalcDir.Checked = false;
        form.ChkClientFlyCalcDirClick();
        Assert.Equal(0, cfg.ClientConfigs[0].Value.Fly_CalcDir);
        form.chkClientFlyCalcDir.Checked = true;
        form.ChkClientFlyCalcDirClick();
        Assert.Equal(1, cfg.ClientConfigs[0].Value.Fly_CalcDir);
    }

    private static void Invoke(TFrmCustomMagic form, CustomMagicHandlerRow row)
    {
        var mi = typeof(TFrmCustomMagic).GetMethod(row.Method, BindingFlags.Public | BindingFlags.Instance,
            null, Type.EmptyTypes, null);
        Assert.NotNull(mi);
        mi!.Invoke(form, null);
    }

    private static void SetControl(TFrmCustomMagic form, CustomMagicHandlerRow row, int value)
    {
        var fi = typeof(TFrmCustomMagic).GetField(row.Control, BindingFlags.Public | BindingFlags.Instance)!;
        object control = fi.GetValue(form)!;

        switch (row.ControlKind)
        {
            case "spin":
                ((TSpinEditExSeam)control).Value = value;
                break;
            case "check":
                ((TCheckBoxSeam)control).Checked = value > 0;
                break;
            case "edit":
                ((TEditSeam)control).Text = "T" + value;
                break;
            case "combo":
                ((TComboBoxSeam)control).ItemIndex = value;
                break;
            default:
                throw new InvalidOperationException(row.ControlKind);
        }

        // AttackPowerRates[cbbAttackPowerLevel.ItemIndex] 这类索引写入需要合法下标
        if (row.TargetPath.Contains("AttackPowerRates", StringComparison.Ordinal))
            form.cbbAttackPowerLevel.ItemIndex = 0;
    }
}
