namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J36：MonsterConfig.pas 巨片第一片（Open 骨架 + 常规信息组 + 怪物时序组）
/// 依赖 g_Config 字段（M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- 常规信息组 ----
    public static uint dwMonButchDelayClearTime = 60;     // 挖肉时延长怪物尸体清理时间
    public static bool boNoHumanClearMon = false;         // 自动清除无人地图怪物
    public static uint dwNoHumanClearMonTime = 60;        // 自动清除无人地图怪物清理间隔
    public static byte btMonsterShowLevel = 0;            // 显示怪物等级
    public static string sMonsterShowFormat = "%s\\[Lv:%d]"; // 怪物等级显示格式
    public static int nMagStruckMonLevel = 50;
    public static int nMagStruckMonDecTime = 800;
    public static int nMagStruckMonDecRandom = 1000;
    public static byte btMonStruckFrameDelayTime = 0;
    public static bool boEnabledMaxMapItemCount = false;
    public static int nMaxMapItemCount = 5;
    public static bool boNotDropOverlapItemAll = false;
    public static int nScatterItemRange = 3;              // 爆物品范围
    public static int nMonOneDropGoldCount = 2000;
    public static bool boDropGoldToPlayBag = true;
    public static int nElfWarriorMonsterDownDelay = 10;

    // ---- 怪物时序组 ----
    public static uint dwMonsterWarrorAttackTime = 1200;
    public static uint dwMonsterWizardAttackTime = 1200;
    public static uint dwMonsterTaoistAttackTime = 1200;
    public static uint dwMonsterWarrorWalkTime = 500;
    public static uint dwMonsterWizardWalkTime = 500;
    public static uint dwMonsterTaoistWalkTime = 500;
    public static int nMonsterNeedMagicItem = 0;           // 人形怪需要毒符
    public static bool boSendCustomMonsterConfig = true;   // 是否发送自定义怪物配置到客户端
    public static bool boDamageLimitation = false;         // 人形怪伤害封顶

    /// <summary>typed-constant 初值复位（MonsterConfig 第一片字段，测试隔离用）。</summary>
    public static void ResetMonsterConfigSliceDefaults()
    {
        dwMonButchDelayClearTime = 60;
        boNoHumanClearMon = false;
        dwNoHumanClearMonTime = 60;
        btMonsterShowLevel = 0;
        sMonsterShowFormat = "%s\\[Lv:%d]";
        nMagStruckMonLevel = 50;
        nMagStruckMonDecTime = 800;
        nMagStruckMonDecRandom = 1000;
        btMonStruckFrameDelayTime = 0;
        boEnabledMaxMapItemCount = false;
        nMaxMapItemCount = 5;
        boNotDropOverlapItemAll = false;
        nScatterItemRange = 3;
        nMonOneDropGoldCount = 2000;
        boDropGoldToPlayBag = true;
        nElfWarriorMonsterDownDelay = 10;
        dwMonsterWarrorAttackTime = 1200;
        dwMonsterWizardAttackTime = 1200;
        dwMonsterTaoistAttackTime = 1200;
        dwMonsterWarrorWalkTime = 500;
        dwMonsterWizardWalkTime = 500;
        dwMonsterTaoistWalkTime = 500;
        nMonsterNeedMagicItem = 0;
        boSendCustomMonsterConfig = true;
        boDamageLimitation = false;
    }
}
