using System;
using System.Collections.Generic;
using System.Threading;
using GXX.Core.Protocol;

namespace GXX.DBServer;

// ============================================================================================
// 接缝适配：把 <see cref="RoleDatabase"/> 适配成 SelectClient.pas 需要的
// `g_RoleDB.HumanDB: THumanDB` / `g_RoleDB.HeroDB: THeroDB`（RoleDB.pas:117-261）。
//
// ★ 适用范围（**刻意收窄**）：这两个适配器**只实现 SelectClient.pas 真正调用的 Do\***
//   （Human 9 个 / Hero 1 个），其余成员一律 `NotSupportedException`。
//   注意 `THumanDBBase` 的公开包装会 `catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }`
//   —— 也就是说**越界调用不会崩，只会记一条日志并返回初值**（这是 `MySqlRoleDB.Base.cs` 的原文形状，
//   本车道无权改它）。因此这里把**例外信息写清楚**，让那条日志能直接指出"该成员未接线"。
//   ⇒ 不要把这个适配器当成通用 g_RoleDB；通用上下文应当用已移植的 `TMySqlRoleDB`。
//
// 原文对照（每个方法的 SQL 都在 RoleDatabase.cs 的注释里逐字列出）：
//     GetID               THumanDB    RoleDB.pas:533-549   →  MySqlRoleDB.pas:839-851
//     GetHumanCount       THumanDB    RoleDB.pas:569-585   →  :869-881
//     GetBaseInfo         THumanDB    RoleDB.pas:623-639   →  :915-932
//     QueryHumans         THumanDB    RoleDB.pas:641-657   →  :934-968
//     QueryDeleteHumans   THumanDB    RoleDB.pas:659-675   →  :970-998
//     Select              THumanDB    RoleDB.pas:752-768   →  :1180-1208
//     Add                 THumanDB    RoleDB.pas:794-814   →  :1890-1906
//     Delete              THumanDB    RoleDB.pas:816-829   →  :1908-1919
//     DeleteRestore       THumanDB    RoleDB.pas:831-847   →  :1921-1932
//     (Hero) GetID        THeroDB     RoleDB.pas:1030-1047 →  :5960-...
// ============================================================================================

/// <summary>
/// <c>g_RoleDB.HumanDB</c>（THumanDB）的 SelectClient 专用适配器。
/// 已接线：GetID / GetHumanCount / GetBaseInfo / QueryHumans / QueryDeleteHumans /
/// Select / Add / Delete / DeleteRestore —— 即 SelectClient.pas 的全部 HumanDB 调用面。
/// </summary>
public sealed class SelectClientHumanDb : THumanDBBase
{
    private readonly RoleDatabase _db;

    public SelectClientHumanDb(RoleDatabase db) => _db = db ?? throw new ArgumentNullException(nameof(db));

    /// <summary>原文 `FOwner.Lock`（TRoleDB 的临界区）。本适配器直接复用 RoleDatabase 的同步对象。</summary>
    protected override void OwnerLock() => Monitor.Enter(_db.SyncRoot);

    protected override void OwnerUnLock() => Monitor.Exit(_db.SyncRoot);

    protected override int DoGetID(string HumanName) => _db.GetHumanId(HumanName);

    protected override int DoGetHumanCount(string Account) => _db.GetHumanCount(Account);

    protected override bool DoGetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin)
        => _db.GetBaseInfo(HumanName, out Sex, out Job, out Level, out LastLogin);

    protected override int DoQueryHumans(string Account, TQueryHumanList HumanList) => _db.QueryHumans(Account, HumanList);

    protected override int DoQueryDeleteHumans(string Account, TQueryHumanList HumanList) => _db.QueryDeleteHumans(Account, HumanList);

    protected override bool DoSelect(string Account, string HumanName) => _db.SelectHuman(Account, HumanName);

    protected override bool DoAdd(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair)
        => _db.AddHuman(Account, HumanName, IsSelect, Sex, Job, Hair);

    protected override bool DoDelete(string Account, string HumanName) => _db.DeleteHuman(Account, HumanName);

    protected override bool DoDeleteRestore(string Account, string HumanName) => _db.DeleteRestoreHuman(Account, HumanName);

    // ---- 以下成员 SelectClient.pas **不调用** ⇒ 未接线（不是"返回中性值"，是显式抛）----

    private static NotSupportedException Unwired(string member) => new NotSupportedException(
        "接缝：SelectClientHumanDb 只接线了 SelectClient.pas 用到的 9 个 Do*，" + member + " 未接线。" +
        "通用 THumanDB 请用 TMySqlRoleDB.HumanDB；或移植 RoleDB.pas/MySqlRoleDB.pas 的对应实现后在本适配器补上。");

    protected override bool DoCheckHumanExists(string Account, string HumanName) => throw Unwired("CheckHumanExists");
    protected override string DoGetOtherHumanName(string Account, string HumanName) => throw Unwired("GetOtherHumanName");
    protected override bool DoGetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName) => throw Unwired("GetHumanHeroName");
    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw Unwired("SearchByAccount");
    protected override int DoSearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw Unwired("SearchByName");
    protected override int DoSearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList) => throw Unwired("SearchByLevel");
    protected override int DoGetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList) => throw Unwired("GetMobileNumbers");
    protected override bool DoGet(string Account, string HumanName, ref THumData HumData, out int HumanID) => throw Unwired("Get");
    protected override bool DoSetEnabled(string Account, string HumanName, int Enabled) => throw Unwired("SetEnabled");
    protected override bool DoErase(string Account, string HumanName) => throw Unwired("Erase");
    protected override bool DoRecordLoginTime(string Account, string HumanName) => throw Unwired("RecordLoginTime");
    protected override bool DoSave(int HumanID, ref THumData HumData) => throw Unwired("Save");
    protected override bool DoRename(string Account, string HumanName, int HumanID, string NewName) => throw Unwired("Rename");
    protected override bool DoChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue) => throw Unwired("ChangedGold");
    protected override bool DoChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue) => throw Unwired("ChangedCustomMoney");
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount, TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList, TRoleRankList TaoistRankList, TRoleRankList MasterRankList) => throw Unwired("GetRankData");
    protected override bool DoBuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName) => throw Unwired("BuyPlayer");
}

/// <summary>
/// <c>g_RoleDB.HeroDB</c>（THeroDB）的 SelectClient 专用适配器。
/// SelectClient.pas 只用它的 <c>GetID</c>（:964 的重名判定）。
/// </summary>
public sealed class SelectClientHeroDb : THeroDBBase
{
    private readonly RoleDatabase _db;

    public SelectClientHeroDb(RoleDatabase db) => _db = db ?? throw new ArgumentNullException(nameof(db));

    protected override void OwnerLock() => Monitor.Enter(_db.SyncRoot);

    protected override void OwnerUnLock() => Monitor.Exit(_db.SyncRoot);

    protected override int DoGetID(string HeroName) => _db.GetHeroId(HeroName);

    private static NotSupportedException Unwired(string member) => new NotSupportedException(
        "接缝：SelectClientHeroDb 只接线了 SelectClient.pas 用到的 1 个 Do*（GetID），" + member + " 未接线。" +
        "通用 THeroDB 请用 TMySqlRoleDB.HeroDB；或移植 RoleDB.pas/MySqlRoleDB.pas 的对应实现后在本适配器补上。");

    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw Unwired("SearchByAccount");
    protected override int DoSearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw Unwired("SearchByName");
    protected override bool DoGet(string HeroName, ref THeroData HeroData, out int HeroID) => throw Unwired("Get");
    protected override bool DoAdd(string Account, string HumanName, int HumanID, string HeroName, byte Sex, byte Job, byte Hair, bool IsDeputyHero) => throw Unwired("Add");
    protected override bool DoErase(string HeroName) => throw Unwired("Erase");
    protected override bool DoSave(int HeroID, ref THeroData HeroData) => throw Unwired("Save");
    protected override bool DoRename(int HeroID, string HeroName, string NewName) => throw Unwired("Rename");
    protected override bool DoAssess(int HeroID, string HeroName, string DeputyHeroName) => throw Unwired("Assess");
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount, TRoleRankList HeroRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList, TRoleRankList TaoistRankList) => throw Unwired("GetRankData");
}
