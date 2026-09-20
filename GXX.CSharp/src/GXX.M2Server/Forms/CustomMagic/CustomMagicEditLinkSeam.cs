// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = IVTEditLink 两个实现类的**接缝壳**（VirtualTrees.pas 未移植）：
//   * TDecAttribPropertyEditLink（原文 :888-907 / :932-1285）
//   * TElementPropertyEditLink （原文 :909-928 / :1286-1604）
//
// 取值/写回规则已全部抽到 CustomMagicEditLinkLogic.cs（纯逻辑、已单测）；
// 本文件只保留原文的**创建 / 销毁 / 消息泵 / 宿主交互**骨架：
//   FEdit.Free / FEdit.Show / FEdit.SetFocus / FEdit.Hide / FEdit.Visible /
//   FEdit.WindowProc / FTree.EndEditNode / FTree.CancelEditNode / FTree.SetFocus /
//   FTree.CanFocus / FTree.Header.Columns.GetColumnBounds —— 全部通过 EditorFactory 与
//   IVirtualTreeHost 接缝注入；生产侧接 WinForms 控件，测试侧接内存替身。
//
// 覆盖行号（Delphi）：
//   Destroy  :932-938 / :1286-1292
//   BeginEdit:990-995 / :1344-1349
//   CancelEdit:999-1003 / :1353-1357
//   EndEdit  :1007-1145 / :1361-1475（判定已抽纯逻辑；此处只做调用序列）
//   GetBounds:1149-1152 / :1479-1482
//   ProcessMessage:1267-1270 / :1588-1591
//   SetBounds:1274-1282 / :1595-1603
// ============================================================================

using System;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

/// <summary>
/// 编辑器控件工厂接缝（原文 <c>TSpinEditEx.Create(nil)</c> / <c>TComboBox.Create(nil)</c> /
/// <c>TEdit.Create(nil)</c>）。
/// 默认实现返回接缝控件（无头安全）；生产侧可替换为真实 WinForms 控件。
/// </summary>
public static class TVtEditorFactory
{
    /// <summary>按 spec 造编辑器。默认实现造接缝控件并把 spec 的初值灌进去。</summary>
    public static Func<TVtEditSpec, TWinControlSeam> Create = spec =>
    {
        switch (spec.Kind)
        {
            case TVtEditKind.SpinEdit:
                var se = new TSpinEditExSeam
                {
                    Value = spec.Value,
                    Visible = !spec.VisibleInitFalse,
                };
                if (spec.HasMinMax)
                {
                    se.MinValue = spec.MinValue;
                    se.MaxValue = spec.MaxValue;
                }
                return se;

            case TVtEditKind.ComboBox:
                var cbb = new TComboBoxSeam
                {
                    Visible = !spec.VisibleInitFalse,
                    Style = spec.DropDownList ? 3 /* csDropDownList */ : 0,
                    ItemIndex = spec.ItemIndex,
                };
                cbb.Items.AddRange(spec.Items);
                return cbb;

            case TVtEditKind.Edit:
                return new TEditSeam { Visible = !spec.VisibleInitFalse, Text = spec.Text };

            default:
                throw new InvalidOperationException(
                    $"TVtEditorFactory: 列未匹配任何编辑器分支（原文 FEdit 保持 nil，BeginEdit 会 AV）。Kind={spec.Kind}");
        }
    };
}

/// <summary>
/// 两个 EditLink 的共享骨架（原文两份实现除 PrepareEdit/EndEdit 的字段名外逐字相同）。
/// </summary>
public abstract class TVtEditLinkSeamBase : IVTEditLinkSeam
{
    /// <summary>原文 <c>FEdit: TWinControl</c>（:890 / :911）。</summary>
    protected TWinControlSeam? FEdit;

    /// <summary>原文 <c>FTree: TVirtualStringTree</c>（:891 / :912）。</summary>
    protected IVirtualTreeHost? FTree;

    /// <summary>原文 <c>FNode: PVirtualNode</c>（:892 / :913）。</summary>
    protected TVirtualNodeSeam? FNode;

    /// <summary>原文 <c>FColumn: Integer</c>（:893 / :914）。</summary>
    protected int FColumn;

    /// <summary>PrepareEdit / EndEdit 用的节点数据通道（原文 <c>FTree.GetNodeData</c>）。</summary>
    protected abstract object? GetNodeData(TVirtualNodeSeam node);

    /// <summary>
    /// 原文 <c>EndEdit</c> 里 <c>if (FTree.Owner is TFrmCustomMagic) then TFrmCustomMagic(FTree.Owner).SetConfigChanged();</c>
    /// </summary>
    protected static void NotifyOwnerIfChanged(IVirtualTreeHost tree)
    {
        if (tree.Owner is TFrmCustomMagic frm)
            frm.SetConfigChanged();
    }

    /// <summary>原文 <c>destructor Destroy</c>（:932-938 / :1286-1292）：FEdit 非 nil 则 Free。</summary>
    public void Destroy()
    {
        if (FEdit != null)
        {
            FEdit = null;   // FEdit.Free
        }
        // inherited;
    }

    /// <summary>原文 <c>function BeginEdit: Boolean</c>（:990-995 / :1344-1349）。</summary>
    public bool BeginEdit()
    {
        // 原文：Result := True; FEdit.Show; FEdit.SetFocus;
        if (FEdit is null)
            throw new NullReferenceException("BeginEdit: FEdit 为 nil（原文此处 AV —— 列未匹配编辑器分支）");
        FEdit.Visible = true;   // FEdit.Show
        FocusedEditor = FEdit;  // FEdit.SetFocus
        return true;
    }

    /// <summary>SetFocus 目标（镜像，无头环境无法真聚焦）。</summary>
    public TWinControlSeam? FocusedEditor { get; private set; }

    /// <summary>原文 <c>function CancelEdit: Boolean</c>（:999-1003 / :1353-1357）。</summary>
    public bool CancelEdit()
    {
        if (FEdit is null)
            throw new NullReferenceException("CancelEdit: FEdit 为 nil（原文此处 AV）");
        FEdit.Visible = false;  // FEdit.Hide
        return true;
    }

    /// <summary>原文 <c>function GetBounds: TRect</c>（:1149-1152 / :1479-1482）。</summary>
    public TRectSeam GetBounds()
    {
        if (FEdit is null)
            throw new NullReferenceException("GetBounds: FEdit 为 nil（原文此处 AV）");
        return new TRectSeam { Left = FEdit.Left, Top = FEdit.Top, Right = FEdit.Left + FEdit.Width, Bottom = FEdit.Top + FEdit.Height };
    }

    /// <summary>原文 <c>procedure ProcessMessage(var Message: TMessage)</c>（:1267-1270 / :1588-1591）。</summary>
    public void ProcessMessage(ref object message)
    {
        if (FEdit is null)
            throw new NullReferenceException("ProcessMessage: FEdit 为 nil（原文此处 AV）");
        // 原文 FEdit.WindowProc(Message)：消息泵留接缝，这里只记录未投递的消息。
        LastProcessedMessage = message;
    }

    /// <summary>ProcessMessage 的消息镜像（生产运行时恒等于真实投递内容）。</summary>
    public object? LastProcessedMessage { get; private set; }

    /// <summary>原文 <c>procedure SetBounds(R: TRect)</c>（:1274-1282 / :1595-1603）。</summary>
    public void SetBounds(TRectSeam r)
    {
        if (FTree is null || FEdit is null)
            throw new NullReferenceException("SetBounds: FTree/FEdit 为 nil（原文此处 AV）");
        // FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right); FEdit.BoundsRect := R;
        int right = FTree.GetColumnBounds(FColumn, r.Right);
        FEdit.Width = right - r.Left;
        FEdit.Left = r.Left;
        FEdit.Top = r.Top;
        FEdit.Height = r.Bottom - r.Top;
    }

    /// <summary>共享前段：<c>FTree := Tree as TVirtualStringTree; FNode := Node; FColumn := Column;
    /// if FEdit &lt;&gt; nil then begin FEdit.Free; FEdit := nil; end;</c>（:1162-1171 / :1492-1501）。</summary>
    protected void PrepareCommon(IVirtualTreeHost tree, TVirtualNodeSeam node, int column)
    {
        FTree = tree;
        FNode = node;
        FColumn = column;

        if (FEdit != null)
            Destroy();
    }

    /// <summary>
    /// 原文 <c>EditKeyDown</c>（:942-975 / :1296-1329）：纯判定在 TVtEditLinkKeys，
    /// 这里执行副作用（Key 置 0 / EndEditNode / PostMessage）。
    /// </summary>
    public TVtEditKeyAction HandleEditKeyDown(bool shiftEmpty, ref int key)
    {
        var kind = FEdit switch
        {
            TComboBoxSeam => TVtEditKind.ComboBox,
            TSpinEditExSeam => TVtEditKind.SpinEdit,
            TEditSeam => TVtEditKind.Edit,
            _ => TVtEditKind.None,
        };
        bool dropped = FEdit is TComboBoxSeam cbb && cbb.DroppedDown;

        var action = TVtEditLinkKeys.EditKeyDown(kind, dropped, shiftEmpty, key);
        switch (action)
        {
            case TVtEditKeyAction.SwallowKey:
                key = 0;
                break;
            case TVtEditKeyAction.SwallowKeyThenEndEditNode:
                key = 0;
                FTree?.EndEditNode();
                break;   // 原文随后 Abort（VCL 吞掉），不再执行本处理器后续语句
            case TVtEditKeyAction.PostKeyDownThenSwallowKey:
                if (FTree != null)
                    CustomMagicPostMessageSeam.PostMessage(FTree.Handle, TVtKeys.WM_KEYDOWN, key, 0);
                key = 0;
                break;
        }
        return action;
    }

    /// <summary>原文 <c>EditKeyUp</c>（:977-986 / :1331-1340）。</summary>
    public TVtEditKeyAction HandleEditKeyUp(ref int key)
    {
        var action = TVtEditLinkKeys.EditKeyUp(key);
        if (action == TVtEditKeyAction.CancelEditNodeThenSwallowKey)
        {
            FTree?.CancelEditNode();
            key = 0;
        }
        return action;
    }

    /// <summary>被持有的编辑器（测试/断言用，只读）。</summary>
    public TWinControlSeam? Editor => FEdit;

    /// <summary>被指向的树（测试/断言用，只读）。</summary>
    public IVirtualTreeHost? Tree => FTree;

    /// <summary>被编辑的节点（测试/断言用，只读）。</summary>
    public TVirtualNodeSeam? Node => FNode;

    /// <summary>被编辑的列（测试/断言用，只读）。</summary>
    public int Column => FColumn;

    /// <summary>原文 <c>FTree.CanFocus / FTree.SetFocus / if FEdit.Visible then FEdit.Visible := False;</c>
    /// （EndEdit 尾部，:1133-1136 / :1463-1466）。</summary>
    protected void EndEditTail()
    {
        if (FTree != null && FTree.CanFocus)
            FTree.SetFocus();
        if (FEdit != null && FEdit.Visible)
            FEdit.Visible = false;
    }

    /// <summary>原文 <c>function PrepareEdit(...): Boolean; stdcall</c>（:1156 / :1486）。</summary>
    public abstract bool PrepareEdit(IVirtualTreeHost tree, TVirtualNodeSeam node, int column);

    /// <summary>原文 <c>function EndEdit: Boolean; stdcall</c>（:1007 / :1361）。</summary>
    public abstract bool EndEdit();
}

/// <summary>
/// 原文 <c>TDecAttribPropertyEditLink</c>（:888-907 / :932-1285）。
/// 被 vstAttackDecAttr（:4414-4419）与 vstProtectedAddAttr（:4709-4714）使用。
/// </summary>
public sealed class TDecAttribPropertyEditLink : TVtEditLinkSeamBase
{
    /// <inheritdoc/>
    protected override object? GetNodeData(TVirtualNodeSeam node) => FTree?.GetNodeData(node);

    /// <summary>原文 <c>function PrepareEdit(...): Boolean; stdcall</c>（:1156-1263）：
    /// 重建编辑器并灌初值；**Result 恒为 True**（原文如此）。</summary>
    public override bool PrepareEdit(IVirtualTreeHost tree, TVirtualNodeSeam node, int column)
    {
        PrepareCommon(tree, node, column);

        if (GetNodeData(node) is not TAttackDecAttribData data)
            throw new InvalidOperationException("PrepareEdit: GetNodeData 不是 TAttackDecAttribData");

        var spec = DecAttribPropertyEditLinkLogic.PrepareEditSpec(column, data);
        if (spec.Kind == TVtEditKind.None)
        {
            // 原文如此（:1161 Result := True 后不建编辑器）——保留"Result 仍为 True"的语义，
            // 但 FEdit 保持 nil；后续 BeginEdit/EndEdit 会与原文一样崩溃（见报告"原文缺陷"#1）。
            FEdit = null;
            return true;
        }

        FEdit = TVtEditorFactory.Create(spec);
        return true;
    }

    /// <summary>原文 <c>function EndEdit: Boolean; stdcall</c>（:1007-1145）。</summary>
    public override bool EndEdit()
    {
        if (FEdit is null)
            throw new NullReferenceException("EndEdit: FEdit 为 nil（原文此处 AV —— 列未匹配编辑器分支）");

        var data = (TAttackDecAttribData)GetNodeData(FNode!)!;
        var result = ReadEditorResult();
        bool isChanged = DecAttribPropertyEditLinkLogic.ApplyEditorResult(FColumn, data, result);

        EndEditTail();

        if (isChanged && FTree != null)
            NotifyOwnerIfChanged(FTree);

        return true;
    }

    /// <summary>取当前编辑器的值（原文三种 <c>as</c> 强转）。</summary>
    private TVtEditorResult ReadEditorResult() => FEdit switch
    {
        TSpinEditExSeam se => new TVtEditorResult { Value = se.Value },
        TSpinEditSeam se => new TVtEditorResult { Value = se.Value },
        TComboBoxSeam cbb => new TVtEditorResult { ItemIndex = cbb.ItemIndex },
        TEditSeam edt => new TVtEditorResult { Text = edt.Text },
        _ => throw new InvalidOperationException("EndEdit: 未知编辑器类型（原文 as 强转会抛异常）"),
    };
}

/// <summary>
/// 原文 <c>TElementPropertyEditLink</c>（:909-928 / :1286-1604）。
/// 被 vstDecElement / vstAddElement 的 OnCreateEditor 使用（:4527-4532）。
/// </summary>
public sealed class TElementPropertyEditLink : TVtEditLinkSeamBase
{
    /// <inheritdoc/>
    protected override object? GetNodeData(TVirtualNodeSeam node) => FTree?.GetNodeData(node);

    /// <summary>原文 <c>function PrepareEdit(...): Boolean; stdcall</c>（:1486-1587）：
    /// 重建编辑器并灌初值；**Result 恒为 True**（原文如此）。</summary>
    public override bool PrepareEdit(IVirtualTreeHost tree, TVirtualNodeSeam node, int column)
    {
        PrepareCommon(tree, node, column);

        if (GetNodeData(node) is not TMagicElementData data)
            throw new InvalidOperationException("PrepareEdit: GetNodeData 不是 TMagicElementData");

        var spec = ElementPropertyEditLinkLogic.PrepareEditSpec(column, data);
        if (spec.Kind == TVtEditKind.None)
        {
            FEdit = null;
            return true;
        }

        FEdit = TVtEditorFactory.Create(spec);
        return true;
    }

    /// <summary>原文 <c>function EndEdit: Boolean; stdcall</c>（:1361-1475）。</summary>
    public override bool EndEdit()
    {
        if (FEdit is null)
            throw new NullReferenceException("EndEdit: FEdit 为 nil（原文此处 AV —— 列未匹配编辑器分支）");

        var data = (TMagicElementData)GetNodeData(FNode!)!;
        var result = ReadEditorResult();
        bool isChanged = ElementPropertyEditLinkLogic.ApplyEditorResult(FColumn, data, result);

        EndEditTail();

        if (isChanged && FTree != null)
            NotifyOwnerIfChanged(FTree);

        return true;
    }

    /// <summary>取当前编辑器的值（原文三种 <c>as</c> 强转）。</summary>
    private TVtEditorResult ReadEditorResult() => FEdit switch
    {
        TSpinEditExSeam se => new TVtEditorResult { Value = se.Value },
        TSpinEditSeam se => new TVtEditorResult { Value = se.Value },
        TComboBoxSeam cbb => new TVtEditorResult { ItemIndex = cbb.ItemIndex },
        TEditSeam edt => new TVtEditorResult { Text = edt.Text },
        _ => throw new InvalidOperationException("EndEdit: 未知编辑器类型（原文 as 强转会抛异常）"),
    };
}

/// <summary>
/// TVirtualStringTree 的**内存宿主**（生产侧由 WinForms TreeView(OwnerDraw) 适配器替换）。
/// 已实现：节点数据、勾选、选中/焦点镜像、AddChild/Clear/GetFirst/GetNext、
/// 列宽表、编辑/取消编辑的派发。
/// 未实现（如实登记）：真实绘制、滚动、命中测试、Header 拖拽。
/// </summary>
public sealed class CustomMagicTreeHost : IVirtualTreeHost
{
    private readonly List<TVirtualNodeSeam> _nodes = new();
    private readonly Dictionary<TVirtualNodeSeam, TCheckState> _checkStates = new();
    private readonly Dictionary<TVirtualNodeSeam, bool> _selected = new();
    private readonly Dictionary<int, int> _columnRights = new();

    /// <summary>构造：Owner 一般为 TFrmCustomMagic。</summary>
    public CustomMagicTreeHost(object? owner = null) => Owner = owner;

    /// <summary>property NodeDataSize（原文 FormCreate 设置 SizeOf(T...Data)）。
    /// 托管侧节点数据是对象引用，故该值只作镜像，不参与分配。</summary>
    public int NodeDataSize { get; set; }

    /// <inheritdoc/>
    public TVirtualNodeSeam? FocusedNode { get; set; }

    /// <inheritdoc/>
    public bool Focused { get; set; }

    /// <inheritdoc/>
    public bool CanFocus { get; set; } = true;

    /// <inheritdoc/>
    public void SetFocus() => Focused = true;

    /// <inheritdoc/>
    public object? Owner { get; }

    /// <inheritdoc/>
    public IntPtr Handle { get; set; }

    /// <inheritdoc/>
    public int FontColor { get; set; }

    /// <inheritdoc/>
    public object? GetNodeData(TVirtualNodeSeam? node) => node?.Data;

    /// <inheritdoc/>
    public void InvalidateNode(TVirtualNodeSeam? node) => InvalidatedNodes.Add(node);

    /// <summary>InvalidateNode 调用记录（无头断言用）。</summary>
    public readonly List<TVirtualNodeSeam?> InvalidatedNodes = new();

    /// <summary>EndEditNode 调用次数。</summary>
    public int EndEditNodeCalls;
    /// <summary>CancelEditNode 调用次数。</summary>
    public int CancelEditNodeCalls;
    /// <summary>Invalidate 调用次数。</summary>
    public int InvalidateCalls;

    /// <inheritdoc/>
    public void EndEditNode() => EndEditNodeCalls++;
    /// <inheritdoc/>
    public void CancelEditNode() => CancelEditNodeCalls++;
    /// <inheritdoc/>
    public void Invalidate() => InvalidateCalls++;

    /// <inheritdoc/>
    public bool IsSelected(TVirtualNodeSeam node) => _selected.TryGetValue(node, out bool v) && v;

    /// <summary>设置选中（测试/生产适配器用）。</summary>
    public void SetSelected(TVirtualNodeSeam node, bool value) => _selected[node] = value;

    /// <inheritdoc/>
    public TCheckState GetCheckState(TVirtualNodeSeam node) => _checkStates.TryGetValue(node, out var v) ? v : node.CheckState;

    /// <summary>设置勾选状态（对应 <c>Sender.CheckState[Node]</c>）。</summary>
    public void SetCheckState(TVirtualNodeSeam node, TCheckState state)
    {
        _checkStates[node] = state;
        node.CheckState = state;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _nodes.Clear();
        _checkStates.Clear();
        _selected.Clear();
        FocusedNode = null;
    }

    /// <inheritdoc/>
    public TVirtualNodeSeam AddChild(TVirtualNodeSeam? parent)
    {
        var node = new TVirtualNodeSeam { Index = _nodes.Count };
        _nodes.Add(node);
        return node;
    }

    /// <summary>全部节点（顺序即 AddChild 顺序）。</summary>
    public IReadOnlyList<TVirtualNodeSeam> Nodes => _nodes;

    /// <inheritdoc/>
    public TVirtualNodeSeam? GetFirst() => _nodes.Count == 0 ? null : _nodes[0];

    /// <inheritdoc/>
    public TVirtualNodeSeam? GetNext(TVirtualNodeSeam node)
    {
        int i = _nodes.IndexOf(node);
        if (i < 0 || i + 1 >= _nodes.Count) return null;
        return _nodes[i + 1];
    }

    /// <summary>EditNode 调用记录：(Node, Column)。</summary>
    public readonly List<(TVirtualNodeSeam? Node, int Column)> EditNodeCalls = new();

    /// <inheritdoc/>
    public void EditNode(TVirtualNodeSeam? node, int column) => EditNodeCalls.Add((node, column));

    /// <summary>设置某列的右边界（<c>Header.Columns.GetColumnBounds</c>）。</summary>
    public void SetColumnRight(int column, int right) => _columnRights[column] = right;

    /// <inheritdoc/>
    public int GetColumnBounds(int column, int right)
        => _columnRights.TryGetValue(column, out int v) ? v : right;
}
