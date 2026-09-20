// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  同源 DFM：Source/M2Engine/Forms/uFrmDummySetting.dfm（34,189 字节）
//  本文件：**接缝层**（控件最小面 + 无头 UI 开关）。
//
//  原文 uses（:5-7 / :212-213）：
//    · Vcl.Samples.Spin / SpinEditEx.pas（TSpinEditEx）→ 尚未移植 → TSpinEditExSeam
//    · M2Share.pas / Grobal2.pas / Envir.pas / M2Definition.pas / M2Threads.pas
//      → 见 DummySettingGlobals.cs 的说明（只取本单元依赖子集）
//
//  ★ 无头 UI 规程（任务书硬性要求 3）：
//    原文有 3 处 `Application.MessageBox('出生地图设置错误！', '错误信息',
//    MB_OK + MB_ICONERROR)`（:352 / :437 / :445）与 1 处 `ShowModal`（:223）。
//    模态调用在无头测试里**会挂死 testhost**，故：
//      · `MessageBox` 一律经 `FrmDummySetting.MessageBoxHandler` 接缝；
//      · `ShowModal` 一律经 `DummySettingMessageBoxSeam.UiEnabled` 开关。
//
//  ★ 命名空间隔离说明：`TSpinEditExSeam` / `TControlSeam` 一族在
//    `GXX.M2Server.Forms.CustomMagic`（车道 p5-m2-custommagic 的独占文件）已有一份。
//    本车道**不引用、不复用、也不修改**那个文件（跨车道只读），而是在本命名空间
//    内自带一份最小面 —— 命名空间不同 ⇒ 无 CS0102，且两车道互不阻塞。
//    待 M2Share.pas / SpinEditEx.pas 正式批次落地后由集成方统一收敛。
// ============================================================================

using System;
using System.Collections.Generic;

namespace GXX.M2Server.Forms.DummySetting;

// ---------------------------------------------------------------------------
// 无头 UI 开关
// ---------------------------------------------------------------------------

/// <summary>
/// `ShowFrmDummySetting`（原文 :217-227）里 `FrmDummySetting.ShowModal`（:223）的无头开关
/// （照搬 `GXX.RunGate.uFrmGameSpeedLogic.MessageBoxSeam.UiEnabled` 的做法）。
/// <para>
/// **`UiEnabled` 默认 true（生产行为）；单测必须置 false，否则真实模态循环挂死 testhost。**
/// </para>
/// </summary>
public static class DummySettingMessageBoxSeam
{
    /// <summary>true = 真实 `ShowModal`（生产）；false = 无头（测试），只返回替身结果。</summary>
    public static bool UiEnabled = true;

    /// <summary>`ShowModal` 返回值替身（`System.UITypes.TModalResult`）：mrOk=1 / mrCancel=2。</summary>
    public static Func<int>? ShowModalHandler;

    /// <summary>无头时累计的 `ShowModal` 次数（供断言"确实走了模态路径"）。</summary>
    public static int ShowModalCount;

    /// <summary>最近一次 `ShowModal` 的返回值（生产与无头都记录）。</summary>
    public static int LastModalResult;

    /// <summary>
    /// 复刻原文 :223 `FrmDummySetting.ShowModal`。
    /// 无头（`UiEnabled == false`）时**不**弹真实窗体，只走 `ShowModalHandler` 替身
    /// （未注入时返回 `mrCancel`，语义 = 用户直接关掉对话框）。
    /// </summary>
    public static int ShowModal(System.Windows.Forms.Form form)
    {
        ShowModalCount++;
        int result;
        if (ShowModalHandler != null)
            result = ShowModalHandler();
        else if (UiEnabled)
            result = (int)form.ShowDialog();
        else
            result = TModalResult.mrCancel;
        LastModalResult = result;
        return result;
    }
}

/// <summary>`System.UITypes.TModalResult` 的取值（`mrNone..mrNo`）。</summary>
public static class TModalResult
{
    /// <summary>mrNone。</summary>
    public const int mrNone = 0;
    /// <summary>mrOk。</summary>
    public const int mrOk = 1;
    /// <summary>mrCancel。</summary>
    public const int mrCancel = 2;
}

// ---------------------------------------------------------------------------
// TSpinEditEx 最小面（SpinEditEx.pas 尚未移植）
// ---------------------------------------------------------------------------

/// <summary>
/// `SpinEditEx.pas` `TSpinEditEx` 的托管替身 —— **只保留原文窗体真正用到的成员面**：
/// `Value`（:239/:248/… 读写）、`Enabled`（:299 `btnDummyLogon.Enabled` 用的是 TButton）。
/// <para>
/// 原文 `TSpinEditEx.Value` 的 `MinValue/MaxValue` 钳制（DFM 里 `MaxValue = 0` 表示不钳制，
/// e.g. `seDummyHPTime_Warrior` 的 DFM `MaxValue = 0`）本替身**不复刻**，理由：
/// 窗体全部写入点都是 `X.Value := g_Config.Y` 或 `g_Config.Y := X.Value` 的直接往返，
/// 钳制只影响用户键入手输，而手输路径属 DFM 装饰性行为（不改变本单元任何分支）。
/// 逐字保留写入顺序与目标即可。见并行报告「偏离登记 D-p8-01」。
/// </para>
/// </summary>
public class TSpinEditExSeam
{
    /// <summary>`property Value: Integer`。</summary>
    public int Value;

    /// <summary>DFM `MinValue`（仅记录，不参与运算）。</summary>
    public int MinValue;

    /// <summary>DFM `MaxValue`（0 = 不钳制；仅记录，不参与运算）。</summary>
    public int MaxValue;

    /// <summary>`property Enabled`。</summary>
    public bool Enabled = true;

    /// <summary>`property Hint`（DFM 提示文本；仅记录）。</summary>
    public string Hint = "";
}

// ---------------------------------------------------------------------------
// VCL 控件最小面（只保留窗体实际用到的成员）
// ---------------------------------------------------------------------------

/// <summary>VCL `TControl` 最小面。</summary>
public class TControlSeam
{
    /// <summary>`property Enabled`。</summary>
    public bool Enabled = true;

    /// <summary>`property Visible`。</summary>
    public bool Visible = true;

    /// <summary>`property Name`（无头断言用的身份标识）。</summary>
    public string Name = "";

    /// <summary>`procedure SetFocus`（无头不可断言 ⇒ 由窗体接缝 `SetFocusProbe` 镜像）。</summary>
    public void SetFocus()
    {
    }
}

/// <summary>VCL `TWinControl` 最小面。</summary>
public class TWinControlSeam : TControlSeam
{
}

/// <summary>VCL `TLabel` 最小面。</summary>
public class TLabelSeam : TControlSeam
{
    /// <summary>`property Caption`。</summary>
    public string Caption = "";
}

/// <summary>VCL `TBevel` 最小面。</summary>
public class TBevelSeam : TControlSeam
{
}

/// <summary>VCL `TGroupBox` 最小面。</summary>
public class TGroupBoxSeam : TWinControlSeam
{
    /// <summary>`property Caption`。</summary>
    public string Caption = "";
}

/// <summary>VCL `TTabSheet` 最小面。</summary>
public class TTabSheetSeam : TWinControlSeam
{
    /// <summary>`property Caption`。</summary>
    public string Caption = "";
}

/// <summary>
/// VCL `TPageControl` 最小面。
/// <para>
/// ★ `ActivePageIndex` 复刻 Delphi `TPageControl.SetActivePageIndex` 的**越界语义**：
/// 越界值静默 `-1`（与 §21.3 登记的 `TComboBox.ItemIndex` 同族陷阱；WinForms
/// `TabControl.SelectedIndex` 越界会抛 `ArgumentOutOfRangeException`）。
/// 原文只有一处写入（:298 `pgcMain.ActivePageIndex := 0`，恒在界内），但本垫片按
/// Delphi 语义实现以防后人扩展时踩坑。
/// </para>
/// </summary>
public class TPageControlSeam : TWinControlSeam
{
    private readonly List<TTabSheetSeam> _pages = new();

    /// <summary>页签集合（原文 DFM 静态 5 页：ts1..ts5）。</summary>
    public IReadOnlyList<TTabSheetSeam> Pages => _pages;

    /// <summary>添加页签（构造期用）。</summary>
    public void AddPage(TTabSheetSeam page) => _pages.Add(page);

    private int _activePageIndex = -1;

    /// <summary>`property ActivePageIndex`（越界静默置 -1，Delphi 语义）。</summary>
    public int ActivePageIndex
    {
        get => _activePageIndex;
        set => _activePageIndex = value >= 0 && value < _pages.Count ? value : -1;
    }
}

/// <summary>VCL `TButton` 最小面。</summary>
public class TButtonSeam : TWinControlSeam
{
    /// <summary>`property Caption`。</summary>
    public string Caption = "";
}

/// <summary>VCL `TCheckBox` 最小面。</summary>
public class TCheckBoxSeam : TWinControlSeam
{
    /// <summary>`property Caption`。</summary>
    public string Caption = "";

    /// <summary>`property Checked`。</summary>
    public bool Checked;
}

/// <summary>VCL `TEdit` 最小面。</summary>
public class TEditSeam : TWinControlSeam
{
    /// <summary>`property Text`。</summary>
    public string Text = "";
}

/// <summary>
/// VCL `TListBox` 最小面。
/// <para>
/// 复刻原文用到的全部成员：`Items`（`Clear/Add/AddStrings/Assign/Delete/IndexOf/Count/Strings[i]`）、
/// `ItemIndex`、`Selected[i]`、`MultiSelect`、`Sorted`、`DeleteSelected`。
/// </para>
/// <para>
/// ★ `Items[i]` 越界语义（§21.3）：Delphi `TStrings[i]` 越界抛 `EStringListError`
/// （非 `IndexOutOfRangeException`）。本替身按托管惯用做法抛 `ArgumentOutOfRangeException`，
/// 并**登记为偏离 D-p8-02**；原文全部读取点都先过 `ItemIndex >= 0` 或
/// `for I := 0 to Count - 1` 守卫，无一处依赖异常类型。
/// </para>
/// </summary>
public class TListBoxSeam : TWinControlSeam
{
    /// <summary>`property Items: TStrings`（保序 + Assign/AddStrings）。</summary>
    public List<string> Items { get; } = new();

    private int _itemIndex = -1;

    /// <summary>`property ItemIndex`（Delphi：越界静默 -1）。</summary>
    public int ItemIndex
    {
        get => _itemIndex;
        set => _itemIndex = value >= 0 && value < Items.Count ? value : -1;
    }

    /// <summary>`property MultiSelect`（DFM：`lstDummyList`/`lstMapList`/`lstMonList` = True）。</summary>
    public bool MultiSelect;

    /// <summary>`property Sorted`（DFM 中三个列表均未设 ⇒ False）。</summary>
    public bool Sorted;

    /// <summary>`function Selected[Index]: Boolean`（越界返回 False，Delphi 语义）。</summary>
    public bool Selected(int index) => index >= 0 && index < _selected.Count && _selected[index];

    private readonly List<bool> _selected = new();

    /// <summary>测试辅助：设置某一行的选中状态（生产由 VCL 消息驱动）。</summary>
    public void SetSelected(int index, bool value)
    {
        while (_selected.Count < Items.Count)
            _selected.Add(false);
        if (index >= 0 && index < _selected.Count)
            _selected[index] = value;
    }

    /// <summary>测试辅助：清空全部选中。</summary>
    public void ClearSelection()
    {
        for (int i = 0; i < _selected.Count; i++)
            _selected[i] = false;
    }

    /// <summary>`procedure DeleteSelected`（Delphi：删除全部选中行；无选中则无操作）。</summary>
    public void DeleteSelected()
    {
        while (_selected.Count < Items.Count)
            _selected.Add(false);
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (_selected[i])
            {
                Items.RemoveAt(i);
                _selected.RemoveAt(i);
            }
        }
        ItemIndex = -1;
    }

    /// <summary>`procedure Clear`（同时清选中）。</summary>
    public void Clear()
    {
        Items.Clear();
        _selected.Clear();
        _itemIndex = -1;
    }

    /// <summary>`procedure Add(const S: string): Integer`。</summary>
    public int Add(string s)
    {
        Items.Add(s);
        return Items.Count - 1;
    }

    /// <summary>`procedure AddStrings(const Strings: TStrings)`（追加，保序）。</summary>
    public void AddStrings(IEnumerable<string> strings)
    {
        foreach (var s in strings)
            Items.Add(s);
    }

    /// <summary>`procedure Assign(Source: TPersistent)`（原文 :320/:321 的 `Items.Assign`）。</summary>
    public void Assign(IEnumerable<string> strings)
    {
        Items.Clear();
        _selected.Clear();
        _itemIndex = -1;
        foreach (var s in strings)
            Items.Add(s);
    }

    /// <summary>`function IndexOf(const S: string): Integer`（未命中返回 -1）。</summary>
    public int IndexOf(string s) => Items.IndexOf(s);

    /// <summary>`procedure Delete(Index: Integer)`（Delphi 越界抛 `EListError`）。</summary>
    public void Delete(int index)
    {
        if (index < 0 || index >= Items.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "列表索引越界（Delphi: EListError）");
        Items.RemoveAt(index);
    }
}
