// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本文件覆盖（原文行号）：
//   43-558     516 个控件字段的**类型面**（属性声明 + 实例化 → MirConfigDlgControls.g.cs 生成）
//   4621-5044  Initialize 里的 279 条 `X.OnY := Handler` 绑定（本文件 = 绑定表 + 挂接）
//
// 本文件手写"生成器无法表达"的部分：
//   TMirConfigDlgEventBinding     一条绑定的三元组（控件名 / 事件名 / 处理器名）
//   TMirConfigDlgEventBindings    原文绑定的常量与区间（对账用）
//   BindingsData.All              279 条绑定（由 Gen/gen-mir-bindings.ps1 从原文逐行提取）
//   TMirConfigDlgControls.BindEvents  按原文次序把 279 条挂到 TMirConfigDlg 的派发器上
//
// ★ 事件成员为什么是**控件对象 + 处理器名派发**而不是展平的 `bool PlugXxx`：
//   原文 279 条绑定里大量出现 `Sender` 判等（`if Sender = PlugBtnUnbindItemDel`、
//   `else if Sender = ImageButton31` 等），必须保留"哪个控件触发"的信息。
//   故每个控件就是原文那个控件类型的托管等价物（见 MirDxControlSeams.cs），
//   绑定后由 <see cref="TMirConfigDlg.DispatchClick"/> 按**处理器名**转回实例方法
//   —— 这与原文 `X.OnClick := CheckBoxClickEx` 是同一套语义，且处理器名可被测试断言。
//
// ★ 委托签名逐字对应原文事件类型：
//     TDxMouseEvent    (Sender:TObject; X,Y:Integer)             -> Action<object,int,int>
//     TNotifyEvent     (Sender:TObject)                         -> Action<object>
//     TDxKeyEvent      (Sender; var Key:Word; Shift)            -> Action<object,ref ushort,DelphiShiftState>
//     TDxMouseDownEvt  (Sender; Button; Shift; X,Y)             -> Action<object,int,DelphiShiftState,int,int>
//     TDxListItemClick (Sender; ARow,ACol; ListItem; ViewItem)  -> Action<object,int,int,object,IntPtr>
//     TDxInRealAreaEvt (Sender; X,Y; var IsRealArea)            -> Action<object,int,int,bool>

using System;
using System.Collections.Generic;

namespace GXX.Client.GUI.GameConfig.Mir;

/// <summary>
/// 接缝：TMirConfigDlg 的控件访问面。
/// 516 个控件属性由 <c>MirConfigDlgControls.g.cs</c> 生成（每个 = 一个只读属性 + 构造时 new）。
/// 本接口只是它们的类型合同；事件绑定见 <see cref="TMirConfigDlgControls.BindEvents"/>。
/// </summary>
public interface IMirConfigDlgControls
{
}

/// <summary>
/// 279 条 <c>X.OnY := Handler</c> 绑定的**机器可读对账表**（原文 4621-5044）。
/// 由 <c>Gen/gen-mir-bindings.ps1</c> 从原文逐行提取，行号即原文行号。
/// </summary>
public sealed class TMirConfigDlgEventBinding
{
    /// <summary>原文行号。</summary>
    public int Line;
    /// <summary>控件字段名。</summary>
    public string Ctrl = "";
    /// <summary>事件名（<c>OnClick</c>/<c>OnChange</c>/…）。</summary>
    public string Event = "";
    /// <summary>处理器方法名（原文 <c>TMirConfigDlg</c> 的成员）。</summary>
    public string Handler = "";
}

/// <summary>绑定表的静态形态：计数与区间（不持有可变状态）。</summary>
public static class TMirConfigDlgEventBindings
{
    /// <summary>原文 4621-5044 里的 <c>X.OnY := ...</c> 绑定总数（逐行提取，含重复绑定）。</summary>
    public const int TotalBindings = 279;

    /// <summary>原文绑定块的行号区间（含）。</summary>
    public const int FirstLine = 4621;
    public const int LastLine = 5044;
}

/// <summary>
/// 接缝：TMirConfigDlg 的控件树 + 事件绑定面的**纯内存默认实现**。
/// 516 个控件实例由生成的 <c>MirConfigDlgControls.g.cs</c> 构造；
/// 本文件负责把它们按原文 4621-5044 的次序绑定到 <see cref="TMirConfigDlg"/> 的处理器上。
/// </summary>
public partial class TMirConfigDlgControls : IMirConfigDlgControls
{
    /// <summary>
    /// 按原文 4621-5044 逐条绑定（279 条）。
    /// ★ **必须与原文条数相等**（<see cref="TMirConfigDlgEventBindings.TotalBindings"/>）；
    ///   绑定顺序也照原文，便于把"某条绑定漏了"定位到原文行号。
    /// </summary>
    public void BindEvents(TMirConfigDlg dlg)
    {
        foreach (var b in TMirConfigDlgEventBindings_Data.All)
        {
            var ctrl = Find(b.Ctrl);
            if (ctrl == null) continue;   // 绑定表里出现了未声明的控件名 → 由对账测试抓出
            switch (b.Event)
            {
                case "OnClick":
                    if (ctrl is TDxImageButton ib) ib.OnClick = (s, x, y) => dlg.DispatchClick(b.Handler, s, x, y);
                    else if (ctrl is TDxLabel lb) lb.OnClick = (s, x, y) => dlg.DispatchClick(b.Handler, s, x, y);
                    else if (ctrl is TDxChatMemo cm) cm.OnClick = (s, x, y) => dlg.DispatchClick(b.Handler, s, x, y);
                    else if (ctrl is TDxPopupMenu pm) pm.OnClick = (s, x, y) => dlg.DispatchClick(b.Handler, s, x, y);
                    break;
                case "OnChange":
                    if (ctrl is TDxEdit e1) e1.OnChange = s => dlg.DispatchNotify(b.Handler, s);
                    break;
                case "OnUnFocused":
                    if (ctrl is TDxEdit e2) e2.OnUnFocused = s => dlg.DispatchNotify(b.Handler, s);
                    break;
                case "OnSelect":
                    if (ctrl is TDxComboBox cb) cb.OnSelect = s => dlg.DispatchNotify(b.Handler, s);
                    break;
                case "OnMouseMove":
                    ctrl.OnMouseMove = (s, sh, x, y) => dlg.DispatchMouseMove(b.Handler, s, sh, x, y);
                    break;
                case "OnMouseDown":
                    if (ctrl is TDxLabel l2) l2.OnMouseDown = (s, btn, sh, x, y) => dlg.DispatchMouseDown(b.Handler, s, btn, sh, x, y);
                    break;
                case "OnKeyDown":
                    if (ctrl is TDxLabel l3) l3.OnKeyDown = (object s, ref ushort k, Seams.DelphiShiftState sh) => dlg.DispatchKeyDown(b.Handler, s, ref k, sh);
                    break;
                case "OnListItemClick":
                    if (ctrl is TDxListView v) v.OnListItemClick = (s, r, col, item, view) => dlg.DispatchListItemClick(b.Handler, s, r, col, item, view);
                    break;
                case "OnActivePageChange":
                    if (ctrl is TDxPageControl p1) p1.OnActivePageChange = s => dlg.DispatchNotify(b.Handler, s);
                    break;
                case "OnChangedPosition":
                    if (ctrl is TDxTrackBar t1) t1.OnChangedPosition = s => dlg.DispatchNotify(b.Handler, s);
                    break;
                case "OnChanggingPosition":
                    if (ctrl is TDxTrackBar t2) t2.OnChanggingPosition = s => dlg.DispatchNotify(b.Handler, s);
                    break;
                case "OnInRealArea":
                    if (ctrl is TDxPageControl p2) p2.OnInRealArea = (s, x, y, r) => dlg.DispatchInRealArea(b.Handler, s, x, y, r);
                    break;
                case "OnGetImage":
                    // 原文 2 处（DOptBtnSKiilIcon/DOptBtnSkillLine）：DxImageButton 的**绘制**回调，
                    // TMirConfigDlg 侧无处理器方法（原文赋的是控件自身的 GetImage），无业务分支。
                    break;
                default:
                    break;
            }
        }
    }

    private readonly Dictionary<string, TMirDxControl> _byName = new Dictionary<string, TMirDxControl>(StringComparer.Ordinal);

    /// <summary>把生成的 516 个控件登记进名字表（构造函数调用）。</summary>
    private void Register(string name, TMirDxControl c) => _byName[name] = c;

    /// <summary>按名字取控件（绑定与测试查表用）。名字与原文控件字段名逐字一致。</summary>
    public TMirDxControl Find(string name) => _byName.TryGetValue(name, out var c) ? c : null;

    /// <summary>已登记的控件数（对账：必须 = 516）。</summary>
    public int RegisteredCount => _byName.Count;
}

/// <summary>
/// 接缝：<c>X.OnY := H</c> 里 <c>H</c> 不是 <c>TMirConfigDlg</c> 成员的例外。
/// 原文有 2 处赋给控件自身的绘制回调（4621-5044 区间外/内各有一处），已在
/// <see cref="TMirConfigDlgControls.BindEvents"/> 的 <c>OnGetImage</c> 分支登记。
/// </summary>
public static class TMirConfigDlgBindingExceptions
{
    /// <summary>原文 4610 附近的 <c>PlugConfigDlg.OnMouseMove := MouseMoveEvent;</c> 属 protected 成员。</summary>
    public const string Note = "见 并行报告-p10-client-mirconfig.md §事件对账表";
}
