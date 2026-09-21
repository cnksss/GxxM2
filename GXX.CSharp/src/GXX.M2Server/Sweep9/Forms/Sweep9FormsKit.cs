// ============================================================================
// 车道 p9-m2-forms（M2Engine 窗体/杂项族 7 单元）
// 本文件：本车道共用的**垫片/工具**（不含任何被移植单元的语义）
//
//  · Sweep9FormsMessageBoxSeam —— ShowModal 无头开关（照 p8-m2-dummysetting /
//    p8-m2-itemprop-misc 已验证形态）。**UiEnabled 默认 true = 生产；测试必须置 false**，
//    否则真实模态循环挂死 testhost。
//  · Sweep9FormsKit —— Delphi 语义垫片：CompareText（大小写不敏感，非区域敏感）、
//    TComboBox/TListBox.ItemIndex 的**静默越界**语义（台账 §21.3）、TSpinEditEx → NumericUpDown。
//  · Sweep9Memo / Sweep9MemoLines —— TMemo.Lines 的等价物：**Text 与 Lines 同源**
//    （单一存储 = GXX.Core.Util.TStringList），故 `Lines[i] := x` 与 `.Text` 永不发散。
//
// ★ 命名纪律：本文件新增类型名一律带 `Sweep9Forms` / `Sweep9` 前缀，
//   开工前已对全树 `git grep` 核验 0 命中（见 docs\并行报告-p9-m2-forms.md §0.4）。
// ============================================================================

using GXX.Core.Util;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// `ShowModal`（本车道 5 个窗体各自 `Open` 里的最后一句话）的无头开关。
/// </summary>
public static class Sweep9FormsMessageBoxSeam
{
    /// <summary>true = 真实 `ShowDialog`（生产）；false = 无头（测试），不弹真窗。</summary>
    public static bool UiEnabled = true;

    /// <summary>累计 `ShowModal` 次数（供断言"确实走到模态路径"）。</summary>
    public static int ShowModalCount;

    /// <summary>最近一次模态返回值（Delphi `TModalResult`：mrOk=1 / mrCancel=2）。</summary>
    public static int LastModalResult;

    /// <summary>替身返回值注入（优先于 UiEnabled）。</summary>
    public static Func<int>? ShowModalHandler;

    /// <summary>
    /// 复刻 Delphi `Form.ShowModal`。原文各处返回值均被丢弃，故返回 int 只为可观测。
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
            result = 2;   // mrCancel：语义 = 无头环境等价于"用户直接关掉对话框"
        LastModalResult = result;
        return result;
    }

    /// <summary>测试隔离：复位全部静态量。</summary>
    public static void Reset()
    {
        UiEnabled = true;
        ShowModalCount = 0;
        LastModalResult = 0;
        ShowModalHandler = null;
    }
}

/// <summary>Delphi 语义垫片（只含被移植单元真正用到的几条）。</summary>
public static class Sweep9FormsKit
{
    /// <summary>原文 `System.sLineBreak`（Delphi = `#13#10`，Windows 目标）。</summary>
    public const string sLineBreak = "\r\n";

    /// <summary>
    /// 原文 `SysUtils.CompareText(S1, S2)`（Delphi 7 实现是**ASCII 大写表**逐字节比较，
    /// 非区域敏感）⇒ 托管侧用 <see cref="StringComparison.OrdinalIgnoreCase"/>（ASCII 上等价）。
    /// </summary>
    public static int CompareText(string a, string b)
        => string.Compare(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 复刻 Delphi `TComboBox.ItemIndex := V` 的**静默容忍**语义（《并行派发台账》§21.3）：
    /// 越界（&lt; -1 或 &gt;= Items.Count）时取值 **-1**，**不抛**。
    /// WinForms 的 `SelectedIndex` 对越界值抛 <see cref="ArgumentOutOfRangeException"/>。
    /// </summary>
    public static void SetComboBoxItemIndex(System.Windows.Forms.ComboBox combo, int value)
    {
        if (value < -1 || value >= combo.Items.Count)
        {
            combo.SelectedIndex = -1;      // Delphi：静默置 -1
            return;
        }
        combo.SelectedIndex = value;
    }

    /// <summary>同上，用于 `TListBox.ItemIndex`。</summary>
    public static void SetListBoxItemIndex(System.Windows.Forms.ListBox listBox, int value)
    {
        if (value < -1 || value >= listBox.Items.Count)
        {
            listBox.SelectedIndex = -1;
            return;
        }
        listBox.SelectedIndex = value;
    }

    /// <summary>
    /// `TSpinEditEx`（`SpinEditEx.pas`，尚未移植）→ WinForms <see cref="System.Windows.Forms.NumericUpDown"/>。
    /// <para>
    /// DFM 里 `MaxValue = 0` 的语义是**不钳制**（该控件在 Delphi 侧 0 表示"无上限"），
    /// 故映射为 <see cref="int.MaxValue"/>（照 p8-m2-dummysetting 车道已验证的做法）。
    /// </para>
    /// </summary>
    public static System.Windows.Forms.NumericUpDown MakeSpin(int left, int top, int width,
        int minValue, int maxValue, int value, int height = 21)
    {
        var spin = new System.Windows.Forms.NumericUpDown
        {
            Left = left,
            Top = top,
            Width = width,
            Height = height,
            Minimum = minValue,
            Maximum = maxValue == 0 ? int.MaxValue : maxValue,
        };
        // ★ 先设 Value 再设 Minimum/Maximum 会在 WinForms 里被夹取；这里按
        //   "先边界后取值"的顺序写，与 Delphi `MinValue/MaxValue/Value` 的 DFM 语义一致。
        if (value < spin.Minimum) value = (int)spin.Minimum;
        if (value > spin.Maximum) value = (int)spin.Maximum;
        spin.Value = value;
        return spin;
    }
}

/// <summary>
/// Delphi `TMemo` 的等价物：<b>`Text` 与 `Lines` 共用同一份存储</b>
/// （存储是既有 <see cref="TStringList"/>，即已 1:1 移植的 `TStrings`）。
/// 这样 `Lines[i] := x` / `Lines.Add` / `Lines.Clear` / `.Text` 之间永不发散。
/// </summary>
public sealed class Sweep9Memo : System.Windows.Forms.TextBox
{
    private readonly Sweep9MemoLines _lines = new();

    /// <summary>原文 `TMemo.Lines`（`TStrings`）。</summary>
    public new Sweep9MemoLines Lines => _lines;

    /// <summary>Delphi `TMemo.Text` == `Lines.Text`（同一份存储 ⇒ 读写都走 Lines）。</summary>
    public override string Text
    {
        get => _lines.Text;
        set => _lines.Text = value ?? "";
    }
}

/// <summary>`TMemo.Lines` 的最小面（`TStrings` 子集：本车道 7 单元实际用到的成员）。</summary>
public sealed class Sweep9MemoLines
{
    private readonly TStringList _list = new();

    /// <summary>`TStrings.Count`。</summary>
    public int Count => _list.Count;

    /// <summary>`TStrings.Strings[Index]`（越界行为同 Delphi：抛）。</summary>
    public string this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    /// <summary>`TStrings.Text`（Delphi 语义由既有 <see cref="TStringList"/> 保证）。</summary>
    public string Text
    {
        get => _list.Text;
        set => _list.Text = value ?? "";
    }

    /// <summary>`TStrings.Add`。</summary>
    public void Add(string s) => _list.Add(s ?? "");

    /// <summary>`TStrings.Clear`。</summary>
    public void Clear() => _list.Clear();

    /// <summary>`TStrings.GetTextStr`（Delphi：**每行后**都补 LineBreak，含末行）。</summary>
    public string GetTextStr() => _list.GetTextStr();

    /// <summary>
    /// `TStrings.SaveToFile`。复用既有 `TStringList.SaveToFile`（已含原文的
    /// GBK 固定写出口径 + 编码状态），不自己写盘，避免第二份实现。
    /// </summary>
    public void SaveToFile(string fileName) => _list.SaveToFile(fileName);
}
