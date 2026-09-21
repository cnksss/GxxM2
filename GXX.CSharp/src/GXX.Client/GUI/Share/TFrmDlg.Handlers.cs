using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.Scenes;
using static GXX.Client.GUI.Mir.MShareGlobals;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// FState.pas TFrmDlg —— **切片 1：无依赖/空体/纯字段读写面**（车道 p14-client-fstate）
//
// ★★ 为什么切片 1 的成员名要同时出现在两处（关键工程事实，实测证据）
//
// 本车道先按常规做法：把 25 条真实现放进本文件的 `partial class TFrmDlg`，不动生成壳。
// 结果**编译不过**，两条实测结论（本轮 `dotnet build GXX.Client.csproj` 原始输出）：
//
//   1) 对 `TFrmDlg.Decl.g.cs` 里的 `public virtual` 壳写 `override`
//      → `error CS0115: "TFrmDlg.XXX(...)": 没有找到适合的方法来重写`。
//      **对照实验**：把同一份 `override` 体直接写进 `TFrmDlg.Decl.g.cs` **自身**，
//      同样 CS0115 —— 不是"跨 partial 文件"的问题，而是那些壳成员在本编译单元里
//      **根本不可作为 override 目标**。
//   2) 用相同签名把成员写在本文件（`public virtual` 或 `public void`）
//      → `error CS0111: 类型"TFrmDlg"已定义了一个名为"XXX"的具有相同参数类型的成员`。
//
// ⇒ 结论：**实现面必须落在"TFrmDlg.Decl.g.cs 那个 partial 组"里**。
//   既有设计早就承认了这一点：`FStateDeclGen.ps1` 带一个 `$Handwritten` 跳过表（原本 20 条），
//   让真体由生成壳之外的 partial 供给、生成壳不再重复声明。
//
// ⇒ 切片 1 因此分两步落地（两步都必须做，缺一即编译失败或成员丢失）：
//   (1) `FStateDeclGen.ps1` 的 `$Handwritten` 加入这 25 个名字并重跑生成器
//       → 生成壳的 `throw` 从 **515 降到 490**（差集经 `Compare-Object` 实测：
//         恰好是这 25 个名字，无增项）；
//   (2) 本文件给出这 25 条成员的**真体**（下列 partial class 块）。
//       因为生成器不再声明它们，本文件的声明**不产生 CS0111**，成员也不会丢失。
//
//   这就是生成器设计意图的完整形态，不是绕过它 —— 生成器负责"未移植面的 throw 声明"，
//   手写 partial 负责"已移植面的真实现"，`$Handwritten` 是两侧的唯一交界。
//
// ★ 虚分派：25 条在原文里都是 `procedure ...; virtual;`（TFrmDlg 是 UI 状态窗口族基类，
//   原文子类会 override）。托管侧沿用本工程既有写法 `public virtual`
//   （同 `TFrmDlg.Core.cs` 的 20 条真实现）：定义一个可被后续子类 override 的新槽，
//   对 TFrmDlg 实例的直接调用命中真体而非 throw 壳。
//
// 覆盖原文行号（逐条见 `TFrmDlgPortLedger.Slice1`）：
//   1891-1895  HideAllControls              1897-1900  RestoreHideControls
//   2316-2319  DStateWinClick               2883-2886  DBottomInRealArea
//   2888-2891  DBotPlusAbilDirectPaint      12457-12460 MerchantDlgPaint
//   17795-17798 DUserState1MouseDown        18018-18021 DChgGamePwdCloseClick
//   18023-18026 DChgGamePwdDirectPaint      18028-18031 DscSelect1InRealArea
//   18038-18042 DItemBagMouseMove           18197-18200 DMinMapDlgShow
//   18202-18205 DMinMapDlgHide              18207-18210 DMinMapDlgResize
//   18655-18658 DGameGoldDealCancelClick    20367-20370 AttactkModeChange
//   20741-20744 OpenDUpgradeDlg             20746-20749 CloseDUpgradeDlg
//   21062-21065 OpenDRandomCodeDlg          21067-21070 CloseDRandomCodeDlg
//   24133-24137 DMouseMoveClearHints        24356-24359 DSayItemDlgCloseClick
//   24361-24365 DSayItemDlgMouseDown        24367-24372 DSayItemDlgMouseMove
//   24464-24467 DUpdateStatusDlgMouseLeave
//
// 原文缺陷 / 原文如此：`DChgGamePwdCloseClick`（18018-18021）原文体内只有一行被注释掉的
//   `// CloseDChgGamePwd;` —— 函数体为空，托管侧**照抄空体**并标 `// 原文如此`，不顺手接线。
// ============================================================================================

public partial class TFrmDlg
{
    // ==========================================================================================
    // 1891-1900  隐藏/恢复隐藏控件（Memo 前后台切换）
    // ==========================================================================================

    /// <summary>
    /// FState.pas:1891-1895 procedure TFrmDlg.HideAllControls。
    /// 先把 Memo 当前的 Visible **快照进** GuildMemoVisible，再隐藏 Memo。
    /// </summary>
    public virtual void HideAllControls()
    {
        GuildMemoVisible = Memo.Visible;    // 1893
        Memo.Visible = false;               // 1894
    }

    /// <summary>
    /// FState.pas:1897-1900 procedure TFrmDlg.RestoreHideControls。
    /// 只按 HideAllControls 存下的快照恢复 Memo.Visible，**不读也不改** GuildMemoVisible。
    /// </summary>
    public virtual void RestoreHideControls()
    {
        Memo.Visible = GuildMemoVisible;    // 1899
    }

    // ==========================================================================================
    // 2316-2319 / 20367-20370 / 18023-18026 / 12457-12460 / 2888-2891 / 18655-18658 /
    // 20741-20749 / 21062-21070  原文空体（逐字保留空体）
    // ==========================================================================================

    /// <summary>FState.pas:2316-2319 procedure TFrmDlg.DStateWinClick（原文**空体**）。</summary>
    public virtual void DStateWinClick(object Sender, int X, int Y)
    {
        // 原文 2317-2318 为空
    }

    /// <summary>FState.pas:20367-20370 procedure TFrmDlg.AttactkModeChange（原文**空体**）。</summary>
    public virtual void AttactkModeChange()
    {
        // 原文 20368-20369 为空
    }

    /// <summary>FState.pas:18023-18026 procedure TFrmDlg.DChgGamePwdDirectPaint（原文**空体**）。</summary>
    public virtual void DChgGamePwdDirectPaint(object Sender)
    {
        // 原文 18024-18025 为空
    }

    /// <summary>FState.pas:12457-12460 procedure TFrmDlg.MerchantDlgPaint（原文**空体**）。</summary>
    public virtual void MerchantDlgPaint(object Sender)
    {
        // 原文 12458-12459 为空
    }

    /// <summary>FState.pas:2888-2891 procedure TFrmDlg.DBotPlusAbilDirectPaint（原文**空体**）。</summary>
    public virtual void DBotPlusAbilDirectPaint(object Sender)
    {
        // 原文 2889-2890 为空
    }

    /// <summary>FState.pas:18655-18658 procedure TFrmDlg.DGameGoldDealCancelClick（原文**空体**）。</summary>
    public virtual void DGameGoldDealCancelClick(object Sender, int X, int Y)
    {
        // 原文 18656-18657 为空
    }

    /// <summary>FState.pas:20741-20744 procedure TFrmDlg.OpenDUpgradeDlg（原文**空体**）。</summary>
    public virtual void OpenDUpgradeDlg()
    {
        // 原文 20742-20743 为空
    }

    /// <summary>FState.pas:20746-20749 procedure TFrmDlg.CloseDUpgradeDlg（原文**空体**）。</summary>
    public virtual void CloseDUpgradeDlg()
    {
        // 原文 20747-20748 为空
    }

    /// <summary>FState.pas:21062-21065 procedure TFrmDlg.OpenDRandomCodeDlg（原文**空体**）。</summary>
    public virtual void OpenDRandomCodeDlg()
    {
        // 原文 21063-21064 为空
    }

    /// <summary>FState.pas:21067-21070 procedure TFrmDlg.CloseDRandomCodeDlg（原文**空体**）。</summary>
    public virtual void CloseDRandomCodeDlg()
    {
        // 原文 21068-21069 为空
    }

    /// <summary>
    /// FState.pas:18018-18021 procedure TFrmDlg.DChgGamePwdCloseClick。
    /// 原文体内只有一行**被注释掉**的 `// CloseDChgGamePwd;` —— 即**空体 + 原文如此**。
    /// </summary>
    public virtual void DChgGamePwdCloseClick(object Sender, int X, int Y)
    {
        // 18020：// CloseDChgGamePwd;   ← 原文就是注释，函数体为空（原文如此）
    }

    // ==========================================================================================
    // 2883-2886 / 18028-18031  两个 RealArea 判定回调：原文恒置 True
    // ==========================================================================================

    /// <summary>
    /// FState.pas:2883-2886 procedure TFrmDlg.DBottomInRealArea。
    /// 原文**无条件**把 IsRealArea 置 True，忽略 Sender/X/Y（原文如此）。
    /// </summary>
    public virtual void DBottomInRealArea(object Sender, int X, int Y, ref bool IsRealArea)
    {
        IsRealArea = true;                  // 2885
    }

    /// <summary>
    /// FState.pas:18028-18031 procedure TFrmDlg.DscSelect1InRealArea。
    /// 与 DBottomInRealArea 同形：**无条件**置 True（原文如此）。
    /// </summary>
    public virtual void DscSelect1InRealArea(object Sender, int X, int Y, ref bool IsRealArea)
    {
        IsRealArea = true;                  // 18030
    }

    // ==========================================================================================
    // 17795-17798  鼠标按下：原文空体（不改变任何状态）
    // ==========================================================================================

    /// <summary>FState.pas:17795-17798 procedure TFrmDlg.DUserState1MouseDown（原文**空体**）。</summary>
    public virtual void DUserState1MouseDown(object Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
    {
        // 原文 17796-17797 为空
    }

    // ==========================================================================================
    // 18197-18210  小地图对话框的 Show/Hide/Resize：三条原文都是空体
    // ==========================================================================================

    /// <summary>FState.pas:18197-18200 procedure TFrmDlg.DMinMapDlgShow（原文**空体**）。</summary>
    public virtual void DMinMapDlgShow(object Sender)
    {
        // 原文 18198-18199 为空
    }

    /// <summary>FState.pas:18202-18205 procedure TFrmDlg.DMinMapDlgHide（原文**空体**）。</summary>
    public virtual void DMinMapDlgHide(object Sender)
    {
        // 原文 18203-18204 为空
    }

    /// <summary>FState.pas:18207-18210 procedure TFrmDlg.DMinMapDlgResize（原文**空体**）。</summary>
    public virtual void DMinMapDlgResize(object Sender)
    {
        // 原文 18208-18209 为空
    }

    // ==========================================================================================
    // 提示窗清理面（HintWindows.Clear）
    //
    // 接缝说明：原文这里写的是**单元级全局** `HintWindows:THintWindows`（MShare.pas:1464），
    // 托管侧正式归属是 `GXX.Client.Scenes.DrawScrnEnv.HintWindows`（DrawScrn 车道已落地）。
    // D-P10-06 之后 FStateSeams.cs 不再持有 THintWindows 接缝，故此处直接引用正式归属。
    // ==========================================================================================

    /// <summary>
    /// FState.pas:24133-24137 procedure TFrmDlg.DMouseMoveClearHints。
    /// 原文顺序：先 DScreen.ClearHint，再 HintWindows.Clear（**两步都保留**）。
    /// </summary>
    public virtual void DMouseMoveClearHints(object Sender, TShiftState Shift, int X, int Y)
    {
        FStateScreenSeam.ClearHint();                       // 24135
        DrawScrnEnv.HintWindows.Clear();                    // 24136
    }

    /// <summary>
    /// FState.pas:24464-24467 procedure TFrmDlg.DUpdateStatusDlgMouseLeave。
    /// 原文只清全局提示窗集合（**不**调 DScreen.ClearHint）。
    /// </summary>
    public virtual void DUpdateStatusDlgMouseLeave(object Sender)
    {
        DrawScrnEnv.HintWindows.Clear();                    // 24466
    }

    /// <summary>
    /// FState.pas:18038-18042 procedure TFrmDlg.DItemBagMouseMove。
    /// 原文：清提示窗 → 清提示 → 清背包提示标志。
    /// </summary>
    public virtual void DItemBagMouseMove(object Sender, TShiftState Shift, int X, int Y)
    {
        DrawScrnEnv.HintWindows.Clear();                    // 18040
        FStateScreenSeam.ClearHint();                       // 18041
        g_boShowBagInfo = 0;                                // 18042（原文 g_boShowBagInfo := False）
    }

    // ==========================================================================================
    // 24356-24372  SayItem 对话框的关闭/右键关闭/移出关闭
    // ==========================================================================================

    /// <summary>FState.pas:24356-24359 procedure TFrmDlg.DSayItemDlgCloseClick。</summary>
    public virtual void DSayItemDlgCloseClick(object Sender, int X, int Y)
    {
        DSayItemDlg.Visible = false;                        // 24358
    }

    /// <summary>
    /// FState.pas:24361-24365 procedure TFrmDlg.DSayItemDlgMouseDown。
    /// **只在右键**时关闭；左键/中键都不做事（原文如此）。
    /// </summary>
    public virtual void DSayItemDlgMouseDown(object Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
    {
        if (Button == TMouseButton.mbRight)                 // 24363
            DSayItemDlg.Visible = false;                    // 24364
    }

    /// <summary>
    /// FState.pas:24367-24372 procedure TFrmDlg.DSayItemDlgMouseMove。
    /// 原文顺序：先置 "移出即关闭" 标志，再清提示窗、清提示。
    /// </summary>
    public virtual void DSayItemDlgMouseMove(object Sender, TShiftState Shift, int X, int Y)
    {
        boSayItemDlgMoveOutClose = true;                    // 24369
        DrawScrnEnv.HintWindows.Clear();                    // 24370
        FStateScreenSeam.ClearHint();                       // 24371
    }
}

/// <summary>
/// 切片 1 的**移植台账**：逐条列出本切片已换成真实现的 TFrmDlg 成员及其原文行号。
///
/// 用途（为什么要有这个类）：
///   1) 报告里的"515 成员对账表"由 `docs/并行报告-p14-client-fstate.md` 承载，
///      但那份 Markdown 是**人类可读**的；本表是同一事实的**机器可读**副本，
///      使测试可以断言"登记了行号"而不是靠人工核对。
///   2) 后继切片往这里追加一行即可，报告与代码不会各说各话。
///   3) `Slice1` 的名字集合与 `FStateDeclGen.ps1` 的 `$Handwritten` 是同一事实的两侧登记；
///      测试可断言"台账里的名字都能被调用且不抛 NotSupportedException"。
///
/// ★ 注意：这里**只登记已经换成真实现**的成员（真体）。
///   `TFrmDlg.Decl.g.cs` 中仍为 `throw new NotSupportedException` 的成员**不在此表**，
///   它们的数量由 `Select-String 'throw new NotSupportedException'` 实测（见报告）。
/// </summary>
public static class TFrmDlgPortLedger
{
    /// <summary>一条已移植成员记录：TFrmDlg 成员名 + 原文（FState.pas）行号区间。</summary>
    public sealed class PortedMember
    {
        /// <summary>TFrmDlg 的成员名（与 TFrmDlg.Decl.g.cs / 原文一致）。</summary>
        public readonly string Name;

        /// <summary>原文 FState.pas 的实现行号（含起止，形如 "1891-1895"）。</summary>
        public readonly string SourceLines;

        public PortedMember(string name, string sourceLines)
        {
            Name = name;
            SourceLines = sourceLines;
        }
    }

    /// <summary>切片 1 落地的成员（25 条）。</summary>
    public static readonly IReadOnlyList<PortedMember> Slice1 = new[]
    {
        new PortedMember("HideAllControls",            "1891-1895"),
        new PortedMember("RestoreHideControls",        "1897-1900"),
        new PortedMember("DStateWinClick",             "2316-2319"),
        new PortedMember("DBottomInRealArea",          "2883-2886"),
        new PortedMember("DBotPlusAbilDirectPaint",    "2888-2891"),
        new PortedMember("MerchantDlgPaint",           "12457-12460"),
        new PortedMember("DUserState1MouseDown",       "17795-17798"),
        new PortedMember("DChgGamePwdCloseClick",      "18018-18021"),
        new PortedMember("DChgGamePwdDirectPaint",     "18023-18026"),
        new PortedMember("DscSelect1InRealArea",       "18028-18031"),
        new PortedMember("DItemBagMouseMove",          "18038-18042"),
        new PortedMember("DMinMapDlgShow",             "18197-18200"),
        new PortedMember("DMinMapDlgHide",             "18202-18205"),
        new PortedMember("DMinMapDlgResize",           "18207-18210"),
        new PortedMember("DGameGoldDealCancelClick",   "18655-18658"),
        new PortedMember("AttactkModeChange",          "20367-20370"),
        new PortedMember("OpenDUpgradeDlg",            "20741-20744"),
        new PortedMember("CloseDUpgradeDlg",           "20746-20749"),
        new PortedMember("OpenDRandomCodeDlg",         "21062-21065"),
        new PortedMember("CloseDRandomCodeDlg",        "21067-21070"),
        new PortedMember("DMouseMoveClearHints",       "24133-24137"),
        new PortedMember("DSayItemDlgCloseClick",      "24356-24359"),
        new PortedMember("DSayItemDlgMouseDown",       "24361-24365"),
        new PortedMember("DSayItemDlgMouseMove",       "24367-24372"),
        new PortedMember("DUpdateStatusDlgMouseLeave", "24464-24467"),
    };

    /// <summary>全部已登记切片（后继切片在这里追加）。</summary>
    public static readonly IReadOnlyList<IReadOnlyList<PortedMember>> AllSlices = new[] { Slice1 };

    /// <summary>切片 1 的真实现成员数（报告里的"真实体"分子）。</summary>
    public static int Slice1Count => Slice1.Count;

    /// <summary>登记表中是否包含某成员（不区分大小写，Delphi 标识符本就大小写不敏感）。</summary>
    public static bool Contains(string name)
    {
        for (int s = 0; s < AllSlices.Count; s++)
        {
            var slice = AllSlices[s];
            for (int i = 0; i < slice.Count; i++)
            {
                if (string.Equals(slice[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }
}
