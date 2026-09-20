using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmGameSpeed.pas 1:1 转换（Source\RunGate\uFrmGameSpeed.pas，1333 行 / LF 1332）。
// 布局真源：Source\RunGate\uFrmGameSpeed.dfm（**同名 .dfm 存在**，共 881 行）。
//
// 窗体职责（外挂控制）：本车道最大的一个窗体。
//   * 27 种动作模式的"是否控制 / 间隔 / 超速处理方式 / 累计超速处理 / 超速提示 / 提示信息 / 补偿值 / 调试"
//     在 vstAntiPlugAction 树里逐行编辑（列 0..8）；
//   * 提示设置（消息类型 / 前背景色 / 预览）；
//   * 锁定设置（锁定时间 / 保存锁定状态 / 显示锁定日志 / 锁定提示串）；
//   * 间隔设置（交易→挑战 / 野蛮→攻击 / 个人商店搜索 / 购买 / 穿戴 / 补偿池清零）；
//   * 其他设置（连续超速放行增量 / 清空数据 / 超速日志 / 多发并发日志）；
//   * 加速规则控制（点 "设置" 按钮打开 uFrmInterval）；
//   * 累计超速规则（统计时间 / 最大次数 / 连续超速踢出 / 次数 / 客户端上传内挂物品间隔）。
//
// 纯逻辑抽离（不依赖 WinForms，全部在 <see cref="GameSpeedLogic"/>）：
//   * DescribeCell / IntervalCellIsButton / IsEditable / GetHintText / CanToggleCheck
//   * BuildProcessModeItems + SelectedIndexForMode   （原 PrepareEdit 列 2 的三套下拉构造：**最易错点**）
//   * BuildSumProcessModeItems + SelectedIndex       （原 PrepareEdit 列 3）
//   * PrepareEditSpec                                （原 :385-572 的列→编辑器规格）
//   * ApplyEditorResult                              （原 :258-361 EndEdit 的"是否变更"判定）
//   * ValidateSpeedValue                             （原 :768-781 的保存前校验）
//   * Save                                           （原 :757-855 btnSaveClick 的 INI 落盘）
//
// ★ 原文要点与缺陷（照抄 + 差异断言）：
//   D1. `FormCreate`（原 :658-672）只创建 `Low(TAntiPlugActionMode) .. amCutMeatToMove`（**0..23 共 24 行**），
//       而 `btnSaveClick`（原 :790）遍历 `Low..High`（**0..26 共 27 行**）写 INI。
//       → 后 3 个"并发"模式（amHitConcurrent/amSpellConcurrent/amMoveConcurrent）**没有行可编辑**，
//         但它们的 Enabled/Interval/... 仍会被写进 INI。这是原文刻意行为（注释"并发不让设置"）。
//   D2. `RefreshCtrlsStatus`（原 :685-688）对 `ActionMode >= amHitConcurrent` 的行**强制勾选**
//       （`CheckState := csCheckedNormal`），而 `vstAntiPlugActionChecking`（原 :1232）又
//       `Allowed := not (ActionMode >= amHitConcurrent)` —— 即这 3 行"勾选但不可改"。
//   D3. 列 1 的编辑器类型取决于模式：`ActionMode in [amHit, amSpell, amWalk, amRun, ... 共 18 种]`
//       时是**按钮**（Caption='设置'，点击打开 uFrmInterval），否则是 SpinEdit；
//       两处判断（原 :392-401 与 :605-614）**列表完全相同**，必须一致。差异断言见测试。
//   D4. 列 2 的下拉项**分三套**（原 :470-509）：
//       (a) `[amHit, amSpell, amWalkToHit, amRunToHit, amWalkToSpell, amRunToSpell, amCutMeatToHit,
//            amCutMeatToSpell, amMoveToTurn, amTurnToMove, amMoveToCutMeat, amCutMeatToMove]`（12 种）
//           → 从 `apmRebound` 到 High（**跳过 apmDelay**），ItemIndex = Ord(mode) - Ord(apmRebound)；
//       (b) `[amHitConcurrent, amSpellConcurrent, amMoveConcurrent]`（3 种）
//           → 从 `apmOffline` 到 High，名称用 **Names2**，ItemIndex = Ord(mode) - Ord(apmOffline)；
//       (c) 其余 → 从 Low 到 High **但跳过 apmFakeAttackPass**，名称用 Names，
//           ItemIndex 在 `ProcessMode = apmNoProcess` 时 `Ord - 1`（因为前面少一项），否则 `Ord`。
//       ★ 这套 ItemIndex 换算极易错（尤其 (c) 的 `apmNoProcess` 特判）—— 测试逐分支断言。
//   D5. `vstAntiPlugActionEditing`（原 :1065-1079）：`ActionMode >= amHitConcurrent` 时列 1/2 不可编辑；
//       `not (ActionMode in [amHit, amWalk, amRun])` 时列 7（补偿值）不可编辑。
//   D6. `vstAntiPlugActionGetText` 列 1（原 :951-963）：18 种"设置按钮"模式显示 '设置'，
//       其余显示 `g_Config.ActionList[Mode].nInterval`。
//   D7. 列 0 的文本（原 :944-947）：`>= amHitConcurrent` 时后缀 ' >'，否则 ' <'。
//   D8. `vstAntiPlugActionGetHint`（原 :1174-1204）列 0 只对 3 个并发模式给提示；
//       列 7 恒给 '补偿值小于或等于0表示关闭'；**列 6 的提示被原文注释掉了**（原 :1180-1186）。
//   D9. `btnSaveClick`（原 :768-776）先把两个 TrackBar 的值**下限夹到 3**，再判
//       `dwSpeedValue >= dwCollectCount` → 弹 '“超速次数”必须 < “总记录数”'（含全角引号）并 Exit。
//       ★ 差异点：`trckbrSpeedValue.Position` 若为 0 会被**提升到 3**再比较，
//         所以"超速次数=0、总记录数=3"竟然会**通过**校验并保存 speedValue=3、collectCount=3
//         —— 保存后二者相等，与校验条件冲突。这是原文缺陷，照抄并断言。
//   D10. `RefreshCtrlsStatus`（原 :719-723）里 `chkShowDropConcurrentLog` 的赋值被注释掉
//        （原 :720），故该复选框**永远是 DFM 的初值**（.dfm 里未设 Checked → False）。
//   D11. `btnDefaultClick`（原 :1164-1172）整个函数体被注释掉 → **空实现**（保留为空方法）。
//   D12. `chkShowDropConcurrentLogClick`（原 :1218-1222）也把赋值注释掉 → 只 `SetSaveStatus(True)`。
//   D13. `vstAntiPlugActionGetText` 列 8 恒为 `' '`（单个空格，原 :1003），列 5 也是 `' '`（原 :994）；
//        这两个"提示/调试"列实际靠 `vstAntiPlugActionDrawText` 画 ImageList 图片（原 :1022-1032）。
//   D14. `PrepareEdit` 列 7 的 SpinEdit 是 `MaxValue := 0; MinValue := 0;`（原 :562-563）
//        —— 与 DFM 中 84 个 SpinEdit 同款"0/0 且不裁剪"语义；但列 4 是 `MinValue=0 MaxValue=30000`（原 :434-438）。
//   D15. `WM_STARTEDITING = WM_USER + 778`（原 :12）；uFrmMagicCD 用的是 +779。
// =====================================================================================

/// <summary>`vstAntiPlugAction` 的列号（0..8，与 .dfm Columns 的 Position 一致）。</summary>
public static class GameSpeedColumn
{
    public const int Enabled = 0;        // WideText='是否控制'  Width=112
    public const int Interval = 1;       // WideText='间隔'
    public const int ProcessMode = 2;    // WideText='超速处理方式' Width=120
    public const int SumProcessMode = 3;// WideText='累计超速处理'
    public const int FloatingInterval = 4; // WideText='浮动间隔'
    public const int ShowHint = 5;       // WideText='超速提示'
    public const int HintText = 6;       // WideText='超速提示信息'
    public const int CompensationValue = 7; // WideText='补偿值'
    public const int Debug = 8;          // WideText='调试'
    public const int Count = 9;
}

/// <summary>`btnSaveClick` 的校验结果。</summary>
public enum GameSpeedSaveResult
{
    OK = 0,
    SpeedValueNotLessThanCollectCount = 1   // 原 :776-781
}

/// <summary>`PrepareEdit` 的编辑器规格（原 :385-572 的结果面）。</summary>
public class EditorSpec
{
    /// <summary>编辑器种类（原：TButton / TSpinEditEx / TComboBox / TEdit）。</summary>
    public EditorKind Kind = EditorKind.None;

    /// <summary>下拉项的原始枚举值（原文 `Items.Objects[TempValue]`，即 `TObject(ProcessMode)`）。
    /// `PrepareEditSpec` 会填好，测试/编辑结束时可直接用。</summary>
    public List<int> ItemRawValues = new List<int>();

    /// <summary>当前选中项对应的原始枚举值（无选中时 = -1）。</summary>
    public int ItemIndexRaw
    {
        get
        {
            if (ItemIndex < 0 || ItemIndex >= ItemRawValues.Count) return -1;
            return ItemRawValues[ItemIndex];
        }
        set
        {
            for (int i = 0; i < ItemRawValues.Count; i++)
            {
                if (ItemRawValues[i] == value) { ItemIndex = i; return; }
            }
        }
    }

    /// <summary>列的 DFM Min/Max（SpinEdit 用）。</summary>
    public int MinValue;
    public int MaxValue;

    /// <summary>初始值（SpinEdit 用）。</summary>
    public int Value;

    /// <summary>下拉项文本（ComboBox 用）。</summary>
    public List<string> Items = new List<string>();

    /// <summary>下拉选中项（ComboBox 用；-1 = 无）。</summary>
    public int ItemIndex = -1;

    /// <summary>TEdit 用初始文本。</summary>
    public string Text = "";
}

/// <summary>编辑器种类。</summary>
public enum EditorKind
{
    None = 0,
    Button = 1,
    SpinEdit = 2,
    ComboBox = 3,
    Edit = 4
}

/// <summary>uFrmGameSpeed.pas 的非 UI 逻辑（可单测）。</summary>
public static class GameSpeedLogic
{
    // ---------------------------------------------------------------------------------
    // 原 :392-401（PrepareEdit 列 1）与 :605-614（OnBtnIntervalClick）共用的 18 种模式。
    // 两处列表**完全相同**；一旦不一致就会出现"显示了按钮但点了没反应"。
    // ---------------------------------------------------------------------------------
    public static readonly TAntiPlugActionMode[] IntervalButtonModes =
    {
        TAntiPlugActionMode.amHit, TAntiPlugActionMode.amSpell,
        TAntiPlugActionMode.amWalk, TAntiPlugActionMode.amRun,
        TAntiPlugActionMode.amWalkToHit, TAntiPlugActionMode.amHitToWalk,
        TAntiPlugActionMode.amRunToHit, TAntiPlugActionMode.amHitToRun,
        TAntiPlugActionMode.amWalkToSpell, TAntiPlugActionMode.amSpellToWalk,
        TAntiPlugActionMode.amRunToSpell, TAntiPlugActionMode.amSpellToRun,
        TAntiPlugActionMode.amTurnToHit, TAntiPlugActionMode.amTurnToSpell,
        TAntiPlugActionMode.amCutMeatToHit, TAntiPlugActionMode.amCutMeatToSpell,
        TAntiPlugActionMode.amTurnToMove, TAntiPlugActionMode.amCutMeatToMove
    };

    /// <summary>原 :470-471 (a) 组的 12 种模式（走 apmRebound..High）。</summary>
    public static readonly TAntiPlugActionMode[] ReboundGroupModes =
    {
        TAntiPlugActionMode.amHit, TAntiPlugActionMode.amSpell,
        TAntiPlugActionMode.amWalkToHit, TAntiPlugActionMode.amRunToHit,
        TAntiPlugActionMode.amWalkToSpell, TAntiPlugActionMode.amRunToSpell,
        TAntiPlugActionMode.amCutMeatToHit, TAntiPlugActionMode.amCutMeatToSpell,
        TAntiPlugActionMode.amMoveToTurn, TAntiPlugActionMode.amTurnToMove,
        TAntiPlugActionMode.amMoveToCutMeat, TAntiPlugActionMode.amCutMeatToMove
    };

    /// <summary>原 :482 的 3 个并发模式（走 apmOffline..High + Names2）。</summary>
    public static readonly TAntiPlugActionMode[] ConcurrentModes =
    {
        TAntiPlugActionMode.amHitConcurrent, TAntiPlugActionMode.amSpellConcurrent,
        TAntiPlugActionMode.amMoveConcurrent
    };

    /// <summary>原 :998 列 7 只对这 3 个模式显示补偿值。</summary>
    public static readonly TAntiPlugActionMode[] CompensationModes =
    {
        TAntiPlugActionMode.amHit, TAntiPlugActionMode.amWalk, TAntiPlugActionMode.amRun
    };

    /// <summary>原 :658 `for ActionMode := Low(TAntiPlugActionMode) to amCutMeatToMove` → 0..23（共 24 行）。</summary>
    public const int EditableRowCount = (int)TAntiPlugActionMode.amCutMeatToMove + 1;   // 24

    /// <summary>原 :1232 / :685 的"并发"分界线。</summary>
    public static bool IsConcurrent(TAntiPlugActionMode mode) => mode >= TAntiPlugActionMode.amHitConcurrent;

    /// <summary>原 :392-401 / :605-614 `ActionMode in [...]`。</summary>
    public static bool IntervalCellIsButton(TAntiPlugActionMode mode)
        => Array.IndexOf(IntervalButtonModes, mode) >= 0;

    /// <summary>原 :951-963 `vstAntiPlugActionGetText` 列 1。
    /// 命中 18 种模式 → '设置'；否则 `IntToStr(nInterval)`。</summary>
    public static string IntervalCellText(TAntiPlugActionMode mode)
        => IntervalCellIsButton(mode) ? "设置" : DelphiRTL.IntToStr((int)Config().ActionList[(int)mode].nInterval);

    /// <summary>原 :941-1004 `vstAntiPlugActionGetText` 的完整取文本（0..8 列）。</summary>
    public static string DescribeCell(TAntiPlugActionMode mode, int column)
    {
        var action = Config().ActionList[(int)mode];
        switch (column)
        {
            case GameSpeedColumn.Enabled:       // 原 :942-948
                return RunGateConst.AntiPlugActionModeNames[(int)mode] + (IsConcurrent(mode) ? " >" : " <");
            case GameSpeedColumn.Interval:      // 原 :949-964
                return IntervalCellText(mode);
            case GameSpeedColumn.ProcessMode:   // 原 :965-971
                return IsConcurrent(mode)
                    ? RunGateConst.ActionProcessModeNames2[(int)action.ProcessMode]      // 原 :968
                    : RunGateConst.ActionProcessModeNames[(int)action.ProcessMode];      // 原 :970
            case GameSpeedColumn.SumProcessMode: // 原 :972-975
                return RunGateConst.SumActionProcessModeNames[(int)action.SumProcessMode];
            case GameSpeedColumn.FloatingInterval: // 原 :994（列 5 在 gettext 里是 ' '）
                return " ";
            case GameSpeedColumn.ShowHint:      // 原 :994 `5: CellText := ' ';`
                return " ";
            case GameSpeedColumn.HintText:      // 原 :995
                return action.sHintText;
            case GameSpeedColumn.CompensationValue:  // 原 :996-1002
                return Array.IndexOf(CompensationModes, mode) >= 0
                    ? DelphiRTL.IntToStr(action.nCompensationValue)
                    : " ";
            case GameSpeedColumn.Debug:         // 原 :1003 `8: CellText := ' ';`
                return " ";
            default:
                return "";
        }
    }

    /// <summary>原 :1065-1079 `vstAntiPlugActionEditing`：某列能否编辑。</summary>
    public static bool IsEditable(TAntiPlugActionMode mode, int column)
    {
        if (IsConcurrent(mode) && (column == GameSpeedColumn.Interval || column == GameSpeedColumn.ProcessMode))
            return false;                                                                    // 原 :1074-1075
        if (Array.IndexOf(CompensationModes, mode) < 0 && column == GameSpeedColumn.CompensationValue)
            return false;                                                                    // 原 :1076-1077
        return true;                                                                         // 原 :1070 Allowed := Node <> nil
    }

    /// <summary>原 :1224-1234 `vstAntiPlugActionChecking`：并发行不允许改勾选。</summary>
    public static bool CanToggleCheck(TAntiPlugActionMode mode) => !IsConcurrent(mode);      // 原 :1232

    /// <summary>原 :685-688 `RefreshCtrlsStatus`：并发行强制勾选。</summary>
    public static bool ShouldBeChecked(TAntiPlugActionMode mode)
        => Config().ActionList[(int)mode].boEnabled || IsConcurrent(mode);                   // 原 :685

    /// <summary>原 :1174-1204 `vstAntiPlugActionGetHint`（返回 "" 表示无提示）。
    /// ★ 列 6 的提示在原文被注释掉（原 :1180-1186），故此处**不返回**它。</summary>
    public static string GetHintText(TAntiPlugActionMode mode, int column)
    {
        if (column == GameSpeedColumn.Enabled)                                                // 原 :1187
        {
            switch (mode)
            {
                case TAntiPlugActionMode.amHitConcurrent:                                     // 原 :1192-1193
                    return "此项默认勾选，取消后游戏中将出现多倍攻击，需求封双倍请配合攻击间隔设置！";
                case TAntiPlugActionMode.amSpellConcurrent:                                   // 原 :1194-1195
                    return "此项默认勾选，取消后游戏中将出现多倍魔法，需求封双倍请配合攻击间隔设置！";
                case TAntiPlugActionMode.amMoveConcurrent:                                    // 原 :1196-1197
                    return "此项默认勾选，取消后游戏中可能会出现暗杀或飞机速度的玩家！";
            }
        }
        else if (column == GameSpeedColumn.CompensationValue)                                 // 原 :1200-1203
        {
            return "补偿值小于或等于0表示关闭";
        }
        return "";
    }

    /// <summary>
    /// 原 :470-509（PrepareEdit 列 2）的下拉构造 —— **原文最易错的一段**。
    /// 分三套：
    ///   (a) ReboundGroupModes → apmRebound..apmNoProcess，名称 Names，ItemIndex = Ord - Ord(apmRebound)
    ///   (b) ConcurrentModes   → apmOffline..apmNoProcess，名称 Names2，ItemIndex = Ord - Ord(apmOffline)
    ///   (c) 其余              → apmDelay..apmNoProcess **跳 apmFakeAttackPass**，名称 Names，
    ///                           ItemIndex = Ord（apmNoProcess 时 Ord - 1）
    /// `rawValues` 对应原文 `Items.Objects[i]`（即 `TObject(ProcessMode)`）。
    /// </summary>
    public static void BuildProcessModeItems(TAntiPlugActionMode mode, TActionProcessMode current,
                                            List<string> items, List<int> rawValues, out int itemIndex)
    {
        items.Clear();
        rawValues.Clear();
        itemIndex = -1;

        if (Array.IndexOf(ReboundGroupModes, mode) >= 0)                                       // 原 :470-481
        {
            for (var pm = TActionProcessMode.apmRebound; pm <= TActionProcessMode.apmNoProcess; pm++)   // 原 :473
            {
                items.Add(RunGateConst.ActionProcessModeNames[(int)pm]);                        // 原 :475
                rawValues.Add((int)pm);                                                         // 原 :475 AddObject
                if (current == pm)                                                              // 原 :476
                    itemIndex = (int)pm - (int)TActionProcessMode.apmRebound;                   // 原 :478
            }
            return;
        }

        if (Array.IndexOf(ConcurrentModes, mode) >= 0)                                          // 原 :482-492
        {
            for (var pm = TActionProcessMode.apmOffline; pm <= TActionProcessMode.apmNoProcess; pm++)   // 原 :484
            {
                items.Add(RunGateConst.ActionProcessModeNames2[(int)pm]);                       // 原 :486
                rawValues.Add((int)pm);
                if (current == pm)                                                              // 原 :487
                    itemIndex = (int)pm - (int)TActionProcessMode.apmOffline;                   // 原 :489
            }
            return;
        }

        for (var pm = TActionProcessMode.apmDelay; pm <= TActionProcessMode.apmNoProcess; pm++) // 原 :495
        {
            if (pm == TActionProcessMode.apmFakeAttackPass) continue;                           // 原 :497
            items.Add(RunGateConst.ActionProcessModeNames[(int)pm]);                            // 原 :499
            rawValues.Add((int)pm);
            if (current == pm)                                                                  // 原 :500
            {
                if (pm == TActionProcessMode.apmNoProcess)                                      // 原 :502
                    itemIndex = (int)pm - 1;                                                    // 原 :503
                else
                    itemIndex = (int)pm;                                                        // 原 :505
            }
        }
    }

    /// <summary>原 :525-532（PrepareEdit 列 3）：`sapmNone .. sampLockUser`，ItemIndex = Ord。</summary>
    public static void BuildSumProcessModeItems(TSumActionProcessMode current, List<string> items,
                                                List<int> rawValues, out int itemIndex)
    {
        items.Clear();
        rawValues.Clear();
        itemIndex = -1;
        for (var sm = TSumActionProcessMode.sapmNone; sm <= TSumActionProcessMode.sampLockUser; sm++)   // 原 :525
        {
            items.Add(RunGateConst.SumActionProcessModeNames[(int)sm]);                                  // 原 :527
            rawValues.Add((int)sm);
            if (current == sm) itemIndex = (int)sm;                                                      // 原 :530
        }
    }

    /// <summary>
    /// 原 :385-572 `TPropertyEditLink.PrepareEdit`：列 → 编辑器规格。
    /// 列 1/4 → SpinEdit（列 1 在 18 种模式下改为 Button）；
    /// 列 2/3 → ComboBox；列 6 → TEdit；列 7 → SpinEdit(0..0)；其余 → Result := False（无编辑器）。
    /// </summary>
    public static EditorSpec PrepareEditSpec(TAntiPlugActionMode mode, int column)
    {
        var spec = new EditorSpec();
        var action = Config().ActionList[(int)mode];

        switch (column)
        {
            case 1:
            case 4:
                if (column == 1 && IntervalCellIsButton(mode))                                   // 原 :390-417
                {
                    spec.Kind = EditorKind.Button;
                    return spec;
                }
                spec.Kind = EditorKind.SpinEdit;                                                 // 原 :420
                spec.MaxValue = int.MaxValue;                                                    // 原 :429 MaxValue := High(Integer)
                if (column == 1) spec.MinValue = 1;                                              // 原 :431-432
                else if (column == 4) { spec.MinValue = 0; spec.MaxValue = 30000; }               // 原 :434-438
                else spec.MinValue = 1;                                                          // 原 :446-447
                if (column == 1) spec.Value = (int)action.nInterval;                              // 原 :450
                else if (column == 4) spec.Value = 0;                                             // 原 :451（注释掉的 nFloatingInterval）
                return spec;

            case 2:
                spec.Kind = EditorKind.ComboBox;                                                  // 原 :460-514
                BuildProcessModeItems(mode, action.ProcessMode, spec.Items, spec.ItemRawValues, out int idx2);
                spec.ItemIndex = idx2;
                return spec;

            case 3:
                spec.Kind = EditorKind.ComboBox;                                                  // 原 :515-537
                BuildSumProcessModeItems(action.SumProcessMode, spec.Items, spec.ItemRawValues, out int idx3);
                spec.ItemIndex = idx3;
                return spec;

            case 6:
                spec.Kind = EditorKind.Edit;                                                      // 原 :538-552
                spec.Text = action.sHintText;
                return spec;

            case 7:
                spec.Kind = EditorKind.SpinEdit;                                                  // 原 :553-568
                spec.MaxValue = 0;                                                                // 原 :562
                spec.MinValue = 0;                                                                // 原 :563
                spec.Value = action.nCompensationValue;                                           // 原 :564
                return spec;

            default:
                spec.Kind = EditorKind.None;                                                      // 原 :569-570 Result := False
                return spec;
        }
    }

    /// <summary>
    /// 原 :258-361 `TPropertyEditLink.EndEdit`：把编辑器结果写回 g_Config，返回"是否发生变更"。
    /// 变更时调用方应 `SetSaveStatus(True)`。
    /// </summary>
    public static bool ApplyEditorResult(TAntiPlugActionMode mode, int column, EditorSpec spec, string editText)
    {
        var action = Config().ActionList[(int)mode];
        switch (spec.Kind)
        {
            case EditorKind.ComboBox:
                if (column == 2)                                                                  // 原 :274-283
                {
                    // 原 :277 ProcessMode := TActionProcessMode(Items.Objects[TempValue])
                    var newMode = (TActionProcessMode)spec.ItemIndexRaw;
                    if (action.ProcessMode != newMode) { action.ProcessMode = newMode; return true; }
                }
                else if (column == 3)                                                              // 原 :284-292
                {
                    var newSum = (TSumActionProcessMode)spec.Value;                                // 原 :286 TSumActionProcessMode(TempValue)
                    if (action.SumProcessMode != newSum) { action.SumProcessMode = newSum; return true; }
                }
                return false;

            case EditorKind.SpinEdit:
                int tempValue = spec.Value;                                                        // 原 :297
                switch (column)
                {
                    case 1:                                                                        // 原 :299-304
                        if ((int)action.nInterval != tempValue) { action.nInterval = (uint)tempValue; return true; }
                        return false;
                    case 7:                                                                        // 原 :328-333
                        if (action.nCompensationValue != tempValue) { action.nCompensationValue = tempValue; return true; }
                        return false;
                    default:
                        return false;                                                              // 原 :306-326 被注释掉的列 4/5/6
                }

            case EditorKind.Edit:
                if (column == 6)                                                                   // 原 :339-346
                {
                    if (!SameText(action.sHintText, editText)) { action.sHintText = editText; return true; }
                }
                return false;

            default:
                return false;
        }
    }

    /// <summary>原 :368 `not SameText(...)`（SysUtils.SameText：忽略首尾空白 + 大小写不敏感）。</summary>
    public static bool SameText(string a, string b)
        => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 原 :768-781 `btnSaveClick` 的两个 TrackBar 下限夹取与校验。
    /// 返回校验结果，并通过 out 参数给出夹取后的 (collectCount, speedValue)。
    /// </summary>
    public static GameSpeedSaveResult ValidateSpeedValue(int trackbarCollectCount, int trackbarSpeedValue,
                                                        out int collectCount, out int speedValue)
    {
        collectCount = trackbarCollectCount;                               // 原 :768
        if (collectCount < 3) collectCount = 3;                            // 原 :769-770

        speedValue = trackbarSpeedValue;                                   // 原 :772
        if (speedValue < 3) speedValue = 3;                                // 原 :773-774

        if (speedValue >= collectCount)                                    // 原 :776
            return GameSpeedSaveResult.SpeedValueNotLessThanCollectCount;  // 原 :778 弹窗 + Exit
        return GameSpeedSaveResult.OK;
    }

    /// <summary>原 :778 的错误提示文本（含全角引号，逐字照抄）。</summary>
    public const string SpeedValueErrorText = "“超速次数”必须 < “总记录数”";

    /// <summary>原 :725 / :748 的 lblSpeedValue 文本。
    /// ★ 两处格式**不同**：RefreshCtrlsStatus 用 `'[%d/%d]'`（原 :725，无空格），
    ///   trckbrSpeedValueChange 用 `'[%d/ %d]'`（原 :748，斜杠后**多一个空格**）—— 差异断言点。</summary>
    public static string SpeedValueLabelRefresh(int speedValue, int collectCount)
        => "[" + speedValue + "/" + collectCount + "]";                    // 原 :725 Format('[%d/%d]', ...)

    public static string SpeedValueLabelOnChange(int speedValue, int collectCount)
        => "[" + speedValue + "/ " + collectCount + "]";                   // 原 :748 Format('[%d/ %d]', ...)

    /// <summary>原 :757-855 `btnSaveClick` 的 INI 落盘（`[Section]` 10 键 + `[Setup]` 24 键）。</summary>
    public static void Save(string iniFileName)
    {
        var cfg = Config();
        var ini = new TIniFileEx(iniFileName);                                    // 原 :788

        for (int i = 0; i < RunGateConst.ActionModeCount; i++)                    // 原 :790
        {
            string section = RunGateConst.AntiPlugActionModeSections[i];          // 原 :792
            var action = cfg.ActionList[i];
            ini.WriteBool(section, "Enabled", action.boEnabled ? (byte)1 : (byte)0);              // 原 :795
            ini.WriteInteger(section, "Interval", (int)action.nInterval);                         // 原 :796
            ini.WriteInteger(section, "ProcessMode", (int)action.ProcessMode);                    // 原 :797
            ini.WriteBool(section, "ProcessScript", action.boProcessScript ? (byte)1 : (byte)0);  // 原 :798
            ini.WriteInteger(section, "SumProcessMode", (int)action.SumProcessMode);              // 原 :799
            // 原 :800-802 FloatingInterval / CollectCount / CollectSpeedCount 被注释掉
            ini.WriteBool(section, "ShowHint", action.boShowHint ? (byte)1 : (byte)0);            // 原 :803
            ini.WriteString(section, "HintText", action.sHintText);                               // 原 :804
            ini.WriteInteger(section, "CompensationValue", action.nCompensationValue);            // 原 :805
            ini.WriteBool(section, "Debug", action.boDebug ? (byte)1 : (byte)0);                  // 原 :806
        }

        ini.WriteInteger("Setup", "LockTime", cfg.nLockTime);                                                     // 原 :809
        ini.WriteBool("Setup", "SaveLockStatus", cfg.boSaveLockStatus ? (byte)1 : (byte)0);                       // 原 :810
        ini.WriteBool("Setup", "ShowLockLog", cfg.boShowLockLog ? (byte)1 : (byte)0);                             // 原 :811
        ini.WriteString("Setup", "ShowLockMsg", cfg.sShowLockMsg);                                                // 原 :812
        ini.WriteBool("Setup", "SpeedClearData", cfg.boSpeedClearData ? (byte)1 : (byte)0);                       // 原 :814
        ini.WriteInteger("Setup", "MsgType", cfg.btMsgType);                                                      // 原 :816
        ini.WriteInteger("Setup", "MsgFColor", cfg.btMsgFColor);                                                  // 原 :817
        ini.WriteInteger("Setup", "MsgBColor", cfg.btMsgBColor);                                                  // 原 :818
        ini.WriteInteger("Setup", "UserShopSearchInterval", (int)cfg.dwUserShop_Search_Interval);                 // 原 :820
        ini.WriteBool("Setup", "UserShopSearchShowHint", cfg.boUserShop_Search_ShowHint ? (byte)1 : (byte)0);     // 原 :821
        ini.WriteInteger("Setup", "UserShopBuyInterval", (int)cfg.dwUserShop_Buy_Interval);                       // 原 :823
        ini.WriteBool("Setup", "UserShopBuyShowHint", cfg.boUserShop_Buy_ShowHint ? (byte)1 : (byte)0);           // 原 :824
        ini.WriteInteger("Setup", "TakeOnItemInterval", (int)cfg.dwTakeOn_Item_Interval);                         // 原 :826
        ini.WriteBool("Setup", "TakeOnItemShowHint", cfg.boTakeOn_Item_ShowHint ? (byte)1 : (byte)0);             // 原 :827
        ini.WriteInteger("Setup", "DealTryAttackInterval", (int)cfg.dwDealTry_Attack_Interval);                   // 原 :829
        ini.WriteBool("Setup", "DealTryAttackShowHint", cfg.boDealTry_Attack_ShowHint ? (byte)1 : (byte)0);       // 原 :830
        ini.WriteInteger("Setup", "BrutalAttackInterval", (int)cfg.dwBrutal_Attack_Interval);                     // 原 :832
        ini.WriteBool("Setup", "BrutalAttackShowHint", cfg.boBrutal_Attack_ShowHint ? (byte)1 : (byte)0);         // 原 :833
        ini.WriteBool("Setup", "ShowAttackLog", cfg.boShowAttackLog ? (byte)1 : (byte)0);                         // 原 :835
        // 原 :836 ShowDropConcurrentLog 被注释掉
        ini.WriteInteger("Setup", "ContinueSpeedPassIncTime", (int)cfg.dwContinueSpeedPassIncTime);               // 原 :837
        ini.WriteInteger("Setup", "CollectCount", cfg.dwCollectCount);                                            // 原 :839
        ini.WriteInteger("Setup", "SpeedValue", cfg.dwSpeedValue);                                                // 原 :840
        ini.WriteBool("Setup", "ContinueSpeedCloseSocket", cfg.boContinueSpeedCloseSocket ? (byte)1 : (byte)0);   // 原 :842
        ini.WriteInteger("Setup", "ContinueSpeedCount", cfg.nContinueSpeedCount);                                 // 原 :843
        ini.WriteInteger("Setup", "SumSpeedCheckTime", cfg.nSumSpeedCheckTime);                                   // 原 :845
        ini.WriteInteger("Setup", "SumSpeedMaxCount", cfg.nSumSpeedMaxCount);                                     // 原 :846
        ini.WriteBool("Setup", "ZeroCompensationValueClearPool", cfg.boZeroCompensationValueClearPool ? (byte)1 : (byte)0);  // 原 :848
        ini.WriteInteger("Setup", "ClientUploadPickItemsTime", cfg.dwClientUploadPickItemsTime);                  // 原 :850

        ini.Dispose();                                                            // 原 :852 IniFile.Free
    }

    /// <summary>当前配置（GateShare.pas `g_Config` 的接缝）。</summary>
    private static TAntiPlugConfig Config() => FormGlobals.g_Config;
}

/// <summary>原 :619-647 `function ShowFrmGameSpeed(MainForm: TForm; RunGateManager: TRunGateManager): Boolean;`。</summary>
public static class GameSpeedUnit
{
    public static bool ShowFrmGameSpeed(Form mainForm)
    {
        using var form = new FrmGameSpeed();
        if (mainForm != null)                                                  // 原 :629-630
        {
            form.Left = mainForm.Left + (mainForm.Width - form.Width) / 2;
            form.Top = mainForm.Top + (mainForm.Height - form.Height) / 2;
        }
        form.RefreshCtrlsStatus();                                             // 原 :638
        return form.ShowDialog() == DialogResult.OK;                           // 原 :643
    }
}

/// <summary>原 uFrmGameSpeed.pas:15-146 `TFrmGameSpeed`（DFM: uFrmGameSpeed.dfm）。
/// DFM 几何：Left=219 Top=171 BorderStyle=bsDialog Caption='外挂控制'
///           ClientHeight=589 ClientWidth=761 Font.Charset=GB2312_CHARSET Font.Height=-12 Font.Name='宋体'
///           Position=poMainFormCenter PixelsPerInch=96</summary>
public class FrmGameSpeed : Form
{
    // ---- 顶行（GroupBox1 之外）----
    public Label lbl7, lbl10, lbl11;
    // DFM: GroupBox1 Left=8 Top=8 Width=744 Height=383 Caption='参数设置' TabOrder=0
    public GroupBox GroupBox1;
    public Label Label3;                 // DFM: Label3（GroupBox1 内）
    public TVirtualStringTreeStub vstAntiPlugAction;  // DFM: vstAntiPlugAction Left=7 Top=18 Width=112(+列) Height=358
    public Button btnSave;               // DFM: btnSave Left=607 Top=558 Width=70 Height=23 Caption='保存(&S)'
    public Button btnClose;              // DFM: btnClose Left=682 Top=558 Width=70 Height=23 Caption='关闭(&E)'

    // DFM: GroupBox2 Left=8 Top=396 Width=252 Height=65 Caption='提示设置' TabOrder=1
    public GroupBox GroupBox2;
    public Label lbl2, Label1, lbl1, lbl4;
    public TColorIndexEdit seFColor;     // DFM: seFColor Left=39 Top=39 Width=60 Height=21 (TColorIndexEdit)
    public TColorIndexEdit seBColor;     // DFM: seBColor Left=39 Top=16 Width=60 Height=21 (TColorIndexEdit)
    public ComboBox cbbMsgType;          // DFM: cbbMsgType Left=166 Top=16 Width=80 Height=20 Text='密人提示'
    public TextBox edtPreview;           // DFM: edtPreview Left=166 Top=39 Width=79 Height=20 Text='提示文字预览'

    // DFM: GroupBox3 Left=265 Top=396 Width=297 Height=65 Caption='锁定设置' TabOrder=2
    public GroupBox GroupBox3;
    public Label Label6, Label7;         // DFM: Label6 / Label7
    public Label lbl3;                   // DFM: lbl3
    public TSpinEditEx seLockTime;       // DFM: seLockTime Left=63 Top=15 Width=50 Height=21
    public CheckBox chkSaveLockStatus;   // DFM: chkSaveLockStatus Left=141 Top=18 Width=67 Height=17 Caption='保存状态'
    public CheckBox chkShowLockLog;      // DFM: chkShowLockLog Left=223 Top=18 Width=66 Height=17 Caption='显示日志'
    public TextBox edtShowLockMsg;       // DFM: edtShowLockMsg Left=63 Top=38 Width=226 Height=21

    // DFM: GroupBox5 Left=265 Top=464 Width=297 Height=85 Caption='间隔设置 [毫秒]' TabOrder=3
    public GroupBox GroupBox5;
    public Label Label5, Label8, Label4, Label10, Label2;
    public TSpinEditEx seDealTryAttackTime;   // DFM: seDealTryAttackTime Left=232 Top=15 Width=44 Height=21
    public TSpinEditEx seBrutalAttackTime;    // DFM: seBrutalAttackTime Left=232 Top=37 Width=44 Height=21
    public CheckBox chkDealTryAttackHint;     // DFM: chkDealTryAttackHint Left=277 Top=17 Width=12 Height=17
    public CheckBox chkBrutalAttackHint;      // DFM: chkBrutalAttackHint Left=277 Top=39 Width=15 Height=17
    public TSpinEditEx seUserShopSearchTime;  // DFM: seUserShopSearchTime Left=86 Top=14 Width=44 Height=21
    public CheckBox chkUserShopSearchHint;    // DFM: chkUserShopSearchHint Left=132 Top=16 Width=16 Height=17
    public TSpinEditEx seUserShopBuyTime;     // DFM: seUserShopBuyTime Left=86 Top=36 Width=44 Height=21
    public CheckBox chkUserShopBuyHint;       // DFM: chkUserShopBuyHint Left=132 Top=38 Width=16 Height=17
    public TSpinEditEx seTakeOnItemTime;      // DFM: seTakeOnItemTime Left=86 Top=58 Width=44 Height=21
    public CheckBox chkTakeOnItemHint;        // DFM: chkTakeOnItemHint Left=132 Top=60 Width=15 Height=17
    public CheckBox chkZeroCompensationValueClearPool; // DFM: Left=183 Top=61 Width=107 Height=17 Caption='0'补偿时清补偿池

    // DFM: grp2 Left=8 Top=464 Width=252 Height=85 Caption='其他设置' TabOrder=4
    public GroupBox grp2;
    public Label lbl5, Label9;
    public TSpinEditEx seContinueSpeedPassIncTime; // DFM: seContinueSpeedPassIncTime Left=133 Top=14 Width=83 Height=21
    public CheckBox chkSpeedClearData;             // DFM: chkSpeedClearData Left=9 Top=38 Width=237 Height=17
    public CheckBox chkShowAttackLog;              // DFM: chkShowAttackLog Left=9 Top=60 Width=94 Height=17 Caption='显示超速日志'
    public CheckBox chkShowDropConcurrentLog;      // DFM: chkShowDropConcurrentLog Left=129 Top=84 Width=115 Height=17

    // DFM: GroupBox6 Left=566 Top=396 Width=187 Height=65 Caption='加速规则控制' TabOrder=5
    public GroupBox GroupBox6;
    public Label lblSpeedValue;    // DFM: lblSpeedValue（GroupBox6 内）
    public Label lbl9, Label12;
    public TrackBar trckbrSpeedValue;    // DFM: trckbrSpeedValue Left=64 Top=36 Width=118 Height=22
    public TrackBar trckbrCollectCount;  // DFM: trckbrCollectCount Left=64 Top=15 Width=118 Height=22

    // DFM: GroupBox4 Left=566 Top=464 Width=187 Height=60 Caption='累计超速规则' TabOrder=6
    public GroupBox GroupBox4;
    public Label lbl6, Label11, lbl8;
    public TSpinEditEx seSumSpeedCheckTime;  // DFM: seSumSpeedCheckTime Left=87 Top=13 Width=77 Height=21
    public TSpinEditEx seSumSpeedMaxCount;   // DFM: seSumSpeedMaxCount Left=87 Top=35 Width=93 Height=21
    public CheckBox chkContinueSpeedCloseSocket; // DFM: chkContinueSpeedCloseSocket Left=568 Top=533 Width=70 Height=17 Caption='连续超速'
    public TSpinEditEx seContinueSpeedCount;     // DFM: seContinueSpeedCount Left=637 Top=531 Width=47 Height=21
    public TSpinEditEx seClientUploadPickItemsTime; // DFM: seClientUploadPickItemsTime Left=150 Top=555 Width=59 Height=21
    public ImageList ilCheck;                    // DFM: ilCheck Left=264 Top=120 Width=13 Height=13

    public FrmGameSpeed()
    {
        // DFM: FrmGameSpeed Caption='外挂控制' BorderStyle=bsDialog Position=poMainFormCenter
        Text = "外挂控制";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Location = new Point(219, 171);
        ClientSize = new Size(761, 589);
        Font = new Font("宋体", 9F);                       // Font.Height=-12 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: ilCheck Left=264 Top=120 Width=13 Height=13（ImageList：0=未勾 1=勾选）
        ilCheck = new ImageList { ImageSize = new Size(13, 13), ColorDepth = ColorDepth.Depth32Bit };
        ilCheck.Images.Add(NewCheckBitmap(false));   // index 0
        ilCheck.Images.Add(NewCheckBitmap(true));    // index 1

        // DFM: GroupBox1 Left=8 Top=8 Width=744 Height=383 Caption='参数设置' TabOrder=0
        GroupBox1 = new GroupBox { Left = 8, Top = 8, Width = 744, Height = 383, Text = "参数设置", TabIndex = 0 };
        // DFM: Label3（GroupBox1 内，具体坐标见 .dfm）
        Label3 = new Label { Left = 4, Top = 2, Width = 60, Height = 12, Text = "参数" };
        // DFM: vstAntiPlugAction Left=7 Top=18 Width=112 Height=358
        //       Columns: 是否控制(112) / 间隔 / 超速处理方式(120) / 累计超速处理 / 浮动间隔 /
        //                超速提示 / 超速提示信息 / 补偿值 / 调试
        //       Header.Height=24 Header.Options 含 hoVisible；DefaultNodeHeight=22
        vstAntiPlugAction = new TVirtualStringTreeStub { Left = 7, Top = 18, Width = 720, Height = 358, TabIndex = 0 };
        vstAntiPlugAction.DefaultNodeHeight = 22;      // DFM: DefaultNodeHeight=22
        vstAntiPlugAction.HeaderDefaultHeight = 24;    // DFM: Header.Height=24
        vstAntiPlugAction.ItemHeight = 22;
        vstAntiPlugAction.ColumnWidths.AddRange(new[] { 112, 60, 120, 90, 60, 60, 100, 60, 40 });
        vstAntiPlugAction.DrawNode += (s, e) => vstAntiPlugAction_DrawNode(s, e);
        vstAntiPlugAction.AfterSelect += (s, e) => vstAntiPlugAction_AfterSelect(s, e);
        vstAntiPlugAction.NodeMouseClick += (s, e) => vstAntiPlugAction_NodeClick(s, e);

        // DFM: btnSave Left=607 Top=558 Width=70 Height=23 Caption='保存(&S)'
        btnSave = new Button { Left = 607, Top = 558, Width = 70, Height = 23, Text = "保存(&S)" };
        // DFM: btnClose Left=682 Top=558 Width=70 Height=23 Caption='关闭(&E)'
        btnClose = new Button { Left = 682, Top = 558, Width = 70, Height = 23, Text = "关闭(&E)" };
        // DFM: lbl7 / lbl10 / lbl11（底行标签）
        lbl7 = new Label { Left = 8, Top = 560, Width = 60, Height = 12, Text = "连续超速：" };
        lbl10 = new Label { Left = 130, Top = 557, Width = 20, Height = 12, Text = "上传：" };
        lbl11 = new Label { Left = 210, Top = 557, Width = 60, Height = 12, Text = "秒" };

        // DFM: GroupBox2 Left=8 Top=396 Width=252 Height=65 Caption='提示设置' TabOrder=1
        GroupBox2 = new GroupBox { Left = 8, Top = 396, Width = 252, Height = 65, Text = "提示设置", TabIndex = 1 };
        lbl2 = new Label { Left = 8, Top = 20, Width = 30, Height = 12, Text = "背景：" };
        Label1 = new Label { Left = 8, Top = 43, Width = 30, Height = 12, Text = "前景：" };
        lbl1 = new Label { Left = 110, Top = 20, Width = 48, Height = 12, Text = "消息类型：" };
        lbl4 = new Label { Left = 110, Top = 43, Width = 48, Height = 12, Text = "预览：" };
        // DFM: seBColor Left=39 Top=16 Width=60 Height=21（TColorIndexEdit 背景色）
        seBColor = new TColorIndexEdit { Left = 39, Top = 16, Width = 60, Height = 21 };
        // DFM: seFColor Left=39 Top=39 Width=60 Height=21（TColorIndexEdit 前景色）
        seFColor = new TColorIndexEdit { Left = 39, Top = 39, Width = 60, Height = 21 };
        // DFM: cbbMsgType Left=166 Top=16 Width=80 Height=20 Text='密人提示'
        cbbMsgType = new ComboBox { Left = 166, Top = 16, Width = 80, Height = 20, Text = "密人提示",
                                    DropDownStyle = ComboBoxStyle.DropDownList };
        cbbMsgType.Items.AddRange(new object[] { "密人提示", "屏幕提示" });
        // DFM: edtPreview Left=166 Top=39 Width=79 Height=20 Text='提示文字预览'
        edtPreview = new TextBox { Left = 166, Top = 39, Width = 79, Height = 20, Text = "提示文字预览" };

        // DFM: GroupBox3 Left=265 Top=396 Width=297 Height=65 Caption='锁定设置' TabOrder=2
        GroupBox3 = new GroupBox { Left = 265, Top = 396, Width = 297, Height = 65, Text = "锁定设置", TabIndex = 2 };
        Label6 = new Label { Left = 8, Top = 18, Width = 48, Height = 12, Text = "锁定时间：" };
        Label7 = new Label { Left = 113, Top = 18, Width = 24, Height = 12, Text = "秒" };
        lbl3 = new Label { Left = 8, Top = 42, Width = 48, Height = 12, Text = "提示：" };
        // DFM: seLockTime Left=63 Top=15 Width=50 Height=21
        seLockTime = new TSpinEditEx { Left = 63, Top = 15, Width = 50, Height = 21 };
        // DFM: chkSaveLockStatus Left=141 Top=18 Width=67 Height=17 Caption='保存状态'
        chkSaveLockStatus = new CheckBox { Left = 141, Top = 18, Width = 67, Height = 17, Text = "保存状态" };
        // DFM: chkShowLockLog Left=223 Top=18 Width=66 Height=17 Caption='显示日志'
        chkShowLockLog = new CheckBox { Left = 223, Top = 18, Width = 66, Height = 17, Text = "显示日志" };
        // DFM: edtShowLockMsg Left=63 Top=38 Width=226 Height=21
        edtShowLockMsg = new TextBox { Left = 63, Top = 38, Width = 226, Height = 21 };

        // DFM: GroupBox5 Left=265 Top=464 Width=297 Height=85 Caption='间隔设置 [毫秒]' TabOrder=3
        GroupBox5 = new GroupBox { Left = 265, Top = 464, Width = 297, Height = 85,
                                   Text = "间隔设置 [毫秒]", TabIndex = 3 };
        Label5 = new Label { Left = 8, Top = 18, Width = 60, Height = 12, Text = "个人商店搜索：" };
        Label8 = new Label { Left = 8, Top = 40, Width = 60, Height = 12, Text = "购买：" };
        Label4 = new Label { Left = 8, Top = 62, Width = 60, Height = 12, Text = "穿戴：" };
        Label10 = new Label { Left = 152, Top = 18, Width = 78, Height = 12, Text = "交易到挑战：" };
        Label2 = new Label { Left = 152, Top = 40, Width = 78, Height = 12, Text = "野蛮到攻击：" };
        seUserShopSearchTime = new TSpinEditEx { Left = 86, Top = 14, Width = 44, Height = 21 };
        chkUserShopSearchHint = new CheckBox { Left = 132, Top = 16, Width = 16, Height = 17 };
        seUserShopBuyTime = new TSpinEditEx { Left = 86, Top = 36, Width = 44, Height = 21 };
        chkUserShopBuyHint = new CheckBox { Left = 132, Top = 38, Width = 16, Height = 17 };
        seTakeOnItemTime = new TSpinEditEx { Left = 86, Top = 58, Width = 44, Height = 21 };
        chkTakeOnItemHint = new CheckBox { Left = 132, Top = 60, Width = 15, Height = 17 };
        seDealTryAttackTime = new TSpinEditEx { Left = 232, Top = 15, Width = 44, Height = 21 };
        chkDealTryAttackHint = new CheckBox { Left = 277, Top = 17, Width = 12, Height = 17 };
        seBrutalAttackTime = new TSpinEditEx { Left = 232, Top = 37, Width = 44, Height = 21 };
        chkBrutalAttackHint = new CheckBox { Left = 277, Top = 39, Width = 15, Height = 17 };
        // DFM: chkZeroCompensationValueClearPool Left=183 Top=61 Width=107 Height=17 Caption="'0'补偿时清补偿池"
        chkZeroCompensationValueClearPool = new CheckBox { Left = 183, Top = 61, Width = 107, Height = 17,
                                                           Text = "'0'补偿时清补偿池" };

        // DFM: grp2 Left=8 Top=464 Width=252 Height=85 Caption='其他设置' TabOrder=4
        grp2 = new GroupBox { Left = 8, Top = 464, Width = 252, Height = 85, Text = "其他设置", TabIndex = 4 };
        lbl5 = new Label { Left = 8, Top = 18, Width = 120, Height = 12, Text = "连续超速放行增量：" };
        Label9 = new Label { Left = 218, Top = 18, Width = 24, Height = 12, Text = "毫秒" };
        seContinueSpeedPassIncTime = new TSpinEditEx { Left = 133, Top = 14, Width = 83, Height = 21 };
        // DFM: chkSpeedClearData Left=9 Top=38 Width=237 Height=17 Caption='当超速处理后，清空所有未处理的数据包'
        chkSpeedClearData = new CheckBox { Left = 9, Top = 38, Width = 237, Height = 17,
                                           Text = "当超速处理后，清空所有未处理的数据包" };
        // DFM: chkShowAttackLog Left=9 Top=60 Width=94 Height=17 Caption='显示超速日志'
        chkShowAttackLog = new CheckBox { Left = 9, Top = 60, Width = 94, Height = 17, Text = "显示超速日志" };
        // DFM: chkShowDropConcurrentLog Left=129 Top=84 Width=115 Height=17 Caption='显示多发并发日志'
        chkShowDropConcurrentLog = new CheckBox { Left = 129, Top = 84, Width = 115, Height = 17, Text = "显示多发并发日志" };

        // DFM: GroupBox6 Left=566 Top=396 Width=187 Height=65 Caption='加速规则控制' TabOrder=5
        GroupBox6 = new GroupBox { Left = 566, Top = 396, Width = 187, Height = 65, Text = "加速规则控制", TabIndex = 5 };
        lbl9 = new Label { Left = 8, Top = 18, Width = 54, Height = 12, Text = "超速次数：" };
        Label12 = new Label { Left = 8, Top = 40, Width = 54, Height = 12, Text = "总记录数：" };
        lblSpeedValue = new Label { Left = 8, Top = 58, Width = 170, Height = 12, Text = "" };
        // DFM: trckbrCollectCount Left=64 Top=15 Width=118 Height=22
        trckbrCollectCount = new TrackBar { Left = 64, Top = 15, Width = 118, Height = 22, Minimum = 0, Maximum = 100, TickStyle = TickStyle.None };
        // DFM: trckbrSpeedValue Left=64 Top=36 Width=118 Height=22
        trckbrSpeedValue = new TrackBar { Left = 64, Top = 36, Width = 118, Height = 22, Minimum = 0, Maximum = 100, TickStyle = TickStyle.None };

        // DFM: GroupBox4 Left=566 Top=464 Width=187 Height=60 Caption='累计超速规则' TabOrder=6
        GroupBox4 = new GroupBox { Left = 566, Top = 464, Width = 187, Height = 60, Text = "累计超速规则", TabIndex = 6 };
        lbl6 = new Label { Left = 8, Top = 17, Width = 78, Height = 12, Text = "统计时间间隔：" };
        Label11 = new Label { Left = 8, Top = 39, Width = 66, Height = 12, Text = "最大超速次数：" };
        lbl8 = new Label { Left = 166, Top = 13, Width = 18, Height = 12, Text = "秒" };
        seSumSpeedCheckTime = new TSpinEditEx { Left = 87, Top = 13, Width = 77, Height = 21 };
        seSumSpeedMaxCount = new TSpinEditEx { Left = 87, Top = 35, Width = 93, Height = 21 };
        // DFM: chkContinueSpeedCloseSocket Left=568 Top=533 Width=70 Height=17 Caption='连续超速'
        chkContinueSpeedCloseSocket = new CheckBox { Left = 568, Top = 533, Width = 70, Height = 17, Text = "连续超速" };
        // DFM: seContinueSpeedCount Left=637 Top=531 Width=47 Height=21
        seContinueSpeedCount = new TSpinEditEx { Left = 637, Top = 531, Width = 47, Height = 21 };
        // DFM: seClientUploadPickItemsTime Left=150 Top=555 Width=59 Height=21
        seClientUploadPickItemsTime = new TSpinEditEx { Left = 150, Top = 555, Width = 59, Height = 21 };

        GroupBox1.Controls.AddRange(new Control[] { Label3, vstAntiPlugAction });
        GroupBox2.Controls.AddRange(new Control[] { lbl2, Label1, lbl1, lbl4, seBColor, seFColor, cbbMsgType, edtPreview });
        GroupBox3.Controls.AddRange(new Control[] { Label6, Label7, lbl3, seLockTime, chkSaveLockStatus, chkShowLockLog, edtShowLockMsg });
        GroupBox5.Controls.AddRange(new Control[] { Label5, Label8, Label4, Label10, Label2,
                                                    seDealTryAttackTime, seBrutalAttackTime, chkDealTryAttackHint, chkBrutalAttackHint,
                                                    seUserShopSearchTime, chkUserShopSearchHint, seUserShopBuyTime, chkUserShopBuyHint,
                                                    seTakeOnItemTime, chkTakeOnItemHint, chkZeroCompensationValueClearPool });
        grp2.Controls.AddRange(new Control[] { lbl5, Label9, seContinueSpeedPassIncTime,
                                               chkSpeedClearData, chkShowAttackLog, chkShowDropConcurrentLog });
        GroupBox6.Controls.AddRange(new Control[] { lbl9, Label12, lblSpeedValue, trckbrSpeedValue, trckbrCollectCount });
        GroupBox4.Controls.AddRange(new Control[] { lbl6, Label11, lbl8, seSumSpeedCheckTime, seSumSpeedMaxCount });

        Controls.AddRange(new Control[] { lbl7, lbl10, lbl11, GroupBox1, btnSave, btnClose,
                                          GroupBox2, GroupBox3, GroupBox5, grp2, GroupBox6, GroupBox4,
                                          chkContinueSpeedCloseSocket, seContinueSpeedCount, seClientUploadPickItemsTime });

        // DFM 的事件接线
        Load += (s, e) => FormCreate(s, e);                             // OnCreate=FormCreate
        btnSave.Click += (s, e) => btnSave_Click(s, e);                 // OnClick=btnSaveClick
        btnClose.Click += (s, e) => btnClose_Click(s, e);               // OnClick=btnCloseClick
        edtShowLockMsg.TextChanged += (s, e) => edtShowLockMsg_Change(s, e);
        seFColor.ValueChanged += (s, e) => seFColor_Change(s, e);
        seBColor.ValueChanged += (s, e) => seBColor_Change(s, e);
        cbbMsgType.SelectedIndexChanged += (s, e) => cbbMsgType_Change(s, e);
        seLockTime.ValueChanged += (s, e) => seLockTime_Change(s, e);
        chkSaveLockStatus.Click += (s, e) => chkSaveLockStatus_Click(s, e);
        chkShowLockLog.Click += (s, e) => chkShowLockLog_Click(s, e);
        chkSpeedClearData.Click += (s, e) => chkSpeedClearData_Click(s, e);
        seDealTryAttackTime.ValueChanged += (s, e) => seDealTryAttackTime_Change(s, e);
        chkDealTryAttackHint.Click += (s, e) => chkDealTryAttackHint_Click(s, e);
        seBrutalAttackTime.ValueChanged += (s, e) => seBrutalAttackTime_Change(s, e);
        chkBrutalAttackHint.Click += (s, e) => chkBrutalAttackHint_Click(s, e);
        chkShowAttackLog.Click += (s, e) => chkShowAttackLog_Click(s, e);
        seContinueSpeedPassIncTime.ValueChanged += (s, e) => seContinueSpeedPassIncTime_Change(s, e);
        chkShowDropConcurrentLog.Click += (s, e) => chkShowDropConcurrentLog_Click(s, e);
        trckbrSpeedValue.ValueChanged += (s, e) => trckbrSpeedValue_Change(s, e);
        chkContinueSpeedCloseSocket.Click += (s, e) => chkContinueSpeedCloseSocket_Click(s, e);
        seContinueSpeedCount.ValueChanged += (s, e) => seContinueSpeedCount_Change(s, e);
        seUserShopSearchTime.ValueChanged += (s, e) => seUserShopSearchTime_Change(s, e);
        seUserShopBuyTime.ValueChanged += (s, e) => seUserShopBuyTime_Change(s, e);
        seTakeOnItemTime.ValueChanged += (s, e) => seTakeOnItemTime_Change(s, e);
        chkUserShopSearchHint.Click += (s, e) => chkUserShopSearchHint_Click(s, e);
        chkUserShopBuyHint.Click += (s, e) => chkUserShopBuyHint_Click(s, e);
        chkTakeOnItemHint.Click += (s, e) => chkTakeOnItemHint_Click(s, e);
        seSumSpeedCheckTime.ValueChanged += (s, e) => seSumCheckTime_Change(s, e);
        seSumSpeedMaxCount.ValueChanged += (s, e) => seSumSpeedMaxCount_Change(s, e);
        chkZeroCompensationValueClearPool.Click += (s, e) => chkZeroCompensationValueClearPool_Click(s, e);
        trckbrCollectCount.ValueChanged += (s, e) => trckbrCollectCount_Change(s, e);
    }

    /// <summary>原 uFrmGameSpeed.pas 未给 trckbrCollectCount 绑定 OnChange（DFM 中无 OnChange）；
    /// 但 `lblSpeedValue` 显示同时依赖两个 TrackBar，故托管侧在它变化时同步刷新标签与 TSpinEdit 语义。
    /// 原文 `trckbrSpeedValueChange`（原 :746-750）是唯一的 TrackBar 事件 —— 这里保持行为等价：
    /// collectCount 变化**只刷新标签**，不 SetSaveStatus（原文无此事件）。</summary>
    public void trckbrCollectCount_Change(object sender, EventArgs e)
    {
        lblSpeedValue.Text = GameSpeedLogic.SpeedValueLabelOnChange(trckbrSpeedValue.Value, trckbrCollectCount.Value);
    }

    private static Bitmap NewCheckBitmap(bool @checked)
    {
        var bmp = new Bitmap(13, 13);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        using var pen = new Pen(Color.Black);
        g.DrawRectangle(pen, 0, 0, 11, 11);
        if (@checked)
        {
            using var pen2 = new Pen(Color.Black, 2);
            g.DrawLine(pen2, 3, 6, 5, 9);
            g.DrawLine(pen2, 5, 9, 10, 3);
        }
        return bmp;
    }

    /// <summary>原 :649-673 `TFrmGameSpeed.FormCreate`：建 24 个节点（**到 amCutMeatToMove**，见 D1）。</summary>
    public void FormCreate(object sender, EventArgs e)
    {
        vstAntiPlugAction.Nodes.Clear();
        for (int i = 0; i < GameSpeedLogic.EditableRowCount; i++)            // 原 :658 `Low..amCutMeatToMove`
        {
            var mode = (TAntiPlugActionMode)i;
            var node = vstAntiPlugAction.AddChildMode(mode);                  // 原 :660 AddChild(nil)
            vstAntiPlugAction.SetCheckState(node, false);                     // 原 :662 CheckType := ctCheckBox
            // 原 :664-669 的 `if ActionMode >= amHitConcurrent then ... csCheckedNormal` 被注释掉
        }
    }

    /// <summary>原 :675-739 `TFrmGameSpeed.RefreshCtrlsStatus`。</summary>
    public void RefreshCtrlsStatus()
    {
        var node = vstAntiPlugAction.GetFirst();                             // 原 :680
        while (node != null)                                                 // 原 :681
        {
            var mode = vstAntiPlugAction.GetActionMode(node);
            if (mode.HasValue)
            {
                // 原 :685-688：`if boEnabled or (ActionMode >= amHitConcurrent) then csCheckedNormal else csUncheckedNormal`
                vstAntiPlugAction.SetCheckState(node, GameSpeedLogic.ShouldBeChecked(mode.Value));
            }
            node = vstAntiPlugAction.GetNext(node);                          // 原 :690
        }

        var cfg = FormGlobals.g_Config;
        seLockTime.Value = cfg.nLockTime;                                    // 原 :693
        chkSaveLockStatus.Checked = cfg.boSaveLockStatus;                    // 原 :694
        chkShowLockLog.Checked = cfg.boShowLockLog;                          // 原 :695
        edtShowLockMsg.Text = cfg.sShowLockMsg;                              // 原 :696
        chkSpeedClearData.Checked = cfg.boSpeedClearData;                    // 原 :698
        // WinForms 的 ComboBox.SelectedIndex 越界会抛异常；Delphi 的 ItemIndex 不会 → 夹取（等价）
        cbbMsgType.SelectedIndex = cbbMsgType.Items.Count == 0 ? -1 : Math.Max(0, Math.Min(cfg.btMsgType, cbbMsgType.Items.Count - 1));   // 原 :700
        seFColor.ColorIndex = cfg.btMsgFColor;                               // 原 :701
        seBColor.ColorIndex = cfg.btMsgBColor;                               // 原 :702
        seUserShopSearchTime.Value = (int)cfg.dwUserShop_Search_Interval;    // 原 :704
        chkUserShopSearchHint.Checked = cfg.boUserShop_Search_ShowHint;      // 原 :705
        seUserShopBuyTime.Value = (int)cfg.dwUserShop_Buy_Interval;          // 原 :707
        chkUserShopBuyHint.Checked = cfg.boUserShop_Buy_ShowHint;            // 原 :708
        seTakeOnItemTime.Value = (int)cfg.dwTakeOn_Item_Interval;            // 原 :710
        chkTakeOnItemHint.Checked = cfg.boTakeOn_Item_ShowHint;              // 原 :711
        seDealTryAttackTime.Value = (int)cfg.dwDealTry_Attack_Interval;      // 原 :713
        chkDealTryAttackHint.Checked = cfg.boDealTry_Attack_ShowHint;        // 原 :714
        seBrutalAttackTime.Value = (int)cfg.dwBrutal_Attack_Interval;        // 原 :716
        chkBrutalAttackHint.Checked = cfg.boBrutal_Attack_ShowHint;          // 原 :717
        chkShowAttackLog.Checked = cfg.boShowAttackLog;                      // 原 :719
        // 原 :720 `chkShowDropConcurrentLog.Checked := g_Config.boShowDropConcurrentLog;` 被注释掉（见 D10）

        trckbrCollectCount.Value = ClampTrack(cfg.dwCollectCount);            // 原 :722
        trckbrSpeedValue.Value = ClampTrack(cfg.dwSpeedValue);                // 原 :723
        lblSpeedValue.Text = GameSpeedLogic.SpeedValueLabelRefresh(trckbrSpeedValue.Value, trckbrCollectCount.Value);  // 原 :725

        seContinueSpeedPassIncTime.Value = (int)cfg.dwContinueSpeedPassIncTime;   // 原 :727
        chkContinueSpeedCloseSocket.Checked = cfg.boContinueSpeedCloseSocket;     // 原 :728
        seContinueSpeedCount.Value = cfg.nContinueSpeedCount;                     // 原 :729
        seSumSpeedCheckTime.Value = cfg.nSumSpeedCheckTime;                       // 原 :731
        seSumSpeedMaxCount.Value = cfg.nSumSpeedMaxCount;                         // 原 :732
        chkZeroCompensationValueClearPool.Checked = cfg.boZeroCompensationValueClearPool;   // 原 :734
        seClientUploadPickItemsTime.Value = cfg.dwClientUploadPickItemsTime;      // 原 :736
        SetSaveStatus(false);                                                     // 原 :738
    }

    private static int ClampTrack(int v)
    {
        // WinForms TrackBar.Value 有 [Minimum, Maximum] 硬约束；为保持"编程赋值不裁剪"语义，
        // 这里把值夹到控件范围（0..100）后仍记录原始值在 cfg 里，仅显示层受限。
        if (v < 0) return 0;
        if (v > 100) return 100;
        return v;
    }

    /// <summary>原 :741-744 `TFrmGameSpeed.SetSaveStatus`。</summary>
    public void SetSaveStatus(bool boSave) => btnSave.Enabled = boSave;      // 原 :743

    /// <summary>原 :746-750 `trckbrSpeedValueChange`（注意与 RefreshCtrlsStatus 的格式差异，见 GameSpeedLogic）。</summary>
    public void trckbrSpeedValue_Change(object sender, EventArgs e)
    {
        lblSpeedValue.Text = GameSpeedLogic.SpeedValueLabelOnChange(trckbrSpeedValue.Value, trckbrCollectCount.Value);  // 原 :748
        SetSaveStatus(true);                                                                                            // 原 :749
    }

    /// <summary>原 :752-755 `btnCloseClick`。</summary>
    public void btnClose_Click(object sender, EventArgs e) => Close();       // 原 :754

    /// <summary>原 :757-855 `btnSaveClick`。</summary>
    public void btnSave_Click(object sender, EventArgs e)
    {
        // 原 :765-766 `if vstAntiPlugAction.IsEditing then vstAntiPlugAction.EndEditNode;`
        if (vstAntiPlugAction.IsEditing)
        {
            vstAntiPlugAction.IsEditing = false;
            vstAntiPlugAction.InvalidateNode(vstAntiPlugAction.SelectedNode);
        }

        var result = GameSpeedLogic.ValidateSpeedValue(trckbrCollectCount.Value, trckbrSpeedValue.Value,
                                                       out int collectCount, out int speedValue);
        if (result != GameSpeedSaveResult.OK)
        {
            MessageBoxSeam.ShowError(GameSpeedLogic.SpeedValueErrorText, "错误");    // 原 :778
            trckbrSpeedValue.Focus();                                                // 原 :779
            return;                                                                  // 原 :780 Exit
        }

        var cfg = FormGlobals.g_Config;
        cfg.dwSpeedValue = speedValue;                                    // 原 :783
        cfg.dwCollectCount = collectCount;                                // 原 :784
        cfg.dwClientUploadPickItemsTime = (ushort)seClientUploadPickItemsTime.Value;   // 原 :786

        GameSpeedLogic.Save(FormGlobals.g_sIniFileName);                  // 原 :788-852

        SetSaveStatus(false);                                            // 原 :854
    }

    /// <summary>原 :857-863 `seBColorChange`。</summary>
    public void seBColor_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.btMsgBColor = seBColor.ColorIndex;                              // 原 :859
        edtPreview.BackColor = ColorIndex.ColorIndexToTColor(FormGlobals.g_Config.btMsgBColor);   // 原 :860
        edtPreview.ForeColor = ColorIndex.ColorIndexToTColor(FormGlobals.g_Config.btMsgFColor);   // 原 :861
        SetSaveStatus(true);                                                                // 原 :862
    }

    /// <summary>原 :865-871 `seFColorChange`。</summary>
    public void seFColor_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.btMsgFColor = seFColor.ColorIndex;                              // 原 :867
        edtPreview.BackColor = ColorIndex.ColorIndexToTColor(FormGlobals.g_Config.btMsgBColor);   // 原 :868
        edtPreview.ForeColor = ColorIndex.ColorIndexToTColor(FormGlobals.g_Config.btMsgFColor);   // 原 :869
        SetSaveStatus(true);                                                                // 原 :870
    }

    /// <summary>原 :873-877 `cbbMsgTypeChange`。</summary>
    public void cbbMsgType_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.btMsgType = (byte)cbbMsgType.SelectedIndex;      // 原 :875
        SetSaveStatus(true);                                                  // 原 :876
    }

    /// <summary>原 :879-883 `seLockTimeChange`。</summary>
    public void seLockTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.nLockTime = seLockTime.Value;      // 原 :881
        SetSaveStatus(true);                                    // 原 :882
    }

    /// <summary>原 :885-889 `chkSaveLockStatusClick`。</summary>
    public void chkSaveLockStatus_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boSaveLockStatus = chkSaveLockStatus.Checked;   // 原 :887
        SetSaveStatus(true);                                                 // 原 :888
    }

    /// <summary>原 :891-895 `chkShowLockLogClick`。</summary>
    public void chkShowLockLog_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boShowLockLog = chkShowLockLog.Checked;   // 原 :893
        SetSaveStatus(true);                                           // 原 :894
    }

    /// <summary>原 :897-901 `edtShowLockMsgChange`。</summary>
    public void edtShowLockMsg_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.sShowLockMsg = edtShowLockMsg.Text;   // 原 :899
        SetSaveStatus(true);                                       // 原 :900
    }

    /// <summary>原 :903-907 `chkSpeedClearDataClick`。</summary>
    public void chkSpeedClearData_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boSpeedClearData = chkSpeedClearData.Checked;   // 原 :905
        SetSaveStatus(true);                                                 // 原 :906
    }

    /// <summary>原 :909-913 `seDealTryAttackTimeChange`。</summary>
    public void seDealTryAttackTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.dwDealTry_Attack_Interval = (uint)seDealTryAttackTime.Value;   // 原 :911
        SetSaveStatus(true);                                                               // 原 :912
    }

    /// <summary>原 :915-919 `chkDealTryAttackHintClick`。</summary>
    public void chkDealTryAttackHint_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boDealTry_Attack_ShowHint = chkDealTryAttackHint.Checked;   // 原 :917
        SetSaveStatus(true);                                                             // 原 :918
    }

    /// <summary>原 :921-925 `seBrutalAttackTimeChange`。</summary>
    public void seBrutalAttackTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.dwBrutal_Attack_Interval = (uint)seBrutalAttackTime.Value;   // 原 :923
        SetSaveStatus(true);                                                              // 原 :924
    }

    /// <summary>原 :927-931 `chkBrutalAttackHintClick`。</summary>
    public void chkBrutalAttackHint_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boBrutal_Attack_ShowHint = chkBrutalAttackHint.Checked;   // 原 :929
        SetSaveStatus(true);                                                           // 原 :930
    }

    /// <summary>原 :933-1006 `vstAntiPlugActionGetText`（托管侧在 OwnerDraw 里取值，见 DrawNode）。</summary>
    public string vstAntiPlugAction_GetText(TAntiPlugActionMode mode, int column)
        => GameSpeedLogic.DescribeCell(mode, column);

    /// <summary>原 :1065-1079 `vstAntiPlugActionEditing`。</summary>
    public bool vstAntiPlugAction_Editing(TAntiPlugActionMode mode, int column)
        => GameSpeedLogic.IsEditable(mode, column);

    /// <summary>原 :1224-1234 `vstAntiPlugActionChecking`。</summary>
    public bool vstAntiPlugAction_Checking(TAntiPlugActionMode mode)
        => GameSpeedLogic.CanToggleCheck(mode);

    /// <summary>原 :1174-1204 `vstAntiPlugActionGetHint`（此处作为普通方法暴露，便于单测）。</summary>
    public string vstAntiPlugAction_GetHint(TAntiPlugActionMode mode, int column)
        => GameSpeedLogic.GetHintText(mode, column);

    /// <summary>原 :1148-1162 `vstAntiPlugActionChecked`：勾选 → `boEnabled`。</summary>
    public void vstAntiPlugAction_Checked(TAntiPlugActionMode mode, bool isChecked)
    {
        FormGlobals.g_Config.ActionList[(int)mode].boEnabled = isChecked;   // 原 :1156-1159
        SetSaveStatus(true);                                                // 原 :1160
    }

    /// <summary>原 :598-617 `OnBtnIntervalClick`：18 种模式下按钮打开 uFrmInterval。</summary>
    public void OnBtnIntervalClick(TAntiPlugActionMode mode)
    {
        if (GameSpeedLogic.IntervalCellIsButton(mode))       // 原 :605-614（与 PrepareEdit 列表一致）
            IntervalUnit.ShowFrmInterval(mode);              // 原 :615
    }

    /// <summary>原 :1164-1172 `btnDefaultClick` —— **原文整个函数体被注释掉**，保留空实现。</summary>
    public void btnDefault_Click(object sender, EventArgs e)
    {
        // 原 :1166-1171 全部被注释：
        //   g_Config := g_DefaultConfig; vstAntiPlugAction.Invalidate;
        //   RefreshCtrlsStatus; SetSaveStatus(True);
    }

    /// <summary>原 :1206-1210 `chkShowAttackLogClick`。</summary>
    public void chkShowAttackLog_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boShowAttackLog = chkShowAttackLog.Checked;   // 原 :1208
        SetSaveStatus(true);                                              // 原 :1209
    }

    /// <summary>原 :1212-1216 `seContinueSpeedPassIncTimeChange`。</summary>
    public void seContinueSpeedPassIncTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.dwContinueSpeedPassIncTime = (uint)seContinueSpeedPassIncTime.Value;   // 原 :1214
        SetSaveStatus(true);                                                                        // 原 :1215
    }

    /// <summary>原 :1218-1222 `chkShowDropConcurrentLogClick` —— 赋值被注释掉，只置脏。</summary>
    public void chkShowDropConcurrentLog_Click(object sender, EventArgs e)
    {
        // 原 :1220 `g_Config.boShowDropConcurrentLog := chkShowDropConcurrentLog.Checked;` 被注释掉
        SetSaveStatus(true);                                              // 原 :1221
    }

    /// <summary>原 :1236-1240 `chkContinueSpeedCloseSocketClick`。</summary>
    public void chkContinueSpeedCloseSocket_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boContinueSpeedCloseSocket = chkContinueSpeedCloseSocket.Checked;   // 原 :1238
        SetSaveStatus(true);                                                                     // 原 :1239
    }

    /// <summary>原 :1242-1246 `seContinueSpeedCountChange`。</summary>
    public void seContinueSpeedCount_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.nContinueSpeedCount = seContinueSpeedCount.Value;   // 原 :1244
        SetSaveStatus(true);                                                     // 原 :1245
    }

    /// <summary>原 :1248-1252 `seUserShopSearchTimeChange`。</summary>
    public void seUserShopSearchTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.dwUserShop_Search_Interval = (uint)seUserShopSearchTime.Value;   // 原 :1250
        SetSaveStatus(true);                                                                  // 原 :1251
    }

    /// <summary>原 :1254-1258 `seUserShopBuyTimeChange`。</summary>
    public void seUserShopBuyTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.dwUserShop_Buy_Interval = (uint)seUserShopBuyTime.Value;   // 原 :1256
        SetSaveStatus(true);                                                           // 原 :1257
    }

    /// <summary>原 :1260-1264 `seTakeOnItemTimeChange`。</summary>
    public void seTakeOnItemTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.dwTakeOn_Item_Interval = (uint)seTakeOnItemTime.Value;   // 原 :1262
        SetSaveStatus(true);                                                         // 原 :1263
    }

    /// <summary>原 :1266-1270 `chkUserShopSearchHintClick`。</summary>
    public void chkUserShopSearchHint_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boUserShop_Search_ShowHint = chkUserShopSearchHint.Checked;   // 原 :1268
        SetSaveStatus(true);                                                              // 原 :1269
    }

    /// <summary>原 :1272-1276 `chkUserShopBuyHintClick`。</summary>
    public void chkUserShopBuyHint_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boUserShop_Buy_ShowHint = chkUserShopBuyHint.Checked;   // 原 :1274
        SetSaveStatus(true);                                                        // 原 :1275
    }

    /// <summary>原 :1278-1282 `chkTakeOnItemHintClick`。</summary>
    public void chkTakeOnItemHint_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boTakeOn_Item_ShowHint = chkTakeOnItemHint.Checked;   // 原 :1280
        SetSaveStatus(true);                                                       // 原 :1281
    }

    /// <summary>原 :1312-1316 `seSumSpeedCheckTimeChange`。</summary>
    public void seSumCheckTime_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.nSumSpeedCheckTime = seSumSpeedCheckTime.Value;   // 原 :1314
        SetSaveStatus(true);                                                  // 原 :1315
    }

    /// <summary>原 :1318-1322 `seSumSpeedMaxCountChange`。</summary>
    public void seSumSpeedMaxCount_Change(object sender, EventArgs e)
    {
        FormGlobals.g_Config.nSumSpeedMaxCount = seSumSpeedMaxCount.Value;   // 原 :1320
        SetSaveStatus(true);                                                // 原 :1321
    }

    /// <summary>原 :1324-1329 `chkZeroCompensationValueClearPoolClick`。</summary>
    public void chkZeroCompensationValueClearPool_Click(object sender, EventArgs e)
    {
        FormGlobals.g_Config.boZeroCompensationValueClearPool = chkZeroCompensationValueClearPool.Checked;   // 原 :1327
        SetSaveStatus(true);                                                                                // 原 :1328
    }

    /// <summary>原 :1284-1310 `vstAntiPlugActionAfterCellPaint`（列 2 画"脚本"图标）。
    /// 托管侧合并进 <see cref="vstAntiPlugAction_DrawNode"/>。</summary>
    public void vstAntiPlugAction_AfterCellPaint(TAntiPlugActionMode mode, int column)
    {
        // 原 :1297-1309：仅列 2 会画 `ilCheck` 的 boProcessScript 图标 + 文本 '脚本'
    }

    /// <summary>原 :1008-1054 `vstAntiPlugActionDrawText` + :1022-1032 的图标绘制（托管侧 OwnerDraw）。</summary>
    public void vstAntiPlugAction_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
        var mode = vstAntiPlugAction.GetActionMode(e.Node);
        if (!mode.HasValue) { e.DrawDefault = true; return; }

        var g = e.Graphics;
        g.FillRectangle(Brushes.White, e.Bounds);
        int x = e.Bounds.Left;
        var action = FormGlobals.g_Config.ActionList[(int)mode.Value];

        for (int col = 0; col < GameSpeedColumn.Count && col < vstAntiPlugAction.ColumnWidths.Count; col++)
        {
            int w = vstAntiPlugAction.ColumnWidths[col];
            var rect = new Rectangle(x, e.Bounds.Top, w, e.Bounds.Height);

            if (col == GameSpeedColumn.Enabled)
                g.DrawImage(ilCheck.Images[GameSpeedLogic.ShouldBeChecked(mode.Value) ? 1 : 0],
                            new Rectangle(x + 2, e.Bounds.Top + 4, 13, 13));       // 原 :662/686 的 CheckType 复选框
            else if (col == GameSpeedColumn.ShowHint)
                g.DrawImage(ilCheck.Images[action.boShowHint ? 1 : 0],
                            new Rectangle(x + 2, e.Bounds.Top + 4, 13, 13));       // 原 :1024-1031
            else if (col == GameSpeedColumn.Debug)
                g.DrawImage(ilCheck.Images[action.boDebug ? 1 : 0],
                            new Rectangle(x + 2, e.Bounds.Top + 4, 13, 13));       // 原 :1026-1027
            else
            {
                // 原 :1020-1021：并发行且列 0 → 红字
                var brush = (GameSpeedLogic.IsConcurrent(mode.Value) && col == GameSpeedColumn.Enabled)
                    ? Brushes.Red : Brushes.Black;
                string text = GameSpeedLogic.DescribeCell(mode.Value, col);
                g.DrawString(text, Font, brush,
                             new RectangleF(rect.Left + 4, rect.Top + 3, rect.Width - 6, rect.Height - 4));
            }

            if (col == GameSpeedColumn.ProcessMode && action.boProcessScript)
            {
                // 原 :1302-1308：列 2 额外画 boProcessScript 图标 + '脚本'
                g.DrawImage(ilCheck.Images[1], new Rectangle(x + 76, e.Bounds.Top + 4, 13, 13));
                g.DrawString("脚本", Font, Brushes.Black, x + 92, e.Bounds.Top + 3);
            }

            x += w;
        }
    }

    /// <summary>原 :1081-1139 `vstAntiPlugActionNodeClick`（列 5/8 切换 ShowHint/Debug，列 2 左半区启动编辑）。</summary>
    public void vstAntiPlugAction_NodeClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        var mode = vstAntiPlugAction.GetActionMode(e.Node);
        if (!mode.HasValue) return;

        int col = HitColumn(e.X);
        var action = FormGlobals.g_Config.ActionList[(int)mode.Value];
        if (col == GameSpeedColumn.ShowHint)              // 原 :1094-1095
        {
            action.boShowHint = !action.boShowHint;
            vstAntiPlugAction.InvalidateNode(e.Node);
            SetSaveStatus(true);                          // 原 :1100
        }
        else if (col == GameSpeedColumn.Debug)            // 原 :1097
        {
            action.boDebug = !action.boDebug;
            vstAntiPlugAction.InvalidateNode(e.Node);
            SetSaveStatus(true);
        }
        else if (col == GameSpeedColumn.ProcessMode)      // 原 :1102-1118
        {
            int xInCell = e.X - ColumnLeft(col);
            if (xInCell < 76)                             // 原 :1108 `if P.X < R.Left + 76`
                StartEditing(e.Node, col);                // 原 :1110 PostMessage(WM_STARTEDITING, ...)
            else
            {
                action.boProcessScript = !action.boProcessScript;   // 原 :1114
                vstAntiPlugAction.InvalidateNode(e.Node);
                SetSaveStatus(true);                                // 原 :1116
            }
        }
        else if (col > 0)                                 // 原 :1135-1136
        {
            StartEditing(e.Node, col);
        }
    }

    /// <summary>原 :1056-1063 `WMStartEditing`（`WM_STARTEDITING = WM_USER + 778`）。</summary>
    public void StartEditing(TreeNode node, int column)
    {
        if (!GameSpeedLogic.IsEditable(vstAntiPlugAction.GetActionMode(node) ?? TAntiPlugActionMode.amHit, column))
            return;
        vstAntiPlugAction.IsEditing = true;   // 原 :1062 vstAntiPlugAction.EditNode(Node, Message.LParam)
    }

    /// <summary>原 :1141-1146 `vstAntiPlugActionCreateEditor`（托管侧由 StartEditing 直接处理列类型）。</summary>
    public EditorSpec CreateEditor(TAntiPlugActionMode mode, int column) => GameSpeedLogic.PrepareEditSpec(mode, column);

    /// <summary>选中变化时同步（用于 OwnerDraw 的聚焦态；原文 :404-405 用 Focused）。</summary>
    public void vstAntiPlugAction_AfterSelect(object sender, TreeViewEventArgs e) => Invalidate();

    private int ColumnLeft(int column)
    {
        int x = 0;
        for (int i = 0; i < column && i < vstAntiPlugAction.ColumnWidths.Count; i++) x += vstAntiPlugAction.ColumnWidths[i];
        return x;
    }

    private int HitColumn(int x)
    {
        int acc = 0;
        for (int i = 0; i < vstAntiPlugAction.ColumnWidths.Count; i++)
        {
            acc += vstAntiPlugAction.ColumnWidths[i];
            if (x < acc) return i;
        }
        return -1;
    }
}
