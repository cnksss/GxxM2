using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// MySqlRoleDB.pas:139-219 + 3461-5817 `TMySqlHeroDB`（1:1）。
/// SQL 文本取 <see cref="MySqlRoleDBStatements.Hero"/>（脚本抽取，非手工转录）。
/// </summary>
public sealed class TMySqlHeroDB : THeroDBBase
{
    private readonly IRoleMySqlDatabase FDB;
    private readonly TMySqlRoleDB FOwner;

    protected override void OwnerLock() => FOwner.Lock();

    protected override void OwnerUnLock() => FOwner.UnLock();

    // ---- 54 个 TMySqlStatement 字段（MySqlRoleDB.pas:140-193），名字逐字保留 ----
    private IRoleMySqlStatement? FStatementGetID;
    private IRoleMySqlStatement? FStatementGetHeroCount;
    private IRoleMySqlStatement? FStatementGetHumanInfo;
    private IRoleMySqlStatement? FStatementGetHumanInfo2;
    private IRoleMySqlStatement? FStatementSearchByAccountMatchComplete;
    private IRoleMySqlStatement? FStatementSearchByAccountMatchFuzzy;
    private IRoleMySqlStatement? FStatementSearchByNameMatchComplete;
    private IRoleMySqlStatement? FStatementSearchByNameMatchFuzzy;
    private IRoleMySqlStatement? FStatementGetHero;
    private IRoleMySqlStatement? FStatementGetAbil;
    private IRoleMySqlStatement? FStatementGetAbilNG;
    private IRoleMySqlStatement? FStatementGetAbilWine;
    private IRoleMySqlStatement? FStatementGetAbilNpcAdd;
    private IRoleMySqlStatement? FStatementGetGodBlessState;
    private IRoleMySqlStatement? FStatementGetMagic;
    private IRoleMySqlStatement? FStatementGetStatusTime;
    private IRoleMySqlStatement? FStatementGetQuestFlag;
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
    private IRoleMySqlStatement? FStatementAddHero;
    private IRoleMySqlStatement? FStatementDeleteOrRestore;
    private IRoleMySqlStatement? FStatementUpdateHeroName;
    private IRoleMySqlStatement? FStatementUpdateHumanHeroName;
    private IRoleMySqlStatement? FStatementUpdateHumanHeroName2;
    private IRoleMySqlStatement? FStatementUpdateHero;
    private IRoleMySqlStatement? FStatementInsertAbil;
    private IRoleMySqlStatement? FStatementInsertAbilNG;
    private IRoleMySqlStatement? FStatementInsertAbilWine;
    private IRoleMySqlStatement? FStatementInsertAbilNpcAdd;
    private IRoleMySqlStatement? FStatementInsertGodBlessState;
    private IRoleMySqlStatement? FStatementInsertMagic;
    private IRoleMySqlStatement? FStatementInsertStatusTime;
    private IRoleMySqlStatement? FStatementInsertQuestFlag;
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

    internal TMySqlHeroDB(TMySqlRoleDB Owner)
    {
        FOwner = Owner;
        FDB = Owner.DB;
    }

    private void BeginTransaction() => FDB.StartTransaction();
    private void Commit() => FDB.Commit();
    private void RollBack() => FDB.RollBack();
    private void Execute(string Sql) => FDB.Exec(Sql);
    private static void ReportException(Exception E) => RoleDbSeam.MainOutMessage(E.Message);

    // ==========================================================================================
    // DoInit（MySqlRoleDB.pas:3463-3806） / DoFinal（行 3808-3934）
    // ==========================================================================================

    /// <summary>MySqlRoleDB.pas:3463-3806 `TMySqlHeroDB.DoInit`（54 条语句）。</summary>
    public void DoInit()
    {
        // 原文 Assert(Owner is TMySqlRoleDB, 'TSqliteHeroDB owner type error.') 与 Assert(FDB <> nil, ...)
        if (FOwner is not TMySqlRoleDB) throw new InvalidOperationException("TSqliteHeroDB owner type error.");
        if (FOwner.DB is null) throw new InvalidOperationException("TMySqlRoleDB.DB not create");

        FStatementGetID = Add("FStatementGetID", MySqlRoleDBStatements.Hero.FStatementGetID);
        FStatementGetHeroCount = Add("FStatementGetHeroCount", MySqlRoleDBStatements.Hero.FStatementGetHeroCount);
        FStatementGetHumanInfo = Add("FStatementGetHumanInfo", MySqlRoleDBStatements.Hero.FStatementGetHumanInfo);
        FStatementGetHumanInfo2 = Add("FStatementGetHumanInfo2", MySqlRoleDBStatements.Hero.FStatementGetHumanInfo2);
        FStatementSearchByAccountMatchComplete = Add("FStatementSearchByAccountMatchComplete", MySqlRoleDBStatements.Hero.FStatementSearchByAccountMatchComplete);
        FStatementSearchByAccountMatchFuzzy = Add("FStatementSearchByAccountMatchFuzzy", MySqlRoleDBStatements.Hero.FStatementSearchByAccountMatchFuzzy);
        FStatementSearchByNameMatchComplete = Add("FStatementSearchByNameMatchComplete", MySqlRoleDBStatements.Hero.FStatementSearchByNameMatchComplete);
        FStatementSearchByNameMatchFuzzy = Add("FStatementSearchByNameMatchFuzzy", MySqlRoleDBStatements.Hero.FStatementSearchByNameMatchFuzzy);
        FStatementGetHero = Add("FStatementGetHero", MySqlRoleDBStatements.Hero.FStatementGetHero);
        FStatementGetAbil = Add("FStatementGetAbil", MySqlRoleDBStatements.Hero.FStatementGetAbil);
        FStatementGetAbilNG = Add("FStatementGetAbilNG", MySqlRoleDBStatements.Hero.FStatementGetAbilNG);
        FStatementGetAbilWine = Add("FStatementGetAbilWine", MySqlRoleDBStatements.Hero.FStatementGetAbilWine);
        FStatementGetAbilNpcAdd = Add("FStatementGetAbilNpcAdd", MySqlRoleDBStatements.Hero.FStatementGetAbilNpcAdd);
        FStatementGetGodBlessState = Add("FStatementGetGodBlessState", MySqlRoleDBStatements.Hero.FStatementGetGodBlessState);
        FStatementGetMagic = Add("FStatementGetMagic", MySqlRoleDBStatements.Hero.FStatementGetMagic);
        FStatementGetStatusTime = Add("FStatementGetStatusTime", MySqlRoleDBStatements.Hero.FStatementGetStatusTime);
        FStatementGetQuestFlag = Add("FStatementGetQuestFlag", MySqlRoleDBStatements.Hero.FStatementGetQuestFlag);
        FStatementGetItems = Add("FStatementGetItems", MySqlRoleDBStatements.Hero.FStatementGetItems);
        FStatementGetItemValueAdd = Add("FStatementGetItemValueAdd", MySqlRoleDBStatements.Hero.FStatementGetItemValueAdd);
        FStatementGetItemElementAdd = Add("FStatementGetItemElementAdd", MySqlRoleDBStatements.Hero.FStatementGetItemElementAdd);
        FStatementGetItemAddDataByte = Add("FStatementGetItemAddDataByte", MySqlRoleDBStatements.Hero.FStatementGetItemAddDataByte);
        FStatementGetItemAddDataInt = Add("FStatementGetItemAddDataInt", MySqlRoleDBStatements.Hero.FStatementGetItemAddDataInt);
        FStatementGetItemAddDataText = Add("FStatementGetItemAddDataText", MySqlRoleDBStatements.Hero.FStatementGetItemAddDataText);
        FStatementGetItemFlute = Add("FStatementGetItemFlute", MySqlRoleDBStatements.Hero.FStatementGetItemFlute);
        FStatementGetItemProgress = Add("FStatementGetItemProgress", MySqlRoleDBStatements.Hero.FStatementGetItemProgress);
        FStatementGetItemProperty = Add("FStatementGetItemProperty", MySqlRoleDBStatements.Hero.FStatementGetItemProperty);
        FStatementGetSkillPower = Add("FStatementGetSkillPower", MySqlRoleDBStatements.Hero.FStatementGetSkillPower);
        FStatementAddHero = Add("FStatementAddHero", MySqlRoleDBStatements.Hero.FStatementAddHero);
        FStatementDeleteOrRestore = Add("FStatementDeleteOrRestore", MySqlRoleDBStatements.Hero.FStatementDeleteOrRestore);
        FStatementUpdateHeroName = Add("FStatementUpdateHeroName", MySqlRoleDBStatements.Hero.FStatementUpdateHeroName);
        FStatementUpdateHumanHeroName = Add("FStatementUpdateHumanHeroName", MySqlRoleDBStatements.Hero.FStatementUpdateHumanHeroName);
        FStatementUpdateHumanHeroName2 = Add("FStatementUpdateHumanHeroName2", MySqlRoleDBStatements.Hero.FStatementUpdateHumanHeroName2);
        FStatementUpdateHero = Add("FStatementUpdateHero", MySqlRoleDBStatements.Hero.FStatementUpdateHero);
        FStatementInsertAbil = Add("FStatementInsertAbil", MySqlRoleDBStatements.Hero.FStatementInsertAbil);
        FStatementInsertAbilNG = Add("FStatementInsertAbilNG", MySqlRoleDBStatements.Hero.FStatementInsertAbilNG);
        FStatementInsertAbilWine = Add("FStatementInsertAbilWine", MySqlRoleDBStatements.Hero.FStatementInsertAbilWine);
        // 原文 { } 注释掉的 FStatementUpdateAbil / UpdateAbilNG / UpdateAbilWine 三条**不创建**（行 3588-3681）
        FStatementInsertAbilNpcAdd = Add("FStatementInsertAbilNpcAdd", MySqlRoleDBStatements.Hero.FStatementInsertAbilNpcAdd);
        FStatementInsertGodBlessState = Add("FStatementInsertGodBlessState", MySqlRoleDBStatements.Hero.FStatementInsertGodBlessState);
        FStatementInsertMagic = Add("FStatementInsertMagic", MySqlRoleDBStatements.Hero.FStatementInsertMagic);
        FStatementInsertStatusTime = Add("FStatementInsertStatusTime", MySqlRoleDBStatements.Hero.FStatementInsertStatusTime);
        FStatementInsertQuestFlag = Add("FStatementInsertQuestFlag", MySqlRoleDBStatements.Hero.FStatementInsertQuestFlag);
        FStatementInsertItems = Add("FStatementInsertItems", MySqlRoleDBStatements.Hero.FStatementInsertItems);
        FStatementInsertItemValueAdd = Add("FStatementInsertItemValueAdd", MySqlRoleDBStatements.Hero.FStatementInsertItemValueAdd);
        FStatementInsertItemElementAdd = Add("FStatementInsertItemElementAdd", MySqlRoleDBStatements.Hero.FStatementInsertItemElementAdd);
        FStatementInsertItemAddDataByte = Add("FStatementInsertItemAddDataByte", MySqlRoleDBStatements.Hero.FStatementInsertItemAddDataByte);
        FStatementInsertItemAddDataInt = Add("FStatementInsertItemAddDataInt", MySqlRoleDBStatements.Hero.FStatementInsertItemAddDataInt);
        FStatementInsertItemAddDataText = Add("FStatementInsertItemAddDataText", MySqlRoleDBStatements.Hero.FStatementInsertItemAddDataText);
        FStatementInsertItemFlute = Add("FStatementInsertItemFlute", MySqlRoleDBStatements.Hero.FStatementInsertItemFlute);
        FStatementInsertItemProgress = Add("FStatementInsertItemProgress", MySqlRoleDBStatements.Hero.FStatementInsertItemProgress);
        FStatementInsertItemProperty = Add("FStatementInsertItemProperty", MySqlRoleDBStatements.Hero.FStatementInsertItemProperty);
        FStatementInsertSkillPower = Add("FStatementInsertSkillPower", MySqlRoleDBStatements.Hero.FStatementInsertSkillPower);
        FStatementGetLevelRankCheckLevel = Add("FStatementGetLevelRankCheckLevel", MySqlRoleDBStatements.Hero.FStatementGetLevelRankCheckLevel);
        FStatementGetLevelRankTopCount = Add("FStatementGetLevelRankTopCount", MySqlRoleDBStatements.Hero.FStatementGetLevelRankTopCount);
        FStatementGetLevelRankCheckLevelAndCount = Add("FStatementGetLevelRankCheckLevelAndCount", MySqlRoleDBStatements.Hero.FStatementGetLevelRankCheckLevelAndCount);
    }

    private IRoleMySqlStatement Add(string fieldKey, string sql)
    {
        IRoleMySqlStatement S = FDB.AddSQLStatement(MySqlRoleDBStatements.Hero.Names[fieldKey]);
        S.Sql = sql;
        S.Prepare();
        return S;
    }

    /// <summary>MySqlRoleDB.pas:3808-3934 `TMySqlHeroDB.DoFinal`。</summary>
    public void DoFinal()
    {
        Finalize(FStatementGetID);
        Finalize(FStatementGetHeroCount);
        Finalize(FStatementGetHumanInfo);
        Finalize(FStatementGetHumanInfo2);
        Finalize(FStatementSearchByAccountMatchComplete);
        Finalize(FStatementSearchByAccountMatchFuzzy);
        Finalize(FStatementSearchByNameMatchComplete);
        Finalize(FStatementSearchByNameMatchFuzzy);
        Finalize(FStatementGetHero);
        Finalize(FStatementGetAbil);
        Finalize(FStatementGetAbilNG);
        Finalize(FStatementGetAbilWine);
        Finalize(FStatementGetAbilNpcAdd);
        Finalize(FStatementGetGodBlessState);
        Finalize(FStatementGetMagic);
        Finalize(FStatementGetStatusTime);
        Finalize(FStatementGetQuestFlag);
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
        Finalize(FStatementAddHero);
        Finalize(FStatementDeleteOrRestore);
        Finalize(FStatementUpdateHeroName);
        Finalize(FStatementUpdateHumanHeroName);
        Finalize(FStatementUpdateHumanHeroName2);
        Finalize(FStatementUpdateHero);
        Finalize(FStatementInsertAbil);
        Finalize(FStatementInsertAbilNG);
        Finalize(FStatementInsertAbilWine);
        Finalize(FStatementInsertAbilNpcAdd);
        Finalize(FStatementInsertGodBlessState);
        Finalize(FStatementInsertMagic);
        Finalize(FStatementInsertStatusTime);
        Finalize(FStatementInsertQuestFlag);
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
    }

    private static void Finalize(IRoleMySqlStatement S) => S?.FinalizeStatement();

    // ==========================================================================================
    // 读（MySqlRoleDB.pas:3936-4548）
    // ==========================================================================================

    /// <summary>MySqlRoleDB.pas:3936-3948 `DoGetID`。查不到 → NO_ID(-1)。</summary>
    protected override int DoGetID(string HeroName)
    {
        try
        {
            FStatementGetID.Reset();
            FStatementGetID.OrderBindParamText(HeroName);
            if (FStatementGetID.Query() && FStatementGetID.Fetch())
                return FStatementGetID.GetColumnValueInt(0);
            return RoleDbConst.NO_ID;
        }
        finally
        {
            FStatementGetID.Reset();
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:3950-4008 `DoSearchByAccount`。
    /// 与 Human 版的两处真实差异：IsDelete 恒为 **0**（不读列），IsHero 恒为 **True**。
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
                        SearchData.IsDelete = 0;
                        SearchData.Sex = FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
                        SearchData.IsHero = true;
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
                        SearchData.IsDelete = 0;
                        SearchData.Sex = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.IsHero = true;
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

    /// <summary>MySqlRoleDB.pas:4010-4069 `DoSearchByName`（IsDelete 恒 0、IsHero 恒 True）。</summary>
    protected override int DoSearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        int Result = 0;
        var SearchData = new TSerarchRoleData();
        if (MatchType == TSearchMatchType.smtComplete)
        {
            try
            {
                FStatementSearchByNameMatchComplete.Reset();
                FStatementSearchByNameMatchComplete.OrderBindParamText(HeroName);
                if (FStatementSearchByNameMatchComplete.Query())
                {
                    while (FStatementSearchByNameMatchComplete.Fetch())
                    {
                        SearchData.Account = FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
                        SearchData.RoleName = FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
                        SearchData.IsDelete = 0;
                        SearchData.Sex = FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
                        SearchData.IsHero = true;
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
                FStatementSearchByNameMatchFuzzy.OrderBindParamText("%" + HeroName + "%");
                if (FStatementSearchByNameMatchFuzzy.Query())
                {
                    while (FStatementSearchByNameMatchFuzzy.Fetch())
                    {
                        SearchData.Account = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
                        SearchData.RoleName = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
                        SearchData.IsDelete = 0;
                        SearchData.Sex = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Job = FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.Level = (uint)FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
                        SearchData.IsHero = true;
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
    /// MySqlRoleDB.pas:4550-4581 `GetHeroUserItem`。
    /// 英雄只有 **5** 个物品数组（HumItems / JewelryBoxItems / GodBlessItems / FengHaoItems / BagItems）；
    /// ItemType 6 / 7 在英雄侧**不存在**（与 Human 版的 7 组是真实差异）。
    /// </summary>
    private static bool IsHeroItemSlotValid(int ItemType, int ItemIndex)
    {
        switch (ItemType)
        {
            case 1: return ItemIndex >= 0 && ItemIndex <= 29;    // HumItems[0..29]
            case 2: return ItemIndex >= 0 && ItemIndex <= 5;     // JewelryBoxItems[0..5]
            case 3: return ItemIndex >= 0 && ItemIndex <= 11;    // GodBlessItems[0..11]
            case 4: return ItemIndex >= 0 && ItemIndex <= 59;    // FengHaoItems[0..59]
            case 5: return ItemIndex >= 0 && ItemIndex <= 205;   // BagItems[0..205]
            default: return false;
        }
    }

    private static TUserItem GetHeroItem(ref THeroData H, int ItemType, int ItemIndex)
    {
        switch (ItemType)
        {
            case 1: return H.HumItems[ItemIndex];
            case 2: return H.JewelryBoxItems[ItemIndex];
            case 3: return H.GodBlessItems[ItemIndex];
            case 4: return H.FengHaoItems[ItemIndex];
            case 5: return H.BagItems[ItemIndex];
            default: return default;
        }
    }

    private static void SetHeroItem(ref THeroData H, int ItemType, int ItemIndex, in TUserItem Value)
    {
        switch (ItemType)
        {
            case 1: H.HumItems[ItemIndex] = Value; break;
            case 2: H.JewelryBoxItems[ItemIndex] = Value; break;
            case 3: H.GodBlessItems[ItemIndex] = Value; break;
            case 4: H.FengHaoItems[ItemIndex] = Value; break;
            case 5: H.BagItems[ItemIndex] = Value; break;
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:4071-4548 `TMySqlHeroDB.DoGet`。
    /// 与 Human.DoGet 的结构差异（已在报告登记）：
    ///   · 主表列里**没有** HomeMap/HomeX/HomeY（HeroSelect 里被注释掉）、没有 sMasterName；
    ///   · HeroAbilWine 只有 9 列（无 IsDrinkedWine / boPleaseDrink）；
    ///   · 无 GamePetData / MagicUseTick / VariableU/T/J/Z / CustomMoney / GamePetBagItems；
    ///   · 物品只有 5 组。
    /// </summary>
    protected override bool DoGet(string HeroName, ref THeroData HeroData, out int HeroID)
    {
        int I, J, TheType, Index, Index2, Value;
        bool Result = false;
        HeroID = 0;
        THeroData H = HeroData;
        try
        {
            FStatementGetHero.Reset();
            FStatementGetHero.OrderBindParamText(HeroName);
            if (FStatementGetHero.Query() && FStatementGetHero.Fetch())
            {
                Result = true;
                H = default;                             // FillChar(HeroData, SizeOf(HeroData), 0)
                HeroID = FStatementGetHero.OrderGetColumnValueInt;                        // HeroID

                H.Account = FStatementGetHero.OrderGetColumnValueText;                    // (select Account...)
                H.ChrName = FStatementGetHero.OrderGetColumnValueText;                    // HeroName
                H.btSex = (byte)FStatementGetHero.OrderGetColumnValueInt;                 // Sex
                H.btJob = (byte)FStatementGetHero.OrderGetColumnValueInt;                 // Job
                H.btHair = (byte)FStatementGetHero.OrderGetColumnValueInt;                // Hair
                H.btStatus = (byte)FStatementGetHero.OrderGetColumnValueInt;              // Status
                H.btDir = (byte)FStatementGetHero.OrderGetColumnValueInt;                 // Dir
                H.Abil.Level = FStatementGetHero.OrderGetColumnValueInt;                  // Level
                H.btReLevel = (byte)FStatementGetHero.OrderGetColumnValueInt;             // ReLevel
                H.rLoyalPoint = FStatementGetHero.OrderGetColumnValueDouble;              // LoyalPoint
                H.CurMap = FStatementGetHero.OrderGetColumnValueText;                     // Map
                H.wCurX = (ushort)FStatementGetHero.OrderGetColumnValueInt;               // X
                H.wCurY = (ushort)FStatementGetHero.OrderGetColumnValueInt;               // Y
                // H.HomeMap := ...; H.wHomeX := ...; H.wHomeY := ...;                     // 原文注释掉（SQL 里也没有）
                H.btAttackMode = (byte)FStatementGetHero.OrderGetColumnValueInt;          // AttackMode
                H.Abil.CreditPoint = FStatementGetHero.OrderGetColumnValueInt;            // CreditPoint
                H.nPKPoint = FStatementGetHero.OrderGetColumnValueInt;                    // PKPoint
                H.btIncHealth = (byte)FStatementGetHero.OrderGetColumnValueInt;           // IncHP
                H.btIncSpell = (byte)FStatementGetHero.OrderGetColumnValueInt;            // IncMP
                H.btIncHealing = (byte)FStatementGetHero.OrderGetColumnValueInt;          // IncHP2
                H.btFightZoneDieCount = (byte)FStatementGetHero.OrderGetColumnValueInt;   // FightZoneDieCount
                H.dBodyLuck = FStatementGetHero.OrderGetColumnValueDouble;                // BodyLuck
                H.nHungerStatus = FStatementGetHero.OrderGetColumnValueInt;               // HungerStatus
                H.nRevivalTime = FStatementGetHero.OrderGetColumnValueInt;                // RevivalTime
                H.boSaveKillMonExpRate = (byte)(FStatementGetHero.OrderGetColumnValueBool ? 1 : 0);   // IsSaveKillMonExpRate
                H.nKillMonExpRate = FStatementGetHero.OrderGetColumnValueInt;             // KillMonExpRate
                H.dwKillMonExpRateTime = (uint)FStatementGetHero.OrderGetColumnValueInt;  // KillMonExpRateTime
                H.boAttackHumSavePowerRate = (byte)(FStatementGetHero.OrderGetColumnValueBool ? 1 : 0);   // IsSavePowerRate
                H.nAttackHumPowerRate = FStatementGetHero.OrderGetColumnValueInt;                 // PowerRate
                H.dwAttackHumPowerRateTime = (uint)FStatementGetHero.OrderGetColumnValueInt;      // PowerRateTime
                H.boAttackMonSavePowerRate = (byte)(FStatementGetHero.OrderGetColumnValueBool ? 1 : 0);   // IsAttackMonSavePowerRate
                H.nAttackMonPowerRate = FStatementGetHero.OrderGetColumnValueInt;                 // AttackMonPowerRate
                H.dwAttackMonPowerRateTime = (uint)FStatementGetHero.OrderGetColumnValueInt;      // AttackMonPowerRateTime
                H.boSaveKillMonBurstRate = (byte)(FStatementGetHero.OrderGetColumnValueBool ? 1 : 0);     // IsSaveKillMonBurstRate
                H.nKillMonBurstRate = FStatementGetHero.OrderGetColumnValueInt;           // KillMonBurstRate
                H.dwKillMonBurstRateTime = (uint)FStatementGetHero.OrderGetColumnValueInt; // KillMonBurstRateTime
                H.JewelryBoxStatus = (TJewelryBoxStatus)FStatementGetHero.OrderGetColumnValueInt;  // JewelryBoxStatus
                H.boShowFashion = (byte)(FStatementGetHero.OrderGetColumnValueBool ? 1 : 0);       // IsShowFashion
                H.boShowGodBless = (byte)(FStatementGetHero.OrderGetColumnValueBool ? 1 : 0);      // IsShowGodBless
                H.nActiveFengHao = (sbyte)FStatementGetHero.OrderGetColumnValueInt;       // ActiveFengHao
            }

            if (!Result)
                return false;

            // ---------------- HeroAbil（22 列，无 AdjustAbil* / nBonusPoint / BonusAbil） ----------------
            FStatementGetAbil.Reset();
            FStatementGetAbil.OrderBindParamInt(HeroID);
            if (FStatementGetAbil.Query() && FStatementGetAbil.Fetch())
            {
                H.Abil.AC1 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.AC2 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MAC1 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MAC2 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.DC1 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.DC2 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MC1 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MC2 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.SC1 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.SC2 = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.HP = (uint)FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MaxHP = (uint)FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MP = (uint)FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MaxMP = (uint)FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.Exp = (uint)FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MaxExp = (uint)FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.Weight = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MaxWeight = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.WearWeight = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MaxWearWeight = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.HandWeight = FStatementGetAbil.OrderGetColumnValueInt;
                H.Abil.MaxHandWeight = FStatementGetAbil.OrderGetColumnValueInt;
            }

            // ---------------- HeroAbilNG ----------------
            FStatementGetAbilNG.Reset();
            FStatementGetAbilNG.OrderBindParamInt(HeroID);
            if (FStatementGetAbilNG.Query() && FStatementGetAbilNG.Fetch())
            {
                H.boTrainingNG = (byte)(FStatementGetAbilNG.OrderGetColumnValueBool ? 1 : 0);
                H.boTrainingXF = (byte)(FStatementGetAbilNG.OrderGetColumnValueBool ? 1 : 0);
                H.AbilNG.Level = (ushort)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.AbilNG.NH = (ushort)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.AbilNG.MaxNH = (ushort)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.AbilNG.Exp = (uint)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.AbilNG.MaxExp = (uint)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.ContinuousMagicOrder[0] = (byte)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.ContinuousMagicOrder[1] = (byte)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.ContinuousMagicOrder[2] = (byte)FStatementGetAbilNG.OrderGetColumnValueInt;
                H.boOpenLastContinuous = (byte)(FStatementGetAbilNG.OrderGetColumnValueBool ? 1 : 0);
                H.btLastContinuousMagicOrder = (byte)FStatementGetAbilNG.OrderGetColumnValueInt;

                for (I = 0; I <= 4; I++)
                {
                    H.Meridians[I].Level = (byte)FStatementGetAbilNG.OrderGetColumnValueInt;
                    H.Meridians[I].BlastHitRate = (byte)FStatementGetAbilNG.OrderGetColumnValueInt;
                    for (J = 0; J <= 4; J++)
                        MySqlItemAccess.SetHeroMeridianAcupoint(ref H, I, J, (byte)FStatementGetAbilNG.OrderGetColumnValueInt);
                }
            }

            // ---------------- HeroAbilWine（9 列：无 IsDrinkedWine / boPleaseDrink） ----------------
            FStatementGetAbilWine.Reset();
            FStatementGetAbilWine.OrderBindParamInt(HeroID);
            if (FStatementGetAbilWine.Query() && FStatementGetAbilWine.Fetch())
            {
                H.boDrinkWineDrunk = (byte)(FStatementGetAbilWine.OrderGetColumnValueBool ? 1 : 0);         // IsDrinkWineDrunk
                H.nDrinkWineQuality = FStatementGetAbilWine.OrderGetColumnValueInt;                        // DrinkWineQuality
                H.nDrinkWineAlcohol = FStatementGetAbilWine.OrderGetColumnValueInt;                        // DrinkWineAlcohol
                H.Alcohol.Alcohol = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;                  // AbilAlcohol
                H.Alcohol.MaxAlcohol = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;               // AbilMaxAlcohol
                H.Alcohol.WineDrinkValue = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;           // AbilDrinkValue
                H.Alcohol.MedicineLevel = FStatementGetAbilWine.OrderGetColumnValueInt;                    // AbilMedicineLevel
                H.Alcohol.MedicineValue = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;            // AbilMedicineValue
                H.Alcohol.MaxMedicineValue = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;         // AbilMaxMedicineValue
            }

            // ---------------- HeroAbilNpcAdd（AddSaveAbil[0..29]） ----------------
            FStatementGetAbilNpcAdd.Reset();
            FStatementGetAbilNpcAdd.OrderBindParamInt(HeroID);
            if (FStatementGetAbilNpcAdd.Query())
            {
                while (FStatementGetAbilNpcAdd.Fetch())
                {
                    Index = FStatementGetAbilNpcAdd.OrderGetColumnValueInt;
                    Value = FStatementGetAbilNpcAdd.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 29)
                        H.AddSaveAbil[Index] = Value;
                }
            }

            // ---------------- HeroGodBlessState（GodBlessItemsState[0..11]） ----------------
            FStatementGetGodBlessState.Reset();
            FStatementGetGodBlessState.OrderBindParamInt(HeroID);
            if (FStatementGetGodBlessState.Query())
            {
                while (FStatementGetGodBlessState.Fetch())
                {
                    Index = FStatementGetGodBlessState.OrderGetColumnValueInt;
                    Value = FStatementGetGodBlessState.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 11)
                        H.GodBlessItemsState[Index] = (byte)Value;
                }
            }

            // ---------------- HeroMagic（无 MagicUseTick） ----------------
            FStatementGetMagic.Reset();
            FStatementGetMagic.OrderBindParamInt(HeroID);
            if (FStatementGetMagic.Query())
            {
                while (FStatementGetMagic.Fetch())
                {
                    TheType = FStatementGetMagic.OrderGetColumnValueInt;
                    Index = FStatementGetMagic.OrderGetColumnValueInt;
                    bool HasMagic = false;
                    switch (TheType)
                    {
                        case 1: HasMagic = Index >= 0 && Index <= 47; break;
                        case 2: HasMagic = Index >= 0 && Index <= 47; break;
                        case 3: HasMagic = Index >= 0 && Index <= 5; break;
                    }

                    int wMagIdx = FStatementGetMagic.OrderGetColumnValueInt;
                    var MagicAttr = (TMagicAttr)FStatementGetMagic.OrderGetColumnValueInt;
                    byte btLevel = (byte)FStatementGetMagic.OrderGetColumnValueInt;
                    byte btNewLevel = (byte)FStatementGetMagic.OrderGetColumnValueInt;
                    byte btKey = (byte)FStatementGetMagic.OrderGetColumnValueInt;
                    int nTranPoint = FStatementGetMagic.OrderGetColumnValueInt;
                    byte boUsesItemAdd = (byte)(FStatementGetMagic.OrderGetColumnValueBool ? 1 : 0);

                    if (!HasMagic) continue;
                    switch (TheType)
                    {
                        case 1:
                            H.Magics[Index].wMagIdx = (ushort)wMagIdx;
                            H.Magics[Index].MagicAttr = MagicAttr;
                            H.Magics[Index].btLevel = btLevel;
                            H.Magics[Index].btNewLevel = btNewLevel;
                            H.Magics[Index].btKey = btKey;
                            H.Magics[Index].nTranPoint = nTranPoint;
                            H.Magics[Index].boUsesItemAdd = boUsesItemAdd;
                            break;
                        case 2:
                            H.NGMagics[Index].wMagIdx = (ushort)wMagIdx;
                            H.NGMagics[Index].MagicAttr = MagicAttr;
                            H.NGMagics[Index].btLevel = btLevel;
                            H.NGMagics[Index].btNewLevel = btNewLevel;
                            H.NGMagics[Index].btKey = btKey;
                            H.NGMagics[Index].nTranPoint = nTranPoint;
                            H.NGMagics[Index].boUsesItemAdd = boUsesItemAdd;
                            break;
                        case 3:
                            H.ContinuousMagics[Index].wMagIdx = (ushort)wMagIdx;
                            H.ContinuousMagics[Index].MagicAttr = MagicAttr;
                            H.ContinuousMagics[Index].btLevel = btLevel;
                            H.ContinuousMagics[Index].btNewLevel = btNewLevel;
                            H.ContinuousMagics[Index].btKey = btKey;
                            H.ContinuousMagics[Index].nTranPoint = nTranPoint;
                            H.ContinuousMagics[Index].boUsesItemAdd = boUsesItemAdd;
                            break;
                    }
                }
            }

            // ---------------- HeroStatusTime（wStatusTimeArr[0..17]） ----------------
            FStatementGetStatusTime.Reset();
            FStatementGetStatusTime.OrderBindParamInt(HeroID);
            if (FStatementGetStatusTime.Query())
            {
                while (FStatementGetStatusTime.Fetch())
                {
                    Index = FStatementGetStatusTime.OrderGetColumnValueInt;
                    Value = FStatementGetStatusTime.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 17)
                        H.wStatusTimeArr[Index] = (ushort)Value;
                }
            }

            // ---------------- HeroQuestFlag（QuestFlag[0..127]） ----------------
            FStatementGetQuestFlag.Reset();
            FStatementGetQuestFlag.OrderBindParamInt(HeroID);
            if (FStatementGetQuestFlag.Query())
            {
                while (FStatementGetQuestFlag.Fetch())
                {
                    Index = FStatementGetQuestFlag.OrderGetColumnValueInt;
                    Value = FStatementGetQuestFlag.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 127)
                        MySqlItemAccess.SetHeroQuestFlag(ref H, Index, (byte)Value);
                }
            }

            // ---------------- HeroItems（ItemType 1..5） ----------------
            FStatementGetItems.Reset();
            FStatementGetItems.OrderBindParamInt(HeroID);
            if (FStatementGetItems.Query())
            {
                while (FStatementGetItems.Fetch())
                {
                    TheType = FStatementGetItems.OrderGetColumnValueInt;
                    Index = FStatementGetItems.OrderGetColumnValueInt;
                    var It = default(TUserItem);
                    It.MakeIndex = FStatementGetItems.OrderGetColumnValueInt;
                    It.wIndex = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.NameStr = FStatementGetItems.OrderGetColumnValueText;
                    It.Dura = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.DuraMax = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.dwHeroM2DressEffect = (uint)FStatementGetItems.OrderGetColumnValueInt;
                    It.btUpgradeCount = (byte)FStatementGetItems.OrderGetColumnValueInt;
                    It.boStartTime = (byte)(FStatementGetItems.OrderGetColumnValueBool ? 1 : 0);
                    It.nLimitTime = FStatementGetItems.OrderGetColumnValueInt;
                    It.btHeroM2Light = (byte)FStatementGetItems.OrderGetColumnValueInt;
                    It.btColor = (byte)FStatementGetItems.OrderGetColumnValueInt;
                    It.boIsBind = (byte)(FStatementGetItems.OrderGetColumnValueBool ? 1 : 0);
                    It.btBindOption = (byte)FStatementGetItems.OrderGetColumnValueInt;
                    It.wEffect = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.wNewLooks = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.wNewShape = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.btFluteCount = (byte)FStatementGetItems.OrderGetColumnValueInt;
                    It.CustomProperty.TextStr = FStatementGetItems.OrderGetColumnValueText;
                    It.CustomProperty.btTextColor = (byte)FStatementGetItems.OrderGetColumnValueInt;
                    It.ItemFrom.ItemForm = (TItemFormType)FStatementGetItems.OrderGetColumnValueInt;
                    It.ItemFrom.MapName = FStatementGetItems.OrderGetColumnValueText;
                    It.ItemFrom.MonName = FStatementGetItems.OrderGetColumnValueText;
                    It.ItemFrom.MakerName = FStatementGetItems.OrderGetColumnValueText;
                    It.ItemFrom.DateTime = FStatementGetItems.OrderGetColumnValueDouble;
                    It.wInsuranceCount = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.wNewExpand3 = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    It.wNewExpand4 = (ushort)FStatementGetItems.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index))
                        SetHeroItem(ref H, TheType, Index, It);
                }
            }

            // ---------------- HeroItemValueAdd ----------------
            FStatementGetItemValueAdd.Reset();
            FStatementGetItemValueAdd.OrderBindParamInt(HeroID);
            if (FStatementGetItemValueAdd.Query())
            {
                while (FStatementGetItemValueAdd.Fetch())
                {
                    TheType = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                    Index = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 13)
                    {
                        Value = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                        var It = GetHeroItem(ref H, TheType, Index);
                        It.SetBtValue(Index2, Value);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemElementAdd ----------------
            FStatementGetItemElementAdd.Reset();
            FStatementGetItemElementAdd.OrderBindParamInt(HeroID);
            if (FStatementGetItemElementAdd.Query())
            {
                while (FStatementGetItemElementAdd.Fetch())
                {
                    TheType = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                    Index = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 29)
                    {
                        Value = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                        var It = GetHeroItem(ref H, TheType, Index);
                        It.SetNewValue(Index2, (ushort)Value);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemAddDataByte ----------------
            FStatementGetItemAddDataByte.Reset();
            FStatementGetItemAddDataByte.OrderBindParamInt(HeroID);
            if (FStatementGetItemAddDataByte.Query())
            {
                while (FStatementGetItemAddDataByte.Fetch())
                {
                    TheType = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    Index = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= Grobal2Const.USER_ITEM_ADD_DATA_BYTE_COUNT - 1)
                    {
                        Value = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                        var It = GetHeroItem(ref H, TheType, Index);
                        MySqlItemAccess.SetAddDataByte(ref It, Index2, (byte)Value);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemAddDataInt ----------------
            FStatementGetItemAddDataInt.Reset();
            FStatementGetItemAddDataInt.OrderBindParamInt(HeroID);
            if (FStatementGetItemAddDataInt.Query())
            {
                while (FStatementGetItemAddDataInt.Fetch())
                {
                    TheType = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    Index = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= Grobal2Const.USER_ITEM_ADD_DATA_INT_COUNT - 1)
                    {
                        Value = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                        var It = GetHeroItem(ref H, TheType, Index);
                        MySqlItemAccess.SetAddDataInt(ref It, Index2, Value);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemAddDataText ----------------
            FStatementGetItemAddDataText.Reset();
            FStatementGetItemAddDataText.OrderBindParamInt(HeroID);
            if (FStatementGetItemAddDataText.Query())
            {
                while (FStatementGetItemAddDataText.Fetch())
                {
                    TheType = FStatementGetItemAddDataText.OrderGetColumnValueInt;
                    Index = FStatementGetItemAddDataText.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemAddDataText.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 1)
                    {
                        string sTemp = FStatementGetItemAddDataText.OrderGetColumnValueText;
                        var It = GetHeroItem(ref H, TheType, Index);
                        It.SetAddDataText(Index2, sTemp);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemFlute ----------------
            FStatementGetItemFlute.Reset();
            FStatementGetItemFlute.OrderBindParamInt(HeroID);
            if (FStatementGetItemFlute.Query())
            {
                while (FStatementGetItemFlute.Fetch())
                {
                    TheType = FStatementGetItemFlute.OrderGetColumnValueInt;
                    Index = FStatementGetItemFlute.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemFlute.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 7)
                    {
                        var It = GetHeroItem(ref H, TheType, Index);
                        var Flute = It.GetFlute(Index2);
                        Flute.GemIndex = (ushort)FStatementGetItemFlute.OrderGetColumnValueInt;
                        Flute.GemCount = (ushort)FStatementGetItemFlute.OrderGetColumnValueInt;
                        if (Flute.GemIndex > 0 && Flute.GemCount == 0)
                            Flute.GemCount = 1;
                        It.SetFlute(Index2, Flute);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemProgress ----------------
            FStatementGetItemProgress.Reset();
            FStatementGetItemProgress.OrderBindParamInt(HeroID);
            if (FStatementGetItemProgress.Query())
            {
                while (FStatementGetItemProgress.Fetch())
                {
                    TheType = FStatementGetItemProgress.OrderGetColumnValueInt;
                    Index = FStatementGetItemProgress.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemProgress.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 1)
                    {
                        var It = GetHeroItem(ref H, TheType, Index);
                        var P = Index2 == 0 ? It.Progress0 : It.Progress1;
                        P.boOpen = (byte)(FStatementGetItemProgress.OrderGetColumnValueBool ? 1 : 0);
                        P.btNameColor = (byte)FStatementGetItemProgress.OrderGetColumnValueInt;
                        P.btCount = (byte)FStatementGetItemProgress.OrderGetColumnValueInt;
                        P.btShowType = (byte)FStatementGetItemProgress.OrderGetColumnValueInt;
                        P.wMax = (ushort)FStatementGetItemProgress.OrderGetColumnValueInt;
                        P.wValue = (ushort)FStatementGetItemProgress.OrderGetColumnValueInt;
                        P.wLevel = (ushort)FStatementGetItemProgress.OrderGetColumnValueInt;
                        P.NameStr = FStatementGetItemProgress.OrderGetColumnValueText;
                        if (Index2 == 0) It.Progress0 = P; else It.Progress1 = P;
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroItemProperty ----------------
            FStatementGetItemProperty.Reset();
            FStatementGetItemProperty.OrderBindParamInt(HeroID);
            if (FStatementGetItemProperty.Query())
            {
                while (FStatementGetItemProperty.Fetch())
                {
                    TheType = FStatementGetItemProperty.OrderGetColumnValueInt;
                    Index = FStatementGetItemProperty.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemProperty.OrderGetColumnValueInt;
                    if (IsHeroItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 19)
                    {
                        var It = GetHeroItem(ref H, TheType, Index);
                        var Pr = It.CustomProperty.GetProp(Index2);
                        Pr.btColor = (byte)FStatementGetItemProperty.OrderGetColumnValueInt;
                        Pr.btBindType = (byte)FStatementGetItemProperty.OrderGetColumnValueInt;
                        Pr.btShowFlag = (byte)FStatementGetItemProperty.OrderGetColumnValueInt;
                        Pr.btPercent = (byte)FStatementGetItemProperty.OrderGetColumnValueInt;
                        Pr.btHintModule = (byte)FStatementGetItemProperty.OrderGetColumnValueInt;
                        var vals = Pr.GetValues();
                        vals[0] = FStatementGetItemProperty.OrderGetColumnValueInt;
                        vals[1] = FStatementGetItemProperty.OrderGetColumnValueInt;
                        vals[2] = FStatementGetItemProperty.OrderGetColumnValueInt;
                        Pr.SetValues(vals);
                        It.CustomProperty.SetProp(Index2, Pr);
                        SetHeroItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HeroSkillPower（NpcSkillPowerAdd[0..549]） ----------------
            FStatementGetSkillPower.Reset();
            FStatementGetSkillPower.OrderBindParamInt(HeroID);
            if (FStatementGetSkillPower.Query())
            {
                while (FStatementGetSkillPower.Fetch())
                {
                    Index = FStatementGetSkillPower.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 549)
                    {
                        H.NpcSkillPowerAdd[Index].HumanAttackPercent = (short)FStatementGetSkillPower.OrderGetColumnValueInt;
                        H.NpcSkillPowerAdd[Index].HumanAttackValue = (short)FStatementGetSkillPower.OrderGetColumnValueInt;
                        H.NpcSkillPowerAdd[Index].MonAttackPercent = (short)FStatementGetSkillPower.OrderGetColumnValueInt;
                        H.NpcSkillPowerAdd[Index].MonAttackValue = (short)FStatementGetSkillPower.OrderGetColumnValueInt;
                        H.NpcSkillPowerAdd[Index].DefensePercent = (short)FStatementGetSkillPower.OrderGetColumnValueInt;
                        H.NpcSkillPowerAdd[Index].DefenseValue = (short)FStatementGetSkillPower.OrderGetColumnValueInt;
                        H.NpcSkillPowerAdd[Index].RemainingTime = (ushort)FStatementGetSkillPower.OrderGetColumnValueInt;
                    }
                }
            }

            HeroData = H;
            return true;
        }
        finally
        {
            ResetAllGetDataStatement();
        }
    }

    // ==========================================================================================
    // 写（MySqlRoleDB.pas:4583-5817）
    // ==========================================================================================

    /// <summary>
    /// MySqlRoleDB.pas:4583-4630 `DoAdd`。
    /// 先读 Human 的 HeroName/DeputyHeroName：IsDeputyHero=false 覆盖 HeroName，true 覆盖 DeputyHeroName；
    /// AddHero 成功后再 `update Human set HeroName=?, DeputyHeroName=? where HumanID=?`。
    /// </summary>
    protected override bool DoAdd(string Account, string HumanName, int HumanID, string HeroName, byte Sex, byte Job, byte Hair, bool IsDeputyHero)
    {
        bool Result = false;
        Owner_HumanDB_GetHumanHeroName(Account, HumanName, out string HumanHeroName, out string HumanDeputyHeroName);
        if (!IsDeputyHero)
            HumanHeroName = HeroName;
        else
            HumanDeputyHeroName = HeroName;

        BeginTransaction();
        try
        {
            FStatementAddHero.Reset();
            try
            {
                FStatementAddHero.OrderBindParamInt(HumanID);                   // 'HumanID, '
                FStatementAddHero.OrderBindParamText(HeroName);                 // 'HeroName, '
                FStatementAddHero.OrderBindParamBool(false);                    // 'IsDelete, '
                FStatementAddHero.OrderBindParamInt(RoleDbDate.Date2MyDate(RoleDbDate.Now()));  // 'CreateDate, '
                FStatementAddHero.OrderBindParamInt(Sex);                       // 'Sex, '
                FStatementAddHero.OrderBindParamInt(Job);                       // 'Job, '
                FStatementAddHero.OrderBindParamInt(Hair);                      // 'Hair) '
                Result = FStatementAddHero.Step();

                if (Result)
                {
                    try
                    {
                        FStatementUpdateHumanHeroName.Reset();
                        FStatementUpdateHumanHeroName.OrderBindParamText(HumanHeroName);
                        FStatementUpdateHumanHeroName.OrderBindParamText(HumanDeputyHeroName);
                        FStatementUpdateHumanHeroName.OrderBindParamInt(HumanID);
                        FStatementUpdateHumanHeroName.Step();
                    }
                    finally
                    {
                        FStatementUpdateHumanHeroName.Reset();
                    }
                }
                Commit();
            }
            finally
            {
                FStatementAddHero.Reset();
            }
        }
        catch (Exception E)
        {
            RollBack();
            ReportException(E);
        }
        return Result;
    }

    /// <summary>
    /// 原文 `Owner.HumanDB.GetHumanHeroName(Account, HumanName, HumanHeroName, HumanDeputyHeroName)`
    /// —— 走 HumanDB 的公开包装（即"Account 形参被忽略、只按 HumanName 查"的那条）。
    /// </summary>
    private void Owner_HumanDB_GetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName)
    {
        if (FOwner.HumanDB is not null)
        {
            FOwner.HumanDB.GetHumanHeroName(Account, HumanName, out HeroName, out DeputyHeroName);
            return;
        }
        HeroName = "";
        DeputyHeroName = "";
    }

    /// <summary>
    /// MySqlRoleDB.pas:4659-4763 `DoErase`。
    /// `SameText` 命中 HeroName 则清 HeroName/IsStorageHero；否则命中 DeputyHeroName 则清 DeputyHeroName/IsStorageDeputyHero；
    /// 两者都不命中 → IsChanged=False，**不改 Human 行**。注意原文先做文本比较、再删数据。
    /// </summary>
    protected override bool DoErase(string HeroName)
    {
        bool Result = false;

        int HeroID = GetID(HeroName);
        if (HeroID == RoleDbConst.NO_ID)
            return false;

        int HumanID = RoleDbConst.NO_ID;
        string HumanName = "";
        string HumanHeroName = "";
        string HumanDeputyHeroName = "";
        bool IsStorageHero = false;
        bool IsStorageDeputyHero = false;

        Result = false;
        try
        {
            FStatementGetHumanInfo2.Reset();
            FStatementGetHumanInfo2.OrderBindParamInt(HeroID);
            if (FStatementGetHumanInfo2.Query() && FStatementGetHumanInfo2.Fetch())
            {
                Result = true;
                HumanID = FStatementGetHumanInfo2.OrderGetColumnValueInt;
                HumanName = FStatementGetHumanInfo2.OrderGetColumnValueText;
                IsStorageHero = FStatementGetHumanInfo2.OrderGetColumnValueBool;
                IsStorageDeputyHero = FStatementGetHumanInfo2.OrderGetColumnValueBool;
                HumanHeroName = FStatementGetHumanInfo2.OrderGetColumnValueText;
                HumanDeputyHeroName = FStatementGetHumanInfo2.OrderGetColumnValueText;
            }
        }
        finally
        {
            FStatementGetHumanInfo2.Reset();
        }

        bool IsChanged = false;
        if (RoleDbSeam.SameText(HumanHeroName, HeroName))
        {
            IsStorageHero = false;
            HumanHeroName = "";
            IsChanged = true;
        }
        else if (RoleDbSeam.SameText(HumanDeputyHeroName, HeroName))
        {
            IsStorageDeputyHero = false;
            HumanDeputyHeroName = "";
            IsChanged = true;
        }

        BeginTransaction();
        try
        {
            Execute(string.Format("delete from HeroItemElementAdd where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemAddDataByte where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemAddDataInt where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemAddDataText where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemFlute where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemProgress where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemProperty where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItemValueAdd where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroItems where HeroID = {0};", HeroID));

            Execute(string.Format("delete from HeroAbil where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroAbilNG where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroAbilNpcAdd where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroAbilWine where HeroID = {0};", HeroID));

            Execute(string.Format("delete from HeroGodBlessState where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroMagic where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroQuestFlag where HeroID = {0};", HeroID));
            Execute(string.Format("delete from HeroStatusTime where HeroID = {0};", HeroID));

            Execute(string.Format("delete from HeroSkillPower where HeroID = {0};", HeroID));

            Execute(string.Format("delete from Hero where HeroID = {0};", HeroID));

            if (HumanID > 0 && IsChanged)
            {
                try
                {
                    FStatementUpdateHumanHeroName2.Reset();
                    FStatementUpdateHumanHeroName2.OrderBindParamBool(false);
                    FStatementUpdateHumanHeroName2.OrderBindParamBool(IsStorageHero);
                    FStatementUpdateHumanHeroName2.OrderBindParamBool(IsStorageDeputyHero);
                    FStatementUpdateHumanHeroName2.OrderBindParamText(HumanHeroName);
                    FStatementUpdateHumanHeroName2.OrderBindParamText(HumanDeputyHeroName);
                    FStatementUpdateHumanHeroName2.OrderBindParamInt(HumanID);
                    FStatementUpdateHumanHeroName2.Step();
                }
                finally
                {
                    FStatementUpdateHumanHeroName2.Reset();
                }
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

    /// <summary>MySqlRoleDB.pas:4765-4779 `DoSave`（异常时 Result 保持 False）。</summary>
    protected override bool DoSave(int HeroID, ref THeroData HeroData)
    {
        bool Result = false;
        BeginTransaction();
        try
        {
            Result = SaveHeroData(HeroID, ref HeroData);
            Commit();
        }
        catch (Exception E)
        {
            RollBack();
            ReportException(E);
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:4781-4852 `DoRename`。
    /// 用 FStatementGetHumanInfo（含 Account 列）先读 5 列，再按 SameText 决定 HumanHeroName / HumanDeputyHeroName 的新值；
    /// update Hero 成功**且** HumanID>0**且**IsChanged 才同步 Human。
    /// </summary>
    protected override bool DoRename(int HeroID, string HeroName, string NewName)
    {
        int HumanID = RoleDbConst.NO_ID;
        string Account = "";
        string HumanName = "";
        string HumanHeroName = "";
        string HumanDeputyHeroName = "";

        bool Result = false;
        try
        {
            FStatementGetHumanInfo.Reset();
            FStatementGetHumanInfo.OrderBindParamInt(HeroID);
            if (FStatementGetHumanInfo.Query() && FStatementGetHumanInfo.Fetch())
            {
                Account = FStatementGetHumanInfo.GetColumnValueText(0);
                HumanID = FStatementGetHumanInfo.GetColumnValueInt(1);
                HumanName = FStatementGetHumanInfo.GetColumnValueText(2);
                HumanHeroName = FStatementGetHumanInfo.GetColumnValueText(3);
                HumanDeputyHeroName = FStatementGetHumanInfo.GetColumnValueText(4);
            }
        }
        finally
        {
            FStatementGetHumanInfo.Reset();
        }

        bool IsChanged = false;
        if (RoleDbSeam.SameText(HumanHeroName, HeroName))
        {
            HumanHeroName = NewName;
            IsChanged = true;
        }
        else if (RoleDbSeam.SameText(HumanDeputyHeroName, HeroName))
        {
            HumanDeputyHeroName = NewName;
            IsChanged = true;
        }

        BeginTransaction();
        try
        {
            try
            {
                FStatementUpdateHeroName.Reset();
                FStatementUpdateHeroName.OrderBindParamText(NewName);
                FStatementUpdateHeroName.OrderBindParamInt(HeroID);
                Result = FStatementUpdateHeroName.Step();

                if (Result && HumanID > 0 && IsChanged)
                {
                    try
                    {
                        FStatementUpdateHumanHeroName.Reset();
                        FStatementUpdateHumanHeroName.OrderBindParamText(HumanHeroName);
                        FStatementUpdateHumanHeroName.OrderBindParamText(HumanDeputyHeroName);
                        FStatementUpdateHumanHeroName.OrderBindParamInt(HumanID);
                        FStatementUpdateHumanHeroName.Step();
                    }
                    finally
                    {
                        FStatementUpdateHumanHeroName.Reset();
                    }
                }
                Commit();
            }
            finally
            {
                FStatementUpdateHeroName.Reset();
            }
        }
        catch (Exception E)
        {
            RollBack();
            ReportException(E);
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:4854-4896 `DoAssess`（英雄评定）。
    /// 条件：Human 行的 (HeroName,DeputyHeroName) 与入参 **(同序或互换)** 都能对上；
    /// 命中则 `update Human set IsFixedHero=1, IsStorageHero=0, IsStorageDeputyHero=0, HeroName=?, DeputyHeroName=?`。
    /// </summary>
    protected override bool DoAssess(int HeroID, string HeroName, string DeputyHeroName)
    {
        int HumanID = RoleDbConst.NO_ID;
        string HumanHeroName = "";
        string HumanDeputyHeroName = "";

        bool Result = false;
        try
        {
            FStatementGetHumanInfo.Reset();
            FStatementGetHumanInfo.OrderBindParamInt(HeroID);
            if (FStatementGetHumanInfo.Query() && FStatementGetHumanInfo.Fetch())
            {
                HumanID = FStatementGetHumanInfo.GetColumnValueInt(1);
                HumanHeroName = FStatementGetHumanInfo.GetColumnValueText(3);
                HumanDeputyHeroName = FStatementGetHumanInfo.GetColumnValueText(4);
            }
        }
        finally
        {
            FStatementGetHumanInfo.Reset();
        }

        if (HumanID > 0)
        {
            if ((RoleDbSeam.SameText(HumanHeroName, HeroName) && RoleDbSeam.SameText(HumanDeputyHeroName, DeputyHeroName))
                || (RoleDbSeam.SameText(HumanHeroName, DeputyHeroName) && RoleDbSeam.SameText(HumanDeputyHeroName, HeroName)))
            {
                try
                {
                    Result = true;
                    FStatementUpdateHumanHeroName2.Reset();
                    FStatementUpdateHumanHeroName2.OrderBindParamBool(true);
                    FStatementUpdateHumanHeroName2.OrderBindParamBool(false);
                    FStatementUpdateHumanHeroName2.OrderBindParamBool(false);
                    FStatementUpdateHumanHeroName2.OrderBindParamText(HeroName);
                    FStatementUpdateHumanHeroName2.OrderBindParamText(DeputyHeroName);
                    FStatementUpdateHumanHero2_Wrap(HumanID);
                }
                finally
                {
                    FStatementUpdateHumanHeroName2.Reset();
                }
            }
        }
        return Result;
    }

    /// <summary>`FStatementUpdateHumanHeroName2.OrderBindParamInt(HumanID); FStatementUpdateHumanHeroName2.Step;`。</summary>
    private void FStatementUpdateHumanHero2_Wrap(int HumanID)
    {
        FStatementUpdateHumanHeroName2.OrderBindParamInt(HumanID);
        FStatementUpdateHumanHeroName2.Step();
    }

    /// <summary>MySqlRoleDB.pas:4898-5211 `SaveHeroData`（结构同 Human 版，字段集更少）。</summary>
    private bool SaveHeroData(int HeroID, ref THeroData HeroData)
    {
        int I, J;
        bool Result = false;
        try
        {
            FStatementUpdateHero.Reset();
            FStatementUpdateHero.OrderBindParamInt(HeroData.btSex);                   // Sex
            FStatementUpdateHero.OrderBindParamInt(HeroData.btJob);                   // Job
            FStatementUpdateHero.OrderBindParamInt(HeroData.btHair);                  // Hair
            FStatementUpdateHero.OrderBindParamInt(HeroData.btStatus);                // Status
            FStatementUpdateHero.OrderBindParamInt(HeroData.btDir);                   // Dir
            FStatementUpdateHero.OrderBindParamInt(HeroData.Abil.Level);              // Level
            FStatementUpdateHero.OrderBindParamInt(HeroData.btReLevel);               // ReLevel
            FStatementUpdateHero.OrderBindParamDouble(HeroData.rLoyalPoint);          // LoyalPoint
            FStatementUpdateHero.OrderBindParamText(HeroData.CurMap);                 // Map
            FStatementUpdateHero.OrderBindParamInt(HeroData.wCurX);                   // X
            FStatementUpdateHero.OrderBindParamInt(HeroData.wCurY);                   // Y
            // OrderBindParamText(HeroData.HomeMap); OrderBindParamInt(wHomeX); OrderBindParamInt(wHomeY);  ← 原文注释掉
            FStatementUpdateHero.OrderBindParamInt(HeroData.btAttackMode);            // AttackMode
            FStatementUpdateHero.OrderBindParamInt(HeroData.Abil.CreditPoint);        // CreditPoint
            FStatementUpdateHero.OrderBindParamInt(HeroData.nPKPoint);                // PKPoint
            FStatementUpdateHero.OrderBindParamInt(HeroData.btIncHealth);             // IncHP
            FStatementUpdateHero.OrderBindParamInt(HeroData.btIncSpell);              // IncMP
            FStatementUpdateHero.OrderBindParamInt(HeroData.btIncHealing);            // IncHP2
            FStatementUpdateHero.OrderBindParamInt(HeroData.btFightZoneDieCount);     // FightZoneDieCount
            FStatementUpdateHero.OrderBindParamDouble(HeroData.dBodyLuck);            // BodyLuck
            FStatementUpdateHero.OrderBindParamInt(HeroData.nHungerStatus);           // HungerStatus
            FStatementUpdateHero.OrderBindParamInt(HeroData.nRevivalTime);            // RevivalTime
            FStatementUpdateHero.OrderBindParamBool(HeroData.boSaveKillMonExpRate != 0);        // IsSaveKillMonExpRate
            FStatementUpdateHero.OrderBindParamInt(HeroData.nKillMonExpRate);         // KillMonExpRate
            FStatementUpdateHero.OrderBindParamInt((int)HeroData.dwKillMonExpRateTime);       // KillMonExpRateTime
            FStatementUpdateHero.OrderBindParamBool(HeroData.boAttackHumSavePowerRate != 0);  // IsSavePowerRate
            FStatementUpdateHero.OrderBindParamInt(HeroData.nAttackHumPowerRate);             // PowerRate
            FStatementUpdateHero.OrderBindParamInt((int)HeroData.dwAttackHumPowerRateTime);   // PowerRateTime
            FStatementUpdateHero.OrderBindParamBool(HeroData.boAttackMonSavePowerRate != 0);  // IsAttackMonSavePowerRate
            FStatementUpdateHero.OrderBindParamInt(HeroData.nAttackMonPowerRate);             // AttackMonPowerRate
            FStatementUpdateHero.OrderBindParamInt((int)HeroData.dwAttackMonPowerRateTime);   // AttackMonPowerRateTime
            FStatementUpdateHero.OrderBindParamBool(HeroData.boSaveKillMonBurstRate != 0);    // IsSaveKillMonBurstRate
            FStatementUpdateHero.OrderBindParamInt(HeroData.nKillMonBurstRate);       // KillMonBurstRate
            FStatementUpdateHero.OrderBindParamInt((int)HeroData.dwKillMonBurstRateTime);     // KillMonBurstRateTime
            FStatementUpdateHero.OrderBindParamInt((int)HeroData.JewelryBoxStatus);   // JewelryBoxStatus
            // 原文 4944-4945 行：`OrderBindParamBool(boShowFashion);` 后面单独一行分号（空语句）——效果相同
            FStatementUpdateHero.OrderBindParamBool(HeroData.boShowFashion != 0);     // IsShowFashion
            FStatementUpdateHero.OrderBindParamBool(HeroData.boShowGodBless != 0);    // IsShowGodBless
            FStatementUpdateHero.OrderBindParamInt(HeroData.nActiveFengHao);          // ActiveFengHao
            FStatementUpdateHero.OrderBindParamInt(HeroID);

            if (!FStatementUpdateHero.Step())
                return false;

            // ---------------- replace HeroAbil（23 参数：HeroID + 22 属性） ----------------
            FStatementInsertAbil.Reset();
            FStatementInsertAbil.OrderBindParamInt(HeroID);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.AC1);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.AC2);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MAC1);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MAC2);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.DC1);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.DC2);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MC1);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MC2);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.SC1);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.SC2);
            FStatementInsertAbil.OrderBindParamInt((int)HeroData.Abil.HP);
            FStatementInsertAbil.OrderBindParamInt((int)HeroData.Abil.MaxHP);
            FStatementInsertAbil.OrderBindParamInt((int)HeroData.Abil.MP);
            FStatementInsertAbil.OrderBindParamInt((int)HeroData.Abil.MaxMP);
            FStatementInsertAbil.OrderBindParamInt((int)HeroData.Abil.Exp);
            FStatementInsertAbil.OrderBindParamInt((int)HeroData.Abil.MaxExp);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.Weight);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxWeight);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.WearWeight);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxWearWeight);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.HandWeight);
            FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxHandWeight);
            FStatementInsertAbil.Step();

            // ---------------- replace HeroAbilNG（47 参数） ----------------
            FStatementInsertAbilNG.Reset();
            FStatementInsertAbilNG.OrderBindParamInt(HeroID);
            FStatementInsertAbilNG.OrderBindParamBool(HeroData.boTrainingNG != 0);
            FStatementInsertAbilNG.OrderBindParamBool(HeroData.boTrainingXF != 0);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.Level);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.NH);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.MaxNH);
            FStatementInsertAbilNG.OrderBindParamInt((int)HeroData.AbilNG.Exp);
            FStatementInsertAbilNG.OrderBindParamInt((int)HeroData.AbilNG.MaxExp);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.ContinuousMagicOrder[0]);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.ContinuousMagicOrder[1]);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.ContinuousMagicOrder[2]);
            FStatementInsertAbilNG.OrderBindParamBool(HeroData.boOpenLastContinuous != 0);
            FStatementInsertAbilNG.OrderBindParamInt(HeroData.btLastContinuousMagicOrder);
            for (I = 0; I <= 4; I++)
            {
                FStatementInsertAbilNG.OrderBindParamInt(HeroData.Meridians[I].Level);
                FStatementInsertAbilNG.OrderBindParamInt(HeroData.Meridians[I].BlastHitRate);
                for (J = 0; J <= 4; J++)
                {
                    FStatementInsertAbilNG.OrderBindParamInt(MySqlItemAccess.GetHeroMeridianAcupoint(ref HeroData, I, J));
                }
            }
            FStatementInsertAbilNG.Step();

            // ---------------- replace HeroAbilWine（10 参数：无 boPleaseDrink） ----------------
            FStatementInsertAbilWine.Reset();
            FStatementInsertAbilWine.OrderBindParamInt(HeroID);
            FStatementInsertAbilWine.OrderBindParamBool(HeroData.boDrinkWineDrunk != 0);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.nDrinkWineQuality);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.nDrinkWineAlcohol);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.Alcohol);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MaxAlcohol);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.WineDrinkValue);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MedicineLevel);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MedicineValue);
            FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MaxMedicineValue);
            FStatementInsertAbilWine.Step();

            Execute("delete from HeroAbilNpcAdd where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroGodBlessState where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroMagic where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroStatusTime where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroQuestFlag where HeroID = " + RoleDbSeam.IntToStr(HeroID));

            Execute("delete from HeroItemElementAdd where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemAddDataByte where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemAddDataInt where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemAddDataText where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemFlute where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemProgress where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemProperty where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroItemValueAdd where HeroID = " + RoleDbSeam.IntToStr(HeroID));
            Execute("delete from HeroSkillPower where HeroID = " + RoleDbSeam.IntToStr(HeroID));

            Execute("delete from HeroItems where HeroID = " + RoleDbSeam.IntToStr(HeroID));

            for (I = 0; I <= 29; I++)   // AddSaveAbil[0..29]
            {
                if (HeroData.AddSaveAbil[I] != 0)
                {
                    FStatementInsertAbilNpcAdd.Reset();
                    FStatementInsertAbilNpcAdd.OrderBindParamInt(HeroID);
                    FStatementInsertAbilNpcAdd.OrderBindParamInt(I);
                    FStatementInsertAbilNpcAdd.OrderBindParamInt(HeroData.AddSaveAbil[I]);
                    FStatementInsertAbilNpcAdd.Step();
                }
            }

            for (I = 0; I <= 11; I++)   // GodBlessItemsState[0..11]
            {
                if (HeroData.GodBlessItemsState[I] != 0)
                {
                    FStatementInsertGodBlessState.Reset();
                    FStatementInsertGodBlessState.OrderBindParamInt(HeroID);
                    FStatementInsertGodBlessState.OrderBindParamInt(I);
                    FStatementInsertGodBlessState.OrderBindParamInt(HeroData.GodBlessItemsState[I]);
                    FStatementInsertGodBlessState.Step();
                }
            }

            for (I = 0; I <= 47; I++)
            {
                var HumMagic = HeroData.Magics[I];
                if (HumMagic.wMagIdx > 0)
                {
                    FStatementInsertMagic.Reset();
                    FStatementInsertMagic.OrderBindParamInt(HeroID);
                    FStatementInsertMagic.OrderBindParamInt(1);
                    FStatementInsertMagic.OrderBindParamInt(I);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
                    FStatementInsertMagic.OrderBindParamInt((int)HumMagic.MagicAttr);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
                    FStatementInsertMagic.OrderBindParamBool(HumMagic.boUsesItemAdd != 0);
                    FStatementInsertMagic.Step();
                }
            }

            for (I = 0; I <= 47; I++)
            {
                var HumMagic = HeroData.NGMagics[I];
                if (HumMagic.wMagIdx > 0)
                {
                    FStatementInsertMagic.Reset();
                    FStatementInsertMagic.OrderBindParamInt(HeroID);
                    FStatementInsertMagic.OrderBindParamInt(2);
                    FStatementInsertMagic.OrderBindParamInt(I);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
                    FStatementInsertMagic.OrderBindParamInt((int)HumMagic.MagicAttr);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.boUsesItemAdd);
                    FStatementInsertMagic.Step();
                }
            }

            for (I = 0; I <= 5; I++)
            {
                var HumMagic = HeroData.ContinuousMagics[I];
                if (HumMagic.wMagIdx > 0)
                {
                    FStatementInsertMagic.Reset();
                    FStatementInsertMagic.OrderBindParamInt(HeroID);
                    FStatementInsertMagic.OrderBindParamInt(3);
                    FStatementInsertMagic.OrderBindParamInt(I);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
                    FStatementInsertMagic.OrderBindParamInt((int)HumMagic.MagicAttr);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.boUsesItemAdd);
                    FStatementInsertMagic.Step();
                }
            }

            for (I = 0; I <= 17; I++)   // wStatusTimeArr[0..17]
            {
                if (HeroData.wStatusTimeArr[I] != 0)
                {
                    FStatementInsertStatusTime.Reset();
                    FStatementInsertStatusTime.OrderBindParamInt(HeroID);
                    FStatementInsertStatusTime.OrderBindParamInt(I);
                    FStatementInsertStatusTime.OrderBindParamInt(HeroData.wStatusTimeArr[I]);
                    FStatementInsertStatusTime.Step();
                }
            }

            for (I = 0; I <= 127; I++)  // QuestFlag[0..127]
            {
                if (HeroData.QuestFlag[I] != 0)
                {
                    FStatementInsertQuestFlag.Reset();
                    FStatementInsertQuestFlag.OrderBindParamInt(HeroID);
                    FStatementInsertQuestFlag.OrderBindParamInt(I);
                    FStatementInsertQuestFlag.OrderBindParamInt(MySqlItemAccess.GetHeroQuestFlag(ref HeroData, I));
                    FStatementInsertQuestFlag.Step();
                }
            }

            // 5 组物品
            for (I = 0; I <= 29; I++)
            {
                var UserItem = HeroData.HumItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHeroItemToDB(in UserItem, HeroID, 1, I);
            }
            for (I = 0; I <= 5; I++)
            {
                var UserItem = HeroData.JewelryBoxItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHeroItemToDB(in UserItem, HeroID, 2, I);
            }
            for (I = 0; I <= 11; I++)
            {
                var UserItem = HeroData.GodBlessItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHeroItemToDB(in UserItem, HeroID, 3, I);
            }
            for (I = 0; I <= 59; I++)
            {
                var UserItem = HeroData.FengHaoItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHeroItemToDB(in UserItem, HeroID, 4, I);
            }
            for (I = 0; I <= 205; I++)
            {
                var UserItem = HeroData.BagItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHeroItemToDB(in UserItem, HeroID, 5, I);
            }

            for (I = 0; I <= 549; I++)  // HeroSkillPower[0..549]
            {
                var Sp = HeroData.NpcSkillPowerAdd[I];
                if (Sp.HumanAttackPercent != 0 || Sp.HumanAttackValue != 0 || Sp.MonAttackPercent != 0
                    || Sp.MonAttackValue != 0 || Sp.DefensePercent != 0 || Sp.DefenseValue != 0)
                {
                    FStatementInsertSkillPower.Reset();
                    FStatementInsertSkillPower.OrderBindParamInt(HeroID);
                    FStatementInsertSkillPower.OrderBindParamInt(I);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.HumanAttackPercent);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.HumanAttackValue);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.MonAttackPercent);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.MonAttackValue);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.DefensePercent);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.DefenseValue);
                    FStatementInsertSkillPower.OrderBindParamInt(Sp.RemainingTime);
                    FStatementInsertSkillPower.Step();
                }
            }

            Result = true;
            return Result;
        }
        finally
        {
            ResetAllSaveDataStatement();
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:5213-5379 `AddHeroItemToDB`。
    /// 与 Human 版的真实差异：`btAddDataByte` 那组绑的是 **btAddDataByte[J] 本身**（Human 版错绑 btNewValue[J]）。
    /// </summary>
    private void AddHeroItemToDB(in TUserItem UserItem, int HeroID, int ItemType, int ItemIndex)
    {
        int J;
        var It = UserItem;

        FStatementInsertItems.Reset();
        FStatementInsertItems.OrderBindParamInt(HeroID);
        FStatementInsertItems.OrderBindParamInt(ItemType);
        FStatementInsertItems.OrderBindParamInt(ItemIndex);
        FStatementInsertItems.OrderBindParamInt(It.MakeIndex);
        FStatementInsertItems.OrderBindParamInt(It.wIndex);
        FStatementInsertItems.OrderBindParamText(It.NameStr);
        FStatementInsertItems.OrderBindParamInt(It.Dura);
        FStatementInsertItems.OrderBindParamInt(It.DuraMax);
        FStatementInsertItems.OrderBindParamInt((int)It.dwHeroM2DressEffect);
        FStatementInsertItems.OrderBindParamInt(It.btUpgradeCount);
        FStatementInsertItems.OrderBindParamBool(It.boStartTime != 0);
        FStatementInsertItems.OrderBindParamInt(It.nLimitTime);
        FStatementInsertItems.OrderBindParamInt(It.btHeroM2Light);
        FStatementInsertItems.OrderBindParamInt(It.btColor);
        FStatementInsertItems.OrderBindParamBool(It.boIsBind != 0);
        FStatementInsertItems.OrderBindParamInt(It.btBindOption);
        FStatementInsertItems.OrderBindParamInt(It.wEffect);
        FStatementInsertItems.OrderBindParamInt(It.wNewLooks);
        FStatementInsertItems.OrderBindParamInt(It.wNewShape);
        FStatementInsertItems.OrderBindParamInt(It.btFluteCount);
        FStatementInsertItems.OrderBindParamText(It.CustomProperty.TextStr);
        FStatementInsertItems.OrderBindParamInt(It.CustomProperty.btTextColor);
        FStatementInsertItems.OrderBindParamInt((int)It.ItemFrom.ItemForm);
        FStatementInsertItems.OrderBindParamText(It.ItemFrom.MapName);
        FStatementInsertItems.OrderBindParamText(It.ItemFrom.MonName);
        FStatementInsertItems.OrderBindParamText(It.ItemFrom.MakerName);
        FStatementInsertItems.OrderBindParamDouble(It.ItemFrom.DateTime);
        FStatementInsertItems.OrderBindParamInt(It.wInsuranceCount);
        FStatementInsertItems.OrderBindParamInt(It.wNewExpand3);
        FStatementInsertItems.OrderBindParamInt(It.wNewExpand4);
        FStatementInsertItems.Step();

        for (J = 0; J <= 13; J++)
        {
            if (It.GetBtValue(J) != 0)
            {
                FStatementInsertItemValueAdd.Reset();
                FStatementInsertItemValueAdd.OrderBindParamInt(HeroID);
                FStatementInsertItemValueAdd.OrderBindParamInt(ItemType);
                FStatementInsertItemValueAdd.OrderBindParamInt(ItemIndex);
                FStatementInsertItemValueAdd.OrderBindParamInt(J);
                FStatementInsertItemValueAdd.OrderBindParamInt(It.GetBtValue(J));
                FStatementInsertItemValueAdd.Step();
            }
        }

        for (J = 0; J <= 29; J++)
        {
            if (It.GetNewValue(J) != 0)
            {
                FStatementInsertItemElementAdd.Reset();
                FStatementInsertItemElementAdd.OrderBindParamInt(HeroID);
                FStatementInsertItemElementAdd.OrderBindParamInt(ItemType);
                FStatementInsertItemElementAdd.OrderBindParamInt(ItemIndex);
                FStatementInsertItemElementAdd.OrderBindParamInt(J);
                FStatementInsertItemElementAdd.OrderBindParamInt(It.GetNewValue(J));
                FStatementInsertItemElementAdd.Step();
            }
        }

        for (J = 0; J <= Grobal2Const.USER_ITEM_ADD_DATA_BYTE_COUNT - 1; J++)
        {
            if (MySqlItemAccess.GetAddDataByte(ref It, J) != 0)
            {
                FStatementInsertItemAddDataByte.Reset();
                FStatementInsertItemAddDataByte.OrderBindParamInt(HeroID);
                FStatementInsertItemAddDataByte.OrderBindParamInt(ItemType);
                FStatementInsertItemAddDataByte.OrderBindParamInt(ItemIndex);
                FStatementInsertItemAddDataByte.OrderBindParamInt(J);
                // 英雄版正确的是 btAddDataByte[J]（Human 版错绑 btNewValue[J]）
                FStatementInsertItemAddDataByte.OrderBindParamInt(MySqlItemAccess.GetAddDataByte(ref It, J));
                FStatementInsertItemAddDataByte.Step();
            }
        }

        for (J = 0; J <= Grobal2Const.USER_ITEM_ADD_DATA_INT_COUNT - 1; J++)
        {
            if (MySqlItemAccess.GetAddDataInt(ref It, J) != 0)
            {
                FStatementInsertItemAddDataInt.Reset();
                FStatementInsertItemAddDataInt.OrderBindParamInt(HeroID);
                FStatementInsertItemAddDataInt.OrderBindParamInt(ItemType);
                FStatementInsertItemAddDataInt.OrderBindParamInt(ItemIndex);
                FStatementInsertItemAddDataInt.OrderBindParamInt(J);
                FStatementInsertItemAddDataInt.OrderBindParamInt(MySqlItemAccess.GetAddDataInt(ref It, J));
                FStatementInsertItemAddDataInt.Step();
            }
        }

        for (J = 0; J <= 1; J++)
        {
            if (It.GetAddDataText(J) != "")
            {
                FStatementInsertItemAddDataText.Reset();
                FStatementInsertItemAddDataText.OrderBindParamInt(HeroID);
                FStatementInsertItemAddDataText.OrderBindParamInt(ItemType);
                FStatementInsertItemAddDataText.OrderBindParamInt(ItemIndex);
                FStatementInsertItemAddDataText.OrderBindParamInt(J);
                FStatementInsertItemAddDataText.OrderBindParamText(It.GetAddDataText(J));
                FStatementInsertItemAddDataText.Step();
            }
        }

        for (J = 0; J <= 7; J++)
        {
            var Flute = It.GetFlute(J);
            if (Flute.GemIndex != 0)
            {
                FStatementInsertItemFlute.Reset();
                FStatementInsertItemFlute.OrderBindParamInt(HeroID);
                FStatementInsertItemFlute.OrderBindParamInt(ItemType);
                FStatementInsertItemFlute.OrderBindParamInt(ItemIndex);
                FStatementInsertItemFlute.OrderBindParamInt(J);
                FStatementInsertItemFlute.OrderBindParamInt(Flute.GemIndex);
                FStatementInsertItemFlute.OrderBindParamInt(Flute.GemCount);
                FStatementInsertItemFlute.Step();
            }
        }

        for (J = 0; J <= 1; J++)
        {
            var P = J == 0 ? It.Progress0 : It.Progress1;
            if (P.boOpen != 0)
            {
                FStatementInsertItemProgress.Reset();
                FStatementInsertItemProgress.OrderBindParamInt(HeroID);
                FStatementInsertItemProgress.OrderBindParamInt(ItemType);
                FStatementInsertItemProgress.OrderBindParamInt(ItemIndex);
                FStatementInsertItemProgress.OrderBindParamInt(J);
                FStatementInsertItemProgress.OrderBindParamBool(P.boOpen != 0);
                FStatementInsertItemProgress.OrderBindParamInt(P.btNameColor);
                FStatementInsertItemProgress.OrderBindParamInt(P.btCount);
                FStatementInsertItemProgress.OrderBindParamInt(P.btShowType);
                FStatementInsertItemProgress.OrderBindParamInt(P.wMax);
                FStatementInsertItemProgress.OrderBindParamInt(P.wValue);
                FStatementInsertItemProgress.OrderBindParamInt(P.wLevel);
                FStatementInsertItemProgress.OrderBindParamText(P.NameStr);
                FStatementInsertItemProgress.Step();
            }
        }

        for (J = 0; J <= 19; J++)
        {
            var Pr = It.CustomProperty.GetProp(J);
            var Vals = Pr.GetValues();
            if (Vals[0] > 0 || Vals[1] > 0 || Vals[2] > 0)
            {
                FStatementInsertItemProperty.Reset();
                FStatementInsertItemProperty.OrderBindParamInt(HeroID);
                FStatementInsertItemProperty.OrderBindParamInt(ItemType);
                FStatementInsertItemProperty.OrderBindParamInt(ItemIndex);
                FStatementInsertItemProperty.OrderBindParamInt(J);
                FStatementInsertItemProperty.OrderBindParamInt(Pr.btColor);
                FStatementInsertItemProperty.OrderBindParamInt(Pr.btBindType);
                FStatementInsertItemProperty.OrderBindParamInt(Pr.btShowFlag);
                FStatementInsertItemProperty.OrderBindParamInt(Pr.btPercent);
                FStatementInsertItemProperty.OrderBindParamInt(Pr.btHintModule);
                FStatementInsertItemProperty.OrderBindParamInt(Vals[0]);
                FStatementInsertItemProperty.OrderBindParamInt(Vals[1]);
                FStatementInsertItemProperty.OrderBindParamInt(Vals[2]);
                FStatementInsertItemProperty.Step();
            }
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:5381-5749 `TMySqlHeroDB.DoGetRankData`。
    /// 与 Human 版的真实差异：**只有 4 个榜**（无 MasterRankList）；
    /// 且每一行都会同时读 HumanName 与 HeroName 两列，Human（子查询）列在前、HeroName 列在后；
    /// 过滤判据用的是 **sHeroName**（不是 HumanName）。
    /// </summary>
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HeroRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList)
    {
        int QueryCount = (int)TopCount + 50;

        HeroRankList.Clear();
        WarriorRankList.Clear();
        WizardRankList.Clear();
        TaoistRankList.Clear();

        if (MinLevel == 0 && MaxLevel == 0)
        {
            try
            {
                RunHeroRankBlock(FStatementGetLevelRankTopCount, HeroRankList, 0, TopCount, QueryCount, -1, -1);
                RunHeroRankBlock(FStatementGetLevelRankTopCount, WarriorRankList, 0, TopCount, QueryCount, -1, -1);
                RunHeroRankBlock(FStatementGetLevelRankTopCount, WizardRankList, 1, TopCount, QueryCount, -1, -1);
                RunHeroRankBlock(FStatementGetLevelRankTopCount, TaoistRankList, 2, TopCount, QueryCount, -1, -1);
            }
            finally
            {
                FStatementGetLevelRankTopCount.Reset();
            }
        }
        else if (TopCount == 0)
        {
            try
            {
                RunHeroRankBlock(FStatementGetLevelRankCheckLevel, HeroRankList, 0, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel);
                RunHeroRankBlock(FStatementGetLevelRankCheckLevel, WarriorRankList, 0, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel);
                RunHeroRankBlock(FStatementGetLevelRankCheckLevel, WizardRankList, 1, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel);
                RunHeroRankBlock(FStatementGetLevelRankCheckLevel, TaoistRankList, 2, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel);
            }
            finally
            {
                FStatementGetLevelRankCheckLevel.Reset();
            }
        }
        else
        {
            try
            {
                RunHeroRankBlock(FStatementGetLevelRankCheckLevelAndCount, HeroRankList, 0, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel);
                RunHeroRankBlock(FStatementGetLevelRankCheckLevelAndCount, WarriorRankList, 0, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel);
                RunHeroRankBlock(FStatementGetLevelRankCheckLevelAndCount, WizardRankList, 1, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel);
                RunHeroRankBlock(FStatementGetLevelRankCheckLevelAndCount, TaoistRankList, 2, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel);
            }
            finally
            {
                FStatementGetLevelRankCheckLevelAndCount.Reset();
            }
        }
    }

    /// <summary>英雄榜单个职业块（列序：HumanName 子查询列 → HeroName → Level）。</summary>
    private void RunHeroRankBlock(IRoleMySqlStatement Stm, TRoleRankList List, int Job, uint TopCount,
        int QueryCount, int MinLevel, int MaxLevel)
    {
        Stm.Reset();
        Stm.OrderBindParamInt(Job);
        Stm.OrderBindParamInt(Job);
        Stm.OrderBindParamInt(Job);
        if (QueryCount >= 0 && MinLevel >= 0) { Stm.OrderBindParamInt(MinLevel); Stm.OrderBindParamInt(MaxLevel); Stm.OrderBindParamInt(QueryCount); }
        else if (MinLevel >= 0) { Stm.OrderBindParamInt(MinLevel); Stm.OrderBindParamInt(MaxLevel); }
        else { Stm.OrderBindParamInt(QueryCount); }

        int Index = 0;
        if (!Stm.Query()) return;
        while (Stm.Fetch())
        {
            string sHumanName = Stm.OrderGetColumnValueText;
            string sHeroName = Stm.OrderGetColumnValueText;
            int Level = Stm.OrderGetColumnValueInt;

            if (TopCount == 0xFFFFFFFF)
            {
                var RankData = new TRoleRankData();
                RankData.RankIndex = Index;
                RankData.HumanName = sHumanName;
                RankData.HeroName = sHeroName;
                RankData.Level = (uint)Level;
                List.Add(RankData);
                Index++;
                continue;
            }

            if (!DBShareSeamFilter.CheckFilterRankingChrName(sHeroName))
            {
                var RankData = new TRoleRankData();
                RankData.RankIndex = Index;
                RankData.HumanName = sHumanName;
                RankData.HeroName = sHeroName;
                RankData.Level = (uint)Level;
                List.Add(RankData);
                Index++;
                if (Index >= TopCount) break;
            }
        }
    }

    /// <summary>MySqlRoleDB.pas:5771-5792 `ResetAllGetDataStatement`（19 条）。</summary>
    private void ResetAllGetDataStatement()
    {
        FStatementGetHero.Reset();
        FStatementGetAbil.Reset();
        FStatementGetAbilNG.Reset();
        FStatementGetAbilWine.Reset();
        FStatementGetAbilNpcAdd.Reset();
        FStatementGetGodBlessState.Reset();
        FStatementGetMagic.Reset();
        FStatementGetStatusTime.Reset();
        FStatementGetQuestFlag.Reset();
        FStatementGetItems.Reset();
        FStatementGetItemValueAdd.Reset();
        FStatementGetItemElementAdd.Reset();
        FStatementGetItemAddDataByte.Reset();
        FStatementGetItemAddDataInt.Reset();
        FStatementGetItemAddDataText.Reset();
        FStatementGetItemFlute.Reset();
        FStatementGetItemProgress.Reset();
        FStatementGetItemProperty.Reset();
        FStatementGetSkillPower.Reset();
    }

    /// <summary>MySqlRoleDB.pas:5794-5817 `ResetAllSaveDataStatement`（17 条）。</summary>
    private void ResetAllSaveDataStatement()
    {
        FStatementUpdateHero.Reset();

        FStatementInsertAbil.Reset();
        FStatementInsertAbilNG.Reset();
        FStatementInsertAbilWine.Reset();

        FStatementInsertAbilNpcAdd.Reset();
        FStatementInsertGodBlessState.Reset();
        FStatementInsertMagic.Reset();
        FStatementInsertStatusTime.Reset();
        FStatementInsertQuestFlag.Reset();
        FStatementInsertItems.Reset();
        FStatementInsertItemValueAdd.Reset();
        FStatementInsertItemElementAdd.Reset();
        FStatementInsertItemAddDataByte.Reset();
        FStatementInsertItemAddDataInt.Reset();
        FStatementInsertItemAddDataText.Reset();
        FStatementInsertItemFlute.Reset();
        FStatementInsertItemProgress.Reset();
        FStatementInsertItemProperty.Reset();
        FStatementInsertSkillPower.Reset();
    }
}
