using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// MySqlRoleDB.pas `TMySqlHumanDB` 的人物整档读取部分（行 1210-1888）。
///   · DoGet            （Human 主表 + HumanAbil/AbilNG/AbilWine/AbilNpcAdd/GamePetData/GodBlessState/
///                        Magic/MagicUseTick/StatusTime/QuestFlag/VariableU/T/J/Z/Items/ItemValueAdd/
///                        ItemElementAdd/ItemAddDataByte/ItemAddDataInt/ItemAddDataText/ItemFlute/
///                        ItemProgress/ItemProperty/SkillPower/CustomMoney，共 24 段）
///   · GetHumanUserItem （ItemType 1..7 → HumItems/JewelryBoxItems/GodBlessItems/FengHaoItems/BagItems/
///                        StorageItems/GamePetBagItems；其它值 → nil）
///
/// 物品 7 个数组在 Grobal2.Types4.cs 里是 [InlineArray] 展开的一串字段，托管侧无法取地址，
/// 故用 GetHumanItem/SetHumanItem 这一对"整体读、整体写"的辅助访问器还原原文的
/// `UserItem := @HumData.XxxItems[Index]` 语义（每次读写整块结构，与指针语义等价）。
/// </summary>
public sealed partial class TMySqlHumanDB
{
    /// <summary>
    /// MySqlRoleDB.pas:1210-1845 `TMySqlHumanDB.DoGet`。
    /// 原文 `FillChar(HumData, SizeOf(HumData), 0)` 只在主表命中后执行（此处 <c>HumData = default</c>）。
    /// 任何一段子表未命中都不影响 Result（只有主表未命中才 `Exit`）。
    /// </summary>
    protected override bool DoGet(string Account, string HumanName, ref THumData HumData, out int HumanID)
    {
        int I, J, TheType, Index, Index2, Value;
        bool Result = false;
        HumanID = 0;

        // 原文用 PTHumData 指针就地改写；托管侧用局部副本累积，退出时整体回写（语义等价）。
        THumData H = HumData;
        try
        {
            FStatementGetHuman.Reset();
            FStatementGetHuman.OrderBindParamText(Account);
            FStatementGetHuman.OrderBindParamText(HumanName);
            if (FStatementGetHuman.Query() && FStatementGetHuman.Fetch())
            {
                Result = true;
                H = default;                              // FillChar(HumData, SizeOf(HumData), 0)
                HumanID = FStatementGetHuman.OrderGetColumnValueInt;

                H.Account = FStatementGetHuman.OrderGetColumnValueText;
                H.ChrName = FStatementGetHuman.OrderGetColumnValueText;
                H.btSex = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btJob = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btHair = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btDir = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.Abil.Level = FStatementGetHuman.OrderGetColumnValueInt;
                H.btReLevel = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.CurMap = FStatementGetHuman.OrderGetColumnValueText;
                H.wCurX = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.wCurY = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.HomeMap = FStatementGetHuman.OrderGetColumnValueText;
                H.wHomeX = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.wHomeY = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.btAttackMode = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.StoragePwd = FStatementGetHuman.OrderGetColumnValueText;
                H.Abil.CreditPoint = FStatementGetHuman.OrderGetColumnValueInt;
                H.nGold = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.nGameGold = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.nGamePoint = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.nGameDiamond = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.nGameGird = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.nGameGoldEx = FStatementGetHuman.OrderGetColumnValueInt;
                H.nGameGlory = FStatementGetHuman.OrderGetColumnValueInt;
                H.nPKPoint = FStatementGetHuman.OrderGetColumnValueInt;
                H.nPayMentPoint = FStatementGetHuman.OrderGetColumnValueInt;
                H.nMemberType = FStatementGetHuman.OrderGetColumnValueInt;
                H.nMemberLevel = FStatementGetHuman.OrderGetColumnValueInt;
                H.boMaster = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.MasterName = FStatementGetHuman.OrderGetColumnValueText;
                H.wMasterCount = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.btMarryCount = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.DearName = FStatementGetHuman.OrderGetColumnValueText;
                H.btIncHealth = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btIncSpell = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btIncHealing = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btFightZoneDieCount = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.dBodyLuck = FStatementGetHuman.OrderGetColumnValueDouble;
                H.wContribution = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.nHungerStatus = FStatementGetHuman.OrderGetColumnValueInt;
                H.nKickCount = FStatementGetHuman.OrderGetColumnValueInt;
                H.boLockLogin = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boAllowGroup = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boAllowGroupReCall = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.wGroupRecallTime = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.boAllowGuildReCall = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boDisableTrading = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boDisableInviteHorseRiding = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boGameGoldTrading = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boNewServer = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);

                // H.boFilterGlobalMsg := OrderGetColumnValueBool;   ← 原文注释掉，库中无该列
                H.boFilterGlobalDropItemMsg = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);   // 过滤掉落提示信息 chongchong 2017-04-16
                H.boFilterGlobalCenterMsg = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);     // 过滤SendCenterMsg chongchong 2017-04-16
                H.boFilterGolbalSendMsg = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);       // 过滤SendMsg全局信息 chongchong 2017-04-16

                H.boFixedHero = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boStorageHero = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boStorageDeputyHero = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.HeroName = FStatementGetHuman.OrderGetColumnValueText;
                H.DeputyHeroName = FStatementGetHuman.OrderGetColumnValueText;
                H.btDeputyHeroJob = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btNation = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.nNationCredit = FStatementGetHuman.OrderGetColumnValueInt;
                H.nRevivalTime = FStatementGetHuman.OrderGetColumnValueInt;
                H.dwInfinityStorageExtCount = (ushort)FStatementGetHuman.OrderGetColumnValueInt;
                H.boSaveKillMonExpRate = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.nKillMonExpRate = FStatementGetHuman.OrderGetColumnValueInt;
                H.dwKillMonExpRateTime = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.boAttackHumSavePowerRate = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.nAttackHumPowerRate = FStatementGetHuman.OrderGetColumnValueInt;
                H.dwAttackHumPowerRateTime = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.boAttackMonSavePowerRate = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.nAttackMonPowerRate = FStatementGetHuman.OrderGetColumnValueInt;
                H.dwAttackMonPowerRateTime = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.boSaveKillMonBurstRate = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.nKillMonBurstRate = FStatementGetHuman.OrderGetColumnValueInt;
                H.dwKillMonBurstRateTime = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.dwFBCreateTime = (uint)FStatementGetHuman.OrderGetColumnValueInt;
                H.JewelryBoxStatus = (TJewelryBoxStatus)FStatementGetHuman.OrderGetColumnValueInt;
                H.boShowFashion = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.boShowGodBless = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);
                H.nActiveFengHao = (sbyte)FStatementGetHuman.OrderGetColumnValueInt;
                // 原文如此：仓库 1 恒为已开（不读库），只读 IsOpenStorage2 / IsOpenStorage3 两列
                MySqlItemAccess.SetStorageOpen(ref H, 0, 1);
                MySqlItemAccess.SetStorageOpen(ref H, 1, (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0));
                MySqlItemAccess.SetStorageOpen(ref H, 2, (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0));
                MySqlItemAccess.SetStorageOpen(ref H, 3, (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0));

                H.btExtBagPageCount = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.btExtBagOpenItemCount = (byte)FStatementGetHuman.OrderGetColumnValueInt;
                H.dwAddMaxWeight = FStatementGetHuman.OrderGetColumnValueInt;
                H.dwHighLevelKillMonFixExpTimeLeft = (uint)FStatementGetHuman.OrderGetColumnValueInt;

                H.MobileNumber = FStatementGetHuman.OrderGetColumnValueText;                        // 手机号码
                H.boMobileBind = (byte)(FStatementGetHuman.OrderGetColumnValueBool ? 1 : 0);        // 是否绑定
                H.MobileVerifyCode = FStatementGetHuman.OrderGetColumnValueText;                    // 验证码
                H.dwMobileSendTick = (uint)FStatementGetHuman.OrderGetColumnValueInt;               // 最后发送时间
                H.nMobileResendCount = FStatementGetHuman.OrderGetColumnValueInt;                   // 重发验证码次数
                H.nClearDayVarTime = FStatementGetHuman.OrderGetColumnValueInt;                     // 变量清空时间
            }

            if (!Result)
                return false;

            // ---------------- HumanAbil ----------------
            FStatementGetAbil.Reset();
            FStatementGetAbil.OrderBindParamInt(HumanID);
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
                H.nBonusPoint = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.DC = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.MC = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.SC = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.AC = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.MAC = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.HP = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.MP = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.Hit = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.Speed = FStatementGetAbil.OrderGetColumnValueInt;
                H.BonusAbil.X2 = FStatementGetAbil.OrderGetColumnValueInt;
            }

            // ---------------- HumanAbilNG（含 5×5 经脉展开） ----------------
            FStatementGetAbilNG.Reset();
            FStatementGetAbilNG.OrderBindParamInt(HumanID);
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
                        MySqlItemAccess.SetMeridianAcupoint(ref H, I, J, (byte)FStatementGetAbilNG.OrderGetColumnValueInt);
                }
            }

            // ---------------- HumanAbilWine ----------------
            FStatementGetAbilWine.Reset();
            FStatementGetAbilWine.OrderBindParamInt(HumanID);
            if (FStatementGetAbilWine.Query() && FStatementGetAbilWine.Fetch())
            {
                H.boPleaseDrink = (byte)(FStatementGetAbilWine.OrderGetColumnValueBool ? 1 : 0);
                H.boDrinkWineDrunk = (byte)(FStatementGetAbilWine.OrderGetColumnValueBool ? 1 : 0);
                H.nDrinkWineQuality = FStatementGetAbilWine.OrderGetColumnValueInt;
                H.nDrinkWineAlcohol = FStatementGetAbilWine.OrderGetColumnValueInt;
                H.Alcohol.Alcohol = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;
                H.Alcohol.MaxAlcohol = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;
                H.Alcohol.WineDrinkValue = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;
                H.Alcohol.MedicineLevel = FStatementGetAbilWine.OrderGetColumnValueInt;
                H.Alcohol.MedicineValue = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;
                H.Alcohol.MaxMedicineValue = (ushort)FStatementGetAbilWine.OrderGetColumnValueInt;
            }

            // ---------------- HumanAbilNpcAdd（AddSaveAbil[0..29]） ----------------
            FStatementGetAbilNpcAdd.Reset();
            FStatementGetAbilNpcAdd.OrderBindParamInt(HumanID);
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

            // ---------------- HumanGamePetData（GamePetData[0..29]，每只 8 个技能列） ----------------
            FStatementGetGamePetData.Reset();
            FStatementGetGamePetData.OrderBindParamInt(HumanID);
            if (FStatementGetGamePetData.Query())
            {
                while (FStatementGetGamePetData.Fetch())
                {
                    Index = FStatementGetGamePetData.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 29)
                    {
                        H.GamePetData[Index].NameStr = FStatementGetGamePetData.OrderGetColumnValueText;
                        H.GamePetData[Index].Level = (uint)FStatementGetGamePetData.OrderGetColumnValueInt;
                        H.GamePetData[Index].HP = (uint)FStatementGetGamePetData.OrderGetColumnValueInt;
                        H.GamePetData[Index].MP = (uint)FStatementGetGamePetData.OrderGetColumnValueInt;
                        H.GamePetData[Index].Exp = (uint)FStatementGetGamePetData.OrderGetColumnValueInt;
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 0, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 1, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 2, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 3, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 4, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 5, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 6, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                        MySqlItemAccess.SetGamePetMagic(ref H, Index, 7, (ushort)FStatementGetGamePetData.OrderGetColumnValueInt);
                    }
                }
            }

            // ---------------- HumanGodBlessState（GodBlessItemsState[0..11]） ----------------
            FStatementGetGodBlessState.Reset();
            FStatementGetGodBlessState.OrderBindParamInt(HumanID);
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

            // ---------------- HumanMagic（MagicType 1=Magics 2=NGMagics 3=ContinuousMagics） ----------------
            FStatementGetMagic.Reset();
            FStatementGetMagic.OrderBindParamInt(HumanID);
            if (FStatementGetMagic.Query())
            {
                while (FStatementGetMagic.Fetch())
                {
                    TheType = FStatementGetMagic.OrderGetColumnValueInt;
                    Index = FStatementGetMagic.OrderGetColumnValueInt;
                    bool HasMagic = false;
                    switch (TheType)
                    {
                        case 1: HasMagic = Index >= 0 && Index <= 47; break;   // Magics[0..47]
                        case 2: HasMagic = Index >= 0 && Index <= 47; break;   // NGMagics[0..47]
                        case 3: HasMagic = Index >= 0 && Index <= 5; break;    // ContinuousMagics[0..5]
                    }

                    // 原文把 7 列**全部读完**，再判断 Magic <> nil；列游标必须走满，否则后续行错位。
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

            // ---------------- HumanMagicUseTick（列 MagicID = Index + CUSTOM_MAGIC_START_ID） ----------------
            FStatementGetMagicUseTick.Reset();
            FStatementGetMagicUseTick.OrderBindParamInt(HumanID);
            if (FStatementGetMagicUseTick.Query())
            {
                while (FStatementGetMagicUseTick.Fetch())
                {
                    Index = FStatementGetMagicUseTick.OrderGetColumnValueInt - RoleDbSeam.CUSTOM_MAGIC_START_ID;
                    Value = FStatementGetMagicUseTick.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 299)
                        H.CustomSkillUseTicks[Index] = Value;
                }
            }

            // ---------------- HumanStatusTime（wStatusTimeArr[0..17]） ----------------
            FStatementGetStatusTime.Reset();
            FStatementGetStatusTime.OrderBindParamInt(HumanID);
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

            // ---------------- HumanQuestFlag（QuestFlag[0..127]，wire 为 Byte） ----------------
            FStatementGetQuestFlag.Reset();
            FStatementGetQuestFlag.OrderBindParamInt(HumanID);
            if (FStatementGetQuestFlag.Query())
            {
                while (FStatementGetQuestFlag.Fetch())
                {
                    Index = FStatementGetQuestFlag.OrderGetColumnValueInt;
                    Value = FStatementGetQuestFlag.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 127)
                        MySqlItemAccess.SetQuestFlag(ref H, Index, (byte)Value);
                }
            }

            // ---------------- HumanVariableU（UValues[0..499]） ----------------
            FStatementGetVariableU.Reset();
            FStatementGetVariableU.OrderBindParamInt(HumanID);
            if (FStatementGetVariableU.Query())
            {
                while (FStatementGetVariableU.Fetch())
                {
                    Index = FStatementGetVariableU.OrderGetColumnValueInt;
                    Value = FStatementGetVariableU.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 499)
                        H.UValues[Index] = Value;
                }
            }

            // ---------------- HumanVariableT（TValues[0..499]，string[100]） ----------------
            FStatementGetVariableT.Reset();
            FStatementGetVariableT.OrderBindParamInt(HumanID);
            if (FStatementGetVariableT.Query())
            {
                while (FStatementGetVariableT.Fetch())
                {
                    Index = FStatementGetVariableT.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 499)
                        H.TValues[Index].Value = FStatementGetVariableT.OrderGetColumnValueText;
                }
            }

            // ---------------- HumanVariableJ（JValues[0..499]） ----------------
            FStatementGetVariableJ.Reset();
            FStatementGetVariableJ.OrderBindParamInt(HumanID);
            if (FStatementGetVariableJ.Query())
            {
                while (FStatementGetVariableJ.Fetch())
                {
                    Index = FStatementGetVariableJ.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 499)
                        H.JValues[Index] = FStatementGetVariableJ.OrderGetColumnValueInt;
                }
            }

            // ---------------- HumanVariableZ（ZValues[0..499]，string[100]） ----------------
            FStatementGetVariableZ.Reset();
            FStatementGetVariableZ.OrderBindParamInt(HumanID);
            if (FStatementGetVariableZ.Query())
            {
                while (FStatementGetVariableZ.Fetch())
                {
                    Index = FStatementGetVariableZ.OrderGetColumnValueInt;
                    if (Index >= 0 && Index <= 499)
                        H.ZValues[Index].Value = FStatementGetVariableZ.OrderGetColumnValueText;
                }
            }

            // ---------------- HumanItems（ItemType 1..7） ----------------
            FStatementGetItems.Reset();
            FStatementGetItems.OrderBindParamInt(HumanID);
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
                    if (IsHumanItemSlotValid(TheType, Index))
                        SetHumanItem(ref H, TheType, Index, It);
                }
            }

            // ---------------- HumanItemValueAdd（btValue[0..13]） ----------------
            FStatementGetItemValueAdd.Reset();
            FStatementGetItemValueAdd.OrderBindParamInt(HumanID);
            if (FStatementGetItemValueAdd.Query())
            {
                while (FStatementGetItemValueAdd.Fetch())
                {
                    TheType = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                    Index = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 13)
                    {
                        Value = FStatementGetItemValueAdd.OrderGetColumnValueInt;
                        var It = GetHumanItem(ref H, TheType, Index);
                        It.SetBtValue(Index2, Value);
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemElementAdd（btNewValue[0..29]） ----------------
            FStatementGetItemElementAdd.Reset();
            FStatementGetItemElementAdd.OrderBindParamInt(HumanID);
            if (FStatementGetItemElementAdd.Query())
            {
                while (FStatementGetItemElementAdd.Fetch())
                {
                    TheType = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                    Index = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 29)
                    {
                        Value = FStatementGetItemElementAdd.OrderGetColumnValueInt;
                        var It = GetHumanItem(ref H, TheType, Index);
                        It.SetNewValue(Index2, (ushort)Value);
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemAddDataByte（btAddDataByte[0..N]） ----------------
            FStatementGetItemAddDataByte.Reset();
            FStatementGetItemAddDataByte.OrderBindParamInt(HumanID);
            if (FStatementGetItemAddDataByte.Query())
            {
                while (FStatementGetItemAddDataByte.Fetch())
                {
                    TheType = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    Index = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= Grobal2Const.USER_ITEM_ADD_DATA_BYTE_COUNT - 1)
                    {
                        Value = FStatementGetItemAddDataByte.OrderGetColumnValueInt;
                        var It = GetHumanItem(ref H, TheType, Index);
                        MySqlItemAccess.SetAddDataByte(ref It, Index2, (byte)Value);
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemAddDataInt（nAddDataInt[0..N]） ----------------
            FStatementGetItemAddDataInt.Reset();
            FStatementGetItemAddDataInt.OrderBindParamInt(HumanID);
            if (FStatementGetItemAddDataInt.Query())
            {
                while (FStatementGetItemAddDataInt.Fetch())
                {
                    TheType = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    Index = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= Grobal2Const.USER_ITEM_ADD_DATA_INT_COUNT - 1)
                    {
                        Value = FStatementGetItemAddDataInt.OrderGetColumnValueInt;
                        var It = GetHumanItem(ref H, TheType, Index);
                        MySqlItemAccess.SetAddDataInt(ref It, Index2, Value);
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemAddDataText（sAddDataText[0..1]，string[20]） ----------------
            FStatementGetItemAddDataText.Reset();
            FStatementGetItemAddDataText.OrderBindParamInt(HumanID);
            if (FStatementGetItemAddDataText.Query())
            {
                while (FStatementGetItemAddDataText.Fetch())
                {
                    TheType = FStatementGetItemAddDataText.OrderGetColumnValueInt;
                    Index = FStatementGetItemAddDataText.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemAddDataText.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 1)
                    {
                        string sTemp = FStatementGetItemAddDataText.OrderGetColumnValueText;
                        var It = GetHumanItem(ref H, TheType, Index);
                        It.SetAddDataText(Index2, sTemp);
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemFlute（Flutes[0..7]，GemIndex>0 且 GemCount=0 → 补 1） ----------------
            FStatementGetItemFlute.Reset();
            FStatementGetItemFlute.OrderBindParamInt(HumanID);
            if (FStatementGetItemFlute.Query())
            {
                while (FStatementGetItemFlute.Fetch())
                {
                    TheType = FStatementGetItemFlute.OrderGetColumnValueInt;
                    Index = FStatementGetItemFlute.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemFlute.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 7)
                    {
                        var It = GetHumanItem(ref H, TheType, Index);
                        var Flute = It.GetFlute(Index2);
                        Flute.GemIndex = (ushort)FStatementGetItemFlute.OrderGetColumnValueInt;
                        Flute.GemCount = (ushort)FStatementGetItemFlute.OrderGetColumnValueInt;
                        if (Flute.GemIndex > 0 && Flute.GemCount == 0)
                            Flute.GemCount = 1;
                        It.SetFlute(Index2, Flute);
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemProgress（Progress[0..1]） ----------------
            FStatementGetItemProgress.Reset();
            FStatementGetItemProgress.OrderBindParamInt(HumanID);
            if (FStatementGetItemProgress.Query())
            {
                while (FStatementGetItemProgress.Fetch())
                {
                    TheType = FStatementGetItemProgress.OrderGetColumnValueInt;
                    Index = FStatementGetItemProgress.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemProgress.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 1)
                    {
                        var It = GetHumanItem(ref H, TheType, Index);
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
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanItemProperty（CustomProperty.Properties[0..19]） ----------------
            FStatementGetItemProperty.Reset();
            FStatementGetItemProperty.OrderBindParamInt(HumanID);
            if (FStatementGetItemProperty.Query())
            {
                while (FStatementGetItemProperty.Fetch())
                {
                    TheType = FStatementGetItemProperty.OrderGetColumnValueInt;
                    Index = FStatementGetItemProperty.OrderGetColumnValueInt;
                    Index2 = FStatementGetItemProperty.OrderGetColumnValueInt;
                    if (IsHumanItemSlotValid(TheType, Index) && Index2 >= 0 && Index2 <= 19)
                    {
                        var It = GetHumanItem(ref H, TheType, Index);
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
                        SetHumanItem(ref H, TheType, Index, It);
                    }
                }
            }

            // ---------------- HumanSkillPower（NpcSkillPowerAdd[0..549]） ----------------
            FStatementGetSkillPower.Reset();
            FStatementGetSkillPower.OrderBindParamInt(HumanID);
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

            // ---------------- HumanMoney（CustomMoney[0..29]） ----------------
            FStatementGetCustomMoney.Reset();
            FStatementGetCustomMoney.OrderBindParamInt(HumanID);
            Index = 0;
            if (FStatementGetCustomMoney.Query())
            {
                while (FStatementGetCustomMoney.Fetch())
                {
                    // 原文无边界检查（CustomMoney 是 array[0..29]，DB 多返回一行即越界写）；
                    // 托管侧为避免越界写加 Index<=29 守卫 —— 唯一有意加的保护，已在报告登记。
                    if (Index > 29) break;
                    H.CustomMoney[Index].NameStr = FStatementGetCustomMoney.OrderGetColumnValueText;
                    H.CustomMoney[Index].nCount = FStatementGetCustomMoney.OrderGetColumnValueInt;
                    Index++;
                }
            }

            HumData = H;
            return true;
        }
        finally
        {
            ResetAllGetDataStatement();
        }
    }

    /// <summary>
    /// MySqlRoleDB.pas:1847-1888 `GetHumanUserItem` 的边界部分。
    /// ItemType 1..7 → 7 个物品数组；其它 Type（含 0 与负数）→ nil。
    /// </summary>
    private static bool IsHumanItemSlotValid(int ItemType, int ItemIndex)
    {
        switch (ItemType)
        {
            case 1: return ItemIndex >= 0 && ItemIndex <= 29;    // HumItems[0..29]
            case 2: return ItemIndex >= 0 && ItemIndex <= 5;     // JewelryBoxItems[0..5]
            case 3: return ItemIndex >= 0 && ItemIndex <= 11;    // GodBlessItems[0..11]
            case 4: return ItemIndex >= 0 && ItemIndex <= 59;    // FengHaoItems[0..59]
            case 5: return ItemIndex >= 0 && ItemIndex <= 205;   // BagItems[0..205]
            case 6: return ItemIndex >= 0 && ItemIndex <= 195;   // StorageItems[0..195]
            case 7: return ItemIndex >= 0 && ItemIndex <= 29;    // GamePetBagItems[0..29]
            default: return false;
        }
    }

    /// <summary>`@HumData.XxxItems[Idx]` 的读侧（InlineArray 无法取址，整体拷出）。</summary>
    private static TUserItem GetHumanItem(ref THumData H, int ItemType, int ItemIndex)
    {
        switch (ItemType)
        {
            case 1: return H.HumItems[ItemIndex];
            case 2: return H.JewelryBoxItems[ItemIndex];
            case 3: return H.GodBlessItems[ItemIndex];
            case 4: return H.FengHaoItems[ItemIndex];
            case 5: return H.BagItems[ItemIndex];
            case 6: return H.StorageItems[ItemIndex];
            case 7: return H.GamePetBagItems[ItemIndex];
            default: return default;
        }
    }

    /// <summary>`@HumData.XxxItems[Idx]` 的写侧。</summary>
    private static void SetHumanItem(ref THumData H, int ItemType, int ItemIndex, in TUserItem Value)
    {
        switch (ItemType)
        {
            case 1: H.HumItems[ItemIndex] = Value; break;
            case 2: H.JewelryBoxItems[ItemIndex] = Value; break;
            case 3: H.GodBlessItems[ItemIndex] = Value; break;
            case 4: H.FengHaoItems[ItemIndex] = Value; break;
            case 5: H.BagItems[ItemIndex] = Value; break;
            case 6: H.StorageItems[ItemIndex] = Value; break;
            case 7: H.GamePetBagItems[ItemIndex] = Value; break;
        }
    }
}
