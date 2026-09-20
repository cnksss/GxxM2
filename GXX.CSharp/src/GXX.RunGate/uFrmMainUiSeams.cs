// =====================================================================================
// 源单元：Source\RunGate\uFrmMain.pas（实测 LF 4217）—— **UI 接缝垫片**
//
// 为什么需要这个文件（跨车道踩坑，本工程已发生）：
//   Delphi 的 `TComboBox.ItemIndex := V` / `TListBox.ItemIndex := V` 在 **V 越界时静默置 -1**，
//   **不抛异常**；而 WinForms 的 `SelectedIndex = V` 越界会抛 `ArgumentOutOfRangeException` → **必崩**。
//   原文大量"从 INI/配置读出的索引直接赋给控件"的写法正是靠 Delphi 的静默容忍才没崩。
//
//   ★ 本车道的**具体命中点**（不是泛泛而谈）：
//     `uFrmMain.pas:1112-1113` 把 `g_btShowLogLevel` 的上界钳到 **`cbbShowLogLevel.Items.Count`（= 11）**，
//     而该下拉框有 11 项 → **合法 ItemIndex 只到 10**；随后 `:1521`
//     `cbbShowLogLevel.ItemIndex := g_btShowLogLevel;` 在值为 11 时**越界**。
//     Delphi 静默置 -1；WinForms 直接抛。见本报告 §20 的缺陷 **C1**。
//   （同一坑由 `p6-m2-pets` 车道在 `uFrmMainGamePets.pas:408/:415` 用测试抓出，处置方式同此。）
//
// 同类一并覆盖：`TListBox.ItemIndex`、`TCheckListBox`/`TRadioGroup.ItemIndex`（都走同一垫片）。
//
// ★ 另一类**不要**混淆的差异（两边都抛，但类型不同）：
//   Delphi `TStrings[Index]` 越界抛 `EStringListError`；托管 `List<T>[i]` 抛 `ArgumentOutOfRangeException`。
//   若原文用 `try/except` 吞掉，托管侧必须吞**对应的**异常类型，否则行为不一致。
//   本车道的 `TAddressList`/`TAddressListEx`/`TMagicIntervalList` 已按原文**返回 nil（不抛）**，
//   不受此条影响（见 GateShareContainers.cs / GateShareMagicIntervalUtils.cs 的 `this[int]`）。
// =====================================================================================

using System.Windows.Forms;

namespace GXX.RunGate;

/// <summary>`uFrmMain.pas` 的 UI 接缝垫片（复刻 Delphi 控件的静默容忍语义）。</summary>
public static class RunGateUiSeams
{
    /// <summary>Delphi `TComboBox.ItemIndex := V` / `TListBox.ItemIndex := V` 的赋值语义：
    /// `V &lt; 0` 或 `V &gt;= ItemCount` → **静默置 -1**（不抛）。
    /// `V` 合法时原样返回。</summary>
    public static int DelphiItemIndex(int value, int itemCount)
        => (value < 0 || value >= itemCount) ? -1 : value;

    /// <summary>把可能越界的值安全写进 `ComboBox`（Delphi 语义：越界 → -1）。</summary>
    public static void SetComboItemIndex(ComboBox comboBox, int value)
    {
        if (comboBox == null) return;
        comboBox.SelectedIndex = DelphiItemIndex(value, comboBox.Items.Count);
    }

    /// <summary>把可能越界的值安全写进 `ListBox`（Delphi 语义：越界 → -1）。</summary>
    public static void SetListItemIndex(ListBox listBox, int value)
    {
        if (listBox == null) return;
        listBox.SelectedIndex = DelphiItemIndex(value, listBox.Items.Count);
    }

    /// <summary>Delphi `TComboBox.Text` 在 `ItemIndex = -1` 时的读法：返回 **空串**
    /// （WinForms 的 `Text` 在无选中时也可能保留旧文本 → 读之前先按本方法归位）。</summary>
    public static string ComboTextForItemIndex(ComboBox comboBox)
    {
        if (comboBox == null) return "";
        return comboBox.SelectedIndex < 0 ? "" : (comboBox.SelectedItem?.ToString() ?? "");
    }

    /// <summary>`TStrings[Index]` 的安全只读访问：越界返回 `def`（不抛）。
    /// 用于原文那种"先判 `Items.Count` 再取"或"靠 `try/except` 兜"的取文本路径。</summary>
    public static string ItemTextOr(ComboBox comboBox, int index, string def = "")
    {
        if (comboBox == null || index < 0 || index >= comboBox.Items.Count) return def;
        return comboBox.Items[index]?.ToString() ?? def;
    }
}
