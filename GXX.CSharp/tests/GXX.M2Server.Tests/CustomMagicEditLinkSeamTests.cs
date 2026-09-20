// ============================================================================
// uFrmCustomMagic.pas IVTEditLink 两个实现类的**接缝壳**测试
// 源单元：Source\M2Engine\Forms\uFrmCustomMagic.pas
//   TDecAttribPropertyEditLink :932-1285 ／ TElementPropertyEditLink :1286-1604
// 覆盖：创建/销毁、BeginEdit/CancelEdit、GetBounds/SetBounds、ProcessMessage、
//       消息泵（EditKeyDown/Up 的副作用）、EndEdit → SetConfigChanged 通知。
// ============================================================================

using System;
using GXX.M2Server.Forms.CustomMagic;
using TMagicAttackDecAttributesType = GXX.M2Server.Engine.TMagicAttackDecAttributesType;
using TMagicProtectAddAttributesType = GXX.M2Server.Engine.TMagicProtectAddAttributesType;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("CustomMagicFormLane")]
public sealed class CustomMagicEditLinkSeamTests : CustomMagicTestBase
{
    private static (CustomMagicTreeHost Host, TVirtualNodeSeam Node, TAttackDecAttribData Data) DecSetup(TFrmCustomMagic form)
    {
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        var data = new TAttackDecAttribData
        {
            AttribType = TMagicAttackDecAttributesType.daAC,
            Data = new TMagicChangeAttributesRecord(),
        };
        node.Data = data;
        return (host, node, data);
    }

    // ---- PrepareEdit 重建编辑器（原文 :1167-1171 / :1497-1501）----

    [Fact]
    public void DecAttrib_PrepareEdit_CreatesEditorFromSpec()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, data) = DecSetup(form);
        data.Data!.Rate = 37;

        var link = new TDecAttribPropertyEditLink();
        Assert.True(link.PrepareEdit(host, node, 1));

        var se = Assert.IsType<TSpinEditExSeam>(link.Editor);
        Assert.Equal(37, se.Value);
        Assert.Equal(0, se.MinValue);
        Assert.Equal(100, se.MaxValue);      // 列 1 才有范围
        Assert.Equal(1, link.Column);
        Assert.Same(host, link.Tree);
        Assert.Same(node, link.Node);
    }

    [Fact]
    public void DecAttrib_PrepareEdit_SecondCallReplacesEditor()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, data) = DecSetup(form);

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        var first = link.Editor;

        link.PrepareEdit(host, node, 4);
        Assert.NotSame(first, link.Editor);
        Assert.IsType<TComboBoxSeam>(link.Editor);
        Assert.Equal(new[] { "%", "点" }, ((TComboBoxSeam)link.Editor!).Items.ToArray());
    }

    /// <summary>
    /// 原文缺陷#1：不认识的列（如 12/14）<c>PrepareEdit</c> 仍返回 True 但不建编辑器，
    /// 随后 <c>BeginEdit</c> 的 <c>FEdit.Show</c> 必然 AV。托管侧保留 Result=True，FEdit 为 null。
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(12)]
    [InlineData(14)]
    [InlineData(999)]
    public void DecAttrib_PrepareEdit_UnknownColumn_ReturnsTrueWithNullEditor(int column)
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);

        var link = new TDecAttribPropertyEditLink();
        Assert.True(link.PrepareEdit(host, node, column));
        Assert.Null(link.Editor);
        Assert.Throws<NullReferenceException>(() => link.BeginEdit());
        Assert.Throws<NullReferenceException>(() => link.EndEdit());
    }

    [Fact]
    public void Element_PrepareEdit_CreatesEditorAndNullEditorOnUnknownColumn()
    {
        using var form = MakeFormWith(MakeConfig());
        var host = form.TreeHosts["vstDecElement"];
        var node = host.AddChild(null);
        var data = new TMagicElementData
        {
            ElementType = GXX.M2Server.Engine.TItemElementsType.ietBlastHit,
            Data = new TMagicAttackChangeElementRecord { Value = 12, HintText = "h" },
        };
        node.Data = data;

        var link = new TElementPropertyEditLink();
        Assert.True(link.PrepareEdit(host, node, 3));
        Assert.Equal(12, Assert.IsType<TSpinEditExSeam>(link.Editor).Value);

        Assert.True(link.PrepareEdit(host, node, 10));
        Assert.Equal("h", Assert.IsType<TEditSeam>(link.Editor).Text);

        // 元素树的提示列 9 → 无编辑器（原文缺陷#1 的同型问题）
        Assert.True(link.PrepareEdit(host, node, 9));
        Assert.Null(link.Editor);
    }

    [Fact]
    public void PrepareEdit_WrongNodeDataType_Throws()
    {
        using var form = MakeFormWith(MakeConfig());
        var host = form.TreeHosts["vstDecElement"];
        var node = host.AddChild(null);
        node.Data = new TMagicConfigNodeData();     // 类型不对

        Assert.Throws<InvalidOperationException>(() => new TDecAttribPropertyEditLink().PrepareEdit(host, node, 1));
        Assert.Throws<InvalidOperationException>(() => new TElementPropertyEditLink().PrepareEdit(host, node, 1));
    }

    // ---- BeginEdit / CancelEdit / GetBounds / SetBounds / ProcessMessage ----

    [Fact]
    public void BeginEdit_ShowsAndFocuses()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        link.Editor!.Visible = false;

        Assert.True(link.BeginEdit());
        Assert.True(link.Editor!.Visible);
        Assert.Same(link.Editor, link.FocusedEditor);
    }

    [Fact]
    public void CancelEdit_Hides()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        link.Editor!.Visible = true;

        Assert.True(link.CancelEdit());
        Assert.False(link.Editor!.Visible);
    }

    [Fact]
    public void CancelEdit_WithoutEditor_Throws()
    {
        var link = new TDecAttribPropertyEditLink();
        Assert.Throws<NullReferenceException>(() => link.CancelEdit());
    }

    [Fact]
    public void GetBounds_FromEditorRect()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        link.Editor!.Left = 5;
        link.Editor.Top = 6;
        link.Editor.Width = 7;
        link.Editor.Height = 8;

        var b = link.GetBounds();
        Assert.Equal(5, b.Left);
        Assert.Equal(6, b.Top);
        Assert.Equal(12, b.Right);
        Assert.Equal(14, b.Bottom);
    }

    [Fact]
    public void GetBounds_WithoutEditor_Throws()
        => Assert.Throws<NullReferenceException>(() => new TElementPropertyEditLink().GetBounds());

    /// <summary>原文 :1280-1281 <c>Header.Columns.GetColumnBounds</c> 决定编辑器的右边界。</summary>
    [Fact]
    public void SetBounds_UsesColumnBounds()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        host.SetColumnRight(1, 200);

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        link.SetBounds(new TRectSeam { Left = 20, Top = 3, Right = 999, Bottom = 23 });

        Assert.Equal(20, link.Editor!.Left);
        Assert.Equal(180, link.Editor.Width);    // 200 - 20
        Assert.Equal(3, link.Editor.Top);
        Assert.Equal(20, link.Editor.Height);
    }

    [Fact]
    public void SetBounds_WithoutEditorOrTree_Throws()
    {
        var link = new TElementPropertyEditLink();
        Assert.Throws<NullReferenceException>(() => link.SetBounds(new TRectSeam()));
    }

    [Fact]
    public void ProcessMessage_RecordsUntilPumped()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);

        object msg = "WM_CHAR(65)";
        link.ProcessMessage(ref msg);
        Assert.Equal("WM_CHAR(65)", link.LastProcessedMessage);
    }

    [Fact]
    public void ProcessMessage_WithoutEditor_Throws()
        => Assert.Throws<NullReferenceException>(() =>
        {
            object msg = 1;
            new TElementPropertyEditLink().ProcessMessage(ref msg);
        });

    // ---- 消息泵副作用（原文 :942-986 / :1296-1340）----

    [Fact]
    public void EditKeyDown_Return_EndsEditNode()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);

        int key = TVtKeys.VK_RETURN;
        var action = link.HandleEditKeyDown(true, ref key);

        Assert.Equal(TVtEditKeyAction.SwallowKeyThenEndEditNode, action);
        Assert.Equal(0, key);
        Assert.Equal(1, host.EndEditNodeCalls);
    }

    [Fact]
    public void EditKeyUp_Escape_CancelsEditNode()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);

        int key = TVtKeys.VK_ESCAPE;
        Assert.Equal(TVtEditKeyAction.CancelEditNodeThenSwallowKey, link.HandleEditKeyUp(ref key));
        Assert.Equal(0, key);
        Assert.Equal(1, host.CancelEditNodeCalls);
    }

    [Fact]
    public void EditKeyDown_UpDown_PostsToTreeHandle()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);

        int key = TVtKeys.VK_DOWN;
        var action = link.HandleEditKeyDown(true, ref key);

        Assert.Equal(TVtEditKeyAction.PostKeyDownThenSwallowKey, action);
        Assert.Equal(0, key);
        var posted = Assert.Single(CustomMagicPostMessageSeam.Posted);
        Assert.Equal(TVtKeys.WM_KEYDOWN, posted.Msg);
        Assert.Equal(TVtKeys.VK_DOWN, posted.WParam);
    }

    [Fact]
    public void EditKeyDown_Escape_SwallowsWithoutSideEffects()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);

        int key = TVtKeys.VK_ESCAPE;
        Assert.Equal(TVtEditKeyAction.SwallowKey, link.HandleEditKeyDown(true, ref key));
        Assert.Equal(0, key);
        Assert.Equal(0, host.EndEditNodeCalls);
        Assert.Equal(0, host.CancelEditNodeCalls);
        Assert.Empty(CustomMagicPostMessageSeam.Posted);
    }

    // ---- EndEdit 写回 + Owner 通知（原文 :1133-1144 / :1463-1474）----

    [Fact]
    public void EndEdit_Changed_NotifiesOwnerForm()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var (host, node, data) = DecSetup(form);
        data.Data!.Rate = 1;

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        ((TSpinEditExSeam)link.Editor!).Value = 9;

        Assert.True(link.EndEdit());

        Assert.Equal(9, data.Data.Rate);
        Assert.True(cfg.IsChanged);            // Owner is TFrmCustomMagic → SetConfigChanged()
        Assert.True(form.FIsConfigChanged);
        Assert.True(host.Focused);             // FTree.CanFocus → SetFocus
        Assert.False(link.Editor!.Visible);    // FEdit.Visible := False
    }

    [Fact]
    public void EndEdit_Unchanged_DoesNotNotifyOwner()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var (host, node, data) = DecSetup(form);
        data.Data!.Rate = 9;

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);       // Value 初值就是 9
        Assert.Equal(9, ((TSpinEditExSeam)link.Editor!).Value);

        Assert.True(link.EndEdit());

        Assert.False(cfg.IsChanged);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void EndEdit_OwnerIsNotForm_StillWritesButDoesNotNotify()
    {
        var host = new CustomMagicTreeHost(owner: null);       // Owner 不是 TFrmCustomMagic
        var node = host.AddChild(null);
        var data = new TAttackDecAttribData { Data = new TMagicChangeAttributesRecord() };
        node.Data = data;

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        ((TSpinEditExSeam)link.Editor!).Value = 5;

        Assert.True(link.EndEdit());
        Assert.Equal(5, data.Data!.Rate);
    }

    [Fact]
    public void EndEdit_Element_WritesAndNotifies()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstDecElement"];
        var node = host.AddChild(null);
        var data = new TMagicElementData
        {
            ElementType = GXX.M2Server.Engine.TItemElementsType.ietBlastHit,
            Data = new TMagicAttackChangeElementRecord(),
        };
        node.Data = data;

        var link = new TElementPropertyEditLink();
        link.PrepareEdit(host, node, 3);
        ((TSpinEditExSeam)link.Editor!).Value = 21;
        Assert.True(link.EndEdit());

        Assert.Equal(21, data.Data!.Value);
        Assert.True(cfg.IsChanged);
    }

    [Fact]
    public void EndEdit_ComboAndText_WriteThrough()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var (host, node, data) = DecSetup(form);

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 4);
        ((TComboBoxSeam)link.Editor!).ItemIndex = 1;
        Assert.True(link.EndEdit());
        Assert.True(data.Data!.LowValueIsPoint);

        link.PrepareEdit(host, node, 13);
        ((TEditSeam)link.Editor!).Text = "新提示";
        Assert.True(link.EndEdit());
        Assert.Equal("新提示", data.Data.HintText);
    }

    // ---- Destroy（原文 :932-938 / :1286-1292）----

    [Fact]
    public void Destroy_ClearsEditor_AndIsIdempotent()
    {
        using var form = MakeFormWith(MakeConfig());
        var (host, node, _) = DecSetup(form);
        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        Assert.NotNull(link.Editor);

        link.Destroy();
        Assert.Null(link.Editor);
        link.Destroy();                 // 再次调用不抛（原文 if FEdit <> nil）
        Assert.Null(link.Editor);
    }

    [Fact]
    public void Destroy_WithoutEditor_NoThrow()
    {
        var link = new TElementPropertyEditLink();
        link.Destroy();
        Assert.Null(link.Editor);
    }

    [Fact]
    public void ProtectedAddAttrTree_ReusesDecAttribLink()
    {
        using var form = MakeFormWith(MakeConfig());
        foreach (var tree in new[] { "vstAttackDecAttr", "vstProtectedAddAttr" })
        {
            form.TreeHosts[tree].AddChild(null);
        }

        form.VstAttackDecAttrCreateEditor(out var a);
        form.VstProtectedAddAttrCreateEditor(out var b);
        Assert.IsType<TDecAttribPropertyEditLink>(a);
        Assert.IsType<TDecAttribPropertyEditLink>(b);

        // 也能在保护属性节点上工作（Data 指向 ProtectAddAttrib 的元素）
        var cfg = form.FCurrentCustomConfig!;
        var host = form.TreeHosts["vstProtectedAddAttr"];
        var node = host.AddChild(null);
        node.Data = new TProtectAddAttribData
        {
            AttribType = TMagicProtectAddAttributesType.aaAC,
            Data = cfg.ServerConfig.ProtectAddAttrib[(int)TMagicProtectAddAttributesType.aaAC],
        };

        var link = new TDecAttribPropertyEditLink();
        link.PrepareEdit(host, node, 1);
        ((TSpinEditExSeam)link.Editor!).Value = 3;
        Assert.True(link.EndEdit());
        Assert.Equal(3, cfg.ServerConfig.ProtectAddAttrib[(int)TMagicProtectAddAttributesType.aaAC].Rate);
    }

    [Fact]
    public void EditorFactory_NoneKind_Throws()
        => Assert.Throws<InvalidOperationException>(() =>
            TVtEditorFactory.Create(new TVtEditSpec { Kind = TVtEditKind.None }));

    [Fact]
    public void EditorFactory_CanBeReplaced()
    {
        var stub = new TEditSeam { Text = "stub" };
        TVtEditorFactory.Create = _ => stub;
        try
        {
            using var form = MakeFormWith(MakeConfig());
            var (host, node, _) = DecSetup(form);
            var link = new TDecAttribPropertyEditLink();
            link.PrepareEdit(host, node, 1);
            Assert.Same(stub, link.Editor);
        }
        finally
        {
            TVtEditorFactory.Create = _ => throw new InvalidOperationException("restore");
        }
    }
}
