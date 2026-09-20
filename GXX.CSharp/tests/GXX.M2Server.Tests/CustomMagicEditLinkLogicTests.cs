// ============================================================================
// uFrmCustomMagic.pas 纯逻辑（VirtualTrees 就地编辑器）测试
// 源单元：Source\M2Engine\Forms\uFrmCustomMagic.pas
//   TDecAttribPropertyEditLink :888-907 / :932-1285
//   TElementPropertyEditLink   :909-928 / :1286-1604
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Forms.CustomMagic;
using TCheckVarType = GXX.M2Server.Engine.TCheckVarType;
using TMagicAttackDecAttributesType = GXX.M2Server.Engine.TMagicAttackDecAttributesType;
using TMagicProtectAddAttributesType = GXX.M2Server.Engine.TMagicProtectAddAttributesType;
using TItemElementsType = GXX.M2Server.Engine.TItemElementsType;
using TBreakDefenseType = GXX.M2Server.Engine.TBreakDefenseType;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("CustomMagicFormLane")]
public sealed class CustomMagicEditLinkLogicTests
{
    private static TAttackDecAttribData MakeDecAttrib()
        => new() { AttribType = TMagicAttackDecAttributesType.daAC, Data = new TMagicChangeAttributesRecord() };

    private static TMagicElementData MakeElement()
        => new() { ElementType = TItemElementsType.ietBlastHit, Data = new TMagicAttackChangeElementRecord() };

    // ---- PrepareEditSpec：列 → 编辑器类型（原文 :1156-1263 / :1486-1587）----

    [Theory]
    [InlineData(1, TVtEditKind.SpinEdit)]
    [InlineData(2, TVtEditKind.SpinEdit)]
    [InlineData(3, TVtEditKind.SpinEdit)]
    [InlineData(4, TVtEditKind.ComboBox)]
    [InlineData(5, TVtEditKind.SpinEdit)]
    [InlineData(6, TVtEditKind.SpinEdit)]
    [InlineData(7, TVtEditKind.ComboBox)]
    [InlineData(8, TVtEditKind.SpinEdit)]
    [InlineData(9, TVtEditKind.SpinEdit)]
    [InlineData(10, TVtEditKind.SpinEdit)]
    [InlineData(11, TVtEditKind.ComboBox)]
    [InlineData(12, TVtEditKind.None)]     // 提示列：原文不建编辑器（Editing 也禁）
    [InlineData(13, TVtEditKind.Edit)]
    [InlineData(14, TVtEditKind.None)]     // 越界列：原文 Result 仍 True 但不建编辑器（原文缺陷#1）
    [InlineData(0, TVtEditKind.None)]
    public void DecAttrib_EditKindForColumn(int column, TVtEditKind expected)
        => Assert.Equal(expected, DecAttribPropertyEditLinkLogic.EditKindForColumn(column));

    [Theory]
    [InlineData(1, TVtEditKind.SpinEdit)]
    [InlineData(2, TVtEditKind.SpinEdit)]
    [InlineData(3, TVtEditKind.SpinEdit)]
    [InlineData(4, TVtEditKind.ComboBox)]
    [InlineData(5, TVtEditKind.SpinEdit)]
    [InlineData(6, TVtEditKind.SpinEdit)]
    [InlineData(7, TVtEditKind.SpinEdit)]
    [InlineData(8, TVtEditKind.ComboBox)]
    [InlineData(9, TVtEditKind.None)]      // 元素树的提示列是 9
    [InlineData(10, TVtEditKind.Edit)]
    [InlineData(12, TVtEditKind.None)]
    [InlineData(0, TVtEditKind.None)]
    public void Element_EditKindForColumn(int column, TVtEditKind expected)
        => Assert.Equal(expected, ElementPropertyEditLinkLogic.EditKindForColumn(column));

    // ---- 差异断言：两个 EditLink 的列集合确实不同（不能合并实现）----

    [Fact]
    public void EditLinks_ColumnSets_Differ()
    {
        Assert.Equal(new[] { 1, 2, 3, 5, 6, 8, 9, 10 }, DecAttribPropertyEditLinkLogic.SpinColumns);
        Assert.Equal(new[] { 1, 2, 3, 5, 6, 7 }, ElementPropertyEditLinkLogic.SpinColumns);
        Assert.Equal(new[] { 4, 7, 11 }, DecAttribPropertyEditLinkLogic.ComboColumns);
        Assert.Equal(new[] { 4, 8 }, ElementPropertyEditLinkLogic.ComboColumns);
        Assert.Equal(new[] { 13 }, DecAttribPropertyEditLinkLogic.EditColumns);
        Assert.Equal(new[] { 10 }, ElementPropertyEditLinkLogic.EditColumns);

        // 列 7：减属性是下拉（单位：% / 秒），元素是微调（TimeAdd 数值）
        Assert.Equal(TVtEditKind.ComboBox, DecAttribPropertyEditLinkLogic.EditKindForColumn(7));
        Assert.Equal(TVtEditKind.SpinEdit, ElementPropertyEditLinkLogic.EditKindForColumn(7));
        // 列 8：减属性是微调，元素是下拉
        Assert.Equal(TVtEditKind.SpinEdit, DecAttribPropertyEditLinkLogic.EditKindForColumn(8));
        Assert.Equal(TVtEditKind.ComboBox, ElementPropertyEditLinkLogic.EditKindForColumn(8));
    }

    // ---- PrepareEditSpec：初值 / 范围 / 下拉项 ----

    [Fact]
    public void DecAttrib_PrepareEdit_SpinValuesPerColumn()
    {
        var data = MakeDecAttrib();
        data.Data!.Rate = 11;
        data.Data.RateAdd = 22;
        data.Data.LowValue = 33;
        data.Data.LowValueAdd = 55;
        data.Data.HighValue = 66;
        data.Data.HighValueAdd = 88;
        data.Data.Time = 99;
        data.Data.TimeAdd = 1010;

        Assert.Equal(11, DecAttribPropertyEditLinkLogic.PrepareEditSpec(1, data).Value);
        Assert.Equal(22, DecAttribPropertyEditLinkLogic.PrepareEditSpec(2, data).Value);
        Assert.Equal(33, DecAttribPropertyEditLinkLogic.PrepareEditSpec(3, data).Value);
        Assert.Equal(55, DecAttribPropertyEditLinkLogic.PrepareEditSpec(5, data).Value);
        Assert.Equal(66, DecAttribPropertyEditLinkLogic.PrepareEditSpec(6, data).Value);
        Assert.Equal(88, DecAttribPropertyEditLinkLogic.PrepareEditSpec(8, data).Value);
        Assert.Equal(99, DecAttribPropertyEditLinkLogic.PrepareEditSpec(9, data).Value);
        Assert.Equal(1010, DecAttribPropertyEditLinkLogic.PrepareEditSpec(10, data).Value);
    }

    [Fact]
    public void DecAttrib_PrepareEdit_OnlyColumn1HasMinMax()
    {
        var data = MakeDecAttrib();
        foreach (int column in DecAttribPropertyEditLinkLogic.SpinColumns)
        {
            var spec = DecAttribPropertyEditLinkLogic.PrepareEditSpec(column, data);
            if (column == 1)
            {
                Assert.True(spec.HasMinMax);
                Assert.Equal(0, spec.MinValue);
                Assert.Equal(100, spec.MaxValue);
            }
            else
            {
                Assert.False(spec.HasMinMax);   // 原文只有 FColumn in [1] 才设 Min/Max
            }
            Assert.True(spec.VisibleInitFalse); // Visible := False
            Assert.True(spec.HooksKeyEvents);
        }
    }

    [Fact]
    public void DecAttrib_PrepareEdit_ComboItemsAndIndex()
    {
        var data = MakeDecAttrib();

        var c4 = DecAttribPropertyEditLinkLogic.PrepareEditSpec(4, data);
        Assert.True(c4.DropDownList);
        Assert.Equal(new[] { "%", "点" }, c4.Items);
        Assert.Equal(0, c4.ItemIndex);                       // LowValueIsPoint = false

        data.Data!.LowValueIsPoint = true;
        Assert.Equal(1, DecAttribPropertyEditLinkLogic.PrepareEditSpec(4, data).ItemIndex);

        data.Data.HighValueIsPoint = true;
        Assert.Equal(1, DecAttribPropertyEditLinkLogic.PrepareEditSpec(7, data).ItemIndex);

        var c11 = DecAttribPropertyEditLinkLogic.PrepareEditSpec(11, data);
        Assert.Equal(new[] { "%", "秒" }, c11.Items);
        Assert.Equal(0, c11.ItemIndex);
        data.Data.TimeAddIsPoint = true;
        Assert.Equal(1, DecAttribPropertyEditLinkLogic.PrepareEditSpec(11, data).ItemIndex);
    }

    [Fact]
    public void DecAttrib_PrepareEdit_EditColumn13UsesHintText()
    {
        var data = MakeDecAttrib();
        data.Data!.HintText = "提示文本";
        var spec = DecAttribPropertyEditLinkLogic.PrepareEditSpec(13, data);
        Assert.Equal(TVtEditKind.Edit, spec.Kind);
        Assert.Equal("提示文本", spec.Text);
        Assert.Equal("", spec.Items.Count == 0 ? "" : "非空");
    }

    [Fact]
    public void PrepareEdit_NullData_Throws()
    {
        var data = MakeDecAttrib();
        data.Data = null;
        Assert.Throws<InvalidOperationException>(() => DecAttribPropertyEditLinkLogic.PrepareEditSpec(1, data));

        var elem = MakeElement();
        elem.Data = null;
        Assert.Throws<InvalidOperationException>(() => ElementPropertyEditLinkLogic.PrepareEditSpec(1, elem));
    }

    [Fact]
    public void Element_PrepareEdit_SpinValuesPerColumn()
    {
        var data = MakeElement();
        data.Data!.Rate = 1;
        data.Data.RateAdd = 2;
        data.Data.Value = 3;
        data.Data.ValueAdd = 5;
        data.Data.Time = 6;
        data.Data.TimeAdd = 7;

        Assert.Equal(1, ElementPropertyEditLinkLogic.PrepareEditSpec(1, data).Value);
        Assert.Equal(2, ElementPropertyEditLinkLogic.PrepareEditSpec(2, data).Value);
        Assert.Equal(3, ElementPropertyEditLinkLogic.PrepareEditSpec(3, data).Value);
        Assert.Equal(5, ElementPropertyEditLinkLogic.PrepareEditSpec(5, data).Value);
        Assert.Equal(6, ElementPropertyEditLinkLogic.PrepareEditSpec(6, data).Value);
        Assert.Equal(7, ElementPropertyEditLinkLogic.PrepareEditSpec(7, data).Value);
        Assert.True(ElementPropertyEditLinkLogic.PrepareEditSpec(1, data).HasMinMax);
        Assert.False(ElementPropertyEditLinkLogic.PrepareEditSpec(2, data).HasMinMax);
    }

    [Fact]
    public void Element_PrepareEdit_ComboAndText()
    {
        var data = MakeElement();
        data.Data!.ValueIsPoint = true;
        Assert.Equal(1, ElementPropertyEditLinkLogic.PrepareEditSpec(4, data).ItemIndex);
        Assert.Equal(new[] { "%", "点" }, ElementPropertyEditLinkLogic.PrepareEditSpec(4, data).Items);

        data.Data.TimeAddIsPoint = true;
        Assert.Equal(1, ElementPropertyEditLinkLogic.PrepareEditSpec(8, data).ItemIndex);
        Assert.Equal(new[] { "%", "秒" }, ElementPropertyEditLinkLogic.PrepareEditSpec(8, data).Items);

        data.Data.HintText = "hh";
        Assert.Equal("hh", ElementPropertyEditLinkLogic.PrepareEditSpec(10, data).Text);
    }

    // ---- ApplyEditorResult：写回 + IsChanged（原文 :1007-1145 / :1361-1475）----

    [Fact]
    public void DecAttrib_ApplyEditorResult_AllSpinColumns()
    {
        var data = MakeDecAttrib();
        var editor = new TVtEditorResult { Value = 42 };

        foreach (int column in DecAttribPropertyEditLinkLogic.SpinColumns)
        {
            data.Data = new TMagicChangeAttributesRecord();
            Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(column, data, editor), $"column {column}");
        }

        data.Data = new TMagicChangeAttributesRecord();
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(1, data, new TVtEditorResult { Value = 0 }));
    }

    [Fact]
    public void DecAttrib_ApplyEditorResult_NoChange_ReturnsFalse()
    {
        var data = MakeDecAttrib();
        data.Data!.Rate = 5;
        data.Data.LowValueIsPoint = false;
        data.Data.HintText = "abc";

        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(1, data, new TVtEditorResult { Value = 5 }));
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(4, data, new TVtEditorResult { ItemIndex = 0 }));
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(13, data, new TVtEditorResult { Text = "ABC" }));
    }

    [Fact]
    public void DecAttrib_ApplyEditorResult_ComboIndexOnlyOneMeansTrue()
    {
        var data = MakeDecAttrib();
        data.Data!.LowValueIsPoint = false;

        // ItemIndex = 1 → True
        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(4, data, new TVtEditorResult { ItemIndex = 1 }));
        Assert.True(data.Data.LowValueIsPoint);

        // ItemIndex = 0 → False（原文 TempValue = 1 才 True）
        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(4, data, new TVtEditorResult { ItemIndex = 0 }));
        Assert.False(data.Data.LowValueIsPoint);

        // ItemIndex = -1（未选中）→ 与 Integer(True)=1 不等 → 写 False 且 IsChanged
        data.Data.LowValueIsPoint = true;
        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(4, data, new TVtEditorResult { ItemIndex = -1 }));
        Assert.False(data.Data.LowValueIsPoint);

        // ItemIndex = 2（越界下拉值）→ 不等于 1 → False
        data.Data.LowValueIsPoint = true;
        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(4, data, new TVtEditorResult { ItemIndex = 2 }));
        Assert.False(data.Data.LowValueIsPoint);
    }

    [Fact]
    public void DecAttrib_ApplyEditorResult_HintTextIsCaseInsensitive()
    {
        var data = MakeDecAttrib();
        data.Data!.HintText = "AbC";
        // 原文 SameText → 大小写不敏感，故不算改动
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(13, data, new TVtEditorResult { Text = "abc" }));
        Assert.Equal("AbC", data.Data.HintText);

        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(13, data, new TVtEditorResult { Text = "abcd" }));
        Assert.Equal("abcd", data.Data.HintText);

        // 空串：原文 HintText 为 ''，S 为 '' → SameText 成立 → 不写
        data.Data.HintText = "";
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(13, data, new TVtEditorResult { Text = "" }));
    }

    [Fact]
    public void DecAttrib_ApplyEditorResult_UnhandledColumn_NoChange()
    {
        var data = MakeDecAttrib();
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(0, data, new TVtEditorResult { Value = 1 }));
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(12, data, new TVtEditorResult { Value = 1 }));
        Assert.False(DecAttribPropertyEditLinkLogic.ApplyEditorResult(99, data, new TVtEditorResult { Value = 1 }));
    }

    [Fact]
    public void Element_ApplyEditorResult_AllBranches()
    {
        var data = MakeElement();
        var editor = new TVtEditorResult { Value = 7 };
        foreach (int column in ElementPropertyEditLinkLogic.SpinColumns)
        {
            data.Data = new TMagicAttackChangeElementRecord();
            Assert.True(ElementPropertyEditLinkLogic.ApplyEditorResult(column, data, editor), $"column {column}");
        }

        data.Data = new TMagicAttackChangeElementRecord();
        Assert.True(ElementPropertyEditLinkLogic.ApplyEditorResult(4, data, new TVtEditorResult { ItemIndex = 1 }));
        Assert.True(data.Data.ValueIsPoint);

        data.Data = new TMagicAttackChangeElementRecord();
        Assert.True(ElementPropertyEditLinkLogic.ApplyEditorResult(8, data, new TVtEditorResult { ItemIndex = 1 }));
        Assert.True(data.Data.TimeAddIsPoint);

        data.Data = new TMagicAttackChangeElementRecord { HintText = "x" };
        Assert.True(ElementPropertyEditLinkLogic.ApplyEditorResult(10, data, new TVtEditorResult { Text = "y" }));
        Assert.Equal("y", data.Data.HintText);
    }

    [Fact]
    public void Element_ApplyEditorResult_UnhandledColumns()
    {
        var data = MakeElement();
        Assert.False(ElementPropertyEditLinkLogic.ApplyEditorResult(9, data, new TVtEditorResult { Value = 1 }));
        Assert.False(ElementPropertyEditLinkLogic.ApplyEditorResult(11, data, new TVtEditorResult { Value = 1 }));
        Assert.False(ElementPropertyEditLinkLogic.ApplyEditorResult(-1, data, new TVtEditorResult { Value = 1 }));
    }

    /// <summary>
    /// 差异断言：同一"列 7 / 列 8"在两个 EditLink 上写到**不同字段**，
    /// 且元素记录没有 HighValue/HighValueAdd/HighValueIsPoint。
    /// </summary>
    [Fact]
    public void EditLinks_SameColumnDifferentField()
    {
        var dec = MakeDecAttrib();
        var elem = MakeElement();

        // 列 7：减属性写 HighValueIsPoint（布尔），元素写 TimeAdd（整数）
        dec.Data!.HighValueIsPoint = false;
        elem.Data!.TimeAdd = 0;
        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(7, dec, new TVtEditorResult { ItemIndex = 1 }));
        Assert.True(ElementPropertyEditLinkLogic.ApplyEditorResult(7, elem, new TVtEditorResult { Value = 33 }));
        Assert.True(dec.Data.HighValueIsPoint);
        Assert.Equal(0, dec.Data.TimeAdd);
        Assert.Equal(33, elem.Data.TimeAdd);
        Assert.False(elem.Data.ValueIsPoint);

        // 列 8：减属性写 HighValueAdd（整数），元素写 TimeAddIsPoint（布尔）
        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(8, dec, new TVtEditorResult { Value = 88 }));
        Assert.Equal(88, dec.Data.HighValueAdd);
        Assert.True(ElementPropertyEditLinkLogic.ApplyEditorResult(8, elem, new TVtEditorResult { ItemIndex = 1 }));
        Assert.True(elem.Data.TimeAddIsPoint);
    }

    // ---- 键决策（原文 :942-986 / :1296-1340）----

    [Fact]
    public void EditKeyDown_Escape_Swallows()
    {
        Assert.Equal(TVtEditKeyAction.SwallowKey,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.SpinEdit, false, true, TVtKeys.VK_ESCAPE));
        Assert.Equal(TVtEditKeyAction.SwallowKey,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.Edit, true, false, TVtKeys.VK_ESCAPE));
    }

    [Fact]
    public void EditKeyDown_Return_EndsEdit()
    {
        Assert.Equal(TVtEditKeyAction.SwallowKeyThenEndEditNode,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.SpinEdit, false, true, TVtKeys.VK_RETURN));
        Assert.Equal(TVtEditKeyAction.SwallowKeyThenEndEditNode,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.ComboBox, true, false, TVtKeys.VK_RETURN));
    }

    [Fact]
    public void EditKeyDown_UpDown_ComboDroppedOrShift()
    {
        // ComboBox 且下拉已展开 → 不前进（Shift 为空也不行）
        Assert.Equal(TVtEditKeyAction.None,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.ComboBox, true, true, TVtKeys.VK_DOWN));
        // ComboBox 且未展开 + Shift 空 → 转发
        Assert.Equal(TVtEditKeyAction.PostKeyDownThenSwallowKey,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.ComboBox, false, true, TVtKeys.VK_DOWN));
        // ComboBox + Shift 非空 → 不前进
        Assert.Equal(TVtEditKeyAction.None,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.ComboBox, false, false, TVtKeys.VK_UP));
    }

    /// <summary>
    /// 原文如此（:966-967 / :1320-1321）：SpinEditEx 分支把 CanAdvance **无条件覆盖为 True**，
    /// 于是按住 Shift 时 TSpinEditEx 仍然会前进 —— 与 TEdit/未建编辑器时（受 Shift = [] 约束）不同。
    /// </summary>
    [Fact]
    public void EditKeyDown_UpDown_SpinEditOverridesShift()
    {
        Assert.Equal(TVtEditKeyAction.PostKeyDownThenSwallowKey,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.SpinEdit, false, false, TVtKeys.VK_UP));
        Assert.Equal(TVtEditKeyAction.None,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.Edit, false, false, TVtKeys.VK_UP));
        Assert.Equal(TVtEditKeyAction.PostKeyDownThenSwallowKey,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.Edit, false, true, TVtKeys.VK_UP));
        // 未建编辑器（Kind = None）也走"非 ComboBox/非 SpinEditEx"路径 → 受 Shift 约束
        Assert.Equal(TVtEditKeyAction.PostKeyDownThenSwallowKey,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.None, false, true, TVtKeys.VK_UP));
    }

    [Fact]
    public void EditKeyDown_OtherKeys_NoAction()
    {
        Assert.Equal(TVtEditKeyAction.None,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.SpinEdit, false, true, 'A'));
        Assert.Equal(TVtEditKeyAction.None,
            TVtEditLinkKeys.EditKeyDown(TVtEditKind.SpinEdit, false, true, 0));
    }

    [Fact]
    public void EditKeyUp_OnlyEscape()
    {
        Assert.Equal(TVtEditKeyAction.CancelEditNodeThenSwallowKey, TVtEditLinkKeys.EditKeyUp(TVtKeys.VK_ESCAPE));
        Assert.Equal(TVtEditKeyAction.None, TVtEditLinkKeys.EditKeyUp(TVtKeys.VK_RETURN));
        Assert.Equal(TVtEditKeyAction.None, TVtEditLinkKeys.EditKeyUp(0));
    }

    // ---- SetBounds 尺寸（原文 :1274-1282 / :1595-1603）----

    [Fact]
    public void SetBoundsSpec_TakesHeaderColumnRight()
    {
        var tree = new CustomMagicTreeHost();
        tree.SetColumnRight(7, 321);
        var r = new TRectSeam { Left = 10, Right = 999 };
        var result = DecAttribPropertyEditLinkLogic.SetBoundsSpec(7, r, tree);
        Assert.Equal(321, result.Right);
        Assert.Equal(10, result.Left);

        // 未登记的列 → 保持传入的 Right（宿主的兜底语义）
        var r2 = new TRectSeam { Left = 1, Right = 55 };
        Assert.Equal(55, ElementPropertyEditLinkLogic.SetBoundsSpec(3, r2, tree).Right);
    }
}
