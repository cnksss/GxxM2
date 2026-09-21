using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
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

    // ==========================================================================================
    // 切片 2：关闭/打开转发面 + 公会列表翻行 + 卧龙相关关闭
    //
    // 说明：这一族原文都是"一行转发"，被转发的方法在托管侧**都还是 NotSupportedException 壳**
    // （见各条注释）。托管侧按原文 1:1 保留转发形态 —— 既不内联被调者（会造第三份实现，§14.2），
    // 也不加"目标未移植则跳过"的保护（会把缺口静默掉）。因此这一族**调用时会抛**
    // NotSupportedException，抛点在被转发的方法里，**消息里带的是被转发的原文行号**。
    // 台账 `TFrmDlgPortLedger` 只登记"转发体已 1:1 落地"，不声称整条调用链已通。
    // ==========================================================================================

    /// <summary>FState.pas:17362-17365 procedure TFrmDlg.DSellDlgCloseClick（转 `CloseDSellDlg`）。</summary>
    public virtual void DSellDlgCloseClick(object Sender, int X, int Y)
    {
        CloseDSellDlg();                                    // 17364
    }

    /// <summary>FState.pas:17446-17449 procedure TFrmDlg.DMenuCloseClick（转 `CloseDMenuDlg`）。</summary>
    public virtual void DMenuCloseClick(object Sender, int X, int Y)
    {
        CloseDMenuDlg();                                    // 17448
    }

    /// <summary>FState.pas:17506-17509 procedure TFrmDlg.DKsOkClick（转 `CloseDKeySelDlg`）。</summary>
    public virtual void DKsOkClick(object Sender, int X, int Y)
    {
        CloseDKeySelDlg();                                  // 17508
    }

    /// <summary>FState.pas:17806-17809 procedure TFrmDlg.DCloseUS1Click（转 `CloseDUserState1Dlg`）。</summary>
    public virtual void DCloseUS1Click(object Sender, int X, int Y)
    {
        CloseDUserState1Dlg();                              // 17808
    }

    // ==========================================================================================
    // 17854-17872  公会成员列表翻行（GuildTopLine 是本单元字段，**可直接落地并断言**）
    // ==========================================================================================

    /// <summary>
    /// FState.pas:17854-17860 procedure TFrmDlg.DGDUpClick。
    /// 原文**两步**：先 `if GuildTopLine > 0 then Dec(GuildTopLine, 3)`，
    /// 再 `if GuildTopLine < 0 then GuildTopLine := 0`（第二步在第一步保证下**恒不成立**，
    /// 是原文的冗余保护 —— 逐字保留，不删）。
    /// </summary>
    public virtual void DGDUpClick(object Sender, int X, int Y)
    {
        if (GuildTopLine > 0)                               // 17856
            GuildTopLine -= 3;                              // 17857（原文 Dec(GuildTopLine, 3)）
        if (GuildTopLine < 0)                               // 17858
            GuildTopLine = 0;                               // 17859
    }

    /// <summary>
    /// FState.pas:17862-17866 procedure TFrmDlg.DGDDownClick。
    /// 原文判据 `GuildTopLine + 12 &lt; GuildStrs.Count`（**严格小于**；不是 `&lt;=`），
    /// 命中才 `Inc(GuildTopLine, 3)`。
    /// </summary>
    public virtual void DGDDownClick(object Sender, int X, int Y)
    {
        if (GuildTopLine + 12 < GuildStrs.Count)            // 17864
            GuildTopLine += 3;                              // 17865（原文 Inc(GuildTopLine, 3)）
    }

    // ==========================================================================================
    // 17868-17926  公会对话框关闭/编辑入口
    // ==========================================================================================

    /// <summary>
    /// FState.pas:17868-17872 procedure TFrmDlg.DGDCloseClick。
    /// 原文**两步**：先 `CloseDGuildDlg`，再 `BoGuildChat := False`（顺序保留）。
    /// </summary>
    public virtual void DGDCloseClick(object Sender, int X, int Y)
    {
        CloseDGuildDlg();                                   // 17870
        BoGuildChat = false;                                // 17871
    }

    /// <summary>FState.pas:17912-17915 procedure TFrmDlg.DNewGuildDlgCloseClick（转 `CloseDGuildDlg_New`）。</summary>
    public virtual void DNewGuildDlgCloseClick(object Sender, int X, int Y)
    {
        CloseDGuildDlg_New();                               // 17914
    }

    /// <summary>FState.pas:17917-17920 procedure TFrmDlg.DNewGuildNoticeClick（转 `OpenDGuildEditNoticeDlg_New`）。</summary>
    public virtual void DNewGuildNoticeClick(object Sender, int X, int Y)
    {
        OpenDGuildEditNoticeDlg_New();                      // 17919
    }

    /// <summary>
    /// FState.pas:17906-17910 procedure TFrmDlg.DGDEditNoticeClick。
    /// 原文顺序：先把资源串解码后缓存到 `GuildEditHint`，再打开编辑框。
    /// </summary>
    public virtual void DGDEditNoticeClick(object Sender, int X, int Y)
    {
        GuildEditHint = FStateResStrSeam.DecodeResStr(FStateResStrSeam.SGuildEditNotice); // 17908
        OpenDGuildEditNoticeDlg();                          // 17909
    }

    /// <summary>
    /// FState.pas:17922-17926 procedure TFrmDlg.DGDEditGradeClick。
    /// 与 DGDEditNoticeClick 同形，缓存的是 `SGuildEditGradeHint`。
    /// </summary>
    public virtual void DGDEditGradeClick(object Sender, int X, int Y)
    {
        GuildEditHint = FStateResStrSeam.DecodeResStr(FStateResStrSeam.SGuildEditGradeHint); // 17924
        OpenGuildEditGradeDlg();                            // 17925
    }

    // ==========================================================================================
    // 18822-18938  底部按钮一族（关闭/打开转发）
    // ==========================================================================================

    /// <summary>FState.pas:18822-18825 procedure TFrmDlg.DCloseStateClick（转 `CloseDStateWinDlg`）。</summary>
    public virtual void DCloseStateClick(object Sender, int X, int Y)
    {
        CloseDStateWinDlg();                                // 18824
    }

    /// <summary>FState.pas:18827-18830 procedure TFrmDlg.DCloseBagClick（转 `CloseDItemBagDlg`）。</summary>
    public virtual void DCloseBagClick(object Sender, int X, int Y)
    {
        CloseDItemBagDlg();                                 // 18829
    }

    /// <summary>FState.pas:18832-18835 procedure TFrmDlg.DBotRankClick（转 `OpenDRankingDlg`）。</summary>
    public virtual void DBotRankClick(object Sender, int X, int Y)
    {
        OpenDRankingDlg();                                  // 18834
    }

    /// <summary>FState.pas:18837-18840 procedure TFrmDlg.DBotWhisperClick（转 `OpenDWhisperDlg`）。</summary>
    public virtual void DBotWhisperClick(object Sender, int X, int Y)
    {
        OpenDWhisperDlg();                                  // 18839
    }

    /// <summary>FState.pas:18868-18871 procedure TFrmDlg.DMissionDlgClick（转 `OpenDMissionDlg`）。</summary>
    public virtual void DMissionDlgClick(object Sender, int X, int Y)
    {
        OpenDMissionDlg();                                  // 18870
    }

    /// <summary>FState.pas:18873-18876 procedure TFrmDlg.DMissionDlgCloseClick（转 `CloseDMissionDlg`）。</summary>
    public virtual void DMissionDlgCloseClick(object Sender, int X, int Y)
    {
        CloseDMissionDlg();                                 // 18875
    }

    /// <summary>FState.pas:18878-18881 procedure TFrmDlg.DOpenShopClick（转 `OpenDShopDlg`）。</summary>
    public virtual void DOpenShopClick(object Sender, int X, int Y)
    {
        OpenDShopDlg();                                     // 18880
    }

    /// <summary>FState.pas:18888-18891 procedure TFrmDlg.DBotRankingCloseClick（转 `CloseDRankingDlg`）。</summary>
    public virtual void DBotRankingCloseClick(object Sender, int X, int Y)
    {
        CloseDRankingDlg();                                 // 18890
    }

    /// <summary>
    /// FState.pas:18898-18901 procedure TFrmDlg.DFrdCloseClick。
    /// 原文写的是 `CloseDFriendDlg()`（**带空括号**，与同族其它条目的无括号写法不同；原文如此）。
    /// </summary>
    public virtual void DFrdCloseClick(object Sender, int X, int Y)
    {
        CloseDFriendDlg();                                  // 18900
    }

    /// <summary>FState.pas:18935-18938 procedure TFrmDlg.DGrpDlgCloseClick（转 `CloseDGroupDlg`）。</summary>
    public virtual void DGrpDlgCloseClick(object Sender, int X, int Y)
    {
        CloseDGroupDlg();                                   // 18937
    }

    /// <summary>FState.pas:18996-18999 procedure TFrmDlg.DMyHeroStateCloseClick（转 `CloseDHeroStateWinDlg`）。</summary>
    public virtual void DMyHeroStateCloseClick(object Sender, int X, int Y)
    {
        CloseDHeroStateWinDlg();                            // 18998
    }

    /// <summary>FState.pas:19006-19009 procedure TFrmDlg.DMyHeroBagCloseClick（转 `CloseDHeroItemBagDlg`）。</summary>
    public virtual void DMyHeroBagCloseClick(object Sender, int X, int Y)
    {
        CloseDHeroItemBagDlg();                             // 19008
    }

    // ==========================================================================================
    // 24206-24314  卧龙（LieDragon）对话框关闭：直接置 Visible := False（可完整落地并断言）
    // ==========================================================================================

    /// <summary>FState.pas:24206-24209 procedure TFrmDlg.DLieDragonCloseClick。</summary>
    public virtual void DLieDragonCloseClick(object Sender, int X, int Y)
    {
        DLieDragon.Visible = false;                         // 24208
    }

    /// <summary>FState.pas:24311-24314 procedure TFrmDlg.DLieDragonNpcCloseClick。</summary>
    public virtual void DLieDragonNpcCloseClick(object Sender, int X, int Y)
    {
        DLieDragonNpc.Visible = false;                      // 24313
    }

    // ==========================================================================================
    // 切片 3：B-2 授权后解锁的四条 + 骑马两条
    //
    // 原文都是"一行转发给主窗体 / 角色"，被转发方在托管侧经 `FStateClMainSeam` 注入。
    // 接缝未注入时（默认）**什么都不发生** —— 这是**接缝**的既定语义，不是静默吞缺口：
    // 缺口本身登记在报告 §11.2 的 B-2，且注入点都是公开字段，测试可断言"确实转发了"。
    // ==========================================================================================

    /// <summary>
    /// FState.pas:20577-20580 procedure TFrmDlg.DWebClick。
    /// 原文 `frmMain.Navigate(g_ClientConfig.sHomePage)`。
    /// 注：车道1 的 `GXX.Client.GUI.Mir.TConfigClient`（MirForms.cs:23）目前**没有** `sHomePage`
    /// 字段，而它不在本车道分区，故该字段的最小承载在 `FStateClMainSeam.sHomePage`
    /// （默认值与 M2 端一致），见报告 D-P14-11。
    /// </summary>
    public virtual void DWebClick(object Sender, int X, int Y)
    {
        FStateClMainSeam.Navigate(FStateClMainSeam.sHomePage);   // 20579
    }

    /// <summary>
    /// FState.pas:20582-20585 procedure TFrmDlg.DActionLogClick。
    /// 原文 `frmMain.SendDActionLogClick;`（**无括号**，原文如此）。
    /// </summary>
    public virtual void DActionLogClick(object Sender, int X, int Y)
    {
        FStateClMainSeam.SendDActionLogClick();             // 20584
    }

    /// <summary>
    /// FState.pas:20592-20596 procedure TFrmDlg.DGetBackDeleteHumanClick。
    /// 原文先判 `g_SelDeleteHumanInfo.sChrName &lt;&gt; ''`，**非空才**发找回请求。
    /// </summary>
    public virtual void DGetBackDeleteHumanClick(object Sender, int X, int Y)
    {
        if (FStateMShareSeam.g_SelDeleteHumanInfo_sChrName != "")    // 20594
            FStateClMainSeam.SendGetBackDeleteChr(FStateMShareSeam.g_SelDeleteHumanInfo_sChrName); // 20595
    }

    /// <summary>
    /// FState.pas:24917-24922 procedure TFrmDlg.DCustomButtonClick。
    /// 原文只在 Sender **是** `TDxImageButton` 时才发消息（`is` 判定 ⇒ 托管侧同义 `is`）。
    /// </summary>
    public virtual void DCustomButtonClick(object Sender, int X, int Y)
    {
        if (Sender is TDxImageButton)                       // 24919
        {
            FStateClMainSeam.SendClientMessage(
                Grobal2Const.CM_CUSTOM_BUTTON_CLICK, ((TDxImageButton)Sender).Tag, 0, 0, 0, ""); // 24920
        }
    }

    /// <summary>
    /// FState.pas:20570-20575 procedure TFrmDlg.DDownHorseClick。
    /// 判据 `(g_MySelf.m_btHorse in [1, 2]) and (g_MySelf.m_btDoubleHumHorse = 0)`；
    /// **未判 g_MySelf 为 nil**（原文如此：未进场景时点这个按钮会 AV）。
    /// </summary>
    public virtual void DDownHorseClick(object Sender, int X, int Y)
    {
        if ((g_MySelf.m_btHorse == 1 || g_MySelf.m_btHorse == 2)     // 20572
            && g_MySelf.m_btDoubleHumHorse == 0)
        {
            FStateClMainSeam.TakeHorse(g_MySelf);           // 20573（原文 g_MySelf.TakeHorse）
        }
    }

    /// <summary>
    /// FState.pas:18842-18845 procedure TFrmDlg.DBotHorseClick。
    /// 原文**无条件** `g_MySelf.TakeHorse;`（同样未判 nil，原文如此）。
    /// </summary>
    public virtual void DBotHorseClick(object Sender, int X, int Y)
    {
        FStateClMainSeam.TakeHorse(g_MySelf);               // 18844
    }

    // ==========================================================================================
    // 切片 4：tick 守卫族（原文形态统一：一次比较 + 固定 +N 重装 + 一次转发）
    //
    // 这一族是**可完整断言**的：时钟经 `FStateSeamClock.NowHandler` 注入，
    // 于是 `>` 与 `>=`、以及 +3000 的重装窗口都能精确落点（不是靠真实时钟碰运气）。
    // 原文全部使用**严格大于**，且重装写在**守卫体内**（不是体外）。
    // ==========================================================================================

    /// <summary>
    /// FState.pas:17874-17881 procedure TFrmDlg.DGDHomeClick。
    /// 守卫 `MyGetTickCount &gt; g_dwQueryMsgTick`（**严格大于**）→ 重装 `+ 3000` →
    /// 转 `frmMain.SendGuildHome` → **守卫体内**置 `BoGuildChat := False`。
    /// </summary>
    public virtual void DGDHomeClick(object Sender, int X, int Y)
    {
        if (FStateSeamClock.Now > FStateMShareSeam.g_dwQueryMsgTick)   // 17876
        {
            FStateMShareSeam.g_dwQueryMsgTick = FStateSeamClock.Now + 3000;   // 17877
            FStateClMainSeam.SendGuildHome();           // 17878
            BoGuildChat = false;                        // 17879
        }
    }

    /// <summary>
    /// FState.pas:17883-17890 procedure TFrmDlg.DGDListClick。
    /// 与 DGDHomeClick 同形，转发目标换成 `frmMain.SendGuildMemberList`。
    /// </summary>
    public virtual void DGDListClick(object Sender, int X, int Y)
    {
        if (FStateSeamClock.Now > FStateMShareSeam.g_dwQueryMsgTick)   // 17885
        {
            FStateMShareSeam.g_dwQueryMsgTick = FStateSeamClock.Now + 3000;   // 17886
            FStateClMainSeam.SendGuildMemberList();     // 17887
            BoGuildChat = false;                        // 17888
        }
    }

    // ==========================================================================================
    // 18832-18896  排行榜/好友/商铺入口（转发）
    // ==========================================================================================

    /// <summary>
    /// FState.pas:20565-20568 procedure TFrmDlg.DBotUserShopClick。
    /// 原文 `OpenDGameShopDlg;` —— 被调方法有默认参 `IsCheckTime:Boolean = True`（原文声明 946），
    /// 故托管侧同样**不传参**（等价于传 true）。
    /// </summary>
    public virtual void DBotUserShopClick(object Sender, int X, int Y)
    {
        OpenDGameShopDlg();                                 // 20567
    }

    /// <summary>FState.pas:18883-18886 procedure TFrmDlg.DBotRankingClick（转 `OpenDRankingDlg`）。</summary>
    public virtual void DBotRankingClick(object Sender, int X, int Y)
    {
        OpenDRankingDlg();                                  // 18885
    }

    /// <summary>
    /// FState.pas:18893-18896 procedure TFrmDlg.DBotFriendClick。
    /// 原文写的是 `OpenDFriendDlg();`（**带空括号**，与 `DBotRankClick` 的无括号写法不同；原文如此）。
    /// </summary>
    public virtual void DBotFriendClick(object Sender, int X, int Y)
    {
        OpenDFriendDlg();                                   // 18895
    }

    // ==========================================================================================
    // 切片 5：交易 / 挑战的"守卫 + 转发"族
    //
    // 与切片 4 同源，但守卫用的是**各自**的动作时间戳，且 `*ZeroGold` 两条多一个 `not *End` 前置判据。
    // 全部可完整断言（时钟注入）。
    // ==========================================================================================

    /// <summary>
    /// FState.pas:18912-18918 procedure TFrmDlg.DBotTradeClick。
    /// 守卫 `Now &gt; g_dwQueryMsgTick` → 重装 `+3000` → 转 `frmMain.SendDealTry`。
    /// </summary>
    public virtual void DBotTradeClick(object Sender, int X, int Y)
    {
        if (FStateSeamClock.Now > FStateMShareSeam.g_dwQueryMsgTick)   // 18914
        {
            FStateMShareSeam.g_dwQueryMsgTick = FStateSeamClock.Now + 3000;   // 18915
            FStateClMainSeam.SendDealTry();             // 18916
        }
    }

    /// <summary>
    /// FState.pas:18904-18910 procedure TFrmDlg.BotChallengeClick（注意：原文**无 D 前缀**）。
    /// 与 DBotTradeClick 同形，转发 `frmMain.SendChallengeTry`。
    /// </summary>
    public virtual void BotChallengeClick(object Sender, int X, int Y)
    {
        if (FStateSeamClock.Now > FStateMShareSeam.g_dwQueryMsgTick)   // 18906
        {
            FStateMShareSeam.g_dwQueryMsgTick = FStateSeamClock.Now + 3000;   // 18907
            FStateClMainSeam.SendChallengeTry();        // 18908
        }
    }

    /// <summary>
    /// FState.pas:17533-17539 procedure TFrmDlg.DDealCloseClick。
    /// 守卫 `Now &gt; g_dwDealActionTick` → 关交易对话框 → 转 `frmMain.SendCancelDeal`；
    /// **不动** g_dwDealActionTick（原文如此：这里不重装，重装在 DealZeroGold）。
    /// </summary>
    public virtual void DDealCloseClick(object Sender, int X, int Y)
    {
        if (FStateSeamClock.Now > FStateMShareSeam.g_dwDealActionTick)   // 17535
        {
            CloseDDealDlg();                            // 17536
            FStateClMainSeam.SendCancelDeal();          // 17537
        }
    }

    /// <summary>
    /// FState.pas:17746-17752 procedure TFrmDlg.DealZeroGold。
    /// 前置判据 `not g_boDealEnd and (g_nDealGold &gt; 0)`（**两个都成立才**）→
    /// 重装 `g_dwDealActionTick := Now + 4000` → 转 `frmMain.SendChangeDealGold(0)`。
    /// </summary>
    public virtual void DealZeroGold()
    {
        if (!FStateMShareSeam.g_boDealEnd && FStateMShareSeam.g_nDealGold > 0)   // 17748
        {
            FStateMShareSeam.g_dwDealActionTick = FStateSeamClock.Now + 4000;   // 17749
            FStateClMainSeam.SendChangeDealGold(0);     // 17750
        }
    }

    /// <summary>
    /// FState.pas:20813-20819 procedure TFrmDlg.DChallengeCloseClick。
    /// 与 DDealCloseClick 同形，守卫用 `g_dwChallengeActionTick`，转 `frmMain.SendCancelChallenge`。
    /// </summary>
    public virtual void DChallengeCloseClick(object Sender, int X, int Y)
    {
        if (FStateSeamClock.Now > FStateMShareSeam.g_dwChallengeActionTick)   // 20815
        {
            CloseDChallengeDlg();                       // 20816
            FStateClMainSeam.SendCancelChallenge();     // 20817
        }
    }

    /// <summary>
    /// FState.pas:20789-20795 procedure TFrmDlg.ChallengeZeroGold。
    /// 与 DealZeroGold 同形（挑战侧的镜像），重装 `+4000`，转 `SendChangeChallengeGold(0)`。
    /// </summary>
    public virtual void ChallengeZeroGold()
    {
        if (!FStateMShareSeam.g_boChallengeEnd && FStateMShareSeam.g_nChallengeGold > 0)   // 20791
        {
            FStateMShareSeam.g_dwChallengeActionTick = FStateSeamClock.Now + 4000;   // 20792
            FStateClMainSeam.SendChangeChallengeGold(0);    // 20793
        }
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

    /// <summary>
    /// 切片 2 落地的成员（25 条）：关闭/打开转发面 + 公会列表翻行 + 卧龙关闭。
    /// ★ 其中 20 条是"转发给仍在 throw 的壳"（见实现处注释）—— 本表只声称**转发体**已 1:1 落地。
    /// </summary>
    public static readonly IReadOnlyList<PortedMember> Slice2 = new[]
    {
        new PortedMember("DSellDlgCloseClick",         "17362-17365"),
        new PortedMember("DMenuCloseClick",            "17446-17449"),
        new PortedMember("DKsOkClick",                 "17506-17509"),
        new PortedMember("DCloseUS1Click",             "17806-17809"),
        new PortedMember("DGDUpClick",                 "17854-17860"),
        new PortedMember("DGDDownClick",               "17862-17866"),
        new PortedMember("DGDCloseClick",              "17868-17872"),
        new PortedMember("DGDEditNoticeClick",         "17906-17910"),
        new PortedMember("DNewGuildDlgCloseClick",     "17912-17915"),
        new PortedMember("DNewGuildNoticeClick",       "17917-17920"),
        new PortedMember("DGDEditGradeClick",          "17922-17926"),
        new PortedMember("DCloseStateClick",           "18822-18825"),
        new PortedMember("DCloseBagClick",             "18827-18830"),
        new PortedMember("DBotRankClick",              "18832-18835"),
        new PortedMember("DBotWhisperClick",           "18837-18840"),
        new PortedMember("DMissionDlgClick",           "18868-18871"),
        new PortedMember("DMissionDlgCloseClick",      "18873-18876"),
        new PortedMember("DOpenShopClick",             "18878-18881"),
        new PortedMember("DBotRankingCloseClick",      "18888-18891"),
        new PortedMember("DFrdCloseClick",             "18898-18901"),
        new PortedMember("DGrpDlgCloseClick",          "18935-18938"),
        new PortedMember("DMyHeroStateCloseClick",     "18996-18999"),
        new PortedMember("DMyHeroBagCloseClick",       "19006-19009"),
        new PortedMember("DLieDragonCloseClick",       "24206-24209"),
        new PortedMember("DLieDragonNpcCloseClick",    "24311-24314"),
    };

    /// <summary>
    /// 切片 3 落地的成员（6 条）：B-2（frmMain 接缝）授权后解锁的四条 + 骑马两条。
    /// </summary>
    public static readonly IReadOnlyList<PortedMember> Slice3 = new[]
    {
        new PortedMember("DBotHorseClick",             "18842-18845"),
        new PortedMember("DDownHorseClick",            "20570-20575"),
        new PortedMember("DWebClick",                  "20577-20580"),
        new PortedMember("DActionLogClick",            "20582-20585"),
        new PortedMember("DGetBackDeleteHumanClick",   "20592-20596"),
        new PortedMember("DCustomButtonClick",         "24917-24922"),
    };

    /// <summary>
    /// 切片 4 落地的成员（5 条）：tick 守卫族两条 + 商铺/排行/好友入口三条。
    /// </summary>
    public static readonly IReadOnlyList<PortedMember> Slice4 = new[]
    {
        new PortedMember("DGDHomeClick",               "17874-17881"),
        new PortedMember("DGDListClick",               "17883-17890"),
        new PortedMember("DBotRankingClick",           "18883-18886"),
        new PortedMember("DBotFriendClick",            "18893-18896"),
        new PortedMember("DBotUserShopClick",          "20565-20568"),
    };

    /// <summary>
    /// 切片 5 落地的成员（6 条）：交易/挑战的"守卫 + 转发"族。
    /// </summary>
    public static readonly IReadOnlyList<PortedMember> Slice5 = new[]
    {
        new PortedMember("DDealCloseClick",            "17533-17539"),
        new PortedMember("DealZeroGold",               "17746-17752"),
        new PortedMember("BotChallengeClick",          "18904-18910"),
        new PortedMember("DBotTradeClick",             "18912-18918"),
        new PortedMember("ChallengeZeroGold",          "20789-20795"),
        new PortedMember("DChallengeCloseClick",       "20813-20819"),
    };

    /// <summary>全部已登记切片（后继切片在这里追加）。</summary>
    public static readonly IReadOnlyList<IReadOnlyList<PortedMember>> AllSlices =
        new[] { Slice1, Slice2, Slice3, Slice4, Slice5 };

    /// <summary>切片 1 的真实现成员数。</summary>
    public static int Slice1Count => Slice1.Count;

    /// <summary>切片 2 的真实现成员数。</summary>
    public static int Slice2Count => Slice2.Count;

    /// <summary>切片 3 的真实现成员数。</summary>
    public static int Slice3Count => Slice3.Count;

    /// <summary>切片 4 的真实现成员数。</summary>
    public static int Slice4Count => Slice4.Count;

    /// <summary>切片 5 的真实现成员数。</summary>
    public static int Slice5Count => Slice5.Count;

    /// <summary>由本车道（p14）落地的成员总数（切片 1..5）。</summary>
    public static int LaneCount =>
        Slice1.Count + Slice2.Count + Slice3.Count + Slice4.Count + Slice5.Count;

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
