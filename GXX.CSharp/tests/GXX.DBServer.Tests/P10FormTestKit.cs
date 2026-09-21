// ============================================================================
// 车道 p10-db-login-forms —— 窗体 **DFM 对账取证** 工具
// （《并行派发台账》§37.3：否定性断言必须计数取证；§41.3：.NET 8 WinForms 事件
//   不是 field-like event，用反射数私有委托字段会**一律得 0（假绿）**。）
//
// 本文件在 GXX.LoginSrv.Tests / GXX.GameCenter.Tests / GXX.DBServer.Tests 三个
// 测试工程里**各有一份同源副本**（分区只允许 tests/<工程>/P10*，无法跨工程共享）。
// 修改时请三份同步。
//
// 计数口径（两条实测结论，来自 p9-m2-forms 车道的固化成果）：
//  1. 只数**有名字**的控件：WinForms 复合控件自带匿名内部子控件
//     （DataGridView 内部 2 个 ScrollBar、TabControl 内部 UpDown），
//     DFM 的 `object` 节点**全部**有名字 ⇒ "非空名字"才与 DFM 对齐。
//  2. 事件绑定必须走 `Component.Events`（EventHandlerList）+ 声明类型上的**静态键**
//     （.NET Framework 叫 `EventXxx`、.NET 8 叫 `s_xxxEvent`、另一些是 `EVENT_XXX`），
//     同时保留 field-like 私有委托字段这条路。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GXX.DBServer.Forms.Tests;

/// <summary>DFM 对账工具。</summary>
public static class P10FormReconcile
{
    /// <summary>递归计数控件树（**含**窗体自身 = DFM `object Form: TForm` 根节点口径）。</summary>
    public static int CountDfmObjects(System.Windows.Forms.Form form) => 1 + CountChildren(form);

    /// <summary>递归计数控件树（**不含**窗体自身）。</summary>
    public static int CountChildrenOf(System.Windows.Forms.Form form) => CountChildren(form);

    private static int CountChildren(System.Windows.Forms.Control parent)
    {
        int n = 0;
        foreach (System.Windows.Forms.Control c in parent.Controls)
        {
            if (c.Name.Length > 0) n += 1 + CountChildren(c);
        }
        return n;
    }

    /// <summary>有名字（= DFM 节点）控件的 (名称, 类型)，含窗体自身，深度优先。</summary>
    public static List<(string Name, Type Type)> EnumerateDfmObjects(System.Windows.Forms.Form form)
    {
        var list = new List<(string, Type)> { (form.Name.Length > 0 ? form.Name : form.GetType().Name, form.GetType()) };
        Walk(form, list);
        return list;
    }

    private static void Walk(System.Windows.Forms.Control parent, List<(string, Type)> list)
    {
        foreach (System.Windows.Forms.Control c in parent.Controls)
        {
            if (c.Name.Length == 0) continue;
            list.Add((c.Name, c.GetType()));
            Walk(c, list);
        }
    }

    /// <summary>控件树里**有名字**的控件（不含窗体自身）。</summary>
    public static IEnumerable<System.Windows.Forms.Control> DfmControls(System.Windows.Forms.Control root)
        => AllControls(root).Where(c => c.Name.Length > 0);

    /// <summary>全部控件（含匿名内部子控件）。</summary>
    public static IEnumerable<System.Windows.Forms.Control> AllControls(System.Windows.Forms.Control root)
    {
        foreach (System.Windows.Forms.Control c in root.Controls)
        {
            yield return c;
            foreach (var d in AllControls(c)) yield return d;
        }
    }

    /// <summary>按 DFM 名找控件（DFM 名 → 托管控件字段名逐字同名）。</summary>
    public static System.Windows.Forms.Control? FindByName(System.Windows.Forms.Form form, string dfmName)
        => DfmControls(form).FirstOrDefault(c => c.Name == dfmName);

    /// <summary>窗体自身 + 全部有名字控件上**已挂接**的事件总数。</summary>
    public static int CountEventBindings(System.Windows.Forms.Form form)
    {
        int n = CountEventBindingsOn(form);
        foreach (var c in DfmControls(form)) n += CountEventBindingsOn(c);
        return n;
    }

    /// <summary>单个控件/组件上已挂接的事件数。</summary>
    public static int CountEventBindingsOn(System.ComponentModel.Component component)
        => CountEventBindingsOn(component.GetType(), component);

    /// <summary>`xxx.Event += handler` 是否已挂接。</summary>
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

    /// <summary>
    /// 实测已知的"事件名 ↔ 承载键/字段名**不同源**"例外表（.NET 实现细节）。
    /// <list type="bullet">
    /// <item>`System.Windows.Forms.Timer.Tick` 由**实例委托字段**承载，字段名是 `onTimer`；</item>
    /// <item>★ `Control.TextChanged` 由**静态键**承载，键名是 `s_textEvent`（归一化后得到 `text`，
    /// 与事件名 `TextChanged` 无法机械对齐）—— **本条由 p10 车道 uFrmRoleDataEdit 子车道实测发现
    /// （跨区项 B-P10-21）**：修复前 <see cref="TryKeyed"/> 不查本表 ⇒ 该事件**恒计 0（假绿）**，
    /// 实测 `uFrmRoleDataEdit` 的 DFM 35 条绑定被数成 31 条。现已让
    /// <see cref="TryFieldLike"/> 与 <see cref="TryKeyed"/> **两条路都查本表**。</item>
    /// </list>
    /// **只登记实测确认过的条目，不猜。**
    /// </summary>
    private static readonly Dictionary<(Type Type, string Event), string[]> KnownBackingFieldAliases = new()
    {
        [(typeof(System.Windows.Forms.Timer), "Tick")] = new[] { "onTimer", "_onTimer", "s_onTimer" },
        [(typeof(System.Windows.Forms.Control), "TextChanged")] = new[] { "s_textEvent", "EventText" },
    };

    private static bool TryFieldLike(Type declaringType, object instance, string eventName)
    {
        KnownBackingFieldAliases.TryGetValue((declaringType, eventName), out var aliases);
        foreach (var f in declaringType.GetFields(
                     BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            if (!typeof(Delegate).IsAssignableFrom(f.FieldType)) continue;
            bool nameMatches = IsKeyFieldNameFor(f.Name, eventName)
                || (aliases != null && aliases.Contains(f.Name, StringComparer.Ordinal));
            if (!nameMatches) continue;
            if (f.GetValue(instance) != null) return true;
        }
        return false;
    }

    private static bool TryKeyed(System.ComponentModel.EventHandlerList? events, Type declaringType, string eventName)
    {
        if (events == null) return false;
        // ★ 静态键这条路**也要**查别名表（B-P10-21）：`Control.TextChanged` 的键名是 `s_textEvent`，
        //   归一化得 `text` ≠ `TextChanged` ⇒ 不查别名就会恒计 0（假绿）。
        KnownBackingFieldAliases.TryGetValue((declaringType, eventName), out var aliases);
        foreach (var keyField in declaringType.GetFields(
                     BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            if (keyField.FieldType != typeof(object) && keyField.FieldType != typeof(int)) continue;
            bool nameMatches = IsKeyFieldNameFor(keyField.Name, eventName)
                || (aliases != null && aliases.Contains(keyField.Name, StringComparer.Ordinal));
            if (!nameMatches) continue;
            object? key = keyField.GetValue(null);
            if (key != null && events[key] != null) return true;
        }
        return false;
    }

    /// <summary>静态键命名归一化（`s_doubleClickEvent` / `EventDoubleClick` / `EVENT_LOAD`）。</summary>
    private static bool IsKeyFieldNameFor(string fieldName, string eventName)
    {
        string s = NormalizeKeyName(fieldName);
        if (string.Equals(s, eventName, StringComparison.OrdinalIgnoreCase)) return true;
        if (s.Length > 2 && s.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            return string.Equals(s.Substring(2), eventName, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    private static string NormalizeKeyName(string fieldName)
    {
        string s = fieldName;
        if (s.StartsWith("s_", StringComparison.Ordinal)) s = s.Substring(2);
        s = s.Trim('_');
        while (s.Length > 5 && s.StartsWith("Event", StringComparison.OrdinalIgnoreCase)) s = s.Substring(5);
        while (s.Length > 5 && s.EndsWith("Event", StringComparison.OrdinalIgnoreCase)) s = s.Substring(0, s.Length - 5);
        return s.Trim('_');
    }

    private static System.ComponentModel.EventHandlerList? GetEventHandlerList(object instance)
    {
        if (instance is not System.ComponentModel.Component) return null;
        var prop = typeof(System.ComponentModel.Component).GetProperty("Events",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return prop?.GetValue(instance) as System.ComponentModel.EventHandlerList;
    }
}
