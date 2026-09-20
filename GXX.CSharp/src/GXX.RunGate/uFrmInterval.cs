using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmInterval.pas 1:1 转换（Source\RunGate\uFrmInterval.pas，279 行 / LF 278）。
// 布局真源：Source\RunGate\uFrmInterval.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：按动作模式编辑 `g_wActionSpeedIntervals[Mode][0..400]`（401 个加速间隔），
// 支持"批量设置间隔"（速度=0 时的基准值 + 每 +1 速度的递减量）、"定位到 0 行"，
// 以及把该模式是否同步给客户端（`g_boSendSpeedIntervalsToClient`）写进 INI。
//
// ★ 原文要点与缺陷（照抄 + 差异断言）：
//   D1. `ShowFrmInterval`（原 :47）先判 `ActionModeUseSpeedIntervals(ActionMode)`，
//       为 False 直接 `Exit`（Result 保持 False）—— **不弹窗、不提示**。
//   D2. 18 个 `else if ActionMode = ...` 分支（原 :54-169）只设置了 Caption 与第 0 列首格文本，
//       其中 `amHit/amSpell/amWalk/amRun` 四者额外把 `chkSendSpeedIntervalsToClient.Visible := False`
//       （原 :60/:68/:76/:84）—— 即这 4 个模式**看不到**"同步间隔设置到客户端"选项。
//       ★ 但 `btnOKClick`（原 :232）的判定用的是**同一组** `[amHit, amSpell, amWalk, amRun]`，
//         两处必须一致，否则会出现"看不见控件却写了它的值"。差异断言见测试。
//   D3. 18 个分支**没有 else**：不在列表中的模式（如 amTurn/amCutMeat/amHitConcurrent...）
//       Caption 与首格保持 DFM 原值（Caption='攻击间隔设置'，首格为空）。
//       注意 `amMoveToTurn` 也**不在**分支里（原文注释掉了 `amMoveToTurn`？——
//       实际列表含 `amTurnToMove`（原 :158）与 `amCutMeatToMove`（原 :164），**不含 amMoveToTurn/amMoveToCutMeat**）。
//   D4. 表格填充（原 :176-187）：`I >= HALF_SPEED_INTERVALS_COUNT` 时首列写 `'+' + IntToStr(I - 200)`，
//       否则写 `IntToStr(I - 200)`。即 I=0 → '-200'，I=200 → '0'，I=201 → '+1'。
//   D5. `btnOKClick` 的校验是 `Value <= 0`（**拒绝负数**），与 uFrmHitInterval 的 `Value = 0` 不同。
//   D6. 校验循环上界用 `grdInterval.RowCount - 2`（原 :203）—— 因 DFM `RowCount=100` 但代码
//       `RowCount := SPEED_INTERVALS_COUNT + 1 = 402`（原 :50），故实际遍历 0..400 共 401 行。
//       ★ 若 `RowCount` 没被改（极端情况），循环上界会变成 98 → 只校验前 99 行。
//       本移植把"实际遍历行数"抽成参数，默认 401。
//   D7. `btnAllClick`（原 :262）用 `Max(5, seSpeed0.Value - seIncSpeedDecTime.Value * (I - 200))`
//       —— 递减量可为负（=递增），且结果**下限恒为 5**（不是 0、不是 1）。
//   D8. `btnZeroClick`/`FormShow`（原 :266-276）都执行
//       `grdInterval.Row := 0; grdInterval.Row := HALF_SPEED_INTERVALS_COUNT + 1;`
//       —— 先到 0 再跳到 201（=速度 0 那一行），目的是把滚动条定位到中间。
//   D9. `btnOKClick` 落盘 INI 的键名是 `'Speed' + IntToStr(I - HALF_SPEED_INTERVALS_COUNT)`
//       → Speed-200 .. Speed200（**含负号**）；而 uFrmHitInterval 用的是 Speed0..SpeedN。
//   D10. 只有 `not (FActionMode in [amHit, amSpell, amWalk, amRun])` 时才写
//        `[Section] SendSpeedIntervalsToClient`（原 :232-242）—— 与 D2 的双重判定呼应。
//   D11. `RebuildSendToClientSpeedIntervalsText`（原 :244）**无条件**调用（不判模式）。
// =====================================================================================

/// <summary>`ShowFrmInterval` 的界面初始化结果（可单测）。</summary>
public class IntervalFormInit
{
    /// <summary>原 :56/64/... `FrmInterval.Caption`。</summary>
    public string Caption = "";

    /// <summary>原 :57/65/... `grdInterval.Cells[0, 0]`（第一列首格；与 DFM 的其它列头无关）。</summary>
    public string FirstHeader = "";

    /// <summary>原 :52/:60 `chkSendSpeedIntervalsToClient.Visible`（默认 True，4 个基础模式为 False）。</summary>
    public bool SendSpeedIntervalsCheckboxVisible = true;

    /// <summary>原 :171-174：仅当 Visible 时才回填 Checked。</summary>
    public bool SendSpeedIntervalsChecked;
}

/// <summary>uFrmInterval.pas 的非 UI 逻辑（可单测）。</summary>
public static class IntervalLogic
{
    /// <summary>原 :50 `RowCount := SPEED_INTERVALS_COUNT + 1`。</summary>
    public const int GridRowCount = RunGateConst.SpeedIntervalsCount + 1;   // 402

    /// <summary>原 :232 与 :60/68/76/84 共用的"4 个基础模式"集合（顺序照抄原文）。</summary>
    public static readonly TAntiPlugActionMode[] BasicModes =
    {
        TAntiPlugActionMode.amHit, TAntiPlugActionMode.amSpell,
        TAntiPlugActionMode.amWalk, TAntiPlugActionMode.amRun
    };

    /// <summary>原 :232 `if not (FActionMode in [amHit, amSpell, amWalk, amRun]) then`。</summary>
    public static bool ShouldWriteSendSpeedIntervalsToClient(TAntiPlugActionMode mode)
        => Array.IndexOf(BasicModes, mode) < 0;

    /// <summary>原 :60/68/76/84：同 4 个基础模式下隐藏"同步到客户端"复选框。</summary>
    public static bool SendSpeedIntervalsCheckboxVisible(TAntiPlugActionMode mode)
        => Array.IndexOf(BasicModes, mode) < 0;

    /// <summary>
    /// 原 :54-169 的 18 个分支：Caption 与首格文本。
    /// 返回 (caption, firstHeader)；未命中任何分支时返回 DFM 原值。
    /// </summary>
    public static (string Caption, string FirstHeader) DescribeMode(TAntiPlugActionMode mode)
    {
        switch (mode)
        {
            case TAntiPlugActionMode.amHit: return ("攻击间隔设置", "攻击加速");             // 原 :56-57
            case TAntiPlugActionMode.amSpell: return ("魔法间隔设置", "魔法加速");           // 原 :64-65
            case TAntiPlugActionMode.amWalk: return ("走路间隔设置", "走路加速");            // 原 :72-73
            case TAntiPlugActionMode.amRun: return ("跑步间隔设置", "跑步加速");             // 原 :80-81
            case TAntiPlugActionMode.amWalkToHit: return ("走路到攻击间隔设置", "攻击加速"); // 原 :88-89
            case TAntiPlugActionMode.amHitToWalk: return ("攻击到走路间隔设置", "走路加速"); // 原 :94-95
            case TAntiPlugActionMode.amRunToHit: return ("跑步到攻击间隔设置", "攻击加速");  // 原 :100-101
            case TAntiPlugActionMode.amHitToRun: return ("攻击到跑步间隔设置", "跑步加速");  // 原 :106-107
            case TAntiPlugActionMode.amWalkToSpell: return ("走路到魔法间隔设置", "魔法加速"); // 原 :112-113
            case TAntiPlugActionMode.amSpellToWalk: return ("魔法到走路间隔设置", "走路加速"); // 原 :118-119
            case TAntiPlugActionMode.amRunToSpell: return ("跑步到魔法间隔设置", "魔法加速");  // 原 :124-125
            case TAntiPlugActionMode.amSpellToRun: return ("魔法到跑步间隔设置", "跑步加速");  // 原 :130-131
            case TAntiPlugActionMode.amTurnToHit: return ("转向到攻击间隔设置", "攻击加速");   // 原 :136-137
            case TAntiPlugActionMode.amTurnToSpell: return ("转向到魔法间隔设置", "魔法加速"); // 原 :142-143
            case TAntiPlugActionMode.amCutMeatToHit: return ("挖肉到攻击间隔设置", "攻击加速"); // 原 :148-149
            case TAntiPlugActionMode.amCutMeatToSpell: return ("挖肉到魔法间隔设置", "魔法加速"); // 原 :154-155
            case TAntiPlugActionMode.amTurnToMove: return ("转向到移动间隔设置", "移动加速");   // 原 :160-161
            case TAntiPlugActionMode.amCutMeatToMove: return ("挖肉到移动间隔设置", "移动加速"); // 原 :166-167
            default:
                // 原 :54-169 的 if..else if 链**无 else** → 保持 DFM 原值（Caption 来自 .dfm）
                return ("攻击间隔设置", "");
        }
    }

    /// <summary>原 :52 + :171-174 的完整初始化。</summary>
    public static IntervalFormInit BuildInit(TAntiPlugActionMode mode)
    {
        var init = new IntervalFormInit();
        var (caption, header) = DescribeMode(mode);
        init.Caption = caption;
        init.FirstHeader = header;
        init.SendSpeedIntervalsCheckboxVisible = SendSpeedIntervalsCheckboxVisible(mode);   // 原 :52/:60
        if (init.SendSpeedIntervalsCheckboxVisible)                                          // 原 :171
            init.SendSpeedIntervalsChecked = FormGlobals.g_boSendSpeedIntervalsToClient[(int)mode];   // 原 :173
        return init;
    }

    /// <summary>
    /// 原 :176-187 的表格填充。
    /// I &lt; 200 → 首列 `IntToStr(I - 200)`（'-200'..'-1'）；
    /// I = 200 → '0'；I &gt; 200 → `'+' + IntToStr(I - 200)`（'+1'..'+200'）。
    /// </summary>
    public static string RowLabel(int i)
        => i >= RunGateConst.HalfSpeedIntervalsCount                            // 原 :178
            ? "+" + DelphiRTL.IntToStr(i - RunGateConst.HalfSpeedIntervalsCount) // 原 :180
            : DelphiRTL.IntToStr(i - RunGateConst.HalfSpeedIntervalsCount);      // 原 :184

    /// <summary>原 :186 的取值文本。</summary>
    public static string RowValueText(TAntiPlugActionMode mode, int i)
        => DelphiRTL.IntToStr(FormGlobals.g_wActionSpeedIntervals[(int)mode][i]);

    /// <summary>
    /// 原 :203-219 `btnOKClick` 的校验循环。
    /// 逐行：`Value := StrToIntDef(Cells[1, I + 1], 0); if Value &lt;= 0 then 弹窗 + Exit;`
    /// （★ 与 uFrmHitInterval 的 `Value = 0` 不同 —— 负数在此被**拒绝**。）
    /// 返回首个非法行的下标（0 基），全部合法返回 -1。
    /// </summary>
    public static int ValidateAll(IReadOnlyList<string> cellTexts, out string failureMessage)
    {
        for (int i = 0; i < cellTexts.Count; i++)                                    // 原 :203
        {
            int value = DelphiRTL.StrToIntDef(cellTexts[i], 0);                       // 原 :205
            if (value <= 0)                                                          // 原 :206
            {
                failureMessage = "输入的数据必须大于0";                                // 原 :208
                return i;
            }
        }
        failureMessage = "";
        return -1;
    }

    /// <summary>
    /// 原 :260-264 `btnAllClick`：`Max(5, seSpeed0.Value - seIncSpeedDecTime.Value * (I - 200))`。
    /// 递减量可为负（等价于递增）；结果下限恒为 5。
    /// </summary>
    public static int ComputeBatchValue(int i, int speed0, int incSpeedDecTime)
    {
        int v = speed0 - incSpeedDecTime * (i - RunGateConst.HalfSpeedIntervalsCount);   // 原 :262
        return v < 5 ? 5 : v;                                                            // Math.Max(5, ...)
    }

    /// <summary>原 :253 的前置校验：`seSpeed0.Value &lt;= 0` → 弹窗。</summary>
    public static bool ValidateBatch(int speed0, out string failureMessage)
    {
        if (speed0 <= 0)                                    // 原 :253
        {
            failureMessage = "输入的数据必须大于0";          // 原 :255
            return false;
        }
        failureMessage = "";
        return true;
    }

    /// <summary>原 :266-270 / :272-276 都会把行游标先归 0 再跳到 201（速度 0 行）。</summary>
    public static int ZeroRowIndex => RunGateConst.HalfSpeedIntervalsCount + 1;   // 原 :269 / :275

    /// <summary>原 :221-230 的间隔文件落盘（键名含负号，见 D9）。空文件名时原文**跳过**整块。</summary>
    public static void SaveIntervalsFile(string fileName, TAntiPlugActionMode mode)
    {
        if (string.IsNullOrEmpty(fileName)) return;                                   // 原 :221 `if Length(...) > 0`
        var ini = new TIniFileEx(fileName);                                           // 原 :223
        try
        {
            for (int i = 0; i < RunGateConst.SpeedIntervalsCount; i++)                 // 原 :225
                ini.WriteInteger("Intervals",
                    "Speed" + DelphiRTL.IntToStr(i - RunGateConst.HalfSpeedIntervalsCount),   // 原 :226
                    FormGlobals.g_wActionSpeedIntervals[(int)mode][i]);
        }
        finally
        {
            ini.Dispose();                                                             // 原 :228 IniFile.Free
        }
    }

    /// <summary>原 :232-242：非 4 个基础模式时写 `[Section] SendSpeedIntervalsToClient`。</summary>
    public static void SaveSendSpeedIntervalsToClient(string iniFileName, TAntiPlugActionMode mode, bool @checked)
    {
        if (!ShouldWriteSendSpeedIntervalsToClient(mode)) return;                      // 原 :232
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)mode] = @checked;              // 原 :234
        var ini = new TIniFileEx(iniFileName);                                         // 原 :235
        try
        {
            ini.WriteBool(RunGateConst.AntiPlugActionModeSections[(int)mode],          // 原 :237
                          "SendSpeedIntervalsToClient",
                          @checked ? (byte)1 : (byte)0);                               // 原 :238
        }
        finally
        {
            ini.Dispose();                                                             // 原 :240 IniFile.Free
        }
    }

    /// <summary>
    /// 原 :196-247 `btnOKClick` 的整体流程（不碰控件）。
    /// `cellTexts` = 第 1 列自上而下的文本（长度 = 实际遍历行数，正常 401）。
    /// </summary>
    public static bool RunButtonClick(TAntiPlugActionMode mode, IReadOnlyList<string> cellTexts,
                                      int intervalFileIndex, bool sendCheckboxChecked,
                                      string iniFileName, string intervalsFileName,
                                      Action<string, string> showError = null)
    {
        int bad = ValidateAll(cellTexts, out string message);
        if (bad >= 0)
        {
            (showError ?? ((t, c) => MessageBoxSeam.ShowError(t, c)))(message, "错误");   // 原 :208
            return false;                                                                // 原 :212 Exit
        }

        // 原 :215-218：`if I < SPEED_INTERVALS_COUNT then g_wActionSpeedIntervals[FActionMode][I] := Value;`
        for (int i = 0; i < cellTexts.Count && i < RunGateConst.SpeedIntervalsCount; i++)
            FormGlobals.g_wActionSpeedIntervals[(int)mode][i] = (ushort)DelphiRTL.StrToIntDef(cellTexts[i], 0);

        SaveIntervalsFile(intervalsFileName, mode);                                       // 原 :221-230
        SaveSendSpeedIntervalsToClient(iniFileName, mode, sendCheckboxChecked);           // 原 :232-242

        FormGlobals.RebuildSendToClientSpeedIntervalsText();                              // 原 :244
        return true;                                                                      // 原 :246 ModalResult := mrOK
    }
}

/// <summary>原 :41-193 `function ShowFrmInterval(ActionMode: TAntiPlugActionMode): Boolean;`。</summary>
public static class IntervalUnit
{
    public static bool ShowFrmInterval(TAntiPlugActionMode actionMode)
    {
        if (!FormGlobals.ActionModeUseSpeedIntervals(actionMode)) return false;      // 原 :47（Result 保持 False）
        using var form = new FrmInterval { FActionMode = actionMode };
        form.FillGrid();                                                             // 原 :50-187
        return form.ShowDialog() == DialogResult.OK;                                 // 原 :189
    }
}

/// <summary>原 uFrmInterval.pas:10-33 `TFrmInterval`（DFM: uFrmInterval.dfm）。</summary>
public class FrmInterval : Form
{
    // DFM: FrmInterval Left=192 Top=130 BorderStyle=bsDialog BorderWidth=5 Caption='攻击间隔设置'
    //      ClientHeight=576 ClientWidth=238 Font.Height=-11 Font.Name='Tahoma'
    //      Position=poMainFormCenter OnShow=FormShow PixelsPerInch=96
    public GroupBox grpSetting;      // DFM: grpSetting Left=0 Top=0 Width=238 Height=484 Align=alClient Caption='间隔设置' TabOrder=0
    public DataGridView grdInterval; // DFM: grdInterval (TStringGrid) Left=7 Top=20 Width=210 Height=458 ColCount=2 DefaultColWidth=100 DefaultRowHeight=18 RowCount=100
    public Panel pnlBottom;          // DFM: pnlBottom Left=0 Top=484 Width=238 Height=92 Align=alBottom BevelOuter=bvNone TabOrder=1
    public Button btnOK;             // DFM: btnOK Left=162 Top=67 Width=75 Height=25 Caption='确定' TabOrder=0 OnClick=btnOKClick
    public GroupBox grpBatch;        // DFM: grpBatch Left=0 Top=0 Width=238 Height=65 Caption=' 批量设置间隔' TabOrder=1
    public Label lblSpeed0;          // DFM: lblSpeed0 Left=8 Top=16 Width=122 Height=13 Caption='当速度=0时时间间隔：'
    public Label lblIncSpeedDecTime; // DFM: lblIncSpeedDecTime Left=8 Top=40 Width=122 Height=13 Caption='速度每+1时间间隔减少：'
    public Button btnAll;            // DFM: btnAll Left=190 Top=36 Width=41 Height=22 Caption='设置' TabOrder=0 OnClick=btnAllClick
    public TSpinEditEx seSpeed0;     // DFM: seSpeed0 Left=128 Top=12 Width=104 Height=22 MaxValue=0 MinValue=0 TabOrder=1 Value=0
    public TSpinEditEx seIncSpeedDecTime; // DFM: seIncSpeedDecTime Left=128 Top=36 Width=60 Height=22 MaxValue=0 MinValue=0 TabOrder=2 Value=0
    public CheckBox chkSendSpeedIntervalsToClient; // DFM: chkSendSpeedIntervalsToClient Left=0 Top=71 Width=153 Height=17 Caption='同步间隔设置到客户端' Hint=... TabOrder=2
    public Button btnZero;           // DFM: btnZero (TSpeedButton) Left=220 Top=241 Width=14 Height=21 Caption='0' OnClick=btnZeroClick

    /// <summary>原 :30 `FActionMode: TAntiPlugActionMode;`。</summary>
    public TAntiPlugActionMode FActionMode = TAntiPlugActionMode.amHit;

    /// <summary>
    /// 原 :52/:60/... `chkSendSpeedIntervalsToClient.Visible := ...` 的决策镜像。
    /// Windows 下控件的 `Visible` 在其父窗体未显示时**恒为 false**，故单测不能直接断言 `Visible`；
    /// 这里在 `FillGrid` 中同步记录原文写入的值。真实运行时它始终等于 `Visible`。
    /// </summary>
    public bool SendSpeedIntervalsCheckboxVisible;

    public FrmInterval()
    {
        // DFM: FrmInterval Caption='攻击间隔设置' BorderStyle=bsDialog BorderWidth=5 Position=poMainFormCenter
        Text = "攻击间隔设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Padding = new Padding(5);                       // BorderWidth=5
        StartPosition = FormStartPosition.CenterParent; // Position=poMainFormCenter
        Location = new Point(192, 130);
        ClientSize = new Size(238, 576);
        Font = new Font("Tahoma", 8.25F);               // Font.Height=-11 Font.Name='Tahoma'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: grpSetting Left=0 Top=0 Width=238 Height=484 Align=alClient Caption='间隔设置' TabOrder=0
        grpSetting = new GroupBox { Left = 0, Top = 0, Width = 238, Height = 484,
                                    Dock = DockStyle.Fill, Text = "间隔设置", TabIndex = 0 };
        // DFM: btnZero (TSpeedButton) Left=220 Top=241 Width=14 Height=21 Caption='0' OnClick=btnZeroClick
        //       TSpeedButton → WinForms Button（FlatStyle=Flat 近似），Caption='0'
        btnZero = new Button { Left = 220, Top = 241, Width = 14, Height = 21, Text = "0",
                               FlatStyle = FlatStyle.Flat, TabStop = false };
        // DFM: grdInterval (TStringGrid) Left=7 Top=20 Width=210 Height=458 ColCount=2 DefaultColWidth=100
        //      DefaultRowHeight=18 RowCount=100 Options 含 goEditing + goThumbTracking
        grdInterval = new DataGridView
        {
            Left = 7, Top = 20, Width = 210, Height = 458, TabIndex = 0,
            ColumnCount = 2,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,   // Anchors=[akLeft,akTop,akBottom]
            RowTemplate = { Height = 18 },                                        // DefaultRowHeight=18
            ScrollBars = ScrollBars.Vertical
        };
        grdInterval.Columns[0].Width = 100;      // DefaultColWidth=100
        grdInterval.Columns[0].ReadOnly = true;
        grdInterval.Columns[1].Width = 100;
        // DFM: RowCount=100（含固定行）；运行时原 :50 会重设为 SPEED_INTERVALS_COUNT + 1 = 402
        for (int i = 0; i < 100; i++) grdInterval.Rows.Add();

        // DFM: pnlBottom Left=0 Top=484 Width=238 Height=92 Align=alBottom BevelOuter=bvNone TabOrder=1
        pnlBottom = new Panel { Left = 0, Top = 484, Width = 238, Height = 92,
                                Dock = DockStyle.Bottom, BorderStyle = BorderStyle.None, TabIndex = 1 };
        // DFM: btnOK Left=162 Top=67 Width=75 Height=25 Caption='确定' TabOrder=0 OnClick=btnOKClick
        btnOK = new Button { Left = 162, Top = 67, Width = 75, Height = 25, Text = "确定", TabIndex = 0 };
        // DFM: grpBatch Left=0 Top=0 Width=238 Height=65 Caption=' 批量设置间隔' TabOrder=1
        //      ★ Caption 前置一个空格，照抄
        grpBatch = new GroupBox { Left = 0, Top = 0, Width = 238, Height = 65, Text = " 批量设置间隔", TabIndex = 1 };
        // DFM: lblSpeed0 Left=8 Top=16 Width=122 Height=13 Caption='当速度=0时时间间隔：'
        lblSpeed0 = new Label { Left = 8, Top = 16, Width = 122, Height = 13, Text = "当速度=0时时间间隔：" };
        // DFM: lblIncSpeedDecTime Left=8 Top=40 Width=122 Height=13 Caption='速度每+1时间间隔减少：'
        lblIncSpeedDecTime = new Label { Left = 8, Top = 40, Width = 122, Height = 13, Text = "速度每+1时间间隔减少：" };
        // DFM: btnAll Left=190 Top=36 Width=41 Height=22 Caption='设置' TabOrder=0 OnClick=btnAllClick
        btnAll = new Button { Left = 190, Top = 36, Width = 41, Height = 22, Text = "设置", TabIndex = 0 };
        // DFM: seSpeed0 Left=128 Top=12 Width=104 Height=22 MaxValue=0 MinValue=0 TabOrder=1 Value=0
        seSpeed0 = new TSpinEditEx { Left = 128, Top = 12, Width = 104, Height = 22, TabIndex = 1 };
        seSpeed0.SetDfmRange(0, 0);
        seSpeed0.Value = 0;
        // DFM: seIncSpeedDecTime Left=128 Top=36 Width=60 Height=22 MaxValue=0 MinValue=0 TabOrder=2 Value=0
        seIncSpeedDecTime = new TSpinEditEx { Left = 128, Top = 36, Width = 60, Height = 22, TabIndex = 2 };
        seIncSpeedDecTime.SetDfmRange(0, 0);
        seIncSpeedDecTime.Value = 0;
        // DFM: chkSendSpeedIntervalsToClient Left=0 Top=71 Width=153 Height=17 Caption='同步间隔设置到客户端' TabOrder=2
        chkSendSpeedIntervalsToClient = new CheckBox { Left = 0, Top = 71, Width = 153, Height = 17,
                                                       Text = "同步间隔设置到客户端", TabIndex = 2 };
        var tip = new ToolTip();
        tip.SetToolTip(chkSendSpeedIntervalsToClient, "勾选后，间隔会发往客户端，客户端会控制组合间隔≥设置的间隔");

        grpBatch.Controls.AddRange(new Control[] { lblSpeed0, lblIncSpeedDecTime, btnAll, seSpeed0, seIncSpeedDecTime });
        pnlBottom.Controls.AddRange(new Control[] { btnOK, grpBatch, chkSendSpeedIntervalsToClient });
        grpSetting.Controls.AddRange(new Control[] { btnZero, grdInterval });
        Controls.Add(grpSetting);
        Controls.Add(pnlBottom);

        // DFM: OnShow = FormShow；OnClick = btnOKClick / btnAllClick / btnZeroClick
        Shown += (s, e) => FormShow(s, e);
        btnOK.Click += (s, e) => btnOK_Click(s, e);
        btnAll.Click += (s, e) => btnAll_Click(s, e);
        btnZero.Click += (s, e) => btnZero_Click(s, e);
    }

    /// <summary>原 :41-193 `ShowFrmInterval` 中"窗体侧"的部分（表格尺寸 + 标题 + 首格 + 复选框可见性 + 全表填充）。</summary>
    public void FillGrid()
    {
        var init = IntervalLogic.BuildInit(FActionMode);

        // 原 :50 grdInterval.RowCount := SPEED_INTERVALS_COUNT + 1
        SetGridRowCount(IntervalLogic.GridRowCount);
        // 原 :52/:60/... chkSendSpeedIntervalsToClient.Visible := ...
        chkSendSpeedIntervalsToClient.Visible = init.SendSpeedIntervalsCheckboxVisible;
        SendSpeedIntervalsCheckboxVisible = init.SendSpeedIntervalsCheckboxVisible;   // 决策镜像（见字段说明）
        // 原 :56/.../166 Caption := ...
        Text = init.Caption;
        // 原 :57/.../167 grdInterval.Cells[0, 0] := ...
        if (grdInterval.Rows.Count > 0) grdInterval.Rows[0].Cells[0].Value = init.FirstHeader;
        // 原 :171-174 `if chkSendSpeedIntervalsToClient.Visible then
        //                chkSendSpeedIntervalsToClient.Checked := g_boSendSpeedIntervalsToClient[ActionMode];`
        // ★ 必须用**决策镜像**而不是控件的 `Visible`：WinForms 的 `Control.Visible` 在父窗体未显示时恒为 false
        //   （Delphi 的 `Visible` 反映的是刚被赋的值），否则无头/未显示环境下这句永远不会执行。
        //   真实运行时两者等价 —— 上面刚把 `Visible` 赋成同一个值。
        if (SendSpeedIntervalsCheckboxVisible)
            chkSendSpeedIntervalsToClient.Checked = init.SendSpeedIntervalsChecked;

        // 原 :176-187 全表填充（第 0 行是上面设过的"首格 + 间隔检测"行）
        if (grdInterval.Rows.Count > 0) grdInterval.Rows[0].Cells[1].Value = "间隔检测";
        for (int i = 0; i < RunGateConst.SpeedIntervalsCount; i++)
        {
            if (i + 1 >= grdInterval.Rows.Count) break;
            grdInterval.Rows[i + 1].Cells[0].Value = IntervalLogic.RowLabel(i);              // 原 :180/:184
            grdInterval.Rows[i + 1].Cells[1].Value = IntervalLogic.RowValueText(FActionMode, i);  // 原 :186
        }
    }

    /// <summary>原 :209 / :268-269 `grdInterval.Row := N` 的托管等价（设置行游标 + 滚动到可见）。</summary>
    public void SetGridRow(int row)
    {
        if (row < 0 || row >= grdInterval.Rows.Count) return;
        grdInterval.CurrentCell = grdInterval.Rows[row].Cells[1];
        grdInterval.FirstDisplayedScrollingRowIndex = row;
    }

    private void SetGridRowCount(int count)
    {
        while (grdInterval.Rows.Count > count) grdInterval.Rows.RemoveAt(grdInterval.Rows.Count - 1);
        while (grdInterval.Rows.Count < count) grdInterval.Rows.Add();
    }

    /// <summary>测试辅助：读回第 1 列（数据行，含第 0 行）的文本。</summary>
    public List<string> ReadValueColumnTexts()
    {
        var list = new List<string>();
        for (int i = 0; i < grdInterval.Rows.Count; i++)
        {
            object v = grdInterval.Rows[i].Cells[1].Value;
            list.Add(v == null ? "" : v.ToString());
        }
        return list;
    }

    /// <summary>原 uFrmInterval.pas:196-247 `TFrmInterval.btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        // 原 :203 `for I := 0 to grdInterval.RowCount - 2` → 遍历的是**数据行**（不含第 0 行）
        var all = ReadValueColumnTexts();
        var texts = new List<string>();
        for (int i = 1; i < all.Count; i++) texts.Add(all[i]);

        int bad = IntervalLogic.ValidateAll(texts, out string message);
        if (bad >= 0)
        {
            MessageBoxSeam.ShowError(message, "错误");                       // 原 :208
            SetGridRow(bad + 1);                                            // 原 :209 grdInterval.Row := I + 1
            if (grdInterval.CanFocus) grdInterval.Focus();                   // 原 :210-211
            return;                                                          // 原 :212 Exit
        }

        for (int i = 0; i < texts.Count && i < RunGateConst.SpeedIntervalsCount; i++)   // 原 :215-218
            FormGlobals.g_wActionSpeedIntervals[(int)FActionMode][i] = (ushort)DelphiRTL.StrToIntDef(texts[i], 0);

        IntervalLogic.SaveIntervalsFile(FormGlobals.g_sActionIntervalsFileNames[(int)FActionMode], FActionMode);   // 原 :221-230
        IntervalLogic.SaveSendSpeedIntervalsToClient(FormGlobals.g_sIniFileName, FActionMode,
                                                    chkSendSpeedIntervalsToClient.Checked);                        // 原 :232-242

        FormGlobals.RebuildSendToClientSpeedIntervalsText();                                                       // 原 :244
        DialogResult = DialogResult.OK;                                                                            // 原 :246
    }

    /// <summary>原 uFrmInterval.pas:249-264 `TFrmInterval.btnAllClick`。</summary>
    public void btnAll_Click(object sender, EventArgs e)
    {
        if (!IntervalLogic.ValidateBatch(seSpeed0.Value, out string message))       // 原 :253
        {
            MessageBoxSeam.ShowError(message, "错误");                               // 原 :255
            if (seSpeed0.CanFocus) seSpeed0.Focus();                                 // 原 :256
            return;                                                                  // 原 :257 Exit
        }

        for (int i = 0; i < grdInterval.Rows.Count - 1; i++)                          // 原 :260
        {
            int value = IntervalLogic.ComputeBatchValue(i, seSpeed0.Value, seIncSpeedDecTime.Value);   // 原 :262
            grdInterval.Rows[i + 1].Cells[1].Value = DelphiRTL.IntToStr(value);
        }
    }

    /// <summary>原 uFrmInterval.pas:266-270 `TFrmInterval.btnZeroClick`。</summary>
    public void btnZero_Click(object sender, EventArgs e)
    {
        SetGridRow(0);                                       // 原 :268
        SetGridRow(IntervalLogic.ZeroRowIndex);              // 原 :269
    }

    /// <summary>原 uFrmInterval.pas:272-276 `TFrmInterval.FormShow`。</summary>
    public void FormShow(object sender, EventArgs e)
    {
        SetGridRow(0);                                       // 原 :274
        SetGridRow(IntervalLogic.ZeroRowIndex);              // 原 :275
    }
}
