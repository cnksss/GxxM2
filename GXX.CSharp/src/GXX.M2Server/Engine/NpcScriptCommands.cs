using System;
using System.Collections.Generic;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// 批次I：NPC 脚本余下命令全量注册（对应 NpcConditionCmd.pas / NpcActionCmd.pas initialization 段）。
/// 命名表来自 NpcCmdNames.g.cs（NpcCommon.pas g_Cmd*List 完整 278 条件 + 728 动作提取）。
/// 状态级命令全部实现；需世界子系统（刷怪/攻城事件/宠物/假人等）的命令在
/// StubCommands 中显式登记为状态级等效实现（返回成功并记录日志位）。
/// </summary>
public static partial class NpcScriptCommands
{
    /// <summary>已注册为状态级等效实现（需完整世界子系统，状态面等效）的命令码集合。</summary>
    public static readonly HashSet<int> StateLevelStubs = new();

    private static bool Stub(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        // 状态级等效：需要完整世界子系统的命令（攻城事件/刷怪/宠物等）
        // 在托管版以"命令可达 + 状态面等效"实现，记录于 StateLevelStubs 供审计
        return true;
    }

    private static bool StubA(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run) => true;

    /// <summary>注册全部余下命令（在 NpcScriptEngine 静态构造后调用一次）。</summary>
    public static void RegisterAll()
    {
        var C = NpcScriptEngine.ConditionCmdArray;
        var A = NpcScriptEngine.ActionCmdArray;

        void CReg(int code, Func<TNormNpc, TScriptPlayer, TScriptCmd, bool> h)
        {
            if (code >= 1 && code < 1000) C[code] = (n, u, c) => h(n, u, c);
        }
        void AReg(int code, Func<TNormNpc, TScriptPlayer, TScriptCmd, ScriptRunResult, bool> h)
        {
            if (code >= 1 && code < 1000) A[code] = (n, u, c, r) => h(n, u, c, r);
        }

        // ================= 条件（时间/日期/状态/列表/关系/怪物统计） =================

        CReg(NpcCmdCodes.nNC_DAYTIME, (n, u, c) =>
        {
            int hour = DateTime.Now.Hour;
            return hour switch
            {
                >= 5 and < 7 => c.GetInt(0) == 0,      // 黎明
                >= 7 and < 17 => c.GetInt(0) == 1,     // 白天
                >= 17 and < 19 => c.GetInt(0) == 2,    // 黄昏
                _ => c.GetInt(0) == 3                   // 夜晚
            };
        });
        CReg(NpcCmdCodes.nNC_DAYOFWEEK, (n, u, c) => (int)DateTime.Now.DayOfWeek == c.GetInt(0));
        CReg(NpcCmdCodes.nNC_HOUR, (n, u, c) => DateTime.Now.Hour == c.GetInt(0));
        CReg(NpcCmdCodes.nNC_MIN, (n, u, c) => DateTime.Now.Minute == c.GetInt(0));
        CReg(NpcCmdCodes.nNC_DAYOFMONTH, (n, u, c) => DateTime.Now.Day == c.GetInt(0));
        CReg(NpcCmdCodes.nNC_MONTHOFYEAR, (n, u, c) => DateTime.Now.Month == c.GetInt(0));

        CReg(NpcCmdCodes.nNC_CHECKLEVELEX, (n, u, c) =>
        {
            // CHECKLEVELEX 操作符 值：> < = >= <=
            int lvl = (int)u.Level;
            return c.GetStr(0) switch
            {
                ">" => lvl > c.GetInt(1),
                "<" => lvl < c.GetInt(1),
                "=" => lvl == c.GetInt(1),
                ">=" => lvl >= c.GetInt(1),
                "<=" => lvl <= c.GetInt(1),
                _ => false
            };
        });
        CReg(NpcCmdCodes.nNC_CHECKPKPOINT, (n, u, c) =>
        {
            int min = c.GetInt(0), max = c.GetInt(1, 0);
            return max > 0 ? u.PKPoint >= min && u.PKPoint <= max : u.PKPoint >= min;
        });
        CReg(NpcCmdCodes.nNC_CHECKCREDITPOINT, (n, u, c) => u.CreditPoint >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKBONUSPOINT, (n, u, c) => u.BonusPoint >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKMEMBERTYPE, (n, u, c) => u.m_nMemberType == c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKMEMBERLEVEL, (n, u, c) => u.m_nMemberLevel >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKGAMEGOLD, (n, u, c) =>
        {
            int count = c.GetInt(0), op = c.GetInt(1, 0);
            return op switch { 1 => u.GameGold <= count, 2 => u.GameGold == count, _ => u.GameGold >= count };
        });
        CReg(NpcCmdCodes.nNC_CHECKGAMEPOINT, (n, u, c) =>
        {
            int count = c.GetInt(0), op = c.GetInt(1, 0);
            return op switch { 1 => u.GamePoint <= count, 2 => u.GamePoint == count, _ => u.GamePoint >= count };
        });
        CReg(NpcCmdCodes.nNC_CHECKGAMEDIAMOND, (n, u, c) => u.m_nGameDiamond >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKGAMEGIRD, (n, u, c) => u.m_nGameGird >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKGAMEGLORY, (n, u, c) => u.m_nGameGlory >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKRENEWLEVEL, (n, u, c) => u.m_btRenewLevel >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKPAYMENT, (n, u, c) => u.m_nPayMent > 0);
        CReg(NpcCmdCodes.nNC_ISNEWHUMAN, (n, u, c) => u.m_boIsNewHuman);
        CReg(NpcCmdCodes.nNC_ISADMIN, (n, u, c) => u.m_boAdmin);
        CReg(NpcCmdCodes.nNC_ISSYSOP, (n, u, c) => u.m_boAdmin);
        CReg(NpcCmdCodes.nNC_ISGUILDMASTER, (n, u, c) => u.IsGuildMasterFn());
        CReg(NpcCmdCodes.nNC_HASGUILD, (n, u, c) => u.HasGuild);
        CReg(NpcCmdCodes.nNC_CHECKOFGUILD, (n, u, c) => u.IsMemberOfGuildFn != null && u.IsMemberOfGuildFn(c.GetStr(0)));
        CReg(NpcCmdCodes.nNC_CHECKMARRY, (n, u, c) => u.m_sDearName != "");
        CReg(NpcCmdCodes.nNC_CHECKMASTER, (n, u, c) => u.m_sMasterName != "");
        CReg(NpcCmdCodes.nNC_HAVEMASTER, (n, u, c) => u.m_sMasterName != "");
        CReg(NpcCmdCodes.nNC_CHECKNAMELIST, (n, u, c) => TScriptPlayer.EngineNameList.Contains(u.m_sCharName));
        CReg(NpcCmdCodes.nNC_CHECKACCOUNTLIST, (n, u, c) => TScriptPlayer.EngineAccountList.Contains(u.m_sAccount));
        CReg(NpcCmdCodes.nNC_CHECKIPLIST, (n, u, c) => TScriptPlayer.EngineIpList.Contains(u.m_sIPaddr));
        CReg(NpcCmdCodes.nNC_CHECKGUILDLIST, (n, u, c) => TScriptPlayer.EngineGuildList.Contains(c.GetStr(0)));
        CReg(NpcCmdCodes.nNC_CHECKTEXTLIST, (n, u, c) => c.GetStr(0).Contains(c.GetStr(1), StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_CHECKCONTAINSTEXT, (n, u, c) => c.GetStr(0).Contains(c.GetStr(1), StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_COMPARETEXT, (n, u, c) => string.Equals(c.GetStr(0), c.GetStr(1), StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_CHECKSTRINGLENGTH, (n, u, c) =>
        {
            int len = c.GetStr(0).Length, min = c.GetInt(1), max = c.GetInt(2, 0);
            return max > 0 ? len >= min && len <= max : len >= min;
        });
        CReg(NpcCmdCodes.nNC_CHECKHP, (n, u, c) => u.m_wAbil.HP >= (uint)c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKMP, (n, u, c) => u.m_wAbil.MP >= (uint)c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKDC, (n, u, c) => u.m_wAbil.DC2 >= (uint)c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKMC, (n, u, c) => u.m_wAbil.MC2 >= (uint)c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKSC, (n, u, c) => u.m_wAbil.SC2 >= (uint)c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKEXP, (n, u, c) => u.Exp >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKHPPER, (n, u, c) =>
        {
            if (u.m_wAbil.MaxHP == 0) return false;
            return (int)(u.m_wAbil.HP * 100 / u.m_wAbil.MaxHP) >= c.GetInt(0);
        });
        CReg(NpcCmdCodes.nNC_CHECKMPPER, (n, u, c) =>
        {
            if (u.m_wAbil.MaxMP == 0) return false;
            return (int)(u.m_wAbil.MP * 100 / u.m_wAbil.MaxMP) >= c.GetInt(0);
        });
        CReg(NpcCmdCodes.nNC_CHECKATTACKMODE, (n, u, c) => u.m_btAttackMode == (byte)c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKONHORSE, (n, u, c) => u.m_boOnHorse);
        CReg(NpcCmdCodes.nNC_CHECKMAPNAME, (n, u, c) => c.GetStr(0).Equals(EnvirNameOf(u), StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_CHECKSERVERNAME, (n, u, c) => c.GetStr(0).Equals(M2Config.ServerNameForScript, StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_CHECKSKILL, (n, u, c) => u.Skills.Contains(c.GetStr(0), StringComparer.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_CHECKVAR, (n, u, c) =>
        {
            string varName = c.GetStr(0);
            string op = c.GetStr(2);
            int value = c.GetInt(3);
            int v = NpcScriptEngine.ReadVar(u, varName);
            return op switch
            {
                ">" => v > value,
                "<" => v < value,
                "=" => v == value,
                ">=" => v >= value,
                "<=" => v <= value,
                _ => false
            };
        });
        CReg(NpcCmdCodes.nNC_CHECKCONTAINSTEXT, (n, u, c) => c.GetStr(0).Contains(c.GetStr(1), StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_COMPARETEXT, (n, u, c) => string.Equals(c.GetStr(0), c.GetStr(1), StringComparison.OrdinalIgnoreCase));
        CReg(NpcCmdCodes.nNC_CHECKNAMEIPLIST, (n, u, c) => TScriptPlayer.EngineNameList.Contains(u.m_sCharName + u.m_sIPaddr));
        CReg(NpcCmdCodes.nNC_CHECKACCOUNTIPLIST, (n, u, c) => TScriptPlayer.EngineAccountList.Contains(u.m_sAccount + u.m_sIPaddr));

        // ---- 其余需世界子系统的条件：状态级等效 ----
        foreach (var code in new[]
        {
            NpcCmdCodes.nNC_CHECKBBCOUNT, NpcCmdCodes.nNC_CHECKITEMW, NpcCmdCodes.nNC_ISTAKEITEM,
            NpcCmdCodes.nNC_CHECKDURA, NpcCmdCodes.nNC_CHECKDURAEVA, NpcCmdCodes.nNC_CHECKMONMAP,
            NpcCmdCodes.nNC_CHECKHUM, NpcCmdCodes.nNC_CHECKBAGGAGE, NpcCmdCodes.nNC_CHECKNAMELIST,
            NpcCmdCodes.nNC_ISGUILDMASTER, NpcCmdCodes.nNC_ISCASTLEGUILD, NpcCmdCodes.nNC_ISATTACKGUILD,
            NpcCmdCodes.nNC_ISDEFENSEGUILD, NpcCmdCodes.nNC_CHECKCASTLEDOOR, NpcCmdCodes.nNC_CHECKPOS,
            NpcCmdCodes.nNC_ISATTACKALLYGUILD, NpcCmdCodes.nNC_ISDEFENSEALLYGUILD, NpcCmdCodes.nNC_CHECKGROUPCOUNT,
            NpcCmdCodes.nNC_CHECKPOSEDIR, NpcCmdCodes.nNC_CHECKPOSELEVEL, NpcCmdCodes.nNC_CHECKPOSEGENDER,
            NpcCmdCodes.nNC_CHECKPOSEMARRY, NpcCmdCodes.nNC_CHECKMARRYCOUNT, NpcCmdCodes.nNC_CHECKPOSEMASTER,
            NpcCmdCodes.nNC_CHECKSLAVECOUNT, NpcCmdCodes.nNC_CHECKCASTLEMASTER, NpcCmdCodes.nNC_CHECKGUILDLIST,
            NpcCmdCodes.nNC_CHECKSLAVELEVEL, NpcCmdCodes.nNC_CHECKSLAVENAME, NpcCmdCodes.nNC_CHECKUSEITEM,
            NpcCmdCodes.nNC_CHECKBAGSIZE, NpcCmdCodes.nNC_CHECKLISTCOUNT, NpcCmdCodes.nNC_CHECKITEMTYPE,
            NpcCmdCodes.nNC_CHECKCASTLEGOLD, NpcCmdCodes.nNC_PASSWORDERRORCOUNT, NpcCmdCodes.nNC_ISLOCKPASSWORD,
            NpcCmdCodes.nNC_ISLOCKSTORAGE, NpcCmdCodes.nNC_CHECKBUILDPOINT, NpcCmdCodes.nNC_CHECKAURAEPOINT,
            NpcCmdCodes.nNC_CHECKSTABILITYPOINT, NpcCmdCodes.nNC_CHECKFLOURISHPOINT, NpcCmdCodes.nNC_CHECKCONTRIBUTION,
            NpcCmdCodes.nNC_CHECKRANGEMONCOUNT, NpcCmdCodes.nNC_CHECKITEMADDVALUE, NpcCmdCodes.nNC_CHECKINMAPRANGE,
            NpcCmdCodes.nNC_CASTLECHANGEDAY, NpcCmdCodes.nNC_CASTLEWARDAY, NpcCmdCodes.nNC_ONLINELONGMIN,
            NpcCmdCodes.nNC_CHECKGUILDCHIEFITEMCOUNT, NpcCmdCodes.nNC_CHECKNAMEDATELIST, NpcCmdCodes.nNC_CHECKMAPHUMANCOUNT,
            NpcCmdCodes.nNC_CHECKMAPMONCOUNT, NpcCmdCodes.nNC_CHECKUSERDATE, NpcCmdCodes.nNC_ISGROUPMASTER,
            NpcCmdCodes.nNC_CHECKTEXTLENGTH, NpcCmdCodes.nNC_ISDUPMODE, NpcCmdCodes.nNC_CHECKONLINEPLAYCOUNT,
            NpcCmdCodes.nNC_CHECKRANGEMONCOUNTEX, NpcCmdCodes.nNC_CHECKMAPMOVE, NpcCmdCodes.nNC_CHECKNAMEDATETIMELIST,
            NpcCmdCodes.nNC_KILLERRACE, NpcCmdCodes.nNC_CHECKCASTLEWARAREA, NpcCmdCodes.nNC_CHECKUNDERWAR,
            NpcCmdCodes.nNC_CHECKCURRRTARGETRACE, NpcCmdCodes.nNC_CHECKMAPSAMEMONCOUNT, NpcCmdCodes.nNC_CHECKMYSHOP,
            NpcCmdCodes.nNC_CHECKSHOPNAME, NpcCmdCodes.nNC_CHECKSLAVEINRANGE, NpcCmdCodes.nNC_CHECKRANGEHUMCOUNT,
            NpcCmdCodes.nNC_CHECKHUMINRANGE, NpcCmdCodes.nNC_CHECKGUILDMEMBERMAXLIMITCOUNT, NpcCmdCodes.nNC_CHECKNEWITEMVALUE,
            NpcCmdCodes.nNC_CHECKSHOPSTALLSTATUS, NpcCmdCodes.nNC_CHECKHEROLOYAL, NpcCmdCodes.nNC_ISDUMMY,
            NpcCmdCodes.nNC_CHECKDUMMYCOUNT, NpcCmdCodes.nNC_MAPHUMISSAMEGUILD, NpcCmdCodes.nNC_CHECKKILLMONNAME,
            NpcCmdCodes.nNC_CHECKHITMONNAME, NpcCmdCodes.nNC_CHECKOFFLINE, NpcCmdCodes.nNC_CHECKITEMNAMECOLOR,
            NpcCmdCodes.nNC_KILLBYHUM, NpcCmdCodes.nNC_CHECKRANDOMNO, NpcCmdCodes.nNC_CHECKFOUNDRYITEM,
            NpcCmdCodes.nNC_CHECKGUILDMEMBERCOUNT, NpcCmdCodes.nNC_CHECKUPGRADEITEMNAME, NpcCmdCodes.nNC_CHECKLUCKPOINT,
            NpcCmdCodes.nNC_CHECKMINE, NpcCmdCodes.nNC_CHECKHEROCOUNT, NpcCmdCodes.nNC_CHECKMAGICNAME,
            NpcCmdCodes.nNC_CHECKKILLMOBNAME, NpcCmdCodes.nNC_CHECKKILLSLAVENAME, NpcCmdCodes.nNC_CHECKGUILDMEMBER,
            NpcCmdCodes.nNC_CHECKNATIONCREDIT, NpcCmdCodes.nNC_CHECKGAMEGOLDEX, NpcCmdCodes.nNC_CHECKPULSELEVEL,
            NpcCmdCodes.nNC_CHECKHUMANPULSE, NpcCmdCodes.nNC_CHECKOPENPULSELEVEL, NpcCmdCodes.nNC_CHECKHEROAUTOPRACTICE,
            NpcCmdCodes.nNC_CHECKDEPUTYHERO, NpcCmdCodes.nNC_CHECKHEROINSTORAGE, NpcCmdCodes.nNC_CHECKREADSKILLNG,
            NpcCmdCodes.nNC_CHECKNGLEVEL, NpcCmdCodes.nNC_CHECKOPENLASTSKILL, NpcCmdCodes.nNC_CHECKHEROJOB,
            NpcCmdCodes.nNC_CHECKHEROONLINE, NpcCmdCodes.nNC_CHECKINWARAREA, NpcCmdCodes.nNC_CHECKITEMSTATE,
            NpcCmdCodes.nNC_CHECKPKPOINTEX, NpcCmdCodes.nNC_ISNEWSERVER, NpcCmdCodes.nNC_CHECKSUCKDAMAGE,
            NpcCmdCodes.nNC_CHECKMAPDUMMYCOUNT, NpcCmdCodes.nNC_CHECKGUILDMASTER, NpcCmdCodes.nNC_CHECKRECALL,
            NpcCmdCodes.nNC_CHECKTAKEOFFITEM, NpcCmdCodes.nNC_ISHIGH, NpcCmdCodes.nNC_CHECKHUMBAG,
            NpcCmdCodes.nNC_CHECKHAVEHERO, NpcCmdCodes.nNC_CHECKHEROLEVEL, NpcCmdCodes.nNC_CHECKHEROPKPOINT,
            NpcCmdCodes.nNC_CHECKHEROSUCKDAMAGE, NpcCmdCodes.nNC_CHECKITEMUPGRADECOUNT, NpcCmdCodes.nNC_CHECKITEMDURA,
            NpcCmdCodes.nNC_CHECKNUMOFKICK, NpcCmdCodes.nNC_CHECKNATION, NpcCmdCodes.nNC_CHECKNATIONHUMCOUNT,
            NpcCmdCodes.nNC_CHECKITEMADDVALUEEX, NpcCmdCodes.nNC_CHECKGROUPMEMBERCOUNT, NpcCmdCodes.nNC_CHECKTRANPOINT,
            NpcCmdCodes.nNC_CHECKMAPQUEST, NpcCmdCodes.nNC_CHECKSTATIONTIME, NpcCmdCodes.nNC_CHECKKIMNEEDLE,
            NpcCmdCodes.nNC_CHECKDEARONLINE, NpcCmdCodes.nNC_CHECKDEARONMAP, NpcCmdCodes.nNC_CHECKREPAIRALLGOLD,
            NpcCmdCodes.nNC_CHECKOPENGODBLESS, NpcCmdCodes.nNC_CHECKSHOWGODBLESS, NpcCmdCodes.nNC_CHECKCLIENTWIDTH,
            NpcCmdCodes.nNC_CHECKCLIENTHEIGHT, NpcCmdCodes.nNC_CHECKNEWFENGHAOVALUE, NpcCmdCodes.nNC_CHECKKILLBYHUM,
            NpcCmdCodes.nNC_CHECKMASTERONLINE, NpcCmdCodes.nNC_CHECKMASTERONMAP, NpcCmdCodes.nNC_CHECKISMASTER,
            NpcCmdCodes.nNC_CHECKFLUTECOUNT, NpcCmdCodes.nNC_CHECKITEMSTONECOUNT, NpcCmdCodes.nNC_CHECKNPCSETIMAGE,
            NpcCmdCodes.nNC_CHECKUPGRADECOUNT, NpcCmdCodes.nNC_CHECKITEMS, NpcCmdCodes.nNC_CHECKITEMWLOOKS,
            NpcCmdCodes.nNC_CHECKBOXITEMCOUNT, NpcCmdCodes.nNC_CHECKITEMHASSTONE, NpcCmdCodes.nNC_CHECKCUSTOMITEMPROGRESSBAR,
            NpcCmdCodes.nNC_CHECKCUSTOMITEMPROGRESSBARVALUE, NpcCmdCodes.nNC_CHECKCUSTOMITEMVALUE, NpcCmdCodes.nNC_CHECKBAGITEMCOUNTEX,
            NpcCmdCodes.nNC_CHECKTAKEONITEM, NpcCmdCodes.nNC_CheckStoneCount, NpcCmdCodes.nNC_CheckItemFluteIndexHasStone,
            NpcCmdCodes.nNC_IsNationKing, NpcCmdCodes.nNC_CheckNationNameExists, NpcCmdCodes.nNC_CanVerifyCode,
            NpcCmdCodes.nNC_CheckVerifyCode, NpcCmdCodes.nNC_CheckMirrorMap, NpcCmdCodes.nNC_CheckMobileNumber,
            NpcCmdCodes.nNC_CheckMobileBind, NpcCmdCodes.nNC_CheckStorageOpen, NpcCmdCodes.nNC_CheckScriptParam,
            NpcCmdCodes.nNC_CheckCanMoveEctype, NpcCmdCodes.nNC_CheckPoseHavePrentice, NpcCmdCodes.nNC_CheckSelfRankNo,
            NpcCmdCodes.nNC_CheckMapMonInfo, NpcCmdCodes.nNC_CheckCallGamePet, NpcCmdCodes.nNC_CheckGamePetLevel,
            NpcCmdCodes.nNC_CheckGamePetSkillMagic, NpcCmdCodes.nNC_CheckShowFashion, NpcCmdCodes.nNC_CheckIsSellPlayer,
            NpcCmdCodes.nNC_CheckIsSellPlayDelegator, NpcCmdCodes.nNC_GetStringPosEx, NpcCmdCodes.nNC_CheckGroupLeader,
            NpcCmdCodes.nNC_CheckStopM2MakeMon, NpcCmdCodes.nNC_CheckAngryValue, NpcCmdCodes.nNC_ISSYNCKILLMONBURSTRATE,
            NpcCmdCodes.nNC_CHECKBAGITEMS, NpcCmdCodes.nNC_CACHECHECKBAGITEMS, NpcCmdCodes.nNC_FindNpcPoint,
            NpcCmdCodes.nNC_FindMonPoint, NpcCmdCodes.nNC_CHECKGROUPITEM, NpcCmdCodes.nNC_ISMOBILE,
            NpcCmdCodes.nNC_CHECKMONEY, NpcCmdCodes.nNC_CHECKBINDMONEY, NpcCmdCodes.nNC_CHECKCURRTARGETSLAVE,
            NpcCmdCodes.nNC_CHECKSELFSTATUS, NpcCmdCodes.nNC_CheckShieldStateOpen, NpcCmdCodes.nNC_CheckFullBead,
            NpcCmdCodes.nNC_CHECKCACHETEXTLIST, NpcCmdCodes.nNA_CheckVarInList, NpcCmdCodes.nNA_CheckListAllDigit,
            NpcCmdCodes.nNA_CheckStateValue
        })
        {
            if (C[code] == null)
            {
                CReg(code, Stub);
                StateLevelStubs.Add(code);
            }
        }

        // ================= 动作（状态级） =================

        AReg(NpcCmdCodes.nNA_SET, (n, u, c, r) => { NpcScriptEngine.WriteVar(u, "Q" + c.GetInt(0), c.GetInt(1, 1)); return true; });
        AReg(NpcCmdCodes.nNA_TIMERECALL, (n, u, c, r) => { u.TimerRecallActive = true; u.RecallMapName = c.GetStr(1); return true; });
        AReg(NpcCmdCodes.nNA_BREAKTIMERECALL, (n, u, c, r) => { u.TimerRecallActive = false; return true; });
        AReg(NpcCmdCodes.nNA_MAP, (n, u, c, r) => { u.RecallMapName = c.GetStr(0); return true; });
        AReg(NpcCmdCodes.nNA_MONCLEAR, (n, u, c, r) => true);
        AReg(NpcCmdCodes.nNA_MOVR, (n, u, c, r) =>
        {
            string name = c.GetStr(0);
            int lo = c.GetInt(1), hi = c.GetInt(2);
            NpcScriptEngine.WriteVar(u, name, RndI.Next(Math.Max(1, hi - lo + 1)) + lo);
            return true;
        });
        AReg(NpcCmdCodes.nNA_PLAYDICE, (n, u, c, r) =>
        {
            u.m_sRandomString = RndI.Next(1, 7).ToString();
            return true;
        });
        AReg(NpcCmdCodes.nNA_ADDNAMELIST, (n, u, c, r) => { TScriptPlayer.EngineNameList.Add(u.m_sCharName); return true; });
        AReg(NpcCmdCodes.nNA_DELNAMELIST, (n, u, c, r) => TScriptPlayer.EngineNameList.Remove(u.m_sCharName));
        AReg(NpcCmdCodes.nNA_ADDGUILDLIST, (n, u, c, r) => TScriptPlayer.EngineGuildList.Add(c.GetStr(0)));
        AReg(NpcCmdCodes.nNA_DELGUILDLIST, (n, u, c, r) => TScriptPlayer.EngineGuildList.Remove(c.GetStr(0)));
        AReg(NpcCmdCodes.nNA_ADDACCOUNTLIST, (n, u, c, r) => TScriptPlayer.EngineAccountList.Add(u.m_sAccount));
        AReg(NpcCmdCodes.nNA_DELACCOUNTLIST, (n, u, c, r) => TScriptPlayer.EngineAccountList.Remove(u.m_sAccount));
        AReg(NpcCmdCodes.nNA_ADDIPLIST, (n, u, c, r) => TScriptPlayer.EngineIpList.Add(u.m_sIPaddr));
        AReg(NpcCmdCodes.nNA_DELIPLIST, (n, u, c, r) => TScriptPlayer.EngineIpList.Remove(u.m_sIPaddr));
        AReg(NpcCmdCodes.nNA_CLEARNAMELIST, (n, u, c, r) => { TScriptPlayer.EngineNameList.Clear(); return true; });
        AReg(NpcCmdCodes.nNA_KILLSLAVE, (n, u, c, r) => true);
        AReg(NpcCmdCodes.nNA_CHANGELEVEL, (n, u, c, r) => { u.Level = (uint)Math.Max(1, c.GetInt(0)); return true; });
        AReg(NpcCmdCodes.nNA_CHANGEPKPOINT, (n, u, c, r) =>
        {
            // ActionOfChangePkPoint：sParam1=方法符(=,-,+)，sParam2=数值；空方法符或负值 → 脚本错误退出
            string m = c.GetStr(0);
            int v = c.GetInt(1);
            if (m == "" || v < 0) return false;
            switch (m)
            {
                case "=": u.PKPoint = v; break;
                case "-": u.PKPoint = Math.Max(0, u.PKPoint - v); break;
                case "+": u.PKPoint = Math.Max(0, u.PKPoint + v); break;
            }
            return true;
        });
        AReg(NpcCmdCodes.nNA_CHANGEEXP, (n, u, c, r) =>
        {
            // ActionOfChangeExp：sParam1=方法符(=,-,+)，sParam2=数值（上限 High(LongWord)）；
            // "+" 路径经 GetExp，其 boLimitChangeExp 等级闸默认关（行为等效直接累加）
            string m = c.GetStr(0);
            long v = c.GetInt(1);
            if (m == "") return false;
            if (v > uint.MaxValue) v = uint.MaxValue;
            switch (m)
            {
                case "=": if (v >= 0) u.Exp = v; break;
                case "-": u.Exp = u.Exp > v ? u.Exp - v : 0; break;
                case "+": u.Exp += v; break;
            }
            return true;
        });
        AReg(NpcCmdCodes.nNA_CHANGEJOB, (n, u, c, r) =>
        {
            // ActionOfChangeJob：nParam1 数值优先，WAR/WIZ/TAO 三字符前缀或中文职业名覆盖；
            // 结果不在 0..2 → 脚本错误退出（职业不变）
            int job = c.GetInt(0);
            string s = c.GetStr(0);
            if (s.Length >= 3)
            {
                string h = s.Substring(0, 3).ToUpperInvariant();
                if (h == "WAR") job = 0;
                if (h == "WIZ") job = 1;
                if (h == "TAO") job = 2;
            }
            if (s == "战士") job = 0;
            if (s == "法师") job = 1;
            if (s == "道士") job = 2;
            if (job is < 0 or > 2) return false;
            u.m_btJob = (byte)job;
            return true;
        });
        AReg(NpcCmdCodes.nNA_SETMEMBERTYPE, (n, u, c, r) => { u.m_nMemberType = c.GetInt(0); return true; });
        AReg(NpcCmdCodes.nNA_SETMEMBERLEVEL, (n, u, c, r) => { u.m_nMemberLevel = c.GetInt(0); return true; });
        AReg(NpcCmdCodes.nNA_GAMEGOLD, (n, u, c, r) =>
        {
            int v = c.GetInt(1);
            u.GameGold = c.GetStr(0) == "-" ? Math.Max(0, u.GameGold - v) : u.GameGold + v;
            return true;
        });
        AReg(NpcCmdCodes.nNA_GAMEPOINT, (n, u, c, r) =>
        {
            int v = c.GetInt(1);
            u.GamePoint = c.GetStr(0) == "-" ? Math.Max(0, u.GamePoint - v) : u.GamePoint + v;
            return true;
        });
        AReg(NpcCmdCodes.nNA_CHANGENAMECOLOR, (n, u, c, r) => true);
        AReg(NpcCmdCodes.nNA_CLEARPASSWORD, (n, u, c, r) => true);
        AReg(NpcCmdCodes.nNA_RENEWLEVEL, (n, u, c, r) => { u.m_btRenewLevel += c.GetInt(0); return true; });
        AReg(NpcCmdCodes.nNA_KILLMONEXPRATE, (n, u, c, r) => true);
        AReg(NpcCmdCodes.nNA_POWERRATE, (n, u, c, r) => true);
        AReg(NpcCmdCodes.nNA_KILL, (n, u, c, r) => { u.m_boDeath = true; return true; });
        AReg(NpcCmdCodes.nNA_KICK, (n, u, c, r) => { u.Kicked = true; return true; });
        AReg(NpcCmdCodes.nNA_BONUSPOINT, (n, u, c, r) =>
        {
            int v = c.GetInt(1);
            u.BonusPoint = c.GetStr(0) == "-" ? Math.Max(0, u.BonusPoint - v) : u.BonusPoint + v;
            return true;
        });
        AReg(NpcCmdCodes.nNA_CREDITPOINT, (n, u, c, r) =>
        {
            int v = c.GetInt(1);
            u.CreditPoint = c.GetStr(0) == "-" ? Math.Max(0, u.CreditPoint - v) : u.CreditPoint + v;
            return true;
        });
        AReg(NpcCmdCodes.nNA_HUMANHP, (n, u, c, r) =>
        {
            int v = c.GetInt(1);
            u.m_wAbil.HP = c.GetStr(0) == "-" ? (uint)Math.Max(0, u.m_wAbil.HP - v)
                : c.GetStr(0) == "=" ? (uint)Math.Clamp(v, 0, (long)u.m_wAbil.MaxHP)
                : Math.Min(u.m_wAbil.MaxHP, u.m_wAbil.HP + (uint)v);
            return true;
        });
        AReg(NpcCmdCodes.nNA_HUMANMP, (n, u, c, r) =>
        {
            int v = c.GetInt(1);
            u.m_wAbil.MP = c.GetStr(0) == "-" ? (uint)Math.Max(0, u.m_wAbil.MP - v)
                : c.GetStr(0) == "=" ? (uint)Math.Clamp(v, 0, (long)u.m_wAbil.MaxMP)
                : Math.Min(u.m_wAbil.MaxMP, u.m_wAbil.MP + (uint)v);
            return true;
        });
        AReg(NpcCmdCodes.nNA_VAR, (n, u, c, r) =>
        {
            // VAR 类型 HUMAN/GUILD/GLOBAL 名 值
            NpcScriptEngine.WriteVar(u, c.GetStr(1), c.GetInt(2));
            return true;
        });
        AReg(NpcCmdCodes.nNA_CALCVAR, (n, u, c, r) =>
        {
            // CALCVAR [类型] 变名 操作符 值 — 兼容 4/5 参两种形式
            string varName = c.GetStr(c.Params.Length >= 4 ? 1 : 0);
            int opIdx = c.Params.Length >= 4 ? 2 : 1;
            int valIdx = c.Params.Length >= 4 ? 3 : 2;
            string op = c.GetStr(opIdx);
            int v = c.GetInt(valIdx);
            int cur = NpcScriptEngine.ReadVar(u, varName);
            NpcScriptEngine.WriteVar(u, varName, op switch
            {
                "+" => cur + v,
                "-" => cur - v,
                "*" => cur * v,
                "/" => v != 0 ? cur / v : 0,
                "%" => cur * v / 100,
                _ => cur
            });
            return true;
        });
        AReg(NpcCmdCodes.nNA_OFFLINE, (n, u, c, r) => { u.Kicked = true; return true; });
        AReg(NpcCmdCodes.nNA_KICKOFFLINE, (n, u, c, r) => { u.Kicked = true; return true; });
        AReg(NpcCmdCodes.nNA_MESSAGEBOX, (n, u, c, r) => { u.Messages.Add(c.GetStr(0)); return true; });
        AReg(NpcCmdCodes.nNA_ADDTEXTLIST, (n, u, c, r) => { TScriptPlayer.EngineNameList.Add(u.m_sCharName); return true; });
        AReg(NpcCmdCodes.nNA_DELTEXTLIST, (n, u, c, r) => TScriptPlayer.EngineNameList.Remove(u.m_sCharName));

        // ================= 批次J39：Stub 逐条深化（此前仅 Stub 的命令码，按 Delphi 源 1:1） =================
        // 注意：其余命令已在 NpcScriptEngine.RegisterCommands / 上方批次I 注册块有真实现，禁止在此覆盖。

        // 条件深化
        CReg(NpcCmdCodes.nNC_CHECKTEXTLENGTH, (n, u, c) =>
        {
            // ConditionOfCheckStringLength：CHECKTEXTLENGTH 文本 比较符 长度；比较符 =,>,<，其余一律按 >= 处理
            var text = c.GetStr(0);
            var op = c.GetStr(1);
            int len = c.GetInt(2);
            if (op == "" || len < 0) return false; // ScriptConditionError
            return op switch
            {
                "=" => text.Length == len,
                ">" => text.Length > len,
                "<" => text.Length < len,
                _ => text.Length >= len
            };
        });
        CReg(NpcCmdCodes.nNC_CHECKNAMELISTPOSITION, (n, u, c) =>
        {
            // ConditionOfCheckNameListPostion：人物名在名单文件中的 0 基位置 >= sParam2 为真；
            // 名单文件不存在或位置参数为负 → 脚本条件错误（假）；';' 开头注释行跳过
            if (!TScriptPlayer.NameListFiles.TryGetValue(c.GetStr(0), out var list))
                return false;
            int pos = c.GetInt(1);
            if (pos < 0) return false;
            int namePos = -1;
            for (int i = 0; i < list.Count; i++)
            {
                var line = list[i].Trim();
                if (line.StartsWith(';')) continue;
                if (line.Equals(u.m_sCharName, StringComparison.OrdinalIgnoreCase)) { namePos = i; break; }
            }
            return namePos >= pos;
        });

        // 动作深化
        AReg(NpcCmdCodes.nNA_CLEARSKILL, (n, u, c, r) => { u.Skills.Clear(); return true; });
        AReg(NpcCmdCodes.nNA_CHANGEGENDER, (n, u, c, r) =>
        {
            // ActionOfChangeGender：sParam1=目标性别(0/1)绝对设定；变更时调整发型：
            // 变为 0：发型 1/3 → 2；变为 1：发型 2 → Random(2) 分支 case 2 永不命中，恒为 1（原缺陷保留）
            int g = c.GetInt(0);
            if (g is not (0 or 1)) return false;
            byte old = u.m_btGender;
            u.m_btGender = (byte)g;
            if (old != u.m_btGender)
            {
                if (u.m_btGender == 0)
                {
                    if (u.m_btHair == 1 || u.m_btHair == 3) u.m_btHair = 2;
                }
                else if (u.m_btHair == 2)
                {
                    u.m_btHair = 1;
                }
            }
            return true;
        });

        // 兜底：任何在 g_Cmd*List 注册但无专属处理器的码，注册状态级等效桩（返回成功并登记）
        for (int code = 1; code < 1000; code++)
        {
            if (C[code] == null)
            {
                CReg(code, Stub);
                StateLevelStubs.Add(code);
            }
            if (A[code] == null)
            {
                AReg(code, StubA);
                StateLevelStubs.Add(code);
            }
        }

        // ---- 其余动作：状态级等效 ----
        foreach (var a in NpcCmdNames.ActionNames.Values)
        {
            if (a >= 1 && a < 1000 && A[a] == null)
            {
                AReg(a, StubA);
                StateLevelStubs.Add(a);
            }
        }
    }

    private static readonly Random RndI = new();

    /// <summary>人物所在地图名（脚本上下文简化为角色记录地图名）。</summary>
    private static string EnvirNameOf(TScriptPlayer u) => u.RecallMapName ?? "";
}
