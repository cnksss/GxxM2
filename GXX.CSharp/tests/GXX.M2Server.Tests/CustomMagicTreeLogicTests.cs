// ============================================================================
// uFrmCustomMagic.pas 五棵树 + 主列表的纯逻辑测试
// 源单元：Source\M2Engine\Forms\uFrmCustomMagic.pas
//   :1877-1908 主列表三事件；:4290-4413 属性树（减）；
//   :4429-4532 元素树；:4551-4708 属性树（保护）
// ============================================================================

using System;
using GXX.M2Server.Forms.CustomMagic;
using TMagicAttackDecAttributesType = GXX.M2Server.Engine.TMagicAttackDecAttributesType;
using TMagicProtectAddAttributesType = GXX.M2Server.Engine.TMagicProtectAddAttributesType;
using TItemElementsType = GXX.M2Server.Engine.TItemElementsType;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("CustomMagicFormLane")]
public sealed class CustomMagicTreeLogicTests
{
    // ---- 主列表（原文 :1877-1908）----

    [Fact]
    public void MainTree_NodeDataSizeIs4()
        => Assert.Equal(4, CustomMagicMainTreeLogic.NodeDataSize);

    [Fact]
    public void MainTree_GetText_NullAndValue()
    {
        Assert.Equal("", CustomMagicMainTreeLogic.GetText(null));
        Assert.Equal("", CustomMagicMainTreeLogic.GetText(new TMagicConfigNodeData()));
        Assert.Equal("技能A",
            CustomMagicMainTreeLogic.GetText(new TMagicConfigNodeData { Config = new TCustomMagicConfig("技能A", 1, false) }));
    }

    [Fact]
    public void MainTree_FontColor_IsChangedBeatsHighlight()
    {
        const int fontColor = 0x123456;
        var changed = new TMagicConfigNodeData { Config = new TCustomMagicConfig("a", 1, false) };
        changed.Config!.SetChanged(true);

        // IsChanged → clRed，即使同时选中且聚焦
        Assert.Equal(CustomMagicColors.clRed, CustomMagicMainTreeLogic.ResolveFontColor(changed, true, true, fontColor));

        // 未 changed + 选中 + 聚焦 → clHighlightText
        var clean = new TMagicConfigNodeData { Config = new TCustomMagicConfig("a", 1, false) };
        Assert.Equal(CustomMagicColors.clHighlightText, CustomMagicMainTreeLogic.ResolveFontColor(clean, true, true, fontColor));

        // 未 changed + 选中但未聚焦 → Font.Color
        Assert.Equal(fontColor, CustomMagicMainTreeLogic.ResolveFontColor(clean, true, false, fontColor));
        // 未 changed + 未选中 → Font.Color
        Assert.Equal(fontColor, CustomMagicMainTreeLogic.ResolveFontColor(clean, false, true, fontColor));
        // 数据为 nil → Font.Color（原文不改色）
        Assert.Equal(fontColor, CustomMagicMainTreeLogic.ResolveFontColor(null, true, true, fontColor));
        // Config 为 nil → 原文此处解引用 AV；托管侧照抄（抛 NullReferenceException）
        Assert.Throws<NullReferenceException>(
            () => CustomMagicMainTreeLogic.ResolveFontColor(new TMagicConfigNodeData(), true, true, fontColor));
    }

    // ---- 属性树（减）列文本（原文 :4347-4399）----

    private static TAttackDecAttribData DecData(TMagicAttackDecAttributesType type, bool lowPoint = false,
        bool highPoint = false, bool timePoint = false)
    {
        var d = new TMagicChangeAttributesRecord
        {
            Rate = 11, RateAdd = 22, LowValue = 33, LowValueAdd = 55,
            HighValue = 66, HighValueAdd = 88, Time = 99, TimeAdd = 101,
            HintText = "提示",
            LowValueIsPoint = lowPoint, HighValueIsPoint = highPoint, TimeAddIsPoint = timePoint,
        };
        return new TAttackDecAttribData { AttribType = type, Data = d };
    }

    [Fact]
    public void DecAttrib_GetText_AllColumns_ForDaAC()
    {
        var data = DecData(TMagicAttackDecAttributesType.daAC);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.MagicAttackDecAttributesTypeNames[0],
            DecAttribTreeLogic.GetAttackDecAttribText(0, data, data.Data!));
        Assert.Equal("11", DecAttribTreeLogic.GetAttackDecAttribText(1, data, data.Data!));
        Assert.Equal("22", DecAttribTreeLogic.GetAttackDecAttribText(2, data, data.Data!));
        Assert.Equal("33", DecAttribTreeLogic.GetAttackDecAttribText(3, data, data.Data!));
        Assert.Equal("%", DecAttribTreeLogic.GetAttackDecAttribText(4, data, data.Data!));
        Assert.Equal("55", DecAttribTreeLogic.GetAttackDecAttribText(5, data, data.Data!));
        Assert.Equal("66", DecAttribTreeLogic.GetAttackDecAttribText(6, data, data.Data!));
        Assert.Equal("%", DecAttribTreeLogic.GetAttackDecAttribText(7, data, data.Data!));
        Assert.Equal("88", DecAttribTreeLogic.GetAttackDecAttribText(8, data, data.Data!));
        Assert.Equal("99", DecAttribTreeLogic.GetAttackDecAttribText(9, data, data.Data!));
        Assert.Equal("101", DecAttribTreeLogic.GetAttackDecAttribText(10, data, data.Data!));
        Assert.Equal("%", DecAttribTreeLogic.GetAttackDecAttribText(11, data, data.Data!));
        Assert.Equal(" ", DecAttribTreeLogic.GetAttackDecAttribText(12, data, data.Data!));
        Assert.Equal("提示", DecAttribTreeLogic.GetAttackDecAttribText(13, data, data.Data!));
        Assert.Equal("", DecAttribTreeLogic.GetAttackDecAttribText(14, data, data.Data!));
    }

    /// <summary>原文 :4355-4372：<c>AttribType &gt;= daHitPoint</c> 时列 3/4/5 输出 '-'。</summary>
    [Fact]
    public void DecAttrib_GetText_HitPointOnwards_DashOn345()
    {
        var data = DecData(TMagicAttackDecAttributesType.daHitPoint);
        Assert.Equal("-", DecAttribTreeLogic.GetAttackDecAttribText(3, data, data.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetAttackDecAttribText(4, data, data.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetAttackDecAttribText(5, data, data.Data!));
        // 列 6/7/8 不看属性类型
        Assert.Equal("66", DecAttribTreeLogic.GetAttackDecAttribText(6, data, data.Data!));
        Assert.Equal("88", DecAttribTreeLogic.GetAttackDecAttribText(8, data, data.Data!));

        // 最后一个枚举值同样 >= daHitPoint
        var last = DecData(TMagicAttackDecAttributesType.daAntiPoison);
        Assert.Equal("-", DecAttribTreeLogic.GetAttackDecAttribText(3, last, last.Data!));

        // 恰好 daHitPoint - 1 时不是 '-'
        var before = DecData(TMagicAttackDecAttributesType.daSC);
        Assert.Equal("33", DecAttribTreeLogic.GetAttackDecAttribText(3, before, before.Data!));
    }

    [Fact]
    public void DecAttrib_GetText_BooleanUnitNames()
    {
        var data = DecData(TMagicAttackDecAttributesType.daAC, lowPoint: true, highPoint: true, timePoint: true);
        Assert.Equal("点", DecAttribTreeLogic.GetAttackDecAttribText(4, data, data.Data!));
        Assert.Equal("点", DecAttribTreeLogic.GetAttackDecAttribText(7, data, data.Data!));
        Assert.Equal("秒", DecAttribTreeLogic.GetAttackDecAttribText(11, data, data.Data!));
    }

    // ---- 属性树（保护）列文本（原文 :4608-4690）----

    private static TProtectAddAttribData ProtData(TMagicProtectAddAttributesType type)
    {
        var d = new TMagicChangeAttributesRecord
        {
            Rate = 1, RateAdd = 2, LowValue = 3, LowValueAdd = 5,
            HighValue = 6, HighValueAdd = 8, Time = 9, TimeAdd = 10, HintText = "h",
        };
        return new TProtectAddAttribData { AttribType = type, Data = d };
    }

    [Fact]
    public void Protected_GetText_AllColumns_ForAaAC()
    {
        var data = ProtData(TMagicProtectAddAttributesType.aaAC);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.MagicProtectAddAttributesTypeNames[0],
            DecAttribTreeLogic.GetProtectedAddAttribText(0, data, data.Data!));
        for (int col = 1; col <= 11; col++)
            Assert.NotEqual("-", DecAttribTreeLogic.GetProtectedAddAttribText(col, data, data.Data!));
        Assert.Equal(" ", DecAttribTreeLogic.GetProtectedAddAttribText(12, data, data.Data!));
        Assert.Equal("h", DecAttribTreeLogic.GetProtectedAddAttribText(13, data, data.Data!));
        Assert.Equal("", DecAttribTreeLogic.GetProtectedAddAttribText(14, data, data.Data!));
    }

    /// <summary>
    /// 差异断言：保护属性树的三段禁显条件各不相同 ——
    /// 列 3/4/5 用 <c>&gt;= aaHitPoint</c>；列 6/7/8 只对 <c>aaHide</c>；列 9/10/11 只对 <c>aaHP/aaMP</c>。
    /// 特别注意 aaNGDamage / aaNGDefense 落在 <c>&gt;= aaHitPoint</c> 里，因此列 3/4/5 是 '-'，
    /// 但列 6/7/8/9/10/11 仍然有值。
    /// </summary>
    [Fact]
    public void Protected_GetText_ThreeDistinctDashRules()
    {
        var ng = ProtData(TMagicProtectAddAttributesType.aaNGDamage);
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(3, ng, ng.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(4, ng, ng.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(5, ng, ng.Data!));
        Assert.Equal("6", DecAttribTreeLogic.GetProtectedAddAttribText(6, ng, ng.Data!));
        Assert.Equal("9", DecAttribTreeLogic.GetProtectedAddAttribText(9, ng, ng.Data!));
        Assert.Equal("10", DecAttribTreeLogic.GetProtectedAddAttribText(10, ng, ng.Data!));
        Assert.Equal("%", DecAttribTreeLogic.GetProtectedAddAttribText(11, ng, ng.Data!));   // 11 是时间单位列

        var hide = ProtData(TMagicProtectAddAttributesType.aaHide);
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(3, hide, hide.Data!));   // aaHide 亦 >= aaHitPoint
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(6, hide, hide.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(7, hide, hide.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(8, hide, hide.Data!));
        Assert.Equal("9", DecAttribTreeLogic.GetProtectedAddAttribText(9, hide, hide.Data!));

        var hp = ProtData(TMagicProtectAddAttributesType.aaHP);
        Assert.Equal("6", DecAttribTreeLogic.GetProtectedAddAttribText(6, hp, hp.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(9, hp, hp.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(10, hp, hp.Data!));
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(11, hp, hp.Data!));

        var mp = ProtData(TMagicProtectAddAttributesType.aaMP);
        Assert.Equal("-", DecAttribTreeLogic.GetProtectedAddAttribText(9, mp, mp.Data!));

        // aaMaxHP / aaMaxMP 不在 [aaHP, aaMP] → 列 9/10/11 有值
        var maxHp = ProtData(TMagicProtectAddAttributesType.aaMaxHP);
        Assert.Equal("9", DecAttribTreeLogic.GetProtectedAddAttribText(9, maxHp, maxHp.Data!));
    }

    // ---- 元素树列文本（原文 :4489-4520）----

    [Fact]
    public void Element_GetText_AllColumns_NoDashLogic()
    {
        var d = new TMagicAttackChangeElementRecord
        {
            Rate = 1, RateAdd = 2, Value = 3, ValueAdd = 5, Time = 6, TimeAdd = 7, HintText = "h",
        };
        var data = new TMagicElementData { ElementType = TItemElementsType.ietBlastHit, Data = d };

        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.ItemElementsTypeNames[0],
            ElementTreeLogic.GetText(0, data, d));
        Assert.Equal("1", ElementTreeLogic.GetText(1, data, d));
        Assert.Equal("2", ElementTreeLogic.GetText(2, data, d));
        Assert.Equal("3", ElementTreeLogic.GetText(3, data, d));
        Assert.Equal("%", ElementTreeLogic.GetText(4, data, d));
        Assert.Equal("5", ElementTreeLogic.GetText(5, data, d));
        Assert.Equal("6", ElementTreeLogic.GetText(6, data, d));
        Assert.Equal("7", ElementTreeLogic.GetText(7, data, d));
        Assert.Equal("%", ElementTreeLogic.GetText(8, data, d));
        Assert.Equal(" ", ElementTreeLogic.GetText(9, data, d));
        Assert.Equal("h", ElementTreeLogic.GetText(10, data, d));
        Assert.Equal("", ElementTreeLogic.GetText(11, data, d));

        d.ValueIsPoint = true;
        d.TimeAddIsPoint = true;
        Assert.Equal("点", ElementTreeLogic.GetText(4, data, d));
        Assert.Equal("秒", ElementTreeLogic.GetText(8, data, d));

        // 元素树任何 ElementType 都不会出现 '-'（差异断言）
        for (int i = 0; i < 25; i++)
        {
            data.ElementType = (TItemElementsType)i;
            for (int col = 3; col <= 8; col++)
                Assert.NotEqual("-", ElementTreeLogic.GetText(col, data, d));
        }
    }

    // ---- 勾选（原文 :4290-4305 / :4429-4444 / :4551-4566）----

    [Theory]
    [InlineData(TCheckState.csCheckedNormal, true)]
    [InlineData(TCheckState.csUnCheckedNormal, false)]
    [InlineData(TCheckState.csMixedNormal, false)]
    public void ApplyChecked_MapsCheckState(TCheckState state, bool expected)
    {
        bool isChecked = !expected;
        Assert.True(DecAttribTreeLogic.ApplyChecked(false, state, ref isChecked));
        Assert.Equal(expected, isChecked);

        isChecked = !expected;
        Assert.True(ElementTreeLogic.ApplyChecked(false, state, ref isChecked));
        Assert.Equal(expected, isChecked);
    }

    [Fact]
    public void ApplyChecked_NullData_DoesNothing()
    {
        bool isChecked = true;
        Assert.False(DecAttribTreeLogic.ApplyChecked(true, TCheckState.csUnCheckedNormal, ref isChecked));
        Assert.True(isChecked);   // 原文 :4293 if DecAttribData <> nil 才进

        Assert.False(ElementTreeLogic.ApplyChecked(true, TCheckState.csUnCheckedNormal, ref isChecked));
        Assert.True(isChecked);
    }

    // ---- 节点点击决策（原文 :4306-4325 / :4445-4467 / :4567-4586）----

    [Theory]
    // HitNode nil → Exit（即使 HitColumn 是提示列）
    [InlineData(true, 12, TVtNodeClickAction.None)]
    [InlineData(true, 0, TVtNodeClickAction.None)]
    // 提示列
    [InlineData(false, 12, TVtNodeClickAction.ToggleShowHint)]
    // 其它正列 → 发起编辑
    [InlineData(false, 1, TVtNodeClickAction.PostStartEditing)]
    [InlineData(false, 13, TVtNodeClickAction.PostStartEditing)]
    // 列 0 与负列 → 什么也不做
    [InlineData(false, 0, TVtNodeClickAction.None)]
    [InlineData(false, -1, TVtNodeClickAction.None)]
    public void DecAttribTree_ResolveNodeClick(bool hitNull, int column, TVtNodeClickAction expected)
        => Assert.Equal(expected, DecAttribTreeLogic.ResolveNodeClick(hitNull, column));

    [Theory]
    [InlineData(true, 9, TVtNodeClickAction.None)]
    [InlineData(false, 9, TVtNodeClickAction.ToggleShowHint)]
    [InlineData(false, 10, TVtNodeClickAction.PostStartEditing)]
    [InlineData(false, 0, TVtNodeClickAction.None)]
    public void ElementTree_ResolveNodeClick(bool hitNull, int column, TVtNodeClickAction expected)
        => Assert.Equal(expected, ElementTreeLogic.ResolveNodeClick(hitNull, column));

    /// <summary>差异断言：属性树提示列是 12，元素树是 9。</summary>
    [Fact]
    public void HintColumns_Differ()
    {
        Assert.Equal(12, DecAttribTreeLogic.HintColumn);
        Assert.Equal(9, ElementTreeLogic.HintColumn);
        Assert.Equal(TVtNodeClickAction.PostStartEditing, DecAttribTreeLogic.ResolveNodeClick(false, 9));
        Assert.Equal(TVtNodeClickAction.ToggleShowHint, ElementTreeLogic.ResolveNodeClick(false, 9));
    }

    [Fact]
    public void ElementTree_StartEditingMessage_DependsOnSender()
    {
        Assert.Equal(CustomMagicWm.WM_STARTEDITING_DEC_ELEMENT, ElementTreeLogic.StartEditingMessage(true));
        Assert.Equal(CustomMagicWm.WM_STARTEDITING_INC_ELEMENT, ElementTreeLogic.StartEditingMessage(false));
        Assert.Equal(0x0400 + 301, CustomMagicWm.WM_STARTEDITING_DEC_ELEMENT);
        Assert.Equal(0x0400 + 303, CustomMagicWm.WM_STARTEDITING_INC_ELEMENT);
        Assert.Equal(0x0400 + 300, CustomMagicWm.WM_STARTEDITING_DEC_ATTRIB);
        Assert.Equal(0x0400 + 302, CustomMagicWm.WM_STARTEDITING_INC_ATTRIB);
    }

    // ---- 提示图标布局（原文 :4326-4346 / :4468-4488 / :4587-4607）----

    [Fact]
    public void HintIconLayout_OnlyOnHintColumn()
    {
        var rect = new TRectSeam { Left = 100, Top = 50, Right = 140, Bottom = 70 };

        Assert.False(DecAttribTreeLogic.TryGetHintIconLayout(11, rect, 13, 13, true, out _, out _, out _));
        Assert.True(DecAttribTreeLogic.TryGetHintIconLayout(12, rect, 13, 13, false, out int x0, out int y0, out int idx0));
        Assert.Equal(0, idx0);
        Assert.Equal(100 + (140 - 100 - 13) / 2, x0);
        Assert.Equal(50 + (70 - 50 - 13) / 2, y0);

        Assert.True(DecAttribTreeLogic.TryGetHintIconLayout(12, rect, 13, 13, true, out _, out _, out int idx1));
        Assert.Equal(1, idx1);

        Assert.False(ElementTreeLogic.TryGetHintIconLayout(8, rect, 13, 13, true, out _, out _, out _));
        Assert.True(ElementTreeLogic.TryGetHintIconLayout(9, rect, 13, 13, true, out _, out _, out int idx2));
        Assert.Equal(1, idx2);
    }

    /// <summary>Delphi <c>div</c> 与 C# <c>/</c> 一样向零截断；单元格比图标窄时可算出负坐标。</summary>
    [Fact]
    public void HintIconLayout_TruncatesTowardZero()
    {
        // 宽 5、图标 13 → (5-13)/2 = -4
        var narrow = new TRectSeam { Left = 0, Right = 5, Bottom = 5 };
        Assert.True(DecAttribTreeLogic.TryGetHintIconLayout(12, narrow, 13, 13, true, out int x, out int y, out _));
        Assert.Equal(-4, x);
        Assert.Equal(-4, y);

        // 宽 4、图标 13 → (4-13)/2 = -4（Delphi div 向零截断，-9/2 = -4）
        var narrow2 = new TRectSeam { Left = 0, Right = 4, Bottom = 0 };
        Assert.True(DecAttribTreeLogic.TryGetHintIconLayout(12, narrow2, 13, 13, false, out int x2, out _, out _));
        Assert.Equal(-4, x2);
    }

    // ---- Editing 许可矩阵（原文 :4400-4413 / :4521-4526 / :4691-4708）----

    [Fact]
    public void DecAttrib_Editing_NodeAndColumnGuards()
    {
        Assert.False(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(true, 1, TMagicAttackDecAttributesType.daAC));
        Assert.False(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, 0, TMagicAttackDecAttributesType.daAC));
        Assert.False(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, 12, TMagicAttackDecAttributesType.daAC));
        Assert.False(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, -3, TMagicAttackDecAttributesType.daAC));
        Assert.True(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, 1, TMagicAttackDecAttributesType.daAC));
        Assert.True(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, 13, TMagicAttackDecAttributesType.daAC));
    }

    [Fact]
    public void DecAttrib_Editing_HitPointLocksColumns345()
    {
        var ac = TMagicAttackDecAttributesType.daAC;
        var hp = TMagicAttackDecAttributesType.daHitPoint;

        foreach (int column in new[] { 3, 4, 5 })
        {
            Assert.True(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, column, ac));
            Assert.False(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, column, hp));
            Assert.False(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, column, TMagicAttackDecAttributesType.daAntiPoison));
        }
        // 列 6/7/8/9/10/11/13 不受属性类型影响
        foreach (int column in new[] { 1, 2, 6, 7, 8, 9, 10, 11, 13 })
        {
            Assert.True(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, column, hp), $"column {column}");
            Assert.True(DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, column, ac), $"column {column}");
        }
    }

    [Fact]
    public void Protected_Editing_ThreeRules()
    {
        var ac = TMagicProtectAddAttributesType.aaAC;
        var hp = TMagicProtectAddAttributesType.aaHitPoint;
        var hide = TMagicProtectAddAttributesType.aaHide;
        var hpUnit = TMagicProtectAddAttributesType.aaHP;
        var mpUnit = TMagicProtectAddAttributesType.aaMP;

        Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(true, 1, ac));
        Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, 0, ac));
        Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, 12, ac));

        foreach (int column in new[] { 3, 4, 5 })
        {
            Assert.True(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, ac));
            Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, hp));
        }
        foreach (int column in new[] { 6, 7, 8 })
        {
            Assert.True(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, ac));
            Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, hide));
            // aaHP 只有列 9/10/11 被禁，列 6/7/8 仍可编辑
            Assert.True(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, hpUnit));
        }
        foreach (int column in new[] { 9, 10, 11 })
        {
            Assert.True(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, ac));
            Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, hpUnit));
            Assert.False(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, mpUnit));
            Assert.True(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, hide));
        }
        // 列 13 永远可编辑
        foreach (var t in new[] { ac, hp, hide, hpUnit, mpUnit })
            Assert.True(DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, 13, t));
    }

    [Fact]
    public void Element_Editing_NoAttributeTypeRules()
    {
        Assert.False(ElementTreeLogic.IsEditingAllowed(true, 1));
        Assert.False(ElementTreeLogic.IsEditingAllowed(false, 0));
        Assert.False(ElementTreeLogic.IsEditingAllowed(false, 9));
        Assert.True(ElementTreeLogic.IsEditingAllowed(false, 1));
        Assert.True(ElementTreeLogic.IsEditingAllowed(false, 10));
        Assert.True(ElementTreeLogic.IsEditingAllowed(false, 12));   // 元素树的提示列是 9，故 12 可编辑
        Assert.False(ElementTreeLogic.IsEditingAllowed(false, -1));
    }
}
