using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// MySqlRoleDB.pas `TMySqlHumanDB` 的人物写路径（行 1890-3459）：
///   · DoAdd / DoDelete / DoDeleteRestore / DoSetEnabled / DoErase / DoRecordLoginTime /
///     DoSave / DoRename / DoChangedCustomMoney / DoChangedGold
///   · SaveHumanData（行 2241-2748）+ AddHumanItemToDB（行 2750-2916）
///   · DoGetRankData（行 2918-3358）/ DoBuyPlayer / 事务与 Reset 辅助
/// </summary>
public sealed partial class TMySqlHumanDB
{
    // ==========================================================================================
    // 单条写（MySqlRoleDB.pas:1890-2060）
    // ==========================================================================================

    /// <summary>
    /// MySqlRoleDB.pas:1890-1906 `DoAdd`。
    /// 参数顺序：Account, HumanName, IsDelete=False, IsSelect, CreateDate=Date2MyDate(Now), Sex, Job, Hair。
    /// </summary>
    protected override bool DoAdd(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair)
    {
        try
        {
            FStatementAddHuman.Reset();
            FStatementAddHuman.OrderBindParamText(Account);
            FStatementAddHuman.OrderBindParamText(HumanName);
            FStatementAddHuman.OrderBindParamBool(false);
            FStatementAddHuman.OrderBindParamBool(IsSelect);
            FStatementAddHuman.OrderBindParamInt(RoleDbDate.Date2MyDate(RoleDbDate.Now()));
            FStatementAddHuman.OrderBindParamInt(Sex);
            FStatementAddHuman.OrderBindParamInt(Job);
            FStatementAddHuman.OrderBindParamInt(Hair);
            return FStatementAddHuman.Step();
        }
        finally
        {
            FStatementAddHuman.Reset();
        }
    }

    /// <summary>MySqlRoleDB.pas:1908-1919 `DoDelete`（IsDelete = 1）。</summary>
    protected override bool DoDelete(string Account, string HumanName)
    {
        try
        {
            FStatementDeleteOrRestore.Reset();
            FStatementDeleteOrRestore.OrderBindParamInt(1);
            FStatementDeleteOrRestore.OrderBindParamText(Account);
            FStatementDeleteOrRestore.OrderBindParamText(HumanName);
            return FStatementDeleteOrRestore.Step();
        }
        finally
        {
            FStatementDeleteOrRestore.Reset();
        }
    }

    /// <summary>MySqlRoleDB.pas:1921-1932 `DoDeleteRestore`（IsDelete = 0）。</summary>
    protected override bool DoDeleteRestore(string Account, string HumanName)
    {
        try
        {
            FStatementDeleteOrRestore.Reset();
            FStatementDeleteOrRestore.OrderBindParamInt(0);
            FStatementDeleteOrRestore.OrderBindParamText(Account);
            FStatementDeleteOrRestore.OrderBindParamText(HumanName);
            return FStatementDeleteOrRestore.Step();
        }
        finally
        {
            FStatementDeleteOrRestore.Reset();
        }
    }

    /// <summary>MySqlRoleDB.pas:1934-1945 `DoSetEnabled`：Enabled 直接作为 IsDelete 写入（无钳位）。</summary>
    protected override bool DoSetEnabled(string Account, string HumanName, int Enabled)
    {
        try
        {
            FStatementDeleteOrRestore.Reset();
            FStatementDeleteOrRestore.OrderBindParamInt(Enabled);
            FStatementDeleteOrRestore.OrderBindParamText(Account);
            FStatementDeleteOrRestore.OrderBindParamText(HumanName);
            return FStatementDeleteOrRestore.Step();
        }
        finally
        {
            FStatementDeleteOrRestore.Reset();
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:1947-2046 `DoErase`。
    /// 先 GetID（NO_ID → 直接 False），把该人物的全部 HeroID 拼成 "1,2,3" 形式，
    /// 然后在一个事务里删 Hero* 19 张表 + Human* 24 张表，最后 Commit → True。
    /// </summary>
    protected override bool DoErase(string Account, string HumanName)
    {
        bool Result = false;
        int HumanID = GetID(HumanName);
        if (HumanID == RoleDbConst.NO_ID)
            return false;

        var strHeroID = new StringBuilder();
        try
        {
            FStatementGetHumanHeroID.Reset();
            FStatementGetHumanHeroID.OrderBindParamInt(HumanID);
            if (FStatementGetHumanHeroID.Query())
            {
                while (FStatementGetHumanHeroID.Fetch())
                {
                    int HeroID = FStatementGetHumanHeroID.OrderGetColumnValueInt;
                    strHeroID.Append(HeroID).Append(',');
                }
            }
        }
        finally
        {
            FStatementGetHumanHeroID.Reset();
        }

        BeginTransaction();
        try
        {
            string HeroIds = strHeroID.ToString();
            if (HeroIds.Length > 0)
            {
                HeroIds = HeroIds.Substring(0, HeroIds.Length - 1);   // Copy(strHeroID, 1, Length-1)

                Execute(string.Format("delete from HeroItemElementAdd where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItemAddDataByte where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItemAddDataInt where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItemAddDataText where HeroID in ({0});", HeroIds));

                Execute(string.Format("delete from HeroItemFlute where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItemProgress where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItemProperty where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItemValueAdd where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroItems where HeroID in ({0});", HeroIds));

                Execute(string.Format("delete from HeroAbil where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroAbilNG where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroAbilNpcAdd where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroAbilWine where HeroID in ({0});", HeroIds));

                Execute(string.Format("delete from HeroGodBlessState where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroMagic where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroQuestFlag where HeroID in ({0});", HeroIds));
                Execute(string.Format("delete from HeroStatusTime where HeroID in ({0});", HeroIds));

                Execute(string.Format("delete from HeroSkillPower where HeroID in ({0});", HeroIds));

                Execute(string.Format("delete from Hero where HeroID in ({0});", HeroIds));
            }

            Execute(string.Format("delete from HumanItemElementAdd where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanItemAddDataByte where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanItemAddDataInt where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanItemAddDataText where HumanID = {0};", HumanID));

            Execute(string.Format("delete from HumanItemFlute where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanItemProgress where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanItemProperty where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanItems where HumanID = {0};", HumanID));

            Execute(string.Format("delete from HumanAbil where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanAbilNG where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanAbilNpcAdd where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanAbilWine where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanGamePetData where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanGodBlessState where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanMagic where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanMagicUseTick where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanQuestFlag where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanStatusTime where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanVariableT where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanVariableU where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanVariableJ where HumanID = {0};", HumanID));
            Execute(string.Format("delete from HumanVariableZ where HumanID = {0};", HumanID));

            Execute(string.Format("delete from HumanSkillPower where HumanID = {0};", HumanID));

            Execute(string.Format("delete from HumanMoney where HumanID = {0};", HumanID));

            Execute(string.Format("delete from Human where HumanID = {0};", HumanID));

            Commit();
            Result = true;
        }
        catch (Exception E)
        {
            RollBack();
            ReportException(E);
        }
        return Result;
    }

    /// <summary>MySqlRoleDB.pas:2048-2059 `DoRecordLoginTime`（LoginDate = Date2MyDate(Now)）。</summary>
    protected override bool DoRecordLoginTime(string Account, string HumanName)
    {
        try
        {
            FStatementRecordLoginTime.Reset();
            FStatementRecordLoginTime.OrderBindParamInt(RoleDbDate.Date2MyDate(RoleDbDate.Now()));
            FStatementRecordLoginTime.OrderBindParamText(Account);
            FStatementRecordLoginTime.OrderBindParamText(HumanName);
            return FStatementRecordLoginTime.Step();
        }
        finally
        {
            FStatementRecordLoginTime.Reset();
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:2061-2075 `DoSave`。
    /// 原文 `Result := False; BeginTransaction; try Result := SaveHumanData(...); Commit; except RollBack; end`
    /// —— 异常时 Result 保持 False（不是 SaveHumanData 内部的中间值）。
    /// </summary>
    protected override bool DoSave(int HumanID, ref THumData HumData)
    {
        bool Result = false;
        BeginTransaction();
        try
        {
            Result = SaveHumanData(HumanID, ref HumData);
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
    /// MySqlRoleDB.pas:2077-2114 `DoRename`。
    /// 改名成功后**再**同步 DearName / MasterName 两处外键文本；
    /// 注意 `Result` 被 DearName 那一步的 Step 结果覆盖（MasterName 那步的 Step 结果被丢弃）。
    /// </summary>
    protected override bool DoRename(string Account, string HumanName, int HumanID, string NewName)
    {
        bool Result = false;
        BeginTransaction();
        try
        {
            try
            {
                FStatementUpdateHumanName.Reset();
                FStatementUpdateHumanName.OrderBindParamText(NewName);
                FStatementUpdateHumanName.OrderBindParamInt(HumanID);
                Result = FStatementUpdateHumanName.Step();

                if (Result)
                {
                    FStatementUpdateHumanNameDearName.Reset();
                    FStatementUpdateHumanNameDearName.OrderBindParamText(NewName);
                    FStatementUpdateHumanNameDearName.OrderBindParamText(HumanName);
                    Result = FStatementUpdateHumanNameDearName.Step();

                    FStatementUpdateHumanNameMasterName.Reset();
                    FStatementUpdateHumanNameMasterName.OrderBindParamText(NewName);
                    FStatementUpdateHumanNameMasterName.OrderBindParamText(HumanName);
                    FStatementUpdateHumanNameMasterName.Step();
                }

                Commit();
            }
            finally
            {
                FStatementUpdateHumanName.Reset();
                FStatementUpdateHumanNameDearName.Reset();
                FStatementUpdateHumanNameMasterName.Reset();
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
    /// MySqlRoleDB.pas:2116-2151 `DoChangedCustomMoney`。
    /// I64 = nValue + ChangedValue，钳到 [0, High(LongWord)]；ResultValue 先写后更新库。
    /// 原文缺陷（逐字保留）：HumanMoney 里没有该 MoneyName → 整个 if 不进，**Result 从未赋值**（函数返回不确定值）；
    /// C# 按"初值 False"处理，测试里锁定。另：`finally` 里 Reset 的两次都是 FStatementUpdateCustomMoney
    /// （GetCustomMoneyByName 靠它自己那次 Reset 收尾 —— 原文第二处确实写的是 UpdateCustomMoney）。
    /// </summary>
    protected override bool DoChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue)
    {
        bool Result = false;
        ResultValue = 0;
        try
        {
            FStatementGetCustomMoneyByName.Reset();
            FStatementGetCustomMoneyByName.OrderBindParamInt(HumanID);
            FStatementGetCustomMoneyByName.OrderBindParamText(CustomMoneyName);

            if (FStatementGetCustomMoneyByName.Query() && FStatementGetCustomMoneyByName.Fetch())
            {
                uint nValue = (uint)FStatementGetCustomMoneyByName.OrderGetColumnValueInt;

                long I64 = (long)nValue + ChangedValue;
                if (I64 < 0) I64 = 0;
                else if (I64 > uint.MaxValue) I64 = uint.MaxValue;
                nValue = (uint)I64;
                ResultValue = nValue;

                try
                {
                    FStatementUpdateCustomMoney.Reset();
                    FStatementUpdateCustomMoney.OrderBindParamInt((int)nValue);
                    FStatementUpdateCustomMoney.OrderBindParamInt(HumanID);
                    FStatementUpdateCustomMoney.OrderBindParamText(CustomMoneyName);
                    Result = FStatementUpdateCustomMoney.Step();
                }
                finally
                {
                    FStatementUpdateCustomMoney.Reset();
                }
            }
        }
        finally
        {
            FStatementUpdateCustomMoney.Reset();
        }
        return Result;
    }

    /// <summary>
    /// MySqlRoleDB.pas:2153-2239 `DoChangedGold`。
    /// 读 5 个币种 → 按 ChangeType 改其中一个（钳到 [0, High(LongWord)]）→ 一条 update 写回全部 5 个。
    /// 原文缺陷（逐字保留）：ChangeType = cgtCustomMoney（枚举最后一个）时 **case 无匹配分支**，
    /// ResultValue 保持不变，但仍然会执行 UPDATE。
    /// </summary>
    protected override bool DoChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue)
    {
        bool Result = false;
        ResultValue = 0;
        try
        {
            FStatementGetHumanGoldInfo.Reset();
            FStatementGetHumanGoldInfo.OrderBindParamText(HumanName);

            if (FStatementGetHumanGoldInfo.Query() && FStatementGetHumanGoldInfo.Fetch())
            {
                uint Gold = (uint)FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
                uint GameGold = (uint)FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
                uint GamePoint = (uint)FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
                uint GameDiamond = (uint)FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
                uint GameGird = (uint)FStatementGetHumanGoldInfo.OrderGetColumnValueInt;

                switch (ChangeType)
                {
                    case TDBChangeGoldType.cgtGold:
                        Gold = ClampLongWord((long)Gold + ChangedValue);
                        ResultValue = Gold;
                        break;
                    case TDBChangeGoldType.cgtGameGold:
                        GameGold = ClampLongWord((long)GameGold + ChangedValue);
                        ResultValue = GameGold;
                        break;
                    case TDBChangeGoldType.cgtGamePoint:
                        GamePoint = ClampLongWord((long)GamePoint + ChangedValue);
                        ResultValue = GamePoint;
                        break;
                    case TDBChangeGoldType.cgtGameDiamond:
                        GameDiamond = ClampLongWord((long)GameDiamond + ChangedValue);
                        ResultValue = GameDiamond;
                        break;
                    case TDBChangeGoldType.cgtGameGird:
                        GameGird = ClampLongWord((long)GameGird + ChangedValue);
                        ResultValue = GameGird;
                        break;
                    // 原文 case 没有 cgtCustomMoney 分支 → 什么都不改，ResultValue 保持初值
                }

                try
                {
                    FStatementUpdateHumanGoldInfo.Reset();
                    FStatementUpdateHumanGoldInfo.OrderBindParamInt((int)Gold);
                    FStatementUpdateHumanGoldInfo.OrderBindParamInt((int)GameGold);
                    FStatementUpdateHumanGoldInfo.OrderBindParamInt((int)GamePoint);
                    FStatementUpdateHumanGoldInfo.OrderBindParamInt((int)GameDiamond);
                    FStatementUpdateHumanGoldInfo.OrderBindParamInt((int)GameGird);
                    FStatementUpdateHumanGoldInfo.OrderBindParamText(HumanName);
                    Result = FStatementUpdateHumanGoldInfo.Step();
                }
                finally
                {
                    FStatementUpdateHumanGoldInfo.Reset();
                }
            }
        }
        finally
        {
            FStatementGetHumanGoldInfo.Reset();
        }
        return Result;
    }

    /// <summary>原文 `I64 &lt; 0 → 0`；`I64 &gt; High(LongWord) → High(LongWord)`（5 处重复展开）。</summary>
    private static uint ClampLongWord(long I64)
    {
        if (I64 < 0) return 0;
        if (I64 > uint.MaxValue) return uint.MaxValue;
        return (uint)I64;
    }

    /// <summary>
    /// MySqlRoleDB.pas:3360-3371 `DoBuyPlayer`。
    /// 参数顺序：sBuyAccount（新的 Account）、sSellAccount、sSellHumanName；SQL 还带 `IsDelete = 0`。
    /// </summary>
    protected override bool DoBuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName)
    {
        FStatementBuyPlayer.Reset();
        try
        {
            FStatementBuyPlayer.OrderBindParamText(sBuyAccount);
            FStatementBuyPlayer.OrderBindParamText(sSellAccount);
            FStatementBuyPlayer.OrderBindParamText(sSellHumanName);
            return FStatementBuyPlayer.Step();
        }
        finally
        {
            FStatementBuyPlayer.Reset();
        }
    }

    // ==========================================================================================
    // DoSave 主体（MySqlRoleDB.pas:2241-2748）
    // ==========================================================================================

    /// <summary>
    /// MySqlRoleDB.pas:2241-2748 `SaveHumanData`。
    /// 顺序：UPDATE Human（失败即 `Exit`，Result 仍 False）→ replace HumanAbil/AbilNG/AbilWine
    /// → 24 条 delete 清子表 → 逐项 insert 非零槽位 → 物品 7 组 → SkillPower，最后 Result := True。
    /// </summary>
    private bool SaveHumanData(int HumanID, ref THumData HumData)
    {
        int I, J;
        bool Result = false;
        try
        {
            FStatementUpdateHuman.Reset();
            FStatementUpdateHuman.OrderBindParamInt(HumData.btSex);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btJob);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btHair);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btDir);
            FStatementUpdateHuman.OrderBindParamInt(HumData.Abil.Level);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btReLevel);
            FStatementUpdateHuman.OrderBindParamText(HumData.CurMap);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wCurX);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wCurY);
            FStatementUpdateHuman.OrderBindParamText(HumData.HomeMap);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wHomeX);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wHomeY);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btAttackMode);
            FStatementUpdateHuman.OrderBindParamText(HumData.StoragePwd);
            FStatementUpdateHuman.OrderBindParamInt(HumData.Abil.CreditPoint);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.nGold);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.nGameGold);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.nGamePoint);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.nGameDiamond);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.nGameGird);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nGameGoldEx);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nGameGlory);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nPKPoint);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nPayMentPoint);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nMemberType);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nMemberLevel);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boMaster != 0);
            FStatementUpdateHuman.OrderBindParamText(HumData.MasterName);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wMasterCount);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btMarryCount);
            FStatementUpdateHuman.OrderBindParamText(HumData.DearName);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btIncHealth);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btIncSpell);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btIncHealing);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btFightZoneDieCount);
            FStatementUpdateHuman.OrderBindParamDouble(HumData.dBodyLuck);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wContribution);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nHungerStatus);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nKickCount);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boLockLogin != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boAllowGroup != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boAllowGroupReCall != 0);
            FStatementUpdateHuman.OrderBindParamInt(HumData.wGroupRecallTime);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boAllowGuildReCall != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boDisableTrading != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boDisableInviteHorseRiding != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boGameGoldTrading != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boNewServer != 0);

            // FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGlobalMsg);   ← 原文注释掉
            FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGlobalDropItemMsg != 0);   // 过滤掉落提示信息 chongchong 2017-04-16
            FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGlobalCenterMsg != 0);     // 过滤SendCenterMsg chongchong 2017-04-16
            FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGolbalSendMsg != 0);       // 过滤SendMsg全局信息 chongchong 2017-04-16
            // FStatementUpdateHuman.OrderBindParamBool(HumData.boFixedHero);                     ← 原文注释掉
            FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageHero != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageDeputyHero != 0);
            // FStatementUpdateHuman.OrderBindParamText(HumData.HeroName);                        ← 原文注释掉
            // FStatementUpdateHuman.OrderBindParamText(HumData.DeputyHeroName);                  ← 原文注释掉
            FStatementUpdateHuman.OrderBindParamInt(HumData.btDeputyHeroJob);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btNation);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nNationCredit);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nRevivalTime);
            FStatementUpdateHuman.OrderBindParamInt(HumData.dwInfinityStorageExtCount);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boSaveKillMonExpRate != 0);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nKillMonExpRate);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwKillMonExpRateTime);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boAttackHumSavePowerRate != 0);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nAttackHumPowerRate);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwAttackHumPowerRateTime);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boAttackMonSavePowerRate != 0);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nAttackMonPowerRate);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwAttackMonPowerRateTime);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boSaveKillMonBurstRate != 0);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nKillMonBurstRate);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwKillMonBurstRateTime);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwFBCreateTime);
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.JewelryBoxStatus);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boShowFashion != 0);
            FStatementUpdateHuman.OrderBindParamBool(HumData.boShowGodBless != 0);
            FStatementUpdateHuman.OrderBindParamInt(HumData.nActiveFengHao);
            FStatementUpdateHuman.OrderBindParamBool(MySqlItemAccess.GetStorageOpen(ref HumData, 1) != 0);
            FStatementUpdateHuman.OrderBindParamBool(MySqlItemAccess.GetStorageOpen(ref HumData, 2) != 0);
            FStatementUpdateHuman.OrderBindParamBool(MySqlItemAccess.GetStorageOpen(ref HumData, 3) != 0);

            FStatementUpdateHuman.OrderBindParamInt(HumData.btExtBagPageCount);
            FStatementUpdateHuman.OrderBindParamInt(HumData.btExtBagOpenItemCount);
            FStatementUpdateHuman.OrderBindParamInt(HumData.dwAddMaxWeight);

            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwHighLevelKillMonFixExpTimeLeft);

            FStatementUpdateHuman.OrderBindParamText(HumData.MobileNumber);                        // 手机号码
            FStatementUpdateHuman.OrderBindParamBool(HumData.boMobileBind != 0);                   // 是否绑定
            FStatementUpdateHuman.OrderBindParamText(HumData.MobileVerifyCode);                    // 验证码
            FStatementUpdateHuman.OrderBindParamInt((int)HumData.dwMobileSendTick);                // 最后发送时间
            FStatementUpdateHuman.OrderBindParamInt(HumData.nMobileResendCount);                   // 重发验证码次数

            FStatementUpdateHuman.OrderBindParamInt(HumData.nClearDayVarTime);                     // 变量清空时间

            FStatementUpdateHuman.OrderBindParamInt(HumanID);

            if (!FStatementUpdateHuman.Step())
                return false;

            // ---------------- replace HumanAbil（34 参数） ----------------
            FStatementInsertAbil.Reset();
            FStatementInsertAbil.OrderBindParamInt(HumanID);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.AC1);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.AC2);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MAC1);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MAC2);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.DC1);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.DC2);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MC1);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MC2);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.SC1);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.SC2);
            FStatementInsertAbil.OrderBindParamInt((int)HumData.Abil.HP);
            FStatementInsertAbil.OrderBindParamInt((int)HumData.Abil.MaxHP);
            FStatementInsertAbil.OrderBindParamInt((int)HumData.Abil.MP);
            FStatementInsertAbil.OrderBindParamInt((int)HumData.Abil.MaxMP);
            FStatementInsertAbil.OrderBindParamInt((int)HumData.Abil.Exp);
            FStatementInsertAbil.OrderBindParamInt((int)HumData.Abil.MaxExp);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.Weight);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxWeight);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.WearWeight);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxWearWeight);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.HandWeight);
            FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxHandWeight);
            FStatementInsertAbil.OrderBindParamInt(HumData.nBonusPoint);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.DC);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.MC);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.SC);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.AC);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.MAC);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.HP);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.MP);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.Hit);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.Speed);
            FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.X2);
            FStatementInsertAbil.Step();

            // ---------------- replace HumanAbilNG（12 + 5×(2+5) = 47 参数） ----------------
            FStatementInsertAbilNG.Reset();
            FStatementInsertAbilNG.OrderBindParamInt(HumanID);
            FStatementInsertAbilNG.OrderBindParamBool(HumData.boTrainingNG != 0);
            FStatementInsertAbilNG.OrderBindParamBool(HumData.boTrainingXF != 0);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.Level);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.NH);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.MaxNH);
            FStatementInsertAbilNG.OrderBindParamInt((int)HumData.AbilNG.Exp);
            FStatementInsertAbilNG.OrderBindParamInt((int)HumData.AbilNG.MaxExp);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.ContinuousMagicOrder[0]);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.ContinuousMagicOrder[1]);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.ContinuousMagicOrder[2]);
            FStatementInsertAbilNG.OrderBindParamBool(HumData.boOpenLastContinuous != 0);
            FStatementInsertAbilNG.OrderBindParamInt(HumData.btLastContinuousMagicOrder);
            for (I = 0; I <= 4; I++)
            {
                FStatementInsertAbilNG.OrderBindParamInt(HumData.Meridians[I].Level);
                FStatementInsertAbilNG.OrderBindParamInt(HumData.Meridians[I].BlastHitRate);
                for (J = 0; J <= 4; J++)
                {
                    FStatementInsertAbilNG.OrderBindParamInt(MySqlItemAccess.GetMeridianAcupoint(ref HumData, I, J));
                }
            }
            FStatementInsertAbilNG.Step();

            // ---------------- replace HumanAbilWine（11 参数） ----------------
            FStatementInsertAbilWine.Reset();
            FStatementInsertAbilWine.OrderBindParamInt(HumanID);
            FStatementInsertAbilWine.OrderBindParamBool(HumData.boPleaseDrink != 0);
            FStatementInsertAbilWine.OrderBindParamBool(HumData.boDrinkWineDrunk != 0);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.nDrinkWineQuality);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.nDrinkWineAlcohol);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.Alcohol);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MaxAlcohol);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.WineDrinkValue);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MedicineLevel);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MedicineValue);
            FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MaxMedicineValue);
            FStatementInsertAbilWine.Step();

            Execute("delete from HumanAbilNpcAdd where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanGamePetData where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanGodBlessState where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanMagic where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanMagicUseTick where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanStatusTime where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanQuestFlag where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanVariableT where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanVariableU where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanVariableJ where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanVariableZ where HumanID = " + RoleDbSeam.IntToStr(HumanID));

            Execute("delete from HumanItemElementAdd where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanItemAddDataByte where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanItemAddDataInt where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanItemAddDataText where HumanID = " + RoleDbSeam.IntToStr(HumanID));

            Execute("delete from HumanItemFlute where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanItemProgress where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanItemProperty where HumanID = " + RoleDbSeam.IntToStr(HumanID));
            Execute("delete from HumanItemValueAdd where HumanID = " + RoleDbSeam.IntToStr(HumanID));

            Execute("delete from HumanSkillPower where HumanID = " + RoleDbSeam.IntToStr(HumanID));

            Execute("delete from HumanItems where HumanID = " + RoleDbSeam.IntToStr(HumanID));

            Execute("delete from HumanMoney where HumanID = " + RoleDbSeam.IntToStr(HumanID));

            // ---------------- 逐项 insert 非零槽位 ----------------
            for (I = 0; I <= 29; I++)   // AddSaveAbil[0..29]
            {
                if (HumData.AddSaveAbil[I] != 0)
                {
                    FStatementInsertAbilNpcAdd.Reset();
                    FStatementInsertAbilNpcAdd.OrderBindParamInt(HumanID);
                    FStatementInsertAbilNpcAdd.OrderBindParamInt(I);
                    FStatementInsertAbilNpcAdd.OrderBindParamInt(HumData.AddSaveAbil[I]);
                    FStatementInsertAbilNpcAdd.Step();
                }
            }

            for (I = 0; I <= 29; I++)   // GamePetData[0..29]（只写 sName 非空的）
            {
                if (HumData.GamePetData[I].NameStr.Length > 0)
                {
                    FStatementInsertGamePetData.Reset();
                    FStatementInsertGamePetData.OrderBindParamInt(HumanID);
                    FStatementInsertGamePetData.OrderBindParamInt(I);
                    FStatementInsertGamePetData.OrderBindParamText(HumData.GamePetData[I].NameStr);
                    FStatementInsertGamePetData.OrderBindParamInt((int)HumData.GamePetData[I].Level);
                    FStatementInsertGamePetData.OrderBindParamInt((int)HumData.GamePetData[I].HP);
                    FStatementInsertGamePetData.OrderBindParamInt((int)HumData.GamePetData[I].MP);
                    FStatementInsertGamePetData.OrderBindParamInt((int)HumData.GamePetData[I].Exp);
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 0));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 1));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 2));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 3));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 4));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 5));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 6));
                    FStatementInsertGamePetData.OrderBindParamInt(MySqlItemAccess.GetGamePetMagic(ref HumData, I, 7));
                    FStatementInsertGamePetData.Step();
                }
            }

            for (I = 0; I <= 11; I++)   // GodBlessItemsState[0..11]
            {
                if (HumData.GodBlessItemsState[I] != 0)
                {
                    FStatementInsertGodBlessState.Reset();
                    FStatementInsertGodBlessState.OrderBindParamInt(HumanID);
                    FStatementInsertGodBlessState.OrderBindParamInt(I);
                    FStatementInsertGodBlessState.OrderBindParamInt(HumData.GodBlessItemsState[I]);
                    FStatementInsertGodBlessState.Step();
                }
            }

            // 三组魔法：MagicType 1 / 2 / 3（wMagIdx > 0 才写）
            for (I = 0; I <= 47; I++)
            {
                var HumMagic = HumData.Magics[I];
                if (HumMagic.wMagIdx > 0)
                {
                    FStatementInsertMagic.Reset();
                    FStatementInsertMagic.OrderBindParamInt(HumanID);
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
                var HumMagic = HumData.NGMagics[I];
                if (HumMagic.wMagIdx > 0)
                {
                    FStatementInsertMagic.Reset();
                    FStatementInsertMagic.OrderBindParamInt(HumanID);
                    FStatementInsertMagic.OrderBindParamInt(2);
                    FStatementInsertMagic.OrderBindParamInt(I);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
                    FStatementInsertMagic.OrderBindParamInt((int)HumMagic.MagicAttr);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
                    // 原文这一组绑 Int(Integer(boUsesItemAdd))（与上/下两组的 Bool 不同）
                    FStatementInsertMagic.OrderBindParamInt(HumMagic.boUsesItemAdd);
                    FStatementInsertMagic.Step();
                }
            }

            for (I = 0; I <= 5; I++)
            {
                var HumMagic = HumData.ContinuousMagics[I];
                if (HumMagic.wMagIdx > 0)
                {
                    FStatementInsertMagic.Reset();
                    FStatementInsertMagic.OrderBindParamInt(HumanID);
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

            for (I = 0; I <= 299; I++)   // CustomSkillUseTicks[0..299]（列值 +1000）
            {
                if (HumData.CustomSkillUseTicks[I] != 0)
                {
                    FStatementInsertMagicUseTick.Reset();
                    FStatementInsertMagicUseTick.OrderBindParamInt(HumanID);
                    FStatementInsertMagicUseTick.OrderBindParamInt(I + RoleDbSeam.CUSTOM_MAGIC_START_ID);
                    FStatementInsertMagicUseTick.OrderBindParamInt(HumData.CustomSkillUseTicks[I]);
                    FStatementInsertMagicUseTick.Step();
                }
            }

            for (I = 0; I <= 17; I++)    // wStatusTimeArr[0..17]
            {
                if (HumData.wStatusTimeArr[I] != 0)
                {
                    FStatementInsertStatusTime.Reset();
                    FStatementInsertStatusTime.OrderBindParamInt(HumanID);
                    FStatementInsertStatusTime.OrderBindParamInt(I);
                    FStatementInsertStatusTime.OrderBindParamInt(HumData.wStatusTimeArr[I]);
                    FStatementInsertStatusTime.Step();
                }
            }

            for (I = 0; I <= 127; I++)   // QuestFlag[0..127]
            {
                if (HumData.QuestFlag[I] != 0)
                {
                    FStatementInsertQuestFlag.Reset();
                    FStatementInsertQuestFlag.OrderBindParamInt(HumanID);
                    FStatementInsertQuestFlag.OrderBindParamInt(I);
                    FStatementInsertQuestFlag.OrderBindParamInt(MySqlItemAccess.GetQuestFlag(ref HumData, I));
                    FStatementInsertQuestFlag.Step();
                }
            }

            for (I = 0; I <= 499; I++)   // UValues[0..499]
            {
                if (HumData.UValues[I] != 0)
                {
                    FStatementInsertVariableU.Reset();
                    FStatementInsertVariableU.OrderBindParamInt(HumanID);
                    FStatementInsertVariableU.OrderBindParamInt(I);
                    FStatementInsertVariableU.OrderBindParamInt(HumData.UValues[I]);
                    FStatementInsertVariableU.Step();
                }
            }

            for (I = 0; I <= 499; I++)   // TValues[0..499]（非空串才写）
            {
                if (HumData.TValues[I].Value.Length > 0)
                {
                    FStatementInsertVariableT.Reset();
                    FStatementInsertVariableT.OrderBindParamInt(HumanID);
                    FStatementInsertVariableT.OrderBindParamInt(I);
                    FStatementInsertVariableT.OrderBindParamText(HumData.TValues[I].Value);
                    FStatementInsertVariableT.Step();
                }
            }

            for (I = 0; I <= 499; I++)   // JValues[0..499]
            {
                if (HumData.JValues[I] != 0)
                {
                    FStatementInsertVariableJ.Reset();
                    FStatementInsertVariableJ.OrderBindParamInt(HumanID);
                    FStatementInsertVariableJ.OrderBindParamInt(I);
                    FStatementInsertVariableJ.OrderBindParamInt(HumData.JValues[I]);
                    FStatementInsertVariableJ.Step();
                }
            }

            for (I = 0; I <= 499; I++)   // ZValues[0..499]（非空串才写）
            {
                if (HumData.ZValues[I].Value.Length > 0)
                {
                    FStatementInsertVariableZ.Reset();
                    FStatementInsertVariableZ.OrderBindParamInt(HumanID);
                    FStatementInsertVariableZ.OrderBindParamInt(I);
                    FStatementInsertVariableZ.OrderBindParamText(HumData.ZValues[I].Value);
                    FStatementInsertVariableZ.Step();
                }
            }

            for (I = 0; I <= 29; I++)    // CustomMoney[0..29]（sName 非空才写）
            {
                if (HumData.CustomMoney[I].NameStr != "")
                {
                    FStatementInsertCustomMoney.Reset();
                    FStatementInsertCustomMoney.OrderBindParamInt(HumanID);
                    FStatementInsertCustomMoney.OrderBindParamText(HumData.CustomMoney[I].NameStr);
                    FStatementInsertCustomMoney.OrderBindParamInt(HumData.CustomMoney[I].nCount);
                    FStatementInsertCustomMoney.Step();
                }
            }

            // ---------------- 7 组物品（MakeIndex>0 且 wIndex>0） ----------------
            for (I = 0; I <= 29; I++)
            {
                var UserItem = HumData.HumItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 1, I);
            }

            for (I = 0; I <= 5; I++)
            {
                var UserItem = HumData.JewelryBoxItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 2, I);
            }

            for (I = 0; I <= 11; I++)
            {
                var UserItem = HumData.GodBlessItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 3, I);
            }

            for (I = 0; I <= 59; I++)
            {
                var UserItem = HumData.FengHaoItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 4, I);
            }

            for (I = 0; I <= 205; I++)
            {
                var UserItem = HumData.BagItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 5, I);
            }

            for (I = 0; I <= 195; I++)
            {
                var UserItem = HumData.StorageItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 6, I);
            }

            for (I = 0; I <= 29; I++)
            {
                var UserItem = HumData.GamePetBagItems[I];
                if (UserItem.MakeIndex > 0 && UserItem.wIndex > 0)
                    AddHumanItemToDB(in UserItem, HumanID, 7, I);
            }

            // ---------------- HumanSkillPower[0..549]（6 个数值任一非零才写） ----------------
            // 原文只判 6 个数值字段，**不含 RemainingTime** —— 只填 RemainingTime 不会落库
            // （它只是被一起写出的第 9 个参数）。测试 HumanSave_SkillPowerNeedsOneNonZeroNumeric 锁定此语义。
            for (I = 0; I <= 549; I++)
            {
                var Sp = HumData.NpcSkillPowerAdd[I];
                if (Sp.HumanAttackPercent != 0 || Sp.HumanAttackValue != 0 || Sp.MonAttackPercent != 0
                    || Sp.MonAttackValue != 0 || Sp.DefensePercent != 0 || Sp.DefenseValue != 0)
                {
                    FStatementInsertSkillPower.Reset();
                    FStatementInsertSkillPower.OrderBindParamInt(HumanID);
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
    /// MySqlRoleDB.pas:2750-2916 `AddHumanItemToDB`：HumanItems 主行 + 7 组附加子表。
    /// 原文缺陷（逐字保留）：`btAddDataByte` 那组绑的是 **UserItem.btNewValue[J]**（不是 btAddDataByte[J]）。
    /// </summary>
    private void AddHumanItemToDB(in TUserItem UserItem, int HumanID, int ItemType, int ItemIndex)
    {
        int J;
        var It = UserItem;

        FStatementInsertItems.Reset();
        FStatementInsertItems.OrderBindParamInt(HumanID);
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

        for (J = 0; J <= 13; J++)   // btValue[0..13]
        {
            if (It.GetBtValue(J) != 0)
            {
                FStatementInsertItemValueAdd.Reset();
                FStatementInsertItemValueAdd.OrderBindParamInt(HumanID);
                FStatementInsertItemValueAdd.OrderBindParamInt(ItemType);
                FStatementInsertItemValueAdd.OrderBindParamInt(ItemIndex);
                FStatementInsertItemValueAdd.OrderBindParamInt(J);
                FStatementInsertItemValueAdd.OrderBindParamInt(It.GetBtValue(J));
                FStatementInsertItemValueAdd.Step();
            }
        }

        for (J = 0; J <= 29; J++)   // btNewValue[0..29]
        {
            if (It.GetNewValue(J) != 0)
            {
                FStatementInsertItemElementAdd.Reset();
                FStatementInsertItemElementAdd.OrderBindParamInt(HumanID);
                FStatementInsertItemElementAdd.OrderBindParamInt(ItemType);
                FStatementInsertItemElementAdd.OrderBindParamInt(ItemIndex);
                FStatementInsertItemElementAdd.OrderBindParamInt(J);
                FStatementInsertItemElementAdd.OrderBindParamInt(It.GetNewValue(J));
                FStatementInsertItemElementAdd.Step();
            }
        }

        for (J = 0; J <= Grobal2Const.USER_ITEM_ADD_DATA_BYTE_COUNT - 1; J++)   // btAddDataByte[0..N]
        {
            if (MySqlItemAccess.GetAddDataByte(ref It, J) != 0)
            {
                FStatementInsertItemAddDataByte.Reset();
                FStatementInsertItemAddDataByte.OrderBindParamInt(HumanID);
                FStatementInsertItemAddDataByte.OrderBindParamInt(ItemType);
                FStatementInsertItemAddDataByte.OrderBindParamInt(ItemIndex);
                FStatementInsertItemAddDataByte.OrderBindParamInt(J);
                // 原文如此：这里绑的是 btNewValue[J]，不是 btAddDataByte[J]（已登记为原文缺陷）
                FStatementInsertItemAddDataByte.OrderBindParamInt(It.GetNewValue(J));
                FStatementInsertItemAddDataByte.Step();
            }
        }

        for (J = 0; J <= Grobal2Const.USER_ITEM_ADD_DATA_INT_COUNT - 1; J++)   // nAddDataInt[0..N]
        {
            if (MySqlItemAccess.GetAddDataInt(ref It, J) != 0)
            {
                FStatementInsertItemAddDataInt.Reset();
                FStatementInsertItemAddDataInt.OrderBindParamInt(HumanID);
                FStatementInsertItemAddDataInt.OrderBindParamInt(ItemType);
                FStatementInsertItemAddDataInt.OrderBindParamInt(ItemIndex);
                FStatementInsertItemAddDataInt.OrderBindParamInt(J);
                FStatementInsertItemAddDataInt.OrderBindParamInt(MySqlItemAccess.GetAddDataInt(ref It, J));
                FStatementInsertItemAddDataInt.Step();
            }
        }

        for (J = 0; J <= 1; J++)    // sAddDataText[0..1]
        {
            if (It.GetAddDataText(J) != "")
            {
                FStatementInsertItemAddDataText.Reset();
                FStatementInsertItemAddDataText.OrderBindParamInt(HumanID);
                FStatementInsertItemAddDataText.OrderBindParamInt(ItemType);
                FStatementInsertItemAddDataText.OrderBindParamInt(ItemIndex);
                FStatementInsertItemAddDataText.OrderBindParamInt(J);
                FStatementInsertItemAddDataText.OrderBindParamText(It.GetAddDataText(J));
                FStatementInsertItemAddDataText.Step();
            }
        }

        for (J = 0; J <= 7; J++)    // Flutes[0..7]（GemIndex != 0 才写）
        {
            var Flute = It.GetFlute(J);
            if (Flute.GemIndex != 0)
            {
                FStatementInsertItemFlute.Reset();
                FStatementInsertItemFlute.OrderBindParamInt(HumanID);
                FStatementInsertItemFlute.OrderBindParamInt(ItemType);
                FStatementInsertItemFlute.OrderBindParamInt(ItemIndex);
                FStatementInsertItemFlute.OrderBindParamInt(J);
                FStatementInsertItemFlute.OrderBindParamInt(Flute.GemIndex);
                FStatementInsertItemFlute.OrderBindParamInt(Flute.GemCount);
                FStatementInsertItemFlute.Step();
            }
        }

        for (J = 0; J <= 1; J++)    // Progress[0..1]（boOpen 才写）
        {
            var P = J == 0 ? It.Progress0 : It.Progress1;
            if (P.boOpen != 0)
            {
                FStatementInsertItemProgress.Reset();
                FStatementInsertItemProgress.OrderBindParamInt(HumanID);
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

        for (J = 0; J <= 19; J++)   // CustomProperty.Properties[0..19]（三个 nValues 任一 >0 才写）
        {
            var Pr = It.CustomProperty.GetProp(J);
            var Vals = Pr.GetValues();
            if (Vals[0] > 0 || Vals[1] > 0 || Vals[2] > 0)
            {
                FStatementInsertItemProperty.Reset();
                FStatementInsertItemProperty.OrderBindParamInt(HumanID);
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

    // ==========================================================================================
    // 排行榜（MySqlRoleDB.pas:2918-3358）
    // ==========================================================================================

    /// <summary>
    /// MySqlRoleDB.pas:2918-3358 `DoGetRankData`。
    /// 三条路径：
    ///   ① MinLevel=0 且 MaxLevel=0 → `*GetLevelRankTopCount`（limit QueryCount = TopCount+50），
    ///      每行都过 CheckFilterRankingChrName，收满 TopCount 条即 Break；
    ///   ② TopCount=0 → `*GetLevelRankCheckLevel`（等级区间、无 limit），**完全不过滤**，全部收集；
    ///   ③ 其它 → `*GetLevelRankCheckLevelAndCount`（区间 + limit），过滤 + 收满 TopCount 即 Break。
    /// MasterCount 榜在 ①②③ 下都另外查一次 *MasterRank*，写的是 RankData.MasterCount。
    /// 四职业块的绑定序列恒为 (Job,Job,Job)，已抽成 RunHumanRankBlock，参数顺序逐字一致。
    /// </summary>
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount,
        TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList,
        TRoleRankList TaoistRankList, TRoleRankList MasterRankList)
    {
        int QueryCount = (int)TopCount + 50;

        HumanRankList.Clear();
        WarriorRankList.Clear();
        WizardRankList.Clear();
        TaoistRankList.Clear();
        MasterRankList.Clear();

        if (MinLevel == 0 && MaxLevel == 0)
        {
            // ---- 路径 ① ----
            try
            {
                RunRankBlock(FStatementGetLevelRankTopCount, HumanRankList, 0, TopCount, QueryCount, -1, -1, false);
                RunRankBlock(FStatementGetLevelRankTopCount, WarriorRankList, 0, TopCount, QueryCount, -1, -1, false);
                RunRankBlock(FStatementGetLevelRankTopCount, WizardRankList, 1, TopCount, QueryCount, -1, -1, false);
                RunRankBlock(FStatementGetLevelRankTopCount, TaoistRankList, 2, TopCount, QueryCount, -1, -1, false);
            }
            finally
            {
                FStatementGetLevelRankTopCount.Reset();
            }

            try
            {
                RunRankBlock(FStatementGetMasterRankTopCount, MasterRankList, 0, TopCount, QueryCount, -1, -1, true);
            }
            finally
            {
                FStatementGetMasterRankTopCount.Reset();
            }
        }
        else if (TopCount == 0)
        {
            // ---- 路径 ② ----
            try
            {
                RunRankBlock(FStatementGetLevelRankCheckLevel, HumanRankList, 0, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel, false);
                RunRankBlock(FStatementGetLevelRankCheckLevel, WarriorRankList, 0, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel, false);
                RunRankBlock(FStatementGetLevelRankCheckLevel, WizardRankList, 1, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel, false);
                RunRankBlock(FStatementGetLevelRankCheckLevel, TaoistRankList, 2, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel, false);
            }
            finally
            {
                FStatementGetLevelRankCheckLevel.Reset();
            }

            try
            {
                RunRankBlock(FStatementGetMasterRankCheckLevel, MasterRankList, 0, 0xFFFFFFFF, -1, (int)MinLevel, (int)MaxLevel, true);
            }
            finally
            {
                FStatementGetMasterRankCheckLevel.Reset();
            }
        }
        else
        {
            // ---- 路径 ③ ----
            try
            {
                RunRankBlock(FStatementGetLevelRankCheckLevelAndCount, HumanRankList, 0, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel, false);
                RunRankBlock(FStatementGetLevelRankCheckLevelAndCount, WarriorRankList, 0, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel, false);
                RunRankBlock(FStatementGetLevelRankCheckLevelAndCount, WizardRankList, 1, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel, false);
                RunRankBlock(FStatementGetLevelRankCheckLevelAndCount, TaoistRankList, 2, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel, false);
            }
            finally
            {
                FStatementGetLevelRankCheckLevelAndCount.Reset();
            }

            try
            {
                RunRankBlock(FStatementGetMasterRankCheckLevelAndCount, MasterRankList, 0, TopCount, QueryCount, (int)MinLevel, (int)MaxLevel, true);
            }
            finally
            {
                FStatementGetMasterRankCheckLevelAndCount.Reset();
            }
        }
    }

    /// <summary>
    /// 单个「职业块」的公共形状（原文在每个职业上重复展开 4 次）：
    ///   Reset → OrderBindParamInt(Job)×3 →（TopCount 变体再绑 QueryCount；CheckLevel 变体绑 MinLevel,MaxLevel；
    ///   CheckLevelAndCount 变体绑 MinLevel,MaxLevel,QueryCount）→ Query → while Fetch { ... }。
    /// <paramref name="MaxCount"/> = 0xFFFFFFFF 表示"收满 TopCount 才 Break"这一支不生效（路径 ②）。
    /// <paramref name="IsMaster"/> 决定写 RankData.Level 还是 RankData.MasterCount。
    /// </summary>
    private void RunRankBlock(IRoleMySqlStatement Stm, TRoleRankList List, int Job, uint TopCount,
        int QueryCount, int MinLevel, int MaxLevel, bool IsMaster)
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
            int Value = Stm.OrderGetColumnValueInt;

            if (TopCount == 0xFFFFFFFF)
            {
                // 路径 ②：无过滤、无 Break
                var RankData = new TRoleRankData();
                RankData.RankIndex = Index;
                RankData.HumanName = sHumanName;
                if (IsMaster) RankData.MasterCount = (uint)Value; else RankData.Level = (uint)Value;
                RankData.HeroName = "";
                List.Add(RankData);
                Index++;
                continue;
            }

            if (!DBShareSeamFilter.CheckFilterRankingChrName(sHumanName))
            {
                var RankData = new TRoleRankData();
                RankData.RankIndex = Index;
                RankData.HumanName = sHumanName;
                if (IsMaster) RankData.MasterCount = (uint)Value; else RankData.Level = (uint)Value;
                RankData.HeroName = "";
                List.Add(RankData);
                Index++;
                if (Index >= TopCount) break;
            }
        }
    }

    // ==========================================================================================
    // Reset 辅助（MySqlRoleDB.pas:3393-3459）
    // ==========================================================================================

    /// <summary>MySqlRoleDB.pas:3393-3421 `ResetAllGetDataStatement`（26 个 Get 语句 Reset）。</summary>
    private void ResetAllGetDataStatement()
    {
        FStatementGetHuman.Reset();
        FStatementGetAbil.Reset();
        FStatementGetAbilNG.Reset();
        FStatementGetAbilWine.Reset();
        FStatementGetAbilNpcAdd.Reset();
        FStatementGetGamePetData.Reset();
        FStatementGetGodBlessState.Reset();
        FStatementGetMagic.Reset();
        FStatementGetMagicUseTick.Reset();
        FStatementGetStatusTime.Reset();
        FStatementGetQuestFlag.Reset();
        FStatementGetVariableU.Reset();
        FStatementGetVariableT.Reset();
        FStatementGetVariableJ.Reset();
        FStatementGetVariableZ.Reset();
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
        FStatementGetCustomMoney.Reset();
    }

    /// <summary>MySqlRoleDB.pas:3423-3459 `ResetAllSaveDataStatement`（注释掉的 UpdateAbil 三条不实现）。</summary>
    private void ResetAllSaveDataStatement()
    {
        FStatementUpdateHuman.Reset();

        FStatementInsertAbil.Reset();
        FStatementInsertAbilNG.Reset();
        FStatementInsertAbilWine.Reset();

        // 原文被 { } 注释掉的 FStatementUpdateAbil/AbilNG/AbilWine（这三条语句根本不创建）

        FStatementInsertAbilNpcAdd.Reset();
        FStatementInsertGamePetData.Reset();
        FStatementInsertGodBlessState.Reset();
        FStatementInsertMagic.Reset();
        FStatementInsertMagicUseTick.Reset();
        FStatementInsertStatusTime.Reset();
        FStatementInsertQuestFlag.Reset();
        FStatementInsertVariableU.Reset();
        FStatementInsertVariableT.Reset();
        FStatementInsertVariableJ.Reset();
        FStatementInsertVariableZ.Reset();
        FStatementInsertCustomMoney.Reset();
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
