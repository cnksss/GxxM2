using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>pTAttackerInfo：攻城申请（行会 + 攻城日期）。</summary>
public class TAttackerInfo
{
    public Guild Guild;
    public string sGuildName = "";
    public DateTime AttackDate;

    public TAttackerInfo(Guild g, DateTime date) { Guild = g; AttackDate = date; }
}

/// <summary>Castle.pas TObjUnit：城堡门/城墙/弓箭手/卫兵槽位。</summary>
public class TObjUnit
{
    public int nX;
    public int nY;
    public string sName = "";
    public bool nStatus;   // 城门开关状态
    public int nHP;
    public TCreature? BaseObj;
}

/// <summary>
/// Castle.pas TUserCastle 1:1 核心转换：
/// 城堡归属/战争状态机（当日申请 → 20 点开战 / StartWar → 3 小时结束）、
/// 攻城区域、占领（GetCastle）、资金（IncRateGold/WithDrawalGolds/ReceiptGolds）。
/// </summary>
public class TUserCastle
{
    public string m_sMapName = "0151";
    public string m_sName;
    public string m_sOwnGuild = "";
    public string m_sHomeMap = "0151";
    public int m_nHomeX = 100;
    public int m_nHomeY = 100;
    public bool m_boStartWar;
    public bool m_boUnderWar;
    public bool m_boShowOverMsg;
    public uint m_dwStartCastleWarTick;
    public readonly List<TAttackerInfo> m_AttackWarList = new();
    public readonly List<Guild> m_AttackGuildList = new();
    public int m_nTotalGold;
    public int m_nTodayIncome;
    public int m_nWarRangeX = 100;
    public int m_nWarRangeY = 100;
    public string m_sPalaceMap = "0151";
    public string m_sSecretMap = "";
    public int m_nPalaceDoorX;
    public int m_nPalaceDoorY;
    public int m_nTechLevel;
    public int m_nPower;

    public Guild? m_MasterGuild;
    public DateTime m_WarDate;
    public DateTime m_ChangeDate = DateTime.MinValue;
    public DateTime m_IncomeToday = DateTime.Today;

    // ---- 批次J2：CastleManage/AttackSabukWallConfig 窗体族依赖（Castle.pas 1:1） ----
    public const int MAXCALSTEGUARD = 4;
    public const int MAXCASTLEARCHER = 12;

    /// <summary>城堡配置子目录（LoadConfigFile 按目录扫描赋值）。</summary>
    public string m_sConfigDir = "";
    /// <summary>城堡所属地图列表（SaveConfigFile 'Defense/CastleMapList'）。</summary>
    public readonly List<string> m_EnvirList = new();
    public readonly TObjUnit m_MainDoor = new();
    public readonly TObjUnit m_LeftWall = new();
    public readonly TObjUnit m_CenterWall = new();
    public readonly TObjUnit m_RightWall = new();
    public readonly TObjUnit[] m_Guard = new TObjUnit[MAXCALSTEGUARD];
    public readonly TObjUnit[] m_Archer = new TObjUnit[MAXCASTLEARCHER];

    /// <summary>攻城战总时长（g_Config.dwCastleWarTime = 3 小时）。</summary>
    
    /// <summary>占领判定最短攻城时长（g_Config.dwGetCastleTime = 10 分钟）。</summary>
    

    public TUserCastle(string name)
    {
        m_sName = name;
        for (int i = 0; i < m_Guard.Length; i++) m_Guard[i] = new TObjUnit();
        for (int i = 0; i < m_Archer.Length; i++) m_Archer[i] = new TObjUnit();
    }

    /// <summary>设置守方行会（初始化/换主）。</summary>
    public void SetMasterGuild(Guild guild)
    {
        m_MasterGuild = guild;
        m_sOwnGuild = guild.sGuildName;
    }

    /// <summary>RequestCastleWar：申请攻城（加入当日攻城列表）。</summary>
    public bool RequestCastleWar(Guild attackGuild, DateTime date)
    {
        if (attackGuild == null) return false;
        foreach (var a in m_AttackWarList)
            if (a.Guild == attackGuild && a.AttackDate.Date == date.Date) return false;
        m_AttackWarList.Add(new TAttackerInfo(attackGuild, date) { sGuildName = attackGuild.sGuildName });
        return true;
    }

    // ---- 批次J2：AddAttackerInfo 三重载 + Save 持久化（Castle.pas 1:1） ----

    /// <summary>InAttackerList：行会是否已在攻城申请列表。</summary>
    public bool InAttackerList(Guild guild)
    {
        foreach (var a in m_AttackWarList)
            if (a.Guild == guild) return true;
        return false;
    }

    /// <summary>AddAttackerInfo(Guild, AttackDate)。</summary>
    public bool AddAttackerInfo(Guild guild, DateTime attackDate)
    {
        if (InAttackerList(guild)) return false;
        var info = new TAttackerInfo(guild, attackDate) { sGuildName = guild.sGuildName };
        m_AttackWarList.Add(info);
        SaveAttackSabukWall();
        return true;
    }

    /// <summary>AddAttackerInfo(Guild, nDay)：AttackDate = Now + nDay 天。</summary>
    public bool AddAttackerInfo(Guild guild, int nDay)
    {
        if (InAttackerList(guild)) return false;
        var info = new TAttackerInfo(guild, DateTime.Now.AddDays(nDay)) { sGuildName = guild.sGuildName };
        m_AttackWarList.Add(info);
        SaveAttackSabukWall();
        return true;
    }

    /// <summary>Save = SaveConfigFile + SaveAttackSabukWall（Castle.pas 1:1）。</summary>
    public void Save()
    {
        SaveConfigFile();
        SaveAttackSabukWall();
    }

    /// <summary>保存攻城申请列表 AttackSabukWall.txt（行 + '       "日期"'）。</summary>
    public void SaveAttackSabukWall()
    {
        string dir = M2Config.sCastleDir + m_sConfigDir;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string fileName = dir + "\\" + "AttackSabukWall.txt";
        var lines = new List<string>();
        foreach (var a in m_AttackWarList)
            lines.Add(a.sGuildName + "       \"" + DelphiDateToStr(a.AttackDate) + "\"");
        try
        {
            File.WriteAllLines(fileName, lines, EncodingInit.GBK);
        }
        catch
        {
            // MainOutMessage('保存攻城信息失败: ' + sFileName);
        }
    }

    /// <summary>保存城堡配置 SabukW.txt（Setup/Defense 节，键序与条件写盘 1:1）。</summary>
    public void SaveConfigFile()
    {
        // Delphi: if g_MapManager.GetMapOfServerIndex(m_sMapName) <> nServerIndex then Exit
        // （多服务器索引概念未移植，本端恒为主服务器）
        string dir = M2Config.sCastleDir + m_sConfigDir;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string fileName = dir + "\\" + "SabukW.txt";
        var ini = new TFastIniFile(fileName);
        if (m_sName != "") ini.WriteString("Setup", "CastleName", m_sName);
        if (m_sOwnGuild != "") ini.WriteString("Setup", "OwnGuild", m_sOwnGuild);
        ini.WriteString("Setup", "ChangeDate", DelphiDateTimeToIni(m_ChangeDate));
        ini.WriteString("Setup", "WarDate", DelphiDateTimeToIni(m_WarDate));
        ini.WriteString("Setup", "IncomeToday", DelphiDateTimeToIni(m_IncomeToday));
        if (m_nTotalGold != 0) ini.WriteInteger("Setup", "TotalGold", m_nTotalGold);
        if (m_nTodayIncome != 0) ini.WriteInteger("Setup", "TodayIncome", m_nTodayIncome);

        string sMapList = "";
        foreach (var m in m_EnvirList) sMapList += m + ",";
        if (sMapList != "") ini.WriteString("Defense", "CastleMapList", sMapList);
        if (m_sMapName != "") ini.WriteString("Defense", "CastleMap", m_sMapName);
        if (m_sHomeMap != "") ini.WriteString("Defense", "CastleHomeMap", m_sHomeMap);
        if (m_nHomeX != 0) ini.WriteInteger("Defense", "CastleHomeX", m_nHomeX);
        if (m_nHomeY != 0) ini.WriteInteger("Defense", "CastleHomeY", m_nHomeY);
        if (m_nWarRangeX != 0) ini.WriteInteger("Defense", "CastleWarRangeX", m_nWarRangeX);
        if (m_nWarRangeY != 0) ini.WriteInteger("Defense", "CastleWarRangeY", m_nWarRangeY);
        if (m_sPalaceMap != "") ini.WriteString("Defense", "CastlePlaceMap", m_sPalaceMap);
        if (m_sSecretMap != "") ini.WriteString("Defense", "CastleSecretMap", m_sSecretMap);
        if (m_nPalaceDoorX != 0) ini.WriteInteger("Defense", "CastlePalaceDoorX", m_nPalaceDoorX);
        if (m_nPalaceDoorY != 0) ini.WriteInteger("Defense", "CastlePalaceDoorY", m_nPalaceDoorY);

        WriteObjUnit(ini, "MainDoor", m_MainDoor, true);
        WriteObjUnit(ini, "LeftWall", m_LeftWall, false);
        WriteObjUnit(ini, "CenterWall", m_CenterWall, false);
        WriteObjUnit(ini, "RightWall", m_RightWall, false);
        for (int i = 0; i < m_Archer.Length; i++) WriteObjUnit(ini, "Archer_" + (i + 1), m_Archer[i], false);
        for (int i = 0; i < m_Guard.Length; i++) WriteObjUnit(ini, "Guard_" + (i + 1), m_Guard[i], false);
        ini.UpdateFile();
    }

    /// <summary>门/墙/守卫槽位键写入（MainDoor 带 Open/HP，其余 HP 仅在有实体时写）。</summary>
    private static void WriteObjUnit(TFastIniFile ini, string prefix, TObjUnit u, bool isMainDoor)
    {
        if (u.nX != 0) ini.WriteInteger("Defense", prefix + "X", u.nX);
        if (u.nY != 0) ini.WriteInteger("Defense", prefix + "Y", u.nY);
        if (u.sName != "") ini.WriteString("Defense", prefix + "Name", u.sName);
        if (u.BaseObj != null)
        {
            if (isMainDoor) ini.WriteString("Defense", prefix + "Open", u.nStatus ? "1" : "0");
            ini.WriteInteger("Defense", prefix + "HP", (int)u.BaseObj.m_wAbil.HP);
        }
    }

    /// <summary>Delphi DateToStr 缺省形态（zh 区域 yyyy-M-d）。</summary>
    internal static string DelphiDateToStr(DateTime d) => d.ToString("yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Delphi TDateTime 双精度天数值字符串（WriteDateTime 文件形态）。</summary>
    internal static string DelphiDateTimeToIni(DateTime d)
    {
        double days = d == DateTime.MinValue ? 0.0 : (d - new DateTime(1899, 12, 30)).TotalDays;
        return days.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>StartWar：当日有申请则开战（攻守双方自动进入行会战争）。</summary>
    public void StartWar(DateTime now)
    {
        if (m_boStartWar || m_boUnderWar) return;
        if (m_IncomeToday.Date != now.Date)
        {
            m_nTodayIncome = 0;
            m_IncomeToday = now;
        }
        m_boStartWar = true;
        m_AttackGuildList.Clear();
        for (int i = m_AttackWarList.Count - 1; i >= 0; i--)
        {
            var info = m_AttackWarList[i];
            if (info.AttackDate.Date == now.Date)
            {
                m_boUnderWar = true;
                m_boShowOverMsg = false;
                m_WarDate = now;
                m_dwStartCastleWarTick = DelphiRTL.GetTickCount();
                m_AttackGuildList.Add(info.Guild);
                m_AttackWarList.RemoveAt(i);
            }
        }
        if (m_boUnderWar && m_MasterGuild != null)
        {
            m_AttackGuildList.Add(m_MasterGuild);
            // 攻守双方进入战争关系
            foreach (var attacker in m_AttackGuildList)
            {
                if (attacker == m_MasterGuild) continue;
                attacker.AddWarGuild(m_MasterGuild);
                m_MasterGuild.AddWarGuild(attacker);
            }
        }
    }

    /// <summary>StopWar：结束攻城（提前结束则快进到结束前提示期）。</summary>
    public void StopWar()
    {
        if (!m_boUnderWar) return;
        if (!m_boShowOverMsg)
        {
            if (DelphiRTL.GetTickCount() - m_dwStartCastleWarTick < M2Config.dwCastleWarTime)
                m_dwStartCastleWarTick = DelphiRTL.GetTickCount() - M2Config.dwCastleWarTime;
            m_boShowOverMsg = true;
        }
        EndWar();
    }

    /// <summary>Run：战争节奏（到期自动结束）。</summary>
    public void Run()
    {
        var now = DateTime.Now;
        if (m_IncomeToday.Date != now.Date)
        {
            m_nTodayIncome = 0;
            m_IncomeToday = now;
            m_boStartWar = false;
        }
        if (!m_boStartWar && !m_boUnderWar)
        {
            // 自动开战：每日 20 点（g_Config.nStartCastlewarTime）
            if (now.Hour == 20)
            {
                StartWar(now);
            }
        }
        if (m_boUnderWar)
        {
            if (DelphiRTL.GetTickCount() - m_dwStartCastleWarTick > M2Config.dwCastleWarTime)
            {
                EndWar();
            }
        }
    }

    /// <summary>测试辅助：将战争计时快进到超时。</summary>
    public void ForceWarElapsedForTest()
    {
        m_dwStartCastleWarTick = DelphiRTL.GetTickCount() - M2Config.dwCastleWarTime - 1;
    }

    private void EndWar()
    {
        m_boUnderWar = false;
        m_AttackGuildList.Clear();
        m_boStartWar = false;
    }

    /// <summary>InCastleWarArea：是否处于攻城区域（同地图 + 矩形范围）。</summary>
    public bool InCastleWarArea(TEnvirnoment envir, int nX, int nY)
    {
        if (envir.sMapName != m_sHomeMap) return false;
        return Math.Abs(nX - m_nHomeX) <= m_nWarRangeX &&
               Math.Abs(nY - m_nHomeY) <= m_nWarRangeY;
    }

    /// <summary>IsMember：成员判定（同地图名在城堡地图内即成员区）。</summary>
    public bool IsMember(TCreature? cert)
    {
        if (cert == null || cert.m_PEnvir == null) return false;
        return cert.m_PEnvir.sMapName == m_sHomeMap;
    }

    public bool IsAttackGuild(Guild? guild)
        => guild != null && m_AttackGuildList.Contains(guild);

    public bool IsDefenseGuild(Guild? guild)
        => guild != null && guild == m_MasterGuild;

    public bool IsMasterGuild(Guild? guild)
        => guild != null && guild == m_MasterGuild;

    /// <summary>CanGetCastle：开战超 10 分钟且皇宫内无敌对存活成员。</summary>
    public bool CanGetCastle(Guild guild, List<TScriptPlayer> palacePlayers)
    {
        if (DelphiRTL.GetTickCount() - m_dwStartCastleWarTick <= M2Config.dwGetCastleTime) return false;
        foreach (var p in palacePlayers)
        {
            if (p.m_boDeath) continue;
            if (m_MasterGuild == null || !m_MasterGuild.IsMember(p.m_sCharName))
            {
                if (guild == null || !guild.IsMember(p.m_sCharName))
                    return false;
            }
        }
        return true;
    }

    /// <summary>GetCastle：占领城堡（更换归属行会并记录日期）。</summary>
    public void GetCastle(Guild guild)
    {
        m_MasterGuild = guild;
        m_sOwnGuild = guild.sGuildName;
        m_ChangeDate = DateTime.Now;
    }

    /// <summary>IncRateGold：税收入账。</summary>
    public void IncRateGold(int nGold)
    {
        if (nGold > 0 && m_nTotalGold + nGold < 2000000000)
            m_nTotalGold += nGold;
    }

    /// <summary>WithDrawalGolds：行会成员提取城堡资金（-1 非成员 / -2 余额不足 / >=0 成功）。</summary>
    public int WithDrawalGolds(TScriptPlayer playObject, int nGold, Guild? playerGuild)
    {
        if (m_MasterGuild == null || playerGuild != m_MasterGuild)
            return -1;
        if (m_nTotalGold < nGold)
            return -2;
        m_nTotalGold -= nGold;
        playObject.Gold += nGold;
        return nGold;
    }

    /// <summary>ReceiptGolds：存入资金。</summary>
    public int ReceiptGolds(TScriptPlayer playObject, int nGold)
    {
        if (nGold <= 0 || playObject.Gold < nGold) return -1;
        playObject.Gold -= nGold;
        m_nTotalGold += nGold;
        return nGold;
    }

    /// <summary>CheckInPalace：是否处于皇宫内。</summary>
    public bool CheckInPalace(int nX, int nY, TCreature cert)
    {
        return m_sPalaceMap != "" && cert.m_PEnvir != null && cert.m_PEnvir.sMapName == m_sPalaceMap;
    }

    /// <summary>GetWarDate：最近战争日期字符串。</summary>
    public string GetWarDate()
        => m_WarDate == DateTime.MinValue ? "" : m_WarDate.ToString("yyyy-MM-dd");
}

/// <summary>TCastleManager 1:1：城堡列表管理。
/// 声明为 <c>partial</c>：让后续车道能在**自己的新文件**里补成员，而无需再改本文件
/// （见台账 §19.6 —— 这是本工程消除"两个写者"风险的既定手法）。</summary>
public partial class TCastleManager
{
    public List<TUserCastle> m_CastleList = new();

    private readonly object _lock = new();

    /// <summary>Delphi Lock/UnLock 临界区等效。</summary>
    public void Lock() => System.Threading.Monitor.Enter(_lock);
    public void UnLock() => System.Threading.Monitor.Exit(_lock);

    /// <summary>
    /// 原文 <c>Castle.pas</c> <c>TCastleManager.GetCastleNameList(List: TStringList)</c>：
    /// 按 <c>m_CastleList</c> 顺序把每个城堡的 <c>m_sName</c> 追加进 <paramref name="list"/>。
    /// 由集成方补齐（车道 `p4-m2-objnpc` 报为"最高投产比补齐点"：补上它，
    /// <c>NpcSeams.GetCastleNameList</c> 就能从空实现改为转调真实现，攻城列表路径即真通）。
    /// </summary>
    public void GetCastleNameList(TStringList list)
    {
        for (int i = 0; i < m_CastleList.Count; i++)
            list.Add(m_CastleList[i].m_sName);
    }

    public void Add(TUserCastle castle) => m_CastleList.Add(castle);

    public int Count => m_CastleList.Count;

    public TUserCastle? FindCastle(string name)
    {
        foreach (var c in m_CastleList)
            if (c.m_sName.Equals(name, StringComparison.OrdinalIgnoreCase)) return c;
        return null;
    }

    /// <summary>GetCastle：按坐标所属地图获取城堡（InCastleWarArea 语义）。</summary>
    public TUserCastle? InCastleWarArea(TEnvirnoment envir, int nX, int nY)
    {
        foreach (var c in m_CastleList)
            if (c.InCastleWarArea(envir, nX, nY)) return c;
        return null;
    }

    public void Run()
    {
        foreach (var c in m_CastleList)
            c.Run();
    }
}
