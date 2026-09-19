using System;
using System.Collections.Generic;

namespace GXX.Client.GUI.GameConfig.Seams;

/// <summary>
/// 接缝：Grobal2.pas:3191 的 <c>TUnBindItemType = (t_UnKnow, t_HP, t_MP, t_Special, t_Book, t_Poison, t_Bujuk)</c>。
/// 序号即原文顺序。
/// </summary>
public enum TUnBindItemType
{
    t_UnKnow = 0,
    t_HP = 1,
    t_MP = 2,
    t_Special = 3,
    t_Book = 4,
    t_Poison = 5,
    t_Bujuk = 6,
}

/// <summary>
/// 接缝：Grobal2.pas:3420 的 <c>TUnBindItem</c>（内挂自定义绑定物品）。
/// 原文为 packed record { sItemName:string[ITEM_NAME_LEN]; nStdMode:Integer; nShape:Integer;
/// UnBindItemType:TUnBindItemType; nCount:Integer; }。
/// ConfigShare 只读 sItemName / UnBindItemType / nShape，其余字段按原文保留。
/// </summary>
public sealed class TUnBindItem
{
    /// <summary>原文 sItemName:string[ITEM_NAME_LEN]。</summary>
    public string sItemName = "";
    /// <summary>原文 nStdMode:Integer。</summary>
    public int nStdMode;
    /// <summary>原文 nShape:Integer。</summary>
    public int nShape;
    /// <summary>原文 UnBindItemType:TUnBindItemType。</summary>
    public TUnBindItemType UnBindItemType;
    /// <summary>解包物品扩展 chongchong 2014-05-15</summary>
    public int nCount;
}

/// <summary>
/// 接缝：Grobal2.pas:3583 的 <c>TStdItem</c>。
/// **只保留 ConfigShare/FilterItems 实际读写的 6 个字段**
/// （Name / StdMode / Shape / Reserved / AC1 / MAC1）；其余字段待 Grobal2.pas 完整移植后并入。
/// 字段类型严格按原文：StdMode:Byte、Shape:Word、Reserved:Byte、AC1/MAC1:Integer。
/// </summary>
public sealed class TStdItemSeam
{
    /// <summary>原文 Name:string[ITEM_NAME_LEN]。</summary>
    public string Name = "";
    /// <summary>原文 StdMode:Byte。</summary>
    public byte StdMode;
    /// <summary>原文 Shape:Word。</summary>
    public ushort Shape;
    /// <summary>原文 Reserved:Byte。</summary>
    public byte Reserved;
    /// <summary>原文 AC1:Integer。</summary>
    public int AC1;
    /// <summary>原文 MAC1:Integer。</summary>
    public int MAC1;
}

/// <summary>
/// 接缝：Grobal2.pas:4151 的 <c>TClientItem</c>（客户端包裹物品）。
/// 原文 <c>TClientBagItems = array [0..ALL_BAG_ITEM_COUNT-1] of TClientItem</c>；
/// ConfigShare 只访问 <c>g_ItemArr[I].s.*</c>（以及 <c>g_ItemArr[I].MakeIndex = 0</c>
/// 在 DamageHPUseItem 中，见 MirsConfigDlg），这里保留 s 与 MakeIndex。
/// </summary>
public sealed class TClientItemSeam
{
    /// <summary>原文 <c>s: TStdItem</c>（注意原文小写字段名 s）。</summary>
    public TStdItemSeam s = new TStdItemSeam();
    /// <summary>原文 MakeIndex:Integer。</summary>
    public int MakeIndex;
}

/// <summary>
/// 接缝：MShare.pas:1864 的 <c>g_MySelf:THumActor</c>。
/// ConfigShare/FilterItems 只用到"是否存在"与当前坐标、用户名；其余字段待 Actor 移植后并入。
/// </summary>
public interface IHumActorSeam
{
    int m_nCurrX { get; }
    int m_nCurrY { get; }
    string m_sUserName { get; }
}

/// <summary>
/// 接缝：MShare.pas:1865 的 <c>g_MyHero:THeroActor</c>。
/// ConfigShare 只读 <c>m_nBagCount</c>（英雄包裹上限，用于和 HeroBagItemCount 比较）。
/// </summary>
public interface IHeroActorSeam
{
    int m_nBagCount { get; }
}

/// <summary>
/// 接缝：MShare.pas / Grobal2.pas 中配置层所依赖的全局状态（g_ItemArr / g_HeroItemArr /
/// g_UnbindItemList / g_MySelf / g_MyHero / g_ExtBagOpenItemCount / g_MagicList）。
///
/// 这些全局在原文里由 MShare.pas 的 initialization 段建立，本车道只做配置对话框族，
/// 故以静态可注入字段表达；测试通过直接赋值验证 Find* 的边界行为。
/// </summary>
public static class ConfigShareSeam
{
    /// <summary>原文 Grobal2.pas:39 <c>DEF_MAX_BAG_ITEM = 46</c>（人物包裹物品数）。</summary>
    public const int DEF_MAX_BAG_ITEM = 46;

    /// <summary>原文 Grobal2.pas:45 <c>MAX_HERO_BAG_ITEM = 40</c>（英雄包裹）。</summary>
    public const int MAX_HERO_BAG_ITEM = 40;

    /// <summary>接缝：MShare.pas:1881 <c>g_ItemArr:TClientBagItems</c>。</summary>
    public static TClientItemSeam[] g_ItemArr = Array.Empty<TClientItemSeam>();

    /// <summary>接缝：MShare.pas:1884 <c>g_HeroItemArr:TClientHeroBagItems</c>。</summary>
    public static TClientItemSeam[] g_HeroItemArr = Array.Empty<TClientItemSeam>();

    /// <summary>接缝：MShare.pas:2223 <c>g_UnbindItemList:TList</c>（内挂自定义绑定物品）。</summary>
    public static List<TUnBindItem> g_UnbindItemList = new List<TUnBindItem>();

    /// <summary>接缝：MShare.pas:1864 <c>g_MySelf:THumActor</c>。</summary>
    public static IHumActorSeam g_MySelf;

    /// <summary>接缝：MShare.pas:1865 <c>g_MyHero:THeroActor</c>。</summary>
    public static IHeroActorSeam g_MyHero;

    /// <summary>接缝：MShare.pas:1879 <c>g_ExtBagOpenItemCount:Word = 0</c>（扩展包裹开启数）。</summary>
    public static ushort g_ExtBagOpenItemCount;

    /// <summary>
    /// 原文 MShare.pas:11764 <c>GetMaxBagCount := DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount;</c>。
    /// </summary>
    public static int GetMaxBagCount() => DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount;

    /// <summary>
    /// 原文 MShare.pas 的 <c>HumBagNoUseItemCount</c>：统计 <c>g_ItemArr[Low..GetMaxBagCount-1]</c> 中空名槽位数。
    /// </summary>
    public static int HumBagNoUseItemCount()
    {
        int Result = 0;
        for (int I = 0 /*Low(g_ItemArr) = 0*/; I <= GetMaxBagCount() - 1; I++)
        {
            if (g_ItemArr[I].s.Name == "")
                Result++;
        }
        return Result;
    }

    /// <summary>
    /// 原文 MShare.pas 的 <c>HeroBagItemCount</c>：统计 <c>g_HeroItemArr</c> 中非空名槽位数。
    /// </summary>
    public static int HeroBagItemCount()
    {
        int Result = 0;
        for (int I = 0 /*Low(g_HeroItemArr)*/; I <= g_HeroItemArr.Length - 1 /*High(g_HeroItemArr)*/; I++)
        {
            if (g_HeroItemArr[I].s.Name != "")
                Result++;
        }
        return Result;
    }
}

/// <summary>
/// 接缝：MShare.pas 的 <c>GetNextDirection</c>（8 向方位，0=上 … 7=左上，与 Delphi 端同序）。
/// 未移植，由测试注入。
/// </summary>
public static class NextDirectionSeam
{
    /// <summary>接缝：待 MShare.pas 的 GetNextDirection 移植后接入（当前可注入以驱动 GetActorDir/Hint 测试）。</summary>
    public static Func<int, int, int, int, int> GetNextDirection = (x1, y1, x2, y2) => 0;
}

/// <summary>
/// 接缝：MShare.pas 的 <c>DScreen.AddChatBoardString(sMsg:string; nColor:Integer; btBackColor:TColor)</c>。
/// 未移植；由测试注入以捕获聊天栏输出（Hint 的可见行为）。
/// </summary>
public static class ChatBoardSeam
{
    /// <summary>接缝：待 DScreen（DrawScrn）移植后接入。</summary>
    public static Action<string, int, int> AddChatBoardString = (msg, color, backColor) => { };
}
