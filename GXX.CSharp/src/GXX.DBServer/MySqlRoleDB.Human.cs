using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// MySqlRoleDB.pas:10-137 `TMySqlHumanDB`（1:1）。
/// 原文直接调 libmysql 的 <c>FStatement*.Reset/OrderBindParam*/Query/Fetch/Step</c>，
/// 这里逐行对应到 <see cref="IRoleMySqlStatement"/>；SQL 文本取 <see cref="MySqlRoleDBStatements.Human"/>。
/// </summary>
public sealed partial class TMySqlHumanDB : THumanDBBase
{
    /// <summary>内部接缝用：原文的 `TMySqlRoleDB(Owner).FDB`。</summary>
    private readonly IRoleMySqlDatabase FDB;

    /// <summary>原文的 Owner（TMySqlRoleDB），仅用于事务/Exec 转发。</summary>
    private readonly TMySqlRoleDB FOwner;

    protected override void OwnerLock() => FOwner.Lock();

    protected override void OwnerUnLock() => FOwner.UnLock();

    // ---- 86 个 TMySqlStatement 字段（MySqlRoleDB.pas:12-97），名字逐字保留 ----
    private IRoleMySqlStatement? FStatementGetID;
    private IRoleMySqlStatement? FStatementCheckHumanExists;
    private IRoleMySqlStatement? FStatementGetHumanCount;
    private IRoleMySqlStatement? FStatementGetOtherHumanName;
    private IRoleMySqlStatement? FStatementGetHumanHeroName;
    private IRoleMySqlStatement? FStatementGetHumanHeroID;
    private IRoleMySqlStatement? FStatementGetHumanGoldInfo;
    private IRoleMySqlStatement? FStatementUpdateHumanGoldInfo;
    private IRoleMySqlStatement? FStatementGetHumanBaseInfo;
    private IRoleMySqlStatement? FStatementQueryHumans;
    private IRoleMySqlStatement? FStatementSearchByAccountMatchComplete;
    private IRoleMySqlStatement? FStatementSearchByAccountMatchFuzzy;
    private IRoleMySqlStatement? FStatementSearchByNameMatchComplete;
    private IRoleMySqlStatement? FStatementSearchByNameMatchFuzzy;
    private IRoleMySqlStatement? FStatementSearchByLevel;
    private IRoleMySqlStatement? FStatementGetMobileNumbers1;
    private IRoleMySqlStatement? FStatementGetMobileNumbers2;
    private IRoleMySqlStatement? FStatementSelect;
    private IRoleMySqlStatement? FStatementUnSelect;
    private IRoleMySqlStatement? FStatementGetHuman;
    private IRoleMySqlStatement? FStatementGetAbil;
    private IRoleMySqlStatement? FStatementGetAbilNG;
    private IRoleMySqlStatement? FStatementGetAbilWine;
    private IRoleMySqlStatement? FStatementGetAbilNpcAdd;
    private IRoleMySqlStatement? FStatementGetGamePetData;
    private IRoleMySqlStatement? FStatementGetGodBlessState;
    private IRoleMySqlStatement? FStatementGetMagic;
    private IRoleMySqlStatement? FStatementGetMagicUseTick;
    private IRoleMySqlStatement? FStatementGetStatusTime;
    private IRoleMySqlStatement? FStatementGetQuestFlag;
    private IRoleMySqlStatement? FStatementGetVariableU;
    private IRoleMySqlStatement? FStatementGetVariableT;
    private IRoleMySqlStatement? FStatementGetVariableJ;
    private IRoleMySqlStatement? FStatementGetVariableZ;
    private IRoleMySqlStatement? FStatementGetItems;
    private IRoleMySqlStatement? FStatementGetItemValueAdd;
    private IRoleMySqlStatement? FStatementGetItemElementAdd;
    private IRoleMySqlStatement? FStatementGetItemAddDataByte;
    private IRoleMySqlStatement? FStatementGetItemAddDataInt;
    private IRoleMySqlStatement? FStatementGetItemAddDataText;
    private IRoleMySqlStatement? FStatementGetItemFlute;
    private IRoleMySqlStatement? FStatementGetItemProgress;
    private IRoleMySqlStatement? FStatementGetItemProperty;
    private IRoleMySqlStatement? FStatementGetSkillPower;
    private IRoleMySqlStatement? FStatementAddHuman;
    private IRoleMySqlStatement? FStatementDeleteOrRestore;
    private IRoleMySqlStatement? FStatementRecordLoginTime;
    private IRoleMySqlStatement? FStatementUpdateHumanName;
    private IRoleMySqlStatement? FStatementUpdateHumanNameDearName;
    private IRoleMySqlStatement? FStatementUpdateHumanNameMasterName;
    private IRoleMySqlStatement? FStatementUpdateHuman;
    private IRoleMySqlStatement? FStatementInsertAbil;
    private IRoleMySqlStatement? FStatementInsertAbilNG;
    private IRoleMySqlStatement? FStatementInsertAbilWine;
    private IRoleMySqlStatement? FStatementInsertAbilNpcAdd;
    private IRoleMySqlStatement? FStatementInsertGamePetData;
    private IRoleMySqlStatement? FStatementInsertGodBlessState;
    private IRoleMySqlStatement? FStatementInsertMagic;
    private IRoleMySqlStatement? FStatementInsertMagicUseTick;
    private IRoleMySqlStatement? FStatementInsertStatusTime;
    private IRoleMySqlStatement? FStatementInsertQuestFlag;
    private IRoleMySqlStatement? FStatementInsertVariableU;
    private IRoleMySqlStatement? FStatementInsertVariableT;
    private IRoleMySqlStatement? FStatementInsertVariableJ;
    private IRoleMySqlStatement? FStatementInsertVariableZ;
    private IRoleMySqlStatement? FStatementInsertCustomMoney;
    private IRoleMySqlStatement? FStatementInsertItems;
    private IRoleMySqlStatement? FStatementInsertItemValueAdd;
    private IRoleMySqlStatement? FStatementInsertItemElementAdd;
    private IRoleMySqlStatement? FStatementInsertItemAddDataByte;
    private IRoleMySqlStatement? FStatementInsertItemAddDataInt;
    private IRoleMySqlStatement? FStatementInsertItemAddDataText;
    private IRoleMySqlStatement? FStatementInsertItemFlute;
    private IRoleMySqlStatement? FStatementInsertItemProgress;
    private IRoleMySqlStatement? FStatementInsertItemProperty;
    private IRoleMySqlStatement? FStatementInsertSkillPower;
    private IRoleMySqlStatement? FStatementGetLevelRankCheckLevel;
    private IRoleMySqlStatement? FStatementGetLevelRankTopCount;
    private IRoleMySqlStatement? FStatementGetLevelRankCheckLevelAndCount;
    private IRoleMySqlStatement? FStatementGetMasterRankCheckLevel;
    private IRoleMySqlStatement? FStatementGetMasterRankTopCount;
    private IRoleMySqlStatement? FStatementGetMasterRankCheckLevelAndCount;
    private IRoleMySqlStatement? FStatementBuyPlayer;
    private IRoleMySqlStatement? FStatementGetCustomMoney;
    private IRoleMySqlStatement? FStatementGetCustomMoneyByName;
    private IRoleMySqlStatement? FStatementUpdateCustomMoney;

    internal TMySqlHumanDB(TMySqlRoleDB Owner)
    {
        FOwner = Owner;
        FDB = Owner.DB;
    }

    // ==========================================================================================
    // 事务 / 直执（MySqlRoleDB.pas:3373-3391）
    // ==========================================================================================

    private void BeginTransaction() => FDB.StartTransaction();
    private void Commit() => FDB.Commit();
    private void RollBack() => FDB.RollBack();
    private void Execute(string Sql) => FDB.Exec(Sql);

    /// <summary>MySqlRoleDB.pas:3388 `MainOutMessage(E.Message)` 的 except 分支。</summary>
    private static void ReportException(Exception E) => RoleDbSeam.MainOutMessage(E.Message);

    // ==========================================================================================
    // DoInit（MySqlRoleDB.pas:256-644）
    //   原文：Assert(Owner is TMySqlRoleDB)；Stms.AddSQLStatement('<name>')；.Sql := '...'；.Prepare
    //   SQL 文本来自脚本抽取的常量（MySqlRoleDB.SqlStatements.cs），非手工转录。
    // ==========================================================================================

    /// <summary>
    /// MySqlRoleDB.pas:256-644 `TMySqlHumanDB.DoInit`。
    /// 用 <see cref="MySqlRoleDBStatements.Human.Names"/> 保证「常量名 → AddSQLStatement 的语句名」与原文一致。
    /// </summary>
    public void DoInit()
    {
        // 原文两条 Assert 在托管侧退化为显式校验（保留文案）
        if (FOwner is not TMySqlRoleDB) throw new InvalidOperationException("TSqliteHumanDB owner type error.");
        if (FOwner.DB is null) throw new InvalidOperationException("TMySqlRoleDB.DB not create");

        FStatementGetID = Add("FStatementGetID", MySqlRoleDBStatements.Human.FStatementGetID);
        FStatementCheckHumanExists = Add("FStatementCheckHumanExists", MySqlRoleDBStatements.Human.FStatementCheckHumanExists);
        FStatementGetHumanCount = Add("FStatementGetHumanCount", MySqlRoleDBStatements.Human.FStatementGetHumanCount);
        FStatementGetOtherHumanName = Add("FStatementGetOtherHumanName", MySqlRoleDBStatements.Human.FStatementGetOtherHumanName);
        FStatementGetHumanHeroName = Add("FStatementGetHumanHeroName", MySqlRoleDBStatements.Human.FStatementGetHumanHeroName);
        FStatementGetHumanHeroID = Add("FStatementGetHumanHeroID", MySqlRoleDBStatements.Human.FStatementGetHumanHeroID);
        FStatementGetHumanGoldInfo = Add("FStatementGetHumanGoldInfo", MySqlRoleDBStatements.Human.FStatementGetHumanGoldInfo);
        FStatementUpdateHumanGoldInfo = Add("FStatementUpdateHumanGoldInfo", MySqlRoleDBStatements.Human.FStatementUpdateHumanGoldInfo);
        FStatementGetHumanBaseInfo = Add("FStatementGetHumanBaseInfo", MySqlRoleDBStatements.Human.FStatementGetHumanBaseInfo);
        FStatementQueryHumans = Add("FStatementQueryHumans", MySqlRoleDBStatements.Human.FStatementQueryHumans);
        FStatementSearchByAccountMatchComplete = Add("FStatementSearchByAccountMatchComplete", MySqlRoleDBStatements.Human.FStatementSearchByAccountMatchComplete);
        FStatementSearchByAccountMatchFuzzy = Add("FStatementSearchByAccountMatchFuzzy", MySqlRoleDBStatements.Human.FStatementSearchByAccountMatchFuzzy);
        FStatementSearchByNameMatchComplete = Add("FStatementSearchByNameMatchComplete", MySqlRoleDBStatements.Human.FStatementSearchByNameMatchComplete);
        FStatementSearchByNameMatchFuzzy = Add("FStatementSearchByNameMatchFuzzy", MySqlRoleDBStatements.Human.FStatementSearchByNameMatchFuzzy);
        FStatementSearchByLevel = Add("FStatementSearchByLevel", MySqlRoleDBStatements.Human.FStatementSearchByLevel);
        FStatementGetMobileNumbers1 = Add("FStatementGetMobileNumbers1", MySqlRoleDBStatements.Human.FStatementGetMobileNumbers1);
        FStatementGetMobileNumbers2 = Add("FStatementGetMobileNumbers2", MySqlRoleDBStatements.Human.FStatementGetMobileNumbers2);
        FStatementSelect = Add("FStatementSelect", MySqlRoleDBStatements.Human.FStatementSelect);
        FStatementUnSelect = Add("FStatementUnSelect", MySqlRoleDBStatements.Human.FStatementUnSelect);
        FStatementGetHuman = Add("FStatementGetHuman", MySqlRoleDBStatements.Human.FStatementGetHuman);
        FStatementGetAbil = Add("FStatementGetAbil", MySqlRoleDBStatements.Human.FStatementGetAbil);
        FStatementGetAbilNG = Add("FStatementGetAbilNG", MySqlRoleDBStatements.Human.FStatementGetAbilNG);
        FStatementGetAbilWine = Add("FStatementGetAbilWine", MySqlRoleDBStatements.Human.FStatementGetAbilWine);
        FStatementGetAbilNpcAdd = Add("FStatementGetAbilNpcAdd", MySqlRoleDBStatements.Human.FStatementGetAbilNpcAdd);
        FStatementGetGamePetData = Add("FStatementGetGamePetData", MySqlRoleDBStatements.Human.FStatementGetGamePetData);
        FStatementGetGodBlessState = Add("FStatementGetGodBlessState", MySqlRoleDBStatements.Human.FStatementGetGodBlessState);
        FStatementGetMagic = Add("FStatementGetMagic", MySqlRoleDBStatements.Human.FStatementGetMagic);
        FStatementGetMagicUseTick = Add("FStatementGetMagicUseTick", MySqlRoleDBStatements.Human.FStatementGetMagicUseTick);
        FStatementGetStatusTime = Add("FStatementGetStatusTime", MySqlRoleDBStatements.Human.FStatementGetStatusTime);
        FStatementGetQuestFlag = Add("FStatementGetQuestFlag", MySqlRoleDBStatements.Human.FStatementGetQuestFlag);
        FStatementGetVariableU = Add("FStatementGetVariableU", MySqlRoleDBStatements.Human.FStatementGetVariableU);
        FStatementGetVariableT = Add("FStatementGetVariableT", MySqlRoleDBStatements.Human.FStatementGetVariableT);
        FStatementGetVariableJ = Add("FStatementGetVariableJ", MySqlRoleDBStatements.Human.FStatementGetVariableJ);
        FStatementGetVariableZ = Add("FStatementGetVariableZ", MySqlRoleDBStatements.Human.FStatementGetVariableZ);
        FStatementGetItems = Add("FStatementGetItems", MySqlRoleDBStatements.Human.FStatementGetItems);
        FStatementGetItemValueAdd = Add("FStatementGetItemValueAdd", MySqlRoleDBStatements.Human.FStatementGetItemValueAdd);
        FStatementGetItemElementAdd = Add("FStatementGetItemElementAdd", MySqlRoleDBStatements.Human.FStatementGetItemElementAdd);
        FStatementGetItemAddDataByte = Add("FStatementGetItemAddDataByte", MySqlRoleDBStatements.Human.FStatementGetItemAddDataByte);
        FStatementGetItemAddDataInt = Add("FStatementGetItemAddDataInt", MySqlRoleDBStatements.Human.FStatementGetItemAddDataInt);
        FStatementGetItemAddDataText = Add("FStatementGetItemAddDataText", MySqlRoleDBStatements.Human.FStatementGetItemAddDataText);
        FStatementGetItemFlute = Add("FStatementGetItemFlute", MySqlRoleDBStatements.Human.FStatementGetItemFlute);
        FStatementGetItemProgress = Add("FStatementGetItemProgress", MySqlRoleDBStatements.Human.FStatementGetItemProgress);
        FStatementGetItemProperty = Add("FStatementGetItemProperty", MySqlRoleDBStatements.Human.FStatementGetItemProperty);
        FStatementGetSkillPower = Add("FStatementGetSkillPower", MySqlRoleDBStatements.Human.FStatementGetSkillPower);
        FStatementAddHuman = Add("FStatementAddHuman", MySqlRoleDBStatements.Human.FStatementAddHuman);
        FStatementDeleteOrRestore = Add("FStatementDeleteOrRestore", MySqlRoleDBStatements.Human.FStatementDeleteOrRestore);
        FStatementRecordLoginTime = Add("FStatementRecordLoginTime", MySqlRoleDBStatements.Human.FStatementRecordLoginTime);
        FStatementUpdateHumanName = Add("FStatementUpdateHumanName", MySqlRoleDBStatements.Human.FStatementUpdateHumanName);
        FStatementUpdateHumanNameDearName = Add("FStatementUpdateHumanNameDearName", MySqlRoleDBStatements.Human.FStatementUpdateHumanNameDearName);
        FStatementUpdateHumanNameMasterName = Add("FStatementUpdateHumanNameMasterName", MySqlRoleDBStatements.Human.FStatementUpdateHumanNameMasterName);
        FStatementUpdateHuman = Add("FStatementUpdateHuman", MySqlRoleDBStatements.Human.FStatementUpdateHuman);
        FStatementInsertAbil = Add("FStatementInsertAbil", MySqlRoleDBStatements.Human.FStatementInsertAbil);
        FStatementInsertAbilNG = Add("FStatementInsertAbilNG", MySqlRoleDBStatements.Human.FStatementInsertAbilNG);
        FStatementInsertAbilWine = Add("FStatementInsertAbilWine", MySqlRoleDBStatements.Human.FStatementInsertAbilWine);
        FStatementInsertAbilNpcAdd = Add("FStatementInsertAbilNpcAdd", MySqlRoleDBStatements.Human.FStatementInsertAbilNpcAdd);
        FStatementInsertGamePetData = Add("FStatementInsertGamePetData", MySqlRoleDBStatements.Human.FStatementInsertGamePetData);
        FStatementInsertGodBlessState = Add("FStatementInsertGodBlessState", MySqlRoleDBStatements.Human.FStatementInsertGodBlessState);
        FStatementInsertMagic = Add("FStatementInsertMagic", MySqlRoleDBStatements.Human.FStatementInsertMagic);
        FStatementInsertMagicUseTick = Add("FStatementInsertMagicUseTick", MySqlRoleDBStatements.Human.FStatementInsertMagicUseTick);
        FStatementInsertStatusTime = Add("FStatementInsertStatusTime", MySqlRoleDBStatements.Human.FStatementInsertStatusTime);
        FStatementInsertQuestFlag = Add("FStatementInsertQuestFlag", MySqlRoleDBStatements.Human.FStatementInsertQuestFlag);
        FStatementInsertVariableU = Add("FStatementInsertVariableU", MySqlRoleDBStatements.Human.FStatementInsertVariableU);
        FStatementInsertVariableT = Add("FStatementInsertVariableT", MySqlRoleDBStatements.Human.FStatementInsertVariableT);
        FStatementInsertVariableJ = Add("FStatementInsertVariableJ", MySqlRoleDBStatements.Human.FStatementInsertVariableJ);
        FStatementInsertVariableZ = Add("FStatementInsertVariableZ", MySqlRoleDBStatements.Human.FStatementInsertVariableZ);
        FStatementInsertCustomMoney = Add("FStatementInsertCustomMoney", MySqlRoleDBStatements.Human.FStatementInsertCustomMoney);
        FStatementInsertItems = Add("FStatementInsertItems", MySqlRoleDBStatements.Human.FStatementInsertItems);
        FStatementInsertItemValueAdd = Add("FStatementInsertItemValueAdd", MySqlRoleDBStatements.Human.FStatementInsertItemValueAdd);
        FStatementInsertItemElementAdd = Add("FStatementInsertItemElementAdd", MySqlRoleDBStatements.Human.FStatementInsertItemElementAdd);
        FStatementInsertItemAddDataByte = Add("FStatementInsertItemAddDataByte", MySqlRoleDBStatements.Human.FStatementInsertItemAddDataByte);
        FStatementInsertItemAddDataInt = Add("FStatementInsertItemAddDataInt", MySqlRoleDBStatements.Human.FStatementInsertItemAddDataInt);
        FStatementInsertItemAddDataText = Add("FStatementInsertItemAddDataText", MySqlRoleDBStatements.Human.FStatementInsertItemAddDataText);
        FStatementInsertItemFlute = Add("FStatementInsertItemFlute", MySqlRoleDBStatements.Human.FStatementInsertItemFlute);
        FStatementInsertItemProgress = Add("FStatementInsertItemProgress", MySqlRoleDBStatements.Human.FStatementInsertItemProgress);
        FStatementInsertItemProperty = Add("FStatementInsertItemProperty", MySqlRoleDBStatements.Human.FStatementInsertItemProperty);
        FStatementInsertSkillPower = Add("FStatementInsertSkillPower", MySqlRoleDBStatements.Human.FStatementInsertSkillPower);
        FStatementGetLevelRankCheckLevel = Add("FStatementGetLevelRankCheckLevel", MySqlRoleDBStatements.Human.FStatementGetLevelRankCheckLevel);
        FStatementGetLevelRankTopCount = Add("FStatementGetLevelRankTopCount", MySqlRoleDBStatements.Human.FStatementGetLevelRankTopCount);
        FStatementGetLevelRankCheckLevelAndCount = Add("FStatementGetLevelRankCheckLevelAndCount", MySqlRoleDBStatements.Human.FStatementGetLevelRankCheckLevelAndCount);
        FStatementGetMasterRankCheckLevel = Add("FStatementGetMasterRankCheckLevel", MySqlRoleDBStatements.Human.FStatementGetMasterRankCheckLevel);
        FStatementGetMasterRankTopCount = Add("FStatementGetMasterRankTopCount", MySqlRoleDBStatements.Human.FStatementGetMasterRankTopCount);
        FStatementGetMasterRankCheckLevelAndCount = Add("FStatementGetMasterRankCheckLevelAndCount", MySqlRoleDBStatements.Human.FStatementGetMasterRankCheckLevelAndCount);
        FStatementBuyPlayer = Add("FStatementBuyPlayer", MySqlRoleDBStatements.Human.FStatementBuyPlayer);
        FStatementGetCustomMoney = Add("FStatementGetCustomMoney", MySqlRoleDBStatements.Human.FStatementGetCustomMoney);
        FStatementGetCustomMoneyByName = Add("FStatementGetCustomMoneyByName", MySqlRoleDBStatements.Human.FStatementGetCustomMoneyByName);
        FStatementUpdateCustomMoney = Add("FStatementUpdateCustomMoney", MySqlRoleDBStatements.Human.FStatementUpdateCustomMoney);
    }

    /// <summary>
    /// 原文 86 次 `Stms.AddSQLStatement('名称')` + `.Sql := '...'` + 646-837 的 86 次 `.Prepare`。
    /// 语句名从脚本抽取的 <see cref="MySqlRoleDBStatements.Human.Names"/> 取（与原文一致）。
    /// </summary>
    private IRoleMySqlStatement Add(string fieldKey, string sql)
    {
        IRoleMySqlStatement S = FDB.AddSQLStatement(MySqlRoleDBStatements.Human.Names[fieldKey]);
        S.Sql = sql;
        S.Prepare();
        return S;
    }

    /// <summary>MySqlRoleDB.pas:646-837 `TMySqlHumanDB.DoFinal`（86 个 `if FStatementX &lt;&gt; nil then FStatementX.Finalize`）。</summary>
    public void DoFinal()
    {
        Finalize(FStatementGetID);
        Finalize(FStatementCheckHumanExists);
        Finalize(FStatementGetHumanCount);
        Finalize(FStatementGetOtherHumanName);
        Finalize(FStatementGetHumanHeroName);
        Finalize(FStatementGetHumanHeroID);
        Finalize(FStatementGetHumanGoldInfo);
        Finalize(FStatementUpdateHumanGoldInfo);
        Finalize(FStatementGetHumanBaseInfo);
        Finalize(FStatementQueryHumans);
        Finalize(FStatementSearchByAccountMatchComplete);
        Finalize(FStatementSearchByAccountMatchFuzzy);
        Finalize(FStatementSearchByNameMatchComplete);
        Finalize(FStatementSearchByNameMatchFuzzy);
        Finalize(FStatementSearchByLevel);
        Finalize(FStatementGetMobileNumbers1);
        Finalize(FStatementGetMobileNumbers2);
        Finalize(FStatementSelect);
        Finalize(FStatementUnSelect);
        Finalize(FStatementGetHuman);
        Finalize(FStatementGetAbil);
        Finalize(FStatementGetAbilNG);
        Finalize(FStatementGetAbilWine);
        Finalize(FStatementGetAbilNpcAdd);
        Finalize(FStatementGetGamePetData);
        Finalize(FStatementGetGodBlessState);
        Finalize(FStatementGetMagic);
        Finalize(FStatementGetMagicUseTick);
        Finalize(FStatementGetStatusTime);
        Finalize(FStatementGetQuestFlag);
        Finalize(FStatementGetVariableU);
        Finalize(FStatementGetVariableT);
        Finalize(FStatementGetVariableJ);
        Finalize(FStatementGetVariableZ);
        Finalize(FStatementGetItems);
        Finalize(FStatementGetItemValueAdd);
        Finalize(FStatementGetItemElementAdd);
        Finalize(FStatementGetItemAddDataByte);
        Finalize(FStatementGetItemAddDataInt);
        Finalize(FStatementGetItemAddDataText);
        Finalize(FStatementGetItemFlute);
        Finalize(FStatementGetItemProgress);
        Finalize(FStatementGetItemProperty);
        Finalize(FStatementGetSkillPower);
        Finalize(FStatementAddHuman);
        Finalize(FStatementDeleteOrRestore);
        Finalize(FStatementRecordLoginTime);
        Finalize(FStatementUpdateHumanName);
        Finalize(FStatementUpdateHumanNameDearName);
        Finalize(FStatementUpdateHumanNameMasterName);
        Finalize(FStatementUpdateHuman);
        Finalize(FStatementInsertAbil);
        Finalize(FStatementInsertAbilNG);
        Finalize(FStatementInsertAbilWine);
        Finalize(FStatementInsertAbilNpcAdd);
        Finalize(FStatementInsertGamePetData);
        Finalize(FStatementInsertGodBlessState);
        Finalize(FStatementInsertMagic);
        Finalize(FStatementInsertMagicUseTick);
        Finalize(FStatementInsertStatusTime);
        Finalize(FStatementInsertQuestFlag);
        Finalize(FStatementInsertVariableU);
        Finalize(FStatementInsertVariableT);
        Finalize(FStatementInsertVariableJ);
        Finalize(FStatementInsertVariableZ);
        Finalize(FStatementInsertCustomMoney);
        Finalize(FStatementInsertItems);
        Finalize(FStatementInsertItemValueAdd);
        Finalize(FStatementInsertItemElementAdd);
        Finalize(FStatementInsertItemAddDataByte);
        Finalize(FStatementInsertItemAddDataInt);
        Finalize(FStatementInsertItemAddDataText);
        Finalize(FStatementInsertItemFlute);
        Finalize(FStatementInsertItemProgress);
        Finalize(FStatementInsertItemProperty);
        Finalize(FStatementInsertSkillPower);
        Finalize(FStatementGetLevelRankCheckLevel);
        Finalize(FStatementGetLevelRankTopCount);
        Finalize(FStatementGetLevelRankCheckLevelAndCount);
        Finalize(FStatementGetMasterRankCheckLevel);
        Finalize(FStatementGetMasterRankTopCount);
        Finalize(FStatementGetMasterRankCheckLevelAndCount);
        Finalize(FStatementBuyPlayer);
        Finalize(FStatementGetCustomMoney);
        Finalize(FStatementGetCustomMoneyByName);
        Finalize(FStatementUpdateCustomMoney);
    }

    private static void Finalize(IRoleMySqlStatement S) => S?.FinalizeStatement();

    // ==========================================================================================
    // 简单读取（MySqlRoleDB.pas:839-1180）
    // ==========================================================================================

    /// <summary>MySqlRoleDB.pas:839-851 `DoGetID`。查不到 → NO_ID(-1)。</summary>
    protected override int DoGetID(string HumanName)
    {
        try
        {
            FStatementGetID.Reset();
            FStatementGetID.OrderBindParamText(HumanName);
            if (FStatementGetID.Query() && FStatementGetID.Fetch())
                return FStatementGetID.GetColumnValueInt(0);
            return RoleDbConst.NO_ID;
        }
        finally
        {
            FStatementGetID.Reset();
        }
    }

    /// <summary>MySqlRoleDB.pas:853-867 `DoCheckHumanExists`（原文注释保留 Sqlite 的 `Step = SQLITE_ROW` 写法）。</summary>
    protected override bool DoCheckHumanExists(string Account, string HumanName)
    {
        try
        {
            FStatementCheckHumanExists.Reset();
            FStatementCheckHumanExists.OrderBindParamText(Account);
            FStatementCheckHumanExists.OrderBindParamText(HumanName);
            bool Result;
            if (FStatementCheckHumanExists.Query() && FStatementCheckHumanExists.Fetch())
                Result = true;
            else
                Result = false;
            // Result := FStatementCheckHumanExists.Step = SQLITE_ROW;
            return Result;
        }
        finally
        {
            FStatementCheckHumanExists.Reset();
        }
    }

    /// <summary>MySqlRoleDB.pas:869-881 `DoGetHumanCount`。查不到 → 0。</summary>
    protected override int DoGetHumanCount(string Account)
    {
        try
        {
            FStatementGetHumanCount.Reset();
            FStatementGetHumanCount.OrderBindParamText(Account);
            if (FStatementGetHumanCount.Query() && FStatementGetHumanCount.Fetch())
                return FStatementGetHumanCount.GetColumnValueInt(0);
            return 0;
        }
        finally
        {
            FStatementGetHumanCount.Reset();
        }
    }

    /// <summary>MySqlRoleDB.pas:883-896 `DoGetOtherHumanName`。查不到 → ''。</summary>
    protected override string DoGetOtherHumanName(string Account, string HumanName)
    {
        try
        {
            FStatementGetOtherHumanName.Reset();
            FStatementGetOtherHumanName.OrderBindParamText(Account);
            FStatementGetOtherHumanName.OrderBindParamText(HumanName);
            if (FStatementGetOtherHumanName.Query() && FStatementGetOtherHumanName.Fetch())
                return FStatementGetOtherHumanName.GetColumnValueText(0);
            return "";
        }
        finally
        {
            FStatementGetOtherHumanName.Reset();
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:898-913 `DoGetHumanHeroName`。
    /// 注意：Account 形参在原文里**完全没用到**（只绑 HumanName）。差异断言在测试里锁定。
    /// </summary>
    protected override bool DoGetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName)
    {
        bool Result = false;
        HeroName = "";
        DeputyHeroName = "";
        try
        {
            FStatementGetHumanHeroName.Reset();
            FStatementGetHumanHeroName.OrderBindParamText(HumanName);
            if (FStatementGetHumanHeroName.Query() && FStatementGetHumanHeroName.Fetch())
            {
                Result = true;
                HeroName = FStatementGetHumanHeroName.GetColumnValueText(0);
                DeputyHeroName = FStatementGetHumanHeroName.GetColumnValueText(1);
            }
        }
        finally
        {
            FStatementGetHumanHeroName.Reset();
        }
        return Result;
    }

    /// <summary>MySqlRoleDB.pas:915-932 `DoGetBaseInfo`（Sex/Job/Level/LoginDate 共 4 列，索引 0..3）。</summary>
    protected override bool DoGetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin)
    {
        bool Result = false;
        Sex = 0; Job = 0; Level = 0; LastLogin = 0;
        try
        {
            FStatementGetHumanBaseInfo.Reset();
            FStatementGetHumanBaseInfo.OrderBindParamText(HumanName);
            if (FStatementGetHumanBaseInfo.Query() && FStatementGetHumanBaseInfo.Fetch())
            {
                Result = true;
                Sex = FStatementGetHumanBaseInfo.GetColumnValueInt(0);
                Job = FStatementGetHumanBaseInfo.GetColumnValueInt(1);
                Level = FStatementGetHumanBaseInfo.GetColumnValueInt(2);
                LastLogin = FStatementGetHumanBaseInfo.GetColumnValueInt(3);
            }
        }
        finally
        {
            FStatementGetHumanBaseInfo.Reset();
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:934-968 `DoQueryHumans`（IsDelete = 0）。
    /// Sex 越界(&lt;0 或 &gt;1)→0；Job 越界(&lt;0 或 &gt;2)→0（只在这一支做钳位）。
    /// 原文每次 `HumanList.Add(@QueryData)` 传的是**同一个局部 record 的地址**（Delphi 里 TList 只存指针，
    /// QueryData 也未重新初始化）→ 列表里所有元素实际指向同一块栈内存，循环结束后的内容全部相同。
    /// 托管侧逐行等价地每次 new 一个新对象，避免这种悬垂指针式共享（见报告「原文缺陷」）。
    /// </summary>
    protected override int DoQueryHumans(string Account, TQueryHumanList HumanList)
    {
        int Result = 0;
        try
        {
            FStatementQueryHumans.Reset();
            FStatementQueryHumans.OrderBindParamText(Account);
            FStatementQueryHumans.OrderBindParamInt(0);
            if (FStatementQueryHumans.Query())
            {
                while (FStatementQueryHumans.Fetch())
                {
                    var QueryData = new TQueryHumanData();
                    QueryData.HumanName = FStatementQueryHumans.OrderGetColumnValueText;
                    QueryData.IsSelect = FStatementQueryHumans.OrderGetColumnValueBool;
                    QueryData.Sex = FStatementQueryHumans.OrderGetColumnValueInt;
                    QueryData.Job = FStatementQueryHumans.OrderGetColumnValueInt;
                    QueryData.Hair = FStatementQueryHumans.OrderGetColumnValueInt;
                    QueryData.Level = FStatementQueryHumans.OrderGetColumnValueInt;

                    if (QueryData.Sex < 0 || QueryData.Sex > 1) QueryData.Sex = 0;
                    if (QueryData.Job < 0 || QueryData.Job > 2) QueryData.Job = 0;

                    HumanList.Add(QueryData);
                    Result++;
                }
            }
        }
        finally
        {
            FStatementQueryHumans.Reset();
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:970-998 `DoQueryDeleteHumans`（IsDelete = 1）。
    /// 原文这一支**没有** Sex/Job 钳位（与 DoQueryHumans 的真实差异，测试里锁定）。
    /// </summary>
    protected override int DoQueryDeleteHumans(string Account, TQueryHumanList HumanList)
    {
        int Result = 0;
        try
        {
            FStatementQueryHumans.Reset();
            FStatementQueryHumans.OrderBindParamText(Account);
            FStatementQueryHumans.OrderBindParamInt(1);
            if (FStatementQueryHumans.Query())
            {
                while (FStatementQueryHumans.Fetch())
                {
                    var QueryData = new TQueryHumanData();
                    QueryData.HumanName = FStatementQueryHumans.OrderGetColumnValueText;
                    QueryData.IsSelect = FStatementQueryHumans.OrderGetColumnValueBool;
                    QueryData.Sex = FStatementQueryHumans.OrderGetColumnValueInt;
                    QueryData.Job = FStatementQueryHumans.OrderGetColumnValueInt;
                    QueryData.Hair = FStatementQueryHumans.OrderGetColumnValueInt;
                    QueryData.Level = FStatementQueryHumans.OrderGetColumnValueInt;
                    HumanList.Add(QueryData);
                    Result++;
                }
            }
        }
        finally
        {
            FStatementQueryHumans.Reset();
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:1000-1055 `DoSearchByAccount`。
    /// smtComplete → 精确；否则（含 smtFuzzy 与任何其它值）→ `'%' + Account + '%'`。
    /// IsHero 恒为 False。
    /// </summary>
    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        var SearchData = new TSerarchRoleData();
        if (MatchType == TSearchMatchType.smtComplete)
        {
            try
            {
                FStatementSearchByAccountMatchComplete.Reset();
                FStatementSearchByAccountMatchComplete.OrderBindParamText(Account);
                if (FStatementSearchByAccountMatchComplete.Query())
                {
                    while (FStatementSearchByAccountMatchComplete.Fetch())
                    {
                        SearchData.Account = FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
                        SearchData.RoleName = FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
                        SearchData.IsDelete = FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.Sex = FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.IsHero = false;
                        RoleList.Add(SearchData);
                        Result++;
                    }
                }
            }
            finally
            {
                FStatementSearchByAccountMatchComplete.Reset();
            }
        }
        else
        {
            try
            {
                FStatementSearchByAccountMatchFuzzy.Reset();
                FStatementSearchByAccountMatchFuzzy.OrderBindParamText("%" + Account + "%");
                if (FStatementSearchByAccountMatchFuzzy.Query())
                {
                    while (FStatementSearchByAccountMatchFuzzy.Fetch())
                    {
                        SearchData.Account = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
                        SearchData.RoleName = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
                        SearchData.IsDelete = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Sex = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.IsHero = false;
                        RoleList.Add(SearchData);
                        Result++;
                    }
                }
            }
            finally
            {
                FStatementSearchByAccountMatchFuzzy.Reset();
            }
        }
        return Result;
    }

    /// <summary>MySqlRoleDB.pas:1057-1113 `DoSearchByName`（结构同 DoSearchByAccount，绑定列名换成 HumanName）。</summary>
    protected override int DoSearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        var SearchData = new TSerarchRoleData();
        if (MatchType == TSearchMatchType.smtComplete)
        {
            try
            {
                FStatementSearchByNameMatchComplete.Reset();
                FStatementSearchByNameMatchComplete.OrderBindParamText(HumanName);
                if (FStatementSearchByNameMatchComplete.Query())
                {
                    while (FStatementSearchByNameMatchComplete.Fetch())
                    {
                        SearchData.Account = FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
                        SearchData.RoleName = FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
                        SearchData.IsDelete = FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.Sex = FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.IsHero = false;
                        RoleList.Add(SearchData);
                        Result++;
                    }
                }
            }
            finally
            {
                FStatementSearchByNameMatchComplete.Reset();
            }
        }
        else
        {
            try
            {
                FStatementSearchByNameMatchFuzzy.Reset();
                FStatementSearchByNameMatchFuzzy.OrderBindParamText("%" + HumanName + "%");
                if (FStatementSearchByNameMatchFuzzy.Query())
                {
                    while (FStatementSearchByNameMatchFuzzy.Fetch())
                    {
                        SearchData.Account = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
                        SearchData.RoleName = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
                        SearchData.IsDelete = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Sex = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.IsHero = false;
                        RoleList.Add(SearchData);
                        Result++;
                    }
                }
            }
            finally
            {
                FStatementSearchByNameMatchFuzzy.Reset();
            }
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:1115-1142 `DoSearchByLevel`。
    /// 原文缺陷（逐字保留）：`finally` 里 Reset 的是 **FStatementSearchByNameMatchFuzzy**，不是 FStatementSearchByLevel。
    /// </summary>
    protected override int DoSearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList)
    {
        int Result = 0;
        var SearchData = new TSerarchRoleData();
        try
        {
            FStatementSearchByLevel.Reset();
            FStatementSearchByLevel.OrderBindParamInt(MinLevel);
            FStatementSearchByLevel.OrderBindParamInt(LimitCount);
            if (FStatementSearchByLevel.Query())
            {
                while (FStatementSearchByLevel.Fetch())
                {
                    SearchData.Account = FStatementSearchByLevel.OrderGetColumnValueText;
                    SearchData.RoleName = FStatementSearchByLevel.OrderGetColumnValueText;
                    SearchData.IsDelete = FStatementSearchByLevel.OrderGetColumnValueInt;
                    SearchData.Sex = FStatementSearchByLevel.OrderGetColumnValueInt;
                    SearchData.Job = FStatementSearchByLevel.OrderGetColumnValueInt;
                    SearchData.Level = (uint)FStatementSearchByLevel.OrderGetColumnValueInt;
                    SearchData.IsHero = false;
                    RoleList.Add(SearchData);
                    Result++;
                }
            }
        }
        finally
        {
            FStatementSearchByNameMatchFuzzy.Reset();
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:1144-1178 `DoGetMobileNumbers`。
    /// OnlyBindMobile=false → 语句 1（只判 Length&gt;0）；true → 语句 2（额外 `IsMobileBind = 1`）。
    /// 注意 Result 在原文里**从未被显式初值化**，仅在循环里 Inc；C# 必须显式 = 0。
    /// </summary>
    protected override int DoGetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList)
    {
        int Result = 0;
        if (!OnlyBindMobile)
        {
            try
            {
                FStatementGetMobileNumbers1.Reset();
                if (FStatementGetMobileNumbers1.Query())
                {
                    while (FStatementGetMobileNumbers1.Fetch())
                    {
                        MobileNumberList.Add(FStatementGetMobileNumbers1.OrderGetColumnValueText);
                        Result++;
                    }
                }
            }
            finally
            {
                FStatementGetMobileNumbers1.Reset();
            }
        }
        else
        {
            try
            {
                FStatementGetMobileNumbers2.Reset();
                if (FStatementGetMobileNumbers2.Query())
                {
                    while (FStatementGetMobileNumbers2.Fetch())
                    {
                        MobileNumberList.Add(FStatementGetMobileNumbers2.OrderGetColumnValueText);
                        Result++;
                    }
                }
            }
            finally
            {
                FStatementGetMobileNumbers2.Reset();
            }
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:1180-1208 `DoSelect`。
    /// 先 `IsSelect = 1`（该条 Step 的结果即为返回值），再 `IsSelect = 0`（HumanName &lt;&gt; 本角色）；
    /// 异常 → RollBack + MainOutMessage。
    /// </summary>
    protected override bool DoSelect(string Account, string HumanName)
    {
        bool Result = false;
        BeginTransaction();
        try
        {
            try
            {
                FStatementSelect.Reset();
                FStatementSelect.OrderBindParamText(Account);
                FStatementSelect.OrderBindParamText(HumanName);
                Result = FStatementSelect.Step();

                FStatementUnSelect.Reset();
                FStatementUnSelect.OrderBindParamText(Account);
                FStatementUnSelect.OrderBindParamText(HumanName);
                FStatementUnSelect.Step();
            }
            finally
            {
                FStatementSelect.Reset();
                FStatementUnSelect.Reset();
            }

            Commit();
        }
        catch (Exception E)
        {
            RollBack();
            ReportException(E);
        }
        return Result;
    }
}
