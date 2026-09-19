using System;
using System.Windows.Forms;

namespace GXX.DBServer;

/// <summary>
/// Delphi `Spin.pas` 的 `TSpinEdit`（DFM 中 MaxValue/MinValue/Value）。
/// 语义依据（由原文 DFM + 代码交叉验证）：
///   AddrEdit.dfm 中 `ERowCount` 为 MaxValue=0 MinValue=0 **Value=8**，且 AddrEdit.pas:40 再赋 `Value := 8`
///   → TSpinEdit 的**编程赋值不做 Min/Max 裁剪**（否则 DFM 里的 8 不可能存在），裁剪只发生在上下按钮。
/// WinForms `NumericUpDown.Value` 会静默裁剪到 [Minimum, Maximum]，故本类把底层范围放宽到 int 全域，
/// 把 DFM 的 Min/Max 单独保存（DfmMinValue/DfmMaxValue）供上下按钮使用。
/// </summary>
public class TSpinEdit : NumericUpDown
{
    /// <summary>DFM: MinValue。</summary>
    public int DfmMinValue;

    /// <summary>DFM: MaxValue（0 表示原文未设上限，与 MinValue=0 成对出现）。</summary>
    public int DfmMaxValue;

    public TSpinEdit()
    {
        Minimum = int.MinValue;
        Maximum = int.MaxValue;
    }

    /// <summary>DFM: Value（int；编程赋值不裁剪，原文如此）。</summary>
    public new int Value
    {
        get => (int)base.Value;
        set => base.Value = value;
    }

    /// <summary>设置 DFM 的 MinValue/MaxValue（只影响上下按钮裁剪，不影响编程赋值）。</summary>
    public void SetDfmRange(int minValue, int maxValue)
    {
        DfmMinValue = minValue;
        DfmMaxValue = maxValue;
    }

    public override void UpButton()
    {
        if (DfmMaxValue != 0 && Value >= DfmMaxValue) return;
        base.UpButton();
    }

    public override void DownButton()
    {
        if (Value <= DfmMinValue) return;
        base.DownButton();
    }
}

/// <summary>Mir2 自研 `SpinEditEx.pas` 的 `TSpinEditEx`（Value/MinValue/MaxValue 与 TSpinEdit 同构，语义沿用）。</summary>
public class TSpinEditEx : TSpinEdit
{
}
