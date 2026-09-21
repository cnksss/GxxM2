using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using GXX.Core.Protocol;
using GXX.DBServer;

namespace GXX.DBServer.Tests;

// p10-m2-misc 车道的 DBServer 侧测试工具（前缀 P10b* ⇒ 分区内）。
// 内容：① 内存版 THumanDB（供 uFrmHumanExport 的两个导出处理器用）；
//      ② WinForms 事件绑定计数器（**必须**走 Component.Events + 静态键，见 §41.3-2）。

/// <summary>
/// 内存版 <see cref="THumanDBBase"/>（RoleDB.pas:117-209 的公开面）：
/// 只把本车道需要的两个 Do* 做成可编程，其余按初值实现（被调到就说明接线错了）。
/// </summary>
public sealed class P10bFakeHumanDb : THumanDBBase
{
    /// <summary>`SearchByLevel` 的结果集（预置）。</summary>
    public readonly List<TSerarchRoleData> Roles = new();

    /// <summary>`GetMobileNumbers` 的结果集（预置）。</summary>
    public readonly List<string> Mobiles = new();

    /// <summary>`SearchByLevel(LimitCount, MinLevel, _)` 的实参记录。</summary>
    public readonly List<(int LimitCount, int MinLevel)> SearchByLevelCalls = new();

    /// <summary>`GetMobileNumbers(OnlyBindMobile, _)` 的实参记录。</summary>
    public readonly List<bool> GetMobileNumbersCalls = new();

    protected override void OwnerLock() { }
    protected override void OwnerUnLock() { }

    protected override int DoSearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList)
    {
        SearchByLevelCalls.Add((LimitCount, MinLevel));
        foreach (var r in Roles) RoleList.Add(r);
        return Roles.Count;
    }

    protected override int DoGetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList)
    {
        GetMobileNumbersCalls.Add(OnlyBindMobile);
        MobileNumberList.AddRange(Mobiles);
        return Mobiles.Count;
    }

    // ---- 本车道不会调用的成员：按初值实现（不抛，避免测试噪音） ----

    protected override int DoGetID(string HumanName) => RoleDbConst.NO_ID;
    protected override bool DoCheckHumanExists(string Account, string HumanName) => false;
    protected override int DoGetHumanCount(string Account) => 0;
    protected override string DoGetOtherHumanName(string Account, string HumanName) => "";
    protected override bool DoGetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName)
    { HeroName = ""; DeputyHeroName = ""; return false; }
    protected override bool DoGetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin)
    { Sex = 0; Job = 0; Level = 0; LastLogin = 0; return false; }
    protected override int DoQueryHumans(string Account, TQueryHumanList HumanList) => 0;
    protected override int DoQueryDeleteHumans(string Account, TQueryHumanList HumanList) => 0;
    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => 0;
    protected override int DoSearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => 0;
    protected override bool DoSelect(string Account, string HumanName) => false;
    protected override bool DoGet(string Account, string HumanName, ref THumData HumData, out int HumanID) { HumanID = 0; return false; }
    protected override bool DoAdd(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair) => false;
    protected override bool DoDelete(string Account, string HumanName) => false;
    protected override bool DoDeleteRestore(string Account, string HumanName) => false;
    protected override bool DoSetEnabled(string Account, string HumanName, int Enabled) => false;
    protected override bool DoErase(string Account, string HumanName) => false;
    protected override bool DoRecordLoginTime(string Account, string HumanName) => false;
    protected override bool DoSave(int HumanID, ref THumData HumData) => false;
    protected override bool DoRename(string Account, string HumanName, int HumanID, string NewName) => false;
    protected override bool DoChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue)
    { ResultValue = 0; return false; }
    protected override bool DoChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue)
    { ResultValue = 0; return false; }
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList, TRoleRankList MasterRankList) { }
    protected override bool DoBuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName) => false;
}

/// <summary>
/// 写盘替身 <see cref="GXX.DBServer.Forms2.TSaveDialogSeam"/>：不弹窗，直接给出"用户选择"。
/// </summary>
public sealed class P10bFakeSaveDialog : GXX.DBServer.Forms2.TSaveDialogSeam
{
    /// <summary>Execute 的返回值（true = 用户点了保存）。</summary>
    public bool ExecuteResult = true;

    /// <summary>用户在对话框里选定的路径（Execute 成功后写回 <c>FileName</c>）。</summary>
    public string SelectedPath = "";

    /// <summary>Execute 被调用的次数。</summary>
    public int ExecuteCount;

    // ---- Execute 那一刻的对话框状态（原文在 Execute 之前设置这三项） ----
    public string TitleAtExecute = "";
    public string FilterAtExecute = "";
    public string FileNameAtExecute = "";

    public override bool Execute()
    {
        ExecuteCount++;
        TitleAtExecute = Title;
        FilterAtExecute = Filter;
        FileNameAtExecute = FileName;
        if (!ExecuteResult) return false;
        FileName = SelectedPath;
        return true;
    }
}

/// <summary>
/// WinForms 窗体/控件的 DFM 对账工具。
///
/// <para>
/// ★★ §41.3-2：`.NET 8 WinForms` 的事件**不是 field-like event** —— 事件委托挂在
/// <see cref="Component"/> 的 <c>Events</c>（<see cref="EventHandlerList"/>）上，
/// 键是声明类型上的**静态对象字段**（.NET 8 里叫 <c>s_xxxEvent</c>，也有 <c>EVENT_XXX</c>）。
/// 用反射按"同名私有委托字段"找会**一律得 0（假绿）**。
/// </para>
/// <para>
/// 本工具两条路都走：① 沿类型链收集"名字含 Event 的静态对象字段"作为键，去 <c>Events</c> 里查；
/// ② 若命中数为 0，再退回 field-like 私有委托字段。
/// </para>
/// </summary>
public static class P10bDfmRecon
{
    /// <summary>
    /// 已绑定的**事件条目**总数（每个静态键最多算 1；一次绑多个处理器按调用列表长度累加）。
    ///
    /// <para>
    /// <paramref name="handlerOwner"/> 给定时，只统计**处理器声明在它（或其派生类）上**的条目 ——
    /// DFM 的 `OnXxx = FormMethod` 全部落在窗体类上，而 WinForms 会给部分控件**自己**接内部处理器
    /// （实测：`RadioButton` 自带 2 条）⇒ 不筛就会把 2 数成 4。这比"按名字列例外表"更抗框架改名。
    /// </para>
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
        if (count == 0 && handlerOwner == null) count = CountFieldLikeEvents(component);
        return count;
    }

    /// <summary>逐键诊断（哪个静态键命中了几个处理器）；不参与断言，供排查"计数莫名变大"。</summary>
    public static List<string> DescribeBoundEvents(Component component)
    {
        var lines = new List<string>();
        EventHandlerList? list = GetEvents(component);
        if (list == null) return lines;
        foreach (object key in EnumerateStaticKeys(component.GetType()))
        {
            Delegate? d = list[key];
            if (d == null) continue;
            foreach (Delegate one in d.GetInvocationList())
            {
                lines.Add(component.GetType().Name + " → " + key.GetType().Name + "#" +
                    System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(key) + " : " +
                    (one.Method.DeclaringType?.Name ?? "?") + "." + one.Method.Name);
            }
        }
        return lines;
    }

    private static EventHandlerList? GetEvents(Component component)
    {
        PropertyInfo? p = typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
        return p?.GetValue(component) as EventHandlerList;
    }

    /// <summary>窗体 + 声明控件字段上的绑定明细（失败信息里带上它，便于判断多/少算在哪）。</summary>
    public static string DescribeBoundEventsDeep(Component form)
    {
        var lines = new List<string>(DescribeBoundEvents(form));
        if (form is Control)
        {
            foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
            {
                if (f.GetValue(form) is Control c)
                {
                    foreach (string s in DescribeBoundEvents(c)) lines.Add(f.Name + ": " + s);
                }
            }
        }
        return string.Join(" | ", lines);
    }

    /// <summary>
    /// 窗体自身 + **DFM 声明过的控件字段**（递归一层，不遍历 <c>Controls</c>）上的绑定数之和。
    ///
    /// <para>
    /// 为什么不直接递归 <c>Controls</c>：复合控件（`NumericUpDown` 等）自带**匿名内部子控件**，
    /// 它们会把自己的内部事件也算进来（p9-m2-forms 车道实测：29 个 DFM 控件被数成 51）⇒ 假红。
    /// DFM 的 `object` 节点都有名字、且都对应一个声明字段，故按"声明字段"集合遍历最贴近 DFM。
    /// </para>
    /// </summary>
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

    private static IEnumerable<object> EnumerateStaticKeys(Type type)
    {
        var seen = new HashSet<string>();
        for (Type? t = type; t != null && t != typeof(object); t = t.BaseType)
        {
            foreach (FieldInfo f in t.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // ★ 命名有三种形态：.NET Framework 的 `EventXxx`、.NET 8 的 `s_xxxEvent`、以及全大写的 `EVENT_XXX`
                //   ⇒ 必须**大小写不敏感**匹配 "event"，否则 `EVENT_SHOWN` 这类会被整批漏掉（实测：
                //   漏掉后 TFrmCreateChr 的 Shown 绑定数会数成 0 = 假绿）。
                if (f.Name.IndexOf("event", StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (!typeof(object).IsAssignableFrom(f.FieldType)) continue;
                if (!seen.Add(t.FullName + "." + f.Name)) continue;
                object? key = null;
                try { key = f.GetValue(null); } catch { /* 静态构造器可能还没跑 */ }
                if (key != null) yield return key;
            }
        }
    }

    private static int CountFieldLikeEvents(Component component)
    {
        int count = 0;
        for (Type? t = component.GetType(); t != null && t != typeof(object); t = t.BaseType)
        {
            foreach (FieldInfo f in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (!typeof(Delegate).IsAssignableFrom(f.FieldType)) continue;
                if (f.Name.IndexOf("event", StringComparison.OrdinalIgnoreCase) < 0 &&
                    !f.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase)) continue;
                if (f.GetValue(component) is Delegate d) count += d.GetInvocationList().Length;
            }
        }
        return count;
    }

    /// <summary>窗体上**声明**的控件字段数（public 实例字段，类型可赋给 <see cref="Control"/>）。</summary>
    public static List<FieldInfo> DeclaredControlFields(Type formType)
    {
        var list = new List<FieldInfo>();
        foreach (FieldInfo f in formType.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            if (typeof(Control).IsAssignableFrom(f.FieldType)) list.Add(f);
        }
        return list;
    }

    /// <summary>声明控件字段中**已实例化**（非 null）的个数。</summary>
    public static int InstantiatedControlCount(object form)
    {
        int n = 0;
        foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
        {
            if (f.GetValue(form) is Control) n++;
        }
        return n;
    }

    /// <summary>从窗体 <c>Controls</c> 递归可达的控件总数（对账"挂上去了没有"）。</summary>
    public static int ParentedControlCount(Control root)
    {
        int n = 0;
        foreach (Control c in root.Controls)
        {
            n++;
            n += ParentedControlCount(c);
        }
        return n;
    }

    /// <summary>
    /// 递归可达控件中**属于窗体声明字段**的个数。
    ///
    /// <para>
    /// 为什么不数全部递归控件：复合控件自带匿名内部子控件（`NumericUpDown` 有 `UpDownEdit` +
    /// `UpDownButtons`）⇒ 10 个 DFM 控件会被数成 14（p9-m2-forms 车道同类陷阱）。DFM 的 `object`
    /// 节点都对应一个声明字段，故以"声明字段实例"为集合做包含判断。
    /// </para>
    /// </summary>
    public static int ParentedDeclaredControlCount(Form form)
    {
        var declared = new HashSet<Control>();
        foreach (FieldInfo f in DeclaredControlFields(form.GetType()))
        {
            if (f.GetValue(form) is Control c) declared.Add(c);
        }
        return CountReachable(form, declared);
    }

    private static int CountReachable(Control root, HashSet<Control> declared)
    {
        int n = 0;
        foreach (Control c in root.Controls)
        {
            if (declared.Contains(c)) n++;
            n += CountReachable(c, declared);
        }
        return n;
    }
}
