// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   1431-1472  LoadHelpFile
//   1473-1478  ClearShowItem
//   1479-1594  RefShowItem
//   1595-1673  ListViewItemClick
//   1674-1805  DLabelDefaultItemClick
//   1806-1935  DEditSearchItemChange
//   1936-2056  DComboBoxItemStdModeSelect
//   2057-2062  DComboBoxColorShow
//   2063-2361  CheckBoxClickEx（**115 处绑定**，本单元最大的单一处理器）
//   2362-2384  RefUseItemConfigClick
//   2385-2743  RefUseItemConfig
//   2744-2763  RefKeyBoardConfig
//   2764-2961  RefConfig
//   3325-3845  MakeControlAddressList（516 条 AddObject）
//   3846-3850  MouseMoveEvent
//   5065-5079  LoadConfig
//   5080-5084  Run
//   5085-5089  RefActorList
//   5090-5127  GetShowItem / FindShowItem / FindHintItem / FindPickItem
//   5128-5132  HintItem
//   7044-7064  DEditSpecialColorChange 等
//   7311-7356  PatchAddNearEffectCheckBox
//   7746-7809  DControlMouseMoveShowHint
//   7995-8016  OnChanggingVolumePosition / OnChangedVolumePosition / DoInitAllComponentsMouseMove
//   8017-8051  LoadNotesFile / SaveNotesFile
//   8052-8110  OnPlugMemoConfig10ButtonEditClick / OnPlugPageControlConfigActivePageChange / ShowViewNotes
//   8111-8182  OnPopupMenuItemsClick
//
// ★ 本文件是**待填充骨架**（进度登记见报告 §未完成/阻塞项）：
//   方法签名、原文行号、语义摘要、以及"依赖哪些接缝"都已就位，
//   方法体尚未 1:1 移入的，用 `NotPorted(...)` 显式留痕（**绝不静默返回默认值**，照 §25.2）。
//   这样做的目的：让 279 条绑定与 147 个方法声明**全部可达且可编译**，
//   并把"哪些还是空的"变成测试可断言的、不可能被误当成"已完成"的状态。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Client.LoadDx;

namespace GXX.Client.GUI.GameConfig.Mir;

public partial class TMirConfigDlg
{
    /// <summary>
    /// 尚未 1:1 移入方法体的处理器/方法的**显式留痕**。
    /// 每次调用都记一条到 <see cref="NotPortedMethods"/>，报告与测试据此统计真实进度。
    /// ★ 不静默：这是"未完成"的机器可读证据，而不是一个看起来正常的默认返回值。
    /// </summary>
    public static readonly List<string> NotPortedMethods = new List<string>();

    private static void NotPorted(string method, int line)
    {
        NotPortedMethods.Add(method + " (MirConfigDlg.pas:" + line + ")");
    }

    /// <summary>清空留痕（测试用）。</summary>
    public static void ResetNotPorted() => NotPortedMethods.Clear();

    // ================================================================================
    // 1431-1594  帮助页 / 物品显示
    // ================================================================================

    /// <summary>原文 1431-1472：LoadHelpFile（读 Help\%s.txt 灌 PlugMemoConfigHelp）。</summary>
    public void LoadHelpFile() => NotPorted(nameof(LoadHelpFile), 1431);

    /// <summary>原文 1473-1478：<c>PlugMemoConfig2.Clear; PlugMemoConfig2.ColCount := 6;</c>。</summary>
    public override void ClearShowItem()
    {
        PlugCtl.PlugMemoConfig2.Clear();                       // 1475
        PlugCtl.PlugMemoConfig2.ColCount = 6;                  // 1476
    }

    /// <summary>原文 1479-1594：RefShowItem（把 g_FileItemDB 的显示项灌进 PlugMemoConfig2 列表）。</summary>
    public override void RefShowItem() => NotPorted(nameof(RefShowItem), 1479);

    // ================================================================================
    // 1595-2062  物品过滤页
    // ================================================================================

    /// <summary>原文 1595-1673：ListViewItemClick（物品页行点击 → 同步右侧编辑框）。</summary>
    public void ListViewItemClick(object sender, int ARow, int ACol, object ListItem, IntPtr ViewItem)
        => NotPorted(nameof(ListViewItemClick), 1595);

    /// <summary>原文 1674-1805：DLabelDefaultItemClick（"默认"按钮，把当前项写进默认模板）。</summary>
    public void DLabelDefaultItemClick(object sender, int X, int Y)
        => NotPorted(nameof(DLabelDefaultItemClick), 1674);

    /// <summary>原文 1806-1935：DEditSearchItemChange（搜索框 → 过滤 PlugMemoConfig2）。</summary>
    public void DEditSearchItemChange(object sender)
        => NotPorted(nameof(DEditSearchItemChange), 1806);

    /// <summary>原文 1936-2056：DComboBoxItemStdModeSelect（StdMode 下拉 → 重填物品列表）。</summary>
    public void DComboBoxItemStdModeSelect(object sender)
        => NotPorted(nameof(DComboBoxItemStdModeSelect), 1936);

    /// <summary>原文 2057-2062：<c>PlugLabelSpecialColor.CaptionColor.Up.Color := frmMain.GetRGB(PlugEditSpecialColor.Value);</c></summary>
    public void DComboBoxColorShow(object sender)
    {
        PlugCtl.PlugLabelSpecialColor.CaptionColor.Up.Color =
            MirConfigGlobalSeam.GetRGB(PlugCtl.PlugEditSpecialColor.Value);   // 2059
    }

    // ================================================================================
    // 2063-2384  勾选框总处理器（115 处绑定）
    // ================================================================================

    /// <summary>原文 2063-2361：CheckBoxClickEx —— 全单元最大的单一处理器（115 处绑定）。</summary>
    public void CheckBoxClickEx(object sender, int X, int Y)
        => NotPorted(nameof(CheckBoxClickEx), 2063);

    /// <summary>原文 2362-2384：RefUseItemConfigClick（"使用物品配置"按钮）。</summary>
    public void RefUseItemConfigClick(object sender, int X, int Y)
        => NotPorted(nameof(RefUseItemConfigClick), 2362);

    /// <summary>原文 2385-2743：RefUseItemConfig（把 FConfigCheckeds 写回全部勾选框/编辑框）。</summary>
    public void RefUseItemConfig(int nObj) => NotPorted(nameof(RefUseItemConfig), 2385);

    /// <summary>原文 2744-2763：RefKeyBoardConfig（把 g_ShortcutKeys 灌进 16 个快捷键标签）。</summary>
    public override void RefKeyboardConfig() => NotPorted(nameof(RefKeyboardConfig), 2744);

    /// <summary>原文 2764-2961：RefConfig（把 FConfigCheckeds 写回全部控件 + 下发 frmMain）。</summary>
    public void RefConfig() => NotPorted(nameof(RefConfig), 2764);

    // ================================================================================
    // 3325-3850  控件地址表 / 鼠标
    // ================================================================================

    /// <summary>原文 3325-3845：MakeControlAddressList（516 条 <c>Result.AddObject('名字', Pointer(@控件))</c>）。</summary>
    public THashedStringList MakeControlAddressList()
        => throw new NotImplementedException("MirConfigDlg.pas:3325 MakeControlAddressList（516 条）尚未移入");

    /// <summary>原文 3846-3850：MouseMoveEvent（原文 4609-4618 的赋值被 {} 注释掉，此处保留方法）。</summary>
    public void MouseMoveEvent(object sender, DelphiShiftState Shift, int X, int Y)
        => NotPorted(nameof(MouseMoveEvent), 3846);

    /// <summary>原文 7311-7356：PatchAddNearEffectCheckBox（作者打的运行时补丁：手动加一个勾选框）。</summary>
    public void PatchAddNearEffectCheckBox() => NotPorted(nameof(PatchAddNearEffectCheckBox), 7311);

    // ================================================================================
    // 5065-5132  生命周期与查询转发
    // ================================================================================

    /// <summary>原文 5085-5089：**空实现**（原文如此）。
    /// （<c>Logon</c>/<c>LoadConfig</c>/<c>Run</c>/<c>Finalize</c> 等已真实现的成员
    ///   见 <c>MirConfigDlg.Lifecycle.cs</c>，此处不再重复声明。）</summary>
    public override void RefActorList() { }

    // ================================================================================
    // 7044-8182  杂项处理器
    // ================================================================================

    /// <summary>
    /// 原文 7065-7073：DEditSpecialColorChange。
    /// （已真实现的同名方法见 <c>MirConfigDlg.Controls.cs</c> —— 此处不再重复声明。）
    /// </summary>

    /// <summary>原文 7746-7809：DControlMouseMoveShowHint（控件悬停 → 顶部提示条）。</summary>
    public void DControlMouseMoveShowHint(object sender, DelphiShiftState Shift, int X, int Y)
        => NotPorted(nameof(DControlMouseMoveShowHint), 7746);

    /// <summary>原文 7989-7994：OnChangedVolumePosition（音量条松手 → 写 g_SoundVolume）。</summary>
    public void OnChangedVolumePosition(object sender) => NotPorted(nameof(OnChangedVolumePosition), 7989);

    /// <summary>原文 7995-8000：OnChanggingVolumePosition（音量条拖动中）。</summary>
    public void OnChanggingVolumePosition(object sender) => NotPorted(nameof(OnChanggingVolumePosition), 7995);

    /// <summary>原文 8001-8016：DoInitAllComponentsMouseMove（递归给控件树挂 OnMouseMove）。</summary>
    public void DoInitAllComponentsMouseMove(TMirDxControl ParentCtrl)
        => NotPorted(nameof(DoInitAllComponentsMouseMove), 8001);

    /// <summary>原文 8017-8036：LoadNotesFile（PlugMemoConfigNotes.LoadFromFile）。</summary>
    public void LoadNotesFile() => NotPorted(nameof(LoadNotesFile), 8017);

    /// <summary>原文 8037-8051：SaveNotesFile（PlugMemoConfigNotes.Lines.SaveToFile）。</summary>
    public void SaveNotesFile() => NotPorted(nameof(SaveNotesFile), 8037);

    /// <summary>原文 8052-8086：OnPlugMemoConfig10ButtonEditClick（备注页 编辑/保存/取消 三按钮共用）。</summary>
    public void OnPlugMemoConfig10ButtonEditClick(object sender, int X, int Y)
        => NotPorted(nameof(OnPlugMemoConfig10ButtonEditClick), 8052);

    /// <summary>原文 8087-8098：OnPlugPageControlConfigActivePageChange。</summary>
    public void OnPlugPageControlConfigActivePageChange(object sender)
        => NotPorted(nameof(OnPlugPageControlConfigActivePageChange), 8087);

    /// <summary>原文 8099-8110：ShowViewNotes（把 FMemo 作为悬浮备注窗口显示/隐藏）。</summary>
    public void ShowViewNotes() => NotPorted(nameof(ShowViewNotes), 8099);

    /// <summary>
    /// 原文 8111-8182：OnPopupMenuItemsClick（物品页右键菜单 14 项）。
    /// </summary>
    public void OnPopupMenuItemsClick(object sender, int X, int Y)
        => NotPorted(nameof(OnPopupMenuItemsClick), 8111);
}
