// ============================================================================
// 车道 p9-m2-forms（M2Engine 窗体/杂项族 7 单元）—— 测试基础设施
//
//  · 串行集合 `Sweep9FormsSerial`：本车道窗体族读写静态接缝（
//    Sweep9FormsPlugClientGlobals / Sweep9FormsMessageBoxSeam / M2Config.*），
//    按《并行派发台账》§19.5「测试间静态全局污染」的既定处置串行。
//  · Sweep9FormsReconcile —— **DFM 对账取证**（§37.3：否定性断言必须计数取证）：
//      · CountControls      —— 控件树逐层计数（含/不含窗体自身两种口径）
//      · CountEventBindings —— 反射统计**已挂接的事件委托**数
//        （逐类型 DeclaredOnly 找 "与事件同名" 的私有字段，非 null 即已订阅）
//      · CountEventHandlerMethods —— 窗体上的处理器方法数（DFM 处理器去重口径）
//  · Sweep9FormsFakeFileSearch —— FindFirst/FindNext/FindClose 的假实现（不碰磁盘）
// ============================================================================

using System.Reflection;
using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>本车道窗体族测试串行集合（静态接缝互斥）。</summary>
[CollectionDefinition("Sweep9FormsSerial", DisableParallelization = true)]
public sealed class Sweep9FormsSerialCollection
{
}

/// <summary>
/// DFM 对账工具（《并行派发台账》§37.3：「DFM 未绑定某事件」「原文没有这一支」这类
/// **否定性断言必须用计数对账取证**）。
/// </summary>
public static class Sweep9FormsReconcile
{
    /// <summary>
    /// 递归计数控件树（**含**窗体自身 = DFM 的 `object Form: TForm` 根节点口径）。
    /// </summary>
    public static int CountControlsIncludingForm(System.Windows.Forms.Form form)
        => 1 + CountChildren(form);

    /// <summary>递归计数控件树（**不含**窗体自身 = DFM 里挂在窗体下的 `object` 数）。</summary>
    public static int CountControlsExcludingForm(System.Windows.Forms.Form form)
        => CountChildren(form);

    private static int CountChildren(System.Windows.Forms.Control parent)
    {
        int n = 0;
        foreach (System.Windows.Forms.Control c in parent.Controls)
            n += 1 + CountChildren(c);
        return n;
    }

    /// <summary>控件树全部控件的 (名称, 类型) 列表（含窗体自身，深度优先、保持 Controls 顺序）。</summary>
    public static List<(string Name, Type Type)> EnumerateControls(System.Windows.Forms.Form form)
    {
        var list = new List<(string, Type)> { (form.Name, form.GetType()) };
        Walk(form, list);
        return list;
    }

    private static void Walk(System.Windows.Forms.Control parent, List<(string, Type)> list)
    {
        foreach (System.Windows.Forms.Control c in parent.Controls)
        {
            list.Add((c.Name, c.GetType()));
            Walk(c, list);
        }
    }

    /// <summary>
    /// 反射统计**已挂接的事件委托**总数（含窗体自身的 DFM 级事件，如 `OnCreate`）。
    /// <para>
    /// ★ 实测结论（.NET 8 WinForms）：`Control`/`Form` 的事件**不是** field-like event，
    /// 没有"与事件同名"的私有委托字段（先按字段法实现 ⇒ 实测 0，误报！）。实际存储是
    /// `Component.Events`（<see cref="System.ComponentModel.EventHandlerList"/>）+
    /// 每个事件在声明类型上的静态键对象（`private static readonly object EventXxx`）。
    /// 故本方法两条路都走：
    /// </para>
    /// <list type="number">
    /// <item>若存在与事件同名的私有实例委托字段（field-like，自定义控件常见）⇒ 看字段非 null；</item>
    /// <item>否则找声明类型上的静态键 `Event` + 事件名 ⇒ 用
    /// <c>EventHandlerList[key]</c>（public 索引器）判定非 null。</item>
    /// </list>
    /// <para>
    /// 「两条路都没命中」的事件**不予计数**；各窗体的对账用例另有"逐控件期望绑定数"断言，
    /// 因此漏计会被抓出（而不是静默少数）。
    /// </para>
    /// </summary>
    public static int CountEventBindings(System.Windows.Forms.Form form)
    {
        int n = CountEventBindingsOn(form);
        foreach (System.Windows.Forms.Control c in AllControls(form))
            n += CountEventBindingsOn(c);
        return n;
    }

    /// <summary>统计单个控件/组件对象上已挂接的事件数。</summary>
    public static int CountEventBindingsOn(System.ComponentModel.Component component)
        => CountEventBindingsOn(component.GetType(), component);

    /// <summary>统计形如 `edtSearch.TextChanged += ...` 这类"控件 + 事件名"是否已挂接。</summary>
    public static bool IsBound(System.ComponentModel.Component component, string eventName)
        => IsBoundOn(component.GetType(), component, eventName);

    private static bool IsBoundOn(Type type, object instance, string eventName)
    {
        var events = GetEventHandlerList(instance);
        for (Type? t = type; t != null && t != typeof(object); t = t.BaseType)
        {
            const BindingFlags declaredPub = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            if (t.GetEvent(eventName, declaredPub) == null) continue;
            if (TryFieldLike(t, instance, eventName)) return true;
            if (TryKeyed(events, t, eventName)) return true;
        }
        return false;
    }

    private static int CountEventBindingsOn(Type type, object? instance)
    {
        if (instance == null) return 0;
        var events = GetEventHandlerList(instance);
        int n = 0;
        for (Type? t = type; t != null && t != typeof(object); t = t.BaseType)
        {
            const BindingFlags declaredPub = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            foreach (var ev in t.GetEvents(declaredPub))
            {
                if (TryFieldLike(t, instance, ev.Name)) { n++; continue; }
                if (TryKeyed(events, t, ev.Name)) n++;
            }
        }
        return n;
    }

    /// <summary>field-like event：与事件同名的私有实例委托字段非 null。</summary>
    private static bool TryFieldLike(Type declaringType, object instance, string eventName)
    {
        var f = declaringType.GetField(eventName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (f == null || !typeof(Delegate).IsAssignableFrom(f.FieldType)) return false;
        return f.GetValue(instance) != null;
    }

    /// <summary>WinForms 式：静态键对象（`EventXxx` / `s_xxxEvent` 两种命名都实测存在）⇒ `EventHandlerList[key]` 非 null。</summary>
    private static bool TryKeyed(System.ComponentModel.EventHandlerList? events, Type declaringType, string eventName)
    {
        if (events == null) return false;
        foreach (var keyField in declaringType.GetFields(
                     BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            if (keyField.FieldType != typeof(object)) continue;
            if (!IsKeyFieldNameFor(keyField.Name, eventName)) continue;
            object? key = keyField.GetValue(null);
            if (key != null && events[key] != null) return true;
        }
        return false;
    }

    /// <summary>
    /// 判定静态字段名是否是对应事件的键。
    /// .NET Framework：`Event` + 事件名（`EventDoubleClick`）；
    /// .NET 8：`s_` + 首字母小写事件名 + `Event`（`s_doubleClickEvent`）。
    /// 两种前缀/后缀都剥掉后做**整体**（非子串）比较 ⇒ 不会把 `EventClick` 误配给 `DoubleClick`。
    /// </summary>
    private static bool IsKeyFieldNameFor(string fieldName, string eventName)
    {
        string s = fieldName;
        if (s.StartsWith("s_", StringComparison.Ordinal)) s = s.Substring(2);
        else if (s.StartsWith("_", StringComparison.Ordinal)) s = s.Substring(1);
        if (s.StartsWith("Event", StringComparison.Ordinal) && s.Length > 5) s = s.Substring(5);
        if (s.EndsWith("Event", StringComparison.Ordinal) && s.Length > 5) s = s.Substring(0, s.Length - 5);
        return string.Equals(s, eventName, StringComparison.OrdinalIgnoreCase);
    }

    private static System.ComponentModel.EventHandlerList? GetEventHandlerList(object instance)
    {
        if (instance is not System.ComponentModel.Component) return null;
        var prop = typeof(System.ComponentModel.Component).GetProperty("Events",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return prop?.GetValue(instance) as System.ComponentModel.EventHandlerList;
    }

    /// <summary>
    /// 取 <paramref name="type"/> 在控件树中的实例（用于按 DFM 类型核对，如
    /// 「`TimerTimer` 绑的是 `System.Windows.Forms.Timer` 的 `Tick`」）。
    /// </summary>
    public static object? GetInstanceOf(System.Windows.Forms.Form form, Type type)
    {
        if (type.IsInstanceOfType(form)) return form;
        foreach (System.Windows.Forms.Control c in AllControls(form))
            if (type.IsInstanceOfType(c)) return c;
        return null;
    }

    /// <summary>枚举控件树全部控件（不含窗体自身）。</summary>
    public static IEnumerable<System.Windows.Forms.Control> AllControls(System.Windows.Forms.Control root)
    {
        foreach (System.Windows.Forms.Control c in root.Controls)
        {
            yield return c;
            foreach (var d in AllControls(c)) yield return d;
        }
    }

    /// <summary>在控件树里按**字段名**找控件（DFM 名 → 托管字段名逐字同名）。</summary>
    public static System.Windows.Forms.Control? FindByName(System.Windows.Forms.Form form, string dfmName)
        => AllControls(form).FirstOrDefault(c => c.Name == dfmName);
}

/// <summary>
/// `FindFirst/FindNext/FindClose` 的假实现（**不碰磁盘**）：按注册的文件名序列逐个返回。
/// <para>行为对齐 Delphi：`FindFirst` 命中第一条返回 0，否则返回非 0；
/// `FindNext` 还有下一条返回 0，否则返回非 0；每条记录带 `Attr`（目录 = $10）。</para>
/// </summary>
public sealed class Sweep9FormsFakeFileSearch : ISweep9FormsFileSearch
{
    /// <summary>按 `FindFirst` 收到的 searchPath 注册结果（键为完整 searchPath，含掩码）。</summary>
    public readonly Dictionary<string, List<Sweep9FormsSearchRec>> Table = new();

    /// <summary>`FindFirst` 调用次数。</summary>
    public int FindFirstCalls;

    /// <summary>`FindNext` 调用次数。</summary>
    public int FindNextCalls;

    /// <summary>`FindClose` 调用次数（用于断言"FindFirst 失败也照样 FindClose"）。</summary>
    public int FindCloseCalls;

    /// <summary>注册一个目录项。</summary>
    public void Add(string searchPath, string name, bool isDirectory = false)
    {
        if (!Table.TryGetValue(searchPath, out var list))
            Table[searchPath] = list = new List<Sweep9FormsSearchRec>();
        list.Add(new Sweep9FormsSearchRec
        {
            Name = name,
            Attr = isDirectory ? Sweep9FormsFileSearchDefault.faDirectory : 0,
        });
    }

    /// <inheritdoc />
    public int FindFirst(string searchPath, int attr, Sweep9FormsSearchRec info)
    {
        FindFirstCalls++;
        if (!Table.TryGetValue(searchPath, out var list) || list.Count == 0)
            return 1;
        info.Name = list[0].Name;
        info.Attr = list[0].Attr;
        _cursor[info] = (list, 1);
        return 0;
    }

    /// <inheritdoc />
    public int FindNext(Sweep9FormsSearchRec info)
    {
        FindNextCalls++;
        if (!_cursor.TryGetValue(info, out var cur)) return 1;
        var (list, i) = cur;
        if (i >= list.Count) return 1;
        info.Name = list[i].Name;
        info.Attr = list[i].Attr;
        _cursor[info] = (list, i + 1);
        return 0;
    }

    /// <inheritdoc />
    public void FindClose(Sweep9FormsSearchRec info)
    {
        FindCloseCalls++;
        _cursor.Remove(info);
    }

    private readonly Dictionary<Sweep9FormsSearchRec, (List<Sweep9FormsSearchRec> List, int Index)> _cursor = new();
}
