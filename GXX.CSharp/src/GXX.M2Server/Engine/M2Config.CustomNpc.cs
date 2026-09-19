namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas g_Config 字段（批次J66，uFrmCustomNpc.pas 依赖子集）：
/// 自定义 NPC 下发开关、移动间隔、登录器配置文件名。
/// </summary>
public static partial class M2Config
{
    /// <summary>boSendCustomNpcConfig：下发自定义 NPC 配置。</summary>
    public static bool boSendCustomNpcConfig = false;

    /// <summary>dwCustomNpcMoveTime：自定义 NPC 移动间隔。</summary>
    public static int dwCustomNpcMoveTime = 10;

    /// <summary>sCustomNpcClientConfigFileName：上次另存的自定义 NPC 登录器配置路径。</summary>
    public static string sCustomNpcClientConfigFileName = "";

    /// <summary>typed-constant 初值复位（测试隔离用）。</summary>
    public static void ResetCustomNpcConfigDefaults()
    {
        boSendCustomNpcConfig = false;
        dwCustomNpcMoveTime = 10;
        sCustomNpcClientConfigFileName = "";
    }
}
