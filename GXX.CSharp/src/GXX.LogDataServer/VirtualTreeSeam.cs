using System.Collections.Generic;

namespace GXX.LogDataServer;

/// <summary>
/// 接缝：VirtualTrees.pas TVirtualStringTree / TVirtualNode / TVTHeader / TVTHeaderHitInfo 最小等效。
/// 未移植的 VirtualTrees 由本模型替代，保留 LogManage 真正使用的语义：
/// AddChild/GetFirst/GetNext/Clear/DeleteNode/CheckState/CheckType/Index/GetNodeData/
/// Selected/SelectedCount/GetFirstSelected/GetNextSelected/BeginUpdate/EndUpdate/Invalidate/
/// FullExpand/Text[Node, Column]/SortTree。
/// 待 VirtualTrees 控件族移植后以真实控件替换本接缝。
/// </summary>
public enum TCheckState
{
    csUncheckedNormal,
    csUncheckedPressed,
    csCheckedNormal,
    csCheckedPressed,
    csMixedNormal,
    csMixedPressed,
    csUnCheckedNormal,   // 原文拼写（uFrmBatchEditAccountInfo.pas:315）——原文如此
}

/// <summary>VirtualTrees.TCheckType。</summary>
public enum TCheckType
{
    ctNone,
    ctCheckBox,
    ctRadioButton,
    ctTriStateCheckBox,
}

/// <summary>VirtualTrees.TItemEraseAction。</summary>
public enum TItemEraseAction
{
    eaDefault,
    eaColor,
    eaFontColor,
}

/// <summary>VirtualTrees.TSortDirection。</summary>
public enum TSortDirection
{
    sdAscending,
    sdDescending,
}

/// <summary>VirtualTrees.TVTHeaderHitInfo。</summary>
public struct TVTHeaderHitInfo
{
    public int Column;
    public TMouseButton Button;
}

/// <summary>VirtualTrees.TMouseButton（原文 HitInfo.Button = mbLeft）。</summary>
public enum TMouseButton
{
    mbLeft,
    mbRight,
    mbMiddle,
}

/// <summary>VirtualTrees.TShiftState（set of）。</summary>
[System.Flags]
public enum TShiftState
{
    ssNone = 0,
    ssShift = 1,
    ssAlt = 2,
    ssCtrl = 4,
    ssLeft = 8,
    ssRight = 16,
    ssMiddle = 32,
    ssDouble = 64,
}

/// <summary>VirtualTrees TVirtualNode。</summary>
public sealed class TVirtualNode
{
    /// <summary>同级兄弟序号（Node.Index mod 2 斑马线用）。</summary>
    public int Index;

    public TCheckState CheckState = TCheckState.csUncheckedNormal;
    public TCheckType CheckType = TCheckType.ctNone;
    public bool Visible = true;
    public bool Selected;
    public object? Data;

    public TVirtualNode? Parent;
    public readonly List<TVirtualNode> Children = new();
}

/// <summary>VirtualTrees TVTHeader（LogManage.vstLogHeaderClick 使用）。</summary>
public sealed class TVTHeader
{
    public const int NoColumn = -1;

    public int SortColumn = NoColumn;
    public TSortDirection SortDirection = TSortDirection.sdAscending;

    /// <summary>Treeview.SortTree(Column, Direction, Recursive)。</summary>
    public System.Action<int, TSortDirection, bool>? SortTreeHandler;

    public void SortTree(int column, TSortDirection direction, bool recursive)
        => SortTreeHandler?.Invoke(column, direction, recursive);
}

/// <summary>VirtualTrees 树控件最小模型。</summary>
public sealed class TVirtualStringTreeModel
{
    public int NodeDataSize;
    public readonly List<TVirtualNode> Roots = new();
    public readonly TVTHeader Header = new();
    public TVirtualNode? FocusedNode;
    public int FocusedColumn = -1;
    public int UpdateDepth;

    /// <summary>对应 TVirtualStringTree.Text[Node, Column]（由窗体注册取文本回调）。</summary>
    public System.Func<TVirtualNode, int, string>? TextHandler;

    public string GetText(TVirtualNode? node, int column)
        => node == null ? "" : (TextHandler?.Invoke(node, column) ?? "");

    public TVirtualNode AddChild(TVirtualNode? parent)
    {
        var node = new TVirtualNode { Parent = parent };
        var list = parent?.Children ?? Roots;
        node.Index = list.Count;
        list.Add(node);
        return node;
    }

    public void Clear() => Roots.Clear();

    public IEnumerable<TVirtualNode> AllNodes()
    {
        foreach (var n in Roots) foreach (var x in Descend(n)) yield return x;
    }

    private static IEnumerable<TVirtualNode> Descend(TVirtualNode n)
    {
        yield return n;
        foreach (var c in n.Children) foreach (var x in Descend(c)) yield return x;
    }

    public TVirtualNode? GetFirst()
    {
        foreach (var n in AllNodes()) return n;
        return null;
    }

    public TVirtualNode? GetNext(TVirtualNode? node)
    {
        if (node == null) return null;
        var flat = new List<TVirtualNode>(AllNodes());
        int i = flat.IndexOf(node);
        return i >= 0 && i + 1 < flat.Count ? flat[i + 1] : null;
    }

    public TVirtualNode? GetFirstSelected()
    {
        foreach (var n in AllNodes()) if (n.Selected) return n;
        return null;
    }

    public TVirtualNode? GetNextSelected(TVirtualNode? node)
    {
        if (node == null) return null;
        var flat = new List<TVirtualNode>(AllNodes());
        int i = flat.IndexOf(node);
        for (int k = i + 1; k < flat.Count; k++) if (flat[k].Selected) return flat[k];
        return null;
    }

    public int SelectedCount
    {
        get
        {
            int n = 0;
            foreach (var x in AllNodes()) if (x.Selected) n++;
            return n;
        }
    }

    public void DeleteNode(TVirtualNode? node)
    {
        if (node == null) return;
        var list = node.Parent?.Children ?? Roots;
        int idx = list.IndexOf(node);
        if (idx < 0) return;
        list.RemoveAt(idx);
        for (int i = idx; i < list.Count; i++) list[i].Index = i;
    }

    public bool GetVisible(TVirtualNode n) => n.Visible;
    public void SetVisible(TVirtualNode n, bool value) => n.Visible = value;

    public void BeginUpdate() => UpdateDepth++;
    public void EndUpdate() => UpdateDepth--;

    public int InvalidateCount;
    public void Invalidate() => InvalidateCount++;
    public void InvalidateNode(TVirtualNode? node) => InvalidateCount++;

    public void FullExpand(TVirtualNode? node)
    {
        // 模型无折叠状态；保留调用点语义
    }

    public int RootNodeCount => Roots.Count;

    /// <summary>对应 Treeview.SortTree：按 vstLogCompareNodes 排序根节点。</summary>
    public void SortRoots(System.Comparison<TVirtualNode> compare, TSortDirection direction)
    {
        Roots.Sort((a, b) =>
        {
            int r = compare(a, b);
            return direction == TSortDirection.sdAscending ? r : -r;
        });
        for (int i = 0; i < Roots.Count; i++) Roots[i].Index = i;
    }
}
