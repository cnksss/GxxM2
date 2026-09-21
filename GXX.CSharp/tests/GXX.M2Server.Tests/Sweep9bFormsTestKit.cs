using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace GXX.M2Server.Tests;

// p10-m2-misc 车道（M2Engine 侧）测试工具（前缀 Sweep9bForms* ⇒ 分区内）。
// 与 tests/GXX.DBServer.Tests/P10bTestKit.cs、tests/GXX.LoginSrv.Tests/P10bGateSetTestKit.cs 的
// 计数器同源 —— 三个测试工程之间没有公共测试库，故各留一份副本；改动必须同步。

/// <summary>
/// DFM 对账工具（§41.3-2：事件必须走 <c>Component.Events</c> + 声明类型上的**静态键**；
/// 用反射按同名私有委托字段找在 .NET 8 上一律得 0 = **假绿**）。
/// </summary>
public static class Sweep9bFormsRecon
{
    /// <summary>
    /// 已绑定的事件条目数；<paramref name="handlerOwner"/> 给定时只统计处理器声明在它身上的条目
    /// （WinForms 会给部分控件接**内部**处理器，实测 `RadioButton` 自带 2 条、`ComboBox` 1 条）。
    /// </summary>
    public static int CountBoundEvents(Component component, Type? handlerOwner = null)
    {
        EventHandlerList? list = GetEvents(component);
        int count = 0;
        if (list != null)
        {
            foreach (object key in EnumerateStaticKeys(component.GetType()))
            {
                Delegate? d = list[key];
                if (d == null) continue;
                foreach (Delegate one in d.GetInvocationList())
                {
                    Type? owner = one.Method.DeclaringType;
                    if (handlerOwner == null || (owner != null && handlerOwner.IsAssignableFrom(owner)))
                        count++;
                }
            }
        }
        return count;
    }

    /// <summary>窗体自身 + DFM 声明过的控件字段上的绑定数之和。</summary>
    public static int CountBoundEventsDeep(Component form)
    {
        int n = CountBoundEvents(form, form.GetType());
        if (form is Control)
        {
            foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
            {
                if (f.GetValue(form) is Control c) n += CountBoundEvents(c, form.GetType());
            }
        }
        return n;
    }

    /// <summary>绑定明细（失败信息用）。</summary>
    public static string DescribeBoundEventsDeep(Component form)
    {
        var lines = new List<string>();
        void Add(Component c, string prefix)
        {
            EventHandlerList? list = GetEvents(c);
            if (list == null) return;
            foreach (object key in EnumerateStaticKeys(c.GetType()))
            {
                Delegate? d = list[key];
                if (d == null) continue;
                foreach (Delegate one in d.GetInvocationList())
                    lines.Add(prefix + c.GetType().Name + " : " + (one.Method.DeclaringType?.Name ?? "?") + "." + one.Method.Name);
            }
        }
        Add(form, "");
        if (form is Control)
        {
            foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
            {
                if (f.GetValue(form) is Control c) Add(c, f.Name + ": ");
            }
        }
        return string.Join(" | ", lines);
    }

    private static EventHandlerList? GetEvents(Component component)
    {
        PropertyInfo? p = typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
        return p?.GetValue(component) as EventHandlerList;
    }

    private static IEnumerable<object> EnumerateStaticKeys(Type type)
    {
        var seen = new HashSet<string>();
        for (Type? t = type; t != null && t != typeof(object); t = t.BaseType)
        {
            foreach (FieldInfo f in t.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // 命名三形态：EventXxx / s_xxxEvent / EVENT_XXX ⇒ 必须大小写不敏感
                if (f.Name.IndexOf("event", StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (!typeof(object).IsAssignableFrom(f.FieldType)) continue;
                if (!seen.Add(t.FullName + "." + f.Name)) continue;
                object? key = null;
                try { key = f.GetValue(null); } catch { }
                if (key != null) yield return key;
            }
        }
    }

    /// <summary>窗体上声明的控件字段（public 实例字段，类型可赋给 <see cref="Control"/>），含继承来的。</summary>
    public static List<FieldInfo> DeclaredControlFields(Type formType)
    {
        var list = new List<FieldInfo>();
        foreach (FieldInfo f in formType.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            if (typeof(Control).IsAssignableFrom(f.FieldType)) list.Add(f);
        }
        return list;
    }

    /// <summary>声明控件字段名集合。</summary>
    public static string[] DeclaredControlNames(Type formType)
        => DeclaredControlFields(formType).ConvertAll(f => f.Name).ToArray();

    /// <summary>声明控件字段中已实例化的个数。</summary>
    public static int InstantiatedControlCount(object form)
    {
        int n = 0;
        foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
        {
            if (f.GetValue(form) is Control) n++;
        }
        return n;
    }

    /// <summary>递归可达控件中"属于窗体声明字段"的个数（不数复合控件的匿名内部子控件）。</summary>
    public static int ParentedDeclaredControlCount(Form form)
    {
        var declared = new HashSet<Control>();
        foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
        {
            if (f.GetValue(form) is Control c) declared.Add(c);
        }
        int Count(Control root)
        {
            int n = 0;
            foreach (Control c in root.Controls)
            {
                if (declared.Contains(c)) n++;
                n += Count(c);
            }
            return n;
        }
        return Count(form);
    }

    /// <summary>
    /// 全程序集里"名为 <paramref name="methodName"/> 的可调用成员"命中数（用于**否定性断言**的计数取证，§37.3）。
    /// </summary>
    public static int CountPublicStaticMethods(Assembly assembly, string methodName)
    {
        int n = 0;
        foreach (Type t in assembly.GetTypes())
        {
            foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (m.Name == methodName) n++;
            }
        }
        return n;
    }
}
