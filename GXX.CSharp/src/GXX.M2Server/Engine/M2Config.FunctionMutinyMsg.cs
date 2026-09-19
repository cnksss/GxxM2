namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J29：FunctionConfig.pas SpiritMutiny 叛变页 / MonSayMsg 怪物发言页 / WeaponMakeLuck 武器炼 luck 页
/// 依赖 g_Config 字段（M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- SpiritMutiny 叛变页 ----
    public static bool boSpiritMutiny = false;
    public static uint dwSpiritMutinyTime = 30 * 60 * 1000;
    public static int nSpiritPowerRate = 2;

    // ---- MonSayMsg 怪物发言页 ----
    public static bool boMonSayMsg = false;

    // ---- WeaponMakeLuck 武器炼 luck 页 ----
    public static int nWeaponMakeUnLuckRate = 20;
    public static int nWeaponMakeLuckPoint1 = 1;
    public static int nWeaponMakeLuckPoint2 = 3;
    public static int nWeaponMakeLuckPoint3 = 7;
    public static int nWeaponMakeLuckPoint2Rate = 6;
    public static int nWeaponMakeLuckPoint3Rate = 10 + 30;

    /// <summary>typed-constant 初值复位（叛变/怪物发言/炼 luck 页字段，测试隔离用）。</summary>
    public static void ResetFunctionMutinyMsgDefaults()
    {
        boSpiritMutiny = false;
        dwSpiritMutinyTime = 30 * 60 * 1000;
        nSpiritPowerRate = 2;
        boMonSayMsg = false;
        nWeaponMakeUnLuckRate = 20;
        nWeaponMakeLuckPoint1 = 1;
        nWeaponMakeLuckPoint2 = 3;
        nWeaponMakeLuckPoint3 = 7;
        nWeaponMakeLuckPoint2Rate = 6;
        nWeaponMakeLuckPoint3Rate = 10 + 30;
    }
}
