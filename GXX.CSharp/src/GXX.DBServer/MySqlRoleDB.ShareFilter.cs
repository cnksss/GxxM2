using System;
using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// MySqlRoleDB.pas 用到的 DBShare.pas / SysUtils 辅助函数接缝。
/// DBShare.pas 未整体移植（车道4 只做了最小接缝），这里只补本单元确实调用的几个。
/// </summary>
public static class DBShareSeamFilter
{
    /// <summary>
    /// DBShare.pas:1078-1100 `CheckFilterRankingChrName`。
    /// `Result := (Pos(#1, sChrName) > 0) or (Pos(#255, sChrName) > 0)`；
    /// 否则在 g_FilterRankingNameTextList 里找**子串**命中（注意：空串也会命中，因为 Pos('', s) = 1）。
    /// </summary>
    public static bool CheckFilterRankingChrName(string sChrName)
    {
        sChrName ??= "";
        if (sChrName.IndexOf('\u0001') >= 0) return true;
        if (sChrName.IndexOf('\u00FF') >= 0) return true;

        var List = DBShareSeam.g_FilterRankingNameTextList;
        for (int I = 0; I < List.Count; I++)
        {
            string S = List[I];
            if (S.Length == 0) return true;                     // Delphi Pos('', s) = 1 → 命中
            if (sChrName.IndexOf(S, StringComparison.Ordinal) >= 0) return true;
        }
        return false;
    }
}

/// <summary>
/// RoleDB / DBShare 的 `Date2MyDate(Now())`。
/// DBShare.pas 的 Date2MyDate 与 LSShare.pas 同构（Y*10000 + M*100 + D）；
/// 不引用 GXX.LoginSrv（跨项目依赖会引入无谓耦合），此处按同一公式本地实现。
/// </summary>
public static class RoleDbDate
{
    /// <summary>Date2MyDate(Now())：本地日期编码为 YYYYMMDD。</summary>
    public static int Date2MyDate(DateTime Dt) => Dt.Year * 10000 + Dt.Month * 100 + Dt.Day;

    /// <summary>可注入时钟（单测固定日期）。</summary>
    public static Func<DateTime> Now = () => DateTime.Now;
}
