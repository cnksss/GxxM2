using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.DBServer;

// ============================================================================================
// Source\DBServer\RoleDB.pas:117-261 的 THumanDB / THeroDB 公开面（C# 侧骨架）。
//
// 原文每个公开方法都是同一个模板：
//     Result := <初值>;
//     FOwner.Lock;
//     try
//       try
//         Result := DoXxx(...);
//       except
//         on E: Exception do MainOutMessage(E.Message);      ← 吞掉；Result 保持初值或已写值
//       end;
//     finally
//       FOwner.UnLock;
//     end;
// 这里逐字照搬（含"异常时返回初值"这一语义）。因为要传 ref 参数，不能用 lambda 包 Do*，
// 所以锁定/异常骨架直接内联在每个包装方法里（结构性重复 = 原文的真实形状）。
// ============================================================================================

/// <summary>RoleDB.pas:117-209 `THumanDB`。</summary>
public abstract class THumanDBBase
{
    // ---- 原文 protected abstract 的 Do* 面 ----
    protected abstract int DoGetID(string HumanName);
    protected abstract bool DoCheckHumanExists(string Account, string HumanName);
    protected abstract int DoGetHumanCount(string Account);
    protected abstract string DoGetOtherHumanName(string Account, string HumanName);
    protected abstract bool DoGetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName);
    protected abstract bool DoGetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin);
    protected abstract int DoQueryHumans(string Account, TQueryHumanList HumanList);
    protected abstract int DoQueryDeleteHumans(string Account, TQueryHumanList HumanList);
    protected abstract int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    protected abstract int DoSearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    protected abstract int DoSearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList);
    protected abstract int DoGetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList);
    protected abstract bool DoSelect(string Account, string HumanName);
    protected abstract bool DoGet(string Account, string HumanName, ref THumData HumData, out int HumanID);
    protected abstract bool DoAdd(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair);
    protected abstract bool DoDelete(string Account, string HumanName);
    protected abstract bool DoDeleteRestore(string Account, string HumanName);
    protected abstract bool DoSetEnabled(string Account, string HumanName, int Enabled);
    protected abstract bool DoErase(string Account, string HumanName);
    protected abstract bool DoRecordLoginTime(string Account, string HumanName);
    protected abstract bool DoSave(int HumanID, ref THumData HumData);
    protected abstract bool DoRename(string Account, string HumanName, int HumanID, string NewName);
    protected abstract bool DoChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue);
    protected abstract bool DoChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue);
    protected abstract void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList, TRoleRankList MasterRankList);
    protected abstract bool DoBuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName);

    /// <summary>原文 `FOwner.Lock`（子类接到 TMySqlRoleDB 的临界区）。</summary>
    protected abstract void OwnerLock();

    /// <summary>原文 `FOwner.UnLock`。</summary>
    protected abstract void OwnerUnLock();

    // ---- RoleDB.pas:533-1005 的公开包装（Lock + except 吞异常） ----

    /// <summary>RoleDB.pas:533-549 `GetID`（初值 NO_ID）。</summary>
    public int GetID(string HumanName)
    {
        int Result = RoleDbConst.NO_ID;
        OwnerLock();
        try
        {
            try { Result = DoGetID(HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:551-567 `CheckHumanExists`（初值 False）。</summary>
    public bool CheckHumanExists(string Account, string HumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoCheckHumanExists(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:569-585 `GetHumanCount`（初值 0）。</summary>
    public int GetHumanCount(string Account)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoGetHumanCount(Account); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:587-603 `GetOtherHumanName`（初值 ''）。</summary>
    public string GetOtherHumanName(string Account, string HumanName)
    {
        string Result = "";
        OwnerLock();
        try
        {
            try { Result = DoGetOtherHumanName(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:605-621 `GetHumanHeroName`（初值 False；两个 out 参数在异常时保持初值 ''）。</summary>
    public bool GetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName)
    {
        bool Result = false;
        HeroName = "";
        DeputyHeroName = "";
        OwnerLock();
        try
        {
            try { Result = DoGetHumanHeroName(Account, HumanName, out HeroName, out DeputyHeroName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:623-639 `GetBaseInfo`（异常时 4 个 out 参数保持初值 0）。</summary>
    public bool GetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin)
    {
        bool Result = false;
        Sex = 0; Job = 0; Level = 0; LastLogin = 0;
        OwnerLock();
        try
        {
            try { Result = DoGetBaseInfo(HumanName, out Sex, out Job, out Level, out LastLogin); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:641-657 `QueryHumans`。</summary>
    public int QueryHumans(string Account, TQueryHumanList HumanList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoQueryHumans(Account, HumanList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:659-675 `QueryDeleteHumans`。</summary>
    public int QueryDeleteHumans(string Account, TQueryHumanList HumanList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoQueryDeleteHumans(Account, HumanList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:677-694 `SearchByAccount`。</summary>
    public int SearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoSearchByAccount(Account, MatchType, RoleList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:696-712 `SearchByName`。</summary>
    public int SearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoSearchByName(HumanName, MatchType, RoleList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:714-730 `SearchByLevel`。</summary>
    public int SearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoSearchByLevel(LimitCount, MinLevel, RoleList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:733-749 `GetMobileNumbers`（TStrings → List&lt;string&gt;）。</summary>
    public int GetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoGetMobileNumbers(OnlyBindMobile, MobileNumberList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:752-768 `Select`。</summary>
    public bool Select(string Account, string HumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoSelect(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:771-792 `Get`（var THumData + var HumanID）。</summary>
    public bool Get(string Account, string HumanName, ref THumData HumData, out int HumanID)
    {
        bool Result = false;
        HumanID = 0;
        OwnerLock();
        try
        {
            try { Result = DoGet(Account, HumanName, ref HumData, out HumanID); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:795-811 `Add`。</summary>
    public bool Add(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoAdd(Account, HumanName, IsSelect, Sex, Job, Hair); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:813-829 `Delete`。</summary>
    public bool Delete(string Account, string HumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoDelete(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:831-847 `DeleteRestore`。</summary>
    public bool DeleteRestore(string Account, string HumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoDeleteRestore(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:849-865 `SetEnabled`。</summary>
    public bool SetEnabled(string Account, string HumanName, int Enabled)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoSetEnabled(Account, HumanName, Enabled); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:867-883 `Erase`。</summary>
    public bool Erase(string Account, string HumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoErase(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:885-901 `RecordLoginTime`。</summary>
    public bool RecordLoginTime(string Account, string HumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoRecordLoginTime(Account, HumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:903-924 `Save`。</summary>
    public bool Save(int HumanID, ref THumData HumData)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoSave(HumanID, ref HumData); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:926-942 `Rename`。</summary>
    public bool Rename(string Account, string HumanName, int HumanID, string NewName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoRename(Account, HumanName, HumanID, NewName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:944-963 `ChangedGold`。</summary>
    public bool ChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue)
    {
        bool Result = false;
        ResultValue = 0;
        OwnerLock();
        try
        {
            try { Result = DoChangedGold(HumanName, ChangeType, ChangedValue, out ResultValue); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:965-981 `ChangedCustomMoney`。</summary>
    public bool ChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue)
    {
        bool Result = false;
        ResultValue = 0;
        OwnerLock();
        try
        {
            try { Result = DoChangedCustomMoney(HumanID, CustomMoneyName, ChangedValue, out ResultValue); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:983-999 `GetRankData`（原文 procedure，异常被吞）。</summary>
    public void GetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList, TRoleRankList MasterRankList)
    {
        OwnerLock();
        try
        {
            try
            {
                DoGetRankData(MinLevel, MaxLevel, TopCount,
                    HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList);
            }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
    }

    /// <summary>RoleDB.pas:1003-1005 `BuyPlayer`。</summary>
    public bool BuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoBuyPlayer(sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }
}

/// <summary>RoleDB.pas:211-261 `THeroDB`。</summary>
public abstract class THeroDBBase
{
    protected abstract int DoGetID(string HeroName);
    protected abstract int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    protected abstract int DoSearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    protected abstract bool DoGet(string HeroName, ref THeroData HeroData, out int HeroID);
    protected abstract bool DoAdd(string Account, string HumanName, int HumanID, string HeroName, byte Sex, byte Job, byte Hair, bool IsDeputyHero);
    protected abstract bool DoErase(string HeroName);
    protected abstract bool DoSave(int HeroID, ref THeroData HeroData);
    protected abstract bool DoRename(int HeroID, string HeroName, string NewName);
    protected abstract bool DoAssess(int HeroID, string HeroName, string DeputyHeroName);
    protected abstract void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HeroRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList);

    protected abstract void OwnerLock();
    protected abstract void OwnerUnLock();

    /// <summary>RoleDB.pas:1030-1047 `GetID`（初值 NO_ID）。</summary>
    public int GetID(string HeroName)
    {
        int Result = RoleDbConst.NO_ID;
        OwnerLock();
        try
        {
            try { Result = DoGetID(HeroName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1049-1066 `SearchByAccount`。</summary>
    public int SearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoSearchByAccount(Account, MatchType, RoleList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1068-1085 `SearchByName`。</summary>
    public int SearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        OwnerLock();
        try
        {
            try { Result = DoSearchByName(HeroName, MatchType, RoleList); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1087-1108 `Get`。</summary>
    public bool Get(string HeroName, ref THeroData HeroData, out int HeroID)
    {
        bool Result = false;
        HeroID = 0;
        OwnerLock();
        try
        {
            try { Result = DoGet(HeroName, ref HeroData, out HeroID); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1110-1131 `Add`。</summary>
    public bool Add(string Account, string HumanName, int HumanID, string HeroName, byte Sex, byte Job, byte Hair, bool IsDeputyHero)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoAdd(Account, HumanName, HumanID, HeroName, Sex, Job, Hair, IsDeputyHero); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1145-1161 `Erase`。</summary>
    public bool Erase(string HeroName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoErase(HeroName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1181-1202 `Save`。</summary>
    public bool Save(int HeroID, ref THeroData HeroData)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoSave(HeroID, ref HeroData); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1204-1225 `Rename`。</summary>
    public bool Rename(int HeroID, string HeroName, string NewName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoRename(HeroID, HeroName, NewName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1227-1248 `Assess`。</summary>
    public bool Assess(int HeroID, string HeroName, string DeputyHeroName)
    {
        bool Result = false;
        OwnerLock();
        try
        {
            try { Result = DoAssess(HeroID, HeroName, DeputyHeroName); }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
        return Result;
    }

    /// <summary>RoleDB.pas:1250-… `GetRankData`（原文 procedure）。</summary>
    public void GetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HeroRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList)
    {
        OwnerLock();
        try
        {
            try
            {
                DoGetRankData(MinLevel, MaxLevel, TopCount,
                    HeroRankList, WarriorRankList, WizardRankList, TaoistRankList);
            }
            catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }
        }
        finally { OwnerUnLock(); }
    }
}
