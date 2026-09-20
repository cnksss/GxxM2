using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmHitInterval.pas 1:1 转换（Source\RunGate\uFrmHitInterval.pas，85 行 / LF 84）。
// 布局真源：Source\RunGate\uFrmHitInterval.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：编辑 `g_dwHitIntervals`（攻击加速间隔表）并把结果写回
// `g_sHitIntervalsFileName` 指向的 INI（[Intervals] Speed0..SpeedN）。
//
// 接缝（纯逻辑，不依赖 WinForms）：
//   * HitIntervalLogic.BuildGrid(...)   —— 原 :28-49 的表格填充
//   * HitIntervalLogic.ValidateAndApply —— 原 :52-82 的校验 + 写回（含弹窗接缝）
//
// ★ 原文行为要点（易错）：
//   校验用的是 `Value = 0` 判等（原 :61），而 uFrmInterval.pas:206 用的是 `Value <= 0`。
//   两者对负数**不同**：本窗体输入 "-5" 会被接受并写回（g_dwHitIntervals 是 LongWord，
//   负数按二进制补码落盘），uFrmInterval 会拒绝。差异断言见测试。
// =====================================================================================

/// <summary>uFrmHitInterval.pas 的非 UI 逻辑（可单测）。</summary>
public static class HitIntervalLogic
{
    /// <summary>原 uFrmHitInterval.pas:36-37 的两个列头（注意原文用 '间隔检测'）。</summary>
    public const string Column0Header = "攻击加速";
    public const string Column1Header = "间隔检测";

    /// <summary>
    /// 原 uFrmHitInterval.pas:35-43 `ShowFrmHitInterval` 的表格填充：
    ///   RowCount := Length(g_dwHitIntervals) + 1
    ///   Cells[0,0] := '攻击加速'; Cells[1,0] := '间隔检测'
    ///   for I := 0 to High(g_dwHitIntervals): Cells[0,I+1] := '攻击加速+' + IntToStr(I);
    ///                                        Cells[1,I+1] := IntToStr(g_dwHitIntervals[I]);
    /// 返回 (行文本, 值文本) 列表，第一项为表头。
    /// </summary>
    public static List<KeyValuePair<string, string>> BuildGrid(uint[] hitIntervals)
    {
        var rows = new List<KeyValuePair<string, string>>();
        rows.Add(new KeyValuePair<string, string>(Column0Header, Column1Header));   // 原 :36-37
        if (hitIntervals == null) return rows;
        for (int i = 0; i < hitIntervals.Length; i++)                                // 原 :39-43
            rows.Add(new KeyValuePair<string, string>("攻击加速+" + DelphiRTL.IntToStr(i),
                                                     DelphiRTL.IntToStr(hitIntervals[i])));
        return rows;
    }

    /// <summary>
    /// 原 uFrmHitInterval.pas:52-82 `TFrmHitInterval.btn1Click` 的校验部分。
    /// 逐行：
    ///   Value := StrToIntDef(Cells[1, I+1], 0);       // 非数字 → 0
    ///   if Value = 0 then 弹窗 '输入的数据必须大于0' 并 Exit（**判等 0，不判负数**）
    ///   g_dwHitIntervals[I] := Value;
    /// 返回：失败行的索引（0 基），成功返回 -1。`failureMessage` 为弹窗文本（便于断言）。
    /// </summary>
    public static int ValidateAll(IReadOnlyList<string> cellTexts, out string failureMessage)
    {
        for (int i = 0; i < cellTexts.Count; i++)                                     // 原 :58
        {
            int value = DelphiRTL.StrToIntDef(cellTexts[i], 0);                       // 原 :60
            if (value == 0)                                                           // 原 :61
            {
                failureMessage = "输入的数据必须大于0";                                // 原 :63
                return i;
            }
        }
        failureMessage = "";
        return -1;
    }

    /// <summary>原 :70 的赋值循环体：把已校验通过的文本写回 `g_dwHitIntervals[I]`。
    /// 注意 LongWord 语义：负数按 32 位补码截断（Delphi 同理）。</summary>
    public static void ApplyValues(uint[] hitIntervals, IReadOnlyList<string> cellTexts)
    {
        for (int i = 0; i < hitIntervals.Length && i < cellTexts.Count; i++)
            hitIntervals[i] = unchecked((uint)DelphiRTL.StrToIntDef(cellTexts[i], 0));   // 原 :70
    }

    /// <summary>
    /// 原 uFrmHitInterval.pas:73-79 的 INI 落盘：
    ///   IniFile := TIniFile.Create(g_sHitIntervalsFileName);
    ///   for I := 0 to High(g_dwHitIntervals):
    ///     IniFile.WriteInteger('Intervals', 'Speed' + IntToStr(I), g_dwHitIntervals[I]);
    /// 节名固定 'Intervals'，键名 'Speed0'..'SpeedN'（**从 0 开始**，与 uFrmInterval 的
    /// 'Speed' + (I - 200) 即 Speed-200..Speed200 不同）。
    /// 原文**不判断文件名是否为空**（uFrmInterval 会判断），空文件名会写出名为 "" 的文件 →
    /// 托管侧 TIniFileEx("") 的 UpdateFile 在 FileName == "" 时直接返回，此处保留该差异并在注释登记。
    /// </summary>
    public static void Save(string iniFileName, uint[] hitIntervals)
    {
        var ini = new TIniFileEx(iniFileName);                                        // 原 :73
        try
        {
            for (int i = 0; i < hitIntervals.Length; i++)                             // 原 :75-76
                ini.WriteInteger("Intervals", "Speed" + DelphiRTL.IntToStr(i), (int)hitIntervals[i]);
        }
        finally
        {
            ini.Dispose();                                                            // 原 :78 IniFile.Free
        }
    }

    /// <summary>原 :52-82 的整体流程：校验 → 写全局 → 落盘。返回是否成功（成功 = ModalResult := mrOK）。
    /// `showError` 为弹窗接缝（默认走 MessageBoxSeam.ShowError）。</summary>
    public static bool RunButtonClick(uint[] hitIntervals, IReadOnlyList<string> cellTexts, string iniFileName,
                                      Action<string, string> showError = null)
    {
        int bad = ValidateAll(cellTexts, out string message);
        if (bad >= 0)
        {
            (showError ?? DefaultShowError)(message, "错误");                          // 原 :63
            return false;                                                             // 原 :67 Exit
        }
        ApplyValues(hitIntervals, cellTexts);                                         // 原 :70
        Save(iniFileName, hitIntervals);                                              // 原 :73-79
        return true;                                                                  // 原 :81 ModalResult := mrOK
    }

    private static void DefaultShowError(string text, string caption)
        => MessageBoxSeam.ShowError(text, caption);
}

/// <summary>原 :28-49 `function ShowFrmHitInterval: Boolean;`。</summary>
public static class HitIntervalUnit
{
    public static bool ShowFrmHitInterval()
    {
        using var form = new FrmHitInterval();
        form.Load += (s, e) => form.FillGrid();                                        // 原 :35-43
        return form.ShowDialog() == DialogResult.OK;                                   // 原 :45
    }
}

/// <summary>原 uFrmHitInterval.pas:10-20 `TFrmHitInterval`（DFM: uFrmHitInterval.dfm）。</summary>
public class FrmHitInterval : Form
{
    // DFM: FrmHitInterval Left=192 Top=130 BorderStyle=bsDialog BorderWidth=5 Caption='攻击间隔设置'
    //      ClientHeight=518 ClientWidth=195 Font.Height=-11 Font.Name='Tahoma' PixelsPerInch=96
    public GroupBox grpSetting;        // DFM: grpSetting Left=0 Top=0 Width=195 Height=487 Align=alClient Caption='攻击间隔设置' TabOrder=0
    public DataGridView grdHitInterval; // DFM: grdHitInterval (TStringGrid) Left=6 Top=20 Width=183 Height=460 ColCount=2 DefaultColWidth=80 DefaultRowHeight=18 RowCount=100
    public Panel pnlBottom;            // DFM: pnlBottom Left=0 Top=487 Width=195 Height=31 Align=alBottom BevelOuter=bvNone TabOrder=1
    public Button btn1;                // DFM: btn1 Left=119 Top=4 Width=75 Height=25 Caption='确定' TabOrder=0 OnClick=btn1Click

    public FrmHitInterval()
    {
        // DFM: FrmHitInterval Caption='攻击间隔设置' BorderStyle=bsDialog BorderWidth=5
        Text = "攻击间隔设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Padding = new Padding(5);                       // BorderWidth=5
        Location = new Point(192, 130);
        ClientSize = new Size(195, 518);
        Font = new Font("Tahoma", 8.25F);               // Font.Height=-11 Font.Name='Tahoma'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: grpSetting Left=0 Top=0 Width=195 Height=487 Align=alClient Caption='攻击间隔设置' TabOrder=0
        grpSetting = new GroupBox { Left = 0, Top = 0, Width = 195, Height = 487,
                                    Dock = DockStyle.Fill, Text = "攻击间隔设置", TabIndex = 0 };
        // DFM: grdHitInterval (TStringGrid) Left=6 Top=20 Width=183 Height=460 ColCount=2
        //      DefaultColWidth=80 DefaultRowHeight=18 RowCount=100 Options=[goFixedVertLine,goFixedHorzLine,goVertLine,goHorzLine,goEditing]
        // 说明：Delphi TStringGrid → WinForms DataGridView（第三方 Grids 单元未移植），
        //       固定行 = 列头，goEditing → 仅第 1 列可编辑（原文第 0 列是标签、第 1 列是数值）。
        grdHitInterval = new DataGridView
        {
            Left = 6, Top = 20, Width = 183, Height = 460, TabIndex = 0,
            ColumnCount = 2,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,   // Anchors=[akLeft,akTop,akBottom]
            RowTemplate = { Height = 18 },                                       // DefaultRowHeight=18
            ScrollBars = ScrollBars.Vertical
        };
        grdHitInterval.Columns[0].Width = 80;                                        // DefaultColWidth=80
        grdHitInterval.Columns[0].ReadOnly = true;
        grdHitInterval.Columns[1].Width = 80;
        // DFM: RowCount=100（TStringGrid 含固定行）；DataGridView 无固定行 → 100 行数据行
        for (int i = 0; i < 100; i++) grdHitInterval.Rows.Add();

        // DFM: pnlBottom Left=0 Top=487 Width=195 Height=31 Align=alBottom BevelOuter=bvNone TabOrder=1
        pnlBottom = new Panel { Left = 0, Top = 487, Width = 195, Height = 31,
                                Dock = DockStyle.Bottom, BorderStyle = BorderStyle.None, TabIndex = 1 };
        // DFM: btn1 Left=119 Top=4 Width=75 Height=25 Caption='确定' TabOrder=0 OnClick=btn1Click
        btn1 = new Button { Left = 119, Top = 4, Width = 75, Height = 25, Text = "确定", TabIndex = 0 };

        pnlBottom.Controls.Add(btn1);
        grpSetting.Controls.Add(grdHitInterval);
        Controls.Add(grpSetting);
        Controls.Add(pnlBottom);

        // DFM: btn1 OnClick = btn1Click
        btn1.Click += (s, e) => btn1_Click(s, e);
    }

    /// <summary>原 uFrmHitInterval.pas:35-43 的表格填充（供 ShowFrmHitInterval 调用）。</summary>
    public void FillGrid()
    {
        var rows = HitIntervalLogic.BuildGrid(FormGlobals.g_dwHitIntervals);
        grdHitInterval.Rows.Clear();
        grdHitInterval.Rows.Add(rows.Count);
        for (int r = 0; r < rows.Count && r < grdHitInterval.Rows.Count; r++)
        {
            grdHitInterval.Rows[r].Cells[0].Value = rows[r].Key;
            grdHitInterval.Rows[r].Cells[1].Value = rows[r].Value;
        }
    }

    /// <summary>测试辅助：读回当前表格的第 1 列文本（0 基数据行）。</summary>
    public List<string> ReadValueColumnTexts()
    {
        var list = new List<string>();
        for (int i = 1; i < grdHitInterval.Rows.Count; i++)   // 跳过表头行
        {
            object v = grdHitInterval.Rows[i].Cells[1].Value;
            list.Add(v == null ? "" : v.ToString());
        }
        return list;
    }

    /// <summary>原 uFrmHitInterval.pas:52-82 `TFrmHitInterval.btn1Click`。</summary>
    public void btn1_Click(object sender, EventArgs e)
    {
        // 注意原文从 I = 0 开始遍历 g_dwHitIntervals（**不含表头行**），
        // 表格第 0 行是表头，故数据从 DataGridView 的第 1 行（Rows[1]）起对应 g_dwHitIntervals[0]。
        var texts = ReadValueColumnTexts();
        int bad = HitIntervalLogic.ValidateAll(texts, out string message);
        if (bad >= 0)
        {
            MessageBoxSeam.ShowError(message, "错误");                    // 原 :63
            if (bad + 1 < grdHitInterval.Rows.Count)                      // 原 :64 grdHitInterval.Row := I + 1
            {
                grdHitInterval.CurrentCell = grdHitInterval.Rows[bad + 1].Cells[1];
                if (grdHitInterval.CanFocus) grdHitInterval.Focus();       // 原 :65-66
            }
            return;                                                        // 原 :67 Exit
        }

        HitIntervalLogic.ApplyValues(FormGlobals.g_dwHitIntervals, texts);  // 原 :70
        HitIntervalLogic.Save(FormGlobals.g_sHitIntervalsFileName, FormGlobals.g_dwHitIntervals);   // 原 :73-79
        DialogResult = DialogResult.OK;                                     // 原 :81 ModalResult := mrOK
    }
}
