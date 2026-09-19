using System.Collections.Generic;

namespace GXX.LoginSrv;

/// <summary>
/// 接缝：VirtualTrees.pas TVirtualStringTree / TVirtualNode / TVTHeader 最小等效。
/// 未移植的 VirtualTrees 由本模型替代，保留窗体真正使用的语义：
/// AddChild/GetFirst/GetNext/GetFirstChecked/GetNextChecked/DeleteNode/IsVisible/
/// CheckState/CheckType/Index/GetNodeData/BeginUpdate/EndUpdate/Invalidate(InvalidateNode)。
/// 待 VirtualTrees 控件族移植后以真实控件替换本接缝。
/// </summary>
public sealed class TVirtualNode
{
    /// <summary>同级兄弟序号（VirtualTrees.PVirtualNode.Index 语义，供斑马线 Node.Index mod 2 使用）。</summary>
    public int Index;

    /// <summary>TVirtualNode.CheckState（仅用到 csUncheckedNormal / csCheckedNormal / csUnCheckedNormal 拼写差异见原文）。</summary>
    public TCheckState CheckState = TCheckState.csUncheckedNormal;

    /// <summary>TVirtualNode.CheckType。</summary>
    public TCheckType CheckType = TCheckType.ctNone;

    /// <summary>IsVisible[Node]（过滤用）。</summary>
    public bool Visible = true;

    /// <summary>GetNodeData 返回的节点数据（窗体侧用具体结构体/类承载）。</summary>
    public object? Data;

    public TVirtualNode? Parent;
    public readonly List<TVirtualNode> Children = new();
}

/// <summary>VirtualTrees.TCheckState（仅本批次用到的枚举值，拼写 1:1）。</summary>
public enum TCheckState
{
    csUncheckedNormal,
    csUncheckedPressed,
    csCheckedNormal,
    csCheckedPressed,
    csMixedNormal,
    csMixedPressed,
    csUnCheckedNormal,   // 原文 uFrmBatchEditAccountInfo.pas:315 的拼写（大小写 K）——原文如此
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

/// <summary>VirtualTrees 树控件最小模型（见类型注释）。</summary>
public sealed class TVirtualStringTreeModel
{
    public int NodeDataSize;
    public readonly List<TVirtualNode> Roots = new();
    public TVirtualNode? FocusedNode;
    public int FocusedColumn = -1;
    public int UpdateDepth;

    /// <summary>原文 AddChild(nil, [data]) 的等效。</summary>
    public TVirtualNode AddChild(TVirtualNode? parent)
    {
        var node = new TVirtualNode { Parent = parent };
        var list = parent?.Children ?? Roots;
        node.Index = list.Count;
        list.Add(node);
        return node;
    }

    public void Clear() => Roots.Clear();

    /// <summary>按插入顺序（深度优先）枚举全部节点。</summary>
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

    public TVirtualNode? GetFirstChecked()
    {
        foreach (var n in AllNodes())
            if (n.Visible && IsChecked(n)) return n;
        return null;
    }

    public TVirtualNode? GetNextChecked(TVirtualNode? node)
    {
        if (node == null) return null;
        var flat = new List<TVirtualNode>(AllNodes());
        int i = flat.IndexOf(node);
        for (int k = i + 1; k < flat.Count; k++)
            if (flat[k].Visible && IsChecked(flat[k])) return flat[k];
        return null;
    }

    /// <summary>原文 csCheckedNormal 视为选中（csUnCheckedNormal 是原文笔误，非选中）。</summary>
    public static bool IsChecked(TVirtualNode n) => n.CheckState == TCheckState.csCheckedNormal;

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

    public int RootNodeCount => Roots.Count;

    /// <summary>原文 vstLog.Text[Node, Column] 的等效（本批次只取首列）。</summary>
    public string GetText(TVirtualNode? node) => node?.Data as string ?? "";
}
