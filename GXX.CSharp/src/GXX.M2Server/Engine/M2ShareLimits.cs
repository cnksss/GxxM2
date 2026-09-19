namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas 资源处理限额全局（批次J4a：FSrvValue 依赖子集，typed constant 默认值 1:1）。
/// </summary>
public static class M2ShareLimits
{
    public static uint g_dwHumLimit = 30;
    public static uint g_dwMonLimit = 30;
    public static uint g_dwZenLimit = 5;
    public static uint g_dwNpcLimit = 5;
    public static uint g_dwSocLimit = 10;
    public static int nDecLimit = 20;

    public static void ResetDefaults()
    {
        g_dwHumLimit = 30;
        g_dwMonLimit = 30;
        g_dwZenLimit = 5;
        g_dwNpcLimit = 5;
        g_dwSocLimit = 10;
        nDecLimit = 20;
    }
}
