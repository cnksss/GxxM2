// 六个 DB 单元共同依赖的少量宿主侧接缝：
//   * DeliverDateUtil —— 原文 DateUtils 的 UnixToDateTime / DateTimeToUnix
//   * ProcessItemName —— 原文 Grobal2.pas:6380（按物品自定义名规则改写显示名）
//   * GetUserItemBindValue —— 原文 ObjBase/M2Share（判定 TUserItem 绑定选项位）
//   * GetStdItem / GetItemRule / UpdateShopItem / AddGameDataLogItemMove —— DoRun 用到的宿主能力
//
// 这些都属于"未移植的依赖"。按任务书第 6 条，**不顺手移植**，只定义最小接缝，
// 并集中在本文件，便于「待 <单元名> 移植后接入」时一处替换。
// 单测通过 <see cref="DbLayerRunSeam"/> 的可替换委托注入行为，无需真实宿主。

using System;
using GXX.Core.Rtl;

namespace GXX.M2Server.DbLayer;

/// <summary>
/// 原文 DateUtils.pas 的两个函数。
/// <para><c>UnixToDateTime(AValue: Int64): TDateTime</c>：把 Unix 秒（UTC）转成 Delphi 本地时间。</para>
/// <para><c>DateTimeToUnix(ADate: TDateTime): Int64</c>：Delphi 本地时间转 Unix 秒（UTC）。</para>
/// 两者互逆（同一时区偏移），与原库在单机上的往返行为一致。
/// </summary>
public static class DelphiDateUtil
{
    /// <summary>Delphi TDateTime 起点：1899-12-30 00:00:00。</summary>
    private static readonly DateTime DelphiEpoch = new(1899, 12, 30, 0, 0, 0, DateTimeKind.Unspecified);

    /// <summary>DateUtils.UnixToDateTime（本地时区）。</summary>
    public static DateTime UnixToDateTime(long unixSeconds)
    {
        DateTime utc = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
        return utc.ToLocalTime();
    }

    /// <summary>DateUtils.DateTimeToUnix（本地时区 → Unix 秒）。</summary>
    public static long DateTimeToUnix(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
    }

    /// <summary>Delphi TDateTime（双精度天数）↔ DateTime（原文 TDateTime 语义，供后续单元使用）。</summary>
    public static double ToDelphiDate(DateTime value) => (value - DelphiEpoch).TotalDays;

    /// <summary>Delphi TDateTime（双精度天数）→ DateTime。</summary>
    public static DateTime FromDelphiDate(double delphiDate) => DelphiEpoch.AddDays(delphiDate);
}

/// <summary>
/// 接缝：原文 DoRun 用到的宿主能力，以及 Grobal2 的两个自由函数。
/// 全部是可替换委托，默认实现为"无宿主"（GetStdItem 返回 null → DoRun 什么都不做；
/// ProcessItemName 原样返回；GetUserItemBindValue 按位判定）。
/// </summary>
public static class DbLayerRunSeam
{
    /// <summary>原文 <c>UserEngine.GetStdItem(wIndex): pTStdItem</c>。接缝：待 UsrEngn.pas 移植后接入。
    /// 返回既有 <c>GXX.Core.Protocol.TStdItem</c>（Grobal2.Types2.cs:26）的可空引用。</summary>
    public static Func<ushort, GXX.Core.Protocol.TStdItem?> GetStdItem { get; set; } = _ => null;

    /// <summary>原文 <c>Grobal2.ProcessItemName(s: string): string</c>。接缝：待 Grobal2.pas 移植后接入。</summary>
    public static Func<string, string> ProcessItemName { get; set; } = name => name ?? "";

    /// <summary>原文 <c>GetUserItemBindValue(btBindOption, bit): Boolean</c>。接缝：待 ObjBase/M2Share 移植后接入。</summary>
    public static Func<int, int, bool> GetUserItemBindValue { get; set; } = (bindOption, bit) => (bindOption & (1 << bit)) != 0;

    /// <summary>原文 <c>g_ItemRules.Get(wIndex, 9)</c>。接缝：待 ItemRules 移植后接入。</summary>
    public static Func<ushort, int, bool> GetItemRule { get; set; } = (_, _) => false;

    /// <summary>原文 <c>g_M2DataDB.UserShopDB.UpdateItem(...)</c>。接缝：待 TM2DataDB 装配后接入。</summary>
    public static Func<int, int, int, int, int, int, bool> UpdateShopItem { get; set; } = (_, _, _, _, _, _) => false;

    /// <summary>原文 <c>AddGameDataLog(LOG_ItemMove, LOG_ActionNone, latHuman, '0', 0, 0, StdItem.Name, MakeIndex, sMasterName, '个人商店', 0, 0, '店铺-&gt;店铺仓库[到时物品]')</c>。
    /// 接缝：待 M2Share 的 AddGameDataLog 移植后接入。</summary>
    public static Action<string, int, string, string> AddGameDataLogItemMove { get; set; } = (_, _, _, _) => { };

    /// <summary>恢复默认（测试用）。</summary>
    public static void ResetDefaults()
    {
        GetStdItem = _ => null;
        ProcessItemName = name => name ?? "";
        GetUserItemBindValue = (bindOption, bit) => (bindOption & (1 << bit)) != 0;
        GetItemRule = (_, _) => false;
        UpdateShopItem = (_, _, _, _, _, _) => false;
        AddGameDataLogItemMove = (_, _, _, _) => { };
    }
}
