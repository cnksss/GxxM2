namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J30：FunctionConfig.pas OffLine 离线页与 MyShop 个人商铺页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- OffLine 离线页 ----
    public static bool boOffLineLoginSafeArea = false;
    public static byte btOffLineLoginMapName = 1;
    public static string sSetOffLineLoginMapName = "3";
    public static bool boMonNoAttackOffLinePlayer = false; // 怪物不攻击脱机人物

    // ---- MyShop 个人商铺页 ----
    public static int nMaxMyShopSellingItemCount = 7;
    public static int nMaxMyShopStorageItemCount = 7;
    public static bool boOfflineCloseMyShop = false;
    public static bool boProhibitModifyPrices = false; // 个人商店禁止修改价格
    public static bool boUseHeroM2Shop = true;
    public static bool boInfinityStorage = false;      // 自定义仓库
    public static int nInfinityStorageCount = 50;      // 自定义仓库物品容量
    public static bool boMyShopGold = true;            // 允许金币
    public static bool boMyShopGameGold = true;        // 允许元宝
    public static bool boMyShopGameDiamond = true;     // 允许金刚石
    public static bool boMyShopGameGird = true;        // 允许灵符
    public static bool boMyShopGamePoint = true;
    public static bool boOpenSelfShop = true;          // 是否开启摆摊
    public static bool boSafeZoneShop = true;          // 只允许在安全区摆摊
    public static bool boMapShop = false;              // 只允许在指定地图摆摊
    public static bool boShopStallCanNotAttack = false; // 摆摊期间无敌模式
    public static uint dwSellOffGoldTaxRate = 10;          // 金币税收
    public static uint dwSellOffGameGoldTaxRate = 10;      // 元宝税收
    public static uint dwSellOffGameDiamondTaxRate = 10;   // 金刚石税收
    public static uint dwSellOffGameGirdTaxRate = 10;      // 灵符税收
    public static uint dwSellOffGamePointTaxRate = 10;
    public static bool boShopHeadPic = false;          // 头顶显示个人商店图标

    /// <summary>typed-constant 初值复位（离线/商铺页字段，测试隔离用）。</summary>
    public static void ResetFunctionShopDefaults()
    {
        boOffLineLoginSafeArea = false;
        btOffLineLoginMapName = 1;
        sSetOffLineLoginMapName = "3";
        boMonNoAttackOffLinePlayer = false;
        nMaxMyShopSellingItemCount = 7;
        nMaxMyShopStorageItemCount = 7;
        boOfflineCloseMyShop = false;
        boProhibitModifyPrices = false;
        boUseHeroM2Shop = true;
        boInfinityStorage = false;
        nInfinityStorageCount = 50;
        boMyShopGold = true;
        boMyShopGameGold = true;
        boMyShopGameDiamond = true;
        boMyShopGameGird = true;
        boMyShopGamePoint = true;
        boOpenSelfShop = true;
        boSafeZoneShop = true;
        boMapShop = false;
        boShopStallCanNotAttack = false;
        dwSellOffGoldTaxRate = 10;
        dwSellOffGameGoldTaxRate = 10;
        dwSellOffGameDiamondTaxRate = 10;
        dwSellOffGameGirdTaxRate = 10;
        dwSellOffGamePointTaxRate = 10;
        boShopHeadPic = false;
    }
}
