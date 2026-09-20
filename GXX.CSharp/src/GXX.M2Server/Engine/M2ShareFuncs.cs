namespace GXX.M2Server.Engine;

/// <summary>
/// HUtil32.pas / M2Share.pas 展示辅助函数（批次J4b：ViewOnlineHuman 依赖子集）。
/// </summary>
public static class M2ShareFuncs
{
    /// <summary>Delphi Round：银行家舍入（.5 → 偶数）。</summary>
    public static long DelphiRound(double value)
        => (long)Math.Round(value, MidpointRounding.ToEven);

    /// <summary>Delphi IntToSex（HUtil32.pas 1:1）。</summary>
    public static string IntToSex(byte btSex)
    {
        return btSex switch
        {
            0 => "男",
            1 => "女",
            _ => "未知"
        };
    }

    /// <summary>Delphi IntToJob（HUtil32.pas 1:1）。</summary>
    public static string IntToJob(byte btJob)
    {
        return btJob switch
        {
            0 => "战士",
            1 => "法师",
            2 => "道士",
            _ => "未知"
        };
    }

    /// <summary>M2Share.pas CanFilterMsg（恒真——Delphi 原义保留）。</summary>
    public static bool CanFilterMsg(string sMsg) => true;
}

/// <summary>
/// M2Share.pas 全局（批次J4b：ViewOnlineHuman 依赖子集）。
/// </summary>
public static class M2ShareGlobals
{
    /// <summary>g_Config 元宝族名称（typed constant 默认值 1:1）。</summary>
    public static void ResetNames()
    {
        M2Config.sGameGoldName = "元宝";
        M2Config.sGamePointName = "游戏点";
        M2Config.sGameDiamondName = "金刚石";
        M2Config.sGameGirdName = "灵符";
        M2Config.sCreditPointName = "声望";
        M2Config.sPayMentPointName = "秒卡点";
    }

    /// <summary>
    /// g_SellPlayerList（原文 <c>M2Share.pas:8416</c>：<c>g_SellPlayerList: TSellPlayerList;</c>）。
    /// <para>出售/寄售角色列表 —— 生命周期在 svMain：<c>:1636</c> <c>Create</c>、<c>:1637</c> <c>LoadConfig</c>、
    /// <c>:1414</c> <c>AutoLoadSellPlayer</c>、<c>:3180</c> <c>Free</c>；因此托管侧是**可变静态字段**（非 readonly）。</para>
    /// <para>
    /// ★ **接缝臆造修正**（车道 p8-m2-itemprop-misc，批次日 2026-09-20）：本成员原为
    /// <c>List&lt;string&gt; g_SellPlayerList</c> + <c>SearchSellPlayer(name, out index)</c>（<c>IndexOf</c> 语义），
    /// 与原文**类型不符**：原文类型是 <c>TSellPlayerList</c>（SellPlayer.pas:23-47），
    /// 其 <c>Search</c> 是**按角色名二分查找并给出插入位**，另有 <c>Items[I]</c> 记录访问 /
    /// <c>DeleteByIndex</c> / <c>AddSellPlayer</c> / <c>SaveConfig</c> / <c>LoadConfig</c> / <c>AutoLoadSellPlayer</c>
    /// —— <c>List&lt;string&gt;</c> 从类型上无法表达（调用点：<c>UsrEngn.pas</c> 12 处、
    /// <c>ViewOnlineHuman.pas:481</c>、<c>svMain.pas:663-667/1414/1636-1637/3180</c>）。
    /// 现改用正式归属 <see cref="GXX.M2Server.Misc.TSellPlayerList"/>（SellPlayer.pas 1:1 移植）。
    /// </para>
    /// </summary>
    public static GXX.M2Server.Misc.TSellPlayerList g_SellPlayerList = new();
}
