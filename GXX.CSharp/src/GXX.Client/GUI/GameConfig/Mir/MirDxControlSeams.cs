// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 覆盖范围（原文行号）：
//   43-558     TMirConfigDlg 类声明的 516 个 DxComponent 控件字段
//   3325-3845  MakeControlAddressList（516 条 AddObject 的控件名/地址表）
//   4535-5044  Initialize（含 279 条 `X.OnY := Handler` 事件绑定）
//
// 本文件承载 TMirConfigDlg 的**控件访问面接缝**。
//
// ★ 为什么用"真控件对象"而不是"展平的取值面属性"：
//   MirsConfigDlg/MirReturnConfigDlg 两个前序车道把 `PlugXxx.Checked` 展平成
//   `bool PlugXxx`（见 MirReturnControls.g.cs）。对那两单元成立，因为它们的配置逻辑
//   只读"取值"。**本单元的 279 条绑定里大量出现 `Sender` 判等**（如
//   `if Sender = PlugBtnUnbindItemDel then`、`else if Sender = ImageButton31 then`），
//   展平后无法表达"是哪个控件触发的"。因此本车道退回**最忠实的表达**：
//   每个控件就是原文那个控件类型的托管等价物（同名同类型的对象），
//   `Plug.PlugCheckBoxAutoOrderItem.Checked` 与原文逐字对应。
//
// ★ 与既有 DxComponent 的关系（不造第三份实现）：
//   仓库里已有 `GUI/DxComponent/DxControls.cs`（WinForms 派生）与
//   `LoadDx/DxControlSeams.cs`（另一套 TDxControl）—— 两套都**不可**用于无头测试，
//   且同名类型已导致过 CS0104 事故。故本文件把控件类型放在
//   `GXX.Client.GUI.GameConfig.Mir` 命名空间下，**原名保留**（TDxEdit/TDxComboBox/…），
//   它们是纯内存的测试/接缝替身，不继承 WinForms。报告 §偏离登记 有完整说明。
//
// 移植纪律：控件成员名与原文属性名逐字一致（`Checked`/`Value`/`ItemIndex`/`Text`/
//   `Lines`/`Caption`/`ItemIndex`/`OnClick`…）；事件成员的类型是**原始委托形状**
//   （`Action<object,int,int>` 等），赋值处与原文 `X.OnClick := H` 一一对应。

using System;
using System.Collections.Generic;

namespace GXX.Client.GUI.GameConfig.Mir;

// ================================================================================
// 原文 TDxControl 家族的公共取值面（各控件类型按 usage 精确切分，不多给一个成员）
// ================================================================================

/// <summary>
/// 接缝：原文 <c>TDxKeyEvent = procedure(Sender:TObject; var Key:Word; Shift:TShiftState) of object;</c>。
/// C# 不允许 <c>ref</c> 出现在泛型实参里，故用具名委托表达（形参与原文逐字对应）。
/// </summary>
public delegate void TMirDxKeyEvent(object Sender, ref ushort Key, Seams.DelphiShiftState Shift);

/// <summary>
/// 接缝：全部 DxComponent 控件的公共成员。
/// `Visible`/`Enabled`/`Tag`/`Top`/`Left`/`Width`/`Height`/`Name`/`SetFocus` 在原文里
/// 由 TDxControl 基类提供（DxComponents.pas），本单元 516 个控件都继承它。
/// **`OnMouseMove` 也在基类**（原文把 MouseMoveEvent 挂到 7 种不同类型的控件上：
/// PlugConfigDlg(TDxImageForm)/PlugScrollBox*（TDxChatMemo）/PlugPageControlConfig/
/// PlugTabSheet*，见 4609-4618 注释块与 DoInitAllComponentsMouseMove）。
/// </summary>
public abstract class TMirDxControl
{
    public bool Visible;
    public bool Enabled = true;
    public int Tag;
    public int Top;
    public int Left;
    public int Width;
    public int Height;
    public string Name = "";
    public string Hint = "";

    /// <summary>原文 <c>TDxControl.SetFocus</c>（原文 1221/1224/1416/1419/8275/8322 等）。</summary>
    public Action SetFocus = () => { };

    /// <summary>原文 <c>TDxControl.OnMouseMove</c>（签名 <c>(Sender:TObject; Shift:TShiftState; X,Y:Integer)</c>）。</summary>
    public Action<object, Seams.DelphiShiftState, int, int> OnMouseMove;
}

// ================================================================================
// 原文各控件类型（成员集合 = 该类型在本单元里被真正访问到的属性，逐条见 usage 表）
// ================================================================================

/// <summary>接缝：<c>TDxImageButton</c>（本单元 162 处 OnClick / 138 处 Checked —— 用量最大）。</summary>
public class TDxImageButton : TMirDxControl
{
    public bool Checked;
    public int Alignment;
    public int Align;
    public bool AutoSize;
    public string Caption = "";
    public TDxCaptionColor CaptionColor = new TDxCaptionColor();
    public int CaptionDownOffsetX;
    public int CaptionDownOffsetY;
    public int ClickCount;
    public bool Designing;
    public bool ExpandWidth;
    public int GuiType;
    public string ImageIndex = "";
    public int Style;
    public bool Transparent;
    public Action<object, int, int> OnClick;
    public Action<object> OnGetImage;
}

/// <summary>接缝：<c>TDxLabel</c>（本单元用 OnKeyDown/OnMouseDown 做快捷键重绑）。</summary>
public class TDxLabel : TMirDxControl
{
    public string Caption = "";
    public TDxCaptionColor CaptionColor = new TDxCaptionColor();
    public Action<object, int, int> OnClick;
    public TMirDxKeyEvent OnKeyDown;
    public Action<object, int, Seams.DelphiShiftState, int, int> OnMouseDown;
}

/// <summary>接缝：<c>TDxLine</c>（仅作分隔线，本单元只读 Top/Visible）。</summary>
public class TDxLine : TMirDxControl { }

/// <summary>接缝：<c>TDxEdit</c>（54 处 Value / 33 处 OnChange / 24 处 OnUnFocused）。</summary>
public class TDxEdit : TMirDxControl
{
    public int Value;
    public string Text = "";
    public Action<object> OnChange;
    public Action<object> OnUnFocused;
}

/// <summary>接缝：<c>TDxComboBox</c>（ItemIndex 越界是工程已知陷阱 §21.3）。</summary>
public class TDxComboBox : TMirDxControl
{
    public int ItemIndex = -1;
    public string Text = "";
    public readonly TStrings Items = new TStrings();
    public Action<object> OnSelect;
}

/// <summary>接缝：<c>TDxChatMemo</c>（行模型 + 虚拟矩形 + 滚动条）。</summary>
public class TDxChatMemo : TMirDxControl
{
    public int ItemIndex = -1;
    public readonly TStrings Lines = new TStrings();
    public int Position;
    public bool DrawSelect;
    public bool DrawBorder;
    public bool ShowScroll;
    public bool AutoScroll;
    public bool FontBackTransparent;
    public int ScrollBars;
    public TDxRect VirtualRect;
    public Action<object, int, int> OnClick;
    public Action<object, int, int> ScrollMouseDown;

    /// <summary>原文 <c>PlugMemoConfigNotes.LoadFromFile</c>（8046/8033）。</summary>
    public Action<string> LoadFromFile = _ => { };
}

/// <summary>接缝：<c>TDxScrollBox</c>（配置页容器；本单元只挂 OnMouseMove 与 Position）。</summary>
public class TDxScrollBox : TMirDxControl
{
    public int Position;
}

/// <summary>接缝：<c>TDxImageForm</c>（PlugConfigDlg / DOptFrm 两个窗体）。</summary>
public class TDxImageForm : TMirDxControl
{
    public bool Floating;
}

/// <summary>接缝：<c>TDxPageControl</c>。</summary>
public class TDxPageControl : TMirDxControl
{
    public int ActivePageIndex;
    public object ActivePage;
    public Action<object> OnActivePageChange;
    public Action<object, int, int, bool> OnInRealArea;
}

/// <summary>接缝：<c>TDxTabSheet</c>（本单元只读 TabVisible）。</summary>
public class TDxTabSheet : TMirDxControl
{
    public bool TabVisible = true;
}

/// <summary>接缝：<c>TDxTrackBar</c>（音量/地图缩放）。</summary>
public class TDxTrackBar : TMirDxControl
{
    public int Position;
    public int Min;
    public int Max;
    public Action<object> OnChangedPosition;
    public Action<object> OnChanggingPosition;
}

/// <summary>接缝：<c>TDxListView</c>（物品列表 / 挂机技能表）。</summary>
public class TDxListView : TMirDxControl
{
    public int ColCount;
    public int Count => Rows.Count;
    public int Position;
    public bool CanSelect;
    public object PopupMenu;
    public readonly List<string[]> Rows = new List<string[]>();
    public readonly TStrings Items = new TStrings();
    public Action<object, int, int, object, IntPtr> OnListItemClick;

    public Action Add = () => { };
    public Action Clear = () => { };
    public Action<int> Delete = _ => { };
    public Action Lock = () => { };
    public Action UnLock = () => { };
    public Action First = () => { };
    public Action Last = () => { };
}

/// <summary>接缝：<c>TDxPopupMenu</c>（物品页右键菜单）。</summary>
public class TDxPopupMenu : TMirDxControl
{
    public readonly TDxMenuItemCollection Items = new TDxMenuItemCollection();
    public Action<object, int, int> OnClick;
}

// ================================================================================
// 小类型：与原文同名（TDxCaptionColor / TStrings / TDxRect / TDxMenuItemCollection）
// ================================================================================

/// <summary>接缝：<c>TDxCaptionColor</c>（原文 <c>PlugLabelSpecialColor.CaptionColor.Up.Color</c>）。</summary>
public sealed class TDxCaptionColor
{
    public TDxColorState Up = new TDxColorState();
    public TDxColorState Down = new TDxColorState();
}

public sealed class TDxColorState
{
    public int Color;
}

/// <summary>接缝：原文 <c>TStrings</c>（Items / Lines 两个行集合共用）。</summary>
public sealed class TStrings
{
    private readonly List<string> _items = new List<string>();
    private readonly List<object> _objects = new List<object>();

    public int Count => _items.Count;
    public string this[int index] => index >= 0 && index < _items.Count ? _items[index] : "";
    public List<string> Strings => _items;
    public List<object> Objects => _objects;

    /// <summary>原文 <c>Items.Text</c>（一次性按换行赋值，1367-1368）。</summary>
    public string Text
    {
        get => string.Join("\r\n", _items);
        set
        {
            _items.Clear();
            _objects.Clear();
            if (string.IsNullOrEmpty(value)) return;
            foreach (var line in value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
                _items.Add(line);
        }
    }

    public void Clear() { _items.Clear(); _objects.Clear(); }
    public void Add(string s) { _items.Add(s); _objects.Add(null); }
    public void AddObject(string s, object o) { _items.Add(s); _objects.Add(o); }

    /// <summary>原文 <c>AddObject(s, TObject(p))</c> 的强类型形式（本单元 522 处 AddObject 之一）。</summary>
    public void AddObject(string s, int dummy) { _items.Add(s); _objects.Add(dummy); }

    public int IndexOf(string s) => _items.IndexOf(s);
    public void Delete(int index) { if (index >= 0 && index < _items.Count) { _items.RemoveAt(index); _objects.RemoveAt(index); } }
    public void Assign(TStrings src) { _items.Clear(); _objects.Clear(); _items.AddRange(src._items); _objects.AddRange(src._objects); }
    public void SaveToFile(string fileName) => MirConfigGlobalSeam.SaveTextFile(fileName, _items);
    public object GetObject(int index) => index >= 0 && index < _objects.Count ? _objects[index] : null;
}

/// <summary>接缝：<c>TDxRect</c>（原文 8061 <c>vtRect := PlugMemoConfigNotes.VirtualRect;</c>）。</summary>
public struct TDxRect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
    public int Width => Right - Left;
    public int Height => Bottom - Top;
}

/// <summary>接缝：<c>TDxPopupMenu.Items</c> 的按项 Enabled 开关（原文 5015-5028）。</summary>
public sealed class TDxMenuItemCollection
{
    private readonly List<bool> _enabled = new List<bool>();
    public TDxMenuItemEnabled Enabled
    {
        get
        {
            while (_enabled.Count < 16) _enabled.Add(true);
            return new TDxMenuItemEnabled(_enabled);
        }
    }
}

public sealed class TDxMenuItemEnabled
{
    private readonly List<bool> _list;
    public TDxMenuItemEnabled(List<bool> list) { _list = list; }
    public bool this[int index]
    {
        get { while (_list.Count <= index) _list.Add(true); return _list[index]; }
        set { while (_list.Count <= index) _list.Add(true); _list[index] = value; }
    }
}
