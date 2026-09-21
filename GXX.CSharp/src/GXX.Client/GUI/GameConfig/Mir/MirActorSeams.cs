// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
//
// 本文件补齐 TMirConfigDlg 需要的**演员 / 背包 / 自定义绑定物品**接缝。
//
// ★ 为什么不直接复用 Seams/ConfigSeamsData.cs 的 TClientItemSeam / IHumActorSeam：
//   那是只读文件（不在本车道独占分区内），且缺本单元读的字段：
//     TStdItemSeam  缺 AniCount（DuraWarning 5858/5875/5891/5915/5931/5947 用）
//     TClientItemSeam 缺 Dura / DuraMax（同上）
//     TUnBindItem   缺 boSpecialMP（5219/5236/5266/5284/8239/8283/8332 用）
//     IHumActorSeam 缺 m_Abil.HP/MP/MaxHP/MaxMP 与 m_boDeath / m_boShopStall
//   按 §14.2 不造第三份实现 —— 这里只定义**本单元用到的读取面**，
//   报告 §偏离登记 D-P10-02 给出"由集成方并入基线"的最小改法（3 个字段 + 1 个成员）。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

/// <summary>
/// 接缝：原文 <c>pTCustomBindItem</c>（MShare.pas 的自定义绑定物品）在本单元读到的成员。
/// 逐条：<c>sItemName</c>/<c>sBindItemName</c>/<c>UnBindItemType</c>/<c>boSpecialMP</c>。
/// 基线 <c>TUnBindItem</c>（Seams/ConfigSeamsData.cs:27）有不带 <c>boSpecialMP</c> 的同名前 3 项，
/// 故此处以**接口 + 适配器**桥接，既有对象无需改动。
/// </summary>
public interface IMirCustomBindItem
{
    /// <summary>原文 <c>sItemName</c>（被绑定的物品名）。</summary>
    string sItemName { get; set; }
    /// <summary>原文 <c>sBindItemName</c>（用于绑定的物品名，书/毒符场景）。</summary>
    string sBindItemName { get; set; }
    /// <summary>原文 <c>UnBindItemType</c>（t_UnKnow/t_HP/t_MP/t_Special/t_Book/t_Poison/t_Bujuk）。</summary>
    TUnBindItemType UnBindItemType { get; set; }
    /// <summary>原文 <c>boSpecialMP:Boolean</c>（特殊药品/魔法药标记；★ 基线 TUnBindItem 缺此字段）。</summary>
    bool boSpecialMP { get; set; }
}

/// <summary>接缝元素的默认实现（纯内存）。</summary>
public sealed class MirCustomBindItem : IMirCustomBindItem
{
    public string sItemName { get; set; } = "";
    public string sBindItemName { get; set; } = "";
    public TUnBindItemType UnBindItemType { get; set; }
    public bool boSpecialMP { get; set; }
}

/// <summary>
/// 适配器：把基线 <c>TUnBindItem</c>（Seams/ConfigSeamsData.cs）当 <see cref="IMirCustomBindItem"/> 用。
/// ★ 已知损失（登记 D-P10-02）：基线类型没有 <c>boSpecialMP</c>，
///   故走本适配器时该字段恒为 <c>false</c>（即"特殊药/魔法药"判别退化）。
///   集成方给 <c>TUnBindItem</c> 补上该字段后即可改用直通适配。
/// </summary>
public sealed class MirCustomBindItemAdapter : IMirCustomBindItem
{
    private readonly TUnBindItem _inner;
    public MirCustomBindItemAdapter(TUnBindItem inner) { _inner = inner; }
    public string sItemName { get => _inner.sItemName; set => _inner.sItemName = value; }
    public string sBindItemName { get; set; } = "";
    public TUnBindItemType UnBindItemType { get => _inner.UnBindItemType; set => _inner.UnBindItemType = value; }
    /// <summary>基线 TUnBindItem 缺 boSpecialMP ⇒ 恒 false（见类型注释）。</summary>
    public bool boSpecialMP { get; set; }
}

/// <summary>
/// 接缝：TMirConfigDlg 依赖的**演员与背包**读取面。
/// 全部以"可注入函数/表"表达，测试可直接喂假值与假行；
/// <see cref="ResetForTests"/> 恢复成与原文一致的默认（无演员、空包）。
/// </summary>
public static class MirActorSeam
{
    // ---- g_MySelf / g_MyHero ----
    /// <summary>接缝：原文 <c>g_MySelf &lt;&gt; nil</c>。</summary>
    public static Func<bool> MySelfExists = () => false;
    /// <summary>接缝：原文 <c>g_MyHero &lt;&gt; nil</c>。</summary>
    public static Func<bool> MyHeroExists = () => false;
    /// <summary>接缝：原文 <c>g_MySelf.m_sUserName</c>（16 处，落盘文件名用）。</summary>
    public static Func<string> MySelfUserName = () => "";
    /// <summary>接缝：原文 <c>g_MySelf</c> 的**对象身份**（Struck/HealthChange 用 <c>Actor = g_MySelf</c> 引用判等）。</summary>
    public static Func<object> MySelfObject = () => null;
    /// <summary>接缝：原文 <c>g_MyHero</c> 的对象身份。</summary>
    public static Func<object> MyHeroObject = () => null;
    /// <summary>接缝：原文 <c>g_MySelf.m_boDeath</c>。</summary>
    public static Func<bool> MySelfDeath = () => false;
    /// <summary>接缝：原文 <c>g_MyHero.m_boDeath</c>。</summary>
    public static Func<bool> MyHeroDeath = () => false;
    /// <summary>接缝：原文 <c>g_MySelf.m_boShopStall</c>（摆摊中不自动练功）。</summary>
    public static Func<bool> MySelfShopStall = () => false;

    /// <summary>接缝：原文 <c>g_MySelf.m_Abil.HP</c>。</summary>
    public static Func<int> MySelfHP = () => 0;
    /// <summary>接缝：原文 <c>g_MySelf.m_Abil.MP</c>。</summary>
    public static Func<int> MySelfMP = () => 0;
    /// <summary>接缝：原文 <c>g_MySelf.m_Abil.MaxHP</c>。</summary>
    public static Func<int> MySelfMaxHP = () => 0;
    /// <summary>接缝：原文 <c>g_MySelf.m_Abil.MaxMP</c>。</summary>
    public static Func<int> MySelfMaxMP = () => 0;

    /// <summary>接缝：原文 <c>g_MyHero.m_Abil.HP</c>。</summary>
    public static Func<int> MyHeroHP = () => 0;
    /// <summary>接缝：原文 <c>g_MyHero.m_Abil.MP</c>。</summary>
    public static Func<int> MyHeroMP = () => 0;
    /// <summary>接缝：原文 <c>g_MyHero.m_Abil.MaxHP</c>。</summary>
    public static Func<int> MyHeroMaxHP = () => 0;
    /// <summary>接缝：原文 <c>g_MyHero.m_Abil.MaxMP</c>。</summary>
    public static Func<int> MyHeroMaxMP = () => 0;
    /// <summary>接缝：原文 <c>g_MyHero.m_nBagCount</c>。</summary>
    public static Func<int> MyHeroBagCount = () => 0;

    // ---- 背包 ----
    /// <summary>接缝：原文 <c>g_ItemArr[I].s.Name</c>（人物包裹，DEF_MAX_BAG_ITEM=46 + 扩展）。</summary>
    public static Func<int, string> ItemArrName = _ => "";
    /// <summary>接缝：原文 <c>g_HeroItemArr[I].s.Name</c>（英雄包裹）。</summary>
    public static Func<int, string> HeroItemArrName = _ => "";
    /// <summary>接缝：原文 <c>GetMaxBagCount</c>（= DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount）。</summary>
    public static Func<int> GetMaxBagCount = () => ConfigShareSeam.DEF_MAX_BAG_ITEM;
    /// <summary>接缝：原文 <c>HumBagNoUseItemCount</c>（空槽位数）。</summary>
    public static Func<int> HumBagNoUseItemCount = () => 0;
    /// <summary>接缝：原文 <c>HeroBagItemCount</c>（英雄已用槽位数）。</summary>
    public static Func<int> HeroBagItemCount = () => 0;

    /// <summary>接缝：原文 <c>Low(g_ItemArr)</c> / <c>Low(g_HeroItemArr)</c>（Delphi 恒为 0）。</summary>
    public const int ItemArrLow = 0;
    /// <summary>接缝：原文 <c>High(g_ItemArr)</c>（= GetMaxBagCount - 1）。</summary>
    public static int ItemArrHigh() => GetMaxBagCount() - 1;

    // ---- 自定义绑定物品 ----
    /// <summary>接缝：原文 <c>g_CustomUnbindItemList:TList</c>（元素是 <c>pTCustomBindItem</c>）。</summary>
    public static List<IMirCustomBindItem> CustomUnbindItemList = new List<IMirCustomBindItem>();

    /// <summary>接缝：原文 <c>LoadNGCustomUnbindItemList(sFileName)</c>（MShare.pas，5052-5062 的 Logon 用）。</summary>
    public static Action<string> LoadNGCustomUnbindItemList = _ => { };

    /// <summary>接缝：原文 <c>SBindItemFileName</c>（GlobalString.pas 的资源串常量）。</summary>
    public static string SBindItemFileName = "%s.BindItem.set";

    // ---- 杂项全局 ----
    /// <summary>接缝：MShare.pas <c>g_nMouseX</c>（自动练功落点）。</summary>
    public static int g_nMouseX;
    /// <summary>接缝：MShare.pas <c>g_nMouseY</c>。</summary>
    public static int g_nMouseY;
    /// <summary>接缝：MShare.pas <c>g_IsWaitLogout</c>（小退保护置位）。</summary>
    public static bool g_IsWaitLogout;
    /// <summary>接缝：MShare.pas <c>g_boGJRun</c>（挂机运行中）。</summary>
    public static bool g_boGJRun;

    /// <summary>接缝：ClMain.pas <c>frmMain.Logout</c>（小退保护）。</summary>
    public static Action Logout = () => { };
    /// <summary>
    /// 接缝：ClMain.pas <c>frmMain.AutoTakeOnItem(Magic)</c>（自动练功）。
    /// 每次调用记一条到 <see cref="AutoTakeOnItemCalls"/>（与 MirReturnConfigGlobalSeam.ChangePoisonCharmCalls 同一做法）：
    /// 让"练功到底有没有真的下发"变成**可断言**的事实，而不是只信任调用点。
    /// </summary>
    public static Action<object> AutoTakeOnItem = m => AutoTakeOnItemCalls.Add(m);
    /// <summary>接缝调用留痕：<c>AutoTakeOnItem</c> 的实参序列。</summary>
    public static readonly List<object> AutoTakeOnItemCalls = new List<object>();
    /// <summary>接缝：ClMain.pas <c>frmMain.UseMagic(X, Y, Magic)</c>（自动练功）。</summary>
    public static Action<int, int, object> UseMagic = (_, __, ___) => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.RunGJ</c> 一族的开关（挂机按钮）。</summary>
    public static Action<bool> SetGJRun = _ => { };

    /// <summary>接缝：<c>EatHumSpecialItem</c> / <c>EatHeroSpecialItem</c>（MShare.pas 的吃药原语）。</summary>
    public static Action<object, int> EatHumSpecialItem = (_, __) => { };
    /// <summary>接缝：<c>EatHeroSpecialItem</c>。</summary>
    public static Action<object, int> EatHeroSpecialItem = (_, __) => { };
    /// <summary>接缝：<c>AutoEatItem</c>（MShare.pas；17 处）。</summary>
    public static Action<object, int, object> AutoEatItem = (_, __, ___) => { };
    /// <summary>接缝：<c>AutoHeroEatItem</c>（MShare.pas；15 处）。</summary>
    public static Action<object, int, object> AutoHeroEatItem = (_, __, ___) => { };

    /// <summary>接缝：<c>ProcessFileNameSpecialChar</c>（MShare.pas；9 处）。</summary>
    public static Func<string, string> ProcessFileNameSpecialChar = ConfigSeams.ProcessFileNameSpecialChar;

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        MySelfExists = () => false;
        MyHeroExists = () => false;
        MySelfUserName = () => "";
        MySelfObject = () => null;
        MyHeroObject = () => null;
        MySelfDeath = () => false;
        MyHeroDeath = () => false;
        MySelfShopStall = () => false;
        MySelfHP = () => 0; MySelfMP = () => 0; MySelfMaxHP = () => 0; MySelfMaxMP = () => 0;
        MyHeroHP = () => 0; MyHeroMP = () => 0; MyHeroMaxHP = () => 0; MyHeroMaxMP = () => 0;
        MyHeroBagCount = () => 0;
        ItemArrName = _ => ""; HeroItemArrName = _ => "";
        GetMaxBagCount = () => ConfigShareSeam.DEF_MAX_BAG_ITEM;
        HumBagNoUseItemCount = () => 0;
        HeroBagItemCount = () => 0;
        CustomUnbindItemList = new List<IMirCustomBindItem>();
        LoadNGCustomUnbindItemList = _ => { };
        SBindItemFileName = "%s.BindItem.set";
        g_nMouseX = 0; g_nMouseY = 0; g_IsWaitLogout = false; g_boGJRun = false;
        Logout = () => { };
        AutoTakeOnItemCalls.Clear();
        AutoTakeOnItem = m => AutoTakeOnItemCalls.Add(m);
        UseMagic = (_, __, ___) => { };
        SetGJRun = _ => { };
        EatHumSpecialItem = (_, __) => { };
        EatHeroSpecialItem = (_, __) => { };
        AutoEatItem = (_, __, ___) => { };
        AutoHeroEatItem = (_, __, ___) => { };
        ProcessFileNameSpecialChar = ConfigSeams.ProcessFileNameSpecialChar;
    }
}
