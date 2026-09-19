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

    /// <summary>g_SellPlayerList：寄售离线玩家名单（Search 语义：找到返回 true + 索引）。</summary>
    public static readonly List<string> g_SellPlayerList = new();

    /// <summary>TStringList.Search 等效。</summary>
    public static bool SearchSellPlayer(string name, out int index)
    {
        index = g_SellPlayerList.IndexOf(name);
        return index >= 0;
    }
}
