using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.DBServer;
using GXX.DBServer.Forms;
using GXX.DBServer.Tests;

namespace GXX.DBServer.Forms.Tests;

// ============================================================================================
// 车道 p10-db-login-forms / uFrmRoleDataEdit 的测试替身与接缝装配。
//
//   · P10FakeHumanDb / P10FakeHeroDb：THumanDBBase / THeroDBBase 的内存实现
//     （**只**接本单元触达的 Save，其余 Do* 一旦被调到就抛，便于"接线错了"立刻可见）。
//     ★ 不复用 SelectClientTestDoubles.cs 的 FakeSelectHumanDB：那个替身的 DoSave 是
//       `throw new InvalidOperationException("SelectClient 不应调用 Save")`（**只读**文件，不得修改）。
//   · P10RoleDataEditScope：把 uFrmRoleDataEdit 用到的 4 组静态接缝装好并在 Dispose 里复位，
//     使单测**永不弹真实对话框 / 永不阻塞 / 永不连库**。
// ============================================================================================

/// <summary>内存版 <c>THumanDBBase</c>（RoleDB.pas:117-209）：只实现 <c>Save</c> 路径。</summary>
public sealed class P10FakeHumanDb : THumanDBBase
{
    /// <summary>DoSave 的返回值（默认 True = 保存成功）。</summary>
    public bool SaveResult = true;

    /// <summary>非 null 时 DoSave 抛这个异常（用于验证 THumanDBBase 公开包装的"吞异常"语义）。</summary>
    public string ThrowOnSaveMessage;

    /// <summary>每一次 Save 的入参快照（编号 + 记录里几个可辨识字段）。</summary>
    public readonly List<(int HumanID, string ChrName, string StoragePwd, uint Gold, int Level)> SaveCalls = new();

    public int LockCount;
    public int UnLockCount;

    protected override void OwnerLock() => LockCount++;
    protected override void OwnerUnLock() => UnLockCount++;

    protected override bool DoSave(int HumanID, ref THumData HumData)
    {
        SaveCalls.Add((HumanID, HumData.ChrName, HumData.StoragePwd, HumData.nGold, HumData.Abil.Level));
        if (ThrowOnSaveMessage != null) throw new InvalidOperationException(ThrowOnSaveMessage);
        return SaveResult;
    }

    // ---- 本单元不会调到的 Do*：被调到就说明接线错了（与 SelectClient 替身同一处置） ----
    protected override int DoGetID(string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetID");
    protected override bool DoCheckHumanExists(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.CheckHumanExists");
    protected override int DoGetHumanCount(string Account) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetHumanCount");
    protected override string DoGetOtherHumanName(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetOtherHumanName");
    protected override bool DoGetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetHumanHeroName");
    protected override bool DoGetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetBaseInfo");
    protected override int DoQueryHumans(string Account, TQueryHumanList HumanList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.QueryHumans");
    protected override int DoQueryDeleteHumans(string Account, TQueryHumanList HumanList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.QueryDeleteHumans");
    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.SearchByAccount");
    protected override int DoSearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.SearchByName");
    protected override int DoSearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.SearchByLevel");
    protected override int DoGetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetMobileNumbers");
    protected override bool DoSelect(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.Select");
    protected override bool DoGet(string Account, string HumanName, ref THumData HumData, out int HumanID) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.Get");
    protected override bool DoAdd(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.Add");
    protected override bool DoDelete(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.Delete");
    protected override bool DoDeleteRestore(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.DeleteRestore");
    protected override bool DoSetEnabled(string Account, string HumanName, int Enabled) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.SetEnabled");
    protected override bool DoErase(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.Erase");
    protected override bool DoRecordLoginTime(string Account, string HumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.RecordLoginTime");
    protected override bool DoRename(string Account, string HumanName, int HumanID, string NewName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.Rename");
    protected override bool DoChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.ChangedGold");
    protected override bool DoChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.ChangedCustomMoney");
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount, TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList, TRoleRankList TaoistRankList, TRoleRankList MasterRankList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.GetRankData");
    protected override bool DoBuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HumanDB.BuyPlayer");
}

/// <summary>内存版 <c>THeroDBBase</c>（RoleDB.pas:211-261）：只实现 <c>Save</c> 路径。</summary>
public sealed class P10FakeHeroDb : THeroDBBase
{
    /// <summary>DoSave 的返回值（默认 True）。</summary>
    public bool SaveResult = true;

    /// <summary>每一次 Save 的入参快照。</summary>
    public readonly List<(int HeroID, string ChrName, string Account, int Level)> SaveCalls = new();

    protected override void OwnerLock() { }
    protected override void OwnerUnLock() { }

    protected override bool DoSave(int HeroID, ref THeroData HeroData)
    {
        SaveCalls.Add((HeroID, HeroData.ChrName, HeroData.Account, HeroData.Abil.Level));
        return SaveResult;
    }

    protected override int DoGetID(string HeroName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.GetID");
    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.SearchByAccount");
    protected override int DoSearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.SearchByName");
    protected override bool DoGet(string HeroName, ref THeroData HeroData, out int HeroID) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.Get");
    protected override bool DoAdd(string Account, string HumanName, int HumanID, string HeroName, byte Sex, byte Job, byte Hair, bool IsDeputyHero) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.Add");
    protected override bool DoErase(string HeroName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.Erase");
    protected override bool DoRename(int HeroID, string HeroName, string NewName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.Rename");
    protected override bool DoAssess(int HeroID, string HeroName, string DeputyHeroName) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.Assess");
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount, TRoleRankList HeroRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList, TRoleRankList TaoistRankList) => throw new InvalidOperationException("uFrmRoleDataEdit 不应调用 HeroDB.GetRankData");
}

/// <summary>
/// 把 uFrmRoleDataEdit 的静态接缝装好并在 <see cref="Dispose"/> 里复位。
/// 装上之后的保证：**不会弹真实对话框、不会阻塞、不会连库**。
/// </summary>
public sealed class P10RoleDataEditScope : IDisposable
{
    /// <summary>消息框记录器（UiSeam.MessageBox 全部落这里）。</summary>
    public readonly UiRecorder Ui = new UiRecorder();

    public readonly P10FakeHumanDb HumanDb = new P10FakeHumanDb();
    public readonly P10FakeHeroDb HeroDb = new P10FakeHeroDb();

    /// <summary>GetMagicName 的桩数据（wMagicId → 名字）。</summary>
    public readonly Dictionary<ushort, string> MagicNames = new Dictionary<ushort, string>();

    /// <summary>GetStdItemName 的桩数据（wIndex → 名字）。</summary>
    public readonly Dictionary<int, string> StdItemNames = new Dictionary<int, string>();

    /// <summary>ProcessSaveDataToFile：SaveDialog.Execute 的返回值（False = 用户取消）。</summary>
    public bool SaveDialogResult;

    /// <summary>ProcessSaveDataToFile：Execute 返回 True 时写进 SaveDialog.FileName 的路径。</summary>
    public string SaveDialogFileName = "";

    /// <summary>ProcessLoadDataformFile：OpenDialog.Execute 的返回值。</summary>
    public bool OpenDialogResult;

    /// <summary>ProcessLoadDataformFile：Execute 返回 True 时写进 OpenDialog.FileName 的路径。</summary>
    public string OpenDialogFileName = "";

    /// <summary>ShowModalHandler 收到的窗体引用（`ShowFrmRoleDataEdit` 返回后应当已 Dispose）。</summary>
    public readonly List<TFrmRoleDataEdit> ShownForms = new List<TFrmRoleDataEdit>();

    /// <summary>ShowModal 时的窗体状态快照（窗体随后被 Free，故必须在此刻取）。</summary>
    public readonly List<(int FID, bool FIsHuman, string Text, string EdtID, string ChrName)> Shown = new();

    public P10RoleDataEditScope()
    {
        SelectClientRoleDbSeam.HumanDB = HumanDb;
        SelectClientRoleDbSeam.HeroDB = HeroDb;

        RoleDataEditDbShareSeam.GetMagicName = (wMagicId, MagicAttr) =>
            MagicNames.TryGetValue(wMagicId, out string name) ? name : "";
        RoleDataEditDbShareSeam.GetStdItemName = nPosition =>
            StdItemNames.TryGetValue(nPosition, out string name) ? name : "";

        RoleDataEditFileDialogSeam.SaveDialogExecute = frm =>
        {
            if (!SaveDialogResult) return false;
            frm.SaveDialog.FileName = SaveDialogFileName;
            return true;
        };
        RoleDataEditFileDialogSeam.OpenDialogExecute = frm =>
        {
            if (!OpenDialogResult) return false;
            frm.OpenDialog.FileName = OpenDialogFileName;
            return true;
        };

        TFrmRoleDataEdit.ShowModalHandler = frm =>
        {
            ShownForms.Add(frm);
            string chrName = frm.FIsHuman ? frm.FHumData.ChrName : frm.FHeroData.ChrName;
            Shown.Add((frm.FID, frm.FIsHuman, frm.Text, frm.edtID.Text, chrName));
            return false;   // 模态结果：本单元不读它，只要**不阻塞**即可
        };

        DelphiFileIo.ResetAll();
    }

    public void Dispose()
    {
        TFrmRoleDataEdit.ShowModalHandler = null;
        RoleDataEditFileDialogSeam.Reset();
        RoleDataEditDbShareSeam.Reset();
        SelectClientRoleDbSeam.Reset();
        DelphiFileIo.ResetAll();
        Ui.Dispose();
    }
}
