using System.Windows.Forms;

namespace GXX.DBServer;

/// <summary>
/// Delphi `TListView` 的显示接缝（Caption + SubItems + Items.Count + ListItem.Data 对象挂载）。
/// 单测注入内存实现即可断言"写了哪些行、顺序如何、挂了什么对象"，无需真实 UI。
/// </summary>
public interface IListViewSink
{
    int Count { get; }
    void Clear();

    /// <summary>对应 `ListItem := Items.Add; ListItem.Data := Tag; ListItem.Caption := Caption; SubItems.Add(...)`。</summary>
    void AddRow(string caption, object tag, params string[] subItems);

    /// <summary>取第 index 行挂载的对象（等价 `ListItem.Data`）。</summary>
    object GetTag(int index);
}

/// <summary>把 WinForms `ListView` 适配为 IListViewSink（对象挂载走 ListViewItem.Tag）。</summary>
public class ListViewSink : IListViewSink
{
    private readonly ListView _lv;

    public ListViewSink(ListView lv) => _lv = lv;

    public int Count => _lv.Items.Count;

    public void Clear() => _lv.Items.Clear();

    public void AddRow(string caption, object tag, params string[] subItems)
    {
        var it = new ListViewItem(caption) { Tag = tag };
        it.SubItems.AddRange(subItems);
        _lv.Items.Add(it);
    }

    public object GetTag(int index) => _lv.Items[index].Tag;
}
