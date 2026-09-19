using System.Collections.Generic;
using System.Globalization;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewList2.pas 单元级全局（批次J3 建立，批次J57 接入套装组实体）。
/// SelGroupItem 为"当前选中套装"（Delphi pTGroupItem）。
/// </summary>
public static class ViewList2State
{
    /// <summary>SelGroupItem: pTGroupItem = nil。</summary>
    public static TGroupItemModel? SelGroupItem;

    /// <summary>
    /// SelAttackSkillPercent: array[1..114] of Byte（套装组窗体编辑用范围，GroupItemSkillPowerConfig.pas 1:1）。
    /// 底层容器扩到 [1..550]（TSkillPowerItem 的 DEF_MAGIC_COUNT + CUSTOM_MAGIC_COUNT，M2Share.pas 807）。
    /// </summary>
    public static readonly byte[] SelAttackSkillPercent = new byte[551];

    /// <summary>SelDefenseSkillPercent: array[1..114] of Byte（同上，容器 [1..550]）。</summary>
    public static readonly byte[] SelDefenseSkillPercent = new byte[551];

    /// <summary>FAcutionPricesLime（窗体级拍卖价格区间，cbbAuctionPricesType 5 档）。</summary>
    public static readonly uint[] AuctionPricesLimeMin = new uint[5];
    public static readonly uint[] AuctionPricesLimeMax = new uint[5];

    /// <summary>单元全局（ViewList2.pas 内嵌 var 段）。</summary>
    public static readonly TItemRules g_ItemRules = new();
    public static readonly TUserCmds g_UserCmds = new();
    public static readonly TItemEffects g_ItemEffects = new();
    public static readonly TGroupItems g_GroupItems = new();
    public static readonly TBoxsList g_BoxsList = new();
    public static readonly List<string> g_EffectImageList = new();
    public static readonly List<string> g_SkillPowerItemList = new();

    /// <summary>g_EffectItemList / g_EffectItemNameList（特效物品名单：物品名 → 特效编号）。</summary>
    public static readonly List<(string ItemName, int EffectIndex)> g_EffectItemList = new();

    /// <summary>g_SendaShopList 之外的备注文本表（MemoTzItemDesc/MemoItemDesc/mmoItemDescTop）。</summary>
    public static readonly List<string> g_TzItemDescList = new();
    public static readonly List<string> g_ItemDescList = new();
    public static readonly List<string> g_ItemDescTopList = new();

    /// <summary>ResetForTests：清空全部单元级状态（测试隔离）。</summary>
    public static void ResetForTests()
    {
        SelGroupItem = null;
        System.Array.Clear(SelAttackSkillPercent);
        System.Array.Clear(SelDefenseSkillPercent);
        System.Array.Clear(AuctionPricesLimeMin);
        System.Array.Clear(AuctionPricesLimeMax);
        g_ItemRules.Clear();
        g_ItemEffects.Clear();
        g_GroupItems.Clear();
        g_EffectImageList.Clear();
        g_SkillPowerItemList.Clear();
        g_EffectItemList.Clear();
        g_TzItemDescList.Clear();
        g_ItemDescList.Clear();
        g_ItemDescTopList.Clear();
    }
}
