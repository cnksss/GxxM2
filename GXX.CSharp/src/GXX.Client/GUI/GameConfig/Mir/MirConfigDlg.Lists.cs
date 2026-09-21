// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   6877-6939  DLabelKeyBoardKeyDown
//   6940-6955  DLabelKeyBoardMouseDown
//   6956-6971  DMemoBossListClick
//   6972-6984  CheckBossNameExists
//   6985-7004  DBtnBossAddClick
//   7005-7013  DBtnBossDelClick
//   7014-7032  DBtnBossModifyClick
//   7033-7043  SaveOrLoadBossList
//   7074-7102  DBtnDiyAddClick
//   7103-7142  DBtnDiyDelClick
//   7143-7310  DBtnItemsImportOrExportClick
//   7357-7493  PlugBtnDiyEditClick
//   7494-7498  DBtnGJPageControlClick
//   7499-7514  DMemoGJMonListClick
//   7515-7527  CheckGJMonNameExists
//   7528-7547  DBtnGJMonNameAddClick
//   7548-7556  DBtnGJMonNameDelClick
//   7557-7575  DBtnGJMonNameEditClick
//   7576-7604  SaveOrLoadGJMonList
//   7605-7660  SaveOrLoadGJMagicList1
//   7661-7715  SaveOrLoadGJMagicList2
//   7816-7824  ListViewGJMagicItemClick
//   7825-7832  DBtnGJRunClick
//   7833-7837  PlugBtnGJPointClick
//   7838-7902  RefreshGJMagic
//   8183-8194  RefBindItemList
//   8195-8203  AddToBossList
//   8204-8214  RemoveFromBossList
//   8215-8227  AddOrRemoveBossList
//   8228-8259  PlugScrollBoxUnbindItemsClick
//   8260-8355  PlugBtnUnbindItemClick
//
// ⏳ 骨架：签名 + 原文行号 + 语义摘要已就位；逐行体见报告 §未完成/阻塞项。

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

public partial class TMirConfigDlg
{
    // ================================================================================
    // 6877-6984  Boss 名单
    // ================================================================================

    /// <summary>原文 6877-6939：<c>DLabelKeyBoardKeyDown</c>（16 个快捷键标签共用，按 Tag 区分）。</summary>
    public void DLabelKeyBoardKeyDown(object Sender, ref ushort Key, DelphiShiftState Shift)
        => NotPorted(nameof(DLabelKeyBoardKeyDown), 6877);

    /// <summary>原文 6940-6955：<c>DLabelKeyBoardMouseDown</c>（16 处绑定）。</summary>
    public void DLabelKeyBoardMouseDown(object Sender, int Button, DelphiShiftState Shift, int X, int Y)
        => NotPorted(nameof(DLabelKeyBoardMouseDown), 6940);

    /// <summary>原文 6956-6971：<c>DMemoBossListClick</c>。</summary>
    public void DMemoBossListClick(object Sender, int X, int Y) => NotPorted(nameof(DMemoBossListClick), 6956);

    /// <summary>原文 6972-6984：<c>CheckBossNameExists(Name, CurIndex = -1)</c>。</summary>
    public bool CheckBossNameExists(string Name, int CurIndex = -1) => NotPortedBool(nameof(CheckBossNameExists), 6972);

    /// <summary>原文 6985-7004：<c>DBtnBossAddClick</c>。</summary>
    public void DBtnBossAddClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnBossAddClick), 6985);

    /// <summary>原文 7005-7013：<c>DBtnBossDelClick</c>。</summary>
    public void DBtnBossDelClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnBossDelClick), 7005);

    /// <summary>原文 7014-7032：<c>DBtnBossModifyClick</c>。</summary>
    public void DBtnBossModifyClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnBossModifyClick), 7014);

    /// <summary>原文 7033-7043：<c>SaveOrLoadBossList(IsSave)</c>。</summary>
    public void SaveOrLoadBossList(bool IsSave) => NotPorted(nameof(SaveOrLoadBossList), 7033);

    // ================================================================================
    // 7074-7493  特殊物品自定义（DIY）
    // ================================================================================

    /// <summary>原文 7074-7102：<c>DBtnDiyAddClick</c>。</summary>
    public void DBtnDiyAddClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnDiyAddClick), 7074);

    /// <summary>原文 7103-7142：<c>DBtnDiyDelClick</c>（原文缺陷：失败时用 <c>Exit</c> 而非 return 值）。</summary>
    public void DBtnDiyDelClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnDiyDelClick), 7103);

    /// <summary>原文 7143-7310：<c>DBtnItemsImportOrExportClick</c>（物品过滤表导入/导出，含 DMessageDlg）。</summary>
    public void DBtnItemsImportOrExportClick(object Sender, int X, int Y)
        => NotPorted(nameof(DBtnItemsImportOrExportClick), 7143);

    /// <summary>原文 7357-7493：<c>PlugBtnDiyEditClick</c>。</summary>
    public void PlugBtnDiyEditClick(object Sender, int X, int Y) => NotPorted(nameof(PlugBtnDiyEditClick), 7357);

    // ================================================================================
    // 7494-7902  挂机页（不打怪名单 / 技能表 / 群攻 / 运行）
    // ================================================================================

    /// <summary>原文 7494-7498：<c>DBtnGJPageControlClick</c>（**3 处绑定**）。</summary>
    public void DBtnGJPageControlClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnGJPageControlClick), 7494);

    /// <summary>原文 7499-7514：<c>DMemoGJMonListClick</c>。</summary>
    public void DMemoGJMonListClick(object Sender, int X, int Y) => NotPorted(nameof(DMemoGJMonListClick), 7499);

    /// <summary>原文 7515-7527：<c>CheckGJMonNameExists(Name, CurIndex = -1)</c>。</summary>
    public bool CheckGJMonNameExists(string Name, int CurIndex = -1) => NotPortedBool(nameof(CheckGJMonNameExists), 7515);

    /// <summary>原文 7528-7547：<c>DBtnGJMonNameAddClick</c>。</summary>
    public void DBtnGJMonNameAddClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnGJMonNameAddClick), 7528);

    /// <summary>原文 7548-7556：<c>DBtnGJMonNameDelClick</c>。</summary>
    public void DBtnGJMonNameDelClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnGJMonNameDelClick), 7548);

    /// <summary>原文 7557-7575：<c>DBtnGJMonNameEditClick</c>。</summary>
    public void DBtnGJMonNameEditClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnGJMonNameEditClick), 7557);

    /// <summary>原文 7576-7604：<c>SaveOrLoadGJMonList(IsSave)</c>。</summary>
    public void SaveOrLoadGJMonList(bool IsSave) => NotPorted(nameof(SaveOrLoadGJMonList), 7576);

    /// <summary>原文 7605-7660：<c>SaveOrLoadGJMagicList1(IsSave)</c>。</summary>
    public void SaveOrLoadGJMagicList1(bool IsSave) => NotPorted(nameof(SaveOrLoadGJMagicList1), 7605);

    /// <summary>原文 7661-7715：<c>SaveOrLoadGJMagicList2(IsSave)</c>。</summary>
    public void SaveOrLoadGJMagicList2(bool IsSave) => NotPorted(nameof(SaveOrLoadGJMagicList2), 7661);

    /// <summary>原文 7816-7824：<c>ListViewGJMagicItemClick</c>（**2 处绑定**：PlugMemoConfig82/83）。</summary>
    public void ListViewGJMagicItemClick(object Sender, int ARow, int ACol, object ListItem, IntPtr ViewItem)
        => NotPorted(nameof(ListViewGJMagicItemClick), 7816);

    /// <summary>原文 7825-7832：<c>DBtnGJRunClick</c>（开始/停止挂机）。</summary>
    public void DBtnGJRunClick(object Sender, int X, int Y) => NotPorted(nameof(DBtnGJRunClick), 7825);

    /// <summary>原文 7833-7837：<c>PlugBtnGJPointClick</c>。</summary>
    public void PlugBtnGJPointClick(object Sender, int X, int Y) => NotPorted(nameof(PlugBtnGJPointClick), 7833);

    /// <summary>原文 7838-7902：<c>RefreshGJMagic</c>（把 g_MagicList 灌进两页技能表）。</summary>
    public void RefreshGJMagic() => NotPorted(nameof(RefreshGJMagic), 7838);

    // ================================================================================
    // 8183-8355  自定义绑定物品页 + Boss 名单对外接口
    // ================================================================================

    /// <summary>原文 8183-8194：<c>RefBindItemList</c>（重建 PlugScrollBoxUnbindItems 行）。</summary>
    public void RefBindItemList() => NotPorted(nameof(RefBindItemList), 8183);

    /// <summary>原文 8195-8203：<c>AddToBossList(sName)</c>。</summary>
    public override void AddToBossList(string sName) => NotPorted(nameof(AddToBossList), 8195);

    /// <summary>原文 8204-8214：<c>RemoveFromBossList(sName)</c>。</summary>
    public override void RemoveFromBossList(string sName) => NotPorted(nameof(RemoveFromBossList), 8204);

    /// <summary>原文 8215-8227：<c>AddOrRemoveBossList(sName)</c>（存在即删，否则加）。</summary>
    public override void AddOrRemoveBossList(string sName) => NotPorted(nameof(AddOrRemoveBossList), 8215);

    /// <summary>原文 8228-8259：<c>PlugScrollBoxUnbindItemsClick</c>（选中行 → 回填右侧编辑区）。</summary>
    public void PlugScrollBoxUnbindItemsClick(object Sender, int X, int Y)
        => NotPorted(nameof(PlugScrollBoxUnbindItemsClick), 8228);

    /// <summary>原文 8260-8355：<c>PlugBtnUnbindItemClick</c>（**4 处绑定**：Add/Del/Edit/Save 共用）。</summary>
    public void PlugBtnUnbindItemClick(object Sender, int X, int Y)
        => NotPorted(nameof(PlugBtnUnbindItemClick), 8260);

    private bool NotPortedBool(string method, int line) { NotPorted(method, line); return false; }
}
