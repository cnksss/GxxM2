using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>TGuildRank：职位（nRankNo=1 掌门，99 普通）。</summary>
public class TGuildRank
{
    public int nRankNo = 99;
    public string sRankName = "";
    public List<string> MemberList = new();
}

/// <summary>pTWarGuild：战争行会（3 小时有效期）。</summary>
public class TWarGuild
{
    public Guild Guild;
    public uint dwWarTick;

    public TWarGuild(Guild g) { Guild = g; dwWarTick = DelphiRTL.GetTickCount(); }
}

/// <summary>
/// Guild.pas TGuild 1:1 核心转换：
/// 成员/职位表（#No 名称&lt;成员&gt; 格式）、战争/联盟/关注列表、建筑/人气/安定/繁荣度。
/// </summary>
public class Guild
{
    public string sGuildName;
    public List<string> NoticeList = new();
    /// <summary>战争列表：行会名 → TWarGuild。</summary>
    public Dictionary<string, TWarGuild> GuildWarList = new(StringComparer.OrdinalIgnoreCase);
    public List<Guild> GuildAllyList = new();
    public List<Guild> GuildAttentionList = new();
    public List<Guild> GuildRequestAllList = new();
    public List<TGuildRank> RankList = new();
    public int nContestPoint;
    public bool boTeamFight;
    public int nMemberMaxLimit = 200;

    public int nBuildPoint;
    public int nAurae;
    public int nStability;
    public int nFlourishing;
    public int nChiefItemCount;

    public Guild(string name) { sGuildName = name; }

    // ---- 成员 ----

    public int Count
    {
        get
        {
            int c = 0;
            foreach (var r in RankList) c += r.MemberList.Count;
            return c;
        }
    }

    public int RankCount => RankList.Count;

    public bool IsMember(string sName)
    {
        foreach (var r in RankList)
            if (r.MemberList.Contains(sName)) return true;
        return false;
    }

    /// <summary>AddMember2：加入 99 号普通职位。</summary>
    public bool AddMember2(string userName)
    {
        TGuildRank? rank18 = null;
        foreach (var r in RankList)
        {
            if (r.nRankNo == 99) { rank18 = r; break; }
        }
        if (rank18 == null)
        {
            rank18 = new TGuildRank { nRankNo = 99, sRankName = "成员" };
            RankList.Add(rank18);
        }
        rank18.MemberList.Add(userName);
        return true;
    }

    public bool DelMember(string sHumName)
    {
        foreach (var r in RankList)
        {
            if (r.MemberList.Remove(sHumName))
                return true;
        }
        return false;
    }

    /// <summary>CancelGuld：仅当只剩一个职位且只有掌门一人时允许解散。</summary>
    public bool CancelGuld(string sHumName)
    {
        if (RankList.Count != 1) return false;
        var rank = RankList[0];
        if (rank.MemberList.Count != 1) return false;
        return rank.MemberList[0] == sHumName;
    }

    /// <summary>UpdateRank：整表更新（原 UpdateRank 校验流）：0=成功 -1=未变化 -2=无掌门职位 -3=掌门名为空。</summary>
    public int UpdateRank(string sRankData)
    {
        var newRanks = new List<TGuildRank>();
        TGuildRank? cur = null;
        int memberCount = 0;

        var lines = sRankData.Replace("\r\n", "\n").Split('\n');
        foreach (var raw in lines)
        {
            var sRankInfo = raw.Trim();
            if (sRankInfo.Length == 0) continue;
            if (sRankInfo[0] == '#')
            {
                // #编号 名称[<...>]：名称截取到 '<'（原 GetValidStr3(sRankName, ['<','>']) 语义）
                string body = sRankInfo.Substring(1);
                int sp = body.IndexOfAny(new[] { ' ', '<' });
                int rankNo = 99;
                string rankName;
                if (sp > 0)
                {
                    rankNo = HUtil32.Str_ToInt(body.Substring(0, sp), 99);
                    rankName = body.Substring(sp + 1).Trim();
                }
                else
                {
                    rankName = body.Trim();
                }
                int lt = rankName.IndexOf('<');
                if (lt >= 0) rankName = rankName.Substring(0, lt).Trim();
                if (cur != null) newRanks.Add(cur);
                cur = new TGuildRank { nRankNo = rankNo, sRankName = rankName };
                continue;
            }
            if (cur == null) continue;
            foreach (var member in sRankInfo.Split(new[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (memberCount > nMemberMaxLimit) break;
                cur.MemberList.Add(member);
                memberCount++;
            }
        }
        if (cur != null) newRanks.Add(cur);

        // 未变化 → -1
        if (RankList.Count == newRanks.Count)
        {
            bool same = true;
            for (int i = 0; i < RankList.Count && same; i++)
            {
                var oldR = RankList[i];
                var newR = newRanks[i];
                if (oldR.nRankNo != newR.nRankNo || oldR.sRankName != newR.sRankName ||
                    oldR.MemberList.Count != newR.MemberList.Count) { same = false; break; }
                for (int j = 0; j < oldR.MemberList.Count; j++)
                {
                    if (oldR.MemberList[j] != newR.MemberList[j]) { same = false; break; }
                }
            }
            if (same) return -1;
        }

        // 首职位必须是 1 号掌门且名称非空
        int result = -2;
        if (newRanks.Count > 0)
        {
            var first = newRanks[0];
            if (first.nRankNo == 1)
                result = first.sRankName != "" ? 0 : -3;
        }
        if (result == 0)
        {
            RankList = newRanks;
        }
        return result;
    }

    /// <summary>GetRankName：成员职位名（返回 "职位名 成员名" 形式），nRankNo 输出职位号。</summary>
    public string GetRankName(string userName, ref int nRankNo)
    {
        string result = "";
        nRankNo = -1;
        foreach (var r in RankList)
        {
            if (r.MemberList.Contains(userName))
            {
                nRankNo = r.nRankNo;
                if (r.sRankName != "")
                    result = r.sRankName + " " + userName;
                else
                    result = userName;
                break;
            }
        }
        return result;
    }

    public string GetChiefName()
    {
        if (RankList.Count > 0 && RankList[0].MemberList.Count > 0)
            return RankList[0].MemberList[0];
        return "";
    }

    public int GetGuildMasterCount()
    {
        if (RankList.Count > 0)
            return Math.Min(RankList[0].MemberList.Count, 2);
        return 0;
    }

    public void GetGuildMasterName(ref string master1, ref string master2)
    {
        master1 = "";
        master2 = "";
        if (RankList.Count > 0 && RankList[0].MemberList.Count > 0)
        {
            master1 = RankList[0].MemberList[0];
            if (RankList[0].MemberList.Count > 1)
                master2 = RankList[0].MemberList[1];
        }
    }

    // ---- 战争 / 联盟 / 关注 ----

    public bool IsAllyGuild(Guild? other)
        => other != null && GuildAllyList.Contains(other);

    /// <summary>IsWarGuild（含联盟连锁判定，原 2023-05-27 重写版）。</summary>
    public bool IsWarGuild(Guild? other)
    {
        if (other == null) return false;
        // 直接敌对
        if (GuildWarList.ContainsKey(other.sGuildName)) return true;
        // 对方的联盟与我敌对
        foreach (var otherAlly in other.GuildAllyList)
            if (otherAlly.GuildWarList.ContainsKey(sGuildName)) return true;
        // 我的联盟与对方敌对
        foreach (var selfAlly in GuildAllyList)
            if (selfAlly.GuildWarList.ContainsKey(other.sGuildName)) return true;
        return false;
    }

    public TWarGuild? AddWarGuild(Guild guild)
    {
        if (guild == null || GuildWarList.ContainsKey(guild.sGuildName)) return null;
        var war = new TWarGuild(guild);
        GuildWarList[guild.sGuildName] = war;
        return war;
    }

    /// <summary>StopWarGuild：删除战争关系。</summary>
    public void StopWarGuild(Guild guild)
    {
        GuildWarList.Remove(guild.sGuildName);
    }

    /// <summary>检查战争到期并清理（原 Run 中每包调用，战争时长 3 小时）。</summary>
    public void CheckWarExpired()
    {
        uint now = DelphiRTL.GetTickCount();
        uint warTime = 3u * 60 * 60 * 1000;
        var expired = new List<string>();
        foreach (var kv in GuildWarList)
            if (now - kv.Value.dwWarTick > warTime)
                expired.Add(kv.Key);
        foreach (var k in expired)
            GuildWarList.Remove(k);
    }

    public bool IsAttentionGuild(Guild other)
        => other != null && GuildAttentionList.Contains(other);

    public bool AddAttentionGuild(Guild other)
    {
        if (other == null || GuildAttentionList.Contains(other)) return false;
        GuildAttentionList.Add(other);
        return true;
    }

    public bool DelAttentionGuild(Guild other) => GuildAttentionList.Remove(other);

    public bool AllyGuild(Guild other)
    {
        if (other == null || GuildAllyList.Contains(other)) return false;
        GuildAllyList.Add(other);
        other.GuildAllyList.Add(this);
        return true;
    }

    public bool DelAllyGuild(Guild other) => GuildAllyList.Remove(other);
}

/// <summary>TGuildManager 1:1。</summary>
public class GuildManager
{
    public List<Guild> GuildList = new();

    public bool AddGuild(string sGuildName, string sChief)
    {
        if (sGuildName == "" || FindGuild(sGuildName) != null) return false;
        var guild = new Guild(sGuildName);
        guild.RankList.Add(new TGuildRank
        {
            nRankNo = 1,
            sRankName = "掌门",
            MemberList = { sChief }
        });
        GuildList.Add(guild);
        return true;
    }

    public bool DelGuild(string sGuildName, out bool isFound)
    {
        isFound = false;
        var guild = FindGuild(sGuildName);
        if (guild == null) return false;
        isFound = true;
        GuildList.Remove(guild);
        return true;
    }

    public Guild? FindGuild(string sGuildName)
    {
        foreach (var g in GuildList)
            if (g.sGuildName.Equals(sGuildName, StringComparison.OrdinalIgnoreCase)) return g;
        return null;
    }

    public Guild? MemberOfGuild(string sName)
    {
        foreach (var g in GuildList)
            if (g.IsMember(sName)) return g;
        return null;
    }

    public void ClearGuildInf() => GuildList.Clear();

    /// <summary>Run：周期检查各行会战争到期。</summary>
    public void Run()
    {
        foreach (var g in GuildList)
            g.CheckWarExpired();
    }
}
