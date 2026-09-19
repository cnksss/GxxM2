namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas g_Config 字段（批次J57，ViewList2.pas 依赖子集）：物品过滤/物品备注/改名支持/
/// 技能威力物品开关/套装组规则。typed-constant 初值与 M2Share.pas 5343-5362、5470-5471 行 1:1。
/// </summary>
public static partial class M2Config
{
    // ---- 客户端物品列表下发（M2Share.pas 2754-2765）----

    /// <summary>boEnablePlayerUseClientPickItems：客户端物品列表（玩家）。</summary>
    public static bool boEnablePlayerUseClientPickItems = false;

    /// <summary>boEnableHeroUseClientPickItems：客户端物品列表（英雄）。</summary>
    public static bool boEnableHeroUseClientPickItems = false;

    /// <summary>boSendFilterItemList：是否下发送过滤物品列表到客户端。</summary>
    public static bool boSendFilterItemList = false;

    /// <summary>boSendItemDescList：是否下发送物品备注列表到客户端。</summary>
    public static bool boSendItemDescList = false;

    /// <summary>boSendItemDescTopList：是否下发送物品置顶备注列表到客户端。</summary>
    public static bool boSendItemDescTopList = false;

    /// <summary>boSendTzItemDescList：是否下发送套装物品备注列表到客户端。</summary>
    public static bool boSendTzItemDescList = false;

    /// <summary>boSingleHint：单件装备属性单独显示。</summary>
    public static bool boSingleHint = false;

    // ---- 改名支持（M2Share.pas 2774-2776）----

    /// <summary>boTZSupportRenameItem：套装支持改名物品。</summary>
    public static bool boTZSupportRenameItem = false;

    /// <summary>boDescSupportRenamItem：备注支持改名物品。</summary>
    public static bool boDescSupportRenamItem = false;

    /// <summary>boNoRenameDescReadDefault：未改名物品读取默认备注。</summary>
    public static bool boNoRenameDescReadDefault = false;

    // ---- 套装组 / 技能威力物品（M2Share.pas 2414、2892-2893）----

    /// <summary>boGroupItemRule：套装组规则（True=按数值，False=按比率）。</summary>
    public static bool boGroupItemRule = false;

    // ---- 战力计算（M2Share.pas g_Config，uCombatPowerUtils.pas 依赖）----

    /// <summary>boOpenCombatPowerCalc：启用战力计算。</summary>
    public static bool boOpenCombatPowerCalc = false;

    /// <summary>boOpenCombatPowerVarCalc：战力计入自定义变量加成。</summary>
    public static bool boOpenCombatPowerVarCalc = false;

    /// <summary>boEnabledBuyShopItemGive：购买商店物品赠送。</summary>
    public static bool boEnabledBuyShopItemGive = false;

    /// <summary>boSkillPowerItemUseHum：装备技能威力对人有效。</summary>
    public static bool boSkillPowerItemUseHum = false;

    /// <summary>boSkillPowerItemUseMon：装备技能威力对怪有效。</summary>
    public static bool boSkillPowerItemUseMon = true;

    /// <summary>typed-constant 初值复位（测试隔离用）。</summary>
    public static void ResetViewList2ConfigDefaults()
    {
        boEnablePlayerUseClientPickItems = false;
        boEnableHeroUseClientPickItems = false;
        boSendFilterItemList = false;
        boSendItemDescList = false;
        boSendItemDescTopList = false;
        boSendTzItemDescList = false;
        boSingleHint = false;
        boTZSupportRenameItem = false;
        boDescSupportRenamItem = false;
        boNoRenameDescReadDefault = false;
        boGroupItemRule = false;
        boOpenCombatPowerCalc = false;
        boOpenCombatPowerVarCalc = false;
        boEnabledBuyShopItemGive = false;
        boSkillPowerItemUseHum = false;
        boSkillPowerItemUseMon = true;
    }
}
